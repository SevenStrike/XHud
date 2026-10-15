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
            DrawToggle("单例模式", UseInstanceMode, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, (b) => { });
            #endregion

            #region 状态调试
            DrawToggle("状态调试", UseDebug, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, (b) => { });
            #endregion

            #region 辅助结构
            DrawToggle("辅助结构", SafeFrameStructureDisplayer, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, (b) => { });
            #endregion

            XGUI.layout_seperator(
                thickness: 1,
                color: XHud_Dashboard.Theme_SeperateLine,
                margin: new RectOffset(15, 15, 15, 15));

            #region 辅助视觉
            if (SceneCamera.objectReferenceValue != null)
            {
                DrawToggle("辅助视觉", UseSafeFrame, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, (b) =>
                {
                    UseSafeFrame.boolValue = b;
                    UseSafeFrame.serializedObject.ApplyModifiedProperties();

                    if (!b)
                    {
                        BaseScript.hm_AnchorMarks_Destroy();
                    }
                    else
                    {
                        BaseScript.hm_AnchorMarks_Create();
                    }

                    SetHighFrequencyMode();
                    xHud_EditorUpdate_HelperVisual();
                    return;
                });
            }
            #endregion

            #region 构图参考视觉
            DrawToggle("构图参考视觉", UseCompGuide, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, (b) =>
            {
                if (UseCompGuide.boolValue)
                {
                    if (CompGuide_AnchorRoot.objectReferenceValue != null)
                    {
                        return;
                    }
                    xHud_CompGuide_Structure_Create();
                }
                else
                {
                    if (CompGuide_AnchorRoot.objectReferenceValue == null)
                    {
                        return;
                    }
                    xHud_CompGuide_Structure_Destroy();
                }
            });
            #endregion

            #region 蓝图视觉
            DrawToggle("蓝图视觉", BluePrintMode, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, (b) =>
            {
                BluePrintMode.boolValue = b;
                BluePrintMode.serializedObject.ApplyModifiedProperties();

                if (!BluePrintMode.boolValue)
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
                            themecolor: XHud_Dashboard.Theme_Primary,
                            on_selected: (d) =>
                            {
                                if (d == "退出")
                                {
                                    BaseScript.hm_BluePrint_Remove();
                                }
                                else
                                {
                                    BaseScript.BluePrintMode = true;
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
                    if (string.IsNullOrEmpty(BluePrint_bg_name.stringValue))
                        BluePrint_bg_name.stringValue = BluePrintBgs_Name[BluePrintBgs_Name.Length - 1];
                    BluePrint_bg_name.serializedObject.ApplyModifiedProperties();
                    BaseScript.hm_BluePrint_Create(true, mat, title, subtitle);

                    #endregion

                    return;
                }
            });
            #endregion

            #region 关闭时折叠所有
            DrawToggle("关闭时折叠所有", FoldAllPanelWithDisabled, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, (b) => { });
            #endregion

            #region 元素放置器
            ElementVisualPlacer.boolValue = Editor_XHud_Tool_ElementVisualPlacer.GetEnabled();
            ElementVisualPlacer.serializedObject.ApplyModifiedProperties();

            DrawToggle("元素放置器", ElementVisualPlacer, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, (b) =>
            {
                ElementVisualPlacer.boolValue = b;
                ElementVisualPlacer.serializedObject.ApplyModifiedProperties();

                Editor_XHud_Tool_ElementVisualPlacer.SetEnabled(ElementVisualPlacer.boolValue);
                XGUI.x_Editor_Data_Set_With_Bool(MENU_KEY_VISUAL_PLACER, ElementVisualPlacer.boolValue);
            });
            #endregion

            XGUI.layout_seperator(
                thickness: 1,
                color: XHud_Dashboard.Theme_SeperateLine,
                margin: new RectOffset(15, 15, 15, 15));

            #region 像素对齐
            DrawToggle("像素对齐", UsePerfectPixelUpdate, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, (b) =>
            {
                UsePerfectPixelUpdate.boolValue = b;
                UsePerfectPixelUpdate.serializedObject.ApplyModifiedProperties();

                Canvas canvas = (Canvas)HudCanvas_Screen.objectReferenceValue;
                canvas.pixelPerfect = UsePerfectPixelUpdate.boolValue;
            });
            #endregion

            #region 自动像素对齐
            DrawToggle("自动像素对齐", UseAutoPerfectPixel, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, (b) => { });
            #endregion

            XGUI.layout_seperator(
                thickness: 1,
                color: XHud_Dashboard.Theme_SeperateLine,
                margin: new RectOffset(15, 15, 15, 15));

            #region 相机投影
            DrawToggle("相机投影方式", CameraOthograpicMode, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, (b) =>
            {
                CameraOthograpicMode.boolValue = b;
                CameraOthograpicMode.serializedObject.ApplyModifiedProperties();

                Camera cam = (Camera)HudCamera.objectReferenceValue;
                cam.orthographic = CameraOthograpicMode.boolValue;
            });
            #endregion

            #region 音效静音
            DrawToggle("音效静音", VolumeMute, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, (b) => { });
            #endregion

            #region 个性光标
            DrawToggle("个性光标", CustomCursor, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, (b) =>
            {
                CustomCursor.boolValue = b;
                CustomCursor.serializedObject.ApplyModifiedProperties();

                if (!CustomCursor.boolValue)
                {
                    if (Hud_MouseCursor.objectReferenceValue != null)
                    {
                        XHud_CustomMouseCursor mouseCursor = (XHud_CustomMouseCursor)Hud_MouseCursor.objectReferenceValue;

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
                                themecolor: XHud_Dashboard.Theme_Primary,
                                on_selected: (d) =>
                                {
                                    if (d == "是的")
                                    {
                                        DestroyImmediate(mouseCursor.gameObject, true);
                                        Hud_MouseCursor.objectReferenceValue = null;
                                        Hud_MouseCursor.serializedObject.ApplyModifiedProperties();
                                    }
                                    else
                                    {
                                        CustomCursor.boolValue = true;
                                        CustomCursor.serializedObject.ApplyModifiedProperties();
                                        return;
                                    }
                                });
                        };
                    }
                }
                else
                {
                    if (Hud_MouseCursor.objectReferenceValue != null)
                        return;
                    Canvas canvas = (Canvas)HudCanvas_Screen.objectReferenceValue;
                    GameObject MouseStyler = new GameObject();
                    MouseStyler.name = "Cursor";
                    MouseStyler.layer = LayerMask.NameToLayer("XHud");
                    XHud_CustomMouseCursor cursor = MouseStyler.AddComponent<XHud_CustomMouseCursor>();
                    UnityEngine.RectTransform rect = MouseStyler.AddComponent<UnityEngine.RectTransform>();
                    rect.SetParent(canvas.transform);
                    rect.SetAsLastSibling();
                    rect.localPosition = Vector3.zero;
                    rect.localScale = Vector3.one;
                    cursor.CursorRect = rect;
                    cursor.CursorCanvasGroup = rect.GetComponent<CanvasGroup>();
                    cursor.mc_SetCustomCursorEnabled(true);
                    Hud_MouseCursor.objectReferenceValue = cursor;
                }
            });
            #endregion

            #region 转场特效
            DrawToggle("转场特效", CustomTransition, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, (b) =>
            {
                CustomTransition.boolValue = b;
                CustomTransition.serializedObject.ApplyModifiedProperties();

                if (!CustomTransition.boolValue)
                {
                    if (Hud_TransitionController.objectReferenceValue != null)
                    {
                        XHud_TransitionController transition = (XHud_TransitionController)Hud_TransitionController.objectReferenceValue;

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
                                   themecolor: XHud_Dashboard.Theme_Primary,
                                   on_selected: (d) =>
                                   {
                                       if (d == "禁用")
                                       {
                                           transition.Transition_Set_MaskTexture(null);
                                           DestroyImmediate(transition.gameObject, true);
                                           Hud_TransitionController.objectReferenceValue = null;
                                           Hud_TransitionController.serializedObject.ApplyModifiedProperties();
                                       }
                                       else
                                       {
                                           CustomTransition.boolValue = true;
                                           CustomTransition.serializedObject.ApplyModifiedProperties();
                                           return;
                                       }
                                   });
                        };
                    }
                    if (Mask != null)
                        Mask.objectReferenceValue.name = "Mask";
                }
                else
                {
                    if (Mask != null)
                        Mask.objectReferenceValue.name = "Mask";

                    if (Hud_TransitionController.objectReferenceValue != null)
                        return;
                    Canvas canvas = (Canvas)HudCanvas_Screen.objectReferenceValue;

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

                    trans.TransitionMat = mat;
                    trans.TransitionImage = img;
                    Hud_TransitionController.objectReferenceValue = trans;
                }
            });
            #endregion

            XGUI.layout_seperator(
               thickness: 1,
               color: XHud_Dashboard.Theme_SeperateLine,
               margin: new RectOffset(15, 15, 15, 15));

            #region 支持世界元素
            DrawToggle("支持世界元素", SupportWorldUI, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, (b) =>
            {
                SupportWorldUI.boolValue = b;
                SupportWorldUI.serializedObject.ApplyModifiedProperties();

                if (!SupportWorldUI.boolValue)
                {
                    if (HudCanvas_World.objectReferenceValue == null)
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
                               themecolor: XHud_Dashboard.Theme_Primary,
                               on_selected: (d) =>
                               {
                                   if (d == "继续")
                                   {
                                       Canvas cav_w = (Canvas)HudCanvas_World.objectReferenceValue;
                                       if (cav_w != null)
                                       {
                                           Undo.DestroyObjectImmediate(cav_w.gameObject);
                                           //DestroyImmediate(cav_w.gameObject, true);

                                           HudCanvasGroup_World.objectReferenceValue = null;
                                           HudCanvasGroup_World.serializedObject.ApplyModifiedProperties();

                                           HudCanvas_World.objectReferenceValue = null;
                                           HudCanvas_World.serializedObject.ApplyModifiedProperties();

                                           HudCanvas_WorldAnchor.objectReferenceValue = null;
                                           HudCanvas_WorldAnchor.serializedObject.ApplyModifiedProperties();
                                       }
                                   }
                                   else
                                   {
                                       SupportWorldUI.boolValue = true;
                                       SupportWorldUI.serializedObject.ApplyModifiedProperties();
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
                    cav.worldCamera = (Camera)SceneCamera.objectReferenceValue;
                    cav.renderMode = RenderMode.WorldSpace;
                    UnityEngine.RectTransform rect_cav = cav.GetComponent<UnityEngine.RectTransform>();
                    rect_cav.sizeDelta = new Vector2(100f, 100f);
                    rect_cav.SetParent(BaseScript.transform);
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
                    HudCanvasGroup_World.objectReferenceValue = cavgroup;
                    HudCanvasGroup_World.serializedObject.ApplyModifiedProperties();
                    cavgroup.alpha = 1;

                    HudCanvas_WorldAnchor.objectReferenceValue = rect_anchors;
                    HudCanvas_WorldAnchor.serializedObject.ApplyModifiedProperties();

                    HudCanvas_World.objectReferenceValue = cav;
                    HudCanvas_World.serializedObject.ApplyModifiedProperties();
                }
            });
            #endregion

            #region R M S 模块
            DrawToggle("R M S 模块", RMS_Enabled, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, (b) => { });
            #endregion

            XGUI.layout_seperator(
               thickness: 1,
               color: XHud_Dashboard.Theme_SeperateLine,
               margin: new RectOffset(15, 15, 15, 15));

            #region 画布尺寸           
            XGUI.layout_int_popup(
               title: "画布尺寸",
               title_width: 60,
               title_size: XGUIFontSize.M,
               title_anchor: TextAnchor.MiddleLeft,
               prop: CanvasScalerModeIndex,
               options: stroptions_canvassize,
               opt_text_size: XGUIFontSize.M,
               opt_text_color: Color.black,
               opt_text_padding: new RectOffset(10, 10, 0, 0),
               opt_anchor: TextAnchor.MiddleLeft,
               opt_font_style: FontStyle.Normal,
               opt_bg_fill: XGUIGroup.实体,
               opt_bg_color: XGUIColor.亮白,
               opt_bg_color_gui: XHud_Dashboard.Theme_Primary,
               icon_arrow_color: Color.black,
               margin: new RectOffset(0, 0, 5, 5),
               padding: new RectOffset(5, 5, 0, 0),
               title_margin: new RectOffset(0, 0, 0, 0),
               act_on_changed: (value) =>
               {
                   CanvasScalerModeIndex.intValue = value;
                   CanvasScalerModeIndex.serializedObject.ApplyModifiedProperties();

                   CanvasScaler scaler = (CanvasScaler)HudCanvasScaler.objectReferenceValue;
                   Undo.RecordObject(scaler, "CanvasScalerChange");
                   if (scaler != null)
                   {
                       if (CanvasScalerModeIndex.intValue == 0)
                       {
                           scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
                       }
                       if (CanvasScalerModeIndex.intValue == 1)
                       {
                           scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                       }
                       if (CanvasScalerModeIndex.intValue == 2)
                       {
                           scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPhysicalSize;
                       }
                   }
                   HudCanvasScaler.objectReferenceValue = scaler;
                   HudCanvasScaler.serializedObject.ApplyModifiedProperties();
               });
            #endregion

            #region 画布距离           
            XGUI.layout_int_popup(
               title: "画布距离",
               title_width: 60,
               title_size: XGUIFontSize.M,
               title_anchor: TextAnchor.MiddleLeft,
               prop: HudCanvasAnchorIndex,
               options: stroptions_canvasanchor,
               opt_text_size: XGUIFontSize.M,
               opt_text_color: Color.black,
               opt_text_padding: new RectOffset(10, 10, 0, 0),
               opt_anchor: TextAnchor.MiddleLeft,
               opt_font_style: FontStyle.Normal,
               opt_bg_fill: XGUIGroup.实体,
               opt_bg_color: XGUIColor.亮白,
               opt_bg_color_gui: XHud_Dashboard.Theme_Primary,
               icon_arrow_color: Color.black,
               margin: new RectOffset(0, 0, 5, 5),
               padding: new RectOffset(5, 5, 0, 0),
               title_margin: new RectOffset(0, 0, 0, 0),
               act_on_changed: (value) =>
               {
                   HudCanvasAnchorIndex.intValue = value;
                   HudCanvasAnchorIndex.serializedObject.ApplyModifiedProperties();

                   HudCanvasAnchor.enumValueIndex = HudCanvasAnchorIndex.intValue;
               });
            #endregion
        }
        #endregion
    }
}