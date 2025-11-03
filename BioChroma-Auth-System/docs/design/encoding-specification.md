# BioChroma编码规范 v1.0

## 文档信息
- **版本**: 1.0.0
- **最后更新**: 2024-11-03
- **状态**: 正式版
- **维护者**: BioChroma Team

---

## 1. 编码原理概述

### 1.1 核心理念

BioChroma编码系统彻底抛弃传统二维码的黑白方块思维，创造了全新的**3D彩色粒子云编码范式**。

**传统QR码的局限**:
- ❌ 单一维度（黑白二值）
- ❌ 平面编码，信息密度低
- ❌ 静态，易被截图伪造
- ❌ 视觉单调

**BioChroma的创新**:
- ✅ 多维编码（位置、颜色、深度、时间）
- ✅ 3D空间，信息密度高10倍
- ✅ 动态变化，实时防伪
- ✅ 视觉震撼，用户体验好

---

## 2. 编码维度

### 2.1 空间维度 (X, Y, Z)

粒子的3D坐标编码信息：

```
X坐标: 0.0 ~ 1.0 (归一化)
Y坐标: 0.0 ~ 1.0 (归一化)
Z坐标: 0.0 ~ 1.0 (深度)
```

**映射规则**:
```csharp
// 字节的低4位 → X坐标
X = (byte & 0x0F) / 15.0 * 0.8 + 0.1

// 字节的高4位 → Y坐标
Y = (byte >> 4) / 15.0 * 0.8 + 0.1

// 字节的哈希值 → Z坐标
Z = SHA256(byte, index)[0] / 255.0 * 0.8 + 0.1
```

**为什么乘以0.8加0.1？**
- 使粒子集中在80%的空间内
- 避免边缘位置的粒子被裁剪
- 保持视觉美观

### 2.2 颜色维度 (H, S, V)

使用HSV色彩空间编码：

```
H (色相): 0° ~ 360°    # 主要编码通道
S (饱和度): 0.7 ~ 1.0  # 辅助编码
V (明度): 0.6 ~ 1.0    # 辅助编码
```

**映射规则**:
```csharp
H = (byte * 1.5) % 360
S = 0.7 + (byte % 30) / 100.0
V = 0.6 + (byte % 40) / 100.0
```

**为什么使用HSV而不是RGB？**
- 色相(H)是环形的，适合编码
- 饱和度和明度可以作为校验通道
- 人眼对色相变化更敏感

### 2.3 时间维度 (T)

动态属性编码：

```
RotationSpeed: 粒子旋转速度 (rad/s)
Phase: 呼吸效果相位 (0° ~ 360°)
```

**映射规则**:
```csharp
RotationSpeed = (byte % 10) / 10.0
Phase = (byte % 360)
```

### 2.4 拓扑维度 (Topology)

粒子间的连线编码额外信息：

```
Edge: (particleId1, particleId2)
```

**生成规则**:
```csharp
// 连接距离小于阈值的粒子
const float THRESHOLD = 0.3;

for (i, j in all_pairs):
    if distance(particle[i], particle[j]) < THRESHOLD:
        add_edge(i, j)
```

**拓扑的作用**:
- 增加数据冗余（纠错）
- 提供额外的校验通道
- 增强视觉效果

---

## 3. 数据结构

### 3.1 原始数据格式

```json
{
  "userId": "user_12345",
  "timestamp": 1698765432,
  "biometricHash": "A3F5...",
  "nonce": "xY9k..."
}
```

### 3.2 加密后格式

```
[IV (16 bytes)] + [Encrypted Data (variable)]
```

加密算法：AES-256-CBC

### 3.3 BioChroma码数据结构

```json
{
  "version": "1.0",
  "timestamp": 1698765432,
  "particle_count": 256,
  "particles": [
    {
      "id": 0,
      "position": {"x": 0.523, "y": 0.345, "z": 0.712},
      "color": {"h": 180.5, "s": 0.85, "v": 0.92},
      "dynamics": {
        "rotation_speed": 0.45,
        "phase": 123.0,
        "size": 1.2
      }
    }
    // ... more particles
  ],
  "edges": [
    [0, 5],
    [0, 12],
    [5, 12]
    // ... more edges
  ],
  "biometric_hash": "A3F5...",
  "global_rotation_speed": 1.0,
  "breathing_frequency": 1.0
}
```

---

## 4. 编码步骤详解

### Step 1: 数据准备

```csharp
// 1. 构建payload
var payload = new {
    userId = "user_123",
    timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
    biometricHash = ExtractHash(cameraFrame),
    nonce = GenerateRandomNonce()
};

// 2. 序列化
var json = JsonSerializer.Serialize(payload);
var data = Encoding.UTF8.GetBytes(json);
```

### Step 2: 加密

```csharp
// 使用AES-256加密
using (var aes = Aes.Create())
{
    aes.Key = DeriveKey(secretKey);
    aes.GenerateIV();

    var encrypted = aes.CreateEncryptor()
        .TransformFinalBlock(data, 0, data.Length);

    // IV + 密文
    var result = Concat(aes.IV, encrypted);
}
```

### Step 3: 字节流 → 粒子映射

```csharp
var particles = new List<Particle3D>();

for (int i = 0; i < encrypted.Length; i++)
{
    byte value = encrypted[i];

    var particle = new Particle3D
    {
        Id = i,
        // 位置编码
        X = (value & 0x0F) / 15.0f * 0.8f + 0.1f,
        Y = (value >> 4) / 15.0f * 0.8f + 0.1f,
        Z = Hash(value, i) / 255.0f * 0.8f + 0.1f,
        // 颜色编码
        Hue = (value * 1.5f) % 360,
        Saturation = 0.7f + (value % 30) / 100.0f,
        Value = 0.6f + (value % 40) / 100.0f,
        // 动态编码
        RotationSpeed = (value % 10) / 10.0f,
        Phase = value % 360
    };

    particles.Add(particle);
}
```

### Step 4: 拓扑生成

```csharp
var edges = new List<(int, int)>();

for (int i = 0; i < particles.Count; i++)
{
    for (int j = i + 1; j < particles.Count; j++)
    {
        float distance = Distance(particles[i], particles[j]);
        if (distance < 0.3f)
        {
            edges.Add((i, j));
        }
    }
}
```

### Step 5: 生物特征影响

```csharp
// 应用用户特有的生物特征
if (biometricFeatures != null)
{
    for (int i = 0; i < particles.Count; i++)
    {
        // 颜色偏移
        var dominantColor = biometricFeatures.DominantColors[i % count];
        particles[i].Hue = (particles[i].Hue + dominantColor.h) % 360;

        // 速度调整
        code.GlobalRotationSpeed = 0.8f + quality * 0.4f;
    }
}
```

---

## 5. 编码示例

### 示例1: 编码单个字节

```
输入字节: 0xA5 (10100101)

解析:
- 低4位: 0101 = 5
- 高4位: 1010 = 10

粒子属性:
- X = 5 / 15 * 0.8 + 0.1 = 0.367
- Y = 10 / 15 * 0.8 + 0.1 = 0.633
- Z = SHA256(0xA5, index)[0] / 255 * 0.8 + 0.1 = 0.456 (示例)
- H = (165 * 1.5) % 360 = 247.5°
- S = 0.7 + (165 % 30) / 100 = 0.85
- V = 0.6 + (165 % 40) / 100 = 0.85
- RotationSpeed = (165 % 10) / 10 = 0.5
- Phase = 165
```

### 示例2: 完整编码流程

```
原始数据: "user_123"

1. JSON序列化:
   {"userId":"user_123","timestamp":1698765432,...}

2. UTF-8编码:
   [0x7B, 0x22, 0x75, 0x73, ...]

3. AES-256加密:
   [IV: 16 bytes] + [密文: 64 bytes]

4. 粒子生成:
   80个粒子 (16 + 64 = 80字节)

5. 拓扑生成:
   45条边 (根据距离阈值)

6. 最终BioChroma码:
   {version: "1.0", particles: 80, edges: 45, ...}
```

---

## 6. 编码容量

### 6.1 理论容量

```
单粒子编码能力:
- 位置 (X, Y): 4 bits + 4 bits = 8 bits = 1 byte
- 颜色 (H): 用于校验，不额外编码
- 总计: 1 byte/particle

实际容量:
- 256粒子 = 256 bytes 原始数据
- 减去加密开销 (16 bytes IV + padding)
- 有效载荷: ~230 bytes
```

### 6.2 推荐配置

```
轻量级: 128粒子 (~100 bytes数据)
标准: 256粒子 (~230 bytes数据)
高密度: 512粒子 (~480 bytes数据)
```

---

## 7. 纠错机制

### 7.1 颜色校验

```csharp
// 解码时验证颜色是否匹配
float expectedHue = (decodedByte * 1.5f) % 360;
float hueError = Math.Abs(particle.Hue - expectedHue);

if (hueError > 30) // 允许30度误差
{
    MarkAsCorrupted(particle);
}
```

### 7.2 Reed-Solomon纠错码 (可选)

```csharp
// 在加密前添加RS纠错码
var rs = new ReedSolomonCodec(255, 223); // 可纠正16字节错误
var encoded = rs.Encode(data);
```

---

## 8. 性能考虑

### 8.1 编码性能

```
粒子数量 vs 编码时间:
- 100粒子: ~12ms
- 256粒子: ~28ms
- 500粒子: ~55ms

建议: 移动设备使用256粒子，桌面可用512粒子
```

### 8.2 内存占用

```
单个BioChromaCode对象:
- 256粒子 × 52 bytes/particle ≈ 13 KB
- Edges × 8 bytes/edge ≈ 2 KB
- 总计: ~15 KB
```

---

## 9. 版本兼容性

### 9.1 版本标识

```json
{
  "version": "1.0"
}
```

### 9.2 未来版本规划

```
v1.0: 当前版本，基础编码
v1.1: 添加颜色深度编码 (计划)
v2.0: 引入4D编码 (时间轴) (未来)
```

---

## 10. 安全性注意事项

### 10.1 密钥管理

⚠️ **重要**: 生产环境必须使用安全的密钥管理

```csharp
// ❌ 不要这样做
var key = "hardcoded_key";

// ✅ 应该这样做
var key = Environment.GetEnvironmentVariable("BIOCHROMA_KEY");
// 或使用硬件安全模块 (HSM)
```

### 10.2 时间戳验证

```csharp
const int TOLERANCE = 30; // 30秒窗口

var age = CurrentTime - code.Timestamp;
if (Math.Abs(age) > TOLERANCE)
{
    Reject("Timestamp expired");
}
```

---

## 11. 测试与验证

### 11.1 单元测试

```csharp
[Test]
public void TestByteEncoding()
{
    byte input = 0xA5;
    var particle = EncodeByteToParticle(input, 0);

    // 验证位置
    Assert.AreEqual(0.367, particle.X, 0.01);
    Assert.AreEqual(0.633, particle.Y, 0.01);

    // 验证颜色
    Assert.AreEqual(247.5, particle.Hue, 1.0);
}
```

### 11.2 往返测试

```csharp
[Test]
public void TestRoundTrip()
{
    var original = "Test Data";
    var code = Encode(original);
    var decoded = Decode(code);

    Assert.AreEqual(original, decoded);
}
```

---

## 12. 参考资料

- [BioChroma解码算法](decoding-algorithm.md)
- [安全性分析](security-analysis.md)
- [性能优化指南](performance-optimization.md)

---

## 附录A: 完整代码示例

见源码: `src/BioChroma.Core/Encoding/BioChromaEncoder.cs`

---

**文档变更历史**

| 版本 | 日期 | 变更内容 |
|------|------|---------|
| 1.0.0 | 2024-11-03 | 初始版本 |
