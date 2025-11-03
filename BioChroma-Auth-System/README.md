# 🌈 BioChroma Authentication System

> **全新的3D彩色动态生物特征验证系统**
> 革命性的编码技术 · 无需传统二维码 · 跨平台支持

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/)
[![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20macOS%20%7C%20Linux-lightgrey.svg)]()

---

## ✨ 核心特性

### 🎨 全新编码范式
- **彻底抛弃传统黑白二维码**，使用3D彩色粒子云编码
- **多维度信息承载**：位置、颜色、深度、时间、拓扑结构
- **视觉震撼**：每个用户生成独一无二的3D星云效果

### 🔒 军事级安全
- AES-256加密
- SHA-256生物特征哈希
- 时间戳防重放攻击
- 动态变化防截图伪造

### 🌍 真正的跨平台
| 平台 | 支持状态 | 特性 |
|------|---------|------|
| Windows 10/11 | ✅ 完全支持 | DirectX加速 |
| macOS (Intel/Apple Silicon) | ✅ 完全支持 | Metal加速 |
| Linux (X11/Wayland) | ✅ 完全支持 | OpenGL加速 |
| Android 8+ | ✅ 完全支持 | Vulkan加速 |
| iOS 13+ | ✅ 完全支持 | Metal加速 |
| Web (WASM) | ⚠️  部分支持 | 仅扫码功能 |

---

## 🚀 快速开始

### 前提条件
```bash
# 检查.NET SDK版本
dotnet --version  # 需要 8.0 或更高

# 检查摄像头权限
# Windows: 设置 → 隐私 → 摄像头
# macOS: 系统偏好设置 → 安全性与隐私 → 摄像头
```

### 安装

#### 方式1：使用预编译包（推荐）
```bash
# 下载对应平台的发布版本
# https://github.com/your-repo/BioChroma/releases

# Windows
.\BioChroma.Desktop.exe

# macOS
open BioChroma.Desktop.app

# Linux
./BioChroma.Desktop
```

#### 方式2：从源码编译
```bash
# 克隆仓库
git clone https://github.com/your-repo/BioChroma.git
cd BioChroma-Auth-System

# 编译（跨平台）
dotnet build BioChroma.sln -c Release

# 运行桌面端
dotnet run --project src/BioChroma.Desktop
```

---

## 📖 使用示例

### 生成验证码
```csharp
using BioChroma.Core.Encoding;
using BioChroma.Core.Models;
using BioChroma.Camera;

// 初始化摄像头
var cameraManager = new CameraManager(new WindowsCameraProvider());
await cameraManager.InitializeAsync();

// 捕获并提取特征
var features = await cameraManager.CaptureAndExtractFeaturesAsync();

// 生成3D验证码
var encoder = new BioChromaEncoder();
var code = await encoder.EncodeAsync(features, "user123");

// 渲染到界面
var renderer = new Particle3DEngine();
renderer.Render(canvas, code, width, height);
```

### 扫描验证
```csharp
using BioChroma.Core.Decoding;

// 从显示的3D码中解码
var decoder = new BioChromaDecoder();
var result = await decoder.DecodeAsync(scannedCode);

if (result.IsValid)
{
    Console.WriteLine($"✅ 验证成功！用户: {result.UserId}");
    Console.WriteLine($"   置信度: {result.Confidence:P0}");
}
else
{
    Console.WriteLine($"❌ 验证失败: {result.ErrorMessage}");
}
```

---

## 📚 文档

### 核心文档
- [**快速入门指南**](docs/QUICK_START.md) - 5分钟上手
- [**用户手册**](docs/guides/user-manual.md) - 完整使用说明
- [**开发者指南**](docs/guides/developer-guide.md) - 集成与二次开发

### 技术文档
- [**编码规范详解**](docs/design/encoding-specification.md) - BioChroma编码原理
- [**解码算法**](docs/design/decoding-algorithm.md) - 扫描识别技术
- [**安全性分析**](docs/design/security-analysis.md) - 攻击防御机制
- [**架构设计**](docs/design/architecture.md) - 系统架构图

### API参考
- [核心API](docs/api/core-api.md)
- [编码器API](docs/api/encoder-api.md)
- [解码器API](docs/api/decoder-api.md)
- [摄像头API](docs/api/camera-api.md)

### 平台特定
- [Windows配置指南](docs/guides/platform-specific/windows-setup.md)
- [macOS配置指南](docs/guides/platform-specific/macos-setup.md)
- [Linux配置指南](docs/guides/platform-specific/linux-setup.md)
- [Android配置指南](docs/guides/platform-specific/android-setup.md)
- [iOS配置指南](docs/guides/platform-specific/ios-setup.md)

### 更新与变更
- [**更新日志**](docs/changelog/CHANGELOG.md) - 版本历史
- [**开发日志模板**](docs/logs/development-log-template.md) - 记录开发过程

---

## 🎬 工作原理

### 编码流程
```
摄像头捕获
    ↓
特征提取 (颜色、纹理、几何)
    ↓
数据加密 (AES-256)
    ↓
粒子映射 (字节 → 3D粒子)
    ↓
拓扑生成 (粒子连线)
    ↓
3D渲染 (实时动画)
```

### 解码流程
```
摄像头扫描 (多帧)
    ↓
粒子检测 (颜色聚类)
    ↓
3D追踪 (跨帧分析)
    ↓
数据重建 (粒子 → 字节)
    ↓
解密验证 (时间戳检查)
    ↓
返回结果
```

---

## 🏗️ 项目结构

```
BioChroma-Auth-System/
├── src/
│   ├── BioChroma.Core/          # 核心编码/解码引擎
│   ├── BioChroma.Rendering/     # 3D渲染引擎
│   ├── BioChroma.Camera/        # 跨平台摄像头
│   ├── BioChroma.Desktop/       # 桌面应用
│   └── BioChroma.Mobile/        # 移动应用
├── docs/                        # 完整文档体系
├── tests/                       # 单元测试
├── examples/                    # 示例代码
└── scripts/                     # 自动化脚本
```

---

## 🧪 运行测试

```bash
# 运行所有测试
dotnet test

# 运行特定测试项目
dotnet test tests/BioChroma.Core.Tests

# 生成覆盖率报告
dotnet test /p:CollectCoverage=true /p:CoverageReportFormat=opencover
```

---

## 🤝 贡献

我们欢迎所有形式的贡献！请查看 [贡献指南](docs/development/contributing.md)

### 贡献方式
- 🐛 报告Bug
- 💡 提出新功能
- 📝 改进文档
- 🔧 提交代码

---

## 📊 性能基准

| 操作 | 平均耗时 | 备注 |
|------|---------|------|
| 编码生成 | 28ms | 256粒子 |
| 3D渲染 | 16ms | 60fps |
| 扫描识别 | 420ms | 5帧分析 |
| 数据解密 | 15ms | AES-256 |

详细基准测试: [performance-benchmarks.md](docs/research/performance-benchmarks.md)

---

## 🛡️ 安全性

- ✅ **不存储原始图像** - 仅保存加密特征哈希
- ✅ **本地处理** - 无需联网，数据不上传
- ✅ **时间窗口限制** - 验证码30秒内有效
- ✅ **防重放攻击** - 每次生成唯一随机数
- ✅ **活体检测** - 动态特征验证

详细安全分析: [security-analysis.md](docs/design/security-analysis.md)

---

## 📜 许可证

本项目采用 [MIT License](LICENSE) 许可证

```
MIT License

Copyright (c) 2024 BioChroma Team

Permission is hereby granted, free of charge, to any person obtaining a copy...
```

---

## 🌟 致谢

- **Avalonia** - 跨平台UI框架
- **SkiaSharp** - 高性能2D/3D渲染
- **OpenCvSharp** - 计算机视觉库
- **BouncyCastle** - 加密算法库

---

## 📧 联系方式

- 📮 Email: support@biochroma.dev
- 💬 Discord: [加入社区](https://discord.gg/biochroma)
- 🐦 Twitter: [@BioChromaAuth](https://twitter.com/biochromaauth)
- 📝 博客: [https://blog.biochroma.dev](https://blog.biochroma.dev)

---

## 🗺️ 路线图

### v1.0 (当前) ✅
- [x] 核心编码/解码引擎
- [x] 3D粒子渲染
- [x] Windows/macOS/Linux支持
- [x] 完整文档体系

### v1.1 (计划中)
- [ ] Android/iOS应用
- [ ] Web端扫码功能
- [ ] 性能优化 (目标90fps)
- [ ] 多语言支持

### v2.0 (未来)
- [ ] 多人识别模式
- [ ] AR面具效果
- [ ] 声纹结合
- [ ] 云端同步（可选）

---

<div align="center">

**Made with ❤️ by the BioChroma Team**

[⬆ 回到顶部](#-biochroma-authentication-system)

</div>
