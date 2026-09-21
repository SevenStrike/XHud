namespace SevenStrikeModules.XHud.Editor
{
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XTween;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    ///
    /// XHud 迷你时间轴窗口。
    /// <para/>
    /// 用于以时间轴方式可视化编辑 <see cref="XHud_Module_Primitive_Tween"/> 的动画节点，支持：
    /// <list type="bullet">
    /// <item><description>中键 / Alt+左键拖拽平移视图；</description></item>
    /// <item><description>Alt+滚轮以鼠标为锚点缩放，Shift+滚轮调整轨道高度，Ctrl+滚轮垂直滚动；</description></item>
    /// <item><description>拖拽 Clip 修改 Delay / Duration，并支持边界吸附与边缘自动平移；</description></item>
    /// <item><description>快捷键：F 缩放到合适范围 / R 重置视图 / Esc 关闭窗口。</description></item>
    /// </list>
    /// 所有改动通过 <see cref="Undo.RecordObject"/> + <see cref="EditorUtility.SetDirty"/> 实时生效。
    ///</summary>
    public class MiniTimelineWindow : EditorWindow
    {
        #region 常量 - 布局        
        /// <summary>
        ///顶部工具栏高度（像素）。名字列与 Clip 区的局部 Y 坐标均以此作为起点。
        ///</summary>
        private const float TopBarHeight = 21f;
        /// <summary>
        ///轨道最小高度（像素），用于 Shift+滚轮调整轨道高度时的下限。
        ///</summary>
        private const float MinTrackHeight = 16f;
        /// <summary>
        ///轨道最大高度（像素），用于 Shift+滚轮调整轨道高度时的上限。
        ///</summary>
        private const float MaxTrackHeight = 80f;
        /// <summary>
        /// 名字行 / 轨道的垂直内边距占轨道高度的比例（0~0.5）。两侧同步使用。
        /// </summary>
        private const int RowVerticalPadding = 2;
        /// <summary>
        /// 名字列未选中行的背景色。
        /// </summary>
        private static readonly Color ColorNameRowBg = Color.white * 0.5f;
        /// <summary>
        /// 名字列选中行的背景色。
        /// </summary>
        private static readonly Color ColorNameRowSelectedBg = new Color(0.3f, 0.45f, 0.65f);
        /// <summary>
        /// 拖动整段 Clip 时，两端边界的垂直线颜色（区别于吸附黄线）。
        /// </summary>
        private static readonly Color ColorClipEdgeGuide = new Color(1f, 1f, 1f, 0.3f);
        /// <summary>
        /// 拖动整段 Clip 时，两端边界的垂直线颜色（区别于吸附黄线）。
        /// </summary>
        private static readonly Color ColorClipBG = new Color(1f, 1f, 1f, 0.3f);
        /// <summary>
        /// 拖动整段 Clip 时，吸附边界的垂直线颜色
        /// </summary>
        private static readonly Color ColorClipSnapGuide = XHud_Dashboard.Theme_Primary;
        #endregion

        #region 常量 - 交互        
        /// <summary>
        ///吸附判定阈值（像素）。换算成秒后用于时间吸附，保证任意缩放下手感一致。
        ///</summary>
        private const float SnapThresholdPixels = 13f;
        /// <summary>
        ///Clip 左右边缘用于判定「拉伸」的命中宽度（像素）。
        ///</summary>
        private const float ClipEdgeZone = 6f;
        /// <summary>
        ///Shift+滚轮调整轨道高度时，每格滚轮对应的比例变化量。
        ///</summary>
        private const float TrackHeightZoomStep = 0.02f;
        #endregion

        #region 常量 - 拖拽自动平移        
        /// <summary>
        ///拖拽时触发自动平移的边缘宽度（像素）。鼠标进入该区域后视图自动滚动。
        ///</summary>
        private const float AutoScrollEdgeMargin = 24f;
        /// <summary>
        ///拖拽自动平移的基础速度（像素/帧），实际速度会随鼠标接近边缘的程度放大。
        ///</summary>
        private const float AutoScrollSpeed = 8f;
        #endregion

        #region 字段 - 视图参数        
        /// <summary>
        ///每秒对应的像素数（缩放比例）。范围 1～20000，由滚轮缩放或 FitToContent 修改。
        ///</summary>
        private float pixelsPerSecond = 100f;
        /// <summary>
        ///左侧名字列宽度（像素）。当前为固定值，未提供拖拽调整。
        ///</summary>
        private float nameColumnWidth = 220f;
        /// <summary>
        ///右侧参数面板宽度（像素）。当前为固定值，未提供拖拽调整。
        ///</summary>
        private float paramPanelWidth = 280f;
        /// <summary>
        ///单条轨道的高度（像素）。由 Shift+滚轮调整，范围 [MinTrackHeight, MaxTrackHeight]。
        ///</summary>
        private float trackHeight = 26f;
        /// <summary>
        ///顶部刻度尺高度（像素）。名字列 / Clip 区 / 参数面板的标题栏高度共用此值。
        ///</summary>
        private float rulerHeight = 22f;
        #endregion

        #region 字段 - 滚动        
        /// <summary>
        ///名字列的滚动位置。仅使用其 y 分量，与 <see cref="scrollY"/> 保持同步。
        ///</summary>
        private Vector2 nameScroll;
        /// <summary>
        ///参数面板的滚动位置。独立于名字列 / Clip 区。
        ///</summary>
        private Vector2 paramScroll;
        /// <summary>
        ///Clip 区水平滚动量（像素），表示可视区左边缘对应的内容偏移。恒 ≥ 0。
        ///</summary>
        private float scrollX = 0f;
        /// <summary>
        ///Clip 区垂直滚动量（像素），与名字列保持同步。恒 ≥ 0。
        ///</summary>
        private float scrollY = 0f;
        #endregion

        #region 字段 - 目标数据与选中        
        /// <summary>
        ///当前窗口编辑的图元动画器。为 null 时窗口只显示提示信息。
        ///</summary>
        private XHud_Module_Primitive_Tween target;
        /// <summary>
        ///当前动画器的动画节点列表（<see cref="target"/> 的快捷访问）。
        ///</summary>
        private List<TweenNode> Nodes => target.PrimitiveTweenNodes;
        /// <summary>
        ///当前选中的动画节点索引。-1 表示未选中，参数面板会显示提示文本。
        ///</summary>
        private int selectedIndex = -1;
        #endregion

        #region 字段 - 拖拽状态        
        /// <summary>
        ///Clip 拖拽模式。值为 <see cref="DragMode.None"/> 表示当前无拖拽。
        ///</summary>
        private DragMode dragMode = DragMode.None;
        /// <summary>
        ///当前正在拖拽的动画节点索引。-1 表示无。
        ///</summary>
        private int draggingIndex = -1;
        /// <summary>
        ///Clip 拖拽时占用的 ControlID，用于在拖拽期间锁定 <see cref="GUIUtility.hotControl"/>。
        ///</summary>
        private int dragControlID;
        /// <summary>
        ///拖拽开始时鼠标对应的内容时间（秒），已包含 <see cref="scrollX"/>。用于计算位移。
        ///</summary>
        private float dragStartContentSecond;
        /// <summary>
        ///拖拽开始时节点的 Delay（秒）。用于计算相对位移。
        ///</summary>
        private float dragStartDelay;
        /// <summary>
        ///拖拽开始时节点的 Duration（秒）。用于计算相对位移。
        ///</summary>
        private float dragStartDuration;
        /// <summary>
        ///本帧吸附到的参考时间（秒）。&lt;0 表示本帧无吸附。非负时绘制黄色参考线。
        ///</summary>
        private float snapGuideSecond = -1f;
        /// <summary>
        /// 当前 Move 拖拽锁定的吸附源：0 = 无，1 = 左边缘，2 = 右边缘。
        /// 锁定后，只有另一侧明显更近时才切换，避免边缘抖动。
        /// </summary>
        private int moveSnapSource = 0;
        #endregion

        #region 字段 - 平移状态        
        /// <summary>
        ///是否正在使用中键 / Alt+左键平移视图。
        ///</summary>
        private bool isPanning = false;
        /// <summary>
        ///平移时占用的 ControlID，用于在拖拽期间锁定 <see cref="GUIUtility.hotControl"/>。
        ///</summary>
        private int panControlID;
        #endregion

        #region 字段 - 布局缓存
        /// <summary>
        ///Clip 区在窗口坐标系中的矩形。由 <see cref="OnGUI"/> 每帧写入，供滚轮 / 命中判断使用。
        ///</summary>
        private Rect cachedClipAreaRect;
        /// <summary>
        ///名字列在窗口坐标系中的矩形。由 <see cref="OnGUI"/> 每帧写入，供滚轮使用。
        ///</summary>
        private Rect cachedNameAreaRect;
        /// <summary>
        ///本帧左键按下是否命中了 Clip。用于在 Clip 区空白处点击时取消选中。
        ///</summary>
        private bool clipHitThisFrame;
        #endregion

        #region 嵌套类型        
        /// <summary>
        ///
        /// Clip 拖拽模式，描述当前鼠标拖拽的是 Clip 的哪个部分。
        /// 
        ///</summary>
        private enum DragMode
        {
            /// <summary>
            ///未拖拽。
            ///</summary>
            None,
            /// <summary>
            ///整体移动（修改 Delay，Duration 不变）。
            ///</summary>
            Move,
            /// <summary>
            ///拖拽左边缘（同时修改 Delay 与 Duration，右端固定）。
            ///</summary>
            LeftEdge,
            /// <summary>
            ///拖拽右边缘（仅修改 Duration，左端固定）。
            ///</summary>
            RightEdge
        }
        #endregion

        #region 窗口打开        
        /// <summary>
        ///
        /// 通过菜单打开一个空的迷你时间轴窗口（未绑定任何 target）。
        /// 
        ///</summary>
        [MenuItem("Tools/TimelineTool")]
        public static void Open()
        {
            var win = GetWindow<MiniTimelineWindow>("TimelineTool");
            win.minSize = new Vector2(700, 400);
        }
        /// <summary>
        ///
        /// 打开（或复用）迷你时间轴窗口，并绑定指定的图元动画器。
        /// 若目标与当前不同，会先保存旧 target 的视图状态、再加载新 target 的视图状态。
        /// 
        ///</summary>
        /// <param name="tween">要编辑的图元动画器。</param>
        public static void OpenWith(XHud_Module_Primitive_Tween tween)
        {
            var win = GetWindow<MiniTimelineWindow>(false, "TimelineTool");
            if (win.target != tween)
            {
                win.SaveViewState();   // 保存旧 target 的状态
                win.target = tween;
                win.selectedIndex = -1;
                win.LoadViewState();   // 从新 target 读状态
            }
            win.Repaint();
            win.Focus();
        }
        #endregion

        #region 生命周期        
        /// <summary>
        ///
        /// 窗口禁用时保存当前视图状态到 target。
        /// 
        ///</summary>
        private void OnDisable()
        {
            SaveViewState();
        }
        /// <summary>
        ///
        /// 绘制窗口内容。每帧调用。
        /// <para/>
        /// 布局顺序：工具栏 → 名字列 → Clip 区 → 参数面板 → 滚轮 → 快捷键。
        /// 注意：滚轮与快捷键必须在所有区域绘制之后处理，以便正确消费事件。
        /// 
        ///</summary>
        private void OnGUI()
        {
            DrawToolbar();

            if (target == null)
            {
                EditorGUILayout.HelpBox("请通过动画器面板的按钮打开此窗口。", MessageType.Info);
                return;
            }

            // 参数面板占右侧固定宽度，剩下的给时间轴
            float timelineTotalWidth = position.width - paramPanelWidth;

            float viewHeight = position.height - TopBarHeight;
            Rect nameArea = new Rect(0, TopBarHeight, nameColumnWidth, viewHeight);
            Rect clipArea = new Rect(nameColumnWidth, TopBarHeight, timelineTotalWidth - nameColumnWidth, viewHeight);
            Rect paramArea = new Rect(timelineTotalWidth, TopBarHeight, paramPanelWidth, viewHeight);

            cachedNameAreaRect = nameArea;
            cachedClipAreaRect = clipArea;

            DrawNameColumn(nameArea);
            DrawClipArea(clipArea);
            DrawParameterPanel(paramArea);

            HandleScrollWheel();
            HandleShortcuts();
        }
        #endregion

        #region 绘制 - 工具栏        
        /// <summary>
        ///
        /// 绘制顶部工具栏：重置视图、缩放到合适范围，以及右侧提示文本。
        /// 
        ///</summary>
        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (GUILayout.Button("重置视图", EditorStyles.toolbarButton, GUILayout.Width(80)))
            {
                ResetView();
            }

            if (GUILayout.Button("缩放到合适范围", EditorStyles.toolbarButton, GUILayout.Width(150)))
            {
                FitToContent();
            }

            GUILayout.FlexibleSpace();
            GUILayout.Label("改动实时生效", EditorStyles.miniLabel);

            EditorGUILayout.EndHorizontal();
        }
        #endregion

        #region 绘制 - 名字列        
        /// <summary>
        ///
        /// 绘制左侧名字列（固定面板，独立垂直滚动）。
        /// <para/>
        /// 每条轨道显示动画节点的名称与启用状态；点击可选中该节点，点击空白处取消选中。
        /// 滚动位置写入 <see cref="nameScroll"/>，并同步到 <see cref="scrollY"/>。
        /// 
        ///</summary>
        /// <param name="area">名字列在窗口坐标系中的矩形区域。</param>
        private void DrawNameColumn(Rect area)
        {
            GUI.BeginGroup(area);

            // 标题栏区域（固定，不参与滚动）
            Rect rulerNameRect = new Rect(0, 0, area.width, rulerHeight);

            // 滚动视口区域（从标题栏下方开始）
            Rect scrollViewportRect = new Rect(0, rulerHeight, area.width, area.height - rulerHeight);

            nameScroll = GUI.BeginScrollView(
                scrollViewportRect,
                nameScroll,
                new Rect(0, 0, scrollViewportRect.width, Nodes.Count * trackHeight + 20f),
                false,
                true,
                GUIStyle.none,
                GUI.skin.verticalScrollbar);

            bool nameHitThisFrame = false;

            for (int i = 0; i < Nodes.Count; i++)
            {
                TweenNode node = Nodes[i];
                bool isSelected = (i == selectedIndex);

                // 行命中区 + 视觉区
                Rect rowRect = GetNameRowHitRect(i, scrollViewportRect.width);
                Rect nameRect = GetNameRowVisualRect(rowRect);

                // 绘制行样式
                DrawNameRowVisual(nameRect, node, isSelected);

                // 命中检测：点击选中
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0
                    && rowRect.Contains(Event.current.mousePosition))
                {
                    selectedIndex = i;
                    GUIUtility.keyboardControl = 0;
                    nameHitThisFrame = true;
                    Event.current.Use();
                    Repaint();
                }
            }

            GUI.EndScrollView();

            // 空白点击取消选中（仅当未命中轨道、且点击落在滚动视口内）
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0
                && !Event.current.alt && !nameHitThisFrame
                && scrollViewportRect.Contains(Event.current.mousePosition))
            {
                selectedIndex = -1;
                GUIUtility.keyboardControl = 0;
                Event.current.Use();
                Repaint();
            }

            scrollY = nameScroll.y;

            // 标题栏最后画，盖在最上层
            DrawColumnHeader(rulerNameRect, "轨道");

            GUI.EndGroup();
        }
        /// <summary>
        ///
        /// 绘制单条名字行的视觉样式：背景色块 + 名称与启用状态文本。
        /// <para/>
        /// 不参与命中检测与布局计算，仅负责"把名字行画出来"。
        /// 
        ///</summary>
        /// <param name="nameRect">行视觉矩形（已按内边距内缩，Clip 区局部坐标）。</param>
        /// <param name="node">对应的动画节点。</param>
        /// <param name="isSelected">该节点当前是否被选中。</param>
        private void DrawNameRowVisual(Rect nameRect, TweenNode node, bool isSelected)
        {
            EditorGUI.DrawRect(nameRect, isSelected ? ColorNameRowSelectedBg : ColorNameRowBg);

            GUI.Label(
                new Rect(nameRect.x + 6, nameRect.y, nameRect.width - 12, nameRect.height),
                $"{node.Indicator}  [{(node.Enabled ? "开" : "关")}]");
        }
        /// <summary>
        ///
        /// 计算第 <paramref name="index"/> 条名字行的命中矩形（完整行高，用于点击检测与滚动范围）。
        /// 
        ///</summary>
        /// <param name="index">节点索引。</param>
        /// <param name="width">名字列可用宽度。</param>
        /// <returns>行命中矩形（名字列局部坐标）。</returns>
        private Rect GetNameRowHitRect(int index, float width)
        {
            //return new Rect(0, index * trackHeight, width, trackHeight);
            //  用同一套取整规则，保证名字列和 Clip 区行位置对齐
            float y0 = Mathf.Round(index * trackHeight);
            float y1 = Mathf.Round((index + 1) * trackHeight);
            return new Rect(0, y0, width, y1 - y0);
        }

        /// <summary>
        ///
        /// 由行命中矩形计算名字行的视觉矩形：上下按 <see cref="RowVerticalPadding"/> 内缩，
        /// 与 Clip 区的视觉行保持对齐。
        /// 
        ///</summary>
        /// <param name="rowRect">行命中矩形。</param>
        /// <returns>行视觉矩形。</returns>
        private Rect GetNameRowVisualRect(Rect rowRect)
        {
            float pad = RowVerticalPadding;                 // 固定 10px
            return new Rect(
                rowRect.x,
                rowRect.y + pad,
                rowRect.width,
                Mathf.Max(rowRect.height - pad * 2f, 4f));  // rowRect.height 已是整数，减 20 也是整数
        }
        /// <summary>
        ///
        /// 绘制固定列标题栏：深色背景 + 左上角 mini 标签。
        /// <para/>
        /// 名字列与参数面板共用。应在内容绘制之后调用，以盖在最上层。
        /// 
        ///</summary>
        /// <param name="headerRect">标题栏矩形（当前 Group 局部坐标）。</param>
        /// <param name="title">标题文本。</param>
        private void DrawColumnHeader(Rect headerRect, string title)
        {
            EditorGUI.DrawRect(headerRect, new Color(0.18f, 0.18f, 0.18f));
            GUI.Label(
                new Rect(headerRect.x + 6, headerRect.y, headerRect.width, headerRect.height),
                title, EditorStyles.miniLabel);
        }
        #endregion

        #region 绘制 - Clip 区        
        /// <summary>
        ///
        /// 绘制 Clip 区：刻度尺 + 轨道 + Clip，并处理拖拽、平移与空白点击。
        /// <para/>
        /// 只绘制当前可见时间区间内的内容，时间轴长度不受限制，绘制开销与可见区大小相关。
        /// 
        ///</summary>
        /// <param name="area">Clip 区在窗口坐标系中的矩形区域。</param>
        private void DrawClipArea(Rect area)
        {
            GUI.BeginGroup(area);
            Rect localArea = new Rect(0, 0, area.width, area.height);

            // 背景
            EditorGUI.DrawRect(localArea, new Color(0.18f, 0.18f, 0.18f));

            // 可见时间区间
            float startSecond = scrollX / pixelsPerSecond;
            float endSecond = (scrollX + area.width) / pixelsPerSecond;

            // 刻度尺（固定，独立）
            Rect rulerRect = new Rect(0, 0, area.width, rulerHeight);
            DrawRuler(rulerRect, startSecond, endSecond);

            clipHitThisFrame = false;

            for (int i = 0; i < Nodes.Count; i++)
            {
                //  行顶部取整到整数像素
                float trackTop = Mathf.Round(rulerHeight + i * trackHeight - scrollY);
                //  行底部也取整，保证高度是整数
                float trackBottom = Mathf.Round(rulerHeight + (i + 1) * trackHeight - scrollY);
                float trackH = trackBottom - trackTop;

                if (trackBottom < rulerHeight || trackTop > area.height) continue;

                Rect trackRect = new Rect(0, trackTop, area.width, trackH);
                DrawTrack(trackRect, i, startSecond, endSecond);
            }

            // 吸附参考线
            if (dragMode != DragMode.None && snapGuideSecond >= 0f)
            {
                float gx = snapGuideSecond * pixelsPerSecond - scrollX;
                if (gx >= 0 && gx <= area.width)
                {
                    EditorGUI.DrawRect(new Rect(gx, rulerHeight, 1f, area.height - rulerHeight), ColorClipSnapGuide);
                }
            }

            // 拖动整段 Clip 时，显示左右两端边界垂直线
            if (dragMode == DragMode.Move && draggingIndex >= 0 && draggingIndex < Nodes.Count)
            {
                TweenNode draggingNode = Nodes[draggingIndex];

                float leftX = draggingNode.Delay * pixelsPerSecond - scrollX;
                float rightX = (draggingNode.Delay + draggingNode.Duration) * pixelsPerSecond - scrollX;

                float lineTop = rulerHeight;
                float lineHeight = area.height - rulerHeight;

                if (leftX >= 0 && leftX <= area.width)
                    EditorGUI.DrawRect(new Rect(leftX, lineTop, 1f, lineHeight), ColorClipEdgeGuide);

                if (rightX >= 0 && rightX <= area.width)
                    EditorGUI.DrawRect(new Rect(rightX, lineTop, 1f, lineHeight), ColorClipEdgeGuide);
            }

            // 统一处理拖拽（不依赖 Clip 是否可见），必须在所有轨道绘制之后
            ProcessClipDrag(localArea);


            HandleEmptyClick(localArea);
            HandlePan(localArea);

            GUI.EndGroup();
        }
        #endregion

        #region 绘制 - 刻度尺        
        /// <summary>
        ///
        /// 绘制顶部刻度尺。只在 [startSecond, endSecond] 区间内生成刻度，
        /// 保证无论滚动到多远，绘制开销恒定。
        /// <para/>
        /// 刻度密度随 <see cref="pixelsPerSecond"/> 自动分档；只有整秒主刻度带标签。
        /// 
        ///</summary>
        /// <param name="rect">刻度尺矩形（Clip 区局部坐标）。</param>
        /// <param name="startSecond">可见区左边缘对应的时间（秒）。</param>
        /// <param name="endSecond">可见区右边缘对应的时间（秒）。</param>
        private void DrawRuler(Rect rect, float startSecond, float endSecond)
        {
            EditorGUI.DrawRect(rect, new Color(0.15f, 0.15f, 0.15f));

            // 1. 每秒的细分数
            int subdiv;
            if (pixelsPerSecond >= 8000f) subdiv = 80;
            else if (pixelsPerSecond >= 4000f) subdiv = 80;
            else if (pixelsPerSecond >= 2000f) subdiv = 60;
            else if (pixelsPerSecond >= 1000f) subdiv = 40;
            else if (pixelsPerSecond >= 500f) subdiv = 40;
            else if (pixelsPerSecond >= 250f) subdiv = 20;
            else if (pixelsPerSecond >= 100f) subdiv = 20;
            else subdiv = 5;

            float minorStep = 1f / subdiv;   // 次刻度间隔（秒）

            // 2. 可见区间的次刻度索引范围
            int i0 = Mathf.FloorToInt(startSecond / minorStep);
            int i1 = Mathf.CeilToInt(endSecond / minorStep);

            for (int i = i0; i <= i1; i++)
            {
                float t = i * minorStep;
                float x = t * pixelsPerSecond - scrollX;
                if (x < rect.x - 1 || x > rect.xMax + 1) continue;

                // 主刻度：整秒
                bool isMajor = Mathf.Abs(t - Mathf.Round(t)) < minorStep * 0.01f;

                float lineTop = isMajor ? 5f : 17f;   // 主刻度线更长
                Color lineColor = isMajor ? Color.gray : new Color(0.4f, 0.4f, 0.4f);
                float linewidth = isMajor ? 2f : 1f;  // 主刻度线更粗

                EditorGUI.DrawRect(
                    new Rect(x, rect.y + lineTop, linewidth, rect.height - lineTop), lineColor);

                // 只有主刻度带标签
                if (isMajor)
                    GUI.Label(new Rect(x + 2, rect.y, 70, rect.height), $"{t:F0}s");
            }
        }
        #endregion

        #region 绘制 - 轨道    
        /// <summary>
        ///
        /// 绘制单条轨道及其 Clip，并进行 MouseDown 命中检测以进入拖拽模式。
        /// <para/>
        /// 轨道背景与 Clip 都会做垂直裁剪，避免越过刻度尺上边缘或超出 Clip 区。
        /// 
        ///</summary>
        /// <param name="trackRect">轨道矩形（Clip 区局部坐标）。</param>
        /// <param name="index">动画节点索引。</param>
        /// <param name="startSecond">可见区左边缘对应的时间（秒）。当前未使用，保留供扩展。</param>
        /// <param name="endSecond">可见区右边缘对应的时间（秒）。当前未使用，保留供扩展。</param>
        private void DrawTrack(Rect trackRect, int index, float startSecond, float endSecond)
        {
            TweenNode node = Nodes[index];
            bool isSelected = (index == selectedIndex);

            //  轨道视觉矩形：和名字列一样的固定像素内缩
            Rect trackVisualRect = GetTrackVisualRect(trackRect);

            // 垂直裁剪（防止越过刻度尺 / 超出 Clip 区底）
            Rect drawTrackRect = ClipToTimelineVertical(trackVisualRect);
            if (drawTrackRect.height <= 0) return;

            EditorGUI.DrawRect(drawTrackRect, ColorClipBG);

            // Clip 矩形现在可以直接基于 trackVisualRect 计算，这样 Clip 和背景严格对齐
            Rect clipRect = GetClipRect(trackVisualRect, node);

            //// 计算 Clip 矩形
            //Rect clipRect = GetClipRect(trackRect, node);

            // 水平裁剪：完全在可见区外就不画
            if (clipRect.xMax < trackRect.x - 10 || clipRect.x > trackRect.xMax + 10)
                return;

            // 垂直裁剪：Clip 也不允许越过刻度尺上边缘
            Rect drawClipRect = ClipToTimelineVertical(clipRect);
            if (drawClipRect.height <= 0) return;   // 完全被遮住

            // 绘制 Clip 自身样式
            DrawClipVisual(drawClipRect, node, isSelected);

            // 只做 MouseDown 命中检测；拖拽更新统一由 ProcessClipDrag 处理
            TryBeginClipDrag(drawClipRect, index);
        }
        /// <summary>
        /// 由轨道命中矩形计算轨道视觉矩形：上下按固定像素 <see cref="RowVerticalPadding"/> 内缩，
        /// 与名字列的 GetNameRowVisualRect 使用完全相同的规则，保证两边行间距一致。
        /// </summary>
        /// <param name="trackRect">轨道命中矩形（高 = trackHeight）。</param>
        /// <returns>轨道视觉矩形（已内缩）。</returns>
        private Rect GetTrackVisualRect(Rect trackRect)
        {
            float pad = RowVerticalPadding;   // ← 同一个固定像素值
            return new Rect(
                trackRect.x,
                trackRect.y + pad,
                trackRect.width,
                Mathf.Max(trackRect.height - pad * 2f, 4f));
        }
        /// <summary>
        ///
        /// 绘制 Clip 自身的视觉样式：背景色块、文本标签、鼠标光标。
        /// <para/>
        /// 不参与布局计算与命中检测，仅负责"把 Clip 画出来"。
        /// 
        ///</summary>
        /// <param name="clipRect">已裁剪到可见区的 Clip 矩形（Clip 区局部坐标）。</param>
        /// <param name="node">对应的动画节点。</param>
        /// <param name="isSelected">该节点当前是否被选中。</param>
        private void DrawClipVisual(Rect clipRect, TweenNode node, bool isSelected)
        {
            // 类型色 → 禁用变暗 → 选中提亮
            Color clipColor = GetClipColor(node.Type);
            if (!node.Enabled) clipColor *= 0.5f;
            if (isSelected) clipColor = Color.Lerp(clipColor, Color.white, 0.25f);

            // 背景
            EditorGUI.DrawRect(clipRect, clipColor);

            // 时间标签
            GUI.Label(
                new Rect(clipRect.x + 4, clipRect.y, clipRect.width - 8, clipRect.height),
                $"{node.Delay:F2} / {node.Duration:F2}");

            // 光标
            DrawClipCursors(clipRect);
        }
        /// <summary>
        ///
        /// 根据轨道矩形与节点数据计算 Clip 的屏幕矩形。
        /// <para/>
        /// 水平位置由 Delay / Duration 与 scrollX 决定，垂直位置按 RowVerticalPaddingRatio 内缩。
        /// 
        ///</summary>
        /// <param name="trackRect">轨道矩形（Clip 区局部坐标）。</param>
        /// <param name="node">动画节点。</param>
        /// <returns>Clip 矩形（未裁剪）。</returns>
        private Rect GetClipRect(Rect trackVisualRect, TweenNode node)
        {
            float clipX = trackVisualRect.x + node.Delay * pixelsPerSecond - scrollX;
            float clipW = node.Duration * pixelsPerSecond;

            return new Rect(
                Mathf.Round(clipX),                        //  水平也取整
                trackVisualRect.y,                         // 已经是整数
                Mathf.Max(Mathf.Round(clipW), 4f),         //  宽度取整
                trackVisualRect.height);                   // 已经是整数
        }
        /// <summary>
        ///
        /// 将矩形垂直裁剪到时间轴可见区：上边界不低于刻度尺，下边界不超过 Clip 区底。
        /// 
        ///</summary>
        /// <param name="rect">待裁剪的矩形。</param>
        /// <returns>裁剪后的矩形；高度可能为 0 或负，调用方需自行判断。</returns>
        private Rect ClipToTimelineVertical(Rect rect)
        {
            Rect result = rect;
            float areaBottom = cachedClipAreaRect.height;

            if (result.y < rulerHeight)
            {
                float diff = rulerHeight - result.y;
                result.y = rulerHeight;
                result.height -= diff;
            }

            if (result.yMax > areaBottom)
            {
                result.height = areaBottom - result.y;
            }

            return result;
        }
        /// <summary>
        ///
        /// 为 Clip 设置鼠标光标形状：主体为移动光标，左右边缘为水平缩放光标。
        /// 
        ///</summary>
        /// <param name="clipRect">Clip 矩形（Clip 区局部坐标）。</param>
        private void DrawClipCursors(Rect clipRect)
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
        ///
        /// 根据动画类型获取 Clip 的显示颜色。
        /// 
        ///</summary>
        /// <param name="type">动画节点类型。</param>
        /// <returns>该类型对应的颜色。</returns>
        private Color GetClipColor(TweenNodeType type)
        {
            switch (type)
            {
                case TweenNodeType.位移: return new Color(0.2f, 0.5f, 0.9f);
                case TweenNodeType.旋转: return new Color(0.9f, 0.6f, 0.2f);
                case TweenNodeType.缩放: return new Color(0.3f, 0.8f, 0.4f);
                case TweenNodeType.颜色: return new Color(0.9f, 0.3f, 0.4f);
                case TweenNodeType.淡化: return new Color(0.6f, 0.4f, 0.9f);
                case TweenNodeType.打字机: return new Color(0.8f, 0.8f, 0.3f);
                case TweenNodeType.图像填充: return new Color(0.3f, 0.8f, 0.8f);
                case TweenNodeType.尺寸: return new Color(0.7f, 0.5f, 0.3f);
                default: return Color.gray;
            }
        }
        #endregion

        #region 交互 - Clip 拖拽        
        /// <summary>
        ///
        /// 在 Clip 矩形上检测 MouseDown 命中，命中则进入对应的拖拽模式。
        /// <para/>
        /// 真正的拖拽更新由 <see cref="ProcessClipDrag"/> 在 Clip 区末尾统一处理，
        /// 这样即使 Clip 滚出可视区，拖拽仍能继续。
        /// 
        ///</summary>
        /// <param name="clipRect">Clip 矩形（Clip 区局部坐标）。</param>
        /// <param name="index">动画节点索引。</param>
        private void TryBeginClipDrag(Rect clipRect, int index)
        {
            Event e = Event.current;
            if (e.type != EventType.MouseDown) return;
            if (e.button != 0) return;
            if (e.alt) return;                      // Alt + 左键是平移
            if (dragMode != DragMode.None) return;  // 已有拖拽进行中
            if (!clipRect.Contains(e.mousePosition)) return;

            TweenNode node = Nodes[index];

            if (Mathf.Abs(e.mousePosition.x - clipRect.xMin) < ClipEdgeZone)
                dragMode = DragMode.LeftEdge;
            else if (Mathf.Abs(e.mousePosition.x - clipRect.xMax) < ClipEdgeZone)
                dragMode = DragMode.RightEdge;
            else
                dragMode = DragMode.Move;

            draggingIndex = index;
            dragControlID = GUIUtility.GetControlID(FocusType.Passive);
            GUIUtility.hotControl = dragControlID;

            selectedIndex = index;
            GUIUtility.keyboardControl = 0;   // 点击 Clip 时取消参数面板焦点

            // 记录拖拽起点：用「内容时间」而非屏幕 X。
            // 内容时间 = (鼠标局部X + scrollX) / pixelsPerSecond，
            // 这样后续 scrollX 变化时，位移换算依然成立。
            dragStartContentSecond = (e.mousePosition.x + scrollX) / pixelsPerSecond;
            dragStartDelay = node.Delay;
            dragStartDuration = node.Duration;

            snapGuideSecond = -1f;

            Undo.RecordObject(target, "Edit Tween Clip");

            clipHitThisFrame = true;
            e.Use();
            Repaint();
        }
        /// <summary>
        ///
        /// 统一处理 Clip 拖拽的 MouseDrag / MouseUp。
        /// <para/>
        /// 不依赖 Clip 是否在可视区内；拖到边缘时会自动平移视图。
        /// 位移基于「内容时间」计算，因此自动滚动时 Clip 依旧跟手。
        /// 
        ///</summary>
        /// <param name="area">Clip 区局部矩形，用于判断边缘自动平移。</param>
        private void ProcessClipDrag(Rect area)
        {
            Event e = Event.current;

            if (dragMode == DragMode.None || draggingIndex < 0 || draggingIndex >= Nodes.Count)
            {
                // 没有拖拽时，如果残留了 hotControl，清掉
                if (e.type == EventType.MouseUp && GUIUtility.hotControl == dragControlID)
                    GUIUtility.hotControl = 0;
                return;
            }

            switch (e.type)
            {
                case EventType.MouseDrag:
                    if (GUIUtility.hotControl != dragControlID) return;
                    {
                        TweenNode node = Nodes[draggingIndex];

                        // 当前鼠标指向的内容时间（包含当前 scrollX）
                        float currentContentSecond = (e.mousePosition.x + scrollX) / pixelsPerSecond;

                        // 真实位移 = 当前内容时间 - 起始内容时间
                        float totalDelta = currentContentSecond - dragStartContentSecond;

                        snapGuideSecond = -1f;

                        switch (dragMode)
                        {
                            case DragMode.Move:
                                {
                                    float rawDelay = Mathf.Max(0f, dragStartDelay + totalDelta);
                                    float rawEnd = rawDelay + dragStartDuration;

                                    // 两侧各自算吸附候选
                                    float snappedStart = Mathf.Max(0f, SnapTime(rawDelay, draggingIndex));
                                    float snappedEnd = SnapTime(rawEnd, draggingIndex);
                                    float snappedDelayFromRight = Mathf.Max(0f, snappedEnd - dragStartDuration);

                                    float leftDelta = Mathf.Abs(snappedStart - rawDelay);
                                    float rightDelta = Mathf.Abs(snappedDelayFromRight - rawDelay);

                                    // 判断左右哪侧"真的吸上了"（距离 < 阈值）
                                    bool leftSnapped = leftDelta < SnapThresholdSeconds;
                                    bool rightSnapped = rightDelta < SnapThresholdSeconds;

                                    float snapped;
                                    int newSource;

                                    if (leftSnapped && rightSnapped)
                                    {
                                        // 两侧都吸上：优先用上一帧锁定的那侧，避免抖动
                                        if (moveSnapSource == 2) { snapped = snappedDelayFromRight; newSource = 2; }
                                        else { snapped = snappedStart; newSource = 1; }
                                    }
                                    else if (leftSnapped)
                                    {
                                        snapped = snappedStart;
                                        newSource = 1;
                                    }
                                    else if (rightSnapped)
                                    {
                                        snapped = snappedDelayFromRight;
                                        newSource = 2;
                                    }
                                    else
                                    {
                                        snapped = rawDelay;
                                        newSource = 0;
                                    }

                                    // 切换锁定时加死区：只有当另一侧比当前侧"明显更近"时才切换
                                    if (moveSnapSource != 0 && newSource != 0 && moveSnapSource != newSource)
                                    {
                                        float currentDist = moveSnapSource == 1 ? leftDelta : rightDelta;
                                        float otherDist = newSource == 1 ? leftDelta : rightDelta;
                                        const float switchHysteresis = 0.02f;   // 20ms 死区，或换算成像素
                                        if (otherDist > currentDist - switchHysteresis)
                                        {
                                            // 另一侧优势不够，保持原锁定
                                            snapped = moveSnapSource == 1 ? snappedStart : snappedDelayFromRight;
                                            newSource = moveSnapSource;
                                        }
                                    }

                                    moveSnapSource = newSource;

                                    // 黄线画在吸附的那端
                                    if (newSource == 1 && leftSnapped)
                                        snapGuideSecond = snappedStart;
                                    else if (newSource == 2 && rightSnapped)
                                        snapGuideSecond = snappedEnd;
                                    else
                                        snapGuideSecond = -1f;

                                    node.Delay = snapped;
                                    break;
                                }

                            case DragMode.LeftEdge:
                                {
                                    float endFixed = dragStartDelay + dragStartDuration;      // 右端固定
                                    float rawDelay = Mathf.Max(0f, dragStartDelay + totalDelta);
                                    float snapped = SnapTime(rawDelay, draggingIndex);
                                    snapped = Mathf.Clamp(snapped, 0f, endFixed - 0.01f);

                                    if (!Mathf.Approximately(snapped, rawDelay)) snapGuideSecond = snapped;

                                    node.Delay = snapped;
                                    node.Duration = Mathf.Max(0.01f, endFixed - snapped);
                                    break;
                                }

                            case DragMode.RightEdge:
                                {
                                    float minEnd = dragStartDelay + 0.01f;
                                    float rawEnd = Mathf.Max(minEnd, dragStartDelay + dragStartDuration + totalDelta);
                                    float snapped = Mathf.Max(minEnd, SnapTime(rawEnd, draggingIndex));

                                    if (!Mathf.Approximately(snapped, rawEnd)) snapGuideSecond = snapped;

                                    node.Delay = dragStartDelay;                              // 左端固定
                                    node.Duration = Mathf.Max(0.01f, snapped - dragStartDelay);
                                    break;
                                }
                        }

                        // 先算完位移，再自动平移；下一帧的 scrollX 更大，位移会继续增长
                        AutoScrollOnDragEdge(e.mousePosition.x, area);

                        EditorUtility.SetDirty(target);
                        Repaint();
                        e.Use();
                    }
                    break;

                case EventType.MouseUp:
                    // 只要还处于拖拽状态，就无条件收尾，避免鼠标在 Clip 区外松开导致 hotControl 卡住
                    if (dragMode != DragMode.None)
                    {
                        GUIUtility.hotControl = 0;
                        dragMode = DragMode.None;
                        draggingIndex = -1;
                        snapGuideSecond = -1f;
                        e.Use();
                        Repaint();
                    }
                    break;
            }
        }
        /// <summary>
        ///
        /// 拖拽时若鼠标靠近 Clip 区左右边缘，自动平移 <see cref="scrollX"/>，
        /// 让拖出视野的 Clip 重新可见。越靠近边缘滚动越快。
        /// 
        ///</summary>
        /// <param name="mouseLocalX">鼠标在 Clip 区局部坐标下的 X。</param>
        /// <param name="area">Clip 区局部矩形。</param>
        private void AutoScrollOnDragEdge(float mouseLocalX, Rect area)
        {
            if (mouseLocalX < AutoScrollEdgeMargin)
            {
                // 越靠左越快
                float t = 1f - Mathf.Clamp01(mouseLocalX / AutoScrollEdgeMargin);
                float speed = AutoScrollSpeed * (0.5f + t * 1.5f);
                scrollX = Mathf.Max(0f, scrollX - speed);
            }
            else if (mouseLocalX > area.width - AutoScrollEdgeMargin)
            {
                // 越靠右越快
                float over = mouseLocalX - (area.width - AutoScrollEdgeMargin);
                float t = Mathf.Clamp01(over / AutoScrollEdgeMargin);
                float speed = AutoScrollSpeed * (0.5f + t * 1.5f);
                scrollX += speed;
            }
        }
        #endregion

        #region 交互 - 平移        
        /// <summary>
        ///
        /// 处理中键 / Alt+鼠标左键拖拽平移视图（水平 + 垂直）。
        /// <para/>
        /// 只允许向右平移（<see cref="scrollX"/> ≥ 0），避免出现负时间区域；
        /// 垂直平移会同步 <see cref="nameScroll"/> 的 y 分量。
        /// 
        ///</summary>
        /// <param name="localArea">Clip 区局部矩形，用于判断按下的位置是否在区内。</param>
        private void HandlePan(Rect localArea)
        {
            Event e = Event.current;
            int controlID = GUIUtility.GetControlID(FocusType.Passive);

            switch (e.type)
            {
                case EventType.MouseDown:
                    // 中键，或 Alt + 左键
                    bool isPanTrigger = (e.button == 2) || (e.button == 0 && e.alt);
                    if (isPanTrigger && localArea.Contains(e.mousePosition))
                    {
                        isPanning = true;
                        panControlID = controlID;
                        GUIUtility.hotControl = controlID;
                        GUIUtility.keyboardControl = 0;
                        e.Use();
                    }
                    break;

                case EventType.MouseDrag:
                    if (isPanning && GUIUtility.hotControl == panControlID)
                    {
                        // 水平平移
                        scrollX -= e.delta.x;
                        scrollX = Mathf.Max(0f, scrollX);   // 不允许滚到负时间

                        // 垂直平移
                        float maxScrollY = GetMaxScrollY();
                        scrollY = Mathf.Clamp(scrollY - e.delta.y, 0f, maxScrollY);
                        nameScroll.y = scrollY;             // 同步名字列

                        e.Use();
                        Repaint();
                    }
                    break;

                case EventType.MouseUp:
                    // 中键，或左键（含 Alt+左键）释放
                    bool isPanRelease = (e.button == 2) || (e.button == 0);
                    if (isPanRelease && isPanning)
                    {
                        isPanning = false;
                        GUIUtility.hotControl = 0;
                        e.Use();
                    }
                    break;
            }
        }
        #endregion

        #region 交互 - 滚轮        
        /// <summary>
        ///
        /// 处理滚轮操作：
        /// <list type="bullet">
        /// <item><description>单独滚轮：水平平移（<see cref="scrollX"/>）。</description></item>
        /// <item><description>Alt + 滚轮：以鼠标为锚点缩放时间轴（<see cref="pixelsPerSecond"/>）。</description></item>
        /// <item><description>Shift + 滚轮：调整轨道高度（名字列与 Clip 轨道同步，有上下限）。</description></item>
        /// <item><description>Ctrl + 滚轮：垂直滚动（同步名字列与轨道）。</description></item>
        /// </list>
        /// 仅在鼠标位于 Clip 区或名字列内时生效。
        /// 
        ///</summary>
        private void HandleScrollWheel()
        {
            Event e = Event.current;
            if (e.type != EventType.ScrollWheel) return;
            if (target == null) return;
            if (!cachedClipAreaRect.Contains(e.mousePosition)
                && !cachedNameAreaRect.Contains(e.mousePosition)) return;

            // Alt + 滚轮 = 缩放
            if (e.alt)
            {
                float mouseLocalX = e.mousePosition.x - cachedClipAreaRect.x;
                float mouseSecond = (scrollX + mouseLocalX) / pixelsPerSecond;

                float zoomFactor = 1f - e.delta.y * 0.03f;
                pixelsPerSecond = Mathf.Clamp(pixelsPerSecond * zoomFactor, 1f, 20000f);

                scrollX = mouseSecond * pixelsPerSecond - mouseLocalX;
                scrollX = Mathf.Max(0f, scrollX);

                e.Use();
                Repaint();
                return;
            }

            // Shift + 滚轮 = 调整轨道高度（名字列与 Clip 轨道同步）
            if (e.shift)
            {
                // 兼容平台：Shift 可能把纵向 delta 转成横向 dx（本机实测 dy=0, dx=-3）
                float wheel = e.delta.y != 0f ? e.delta.y : e.delta.x;

                // delta 全为 0 时兜底，避免按了没反应
                if (Mathf.Approximately(wheel, 0f)) wheel = 1f;

                float factor = 1f - wheel * TrackHeightZoomStep;
                trackHeight = Mathf.Clamp(trackHeight * factor, MinTrackHeight, MaxTrackHeight);

                float maxScrollY = GetMaxScrollY();
                scrollY = Mathf.Clamp(scrollY, 0f, maxScrollY);
                nameScroll.y = scrollY;

                e.Use();
                Repaint();
                return;
            }

            // Ctrl + 滚轮 = 垂直滚动（同步名字列与轨道）
            if (e.control)
            {
                float maxScrollY = GetMaxScrollY();
                if (maxScrollY <= 0f)
                {
                    e.Use();
                    return;
                }

                scrollY = Mathf.Clamp(scrollY + e.delta.y * 20f, 0f, maxScrollY);
                nameScroll.y = scrollY;

                e.Use();
                Repaint();
                return;
            }

            // 单独滚轮 = 水平平移
            const float panSpeed = 5f;
            scrollX += e.delta.y * panSpeed;
            scrollX = Mathf.Max(0f, scrollX);

            e.Use();
            Repaint();
        }
        #endregion

        #region 交互 - 空点击        
        /// <summary>
        ///
        /// 在 Clip 区空白处按下左键时，取消当前选中并清除参数面板焦点。
        /// 若本帧命中了 Clip（<see cref="clipHitThisFrame"/> 为 true）则不处理，交给 Clip。
        /// 
        ///</summary>
        /// <param name="localArea">Clip 区局部矩形。</param>
        private void HandleEmptyClick(Rect localArea)
        {
            Event e = Event.current;
            if (e.type != EventType.MouseDown) return;
            if (e.button != 0) return;
            if (e.alt) return;                  // Alt + 左键是平移
            if (!localArea.Contains(e.mousePosition)) return;
            if (clipHitThisFrame) return;       // 命中了 clip，交给 clip 处理

            selectedIndex = -1;
            GUIUtility.keyboardControl = 0;     // 取消参数面板焦点

            e.Use();
            Repaint();
        }
        #endregion

        #region 交互 - 快捷键
        /// <summary>
        ///
        /// 处理窗口快捷键：
        /// <list type="bullet">
        /// <item><description>F：缩放到合适范围。</description></item>
        /// <item><description>R：重置视图。</description></item>
        /// <item><description>Esc：关闭窗口。</description></item>
        /// </list>
        /// 当焦点位于输入框中时，字母快捷键不生效，避免与文本输入冲突；Esc 仍然有效。
        /// 
        ///</summary>
        private void HandleShortcuts()
        {
            Event e = Event.current;
            if (e.type != EventType.KeyDown) return;

            // Esc 关窗（不依赖 target，任何时候都可用）
            if (e.keyCode == KeyCode.Escape)
            {
                Close();
                e.Use();
                return;
            }

            // 有输入框获得焦点时，不处理字母快捷键
            if (GUIUtility.keyboardControl != 0) return;

            if (target == null) return;

            switch (e.keyCode)
            {
                case KeyCode.F:
                    FitToContent();
                    e.Use();
                    break;

                case KeyCode.R:
                    ResetView();
                    e.Use();
                    break;
            }
        }
        #endregion

        #region 工具 - 吸附与滚动        
        /// <summary>
        ///
        /// 吸附阈值（秒）：按像素阈值随缩放换算，保证任意缩放下手感一致。
        /// 
        ///</summary>
        private float SnapThresholdSeconds => SnapThresholdPixels / pixelsPerSecond;
        /// <summary>
        ///
        /// 对给定时间尝试吸附：优先吸附到其他 Clip 的边界（Delay / Delay+Duration），
        /// 其次吸附到整秒网格。返回吸附后的时间。
        /// 
        ///</summary>
        /// <param name="time">待吸附的时间（秒）。</param>
        /// <param name="excludeIndex">排除的节点索引（当前正在拖拽的节点），-1 表示不排除。</param>
        /// <returns>吸附后的时间；若未命中任何吸附点，则返回原值。</returns>
        private float SnapTime(float time, int excludeIndex)
        {
            float threshold = SnapThresholdSeconds;
            float bestTime = time;
            float bestDist = threshold;

            // Clip 边界
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (i == excludeIndex) continue;
                TweenNode other = Nodes[i];
                float otherStart = other.Delay;
                float otherEnd = other.Delay + other.Duration;

                float dStart = Mathf.Abs(time - otherStart);
                if (dStart < bestDist) { bestDist = dStart; bestTime = otherStart; }

                float dEnd = Mathf.Abs(time - otherEnd);
                if (dEnd < bestDist) { bestDist = dEnd; bestTime = otherEnd; }
            }

            // 整秒：平等竞争
            float rounded = Mathf.Round(time);
            float distToSecond = Mathf.Abs(time - rounded);
            if (distToSecond < bestDist) { bestDist = distToSecond; bestTime = rounded; }

            return bestTime;
        }
        /// <summary>
        ///
        /// 计算 Clip 区垂直滚动的上限（像素）。
        /// 上限 = 内容总高度 - 视口高度，若内容比视口矮则返回 0。
        /// 
        ///</summary>
        /// <returns>垂直滚动上限（像素），恒 ≥ 0。</returns>
        private float GetMaxScrollY()
        {
            float contentHeight = rulerHeight + Nodes.Count * trackHeight + 20f;
            float viewHeight = cachedClipAreaRect.height;
            return Mathf.Max(0f, contentHeight - viewHeight);
        }
        #endregion

        #region 工具 - 视图操作        
        /// <summary>
        ///
        /// 缩放到合适范围：
        /// <list type="bullet">
        /// <item><description>选中节点时：以该节点的 Duration 铺满 Clip 区宽度，并尽量保留 Delay 的左侧留白。</description></item>
        /// <item><description>未选中时：以所有节点最右端的结束时间适配宽度。</description></item>
        /// </list>
        /// 若节点为空或总时长为 0，则退化为 <see cref="ResetView"/>。
        /// 
        ///</summary>
        private void FitToContent()
        {
            if (Nodes == null || Nodes.Count == 0)
            {
                ResetView();
                return;
            }

            // 可用宽度（Clip 区）
            float availableWidth = cachedClipAreaRect.width;
            if (availableWidth <= 1f)
            {
                availableWidth = position.width - nameColumnWidth - paramPanelWidth;
            }
            if (availableWidth <= 1f) return;

            const float padding = 20f;

            // 选中了节点：以该节点的 Duration 铺满宽度，并考虑 Delay 的留白
            if (selectedIndex >= 0 && selectedIndex < Nodes.Count)
            {
                TweenNode node = Nodes[selectedIndex];

                if (node.Duration <= 0.0001f)
                {
                    ResetView();
                    return;
                }

                // 先假设左右各留白 padding，算出 pixelsPerSecond
                float usableWidth = Mathf.Max(1f, availableWidth - padding * 2f);
                float tempPixelsPerSecond = usableWidth / node.Duration;
                tempPixelsPerSecond = Mathf.Clamp(tempPixelsPerSecond, 1f, 20000f);

                // 检查 Delay 转换后的像素宽度是否大于 padding
                float delayPixels = node.Delay * tempPixelsPerSecond;

                if (delayPixels > padding)
                {
                    // Delay 足够大：左右各留白 padding
                    pixelsPerSecond = tempPixelsPerSecond;
                    scrollX = node.Delay * pixelsPerSecond - padding;
                    scrollX = Mathf.Max(0f, scrollX);
                }
                else
                {
                    // Delay 不够大：左边不留白，右边留白
                    usableWidth = Mathf.Max(1f, availableWidth - padding);
                    pixelsPerSecond = usableWidth / node.Duration;
                    pixelsPerSecond = Mathf.Clamp(pixelsPerSecond, 1f, 20000f);

                    scrollX = 0f;
                }

                scrollY = 0f;
                nameScroll.y = 0f;

                Repaint();
                return;
            }

            // 未选中：按所有节点最右端结束时间适配
            float maxEnd = 0f;
            for (int i = 0; i < Nodes.Count; i++)
            {
                float end = Nodes[i].Delay + Nodes[i].Duration;
                if (end > maxEnd) maxEnd = end;
            }

            if (maxEnd <= 0.0001f)
            {
                ResetView();
                return;
            }

            float usableWidthAll = Mathf.Max(1f, availableWidth - padding * 2f);
            pixelsPerSecond = usableWidthAll / maxEnd;
            pixelsPerSecond = Mathf.Clamp(pixelsPerSecond, 1f, 20000f);

            scrollX = 0f;
            scrollY = 0f;
            nameScroll.y = 0f;

            Repaint();
        }
        /// <summary>
        ///
        /// 重置视图：水平 / 垂直滚动归零，缩放恢复默认值（100 px/s），轨道高度恢复默认值（26 px）。
        /// 
        ///</summary>
        private void ResetView()
        {
            scrollX = 0f;
            scrollY = 0f;
            nameScroll.y = 0f;
            pixelsPerSecond = 100f;
            trackHeight = 26f;
        }
        #endregion

        #region 工具 - 视图状态持久化        
        /// <summary>
        ///
        /// 从 <see cref="target"/> 读取持久化的视图状态（缩放、水平滚动、轨道高度）。
        /// 垂直滚动不持久化，每次从顶部开始。
        /// 
        ///</summary>
        private void LoadViewState()
        {
            if (target == null) return;

            pixelsPerSecond = Mathf.Clamp(target.MiniTimeline_PixelsPerSecond, 20f, 400f);
            scrollX = Mathf.Max(0f, target.MiniTimeline_ScrollX);
            trackHeight = Mathf.Clamp(target.MiniTimeline_TrackHeight, MinTrackHeight, MaxTrackHeight);
            // 垂直滚动不持久化，每次从顶部开始
            scrollY = 0f;
            nameScroll.y = 0f;
        }
        /// <summary>
        ///
        /// 将当前视图状态（缩放、水平滚动、轨道高度）写回 <see cref="target"/>。
        /// 垂直滚动不保存。写入后调用 <see cref="EditorUtility.SetDirty"/>。
        /// 
        ///</summary>
        private void SaveViewState()
        {
            if (target == null) return;

            target.MiniTimeline_PixelsPerSecond = pixelsPerSecond;
            target.MiniTimeline_ScrollX = scrollX;
            target.MiniTimeline_TrackHeight = trackHeight;
            // 垂直滚动不保存

            EditorUtility.SetDirty(target);
        }
        #endregion

        #region 绘制 - 参数面板        
        /// <summary>
        ///
        /// 绘制右侧节点参数面板，用于编辑当前选中动画节点的属性。
        /// 布局形式与左侧名字列保持一致：固定标题栏 + 可滚动内容区。
        /// 
        ///</summary>
        /// <param name="area">参数面板在窗口坐标系中的矩形区域。</param>
        private void DrawParameterPanel(Rect area)
        {
            GUI.BeginGroup(area);

            // 标题栏区域（固定，不参与滚动），高度与名字列一致
            Rect rulerParamRect = new Rect(0, 0, area.width, rulerHeight);

            // 滚动视口区域（从标题栏下方开始）
            Rect scrollViewportRect = new Rect(0, rulerHeight, area.width, area.height - rulerHeight);

            // 内容高度：按实际控件行数估算，不足视口高度时用视口高度撑满
            float contentHeight = Mathf.Max(scrollViewportRect.height, GetParamContentHeight());

            paramScroll = GUI.BeginScrollView(
                scrollViewportRect,
                paramScroll,
                new Rect(0, 0, scrollViewportRect.width - 16f, contentHeight),
                false,
                true,
                GUIStyle.none,
                GUI.skin.verticalScrollbar);

            // 内容区的局部坐标原点在 scrollViewportRect 左上角
            Rect contentRect = new Rect(0, 0, scrollViewportRect.width - 16f, contentHeight);

            if (selectedIndex < 0 || selectedIndex >= Nodes.Count)
            {
                GUI.Label(new Rect(6, 6, contentRect.width - 12, 40),
                          "点击左侧轨道或 Clip 以编辑参数。", EditorStyles.miniLabel);
            }
            else
            {
                DrawParamFields(contentRect);
            }

            GUI.EndScrollView();

            // 点击参数面板空白处：清焦点（不取消选中，参数面板点空白不该取消选中）
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0
                && !Event.current.alt && scrollViewportRect.Contains(Event.current.mousePosition))
            {
                GUIUtility.keyboardControl = 0;
                Event.current.Use();
                Repaint();
            }

            // 标题栏最后画，盖在最上层，样式与名字列标题栏一致
            EditorGUI.DrawRect(rulerParamRect, new Color(0.18f, 0.18f, 0.18f));
            GUI.Label(new Rect(rulerParamRect.x + 6, rulerParamRect.y, rulerParamRect.width, rulerParamRect.height),
                      "节点参数", EditorStyles.miniLabel);

            GUI.EndGroup();
        }
        /// <summary>
        ///
        /// 估算参数面板内容所需高度（像素），用于滚动视图的内容矩形。
        /// 
        ///</summary>
        /// <returns>内容高度（像素）。未选中时返回 60；选中时按固定行数估算。</returns>
        private float GetParamContentHeight()
        {
            if (selectedIndex < 0 || selectedIndex >= Nodes.Count) return 60f;

            // 行高按编辑器默认单行高度估算
            const float rowH = 18f;
            const float spaceH = 6f;

            // 名称 / 启用 / 类型 + 空行 + 耗时 / 延迟 / 缓动 + 空行 + 两个开关
            return rowH * 8 + spaceH * 2 + 20f;
        }
        /// <summary>
        ///
        /// 绘制选中节点的参数字段（手动 Rect 布局，与名字列的绝对定位风格一致）。
        /// 任意字段变化时通过 <see cref="Undo.RecordObject"/> + <see cref="EditorUtility.SetDirty"/> 实时生效。
        /// 
        ///</summary>
        /// <param name="contentRect">内容区矩形（滚动视图局部坐标）。</param>
        private void DrawParamFields(Rect contentRect)
        {
            TweenNode node = Nodes[selectedIndex];

            const float rowH = 18f;
            const float spaceH = 6f;
            const float labelW = 110f;   // 标签列宽
            const float padX = 6f;

            float y = 6f;
            float fieldX = padX + labelW;
            float fieldW = Mathf.Max(40f, contentRect.width - fieldX - padX);

            EditorGUI.BeginChangeCheck();

            // 名称
            GUI.Label(new Rect(padX, y, labelW, rowH), "名称");
            node.Indicator = EditorGUI.TextField(new Rect(fieldX, y, fieldW, rowH), node.Indicator);
            y += rowH;

            // 启用
            GUI.Label(new Rect(padX, y, labelW, rowH), "启用");
            node.Enabled = EditorGUI.Toggle(new Rect(fieldX, y, fieldW, rowH), node.Enabled);
            y += rowH;

            // 类型
            GUI.Label(new Rect(padX, y, labelW, rowH), "类型");
            node.Type = (TweenNodeType)EditorGUI.EnumPopup(new Rect(fieldX, y, fieldW, rowH), node.Type);
            y += rowH + spaceH;

            // 耗时
            GUI.Label(new Rect(padX, y, labelW, rowH), "耗时");
            node.Duration = EditorGUI.FloatField(new Rect(fieldX, y, fieldW, rowH), node.Duration);
            y += rowH;

            // 延迟
            GUI.Label(new Rect(padX, y, labelW, rowH), "延迟");
            node.Delay = EditorGUI.FloatField(new Rect(fieldX, y, fieldW, rowH), node.Delay);
            y += rowH;

            // 缓动
            GUI.Label(new Rect(padX, y, labelW, rowH), "缓动");
            node.Ease = (EaseMode)EditorGUI.EnumPopup(new Rect(fieldX, y, fieldW, rowH), node.Ease);
            y += rowH + spaceH;

            // 重置设为起始值
            GUI.Label(new Rect(padX, y, labelW, rowH), "重置设为起始值");
            node.Rewind_Set_Startvalue = EditorGUI.Toggle(new Rect(fieldX, y, fieldW, rowH), node.Rewind_Set_Startvalue);
            y += rowH;

            // 完成设为结束值
            GUI.Label(new Rect(padX, y, labelW, rowH), "完成设为结束值");
            node.Complete_Set_Endvalue = EditorGUI.Toggle(new Rect(fieldX, y, fieldW, rowH), node.Complete_Set_Endvalue);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Edit Tween Node");
                EditorUtility.SetDirty(target);
                Repaint();
            }
        }
        #endregion
    }
}