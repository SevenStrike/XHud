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
    using UnityEngine.UI;

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
    public class Editor_XHud_Module_Primitive_Tween_Tracker : EditorWindow
    {
        #region 预览状态
        /// <summary>
        /// 当前是否处于预览状态。
        /// <para/>
        /// 直接读取 <see cref="target"/> 上的 <see cref="XHud_Module_Primitive_Tween.TweenIsPreviewing"/> 字段，
        /// 与 Inspector 共享同一个判断依据，无需额外同步。
        /// </summary>
        private bool IsPreviewing => target != null && target.TweenIsPreviewing;
        #endregion

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
        /// <summary> 
        ///音效 Sound 为空时，Clip 在时间轴上的默认占位长度（秒）
        ///</summary>
        private const float DefaultSoundClipSeconds = 1f;
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

        #region 字段：参数面板必要组件
        private XHud_Module_Text HudText;
        private XHud_Module_TmpText HudTmpText;
        private XHud_Module_Button HudButton;
        private XHud_Module_Progress HudProgress;
        private XHud_Module_Toggle HudToggle;
        private XHud_Module_Slider HudSlider;
        private XHud_Module_Option HudOption;
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
        /// 动画轨与音效轨之间的分隔线颜色
        /// </summary>
        private static readonly Color ColorTrackKindSeparator = XGUI_Utilitys.HexString_To_Color("1e1e1e");
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
        private Texture2D icon_muted_r;
        /// <summary>
        /// 静音图标（按下）
        /// </summary>
        private Texture2D icon_muted_p;
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
        /// <summary> 拖拽开始时，选中的动画节点起始 Delay。 </summary>
        private readonly Dictionary<int, float> dragStartNodeDelays = new Dictionary<int, float>();

        /// <summary> 拖拽开始时，选中的动画节点起始 Duration。 </summary>
        private readonly Dictionary<int, float> dragStartNodeDurations = new Dictionary<int, float>();

        /// <summary> 拖拽开始时，选中的音效起始 Delay。 </summary>
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
        /// 轨道种类，用于区分当前操作的是动画节点轨还是音效轨。
        /// <para/>
        /// - <see cref="Node"/>：对应 <see cref="XHud_Module_Primitive_Tween.PrimitiveTweenNodes"/>
        /// - <see cref="Sound"/>：对应 <see cref="XHud_Module_Primitive_Tween.PrimitiveTweenSounds"/>
        /// 两者是平行列表，无父子关系。
        /// </summary>
        private enum TrackKind
        {
            无,
            Node,
            Sound
        }
        #endregion

        #region 窗口入口
        /// <summary> 
        ///打开（或复用）迷你时间轴窗口，并绑定指定的图元动画器
        /// </summary>
        /// <param name="tween">要编辑的图元动画器</param>
        public static void OpenWith(XHud_Module_Primitive_Tween tween)
        {
            Editor_XHud_Module_Primitive_Tween_Tracker window = (Editor_XHud_Module_Primitive_Tween_Tracker)EditorWindow.GetWindow(typeof(Editor_XHud_Module_Primitive_Tween_Tracker), false, "XHUD 图元动画轨道编辑器", true);

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
                window.LoadPersistedViewState();
                window.RefreshHostComponentCache();
            }
            window.Repaint();
            window.Focus();
        }
        /// <summary> 
        ///根据当前 target 刷新其所在的 XHud 宿主组件缓存
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
            Undo.undoRedoPerformed -= OnUndoRedoPerformed;
            Undo.undoRedoPerformed += OnUndoRedoPerformed;
            icon_param_nullselected_warning = XGUI.GetBasedIcon("icon_warning");
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
            b_anim_type_move = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/b_anim_type_move");
            b_anim_type_rotate = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/b_anim_type_rotate");
            b_anim_type_scale = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/b_anim_type_scale");
            b_anim_type_color = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/b_anim_type_color");
            b_anim_type_fade = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/b_anim_type_fade");
            b_anim_type_writter = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/b_anim_type_writter");
            b_anim_type_fill = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/b_anim_type_fill");
            b_anim_type_size = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/b_anim_type_size");
            icon_track_param_record_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/icon_track_param_record_r");
            icon_track_param_record_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/icon_track_param_record_p");
            icon_track_param_apply_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/icon_track_param_apply_r");
            icon_track_param_apply_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/icon_track_param_apply_p");
            icon_track_param_reset_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/icon_track_param_reset_r");
            icon_track_param_reset_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/icon_track_param_reset_p");
            icon_track_param_connector_status_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/icon_track_param_connector_status_r");
            icon_track_param_connector_status_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/icon_track_param_connector_status_p");
            icon_muted_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/icon_muted_r");
            icon_muted_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/icon_muted_p");
            icon_unmute_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/icon_unmute_r");
            icon_unmute_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/icon_unmute_p");


            icon_led = XGUI.GetBasedIcon("icon_field_status");
        }
        /// <summary> 
        ///Unity 禁用回调：注销 Undo/Redo 监听，并保存视图状态
        /// </summary>
        private void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoRedoPerformed;
            SavePersistedViewState();
            SavePersistedWindowSize();
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

                #region 按钮 - 添加音效
                if (XGUI.layout_button(
                    text: "添加音效",
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
                    layout_width: 80, button_text_font: XGUI.GetFont("xg-regular")))
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
                    SavePersistedViewState();
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

        #region 行号映射
        /// <summary>
        /// 全局行总数 = 动画节点数 + 音效数。
        /// <para/>
        /// 行号顺序：先动画节点（0..Nodes.Count-1），后音效（Nodes.Count..Total-1）。
        /// </summary>
        private int TotalRowCount => Nodes.Count + Sounds.Count;

        /// <summary>
        /// 将全局行号解析为 (轨道种类, 列表内索引)。
        /// <para/>
        /// 约定：
        /// <list type="bullet">
        /// <item><description><c>row &lt; Nodes.Count</c> → (Node, row)</description></item>
        /// <item><description>否则 → (Sound, row - Nodes.Count)</description></item>
        /// </list>
        /// </summary>
        private (TrackKind kind, int index) ResolveRow(int row)
        {
            if (row < Nodes.Count) return (TrackKind.Node, row);
            return (TrackKind.Sound, row - Nodes.Count);
        }

        /// <summary>
        /// 由全局行号计算行的命中矩形（内容坐标）。
        /// <para/>
        /// 上下边界取整，避免行高为小数时出现 1px 缝隙或重叠。
        /// </summary>
        private Rect GetRowHitRect(int row, float width)
        {
            float y0 = Mathf.Round(row * trackHeight);
            float y1 = Mathf.Round((row + 1) * trackHeight);
            return new Rect(0, y0, width, y1 - y0);
        }
        #endregion

        #region 绘制：名字列
        /// <summary> 
        /// 绘制左侧名字列（固定面板，垂直滚动由 Clip 区主导）
        /// <para/>
        /// 布局自上而下分为三部分：
        /// <list type="bullet">
        /// <item><description>顶部标题栏（高 <see cref="rulerHeight"/>）：显示「图元动画列表」；</description></item>
        /// <item><description>中部行列表（ScrollView）：每行对应一个动画节点，含类型圆点、
        /// 标识文字、插入 / 删除 / 显隐 / 菜单四个小按钮；</description></item>
        /// <item><description>底部滚动条占位条（高 <see cref="HorizontalScrollbarHeight"/>）：
        /// 与 Clip 区水平滚动条等高，保证两列底边对齐。</description></item>
        /// </list>
        /// <para/>
        /// 关键设计：名字列自身不处理垂直滚动，其 <c>nameScroll.y</c> 每帧从
        /// <see cref="scrollPos"/>.y 同步，从而实现与右侧轨道的垂直对齐。
        /// <para/>
        /// 增删操作采用「延迟执行」模式：循环中只记录 <c>pendingInsertIndex</c> /
        /// <c>pendingDeleteIndex</c>，循环结束后再统一调用增删方法，
        /// 避免在遍历 <see cref="Nodes"/> 过程中修改集合引发异常。
        /// </summary>
        /// <param name="area">名字列在窗口坐标系中的矩形区域</param>
        private void DrawNameColumnPanel(Rect area)
        {
            GUI.BeginGroup(area);
            XGUI.gui_box(new Rect(0, 0, area.width, area.height), ColorBasedBg);

            // ── 顶部标题栏占位（先铺底，文字最后叠加，避免被行列表覆盖）──
            Rect rect_name = new Rect(0, 0, area.width, rulerHeight);
            XGUI.gui_box(rect_name, ColorBasedBg);

            // ── 中部行列表视口：扣除顶部标题栏与底部滚动条占位条 ──
            Rect scrollViewportRect = new Rect(
                0,
                rulerHeight,
                area.width,
                area.height - rulerHeight - HorizontalScrollbarHeight);

            // 内容高度 = 行数 × 行高 + 20px 底部留白（与 Clip 区保持一致，保证滚动范围对齐）
            float contentWidth = scrollViewportRect.width;
            float contentHeight = TotalRowCount * trackHeight + 20f;

            // 关键：nameScroll.y 每帧从 scrollPos.y 同步，实现与右侧轨道垂直对齐。
            // 两个方向滚动条均隐藏（false, false），因为名字列不接受独立滚动输入。
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
            int pendingSoundInsertIndex = -1;
            int pendingSoundDeleteIndex = -1;

            for (int row = 0; row < TotalRowCount; row++)
            {
                var (kind, idx) = ResolveRow(row);

                Rect rowRect = GetRowHitRect(row, scrollViewportRect.width);
                Rect nameRect = CalculateNameRowVisualRect(rowRect);

                if (kind == TrackKind.Node)
                {
                    DrawNodeNameRow(rowRect, nameRect, idx,
                        ref nameHitThisFrame,
                        ref pendingInsertIndex,
                        ref pendingDeleteIndex);
                }
                else
                {
                    DrawSoundNameRow(rowRect, nameRect, idx,
                        ref nameHitThisFrame,
                        ref pendingSoundInsertIndex,
                        ref pendingSoundDeleteIndex);
                }
            }

            // ★ 新增：名字列分隔线
            DrawNameColumnKindSeparator(scrollViewportRect.width);

            GUI.EndScrollView();

            // 同步回写：防止 GUI.BeginScrollView 修改 nameScroll.y 后污染下一帧
            nameScroll.y = scrollPos.y;

            // ── 底部滚动条占位条：与 Clip 区水平滚动条等高，保证两列底边对齐 ──
            Rect bottomStrip = new Rect(
                0,
                area.height - HorizontalScrollbarHeight,
                area.width,
                HorizontalScrollbarHeight);
            EditorGUI.DrawRect(bottomStrip, ColorBasedBg);

            // ── 名字列空白处点击：清空选中 ──
            // 条件：左键、非 Alt、非行内控件命中、且点击落在中部行列表视口内。
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0
                && !Event.current.alt && !nameHitThisFrame
                && scrollViewportRect.Contains(Event.current.mousePosition))
            {
                selectedNodeIndices.Clear();
                selectedSoundIndices.Clear();
                selectedIndex = -1;
                selectedKind = TrackKind.无;
                GUIUtility.keyboardControl = 0;
                Event.current.Use();
                Repaint();
            }

            // ── 顶部标题栏：最后叠加绘制，避免被行列表覆盖 ──
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

            // 名字列右边界分隔线（视觉上区分名字列与 Clip 区）
            XGUI.gui_box(new Rect(area.x + (area.width - 1), area.y, 1, area.height), Color.black * 0.35f);

            if (pendingDeleteIndex >= 0)
            {
                DeleteTweenNodeAt(pendingDeleteIndex);
            }
            else if (pendingInsertIndex >= 0)
            {
                InsertTweenNodeAfter(pendingInsertIndex);
            }
            else if (pendingSoundDeleteIndex >= 0)
            {
                DeleteSoundAt(pendingSoundDeleteIndex);
            }
            else if (pendingSoundInsertIndex >= 0)
            {
                InsertSoundAfter(pendingSoundInsertIndex);
            }
        }
        /// <summary>
        /// 在名字列中绘制动画行与音效行之间的分隔线。
        /// <para/>
        /// 与 Clip 区的分隔线同高，保证左右两栏视觉连贯。
        /// </summary>
        private void DrawNameColumnKindSeparator(float width)
        {
            if (Nodes.Count == 0 || Sounds.Count == 0) return;

            float sepY = Mathf.Round(Nodes.Count * trackHeight);

            // 名字列视口高度用于裁剪
            float viewTop = scrollPos.y;
            float viewBottom = scrollPos.y + (cachedNameAreaRect.height - rulerHeight - HorizontalScrollbarHeight);
            if (sepY < viewTop || sepY > viewBottom) return;

            XGUI.gui_box(new Rect(0, sepY, width, 2f), ColorTrackKindSeparator);
        }
        /// <summary>
        /// 绘制单条动画节点名字行。
        /// <para/>
        /// 这是从原 <see cref="DrawNameColumnPanel"/> 循环体中抽出的逻辑，
        /// 内容与原来完全一致，只是把 <c>i</c> 换成了参数 <paramref name="index"/>，
        /// 把 <c>SetNodeSelection</c> 换成了 <c>SetSelection(TrackKind.Node, ...)</c>，
        /// 把 <c>selectedIndices.Contains(i)</c> 换成了 <c>IsSelected(TrackKind.Node, index)</c>。
        /// </summary>
        private void DrawNodeNameRow(Rect rowRect, Rect nameRect, int index, ref bool nameHitThisFrame, ref int pendingInsertIndex, ref int pendingDeleteIndex)
        {
            TweenNode node = Nodes[index];
            bool isSelected = IsSelected(TrackKind.Node, index);

            // 行背景
            EditorGUI.DrawRect(nameRect, isSelected ? ColorNameRowSelectedBg : ColorNameRowBg);

            // 四个小按钮自右向左排列
            float btnY = nameRect.y + (nameRect.height - NameRowButtonSize) * 0.5f - 1;
            float dis = 3;

            Rect menuRect = new Rect(nameRect.xMax - NameRowButtonSize - 5, btnY, NameRowButtonSize, NameRowButtonSize);
            Rect eyeRect = new Rect(menuRect.x - NameRowButtonSize - dis, btnY, NameRowButtonSize, NameRowButtonSize);
            Rect deleteRect = new Rect(eyeRect.x - NameRowButtonSize - dis, btnY, NameRowButtonSize, NameRowButtonSize);
            Rect insertRect = new Rect(deleteRect.x - NameRowButtonSize - dis, btnY, NameRowButtonSize, NameRowButtonSize);

            Rect sepRect = new Rect(insertRect.x - dis - 2, nameRect.y, 1, nameRect.height + 2);

            // 标识文字
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

            // ── 按钮：插入 ──
            if (XGUI.gui_button(
                rect: insertRect, tooltip: "",
                tex_release: icon_add_r, tex_press: icon_add_p,
                tex_gui_color: Color.white,
                width: NameRowButtonSize, height: NameRowButtonSize,
                border: new RectOffset(0, 0, 0, 0),
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0)))
            {
                pendingInsertIndex = index;
                nameHitThisFrame = true;
            }

            // ── 按钮：删除 ──
            if (XGUI.gui_button(
                rect: deleteRect, tooltip: "",
                tex_release: icon_del_r, tex_press: icon_del_p,
                tex_gui_color: new Color(0.8f, 0.32f, 0.32f, 1),
                width: NameRowButtonSize, height: NameRowButtonSize,
                border: new RectOffset(0, 0, 0, 0),
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0)))
            {
                pendingDeleteIndex = index;
                nameHitThisFrame = true;
            }

            // ── 按钮：显隐 ──
            if (XGUI.gui_button(
                rect: eyeRect, tooltip: "",
                tex_release: node.Enabled ? icon_enabled_r : icon_disabled_r,
                tex_press: node.Enabled ? icon_enabled_p : icon_disabled_p,
                tex_gui_color: node.Enabled ? Color.white : Color.gray * 0.85f,
                width: NameRowButtonSize, height: NameRowButtonSize,
                border: new RectOffset(0, 0, 0, 0),
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0)))
            {
                node.Enabled = !node.Enabled;
                nameHitThisFrame = true;
            }

            // ── 按钮：菜单 ──
            if (XGUI.gui_button(
                rect: menuRect, tooltip: "",
                tex_release: icon_menu_r, tex_press: icon_menu_p,
                tex_gui_color: Color.white,
                width: NameRowButtonSize, height: NameRowButtonSize,
                border: new RectOffset(0, 0, 0, 0),
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0)))
            {
                nameHitThisFrame = true;
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("A"), false, () => { });
                menu.AddItem(new GUIContent("B"), false, () => { });
                menu.AddItem(new GUIContent("C"), false, () => { });
                menu.ShowAsContext();
            }

            // ── 类型圆点 ──
            XGUI.gui_icon(
                rect: new Rect(nameRect.x + 10, nameRect.y + ((nameRect.height / 2) - TweenTypeDotSize / 2), TweenTypeDotSize, TweenTypeDotSize),
                icon: icon_led,
                color: GetTweenTypeColor(node.Type));

            // ── 行本体点击 ──
            if (Event.current.type == EventType.MouseDown
                && Event.current.button == 0
                && rowRect.Contains(Event.current.mousePosition)
                && !nameHitThisFrame)
            {
                bool ctrl = Event.current.control || Event.current.command;
                bool shift = Event.current.shift;
                SetSelection(TrackKind.Node, index, ctrl, shift);
                nameHitThisFrame = true;
                Event.current.Use();
                Repaint();
            }
        }
        /// <summary>
        /// 绘制单条音效名字行。
        /// <para/>
        /// 与节点行的差异：
        /// <list type="bullet">
        /// <item><description>无"显隐"按钮（<see cref="TweenSound"/> 没有 Enabled）；</description></item>
        /// <item><description>标识文字用音频名或"(空音效)"；</description></item>
        /// <item><description>左侧圆点固定青绿色。</description></item>
        /// </list>
        /// </summary>
        private void DrawSoundNameRow(Rect rowRect, Rect nameRect, int index, ref bool nameHitThisFrame, ref int pendingInsertIndex, ref int pendingDeleteIndex)
        {
            TweenSound sound = Sounds[index];
            bool isSelected = IsSelected(TrackKind.Sound, index);

            EditorGUI.DrawRect(nameRect, isSelected ? ColorNameRowSelectedBg : ColorNameRowBg);

            float btnY = nameRect.y + (nameRect.height - NameRowButtonSize) * 0.5f - 1;
            float dis = 3;

            // 音效只有三个按钮：菜单 / 删除 / 插入
            Rect menuRect = new Rect(nameRect.xMax - NameRowButtonSize - 5, btnY, NameRowButtonSize, NameRowButtonSize);
            Rect muteRect = new Rect(menuRect.x - NameRowButtonSize - dis, btnY, NameRowButtonSize, NameRowButtonSize);
            Rect deleteRect = new Rect(muteRect.x - NameRowButtonSize - dis, btnY, NameRowButtonSize, NameRowButtonSize);
            Rect insertRect = new Rect(deleteRect.x - NameRowButtonSize - dis, btnY, NameRowButtonSize, NameRowButtonSize);

            Rect sepRect = new Rect(insertRect.x - dis - 2, nameRect.y, 1, nameRect.height + 2);

            float buttonZoneLeft = insertRect.x - 35;
            string label = sound.Sound != null ? sound.Sound.name : "(空音效)";
            XGUI.gui_label(
                rect: new Rect(nameRect.x + 25, nameRect.y, buttonZoneLeft - nameRect.x, nameRect.height),
                text: new GUIContent(label),
                text_color: Color.white * 0.9f,
                size: XGUIFontSize.B,
                clipping: XGUI.TryEllipsisClipping(),
                anchor: TextAnchor.MiddleLeft,
                font_style: FontStyle.Normal);

            XGUI.gui_box(sepRect, Color.white * 0.35f);

            // ── 按钮：插入 ──
            if (XGUI.gui_button(
                rect: insertRect, tooltip: "",
                tex_release: icon_add_r, tex_press: icon_add_p,
                tex_gui_color: Color.white,
                width: NameRowButtonSize, height: NameRowButtonSize,
                border: new RectOffset(0, 0, 0, 0),
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0)))
            {
                pendingInsertIndex = index;
                nameHitThisFrame = true;
            }

            // ── 按钮：删除 ──
            if (XGUI.gui_button(
                rect: deleteRect, tooltip: "",
                tex_release: icon_del_r, tex_press: icon_del_p,
                tex_gui_color: new Color(0.8f, 0.32f, 0.32f, 1),
                width: NameRowButtonSize, height: NameRowButtonSize,
                border: new RectOffset(0, 0, 0, 0),
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0)))
            {
                pendingDeleteIndex = index;
                nameHitThisFrame = true;
            }

            // ── 按钮：静音 ──
            if (XGUI.gui_button(
                rect: muteRect, tooltip: "",
                tex_release: sound.Mute ? icon_unmute_r : icon_muted_r,
                tex_press: sound.Mute ? icon_unmute_p : icon_muted_p,
                tex_gui_color: sound.Mute ? Color.gray * 0.85f : Color.white,
                width: NameRowButtonSize, height: NameRowButtonSize,
                border: new RectOffset(0, 0, 0, 0),
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0)))
            {
                sound.Mute = !sound.Mute;
                nameHitThisFrame = true;
            }

            // ── 按钮：菜单 ──
            if (XGUI.gui_button(
                rect: menuRect, tooltip: "",
                tex_release: icon_menu_r, tex_press: icon_menu_p,
                tex_gui_color: Color.white,
                width: NameRowButtonSize, height: NameRowButtonSize,
                border: new RectOffset(0, 0, 0, 0),
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0)))
            {
                nameHitThisFrame = true;
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("选择音频资源"), false, () => { });
                menu.ShowAsContext();
            }

            // ── 类型圆点（音效固定青绿色）──
            XGUI.gui_icon(
                rect: new Rect(nameRect.x + 10, nameRect.y + ((nameRect.height / 2) - TweenTypeDotSize / 2), TweenTypeDotSize, TweenTypeDotSize),
                icon: icon_led,
                color: new Color(0.4f, 0.8f, 0.8f));

            // ── 行本体点击 ──
            if (Event.current.type == EventType.MouseDown
                && Event.current.button == 0
                && rowRect.Contains(Event.current.mousePosition)
                && !nameHitThisFrame)
            {
                bool ctrl = Event.current.control || Event.current.command;
                bool shift = Event.current.shift;
                SetSelection(TrackKind.Sound, index, ctrl, shift);
                nameHitThisFrame = true;
                Event.current.Use();
                Repaint();
            }
        }
        /// <summary> 
        /// 计算第 <paramref name="index"/> 条名字行的命中矩形（内容坐标）
        /// <para/>
        /// 「命中矩形」是完整行高（含上下 <see cref="RowVerticalPadding"/> 内边距），
        /// 用于鼠标点击判定；视觉矩形请使用 <see cref="CalculateNameRowVisualRect"/>。
        /// <para/>
        /// 上下边界均做 <see cref="Mathf.Round"/> 取整，原因：行高 <see cref="trackHeight"/>
        /// 可能为小数（Shift+滚轮调整后），若不做取整，相邻行的边界会出现 1px 缝隙
        /// 或重叠，视觉上出现细线。
        /// </summary>
        /// <param name="index">节点索引</param>
        /// <param name="width">名字列可用宽度</param>
        /// <returns>行命中矩形（内容坐标）</returns>
        private Rect CalculateNameRowHitRect(int index, float width)
        {
            // 行高可能为小数，上下边界分别取整，保证相邻行无缝衔接
            float y0 = Mathf.Round(index * trackHeight);
            float y1 = Mathf.Round((index + 1) * trackHeight);
            return new Rect(0, y0, width, y1 - y0);
        }
        /// <summary> 
        /// 由行命中矩形计算名字行的视觉矩形：上下按 <see cref="RowVerticalPadding"/> 内缩
        /// <para/>
        /// 与命中矩形的关系：
        /// <list type="bullet">
        /// <item><description>命中矩形：完整行高，用于鼠标点击判定；</description></item>
        /// <item><description>视觉矩形：上下各内缩 <see cref="RowVerticalPadding"/>，
        /// 用于行背景、按钮、文字等的绘制。</description></item>
        /// </list>
        /// 内缩后若高度小于 4px（行高极小时的边界保护），强制设为 4px，
        /// 避免 <see cref="EditorGUI.DrawRect"/> 绘制出 0 或负高度的无效矩形。
        /// </summary>
        /// <param name="rowRect">行命中矩形</param>
        /// <returns>行视觉矩形</returns>
        private Rect CalculateNameRowVisualRect(Rect rowRect)
        {
            // 上下各内缩 pad，形成行与行之间的视觉间隔
            float pad = RowVerticalPadding;
            return new Rect(
                rowRect.x,
                rowRect.y + pad,
                rowRect.width,
                // 高度下限保护：避免极行高下出现 0 / 负高度矩形
                Mathf.Max(rowRect.height - pad * 2f, 4f));
        }
        #endregion

        #region 绘制：Clip 区
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
            EditorGUI.DrawRect(new Rect(0, 0, area.width, area.height), ColorBasedBg);

            // ── 阶段 1：绘制顶部刻度尺（固定不随 ScrollView 滚动）──
            // 可见时间范围 = [水平滚动偏移, 水平滚动偏移 + 视口宽度] / 每秒像素数
            Rect rulerRect = new Rect(0, 0, area.width, rulerHeight);
            float startSecond = scrollPos.x / pixelsPerSecond;
            float endSecond = (scrollPos.x + area.width) / pixelsPerSecond;
            DrawTimeRuler(rulerRect, startSecond, endSecond);

            // ── 阶段 2：开启双向 ScrollView ──
            // 视口 = 扣除顶部刻度尺与底部水平滚动条后的剩余区域
            Rect scrollViewportRect = new Rect(
                0, rulerHeight, area.width,
                area.height - rulerHeight - HorizontalScrollbarHeight);

            float contentWidth = CalculateContentWidthPixels();
            float contentHeight = TotalRowCount * trackHeight + 20f;
            Rect contentRect = new Rect(0, 0, contentWidth, contentHeight);

            scrollPos = GUI.BeginScrollView(
                scrollViewportRect,
                scrollPos,
                contentRect,
                false,
                false,
                GUIStyle.none,
                GUI.skin.verticalScrollbar);

            // ── 阶段 3：逐条绘制轨道（带垂直裁剪）──
            // clipHitThisFrame 用于记录本帧是否有 Clip 被左键命中，
            // 供后续 HandleClipAreaEmptyClick 判断是否需要取消选中。
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
                else
                    DrawSoundTrackRow(trackRect, idx);
            }

            // ── 阶段 3.5：绘制动画轨与音效轨的分隔线 ──
            DrawTrackKindSeparator(
                contentWidth,
                scrollPos.y,
                scrollViewportRect.height);

            // ── 阶段 4：在 ScrollView 内处理 Clip 拖拽 ──
            // 必须放在 EndScrollView 之前，因为 MouseDrag / MouseUp 事件
            // 依赖于 ScrollView 内部的鼠标坐标系。
            ProcessClipDrag(scrollViewportRect);
            GUI.EndScrollView();

            // ── 阶段 5：关闭 ScrollView 后钳制垂直滚动 ──
            // 同步 nameScroll.y，保证左侧名字列与右侧轨道垂直对齐。
            scrollPos.y = Mathf.Clamp(scrollPos.y, 0f, CalculateMaxVerticalScroll());
            nameScroll.y = scrollPos.y;

            #region 参考线（覆盖刻度尺 + 轨道区）
            // 吸附参考线：拖拽中且本帧吸附到有效时间时，绘制一条白色竖线（按住 Shift 更亮）
            if (dragMode != DragMode.无 && snapGuideSecond >= 0f)
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
            #endregion

            // ── 阶段 6：绘制底部自绘水平滚动条 ──
            // Unity 内置 HorizontalScrollbar 样式不符合本编辑器视觉，故手工绘制。
            Rect hScrollRect = new Rect(0, area.height - HorizontalScrollbarHeight, area.width, HorizontalScrollbarHeight);
            DrawTimelineHorizontalScrollbar(hScrollRect);

            // ── 阶段 7：三类交互处理 ──
            // 顺序不可调换：刻度尺点击 → 空白点击 → 平移，后者需能覆盖前者的 hotControl 占用。
            HandleTimeRulerClick(new Rect(0, 0, area.width, rulerHeight));
            HandleClipAreaEmptyClick(scrollViewportRect);
            HandleViewPan(new Rect(0, 0, area.width, area.height));

            GUI.EndGroup();
        }
        /// <summary>
        /// 在动画节点与音效之间绘制一条水平分隔线（内容坐标）。
        /// <para/>
        /// 位置 = Nodes.Count 行号的顶部，即第 N-1 行与第 N 行之间。
        /// 只有两类轨道都非空时才绘制，否则没有区分意义。
        /// </summary>
        private void DrawTrackKindSeparator(float contentWidth, float viewTop, float viewHeight)
        {
            if (Nodes.Count == 0 || Sounds.Count == 0) return;

            float sepY = Mathf.Round(Nodes.Count * trackHeight);
            if (sepY < viewTop - 4 || sepY > viewTop + viewHeight + 4) return;

            // 分隔带：上下各 1px 深色，中间 2px 亮线
            XGUI.gui_box(new Rect(0, sepY - 1f, contentWidth, 5f), Color.black * 0.5f);
            XGUI.gui_box(new Rect(0, sepY, contentWidth, 2f), ColorTrackKindSeparator);
            XGUI.gui_box(new Rect(0, sepY + 2f, contentWidth, 5f), Color.black * 0.5f);
        }
        /// <summary> 
        /// 计算 Clip 区内容矩形的宽度（像素）
        /// <para/>
        /// 内容宽度 = max(所有 Clip 的最右端时间, <see cref="MinContentSeconds"/>) × 每秒像素数
        /// + 右侧额外留白 <see cref="ContentRightPaddingPixels"/>。
        /// <para/>
        /// 最后再与「视口宽度 - 垂直滚动条宽度」取 max，原因：
        /// <list type="bullet">
        /// <item><description>内容宽度若小于视口宽度，Unity ScrollView 会以内容宽度为准，
        /// 导致轨道背景无法铺满整个视口，视觉上出现右侧空白；</description></item>
        /// <item><description>预留 <c>verticalScrollbarWidth</c> 是为了给垂直滚动条留出空间，
        /// 避免滚动条与内容重叠。</description></item>
        /// </list>
        /// 本方法在每帧绘制、水平滚动条绘制、Clip 拖拽自动平移等多处被调用，
        /// 因此内部只做必要的遍历，不做缓存。
        /// </summary>
        /// <returns>内容宽度（像素）</returns>
        private float CalculateContentWidthPixels()
        {
            // ── 步骤 1：遍历所有节点，取最右端时间 ──
            float maxEnd = 0f;
            if (Nodes != null)
            {
                for (int i = 0; i < Nodes.Count; i++)
                {
                    float end = Nodes[i].Delay + Nodes[i].Duration;
                    if (end > maxEnd) maxEnd = end;
                }
            }
            if (Sounds != null)
            {
                for (int i = 0; i < Sounds.Count; i++)
                {
                    TweenSound s = Sounds[i];
                    float len = s.Sound != null ? s.Sound.length : DefaultSoundClipSeconds;
                    float end = s.Delay + len;
                    if (end > maxEnd) maxEnd = end;
                }
            }

            // ── 步骤 2：内容时间下限 + 右侧留白 → 候选宽度 ──
            // MinContentSeconds 保证节点为空或全在 0 时刻时仍有可滚动范围，
            // 否则内容宽度会退化为「右侧留白」这一小块，时间轴几乎无法操作。
            float seconds = Mathf.Max(maxEnd, MinContentSeconds);
            float width = seconds * pixelsPerSecond + ContentRightPaddingPixels;

            // ── 步骤 3：与「视口宽度 - 垂直滚动条宽度」取 max ──
            // 让内容至少铺满视口（扣掉滚动条），避免右侧出现背景空白。
            float viewportWidth = cachedClipAreaRect.width;
            const float verticalScrollbarWidth = 16f;
            float minWidth = Mathf.Max(1f, viewportWidth - verticalScrollbarWidth);
            return Mathf.Max(width, minWidth);
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
            // 内容与视口宽度：二者共同决定是否需要绘制滑块
            float contentWidth = CalculateContentWidthPixels();
            float viewWidth = cachedClipAreaRect.width;

            // ── 分支 1：内容不超过视口 → 只铺底，不绘制滑块 ──
            if (contentWidth <= viewWidth)
            {
                EditorGUI.DrawRect(rect, new Color(0.15f, 0.15f, 0.15f));
                return;
            }

            // ── 分支 2：内容超过视口 → 绘制标准 Unity 水平滚动条 ──
            // 先铺底，再绘制滑块，保证滑块两侧的「空白槽」颜色与底一致。
            EditorGUI.DrawRect(rect, new Color(0.15f, 0.15f, 0.15f));
            scrollPos.x = GUI.HorizontalScrollbar(
                rect,
                scrollPos.x,   // 当前值（滚动偏移）
                viewWidth,     // 可视区域宽度（决定了滑块长度）
                0f,            // 最小值
                contentWidth); // 最大值

            // 归零保护：GUI.HorizontalScrollbar 在边界情况下可能返回极小负值。
            scrollPos.x = Mathf.Max(0f, scrollPos.x);
        }
        #endregion

        #region 绘制：刻度尺
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
        #endregion

        #region 绘制：轨道与 Clip
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
                    rect: new Rect(clipRect.x + 15, clipRect.y + (trackHeight / 2 - 7), 10, 10),
                    icon: GetTweenTypeIcon(node.Type),
                    color: isSelected ? Color.black : Color.white * 0.9f);

                XGUI.gui_icon(
                    rect: new Rect(clipRect.x + (clipRect.width - 25), clipRect.y + (trackHeight / 2 - 7), 10, 10),
                    icon: GetTweenTypeIcon(node.Type),
                    color: isSelected ? Color.black : Color.white * 0.9f);
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
                    typeicon = big_mode ? b_anim_type_rotate : icon_type_rotator;
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
        #endregion

        #region 绘制：音效轨
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

                GUILayout.Space(6);
                GUILayout.Label($"已选中 {TotalSelectedCount} 项", EditorStyles.miniLabel);
                GUILayout.Label($"（{selectedNodeIndices.Count} 动画 / {selectedSoundIndices.Count} 音效）", EditorStyles.miniLabel);
                GUILayout.Space(4);
                GUILayout.Label("多选状态下不显示单字段编辑。", EditorStyles.miniLabel);
                GUILayout.Label("可按 Delete 删除，或拖动整组平移。", EditorStyles.miniLabel);

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
        /// <summary>
        /// 绘制选中音效的参数字段。
        /// <para/>
        /// 与 <see cref="DrawNodeParameterFields"/> 对称，但字段集是音效专属的：
        /// Sound / Path / Timings / Delay / Volume / MinPitch / MaxPitch。
        /// </summary>
        private void DrawSoundParameterFields(Rect area, TweenSound sound)
        {
            SerializedObject so = new SerializedObject(target);

            SerializedProperty prop_sounds = so.FindProperty("PrimitiveTweenSounds");
            if (prop_sounds == null || selectedIndex < 0 || selectedIndex >= prop_sounds.arraySize)
            {
                return;
            }

            SerializedProperty prop_sound = prop_sounds.GetArrayElementAtIndex(selectedIndex);
            SerializedProperty ser_sound = prop_sound.FindPropertyRelative("Sound");
            SerializedProperty ser_path = prop_sound.FindPropertyRelative("Path");
            SerializedProperty ser_timing = prop_sound.FindPropertyRelative("Timing");
            SerializedProperty ser_delay = prop_sound.FindPropertyRelative("Delay");
            SerializedProperty ser_volume = prop_sound.FindPropertyRelative("Volume");
            SerializedProperty ser_minPitch = prop_sound.FindPropertyRelative("MinPitch");
            SerializedProperty ser_maxPitch = prop_sound.FindPropertyRelative("MaxPitch");

            so.Update();

            XGUI.ChangedCheck_Start();

            #region 参数
            XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.Theme_Group,
                title: "参数",
                title_text_color: XHud_Dashboard.Theme_Primary,
                title_clipping: XGUI.TryEllipsisClipping(),
                title_size: XGUIFontSize.M,
                title_manual_offset: true,
                title_manual_offset_space: 5,
                absolute_margin: true,
                absolute_padding: true,
                margin: new RectOffset(10, 10, 25, 0),
                padding: new RectOffset(10, 10, 15, 15));

            #region 音效资源
            XGUI.layout_property_field(
                title: "音效",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.Theme_Primary,
                title_width: 80,
                prop: ser_sound,
                prop_padding: new RectOffset(5, 5, 0, 5),
                prop_margin: new RectOffset(5, 5, 0, 0));
            ser_sound.serializedObject.ApplyModifiedProperties();
            #endregion

            #region 路径（只读显示 + 按钮：从音效资源反查）
            XGUI.layout_property_field(
                title: "路径",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.Theme_Primary,
                title_width: 80,
                prop: ser_path,
                prop_padding: new RectOffset(5, 5, 0, 5),
                prop_margin: new RectOffset(5, 5, 0, 0));
            ser_path.serializedObject.ApplyModifiedProperties();
            #endregion

            #region 时机
            string[] TimingTypes = null;

            if (HudButton != null)
            {
                TimingTypes = new string[9] { "无", "鼠标进入", "鼠标退出", "鼠标按下", "鼠标松开", "鼠标长按", "鼠标点击", "鼠标选中", "鼠标取消选中" };
            }
            else if (HudProgress != null)
            {
                TimingTypes = new string[5] { "无", "进度开始时", "进度变化时", "进度结束时", "进度重置时" };
            }
            else if (HudToggle != null)
            {
                TimingTypes = new string[7] { "无", "开关打开时", "开关关闭时", "开关按下时", "开关抬起时", "开关变化时", "开关变化中" };
            }
            else if (HudSlider != null)
            {
                TimingTypes = new string[5] { "无", "按下滑动条", "松开滑动条", "滑动条数值改变", "滑动条数值变化中" };
            }
            else if (HudOption != null)
            {
                TimingTypes = new string[5] { "无", "点击选项", "光标移动开始", "光标移动结束", "光标位置改变" };
            }
            else
            {
                TimingTypes = new string[4] { "元素进入时", "元素进入后", "元素退出时", "自定义" };
            }

            ser_timing.stringValue = XGUI.layout_string_popup(
                   title: "时机",
                   title_width: 80,
                   title_size: XGUIFontSize.M,
                   title_anchor: TextAnchor.MiddleLeft,
                   prop: ser_timing,
                   options: TimingTypes,
                   opt_text_size: XGUIFontSize.M,
                   opt_text_color: Color.black,
                   opt_text_padding: new RectOffset(10, 10, 0, 0),
                   opt_anchor: TextAnchor.MiddleLeft,
                   opt_font_style: FontStyle.Normal,
                   opt_bg_fill: XGUIFilled.实体,
                   opt_bg_color: XGUIColor.亮白,
                   opt_bg_color_gui: XHud_Dashboard.Theme_Primary,
                   margin: new RectOffset(0, 0, 0, 0),
                   padding: new RectOffset(10, 9, 0, 5),
                   title_margin: new RectOffset(0, 0, 0, 0),
                   icon_arrow_color: Color.black);

            ser_timing.serializedObject.ApplyModifiedProperties();
            #endregion

            #region 延迟
            XGUI.layout_property_field(
                title: "延迟",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.Theme_Primary,
                title_width: 80,
                prop: ser_delay,
                prop_padding: new RectOffset(5, 5, 0, 5),
                prop_margin: new RectOffset(5, 5, 0, 0));
            ser_delay.serializedObject.ApplyModifiedProperties();
            #endregion

            #region 音量
            XGUI.layout_property_field(
                title: "音量",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.Theme_Primary,
                title_width: 80,
                prop: ser_volume,
                prop_padding: new RectOffset(5, 5, 0, 5),
                prop_margin: new RectOffset(5, 5, 0, 0));
            ser_volume.serializedObject.ApplyModifiedProperties();
            #endregion

            #region 音高
            XGUI.layout_property_field(
                title: "最小音高",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.Theme_Primary,
                title_width: 80,
                prop: ser_minPitch,
                prop_padding: new RectOffset(5, 5, 0, 5),
                prop_margin: new RectOffset(5, 5, 0, 0));
            ser_minPitch.serializedObject.ApplyModifiedProperties();

            XGUI.layout_property_field(
                title: "最大音高",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.Theme_Primary,
                title_width: 80,
                prop: ser_maxPitch,
                prop_padding: new RectOffset(5, 5, 0, 5),
                prop_margin: new RectOffset(5, 5, 0, 0));
            ser_maxPitch.serializedObject.ApplyModifiedProperties();
            #endregion

            XGUI.layout_space(5);

            #region 静音
            sound.Mute = DrawLabeledToggle("静音", sound.Mute, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, new string[] { "禁用", "启用" }, (b) => { });
            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 时长（只读）
            XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.Theme_Group,
                title: "信息",
                title_text_color: XHud_Dashboard.Theme_Primary,
                title_size: XGUIFontSize.M,
                title_manual_offset: true,
                title_manual_offset_space: 5,
                absolute_margin: true,
                absolute_padding: true,
                margin: new RectOffset(10, 10, 20, 0),
                padding: new RectOffset(10, 10, 15, 15));

            string lenText = sound.Sound != null ? $"{sound.Sound.length:F3} 秒" : "—";
            XGUI.layout_state_displayer_text(
                title: "音频时长",
                title_size: XGUIFontSize.M,
                subtitle: lenText,
                subtitle_size: XGUIFontSize.M,
                subtitle_color: XHud_Dashboard.Theme_Primary,
                margin: new RectOffset(5, 5, 0, 5));

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            XGUI.layout_space(6);

            if (XGUI.ChangedCheck_End())
            {
                if (dragMode == DragMode.无 && !IsPreviewing)
                    Undo.RecordObject(target, "Edit Tween Sound");
                sound.GetSoundPath();
                EditorUtility.SetDirty(target);
                Repaint();
            }

            so.ApplyModifiedProperties();
        }
        /// <summary> 
        ///绘制选中节点的参数字段
        ///<para/>使用 EditorGUILayout 自动布局；按 <see cref="TweenNode.Type"/> 动态显示起始值 / 结束值
        /// </summary>
        private void DrawNodeParameterFields(Rect area, Texture2D type_icon, TweenNode node)
        {
            SerializedObject so = new SerializedObject(target);

            #region 序列化字段
            SerializedProperty prop_node = so.FindProperty("PrimitiveTweenNodes").GetArrayElementAtIndex(selectedIndex);
            SerializedProperty ser_indicator = prop_node.FindPropertyRelative("Indicator");
            SerializedProperty ser_type = prop_node.FindPropertyRelative("Type");
            SerializedProperty ser_duration = prop_node.FindPropertyRelative("Duration");
            SerializedProperty ser_delay = prop_node.FindPropertyRelative("Delay");
            SerializedProperty ser_timing = prop_node.FindPropertyRelative("Timings");
            SerializedProperty ser_loop_type = prop_node.FindPropertyRelative("LoopType");
            SerializedProperty ser_loop_count = prop_node.FindPropertyRelative("LoopCount");
            SerializedProperty ser_ease = prop_node.FindPropertyRelative("Ease");
            SerializedProperty ser_curve = prop_node.FindPropertyRelative("Curve");
            SerializedProperty ser_rotate_mode = prop_node.FindPropertyRelative("RotateMode");
            #endregion

            so.Update();

            #region 类型图标
            //XGUI.layout_group_start(
            //    type: XGUIContainerType.Horizontal,
            //    bg_fill: XGUIFilled.透明,
            //    bg_color: XGUIColor.亮白,
            //    absolute_margin: true,
            //    absolute_padding: true,
            //    margin: new RectOffset(10, 10, 25, 10),
            //    padding: new RectOffset(0, 0, 0, 0));

            //XGUI.layout_icon(
            //    icon: type_icon,
            //    icon_color: Color.white * 0.7f,
            //    width: 32,
            //    height: 32,
            //    icon_border: new RectOffset(0, 0, 0, 0),
            //    icon_offset: new Vector2(0, 0),
            //    icon_margin: new RectOffset(0, 0, 0, 0),
            //    icon_padding: new RectOffset(0, 0, 0, 0),
            //    icon_alignment: XGUIIconAlignment.中心);

            //XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            #endregion

            XGUI.ChangedCheck_Start();

            #region 参数
            XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.Theme_Group,
                title: "参数",
                title_text_color: XHud_Dashboard.Theme_Primary,
                title_clipping: XGUI.TryEllipsisClipping(),
                title_size: XGUIFontSize.M,
                title_manual_offset: true,
                title_manual_offset_space: 5,
                absolute_margin: true,
                absolute_padding: true,
                margin: new RectOffset(10, 10, 25, 0),
                padding: new RectOffset(10, 10, 15, 15));

            #region 标识
            XGUI.layout_property_field(
              title: "标识",
              title_size: XGUIFontSize.M,
              title_hover_color: XHud_Dashboard.Theme_Primary,
              title_width: 80,
              prop: ser_indicator,
              prop_padding: new RectOffset(5, 5, 0, 5),
              prop_margin: new RectOffset(5, 5, 0, 0));
            ser_indicator.serializedObject.ApplyModifiedProperties();
            #endregion

            #region 类型
            XGUI.layout_property_field(
               title: "类型",
               title_size: XGUIFontSize.M,
               title_hover_color: XHud_Dashboard.Theme_Primary,
               title_width: 80,
               prop: ser_type,
               prop_padding: new RectOffset(5, 5, 0, 5),
              prop_margin: new RectOffset(5, 5, 0, 0));
            ser_type.serializedObject.ApplyModifiedProperties();
            #endregion

            #region 时机
            string[] TimingTypes = null;

            if (HudButton != null)
            {
                TimingTypes = new string[9] { "无", "鼠标进入", "鼠标退出", "鼠标按下", "鼠标松开", "鼠标长按", "鼠标点击", "鼠标选中", "鼠标取消选中" };
            }
            else if (HudProgress != null)
            {
                TimingTypes = new string[5] { "无", "进度开始时", "进度变化时", "进度结束时", "进度重置时" };
            }
            else if (HudToggle != null)
            {
                TimingTypes = new string[7] { "无", "开关打开时", "开关关闭时", "开关按下时", "开关抬起时", "开关变化时", "开关变化中" };
            }
            else if (HudSlider != null)
            {
                TimingTypes = new string[5] { "无", "按下滑动条", "松开滑动条", "滑动条数值改变", "滑动条数值变化中" };
            }
            else if (HudOption != null)
            {
                TimingTypes = new string[5] { "无", "点击选项", "光标移动开始", "光标移动结束", "光标位置改变" };
            }
            else
            {
                TimingTypes = new string[4] { "元素进入时", "元素进入后", "元素退出时", "自定义" };
            }

            ser_timing.stringValue = XGUI.layout_string_popup(
                   title: "时机",
                   title_width: 80,
                   title_size: XGUIFontSize.M,
                   title_anchor: TextAnchor.MiddleLeft,
                   prop: ser_timing,
                   options: TimingTypes,
                   opt_text_size: XGUIFontSize.M,
                   opt_text_color: Color.black,
                   opt_text_padding: new RectOffset(10, 10, 0, 0),
                   opt_anchor: TextAnchor.MiddleLeft,
                   opt_font_style: FontStyle.Normal,
                   opt_bg_fill: XGUIFilled.实体,
                   opt_bg_color: XGUIColor.亮白,
                   opt_bg_color_gui: XHud_Dashboard.Theme_Primary,
                   margin: new RectOffset(0, 0, 0, 0),
                   padding: new RectOffset(10, 9, 0, 5),
                   title_margin: new RectOffset(0, 0, 0, 0),
                   icon_arrow_color: Color.black);

            ser_timing.serializedObject.ApplyModifiedProperties();
            #endregion

            #region 耗时
            XGUI.layout_property_field(
                title: "耗时",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.Theme_Primary,
                title_width: 80,
                prop: ser_duration,
                prop_padding: new RectOffset(5, 5, 0, 5),
                prop_margin: new RectOffset(5, 5, 0, 0));
            ser_duration.serializedObject.ApplyModifiedProperties();
            #endregion

            #region 延迟
            XGUI.layout_property_field(
                title: "延迟",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.Theme_Primary,
                title_width: 80,
                prop: ser_delay,
                prop_padding: new RectOffset(5, 5, 0, 5),
                prop_margin: new RectOffset(5, 5, 0, 0));
            ser_delay.serializedObject.ApplyModifiedProperties();
            #endregion

            #region 缓动
            XGUI.layout_property_field(
                title: "缓动",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.Theme_Primary,
                title_width: 80,
                prop: ser_ease,
                prop_padding: new RectOffset(5, 5, 0, 5),
                prop_margin: new RectOffset(0, 0, 0, 0));
            ser_ease.serializedObject.ApplyModifiedProperties();
            #endregion

            #region 旋转模式
            if (node.Type == TweenNodeType.r_旋转)
            {
                XGUI.layout_property_field(
                    title: "旋转模式",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 80,
                    prop: ser_rotate_mode,
                    prop_padding: new RectOffset(5, 5, 0, 5),
                    prop_margin: new RectOffset(0, 0, 0, 0));
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
                prop_padding: new RectOffset(5, 5, 0, 5),
                prop_margin: new RectOffset(0, 0, 0, 0));
            ser_loop_type.serializedObject.ApplyModifiedProperties();
            #endregion

            #region 循环次数
            XGUI.layout_property_field(
                title: "循环次数",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.Theme_Primary,
                title_width: 80,
                prop: ser_loop_count,
                prop_padding: new RectOffset(5, 5, 0, 5),
                prop_margin: new RectOffset(0, 0, 0, 0));
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
                    prop_padding: new RectOffset(5, 5, 0, 5),
                    prop_margin: new RectOffset(0, 0, 0, 0));
                ser_curve.serializedObject.ApplyModifiedProperties();
            }
            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 起始值 / 结束值（按类型动态显示）
            XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.Theme_Group,
                title: "数值",
                title_text_color: XHud_Dashboard.Theme_Primary,
                title_clipping: XGUI.TryEllipsisClipping(),
                title_size: XGUIFontSize.M,
                title_manual_offset: true,
                title_manual_offset_space: 5,
                absolute_margin: true,
                absolute_padding: true,
                margin: new RectOffset(10, 10, 20, 0),
                padding: new RectOffset(10, 10, 15, 10),
                can_foldout: false);

            #region 动画数值过渡模式选项卡
            int value_mode_index = node.ValueModeIndex = XGUI.layout_toolbar(
                      index: node.ValueModeIndex,
                      names: new string[] { "S - D", "D - E", "S - E", "C - E" },
                      bg_normal: XGUIFilled.纯色边框,
                      bg_selected: XGUIFilled.实体,
                      bg_color: XGUIColor.亮白,
                      bg_gui_color: Color.black * 0.5f,
                      text_color_normal: Color.white,
                      text_color_selected: XHud_Dashboard.Theme_Primary,
                      bar_height: 25,
                      text_anchor: TextAnchor.MiddleCenter,
                      text_padding: new RectOffset(10, 10, 0, 0),
                      bar_margin: new RectOffset(0, 0, 5, 5),
                      text_offset: new Vector2(0, -2),
                      text_font: XGUI.GetFont("xg-regular"),
                      text_fontstyle: FontStyle.Bold,
                      bg_width_offset: 5,
                      bg_height_offset: 2,
                      navigate_style: true,
                      navigate_style_bg: XGUIFilled.纯色边框,
                      navigate_style_bg_color: Color.black * 0.5f);
            #endregion

            // 数值模式枚举值同步
            node.TweenValueMode = (TweenValueMode)value_mode_index;

            #region 模式说明按钮（弹出说明弹窗）
            Rect rect_toolbar = XGUI.GetLastRect();
            if (XGUI.gui_button(
                rect: new Rect(rect_toolbar.x + (rect_toolbar.width - icon_help_r.width) + 5, rect_toolbar.y - icon_help_r.height - 2, icon_help_r.width - 5, icon_help_r.height - 5),
                tooltip: "",
                tex_release: icon_help_r,
                tex_press: icon_help_p,
                tex_gui_color: Color.white,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0)))
            {
                EditorApplication.delayCall += () =>
                {
                    List<XGUIDialogListDatas> infos = new List<XGUIDialogListDatas>();

                    string hexcolor = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary);

                    infos.Add(new XGUIDialogListDatas($"起始 - 默认", $"<b><color=#{hexcolor}>S</color></b>  -  <b><color=#{hexcolor}>D</color></b>", $"<color=#c1c1c1>从</color>  起始值  <color=#c1c1c1>到</color>  默认值  <color=#c1c1c1>的动画</color>"));
                    infos.Add(new XGUIDialogListDatas($"默认 - 结束", $"<b><color=#{hexcolor}>D</color></b>  -  <b><color=#{hexcolor}>E</color></b>", $"<color=#c1c1c1>从</color>  默认值  <color=#c1c1c1>到</color>  结束值  <color=#c1c1c1>的动画</color>"));
                    infos.Add(new XGUIDialogListDatas($"起始 - 结束", $"<b><color=#{hexcolor}>S</color></b>  -  <b><color=#{hexcolor}>E</color></b>", $"<color=#c1c1c1>从</color>  起始值  <color=#c1c1c1>到</color>  结束值  <color=#c1c1c1>的动画</color>"));
                    infos.Add(new XGUIDialogListDatas($"当前 - 结束", $"<b><color=#{hexcolor}>C</color></b>  -  <b><color=#{hexcolor}>E</color></b>", $"<color=#c1c1c1>从</color>  当前值  <color=#c1c1c1>到</color>  结束值  <color=#c1c1c1>的动画</color>"));

                    XGUI.dialog_listview(
                        datas: infos.ToArray(),
                        type: XGUIDialogType.通知,
                        windowtitle: "XHud - 图元动画时间线编辑器消息",
                        title: "数值模式说明",
                        msg: "以下是数值模式的简码对应的解释",
                        ok: "明白",
                        PrimaryIndex: 0,
                        show_index: false,
                        usemodal: false,
                        themecolor: XHud_Dashboard.Theme_Primary);
                };
            }
            #endregion

            XGUI.layout_space(5);

            // 获取动画类型
            TweenNodeType node_type = (TweenNodeType)ser_type.enumValueIndex;

            // 绘制动画数值控件逻辑
            DrawTweenValueEditor(ResolveTweenValueProperties(node_type, prop_node), value_mode_index);

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
            node.Enabled = DrawLabeledToggle("动画开关", node.Enabled, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, new string[] { "禁用", "启用" }, (b) => { });
            #endregion

            #region 重置设为起始值
            node.Rewind_Set_Startvalue = DrawLabeledToggle("重置设为起始值", node.Rewind_Set_Startvalue, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, new string[] { "禁用", "启用" }, (b) => { });
            #endregion

            #region 完成设为结束值
            node.Complete_Set_Endvalue = DrawLabeledToggle("完成设为结束值", node.Complete_Set_Endvalue, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, new string[] { "禁用", "启用" }, (b) => { });
            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            XGUI.layout_space(6);

            if (XGUI.ChangedCheck_End())
            {
                if (dragMode == DragMode.无)
                {
                    if (!IsPreviewing)
                        Undo.RecordObject(target, "Edit Tween Node");
                }
                EditorUtility.SetDirty(target);
                Repaint();
            }

            so.ApplyModifiedProperties();
        }
        /// <summary>
        /// 绘制动画数值过渡模式的编辑器主体（左/右数值输入框 + 交换按钮）
        /// </summary>
        private void DrawTweenValueEditor(TweenValueProperties val_propertys, int value_mode_index)
        {
            bool only_end_mode = (value_mode_index == 3);

            if (!only_end_mode)
                XGUI.layout_group_start(
                    type: XGUIContainerType.Horizontal,
                    absolute_margin: true,
                    absolute_padding: true,
                    margin: new RectOffset(0, 0, 0, 0),
                    padding: new RectOffset(0, 0, 0, 0),
                    can_foldout: false);

            if (!only_end_mode)
                XGUI.layout_space(5);

            // 数值流向指示器图表式按钮
            if (!only_end_mode)
                if (XGUI.layout_button(
                tooltip: "点击交换数值",
                tex_release: icon_track_param_connector_status_r,
                tex_press: icon_track_param_connector_status_p,
                tex_gui_color: XHud_Dashboard.Theme_Primary,
                margin: new RectOffset(0, 10, 10, 0),
                border: new RectOffset(0, 0, 3, 8),
                width: icon_track_param_connector_status_r.width,
                height: 92))
                {
                    // 根据模式进行数值交换对调
                    switch (value_mode_index)
                    {
                        case 0:
                            SwapValueProperties(val_propertys.prop_from, val_propertys.prop_origin);
                            break;
                        case 1:
                            SwapValueProperties(val_propertys.prop_origin, val_propertys.prop_end);
                            break;
                        case 2:
                            SwapValueProperties(val_propertys.prop_from, val_propertys.prop_end);
                            break;
                    }
                }

            XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                absolute_margin: true,
                absolute_padding: true,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                can_foldout: false);

            /* value_mode_index = 动画数值过渡模式
             *  0  => 起始 - 默认
             *  1  => 默认 - 结束
             *  2  => 起始 - 结束
             *  3  => 当前 - 结束
             * */

            string title_source = null;
            string title_target = null;

            SerializedProperty prop_source = null;
            SerializedProperty prop_target = null;

            switch (value_mode_index)
            {
                case 0:
                    // 起始 - 默认
                    title_source = "起始";
                    title_target = "默认";
                    prop_source = val_propertys.prop_from;
                    prop_target = val_propertys.prop_origin;
                    break;
                case 1:
                    // 默认 - 结束
                    title_source = "默认";
                    title_target = "结束";
                    prop_source = val_propertys.prop_origin;
                    prop_target = val_propertys.prop_end;
                    break;
                case 2:
                    // 起始 - 结束
                    title_source = "起始";
                    title_target = "结束";
                    prop_source = val_propertys.prop_from;
                    prop_target = val_propertys.prop_end;
                    break;
                case 3:
                    // 当前 - 结束
                    title_source = "结束";
                    prop_source = val_propertys.prop_end;
                    break;
            }

            // 控件绘制 - 起源值
            DrawTweenValueFields(
                title_source,
                prop_source,
                // 按钮动作 - 将目标物体的类型属性数据记录到序列化属性中
                () => { RecordValueFromTarget(prop_source, val_propertys.type); },
                // 按钮动作 - 将序列化属性的值应用到目标物体的类型属性数据上
                () => { ApplyValueToTarget(prop_source, val_propertys.type); },
                // 按钮动作 - 重置序列化属性数据
                () => { ResetValueProperty(prop_source); });

            // 如果 value_mode_index != 3 的时候说明动画都是从“起源值”到“目标值”，所以才会出现这第二个输入框
            if (!only_end_mode)
            {
                XGUI.layout_seperator(
                    thickness: 1,
                    color: XHud_Dashboard.Theme_SeperateLine,
                    margin: new RectOffset(15, 15, 8, 12));

                // 控件绘制 - 目标值
                DrawTweenValueFields(
                    title_target,
                    prop_target,
                    // 按钮动作 - 将目标物体的类型属性数据记录到序列化属性中
                    () => { RecordValueFromTarget(prop_target, val_propertys.type); },
                    // 按钮动作 - 将序列化属性的值应用到目标物体的类型属性数据上
                    () => { ApplyValueToTarget(prop_target, val_propertys.type); },
                    // 按钮动作 - 重置序列化属性数据
                    () => { ResetValueProperty(prop_target); });
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);

            if (!only_end_mode)
                XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
        }
        /// <summary>
        /// 绘制单个数值字段（输入框 + 记录 / 应用 / 归零三个操作按钮）
        /// </summary>
        private void DrawTweenValueFields(string title, SerializedProperty prop, Action act_on_pressed_record = null, Action act_on_pressed_apply = null, Action act_on_pressed_reset = null)
        {
            #region 数值输入框
            XGUI.layout_property_field(
                       title: title,
                       title_size: XGUIFontSize.M,
                       title_hover_color: XHud_Dashboard.Theme_Primary,
                       title_width: 50,
                       title_color: XHud_Dashboard.Theme_Primary,
                       title_offset: new Vector2(0, 0),
                       title_font_style: FontStyle.Normal,
                       prop: prop,
                       prop_padding: new RectOffset(5, 5, 0, 0),
                       prop_margin: new RectOffset(0, 0, 0, 0));
            prop.serializedObject.ApplyModifiedProperties();

            #region 按钮操作区域
            XGUI.layout_group_start(
            type: XGUIContainerType.Horizontal,
            absolute_margin: true,
            absolute_padding: true,
            margin: new RectOffset(0, 0, 10, 0),
            padding: new RectOffset(10, 10, 0, 0));

            #region 记录当前物体
            if (XGUI.layout_button(
                tooltip: "记录当前物体",
                tex_release: icon_track_param_record_r,
                tex_press: icon_track_param_record_p,
                tex_gui_color: Color.white,
                margin: new RectOffset(10, 10, 0, 0),
                border: new RectOffset(0, 0, 0, 0),
                width: 40,
                height: 40))
            {
                if (act_on_pressed_record != null)
                    act_on_pressed_record();
            }
            #endregion

            XGUI.layout_flexspace();

            #region 应用到物体
            if (XGUI.layout_button(
                tooltip: "应用到物体",
                tex_release: icon_track_param_apply_r,
                tex_press: icon_track_param_apply_p,
                tex_gui_color: Color.white,
                margin: new RectOffset(10, 10, 0, 0),
                border: new RectOffset(0, 0, 0, 0),
                width: 40,
                height: 40))
            {
                if (act_on_pressed_apply != null)
                    act_on_pressed_apply();
            }
            #endregion

            XGUI.layout_flexspace();

            #region 当前物体对应类型值归零
            if (XGUI.layout_button(
                tooltip: "当前物体对应类型值归零",
                tex_release: icon_track_param_reset_r,
                tex_press: icon_track_param_reset_p,
                tex_gui_color: Color.white,
                margin: new RectOffset(10, 10, 0, 0),
                border: new RectOffset(0, 0, 0, 0),
                width: 40,
                height: 40))
            {
                if (act_on_pressed_reset != null)
                    act_on_pressed_reset();
            }
            #endregion


            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            #endregion
            #endregion
        }
        /// <summary>
        /// 根据动画类型解析出其对应的「起始 / 结束 / 默认」三组序列化属性
        /// </summary>
        private TweenValueProperties ResolveTweenValueProperties(TweenNodeType type, SerializedProperty prop)
        {
            #region 数值类型序列化获取- Vector4
            SerializedProperty ori_v4 = prop.FindPropertyRelative("Original_Vector4");
            SerializedProperty from_v4 = prop.FindPropertyRelative("From_Vector4");
            SerializedProperty end_v4 = prop.FindPropertyRelative("End_Vector4");
            #endregion

            #region 数值类型序列化获取- Vector3
            SerializedProperty ori_v3 = prop.FindPropertyRelative("Original_Vector3");
            SerializedProperty from_v3 = prop.FindPropertyRelative("From_Vector3");
            SerializedProperty end_v3 = prop.FindPropertyRelative("End_Vector3");
            #endregion

            #region 数值类型序列化获取- Vector2
            SerializedProperty ori_v2 = prop.FindPropertyRelative("Original_Vector2");
            SerializedProperty from_v2 = prop.FindPropertyRelative("From_Vector2");
            SerializedProperty end_v2 = prop.FindPropertyRelative("End_Vector2");
            #endregion

            #region 数值类型序列化获取- Color
            SerializedProperty ori_color = prop.FindPropertyRelative("Original_Color");
            SerializedProperty from_color = prop.FindPropertyRelative("From_Color");
            SerializedProperty end_color = prop.FindPropertyRelative("End_Color");
            #endregion

            #region 数值类型序列化获取- String
            SerializedProperty ori_string = prop.FindPropertyRelative("Original_String");
            SerializedProperty from_string = prop.FindPropertyRelative("From_String");
            SerializedProperty end_string = prop.FindPropertyRelative("End_String");
            #endregion

            #region 数值类型序列化获取- Float
            SerializedProperty ori_float = prop.FindPropertyRelative("Original_Float");
            SerializedProperty from_float = prop.FindPropertyRelative("From_Float");
            SerializedProperty end_float = prop.FindPropertyRelative("End_Float");
            #endregion

            #region 数值类型序列化获取- Int
            SerializedProperty ori_int = prop.FindPropertyRelative("Original_Int");
            SerializedProperty from_int = prop.FindPropertyRelative("From_Int");
            SerializedProperty end_int = prop.FindPropertyRelative("End_Int");
            #endregion

            TweenValueProperties value_propertys = new TweenValueProperties();

            // 第 1 步：统一设置 title（去掉枚举名前两个字符 "x_"）
            value_propertys.title = type.ToString().Substring(2);

            // 第 2 步：按数据类型分组设置值
            switch (type)
            {
                case TweenNodeType.a_位移:
                case TweenNodeType.r_旋转:
                case TweenNodeType.s_缩放:
                    value_propertys.prop_origin = ori_v3;
                    value_propertys.prop_from = from_v3;
                    value_propertys.prop_end = end_v3;
                    break;
                case TweenNodeType.c_颜色:
                    value_propertys.prop_origin = ori_color;
                    value_propertys.prop_from = from_color;
                    value_propertys.prop_end = end_color;
                    break;
                case TweenNodeType.g_淡化:
                case TweenNodeType.f_图像填充:
                    value_propertys.prop_origin = ori_float;
                    value_propertys.prop_from = from_float;
                    value_propertys.prop_end = end_float;
                    break;
                case TweenNodeType.w_打字机:
                    value_propertys.prop_origin = ori_string;
                    value_propertys.prop_from = from_string;
                    value_propertys.prop_end = end_string;
                    break;
                case TweenNodeType.z_尺寸:
                    value_propertys.prop_origin = ori_v2;
                    value_propertys.prop_from = from_v2;
                    value_propertys.prop_end = end_v2;
                    break;
            }

            value_propertys.type = type;

            // 返回的类中包含了目标类型动画的序列化数据（引用）
            return value_propertys;
        }
        /// <summary>
        /// 从目标物体的当前属性读值并写入指定序列化属性
        /// <para/>
        /// 使用方式：参数面板中「记录当前物体」按钮点击时调用。
        /// 与 <see cref="ApplyValueToTarget"/> 互为逆操作：
        /// <list type="bullet">
        /// <item><description><c>RecordValueFromTarget</c>：目标物体 → 序列化属性（本方法）；</description></item>
        /// <item><description><c>ApplyValueToTarget</c>：序列化属性 → 目标物体。</description></item>
        /// </list>
        /// 本方法只读取运行时组件的当前值，不做 Undo 记录（读取不修改任何对象）。
        /// </summary>
        /// <param name="p">目标序列化属性，按 <paramref name="node_type"/> 决定写入的类型分支</param>
        /// <param name="node_type">动画节点类型，决定从哪个组件 / 字段读取当前值</param>
        private void RecordValueFromTarget(SerializedProperty p, TweenNodeType node_type)
        {
            switch (node_type)
            {
                // ── 位移：读取 RectTransform.anchoredPosition3D ──
                case TweenNodeType.a_位移:
                    p.vector3Value = target.controller.mod_Rect.anchoredPosition3D;
                    break;

                // ── 旋转：读取 RectTransform.localEulerAngles（欧拉角）──
                case TweenNodeType.r_旋转:
                    p.vector3Value = target.controller.mod_Rect.localEulerAngles;
                    break;

                // ── 缩放：读取 RectTransform.localScale ──
                case TweenNodeType.s_缩放:
                    p.vector3Value = target.controller.mod_Rect.localScale;
                    break;

                // ── 颜色：读取 Graphic.color ──
                case TweenNodeType.c_颜色:
                    // 通过 Controller 识别目标实际挂载的图形类型（Image / RawImage / Text 等），
                    // RecognizeType() 返回对应的 Graphic 基类实例，供统一取色。
                    Graphic gc = target.controller.RecognizeType();

                    p.colorValue = gc.color;
                    break;

                // ── 淡化：读取 CanvasGroup.alpha ──
                case TweenNodeType.g_淡化:
                    p.floatValue = target.controller.mod_CanvasGroup.alpha;
                    break;

                // ── 打字机：读取 Text / TmpText 的内容 ──
                // 注意：Text 与 TmpText 互斥，优先 Text，回退 TmpText；
                // 两者都为 null 时保持属性原值不变。
                case TweenNodeType.w_打字机:
                    if (target.controller.mod_Text != null)
                    {
                        p.stringValue = target.controller.mod_Text.text;
                    }
                    else if (target.controller.mod_TmpText != null)
                    {
                        p.stringValue = target.controller.mod_TmpText.text;
                    }
                    break;

                // ── 图像填充：读取 Image.fillAmount ──
                case TweenNodeType.f_图像填充:
                    p.floatValue = target.controller.mod_Image.fillAmount;
                    break;

                // ── 尺寸：读取 RectTransform.sizeDelta ──
                case TweenNodeType.z_尺寸:
                    p.vector2Value = target.controller.mod_Rect.sizeDelta;
                    break;
            }

            // 将序列化属性的改动立即写回 SerializedObject，
            // 否则参数面板上的输入框不会同步刷新。
            p.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
        /// <summary>
        /// 将指定序列化属性的值写回到目标物体的对应属性上
        /// <para/>
        /// 使用方式：参数面板中「应用到物体」按钮点击时调用。
        /// 每个分支都会先用 <see cref="Undo.RecordObject"/> 记录被修改对象，
        /// 以支持 Ctrl+Z 撤销；同时按 <paramref name="node_type"/> 决定操作目标。
        /// </summary>
        /// <param name="p">来源序列化属性（已由 <see cref="RecordValueFromTarget"/> 或手动编辑填入值）</param>
        /// <param name="node_type">动画节点类型，决定值应写回哪个组件 / 字段</param>
        private void ApplyValueToTarget(SerializedProperty p, TweenNodeType node_type)
        {
            switch (node_type)
            {
                // ── 位移：写入 RectTransform.anchoredPosition3D ──
                case TweenNodeType.a_位移:
                    if (!IsPreviewing)
                        Undo.RecordObject(target.controller.mod_Rect, "undotransform-position");
                    target.controller.mod_Rect.anchoredPosition3D = p.vector3Value;
                    break;

                // ── 旋转：写入 RectTransform.localEulerAngles（欧拉角）──
                case TweenNodeType.r_旋转:
                    if (!IsPreviewing)
                        Undo.RecordObject(target.controller.mod_Rect, "undotransform-eulerangle");
                    target.controller.mod_Rect.localEulerAngles = p.vector3Value;
                    break;

                // ── 缩放：写入 RectTransform.localScale ──
                case TweenNodeType.s_缩放:
                    if (!IsPreviewing)
                        Undo.RecordObject(target.controller.mod_Rect, "undotransform-localscale");
                    target.controller.mod_Rect.localScale = p.vector3Value;
                    break;

                // ── 颜色：写入 Graphic.color，并同步刷新 Control 上缓存的 OriginalColor ──
                case TweenNodeType.c_颜色:
                    // 通过 Controller 识别目标实际挂载的图形类型（Image / RawImage / Text 等），
                    // RecognizeType() 会返回对应的 Graphic 基类实例，供统一写色。
                    Graphic gc = target.controller.RecognizeType();

                    // 同步记录 Control 内部的 OriginalColor 缓存，避免下次动画运行时
                    // 用旧的 OriginalColor 覆盖用户刚刚写入的颜色。
                    if (!IsPreviewing)
                        Undo.RecordObject(target.controller.pt_Painting, "undocolor-origin");

                    ModuleType x_Type = target.controller.GetModuleType();
                    if (x_Type == ModuleType.Image)
                        target.controller.pt_Painting.OriginalColor = p.colorValue;
                    else if (x_Type == ModuleType.RawImage)
                        target.controller.pt_Painting.OriginalColor = p.colorValue;

                    gc.color = p.colorValue;
                    break;

                // ── 淡化：写入 CanvasGroup.alpha ──
                case TweenNodeType.g_淡化:
                    if (!IsPreviewing)
                        Undo.RecordObject(target.controller.mod_CanvasGroup, "undoAlpha");
                    target.controller.mod_CanvasGroup.alpha = p.floatValue;
                    break;

                // ── 打字机：写入 Text / TmpText 的内容 ──
                // 注意：Text 与 TmpText 互斥，优先 Text，回退 TmpText。
                case TweenNodeType.w_打字机:
                    if (target.controller.mod_Text != null)
                    {
                        if (!IsPreviewing)
                            Undo.RecordObject(target.controller.mod_Text, "undoText");
                        target.controller.mod_Text.txt_Set_Content(p.stringValue);
                    }
                    else if (target.controller.mod_TmpText != null)
                    {
                        if (!IsPreviewing)
                            Undo.RecordObject(target.controller.mod_TmpText, "undoTmpText");
                        target.controller.mod_TmpText.tmp_Set_Content(p.stringValue);
                    }
                    // 文本变更需要刷新编辑器窗口与场景视图，否则预览不会立即更新。
                    Repaint();
                    SceneView.RepaintAll();
                    break;

                // ── 图像填充：写入 Image.fillAmount ──
                case TweenNodeType.f_图像填充:
                    if (!IsPreviewing)
                        Undo.RecordObject(target.controller.mod_Image, "undofill");
                    target.controller.mod_Image.fillAmount = p.floatValue;
                    break;

                // ── 尺寸：写入 RectTransform.sizeDelta ──
                case TweenNodeType.z_尺寸:
                    if (!IsPreviewing)
                        Undo.RecordObject(target.controller.mod_Rect, "undotransform-size");
                    target.controller.mod_Rect.sizeDelta = p.vector2Value;
                    break;
            }
        }
        /// <summary>
        /// 按序列化属性类型将其值重置为「零值」（Vector 零 / Color 透明 / 字符串空 / 数值 0）
        /// </summary>
        private void ResetValueProperty(SerializedProperty prop)
        {
            //Debug.Log($"{prop.propertyType}");

            // 根据类型交换值
            switch (prop.propertyType)
            {
                case SerializedPropertyType.Vector4:
                    prop.vector4Value = Vector4.zero;
                    break;
                case SerializedPropertyType.Vector3:
                    prop.vector3Value = Vector3.zero;
                    break;
                case SerializedPropertyType.Vector2:
                    prop.vector2Value = Vector2.zero;
                    break;
                case SerializedPropertyType.Color:
                    prop.colorValue = Color.clear;
                    break;
                case SerializedPropertyType.String:
                    prop.stringValue = null;
                    break;
                case SerializedPropertyType.Float:
                    prop.floatValue = 0f;
                    break;
                case SerializedPropertyType.Integer:
                    prop.intValue = 0;
                    break;
            }
        }
        /// <summary>
        /// 交换两个同类型序列化属性的值
        /// <para/>
        /// 使用方式：参数面板「数值」分组中，点击数值流向指示器按钮时调用。
        /// 按当前 <c>value_mode_index</c> 决定交换哪两个属性：
        /// <list type="bullet">
        /// <item><description>0 (S-D)：交换 <c>From</c> ↔ <c>Origin</c>；</description></item>
        /// <item><description>1 (D-E)：交换 <c>Origin</c> ↔ <c>End</c>；</description></item>
        /// <item><description>2 (S-E)：交换 <c>From</c> ↔ <c>End</c>。</description></item>
        /// </list>
        /// 本方法只操作 <see cref="SerializedProperty"/>，不直接修改场景对象；
        /// 交换结果由调用方后续的 <c>ApplyModifiedProperties()</c> 落地。
        /// <para/>
        /// 前置条件：两个属性的 <see cref="SerializedProperty.propertyType"/> 必须一致，
        /// 否则直接返回不做任何修改（例如 Vector3 ↔ Color 的误用会被安全拦截）。
        /// </summary>
        /// <param name="prop_primary">主属性（交换后获得 <paramref name="prop_secondary"/> 的原值）</param>
        /// <param name="prop_secondary">次属性（交换后获得 <paramref name="prop_primary"/> 的原值）</param>
        private void SwapValueProperties(SerializedProperty prop_primary, SerializedProperty prop_secondary)
        {
            // 类型不一致时不做任何交换，避免把 Vector3 写进 Color 等类型错配。
            if (prop_primary.propertyType != prop_secondary.propertyType)
                return;

            // 根据类型交换值
            switch (prop_primary.propertyType)
            {
                // ── Vector4：先暂存 secondary，再依次对调 ──
                case SerializedPropertyType.Vector4:
                    Vector4 oriV4 = prop_secondary.vector4Value;
                    prop_secondary.vector4Value = prop_primary.vector4Value;
                    prop_primary.vector4Value = oriV4;
                    break;

                // ── Vector3：位移 / 旋转 / 缩放使用 ──
                case SerializedPropertyType.Vector3:
                    Vector3 oriV3 = prop_secondary.vector3Value;
                    prop_secondary.vector3Value = prop_primary.vector3Value;
                    prop_primary.vector3Value = oriV3;
                    break;

                // ── Vector2：尺寸使用 ──
                case SerializedPropertyType.Vector2:
                    Vector2 oriV2 = prop_secondary.vector2Value;
                    prop_secondary.vector2Value = prop_primary.vector2Value;
                    prop_primary.vector2Value = oriV2;
                    break;

                // ── Color：颜色使用 ──
                case SerializedPropertyType.Color:
                    Color oriColor = prop_secondary.colorValue;
                    prop_secondary.colorValue = prop_primary.colorValue;
                    prop_primary.colorValue = oriColor;
                    break;

                // ── String：打字机文本使用 ──
                case SerializedPropertyType.String:
                    string oriStr = prop_secondary.stringValue;
                    prop_secondary.stringValue = prop_primary.stringValue;
                    prop_primary.stringValue = oriStr;
                    break;

                // ── Float：淡化 / 图像填充使用 ──
                case SerializedPropertyType.Float:
                    float oriFloat = prop_secondary.floatValue;
                    prop_secondary.floatValue = prop_primary.floatValue;
                    prop_primary.floatValue = oriFloat;
                    break;

                // ── Integer：保留分支，当前 TweenNodeType 尚未用到 ──
                case SerializedPropertyType.Integer:
                    int oriInt = prop_secondary.intValue;
                    prop_secondary.intValue = prop_primary.intValue;
                    prop_primary.intValue = oriInt;
                    break;
            }
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

        #region 交互：Clip 拖拽
        /// <summary> 
        ///在 Clip 矩形上检测 MouseDown 命中，命中则进入对应的拖拽模式
        /// </summary>
        /// <param name="clipRect">Clip 矩形（内容坐标）</param>
        /// <param name="index">动画节点索引</param>
        private void TryBeginClipDrag(Rect clipRect, int index)
        {
            Event e = Event.current;
            if (e.type != EventType.MouseDown) return;
            if (e.button != 0) return;
            if (e.alt) return;
            if (dragMode != DragMode.无) return;
            if (!clipRect.Contains(e.mousePosition)) return;

            bool ctrl = e.control || e.command;
            bool shift = e.shift;

            if (ctrl || shift)
            {
                SetSelection(TrackKind.Node, index, ctrl, shift);
            }
            else if (!IsSelected(TrackKind.Node, index))
            {
                SetSelection(TrackKind.Node, index, false, false);
            }
            else
            {
                // 点在已选中的动画上 → 保持全部选中，只更新主选中
                selectedKind = TrackKind.Node;
                selectedIndex = index;
                GUIUtility.keyboardControl = 0;
            }

            // 判断拖拽模式
            if (Mathf.Abs(e.mousePosition.x - clipRect.xMin) < ClipEdgeZone)
                dragMode = DragMode.左边缘;
            else if (Mathf.Abs(e.mousePosition.x - clipRect.xMax) < ClipEdgeZone)
                dragMode = DragMode.右边缘;
            else
                dragMode = DragMode.移动;

            if (dragMode == DragMode.移动)
            {
                float clipCenterX = (clipRect.xMin + clipRect.xMax) * 0.5f;
                moveDragAnchorSide = e.mousePosition.x < clipCenterX ? 1 : 2;
            }
            else
            {
                moveDragAnchorSide = 0;
            }

            primaryDragIndex = selectedNodeIndices.Contains(index)
                ? GetTopmostSelectedIndex(selectedNodeIndices)
                : index;
            if (primaryDragIndex < 0) primaryDragIndex = index;

            // 记录起始 Delay（两类都记）
            dragStartNodeDelays.Clear();
            dragStartNodeDurations.Clear();
            dragStartSoundDelays.Clear();

            foreach (int i in selectedNodeIndices)
            {
                if (i < 0 || i >= Nodes.Count) continue;
                dragStartNodeDelays[i] = QuantizeTime(Nodes[i].Delay);
                dragStartNodeDurations[i] = QuantizeTime(Nodes[i].Duration);
            }
            foreach (int i in selectedSoundIndices)
            {
                if (i < 0 || i >= Sounds.Count) continue;
                dragStartSoundDelays[i] = QuantizeTime(Sounds[i].Delay);
            }

            // 保证当前拖拽项在字典里
            if (!dragStartNodeDelays.ContainsKey(index))
            {
                dragStartNodeDelays[index] = QuantizeTime(Nodes[index].Delay);
                dragStartNodeDurations[index] = QuantizeTime(Nodes[index].Duration);
                primaryDragIndex = index;
            }

            dragStartDelay = QuantizeTime(Nodes[primaryDragIndex].Delay);
            dragStartDuration = QuantizeTime(Nodes[primaryDragIndex].Duration);

            draggingIndex = index;
            draggingKind = TrackKind.Node;
            dragControlID = GUIUtility.GetControlID(FocusType.Passive);
            GUIUtility.hotControl = dragControlID;
            GUIUtility.keyboardControl = 0;
            dragStartContentSecond = e.mousePosition.x / pixelsPerSecond;
            snapGuideSecond = -1f;

            if (!IsPreviewing)
            {
                Undo.IncrementCurrentGroup();
                Undo.SetCurrentGroupName("Edit Tween Clip");
                dragUndoGroup = Undo.GetCurrentGroup();
                Undo.RecordObject(target, "Edit Tween Clip");
            }
            else
            {
                dragUndoGroup = -1;
            }

            clipHitThisFrame = true;
            e.Use();
            Repaint();
        }
        /// <summary> 
        /// 在音效 Clip 上检测 MouseDown 命中，命中则进入"移动"拖拽模式。
        /// <para/>
        /// 与动画 Clip 不同：音效不做左右边缘拉伸，只做整体移动。
        /// </summary>
        private void TryBeginSoundClipDrag(Rect clipRect, int soundIndex)
        {
            Event e = Event.current;
            if (e.type != EventType.MouseDown) return;
            if (e.button != 0) return;
            if (e.alt) return;
            if (dragMode != DragMode.无) return;
            if (!clipRect.Contains(e.mousePosition)) return;

            bool ctrl = e.control || e.command;
            bool shift = e.shift;

            if (ctrl || shift)
            {
                SetSelection(TrackKind.Sound, soundIndex, ctrl, shift);
            }
            else if (!IsSelected(TrackKind.Sound, soundIndex))
            {
                SetSelection(TrackKind.Sound, soundIndex, false, false);
            }
            else
            {
                selectedKind = TrackKind.Sound;
                selectedIndex = soundIndex;
                GUIUtility.keyboardControl = 0;
            }

            dragMode = DragMode.移动;
            draggingKind = TrackKind.Sound;
            draggingIndex = soundIndex;
            moveDragAnchorSide = 0;
            primaryDragIndex = soundIndex;

            // 记录起始 Delay（两类都记）
            dragStartNodeDelays.Clear();
            dragStartNodeDurations.Clear();
            dragStartSoundDelays.Clear();

            foreach (int i in selectedNodeIndices)
            {
                if (i < 0 || i >= Nodes.Count) continue;
                dragStartNodeDelays[i] = QuantizeTime(Nodes[i].Delay);
                dragStartNodeDurations[i] = QuantizeTime(Nodes[i].Duration);
            }
            foreach (int i in selectedSoundIndices)
            {
                if (i < 0 || i >= Sounds.Count) continue;
                dragStartSoundDelays[i] = QuantizeTime(Sounds[i].Delay);
            }

            if (!dragStartSoundDelays.ContainsKey(soundIndex))
                dragStartSoundDelays[soundIndex] = QuantizeTime(Sounds[soundIndex].Delay);

            dragStartDelay = QuantizeTime(Sounds[soundIndex].Delay);
            dragStartDuration = 0f;

            dragControlID = GUIUtility.GetControlID(FocusType.Passive);
            GUIUtility.hotControl = dragControlID;
            GUIUtility.keyboardControl = 0;
            dragStartContentSecond = e.mousePosition.x / pixelsPerSecond;
            snapGuideSecond = -1f;

            if (!IsPreviewing)
            {
                Undo.IncrementCurrentGroup();
                Undo.SetCurrentGroupName("Edit Tween Sound Clip");
                dragUndoGroup = Undo.GetCurrentGroup();
                Undo.RecordObject(target, "Edit Tween Sound Clip");
            }
            else
            {
                dragUndoGroup = -1;
            }

            clipHitThisFrame = true;
            e.Use();
            Repaint();
        }
        /// <summary> 
        ///统一处理 Clip 拖拽的 MouseDrag / MouseUp
        /// </summary>
        /// <param name="viewportArea">ScrollView 视口矩形（Clip 区局部坐标，用于判断边缘自动平移）</param>
        private void ProcessClipDrag(Rect viewportArea)
        {
            Event e = Event.current;

            // 按拖拽类型分别做有效性校验
            bool valid = false;
            if (dragMode != DragMode.无)
            {
                if (draggingKind == TrackKind.Sound)
                    valid = draggingIndex >= 0 && draggingIndex < Sounds.Count;
                else
                    valid = draggingIndex >= 0 && draggingIndex < Nodes.Count;
            }

            if (!valid)
            {
                if (e.type == EventType.MouseUp && GUIUtility.hotControl == dragControlID)
                    GUIUtility.hotControl = 0;
                return;
            }
            switch (e.type)
            {
                case EventType.MouseDrag:
                    if (GUIUtility.hotControl != dragControlID) return;
                    if (draggingKind == TrackKind.Sound)
                        ProcessSoundDrag(e, viewportArea);
                    else
                        ProcessNodeDrag(e, viewportArea);
                    break;
                case EventType.MouseUp:
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
        private void ProcessNodeDrag(Event e, Rect viewportArea)
        {
            TweenNode node = Nodes[draggingIndex];
            float currentContentSecond = e.mousePosition.x / pixelsPerSecond;
            float totalDelta = currentContentSecond - dragStartContentSecond;
            snapGuideSecond = -1f;
            bool snapOn = this.snapEnabled || e.shift;

            if (!IsPreviewing)
                Undo.RecordObject(target, "Edit Tween Clip");

            switch (dragMode)
            {
                case DragMode.移动:
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
                                bool leftSnapped = TrySnapTimeToReference(rawDelay, selectedNodeIndices, out snappedStart);
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
                                bool rightSnapped = TrySnapTimeToReference(rawEnd, selectedNodeIndices, out snappedEnd);
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

                        // 整体下界：所有选中的动画和音效，Delay + delta >= 0
                        float minStartDelay = float.MaxValue;
                        foreach (var kv in dragStartNodeDelays)
                            if (kv.Value < minStartDelay) minStartDelay = kv.Value;
                        foreach (var kv in dragStartSoundDelays)
                            if (kv.Value < minStartDelay) minStartDelay = kv.Value;
                        if (minStartDelay + finalDelta < 0f)
                            finalDelta = -minStartDelay;

                        // 写回动画
                        foreach (var kv in dragStartNodeDelays)
                        {
                            int idx = kv.Key;
                            if (idx < 0 || idx >= Nodes.Count) continue;
                            Nodes[idx].Delay = QuantizeTime(kv.Value + finalDelta);
                        }
                        // 写回音效
                        foreach (var kv in dragStartSoundDelays)
                        {
                            int idx = kv.Key;
                            if (idx < 0 || idx >= Sounds.Count) continue;
                            Sounds[idx].Delay = QuantizeTime(kv.Value + finalDelta);
                        }
                        break;
                    }

                case DragMode.左边缘:
                    {
                        TweenNode primary = Nodes[primaryDragIndex];
                        float primaryStartDelay = dragStartNodeDelays.ContainsKey(primaryDragIndex)
                            ? dragStartNodeDelays[primaryDragIndex]
                            : QuantizeTime(primary.Delay);

                        float rawDelay = Mathf.Max(0f, primaryStartDelay + totalDelta);
                        float snapped;
                        if (snapOn)
                        {
                            float snapValue;
                            bool snappedFlag = TrySnapTimeToReference(rawDelay, selectedNodeIndices, out snapValue);
                            snapped = QuantizeTime(Mathf.Max(0f, snapValue));
                            snapGuideSecond = snappedFlag ? snapped : -1f;
                        }
                        else
                        {
                            snapped = QuantizeTime(rawDelay);
                            snapGuideSecond = -1f;
                        }

                        float delta = snapped - primaryStartDelay;

                        // 动画约束：左边缘不能越过右边缘，也不能让 Delay < 0
                        float minDelta = float.MinValue;
                        float maxDelta = float.MaxValue;
                        foreach (var kv in dragStartNodeDelays)
                        {
                            int idx = kv.Key;
                            if (idx < 0 || idx >= Nodes.Count) continue;
                            float startD = kv.Value;
                            float startDur = dragStartNodeDurations[idx];
                            float lo = -startD;
                            float hi = startDur - 0.01f;
                            if (lo > minDelta) minDelta = lo;
                            if (hi < maxDelta) maxDelta = hi;
                        }
                        // 音效约束：只约束"起点不能为负"（音效不参与拉伸，但要跟随整体位移）
                        foreach (var kv in dragStartSoundDelays)
                        {
                            float lo = -kv.Value;
                            if (lo > minDelta) minDelta = lo;
                        }

                        delta = Mathf.Clamp(delta, minDelta, maxDelta);

                        // 写回动画：拉伸（Delay + Duration 同时改）
                        foreach (var kv in dragStartNodeDelays)
                        {
                            int idx = kv.Key;
                            if (idx < 0 || idx >= Nodes.Count) continue;
                            float startD = kv.Value;
                            float startDur = dragStartNodeDurations[idx];
                            float endFixed = QuantizeTime(startD + startDur);
                            float newDelay = QuantizeTime(startD + delta);
                            Nodes[idx].Delay = newDelay;
                            Nodes[idx].Duration = QuantizeTime(Mathf.Max(0.01f, endFixed - newDelay));
                        }
                        // 写回音效：只跟随整体位移（Delay 改，长度不变）
                        foreach (var kv in dragStartSoundDelays)
                        {
                            int idx = kv.Key;
                            if (idx < 0 || idx >= Sounds.Count) continue;
                            Sounds[idx].Delay = QuantizeTime(kv.Value + delta);
                        }
                        break;
                    }

                case DragMode.右边缘:
                    {
                        float primaryStartDelay = dragStartNodeDelays.ContainsKey(primaryDragIndex)
                            ? dragStartNodeDelays[primaryDragIndex] : dragStartDelay;
                        float primaryStartDur = dragStartNodeDurations.ContainsKey(primaryDragIndex)
                            ? dragStartNodeDurations[primaryDragIndex] : dragStartDuration;
                        float primaryStartEnd = QuantizeTime(primaryStartDelay + primaryStartDur);
                        float rawEnd = primaryStartEnd + totalDelta;
                        float snapped;
                        if (snapOn)
                        {
                            float snapValue;
                            bool snappedFlag = TrySnapTimeToReference(rawEnd, selectedNodeIndices, out snapValue);
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
                        foreach (var kv in dragStartNodeDelays)
                        {
                            int idx = kv.Key;
                            if (idx < 0 || idx >= Nodes.Count) continue;
                            float startD = kv.Value;
                            float startDur = dragStartNodeDurations[idx];
                            float lo = 0.01f - startDur;
                            float hi = float.MaxValue;
                            if (lo > minDelta) minDelta = lo;
                            if (hi < maxDelta) maxDelta = hi;
                        }
                        delta = Mathf.Clamp(delta, minDelta, maxDelta);

                        // 写回动画：仅改 Duration
                        foreach (var kv in dragStartNodeDelays)
                        {
                            int idx = kv.Key;
                            if (idx < 0 || idx >= Nodes.Count) continue;
                            float startD = kv.Value;
                            float startDur = dragStartNodeDurations[idx];
                            Nodes[idx].Delay = QuantizeTime(startD);
                            Nodes[idx].Duration = QuantizeTime(Mathf.Max(0.01f, startDur + delta));
                        }
                        // 右边缘拉伸：音效**不动**（决策 2.a）
                        break;
                    }
            }

            AutoScrollViewOnDragEdge(e.mousePosition.x - scrollPos.x, new Rect(0, 0, viewportArea.width, viewportArea.height));
            EditorUtility.SetDirty(target);
            Repaint();
            e.Use();
        }
        /// <summary> 
        /// 处理音效 Clip 拖拽：只修改 <see cref="TweenSound.Delay"/>。
        /// </summary>
        private void ProcessSoundDrag(Event e, Rect viewportArea)
        {
            if (draggingIndex < 0 || draggingIndex >= Sounds.Count) return;

            float currentContentSecond = e.mousePosition.x / pixelsPerSecond;
            float totalDelta = currentContentSecond - dragStartContentSecond;
            snapGuideSecond = -1f;
            bool snapOn = this.snapEnabled || e.shift;

            if (!IsPreviewing)
                Undo.RecordObject(target, "Edit Tween Sound Clip");

            TweenSound primary = Sounds[draggingIndex];
            float primaryStart = dragStartSoundDelays.ContainsKey(draggingIndex)
                ? dragStartSoundDelays[draggingIndex]
                : QuantizeTime(primary.Delay);
            float rawDelay = Mathf.Max(0f, primaryStart + totalDelta);

            float snapped;
            if (snapOn)
            {
                float snapValue;
                bool snappedFlag = TrySnapTimeToReferenceForSound(rawDelay, draggingIndex, out snapValue);
                snapped = QuantizeTime(Mathf.Max(0f, snapValue));
                snapGuideSecond = snappedFlag ? snapped : -1f;
            }
            else
            {
                snapped = QuantizeTime(rawDelay);
            }

            float finalDelta = snapped - primaryStart;

            // 整体下界：所有选中的动画和音效，Delay + delta >= 0
            float minStart = float.MaxValue;
            foreach (var kv in dragStartNodeDelays)
                if (kv.Value < minStart) minStart = kv.Value;
            foreach (var kv in dragStartSoundDelays)
                if (kv.Value < minStart) minStart = kv.Value;
            if (minStart + finalDelta < 0f)
                finalDelta = -minStart;

            // 写回音效
            foreach (var kv in dragStartSoundDelays)
            {
                int idx = kv.Key;
                if (idx < 0 || idx >= Sounds.Count) continue;
                Sounds[idx].Delay = QuantizeTime(kv.Value + finalDelta);
            }
            // 写回动画（跟随平移）
            foreach (var kv in dragStartNodeDelays)
            {
                int idx = kv.Key;
                if (idx < 0 || idx >= Nodes.Count) continue;
                Nodes[idx].Delay = QuantizeTime(kv.Value + finalDelta);
            }

            AutoScrollViewOnDragEdge(e.mousePosition.x - scrollPos.x,
                new Rect(0, 0, viewportArea.width, viewportArea.height));
            EditorUtility.SetDirty(target);
            Repaint();
            e.Use();
        }
        /// <summary> 
        ///拖拽时若鼠标靠近 Clip 区左右边缘，自动平移 <see cref="scrollPos"/>
        /// </summary>
        /// <param name="mouseViewportX">鼠标在 ScrollView 视口坐标系下的 X（已减 scrollPos.x）</param>
        /// <param name="viewportArea">ScrollView 视口矩形（宽高用于判断边缘）</param>
        private void AutoScrollViewOnDragEdge(float mouseViewportX, Rect viewportArea)
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
        /// </summary>
        /// <param name="localArea">Clip 区局部矩形（刻度尺下方），用于判断按下的位置是否在区内</param>
        private void HandleViewPan(Rect localArea)
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
                        float maxScrollY = CalculateMaxVerticalScroll();
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
        /// </summary>
        private void HandleMouseScrollWheel()
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
                float maxScrollY = CalculateMaxVerticalScroll();
                scrollPos.y = Mathf.Clamp(scrollPos.y, 0f, maxScrollY);
                nameScroll.y = scrollPos.y;
                e.Use();
                Repaint();
                return;
            }
            if (e.control)
            {
                float maxScrollY = CalculateMaxVerticalScroll();
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
        /// </summary>
        /// <param name="localArea">Clip 区局部矩形（刻度尺下方）</param>
        private void HandleClipAreaEmptyClick(Rect localArea)
        {
            Event e = Event.current;
            if (e.type != EventType.MouseDown) return;
            if (e.button != 0) return;
            if (e.alt) return;
            if (!localArea.Contains(e.mousePosition)) return;
            if (clipHitThisFrame) return;
            selectedNodeIndices.Clear();
            selectedSoundIndices.Clear();
            selectedIndex = -1;
            selectedKind = TrackKind.无;
            GUIUtility.keyboardControl = 0;
            e.Use();
            Repaint();
        }
        /// <summary> 
        ///处理刻度尺区域的左键点击：只清除参数面板焦点，不取消选中
        /// </summary>
        /// <param name="rulerArea">刻度尺在 Clip 区局部坐标系下的矩形</param>
        private void HandleTimeRulerClick(Rect rulerArea)
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
        /// </summary>
        private void HandleKeyboardShortcuts()
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

        #region 交互：多选
        /// <summary> 取指定集合里索引最小的（视觉最上面）。 </summary>
        private int GetTopmostSelectedIndex(HashSet<int> set)
        {
            int top = int.MaxValue;
            foreach (int i in set) if (i < top) top = i;
            return top == int.MaxValue ? -1 : top;
        }

        /// <summary> 取当前主类型选中集合里索引最小的。 </summary>
        private int GetTopmostSelectedIndex()
        {
            if (selectedKind == TrackKind.Node) return GetTopmostSelectedIndex(selectedNodeIndices);
            if (selectedKind == TrackKind.Sound) return GetTopmostSelectedIndex(selectedSoundIndices);
            return -1;
        }
        /// <summary> 
        /// 判断某个轨道项是否被选中。
        /// </summary>
        private bool IsSelected(TrackKind kind, int index)
        {
            if (kind == TrackKind.Node) return selectedNodeIndices.Contains(index);
            if (kind == TrackKind.Sound) return selectedSoundIndices.Contains(index);
            return false;
        }

        /// <summary>
        /// 统一的选中设置入口。
        /// <para/>
        /// 支持 Ctrl 切换单项（允许跨类型累加）、Shift 范围选（仅同类型内生效）；
        /// 普通单击会清空所有类型的选中，只保留当前项。
        /// </summary>
        private void SetSelection(TrackKind kind, int index, bool additive, bool range)
        {
            int count = kind == TrackKind.Node ? Nodes.Count : Sounds.Count;
            if (index < 0 || index >= count)
            {
                // 越界：清空该类型选中
                if (kind == TrackKind.Node) selectedNodeIndices.Clear();
                else if (kind == TrackKind.Sound) selectedSoundIndices.Clear();
                selectedIndex = -1;
                if (TotalSelectedCount == 0) selectedKind = TrackKind.无;
                return;
            }

            HashSet<int> currentSet = kind == TrackKind.Node ? selectedNodeIndices : selectedSoundIndices;

            if (range && selectedKind == kind && selectedIndex >= 0)
            {
                // Shift 范围选：只在同类型内生效
                int from = Mathf.Min(selectedIndex, index);
                int to = Mathf.Max(selectedIndex, index);
                currentSet.Clear();
                for (int i = from; i <= to; i++) currentSet.Add(i);
            }
            else if (additive)
            {
                // Ctrl 切换单项：允许跨类型累加
                if (currentSet.Contains(index))
                {
                    currentSet.Remove(index);
                    if (selectedKind == kind && selectedIndex == index)
                        selectedIndex = currentSet.Count > 0 ? GetTopmostSelectedIndex(currentSet) : -1;
                }
                else
                {
                    currentSet.Add(index);
                    selectedIndex = index;
                    selectedKind = kind;
                }
            }
            else
            {
                // 普通单击：清空所有类型的选中，只保留当前项
                selectedNodeIndices.Clear();
                selectedSoundIndices.Clear();
                currentSet.Add(index);
                selectedIndex = index;
                selectedKind = kind;
            }

            GUIUtility.keyboardControl = 0;
            Repaint();
        }

        /// <summary> 
        /// 当前选中集合对应的列表数量（Node / Sound）。
        /// </summary>
        private int SelectedListCount =>
            selectedKind == TrackKind.Node ? Nodes.Count :
            selectedKind == TrackKind.Sound ? Sounds.Count : 0;
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
                // ★ 这里
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

        #region 工具：时间量化
        /// <summary> 
        ///将时间值量化到两位小数精度（秒），即 10ms 分辨率
        /// </summary>
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
        /// </summary>
        private float SnapThresholdSeconds => SnapThresholdPixels / pixelsPerSecond;
        /// <summary> 
        /// 对给定时间尝试吸附：优先吸附到其他 Clip 的边界，其次吸附到整秒网格
        /// <para/>
        /// 吸附优先级（从高到低）：
        /// <list type="number">
        /// <item><description>其他 Clip 的起始边界（<c>other.Delay</c>）；</description></item>
        /// <item><description>其他 Clip 的结束边界（<c>other.Delay + other.Duration</c>）；</description></item>
        /// <item><description>整秒网格（<see cref="Mathf.Round(float)"/>）。</description></item>
        /// </list>
        /// 之所以 Clip 边界优先：用户拖拽时更希望对齐到已有动画的首尾，
        /// 而非被整秒网格「抢走」，否则会导致视觉上明显的错位感。
        /// <para/>
        /// 判定阈值 <see cref="SnapThresholdSeconds"/> 按像素阈值换算成秒，
        /// 保证任意缩放下手感一致（缩放大时阈值小、缩放小时阈值大）。
        /// <para/>
        /// 命中判定使用<b>严格小于</b>（<c>&lt;</c>）而非小于等于，
        /// 这样多个候选时间距离相同时，会保留<b>先遍历到的</b>那个，
        /// 配合「先 Clip 边界后整秒网格」的遍历顺序，间接实现了「Clip 边界优先」。
        /// <para/>
        /// 输出 <paramref name="snappedTime"/> 一定经过 <see cref="QuantizeTime"/> 量化（两位小数），
        /// 与节点 Delay / Duration 的量化规则保持一致；未命中时直接返回原时间。
        /// </summary>
        /// <param name="time">待吸附的时间（秒）</param>
        /// <param name="exclude">排除的节点索引集合；为 null 或空表示不排除</param>
        /// <param name="snappedTime">输出：吸附后的时间（已量化）；未命中时等于 <paramref name="time"/></param>
        /// <returns>true 表示命中吸附；false 表示未命中</returns>
        private bool TrySnapTimeToReference(float time, HashSet<int> exclude, out float snappedTime)
        {
            // 阈值 = 像素阈值 / 每秒像素数，换算成秒；
            // 缩放越大 → 阈值越小（像素固定但对应秒数更小），反之亦然。
            float threshold = SnapThresholdSeconds;

            // bestTime / bestDist 记录当前最优候选：
            // bestDist 初值为 threshold，配合「严格小于」判定，
            // 保证只有距离更近的候选才会覆盖 bestTime。
            float bestTime = time;
            float bestDist = threshold;
            bool snapped = false;

            // ── 优先级 1 / 2：遍历其他 Clip 的起始与结束边界 ──
            for (int i = 0; i < Nodes.Count; i++)
            {
                // exclude 用于排除「正在被拖拽的节点自身」，
                // 否则节点会吸附到自己原来的位置上，导致拖不动。
                if (exclude != null && exclude.Contains(i)) continue;
                TweenNode other = Nodes[i];
                float otherStart = other.Delay;
                float otherEnd = other.Delay + other.Duration;

                // 优先比较起始边界：距离更近才更新最优候选
                float dStart = Mathf.Abs(time - otherStart);
                if (dStart < bestDist) { bestDist = dStart; bestTime = otherStart; snapped = true; }

                // 再比较结束边界
                float dEnd = Mathf.Abs(time - otherEnd);
                if (dEnd < bestDist) { bestDist = dEnd; bestTime = otherEnd; snapped = true; }
            }

            // ── 优先级 3：整秒网格 ──
            // 放在 Clip 边界之后比较，配合「严格小于」判定，
            // 当整秒与 Clip 边界距离完全相同时，Clip 边界优先（先遍历到的胜出）。
            float rounded = Mathf.Round(time);
            float distToSecond = Mathf.Abs(time - rounded);
            if (distToSecond < bestDist) { bestDist = distToSecond; bestTime = rounded; snapped = true; }

            // 命中时对最佳候选做量化，保证与节点 Delay / Duration 精度一致；
            // 未命中时直接返回原始 time（不做量化，避免引入无谓的舍入）。
            snappedTime = snapped ? QuantizeTime(bestTime) : time;
            return snapped;
        }
        /// <summary> 
        /// 音效吸附：参照物 = 所有动画节点边界 + 所有音效边界（排除拖拽中的音效自身）+ 整秒网格。
        /// </summary>
        private bool TrySnapTimeToReferenceForSound(float time, int excludeSoundIndex, out float snappedTime)
        {
            float threshold = SnapThresholdSeconds;
            float bestTime = time;
            float bestDist = threshold;
            bool snapped = false;

            // 动画节点边界
            for (int i = 0; i < Nodes.Count; i++)
            {
                TweenNode other = Nodes[i];
                float s = other.Delay;
                float e = other.Delay + other.Duration;
                float ds = Mathf.Abs(time - s);
                if (ds < bestDist) { bestDist = ds; bestTime = s; snapped = true; }
                float de = Mathf.Abs(time - e);
                if (de < bestDist) { bestDist = de; bestTime = e; snapped = true; }
            }

            // 其他音效边界
            for (int i = 0; i < Sounds.Count; i++)
            {
                if (i == excludeSoundIndex) continue;
                TweenSound other = Sounds[i];
                float s = other.Delay;
                float len = other.Sound != null ? other.Sound.length : DefaultSoundClipSeconds;
                float e = other.Delay + len;
                float ds = Mathf.Abs(time - s);
                if (ds < bestDist) { bestDist = ds; bestTime = s; snapped = true; }
                float de = Mathf.Abs(time - e);
                if (de < bestDist) { bestDist = de; bestTime = e; snapped = true; }
            }

            // 整秒网格
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
        /// </summary>
        /// <returns>垂直滚动上限（像素），恒 ≥ 0</returns>
        private float CalculateMaxVerticalScroll()
        {
            float contentHeight = TotalRowCount * trackHeight + 20f;
            float viewH = cachedClipAreaRect.height - rulerHeight - HorizontalScrollbarHeight;
            return Mathf.Max(0f, contentHeight - Mathf.Max(1f, viewH));
        }
        #endregion

        #region 工具：视图操作
        /// <summary>
        /// 缩放到合适范围。
        /// <para/>
        /// 按当前选中状态分四种策略：
        /// <list type="bullet">
        /// <item><description>多选（总数 &gt; 1）：以选中内容（动画 + 音效）的最右端适配；</description></item>
        /// <item><description>单选动画节点：以该节点 Duration 铺满视口；</description></item>
        /// <item><description>单选音效：以该音效的音频长度（Sound 为空时用 <see cref="DefaultSoundClipSeconds"/>）铺满视口；</description></item>
        /// <item><description>无选中 / 选中项越界：以所有内容（动画 + 音效）的最右端适配。</description></item>
        /// </list>
        /// <para/>
        /// 单选项分支保持一致的"左边距 padding"手感：
        /// 若起始位置（Delay）换算成像素后大于 padding，则起点留出 padding 余量，让 Delay 部分可见；
        /// 否则从 0 开始让内容铺满整个宽度。
        /// <para/>
        /// 调用入口：快捷键 F、工具栏「合适范围」按钮、底部「缩放到全部」按钮。
        /// </summary>
        private void FitViewToContent()
        {
            // ── 前置校验：动画节点为空时退化为重置视图 ──
            // 注：这里保持与原有逻辑一致，动画节点为空即视为"无可适配内容"。
            // 若将来要支持"只有音效没有动画"的场景，需要放宽这个判断。
            if (Nodes == null || Nodes.Count == 0)
            {
                ResetTimelineView();
                return;
            }

            // ── 计算可用宽度 ──
            // 优先用缓存矩形；缓存无效时用窗口宽度减去左右两栏宽度兜底。
            float availableWidth = cachedClipAreaRect.width;
            if (availableWidth <= 1f)
            {
                availableWidth = position.width - nameColumnWidth - paramPanelWidth;
            }
            if (availableWidth <= 1f) return;

            // 两侧各留 20px 的视觉边距
            const float padding = 20f;

            // ══════════════════════════════════════════════════════════
            // 分支 0：多选（总数 > 1）→ 以选中内容的最右端适配
            // ══════════════════════════════════════════════════════════
            if (TotalSelectedCount > 1)
            {
                float maxEndSel = 0f;
                foreach (int i in selectedNodeIndices)
                {
                    if (i < 0 || i >= Nodes.Count) continue;
                    float end = Nodes[i].Delay + Nodes[i].Duration;
                    if (end > maxEndSel) maxEndSel = end;
                }
                foreach (int i in selectedSoundIndices)
                {
                    if (i < 0 || i >= Sounds.Count) continue;
                    TweenSound s = Sounds[i];
                    float len = s.Sound != null ? s.Sound.length : DefaultSoundClipSeconds;
                    float end = s.Delay + len;
                    if (end > maxEndSel) maxEndSel = end;
                }
                if (maxEndSel > 0.0001f)
                {
                    float usable = Mathf.Max(1f, availableWidth - padding * 2f);
                    pixelsPerSecond = Mathf.Clamp(usable / maxEndSel, 1f, 20000f);
                    scrollPos.x = 0f;
                    scrollPos.y = 0f;
                    nameScroll.y = 0f;
                    Repaint();
                    return;
                }
                // maxEndSel == 0（比如都空）→ 落到下面分支
            }

            // ══════════════════════════════════════════════════════════
            // 分支 1：单选动画节点 → 以该节点 Duration 铺满
            // ══════════════════════════════════════════════════════════
            if (TotalSelectedCount == 1
                && selectedKind == TrackKind.Node
                && selectedIndex >= 0 && selectedIndex < Nodes.Count)
            {
                TweenNode node = Nodes[selectedIndex];

                // 时长为 0 无法适配，退回重置视图
                if (node.Duration <= 0.0001f)
                {
                    ResetTimelineView();
                    return;
                }

                float usableWidth = Mathf.Max(1f, availableWidth - padding * 2f);
                float tempPixelsPerSecond = usableWidth / node.Duration;
                tempPixelsPerSecond = Mathf.Clamp(tempPixelsPerSecond, 1f, 20000f);

                float delayPixels = node.Delay * tempPixelsPerSecond;
                if (delayPixels > padding)
                {
                    // 前置延迟较长：起点留出 padding 余量，让 Delay 部分可见
                    pixelsPerSecond = tempPixelsPerSecond;
                    scrollPos.x = node.Delay * pixelsPerSecond - padding;
                    scrollPos.x = Mathf.Max(0f, scrollPos.x);
                }
                else
                {
                    // 前置延迟较短：直接从 0 开始，让 Clip 铺满
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

            // ══════════════════════════════════════════════════════════
            // 分支 2：单选音效 → 以该音效的音频长度铺满
            // ══════════════════════════════════════════════════════════
            if (TotalSelectedCount == 1
                && selectedKind == TrackKind.Sound
                && selectedIndex >= 0 && selectedIndex < Sounds.Count)
            {
                TweenSound sound = Sounds[selectedIndex];

                // Sound 为空时用默认占位长度（与 CalculateSoundClipRect 保持一致）
                float len = sound.Sound != null ? sound.Sound.length : DefaultSoundClipSeconds;

                if (len <= 0.0001f)
                {
                    ResetTimelineView();
                    return;
                }

                float usableWidth = Mathf.Max(1f, availableWidth - padding * 2f);
                float tempPixelsPerSecond = usableWidth / len;
                tempPixelsPerSecond = Mathf.Clamp(tempPixelsPerSecond, 1f, 20000f);

                float delayPixels = sound.Delay * tempPixelsPerSecond;
                if (delayPixels > padding)
                {
                    // 前置延迟较长：起点留出 padding 余量，让 Delay 部分可见
                    pixelsPerSecond = tempPixelsPerSecond;
                    scrollPos.x = sound.Delay * pixelsPerSecond - padding;
                    scrollPos.x = Mathf.Max(0f, scrollPos.x);
                }
                else
                {
                    // 前置延迟较短：直接从 0 开始，让 Clip 铺满
                    usableWidth = Mathf.Max(1f, availableWidth - padding);
                    pixelsPerSecond = usableWidth / len;
                    pixelsPerSecond = Mathf.Clamp(pixelsPerSecond, 1f, 20000f);
                    scrollPos.x = 0f;
                }

                scrollPos.y = 0f;
                nameScroll.y = 0f;
                Repaint();
                return;
            }

            // ══════════════════════════════════════════════════════════
            // 分支 3：无选中 / 选中项越界 → 以所有内容（动画 + 音效）最右端适配
            // ══════════════════════════════════════════════════════════
            float maxEnd = 0f;

            // 动画节点最右端
            for (int i = 0; i < Nodes.Count; i++)
            {
                float end = Nodes[i].Delay + Nodes[i].Duration;
                if (end > maxEnd) maxEnd = end;
            }

            // 音效最右端
            for (int i = 0; i < Sounds.Count; i++)
            {
                TweenSound s = Sounds[i];
                float len = s.Sound != null ? s.Sound.length : DefaultSoundClipSeconds;
                float end = s.Delay + len;
                if (end > maxEnd) maxEnd = end;
            }

            if (maxEnd <= 0.0001f)
            {
                ResetTimelineView();
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
        /// </summary>
        private void ResetTimelineView()
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
        /// </summary>
        private void LoadPersistedViewState()
        {
            if (target == null) return;
            pixelsPerSecond = Mathf.Clamp(target.Timeline_TrackPosition, 1f, 20000f);
            scrollPos = Vector2.Max(Vector2.zero, target.Timeline_TrackScroll);
            nameScroll.y = scrollPos.y;
            trackHeight = Mathf.Clamp(target.Timeline_TrackHeight, MinTrackHeight, MaxTrackHeight);
            snapEnabled = target.Timeline_TrackSnapEnabled;

            nameColumnWidth = LoadClampedFloatPreference(PrefKey_NameWidthWidth, DefaultNameColumnWidth, MinNameColumnWidth, MaxNameColumnWidth);
            paramPanelWidth = LoadClampedFloatPreference(PrefKey_ParamWidthHeight, DefaultParamPanelWidth, MinParamPanelWidth, MaxParamPanelWidth);
        }
        /// <summary> 
        ///将当前视图状态写回 <see cref="target"/>垂直滚动不保存
        /// </summary>
        private void SavePersistedViewState()
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
        /// </summary>
        private static float LoadClampedFloatPreference(string key, float defaultValue, float min, float max)
        {
            if (!XGUI.x_Editor_Data_Has_String(key))
                return Mathf.Clamp(defaultValue, min, max);
            return Mathf.Clamp(XGUI.x_Editor_Data_Get_With_Float(key), min, max);
        }
        #endregion

        #region 工具：窗口尺寸持久化
        /// <summary> 
        ///读取上次关闭时保存的窗口尺寸无记录时返回 <see cref="DefaultWindowSize"/>
        /// </summary>
        /// <returns>窗口尺寸（已按 <see cref="MinWindowSize"/> / <see cref="MaxWindowSize"/> 钳制）</returns>
        private static Vector2 LoadPersistedWindowSize()
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
        /// </summary>
        private void SavePersistedWindowSize()
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
        /// 在指定索引位置插入一条新的空白动画节点
        /// <para/>
        /// 本方法是节点插入的唯一底层入口：
        /// <list type="bullet">
        /// <item><description>窗口空状态下的「新增首个动画」按钮 → <c>InsertTweenNodeAt(0)</c>；</description></item>
        /// <item><description>名字行「插入」按钮 → <see cref="InsertTweenNodeAfter"/> → <c>InsertTweenNodeAt(index + 1)</c>。</description></item>
        /// </list>
        /// <para/>
        /// 执行流程：
        /// <list type="number">
        /// <item><description>注册 Undo（<see cref="Undo.RegisterCompleteObjectUndo"/>），保证 Ctrl+Z 可撤销；</description></item>
        /// <item><description>构造新节点并填入一套「安全默认值」（详见下方字段初始化）；</description></item>
        /// <item><description>钳制插入索引后插入集合；</description></item>
        /// <item><description>清空并重设选中集合，把新节点设为唯一选中项。</description></item>
        /// </list>
        /// <para/>
        /// 注意：新节点的 <c>Type</c> 使用 <see cref="TweenNode"/> 的默认值（枚举 0），
        /// 因此「新增首个动画」后需要用户在参数面板手动选择类型。
        /// </summary>
        /// <param name="insertAt">插入位置；0 表示插到最前，Nodes.Count 表示追加到末尾</param>
        private void InsertTweenNodeAt(int insertAt)
        {
            // 前置校验：目标为空时直接返回，避免 NRE
            if (target == null || target.Equals(null)) return;

            // 注册完整对象 Undo，保证本次插入可通过 Ctrl+Z 撤销
            if (!IsPreviewing)
                Undo.RegisterCompleteObjectUndo(target, "Insert Tween Node");

            // ── 构造新节点并填入安全默认值 ──
            // 所有字段均显式赋值，避免依赖字段初始化器的默认值（尤其是 Color 等引用类型）。
            TweenNode newNode = new TweenNode();

            // ID：由目标统一分配，保证全局唯一
            newNode.ID = target.TweenNode_GenerateId();

            // 显示名：默认 "NewTween"，用户可在参数面板中修改
            newNode.Indicator = "NewTween";

            // 启用状态：默认启用，插入后即可在预览中看到效果
            newNode.Enabled = true;

            // 触发时机：默认「无」，需用户手动选择
            newNode.Timings = "元素进入时";

            // 时长 / 延迟：默认 1 秒、无延迟，是一个「开箱即用」的起始值
            newNode.Duration = 1f;
            newNode.Delay = 0f;

            // 循环：默认 Restart 模式、循环 0 次（即不循环）
            newNode.LoopType = XTween_LoopType.Restart;
            newNode.LoopCount = 0;

            // 运行时进度：默认 0，由 XTween 运行时驱动
            newNode.Progress = 0f;

            // 参数面板折叠状态：默认展开
            newNode.IsFold = false;

            // 重置 / 完成时是否写回起始 / 结束值：默认都开启
            newNode.Rewind_Set_Startvalue = true;
            newNode.Complete_Set_Endvalue = true;

            // 颜色三值：默认白色（对颜色动画是安全起始值，对其他类型无影响）
            newNode.Original_Color = Color.white;
            newNode.From_Color = Color.white;
            newNode.End_Color = Color.white;

            newNode.ValueModeIndex = 0;
            newNode.TweenValueMode = TweenValueMode.起始到默认_S_D;

            // 缓动曲线：默认 EaseInOut(0,0,1,1)，即标准缓入缓出
            newNode.Curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

            // ── 钳制插入索引后插入集合 ──
            // Clamp 到 [0, Nodes.Count]，防止调用方传入越界索引
            insertAt = Mathf.Clamp(insertAt, 0, Nodes.Count);
            Nodes.Insert(insertAt, newNode);
            EditorUtility.SetDirty(target);

            // ── 重置选中集合：新节点成为唯一选中项 ──
            // 无论之前选中多少节点，插入后都只选中新节点，
            // 这样参数面板会立即显示新节点的属性，用户可马上编辑。
            // 同时清空音效选中（普通单击语义：重新开始选择）。
            selectedNodeIndices.Clear();
            selectedNodeIndices.Add(insertAt);
            selectedSoundIndices.Clear();
            selectedIndex = insertAt;
            selectedKind = TrackKind.Node;
            Repaint();
        }
        /// <summary> 
        /// 在指定索引的节点下方插入一条新的空白动画节点
        /// <para/>
        /// 使用方式：名字行「插入」按钮点击时调用（<see cref="DrawNameColumnPanel"/> 中
        /// 记录 <c>pendingInsertIndex</c> 后延迟执行）。
        /// <para/>
        /// 本方法是 <see cref="InsertTweenNodeAt"/> 的便捷包装，只做两件事：
        /// <list type="number">
        /// <item><description>做一次边界保护（目标非空、索引有效）；</description></item>
        /// <item><description>把「在 <paramref name="index"/> 之后插入」翻译为
        /// <c>InsertTweenNodeAt(index + 1)</c>。</description></item>
        /// </list>
        /// 真正的节点构造、Undo 注册、选中重置等逻辑全部由
        /// <see cref="InsertTweenNodeAt"/> 承担。
        /// <para/>
        /// 之所以「插入到之后」而非「之前」：名字行的插入按钮位于行内，
        /// 用户点击时的直觉是「在当前行下方新增一条」，符合自上而下的列表阅读顺序。
        /// </summary>
        /// <param name="index">参考节点索引；新节点将插入到该节点之后（即索引 <c>index + 1</c> 处）</param>
        private void InsertTweenNodeAfter(int index)
        {
            // 目标为空时直接返回，避免后续调用 NRE
            if (target == null) return;

            // 索引越界保护：index 必须落在 [0, Nodes.Count - 1] 范围内
            // 越界时直接返回，不做任何操作（调用方本不应传入越界值）
            if (index < 0 || index >= Nodes.Count) return;

            // 委托给底层入口：在 index + 1 处插入
            // 注意此处不需要再 Clamp，因为 InsertTweenNodeAt 内部已做钳制
            InsertTweenNodeAt(index + 1);
        }
        /// <summary> 
        /// 删除指定索引的动画节点，并修正选中集合
        /// <para/>
        /// 本方法是节点删除的底层入口：
        /// <list type="bullet">
        /// <item><description>名字行「删除」按钮 → <c>DeleteTweenNodeAt(i)</c>
        /// （由 <see cref="DrawNameColumnPanel"/> 记录 <c>pendingDeleteIndex</c> 后延迟执行）。</description></item>
        /// </list>
        /// 批量删除请使用 <see cref="DeleteSelectedTweenNodes"/>。
        /// <para/>
        /// 执行流程：
        /// <list type="number">
        /// <item><description>注册 Undo（<see cref="Undo.RegisterCompleteObjectUndo"/>）；</description></item>
        /// <item><description>若节点仍有运行时 Tweener，先 Kill 并置空，避免残留动画继续影响目标；</description></item>
        /// <item><description>从集合移除该节点；</description></item>
        /// <item><description>重建选中集合：删除位置之后的索引全部前移一位；</description></item>
        /// <item><description>修正 <see cref="selectedIndex"/>；</description></item>
        /// <item><description>钳制垂直滚动，避免删除后出现滚动越界。</description></item>
        /// </list>
        /// </summary>
        /// <param name="index">要删除的节点索引</param>
        private void DeleteTweenNodeAt(int index)
        {
            // 前置校验：目标为空、索引越界时直接返回
            if (target == null) return;
            if (index < 0 || index >= Nodes.Count) return;

            // 注册完整对象 Undo，保证本次删除可撤销（预览期间不记录）
            if (!IsPreviewing)
                Undo.RegisterCompleteObjectUndo(target, "Delete Tween Node");

            // ── 运行时清理：先 Kill 掉仍在运行的 Tweener ──
            TweenNode node = Nodes[index];
            if (node.Tweener != null)
            {
                node.Tweener.Kill();
                node.Tweener = null;
            }

            // 从集合移除
            Nodes.RemoveAt(index);
            EditorUtility.SetDirty(target);

            // ── 重建动画选中集合：删除位置之后的索引全部前移一位 ──
            // 规则：
            //   i <  index → 保留原值
            //   i >  index → 减 1（因为前面的元素被删，索引整体前移）
            //   i == index → 丢弃（该节点已不存在）
            HashSet<int> newSelection = new HashSet<int>();
            foreach (int i in selectedNodeIndices)
            {
                if (i < index) newSelection.Add(i);
                else if (i > index) newSelection.Add(i - 1);
            }
            selectedNodeIndices.Clear();
            foreach (int i in newSelection) selectedNodeIndices.Add(i);

            // ── 修正主选中索引（仅当主选中是动画时）──
            if (selectedKind == TrackKind.Node)
            {
                if (selectedIndex == index)
                {
                    // 被删的正是主选中项：改为动画集合中最上面那个；若动画空了则切到音效
                    selectedIndex = selectedNodeIndices.Count > 0
                        ? GetTopmostSelectedIndex(selectedNodeIndices) : -1;
                }
                else if (selectedIndex > index)
                {
                    // 主选中项位于被删项之后：索引前移一位
                    selectedIndex--;
                }

                // 动画已无主选中 → 尝试切到音效
                if (selectedIndex < 0 && selectedNodeIndices.Count == 0)
                {
                    if (selectedSoundIndices.Count > 0)
                    {
                        selectedKind = TrackKind.Sound;
                        selectedIndex = GetTopmostSelectedIndex(selectedSoundIndices);
                    }
                    else
                    {
                        selectedKind = TrackKind.无;
                    }
                }
            }
            // 其余情况（selectedKind == Sound / 无）保持不变

            // ── 钳制垂直滚动 ──
            // 删除后内容高度变小，原 scrollPos.y 可能超出新的滚动上限，
            // 需重新钳制并同步 nameScroll.y，保持名字列与轨道对齐。
            scrollPos.y = Mathf.Clamp(scrollPos.y, 0f, CalculateMaxVerticalScroll());
            nameScroll.y = scrollPos.y;
            Repaint();
        }
        /// <summary> 
        /// 删除所有当前选中的动画节点（批量删除）
        /// <para/>
        /// 使用方式：在 Clip 区 / 名字列选中若干节点后，按下 Delete 或 Backspace 键触发
        /// （见 <see cref="HandleKeyboardShortcuts"/>）。
        /// <para/>
        /// 与 <see cref="DeleteTweenNodeAt"/> 的关键区别：
        /// <list type="bullet">
        /// <item><description>本方法<b>倒序</b>删除，避免正序删除时索引错位；</description></item>
        /// <item><description>本方法删除后<b>只清空动画选中</b>，音效选中保留。</description></item>
        /// </list>
        /// <para/>
        /// 为什么倒序删除：删除索引 i 会使其后所有元素前移一位，
        /// 若从 0 开始正序删除，则后续原本记录的下标全部失效；
        /// 从大到小删除则不会影响尚未处理的下标。
        /// </summary>
        private void DeleteSelectedTweenNodes()
        {
            // 前置校验：目标为空、无选中项时直接返回
            if (target == null) return;
            if (selectedNodeIndices.Count == 0) return;

            // 注册完整对象 Undo，保证批量删除可一次撤销（预览期间不记录）
            if (!IsPreviewing)
                Undo.RegisterCompleteObjectUndo(target, "Delete Tween Nodes");

            // ── 拷贝选中集合到列表，并降序排序 ──
            // 降序是为了「从后往前删」，使未处理的下标始终有效。
            List<int> indices = new List<int>(selectedNodeIndices);
            indices.Sort((a, b) => b.CompareTo(a));

            // ── 按降序逐个删除 ──
            foreach (int i in indices)
            {
                // 越界保护：理论上不会触发，此处仅作防御
                if (i < 0 || i >= Nodes.Count) continue;

                // 与 DeleteTweenNodeAt 一致：先 Kill 运行时 Tweener，
                // 否则节点虽删但动画仍在跑。
                TweenNode node = Nodes[i];
                if (node.Tweener != null)
                {
                    node.Tweener.Kill();
                    node.Tweener = null;
                }

                Nodes.RemoveAt(i);
            }

            EditorUtility.SetDirty(target);

            // ── 清空动画选中 ──
            selectedNodeIndices.Clear();

            // ── 修正主选中：若主选中是动画且已空，切到音效（如果还有）──
            if (selectedKind == TrackKind.Node)
            {
                if (selectedSoundIndices.Count > 0)
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

            // ── 钳制垂直滚动 ──
            scrollPos.y = Mathf.Clamp(scrollPos.y, 0f, CalculateMaxVerticalScroll());
            nameScroll.y = scrollPos.y;
            Repaint();
        }
        #endregion

        #region 工具：音效增删
        /// <summary>
        /// 在指定索引处插入一条新的空白音效。
        /// </summary>
        private void InsertSoundAt(int insertAt)
        {
            if (target == null || target.Equals(null)) return;

            if (!IsPreviewing)
                Undo.RegisterCompleteObjectUndo(target, "Insert Tween Sound");

            TweenSound s = new TweenSound();
            s.Sound = null;
            s.Path = "";
            s.Timing = "";
            s.Delay = 0f;
            s.Volume = 1f;
            s.MinPitch = 1f;
            s.MaxPitch = 1f;

            insertAt = Mathf.Clamp(insertAt, 0, Sounds.Count);
            Sounds.Insert(insertAt, s);
            EditorUtility.SetDirty(target);

            // 新音效成为唯一选中项；同时清空动画选中（普通单击语义）
            selectedSoundIndices.Clear();
            selectedSoundIndices.Add(insertAt);
            selectedNodeIndices.Clear();
            selectedIndex = insertAt;
            selectedKind = TrackKind.Sound;
            Repaint();
        }
        /// <summary>
        /// 在指定音效下方插入一条新的空白音效。
        /// </summary>
        private void InsertSoundAfter(int index)
        {
            if (target == null) return;
            if (index < 0 || index >= Sounds.Count) return;
            InsertSoundAt(index + 1);
        }
        /// <summary>
        /// 删除指定索引的音效，并修正选中集合。
        /// </summary>
        private void DeleteSoundAt(int index)
        {
            if (target == null) return;
            if (index < 0 || index >= Sounds.Count) return;

            if (!IsPreviewing)
                Undo.RegisterCompleteObjectUndo(target, "Delete Tween Sound");

            Sounds.RemoveAt(index);
            EditorUtility.SetDirty(target);

            // ── 重建音效选中集合：删除位置之后的索引全部前移一位 ──
            HashSet<int> newSelection = new HashSet<int>();
            foreach (int i in selectedSoundIndices)
            {
                if (i < index) newSelection.Add(i);
                else if (i > index) newSelection.Add(i - 1);
            }
            selectedSoundIndices.Clear();
            foreach (int i in newSelection) selectedSoundIndices.Add(i);

            // ── 修正主选中索引（仅当主选中是音效时）──
            if (selectedKind == TrackKind.Sound)
            {
                if (selectedIndex == index)
                    selectedIndex = selectedSoundIndices.Count > 0
                        ? GetTopmostSelectedIndex(selectedSoundIndices) : -1;
                else if (selectedIndex > index)
                    selectedIndex--;

                // 音效已无主选中 → 尝试切到动画
                if (selectedIndex < 0 && selectedSoundIndices.Count == 0)
                {
                    if (selectedNodeIndices.Count > 0)
                    {
                        selectedKind = TrackKind.Node;
                        selectedIndex = GetTopmostSelectedIndex(selectedNodeIndices);
                    }
                    else
                    {
                        selectedKind = TrackKind.无;
                    }
                }
            }

            scrollPos.y = Mathf.Clamp(scrollPos.y, 0f, CalculateMaxVerticalScroll());
            nameScroll.y = scrollPos.y;
            Repaint();
        }
        /// <summary>
        /// 删除所有当前选中的音效。
        /// </summary>
        private void DeleteSelectedSounds()
        {
            if (target == null) return;
            if (selectedSoundIndices.Count == 0) return;

            if (!IsPreviewing)
                Undo.RegisterCompleteObjectUndo(target, "Delete Tween Sounds");

            List<int> indices = new List<int>(selectedSoundIndices);
            indices.Sort((a, b) => b.CompareTo(a));
            foreach (int i in indices)
            {
                if (i < 0 || i >= Sounds.Count) continue;
                Sounds.RemoveAt(i);
            }
            EditorUtility.SetDirty(target);

            // ── 清空音效选中 ──
            selectedSoundIndices.Clear();

            // ── 修正主选中：若主选中是音效且已空，切到动画（如果还有）──
            if (selectedKind == TrackKind.Sound)
            {
                if (selectedNodeIndices.Count > 0)
                {
                    selectedKind = TrackKind.Node;
                    selectedIndex = GetTopmostSelectedIndex(selectedNodeIndices);
                }
                else
                {
                    selectedIndex = -1;
                    selectedKind = TrackKind.无;
                }
            }

            // ── 钳制垂直滚动 ──
            scrollPos.y = Mathf.Clamp(scrollPos.y, 0f, CalculateMaxVerticalScroll());
            nameScroll.y = scrollPos.y;
            Repaint();
        }
        #endregion

        #region 工具：清空窗口以及面板配置
        /// <summary> 
        /// 【测试】清空本窗口在 EditorPrefs 中保存的所有 key，
        /// 使下次打开等同于「首次运行」：窗口尺寸 / 名字列宽 / 参数面板宽全部回落到默认值
        /// <para/>
        /// 清空的 key 共四个：
        /// <list type="bullet">
        /// <item><description><see cref="PrefKey_WindowWidth"/>：窗口宽度；</description></item>
        /// <item><description><see cref="PrefKey_WindowHeight"/>：窗口高度；</description></item>
        /// <item><description><see cref="PrefKey_NameWidthWidth"/>：名字列宽度；</description></item>
        /// <item><description><see cref="PrefKey_ParamWidthHeight"/>：参数面板宽度。</description></item>
        /// </list>
        /// <para/>
        /// 注意：本方法<b>不会</b>动 <c>target</c> 上的视图状态
        /// （缩放 <c>Timeline_TrackPosition</c>、滚动 <c>Timeline_TrackScroll</c>、
        /// 轨道高 <c>Timeline_TrackHeight</c>、吸附 <c>Timeline_TrackSnapEnabled</c>），
        /// 因为那些状态存在 <see cref="XHud_Module_Primitive_Tween"/> 组件上，而非 EditorPrefs 中。
        /// <para/>
        /// 调用方式：默认被 <c>[MenuItem]</c> 注释掉，需手动取消注释后从菜单
        /// <c>Tools/XHud/Tween Tracker/Reset Window Prefs (Test)</c> 触发。
        /// </summary>
        //[MenuItem("Tools/XHud/Tween Tracker/Reset Window Prefs (Test)")]
        private static void ResetAllEditorPrefsForTest()
        {
            // ── 清空四个 EditorPrefs key ──
            // 逐个调用 XGUI.x_Editor_Data_Clear，内部会从 EditorPrefs 中移除对应项；
            // 下次 LoadPersistedWindowSize / LoadClampedFloatPreference 时会读不到值，
            // 从而回落到各自方法中的默认值。
            XGUI.x_Editor_Data_Clear(PrefKey_WindowWidth);
            XGUI.x_Editor_Data_Clear(PrefKey_WindowHeight);
            XGUI.x_Editor_Data_Clear(PrefKey_NameWidthWidth);
            XGUI.x_Editor_Data_Clear(PrefKey_ParamWidthHeight);

            // 提示日志：确认操作已执行
            Debug.Log("[XHud] Primitive Tween Tracker EditorPrefs 已清空，窗口已回落默认尺寸");
        }
        #endregion
    }
}