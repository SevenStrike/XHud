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

    [CanEditMultipleObjects]
    [CustomEditor(typeof(XHud_Spawner))]
    public class Editor_XHud_Spawner : Editor
    {
        #region 组件引用
        /// <summary>
        /// 当前正在编辑的 XHud_Spawner 实例（单选时的主目标）
        /// </summary>
        private XHud_Spawner base_script;
        #endregion

        #region 序列化属性
        /// <summary>
        /// 可见性创建与回收开关
        /// </summary>
        private SerializedProperty sp_opt_create_visual;
        /// <summary>
        /// 手动创建与回收开关
        /// </summary>
        private SerializedProperty sp_opt_create_manully;
        /// <summary>
        /// 是否使用世界空间
        /// </summary>
        private SerializedProperty sp_opt_create_world;
        /// <summary>
        /// 生成器是否处于生成中状态
        /// </summary>
        private SerializedProperty sp_m_opt_is_spawing;
        /// <summary>
        /// RMS 布局模式开关
        /// </summary>
        private SerializedProperty sp_opt_rms_mode;
        /// <summary>
        /// 元素自动播放开关
        /// </summary>
        private SerializedProperty sp_opt_auto_in;
        /// <summary>
        /// 安全保护机制开关
        /// </summary>
        private SerializedProperty sp_opt_protected_spawn_action;
        /// <summary>
        /// 是否使用元素自身动效
        /// </summary>
        private SerializedProperty sp_opt_use_element_motion;
        /// <summary>
        /// 目标元素库名称
        /// </summary>
        private SerializedProperty sp_target_library_name;
        /// <summary>
        /// 目标元素名称
        /// </summary>
        private SerializedProperty sp_target_spawn_name;
        /// <summary>
        /// 目标元素标识
        /// </summary>
        private SerializedProperty sp_target_spawn_indicator;
        /// <summary>
        /// 生成器标识
        /// </summary>
        private SerializedProperty sp_spawner_indicator;
        /// <summary>
        /// RMS 方案索引名称
        /// </summary>
        private SerializedProperty sp_rms_index_name;
        /// <summary>
        /// 预设元素尺寸
        /// </summary>
        private SerializedProperty sp_preset_element_size;
        /// <summary>
        /// 预设元素平面偏移
        /// </summary>
        private SerializedProperty sp_preset_element_offset;
        /// <summary>
        /// 预设元素平面缩放
        /// </summary>
        private SerializedProperty sp_preset_element_scale;
        /// <summary>
        /// 预设元素世界坐标位置
        /// </summary>
        private SerializedProperty sp_preset_element_world_position;
        /// <summary>
        /// 预设元素世界坐标旋转
        /// </summary>
        private SerializedProperty sp_preset_element_world_rotation;
        /// <summary>
        /// 预设元素世界坐标缩放
        /// </summary>
        private SerializedProperty sp_preset_element_world_scale;
        /// <summary>
        /// 预设元素参考物体
        /// </summary>
        private SerializedProperty sp_preset_element_referobject;
        /// <summary>
        /// 生成按键
        /// </summary>
        private SerializedProperty sp_spawn_key_create;
        /// <summary>
        /// 回收按键
        /// </summary>
        private SerializedProperty sp_spawn_key_recycle;
        /// <summary>
        /// 主要辅助按键
        /// </summary>
        private SerializedProperty sp_spawn_key_primary;
        /// <summary>
        /// 次级辅助按键
        /// </summary>
        private SerializedProperty sp_spawn_key_secondary;
        /// <summary>
        /// 生成动效库索引名称
        /// </summary>
        private SerializedProperty sp_motion_create_library_index_name;
        /// <summary>
        /// 回收动效库索引名称
        /// </summary>
        private SerializedProperty sp_motion_recycle_library_index_name;
        #endregion

        #region 选项文字
        /// <summary>
        /// 开关类选项的文字：「关闭 / 开启」
        /// </summary>
        string[] stroptions_enabled = new string[2] { "关闭", "开启" };
        /// <summary>
        /// 空间模式选项的文字：「平面 / 世界」
        /// </summary>
        string[] stroptions_world = new string[2] { "平面", "世界" };
        /// <summary>
        /// 保护机制选项的文字：「开放 / 保护」
        /// </summary>
        string[] stroptions_protecte = new string[2] { "开放", "保护" };
        #endregion

        #region 图标
        /// <summary>
        /// 面板主图标
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

        #region 批量化操作
        /// <summary>
        /// 当前选中的全部 XHud_Spawner 实例，用于批量操作
        /// </summary>
        XHud_Spawner[] selected_objects;
        /// <summary>
        /// 从 Editor.targets 收集全部选中对象并填充到 SelectedObjects
        /// </summary>
        private void Targets_Get()
        {
            if (targets.Length > 1)
            {
                selected_objects = new XHud_Spawner[targets.Length];
                for (int i = 0; i < selected_objects.Length; i++)
                {
                    var t = targets[i];
                    selected_objects[i] = (XHud_Spawner)t;
                }
            }
            else
            {
                selected_objects = new XHud_Spawner[targets.Length];
                selected_objects[0] = (XHud_Spawner)target;
            }
        }
        /// <summary>
        /// 判断当前是否处于多选（批量）状态
        /// </summary>
        /// <returns>
        /// 选中对象数量大于 1 时返回 true；否则返回 false
        /// </returns>
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

        #region 生命周期
        /// <summary>
        /// Inspector 启用时调用：加载图标、缓存序列化属性、收集选中对象，并在首次加载时补全空的库名与元素名
        /// </summary>
        private void OnEnable()
        {
            base_script = (XHud_Spawner)target;

            #region 获取序列化属性
            sp_target_library_name = serializedObject.FindProperty(nameof(XHud_Spawner.target_library_name));
            sp_target_spawn_name = serializedObject.FindProperty(nameof(XHud_Spawner.target_spawn_name));
            sp_target_spawn_indicator = serializedObject.FindProperty(nameof(XHud_Spawner.target_spawn_indicator));
            sp_spawner_indicator = serializedObject.FindProperty(nameof(XHud_Spawner.spawner_indicator));

            sp_preset_element_size = serializedObject.FindProperty(nameof(XHud_Spawner.preset_element_size));
            sp_preset_element_offset = serializedObject.FindProperty(nameof(XHud_Spawner.preset_element_offset));
            sp_preset_element_scale = serializedObject.FindProperty(nameof(XHud_Spawner.preset_element_scale));
            sp_preset_element_world_position = serializedObject.FindProperty(nameof(XHud_Spawner.preset_element_world_position));
            sp_preset_element_world_rotation = serializedObject.FindProperty(nameof(XHud_Spawner.preset_element_world_rotation));
            sp_preset_element_world_scale = serializedObject.FindProperty(nameof(XHud_Spawner.preset_element_world_scale));
            sp_preset_element_referobject = serializedObject.FindProperty(nameof(XHud_Spawner.preset_element_referobject));

            sp_spawn_key_primary = serializedObject.FindProperty(nameof(XHud_Spawner.spawn_key_primary));
            sp_spawn_key_secondary = serializedObject.FindProperty(nameof(XHud_Spawner.spawn_key_secondary));
            sp_spawn_key_create = serializedObject.FindProperty(nameof(XHud_Spawner.spawn_key_create));
            sp_spawn_key_recycle = serializedObject.FindProperty(nameof(XHud_Spawner.spawn_key_recycle));

            sp_opt_create_manully = serializedObject.FindProperty(nameof(XHud_Spawner.opt_create_manully));
            sp_opt_create_visual = serializedObject.FindProperty(nameof(XHud_Spawner.opt_create_visual));
            sp_opt_create_world = serializedObject.FindProperty(nameof(XHud_Spawner.opt_create_world));
            sp_opt_rms_mode = serializedObject.FindProperty(nameof(XHud_Spawner.opt_rms_mode));
            sp_opt_auto_in = serializedObject.FindProperty(nameof(XHud_Spawner.opt_auto_in));
            sp_opt_use_element_motion = serializedObject.FindProperty(nameof(XHud_Spawner.opt_use_element_motion));

            sp_m_opt_is_spawing = serializedObject.FindProperty(nameof(XHud_Spawner.editor_serializefield_m_opt_is_spawing));

            sp_motion_create_library_index_name = serializedObject.FindProperty(nameof(XHud_Spawner.motion_create_library_index_name));
            sp_motion_recycle_library_index_name = serializedObject.FindProperty(nameof(XHud_Spawner.motion_recycle_library_index_name));
            sp_rms_index_name = serializedObject.FindProperty(nameof(XHud_Spawner.rms_index_name));

            sp_opt_protected_spawn_action = serializedObject.FindProperty(nameof(XHud_Spawner.opt_protected_spawn_action));
            #endregion

            #region 获取图标
            icon_main = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_spawner/icon_main");
            save_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_spawner/save_r");
            save_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_spawner/save_p");
            locate_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_spawner/locate_r");
            locate_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_spawner/locate_p");
            reset_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_spawner/reset_r");
            reset_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_spawner/reset_p");
            edit_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_spawner/edit_r");
            edit_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_spawner/edit_p");
            #endregion

            Targets_Get();

            #region 用于第一次加载脚本判断库名和元素名是否是空的

            XHud_Manager mgr = XHud_Dashboard.get_manager();
            if (!Application.isPlaying && mgr != null)
            {
                //获取所有元素库名称
                string[] LibNames = mgr.hm_ElementLibrary_GetAllLibraryNames();
                //如果不是空的
                if (LibNames.Length > 0)
                {
                    //判断如果库名是空的就把首个元素库名给他
                    if (string.IsNullOrEmpty(sp_target_library_name.stringValue))
                    {
                        sp_target_library_name.stringValue = LibNames[0];
                        sp_target_library_name.serializedObject.ApplyModifiedProperties();
                    }
                    //判断元素名是否是空的
                    if (string.IsNullOrEmpty(sp_target_spawn_name.stringValue))
                    {
                        //获取指定库名的元素库
                        XHud_Library_Element lib = mgr.hm_ElementLibrary_GetTargetLibrary(sp_target_library_name.stringValue);
                        //如果元素库不是空的
                        if (lib != null)
                        {
                            //获取指定库名的元素库的所有元素名称
                            string[] ItemNames = lib.ElementsLibrary_GetAllElementsNames();
                            //如果目标元素库不是空的就把首个元素名给他
                            if (ItemNames.Length > 0)
                            {
                                sp_target_spawn_name.stringValue = ItemNames[0];
                                sp_target_spawn_name.serializedObject.ApplyModifiedProperties();

                                sp_target_spawn_indicator.stringValue = "Indicator_" + sp_target_spawn_name.stringValue;
                                sp_target_spawn_indicator.serializedObject.ApplyModifiedProperties();
                            }
                        }
                    }
                }
            }
            #endregion
        }
        /// <summary>
        /// Inspector 禁用时调用（当前无额外清理逻辑）
        /// </summary>
        private void OnDisable()
        {

        }
        #endregion

        #region Inspector 绘制
        /// <summary>
        /// 绘制元素生成器的完整 Inspector 面板
        /// </summary>
        public override void OnInspectorGUI()
        {
            XHud_Manager mgr = XHud_Dashboard.get_manager();

            serializedObject.Update();

            TextClipping clipping_ellipsis = XGUI.TryEllipsisClipping();

            #region 标题
            XGUI.layout_banner(
                bg_fill: XGUIGroup.实体,
                bg_color: XGUIColor.深空灰,
                bg_height: 30,
                icon: icon_main,
                icon_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_text: "XHud  -  元素生成器",
                title_anchor: TextAnchor.MiddleLeft,
                title_style: FontStyle.Normal,
                title_color: Color.white,
                title_size: XGUIFontSize.B,
                title_clipping: clipping_ellipsis,
                bg_margin: new RectOffset(0, 0, 5, 5));
            #endregion

            #region 目标
            base_script.fold_target = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "目标",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: base_script.fold_target);

            if (!base_script.fold_target)
            {
                if (mgr == null)
                {
                    XGUI.layout_label(
                        text: "未找到任何 XHud 管理器!",
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
                    if (mgr.ele_library_elements == null || mgr.ele_library_elements.Count <= 0)
                    {
                        XGUI.layout_label(
                        text: "未找到任何已指定的元素库，请先添加元素库项!",
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
                        string[] libsname = mgr.hm_ElementLibrary_GetAllLibraryNames();
                        if (!mgr.hm_ElementLibrary_IsExist(sp_target_library_name.stringValue))
                        {
                            sp_target_library_name.stringValue = mgr.hm_ElementLibrary_GetFirstLibrary().library_name;
                        }
                        XGUI.layout_string_popup(
                            title: "目标库",
                            title_width: 60,
                            title_size: XGUIFontSize.M,
                            title_anchor: TextAnchor.MiddleLeft,
                            prop: sp_target_library_name,
                            options: libsname,
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
                                sp_target_library_name.stringValue = d;
                                sp_target_library_name.serializedObject.ApplyModifiedProperties();

                                //如果切换库名后之前指定的目标元素不存在则使用切换后的库的首个元素名称来指定元素
                                if (!mgr.hm_ElementLibrary_GetTargetLibrary(d).ElementLibrary_IsExist(sp_target_spawn_name.stringValue))
                                {
                                    sp_target_spawn_name.stringValue = mgr.hm_ElementLibrary_GetTargetLibrary(d).library_elements[0].name;
                                    sp_target_spawn_name.serializedObject.ApplyModifiedProperties();
                                }
                            });

                        string[] ele_names = mgr.hm_ElementLibrary_GetTargetLibrary(sp_target_library_name.stringValue).ElementsLibrary_GetAllElementsNames();
                        XGUI.layout_string_popup(
                            title: "目标元素",
                            title_width: 60,
                            title_size: XGUIFontSize.M,
                            title_anchor: TextAnchor.MiddleLeft,
                            prop: sp_target_spawn_name,
                            options: ele_names,
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
                                sp_target_spawn_name.stringValue = d;
                                sp_target_spawn_name.serializedObject.ApplyModifiedProperties();

                                sp_target_spawn_indicator.stringValue = "Indicator_" + d;
                                sp_target_spawn_indicator.serializedObject.ApplyModifiedProperties();
                            });

                        XGUI.layout_space(5);
                    }
                }
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
                #region 生成状态
                XGUI.layout_state_displayer_text(
                    title: "生成状态",
                    title_size: XGUIFontSize.M,
                    subtitle: sp_m_opt_is_spawing.boolValue ? "生成中" : "待命中",
                    subtitle_size: XGUIFontSize.M,
                    subtitle_color: sp_m_opt_is_spawing.boolValue ? XHud_Dashboard.themes_get_color(ThemeColorType.primary) : Color.gray,
                    margin: new RectOffset(5, 5, 0, 5));
                #endregion
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 选项
            base_script.fold_option = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "选项",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: base_script.fold_option);

            if (!base_script.fold_option)
            {
                #region 可见性创建与回收
                DrawToggle("可见性创建与回收", sp_opt_create_visual, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, stroptions_enabled, (b) => { });
                #endregion

                #region 手动创建与回收
                DrawToggle("手动创建与回收", sp_opt_create_manully, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, stroptions_enabled, (b) => { });
                #endregion

                XGUI.layout_seperator(
                    thickness: 1,
                    color: XHud_Dashboard.themes_get_color(ThemeColorType.sepline),
                    margin: new RectOffset(15, 15, 15, 15));

                #region 使用世界空间
                DrawToggle("使用世界空间", sp_opt_create_world, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, stroptions_world, (b) =>
                {
                    sp_opt_create_world.boolValue = b;
                    sp_opt_create_world.serializedObject.ApplyModifiedProperties();

                    if (sp_opt_create_world.intValue == 0)
                        base_script.motion_create_args.anchor = XHudAnchor.中心;
                    else
                        base_script.motion_create_args.anchor = XHudAnchor.顶层;
                });
                #endregion

                #region R M S 布局模式
                DrawToggle("R M S 布局模式", sp_opt_rms_mode, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, stroptions_enabled, (b) =>
                {
                });
                #endregion

                #region 安全保护机制
                DrawToggle("安全保护机制", sp_opt_protected_spawn_action, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, stroptions_protecte, (b) => { });
                #endregion

                #region 元素自动播放
                DrawToggle("元素自动播放", sp_opt_auto_in, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, stroptions_enabled, (b) => { });
                #endregion

                #region 使用元素自身动效
                DrawToggle("使用元素自身动效", sp_opt_use_element_motion, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, stroptions_enabled, (b) => { });
                #endregion
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region RMS
            base_script.fold_rms = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "RMS",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: base_script.fold_rms);

            if (!base_script.fold_rms)
            {
                XGUI.SetEnabled(!sp_opt_create_world.boolValue && sp_opt_rms_mode.boolValue);
                ScreenResolutionNode[] nodes = mgr.hm_RMS_GetResolutionNodes();

                if (nodes.Length > 0)
                {
                    string[] nodesName = mgr.hm_RMS_GetResolutionNodeNames();
                    XGUI.layout_string_popup(
                      title: "RMS 方案",
                      title_width: 60,
                      title_size: XGUIFontSize.M,
                      title_anchor: TextAnchor.MiddleLeft,
                      prop: sp_rms_index_name,
                      options: nodesName,
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

                      });
                }
                else
                {
                    XGUI.layout_label(
                        text: "暂未在管理器中配置 R M S 方案列表",
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
                XGUI.SetEnabled(true);
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 生成器参数
            base_script.fold_param = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "生成器参数",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: base_script.fold_param);

            if (!base_script.fold_param)
            {
                #region 生成器标识
                XGUI.layout_property_field(
                    title: "生成器标识",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: sp_spawner_indicator,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                if (sp_opt_create_manully.boolValue)
                {
                    XGUI.layout_property_field(
                    title: "主要辅助按键",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: sp_spawn_key_primary,
                    prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_property_field(
                    title: "次级辅助按键",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: sp_spawn_key_secondary,
                    prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_property_field(
                    title: "生成按键",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: sp_spawn_key_create,
                    prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_property_field(
                    title: "回收按键",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: sp_spawn_key_recycle,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                }
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 目标变换 - 平面特性
            base_script.fold_trans = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "目标变换 - 平面特性",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: base_script.fold_trans);

            if (!base_script.fold_trans)
            {
                XGUI.layout_label(
                    text: "如果不希望指定生成的元素尺寸，请保持尺寸值为 0 !",
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

                #region 尺寸
                sp_preset_element_size.vector2Value = XGUI.layout_inputfield(
                    title: "尺寸",
                    prop: sp_preset_element_size.vector2Value,
                    title_size: XGUIFontSize.M,
                    title_anchor: TextAnchor.MiddleLeft,
                    title_width: 80,
                    field_padding: new RectOffset(0, 0, 0, 0),
                    field_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                #region 偏移
                sp_preset_element_offset.vector3Value = XGUI.layout_inputfield(
                    title: "偏移",
                    prop: sp_preset_element_offset.vector3Value,
                    title_size: XGUIFontSize.M,
                    title_anchor: TextAnchor.MiddleLeft,
                    title_width: 80,
                    field_padding: new RectOffset(0, 0, 0, 0),
                    field_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                XGUI.SetEnabled(!sp_opt_create_world.boolValue);
                #region 缩放
                sp_preset_element_scale.vector3Value = XGUI.layout_inputfield(
                    title: "缩放",
                    prop: sp_preset_element_scale.vector3Value,
                    title_size: XGUIFontSize.M,
                    title_anchor: TextAnchor.MiddleLeft,
                    title_width: 80,
                    field_padding: new RectOffset(0, 0, 0, 0),
                    field_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                #region 锚点
                SerializedProperty sp_anchor_type = serializedObject.FindProperty(nameof(XHud_Spawner.motion_create_args)).FindPropertyRelative(nameof(Motion_Creator.anchor));
                XGUI.layout_property_field(
                    title: "锚点",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: sp_anchor_type,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion
                XGUI.SetEnabled(true);
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 目标变换 - 世界特性
            base_script.fold_trans_world = XGUI.layout_group_start(
            type: XGUIContainerType.Vertical,
            bg_fill: XGUIGroup.缺口纯色边框,
            bg_color: XGUIColor.亮白,
            bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
            title: "目标变换 - 世界特性",
            title_size: XGUIFontSize.M,
            title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
            title_clipping: TextClipping.Clip,
            padding: new RectOffset(10, 10, 15, 15),
            foldout: base_script.fold_trans_world);

            if (!base_script.fold_trans_world)
            {
                XGUI.SetEnabled(sp_opt_create_world.boolValue);
                #region 位置
                sp_preset_element_world_position.vector3Value = XGUI.layout_inputfield(
                    title: "位置",
                    prop: sp_preset_element_world_position.vector3Value,
                    title_size: XGUIFontSize.M,
                    title_anchor: TextAnchor.MiddleLeft,
                    title_width: 80,
                    field_padding: new RectOffset(0, 0, 0, 0),
                    field_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                #region 角度
                sp_preset_element_world_rotation.vector3Value = XGUI.layout_inputfield(
                    title: "角度",
                    prop: sp_preset_element_world_rotation.vector3Value,
                    title_size: XGUIFontSize.M,
                    title_anchor: TextAnchor.MiddleLeft,
                    title_width: 80,
                    field_padding: new RectOffset(0, 0, 0, 0),
                    field_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                #region 缩放
                sp_preset_element_world_scale.vector3Value = XGUI.layout_inputfield(
                    title: "缩放",
                    prop: sp_preset_element_world_scale.vector3Value,
                    title_size: XGUIFontSize.M,
                    title_anchor: TextAnchor.MiddleLeft,
                    title_width: 80,
                    field_padding: new RectOffset(0, 0, 0, 0),
                    field_margin: new RectOffset(0, 0, 5, 0));
                #endregion
                XGUI.SetEnabled(true);
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 参考物体
            base_script.fold_trans_world_refer = XGUI.layout_group_start(
            type: XGUIContainerType.Vertical,
            bg_fill: XGUIGroup.缺口纯色边框,
            bg_color: XGUIColor.亮白,
            bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
            title: "参考物体",
            title_size: XGUIFontSize.M,
            title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
            title_clipping: TextClipping.Clip,
            padding: new RectOffset(10, 10, 15, 15),
            foldout: base_script.fold_trans_world_refer);

            if (!base_script.fold_trans_world_refer)
            {
                if (sp_preset_element_referobject.objectReferenceValue == null)
                    XGUI.layout_label(
                        text: "如果不指定参考坐标物体，则默认使用生成坐标作为元素生成条件",
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

                XGUI.SetEnabled(sp_opt_create_world.boolValue);
                #region 参考物体
                XGUI.layout_property_field(
                    title: "参考物体",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: sp_preset_element_referobject,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion
                XGUI.SetEnabled(true);
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
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
                if (mgr.library_motions != null)
                {
                    //确保动效库不是空的
                    if (mgr.library_motions.motion_list == null || mgr.library_motions.motion_list.Count <= 0)
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
                        string[] motnames = mgr.library_motions.ElementMotion_GetAllName_With_Create();
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

                                Motion_Creator crc = mgr.library_motions.ElementMotion_GetElementCreator_At_Create(sp_motion_create_library_index_name.stringValue);
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
                XGUI.SetEnabled(mgr.library_motions != null);
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
                XGUI.SetEnabled(mgr.library_motions != null);
                if (XGUI.layout_button(
                  tooltip: "定位",
                  tex_release: locate_r,
                  tex_press: locate_p,
                  tex_gui_color: Color.white,
                  border: new RectOffset(0, 0, 0, 0),
                  width: locate_r.width,
                  height: locate_r.height))
                {
                    if (!mgr.library_motions.ElementMotion_IsExist(sp_motion_create_library_index_name.stringValue))
                        return;
                    Editor_XHud_MenuItemsAction_OpenLibrary.open_elementmotion();
                    mgr.library_motions.ElementMotionLibrary_Location(sp_motion_create_library_index_name.stringValue);
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
                if (mgr.library_motions != null)
                {
                    //确保动效库不是空的
                    if (mgr.library_motions.motion_list == null || mgr.library_motions.motion_list.Count <= 0)
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
                        string[] motnames = mgr.library_motions.ElementMotion_GetAllName_With_Recycle();
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

                                Motion_Recycler crc = mgr.library_motions.ElementMotion_GetElementCreator_At_Recycle(sp_motion_recycle_library_index_name.stringValue);
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
                XGUI.SetEnabled(mgr.library_motions != null);
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
                XGUI.SetEnabled(mgr.library_motions != null);
                if (XGUI.layout_button(
                  tooltip: "定位",
                  tex_release: locate_r,
                  tex_press: locate_p,
                  tex_gui_color: Color.white,
                  border: new RectOffset(0, 0, 0, 0),
                  width: locate_r.width,
                  height: locate_r.height))
                {
                    if (!mgr.library_motions.ElementMotion_IsExist(sp_motion_recycle_library_index_name.stringValue))
                        return;
                    Editor_XHud_MenuItemsAction_OpenLibrary.open_elementmotion();
                    mgr.library_motions.ElementMotionLibrary_Location(sp_motion_recycle_library_index_name.stringValue);
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

            #region 弹出菜单
            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                #region 获取属性
                SerializedProperty target_library_name = serializedObject.FindProperty(nameof(XHud_Spawner.target_library_name));
                SerializedProperty target_spawn_name = serializedObject.FindProperty(nameof(XHud_Spawner.target_spawn_name));
                SerializedProperty target_spawn_indicator = serializedObject.FindProperty(nameof(XHud_Spawner.target_spawn_indicator));
                SerializedProperty spawner_indicator = serializedObject.FindProperty(nameof(XHud_Spawner.spawner_indicator));

                SerializedProperty preset_element_size = serializedObject.FindProperty(nameof(XHud_Spawner.preset_element_size));
                SerializedProperty preset_element_offset = serializedObject.FindProperty(nameof(XHud_Spawner.preset_element_offset));
                SerializedProperty preset_element_scale = serializedObject.FindProperty(nameof(XHud_Spawner.preset_element_scale));
                SerializedProperty preset_element_world_position = serializedObject.FindProperty(nameof(XHud_Spawner.preset_element_world_position));
                SerializedProperty preset_element_world_rotation = serializedObject.FindProperty(nameof(XHud_Spawner.preset_element_world_rotation));
                SerializedProperty preset_element_world_scale = serializedObject.FindProperty(nameof(XHud_Spawner.preset_element_world_scale));
                SerializedProperty preset_element_referobject = serializedObject.FindProperty(nameof(XHud_Spawner.preset_element_referobject));

                SerializedProperty spawn_key_primary = serializedObject.FindProperty(nameof(XHud_Spawner.spawn_key_primary));
                SerializedProperty spawn_key_secondary = serializedObject.FindProperty(nameof(XHud_Spawner.spawn_key_secondary));
                SerializedProperty spawn_key_create = serializedObject.FindProperty(nameof(XHud_Spawner.spawn_key_create));
                SerializedProperty spawn_key_recycle = serializedObject.FindProperty(nameof(XHud_Spawner.spawn_key_recycle));
                SerializedProperty opt_create_manully = serializedObject.FindProperty(nameof(XHud_Spawner.opt_create_manully));
                SerializedProperty opt_create_visual = serializedObject.FindProperty(nameof(XHud_Spawner.opt_create_visual));
                SerializedProperty opt_create_world = serializedObject.FindProperty(nameof(XHud_Spawner.opt_create_world));
                SerializedProperty opt_rms_mode = serializedObject.FindProperty(nameof(XHud_Spawner.opt_rms_mode));
                SerializedProperty opt_auto_in = serializedObject.FindProperty(nameof(XHud_Spawner.opt_auto_in));

                SerializedProperty opt_use_element_motion = serializedObject.FindProperty(nameof(XHud_Spawner.opt_use_element_motion));

                SerializedProperty motion_create_library_index_name = serializedObject.FindProperty(nameof(XHud_Spawner.motion_create_library_index_name));
                SerializedProperty motion_recycle_library_index_name = serializedObject.FindProperty(nameof(XHud_Spawner.motion_recycle_library_index_name));
                SerializedProperty sp_rms_index_name = serializedObject.FindProperty(nameof(XHud_Spawner.rms_index_name));

                SerializedProperty sp_spawner_running = serializedObject.FindProperty(XHud_Spawner.editor_serializefield_m_opt_is_spawing);
                #endregion

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
            XGUI.CenterEditorWindow(new Vector2Int(348, group.type == HudElementMotionType.Creator ? 710 : 640), window);
            window.set_motion_type(group.type);
            window.set_preedit_target_id(base_script.GetInstanceID().ToString());
            window.set_motion(group.motion_creator, group.motion_recycler);
            window.set_library_setter_mode(LibrarySetterMode.修改动效参数);
            window.set_button_text("更新");

            string hexcol = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.themes_get_color(ThemeColorType.primary), true);
            window.set_title($"<color={hexcol}>{(string.IsNullOrEmpty(sp_spawner_indicator.stringValue) ? base_script.gameObject.name : $"{base_script.gameObject.name} - {sp_spawner_indicator.stringValue}")} </color>");

            //window.ShowModal();
            window.Show();
        }
        #endregion

        #region 绘制控件
        /// <summary>
        /// 通用方法：绘制带标题的开关控件，统一标题宽度、字体与混合选项样式
        /// </summary>
        /// <param name="title">开关标题文字</param>
        /// <param name="prop">开关对应的序列化属性</param>
        /// <param name="width">标题宽度</param>
        /// <param name="style">开关样式</param>
        /// <param name="color_bg_on">开启状态背景色</param>
        /// <param name="color_bg_off">关闭状态背景色</param>
        /// <param name="color_on">开启状态手柄颜色</param>
        /// <param name="color_off">关闭状态手柄颜色</param>
        /// <param name="options">混合选项文字（多选时显示）</param>
        /// <param name="act_on_changed">值变化时的回调</param>
        private void DrawToggle(string title, SerializedProperty prop, float width, XGUIToggleStyle style = XGUIToggleStyle.实体, Color color_bg_on = default, Color color_bg_off = default, Color color_on = default, Color color_off = default, string[] options = null, Action<bool> act_on_changed = null)
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
                tog_mixed_bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                act_on_changed: act_on_changed);
        }
        #endregion
    }
}