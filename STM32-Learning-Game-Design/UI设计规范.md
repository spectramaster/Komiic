# ChipQuest UI/UX 设计规范

## 📚 目录
1. [设计理念](#设计理念)
2. [视觉风格指南](#视觉风格指南)
3. [界面布局详解](#界面布局详解)
4. [交互设计规范](#交互设计规范)
5. [动画效果库](#动画效果库)
6. [响应式设计](#响应式设计)
7. [无障碍设计](#无障碍设计)

---

## 设计理念

### 核心原则

1. **极简主义**（Minimalism）
   - 去除不必要的视觉元素
   - 专注于内容和功能
   - 减少认知负担

2. **即时反馈**（Instant Feedback）
   - 每个操作都有明确的视觉/听觉反馈
   - 加载状态清晰可见
   - 错误提示友好且具体

3. **沉浸式体验**（Immersive）
   - 赛博朋克科技感
   - 流畅的过渡动画
   - 统一的世界观呈现

4. **学习友好**（Learning-Friendly）
   - 清晰的信息层级
   - 渐进式披露（Progressive Disclosure）
   - 帮助信息触手可及

---

## 视觉风格指南

### 配色方案

#### 主色调：科技蓝

```css
/* 主色板 */
--primary-dark: #0A1929;      /* 深蓝背景 */
--primary-blue: #00D9FF;      /* 电光蓝（主要强调色）*/
--primary-purple: #B931FC;    /* 霓虹紫（次要强调色）*/
--primary-green: #00FF88;     /* 荧光绿（成功提示）*/
--primary-red: #FF3366;       /* 红色（错误提示）*/
--primary-gold: #FFD700;      /* 金色（奖励/成就）*/

/* 中性色 */
--neutral-100: #FFFFFF;       /* 纯白（主要文字）*/
--neutral-80: #CCCCCC;        /* 浅灰（次要文字）*/
--neutral-60: #999999;        /* 中灰（禁用状态）*/
--neutral-40: #666666;        /* 深灰（边框）*/
--neutral-20: #1A2332;        /* 深蓝灰（卡片背景）*/
--neutral-10: #0D1117;        /* 极深蓝（面板背景）*/

/* 渐变色 */
--gradient-primary: linear-gradient(135deg, #00D9FF 0%, #B931FC 100%);
--gradient-success: linear-gradient(135deg, #00FF88 0%, #00D9FF 100%);
--gradient-warning: linear-gradient(135deg, #FFD700 0%, #FF8C00 100%);
--gradient-danger: linear-gradient(135deg, #FF3366 0%, #FF6B9D 100%);
```

#### 使用场景

| 颜色 | 使用场景 | 示例 |
|------|---------|------|
| 电光蓝 (#00D9FF) | 主要交互元素、链接、按钮 | "运行代码"按钮 |
| 霓虹紫 (#B931FC) | 次要强调、徽章、特殊状态 | 成就徽章边框 |
| 荧光绿 (#00FF88) | 成功状态、通过提示 | 编译成功提示 |
| 红色 (#FF3366) | 错误、警告、危险操作 | 编译错误下划线 |
| 金色 (#FFD700) | 奖励、星级评分、特殊道具 | 三星评分 |

### 字体系统

#### 字体选择

**代码字体**（等宽）:
```css
font-family: 'Fira Code', 'Consolas', 'Monaco', 'Courier New', monospace;
```
- 用于：代码编辑器、终端输出、寄存器值

**UI字体**（无衬线）:
```css
/* 中文优先 */
font-family: 'Noto Sans SC', 'Microsoft YaHei', 'PingFang SC', 'Helvetica Neue', Arial, sans-serif;

/* 英文优先 */
font-family: 'Roboto', 'Segoe UI', 'Helvetica Neue', Arial, sans-serif;

/* 标题字体（未来感）*/
font-family: 'Orbitron', 'Rajdhani', 'Exo 2', sans-serif;
```

#### 字阶系统

| 级别 | 字号 | 行高 | 用途 |
|-----|------|------|------|
| H1  | 32px | 40px | 章节标题 |
| H2  | 24px | 32px | 关卡标题 |
| H3  | 20px | 28px | 卡片标题 |
| H4  | 16px | 24px | 小标题 |
| Body | 14px | 22px | 正文 |
| Small | 12px | 18px | 辅助文字 |
| Tiny | 10px | 16px | 标签、注释 |

**字重**:
- Light (300): 辅助信息
- Regular (400): 正文
- Medium (500): 强调
- Bold (700): 标题
- Black (900): 超大标题

### 圆角与间距

#### 圆角系统
```css
--radius-none: 0;
--radius-sm: 4px;       /* 小按钮、标签 */
--radius-md: 8px;       /* 卡片、输入框 */
--radius-lg: 12px;      /* 大卡片、对话框 */
--radius-xl: 16px;      /* 特殊容器 */
--radius-full: 9999px;  /* 圆形按钮、头像 */
```

#### 间距系统（8px基准）
```css
--space-xs: 4px;
--space-sm: 8px;
--space-md: 16px;
--space-lg: 24px;
--space-xl: 32px;
--space-2xl: 48px;
--space-3xl: 64px;
```

### 阴影系统

```css
/* 卡片阴影 */
--shadow-sm: 0 2px 4px rgba(0, 217, 255, 0.1);
--shadow-md: 0 4px 8px rgba(0, 217, 255, 0.15);
--shadow-lg: 0 8px 16px rgba(0, 217, 255, 0.2);
--shadow-xl: 0 12px 24px rgba(0, 217, 255, 0.25);

/* 发光效果（科技感）*/
--glow-blue: 0 0 10px rgba(0, 217, 255, 0.5),
             0 0 20px rgba(0, 217, 255, 0.3);
--glow-purple: 0 0 10px rgba(185, 49, 252, 0.5),
               0 0 20px rgba(185, 49, 252, 0.3);
--glow-green: 0 0 10px rgba(0, 255, 136, 0.5),
              0 0 20px rgba(0, 255, 136, 0.3);
```

---

## 界面布局详解

### 1. 主界面（Dashboard）

```
┌─────────────────────────────────────────────────────────────┐
│ [LOGO] ChipQuest    [搜索🔍]          [🔔][⚙️][👤 用户名]     │ 60px
├───────────┬─────────────────────────────────────────────────┤
│           │                                                 │
│  侧边栏    │                主内容区                          │
│  240px    │                                                 │
│           │  ┌─────────────────────────────────────┐        │
│ 🗺️ 世界   │  │  🎯 每日挑战                         │        │
│ 💻 编码室  │  │  • 完成1个关卡                       │        │
│ 🏆 成就   │  │  • 优化代码减少20%内存                │        │
│ 📚 代码库 │  └─────────────────────────────────────┘        │
│ 👥 社区   │                                                 │
│ 🎯 竞技场 │  ┌─────────┐ ┌─────────┐ ┌─────────┐         │
│           │  │ 第1章   │ │ 第2章   │ │ 第3章   │         │
│ ───────   │  │ GPIO    │ │ Timer   │ │ 中断    │         │
│           │  │ ⭐⭐⭐  │ │ ⭐⭐○   │ │ 🔒      │         │
│ 📊 进度   │  │ 5/5 ✓   │ │ 3/5     │ │ 0/5     │         │
│ 45%       │  └─────────┘ └─────────┘ └─────────┘         │
│ ▓▓▓▓▓░░   │                                                 │
│           │  📈 学习统计                                    │
│ 等级 15   │  ┌──────────────────────────┐                  │
│ ⚡1580XP  │  │  本周学习: 3.5小时        │                  │
│ 下一级    │  │  [████████░░] 70%        │                  │
│ 需420XP   │  │  连续学习: 7天 🔥        │                  │
│           │  └──────────────────────────┘                  │
│ 🔹 2340   │                                                 │
│ (晶体)    │                                                 │
└───────────┴─────────────────────────────────────────────────┘
```

#### 布局参数
```css
.dashboard {
    display: grid;
    grid-template-columns: 240px 1fr;
    grid-template-rows: 60px 1fr;
    height: 100vh;
    background: var(--primary-dark);
}

.sidebar {
    background: var(--neutral-10);
    border-right: 1px solid var(--neutral-40);
    padding: var(--space-md);
}

.main-content {
    padding: var(--space-xl);
    overflow-y: auto;
}
```

### 2. 编码界面（最重要）

```
┌─────────────────────────────────────────────────────────────────┐
│ ← 返回  关卡1-3: 流水灯阵        [💡提示] [▶️运行] [⏸️暂停] [🐛调试] │ 60px
├──────────────────────────┬──────────────────────────────────────┤
│                          │                                      │
│   📝 代码编辑器            │   🔧 虚拟硬件                         │
│   (左侧,55%宽度)          │   (右侧,45%宽度)                      │
│                          │                                      │
│  1  #include "stm32..."  │   ┌─────────────────────┐            │
│  2                       │   │  STM32F103C8        │            │
│  3  void main() {        │   │                     │            │
│  4    while(1) {         │   │  💡💡💡💡💡💡💡💡   │            │
│  5 ▶    LED_Shift();     │   │  PA0 1 2 3 4 5 6 7  │ ← 实时动画 │
│  6      Delay(200);      │   │                     │            │
│  7    }                  │   │  [流水灯动画]        │            │
│  8  }                    │   └─────────────────────┘            │
│                          │                                      │
│  ⚠️ 0 错误  ⚡ 0 警告    │   🔍 寄存器监视 (可折叠)              │
│                          │   ┌─────────────────────┐            │
│  [编译输出]              │   │ GPIOA->ODR:         │            │
│  > Build Success         │   │ 0b00000001          │            │
│  > Size: 2.1KB           │   │                     │            │
│                          │   │ TIM2->CNT: 0        │            │
├──────────────────────────┤   └─────────────────────┘            │
│  📋 任务目标 (可折叠)     │                                      │
│  ✅ LED从左到右流动       │   📡 串口输出 (可折叠)               │
│  ✅ 速度200ms            │   ┌─────────────────────┐            │
│  ⬜ 循环3次后停止         │   │ > LED Shift: 1      │            │
│                          │   │ > LED Shift: 2      │            │
│  💡 提示 (剩余3次)       │   │                     │            │
│  [显示下一个提示]         │   └─────────────────────┘            │
└──────────────────────────┴──────────────────────────────────────┘
```

#### 编码界面特性

**左侧编辑器区**:
- 代码编辑器占主要空间
- 底部折叠面板：任务目标、提示、编译输出
- 顶部工具栏：运行、调试、保存

**右侧硬件区**:
- 虚拟开发板3D可视化
- 可切换视图：
  - 标准视图（2D图标）
  - 3D视图（可旋转）
  - 透视视图（看到内部）
- 寄存器监视窗口（折叠）
- 串口/I2C/SPI监视器（折叠）

#### 响应式调整
```css
/* 大屏幕 (>1600px) */
.code-area { width: 55%; }
.hardware-area { width: 45%; }

/* 中等屏幕 (1200-1600px) */
.code-area { width: 60%; }
.hardware-area { width: 40%; }

/* 小屏幕 (<1200px) */
/* 上下布局 */
.code-area { width: 100%; height: 60%; }
.hardware-area { width: 100%; height: 40%; }
```

### 3. 关卡选择界面

```
┌────────────────────────────────────────────────────────────┐
│  第一章: 数字王国 (GPIO Kingdom)                            │
│  进度: 5/5 ✓                                [⭐⭐⭐ 15星]    │
├────────────────────────────────────────────────────────────┤
│                                                            │
│  [地图视觉化路径]                                          │
│                                                            │
│   🏁起点                                                   │
│     │                                                      │
│     ▼                                                      │
│  ┌─────────┐                                              │
│  │  1-1    │                                              │
│  │ 觉醒之光 │  ← 点击进入关卡                               │
│  │ ⭐⭐⭐  │                                              │
│  └────┬────┘                                              │
│       │                                                   │
│       ▼                                                   │
│  ┌─────────┐    ┌─────────┐                              │
│  │  1-2    │────│  1-3    │                              │
│  │ 按键试炼 │    │ 流水灯阵 │                              │
│  │ ⭐⭐⭐  │    │ ⭐⭐○   │                              │
│  └─────────┘    └────┬────┘                              │
│                      │                                    │
│                      ▼                                    │
│                 ┌─────────┐                               │
│                 │  1-4    │                               │
│                 │ 蜂鸣警报 │                               │
│                 │ ⭐⭐○   │                               │
│                 └────┬────┘                               │
│                      │                                    │
│                      ▼                                    │
│                 ┌─────────┐                               │
│                 │ 🔥 BOSS │                               │
│                 │  1-5    │                               │
│                 │ 交通灯  │                               │
│                 │ ⭐○○    │                               │
│                 └─────────┘                               │
│                                                            │
│  [下一章：时间神殿 🔒]                                     │
└────────────────────────────────────────────────────────────┘
```

#### 关卡卡片设计

**未完成关卡**:
```
┌──────────────┐
│   1-3        │
│ 流水灯阵      │  ← 标题
│              │
│ [LED图标]    │  ← 关卡图标
│              │
│ ⭐⭐○        │  ← 当前星级
│ 难度: ⭐⭐   │  ← 难度指示
│              │
│ [开始挑战]   │  ← 按钮
└──────────────┘
```

**已完成关卡**:
```
┌──────────────┐
│   1-1 ✓      │
│ 觉醒之光      │
│              │
│ [LED图标]    │
│              │
│ ⭐⭐⭐       │
│ 用时: 2m30s  │  ← 通关数据
│ 排名: 前15%  │
│              │
│ [重新挑战]   │  ← 可重玩
└──────────────┘
```

**锁定关卡**:
```
┌──────────────┐
│   2-1        │
│ 时钟迷宫      │
│              │
│   🔒         │  ← 大锁图标
│              │
│ 完成1-5解锁  │  ← 解锁条件
└──────────────┘
```

---

## 交互设计规范

### 按钮系统

#### 主要按钮（Primary Button）
```css
.btn-primary {
    background: var(--gradient-primary);
    color: white;
    padding: 12px 24px;
    border-radius: var(--radius-md);
    border: none;
    font-weight: 500;
    cursor: pointer;
    transition: all 0.3s ease;
    box-shadow: var(--glow-blue);
}

.btn-primary:hover {
    transform: translateY(-2px);
    box-shadow: var(--glow-blue), var(--shadow-lg);
}

.btn-primary:active {
    transform: translateY(0);
}
```

**视觉示例**:
```
┌───────────┐       ┌───────────┐       ┌───────────┐
│  运行代码  │  →    │  运行代码  │  →    │  运行代码  │
└───────────┘       └───────────┘       └───────────┘
   默认状态         悬停状态(上移)       点击状态
               (蓝色发光增强)
```

#### 按钮状态

| 状态 | 视觉效果 | 用户反馈 |
|------|---------|---------|
| Default | 渐变背景 + 轻微发光 | - |
| Hover | 上移2px + 发光增强 | 鼠标变手型 |
| Active | 回到原位 | 轻微震动 |
| Loading | 按钮禁用 + 旋转图标 | "正在编译..." |
| Disabled | 灰色 + 无光效 | 鼠标禁止图标 |
| Success | 绿色光效 + ✓图标 | 震动反馈 |
| Error | 红色光效 + ✗图标 | 震动反馈 |

### 输入框系统

```css
.input {
    background: var(--neutral-20);
    border: 1px solid var(--neutral-40);
    color: var(--neutral-100);
    padding: 10px 16px;
    border-radius: var(--radius-md);
    font-size: 14px;
    transition: all 0.3s ease;
}

.input:focus {
    outline: none;
    border-color: var(--primary-blue);
    box-shadow: 0 0 0 3px rgba(0, 217, 255, 0.1);
}

.input.error {
    border-color: var(--primary-red);
    box-shadow: 0 0 0 3px rgba(255, 51, 102, 0.1);
}
```

### 通知系统

#### Toast通知
```
┌────────────────────────────────┐
│ ✅ 编译成功！                   │  ← 成功（绿色）
│    二进制大小: 2.1KB            │
└────────────────────────────────┘

┌────────────────────────────────┐
│ ⚠️ 警告: 未使用的变量 'temp'    │  ← 警告（黄色）
│    在第8行                      │
└────────────────────────────────┘

┌────────────────────────────────┐
│ ❌ 编译失败                     │  ← 错误（红色）
│    未定义的引用: HAL_Delay      │
│    [查看详情]                   │
└────────────────────────────────┘

┌────────────────────────────────┐
│ 💡 提示: 尝试使用循环简化代码   │  ← 提示（蓝色）
└────────────────────────────────┘
```

**Toast动画**:
1. 从右侧滑入（300ms，ease-out）
2. 停留3秒（可点击关闭）
3. 淡出（300ms）

```css
@keyframes slideInRight {
    from {
        transform: translateX(400px);
        opacity: 0;
    }
    to {
        transform: translateX(0);
        opacity: 1;
    }
}

.toast {
    animation: slideInRight 0.3s ease-out;
    position: fixed;
    top: 80px;
    right: 24px;
    padding: 16px 20px;
    border-radius: var(--radius-md);
    box-shadow: var(--shadow-lg);
    min-width: 300px;
}

.toast.success {
    background: var(--neutral-20);
    border-left: 4px solid var(--primary-green);
}

.toast.error {
    background: var(--neutral-20);
    border-left: 4px solid var(--primary-red);
}
```

---

## 动画效果库

### 1. 关卡完成动画

```
步骤1: 虚拟硬件成功光效 (500ms)
┌─────────────┐           ┌─────────────┐
│  💡 LED ●   │    →      │  💡 LED ● ✨│
└─────────────┘           └─────────────┘
                          (金色粒子爆炸)

步骤2: 代码编辑器淡出 (300ms)
[整个编辑器区域 opacity: 1 → 0.3]

步骤3: 成绩卡片飞入 (500ms)
         ↙
       ┌──────────┐
       │ 🎉任务完成│
       │ ⭐⭐⭐   │
       └──────────┘

步骤4: 经验值条增长 (800ms)
▓▓▓░░░░░░░ → ▓▓▓▓▓▓░░░░
(平滑动画 + 数字跳动)

步骤5: 解锁提示 (500ms)
💡 "解锁新成就：第一道光"
(从下方滑入 + 发光效果)
```

**代码实现**:
```cpp
void playLevelCompleteAnimation() {
    // 1. 硬件光效
    emit showSuccessGlow();
    QTimer::singleShot(500, [this]() {

        // 2. 编辑器淡出
        QPropertyAnimation* fadeOut = new QPropertyAnimation(m_editor, "opacity");
        fadeOut->setDuration(300);
        fadeOut->setStartValue(1.0);
        fadeOut->setEndValue(0.3);
        fadeOut->start();

        // 3. 成绩卡片飞入
        ResultCard* card = new ResultCard(stars, xp, crystals);
        QPropertyAnimation* flyIn = new QPropertyAnimation(card, "geometry");
        flyIn->setDuration(500);
        flyIn->setStartValue(QRect(width()/2, -200, 400, 300));
        flyIn->setEndValue(QRect(width()/2 - 200, height()/2 - 150, 400, 300));
        flyIn->setEasingCurve(QEasingCurve::OutBack);
        flyIn->start();
    });

    QTimer::singleShot(800, [this, xp]() {
        // 4. 经验值增长
        animateXPGain(xp);
    });
}
```

### 2. LED闪烁动画

```css
@keyframes ledBlink {
    0%, 100% {
        background: #FF0000;
        box-shadow: 0 0 10px #FF0000,
                    0 0 20px #FF0000,
                    0 0 30px #FF0000;
    }
    50% {
        background: #330000;
        box-shadow: none;
    }
}

.led.active {
    animation: ledBlink 1s ease-in-out infinite;
}
```

### 3. 代码编译动画

**编译中**:
```
┌─────────────────────────────┐
│  ⚙️ 正在编译...              │
│  ████████░░ 80%             │  ← 进度条动画
│                             │
│  [芯片旋转图标]              │  ← 旋转360度
└─────────────────────────────┘
```

```css
@keyframes spin {
    from { transform: rotate(0deg); }
    to { transform: rotate(360deg); }
}

.compiling-icon {
    animation: spin 1s linear infinite;
}

@keyframes progress {
    0% { width: 0%; }
    100% { width: 100%; }
}

.progress-bar {
    animation: progress 2s ease-out forwards;
}
```

### 4. 成就解锁动画

```
步骤1: 徽章从屏幕底部弹出
      ↑
    🏆
   [徽章]

步骤2: 旋转 + 放大
    🏆  →  🏆  →  🏆
   (小)   (中)   (大)

步骤3: 发光特效
     ✨
    ✨🏆✨
     ✨

步骤4: 标题显示
  ┌──────────────┐
  │  成就解锁!    │
  │  第一道光     │
  └──────────────┘

步骤5: 停留2秒后缩小到右上角
```

---

## 响应式设计

### 断点系统

```css
/* 超小屏幕（手机竖屏）*/
@media (max-width: 576px) {
    .main-layout { grid-template-columns: 1fr; }
    .sidebar { display: none; } /* 隐藏侧边栏，改为抽屉 */
}

/* 小屏幕（手机横屏 / 小平板）*/
@media (min-width: 576px) and (max-width: 768px) {
    .code-area { width: 100%; height: 60%; }
    .hardware-area { width: 100%; height: 40%; }
}

/* 中等屏幕（平板）*/
@media (min-width: 768px) and (max-width: 1200px) {
    .code-area { width: 60%; }
    .hardware-area { width: 40%; }
}

/* 大屏幕（笔记本）*/
@media (min-width: 1200px) and (max-width: 1600px) {
    .code-area { width: 55%; }
    .hardware-area { width: 45%; }
}

/* 超大屏幕（台式机）*/
@media (min-width: 1600px) {
    .code-area { width: 50%; }
    .hardware-area { width: 50%; }
}
```

---

## 无障碍设计

### 1. 键盘导航

**快捷键系统**:
| 快捷键 | 功能 |
|--------|-----|
| Ctrl+S | 保存代码 |
| F5 | 运行代码 |
| F9 | 设置断点 |
| F10 | 单步执行 |
| Ctrl+/ | 注释/取消注释 |
| Ctrl+Shift+F | 格式化代码 |
| Esc | 关闭对话框 |
| Tab | 下一个焦点 |
| Shift+Tab | 上一个焦点 |

### 2. 色盲友好

**除了颜色外，使用图标和文字**:
- ✅ 成功：绿色 + ✓图标 + "成功"文字
- ❌ 错误：红色 + ✗图标 + "错误"文字
- ⚠️ 警告：黄色 + ⚠图标 + "警告"文字

### 3. 字体缩放

支持用户自定义字体大小（Ctrl + 滚轮）:
```javascript
document.addEventListener('wheel', (e) => {
    if(e.ctrlKey) {
        e.preventDefault();
        let fontSize = parseInt(window.getComputedStyle(editor).fontSize);
        fontSize += e.deltaY > 0 ? -1 : 1;
        fontSize = Math.max(10, Math.min(24, fontSize));
        editor.style.fontSize = fontSize + 'px';
    }
});
```

---

## 主题系统

### 预设主题

**1. 赛博朋克（默认）**
- 深蓝背景 + 电光蓝强调色

**2. 暗黑模式**
- 纯黑背景 + 白色/蓝色

**3. 护眼模式**
- 深绿背景 + 浅绿强调色
- 降低对比度

**4. 高对比度**
- 纯黑背景 + 纯白文字
- 无障碍优化

### 主题切换实现

```cpp
class ThemeManager {
public:
    enum Theme {
        CyberPunk,
        Dark,
        EyeCare,
        HighContrast
    };

    void setTheme(Theme theme) {
        QString qssFile;
        switch(theme) {
            case CyberPunk:
                qssFile = ":/themes/cyberpunk.qss";
                break;
            case Dark:
                qssFile = ":/themes/dark.qss";
                break;
            // ...
        }

        QFile file(qssFile);
        file.open(QFile::ReadOnly);
        QString styleSheet = file.readAll();
        qApp->setStyleSheet(styleSheet);
    }
};
```

---

## 总结

本UI/UX设计规范确保了ChipQuest的：

✅ **统一的视觉语言** - 赛博朋克科技风格
✅ **流畅的用户体验** - 60fps动画，即时反馈
✅ **清晰的信息层级** - 重要信息突出显示
✅ **无障碍友好** - 键盘导航、色盲支持
✅ **响应式适配** - 支持多种屏幕尺寸

遵循这些规范，将打造出专业、沉浸、易用的学习应用！
