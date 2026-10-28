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
    using TMPro;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;

    public partial class Editor_XHud_Manager : Editor
    {
        #region 绘制 - 选项
        /// <summary>
        /// 绘制「选项」面板内容。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>绘制单例模式、状态调试、辅助结构等开关。</description></item>
        /// <item><description>绘制辅助视觉、构图参考视觉、蓝图视觉、元素放置器。</description></item>
        /// <item><description>绘制像素对齐、相机投影、音效静音、个性光标、转场特效、世界元素、RMS 模块。</description></item>
        /// <item><description>绘制画布尺寸与画布距离两个下拉选项。</description></item>
        /// </list>
        /// </summary>
        private void Draw_Options()
        {
            #region 单例模式
            DrawToggle("单例模式", sp_use_instance, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, (b) => { });
            #endregion

            #region 状态调试
            DrawToggle("状态调试", sp_use_debug, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, (b) => { });
            #endregion

            #region 辅助结构
            DrawToggle("辅助结构", sp_sf_structure_displayer, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, (b) => { });
            #endregion

            XGUI.layout_seperator(
                thickness: 1,
                color: XHud_Dashboard.themes_get_color(ThemeColorType.sepline),
                margin: new RectOffset(15, 15, 15, 15));

            #region 辅助视觉
            if (sp_camera_scene.objectReferenceValue != null)
            {
                DrawToggle("辅助视觉", sp_sf_use, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, (b) =>
                {
                    sp_sf_use.boolValue = b;
                    sp_sf_use.serializedObject.ApplyModifiedProperties();

                    if (!b)
                    {
                        base_script.hm_AnchorMarks_Destroy();
                    }
                    else
                    {
                        base_script.hm_AnchorMarks_Create();
                    }

                    SetHighFrequencyMode();
                    xHud_EditorUpdate_HelperVisual();
                    return;
                });
            }
            #endregion

            #region 构图参考视觉
            DrawToggle("构图参考视觉", sp_cg_use, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, (b) =>
            {
                if (sp_cg_use.boolValue)
                {
                    if (sp_cg_anchor_root.objectReferenceValue != null)
                    {
                        return;
                    }
                    xHud_CompGuide_Structure_Create();
                }
                else
                {
                    if (sp_cg_anchor_root.objectReferenceValue == null)
                    {
                        return;
                    }
                    xHud_CompGuide_Structure_Destroy();
                }
            });
            #endregion

            #region 蓝图视觉
            DrawToggle("蓝图视觉", sp_bp_mode, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, (b) =>
            {
                sp_bp_mode.boolValue = b;
                sp_bp_mode.serializedObject.ApplyModifiedProperties();

                if (!sp_bp_mode.boolValue)
                {
                    EditorApplication.delayCall += () =>
                    {
                        XGUI.dialog(
                            type: XGUIDialogType.警告,
                            windowtitle: "XHud - 管理器消息",
                            title: "蓝图模式",
                            msg: "此操作会退出蓝图模式，请确实是否继续该操作？",
                            ok: "退出",
                            cancel: "暂不",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                            on_selected: (d) =>
                            {
                                if (d == "退出")
                                {
                                    base_script.hm_BluePrint_Remove();
                                }
                                else
                                {
                                    base_script.bp_mode = true;
                                    return;
                                }
                            });
                    };
                }
                else
                {
                    Material mat = AssetDatabase.LoadAssetAtPath<Material>($"{XHud_Dashboard.Get_Path_XHUD_MATERIALS_Path()}BluePrints/BluePrint.mat");
                    TMP_FontAsset title = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>($"{XHud_Dashboard.Get_Path_XHUD_FONTS_Path()}Tmp/sx_bold SDF.asset");
                    TMP_FontAsset subtitle = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>($"{XHud_Dashboard.Get_Path_XHUD_FONTS_Path()}Tmp/sx_light SDF.asset");
                    #region 指定叠加背景
                    if (string.IsNullOrEmpty(sp_bp_bg_name.stringValue))
                        sp_bp_bg_name.stringValue = blue_print_bgs_name[blue_print_bgs_name.Length - 1];
                    sp_bp_bg_name.serializedObject.ApplyModifiedProperties();
                    base_script.hm_BluePrint_Create(true, mat, title, subtitle);

                    #endregion

                    return;
                }
            });
            #endregion

            #region 关闭时折叠所有
            DrawToggle("关闭时折叠所有", sp_fold_all_panel_with_disabled, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, (b) => { });
            #endregion

            #region 元素放置器
            sp_ele_visual_placer.boolValue = Editor_XHud_Tool_SceneView_ElementVisualPlacer.GetEnabled();
            sp_ele_visual_placer.serializedObject.ApplyModifiedProperties();

            DrawToggle("元素放置器", sp_ele_visual_placer, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, (b) =>
            {
                sp_ele_visual_placer.boolValue = b;
                sp_ele_visual_placer.serializedObject.ApplyModifiedProperties();

                Editor_XHud_Tool_SceneView_ElementVisualPlacer.SetEnabled(sp_ele_visual_placer.boolValue);
                XHud_Dashboard.utilitys_set_util_screeen_element_visual_placer(sp_ele_visual_placer.boolValue);
            });
            #endregion

            XGUI.layout_seperator(
                thickness: 1,
                color: XHud_Dashboard.themes_get_color(ThemeColorType.sepline),
                margin: new RectOffset(15, 15, 15, 15));

            #region 像素对齐
            DrawToggle("像素对齐", sp_cv_use_perfect_pixel_update, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, (b) =>
            {
                sp_cv_use_perfect_pixel_update.boolValue = b;
                sp_cv_use_perfect_pixel_update.serializedObject.ApplyModifiedProperties();

                Canvas canvas = (Canvas)sp_cv_screen.objectReferenceValue;
                canvas.pixelPerfect = sp_cv_use_perfect_pixel_update.boolValue;
            });
            #endregion

            #region 自动像素对齐
            DrawToggle("自动像素对齐", sp_cv_use_auto_perfect_pixel, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, (b) => { });
            #endregion

            XGUI.layout_seperator(
                thickness: 1,
                color: XHud_Dashboard.themes_get_color(ThemeColorType.sepline),
                margin: new RectOffset(15, 15, 15, 15));

            #region 相机投影
            DrawToggle("正交相机投影", sp_camera_othograpic_mode, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, (b) =>
            {
                sp_camera_othograpic_mode.boolValue = b;
                sp_camera_othograpic_mode.serializedObject.ApplyModifiedProperties();

                Camera cam = (Camera)sp_camera_ui.objectReferenceValue;
                cam.orthographic = sp_camera_othograpic_mode.boolValue;
            });
            #endregion

            #region 音效静音
            DrawToggle("音效静音", sp_global_volume_mute, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, (b) => { });
            #endregion

            #region 个性光标
            DrawToggle("个性光标", sp_mos_custom_cursor, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, (b) =>
            {
                sp_mos_custom_cursor.boolValue = b;
                sp_mos_custom_cursor.serializedObject.ApplyModifiedProperties();

                if (!sp_mos_custom_cursor.boolValue)
                {
                    if (sp_mos_mouse_cursor.objectReferenceValue != null)
                    {
                        XHud_CustomMouseCursor mouseCursor = (XHud_CustomMouseCursor)sp_mos_mouse_cursor.objectReferenceValue;

                        EditorApplication.delayCall += () =>
                        {
                            XGUI.dialog(
                                type: XGUIDialogType.警告,
                                windowtitle: "XHud - 管理器消息",
                                title: "自定义光标",
                                msg: "当前已经存在自定义鼠标样式器，如果选择禁用，那么将会立即移除它，并且您为其配置好的参数也将一并丢失，确认要这样操作吗？",
                                ok: "是的",
                                cancel: "暂不",
                                PrimaryIndex: 0,
                                usemodal: true,
                                themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                on_selected: (d) =>
                                {
                                    if (d == "是的")
                                    {
                                        DestroyImmediate(mouseCursor.gameObject, true);
                                        sp_mos_mouse_cursor.objectReferenceValue = null;
                                        sp_mos_mouse_cursor.serializedObject.ApplyModifiedProperties();
                                    }
                                    else
                                    {
                                        sp_mos_custom_cursor.boolValue = true;
                                        sp_mos_custom_cursor.serializedObject.ApplyModifiedProperties();
                                        return;
                                    }
                                });
                        };
                    }
                }
                else
                {
                    if (sp_mos_mouse_cursor.objectReferenceValue != null)
                        return;
                    Canvas canvas = (Canvas)sp_cv_screen.objectReferenceValue;
                    GameObject MouseStyler = new GameObject();
                    MouseStyler.name = "Cursor";
                    MouseStyler.layer = LayerMask.NameToLayer("XHud");
                    XHud_CustomMouseCursor cursor = MouseStyler.AddComponent<XHud_CustomMouseCursor>();
                    UnityEngine.RectTransform rect = MouseStyler.AddComponent<UnityEngine.RectTransform>();
                    rect.SetParent(canvas.transform);
                    rect.SetAsLastSibling();
                    rect.localPosition = Vector3.zero;
                    rect.localScale = Vector3.one;
                    cursor.cursor_rect = rect;
                    cursor.cursor_canvas_group = rect.GetComponent<CanvasGroup>();
                    cursor.mc_SetCustomCursorEnabled(true);
                    sp_mos_mouse_cursor.objectReferenceValue = cursor;
                }
            });
            #endregion

            #region 转场特效
            DrawToggle("转场特效", sp_trans_custom_transition, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, (b) =>
            {
                sp_trans_custom_transition.boolValue = b;
                sp_trans_custom_transition.serializedObject.ApplyModifiedProperties();

                if (!sp_trans_custom_transition.boolValue)
                {
                    if (sp_trans_transition_controller.objectReferenceValue != null)
                    {
                        XHud_TransitionController transition = (XHud_TransitionController)sp_trans_transition_controller.objectReferenceValue;

                        EditorApplication.delayCall += () =>
                        {
                            XGUI.dialog(
                                   type: XGUIDialogType.警告,
                                   windowtitle: "XHud - 管理器消息",
                                   title: "转场器",
                                   msg: "当前已经存在转场器，如果选择禁用，那么将会立即移除它，并且您为其配置好的参数也将一并丢失，确认要这样操作吗？",
                                   ok: "禁用",
                                   cancel: "暂不",
                                   PrimaryIndex: 0,
                                   usemodal: true,
                                   themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                   on_selected: (d) =>
                                   {
                                       if (d == "禁用")
                                       {
                                           transition.Transition_Set_MaskTexture(null);
                                           DestroyImmediate(transition.gameObject, true);
                                           sp_trans_transition_controller.objectReferenceValue = null;
                                           sp_trans_transition_controller.serializedObject.ApplyModifiedProperties();
                                       }
                                       else
                                       {
                                           sp_trans_custom_transition.boolValue = true;
                                           sp_trans_custom_transition.serializedObject.ApplyModifiedProperties();
                                           return;
                                       }
                                   });
                        };
                    }
                    if (sp_mk_mask != null)
                        sp_mk_mask.objectReferenceValue.name = "Mask";
                }
                else
                {
                    if (sp_mk_mask != null)
                        sp_mk_mask.objectReferenceValue.name = "Mask";

                    if (sp_trans_transition_controller.objectReferenceValue != null)
                        return;
                    Canvas canvas = (Canvas)sp_cv_screen.objectReferenceValue;

                    GameObject NewTransition = new GameObject();
                    NewTransition.name = "Transition";
                    NewTransition.layer = LayerMask.NameToLayer("XHud");
                    XHud_TransitionController trans = NewTransition.AddComponent<XHud_TransitionController>();

                    UnityEngine.RectTransform rect = NewTransition.AddComponent<UnityEngine.RectTransform>();
                    rect.SetParent(canvas.transform);
                    rect.SetAsLastSibling();
                    rect.localScale = Vector3.one;
                    rect.anchorMin = Vector2.zero;
                    rect.anchorMax = Vector2.one;
                    rect.pivot = Vector2.one * 0.5f;
                    rect.sizeDelta = Vector2.one * 4;
                    rect.anchoredPosition3D = Vector3.zero;

                    Material mat = AssetDatabase.LoadAssetAtPath<Material>($"{XHud_Dashboard.Get_Path_XHUD_MATERIALS_Path()}Transitions/Transition.mat");
                    Image img = NewTransition.AddComponent<Image>();
                    img.raycastTarget = false;
                    img.color = Color.white;
                    img.material = mat;

                    trans.transition_mat = mat;
                    trans.transition_image = img;
                    sp_trans_transition_controller.objectReferenceValue = trans;
                }
            });
            #endregion

            XGUI.layout_seperator(
               thickness: 1,
               color: XHud_Dashboard.themes_get_color(ThemeColorType.sepline),
               margin: new RectOffset(15, 15, 15, 15));

            #region 支持世界元素
            DrawToggle("支持世界元素", sp_cv_support_world_ui, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, (b) =>
            {
                sp_cv_support_world_ui.boolValue = b;
                sp_cv_support_world_ui.serializedObject.ApplyModifiedProperties();

                if (!sp_cv_support_world_ui.boolValue)
                {
                    if (sp_cv_hudcanvas_world.objectReferenceValue == null)
                        return;

                    EditorApplication.delayCall += () =>
                    {
                        XGUI.dialog(
                               type: XGUIDialogType.警告,
                               windowtitle: "XHud - 管理器消息",
                               title: "支持世界元素",
                               msg: "此操作会移除World节点以及其下所有物体并禁用支持世界UI的特性，请确实是否继续该操作？",
                               ok: "继续",
                               cancel: "暂不",
                               PrimaryIndex: 0,
                               usemodal: true,
                               themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                               on_selected: (d) =>
                               {
                                   if (d == "继续")
                                   {
                                       Canvas cav_w = (Canvas)sp_cv_hudcanvas_world.objectReferenceValue;
                                       if (cav_w != null)
                                       {
                                           Undo.DestroyObjectImmediate(cav_w.gameObject);
                                           //DestroyImmediate(cav_w.gameObject, true);

                                           sp_op_hud_canvas_group_world.objectReferenceValue = null;
                                           sp_op_hud_canvas_group_world.serializedObject.ApplyModifiedProperties();

                                           sp_cv_hudcanvas_world.objectReferenceValue = null;
                                           sp_cv_hudcanvas_world.serializedObject.ApplyModifiedProperties();

                                           sp_cv_hudcanvas_world_anchor.objectReferenceValue = null;
                                           sp_cv_hudcanvas_world_anchor.serializedObject.ApplyModifiedProperties();
                                       }
                                   }
                                   else
                                   {
                                       sp_cv_support_world_ui.boolValue = true;
                                       sp_cv_support_world_ui.serializedObject.ApplyModifiedProperties();
                                       return;
                                   }
                               });
                    };
                }
                else
                {
                    ///-------Canvas
                    GameObject obj_cav_world = new GameObject();
                    obj_cav_world.name = "World";
                    obj_cav_world.layer = LayerMask.NameToLayer("XHud_World");
                    Canvas cav = obj_cav_world.AddComponent<Canvas>();
                    cav.renderMode = RenderMode.ScreenSpaceCamera;
                    cav.worldCamera = (Camera)sp_camera_scene.objectReferenceValue;
                    cav.renderMode = RenderMode.WorldSpace;
                    UnityEngine.RectTransform rect_cav = cav.GetComponent<UnityEngine.RectTransform>();
                    rect_cav.sizeDelta = new Vector2(100f, 100f);
                    rect_cav.SetParent(base_script.transform);
                    rect_cav.localPosition = Vector3.zero;
                    rect_cav.localEulerAngles = Vector3.zero;
                    rect_cav.localScale = Vector3.one * 0.01f;
                    rect_cav.SetSiblingIndex(rect_cav.parent.childCount - 2);
                    obj_cav_world.AddComponent<CanvasScaler>();
                    obj_cav_world.AddComponent<GraphicRaycaster>();

                    GameObject obj_anc_world = new GameObject();
                    obj_anc_world.layer = LayerMask.NameToLayer("XHud_World");
                    obj_anc_world.name = "Anchors";
                    UnityEngine.RectTransform rect_anchors = obj_anc_world.AddComponent<UnityEngine.RectTransform>();
                    rect_anchors.sizeDelta = new Vector2(1, 1);
                    rect_anchors.SetParent(rect_cav);
                    rect_anchors.localPosition = Vector3.zero;
                    rect_anchors.localEulerAngles = Vector3.zero;
                    rect_anchors.localScale = Vector3.one;

                    CanvasGroup cavgroup = obj_anc_world.AddComponent<CanvasGroup>();
                    sp_op_hud_canvas_group_world.objectReferenceValue = cavgroup;
                    sp_op_hud_canvas_group_world.serializedObject.ApplyModifiedProperties();
                    cavgroup.alpha = 1;

                    sp_cv_hudcanvas_world_anchor.objectReferenceValue = rect_anchors;
                    sp_cv_hudcanvas_world_anchor.serializedObject.ApplyModifiedProperties();

                    sp_cv_hudcanvas_world.objectReferenceValue = cav;
                    sp_cv_hudcanvas_world.serializedObject.ApplyModifiedProperties();
                }
            });
            #endregion

            #region R M S 模块
            DrawToggle("R M S 模块", sp_rms_enabled, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, (b) => { });
            #endregion

            XGUI.layout_seperator(
               thickness: 1,
               color: XHud_Dashboard.themes_get_color(ThemeColorType.sepline),
               margin: new RectOffset(15, 15, 15, 15));

            #region 画布尺寸           
            XGUI.layout_int_popup(
               title: "画布尺寸",
               title_width: 60,
               title_size: XGUIFontSize.M,
               title_anchor: TextAnchor.MiddleLeft,
               prop: sp_cv_scaler_mode_index,
               options: stroptions_canvassize,
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
                   sp_cv_scaler_mode_index.intValue = value;
                   sp_cv_scaler_mode_index.serializedObject.ApplyModifiedProperties();

                   CanvasScaler scaler = (CanvasScaler)sp_cv_scaler.objectReferenceValue;
                   Undo.RecordObject(scaler, "CanvasScalerChange");
                   if (scaler != null)
                   {
                       if (sp_cv_scaler_mode_index.intValue == 0)
                       {
                           scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
                       }
                       if (sp_cv_scaler_mode_index.intValue == 1)
                       {
                           scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                       }
                       if (sp_cv_scaler_mode_index.intValue == 2)
                       {
                           scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPhysicalSize;
                       }
                   }
                   sp_cv_scaler.objectReferenceValue = scaler;
                   sp_cv_scaler.serializedObject.ApplyModifiedProperties();
               });
            #endregion

            #region 画布距离           
            XGUI.layout_int_popup(
               title: "画布距离",
               title_width: 60,
               title_size: XGUIFontSize.M,
               title_anchor: TextAnchor.MiddleLeft,
               prop: sp_cv_hudcanvas_anchor_index,
               options: stroptions_canvasanchor,
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
                   sp_cv_hudcanvas_anchor_index.intValue = value;
                   sp_cv_hudcanvas_anchor_index.serializedObject.ApplyModifiedProperties();

                   sp_cv_hudcanvas_anchor.enumValueIndex = sp_cv_hudcanvas_anchor_index.intValue;
               });
            #endregion
        }
        #endregion
    }
}