## XHud UGUI框架总成
[![license](https://img.shields.io/badge/license-AGPLv3.0-red.svg)](https://gitee.com/SevenStrike/XHud/blob/main/LICENSE)
[![GitHub release (latest SemVer)](https://img.shields.io/github/v/release/SevenStrike/XHud)](https://github.com/SevenStrike/XHud/releases/latest)
[![supported](https://img.shields.io/badge/Supported-Unity-success.svg)](https://unity.com/)

### 概述
------------
这是一套基于 Unity UGUI 的深度可定制、模块化框架总成
<br>
<br>
| 开源不易，您的支持是持续更新的动力，<br>这个小工具倾注了我无数个深夜的调试与优化，它永远免费，但绝非无成本，如果您觉得这个工具<br>能为您节省时间、解决问题，甚至带来一丝愉悦，请考虑赞助一杯咖啡，让我知道：有人在乎这份付出，而这将成为我熬夜修复Bug、<br>添加新功能的最大动力。开源不是用爱发电，您的认可会让它走得更远|![](Docs/donate.jpg) |
|:-|-:|
| **欢迎加入技术研讨群，在这里可以和我以及大家一起探讨插件的优化以及相关的技术实现思路，同时在做项目时遇到的众多问题以及瓶颈<br>阻碍都可以互相探讨学习**|![](Docs/qqgroups.jpg) |

<br>

# 🎮 XHud

## Unity UGUI 高级管理架构插件

[![Unity Version](https://img.shields.io/badge/Unity-2021.3%2B-222C37?style=flat-square&logo=unity)](https://unity.com/)
[![URP](https://img.shields.io/badge/URP-Compatible-00B4D8?style=flat-square)](https://unity.com/srp/universal-render-pipeline)
[![License](https://img.shields.io/badge/License-AGPL--3.0-EF233C?style=flat-square)](LICENSE)
[![Version](https://img.shields.io/badge/Version-1.0.0-F4A261?style=flat-square)](https://github.com/SevenStrikeMedia/XHud)

<br>

> **一套完整的Unity UI开发解决方案**  
> 集对象池、动画系统、多分辨率适配、资源管理于一体

<br>

[📖 文档](#-目录) • [🚀 快速开始](#-快速开始) • [📦 安装](#-安装) • [🛠️ 工具](#️-编辑器工具) • [📄 许可证](#-许可证)

<br>

</div>

---

## 📖 目录

<table>
<tr>
<td width="33%">

**入门指南**
- [项目简介](#-项目简介)
- [核心特性](#-核心特性)
- [功能对比](#-功能对比)
- [架构设计](#-架构设计)

</td>
<td width="33%">

**使用文档**
- [快速开始](#-快速开始)
- [安装指南](#-安装指南)
- [目录结构](#-目录结构)
- [核心模块](#-核心模块)

</td>
<td width="33%">

**高级内容**
- [API参考](#-api参考)
- [编辑器工具](#️-编辑器工具)
- [配置说明](#-配置说明)
- [许可证](#-许可证)

</td>
</tr>
</table>

---

## 📖 项目简介

### 🎯 什么是XHud？

**XHud** 是一个专为Unity开发者打造的专业级UI管理框架。它不仅是一个UI组件库，更是一套**完整的UI系统解决方案**，内置了企业级项目所需的各类核心模块。

### ✨ 核心价值

| 价值点 | 说明 |
|:-----:|------|
| **⚡ 高效开发** | 链式API + 可视化编辑器，开发效率提升3-5倍 |
| **🎯 性能优化** | 内置对象池系统，零GC压力，流畅运行 |
| **📱 多端适配** | RMS响应式布局，一套UI适配所有分辨率 |
| **🔧 易于维护** | 模块化设计，资源库管理，代码清晰 |
| **🎨 视觉丰富** | 自研动画引擎 + URP散焦模糊 + 转场特效 |

### 📊 适用场景

| 项目类型 | 推荐度 | 说明 |
|:-------:|:-----:|------|
| 大型MMO/RPG | ★★★★★ | 复杂UI系统、大量弹窗、性能要求高 |
| 卡牌策略游戏 | ★★★★★ | 丰富UI动画、特效表现、资源管理 |
| 超休闲游戏 | ★★★★☆ | 快速搭建原型、易于维护迭代 |
| 工具类应用 | ★★★★☆ | 多分辨率适配、界面复杂度高 |
| 原型开发 | ★★★★★ | 可视化工具、快速验证设计 |

---

## ✨ 核心特性

### 🏆 特性全景图

| 分类 | 功能 | 说明 |
|:---:|------|------|
| 🎮 | **增强控件** | Button、Toggle、Slider、Progress、Option、Text/TMP |
| 🎬 | **动画引擎** | 8种动画类型 + 4种动画模式 + 音效同步 |
| 📦 | **资源管理** | 色卡库、曲线库、音效库、字体库、动效库、转场库 |
| 🛠️ | **编辑器工具** | 布局生成器、动画预览、截图工具、性能监控 |
| 🎨 | **视觉特效** | URP散焦模糊、序列帧转场、蓝图视觉模式 |
| 📱 | **多分辨率** | RMS响应式布局系统，精细适配 |

### 📋 详细功能

<details>
<summary><b>🎮 UI控件增强（点击展开）</b></summary>

| 控件 | 增强功能 |
|-----|---------|
| **Button** | 长按事件、长按进度回调、文字/图标/背景变色过渡 |
| **Toggle** | 滑动开关动画、缓动模式、控制柄范围可调 |
| **Slider** | 数值精度控制、自定义单位后缀、标题/副标题显示 |
| **Progress** | 平滑/闪现双模式、标题/副标题、图标显示 |
| **Option** | 光标移动动画、缓动/差值运动、多选项管理 |
| **Text/TMP** | 字体样式库、渐变色、全局尺寸缩放 |

</details>

<details>
<summary><b>🎬 动画系统（点击展开）</b></summary>

**动画类型：** 位移 · 旋转 · 缩放 · 颜色 · 淡化 · 打字机 · 图像填充 · 尺寸

**动画模式：** 起始→默认 · 默认→结束 · 起始→结束 · 当前→结束

**高级特性：** 缓动曲线 · 循环播放 · 音效同步触发

</details>

<details>
<summary><b>📦 资源管理（点击展开）</b></summary>

| 资源库 | 功能 |
|-------|------|
| 色卡库 | 主题色管理、动态换肤 |
| 曲线库 | 动画曲线预设 |
| 音效库 | UI音效集中管理 |
| 字体样式库 | 文字样式统一管理 |
| 动效库 | 入场/出场动效模板 |
| 转场库 | 屏幕切换特效 |
| 元素库 | UI预制体对象池 |

</details>

---

## 📊 功能对比

### vs 传统UGUI

| 对比项 | 传统UGUI | XHud | 提升 |
|-------|:-------:|:----:|:---:|
| UI对象池 | ❌ | ✅ | +100% |
| 动画系统 | ❌ | ✅ | +100% |
| 多分辨率适配 | ⚠️ | ✅ | +80% |
| 主题换肤 | ❌ | ✅ | +90% |
| 文字样式管理 | ❌ | ✅ | +85% |
| 编辑器预览 | ❌ | ✅ | +100% |
| 开发效率 | 基准 | 🚀 | **3-5倍** |

### vs 主流UI方案

| 特性 | XHud | UnityUI | DoTween | FairyGUI |
|-----|:----:|:-------:|:-------:|:--------:|
| 对象池 | ✅ | ❌ | ❌ | ✅ |
| 动画系统 | ✅ | ❌ | ✅ | ✅ |
| 多分辨率 | ✅ | ⚠️ | ❌ | ✅ |
| 资源库 | ✅ | ❌ | ❌ | ✅ |
| 编辑器工具 | ✅ | ❌ | ✅ | ✅ |
| URP支持 | ✅ | ✅ | ✅ | ⚠️ |
| 开源协议 | AGPL | 内置 | MIT | 商业 |

---

## 🏗️ 架构设计

### 整体架构
┌─────────────────────────────────────────────────────────────┐
│ 应用层 (Application) │
│ 游戏逻辑 / UI业务代码 │
├─────────────────────────────────────────────────────────────┤
│ API层 (XHud_Manager) │
│ 元素生成 · 动画控制 · 相机管理 · 音效播放 │
├─────────────────────────────────────────────────────────────┤
│ 模块层 (Modules) │
│ Element · Button · Slider · Toggle · Progress · Option │
├─────────────────────────────────────────────────────────────┤
│ 图元层 (Primitives) │
│ Painting · Feature · Tween · Controller │
├─────────────────────────────────────────────────────────────┤
│ 资源层 (Libraries) │
│ Colors · Curves · Sounds · TextStyle · Motion · Transition│
├─────────────────────────────────────────────────────────────┤
│ 运行时层 (Runtime) │
│ Unity UGUI · URP · XTween · ObjectPool │
└─────────────────────────────────────────────────────────────┘

### 核心依赖说明

> 💡 **XTween** 是一个独立的动画引擎插件，与XHud由同一作者（徐寅智）开发。它提供了强大的补间动画功能，是XHud动画系统的底层支撑。XTween可独立使用，也作为XHud的核心依赖。

| 依赖 | 作者 | 说明 |
|-----|------|------|
| **XTween** | 徐寅智 | 独立动画引擎，XHud动画系统底层 |
| Unity UGUI | Unity | 基础UI框架 |
| URP | Unity | 通用渲染管线（可选） |
| TextMeshPro | Unity | 高级文字渲染 |

---

## 🚀 快速开始

### 最小示例

只需几行代码，即可创建并显示一个UI元素：

```csharp
using SevenStrikeModules.XHud;

public class Demo : MonoBehaviour
{
    void Start()
    {
        // 生成一个按钮并显示
        XHud_Manager.Instance
            .hm_ScreenElement_Create("MainUI", "Btn_Confirm")
            .SetAnchored_Screen(XHudAnchor.中心, "ConfirmBtn")
            .SetSize(200, 60)
            .Element_In();
    }
}
```

## 💡链式API示例

```csharp
// 创建带弹窗动画的对话框
XHud_Manager.Instance
    .hm_ScreenElement_Create("Dialogs", "Popup_Message")
    .SetAnchored_Screen(XHudAnchor.中心, "MessagePopup")
    .SetSize(400, 300)
    .SetScale(0.8f, 0.8f, 1)
    .SetAlpha(0)
    .SetRMS(true, "iPhoneX")
    .On_In_Start((ele) => Debug.Log("弹窗开始显示"))
    .On_In_End((ele) => Debug.Log("弹窗显示完成"))
    .Element_In();
```

## 📁 目录结构

```csharp
XHud/
├── Editor/                          # 编辑器扩展
│   ├── Editor_XHud_Manager.cs       # 管理器编辑器
│   ├── Editor_XHud_Module_*.cs      # 控件编辑器
│   └── Editor_XHud_LibrarySetTool_*.cs # 资源库采集器
│
├── Scripts/                         # 运行时脚本
│   ├── XHud_Manager.cs              # 核心管理器
│   ├── XHud_Module_Element.cs       # UI元素基类
│   ├── XHud_Module_Button.cs        # 按钮控件
│   ├── XHud_Module_Slider.cs        # 滑动条控件
│   ├── XHud_Module_Toggle.cs        # 开关控件
│   ├── XHud_Module_Progress.cs      # 进度条控件
│   ├── XHud_Module_Option.cs        # 选项控件
│   ├── XHud_Module_Text.cs          # 文字控件
│   ├── XHud_Module_TmpText.cs       # TMP文字控件
│   ├── XHud_Module_Primitive_*.cs   # 图元控制器
│   ├── XHud_Library_*.cs            # 资源库
│   └── ...
│
├── Materials/                       # 材质资源
├── Prefabs/                         # 预制体
├── Fonts/                           # 字体资源
├── Textures/                        # 贴图资源
├── Sprites/                         # 精灵资源
├── Sound/                           # 音效资源
├── Shaders/                         # 着色器
├── Config/                          # 配置文件
└── GUI/                             # GUI样式资源
```
