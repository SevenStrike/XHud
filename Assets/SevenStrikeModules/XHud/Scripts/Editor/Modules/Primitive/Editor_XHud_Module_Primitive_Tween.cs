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
    using SevenStrikeModules.XHud.Utilitys;
    using SevenStrikeModules.XTween;
    using SevenStrikeModules.XTween.Editor;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.InputSystem.LowLevel;
    using UnityEngine.UI;
    using Random = UnityEngine.Random;

    /// <summary>
    /// 图元动画器的自定义 Inspector 编辑器
    /// <para/>
    /// 负责绘制 <see cref="XHud_Module_Primitive_Tween"/> 组件的完整检视面板，包含：
    /// <list type="bullet">
    /// <item><description>快捷功能栏：预览播放 / 停止、打开时间轴编辑器；</description></item>
    /// <item><description>动画节点列表：基于 <see cref="ReorderableList"/> 的可拖拽列表；</description></item>
    /// <item><description>节点参数面板：类型 / 时长 / 延迟 / 循环 / 缓动 / 曲线 / 时机 / 动向 / 数值；</description></item>
    /// <item><description>全局选项：调试 / 静音 / 元素联动；</description></item>
    /// <item><description>状态统计：节点数、耗时、倍增耗时；</description></item>
    /// <item><description>批量模式：多选组件时的切换与统计。</description></item>
    /// </list>
    /// <para/>
    /// 支持多选编辑（<see cref="CanEditMultipleObjects"/>），但动画节点列表本身不支持批量操作。
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(XHud_Module_Primitive_Tween))]
    public class Editor_XHud_Module_Primitive_Tween : Editor
    {
        #region 字段 - 组件与列表
        /// <summary>
        /// 当前编辑的目标组件（单选模式下使用）
        /// </summary>
        private XHud_Module_Primitive_Tween BaseScript;
        /// <summary>
        /// 动画节点列表的 ReorderableList 包装
        /// </summary>
        private ReorderableList AnimateTweenNodesList;
        /// <summary>
        /// XHud 管理器实例，用于访问曲线库 / 音效库 / 全局倍增系数
        /// </summary>
        private XHud_Manager HudManager;
        #endregion

        #region 字段 - 序列化属性
        /// <summary>
        /// 序列化属性集合：调试、节点列表、预览中、全局倍增、静音、最大/最小耗时、倍增耗时、元素联动、预览时机
        /// </summary>
        private SerializedProperty sp_UseDebug, sp_PrimitiveTweenNodes, sp_TweenIsPreviewing, sp_GlobalDuration, sp_MutePlay, sp_MaxTimer, sp_MinTimer, sp_MinTimerWithGlobalDuration, sp_MaxTimerWithGlobalDuration, sp_IgnoreElementAnimationPlay, sp_PreviewTiming;
        #endregion

        #region 字段 - 绘制缓存
        /// <summary>
        /// 通用绘制矩形缓存，避免每帧分配新的 Rect 对象
        /// </summary>
        Rect draw_rect;
        /// <summary>
        /// 动画节点类型的名称数组（从枚举一次性获取）
        /// </summary>
        string[] TweenNodeTypes;
        #endregion

        #region 字段 - GUI 参数
        /// <summary>
        /// 多选模式下当前查看的组件索引
        /// </summary>
        private int MultiPrimitiveTween_Index;
        private bool isPreviewing = false;
        #endregion

        #region 字段 - 选项文字
        /// <summary>调试开关的显示文字</summary>
        string[] opt_debug = new string[2] { "关闭", "调试" };
        /// <summary>静音开关的显示文字</summary>
        string[] opt_mute = new string[] { "正常", "静音" };
        /// <summary>元素联动开关的显示文字</summary>
        string[] opt_control = new string[] { "可控", "忽略" };
        /// <summary>
        /// 当前组件动画列表中所有出现过的时机名称
        /// </summary>
        string[] opt_preview_timings;
        #endregion

        #region 字段 - 图标
        /// <summary>
        /// 图标集合：预览播放/停止、左右箭头、数值连接器、数值圆点、展开/折叠、组件标题、打开时间轴
        /// </summary>
        private Texture2D prw_play_r, prw_play_p, prw_stop_r, prw_stop_p, left_arrow_r, left_arrow_p, right_arrow_r, right_arrow_p, dir_connector_r, dir_connector_p, anim_dot_r, anim_dot_p, icon_unfold_r, icon_unfold_p, icon_fold_r, icon_fold_p, icon_main, opentrack_r, opentrack_p;
        #endregion

        #region 字段 - 宿主组件缓存
        /// <summary>父级 XHud 文字组件（用于判断时机类型）</summary>
        private XHud_Module_Text HudText;
        /// <summary>父级 XHud TmpText 组件（用于判断时机类型）</summary>
        private XHud_Module_TmpText HudTmpText;
        /// <summary>父级 XHud 按钮组件（用于判断时机类型）</summary>
        private XHud_Module_Button HudButton;
        /// <summary>父级 XHud 进度条组件（用于判断时机类型）</summary>
        private XHud_Module_Progress HudProgress;
        /// <summary>父级 XHud 开关组件（用于判断时机类型）</summary>
        private XHud_Module_Toggle HudToggle;
        /// <summary>父级 XHud 滑动条组件（用于判断时机类型）</summary>
        private XHud_Module_Slider HudSlider;
        /// <summary>父级 XHud 选项组件（用于判断时机类型）</summary>
        private XHud_Module_Option HudOption;
        #endregion

        #region 字段 - 音效预览
        /// <summary>
        /// 预览音效播放时生成的 AudioSource 列表，用于停止时统一销毁
        /// </summary>
        private List<AudioSource> Preview_PrimitiveTweens_SoundList = new List<AudioSource>();
        /// <summary>
        /// 预览音效播放的协程句柄列表，用于停止时统一终止
        /// </summary>
        private List<XCoroutine> Preview_PrimitiveTweens_SoundCoroutineList_Stop = new List<XCoroutine>();
        #endregion

        #region 字段 - 颜色
        /// <summary>
        /// 数值圆点按钮使用的红色（起始值标识色）
        /// </summary>
        private Color dot_color_red;
        #endregion

        #region 字段 - 批量操作
        /// <summary>
        /// 当前多选状态下所有被选中的组件数组
        /// </summary>
        XHud_Module_Primitive_Tween[] SelectedObjects;
        /// <summary>
        /// 缓存当前所有被选中的组件引用到 <see cref="SelectedObjects"/>
        /// <para/>
        /// 多选时逐个转换，单选时只填充一个元素。
        /// </summary>
        private void CacheSelectedTargets()
        {
            if (targets.Length > 1)
            {
                SelectedObjects = new XHud_Module_Primitive_Tween[targets.Length];
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    var t = targets[i];
                    SelectedObjects[i] = (XHud_Module_Primitive_Tween)t;
                }
            }
            else
            {
                SelectedObjects = new XHud_Module_Primitive_Tween[targets.Length];
                SelectedObjects[0] = (XHud_Module_Primitive_Tween)target;
            }
        }
        /// <summary>
        /// 判断当前是否处于多选状态
        /// </summary>
        /// <returns>true 表示选中了多个组件</returns>
        private bool IsMultiSelection()
        {
            if (SelectedObjects == null)
                return false;
            if (SelectedObjects.Length > 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 生命周期
        /// <summary>
        /// Unity 启用回调
        /// <para/>
        /// 执行流程：
        /// <list type="number">
        /// <item><description>获取 HudManager 单例；</description></item>
        /// <item><description>缓存目标组件与所有选中组件；</description></item>
        /// <item><description>缓存所有序列化属性引用；</description></item>
        /// <item><description>加载所有图标资源；</description></item>
        /// <item><description>缓存宿主组件（用于时机判断）；</description></item>
        /// <item><description>构造 ReorderableList 并绑定所有回调；</description></item>
        /// <item><description>首次收集预览时机名称。</description></item>
        /// </list>
        /// </summary>
        private void OnEnable()
        {
            HudManager = XHud_Dashboard.HudManagerGet();

            BaseScript = (XHud_Module_Primitive_Tween)target;

            CacheSelectedTargets();

            #region 序列化属性缓存
            sp_UseDebug = serializedObject.FindProperty("UseDebug");
            sp_PrimitiveTweenNodes = serializedObject.FindProperty("PrimitiveTweenNodes");
            sp_TweenIsPreviewing = serializedObject.FindProperty("TweenIsPreviewing");
            sp_GlobalDuration = serializedObject.FindProperty("GlobalDuration");
            sp_MutePlay = serializedObject.FindProperty("MutePlay");
            sp_MaxTimer = serializedObject.FindProperty("MaxTimer");
            sp_MinTimer = serializedObject.FindProperty("MinTimer");
            sp_MinTimerWithGlobalDuration = serializedObject.FindProperty("MinTimerWithGlobalDuration");
            sp_MaxTimerWithGlobalDuration = serializedObject.FindProperty("MaxTimerWithGlobalDuration");
            sp_IgnoreElementAnimationPlay = serializedObject.FindProperty("IgnoreElementAnimationPlay");
            sp_PreviewTiming = serializedObject.FindProperty("PreviewTiming");
            #endregion

            dot_color_red = XGUI_Utilitys.HexString_To_Color("ff4848");

            #region 获取图标
            icon_main = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/icon_main");
            left_arrow_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/left_arrow_r");
            left_arrow_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/left_arrow_p");
            right_arrow_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/right_arrow_r");
            right_arrow_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/right_arrow_p");
            prw_play_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/prw_play_r");
            prw_play_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/prw_play_p");
            prw_stop_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/prw_stop_r");
            prw_stop_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/prw_stop_p");
            dir_connector_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/dir_connector_r");
            dir_connector_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/dir_connector_p");
            anim_dot_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/anim_dot_r");
            anim_dot_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/anim_dot_p");
            icon_fold_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/icon_fold_r");
            icon_fold_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/icon_fold_p");
            icon_unfold_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/icon_unfold_r");
            icon_unfold_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/icon_unfold_p");
            opentrack_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/opentrack_r");
            opentrack_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/opentrack_p");
            #endregion

            TweenNodeTypes = System.Enum.GetNames(typeof(TweenNodeType));

            CacheHostComponents();

            #region 动画列表
            AnimateTweenNodesList = new ReorderableList(serializedObject, sp_PrimitiveTweenNodes)
            {
                displayAdd = true,
                displayRemove = true,
                draggable = true,
                drawHeaderCallback = rect =>
                {
                    XGUI.gui_label(
                        rect: rect,
                        text: new GUIContent("动画堆栈"),
                        text_color: Color.gray,
                        size: XGUIFontSize.M,
                        clipping: TextClipping.Clip,
                        anchor: TextAnchor.MiddleLeft,
                        offset: new Vector2(0, 0),
                        font_style: FontStyle.Normal);
                },
                drawElementBackgroundCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    if (index >= 0)
                    {
                        if (isFocused)
                            XGUI.gui_box(new Rect(rect.x + 20, rect.y + 5, 2, 11), XHud_Dashboard.Theme_Primary);
                    }
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    SerializedProperty sp_node = sp_PrimitiveTweenNodes.GetArrayElementAtIndex(index);

                    #region 获取动画节点的序列化字段
                    SerializedProperty sp_ID = sp_node.FindPropertyRelative("ID");
                    SerializedProperty sp_Name = sp_node.FindPropertyRelative("Indicator");
                    SerializedProperty sp_Duration = sp_node.FindPropertyRelative("Duration");
                    SerializedProperty sp_Enabled = sp_node.FindPropertyRelative("Enabled");
                    SerializedProperty sp_TweenSounds = sp_node.FindPropertyRelative("TweenSounds");
                    SerializedProperty sp_Delay = sp_node.FindPropertyRelative("Delay");
                    SerializedProperty sp_Ease = sp_node.FindPropertyRelative("Ease");
                    SerializedProperty sp_Rewind_Set_Startvalue = sp_node.FindPropertyRelative("Rewind_Set_Startvalue");
                    SerializedProperty sp_Complete_Set_Endvalue = sp_node.FindPropertyRelative("Complete_Set_Endvalue");
                    SerializedProperty sp_AnimationCurveName = sp_node.FindPropertyRelative("AnimationCurveName");
                    SerializedProperty sp_Curve = sp_node.FindPropertyRelative("Curve");
                    SerializedProperty sp_IsFold = sp_node.FindPropertyRelative("IsFold");
                    SerializedProperty sp_Timings = sp_node.FindPropertyRelative("Timings");
                    SerializedProperty sp_ValueModeIndex = sp_node.FindPropertyRelative("ValueModeIndex");
                    SerializedProperty sp_TweenValueMode = sp_node.FindPropertyRelative("TweenValueMode");
                    SerializedProperty sp_LoopType = sp_node.FindPropertyRelative("LoopType");
                    SerializedProperty sp_LoopCount = sp_node.FindPropertyRelative("LoopCount");
                    SerializedProperty sp_Progress = sp_node.FindPropertyRelative("Progress");
                    SerializedProperty sp_Type = sp_node.FindPropertyRelative("Type");
                    #endregion

                    Rect rect_panel_title = rect;

                    #region 折叠按钮
                    rect_panel_title.Set(rect.x + 5, rect.y + 5, 12, 12);
                    if (XGUI.gui_button(
                        rect: rect_panel_title,
                        tooltip: "",
                        tex_release: sp_IsFold.boolValue ? icon_fold_r : icon_unfold_r,
                        tex_press: sp_IsFold.boolValue ? icon_fold_p : icon_unfold_p,
                        tex_gui_color: Color.white,
                        margin: new RectOffset(0, 0, 0, 0),
                        padding: new RectOffset(0, 0, 0, 0)))
                    {
                        sp_IsFold.boolValue = !sp_IsFold.boolValue;
                        sp_IsFold.serializedObject.ApplyModifiedProperties();
                    }
                    #endregion

                    float cur_width = XGUI.GetCurrentWindowWidth();
                    //Debug.Log(cur_width);

                    #region 名称
                    bool is_shrink_name = cur_width > 180;
                    rect_panel_title.Set(rect.x + 25, rect.y + 3, rect.width - (is_shrink_name ? 80 : 30), XGUI.GetSingleLineHeight());
                    if (cur_width > 120)
                    {
                        XGUI.gui_property_field(
                            rect: rect_panel_title,
                            title: GUIContent.none,
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 0,
                            prop: sp_Name);
                        sp_Name.serializedObject.ApplyModifiedProperties();
                    }
                    #endregion

                    #region 开关
                    bool is_shrink_toggle = cur_width > 180;
                    if (is_shrink_toggle)
                    {
                        rect_panel_title.Set(rect.x + (rect.width - 90), rect.y + 1.5f, 85, XGUI.GetSingleLineHeight());
                        XGUI.gui_toggle(
                            rect: rect_panel_title,
                            title: "",
                            title_color: Color.white,
                            title_size: XGUIFontSize.M,
                            title_font_style: FontStyle.Normal,
                            title_padding: new RectOffset(0, 0, 0, 0),
                            title_width: 0,
                            tog_interval: 10,
                            prop: sp_Enabled,
                            tog_style: XGUIToggleStyle.实体,
                            tog_mixed_options: new string[] { "关闭", "开启" },
                            tog_mixed_text_size: XGUIFontSize.M,
                            tog_mixed_text_color: Color.black,
                            tog_mixed_text_padding: new RectOffset(0, 0, 0, 0),
                            tog_mixed_text_anchor: TextAnchor.MiddleCenter,
                            tog_mixed_text_font_style: FontStyle.Normal,
                            tog_bg_off_color: Color.gray,
                            tog_bg_on_color: XHud_Dashboard.Theme_Primary,
                            tog_handler_off_color: Color.white,
                            tog_handler_on_color: Color.white,
                            tog_mixed_bg_color_gui: XHud_Dashboard.Theme_Primary);
                        sp_Enabled.serializedObject.ApplyModifiedProperties();
                    }
                    #endregion

                    Rect rect_panel_root = rect;

                    if (!sp_IsFold.boolValue && cur_width > 145)
                    {
                        XGUI.gui_box(new Rect(rect.x + 5, rect.y + 30, rect.width - 10, 1), Color.gray * 0.85f);

                        float baseheight = rect.y + 40;

                        #region 动画类型
                        bool is_shrink_type_popup = cur_width < 270;

                        rect_panel_root.Set(rect.x + 5, baseheight, rect.width - (is_shrink_type_popup ? 10 : 50), XGUI.GetSingleLineHeight());
                        sp_Type.intValue = XGUI.gui_int_popup(
                            rect: rect_panel_root,
                            title_width: 0,
                            prop: sp_Type,
                            options: TweenNodeTypes,
                            opt_text_size: XGUIFontSize.M,
                            opt_text_color: Color.black,
                            opt_text_padding: new RectOffset(10, 10, 0, 0),
                            opt_anchor: TextAnchor.MiddleCenter,
                            opt_font_style: FontStyle.Normal,
                            opt_bg_fill: XGUIFilled.实体,
                            opt_bg_color: XGUIColor.亮白,
                            opt_bg_color_gui: Color.white,
                            icon_arrow_color: Color.black,
                            width_limite: 270,
                            usearrow: true);

                        sp_Type.serializedObject.ApplyModifiedProperties();
                        #endregion

                        TweenNodeType nodetype = (TweenNodeType)sp_Type.enumValueIndex;

                        #region 判断文字组件是否为库同步样式状态
                        bool IsTextColorMode = false;
                        if (BaseScript.controller != null)
                        {
                            ModuleType animtype = BaseScript.controller.GetModuleType();
                            if (nodetype == TweenNodeType.c_颜色)
                            {
                                if (animtype == Enums.ModuleType.Text || animtype == Enums.ModuleType.TmpText)
                                {
                                    XHud_Module_Text text = BaseScript.controller.mod_Text;
                                    XHud_Module_TmpText tmptext = BaseScript.controller.mod_TmpText;
                                    if ((text && text.StyleLibSynching && text.TextStyleInfo.LibStyle_Effect_color) ||
                                    (tmptext && tmptext.StyleLibSynching && tmptext.TextStyleInfo.LibStyle_Effect_color))
                                    {
                                        IsTextColorMode = true;
                                    }
                                }
                            }
                        }
                        #endregion

                        if (!IsTextColorMode)
                        {
                            #region 刷新ID按钮
                            if (!is_shrink_type_popup)
                            {
                                rect_panel_root.Set(rect.x + (rect.width - 50), baseheight, 60, XGUI.GetSingleLineHeight());
                                if (XGUI.gui_button(
                                    rect: rect_panel_root,
                                    text: $"{sp_ID.intValue}",
                                    tooltip: "此ID值代表该动画节点的身份标识",
                                    anchor: TextAnchor.MiddleCenter,
                                    btn_fill: XGUIFilled.透明,
                                    btn_text_color: XHud_Dashboard.Theme_Primary,
                                    press_fill: XGUIFilled.透明,
                                    press_color: XGUIColor.深空灰,
                                    press_text_color: Color.white,
                                    font_size: XGUIFontSize.S,
                                    margin: new RectOffset(0, 0, 0, 0),
                                    padding: new RectOffset(0, 0, 0, 0)))
                                {
                                    EditorApplication.delayCall += () =>
                                    {
                                        string res = XGUI.dialog(
                                            type: XGUIDialogType.警告,
                                            windowtitle: "XHud - 图元动画器消息",
                                            title: "更新动画 ID",
                                            msg: $"请注意！如果您通过该动画ID来控制动画状态的话，请记得同步修改您的脚本参数对应的目标ID值，否则将导致您无法获取目标动画！",
                                            ok: "更新",
                                            cancel: "暂不",
                                            PrimaryIndex: 0,
                                            usemodal: true,
                                            themecolor: XHud_Dashboard.Theme_Primary);

                                        if (res == "更新")
                                        {
                                            int id = BaseScript.TweenNode_GenerateId();
                                            sp_ID.intValue = id;
                                            sp_ID.serializedObject.ApplyModifiedProperties();
                                        }
                                    };
                                }
                            }
                            #endregion

                            #region 耗时
                            rect_panel_root.Set(rect.x + 5, baseheight + 25, rect.width / 2 - 10, XGUI.GetSingleLineHeight());
                            XGUI.gui_property_field(
                                rect: rect_panel_root,
                                title: new GUIContent("耗时"),
                                title_size: XGUIFontSize.M,
                                title_hover_color: XHud_Dashboard.Theme_Primary,
                                title_width: 30,
                                prop: sp_Duration);
                            #endregion

                            #region 延迟
                            rect_panel_root.Set(rect.x + rect.width / 2 + 10, baseheight + 25, rect.width / 2 - 15, XGUI.GetSingleLineHeight());
                            XGUI.gui_property_field(
                               rect: rect_panel_root,
                               title: new GUIContent("延迟"),
                               title_size: XGUIFontSize.M,
                               title_hover_color: XHud_Dashboard.Theme_Primary,
                               title_width: 30,
                               prop: sp_Delay);
                            #endregion

                            #region 循环
                            rect_panel_root.Set(rect.x + 5, baseheight + 50, rect.width / 2 - 10, XGUI.GetSingleLineHeight());
                            XGUI.gui_property_field(
                                rect: rect_panel_root,
                                title: new GUIContent("循环"),
                                title_size: XGUIFontSize.M,
                                title_hover_color: XHud_Dashboard.Theme_Primary,
                                title_width: 30,
                                prop: sp_LoopType);
                            #endregion

                            #region 循环次数
                            rect_panel_root.Set(rect.x + rect.width / 2 + 10, baseheight + 50, rect.width / 2 - 15, XGUI.GetSingleLineHeight());
                            XGUI.gui_property_field(
                               rect: rect_panel_root,
                               title: new GUIContent("次数"),
                               title_size: XGUIFontSize.M,
                               title_hover_color: XHud_Dashboard.Theme_Primary,
                               title_width: 30,
                               prop: sp_LoopCount);
                            #endregion

                            #region 缓动
                            rect_panel_root.Set(rect.x + 5, baseheight + 75, rect.width - 10, XGUI.GetSingleLineHeight());
                            XGUI.gui_property_field(
                                rect: rect_panel_root,
                                title: new GUIContent("缓动"),
                                title_size: XGUIFontSize.M,
                                title_hover_color: XHud_Dashboard.Theme_Primary,
                                title_width: 30,
                                prop: sp_Ease);
                            #endregion

                            #region 曲线
                            bool is_shrink_curve = cur_width < 150;

                            rect_panel_root.Set(rect.x + 5, baseheight + 100, is_shrink_curve ? rect.width - 10 : rect.width / 2, XGUI.GetSingleLineHeight());
                            XGUI.gui_property_field(
                               rect: rect_panel_root,
                               title: new GUIContent("曲线"),
                               title_size: XGUIFontSize.M,
                               title_hover_color: XHud_Dashboard.Theme_Primary,
                               title_width: 30,
                               prop: sp_Curve);
                            #endregion

                            #region 曲线列表
                            if (!is_shrink_curve)
                            {
                                if (HudManager != null && HudManager.Hud_Curves != null && !HudManager.Hud_Curves.CurveLibrary_IsEmpty())
                                {
                                    string[] curves_names = HudManager.Hud_Curves.CurveLibrary_GetCurveNames();

                                    rect_panel_root.Set(rect.x + rect.width / 2 + 10, baseheight + 100, rect.width / 2 - 15, XGUI.GetSingleLineHeight() + 3);
                                    XGUI.ChangedCheck_Start();
                                    XGUI.gui_string_popup(
                                        rect: rect_panel_root,
                                        prop: sp_AnimationCurveName,
                                        title_width: 0,
                                        options: curves_names,
                                        opt_text_size: XGUIFontSize.S,
                                        opt_text_color: Color.white,
                                        opt_text_padding: new RectOffset(10, 10, 0, 0),
                                        opt_anchor: TextAnchor.MiddleCenter,
                                        opt_font_style: FontStyle.Normal,
                                        opt_bg_fill: XGUIFilled.实体,
                                        opt_bg_color: XGUIColor.亮白,
                                        opt_bg_color_gui: Color.black * 0.6f,
                                        icon_arrow_color: Color.white,
                                        width_limite: 100,
                                        usearrow: false);

                                    if (!HudManager.Hud_Curves.CurvesLibrary_NameIsValid(sp_AnimationCurveName.stringValue))
                                    {
                                        sp_AnimationCurveName.stringValue = "";
                                    }
                                    if (XGUI.ChangedCheck_End())
                                    {
                                        sp_Curve.animationCurveValue = HudManager.Hud_Curves.CurveLibrary_GetCurve(sp_AnimationCurveName.stringValue);
                                    }
                                    sp_AnimationCurveName.serializedObject.ApplyModifiedProperties();
                                }
                            }
                            #endregion

                            XGUI.gui_box(new Rect(rect.x + 5, baseheight + 130, rect.width - 10, 1), Color.gray * 0.85f);

                            #region 时机
                            string[] TimingType = null;

                            #region 根据类型来判断动画时机的枚举条件
                            if (HudButton != null)
                            {
                                TimingType = new string[9] { "无", "鼠标进入", "鼠标退出", "鼠标按下", "鼠标松开", "鼠标长按", "鼠标点击", "鼠标选中", "鼠标取消选中" };
                            }
                            else if (HudProgress != null)
                            {
                                TimingType = new string[5] { "无", "进度开始时", "进度变化时", "进度结束时", "进度重置时" };
                            }
                            else if (HudToggle != null)
                            {
                                TimingType = new string[7] { "无", "开关打开时", "开关关闭时", "开关按下时", "开关抬起时", "开关变化时", "开关变化中" };
                            }
                            else if (HudSlider != null)
                            {
                                TimingType = new string[5] { "无", "按下滑动条", "松开滑动条", "滑动条数值改变", "滑动条数值变化中" };
                            }
                            else if (HudOption != null)
                            {
                                TimingType = new string[5] { "无", "点击选项", "光标移动开始", "光标移动结束", "光标位置改变" };
                            }
                            else
                            {
                                TimingType = new string[4] { "元素进入时", "元素进入后", "元素退出时", "自定义" };
                            }
                            #endregion

                            rect_panel_root.Set(rect.x + 5, baseheight + 145, rect.width / 2 - 15, XGUI.GetSingleLineHeight());
                            XGUI.ChangedCheck_Start();
                            sp_Timings.stringValue = XGUI.gui_string_popup(
                                rect: rect_panel_root,
                                title: cur_width <= 322 ? "" : "时机",
                                title_color: Color.white,
                                title_size: XGUIFontSize.M,
                                title_font_style: FontStyle.Normal,
                                title_padding: new RectOffset(0, 0, 0, 0),
                                title_width: 35,
                                prop: sp_Timings,
                                options: TimingType,
                                opt_text_size: XGUIFontSize.M,
                                opt_text_color: Color.black,
                                opt_text_padding: new RectOffset(10, 10, 0, 0),
                                opt_anchor: TextAnchor.MiddleCenter,
                                opt_font_style: FontStyle.Normal,
                                opt_bg_fill: XGUIFilled.实体,
                                opt_bg_color: XGUIColor.亮白,
                                opt_bg_color_gui: XHud_Dashboard.Theme_Primary,
                                width_limite: 100,
                                usearrow: false);
                            sp_Timings.serializedObject.ApplyModifiedProperties();
                            if (XGUI.ChangedCheck_End())
                            {
                                // 再次收集动画列表所有动画时机名称
                                CollectPreviewTimings(BaseScript);
                            }
                            #endregion

                            #region 动向
                            string[] dir_type = new string[4] { "起始 -> 默认", "默认 -> 结束", "起始 -> 结束", "当前 -> 结束" };

                            rect_panel_root.Set(rect.x + rect.width / 2, baseheight + 145, rect.width / 2 - 5, XGUI.GetSingleLineHeight());
                            XGUI.ChangedCheck_Start();
                            sp_ValueModeIndex.intValue = XGUI.gui_int_popup(
                                rect: rect_panel_root,
                                title: cur_width <= 322 ? "" : "动向",
                                title_color: Color.white,
                                title_size: XGUIFontSize.M,
                                title_font_style: FontStyle.Normal,
                                title_padding: new RectOffset(0, 0, 0, 0),
                                title_width: 35,
                                prop: sp_ValueModeIndex,
                                options: dir_type,
                                opt_text_size: XGUIFontSize.M,
                                opt_text_color: Color.black,
                                opt_text_padding: new RectOffset(10, 10, 0, 0),
                                opt_anchor: TextAnchor.MiddleCenter,
                                opt_font_style: FontStyle.Normal,
                                opt_bg_fill: XGUIFilled.实体,
                                opt_bg_color: XGUIColor.亮白,
                                opt_bg_color_gui: XHud_Dashboard.Theme_Primary,
                                width_limite: 110,
                                usearrow: false);
                            sp_ValueModeIndex.serializedObject.ApplyModifiedProperties();
                            if (XGUI.ChangedCheck_End())
                            {
                                switch (sp_ValueModeIndex.intValue)
                                {
                                    case 0:
                                        sp_TweenValueMode.enumValueIndex = (int)TweenValueMode.起始到默认_S_D;
                                        break;
                                    case 1:
                                        sp_TweenValueMode.enumValueIndex = (int)TweenValueMode.默认到结束_D_E;
                                        break;
                                    case 2:
                                        sp_TweenValueMode.enumValueIndex = (int)TweenValueMode.起始到结束_S_E;
                                        break;
                                    case 3:
                                        sp_TweenValueMode.enumValueIndex = (int)TweenValueMode.当前到结束_C_E;
                                        break;
                                }
                                sp_TweenValueMode.serializedObject.ApplyModifiedProperties();
                                // 再次收集动画列表所有动画时机名称
                                CollectPreviewTimings(BaseScript);
                            }
                            #endregion

                            XGUI.gui_box(new Rect(rect.x + 5, baseheight + 175, rect.width - 10, 1), Color.gray * 0.85f);

                            #region 重置时设为起始值
                            rect_panel_title.Set(rect.x + 5, baseheight + 185, rect.width - 15, XGUI.GetSingleLineHeight());
                            XGUI.gui_toggle(
                                rect: rect_panel_title,
                                title: "动画 - 重置时 - 设为起始值",
                                title_color: Color.white,
                                title_size: XGUIFontSize.M,
                                title_font_style: FontStyle.Normal,
                                title_padding: new RectOffset(0, 0, 0, 0),
                                title_width: 110,
                                tog_interval: 10,
                                prop: sp_Rewind_Set_Startvalue,
                                tog_style: XGUIToggleStyle.实体,
                                tog_mixed_options: new string[] { "关闭", "开启" },
                                tog_mixed_text_size: XGUIFontSize.M,
                                tog_mixed_text_color: Color.black,
                                tog_mixed_text_padding: new RectOffset(0, 0, 0, 0),
                                tog_mixed_text_anchor: TextAnchor.MiddleCenter,
                                tog_mixed_text_font_style: FontStyle.Normal,
                                tog_bg_off_color: Color.gray,
                                tog_bg_on_color: XHud_Dashboard.Theme_Primary,
                                tog_handler_off_color: Color.white,
                                tog_handler_on_color: Color.white,
                                tog_mixed_bg_color_gui: XHud_Dashboard.Theme_Primary);
                            #endregion

                            #region 完成时设为结束值
                            rect_panel_title.Set(rect.x + 5, baseheight + 210, rect.width - 15, XGUI.GetSingleLineHeight());
                            XGUI.gui_toggle(
                                rect: rect_panel_title,
                                title: "动画 - 完成时 - 设为结束值",
                                title_color: Color.white,
                                title_size: XGUIFontSize.M,
                                title_font_style: FontStyle.Normal,
                                title_padding: new RectOffset(0, 0, 0, 0),
                                title_width: 110,
                                tog_interval: 10,
                                prop: sp_Complete_Set_Endvalue,
                                tog_style: XGUIToggleStyle.实体,
                                tog_mixed_options: new string[] { "关闭", "开启" },
                                tog_mixed_text_size: XGUIFontSize.M,
                                tog_mixed_text_color: Color.black,
                                tog_mixed_text_padding: new RectOffset(0, 0, 0, 0),
                                tog_mixed_text_anchor: TextAnchor.MiddleCenter,
                                tog_mixed_text_font_style: FontStyle.Normal,
                                tog_bg_off_color: Color.gray,
                                tog_bg_on_color: XHud_Dashboard.Theme_Primary,
                                tog_handler_off_color: Color.white,
                                tog_handler_on_color: Color.white,
                                tog_mixed_bg_color_gui: XHud_Dashboard.Theme_Primary);
                            #endregion

                            XGUI.gui_box(new Rect(rect.x + 5, baseheight + 240, rect.width - 10, 1), Color.gray * 0.85f);

                            Rect rect_valuepanel = new Rect(rect.x + 5, baseheight + 255, rect.width - 10, 60);

                            TweenValueProperties properties = ResolveValueProperties(nodetype, sp_node);
                            int mode_index = sp_ValueModeIndex.intValue;

                            if (sp_ValueModeIndex.intValue != 3)
                                DrawValueConnector(rect_valuepanel, new Vector2(0, 0), XHud_Dashboard.Theme_Primary, properties, mode_index);

                            DrawValueFieldsPanel(rect_valuepanel, properties, mode_index, nodetype);
                        }
                        else
                        {
                            #region 文字变色不支持提示
                            draw_rect.Set(rect.x, baseheight, rect.width - 20, 35);
                            XGUI.gui_label(
                                rect: draw_rect,
                                text: new GUIContent("当前文字颜色属性使用  \"库样式同步\" 状态时不支持颜色动画"),
                                text_color: Color.white * 0.85f,
                                size: XGUIFontSize.S,
                                wrap: true,
                                clipping: TextClipping.Clip,
                                anchor: TextAnchor.UpperCenter,
                                offset: new Vector2(0, 0),
                                font_style: FontStyle.Normal);
                            #endregion
                        }
                    }

                    #region 处理右键菜单
                    // 获取当前元素所在的矩形区域（整个元素的范围）
                    Rect elementRect = new Rect(rect.x, rect.y, rect.width, rect.height);

                    // 检查鼠标是否在当前元素区域内
                    if (Event.current.type == EventType.ContextClick && elementRect.Contains(Event.current.mousePosition))
                    {
                        GenericMenu menu = new GenericMenu();
                        menu.AddItem(new GUIContent("C 拷贝节点"), false, () =>
                        {
                            TweenNode node = BaseScript.PrimitiveTweenNodes[index];
                            string json = JsonUtility.ToJson(node);

                            XGUI.x_Editor_Data_Set_With_String("xData_Copied_TweenNode", json);
                        });
                        menu.AddSeparator("");
                        menu.AddItem(new GUIContent("V 粘贴节点"), false, () =>
                        {
                            TweenNode node = new TweenNode();
                            node = JsonUtility.FromJson<TweenNode>(XGUI.x_Editor_Data_Get_With_String("xData_Copied_TweenNode"));

                            if (BaseScript.TweenNode_IsDuplicated(node))
                            {
                                EditorApplication.delayCall += () =>
                                {
                                    string res = XGUI.dialog(
                                        type: XGUIDialogType.警告,
                                        windowtitle: "XHud - 图元动画器消息",
                                        title: "节点已存在",
                                        msg: $"发现存在重复的动画节点！请您检查后再做决定！",
                                        ok: "追加 ",
                                        cancel: "覆盖 ",
                                        alt: "跳过",
                                        PrimaryIndex: 0,
                                        usemodal: true,
                                        themecolor: XHud_Dashboard.Theme_Primary);

                                    if (res == "追加")
                                    {
                                        node.Indicator += "Copied";
                                        node.ID = BaseScript.TweenNode_GenerateId();
                                        BaseScript.PrimitiveTweenNodes.Add(node);
                                    }
                                    if (res == "覆盖")
                                    {
                                        TweenNode r_node = BaseScript.TweenNode_GetDuplicate(node);
                                        r_node.CopyFrom(node, true);
                                    }
                                };
                            }
                            else
                            {
                                node.Indicator += "Copied";
                                node.ID = BaseScript.TweenNode_GenerateId();
                                BaseScript.PrimitiveTweenNodes.Add(node);
                            }
                        });
                        menu.ShowAsContext();

                        // 使用事件，防止传递给其他控件
                        Event.current.Use();
                    }
                    #endregion

                    sp_node.serializedObject.ApplyModifiedProperties();

                    sp_PrimitiveTweenNodes.serializedObject.ApplyModifiedProperties();
                },
                onAddCallback = (ReorderableList list) =>
                {
                    sp_PrimitiveTweenNodes.InsertArrayElementAtIndex(list.count);

                    SerializedProperty sp_Root = sp_PrimitiveTweenNodes.GetArrayElementAtIndex(list.count - 1);

                    sp_Root.FindPropertyRelative("Indicator").stringValue = "NewTween";
                    sp_Root.FindPropertyRelative("ID").intValue = BaseScript.TweenNode_GenerateId();
                    sp_Root.FindPropertyRelative("Type").enumValueIndex = 0;
                    sp_Root.FindPropertyRelative("Enabled").boolValue = true;
                    sp_Root.FindPropertyRelative("Timings").stringValue = "元素进入时";
                    sp_Root.FindPropertyRelative("Duration").floatValue = 1;
                    sp_Root.FindPropertyRelative("Ease").enumValueIndex = 10;
                    sp_Root.FindPropertyRelative("Rewind_Set_Startvalue").boolValue = true;
                    sp_Root.FindPropertyRelative("Complete_Set_Endvalue").boolValue = true;
                    sp_Root.FindPropertyRelative("Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
                    sp_Root.FindPropertyRelative("Original_Color").colorValue = Color.white;
                    sp_Root.FindPropertyRelative("From_Color").colorValue = Color.white;
                    sp_Root.FindPropertyRelative("End_Color").colorValue = Color.white;
                    sp_Root.FindPropertyRelative("IsFold").boolValue = false;
                    sp_Root.FindPropertyRelative("TweenSounds").ClearArray();
                    sp_Root.FindPropertyRelative("LoopType").enumValueIndex = (int)XTween_LoopType.Restart;
                    sp_Root.FindPropertyRelative("LoopCount").intValue = 0;
                    sp_Root.FindPropertyRelative("Progress").floatValue = 0;
                    sp_Root.FindPropertyRelative("ValueModeIndex").intValue = 0;
                    sp_Root.FindPropertyRelative("TweenValueMode").enumValueIndex = (int)TweenValueMode.起始到默认_S_D;
                    sp_Root.serializedObject.ApplyModifiedProperties();

                    EditorApplication.delayCall += () =>
                    {
                        // 刷新获取动画列表所有动画时机名称
                        CollectPreviewTimings(BaseScript);
                    };
                },
                onRemoveCallback = (ReorderableList list) =>
                {
                    sp_PrimitiveTweenNodes.DeleteArrayElementAtIndex(list.index);
                    sp_PrimitiveTweenNodes.serializedObject.ApplyModifiedProperties();

                    EditorApplication.delayCall += () =>
                    {
                        // 刷新获取动画列表所有动画时机名称
                        CollectPreviewTimings(BaseScript);
                    };
                },
                elementHeightCallback = index =>
                {
                    SerializedProperty sp_Root = sp_PrimitiveTweenNodes.GetArrayElementAtIndex(index);
                    SerializedProperty sp_IsFold = sp_Root.FindPropertyRelative("IsFold");
                    SerializedProperty sp_ValueModeIndex = sp_Root.FindPropertyRelative("ValueModeIndex");

                    float height = 20;

                    if (sp_IsFold.boolValue)
                        height = 1.6f;
                    else
                    {
                        if (sp_ValueModeIndex.intValue == 3)
                            height = 19;
                    }

                    if (XGUI.GetCurrentWindowWidth() < 145)
                    {
                        height = 1.6f;
                    }

                    return height * XGUI.GetSingleLineHeight();
                }
            };
            #endregion

            // 收集动画列表所有动画时机名称
            CollectPreviewTimings(BaseScript);
        }
        /// <summary>
        /// Unity 禁用回调
        /// <para/>
        /// 编辑器非播放模式下停止所有预览动画与音效，并关闭子窗口。
        /// </summary>
        private void OnDisable()
        {
            if (!Application.isPlaying)
            {
                StopPreview();
                StopAllPreviewSounds();
            }

            Editor_XHud_Tool_SceneView_Activate_Mark.SetEnabled(false);
        }
        /// <summary>
        /// 绘制 Inspector 主入口
        /// <para/>
        /// 按自上而下的顺序绘制所有面板：标题栏 → 快捷功能 → 动画参数 → 动画节点列表 → 选项 → 状态 → 源脚本。
        /// 右键菜单在最后统一处理。
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            string h_color = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary);

            #region 标题
            XGUI.layout_banner(
                bg_fill: XGUIFilled.实体,
                bg_color: XGUIColor.深空灰,
                bg_height: 30,
                icon: icon_main,
                icon_color: XHud_Dashboard.Theme_Primary,
                title_text: "XHud  -  图元  >  动画",
                title_anchor: TextAnchor.MiddleLeft,
                title_style: FontStyle.Normal,
                title_color: Color.white,
                title_size: XGUIFontSize.B,
                title_clipping: XGUI.TryEllipsisClipping(),
                bg_margin: new RectOffset(0, 0, 5, 5));
            #endregion

            Rect rect = GUILayoutUtility.GetLastRect();

            #region 快捷功能
            XGUI.layout_group_start(
              type: XGUIContainerType.Vertical,
              bg_fill: XGUIFilled.缺口纯色边框,
              bg_color: XGUIColor.亮白,
              bg_color_gui: XHud_Dashboard.Theme_Group,
              title: "快捷功能",
              title_size: XGUIFontSize.M,
              title_text_color: XHud_Dashboard.Theme_Primary,
              title_clipping: TextClipping.Clip,
              padding: new RectOffset(15, 15, 20, 15));

            #region 快捷按钮
            XGUI.layout_group_start(
               type: XGUIContainerType.Horizontal,
               absolute_margin: true,
               absolute_padding: true,
               margin: new RectOffset(0, 0, 0, 0),
               padding: new RectOffset(0, 0, 0, 0));

            #region 预览按钮
            if (!sp_TweenIsPreviewing.boolValue)
            {
                #region 预览
                if (XGUI.layout_button(
                    tooltip: "预览",
                    tex_release: prw_play_r,
                    tex_press: prw_play_p,
                    tex_gui_color: Color.white,
                    border: new RectOffset(0, 0, 0, 0),
                    width: 14,
                    height: 14))
                {
                    PlayPreview();
                }
                #endregion
            }
            else
            {
                #region 停止
                if (XGUI.layout_button(
                    tooltip: "停止",
                    tex_release: prw_stop_r,
                    tex_press: prw_stop_p,
                    tex_gui_color: Color.white,
                    border: new RectOffset(0, 0, 0, 0),
                    width: 14,
                    height: 14))
                {
                    StopPreview();
                }
                #endregion
            }
            #endregion

            XGUI.layout_flexspace();

            #region 打开时间线轨道编辑器
            if (!IsMultiSelection())
            {
                if (XGUI.layout_button(
                tooltip: "打开时间线轨道编辑器",
                tex_release: opentrack_r,
                tex_press: opentrack_p,
                tex_gui_color: Color.white,
                border: new RectOffset(0, 0, 0, 0),
                width: 14,
                height: 14))
                {
                    Editor_XHud_Module_Primitive_Tween_Tracker.OpenWith(BaseScript);
                }
            }
            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            #endregion

            XGUI.layout_seperator(
                thickness: 1,
                color: XHud_Dashboard.Theme_SeperateLine,
                margin: new RectOffset(15, 15, 15, 15));

            #region 预览时机
            if (!IsMultiSelection())
            {
                Rect last = XGUI.GetLastRect();
                Rect timRefresh_Rect = new Rect(rect.width - 140, last.y - 10, 150, 38);

                // 点击预览时机下拉菜单时先更新一下
                Event e = Event.current;
                if (e.type == EventType.MouseDown && e.button == (int)MouseButton.Left && timRefresh_Rect.Contains(e.mousePosition))
                {
                    // 再次收集动画列表所有动画时机名称
                    CollectPreviewTimings(BaseScript);
                }

                sp_PreviewTiming.stringValue = XGUI.layout_string_popup(
                    title: "预览时机",
                    title_width: 100,
                    title_size: XGUIFontSize.M,
                    title_anchor: TextAnchor.MiddleLeft,
                    prop: sp_PreviewTiming,
                    options: opt_preview_timings,
                    opt_text_size: XGUIFontSize.M,
                    opt_text_color: Color.black,
                    opt_text_padding: new RectOffset(10, 10, 0, 0),
                    opt_anchor: TextAnchor.MiddleLeft,
                    opt_font_style: FontStyle.Normal,
                    opt_bg_fill: XGUIFilled.实体,
                    opt_bg_color: XGUIColor.亮白,
                    opt_bg_color_gui: XHud_Dashboard.Theme_Primary,
                    margin: new RectOffset(0, 0, 5, 5),
                    padding: new RectOffset(0, 0, 0, 0),
                    title_margin: new RectOffset(0, 0, 0, 0),
                    icon_arrow_color: Color.black);

                sp_PreviewTiming.serializedObject.ApplyModifiedProperties();
            }
            else
            {
                XGUI.layout_label(
                    text: "暂不支持批量预览",
                    size: XGUIFontSize.S,
                    anchor: TextAnchor.MiddleCenter,
                    text_color: Color.white * 0.85f,
                    offset: new Vector2(0, 0),
                    padding: new RectOffset(0, 0, 0, 0),
                    margin: new RectOffset(0, 0, 6, 0),
                    clipping: TextClipping.Clip);
            }
            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 动画参数
            BaseScript.fold_param = XGUI.layout_group_start(
                type: XGUIContainerType.Horizontal,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.Theme_Group,
                title: "参数",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.Theme_Primary,
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 20, 15),
                foldout: BaseScript.fold_param);

            if (!BaseScript.fold_param)
            {
                #region 速率倍增
                XGUI.layout_property_field(
                    title: "速率倍增",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: sp_GlobalDuration,
                    prop_margin: new RectOffset(5, 5, 0, 5));
                #endregion
            }

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            #endregion

            #region 动画节点列表
            BaseScript.fold_list = XGUI.layout_group_start(
                type: XGUIContainerType.Horizontal,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.Theme_Group,
                title: "动画节点列表",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.Theme_Primary,
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 20, 15),
                foldout: BaseScript.fold_list);

            if (!BaseScript.fold_list)
            {
                if (!IsMultiSelection())
                {
                    AnimateTweenNodesList.DoLayoutList();
                    sp_PrimitiveTweenNodes.serializedObject.ApplyModifiedProperties();
                }
                else
                {
                    XGUI.layout_label(
                        text: "动画节点不支持多项操作",
                        size: XGUIFontSize.S,
                        anchor: TextAnchor.MiddleCenter,
                        text_color: Color.white * 0.85f,
                        offset: new Vector2(0, 0),
                        padding: new RectOffset(0, 0, 0, 0),
                        margin: new RectOffset(0, 0, 6, 0),
                        clipping: TextClipping.Clip);
                }
            }

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            #endregion

            #region 选项
            BaseScript.fold_option = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.Theme_Group,
                title: "选项",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.Theme_Primary,
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: BaseScript.fold_option);

            if (!BaseScript.fold_option)
            {
                #region 状态调试
                DrawLabeledToggle("状态调试", "", sp_UseDebug, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, opt_debug, (b) => { });
                #endregion

                #region 静音
                DrawLabeledToggle("静音", "开启后再动画播放时所有音效均不播放", sp_MutePlay, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, opt_mute, (b) => { });
                #endregion

                #region 元素联动
                DrawLabeledToggle("元素联动", "如果关闭则元素入场和出场动画时则不会自动调用该动画器的动画播放！", sp_IgnoreElementAnimationPlay, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, opt_control, (b) => { });
                #endregion
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 状态
            string statu_title = "状态";
            if (IsMultiSelection())
                statu_title = "状态 - ( 批量模式 )";

            RefreshTimerStatistics();

            BaseScript.fold_state = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.Theme_Group,
                title: statu_title,
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.Theme_Primary,
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: BaseScript.fold_state);

            if (!BaseScript.fold_state)
            {
                if (!IsMultiSelection())
                {
                    if (sp_PrimitiveTweenNodes.arraySize <= 0)
                    {
                        XGUI.layout_label(
                            text: "暂无统计数据",
                            size: XGUIFontSize.S,
                            anchor: TextAnchor.MiddleCenter,
                            text_color: Color.white * 0.85f,
                            offset: new Vector2(0, 0),
                            padding: new RectOffset(0, 0, 0, 0),
                            margin: new RectOffset(0, 0, 6, 0),
                            clipping: TextClipping.Clip);
                    }
                    else
                    {
                        #region 动画状态
                        XGUI.layout_state_displayer_text(
                            title: "动画状态",
                            title_size: XGUIFontSize.M,
                            subtitle: sp_TweenIsPreviewing.boolValue ? "运动" : "静止",
                            subtitle_size: XGUIFontSize.M,
                            subtitle_color: XHud_Dashboard.Theme_Primary,
                            margin: new RectOffset(5, 5, 0, 5));
                        #endregion

                        #region 动画节点
                        XGUI.layout_state_displayer_text(
                           title: "动画节点",
                           title_size: XGUIFontSize.M,
                           subtitle: sp_PrimitiveTweenNodes.arraySize.ToString() + " 个",
                           subtitle_size: XGUIFontSize.M,
                           subtitle_color: XHud_Dashboard.Theme_Primary,
                           margin: new RectOffset(5, 5, 0, 5));
                        #endregion

                        #region 最小耗时
                        XGUI.layout_state_displayer_text(
                            title: "最小耗时",
                            title_size: XGUIFontSize.M,
                            subtitle: sp_MinTimer.floatValue.ToString() + " 秒",
                            subtitle_size: XGUIFontSize.M,
                            subtitle_color: XHud_Dashboard.Theme_Primary,
                            margin: new RectOffset(5, 5, 0, 5));
                        #endregion

                        #region 最大耗时
                        XGUI.layout_state_displayer_text(
                           title: "最大耗时",
                           title_size: XGUIFontSize.M,
                           subtitle: sp_MaxTimer.floatValue.ToString() + " 秒",
                           subtitle_size: XGUIFontSize.M,
                           subtitle_color: XHud_Dashboard.Theme_Primary,
                           margin: new RectOffset(5, 5, 0, 5));
                        #endregion

                        #region 最大耗时（图元动画器倍增）
                        XGUI.layout_state_displayer_text(
                           title: "最大耗时<color=#909090>（图元动画器倍增）</color>",
                           title_size: XGUIFontSize.M,
                           subtitle: sp_MinTimerWithGlobalDuration.floatValue.ToString() + " 秒",
                           subtitle_size: XGUIFontSize.M,
                           subtitle_color: XHud_Dashboard.Theme_Primary,
                           margin: new RectOffset(5, 5, 0, 5));
                        #endregion

                        #region 最大耗时（XHUD倍增）
                        XGUI.layout_state_displayer_text(
                           title: "最大耗时<color=#909090>（XHUD倍增）</color>",
                           title_size: XGUIFontSize.M,
                           subtitle: (sp_MaxTimerWithGlobalDuration.floatValue * HudManager.DurationMultiply).ToString() + " 秒",
                           subtitle_size: XGUIFontSize.M,
                           subtitle_color: XHud_Dashboard.Theme_Primary,
                           margin: new RectOffset(5, 5, 0, 5));
                        #endregion
                    }
                }
                else
                {
                    #region 控制区
                    XGUI.layout_group_start(
                        type: XGUIContainerType.Horizontal,
                        bg_fill: XGUIFilled.透明,
                        bg_color: XGUIColor.亮白,
                        bg_color_gui: XHud_Dashboard.Theme_Group,
                        absolute_margin: true,
                        absolute_padding: true,
                        margin: new RectOffset(0, 0, 0, 0),
                        padding: new RectOffset(15, 15, 5, 10));

                    #region 标题按钮
                    if (XGUI.layout_button(
                        text: $"{SelectedObjects[MultiPrimitiveTween_Index].name}",
                        tooltip: "",
                        bg_fill: XGUIFilled.透明,
                        button_text_color: Color.gray,
                        press_fill: XGUIFilled.透明,
                        press_text_color: XHud_Dashboard.Theme_Primary,
                        font_size: XGUIFontSize.M,
                        anchor: TextAnchor.MiddleLeft,
                        margin: new RectOffset(0, 0, 0, 0),
                        padding: new RectOffset(0, 0, 0, 0),
                        layout_width: 150,
                        button_text_font: XGUI.GetFont("xg-medium")))
                    {
                        EditorGUIUtility.PingObject(SelectedObjects[MultiPrimitiveTween_Index]);
                    }
                    #endregion

                    XGUI.layout_flexspace();

                    #region 上一个
                    if (XGUI.layout_button(
                        tooltip: "上一个",
                        tex_release: left_arrow_r,
                        tex_press: left_arrow_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: 12,
                        height: 12))
                    {
                        if (MultiPrimitiveTween_Index <= 0)
                        {
                            MultiPrimitiveTween_Index = SelectedObjects.Length - 1;
                        }
                        else
                        {
                            MultiPrimitiveTween_Index--;
                        }
                        EditorGUIUtility.PingObject(SelectedObjects[MultiPrimitiveTween_Index]);
                    }
                    #endregion

                    XGUI.layout_space(20);

                    #region 下一个
                    if (XGUI.layout_button(
                        tooltip: "下一个",
                        tex_release: right_arrow_r,
                        tex_press: right_arrow_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: 12,
                        height: 12))
                    {
                        if (MultiPrimitiveTween_Index >= SelectedObjects.Length - 1)
                        {
                            MultiPrimitiveTween_Index = 0;
                        }
                        else
                        {
                            MultiPrimitiveTween_Index++;
                        }
                        EditorGUIUtility.PingObject(SelectedObjects[MultiPrimitiveTween_Index]);
                    }
                    #endregion

                    XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
                    #endregion

                    XGUI.layout_seperator(
                            thickness: 1,
                            color: XHud_Dashboard.Theme_SeperateLine,
                            margin: new RectOffset(15, 15, 5, 25));

                    if (SelectedObjects[MultiPrimitiveTween_Index].PrimitiveTweenNodes.Count <= 0)
                    {
                        XGUI.layout_label(
                          text: "暂无统计数据",
                          size: XGUIFontSize.S,
                          anchor: TextAnchor.MiddleCenter,
                          text_color: Color.white * 0.85f,
                          offset: new Vector2(0, 0),
                          padding: new RectOffset(0, 0, 0, 0),
                          margin: new RectOffset(0, 0, 6, 0),
                          clipping: TextClipping.Clip);
                    }
                    else
                    {
                        #region 动画状态
                        XGUI.layout_state_displayer_text(
                            title: "动画状态",
                            title_size: XGUIFontSize.M,
                            subtitle: SelectedObjects[MultiPrimitiveTween_Index].TweenIsPreviewing ? "运动" : "静止",
                            subtitle_size: XGUIFontSize.M,
                            subtitle_color: XHud_Dashboard.Theme_Primary,
                            margin: new RectOffset(5, 5, 0, 5));
                        #endregion

                        #region 动画节点
                        XGUI.layout_state_displayer_text(
                           title: "动画节点",
                           title_size: XGUIFontSize.M,
                           subtitle: SelectedObjects[MultiPrimitiveTween_Index].PrimitiveTweenNodes.Count.ToString() + " 个",
                           subtitle_size: XGUIFontSize.M,
                           subtitle_color: XHud_Dashboard.Theme_Primary,
                           margin: new RectOffset(5, 5, 0, 5));
                        #endregion

                        #region 最小耗时
                        XGUI.layout_state_displayer_text(
                            title: "最小耗时",
                            title_size: XGUIFontSize.M,
                            subtitle: SelectedObjects[MultiPrimitiveTween_Index].MinTimer.ToString() + " 秒",
                            subtitle_size: XGUIFontSize.M,
                            subtitle_color: XHud_Dashboard.Theme_Primary,
                            margin: new RectOffset(5, 5, 0, 5));
                        #endregion

                        #region 最大耗时
                        XGUI.layout_state_displayer_text(
                           title: "最大耗时",
                           title_size: XGUIFontSize.M,
                           subtitle: SelectedObjects[MultiPrimitiveTween_Index].MaxTimer.ToString() + " 秒",
                           subtitle_size: XGUIFontSize.M,
                           subtitle_color: XHud_Dashboard.Theme_Primary,
                           margin: new RectOffset(5, 5, 0, 5));
                        #endregion

                        #region 最大耗时（图元动画器倍增）
                        XGUI.layout_state_displayer_text(
                           title: "最大耗时<color=#909090>（图元动画器倍增）</color>",
                           title_size: XGUIFontSize.M,
                           subtitle: SelectedObjects[MultiPrimitiveTween_Index].MinTimerWithGlobalDuration.ToString() + " 秒",
                           subtitle_size: XGUIFontSize.M,
                           subtitle_color: XHud_Dashboard.Theme_Primary,
                           margin: new RectOffset(5, 5, 0, 5));
                        #endregion

                        #region 最大耗时（XHUD倍增）
                        XGUI.layout_state_displayer_text(
                           title: "最大耗时<color=#909090>（XHUD倍增）</color>",
                           title_size: XGUIFontSize.M,
                           subtitle: (SelectedObjects[MultiPrimitiveTween_Index].MaxTimerWithGlobalDuration * HudManager.DurationMultiply).ToString() + " 秒",
                           subtitle_size: XGUIFontSize.M,
                           subtitle_color: XHud_Dashboard.Theme_Primary,
                           margin: new RectOffset(5, 5, 0, 5));
                        #endregion
                    }
                }
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 右键菜单
            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                // 创建右键菜单
                GenericMenu menu = new GenericMenu();
                if (!sp_TweenIsPreviewing.boolValue)
                {
                    menu.AddItem(new GUIContent("S (预览动画)"), false, () =>
                    {
                        PlayPreview();
                    });
                }
                else
                {
                    menu.AddItem(new GUIContent("S (停止预览)"), false, () =>
                    {
                        StopPreview();
                    });
                }
                menu.AddSeparator("");
                if (!IsMultiSelection())
                {
                    menu.AddItem(new GUIContent("A (拷贝动画列表)"), false, () =>
                    {
                        TweenNodeArray tnc = new TweenNodeArray();
                        tnc.TweenNodeList = new List<TweenNode>();

                        for (int i = 0; i < BaseScript.PrimitiveTweenNodes.Count; i++)
                        {
                            tnc.TweenNodeList.Add(BaseScript.PrimitiveTweenNodes[i]);
                        }

                        string json = JsonUtility.ToJson(tnc);

                        XGUI.x_Editor_Data_Set_With_String("xData_PrimitiveTween_Copied_TweenNodes", json);
                        string indicator = $"( {BaseScript.controller.Indicator} )";

                        XGUI.dialog(
                            type: XGUIDialogType.确认,
                            windowtitle: "XHud - 图元动画器消息",
                            title: "动画节点数据",
                            msg: $"已拷贝 {BaseScript.name}{(string.IsNullOrEmpty(indicator) ? "" : indicator)} 动画器的动画节点数据！",
                            ok: "明白 ",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.Theme_Primary);
                    });
                }
                menu.AddItem(new GUIContent("C (粘贴动画列表)"), false, () =>
                {
                    string json = XGUI.x_Editor_Data_Get_With_String("xData_PrimitiveTween_Copied_TweenNodes");
                    TweenNodeArray tnc = JsonUtility.FromJson<TweenNodeArray>(json);

                    if (IsMultiSelection())
                    {
                        #region 询问
                        List<XGUIDialogListDatas> Datas = new List<XGUIDialogListDatas>();
                        for (int i = 0; i < SelectedObjects.Length; i++)
                        {
                            XGUIDialogListDatas dataitem = new XGUIDialogListDatas();
                            dataitem.Title = $"动画节点数据";
                            dataitem.SubTitle = "即将粘贴到";
                            string indicator = $"( {SelectedObjects[i].controller.Indicator} )";
                            dataitem.Message = $"{SelectedObjects[i].name}{(string.IsNullOrEmpty(indicator) ? "" : indicator)}";
                            Datas.Add(dataitem);
                        }

                        string res_mul = XGUI.dialog_listview(
                            datas: Datas.ToArray(),
                            type: XGUIDialogType.警告,
                            windowtitle: "XHud - 图元动画器消息",
                            title: "批量粘贴动画节点数据",
                            msg: "确认要将  (xData) 中的动画节点数据粘贴到列表中的图元动画器中吗？",
                            ok: "粘贴",
                            cancel: "暂不",
                            PrimaryIndex: 0,
                            show_index: false,
                            usemodal: false,
                            themecolor: XHud_Dashboard.Theme_Primary);

                        if (res_mul == "暂不")
                            return;
                        #endregion

                        #region 批量粘贴动画节点数据
                        Datas.Clear();
                        for (int s = 0; s < SelectedObjects.Length; s++)
                        {
                            List<TweenNode> tweenNodes = new List<TweenNode>();
                            for (int c = 0; c < tnc.TweenNodeList.Count; c++)
                            {
                                tweenNodes.Add(tnc.TweenNodeList[c].Clone());
                            }

                            SelectedObjects[s].PrimitiveTweenNodes = tweenNodes;

                            XGUIDialogListDatas dataitem = new XGUIDialogListDatas();
                            dataitem.Title = "动画节点数据";
                            dataitem.SubTitle = "已粘贴到动画器";
                            string indicator = $"( {SelectedObjects[s].controller.Indicator} )";
                            dataitem.Message = $"{SelectedObjects[s].name}{(string.IsNullOrEmpty(indicator) ? "" : indicator)}";

                            Datas.Add(dataitem);
                        }

                        XGUI.dialog_listview(
                            datas: Datas.ToArray(),
                            type: XGUIDialogType.确认,
                            windowtitle: "XHud - 图元动画器消息",
                            title: "批量粘贴动画节点数据",
                            msg: "以下是已粘贴动画节点数据的动画器列表，请您检查核对：",
                            ok: "明白",
                            PrimaryIndex: 0,
                            show_index: false,
                            usemodal: false,
                            themecolor: XHud_Dashboard.Theme_Primary);
                        #endregion
                    }
                    else
                    {
                        #region 询问
                        string indicator = $"( {BaseScript.controller.Indicator} )";

                        string res = XGUI.dialog(
                           type: XGUIDialogType.警告,
                           windowtitle: "XHud - 图元动画器消息",
                           title: "粘贴动画节点",
                           msg: $"确认要将 (xData) 中的动画节点数据粘贴到  {BaseScript.name}{(string.IsNullOrEmpty(indicator) ? "" : indicator)} 动画器中吗？",
                           ok: "粘贴 ",
                           cancel: "暂不 ",
                           PrimaryIndex: 0,
                           usemodal: true,
                           themecolor: XHud_Dashboard.Theme_Primary);

                        if (res == "暂不")
                            return;
                        #endregion

                        #region 粘贴动画节点数据
                        List<TweenNode> tweenNodes = new List<TweenNode>();
                        for (int s = 0; s < tnc.TweenNodeList.Count; s++)
                        {
                            tweenNodes.Add(tnc.TweenNodeList[s].Clone());
                        }
                        BaseScript.PrimitiveTweenNodes = tweenNodes;

                        XGUI.dialog(
                           type: XGUIDialogType.确认,
                           windowtitle: "XHud - 图元动画器消息",
                           title: "粘贴动画节点",
                           msg: $"已将动画节点粘贴到： {BaseScript.name} {BaseScript.name}{(string.IsNullOrEmpty(indicator) ? "" : indicator)}",
                           ok: "明白 ",
                           PrimaryIndex: 0,
                           usemodal: true,
                           themecolor: XHud_Dashboard.Theme_Primary);
                        #endregion
                    }
                });
                menu.AddItem(new GUIContent("E (清空动画列表)"), false, () =>
                {
                    string res = XGUI.dialog(
                           type: XGUIDialogType.警告,
                           windowtitle: "XHud - 图元动画器消息",
                           title: "清空动画效果列表",
                           msg: $"快速清空动画效果列表此操作不可逆，是否需要清空？清空后您为此动画器做的动画效果参数将全部丢失，请谨慎此操作！",
                           ok: "清空 ",
                           cancel: "暂不 ",
                           PrimaryIndex: 0,
                           usemodal: true,
                           themecolor: XHud_Dashboard.Theme_Primary);

                    if (res == "暂不")
                    {
                        return;
                    }

                    if (IsMultiSelection())
                    {
                        List<XGUIDialogListDatas> Datas = new List<XGUIDialogListDatas>();
                        for (int s = 0; s < SelectedObjects.Length; s++)
                        {
                            Undo.RecordObject(SelectedObjects[s], "PasteAnimationTweenNodes");
                            for (int i = 1; i < SelectedObjects.Length; i++)
                            {
                                SelectedObjects[s].PrimitiveTweenNodes.Clear();

                                XGUIDialogListDatas dataitem = new XGUIDialogListDatas();
                                dataitem.Title = $"图元动画器";
                                string indicator = $"( {SelectedObjects[s].controller.Indicator} )";
                                dataitem.SubTitle = $"{SelectedObjects[s].name}{(string.IsNullOrEmpty(indicator) ? "" : indicator)}";
                                dataitem.Message = $"动画效果已清空";

                                Datas.Add(dataitem);
                            }
                        }

                        XGUI.dialog_listview(
                           datas: Datas.ToArray(),
                           type: XGUIDialogType.确认,
                           windowtitle: "XHud - 图元动画器消息",
                           title: "批量清空动画效果列表",
                           msg: "以下是已清空动画效果列表的图元动画器列表，请您检查核对：",
                           ok: "明白",
                           PrimaryIndex: 0,
                           show_index: false,
                           usemodal: false,
                           themecolor: XHud_Dashboard.Theme_Primary);
                    }
                    else
                    {
                        BaseScript.PrimitiveTweenNodes.Clear();

                        string indicator = $"( {BaseScript.controller.Indicator} )";
                        XGUI.dialog(
                           type: XGUIDialogType.确认,
                           windowtitle: "XHud - 图元动画器消息",
                           title: "清空图元动画效果列表",
                           msg: $"已将  {BaseScript.name}{(string.IsNullOrEmpty(indicator) ? "" : indicator)} 动画器的动画效果列表清空！",
                           ok: "明白 ",
                           PrimaryIndex: 0,
                           usemodal: true,
                           themecolor: XHud_Dashboard.Theme_Primary);
                    }
                });
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Q (折叠编组)"), false, () =>
                {
                    if (IsMultiSelection())
                    {
                        for (int i = 0; i < SelectedObjects.Length; i++)
                        {
                            SelectedObjects[i].GroupFold(true);
                        }
                    }
                    else
                    {
                        BaseScript.GroupFold(true);
                    }
                });
                menu.AddItem(new GUIContent("W (展开编组)"), false, () =>
                {
                    if (IsMultiSelection())
                    {
                        for (int i = 0; i < SelectedObjects.Length; i++)
                        {
                            SelectedObjects[i].GroupFold(false);
                        }
                    }
                    else
                    {
                        BaseScript.GroupFold(false);
                    }
                });
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("F (动画列表 - 全部折叠)"), false, () =>
                {
                    if (IsMultiSelection())
                    {
                        for (int i = 0; i < SelectedObjects.Length; i++)
                        {
                            for (int s = 1; s < SelectedObjects[i].PrimitiveTweenNodes.Count; s++)
                            {
                                SelectedObjects[i].PrimitiveTweenNodes[s].IsFold = true;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < BaseScript.PrimitiveTweenNodes.Count; i++)
                        {
                            BaseScript.PrimitiveTweenNodes[i].IsFold = true;
                        }
                    }
                });
                menu.AddItem(new GUIContent("D (动画列表 - 全部展开)"), false, () =>
                {
                    if (IsMultiSelection())
                    {
                        for (int i = 0; i < SelectedObjects.Length; i++)
                        {
                            for (int s = 1; s < SelectedObjects[i].PrimitiveTweenNodes.Count; s++)
                            {
                                SelectedObjects[i].PrimitiveTweenNodes[s].IsFold = false;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < BaseScript.PrimitiveTweenNodes.Count; i++)
                        {
                            BaseScript.PrimitiveTweenNodes[i].IsFold = false;
                        }
                    }
                });
                menu.ShowAsContext();
            }
            #endregion

            #region 源脚本
            BaseScript.fold_based = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.Theme_Group,
                title: "源脚本",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.Theme_Primary,
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: BaseScript.fold_based);

            if (!BaseScript.fold_based)
            {
                DrawDefaultInspector();
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            serializedObject.ApplyModifiedProperties();
        }
        #endregion

        #region 绘制：动画值控件
        /// <summary>
        /// 绘制数值面板的「流向连接器」按钮
        /// <para/>
        /// 点击后根据当前动向模式交换两个值的序列化属性。
        /// </summary>
        private void DrawValueConnector(Rect rect, Vector2 offset, Color color, TweenValueProperties properties, int mode)
        {
            if (XGUI.gui_button(
                rect: new Rect(rect.x - 20, rect.y + 4 + offset.y, dir_connector_r.width, 38),
                tooltip: "",
                tex_release: dir_connector_r,
                tex_press: dir_connector_p,
                tex_gui_color: color,
                border: new RectOffset(0, 0, 12, 13),
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0)))
            {
                SerializedProperty primary = null;
                SerializedProperty secondary = null;

                switch (mode)
                {
                    case 0:
                        primary = properties.prop_from;
                        secondary = properties.prop_origin;
                        break;
                    case 1:
                        primary = properties.prop_origin;
                        secondary = properties.prop_end;
                        break;
                    case 2:
                        primary = properties.prop_from;
                        secondary = properties.prop_end;
                        break;
                }

                SwapSerializedValues(primary, secondary);
            }
        }
        /// <summary>
        /// 绘制数值面板的所有数值字段（按动向模式决定显示哪些）
        /// </summary>
        private void DrawValueFieldsPanel(Rect rect, TweenValueProperties propties, int mode, TweenNodeType type)
        {
            switch (mode)
            {
                case 0:
                    DrawValueField(rect, propties.prop_from, mode, "起始 - S", new Vector2(0, 0), dot_color_red,
                        () =>
                        {
                            RecordValueFromHost(propties.prop_from, propties.type);
                        },
                        () =>
                        {
                            ApplyValueToHost(propties.prop_from, propties.type);
                        },
                        () =>
                        {
                            ResetValueToZero(propties.prop_from);
                        });
                    DrawValueField(rect, propties.prop_origin, mode, "默认 - D", new Vector2(0, 25), XHud_Dashboard.Theme_Primary,
                         () =>
                         {
                             RecordValueFromHost(propties.prop_origin, propties.type);
                         },
                        () =>
                        {
                            ApplyValueToHost(propties.prop_origin, propties.type);
                        },
                        () =>
                        {
                            ResetValueToZero(propties.prop_origin);
                        });
                    break;
                case 1:
                    DrawValueField(rect, propties.prop_origin, mode, "默认 - D", new Vector2(0, 0), XHud_Dashboard.Theme_Primary,
                         () =>
                         {
                             RecordValueFromHost(propties.prop_origin, propties.type);
                         },
                        () =>
                        {
                            ApplyValueToHost(propties.prop_origin, propties.type);
                        },
                        () =>
                        {
                            ResetValueToZero(propties.prop_origin);
                        });
                    DrawValueField(rect, propties.prop_end, mode, "结束 - E", new Vector2(0, 25), Color.white,
                     () =>
                     {
                         RecordValueFromHost(propties.prop_end, propties.type);
                     },
                        () =>
                        {
                            ApplyValueToHost(propties.prop_end, propties.type);
                        },
                        () =>
                        {
                            ResetValueToZero(propties.prop_end);
                        });
                    break;
                case 2:
                    DrawValueField(rect, propties.prop_from, mode, "起始 - S", new Vector2(0, 0), dot_color_red,
                         () =>
                         {
                             RecordValueFromHost(propties.prop_from, propties.type);
                         },
                        () =>
                        {
                            ApplyValueToHost(propties.prop_from, propties.type);
                        },
                        () =>
                        {
                            ResetValueToZero(propties.prop_from);
                        });
                    DrawValueField(rect, propties.prop_end, mode, "结束 - E", new Vector2(0, 25), Color.white,
                     () =>
                     {
                         RecordValueFromHost(propties.prop_end, propties.type);
                     },
                        () =>
                        {
                            ApplyValueToHost(propties.prop_end, propties.type);
                        },
                        () =>
                        {
                            ResetValueToZero(propties.prop_end);
                        });
                    break;
                case 3:
                    DrawValueField(rect, propties.prop_end, mode, "结束 - E", new Vector2(0, 0), Color.white,
                        () =>
                        {
                            RecordValueFromHost(propties.prop_end, propties.type);
                        },
                        () =>
                        {
                            ApplyValueToHost(propties.prop_end, propties.type);
                        },
                        () =>
                        {
                            ResetValueToZero(propties.prop_end);
                        });
                    break;
            }
        }
        /// <summary>
        /// 动画值标记按钮控件绘制
        /// <para/>
        /// 圆点按钮支持三种操作：
        /// <list type="bullet">
        /// <item><description>鼠标左键：从宿主组件读取当前值（记录）；</description></item>
        /// <item><description>鼠标右键：将当前值写回宿主组件（应用）；</description></item>
        /// <item><description>鼠标中键：将当前值归零（重置）。</description></item>
        /// </list>
        /// </summary>
        private void DrawValueField(Rect rect, SerializedProperty prop, int dir_index, string title, Vector2 offset, Color dot_color, Action act_on_pressed_record = null, Action act_on_pressed_apply = null, Action act_on_pressed_reset = null)
        {
            #region 数值输入控件
            XGUI.gui_property_field(
                    rect: new Rect(rect.x + offset.x + (dir_index == 3 ? 25 : 40), rect.y + offset.y, rect.width - (5 + offset.x) - (dir_index == 3 ? 25 : 40), XGUI.GetSingleLineHeight()),
                    title: new GUIContent(title),
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 40,
                    prop: prop);
            #endregion

            #region 圆点按钮控件（数值获取赋值的多元操作方式）
            if (XGUI.gui_button(
              rect: new Rect(rect.x + (dir_index == 3 ? -5 : 12), rect.y - 2 + offset.y, 24, 24),
              tooltip: "",
              tex_release: anim_dot_r,
              tex_press: anim_dot_p,
              tex_gui_color: dot_color,
              border: new RectOffset(0, 0, 0, 0),
              margin: new RectOffset(0, 0, 0, 0),
              padding: new RectOffset(0, 0, 0, 0)))
            {
                Event eve = Event.current;
                switch (eve.button)
                {
                    #region 鼠标左键：
                    case 0:
                        if (act_on_pressed_record != null)
                            act_on_pressed_record();
                        break;
                    #endregion
                    #region 鼠标右键：
                    case 1:
                        if (act_on_pressed_apply != null)
                            act_on_pressed_apply();
                        break;
                    #endregion
                    #region 鼠标中键：
                    case 2:
                        if (act_on_pressed_reset != null)
                            act_on_pressed_reset();
                        break;
                        #endregion
                }
            }
            #endregion
        }
        /// <summary>
        /// 根据动画类型解析出其对应的「起始 / 结束 / 默认」三组序列化属性
        /// </summary>
        private TweenValueProperties ResolveValueProperties(TweenNodeType type, SerializedProperty prop)
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
        /// 从宿主组件的当前属性读值并写入指定序列化属性
        /// <para/>
        /// 与 <see cref="ApplyValueToHost"/> 互为逆操作：
        /// <list type="bullet">
        /// <item><description><c>RecordValueFromHost</c>：宿主组件 → 序列化属性（本方法）；</description></item>
        /// <item><description><c>ApplyValueToHost</c>：序列化属性 → 宿主组件。</description></item>
        /// </list>
        /// 本方法只读取，不做 Undo 记录。
        /// </summary>
        /// <param name="p">目标序列化属性，按 <paramref name="node_type"/> 决定写入的类型分支</param>
        /// <param name="node_type">动画节点类型，决定从哪个组件 / 字段读取当前值</param>
        private void RecordValueFromHost(SerializedProperty p, TweenNodeType node_type)
        {
            switch (node_type)
            {
                // ── 位移：读取 RectTransform.anchoredPosition3D ──
                case TweenNodeType.a_位移:
                    p.vector3Value = BaseScript.controller.mod_Rect.anchoredPosition3D;
                    break;

                // ── 旋转：读取 RectTransform.localEulerAngles（欧拉角）──
                case TweenNodeType.r_旋转:
                    p.vector3Value = BaseScript.controller.mod_Rect.localEulerAngles;
                    break;

                // ── 缩放：读取 RectTransform.localScale ──
                case TweenNodeType.s_缩放:
                    p.vector3Value = BaseScript.controller.mod_Rect.localScale;
                    break;

                // ── 颜色：读取 Graphic.color ──
                case TweenNodeType.c_颜色:
                    // 通过 Controller 识别目标实际挂载的图形类型（Image / RawImage / Text 等），
                    // RecognizeType() 返回对应的 Graphic 基类实例，供统一取色。
                    Graphic gc = BaseScript.controller.RecognizeType();

                    p.colorValue = gc.color;
                    break;

                // ── 淡化：读取 CanvasGroup.alpha ──
                case TweenNodeType.g_淡化:
                    p.floatValue = BaseScript.controller.mod_CanvasGroup.alpha;
                    break;

                // ── 打字机：读取 Text / TmpText 的内容 ──
                // 注意：Text 与 TmpText 互斥，优先 Text，回退 TmpText；
                // 两者都为 null 时保持属性原值不变。
                case TweenNodeType.w_打字机:
                    if (BaseScript.controller.mod_Text != null)
                    {
                        p.stringValue = BaseScript.controller.mod_Text.text;
                    }
                    else if (BaseScript.controller.mod_TmpText != null)
                    {
                        p.stringValue = BaseScript.controller.mod_TmpText.text;
                    }
                    break;

                // ── 图像填充：读取 Image.fillAmount ──
                case TweenNodeType.f_图像填充:
                    p.floatValue = BaseScript.controller.mod_Image.fillAmount;
                    break;

                // ── 尺寸：读取 RectTransform.sizeDelta ──
                case TweenNodeType.z_尺寸:
                    p.vector2Value = BaseScript.controller.mod_Rect.sizeDelta;
                    break;
            }

            // 将序列化属性的改动立即写回 SerializedObject，
            // 否则参数面板上的输入框不会同步刷新。
            p.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
        /// <summary>
        /// 将指定序列化属性的值写回到宿主组件的对应属性上
        /// <para/>
        /// 使用方式：参数面板中「应用到物体」按钮点击时调用。
        /// 每个分支都会先用 <see cref="Undo.RecordObject"/> 记录被修改对象，
        /// 以支持 Ctrl+Z 撤销；同时按 <paramref name="node_type"/> 决定操作目标。
        /// </summary>
        /// <param name="p">来源序列化属性（已由 <see cref="RecordValueFromHost"/> 或手动编辑填入值）</param>
        /// <param name="node_type">动画节点类型，决定值应写回哪个组件 / 字段</param>
        private void ApplyValueToHost(SerializedProperty p, TweenNodeType node_type)
        {
            switch (node_type)
            {
                // ── 位移：写入 RectTransform.anchoredPosition3D ──
                case TweenNodeType.a_位移:
                    if (!isPreviewing)
                        Undo.RecordObject(BaseScript.controller.mod_Rect, "undotransform-position");
                    BaseScript.controller.mod_Rect.anchoredPosition3D = p.vector3Value;
                    break;

                // ── 旋转：写入 RectTransform.localEulerAngles（欧拉角）──
                case TweenNodeType.r_旋转:
                    if (!isPreviewing)
                        Undo.RecordObject(BaseScript.controller.mod_Rect, "undotransform-eulerangle");
                    BaseScript.controller.mod_Rect.localEulerAngles = p.vector3Value;
                    break;

                // ── 缩放：写入 RectTransform.localScale ──
                case TweenNodeType.s_缩放:
                    if (!isPreviewing)
                        Undo.RecordObject(BaseScript.controller.mod_Rect, "undotransform-localscale");
                    BaseScript.controller.mod_Rect.localScale = p.vector3Value;
                    break;

                // ── 颜色：写入 Graphic.color，并同步刷新 Control 上缓存的 OriginalColor ──
                case TweenNodeType.c_颜色:
                    // 通过 Controller 识别目标实际挂载的图形类型（Image / RawImage / Text 等），
                    // RecognizeType() 会返回对应的 Graphic 基类实例，供统一写色。
                    Graphic gc = BaseScript.controller.RecognizeType();

                    // 同步记录 Control 内部的 OriginalColor 缓存，避免下次动画运行时
                    // 用旧的 OriginalColor 覆盖用户刚刚写入的颜色。
                    if (!isPreviewing)
                        Undo.RecordObject(BaseScript.controller.pt_Painting, "undocolor-origin");

                    ModuleType x_Type = BaseScript.controller.GetModuleType();
                    if (x_Type == ModuleType.Image)
                        BaseScript.controller.pt_Painting.OriginalColor = p.colorValue;
                    else if (x_Type == ModuleType.RawImage)
                        BaseScript.controller.pt_Painting.OriginalColor = p.colorValue;

                    gc.color = p.colorValue;
                    break;

                // ── 淡化：写入 CanvasGroup.alpha ──
                case TweenNodeType.g_淡化:
                    if (!isPreviewing)
                        Undo.RecordObject(BaseScript.controller.mod_CanvasGroup, "undoAlpha");
                    BaseScript.controller.mod_CanvasGroup.alpha = p.floatValue;
                    break;

                // ── 打字机：写入 Text / TmpText 的内容 ──
                // 注意：Text 与 TmpText 互斥，优先 Text，回退 TmpText。
                case TweenNodeType.w_打字机:
                    if (BaseScript.controller.mod_Text != null)
                    {
                        if (!isPreviewing)
                            Undo.RecordObject(BaseScript.controller.mod_Text, "undoText");
                        BaseScript.controller.mod_Text.txt_Set_Content(p.stringValue);
                    }
                    else if (BaseScript.controller.mod_TmpText != null)
                    {
                        if (!isPreviewing)
                            Undo.RecordObject(BaseScript.controller.mod_TmpText, "undoTmpText");
                        BaseScript.controller.mod_TmpText.tmp_Set_Content(p.stringValue);
                    }
                    // 文本变更需要刷新编辑器窗口与场景视图，否则预览不会立即更新。
                    Repaint();
                    SceneView.RepaintAll();
                    break;

                // ── 图像填充：写入 Image.fillAmount ──
                case TweenNodeType.f_图像填充:
                    if (!isPreviewing)
                        Undo.RecordObject(BaseScript.controller.mod_Image, "undofill");
                    BaseScript.controller.mod_Image.fillAmount = p.floatValue;
                    break;

                // ── 尺寸：写入 RectTransform.sizeDelta ──
                case TweenNodeType.z_尺寸:
                    if (!isPreviewing)
                        Undo.RecordObject(BaseScript.controller.mod_Rect, "undotransform-size");
                    BaseScript.controller.mod_Rect.sizeDelta = p.vector2Value;
                    break;
            }
        }
        /// <summary>
        /// 按序列化属性类型将其值重置为「零值」（Vector 零 / Color 透明 / 字符串空 / 数值 0）
        /// </summary>
        private void ResetValueToZero(SerializedProperty prop)
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
        private void SwapSerializedValues(SerializedProperty prop_primary, SerializedProperty prop_secondary)
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
        private void DrawLabeledToggle(string title = null, string tooltip = null, SerializedProperty prop = null, float width = 0, XGUIToggleStyle style = XGUIToggleStyle.实体, Color color_bg_on = default, Color color_bg_off = default, Color color_on = default, Color color_off = default, string[] options = null, Action<bool> act_on_changed = null)
        {
            XGUI.layout_toggle(
                title: title,
                tooltip: tooltip,
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

        #region 工具 - 解析TweenerNode
        /// <summary>
        /// 从序列化属性反序列化为 <see cref="TweenNode"/> 实例
        /// <para/>
        /// 用于编辑器中的复制 / 粘贴 / 深拷贝场景。
        /// </summary>
        /// <param name="args">包含节点数据的序列化属性</param>
        /// <returns>反序列化后的节点实例</returns>
        private TweenNode DeserializeTweenNode(SerializedProperty args)
        {
            TweenNode node = new TweenNode();
            node.Indicator = args.FindPropertyRelative("Indicator").stringValue;
            node.ID = args.FindPropertyRelative("ID").intValue;
            node.Enabled = args.FindPropertyRelative("Enabled").boolValue;
            node.Type = (TweenNodeType)args.FindPropertyRelative("Type").enumValueIndex;
            node.Timings = args.FindPropertyRelative("Timings").stringValue;
            node.Duration = args.FindPropertyRelative("Duration").floatValue;
            node.Progress = args.FindPropertyRelative("Progress").floatValue;
            node.Delay = args.FindPropertyRelative("Delay").floatValue;
            node.Ease = (EaseMode)args.FindPropertyRelative("Ease").enumValueIndex;
            node.AnimationCurveName = args.FindPropertyRelative("AnimationCurveName").stringValue;
            node.Curve = args.FindPropertyRelative("Curve").animationCurveValue;
            node.Original_Int = args.FindPropertyRelative("Original_Int").intValue;
            node.Original_Float = args.FindPropertyRelative("Original_Float").floatValue;
            node.Original_Vector2 = args.FindPropertyRelative("Original_Vector2").vector2Value;
            node.Original_Vector3 = args.FindPropertyRelative("Original_Vector3").vector3Value;
            node.Original_Vector4 = args.FindPropertyRelative("Original_Vector4").vector4Value;
            node.Original_Color = args.FindPropertyRelative("Original_Color").colorValue;
            node.Original_String = args.FindPropertyRelative("Original_String").stringValue;
            node.From_Int = args.FindPropertyRelative("From_Int").intValue;
            node.From_Float = args.FindPropertyRelative("From_Float").floatValue;
            node.From_Vector2 = args.FindPropertyRelative("From_Vector2").vector2Value;
            node.From_Vector3 = args.FindPropertyRelative("From_Vector3").vector3Value;
            node.From_Vector4 = args.FindPropertyRelative("From_Vector4").vector4Value;
            node.From_Color = args.FindPropertyRelative("From_Color").colorValue;
            node.From_String = args.FindPropertyRelative("From_String").stringValue;
            node.End_Int = args.FindPropertyRelative("End_Int").intValue;
            node.End_Float = args.FindPropertyRelative("End_Float").floatValue;
            node.End_Vector2 = args.FindPropertyRelative("End_Vector2").vector2Value;
            node.End_Vector3 = args.FindPropertyRelative("End_Vector3").vector3Value;
            node.End_Vector4 = args.FindPropertyRelative("End_Vector4").vector4Value;
            node.End_Color = args.FindPropertyRelative("End_Color").colorValue;
            node.End_String = args.FindPropertyRelative("End_String").stringValue;
            node.RotateMode = (XTweenRotationMode)args.FindPropertyRelative("RotateMode").enumValueIndex;
            node.LoopType = (XTween_LoopType)args.FindPropertyRelative("LoopType").enumValueIndex;
            node.LoopCount = args.FindPropertyRelative("LoopCount").intValue;
            node.IsFold = args.FindPropertyRelative("IsFold").boolValue;
            node.ValueModeIndex = args.FindPropertyRelative("ValueModeIndex").intValue;
            node.TweenValueMode = (TweenValueMode)args.FindPropertyRelative("TweenValueMode").enumValueIndex;

            return node;
        }
        /// <summary>
        /// 将 <see cref="TweenNode"/> 实例序列化到指定的序列化属性中
        /// <para/>
        /// 与 <see cref="DeserializeTweenNode"/> 互为逆操作。
        /// </summary>
        /// <param name="property">目标序列化属性</param>
        /// <param name="node">源节点实例</param>
        private void SerializeTweenNode(SerializedProperty property, TweenNode node)
        {
            property.FindPropertyRelative("Indicator").stringValue = node.Indicator;
            property.FindPropertyRelative("ID").intValue = node.ID;
            property.FindPropertyRelative("Enabled").boolValue = node.Enabled;
            property.FindPropertyRelative("Type").enumValueIndex = (int)node.Type;
            property.FindPropertyRelative("Timings").stringValue = node.Timings;
            property.FindPropertyRelative("Duration").floatValue = node.Duration;
            property.FindPropertyRelative("Progress").floatValue = node.Progress;
            property.FindPropertyRelative("Delay").floatValue = node.Delay;
            property.FindPropertyRelative("Ease").enumValueIndex = (int)node.Ease;
            property.FindPropertyRelative("AnimationCurveName").stringValue = node.AnimationCurveName;
            property.FindPropertyRelative("Curve").animationCurveValue = node.Curve;
            property.FindPropertyRelative("Original_Int").intValue = node.Original_Int;
            property.FindPropertyRelative("Original_Float").floatValue = node.Original_Float;
            property.FindPropertyRelative("Original_Vector2").vector2Value = node.Original_Vector2;
            property.FindPropertyRelative("Original_Vector3").vector3Value = node.Original_Vector3;
            property.FindPropertyRelative("Original_Vector4").vector4Value = node.Original_Vector4;
            property.FindPropertyRelative("Original_Color").colorValue = node.Original_Color;
            property.FindPropertyRelative("Original_String").stringValue = node.Original_String;
            property.FindPropertyRelative("From_Int").intValue = node.From_Int;
            property.FindPropertyRelative("From_Float").floatValue = node.From_Float;
            property.FindPropertyRelative("From_Vector2").vector2Value = node.From_Vector2;
            property.FindPropertyRelative("From_Vector3").vector3Value = node.From_Vector3;
            property.FindPropertyRelative("From_Vector4").vector4Value = node.From_Vector4;
            property.FindPropertyRelative("From_Color").colorValue = node.From_Color;
            property.FindPropertyRelative("From_String").stringValue = node.From_String;
            property.FindPropertyRelative("End_Int").intValue = node.End_Int;
            property.FindPropertyRelative("End_Float").floatValue = node.End_Float;
            property.FindPropertyRelative("End_Vector2").vector2Value = node.End_Vector2;
            property.FindPropertyRelative("End_Vector3").vector3Value = node.End_Vector3;
            property.FindPropertyRelative("End_Vector4").vector4Value = node.End_Vector4;
            property.FindPropertyRelative("End_Color").colorValue = node.End_Color;
            property.FindPropertyRelative("End_String").stringValue = node.End_String;
            property.FindPropertyRelative("RotateMode").enumValueIndex = (int)node.RotateMode;
            property.FindPropertyRelative("LoopType").enumValueIndex = (int)node.LoopType;
            property.FindPropertyRelative("LoopCount").intValue = node.LoopCount;
            property.FindPropertyRelative("IsFold").boolValue = node.IsFold;
            property.FindPropertyRelative("ValueModeIndex").intValue = node.ValueModeIndex;
            property.FindPropertyRelative("TweenValueMode").enumValueIndex = (int)node.TweenValueMode;
            property.serializedObject.ApplyModifiedProperties();
        }
        #endregion

        #region 工具 - 时间统计
        /// <summary>
        /// 判断动画器中是否存在无限循环节点（LoopCount == -1）
        /// </summary>
        /// <returns>true 表示存在无限循环节点</returns>
        public bool HasInfiniteLoopNode()
        {
            bool hasLoop = false;
            for (int i = 0; i < sp_PrimitiveTweenNodes.arraySize; i++)
            {
                SerializedProperty sp_tween_loopcount = sp_PrimitiveTweenNodes.GetArrayElementAtIndex(i).FindPropertyRelative("LoopCount");
                if (sp_tween_loopcount.intValue == -1)
                {
                    hasLoop = true;
                    break;
                }
            }
            return hasLoop;
        }
        /// <summary>
        /// 刷新所有目标组件的动画耗时统计缓存
        /// <para/>
        /// 单选时刷新目标组件，多选时遍历刷新所有选中组件。
        /// </summary>
        private void RefreshTimerStatistics()
        {
            if (!IsMultiSelection())
            {
                BaseScript.TweenNode_GetTimers();
            }
            else
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    SelectedObjects[i].TweenNode_GetTimers();
                }
            }
        }
        #endregion

        #region 工具 - 宿主组件缓存刷新
        /// <summary>
        /// 根据当前 target 刷新其所在的 XHud 宿主组件缓存
        /// <para/>
        /// 宿主组件用于判断动画时机枚举（不同组件类型时机选项不同）。
        /// </summary>
        private void CacheHostComponents()
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

            HudText = BaseScript.GetComponentInParent<XHud_Module_Text>();
            HudTmpText = BaseScript.GetComponentInParent<XHud_Module_TmpText>();
            HudButton = BaseScript.GetComponentInParent<XHud_Module_Button>();
            HudProgress = BaseScript.GetComponentInParent<XHud_Module_Progress>();
            HudToggle = BaseScript.GetComponentInParent<XHud_Module_Toggle>();
            HudSlider = BaseScript.GetComponentInParent<XHud_Module_Slider>();
            HudOption = BaseScript.GetComponentInParent<XHud_Module_Option>();
        }
        #endregion

        #region 工具 - 动画预览
        /// <summary>
        /// 创建并收集图元动画器指定时机下的所有动画实例
        /// </summary>
        /// <param name="tweener">目标图元动画器</param>
        /// <param name="tim">目标时机名称</param>
        /// <returns>收集到的动画实例数组</returns>
        private XTween_Interface[] CollectPreviewTweens(XHud_Module_Primitive_Tween tweener, string tim)
        {
            List<XTween_Interface> tweens = new List<XTween_Interface>();
            for (int i = 0; i < tweener.PrimitiveTweenNodes.Count; i++)
            {
                if (!tweener.PrimitiveTweenNodes[i].Enabled)
                    continue;
                if (tweener.PrimitiveTweenNodes[i].Timings != tim)
                    continue;
                XTween_Interface tween = tweener.Tween_Create(tweener.PrimitiveTweenNodes[i], tweener.GlobalDuration * HudManager.DurationMultiply);

                if (tween != null)
                    tweens.Add(tween);
            }

            return tweens.ToArray();
        }
        /// <summary>
        /// 收集图元动画器中所有出现过的动画时机名称
        /// <para/>
        /// 结果同时写入字段 <see cref="opt_preview_timings"/>，供预览时机下拉框使用。
        /// </summary>
        /// <param name="tweener">目标图元动画器</param>
        /// <returns>时机名称数组</returns>
        private string[] CollectPreviewTimings(XHud_Module_Primitive_Tween tweener)
        {
            List<string> tims = new List<string>();
            for (int i = 0; i < tweener.PrimitiveTweenNodes.Count; i++)
            {
                if (!tweener.PrimitiveTweenNodes[i].Enabled)
                    continue;

                string timing = tweener.PrimitiveTweenNodes[i].Timings;
                if (!tims.Contains(timing))  // 添加前检查是否已存在
                    tims.Add(timing);
            }

            opt_preview_timings = tims.ToArray();

            // ========== 校正 sp_PreviewTiming，避免时机对不上导致无法预览 ==========
            if (sp_PreviewTiming != null)
            {
                // 情况 1：列表非空，但当前值不在合法范围内 → 取第一个合法值
                if (opt_preview_timings.Length > 0 &&
                    !tims.Contains(sp_PreviewTiming.stringValue))
                {
                    sp_PreviewTiming.stringValue = opt_preview_timings[0];
                    sp_PreviewTiming.serializedObject.ApplyModifiedProperties();
                }
                // 情况 2：列表为空 → 清空预览时机
                else if (opt_preview_timings.Length == 0)
                {
                    sp_PreviewTiming.stringValue = "";
                    sp_PreviewTiming.serializedObject.ApplyModifiedProperties();
                }
            }

            return tims.ToArray();
        }
        /// <summary>
        /// 杀死并清空所有节点上已生成的 XTween_Interface 实例
        /// </summary>
        /// <param name="nodes">目标节点列表</param>
        private void KillAndClearPreviewTweens(List<TweenNode> nodes)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                TweenNode node = nodes[i];
                if (node == null || !node.Enabled)
                    continue;

                node.Tweener?.Kill();  // 使用 ?. 简化
                node.Tweener = null;
                node.Progress = 0;
            }
        }
        /// <summary>
        /// 播放预览动画
        /// <para/>
        /// 执行流程：
        /// <list type="number">
        /// <item><description>先停止上一次预览；</description></item>
        /// <item><description>设置预览中标志；</description></item>
        /// <item><description>按当前预览时机收集动画；</description></item>
        /// <item><description>收集并延迟播放匹配时机的音效；</description></item>
        /// <item><description>启动 XTween 预览器播放所有动画。</description></item>
        /// </list>
        /// </summary>
        private void PlayPreview()
        {
            if (sp_PrimitiveTweenNodes == null || sp_PrimitiveTweenNodes.arraySize <= 0)
                return;

            //先停止之前的动画预览
            StopPreview();

            isPreviewing = true;

            //将动画预览中的开关打开
            sp_TweenIsPreviewing.boolValue = true;
            sp_TweenIsPreviewing.serializedObject.ApplyModifiedPropertiesWithoutUndo();

            XTween_Interface[] tweens = null;

            if (IsMultiSelection())
            {
                List<XTween_Interface> mo = new List<XTween_Interface>();
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    XTween_Interface[] sel_tweens = CollectPreviewTweens(SelectedObjects[i], SelectedObjects[i].PreviewTiming);
                    for (int s = 0; s < sel_tweens.Length; s++)
                    {
                        mo.Add(sel_tweens[s]);
                    }
                }
                tweens = mo.ToArray();
                // 预览收集到的有效的音效
                PreviewTweenSounds(sp_PreviewTiming.stringValue, SelectedObjects);
            }
            else
            {
                tweens = CollectPreviewTweens(BaseScript, sp_PreviewTiming.stringValue);

                // 预览收集到的有效的音效
                PreviewTweenSounds(sp_PreviewTiming.stringValue, BaseScript);
            }

            // 使用XTween预览器预览收集到的有效的动画
            StartXTweenPreview(tweens);

            Editor_XHud_Tool_SceneView_Activate_Mark.SetEnabled(
                state: true,
                x_color: XHud_Dashboard.Theme_Primary,
                x_title: "Seven Strike Media",
                x_msg: "图元动画预览中...",
                x_anchor: XHudSceneActivateMarkAnchor.左下);
        }
        /// <summary>
        /// 停止预览动画
        /// <para/>
        /// 可选在停止后从宿主组件恢复原始属性状态。
        /// </summary>
        /// <param name="LoadOriginalState">是否在停止后恢复宿主组件的原始属性</param>
        private void StopPreview(bool LoadOriginalState = true)
        {
            if (target != null)
            {
                sp_TweenIsPreviewing.boolValue = false;
                sp_TweenIsPreviewing.serializedObject.ApplyModifiedPropertiesWithoutUndo();

                KillXTweenPreview();

                if (IsMultiSelection())
                {
                    for (int i = 0; i < SelectedObjects.Length; i++)
                    {
                        if (LoadOriginalState && SelectedObjects[i] != null)
                            SelectedObjects[i].controller.pt_Feature.PrimitiveFeature_Load();
                    }
                }
                else
                {
                    if (LoadOriginalState && target != null)
                        BaseScript.controller.pt_Feature.PrimitiveFeature_Load();
                }

                isPreviewing = false;
            }

            Editor_XHud_Tool_SceneView_Activate_Mark.SetEnabled(false);
        }
        //------------------------------------------------------------------------------------
        /// <summary>
        /// 启动 XTween 预览器并播放传入的所有动画
        /// </summary>
        /// <param name="tweens">已创建好的动画实例数组</param>
        public void StartXTweenPreview(XTween_Interface[] tweens)
        {
            if (Application.isPlaying)
                return;

            Editor_XTween_Previewer.AutoKillWithDuration = false;
            // 预览动画杀死后自动清空预览器的列表 - 状态根据元素脚本的开关
            Editor_XTween_Previewer.AfterKillClear = true;
            // 预览动画杀死前将动画目标的属性倒退 - 状态根据元素脚本的开关
            Editor_XTween_Previewer.BeforeKillRewind = true;

            for (int i = 0; i < tweens.Length; i++)
            {
                Editor_XTween_Previewer.Append(tweens[i]);
            }

            Editor_XTween_Previewer.Play(null);
        }
        /// <summary>
        /// 杀死 XTween 预览器中的所有动画，并清空所有节点的运行时 Tweener
        /// </summary>
        private void KillXTweenPreview()
        {
            if (Application.isPlaying)
                return;

            // 预览开关状态复位
            sp_TweenIsPreviewing.boolValue = false;
            sp_TweenIsPreviewing.serializedObject.ApplyModifiedPropertiesWithoutUndo();

            if (IsMultiSelection())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    XHud_Module_Primitive_Tween tween = SelectedObjects[i];
                    KillAndClearPreviewTweens(tween.PrimitiveTweenNodes);
                }
            }
            else
            {
                KillAndClearPreviewTweens(BaseScript.PrimitiveTweenNodes);
            }

            // 预览器执行动作：杀死动画
            Editor_XTween_Previewer.Kill(true, true, () =>
            {
                //BaseScript.CurrentTweener = null;
            });
        }
        /// <summary>
        /// 将 XTween 预览器中的所有动画倒退到起始状态
        /// </summary>
        private void RewindXTweenPreview()
        {
            Editor_XTween_Previewer.Rewind();
        }
        //------------------------------------------------------------------------------------
        #endregion

        #region 工具 - 音效预览实现
        /// <summary>
        /// 预览单个图元动画器上匹配指定时机的所有音效
        /// <para/>
        /// 音效的延迟时间计算公式：
        /// <c>音效百分比 × 节点时长 × 全局倍增 + 节点延迟</c>
        /// </summary>
        /// <param name="Timings">目标时机名称</param>
        /// <param name="tweener">目标图元动画器</param>
        private void PreviewTweenSounds(string Timings, XHud_Module_Primitive_Tween tweener)
        {
            for (int s = 0; s < tweener.PrimitiveTweenSounds.Count; s++)
            {
                TweenSound sod = tweener.PrimitiveTweenSounds[s];

                // 判断该音效的播放时机是否匹配，如果不匹配则跳过
                if (Timings != sod.Timing)
                    continue;

                float x_vol = sod.Volume;
                float x_pit_min = sod.MinPitch;
                float x_pit_max = sod.MaxPitch;
                float x_delay = sod.Delay;
                bool x_userandom = !(sod.MinPitch == 1 && sod.MaxPitch == 1);
                string x_soundname = sod.Sound.name;

                AudioClip x_clip = HudManager.Hud_Sounds.SoundLibrary_GetSound(x_soundname);
                Preview_PrimitiveTweens_SoundCoroutineList_Stop.Add(XCoroutineUtility.xec_StartCoroutineOwnerless(PlayPreviewSoundCoroutine(x_vol, x_pit_min, x_pit_max, x_userandom, x_clip, x_delay)));
            }
        }
        /// <summary>
        /// 预览多个图元动画器上匹配指定时机的所有音效（批量模式）
        /// </summary>
        /// <param name="Timings">目标时机名称</param>
        /// <param name="tweeners">目标图元动画器数组</param>
        private void PreviewTweenSounds(string Timings, XHud_Module_Primitive_Tween[] tweeners)
        {
            for (int i = 0; i < tweeners.Length; i++)
            {
                XHud_Module_Primitive_Tween tweener = tweeners[i];
                for (int k = 0; k < tweener.PrimitiveTweenSounds.Count; k++)
                {
                    TweenSound sod = tweener.PrimitiveTweenSounds[k];

                    if (sod.Timing != Timings)
                        continue;

                    float x_vol = sod.Volume;
                    float x_pit_min = sod.MinPitch;
                    float x_pit_max = sod.MaxPitch;
                    float x_delay = sod.Delay;
                    bool x_userandom = !(sod.MinPitch == 1 && sod.MaxPitch == 1);
                    string x_soundname = sod.Sound.name;

                    AudioClip x_clip = HudManager.Hud_Sounds.SoundLibrary_GetSound(x_soundname);
                    Preview_PrimitiveTweens_SoundCoroutineList_Stop.Add(XCoroutineUtility.xec_StartCoroutineOwnerless(PlayPreviewSoundCoroutine(x_vol, x_pit_min, x_pit_max, x_userandom, x_clip, x_delay)));
                }
            }
        }
        /// <summary>
        /// 音效预览协程：延迟指定时间后创建 AudioSource 播放，播放完成后销毁
        /// </summary>
        IEnumerator PlayPreviewSoundCoroutine(float sp_vol, float sp_pitch_min, float sp_pitch_max, bool sp_userandom, AudioClip clip, float delay)
        {
            yield return new XCoroutineWaitForSeconds(delay);
            Preview_PrimitiveTweens_SoundList.Add(CreatePreviewAudioSource(sp_vol, sp_pitch_min, sp_pitch_max, sp_userandom, clip));
            AudioSource au = Preview_PrimitiveTweens_SoundList[Preview_PrimitiveTweens_SoundList.Count - 1];
            while (true)
            {
                if (au != null && !au.isPlaying)
                {
                    break;
                }
                yield return null;
            }
            DestroyImmediate(au.gameObject, true);
        }
        /// <summary>
        /// 停止所有音效预览协程，并销毁所有已生成的预览 AudioSource
        /// </summary>
        private void StopAllPreviewSounds()
        {
            for (int i = 0; i < Preview_PrimitiveTweens_SoundCoroutineList_Stop.Count; i++)
            {
                if (Preview_PrimitiveTweens_SoundCoroutineList_Stop[i] != null)
                    XCoroutineUtility.xec_StopCoroutine(Preview_PrimitiveTweens_SoundCoroutineList_Stop[i]);
            }
            Preview_PrimitiveTweens_SoundCoroutineList_Stop.Clear();

            if (Preview_PrimitiveTweens_SoundList != null)
            {
                for (int i = 0; i < Preview_PrimitiveTweens_SoundList.Count; i++)
                {
                    if (Preview_PrimitiveTweens_SoundList[i] != null)
                    {
                        Preview_PrimitiveTweens_SoundList[i].Stop();
                        DestroyImmediate(Preview_PrimitiveTweens_SoundList[i].gameObject, true);
                        Preview_PrimitiveTweens_SoundList[i] = null;
                    }
                }
                Preview_PrimitiveTweens_SoundList.Clear();
            }

            SceneView.RepaintAll();
        }
        /// <summary>
        /// 创建一个用于音效预览的临时 AudioSource 并立即播放
        /// <para/>
        /// 附带 <see cref="XHud_AudioStoper"/> 组件以支持自动停止。
        /// </summary>
        public AudioSource CreatePreviewAudioSource(float sp_vol, float sp_pitch_min, float sp_pitch_max, bool sp_userandom, AudioClip clip)
        {
            GameObject obj = new GameObject();
            obj.name = "PrimitiveTweens_Sound_Previewer-" + "[" + clip.length.ToString("F2") + " s]-" + "[" + clip.channels + " ch]-" + "[" + clip.frequency + " hz]";
            AudioSource au = obj.AddComponent<AudioSource>();
            au.clip = clip;
            au.volume = sp_vol;
            if (sp_userandom)
            {
                au.pitch = Random.Range(sp_pitch_min, sp_pitch_max);
            }
            else
            {
                au.pitch = 1.0f;
            }
            au.Play();
            XHud_AudioStoper sp = au.gameObject.AddComponent<XHud_AudioStoper>();
            sp.SetAudioSource(au);
            return au;
        }
        #endregion
    }
}