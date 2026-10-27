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
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// 元素预览器参数快照。
    /// <para/>
    /// <list type="bullet">
    /// <item><description>用于右键菜单中拷贝/粘贴脚本参数。</description></item>
    /// <item><description>通过 <c>JsonUtility</c> 序列化为 JSON 字符串。</description></item>
    /// </list>
    /// </summary>
    public class XHud_Element_PreviewArgs
    {
        public float DelayWithIn;
        public float DelayWithOut;
        public KeyCode key_Element_In;
        public KeyCode key_Element_Out;
        public bool HideWithStart;
        public Motion_Creator CreateArgs;
        public Motion_Recycler RecycleArgs;
        public KeyCode key_create;
        public KeyCode key_recycle;
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(XHud_Element_Preview))]
    public class Editor_XHud_Element_Preview : Editor
    {
        #region 字段 - 组件 / 列表
        /// <summary>
        /// 当前编辑的目标脚本实例。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>在 <c>OnEnable</c> 中由 <c>target</c> 强制转换获得。</description></item>
        /// <item><description>仅用于单选模式下的直接读写操作。</description></item>
        /// </list>
        /// </summary>
        private XHud_Element_Preview base_script;
        /// <summary>
        /// HUD 管理器引用。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>在 <c>OnEnable</c> 中通过 <c>XHud_Dashboard.HudManagerGet()</c> 获取。</description></item>
        /// <item><description>用于访问动效库、RMS 配置等全局数据。</description></item>
        /// </list>
        /// </summary>
        private XHud_Manager hud_manager;
        #endregion

        #region 字段 - 序列化属性
        /// <summary>
        /// 预览开关属性
        /// </summary>
        SerializedProperty sp_is_enabled;
        /// <summary>
        /// 调试开关属性
        /// </summary>
        SerializedProperty sp_debug_state;
        /// <summary>
        /// 缩放耗时属性
        /// </summary>
        SerializedProperty sp_duration_scaler;
        /// <summary>
        /// 预设位置属性
        /// </summary>
        SerializedProperty sp_preset_position;
        /// <summary>
        /// 预设角度属性
        /// </summary>
        SerializedProperty sp_preset_euler;
        /// <summary>
        /// 预览进入按键属性
        /// </summary>
        SerializedProperty sp_key_element_in;
        /// <summary>
        /// 预览退出按键属性
        /// </summary>
        SerializedProperty sp_key_element_out;
        /// <summary>
        /// 开始时隐藏属性
        /// </summary>
        SerializedProperty sp_hide_with_start;
        /// <summary>
        /// 生成动效库名称属性
        /// </summary>
        SerializedProperty sp_motion_create_library_index_name;
        /// <summary>
        /// 回收动效库名称属性
        /// </summary>
        SerializedProperty sp_motion_recycle_library_index_name;
        /// <summary>
        /// 进入延时属性
        /// </summary>
        SerializedProperty sp_delay_with_in;
        /// <summary>
        /// 退出延时属性
        /// </summary>
        SerializedProperty sp_delay_with_out;
        /// <summary>
        /// RMS 开关属性
        /// </summary>
        SerializedProperty sp_rms_enabled;
        /// <summary>
        /// RMS 名称属性
        /// </summary>
        SerializedProperty sp_rms_name;
        /// <summary>
        /// 元素组件属性
        /// </summary>
        SerializedProperty sp_element;
        /// <summary>
        /// 编辑器预览运行状态属性
        /// </summary>
        SerializedProperty sp_editor_preview_is_running;
        #endregion

        #region 字段 - 图标
        /// <summary>
        /// 标题主图标
        /// </summary>
        private Texture2D icon_main;
        /// <summary>
        /// 保存按钮常态图标
        /// </summary>
        private Texture2D save_r;
        /// <summary>
        /// 保存按钮按下图标
        /// </summary>
        private Texture2D save_p;
        /// <summary>
        /// 定位按钮常态图标
        /// </summary>
        private Texture2D locate_r;
        /// <summary>
        /// 定位按钮按下图标
        /// </summary>
        private Texture2D locate_p;
        /// <summary>
        /// 重置按钮常态图标
        /// </summary>
        private Texture2D reset_r;
        /// <summary>
        /// 重置按钮按下图标
        /// </summary>
        private Texture2D reset_p;
        /// <summary>
        /// 编辑按钮常态图标
        /// </summary>
        private Texture2D edit_r;
        /// <summary>
        /// 编辑按钮按下图标
        /// </summary>
        private Texture2D edit_p;
        #endregion

        #region 字段 - 批量化操作
        /// <summary>
        /// 批量选择脚本数组。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>在 <c>Targets_Get</c> 中根据 <c>targets</c> 填充。</description></item>
        /// <item><description>单选时长度为 1，多选时长度等于选中对象数量。</description></item>
        /// </list>
        /// </summary>
        private XHud_Element_Preview[] selected_objects;
        #endregion

        #region 生命周期        
        private void OnEnable()
        {
            hud_manager = XHud_Dashboard.get_manager();
            base_script = (XHud_Element_Preview)target;

            #region 获取序列化属性
            sp_element = serializedObject.FindProperty(nameof(XHud_Element_Preview.element));
            sp_key_element_in = serializedObject.FindProperty(nameof(XHud_Element_Preview.key_element_in));
            sp_key_element_out = serializedObject.FindProperty(nameof(XHud_Element_Preview.key_element_out));
            sp_hide_with_start = serializedObject.FindProperty(nameof(XHud_Element_Preview.hide_with_start));
            sp_motion_create_library_index_name = serializedObject.FindProperty(nameof(XHud_Element_Preview.motion_create_library_index_name));
            sp_motion_recycle_library_index_name = serializedObject.FindProperty(nameof(XHud_Element_Preview.motion_recycle_library_index_name));
            sp_delay_with_in = serializedObject.FindProperty(nameof(XHud_Element_Preview.delay_with_in));
            sp_delay_with_out = serializedObject.FindProperty(nameof(XHud_Element_Preview.delay_with_out));
            sp_is_enabled = serializedObject.FindProperty(nameof(XHud_Element_Preview.is_enabled));
            sp_debug_state = serializedObject.FindProperty(nameof(XHud_Element_Preview.debug_state));
            sp_preset_position = serializedObject.FindProperty(nameof(XHud_Element_Preview.preset_position));
            sp_preset_euler = serializedObject.FindProperty(nameof(XHud_Element_Preview.preset_euler));
            sp_duration_scaler = serializedObject.FindProperty(nameof(XHud_Element_Preview.duration_scaler));
            sp_rms_enabled = serializedObject.FindProperty(nameof(XHud_Element_Preview.rms_enabled));
            sp_rms_name = serializedObject.FindProperty(nameof(XHud_Element_Preview.rms_name));
            sp_editor_preview_is_running = serializedObject.FindProperty(XHud_Element_Preview.editor_preview_is_running);
            #endregion

            #region 获取图标
            icon_main = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_element_preview/icon_main");
            save_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_element_preview/save_r");
            save_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_element_preview/save_p");
            locate_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_element_preview/locate_r");
            locate_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_element_preview/locate_p");
            reset_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_element_preview/reset_r");
            reset_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_element_preview/reset_p");
            edit_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_element_preview/edit_r");
            edit_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_element_preview/edit_p");
            #endregion

            Targets_Get();

            CheckRmsNameValid();

            if (Targets_Selected())
            {
                for (int i = 0; i < selected_objects.Length; i++)
                {
                    if (selected_objects[i].element == null)
                    {
                        selected_objects[i].element = selected_objects[i].GetComponent<XHud_Module_Element>();
                    }
                    else
                    {
                        if (selected_objects[i].element.GetInstanceID() != selected_objects[i].GetInstanceID())
                        {
                            selected_objects[i].element = selected_objects[i].GetComponent<XHud_Module_Element>();
                        }
                    }
                }
            }
            else
            {
                if (sp_element.objectReferenceValue == null)
                {
                    sp_element.objectReferenceValue = base_script.GetComponent<XHud_Module_Element>();
                    sp_element.serializedObject.ApplyModifiedProperties();
                }
                else
                {
                    if (sp_element.objectReferenceValue.GetInstanceID() != base_script.GetInstanceID())
                    {
                        sp_element.objectReferenceValue = base_script.GetComponent<XHud_Module_Element>();
                        sp_element.serializedObject.ApplyModifiedProperties();
                    }
                }
            }
        }

        private void OnDisable()
        {

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            string hexcol = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.themes_get_color(ThemeColorType.primary), true);

            TextClipping clipping_ellipsis = XGUI.TryEllipsisClipping();

            #region 标题
            XGUI.layout_banner(
             bg_fill: XGUIGroup.实体,
             bg_color: XGUIColor.深空灰,
             bg_height: 30,
             icon: icon_main,
             icon_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
             title_text: "XHud  -  元素预览器",
             title_anchor: TextAnchor.MiddleLeft,
             title_style: FontStyle.Normal,
             title_color: Color.white,
             title_size: XGUIFontSize.B,
             title_clipping: clipping_ellipsis,
             bg_margin: new RectOffset(0, 0, 5, 5));
            #endregion

            XGUI.layout_space(5);

            #region 参数
            base_script.fold_param = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "参数",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: base_script.fold_param);
            if (!base_script.fold_param)
            {
                #region 元素组件
                XGUI.layout_property_field(
                    title: "元素组件",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: sp_element,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                #region 延时进入
                XGUI.layout_property_field(
                    title: "延时进入",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: sp_delay_with_in,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                #region 延时退出
                XGUI.layout_property_field(
                    title: "延时退出",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: sp_delay_with_out,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                #region 缩放耗时
                XGUI.layout_property_field(
                    title: "缩放耗时",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: sp_duration_scaler,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 预览按键
            base_script.fold_key = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "预览按键",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: base_script.fold_key);
            if (!base_script.fold_key)
            {
                #region 元素进入
                XGUI.layout_property_field(
                    title: "元素进入",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: sp_key_element_in,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                #region 元素退出
                XGUI.layout_property_field(
                    title: "元素退出",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: sp_key_element_out,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 状态
            base_script.fold_state = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "状态",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: base_script.fold_state);
            if (!base_script.fold_state)
            {
                #region 动效状态
                XGUI.layout_state_displayer_text(
                    title: "动效状态",
                    title_size: XGUIFontSize.M,
                    subtitle: sp_editor_preview_is_running.boolValue ? "动效中" : "待命中",
                    subtitle_size: XGUIFontSize.M,
                    subtitle_color: sp_editor_preview_is_running.boolValue ? XHud_Dashboard.themes_get_color(ThemeColorType.primary) : Color.gray,
                    margin: new RectOffset(5, 5, 0, 5));
                #endregion
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 选项
            base_script.fold_options = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "选项",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: base_script.fold_options);
            if (!base_script.fold_options)
            {
                DrawToggle("状态调试", sp_debug_state, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, (b) => { });
                DrawToggle("预览开关", sp_is_enabled, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, (b) =>
                {

                });
                DrawToggle("开始时隐藏", sp_hide_with_start, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, (b) =>
                {

                });
                DrawToggle("R M S 模式", sp_rms_enabled, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, (b) =>
                {
                    CheckRmsNameValid();
                });

                #region RMS 说明
                if (sp_rms_enabled.boolValue)
                {
                    string msg = $"请注意！如果您的预览目标<color={hexcol}><b>元素自身的 RMS 未开启或无效</b></color>，则不会根据 RMS的参数<color={hexcol}><b>（指定且存在RMS配置） </b></color>进行元素的<color={hexcol}><b>变换姿态设定</b></color>赋值！";
                    XGUI.layout_label(
                        text: msg,
                        bg_fill: XGUIGroup.实体,
                        bg_color: XGUIColor.深空灰,
                        size: XGUIFontSize.S,
                        anchor: TextAnchor.MiddleLeft,
                        text_color: Color.white * 0.85f,
                        offset: new Vector2(0, 0),
                        padding: new RectOffset(10, 10, 10, 10),
                        margin: new RectOffset(0, 0, 6, 0),
                        clipping: TextClipping.Clip,
                        wrap: true,
                        font: XGUI.GetFont("xg-regular"),
                        font_style: FontStyle.Normal);
                }
                #endregion
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 预设变换信息
            base_script.fold_preset_trans = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "预设变换信息",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: base_script.fold_preset_trans);
            if (!base_script.fold_preset_trans)
            {
                DrawParamField("预设原始位置", sp_preset_position, 100, Color.clear);

                DrawParamField("预设原始角度", sp_preset_euler, 100, Color.clear);

                #region 记录当前位置 & 角度
                if (XGUI.layout_button(
                    text: "记录当前位置 & 角度",
                    tooltip: "",
                    bg_fill: XGUIGroup.实体,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    button_text_color: Color.black,
                    press_fill: XGUIGroup.实体,
                    press_color: XGUIColor.深空灰,
                    press_text_color: Color.white,
                    font_size: XGUIFontSize.M,
                    anchor: TextAnchor.MiddleCenter,
                    margin: new RectOffset(0, 0, 15, 0),
                    padding: new RectOffset(0, 0, 0, 0),
                    height: 20,
                    button_text_font: XGUI.GetFont("xg-medium")))
                {
                    if (Targets_Selected())
                    {
                        for (int i = 0; i < selected_objects.Length; i++)
                        {
                            if (selected_objects[i].element == null)
                                continue;
                            selected_objects[i].preset_position = selected_objects[i].element.rect_transform.anchoredPosition3D;
                            selected_objects[i].preset_euler = selected_objects[i].element.rect_transform.localEulerAngles;
                        }
                    }
                    else
                    {
                        if (base_script.element == null)
                            return;
                        sp_preset_position.vector3Value = base_script.element.rect_transform.anchoredPosition3D;
                        sp_preset_euler.vector3Value = base_script.element.rect_transform.localEulerAngles;
                        sp_preset_position.serializedObject.ApplyModifiedProperties();
                        sp_preset_euler.serializedObject.ApplyModifiedProperties();
                    }
                }
                #endregion
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region RMS布局信息
            if (sp_rms_enabled.boolValue)
            {
                base_script.fold_rms = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIGroup.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                    title: "RMS 布局信息",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: base_script.fold_rms);
                if (!base_script.fold_rms)
                {
                    #region 分辨率方案列表
                    ScreenResolutionNode[] nodes = hud_manager.hm_RMS_GetResolutionNodes();
                    if (nodes.Length > 0)
                    {
                        string[] nodesName = hud_manager.hm_RMS_GetResolutionNodeNames();
                        sp_rms_name.stringValue = XGUI.layout_string_popup(
                          title: "画布尺寸",
                          title_width: 60,
                          title_size: XGUIFontSize.M,
                          title_anchor: TextAnchor.MiddleLeft,
                          prop: sp_rms_name,
                          options: nodesName,
                          opt_text_size: XGUIFontSize.M,
                          opt_text_color: Color.black,
                          opt_text_padding: new RectOffset(10, 10, 0, 0),
                          opt_anchor: TextAnchor.MiddleLeft,
                          opt_font_style: FontStyle.Normal,
                          opt_bg_fill: XGUIGroup.实体,
                          opt_bg_color: XGUIColor.亮白,
                          opt_bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                          icon_arrow_color: Color.black,
                          margin: new RectOffset(0, 0, 5, 5),
                          padding: new RectOffset(5, 5, 0, 0),
                          title_margin: new RectOffset(0, 0, 0, 0),
                          act_on_changed: (value) =>
                          {

                          });
                        sp_rms_name.serializedObject.ApplyModifiedProperties();
                        XGUI.layout_seperator(
                            thickness: 1,
                            color: XHud_Dashboard.themes_get_color(ThemeColorType.sepline),
                            margin: new RectOffset(15, 15, 15, 15));
                        Element_RMS_LayoutData info = base_script.element.elelemt_RMS_Get(sp_rms_name.stringValue);
                        if (info != null)
                        {
                            #region 锚点
                            XGUI.layout_state_displayer_text(
                                title: "锚点",
                                title_size: XGUIFontSize.M,
                                subtitle: info.anchor.ToString(),
                                subtitle_size: XGUIFontSize.M,
                                subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                margin: new RectOffset(5, 5, 0, 5));
                            #endregion

                            #region 最小锚点
                            XGUI.layout_state_displayer_text(
                                title: "最小锚点",
                                title_size: XGUIFontSize.M,
                                subtitle: info.anchor_min.ToString(),
                                subtitle_size: XGUIFontSize.M,
                                subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                margin: new RectOffset(5, 5, 0, 5));
                            #endregion

                            #region 最大锚点
                            XGUI.layout_state_displayer_text(
                                title: "最大锚点",
                                title_size: XGUIFontSize.M,
                                subtitle: info.anchor_max.ToString(),
                                subtitle_size: XGUIFontSize.M,
                                subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                margin: new RectOffset(5, 5, 0, 5));
                            #endregion

                            #region 位置
                            XGUI.layout_state_displayer_text(
                                title: "位置",
                                title_size: XGUIFontSize.M,
                                subtitle: info.position.ToString(),
                                subtitle_size: XGUIFontSize.M,
                                subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                margin: new RectOffset(5, 5, 0, 5));
                            #endregion

                            #region 角度
                            XGUI.layout_state_displayer_text(
                                title: "角度",
                                title_size: XGUIFontSize.M,
                                subtitle: info.euler.ToString(),
                                subtitle_size: XGUIFontSize.M,
                                subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                margin: new RectOffset(5, 5, 0, 5));
                            #endregion

                            #region 缩放
                            XGUI.layout_state_displayer_text(
                                title: "缩放",
                                title_size: XGUIFontSize.M,
                                subtitle: info.scale.ToString(),
                                subtitle_size: XGUIFontSize.M,
                                subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                margin: new RectOffset(5, 5, 0, 5));
                            #endregion

                            #region 轴心
                            XGUI.layout_state_displayer_text(
                                title: "轴心",
                                title_size: XGUIFontSize.M,
                                subtitle: info.pivot.ToString(),
                                subtitle_size: XGUIFontSize.M,
                                subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                margin: new RectOffset(5, 5, 0, 5));
                            #endregion
                        }
                        else
                        {
                            XGUI.layout_label(
                                text: "暂未在元素上找到 RMS 设计布局信息",
                                size: XGUIFontSize.M,
                                text_color: Color.gray,
                                padding: new RectOffset(0, 0, 0, -10),
                                margin: new RectOffset(0, 0, 15, 35),
                                clipping: TextClipping.Clip,
                                font_style: FontStyle.Normal,
                                anchor: TextAnchor.MiddleCenter);
                        }
                    }
                    else
                    {
                        XGUI.layout_label(
                            text: "暂未在管理器中配置 RMS 布局列表",
                            size: XGUIFontSize.M,
                            text_color: Color.gray,
                            padding: new RectOffset(0, 0, 0, -10),
                            margin: new RectOffset(0, 0, 15, 35),
                            clipping: TextClipping.Clip,
                            font_style: FontStyle.Normal,
                            anchor: TextAnchor.MiddleCenter);
                    }
                    #endregion
                }
                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            }
            #endregion

            #region 动效参数
            base_script.fold_motion = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "动效参数",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: base_script.fold_motion);
            if (!base_script.fold_motion)
            {
                #region 生成
                #region 动效模板
                if (hud_manager.library_motions != null)
                {
                    //确保动效库不是空的
                    if (hud_manager.library_motions.motion_list == null || hud_manager.library_motions.motion_list.Count <= 0)
                    {
                        XGUI.layout_label(
                            text: "未在动效库中发现任何动效资源，请先为其添加动效资源！",
                            bg_fill: XGUIGroup.实体,
                            bg_color: XGUIColor.深空灰,
                            size: XGUIFontSize.S,
                            anchor: TextAnchor.MiddleLeft,
                            text_color: Color.white * 0.85f,
                            offset: new Vector2(0, 0),
                            padding: new RectOffset(10, 10, 10, 10),
                            margin: new RectOffset(0, 0, 6, 0),
                            clipping: TextClipping.Clip,
                            wrap: true,
                            font_style: FontStyle.Normal);
                    }
                    else
                    {
                        #region 模板下拉菜单
                        string[] motnames = hud_manager.library_motions.ElementMotion_GetAllName_With_Create();
                        XGUI.layout_string_popup(
                            title: XGUI.CurrentWindowWidthThreshold(">", 240) ? "模板 - 生成" : null,
                            title_width: 120,
                            title_size: XGUIFontSize.M,
                            title_anchor: TextAnchor.MiddleLeft,
                            prop: sp_motion_create_library_index_name,
                            options: motnames,
                            opt_text_size: XGUIFontSize.M,
                            opt_text_color: Color.black,
                            opt_text_padding: new RectOffset(10, 10, 0, 0),
                            opt_anchor: TextAnchor.MiddleCenter,
                            opt_font_style: FontStyle.Normal,
                            opt_bg_fill: XGUIGroup.实体,
                            opt_bg_color: XGUIColor.亮白,
                            opt_bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                            icon_arrow_color: Color.black,
                            margin: new RectOffset(0, 0, 5, 5),
                            padding: new RectOffset(5, 5, 0, 0),
                            title_margin: new RectOffset(0, 0, 0, 0),
                            act_on_changed: (value) =>
                            {
                                sp_motion_create_library_index_name.stringValue = value;
                                sp_motion_create_library_index_name.serializedObject.ApplyModifiedProperties();

                                Motion_Creator crc = hud_manager.library_motions.ElementMotion_GetElementCreator_At_Create(sp_motion_create_library_index_name.stringValue);
                                base_script.motion_create_args = crc.Clone();
                            });
                        #endregion
                    }
                }
                else
                {
                    XGUI.layout_label(
                       text: "XHud 管理器中未指定动效库，请先配置动效库！",
                       bg_fill: XGUIGroup.实体,
                       bg_color: XGUIColor.深空灰,
                       size: XGUIFontSize.S,
                       anchor: TextAnchor.MiddleLeft,
                       text_color: Color.white * 0.85f,
                       offset: new Vector2(0, 0),
                       padding: new RectOffset(10, 10, 10, 10),
                       margin: new RectOffset(0, 0, 6, 0),
                       clipping: TextClipping.Clip,
                       wrap: true,
                       font_style: FontStyle.Normal);
                }
                #endregion

                XGUI.layout_group_start(
                    type: XGUIContainerType.Horizontal,
                    title_clipping: TextClipping.Clip,
                    absolute_padding: true,
                    absolute_margin: true,
                    margin: new RectOffset(0, 0, 12, 0),
                    padding: new RectOffset(5, 10, 0, 0));

                #region 保存
                XGUI.SetEnabled(hud_manager.library_motions != null);
                if (XGUI.layout_button(
                    tooltip: "保存",
                    tex_release: save_r,
                    tex_press: save_p,
                    tex_gui_color: Color.white,
                    border: new RectOffset(0, 0, 0, 0),
                    width: save_r.width,
                    height: save_r.height))
                {
                    motion_save(HudElementMotionType.Creator);
                    return;
                }
                #endregion

                XGUI.layout_flexspace();

                #region 定位
                XGUI.SetEnabled(hud_manager.library_motions != null);
                if (XGUI.layout_button(
                  tooltip: "定位",
                  tex_release: locate_r,
                  tex_press: locate_p,
                  tex_gui_color: Color.white,
                  border: new RectOffset(0, 0, 0, 0),
                  width: locate_r.width,
                  height: locate_r.height))
                {
                    if (!hud_manager.library_motions.ElementMotion_IsExist(sp_motion_create_library_index_name.stringValue))
                        return;
                    Editor_XHud_MenuItemsAction_OpenLibrary.open_elementmotion();
                    hud_manager.library_motions.ElementMotionLibrary_Location(sp_motion_create_library_index_name.stringValue);
                    return;
                }
                XGUI.SetEnabled(true);
                #endregion

                XGUI.layout_flexspace();

                #region 修改
                if (XGUI.layout_button(
                  tooltip: "修改",
                  tex_release: edit_r,
                  tex_press: edit_p,
                  tex_gui_color: Color.white,
                  border: new RectOffset(0, 0, 0, 0),
                  width: edit_r.width,
                  height: edit_r.height))
                {
                    motion_edit(new XHud_Arg_MotionGroup(base_script.motion_create_args));
                    return;
                }
                #endregion

                XGUI.layout_flexspace();

                #region 重置
                if (XGUI.layout_button(
                  tooltip: "重置",
                  tex_release: reset_r,
                  tex_press: reset_p,
                  tex_gui_color: Color.white,
                  border: new RectOffset(0, 0, 0, 0),
                  width: reset_r.width,
                  height: reset_r.height))
                {
                    XGUI.dialog(
                        type: XGUIDialogType.警告,
                        windowtitle: "XHud - 元素生成器消息",
                        title: "重置动效参数",
                        msg: "确定要将动效参数重置吗？您将丢失当前的动效参数！",
                        ok: "重置",
                        cancel: "暂不",
                        PrimaryIndex: 0,
                        usemodal: true,
                        themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        on_selected: (d) =>
                        {
                            if (d == "重置")
                            {
                                sp_motion_create_library_index_name.stringValue = null;
                                sp_motion_create_library_index_name.serializedObject.ApplyModifiedProperties();
                                base_script.motion_create_args.Reset();
                            }
                        });
                    return;
                }
                #endregion

                XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
                #endregion

                XGUI.layout_seperator(
                    thickness: 1,
                    color: XHud_Dashboard.themes_get_color(ThemeColorType.sepline),
                    margin: new RectOffset(15, 15, 15, 15));

                #region 回收
                #region 动效模板
                if (hud_manager.library_motions != null)
                {
                    //确保动效库不是空的
                    if (hud_manager.library_motions.motion_list == null || hud_manager.library_motions.motion_list.Count <= 0)
                    {
                        XGUI.layout_label(
                            text: "未在动效库中发现任何动效资源，请先为其添加动效资源！",
                            bg_fill: XGUIGroup.实体,
                            bg_color: XGUIColor.深空灰,
                            size: XGUIFontSize.S,
                            anchor: TextAnchor.MiddleLeft,
                            text_color: Color.white * 0.85f,
                            offset: new Vector2(0, 0),
                            padding: new RectOffset(10, 10, 10, 10),
                            margin: new RectOffset(0, 0, 6, 0),
                            clipping: TextClipping.Clip,
                            wrap: true,
                            font_style: FontStyle.Normal);
                    }
                    else
                    {
                        #region 模板下拉菜单
                        string[] motnames = hud_manager.library_motions.ElementMotion_GetAllName_With_Recycle();
                        XGUI.layout_string_popup(
                            title: XGUI.CurrentWindowWidthThreshold(">", 240) ? "模板 - 回收" : null,
                            title_width: 120,
                            title_size: XGUIFontSize.M,
                            title_anchor: TextAnchor.MiddleLeft,
                            prop: sp_motion_recycle_library_index_name,
                            options: motnames,
                            opt_text_size: XGUIFontSize.M,
                            opt_text_color: Color.black,
                            opt_text_padding: new RectOffset(10, 10, 0, 0),
                            opt_anchor: TextAnchor.MiddleCenter,
                            opt_font_style: FontStyle.Normal,
                            opt_bg_fill: XGUIGroup.实体,
                            opt_bg_color: XGUIColor.亮白,
                            opt_bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                            icon_arrow_color: Color.black,
                            margin: new RectOffset(0, 0, 5, 5),
                            padding: new RectOffset(5, 5, 0, 0),
                            title_margin: new RectOffset(0, 0, 0, 0),
                            act_on_changed: (value) =>
                            {
                                sp_motion_recycle_library_index_name.stringValue = value;
                                sp_motion_recycle_library_index_name.serializedObject.ApplyModifiedProperties();

                                Motion_Recycler crc = hud_manager.library_motions.ElementMotion_GetElementCreator_At_Recycle(sp_motion_recycle_library_index_name.stringValue);
                                base_script.motion_recycle_args = crc.Clone();
                            });
                        #endregion
                    }
                }
                else
                {
                    XGUI.layout_label(
                       text: "XHud 管理器中未指定动效库，请先配置动效库！",
                       bg_fill: XGUIGroup.实体,
                       bg_color: XGUIColor.深空灰,
                       size: XGUIFontSize.S,
                       anchor: TextAnchor.MiddleLeft,
                       text_color: Color.white * 0.85f,
                       offset: new Vector2(0, 0),
                       padding: new RectOffset(10, 10, 10, 10),
                       margin: new RectOffset(0, 0, 6, 0),
                       clipping: TextClipping.Clip,
                       wrap: true,
                       font_style: FontStyle.Normal);
                }
                #endregion

                XGUI.layout_group_start(
                    type: XGUIContainerType.Horizontal,
                    title_clipping: TextClipping.Clip,
                    absolute_padding: true,
                    absolute_margin: true,
                    margin: new RectOffset(0, 0, 12, 0),
                    padding: new RectOffset(5, 10, 0, 0));

                #region 保存
                XGUI.SetEnabled(hud_manager.library_motions != null);
                if (XGUI.layout_button(
                    tooltip: "保存",
                    tex_release: save_r,
                    tex_press: save_p,
                    tex_gui_color: Color.white,
                    border: new RectOffset(0, 0, 0, 0),
                    width: save_r.width,
                    height: save_r.height))
                {
                    motion_save(HudElementMotionType.Recycler);
                    return;
                }
                #endregion

                XGUI.layout_flexspace();

                #region 定位
                XGUI.SetEnabled(hud_manager.library_motions != null);
                if (XGUI.layout_button(
                  tooltip: "定位",
                  tex_release: locate_r,
                  tex_press: locate_p,
                  tex_gui_color: Color.white,
                  border: new RectOffset(0, 0, 0, 0),
                  width: locate_r.width,
                  height: locate_r.height))
                {
                    if (!hud_manager.library_motions.ElementMotion_IsExist(sp_motion_recycle_library_index_name.stringValue))
                        return;
                    Editor_XHud_MenuItemsAction_OpenLibrary.open_elementmotion();
                    hud_manager.library_motions.ElementMotionLibrary_Location(sp_motion_recycle_library_index_name.stringValue);
                    return;
                }
                XGUI.SetEnabled(true);
                #endregion

                XGUI.layout_flexspace();

                #region 修改
                if (XGUI.layout_button(
                  tooltip: "修改",
                  tex_release: edit_r,
                  tex_press: edit_p,
                  tex_gui_color: Color.white,
                  border: new RectOffset(0, 0, 0, 0),
                  width: edit_r.width,
                  height: edit_r.height))
                {
                    motion_edit(new XHud_Arg_MotionGroup(base_script.motion_recycle_args));
                    return;
                }
                #endregion

                XGUI.layout_flexspace();

                #region 重置
                if (XGUI.layout_button(
                  tooltip: "重置",
                  tex_release: reset_r,
                  tex_press: reset_p,
                  tex_gui_color: Color.white,
                  border: new RectOffset(0, 0, 0, 0),
                  width: reset_r.width,
                  height: reset_r.height))
                {
                    XGUI.dialog(
                        type: XGUIDialogType.警告,
                        windowtitle: "XHud - 元素生成器消息",
                        title: "重置动效参数",
                        msg: "确定要将动效参数重置吗？您将丢失当前的动效参数！",
                        ok: "重置",
                        cancel: "暂不",
                        PrimaryIndex: 0,
                        usemodal: true,
                        themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        on_selected: (d) =>
                        {
                            if (d == "重置")
                            {
                                sp_motion_recycle_library_index_name.stringValue = null;
                                sp_motion_recycle_library_index_name.serializedObject.ApplyModifiedProperties();
                                base_script.motion_recycle_args.Reset();
                            }
                        });
                    return;
                }
                #endregion

                XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
                #endregion
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 菜单
            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                if (Targets_Selected())
                    return;

                // 创建右键菜单
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("E (复制动效)"), false, () =>
                {
                    string res = XGUI.dialog(
                        type: XGUIDialogType.警告,
                        windowtitle: "XHud - 元素生成器消息",
                        title: "复制动效",
                        msg: "请选择动效参数复制模式！",
                        ok: "取消",
                        cancel: "生成",
                        alt: "回收",
                        PrimaryIndex: 0,
                        usemodal: true,
                        themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));

                    if (res == "取消")
                        return;

                    string json = "";

                    switch (res)
                    {
                        case "生成":
                            json = JsonUtility.ToJson(base_script.motion_create_args.Clone());
                            break;
                        case "回收":
                            json = JsonUtility.ToJson(base_script.motion_recycle_args.Clone());
                            break;
                    }

                    XGUI.x_Editor_Data_Set_With_String("xData_motion_args", json);

                    string mode = res == "生成" ? "动效参数 - 生成" : "动效参数 - 回收";

                    XGUI.dialog(
                        type: XGUIDialogType.确认,
                        windowtitle: "XHud - 元素生成器消息",
                        title: "复制动效",
                        msg: $"已复制 \" {mode} \" 到 xData 中！",
                        ok: "明白",
                        PrimaryIndex: 0,
                        usemodal: true,
                        themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                });
                menu.AddItem(new GUIContent("R (粘贴动效)"), false, () =>
                {
                    string buffer = XGUI.x_Editor_Data_Get_With_String("xData_motion_args");
                    if (buffer.Contains("anchor"))
                    {
                        string res = XGUI.dialog(
                            type: XGUIDialogType.帮助,
                            windowtitle: "XHud - 元素生成器消息",
                            title: "粘贴动效",
                            msg: $"检测到动效参数类型为： \"生成动效\"，确定要使用这个参数吗？",
                            ok: "确定",
                            cancel: "暂不",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                        if (res == "暂不")
                            return;

                        Motion_Creator crc = JsonUtility.FromJson<Motion_Creator>(buffer);

                        if (Targets_Selected())
                        {
                            for (int i = 0; i < selected_objects.Length; i++)
                            {
                                selected_objects[i].motion_create_args = crc.Clone();
                            }
                        }
                        else
                            base_script.motion_create_args = crc.Clone();

                        XGUI.dialog(
                            type: XGUIDialogType.确认,
                            windowtitle: "XHud - 元素生成器消息",
                            title: "粘贴动效",
                            msg: $"已更新 \"生成\" 动效参数！",
                            ok: "明白",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                    }
                    else//粘贴回收参数
                    {
                        string res = XGUI.dialog(
                          type: XGUIDialogType.帮助,
                          windowtitle: "XHud - 元素生成器消息",
                          title: "粘贴动效",
                          msg: $"检测到动效参数类型为： \"回收动效\"，确定要使用这个参数吗？",
                          ok: "确定",
                          cancel: "暂不",
                          PrimaryIndex: 0,
                          usemodal: true,
                          themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                        if (res == "暂不")
                            return;

                        Motion_Recycler rec = JsonUtility.FromJson<Motion_Recycler>(buffer);

                        if (Targets_Selected())
                        {
                            for (int i = 0; i < selected_objects.Length; i++)
                            {
                                selected_objects[i].motion_recycle_args = rec.Clone();
                            }
                        }
                        else
                            base_script.motion_recycle_args = rec.Clone();

                        XGUI.dialog(
                           type: XGUIDialogType.确认,
                           windowtitle: "XHud - 元素生成器消息",
                           title: "粘贴动效",
                           msg: $"已更新 \"回收\" 动效参数！",
                           ok: "明白",
                           PrimaryIndex: 0,
                           usemodal: true,
                           themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                    }
                });
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Q (折叠编组)"), false, () =>
                {
                    base_script.GroupFold(true);
                });
                menu.AddItem(new GUIContent("W (展开编组)"), false, () =>
                {
                    base_script.GroupFold(false);
                });
                menu.ShowAsContext();
            }
            #endregion

            #region 源脚本
            base_script.fold_based = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "源脚本",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: base_script.fold_based);
            if (!base_script.fold_based)
            {
                DrawDefaultInspector();
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            serializedObject.ApplyModifiedProperties();
        }
        #endregion     

        #region 工具 - RMS 校验
        /// <summary>
        /// 校验 RMS 名称有效性。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>当 HUD 管理器 RMS 未启用时直接返回。</description></item>
        /// <item><description>当本组件 RMS 未启用时直接返回。</description></item>
        /// <item><description>名称空时取第一个分辨率节点名。</description></item>
        /// </list>
        /// </summary>
        private void CheckRmsNameValid()
        {
            if (!hud_manager.rms_enabled)
                return;
            if (!sp_rms_enabled.boolValue)
                return;
            if (string.IsNullOrEmpty(sp_rms_name.stringValue))
            {
                string[] nodes = hud_manager.hm_RMS_GetResolutionNodeNames();
                if (nodes.Length <= 0)
                    return;
                sp_rms_name.stringValue = nodes[0];
                sp_rms_name.serializedObject.ApplyModifiedProperties();
            }
        }
        #endregion

        #region 动效设置
        /// <summary>
        /// 打开元素动效资源采集器窗口，用于把当前生成或回收动效保存到动效库
        /// </summary>
        /// <param name="Type">
        /// 动效类型（生成动效或回收动效）
        /// </param>
        public void motion_save(HudElementMotionType Type)
        {
            Editor_XHud_LibrarySetTool_Motion window = EditorWindow.GetWindow<Editor_XHud_LibrarySetTool_Motion>(true);

            window.titleContent = new GUIContent("XHud - 元素动效资源采集器");
            XGUI.CenterEditorWindow(new Vector2Int(348, Type == HudElementMotionType.Creator ? 870 : 800), window);

            switch (Type)
            {
                case HudElementMotionType.Creator:
                    window.set_motion(base_script.motion_create_args.Clone());
                    break;
                case HudElementMotionType.Recycler:
                    window.set_motion(base_script.motion_recycle_args.Clone());
                    break;
            }

            window.set_motion_type(Type);
            window.set_library_setter_mode(LibrarySetterMode.添加到库);
            window.set_button_text("添加", "取消");
            window.set_title("元素动效资源采集器");
            window.set_target_motion_library(XHud_Dashboard.get_manager().library_motions);
            //window.ShowModal();
            window.Show();
        }
        /// <summary>
        /// 打开动效资源修改器窗口，用于编辑当前生成或回收动效参数
        /// </summary>
        /// <param name="group">
        /// 待编辑的动效参数分组
        /// </param>
        public void motion_edit(XHud_Arg_MotionGroup group)
        {
            Editor_XHud_LibrarySetTool_Motion window = EditorWindow.GetWindow<Editor_XHud_LibrarySetTool_Motion>(true);

            window.titleContent = new GUIContent("XHud - 动效资源修改器");
            XGUI.CenterEditorWindow(new Vector2Int(348, group.Type == HudElementMotionType.Creator ? 710 : 640), window);
            window.set_motion_type(group.Type);
            window.set_preedit_target_id(base_script.GetInstanceID().ToString());
            window.set_motion(group.Creator, group.Recycler);
            window.set_library_setter_mode(LibrarySetterMode.修改动效参数);
            window.set_button_text("更新");

            string hexcol = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.themes_get_color(ThemeColorType.primary), true);
            window.set_title($"<color={hexcol}>{base_script.gameObject.name} </color>");

            //window.ShowModal();
            window.Show();
        }
        #endregion

        #region 工具 - 批量化操作
        /// <summary>
        /// 获取所有批量脚本目标。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>多选时创建与 <c>targets</c> 等长的数组并逐个填充。</description></item>
        /// <item><description>单选时创建长度为 1 的数组并填充当前目标。</description></item>
        /// </list>
        /// </summary>
        private void Targets_Get()
        {
            if (targets.Length > 1)
            {
                selected_objects = new XHud_Element_Preview[targets.Length];
                for (int i = 0; i < selected_objects.Length; i++)
                {
                    var t = targets[i];
                    selected_objects[i] = (XHud_Element_Preview)t;
                }
            }
            else
            {
                selected_objects = new XHud_Element_Preview[targets.Length];
                selected_objects[0] = (XHud_Element_Preview)target;
            }
        }
        /// <summary>
        /// 判断是否是多选状态。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>当 <c>SelectedObjects</c> 为 null 时返回 false。</description></item>
        /// <item><description>当 <c>SelectedObjects.Length > 1</c> 时返回 true。</description></item>
        /// </list>
        /// </summary>
        /// <returns>是否为多选状态。</returns>
        private bool Targets_Selected()
        {
            if (selected_objects == null)
                return false;
            if (selected_objects.Length > 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 绘制控件
        /// <summary>
        /// 通用方法：绘制开关。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>封装 <c>XGUI.layout_toggle</c> 调用。</description></item>
        /// <item><description>提供标题、宽度、样式、颜色等参数。</description></item>
        /// </list>
        /// </summary>
        private void DrawToggle(string title, SerializedProperty prop, float width, XGUIToggleStyle style = XGUIToggleStyle.实体, Color color_bg_on = default, Color color_bg_off = default, Color color_on = default, Color color_off = default, Action<bool> act_on_changed = null)
        {
            XGUI.layout_toggle(
                title: title,
                title_size: XGUIFontSize.M,
                title_font_style: FontStyle.Normal,
                title_padding: new RectOffset(5, 10, 0, 0),
                title_width: width,
                prop: prop,
                tog_style: style,
                tog_padding: new RectOffset(0, 9, 0, 0),
                tog_margin: new RectOffset(0, 0, 0, 5),
                tog_mixed_options: new string[] { "禁用", "启用" },
                tog_mixed_text_size: XGUIFontSize.M,
                tog_mixed_text_color: Color.black,
                tog_mixed_text_padding: new RectOffset(10, 10, 0, 0),
                tog_mixed_text_anchor: TextAnchor.MiddleCenter,
                tog_mixed_font_style: FontStyle.Normal,
                tog_bg_off_color: color_bg_off,
                tog_bg_on_color: color_bg_on,
                tog_handler_off_color: color_off,
                tog_handler_on_color: color_on,
                tog_mixed_bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                act_on_changed: act_on_changed);
        }
        /// <summary>
        /// 通用方法：绘制参数。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>封装 <c>XGUI.layout_property_field</c> 调用。</description></item>
        /// <item><description>提供标题、宽度、状态色等参数。</description></item>
        /// </list>
        /// </summary>
        private void DrawParamField(string title, SerializedProperty prop, float width, Color status_color)
        {
            XGUI.layout_property_field(
                title: title,
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: width,
                //status_icon: status_color == Color.clear ? null : "icon_field_status",
                //status_icon_color: status_color,
                prop: prop,
                prop_margin: new RectOffset(0, 0, 5, 10));
        }
        #endregion
    }
}