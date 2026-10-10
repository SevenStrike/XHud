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
    using SevenStrikeModules.XGUI.Runtime;
    using SevenStrikeModules.XHud.Enums;
    using UnityEditor;
    using UnityEngine;

    public partial class Editor_XHud_Module_Primitive_Tween_Tracker
    {
        /// <summary> 
        /// 绘制 Clip 时间轴区：刻度尺（固定）+ 双向 ScrollView（轨道 + Clip）+ 自绘水平滚动条
        /// <para/>
        /// 本方法是 Clip 区的总入口，按以下顺序执行：
        /// <list type="number">
        /// <item><description>绘制区域底色，并绘制顶部固定刻度尺；</description></item>
        /// <item><description>开启双向 ScrollView，逐条绘制可见轨道（垂直裁剪，屏幕外轨道跳过）；</description></item>
        /// <item><description>在 ScrollView 内处理 Clip 拖拽（<see cref="ProcessClipDrag"/>）；</description></item>
        /// <item><description>关闭 ScrollView 后钳制垂直滚动，并绘制吸附 / 边界参考线；</description></item>
        /// <item><description>绘制底部自绘水平滚动条；</description></item>
        /// <item><description>处理刻度尺点击、空白点击、平移三类交互。</description></item>
        /// </list>
        /// 注意：本方法通过 <see cref="GUI.BeginGroup"/> 将局部坐标系原点移到 Clip 区左上角，
        /// 方法内所有 Rect 均使用 Clip 区局部坐标。
        /// </summary>
        /// <param name="area">Clip 区在窗口坐标系中的矩形区域</param>
        private void DrawClipTimelineArea(Rect area)
        {
            GUI.BeginGroup(area);
            XGUI.gui_box(new Rect(0, 0, area.width, area.height), ColorBasedBg);

            // ── 阶段 1：绘制顶部刻度尺 ──
            Rect rulerRect = new Rect(0, 0, area.width, rulerHeight);
            float startSecond = scrollPos.x / pixelsPerSecond;
            float endSecond = (scrollPos.x + area.width) / pixelsPerSecond;
            DrawTimeRuler(rulerRect, startSecond, endSecond);

            //  阶段 2：绘制底部水平滚动条（必须在 BeginScrollView 之前）
            // 原因：GUI.HorizontalScrollbar 拖拽期间需要独占 Event.current，
            // 若画在 ScrollView 之后，ScrollView 的垂直滚动条会抢占鼠标热区，
            // 导致水平滚动条在鼠标靠近右边缘时丢失 hotControl，表现为"拖到某个位置就拖不动"。
            Rect hScrollRect = new Rect(
                0,
                area.height - HorizontalScrollbarHeight,
                area.width,
                HorizontalScrollbarHeight);
            DrawTimelineHorizontalScrollbar(hScrollRect);

            // ── 阶段 3：开启双向 ScrollView ──
            Rect scrollViewportRect = new Rect(
                0, rulerHeight, area.width,
                area.height - rulerHeight - HorizontalScrollbarHeight);

            float contentWidth = CalculateContentWidthPixels();
            float contentHeight = GetTotalContentHeight();
            Rect contentRect = new Rect(0, 0, contentWidth, contentHeight);

            scrollPos = GUI.BeginScrollView(
                scrollViewportRect,
                scrollPos,
                contentRect,
                false,
                false,
                GUIStyle.none,
                GUI.skin.verticalScrollbar);

            // ── 阶段 4：逐条绘制轨道 ──
            clipHitThisFrame = false;
            for (int row = 0; row < TotalRowCount; row++)
            {
                Rect trackRect = GetRowHitRect(row, contentWidth);
                float viewTop = scrollPos.y;
                float viewBottom = scrollPos.y + scrollViewportRect.height;
                if (trackRect.yMax < viewTop || trackRect.y > viewBottom) continue;

                var (kind, idx) = ResolveRow(row);
                if (kind == TrackKind.Node)
                    DrawTrackRow(trackRect, idx, startSecond, endSecond);
                else if (kind == TrackKind.Sound)
                    DrawSoundTrackRow(trackRect, idx);
                else if (kind == TrackKind.Gap)
                    DrawTrackKindGapRow(trackRect);
            }

            // ── 阶段 5：处理 Clip 拖拽 ──
            ProcessClipDrag(scrollViewportRect);
            GUI.EndScrollView();

            // ── 阶段 6：钳制垂直滚动 ──
            scrollPos.y = Mathf.Clamp(scrollPos.y, 0f, CalculateMaxVerticalScroll());
            nameScroll.y = scrollPos.y;

            //  新增：绘制飞梭（在参考线之前，让参考线覆盖在飞梭上方更显眼）
            DrawPlayhead(new Rect(0, 0, area.width, area.height));

            #region 参考线（覆盖刻度尺 + 轨道区）
            // 吸附参考线：拖拽中且本帧吸附到有效时间时，绘制一条白色竖线（按住 Shift 更亮）
            if ((dragMode != DragMode.无 || isDraggingPlayhead) && snapGuideSecond >= 0f)
            {
                float gx = snapGuideSecond * pixelsPerSecond - scrollPos.x;
                if (gx >= 0f && gx <= area.width)
                {
                    Color guideColor = Event.current.shift
                        ? Color.white
                        : Color.white * 0.7f;
                    guideColor.a = 1f;

                    float guideX = Mathf.Round(gx);   //  新增
                    XGUI.gui_box(new Rect(guideX, 0, 1f, area.height), guideColor);
                }
            }

            // 边界参考线：Move 拖拽时，在 Clip 的左右两端各绘制一条暗色竖线，
            // 便于用户判断整段动画的首尾位置（区别于吸附黄线）。
            if (dragMode == DragMode.移动 && draggingKind == TrackKind.Node && draggingIndex >= 0 && draggingIndex < Nodes.Count)
            {
                TweenNode draggingNode = Nodes[draggingIndex];
                float leftX = draggingNode.Delay * pixelsPerSecond - scrollPos.x;
                float rightX = (draggingNode.Delay + draggingNode.Duration) * pixelsPerSecond - scrollPos.x;
                if (leftX >= 0f && leftX <= area.width)
                    XGUI.gui_box(new Rect(leftX, 0, 1f, area.height), ColorClipEdgeGuide);
                if (rightX >= 0f && rightX <= area.width)
                    XGUI.gui_box(new Rect(rightX, 0, 1f, area.height), ColorClipEdgeGuide);
            }

            // 音效 Clip：Move 拖拽时同样绘制左右边界参考线
            if (dragMode == DragMode.移动 && draggingKind == TrackKind.Sound && draggingIndex >= 0 && draggingIndex < Sounds.Count)
            {
                TweenSound draggingSound = Sounds[draggingIndex];
                float sLen = draggingSound.Sound != null ? draggingSound.Sound.length : DefaultSoundClipSeconds;
                float sLeftX = draggingSound.Delay * pixelsPerSecond - scrollPos.x;
                float sRightX = (draggingSound.Delay + sLen) * pixelsPerSecond - scrollPos.x;
                if (sLeftX >= 0f && sLeftX <= area.width)
                    XGUI.gui_box(new Rect(sLeftX, 0, 1f, area.height), ColorClipEdgeGuide);
                if (sRightX >= 0f && sRightX <= area.width)
                    XGUI.gui_box(new Rect(sRightX, 0, 1f, area.height), ColorClipEdgeGuide);
            }
            #endregion

            //  注意：这里不再调用 DrawTimelineHorizontalScrollbar，已经挪到阶段 2 了

            // ── 阶段 7：交互处理 ──
            //  飞梭交互：命中范围是刻度尺（不是整个 Clip 区），必须最先
            HandlePlayheadInteraction(
                new Rect(0, 0, area.width, rulerHeight),   // 刻度尺矩形
                new Rect(0, 0, area.width, area.height));  // Clip 区整体矩形

            if (!isDraggingPlayhead)
            {
                // 拖拽飞梭期间，跳过这些交互，避免冲突
                // 注意：HandleTimeRulerClick 现在基本不会被触发（飞梭已接管刻度尺左键），
                //       保留它是为了兜底——比如未来飞梭交互被禁用时仍能清焦点。
                HandleTimeRulerClick(new Rect(0, 0, area.width, rulerHeight));
                HandleClipAreaEmptyClick(scrollViewportRect);
                HandleViewPan(new Rect(0, 0, area.width, area.height));
            }

            // ── 阶段 8：飞梭光标 ──
            DrawPlayheadCursor(new Rect(0, 0, area.width, rulerHeight));

            GUI.EndGroup();
        }
        /// <summary> 
        /// 自绘 Clip 区底部的水平滚动条
        /// <para/>
        /// 使用 <see cref="GUI.HorizontalScrollbar"/> 绘制，但前提是内容宽度大于视口宽度：
        /// <list type="bullet">
        /// <item><description>内容 ≤ 视口：整个矩形只铺一层深色底，不绘制可拖拽的滑块
        /// （此时水平方向无滚动空间，绘制滑块反而误导用户）；</description></item>
        /// <item><description>内容 &gt; 视口：绘制标准 Unity 水平滚动条，并将拖动结果写回
        /// <see cref="scrollPos"/>.x。</description></item>
        /// </list>
        /// <para/>
        /// 注意：本方法内部对 <see cref="scrollPos"/>.x 做了 <see cref="Mathf.Max"/> 归零保护，
        /// 因为 <see cref="GUI.HorizontalScrollbar"/> 在内容刚好等于视口宽度时可能返回极小负值。
        /// </summary>
        /// <param name="rect">水平滚动条矩形（Clip 区局部坐标，位于 Clip 区最底部）</param>
        private void DrawTimelineHorizontalScrollbar(Rect rect)
        {
            float contentWidth = CalculateContentWidthPixels();
            float viewWidth = cachedClipAreaRect.width;

            // 分支 1：内容不超过视口 → 只铺底，并把 scrollPos.x 归零
            if (contentWidth <= viewWidth)
            {
                EditorGUI.DrawRect(rect, new Color(0.15f, 0.15f, 0.15f));
                scrollPos.x = 0f;   //  归零，避免残留旧滚动值
                return;
            }

            // 分支 2：内容超过视口 → 绘制滚动条
            EditorGUI.DrawRect(rect, new Color(0.15f, 0.15f, 0.15f));
            scrollPos.x = GUI.HorizontalScrollbar(
                rect,
                scrollPos.x,
                viewWidth,
                0f,
                contentWidth);

            //  归零保护 + 上限钳制
            scrollPos.x = Mathf.Clamp(scrollPos.x, 0f, contentWidth - viewWidth);
        }
        /// <summary>
        /// 绘制动画轨与音效轨之间的间距行（Clip 区一侧）。
        /// <para/>
        /// 本方法是一个**独立的自定义绘制入口**：间距行不承载任何轨道数据，
        /// 可以在这里自由放置标题、分组标签、折叠按钮、装饰线等。
        /// <para/>
        /// 目前的实现：铺一层比轨道底色略深的底色 + 上下描边，
        /// 形成视觉上的"凹陷分隔带"。
        /// </summary>
        /// <param name="gapRect">间距行的完整矩形（内容坐标，高 = <see cref="trackKindGapHeight"/>）</param>
        private void DrawTrackKindGapRow(Rect gapRect)
        {
            //  同名字列：上下各扩展 1px
            gapRect = new Rect(
                gapRect.x,
                gapRect.y - RowVerticalPadding,
                gapRect.width,
                gapRect.height + RowVerticalPadding * 2f);

            XGUI.gui_box(gapRect, ColorTrackKindGapBg);

            //XGUI.gui_box(new Rect(gapRect.x, gapRect.y, gapRect.width, 1f), ColorTrackKindGapEdge);
            //XGUI.gui_box(new Rect(gapRect.x, gapRect.yMax - 1f, gapRect.width, 1f), ColorTrackKindGapEdge);
        }
        /// <summary> 
        ///绘制顶部刻度尺
        /// </summary>
        /// <param name="rect">刻度尺矩形（Clip 区局部坐标）</param>
        /// <param name="startSecond">可见区左边缘对应的时间（秒）</param>
        /// <param name="endSecond">可见区右边缘对应的时间（秒）</param>
        private void DrawTimeRuler(Rect rect, float startSecond, float endSecond)
        {
            EditorGUI.DrawRect(rect, ColorRulerBG);
            int subdiv;
            if (pixelsPerSecond >= 8000f) subdiv = 30;
            else if (pixelsPerSecond >= 4000f) subdiv = 10;
            else if (pixelsPerSecond >= 2000f) subdiv = 10;
            else if (pixelsPerSecond >= 1000f) subdiv = 10;
            else if (pixelsPerSecond >= 500f) subdiv = 10;
            else if (pixelsPerSecond >= 250f) subdiv = 10;
            else if (pixelsPerSecond >= 100f) subdiv = 10;
            else if (pixelsPerSecond >= 50f) subdiv = 10;
            else subdiv = 2;
            float minorStep = 1f / subdiv;
            int i0 = Mathf.FloorToInt(startSecond / minorStep);
            int i1 = Mathf.CeilToInt(endSecond / minorStep);
            for (int i = i0; i <= i1; i++)
            {
                float t = i * minorStep;
                float x = t * pixelsPerSecond - scrollPos.x;
                if (x < rect.x - 1 || x > rect.xMax + 1) continue;

                float distToInt = Mathf.Abs(t - Mathf.Round(t));
                bool isMajor = distToInt < minorStep * 0.01f;
                bool isHalf = !isMajor
                              && Mathf.Abs(t * 2f - Mathf.Round(t * 2f)) < minorStep * 0.01f;

                // 三档刻度线高度：整秒最高、半秒次之、细分最低
                float lineTop;
                if (isMajor) lineTop = 8f;
                else if (isHalf) lineTop = 15f;
                else lineTop = 20f;

                Color lineColor;
                if (isMajor) lineColor = Color.white * 0.85f;
                else if (isHalf) lineColor = new Color(0.65f, 0.65f, 0.65f);
                else lineColor = new Color(0.4f, 0.4f, 0.4f);

                XGUI.gui_box(new Rect(x, rect.y + lineTop, 1f, rect.height - lineTop), lineColor);

                // 整秒显示 "N s"
                if (isMajor)
                {
                    XGUI.gui_label(
                        rect: new Rect(x + 6, rect.y - 4, 70, rect.height),
                        text: new GUIContent($"{t:F0} s"),
                        text_color: Color.white * 0.8f,
                        size: XGUIFontSize.M,
                        clipping: TextClipping.Clip,
                        anchor: TextAnchor.MiddleLeft,
                        font_style: FontStyle.Normal);
                }
                // 半秒显示 "N.5 s"
                else if (isHalf)
                {
                    XGUI.gui_label(
                        rect: new Rect(x + 6, rect.y - 4, 70, rect.height),
                        text: new GUIContent($"{t:0.0} s"),
                        text_color: Color.white * 0.75f,
                        size: XGUIFontSize.S,
                        clipping: TextClipping.Clip,
                        anchor: TextAnchor.MiddleLeft,
                        font_style: FontStyle.Normal);
                }
            }
        }
        /// <summary> 
        ///绘制单条轨道及其 Clip，并进行 MouseDown 命中检测以进入拖拽模式
        /// </summary>
        /// <param name="trackRect">轨道矩形（内容坐标，高 = trackHeight）</param>
        /// <param name="index">动画节点索引</param>
        /// <param name="startSecond">可见区左边缘对应的时间（秒）当前未使用，保留供扩展</param>
        /// <param name="endSecond">可见区右边缘对应的时间（秒）当前未使用，保留供扩展</param>
        private void DrawTrackRow(Rect trackRect, int index, float startSecond, float endSecond)
        {
            TweenNode node = Nodes[index];
            bool isSelected = IsSelected(TrackKind.Node, index);
            Rect trackVisualRect = CalculateTrackVisualRect(trackRect);
            XGUI.gui_box(trackVisualRect, ColorClipBG);
            Rect clipRect = CalculateClipRect(trackVisualRect, node);
            float viewLeft = scrollPos.x;
            float viewRight = scrollPos.x + cachedClipAreaRect.width;
            if (clipRect.xMax < viewLeft - 10 || clipRect.x > viewRight + 10)
                return;
            DrawClipVisual(clipRect, node, isSelected);
            TryBeginClipDrag(clipRect, index);
        }
        /// <summary> 
        ///由轨道命中矩形计算轨道视觉矩形：上下按固定像素 <see cref="RowVerticalPadding"/> 内缩
        /// </summary>
        /// <param name="trackRect">轨道命中矩形（高 = trackHeight）</param>
        /// <returns>轨道视觉矩形（已内缩）</returns>
        private Rect CalculateTrackVisualRect(Rect trackRect)
        {
            float pad = RowVerticalPadding;
            return new Rect(
                trackRect.x,
                trackRect.y + pad,
                trackRect.width,
                Mathf.Max(trackRect.height - pad * 2f, 4f));
        }
        /// <summary> 
        ///绘制 Clip 自身的视觉样式：背景色块、文本标签、鼠标光标
        /// </summary>
        /// <param name="clipRect">Clip 矩形（内容坐标）</param>
        /// <param name="node">对应的动画节点</param>
        /// <param name="isSelected">该节点当前是否被选中</param>
        private void DrawClipVisual(Rect clipRect, TweenNode node, bool isSelected)
        {
            #region 轨道条
            Color clipColor = ColorClipFG_Normal;
            if (!node.Enabled)
                clipColor = ColorClipFG_Disabled;
            if (isSelected)
                clipColor = !node.Enabled ? ColorClipFG_Selected * 0.6f : XHud_Dashboard.Theme_Primary;
            XGUI.gui_box(clipRect, clipColor);
            #endregion

            #region 类型指示条
            Color clip_type_Color = GetTweenTypeColor(node.Type);
            if (!node.Enabled)
                clip_type_Color *= 0.55f;
            if (isSelected)
                clip_type_Color = Color.Lerp(clip_type_Color, Color.white, 0.25f);
            if (!isSelected)
                XGUI.gui_box(new Rect(clipRect.x, clipRect.y + clipRect.height - ClipTypeLineThickness, clipRect.width, ClipTypeLineThickness), clip_type_Color);
            #endregion

            #region 概述信息
            bool is_minmal = clipRect.width < 150;
            bool is_very_minmal = clipRect.width <= 90;

            if (!is_very_minmal)
            {
                string twn_content = $"{(is_minmal ? "" : "延迟：")}{node.Delay:F3}   /   {(is_minmal ? "" : "耗时：")}{node.Duration:F3}";
                XGUI.gui_label(
                    rect: new Rect(clipRect.x + 4, clipRect.y, clipRect.width - 8, clipRect.height),
                    text: new GUIContent(twn_content),
                    text_color: isSelected ? (!node.Enabled ? Color.white * 0.7f : Color.black) : Color.white,
                    size: XGUIFontSize.M,
                    clipping: TextClipping.Clip,
                    anchor: TextAnchor.MiddleCenter,
                    font_style: FontStyle.Normal);
            }
            #endregion

            #region 类型图标
            if (is_very_minmal)
            {
                XGUI.gui_icon(
                      rect: new Rect(clipRect.x + (clipRect.width / 2 - 6), clipRect.y + (trackHeight / 2 - 7), 12, 12),
                      icon: GetTweenTypeIcon(node.Type, false),
                      border: new RectOffset(0, 0, 0, 0),
                      color: isSelected ? Color.black : Color.white * 0.9f);
            }
            else
            {
                if (clipRect.width >= 220)
                {
                    XGUI.gui_icon(
                        rect: new Rect(clipRect.x + 15, clipRect.y + (trackHeight / 2 - 7), 12, 12),
                        icon: GetTweenTypeIcon(node.Type, false),
                        border: new RectOffset(0, 0, 0, 0),
                        color: isSelected ? Color.black : Color.white * 0.9f);

                    XGUI.gui_icon(
                        rect: new Rect(clipRect.x + (clipRect.width - 25), clipRect.y + (trackHeight / 2 - 7), 12, 12),
                        icon: GetTweenTypeIcon(node.Type, false),
                        border: new RectOffset(0, 0, 0, 0),
                        color: isSelected ? Color.black : Color.white * 0.9f);
                }
            }
            #endregion

            ApplyClipMouseCursors(clipRect);
        }
        /// <summary> 
        ///根据轨道矩形与节点数据计算 Clip 的内容坐标矩形
        /// </summary>
        /// <param name="trackVisualRect">轨道视觉矩形（内容坐标）</param>
        /// <param name="node">动画节点</param>
        /// <returns>Clip 矩形（内容坐标）</returns>
        private Rect CalculateClipRect(Rect trackVisualRect, TweenNode node)
        {
            float clipX = trackVisualRect.x + node.Delay * pixelsPerSecond;
            float clipW = node.Duration * pixelsPerSecond;
            return new Rect(
                Mathf.Round(clipX),
                trackVisualRect.y,
                Mathf.Max(Mathf.Round(clipW), 4f),
                trackVisualRect.height);
        }
        /// <summary> 
        ///为 Clip 设置鼠标光标形状：主体为移动光标，左右边缘为水平缩放光标
        /// </summary>
        /// <param name="clipRect">Clip 矩形（内容坐标）</param>
        private void ApplyClipMouseCursors(Rect clipRect)
        {
            Rect bodyRect = new Rect(clipRect.x + ClipEdgeZone, clipRect.y,
                                     Mathf.Max(clipRect.width - ClipEdgeZone * 2, 1), clipRect.height);
            EditorGUIUtility.AddCursorRect(bodyRect, MouseCursor.MoveArrow);
            Rect leftEdge = new Rect(clipRect.x - 2, clipRect.y, ClipEdgeZone + 2, clipRect.height);
            EditorGUIUtility.AddCursorRect(leftEdge, MouseCursor.ResizeHorizontal);
            Rect rightEdge = new Rect(clipRect.xMax - ClipEdgeZone - 2, clipRect.y, ClipEdgeZone + 2, clipRect.height);
            EditorGUIUtility.AddCursorRect(rightEdge, MouseCursor.ResizeHorizontal);
        }
        /// <summary> 
        ///根据动画类型获取 Clip 的显示颜色
        /// </summary>
        /// <param name="type">动画节点类型</param>
        /// <returns>该类型对应的颜色</returns>
        private Color GetTweenTypeColor(TweenNodeType type)
        {
            switch (type)
            {
                case TweenNodeType.a_位移: return new Color(0.2f, 0.5f, 0.9f);
                case TweenNodeType.r_旋转: return new Color(0.9f, 0.6f, 0.2f);
                case TweenNodeType.s_缩放: return new Color(0.3f, 0.8f, 0.4f);
                case TweenNodeType.c_颜色: return new Color(0.9f, 0.3f, 0.4f);
                case TweenNodeType.g_淡化: return new Color(0.6f, 0.4f, 0.9f);
                case TweenNodeType.w_打字机: return new Color(0.8f, 0.8f, 0.3f);
                case TweenNodeType.f_图像填充: return new Color(0.3f, 0.8f, 0.8f);
                case TweenNodeType.z_尺寸: return new Color(0.7f, 0.5f, 0.3f);
                default: return Color.gray;
            }
        }
        /// <summary>
        /// 根据类型枚举值来获取动画图标
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private Texture2D GetTweenTypeIcon(TweenNodeType type, bool big_mode = false)
        {
            Texture2D typeicon = null;
            switch (type)
            {
                case TweenNodeType.a_位移:
                    typeicon = big_mode ? b_anim_type_move : icon_type_move;
                    break;
                case TweenNodeType.r_旋转:
                    typeicon = big_mode ? b_anim_type_rotate : icon_type_rotate;
                    break;
                case TweenNodeType.s_缩放:
                    typeicon = big_mode ? b_anim_type_scale : icon_type_scale;
                    break;
                case TweenNodeType.c_颜色:
                    typeicon = big_mode ? b_anim_type_color : icon_type_color;
                    break;
                case TweenNodeType.g_淡化:
                    typeicon = big_mode ? b_anim_type_fade : icon_type_fade;
                    break;
                case TweenNodeType.w_打字机:
                    typeicon = big_mode ? b_anim_type_writter : icon_type_writter;
                    break;
                case TweenNodeType.f_图像填充:
                    typeicon = big_mode ? b_anim_type_fill : icon_type_fill;
                    break;
                case TweenNodeType.z_尺寸:
                    typeicon = big_mode ? b_anim_type_size : icon_type_size;
                    break;
            }

            return typeicon;
        }
        /// <summary> 
        /// 绘制单条音效轨及其 Clip，并进行 MouseDown 命中检测以进入拖拽模式。
        /// <para/>
        /// 与 <see cref="DrawTrackRow"/> 结构完全对称，只是：
        /// <list type="bullet">
        /// <item><description>数据源是 <see cref="Sounds"/>；</description></item>
        /// <item><description>Clip 宽度由音频长度决定；</description></item>
        /// <item><description>选中判定走 <see cref="IsSelected"/>；</description></item>
        /// <item><description>拖拽走 <see cref="TryBeginSoundClipDrag"/>。</description></item>
        /// </list>
        /// </summary>
        /// <param name="trackRect">轨道矩形（内容坐标，高 = trackHeight）</param>
        /// <param name="soundIndex">音效索引</param>
        private void DrawSoundTrackRow(Rect trackRect, int soundIndex)
        {
            if (soundIndex < 0 || soundIndex >= Sounds.Count) return;
            TweenSound sound = Sounds[soundIndex];
            bool isSelected = IsSelected(TrackKind.Sound, soundIndex);

            Rect trackVisualRect = CalculateTrackVisualRect(trackRect);

            // 行底色：音效轨用略深的底色，和动画轨区分
            XGUI.gui_box(trackVisualRect, ColorSoundTrackBG);

            Rect clipRect = CalculateSoundClipRect(trackVisualRect, sound);

            // 屏幕外裁剪
            float viewLeft = scrollPos.x;
            float viewRight = scrollPos.x + cachedClipAreaRect.width;
            if (clipRect.xMax < viewLeft - 10 || clipRect.x > viewRight + 10)
                return;

            DrawSoundClipVisual(clipRect, sound, isSelected);
            TryBeginSoundClipDrag(clipRect, soundIndex);
        }
        /// <summary> 
        /// 由轨道视觉矩形与音效数据计算 Clip 的内容坐标矩形。
        /// <para/>
        /// 规则：
        /// <list type="bullet">
        /// <item><description>左边缘 = <c>sound.Delay</c>；</description></item>
        /// <item><description>宽度 = 音频长度（<c>sound.Sound.length</c>），
        /// 若 <c>Sound</c> 为空则用 <see cref="DefaultSoundClipSeconds"/>；</description></item>
        /// </list>
        /// </summary>
        private Rect CalculateSoundClipRect(Rect trackVisualRect, TweenSound sound)
        {
            float clipX = trackVisualRect.x + sound.Delay * pixelsPerSecond;
            float lengthSec = sound.Sound != null ? sound.Sound.length : DefaultSoundClipSeconds;
            float clipW = lengthSec * pixelsPerSecond;
            return new Rect(
                Mathf.Round(clipX),
                trackVisualRect.y,
                Mathf.Max(Mathf.Round(clipW), 4f),
                trackVisualRect.height);
        }
        /// <summary> 
        /// 绘制音效 Clip 的视觉样式：背景色块、音效名、延迟文本。
        /// <para/>
        /// 与 <see cref="DrawClipVisual"/> 对应，但只表达音效相关的信息。
        /// </summary>
        private void DrawSoundClipVisual(Rect clipRect, TweenSound sound, bool isSelected)
        {
            #region 背景
            Color bg = sound.Mute ? ColorSoundTrackBG_muted : (sound.Sound != null ? ColorSoundClipBG : ColorSoundClipBG_Empty);

            if (isSelected)
            {
                bg = sound.Mute ? ColorSoundTrackBG_muted_Selected : (sound.Sound != null ? ColorSoundClipBG_Selected : ColorSoundClipBG_Empty_Selected);
            }

            XGUI.gui_box(clipRect, bg);
            #endregion

            #region 概述信息
            bool minimal = clipRect.width < 120;
            string state = sound.Mute ? "已静音" : sound.Sound != null ? $"时长：{sound.Sound.length:F2}  s" : "空音效";
            string content = minimal ? $"延迟：{sound.Delay:F3}  s" : $"{state}   /   延迟：{sound.Delay:F3}  s";
            XGUI.gui_label(
                rect: new Rect(clipRect.x + 4, clipRect.y, clipRect.width - 8, clipRect.height),
                text: new GUIContent(content),
                text_color: sound.Mute ? Color.white * 0.7f : Color.white,
                size: XGUIFontSize.M,
                clipping: TextClipping.Clip,
                anchor: TextAnchor.MiddleCenter,
                font_style: FontStyle.Normal);
            #endregion

            // 音效只做整体移动，光标固定为 MoveArrow
            EditorGUIUtility.AddCursorRect(clipRect, MouseCursor.MoveArrow);
        }
    }
}
