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
    using UnityEngine.UI;

    [CustomEditor(typeof(XHud_TransitionController))]
    public class Editor_XHud_TransitionController : Editor
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
        private XHud_TransitionController base_script;
        /// <summary>
        /// HUD 管理器引用。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>在 <c>OnEnable</c> 中通过 <c>XHud_Dashboard.HudManagerGet()</c> 获取。</description></item>
        /// <item><description>用于访问转场库等全局数据。</description></item>
        /// </list>
        /// </summary>
        private XHud_Manager hud_manager;
        #endregion

        #region 字段 - 序列化属性
        /// <summary>
        /// 转场组件属性
        /// </summary>
        private SerializedProperty sp_transition_image;
        /// <summary>
        /// 转场材质属性
        /// </summary>
        private SerializedProperty sp_transition_mat;
        /// <summary>
        /// 转场模式属性
        /// </summary>
        private SerializedProperty sp_mode;
        /// <summary>
        /// 转场模式开关属性
        /// </summary>
        private SerializedProperty sp_mode_switch;
        /// <summary>
        /// 调试开关属性
        /// </summary>
        private SerializedProperty sp_debug_state;
        /// <summary>
        /// 帧间隔速率属性
        /// </summary>
        private SerializedProperty sp_transition_time;
        /// <summary>
        /// 转场名称属性
        /// </summary>
        private SerializedProperty sp_transition_name;
        /// <summary>
        /// 转场进行中状态属性
        /// </summary>
        private SerializedProperty sp_is_transiting;
        /// <summary>
        /// 转场结束状态属性
        /// </summary>
        private SerializedProperty sp_is_transiting_with_end;
        /// <summary>
        /// 转场开始状态属性
        /// </summary>
        private SerializedProperty sp_is_transiting_with_start;
        /// <summary>
        /// 转场进度属性
        /// </summary>
        private SerializedProperty sp_transition_progress;
        /// <summary>
        /// 当前帧属性
        /// </summary>
        private SerializedProperty sp_current_frame;
        /// <summary>
        /// 当前转场节点属性
        /// </summary>
        private SerializedProperty sp_current_transition_node;
        /// <summary>
        /// 转场播放按键属性
        /// </summary>
        private SerializedProperty sp_transition_play_key;
        /// <summary>
        /// 水平翻转按键属性
        /// </summary>
        private SerializedProperty sp_transition_flip_h_key;
        /// <summary>
        /// 垂直翻转按键属性
        /// </summary>
        private SerializedProperty sp_transition_flip_v_key;
        /// <summary>
        /// 转场修饰按键属性
        /// </summary>
        private SerializedProperty sp_transition_mode_key;
        /// <summary>
        /// 转场叠加颜色属性
        /// </summary>
        private SerializedProperty sp_transition_overlay_color;
        /// <summary>
        /// 转场透明度属性
        /// </summary>
        private SerializedProperty sp_transition_alpha;
        /// <summary>
        /// 接近转场起始帧比例属性
        /// </summary>
        private SerializedProperty sp_limite_frame_per_start;
        /// <summary>
        /// 接近转场结束帧比例属性
        /// </summary>
        private SerializedProperty sp_limite_frame_per_end;
        /// <summary>
        /// 水平翻转开关属性
        /// </summary>
        private SerializedProperty sp_flip_hor;
        /// <summary>
        /// 垂直翻转开关属性
        /// </summary>
        private SerializedProperty sp_flip_ver;
        /// <summary>
        /// 按键控制开关属性
        /// </summary>
        private SerializedProperty sp_use_key_control;
        #endregion

        #region 字段 - 图标
        /// <summary>
        /// 标题主图标
        /// </summary>
        private Texture2D icon_main;
        /// <summary>
        /// 转场状态图标
        /// </summary>
        private Texture2D trans_state;
        /// <summary>
        /// 转场首帧图标
        /// </summary>
        private Texture2D trans_first_frame;
        /// <summary>
        /// 转场尾帧图标
        /// </summary>
        private Texture2D trans_end_frame;
        /// <summary>
        /// 打开库按钮常态图标
        /// </summary>
        private Texture2D openlib_r;
        /// <summary>
        /// 打开库按钮按下图标
        /// </summary>
        private Texture2D openlib_p;
        /// <summary>
        /// 刷新按钮常态图标
        /// </summary>
        private Texture2D update_r;
        /// <summary>
        /// 刷新按钮按下图标
        /// </summary>
        private Texture2D update_p;
        /// <summary>
        /// 定位按钮常态图标
        /// </summary>
        private Texture2D locate_r;
        /// <summary>
        /// 定位按钮按下图标
        /// </summary>
        private Texture2D locate_p;
        #endregion

        #region 字段 - 选项文字
        /// <summary>
        /// 启用开关选项文字。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>索引 0：关闭。</description></item>
        /// <item><description>索引 1：开启。</description></item>
        /// </list>
        /// </summary>
        string[] stroptions_enabled = new string[2] { "关闭", "开启" };
        /// <summary>
        /// 调试开关选项文字。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>索引 0：关闭。</description></item>
        /// <item><description>索引 1：调试。</description></item>
        /// </list>
        /// </summary>
        string[] stroptions_debug = new string[2] { "关闭", "调试" };
        /// <summary>
        /// 转场模式选项文字。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>索引 0：入场。</description></item>
        /// <item><description>索引 1：出场。</description></item>
        /// </list>
        /// </summary>
        string[] stroptions_transmode = new string[] { "入场", "出场" };
        /// <summary>
        /// 翻转选项文字。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>索引 0：常规。</description></item>
        /// <item><description>索引 1：翻转。</description></item>
        /// </list>
        /// </summary>
        string[] stroptions_flip = new string[] { "常规", "翻转" };
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
        XHud_TransitionController[] selected_objects;
        #endregion

        #region 生命周期    
        void OnEnable()
        {
            base_script = (XHud_TransitionController)target;

            #region 获取序列化属性
            sp_transition_image = serializedObject.FindProperty(nameof(XHud_TransitionController.transition_image));
            sp_transition_mat = serializedObject.FindProperty(nameof(XHud_TransitionController.transition_mat));
            sp_mode = serializedObject.FindProperty(nameof(XHud_TransitionController.mode));
            sp_mode_switch = serializedObject.FindProperty(nameof(XHud_TransitionController.mode_switch));
            sp_transition_time = serializedObject.FindProperty(nameof(XHud_TransitionController.transition_time));
            sp_transition_name = serializedObject.FindProperty(nameof(XHud_TransitionController.transition_name));
            sp_is_transiting = serializedObject.FindProperty(nameof(XHud_TransitionController.is_transiting));
            sp_transition_progress = serializedObject.FindProperty(nameof(XHud_TransitionController.transition_progress));
            sp_current_frame = serializedObject.FindProperty(nameof(XHud_TransitionController.current_frame));
            sp_current_transition_node = serializedObject.FindProperty(nameof(XHud_TransitionController.current_transition_node));
            sp_transition_play_key = serializedObject.FindProperty(nameof(XHud_TransitionController.transition_play_key));
            sp_transition_flip_h_key = serializedObject.FindProperty(nameof(XHud_TransitionController.transition_flip_h_key));
            sp_transition_flip_v_key = serializedObject.FindProperty(nameof(XHud_TransitionController.transition_flip_v_key));
            sp_transition_mode_key = serializedObject.FindProperty(nameof(XHud_TransitionController.transition_mode_key));
            sp_limite_frame_per_start = serializedObject.FindProperty(nameof(XHud_TransitionController.limite_frame_per_start));
            sp_limite_frame_per_end = serializedObject.FindProperty(nameof(XHud_TransitionController.limite_frame_per_end));
            sp_is_transiting_with_end = serializedObject.FindProperty(nameof(XHud_TransitionController.is_transiting_with_end));
            sp_is_transiting_with_start = serializedObject.FindProperty(nameof(XHud_TransitionController.is_transiting_with_start));
            sp_debug_state = serializedObject.FindProperty(nameof(XHud_TransitionController.debug_state));
            sp_transition_overlay_color = serializedObject.FindProperty(nameof(XHud_TransitionController.transition_overlay_color));
            sp_transition_alpha = serializedObject.FindProperty(nameof(XHud_TransitionController.transition_alpha));
            sp_flip_hor = serializedObject.FindProperty(nameof(XHud_TransitionController.flip_hor));
            sp_flip_ver = serializedObject.FindProperty(nameof(XHud_TransitionController.flip_ver));
            sp_use_key_control = serializedObject.FindProperty(nameof(XHud_TransitionController.use_key_control));
            hud_manager = XHud_Dashboard.get_manager();
            #endregion

            #region 获取图标
            icon_main = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_transition_controller/icon_main");
            trans_state = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_transition_controller/trans_state");
            trans_first_frame = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_transition_controller/trans_first_frame");
            trans_end_frame = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_transition_controller/trans_end_frame");
            openlib_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_transition_controller/openlib_r");
            openlib_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_transition_controller/openlib_p");
            update_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_transition_controller/update_r");
            update_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_transition_controller/update_p");
            locate_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_transition_controller/locate_r");
            locate_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_transition_controller/locate_p");
            #endregion

            Targets_Get();

            if ((Image)sp_transition_image.objectReferenceValue == null)
            {
                sp_transition_image.objectReferenceValue = base_script.GetComponent<Image>();
                sp_transition_image.serializedObject.ApplyModifiedProperties();
            }
            if (sp_transition_mat.objectReferenceValue == null)
            {
                sp_transition_mat.objectReferenceValue = (Material)AssetDatabase.LoadAssetAtPath($"{XHud_Dashboard.Get_Path_XHUD_MATERIALS_Path()}Transitions/Transition.mat", typeof(Material));
            }
            if (sp_transition_mat.objectReferenceValue != null)
            {
                Material mat = sp_transition_mat.objectReferenceValue as Material;
                sp_transition_overlay_color.colorValue = mat.color;
                sp_transition_overlay_color.serializedObject.ApplyModifiedProperties();
            }
            if (string.IsNullOrEmpty(sp_transition_name.stringValue))
            {
                if (hud_manager.library_transitions != null && !hud_manager.library_transitions.TransitionLibrary_IsEmpty())
                {
                    sp_transition_name.stringValue = hud_manager.library_transitions.TransitionLibrary_GetAllNames()[0];
                    sp_transition_name.serializedObject.ApplyModifiedProperties();
                    base_script.current_transition_node = hud_manager.library_transitions.TransitionLibrary_Get(sp_transition_name.stringValue);
                }
            }
        }

        void OnDisable()
        {

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            TextClipping clipping_ellipsis = XGUI.TryEllipsisClipping();

            #region 标题
            XGUI.layout_banner(
             bg_fill: XGUIGroup.实体,
             bg_color: XGUIColor.深空灰,
             bg_height: 30,
             icon: icon_main,
             icon_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
             title_text: "XHud  -  转场控制器",
             title_anchor: TextAnchor.MiddleLeft,
             title_style: FontStyle.Normal,
             title_color: Color.white,
             title_size: XGUIFontSize.B,
             title_clipping: clipping_ellipsis,
             bg_margin: new RectOffset(0, 0, 5, 5));
            #endregion

            XGUI.layout_space(5);

            #region 快捷功能
            XGUI.layout_group_start(
                type: XGUIContainerType.Horizontal,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "快捷功能",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(15, 15, 20, 15));

            #region 查看当前转场库
            if (XGUI.layout_button(
                tooltip: "查看当前转场库",
                tex_release: openlib_r,
                tex_press: openlib_p,
                tex_gui_color: Color.white,
                border: new RectOffset(0, 0, 0, 0),
                width: 14,
                height: 14))
            {
                Editor_XHud_MenuItemsAction_OpenLibrary.open_transition();
            }
            #endregion

            GUILayout.FlexibleSpace();

            #region 快速刷新转场资源
            if (XGUI.layout_button(
                tooltip: "快速刷新转场资源",
                tex_release: update_r,
                tex_press: update_p,
                tex_gui_color: Color.white,
                border: new RectOffset(0, 0, 0, 0),
                width: 14,
                height: 14))
            {
                if (hud_manager.library_transitions == null)
                    return;
                if (hud_manager.library_transitions.TransitionLibrary_IsEmpty())
                    return;
                SerializedProperty sp_Name = sp_current_transition_node.FindPropertyRelative("Name");
                SerializedProperty sp_TotalFramesCount = sp_current_transition_node.FindPropertyRelative("TotalFramesCount");
                SerializedProperty sp_LastFrameIndex = sp_current_transition_node.FindPropertyRelative("LastFrameIndex");
                SerializedProperty sp_SkipFrame = sp_current_transition_node.FindPropertyRelative("SkipFrame");
                SerializedProperty sp_Res = sp_current_transition_node.FindPropertyRelative("Res");
                SerializedProperty sp_x_frames = sp_current_transition_node.FindPropertyRelative("Frames");
                XHud_LibraryArg_Transition node = hud_manager.library_transitions.TransitionLibrary_Get(sp_transition_name.stringValue);
                sp_Name.stringValue = node.name;
                sp_TotalFramesCount.intValue = node.total_frames_count;
                sp_LastFrameIndex.intValue = node.last_frame_index;
                sp_SkipFrame.intValue = node.skip_frame;
                sp_Res.vector2IntValue = node.res;
                sp_x_frames.ClearArray();
                for (int i = 0; i < node.frames.Count; i++)
                {
                    sp_x_frames.InsertArrayElementAtIndex(i);
                    sp_x_frames.GetArrayElementAtIndex(i).objectReferenceValue = node.frames[i];
                }
                sp_transition_progress.floatValue = 0;
                sp_transition_progress.serializedObject.ApplyModifiedProperties();
                UpdateTransition(sp_x_frames, sp_LastFrameIndex);
                return;
            }
            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
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
                DrawToggle("按键控制", sp_use_key_control, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, (b) => { });
                DrawToggle("转场模式 (入场 & 出场)", sp_mode_switch, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, (b) =>
                {
                    if (sp_mode_switch.boolValue)
                        sp_mode.enumValueIndex = (int)TransitionMode.出场;
                    else
                        sp_mode.enumValueIndex = (int)TransitionMode.入场;
                    sp_mode.serializedObject.ApplyModifiedProperties();
                    sp_transition_progress.floatValue = 0;
                    sp_transition_progress.serializedObject.ApplyModifiedProperties();
                });
                DrawToggle("水平翻转", sp_flip_hor, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, (b) =>
                {
                    Material mat = (Material)sp_transition_mat.objectReferenceValue;
                    mat.SetInt("_Flip_Hor", sp_flip_hor.boolValue ? 1 : 0);
                    sp_transition_mat.serializedObject.ApplyModifiedProperties();
                });
                DrawToggle("垂直翻转", sp_flip_ver, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, (b) =>
                {
                    Material mat = (Material)sp_transition_mat.objectReferenceValue;
                    mat.SetInt("_Flip_Ver", sp_flip_ver.boolValue ? 1 : 0);
                    sp_transition_mat.serializedObject.ApplyModifiedProperties();
                });
                DrawToggle("状态调试", sp_debug_state, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, (b) => { });

                XGUI.layout_group_start(
                    type: XGUIContainerType.Horizontal,
                    absolute_margin: true,
                    absolute_padding: true,
                    padding: new RectOffset(0, 0, 0, 0),
                    margin: new RectOffset(0, 0, 0, 0));
                string[] TransLibNames = hud_manager.library_transitions.TransitionLibrary_GetAllNames();
                XGUI.layout_string_popup(
                    title: "转场资源库",
                    title_width: 120,
                    title_size: XGUIFontSize.M,
                    title_anchor: TextAnchor.MiddleLeft,
                    prop: sp_transition_name,
                    options: TransLibNames,
                    opt_text_size: XGUIFontSize.M,
                    opt_text_color: Color.black,
                    opt_text_padding: new RectOffset(10, 10, 0, 0),
                    opt_anchor: TextAnchor.MiddleLeft,
                    opt_font_style: FontStyle.Normal,
                    opt_bg_fill: XGUIGroup.实体,
                    opt_bg_color: XGUIColor.亮白,
                    opt_bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    margin: new RectOffset(0, 0, 5, 5),
                    padding: new RectOffset(5, 5, 0, 0),
                    title_margin: new RectOffset(0, 0, 0, 0),
                    icon_arrow_color: Color.black,
                    act_on_changed: (d) =>
                    {
                        SerializedProperty sp_Name = sp_current_transition_node.FindPropertyRelative("Name");
                        SerializedProperty sp_TotalFramesCount = sp_current_transition_node.FindPropertyRelative("TotalFramesCount");
                        SerializedProperty sp_LastFrameIndex = sp_current_transition_node.FindPropertyRelative("LastFrameIndex");
                        SerializedProperty sp_SkipFrame = sp_current_transition_node.FindPropertyRelative("SkipFrame");
                        SerializedProperty sp_Res = sp_current_transition_node.FindPropertyRelative("Res");
                        SerializedProperty sp_x_frames = sp_current_transition_node.FindPropertyRelative("Frames");
                        XHud_LibraryArg_Transition node = hud_manager.library_transitions.TransitionLibrary_Get(d);
                        sp_Name.stringValue = node.name;
                        sp_TotalFramesCount.intValue = node.total_frames_count;
                        sp_LastFrameIndex.intValue = node.last_frame_index;
                        sp_SkipFrame.intValue = node.skip_frame;
                        sp_Res.vector2IntValue = node.res;
                        sp_x_frames.ClearArray();
                        for (int i = 0; i < node.frames.Count; i++)
                        {
                            sp_x_frames.InsertArrayElementAtIndex(i);
                            sp_x_frames.GetArrayElementAtIndex(i).objectReferenceValue = node.frames[i];
                        }
                        sp_transition_progress.floatValue = 0;
                        UpdateTransition(sp_x_frames, sp_LastFrameIndex);
                    });
                if (!Targets_Selected())
                {
                    XGUI.layout_space(10);
                    if (XGUI.layout_button(
                        tooltip: "定位到转场库",
                        tex_release: locate_r,
                        tex_press: locate_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        margin: new RectOffset(0, 15, 6, 0),
                        width: 14,
                        height: 14))
                    {
                        //获取目标转场的索引号
                        int index = 0;
                        for (int k = 0; k < TransLibNames.Length; k++)
                        {
                            if (TransLibNames[k] == sp_transition_name.stringValue)
                            {
                                index = k;
                                break;
                            }
                        }
                        //定位到元素库中的对应当前资源
                        //打开目标转场库
                        Editor_XHud_MenuItemsAction_OpenLibrary.open_transition();
                        hud_manager.library_transitions.TransitionLibrary_Location(TransLibNames[index]);
                    }
                }
                XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
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
                #region 转场状态
                XGUI.layout_state_displayer_text(
                    title: "转场状态",
                    title_size: XGUIFontSize.M,
                    subtitle: sp_is_transiting.boolValue ? "正在转场中" : "等待转场",
                    subtitle_size: XGUIFontSize.M,
                    subtitle_color: sp_is_transiting.boolValue ? XHud_Dashboard.themes_get_color(ThemeColorType.primary) : Color.gray,
                    margin: new RectOffset(5, 5, 0, 5));
                #endregion

                #region 接近首帧状态
                SerializedProperty sp_frames_co = sp_current_transition_node.FindPropertyRelative("Frames");
                int frameLimite_start = (int)Mathf.Floor(sp_frames_co.arraySize * sp_limite_frame_per_start.floatValue);
                XGUI.layout_state_displayer_text(
                    title: "接近首帧状态",
                    title_size: XGUIFontSize.M,
                    subtitle: sp_is_transiting_with_start.boolValue ? $"接近首帧状态 ({frameLimite_start})" : $"等待转场 ({frameLimite_start})",
                    subtitle_size: XGUIFontSize.M,
                    subtitle_color: sp_is_transiting_with_start.boolValue ? XHud_Dashboard.themes_get_color(ThemeColorType.primary) : Color.gray,
                    margin: new RectOffset(5, 5, 0, 5));
                #endregion

                #region 接近尾帧状态
                int frameLimite_end = (int)Mathf.Floor(sp_frames_co.arraySize * sp_limite_frame_per_end.floatValue);
                XGUI.layout_state_displayer_text(
                    title: "接近尾帧状态",
                    title_size: XGUIFontSize.M,
                    subtitle: sp_is_transiting_with_end.boolValue ? $"接近尾帧状态 ({frameLimite_end})" : $"等待转场 ({frameLimite_end})",
                    subtitle_size: XGUIFontSize.M,
                    subtitle_color: sp_is_transiting_with_end.boolValue ? XHud_Dashboard.themes_get_color(ThemeColorType.primary) : Color.gray,
                    margin: new RectOffset(5, 5, 0, 5));
                #endregion
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 参数
            base_script.fold_params = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "参数",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(5, 10, 15, 15),
                foldout: base_script.fold_params);
            if (!base_script.fold_params)
            {
                #region 转场组件
                XGUI.layout_property_field(
                    title: "转场组件",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: sp_transition_image,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                #region 转场材质
                EditorGUI.BeginChangeCheck();
                XGUI.layout_property_field(
                  title: "转场材质",
                  title_size: XGUIFontSize.M,
                  title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                  title_width: 90,
                  prop: sp_transition_mat,
                  prop_margin: new RectOffset(0, 0, 5, 0));
                if (EditorGUI.EndChangeCheck())
                {
                    Image img = (Image)sp_transition_image.objectReferenceValue;
                    img.material = (Material)sp_transition_mat.objectReferenceValue;
                    sp_transition_image.serializedObject.ApplyModifiedProperties();
                }
                #endregion

                #region 转场叠加颜色
                EditorGUI.BeginChangeCheck();
                XGUI.layout_property_field(
                  title: "转场叠加颜色",
                  title_size: XGUIFontSize.M,
                  title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                  title_width: 90,
                  prop: sp_transition_overlay_color,
                  prop_margin: new RectOffset(0, 0, 5, 0));
                if (EditorGUI.EndChangeCheck())
                {
                    Image img = (Image)sp_transition_image.objectReferenceValue;
                    img.color = Color.white;
                    img.material = (Material)sp_transition_mat.objectReferenceValue;
                    img.material.SetColor("_Color", sp_transition_overlay_color.colorValue);
                    sp_transition_image.serializedObject.ApplyModifiedProperties();
                }
                #endregion

                #region 转场参数
                XGUI.layout_property_field(
                    title: "转场参数",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: sp_current_transition_node,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                #region 转场进度
                SerializedProperty sp_frames = sp_current_transition_node.FindPropertyRelative("Frames");
                SerializedProperty sp_lastframeindex = sp_current_transition_node.FindPropertyRelative("LastFrameIndex");
                if (sp_frames != null && sp_frames.arraySize > 0)
                {
                    EditorGUI.BeginChangeCheck();
                    XGUI.layout_property_field(
                        title: "转场进度",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: sp_transition_progress,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    if (EditorGUI.EndChangeCheck())
                    {
                        UpdateTransition(sp_frames, sp_lastframeindex);
                    }
                }
                #endregion

                #region 转场透明度            
                EditorGUI.BeginChangeCheck();
                sp_transition_alpha.floatValue = XGUI.layout_slider(
                    title: "透明度",
                    prop: sp_transition_alpha,
                    left: 0,
                    right: 1,
                    title_width: 100,
                    title_size: XGUIFontSize.M,
                    title_anchor: TextAnchor.MiddleLeft,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                sp_transition_alpha.serializedObject.ApplyModifiedProperties();
                if (EditorGUI.EndChangeCheck())
                {
                    base_script.Transition_Set_Alpha(sp_transition_alpha.floatValue);
                    if (sp_transition_mat.objectReferenceValue != null)
                    {
                        Material mat = sp_transition_mat.objectReferenceValue as Material;
                        mat.SetFloat("_Alpha", sp_transition_alpha.floatValue);
                        sp_transition_overlay_color.serializedObject.ApplyModifiedProperties();
                    }
                }
                #endregion

                XGUI.layout_seperator(
                    thickness: 1,
                    color: Color.white * 0.5f,
                    margin: new RectOffset(15, 30, 15, 15));

                #region 当前帧
                XGUI.layout_property_field(
                    title: "当前帧",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: sp_current_frame,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                #region 帧间隔速率
                XGUI.layout_property_field(
                    title: "帧间隔速率",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: sp_transition_time,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                XGUI.layout_seperator(
                    thickness: 1,
                    color: Color.white * 0.5f,
                    margin: new RectOffset(15, 30, 15, 15));

                #region 接近开始帧
                xgui_minmax_value minmax = XGUI.layout_slider_min_max(
                 title: "接近转场",
                 ref_min: ref base_script.limite_frame_per_start,
                 ref_max: ref base_script.limite_frame_per_end,
                 min_limite: 0,
                 max_limite: 1,
                 title_width: 100,
                 title_size: XGUIFontSize.M,
                 title_anchor: TextAnchor.MiddleLeft,
                 min_field_color: Color.white,
                 max_field_color: Color.white,
                 shrink_text_color: Color.white * 0.65f,
                 control_limite: 210,
                 prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 控制按键
            base_script.fold_key = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "控制按键",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(5, 10, 15, 15),
                foldout: base_script.fold_key);
            if (!base_script.fold_key)
            {
                #region 转场模式
                XGUI.layout_property_field(
                    title: "转场模式",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: sp_transition_mode_key,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                #region 按键转场
                XGUI.layout_property_field(
                    title: "按键转场",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: sp_transition_play_key,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                #region 水平翻转
                XGUI.layout_property_field(
                    title: "水平翻转",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: sp_transition_flip_h_key,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                #region 垂直翻转
                XGUI.layout_property_field(
                    title: "垂直翻转",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: sp_transition_flip_v_key,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 弹出菜单
            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                // 创建右键菜单
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Q (折叠编组)"), false, () =>
                {
                    base_script.GroupFold(true);
                });
                menu.AddItem(new GUIContent("W (展开编组)"), false, () =>
                {
                    base_script.GroupFold(false);
                });
                menu.ShowAsContext();

                Event.current.Use();
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

        #region 工具 - 转场更新
        /// <summary>
        /// 更新转场数据。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>根据进度计算当前帧。</description></item>
        /// <item><description>按模式设置通道反转。</description></item>
        /// <item><description>设置当前帧的遮罩贴图。</description></item>
        /// </list>
        /// </summary>
        /// <param name="sp_frames">帧贴图数组属性。</param>
        /// <param name="sp_lastframeindex">最后一帧索引属性。</param>
        private void UpdateTransition(SerializedProperty sp_frames, SerializedProperty sp_lastframeindex)
        {
            TransitionMode mode = (TransitionMode)sp_mode.enumValueIndex;
            sp_current_frame.intValue = (int)(sp_transition_progress.floatValue * sp_lastframeindex.intValue);
            if (mode == TransitionMode.入场)
            {
                base_script.Transition_Set_ChannelInvert(false);
            }
            else
            {
                base_script.Transition_Set_ChannelInvert(true);
            }
            if (sp_frames.arraySize > 0)
            {
                Texture2D tex = (Texture2D)sp_frames.GetArrayElementAtIndex(sp_current_frame.intValue).objectReferenceValue;
                base_script.Transition_Set_MaskTexture(tex);
            }
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
                selected_objects = new XHud_TransitionController[targets.Length];
                for (int i = 0; i < selected_objects.Length; i++)
                {
                    var t = targets[i];
                    selected_objects[i] = (XHud_TransitionController)t;
                }
            }
            else
            {
                selected_objects = new XHud_TransitionController[targets.Length];
                selected_objects[0] = (XHud_TransitionController)target;
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
        #endregion
    }
}