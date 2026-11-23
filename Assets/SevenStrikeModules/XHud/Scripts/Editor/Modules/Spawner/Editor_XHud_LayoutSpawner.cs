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
    using SevenStrikeModules.XTween.Editor;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;
    using Object = UnityEngine.Object;

    /// <summary>
    /// 元素信息
    /// </summary>
    [System.Serializable]
    public class HudElementInfo
    {
        public List<XHud_Module_Element> elements = new List<XHud_Module_Element>();
        public List<GameObject> notPrefabsList = new List<GameObject>();
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(XHud_LayoutSpawner))]
    public class Editor_XHud_LayoutSpawner : Editor
    {
        #region 组件 / 列表
        private XHud_LayoutSpawner base_script;
        /// <summary>
        /// 元素列表 - 屏幕
        /// </summary>
        public ReorderableList element_list_screen;
        /// <summary>
        /// 元素列表 - 屏幕
        /// </summary>
        public ReorderableList element_list_world;
        private XHud_Manager hud_manager;
        #endregion

        private bool IsPreviewing;

        #region 序列化属性
        [SerializeField]
        private SerializedProperty sp_library_name;
        [SerializeField]
        private SerializedProperty sp_library_mode;
        [SerializeField]
        private SerializedProperty sp_is_loaded_layout_screen;
        [SerializeField]
        private SerializedProperty sp_is_loaded_layout_world;
        [SerializeField]
        private SerializedProperty sp_spawn_item_list_screen;
        [SerializeField]
        private SerializedProperty sp_spawn_item_list_world;
        [SerializeField]
        private SerializedProperty sp_spawner_indicator;
        [SerializeField]
        private SerializedProperty sp_motion_create_args;
        [SerializeField]
        private SerializedProperty sp_motion_recycle_args;
        [SerializeField]
        private SerializedProperty sp_key_create;
        [SerializeField]
        private SerializedProperty sp_key_recycle;
        [SerializeField]
        private SerializedProperty sp_use_manully_key;
        [SerializeField]
        private SerializedProperty sp_motion_create_library_index_name;
        [SerializeField]
        private SerializedProperty sp_motion_recycle_library_index_name;
        [SerializeField]
        private SerializedProperty sp_spawn_function_key_primary;
        [SerializeField]
        private SerializedProperty sp_spawn_function_key_secondary;
        [SerializeField]
        private SerializedProperty sp_key_control_screen;
        [SerializeField]
        private SerializedProperty sp_key_control_world;
        [NonSerialized]
        private SerializedProperty sp_fold_screen;
        [NonSerialized]
        private SerializedProperty sp_fold_world;
        [SerializeField]
        private SerializedProperty sp_editor_seializedfield_spawner_is_spawning;
        [SerializeField]
        private SerializedProperty sp_use_instantiate_mode;
        [SerializeField]
        private SerializedProperty sp_use_debug;
        #endregion

        #region 选项文字
        string[] stroptions_enabled = new string[2] { "关闭", "开启" };
        string[] stroptions_control = new string[2] { "禁用", "可控" };
        #endregion

        #region 图标
        /// <summary>
        /// 主图标
        /// </summary>
        private Texture2D icon_main;
        /// <summary>
        /// 保存按钮常态图标
        /// </summary>
        private Texture2D icon_save_r;
        /// <summary>
        /// 保存按钮按下图标
        /// </summary>
        private Texture2D icon_save_p;
        /// <summary>
        /// 定位按钮常态图标
        /// </summary>
        private Texture2D icon_locate_r;
        /// <summary>
        /// 定位按钮按下图标
        /// </summary>
        private Texture2D icon_locate_p;
        /// <summary>
        /// 元素图标
        /// </summary>
        private Texture2D icon_elementicon;
        /// <summary>
        /// 节点圆点图标
        /// </summary>
        private Texture2D icon_dot;
        /// <summary>
        /// 展开按钮常态图标
        /// </summary>
        private Texture2D icon_icon_unfold_r;
        /// <summary>
        /// 展开按钮按下图标
        /// </summary>
        private Texture2D icon_icon_unfold_p;
        /// <summary>
        /// 动画折叠按钮常态图标
        /// </summary>
        private Texture2D icon_anim_fold_r;
        /// <summary>
        /// 动画折叠按钮按下图标
        /// </summary>
        private Texture2D icon_anim_fold_p;
        /// <summary>
        /// 重置按钮常态图标
        /// </summary>
        private Texture2D icon_reset_r;
        /// <summary>
        /// 重置按钮按下图标
        /// </summary>
        private Texture2D icon_reset_p;
        /// <summary>
        /// 扫描按钮常态图标
        /// </summary>
        private Texture2D icon_scan_r;
        /// <summary>
        /// 扫描按钮按下图标
        /// </summary>
        private Texture2D icon_scan_p;
        /// <summary>
        /// 预览入场按钮常态图标
        /// </summary>
        private Texture2D icon_preview_in_r;
        /// <summary>
        /// 预览入场按钮按下图标
        /// </summary>
        private Texture2D icon_preview_in_p;
        /// <summary>
        /// 预览出场按钮常态图标
        /// </summary>
        private Texture2D icon_preview_out_r;
        /// <summary>
        /// 预览出场按钮按下图标
        /// </summary>
        private Texture2D icon_preview_out_p;
        /// <summary>
        /// 停止预览按钮常态图标
        /// </summary>
        private Texture2D icon_stoppreview_r;
        /// <summary>
        /// 停止预览按钮按下图标
        /// </summary>
        private Texture2D icon_stoppreview_p;
        /// <summary>
        /// 加载按钮常态图标
        /// </summary>
        private Texture2D icon_load_r;
        /// <summary>
        /// 加载按钮按下图标
        /// </summary>
        private Texture2D icon_load_p;
        /// <summary>
        /// 卸载按钮常态图标
        /// </summary>
        private Texture2D icon_unload_r;
        /// <summary>
        /// 卸载按钮按下图标
        /// </summary>
        private Texture2D icon_unload_p;
        /// <summary>
        /// 重建 ID 按钮常态图标
        /// </summary>
        private Texture2D icon_recreate_id_r;
        /// <summary>
        /// 重建 ID 按钮按下图标
        /// </summary>
        private Texture2D icon_recreate_id_p;
        /// <summary>
        /// 编辑按钮常态图标
        /// </summary>
        private Texture2D edit_r;
        /// <summary>
        /// 编辑按钮按下图标
        /// </summary>
        private Texture2D edit_p;
        #endregion

        public XHudSpace cur_space;

        #region 批量化操作
        XHud_LayoutSpawner[] selected_objects;

        private void Targets_Get()
        {
            if (targets.Length > 1)
            {
                selected_objects = new XHud_LayoutSpawner[targets.Length];
                for (int i = 0; i < selected_objects.Length; i++)
                {
                    var t = targets[i];
                    selected_objects[i] = (XHud_LayoutSpawner)t;
                }
            }
            else
            {
                selected_objects = new XHud_LayoutSpawner[targets.Length];
                selected_objects[0] = (XHud_LayoutSpawner)target;
            }
        }

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

        Rect drawelement_screen_rect;
        Rect item_rect_screen;
        int selected_Screen = -1;

        Rect drawelement_world_rect;
        Rect item_rect_world;
        int selected_World = -1;

        private void OnEnable()
        {
            hud_manager = XHud_Dashboard.get_manager();

            base_script = (XHud_LayoutSpawner)target;

            #region 获取序列化属性
            sp_library_name = serializedObject.FindProperty(nameof(XHud_LayoutSpawner.library_name));
            sp_library_mode = serializedObject.FindProperty(nameof(XHud_LayoutSpawner.library_mode));
            sp_spawn_item_list_screen = serializedObject.FindProperty(nameof(XHud_LayoutSpawner.spawn_item_list_screen));
            sp_spawn_item_list_world = serializedObject.FindProperty(nameof(XHud_LayoutSpawner.spawn_item_list_world));
            sp_spawner_indicator = serializedObject.FindProperty(nameof(XHud_LayoutSpawner.spawner_indicator));
            sp_motion_create_args = serializedObject.FindProperty(nameof(XHud_LayoutSpawner.motion_create_args));
            sp_motion_recycle_args = serializedObject.FindProperty(nameof(XHud_LayoutSpawner.motion_recycle_args));
            sp_key_create = serializedObject.FindProperty(nameof(XHud_LayoutSpawner.key_create));
            sp_key_recycle = serializedObject.FindProperty(nameof(XHud_LayoutSpawner.key_recycle));
            sp_use_manully_key = serializedObject.FindProperty(nameof(XHud_LayoutSpawner.use_manully_key));
            sp_motion_create_library_index_name = serializedObject.FindProperty(nameof(XHud_LayoutSpawner.motion_create_library_index_name));
            sp_motion_recycle_library_index_name = serializedObject.FindProperty(nameof(XHud_LayoutSpawner.motion_recycle_library_index_name));
            sp_is_loaded_layout_screen = serializedObject.FindProperty(nameof(XHud_LayoutSpawner.is_loaded_layout_screen));
            sp_is_loaded_layout_world = serializedObject.FindProperty(nameof(XHud_LayoutSpawner.is_loaded_layout_world));
            sp_spawn_function_key_primary = serializedObject.FindProperty(nameof(XHud_LayoutSpawner.spawn_function_key_primary));
            sp_spawn_function_key_secondary = serializedObject.FindProperty(nameof(XHud_LayoutSpawner.spawn_function_key_secondary));
            sp_key_control_screen = serializedObject.FindProperty(nameof(XHud_LayoutSpawner.key_control_screen));
            sp_key_control_world = serializedObject.FindProperty(nameof(XHud_LayoutSpawner.key_control_world));
            sp_fold_screen = serializedObject.FindProperty(nameof(XHud_LayoutSpawner.fold_screen));
            sp_fold_world = serializedObject.FindProperty(nameof(XHud_LayoutSpawner.fold_world));
            sp_editor_seializedfield_spawner_is_spawning = serializedObject.FindProperty(XHud_LayoutSpawner.editor_seializedfield_spawner_is_spawning);
            sp_use_instantiate_mode = serializedObject.FindProperty(nameof(XHud_LayoutSpawner.use_instantiate_mode));
            sp_use_debug = serializedObject.FindProperty(nameof(XHud_LayoutSpawner.use_debug));
            #endregion

            #region 获取图标
            icon_main = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_layout_spawner/icon_main");
            icon_save_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_layout_spawner/icon_save_r");
            icon_save_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_layout_spawner/icon_save_p");
            icon_locate_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_layout_spawner/icon_locate_r");
            icon_locate_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_layout_spawner/icon_locate_p");
            icon_elementicon = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_layout_spawner/icon_elementicon");
            icon_dot = XGUI.GetBasedIcon("icon_dot");
            icon_icon_unfold_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_layout_spawner/icon_unfold_r");
            icon_icon_unfold_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_layout_spawner/icon_unfold_p");
            icon_anim_fold_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_layout_spawner/icon_anim_fold_r");
            icon_anim_fold_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_layout_spawner/icon_anim_fold_p");
            icon_reset_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_layout_spawner/icon_reset_r");
            icon_reset_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_layout_spawner/icon_reset_p");
            icon_scan_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_layout_spawner/icon_scan_r");
            icon_scan_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_layout_spawner/icon_scan_p");
            icon_preview_in_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_layout_spawner/icon_preview_in_r");
            icon_preview_in_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_layout_spawner/icon_preview_in_p");
            icon_preview_out_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_layout_spawner/icon_preview_out_r");
            icon_preview_out_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_layout_spawner/icon_preview_out_p");
            icon_stoppreview_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_layout_spawner/icon_stoppreview_r");
            icon_stoppreview_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_layout_spawner/icon_stoppreview_p");
            icon_load_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_layout_spawner/icon_load_r");
            icon_load_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_layout_spawner/icon_load_p");
            icon_unload_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_layout_spawner/icon_unload_r");
            icon_unload_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_layout_spawner/icon_unload_p");
            icon_recreate_id_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_layout_spawner/icon_recreate_id_r");
            icon_recreate_id_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_layout_spawner/icon_recreate_id_p");
            edit_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_layout_spawner/edit_r");
            edit_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_layout_spawner/edit_p");
            #endregion

            Targets_Get();

            #region 用于第一次加载脚本判断库名是否是空的
            if (!Application.isPlaying && hud_manager != null)
            {
                //获取所有元素库名称
                string[] LibNames = hud_manager.hm_ElementLibrary_GetAllLibraryNames();
                //如果不是空的
                if (LibNames.Length > 0)
                {
                    //判断如果库名是空的就把首个元素库名给他
                    if (string.IsNullOrEmpty(sp_library_name.stringValue))
                    {
                        sp_library_name.stringValue = LibNames[0];
                        sp_library_name.serializedObject.ApplyModifiedProperties();
                    }
                }
            }
            #endregion

            #region 屏幕元素
            element_list_screen = new ReorderableList(serializedObject, sp_spawn_item_list_screen)
            {
                draggable = false,
                drawElementBackgroundCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    if (selected_Screen != -1 && selected_Screen == index)
                    {
                        // 高亮标记表示选中
                        item_rect_screen.Set(rect.x, rect.y, 1, 25);
                        EditorGUI.DrawRect(item_rect_screen, XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                    }
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    Event e = Event.current;

                    #region 获取序列化属性
                    SerializedProperty prop = sp_spawn_item_list_screen.GetArrayElementAtIndex(index);
                    SerializedProperty sp_spawn_name = prop.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.spawn_name));
                    SerializedProperty sp_lib = prop.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.library_name));
                    SerializedProperty sp_euid = prop.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.element_uid));
                    SerializedProperty sp_indicator = prop.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.indicator));
                    SerializedProperty sp_id = prop.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.id));
                    SerializedProperty sp_delay_spawn = prop.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.delay_spawn));
                    SerializedProperty sp_delay_despawn = prop.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.delay_despawn));
                    SerializedProperty sp_auto_in = prop.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.auto_in));
                    SerializedProperty sp_in_motion = prop.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.in_motion));
                    SerializedProperty sp_spawned = prop.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.spawned));
                    SerializedProperty sp_motion_percentage = prop.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.motion_percentage));
                    SerializedProperty sp_motion_mode = prop.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.motion_mode));
                    SerializedProperty sp_position = prop.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.position));
                    SerializedProperty sp_euler = prop.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.euler));
                    SerializedProperty sp_offset = prop.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.offset));
                    SerializedProperty sp_scale = prop.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.scale));
                    SerializedProperty sp_size = prop.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.size));
                    SerializedProperty sp_pivot = prop.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.pivot));
                    SerializedProperty sp_anchor_min = prop.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.anchor_min));
                    SerializedProperty sp_anchor_max = prop.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.anchor_max));
                    SerializedProperty sp_is_fold = prop.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.is_fold));
                    SerializedProperty sp_is_enabled = prop.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.is_enabled));
                    #endregion

                    float cur_width = XGUI.GetCurrentWindowWidth();

                    #region 详细数据折叠开关
                    drawelement_screen_rect.Set(rect.x + 2, rect.y + 4, 12, 12);
                    if (!sp_is_fold.boolValue)
                    {
                        if (XGUI.gui_button(
                            rect: drawelement_screen_rect,
                            tooltip: $"折叠详细数据",
                            tex_release: icon_icon_unfold_r,
                            tex_press: icon_icon_unfold_p,
                            tex_gui_color: Color.white,
                            margin: new RectOffset(0, 0, 0, 0),
                            padding: new RectOffset(0, 0, 0, 0)))
                        {
                            for (int i = 0; i < element_list_screen.count; i++)
                            {
                                var foldstate = sp_spawn_item_list_screen
                                    .GetArrayElementAtIndex(i)
                                    .FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.is_fold));
                                if (foldstate != null)
                                    foldstate.boolValue = true;
                            }
                            sp_spawn_item_list_screen.serializedObject.ApplyModifiedProperties();
                        }
                    }
                    else
                    {
                        if (XGUI.gui_button(
                            rect: drawelement_screen_rect,
                            tooltip: $"展开详细数据",
                            tex_release: icon_anim_fold_r,
                            tex_press: icon_anim_fold_p,
                            tex_gui_color: Color.white,
                            margin: new RectOffset(0, 0, 0, 0),
                            padding: new RectOffset(0, 0, 0, 0)))
                        {
                            // 手风琴：只展开当前项，其余全部折叠
                            for (int i = 0; i < element_list_screen.count; i++)
                            {
                                var foldstate = sp_spawn_item_list_screen
                                    .GetArrayElementAtIndex(i)
                                    .FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.is_fold));
                                if (foldstate != null)
                                    foldstate.boolValue = (i != index);   // 关键：当前项 false，其余 true
                            }
                            sp_spawn_item_list_screen.serializedObject.ApplyModifiedProperties();
                        }
                    }
                    #endregion

                    #region 索引号
                    drawelement_screen_rect.Set(rect.x + 25, rect.y, 40, 20);
                    if (cur_width >= 125)
                    {
                        XGUI.gui_label(
                            rect: drawelement_screen_rect,
                            text: new GUIContent(index.ToString("D1")),
                            text_color: Color.gray,
                            size: XGUIFontSize.M,
                            clipping: TextClipping.Overflow,
                            anchor: TextAnchor.MiddleLeft,
                            offset: new Vector2(0, 0),
                            font_style: FontStyle.Normal);
                    }
                    #endregion

                    #region 开关
                    bool is_shrink_toggle = cur_width > 180;
                    if (is_shrink_toggle)
                    {
                        drawelement_screen_rect.Set(rect.x + 50, rect.y + 1, 40, XGUI.GetSingleLineHeight());
                        XGUI.gui_toggle(
                            rect: drawelement_screen_rect,
                            title_color: Color.white,
                            title_size: XGUIFontSize.M,
                            title_font_style: FontStyle.Normal,
                            title_padding: new RectOffset(0, 0, 0, 0),
                            title_width: 0,
                            tog_interval: 10,
                            prop: sp_is_enabled,
                            tog_style: XGUIToggleStyle.实体,
                            tog_mixed_options: new string[] { "关闭", "开启" },
                            tog_mixed_text_size: XGUIFontSize.M,
                            tog_mixed_text_color: Color.black,
                            tog_mixed_text_padding: new RectOffset(0, 0, 0, 0),
                            tog_mixed_text_anchor: TextAnchor.MiddleCenter,
                            tog_mixed_text_font_style: FontStyle.Normal,
                            tog_bg_off_color: Color.gray,
                            tog_bg_on_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                            tog_handler_off_color: Color.white,
                            tog_handler_on_color: Color.white,
                            tog_mixed_bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                        sp_is_enabled.serializedObject.ApplyModifiedProperties();
                    }
                    #endregion

                    #region 徽标 （已生成）
                    drawelement_screen_rect.Set(rect.x + 105, rect.y + 5, 10, 10);
                    XGUI.gui_icon(
                        rect: drawelement_screen_rect,
                        icon: icon_elementicon,
                        color: sp_spawned.boolValue ? XHud_Dashboard.themes_get_color(ThemeColorType.primary) : Color.gray);
                    #endregion

                    #region 徽标 （运动中）
                    drawelement_screen_rect.Set(rect.x + 125, rect.y + 5, 10, 10);
                    XGUI.gui_icon(
                        rect: drawelement_screen_rect,
                        icon: icon_elementicon,
                        color: sp_in_motion.boolValue ? XHud_Dashboard.themes_get_color(ThemeColorType.primary) : Color.gray);
                    #endregion

                    #region 元素 ID
                    drawelement_screen_rect.Set(rect.x + rect.width - 55, rect.y + 1, 50, 21);
                    if (cur_width >= 230)
                    {
                        XGUI.gui_label(
                            rect: drawelement_screen_rect,
                            text: new GUIContent(sp_id.stringValue),
                            text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                            bg_fill: XGUIGroup.实体,
                            bg_color: XGUIColor.深空灰,
                            bg_color_gui: Color.white,
                            size: XGUIFontSize.M,
                            clipping: TextClipping.Overflow,
                            anchor: TextAnchor.MiddleCenter,
                            padding: new RectOffset(5, 5, 10, 10),
                            offset: new Vector2(0, -1),
                            font_style: FontStyle.Normal);
                    }
                    if (e.type == EventType.MouseDown && e.button == 0 && drawelement_screen_rect.Contains(e.mousePosition))
                    {
                        EditorGUIUtility.systemCopyBuffer = sp_id.stringValue;
                        EditorApplication.delayCall += () =>
                        {
                            string hexcol = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.themes_get_color(ThemeColorType.primary), true);

                            XGUI.dialog(
                                type: XGUIDialogType.确认,
                                windowtitle: "XHud - 布局元素生成器消息",
                                title: "拷贝元素项ID",
                                msg: $"已将元素项的ID： <color={hexcol}><b>   {sp_id.stringValue}   </b></color>拷贝至系统剪贴板中！",
                                ok: "明白",
                                PrimaryIndex: 0,
                                usemodal: true,
                                themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                        };
                        e.Use();
                    }
                    #endregion

                    #region 拖拽数据
                    drawelement_screen_rect.Set(rect.x, rect.y + 5, rect.width - 70, rect.height - 10);
                    //XGUI.gui_box(drawelement_screen_rect, Color.clear);

                    if (e.type == EventType.MouseDrag && drawelement_screen_rect.Contains(e.mousePosition))
                    {
                        DragAndDrop.PrepareStartDrag();
                        DragAndDrop.SetGenericData("LayoutSpawnerBindData", JsonUtility.ToJson(base_script.spawn_item_list_screen[index]));
                        DragAndDrop.StartDrag("LayoutSpawnerBindDataDrag");
                        e.Use();
                    }
                    #endregion

                    #region 元素名称
                    drawelement_screen_rect.Set(rect.x + 8, rect.y + 28, rect.width - 20, 20);
                    if (cur_width >= 125)
                    {
                        XGUI.gui_label(
                            rect: drawelement_screen_rect,
                            text: new GUIContent(sp_spawn_name.stringValue, $"uid: {sp_euid.stringValue}"),
                            text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                            size: XGUIFontSize.B,
                            clipping: TextClipping.Ellipsis,
                            anchor: TextAnchor.MiddleLeft,
                            offset: new Vector2(0, 0),
                            font_style: FontStyle.Normal);
                    }
                    #endregion

                    #region AutoIn
                    drawelement_screen_rect.Set(rect.x + (rect.width - rect.x) - 15, rect.y + 30, 55, 20);
                    if (cur_width >= 137)
                    {
                        XGUI.gui_string_popup(
                            rect: drawelement_screen_rect,
                            //title: "类型",
                            //title_color: Color.white,
                            //title_size: XGUIFontSize.M,
                            //title_font_style: FontStyle.Normal,
                            //title_padding: new RectOffset(0, 0, 0, 0),
                            title_width: 0,
                            prop: sp_auto_in,
                            options: new string[2] { "自动 In", "手动 In" },
                            opt_text_size: XGUIFontSize.S,
                            opt_text_color: Color.white * 0.88f,
                            opt_text_padding: new RectOffset(0, 0, 0, 0),
                            opt_anchor: TextAnchor.MiddleCenter,
                            opt_font_style: FontStyle.Normal,
                            opt_bg_fill: XGUIGroup.透明,
                            //opt_bg_color: XGUIColor.亮白,
                            //opt_bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                            width_limite: 55,
                            usearrow: false);
                    }
                    #endregion

                    #region 使用生成器的通用动效
                    drawelement_screen_rect.Set(rect.x + (rect.width - rect.x) - 95, rect.y + 30, 88, 20);
                    if (cur_width >= 210)
                    {
                        XGUI.gui_string_popup(
                           rect: drawelement_screen_rect,
                           //title: "类型",
                           //title_color: Color.white,
                           //title_size: XGUIFontSize.M,
                           //title_font_style: FontStyle.Normal,
                           //title_padding: new RectOffset(0, 0, 0, 0),
                           title_width: 0,
                           prop: sp_motion_mode,
                           options: new string[3] { "元素自身动效", "列表项动效", "生成器动效" },
                           opt_text_size: XGUIFontSize.S,
                           opt_text_color: Color.white * 0.88f,
                           opt_text_padding: new RectOffset(0, 0, 0, 0),
                           opt_anchor: TextAnchor.MiddleCenter,
                           opt_font_style: FontStyle.Normal,
                           opt_bg_fill: XGUIGroup.透明,
                           //opt_bg_color: XGUIColor.亮白,
                           //opt_bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                           width_limite: 55,
                           usearrow: false);
                    }
                    #endregion

                    float baseheight = rect.y + 52;

                    #region 分割线
                    drawelement_screen_rect.Set(rect.x, baseheight, rect.width, 1);
                    XGUI.gui_box(drawelement_screen_rect, Color.gray);
                    #endregion

                    #region UID 状态点 + 多元库标签
                    if (base_script.library_mode == LibraryMode.多元库模式)
                    {
                        #region uid 有效性图标可视化
                        drawelement_screen_rect.Set(rect.x + 10, baseheight + 14, icon_dot.width, icon_dot.width);
                        XGUI.gui_icon(
                            rect: drawelement_screen_rect,
                            icon: icon_dot,
                            color: string.IsNullOrEmpty(sp_euid.stringValue) ? new Color(1, 0.3f, 0.3f) : XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                        #endregion

                        #region  库标签显示                       
                        drawelement_screen_rect.Set(rect.x + 25, baseheight + 6, rect.width - 20, XGUI.GetSingleLineHeight());
                        XGUI.gui_label(
                            rect: drawelement_screen_rect,
                            text: new GUIContent($"<b>[</b> {sp_lib.stringValue} <b>]</b>"),
                            text_color: Color.white * 0.8f,
                            size: XGUIFontSize.S,
                            clipping: TextClipping.Ellipsis,
                            anchor: TextAnchor.MiddleLeft,
                            padding: new RectOffset(5, 10, 0, 0),
                            font_style: FontStyle.Normal);
                        #endregion
                    }
                    #endregion

                    #region 正在生成 / 回收进度条
                    if (sp_in_motion.boolValue)
                    {
                        Repaint();
                        drawelement_screen_rect.Set(rect.x, baseheight, rect.width * sp_motion_percentage.floatValue, 1);
                        XGUI.gui_box(drawelement_screen_rect, XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                    }
                    #endregion

                    #region 延迟参数
                    drawelement_screen_rect.Set(rect.x + 5, baseheight + (base_script.library_mode == LibraryMode.多元库模式 ? 32 : 10), rect.width / 2 - 15, 18);
                    XGUI.gui_property_field(
                        rect: drawelement_screen_rect,
                        title: new GUIContent("延迟生成"),
                        title_size: XGUIFontSize.S,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 45,
                        prop: sp_delay_spawn);

                    drawelement_screen_rect.Set(rect.x + rect.width / 2 + 5, baseheight + (base_script.library_mode == LibraryMode.多元库模式 ? 32 : 10), rect.width / 2 - 10, 18);
                    XGUI.gui_property_field(
                        rect: drawelement_screen_rect,
                        title: new GUIContent("延迟回收"),
                        title_size: XGUIFontSize.S,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 45,
                        prop: sp_delay_despawn);
                    #endregion

                    #region 信息
                    if (!sp_is_fold.boolValue)
                    {
                        string hexcol = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.themes_get_color(ThemeColorType.primary) * 0.85f, true);
                        string darkcol = XGUI_Utilitys.Color_To_HexString(Color.white * 0.75f, true);

                        Vector2 margin = new Vector2(6, 10);
                        float offset_height = base_script.library_mode == LibraryMode.多元库模式 ? 18 : 0;
                        #region pos
                        drawelement_screen_rect.Set(rect.x + margin.x, baseheight + 40 + offset_height, rect.width / 2 - 15, 20);
                        XGUI.gui_label(
                            rect: drawelement_screen_rect,
                            text: new GUIContent($"<color={hexcol}>pos：</color> <color={darkcol}>{sp_position.vector3Value}</color>"),
                            text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                            size: XGUIFontSize.S,
                            clipping: TextClipping.Clip,
                            anchor: TextAnchor.MiddleLeft,
                            offset: new Vector2(0, 0),
                            font_style: FontStyle.Normal);
                        #endregion

                        #region rot
                        drawelement_screen_rect.Set(rect.x + rect.width / 2 + 5, baseheight + 40 + offset_height, rect.width / 2 - margin.y, 20);
                        XGUI.gui_label(
                            rect: drawelement_screen_rect,
                            text: new GUIContent($"<color={hexcol}>rot：</color> <color={darkcol}>{sp_euler.vector3Value}</color>"),
                            text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                            size: XGUIFontSize.S,
                            clipping: TextClipping.Clip,
                            anchor: TextAnchor.MiddleRight,
                            offset: new Vector2(0, 0),
                            font_style: FontStyle.Normal);
                        #endregion

                        #region offset
                        drawelement_screen_rect.Set(rect.x + margin.x, baseheight + 65 + offset_height, rect.width / 2 - 15, 20);
                        XGUI.gui_label(
                            rect: drawelement_screen_rect,
                            text: new GUIContent($"<color={hexcol}>offset：</color> <color={darkcol}>{sp_offset.vector3Value}</color>"),
                            text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                            size: XGUIFontSize.S,
                            clipping: TextClipping.Clip,
                            anchor: TextAnchor.MiddleLeft,
                            offset: new Vector2(0, 0),
                            font_style: FontStyle.Normal);
                        #endregion

                        #region scl
                        drawelement_screen_rect.Set(rect.x + rect.width / 2 + 5, baseheight + 65 + offset_height, rect.width / 2 - margin.y, 20);
                        XGUI.gui_label(
                            rect: drawelement_screen_rect,
                            text: new GUIContent($"<color={hexcol}>scl：</color> <color={darkcol}>{sp_scale.vector3Value}</color>"),
                            text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                            size: XGUIFontSize.S,
                            clipping: TextClipping.Clip,
                            anchor: TextAnchor.MiddleRight,
                            offset: new Vector2(0, 0),
                            font_style: FontStyle.Normal);
                        #endregion

                        drawelement_screen_rect.Set(rect.x, baseheight + 90 + offset_height, rect.width, 1);
                        XGUI.gui_box(drawelement_screen_rect, Color.gray * 0.7f);

                        #region size
                        drawelement_screen_rect.Set(rect.x + margin.x, baseheight + 98 + offset_height, rect.width / 2 - 15, 20);
                        XGUI.gui_label(
                            rect: drawelement_screen_rect,
                            text: new GUIContent($"<color={hexcol}>size：</color> <color={darkcol}>{sp_size.vector2Value}</color>"),
                            text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                            size: XGUIFontSize.S,
                            clipping: TextClipping.Clip,
                            anchor: TextAnchor.MiddleLeft,
                            offset: new Vector2(0, 0),
                            font_style: FontStyle.Normal);
                        #endregion

                        #region pivot
                        drawelement_screen_rect.Set(rect.x + rect.width / 2 + 5, baseheight + 98 + offset_height, rect.width / 2 - margin.y, 20);
                        XGUI.gui_label(
                            rect: drawelement_screen_rect,
                            text: new GUIContent($"<color={hexcol}>pivot：</color> <color={darkcol}>{sp_pivot.vector2Value}</color>"),
                            text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                            size: XGUIFontSize.S,
                            clipping: TextClipping.Clip,
                            anchor: TextAnchor.MiddleRight,
                            offset: new Vector2(0, 0),
                            font_style: FontStyle.Normal);
                        #endregion

                        #region anc min
                        drawelement_screen_rect.Set(rect.x + margin.x, baseheight + 118 + offset_height, rect.width / 2 - 15, 20);
                        XGUI.gui_label(
                            rect: drawelement_screen_rect,
                            text: new GUIContent($"<color={hexcol}>anc min：</color> <color={darkcol}>{sp_anchor_min.vector2Value}</color>"),
                            text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                            size: XGUIFontSize.S,
                            clipping: TextClipping.Clip,
                            anchor: TextAnchor.MiddleLeft,
                            offset: new Vector2(0, 0),
                            font_style: FontStyle.Normal);
                        #endregion

                        #region anc max
                        drawelement_screen_rect.Set(rect.x + rect.width / 2 + 5, baseheight + 118 + offset_height, rect.width / 2 - margin.y, 20);
                        XGUI.gui_label(
                            rect: drawelement_screen_rect,
                            text: new GUIContent($"<color={hexcol}>anc max：</color> <color={darkcol}>{sp_anchor_max.vector2Value}</color>"),
                            text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                            size: XGUIFontSize.S,
                            clipping: TextClipping.Clip,
                            anchor: TextAnchor.MiddleRight,
                            offset: new Vector2(0, 0),
                            font_style: FontStyle.Normal);
                        #endregion
                    }
                    #endregion

                    #region  右键菜单
                    drawelement_screen_rect.Set(rect.x, rect.y, rect.width - 50, rect.height);
                    if (e.type == EventType.MouseDown && e.button == 1 && drawelement_screen_rect.Contains(e.mousePosition))
                    {
                        // 创建右键菜单
                        GenericMenu menu = new GenericMenu();
                        menu.AddItem(new GUIContent("E 修改动效"), false, () =>
                        {
                            if (!sp_is_loaded_layout_screen.boolValue && sp_motion_mode.stringValue == "元素自身动效")
                            {
                                XGUI.dialog(
                                    type: XGUIDialogType.警告,
                                    windowtitle: "XHud - 布局元素生成器消息",
                                    title: "无法修改动效",
                                    msg: "在 \"元素自身动效\" 模式下，前提要先把元素加载才可以对元素自身动效参数进行修改",
                                    ok: "明白",
                                    PrimaryIndex: 0,
                                    usemodal: true,
                                    themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                                return;
                            }

                            motion_edit(
                                group: new XHud_Arg_MotionGroup(
                                    mode: HudElementMotionType.Creator,
                                    creator: base_script.spawn_item_list_screen[index].motion_create_args,
                                    recycler: base_script.spawn_item_list_screen[index].motion_recycle_args),
                                space: XHudSpace.屏幕空间,
                                index: index);
                            return;
                        });
                        menu.AddItem(new GUIContent("C 复制元素 UID)"), false, () =>
                        {
                            GUIUtility.systemCopyBuffer = base_script.spawn_item_list_screen[index].element_uid ?? "";
                        });
                        // 显示右键菜单
                        menu.ShowAsContext();

                        e.Use(); // 标记事件已被处理，防止其他操作处理该事件
                    }
                    GUI.enabled = true;
                    #endregion
                },
                elementHeightCallback = index =>
                {
                    var foldProp = sp_spawn_item_list_screen.GetArrayElementAtIndex(index).FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.is_fold));
                    return (foldProp != null && foldProp.boolValue) ? (base_script.library_mode == LibraryMode.多元库模式 ? 110 : 90f) : (base_script.library_mode == LibraryMode.多元库模式 ? 220f : 205f);
                },
                onMouseUpCallback = list =>
                {
                    selected_Screen = list.index;
                },
                onSelectCallback = list =>
                {
                    selected_Screen = list.index;

                    if (base_script.spawn_item_list_screen.Count > 0)
                    {
                        var item = base_script.spawn_item_list_screen[list.index];
                        if (item == null || item.spawned_element_node == null)
                            return;

                        XHud_Module_Element element = item.spawned_element_node.element;

                        if (element != null)
                        {
                            //  停止预览
                            XTween_Preview_Stop();

                            EditorGUIUtility.PingObject(element);
                            // 元素高亮闪烁
                            XTween_Preview_Start(new XTween_Interface[] { element.element_Highlight() }, XHudSpace.屏幕空间, false);
                        }
                    }
                }
            };
            #endregion

            #region 世界元素
            element_list_world = new ReorderableList(serializedObject, sp_spawn_item_list_world)
            {
                draggable = false,
                drawElementBackgroundCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    if (selected_World != -1 && selected_World == index)
                    {
                        // 高亮标记表示选中
                        item_rect_world.Set(rect.x, rect.y, 1, 25);
                        EditorGUI.DrawRect(item_rect_world, XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                    }
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    Event e = Event.current;

                    #region 获取序列化属性
                    SerializedProperty prop = sp_spawn_item_list_world.GetArrayElementAtIndex(index);
                    SerializedProperty sp_spawn_name = prop.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.spawn_name));
                    SerializedProperty sp_lib = prop.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.library_name));
                    SerializedProperty sp_euid = prop.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.element_uid));
                    SerializedProperty sp_indicator = prop.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.indicator));
                    SerializedProperty sp_id = prop.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.id));
                    SerializedProperty sp_delay_spawn = prop.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.delay_spawn));
                    SerializedProperty sp_delay_despawn = prop.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.delay_despawn));
                    SerializedProperty sp_auto_in = prop.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.auto_in));
                    SerializedProperty sp_in_motion = prop.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.in_motion));
                    SerializedProperty sp_spawned = prop.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.spawned));
                    SerializedProperty sp_motion_percentage = prop.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.motion_percentage));
                    SerializedProperty sp_motion_mode = prop.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.motion_mode));
                    SerializedProperty sp_position = prop.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.position));
                    SerializedProperty sp_euler = prop.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.euler));
                    SerializedProperty sp_offset = prop.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.offset));
                    SerializedProperty sp_scale = prop.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.scale));
                    SerializedProperty sp_size = prop.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.size));
                    SerializedProperty sp_pivot = prop.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.pivot));
                    SerializedProperty sp_anchor_min = prop.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.anchor_min));
                    SerializedProperty sp_anchor_max = prop.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.anchor_max));
                    SerializedProperty sp_is_fold = prop.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.is_fold));
                    SerializedProperty sp_is_enabled = prop.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.is_enabled));
                    #endregion

                    float cur_width = XGUI.GetCurrentWindowWidth();

                    #region 详细数据折叠开关
                    drawelement_world_rect.Set(rect.x + 2, rect.y + 4, 12, 12);
                    if (!sp_is_fold.boolValue)
                    {
                        if (XGUI.gui_button(
                            rect: drawelement_world_rect,
                            tooltip: $"折叠详细数据",
                            tex_release: icon_icon_unfold_r,
                            tex_press: icon_icon_unfold_p,
                            tex_gui_color: Color.white,
                            margin: new RectOffset(0, 0, 0, 0),
                            padding: new RectOffset(0, 0, 0, 0)))
                        {
                            for (int i = 0; i < element_list_world.count; i++)
                            {
                                var foldstate = sp_spawn_item_list_world
                                    .GetArrayElementAtIndex(i)
                                    .FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.is_fold));
                                if (foldstate != null)
                                    foldstate.boolValue = true;
                            }
                            sp_spawn_item_list_world.serializedObject.ApplyModifiedProperties();
                        }
                    }
                    else
                    {
                        if (XGUI.gui_button(
                            rect: drawelement_world_rect,
                            tooltip: $"展开详细数据",
                            tex_release: icon_anim_fold_r,
                            tex_press: icon_anim_fold_p,
                            tex_gui_color: Color.white,
                            margin: new RectOffset(0, 0, 0, 0),
                            padding: new RectOffset(0, 0, 0, 0)))
                        {
                            for (int i = 0; i < element_list_world.count; i++)
                            {
                                var foldstate = sp_spawn_item_list_world
                                    .GetArrayElementAtIndex(i)
                                    .FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.is_fold));
                                if (foldstate != null)
                                    foldstate.boolValue = (i != index);   // 关键
                            }
                            sp_spawn_item_list_world.serializedObject.ApplyModifiedProperties();
                        }
                    }
                    #endregion

                    #region 索引号
                    drawelement_world_rect.Set(rect.x + 25, rect.y, 40, 20);
                    if (cur_width >= 125)
                    {
                        XGUI.gui_label(
                            rect: drawelement_world_rect,
                            text: new GUIContent(index.ToString("D1")),
                            text_color: Color.gray,
                            size: XGUIFontSize.M,
                            clipping: TextClipping.Overflow,
                            anchor: TextAnchor.MiddleLeft,
                            offset: new Vector2(0, 0),
                            font_style: FontStyle.Normal);
                    }
                    #endregion

                    #region 开关
                    bool is_shrink_toggle = cur_width > 180;
                    if (is_shrink_toggle)
                    {
                        drawelement_world_rect.Set(rect.x + 50, rect.y + 1, 40, XGUI.GetSingleLineHeight());
                        XGUI.gui_toggle(
                            rect: drawelement_world_rect,
                            title_color: Color.white,
                            title_size: XGUIFontSize.M,
                            title_font_style: FontStyle.Normal,
                            title_padding: new RectOffset(0, 0, 0, 0),
                            title_width: 0,
                            tog_interval: 10,
                            prop: sp_is_enabled,
                            tog_style: XGUIToggleStyle.实体,
                            tog_mixed_options: new string[] { "关闭", "开启" },
                            tog_mixed_text_size: XGUIFontSize.M,
                            tog_mixed_text_color: Color.black,
                            tog_mixed_text_padding: new RectOffset(0, 0, 0, 0),
                            tog_mixed_text_anchor: TextAnchor.MiddleCenter,
                            tog_mixed_text_font_style: FontStyle.Normal,
                            tog_bg_off_color: Color.gray,
                            tog_bg_on_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                            tog_handler_off_color: Color.white,
                            tog_handler_on_color: Color.white,
                            tog_mixed_bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                        sp_is_enabled.serializedObject.ApplyModifiedProperties();
                    }
                    #endregion

                    #region 徽标 （已生成）
                    drawelement_world_rect.Set(rect.x + 105, rect.y + 5, 10, 10);
                    XGUI.gui_icon(
                        rect: drawelement_world_rect,
                        icon: icon_elementicon,
                        color: sp_spawned.boolValue ? XHud_Dashboard.themes_get_color(ThemeColorType.primary) : Color.gray);
                    #endregion

                    #region 徽标 （运动中）
                    drawelement_world_rect.Set(rect.x + 125, rect.y + 5, 10, 10);
                    XGUI.gui_icon(
                        rect: drawelement_world_rect,
                        icon: icon_elementicon,
                        color: sp_in_motion.boolValue ? XHud_Dashboard.themes_get_color(ThemeColorType.primary) : Color.gray);
                    #endregion

                    #region 元素 ID
                    drawelement_world_rect.Set(rect.x + rect.width - 55, rect.y + 1, 50, 21);
                    if (cur_width >= 230)
                    {
                        XGUI.gui_label(
                            rect: drawelement_world_rect,
                            text: new GUIContent(sp_id.stringValue),
                            text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                            bg_fill: XGUIGroup.实体,
                            bg_color: XGUIColor.深空灰,
                            bg_color_gui: Color.white,
                            size: XGUIFontSize.M,
                            clipping: TextClipping.Overflow,
                            anchor: TextAnchor.MiddleCenter,
                            padding: new RectOffset(5, 5, 10, 10),
                            offset: new Vector2(0, -1),
                            font_style: FontStyle.Normal);
                    }
                    if (e.type == EventType.MouseDown && e.button == 0 && drawelement_world_rect.Contains(e.mousePosition))
                    {
                        EditorGUIUtility.systemCopyBuffer = sp_id.stringValue;
                        EditorApplication.delayCall += () =>
                        {
                            string hexcol = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.themes_get_color(ThemeColorType.primary), true);

                            XGUI.dialog(
                                type: XGUIDialogType.确认,
                                windowtitle: "XHud - 布局元素生成器消息",
                                title: "拷贝元素项ID",
                                msg: $"已将元素项的ID： <color={hexcol}><b>   {sp_id.stringValue}   </b></color>拷贝至系统剪贴板中！",
                                ok: "明白",
                                PrimaryIndex: 0,
                                usemodal: true,
                                themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                        };
                        e.Use();
                    }
                    #endregion

                    #region 拖拽数据
                    drawelement_world_rect.Set(rect.x, rect.y + 5, rect.width - 70, rect.height - 10);
                    //XGUI.gui_box(drawelement_world_rect, Color.clear);

                    if (e.type == EventType.MouseDrag && drawelement_world_rect.Contains(e.mousePosition))
                    {
                        DragAndDrop.PrepareStartDrag();
                        DragAndDrop.SetGenericData("LayoutSpawnerBindData", JsonUtility.ToJson(base_script.spawn_item_list_world[index]));
                        DragAndDrop.StartDrag("LayoutSpawnerBindDataDrag");
                        e.Use();
                    }
                    #endregion

                    #region 元素名称
                    drawelement_world_rect.Set(rect.x + 8, rect.y + 28, rect.width - 20, 20);
                    if (cur_width >= 125)
                    {
                        XGUI.gui_label(
                            rect: drawelement_world_rect,
                            text: new GUIContent(sp_spawn_name.stringValue, $"uid: {sp_euid.stringValue}"),
                            text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                            size: XGUIFontSize.B,
                            clipping: TextClipping.Ellipsis,
                            anchor: TextAnchor.MiddleLeft,
                            offset: new Vector2(0, 0),
                            font_style: FontStyle.Normal);
                    }
                    #endregion             

                    #region AutoIn
                    drawelement_world_rect.Set(rect.x + (rect.width - rect.x) - 15, rect.y + 30, 55, 20);
                    if (cur_width >= 137)
                    {
                        XGUI.gui_string_popup(
                            rect: drawelement_world_rect,
                            //title: "类型",
                            //title_color: Color.white,
                            //title_size: XGUIFontSize.M,
                            //title_font_style: FontStyle.Normal,
                            //title_padding: new RectOffset(0, 0, 0, 0),
                            title_width: 0,
                            prop: sp_auto_in,
                            options: new string[2] { "自动 In", "手动 In" },
                            opt_text_size: XGUIFontSize.S,
                            opt_text_color: Color.white * 0.88f,
                            opt_text_padding: new RectOffset(0, 0, 0, 0),
                            opt_anchor: TextAnchor.MiddleCenter,
                            opt_font_style: FontStyle.Normal,
                            opt_bg_fill: XGUIGroup.透明,
                            //opt_bg_color: XGUIColor.亮白,
                            //opt_bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                            width_limite: 55,
                            usearrow: false);
                    }
                    #endregion

                    #region 使用生成器的通用动效
                    drawelement_world_rect.Set(rect.x + (rect.width - rect.x) - 95, rect.y + 30, 88, 20);
                    if (cur_width >= 210)
                    {
                        XGUI.gui_string_popup(
                           rect: drawelement_world_rect,
                           //title: "类型",
                           //title_color: Color.white,
                           //title_size: XGUIFontSize.M,
                           //title_font_style: FontStyle.Normal,
                           //title_padding: new RectOffset(0, 0, 0, 0),
                           title_width: 0,
                           prop: sp_motion_mode,
                           options: new string[3] { "元素自身动效", "列表项动效", "生成器动效" },
                           opt_text_size: XGUIFontSize.S,
                           opt_text_color: Color.white * 0.88f,
                           opt_text_padding: new RectOffset(0, 0, 0, 0),
                           opt_anchor: TextAnchor.MiddleCenter,
                           opt_font_style: FontStyle.Normal,
                           opt_bg_fill: XGUIGroup.透明,
                           //opt_bg_color: XGUIColor.亮白,
                           //opt_bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                           width_limite: 55,
                           usearrow: false);
                    }
                    #endregion

                    float baseheight = rect.y + 52;

                    #region 分割线
                    drawelement_world_rect.Set(rect.x, baseheight, rect.width, 1);
                    XGUI.gui_box(drawelement_world_rect, Color.gray);
                    #endregion

                    #region UID 状态点 + 多元库标签
                    if (base_script.library_mode == LibraryMode.多元库模式)
                    {
                        #region uid 有效性图标可视化
                        drawelement_world_rect.Set(rect.x + 10, baseheight + 14, icon_dot.width, icon_dot.width);
                        XGUI.gui_icon(
                            rect: drawelement_world_rect,
                            icon: icon_dot,
                            color: string.IsNullOrEmpty(sp_euid.stringValue) ? new Color(1, 0.3f, 0.3f) : XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                        #endregion

                        #region  库标签显示                       
                        drawelement_world_rect.Set(rect.x + 25, baseheight + 6, rect.width - 20, XGUI.GetSingleLineHeight());
                        XGUI.gui_label(
                            rect: drawelement_world_rect,
                            text: new GUIContent($"<b>[</b> {sp_lib.stringValue} <b>]</b>"),
                            text_color: Color.white * 0.8f,
                            size: XGUIFontSize.S,
                            clipping: TextClipping.Ellipsis,
                            anchor: TextAnchor.MiddleLeft,
                            padding: new RectOffset(5, 10, 0, 0),
                            font_style: FontStyle.Normal);
                        #endregion
                    }
                    #endregion

                    #region 正在生成 / 回收进度条
                    if (sp_in_motion.boolValue)
                    {
                        Repaint();
                        drawelement_world_rect.Set(rect.x, baseheight, rect.width * sp_motion_percentage.floatValue, 1);
                        XGUI.gui_box(drawelement_world_rect, XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                    }
                    #endregion

                    #region 延迟参数
                    drawelement_world_rect.Set(rect.x + 5, baseheight + (base_script.library_mode == LibraryMode.多元库模式 ? 32 : 10), rect.width / 2 - 15, 18);
                    XGUI.gui_property_field(
                        rect: drawelement_world_rect,
                        title: new GUIContent("延迟生成"),
                        title_size: XGUIFontSize.S,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 45,
                        prop: sp_delay_spawn);

                    drawelement_world_rect.Set(rect.x + rect.width / 2 + 5, baseheight + (base_script.library_mode == LibraryMode.多元库模式 ? 32 : 10), rect.width / 2 - 10, 18);
                    XGUI.gui_property_field(
                        rect: drawelement_world_rect,
                        title: new GUIContent("延迟回收"),
                        title_size: XGUIFontSize.S,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 45,
                        prop: sp_delay_despawn);
                    #endregion

                    #region 信息
                    if (!sp_is_fold.boolValue)
                    {
                        string hexcol = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.themes_get_color(ThemeColorType.primary) * 0.85f, true);
                        string darkcol = XGUI_Utilitys.Color_To_HexString(Color.white * 0.75f, true);
                        Vector2 margin = new Vector2(6, 10);
                        float offset_height = base_script.library_mode == LibraryMode.多元库模式 ? 18 : 0;

                        #region pos
                        drawelement_world_rect.Set(rect.x + margin.x, baseheight + 40 + offset_height, rect.width / 2 - 15, 20);
                        XGUI.gui_label(
                            rect: drawelement_world_rect,
                            text: new GUIContent($"<color={hexcol}>pos：</color> <color={darkcol}>{sp_position.vector3Value}</color>"),
                            text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                            size: XGUIFontSize.S,
                            clipping: TextClipping.Clip,
                            anchor: TextAnchor.MiddleLeft,
                            offset: new Vector2(0, 0),
                            font_style: FontStyle.Normal);
                        #endregion

                        #region rot
                        drawelement_world_rect.Set(rect.x + rect.width / 2 + 5, baseheight + 40 + offset_height, rect.width / 2 - margin.y, 20);
                        XGUI.gui_label(
                            rect: drawelement_world_rect,
                            text: new GUIContent($"<color={hexcol}>rot：</color> <color={darkcol}>{sp_euler.vector3Value}</color>"),
                            text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                            size: XGUIFontSize.S,
                            clipping: TextClipping.Clip,
                            anchor: TextAnchor.MiddleRight,
                            offset: new Vector2(0, 0),
                            font_style: FontStyle.Normal);
                        #endregion

                        #region offset
                        drawelement_world_rect.Set(rect.x + margin.x, baseheight + 65 + offset_height, rect.width / 2 - 15, 20);
                        XGUI.gui_label(
                            rect: drawelement_world_rect,
                            text: new GUIContent($"<color={hexcol}>offset：</color> <color={darkcol}>{sp_offset.vector3Value}</color>"),
                            text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                            size: XGUIFontSize.S,
                            clipping: TextClipping.Clip,
                            anchor: TextAnchor.MiddleLeft,
                            offset: new Vector2(0, 0),
                            font_style: FontStyle.Normal);
                        #endregion

                        #region scl
                        drawelement_world_rect.Set(rect.x + rect.width / 2 + 5, baseheight + 65 + offset_height, rect.width / 2 - margin.y, 20);
                        XGUI.gui_label(
                            rect: drawelement_world_rect,
                            text: new GUIContent($"<color={hexcol}>scl：</color> <color={darkcol}>{sp_scale.vector3Value}</color>"),
                            text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                            size: XGUIFontSize.S,
                            clipping: TextClipping.Clip,
                            anchor: TextAnchor.MiddleRight,
                            offset: new Vector2(0, 0),
                            font_style: FontStyle.Normal);
                        #endregion

                        drawelement_world_rect.Set(rect.x, baseheight + 90 + offset_height, rect.width, 1);
                        XGUI.gui_box(drawelement_world_rect, Color.gray * 0.7f);

                        #region size
                        drawelement_world_rect.Set(rect.x + margin.x, baseheight + 98 + offset_height, rect.width / 2 - 15, 20);
                        XGUI.gui_label(
                            rect: drawelement_world_rect,
                            text: new GUIContent($"<color={hexcol}>size：</color> <color={darkcol}>{sp_size.vector2Value}</color>"),
                            text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                            size: XGUIFontSize.S,
                            clipping: TextClipping.Clip,
                            anchor: TextAnchor.MiddleLeft,
                            offset: new Vector2(0, 0),
                            font_style: FontStyle.Normal);
                        #endregion

                        #region pivot
                        drawelement_world_rect.Set(rect.x + rect.width / 2 + 5, baseheight + 98 + offset_height, rect.width / 2 - margin.y, 20);
                        XGUI.gui_label(
                            rect: drawelement_world_rect,
                            text: new GUIContent($"<color={hexcol}>pivot：</color> <color={darkcol}>{sp_pivot.vector2Value}</color>"),
                            text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                            size: XGUIFontSize.S,
                            clipping: TextClipping.Clip,
                            anchor: TextAnchor.MiddleRight,
                            offset: new Vector2(0, 0),
                            font_style: FontStyle.Normal);
                        #endregion

                        #region anc min
                        drawelement_world_rect.Set(rect.x + margin.x, baseheight + 118 + offset_height, rect.width / 2 - 15, 20);
                        XGUI.gui_label(
                            rect: drawelement_world_rect,
                            text: new GUIContent($"<color={hexcol}>anc min：</color> <color={darkcol}>{sp_anchor_min.vector2Value}</color>"),
                            text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                            size: XGUIFontSize.S,
                            clipping: TextClipping.Clip,
                            anchor: TextAnchor.MiddleLeft,
                            offset: new Vector2(0, 0),
                            font_style: FontStyle.Normal);
                        #endregion

                        #region anc max
                        drawelement_world_rect.Set(rect.x + rect.width / 2 + 5, baseheight + 118 + offset_height, rect.width / 2 - margin.y, 20);
                        XGUI.gui_label(
                            rect: drawelement_world_rect,
                            text: new GUIContent($"<color={hexcol}>anc max：</color> <color={darkcol}>{sp_anchor_max.vector2Value}</color>"),
                            text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                            size: XGUIFontSize.S,
                            clipping: TextClipping.Clip,
                            anchor: TextAnchor.MiddleRight,
                            offset: new Vector2(0, 0),
                            font_style: FontStyle.Normal);
                        #endregion
                    }
                    #endregion

                    #region  右键菜单
                    drawelement_world_rect.Set(rect.x, rect.y, rect.width - 50, rect.height);
                    if (e.type == EventType.MouseDown && e.button == 1 && drawelement_world_rect.Contains(e.mousePosition))
                    {
                        // 创建右键菜单
                        GenericMenu menu = new GenericMenu();
                        menu.AddItem(new GUIContent("E 修改动效"), false, () =>
                        {
                            if (!sp_is_loaded_layout_world.boolValue && sp_motion_mode.stringValue == "元素自身动效")
                            {
                                XGUI.dialog(
                                    type: XGUIDialogType.警告,
                                    windowtitle: "XHud - 布局元素生成器消息",
                                    title: "无法修改动效",
                                    msg: "在 \"元素自身动效\" 模式下，前提要先把元素加载才可以对元素自身动效参数进行修改",
                                    ok: "明白",
                                    PrimaryIndex: 0,
                                    usemodal: true,
                                    themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                                return;
                            }

                            motion_edit(
                                group: new XHud_Arg_MotionGroup(
                                    mode: HudElementMotionType.Creator,
                                    creator: base_script.spawn_item_list_world[index].motion_create_args,
                                    recycler: base_script.spawn_item_list_world[index].motion_recycle_args),
                                space: XHudSpace.世界空间,
                                index: index);
                            return;
                        });
                        menu.AddItem(new GUIContent("C 复制元素 UID)"), false, () =>
                        {
                            GUIUtility.systemCopyBuffer = base_script.spawn_item_list_world[index].element_uid ?? "";
                        });
                        // 显示右键菜单
                        menu.ShowAsContext();

                        e.Use(); // 标记事件已被处理，防止其他操作处理该事件                            
                    }
                    GUI.enabled = true;
                    #endregion
                },
                elementHeightCallback = index =>
                {
                    var foldProp = sp_spawn_item_list_world.GetArrayElementAtIndex(index).FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.is_fold));
                    return (foldProp != null && foldProp.boolValue) ? (base_script.library_mode == LibraryMode.多元库模式 ? 110 : 90f) : (base_script.library_mode == LibraryMode.多元库模式 ? 220f : 205f);
                },
                onMouseUpCallback = list =>
                {
                    selected_World = list.index;
                },
                onSelectCallback = list =>
                {
                    selected_World = list.index;

                    if (base_script.spawn_item_list_world.Count > 0)
                    {
                        var item = base_script.spawn_item_list_world[list.index];
                        if (item == null || item.spawned_element_node == null)
                            return;

                        XHud_Module_Element element = item.spawned_element_node.element;

                        if (element != null)
                        {
                            //  停止预览
                            XTween_Preview_Stop();

                            EditorGUIUtility.PingObject(element);
                            // 元素高亮闪烁
                            XTween_Preview_Start(new XTween_Interface[] { element.element_Highlight() }, XHudSpace.世界空间, false);
                        }
                    }
                }
            };
            #endregion
        }

        private void OnDisable()
        {
            if (target == null)
                return;

            // 屏幕列表：全部折叠（循环改值，最后统一 Apply 一次）
            if (element_list_screen != null && sp_spawn_item_list_screen != null)
            {
                for (int i = 0; i < element_list_screen.count; i++)
                {
                    var foldstate = sp_spawn_item_list_screen.GetArrayElementAtIndex(i).FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.is_fold));
                    if (foldstate != null)
                        foldstate.boolValue = true;
                }
                sp_spawn_item_list_screen.serializedObject.ApplyModifiedProperties();
            }

            // 世界列表：全部折叠
            if (element_list_world != null && sp_spawn_item_list_world != null)
            {
                for (int i = 0; i < element_list_world.count; i++)
                {
                    var foldstate = sp_spawn_item_list_world.GetArrayElementAtIndex(i).FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.is_fold));
                    if (foldstate != null)
                        foldstate.boolValue = true;
                }
                sp_spawn_item_list_world.serializedObject.ApplyModifiedProperties();
            }

            XTween_Preview_Stop();
            Editor_XHud_Tool_SceneView_Activate_Mark.SetEnabled(false);
        }

        public override void OnInspectorGUI()
        {
            XHud_Manager mgr = XHud_Dashboard.get_manager();

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
                title_text: "XHud  -  布局元素生成器",
                title_anchor: TextAnchor.MiddleLeft,
                title_style: FontStyle.Normal,
                title_color: Color.white,
                title_size: XGUIFontSize.B,
                title_clipping: clipping_ellipsis,
                bg_margin: new RectOffset(0, 0, 5, 5));
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
                    subtitle: sp_editor_seializedfield_spawner_is_spawning.boolValue ? "运行中" : "待命中",
                    subtitle_size: XGUIFontSize.M,
                    subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
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
                #region 调试信息
                DrawToggle("调试信息", sp_use_debug, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, stroptions_enabled, (b) => { });
                #endregion

                #region 手动生成与回收
                DrawToggle("手动生成与回收", sp_use_manully_key, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, stroptions_enabled, (b) => { });
                #endregion

                if (sp_use_manully_key.boolValue)
                {
                    #region 屏幕元素控制
                    DrawToggle("屏幕元素控制", sp_key_control_screen, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, stroptions_control, (b) => { });
                    #endregion

                    #region 世界元素控制
                    DrawToggle("世界元素控制", sp_key_control_world, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, stroptions_control, (b) => { });
                    #endregion
                }

                #region 使用实例化
                DrawToggle("使用实例化", sp_use_instantiate_mode, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, stroptions_enabled, (b) => { });
                #endregion

                #region 元素库模式
                string libmode = ((LibraryMode)sp_library_mode.enumValueIndex).ToString();
                string[] libmodes = Enum.GetNames(typeof(LibraryMode));
                XGUI.layout_string_popup(
                    title: "库模式",
                    title_width: 60,
                    title_size: XGUIFontSize.M,
                    title_anchor: TextAnchor.MiddleLeft,
                    prop: libmode,
                    options: libmodes,
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
                        sp_library_mode.enumValueIndex = (int)Enum.Parse(typeof(LibraryMode), d);
                        sp_library_mode.serializedObject.ApplyModifiedProperties();

                        //  新增：切换到多元库模式时，自动为老数据回填库名
                        if ((LibraryMode)sp_library_mode.enumValueIndex == LibraryMode.多元库模式)
                        {
                            bool changed = false;
                            for (int i = 0; i < sp_spawn_item_list_screen.arraySize; i++)
                            {
                                var prop = sp_spawn_item_list_screen.GetArrayElementAtIndex(i);
                                var sp_lib = prop.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.library_name));
                                var sp_uid = prop.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.element_uid));

                                // 如果库名为空，且 UID 不为空，则尝试反查
                                if (string.IsNullOrEmpty(sp_lib.stringValue) && !string.IsNullOrEmpty(sp_uid.stringValue))
                                {
                                    string foundLib = mgr.hm_ElementLibrary_GetTargetLibraryNameByUID(sp_uid.stringValue);
                                    if (!string.IsNullOrEmpty(foundLib))
                                    {
                                        sp_lib.stringValue = foundLib;
                                        changed = true;
                                    }
                                }
                            }
                            for (int i = 0; i < sp_spawn_item_list_world.arraySize; i++)
                            {
                                var prop = sp_spawn_item_list_world.GetArrayElementAtIndex(i);
                                var sp_lib = prop.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.library_name));
                                var sp_uid = prop.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.element_uid));

                                if (string.IsNullOrEmpty(sp_lib.stringValue) && !string.IsNullOrEmpty(sp_uid.stringValue))
                                {
                                    string foundLib = mgr.hm_ElementLibrary_GetTargetLibraryNameByUID(sp_uid.stringValue);
                                    if (!string.IsNullOrEmpty(foundLib))
                                    {
                                        sp_lib.stringValue = foundLib;
                                        changed = true;
                                    }
                                }
                            }

                            if (changed)
                            {
                                sp_spawn_item_list_screen.serializedObject.ApplyModifiedProperties();
                                sp_spawn_item_list_world.serializedObject.ApplyModifiedProperties();
                                EditorUtility.SetDirty(base_script);
                            }
                        }
                    });
                #endregion
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 目标库 / 默认库
            base_script.fold_libselector = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "目标库",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: base_script.fold_libselector);

            if (!base_script.fold_libselector)
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
                        #region 多元库模式提示
                        if (base_script.library_mode == LibraryMode.多元库模式)
                        {
                            XGUI.layout_label(
                                text: "元素列表按各项自身库生成\n此项仅用于新建/回填",
                                bg_fill: XGUIGroup.透明,
                                bg_color: XGUIColor.无,
                                size: XGUIFontSize.S,
                                anchor: TextAnchor.MiddleCenter,
                                text_color: Color.white * 0.85f,
                                padding: new RectOffset(0, 0, 0, 0),
                                margin: new RectOffset(0, 0, 2, 10),
                                clipping: TextClipping.Clip,
                                wrap: false,
                                font_style: FontStyle.Normal);
                        }
                        #endregion

                        #region 单库目标选择
                        string[] libsname = mgr.hm_ElementLibrary_GetAllLibraryNames();
                        if (!mgr.hm_ElementLibrary_IsExist(sp_library_name.stringValue))
                        {
                            sp_library_name.stringValue = mgr.hm_ElementLibrary_GetFirstLibrary().library_name;
                        }
                        XGUI.layout_string_popup(
                            title: base_script.library_mode == LibraryMode.多元库模式 ? "默认库" : "目标库",
                            title_width: 60,
                            title_size: XGUIFontSize.M,
                            title_anchor: TextAnchor.MiddleLeft,
                            prop: sp_library_name,
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
                            });
                        #endregion

                        XGUI.layout_space(5);
                    }
                }
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

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
                #region 生成器标识
                XGUI.layout_property_field(
                    title: "生成器标识",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: sp_spawner_indicator,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                if (sp_use_manully_key.boolValue)
                {
                    #region 主要辅助按键
                    XGUI.layout_property_field(
                        title: "主要辅助按键",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: sp_spawn_function_key_primary,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion

                    #region 次级辅助按键
                    XGUI.layout_property_field(
                        title: "次级辅助按键",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: sp_spawn_function_key_secondary,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion

                    #region 生成按键
                    XGUI.layout_property_field(
                        title: "生成按键",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: sp_key_create,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion

                    #region 回收按键
                    XGUI.layout_property_field(
                        title: "回收按键",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: sp_key_recycle,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion
                }

                #region 生成锚点
                SerializedProperty sp_anchor_type = sp_motion_create_args.FindPropertyRelative(nameof(Motion_Creator.anchor));
                XGUI.layout_property_field(
                    title: "生成锚点",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: sp_anchor_type,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 元素项 - 屏幕空间
            base_script.fold_screen = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "元素项 - 屏幕空间",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: base_script.fold_screen);

            if (!base_script.fold_screen)
            {

                #region 快捷按钮
                XGUI.layout_group_start(
                 type: XGUIContainerType.Horizontal,
                 title_clipping: TextClipping.Clip,
                 absolute_padding: true,
                 absolute_margin: true,
                 margin: new RectOffset(0, 0, 12, 0),
                 padding: new RectOffset(5, 10, 0, 0));

                #region 收集 & 重置
                if (sp_spawn_item_list_screen.arraySize <= 0)
                {
                    #region 收集布局      
                    if (XGUI.layout_button(
                        tooltip: "收集当前在LayoutScreen锚点下的所有屏幕元素部署布局",
                        tex_release: icon_scan_r,
                        tex_press: icon_scan_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: icon_scan_r.width,
                        height: icon_scan_r.height))
                    {
                        if (string.IsNullOrEmpty(sp_library_name.stringValue))
                        {
                            XGUI.dialog(
                                type: XGUIDialogType.警告,
                                windowtitle: "XHud - 布局元素生成器消息",
                                title: "收集元素异常",
                                msg: "请先指定目标元素库！",
                                ok: "明白",
                                PrimaryIndex: 0,
                                usemodal: true,
                                themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                            return;
                        }
                        if (hud_manager == null)
                            return;

                        ClearSpawnItemList_Screen();
                        HudElementInfo ele_info_s = AnchorElementInfosGet_Screen();

                        //无效名称列表
                        List<XGUIDialogListDatas> DialogDatas = new List<XGUIDialogListDatas>();

                        //---获取元素
                        for (int i = 0; i < ele_info_s.elements.Count; i++)
                        {
                            string item_name = PrefabUtility.GetCorrespondingObjectFromOriginalSource(ele_info_s.elements[i]).name;
                            if (!FindLibraryItemByInstance(ele_info_s.elements[i], out string hitLib, out string hitUID, out string hitName))
                            {
                                #region 加入非库中元素名称列表
                                string s = item_name;
                                XGUIDialogListDatas data = new XGUIDialogListDatas();
                                data.Title = sp_spawner_indicator.stringValue;
                                data.SubTitle = "非库中元素";
                                data.Message = s;
                                DialogDatas.Add(data);
                                #endregion
                                continue;
                            }
                            else
                            {
                                // 新增：优先使用库中自定义名称
                                if (!string.IsNullOrEmpty(hitName))
                                    item_name = hitName;

                                #region 新建一个生成项，用于快速收集目标元素的基础信息（坐标、缩放、尺寸）
                                XHud_LayoutSpawner_Item item = new XHud_LayoutSpawner_Item();
                                item.spawn_name = item_name;
                                item.element_uid = hitUID;
                                item.library_name = base_script.library_mode == LibraryMode.多元库模式 ? hitLib : null;
                                item.is_enabled = true;
                                item.is_fold = true;
                                item.position = ele_info_s.elements[i].rect_transform.anchoredPosition3D;
                                item.scale = ele_info_s.elements[i].rect_transform.localScale;
                                item.euler = ele_info_s.elements[i].rect_transform.localEulerAngles;
                                item.size = ele_info_s.elements[i].rect_transform.sizeDelta;
                                item.pivot = ele_info_s.elements[i].rect_transform.pivot;
                                item.anchor_min = ele_info_s.elements[i].rect_transform.anchorMin;
                                item.anchor_max = ele_info_s.elements[i].rect_transform.anchorMax;
                                #endregion

                                #region 判断收集的元素在哪个锚点下
                                XHudAnchor anchor = XHudAnchor.中心;
                                string ele_parent_name = ele_info_s.elements[i].rect_transform.parent.name;

                                if (ele_parent_name == "Anchor_B")
                                {
                                    anchor = XHudAnchor.底层;
                                }
                                if (ele_parent_name == "Anchor_C")
                                {
                                    anchor = XHudAnchor.中心;
                                }
                                if (ele_parent_name == "Anchor_L")
                                {
                                    anchor = XHudAnchor.左;
                                }
                                if (ele_parent_name == "Anchor_R")
                                {
                                    anchor = XHudAnchor.右;
                                }
                                if (ele_parent_name == "Anchor_U")
                                {
                                    anchor = XHudAnchor.上;
                                }
                                if (ele_parent_name == "Anchor_D")
                                {
                                    anchor = XHudAnchor.下;
                                }
                                if (ele_parent_name == "Anchor_L_U")
                                {
                                    anchor = XHudAnchor.左上;
                                }
                                if (ele_parent_name == "Anchor_L_D")
                                {
                                    anchor = XHudAnchor.左下;
                                }
                                if (ele_parent_name == "Anchor_R_U")
                                {
                                    anchor = XHudAnchor.右上;
                                }
                                if (ele_parent_name == "Anchor_R_D")
                                {
                                    anchor = XHudAnchor.右下;
                                }
                                if (ele_parent_name == "Anchor_T")
                                {
                                    anchor = XHudAnchor.顶层;
                                }
                                item.motion_create_args.anchor = anchor;
                                #endregion

                                //加入屏幕列表中
                                AddedSpawnItem_Screen(item);
                            }
                        }

                        #region 异常提示
                        //如果获取到无效元素名称
                        if (DialogDatas.Count > 0)
                        {
                            string res = XGUI.dialog_listview(
                                datas: DialogDatas.ToArray(),
                                type: XGUIDialogType.警告,
                                windowtitle: "XHud - 布局元素生成器消息",
                                title: "元素物体异常",
                                msg: $"检测到以下不合理的物体，原因：未加入到元素池中。解决方法：如果您想让这些元素被记录到部署布局中，请将其加入元素池！",
                                ok: "明白",
                                PrimaryIndex: 0,
                                usemodal: true,
                                themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                        }

                        //如果获取到非预制体元素名称
                        if (ele_info_s.notPrefabsList.Count > 0)
                        {
                            DialogDatas.Clear();
                            for (int w = 0; w < ele_info_s.notPrefabsList.Count; w++)
                            {
                                string s = ele_info_s.notPrefabsList[w].name;

                                #region 加入非预制体元素名称列表
                                XGUIDialogListDatas data = new XGUIDialogListDatas();
                                data.Title = sp_spawner_indicator.stringValue;
                                data.SubTitle = "非预制体元素";
                                data.Message = s;
                                DialogDatas.Add(data);
                                #endregion
                            }

                            string res = XGUI.dialog_listview(
                               datas: DialogDatas.ToArray(),
                               type: XGUIDialogType.警告,
                               windowtitle: "XHud - 布局元素生成器消息",
                               title: "元素物体异常",
                               msg: $"检测到以下不合理的物体，原因：物体虽然是HudElement元素但未将其制成预制体。解决方法：如果您想让这些元素被记录到部署布局中，请将其制成预制体！",
                               ok: "明白",
                               cancel: "选中",
                               PrimaryIndex: 0,
                               usemodal: true,
                               themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                            if (res == "选中")
                            {
                                Selection.objects = ele_info_s.notPrefabsList.ToArray();
                            }
                        }
                        #endregion

                        #region 记录后处理元素方式
                        if (ele_info_s.elements.Count > 0)
                        {
                            string res = XGUI.dialog(
                                type: XGUIDialogType.确认,
                                windowtitle: "XHud - 布局元素生成器消息",
                                title: "元素物体处理",
                                msg: $"如何处理场景中被记录的屏幕UI的HudElement元素物体？",
                                ok: "不做处理",
                                cancel: "移除",
                                alt: "隐藏",
                                PrimaryIndex: 0,
                                themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                            if (res == "不做处理")
                            {

                            }
                            else if (res == "移除")
                            {
                                for (int i = 0; i < ele_info_s.elements.Count; i++)
                                {
                                    Undo.DestroyObjectImmediate(ele_info_s.elements[i].gameObject);
                                }
                            }
                            else if (res == "隐藏")
                            {
                                for (int i = 0; i < ele_info_s.elements.Count; i++)
                                {
                                    Undo.RecordObject(ele_info_s.elements[i].gameObject, "HideOriginalElementPrefab_" + i);
                                    ele_info_s.elements[i].gameObject.SetActive(false);
                                }
                            }
                        }
                        else
                        {
                            XGUI.dialog(
                                type: XGUIDialogType.确认,
                                windowtitle: "XHud - 布局元素生成器消息",
                                title: "元素物体收集",
                                msg: $"未发现<color={hexcol}> 屏幕根节点 </color>下存在任何 XHud 元素物体！",
                                ok: "明白",
                                PrimaryIndex: 0);
                        }
                        #endregion
                        return;
                    }
                    #endregion
                }
                else
                {
                    #region 清空布局    
                    if (XGUI.layout_button(
                        tooltip: "将当前的元素部署布局列表清空",
                        tex_release: icon_reset_r,
                        tex_press: icon_reset_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: icon_reset_r.width,
                        height: icon_reset_r.height))
                    {
                        EditorApplication.delayCall += () =>
                        {
                            string res = XGUI.dialog(
                                type: XGUIDialogType.警告,
                                windowtitle: "XHud - 布局元素生成器消息",
                                title: "重置元素列表",
                                msg: "确定要将元素列表重置吗？您将丢失当前的所有元素记录和他们的延迟（生成 & 回收）参数！",
                                ok: "重置",
                                cancel: "暂不",
                                PrimaryIndex: 0,
                                usemodal: true,
                                themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                            if (res == "暂不")
                                return;

                            if (hud_manager == null)
                                return;

                            sp_is_loaded_layout_screen.boolValue = false;
                            sp_is_loaded_layout_screen.serializedObject.ApplyModifiedProperties();
                            for (int i = 0; i < sp_spawn_item_list_screen.arraySize; i++)
                            {
                                UnLoadSpawnItem_Screen(base_script.spawn_item_list_screen[i]);
                            }
                            ClearSpawnItemList_Screen();
                        };
                    }
                    #endregion

                    XGUI.layout_flexspace();

                    #region 重新生成ID
                    if (XGUI.layout_button(
                      tooltip: "为当前的元素列表中的项重新创建ID",
                      tex_release: icon_recreate_id_r,
                      tex_press: icon_recreate_id_p,
                      tex_gui_color: Color.white,
                      border: new RectOffset(0, 0, 0, 0),
                      width: icon_recreate_id_r.width,
                      height: icon_recreate_id_r.height))
                    {
                        EditorApplication.delayCall += () =>
                        {
                            string res = XGUI.dialog(
                                type: XGUIDialogType.警告,
                                windowtitle: "XHud - 布局元素生成器消息",
                                title: "元素ID刷新",
                                msg: "确定要将元素列表中的所有项的ID进行重新生成吗？您将丢失所有元素的当前ID！我们并不建议您这么做，因为会导致您其他脚本在获取元素时发生\" 无法找到目标元素 \"的异常！",
                                ok: "重新生成",
                                cancel: "暂不",
                                PrimaryIndex: 0,
                                usemodal: true,
                                themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));

                            if (res == "暂不")
                                return;

                            Undo.RecordObject(base_script, "刷新所有元素项ID");

                            // 一次性收集现有 ID，之后边生成边加入集合，避免循环内重复收集（O(n²) -> O(n)）
                            var existingIds = new HashSet<string>(CollectIDs(XHudSpace.屏幕空间));

                            for (int i = 0; i < base_script.spawn_item_list_screen.Count; i++)
                            {
                                string newId = XGUI_Utilitys.GenerateUniqueId(existingIds.ToArray());
                                // 生成后立即加入集合，保证后续项不会重复
                                existingIds.Add(newId);
                                base_script.spawn_item_list_screen[i].id = newId;
                            }
                        };
                    }
                    #endregion
                }
                #endregion

                XGUI.layout_flexspace();

                #region 加载 & 卸载布局 & 预览
                if (sp_spawn_item_list_screen.arraySize > 0)
                {
                    #region 加载 & 卸载布局 
                    if (!sp_is_loaded_layout_screen.boolValue)
                    {
                        #region 加载
                        if (XGUI.layout_button(
                            tooltip: "根据元素列表中的信息重建元素部署布局到LayoutScreen锚点下",
                            tex_release: icon_load_r,
                            tex_press: icon_load_p,
                            tex_gui_color: Color.white,
                            border: new RectOffset(0, 0, 0, 0),
                            width: icon_load_r.width,
                            height: icon_load_r.height))
                        {
                            if (hud_manager == null)
                                return;
                            if (sp_is_loaded_layout_screen.boolValue)
                                return;
                            sp_is_loaded_layout_screen.boolValue = true;
                            sp_is_loaded_layout_screen.serializedObject.ApplyModifiedProperties();

                            for (int i = 0; i < sp_spawn_item_list_screen.arraySize; i++)
                            {
                                LoadSpawnItem_Screen(base_script.spawn_item_list_screen[i]);
                            }

                            // 加载后标脏，持久化自愈回填的 uid
                            sp_spawn_item_list_screen.serializedObject.ApplyModifiedProperties();
                            EditorUtility.SetDirty(base_script);

                            if (base_script.act_on_spawn != null)
                                base_script.act_on_spawn(XHudSpace.屏幕空间);

                            return;
                        }
                        #endregion
                    }
                    else
                    {
                        #region 卸载
                        if (XGUI.layout_button(
                           tooltip: "将当前重建的元素部署布局清空",
                           tex_release: icon_unload_r,
                           tex_press: icon_unload_p,
                           tex_gui_color: Color.white,
                           border: new RectOffset(0, 0, 0, 0),
                           width: icon_unload_r.width,
                           height: icon_unload_r.height))
                        {
                            if (hud_manager == null)
                                return;

                            if (!sp_is_loaded_layout_screen.boolValue)
                                return;
                            sp_is_loaded_layout_screen.boolValue = false;
                            sp_is_loaded_layout_screen.serializedObject.ApplyModifiedProperties();

                            //  停止预览
                            XTween_Preview_Stop();

                            for (int i = 0; i < sp_spawn_item_list_screen.arraySize; i++)
                            {
                                UnLoadSpawnItem_Screen(base_script.spawn_item_list_screen[i]);
                            }

                            if (base_script.act_on_despawn != null)
                                base_script.act_on_despawn(XHudSpace.屏幕空间);
                        }
                        #endregion
                    }
                    #endregion

                    XGUI.layout_flexspace();

                    #region 动画预览        
                    if (sp_is_loaded_layout_screen.boolValue)
                    {
                        #region 预览播放 - 生成
                        if (!IsPreviewing && XGUI.layout_button(
                            tooltip: "预览当前加载的元素的 - 生成 - 动效",
                            tex_release: icon_preview_in_r,
                            tex_press: icon_preview_in_p,
                            tex_gui_color: Color.white,
                            border: new RectOffset(0, 0, 0, 0),
                            width: icon_preview_in_r.width,
                            height: icon_preview_in_r.height))
                        {
                            if (hud_manager == null)
                                return;

                            IsPreviewing = true;
                            LayoutsTweenPreview(XHudSpace.屏幕空间, XHudElementCreateState.Created);
                        }
                        #endregion

                        #region 预览停止 - 生成
                        if (IsPreviewing && XGUI.layout_button(
                            tooltip: "停止预览当前播放的元素的 - 生成 - 动效",
                            tex_release: icon_stoppreview_r,
                            tex_press: icon_stoppreview_p,
                            tex_gui_color: Color.white,
                            border: new RectOffset(0, 0, 0, 0),
                            width: icon_stoppreview_r.width,
                            height: icon_stoppreview_r.height))
                        {
                            if (hud_manager == null)
                                return;

                            IsPreviewing = false;
                            XTween_Preview_Kill(XHudSpace.屏幕空间);
                        }
                        #endregion

                        XGUI.layout_flexspace();

                        #region 预览播放 - 回收
                        if (!IsPreviewing && XGUI.layout_button(
                            tooltip: "预览当前加载的元素的 - 回收 - 动效",
                            tex_release: icon_preview_out_r,
                            tex_press: icon_preview_out_p,
                            tex_gui_color: Color.white,
                            border: new RectOffset(0, 0, 0, 0),
                            width: icon_preview_out_r.width,
                            height: icon_preview_out_r.height))
                        {
                            if (hud_manager == null)
                                return;

                            IsPreviewing = true;
                            LayoutsTweenPreview(XHudSpace.屏幕空间, XHudElementCreateState.Recycled);
                        }
                        #endregion

                        #region 预览停止 - 回收
                        if (IsPreviewing && XGUI.layout_button(
                            tooltip: "停止预览当前播放的元素的 - 回收 - 动效",
                            tex_release: icon_stoppreview_r,
                            tex_press: icon_stoppreview_p,
                            tex_gui_color: Color.white,
                            border: new RectOffset(0, 0, 0, 0),
                            width: icon_stoppreview_r.width,
                            height: icon_stoppreview_r.height))
                        {
                            if (hud_manager == null)
                                return;

                            IsPreviewing = false;
                            XTween_Preview_Kill(XHudSpace.屏幕空间);
                        }
                        #endregion
                    }
                    #endregion
                }
                #endregion                

                XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
                #endregion

                XGUI.layout_space(15);
                element_list_screen.DoLayoutList();
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 元素项 - 世界空间
            base_script.fold_world = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "元素项 - 世界空间",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: base_script.fold_world);

            if (!base_script.fold_world)
            {
                #region 快捷按钮
                XGUI.layout_group_start(
                 type: XGUIContainerType.Horizontal,
                 title_clipping: TextClipping.Clip,
                 absolute_padding: true,
                 absolute_margin: true,
                 margin: new RectOffset(0, 0, 12, 0),
                 padding: new RectOffset(5, 10, 0, 0));

                #region 收集 & 重置
                if (sp_spawn_item_list_world.arraySize <= 0)
                {
                    #region 收集布局      
                    if (XGUI.layout_button(
                        tooltip: "收集当前在LayoutWorld锚点下的所有世界元素部署布局",
                        tex_release: icon_scan_r,
                        tex_press: icon_scan_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: icon_scan_r.width,
                        height: icon_scan_r.height))
                    {
                        if (string.IsNullOrEmpty(sp_library_name.stringValue))
                        {
                            XGUI.dialog(
                               type: XGUIDialogType.警告,
                               windowtitle: "XHud - 布局元素生成器消息",
                               title: "收集元素异常",
                               msg: "请先指定目标元素库！",
                               ok: "明白",
                               PrimaryIndex: 0,
                               usemodal: true,
                               themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                            return;
                        }
                        if (hud_manager == null)
                            return;

                        ClearSpawnItemList_World();
                        HudElementInfo ele_info_w = AnchorElementInfosGet_World();

                        //无效名称列表
                        List<XGUIDialogListDatas> DialogDatas = new List<XGUIDialogListDatas>();

                        //---获取元素
                        for (int i = 0; i < ele_info_w.elements.Count; i++)
                        {
                            string item_name = PrefabUtility.GetCorrespondingObjectFromOriginalSource(ele_info_w.elements[i]).name;
                            if (!FindLibraryItemByInstance(ele_info_w.elements[i], out string hitLib, out string hitUID, out string hitName))
                            {
                                #region 加入非库中元素名称列表
                                string s = item_name;
                                XGUIDialogListDatas data = new XGUIDialogListDatas();
                                data.Title = sp_spawner_indicator.stringValue;
                                data.SubTitle = "非库中元素";
                                data.Message = s;
                                DialogDatas.Add(data);
                                #endregion
                                continue;
                            }
                            else
                            {
                                // 新增：优先使用库中自定义名称
                                if (!string.IsNullOrEmpty(hitName))
                                    item_name = hitName;

                                #region 新建一个生成项，用于快速收集目标元素的基础信息（坐标、缩放、尺寸）
                                XHud_LayoutSpawner_Item item = new XHud_LayoutSpawner_Item();
                                item.spawn_name = item_name;
                                item.element_uid = hitUID;
                                item.library_name = base_script.library_mode == LibraryMode.多元库模式 ? hitLib : null;
                                item.is_enabled = true;
                                item.is_fold = true;
                                item.position = ele_info_w.elements[i].rect_transform.position;
                                item.scale = ele_info_w.elements[i].rect_transform.localScale;
                                item.euler = ele_info_w.elements[i].rect_transform.localEulerAngles;
                                item.size = ele_info_w.elements[i].rect_transform.sizeDelta;
                                item.pivot = ele_info_w.elements[i].rect_transform.pivot;
                                item.anchor_min = ele_info_w.elements[i].rect_transform.anchorMin;
                                item.anchor_max = ele_info_w.elements[i].rect_transform.anchorMax;
                                AddedSpawnItem_World(item);
                                #endregion
                            }
                        }

                        #region 异常提示
                        //如果获取到无效元素名称
                        if (DialogDatas.Count > 0)
                        {
                            string res = XGUI.dialog_listview(
                                datas: DialogDatas.ToArray(),
                                type: XGUIDialogType.警告,
                                windowtitle: "XHud - 布局元素生成器消息",
                                title: "元素物体异常",
                                msg: $"检测到以下不合理的物体，原因：未加入到元素池中。解决方法：如果您想让这些元素被记录到部署布局中，请将其加入元素池！",
                                ok: "明白",
                                PrimaryIndex: 0,
                                usemodal: true,
                                themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                        }

                        if (ele_info_w.notPrefabsList.Count > 0)
                        {
                            DialogDatas.Clear();
                            for (int w = 0; w < ele_info_w.notPrefabsList.Count; w++)
                            {
                                string s = ele_info_w.notPrefabsList[w].name;

                                #region 加入非预制体元素名称列表
                                XGUIDialogListDatas data = new XGUIDialogListDatas();
                                data.Title = sp_spawner_indicator.stringValue;
                                data.SubTitle = "非预制体元素";
                                data.Message = s;
                                DialogDatas.Add(data);
                                #endregion
                            }

                            string res = XGUI.dialog_listview(
                               datas: DialogDatas.ToArray(),
                               type: XGUIDialogType.警告,
                               windowtitle: "XHud - 布局元素生成器消息",
                               title: "元素物体异常",
                               msg: $"检测到以下不合理的物体，原因：物体虽然是HudElement元素但未将其制成预制体。解决方法：如果您想让这些元素被记录到部署布局中，请将其制成预制体！",
                               ok: "明白",
                               cancel: "选中",
                               PrimaryIndex: 0,
                               usemodal: true,
                               themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                            if (res == "选中")
                            {
                                Selection.objects = ele_info_w.notPrefabsList.ToArray();
                            }
                        }
                        #endregion

                        #region 记录后处理元素方式
                        if (ele_info_w.elements.Count > 0)
                        {
                            string res = XGUI.dialog(
                                type: XGUIDialogType.确认,
                                windowtitle: "XHud - 布局元素生成器消息",
                                title: "元素物体处理",
                                msg: $"如何处理场景中被记录的世界UI的HudElement元素物体？",
                                ok: "不做处理",
                                cancel: "移除",
                                alt: "隐藏",
                                PrimaryIndex: 0,
                                themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                            if (res == "不做处理")
                            {

                            }
                            else if (res == "移除")
                            {
                                for (int i = 0; i < ele_info_w.elements.Count; i++)
                                {
                                    Undo.DestroyObjectImmediate(ele_info_w.elements[i].gameObject);
                                }
                            }
                            else if (res == "隐藏")
                            {
                                for (int i = 0; i < ele_info_w.elements.Count; i++)
                                {
                                    Undo.RecordObject(ele_info_w.elements[i].gameObject, "HideOriginalElementPrefab_" + i);
                                    ele_info_w.elements[i].gameObject.SetActive(false);
                                }
                            }
                        }
                        else
                        {
                            XGUI.dialog(
                                type: XGUIDialogType.确认,
                                windowtitle: "XHud - 布局元素生成器消息",
                                title: "元素物体收集",
                                msg: $"未发现<color={hexcol}> 世界根节点 </color>下存在任何 XHud 元素物体！",
                                ok: "明白",
                                PrimaryIndex: 0);
                        }
                        #endregion
                        return;
                    }
                    #endregion
                }
                else
                {
                    #region 清空布局    
                    if (XGUI.layout_button(
                        tooltip: "将当前的元素部署布局列表清空",
                        tex_release: icon_reset_r,
                        tex_press: icon_reset_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: icon_reset_r.width,
                        height: icon_reset_r.height))
                    {
                        EditorApplication.delayCall += () =>
                        {
                            string res = XGUI.dialog(
                                type: XGUIDialogType.警告,
                                windowtitle: "XHud - 布局元素生成器消息",
                                title: "重置元素列表",
                                msg: "确定要将元素列表重置吗？您将丢失当前的所有元素记录和他们的延迟（生成 & 回收）参数！",
                                ok: "重置",
                                cancel: "暂不",
                                PrimaryIndex: 0,
                                usemodal: true,
                                themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                            if (res == "暂不")
                                return;

                            if (hud_manager == null)
                                return;

                            sp_is_loaded_layout_world.boolValue = false;
                            sp_is_loaded_layout_world.serializedObject.ApplyModifiedProperties();
                            for (int i = 0; i < sp_spawn_item_list_world.arraySize; i++)
                            {
                                UnLoadSpawnItem_World(base_script.spawn_item_list_world[i]);
                            }
                            ClearSpawnItemList_World();
                        };
                    }
                    #endregion

                    XGUI.layout_flexspace();

                    #region 重新生成ID
                    if (XGUI.layout_button(
                      tooltip: "为当前的元素列表中的项重新创建ID",
                      tex_release: icon_recreate_id_r,
                      tex_press: icon_recreate_id_p,
                      tex_gui_color: Color.white,
                      border: new RectOffset(0, 0, 0, 0),
                      width: icon_recreate_id_r.width,
                      height: icon_recreate_id_r.height))
                    {
                        EditorApplication.delayCall += () =>
                        {
                            string res = XGUI.dialog(
                                type: XGUIDialogType.警告,
                                windowtitle: "XHud - 布局元素生成器消息",
                                title: "元素ID刷新",
                                msg: "确定要将元素列表中的所有项的ID进行重新生成吗？您将丢失所有元素的当前ID！我们并不建议您这么做，因为会导致您其他脚本在获取元素时发生\" 无法找到目标元素 \"的异常！",
                                ok: "重新生成",
                                cancel: "暂不",
                                PrimaryIndex: 0,
                                usemodal: true,
                                themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));

                            if (res == "暂不")
                                return;

                            Undo.RecordObject(base_script, "刷新所有元素项ID");

                            var existingIds = new HashSet<string>(CollectIDs(XHudSpace.世界空间));

                            for (int i = 0; i < base_script.spawn_item_list_world.Count; i++)
                            {
                                string newId = XGUI_Utilitys.GenerateUniqueId(existingIds.ToArray());
                                existingIds.Add(newId);
                                base_script.spawn_item_list_world[i].id = newId;
                            }
                        };
                    }
                    #endregion
                }
                #endregion

                XGUI.layout_flexspace();

                #region 加载 & 卸载布局 & 预览
                if (sp_spawn_item_list_world.arraySize > 0)
                {
                    #region 加载 & 卸载布局 
                    if (!sp_is_loaded_layout_world.boolValue)
                    {
                        #region 加载
                        if (XGUI.layout_button(
                            tooltip: "根据元素列表中的信息重建元素部署布局到LayoutScreen锚点下",
                            tex_release: icon_load_r,
                            tex_press: icon_load_p,
                            tex_gui_color: Color.white,
                            border: new RectOffset(0, 0, 0, 0),
                            width: icon_load_r.width,
                            height: icon_load_r.height))
                        {
                            if (hud_manager == null)
                                return;
                            if (sp_is_loaded_layout_world.boolValue)
                                return;
                            sp_is_loaded_layout_world.boolValue = true;
                            sp_is_loaded_layout_world.serializedObject.ApplyModifiedProperties();

                            for (int i = 0; i < sp_spawn_item_list_world.arraySize; i++)
                            {
                                LoadSpawnItem_World(base_script.spawn_item_list_world[i]);
                            }

                            // 加载后标脏，持久化自愈回填的 uid
                            sp_spawn_item_list_world.serializedObject.ApplyModifiedProperties();
                            EditorUtility.SetDirty(base_script);

                            if (base_script.act_on_spawn != null)
                                base_script.act_on_spawn(XHudSpace.世界空间);

                            return;
                        }
                        #endregion
                    }
                    else
                    {
                        #region 卸载
                        if (XGUI.layout_button(
                           tooltip: "将当前重建的元素部署布局清空",
                           tex_release: icon_unload_r,
                           tex_press: icon_unload_p,
                           tex_gui_color: Color.white,
                           border: new RectOffset(0, 0, 0, 0),
                           width: icon_unload_r.width,
                           height: icon_unload_r.height))
                        {
                            if (hud_manager == null)
                                return;

                            if (!sp_is_loaded_layout_world.boolValue)
                                return;
                            sp_is_loaded_layout_world.boolValue = false;
                            sp_is_loaded_layout_world.serializedObject.ApplyModifiedProperties();

                            //  停止预览
                            XTween_Preview_Stop();

                            for (int i = 0; i < sp_spawn_item_list_world.arraySize; i++)
                            {
                                UnLoadSpawnItem_World(base_script.spawn_item_list_world[i]);
                            }

                            if (base_script.act_on_despawn != null)
                                base_script.act_on_despawn(XHudSpace.世界空间);
                        }
                        #endregion
                    }
                    #endregion

                    XGUI.layout_flexspace();

                    #region 动画预览        
                    if (sp_is_loaded_layout_world.boolValue)
                    {
                        #region 预览播放 - 生成
                        if (!IsPreviewing && XGUI.layout_button(
                            tooltip: "预览当前加载的元素的 - 生成 - 动效",
                            tex_release: icon_preview_in_r,
                            tex_press: icon_preview_in_p,
                            tex_gui_color: Color.white,
                            border: new RectOffset(0, 0, 0, 0),
                            width: icon_preview_in_r.width,
                            height: icon_preview_in_r.height))
                        {
                            if (hud_manager == null)
                                return;

                            IsPreviewing = true;
                            LayoutsTweenPreview(XHudSpace.世界空间, XHudElementCreateState.Created);
                        }
                        #endregion

                        #region 预览停止 - 生成
                        if (IsPreviewing && XGUI.layout_button(
                            tooltip: "停止预览当前播放的元素的 - 生成 - 动效",
                            tex_release: icon_stoppreview_r,
                            tex_press: icon_stoppreview_p,
                            tex_gui_color: Color.white,
                            border: new RectOffset(0, 0, 0, 0),
                            width: icon_stoppreview_r.width,
                            height: icon_stoppreview_r.height))
                        {
                            if (hud_manager == null)
                                return;

                            IsPreviewing = false;
                            XTween_Preview_Kill(XHudSpace.世界空间);
                        }
                        #endregion

                        XGUI.layout_flexspace();

                        #region 预览播放 - 回收
                        if (!IsPreviewing && XGUI.layout_button(
                            tooltip: "预览当前加载的元素的 - 回收 - 动效",
                            tex_release: icon_preview_out_r,
                            tex_press: icon_preview_out_p,
                            tex_gui_color: Color.white,
                            border: new RectOffset(0, 0, 0, 0),
                            width: icon_preview_out_r.width,
                            height: icon_preview_out_r.height))
                        {
                            if (hud_manager == null)
                                return;

                            IsPreviewing = true;
                            LayoutsTweenPreview(XHudSpace.世界空间, XHudElementCreateState.Recycled);
                        }
                        #endregion

                        #region 预览停止 - 回收
                        if (IsPreviewing && XGUI.layout_button(
                            tooltip: "停止预览当前播放的元素的 - 回收 - 动效",
                            tex_release: icon_stoppreview_r,
                            tex_press: icon_stoppreview_p,
                            tex_gui_color: Color.white,
                            border: new RectOffset(0, 0, 0, 0),
                            width: icon_stoppreview_r.width,
                            height: icon_stoppreview_r.height))
                        {
                            if (hud_manager == null)
                                return;

                            IsPreviewing = false;
                            XTween_Preview_Kill(XHudSpace.世界空间);
                        }
                        #endregion
                    }
                    #endregion
                }
                #endregion                

                XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
                #endregion

                XGUI.layout_space(15);
                element_list_world.DoLayoutList();
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
                    tex_release: icon_save_r,
                    tex_press: icon_save_p,
                    tex_gui_color: Color.white,
                    border: new RectOffset(0, 0, 0, 0),
                    width: icon_save_r.width,
                    height: icon_save_r.height))
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
                  tex_release: icon_locate_r,
                  tex_press: icon_locate_p,
                  tex_gui_color: Color.white,
                  border: new RectOffset(0, 0, 0, 0),
                  width: icon_locate_r.width,
                  height: icon_locate_r.height))
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
                  tex_release: icon_reset_r,
                  tex_press: icon_reset_p,
                  tex_gui_color: Color.white,
                  border: new RectOffset(0, 0, 0, 0),
                  width: icon_reset_r.width,
                  height: icon_reset_r.height))
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
                    tex_release: icon_save_r,
                    tex_press: icon_save_p,
                    tex_gui_color: Color.white,
                    border: new RectOffset(0, 0, 0, 0),
                    width: icon_save_r.width,
                    height: icon_save_r.height))
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
                  tex_release: icon_locate_r,
                  tex_press: icon_locate_p,
                  tex_gui_color: Color.white,
                  border: new RectOffset(0, 0, 0, 0),
                  width: icon_locate_r.width,
                  height: icon_locate_r.height))
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
                  tex_release: icon_reset_r,
                  tex_press: icon_reset_p,
                  tex_gui_color: Color.white,
                  border: new RectOffset(0, 0, 0, 0),
                  width: icon_reset_r.width,
                  height: icon_reset_r.height))
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
                SerializedProperty sp_CreateArgs = serializedObject.FindProperty(nameof(XHud_LayoutSpawner.motion_create_args));
                SerializedProperty sp_RecycleArgs = serializedObject.FindProperty(nameof(XHud_LayoutSpawner.motion_recycle_args));

                // 创建右键菜单
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("V (折叠元素列表)"), false, () =>
                {
                    sp_fold_screen.boolValue = false;
                    sp_fold_world.boolValue = false;

                    sp_fold_screen.serializedObject.ApplyModifiedProperties();
                    sp_fold_world.serializedObject.ApplyModifiedProperties();
                });
                menu.AddItem(new GUIContent("B (展开元素列表)"), false, () =>
                {
                    sp_fold_screen.boolValue = true;
                    sp_fold_world.boolValue = true;

                    sp_fold_screen.serializedObject.ApplyModifiedProperties();
                    sp_fold_world.serializedObject.ApplyModifiedProperties();
                });
                menu.AddSeparator("");
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

        #region 辅助
        /// <summary>
        /// 主要在编辑器模式下用于获取所有锚点下的物体进行集控 - 屏幕级别
        /// </summary>
        /// <returns></returns>
        public HudElementInfo AnchorElementInfosGet_Screen()
        {
            XHud_Module_Element[] trans = hud_manager.cv_hudcanvas_screen_anchor.GetComponentsInChildren<XHud_Module_Element>();
            HudElementInfo ElementInfos = new HudElementInfo();
            for (int i = 0; i < trans.Length; i++)
            {
                if (PrefabUtility.IsPartOfPrefabInstance(trans[i]))
                {
                    ElementInfos.elements.Add(trans[i]);
                }
                else
                {
                    ElementInfos.notPrefabsList.Add(trans[i].gameObject);
                }
            }
            return ElementInfos;
        }
        /// <summary>
        /// 主要在编辑器模式下用于获取所有锚点下的物体进行集控 - 世界级别
        /// </summary>
        /// <returns></returns>
        public HudElementInfo AnchorElementInfosGet_World()
        {
            XHud_Module_Element[] trans = hud_manager.cv_hudcanvas_world.GetComponentsInChildren<XHud_Module_Element>();
            HudElementInfo ElementInfos = new HudElementInfo();
            for (int i = 0; i < trans.Length; i++)
            {
                if (PrefabUtility.IsPartOfPrefabInstance(trans[i]))
                {
                    ElementInfos.elements.Add(trans[i]);
                }
                else
                {
                    ElementInfos.notPrefabsList.Add(trans[i].gameObject);
                }
            }
            return ElementInfos;
        }
        /// <summary>
        /// 根据场景元素实例，在所有已装配的库中查找其归属条目。
        /// <para/> 识别依据：实例的原始预制体引用 == 库条目 target（精确匹配）。
        /// <para/> 变体/引用回溯失败时退化到按名字匹配。
        /// </summary>
        private bool FindLibraryItemByInstance(XHud_Module_Element sceneEle, out string hitLib, out string hitUID, out string hitName)
        {
            hitLib = null;
            hitUID = null;
            hitName = null;
            if (hud_manager == null || sceneEle == null) return false;

            bool multiMode = base_script.library_mode == LibraryMode.多元库模式;

            // 1) 取原始预制体源（变体兜底）
            Object src = PrefabUtility.GetCorrespondingObjectFromOriginalSource(sceneEle);
            if (src == null)
                src = PrefabUtility.GetCorrespondingObjectFromSource(sceneEle);

            // 2) 引用精确匹配
            if (src != null)
            {
                string firstName = null, firstLib = null, firstUID = null;
                int matchCount = 0;

                for (int l = 0; l < hud_manager.ele_library_elements.Count; l++)
                {
                    var lib = hud_manager.ele_library_elements[l];
                    if (lib == null) continue;

                    // 单库模式：只查组件级 library_name 指定的库
                    if (!multiMode && lib.library_name != sp_library_name.stringValue)
                        continue;

                    for (int i = 0; i < lib.library_elements.Count; i++)
                    {
                        var entry = lib.library_elements[i];
                        if (entry == null || entry.target == null) continue;

                        if (entry.target == src)
                        {
                            matchCount++;
                            if (firstUID == null)
                            {
                                firstName = entry.name;
                                firstLib = lib.library_name;
                                firstUID = entry.element_uid;
                            }
                        }
                    }
                }

                if (matchCount == 1)
                {
                    hitLib = firstLib;
                    hitUID = firstUID;
                    hitName = firstName;
                    return true;
                }
                if (matchCount > 1)
                {
                    XGUI_Utilitys.Console("XHud - 布局元素生成器通知", $"元素 \"{firstName}\" 的预制体同时存在于 {matchCount} 个元素库中，本次收集归属库 \"{firstLib}\"，请检查是否存在重复入库", XGUIMsgState.警告);
                    hitLib = firstLib;
                    hitUID = firstUID;
                    hitName = firstName;
                    return true;
                }
            }

            // 3) 名字兜底（引用无法回溯或未命中）
            string eleName = src != null ? ((Component)src).name : sceneEle.name;

            string nameFirstLib = null, nameFirstUID = null;
            int nameMatchCount = 0;

            for (int l = 0; l < hud_manager.ele_library_elements.Count; l++)
            {
                var lib = hud_manager.ele_library_elements[l];
                if (lib == null) continue;
                if (!multiMode && lib.library_name != sp_library_name.stringValue) continue;

                if (lib.ElementLibrary_IsExist(eleName))
                {
                    nameMatchCount++;
                    if (nameFirstUID == null)
                    {
                        nameFirstLib = lib.library_name;
                        nameFirstUID = lib.GetUID_WithName(eleName);
                    }
                }
            }

            if (nameMatchCount >= 1)
            {
                if (nameMatchCount > 1)
                    XGUI_Utilitys.Console("XHud - 布局元素生成器通知", $"名字 \"{eleName}\" 在 {nameMatchCount} 个库中重名，取首个命中（库 \"{nameFirstLib}\"）。若并非预期，请检查库配置", XGUIMsgState.警告);
                hitLib = nameFirstLib;
                hitUID = nameFirstUID;
                hitName = eleName;
                return true;
            }

            return false;
        }
        public string[] CollectIDs(XHudSpace space)
        {
            List<string> list = new List<string>();

            for (int i = 0; i < (space == XHudSpace.屏幕空间 ? base_script.spawn_item_list_screen.Count : base_script.spawn_item_list_world.Count); i++)
            {
                list.Add(space == XHudSpace.屏幕空间 ? base_script.spawn_item_list_screen[i].id : base_script.spawn_item_list_world[i].id);
            }

            return list.ToArray();
        }
        /// <summary>
        /// 清空生成项列表 - 屏幕 Screen
        /// </summary>
        private void ClearSpawnItemList_Screen()
        {
            sp_spawn_item_list_screen.ClearArray();
            sp_spawn_item_list_screen.serializedObject.ApplyModifiedProperties();
        }
        /// <summary>
        /// 添加一个生成项 - 屏幕 Screen
        /// </summary>
        /// <param name="item"></param>
        private void AddedSpawnItem_Screen(XHud_LayoutSpawner_Item item)
        {
            SerializedProperty sp_item;
            if (sp_spawn_item_list_screen.arraySize <= 0)
            {
                sp_spawn_item_list_screen.InsertArrayElementAtIndex(0);
                sp_item = sp_spawn_item_list_screen.GetArrayElementAtIndex(0);
            }
            else
            {
                sp_spawn_item_list_screen.InsertArrayElementAtIndex(sp_spawn_item_list_screen.arraySize);
                sp_item = sp_spawn_item_list_screen.GetArrayElementAtIndex(sp_spawn_item_list_screen.arraySize - 1);
            }

            SerializedProperty sp_spawn_name = sp_item.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.spawn_name));
            SerializedProperty sp_element_uid = sp_item.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.element_uid));
            SerializedProperty sp_library_name = sp_item.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.library_name));
            SerializedProperty sp_indicator = sp_item.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.indicator));
            SerializedProperty sp_id = sp_item.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.id));
            SerializedProperty sp_delay_spawn = sp_item.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.delay_spawn));
            SerializedProperty sp_position = sp_item.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.position));
            SerializedProperty sp_euler = sp_item.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.euler));
            SerializedProperty sp_pivot = sp_item.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.pivot));
            SerializedProperty sp_anchor_min = sp_item.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.anchor_min));
            SerializedProperty sp_anchor_max = sp_item.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.anchor_max));
            SerializedProperty sp_offset = sp_item.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.offset));
            SerializedProperty sp_scale = sp_item.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.scale));
            SerializedProperty sp_size = sp_item.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.size));
            SerializedProperty sp_auto_in = sp_item.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.auto_in));
            SerializedProperty sp_motion_mode = sp_item.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.motion_mode));
            SerializedProperty sp_is_fold = sp_item.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.is_fold));
            SerializedProperty sp_is_enabled = sp_item.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.is_enabled));

            #region SerializedProperty - CreateArgs
            SerializedProperty sp_motion_create_args = sp_item.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.motion_create_args));
            SerializedProperty sp_creator_anchor = sp_motion_create_args.FindPropertyRelative(nameof(Motion_Creator.anchor));
            SerializedProperty sp_creator_Movement = sp_motion_create_args.FindPropertyRelative($"{nameof(Motion_Creator.Movement)}.{nameof(MotionNode_Movement.Movement)}");
            SerializedProperty sp_creator_Movement_Distance = sp_motion_create_args.FindPropertyRelative($"{nameof(Motion_Creator.Movement)}.{nameof(MotionNode_Movement.Distance)}");
            SerializedProperty sp_creator_Movement_Duration = sp_motion_create_args.FindPropertyRelative($"{nameof(Motion_Creator.Movement)}.{nameof(MotionNode_Movement.Duration)}");
            SerializedProperty sp_creator_Movement_Delay = sp_motion_create_args.FindPropertyRelative($"{nameof(Motion_Creator.Movement)}.{nameof(MotionNode_Movement.Delay)}");
            SerializedProperty sp_creator_Movement_Ease = sp_motion_create_args.FindPropertyRelative($"{nameof(Motion_Creator.Movement)}.{nameof(MotionNode_Movement.Ease)}");
            SerializedProperty sp_creator_Rotation = sp_motion_create_args.FindPropertyRelative($"{nameof(Motion_Creator.Rotation)}.{nameof(MotionNode_Rotation.Rotation)}");
            SerializedProperty sp_creator_Rotation_Degree = sp_motion_create_args.FindPropertyRelative($"{nameof(Motion_Creator.Rotation)}.{nameof(MotionNode_Rotation.Degree)}");
            SerializedProperty sp_creator_Rotation_Duration = sp_motion_create_args.FindPropertyRelative($"{nameof(Motion_Creator.Rotation)}.{nameof(MotionNode_Rotation.Duration)}");
            SerializedProperty sp_creator_Rotation_Delay = sp_motion_create_args.FindPropertyRelative($"{nameof(Motion_Creator.Rotation)}.{nameof(MotionNode_Rotation.Delay)}");
            SerializedProperty sp_creator_Rotation_Ease = sp_motion_create_args.FindPropertyRelative($"{nameof(Motion_Creator.Rotation)}.{nameof(MotionNode_Rotation.Ease)}");
            SerializedProperty sp_creator_Alpha_Duration = sp_motion_create_args.FindPropertyRelative($"{nameof(Motion_Creator.Alpha)}.{nameof(MotionNode_Alpha.Duration)}");
            SerializedProperty sp_creator_Alpha_Delay = sp_motion_create_args.FindPropertyRelative($"{nameof(Motion_Creator.Alpha)}.{nameof(MotionNode_Alpha.Delay)}");
            SerializedProperty sp_creator_Alpha_Ease = sp_motion_create_args.FindPropertyRelative($"{nameof(Motion_Creator.Alpha)}.{nameof(MotionNode_Alpha.Ease)}");
            #endregion

            #region SerializedProperty - RecycleArgs
            SerializedProperty sp_motion_recycle_args = sp_item.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.motion_recycle_args));
            SerializedProperty sp_recycle_Movement = sp_motion_recycle_args.FindPropertyRelative($"{nameof(Motion_Recycler.Movement)}.{nameof(MotionNode_Movement.Movement)}");
            SerializedProperty sp_recycle_Movement_Distance = sp_motion_recycle_args.FindPropertyRelative($"{nameof(Motion_Recycler.Movement)}.{nameof(MotionNode_Movement.Distance)}");
            SerializedProperty sp_recycle_Movement_Duration = sp_motion_recycle_args.FindPropertyRelative($"{nameof(Motion_Recycler.Movement)}.{nameof(MotionNode_Movement.Duration)}");
            SerializedProperty sp_recycle_Movement_Delay = sp_motion_recycle_args.FindPropertyRelative($"{nameof(Motion_Recycler.Movement)}.{nameof(MotionNode_Movement.Delay)}");
            SerializedProperty sp_recycle_Movement_Ease = sp_motion_recycle_args.FindPropertyRelative($"{nameof(Motion_Recycler.Movement)}.{nameof(MotionNode_Movement.Ease)}");
            SerializedProperty sp_recycle_Rotation = sp_motion_recycle_args.FindPropertyRelative($"{nameof(Motion_Recycler.Rotation)}.{nameof(MotionNode_Rotation.Rotation)}");
            SerializedProperty sp_recycle_Rotation_Degree = sp_motion_recycle_args.FindPropertyRelative($"{nameof(Motion_Recycler.Rotation)}.{nameof(MotionNode_Rotation.Degree)}");
            SerializedProperty sp_recycle_Rotation_Duration = sp_motion_recycle_args.FindPropertyRelative($"{nameof(Motion_Recycler.Rotation)}.{nameof(MotionNode_Rotation.Duration)}");
            SerializedProperty sp_recycle_Rotation_Delay = sp_motion_recycle_args.FindPropertyRelative($"{nameof(Motion_Recycler.Rotation)}.{nameof(MotionNode_Rotation.Delay)}");
            SerializedProperty sp_recycle_Rotation_Ease = sp_motion_recycle_args.FindPropertyRelative($"{nameof(Motion_Recycler.Rotation)}.{nameof(MotionNode_Rotation.Ease)}");
            SerializedProperty sp_recycle_Alpha_Duration = sp_motion_recycle_args.FindPropertyRelative($"{nameof(Motion_Recycler.Alpha)}.{nameof(MotionNode_Alpha.Duration)}");
            SerializedProperty sp_recycle_Alpha_Delay = sp_motion_recycle_args.FindPropertyRelative($"{nameof(Motion_Recycler.Alpha)}.{nameof(MotionNode_Alpha.Delay)}");
            SerializedProperty sp_recycle_Alpha_Ease = sp_motion_recycle_args.FindPropertyRelative($"{nameof(Motion_Recycler.Alpha)}.{nameof(MotionNode_Alpha.Ease)}");
            #endregion

            #region CreateArgs
            sp_creator_anchor.enumValueIndex = (int)item.motion_create_args.anchor;

            #region Movement
            sp_creator_Movement.enumValueIndex = (int)item.motion_create_args.Movement.Movement;
            sp_creator_Movement_Distance.floatValue = item.motion_create_args.Movement.Distance;
            sp_creator_Movement_Duration.floatValue = item.motion_create_args.Movement.Duration;
            sp_creator_Movement_Delay.floatValue = item.motion_create_args.Movement.Delay;
            sp_creator_Movement_Ease.enumValueIndex = (int)item.motion_create_args.Movement.Ease;
            #endregion

            #region Rotation
            sp_creator_Rotation.enumValueIndex = (int)item.motion_create_args.Rotation.Rotation;
            sp_creator_Rotation_Degree.floatValue = item.motion_create_args.Rotation.Degree;
            sp_creator_Rotation_Duration.floatValue = item.motion_create_args.Rotation.Duration;
            sp_creator_Rotation_Delay.floatValue = item.motion_create_args.Rotation.Delay;
            sp_creator_Rotation_Ease.enumValueIndex = (int)item.motion_create_args.Rotation.Ease;
            #endregion

            #region Alpha
            sp_creator_Alpha_Duration.floatValue = item.motion_create_args.Alpha.Duration;
            sp_creator_Alpha_Delay.floatValue = item.motion_create_args.Alpha.Delay;
            sp_creator_Alpha_Ease.enumValueIndex = (int)item.motion_create_args.Alpha.Ease;
            #endregion
            #endregion

            #region RecycleArgs
            #region Movement
            sp_recycle_Movement.enumValueIndex = (int)item.motion_recycle_args.Movement.Movement;
            sp_recycle_Movement_Distance.floatValue = item.motion_recycle_args.Movement.Distance;
            sp_recycle_Movement_Duration.floatValue = item.motion_recycle_args.Movement.Duration;
            sp_recycle_Movement_Delay.floatValue = item.motion_recycle_args.Movement.Delay;
            sp_recycle_Movement_Ease.enumValueIndex = (int)item.motion_recycle_args.Movement.Ease;
            #endregion

            #region Rotation
            sp_recycle_Rotation.enumValueIndex = (int)item.motion_recycle_args.Rotation.Rotation;
            sp_recycle_Rotation_Degree.floatValue = item.motion_recycle_args.Rotation.Degree;
            sp_recycle_Rotation_Duration.floatValue = item.motion_recycle_args.Rotation.Duration;
            sp_recycle_Rotation_Delay.floatValue = item.motion_recycle_args.Rotation.Delay;
            sp_recycle_Rotation_Ease.enumValueIndex = (int)item.motion_recycle_args.Rotation.Ease;
            #endregion

            #region Alpha
            sp_recycle_Alpha_Duration.floatValue = item.motion_recycle_args.Alpha.Duration;
            sp_recycle_Alpha_Delay.floatValue = item.motion_recycle_args.Alpha.Delay;
            sp_recycle_Alpha_Ease.enumValueIndex = (int)item.motion_recycle_args.Alpha.Ease;
            #endregion
            #endregion

            sp_spawn_name.stringValue = item.spawn_name;
            sp_element_uid.stringValue = item.element_uid ?? string.Empty;
            sp_library_name.stringValue = item.library_name ?? string.Empty;
            sp_indicator.stringValue = "Indicator_" + item.spawn_name;
            sp_id.stringValue = XGUI_Utilitys.GenerateUniqueId(CollectIDs(XHudSpace.屏幕空间));
            sp_delay_spawn.floatValue = item.delay_spawn;
            sp_position.vector3Value = item.position;
            sp_offset.vector3Value = item.offset;
            sp_scale.vector3Value = item.scale;
            sp_euler.vector3Value = item.euler;
            sp_pivot.vector2Value = item.pivot;
            sp_anchor_min.vector2Value = item.anchor_min;
            sp_anchor_max.vector2Value = item.anchor_max;
            sp_size.vector2Value = item.size;
            sp_auto_in.stringValue = item.auto_in;
            sp_motion_mode.stringValue = item.motion_mode;
            sp_is_fold.boolValue = item.is_fold;
            sp_is_enabled.boolValue = item.is_enabled;

            sp_spawn_item_list_screen.serializedObject.ApplyModifiedProperties();
        }
        /// <summary>
        /// 载入一个生成元素 - 屏幕 Screen
        /// </summary>
        /// <param name="item"></param>
        private void LoadSpawnItem_Screen(XHud_LayoutSpawner_Item item)
        {
            string hex_col = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.themes_get_color(ThemeColorType.primary), true);

            //  统一分派取库名（禁止裸读 library_name）
            string libName = base_script.GetItemLibraryName(item);
            XHud_Library_Element targetLib = hud_manager.hm_ElementLibrary_GetTargetLibrary(libName);

            if (targetLib == null)
            {
                XGUI_Utilitys.Console(
                    "XHud - 布局元素生成器通知",
                    $"未找到元素库： <b><color={hex_col}>{libName}</color></b>！请检查XHud 管理器中是否正确 配置 / 加入 了<b><color={hex_col}> {libName} </color></b>元素库？",
                    XGUIMsgState.通知);
                return;
            }

            if (!item.is_enabled)
                return;

            //  UID 优先定位
            XHud_Module_Element ele = null;
            if (!string.IsNullOrEmpty(item.element_uid))
                ele = targetLib.GetTargetElement_WithUID(item.element_uid);

            //  名字兜底 + 自愈：UID 没命中但名字命中 → 回填 UID
            if (ele == null && !string.IsNullOrEmpty(item.spawn_name))
            {
                ele = targetLib.ElementsLibrary_GetTargetElement(item.spawn_name);
                if (ele != null)
                {
                    string uid = targetLib.GetUID_WithName(item.spawn_name);
                    if (!string.IsNullOrEmpty(uid))
                        item.element_uid = uid;
                }
            }

            //  两者都未命中 → 报元素丢失，不崩
            if (ele == null)
            {
                XGUI_Utilitys.Console(
                    "XHud - 布局元素生成器通知",
                    $"元素丢失：库 <b><color={hex_col}>{libName}</color></b> 中，UID=<b><color={hex_col}>{item.element_uid}</color></b> 与 名称=<b><color={hex_col}>{item.spawn_name}</color></b> 均未命中！",
                    XGUIMsgState.通知);
                return;
            }

            XHud_Module_Element Element = (XHud_Module_Element)PrefabUtility.InstantiatePrefab(ele);
            // 这里的判断是为了更符合直觉的锚点选用，如果用户切换为  "元素自身动效" 那么生成的元素锚点则跟随元素自身的锚点设置，如果用户切换为  "列表项动效" 那么就是根据用户扫描的锚点为基准
            RectTransform Root = hud_manager.hm_ScreenElement_GetAnchored_RectTransform(item.motion_mode == "元素自身动效" ? Element.motion_create_args.anchor : item.motion_mode == "列表项动效" ? item.motion_create_args.anchor : base_script.motion_create_args.anchor);
            Element.rect_transform.SetParent(Root);
            Element.rect_transform.anchoredPosition3D = item.position + item.offset;
            Element.rect_transform.localScale = item.scale;
            Element.rect_transform.localEulerAngles = item.euler;
            Element.rect_transform.pivot = item.pivot;
            Element.rect_transform.anchorMin = item.anchor_min;
            Element.rect_transform.anchorMax = item.anchor_max;
            Element.element_AlphaSet(1);

            Element.rect_transform.sizeDelta = item.size;
            item.spawned_element_node = new XHudElementNode();
            item.spawned_element_node.element = Element;
            item.spawned_element_node.id = 0;
            item.spawned_element_node.indicator = Element.indicator;
            item.spawned_element_node.module_name = Element.name;
        }
        /// <summary>
        /// 卸载一个生成元素 - 屏幕 Screen
        /// </summary>
        /// <param name="item"></param>
        private void UnLoadSpawnItem_Screen(XHud_LayoutSpawner_Item item)
        {
            if (item == null || item.spawned_element_node == null)
                return;
            if (item.spawned_element_node.element == null)
            {
                item.spawned_element_node = null;
                return;
            }
            Undo.DestroyObjectImmediate(item.spawned_element_node.element.gameObject);
            item.spawned_element_node = null;
        }
        /// <summary>
        /// 清空生成项列表 - 世界 World
        /// </summary>
        private void ClearSpawnItemList_World()
        {
            sp_spawn_item_list_world.ClearArray();
            sp_spawn_item_list_world.serializedObject.ApplyModifiedProperties();
        }
        /// <summary>
        /// 添加一个生成项 - 世界 World
        /// </summary>
        /// <param name="item"></param>
        private void AddedSpawnItem_World(XHud_LayoutSpawner_Item item)
        {
            SerializedProperty sp_item;
            if (sp_spawn_item_list_world.arraySize <= 0)
            {
                sp_spawn_item_list_world.InsertArrayElementAtIndex(0);
                sp_item = sp_spawn_item_list_world.GetArrayElementAtIndex(0);
            }
            else
            {
                sp_spawn_item_list_world.InsertArrayElementAtIndex(sp_spawn_item_list_world.arraySize);
                sp_item = sp_spawn_item_list_world.GetArrayElementAtIndex(sp_spawn_item_list_world.arraySize - 1);
            }

            SerializedProperty sp_spawn_name = sp_item.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.spawn_name));
            SerializedProperty sp_element_uid = sp_item.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.element_uid));
            SerializedProperty sp_library_name = sp_item.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.library_name));
            SerializedProperty sp_indicator = sp_item.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.indicator));
            SerializedProperty sp_id = sp_item.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.id));
            SerializedProperty sp_delay_spawn = sp_item.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.delay_spawn));
            SerializedProperty sp_position = sp_item.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.position));
            SerializedProperty sp_euler = sp_item.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.euler));
            SerializedProperty sp_pivot = sp_item.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.pivot));
            SerializedProperty sp_anchor_min = sp_item.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.anchor_min));
            SerializedProperty sp_anchor_max = sp_item.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.anchor_max));
            SerializedProperty sp_offset = sp_item.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.offset));
            SerializedProperty sp_scale = sp_item.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.scale));
            SerializedProperty sp_size = sp_item.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.size));
            SerializedProperty sp_auto_in = sp_item.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.auto_in));
            SerializedProperty sp_motion_mode = sp_item.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.motion_mode));
            SerializedProperty sp_is_fold = sp_item.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.is_fold));
            SerializedProperty sp_is_enabled = sp_item.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.is_enabled));

            #region SerializedProperty - CreateArgs
            SerializedProperty sp_motion_create_args = sp_item.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.motion_create_args));
            SerializedProperty sp_creator_anchor = sp_motion_create_args.FindPropertyRelative(nameof(Motion_Creator.anchor));
            SerializedProperty sp_creator_Movement = sp_motion_create_args.FindPropertyRelative($"{nameof(Motion_Creator.Movement)}.{nameof(MotionNode_Movement.Movement)}");
            SerializedProperty sp_creator_Movement_Distance = sp_motion_create_args.FindPropertyRelative($"{nameof(Motion_Creator.Movement)}.{nameof(MotionNode_Movement.Distance)}");
            SerializedProperty sp_creator_Movement_Duration = sp_motion_create_args.FindPropertyRelative($"{nameof(Motion_Creator.Movement)}.{nameof(MotionNode_Movement.Duration)}");
            SerializedProperty sp_creator_Movement_Delay = sp_motion_create_args.FindPropertyRelative($"{nameof(Motion_Creator.Movement)}.{nameof(MotionNode_Movement.Delay)}");
            SerializedProperty sp_creator_Movement_Ease = sp_motion_create_args.FindPropertyRelative($"{nameof(Motion_Creator.Movement)}.{nameof(MotionNode_Movement.Ease)}");
            SerializedProperty sp_creator_Rotation = sp_motion_create_args.FindPropertyRelative($"{nameof(Motion_Creator.Rotation)}.{nameof(MotionNode_Rotation.Rotation)}");
            SerializedProperty sp_creator_Rotation_Degree = sp_motion_create_args.FindPropertyRelative($"{nameof(Motion_Creator.Rotation)}.{nameof(MotionNode_Rotation.Degree)}");
            SerializedProperty sp_creator_Rotation_Duration = sp_motion_create_args.FindPropertyRelative($"{nameof(Motion_Creator.Rotation)}.{nameof(MotionNode_Rotation.Duration)}");
            SerializedProperty sp_creator_Rotation_Delay = sp_motion_create_args.FindPropertyRelative($"{nameof(Motion_Creator.Rotation)}.{nameof(MotionNode_Rotation.Delay)}");
            SerializedProperty sp_creator_Rotation_Ease = sp_motion_create_args.FindPropertyRelative($"{nameof(Motion_Creator.Rotation)}.{nameof(MotionNode_Rotation.Ease)}");
            SerializedProperty sp_creator_Alpha_Duration = sp_motion_create_args.FindPropertyRelative($"{nameof(Motion_Creator.Alpha)}.{nameof(MotionNode_Alpha.Duration)}");
            SerializedProperty sp_creator_Alpha_Delay = sp_motion_create_args.FindPropertyRelative($"{nameof(Motion_Creator.Alpha)}.{nameof(MotionNode_Alpha.Delay)}");
            SerializedProperty sp_creator_Alpha_Ease = sp_motion_create_args.FindPropertyRelative($"{nameof(Motion_Creator.Alpha)}.{nameof(MotionNode_Alpha.Ease)}");
            #endregion

            #region SerializedProperty - RecycleArgs
            SerializedProperty sp_motion_recycle_args = sp_item.FindPropertyRelative(nameof(XHud_LayoutSpawner_Item.motion_recycle_args));
            SerializedProperty sp_recycle_Movement = sp_motion_recycle_args.FindPropertyRelative($"{nameof(Motion_Recycler.Movement)}.{nameof(MotionNode_Movement.Movement)}");
            SerializedProperty sp_recycle_Movement_Distance = sp_motion_recycle_args.FindPropertyRelative($"{nameof(Motion_Recycler.Movement)}.{nameof(MotionNode_Movement.Distance)}");
            SerializedProperty sp_recycle_Movement_Duration = sp_motion_recycle_args.FindPropertyRelative($"{nameof(Motion_Recycler.Movement)}.{nameof(MotionNode_Movement.Duration)}");
            SerializedProperty sp_recycle_Movement_Delay = sp_motion_recycle_args.FindPropertyRelative($"{nameof(Motion_Recycler.Movement)}.{nameof(MotionNode_Movement.Delay)}");
            SerializedProperty sp_recycle_Movement_Ease = sp_motion_recycle_args.FindPropertyRelative($"{nameof(Motion_Recycler.Movement)}.{nameof(MotionNode_Movement.Ease)}");
            SerializedProperty sp_recycle_Rotation = sp_motion_recycle_args.FindPropertyRelative($"{nameof(Motion_Recycler.Rotation)}.{nameof(MotionNode_Rotation.Rotation)}");
            SerializedProperty sp_recycle_Rotation_Degree = sp_motion_recycle_args.FindPropertyRelative($"{nameof(Motion_Recycler.Rotation)}.{nameof(MotionNode_Rotation.Degree)}");
            SerializedProperty sp_recycle_Rotation_Duration = sp_motion_recycle_args.FindPropertyRelative($"{nameof(Motion_Recycler.Rotation)}.{nameof(MotionNode_Rotation.Duration)}");
            SerializedProperty sp_recycle_Rotation_Delay = sp_motion_recycle_args.FindPropertyRelative($"{nameof(Motion_Recycler.Rotation)}.{nameof(MotionNode_Rotation.Delay)}");
            SerializedProperty sp_recycle_Rotation_Ease = sp_motion_recycle_args.FindPropertyRelative($"{nameof(Motion_Recycler.Rotation)}.{nameof(MotionNode_Rotation.Ease)}");
            SerializedProperty sp_recycle_Alpha_Duration = sp_motion_recycle_args.FindPropertyRelative($"{nameof(Motion_Recycler.Alpha)}.{nameof(MotionNode_Alpha.Duration)}");
            SerializedProperty sp_recycle_Alpha_Delay = sp_motion_recycle_args.FindPropertyRelative($"{nameof(Motion_Recycler.Alpha)}.{nameof(MotionNode_Alpha.Delay)}");
            SerializedProperty sp_recycle_Alpha_Ease = sp_motion_recycle_args.FindPropertyRelative($"{nameof(Motion_Recycler.Alpha)}.{nameof(MotionNode_Alpha.Ease)}");
            #endregion

            #region CreateArgs - 赋值
            sp_creator_anchor.enumValueIndex = (int)item.motion_create_args.anchor;

            #region Movement
            sp_creator_Movement.enumValueIndex = (int)item.motion_create_args.Movement.Movement;
            sp_creator_Movement_Distance.floatValue = item.motion_create_args.Movement.Distance;
            sp_creator_Movement_Duration.floatValue = item.motion_create_args.Movement.Duration;
            sp_creator_Movement_Delay.floatValue = item.motion_create_args.Movement.Delay;
            sp_creator_Movement_Ease.enumValueIndex = (int)item.motion_create_args.Movement.Ease;
            #endregion

            #region Rotation
            sp_creator_Rotation.enumValueIndex = (int)item.motion_create_args.Rotation.Rotation;
            sp_creator_Rotation_Degree.floatValue = item.motion_create_args.Rotation.Degree;
            sp_creator_Rotation_Duration.floatValue = item.motion_create_args.Rotation.Duration;
            sp_creator_Rotation_Delay.floatValue = item.motion_create_args.Rotation.Delay;
            sp_creator_Rotation_Ease.enumValueIndex = (int)item.motion_create_args.Rotation.Ease;
            #endregion

            #region Alpha
            sp_creator_Alpha_Duration.floatValue = item.motion_create_args.Alpha.Duration;
            sp_creator_Alpha_Delay.floatValue = item.motion_create_args.Alpha.Delay;
            sp_creator_Alpha_Ease.enumValueIndex = (int)item.motion_create_args.Alpha.Ease;
            #endregion
            #endregion

            #region RecycleArgs - 赋值
            #region Movement
            sp_recycle_Movement.enumValueIndex = (int)item.motion_recycle_args.Movement.Movement;
            sp_recycle_Movement_Distance.floatValue = item.motion_recycle_args.Movement.Distance;
            sp_recycle_Movement_Duration.floatValue = item.motion_recycle_args.Movement.Duration;
            sp_recycle_Movement_Delay.floatValue = item.motion_recycle_args.Movement.Delay;
            sp_recycle_Movement_Ease.enumValueIndex = (int)item.motion_recycle_args.Movement.Ease;
            #endregion

            #region Rotation
            sp_recycle_Rotation.enumValueIndex = (int)item.motion_recycle_args.Rotation.Rotation;
            sp_recycle_Rotation_Degree.floatValue = item.motion_recycle_args.Rotation.Degree;
            sp_recycle_Rotation_Duration.floatValue = item.motion_recycle_args.Rotation.Duration;
            sp_recycle_Rotation_Delay.floatValue = item.motion_recycle_args.Rotation.Delay;
            sp_recycle_Rotation_Ease.enumValueIndex = (int)item.motion_recycle_args.Rotation.Ease;
            #endregion

            #region Alpha
            sp_recycle_Alpha_Duration.floatValue = item.motion_recycle_args.Alpha.Duration;
            sp_recycle_Alpha_Delay.floatValue = item.motion_recycle_args.Alpha.Delay;
            sp_recycle_Alpha_Ease.enumValueIndex = (int)item.motion_recycle_args.Alpha.Ease;
            #endregion
            #endregion

            sp_spawn_name.stringValue = item.spawn_name;
            sp_element_uid.stringValue = item.element_uid ?? string.Empty;
            sp_library_name.stringValue = item.library_name ?? string.Empty;
            sp_indicator.stringValue = "Indicator_" + item.spawn_name;
            sp_id.stringValue = XGUI_Utilitys.GenerateUniqueId(CollectIDs(XHudSpace.世界空间));
            sp_delay_spawn.floatValue = item.delay_spawn;
            sp_position.vector3Value = item.position;
            sp_euler.vector3Value = item.euler;
            sp_pivot.vector2Value = item.pivot;
            sp_anchor_min.vector2Value = item.anchor_min;
            sp_anchor_max.vector2Value = item.anchor_max;
            sp_offset.vector3Value = item.offset;
            sp_scale.vector3Value = item.scale;
            sp_size.vector2Value = item.size;
            sp_auto_in.stringValue = item.auto_in;
            sp_motion_mode.stringValue = item.motion_mode;
            sp_is_fold.boolValue = item.is_fold;
            sp_is_enabled.boolValue = item.is_enabled;

            sp_spawn_item_list_world.serializedObject.ApplyModifiedProperties();
        }
        /// <summary>
        /// 卸载一个生成元素 - 世界 World
        /// </summary>
        /// <param name="item"></param>
        private void UnLoadSpawnItem_World(XHud_LayoutSpawner_Item item)
        {
            if (item == null || item.spawned_element_node == null)
                return;
            if (item.spawned_element_node.element == null)
            {
                item.spawned_element_node = null;
                return;
            }
            Undo.DestroyObjectImmediate(item.spawned_element_node.element.gameObject);
            item.spawned_element_node = null;
        }
        /// <summary>
        /// 载入一个生成元素 - 世界 World
        /// </summary>
        /// <param name="item"></param>
        private void LoadSpawnItem_World(XHud_LayoutSpawner_Item item)
        {
            string hex_col = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.themes_get_color(ThemeColorType.primary), true);

            //  统一分派取库名（禁止裸读 library_name）
            string libName = base_script.GetItemLibraryName(item);
            XHud_Library_Element targetLib = hud_manager.hm_ElementLibrary_GetTargetLibrary(libName);

            if (targetLib == null)
            {
                XGUI_Utilitys.Console(
                    "XHud - 布局元素生成器通知",
                    $"未找到元素库： <b><color={hex_col}>{libName}</color></b>！请检查XHud 管理器中是否正确 配置 / 加入 了<b><color={hex_col}> {libName} </color></b>元素库？",
                    XGUIMsgState.通知);
                return;
            }

            if (!item.is_enabled)
                return;

            //  UID 优先定位
            XHud_Module_Element ele = null;
            if (!string.IsNullOrEmpty(item.element_uid))
                ele = targetLib.GetTargetElement_WithUID(item.element_uid);

            //  名字兜底 + 自愈：UID 没命中但名字命中 → 回填 UID
            if (ele == null && !string.IsNullOrEmpty(item.spawn_name))
            {
                ele = targetLib.ElementsLibrary_GetTargetElement(item.spawn_name);
                if (ele != null)
                {
                    string uid = targetLib.GetUID_WithName(item.spawn_name);
                    if (!string.IsNullOrEmpty(uid))
                        item.element_uid = uid;
                }
            }

            //  两者都未命中 → 报元素丢失，不崩
            if (ele == null)
            {
                XGUI_Utilitys.Console(
                    "XHud - 布局元素生成器通知",
                    $"元素丢失：库 <b><color={hex_col}>{libName}</color></b> 中，UID=<b><color={hex_col}>{item.element_uid}</color></b> 与 名称=<b><color={hex_col}>{item.spawn_name}</color></b> 均未命中！",
                    XGUIMsgState.通知);
                return;
            }

            XHud_Module_Element Element = (XHud_Module_Element)PrefabUtility.InstantiatePrefab(ele);
            RectTransform Root = hud_manager.hm_GetAnchorRoot(XHudSpace.世界空间);
            Element.rect_transform.SetParent(Root);
            Element.rect_transform.position = item.position + item.offset;
            Element.rect_transform.localScale = item.scale;
            Element.rect_transform.localEulerAngles = item.euler;
            Element.rect_transform.sizeDelta = item.size;
            Element.rect_transform.pivot = item.pivot;
            Element.rect_transform.anchorMin = item.anchor_min;
            Element.rect_transform.anchorMax = item.anchor_max;
            Element.element_AlphaSet(1);

            item.spawned_element_node = new XHudElementNode();
            item.spawned_element_node.element = Element;
            item.spawned_element_node.id = 0;
            item.spawned_element_node.indicator = Element.indicator;
            item.spawned_element_node.module_name = Element.name;
        }
        #endregion

        #region 动画预览
        /// <summary>
        /// 创建收集图元动画器的动画节点列表所有动画
        /// </summary>
        /// <param name="tweener"></param>
        /// <returns></returns>
        private XTween_Interface[] Preview_PrimitiveTweens_Collected(XHud_Module_Primitive_Tween tweener, string tim, float dur, float delay)
        {
            List<XTween_Interface> tweens = new List<XTween_Interface>();
            for (int i = 0; i < tweener.primitive_tween_nodes.Count; i++)
            {
                if (!tweener.primitive_tween_nodes[i].enabled)
                    continue;
                if (tweener.primitive_tween_nodes[i].timings != tim)
                    continue;
                if (!tweener.Tween_CreateRuleValidCheck(tweener.primitive_tween_nodes[i]))
                    continue;

                XTween_Interface tween = tweener.Tween_Create(tweener.primitive_tween_nodes[i], tweener.global_duration * hud_manager.tween_global_duration * dur);

                if (tween == null)
                    continue;

                tween.SetDelay(tween.Delay + delay);
                tweens.Add(tween);
            }

            return tweens.ToArray();
        }
        /// <summary>
        /// 根据空间类型预览元素动画
        /// </summary>
        private void LayoutsTweenPreview(XHudSpace space, XHudElementCreateState state)
        {
            XTween_Preview_Kill(space);

            // 预览列表
            List<XTween_Interface> preview_twns = new List<XTween_Interface>();

            for (int i = 0; i < (space == XHudSpace.屏幕空间 ? base_script.spawn_item_list_screen.Count : base_script.spawn_item_list_world.Count); i++)
            {
                XHud_LayoutSpawner_Item spw_Item = (space == XHudSpace.屏幕空间 ? base_script.spawn_item_list_screen[i] : base_script.spawn_item_list_world[i]);

                if (spw_Item == null || spw_Item.spawned_element_node == null || spw_Item.spawned_element_node.element == null)
                    continue;

                XHud_Module_Element ele = spw_Item.spawned_element_node.element;

                switch (state)
                {
                    case XHudElementCreateState.Created:
                        // 每个元素创建动画
                        ele.ElementTweens_Creator(spw_Item.motion_mode == "元素自身动效" ? ele.motion_create_args : (spw_Item.motion_mode == "列表项动效" ? spw_Item.motion_create_args : base_script.motion_create_args), null, true);
                        break;
                    case XHudElementCreateState.Recycled:
                        // 每个元素创建动画
                        ele.ElementTweens_Recycler(spw_Item.motion_mode == "元素自身动效" ? ele.motion_recycle_args : (spw_Item.motion_mode == "列表项动效" ? spw_Item.motion_recycle_args : base_script.motion_recycle_args), null, true);
                        break;
                }

                // 生成器列表中的延迟时间
                float delay_spawner = state == XHudElementCreateState.Created ? spw_Item.delay_spawn : spw_Item.delay_despawn;

                #region 每个元素的基础三项动画 - 加入预览列表
                if (ele.tween_alpha != null)
                {
                    ele.tween_alpha.SetDelay(ele.tween_alpha.Delay + delay_spawner);
                    preview_twns.Add(ele.tween_alpha);
                }
                if (ele.tween_move != null)
                {
                    ele.tween_move.SetDelay(ele.tween_move.Delay + delay_spawner);
                    preview_twns.Add(ele.tween_move);
                }
                if (ele.tween_rotation != null)
                {
                    ele.tween_rotation.SetDelay(ele.tween_rotation.Delay + delay_spawner);
                    preview_twns.Add(ele.tween_rotation);
                }
                #endregion

                #region 元素子级中的图元动画 - 加入预览列表
                if (ele.preview_include_primitives_tween)
                {
                    for (int c = 0; c < ele.primitive_controller_nodes.Count; c++)
                    {
                        if (ele.primitive_controller_nodes[c].controller.pt_tween == null)
                            continue;

                        XTween_Interface[] primitive_tweens = Preview_PrimitiveTweens_Collected(ele.primitive_controller_nodes[c].controller.pt_tween, state == XHudElementCreateState.Created ? "元素进入时" : "元素退出时", ele.primitives_tween_global_duration, ele.primitive_controller_nodes[c].delay_time + delay_spawner);

                        for (int s = 0; s < primitive_tweens.Length; s++)
                        {
                            preview_twns.Add(primitive_tweens[s]);
                        }

                        // ── 新增：子图元音效预览 ──
                        Preview_PrimitiveTween_Sound(
                            ele.primitive_controller_nodes[c].controller.pt_tween,
                            state == XHudElementCreateState.Created ? "元素进入时" : "元素退出时",
                            ele.primitives_tween_global_duration,
                            ele.primitive_controller_nodes[c].delay_time + delay_spawner);
                    }
                }
                #endregion
            }

            XTween_Preview_Start(preview_twns.ToArray(), space);
        }
        /// <summary>
        /// 动画预览 - 播放
        /// </summary>
        /// <param name="tweens">传入需要预览的动画，但前提是动画已创建，如果是空的则会导致预览异常</param>
        public void XTween_Preview_Start(XTween_Interface[] tweens, XHudSpace space, bool activate_mark = true)
        {
            if (Application.isPlaying)
                return;

            // 预览动画杀死后自动清空预览器的列表
            Editor_XTween_Previewer.AfterKillClear = true;
            // 预览动画杀死前将动画目标的属性倒退
            Editor_XTween_Previewer.BeforeKillRewind = true;

            Editor_XTween_Previewer.AutoKillWithDuration = true;

            // 预览动画杀死后的委托事件
            Editor_XTween_Previewer.act_on_editor_autokill += XTween_OnAutoKillPreview;


            for (int i = 0; i < tweens.Length; i++)
            {
                Editor_XTween_Previewer.Append(tweens[i]);
            }

            Editor_XTween_Previewer.Play(null);

            cur_space = space;

            if (activate_mark)
                Editor_XHud_Tool_SceneView_Activate_Mark.SetEnabled(
                   state: true,
                   x_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                   x_title: "Seven Strike Media",
                   x_msg: "布局元素动效预览中...",
                   x_anchor: XHudSceneActivateMarkAnchor.左下);
        }
        /// <summary>
        ///  动画预览 - 杀死
        /// </summary>
        private void XTween_Preview_Kill(XHudSpace space)
        {
            if (Application.isPlaying)
                return;

            // 预览器执行动作：杀死动画
            Editor_XTween_Previewer.Kill(true, true, () =>
            {
                //BaseScript.CurrentTweener = null;
            });

            // 当动画预览器为根据动画耗时自动杀死的情况下
            Editor_XTween_Previewer.act_on_editor_autokill -= XTween_OnAutoKillPreview;

            // 根据是否包含图元动画预览类型对图元动画进行特性复位
            for (int i = 0; i < (space == XHudSpace.屏幕空间 ? base_script.spawn_item_list_screen.Count : base_script.spawn_item_list_world.Count); i++)
            {
                XHud_LayoutSpawner_Item spw_item = space == XHudSpace.屏幕空间 ? base_script.spawn_item_list_screen[i] : base_script.spawn_item_list_world[i];

                if (spw_item == null || spw_item.spawned_element_node == null || spw_item.spawned_element_node.element == null)
                    continue;

                for (int s = 0; s < spw_item.spawned_element_node.element.primitive_controller_nodes.Count; s++)
                {
                    PrimitiveControllerNode node = spw_item.spawned_element_node.element.primitive_controller_nodes[s];
                    if (node.controller.pt_feature == null)
                        continue;
                    node.controller.pt_feature.PrimitiveFeature_Load();
                }

                // 杀死元素三项自身动画
                spw_item.spawned_element_node.element.ElementTweens_Kill();
                // 清空元素三项自身动画
                spw_item.spawned_element_node.element.ElementTweens_Clear();
            }

            cur_space = space;

            XGUI.EditorPlayer_Sound.Stop();

            Editor_XHud_Tool_SceneView_Activate_Mark.SetEnabled(false);
        }
        /// <summary>
        ///  动画预览 - 自动杀死的委托
        /// </summary>
        private void XTween_OnAutoKillPreview()
        {
            // 根据是否包含图元动画预览类型对图元动画进行特性复位
            for (int i = 0; i < (cur_space == XHudSpace.屏幕空间 ? base_script.spawn_item_list_screen.Count : base_script.spawn_item_list_world.Count); i++)
            {
                XHud_LayoutSpawner_Item spw_item = (cur_space == XHudSpace.屏幕空间 ? base_script.spawn_item_list_screen[i] : base_script.spawn_item_list_world[i]);

                if (spw_item == null || spw_item.spawned_element_node == null || spw_item.spawned_element_node.element == null)
                    continue;

                for (int s = 0; s < spw_item.spawned_element_node.element.primitive_controller_nodes.Count; s++)
                {
                    PrimitiveControllerNode node = spw_item.spawned_element_node.element.primitive_controller_nodes[s];
                    if (node.controller.pt_feature == null)
                        continue;
                    node.controller.pt_feature.PrimitiveFeature_Load();
                }

                // 杀死元素三项自身动画
                spw_item.spawned_element_node.element.ElementTweens_Kill();
                // 清空元素三项自身动画
                spw_item.spawned_element_node.element.ElementTweens_Clear();
            }

            // 清空预览动画杀死后的委托事件
            Editor_XTween_Previewer.act_on_editor_autokill -= XTween_OnAutoKillPreview;

            IsPreviewing = false;
            XGUI.EditorPlayer_Sound.Stop();

            Editor_XHud_Tool_SceneView_Activate_Mark.SetEnabled(false);
        }
        /// <summary>
        /// 预览倒退重置
        /// </summary>
        private void XTween_Preview_Rewind()
        {
            Editor_XTween_Previewer.Rewind();
        }
        /// <summary>
        /// 停止预览
        /// </summary>
        private void XTween_Preview_Stop()
        {
            if (element_list_screen != null && element_list_screen.count > 0)
                XTween_Preview_Kill(XHudSpace.屏幕空间);

            if (element_list_world != null && element_list_world.count > 0)
                XTween_Preview_Kill(XHudSpace.世界空间);

            IsPreviewing = false;
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
            window.set_target_motion_mode(LayoutSpawnerMotionTarget.自身动效);
            string hexcol = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.themes_get_color(ThemeColorType.primary), true);
            window.set_title($"<color={hexcol}>{(string.IsNullOrEmpty(sp_spawner_indicator.stringValue) ? base_script.gameObject.name : $"{base_script.gameObject.name} - {sp_spawner_indicator.stringValue}")} </color>");

            //window.ShowModal();
            window.Show();
        }
        /// <summary>
        /// 打开动效资源修改器窗口，用于编辑当前生成或回收动效参数
        /// </summary>
        /// <param name="group">
        /// 待编辑的动效参数分组
        /// </param>
        public void motion_edit(XHud_Arg_MotionGroup group, XHudSpace space, int index)
        {
            Editor_XHud_LibrarySetTool_Motion window = EditorWindow.GetWindow<Editor_XHud_LibrarySetTool_Motion>(true);

            window.titleContent = new GUIContent("XHud - 动效资源修改器");
            XGUI.CenterEditorWindow(new Vector2Int(348, group.type == HudElementMotionType.Creator ? 710 : 640), window);
            window.set_motion_type(group.type);
            window.set_preedit_target_id(base_script.GetInstanceID().ToString());
            window.set_target_index(index);
            window.set_target_space(space);
            window.set_motion(group.motion_creator, group.motion_recycler);
            window.set_library_setter_mode(LibrarySetterMode.修改动效参数);
            window.set_button_text("更新");
            window.set_target_motion_mode(LayoutSpawnerMotionTarget.列表项动效);
            string hexcol = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.themes_get_color(ThemeColorType.primary), true);

            string item_name = space == XHudSpace.屏幕空间 ? base_script.spawn_item_list_screen[index].spawn_name : base_script.spawn_item_list_world[index].spawn_name;

            window.set_title($"<color={hexcol}>{item_name} </color>");

            //window.ShowModal();
            window.Show();
        }
        #endregion

        #region 音效预览
        /// <summary>
        /// 预览子图元动画器上匹配指定时机的音效。
        /// <para/>
        /// 【延迟公式】sod.Delay × HudManager.DurationMultiply × globalDuration + nodeDelay
        /// 与子图元动画的延迟公式保持一致（动画延迟 = tween.Delay + node.DelayTime）。
        /// </summary>
        private void Preview_PrimitiveTween_Sound(XHud_Module_Primitive_Tween tweener, string timing, float globalDuration, float nodeDelay)
        {
            if (tweener == null) return;
            if (tweener.mute_play) return;
            if (EditorUtility.audioMasterMute) return;
            if (string.IsNullOrEmpty(timing)) return;

            for (int s = 0; s < tweener.primitive_tween_sounds.Count; s++)
            {
                TweenSound sod = tweener.primitive_tween_sounds[s];
                if (sod == null) continue;
                if (sod.mute) continue;
                if (sod.timing != timing) continue;

                AudioClip clip = Resolve_PrimitiveSoundClip(sod);
                if (clip == null) continue;

                // ── 延迟：与子图元动画对齐 ──
                float mul = 1f;
                if (hud_manager != null && hud_manager.tween_global_duration > 0f)
                    mul = hud_manager.tween_global_duration;
                mul *= globalDuration;

                float delay = Mathf.Max(0f, sod.delay * mul + nodeDelay);

                // ── 音量 / 音高 / 静音 ──
                float masterVol = (hud_manager != null) ? hud_manager.global_volume * 0.01f : 1f;
                bool masterMute = (hud_manager != null) && hud_manager.global_volume_mute;
                float volume = masterVol * Mathf.Clamp01(sod.volume);
                bool mute = masterMute;

                float pMin = Mathf.Min(sod.min_pitch, sod.max_pitch);
                float pMax = Mathf.Max(sod.min_pitch, sod.max_pitch);
                bool useRandomPitch = !Mathf.Approximately(pMin, 1f) || !Mathf.Approximately(pMax, 1f);

                XGUI.EditorPlayer_Sound.Enqueue(
                    clip, delay, volume, mute, useRandomPitch, pMin, pMax);
            }
        }
        /// <summary>
        /// 从音效库安全取出一条 AudioClip（与图元侧 ResolvePreviewClip 逻辑一致）。
        /// </summary>
        private AudioClip Resolve_PrimitiveSoundClip(TweenSound sod)
        {
            if (sod == null || sod.sound == null) return null;

            if (hud_manager != null && hud_manager.library_sounds != null)
            {
                string soundName = sod.sound.name;
                if (!string.IsNullOrEmpty(soundName))
                {
                    AudioClip clip = hud_manager.library_sounds.SoundLibrary_GetSound(soundName);
                    if (clip != null) return clip;
                }
            }
            return sod.sound;
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