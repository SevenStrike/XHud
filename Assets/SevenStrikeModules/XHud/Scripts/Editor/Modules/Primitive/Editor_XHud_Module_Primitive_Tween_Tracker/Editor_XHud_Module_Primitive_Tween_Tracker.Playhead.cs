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
    using SevenStrikeModules.XGUI.Editor;
    using SevenStrikeModules.XHud.Enums;
    using UnityEditor;
    using UnityEngine;

    public partial class Editor_XHud_Module_Primitive_Tween_Tracker
    {
        /// <summary>
        /// 飞梭头部在 Clip 区局部坐标系中的矩形。
        /// <para/>头部始终贴在刻度尺顶部，横坐标随飞梭时间变化。
        /// <para/>中心 X 经过取整，避免图标因半像素坐标而模糊。
        /// </summary>
        private Rect GetPlayheadHeadRect()
        {
            float x = playheadSecond * pixelsPerSecond - scrollPos.x;
            //  中心对齐到整数像素，图标更锐利
            float centerX = Mathf.Round(x);
            float headX = centerX - PlayheadHeadWidth * 0.5f;
            return new Rect(headX, 0f, PlayheadHeadWidth, rulerHeight);
        }
        /// <summary>
        /// 判断鼠标是否落在飞梭头部的命中区域内。
        /// <para/>命中区域比视觉矩形大 <see cref="PlayheadHeadHitPadding"/> 像素，
        /// 让用户更容易抓到。
        /// </summary>
        private bool HitPlayheadHead(Vector2 mouseLocal)
        {
            Rect head = GetPlayheadHeadRect();
            Rect hit = new Rect(
                head.x - PlayheadHeadHitPadding,
                head.y,
                head.width + PlayheadHeadHitPadding * 2f,
                head.height + PlayheadHeadHitPadding);
            return hit.Contains(mouseLocal);
        }
        /// <summary>
        /// 处理飞梭交互：刻度尺任意位置点击/拖动 → 飞梭跳转并跟随。
        /// <para/>命中规则：
        /// <list type="bullet">
        /// <item><description>MouseDown 落在刻度尺矩形内（含头部）→ 进入拖拽飞梭状态，并立即跳一次；</description></item>
        /// <item><description>MouseDrag 期间不检查是否还在刻度尺内，只按鼠标 X 计算时间；</description></item>
        /// <item><description>MouseUp 结束拖拽。</description></item>
        /// </list>
        /// <para/>必须在 <see cref="HandleTimeRulerClick"/> 与 <see cref="HandleViewPan"/> 之前调用，
        /// 否则会被它们抢走事件。
        /// </summary>
        /// <param name="rulerRect">刻度尺局部矩形（Clip 区坐标，通常是 (0,0,width,rulerHeight)）</param>
        /// <param name="clipAreaLocalRect">Clip 区整体局部矩形（用于算内容总时长钳制）</param>
        private void HandlePlayheadInteraction(Rect rulerRect, Rect clipAreaLocalRect)
        {
            Event e = Event.current;
            int controlID = GUIUtility.GetControlID(FocusType.Passive);

            switch (e.type)
            {
                case EventType.MouseDown:
                    // 左键 + 非 Alt + 落在刻度尺内 + 当前无其他 hotControl
                    if (e.button == 0 && !e.alt && GUIUtility.hotControl == 0
                        && rulerRect.Contains(e.mousePosition))
                    {
                        //  [新增] 播放中拖飞梭，先停播放。
                        // 否则 OnPlaybackEditorUpdate 与 UpdatePlayheadFromMouse
                        // 会同时写 playheadSecond，互相打架（拖不动 / 跳来跳去）。
                        StopPlayback();

                        isDraggingPlayhead = true;
                        playheadControlID = controlID;
                        GUIUtility.hotControl = controlID;
                        GUIUtility.keyboardControl = 0;

                        //  关键：按下即跳转，不管点的是头部还是空白
                        UpdatePlayheadFromMouse(e.mousePosition, clipAreaLocalRect);

                        //EditorApplication.QueuePlayerLoopUpdate();

                        e.Use();
                        Repaint();
                    }
                    break;

                case EventType.MouseDrag:
                    if (isDraggingPlayhead && GUIUtility.hotControl == playheadControlID)
                    {
                        // 拖动期间不限制在刻度尺内，鼠标跑到轨道区甚至窗口外也继续跟随 X
                        //  [不动] 这里绝不能再加 rulerRect.Contains 判断，否则越界跟随失效
                        UpdatePlayheadFromMouse(e.mousePosition, clipAreaLocalRect);

                        e.Use();
                        Repaint();
                    }
                    break;

                case EventType.MouseUp:
                    if (isDraggingPlayhead && e.button == 0)
                    {
                        isDraggingPlayhead = false;
                        GUIUtility.hotControl = 0;
                        playheadControlID = 0;
                        snapGuideSecond = -1f;

                        //  飞梭拖动结束收尾（清空预览 tween + 区间标记）
                        OnPlayheadDragEnd();

                        //EditorApplication.QueuePlayerLoopUpdate();

                        e.Use();
                        Repaint();
                    }
                    break;
            }
        }
        /// <summary>
        /// 根据鼠标位置更新飞梭时间。
        /// <para/>按住 Shift 时，会尝试把飞梭吸附到所有 Clip（动画 + 音效）的左右两端；
        /// 吸附成功时把命中时间写入 <see cref="snapGuideSecond"/>，由 <see cref="DrawClipTimelineArea"/>
        /// 统一绘制吸附参考线。
        /// </summary>
        private void UpdatePlayheadFromMouse(Vector2 mouseLocal, Rect clipAreaLocalRect)
        {
            // 鼠标 X 相对视口左边缘 → 加 scrollPos.x → 除以 pixelsPerSecond → 内容时间
            float contentPixelX = mouseLocal.x + scrollPos.x;
            float rawSecond = contentPixelX / Mathf.Max(1f, pixelsPerSecond);

            // 钳制到 [0, 内容总时长]，避免飞梭跑出内容范围
            float totalSeconds = CalculateContentWidthPixels() / Mathf.Max(1f, pixelsPerSecond);
            float clamped = Mathf.Clamp(rawSecond, 0f, totalSeconds);

            // 先量化，再做吸附（吸附方法内部也会量化，这里保持一致的手感基准）
            clamped = QuantizeTime(clamped);

            // ── 按住 Shift 时启用吸附 ──
            if (Event.current != null && Event.current.shift)
            {
                float snapped;
                bool hit = TrySnapTimeForPlayhead(clamped, out snapped);
                if (hit)
                {
                    playheadSecond = snapped;
                    snapGuideSecond = snapped;   //  让吸附线可见
                }
                else
                {
                    playheadSecond = clamped;
                    snapGuideSecond = -1f;
                }
            }
            else
            {
                playheadSecond = clamped;
                snapGuideSecond = -1f;
            }

            //  驱动飞梭预览（仅写属性，刷新由调用方按场景决定）
            RefreshPlayheadPreview(playheadSecond);

            ////  你要的 Debug.Log
            //Debug.Log($"[XHud] 时间飞梭：{playheadSecond:F3} 秒");
        }
        /// <summary>
        /// 绘制飞梭：贯穿刻度尺 + 轨道区的竖线 + 顶部头部。
        /// <para/>必须在 <see cref="GUI.EndScrollView"/> 之后绘制，
        /// 否则会被 ScrollView 裁剪掉超出内容区的部分。
        /// </summary>
        /// <param name="clipAreaLocalRect">Clip 区局部矩形</param>
        private void DrawPlayhead(Rect clipAreaLocalRect)
        {
            float x = playheadSecond * pixelsPerSecond - scrollPos.x;

            // 飞梭在视口外 → 不绘制
            if (x < -PlayheadHeadWidth || x > clipAreaLocalRect.width + PlayheadHeadWidth)
                return;

            // ── 竖线：从刻度尺底部一直画到 Clip 区底部（水平滚动条之上）──
            float lineTop = rulerHeight * 0.5f;   // 从刻度尺中线开始
            float lineBottom = clipAreaLocalRect.height - HorizontalScrollbarHeight;
            if (lineBottom > lineTop)
            {
                float lineX = Mathf.Round(x);

                //  飞梭落在 C-E 区间内时，竖线用绿色以示区别
                Color lineColor = IsPlayheadInCEInterval(playheadSecond)
                    ? ColorPlayhead_C_E
                    : ColorPlayheadLine;

                XGUI.gui_box(
                    new Rect(lineX, lineTop, 1f, lineBottom - lineTop),
                    lineColor);
            }

            // ── 头部：自定义图标 ──
            Rect head = GetPlayheadHeadRect();
            bool hover = !isDraggingPlayhead && HitPlayheadHead(Event.current.mousePosition);
            Color headColor = (hover || isDraggingPlayhead) ? ColorPlayheadHeadHover : ColorPlayheadHead;

            if (icon_playhead != null)
            {
                XGUI.gui_icon(
                    rect: head,
                    icon: icon_playhead,
                    color: headColor);
            }
            else
            {
                // 兜底：图标未加载时退回纯色方块，避免完全看不见
                XGUI.gui_box(head, headColor);
            }
        }
        /// <summary>
        /// 判断飞梭当前时间是否落在任意一个 C-E 节点的区间内。
        /// <para/>用于 <see cref="DrawPlayhead"/> 决定竖线颜色：
        /// 落在 C-E 区间时竖线变绿，提示"此段是动作式，反向拖动不回退"。
        /// <para/>判定范围：所有 Enabled 节点，所有 TweenNodeType，不区分是否已实现飞梭驱动。
        /// <para/>区间定义：t ∈ [Delay, Delay + Duration]。
        /// </summary>
        /// <param name="t">飞梭时间（内容绝对秒）</param>
        /// <returns>true 表示落在至少一个 C-E 区间内</returns>
        private bool IsPlayheadInCEInterval(float t)
        {
            if (Nodes == null || Nodes.Count == 0) return false;

            for (int i = 0; i < Nodes.Count; i++)
            {
                TweenNode node = Nodes[i];
                if (node == null) continue;
                if (!node.Enabled) continue;
                if (node.TweenValueMode != TweenValueMode.当前到结束_C_E) continue;

                // 零时长节点无区间，跳过（与 DrivePlayheadPreview 的守卫一致）
                if (node.Duration <= 0f) continue;

                if (t >= node.Delay && t <= node.Delay + node.Duration)
                    return true;
            }

            return false;
        }
        /// <summary>
        /// 在 Repaint 阶段为飞梭头部设置鼠标光标。
        /// </summary>
        private void DrawPlayheadCursor(Rect rulerRect)
        {
            if (Event.current.type != EventType.Repaint) return;

            // 拖拽中 → 整个刻度尺显示横向光标
            if (isDraggingPlayhead)
            {
                EditorGUIUtility.AddCursorRect(rulerRect, MouseCursor.ResizeHorizontal);
                return;
            }

            // 悬停在刻度尺内 → 显示横向光标（头部/空白一视同仁，因为都能拖）
            if (rulerRect.Contains(Event.current.mousePosition))
            {
                EditorGUIUtility.AddCursorRect(rulerRect, MouseCursor.ResizeHorizontal);
            }
        }
    }
}
