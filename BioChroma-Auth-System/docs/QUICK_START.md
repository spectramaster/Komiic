# 🚀 BioChroma 快速开始指南

> 5分钟快速上手BioChroma 3D验证系统

---

## 第一步：安装

### Windows
```bash
# 下载并解压
# 双击 BioChroma.Desktop.exe
```

### macOS
```bash
# 下载 .dmg 文件
# 拖拽到应用程序文件夹
# 打开 BioChroma.app
```

### Linux
```bash
# 下载 .AppImage
chmod +x BioChroma-Linux-x64.AppImage
./BioChroma-Linux-x64.AppImage
```

---

## 第二步：生成验证码

1. 点击左侧 **"生成验证码"** 按钮
2. 允许摄像头权限
3. 看向摄像头（2秒）
4. 查看右侧生成的3D粒子云

---

## 第三步：扫描验证

1. 点击右侧 **"扫描验证"** 按钮
2. 将摄像头对准屏幕上的验证码
3. 等待识别（约0.5秒）
4. 查看验证结果

---

## 开发者快速集成

### 安装NuGet包
```bash
dotnet add package BioChroma.Core
dotnet add package BioChroma.Rendering
```

### 最小示例
```csharp
using BioChroma.Core;

// 生成
var encoder = new BioChromaEncoder();
var code = await encoder.EncodeAsync(features, "user123");

// 验证
var decoder = new BioChromaDecoder();
var result = await decoder.DecodeAsync(code);

Console.WriteLine($"验证结果: {result.IsValid}");
```

---

## 下一步

- 📖 阅读完整的[用户手册](guides/user-manual.md)
- 👨‍💻 查看[开发者指南](guides/developer-guide.md)
- 🔧 了解[API参考](api/core-api.md)

---

**遇到问题？** 查看[常见问题](guides/faq.md)或联系我们 support@biochroma.dev
