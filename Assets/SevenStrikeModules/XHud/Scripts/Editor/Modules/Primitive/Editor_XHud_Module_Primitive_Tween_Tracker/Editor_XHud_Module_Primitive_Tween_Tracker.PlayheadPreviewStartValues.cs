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
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    public partial class Editor_XHud_Module_Primitive_Tween_Tracker
    {
        #region 飞梭跳开头 - 每类型第一个 Clip 的起始值应用
        /// <summary>
        /// 【实验性】按类型取"每类型 Delay 最小的 Clip"，把这些 Clip 的起始值应用到物体。
        /// <para/>
        /// 规则（与 <see cref="ApplyLastClipEndValuesByType"/> 镜像）：
        /// <list type="bullet">
        /// <item><description>按 <see cref="TweenNode.Type"/> 分组；</description></item>
        /// <item><description>每组内取 <c>Delay</c> 最小的那个节点（并列时取列表中索引更小的，即先出现的优先）；</description></item>
        /// <item><description>把选中节点的"起始值"应用到目标组件；</description></item>
        /// <item><description>跳过 <c>!Enabled</c> 的节点；</description></item>
        /// <item><description>跳过 <c>Duration &lt;= 0</c> 的节点；</description></item>
        /// <item><description>跳过 <c>C-E</c> 模式（该模式无静态起点，硬写无从下手）。</description></item>
        /// </list>
        /// <para/>
        /// 起点字段按 <see cref="TweenValueMode"/> 决定：
        /// <list type="bullet">
        /// <item><description>S-D 模式：起点 = <c>From_*</c>；</description></item>
        /// <item><description>S-E 模式：起点 = <c>From_*</c>；</description></item>
        /// <item><description>D-E 模式：起点 = <c>Original_*</c>；</description></item>
        /// <item><description>C-E 模式：无静态起点，跳过该节点。</description></item>
        /// </list>
        /// </summary>
        public void ApplyFirstClipStartValuesByType()
        {
            if (target == null || target.Equals(null)) return;
            if (Nodes == null || Nodes.Count == 0) return;
            if (target.controller == null) return;

            //   新增：标记"曾对组件写过值"，让窗口关闭时能正确还原
            hasModifiedComponent = true;

            // ── 按类型分组，取每组 Delay 最小的节点 ──
            // 并列 Delay 时取先出现的（严格小于，先出现的优先）
            Dictionary<TweenNodeType, TweenNode> earliestByType =
                new Dictionary<TweenNodeType, TweenNode>();

            for (int i = 0; i < Nodes.Count; i++)
            {
                TweenNode node = Nodes[i];
                if (node == null) continue;
                if (!node.Enabled) continue;
                if (node.Duration <= 0f) continue;
                if (node.TweenValueMode == TweenValueMode.当前到结束_C_E) continue;

                TweenNodeType type = node.Type;

                if (!earliestByType.TryGetValue(type, out TweenNode current))
                {
                    earliestByType[type] = node;
                }
                else
                {
                    // 严格小于：并列时保留先出现的
                    if (node.Delay < current.Delay)
                        earliestByType[type] = node;
                }
            }

            // ── 逐个类型，把起始值应用到组件 ──
            foreach (var kv in earliestByType)
            {
                ApplyNodeStartValueToTarget(kv.Value);
            }

            // 刷新视图
            SceneView.RepaintAll();
            Repaint();
            EditorApplication.QueuePlayerLoopUpdate();
        }
        /// <summary>
        /// 把单个节点的"动画起点值"应用到目标组件。
        /// <para/>起点字段按 <see cref="TweenValueMode"/> 决定：
        /// S-D / S-E 用 <c>From_*</c>，D-E 用 <c>Original_*</c>。
        /// <para/>C-E 模式无静态起点，调用方应提前跳过。
        /// </summary>
        private void ApplyNodeStartValueToTarget(TweenNode node)
        {
            if (node == null) return;
            if (target == null || target.controller == null) return;

            // S-D / S-E 的起点是 From；D-E 的起点是 Original
            bool useFromAsStart = (node.TweenValueMode == TweenValueMode.起始到默认_S_D ||
                                   node.TweenValueMode == TweenValueMode.起始到结束_S_E);

            switch (node.Type)
            {
                // ── 位移：RectTransform.anchoredPosition3D ──
                case TweenNodeType.a_位移:
                    if (target.controller.mod_Rect != null)
                        target.controller.mod_Rect.anchoredPosition3D =
                            useFromAsStart ? node.From_Vector3 : node.Original_Vector3;
                    break;

                // ── 旋转：RectTransform.localEulerAngles ──
                case TweenNodeType.r_旋转:
                    if (target.controller.mod_Rect != null)
                        target.controller.mod_Rect.localEulerAngles =
                            useFromAsStart ? node.From_Vector3 : node.Original_Vector3;
                    break;

                // ── 缩放：RectTransform.localScale ──
                case TweenNodeType.s_缩放:
                    if (target.controller.mod_Rect != null)
                        target.controller.mod_Rect.localScale =
                            useFromAsStart ? node.From_Vector3 : node.Original_Vector3;
                    break;

                // ── 颜色：Graphic.color ──
                case TweenNodeType.c_颜色:
                    {
                        var gc = target.controller.RecognizeType();
                        if (gc != null)
                            gc.color = useFromAsStart ? node.From_Color : node.Original_Color;
                    }
                    break;

                // ── 淡化：CanvasGroup.alpha ──
                case TweenNodeType.g_淡化:
                    if (target.controller.mod_CanvasGroup != null)
                        target.controller.mod_CanvasGroup.alpha =
                            useFromAsStart ? node.From_Float : node.Original_Float;
                    break;

                // ── 打字机：Text / TmpText 内容 ──
                case TweenNodeType.w_打字机:
                    {
                        string content = useFromAsStart ? node.From_String : node.Original_String;
                        if (target.controller.mod_Text != null)
                        {
                            target.controller.mod_Text.txt_Set_Content(content);
                        }
                        else if (target.controller.mod_TmpText != null)
                        {
                            target.controller.mod_TmpText.tmp_Set_Content(content);
                        }
                    }
                    break;

                // ── 图像填充：Image.fillAmount ──
                case TweenNodeType.f_图像填充:
                    if (target.controller.mod_Image != null)
                        target.controller.mod_Image.fillAmount =
                            Mathf.Clamp01(useFromAsStart ? node.From_Float : node.Original_Float);
                    break;

                // ── 尺寸：RectTransform.sizeDelta ──
                case TweenNodeType.z_尺寸:
                    if (target.controller.mod_Rect != null)
                        target.controller.mod_Rect.sizeDelta =
                            useFromAsStart ? node.From_Vector2 : node.Original_Vector2;
                    break;
            }
        }
        /// <summary>
        /// 【实验性】跳到开头并应用每类型第一个 Clip 的起始值。
        /// <para/>与 <see cref="JumpToTimelineEndAndApplyLastValues"/> 镜像。
        /// </summary>
        public void JumpToTimelineStartAndApplyFirstValues()
        {
            StopPlayback();

            playheadSecond = 0f;
            snapGuideSecond = -1f;

            //  清预览缓存：飞梭位置发生大跳跃，缓存里的"上帧状态"已失效
            OnPlayheadDragEnd();

            // 跳转时重置音效区间标记（避免飞梭跳跃后残留旧音效）
            ResetSoundDriveBaseline();

            // 硬写每类型第一个 Clip 的起点值
            ApplyFirstClipStartValuesByType();

            Repaint();
        }
        #endregion
    }
}