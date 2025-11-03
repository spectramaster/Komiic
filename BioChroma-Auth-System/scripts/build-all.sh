#!/bin/bash

###############################################################################
# BioChroma - 全平台编译脚本
#
# 用途: 一键编译所有支持的平台
# 作者: BioChroma Team
# 日期: 2024-11-03
###############################################################################

set -e  # 遇到错误立即退出

# 颜色定义
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# 配置
BUILD_CONFIG="Release"
OUTPUT_DIR="./dist"
VERSION="1.0.0"

echo -e "${BLUE}========================================${NC}"
echo -e "${BLUE}  BioChroma 全平台编译脚本 v${VERSION}${NC}"
echo -e "${BLUE}========================================${NC}"
echo ""

# 检查dotnet是否安装
if ! command -v dotnet &> /dev/null; then
    echo -e "${RED}错误: 未找到.NET SDK${NC}"
    echo "请访问 https://dotnet.microsoft.com/download 安装"
    exit 1
fi

echo -e "${GREEN}✓ .NET SDK 已安装${NC}"
dotnet --version
echo ""

# 创建输出目录
mkdir -p "$OUTPUT_DIR"

# 函数：编译指定平台
build_platform() {
    local platform=$1
    local runtime=$2
    local project=$3

    echo -e "${YELLOW}► 编译 ${platform}...${NC}"

    dotnet publish "$project" \
        -c "$BUILD_CONFIG" \
        -r "$runtime" \
        --self-contained true \
        -p:PublishSingleFile=true \
        -p:Version="$VERSION" \
        -o "$OUTPUT_DIR/$platform" \
        > /dev/null 2>&1

    if [ $? -eq 0 ]; then
        echo -e "${GREEN}✓ ${platform} 编译成功${NC}"
    else
        echo -e "${RED}✗ ${platform} 编译失败${NC}"
        return 1
    fi
}

# 1. 编译 Windows x64
build_platform "Windows-x64" "win-x64" "src/BioChroma.Desktop/BioChroma.Desktop.csproj"

# 2. 编译 macOS Apple Silicon
build_platform "macOS-arm64" "osx-arm64" "src/BioChroma.Desktop/BioChroma.Desktop.csproj"

# 3. 编译 macOS Intel
build_platform "macOS-x64" "osx-x64" "src/BioChroma.Desktop/BioChroma.Desktop.csproj"

# 4. 编译 Linux x64
build_platform "Linux-x64" "linux-x64" "src/BioChroma.Desktop/BioChroma.Desktop.csproj"

# 5. 编译 Android (如果存在)
if [ -f "src/BioChroma.Mobile/BioChroma.Mobile.Android.csproj" ]; then
    echo -e "${YELLOW}► 编译 Android...${NC}"
    dotnet publish src/BioChroma.Mobile/BioChroma.Mobile.Android.csproj \
        -c "$BUILD_CONFIG" \
        -f net8.0-android \
        -o "$OUTPUT_DIR/Android" \
        > /dev/null 2>&1

    if [ $? -eq 0 ]; then
        echo -e "${GREEN}✓ Android 编译成功${NC}"
    else
        echo -e "${YELLOW}⚠ Android 编译跳过${NC}"
    fi
fi

# 6. 编译 iOS (如果存在且在macOS上)
if [[ "$OSTYPE" == "darwin"* ]] && [ -f "src/BioChroma.Mobile/BioChroma.Mobile.iOS.csproj" ]; then
    echo -e "${YELLOW}► 编译 iOS...${NC}"
    dotnet publish src/BioChroma.Mobile/BioChroma.Mobile.iOS.csproj \
        -c "$BUILD_CONFIG" \
        -f net8.0-ios \
        -o "$OUTPUT_DIR/iOS" \
        > /dev/null 2>&1

    if [ $? -eq 0 ]; then
        echo -e "${GREEN}✓ iOS 编译成功${NC}"
    else
        echo -e "${YELLOW}⚠ iOS 编译跳过${NC}"
    fi
fi

echo ""
echo -e "${BLUE}========================================${NC}"
echo -e "${GREEN}✓ 所有平台编译完成！${NC}"
echo -e "${BLUE}========================================${NC}"
echo ""

# 显示输出文件大小
echo -e "${BLUE}输出文件:${NC}"
du -sh "$OUTPUT_DIR"/* 2>/dev/null || echo "无输出文件"

echo ""
echo -e "${GREEN}编译产物位于: $OUTPUT_DIR/${NC}"
echo ""

# 可选：打包为压缩文件
read -p "是否创建压缩包? (y/n) " -n 1 -r
echo
if [[ $REPLY =~ ^[Yy]$ ]]; then
    echo -e "${YELLOW}► 创建压缩包...${NC}"

    cd "$OUTPUT_DIR"

    for dir in */; do
        platform=${dir%/}
        tar -czf "${platform}-v${VERSION}.tar.gz" "$platform"
        echo -e "${GREEN}✓ 已创建 ${platform}-v${VERSION}.tar.gz${NC}"
    done

    cd - > /dev/null

    echo ""
    echo -e "${GREEN}✓ 压缩包创建完成！${NC}"
fi

echo ""
echo -e "${BLUE}完成！${NC}"
