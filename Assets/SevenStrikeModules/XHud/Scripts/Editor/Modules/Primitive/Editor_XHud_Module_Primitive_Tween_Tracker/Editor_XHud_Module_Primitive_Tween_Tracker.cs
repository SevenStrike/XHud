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
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// 图元动画数值属性包装类，用于在参数面板中统一引用「起始 / 结束 / 默认」三组序列化属性
    /// <para/>
    /// 使用场景：<see cref="Editor_XHud_Module_Primitive_Tween_Tracker.DrawNodeParameterFields"/>
    /// 中通过 <c>ResolveTweenValueProperties</c> 构造本类实例，
    /// 再传递给 <c>DrawTweenValueEditor</c> 使用。
    /// <para/>
    /// 三个序列化属性的语义：
    /// <list type="bullet">
    /// <item><description><see cref="prop_from"/>：起始值（From），动画开始时的值；</description></item>
    /// <item><description><see cref="prop_end"/>：结束值（End），动画结束时的值；</description></item>
    /// <item><description><see cref="prop_origin"/>：默认值（Original），动画开始前的默认状态值。</description></item>
    /// </list>
    /// 三者的组合可表达四种数值过渡模式（S-D / D-E / S-E / C-E），
    /// 具体哪两组参与动画由节点的 <c>valuemode_index</c> 决定。
    /// <para/>
    /// 注意：类名沿用项目的 <c>snake_case</c> 风格（与 <c>value_*</c> 系列方法一致），
    /// 未改为 PascalCase 是为避免波及既有调用点。
    /// </summary>
    internal class TweenValueProperties
    {
        /// <summary>
        /// 动画类型名称（由 <see cref="TweenNodeType"/> 去掉前缀 <c>"x_"</c> 得到），
        /// 例如 <c>a_位移</c> → <c>"位移"</c>。当前未在 UI 中使用，保留供扩展。
        /// </summary>
        public string title;

        /// <summary>
        /// 该数值属性对应的动画类型，决定参数面板显示哪些字段。
        /// </summary>
        public TweenNodeType type;

        /// <summary>
        /// 起始值序列化属性（From）。在 S-D / S-E 模式中作为动画起点。
        /// </summary>
        public SerializedProperty prop_from;

        /// <summary>
        /// 结束值序列化属性（End）。在 D-E / S-E / C-E 模式中作为动画终点。
        /// </summary>
        public SerializedProperty prop_end;

        /// <summary>
        /// 默认值序列化属性（Original）。在 S-D / D-E 模式中作为一端，
        /// 也用于「重置」操作的回退目标。
        /// </summary>
        public SerializedProperty prop_origin;

        /// <summary>
        /// 构造：以标题、类型与三组序列化属性初始化包装对象
        /// <para/>
        /// 注意：当前代码中实际使用的是无参构造 + 逐字段赋值的方式，
        /// 本重载保留供未来直接构造场景使用。
        /// </summary>
        /// <param name="title">动画类型名称（已去掉枚举前缀）</param>
        /// <param name="type">动画节点类型</param>
        /// <param name="prop_start">起始值序列化属性</param>
        /// <param name="prop_end">结束值序列化属性</param>
        /// <param name="prop_origin">默认值序列化属性</param>
        public TweenValueProperties(string title, TweenNodeType type, SerializedProperty prop_start, SerializedProperty prop_end, SerializedProperty prop_origin)
        {
            this.title = title;
            this.type = type;
            this.prop_from = prop_start;
            this.prop_end = prop_end;
            this.prop_origin = prop_origin;
        }

        /// <summary>
        /// 构造：创建空包装对象，字段由调用方后续赋值
        /// <para/>
        /// 这是 <c>ResolveTweenValueProperties</c> 中实际使用的方式：
        /// 先 new 出空对象，再按动画类型逐字段填入 <c>prop_from / prop_end / prop_origin</c>。
        /// </summary>
        public TweenValueProperties() { }
    }

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
    public partial class Editor_XHud_Module_Primitive_Tween_Tracker : EditorWindow
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
        /// <summary> 
        ///轨道种类之间的间距行默认高度（像素）
        ///</summary>
        private const float DefaultTrackKindGapHeight = 2;
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
        /// 时间轴内容末尾之后额外保留的「富余秒数」。
        /// <para/>
        /// 例如内容最右端为 4s、本值为 2f，则时间轴总时长 = 6s，
        /// 末尾多出的 2 秒用于把 Clip 拖到最右端、或放置新 Clip。
        /// <para/>
        /// 该值以「秒」为单位，不随缩放变化，保证任何缩放下末尾富余都是固定时长。
        /// </summary>
        private const float ContentTrailingSeconds = 2f;
        /// <summary> 
        /// 音效 Sound 为空时，Clip 在时间轴上的默认占位长度（秒）
        /// </summary>
        private const float DefaultSoundClipSeconds = 1f;
        /// <summary>
        /// 内容区底部留白（像素）。
        /// <para/>
        /// 用于统一控制轨道区（名字列 + Clip 区）**内容底部**的留白高度：
        /// 追加在最后一行（音效或动画）之后，使滚动到底时最后一行不会紧贴
        /// 底部水平滚动条。
        /// <para/>
        /// 值以「像素」为单位，与 <see cref="trackKindGapHeight"/> 不同——
        /// 后者是动画轨与音效轨之间的分隔带高度，本字段是整块内容的收尾留白。
        /// <para/>
        /// 只在「行高总和 > 视口高度」时才追加，避免刚好放下时多出无意义的滚动空间。
        /// </summary>
        private float contentBottomPadding = 2f;
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

        #region 常量：颜色
        private string Theme_PrimaryColor = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary);
        private string DisableTextColor = XGUI_Utilitys.Color_To_HexString(Color.white * 0.85f);
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
        /// <summary> 
        ///音效轨 Clip 背景色（常规）
        ///</summary>
        private static readonly Color ColorSoundClipBG = XGUI_Utilitys.HexString_To_Color("275d57");
        /// <summary> 
        ///音效轨 Clip 背景色（选中）
        ///</summary>
        private static readonly Color ColorSoundClipBG_Selected = XGUI_Utilitys.HexString_To_Color("398980");
        /// <summary> 
        ///音效轨 Clip 背景色（Sound 为空）
        ///</summary>
        private static readonly Color ColorSoundClipBG_Empty = XGUI_Utilitys.HexString_To_Color("3f3f3f");
        /// <summary> 
        ///音效轨 Clip 背景色（Sound 为空）-选中
        ///</summary>
        private static readonly Color ColorSoundClipBG_Empty_Selected = Color.gray * 0.65f;
        /// <summary> 
        ///音效轨行底色（与动画轨区分）
        ///</summary>
        private static readonly Color ColorSoundTrackBG = XGUI_Utilitys.HexString_To_Color("363636");
        /// <summary> 
        ///音效轨行底色（与动画轨区分）静音-未选中
        ///</summary>
        private static readonly Color ColorSoundTrackBG_muted = Color.gray * 0.55f;
        /// <summary> 
        ///音效轨行底色（与动画轨区分）静音-选中
        ///</summary>
        private static readonly Color ColorSoundTrackBG_muted_Selected = Color.gray * 0.65f;
        /// <summary> 
        ///轨道种类间距行的底色（比轨道底色略深，形成凹陷感）
        ///</summary>
        private static readonly Color ColorTrackKindGapBg = XGUI_Utilitys.HexString_To_Color("545454");
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

        #region 字段：预览状态
        /// <summary>
        /// 当前是否处于预览状态。
        /// <para/>
        /// 直接读取 <see cref="target"/> 上的 <see cref="XHud_Module_Primitive_Tween.TweenIsPreviewing"/> 字段，
        /// 与 Inspector 共享同一个判断依据，无需额外同步。
        /// </summary>
        private bool IsPreviewing => target != null && target.TweenIsPreviewing;
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

        #region 字段：参数面板必要组件
        private XHud_Module_Text HudText;
        private XHud_Module_TmpText HudTmpText;
        private XHud_Module_Button HudButton;
        private XHud_Module_Progress HudProgress;
        private XHud_Module_Toggle HudToggle;
        private XHud_Module_Slider HudSlider;
        private XHud_Module_Option HudOption;
        #endregion

        #region 字段：SerializedObject 缓存

        /// <summary>
        /// 参数面板共用的 SerializedObject。
        /// <para/>缓存策略：与 <see cref="cachedSO_target"/> 引用相等时复用，否则重建。
        /// 目标对象切换（OpenWith 或 target 被替换）时缓存自动失效。
        /// </summary>
        private SerializedObject cachedSO;

        /// <summary>
        /// cachedSO 对应的目标对象。用于判断缓存是否还有效。
        /// 当 target 变化时，cachedSO_target != target，触发重建。
        /// </summary>
        private UnityEngine.Object cachedSO_target;

        /// <summary>
        /// 缓存 <see cref="cachedSO"/> 里的 PrimitiveTweenNodes 数组属性。
        /// <para/>每次选中变化时重建，避免每帧 FindProperty 的开销。
        /// </summary>
        private SerializedProperty cachedNodesProp;

        /// <summary>
        /// 缓存 <see cref="cachedSO"/> 里的 PrimitiveTweenSounds 数组属性。
        /// </summary>
        private SerializedProperty cachedSoundsProp;

        /// <summary>
        /// 上一帧选中的轨道种类，用于判断是否需要重建子属性缓存。
        /// </summary>
        private TrackKind cachedSOKind = TrackKind.无;

        /// <summary>
        /// 上一帧选中的索引，用于判断是否需要重建子属性缓存。
        /// </summary>
        private int cachedSOIndex = -1;

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
        /// 静音图标（常规）
        /// </summary>
        private Texture2D icon_mute_r;
        /// <summary>
        /// 静音图标（按下）
        /// </summary>
        private Texture2D icon_mute_p;
        /// <summary>
        /// 不静音图标（常规）
        /// </summary>
        private Texture2D icon_unmute_r;
        /// <summary>
        /// 不静音图标（按下）
        /// </summary>
        private Texture2D icon_unmute_p;
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
        /// <summary>
        /// 类型大图标 - 位移
        /// </summary>
        private Texture2D b_anim_type_move;
        /// <summary>
        /// 类型大图标 - 旋转
        /// </summary>
        private Texture2D b_anim_type_rotate;
        /// <summary>
        /// 类型大图标 - 缩放
        /// </summary>
        private Texture2D b_anim_type_scale;
        /// <summary>
        /// 类型大图标 - 颜色
        /// </summary>
        private Texture2D b_anim_type_color;
        /// <summary>
        /// 类型大图标 - 淡化
        /// </summary>
        private Texture2D b_anim_type_fade;
        /// <summary>
        /// 类型大图标 - 打字机
        /// </summary>
        private Texture2D b_anim_type_writter;
        /// <summary>
        /// 类型大图标 - 填充
        /// </summary>
        private Texture2D b_anim_type_fill;
        /// <summary>
        /// 类型大图标 - 尺寸
        /// </summary>
        private Texture2D b_anim_type_size;
        /// <summary>
        /// 轨道参数取数值操作图标 - 记录（常规）
        /// </summary>
        private Texture2D icon_track_param_record_r;
        /// <summary>
        /// 轨道参数取数值操作图标 - 记录（按下）
        /// </summary>
        private Texture2D icon_track_param_record_p;
        /// <summary>
        /// 轨道参数取数值操作图标 - 应用（常规）
        /// </summary>
        private Texture2D icon_track_param_apply_r;
        /// <summary>
        /// 轨道参数取数值操作图标 - 应用（按下）
        /// </summary>
        private Texture2D icon_track_param_apply_p;
        /// <summary>
        /// 轨道参数取数值操作图标 - 归零（常规）
        /// </summary>
        private Texture2D icon_track_param_reset_r;
        /// <summary>
        /// 轨道参数取数值操作图标 - 归零（按下）
        /// </summary>
        private Texture2D icon_track_param_reset_p;
        /// <summary>
        /// 轨道参数数值流向图标 - 指示从起始到默认（常规）
        /// </summary>
        private Texture2D icon_track_param_connector_status_r;
        /// <summary>
        /// 轨道参数数值流向图标 - 指示从？到？（按下）
        /// </summary>
        private Texture2D icon_track_param_connector_status_p;
        /// <summary>
        /// 参数面板无选中时提示图标
        /// </summary>
        private Texture2D icon_param_nullselected_warning;
        /// <summary>
        /// 参数面板多选中时提示图标
        /// </summary>
        private Texture2D icon_param_mixedselected_warning;
        /// <summary>
        /// 添加音效图标（常规）
        /// </summary>
        private Texture2D icon_add_sound_r;
        /// <summary>
        /// 添加音效图标（按下）
        /// </summary>
        private Texture2D icon_add_sound_p;
        /// <summary>
        /// 添加动画图标（常规）
        /// </summary>
        private Texture2D icon_add_tween_r;
        /// <summary>
        /// 添加动画图标（按下）
        /// </summary>
        private Texture2D icon_add_tween_p;
        /// <summary>
        /// 时间飞梭头部图标。
        /// <para/>建议做成倒三角 / 播放头形状，尺寸与 PlayheadHeadWidth × PlayheadHeadHeight 匹配。
        /// </summary>
        private Texture2D icon_playhead;
        /// <summary>
        /// 播放控制图标 - 跳转到轨道头（常规）
        /// </summary>
        private Texture2D icon_playback_home_r;
        /// <summary>
        /// 播放控制图标 - 跳转到轨道头（按下）
        /// </summary>
        private Texture2D icon_playback_home_p;
        /// <summary>
        /// 播放控制图标 - 步进后退（常规）
        /// </summary>
        private Texture2D icon_playback_stepback_r;
        /// <summary>
        /// 播放控制图标 - 步进后退（按下）
        /// </summary>
        private Texture2D icon_playback_stepback_p;
        /// <summary>
        /// 播放控制图标 - 播放（常规）
        /// </summary>
        private Texture2D icon_playback_play_r;
        /// <summary>
        /// 播放控制图标 - 播放（按下）
        /// </summary>
        private Texture2D icon_playback_play_p;
        /// <summary>
        /// 播放控制图标 - 停止（常规）
        /// </summary>
        private Texture2D icon_playback_stop_r;
        /// <summary>
        /// 播放控制图标 - 停止（按下）
        /// </summary>
        private Texture2D icon_playback_stop_p;
        /// <summary>
        /// 播放控制图标 - 步进前进（常规）
        /// </summary>
        private Texture2D icon_playback_stepforward_r;
        /// <summary>
        /// 播放控制图标 - 步进前进（按下）
        /// </summary>
        private Texture2D icon_playback_stepforward_p;
        /// <summary>
        /// 播放控制图标 - 跳转到轨道尾（常规）
        /// </summary>
        private Texture2D icon_playback_end_r;
        /// <summary>
        /// 播放控制图标 - 跳转到轨道尾（按下）
        /// </summary>
        private Texture2D icon_playback_end_p;
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
        /// <summary> 
        ///动画轨与音效轨之间的间距行高度（像素）。
        ///<para/>该行不承载任何轨道数据，仅供布局分隔与自定义绘制。
        ///</summary>
        private float trackKindGapHeight = DefaultTrackKindGapHeight;
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
        /// 选中的动画节点索引集合。
        /// <para/>
        /// 跨类型多选时，音效的选中集合见 <see cref="selectedSoundIndices"/>。
        /// </summary>
        private readonly HashSet<int> selectedNodeIndices = new HashSet<int>();
        /// <summary>
        /// 选中的音效索引集合。
        /// <para/>
        /// 跨类型多选时，动画的选中集合见 <see cref="selectedNodeIndices"/>。
        /// </summary>
        private readonly HashSet<int> selectedSoundIndices = new HashSet<int>();
        /// <summary>
        /// 当前动画器的音效列表（<see cref="target"/> 的快捷访问）
        /// <para/>
        /// 与 <see cref="Nodes"/> 平行，无父子关系。
        /// </summary>
        private List<TweenSound> Sounds => target.PrimitiveTweenSounds;
        /// <summary>
        /// 当前选中的轨道种类。
        /// <para/>
        /// - <see cref="TrackKind.Node"/>：<see cref="selectedIndex"/> / <see cref="selectedIndices"/>
        ///   的索引指向 <see cref="Nodes"/>；
        /// - <see cref="TrackKind.Sound"/>：索引指向 <see cref="Sounds"/>；
        /// - <see cref="TrackKind.无"/>：无选中。
        /// <para/>
        /// 多选**不允许跨类型**：一旦切换 <see cref="selectedKind"/>，选中集合会被清空。
        /// </summary>
        private TrackKind selectedKind = TrackKind.无;
        /// <summary> 当前选中的总数（动画 + 音效）。 </summary>
        private int TotalSelectedCount => selectedNodeIndices.Count + selectedSoundIndices.Count;

        /// <summary> 是否处于跨类型多选状态（动画和音效都有选中）。 </summary>
        private bool IsCrossTypeSelection =>
            selectedNodeIndices.Count > 0 && selectedSoundIndices.Count > 0;
        #endregion

        #region 字段：Clip 拖拽状态
        /// <summary> 
        ///Clip 拖拽模式值为 <see cref="DragMode.无"/> 表示当前无拖拽
        ///</summary>
        private DragMode dragMode = DragMode.无;
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
        /// 拖拽开始时，选中的动画节点起始 Delay。 
        /// </summary>
        private readonly Dictionary<int, float> dragStartNodeDelays = new Dictionary<int, float>();
        /// <summary> 
        /// 拖拽开始时，选中的动画节点起始 Duration。 
        /// </summary>
        private readonly Dictionary<int, float> dragStartNodeDurations = new Dictionary<int, float>();
        /// <summary> 
        /// 拖拽开始时，选中的音效起始 Delay。 
        /// </summary>
        private readonly Dictionary<int, float> dragStartSoundDelays = new Dictionary<int, float>();
        /// <summary> 
        ///多选拖拽时的「主节点」索引（选中最上面那个）吸附以此为准单选时等于 selectedIndex
        ///</summary>
        private int primaryDragIndex = -1;
        /// <summary> 
        ///本次 Clip 拖拽所属的 Undo 组 IDMouseDown 时开组，MouseUp 时 Collapse
        ///</summary>
        private int dragUndoGroup = -1;
        /// <summary> 
        /// 当前正在拖拽的轨道种类。
        /// <para/>
        /// <see cref="DragMode.无"/> 时此值应保持 <see cref="TrackKind.无"/>。
        /// </summary>
        private TrackKind draggingKind = TrackKind.无;
        #endregion

        #region 字段：时间飞梭
        /// <summary>
        /// 时间飞梭当前位置（秒）。
        /// <para/>这是"内容时间"，与 scrollPos.x / pixelsPerSecond 无关，
        /// 转换到屏幕坐标需减去 scrollPos.x。
        /// </summary>
        private float playheadSecond = 0f;
        /// <summary>
        /// 是否正在拖拽时间飞梭。
        /// </summary>
        private bool isDraggingPlayhead = false;
        /// <summary>
        /// 拖拽飞梭时占用的 ControlID。
        /// </summary>
        private int playheadControlID = 0;
        /// <summary>
        /// 飞梭头部（刻度尺上那个可抓取的小方块）的宽度（像素）。
        /// </summary>
        private const float PlayheadHeadWidth = 25f;
        /// <summary>
        /// 飞梭头部的高度（像素）。
        /// </summary>
        private const float PlayheadHeadHeight = 25f;
        /// <summary>
        /// 飞梭头部命中判定额外扩展的容差（像素），让头部更好抓。
        /// </summary>
        private const float PlayheadHeadHitPadding = 4f;
        /// <summary>
        /// 飞梭竖线颜色。
        /// </summary>
        private static readonly Color ColorPlayheadLine = new Color(1f, 0.35f, 0.35f, 1f);
        /// <summary>
        /// 飞梭头部颜色。
        /// </summary>
        private static readonly Color ColorPlayheadHead = new Color(1f, 0.35f, 0.35f, 1f);
        /// <summary>
        /// 飞梭头部悬停时的颜色（更亮）。
        /// </summary>
        private static readonly Color ColorPlayheadHeadHover = new Color(1f, 0.6f, 0.6f, 1f);
        /// <summary> 
        ///飞梭线进入C-E模式Clip的颜色
        ///</summary>
        private static readonly Color ColorPlayhead_C_E = Color.white;
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

        #region 字段：Play 模式冻结
        /// <summary>
        /// 进入 Play 前缓存的 target 引用（用于退出 Play 后恢复）。
        /// <para/>Unity Play 时场景重载，编辑器里的组件实例被销毁，
        /// 退出 Play 后编辑器场景恢复，但 C# 引用已断，需要用 instanceID 重新查找。
        /// </summary>
        private int cachedTargetInstanceID = 0;
        /// <summary>
        /// 是否正在被 Play 模式冻结（Play 期间为 true）。
        /// </summary>
        private bool isFrozenByPlayMode = false;
        /// <summary>
        /// 进入 Play 前缓存的目标 GameObject 名称（用于恢复失败时提示）。
        /// </summary>
        private string cachedTargetName = "";
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
            无,
            /// <summary> 
            ///整体移动（修改 Delay，Duration 不变）
            ///</summary>
            移动,
            /// <summary> 
            ///拖拽左边缘（同时修改 Delay 与 Duration，右端固定）
            ///</summary>
            左边缘,
            /// <summary> 
            ///拖拽右边缘（仅修改 Duration，左端固定）
            ///</summary>
            右边缘
        }
        #endregion

        #region 嵌套类型：轨道种类
        /// <summary>
        /// 轨道种类，用于区分当前操作的是动画节点轨还是音效轨，
        /// 以及两类轨道之间的间距行。
        /// <para/>
        /// - <see cref="Node"/>：对应 <see cref="XHud_Module_Primitive_Tween.PrimitiveTweenNodes"/>；
        /// - <see cref="Sound"/>：对应 <see cref="XHud_Module_Primitive_Tween.PrimitiveTweenSounds"/>；
        /// - <see cref="Gap"/>：动画与音效之间的间距行，无数据索引；
        /// - <see cref="无"/>：无选中。
        /// <para/>
        /// <see cref="Node"/> 与 <see cref="Sound"/> 是平行列表，无父子关系。
        /// </summary>
        private enum TrackKind
        {
            无,
            Node,
            Sound,
            /// <summary> 
            ///动画轨与音效轨之间的间距行。
            ///<para/>该行不承载轨道数据，仅用于布局分隔与自定义绘制。
            ///</summary>
            Gap
        }
        #endregion

        #region 窗口入口
        /// <summary> 
        /// 打开（或复用）迷你时间轴窗口，并绑定指定的图元动画器
        /// </summary>
        /// <param name="tween">要编辑的图元动画器</param>
        public static void OpenWith(XHud_Module_Primitive_Tween tween)
        {
            Editor_XHud_Module_Primitive_Tween_Tracker window =
                (Editor_XHud_Module_Primitive_Tween_Tracker)EditorWindow.GetWindow(
                    typeof(Editor_XHud_Module_Primitive_Tween_Tracker),
                    false, "XHud 图元动画轨道编辑器", true);

            window.minSize = MinWindowSize;
            Vector2 savedSize = LoadPersistedWindowSize();

            XGUI.CenterEditorWindow(new Vector2Int((int)savedSize.x, (int)savedSize.y), window, false, false);

            if (window.target != tween)
            {
                window.SavePersistedViewState();
                window.target = tween;
                window.selectedIndex = -1;
                window.selectedNodeIndices.Clear();
                window.selectedSoundIndices.Clear();
                window.selectedKind = TrackKind.无;

                //  target 换了，缓存的 SO 失效
                window.cachedSO = null;
                window.cachedSO_target = null;
                window.cachedNodesProp = null;
                window.cachedSoundsProp = null;
                window.cachedSOKind = TrackKind.无;
                window.cachedSOIndex = -1;

                window.LoadPersistedViewState();
                window.RefreshHostComponentCache();

                //  ← 新增：每次绑定新 target 时记录一次特性基线，
                // 保证关窗口时 PrimitiveFeature_Load 能还原到"本次打开前"的状态
                SaveFeatureBaseline(tween);
            }
            window.Repaint();
            window.Focus();
        }
        /// <summary>
        /// 记录图元特性基线（每次打开窗口绑定新 target 时调用）。
        /// <para/>
        /// 用"本次打开窗口前的组件状态"作为关闭窗口时的还原目标。
        /// <para/>
        /// 与"只在 FirstSaveFeatures == false 时记录"不同：本方法<b>无条件</b>覆盖基线。
        /// 原因：历史上有过"关窗口没还原"的不可复现问题，宁可每次重记一次，
        /// 也不依赖上一次留下的基线（可能已被污染 / 未持久化 / 状态错乱）。
        /// <para/>
        /// 副作用：会覆盖 FirstSaveFeatures 标志为 true，并标记组件脏以持久化。
        /// <para/>
        /// 注意：本方法只在 <c>window.target != tween</c> 分支里调用，
        /// 同一 target 重复打开不会重复 Save，天然避免"把预览残留当基线"。
        /// </summary>
        private static void SaveFeatureBaseline(XHud_Module_Primitive_Tween tween)
        {
            if (tween == null || tween.Equals(null)) return;

            // 补全 controller 引用（可能尚未绑定）
            if (tween.controller == null)
                tween.Tween_GetController();
            if (tween.controller == null) return;

            // 取 Feature（优先 controller.pt_Feature，兜底 GetComponent）
            XHud_Module_Primitive_Feature feature = tween.controller.pt_Feature;
            if (feature == null)
            {
                feature = tween.GetComponent<XHud_Module_Primitive_Feature>();
                if (feature != null)
                {
                    tween.controller.pt_Feature = feature;
                    feature.FindController();   // 反向绑定
                }
            }
            if (feature == null) return;

            // 无条件记录基线
            feature.PrimitiveFeature_Save();
            feature.FirstSaveFeatures = true;

            // 标记脏，让改动持久化
            UnityEditor.EditorUtility.SetDirty(feature);
        }
        /// <summary> 
        /// 根据当前 target 刷新其所在的 XHud 宿主组件缓存
        /// </summary>
        private void RefreshHostComponentCache()
        {
            if (target == null || target.Equals(null))
            {
                HudText = null;
                HudTmpText = null;
                HudButton = null;
                HudProgress = null;
                HudToggle = null;
                HudSlider = null;
                HudOption = null;
                return;
            }

            HudText = target.GetComponentInParent<XHud_Module_Text>();
            HudTmpText = target.GetComponentInParent<XHud_Module_TmpText>();
            HudButton = target.GetComponentInParent<XHud_Module_Button>();
            HudProgress = target.GetComponentInParent<XHud_Module_Progress>();
            HudToggle = target.GetComponentInParent<XHud_Module_Toggle>();
            HudSlider = target.GetComponentInParent<XHud_Module_Slider>();
            HudOption = target.GetComponentInParent<XHud_Module_Option>();
        }
        #endregion

        #region 生命周期
        /// <summary> 
        ///Unity 启用回调：注册 Undo/Redo 监听，加载图标
        /// </summary>
        private void OnEnable()
        {
            //  Play 模式监听
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            //  防御：重编译后 OnEnable 重跑，先把可能残留的静态回调摘掉
            EditorApplication.update -= OnPlaybackEditorUpdate;
            EditorApplication.focusChanged -= OnEditorFocusChanged;
            Undo.undoRedoPerformed -= OnUndoRedoPerformed;

            // 然后按正常流程重新注册
            Undo.undoRedoPerformed += OnUndoRedoPerformed;
            EditorApplication.focusChanged += OnEditorFocusChanged;

            string iconRoot = XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path() + "gui_module_primitive_tween/";

            icon_param_nullselected_warning = XGUI.GetBasedIcon("icon_warning");
            icon_param_mixedselected_warning = XGUI.GetBasedIcon("icon_warning");
            icon_del_r = XGUI.GetCustomIcon($"{iconRoot}icon_del_r");
            icon_del_p = XGUI.GetCustomIcon($"{iconRoot}icon_del_p");
            icon_add_r = XGUI.GetCustomIcon($"{iconRoot}icon_add_r");
            icon_add_p = XGUI.GetCustomIcon($"{iconRoot}icon_add_p");
            icon_menu_r = XGUI.GetCustomIcon($"{iconRoot}icon_menu_r");
            icon_menu_p = XGUI.GetCustomIcon($"{iconRoot}icon_menu_p");
            icon_enabled_r = XGUI.GetCustomIcon($"{iconRoot}icon_enabled_r");
            icon_enabled_p = XGUI.GetCustomIcon($"{iconRoot}icon_enabled_p");
            icon_disabled_r = XGUI.GetCustomIcon($"{iconRoot}icon_disabled_r");
            icon_disabled_p = XGUI.GetCustomIcon($"{iconRoot}icon_disabled_p");
            icon_help_r = XGUI.GetCustomIcon($"{iconRoot}icon_help_r");
            icon_help_p = XGUI.GetCustomIcon($"{iconRoot}icon_help_p");
            icon_type_color = XGUI.GetCustomIcon($"{iconRoot}anim_type_color");
            icon_type_fade = XGUI.GetCustomIcon($"{iconRoot}anim_type_fade");
            icon_type_fill = XGUI.GetCustomIcon($"{iconRoot}anim_type_fill");
            icon_type_move = XGUI.GetCustomIcon($"{iconRoot}anim_type_move");
            icon_type_rotator = XGUI.GetCustomIcon($"{iconRoot}anim_type_rotator");
            icon_type_scale = XGUI.GetCustomIcon($"{iconRoot}anim_type_scale");
            icon_type_size = XGUI.GetCustomIcon($"{iconRoot}anim_type_size");
            icon_type_writter = XGUI.GetCustomIcon($"{iconRoot}anim_type_writter");
            icon_timeline_logo = XGUI.GetCustomIcon($"{iconRoot}icon_timeline_logo");
            icon_null_check = XGUI.GetCustomIcon($"{iconRoot}icon_null_check");
            icon_null_add_r = XGUI.GetCustomIcon($"{iconRoot}icon_null_add_r");
            icon_null_add_p = XGUI.GetCustomIcon($"{iconRoot}icon_null_add_p");
            b_anim_type_move = XGUI.GetCustomIcon($"{iconRoot}b_anim_type_move");
            b_anim_type_rotate = XGUI.GetCustomIcon($"{iconRoot}b_anim_type_rotate");
            b_anim_type_scale = XGUI.GetCustomIcon($"{iconRoot}b_anim_type_scale");
            b_anim_type_color = XGUI.GetCustomIcon($"{iconRoot}b_anim_type_color");
            b_anim_type_fade = XGUI.GetCustomIcon($"{iconRoot}b_anim_type_fade");
            b_anim_type_writter = XGUI.GetCustomIcon($"{iconRoot}b_anim_type_writter");
            b_anim_type_fill = XGUI.GetCustomIcon($"{iconRoot}b_anim_type_fill");
            b_anim_type_size = XGUI.GetCustomIcon($"{iconRoot}b_anim_type_size");
            icon_track_param_record_r = XGUI.GetCustomIcon($"{iconRoot}icon_track_param_record_r");
            icon_track_param_record_p = XGUI.GetCustomIcon($"{iconRoot}icon_track_param_record_p");
            icon_track_param_apply_r = XGUI.GetCustomIcon($"{iconRoot}icon_track_param_apply_r");
            icon_track_param_apply_p = XGUI.GetCustomIcon($"{iconRoot}icon_track_param_apply_p");
            icon_track_param_reset_r = XGUI.GetCustomIcon($"{iconRoot}icon_track_param_reset_r");
            icon_track_param_reset_p = XGUI.GetCustomIcon($"{iconRoot}icon_track_param_reset_p");
            icon_track_param_connector_status_r = XGUI.GetCustomIcon($"{iconRoot}icon_track_param_connector_status_r");
            icon_track_param_connector_status_p = XGUI.GetCustomIcon($"{iconRoot}icon_track_param_connector_status_p");
            icon_mute_r = XGUI.GetCustomIcon($"{iconRoot}icon_mute_r");
            icon_mute_p = XGUI.GetCustomIcon($"{iconRoot}icon_mute_p");
            icon_unmute_r = XGUI.GetCustomIcon($"{iconRoot}icon_unmute_r");
            icon_unmute_p = XGUI.GetCustomIcon($"{iconRoot}icon_unmute_p");
            icon_add_sound_r = XGUI.GetCustomIcon($"{iconRoot}icon_add_sound_r");
            icon_add_sound_p = XGUI.GetCustomIcon($"{iconRoot}icon_add_sound_p");
            icon_add_tween_r = XGUI.GetCustomIcon($"{iconRoot}icon_add_tween_r");
            icon_add_tween_p = XGUI.GetCustomIcon($"{iconRoot}icon_add_tween_p");
            // ── 时间飞梭头部图标 ──
            icon_playhead = XGUI.GetCustomIcon($"{iconRoot}icon_playhead");
            icon_led = XGUI.GetBasedIcon("icon_field_status");

            // ── 播放控制图标 ──
            icon_playback_home_r = XGUI.GetCustomIcon($"{iconRoot}icon_playback_home_r");
            icon_playback_home_p = XGUI.GetCustomIcon($"{iconRoot}icon_playback_home_p");
            icon_playback_stepback_r = XGUI.GetCustomIcon($"{iconRoot}icon_playback_stepback_r");
            icon_playback_stepback_p = XGUI.GetCustomIcon($"{iconRoot}icon_playback_stepback_p");
            icon_playback_play_r = XGUI.GetCustomIcon($"{iconRoot}icon_playback_play_r");
            icon_playback_play_p = XGUI.GetCustomIcon($"{iconRoot}icon_playback_play_p");
            icon_playback_stop_r = XGUI.GetCustomIcon($"{iconRoot}icon_playback_stop_r");
            icon_playback_stop_p = XGUI.GetCustomIcon($"{iconRoot}icon_playback_stop_p");
            icon_playback_stepforward_r = XGUI.GetCustomIcon($"{iconRoot}icon_playback_stepforward_r");
            icon_playback_stepforward_p = XGUI.GetCustomIcon($"{iconRoot}icon_playback_stepforward_p");
            icon_playback_end_r = XGUI.GetCustomIcon($"{iconRoot}icon_playback_end_r");
            icon_playback_end_p = XGUI.GetCustomIcon($"{iconRoot}icon_playback_end_p");
        }
        /// <summary> 
        ///Unity 禁用回调：注销 Undo/Redo 监听，并保存视图状态
        /// </summary>
        private void OnDisable()
        {
            try { EditorApplication.playModeStateChanged -= OnPlayModeStateChanged; } catch { }

            //  防御：重编译 / 关闭窗口时，清理可能因对象失效抛异常
            try { EditorApplication.update -= OnPlaybackEditorUpdate; } catch { }
            try { EditorApplication.focusChanged -= OnEditorFocusChanged; } catch { }
            try { Undo.undoRedoPerformed -= OnUndoRedoPerformed; } catch { }

            try { SavePersistedViewState(); } catch { }
            try { SavePersistedWindowSize(); } catch { }

            //  飞梭预览还原
            CleanupPlayheadPreviewOnDisable();

            //  播放状态清理
            CleanupPlaybackOnDisable();
        }
        /// <summary>
        /// Play 模式状态变化回调。
        /// <para/>策略：
        /// <list type="bullet">
        /// <item><description>ExitingEditMode（即将进入 Play）：缓存 target 的 instanceID，置冻结标志；</description></item>
        /// <item><description>EnteredPlayMode：保持冻结，什么都不做；</description></item>
        /// <item><description>ExitingPlayMode（即将退出 Play）：等待场景恢复；</description></item>
        /// <item><description>EnteredEditMode（已回到编辑）：按 instanceID 找原 target，找到就恢复，找不到就关窗。</description></item>
        /// </list>
        /// </summary>
        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.ExitingEditMode:
                    // 即将进入 Play：缓存 target 身份信息
                    if (target != null && !target.Equals(null))
                    {
                        cachedTargetInstanceID = target.GetInstanceID();
                        cachedTargetName = target.name;
                    }
                    else
                    {
                        cachedTargetInstanceID = 0;
                        cachedTargetName = "";
                    }
                    isFrozenByPlayMode = true;
                    Repaint();
                    break;

                case PlayModeStateChange.EnteredPlayMode:
                    // 已进入 Play：保持冻结
                    isFrozenByPlayMode = true;
                    Repaint();
                    break;

                case PlayModeStateChange.ExitingPlayMode:
                    // 即将退出 Play：保持冻结（场景恢复中）
                    isFrozenByPlayMode = true;
                    break;

                case PlayModeStateChange.EnteredEditMode:
                    // 已回到编辑：尝试恢复 target
                    RestoreTargetAfterPlay();
                    break;
            }
        }
        /// <summary>
        /// 退出 Play 后尝试恢复 target。
        /// <para/>通过进入 Play 前缓存的 instanceID 在场景中重新查找同名组件。
        /// <list type="bullet">
        /// <item><description>找到 → 恢复编辑状态；</description></item>
        /// <item><description>找不到 → 关闭窗口。</description></item>
        /// </list>
        /// </summary>
        private void RestoreTargetAfterPlay()
        {
            isFrozenByPlayMode = false;

            if (cachedTargetInstanceID == 0)
            {
                Close();
                return;
            }

            XHud_Module_Primitive_Tween[] allTweens =
                Resources.FindObjectsOfTypeAll<XHud_Module_Primitive_Tween>();

            XHud_Module_Primitive_Tween restored = null;
            for (int i = 0; i < allTweens.Length; i++)
            {
                if (allTweens[i] == null) continue;

                //  不要用 scene.IsValid() 过滤，Prefab Stage 里可能返回 false
                // 直接按 instanceID 匹配——唯一性由 instanceID 保证
                if (allTweens[i].GetInstanceID() == cachedTargetInstanceID)
                {
                    restored = allTweens[i];
                    break;
                }
            }

            if (restored != null)
            {
                target = restored;

                // 清 SO 缓存
                cachedSO = null;
                cachedSO_target = null;
                cachedNodesProp = null;
                cachedSoundsProp = null;
                cachedSOKind = TrackKind.无;
                cachedSOIndex = -1;

                // 清空选中
                selectedIndex = -1;
                selectedNodeIndices.Clear();
                selectedSoundIndices.Clear();
                selectedKind = TrackKind.无;

                RefreshHostComponentCache();
                Repaint();
            }
            else
            {
                Close();
            }

            cachedTargetInstanceID = 0;
            cachedTargetName = "";
        }
        /// <summary> 
        ///Undo / Redo 执行后回调：清拖拽状态，强制重绘
        /// </summary>
        private void OnUndoRedoPerformed()
        {
            dragMode = DragMode.无;
            draggingIndex = -1;

            snapGuideSecond = -1f;
            primaryDragIndex = -1;
            moveDragAnchorSide = 0;

            dragStartNodeDelays.Clear();
            dragStartNodeDurations.Clear();
            dragStartSoundDelays.Clear();
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

            // 校验两个集合的索引合法性
            selectedNodeIndices.RemoveWhere(i => i < 0 || i >= Nodes.Count);
            selectedSoundIndices.RemoveWhere(i => i < 0 || i >= Sounds.Count);

            // 主选中索引修正
            if (selectedKind == TrackKind.Node)
            {
                if (selectedIndex < 0 || selectedIndex >= Nodes.Count)
                    selectedIndex = selectedNodeIndices.Count > 0 ? GetTopmostSelectedIndex(selectedNodeIndices) : -1;
                if (selectedIndex < 0 && selectedSoundIndices.Count > 0)
                {
                    selectedKind = TrackKind.Sound;
                    selectedIndex = GetTopmostSelectedIndex(selectedSoundIndices);
                }
                else if (selectedIndex < 0 && selectedNodeIndices.Count == 0)
                {
                    selectedKind = TrackKind.无;
                }
            }
            else if (selectedKind == TrackKind.Sound)
            {
                if (selectedIndex < 0 || selectedIndex >= Sounds.Count)
                    selectedIndex = selectedSoundIndices.Count > 0 ? GetTopmostSelectedIndex(selectedSoundIndices) : -1;
                if (selectedIndex < 0 && selectedNodeIndices.Count > 0)
                {
                    selectedKind = TrackKind.Node;
                    selectedIndex = GetTopmostSelectedIndex(selectedNodeIndices);
                }
                else if (selectedIndex < 0 && selectedSoundIndices.Count == 0)
                {
                    selectedKind = TrackKind.无;
                }
            }
            else
            {
                // 无主选中：任选一个非空集合做主
                if (selectedNodeIndices.Count > 0)
                {
                    selectedKind = TrackKind.Node;
                    selectedIndex = GetTopmostSelectedIndex(selectedNodeIndices);
                }
                else if (selectedSoundIndices.Count > 0)
                {
                    selectedKind = TrackKind.Sound;
                    selectedIndex = GetTopmostSelectedIndex(selectedSoundIndices);
                }
                else
                {
                    selectedIndex = -1;
                    selectedKind = TrackKind.无;
                }
            }

            //  新增：Undo 后强制重建 SO，避免引用失效
            cachedSO = null;
            cachedSO_target = null;
            cachedNodesProp = null;
            cachedSoundsProp = null;
            cachedSOKind = TrackKind.无;
            cachedSOIndex = -1;

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
            bool valid = target != null
                && !target.Equals(null)
                && target.PrimitiveTweenNodes != null
                && target.PrimitiveTweenNodes.Count > 0;
            #endregion

            #region 绘制：顶部工具栏
            DrawTopToolbar(valid);
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
                    InsertTweenNodeAt(0);
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
                //  从刻度尺下方开始，避免与飞梭头部（刻度尺上）重叠。
                // 若热区纵跨刻度尺，飞梭拖到 0 时鼠标会落在重叠区，
                // 被 HandleNameColumnResizeInput 抢先消费 MouseUp，导致 isDraggingPlayhead 卡住。
                Rect nameResizeZone = new Rect(
                    nameArea.xMax - NameColumnResizeZone,
                    TopBarHeight + rulerHeight,
                    NameColumnResizeZone * 2f,
                    viewHeight - rulerHeight);
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
                HandleMouseScrollWheel();
                #endregion

                #region 交互：名字列宽度输入（必须先于 Clip 区，优先抢 hotControl）
                HandleNameColumnResizeInput(nameResizeZone);
                #endregion

                #region 绘制：三大主区域
                DrawClipTimelineArea(clipArea);
                DrawNameColumnPanel(nameArea);
                DrawNodeParameterPanel(paramArea);
                #endregion

                #region 绘制：名字列宽度光标（必须在所有绘制之后，避免被 Clip 区光标覆盖）
                DrawNameColumnResizeCursor(nameResizeZone);
                #endregion

                #region 绘制：参数面板宽度光标
                DrawParamPanelResizeCursor(paramResizeZone);
                #endregion

                #region 交互：快捷键与全局兜底
                HandleKeyboardShortcuts();
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

        #region 绘制：顶部工具栏
        /// <summary> 
        ///绘制顶部工具栏：重置视图、缩放到合适范围、吸附开关，以及右侧提示文本
        /// </summary>
        private void DrawTopToolbar(bool valid)
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
            //  防御：target 已销毁时用占位名，避免 GetName() 抛异常
            string targetName = (target == null || target.Equals(null)) ? "(已失效)" : target.name;
            XGUI.layout_label(
                text: targetName,
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
                    margin: new RectOffset(10, 0, 0, 0));
                #endregion

                #region 按钮 - 添加音效
                if (XGUI.layout_button(
                    tooltip: "添加一个音效",
                    tex_release: icon_add_sound_r,
                    tex_press: icon_add_sound_p,
                    tex_gui_color: Color.white,
                    absolute_margin: true,
                    absolute_padding: true,
                    margin: new RectOffset(0, 0, 0, 0),
                    border: new RectOffset(0, 0, 0, 0),
                    width: icon_add_sound_r.width,
                    height: TopBarHeight))
                {
                    InsertSoundAt(Sounds.Count);
                }
                #endregion

                #region 分割线
                XGUI.layout_seperator(
                    thickness: 1,
                    dir: XGUISeplineDir.垂直,
                    color: Color.black * 0.4f,
                    padding: new RectOffset(0, 0, 0, 0),
                    margin: new RectOffset(0, 0, 0, 0));
                #endregion

                #region 按钮 - 添加动画
                if (XGUI.layout_button(
                    tooltip: "添加一个动画",
                    tex_release: icon_add_tween_r,
                    tex_press: icon_add_tween_p,
                    tex_gui_color: Color.white,
                    absolute_margin: true,
                    absolute_padding: true,
                    margin: new RectOffset(0, 0, 0, 0),
                    border: new RectOffset(0, 0, 0, 0),
                    width: icon_add_tween_r.width,
                    height: TopBarHeight))
                {
                    InsertTweenNodeAt(Nodes.Count);
                }
                #endregion

                #region 分割线
                XGUI.layout_seperator(
                    thickness: 1,
                    dir: XGUISeplineDir.垂直,
                    color: Color.black * 0.4f,
                padding: new RectOffset(0, 0, 0, 0),
                    margin: new RectOffset(0, 10, 0, 0));
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
                    SavePersistedViewState();
                }
                #endregion

                #region 分割线
                XGUI.layout_seperator(
                    thickness: 1,
                    dir: XGUISeplineDir.垂直,
                    color: Color.black * 0.4f,
                    padding: new RectOffset(0, 0, 0, 0),
                    margin: new RectOffset(0, 10, 0, 0));
                #endregion

                XGUI.layout_label(
                    text: $"<color=#{DisableTextColor}>当前预览时间点：  </color> <color=#{Theme_PrimaryColor}>{playheadSecond}   </color>s",
                    size: XGUIFontSize.M,
                    text_color: Color.white,
                    margin: new RectOffset(20, 10, 0, 0),
                    offset: new Vector2(0, 1),
                    clipping: TextClipping.Clip,
                    width: 150,
                    font_style: FontStyle.Normal,
                    anchor: TextAnchor.MiddleLeft);
            }

            #region 分割线
            XGUI.layout_seperator(
                thickness: 1,
                dir: XGUISeplineDir.垂直,
                color: Color.black * 0.4f,
                padding: new RectOffset(0, 0, 0, 0),
                margin: new RectOffset(10, 10, 0, 0));
            #endregion

            #region 播放控制按钮组
            DrawPlaybackToolbarButtons();
            #endregion

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

        #region 绘制：底部工具栏
        /// <summary> 
        ///绘制窗口底部工具栏
        /// </summary>
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
                text: $"动画轨道：{Nodes.Count}    音效轨道：{Sounds.Count}    选中：{TotalSelectedCount}",
                size: XGUIFontSize.M,
                text_color: Color.white * 0.85f,
                margin: new RectOffset(0, 0, 0, 0),
                offset: new Vector2(0, 0),
                clipping: TextClipping.Clip,
                font_style: FontStyle.Normal,
                anchor: TextAnchor.MiddleLeft,
                font: XGUI.GetFont("xg-regular"));

            #region 分割线
            XGUI.layout_seperator(
                thickness: 1,
                dir: XGUISeplineDir.垂直,
                color: Color.black * 0.4f,
                padding: new RectOffset(0, 0, 0, 0),
                margin: new RectOffset(10, 10, 0, 0));
            #endregion

            XGUI.layout_flexspace();

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
                padding: new RectOffset(0, 0, 1, 0),
                layout_min_width: 0,
                layout_width: 80, button_text_font: XGUI.GetFont("xg-regular")))
            {
                ResetTimelineView();
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
                padding: new RectOffset(0, 0, 1, 0),
                layout_min_width: 0,
                layout_width: 80, button_text_font: XGUI.GetFont("xg-regular")))
            {
                FitViewToContent();
            }
            #endregion
            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);

            GUILayout.EndArea();
        }
        #endregion

        #region 绘制：参数面板
        /// <summary>
        /// 绘制右侧节点参数面板，用于编辑当前选中动画节点 / 音效 / 多选的属性。
        /// <para/>内部使用 GUILayout 布局，高度由 Unity 自动计算，无需再手工估算。
        /// <para/>按选中状态分三种：
        /// <list type="bullet">
        /// <item><description>多选（总数 &gt; 1）：显示多选占位；</description></item>
        /// <item><description>单选动画节点：显示节点参数字段；</description></item>
        /// <item><description>单选音效：显示音效参数字段。</description></item>
        /// </list>
        /// </summary>
        /// <param name="area">参数面板在窗口坐标系中的矩形区域</param>
        private void DrawNodeParameterPanel(Rect area)
        {
            GUI.BeginGroup(area);
            XGUI.gui_box(new Rect(0, 0, area.width, area.height), ColorBasedBg);

            Rect rulerParamRect = new Rect(0, 0, area.width, rulerHeight);
            Rect scrollViewportRect = new Rect(0, rulerHeight, area.width, area.height - rulerHeight);

            // ── 分支 1：多选（跨类型或同类型多选）──
            if (TotalSelectedCount > 1)
            {
                paramScroll = GUILayout.BeginScrollView(
                    paramScroll,
                    false,
                    false,
                    GUIStyle.none,
                    GUI.skin.verticalScrollbar,
                    GUILayout.Width(scrollViewportRect.width),
                    GUILayout.Height(scrollViewportRect.height));

                XGUI.gui_icon(
                    rect: new Rect(scrollViewportRect.x + (scrollViewportRect.width / 2 - 12), scrollViewportRect.y + (scrollViewportRect.height / 2 - 50), 24, 24),
                    icon: icon_param_mixedselected_warning,
                    color: Color.gray * 0.8f);

                XGUI.layout_label(
                        text: $"多选状态下不支持参数编辑\n（{selectedNodeIndices.Count} 动画 / {selectedSoundIndices.Count} 音效）",
                        size: XGUIFontSize.M,
                        text_color: Color.white * 0.75f,
                        margin: new RectOffset(0, 0, 0, 0),
                        offset: new Vector2(0, 20),
                        clipping: TextClipping.Clip,
                        height: scrollViewportRect.height,
                        font_style: FontStyle.Normal,
                        anchor: TextAnchor.MiddleCenter);

                GUILayout.EndScrollView();
            }
            // ── 分支 2 / 3：单选（或有主选中）──
            else if (selectedIndex != -1)
            {
                paramScroll = GUILayout.BeginScrollView(
                    paramScroll,
                    false,
                    false,
                    GUIStyle.none,
                    GUI.skin.verticalScrollbar,
                    GUILayout.Width(scrollViewportRect.width),
                    GUILayout.Height(scrollViewportRect.height));

                if (selectedKind == TrackKind.Node && selectedIndex >= 0 && selectedIndex < Nodes.Count)
                {
                    TweenNode node = Nodes[selectedIndex];
                    Texture2D icon_type = GetTweenTypeIcon(node.Type, true);
                    DrawNodeParameterFields(scrollViewportRect, icon_type, node);
                }
                else if (selectedKind == TrackKind.Sound && selectedIndex >= 0 && selectedIndex < Sounds.Count)
                {
                    DrawSoundParameterFields(scrollViewportRect, Sounds[selectedIndex]);
                }
                else
                {
                    XGUI.gui_icon(
                        rect: new Rect(scrollViewportRect.x + (scrollViewportRect.width / 2 - 12), scrollViewportRect.y + (scrollViewportRect.height / 2 - 50), 24, 24),
                        icon: icon_param_nullselected_warning,
                        color: Color.gray * 0.8f);

                    XGUI.layout_label(
                            text: "点击动画轨道或音效轨道以编辑参数",
                            size: XGUIFontSize.M,
                            text_color: Color.gray * 0.9f,
                            margin: new RectOffset(0, 0, 0, 0),
                            offset: new Vector2(0, 20),
                            clipping: TextClipping.Clip,
                            height: scrollViewportRect.height,
                            font_style: FontStyle.Normal,
                            anchor: TextAnchor.MiddleCenter);
                }

                GUILayout.EndScrollView();
            }
            // ── 分支 4：完全无选中 ──
            else
            {
                paramScroll = GUILayout.BeginScrollView(
                    paramScroll,
                    false,
                    false,
                    GUIStyle.none,
                    GUI.skin.verticalScrollbar,
                    GUILayout.Width(scrollViewportRect.width),
                    GUILayout.Height(scrollViewportRect.height));

                XGUI.gui_icon(
                    rect: new Rect(scrollViewportRect.x + (scrollViewportRect.width / 2 - 12), scrollViewportRect.y + (scrollViewportRect.height / 2 - 50), 24, 24),
                    icon: icon_param_nullselected_warning,
                    color: Color.gray * 0.8f);

                XGUI.layout_label(
                        text: "点击动画轨道或音效轨道以编辑参数",
                        size: XGUIFontSize.M,
                        text_color: Color.gray * 0.9f,
                        margin: new RectOffset(0, 0, 0, 0),
                        offset: new Vector2(0, 20),
                        clipping: TextClipping.Clip,
                        height: scrollViewportRect.height,
                        font_style: FontStyle.Normal,
                        anchor: TextAnchor.MiddleCenter);

                GUILayout.EndScrollView();
            }

            // 点击面板空白处清除键盘焦点（注意：GUILayout 里取鼠标位置仍可用 Event.current）
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && !Event.current.alt && scrollViewportRect.Contains(Event.current.mousePosition))
            {
                GUIUtility.keyboardControl = 0;
                Event.current.Use();
                Repaint();
            }

            // ── 标题：按选中状态切换 ──
            string headerTitle;
            if (TotalSelectedCount > 1)
                headerTitle = "多选参数";
            else if (selectedKind == TrackKind.Sound)
                headerTitle = "音效参数";
            else
                headerTitle = "动画参数";

            XGUI.gui_box(rulerParamRect, Color_Params_Header_BG);
            XGUI.gui_label(
                rect: new Rect(rulerParamRect.x + 6, rulerParamRect.y, rulerParamRect.width, rulerParamRect.height),
                text: new GUIContent(headerTitle),
                text_color: Color.white,
                size: XGUIFontSize.M,
                clipping: TextClipping.Clip,
                anchor: TextAnchor.MiddleLeft,
                font_style: FontStyle.Normal);

            GUI.EndGroup();
            XGUI.gui_box(new Rect(area.x, area.y, 1, area.height), Color.black * 0.35f);
        }
        #endregion

        #region 绘制：公共控件选项
        /// <summary>
        /// 通用方法：绘制带标题的开关控件
        /// </summary>
        private bool DrawLabeledToggle(string title, bool prop, float width, XGUIToggleStyle style = XGUIToggleStyle.实体, Color color_bg_on = default, Color color_bg_off = default, Color color_on = default, Color color_off = default, string[] options = null, Action<bool> act_on_changed = null)
        {
            return XGUI.layout_toggle(
                   title: title,
                   title_size: XGUIFontSize.M,
                   title_font_style: FontStyle.Normal,
                   title_padding: new RectOffset(10, 10, 0, 0),
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

        #region 交互：名字列宽度拖拽
        /// <summary> 
        ///处理名字列右边缘的拖拽输入，用于调整 <see cref="nameColumnWidth"/>
        /// </summary>
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
        /// </summary>
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
        /// </summary>
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
                        SavePersistedViewState();
                        e.Use();
                        Repaint();
                    }
                    break;
            }
        }
        /// <summary> 
        ///在 Repaint 阶段为参数面板左边缘设置水平缩放光标
        /// </summary>
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

        #region 交互：全局兜底
        /// <summary> 
        ///全局兜底：鼠标在窗口任意位置松开左键时，强制收尾 Clip 拖拽与名字列宽度拖拽
        /// </summary>
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
            //  [新增] 飞梭拖拽兜底：即使飞梭 MouseUp 分支被别的控件抢先消费，
            // 这里也能强制收尾，避免 isDraggingPlayhead 卡住导致：
            //   - 飞梭头部持续高亮
            //   - HandlePlaybackShortcuts 吞掉 Space
            if (isDraggingPlayhead)
            {
                isDraggingPlayhead = false;
                GUIUtility.hotControl = 0;
                playheadControlID = 0;
                snapGuideSecond = -1f;

                // 清缓存，让下次进入区间时重建（与飞梭 MouseUp 分支一致）
                OnPlayheadDragEnd();

                handled = true;
            }
            if (dragMode != DragMode.无)
            {
                GUIUtility.hotControl = 0;
                dragMode = DragMode.无;
                draggingKind = TrackKind.无;
                draggingIndex = -1;
                snapGuideSecond = -1f;
                primaryDragIndex = -1;
                moveDragAnchorSide = 0;
                dragStartNodeDelays.Clear();
                dragStartNodeDurations.Clear();
                dragStartSoundDelays.Clear();
                //  这里
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

        #region 交互：快捷键
        /// <summary> 
        ///处理窗口快捷键：Ctrl+D 克隆 / F 缩放到合适范围 / Esc 关闭窗口 / Delete 删除选中节点
        /// </summary>
        private void HandleKeyboardShortcuts()
        {
            Event e = Event.current;
            if (e.type != EventType.KeyDown) return;

            // ── 播放控制快捷键优先处理 ──
            if (HandlePlaybackShortcuts()) return;

            //  新增：拖拽飞梭期间禁用快捷键，避免拖拽中增删节点导致索引错乱
            if (isDraggingPlayhead) return;

            if (e.keyCode == KeyCode.Escape)
            {
                Close();
                e.Use();
                return;
            }

            if (target == null) return;
            if (GUIUtility.keyboardControl != 0) return;

            bool ctrl = e.control || e.command;

            // ── 克隆 ──
            if (ctrl && e.keyCode == KeyCode.D)
            {
                CloneSelectedClips();
                e.Use();
                return;
            }

            // ── 删除 ──
            if (e.keyCode == KeyCode.Delete || e.keyCode == KeyCode.Backspace)
            {
                bool any = false;
                if (selectedNodeIndices.Count > 0)
                {
                    DeleteSelectedTweenNodes();
                    any = true;
                }
                if (selectedSoundIndices.Count > 0)
                {
                    DeleteSelectedSounds();
                    any = true;
                }
                if (any) e.Use();
                return;
            }

            switch (e.keyCode)
            {
                case KeyCode.F:
                    FitViewToContent();
                    e.Use();
                    break;
            }
        }
        #endregion
    }
}