# 更新日志 (Changelog)

所有重要的项目变更都将记录在此文件中。

本文档格式基于 [Keep a Changelog](https://keepachangelog.com/zh-CN/1.0.0/)，
项目遵循 [语义化版本](https://semver.org/lang/zh-CN/)。

---

## [未发布] - Unreleased

### 计划新增
- [ ] Web端扫码功能（WebAssembly）
- [ ] 多人识别模式
- [ ] AR面具特效
- [ ] 多语言支持（中文/英文/日文）

---

## [1.1.1] - 2025-01-03

### 🔒 安全与质量更新

此版本专注于安全加固、代码质量提升和资源管理优化。

### 🔐 安全修复 (Security Fixes)

#### 关键安全改进
- **🔴 可配置加密密钥**: 移除硬编码密钥，支持通过构造函数或环境变量 `BIOCHROMA_KEY` 配置
  - `BioChromaEncoder` 和 `BioChromaDecoder` 现在接受可选的加密密钥参数
  - 使用默认密钥时会输出警告信息
  - 详见 [SECURITY.md](../../SECURITY.md)

- **🔒 加密随机数生成器**: 将 `System.Random` 替换为 `System.Security.Cryptography.RandomNumberGenerator`
  - 影响文件: `BioChromaEncoder.cs:193-201`
  - Nonce生成现在使用密码学安全的随机源

- **🛡️ 输入验证增强**: 全面的输入验证防护
  - 空值检查：所有公共API参数
  - 数据大小限制：最大10MB防止DoS攻击
  - 加密数据验证：最小16字节（IV大小）
  - 影响文件: `BioChromaEncoder.cs:42-46,69-74`, `BioChromaDecoder.cs:40-44,175-178`

#### 新增安全文档
- **📄 SECURITY.md**: 完整的安全策略文档
  - 加密密钥管理最佳实践
  - 生产环境部署检查清单
  - 威胁模型和已知限制
  - 漏洞报告流程

- **📖 README.md更新**: 添加醒目的安全警告和配置指南
  - 生产环境配置步骤
  - 安全密钥生成示例
  - SECURITY.md链接

### 🐛 Bug修复 (Bug Fixes)

#### 资源管理
- **修复Shader资源泄漏** (`Particle3DEngine.cs:112-139`)
  - 使用 `using` 语句确保 `SKShader` 正确释放
  - 修复前: 每帧泄漏多个shader对象
  - 修复后: 零shader泄漏

- **实现IDisposable模式** (`Particle3DEngine.cs:141-158`)
  - 添加完整的 `IDisposable` 实现
  - 正确释放 `SKPaint` 和其他图形资源
  - 添加 `_disposed` 标志防止重复释放和使用已释放对象

#### 数学安全
- **防止除零错误**
  - `Particle3DEngine.cs:127-131`: 透视投影计算
  - `OptimizedParticle3DEngine.cs:187-191`: 快速旋转计算
  - 添加安全检查: 分母 < 0.01 时设为 0.01

#### 输入验证
- **渲染方法参数验证** (`Particle3DEngine.cs:88-99`, `OptimizedParticle3DEngine.cs:49-57`)
  - Canvas空值检查
  - Code空值和粒子数量检查
  - Width/Height正数验证

### ✨ 改进 (Improvements)

#### 代码质量
- **更好的错误消息**: 所有异常现在包含描述性消息
- **防御性编程**: 添加大量安全检查和边界条件处理
- **文档注释**: 更新构造函数和关键方法的XML文档

#### 性能
- **优化资源清理**: `OptimizedParticle3DEngine` 已正确实现资源释放
- **减少内存泄漏**: 所有图形资源现在都正确释放

### 📊 测试覆盖

现有测试全部通过，新增边界条件测试：
- 空值参数测试
- 超大数据测试
- 加密/解密往返测试

### 🔄 迁移指南

#### 从 v1.1.0 升级

**代码更改（可选但推荐）**:

```csharp
// 旧代码 (仍然可用，但会显示警告)
var encoder = new BioChromaEncoder();
var decoder = new BioChromaDecoder();

// 新代码 (推荐 - 方法1: 环境变量)
// 在程序启动前设置: export BIOCHROMA_KEY="your-key"
var encoder = new BioChromaEncoder();
var decoder = new BioChromaDecoder();

// 新代码 (推荐 - 方法2: 显式传递)
var key = GetSecureKeyFromVault(); // 从密钥管理系统获取
var encoder = new BioChromaEncoder(key);
var decoder = new BioChromaDecoder(key);
```

**环境配置（生产环境必需）**:

```bash
# Linux/macOS
export BIOCHROMA_KEY=$(openssl rand -base64 32)

# Windows PowerShell
$env:BIOCHROMA_KEY = [Convert]::ToBase64String((1..32 | % { Get-Random -Max 256 }))
```

**重要提示**:
- ⚠️ 编码器和解码器必须使用相同的密钥
- ⚠️ 密钥更改后，旧的编码将无法解码
- ⚠️ 生产环境必须使用自定义密钥，不能使用默认密钥

### 📁 文件变更统计

```
Modified files:
  src/BioChroma.Core/Encoding/BioChromaEncoder.cs     (+45 -10)
  src/BioChroma.Core/Decoding/BioChromaDecoder.cs     (+30 -6)
  src/BioChroma.Rendering/Particle3DEngine.cs         (+52 -15)
  src/BioChroma.Rendering/OptimizedParticle3DEngine.cs (+14 -3)

New files:
  SECURITY.md                                          (+248)

Updated files:
  README.md                                            (+20)
  docs/changelog/CHANGELOG.md                          (this file)
```

### 🙏 致谢

感谢所有提出安全建议和代码质量反馈的贡献者。

---

## [1.0.0] - 2024-11-03

### 🎉 首次正式发布

这是BioChroma Authentication System的第一个正式版本！

### 新增 (Added)
- ✨ **核心功能**
  - 全新的BioChroma 3D粒子云编码系统
  - 多维度编码：位置、颜色、深度、时间、拓扑
  - 实时3D粒子渲染引擎（60fps）
  - 动态呼吸和旋转动画效果

- 🔐 **安全特性**
  - AES-256加密算法
  - SHA-256生物特征哈希
  - 时间戳防重放攻击（30秒窗口）
  - 动态变化防截图伪造

- 📷 **摄像头支持**
  - 跨平台摄像头访问接口
  - 生物特征提取器
  - 颜色直方图分析
  - 自动质量评估

- 🎨 **渲染功能**
  - SkiaSharp 3D渲染引擎
  - 粒子发光效果
  - 动态连线拓扑
  - 深度感知透视投影

- 🖥️ **桌面应用**
  - Avalonia UI 跨平台界面
  - 生成器和扫描器双模式
  - 实时摄像头预览
  - 响应式布局

- 🌍 **跨平台支持**
  - Windows 10/11 (x64)
  - macOS 11+ (Intel + Apple Silicon)
  - Linux (Ubuntu 20.04+, Fedora 36+)

- 📚 **完整文档**
  - 用户手册
  - 开发者指南
  - API参考文档
  - 编码规范详解
  - 平台配置指南

- 🧪 **测试框架**
  - 单元测试（Core模块）
  - 集成测试（端到端）
  - 性能基准测试

### 变更 (Changed)
- 无（首次发布）

### 修复 (Fixed)
- 无（首次发布）

### 安全 (Security)
- 🔒 实现端到端加密
- 🔒 不存储原始图像数据
- 🔒 本地处理，无需联网
- 🔒 符合GDPR隐私标准

### 性能指标
```
编码生成: 28ms (256粒子)
渲染帧率: 60fps
扫描识别: 420ms (5帧)
内存占用: ~50MB
```

### 技术栈
- .NET 8
- Avalonia 11.2.1
- SkiaSharp 2.88.8
- OpenCvSharp4 4.10.0
- BouncyCastle 2.3.1

---

## [0.9.0-beta] - 2024-10-20

### 新增
- 🧪 Beta测试版本发布
- ✨ 基础编码/解码功能
- 🎨 简单的2D粒子渲染
- 📷 Windows摄像头支持（实验性）

### 已知问题
- ⚠️ macOS摄像头权限问题
- ⚠️ Linux上渲染性能较低
- ⚠️ 部分情况下解码失败率高

---

## [0.5.0-alpha] - 2024-09-15

### 新增
- 🔬 首个Alpha版本
- ✨ 概念验证：3D粒子编码
- 📐 基础数据结构设计

### 变更
- 确定使用HSV色彩空间
- 选择Avalonia作为UI框架

---

## [0.1.0-pre-alpha] - 2024-08-01

### 新增
- 🎬 项目启动
- 📝 初始设计文档
- 🧪 技术可行性研究

---

## 版本说明

### 版本号规则

```
Major.Minor.Patch[-PreRelease]

示例:
- 1.0.0         → 首个正式版
- 1.1.0         → 新增功能（向后兼容）
- 1.1.1         → Bug修复
- 2.0.0         → 重大更新（破坏兼容性）
- 1.2.0-beta.1  → Beta测试版
- 1.2.0-rc.1    → 候选发布版
```

### 更新类型说明

| 类型 | 说明 | 示例 |
|------|------|------|
| **新增 (Added)** | 全新功能 | 添加Android支持 |
| **变更 (Changed)** | 既有功能修改 | 更改默认粒子数量 |
| **弃用 (Deprecated)** | 即将移除的功能 | 旧版API标记弃用 |
| **移除 (Removed)** | 已删除的功能 | 移除实验性功能 |
| **修复 (Fixed)** | Bug修复 | 修复内存泄漏 |
| **安全 (Security)** | 安全相关 | 修复加密漏洞 |

---

## 路线图

### v1.1.0 (预计 2024-12)
- [ ] Android应用发布
- [ ] iOS应用发布
- [ ] Web扫码功能（WASM）
- [ ] 性能优化：目标90fps
- [ ] 多语言界面

### v1.2.0 (预计 2025-02)
- [ ] 云端同步（可选）
- [ ] 团队协作功能
- [ ] 高级安全选项
- [ ] 自定义主题

### v2.0.0 (未来)
- [ ] 多人识别模式
- [ ] AR面具效果
- [ ] 声纹结合验证
- [ ] 企业级部署方案

---

## 贡献指南

想要为BioChroma贡献？请查看 [CONTRIBUTING.md](../development/contributing.md)

### 报告Bug
1. 访问 [GitHub Issues](https://github.com/your-repo/BioChroma/issues)
2. 点击 "New Issue"
3. 选择 "Bug Report" 模板
4. 填写详细信息

### 提出新功能
1. 访问 [GitHub Issues](https://github.com/your-repo/BioChroma/issues)
2. 点击 "New Issue"
3. 选择 "Feature Request" 模板
4. 描述你的想法

---

## 致谢

感谢所有为BioChroma v1.0.0做出贡献的开发者和测试者！

**特别鸣谢:**
- @contributor1 - 性能优化
- @contributor2 - macOS平台支持
- @contributor3 - 文档编写

---

**链接**
- [官方网站](https://biochroma.dev)
- [文档](https://docs.biochroma.dev)
- [GitHub](https://github.com/your-repo/BioChroma)
- [Discord社区](https://discord.gg/biochroma)

---

*本文档持续更新中...*
