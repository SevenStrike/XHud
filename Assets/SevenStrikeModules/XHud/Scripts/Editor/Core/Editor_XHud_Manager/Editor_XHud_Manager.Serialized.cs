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
    using UnityEditor;

    public partial class Editor_XHud_Manager : Editor
    {
        #region 序列化属性 / 字段
        /// <summary>
        /// 序列化变量。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>在 <c>OnEnable</c> 中集中获取全部 <c>SerializedProperty</c>。</description></item>
        /// <item><description>覆盖初始化、库、画布、相机、安全框、掩码、蓝图、RMS、主题、参考图、组件引导等配置项。</description></item>
        /// </list>
        /// </summary>
        private void xHud_SerializedAllVariables()
        {
            initialized = serializedObject.FindProperty(nameof(XHud_Manager.initialized));

            #region Camera
            sp_camera_scene = serializedObject.FindProperty(nameof(XHud_Manager.camera_scene));
            sp_camera_ui = serializedObject.FindProperty(nameof(XHud_Manager.camera_ui));
            sp_camera_ui_cutter_near = serializedObject.FindProperty(nameof(XHud_Manager.camera_ui_cutter_near));
            sp_camera_ui_cutter_far = serializedObject.FindProperty(nameof(XHud_Manager.camera_ui_cutter_far));
            sp_camera_othograpic_mode = serializedObject.FindProperty(nameof(XHud_Manager.camera_othograpic_mode));
            sp_camera_ui_orthographic_size = serializedObject.FindProperty(nameof(XHud_Manager.camera_ui_orthographic_size));
            sp_camera_ui_fov = serializedObject.FindProperty(nameof(XHud_Manager.camera_ui_fov));
            #endregion

            #region BluePrint
            sp_bp_mode = serializedObject.FindProperty(nameof(XHud_Manager.bp_mode));
            sp_bp_opacity = serializedObject.FindProperty(nameof(XHud_Manager.bp_opacity));
            sp_bp_grid_size = serializedObject.FindProperty(nameof(XHud_Manager.bp_grid_size));
            sp_bp_linewidth = serializedObject.FindProperty(nameof(XHud_Manager.bp_linewidth));
            sp_bp_grid_color = serializedObject.FindProperty(nameof(XHud_Manager.bp_grid_color));
            sp_bp_bg_color = serializedObject.FindProperty(nameof(XHud_Manager.bp_bg_color));
            sp_bp_bg_decal_color = serializedObject.FindProperty(nameof(XHud_Manager.bp_bg_decal_color));
            sp_bp_bg_name = serializedObject.FindProperty(nameof(XHud_Manager.bp_bg_name));
            sp_bp_grid_opacity = serializedObject.FindProperty(nameof(XHud_Manager.bp_grid_opacity));
            sp_bp_bg_map_opacity = serializedObject.FindProperty(nameof(XHud_Manager.bp_bg_map_opacity));
            sp_bp_bg_usetilling_index = serializedObject.FindProperty(nameof(XHud_Manager.bp_bg_usetilling_index));
            sp_bp_bg_usesquareratio_index = serializedObject.FindProperty(nameof(XHud_Manager.bp_bg_usesquareratio_index));
            sp_bp_bg_opacity = serializedObject.FindProperty(nameof(XHud_Manager.bp_bg_opacity));
            sp_bp_bg_tilling = serializedObject.FindProperty(nameof(XHud_Manager.bp_bg_tilling));
            sp_bp_marktitle_color = serializedObject.FindProperty(nameof(XHud_Manager.bp_marktitle_color));
            sp_bp_marksubtitle_color = serializedObject.FindProperty(nameof(XHud_Manager.bp_marksubtitle_color));
            sp_bp_mark_size = serializedObject.FindProperty(nameof(XHud_Manager.bp_mark_size));
            sp_bp_mark_opacity = serializedObject.FindProperty(nameof(XHud_Manager.bp_mark_opacity));
            sp_bp_mark_margin = serializedObject.FindProperty(nameof(XHud_Manager.bp_mark_margin));
            sp_bp_mark_space = serializedObject.FindProperty(nameof(XHud_Manager.bp_mark_space));
            sp_bp_root = serializedObject.FindProperty(nameof(XHud_Manager.bp_root));
            sp_bp_title_content = serializedObject.FindProperty(nameof(XHud_Manager.bp_title_content));
            sp_bp_subtitle_content = serializedObject.FindProperty(nameof(XHud_Manager.bp_subtitle_content));
            sp_bp_mark_anchors = serializedObject.FindProperty(nameof(XHud_Manager.bp_mark_anchors));
            sp_bp_grid_animation_ease = serializedObject.FindProperty(nameof(XHud_Manager.bp_grid_animation_ease));
            sp_bp_bg_animation_ease_in = serializedObject.FindProperty(nameof(XHud_Manager.bp_bg_animation_ease_in));
            sp_bp_bg_animation_ease_out = serializedObject.FindProperty(nameof(XHud_Manager.bp_bg_animation_ease_out));
            sp_bp_grid_length_percentage = serializedObject.FindProperty(nameof(XHud_Manager.bp_grid_length_percentage));
            sp_bp_grid_level_height = serializedObject.FindProperty(nameof(XHud_Manager.bp_grid_level_height));
            sp_bp_animation_duration = serializedObject.FindProperty(nameof(XHud_Manager.bp_animation_duration));
            sp_bp_displayed = serializedObject.FindProperty(nameof(XHud_Manager.bp_displayed));
            sp_bp_on_start_hide = serializedObject.FindProperty(nameof(XHud_Manager.bp_on_start_hide));
            sp_bp_bg_fade_animation_delay = serializedObject.FindProperty(nameof(XHud_Manager.bp_bg_fade_animation_delay));
            sp_bp_grid_end = serializedObject.FindProperty(nameof(XHud_Manager.bp_grid_end));
            sp_bp_grid_animation_duration = serializedObject.FindProperty(nameof(XHud_Manager.bp_grid_animation_duration));
            sp_bp_eft_grid = serializedObject.FindProperty(nameof(XHud_Manager.bp_eft_grid));
            sp_bp_eft_grid_fade = serializedObject.FindProperty(nameof(XHud_Manager.bp_eft_grid_fade));
            sp_bp_eft_bg = serializedObject.FindProperty(nameof(XHud_Manager.bp_eft_bg));
            sp_bp_eft_mark = serializedObject.FindProperty(nameof(XHud_Manager.bp_eft_mark));
            #endregion

            #region CompGuide
            sp_cg_anchor_root = serializedObject.FindProperty(nameof(XHud_Manager.cg_anchor_root));
            sp_cg_anchors = serializedObject.FindProperty(nameof(XHud_Manager.cg_anchors));
            sp_cg_mode = serializedObject.FindProperty(nameof(XHud_Manager.cg_mode));
            sp_cg_use = serializedObject.FindProperty(nameof(XHud_Manager.cg_use));
            sp_cg_color = serializedObject.FindProperty(nameof(XHud_Manager.cg_color));
            sp_cg_point_color = serializedObject.FindProperty(nameof(XHud_Manager.cg_point_color));
            sp_cg_mirror_lr_offset = serializedObject.FindProperty(nameof(XHud_Manager.cg_mirror_lr_offset));
            sp_cg_mirror_lr_golden_mode = serializedObject.FindProperty(nameof(XHud_Manager.cg_mirror_lr_golden_mode));
            sp_cg_mirror_ud_offset = serializedObject.FindProperty(nameof(XHud_Manager.cg_mirror_ud_offset));
            sp_cg_mirror_ud_golden_mode = serializedObject.FindProperty(nameof(XHud_Manager.cg_mirror_ud_golden_mode));
            sp_cg_fibonacci_mode = serializedObject.FindProperty(nameof(XHud_Manager.cg_fibonacci_mode));
            sp_cg_editor_serializefield_corner_lookat_offset_h = serializedObject.FindProperty(XHud_Manager.cg_editor_serializefield_corner_lookat_offset_h);
            sp_cg_editor_serializefield_corner_lookat_offset_v = serializedObject.FindProperty(XHud_Manager.cg_editor_serializefield_corner_lookat_offset_v);
            sp_cg_editor_serializefield_three_offset_h = serializedObject.FindProperty(XHud_Manager.cg_editor_serializefield_three_offset_h);
            sp_cg_editor_serializefield_three_offset_v = serializedObject.FindProperty(XHud_Manager.cg_editor_serializefield_three_offset_v);
            sp_cg_editor_serializefield_three_offset_coverage = serializedObject.FindProperty(XHud_Manager.cg_editor_serializefield_three_offset_coverage);
            sp_cg_editor_serializefield_guide_line_base_offset = serializedObject.FindProperty(XHud_Manager.cg_editor_serializefield_guide_line_base_offset);
            sp_cg_editor_serializefield_guide_line_base_height = serializedObject.FindProperty(XHud_Manager.cg_editor_serializefield_guide_line_base_height);
            sp_cg_editor_serializefield_guide_line_offset_near = serializedObject.FindProperty(XHud_Manager.cg_editor_serializefield_guide_line_offset_near);
            sp_cg_editor_serializefield_guide_line_offset_far = serializedObject.FindProperty(XHud_Manager.cg_editor_serializefield_guide_line_offset_far);
            sp_cg_editor_serializefield_guide_line_offset_near_height = serializedObject.FindProperty(XHud_Manager.cg_editor_serializefield_guide_line_offset_near_height);
            sp_cg_editor_serializefield_triangle_top_offset = serializedObject.FindProperty(XHud_Manager.cg_editor_serializefield_triangle_top_offset);
            sp_cg_editor_serializefield_triangle_base_height = serializedObject.FindProperty(XHud_Manager.cg_editor_serializefield_triangle_base_height);
            sp_cg_editor_serializefield_guide_param_triangle_bottom_height = serializedObject.FindProperty(XHud_Manager.cg_editor_serializefield_guide_param_triangle_bottom_height);
            sp_cg_editor_serializefield_triangle_offset_left = serializedObject.FindProperty(XHud_Manager.cg_editor_serializefield_triangle_offset_left);
            sp_cg_editor_serializefield_triangle_offset_right = serializedObject.FindProperty(XHud_Manager.cg_editor_serializefield_triangle_offset_right);
            sp_cg_editor_serializefield_center_point_size = serializedObject.FindProperty(XHud_Manager.cg_editor_serializefield_center_point_size);
            sp_cg_editor_serializefield_ishape_top_height = serializedObject.FindProperty(XHud_Manager.cg_editor_serializefield_ishape_top_height);
            sp_cg_editor_serializefield_ishape_bottom_height = serializedObject.FindProperty(XHud_Manager.cg_editor_serializefield_ishape_bottom_height);
            sp_cg_editor_serializefield_ishape_offset_left = serializedObject.FindProperty(XHud_Manager.cg_editor_serializefield_ishape_offset_left);
            sp_cg_editor_serializefield_ishape_offset_right = serializedObject.FindProperty(XHud_Manager.cg_editor_serializefield_ishape_offset_right);
            #endregion

            #region Canvas
            sp_cv_scaler = serializedObject.FindProperty(nameof(XHud_Manager.cv_scaler));
            sp_cv_scaler_mode_index = serializedObject.FindProperty(nameof(XHud_Manager.cv_scaler_mode_index));
            sp_cv_match_dir = serializedObject.FindProperty(nameof(XHud_Manager.cv_match_dir));
            sp_cv_screen_res = serializedObject.FindProperty(nameof(XHud_Manager.cv_screen_res));
            sp_cv_use_perfect_pixel_update = serializedObject.FindProperty(nameof(XHud_Manager.cv_use_perfect_pixel_update));
            sp_cv_use_auto_perfect_pixel = serializedObject.FindProperty(nameof(XHud_Manager.cv_use_auto_perfect_pixel));
            sp_cv_hudcanvas_screen_anchor = serializedObject.FindProperty(nameof(XHud_Manager.cv_hudcanvas_screen_anchor));
            sp_cv_hudcanvas_world_anchor = serializedObject.FindProperty(nameof(XHud_Manager.cv_hudcanvas_world_anchor));
            sp_cv_support_world_ui = serializedObject.FindProperty(nameof(XHud_Manager.cv_support_world_ui));
            sp_cv_anchors_layout_screen = serializedObject.FindProperty(nameof(XHud_Manager.cv_anchors_layout_screen));
            sp_cv_margins = serializedObject.FindProperty(nameof(XHud_Manager.cv_margins));
            sp_cv_margin_horizontal = serializedObject.FindProperty(nameof(XHud_Manager.cv_margin_horizontal));
            sp_cv_margin_multiply = serializedObject.FindProperty(nameof(XHud_Manager.cv_margin_multiply));
            sp_cv_margin_vertical = serializedObject.FindProperty(nameof(XHud_Manager.cv_margin_vertical));
            sp_cv_distance = serializedObject.FindProperty(nameof(XHud_Manager.cv_distance));
            sp_cv_screen = serializedObject.FindProperty(nameof(XHud_Manager.cv_screen));
            sp_cv_hudcanvas_world = serializedObject.FindProperty(nameof(XHud_Manager.cv_hudcanvas_world));
            sp_cv_hudcanvas_anchor = serializedObject.FindProperty(nameof(XHud_Manager.cv_hudcanvas_anchor));
            sp_cv_hudcanvas_anchor_index = serializedObject.FindProperty(nameof(XHud_Manager.cv_hudcanvas_anchor_index));
            #endregion

            #region Mask
            sp_mk_mask = serializedObject.FindProperty(nameof(XHud_Manager.mk_mask));
            sp_mk_alpha = serializedObject.FindProperty(nameof(XHud_Manager.mk_alpha));
            sp_mk_texture = serializedObject.FindProperty(nameof(XHud_Manager.mk_texture));
            sp_mk_raycast_alpha_threshold = serializedObject.FindProperty(nameof(XHud_Manager.mk_raycast_alpha_threshold));
            sp_mk_color = serializedObject.FindProperty(nameof(XHud_Manager.mk_color));
            sp_mk_raycast_enabled = serializedObject.FindProperty(nameof(XHud_Manager.mk_raycast_enabled));
            #endregion

            #region Opacity
            sp_op_hud_canvas_group_screen = serializedObject.FindProperty(nameof(XHud_Manager.op_hud_canvas_group_screen));
            sp_op_hud_canvas_group_world = serializedObject.FindProperty(nameof(XHud_Manager.op_hud_canvas_group_world));
            sp_op_content_opacity_screen = serializedObject.FindProperty(nameof(XHud_Manager.op_content_opacity_screen));
            sp_op_content_opacity_world = serializedObject.FindProperty(nameof(XHud_Manager.op_content_opacity_world));
            #endregion

            #region Elements
            sp_ele_library_elements = serializedObject.FindProperty(nameof(XHud_Manager.ele_library_elements));
            sp_ele_global_motion_create_library_index_name = serializedObject.FindProperty(nameof(XHud_Manager.ele_global_motion_create_library_index_name));
            sp_ele_global_motion_recycle_library_index_name = serializedObject.FindProperty(nameof(XHud_Manager.ele_global_motion_recycle_library_index_name));
            sp_ele_visual_placer = serializedObject.FindProperty(nameof(XHud_Manager.ele_visual_placer));
            #endregion

            #region Based
            sp_library_colors = serializedObject.FindProperty(nameof(XHud_Manager.library_colors));
            sp_library_curves = serializedObject.FindProperty(nameof(XHud_Manager.library_curves));
            sp_library_sounds = serializedObject.FindProperty(nameof(XHud_Manager.library_sounds));
            sp_library_textstyles = serializedObject.FindProperty(nameof(XHud_Manager.library_textstyles));
            sp_library_motions = serializedObject.FindProperty(nameof(XHud_Manager.library_motions));
            sp_library_transitions = serializedObject.FindProperty(nameof(XHud_Manager.library_transitions));
            sp_tween_global_duration = serializedObject.FindProperty(nameof(XHud_Manager.tween_global_duration));
            sp_hud_event_system = serializedObject.FindProperty(nameof(XHud_Manager.hud_event_system));
            sp_hud_input_system_ui_input_module = serializedObject.FindProperty(nameof(XHud_Manager.hud_input_system_ui_input_module));
            sp_primitive_id_label_line_height = serializedObject.FindProperty(nameof(XHud_Manager.primitive_id_label_line_height));
            sp_primitive_id_label_line_color = serializedObject.FindProperty(nameof(XHud_Manager.primitive_id_label_line_color));
            sp_primitive_id_label_font_size = serializedObject.FindProperty(nameof(XHud_Manager.primitive_id_label_font_size));
            sp_primitive_id_label_font_color = serializedObject.FindProperty(nameof(XHud_Manager.primitive_id_label_font_color));
            sp_use_debug = serializedObject.FindProperty(nameof(XHud_Manager.use_debug));
            sp_fold_all_panel_with_disabled = serializedObject.FindProperty(nameof(XHud_Manager.fold_all_panel_with_disabled));
            #endregion

            #region Pool - Sound
            sp_pool_sounder = serializedObject.FindProperty(nameof(XHud_Manager.pool_sounder));
            sp_pool_sounder_count = serializedObject.FindProperty(nameof(XHud_Manager.pool_sounder_count));
            sp_global_volume = serializedObject.FindProperty(nameof(XHud_Manager.global_volume));
            sp_global_volume_mute = serializedObject.FindProperty(nameof(XHud_Manager.global_volume_mute));
            #endregion

            #region InstanceMode
            sp_use_instance = serializedObject.FindProperty(nameof(XHud_Manager.use_instance));
            #endregion

            #region SafeFrame
            sp_sf_structure_displayer = serializedObject.FindProperty(nameof(XHud_Manager.sf_structure_displayer));
            sp_sf_canvas_scaler_screen_size = serializedObject.FindProperty(nameof(XHud_Manager.sf_canvas_scaler_screen_size));
            sp_sf_safeframe = serializedObject.FindProperty(nameof(XHud_Manager.sf_safeframe));
            sp_sf_mark_size = serializedObject.FindProperty(nameof(XHud_Manager.sf_mark_size));
            sp_sf_color_layout_anchor_mark = serializedObject.FindProperty(nameof(XHud_Manager.sf_color_layout_anchor_mark));
            sp_sf_color_center_mark = serializedObject.FindProperty(nameof(XHud_Manager.sf_color_center_mark));
            sp_sf_color_frame_line = serializedObject.FindProperty(nameof(XHud_Manager.sf_color_frame_line));
            sp_sf_color_seperater_line = serializedObject.FindProperty(nameof(XHud_Manager.sf_color_seperater_line));
            sp_sf_safe_frame_line_width = serializedObject.FindProperty(nameof(XHud_Manager.sf_safe_frame_line_width));
            sp_sf_line_margins = serializedObject.FindProperty(nameof(XHud_Manager.sf_line_margins));
            sp_sf_seperater_length = serializedObject.FindProperty(nameof(XHud_Manager.sf_seperater_length));
            sp_sf_center_mark_length = serializedObject.FindProperty(nameof(XHud_Manager.sf_center_mark_length));
            sp_sf_center_mark_width = serializedObject.FindProperty(nameof(XHud_Manager.sf_center_mark_width));
            sp_sf_center_mark_distance = serializedObject.FindProperty(nameof(XHud_Manager.sf_center_mark_distance));
            sp_sf_use = serializedObject.FindProperty(nameof(XHud_Manager.sf_use));
            #endregion

            #region Mouse
            sp_mos_custom_cursor = serializedObject.FindProperty(nameof(XHud_Manager.mos_custom_cursor));
            sp_mos_mouse_cursor = serializedObject.FindProperty(nameof(XHud_Manager.mos_mouse_cursor));
            #endregion

            #region Transition
            sp_trans_custom_transition = serializedObject.FindProperty(nameof(XHud_Manager.trans_custom_transition));
            sp_trans_transition_controller = serializedObject.FindProperty(nameof(XHud_Manager.trans_transition_controller));
            #endregion

            #region Font
            sp_ft_font_size_multiply = serializedObject.FindProperty(nameof(XHud_Manager.ft_font_size_multiply));
            #endregion

            #region RMS 
            sp_rms_enabled = serializedObject.FindProperty(nameof(XHud_Manager.rms_enabled));
            sp_rms_nodes = serializedObject.FindProperty(nameof(XHud_Manager.rms_nodes));
            sp_rms_current_solution = serializedObject.FindProperty(nameof(XHud_Manager.rms_current_solution));
            #endregion

            #region Theme
            sp_theme_color_primary = serializedObject.FindProperty(nameof(XHud_Manager.theme_color_primary));
            sp_theme_color_group = serializedObject.FindProperty(nameof(XHud_Manager.theme_color_group));
            sp_theme_color_sepline = serializedObject.FindProperty(nameof(XHud_Manager.theme_color_sepline));
            sp_theme_solution_primary = serializedObject.FindProperty(nameof(XHud_Manager.theme_solution_primary));
            sp_theme_solution_group = serializedObject.FindProperty(nameof(XHud_Manager.theme_solution_group));
            sp_theme_solution_sepline = serializedObject.FindProperty(nameof(XHud_Manager.theme_solution_sepline));
            #endregion

            #region HumanRatioRefer
            sp_hmr_physics_screen_size = serializedObject.FindProperty(nameof(XHud_Manager.hmr_physics_screen_size));
            sp_hmr_reference_image = serializedObject.FindProperty(nameof(XHud_Manager.hmr_reference_image));
            sp_hmr_use_ratio_reference = serializedObject.FindProperty(nameof(XHud_Manager.hmr_use_ratio_reference));
            sp_hmr_ratio_reference_is_part = serializedObject.FindProperty(nameof(XHud_Manager.hmr_ratio_reference_is_part));
            sp_hmr_reference_image_color = serializedObject.FindProperty(nameof(XHud_Manager.hmr_reference_image_color));
            sp_hmr_res_full = serializedObject.FindProperty(nameof(XHud_Manager.hmr_res_full));
            sp_hmr_res_part = serializedObject.FindProperty(nameof(XHud_Manager.hmr_res_part));
            sp_hmr_refer_shape_ratio_size = serializedObject.FindProperty(nameof(XHud_Manager.hmr_refer_shape_ratio_size));
            sp_hmr_refer_shape_ratio_tolerance = serializedObject.FindProperty(nameof(XHud_Manager.hmr_refer_shape_ratio_tolerance));
            #endregion
        }
        #endregion
    }
}