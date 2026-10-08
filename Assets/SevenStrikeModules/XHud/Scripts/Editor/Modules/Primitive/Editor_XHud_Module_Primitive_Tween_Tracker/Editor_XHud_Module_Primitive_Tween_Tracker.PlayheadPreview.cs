/*
 * ============================================================================
 * ⚠ 版权声明（禁止删除、禁止修改、衍生作品必须保留此注释）⚠
 * ============================================================================
 * 版权声明 Copyright (C) 2025-Present Nanjing SevenStrike Media Co., Ltd.
 * 中文名称：南京塞维斯传媒有限公司
 * 英文名称：SevenStrikeMedia
 * 项目作者：徐寅智
 * 项目名称：XHud - Unity UGUI 高级管理架构插件
 * 项目启动：2025年8月
 * 官方网站：http://sevenstrike.com/
 * 授权协议：GNU Affero General Public License Version 3 (AGPL 3.0)
 * 协议说明：
 * 1. 你可以自由使用、修改、分发本插件的源代码，但必须保留此版权注释
 * 2. 基于本插件修改后的衍生作品，必须同样遵循 AGPL 3.0 授权协议
 * 3. 若将本插件用于网络服务（如云端Unity编辑器、在线动效生成工具），必须公开修改后的完整源代码
 * 4. 完整协议文本可查阅：https://www.gnu.org/licenses/agpl-3.0.html
 * ============================================================================
 * 违反本注释保留要求，将违反 AGPL 3.0 授权协议，需承担相应法律责任
 */
namespace SevenStrikeModules.XHud.Editor
{
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XTween;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public partial class Editor_XHud_Module_Primitive_Tween_Tracker
    {
        #region 字段：飞梭预览驱动

        /// <summary>
        /// 飞梭预览用的 XTween 缓存。
        /// <para/>key = 节点在 <see cref="Nodes"/> 列表中的索引。
        /// <para/>拖动期间索引稳定（拖拽期间禁用增删快捷键），松手时整体清空。
        /// </summary>
        private readonly Dictionary<int, XTween_Interface> playheadPreviewTweens
            = new Dictionary<int, XTween_Interface>();

        /// <summary>
        /// 上一帧处于 [Delay, Delay+Duration] 区间内的节点索引集合。
        /// <para/>用于判定"刚进入区间"（方案 a）：
        /// 本帧在区间、上一帧不在 → 刚进入，需要重建 tween / 重置 C-E 动态起点。
        /// <para/>松手时整体清空，使下次拖动必然重建。
        /// </summary>
        private readonly HashSet<int> playheadPrevInRange = new HashSet<int>();

        /// <summary>
        /// 本窗口是否曾驱动过飞梭预览。
        /// <para/>用于 <see cref="CleanupPlayheadPreviewOnDisable"/> 决定是否调用
        /// <c>PrimitiveFeature_Load()</c> 还原物体——只在驱动过时才还原，避免无谓副作用。
        /// </summary>
        private bool playheadPreviewEverDriven = false;

        #endregion

        #region 飞梭预览驱动 - 总入口

        /// <summary>
        /// 飞梭驱动预览总入口。
        /// <para/>仅在拖动飞梭期间由 <c>UpdatePlayheadFromMouse</c> 调用。
        /// <para/>遍历所有 <c>Enabled</c> 节点，对每个节点按 <paramref name="t"/> 求值：
        /// <list type="bullet">
        /// <item><description>t &lt; Delay：不驱动，保持当前状态；</description></item>
        /// <item><description>Delay ≤ t ≤ Delay+Duration：按 p=(t-Delay)/Duration 求值并写组件；</description></item>
        /// <item><description>t &gt; Delay+Duration：不驱动，保持当前状态。</description></item>
        /// </list>
        /// <para/>循环（LoopCount/LoopType）不参与，p 直接由区间位置决定。
        /// <para/>本方法不记 Undo、不置 TweenIsPreviewing、不触发除 OnUpdate 外的任何回调。
        /// </summary>
        /// <param name="t">当前飞梭时间（内容绝对秒）</param>
        private void DrivePlayheadPreview(float t)
        {
            if (target == null || target.Equals(null)) return;
            if (Nodes == null || Nodes.Count == 0) return;

            playheadPreviewEverDriven = true;

            for (int i = 0; i < Nodes.Count; i++)
            {
                TweenNode node = Nodes[i];

                // ── 未启用节点：不参与驱动，清理其残留 tween 与区间标记 ──
                if (!node.Enabled)
                {
                    CleanupPreviewTween(i);
                    playheadPrevInRange.Remove(i);
                    continue;
                }

                float D = node.Delay;
                float U = node.Duration;
                bool inRange = (t >= D && t <= D + U);
                bool wasInRange = playheadPrevInRange.Contains(i);

                if (!inRange)
                {
                    // ── 不在区间：不驱动、不还原 ──
                    // 只清区间标记，让下次"进入"时重新走刚进入逻辑（重建 tween / 重置 C-E 起点）。
                    // 注意：此处**不** Kill 预览 tween、**不**还原物体——
                    // 需求规定 t > Delay+Duration 时物体保持当前状态。
                    playheadPrevInRange.Remove(i);
                    continue;
                }

                // ── 在区间 ──
                if (!wasInRange)
                {
                    // ★ 刚进入区间（方案 a）：
                    // 1. 清理可能残留的旧 tween（例如上一次进入后未被替换的实例）
                    CleanupPreviewTween(i);

                    // 2. 创建新的预览 tween
                    XTween_Interface tw = CreatePreviewTweenForNode(node, i);
                    if (tw == null)
                    {
                        // 创建失败（组件缺失 / 类型未实现）：跳过该节点，不标记区间
                        playheadPrevInRange.Remove(i);
                        continue;
                    }
                    playheadPreviewTweens[i] = tw;

                    // 3. 标记已进入区间
                    playheadPrevInRange.Add(i);

                    // C-E 模式的动态起点：新 tween 内部 _StartValueResolved 初始为 false，
                    // 首次 EvaluateAt 时会捕获 rt.anchoredPosition3D 当前值作为起点，天然满足 (iii)。
                    // 无需显式调用 ResetDynamicStart。
                }

                // ── 区间内求值 ──
                // elapsed = t - D；clamp 到 [0, U] 由 EvaluateAt 内部完成。
                float elapsed = t - D;
                playheadPreviewTweens[i].EvaluateAt(elapsed);
            }
        }

        #endregion

        #region 飞梭预览驱动 - tween 创建

        /// <summary>
        /// 为一个动画节点创建飞梭预览用的 XTween 实例。
        /// <para/>与运行时 <c>Tween_Create_*</c> 的关键区别：
        /// <list type="bullet">
        /// <item><description>duration = node.Duration（不乘任何倍率），保证 (t-D)/U 与 tween 内部 Duration 一致；</description></item>
        /// <item><description>不调用 Tween_Rewind / Tween_Kill，无前置副作用；</description></item>
        /// <item><description>不注册除 OnUpdate 之外的任何回调；</description></item>
        /// <item><description>不做 Undo。</description></item>
        /// </list>
        /// <para/>第二步仅实现 <see cref="TweenNodeType.a_位移"/>，其他类型返回 null（该节点本帧不驱动）。
        /// </summary>
        /// <param name="node">目标节点</param>
        /// <param name="index">节点索引（当前未使用，保留供调试/扩展）</param>
        /// <returns>创建好的预览 tween；失败返回 null</returns>
        private XTween_Interface CreatePreviewTweenForNode(TweenNode node, int index)
        {
            if (node == null) return null;
            if (target == null || target.controller == null) return null;

            switch (node.Type)
            {
                case TweenNodeType.a_位移:
                    return CreatePreviewTween_Position(node);
                case TweenNodeType.r_旋转: return CreatePreviewTween_Rotate(node);
                case TweenNodeType.s_缩放: return CreatePreviewTween_Scale(node);
                case TweenNodeType.z_尺寸: return CreatePreviewTween_Size(node);
                case TweenNodeType.c_颜色: return CreatePreviewTween_Color(node);
                case TweenNodeType.g_淡化: return CreatePreviewTween_Fader(node);
                case TweenNodeType.f_图像填充: return CreatePreviewTween_Fill(node);
                case TweenNodeType.w_打字机: return CreatePreviewTween_Writer(node);
                default:
                    return null;
            }
        }

        /// <summary>
        /// 创建位移动画（anchoredPosition3D）的飞梭预览 tween。
        /// <para/>按 <see cref="TweenValueMode"/> 决定起点：
        /// <list type="bullet">
        /// <item><description>S-D：起点 = From_Vector3，终点 = Original_Vector3；</description></item>
        /// <item><description>S-E：起点 = From_Vector3，终点 = End_Vector3；</description></item>
        /// <item><description>D-E：起点 = Original_Vector3，终点 = End_Vector3；</description></item>
        /// <item><description>C-E：起点 = 进入瞬间的当前 anchoredPosition3D（动态），终点 = End_Vector3。</description></item>
        /// </list>
        /// </summary>
        private XTween_Interface CreatePreviewTween_Position(TweenNode node)
        {
            RectTransform rt = target.controller.mod_Rect;
            if (rt == null) return null;

            // ── 按模式解析目标值与起点策略 ──
            Vector3 targetValue;
            Vector3 fromStatic = default;
            bool useStaticFrom = false;
            bool useDynamicFrom = false;

            switch (node.TweenValueMode)
            {
                case TweenValueMode.起始到默认_S_D:
                    targetValue = node.Original_Vector3;
                    fromStatic = node.From_Vector3;
                    useStaticFrom = true;
                    break;

                case TweenValueMode.起始到结束_S_E:
                    targetValue = node.End_Vector3;
                    fromStatic = node.From_Vector3;
                    useStaticFrom = true;
                    break;

                case TweenValueMode.默认到结束_D_E:
                    targetValue = node.End_Vector3;
                    fromStatic = node.Original_Vector3;
                    useStaticFrom = true;
                    break;

                case TweenValueMode.当前到结束_C_E:
                    targetValue = node.End_Vector3;
                    useDynamicFrom = true;
                    break;

                default:
                    return null;
            }

            // ── 创建底层 tween ──
            // isRelative = false
            // autokill = false
            // rewind_set_startvalue = false（飞梭驱动不依赖 rewind）
            // complete_set_endvalue = false（飞梭驱动不依赖 complete）
            XTween_Interface tw = rt.xt_AnchoredPosition3D_To(
                targetValue,
                node.Duration,
                false,
                false,
                false,
                false);

            if (tw == null) return null;

            // ── 起点 ──
            if (useStaticFrom)
            {
                tw = tw.SetFrom(fromStatic);
            }
            else if (useDynamicFrom)
            {
                // C-E：动态起点 = 进入瞬间的 rt.anchoredPosition3D
                // 闭包内做 null 检查（rt 可能在中途被销毁），兜底返回 Vector3.zero
                tw = tw.SetFromDynamic(() =>
                {
                    if (rt == null) return Vector3.zero;
                    return rt.anchoredPosition3D;
                });
            }

            // ── 缓动 ──
            if (node.Ease == EaseMode.None)
                tw = tw.SetEase(node.Curve);
            else
                tw = tw.SetEase(node.Ease);

            return tw;
        }
        /// <summary>
        /// 创建旋转动画（eulerAngles）的飞梭预览 tween。
        /// <para/>字段映射与运行时 <c>Tween_Create_Rotate</c> 保持一致：
        /// 起点/终点使用 <c>From_Vector3</c> / <c>Original_Vector3</c> / <c>End_Vector3</c>，
        /// 旋转空间固定 <see cref="XTweenRotationSpace.世界坐标"/>，旋转模式用 <c>node.RotateMode</c>。
        /// <para/>按 <see cref="TweenValueMode"/> 决定起点：
        /// <list type="bullet">
        /// <item><description>S-D：起点 = From_Vector3，终点 = Original_Vector3；</description></item>
        /// <item><description>S-E：起点 = From_Vector3，终点 = End_Vector3；</description></item>
        /// <item><description>D-E：起点 = Original_Vector3，终点 = End_Vector3；</description></item>
        /// <item><description>C-E：起点 = 进入瞬间的当前 eulerAngles（动态），终点 = End_Vector3。</description></item>
        /// </list>
        /// </summary>
        private XTween_Interface CreatePreviewTween_Rotate(TweenNode node)
        {
            RectTransform rt = target.controller.mod_Rect;
            if (rt == null) return null;

            Vector3 targetValue;
            Vector3 fromStatic = default;
            bool useStaticFrom = false;
            bool useDynamicFrom = false;

            switch (node.TweenValueMode)
            {
                case TweenValueMode.起始到默认_S_D:
                    targetValue = node.Original_Vector3;
                    fromStatic = node.From_Vector3;
                    useStaticFrom = true;
                    break;

                case TweenValueMode.起始到结束_S_E:
                    targetValue = node.End_Vector3;
                    fromStatic = node.From_Vector3;
                    useStaticFrom = true;
                    break;

                case TweenValueMode.默认到结束_D_E:
                    targetValue = node.End_Vector3;
                    fromStatic = node.Original_Vector3;
                    useStaticFrom = true;
                    break;

                case TweenValueMode.当前到结束_C_E:
                    targetValue = node.End_Vector3;
                    useDynamicFrom = true;
                    break;

                default:
                    return null;
            }

            // 运行时固定使用世界坐标 → eulerAngles
            const XTweenRotationSpace space = XTweenRotationSpace.世界坐标;

            // 注意：xt_Rotate_To 的这个重载没有 rewind_set_startvalue / complete_set_endvalue 参数
            XTween_Interface tw = rt.xt_Rotate_To(
                targetValue,
                node.Duration,
                false,              // isRelative
                false,              // autokill
                space,              // 世界坐标
                node.RotateMode);   // 照用 node.RotateMode

            if (tw == null) return null;

            if (useStaticFrom)
            {
                tw = tw.SetFrom(fromStatic);
            }
            else if (useDynamicFrom)
            {
                tw = tw.SetFromDynamic(() =>
                {
                    if (rt == null) return Vector3.zero;
                    return space == XTweenRotationSpace.本地坐标
                        ? rt.localEulerAngles
                        : rt.eulerAngles;
                });
            }

            if (node.Ease == EaseMode.None)
                tw = tw.SetEase(node.Curve);
            else
                tw = tw.SetEase(node.Ease);

            return tw;
        }
        /// <summary>
        /// 创建缩放动画（localScale）的飞梭预览 tween。
        /// <para/>字段映射与运行时 <c>Tween_Create_Scale</c> 保持一致：
        /// 起点/终点使用 <c>From_Vector3</c> / <c>Original_Vector3</c> / <c>End_Vector3</c>，
        /// C-E 动态起点读 <c>localScale</c>。
        /// </summary>
        private XTween_Interface CreatePreviewTween_Scale(TweenNode node)
        {
            RectTransform rt = target.controller.mod_Rect;
            if (rt == null) return null;

            Vector3 targetValue;
            Vector3 fromStatic = default;
            bool useStaticFrom = false;
            bool useDynamicFrom = false;

            switch (node.TweenValueMode)
            {
                case TweenValueMode.起始到默认_S_D:
                    targetValue = node.Original_Vector3;
                    fromStatic = node.From_Vector3;
                    useStaticFrom = true;
                    break;

                case TweenValueMode.起始到结束_S_E:
                    targetValue = node.End_Vector3;
                    fromStatic = node.From_Vector3;
                    useStaticFrom = true;
                    break;

                case TweenValueMode.默认到结束_D_E:
                    targetValue = node.End_Vector3;
                    fromStatic = node.Original_Vector3;
                    useStaticFrom = true;
                    break;

                case TweenValueMode.当前到结束_C_E:
                    targetValue = node.End_Vector3;
                    useDynamicFrom = true;
                    break;

                default:
                    return null;
            }

            // 与位置一致：rewind_set_startvalue = false、complete_set_endvalue = false
            XTween_Interface tw = rt.xt_Scale_To(
                targetValue,
                node.Duration,
                false,   // isRelative
                false,   // autokill
                false,   // rewind_set_startvalue
                false);  // complete_set_endvalue

            if (tw == null) return null;

            if (useStaticFrom)
            {
                tw = tw.SetFrom(fromStatic);
            }
            else if (useDynamicFrom)
            {
                tw = tw.SetFromDynamic(() =>
                {
                    if (rt == null) return Vector3.zero;
                    return rt.localScale;
                });
            }

            if (node.Ease == EaseMode.None)
                tw = tw.SetEase(node.Curve);
            else
                tw = tw.SetEase(node.Ease);

            return tw;
        }
        /// <summary>
        /// 创建尺寸动画（sizeDelta）的飞梭预览 tween。
        /// <para/>字段映射与运行时 <c>Tween_Create_Size</c> 保持一致：
        /// 起点/终点使用 <c>From_Vector2</c> / <c>Original_Vector2</c> / <c>End_Vector2</c>，
        /// C-E 动态起点读 <c>sizeDelta</c>。
        /// </summary>
        private XTween_Interface CreatePreviewTween_Size(TweenNode node)
        {
            RectTransform rt = target.controller.mod_Rect;
            if (rt == null) return null;

            Vector2 targetValue;
            Vector2 fromStatic = default;
            bool useStaticFrom = false;
            bool useDynamicFrom = false;

            switch (node.TweenValueMode)
            {
                case TweenValueMode.起始到默认_S_D:
                    targetValue = node.Original_Vector2;
                    fromStatic = node.From_Vector2;
                    useStaticFrom = true;
                    break;

                case TweenValueMode.起始到结束_S_E:
                    targetValue = node.End_Vector2;
                    fromStatic = node.From_Vector2;
                    useStaticFrom = true;
                    break;

                case TweenValueMode.默认到结束_D_E:
                    targetValue = node.End_Vector2;
                    fromStatic = node.Original_Vector2;
                    useStaticFrom = true;
                    break;

                case TweenValueMode.当前到结束_C_E:
                    targetValue = node.End_Vector2;
                    useDynamicFrom = true;
                    break;

                default:
                    return null;
            }

            XTween_Interface tw = rt.xt_Size_To(
                targetValue,
                node.Duration,
                false,   // isRelative
                false,   // autokill
                false,   // rewind_set_startvalue
                false);  // complete_set_endvalue

            if (tw == null) return null;

            if (useStaticFrom)
            {
                tw = tw.SetFrom(fromStatic);
            }
            else if (useDynamicFrom)
            {
                tw = tw.SetFromDynamic(() =>
                {
                    if (rt == null) return Vector2.zero;
                    return rt.sizeDelta;
                });
            }

            if (node.Ease == EaseMode.None)
                tw = tw.SetEase(node.Curve);
            else
                tw = tw.SetEase(node.Ease);

            return tw;
        }
        /// <summary>
        /// 创建颜色动画（Graphic.color）的飞梭预览 tween。
        /// <para/>字段映射与运行时 <c>Tween_Create_Color</c> 保持一致：
        /// 起点/终点使用 <c>From_Color</c> / <c>Original_Color</c> / <c>End_Color</c>，
        /// C-E 动态起点读 <c>gc.color</c>。
        /// <para/>
        /// <b>前置校验照抄运行时：</b>
        /// <list type="number">
        /// <item><description>文字库同步样式检查：若 Text / TmpText 处于"库同步样式"且启用颜色效果，返回 null（不驱动）；</description></item>
        /// <item><description>配色器接管检查：若 Image / RawImage 处于"同步库颜色"接管状态，返回 null（不驱动）；</description></item>
        /// </list>
        /// 这与运行时 <c>Tween_Create_Color</c> 的约束完全一致——飞梭预览不产生"运行时不存在的效果"。
        /// </summary>
        private XTween_Interface CreatePreviewTween_Color(TweenNode node)
        {
            XHud_Module_Primitive_Controller ctrl = target.controller;

            // ── 前置校验 1：文字库同步样式 ──
            bool isTextColorMode = false;
            if ((ctrl.mod_Text && ctrl.mod_Text.StyleLibSynching &&
                 ctrl.mod_Text.TextStyleInfo.LibStyle_Effect_color) ||
                (ctrl.mod_TmpText && ctrl.mod_TmpText.StyleLibSynching &&
                 ctrl.mod_TmpText.TextStyleInfo.LibStyle_Effect_color))
            {
                isTextColorMode = true;
            }
            if (isTextColorMode) return null;

            // ── 前置校验 2：配色器接管 ──
            if (ctrl.GetModuleType() == ModuleType.Image && ctrl.pt_Painting.SyncLibraryColor)
                return null;
            if (ctrl.GetModuleType() == ModuleType.RawImage && ctrl.pt_Painting.SyncLibraryColor)
                return null;

            Graphic gc = ctrl.RecognizeType();
            if (gc == null) return null;

            Color targetValue;
            Color fromStatic = default;
            bool useStaticFrom = false;
            bool useDynamicFrom = false;

            switch (node.TweenValueMode)
            {
                case TweenValueMode.起始到默认_S_D:
                    targetValue = node.Original_Color;
                    fromStatic = node.From_Color;
                    useStaticFrom = true;
                    break;

                case TweenValueMode.起始到结束_S_E:
                    targetValue = node.End_Color;
                    fromStatic = node.From_Color;
                    useStaticFrom = true;
                    break;

                case TweenValueMode.默认到结束_D_E:
                    targetValue = node.End_Color;
                    fromStatic = node.Original_Color;
                    useStaticFrom = true;
                    break;

                case TweenValueMode.当前到结束_C_E:
                    targetValue = node.End_Color;
                    useDynamicFrom = true;
                    break;

                default:
                    return null;
            }

            // Graphic 重载：(endValue, duration, autokill, rewind_set_startvalue, complete_set_endvalue)
            XTween_Interface tw = gc.xt_Color_To(
                targetValue,
                node.Duration,
                false,   // autokill
                false,   // rewind_set_startvalue
                false);  // complete_set_endvalue

            if (tw == null) return null;

            if (useStaticFrom)
            {
                tw = tw.SetFrom(fromStatic);
            }
            else if (useDynamicFrom)
            {
                // 闭包捕获 gc；gc 与动画器同物体，正常生命周期内不会失效，仅做防御性判空
                tw = tw.SetFromDynamic(() =>
                {
                    if (gc == null) return Color.clear;
                    return gc.color;
                });
            }

            if (node.Ease == EaseMode.None)
                tw = tw.SetEase(node.Curve);
            else
                tw = tw.SetEase(node.Ease);

            return tw;
        }
        /// <summary>
        /// 创建淡化动画（CanvasGroup.alpha）的飞梭预览 tween。
        /// <para/>字段映射与运行时 <c>Tween_Create_Fader</c> 保持一致：
        /// 起点/终点使用 <c>From_Float</c> / <c>Original_Float</c> / <c>End_Float</c>，
        /// C-E 动态起点读 <c>mod_CanvasGroup.alpha</c>。
        /// <para/>CanvasGroup 重载签名：(endValue, duration, autokill, rewind_set_startvalue, complete_set_endvalue)，无 isRelative。
        /// </summary>
        private XTween_Interface CreatePreviewTween_Fader(TweenNode node)
        {
            CanvasGroup cg = target.controller.mod_CanvasGroup;
            if (cg == null) return null;

            float targetValue;
            float fromStatic = 0f;
            bool useStaticFrom = false;
            bool useDynamicFrom = false;

            switch (node.TweenValueMode)
            {
                case TweenValueMode.起始到默认_S_D:
                    targetValue = node.Original_Float;
                    fromStatic = node.From_Float;
                    useStaticFrom = true;
                    break;

                case TweenValueMode.起始到结束_S_E:
                    targetValue = node.End_Float;
                    fromStatic = node.From_Float;
                    useStaticFrom = true;
                    break;

                case TweenValueMode.默认到结束_D_E:
                    targetValue = node.End_Float;
                    fromStatic = node.Original_Float;
                    useStaticFrom = true;
                    break;

                case TweenValueMode.当前到结束_C_E:
                    targetValue = node.End_Float;
                    useDynamicFrom = true;
                    break;

                default:
                    return null;
            }

            XTween_Interface tw = cg.xt_Alpha_To(
                targetValue,
                node.Duration,
                false,   // autokill
                false,   // rewind_set_startvalue
                false);  // complete_set_endvalue

            if (tw == null) return null;

            if (useStaticFrom)
            {
                tw = tw.SetFrom(fromStatic);
            }
            else if (useDynamicFrom)
            {
                tw = tw.SetFromDynamic(() =>
                {
                    if (cg == null) return 0f;
                    return cg.alpha;
                });
            }

            if (node.Ease == EaseMode.None)
                tw = tw.SetEase(node.Curve);
            else
                tw = tw.SetEase(node.Ease);

            return tw;
        }
        /// <summary>
        /// 创建图像填充动画（Image.fillAmount）的飞梭预览 tween。
        /// <para/>字段映射与运行时 <c>Tween_Create_Fill</c> 保持一致：
        /// 起点/终点使用 <c>From_Float</c> / <c>Original_Float</c> / <c>End_Float</c>，
        /// C-E 动态起点读 <c>mod_Image.fillAmount</c>。
        /// <para/>与运行时一致：targetValue 与静态起点都过 <see cref="Mathf.Clamp01"/>。
        /// </summary>
        private XTween_Interface CreatePreviewTween_Fill(TweenNode node)
        {
            Image img = target.controller.mod_Image;
            if (img == null) return null;

            float targetValue;
            float fromStatic = 0f;
            bool useStaticFrom = false;
            bool useDynamicFrom = false;

            switch (node.TweenValueMode)
            {
                case TweenValueMode.起始到默认_S_D:
                    targetValue = node.Original_Float;
                    fromStatic = node.From_Float;
                    useStaticFrom = true;
                    break;

                case TweenValueMode.起始到结束_S_E:
                    targetValue = node.End_Float;
                    fromStatic = node.From_Float;
                    useStaticFrom = true;
                    break;

                case TweenValueMode.默认到结束_D_E:
                    targetValue = node.End_Float;
                    fromStatic = node.Original_Float;
                    useStaticFrom = true;
                    break;

                case TweenValueMode.当前到结束_C_E:
                    targetValue = node.End_Float;
                    useDynamicFrom = true;
                    break;

                default:
                    return null;
            }

            // 与运行时一致：目标值 Clamp01
            XTween_Interface tw = img.xt_Fill_To(
                Mathf.Clamp01(targetValue),
                node.Duration,
                false,   // autokill
                false,   // rewind_set_startvalue
                false);  // complete_set_endvalue

            if (tw == null) return null;

            if (useStaticFrom)
            {
                // 与运行时一致：静态起点也 Clamp01
                tw = tw.SetFrom(Mathf.Clamp01(fromStatic));
            }
            else if (useDynamicFrom)
            {
                tw = tw.SetFromDynamic(() =>
                {
                    if (img == null) return 0f;
                    return img.fillAmount;
                });
            }

            if (node.Ease == EaseMode.None)
                tw = tw.SetEase(node.Curve);
            else
                tw = tw.SetEase(node.Ease);

            return tw;
        }
        /// <summary>
        /// 创建打字机动画的飞梭预览 tween。
        /// <para/>与运行时 <c>Tween_Create_Writer_Text</c> / <c>Tween_Create_Writer_TmpText</c> 一致：
        /// <see cref="XHud_Module_Primitive_Controller.mod_Text"/> 优先，
        /// <see cref="XHud_Module_Primitive_Controller.mod_TmpText"/> 兜底（两者互斥）。
        /// </summary>
        private XTween_Interface CreatePreviewTween_Writer(TweenNode node)
        {
            XHud_Module_Primitive_Controller ctrl = target.controller;

            if (ctrl.mod_Text != null)
                return CreatePreviewTween_Writer_Text(node, ctrl.mod_Text);
            if (ctrl.mod_TmpText != null)
                return CreatePreviewTween_Writer_TmpText(node, ctrl.mod_TmpText);

            return null;
        }
        /// <summary>
        /// 打字机 - UGUI Text 版。
        /// <para/>与运行时 <c>Tween_Create_Writer_Text</c> 一致，使用有光标版 <c>xt_Text_To</c>：
        /// 参数顺序 (extended, cursor, endValue, duration, autokill, blinkInterval, rewind, complete)。
        /// </summary>
        private XTween_Interface CreatePreviewTween_Writer_Text(TweenNode node, Text text)
        {
            string targetValue;
            string fromStatic = null;
            bool useStaticFrom = false;
            bool useDynamicFrom = false;

            switch (node.TweenValueMode)
            {
                case TweenValueMode.起始到默认_S_D:
                    targetValue = node.Original_String;
                    fromStatic = node.From_String;
                    useStaticFrom = true;
                    break;

                case TweenValueMode.起始到结束_S_E:
                    targetValue = node.End_String;
                    fromStatic = node.From_String;
                    useStaticFrom = true;
                    break;

                case TweenValueMode.默认到结束_D_E:
                    targetValue = node.End_String;
                    fromStatic = node.Original_String;
                    useStaticFrom = true;
                    break;

                case TweenValueMode.当前到结束_C_E:
                    targetValue = node.End_String;
                    useDynamicFrom = true;
                    break;

                default:
                    return null;
            }

            // Text 版：有光标重载，8 参数
            // 参数顺序 (extended, cursor, endValue, duration, autokill, blinkInterval, rewind, complete)
            XTween_Interface tw = text.xt_Text_To(
                node.TextExtended,                                          // extended
                node.TextCursor,                                // cursor
                targetValue,                                    // endValue
                node.Duration,                                  // duration
                false,                                          // autokill
                node.TextCursorBlinkSpeed > 0
                    ? node.TextCursorBlinkSpeed : 0.5f,         // blinkInterval
                false,                                          // rewind_set_startvalue
                false);                                         // complete_set_endvalue

            if (tw == null) return null;

            if (useStaticFrom)
            {
                tw = tw.SetFrom(fromStatic);
            }
            else if (useDynamicFrom)
            {
                tw = tw.SetFromDynamic(() =>
                {
                    if (text == null) return "";
                    return text.text;
                });
            }

            if (node.Ease == EaseMode.None)
                tw = tw.SetEase(node.Curve);
            else
                tw = tw.SetEase(node.Ease);

            return tw;
        }
        /// <summary>
        /// 打字机 - TextMeshPro 版。
        /// <para/>与运行时 <c>Tween_Create_Writer_TmpText</c> 一致，使用<b>有光标版</b> <c>xt_Text_To</c>：
        /// 参数顺序 (extended, cursor, endValue, duration, autokill, blinkInterval, rewind, complete)。
        /// </summary>
        private XTween_Interface CreatePreviewTween_Writer_TmpText(TweenNode node, TextMeshProUGUI tmp)
        {
            string targetValue;
            string fromStatic = null;
            bool useStaticFrom = false;
            bool useDynamicFrom = false;

            switch (node.TweenValueMode)
            {
                case TweenValueMode.起始到默认_S_D:
                    targetValue = node.Original_String;
                    fromStatic = node.From_String;
                    useStaticFrom = true;
                    break;

                case TweenValueMode.起始到结束_S_E:
                    targetValue = node.End_String;
                    fromStatic = node.From_String;
                    useStaticFrom = true;
                    break;

                case TweenValueMode.默认到结束_D_E:
                    targetValue = node.End_String;
                    fromStatic = node.Original_String;
                    useStaticFrom = true;
                    break;

                case TweenValueMode.当前到结束_C_E:
                    targetValue = node.End_String;
                    useDynamicFrom = true;
                    break;

                default:
                    return null;
            }

            // TmpText 版：有光标重载，8 参数
            // 参数顺序 (extended, cursor, endValue, duration, autokill, blinkInterval, rewind, complete)
            // 与运行时 Tween_Create_Writer_TmpText 一致；autokill 统一传 false
            XTween_Interface tw = tmp.xt_Text_To(
                node.TextExtended,                                          // extended
                node.TextCursor,                                // cursor
                targetValue,                                    // endValue
                node.Duration,                                  // duration
                false,                                          // autokill
                node.TextCursorBlinkSpeed > 0
                    ? node.TextCursorBlinkSpeed : 0.5f,         // blinkInterval
                false,                                          // rewind_set_startvalue
                false);                                         // complete_set_endvalue

            if (tw == null) return null;

            if (useStaticFrom)
            {
                tw = tw.SetFrom(fromStatic);
            }
            else if (useDynamicFrom)
            {
                tw = tw.SetFromDynamic(() =>
                {
                    if (tmp == null) return "";
                    return tmp.text;
                });
            }

            if (node.Ease == EaseMode.None)
                tw = tw.SetEase(node.Curve);
            else
                tw = tw.SetEase(node.Ease);

            return tw;
        }
        #endregion

        #region 飞梭预览驱动 - 清理

        /// <summary>
        /// 清理指定节点的预览 tween（Kill 并移出字典）。
        /// <para/>**不**还原物体——按需求，飞梭驱动只在窗口关闭时统一还原。
        /// </summary>
        private void CleanupPreviewTween(int index)
        {
            if (playheadPreviewTweens.TryGetValue(index, out XTween_Interface tw))
            {
                tw?.Kill(false);
                playheadPreviewTweens.Remove(index);
            }
        }

        /// <summary>
        /// 拖动飞梭结束时（MouseUp）的收尾。
        /// <para/>按决策 (B)：清空预览 tween + 区间标记，使下次拖动必然重建。
        /// <para/>物体状态**不还原**，停在松手时刻的状态。
        /// </summary>
        private void OnPlayheadDragEnd()
        {
            foreach (var kv in playheadPreviewTweens)
                kv.Value?.Kill(false);
            playheadPreviewTweens.Clear();
            playheadPrevInRange.Clear();
        }

        /// <summary>
        /// 窗口关闭时的飞梭预览收尾。
        /// <para/>清空预览 tween（不还原），并在曾驱动过预览时调用
        /// <c>PrimitiveFeature_Load()</c> 还原物体到原始状态。
        /// </summary>
        private void CleanupPlayheadPreviewOnDisable()
        {
            foreach (var kv in playheadPreviewTweens)
                kv.Value?.Kill(false);
            playheadPreviewTweens.Clear();
            playheadPrevInRange.Clear();

            if (playheadPreviewEverDriven)
            {
                if (target != null && !target.Equals(null)
                    && target.controller != null
                    && target.controller.pt_Feature != null)
                {
                    target.controller.pt_Feature.PrimitiveFeature_Load();
                }
                playheadPreviewEverDriven = false;
            }
        }

        #endregion
    }
}