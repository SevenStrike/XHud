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
    using SevenStrikeModules.XTween;
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    /// <summary> 
    ///
    /// XHud 图元动画轨道编辑器
    /// <para/>
    /// 用于以时间轴方式可视化编辑 <see cref="XHud_Module_Primitive_Tween"/> 的动画节点，支持：
    /// <list type="bullet">
    /// <item><description>中键 / Alt+左键拖拽平移视图；</description></item>
    /// <item><description>Alt+滚轮以鼠标为锚点缩放，Shift+滚轮调整轨道高度，Ctrl+滚轮垂直滚动；</description></item>
    /// <item><description>拖拽 Clip 修改 Delay / Duration，并支持边界吸附与边缘自动平移；</description></item>
    /// <item><description>Ctrl / Shift 多选 Clip，可整组拖动；</description></item>
    /// <item><description>快捷键：F 缩放到合适范围 / R 重置视图 / Esc 关闭窗口</description></item>
    /// </list>
    /// 所有改动通过 <see cref="Undo.RecordObject"/> + <see cref="EditorUtility.SetDirty"/> 实时生效
    /// <para/>
    /// Delay / Duration 统一量化为两位小数（10ms 精度），保证视觉对齐且避免浮点尾数
    /// 
    ///</summary>
    public class Editor_XHud_Module_Primitive_Tween_Tracker : EditorWindow
    {
        #region 常量：布局参数
        /// <summary> 
        ///顶部工具栏高度（像素）名字列与 Clip 区的局部 Y 坐标均以此作为起点
        ///</summary>
        private const float TopBarHeight = 23f;
        /// <summary> 
        ///轨道最小高度（像素），用于 Shift+滚轮调整轨道高度时的下限
        ///</summary>
        private const float MinTrackHeight = 16f;
        /// <summary> 
        ///轨道最大高度（像素），用于 Shift+滚轮调整轨道高度时的上限
        ///</summary>
        private const float MaxTrackHeight = 80f;
        /// <summary> 
        ///名字行 / 轨道的垂直内边距（像素）两侧同步使用，保证行间距一致
        ///</summary>
        private const int RowVerticalPadding = 1;
        /// <summary> 
        ///轨道的类型标识条厚度（像素）
        ///</summary>
        private const int ClipTypeLineThickness = 1;
        /// <summary> 
        ///水平滚动条高度（像素）Clip 区自绘水平滚动条使用此高度；名字列底部同步预留同高区域
        ///</summary>
        private const float HorizontalScrollbarHeight = 16f;
        /// <summary> 
        ///底部工具栏高度（像素）用于放置状态信息 / 操作按钮
        ///</summary>
        private const float BottomBarHeight = 24f;
        /// <summary> 
        ///名称行内小按钮的尺寸（像素）
        ///</summary>
        private const float NameRowButtonSize = 30;
        #endregion

        #region 常量：交互参数
        /// <summary> 
        ///吸附判定阈值（像素）换算成秒后用于时间吸附，保证任意缩放下手感一致
        ///</summary>
        private const float SnapThresholdPixels = 13f;
        /// <summary> 
        ///Clip 左右边缘用于判定「拉伸」的命中宽度（像素）
        ///</summary>
        private const float ClipEdgeZone = 12f;
        /// <summary> 
        ///Shift+滚轮调整轨道高度时，每格滚轮对应的比例变化量
        ///</summary>
        private const float TrackHeightZoomStep = 0.02f;
        #endregion

        #region 常量：拖拽自动平移
        /// <summary> 
        ///拖拽时触发自动平移的边缘宽度（像素）鼠标进入该区域后视图自动滚动
        ///</summary>
        private const float AutoScrollEdgeMargin = 24f;
        /// <summary> 
        ///拖拽自动平移的基础速度（像素/帧），实际速度会随鼠标接近边缘的程度放大
        ///</summary>
        private const float AutoScrollSpeed = 8f;
        #endregion

        #region 常量：内容矩形
        /// <summary> 
        ///时间轴内容矩形的最小宽度（秒），防止节点为空或全在 0 时刻时滚动范围过窄
        ///</summary>
        private const float MinContentSeconds = 10f;
        /// <summary> 
        ///时间轴内容矩形右侧额外留白（像素），方便把 Clip 拖到最右端
        ///</summary>
        private const float ContentRightPaddingPixels = 200f;
        #endregion

        #region 常量：名字列宽度拖拽
        /// <summary> 
        ///名字列宽度可调下限（像素）
        ///</summary>
        private const float MinNameColumnWidth = 160f;
        /// <summary> 
        ///名字列宽度可调上限（像素）
        ///</summary>
        private const float MaxNameColumnWidth = 520f;
        /// <summary> 
        ///名字列右边缘拖拽热区的半宽（像素）热区总宽 = 此值 * 2
        ///</summary>
        private const float NameColumnResizeZone = 6f;
        private const float DefaultNameColumnWidth = 280f;
        /// <summary> 
        ///名字列的宽度
        ///</summary>
        private const string PrefKey_NameWidthWidth = "xData_PrimitiveTweenTracker_NameColumn_Width";
        #endregion

        #region 常量：参数面板宽度拖拽
        /// <summary> 
        ///参数面板宽度可调下限（像素）
        ///</summary>
        private const float MinParamPanelWidth = 100f;
        /// <summary> 
        ///参数面板宽度可调上限（像素）
        ///</summary>
        private const float MaxParamPanelWidth = 600f;
        /// <summary> 
        ///参数面板左边缘拖拽热区的半宽（像素）热区总宽 = 此值 * 2
        ///</summary>
        private const float ParamPanelResizeZone = 10f;
        private const float DefaultParamPanelWidth = 280f;
        /// <summary> 
        ///参数列的宽度
        ///</summary>
        private const string PrefKey_ParamWidthHeight = "xData_PrimitiveTweenTracker_Param_Width";
        #endregion

        #region 字段：参数面板宽度拖拽状态
        /// <summary> 
        ///是否正在拖拽调整参数面板宽度
        ///</summary>
        private bool isResizingParamPanel = false;
        /// <summary> 
        ///拖拽参数面板宽度时占用的 ControlID
        ///</summary>
        private int paramPanelResizeControlID;
        /// <summary> 
        ///鼠标当前是否悬停在参数面板宽度拖拽热区上由 MouseMove / Repaint 更新
        ///</summary>
        private bool isHoveringParamResizeZone = false;
        #endregion

        #region 常量：颜色
        /// <summary> 
        ///轨道基础底色
        ///</summary>
        private static readonly Color ColorBasedBg = XGUI_Utilitys.HexString_To_Color("313131");
        /// <summary> 
        ///名字列未选中行的背景色
        ///</summary>
        private static readonly Color ColorNameRowBg = XGUI_Utilitys.HexString_To_Color("262626");
        /// <summary> 
        ///名字列选中行的背景色
        ///</summary>
        private static readonly Color ColorNameRowSelectedBg = Color.white * 0.35f;
        /// <summary> 
        ///拖动整段 Clip 时，两端边界的垂直线颜色（区别于吸附黄线）
        ///</summary>
        private static readonly Color ColorClipEdgeGuide = Color.white * 0.45f;
        /// <summary> 
        ///轨道前景色（常规）
        ///</summary>
        private Color ColorClipFG_Normal = Color.black * 0.3f;
        /// <summary> 
        ///轨道前景色（禁用）
        ///</summary>
        private Color ColorClipFG_Disabled = Color.gray * 0.55f;
        /// <summary> 
        ///轨道前景色（选中）
        ///</summary>
        private static readonly Color ColorClipFG_Selected = Color.white * 0.9f;
        /// <summary> 
        ///轨道背景色
        ///</summary>
        private static readonly Color ColorClipBG = XGUI_Utilitys.HexString_To_Color("363636");
        /// <summary> 
        ///标尺背景色
        ///</summary>
        private static readonly Color ColorRulerBG = XGUI_Utilitys.HexString_To_Color("1e1e1e");
        /// <summary> 
        ///名称区标题背景色
        ///</summary>
        private static readonly Color Color_Name_Header_BG = XGUI_Utilitys.HexString_To_Color("1e1e1e");
        /// <summary> 
        ///参数区标题背景色
        ///</summary>
        private static readonly Color Color_Params_Header_BG = XGUI_Utilitys.HexString_To_Color("1e1e1e");
        #endregion

        #region 字段：图标
        /// <summary>
        /// 轨道编辑器图标
        /// </summary>
        private Texture2D icon_timeline_logo;
        /// <summary>
        /// 空动画列表图标
        /// </summary>
        private Texture2D icon_null_check;
        /// <summary> 
        ///帮助按钮常规图标
        ///</summary>
        private Texture2D icon_help_r;
        /// <summary> 
        ///帮助按钮按下图标
        ///</summary>
        private Texture2D icon_help_p;
        /// <summary> 
        ///删除按钮常规图标
        ///</summary>
        private Texture2D icon_del_r;
        /// <summary> 
        ///删除按钮按下图标
        ///</summary>
        private Texture2D icon_del_p;
        /// <summary> 
        ///插入按钮常规图标
        ///</summary>
        private Texture2D icon_add_r;
        /// <summary> 
        ///插入按钮按下图标
        ///</summary>
        private Texture2D icon_add_p;
        /// <summary> 
        ///菜单按钮常规图标
        ///</summary>
        private Texture2D icon_menu_r;
        /// <summary> 
        ///菜单按钮按下图标
        ///</summary>
        private Texture2D icon_menu_p;
        /// <summary> 
        ///启用状态图标（常规）
        ///</summary>
        private Texture2D icon_enabled_r;
        /// <summary> 
        ///启用状态图标（按下）
        ///</summary>
        private Texture2D icon_enabled_p;
        /// <summary> 
        ///禁用状态图标（常规）
        ///</summary>
        private Texture2D icon_disabled_r;
        /// <summary> 
        ///禁用状态图标（按下）
        ///</summary>
        private Texture2D icon_disabled_p;
        /// <summary> 
        ///类型指示器小圆点图标
        ///</summary>
        private Texture2D icon_led;
        /// <summary>
        /// 动画类型图标 - 颜色
        /// </summary>
        private Texture2D icon_type_color;
        /// <summary>
        /// 动画类型图标 - 淡化
        /// </summary>
        private Texture2D icon_type_fade;
        /// <summary>
        /// 动画类型图标 - 填充
        /// </summary>
        private Texture2D icon_type_fill;
        /// <summary>
        /// 动画类型图标 - 位移
        /// </summary>
        private Texture2D icon_type_move;
        /// <summary>
        /// 动画类型图标 - 旋转
        /// </summary>
        private Texture2D icon_type_rotator;
        /// <summary>
        /// 动画类型图标 - 缩放
        /// </summary>
        private Texture2D icon_type_scale;
        /// <summary>
        /// 动画类型图标 - 尺寸
        /// </summary>
        private Texture2D icon_type_size;
        /// <summary>
        /// 动画类型图标 - 打字机
        /// </summary>
        private Texture2D icon_type_writter;
        /// <summary>
        /// 新增首个动画按钮图标（常规）
        /// </summary>
        private Texture2D icon_null_add_r;
        /// <summary>
        /// 新增首个动画按钮图标（按下）
        /// </summary>
        private Texture2D icon_null_add_p;


        #endregion

        #region 字段：视图参数
        /// <summary> 
        ///每秒对应的像素数（缩放比例）范围 1～20000，由滚轮缩放或 FitToContent 修改
        ///</summary>
        private float pixelsPerSecond = 100f;
        /// <summary> 
        ///左侧名字列宽度（像素）可拖拽调整，范围 [MinNameColumnWidth, MaxNameColumnWidth]
        ///</summary>
        private float nameColumnWidth = 280f;
        /// <summary> 
        ///右侧参数面板宽度（像素）当前为固定值，未提供拖拽调整
        ///</summary>
        private float paramPanelWidth = 280f;
        /// <summary> 
        ///单条轨道的高度（像素）由 Shift+滚轮调整，范围 [MinTrackHeight, MaxTrackHeight]
        ///</summary>
        private float trackHeight = 26f;
        /// <summary> 
        ///顶部刻度尺高度（像素）名字列 / Clip 区 / 参数面板的标题栏高度共用此值
        ///</summary>
        private float rulerHeight = 25;
        /// <summary> 
        ///动画类型指示器点的大小（像素）
        ///</summary>
        private float TweenTypeDotSize = 3f;
        #endregion

        #region 字段：滚动状态
        /// <summary> 
        ///名字列的滚动位置仅使用其 y 分量，与 <see cref="scrollPos"/> 保持同步
        ///</summary>
        private Vector2 nameScroll;
        /// <summary> 
        ///参数面板的滚动位置独立于名字列 / Clip 区
        ///</summary>
        private Vector2 paramScroll;
        /// <summary> 
        ///Clip 区的滚动位置（像素）x = 水平时间偏移，y = 垂直轨道偏移
        ///</summary>
        private Vector2 scrollPos;
        #endregion

        #region 字段：目标数据与选中
        /// <summary> 
        ///当前窗口编辑的图元动画器为 null 时窗口只显示提示信息
        ///</summary>
        private XHud_Module_Primitive_Tween target;
        /// <summary> 
        ///当前动画器的动画节点列表（<see cref="target"/> 的快捷访问）
        ///</summary>
        private List<TweenNode> Nodes => target.PrimitiveTweenNodes;
        /// <summary> 
        ///当前选中的动画节点索引-1 表示未选中，参数面板会显示提示文本
        ///</summary>
        private int selectedIndex = -1;
        /// <summary> 
        ///多选集合始终包含 selectedIndex（若 &gt;= 0）用于批量拖动
        ///</summary>
        private readonly HashSet<int> selectedIndices = new HashSet<int>();
        #endregion

        #region 字段：Clip 拖拽状态
        /// <summary> 
        ///Clip 拖拽模式值为 <see cref="DragMode.None"/> 表示当前无拖拽
        ///</summary>
        private DragMode dragMode = DragMode.None;
        /// <summary> 
        ///当前正在拖拽的动画节点索引-1 表示无
        ///</summary>
        private int draggingIndex = -1;
        /// <summary> 
        ///Clip 拖拽时占用的 ControlID，用于在拖拽期间锁定 <see cref="GUIUtility.hotControl"/>
        ///</summary>
        private int dragControlID;
        /// <summary> 
        ///拖拽开始时鼠标对应的内容时间（秒），已包含 <see cref="scrollPos"/>用于计算位移
        ///</summary>
        private float dragStartContentSecond;
        /// <summary> 
        ///拖拽开始时节点的 Delay（秒）用于计算相对位移
        ///</summary>
        private float dragStartDelay;
        /// <summary> 
        ///拖拽开始时节点的 Duration（秒）用于计算相对位移
        ///</summary>
        private float dragStartDuration;
        /// <summary> 
        ///本帧吸附到的参考时间（秒）&lt;0 表示本帧无吸附非负时绘制黄色参考线
        ///</summary>
        private float snapGuideSecond = -1f;
        /// <summary> 
        ///Move 拖拽时锁定的吸附侧：0 = 未定，1 = 左边缘，2 = 右边缘整个拖拽过程保持不变
        ///</summary>
        private int moveDragAnchorSide = 0;
        /// <summary> 
        ///拖拽开始时，所有被选中节点的起始 Delay（key = 节点索引）多选整体平移时保证相对间隔不变
        ///</summary>
        private readonly Dictionary<int, float> dragStartDelays = new Dictionary<int, float>();
        /// <summary> 
        ///拖拽开始时，所有被选中节点的起始 Duration
        ///</summary>
        private readonly Dictionary<int, float> dragStartDurations = new Dictionary<int, float>();
        /// <summary> 
        ///多选拖拽时的「主节点」索引（选中最上面那个）吸附以此为准单选时等于 selectedIndex
        ///</summary>
        private int primaryDragIndex = -1;
        /// <summary> 
        ///本次 Clip 拖拽所属的 Undo 组 IDMouseDown 时开组，MouseUp 时 Collapse
        ///</summary>
        private int dragUndoGroup = -1;
        #endregion

        #region 字段：名字列宽度拖拽状态
        /// <summary> 
        ///是否正在拖拽调整名字列宽度
        ///</summary>
        private bool isResizingNameColumn = false;
        /// <summary> 
        ///拖拽名字列宽度时占用的 ControlID
        ///</summary>
        private int nameColumnResizeControlID;
        /// <summary> 
        ///鼠标当前是否悬停在名字列宽度拖拽热区上由 MouseMove / Repaint 更新
        ///</summary>
        private bool isHoveringNameResizeZone = false;
        #endregion

        #region 字段：吸附设置
        /// <summary> 
        ///默认吸附开关由工具栏按钮控制按住 Shift 拖拽时会强制吸附，无视此开关
        ///</summary>
        private bool snapEnabled = true;
        #endregion

        #region 字段：平移状态
        /// <summary> 
        ///是否正在使用中键 / Alt+左键平移视图
        ///</summary>
        private bool isPanning = false;
        /// <summary> 
        ///平移时占用的 ControlID，用于在拖拽期间锁定 <see cref="GUIUtility.hotControl"/>
        ///</summary>
        private int panControlID;
        #endregion

        #region 字段：布局缓存
        /// <summary> 
        ///Clip 区在窗口坐标系中的矩形由 <see cref="OnGUI"/> 每帧写入
        ///</summary>
        private Rect cachedClipAreaRect;
        /// <summary> 
        ///名字列在窗口坐标系中的矩形由 <see cref="OnGUI"/> 每帧写入
        ///</summary>
        private Rect cachedNameAreaRect;
        /// <summary> 
        ///本帧左键按下是否命中了 Clip用于在 Clip 区空白处点击时取消选中
        ///</summary>
        private bool clipHitThisFrame;
        #endregion

        #region 常量：窗口尺寸持久化
        /// <summary> 
        ///窗口宽度持久化使用的 key
        ///</summary>
        private const string PrefKey_WindowWidth = "xData_PrimitiveTweenTracker_WinW";
        /// <summary> 
        ///窗口高度持久化使用的 key
        ///</summary>
        private const string PrefKey_WindowHeight = "xData_PrimitiveTweenTracker_WinH";
        /// <summary> 
        ///首次打开窗口时使用的默认尺寸（宽 × 高，像素）
        ///</summary>
        private static readonly Vector2 DefaultWindowSize = new Vector2(1000f, 450f);
        /// <summary> 
        ///窗口尺寸允许的最小值，避免从持久化数据中恢复出不可用尺寸
        ///</summary>
        private static readonly Vector2 MinWindowSize = new Vector2(800f, 150f);
        /// <summary> 
        ///窗口尺寸允许的最大值，避免异常尺寸被持久化
        ///</summary>
        private static readonly Vector2 MaxWindowSize = new Vector2(4000f, 3000f);
        #endregion

        #region 嵌套类型：拖拽模式
        /// <summary> 
        ///Clip 拖拽模式，描述当前鼠标拖拽的是 Clip 的哪个部分
        ///</summary>
        private enum DragMode
        {
            /// <summary> 
            ///未拖拽
            ///</summary>
            None,
            /// <summary> 
            ///整体移动（修改 Delay，Duration 不变）
            ///</summary>
            Move,
            /// <summary> 
            ///拖拽左边缘（同时修改 Delay 与 Duration，右端固定）
            ///</summary>
            LeftEdge,
            /// <summary> 
            ///拖拽右边缘（仅修改 Duration，左端固定）
            ///</summary>
            RightEdge
        }
        #endregion

        #region 窗口入口
        /// <summary> 
        ///打开（或复用）迷你时间轴窗口，并绑定指定的图元动画器
        ///</summary>
        /// <param name="tween">要编辑的图元动画器</param>
        public static void OpenWith(XHud_Module_Primitive_Tween tween)
        {
            Editor_XHud_Module_Primitive_Tween_Tracker window = (Editor_XHud_Module_Primitive_Tween_Tracker)EditorWindow.GetWindow(typeof(Editor_XHud_Module_Primitive_Tween_Tracker), false, "XHUD 图元动画轨道编辑器", true);

            window.minSize = MinWindowSize;
            Vector2 savedSize = LoadWindowSize();

            XGUI.CenterEditorWindow(new Vector2Int((int)savedSize.x, (int)savedSize.y), window, false, false);

            if (window.target != tween)
            {
                window.SaveViewState();
                window.target = tween;
                window.selectedIndex = -1;
                window.selectedIndices.Clear();
                window.LoadViewState();
            }
            window.Repaint();
            window.Focus();
        }
        #endregion

        #region 生命周期
        /// <summary> 
        ///Unity 启用回调：注册 Undo/Redo 监听，加载图标
        ///</summary>
        private void OnEnable()
        {
            Undo.undoRedoPerformed -= OnUndoRedo;
            Undo.undoRedoPerformed += OnUndoRedo;
            icon_del_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/icon_del_r");
            icon_del_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/icon_del_p");
            icon_add_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/icon_add_r");
            icon_add_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/icon_add_p");
            icon_menu_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/icon_menu_r");
            icon_menu_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/icon_menu_p");
            icon_enabled_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/icon_enabled_r");
            icon_enabled_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/icon_enabled_p");
            icon_disabled_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/icon_disabled_r");
            icon_disabled_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/icon_disabled_p");
            icon_help_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/icon_help_r");
            icon_help_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/icon_help_p");
            icon_type_color = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/anim_type_color");
            icon_type_fade = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/anim_type_fade");
            icon_type_fill = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/anim_type_fill");
            icon_type_move = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/anim_type_move");
            icon_type_rotator = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/anim_type_rotator");
            icon_type_scale = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/anim_type_scale");
            icon_type_size = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/anim_type_size");
            icon_type_writter = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/anim_type_writter");
            icon_timeline_logo = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/icon_timeline_logo");
            icon_null_check = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/icon_null_check");
            icon_null_add_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/icon_null_add_r");
            icon_null_add_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/icon_null_add_p");

            icon_led = XGUI.GetBasedIcon("icon_field_status");
        }
        /// <summary> 
        ///Unity 禁用回调：注销 Undo/Redo 监听，并保存视图状态
        ///</summary>
        private void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoRedo;
            SaveViewState();
            SaveWindowSize();
        }
        /// <summary> 
        ///Undo / Redo 执行后回调：清拖拽状态，强制重绘
        ///</summary>
        private void OnUndoRedo()
        {
            dragMode = DragMode.None;
            draggingIndex = -1;

            snapGuideSecond = -1f;
            primaryDragIndex = -1;
            moveDragAnchorSide = 0;

            dragStartDelays.Clear();
            dragStartDurations.Clear();
            dragUndoGroup = -1;

            isResizingNameColumn = false;
            nameColumnResizeControlID = 0;

            isResizingParamPanel = false;
            paramPanelResizeControlID = 0;

            GUIUtility.hotControl = 0;
            if (target == null || target.Equals(null))
            {
                Close(); return;
            }
            selectedIndices.RemoveWhere(i => i < 0 || i >= Nodes.Count);
            if (selectedIndex >= Nodes.Count)
                selectedIndex = -1;
            if (selectedIndex < 0 && selectedIndices.Count > 0)
                selectedIndex = GetTopmostSelected();
            Repaint();
        }
        /// <summary>
        /// 绘制窗口内容每帧调用
        /// <para/>
        /// 整体流程：
        /// <list type="bullet">
        /// <item><description>顶部工具栏；</description></item>
        /// <item><description>计算三大区域矩形与名字列拖拽热区；</description></item>
        /// <item><description>滚轮 / 名字列宽度输入；</description></item>
        /// <item><description>三大区域绘制；</description></item>
        /// <item><description>名字列光标；</description></item>
        /// <item><description>底部工具栏；</description></item>
        /// <item><description>快捷键与全局兜底</description></item>
        /// </list>
        /// </summary>
        private void OnGUI()
        {
            #region 空动画列表检测
            bool valid = target != null || !target.Equals(null);
            if (target.PrimitiveTweenNodes.Count > 0)
                valid = true;
            else
                valid = false;
            #endregion

            #region 绘制：顶部工具栏
            DrawToolbar(valid);
            XGUI.gui_box(new Rect(0, TopBarHeight, position.width, 1), Color.black * 0.3f);
            #endregion

            #region 目标有效性检查
            if (!valid)
            {
                //EditorGUILayout.HelpBox("请通过动画器面板的按钮打开此窗口", MessageType.Info);
                float dis = (position.height - TopBarHeight) / 2 - 30;
                XGUI.layout_group_start(
                    type: XGUIContainerType.Horizontal,
                    bg_fill: XGUIFilled.透明,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: Color.white,
                    absolute_margin: true,
                    absolute_padding: true,
                    bg_height: TopBarHeight,
                    margin: new RectOffset(0, 0, (int)dis, 0),
                    padding: new RectOffset(10, 10, 0, 0));

                XGUI.layout_flexspace();

                XGUI.layout_icon(
                    icon: icon_null_check,
                    icon_color: Color.white * 0.5f,
                    width: 15,
                    height: 15,
                    icon_border: new RectOffset(0, 0, 0, 0),
                    icon_offset: new Vector2(0, 0),
                    icon_margin: new RectOffset(0, 0, 0, 0),
                    icon_padding: new RectOffset(0, 0, 0, 0),
                    icon_alignment: XGUIIconAlignment.默认);

                XGUI.layout_space(10);

                XGUI.layout_label(
                    text: "暂未发现任何动画，请点击按钮创建一个动画",
                    size: XGUIFontSize.M,
                    text_color: Color.white * 0.7f,
                    margin: new RectOffset(0, 0, 0, 0),
                    offset: new Vector2(0, -2),
                    clipping: XGUI.TryEllipsisClipping(),
                    width: 250,
                    font_style: FontStyle.Normal,
                    anchor: TextAnchor.MiddleCenter,
                    font: XGUI.GetFont("xg-regular"));

                XGUI.layout_flexspace();
                XGUI.layout_group_end(type: XGUIContainerType.Horizontal);

                XGUI.layout_group_start(
                 type: XGUIContainerType.Horizontal,
                 bg_fill: XGUIFilled.透明,
                 bg_color: XGUIColor.亮白,
                 bg_color_gui: Color.white,
                 absolute_margin: true,
                 absolute_padding: true,
                 margin: new RectOffset(0, 0, 15, 0),
                 padding: new RectOffset(0, 0, 0, 0));
                XGUI.layout_flexspace();
                if (XGUI.layout_button(
                    tooltip: "增加",
                    tex_release: icon_null_add_r,
                    tex_press: icon_null_add_p,
                    tex_gui_color: Color.white,
                    width: icon_null_add_r.width,
                    height: icon_null_add_r.height))
                {
                    InsertNodeAt(0);
                }
                XGUI.layout_flexspace();
                XGUI.layout_group_end(type: XGUIContainerType.Horizontal);


                return;
            }
            else
            {

                #region 布局：计算三大区域矩形
                float timelineTotalWidth = position.width - paramPanelWidth;
                float viewHeight = position.height - TopBarHeight - BottomBarHeight;
                Rect nameArea = new Rect(0, TopBarHeight, nameColumnWidth, viewHeight);
                Rect clipArea = new Rect(nameColumnWidth, TopBarHeight, timelineTotalWidth - nameColumnWidth, viewHeight);
                Rect paramArea = new Rect(timelineTotalWidth, TopBarHeight, paramPanelWidth, viewHeight);
                cachedNameAreaRect = nameArea;
                cachedClipAreaRect = clipArea;
                #endregion

                #region 布局：名字列宽度拖拽热区
                Rect nameResizeZone = new Rect(
                    nameArea.xMax - NameColumnResizeZone,
                    TopBarHeight,
                    NameColumnResizeZone * 2f,
                    viewHeight);
                #endregion

                #region 布局：参数面板宽度拖拽热区
                Rect paramResizeZone = new Rect(
                    paramArea.x - ParamPanelResizeZone + 10,
                    TopBarHeight,
                    ParamPanelResizeZone * 2f,
                    viewHeight);
                #endregion

                #region 交互：参数面板宽度输入
                HandleParamPanelResizeInput(paramResizeZone);
                #endregion

                #region 交互：滚轮
                HandleScrollWheel();
                #endregion

                #region 交互：名字列宽度输入（必须先于 Clip 区，优先抢 hotControl）
                HandleNameColumnResizeInput(nameResizeZone);
                #endregion

                #region 绘制：三大主区域
                DrawClipArea(clipArea);
                DrawNameColumn(nameArea);
                DrawParameterPanel(paramArea);
                #endregion

                #region 绘制：名字列宽度光标（必须在所有绘制之后，避免被 Clip 区光标覆盖）
                DrawNameColumnResizeCursor(nameResizeZone);
                #endregion

                #region 绘制：参数面板宽度光标
                DrawParamPanelResizeCursor(paramResizeZone);
                #endregion

                #region 交互：快捷键与全局兜底
                HandleShortcuts();
                HandleGlobalMouseUp();
                #endregion
            }
            #endregion

            #region 绘制：底部工具栏
            Rect bottomBarArea = new Rect(0, position.height - BottomBarHeight, position.width, BottomBarHeight);
            DrawBottomToolbar(bottomBarArea);
            #endregion
        }
        #endregion

        #region 绘制：工具栏
        /// <summary> 
        ///绘制顶部工具栏：重置视图、缩放到合适范围、吸附开关，以及右侧提示文本
        ///</summary>
        private void DrawToolbar(bool valid)
        {
            XGUI.layout_group_start(
               type: XGUIContainerType.Horizontal,
               bg_fill: XGUIFilled.透明,
               bg_color: XGUIColor.亮白,
               bg_color_gui: Color.white,
               absolute_margin: true,
               absolute_padding: true,
               bg_height: TopBarHeight,
               margin: new RectOffset(0, 0, 0, 0),
               padding: new RectOffset(10, 10, 0, 0));

            #region 时间线图标
            XGUI.layout_icon(
                icon: icon_timeline_logo,
                icon_color: Color.white,
                width: 14,
                height: 14,
                icon_border: new RectOffset(0, 0, 0, 0),
                icon_offset: new Vector2(0, 5),
                icon_margin: new RectOffset(5, 25, 0, 0),
                icon_padding: new RectOffset(0, 0, 0, 0),
                icon_alignment: XGUIIconAlignment.默认);
            #endregion

            #region 目标名称
            XGUI.layout_label(
                text: target.name,
                size: XGUIFontSize.M,
                text_color: XHud_Dashboard.Theme_Primary,
                margin: new RectOffset(0, 10, 0, 0),
                offset: new Vector2(0, 2),
                clipping: XGUI.TryEllipsisClipping(),
                font_style: FontStyle.Bold,
                anchor: TextAnchor.MiddleLeft,
                width: 150,
                font: XGUI.GetFont("xg-regular"));
            #endregion

            if (valid)
            {
                #region 分割线
                XGUI.layout_seperator(
                    thickness: 1,
                    dir: XGUISeplineDir.垂直,
                    color: Color.black * 0.4f,
                    padding: new RectOffset(0, 0, 0, 0),
                    margin: new RectOffset(10, 10, 0, 0));
                #endregion

                #region 按钮 - 重置轨道
                if (XGUI.layout_button(
                    text: "重置轨道",
                    tooltip: "",
                    bg_fill: XGUIFilled.无,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: Color.white,
                    button_text_color: Color.white * 0.9f,
                    press_fill: XGUIFilled.透明,
                    press_color: XGUIColor.无,
                    press_text_color: XHud_Dashboard.Theme_Primary,
                    font_size: XGUIFontSize.M,
                    anchor: TextAnchor.MiddleCenter,
                    margin: new RectOffset(0, 0, 0, 0),
                    padding: new RectOffset(0, 0, 3, 0),
                    layout_min_width: 0,
                    layout_width: 100, button_text_font: XGUI.GetFont("xg-regular")))
                {
                    ResetView();
                }
                #endregion

                #region 分割线
                XGUI.layout_seperator(
                    thickness: 1,
                    dir: XGUISeplineDir.垂直,
                    color: Color.black * 0.4f,
                    padding: new RectOffset(0, 0, 0, 0),
                    margin: new RectOffset(10, 10, 0, 0));
                #endregion

                #region 按钮 - 合适范围
                if (XGUI.layout_button(
                    text: "合适范围",
                    tooltip: "",
                    bg_fill: XGUIFilled.无,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: Color.white,
                    button_text_color: Color.white * 0.9f,
                    press_fill: XGUIFilled.透明,
                    press_color: XGUIColor.无,
                    press_text_color: XHud_Dashboard.Theme_Primary,
                    font_size: XGUIFontSize.M,
                    anchor: TextAnchor.MiddleCenter,
                    margin: new RectOffset(0, 0, 0, 0),
                    padding: new RectOffset(0, 0, 3, 0),
                    layout_min_width: 0,
                    layout_width: 120, button_text_font: XGUI.GetFont("xg-regular")))
                {
                    FitToContent();
                }
                #endregion

                #region 分割线
                XGUI.layout_seperator(
                    thickness: 1,
                    dir: XGUISeplineDir.垂直,
                    color: Color.black * 0.4f,
                padding: new RectOffset(0, 0, 0, 0),
                    margin: new RectOffset(10, 10, 0, 0));
                #endregion

                #region 开关 - 轨道吸附
                bool newSnap = XGUI.layout_toggle(
                    title: "轨道吸附",
                    tooltip: "拖拽时按住 Shift 可临时强制吸附",
                    title_size: XGUIFontSize.M,
                    title_font_style: FontStyle.Normal,
                    title_padding: new RectOffset(0, 10, 0, 2),
                    title_width: 60,
                    prop: snapEnabled,
                    tog_style: XGUIToggleStyle.实体,
                    tog_padding: new RectOffset(5, 8, 3, 0),
                    tog_margin: new RectOffset(0, 0, 0, 0),
                    tog_mixed_options: new string[] { "禁用", "启用" },
                    tog_mixed_text_size: XGUIFontSize.M,
                    tog_mixed_text_color: Color.black,
                    tog_mixed_text_padding: new RectOffset(10, 10, 0, 0),
                    tog_mixed_text_anchor: TextAnchor.MiddleCenter,
                    tog_mixed_font_style: FontStyle.Normal,
                    tog_bg_off_color: Color.white * 0.65f,
                    tog_bg_on_color: XHud_Dashboard.Theme_Primary,
                    tog_handler_off_color: Color.white,
                    tog_handler_on_color: Color.white,
                    tog_mixed_bg_color_gui: XHud_Dashboard.Theme_Primary,
                    act_on_changed: null);
                if (newSnap != snapEnabled)
                {
                    snapEnabled = newSnap;
                    SaveViewState();
                }
                #endregion
            }

            #region 分割线
            XGUI.layout_seperator(
                thickness: 1,
                dir: XGUISeplineDir.垂直,
                color: Color.black * 0.4f,
                padding: new RectOffset(0, 0, 0, 0),
                margin: new RectOffset(10, 10, 0, 0));
            #endregion

            #region 自适应间距
            XGUI.layout_flexspace();
            #endregion

            #region 开发者标识
            XGUI.layout_label(
                bg_fill: XGUIFilled.实体,
                bg_color: XGUIColor.亮白,
                bg_color_gui: Color.white * 0.85f,
                text: "SevenStrike Media",
                size: XGUIFontSize.S,
                text_color: Color.black,
                padding: new RectOffset(15, 15, 0, 0),
                margin: new RectOffset(0, 0, 4, 0),
                offset: new Vector2(0, -2.5f),
                clipping: TextClipping.Clip,
                font_style: FontStyle.Normal,
                anchor: TextAnchor.MiddleRight,
                font: XGUI.GetFont("xg-medium"));
            #endregion

            #region 按钮 - 帮助
            if (XGUI.layout_button(
                tooltip: "帮助",
                tex_release: icon_help_r,
                tex_press: icon_help_p,
                tex_gui_color: Color.white,
                margin: new RectOffset(15, 10, 0, 0),
                border: new RectOffset(0, 0, 0, 0),
                width: TopBarHeight,
                height: TopBarHeight))
            {

            }
            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
        }
        #endregion

        #region 绘制：名字列
        /// <summary> 
        ///绘制左侧名字列（固定面板，垂直滚动由 Clip 区主导）
        ///</summary>
        /// <param name="area">名字列在窗口坐标系中的矩形区域</param>
        private void DrawNameColumn(Rect area)
        {
            GUI.BeginGroup(area);
            XGUI.gui_box(new Rect(0, 0, area.width, area.height), ColorBasedBg);
            Rect rect_name = new Rect(0, 0, area.width, rulerHeight);
            XGUI.gui_box(rect_name, ColorBasedBg);
            Rect scrollViewportRect = new Rect(
                0,
                rulerHeight,
                area.width,
                area.height - rulerHeight - HorizontalScrollbarHeight);
            float contentWidth = scrollViewportRect.width;
            float contentHeight = Nodes.Count * trackHeight + 20f;
            nameScroll = new Vector2(0f, scrollPos.y);
            nameScroll = GUI.BeginScrollView(
                scrollViewportRect,
                nameScroll,
                new Rect(0, 0, contentWidth, contentHeight),
                false,
                false,
                GUIStyle.none,
                GUIStyle.none);
            bool nameHitThisFrame = false;
            int pendingInsertIndex = -1;
            int pendingDeleteIndex = -1;
            for (int i = 0; i < Nodes.Count; i++)
            {
                TweenNode node = Nodes[i];
                bool isSelected = selectedIndices.Contains(i);
                Rect rowRect = GetNameRowHitRect(i, scrollViewportRect.width);
                Rect nameRect = GetNameRowVisualRect(rowRect);

                EditorGUI.DrawRect(nameRect, isSelected ? ColorNameRowSelectedBg : ColorNameRowBg);

                float btnY = nameRect.y + (nameRect.height - NameRowButtonSize) * 0.5f - 1;
                float dis = 3;

                Rect menuRect = new Rect(
                 nameRect.xMax - NameRowButtonSize - 5,
                 btnY,
                 NameRowButtonSize,
                 NameRowButtonSize);
                Rect eyeRect = new Rect(
                   menuRect.x - NameRowButtonSize - dis,
                   btnY,
                   NameRowButtonSize,
                   NameRowButtonSize);
                Rect deleteRect = new Rect(
                    eyeRect.x - NameRowButtonSize - dis,
                    btnY,
                    NameRowButtonSize,
                    NameRowButtonSize);
                Rect insertRect = new Rect(
                    deleteRect.x - NameRowButtonSize - dis,
                    btnY,
                    NameRowButtonSize,
                    NameRowButtonSize);

                // 分段式
                //Rect sepRect = new Rect(
                //  insertRect.x - dis - 2, btnY + 10, 1, NameRowButtonSize - 20);

                // 贯穿式
                Rect sepRect = new Rect(
               insertRect.x - dis - 2, btnY, 1, NameRowButtonSize + 2);

                float buttonZoneLeft = insertRect.x - 35;
                XGUI.gui_label(
                    rect: new Rect(nameRect.x + 25, nameRect.y, buttonZoneLeft - nameRect.x, nameRect.height),
                    text: new GUIContent(node.Indicator),
                    text_color: Color.white * 0.9f,
                    size: XGUIFontSize.B,
                    clipping: XGUI.TryEllipsisClipping(),
                    anchor: TextAnchor.MiddleLeft,
                    font_style: FontStyle.Normal);

                XGUI.gui_box(sepRect, Color.white * 0.35f);

                if (XGUI.gui_button(
                    rect: insertRect,
                    tooltip: "",
                    tex_release: icon_add_r,
                    tex_press: icon_add_p,
                    tex_gui_color: Color.white,
                    width: NameRowButtonSize,
                    height: NameRowButtonSize,
                    border: new RectOffset(0, 0, 0, 0),
                    margin: new RectOffset(0, 0, 0, 0),
                    padding: new RectOffset(0, 0, 0, 0)))
                {
                    pendingInsertIndex = i;
                    nameHitThisFrame = true;
                }

                if (XGUI.gui_button(
                    rect: deleteRect,
                    tooltip: "",
                    tex_release: icon_del_r,
                    tex_press: icon_del_p,
                    tex_gui_color: new Color(0.8f, 0.32f, 0.32f, 1),
                    width: NameRowButtonSize,
                    height: NameRowButtonSize,
                    border: new RectOffset(0, 0, 0, 0),
                    margin: new RectOffset(0, 0, 0, 0),
                    padding: new RectOffset(0, 0, 0, 0)))
                {
                    pendingDeleteIndex = i;
                    nameHitThisFrame = true;
                }

                if (XGUI.gui_button(
                    rect: eyeRect,
                    tooltip: "",
                    tex_release: node.Enabled ? icon_enabled_r : icon_disabled_r,
                    tex_press: node.Enabled ? icon_enabled_p : icon_disabled_p,
                    tex_gui_color: node.Enabled ? Color.white : Color.gray * 0.7f,
                    width: NameRowButtonSize,
                    height: NameRowButtonSize,
                    border: new RectOffset(0, 0, 0, 0),
                    margin: new RectOffset(0, 0, 0, 0),
                    padding: new RectOffset(0, 0, 0, 0)))
                {
                    node.Enabled = !node.Enabled;
                    nameHitThisFrame = true;
                }

                if (XGUI.gui_button(
                    rect: menuRect,
                    tooltip: "",
                    tex_release: icon_menu_r,
                    tex_press: icon_menu_p,
                    tex_gui_color: Color.white,
                    width: NameRowButtonSize,
                    height: NameRowButtonSize,
                    border: new RectOffset(0, 0, 0, 0),
                    margin: new RectOffset(0, 0, 0, 0),
                    padding: new RectOffset(0, 0, 0, 0)))
                {
                    nameHitThisFrame = true;
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("A"), false, () =>
                    {
                    });
                    menu.AddItem(new GUIContent("B"), false, () =>
                    {
                    });
                    menu.AddItem(new GUIContent("C"), false, () =>
                    {
                    });
                    menu.ShowAsContext();
                }

                XGUI.gui_icon(
                   rect: new Rect(nameRect.x + 10, nameRect.y + ((nameRect.height / 2) - TweenTypeDotSize / 2), TweenTypeDotSize, TweenTypeDotSize),
                   icon: icon_led,
                   color: GetClipColor(node.Type));

                if (Event.current.type == EventType.MouseDown
                    && Event.current.button == 0
                    && rowRect.Contains(Event.current.mousePosition)
                    && !nameHitThisFrame)
                {
                    bool ctrl = Event.current.control || Event.current.command;
                    bool shift = Event.current.shift;
                    SetSelection(i, ctrl, shift);
                    nameHitThisFrame = true;
                    Event.current.Use();
                    Repaint();
                }
            }
            GUI.EndScrollView();
            nameScroll.y = scrollPos.y;
            Rect bottomStrip = new Rect(
                0,
                area.height - HorizontalScrollbarHeight,
                area.width,
                HorizontalScrollbarHeight);
            EditorGUI.DrawRect(bottomStrip, ColorBasedBg);
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0
                && !Event.current.alt && !nameHitThisFrame
                && scrollViewportRect.Contains(Event.current.mousePosition))
            {
                selectedIndices.Clear();
                selectedIndex = -1;
                GUIUtility.keyboardControl = 0;
                Event.current.Use();
                Repaint();
            }
            XGUI.gui_box(rect_name, Color_Name_Header_BG);
            XGUI.gui_label(
                rect: new Rect(rect_name.x + 8, rect_name.y, rect_name.width, rect_name.height),
                text: new GUIContent("图元动画列表"),
                text_color: Color.white,
                size: XGUIFontSize.M,
                clipping: TextClipping.Clip,
                anchor: TextAnchor.MiddleLeft,
                font_style: FontStyle.Normal);
            GUI.EndGroup();
            XGUI.gui_box(new Rect(area.x + (area.width - 1), area.y, 1, area.height), Color.black * 0.35f);
            if (pendingDeleteIndex >= 0)
            {
                DeleteNodeAt(pendingDeleteIndex);
            }
            else if (pendingInsertIndex >= 0)
            {
                InsertNodeAfter(pendingInsertIndex);
            }
        }
        /// <summary> 
        ///计算第 <paramref name="index"/> 条名字行的命中矩形（内容坐标）
        ///</summary>
        /// <param name="index">节点索引</param>
        /// <param name="width">名字列可用宽度</param>
        /// <returns>行命中矩形（内容坐标）</returns>
        private Rect GetNameRowHitRect(int index, float width)
        {
            float y0 = Mathf.Round(index * trackHeight);
            float y1 = Mathf.Round((index + 1) * trackHeight);
            return new Rect(0, y0, width, y1 - y0);
        }
        /// <summary> 
        ///由行命中矩形计算名字行的视觉矩形：上下按 <see cref="RowVerticalPadding"/> 内缩
        ///</summary>
        /// <param name="rowRect">行命中矩形</param>
        /// <returns>行视觉矩形</returns>
        private Rect GetNameRowVisualRect(Rect rowRect)
        {
            float pad = RowVerticalPadding;
            return new Rect(
                rowRect.x,
                rowRect.y + pad,
                rowRect.width,
                Mathf.Max(rowRect.height - pad * 2f, 4f));
        }
        #endregion

        #region 绘制：Clip 区
        /// <summary> 
        ///绘制 Clip 区：刻度尺（固定）+ 双向 ScrollView（轨道 + Clip）+ 自绘水平滚动条
        ///</summary>
        /// <param name="area">Clip 区在窗口坐标系中的矩形区域</param>
        private void DrawClipArea(Rect area)
        {
            GUI.BeginGroup(area);
            EditorGUI.DrawRect(new Rect(0, 0, area.width, area.height), ColorBasedBg);

            Rect rulerRect = new Rect(0, 0, area.width, rulerHeight);
            float startSecond = scrollPos.x / pixelsPerSecond;
            float endSecond = (scrollPos.x + area.width) / pixelsPerSecond;
            DrawRuler(rulerRect, startSecond, endSecond);

            Rect scrollViewportRect = new Rect(
                0, rulerHeight, area.width,
                area.height - rulerHeight - HorizontalScrollbarHeight);

            float contentWidth = GetContentWidthPixels();
            float contentHeight = Nodes.Count * trackHeight + 20f;
            Rect contentRect = new Rect(0, 0, contentWidth, contentHeight);

            scrollPos = GUI.BeginScrollView(
                scrollViewportRect,
                scrollPos,
                contentRect,
                false,
                false,
                GUIStyle.none,
                GUI.skin.verticalScrollbar);

            clipHitThisFrame = false;
            for (int i = 0; i < Nodes.Count; i++)
            {
                float trackTop = Mathf.Round(i * trackHeight);
                float trackBottom = Mathf.Round((i + 1) * trackHeight);
                float trackH = trackBottom - trackTop;
                float viewTop = scrollPos.y;
                float viewBottom = scrollPos.y + scrollViewportRect.height;
                if (trackBottom < viewTop || trackTop > viewBottom) continue;
                Rect trackRect = new Rect(0, trackTop, contentWidth, trackH);
                DrawTrack(trackRect, i, startSecond, endSecond);
            }
            ProcessClipDrag(scrollViewportRect);
            GUI.EndScrollView();

            scrollPos.y = Mathf.Clamp(scrollPos.y, 0f, GetMaxScrollY());
            nameScroll.y = scrollPos.y;

            #region 参考线（覆盖刻度尺 + 轨道区）
            if (dragMode != DragMode.None && snapGuideSecond >= 0f)
            {
                float gx = snapGuideSecond * pixelsPerSecond - scrollPos.x;
                if (gx >= 0f && gx <= area.width)
                {
                    Color guideColor = Event.current.shift
                        ? Color.white
                        : Color.white * 0.7f;
                    guideColor.a = 1f;
                    XGUI.gui_box(new Rect(gx, 0, 1f, area.height), guideColor);
                }
            }
            if (dragMode == DragMode.Move && draggingIndex >= 0 && draggingIndex < Nodes.Count)
            {
                TweenNode draggingNode = Nodes[draggingIndex];
                float leftX = draggingNode.Delay * pixelsPerSecond - scrollPos.x;
                float rightX = (draggingNode.Delay + draggingNode.Duration) * pixelsPerSecond - scrollPos.x;
                if (leftX >= 0f && leftX <= area.width)
                    XGUI.gui_box(new Rect(leftX, 0, 1f, area.height), ColorClipEdgeGuide);
                if (rightX >= 0f && rightX <= area.width)
                    XGUI.gui_box(new Rect(rightX, 0, 1f, area.height), ColorClipEdgeGuide);
            }
            #endregion

            Rect hScrollRect = new Rect(0, area.height - HorizontalScrollbarHeight, area.width, HorizontalScrollbarHeight);
            DrawHorizontalScrollbar(hScrollRect);

            HandleRulerClick(new Rect(0, 0, area.width, rulerHeight));
            HandleEmptyClick(scrollViewportRect);
            HandlePan(new Rect(0, 0, area.width, area.height));

            GUI.EndGroup();
        }
        /// <summary> 
        ///计算 Clip 区内容矩形的宽度（像素）
        ///</summary>
        /// <returns>内容宽度（像素）</returns>
        private float GetContentWidthPixels()
        {
            float maxEnd = 0f;
            if (Nodes != null)
            {
                for (int i = 0; i < Nodes.Count; i++)
                {
                    float end = Nodes[i].Delay + Nodes[i].Duration;
                    if (end > maxEnd) maxEnd = end;
                }
            }
            float seconds = Mathf.Max(maxEnd, MinContentSeconds);
            float width = seconds * pixelsPerSecond + ContentRightPaddingPixels;
            float viewportWidth = cachedClipAreaRect.width;
            const float verticalScrollbarWidth = 16f;
            float minWidth = Mathf.Max(1f, viewportWidth - verticalScrollbarWidth);
            return Mathf.Max(width, minWidth);
        }
        /// <summary> 
        ///自绘 Clip 区底部的水平滚动条
        ///</summary>
        /// <param name="rect">水平滚动条矩形（Clip 区局部坐标，位于 Clip 区最底部）</param>
        private void DrawHorizontalScrollbar(Rect rect)
        {
            float contentWidth = GetContentWidthPixels();
            float viewWidth = cachedClipAreaRect.width;
            if (contentWidth <= viewWidth)
            {
                EditorGUI.DrawRect(rect, new Color(0.15f, 0.15f, 0.15f));
                return;
            }
            EditorGUI.DrawRect(rect, new Color(0.15f, 0.15f, 0.15f));
            scrollPos.x = GUI.HorizontalScrollbar(
                rect,
                scrollPos.x,
                viewWidth,
                0f,
                contentWidth);
            scrollPos.x = Mathf.Max(0f, scrollPos.x);
        }
        #endregion

        #region 绘制：刻度尺
        /// <summary> 
        ///绘制顶部刻度尺
        ///</summary>
        /// <param name="rect">刻度尺矩形（Clip 区局部坐标）</param>
        /// <param name="startSecond">可见区左边缘对应的时间（秒）</param>
        /// <param name="endSecond">可见区右边缘对应的时间（秒）</param>
        private void DrawRuler(Rect rect, float startSecond, float endSecond)
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
        #endregion

        #region 绘制：轨道与 Clip
        /// <summary> 
        ///绘制单条轨道及其 Clip，并进行 MouseDown 命中检测以进入拖拽模式
        ///</summary>
        /// <param name="trackRect">轨道矩形（内容坐标，高 = trackHeight）</param>
        /// <param name="index">动画节点索引</param>
        /// <param name="startSecond">可见区左边缘对应的时间（秒）当前未使用，保留供扩展</param>
        /// <param name="endSecond">可见区右边缘对应的时间（秒）当前未使用，保留供扩展</param>
        private void DrawTrack(Rect trackRect, int index, float startSecond, float endSecond)
        {
            TweenNode node = Nodes[index];
            bool isSelected = selectedIndices.Contains(index);
            Rect trackVisualRect = GetTrackVisualRect(trackRect);
            XGUI.gui_box(trackVisualRect, ColorClipBG);
            Rect clipRect = GetClipRect(trackVisualRect, node);
            float viewLeft = scrollPos.x;
            float viewRight = scrollPos.x + cachedClipAreaRect.width;
            if (clipRect.xMax < viewLeft - 10 || clipRect.x > viewRight + 10)
                return;
            DrawClipVisual(clipRect, node, isSelected);
            TryBeginClipDrag(clipRect, index);
        }
        /// <summary> 
        ///由轨道命中矩形计算轨道视觉矩形：上下按固定像素 <see cref="RowVerticalPadding"/> 内缩
        ///</summary>
        /// <param name="trackRect">轨道命中矩形（高 = trackHeight）</param>
        /// <returns>轨道视觉矩形（已内缩）</returns>
        private Rect GetTrackVisualRect(Rect trackRect)
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
        ///</summary>
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
            Color clip_type_Color = GetClipColor(node.Type);
            if (!node.Enabled)
                clip_type_Color *= 0.55f;
            if (isSelected)
                clip_type_Color = Color.Lerp(clip_type_Color, Color.white, 0.25f);
            if (!isSelected)
                XGUI.gui_box(new Rect(clipRect.x, clipRect.y + clipRect.height - ClipTypeLineThickness, clipRect.width, ClipTypeLineThickness), clip_type_Color);
            #endregion

            #region 概述信息
            bool is_minmal = clipRect.width < 150;
            string twn_content = $"{(is_minmal ? "" : "延迟：")}{node.Delay:F3}   /   {(is_minmal ? "" : "耗时：")}{node.Duration:F3}";
            XGUI.gui_label(
                rect: new Rect(clipRect.x + 4, clipRect.y, clipRect.width - 8, clipRect.height),
                text: new GUIContent(twn_content),
                text_color: isSelected ? (!node.Enabled ? Color.white * 0.7f : Color.black) : Color.white,
                size: XGUIFontSize.M,
                clipping: TextClipping.Clip,
                anchor: TextAnchor.MiddleCenter,
                font_style: FontStyle.Normal);
            #endregion

            #region 类型图标
            if (clipRect.width >= 220)
            {
                XGUI.gui_icon(
                rect: new Rect(clipRect.x + 15, clipRect.y + (trackHeight / 2 - 5), 10, 10),
                icon: GetTweenTypeIcon(node.Type),
                color: isSelected ? Color.black : Color.white * 0.9f);
            }
            #endregion

            DrawClipCursors(clipRect);
        }
        /// <summary> 
        ///根据轨道矩形与节点数据计算 Clip 的内容坐标矩形
        ///</summary>
        /// <param name="trackVisualRect">轨道视觉矩形（内容坐标）</param>
        /// <param name="node">动画节点</param>
        /// <returns>Clip 矩形（内容坐标）</returns>
        private Rect GetClipRect(Rect trackVisualRect, TweenNode node)
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
        ///</summary>
        /// <param name="clipRect">Clip 矩形（内容坐标）</param>
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
        ///根据动画类型获取 Clip 的显示颜色
        ///</summary>
        /// <param name="type">动画节点类型</param>
        /// <returns>该类型对应的颜色</returns>
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
        /// <summary>
        /// 根据类型枚举值来获取动画图标
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private Texture2D GetTweenTypeIcon(TweenNodeType type)
        {
            Texture2D typeicon = null;
            switch (type)
            {
                case TweenNodeType.位移:
                    typeicon = icon_type_move;
                    break;
                case TweenNodeType.旋转:
                    typeicon = icon_type_rotator;
                    break;
                case TweenNodeType.缩放:
                    typeicon = icon_type_scale;
                    break;
                case TweenNodeType.颜色:
                    typeicon = icon_type_color;
                    break;
                case TweenNodeType.淡化:
                    typeicon = icon_type_fade;
                    break;
                case TweenNodeType.打字机:
                    typeicon = icon_type_writter;
                    break;
                case TweenNodeType.图像填充:
                    typeicon = icon_type_fill;
                    break;
                case TweenNodeType.尺寸:
                    typeicon = icon_type_size;
                    break;
            }

            return typeicon;
        }
        #endregion

        #region 绘制：参数面板
        /// <summary> 
        ///绘制右侧节点参数面板，用于编辑当前选中动画节点的属性
        ///<para/>内部使用 GUILayout 布局，高度由 Unity 自动计算，无需再手工估算
        ///</summary>
        /// <param name="area">参数面板在窗口坐标系中的矩形区域</param>
        private void DrawParameterPanel(Rect area)
        {
            GUI.BeginGroup(area);
            XGUI.gui_box(new Rect(0, 0, area.width, area.height), ColorBasedBg);

            Rect rulerParamRect = new Rect(0, 0, area.width, rulerHeight);
            Rect scrollViewportRect = new Rect(0, rulerHeight, area.width, area.height - rulerHeight);

            // 用 GUILayout.BeginScrollView，让内部控件走自动布局
            paramScroll = GUILayout.BeginScrollView(
                paramScroll,
                false,
                false,
                GUIStyle.none,
                GUI.skin.verticalScrollbar,
                GUILayout.Width(scrollViewportRect.width),
                GUILayout.Height(scrollViewportRect.height));

            if (selectedIndex >= 0 && selectedIndex < Nodes.Count)
            {
                DrawParamFields();
            }
            else
            {
                GUILayout.Space(6);
                GUILayout.Label("点击左侧轨道或 Clip 以编辑参数。", EditorStyles.miniLabel);
            }

            GUILayout.EndScrollView();

            // 点击面板空白处清除键盘焦点（注意：GUILayout 里取鼠标位置仍可用 Event.current）
            if (Event.current.type == EventType.MouseDown
                && Event.current.button == 0
                && !Event.current.alt
                && scrollViewportRect.Contains(Event.current.mousePosition))
            {
                GUIUtility.keyboardControl = 0;
                Event.current.Use();
                Repaint();
            }

            XGUI.gui_box(rulerParamRect, Color_Params_Header_BG);
            XGUI.gui_label(
                rect: new Rect(rulerParamRect.x + 6, rulerParamRect.y, rulerParamRect.width, rulerParamRect.height),
                text: new GUIContent("节点参数"),
                text_color: Color.white,
                size: XGUIFontSize.M,
                clipping: TextClipping.Clip,
                anchor: TextAnchor.MiddleLeft,
                font_style: FontStyle.Normal);

            GUI.EndGroup();
            XGUI.gui_box(new Rect(area.x, area.y, 1, area.height), Color.black * 0.35f);
        }
        /// <summary> 
        ///绘制选中节点的参数字段
        ///<para/>使用 EditorGUILayout 自动布局；按 <see cref="TweenNode.Type"/> 动态显示起始值 / 结束值
        ///</summary>
        private void DrawParamFields()
        {
            TweenNode node = Nodes[selectedIndex];

            XGUI.ChangedCheck_Start();

            #region 基础
            XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.Theme_Group,
                title: "基础",
                title_text_color: XHud_Dashboard.Theme_Primary,
                title_clipping: XGUI.TryEllipsisClipping(),
                title_size: XGUIFontSize.M,
                title_manual_offset: true,
                title_manual_offset_space: 5,
                absolute_margin: true,
                absolute_padding: true,
                margin: new RectOffset(10, 10, 25, 0),
                padding: new RectOffset(10, 10, 15, 15));

            SerializedObject so = new SerializedObject(target);
            SerializedProperty ser_indicator = so.FindProperty("PrimitiveTweenNodes").GetArrayElementAtIndex(selectedIndex).FindPropertyRelative("Indicator");
            SerializedProperty ser_type = so.FindProperty("PrimitiveTweenNodes").GetArrayElementAtIndex(selectedIndex).FindPropertyRelative("Type");
            SerializedProperty ser_duration = so.FindProperty("PrimitiveTweenNodes").GetArrayElementAtIndex(selectedIndex).FindPropertyRelative("Duration");
            SerializedProperty ser_delay = so.FindProperty("PrimitiveTweenNodes").GetArrayElementAtIndex(selectedIndex).FindPropertyRelative("Delay");
            SerializedProperty ser_timing = so.FindProperty("PrimitiveTweenNodes").GetArrayElementAtIndex(selectedIndex).FindPropertyRelative("Timings");
            SerializedProperty ser_loop_type = so.FindProperty("PrimitiveTweenNodes").GetArrayElementAtIndex(selectedIndex).FindPropertyRelative("LoopType");
            SerializedProperty ser_loop_count = so.FindProperty("PrimitiveTweenNodes").GetArrayElementAtIndex(selectedIndex).FindPropertyRelative("LoopCount");
            SerializedProperty ser_ease = so.FindProperty("PrimitiveTweenNodes").GetArrayElementAtIndex(selectedIndex).FindPropertyRelative("Ease");
            SerializedProperty ser_curve = so.FindProperty("PrimitiveTweenNodes").GetArrayElementAtIndex(selectedIndex).FindPropertyRelative("Curve");
            SerializedProperty ser_rotate_mode = so.FindProperty("PrimitiveTweenNodes").GetArrayElementAtIndex(selectedIndex).FindPropertyRelative("RotateMode");

            so.Update();

            #region 标识
            XGUI.layout_property_field(
              title: "标识",
              title_size: XGUIFontSize.M,
              title_hover_color: XHud_Dashboard.Theme_Primary,
              title_width: 80,
              prop: ser_indicator,
              prop_margin: new RectOffset(0, 0, 5, 0));
            ser_indicator.serializedObject.ApplyModifiedProperties();
            #endregion

            #region 时机
            XGUI.layout_property_field(
                title: "时机",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.Theme_Primary,
                title_width: 80,
                prop: ser_timing,
                prop_margin: new RectOffset(0, 0, 5, 0));
            ser_timing.serializedObject.ApplyModifiedProperties();
            #endregion

            #region 类型
            XGUI.layout_property_field(
               title: "类型",
               title_size: XGUIFontSize.M,
               title_hover_color: XHud_Dashboard.Theme_Primary,
               title_width: 80,
               prop: ser_type,
               prop_margin: new RectOffset(0, 0, 5, 0));
            ser_type.serializedObject.ApplyModifiedProperties();
            #endregion

            #region 耗时
            XGUI.layout_property_field(
                title: "耗时",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.Theme_Primary,
                title_width: 80,
                prop: ser_duration,
                prop_margin: new RectOffset(0, 0, 5, 0));
            ser_duration.serializedObject.ApplyModifiedProperties();
            #endregion

            #region 延迟
            XGUI.layout_property_field(
                title: "延迟",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.Theme_Primary,
                title_width: 80,
                prop: ser_delay,
                prop_margin: new RectOffset(0, 0, 5, 0));
            ser_delay.serializedObject.ApplyModifiedProperties();
            #endregion

            #region 缓动
            XGUI.layout_property_field(
                title: "缓动",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.Theme_Primary,
                title_width: 80,
                prop: ser_ease,
                prop_margin: new RectOffset(0, 0, 5, 0));
            ser_ease.serializedObject.ApplyModifiedProperties();
            #endregion

            #region 旋转模式
            if (node.Type == TweenNodeType.旋转)
            {
                XGUI.layout_property_field(
                    title: "旋转模式",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 80,
                    prop: ser_rotate_mode,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                ser_rotate_mode.serializedObject.ApplyModifiedProperties();
            }
            #endregion

            #region 循环  
            XGUI.layout_property_field(
                title: "循环",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.Theme_Primary,
                title_width: 80,
                prop: ser_loop_type,
                prop_margin: new RectOffset(0, 0, 5, 0));
            ser_loop_type.serializedObject.ApplyModifiedProperties();
            #endregion

            #region 循环次数
            XGUI.layout_property_field(
                title: "循环次数",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.Theme_Primary,
                title_width: 80,
                prop: ser_loop_count,
                prop_margin: new RectOffset(0, 0, 5, 0));
            ser_loop_type.serializedObject.ApplyModifiedProperties();
            #endregion

            #region 曲线
            if (node.Ease == EaseMode.None)
            {
                XGUI.layout_property_field(
                    title: "曲线",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 80,
                    prop: ser_curve,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                ser_curve.serializedObject.ApplyModifiedProperties();
            }
            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 选项
            XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.Theme_Group,
                title: "选项",
                title_text_color: XHud_Dashboard.Theme_Primary,
                title_clipping: XGUI.TryEllipsisClipping(),
                title_size: XGUIFontSize.M,
                title_manual_offset: true,
                title_manual_offset_space: 5,
                absolute_margin: true,
                absolute_padding: true,
                margin: new RectOffset(10, 10, 25, 0),
                padding: new RectOffset(10, 10, 15, 15));

            #region 动画开关
            node.Enabled = DrawToggle("动画开关", node.Enabled, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, new string[] { "禁用", "启用" }, (b) => { });
            #endregion

            #region 重置设为起始值
            node.Rewind_Set_Startvalue = DrawToggle("重置设为起始值", node.Rewind_Set_Startvalue, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, new string[] { "禁用", "启用" }, (b) => { });
            #endregion

            #region 完成设为结束值
            node.Complete_Set_Endvalue = DrawToggle("完成设为结束值", node.Complete_Set_Endvalue, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, new string[] { "禁用", "启用" }, (b) => { });
            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            EditorGUILayout.Space(6);

            #region 起始值 / 结束值（按类型动态显示）
            DrawTypeSpecificFields(node);
            #endregion

            if (XGUI.ChangedCheck_End())
            {
                if (dragMode == DragMode.None)
                {
                    Undo.RecordObject(target, "Edit Tween Node");
                }
                EditorUtility.SetDirty(target);
                Repaint();
            }

            so.ApplyModifiedProperties();
        }
        /// <summary> 
        ///按 <see cref="TweenNode.Type"/> 动态绘制起始值 / 结束值字段
        ///</summary>
        private void DrawTypeSpecificFields(TweenNode node)
        {
            // 没有任何一端激活时不显示数值字段
            if (!node.ActivateFrom && !node.ActivateEnd && !node.ActivateOnlyToEnd)
                return;

            EditorGUILayout.LabelField("数值", EditorStyles.boldLabel);

            switch (node.Type)
            {
                case TweenNodeType.位移:
                    DrawVec3Row("起始", ref node.ActivateFrom, ref node.From_Vector3);
                    DrawVec3Row("结束", ref node.ActivateEnd, ref node.End_Vector3);
                    break;

                case TweenNodeType.旋转:
                    DrawVec3Row("起始", ref node.ActivateFrom, ref node.From_Vector3);
                    DrawVec3Row("结束", ref node.ActivateEnd, ref node.End_Vector3);
                    break;

                case TweenNodeType.缩放:
                    DrawVec3Row("起始", ref node.ActivateFrom, ref node.From_Vector3);
                    DrawVec3Row("结束", ref node.ActivateEnd, ref node.End_Vector3);
                    break;

                case TweenNodeType.尺寸:
                    DrawVec2Row("起始", ref node.ActivateFrom, ref node.From_Vector2);
                    DrawVec2Row("结束", ref node.ActivateEnd, ref node.End_Vector2);
                    break;

                case TweenNodeType.颜色:
                    DrawColorRow("起始", ref node.ActivateFrom, ref node.From_Color);
                    DrawColorRow("结束", ref node.ActivateEnd, ref node.End_Color);
                    break;

                case TweenNodeType.淡化:
                    DrawFloatRow("起始", ref node.ActivateFrom, ref node.From_Float);
                    DrawFloatRow("结束", ref node.ActivateEnd, ref node.End_Float);
                    break;

                case TweenNodeType.打字机:
                    DrawStringRow("起始", ref node.ActivateFrom, ref node.From_String);
                    DrawStringRow("结束", ref node.ActivateEnd, ref node.End_String);
                    break;

                case TweenNodeType.图像填充:
                    DrawFloatRow("起始", ref node.ActivateFrom, ref node.From_Float);
                    DrawFloatRow("结束", ref node.ActivateEnd, ref node.End_Float);
                    break;

                default:
                    DrawFloatRow("起始", ref node.ActivateFrom, ref node.From_Float);
                    DrawFloatRow("结束", ref node.ActivateEnd, ref node.End_Float);
                    break;
            }
        }

        // ---- 行级助手：勾选 + 数值 ----

        private void DrawVec2Row(string label, ref bool activate, ref Vector2 value)
        {
            EditorGUILayout.BeginHorizontal();
            activate = EditorGUILayout.Toggle(activate, GUILayout.Width(16));
            value = EditorGUILayout.Vector2Field(label, value);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawVec3Row(string label, ref bool activate, ref Vector3 value)
        {
            EditorGUILayout.BeginHorizontal();
            activate = EditorGUILayout.Toggle(activate, GUILayout.Width(16));
            value = EditorGUILayout.Vector3Field(label, value);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawFloatRow(string label, ref bool activate, ref float value)
        {
            EditorGUILayout.BeginHorizontal();
            activate = EditorGUILayout.Toggle(activate, GUILayout.Width(16));
            value = EditorGUILayout.FloatField(label, value);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawStringRow(string label, ref bool activate, ref string value)
        {
            EditorGUILayout.BeginHorizontal();
            activate = EditorGUILayout.Toggle(activate, GUILayout.Width(16));
            value = EditorGUILayout.TextField(label, value);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawColorRow(string label, ref bool activate, ref Color value)
        {
            EditorGUILayout.BeginHorizontal();
            activate = EditorGUILayout.Toggle(activate, GUILayout.Width(16));
            value = EditorGUILayout.ColorField(label, value);
            EditorGUILayout.EndHorizontal();
        }

        #region Draw 开关选项
        /// <summary>
        /// 通用方法：绘制开关
        /// </summary>
        private bool DrawToggle(string title, bool prop, float width, XGUIToggleStyle style = XGUIToggleStyle.实体, Color color_bg_on = default, Color color_bg_off = default, Color color_on = default, Color color_off = default, string[] options = null, Action<bool> act_on_changed = null)
        {
            return XGUI.layout_toggle(
                   title: title,
                   title_size: XGUIFontSize.M,
                   title_font_style: FontStyle.Normal,
                   title_padding: new RectOffset(5, 10, 0, 0),
                   title_width: width,
                   prop: prop,
                   tog_style: style,
                   tog_padding: new RectOffset(0, 9, 0, 0),
                   tog_margin: new RectOffset(0, 0, 0, 5),
                   tog_mixed_options: options,
                   tog_mixed_text_size: XGUIFontSize.M,
                   tog_mixed_text_color: Color.black,
                   tog_mixed_text_padding: new RectOffset(10, 10, 0, 0),
                   tog_mixed_text_anchor: TextAnchor.MiddleCenter,
                   tog_mixed_font_style: FontStyle.Normal,
                   tog_bg_off_color: color_bg_off,
                   tog_bg_on_color: color_bg_on,
                   tog_handler_off_color: color_off,
                   tog_handler_on_color: color_on,
                   tog_mixed_bg_color_gui: XHud_Dashboard.Theme_Primary,
                   act_on_changed: act_on_changed);
        }
        #endregion
        #endregion

        #region 绘制：底部工具栏
        /// <summary> 
        ///绘制窗口底部工具栏
        ///</summary>
        /// <param name="area">底部工具栏在窗口坐标系中的矩形</param>
        private void DrawBottomToolbar(Rect area)
        {
            EditorGUI.DrawRect(area, XGUI_Utilitys.HexString_To_Color("1e1e1e"));
            XGUI.gui_box(new Rect(area.x, area.y, area.width, 1), Color.black * 0.4f);
            GUILayout.BeginArea(area);
            XGUI.layout_group_start(
                type: XGUIContainerType.Horizontal,
                bg_fill: XGUIFilled.透明,
                bg_color: XGUIColor.亮白,
                bg_color_gui: Color.white,
                absolute_margin: true,
                absolute_padding: true,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(10, 10, 3, 0));
            XGUI.layout_label(
                text: $"动画轨道：{Nodes.Count}    选中轨道：{selectedIndices.Count}",
                size: XGUIFontSize.M,
                text_color: Color.white * 0.7f,
                margin: new RectOffset(0, 0, 0, 0),
                offset: new Vector2(0, 0),
                clipping: TextClipping.Clip,
                font_style: FontStyle.Normal,
                anchor: TextAnchor.MiddleLeft,
                font: XGUI.GetFont("xg-regular"));
            XGUI.layout_seperator(
                thickness: 1,
                dir: XGUISeplineDir.垂直,
                color: Color.black * 0.4f,
                padding: new RectOffset(0, 0, 5, 0),
                margin: new RectOffset(10, 10, 0, 0));
            XGUI.layout_flexspace();
            if (XGUI.layout_button(
                text: "缩放到全部",
                tooltip: "",
                bg_fill: XGUIFilled.无,
                bg_color: XGUIColor.亮白,
                bg_color_gui: Color.white,
                button_text_color: Color.white * 0.9f,
                press_fill: XGUIFilled.透明,
                press_color: XGUIColor.无,
                press_text_color: XHud_Dashboard.Theme_Primary,
                font_size: XGUIFontSize.M,
                anchor: TextAnchor.MiddleCenter,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                layout_min_width: 0,
                layout_width: 90,
                button_text_font: XGUI.GetFont("xg-regular")))
            {
                FitToContent();
            }
            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            GUILayout.EndArea();
        }
        #endregion

        #region 交互：Clip 拖拽
        /// <summary> 
        ///在 Clip 矩形上检测 MouseDown 命中，命中则进入对应的拖拽模式
        ///</summary>
        /// <param name="clipRect">Clip 矩形（内容坐标）</param>
        /// <param name="index">动画节点索引</param>
        private void TryBeginClipDrag(Rect clipRect, int index)
        {
            Event e = Event.current;
            if (e.type != EventType.MouseDown) return;
            if (e.button != 0) return;
            if (e.alt) return;
            if (dragMode != DragMode.None) return;
            if (!clipRect.Contains(e.mousePosition)) return;
            TweenNode node = Nodes[index];
            bool ctrl = e.control || e.command;
            bool shift = e.shift;
            if (ctrl || shift)
            {
                SetSelection(index, ctrl, shift);
            }
            else if (!selectedIndices.Contains(index))
            {
                SetSelection(index, false, false);
            }
            else
            {
                selectedIndex = index;
                GUIUtility.keyboardControl = 0;
            }
            if (Mathf.Abs(e.mousePosition.x - clipRect.xMin) < ClipEdgeZone)
                dragMode = DragMode.LeftEdge;
            else if (Mathf.Abs(e.mousePosition.x - clipRect.xMax) < ClipEdgeZone)
                dragMode = DragMode.RightEdge;
            else
                dragMode = DragMode.Move;
            if (dragMode == DragMode.Move)
            {
                float clipCenterX = (clipRect.xMin + clipRect.xMax) * 0.5f;
                moveDragAnchorSide = e.mousePosition.x < clipCenterX ? 1 : 2;
            }
            else
            {
                moveDragAnchorSide = 0;
            }
            primaryDragIndex = selectedIndices.Contains(index) ? GetTopmostSelected() : index;
            if (primaryDragIndex < 0) primaryDragIndex = index;
            dragStartDelays.Clear();
            dragStartDurations.Clear();
            foreach (int i in selectedIndices)
            {
                if (i < 0 || i >= Nodes.Count) continue;
                dragStartDelays[i] = QuantizeTime(Nodes[i].Delay);
                dragStartDurations[i] = QuantizeTime(Nodes[i].Duration);
            }
            if (dragStartDelays.Count == 0)
            {
                dragStartDelays[index] = QuantizeTime(node.Delay);
                dragStartDurations[index] = QuantizeTime(node.Duration);
                primaryDragIndex = index;
            }
            dragStartDelay = QuantizeTime(Nodes[primaryDragIndex].Delay);
            dragStartDuration = QuantizeTime(Nodes[primaryDragIndex].Duration);
            draggingIndex = index;
            dragControlID = GUIUtility.GetControlID(FocusType.Passive);
            GUIUtility.hotControl = dragControlID;
            GUIUtility.keyboardControl = 0;
            dragStartContentSecond = e.mousePosition.x / pixelsPerSecond;
            snapGuideSecond = -1f;
            Undo.IncrementCurrentGroup();
            Undo.SetCurrentGroupName("Edit Tween Clip");
            dragUndoGroup = Undo.GetCurrentGroup();
            Undo.RecordObject(target, "Edit Tween Clip");
            clipHitThisFrame = true;
            e.Use();
            Repaint();
        }
        /// <summary> 
        ///统一处理 Clip 拖拽的 MouseDrag / MouseUp
        ///</summary>
        /// <param name="viewportArea">ScrollView 视口矩形（Clip 区局部坐标，用于判断边缘自动平移）</param>
        private void ProcessClipDrag(Rect viewportArea)
        {
            Event e = Event.current;
            if (dragMode == DragMode.None || draggingIndex < 0 || draggingIndex >= Nodes.Count)
            {
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
                        float currentContentSecond = e.mousePosition.x / pixelsPerSecond;
                        float totalDelta = currentContentSecond - dragStartContentSecond;
                        snapGuideSecond = -1f;
                        bool snapOn = this.snapEnabled || e.shift;
                        Undo.RecordObject(target, "Edit Tween Clip");
                        switch (dragMode)
                        {
                            case DragMode.Move:
                                {
                                    TweenNode primary = Nodes[primaryDragIndex];
                                    float rawDelay = Mathf.Max(0f, dragStartDelay + totalDelta);
                                    float rawEnd = QuantizeTime(rawDelay + dragStartDuration);
                                    float snappedPrimaryDelay;
                                    if (snapOn && moveDragAnchorSide != 0)
                                    {
                                        if (moveDragAnchorSide == 1)
                                        {
                                            float snappedStart;
                                            bool leftSnapped = SnapTime(rawDelay, selectedIndices, out snappedStart);
                                            snappedStart = QuantizeTime(Mathf.Max(0f, snappedStart));
                                            if (leftSnapped)
                                            {
                                                snappedPrimaryDelay = snappedStart;
                                                snapGuideSecond = snappedStart;
                                            }
                                            else
                                            {
                                                snappedPrimaryDelay = QuantizeTime(rawDelay);
                                                snapGuideSecond = -1f;
                                            }
                                        }
                                        else
                                        {
                                            float snappedEnd;
                                            bool rightSnapped = SnapTime(rawEnd, selectedIndices, out snappedEnd);
                                            snappedEnd = QuantizeTime(Mathf.Max(0f, snappedEnd));
                                            if (rightSnapped)
                                            {
                                                snappedPrimaryDelay = QuantizeTime(Mathf.Max(0f, snappedEnd - dragStartDuration));
                                                snapGuideSecond = snappedEnd;
                                            }
                                            else
                                            {
                                                snappedPrimaryDelay = QuantizeTime(rawDelay);
                                                snapGuideSecond = -1f;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        snappedPrimaryDelay = QuantizeTime(rawDelay);
                                        snapGuideSecond = -1f;
                                    }
                                    float finalDelta = snappedPrimaryDelay - dragStartDelay;
                                    float minStartDelay = float.MaxValue;
                                    foreach (var kv in dragStartDelays)
                                        if (kv.Value < minStartDelay) minStartDelay = kv.Value;
                                    if (minStartDelay + finalDelta < 0f)
                                        finalDelta = -minStartDelay;
                                    foreach (var kv in dragStartDelays)
                                    {
                                        int idx = kv.Key;
                                        if (idx < 0 || idx >= Nodes.Count) continue;
                                        Nodes[idx].Delay = QuantizeTime(kv.Value + finalDelta);
                                    }
                                    break;
                                }
                            case DragMode.LeftEdge:
                                {
                                    TweenNode primary = Nodes[primaryDragIndex];
                                    float primaryStartDelay = dragStartDelays.ContainsKey(primaryDragIndex)
                                        ? dragStartDelays[primaryDragIndex] : QuantizeTime(primary.Delay);
                                    float rawDelay = Mathf.Max(0f, primaryStartDelay + totalDelta);
                                    float snapped;
                                    if (snapOn)
                                    {
                                        float snapValue;
                                        bool snappedFlag = SnapTime(rawDelay, selectedIndices, out snapValue);
                                        snapped = QuantizeTime(Mathf.Max(0f, snapValue));
                                        snapGuideSecond = snappedFlag ? snapped : -1f;
                                    }
                                    else
                                    {
                                        snapped = QuantizeTime(rawDelay);
                                        snapGuideSecond = -1f;
                                    }
                                    float delta = snapped - primaryStartDelay;
                                    float minDelta = float.MinValue;
                                    float maxDelta = float.MaxValue;
                                    foreach (var kv in dragStartDelays)
                                    {
                                        int idx = kv.Key;
                                        if (idx < 0 || idx >= Nodes.Count) continue;
                                        float startD = kv.Value;
                                        float startDur = dragStartDurations[idx];
                                        float lo = -startD;
                                        float hi = startDur - 0.01f;
                                        if (lo > minDelta) minDelta = lo;
                                        if (hi < maxDelta) maxDelta = hi;
                                    }
                                    delta = Mathf.Clamp(delta, minDelta, maxDelta);
                                    foreach (var kv in dragStartDelays)
                                    {
                                        int idx = kv.Key;
                                        if (idx < 0 || idx >= Nodes.Count) continue;
                                        float startD = kv.Value;
                                        float startDur = dragStartDurations[idx];
                                        float endFixed = QuantizeTime(startD + startDur);
                                        float newDelay = QuantizeTime(startD + delta);
                                        Nodes[idx].Delay = newDelay;
                                        Nodes[idx].Duration = QuantizeTime(Mathf.Max(0.01f, endFixed - newDelay));
                                    }
                                    break;
                                }
                            case DragMode.RightEdge:
                                {
                                    float primaryStartDelay = dragStartDelays.ContainsKey(primaryDragIndex)
                                        ? dragStartDelays[primaryDragIndex] : dragStartDelay;
                                    float primaryStartDur = dragStartDurations.ContainsKey(primaryDragIndex)
                                        ? dragStartDurations[primaryDragIndex] : dragStartDuration;
                                    float primaryStartEnd = QuantizeTime(primaryStartDelay + primaryStartDur);
                                    float rawEnd = primaryStartEnd + totalDelta;
                                    float snapped;
                                    if (snapOn)
                                    {
                                        float snapValue;
                                        bool snappedFlag = SnapTime(rawEnd, selectedIndices, out snapValue);
                                        snapped = QuantizeTime(Mathf.Max(primaryStartDelay + 0.01f, snapValue));
                                        snapGuideSecond = snappedFlag ? snapped : -1f;
                                    }
                                    else
                                    {
                                        snapped = QuantizeTime(rawEnd);
                                        snapGuideSecond = -1f;
                                    }
                                    float delta = snapped - primaryStartEnd;
                                    float minDelta = float.MinValue;
                                    float maxDelta = float.MaxValue;
                                    foreach (var kv in dragStartDelays)
                                    {
                                        int idx = kv.Key;
                                        if (idx < 0 || idx >= Nodes.Count) continue;
                                        float startD = kv.Value;
                                        float startDur = dragStartDurations[idx];
                                        float lo = 0.01f - startDur;
                                        float hi = float.MaxValue;
                                        if (lo > minDelta) minDelta = lo;
                                        if (hi < maxDelta) maxDelta = hi;
                                    }
                                    delta = Mathf.Clamp(delta, minDelta, maxDelta);
                                    foreach (var kv in dragStartDelays)
                                    {
                                        int idx = kv.Key;
                                        if (idx < 0 || idx >= Nodes.Count) continue;
                                        float startD = kv.Value;
                                        float startDur = dragStartDurations[idx];
                                        Nodes[idx].Delay = QuantizeTime(startD);
                                        Nodes[idx].Duration = QuantizeTime(Mathf.Max(0.01f, startDur + delta));
                                    }
                                    break;
                                }
                        }
                        AutoScrollOnDragEdge(e.mousePosition.x - scrollPos.x, new Rect(0, 0, viewportArea.width, viewportArea.height));
                        EditorUtility.SetDirty(target);
                        Repaint();
                        e.Use();
                    }
                    break;
                case EventType.MouseUp:
                    if (dragMode != DragMode.None)
                    {
                        GUIUtility.hotControl = 0;
                        dragMode = DragMode.None;
                        draggingIndex = -1;
                        snapGuideSecond = -1f;
                        primaryDragIndex = -1;
                        moveDragAnchorSide = 0;
                        dragStartDelays.Clear();
                        dragStartDurations.Clear();
                        if (dragUndoGroup >= 0)
                        {
                            Undo.CollapseUndoOperations(dragUndoGroup);
                            dragUndoGroup = -1;
                        }
                        e.Use();
                        Repaint();
                    }
                    break;
            }
        }
        /// <summary> 
        ///拖拽时若鼠标靠近 Clip 区左右边缘，自动平移 <see cref="scrollPos"/>
        ///</summary>
        /// <param name="mouseViewportX">鼠标在 ScrollView 视口坐标系下的 X（已减 scrollPos.x）</param>
        /// <param name="viewportArea">ScrollView 视口矩形（宽高用于判断边缘）</param>
        private void AutoScrollOnDragEdge(float mouseViewportX, Rect viewportArea)
        {
            if (mouseViewportX < AutoScrollEdgeMargin)
            {
                float t = 1f - Mathf.Clamp01(mouseViewportX / AutoScrollEdgeMargin);
                float speed = AutoScrollSpeed * (0.5f + t * 1.5f);
                scrollPos.x = Mathf.Max(0f, scrollPos.x - speed);
            }
            else if (mouseViewportX > viewportArea.width - AutoScrollEdgeMargin)
            {
                float over = mouseViewportX - (viewportArea.width - AutoScrollEdgeMargin);
                float t = Mathf.Clamp01(over / AutoScrollEdgeMargin);
                float speed = AutoScrollSpeed * (0.5f + t * 1.5f);
                scrollPos.x += speed;
            }
        }
        #endregion

        #region 交互：平移视图
        /// <summary> 
        ///处理中键 / Alt+鼠标左键拖拽平移视图（水平 + 垂直）
        ///</summary>
        /// <param name="localArea">Clip 区局部矩形（刻度尺下方），用于判断按下的位置是否在区内</param>
        private void HandlePan(Rect localArea)
        {
            Event e = Event.current;
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            switch (e.type)
            {
                case EventType.MouseDown:
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
                        scrollPos.x -= e.delta.x;
                        scrollPos.x = Mathf.Max(0f, scrollPos.x);
                        float maxScrollY = GetMaxScrollY();
                        scrollPos.y = Mathf.Clamp(scrollPos.y - e.delta.y, 0f, maxScrollY);
                        nameScroll.y = scrollPos.y;
                        e.Use();
                        Repaint();
                    }
                    break;
                case EventType.MouseUp:
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

        #region 交互：滚轮
        /// <summary> 
        ///处理滚轮操作：单独滚轮水平平移 / Alt+滚轮缩放 / Shift+滚轮调整轨道高度 / Ctrl+滚轮垂直滚动
        ///</summary>
        private void HandleScrollWheel()
        {
            Event e = Event.current;
            if (e.type != EventType.ScrollWheel) return;
            if (target == null) return;
            if (!cachedClipAreaRect.Contains(e.mousePosition)
                && !cachedNameAreaRect.Contains(e.mousePosition)) return;
            if (e.alt)
            {
                float mouseLocalX = e.mousePosition.x - cachedClipAreaRect.x;
                float mouseSecond = (scrollPos.x + mouseLocalX) / pixelsPerSecond;
                float zoomFactor = 1f - e.delta.y * 0.03f;
                pixelsPerSecond = Mathf.Clamp(pixelsPerSecond * zoomFactor, 1f, 20000f);
                scrollPos.x = mouseSecond * pixelsPerSecond - mouseLocalX;
                scrollPos.x = Mathf.Max(0f, scrollPos.x);
                e.Use();
                Repaint();
                return;
            }
            if (e.shift)
            {
                float wheel = e.delta.y != 0f ? e.delta.y : e.delta.x;
                if (Mathf.Approximately(wheel, 0f)) wheel = 1f;
                float factor = 1f - wheel * TrackHeightZoomStep;
                trackHeight = Mathf.Clamp(trackHeight * factor, MinTrackHeight, MaxTrackHeight);
                float maxScrollY = GetMaxScrollY();
                scrollPos.y = Mathf.Clamp(scrollPos.y, 0f, maxScrollY);
                nameScroll.y = scrollPos.y;
                e.Use();
                Repaint();
                return;
            }
            if (e.control)
            {
                float maxScrollY = GetMaxScrollY();
                if (maxScrollY <= 0f)
                {
                    e.Use();
                    return;
                }
                scrollPos.y = Mathf.Clamp(scrollPos.y + e.delta.y * 20f, 0f, maxScrollY);
                nameScroll.y = scrollPos.y;
                e.Use();
                Repaint();
                return;
            }
            const float panSpeed = 5f;
            scrollPos.x += e.delta.y * panSpeed;
            scrollPos.x = Mathf.Max(0f, scrollPos.x);
            e.Use();
            Repaint();
        }
        #endregion

        #region 交互：空点击与刻度尺点击
        /// <summary> 
        ///在 Clip 区空白处按下左键时，取消当前选中并清除参数面板焦点
        ///</summary>
        /// <param name="localArea">Clip 区局部矩形（刻度尺下方）</param>
        private void HandleEmptyClick(Rect localArea)
        {
            Event e = Event.current;
            if (e.type != EventType.MouseDown) return;
            if (e.button != 0) return;
            if (e.alt) return;
            if (!localArea.Contains(e.mousePosition)) return;
            if (clipHitThisFrame) return;
            selectedIndices.Clear();
            selectedIndex = -1;
            GUIUtility.keyboardControl = 0;
            e.Use();
            Repaint();
        }
        /// <summary> 
        ///处理刻度尺区域的左键点击：只清除参数面板焦点，不取消选中
        ///</summary>
        /// <param name="rulerArea">刻度尺在 Clip 区局部坐标系下的矩形</param>
        private void HandleRulerClick(Rect rulerArea)
        {
            Event e = Event.current;
            if (e.type != EventType.MouseDown) return;
            if (e.button != 0) return;
            if (e.alt) return;
            if (!rulerArea.Contains(e.mousePosition)) return;
            GUIUtility.keyboardControl = 0;
            e.Use();
            Repaint();
        }
        #endregion

        #region 交互：快捷键
        /// <summary> 
        ///处理窗口快捷键：F 缩放到合适范围 / R 重置视图 / Esc 关闭窗口 / Delete 删除选中节点
        ///</summary>
        private void HandleShortcuts()
        {
            Event e = Event.current;
            if (e.type != EventType.KeyDown) return;
            if (e.keyCode == KeyCode.Escape)
            {
                Close();
                e.Use();
                return;
            }
            if (target == null) return;
            if (GUIUtility.keyboardControl != 0) return;
            if (e.keyCode == KeyCode.Delete || e.keyCode == KeyCode.Backspace)
            {
                if (selectedIndices.Count > 0)
                {
                    DeleteSelectedNodes();
                    e.Use();
                }
                return;
            }
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

        #region 交互：多选
        /// <summary> 
        ///设置当前选中支持 Ctrl 多选（切换单项）、Shift 范围选
        ///</summary>
        /// <param name="index">目标节点索引</param>
        /// <param name="additive">true 表示 Ctrl 点击（切换单项）</param>
        /// <param name="range">true 表示 Shift 点击（范围选）</param>
        private void SetSelection(int index, bool additive, bool range)
        {
            if (index < 0 || index >= Nodes.Count)
            {
                selectedIndices.Clear();
                selectedIndex = -1;
                return;
            }
            if (range && selectedIndex >= 0)
            {
                int from = Mathf.Min(selectedIndex, index);
                int to = Mathf.Max(selectedIndex, index);
                selectedIndices.Clear();
                for (int i = from; i <= to; i++) selectedIndices.Add(i);
            }
            else if (additive)
            {
                if (selectedIndices.Contains(index))
                {
                    selectedIndices.Remove(index);
                    if (selectedIndex == index)
                        selectedIndex = selectedIndices.Count > 0 ? GetTopmostSelected() : -1;
                }
                else
                {
                    selectedIndices.Add(index);
                    selectedIndex = index;
                }
            }
            else
            {
                selectedIndices.Clear();
                selectedIndices.Add(index);
                selectedIndex = index;
            }
            GUIUtility.keyboardControl = 0;
            Repaint();
        }
        /// <summary> 
        ///取选中集合里索引最小的（视觉最上面）作为主节点
        ///</summary>
        /// <returns>主节点索引；集合为空时返回 -1</returns>
        private int GetTopmostSelected()
        {
            int top = int.MaxValue;
            foreach (int i in selectedIndices) if (i < top) top = i;
            return top == int.MaxValue ? -1 : top;
        }
        #endregion

        #region 交互：全局兜底
        /// <summary> 
        ///全局兜底：鼠标在窗口任意位置松开左键时，强制收尾 Clip 拖拽与名字列宽度拖拽
        ///</summary>
        private void HandleGlobalMouseUp()
        {
            Event e = Event.current;
            if (e.type != EventType.MouseUp) return;
            if (e.button != 0) return;
            bool handled = false;
            if (isResizingNameColumn)
            {
                isResizingNameColumn = false;
                nameColumnResizeControlID = 0;
                GUIUtility.hotControl = 0;
                handled = true;
            }
            if (isResizingParamPanel)
            {
                isResizingParamPanel = false;
                paramPanelResizeControlID = 0;
                GUIUtility.hotControl = 0;
                handled = true;
            }
            if (dragMode != DragMode.None)
            {
                GUIUtility.hotControl = 0;
                dragMode = DragMode.None;
                draggingIndex = -1;
                snapGuideSecond = -1f;
                primaryDragIndex = -1;
                moveDragAnchorSide = 0;
                dragStartDelays.Clear();
                dragStartDurations.Clear();
                if (dragUndoGroup >= 0)
                {
                    Undo.CollapseUndoOperations(dragUndoGroup);
                    dragUndoGroup = -1;
                }
                handled = true;
            }
            if (handled)
            {
                e.Use();
                Repaint();
            }
        }
        #endregion

        #region 交互：名字列宽度拖拽
        /// <summary> 
        ///处理名字列右边缘的拖拽输入，用于调整 <see cref="nameColumnWidth"/>
        ///</summary>
        /// <param name="hotZone">拖拽热区（窗口坐标）</param>
        private void HandleNameColumnResizeInput(Rect hotZone)
        {
            Event e = Event.current;
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            switch (e.type)
            {
                case EventType.MouseMove:
                case EventType.Repaint:
                    {
                        bool hover = hotZone.Contains(e.mousePosition);
                        if (hover != isHoveringNameResizeZone)
                        {
                            isHoveringNameResizeZone = hover;
                            if (e.type == EventType.MouseMove) Repaint();
                        }
                    }
                    break;
                case EventType.MouseDown:
                    if (e.button == 0 && !e.alt && GUIUtility.hotControl == 0
                        && hotZone.Contains(e.mousePosition))
                    {
                        isResizingNameColumn = true;
                        nameColumnResizeControlID = controlID;
                        GUIUtility.hotControl = controlID;
                        GUIUtility.keyboardControl = 0;
                        e.Use();
                    }
                    break;
                case EventType.MouseDrag:
                    if (isResizingNameColumn && GUIUtility.hotControl == nameColumnResizeControlID)
                    {
                        nameColumnWidth = Mathf.Clamp(
                            nameColumnWidth + e.delta.x,
                            MinNameColumnWidth,
                            MaxNameColumnWidth);
                        e.Use();
                        Repaint();
                    }
                    break;
                case EventType.MouseUp:
                    if (isResizingNameColumn && e.button == 0)
                    {
                        isResizingNameColumn = false;
                        GUIUtility.hotControl = 0;
                        e.Use();
                        Repaint();
                    }
                    break;
            }
        }
        /// <summary> 
        ///在 Repaint 阶段为名字列右边缘设置水平缩放光标
        ///</summary>
        /// <param name="hotZone">拖拽热区（窗口坐标）</param>
        private void DrawNameColumnResizeCursor(Rect hotZone)
        {
            if (Event.current.type != EventType.Repaint) return;
            if (isHoveringNameResizeZone || isResizingNameColumn)
            {
                EditorGUIUtility.AddCursorRect(hotZone, MouseCursor.ResizeHorizontal);
            }
        }
        #endregion

        #region 交互：参数面板宽度拖拽
        /// <summary> 
        ///处理参数面板左边缘的拖拽输入，用于调整 <see cref="paramPanelWidth"/>
        ///<para/>向左拖拽使面板变宽，向右拖拽使面板变窄（与面板位置相反）
        ///</summary>
        /// <param name="hotZone">拖拽热区（窗口坐标）</param>
        private void HandleParamPanelResizeInput(Rect hotZone)
        {
            Event e = Event.current;
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            switch (e.type)
            {
                case EventType.MouseMove:
                case EventType.Repaint:
                    {
                        bool hover = hotZone.Contains(e.mousePosition);
                        if (hover != isHoveringParamResizeZone)
                        {
                            isHoveringParamResizeZone = hover;
                            if (e.type == EventType.MouseMove) Repaint();
                        }
                    }
                    break;
                case EventType.MouseDown:
                    if (e.button == 0 && !e.alt && GUIUtility.hotControl == 0
                        && hotZone.Contains(e.mousePosition))
                    {
                        isResizingParamPanel = true;
                        paramPanelResizeControlID = controlID;
                        GUIUtility.hotControl = controlID;
                        GUIUtility.keyboardControl = 0;
                        e.Use();
                    }
                    break;
                case EventType.MouseDrag:
                    if (isResizingParamPanel && GUIUtility.hotControl == paramPanelResizeControlID)
                    {
                        paramPanelWidth = Mathf.Clamp(
                            paramPanelWidth - e.delta.x,
                            MinParamPanelWidth,
                            MaxParamPanelWidth);
                        e.Use();
                        Repaint();
                    }
                    break;
                case EventType.MouseUp:
                    if (isResizingParamPanel && e.button == 0)
                    {
                        isResizingParamPanel = false;
                        GUIUtility.hotControl = 0;
                        SaveViewState();
                        e.Use();
                        Repaint();
                    }
                    break;
            }
        }
        /// <summary> 
        ///在 Repaint 阶段为参数面板左边缘设置水平缩放光标
        ///</summary>
        /// <param name="hotZone">拖拽热区（窗口坐标）</param>
        private void DrawParamPanelResizeCursor(Rect hotZone)
        {
            if (Event.current.type != EventType.Repaint) return;
            if (isHoveringParamResizeZone || isResizingParamPanel)
            {
                EditorGUIUtility.AddCursorRect(hotZone, MouseCursor.ResizeHorizontal);
            }
        }
        #endregion

        #region 工具：时间量化
        /// <summary> 
        ///将时间值量化到两位小数精度（秒），即 10ms 分辨率
        ///</summary>
        /// <param name="t">原始时间（秒）</param>
        /// <returns>量化后的时间（秒），保留两位小数</returns>
        private static float QuantizeTime(float t)
        {
            return Mathf.Round(t * 1000f) / 1000f;
        }
        #endregion

        #region 工具：吸附
        /// <summary> 
        ///吸附阈值（秒）：按像素阈值随缩放换算
        ///</summary>
        private float SnapThresholdSeconds => SnapThresholdPixels / pixelsPerSecond;
        /// <summary> 
        ///对给定时间尝试吸附：优先吸附到其他 Clip 的边界，其次吸附到整秒网格
        ///</summary>
        /// <param name="time">待吸附的时间（秒）</param>
        /// <param name="exclude">排除的节点索引集合；为 null 或空表示不排除</param>
        /// <param name="snappedTime">输出：吸附后的时间（已量化）；未命中时等于 <paramref name="time"/></param>
        /// <returns>true 表示命中吸附；false 表示未命中</returns>
        private bool SnapTime(float time, HashSet<int> exclude, out float snappedTime)
        {
            float threshold = SnapThresholdSeconds;
            float bestTime = time;
            float bestDist = threshold;
            bool snapped = false;
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (exclude != null && exclude.Contains(i)) continue;
                TweenNode other = Nodes[i];
                float otherStart = other.Delay;
                float otherEnd = other.Delay + other.Duration;
                float dStart = Mathf.Abs(time - otherStart);
                if (dStart < bestDist) { bestDist = dStart; bestTime = otherStart; snapped = true; }
                float dEnd = Mathf.Abs(time - otherEnd);
                if (dEnd < bestDist) { bestDist = dEnd; bestTime = otherEnd; snapped = true; }
            }
            float rounded = Mathf.Round(time);
            float distToSecond = Mathf.Abs(time - rounded);
            if (distToSecond < bestDist) { bestDist = distToSecond; bestTime = rounded; snapped = true; }
            snappedTime = snapped ? QuantizeTime(bestTime) : time;
            return snapped;
        }
        #endregion

        #region 工具：滚动范围
        /// <summary> 
        ///计算 Clip 区垂直滚动的上限（像素）
        ///</summary>
        /// <returns>垂直滚动上限（像素），恒 ≥ 0</returns>
        private float GetMaxScrollY()
        {
            float contentHeight = Nodes.Count * trackHeight + 20f;
            float viewH = cachedClipAreaRect.height - rulerHeight - HorizontalScrollbarHeight;
            return Mathf.Max(0f, contentHeight - Mathf.Max(1f, viewH));
        }
        #endregion

        #region 工具：视图操作
        /// <summary> 
        ///缩放到合适范围：选中节点时以该节点 Duration 铺满；未选中时以所有节点最右端适配
        ///</summary>
        private void FitToContent()
        {
            if (Nodes == null || Nodes.Count == 0)
            {
                ResetView();
                return;
            }
            float availableWidth = cachedClipAreaRect.width;
            if (availableWidth <= 1f)
            {
                availableWidth = position.width - nameColumnWidth - paramPanelWidth;
            }
            if (availableWidth <= 1f) return;
            const float padding = 20f;
            if (selectedIndex >= 0 && selectedIndex < Nodes.Count)
            {
                TweenNode node = Nodes[selectedIndex];
                if (node.Duration <= 0.0001f)
                {
                    ResetView();
                    return;
                }
                float usableWidth = Mathf.Max(1f, availableWidth - padding * 2f);
                float tempPixelsPerSecond = usableWidth / node.Duration;
                tempPixelsPerSecond = Mathf.Clamp(tempPixelsPerSecond, 1f, 20000f);
                float delayPixels = node.Delay * tempPixelsPerSecond;
                if (delayPixels > padding)
                {
                    pixelsPerSecond = tempPixelsPerSecond;
                    scrollPos.x = node.Delay * pixelsPerSecond - padding;
                    scrollPos.x = Mathf.Max(0f, scrollPos.x);
                }
                else
                {
                    usableWidth = Mathf.Max(1f, availableWidth - padding);
                    pixelsPerSecond = usableWidth / node.Duration;
                    pixelsPerSecond = Mathf.Clamp(pixelsPerSecond, 1f, 20000f);
                    scrollPos.x = 0f;
                }
                scrollPos.y = 0f;
                nameScroll.y = 0f;
                Repaint();
                return;
            }
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
            scrollPos.x = 0f;
            scrollPos.y = 0f;
            nameScroll.y = 0f;
            Repaint();
        }
        /// <summary> 
        ///重置视图：水平 / 垂直滚动归零，缩放恢复默认值（100 px/s），轨道高度恢复默认值（26 px）
        ///</summary>
        private void ResetView()
        {
            scrollPos.x = 0f;
            scrollPos.y = 0f;
            nameScroll.y = 0f;
            trackHeight = 26f;

            // 让 2 秒铺满当前 Clip 区视口宽度
            float viewportWidth = cachedClipAreaRect.width;
            if (viewportWidth <= 1f)
                viewportWidth = position.width - nameColumnWidth - paramPanelWidth;
            if (viewportWidth <= 1f)
            {
                pixelsPerSecond = 100f;   // 兜底
                return;
            }

            const float targetSeconds = 5f;
            pixelsPerSecond = Mathf.Clamp(viewportWidth / targetSeconds, 1f, 20000f);
        }
        #endregion

        #region 工具：视图状态持久化
        /// <summary> 
        ///从 <see cref="target"/> 读取持久化的视图状态
        ///</summary>
        private void LoadViewState()
        {
            if (target == null) return;
            pixelsPerSecond = Mathf.Clamp(target.Timeline_TrackPosition, 1f, 20000f);
            scrollPos = Vector2.Max(Vector2.zero, target.Timeline_TrackScroll);
            nameScroll.y = scrollPos.y;
            trackHeight = Mathf.Clamp(target.Timeline_TrackHeight, MinTrackHeight, MaxTrackHeight);
            snapEnabled = target.Timeline_TrackSnapEnabled;

            nameColumnWidth = LoadFloatPreference(PrefKey_NameWidthWidth, DefaultNameColumnWidth, MinNameColumnWidth, MaxNameColumnWidth);
            paramPanelWidth = LoadFloatPreference(PrefKey_ParamWidthHeight, DefaultParamPanelWidth, MinParamPanelWidth, MaxParamPanelWidth);
        }
        /// <summary> 
        ///将当前视图状态写回 <see cref="target"/>垂直滚动不保存
        ///</summary>
        private void SaveViewState()
        {
            if (target == null) return;
            target.Timeline_TrackPosition = pixelsPerSecond;
            target.Timeline_TrackScroll = scrollPos;
            target.Timeline_TrackHeight = trackHeight;
            target.Timeline_TrackSnapEnabled = snapEnabled;

            XGUI.x_Editor_Data_Set_With_Float(PrefKey_NameWidthWidth, nameColumnWidth);
            XGUI.x_Editor_Data_Set_With_Float(PrefKey_ParamWidthHeight, paramPanelWidth);

            EditorUtility.SetDirty(target);
        }
        /// <summary> 
        ///读取持久化的 float 偏好值无记录时返回 <paramref name="defaultValue"/>，
        ///并按 <paramref name="min"/> / <paramref name="max"/> 钳制
        ///</summary>
        private static float LoadFloatPreference(string key, float defaultValue, float min, float max)
        {
            if (!XGUI.x_Editor_Data_Has_String(key))
                return Mathf.Clamp(defaultValue, min, max);
            return Mathf.Clamp(XGUI.x_Editor_Data_Get_With_Float(key), min, max);
        }
        #endregion

        #region 工具：窗口尺寸持久化
        /// <summary> 
        ///读取上次关闭时保存的窗口尺寸无记录时返回 <see cref="DefaultWindowSize"/>
        ///</summary>
        /// <returns>窗口尺寸（已按 <see cref="MinWindowSize"/> / <see cref="MaxWindowSize"/> 钳制）</returns>
        private static Vector2 LoadWindowSize()
        {
            if (!XGUI.x_Editor_Data_Has_String(PrefKey_WindowWidth) || !XGUI.x_Editor_Data_Has_String(PrefKey_WindowHeight))
            {
                return DefaultWindowSize;
            }
            float w = XGUI.x_Editor_Data_Get_With_Float(PrefKey_WindowWidth);
            float h = XGUI.x_Editor_Data_Get_With_Float(PrefKey_WindowHeight);
            if (w <= 0f || h <= 0f)
                return DefaultWindowSize;
            return new Vector2(
                Mathf.Clamp(w, MinWindowSize.x, MaxWindowSize.x),
                Mathf.Clamp(h, MinWindowSize.y, MaxWindowSize.y));
        }
        /// <summary> 
        ///将当前窗口尺寸写入 EditorPrefs
        ///</summary>
        private void SaveWindowSize()
        {
            if (this == null) return;
            Vector2 size = position.size;
            if (size.x <= 0f || size.y <= 0f) return;
            size.x = Mathf.Clamp(size.x, MinWindowSize.x, MaxWindowSize.x);
            size.y = Mathf.Clamp(size.y, MinWindowSize.y, MaxWindowSize.y);
            XGUI.x_Editor_Data_Set_With_Float(PrefKey_WindowWidth, size.x);
            XGUI.x_Editor_Data_Set_With_Float(PrefKey_WindowHeight, size.y);
        }
        #endregion

        #region 工具：节点增删
        /// <summary> 
        ///在指定索引位置插入一条新的空白动画节点
        ///</summary>
        /// <param name="insertAt">插入位置；0 表示插到最前，Nodes.Count 表示追加到末尾</param>
        private void InsertNodeAt(int insertAt)
        {
            if (target == null || target.Equals(null)) return;

            Undo.RegisterCompleteObjectUndo(target, "Insert Tween Node");

            TweenNode newNode = new TweenNode();
            newNode.ID = target.TweenNode_ID_Create();
            newNode.Indicator = "NewTween";
            newNode.Enabled = true;
            newNode.Timings = "无";
            newNode.Duration = 1f;
            newNode.Delay = 0f;
            newNode.ActivateFrom = true;
            newNode.ActivateEnd = false;
            newNode.ActivateOnlyToEnd = false;
            newNode.LoopType = XTween_LoopType.Restart;
            newNode.LoopCount = 0;
            newNode.Progress = 0f;
            newNode.IsFold = false;
            newNode.Rewind_Set_Startvalue = true;
            newNode.Complete_Set_Endvalue = true;
            newNode.Original_Color = Color.white;
            newNode.From_Color = Color.white;
            newNode.End_Color = Color.white;
            newNode.Curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

            insertAt = Mathf.Clamp(insertAt, 0, Nodes.Count);
            Nodes.Insert(insertAt, newNode);
            EditorUtility.SetDirty(target);

            selectedIndices.Clear();
            selectedIndices.Add(insertAt);
            selectedIndex = insertAt;
            Repaint();
        }
        /// <summary> 
        ///在指定索引的节点下方插入一条新的空白动画节点
        ///</summary>
        private void InsertNodeAfter(int index)
        {
            if (target == null) return;
            if (index < 0 || index >= Nodes.Count) return;
            InsertNodeAt(index + 1);
        }
        /// <summary> 
        ///删除指定索引的动画节点，并修正选中集合
        ///</summary>
        /// <param name="index">要删除的节点索引</param>
        private void DeleteNodeAt(int index)
        {
            if (target == null) return;
            if (index < 0 || index >= Nodes.Count) return;
            Undo.RegisterCompleteObjectUndo(target, "Delete Tween Node");
            TweenNode node = Nodes[index];
            if (node.Tweener != null)
            {
                node.Tweener.Kill();
                node.Tweener = null;
            }
            Nodes.RemoveAt(index);
            EditorUtility.SetDirty(target);
            HashSet<int> newSelection = new HashSet<int>();
            foreach (int i in selectedIndices)
            {
                if (i < index) newSelection.Add(i);
                else if (i > index) newSelection.Add(i - 1);
            }
            selectedIndices.Clear();
            foreach (int i in newSelection) selectedIndices.Add(i);
            if (selectedIndex == index)
            {
                selectedIndex = selectedIndices.Count > 0 ? GetTopmostSelected() : -1;
            }
            else if (selectedIndex > index)
            {
                selectedIndex--;
            }
            scrollPos.y = Mathf.Clamp(scrollPos.y, 0f, GetMaxScrollY());
            nameScroll.y = scrollPos.y;
            Repaint();
        }
        /// <summary> 
        ///删除所有当前选中的动画节点（批量删除）
        ///</summary>
        private void DeleteSelectedNodes()
        {
            if (target == null) return;
            if (selectedIndices.Count == 0) return;
            Undo.RegisterCompleteObjectUndo(target, "Delete Tween Nodes");
            List<int> indices = new List<int>(selectedIndices);
            indices.Sort((a, b) => b.CompareTo(a));
            foreach (int i in indices)
            {
                if (i < 0 || i >= Nodes.Count) continue;
                TweenNode node = Nodes[i];
                if (node.Tweener != null)
                {
                    node.Tweener.Kill();
                    node.Tweener = null;
                }
                Nodes.RemoveAt(i);
            }
            EditorUtility.SetDirty(target);
            selectedIndices.Clear();
            selectedIndex = -1;
            scrollPos.y = Mathf.Clamp(scrollPos.y, 0f, GetMaxScrollY());
            nameScroll.y = scrollPos.y;
            Repaint();
        }
        #endregion

        #region 工具：清空窗口以及面板配置
        /// <summary> 
        ///【测试】清空本窗口在 EditorPrefs 中保存的所有 key，
        ///使下次打开等同于「首次运行」：窗口尺寸 / 名字列宽 / 参数面板宽全部回落到默认值
        ///<para/>不会动 <see cref="target"/> 上的视图状态（缩放 / 滚动 / 轨道高 / 吸附）
        ///</summary>
        //[MenuItem("Tools/XHud/Tween Tracker/Reset Window Prefs (Test)")]
        private static void ResetAllEditorPrefs_Test()
        {
            XGUI.x_Editor_Data_Clear(PrefKey_WindowWidth);
            XGUI.x_Editor_Data_Clear(PrefKey_WindowHeight);
            XGUI.x_Editor_Data_Clear(PrefKey_NameWidthWidth);
            XGUI.x_Editor_Data_Clear(PrefKey_ParamWidthHeight);

            Debug.Log("[XHud] Primitive Tween Tracker EditorPrefs 已清空，窗口已回落默认尺寸");
        }
        #endregion
    }
}