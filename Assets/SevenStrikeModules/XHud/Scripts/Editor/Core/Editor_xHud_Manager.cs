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
    using System.Reflection;
    using TMPro;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.InputSystem.UI;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;
    using UnityEngine.UI;

    [CustomEditor(typeof(XHud_Manager))]
    public class Editor_XHud_Manager : Editor
    {
        #region 组件 / 列表
        public XHud_Manager BaseScript;
        private ReorderableList
            List_ElementLib,
            List_RMS,
            List_SoundsPool;
        #endregion

        private float LineHeight;
        static MethodInfo getGroup;
        static object gameViewSizesInstance;

        #region 频率更新
        private double _lastSceneUpdate;
        private double _lastHierarchyUpdate;
        private double _lastUserActionTime;
        private double UPDATE_KEEPTIME = 0.6f; // 自动降频阈值
        private double UPDATE_INTERVAL = 1; // 每秒最1次
        private double UPDATE_INTERVAL_Low = 0.033f; // 低频
        private double UPDATE_INTERVAL_High = 0.005f; // 高频
        #endregion

        private const string MENU_KEY_VISUAL_PLACER = "XHud_ElementVisualPlacer_Enabled";

        #region 序列化属性
        SerializedProperty
            IsInitialized,
            Lib_ElementLibrarys,
            SoundLibrary,
            SounderPoolCount,
            Anchors_Layout_Screen,
            HudCanvas_ScreenAnchor,
            Lib_Color,
            Lib_Curve,
            Lib_Sound,
            Lib_TextStyleLibrary,
            Lib_Motions,
            Lib_Transition,
            HudCamera,
            FontSizeMultiply,
            CanvasScalerModeIndex,
            CanvasScalerScreenSize,
            CanvasMatchDir,
            HudCanvas_Screen,
            HudCanvas_World,
            HudCanvasScaler,
            HudCanvas_WorldAnchor,
            Mask,
            HudCanvasGroup_Screen,
            HudCanvasGroup_World,
            UseInstanceMode,
            UseDebug,
            UseSafeFrame,
            UseAutoPerfectPixel,
            UsePerfectPixelUpdate,
            SafeFrameStructureDisplayer,
            CameraOthograpicMode,
            CustomCursor,
            CustomTransition,
            SupportWorldUI,
            Safe_Frame,
            Margins,
            MarginHorizontal,
            MarginMultiply,
            MarginVertical,
            MarkSize,
            Color_LayoutAnchorMark,
            Color_FrameLine,
            ScreenRes,
            Safe_FrameLine_Width,
            Safe_FrameLine_Margins,
            Color_SeperaterLine,
            Safe_Seperater_Length,
            Safe_CenterMarkLength,
            Safe_CenterMarkWidth,
            Color_CenterMark,
            Safe_CenterMarkDistance,
            CameraOrthographicSize,
            CameraFov,
            CameraCutter_Near,
            CameraCutter_Far,
            HudCanvasAnchor,
            CanvasDistance,
            HudCanvasAnchorIndex,
            MaskAlpha,
            MaskTexture,
            MaskRaycastAlphaThreshold,
            MaskRaycastEnabled,
            MaskColor,
            ContentOpacity_Screen,
            ContentOpacity_World,
            DurationMultiply,
            Hud_MouseCursor,
            Hud_TransitionController,
            RecycleArgs_Default,
            CreateArgs_Default,
            SceneCamera,
            Volume,
            VolumeMute,
            BluePrint_root,
            BluePrintMode,
            BluePrint_grid_size,
            BluePrint_grid_color,
            BluePrint_bg_color,
            BluePrint_bg_decal_color,
            BluePrint_mark_size,
            BluePrint_mark_opacity,
            BluePrint_linewidth,
            BluePrint_title_content,
            BluePrint_subtitle_content,
            BluePrint_marktitle_color,
            BluePrint_marksubtitle_color,
            BluePrint_mark_margin,
            BluePrint_MarkAnchors,
            BluePrint_mark_space,
            BluePrint_opacity,
            BluePrint_AnimationDuration,
            BluePrint_Displayed,
            BluePrint_OnStartHide,
            BluePrint_Grid_AnimationEase,
            BluePrint_grid_Opacity,
            BluePrint_Grid_AnimationDuration,
            BluePrint_Bg_AnimationEase_In,
            BluePrint_Bg_AnimationEase_Out,
            BluePrint_bg_opacity,
            BluePrint_Bg_FadeAnimationDelay,
            BluePrint_Grid_LengthPercentage,
            BluePrint_Grid_LevelHeight,
            BluePrint_GridEnd,
            BluePrint_bg_tilling,
            BluePrint_bg_name,
            BluePrint_bg_usetilling_index,
            BluePrint_bg_usesquareratio_index,
            BluePrint_bg_mapOpacity,
            Eft_Grid,
            Eft_GridFade,
            Eft_Bg,
            Eft_Mark,
            RMS_Enabled,
            RMS_CurrentSolution,
            RMS_Nodes,
            theme_color,
            theme_color_gp,
            theme_color_sep,
            ThemeSolution,
            ThemeEdgeSolution,
            Hud_EventSystem,
            Hud_InputSystemUIInputModule,
            CreateArgs_MotionAnimateEndState,
            RecycleArgs_MotionAnimateEndState,
            Crc_Lib_Name,
            Rec_Lib_Name,
            sp_PhysicsScreenSize,
            sp_Reference_Image,
            UseRatioReference,
            RatioReferenceIsPart,
            sp_Reference_Image_Color,
            sp_Res_Full,
            sp_Res_Part,
            sp_ReferShape_RatioSize,
            sp_ReferShape_RatioTolerance,
            FoldAllPanelWithDisabled,
            CompGuide_Anchors,
            CompGuide_AnchorRoot,
            UseCompGuide,
            CompGuideMode,
            GuideColor,
            GuidePointColor,
            GuideParam_Mirror_LR_Offset,
            GuideParam_Mirror_UD_Offset,
            GuideParam_Mirror_LR_GoldenMode,
            GuideParam_Mirror_UD_GoldenMode,
            GuideParam_Fibonacci_Mode,
            GuideParam_CornerLookat_Offset_H,
            GuideParam_CornerLookat_Offset_V,
            GuideParam_Three_Offset_H,
            GuideParam_Three_Offset_Coverage,
            GuideParam_Three_Offset_V,
            GuideParam_GuideLine_BaseHeight,
            GuideParam_GuideLine_Offset_Near,
            GuideParam_GuideLine_Offset_Far,
            GuideParam_GuideLine_Offset_NearHeight,
            GuideParam_Triangle_BaseHeight,
            GuideParam_Triangle_BottomHeight,
            GuideParam_Triangle_Offset_Left,
            GuideParam_Triangle_Offset_Right,
            GuideParam_Triangle_TopOffset,
            GuideParam_CenterPointSize,
            GuideParam_IShape_TopHeight,
            GuideParam_IShape_BottomHeight,
            GuideParam_IShape_Offset_Left,
            GuideParam_IShape_Offset_Right,
            GuideParam_GuideLine_BaseOffset,
            ElementVisualPlacer,
            PrimtiveID_LabelLine_Height,
            PrimtiveID_LabelLine_Color,
            PrimtiveID_LabelFont_Size,
            PrimtiveID_LabelFont_Color;
        #endregion

        #region 图标
        public Texture2D status, rmsicon, icon_MC_Use, icon_MC_None, soundplaying, panel_option, panel_canvas, panel_camera, panel_libs, panel_elementlibs, panel_frame, panel_assist, panel_bulueprint, panel_matchres, panel_audios, panel_mask, panel_global, panel_tools, panel_eleparamrecycle_auto, panel_eleparammotion_default, panel_theme, panel_localized, panel_physicsratio, save_r, save_p, locate_r, locate_p, reset_r, reset_p, icon_initia_r, icon_initia_p, icon_libs_create_r, icon_libs_create_p, icon_safeframevisual_r, icon_safeframevisual_p, icon_checkstate_r, icon_checkstate_p, icon_ray_normal, icon_ray_masked, panel_comp_guide, GuideHelp_Released, GuideHelp_Press, logo_bg, logo, themes_mark;
        #endregion

        #region 开关
        private bool def_recycle_fold_move, def_recycle_fold_rotate, def_recycle_fold_alpha, def_create_fold_move, def_create_fold_rotate, def_create_fold_alpha;
        #endregion

        #region 蓝图
        List<Texture2D> BluePrintBgs = new List<Texture2D>();
        string[] BluePrintBgs_Name;
        #endregion

        #region 键
        private static string PrefsKeyColor_Theme = "XHUD-MANAGER-COLOR-THEME";
        private static string PrefsKeyColor_Theme_GP = "XHUD-MANAGER-COLOR-THEME-GROUP";
        private static string PrefsKeyColor_Theme_SEP = "XHUD-MANAGER-COLOR-THEME-SEPERATE";
        #endregion     

        #region 选项文字
        string[] stroptions_enabled = new string[2] { "关闭", "开启" }, stroptions_canvassize = new string[3] { "固定像素尺寸", "屏幕尺寸", "固定物理尺寸" }, stroptions_canvasanchor = System.Enum.GetNames(typeof(CanvasCameraAttachment)), ThemeSolutionNames = new string[] { "默认", "白色", "黑色", "乳白", "浅灰", "沙漠灰", "科幻青", "液晶绿", "橄榄绿", "湖蓝", "天蓝", "胭脂粉", "灵动粉", "秋叶黄", "警示黄", "高亮橘", "烈焰红", }, ThemeEdgeSolutionNames = new string[] { "默认", "白色", "浅灰", "深灰", "黑色", "极简", "珊瑚红", "落叶黄", "烟灰蓝", "青苔绿", "荧光绿", "淡粉" }, compguid_modes = new string[8] { "水平对称", "垂直对称", "黄金螺旋", "对角线", "三分线", "引导线", "三角", "工字型" };
        #endregion

        #region 批量化操作
        XHud_Manager[] SelectedObjects;


        private void Targets_Get()
        {
            if (targets.Length > 1)
            {
                SelectedObjects = new XHud_Manager[targets.Length];
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    var t = targets[i];
                    SelectedObjects[i] = (XHud_Manager)t;
                }
            }
            else
            {
                SelectedObjects = new XHud_Manager[targets.Length];
                SelectedObjects[0] = (XHud_Manager)target;
            }
        }

        private bool Targets_Selected()
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

        private Color color_important = new Color(1, 0.4f, 0.4f);

        private void OnEnable()
        {
            Undo.undoRedoPerformed += OnUndoRedoPerformed;

            BaseScript = (XHud_Manager)target;
            LineHeight = EditorGUIUtility.singleLineHeight;

            Targets_Get();

            #region 获取图标       
            icon_MC_Use = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/Icon_Anchor_MC_Use");
            icon_MC_None = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/Icon_Anchor_MC_None");
            status = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/status");
            rmsicon = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/rmsicon");
            soundplaying = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/soundplaying");
            save_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/save_r");
            save_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/save_p");
            locate_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/locate_r");
            locate_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/locate_p");
            reset_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/reset_r");
            reset_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/reset_p");

            #region 标题图标
            panel_option = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/panel_option");
            panel_canvas = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/panel_canvas");
            panel_camera = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/panel_camera");
            panel_libs = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/panel_libs");
            panel_elementlibs = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/panel_elementlibs");
            panel_frame = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/panel_frame");
            panel_assist = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/panel_assist");
            panel_bulueprint = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/panel_bulueprint");
            panel_matchres = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/panel_matchres");
            panel_audios = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/panel_audios");
            panel_mask = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/panel_mask");
            panel_global = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/panel_global");
            panel_tools = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/panel_tools");
            panel_eleparamrecycle_auto = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/panel_eleparamrecycle_auto");
            panel_eleparammotion_default = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/panel_eleparammotion_default");
            panel_localized = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/panel_localized");
            panel_theme = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/panel_theme");
            panel_physicsratio = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/panel_physics");
            panel_comp_guide = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/panel_CompGuide");
            #endregion

            icon_initia_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/icon_initia_r");
            icon_initia_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/icon_initia_p");
            icon_libs_create_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/icon_libs_create_r");
            icon_libs_create_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/icon_libs_create_p");
            icon_safeframevisual_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/icon_safeframevisual_r");
            icon_safeframevisual_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/icon_safeframevisual_p");
            icon_checkstate_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/icon_checkstate_r");
            icon_checkstate_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/icon_checkstate_p");

            GuideHelp_Released = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/GuideHelp_Released");
            GuideHelp_Press = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/GuideHelp_Press");

            icon_ray_normal = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/icon_ray_normal");
            icon_ray_masked = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/icon_ray_masked");

            themes_mark = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/icon_themes_mark");

            logo_bg = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/logo_bg");
            logo = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/logo");
            #endregion

            xHud_SerializedAllVariables();

            #region 获取Mask遮罩层
            if (!Application.isPlaying)
            {
                if (HudCanvas_ScreenAnchor.objectReferenceValue != null)
                {
                    if (Anchors_Layout_Screen.arraySize <= 0)
                    {
                        UnityEngine.RectTransform root = HudCanvas_ScreenAnchor.objectReferenceValue as UnityEngine.RectTransform;
                        for (int i = 0; i < root.childCount; i++)
                        {
                            if (root.GetChild(i).name.Contains("Anchor"))
                            {
                                UnityEngine.RectTransform obj = root.GetChild(i).GetComponent<UnityEngine.RectTransform>();
                                Anchors_Layout_Screen.InsertArrayElementAtIndex(Anchors_Layout_Screen.arraySize);
                                Anchors_Layout_Screen.GetArrayElementAtIndex(i).FindPropertyRelative("Name").stringValue = obj.name;
                                Anchors_Layout_Screen.GetArrayElementAtIndex(i).FindPropertyRelative("Anchor").objectReferenceValue = obj;
                                if (obj.childCount > 0)
                                {
                                    for (int c = 0; c < obj.childCount; c++)
                                    {
                                        if (obj.GetChild(c).name == "Mark")
                                        {
                                            Image mark = obj.GetChild(c).GetComponent<Image>();
                                            Anchors_Layout_Screen.GetArrayElementAtIndex(i).FindPropertyRelative("Mark").objectReferenceValue = mark;
                                        }
                                    }
                                }
                            }
                            else
                                continue;
                        }
                    }
                }
            }
            #endregion

            ///--------寻找布局节点根物体
            HudCanvas_ScreenAnchor.objectReferenceValue = BaseScript.transform.Find("Screen/Anchors");
            HudCanvas_ScreenAnchor.serializedObject.ApplyModifiedProperties();

            xHud_GetElementPoolIndicators();

            #region 分辨率库
            List_RMS = new ReorderableList(serializedObject, RMS_Nodes)
            {
                draggable = false,
                displayAdd = !Application.isPlaying,
                displayRemove = !Application.isPlaying,
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "分辨率列表");
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    SerializedProperty sp_node = RMS_Nodes.GetArrayElementAtIndex(index);
                    SerializedProperty sp_node_indicator = sp_node.FindPropertyRelative("Indicator");
                    SerializedProperty sp_node_res = sp_node.FindPropertyRelative("Res");
                    if (sp_node != null)
                    {
                        if (XGUI.GetCurrentWindowWidth() > 120)
                        {
                            XGUI.gui_property_field(
                            rect: new Rect(rect.x + 30, rect.y + 3, (XGUI.GetCurrentWindowWidth() > 180 ? (rect.width / 2) : rect.width) - 40, 20),
                            title: null,
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 80,
                            prop: sp_node_indicator);
                        }

                        if (XGUI.GetCurrentWindowWidth() > 180)
                        {
                            XGUI.gui_property_field(
                            rect: new Rect(rect.x + (rect.width - (rect.width / 2)), rect.y + 3, (rect.width / 2) - 5, 20),
                            title: null,
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 80,
                            prop: sp_node_res);
                        }

                        XGUI.gui_icon(
                            rect: new Rect(rect.x + 5, rect.y + 3, 14, 14),
                            icon: rmsicon,
                            color: ScreenRes.vector2Value != sp_node_res.vector2Value ? Color.gray : XHud_Dashboard.Theme_Primary);
                    }
                },
                elementHeightCallback = index =>
                {
                    return 1.5f * LineHeight;
                },
                onAddCallback = (ReorderableList list) =>
                {
                    RMS_Nodes.InsertArrayElementAtIndex(list.count);
                },
                onRemoveCallback = (ReorderableList list) =>
                {
                    RMS_Nodes.DeleteArrayElementAtIndex(list.index);
                },
                onSelectCallback = (ReorderableList list) =>
                {
                    SerializedProperty indicator = RMS_Nodes.GetArrayElementAtIndex(list.index).FindPropertyRelative("Indicator");
                    RMS_CurrentSolution.stringValue = indicator.stringValue;
                    xHud_SwitchResolution(indicator.stringValue);
                }
            };
            #endregion

            #region 元素库
            List_ElementLib = new ReorderableList(serializedObject, Lib_ElementLibrarys)
            {
                draggable = !Application.isPlaying,
                displayAdd = !Application.isPlaying,
                displayRemove = !Application.isPlaying,
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "元素库列表");
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    SerializedProperty sp_elementlib = Lib_ElementLibrarys.GetArrayElementAtIndex(index);
                    XHud_Library_Element element = sp_elementlib.objectReferenceValue as XHud_Library_Element;

                    XGUI.gui_property_field(
                        rect: new Rect(rect.x, rect.y + 3, rect.width - 5, 20),
                        title: null,
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 80,
                        prop: sp_elementlib);
                },
                elementHeightCallback = index =>
                {
                    return 1.5f * LineHeight;
                },
                onAddCallback = (ReorderableList list) =>
                {
                    Lib_ElementLibrarys.InsertArrayElementAtIndex(list.count);
                },
                onRemoveCallback = (ReorderableList list) =>
                {
                    Lib_ElementLibrarys.DeleteArrayElementAtIndex(list.index);
                },
                onSelectCallback = (ReorderableList list) =>
                {

                }
            };
            #endregion

            #region 音效池
            List_SoundsPool = new ReorderableList(serializedObject, SoundLibrary)
            {
                draggable = !Application.isPlaying,
                displayAdd = !Application.isPlaying,
                displayRemove = !Application.isPlaying,
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "音效池列表");
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    SerializedProperty sp_Soundlib = SoundLibrary.GetArrayElementAtIndex(index);
                    SerializedProperty player = sp_Soundlib.FindPropertyRelative("Player");
                    SerializedProperty isplaying = sp_Soundlib.FindPropertyRelative("IsPlaying");

                    XGUI.gui_icon(
                           rect: new Rect(rect.x + 5, rect.y + 6, 14, 14),
                           icon: soundplaying,
                           color: isplaying.boolValue ? XHud_Dashboard.Theme_Primary : Color.gray);

                    XGUI.gui_property_field(
                        rect: new Rect(rect.x + 28, rect.y + 5, rect.width - 25, 20),
                        title: new GUIContent(player.name),
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 80,
                        prop: player);
                },
                elementHeightCallback = index =>
                {
                    return 1.5f * LineHeight;
                },
                onAddCallback = (ReorderableList list) =>
                {
                    SoundLibrary.InsertArrayElementAtIndex(list.count);
                },
                onRemoveCallback = (ReorderableList list) =>
                {
                    SoundLibrary.DeleteArrayElementAtIndex(list.index);
                },
                onSelectCallback = (ReorderableList list) =>
                {

                }
            };
            #endregion

            #region 获取所有蓝图背景贴图
            Texture2D[] AllTextures = XGUI.LoadAllAssetsAtPathWithPattern<Texture2D>($"{XHud_Dashboard.Get_Path_XHUD_TEXTURES_Path()}BluePrints/", ".png").ToArray();

            BluePrintBgs_Name = new string[AllTextures.Length];
            for (int i = 0; i < AllTextures.Length; i++)
            {
                BluePrintBgs.Add(AllTextures[i]);
                BluePrintBgs_Name[i] = AllTextures[i].name;
            }
            #endregion

            BaseScript.hm_BluePrintRootFirstSibling();

            EditorApplication.hierarchyChanged -= EditorApplication_EditorManagerUpdate;
            EditorApplication.hierarchyChanged += EditorApplication_EditorManagerUpdate;

            xHud_InitiaGameViewResolutions();

            xHud_LoadThemesColor();

            // 创建图层 - XHud
            xHud_EnsureLayerExists("XHud");

            // 创建图层 - XHud_World
            xHud_EnsureLayerExists("XHud_World");

            //设置XHud管理器图层为XHud
            BaseScript.gameObject.layer = LayerMask.NameToLayer("XHud");

            xHud_RatioReference_Update();
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoRedoPerformed;

            BaseScript.hm_BluePrintRootFirstSibling();

            if (FoldAllPanelWithDisabled.boolValue)
                xHud_GroupFolds(false);
        }

        private void OnDestroy()
        {
            EditorApplication.hierarchyChanged -= EditorApplication_EditorManagerUpdate;
        }

        private void OnUndoRedoPerformed()
        {
            // 重新同步序列化属性
            serializedObject.Update();

            xHud_EditorUpdate_HelperVisual();
            xHud_EditorUpdate_Canvas_Camera();
            xHud_EditorUpdate_Mask();
            xHud_EditorUpdate_ContentOpacity();
            xHud_EditorUpdate_CameraArgs();

            // 重绘 Inspector
            Repaint();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            string hexcol = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary, true);

            float currentWidth = XGUI.GetCurrentWindowWidth();

            #region Logo 背景
            float width = logo_bg.width;
            float height = logo_bg.height;
            float _x = currentWidth / 2 - logo_bg.width / 2 + 5;
            float _y = 30;
            float rw = currentWidth * 0.575f;
            float ratio = width / height;

            XGUI.gui_icon(
              rect: new Rect(_x, _y, width, height),
              icon: logo_bg,
              color: Color.white * 0.7f);
            #endregion

            #region Logo 主体
            float w_logo_main = logo.width;
            float h_logo_main = logo.height;
            float x_main = _x + (width / 2) - (w_logo_main / 2) - 5;
            float y_main = _y + (height / 2) - (h_logo_main / 2);
            Rect rect_logo_main = new Rect(x_main, y_main, w_logo_main, h_logo_main);

            XGUI.gui_icon(
                rect: rect_logo_main,
                icon: logo);
            #endregion

            XGUI.layout_space(160);

            #region 基础
            XGUI.layout_group_start(
                type: XGUIContainerType.Horizontal,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.Theme_Group,
                title: "基础",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.Theme_Primary,
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(20, 20, 20, 20));

            // 运行时不可操作
            if (Application.isPlaying)
                XGUI.SetEnabled(false);
            else
                XGUI.SetEnabled(true);

            #region 初始化结构
            if (XGUI.layout_button(
              tooltip: "初始化结构",
              tex_release: icon_initia_r,
              tex_press: icon_initia_p,
              tex_gui_color: Color.white,
              width: 15,
              height: 15))
            {
                EditorApplication.hierarchyChanged -= EditorApplication_EditorManagerUpdate;
                EditorApplication.hierarchyChanged += EditorApplication_EditorManagerUpdate;

                xHud_StructureInitialize();
            }
            #endregion

            GUILayout.FlexibleSpace();

            if (IsInitialized.boolValue)
                XGUI.SetEnabled(true);
            else
                XGUI.SetEnabled(false);

            #region 生成默认资源库
            if (XGUI.layout_button(
            tooltip: "生成默认资源库",
            tex_release: icon_libs_create_r,
            tex_press: icon_libs_create_p,
            tex_gui_color: Color.white,
            width: 15,
            height: 15))
            {
                string path = EditorUtility.OpenFolderPanel("", Application.dataPath, "");

                if (string.IsNullOrEmpty(path))
                    return;

                string path_folder = path.Substring(Application.dataPath.Length - 6);

                //创建色卡库
                XHud_Library_Colors lib_color = ScriptableObject.CreateInstance<XHud_Library_Colors>();
                lib_color.LibraryName = $"NewColorsLibrary";
                lib_color.ColorsLibrary_AddColor("DefaultColor", Color.white);

                //创建曲线库
                XHud_Library_Curves lib_curve = ScriptableObject.CreateInstance<XHud_Library_Curves>();
                lib_curve.LibraryName = $"NewCurveLibrary";
                lib_curve.CurveLibrary_AddCurve("DefaultCurve", AnimationCurve.EaseInOut(0, 0, 1, 1));

                //创建音效库
                XHud_Library_Sounds lib_sound = ScriptableObject.CreateInstance<XHud_Library_Sounds>();
                lib_sound.LibraryName = $"NewSoundLibrary";
                lib_sound.SoundLibrary_AddSound("DefaultSound", AssetDatabase.LoadAssetAtPath<AudioClip>($"{XHud_Dashboard.Get_Path_XHUD_SOUND_Path()}DefaultSound.wav"), "wav");

                //创建元素库
                XHud_Library_Element lib_element = ScriptableObject.CreateInstance<XHud_Library_Element>();
                lib_element.LibraryName = $"NewElementLibrary";
                Transform obj_XHudSample = AssetDatabase.LoadAssetAtPath<Transform>($"{XHud_Dashboard.Get_Path_XHUD_PREFABS_Path()}XHudIcon.prefab");
                lib_element.ElementsLibrary_Add(new XHud_LibraryArg_Element_Item(1, obj_XHudSample.GetComponent<XHud_Module_Element>()));

                //创建字体库
                XHud_Library_TextStyle lib_textstyle = ScriptableObject.CreateInstance<XHud_Library_TextStyle>();
                lib_textstyle.LibraryName = $"NewTextStyleLibrary";

                XHud_LibraryArg_TextStyle info_text = new XHud_LibraryArg_TextStyle();
                info_text.Type = xHud_TextType.Text;
                info_text.RichText = true;
                info_text.Maskable = false;
                info_text.Raycast = false;
                info_text.SyncPrimitivePaintingColor = false;
                info_text.BestFit = false;
                info_text.GeometreAlign = false;
                info_text.Font = AssetDatabase.LoadAssetAtPath<Font>($"{XHud_Dashboard.Get_Path_XHUD_FONTS_Path()}Text/sx_light.otf");
                info_text.FontColor = Color.white;
                info_text.Size = 50;
                info_text.LineHeight = 1;
                info_text.Name = "NewTextStyle_HudText";
                info_text.Description = "这是一个用于HudText文字组件的默认字体样式";
                info_text.Style = FontStyle.Normal;
                info_text.Overflow_Horizon = HorizontalWrapMode.Overflow;
                info_text.Overflow_Vertical = VerticalWrapMode.Overflow;
                info_text.ContentAnchor = ContentAnchor.中心;

                XHud_LibraryArg_TextStyle info_tmptext = new XHud_LibraryArg_TextStyle();
                info_tmptext.Type = xHud_TextType.TmpText;
                info_tmptext.tmp_rich = true;
                info_tmptext.Maskable = false;
                info_tmptext.Raycast = false;
                info_tmptext.SyncPrimitivePaintingColor = false;
                info_tmptext.tmp_EnableAutoSizing = false;
                info_tmptext.tmp_font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>($"{XHud_Dashboard.Get_Path_XHUD_FONTS_Path()}Tmp/sx_light SDF.asset");
                info_tmptext.tmp_color = Color.white;
                info_tmptext.Size = 50;
                info_tmptext.tmp_overflow = TextOverflowModes.Overflow;
                info_tmptext.tmp_contentwrap = TextWrappingModes.NoWrap;
                info_tmptext.tmp_anchor = TmpContentAnchor.中心;
                info_tmptext.tmp_space_lineheight = 1;
                info_tmptext.Name = "NewTextStyle_HudTmpText";
                info_tmptext.Description = "这是一个用于HudTmpText文字组件的默认字体样式";
                info_tmptext.ContentAnchor = ContentAnchor.中心;

                lib_textstyle.TextStyle_Library_Add(info_text);
                lib_textstyle.TextStyle_Library_Add(info_tmptext);

                //创建动效库
                XHud_Library_Motion lib_motion = ScriptableObject.CreateInstance<XHud_Library_Motion>();
                lib_motion.LibraryName = $"NewMotionLibrary";

                #region 预制动效
                Motion_Creator crc = new Motion_Creator();
                crc.anchor = XHudAnchor.中心;
                crc.Movement = new MotionNode_Movement();
                crc.Movement.Movement = HudMotion_Movement.S_从下至上;
                crc.Movement.Distance = 100f;
                crc.Movement.Duration = 1f;
                crc.Movement.Delay = 0f;
                crc.Movement.CurveName = "";
                crc.Movement.Curve = BaseScript.Hud_Curves != null ? BaseScript.Hud_Curves.CurveLibrary_GetCurve(crc.Movement.CurveName) : null;
                crc.Movement.Ease = EaseMode.OutQuart;
                crc.Rotation = new MotionNode_Rotation();
                crc.Rotation.Rotation = HudMotion_Rotation.A_无旋转;
                crc.Rotation.Degree = 0f;
                crc.Rotation.Duration = 1f;
                crc.Rotation.Delay = 0f;
                crc.Rotation.CurveName = "";
                crc.Rotation.Curve = BaseScript.Hud_Curves != null ? BaseScript.Hud_Curves.CurveLibrary_GetCurve(crc.Rotation.CurveName) : null;
                crc.Rotation.Ease = EaseMode.OutQuart;
                crc.Alpha = new MotionNode_Alpha();
                crc.Alpha.Duration = 1f;
                crc.Alpha.Delay = 0f;
                crc.Alpha.CurveName = "";
                crc.Alpha.Curve = BaseScript.Hud_Curves != null ? BaseScript.Hud_Curves.CurveLibrary_GetCurve(crc.Alpha.CurveName) : null;
                crc.Alpha.Ease = EaseMode.OutQuart;

                Motion_Recycler rec = new Motion_Recycler();
                rec.Movement = new MotionNode_Movement();
                rec.Movement.Movement = HudMotion_Movement.D_从上至下;
                rec.Movement.Distance = 100f;
                rec.Movement.Duration = 1f;
                rec.Movement.Delay = 0f;
                rec.Movement.CurveName = "";
                rec.Movement.Curve = BaseScript.Hud_Curves != null ? BaseScript.Hud_Curves.CurveLibrary_GetCurve(rec.Movement.CurveName) : null;
                rec.Movement.Ease = EaseMode.OutQuart;
                rec.Rotation = new MotionNode_Rotation();
                rec.Rotation.Rotation = HudMotion_Rotation.A_无旋转;
                rec.Rotation.Degree = 0f;
                rec.Rotation.Duration = 1f;
                rec.Rotation.Delay = 0f;
                rec.Rotation.CurveName = "";
                rec.Rotation.Curve = BaseScript.Hud_Curves != null ? BaseScript.Hud_Curves.CurveLibrary_GetCurve(rec.Rotation.CurveName) : null;
                rec.Rotation.Ease = EaseMode.OutQuart;
                rec.Alpha = new MotionNode_Alpha();
                rec.Alpha.Duration = 1f;
                rec.Alpha.Delay = 0f;
                rec.Alpha.CurveName = "";
                rec.Alpha.Curve = BaseScript.Hud_Curves != null ? BaseScript.Hud_Curves.CurveLibrary_GetCurve(rec.Alpha.CurveName) : null;
                rec.Alpha.Ease = EaseMode.OutQuart;
                #endregion

                XHud.XHud_LibraryArg_Motion mot_crc = new XHud.XHud_LibraryArg_Motion();
                mot_crc.Name = "Motion_Element_Create";
                mot_crc.Des = "这是一个用于元素载入时的动效";
                mot_crc.Mode = 0;
                mot_crc.Crc = crc;
                mot_crc.Rec = rec;

                XHud.XHud_LibraryArg_Motion mot_rec = new XHud.XHud_LibraryArg_Motion();
                mot_rec.Name = "Motion_Element_Recycle";
                mot_rec.Des = "这是一个用于元素回收时的动效";
                mot_rec.Mode = 1;
                mot_rec.Crc = crc;
                mot_rec.Rec = rec;

                lib_motion.ElementMotion_Add(mot_crc);
                lib_motion.ElementMotion_Add(mot_rec);

                //创建转场库
                XHud_Library_Transition lib_transition = ScriptableObject.CreateInstance<XHud_Library_Transition>();
                lib_transition.LibraryName = $"NewTransitionLibrary";
                List<Texture2D> texs_list = XGUI.LoadAllAssetsAtPathWithPattern<Texture2D>($"{XHud_Dashboard.Get_Path_XHUD_TEXTURES_Path()}Transitions/Ink/", ".jpg");
                // 调用排序方法
                xHud_SortTexturesByNameSuffix(texs_list);
                lib_transition.TransitionLibrary_Add("InkPaint", texs_list.ToArray(), 1);
                XHud_LibraryArg_Transition node = lib_transition.TransitionLibrary_Get("InkPaint");
                #region 分析转场数据
                if (node.Frames == null || node.Frames.Count <= 0)
                {
                    node.TotalFramesCount = 0;
                    node.LastFrameIndex = 0;
                    node.Res = Vector2Int.zero;
                }
                else
                {
                    node.TotalFramesCount = node.Frames.Count;
                    node.LastFrameIndex = node.Frames.Count - 1;
                    for (int s = 0; s < node.Frames.Count; s++)
                    {
                        // 获取纹理的导入设置
                        TextureImporter importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(node.Frames[s])) as TextureImporter;

                        if (importer != null)
                        {
                            if (importer.npotScale != TextureImporterNPOTScale.None)
                            {
                                // 修改 m_NPOTScale 设置为 无_None
                                importer.npotScale = TextureImporterNPOTScale.None;

                                // 保存修改后的设置
                                importer.SaveAndReimport();
                            }
                        }
                    }
                    node.Res = new Vector2Int(node.Frames[0].width, node.Frames[0].height);
                }
                #endregion

                AssetDatabase.CreateAsset(lib_color, $"{path_folder}/Lib_Color.asset");
                AssetDatabase.CreateAsset(lib_curve, $"{path_folder}/Lib_Curve.asset");
                AssetDatabase.CreateAsset(lib_sound, $"{path_folder}/Lib_Sound.asset");
                AssetDatabase.CreateAsset(lib_element, $"{path_folder}/Lib_Element.asset");
                AssetDatabase.CreateAsset(lib_textstyle, $"{path_folder}/Lib_Textstyle.asset");
                AssetDatabase.CreateAsset(lib_motion, $"{path_folder}/Lib_Motion.asset");
                AssetDatabase.CreateAsset(lib_transition, $"{path_folder}/Lib_Transition.asset");

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                string res = XGUI.dialog(
                    type: XGUIDialogType.警告,
                    windowtitle: "XHud - 管理器消息",
                    title: "资源库引用",
                    msg: "是否要将创建的资源库全部指定到管理器中？",
                    ok: "指定",
                    cancel: "暂不",
                    PrimaryIndex: 0);

                if (res == "指定")
                {
                    BaseScript.Hud_Colors = lib_color;
                    BaseScript.Hud_Curves = lib_curve;
                    BaseScript.Hud_Sounds = lib_sound;
                    BaseScript.Hud_TextStyleLibrary = lib_textstyle;
                    BaseScript.Hud_Motions = lib_motion;
                    BaseScript.Hud_TransitionLib = lib_transition;
                    BaseScript.Hud_ElementLibrarys.Add(lib_element);
                }

                return;
            }
            #endregion

            GUILayout.FlexibleSpace();

            #region 辅助视觉切换
            if (XGUI.layout_button(
            tooltip: "辅助视觉切换",
            tex_release: icon_safeframevisual_r,
            tex_press: icon_safeframevisual_p,
            tex_gui_color: Color.white,
            width: 15,
            height: 15))
            {
                if (SceneCamera.objectReferenceValue == null)
                {
                    XGUI.dialog(
                        type: XGUIDialogType.警告,
                        windowtitle: "XHud - 管理器消息",
                        title: "辅助视觉",
                        msg: "当前未指定场景相机，无法启用辅助视觉！！",
                        ok: "明白",
                        PrimaryIndex: 0);
                    return;
                }

                UseSafeFrame.boolValue = !UseSafeFrame.boolValue;
                UseSafeFrame.serializedObject.ApplyModifiedProperties();

                if (!UseSafeFrame.boolValue)
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
            }
            #endregion

            GUILayout.FlexibleSpace();

            #region 检查配置状态
            if (XGUI.layout_button(
            tooltip: "检查配置状态",
            tex_release: icon_checkstate_r,
            tex_press: icon_checkstate_p,
            tex_gui_color: Color.white,
            width: 15,
            height: 15))
            {
                Editor_XHud_Tool_Diagnostic window = (Editor_XHud_Tool_Diagnostic)EditorWindow.GetWindow(typeof(Editor_XHud_Tool_Diagnostic), false, "XHUD 诊断/概况面板", true);
                XGUI.CenterEditorWindow(new Vector2Int(300, 560), window, false);
                window.Show();
                return;
            }
            #endregion

            #region 检查配置是否完善
            if (IsInitialized.boolValue)
            {
                if (xHud_Diagnostic())
                {
                    Rect rect_diagnostic = XGUI.GetLastRect();
                    XGUI.gui_icon(
                        rect: new Rect(rect_diagnostic.x + rect_diagnostic.width + 2, rect_diagnostic.y - 6, 10, 10),
                        icon: status,
                        color: Color.red);
                }
            }
            #endregion

            XGUI.SetEnabled(true);
            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            #endregion

            if (IsInitialized.boolValue)
            {
                #region 分辨率信息
                XGUI.layout_group_start(
                    type: XGUIContainerType.Horizontal,
                    bg_fill: XGUIFilled.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.Theme_Group,
                    title: "分辨率",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.Theme_Primary,
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(20, 20, 15, 15));

                string resinfo = string.Format("{0} x {1}", ScreenRes.vector2Value.x, ScreenRes.vector2Value.y);
                XGUI.layout_label(
                    text: resinfo,
                    size: XGUIFontSize.M,
                    text_color: XHud_Dashboard.Theme_Primary,
                    margin: new RectOffset(0, 0, 0, 0),
                    offset: new Vector2(0, 1),
                    clipping: TextClipping.Clip,
                    font_style: FontStyle.Normal,
                    anchor: TextAnchor.MiddleCenter,
                    font: XGUI.GetFont("xg-medium"));

                if (RMS_Nodes.arraySize > 0)
                {
                    XGUI.layout_label(
                        text: "RMS： " + RMS_CurrentSolution.stringValue,
                        size: XGUIFontSize.M,
                        text_color: Color.white * 0.7f,
                        margin: new RectOffset(0, 0, 0, 0),
                        offset: new Vector2(0, 1),
                        clipping: TextClipping.Clip,
                        font_style: FontStyle.Normal,
                        anchor: TextAnchor.MiddleCenter,
                        font: XGUI.GetFont("xg-medium"));
                }

                XGUI.layout_group_end(type: XGUIContainerType.Horizontal);

                #endregion

                #region 选项
                BaseScript.fold_options = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIFilled.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.Theme_Group,
                    title: "选项",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.Theme_Primary,
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: BaseScript.fold_options);

                if (!BaseScript.fold_options)
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
                       opt_bg_fill: XGUIFilled.实体,
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
                       opt_bg_fill: XGUIFilled.实体,
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

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                #endregion

                #region 画布 
                BaseScript.fold_canvasset = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIFilled.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.Theme_Group,
                    title: "画布",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.Theme_Primary,
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: BaseScript.fold_canvasset);

                if (!BaseScript.fold_canvasset)
                {
                    #region 画布尺寸    
                    XGUI.ChangedCheck_Start();
                    XGUI.layout_property_field(
                     title: "画布尺寸",
                     title_size: XGUIFontSize.M,
                     //title_color: Color.white,
                     title_hover_color: XHud_Dashboard.Theme_Primary,
                     title_width: 100,
                     status_icon: "icon_field_status",
                     status_icon_color: CanvasScalerScreenSize.vector2Value != Vector2.zero ? XHud_Dashboard.Theme_Primary : Color.red,
                     prop: CanvasScalerScreenSize,
                     prop_margin: new RectOffset(0, 0, 10, 10));
                    if (XGUI.ChangedCheck_End())
                    {
                        CanvasScaler scaler = (CanvasScaler)HudCanvasScaler.objectReferenceValue;
                        scaler.referenceResolution = CanvasScalerScreenSize.vector2Value;
                        ScreenRes.vector2Value = CanvasScalerScreenSize.vector2Value;
                    }
                    #endregion

                    #region 高度优先    
                    XGUI.ChangedCheck_Start();
                    XGUI.layout_property_field(
                     title: "高度优先",
                     title_size: XGUIFontSize.M,
                     //title_color: Color.white,
                     title_hover_color: XHud_Dashboard.Theme_Primary,
                     title_width: 100,
                     //status_icon: "icon_field_status",
                     //status_icon_color: CanvasMatchDir.floatValue != 0 ? XHud_Dashboard.Theme_Primary : Color.red,
                     prop: CanvasMatchDir,
                     prop_margin: new RectOffset(0, 0, 10, 10));
                    if (XGUI.ChangedCheck_End())
                    {
                        CanvasScaler scaler = (CanvasScaler)HudCanvasScaler.objectReferenceValue;
                        scaler.matchWidthOrHeight = CanvasMatchDir.floatValue;
                    }
                    #endregion

                    #region 画布距离     
                    if (HudCanvasAnchorIndex.intValue == 2)
                    {
                        XGUI.layout_space(5);

                        XGUI.layout_property_field(
                            title: "画布距离",
                            title_size: XGUIFontSize.M,
                            //title_color: Color.white,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 100,
                            prop: CanvasDistance,
                            prop_margin: new RectOffset(0, 0, 10, 10));
                    }
                    #endregion

                    #region 获取分辨率
                    if (XGUI.layout_button(
                        text: "获取分辨率",
                        tooltip: "",
                        bg_fill: XGUIFilled.实体,
                        bg_color: XGUIColor.亮白,
                        bg_color_gui: XHud_Dashboard.Theme_Primary,
                        button_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(XHud_Dashboard.Theme_Primary) ? Color.black : Color.white,
                        press_fill: XGUIFilled.实体,
                        press_color: XGUIColor.深空灰,
                        press_text_color: Color.white,
                        font_size: XGUIFontSize.M,
                        anchor: TextAnchor.MiddleCenter,
                        margin: new RectOffset(0, 0, 0, 0),
                        padding: new RectOffset(0, 0, 0, 0),
                        button_text_font: XGUI.GetFont("xg-medium")))
                    {
                        CanvasScalerScreenSize.vector2Value = xHud_GetMainGameViewSize();
                        CanvasScalerScreenSize.serializedObject.ApplyModifiedProperties();

                        CanvasScaler scaler = (CanvasScaler)HudCanvasScaler.objectReferenceValue;
                        scaler.referenceResolution = CanvasScalerScreenSize.vector2Value;

                        ScreenRes.vector2Value = CanvasScalerScreenSize.vector2Value;

                        xHud_RatioReference_Update();
                    }
                    #endregion

                    #region 说明
                    string msg = $"请注意！如果您的目标分辨率尺寸是<b><color={hexcol}> 大于 8192 x 8192 </color></b>的情况下，建议<b><color={hexcol}> 手动输入画布尺寸 </color></b>为您的目标分辨率尺寸！因为当您设置大于8192的分辨率时，Unity内部会将其钳位或缩放到可显示范围内！";

                    XGUI.layout_label(
                        text: msg,
                        bg_fill: XGUIFilled.实体,
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
                    #endregion
                }

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                #endregion

                #region 相机 
                BaseScript.fold_camera = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIFilled.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.Theme_Group,
                    title: "相机",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.Theme_Primary,
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: BaseScript.fold_camera);

                if (!BaseScript.fold_camera)
                {
                    XGUI.ChangedCheck_Start();

                    #region 正交尺寸
                    XGUI.layout_property_field(
                        title: "正交尺寸",
                        title_size: XGUIFontSize.M,
                        //title_color: Color.white,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 100,
                        //status_icon: "icon_field_status",
                        //status_icon_color: CameraOrthographicSize.floatValue != 0 ? XHud_Dashboard.Theme_Primary : Color.red,
                        prop: CameraOrthographicSize,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion

                    XGUI.layout_space(5);

                    #region 近距剪切
                    XGUI.layout_property_field(
                        title: "近距剪切",
                        title_size: XGUIFontSize.M,
                        //title_color: Color.white,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 100,
                        //status_icon: "icon_field_status",
                        //status_icon_color: CameraCutter_Near.floatValue != 0, XHud_Dashboard.Theme_Primary : Color.red,
                        prop: CameraCutter_Near,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion

                    XGUI.layout_space(5);

                    #region 远距剪切
                    XGUI.layout_property_field(
                        title: "远距剪切",
                        title_size: XGUIFontSize.M,
                        //title_color: Color.white,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 100,
                        //status_icon: "icon_field_status",
                        //status_icon_color: CameraCutter_Far.floatValue != 0 ? XHud_Dashboard.Theme_Primary : Color.red,
                        prop: CameraCutter_Far,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion

                    XGUI.layout_space(5);

                    #region 视场角
                    if (!CameraOthograpicMode.boolValue)
                    {
                        XGUI.layout_space(5);

                        XGUI.layout_property_field(
                        title: "视场角",
                        title_size: XGUIFontSize.M,
                        //title_color: Color.white,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 100,
                        //status_icon: "icon_field_status",
                        //status_icon_color: CameraFov.floatValue != 0 ? XHud_Dashboard.Theme_Primary : Color.red,
                        prop: CameraFov,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    }
                    #endregion

                    if (XGUI.ChangedCheck_End())
                    {
                        SetHighFrequencyMode();
                        xHud_EditorUpdate_CameraArgs();
                    }

                    #region 场景相机
                    XGUI.ChangedCheck_Start();
                    XGUI.layout_property_field(
                        title: "场景相机",
                        title_size: XGUIFontSize.M,
                        //title_color: Color.white,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 100,
                        status_icon: "icon_field_status",
                        status_icon_color: SceneCamera.objectReferenceValue != null ? XHud_Dashboard.Theme_Primary : Color.red,
                        prop: SceneCamera,
                        prop_margin: new RectOffset(0, 0, 10, 10));
                    if (XGUI.ChangedCheck_End())
                    {
                        if (!Application.isPlaying)
                        {
                            Camera cam = (Camera)SceneCamera.objectReferenceValue;

                            EditorApplication.delayCall += () =>
                            {
                                XGUI.dialog(
                                    type: XGUIDialogType.警告,
                                    windowtitle: "XHud - 管理器消息",
                                    title: "指定场景相机",
                                    msg: $"确认要将 {cam.name} 相机指定为XHud的主场景相机吗？ 如果不指定场景主相机则您将无法正常使用XHud！",
                                    ok: "指定",
                                    cancel: "暂不",
                                    PrimaryIndex: 0,
                                    usemodal: true,
                                    themecolor: XHud_Dashboard.Theme_Primary,
                                    on_selected: (d) =>
                                    {
                                        if (d == "指定")
                                        {
                                            if (cam != null && cam.GetUniversalAdditionalCameraData().renderType == CameraRenderType.Base)
                                            {
                                                BaseScript.hm_SceneCam_CheckStack((Camera)SceneCamera.objectReferenceValue);
                                            }
                                            else
                                            {
                                                SceneCamera.objectReferenceValue = null;
                                            }

                                            SceneCamera.serializedObject.ApplyModifiedProperties();

                                            // 获取 "XHUD" 图层的 LayerMask
                                            int xhudMask = LayerMask.GetMask("XHud");
                                            // 取消勾选 "XHUD" 图层
                                            cam.cullingMask &= ~xhudMask;

                                            EditorApplication.delayCall += () =>
                                            {
                                                XGUI.dialog(
                                                    type: XGUIDialogType.警告,
                                                    windowtitle: "XHud - 管理器消息",
                                                    title: "指定场景相机",
                                                    msg: "已取消勾选主场景相机的 CullingMask 中的 'XHud' 图层！",
                                                    ok: "明白 ",
                                                    PrimaryIndex: 0,
                                                    usemodal: true,
                                                    themecolor: XHud_Dashboard.Theme_Primary);
                                            };
                                        }
                                        else
                                        {
                                            SceneCamera.objectReferenceValue = null;
                                            SceneCamera.serializedObject.ApplyModifiedProperties();
                                        }
                                        return;
                                    });
                            };
                        }
                    }

                    if (SceneCamera.objectReferenceValue == null)
                        XGUI.layout_helpbox(
                            state: XGUIHelboxState.错误,
                            title_text: "请指定场景主相机！",
                            title_size: XGUIFontSize.M,
                            title_style: FontStyle.Normal,
                            title_color: Color.white * 0.75f);
                    #endregion
                }

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                #endregion

                #region 比例图例 
                BaseScript.fold_ratiorefer = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIFilled.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.Theme_Group,
                    title: "比例图例 (单位 cm)",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.Theme_Primary,
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: BaseScript.fold_ratiorefer);

                if (!BaseScript.fold_ratiorefer)
                {
                    #region 使用图例比例参考
                    DrawToggle("使用图例比例参考", UseRatioReference, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, (b) =>
                    {
                        UseRatioReference.boolValue = b;
                        UseRatioReference.serializedObject.ApplyModifiedProperties();

                        if (UseRatioReference.boolValue)
                        {
                            if (BaseScript.Res_Part.sprite == null && BaseScript.Res_Full.sprite == null)
                            {
                                UseRatioReference.boolValue = false;
                                UseRatioReference.serializedObject.ApplyModifiedProperties();
                                return;
                            }

                            //如果已经存在就不用创建
                            if (sp_Reference_Image.objectReferenceValue != null)
                                return;

                            Sprite spr = BaseScript.Res_Part.sprite;
                            if (!RatioReferenceIsPart.boolValue)
                                spr = BaseScript.Res_Full.sprite;

                            xHud_RatioReference_Create(spr);
                        }
                        else
                        {
                            xHud_RatioReference_Destroy();
                        }
                    });
                    #endregion

                    #region 使用局部比例
                    DrawToggle("使用局部模式", RatioReferenceIsPart, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, (b) =>
                    {
                        xHud_RatioReference_Switch();
                        xHud_RatioReference_Update();
                    });
                    #endregion

                    #region 物理屏幕可视尺寸
                    XGUI.layout_property_field(
                        title: "物理屏幕可视尺寸",
                        title_size: XGUIFontSize.M,
                        //title_color: Color.white,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 200,
                        //status_icon: "icon_field_status",
                        //status_icon_color: CameraOrthographicSize.floatValue != 0 ? XHud_Dashboard.Theme_Primary : Color.red,
                        prop: sp_PhysicsScreenSize,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion

                    XGUI.layout_space(5);

                    XGUI.ChangedCheck_Start();

                    #region 图例着色
                    XGUI.layout_colorfield(
                        prop: sp_Reference_Image_Color,
                        title: "图例着色",
                        title_width: 80,
                        //title_color: Color.white,
                        //icon: "icon_field_status",
                        //icon_color: Color.red,
                        state_title: "Hex：",
                        state_value: $"#{XGUI_Utilitys.Color_To_HexString(sp_Reference_Image_Color.colorValue)}",
                        state_value_color: sp_Reference_Image_Color.colorValue);
                    #endregion

                    XGUI.layout_seperator(
                        thickness: 1,
                        color: XHud_Dashboard.Theme_SeperateLine,
                        margin: new RectOffset(15, 15, 5, 15));

                    #region 参考物尺寸 - 整体
                    XGUI.layout_property_field(
                        title: "参考物尺寸 - 整体",
                        title_size: XGUIFontSize.M,
                        //title_color: Color.white,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 140,
                        //status_icon: "icon_field_status",
                        //status_icon_color: CameraOrthographicSize.floatValue != 0 ? XHud_Dashboard.Theme_Primary : Color.red,
                        prop: sp_Res_Full.FindPropertyRelative("size"),
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    XGUI.layout_property_field(
                        title: "参考物图例 - 整体",
                        title_size: XGUIFontSize.M,
                        //title_color: Color.white,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 140,
                        //status_icon: "icon_field_status",
                        //status_icon_color: CameraOrthographicSize.floatValue != 0 ? XHud_Dashboard.Theme_Primary : Color.red,
                        prop: sp_Res_Full.FindPropertyRelative("sprite"),
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion

                    XGUI.layout_seperator(
                        thickness: 1,
                        color: XHud_Dashboard.Theme_SeperateLine,
                        margin: new RectOffset(15, 15, 15, 10));

                    #region 参考物尺寸 - 局部
                    XGUI.layout_property_field(
                        title: "参考物尺寸 - 局部",
                        title_size: XGUIFontSize.M,
                        //title_color: Color.white,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 140,
                        //status_icon: "icon_field_status",
                        //status_icon_color: CameraOrthographicSize.floatValue != 0 ? XHud_Dashboard.Theme_Primary : Color.red,
                        prop: sp_Res_Part.FindPropertyRelative("size"),
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    XGUI.layout_property_field(
                        title: "参考物图例 - 局部",
                        title_size: XGUIFontSize.M,
                        //title_color: Color.white,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 140,
                        //status_icon: "icon_field_status",
                        //status_icon_color: CameraOrthographicSize.floatValue != 0 ? XHud_Dashboard.Theme_Primary : Color.red,
                        prop: sp_Res_Part.FindPropertyRelative("sprite"),
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion

                    if (XGUI.ChangedCheck_End())
                    {
                        xHud_RatioReference_Update();
                        xHud_RatioReference_Switch();
                    }

                    XGUI.layout_seperator(
                        thickness: 1,
                        color: XHud_Dashboard.Theme_SeperateLine,
                        margin: new RectOffset(15, 15, 15, 10));

                    #region 比例参考图形
                    XGUI.layout_property_field(
                        title: "比例参考图形 - 尺寸",
                        title_size: XGUIFontSize.M,
                        //title_color: Color.white,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 140,
                        prop: sp_ReferShape_RatioSize,
                        prop_margin: new RectOffset(0, 0, 10, 10));

                    XGUI.layout_property_field(
                        title: "比例参考图形 - 公差",
                        title_size: XGUIFontSize.M,
                        //title_color: Color.white,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 140,
                        prop: sp_ReferShape_RatioTolerance,
                        prop_margin: new RectOffset(0, 0, 10, 10));
                    #endregion

                    if (XGUI.layout_button(
                        text: "创建比例参考图形",
                        tooltip: "",
                        bg_fill: XGUIFilled.实体,
                        bg_color: XGUIColor.亮白,
                        bg_color_gui: XHud_Dashboard.Theme_Primary,
                        button_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(XHud_Dashboard.Theme_Primary) ? Color.black : Color.white,
                        press_fill: XGUIFilled.实体,
                        press_color: XGUIColor.深空灰,
                        press_text_color: Color.white,
                        font_size: XGUIFontSize.M,
                        anchor: TextAnchor.MiddleCenter,
                        margin: new RectOffset(0, 0, 0, 10),
                        padding: new RectOffset(0, 0, 0, 0),
                        button_text_font: XGUI.GetFont("xg-medium")))
                    {
                        xHud_CreateReferenceShape(sp_ReferShape_RatioSize.vector2Value.x, sp_ReferShape_RatioSize.vector2Value.y, sp_ReferShape_RatioTolerance.vector2Value.x, sp_ReferShape_RatioTolerance.vector2Value.y);
                        return;
                    }
                }

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                #endregion

                #region 构图参考 
                if (UseCompGuide.boolValue)
                {
                    BaseScript.fold_compguid = XGUI.layout_group_start(
                        type: XGUIContainerType.Vertical,
                        bg_fill: XGUIFilled.缺口纯色边框,
                        bg_color: XGUIColor.亮白,
                        bg_color_gui: XHud_Dashboard.Theme_Group,
                        title: "构图参考",
                        title_size: XGUIFontSize.M,
                        title_text_color: XHud_Dashboard.Theme_Primary,
                        title_clipping: TextClipping.Clip,
                        padding: new RectOffset(10, 10, 15, 15),
                        foldout: BaseScript.fold_compguid);

                    if (!BaseScript.fold_compguid)
                    {
                        #region 构图辅助颜色 - 线
                        XGUI.layout_colorfield(
                            prop: GuideColor,
                            title: "辅助颜色 - 线",
                            title_width: 140,
                            //title_color: Color.white,
                            //icon: "icon_field_status",
                            //icon_color: Color.red,
                            state_title: "Hex：",
                            state_value: $"#{XGUI_Utilitys.Color_To_HexString(GuideColor.colorValue)}",
                            state_value_color: GuideColor.colorValue);
                        #endregion

                        #region 构图辅助颜色 - 点
                        XGUI.layout_colorfield(
                            prop: GuidePointColor,
                            title: "辅助颜色 - 点",
                            title_width: 140,
                            //title_color: Color.white,
                            //icon: "icon_field_status",
                            //icon_color: Color.red,
                            state_title: "Hex：",
                            state_value: $"#{XGUI_Utilitys.Color_To_HexString(GuidePointColor.colorValue)}",
                            state_value_color: GuidePointColor.colorValue);
                        #endregion

                        XGUI.layout_seperator(
                            thickness: 1,
                            color: XHud_Dashboard.Theme_SeperateLine,
                            margin: new RectOffset(15, 15, 5, 15));

                        #region 构图模式       
                        if (currentWidth > 260)
                        {
                            XGUI.layout_group_start(
                                type: XGUIContainerType.Horizontal,
                                absolute_margin: true,
                                absolute_padding: true,
                                margin: new RectOffset(0, 0, 0, 0),
                                padding: new RectOffset(0, 0, 0, 0));
                        }

                        XGUI.layout_string_popup(
                           title: "构图模式",
                           title_width: 60,
                           title_size: XGUIFontSize.M,
                           title_anchor: TextAnchor.MiddleLeft,
                           prop: CompGuideMode,
                           options: compguid_modes,
                           opt_text_size: XGUIFontSize.M,
                           opt_text_color: Color.black,
                           opt_text_padding: new RectOffset(10, 10, 0, 0),
                           opt_anchor: TextAnchor.MiddleCenter,
                           opt_font_style: FontStyle.Normal,
                           opt_bg_fill: XGUIFilled.实体,
                           opt_bg_color: XGUIColor.亮白,
                           opt_bg_color_gui: XHud_Dashboard.Theme_Primary,
                           icon_arrow_color: Color.black,
                           margin: new RectOffset(0, 0, 5, 5),
                           padding: new RectOffset(5, 5, 0, 0),
                           title_margin: new RectOffset(0, 0, 0, 0),
                           act_on_changed: (value) =>
                           {

                           });

                        if (currentWidth > 260)
                        {
                            if (XGUI.layout_button(
                                tooltip: "查看",
                                tex_release: GuideHelp_Released,
                                tex_press: GuideHelp_Press,
                                tex_gui_color: Color.white,
                                border: new RectOffset(0, 0, 0, 0),
                                margin: new RectOffset(5, 0, 1, 0),
                                width: 15,
                                height: 15))
                            {
                                xHud_OpenGuideHelpPanel(CompGuideMode.stringValue);
                            }
                        }

                        if (currentWidth > 260)
                            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
                        #endregion

                        XGUI.layout_seperator(
                            thickness: 1,
                            color: XHud_Dashboard.Theme_SeperateLine,
                            margin: new RectOffset(15, 15, 15, 10));

                        switch (CompGuideMode.stringValue)
                        {
                            case "水平对称":
                                XGUI.layout_space(5);
                                string[] dir_lr = new string[3] { "自定义对称分割", "靠上黄金比例", "靠下黄金比例" };
                                string mode_lr = XGUI.layout_string_popup(
                                    title: "对称方式",
                                    title_width: 60,
                                    title_size: XGUIFontSize.M,
                                    title_anchor: TextAnchor.MiddleLeft,
                                    prop: GuideParam_Mirror_UD_GoldenMode,
                                    options: dir_lr,
                                    opt_text_size: XGUIFontSize.M,
                                    opt_text_color: Color.black,
                                    opt_text_padding: new RectOffset(10, 10, 0, 0),
                                    opt_anchor: TextAnchor.MiddleCenter,
                                    opt_font_style: FontStyle.Normal,
                                    opt_bg_fill: XGUIFilled.实体,
                                    opt_bg_color: XGUIColor.亮白,
                                    opt_bg_color_gui: XHud_Dashboard.Theme_Primary,
                                    icon_arrow_color: Color.black,
                                    margin: new RectOffset(0, 0, 5, 5),
                                    padding: new RectOffset(5, 5, 0, 0),
                                    title_margin: new RectOffset(0, 0, 0, 0),
                                    act_on_changed: (value) =>
                                    {

                                    });

                                if (mode_lr == "自定义对称分割")
                                {
                                    XGUI.layout_space(5);
                                    XGUI.layout_property_field(
                                        title: "对称偏移校正",
                                        title_size: XGUIFontSize.M,
                                        //title_color: Color.white,
                                        title_hover_color: XHud_Dashboard.Theme_Primary,
                                        title_width: 90,
                                        //status_icon: "icon_field_status",
                                        prop: GuideParam_Mirror_UD_Offset,
                                        prop_margin: new RectOffset(0, 0, 5, 0));
                                }

                                XGUI.layout_space(5);

                                XGUI.layout_property_field(
                                    title: "中心点尺寸",
                                    title_size: XGUIFontSize.M,
                                    //title_color: Color.white,
                                    title_hover_color: XHud_Dashboard.Theme_Primary,
                                    title_width: 90,
                                    //status_icon: "icon_field_status",
                                    prop: GuideParam_CenterPointSize,
                                    prop_margin: new RectOffset(0, 0, 5, 0));

                                XGUI.layout_space(15);

                                #region 复位按钮
                                if (XGUI.layout_button(
                                    text: "复位校正",
                                    tooltip: "",
                                    bg_fill: XGUIFilled.实体,
                                    bg_color: XGUIColor.深空灰,
                                    bg_color_gui: Color.white,
                                    button_text_color: color_important,
                                    press_fill: XGUIFilled.实体,
                                    press_color: XGUIColor.深空灰,
                                    press_text_color: Color.white,
                                    font_size: XGUIFontSize.M,
                                    anchor: TextAnchor.MiddleCenter,
                                    margin: new RectOffset(0, 0, 0, 0),
                                    padding: new RectOffset(0, 0, 0, 0),
                                    height: 25,
                                    button_text_font: XGUI.GetFont("xg-medium")))
                                {
                                    GuideParam_Mirror_UD_GoldenMode.stringValue = "自定义对称分割";
                                    GuideParam_Mirror_UD_GoldenMode.serializedObject.ApplyModifiedProperties();

                                    GuideParam_Mirror_UD_Offset.floatValue = 0;
                                    GuideParam_Mirror_UD_Offset.serializedObject.ApplyModifiedProperties();

                                    GuideParam_CenterPointSize.floatValue = 1;
                                    GuideParam_CenterPointSize.serializedObject.ApplyModifiedProperties();
                                }
                                #endregion
                                break;
                            case "垂直对称":
                                XGUI.layout_space(5);

                                string[] dir_ud = new string[3] { "自定义对称分割", "靠左黄金比例", "靠右黄金比例" };

                                string mode_ud = XGUI.layout_string_popup(
                                    title: "对称方式",
                                    title_width: 60,
                                    title_size: XGUIFontSize.M,
                                    title_anchor: TextAnchor.MiddleLeft,
                                    prop: GuideParam_Mirror_LR_GoldenMode,
                                    options: dir_ud,
                                    opt_text_size: XGUIFontSize.M,
                                    opt_text_color: Color.black,
                                    opt_text_padding: new RectOffset(10, 10, 0, 0),
                                    opt_anchor: TextAnchor.MiddleCenter,
                                    opt_font_style: FontStyle.Normal,
                                    opt_bg_fill: XGUIFilled.实体,
                                    opt_bg_color: XGUIColor.亮白,
                                    opt_bg_color_gui: XHud_Dashboard.Theme_Primary,
                                    icon_arrow_color: Color.black,
                                    margin: new RectOffset(0, 0, 5, 5),
                                    padding: new RectOffset(5, 5, 0, 0),
                                    title_margin: new RectOffset(0, 0, 0, 0),
                                    act_on_changed: (value) =>
                                    {

                                    });

                                if (mode_ud == "自定义对称分割")
                                {
                                    XGUI.layout_space(5);

                                    XGUI.layout_property_field(
                                        title: "对称偏移校正",
                                        title_size: XGUIFontSize.M,
                                        //title_color: Color.white,
                                        title_hover_color: XHud_Dashboard.Theme_Primary,
                                        title_width: 90,
                                        //status_icon: "icon_field_status",
                                        prop: GuideParam_Mirror_LR_Offset,
                                        prop_margin: new RectOffset(0, 0, 5, 0));
                                }

                                XGUI.layout_space(5);

                                XGUI.layout_property_field(
                                    title: "中心点尺寸",
                                    title_size: XGUIFontSize.M,
                                    //title_color: Color.white,
                                    title_hover_color: XHud_Dashboard.Theme_Primary,
                                    title_width: 90,
                                    //status_icon: "icon_field_status",
                                    prop: GuideParam_CenterPointSize,
                                    prop_margin: new RectOffset(0, 0, 5, 0));

                                XGUI.layout_space(15);

                                #region 复位按钮
                                if (XGUI.layout_button(
                                    text: "复位校正",
                                    tooltip: "",
                                    bg_fill: XGUIFilled.实体,
                                    bg_color: XGUIColor.深空灰,
                                    bg_color_gui: Color.white,
                                    button_text_color: color_important,
                                    press_fill: XGUIFilled.实体,
                                    press_color: XGUIColor.深空灰,
                                    press_text_color: Color.white,
                                    font_size: XGUIFontSize.M,
                                    anchor: TextAnchor.MiddleCenter,
                                    margin: new RectOffset(0, 0, 0, 0),
                                    padding: new RectOffset(0, 0, 0, 0),
                                    height: 25,
                                    button_text_font: XGUI.GetFont("xg-medium")))
                                {
                                    GuideParam_Mirror_LR_GoldenMode.stringValue = "自定义对称分割";
                                    GuideParam_Mirror_LR_GoldenMode.serializedObject.ApplyModifiedProperties();

                                    GuideParam_Mirror_LR_Offset.floatValue = 0;
                                    GuideParam_Mirror_LR_Offset.serializedObject.ApplyModifiedProperties();

                                    GuideParam_CenterPointSize.floatValue = 1;
                                    GuideParam_CenterPointSize.serializedObject.ApplyModifiedProperties();
                                }
                                #endregion
                                break;
                            case "黄金螺旋":
                                XGUI.layout_space(5);

                                string[] dir_fb = new string[4] { "右上", "左上", "右下", "左下" };

                                string mode_fb = XGUI.layout_string_popup(
                                   title: "对称方式",
                                   title_width: 60,
                                   title_size: XGUIFontSize.M,
                                   title_anchor: TextAnchor.MiddleLeft,
                                   prop: GuideParam_Fibonacci_Mode,
                                   options: dir_fb,
                                   opt_text_size: XGUIFontSize.M,
                                   opt_text_color: Color.black,
                                   opt_text_padding: new RectOffset(10, 10, 0, 0),
                                   opt_anchor: TextAnchor.MiddleCenter,
                                   opt_font_style: FontStyle.Normal,
                                   opt_bg_fill: XGUIFilled.实体,
                                   opt_bg_color: XGUIColor.亮白,
                                   opt_bg_color_gui: XHud_Dashboard.Theme_Primary,
                                   icon_arrow_color: Color.black,
                                   margin: new RectOffset(0, 0, 5, 5),
                                   padding: new RectOffset(5, 5, 0, 0),
                                   title_margin: new RectOffset(0, 0, 0, 0),
                                   act_on_changed: (value) =>
                                   {

                                   });

                                XGUI.layout_space(15);

                                #region 复位按钮
                                if (XGUI.layout_button(
                                    text: "复位校正",
                                    tooltip: "",
                                    bg_fill: XGUIFilled.实体,
                                    bg_color: XGUIColor.深空灰,
                                    bg_color_gui: Color.white,
                                    button_text_color: color_important,
                                    press_fill: XGUIFilled.实体,
                                    press_color: XGUIColor.深空灰,
                                    press_text_color: Color.white,
                                    font_size: XGUIFontSize.M,
                                    anchor: TextAnchor.MiddleCenter,
                                    margin: new RectOffset(0, 0, 0, 0),
                                    padding: new RectOffset(0, 0, 0, 0),
                                    height: 25,
                                    button_text_font: XGUI.GetFont("xg-medium")))
                                {
                                    GuideParam_Fibonacci_Mode.stringValue = "右上";
                                    GuideParam_Fibonacci_Mode.serializedObject.ApplyModifiedProperties();
                                }
                                #endregion
                                break;
                            case "对角线":
                                XGUI.layout_space(5);

                                XGUI.layout_property_field(
                                    title: "水平偏移",
                                    title_size: XGUIFontSize.M,
                                    //title_color: Color.white,
                                    title_hover_color: XHud_Dashboard.Theme_Primary,
                                    title_width: 90,
                                    //status_icon: "icon_field_status",
                                    prop: GuideParam_CornerLookat_Offset_H,
                                    prop_margin: new RectOffset(0, 0, 5, 0));

                                XGUI.layout_space(5);

                                XGUI.layout_property_field(
                                    title: "垂直偏移",
                                    title_size: XGUIFontSize.M,
                                    //title_color: Color.white,
                                    title_hover_color: XHud_Dashboard.Theme_Primary,
                                    title_width: 90,
                                    //status_icon: "icon_field_status",
                                    prop: GuideParam_CornerLookat_Offset_V,
                                    prop_margin: new RectOffset(0, 0, 5, 0));

                                XGUI.layout_space(5);

                                XGUI.layout_property_field(
                                    title: "中心点尺寸",
                                    title_size: XGUIFontSize.M,
                                    //title_color: Color.white,
                                    title_hover_color: XHud_Dashboard.Theme_Primary,
                                    title_width: 90,
                                    //status_icon: "icon_field_status",
                                    prop: GuideParam_CenterPointSize,
                                    prop_margin: new RectOffset(0, 0, 5, 0));

                                XGUI.layout_space(15);

                                #region 复位按钮
                                if (XGUI.layout_button(
                                    text: "复位校正",
                                    tooltip: "",
                                    bg_fill: XGUIFilled.实体,
                                    bg_color: XGUIColor.深空灰,
                                    bg_color_gui: Color.white,
                                    button_text_color: color_important,
                                    press_fill: XGUIFilled.实体,
                                    press_color: XGUIColor.深空灰,
                                    press_text_color: Color.white,
                                    font_size: XGUIFontSize.M,
                                    anchor: TextAnchor.MiddleCenter,
                                    margin: new RectOffset(0, 0, 0, 0),
                                    padding: new RectOffset(0, 0, 0, 0),
                                    height: 25,
                                    button_text_font: XGUI.GetFont("xg-medium")))
                                {
                                    GuideParam_CornerLookat_Offset_H.floatValue = 0;
                                    GuideParam_CornerLookat_Offset_H.serializedObject.ApplyModifiedProperties();

                                    GuideParam_CornerLookat_Offset_V.floatValue = 0;
                                    GuideParam_CornerLookat_Offset_V.serializedObject.ApplyModifiedProperties();

                                    GuideParam_CenterPointSize.floatValue = 1;
                                    GuideParam_CenterPointSize.serializedObject.ApplyModifiedProperties();
                                }
                                #endregion
                                break;
                            case "三分线":
                                XGUI.layout_space(5);

                                XGUI.layout_property_field(
                                    title: "水平伸缩",
                                    title_size: XGUIFontSize.M,
                                    //title_color: Color.white,
                                    title_hover_color: XHud_Dashboard.Theme_Primary,
                                    title_width: 90,
                                    //status_icon: "icon_field_status",
                                    prop: GuideParam_Three_Offset_H,
                                    prop_margin: new RectOffset(0, 0, 5, 0));

                                XGUI.layout_space(5);

                                XGUI.layout_property_field(
                                    title: "垂直伸缩",
                                    title_size: XGUIFontSize.M,
                                    //title_color: Color.white,
                                    title_hover_color: XHud_Dashboard.Theme_Primary,
                                    title_width: 90,
                                    //status_icon: "icon_field_status",
                                    prop: GuideParam_Three_Offset_V,
                                    prop_margin: new RectOffset(0, 0, 5, 0));

                                XGUI.layout_space(5);

                                XGUI.layout_property_field(
                                    title: "整体伸缩",
                                    title_size: XGUIFontSize.M,
                                    //title_color: Color.white,
                                    title_hover_color: XHud_Dashboard.Theme_Primary,
                                    title_width: 90,
                                    //status_icon: "icon_field_status",
                                    prop: GuideParam_Three_Offset_Coverage,
                                    prop_margin: new RectOffset(0, 0, 5, 0));

                                XGUI.layout_space(5);

                                #region 平均等分
                                if (XGUI.layout_button(
                                    text: "平均等分",
                                    tooltip: "",
                                    bg_fill: XGUIFilled.实体,
                                    bg_color: XGUIColor.深空灰,
                                    bg_color_gui: Color.white,
                                    button_text_color: XHud_Dashboard.Theme_Primary,
                                    press_fill: XGUIFilled.实体,
                                    press_color: XGUIColor.深空灰,
                                    press_text_color: Color.white,
                                    font_size: XGUIFontSize.M,
                                    anchor: TextAnchor.MiddleCenter,
                                    margin: new RectOffset(0, 0, 0, 0),
                                    padding: new RectOffset(0, 0, 0, 0),
                                    height: 25,
                                    button_text_font: XGUI.GetFont("xg-medium")))
                                {
                                    GuideParam_Three_Offset_Coverage.floatValue = 1;
                                    GuideParam_Three_Offset_Coverage.serializedObject.ApplyModifiedProperties();

                                    float seg_x = ScreenRes.vector2Value.x / 3;
                                    GuideParam_Three_Offset_H.floatValue = seg_x / ScreenRes.vector2Value.x * 2f;
                                    GuideParam_Three_Offset_H.serializedObject.ApplyModifiedProperties();

                                    float seg_y = ScreenRes.vector2Value.y / 3;
                                    GuideParam_Three_Offset_V.floatValue = seg_y / ScreenRes.vector2Value.y * 2f;
                                    GuideParam_Three_Offset_V.serializedObject.ApplyModifiedProperties();
                                }
                                #endregion

                                XGUI.layout_space(15);

                                #region 复位按钮
                                if (XGUI.layout_button(
                                    text: "复位校正",
                                    tooltip: "",
                                    bg_fill: XGUIFilled.实体,
                                    bg_color: XGUIColor.深空灰,
                                    bg_color_gui: Color.white,
                                    button_text_color: color_important,
                                    press_fill: XGUIFilled.实体,
                                    press_color: XGUIColor.深空灰,
                                    press_text_color: Color.white,
                                    font_size: XGUIFontSize.M,
                                    anchor: TextAnchor.MiddleCenter,
                                    margin: new RectOffset(0, 0, 0, 0),
                                    padding: new RectOffset(0, 0, 0, 0),
                                    height: 25,
                                    button_text_font: XGUI.GetFont("xg-medium")))
                                {
                                    GuideParam_Three_Offset_H.floatValue = 1;
                                    GuideParam_Three_Offset_H.serializedObject.ApplyModifiedProperties();

                                    GuideParam_Three_Offset_V.floatValue = 1;
                                    GuideParam_Three_Offset_V.serializedObject.ApplyModifiedProperties();

                                    GuideParam_Three_Offset_Coverage.floatValue = 0.5f;
                                    GuideParam_Three_Offset_Coverage.serializedObject.ApplyModifiedProperties();

                                    GuideParam_CenterPointSize.floatValue = 1;
                                    GuideParam_CenterPointSize.serializedObject.ApplyModifiedProperties();
                                }
                                #endregion
                                break;
                            case "引导线":
                                XGUI.layout_space(5);

                                XGUI.layout_property_field(
                                    title: "基准水平位移",
                                    title_size: XGUIFontSize.M,
                                    title_hover_color: XHud_Dashboard.Theme_Primary,
                                    title_width: 90,
                                    prop: GuideParam_GuideLine_BaseOffset,
                                    prop_margin: new RectOffset(0, 0, 5, 0));

                                XGUI.layout_space(5);

                                XGUI.layout_property_field(
                                    title: "基准水平高度",
                                    title_size: XGUIFontSize.M,
                                    title_hover_color: XHud_Dashboard.Theme_Primary,
                                    title_width: 90,
                                    prop: GuideParam_GuideLine_BaseHeight,
                                    prop_margin: new RectOffset(0, 0, 5, 0));

                                XGUI.layout_space(5);

                                XGUI.layout_property_field(
                                    title: "近景高度",
                                    title_size: XGUIFontSize.M,
                                    title_hover_color: XHud_Dashboard.Theme_Primary,
                                    title_width: 90,
                                    prop: GuideParam_GuideLine_Offset_NearHeight,
                                    prop_margin: new RectOffset(0, 0, 5, 0));

                                XGUI.layout_space(5);

                                XGUI.layout_property_field(
                                    title: "近景伸缩",
                                    title_size: XGUIFontSize.M,
                                    title_hover_color: XHud_Dashboard.Theme_Primary,
                                    title_width: 90,
                                    prop: GuideParam_GuideLine_Offset_Near,
                                    prop_margin: new RectOffset(0, 0, 5, 0));

                                XGUI.layout_space(5);

                                XGUI.layout_property_field(
                                    title: "远景伸缩",
                                    title_size: XGUIFontSize.M,
                                    title_hover_color: XHud_Dashboard.Theme_Primary,
                                    title_width: 90,
                                    prop: GuideParam_GuideLine_Offset_Far,
                                    prop_margin: new RectOffset(0, 0, 5, 0));

                                XGUI.layout_space(5);

                                XGUI.layout_property_field(
                                    title: "中心点尺寸",
                                    title_size: XGUIFontSize.M,
                                    title_hover_color: XHud_Dashboard.Theme_Primary,
                                    title_width: 90,
                                    prop: GuideParam_CenterPointSize,
                                    prop_margin: new RectOffset(0, 0, 5, 0));

                                XGUI.layout_space(15);

                                #region 复位按钮
                                if (XGUI.layout_button(
                                    text: "复位校正",
                                    tooltip: "",
                                    bg_fill: XGUIFilled.实体,
                                    bg_color: XGUIColor.深空灰,
                                    bg_color_gui: Color.white,
                                    button_text_color: color_important,
                                    press_fill: XGUIFilled.实体,
                                    press_color: XGUIColor.深空灰,
                                    press_text_color: Color.white,
                                    font_size: XGUIFontSize.M,
                                    anchor: TextAnchor.MiddleCenter,
                                    margin: new RectOffset(0, 0, 0, 0),
                                    padding: new RectOffset(0, 0, 0, 0),
                                    height: 25,
                                    button_text_font: XGUI.GetFont("xg-medium")))
                                {
                                    GuideParam_GuideLine_BaseOffset.floatValue = 0;
                                    GuideParam_GuideLine_BaseOffset.serializedObject.ApplyModifiedProperties();

                                    GuideParam_GuideLine_BaseHeight.floatValue = 0.5f;
                                    GuideParam_GuideLine_BaseHeight.serializedObject.ApplyModifiedProperties();

                                    GuideParam_GuideLine_Offset_Far.floatValue = 0;
                                    GuideParam_GuideLine_Offset_Far.serializedObject.ApplyModifiedProperties();

                                    GuideParam_GuideLine_Offset_Near.floatValue = 0;
                                    GuideParam_GuideLine_Offset_Near.serializedObject.ApplyModifiedProperties();

                                    GuideParam_GuideLine_Offset_NearHeight.floatValue = 0;
                                    GuideParam_GuideLine_Offset_NearHeight.serializedObject.ApplyModifiedProperties();

                                    GuideParam_CenterPointSize.floatValue = 1;
                                    GuideParam_CenterPointSize.serializedObject.ApplyModifiedProperties();
                                }
                                #endregion
                                break;
                            case "三角":
                                XGUI.layout_space(5);

                                XGUI.layout_property_field(
                                    title: "基准顶部位移",
                                    title_size: XGUIFontSize.M,
                                    title_hover_color: XHud_Dashboard.Theme_Primary,
                                    title_width: 90,
                                    prop: GuideParam_Triangle_TopOffset,
                                    prop_margin: new RectOffset(0, 0, 5, 0));

                                XGUI.layout_space(5);

                                XGUI.layout_property_field(
                                    title: "基准顶部高度",
                                    title_size: XGUIFontSize.M,
                                    title_hover_color: XHud_Dashboard.Theme_Primary,
                                    title_width: 90,
                                    prop: GuideParam_Triangle_BaseHeight,
                                    prop_margin: new RectOffset(0, 0, 5, 0));

                                XGUI.layout_space(5);

                                XGUI.layout_property_field(
                                    title: "基准底部高度",
                                    title_size: XGUIFontSize.M,
                                    title_hover_color: XHud_Dashboard.Theme_Primary,
                                    title_width: 90,
                                    prop: GuideParam_Triangle_BottomHeight,
                                    prop_margin: new RectOffset(0, 0, 5, 0));

                                XGUI.layout_space(5);

                                XGUI.layout_property_field(
                                    title: "左角点偏移",
                                    title_size: XGUIFontSize.M,
                                    title_hover_color: XHud_Dashboard.Theme_Primary,
                                    title_width: 90,
                                    prop: GuideParam_Triangle_Offset_Left,
                                    prop_margin: new RectOffset(0, 0, 5, 0));

                                XGUI.layout_space(5);

                                XGUI.layout_property_field(
                                    title: "右角点偏移",
                                    title_size: XGUIFontSize.M,
                                    title_hover_color: XHud_Dashboard.Theme_Primary,
                                    title_width: 90,
                                    prop: GuideParam_Triangle_Offset_Right,
                                    prop_margin: new RectOffset(0, 0, 5, 0));

                                XGUI.layout_space(5);

                                XGUI.layout_property_field(
                                    title: "中心点尺寸",
                                    title_size: XGUIFontSize.M,
                                    title_hover_color: XHud_Dashboard.Theme_Primary,
                                    title_width: 90,
                                    prop: GuideParam_CenterPointSize,
                                    prop_margin: new RectOffset(0, 0, 5, 0));

                                XGUI.layout_space(15);

                                #region 复位按钮
                                if (XGUI.layout_button(
                                    text: "复位校正",
                                    tooltip: "",
                                    bg_fill: XGUIFilled.实体,
                                    bg_color: XGUIColor.深空灰,
                                    bg_color_gui: Color.white,
                                    button_text_color: color_important,
                                    press_fill: XGUIFilled.实体,
                                    press_color: XGUIColor.深空灰,
                                    press_text_color: Color.white,
                                    font_size: XGUIFontSize.M,
                                    anchor: TextAnchor.MiddleCenter,
                                    margin: new RectOffset(0, 0, 0, 0),
                                    padding: new RectOffset(0, 0, 0, 0),
                                    height: 25,
                                    button_text_font: XGUI.GetFont("xg-medium")))
                                {
                                    GuideParam_Triangle_TopOffset.floatValue = 0;
                                    GuideParam_Triangle_TopOffset.serializedObject.ApplyModifiedProperties();

                                    GuideParam_Triangle_BaseHeight.floatValue = 0.8f;
                                    GuideParam_Triangle_BaseHeight.serializedObject.ApplyModifiedProperties();

                                    GuideParam_Triangle_BottomHeight.floatValue = 0.2f;
                                    GuideParam_Triangle_BottomHeight.serializedObject.ApplyModifiedProperties();

                                    GuideParam_Triangle_Offset_Left.floatValue = 0.5f;
                                    GuideParam_Triangle_Offset_Left.serializedObject.ApplyModifiedProperties();

                                    GuideParam_Triangle_Offset_Right.floatValue = 0.5f;
                                    GuideParam_Triangle_Offset_Right.serializedObject.ApplyModifiedProperties();

                                    GuideParam_CenterPointSize.floatValue = 1;
                                    GuideParam_CenterPointSize.serializedObject.ApplyModifiedProperties();
                                }
                                #endregion
                                break;
                            case "工字型":
                                XGUI.layout_space(5);

                                XGUI.layout_property_field(
                                    title: "工字顶部高度",
                                    title_size: XGUIFontSize.M,
                                    title_hover_color: XHud_Dashboard.Theme_Primary,
                                    title_width: 90,
                                    prop: GuideParam_IShape_TopHeight,
                                    prop_margin: new RectOffset(0, 0, 5, 0));

                                XGUI.layout_space(5);

                                XGUI.layout_property_field(
                                    title: "工字底部高度",
                                    title_size: XGUIFontSize.M,
                                    title_hover_color: XHud_Dashboard.Theme_Primary,
                                    title_width: 90,
                                    prop: GuideParam_IShape_BottomHeight,
                                    prop_margin: new RectOffset(0, 0, 5, 0));

                                XGUI.layout_space(5);

                                XGUI.layout_property_field(
                                    title: "工字左偏移",
                                    title_size: XGUIFontSize.M,
                                    title_hover_color: XHud_Dashboard.Theme_Primary,
                                    title_width: 90,
                                    prop: GuideParam_IShape_Offset_Left,
                                    prop_margin: new RectOffset(0, 0, 5, 0));

                                XGUI.layout_space(5);

                                XGUI.layout_property_field(
                                    title: "工字右偏移",
                                    title_size: XGUIFontSize.M,
                                    title_hover_color: XHud_Dashboard.Theme_Primary,
                                    title_width: 90,
                                    prop: GuideParam_IShape_Offset_Right,
                                    prop_margin: new RectOffset(0, 0, 5, 0));

                                XGUI.layout_space(15);

                                #region 复位按钮
                                if (XGUI.layout_button(
                                    text: "复位校正",
                                    tooltip: "",
                                    bg_fill: XGUIFilled.实体,
                                    bg_color: XGUIColor.深空灰,
                                    bg_color_gui: Color.white,
                                    button_text_color: color_important,
                                    press_fill: XGUIFilled.实体,
                                    press_color: XGUIColor.深空灰,
                                    press_text_color: Color.gray,
                                    font_size: XGUIFontSize.M,
                                    anchor: TextAnchor.MiddleCenter,
                                    margin: new RectOffset(0, 0, 0, 0),
                                    padding: new RectOffset(0, 0, 0, 0),
                                    height: 25,
                                    button_text_font: XGUI.GetFont("xg-medium")))
                                {
                                    GuideParam_IShape_TopHeight.floatValue = 0.8f;
                                    GuideParam_IShape_TopHeight.serializedObject.ApplyModifiedProperties();

                                    GuideParam_IShape_BottomHeight.floatValue = 0.2f;
                                    GuideParam_IShape_BottomHeight.serializedObject.ApplyModifiedProperties();

                                    GuideParam_IShape_Offset_Left.floatValue = 0.5f;
                                    GuideParam_IShape_Offset_Left.serializedObject.ApplyModifiedProperties();

                                    GuideParam_IShape_Offset_Right.floatValue = 0.5f;
                                    GuideParam_IShape_Offset_Right.serializedObject.ApplyModifiedProperties();

                                    GuideParam_CenterPointSize.floatValue = 1;
                                    GuideParam_CenterPointSize.serializedObject.ApplyModifiedProperties();
                                }
                                #endregion
                                break;
                        }
                    }
                    XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                }
                #endregion

                #region 资源库 
                BaseScript.fold_reslibs = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIFilled.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.Theme_Group,
                    title: "资源库",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.Theme_Primary,
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: BaseScript.fold_reslibs);

                if (!BaseScript.fold_reslibs)
                {
                    XGUI.layout_property_field(
                        title: "色卡库",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 100,
                        prop: Lib_Color,
                        status_icon: "icon_field_status",
                        status_icon_color: Lib_Color.objectReferenceValue != null ? XHud_Dashboard.Theme_Primary : Color.black,
                        prop_margin: new RectOffset(0, 0, 10, 10));

                    XGUI.layout_property_field(
                        title: "曲线库",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 100,
                        prop: Lib_Curve,
                        status_icon: "icon_field_status",
                        status_icon_color: Lib_Curve.objectReferenceValue != null ? XHud_Dashboard.Theme_Primary : Color.black,
                        prop_margin: new RectOffset(0, 0, 10, 10));

                    XGUI.layout_property_field(
                        title: "音效库",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 100,
                        prop: Lib_Sound,
                        status_icon: "icon_field_status",
                        status_icon_color: Lib_Sound.objectReferenceValue != null ? XHud_Dashboard.Theme_Primary : Color.black,
                        prop_margin: new RectOffset(0, 0, 10, 10));

                    XGUI.layout_property_field(
                        title: "字体库",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 100,
                        prop: Lib_TextStyleLibrary,
                        status_icon: "icon_field_status",
                        status_icon_color: Lib_TextStyleLibrary.objectReferenceValue != null ? XHud_Dashboard.Theme_Primary : Color.black,
                        prop_margin: new RectOffset(0, 0, 10, 10));

                    XGUI.layout_property_field(
                        title: "动效库",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 100,
                        prop: Lib_Motions,
                        status_icon: "icon_field_status",
                        status_icon_color: Lib_Motions.objectReferenceValue != null ? XHud_Dashboard.Theme_Primary : Color.black,
                        prop_margin: new RectOffset(0, 0, 10, 10));

                    XGUI.layout_property_field(
                        title: "转场库",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 100,
                        prop: Lib_Transition,
                        status_icon: "icon_field_status",
                        status_icon_color: Lib_Transition.objectReferenceValue != null ? XHud_Dashboard.Theme_Primary : Color.black,
                        prop_margin: new RectOffset(0, 0, 10, 10));

                    XGUI.layout_space(5);
                }

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                #endregion

                #region 元素库 
                BaseScript.fold_elementlibs = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIFilled.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.Theme_Group,
                    title: "元素库",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.Theme_Primary,
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: BaseScript.fold_elementlibs);

                if (!BaseScript.fold_elementlibs)
                {
                    if (Lib_ElementLibrarys.arraySize <= 0)
                        XGUI.layout_helpbox(
                            state: XGUIHelboxState.错误,
                            title_text: "请至少指定一个元素库资源！",
                            title_size: XGUIFontSize.M,
                            title_style: FontStyle.Normal,
                            title_color: Color.white * 0.75f);

                    List_ElementLib.DoLayoutList();
                }

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                #endregion

                #region 音效池 
                BaseScript.fold_soundslib = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIFilled.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.Theme_Group,
                    title: "音效池",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.Theme_Primary,
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: BaseScript.fold_soundslib);

                if (!BaseScript.fold_soundslib)
                {
                    string msg = $"音效池支持提供<b><color={hexcol}> 回环队列池化 音效 / 音乐 / 声音播放 </color></b>的能力，如果您的项目<b><color={hexcol}> 不涉及UI音效 </color></b>则可忽略该提示！并保持<b><color={hexcol}> 预加载数量为0 </color></b>";

                    XGUI.layout_label(
                        text: msg,
                        bg_fill: XGUIFilled.实体,
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

                    XGUI.layout_property_field(
                     title: "预加载数量",
                     title_size: XGUIFontSize.M,
                     title_hover_color: XHud_Dashboard.Theme_Primary,
                     title_width: 100,
                     prop: SounderPoolCount,
                     //status_icon: "icon_field_status",
                     //status_icon_color: Lib_TextStyleLibrary.objectReferenceValue != null ? XHud_Dashboard.Theme_Primary : Color.black,
                     prop_margin: new RectOffset(0, 0, 10, 10));

                    List_SoundsPool.DoLayoutList();
                }

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                #endregion

                #region 遮罩 / 内容透明度 
                BaseScript.fold_masks = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIFilled.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.Theme_Group,
                    title: "遮罩 / 内容透明度",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.Theme_Primary,
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: BaseScript.fold_masks);

                if (!BaseScript.fold_masks)
                {
                    XGUI.ChangedCheck_Start();

                    XGUI.layout_state_displayer_icon(
                          title: MaskRaycastEnabled.boolValue ? "射线阻挡" : "射线穿透",
                          title_size: XGUIFontSize.M,
                          icon: MaskRaycastEnabled.boolValue ? icon_ray_masked : icon_ray_normal,
                          icon_size: new Vector2(12, 12),
                          icon_offset: new Vector2(0, 0),
                          icon_color: MaskRaycastEnabled.boolValue ? Color.red : XHud_Dashboard.Theme_Primary,
                          padding: new RectOffset(0, 0, 0, 0),
                          margin: new RectOffset(5, 5, 0, 5));

                    #region 透明度
                    XGUI.layout_property_field(
                     title: "透明度",
                     title_size: XGUIFontSize.M,
                     title_hover_color: XHud_Dashboard.Theme_Primary,
                     title_width: 90,
                     prop: MaskAlpha,
                     prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion

                    #region 遮挡阈值
                    XGUI.layout_property_field(
                    title: "遮挡阈值",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: MaskRaycastAlphaThreshold,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion

                    #region 颜色
                    XGUI.layout_colorfield(
                        prop: MaskColor,
                        title: "颜色",
                        title_width: 90,
                        state_title: "Hex：",
                        state_value: $"#{XGUI_Utilitys.Color_To_HexString(MaskColor.colorValue)}",
                        state_value_color: MaskColor.colorValue);
                    #endregion

                    #region 图形
                    XGUI.layout_property_field(
                    title: "图形",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: MaskTexture,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion

                    XGUI.layout_seperator(
                       thickness: 1,
                       color: XHud_Dashboard.Theme_SeperateLine,
                       margin: new RectOffset(15, 15, 15, 15));

                    #region 透明度 - 屏幕
                    XGUI.layout_property_field(
                    title: "透明度-屏幕",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: ContentOpacity_Screen,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion

                    #region 透明度 - 世界
                    XGUI.layout_property_field(
                    title: "透明度-世界",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: ContentOpacity_World,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion

                    if (XGUI.ChangedCheck_End())
                    {
                        SetHighFrequencyMode();
                        xHud_EditorUpdate_Mask();
                        xHud_EditorUpdate_ContentOpacity();
                    }
                }

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                #endregion

                #region 全局
                BaseScript.fold_global = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIFilled.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.Theme_Group,
                    title: "全局",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.Theme_Primary,
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: BaseScript.fold_global);

                if (!BaseScript.fold_global)
                {
                    #region 音效音量
                    XGUI.layout_property_field(
                    title: "音效音量",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: Volume,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion

                    #region 动画速率
                    XGUI.layout_property_field(
                    title: "动画速率",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: DurationMultiply,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion

                    #region 字体尺寸
                    XGUI.ChangedCheck_Start();
                    XGUI.layout_property_field(
                    title: "字体尺寸",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: FontSizeMultiply,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                    if (XGUI.ChangedCheck_End())
                    {
                        BaseScript.hm_ChangeFontGlobalSize(FontSizeMultiply.floatValue);
                    }
                    #endregion

                    #region 光标指针
                    XGUI.layout_property_field(
                    title: "光标指针",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: Hud_MouseCursor,
                    status_icon: "icon_field_status",
                    status_icon_color: Hud_MouseCursor.objectReferenceValue != null ? XHud_Dashboard.Theme_Primary : Color.black,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion

                    #region 转场控制器
                    XGUI.layout_property_field(
                    title: "转场控制器",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: Hud_TransitionController,
                    status_icon: "icon_field_status",
                    status_icon_color: Hud_TransitionController.objectReferenceValue != null ? XHud_Dashboard.Theme_Primary : Color.black,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion
                }

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                #endregion

                #region 工具
                BaseScript.fold_tool = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIFilled.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.Theme_Group,
                    title: "工具",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.Theme_Primary,
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: BaseScript.fold_tool);

                if (!BaseScript.fold_tool)
                {
                    #region 截屏工具
                    if (Application.isPlaying)
                        XGUI.SetEnabled(false);

                    if (XGUI.layout_button(
                        text: "截屏工具",
                        tooltip: "",
                        bg_fill: XGUIFilled.实体,
                        bg_color: XGUIColor.深空灰,
                        bg_color_gui: Color.white,
                        button_text_color: Color.white,
                        press_fill: XGUIFilled.实体,
                        press_color: XGUIColor.深空灰,
                        press_text_color: Color.gray,
                        font_size: XGUIFontSize.M,
                        anchor: TextAnchor.MiddleCenter,
                        margin: new RectOffset(0, 0, 0, 0),
                        padding: new RectOffset(0, 0, 0, 0),
                        height: 25,
                        button_text_font: XGUI.GetFont("xg-medium")))
                    {
                        XHud_CameraCapture cc = FindFirstObjectByType<XHud_CameraCapture>();
                        if (cc == null)
                        {
                            GameObject obj_cc = new GameObject();
                            obj_cc.name = "CameraCapture";
                            obj_cc.AddComponent<XHud_CameraCapture>();
                        }
                        else
                        {
                            EditorApplication.delayCall += () =>
                            {
                                XGUI.dialog(
                                    type: XGUIDialogType.警告,
                                    windowtitle: "XHud - 管理器消息",
                                    title: "创建截屏工具",
                                    msg: "场景中已存在CameraCapture！",
                                    ok: "选中",
                                    cancel: "暂不",
                                    PrimaryIndex: 0,
                                    usemodal: true,
                                    themecolor: XHud_Dashboard.Theme_Primary,
                                    on_selected: (d) =>
                                    {
                                        if (d == "选中")
                                            Selection.activeGameObject = cc.gameObject;
                                        return;
                                    });
                            };
                        }
                        return;
                    }
                    XGUI.SetEnabled(true);
                    #endregion

                    XGUI.layout_space(5);

                    #region 创建效能面板
                    if (XGUI.layout_button(
                        text: "创建效能面板",
                        tooltip: "",
                        bg_fill: XGUIFilled.实体,
                        bg_color: XGUIColor.深空灰,
                        bg_color_gui: Color.white,
                        button_text_color: Color.white,
                        press_fill: XGUIFilled.实体,
                        press_color: XGUIColor.深空灰,
                        press_text_color: Color.gray,
                        font_size: XGUIFontSize.M,
                        anchor: TextAnchor.MiddleCenter,
                        margin: new RectOffset(0, 0, 0, 0),
                        padding: new RectOffset(0, 0, 0, 0),
                        height: 25,
                        button_text_font: XGUI.GetFont("xg-medium")))
                    {
                        XHud_PerformanceMonitor cc = FindFirstObjectByType<XHud_PerformanceMonitor>();
                        if (cc == null)
                        {
                            #region  创建根物体
                            GameObject obj_cc = new GameObject();
                            obj_cc.layer = LayerMask.NameToLayer("XHud");
                            obj_cc.name = "PerformanceMonitor";
                            RectTransform rect = obj_cc.AddComponent<RectTransform>();
                            rect.SetParent(BaseScript.HudCanvas_Screen.transform);
                            rect.anchoredPosition3D = Vector3.zero;
                            rect.sizeDelta = new Vector2(240, 500);
                            rect.localEulerAngles = Vector3.zero;
                            rect.localScale = Vector3.one;
                            XHud_PerformanceMonitor monitor = obj_cc.AddComponent<XHud_PerformanceMonitor>();
                            monitor.updateInterval = 0.2f;
                            monitor.RectMargin = 20;
                            monitor.ScaleRatio = 1f;
                            #endregion

                            #region  创建标题
                            GameObject obj_Header_Root = new GameObject();
                            obj_Header_Root.layer = LayerMask.NameToLayer("XHud");
                            obj_Header_Root.name = "Header";
                            RectTransform rect_header_root = obj_Header_Root.AddComponent<RectTransform>();
                            rect_header_root.SetParent(rect);
                            rect_header_root.sizeDelta = new Vector2(0, 35);
                            rect_header_root.anchorMin = new Vector2(0, 1);
                            rect_header_root.anchorMax = new Vector2(1, 1);
                            rect_header_root.pivot = new Vector2(0.5f, 0.5f);
                            rect_header_root.anchoredPosition3D = Vector3.zero;
                            rect_header_root.localEulerAngles = Vector3.zero;
                            rect_header_root.localScale = Vector3.one;
                            #endregion

                            #region  创建标题背景
                            GameObject obj_Header_Bg = new GameObject();
                            obj_Header_Bg.layer = LayerMask.NameToLayer("XHud");
                            obj_Header_Bg.name = "Bg";
                            RectTransform rect_header_bg = obj_Header_Bg.AddComponent<RectTransform>();
                            Image img_header_bg = obj_Header_Bg.AddComponent<Image>();
                            rect_header_bg.SetParent(rect_header_root);
                            rect_header_bg.sizeDelta = new Vector2(0, 0);
                            rect_header_bg.anchorMin = new Vector2(0, 0);
                            rect_header_bg.anchorMax = new Vector2(1, 1);
                            rect_header_bg.pivot = new Vector2(0.5f, 0.5f);
                            rect_header_bg.anchoredPosition3D = Vector3.zero;
                            rect_header_bg.localEulerAngles = Vector3.zero;
                            rect_header_bg.localScale = Vector3.one;
                            img_header_bg.sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{XHud_Dashboard.Get_Path_XHUD_SPRITES_Path()}Others/BtnRect_Pure.png");
                            img_header_bg.raycastTarget = false;
                            img_header_bg.color = XHud_Dashboard.Theme_Primary;
                            img_header_bg.type = Image.Type.Sliced;
                            #endregion

                            #region  创建标题图标
                            GameObject obj_Header_Icon = new GameObject();
                            obj_Header_Icon.layer = LayerMask.NameToLayer("XHud");
                            obj_Header_Icon.name = "Icon";
                            RectTransform rect_header_icon = obj_Header_Icon.AddComponent<RectTransform>();
                            Image img_header_icon = obj_Header_Icon.AddComponent<Image>();
                            rect_header_icon.SetParent(rect);
                            rect_header_icon.sizeDelta = new Vector2(20, 20);
                            rect_header_icon.anchorMin = new Vector2(0, 1);
                            rect_header_icon.anchorMax = new Vector2(0, 1);
                            rect_header_icon.pivot = new Vector2(0.5f, 0.5f);
                            rect_header_icon.anchoredPosition3D = Vector3.right * 40;
                            rect_header_icon.localEulerAngles = Vector3.zero;
                            rect_header_icon.localScale = Vector3.one;
                            img_header_icon.sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{XHud_Dashboard.Get_Path_XHUD_SPRITES_Path()}Others/Eye.png");
                            img_header_icon.raycastTarget = false;
                            img_header_icon.color = Color.black;
                            img_header_icon.type = Image.Type.Simple;
                            #endregion

                            #region  创建标题文字
                            GameObject obj_HeaderText = new GameObject();
                            obj_HeaderText.layer = LayerMask.NameToLayer("XHud");
                            obj_HeaderText.name = "Title";
                            RectTransform rect_header_title = obj_HeaderText.AddComponent<RectTransform>();
                            XHud_Module_TmpText txt_header_title = obj_HeaderText.AddComponent<XHud_Module_TmpText>();
                            rect_header_title.SetParent(rect_header_bg);
                            rect_header_title.sizeDelta = new Vector2(0, 30);
                            rect_header_title.anchorMin = new Vector2(0, 0.5f);
                            rect_header_title.anchorMax = new Vector2(1, 0.5f);
                            rect_header_title.pivot = new Vector2(0.5f, 0.5f);
                            rect_header_title.anchoredPosition3D = Vector3.zero;
                            rect_header_title.localEulerAngles = Vector3.zero;
                            rect_header_title.localScale = Vector3.one;
                            txt_header_title.TextStyleInfo.tmp_Set_FontAsset(AssetDatabase.LoadAssetAtPath<TMP_FontAsset>($"{XHud_Dashboard.Get_Path_XHUD_FONTS_Path()}Tmp/sx_bold SDF.asset"));
                            txt_header_title.TextStyleInfo.gen_RayCastSet(false);
                            txt_header_title.raycastTarget = false;
                            txt_header_title.color = Color.black;
                            txt_header_title.fontSize = 16;
                            txt_header_title.alignment = TextAlignmentOptions.Left;
                            txt_header_title.textWrappingMode = TextWrappingModes.NoWrap;
                            txt_header_title.overflowMode = TextOverflowModes.Truncate;
                            txt_header_title.fontStyle = FontStyles.Bold;
                            txt_header_title.margin = new Vector4(75, 0, 15, 0);
                            txt_header_title.TextStyleInfo.tmp_Set_FontColor(Color.black);
                            txt_header_title.TextStyleInfo.tmp_Set_FontSize(16);
                            txt_header_title.TextStyleInfo.tmp_Set_Alignment(TmpContentAnchor.左);
                            txt_header_title.TextStyleInfo.tmp_Set_WordWrappingMode(TextWrappingModes.NoWrap);
                            txt_header_title.TextStyleInfo.tmp_Set_Overflow(TextOverflowModes.Truncate);
                            txt_header_title.TextStyleInfo.tmp_Set_FontStyle(FontStyles.Bold);
                            txt_header_title.TextStyleInfo.tmp_Set_MarginSet(new Vector4(75, 0, 15, 0));
                            txt_header_title.tmp_Set_Content("DataMonitor");
                            #endregion
                            monitor.Title = txt_header_title;
                            #region  创建列表
                            GameObject obj_Header_List = new GameObject();
                            obj_Header_List.layer = LayerMask.NameToLayer("XHud");
                            obj_Header_List.name = "List";
                            RectTransform rect_header_list = obj_Header_List.AddComponent<RectTransform>();
                            rect_header_list.SetParent(rect);
                            rect_header_list.sizeDelta = new Vector2(0, 0);
                            rect_header_list.anchorMin = new Vector2(0, 0);
                            rect_header_list.anchorMax = new Vector2(1, 1);
                            rect_header_list.pivot = new Vector2(0.5f, 0.5f);
                            rect_header_list.anchoredPosition3D = Vector3.zero;
                            rect_header_list.offsetMin = new Vector2(0, 0);
                            rect_header_list.offsetMax = new Vector2(0, -32);
                            rect_header_list.localEulerAngles = Vector3.zero;
                            rect_header_list.localScale = Vector3.one;
                            ScrollRect rect_header_scroll = obj_Header_List.AddComponent<ScrollRect>();
                            rect_header_scroll.vertical = true;
                            rect_header_scroll.horizontal = false;
                            #endregion
                            monitor.ScrollRect = rect_header_scroll;
                            #region  创建列表可视区域
                            GameObject obj_Header_ListViewport = new GameObject();
                            obj_Header_ListViewport.layer = LayerMask.NameToLayer("XHud");
                            obj_Header_ListViewport.name = "Viewport";
                            RectTransform rect_header_listviewport = obj_Header_ListViewport.AddComponent<RectTransform>();
                            rect_header_listviewport.SetParent(rect_header_list);
                            rect_header_listviewport.sizeDelta = new Vector2(0, 0);
                            rect_header_listviewport.anchorMin = new Vector2(0, 0);
                            rect_header_listviewport.anchorMax = new Vector2(1, 1);
                            rect_header_listviewport.pivot = new Vector2(0.5f, 0.5f);
                            rect_header_listviewport.anchoredPosition3D = Vector3.zero;
                            rect_header_listviewport.localEulerAngles = Vector3.zero;
                            rect_header_listviewport.localScale = Vector3.one;
                            Image img_header_viewport = obj_Header_ListViewport.AddComponent<Image>();
                            Mask img_header_viewport_mask = obj_Header_ListViewport.AddComponent<Mask>();
                            img_header_viewport_mask.showMaskGraphic = false;
                            RectMask2D img_header_viewport_maskrect = obj_Header_ListViewport.AddComponent<RectMask2D>();
                            img_header_viewport_maskrect.softness = new Vector2Int(0, 245);
                            img_header_viewport_maskrect.padding = new Vector4(0, 0, 0, -122);
                            #endregion

                            #region  创建列表内容区域
                            GameObject obj_Header_ListContent = new GameObject();
                            obj_Header_ListContent.layer = LayerMask.NameToLayer("XHud");
                            obj_Header_ListContent.name = "Content";
                            RectTransform rect_header_listcontent = obj_Header_ListContent.AddComponent<RectTransform>();
                            rect_header_listcontent.SetParent(rect_header_listviewport);
                            rect_header_listcontent.sizeDelta = new Vector2(0, 0);
                            rect_header_listcontent.anchorMin = new Vector2(0, 0);
                            rect_header_listcontent.anchorMax = new Vector2(1, 1);
                            rect_header_listcontent.pivot = new Vector2(0.5f, 1f);
                            rect_header_listcontent.anchoredPosition3D = Vector3.zero;
                            rect_header_listcontent.localEulerAngles = Vector3.zero;
                            rect_header_listcontent.localScale = Vector3.one;
                            VerticalLayoutGroup img_header_list_content_vgp = obj_Header_ListContent.AddComponent<VerticalLayoutGroup>();
                            img_header_list_content_vgp.spacing = 3.6f;
                            img_header_list_content_vgp.childAlignment = TextAnchor.UpperLeft;
                            img_header_list_content_vgp.childControlWidth = true;
                            img_header_list_content_vgp.childForceExpandWidth = true;
                            img_header_list_content_vgp.childForceExpandHeight = false;
                            ContentSizeFitter header_content_sizefilter = obj_Header_ListContent.AddComponent<ContentSizeFitter>();
                            header_content_sizefilter.verticalFit = ContentSizeFitter.FitMode.MinSize;

                            rect_header_scroll.content = rect_header_listcontent;
                            rect_header_scroll.viewport = rect_header_listviewport;
                            #endregion

                            #region 创建列表内容节点
                            string[] node_names = new string[10] { "FPS", "CpuTime", "ScreenSize", "Resolution", "FreshRate", "Batches", "SetpassCalls", "Vertices", "Triangles", "DrawCalls" };

                            for (int i = 0; i < node_names.Length; i++)
                            {
                                GameObject obj_Header_ListNode = new GameObject();
                                obj_Header_ListNode.layer = LayerMask.NameToLayer("XHud");
                                obj_Header_ListNode.name = $"Node_{node_names[i]}";
                                RectTransform rect_monitornode = obj_Header_ListNode.AddComponent<RectTransform>();
                                rect_monitornode.SetParent(rect_header_listcontent);
                                rect_monitornode.sizeDelta = new Vector2(0, 60);
                                rect_monitornode.anchorMin = new Vector2(0, 1);
                                rect_monitornode.anchorMax = new Vector2(0, 1);
                                rect_monitornode.pivot = new Vector2(0.5f, 0.5f);
                                rect_monitornode.anchoredPosition3D = Vector3.zero;
                                rect_monitornode.localEulerAngles = Vector3.zero;
                                rect_monitornode.localScale = Vector3.one;

                                Image monitornode_bg = obj_Header_ListNode.AddComponent<Image>();
                                monitornode_bg.sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{XHud_Dashboard.Get_Path_XHUD_SPRITES_Path()}Others/BtnRect_3Dot.png");
                                monitornode_bg.color = new Color(0.099f, 0.099f, 0.099f, 0.5960785f);
                                monitornode_bg.type = Image.Type.Sliced;
                                XHud_PerfomanceMonitor_Node monitornode = obj_Header_ListNode.AddComponent<XHud_PerfomanceMonitor_Node>();
                                monitornode.title = node_names[i];
                                monitornode.value = "-";

                                #region  创建标题文字
                                GameObject obj_NodeText_Title = new GameObject();
                                obj_NodeText_Title.layer = LayerMask.NameToLayer("XHud");
                                obj_NodeText_Title.name = "Title";
                                RectTransform rect_node_title = obj_NodeText_Title.AddComponent<RectTransform>();
                                XHud_Module_TmpText txt_node_title = obj_NodeText_Title.AddComponent<XHud_Module_TmpText>();
                                rect_node_title.SetParent(rect_monitornode);

                                rect_node_title.anchorMin = new Vector2(0, 0);
                                rect_node_title.anchorMax = new Vector2(1, 1);
                                rect_node_title.pivot = new Vector2(0.5f, 0.5f);
                                rect_node_title.anchoredPosition3D = new Vector3(0, 0, 0);  // Y位置
                                rect_node_title.offsetMin = new Vector2(20, 30);  // Left:20px
                                rect_node_title.offsetMax = new Vector2(-25, -10); // Right:50px
                                rect_node_title.localEulerAngles = Vector3.zero;
                                rect_node_title.localScale = Vector3.one;
                                txt_node_title.TextStyleInfo.tmp_Set_FontAsset(AssetDatabase.LoadAssetAtPath<TMP_FontAsset>($"{XHud_Dashboard.Get_Path_XHUD_FONTS_Path()}Tmp/sx_bold SDF.asset"));
                                txt_node_title.TextStyleInfo.gen_RayCastSet(false);
                                txt_node_title.TextStyleInfo.gen_Set_MaskableSet(true);
                                txt_node_title.maskable = true;
                                txt_node_title.raycastTarget = false;
                                txt_node_title.color = monitornode.col_title;
                                txt_node_title.fontSize = 14;
                                txt_node_title.alignment = TextAlignmentOptions.Left;
                                txt_node_title.textWrappingMode = TextWrappingModes.NoWrap;
                                txt_node_title.overflowMode = TextOverflowModes.Ellipsis;
                                txt_node_title.fontStyle = FontStyles.Normal;
                                txt_node_title.margin = new Vector4(0, 0, 0, 0);
                                txt_node_title.TextStyleInfo.tmp_Set_FontColor(monitornode.col_title);
                                txt_node_title.TextStyleInfo.tmp_Set_FontSize(14);
                                txt_node_title.TextStyleInfo.tmp_Set_Alignment(TmpContentAnchor.左);
                                txt_node_title.TextStyleInfo.tmp_Set_WordWrappingMode(TextWrappingModes.NoWrap);
                                txt_node_title.TextStyleInfo.tmp_Set_Overflow(TextOverflowModes.Ellipsis);
                                txt_node_title.TextStyleInfo.tmp_Set_FontStyle(FontStyles.Normal);
                                txt_node_title.TextStyleInfo.tmp_Set_MarginSet(Vector4.zero);
                                txt_node_title.tmp_Set_Content(node_names[i]);
                                #endregion

                                #region  创建数值文字
                                GameObject obj_NodeText_Value = new GameObject();
                                obj_NodeText_Value.layer = LayerMask.NameToLayer("XHud");
                                obj_NodeText_Value.name = "Value";
                                RectTransform rect_node_value = obj_NodeText_Value.AddComponent<RectTransform>();
                                XHud_Module_TmpText txt_node_value = obj_NodeText_Value.AddComponent<XHud_Module_TmpText>();
                                rect_node_value.SetParent(rect_monitornode);
                                rect_node_value.anchorMin = new Vector2(0, 0);
                                rect_node_value.anchorMax = new Vector2(1, 1);
                                rect_node_value.pivot = new Vector2(0.5f, 0.5f);
                                rect_node_value.anchoredPosition3D = new Vector3(0, 0, 0);  // Y位置
                                rect_node_value.offsetMin = new Vector2(50, 10);  // Left:20px
                                rect_node_value.offsetMax = new Vector2(-20, -30); // Right:50px
                                rect_node_value.localEulerAngles = Vector3.zero;
                                rect_node_value.localScale = Vector3.one;
                                txt_node_value.TextStyleInfo.tmp_Set_FontAsset(AssetDatabase.LoadAssetAtPath<TMP_FontAsset>($"{XHud_Dashboard.Get_Path_XHUD_FONTS_Path()}Tmp/sx_regular SDF.asset"));
                                txt_node_value.TextStyleInfo.gen_RayCastSet(false);
                                txt_node_value.TextStyleInfo.gen_Set_MaskableSet(true);
                                txt_node_value.maskable = true;
                                txt_node_value.raycastTarget = false;
                                txt_node_value.color = monitornode.col_value;
                                txt_node_value.fontSize = 13;
                                txt_node_value.alignment = TextAlignmentOptions.Right;
                                txt_node_value.textWrappingMode = TextWrappingModes.NoWrap;
                                txt_node_value.overflowMode = TextOverflowModes.Ellipsis;
                                txt_node_value.fontStyle = FontStyles.Normal;
                                txt_node_value.margin = new Vector4(0, 0, 0, 0);
                                txt_node_value.TextStyleInfo.tmp_Set_FontColor(monitornode.col_value);
                                txt_node_value.TextStyleInfo.tmp_Set_FontSize(13);
                                txt_node_value.TextStyleInfo.tmp_Set_Alignment(TmpContentAnchor.右);
                                txt_node_value.TextStyleInfo.tmp_Set_WordWrappingMode(TextWrappingModes.NoWrap);
                                txt_node_value.TextStyleInfo.tmp_Set_Overflow(TextOverflowModes.Ellipsis);
                                txt_node_value.TextStyleInfo.tmp_Set_FontStyle(FontStyles.Normal);
                                txt_node_value.TextStyleInfo.tmp_Set_MarginSet(Vector4.zero);
                                txt_node_value.tmp_Set_Content("-");
                                #endregion

                                monitornode.tmp_title = txt_node_title;
                                monitornode.tmp_value = txt_node_value;
                            }

                            GameObject obj_Header_ListNode_Null = new GameObject();
                            obj_Header_ListNode_Null.layer = LayerMask.NameToLayer("XHud");
                            obj_Header_ListNode_Null.name = $"NullSpace";
                            RectTransform rect_monitornode_null = obj_Header_ListNode_Null.AddComponent<RectTransform>();
                            rect_monitornode_null.SetParent(rect_header_listcontent);
                            rect_monitornode_null.sizeDelta = new Vector2(0, 60);
                            rect_monitornode_null.anchorMin = new Vector2(0, 1);
                            rect_monitornode_null.anchorMax = new Vector2(0, 1);
                            rect_monitornode_null.pivot = new Vector2(0.5f, 0.5f);
                            rect_monitornode_null.anchoredPosition3D = Vector3.zero;
                            rect_monitornode_null.localEulerAngles = Vector3.zero;
                            rect_monitornode_null.localScale = Vector3.one;
                            #endregion

                            Selection.activeGameObject = obj_cc.gameObject;
                        }
                        else
                        {
                            EditorApplication.delayCall += () =>
                            {
                                XGUI.dialog(
                                    type: XGUIDialogType.警告,
                                    windowtitle: "XHud - 管理器消息",
                                    title: "创建效能面板",
                                    msg: "场景中已存在效能面板！无需重复添加！",
                                    ok: "选中",
                                    cancel: "暂不",
                                    PrimaryIndex: 0,
                                    usemodal: true,
                                    themecolor: XHud_Dashboard.Theme_Primary,
                                    on_selected: (d) =>
                                    {
                                        if (d == "选中")
                                            Selection.activeGameObject = cc.gameObject;
                                        return;
                                    });
                            };
                        }
                        return;
                    }
                    #endregion
                }

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                #endregion

                #region 布局
                BaseScript.fold_layout = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIFilled.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.Theme_Group,
                    title: "布局",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.Theme_Primary,
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: BaseScript.fold_layout);

                if (!BaseScript.fold_layout)
                {
                    #region 边距
                    SerializedProperty sp_mar_x = Margins.FindPropertyRelative("x");
                    SerializedProperty sp_mar_y = Margins.FindPropertyRelative("y");
                    SerializedProperty sp_mar_z = Margins.FindPropertyRelative("z");
                    SerializedProperty sp_mar_w = Margins.FindPropertyRelative("w");

                    XGUI.layout_property_field(
                        title: "上边距",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 110,
                        prop: sp_mar_x,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_property_field(
                        title: "下边距",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 110,
                        prop: sp_mar_y,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_property_field(
                        title: "左边距",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 110,
                        prop: sp_mar_z,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_property_field(
                        title: "右边距",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 110,
                        prop: sp_mar_w,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion

                    #region 水平缩放
                    XGUI.layout_property_field(
                        title: "水平缩放",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 110,
                        prop: MarginHorizontal,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion

                    #region 垂直缩放
                    XGUI.layout_property_field(
                        title: "垂直缩放",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 110,
                        prop: MarginVertical,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion

                    #region 整体缩放
                    XGUI.layout_property_field(
                        title: "整体缩放",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 110,
                        prop: MarginMultiply,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion

                    #region 图元ID标识高度
                    XGUI.layout_property_field(
                        title: "图元ID标识高度",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 110,
                        prop: PrimtiveID_LabelLine_Height,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion

                    #region 图元ID标识尺寸
                    XGUI.layout_property_field(
                        title: "图元ID标识尺寸",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 110,
                        prop: PrimtiveID_LabelFont_Size,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion                  

                    XGUI.layout_space(10);

                    #region 图元ID标识颜色 - 线
                    XGUI.layout_colorfield(
                        prop: PrimtiveID_LabelLine_Color,
                        title: "图元ID标识颜色 - 线",
                        title_width: 140,
                        //title_color: Color.white,
                        //icon: "icon_field_status",
                        //icon_color: Color.red,
                        state_title: "Hex：",
                        state_value: $"#{XGUI_Utilitys.Color_To_HexString(PrimtiveID_LabelLine_Color.colorValue)}",
                        state_value_color: PrimtiveID_LabelLine_Color.colorValue);
                    #endregion

                    #region 图元ID标识颜色 - 文字
                    XGUI.layout_colorfield(
                        prop: PrimtiveID_LabelFont_Color,
                        title: "图元ID标识颜色 - 文字",
                        title_width: 140,
                        //title_color: Color.white,
                        //icon: "icon_field_status",
                        //icon_color: Color.red,
                        state_title: "Hex：",
                        state_value: $"#{XGUI_Utilitys.Color_To_HexString(PrimtiveID_LabelFont_Color.colorValue)}",
                        state_value_color: PrimtiveID_LabelFont_Color.colorValue);
                    #endregion
                }

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                #endregion

                #region 辅助
                if (UseSafeFrame.boolValue)
                {
                    BaseScript.fold_visualsafe = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIFilled.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.Theme_Group,
                    title: "辅助",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.Theme_Primary,
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: BaseScript.fold_visualsafe);

                    if (!BaseScript.fold_visualsafe)
                    {
                        XGUI.ChangedCheck_Start();

                        #region 角点标记
                        XGUI.layout_property_field(
                            title: "角点标记",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 110,
                            prop: Color_LayoutAnchorMark,
                            prop_margin: new RectOffset(0, 0, 5, 0));
                        #endregion

                        #region 中心标记
                        XGUI.layout_property_field(
                            title: "中心标记",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 110,
                            prop: Color_CenterMark,
                            prop_margin: new RectOffset(0, 0, 5, 0));
                        #endregion

                        #region 安全框
                        XGUI.layout_property_field(
                            title: "安全框",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 110,
                            prop: Color_FrameLine,
                            prop_margin: new RectOffset(0, 0, 5, 0));
                        #endregion

                        #region 分割标记
                        XGUI.layout_property_field(
                            title: "分割标记",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 110,
                            prop: Color_SeperaterLine,
                            prop_margin: new RectOffset(0, 0, 5, 0));
                        #endregion

                        XGUI.layout_seperator(
                            thickness: 1,
                            color: XHud_Dashboard.Theme_SeperateLine,
                            margin: new RectOffset(15, 15, 20, 15));

                        #region 角点标记尺寸
                        XGUI.layout_property_field(
                            title: "角点标记尺寸",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 110,
                            prop: MarkSize,
                            prop_margin: new RectOffset(0, 0, 5, 0));
                        #endregion

                        #region 安全框粗细
                        XGUI.layout_property_field(
                            title: "安全框粗细",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 110,
                            prop: Safe_FrameLine_Width,
                            prop_margin: new RectOffset(0, 0, 5, 0));
                        #endregion

                        #region 安全中分线长度
                        XGUI.layout_property_field(
                            title: "安全中分线长度",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 110,
                            prop: Safe_Seperater_Length,
                            prop_margin: new RectOffset(0, 0, 5, 0));
                        #endregion

                        #region 安全框边距
                        XGUI.layout_property_field(
                            title: "安全框边距",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 110,
                            prop: Safe_FrameLine_Margins,
                            prop_margin: new RectOffset(0, 0, 5, 0));
                        #endregion

                        #region 中心标记间距
                        XGUI.layout_property_field(
                            title: "中心标记间距",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 110,
                            prop: Safe_CenterMarkDistance,
                            prop_margin: new RectOffset(0, 0, 5, 0));
                        #endregion

                        #region 中心标记宽度
                        XGUI.layout_property_field(
                            title: "中心标记宽度",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 110,
                            prop: Safe_CenterMarkWidth,
                            prop_margin: new RectOffset(0, 0, 5, 0));
                        #endregion

                        #region 中心标记长度
                        XGUI.layout_property_field(
                            title: "中心标记长度",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 110,
                            prop: Safe_CenterMarkLength,
                            prop_margin: new RectOffset(0, 0, 5, 0));
                        #endregion

                        XGUI.layout_space(15);

                        if (XGUI.layout_button(
                            text: "重置参数",
                            tooltip: "",
                            bg_fill: XGUIFilled.实体,
                            bg_color: XGUIColor.深空灰,
                            bg_color_gui: Color.white,
                            button_text_color: new Color(1, 0.4f, 0.4f),
                            press_fill: XGUIFilled.实体,
                            press_color: XGUIColor.深空灰,
                            press_text_color: Color.gray,
                            font_size: XGUIFontSize.M,
                            anchor: TextAnchor.MiddleCenter,
                            margin: new RectOffset(0, 0, 0, 0),
                            padding: new RectOffset(0, 0, 0, 0),
                            height: 25,
                            button_text_font: XGUI.GetFont("xg-medium")))
                        {
                            Color_LayoutAnchorMark.colorValue = Color.white;
                            Color_CenterMark.colorValue = XHud_Dashboard.Theme_Primary;
                            Color_FrameLine.colorValue = Color.white * 0.7f;
                            Color_SeperaterLine.colorValue = XHud_Dashboard.Theme_Primary;
                            MarkSize.floatValue = 10;
                            Safe_FrameLine_Width.floatValue = 0.5f;
                            Safe_Seperater_Length.floatValue = 30f;
                            Safe_FrameLine_Margins.vector2Value = new Vector2(50, 50);
                            Safe_CenterMarkDistance.floatValue = 40f;
                            Safe_CenterMarkWidth.floatValue = 1f;
                            Safe_CenterMarkLength.floatValue = 12f;

                            xHud_EditorUpdate_HelperVisual();
                            xHud_EditorUpdate_Canvas_Camera();
                        }

                        if (XGUI.ChangedCheck_End())
                        {
                            SetHighFrequencyMode();
                        }
                    }

                    XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                }
                #endregion

                #region 蓝图
                if (BluePrintMode.boolValue)
                {
                    BaseScript.fold_blueprint = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIFilled.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.Theme_Group,
                    title: "蓝图",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.Theme_Primary,
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: BaseScript.fold_blueprint);

                    if (!BaseScript.fold_blueprint)
                    {
                        #region 蓝图激活状态
                        XGUI.layout_state_displayer_text(
                            title: "蓝图激活状态",
                            title_size: XGUIFontSize.M,
                            subtitle: BluePrint_Displayed.boolValue ? "激活" : "未激活",
                            subtitle_size: XGUIFontSize.M,
                            subtitle_color: XHud_Dashboard.Theme_Primary,
                            padding: new RectOffset(0, 0, 5, 0));
                        #endregion

                        #region 整体透明度
                        XGUI.layout_slider(
                            title: "整体透明度",
                            prop: BluePrint_opacity,
                            left: 0,
                            right: 1,
                            title_width: 100,
                            title_size: XGUIFontSize.M,
                            title_anchor: TextAnchor.MiddleLeft,
                            //title_color: Color.white,
                            //status_icon: "icon_field_status",
                            //status_icon_color: Color.green,
                            prop_margin: new RectOffset(0, 0, 5, 0));
                        #endregion

                        #region 背景透明度
                        XGUI.layout_slider(
                          title: "背景透明度",
                          prop: BluePrint_bg_opacity,
                          left: 0,
                          right: 1,
                          title_width: 100,
                          title_size: XGUIFontSize.M,
                          title_anchor: TextAnchor.MiddleLeft,
                          //title_color: Color.white,
                          //status_icon: "icon_field_status",
                          //status_icon_color: Color.green,
                          prop_margin: new RectOffset(0, 0, 5, 0));
                        #endregion

                        #region 背景动画缓动 
                        XGUI.layout_property_field(
                            title: "背景动画缓动 - 入场",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 120,
                            prop: BluePrint_Bg_AnimationEase_In,
                            prop_margin: new RectOffset(0, 0, 5, 0));
                        #endregion

                        #region 背景动画缓动 
                        XGUI.layout_property_field(
                            title: "背景动画缓动 - 出场",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 120,
                            prop: BluePrint_Bg_AnimationEase_Out,
                            prop_margin: new RectOffset(0, 0, 5, 0));
                        #endregion

                        #region 背景淡化退场延迟
                        XGUI.layout_property_field(
                            title: "背景淡化退场延迟",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 120,
                            prop: BluePrint_Bg_FadeAnimationDelay,
                            prop_margin: new RectOffset(0, 0, 5, 0));
                        #endregion

                        #region 背景色
                        XGUI.layout_colorfield(
                            prop: BluePrint_bg_color,
                            title: "背景色",
                            title_width: 140,
                            //title_color: Color.white,
                            //icon: "icon_field_status",
                            //icon_color: Color.red,
                            state_title: "Hex：",
                            state_value: $"#{XGUI_Utilitys.Color_To_HexString(BluePrint_bg_color.colorValue)}",
                            state_value_color: BluePrint_bg_color.colorValue);
                        #endregion

                        XGUI.layout_seperator(
                           thickness: 1,
                           color: XHud_Dashboard.Theme_SeperateLine,
                           margin: new RectOffset(15, 15, 5, 15));

                        #region 叠加图像透明度
                        XGUI.layout_property_field(
                            title: "叠加图像透明度",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 120,
                            prop: BluePrint_bg_mapOpacity,
                            prop_margin: new RectOffset(0, 0, 5, 0));
                        #endregion

                        XGUI.layout_colorfield(
                        #region 叠加图像颜色
                            prop: BluePrint_bg_decal_color,
                            title: "叠加图像颜色",
                            title_width: 140,
                            //title_color: Color.white,
                            //icon: "icon_field_status",
                            //icon_color: Color.red,
                            state_title: "Hex：",
                            state_value: $"#{XGUI_Utilitys.Color_To_HexString(BluePrint_bg_decal_color.colorValue)}",
                            state_value_color: BluePrint_bg_decal_color.colorValue);
                        #endregion

                        #region 叠加图像
                        XGUI.layout_string_popup(
                            title: "叠加图像",
                            title_width: 110,
                            title_size: XGUIFontSize.M,
                            title_anchor: TextAnchor.MiddleLeft,
                            prop: BluePrint_bg_name,
                            options: BluePrintBgs_Name,
                            opt_text_size: XGUIFontSize.M,
                            opt_text_color: Color.black,
                            opt_text_padding: new RectOffset(10, 10, 0, 0),
                            opt_anchor: TextAnchor.MiddleCenter,
                            opt_font_style: FontStyle.Normal,
                            opt_bg_fill: XGUIFilled.实体,
                            opt_bg_color: XGUIColor.亮白,
                            opt_bg_color_gui: XHud_Dashboard.Theme_Primary,
                            icon_arrow_color: Color.black,
                            margin: new RectOffset(0, 0, 5, 5),
                            padding: new RectOffset(5, 5, 0, 0),
                            title_margin: new RectOffset(0, 0, 0, 0),
                            act_on_changed: (value) =>
                            {
                                BluePrint_bg_name.stringValue = value;
                                BluePrint_bg_name.serializedObject.ApplyModifiedProperties();

                                #region 指定叠加背景                              
                                Texture2D texselected = AssetDatabase.LoadAssetAtPath<Texture2D>($"{XHud_Dashboard.Get_Path_XHUD_TEXTURES_Path()}BluePrints/{value}.png");
                                if (texselected != null)
                                {
                                    BaseScript.hm_BluePrint_SetBgTexture(texselected);
                                }
                                #endregion
                            });
                        #endregion

                        #region 叠加图像 - 平铺模式 
                        XGUI.layout_int_popup(
                            title: "叠加图像 - 平铺模式",
                            title_width: 110,
                            title_size: XGUIFontSize.M,
                            title_anchor: TextAnchor.MiddleLeft,
                            prop: BluePrint_bg_usetilling_index,
                            options: stroptions_enabled,
                            opt_text_size: XGUIFontSize.M,
                            opt_text_color: Color.black,
                            opt_text_padding: new RectOffset(10, 10, 0, 0),
                            opt_anchor: TextAnchor.MiddleCenter,
                            opt_font_style: FontStyle.Normal,
                            opt_bg_fill: XGUIFilled.实体,
                            opt_bg_color: XGUIColor.亮白,
                            opt_bg_color_gui: XHud_Dashboard.Theme_Primary,
                            icon_arrow_color: Color.black,
                            margin: new RectOffset(0, 0, 5, 5),
                            padding: new RectOffset(5, 5, 0, 0),
                            title_margin: new RectOffset(0, 0, 0, 0),
                            act_on_changed: (value) => { });
                        #endregion

                        #region 叠加图像 - 方形比例 
                        XGUI.layout_int_popup(
                            title: "叠加图像 - 方形比例",
                            title_width: 110,
                            title_size: XGUIFontSize.M,
                            title_anchor: TextAnchor.MiddleLeft,
                            prop: BluePrint_bg_usesquareratio_index,
                            options: stroptions_enabled,
                            opt_text_size: XGUIFontSize.M,
                            opt_text_color: Color.black,
                            opt_text_padding: new RectOffset(10, 10, 0, 0),
                            opt_anchor: TextAnchor.MiddleCenter,
                            opt_font_style: FontStyle.Normal,
                            opt_bg_fill: XGUIFilled.实体,
                            opt_bg_color: XGUIColor.亮白,
                            opt_bg_color_gui: XHud_Dashboard.Theme_Primary,
                            icon_arrow_color: Color.black,
                            margin: new RectOffset(0, 0, 5, 5),
                            padding: new RectOffset(5, 5, 0, 0),
                            title_margin: new RectOffset(0, 0, 0, 0),
                            act_on_changed: (value) => { });
                        #endregion


                        #region 叠加图像平铺
                        if (BluePrint_bg_usetilling_index.intValue == 1)
                        {
                            XGUI.layout_property_field(
                                title: "叠加图像平铺",
                                title_size: XGUIFontSize.M,
                                title_hover_color: XHud_Dashboard.Theme_Primary,
                                title_width: 120,
                                prop: BluePrint_bg_tilling,
                                prop_margin: new RectOffset(0, 0, 5, 0));
                        }
                        #endregion

                        XGUI.layout_seperator(
                          thickness: 1,
                          color: XHud_Dashboard.Theme_SeperateLine,
                          margin: new RectOffset(15, 15, 15, 15));

                        #region 网格色
                        XGUI.layout_colorfield(
                            prop: BluePrint_grid_color,
                            title: "网格色",
                            title_width: 120,
                            //title_color: Color.white,
                            //icon: "icon_field_status",
                            //icon_color: Color.red,
                            state_title: "Hex：",
                            state_value: $"#{XGUI_Utilitys.Color_To_HexString(BluePrint_grid_color.colorValue)}",
                            state_value_color: BluePrint_grid_color.colorValue);
                        #endregion

                        #region 网格透明度
                        XGUI.layout_property_field(
                            title: "网格透明度",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 120,
                            prop: BluePrint_grid_Opacity,
                            prop_margin: new RectOffset(0, 0, 5, 0));
                        #endregion

                        #region 网格尺寸
                        XGUI.ChangedCheck_Start();
                        XGUI.layout_property_field(
                            title: "网格尺寸",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 120,
                            prop: BluePrint_grid_size,
                            prop_margin: new RectOffset(0, 0, 5, 0));
                        if (XGUI.ChangedCheck_End())
                        {
                            if (!Application.isPlaying)
                            {
                                BaseScript.hm_BluePrint_Remove();

                                Material mat = AssetDatabase.LoadAssetAtPath<Material>($"{XHud_Dashboard.Get_Path_XHUD_MATERIALS_Path()}BluePrints/Mat/BluePrint.mat");
                                TMP_FontAsset title = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>($"{XHud_Dashboard.Get_Path_XHUD_FONTS_Path()}Fonts/Tmp/sx_bold SDF.asset");
                                TMP_FontAsset subtitle = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>($"{XHud_Dashboard.Get_Path_XHUD_FONTS_Path()}Fonts/Tmp/sx_light SDF.asset");
                                BaseScript.hm_BluePrint_Create(true, mat, title, subtitle);
                            }
                        }
                        #endregion

                        #region 网格线宽度
                        XGUI.layout_property_field(
                            title: "网格线宽度",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 120,
                            prop: BluePrint_linewidth,
                            prop_margin: new RectOffset(0, 0, 5, 0));
                        #endregion

                        #region 网格线百分比
                        XGUI.layout_property_field(
                            title: "网格线百分比",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 120,
                            prop: BluePrint_Grid_LengthPercentage,
                            prop_margin: new RectOffset(0, 0, 5, 0));
                        #endregion

                        #region 网格分级高差
                        XGUI.layout_property_field(
                            title: "网格分级高差",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 120,
                            prop: BluePrint_Grid_LevelHeight,
                            prop_margin: new RectOffset(0, 0, 5, 0));
                        #endregion

                        #region 网格动画结束
                        XGUI.layout_property_field(
                            title: "网格动画结束",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 120,
                            prop: BluePrint_GridEnd,
                            prop_margin: new RectOffset(0, 0, 5, 0));
                        #endregion

                        #region 网格动画速率
                        XGUI.layout_property_field(
                            title: "网格动画速率",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 120,
                            prop: BluePrint_Grid_AnimationDuration,
                            prop_margin: new RectOffset(0, 0, 5, 0));
                        #endregion

                        XGUI.layout_seperator(
                            thickness: 1,
                            color: XHud_Dashboard.Theme_SeperateLine,
                            margin: new RectOffset(15, 15, 15, 15));

                        #region 大标题色
                        XGUI.layout_colorfield(
                            prop: BluePrint_marktitle_color,
                            title: "大标题色",
                            title_width: 140,
                            //title_color: Color.white,
                            //icon: "icon_field_status",
                            //icon_color: Color.red,
                            state_title: "Hex：",
                            state_value: $"#{XGUI_Utilitys.Color_To_HexString(BluePrint_marktitle_color.colorValue)}",
                            state_value_color: BluePrint_marktitle_color.colorValue);
                        #endregion

                        #region 小标题色
                        XGUI.layout_colorfield(
                            prop: BluePrint_marksubtitle_color,
                            title: "小标题色",
                            title_width: 140,
                            //title_color: Color.white,
                            //icon: "icon_field_status",
                            //icon_color: Color.red,
                            state_title: "Hex：",
                            state_value: $"#{XGUI_Utilitys.Color_To_HexString(BluePrint_marksubtitle_color.colorValue)}",
                            state_value_color: BluePrint_marksubtitle_color.colorValue);
                        #endregion

                        #region 水印大标题
                        XGUI.layout_property_field(
                            title: "水印大标题",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 120,
                            prop: BluePrint_title_content,
                            prop_margin: new RectOffset(0, 0, 5, 0));
                        #endregion

                        #region 水印小标题
                        XGUI.layout_property_field(
                            title: "水印小标题",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 120,
                            prop: BluePrint_subtitle_content,
                            prop_margin: new RectOffset(0, 0, 5, 0));
                        #endregion

                        #region 水印透明度
                        XGUI.layout_property_field(
                            title: "水印透明度",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 120,
                            prop: BluePrint_mark_opacity,
                            prop_margin: new RectOffset(0, 0, 5, 0));
                        #endregion

                        #region 水印尺寸
                        XGUI.layout_property_field(
                            title: "水印尺寸",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 120,
                            prop: BluePrint_mark_size,
                            prop_margin: new RectOffset(0, 0, 5, 0));
                        #endregion

                        #region 水印间距
                        XGUI.layout_property_field(
                            title: "水印间距",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 120,
                            prop: BluePrint_mark_space,
                            prop_margin: new RectOffset(0, 0, 5, 0));
                        #endregion

                        #region 水印锚点
                        XGUI.layout_property_field(
                            title: "水印锚点",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 120,
                            prop: BluePrint_MarkAnchors,
                            prop_margin: new RectOffset(0, 0, 5, 0));
                        #endregion

                        #region 水印边距
                        SerializedProperty sp_mar_x = BluePrint_mark_margin.FindPropertyRelative("x");
                        SerializedProperty sp_mar_y = BluePrint_mark_margin.FindPropertyRelative("y");
                        SerializedProperty sp_mar_z = BluePrint_mark_margin.FindPropertyRelative("z");
                        SerializedProperty sp_mar_w = BluePrint_mark_margin.FindPropertyRelative("w");

                        XGUI.layout_property_field(
                            title: "水印 - 上边距",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 110,
                            prop: sp_mar_x,
                            prop_margin: new RectOffset(0, 0, 5, 0));

                        XGUI.layout_property_field(
                            title: "水印 - 下边距",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 110,
                            prop: sp_mar_y,
                            prop_margin: new RectOffset(0, 0, 5, 0));

                        XGUI.layout_property_field(
                            title: "水印 - 左边距",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 110,
                            prop: sp_mar_z,
                            prop_margin: new RectOffset(0, 0, 5, 0));

                        XGUI.layout_property_field(
                            title: "水印 - 右边距",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 110,
                            prop: sp_mar_w,
                            prop_margin: new RectOffset(0, 0, 5, 0));
                        #endregion

                        XGUI.layout_seperator(
                            thickness: 1,
                            color: XHud_Dashboard.Theme_SeperateLine,
                            margin: new RectOffset(15, 15, 15, 15));

                        #region 总动画速度
                        XGUI.layout_property_field(
                            title: "总动画速度",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 120,
                            prop: BluePrint_AnimationDuration,
                            prop_margin: new RectOffset(0, 0, 5, 0));
                        #endregion

                        #region 网格动画缓动
                        XGUI.layout_property_field(
                            title: "网格动画缓动",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 120,
                            prop: BluePrint_Grid_AnimationEase,
                            prop_margin: new RectOffset(0, 0, 5, 0));
                        #endregion

                        XGUI.layout_seperator(
                            thickness: 1,
                            color: XHud_Dashboard.Theme_SeperateLine,
                            margin: new RectOffset(15, 15, 15, 15));

                        #region 开始时退场状态
                        DrawToggle("开始时退场状态", BluePrint_OnStartHide, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, (b) => { });
                        #endregion

                        #region 入/退场影响网格
                        DrawToggle("入/退场影响网格", Eft_Grid, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, (b) => { });
                        #endregion

                        #region 入/退场影响网格透明度
                        DrawToggle("入/退场影响网格透明度", Eft_GridFade, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, (b) => { });
                        #endregion

                        #region 入/退场影响背景
                        DrawToggle("入/退场影响背景", Eft_Bg, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, (b) => { });
                        #endregion

                        #region 入/退场影响水印
                        DrawToggle("入/退场影响水印", Eft_Mark, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, (b) => { });
                        #endregion
                    }

                    if (!Application.isPlaying)
                    {
                        BaseScript.hm_BluePrint_Update();
                    }

                    XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                }
                #endregion

                #region RMS
                if (RMS_Enabled.boolValue)
                {
                    BaseScript.fold_RMS = XGUI.layout_group_start(
                        type: XGUIContainerType.Vertical,
                        bg_fill: XGUIFilled.缺口纯色边框,
                        bg_color: XGUIColor.亮白,
                        bg_color_gui: XHud_Dashboard.Theme_Group,
                        title: "RMS",
                        title_size: XGUIFontSize.M,
                        title_text_color: XHud_Dashboard.Theme_Primary,
                        title_clipping: TextClipping.Clip,
                        padding: new RectOffset(10, 10, 15, 15),
                        foldout: BaseScript.fold_RMS);

                    if (!BaseScript.fold_RMS)
                    {
                        #region RMS说明
                        if (RMS_Nodes.arraySize <= 0)
                        {
                            string msg = $"如果开发的应用布局元素需要<color={hexcol}>同时针对多个分辨率</color>进行匹配和定位，那么请在此添加目标分辨率";

                            XGUI.layout_label(
                                text: msg,
                                bg_fill: XGUIFilled.实体,
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

                        XGUI.layout_space(5);

                        if (Application.isPlaying)
                            XGUI.SetEnabled(false);

                        #region 分辨率列表
                        List_RMS.DoLayoutList();
                        #endregion

                        XGUI.layout_space(5);

                        #region 清空列表
                        if (XGUI.layout_button(
                            text: "清空列表",
                            tooltip: "清空分辨率列表",
                            bg_fill: XGUIFilled.实体,
                            bg_color: XGUIColor.深空灰,
                            bg_color_gui: Color.white,
                            button_text_color: color_important,
                            press_fill: XGUIFilled.实体,
                            press_color: XGUIColor.深空灰,
                            press_text_color: Color.gray,
                            font_size: XGUIFontSize.M,
                            anchor: TextAnchor.MiddleCenter,
                            margin: new RectOffset(0, 0, 0, 0),
                            padding: new RectOffset(0, 0, 0, 0),
                            height: 25,
                            button_text_font: XGUI.GetFont("xg-medium")))
                        {
                            XGUI.dialog(
                                type: XGUIDialogType.警告,
                                windowtitle: "XHud - 管理器消息",
                                title: "RMS列表",
                                msg: "如果清空该列表可能会导致元素匹配分辨率数据异常！请确定是否继续该操作？",
                                ok: "清空",
                                cancel: "暂不",
                                PrimaryIndex: 0,
                                usemodal: true,
                                themecolor: XHud_Dashboard.Theme_Primary,
                                on_selected: (d) =>
                                {
                                    if (d == "清空")
                                    {
                                        RMS_Nodes.ClearArray();
                                        RMS_Nodes.serializedObject.ApplyModifiedProperties();
                                    }
                                });
                            return;
                        }
                        #endregion

                        XGUI.SetEnabled(true);
                    }

                    XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                }
                #endregion

                #region 动效默认参数
                BaseScript.fold_motion_default = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIFilled.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.Theme_Group,
                    title: "动效默认参数",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.Theme_Primary,
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: BaseScript.fold_motion_default);

                if (!BaseScript.fold_motion_default)
                {
                    #region 生成
                    #region 模版库 - 生成
                    if (BaseScript.Hud_Motions != null)
                    {
                        //确保动效库不是空的
                        if (BaseScript.Hud_Motions.ElementMotionList != null && BaseScript.Hud_Motions.ElementMotionList.Count > 0)
                        {
                            //动效列表
                            string[] motnames = BaseScript.Hud_Motions.ElementMotion_GetAllName_With_Create();
                            XGUI.layout_string_popup(
                                title: "生成",
                                title_width: 60,
                                title_size: XGUIFontSize.M,
                                title_anchor: TextAnchor.MiddleLeft,
                                prop: Crc_Lib_Name,
                                options: motnames,
                                opt_text_size: XGUIFontSize.M,
                                opt_text_color: Color.black,
                                opt_text_padding: new RectOffset(10, 10, 0, 0),
                                opt_anchor: TextAnchor.MiddleCenter,
                                opt_font_style: FontStyle.Normal,
                                opt_bg_fill: XGUIFilled.实体,
                                opt_bg_color: XGUIColor.亮白,
                                opt_bg_color_gui: XHud_Dashboard.Theme_Primary,
                                icon_arrow_color: Color.black,
                                margin: new RectOffset(0, 0, 5, 5),
                                padding: new RectOffset(5, 5, 0, 0),
                                title_margin: new RectOffset(0, 0, 0, 0),
                                act_on_changed: (value) =>
                                {
                                    Crc_Lib_Name.stringValue = value;
                                    Crc_Lib_Name.serializedObject.ApplyModifiedProperties();

                                    Motion_Creator crc = BaseScript.Hud_Motions.ElementMotion_GetElementCreator_At_Create(Crc_Lib_Name.stringValue);

                                    CreateArgs_Default.FindPropertyRelative("anchor").enumValueIndex = (int)crc.anchor;
                                    CreateArgs_Default.FindPropertyRelative("Movement.Movement").enumValueIndex = (int)crc.Movement.Movement;
                                    CreateArgs_Default.FindPropertyRelative("Movement.Distance").floatValue = crc.Movement.Distance;
                                    CreateArgs_Default.FindPropertyRelative("Movement.Duration").floatValue = crc.Movement.Duration;
                                    CreateArgs_Default.FindPropertyRelative("Movement.Delay").floatValue = crc.Movement.Delay;
                                    CreateArgs_Default.FindPropertyRelative("Movement.Curve").animationCurveValue = crc.Movement.Curve;
                                    CreateArgs_Default.FindPropertyRelative("Movement.CurveName").stringValue = crc.Movement.CurveName;
                                    CreateArgs_Default.FindPropertyRelative("Movement.Ease").enumValueIndex = (int)crc.Movement.Ease;
                                    CreateArgs_Default.FindPropertyRelative("Rotation.Rotation").enumValueIndex = (int)crc.Rotation.Rotation;
                                    CreateArgs_Default.FindPropertyRelative("Rotation.Degree").floatValue = crc.Rotation.Degree;
                                    CreateArgs_Default.FindPropertyRelative("Rotation.Duration").floatValue = crc.Rotation.Duration;
                                    CreateArgs_Default.FindPropertyRelative("Rotation.Delay").floatValue = crc.Rotation.Delay;
                                    CreateArgs_Default.FindPropertyRelative("Rotation.Curve").animationCurveValue = crc.Rotation.Curve;
                                    CreateArgs_Default.FindPropertyRelative("Rotation.CurveName").stringValue = crc.Rotation.CurveName;
                                    CreateArgs_Default.FindPropertyRelative("Rotation.Ease").enumValueIndex = (int)crc.Rotation.Ease;
                                    CreateArgs_Default.FindPropertyRelative("Alpha.Duration").floatValue = crc.Alpha.Duration;
                                    CreateArgs_Default.FindPropertyRelative("Alpha.Delay").floatValue = crc.Alpha.Delay;
                                    CreateArgs_Default.FindPropertyRelative("Alpha.Curve").animationCurveValue = crc.Alpha.Curve;
                                    CreateArgs_Default.FindPropertyRelative("Alpha.CurveName").stringValue = crc.Alpha.CurveName;
                                    CreateArgs_Default.FindPropertyRelative("Alpha.Ease").enumValueIndex = (int)crc.Alpha.Ease;
                                    CreateArgs_Default.FindPropertyRelative("MotionAnimateEndState").enumValueIndex = (int)crc.MotionAnimateEndState;
                                    CreateArgs_Default.serializedObject.ApplyModifiedProperties();
                                });

                            XGUI.layout_space(12);

                            #region 保存 & 定位模板
                            XGUI.layout_group_start(
                                type: XGUIContainerType.Horizontal,
                                title_clipping: TextClipping.Clip,
                                absolute_padding: true,
                                absolute_margin: true,
                                margin: new RectOffset(0, 0, 0, 0),
                                padding: new RectOffset(5, 10, 0, 0));

                            if (XGUI.layout_button(
                                tooltip: "保存",
                                tex_release: save_r,
                                tex_press: save_p,
                                tex_gui_color: Color.white,
                                border: new RectOffset(0, 0, 0, 0),
                                width: save_r.width,
                                height: save_r.height))
                            {
                                xHud_OpenParameterSetter(HudElementMotionType.Creator);
                                return;
                            }

                            GUILayout.FlexibleSpace();

                            if (XGUI.layout_button(
                              tooltip: "定位",
                              tex_release: locate_r,
                              tex_press: locate_p,
                              tex_gui_color: Color.white,
                              border: new RectOffset(0, 0, 0, 0),
                              width: locate_r.width,
                              height: locate_r.height))
                            {
                                if (!BaseScript.Hud_Motions.ElementMotion_IsExist(Crc_Lib_Name.stringValue))
                                    return;
                                Editor_XHud_MenuItemsAction_OpenLibrary.open_elementmotion();
                                BaseScript.Hud_Motions.ElementMotionLibrary_Location(Crc_Lib_Name.stringValue);
                                return;
                            }

                            GUILayout.FlexibleSpace();

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
                                    windowtitle: "XHud - 管理器消息",
                                    title: "重置动效参数",
                                    msg: "确定要将动效参数重置吗？您将丢失当前的动效参数！",
                                    ok: "重置",
                                    cancel: "暂不",
                                    PrimaryIndex: 0,
                                    usemodal: true,
                                    themecolor: XHud_Dashboard.Theme_Primary,
                                    on_selected: (d) =>
                                    {
                                        if (d == "重置")
                                        {
                                            Crc_Lib_Name.stringValue = null;
                                            Crc_Lib_Name.serializedObject.ApplyModifiedProperties();
                                            ResetMotionParams("c");
                                        }
                                    });
                                return;
                            }

                            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
                            #endregion
                        }
                        else
                        {
                            XGUI.layout_helpbox(
                                state: XGUIHelboxState.警告,
                                title_text: "未在动效库中发现任何动效资源，请先为其添加动效资源！",
                                title_size: XGUIFontSize.M,
                                title_style: FontStyle.Normal,
                                title_color: Color.white * 0.75f);
                        }
                    }
                    else
                    {
                        XGUI.layout_helpbox(
                              state: XGUIHelboxState.警告,
                              title_text: "Hud管理器中未指定动效库，请先配置动效库！",
                              title_size: XGUIFontSize.M,
                              title_style: FontStyle.Normal,
                              title_color: Color.white * 0.75f);
                    }
                    #endregion

                    XGUI.layout_seperator(
                       thickness: 1,
                       color: XHud_Dashboard.Theme_SeperateLine,
                       margin: new RectOffset(15, 15, 18, 15));

                    #region 参数 - 生成
                    SerializedProperty sp_def_create_anchor = CreateArgs_Default.FindPropertyRelative("anchor");

                    #region 锚点
                    XGUI.layout_property_field(
                    title: "锚点",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: sp_def_create_anchor,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion

                    XGUI.layout_group_start(
                        type: XGUIContainerType.Vertical,
                        absolute_padding: true,
                        absolute_margin: true,
                        margin: new RectOffset(0, 0, 10, 0),
                        padding: new RectOffset(15, 0, 0, 0));

                    #region 位移
                    def_create_fold_move = EditorGUILayout.Foldout(def_create_fold_move, "位移", true);

                    if (def_create_fold_move)
                    {
                        SerializedProperty sp_def_create_move_type = CreateArgs_Default.FindPropertyRelative("Movement.Movement");
                        XGUI.layout_property_field(
                            title: "方式",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 90,
                            prop: sp_def_create_move_type,
                            prop_margin: new RectOffset(0, 0, 5, 0));

                        XGUI.layout_space(5);

                        SerializedProperty sp_def_create_move_dis = CreateArgs_Default.FindPropertyRelative("Movement.Distance");
                        XGUI.layout_property_field(
                            title: "距离",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 90,
                            prop: sp_def_create_move_dis,
                            prop_margin: new RectOffset(0, 0, 5, 0));

                        XGUI.layout_space(5);

                        SerializedProperty sp_def_create_move_dur = CreateArgs_Default.FindPropertyRelative("Movement.Duration");
                        XGUI.layout_property_field(
                            title: "耗时",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 90,
                            prop: sp_def_create_move_dur,
                            prop_margin: new RectOffset(0, 0, 5, 0));

                        XGUI.layout_space(5);

                        SerializedProperty sp_def_create_move_delay = CreateArgs_Default.FindPropertyRelative("Movement.Delay");
                        XGUI.layout_property_field(
                            title: "延迟",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 90,
                            prop: sp_def_create_move_delay,
                            prop_margin: new RectOffset(0, 0, 5, 0));

                        XGUI.layout_space(5);

                        SerializedProperty sp_def_create_move_curve = CreateArgs_Default.FindPropertyRelative("Movement.Curve");
                        XGUI.layout_property_field(
                            title: "曲线",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 90,
                            prop: sp_def_create_move_curve,
                            prop_margin: new RectOffset(0, 0, 5, 0));

                        XGUI.layout_space(5);

                        SerializedProperty sp_def_create_move_ease = CreateArgs_Default.FindPropertyRelative("Movement.Ease");
                        XGUI.layout_property_field(
                            title: "缓动",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 90,
                            prop: sp_def_create_move_ease,
                            prop_margin: new RectOffset(0, 0, 5, 0));

                    }
                    #endregion

                    XGUI.layout_space(5);

                    #region 旋转
                    def_create_fold_rotate = EditorGUILayout.Foldout(def_create_fold_rotate, "旋转", true);

                    if (def_create_fold_rotate)
                    {
                        SerializedProperty sp_def_create_rot_type = CreateArgs_Default.FindPropertyRelative("Rotation.Rotation");
                        XGUI.layout_property_field(
                          title: "方式",
                          title_size: XGUIFontSize.M,
                          title_hover_color: XHud_Dashboard.Theme_Primary,
                          title_width: 90,
                          prop: sp_def_create_rot_type,
                          prop_margin: new RectOffset(0, 0, 5, 0));

                        XGUI.layout_space(5);

                        SerializedProperty sp_def_create_rot_deg = CreateArgs_Default.FindPropertyRelative("Rotation.Degree");
                        XGUI.layout_property_field(
                          title: "角度",
                          title_size: XGUIFontSize.M,
                          title_hover_color: XHud_Dashboard.Theme_Primary,
                          title_width: 90,
                          prop: sp_def_create_rot_deg,
                          prop_margin: new RectOffset(0, 0, 5, 0));

                        XGUI.layout_space(5);

                        SerializedProperty sp_def_create_rot_dur = CreateArgs_Default.FindPropertyRelative("Rotation.Duration");
                        XGUI.layout_property_field(
                          title: "耗时",
                          title_size: XGUIFontSize.M,
                          title_hover_color: XHud_Dashboard.Theme_Primary,
                          title_width: 90,
                          prop: sp_def_create_rot_dur,
                          prop_margin: new RectOffset(0, 0, 5, 0));

                        XGUI.layout_space(5);

                        SerializedProperty sp_def_create_rot_delay = CreateArgs_Default.FindPropertyRelative("Rotation.Delay");
                        XGUI.layout_property_field(
                          title: "延迟",
                          title_size: XGUIFontSize.M,
                          title_hover_color: XHud_Dashboard.Theme_Primary,
                          title_width: 90,
                          prop: sp_def_create_rot_delay,
                          prop_margin: new RectOffset(0, 0, 5, 0));

                        XGUI.layout_space(5);

                        SerializedProperty sp_def_create_rot_curve = CreateArgs_Default.FindPropertyRelative("Rotation.Curve");
                        XGUI.layout_property_field(
                          title: "曲线",
                          title_size: XGUIFontSize.M,
                          title_hover_color: XHud_Dashboard.Theme_Primary,
                          title_width: 90,
                          prop: sp_def_create_rot_curve,
                          prop_margin: new RectOffset(0, 0, 5, 0));

                        XGUI.layout_space(5);

                        SerializedProperty sp_def_create_rot_ease = CreateArgs_Default.FindPropertyRelative("Rotation.Ease");
                        XGUI.layout_property_field(
                          title: "缓动",
                          title_size: XGUIFontSize.M,
                          title_hover_color: XHud_Dashboard.Theme_Primary,
                          title_width: 90,
                          prop: sp_def_create_rot_ease,
                          prop_margin: new RectOffset(0, 0, 5, 0));
                    }
                    #endregion

                    XGUI.layout_space(5);

                    #region 透明度                   
                    def_create_fold_alpha = EditorGUILayout.Foldout(def_create_fold_alpha, "透明度", true);

                    if (def_create_fold_alpha)
                    {
                        SerializedProperty sp_def_create_alpha_type = CreateArgs_Default.FindPropertyRelative("Alpha.Duration");
                        XGUI.layout_property_field(
                            title: "耗时",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 90,
                            prop: sp_def_create_alpha_type,
                            prop_margin: new RectOffset(0, 0, 5, 0));

                        XGUI.layout_space(5);

                        SerializedProperty sp_def_create_alpha_delay = CreateArgs_Default.FindPropertyRelative("Alpha.Delay");
                        XGUI.layout_property_field(
                            title: "延迟",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 90,
                            prop: sp_def_create_alpha_delay,
                            prop_margin: new RectOffset(0, 0, 5, 0));

                        XGUI.layout_space(5);

                        SerializedProperty sp_def_create_alpha_curve = CreateArgs_Default.FindPropertyRelative("Alpha.Curve");
                        XGUI.layout_property_field(
                            title: "曲线",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 90,
                            prop: sp_def_create_alpha_curve,
                            prop_margin: new RectOffset(0, 0, 5, 0));

                        XGUI.layout_space(5);

                        SerializedProperty sp_def_create_alpha_ease = CreateArgs_Default.FindPropertyRelative("Alpha.Ease");
                        XGUI.layout_property_field(
                            title: "缓动",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 90,
                            prop: sp_def_create_alpha_ease,
                            prop_margin: new RectOffset(0, 0, 5, 0));
                    }
                    #endregion

                    XGUI.editor_layout_group_end(XGUIContainerType.Vertical);

                    #region 动效结束时机
                    XGUI.ChangedCheck_Start();
                    XGUI.layout_property_field(
                           title: "动效结束时机",
                           title_size: XGUIFontSize.M,
                           title_hover_color: XHud_Dashboard.Theme_Primary,
                           title_width: 90,
                           prop: CreateArgs_MotionAnimateEndState,
                           prop_margin: new RectOffset(0, 0, 15, 0));
                    if (XGUI.ChangedCheck_End())
                    {
                        MotionAnimateEndState state = (MotionAnimateEndState)CreateArgs_MotionAnimateEndState.enumValueIndex;
                        switch (state)
                        {
                            case MotionAnimateEndState.以_移动为准:
                                HudMotion_Movement m = (HudMotion_Movement)CreateArgs_Default.FindPropertyRelative("Movement.Movement").enumValueIndex;
                                if (m == HudMotion_Movement.A_无运动)
                                {
                                    XGUI.dialog(
                                        type: XGUIDialogType.警告,
                                        windowtitle: "XHud - 管理器消息",
                                        title: "设定动画结束时机",
                                        msg: $"当前位移方式为 <color={hexcol}> A_无运动 </color>，将发生<color={hexcol}> 动效动画无法正常结束</color>从而导致<color={hexcol}> 相应的事件和动作委托 </color>不能正确触发的情况 </color>！请将<color={hexcol}> 位移方式 </color>改为<color={hexcol}> 非无运动方式 </color>！",
                                        ok: "明白",
                                        PrimaryIndex: 0,
                                        usemodal: true,
                                        themecolor: XHud_Dashboard.Theme_Primary);

                                    CreateArgs_MotionAnimateEndState.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
                                }
                                break;
                            case MotionAnimateEndState.以_旋转为准:
                                HudMotion_Rotation r = (HudMotion_Rotation)CreateArgs_Default.FindPropertyRelative("Rotation.Rotation").enumValueIndex;
                                if (r == HudMotion_Rotation.A_无旋转)
                                {
                                    XGUI.dialog(
                                        type: XGUIDialogType.警告,
                                        windowtitle: "XHud - 管理器消息",
                                        title: "设定动画结束时机",
                                        msg: $"当前旋转方式为 <color={hexcol}> A_无旋转 </color>，将发生<color={hexcol}> 动效动画无法正常结束</color>从而导致<color={hexcol}> 相应的事件和动作委托 </color>不能正确触发的情况 </color>！请将<color={hexcol}> 旋转方式 </color>改为<color={hexcol}> 非无旋转方式 </color>！",
                                        ok: "明白",
                                        PrimaryIndex: 0,
                                        usemodal: true,
                                        themecolor: XHud_Dashboard.Theme_Primary);

                                    CreateArgs_MotionAnimateEndState.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
                                }
                                break;
                        }
                    }
                    #endregion
                    #endregion
                    #endregion

                    XGUI.layout_seperator(
                        thickness: 3,
                        color: XHud_Dashboard.Theme_SeperateLine * 1.5f,
                        margin: new RectOffset(15, 15, 18, 15));

                    #region 回收
                    #region 模版库 - 回收
                    if (BaseScript.Hud_Motions != null)
                    {
                        //确保动效库不是空的
                        if (BaseScript.Hud_Motions.ElementMotionList != null && BaseScript.Hud_Motions.ElementMotionList.Count > 0)
                        {
                            //动效列表
                            string[] motnames = BaseScript.Hud_Motions.ElementMotion_GetAllName_With_Recycle();
                            XGUI.layout_string_popup(
                                title: "回收",
                                title_width: 60,
                                title_size: XGUIFontSize.M,
                                title_anchor: TextAnchor.MiddleLeft,
                                prop: Rec_Lib_Name,
                                options: motnames,
                                opt_text_size: XGUIFontSize.M,
                                opt_text_color: Color.black,
                                opt_text_padding: new RectOffset(10, 10, 0, 0),
                                opt_anchor: TextAnchor.MiddleCenter,
                                opt_font_style: FontStyle.Normal,
                                opt_bg_fill: XGUIFilled.实体,
                                opt_bg_color: XGUIColor.亮白,
                                opt_bg_color_gui: XHud_Dashboard.Theme_Primary,
                                icon_arrow_color: Color.black,
                                margin: new RectOffset(0, 0, 5, 5),
                                padding: new RectOffset(5, 5, 0, 0),
                                title_margin: new RectOffset(0, 0, 0, 0),
                                act_on_changed: (value) =>
                                {
                                    Rec_Lib_Name.stringValue = value;
                                    Rec_Lib_Name.serializedObject.ApplyModifiedProperties();

                                    Motion_Recycler rec = BaseScript.Hud_Motions.ElementMotion_GetElementCreator_At_Recycle(Rec_Lib_Name.stringValue);

                                    RecycleArgs_Default.FindPropertyRelative("Movement.Movement").enumValueIndex = (int)rec.Movement.Movement;
                                    RecycleArgs_Default.FindPropertyRelative("Movement.Distance").floatValue = rec.Movement.Distance;
                                    RecycleArgs_Default.FindPropertyRelative("Movement.Duration").floatValue = rec.Movement.Duration;
                                    RecycleArgs_Default.FindPropertyRelative("Movement.Delay").floatValue = rec.Movement.Delay;
                                    RecycleArgs_Default.FindPropertyRelative("Movement.Curve").animationCurveValue = rec.Movement.Curve;
                                    RecycleArgs_Default.FindPropertyRelative("Movement.CurveName").stringValue = rec.Movement.CurveName;
                                    RecycleArgs_Default.FindPropertyRelative("Movement.Ease").enumValueIndex = (int)rec.Movement.Ease;
                                    RecycleArgs_Default.FindPropertyRelative("Rotation.Rotation").enumValueIndex = (int)rec.Rotation.Rotation;
                                    RecycleArgs_Default.FindPropertyRelative("Rotation.Degree").floatValue = rec.Rotation.Degree;
                                    RecycleArgs_Default.FindPropertyRelative("Rotation.Duration").floatValue = rec.Rotation.Duration;
                                    RecycleArgs_Default.FindPropertyRelative("Rotation.Delay").floatValue = rec.Rotation.Delay;
                                    RecycleArgs_Default.FindPropertyRelative("Rotation.Curve").animationCurveValue = rec.Rotation.Curve;
                                    RecycleArgs_Default.FindPropertyRelative("Rotation.CurveName").stringValue = rec.Rotation.CurveName;
                                    RecycleArgs_Default.FindPropertyRelative("Rotation.Ease").enumValueIndex = (int)rec.Rotation.Ease;
                                    RecycleArgs_Default.FindPropertyRelative("Alpha.Duration").floatValue = rec.Alpha.Duration;
                                    RecycleArgs_Default.FindPropertyRelative("Alpha.Delay").floatValue = rec.Alpha.Delay;
                                    RecycleArgs_Default.FindPropertyRelative("Alpha.Curve").animationCurveValue = rec.Alpha.Curve;
                                    RecycleArgs_Default.FindPropertyRelative("Alpha.CurveName").stringValue = rec.Alpha.CurveName;
                                    RecycleArgs_Default.FindPropertyRelative("Alpha.Ease").enumValueIndex = (int)rec.Alpha.Ease;
                                    RecycleArgs_Default.FindPropertyRelative("MotionAnimateEndState").enumValueIndex = (int)rec.MotionAnimateEndState;
                                    RecycleArgs_Default.serializedObject.ApplyModifiedProperties();
                                });

                            XGUI.layout_space(12);

                            #region 保存 & 定位模板
                            XGUI.layout_group_start(
                                type: XGUIContainerType.Horizontal,
                                title_clipping: TextClipping.Clip,
                                absolute_padding: true,
                                absolute_margin: true,
                                margin: new RectOffset(0, 0, 0, 0),
                                padding: new RectOffset(5, 10, 0, 0));

                            if (XGUI.layout_button(
                                tooltip: "保存",
                                tex_release: save_r,
                                tex_press: save_p,
                                tex_gui_color: Color.white,
                                border: new RectOffset(0, 0, 0, 0),
                                width: save_r.width,
                                height: save_r.height))
                            {
                                xHud_OpenParameterSetter(HudElementMotionType.Recycler);
                                return;
                            }

                            GUILayout.FlexibleSpace();

                            if (XGUI.layout_button(
                              tooltip: "定位",
                              tex_release: locate_r,
                              tex_press: locate_p,
                              tex_gui_color: Color.white,
                              border: new RectOffset(0, 0, 0, 0),
                              width: locate_r.width,
                              height: locate_r.height))
                            {
                                if (!BaseScript.Hud_Motions.ElementMotion_IsExist(Rec_Lib_Name.stringValue))
                                    return;
                                Editor_XHud_MenuItemsAction_OpenLibrary.open_elementmotion();
                                BaseScript.Hud_Motions.ElementMotionLibrary_Location(Rec_Lib_Name.stringValue);
                                return;
                            }

                            GUILayout.FlexibleSpace();

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
                                    windowtitle: "XHud - 管理器消息",
                                    title: "重置动效参数",
                                    msg: "确定要将动效参数重置吗？您将丢失当前的动效参数！",
                                    ok: "重置",
                                    cancel: "暂不",
                                    PrimaryIndex: 0,
                                    usemodal: true,
                                    themecolor: XHud_Dashboard.Theme_Primary,
                                    on_selected: (d) =>
                                    {
                                        if (d == "重置")
                                        {
                                            ResetMotionParams("r");
                                        }
                                    });
                                return;
                            }

                            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
                            #endregion
                        }
                        else
                        {
                            XGUI.layout_helpbox(
                                state: XGUIHelboxState.警告,
                                title_text: "未在动效库中发现任何动效资源，请先为其添加动效资源！",
                                title_size: XGUIFontSize.M,
                                title_style: FontStyle.Normal,
                                title_color: Color.white * 0.75f);
                        }
                    }
                    else
                    {
                        XGUI.layout_helpbox(
                              state: XGUIHelboxState.警告,
                              title_text: "Hud管理器中未指定动效库，请先配置动效库！",
                              title_size: XGUIFontSize.M,
                              title_style: FontStyle.Normal,
                              title_color: Color.white * 0.75f);
                    }
                    #endregion

                    XGUI.layout_seperator(
                        thickness: 1,
                        color: XHud_Dashboard.Theme_SeperateLine,
                        margin: new RectOffset(15, 15, 18, 15));

                    #region 参数 - 回收
                    XGUI.layout_group_start(
                      type: XGUIContainerType.Vertical,
                      absolute_padding: true,
                      absolute_margin: true,
                      margin: new RectOffset(0, 0, 10, 0),
                      padding: new RectOffset(15, 0, 0, 0));

                    #region 位移          
                    def_recycle_fold_move = EditorGUILayout.Foldout(def_recycle_fold_move, "位移", true);

                    if (def_recycle_fold_move)
                    {
                        SerializedProperty sp_def_recycle_move_type = RecycleArgs_Default.FindPropertyRelative("Movement.Movement");
                        XGUI.layout_property_field(
                            title: "方式",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 90,
                            prop: sp_def_recycle_move_type,
                            prop_margin: new RectOffset(0, 0, 5, 0));

                        XGUI.layout_space(5);

                        SerializedProperty sp_def_recycle_move_dis = RecycleArgs_Default.FindPropertyRelative("Movement.Distance");
                        XGUI.layout_property_field(
                            title: "距离",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 90,
                            prop: sp_def_recycle_move_dis,
                            prop_margin: new RectOffset(0, 0, 5, 0));

                        XGUI.layout_space(5);

                        SerializedProperty sp_def_recycle_move_dur = RecycleArgs_Default.FindPropertyRelative("Movement.Duration");
                        XGUI.layout_property_field(
                            title: "耗时",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 90,
                            prop: sp_def_recycle_move_dur,
                            prop_margin: new RectOffset(0, 0, 5, 0));

                        XGUI.layout_space(5);

                        SerializedProperty sp_def_recycle_move_delay = RecycleArgs_Default.FindPropertyRelative("Movement.Delay");
                        XGUI.layout_property_field(
                            title: "延迟",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 90,
                            prop: sp_def_recycle_move_delay,
                            prop_margin: new RectOffset(0, 0, 5, 0));

                        XGUI.layout_space(5);

                        SerializedProperty sp_def_recycle_move_curve = RecycleArgs_Default.FindPropertyRelative("Movement.Curve");
                        XGUI.layout_property_field(
                            title: "曲线",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 90,
                            prop: sp_def_recycle_move_curve,
                            prop_margin: new RectOffset(0, 0, 5, 0));

                        XGUI.layout_space(5);

                        SerializedProperty sp_def_recycle_move_ease = RecycleArgs_Default.FindPropertyRelative("Movement.Ease");
                        XGUI.layout_property_field(
                            title: "缓动",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 90,
                            prop: sp_def_recycle_move_ease,
                            prop_margin: new RectOffset(0, 0, 5, 0));
                    }
                    #endregion

                    XGUI.layout_space(5);

                    #region 旋转      
                    def_recycle_fold_rotate = EditorGUILayout.Foldout(def_recycle_fold_rotate, "旋转", true);

                    if (def_recycle_fold_rotate)
                    {
                        SerializedProperty sp_def_recycle_rot_type = RecycleArgs_Default.FindPropertyRelative("Rotation.Rotation");
                        XGUI.layout_property_field(
                            title: "方式",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 90,
                            prop: sp_def_recycle_rot_type,
                            prop_margin: new RectOffset(0, 0, 5, 0));

                        XGUI.layout_space(5);

                        SerializedProperty sp_def_recycle_rot_deg = RecycleArgs_Default.FindPropertyRelative("Rotation.Degree");
                        XGUI.layout_property_field(
                            title: "角度",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 90,
                            prop: sp_def_recycle_rot_deg,
                            prop_margin: new RectOffset(0, 0, 5, 0));

                        XGUI.layout_space(5);

                        SerializedProperty sp_def_recycle_rot_dur = RecycleArgs_Default.FindPropertyRelative("Rotation.Duration");
                        XGUI.layout_property_field(
                            title: "耗时",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 90,
                            prop: sp_def_recycle_rot_dur,
                            prop_margin: new RectOffset(0, 0, 5, 0));

                        XGUI.layout_space(5);

                        SerializedProperty sp_def_recycle_rot_delay = RecycleArgs_Default.FindPropertyRelative("Rotation.Delay");
                        XGUI.layout_property_field(
                            title: "延迟",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 90,
                            prop: sp_def_recycle_rot_delay,
                            prop_margin: new RectOffset(0, 0, 5, 0));

                        XGUI.layout_space(5);

                        SerializedProperty sp_def_recycle_rot_curve = RecycleArgs_Default.FindPropertyRelative("Rotation.Curve");
                        XGUI.layout_property_field(
                            title: "曲线",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 90,
                            prop: sp_def_recycle_rot_curve,
                            prop_margin: new RectOffset(0, 0, 5, 0));

                        XGUI.layout_space(5);

                        SerializedProperty sp_def_recycle_rot_ease = RecycleArgs_Default.FindPropertyRelative("Rotation.Ease");
                        XGUI.layout_property_field(
                            title: "缓动",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 90,
                            prop: sp_def_recycle_rot_ease,
                            prop_margin: new RectOffset(0, 0, 5, 0));
                    }
                    #endregion

                    XGUI.layout_space(5);

                    #region 透明度      
                    def_recycle_fold_alpha = EditorGUILayout.Foldout(def_recycle_fold_alpha, "透明度", true);

                    if (def_recycle_fold_alpha)
                    {
                        SerializedProperty sp_def_recycle_alpha_type = RecycleArgs_Default.FindPropertyRelative("Alpha.Duration");
                        XGUI.layout_property_field(
                            title: "耗时",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 90,
                            prop: sp_def_recycle_alpha_type,
                            prop_margin: new RectOffset(0, 0, 5, 0));

                        XGUI.layout_space(5);

                        SerializedProperty sp_def_recycle_alpha_delay = RecycleArgs_Default.FindPropertyRelative("Alpha.Delay");
                        XGUI.layout_property_field(
                            title: "延迟",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 90,
                            prop: sp_def_recycle_alpha_delay,
                            prop_margin: new RectOffset(0, 0, 5, 0));

                        XGUI.layout_space(5);

                        SerializedProperty sp_def_recycle_alpha_curve = RecycleArgs_Default.FindPropertyRelative("Alpha.Curve");
                        XGUI.layout_property_field(
                            title: "曲线",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 90,
                            prop: sp_def_recycle_alpha_curve,
                            prop_margin: new RectOffset(0, 0, 5, 0));

                        XGUI.layout_space(5);

                        SerializedProperty sp_def_recycle_alpha_ease = RecycleArgs_Default.FindPropertyRelative("Alpha.Ease");
                        XGUI.layout_property_field(
                            title: "缓动",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 90,
                            prop: sp_def_recycle_alpha_ease,
                            prop_margin: new RectOffset(0, 0, 5, 0));
                    }
                    #endregion

                    XGUI.editor_layout_group_end(XGUIContainerType.Vertical);

                    #region 动效结束时机
                    XGUI.ChangedCheck_Start();
                    XGUI.layout_property_field(
                        title: "动效结束时机",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 90,
                        prop: RecycleArgs_MotionAnimateEndState,
                        prop_margin: new RectOffset(0, 0, 15, 0));
                    if (XGUI.ChangedCheck_End())
                    {
                        MotionAnimateEndState state = (MotionAnimateEndState)RecycleArgs_MotionAnimateEndState.enumValueIndex;
                        switch (state)
                        {
                            case MotionAnimateEndState.以_移动为准:
                                HudMotion_Movement m = (HudMotion_Movement)RecycleArgs_Default.FindPropertyRelative("Movement.Movement").enumValueIndex;
                                if (m == HudMotion_Movement.A_无运动)
                                {
                                    XGUI.dialog(
                                        type: XGUIDialogType.警告,
                                        windowtitle: "XHud - 管理器消息",
                                        title: "设定动画结束时机",
                                        msg: $"当前位移方式为 <color={hexcol}> A_无运动 </color>，将发生<color={hexcol}> 动效动画无法正常结束</color>从而导致<color={hexcol}> 相应的事件和动作委托 </color>不能正确触发的情况 </color>！请将<color={hexcol}> 位移方式 </color>改为<color={hexcol}> 非无运动方式 </color>！",
                                        ok: "明白",
                                        PrimaryIndex: 0,
                                        usemodal: true,
                                        themecolor: XHud_Dashboard.Theme_Primary);

                                    RecycleArgs_MotionAnimateEndState.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
                                }
                                break;
                            case MotionAnimateEndState.以_旋转为准:
                                HudMotion_Rotation r = (HudMotion_Rotation)RecycleArgs_Default.FindPropertyRelative("Rotation.Rotation").enumValueIndex;
                                if (r == HudMotion_Rotation.A_无旋转)
                                {
                                    XGUI.dialog(
                                        type: XGUIDialogType.警告,
                                        windowtitle: "XHud - 管理器消息",
                                        title: "设定动画结束时机",
                                        msg: $"当前旋转方式为 <color={hexcol}> A_无旋转 </color>，将发生<color={hexcol}> 动效动画无法正常结束</color>从而导致<color={hexcol}> 相应的事件和动作委托 </color>不能正确触发的情况 </color>！请将<color={hexcol}> 旋转方式 </color>改为<color={hexcol}> 非无旋转方式 </color>！",
                                        ok: "明白",
                                        PrimaryIndex: 0,
                                        usemodal: true,
                                        themecolor: XHud_Dashboard.Theme_Primary);

                                    RecycleArgs_MotionAnimateEndState.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
                                }
                                break;
                        }
                    }
                    #endregion
                    #endregion
                    #endregion
                }

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                #endregion

                #region 主题色
                BaseScript.fold_themes = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIFilled.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.Theme_Group,
                    title: "主题色",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.Theme_Primary,
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: BaseScript.fold_themes);

                if (!BaseScript.fold_themes)
                {
                    #region 重置主题色
                    if (XGUI.layout_button(
                        text: "重置主题色",
                        tooltip: "",
                        bg_fill: XGUIFilled.实体,
                        bg_color: XGUIColor.深空灰,
                        bg_color_gui: Color.white,
                        button_text_color: Color.white,
                        press_fill: XGUIFilled.实体,
                        press_color: XGUIColor.深空灰,
                        press_text_color: Color.gray,
                        font_size: XGUIFontSize.M,
                        anchor: TextAnchor.MiddleCenter,
                        margin: new RectOffset(0, 0, 0, 0),
                        padding: new RectOffset(0, 0, 0, 0),
                        height: 25,
                        button_text_font: XGUI.GetFont("xg-medium")))
                    {
                        xHud_ResetThemesColor();
                    }
                    #endregion

                    XGUI.layout_space(10);

                    #region 主题色
                    XGUI.layout_string_popup(
                      title: "主题色",
                      title_width: 60,
                      title_size: XGUIFontSize.M,
                      title_anchor: TextAnchor.MiddleLeft,
                      prop: ThemeSolution,
                      options: ThemeSolutionNames,
                      opt_text_size: XGUIFontSize.M,
                      opt_text_color: Color.black,
                      opt_text_padding: new RectOffset(10, 10, 0, 0),
                      opt_anchor: TextAnchor.MiddleCenter,
                      opt_font_style: FontStyle.Normal,
                      opt_bg_fill: XGUIFilled.实体,
                      opt_bg_color: XGUIColor.亮白,
                      opt_bg_color_gui: XHud_Dashboard.Theme_Primary,
                      icon_arrow_color: Color.black,
                      margin: new RectOffset(0, 0, 5, 5),
                      padding: new RectOffset(5, 5, 0, 0),
                      title_margin: new RectOffset(0, 0, 0, 0),
                      act_on_changed: (value) =>
                      {
                          ThemeSolution.stringValue = value;
                          ThemeSolution.serializedObject.ApplyModifiedProperties();

                          Color col = Color.white;
                          switch (value)
                          {
                              case "白色":
                                  col = new Color(0.8196079f, 0.8196079f, 0.8196079f);
                                  break;
                              case "黑色":
                                  col = new Color(0, 0, 0);
                                  break;
                              case "乳白":
                                  col = new Color(0.8235295f, 0.7882354f, 0.7411765f);
                                  break;
                              case "浅灰":
                                  col = new Color(0.5254902f, 0.5254902f, 0.5254902f);
                                  break;
                              case "沙漠灰":
                                  col = new Color(0.9176471f, 0.8000001f, 0.5843138f);
                                  break;
                              case "科幻青":
                                  col = new Color(0.2313726f, 0.9921569f, 0.6039216f);
                                  break;
                              case "液晶绿":
                                  col = new Color(0.7254902f, 0.7882354f, 0.3568628f);
                                  break;
                              case "橄榄绿":
                                  col = new Color(0.7176471f, 0.8235295f, 0.2039216f);
                                  break;
                              case "湖蓝":
                                  col = new Color(0.2470588f, 0.6352941f, 0.9960785f);
                                  break;
                              case "天蓝":
                                  col = new Color(0.4f, 0.6745098f, 0.854902f);
                                  break;
                              case "胭脂粉":
                                  col = new Color(0.8588236f, 0.4901961f, 0.7294118f);
                                  break;
                              case "灵动粉":
                                  col = new Color(0.882353f, 0.4705883f, 0.5215687f);
                                  break;
                              case "秋叶黄":
                                  col = new Color(0.8352942f, 0.6117647f, 0.3411765f);
                                  break;
                              case "警示黄":
                                  col = new Color(0.9960785f, 0.7686275f, 0);
                                  break;
                              case "高亮橘":
                                  col = new Color(0.9960785f, 0.5176471f, 0.1686275f);
                                  break;
                              case "烈焰红":
                                  col = new Color(0.9960785f, 0.3529412f, 0.2156863f);
                                  break;
                          }

                          #region 更新主题色
                          float h = 0; float s = 0; float v = 0;
                          Color.RGBToHSV(col, out h, out s, out v);
                          theme_color.vector3Value = new Vector3(h, s, v);
                          xHud_UpdateThemeColor(theme_color.vector3Value);

                          if (XGUI.x_Editor_Data_Has_String(PrefsKeyColor_Theme))
                          {
                              Color x = Color.HSVToRGB(h, s, v);
                              XGUI.x_Editor_Data_Set_With_String(PrefsKeyColor_Theme, $"{x.r},{x.g},{x.b}");
                          }
                          #endregion
                      });
                    #endregion

                    #region 边框色
                    XGUI.layout_string_popup(
                    title: "边框色",
                    title_width: 60,
                    title_size: XGUIFontSize.M,
                    title_anchor: TextAnchor.MiddleLeft,
                    prop: ThemeEdgeSolution,
                    options: ThemeEdgeSolutionNames,
                    opt_text_size: XGUIFontSize.M,
                    opt_text_color: Color.black,
                    opt_text_padding: new RectOffset(10, 10, 0, 0),
                    opt_anchor: TextAnchor.MiddleCenter,
                    opt_font_style: FontStyle.Normal,
                    opt_bg_fill: XGUIFilled.实体,
                    opt_bg_color: XGUIColor.亮白,
                    opt_bg_color_gui: XHud_Dashboard.Theme_Primary,
                    icon_arrow_color: Color.black,
                    margin: new RectOffset(0, 0, 5, 5),
                    padding: new RectOffset(5, 5, 0, 0),
                    title_margin: new RectOffset(0, 0, 0, 0),
                    act_on_changed: (value) =>
                    {
                        ThemeEdgeSolution.stringValue = value;
                        ThemeEdgeSolution.serializedObject.ApplyModifiedProperties();

                        Color col = Color.white;
                        switch (value)
                        {
                            case "白色":
                                col = new Color(0.5568628f, 0.5568628f, 0.5568628f);
                                break;
                            case "浅灰":
                                col = new Color(0.4f, 0.4f, 0.4f);
                                break;
                            case "深灰":
                                col = new Color(0.1176471f, 0.1176471f, 0.1176471f);
                                break;
                            case "黑色":
                                col = new Color(0, 0, 0);
                                break;
                            case "极简":
                                col = new Color(0.2196079f, 0.2196079f, 0.2196079f);
                                break;
                            case "珊瑚红":
                                col = new Color(0.5529412f, 0.3058824f, 0.3058824f);
                                break;
                            case "落叶黄":
                                col = new Color(0.5529412f, 0.4470589f, 0.3058824f);
                                break;
                            case "烟灰蓝":
                                col = new Color(0.2980392f, 0.3568628f, 0.427451f);
                                break;
                            case "青苔绿":
                                col = new Color(0.3529412f, 0.4039216f, 0.2470588f);
                                break;
                            case "荧光绿":
                                col = new Color(0.3058824f, 0.427451f, 0.2235294f);
                                break;
                            case "淡粉":
                                col = new Color(0.427451f, 0.3058824f, 0.4078432f);
                                break;
                        }

                        #region 更新主题边框色
                        float h = 0; float s = 0; float v = 0;
                        Color.RGBToHSV(col, out h, out s, out v);
                        theme_color_gp.vector3Value = new Vector3(h, s, v);
                        xHud_UpdateThemeColor_Gp(theme_color_gp.vector3Value);

                        if (XGUI.x_Editor_Data_Has_String(PrefsKeyColor_Theme_GP))
                        {
                            Color x = Color.HSVToRGB(h, s, v);
                            XGUI.x_Editor_Data_Set_With_String(PrefsKeyColor_Theme_GP, $"{x.r},{x.g},{x.b}");
                        }
                        #endregion
                    });
                    #endregion

                    XGUI.layout_space(10);

                    #region Theme_Color
                    // 绘制滑动条基准坐标
                    Rect color_rect = XGUI.GetControlRect(false, 1);

                    //XGUI.gui_box(color_rect, Color.red * 0.2f);

                    XGUI.layout_space(18);

                    SerializedProperty prop_theme_hue = theme_color.FindPropertyRelative("x");
                    SerializedProperty prop_theme_sat = theme_color.FindPropertyRelative("y");
                    SerializedProperty prop_theme_lgt = theme_color.FindPropertyRelative("z");

                    XGUI.ChangedCheck_Start();

                    #region 色调
                    prop_theme_hue.floatValue = XGUI.layout_slider(
                        title: "色调",
                        prop: prop_theme_hue,
                        left: 0,
                        right: 1,
                        title_width: 100,
                        title_size: XGUIFontSize.M,
                        title_anchor: TextAnchor.MiddleLeft,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion

                    #region 饱和
                    prop_theme_sat.floatValue = XGUI.layout_slider(
                        title: "饱和",
                        prop: prop_theme_sat,
                        left: 0,
                        right: 1,
                        title_width: 100,
                        title_size: XGUIFontSize.M,
                        title_anchor: TextAnchor.MiddleLeft,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion

                    #region 明度
                    prop_theme_lgt.floatValue = XGUI.layout_slider(
                        title: "明度",
                        prop: prop_theme_lgt,
                        left: 0,
                        right: 1,
                        title_width: 100,
                        title_size: XGUIFontSize.M,
                        title_anchor: TextAnchor.MiddleLeft,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion

                    theme_color.serializedObject.ApplyModifiedProperties();

                    XGUI.gui_hue_gradient(
                        rect: color_rect,
                        prop: theme_color,
                        mark_color: Color.white,
                        mark: themes_mark,
                        mark_height_offset: -6,
                        mark_size: new Vector2(8, 8));

                    if (XGUI.ChangedCheck_End())
                    {
                        xHud_UpdateThemeColor(theme_color.vector3Value);

                        if (XGUI.x_Editor_Data_Has_String(PrefsKeyColor_Theme))
                        {
                            Color x = Color.HSVToRGB(prop_theme_hue.floatValue, prop_theme_sat.floatValue, prop_theme_lgt.floatValue);
                            XGUI.x_Editor_Data_Set_With_String(PrefsKeyColor_Theme, $"{x.r},{x.g},{x.b}");
                        }
                    }
                    #endregion

                    XGUI.layout_seperator(
                        thickness: 1,
                        color: XHud_Dashboard.Theme_SeperateLine,
                        margin: new RectOffset(15, 15, 15, 15));

                    #region Theme_Color_Gp
                    // 绘制滑动条基准坐标
                    Rect color_rect_gp = XGUI.GetControlRect(false, 1);

                    //XGUI.gui_box(color_rect, Color.red * 0.2f);

                    XGUI.layout_space(18);

                    SerializedProperty theme_color_gp_hue = theme_color_gp.FindPropertyRelative("x");
                    SerializedProperty theme_color_gp_sat = theme_color_gp.FindPropertyRelative("y");
                    SerializedProperty theme_color_gp_lgt = theme_color_gp.FindPropertyRelative("z");

                    XGUI.ChangedCheck_Start();

                    #region 色调
                    theme_color_gp_hue.floatValue = XGUI.layout_slider(
                        title: "色调",
                        prop: theme_color_gp_hue,
                        left: 0,
                        right: 1,
                        title_width: 100,
                        title_size: XGUIFontSize.M,
                        title_anchor: TextAnchor.MiddleLeft,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion

                    #region 饱和
                    theme_color_gp_sat.floatValue = XGUI.layout_slider(
                        title: "饱和",
                        prop: theme_color_gp_sat,
                        left: 0,
                        right: 1,
                        title_width: 100,
                        title_size: XGUIFontSize.M,
                        title_anchor: TextAnchor.MiddleLeft,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion

                    #region 明度
                    theme_color_gp_lgt.floatValue = XGUI.layout_slider(
                        title: "明度",
                        prop: theme_color_gp_lgt,
                        left: 0,
                        right: 1,
                        title_width: 100,
                        title_size: XGUIFontSize.M,
                        title_anchor: TextAnchor.MiddleLeft,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion

                    theme_color_gp.serializedObject.ApplyModifiedProperties();

                    XGUI.gui_hue_gradient(
                        rect: color_rect_gp,
                        prop: theme_color_gp,
                        mark_color: Color.white,
                        mark: themes_mark,
                        mark_height_offset: -6,
                        mark_size: new Vector2(8, 8));

                    if (XGUI.ChangedCheck_End())
                    {
                        xHud_UpdateThemeColor_Gp(theme_color_gp.vector3Value);

                        if (XGUI.x_Editor_Data_Has_String(PrefsKeyColor_Theme_GP))
                        {
                            Color x = Color.HSVToRGB(theme_color_gp_hue.floatValue, theme_color_gp_sat.floatValue, theme_color_gp_lgt.floatValue);
                            XGUI.x_Editor_Data_Set_With_String(PrefsKeyColor_Theme_GP, $"{x.r},{x.g},{x.b}");
                        }
                    }
                    #endregion

                    XGUI.layout_seperator(
                        thickness: 1,
                        color: XHud_Dashboard.Theme_SeperateLine,
                        margin: new RectOffset(15, 15, 15, 15));

                    #region Theme_Color_Sep
                    // 绘制滑动条基准坐标
                    Rect color_rect_sep = XGUI.GetControlRect(false, 1);

                    //XGUI.gui_box(color_rect, Color.red * 0.2f);

                    XGUI.layout_space(18);

                    SerializedProperty theme_color_sep_hue = theme_color_sep.FindPropertyRelative("x");
                    SerializedProperty theme_color_sep_sat = theme_color_sep.FindPropertyRelative("y");
                    SerializedProperty theme_color_sep_lgt = theme_color_sep.FindPropertyRelative("z");

                    XGUI.ChangedCheck_Start();

                    #region 色调
                    theme_color_sep_hue.floatValue = XGUI.layout_slider(
                        title: "色调",
                        prop: theme_color_sep_hue,
                        left: 0,
                        right: 1,
                        title_width: 100,
                        title_size: XGUIFontSize.M,
                        title_anchor: TextAnchor.MiddleLeft,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion

                    #region 饱和
                    theme_color_sep_sat.floatValue = XGUI.layout_slider(
                        title: "饱和",
                        prop: theme_color_sep_sat,
                        left: 0,
                        right: 1,
                        title_width: 100,
                        title_size: XGUIFontSize.M,
                        title_anchor: TextAnchor.MiddleLeft,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion

                    #region 明度
                    theme_color_sep_lgt.floatValue = XGUI.layout_slider(
                        title: "明度",
                        prop: theme_color_sep_lgt,
                        left: 0,
                        right: 1,
                        title_width: 100,
                        title_size: XGUIFontSize.M,
                        title_anchor: TextAnchor.MiddleLeft,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion

                    theme_color_sep.serializedObject.ApplyModifiedProperties();

                    XGUI.gui_hue_gradient(
                        rect: color_rect_sep,
                        prop: theme_color_sep,
                        mark_color: Color.white,
                        mark: themes_mark,
                        mark_height_offset: -6,
                        mark_size: new Vector2(8, 8));

                    if (XGUI.ChangedCheck_End())
                    {
                        xHud_UpdateThemeColor_Sep(theme_color_sep.vector3Value);

                        if (XGUI.x_Editor_Data_Has_String(PrefsKeyColor_Theme_SEP))
                        {
                            Color x = Color.HSVToRGB(theme_color_sep_hue.floatValue, theme_color_sep_sat.floatValue, theme_color_sep_lgt.floatValue);
                            XGUI.x_Editor_Data_Set_With_String(PrefsKeyColor_Theme_SEP, $"{x.r},{x.g},{x.b}");
                        }
                    }
                    #endregion
                }

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
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
            }

            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                // 创建右键菜单
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("F (折叠所有项)"), false, () =>
                {
                    xHud_GroupFolds(true);
                });
                menu.AddItem(new GUIContent("D (展开所有项)"), false, () =>
                {
                    xHud_GroupFolds(false);
                });
                menu.ShowAsContext(); // 在鼠标位置显示右键菜单
                Event.current.Use();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void OnSceneGUI()
        {
            // 用户操作后保持高频 0.8 秒，然后降回低频
            if (EditorApplication.timeSinceStartup - _lastUserActionTime > UPDATE_KEEPTIME)
            {
                UPDATE_INTERVAL = UPDATE_INTERVAL_Low;
            }

            // 频率限制
            if (EditorApplication.timeSinceStartup - _lastSceneUpdate < UPDATE_INTERVAL)
                return;
            _lastSceneUpdate = EditorApplication.timeSinceStartup;

            // 刷新
            xHud_EditorUpdate_HelperVisual();
            xHud_EditorUpdate_Canvas_Camera();
        }

        #region 主题颜色
        /// <summary>
        /// 读取主题颜色
        /// </summary>
        private void xHud_LoadThemesColor()
        {
            string colorval = XGUI.x_Editor_Data_Get_With_String(PrefsKeyColor_Theme);
            Vector3 ori_col = XGUI_Utilitys.String_To_Vector3(colorval);
            Color col = XGUI_Utilitys.RGBA_To_Color(ori_col.x, ori_col.y, ori_col.z, 1, false);
            Color ccc = Color.white;
            Color.RGBToHSV(col, out ccc.r, out ccc.g, out ccc.b);
            theme_color.vector3Value = new Vector3(ccc.r, ccc.g, ccc.b);
            theme_color.serializedObject.ApplyModifiedProperties();
            xHud_UpdateThemeColor(theme_color.vector3Value);

            string colorval_gp = XGUI.x_Editor_Data_Get_With_String(PrefsKeyColor_Theme_GP);
            Vector3 ori_col_gp = XGUI_Utilitys.String_To_Vector3(colorval_gp);
            Color col_gp = XGUI_Utilitys.RGBA_To_Color(ori_col_gp.x, ori_col_gp.y, ori_col_gp.z, 1, false);
            Color ccc_gp = Color.white;
            Color.RGBToHSV(col_gp, out ccc_gp.r, out ccc_gp.g, out ccc_gp.b);
            theme_color_gp.vector3Value = new Vector3(ccc_gp.r, ccc_gp.g, ccc_gp.b);
            theme_color_gp.serializedObject.ApplyModifiedProperties();
            xHud_UpdateThemeColor_Gp(theme_color_gp.vector3Value);


            string colorval_sep = XGUI.x_Editor_Data_Get_With_String(PrefsKeyColor_Theme_SEP);
            Vector3 ori_col_sep = XGUI_Utilitys.String_To_Vector3(colorval_sep);
            Color col_sep = XGUI_Utilitys.RGBA_To_Color(ori_col_sep.x, ori_col_sep.y, ori_col_sep.z, 1, false);
            Color ccc_sep = Color.white;
            Color.RGBToHSV(col_sep, out ccc_sep.r, out ccc_sep.g, out ccc_sep.b);
            theme_color_sep.vector3Value = new Vector3(ccc_sep.r, ccc_sep.g, ccc_sep.b);
            theme_color_sep.serializedObject.ApplyModifiedProperties();
            xHud_UpdateThemeColor_Sep(theme_color_sep.vector3Value);
        }

        /// <summary>
        /// 重置主题颜色
        /// </summary>
        private void xHud_ResetThemesColor()
        {
            EditorPrefs.DeleteKey(PrefsKeyColor_Theme);
            EditorPrefs.DeleteKey(PrefsKeyColor_Theme_GP);
            EditorPrefs.DeleteKey(PrefsKeyColor_Theme_SEP);

            ThemeSolution.stringValue = "默认";
            ThemeEdgeSolution.stringValue = "默认";

            ThemeSolution.serializedObject.ApplyModifiedProperties();
            ThemeEdgeSolution.serializedObject.ApplyModifiedProperties();

            XHud_Dashboard.LoadThemes();

            xHud_LoadThemesColor();
        }

        /// <summary>
        /// 更新主题颜色 - 主颜色
        /// </summary>
        /// <param name="col"></param>
        private void xHud_UpdateThemeColor(Vector3 col)
        {
            XHud_Dashboard.Theme_Primary = Color.HSVToRGB(col.x, col.y, col.z);
        }

        /// <summary>
        /// 更新主题颜色 - 编组色
        /// </summary>
        /// <param name="col"></param>
        private void xHud_UpdateThemeColor_Gp(Vector3 col)
        {
            XHud_Dashboard.Theme_Group = Color.HSVToRGB(col.x, col.y, col.z);
        }

        /// <summary>
        /// 更新主题颜色 - 分割线
        /// </summary>
        /// <param name="col"></param>
        private void xHud_UpdateThemeColor_Sep(Vector3 col)
        {
            XHud_Dashboard.Theme_SeperateLine = Color.HSVToRGB(col.x, col.y, col.z);
        }

        #endregion

        #region 委托事件
        private void EditorApplication_EditorManagerUpdate()
        {
            // 用户操作后保持高频 0.8 秒，然后降回低频
            if (EditorApplication.timeSinceStartup - _lastUserActionTime > UPDATE_KEEPTIME)
            {
                UPDATE_INTERVAL = UPDATE_INTERVAL_Low;
            }

            if (EditorApplication.timeSinceStartup - _lastHierarchyUpdate < UPDATE_INTERVAL)
                return;
            _lastHierarchyUpdate = EditorApplication.timeSinceStartup;

            xHud_EditorUpdate_HelperVisual();
            xHud_EditorUpdate_Canvas_Camera();
        }
        #endregion

        #region 主逻辑更新
        private void xHud_EditorUpdate_HelperVisual()
        {
            if (Application.isPlaying)
                return;

            if (SceneCamera.objectReferenceValue != null && ScreenRes.vector2Value != Vector2.zero && UseSafeFrame.boolValue)
                xHud_HiddenStructureDisplayer(SafeFrameStructureDisplayer.boolValue);

            BaseScript.hm_Layout_Update();

            BaseScript.hm_SafeFrameUpdate();

            BaseScript.hm_TransitionTopView();
        }
        private void xHud_EditorUpdate_Canvas_Camera()
        {
            if (Application.isPlaying)
                return;

            Canvas canvas_s = (Canvas)HudCanvas_Screen.objectReferenceValue;
            if (canvas_s != null)
            {
                canvas_s.renderMode = RenderMode.ScreenSpaceCamera;

                Camera cam = (Camera)HudCamera.objectReferenceValue;
                if (cam != null)
                {
                    canvas_s.worldCamera = cam;
                }
            }

            Canvas canvas_w = (Canvas)HudCanvas_World.objectReferenceValue;
            if (canvas_w != null)
            {
                canvas_w.renderMode = RenderMode.WorldSpace;

                Camera cam = (Camera)SceneCamera.objectReferenceValue;
                if (cam != null)
                {
                    canvas_w.worldCamera = cam;
                }
            }
        }
        private void xHud_EditorUpdate_CameraArgs()
        {
            BaseScript.hm_CameraCutterRange(BaseScript.CameraCutter_Near, BaseScript.CameraCutter_Far);
            BaseScript.hm_CameraOrthographicProjection(CameraOthograpicMode.boolValue);
            BaseScript.hm_CameraOrthographicSize(BaseScript.CameraOrthographicSize);
            BaseScript.hm_CameraPerspectiveFov(BaseScript.CameraFov);
        }
        private void xHud_EditorUpdate_Mask()
        {
            BaseScript.hm_MaskUpdate();
        }
        private void xHud_EditorUpdate_ContentOpacity()
        {
            BaseScript.hm_Screen_ContentOpacity_Update();
            BaseScript.hm_World_ContentOpacity_Update();
        }
        #endregion

        #region 面板折叠
        /// <summary>
        /// 面板折叠键值设置
        /// </summary>
        /// <param name="state"></param>
        private void xHud_GroupFolds(bool state)
        {
            BaseScript.fold_options = state;
            BaseScript.fold_canvasset = state;
            BaseScript.fold_camera = state;
            BaseScript.fold_ratiorefer = state;
            BaseScript.fold_compguid = state;
            BaseScript.fold_reslibs = state;
            BaseScript.fold_elementlibs = state;
            BaseScript.fold_soundslib = state;
            BaseScript.fold_masks = state;
            BaseScript.fold_global = state;
            BaseScript.fold_tool = state;
            BaseScript.fold_RMS = state;
            BaseScript.fold_layout = state;
            BaseScript.fold_visualsafe = state;
            BaseScript.fold_blueprint = state;
            BaseScript.fold_based = state;
            BaseScript.fold_motion_default = state;
            BaseScript.fold_themes = state;

            Repaint();
        }
        #endregion

        #region 序列化属性 / 字段
        /// <summary>
        /// 序列化变量
        /// </summary>
        private SerializedProperty xHud_GetSerializedProperty(string name)
        {
            return serializedObject.FindProperty(name);
        }
        /// <summary>
        /// 获取序列化变量
        /// </summary>
        private void xHud_SerializedAllVariables()
        {
            IsInitialized = xHud_GetSerializedProperty("IsInitialized");
            HudCamera = xHud_GetSerializedProperty("HudCamera");
            HudCanvas_Screen = xHud_GetSerializedProperty("HudCanvas_Screen");
            HudCanvasScaler = xHud_GetSerializedProperty("HudCanvasScaler");
            Mask = xHud_GetSerializedProperty("Mask");
            HudCanvasGroup_Screen = xHud_GetSerializedProperty("HudCanvasGroup_Screen");
            HudCanvas_ScreenAnchor = xHud_GetSerializedProperty("HudCanvas_ScreenAnchor");
            Anchors_Layout_Screen = xHud_GetSerializedProperty("Anchors_Layout_Screen");
            Lib_ElementLibrarys = xHud_GetSerializedProperty("Hud_ElementLibrarys");
            Lib_Color = xHud_GetSerializedProperty("Hud_Colors");
            Lib_Curve = xHud_GetSerializedProperty("Hud_Curves");
            Lib_Sound = xHud_GetSerializedProperty("Hud_Sounds");
            Lib_TextStyleLibrary = xHud_GetSerializedProperty("Hud_TextStyleLibrary");
            Lib_Motions = xHud_GetSerializedProperty("Hud_Motions");
            Lib_Transition = xHud_GetSerializedProperty("Hud_TransitionLib");
            SoundLibrary = xHud_GetSerializedProperty("Pool_Sounder");
            SounderPoolCount = xHud_GetSerializedProperty("SounderPoolCount");
            UseInstanceMode = xHud_GetSerializedProperty("UseInstanceMode");
            UseDebug = xHud_GetSerializedProperty("UseDebug");
            UseSafeFrame = xHud_GetSerializedProperty("UseSafeFrame");
            UsePerfectPixelUpdate = xHud_GetSerializedProperty("UsePerfectPixelUpdate");
            UseAutoPerfectPixel = xHud_GetSerializedProperty("UseAutoPerfectPixel");
            SafeFrameStructureDisplayer = xHud_GetSerializedProperty("SafeFrameStructureDisplayer");
            CameraOthograpicMode = xHud_GetSerializedProperty("CameraOthograpicMode");
            CanvasScalerModeIndex = xHud_GetSerializedProperty("CanvasScalerModeIndex");
            CanvasScalerScreenSize = xHud_GetSerializedProperty("CanvasScalerScreenSize");
            CanvasMatchDir = xHud_GetSerializedProperty("CanvasMatchDir");
            SupportWorldUI = xHud_GetSerializedProperty("SupportWorldUI");
            BluePrintMode = xHud_GetSerializedProperty("BluePrintMode");
            Safe_Frame = xHud_GetSerializedProperty("Safe_Frame");
            Margins = xHud_GetSerializedProperty("Margins");
            MarginHorizontal = xHud_GetSerializedProperty("MarginHorizontal");
            MarginMultiply = xHud_GetSerializedProperty("MarginMultiply");
            MarginVertical = xHud_GetSerializedProperty("MarginVertical");
            MarkSize = xHud_GetSerializedProperty("MarkSize");
            Color_LayoutAnchorMark = xHud_GetSerializedProperty("Color_LayoutAnchorMark");
            Color_FrameLine = xHud_GetSerializedProperty("Color_FrameLine");
            ScreenRes = xHud_GetSerializedProperty("ScreenRes");
            Safe_FrameLine_Width = xHud_GetSerializedProperty("Safe_FrameLine_Width");
            Safe_FrameLine_Margins = xHud_GetSerializedProperty("Safe_FrameLine_Margins");
            Color_SeperaterLine = xHud_GetSerializedProperty("Color_SeperaterLine");
            Safe_Seperater_Length = xHud_GetSerializedProperty("Safe_Seperater_Length");
            Safe_CenterMarkLength = xHud_GetSerializedProperty("Safe_CenterMarkLength");
            Safe_CenterMarkWidth = xHud_GetSerializedProperty("Safe_CenterMarkWidth");
            Color_CenterMark = xHud_GetSerializedProperty("Color_CenterMark");
            Safe_CenterMarkDistance = xHud_GetSerializedProperty("Safe_CenterMarkDistance");
            CameraOrthographicSize = xHud_GetSerializedProperty("CameraOrthographicSize");
            CameraFov = xHud_GetSerializedProperty("CameraFov");
            CameraCutter_Near = xHud_GetSerializedProperty("CameraCutter_Near");
            CameraCutter_Far = xHud_GetSerializedProperty("CameraCutter_Far");
            HudCanvasAnchor = xHud_GetSerializedProperty("HudCanvasAnchor");
            CanvasDistance = xHud_GetSerializedProperty("CanvasDistance");
            HudCanvasAnchorIndex = xHud_GetSerializedProperty("HudCanvasAnchorIndex");
            MaskAlpha = xHud_GetSerializedProperty("MaskAlpha");
            MaskTexture = xHud_GetSerializedProperty("MaskTexture");
            MaskRaycastAlphaThreshold = xHud_GetSerializedProperty("MaskRaycastAlphaThreshold");
            MaskColor = xHud_GetSerializedProperty("MaskColor");
            ContentOpacity_Screen = xHud_GetSerializedProperty("ContentOpacity_Screen");
            ContentOpacity_World = xHud_GetSerializedProperty("ContentOpacity_World");
            DurationMultiply = xHud_GetSerializedProperty("DurationMultiply");
            CustomCursor = xHud_GetSerializedProperty("CustomCursor");
            CustomTransition = xHud_GetSerializedProperty("CustomTransition");
            Hud_MouseCursor = xHud_GetSerializedProperty("Hud_MouseCursor");
            Hud_TransitionController = xHud_GetSerializedProperty("Hud_TransitionController");
            SceneCamera = xHud_GetSerializedProperty("SceneCamera");
            MaskRaycastEnabled = xHud_GetSerializedProperty("MaskRaycastEnabled");
            Volume = xHud_GetSerializedProperty("Volume");
            VolumeMute = xHud_GetSerializedProperty("VolumeMute");
            HudCanvas_World = xHud_GetSerializedProperty("HudCanvas_World");
            HudCanvas_WorldAnchor = xHud_GetSerializedProperty("HudCanvas_WorldAnchor");
            HudCanvasGroup_World = xHud_GetSerializedProperty("HudCanvasGroup_World");
            RecycleArgs_Default = xHud_GetSerializedProperty("RecycleArgs_Default");
            CreateArgs_Default = xHud_GetSerializedProperty("CreateArgs_Default");
            FontSizeMultiply = xHud_GetSerializedProperty("FontSizeMultiply");
            BluePrint_root = xHud_GetSerializedProperty("BluePrint_root");
            BluePrint_grid_size = xHud_GetSerializedProperty("BluePrint_grid_size");
            BluePrint_linewidth = xHud_GetSerializedProperty("BluePrint_linewidth");
            BluePrint_grid_color = xHud_GetSerializedProperty("BluePrint_grid_color");
            BluePrint_bg_color = xHud_GetSerializedProperty("BluePrint_bg_color");
            BluePrint_bg_decal_color = xHud_GetSerializedProperty("BluePrint_bg_decal_color");
            BluePrint_marktitle_color = xHud_GetSerializedProperty("BluePrint_marktitle_color");
            BluePrint_marksubtitle_color = xHud_GetSerializedProperty("BluePrint_marksubtitle_color");
            BluePrint_mark_size = xHud_GetSerializedProperty("BluePrint_mark_size");
            BluePrint_mark_opacity = xHud_GetSerializedProperty("BluePrint_mark_opacity");
            BluePrint_mark_margin = xHud_GetSerializedProperty("BluePrint_mark_margin");
            BluePrint_mark_space = xHud_GetSerializedProperty("BluePrint_mark_space");
            BluePrint_title_content = xHud_GetSerializedProperty("BluePrint_title_content");
            BluePrint_subtitle_content = xHud_GetSerializedProperty("BluePrint_subtitle_content");
            BluePrint_MarkAnchors = xHud_GetSerializedProperty("BluePrint_MarkAnchors");
            BluePrint_opacity = xHud_GetSerializedProperty("BluePrint_opacity");
            BluePrint_Grid_AnimationEase = xHud_GetSerializedProperty("BluePrint_Grid_AnimationEase");
            BluePrint_Bg_AnimationEase_In = xHud_GetSerializedProperty("BluePrint_Bg_AnimationEase_In");
            BluePrint_Bg_AnimationEase_Out = xHud_GetSerializedProperty("BluePrint_Bg_AnimationEase_Out");
            BluePrint_Grid_LengthPercentage = xHud_GetSerializedProperty("BluePrint_Grid_LengthPercentage");
            BluePrint_Grid_LevelHeight = xHud_GetSerializedProperty("BluePrint_Grid_LevelHeight");
            BluePrint_AnimationDuration = xHud_GetSerializedProperty("BluePrint_AnimationDuration");
            BluePrint_Displayed = xHud_GetSerializedProperty("BluePrint_Displayed");
            BluePrint_OnStartHide = xHud_GetSerializedProperty("BluePrint_OnStartHide");
            BluePrint_bg_opacity = xHud_GetSerializedProperty("BluePrint_bg_opacity");
            BluePrint_bg_mapOpacity = xHud_GetSerializedProperty("BluePrint_bg_mapOpacity");
            BluePrint_Bg_FadeAnimationDelay = xHud_GetSerializedProperty("BluePrint_Bg_FadeAnimationDelay");
            BluePrint_GridEnd = xHud_GetSerializedProperty("BluePrint_GridEnd");
            BluePrint_bg_tilling = xHud_GetSerializedProperty("BluePrint_bg_tilling");
            BluePrint_bg_name = xHud_GetSerializedProperty("BluePrint_bg_name");
            BluePrint_bg_usetilling_index = xHud_GetSerializedProperty("BluePrint_bg_usetilling_index");
            BluePrint_bg_usesquareratio_index = xHud_GetSerializedProperty("BluePrint_bg_usesquareratio_index");
            BluePrint_grid_Opacity = xHud_GetSerializedProperty("BluePrint_grid_Opacity");
            BluePrint_Grid_AnimationDuration = xHud_GetSerializedProperty("BluePrint_Grid_AnimationDuration");
            Eft_Grid = xHud_GetSerializedProperty("Eft_Grid");
            Eft_GridFade = xHud_GetSerializedProperty("Eft_GridFade");
            Eft_Bg = xHud_GetSerializedProperty("Eft_Bg");
            Eft_Mark = xHud_GetSerializedProperty("Eft_Mark");
            RMS_Enabled = xHud_GetSerializedProperty("RMS_Enabled");
            RMS_Nodes = xHud_GetSerializedProperty("RMS_Nodes");
            RMS_CurrentSolution = xHud_GetSerializedProperty("RMS_CurrentSolution");
            theme_color = xHud_GetSerializedProperty("theme_color");
            theme_color_gp = xHud_GetSerializedProperty("theme_color_gp");
            theme_color_sep = xHud_GetSerializedProperty("theme_color_sep");
            ThemeSolution = xHud_GetSerializedProperty("ThemeSolution");
            ThemeEdgeSolution = xHud_GetSerializedProperty("ThemeEdgeSolution");
            Hud_EventSystem = xHud_GetSerializedProperty("Hud_EventSystem");
            Hud_InputSystemUIInputModule = xHud_GetSerializedProperty("Hud_InputSystemUIInputModule");
            CreateArgs_MotionAnimateEndState = CreateArgs_Default.FindPropertyRelative("MotionAnimateEndState");
            RecycleArgs_MotionAnimateEndState = RecycleArgs_Default.FindPropertyRelative("MotionAnimateEndState");
            sp_PhysicsScreenSize = xHud_GetSerializedProperty("PhysicsScreenSize");
            sp_Reference_Image = xHud_GetSerializedProperty("Reference_Image");
            UseRatioReference = xHud_GetSerializedProperty("UseRatioReference");
            RatioReferenceIsPart = xHud_GetSerializedProperty("RatioReferenceIsPart");
            sp_Reference_Image_Color = xHud_GetSerializedProperty("Reference_Image_Color");
            sp_Res_Full = xHud_GetSerializedProperty("Res_Full");
            sp_Res_Part = xHud_GetSerializedProperty("Res_Part");
            sp_ReferShape_RatioSize = xHud_GetSerializedProperty("ReferShape_RatioSize");
            sp_ReferShape_RatioTolerance = xHud_GetSerializedProperty("ReferShape_RatioTolerance");
            FoldAllPanelWithDisabled = xHud_GetSerializedProperty("FoldAllPanelWithDisabled");
            CompGuide_Anchors = xHud_GetSerializedProperty("CompGuide_Anchors");
            CompGuide_AnchorRoot = xHud_GetSerializedProperty("CompGuide_AnchorRoot");

            UseCompGuide = xHud_GetSerializedProperty("UseCompGuide");
            CompGuideMode = xHud_GetSerializedProperty("CompGuideMode");
            GuideColor = xHud_GetSerializedProperty("GuideColor");
            GuidePointColor = xHud_GetSerializedProperty("GuidePointColor");

            GuideParam_Mirror_LR_Offset = xHud_GetSerializedProperty("GuideParam_Mirror_LR_Offset");
            GuideParam_Mirror_LR_GoldenMode = xHud_GetSerializedProperty("GuideParam_Mirror_LR_GoldenMode");

            GuideParam_Mirror_UD_Offset = xHud_GetSerializedProperty("GuideParam_Mirror_UD_Offset");
            GuideParam_Mirror_UD_GoldenMode = xHud_GetSerializedProperty("GuideParam_Mirror_UD_GoldenMode");
            GuideParam_Fibonacci_Mode = xHud_GetSerializedProperty("GuideParam_Fibonacci_Mode");
            GuideParam_CornerLookat_Offset_H = xHud_GetSerializedProperty("GuideParam_CornerLookat_Offset_H");
            GuideParam_CornerLookat_Offset_V = xHud_GetSerializedProperty("GuideParam_CornerLookat_Offset_V");

            GuideParam_Three_Offset_H = xHud_GetSerializedProperty("GuideParam_Three_Offset_H");
            GuideParam_Three_Offset_V = xHud_GetSerializedProperty("GuideParam_Three_Offset_V");
            GuideParam_Three_Offset_Coverage = xHud_GetSerializedProperty("GuideParam_Three_Offset_Coverage");

            GuideParam_GuideLine_BaseOffset = xHud_GetSerializedProperty("GuideParam_GuideLine_BaseOffset");
            GuideParam_GuideLine_BaseHeight = xHud_GetSerializedProperty("GuideParam_GuideLine_BaseHeight");
            GuideParam_GuideLine_Offset_Near = xHud_GetSerializedProperty("GuideParam_GuideLine_Offset_Near");
            GuideParam_GuideLine_Offset_Far = xHud_GetSerializedProperty("GuideParam_GuideLine_Offset_Far");
            GuideParam_GuideLine_Offset_NearHeight = xHud_GetSerializedProperty("GuideParam_GuideLine_Offset_NearHeight");

            GuideParam_Triangle_TopOffset = xHud_GetSerializedProperty("GuideParam_Triangle_TopOffset");
            GuideParam_Triangle_BaseHeight = xHud_GetSerializedProperty("GuideParam_Triangle_BaseHeight");
            GuideParam_Triangle_BottomHeight = xHud_GetSerializedProperty("GuideParam_Triangle_BottomHeight");
            GuideParam_Triangle_Offset_Left = xHud_GetSerializedProperty("GuideParam_Triangle_Offset_Left");
            GuideParam_Triangle_Offset_Right = xHud_GetSerializedProperty("GuideParam_Triangle_Offset_Right");
            GuideParam_CenterPointSize = xHud_GetSerializedProperty("GuideParam_CenterPointSize");

            GuideParam_IShape_TopHeight = xHud_GetSerializedProperty("GuideParam_IShape_TopHeight");
            GuideParam_IShape_BottomHeight = xHud_GetSerializedProperty("GuideParam_IShape_BottomHeight");
            GuideParam_IShape_Offset_Left = xHud_GetSerializedProperty("GuideParam_IShape_Offset_Left");
            GuideParam_IShape_Offset_Right = xHud_GetSerializedProperty("GuideParam_IShape_Offset_Right");

            Crc_Lib_Name = xHud_GetSerializedProperty("Crc_Lib_Name");
            Rec_Lib_Name = xHud_GetSerializedProperty("Rec_Lib_Name");

            ElementVisualPlacer = xHud_GetSerializedProperty("ElementVisualPlacer");
            PrimtiveID_LabelLine_Height = xHud_GetSerializedProperty("PrimtiveID_LabelLine_Height");
            PrimtiveID_LabelLine_Color = xHud_GetSerializedProperty("PrimtiveID_LabelLine_Color");
            PrimtiveID_LabelFont_Size = xHud_GetSerializedProperty("PrimtiveID_LabelFont_Size");
            PrimtiveID_LabelFont_Color = xHud_GetSerializedProperty("PrimtiveID_LabelFont_Color");

        }
        #endregion

        #region 可视化辅助
        /// <summary>
        /// 初始化布局辅助线的结构
        /// </summary>
        private void xHud_CompGuide_Structure_Create()
        {
            ///-------创建 HudAuxiliary_Anchor
            GameObject obj_Anchors = new GameObject();
            obj_Anchors.layer = LayerMask.NameToLayer("XHud");
            UnityEngine.RectTransform Anchors = obj_Anchors.AddComponent<UnityEngine.RectTransform>();
            CanvasGroup cavgroup = obj_Anchors.AddComponent<CanvasGroup>();
            HudCanvasGroup_Screen.objectReferenceValue = cavgroup;
            HudCanvasGroup_Screen.serializedObject.ApplyModifiedProperties();
            cavgroup.alpha = 1;
            Anchors.name = "CompGuide";
            Anchors.SetParent(BaseScript.HudCanvas_Screen.transform);
            Anchors.localPosition = Vector3.zero;
            Anchors.localScale = Vector3.one;
            Anchors.anchorMin = new Vector2(0, 0);
            Anchors.anchorMax = new Vector2(1, 1);
            Anchors.sizeDelta = new Vector2(0, 0);

            CompGuide_AnchorRoot.objectReferenceValue = Anchors;
            CompGuide_AnchorRoot.serializedObject.ApplyModifiedProperties();

            for (int i = 0; i < 9; i++)
            {
                GameObject obj_Layanchor = new GameObject();
                obj_Layanchor.layer = LayerMask.NameToLayer("XHud");
                UnityEngine.RectTransform layanchor = obj_Layanchor.AddComponent<UnityEngine.RectTransform>();
                layanchor.SetParent(Anchors);
                if (i == 0)
                {
                    layanchor.name = "Up";
                    layanchor.anchorMin = new Vector2(0.5f, 1);
                    layanchor.anchorMax = new Vector2(0.5f, 1);
                    layanchor.pivot = new Vector2(0.5f, 1);
                    layanchor.sizeDelta = new Vector2(20, 20);
                }
                if (i == 1)
                {
                    layanchor.name = "Down";
                    layanchor.anchorMin = new Vector2(0.5f, 0);
                    layanchor.anchorMax = new Vector2(0.5f, 0);
                    layanchor.pivot = new Vector2(0.5f, 0);
                    layanchor.sizeDelta = new Vector2(20, 20);
                }
                if (i == 2)
                {
                    layanchor.name = "Left";
                    layanchor.anchorMin = new Vector2(0, 0.5f);
                    layanchor.anchorMax = new Vector2(0, 0.5f);
                    layanchor.pivot = new Vector2(0, 0.5f);
                    layanchor.sizeDelta = new Vector2(20, 20);
                }
                if (i == 3)
                {
                    layanchor.name = "Right";
                    layanchor.anchorMin = new Vector2(1, 0.5f);
                    layanchor.anchorMax = new Vector2(1, 0.5f);
                    layanchor.pivot = new Vector2(1, 0.5f);
                    layanchor.sizeDelta = new Vector2(20, 20);
                }
                if (i == 4)
                {
                    layanchor.name = "Center";
                    layanchor.anchorMin = new Vector2(0.5f, 0.5f);
                    layanchor.anchorMax = new Vector2(0.5f, 0.5f);
                    layanchor.pivot = new Vector2(0.5f, 0.5f);
                    layanchor.sizeDelta = new Vector2(20, 20);
                }
                if (i == 5)
                {
                    layanchor.name = "Left_Up";
                    layanchor.anchorMin = new Vector2(0, 1);
                    layanchor.anchorMax = new Vector2(0, 1);
                    layanchor.pivot = new Vector2(0, 1);
                    layanchor.sizeDelta = new Vector2(20, 20);
                }
                if (i == 6)
                {
                    layanchor.name = "Left_Down";
                    layanchor.anchorMin = new Vector2(0, 0);
                    layanchor.anchorMax = new Vector2(0, 0);
                    layanchor.pivot = new Vector2(0, 0);
                    layanchor.sizeDelta = new Vector2(20, 20);
                }
                if (i == 7)
                {
                    layanchor.name = "Right_Up";
                    layanchor.anchorMin = new Vector2(1, 1);
                    layanchor.anchorMax = new Vector2(1, 1);
                    layanchor.pivot = new Vector2(1, 1);
                    layanchor.sizeDelta = new Vector2(20, 20);
                }
                if (i == 8)
                {
                    layanchor.name = "Right_Down";
                    layanchor.anchorMin = new Vector2(1, 0);
                    layanchor.anchorMax = new Vector2(1, 0);
                    layanchor.pivot = new Vector2(1, 0);
                    layanchor.sizeDelta = new Vector2(20, 20);
                }

                layanchor.anchoredPosition3D = Vector3.zero;
                layanchor.localScale = Vector3.one;
            }

            #region 获取所有锚点结构 - 布局
            if (CompGuide_Anchors != null && CompGuide_AnchorRoot != null)
            {
                Transform anchorRoot = CompGuide_AnchorRoot.objectReferenceValue as Transform;
                for (int i = 0; i < anchorRoot.childCount; i++)
                {
                    UnityEngine.RectTransform obj = anchorRoot.GetChild(i).GetComponent<UnityEngine.RectTransform>();
                    CompGuide_Anchors.InsertArrayElementAtIndex(CompGuide_Anchors.arraySize);
                    CompGuide_Anchors.GetArrayElementAtIndex(i).FindPropertyRelative("Anchor").objectReferenceValue = obj;
                    CompGuide_Anchors.GetArrayElementAtIndex(i).FindPropertyRelative("Name").stringValue = obj.name;
                    string Type = anchorRoot.GetChild(i).name;
                    switch (Type)
                    {
                        case "Up":
                            CompGuide_Anchors.GetArrayElementAtIndex(i).FindPropertyRelative("Type").enumValueIndex = 0;
                            break;
                        case "Down":
                            CompGuide_Anchors.GetArrayElementAtIndex(i).FindPropertyRelative("Type").enumValueIndex = 1;
                            break;
                        case "Left":
                            CompGuide_Anchors.GetArrayElementAtIndex(i).FindPropertyRelative("Type").enumValueIndex = 2;
                            break;
                        case "Right":
                            CompGuide_Anchors.GetArrayElementAtIndex(i).FindPropertyRelative("Type").enumValueIndex = 3;
                            break;
                        case "Center":
                            CompGuide_Anchors.GetArrayElementAtIndex(i).FindPropertyRelative("Type").enumValueIndex = 4;
                            break;
                        case "Left_Up":
                            CompGuide_Anchors.GetArrayElementAtIndex(i).FindPropertyRelative("Type").enumValueIndex = 5;
                            break;
                        case "Left_Down":
                            CompGuide_Anchors.GetArrayElementAtIndex(i).FindPropertyRelative("Type").enumValueIndex = 6;
                            break;
                        case "Right_Up":
                            CompGuide_Anchors.GetArrayElementAtIndex(i).FindPropertyRelative("Type").enumValueIndex = 7;
                            break;
                        case "Right_Down":
                            CompGuide_Anchors.GetArrayElementAtIndex(i).FindPropertyRelative("Type").enumValueIndex = 8;
                            break;
                    }

                    Anchors_Layout_Screen.serializedObject.ApplyModifiedProperties();
                }
            }

            #endregion
        }
        /// <summary>
        /// 移除布局辅助线的结构
        /// </summary>
        private void xHud_CompGuide_Structure_Destroy()
        {
            CompGuide_Anchors.ClearArray();
            CompGuide_Anchors.serializedObject.ApplyModifiedProperties();

            UnityEngine.RectTransform obj_auxiliary = CompGuide_AnchorRoot.objectReferenceValue as UnityEngine.RectTransform;
            DestroyImmediate(obj_auxiliary.gameObject, true);
            CompGuide_AnchorRoot.objectReferenceValue = null;
            CompGuide_AnchorRoot.serializedObject.ApplyModifiedProperties();
        }
        /// <summary>
        /// 物体结构显示
        /// </summary>
        /// <param name="state"></param>
        private void xHud_HiddenStructureDisplayer(bool state)
        {
            Transform SafeFrame = (Transform)Safe_Frame.objectReferenceValue;
            Transform BluePrintRoot = (Transform)BluePrint_root.objectReferenceValue;
            if (!state)
            {
                if (SafeFrame != null)
                    SafeFrame.gameObject.hideFlags = HideFlags.HideInHierarchy;
                if (BluePrintRoot != null)
                    BluePrintRoot.gameObject.hideFlags = HideFlags.HideInHierarchy;

                if (Anchors_Layout_Screen != null && Anchors_Layout_Screen.arraySize > 0)
                    for (int i = 0; i < Anchors_Layout_Screen.arraySize; i++)
                    {
                        SerializedProperty sp_anchorlay = Anchors_Layout_Screen.GetArrayElementAtIndex(i);
                        if (sp_anchorlay != null)
                        {
                            Image obj_anchorlay = (Image)sp_anchorlay.FindPropertyRelative("Mark").objectReferenceValue;
                            //sp_DebugMode.Log(obj_anchorlay);
                            if (obj_anchorlay != null)
                            {
                                obj_anchorlay.gameObject.hideFlags = HideFlags.HideInHierarchy;
                            }
                        }
                    }
            }
            else
            {
                if (SafeFrame != null)
                    SafeFrame.gameObject.hideFlags = HideFlags.None;
                if (BluePrintRoot != null)
                    BluePrintRoot.gameObject.hideFlags = HideFlags.None;

                if (Anchors_Layout_Screen != null && Anchors_Layout_Screen.arraySize > 0)
                    for (int i = 0; i < Anchors_Layout_Screen.arraySize; i++)
                    {
                        Image obj_anchorlay = (Image)Anchors_Layout_Screen.GetArrayElementAtIndex(i).FindPropertyRelative("Mark").objectReferenceValue;

                        if (obj_anchorlay != null)
                        {
                            obj_anchorlay.gameObject.hideFlags = HideFlags.None;
                        }
                    }
            }
        }
        #endregion

        #region 管理器结构初始化 / 重建
        /// <summary>
        /// 初始化HudManager的结构
        /// </summary>
        private void xHud_StructureInitialize()
        {
            RenderPipelineAsset currentPipelineAsset = GraphicsSettings.currentRenderPipeline;
            if (currentPipelineAsset == null)
            {
                XGUI.dialog(
                    type: XGUIDialogType.警告,
                    windowtitle: "XHud - 管理器消息",
                    title: "渲染管线异常",
                    msg: "请先指定URP渲染管线资源后再试！",
                    ok: "明白",
                    PrimaryIndex: 0,
                    usemodal: true,
                    themecolor: XHud_Dashboard.Theme_Primary);
                return;
            }

            string[] renderlist = null;

            // 使用反射获取 m_RendererDataList
            FieldInfo fieldInfo = typeof(UniversalRenderPipelineAsset).GetField("m_RendererDataList", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo != null)
            {
                ScriptableRendererData[] rendererDataList = (ScriptableRendererData[])fieldInfo.GetValue(currentPipelineAsset);
                renderlist = new string[rendererDataList.Length];
                for (int i = 0; i < renderlist.Length; i++)
                {
                    renderlist[i] = rendererDataList[i].name;
                }
            }
            else
            {
                XGUI.dialog(
                    type: XGUIDialogType.警告,
                    windowtitle: "XHud - 管理器消息",
                    title: "渲染管线异常",
                    msg: "请确认当前渲染管线资源文件的渲染器列表的有效性！",
                    ok: "明白",
                    PrimaryIndex: 0,
                    usemodal: true,
                    themecolor: XHud_Dashboard.Theme_Primary);
            }

            EditorApplication.delayCall += () =>
            {
                if (IsInitialized.boolValue)
                {
                    string repeat = XGUI.dialog(
                       type: XGUIDialogType.警告,
                       windowtitle: "XHud - 管理器消息",
                       title: "初始化XHud管理器",
                       msg: "您已经初始化了该XHud管理器，是否需要对其重新进行初始化？（我们并不推荐您这样做）",
                       ok: "覆盖",
                       cancel: "暂不",
                       PrimaryIndex: 0,
                       usemodal: true,
                       themecolor: XHud_Dashboard.Theme_Primary,
                       on_selected: (d) =>
                       {
                       });

                    if (repeat == "暂不")
                    {
                        return;
                    }
                }

                string res = XGUI.dialog(
                    type: XGUIDialogType.警告,
                    windowtitle: "XHud - 管理器消息",
                    title: "初始化XHud管理器",
                    msg: "接下来会自动创建HudManager的所有子结构组成，在这之前会先清空所有现存的子物体，请确定是否继续该操作？",
                    ok: "初始化",
                    cancel: "暂不",
                    PrimaryIndex: 0,
                    usemodal: true,
                    themecolor: XHud_Dashboard.Theme_Primary,
                    on_selected: (d) =>
                    {
                        if (d == "初始化")
                        {
                            #region 重建Manager的结构

                            #region  清空并重置所有组件
                            Anchors_Layout_Screen.ClearArray();
                            Anchors_Layout_Screen.serializedObject.ApplyModifiedProperties();

                            foreach (Transform child in BaseScript.transform)
                            {
                                DestroyImmediate(child.gameObject, true);
                            }

                            GameObject[] objs = new GameObject[BaseScript.transform.childCount];
                            for (int i = 0; i < objs.Length; i++)
                            {
                                objs[i] = BaseScript.transform.GetChild(i).gameObject;
                                objs[i].layer = LayerMask.NameToLayer("XHud");
                            }
                            for (int i = 0; i < objs.Length; i++)
                            {
                                DestroyImmediate(objs[i], true);
                            }

                            BaseScript.Anchors_Layout_Screen = null;
                            BaseScript.Safe_FrameLine = null;
                            BaseScript.Safe_Seperater = null;
                            BaseScript.Safe_CenterMarks = null;

                            BaseScript.gameObject.name = "XHud";
                            BaseScript.Safe_Frame = null;
                            BaseScript.XTweenManager = null;
                            #endregion

                            #region Cam
                            GameObject obj_cam = new GameObject();
                            obj_cam.layer = LayerMask.NameToLayer("XHud");
                            Camera cam = obj_cam.AddComponent<Camera>();
                            cam.name = "Cam";
                            var uac = cam.gameObject.AddComponent<UniversalAdditionalCameraData>();
                            uac.renderPostProcessing = false;
                            cam.cullingMask = (1 << LayerMask.NameToLayer("XHud"));
                            UniversalAdditionalCameraData toOverlayData = cam.GetUniversalAdditionalCameraData();
                            toOverlayData.renderType = CameraRenderType.Overlay;

                            ///---设置相机渲染器
                            for (int i = 0; i < renderlist.Length; i++)
                            {
                                if (renderlist[i] == "Render_XHud")
                                {
                                    toOverlayData.SetRenderer(i);
                                }
                            }
                            cam.orthographic = CameraOthograpicMode.boolValue;
                            cam.clearFlags = CameraClearFlags.SolidColor;
                            cam.transform.SetParent(BaseScript.transform);
                            cam.transform.localPosition = Vector3.zero;
                            HudCamera.objectReferenceValue = cam;
                            HudCamera.serializedObject.ApplyModifiedProperties();
                            #endregion

                            #region Canvas-Screen
                            GameObject obj_cav = new GameObject();
                            obj_cav.layer = LayerMask.NameToLayer("XHud");
                            Canvas cav = obj_cav.AddComponent<Canvas>();
                            cav.renderMode = RenderMode.ScreenSpaceCamera;
                            cav.worldCamera = cam;
                            obj_cav.name = "Screen";
                            obj_cav.transform.SetParent(BaseScript.transform);
                            obj_cav.transform.localPosition = Vector3.zero;
                            CanvasScaler scaler = obj_cav.AddComponent<CanvasScaler>();
                            obj_cav.AddComponent<GraphicRaycaster>();
                            cav.renderMode = RenderMode.ScreenSpaceCamera;
                            CanvasScalerModeIndex.intValue = 1;
                            CanvasScalerModeIndex.serializedObject.ApplyModifiedProperties();
                            switch (CanvasScalerModeIndex.intValue)
                            {
                                case 0:
                                    scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
                                    break;
                                case 1:
                                    scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                                    break;
                                case 2:
                                    scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPhysicalSize;
                                    break;
                            }
                            HudCanvasScaler.objectReferenceValue = scaler;
                            cav.pixelPerfect = UsePerfectPixelUpdate.boolValue;
                            HudCanvas_Screen.objectReferenceValue = cav;
                            HudCanvas_Screen.serializedObject.ApplyModifiedProperties();

                            HudCanvasAnchorIndex.intValue = 1;
                            HudCanvasAnchorIndex.serializedObject.ApplyModifiedProperties();
                            HudCanvasAnchor.enumValueIndex = (int)CanvasCameraAttachment.CameraFar;
                            HudCanvasAnchor.serializedObject.ApplyModifiedProperties();
                            #endregion

                            #region Anchors
                            GameObject obj_Anchors = new GameObject();
                            obj_Anchors.layer = LayerMask.NameToLayer("XHud");
                            UnityEngine.RectTransform Anchors = obj_Anchors.AddComponent<UnityEngine.RectTransform>();
                            CanvasGroup cavgroup = obj_Anchors.AddComponent<CanvasGroup>();
                            HudCanvasGroup_Screen.objectReferenceValue = cavgroup;
                            HudCanvasGroup_Screen.serializedObject.ApplyModifiedProperties();
                            cavgroup.alpha = 1;
                            Anchors.name = "Anchors";
                            Anchors.SetParent(cav.transform);
                            Anchors.localPosition = Vector3.zero;
                            Anchors.localScale = Vector3.one;
                            Anchors.anchorMin = new Vector2(0, 0);
                            Anchors.anchorMax = new Vector2(1, 1);
                            Anchors.sizeDelta = new Vector2(0, 0);

                            HudCanvas_ScreenAnchor.objectReferenceValue = Anchors;
                            HudCanvas_ScreenAnchor.serializedObject.ApplyModifiedProperties();

                            for (int i = 0; i < 11; i++)
                            {
                                GameObject obj_Layanchor = new GameObject();
                                obj_Layanchor.layer = LayerMask.NameToLayer("XHud");
                                UnityEngine.RectTransform layanchor = obj_Layanchor.AddComponent<UnityEngine.RectTransform>();
                                layanchor.SetParent(Anchors);
                                if (i == 0)
                                {
                                    layanchor.name = "Anchor_B";
                                    layanchor.anchorMin = new Vector2(0f, 0f);
                                    layanchor.anchorMax = new Vector2(1f, 1f);
                                    layanchor.pivot = new Vector2(0.5f, 0.5f);
                                    layanchor.sizeDelta = new Vector2(0, 0);
                                }
                                if (i == 1)
                                {
                                    layanchor.name = "Anchor_U";
                                    layanchor.anchorMin = new Vector2(0.5f, 1);
                                    layanchor.anchorMax = new Vector2(0.5f, 1);
                                    layanchor.pivot = new Vector2(0.5f, 1);
                                    layanchor.sizeDelta = new Vector2(20, 20);
                                }
                                if (i == 2)
                                {
                                    layanchor.name = "Anchor_D";
                                    layanchor.anchorMin = new Vector2(0.5f, 0);
                                    layanchor.anchorMax = new Vector2(0.5f, 0);
                                    layanchor.pivot = new Vector2(0.5f, 0);
                                    layanchor.sizeDelta = new Vector2(20, 20);
                                }
                                if (i == 3)
                                {
                                    layanchor.name = "Anchor_L";
                                    layanchor.anchorMin = new Vector2(0, 0.5f);
                                    layanchor.anchorMax = new Vector2(0, 0.5f);
                                    layanchor.pivot = new Vector2(0, 0.5f);
                                    layanchor.sizeDelta = new Vector2(20, 20);
                                }
                                if (i == 4)
                                {
                                    layanchor.name = "Anchor_R";
                                    layanchor.anchorMin = new Vector2(1, 0.5f);
                                    layanchor.anchorMax = new Vector2(1, 0.5f);
                                    layanchor.pivot = new Vector2(1, 0.5f);
                                    layanchor.sizeDelta = new Vector2(20, 20);
                                }
                                if (i == 5)
                                {
                                    layanchor.name = "Anchor_C";
                                    layanchor.anchorMin = new Vector2(0.5f, 0.5f);
                                    layanchor.anchorMax = new Vector2(0.5f, 0.5f);
                                    layanchor.pivot = new Vector2(0.5f, 0.5f);
                                    layanchor.sizeDelta = new Vector2(20, 20);
                                }
                                if (i == 6)
                                {
                                    layanchor.name = "Anchor_L_U";
                                    layanchor.anchorMin = new Vector2(0, 1);
                                    layanchor.anchorMax = new Vector2(0, 1);
                                    layanchor.pivot = new Vector2(0, 1);
                                    layanchor.sizeDelta = new Vector2(20, 20);
                                }
                                if (i == 7)
                                {
                                    layanchor.name = "Anchor_L_D";
                                    layanchor.anchorMin = new Vector2(0, 0);
                                    layanchor.anchorMax = new Vector2(0, 0);
                                    layanchor.pivot = new Vector2(0, 0);
                                    layanchor.sizeDelta = new Vector2(20, 20);
                                }
                                if (i == 8)
                                {
                                    layanchor.name = "Anchor_R_U";
                                    layanchor.anchorMin = new Vector2(1, 1);
                                    layanchor.anchorMax = new Vector2(1, 1);
                                    layanchor.pivot = new Vector2(1, 1);
                                    layanchor.sizeDelta = new Vector2(20, 20);
                                }
                                if (i == 9)
                                {
                                    layanchor.name = "Anchor_R_D";
                                    layanchor.anchorMin = new Vector2(1, 0);
                                    layanchor.anchorMax = new Vector2(1, 0);
                                    layanchor.pivot = new Vector2(1, 0);
                                    layanchor.sizeDelta = new Vector2(20, 20);
                                }
                                if (i == 10)
                                {
                                    layanchor.name = "Anchor_T";
                                    layanchor.anchorMin = new Vector2(0.5f, 0.5f);
                                    layanchor.anchorMax = new Vector2(0.5f, 0.5f);
                                    layanchor.pivot = new Vector2(0.5f, 0.5f);
                                    layanchor.sizeDelta = new Vector2(20, 20);
                                }
                                layanchor.anchoredPosition3D = Vector3.zero;
                                layanchor.localScale = Vector3.one;
                            }
                            #endregion

                            #region ScreenMask
                            GameObject obj_Mask = new GameObject();
                            obj_Mask.layer = LayerMask.NameToLayer("XHud");
                            UnityEngine.RectTransform m_Mask = obj_Mask.AddComponent<UnityEngine.RectTransform>();
                            Image Mask_img = obj_Mask.AddComponent<Image>();
                            Mask_img.color = Color.clear;
                            m_Mask.name = "Mask";
                            m_Mask.SetParent(cav.transform);
                            m_Mask.localPosition = Vector3.zero;
                            m_Mask.localScale = Vector3.one;
                            m_Mask.anchorMin = new Vector2(0, 0);
                            m_Mask.anchorMax = new Vector2(1, 1);
                            m_Mask.sizeDelta = new Vector2(0, 0);
                            Mask.objectReferenceValue = Mask_img;
                            Mask.serializedObject.ApplyModifiedProperties();
                            #endregion

                            #region EventSystem
                            GameObject obj_esys = new GameObject();
                            obj_esys.layer = LayerMask.NameToLayer("XHud");
                            EventSystem esys = obj_esys.AddComponent<EventSystem>();
                            InputSystemUIInputModule slm = obj_esys.AddComponent<InputSystemUIInputModule>();
                            obj_esys.name = "Event";
                            obj_esys.transform.SetParent(BaseScript.transform);
                            obj_esys.transform.localPosition = Vector3.zero;
                            Hud_EventSystem.objectReferenceValue = esys;
                            Hud_InputSystemUIInputModule.objectReferenceValue = slm;
                            Hud_EventSystem.serializedObject.ApplyModifiedProperties();
                            Hud_InputSystemUIInputModule.serializedObject.ApplyModifiedProperties();
                            #endregion

                            #region 获取所有锚点结构 - 布局
                            if (Anchors_Layout_Screen != null && HudCanvas_ScreenAnchor != null)
                            {
                                Transform anchorRoot = HudCanvas_ScreenAnchor.objectReferenceValue as Transform;
                                for (int i = 0; i < anchorRoot.childCount; i++)
                                {
                                    if (anchorRoot.GetChild(i).name.Contains("Anchor"))
                                    {
                                        UnityEngine.RectTransform obj = anchorRoot.GetChild(i).GetComponent<UnityEngine.RectTransform>();
                                        Anchors_Layout_Screen.InsertArrayElementAtIndex(Anchors_Layout_Screen.arraySize);
                                        Anchors_Layout_Screen.GetArrayElementAtIndex(i).FindPropertyRelative("Anchor").objectReferenceValue = obj;
                                        Anchors_Layout_Screen.GetArrayElementAtIndex(i).FindPropertyRelative("Name").stringValue = obj.name;
                                        string Type = anchorRoot.GetChild(i).name;
                                        switch (Type)
                                        {
                                            case "B":
                                                Anchors_Layout_Screen.GetArrayElementAtIndex(i).FindPropertyRelative("Type").enumValueIndex = 0;
                                                break;
                                            case "U":
                                                Anchors_Layout_Screen.GetArrayElementAtIndex(i).FindPropertyRelative("Type").enumValueIndex = 1;
                                                break;
                                            case "D":
                                                Anchors_Layout_Screen.GetArrayElementAtIndex(i).FindPropertyRelative("Type").enumValueIndex = 2;
                                                break;
                                            case "L":
                                                Anchors_Layout_Screen.GetArrayElementAtIndex(i).FindPropertyRelative("Type").enumValueIndex = 3;
                                                break;
                                            case "R":
                                                Anchors_Layout_Screen.GetArrayElementAtIndex(i).FindPropertyRelative("Type").enumValueIndex = 4;
                                                break;
                                            case "C":
                                                Anchors_Layout_Screen.GetArrayElementAtIndex(i).FindPropertyRelative("Type").enumValueIndex = 5;
                                                break;
                                            case "L_U":
                                                Anchors_Layout_Screen.GetArrayElementAtIndex(i).FindPropertyRelative("Type").enumValueIndex = 6;
                                                break;
                                            case "L_D":
                                                Anchors_Layout_Screen.GetArrayElementAtIndex(i).FindPropertyRelative("Type").enumValueIndex = 7;
                                                break;
                                            case "R_U":
                                                Anchors_Layout_Screen.GetArrayElementAtIndex(i).FindPropertyRelative("Type").enumValueIndex = 8;
                                                break;
                                            case "R_D":
                                                Anchors_Layout_Screen.GetArrayElementAtIndex(i).FindPropertyRelative("Type").enumValueIndex = 9;
                                                break;
                                            case "T":
                                                Anchors_Layout_Screen.GetArrayElementAtIndex(i).FindPropertyRelative("Type").enumValueIndex = 10;
                                                break;
                                        }

                                        Anchors_Layout_Screen.serializedObject.ApplyModifiedProperties();
                                    }
                                    else
                                        continue;
                                }
                            }

                            #endregion

                            #region 比例参考图形信息初始化
                            sp_PhysicsScreenSize.vector2Value = new Vector2(59.6f, 33.7f);
                            sp_PhysicsScreenSize.serializedObject.ApplyModifiedProperties();

                            SerializedProperty sp_res_full_sprite = sp_Res_Full.FindPropertyRelative("sprite");
                            SerializedProperty sp_res_full_size = sp_Res_Full.FindPropertyRelative("size");
                            sp_res_full_sprite.objectReferenceValue = AssetDatabase.LoadAssetAtPath<Sprite>($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/HumanRefer/Body_Solid.png");
                            sp_res_full_size.vector2Value = new Vector2(47, 172);
                            sp_Res_Full.serializedObject.ApplyModifiedProperties();

                            SerializedProperty sp_res_part_sprite = sp_Res_Part.FindPropertyRelative("sprite");
                            SerializedProperty sp_res_part_size = sp_Res_Part.FindPropertyRelative("size");
                            sp_res_part_sprite.objectReferenceValue = AssetDatabase.LoadAssetAtPath<Sprite>($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/HumanRefer/Head_Solid.png");
                            sp_res_part_size.vector2Value = new Vector2(19, 30);
                            sp_Res_Part.serializedObject.ApplyModifiedProperties();

                            sp_Reference_Image_Color.colorValue = new Color(0.6f, 0.6f, 0.6f, 0.05f);
                            sp_Reference_Image_Color.serializedObject.ApplyModifiedProperties();
                            #endregion

                            #region 创建XTween动画管理器
                            XTween_Manager xTween_Manager = FindFirstObjectByType<XTween_Manager>();
                            if (xTween_Manager != null)
                                BaseScript.XTweenManager = xTween_Manager;
                            else
                            {
                                GameObject xtween_obj = new GameObject();
                                xtween_obj.name = "XTween(Manager)";
                                xtween_obj.layer = LayerMask.NameToLayer("XHud");
                                xtween_obj.transform.SetParent(null);
                                xtween_obj.transform.localPosition = Vector3.zero;
                                xtween_obj.transform.localEulerAngles = Vector3.zero;
                                xtween_obj.transform.localScale = Vector3.one;
                                xtween_obj.transform.SetAsLastSibling();
                                BaseScript.XTweenManager = xtween_obj.AddComponent<XTween_Manager>();
                            }
                            #endregion

                            #region 初始化动效
                            // 元素生成动效
                            ResetMotionParams("c");
                            // 元素回收动效
                            ResetMotionParams("r");
                            #endregion

                            #endregion

                            IsInitialized.boolValue = true;
                            IsInitialized.serializedObject.ApplyModifiedProperties();
                            return;
                        }
                    });
            };
        }
        #endregion

        #region 辅助
        /// <summary>
        /// 重置生成与回收的参数到默认
        /// </summary>
        private void ResetMotionParams(string state)
        {
            if (state == "c")
            {
                CreateArgs_Default.FindPropertyRelative("anchor").enumValueIndex = (int)XHudAnchor.中心;
                CreateArgs_Default.FindPropertyRelative("Movement.Movement").enumValueIndex = (int)HudMotion_Movement.S_从下至上;
                CreateArgs_Default.FindPropertyRelative("Movement.Distance").floatValue = 100;
                CreateArgs_Default.FindPropertyRelative("Movement.Duration").floatValue = 1;
                CreateArgs_Default.FindPropertyRelative("Movement.Delay").floatValue = 0;
                CreateArgs_Default.FindPropertyRelative("Movement.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
                CreateArgs_Default.FindPropertyRelative("Movement.CurveName").stringValue = "";
                CreateArgs_Default.FindPropertyRelative("Movement.Ease").enumValueIndex = (int)EaseMode.InOutCubic;
                CreateArgs_Default.FindPropertyRelative("Rotation.Rotation").enumValueIndex = (int)HudMotion_Rotation.A_无旋转;
                CreateArgs_Default.FindPropertyRelative("Rotation.Degree").floatValue = 0;
                CreateArgs_Default.FindPropertyRelative("Rotation.Duration").floatValue = 1;
                CreateArgs_Default.FindPropertyRelative("Rotation.Delay").floatValue = 0;
                CreateArgs_Default.FindPropertyRelative("Rotation.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
                CreateArgs_Default.FindPropertyRelative("Rotation.CurveName").stringValue = "";
                CreateArgs_Default.FindPropertyRelative("Rotation.Ease").enumValueIndex = (int)EaseMode.InOutCubic;
                CreateArgs_Default.FindPropertyRelative("Alpha.Duration").floatValue = 1;
                CreateArgs_Default.FindPropertyRelative("Alpha.Delay").floatValue = 0;
                CreateArgs_Default.FindPropertyRelative("Alpha.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
                CreateArgs_Default.FindPropertyRelative("Alpha.CurveName").stringValue = "";
                CreateArgs_Default.FindPropertyRelative("Alpha.Ease").enumValueIndex = (int)EaseMode.InOutCubic;
                CreateArgs_MotionAnimateEndState.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
                CreateArgs_Default.serializedObject.ApplyModifiedProperties();

                Crc_Lib_Name.stringValue = null;
                Crc_Lib_Name.serializedObject.ApplyModifiedProperties();
            }
            else if (state == "r")
            {
                RecycleArgs_Default.FindPropertyRelative("Movement.Movement").enumValueIndex = (int)HudMotion_Movement.D_从上至下;
                RecycleArgs_Default.FindPropertyRelative("Movement.Distance").floatValue = 100;
                RecycleArgs_Default.FindPropertyRelative("Movement.Duration").floatValue = 1;
                RecycleArgs_Default.FindPropertyRelative("Movement.Delay").floatValue = 0;
                RecycleArgs_Default.FindPropertyRelative("Movement.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
                RecycleArgs_Default.FindPropertyRelative("Movement.CurveName").stringValue = "";
                RecycleArgs_Default.FindPropertyRelative("Movement.Ease").enumValueIndex = (int)EaseMode.InOutCubic;
                RecycleArgs_Default.FindPropertyRelative("Rotation.Rotation").enumValueIndex = (int)HudMotion_Rotation.A_无旋转;
                RecycleArgs_Default.FindPropertyRelative("Rotation.Degree").floatValue = 0;
                RecycleArgs_Default.FindPropertyRelative("Rotation.Duration").floatValue = 1;
                RecycleArgs_Default.FindPropertyRelative("Rotation.Delay").floatValue = 0;
                RecycleArgs_Default.FindPropertyRelative("Rotation.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
                RecycleArgs_Default.FindPropertyRelative("Rotation.CurveName").stringValue = "";
                RecycleArgs_Default.FindPropertyRelative("Rotation.Ease").enumValueIndex = (int)EaseMode.InOutCubic;
                RecycleArgs_Default.FindPropertyRelative("Alpha.Duration").floatValue = 1;
                RecycleArgs_Default.FindPropertyRelative("Alpha.Delay").floatValue = 0;
                RecycleArgs_Default.FindPropertyRelative("Alpha.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
                RecycleArgs_Default.FindPropertyRelative("Alpha.CurveName").stringValue = "";
                RecycleArgs_Default.FindPropertyRelative("Alpha.Ease").enumValueIndex = (int)EaseMode.InOutCubic;
                RecycleArgs_MotionAnimateEndState.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
                RecycleArgs_Default.serializedObject.ApplyModifiedProperties();

                Rec_Lib_Name.stringValue = null;
                Rec_Lib_Name.serializedObject.ApplyModifiedProperties();
            }
        }
        /// <summary>
        /// 确保图层存在
        /// </summary>
        /// <param name="layerName"></param>
        private void xHud_EnsureLayerExists(string layerName)
        {
            // 获取 TagManager 资源
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layersProp = tagManager.FindProperty("layers");

            // 检查图层是否已经存在
            bool layerExists = false;
            for (int i = 0; i < layersProp.arraySize; i++)
            {
                SerializedProperty layerProp = layersProp.GetArrayElementAtIndex(i);
                if (layerProp.stringValue == layerName)
                {
                    layerExists = true;
                    break;
                }
            }

            // 如果图层不存在，则创建它
            if (!layerExists)
            {
                for (int i = 8; i < layersProp.arraySize; i++) // 从索引 8 开始，因为前 8 个是 Unity 预留的
                {
                    SerializedProperty layerProp = layersProp.GetArrayElementAtIndex(i);
                    if (layerProp.stringValue == "")
                    {
                        layerProp.stringValue = layerName;
                        tagManager.ApplyModifiedProperties();
                        //sp_DebugMode.Log($"Layer '{layerName}' has been created.");
                        return;
                    }
                }

                //sp_DebugMode.LogError($"No empty layer slots available to create layer '{layerName}'.");
            }
            else
            {
                //sp_DebugMode.Log($"Layer '{layerName}' already exists.");
            }
        }
        /// <summary>
        /// 公共方法，用于对List<Texture2D>进行排序
        /// </summary>
        /// <param name="textures"></param>
        public static void xHud_SortTexturesByNameSuffix(List<Texture2D> textures)
        {
            // 使用Sort方法，并传入自定义的比较器
            textures.Sort((tex1, tex2) =>
            {
                // 获取贴图名称
                string name1 = tex1.name;
                string name2 = tex2.name;

                // 提取名称中的数字后缀
                int num1 = xHud_ExtractNumberFromName(name1);
                int num2 = xHud_ExtractNumberFromName(name2);

                // 按照数字后缀进行比较
                return num1.CompareTo(num2);
            });
        }
        /// <summary>
        /// 从贴图名称中提取数字后缀
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private static int xHud_ExtractNumberFromName(string name)
        {
            // 假设名称格式为 "Ink_数字"
            string[] parts = name.Split('_');

            // 检查是否符合格式
            if (parts.Length != 2 || !int.TryParse(parts[1], out int number))
            {
                throw new ArgumentException($"Invalid texture name format: {name}");
            }

            return number;
        }
        private void SetHighFrequencyMode()
        {
            UPDATE_INTERVAL = UPDATE_INTERVAL_High;
            _lastUserActionTime = EditorApplication.timeSinceStartup;
        }
        #endregion

        #region 打开辅助窗口
        /// <summary>
        /// 构图参考解释窗口
        /// </summary>
        public void xHud_OpenGuideHelpPanel(string guidetype)
        {
            Editor_XHud_Tool_GuideReference window = (Editor_XHud_Tool_GuideReference)EditorWindow.GetWindow(typeof(Editor_XHud_Tool_GuideReference), false, "XHud 构图参考说明书", true);
            XGUI.CenterEditorWindow(new Vector2Int(370, 670), window, false);
            window.GuideType = (guidetype);
            window.SetGuideType(guidetype);
            window.Show();
        }
        /// <summary>
        /// 动效库添加器
        /// </summary>
        public void xHud_OpenParameterSetter(HudElementMotionType Type)
        {
            Editor_XHud_LibrarySetTool_Motion window = EditorWindow.GetWindow<Editor_XHud_LibrarySetTool_Motion>(true);

            window.titleContent = new GUIContent("XHud - 元素动效资源采集器");
            XGUI.CenterEditorWindow(new Vector2Int(348, Type == HudElementMotionType.Creator ? 850 : 780), window);

            switch (Type)
            {
                case HudElementMotionType.Recycler:
                    Motion_Recycler rec = new Motion_Recycler();
                    rec.MotionAnimateEndState = BaseScript.RecycleArgs_Default.MotionAnimateEndState;
                    rec.Alpha = BaseScript.RecycleArgs_Default.Alpha;

                    rec.Movement = new MotionNode_Movement();
                    rec.Movement.CopyData(BaseScript.RecycleArgs_Default.Movement);

                    rec.Rotation = new MotionNode_Rotation();
                    rec.Rotation.CopyData(BaseScript.RecycleArgs_Default.Rotation);

                    rec.Alpha = new MotionNode_Alpha();
                    rec.Alpha.CopyData(BaseScript.RecycleArgs_Default.Alpha);

                    window.SetElementMotion(rec);
                    break;
                case HudElementMotionType.Creator:
                    Motion_Creator crc = new Motion_Creator();
                    crc.MotionAnimateEndState = BaseScript.CreateArgs_Default.MotionAnimateEndState;
                    crc.anchor = BaseScript.CreateArgs_Default.anchor;

                    crc.Alpha = BaseScript.CreateArgs_Default.Alpha;

                    crc.Movement = new MotionNode_Movement();
                    crc.Movement.CopyData(BaseScript.CreateArgs_Default.Movement);

                    crc.Rotation = new MotionNode_Rotation();
                    crc.Rotation.CopyData(BaseScript.CreateArgs_Default.Rotation);

                    crc.Alpha = new MotionNode_Alpha();
                    crc.Alpha.CopyData(BaseScript.CreateArgs_Default.Alpha);

                    window.SetElementMotion(crc);
                    break;
            }

            window.SetElementMotionType(Type);
            window.SetLibrarySetterMode(LibrarySetterMode.添加到库);
            window.SetButtonText("添加", "取消");
            window.SetTitle("元素动效资源采集器");
            window.SetTarget_Hud_MotionLibrary(BaseScript.Hud_Motions);
            //window.ShowModal();
            window.Show();
        }
        #endregion

        #region 元素池
        /// <summary>
        /// 获取元素池的每项元素的标识名称
        /// </summary>
        private void xHud_GetElementPoolIndicators()
        {
            if (BaseScript.Hud_ElementLibrarys != null && BaseScript.Hud_ElementLibrarys.Count > 0)
            {
                for (int i = 0; i < BaseScript.Hud_ElementLibrarys.Count; i++)
                {
                    XHud_Library_Element lib = BaseScript.Hud_ElementLibrarys[i];
                    if (lib != null)
                    {
                        for (int s = 0; s < lib.ElementLibrary.Count; s++)
                        {
                            XHud_LibraryArg_Element_Item item = lib.ElementLibrary[s];
                            if (item.Target != null)
                            {
                                if (item.Name != item.Target.transform.name)
                                    item.Name = item.Target.transform.name;
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region 诊断面板
        /// <summary>
        /// 诊断
        /// </summary>
        /// <returns></returns>
        private bool xHud_Diagnostic()
        {
            bool hasDiagnostic = false;

            if (ScreenRes.vector2Value == Vector2.zero)
            {
                hasDiagnostic = true;
            }

            if (SceneCamera.objectReferenceValue == null)
            {
                hasDiagnostic = true;
            }

            if (Lib_Color.objectReferenceValue == null)
            {
                hasDiagnostic = true;
            }

            if (Lib_Curve.objectReferenceValue == null)
            {
                hasDiagnostic = true;
            }

            if (Lib_Sound.objectReferenceValue == null)
            {
                hasDiagnostic = true;
            }

            if (Lib_TextStyleLibrary.objectReferenceValue == null)
            {
                hasDiagnostic = true;
            }

            if (Lib_Motions.objectReferenceValue == null)
            {
                hasDiagnostic = true;
            }

            if (Lib_Transition.objectReferenceValue == null)
            {
                hasDiagnostic = true;
            }

            if (Lib_ElementLibrarys.arraySize <= 0)
            {
                hasDiagnostic = true;
            }

            return hasDiagnostic;
        }
        #endregion

        #region 参考比例计算
        /// <summary>
        /// 比例参考标识物体 - 计算
        /// </summary>
        public void xHud_RatioReference_Calculate()
        {
            Vector2 res = ScreenRes.vector2Value;
            Vector2 screen_size = sp_PhysicsScreenSize.vector2Value;
            Vector2 refer_size = RatioReferenceIsPart.boolValue ? BaseScript.Res_Part.size : BaseScript.Res_Full.size;

            if (refer_size == Vector2.zero)
                return;

            float pw = refer_size.x / screen_size.x;
            float ph = refer_size.y / screen_size.y;

            float nw = pw * res.x;
            float nh = ph * res.y;

            BaseScript.Reference_Image.rectTransform.sizeDelta = new Vector2(nw, nh);
        }
        /// <summary>
        /// 比例参考标识物体 - 创建
        /// </summary>
        public void xHud_RatioReference_Create(Sprite spr)
        {
            Undo.SetCurrentGroupName("Create Image Operation");
            GameObject ref_hum_ima_obj = new GameObject();
            ref_hum_ima_obj.name = "ReferRatioMark";
            Image ref_hum_ima = ref_hum_ima_obj.AddComponent<Image>();
            ref_hum_ima.raycastTarget = false;
            ref_hum_ima.maskable = false;
            ref_hum_ima.sprite = spr;

            ref_hum_ima.rectTransform.SetParent(BaseScript.HudCanvas_Screen.transform);
            ref_hum_ima.rectTransform.SetAsLastSibling();

            ref_hum_ima.rectTransform.anchorMin = new Vector2(0.5f, 0);
            ref_hum_ima.rectTransform.anchorMax = new Vector2(0.5f, 0);
            ref_hum_ima.rectTransform.pivot = new Vector2(0.5f, 0);

            ref_hum_ima.rectTransform.localScale = Vector3.one;
            ref_hum_ima.rectTransform.localPosition = Vector3.zero;
            ref_hum_ima.rectTransform.localEulerAngles = Vector3.zero;
            ref_hum_ima.rectTransform.anchoredPosition3D = Vector3.zero;

            ref_hum_ima.color = sp_Reference_Image_Color.colorValue;

            ref_hum_ima.SetNativeSize();

            sp_Reference_Image.objectReferenceValue = ref_hum_ima;
            sp_Reference_Image.serializedObject.ApplyModifiedProperties();


            // 记录撤销操作
            Undo.RegisterCreatedObjectUndo(ref_hum_ima_obj, "Create Image");
            Undo.RecordObject(sp_Reference_Image.serializedObject.targetObject, "Update SerializedProperty");
            xHud_RatioReference_Update();
        }
        /// <summary>
        /// 比例参考标识物体 - 销毁
        /// </summary>
        public void xHud_RatioReference_Destroy()
        {
            if (sp_Reference_Image.objectReferenceValue == null)
                return;

            Image img = sp_Reference_Image.objectReferenceValue as Image;

            DestroyImmediate(img.gameObject, true);
            sp_Reference_Image.objectReferenceValue = null;
            sp_Reference_Image.serializedObject.ApplyModifiedProperties();
        }
        /// <summary>
        /// 切换参考类型
        /// </summary>
        public void xHud_RatioReference_Switch()
        {
            if (sp_Reference_Image.objectReferenceValue != null)
            {
                Sprite spr = BaseScript.Res_Part.sprite;
                if (!RatioReferenceIsPart.boolValue)
                    spr = BaseScript.Res_Full.sprite;

                if (spr == null)
                    return;

                Image img = sp_Reference_Image.objectReferenceValue as Image;

                img.sprite = spr;
            }
        }
        /// <summary>
        /// 比例参考标识物体 - 刷新
        /// </summary>
        public void xHud_RatioReference_Update()
        {
            if (!UseRatioReference.boolValue)
                return;
            if (sp_Reference_Image.objectReferenceValue != null)
            {
                BaseScript.Reference_Image.color = sp_Reference_Image_Color.colorValue;
                BaseScript.Reference_Image.rectTransform.anchoredPosition3D = Vector3.zero;
                xHud_RatioReference_Calculate();
            }
        }
        #endregion

        #region 生成比例参考标记
        private void xHud_CreateReferenceShape(float width, float height, float tolerance_w = 0, float tolerance_h = 0)
        {
            float o_w = width;
            float o_h = height;

            width += tolerance_w;
            height += tolerance_h;

            // 屏幕宽度（厘米）
            float screenWidthCm = sp_PhysicsScreenSize.vector2Value.x;
            // 屏幕高度（厘米）
            float screenHeightCm = sp_PhysicsScreenSize.vector2Value.y;

            // 获取屏幕分辨率
            float screenWidthPx = ScreenRes.vector2Value.x;
            float screenHeightPx = ScreenRes.vector2Value.y;

            // 计算目标宽度对应的像素值
            float imageWidthPx = (width / screenWidthCm) * screenWidthPx;

            // 计算目标高度对应的像素值
            float imageHeightPx = (height / screenHeightCm) * screenHeightPx;

            // 校准宽度或高度，确保比例一致
            float widthToHeightRatio = width / height;
            float actualWidthToHeightRatio = imageWidthPx / imageHeightPx;

            if (actualWidthToHeightRatio != widthToHeightRatio)
            {
                // 如果比例不一致，调整高度以匹配宽度
                imageHeightPx = imageWidthPx / widthToHeightRatio;
            }

            // 创建 Element 对象
            GameObject obj = new GameObject();
            Undo.RegisterCreatedObjectUndo(obj, $"CreateRealWorldReferShape - {o_w} cm x {o_h} cm");
            obj.name = $"RealWorldReferShape - {o_w} cm x {o_h} cm";
            obj.layer = LayerMask.NameToLayer("XHud");
            XHud_Module_Element obj_ele = obj.AddComponent<XHud_Module_Element>();
            obj_ele.Indicator = "RealWorldReferShape";
            UnityEngine.RectTransform obj_ele_rect = obj.GetComponent<UnityEngine.RectTransform>();
            obj_ele_rect.transform.SetParent(BaseScript.hm_ScreenElement_GetAnchored_RectTransform(XHudAnchor.顶层));
            obj_ele_rect.anchoredPosition3D = Vector3.zero;
            obj_ele_rect.localEulerAngles = Vector3.zero;
            obj_ele_rect.localScale = Vector3.one;

            // 创建 Image组件 对象
            GameObject image = new GameObject($"ReferShape");
            Image img = image.AddComponent<Image>();
            img.raycastTarget = false;
            img.maskable = false;
            img.sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/ReferShapeBG.png");
            img.type = Image.Type.Tiled;
            img.color = Color.white * 0.48f;
            img.pixelsPerUnitMultiplier = 1;
            img.rectTransform.SetParent(obj.transform);
            img.rectTransform.localScale = Vector3.one;
            img.rectTransform.localEulerAngles = Vector3.zero;
            img.rectTransform.localPosition = Vector3.zero;
            img.rectTransform.anchoredPosition3D = Vector3.zero;
            img.rectTransform.sizeDelta = new Vector2(imageWidthPx, imageHeightPx);

            GameObject textobj = new GameObject();
            textobj.transform.SetParent(obj.transform);
            textobj.name = "ValueText";
            textobj.transform.localScale = Vector3.one;
            textobj.transform.localEulerAngles = Vector3.zero;
            textobj.transform.localPosition = Vector3.zero;
            textobj.layer = LayerMask.NameToLayer("XHud");

            Font font = AssetDatabase.LoadAssetAtPath<Font>($"{XHud_Dashboard.Get_Path_XHUD_ROOT()}Fonts/Text/sx_light.otf");

            UnityEngine.RectTransform rect = textobj.AddComponent<UnityEngine.RectTransform>();
            rect.sizeDelta = new Vector2(120, 30);
            rect.anchorMin = Vector2.one * 0.5f;
            rect.anchorMax = Vector2.one * 0.5f;
            rect.pivot = Vector2.one * 0.5f;
            rect.anchoredPosition3D = new Vector3(0, rect.anchoredPosition3D.y + img.rectTransform.sizeDelta.y / 2 + 30, 0);

            XHud_Module_Text hud_text = textobj.AddComponent<XHud_Module_Text>();
            hud_text.TextStyleInfo.txt_Set_Alignment(ContentAnchor.中心);
            hud_text.alignment = TextAnchor.MiddleCenter;
            hud_text.text = $"{o_w} cm x {o_h} cm";
            hud_text.TextStyleInfo.txt_Set_FontSize(18);
            hud_text.TextStyleInfo.txt_Set_FontColor(Color.white);
            hud_text.TextStyleInfo.txt_Set_Font(font);
            hud_text.font = font;
            hud_text.TextStyleInfo.txt_Set_FontStyle(FontStyle.Normal);
            hud_text.TextStyleInfo.txt_Set_Overflow(HorizontalWrapMode.Overflow);
            hud_text.TextStyleInfo.txt_Set_Overflow(HorizontalWrapMode.Overflow);
            hud_text.TextStyleInfo.gen_RayCastSet(false);

            Selection.activeObject = obj;
        }
        #endregion

        #region 分辨率
        /// <summary>
        /// 刷新分辨率
        /// </summary>
        private void RefreshResolution()
        {
            CanvasScaler scaler = (CanvasScaler)HudCanvasScaler.objectReferenceValue;
            scaler.referenceResolution = ScreenRes.vector2Value;
            CanvasScalerScreenSize.vector2Value = ScreenRes.vector2Value;
        }
        /// <summary>
        /// 切换分辨率
        /// </summary>
        /// <param name="indicator"></param>
        public void xHud_SwitchResolution(string indicator)
        {
            for (int i = 0; i < RMS_Nodes.arraySize; i++)
            {
                SerializedProperty resroot = RMS_Nodes.GetArrayElementAtIndex(i);
                SerializedProperty x_res = resroot.FindPropertyRelative("Res");
                SerializedProperty x_indicator = resroot.FindPropertyRelative("Indicator");

                if (indicator == x_indicator.stringValue)
                {
                    ScreenRes.vector2Value = x_res.vector2Value;

                    RefreshResolution();
                    // 设置编辑器的分辨率
                    if (SizeExists(GameViewSizeGroupType.Standalone, (int)ScreenRes.vector2Value.x, (int)ScreenRes.vector2Value.y))
                    {
                        SetSize(FindSize(GameViewSizeGroupType.Standalone, (int)ScreenRes.vector2Value.x, (int)ScreenRes.vector2Value.y));
                    }
                    else
                    {
                        Debug.Log("不存在目标分辨率项！");
                    }
                }
                xHud_EditorUpdate_HelperVisual();
            }
        }
        public static Vector2 xHud_GetMainGameViewSize()
        {
            System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
            System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            System.Object Res = GetSizeOfMainGameView.Invoke(null, null);
            return (Vector2)Res;
        }
        private static void xHud_InitiaGameViewResolutions()
        {
            var sizesType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizes");
            var singleType = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
            var instanceProp = singleType.GetProperty("instance");
            getGroup = sizesType.GetMethod("GetGroup");
            gameViewSizesInstance = instanceProp.GetValue(null, null);
        }
        static object GetGroup(GameViewSizeGroupType type)
        {
            return getGroup.Invoke(gameViewSizesInstance, new object[] { (int)type });
        }
        public static int FindSize(GameViewSizeGroupType sizeGroupType, string text)
        {
            var group = GetGroup(sizeGroupType);
            var getDisplayTexts = group.GetType().GetMethod("GetDisplayTexts");
            var displayTexts = getDisplayTexts.Invoke(group, null) as string[];
            for (int i = 0; i < displayTexts.Length; i++)
            {
                string display = displayTexts[i];

                int pren = display.IndexOf('(');
                if (pren != -1)
                    display = display.Substring(0, pren - 1);
                if (display == text)
                    return i;
            }
            return -1;
        }
        public static int FindSize(GameViewSizeGroupType sizeGroupType, int width, int height)
        {
            var group = GetGroup(sizeGroupType);
            var groupType = group.GetType();
            var getBuiltinCount = groupType.GetMethod("GetBuiltinCount");
            var getCustomCount = groupType.GetMethod("GetCustomCount");
            int sizesCount = (int)getBuiltinCount.Invoke(group, null) + (int)getCustomCount.Invoke(group, null);
            var getGameViewSize = groupType.GetMethod("GetGameViewSize");
            var gvsType = getGameViewSize.ReturnType;
            var widthProp = gvsType.GetProperty("width");
            var heightProp = gvsType.GetProperty("height");
            var indexValue = new object[1];
            for (int i = 0; i < sizesCount; i++)
            {
                indexValue[0] = i;
                var size = getGameViewSize.Invoke(group, indexValue);
                int sizeWidth = (int)widthProp.GetValue(size, null);
                int sizeHeight = (int)heightProp.GetValue(size, null);
                if (sizeWidth == width && sizeHeight == height)
                    return i;
            }
            return -1;
        }
        public static bool SizeExists(GameViewSizeGroupType sizeGroupType, int width, int height)
        {
            return FindSize(sizeGroupType, width, height) != -1;
        }
        public static bool SizeExists(GameViewSizeGroupType sizeGroupType, string text)
        {
            return FindSize(sizeGroupType, text) != -1;
        }
        public static void AddCustomSize(GameViewSizeType viewSizeType, GameViewSizeGroupType sizeGroupType, int width, int height, string text)
        {
            var group = GetGroup(sizeGroupType);
            var addCustomSize = getGroup.ReturnType.GetMethod("AddCustomSize");
            var gvsType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSize");
            var ctor = gvsType.GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int), typeof(string) });
            var newSize = ctor.Invoke(new object[] { (int)viewSizeType, width, height, text });
            addCustomSize.Invoke(group, new object[] { newSize });
        }
        public static void SetSize(int index)
        {
            var gvWndType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
            var selectedSizeIndexProp = gvWndType.GetProperty("selectedSizeIndex",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var gvWnd = EditorWindow.GetWindow(gvWndType);
            selectedSizeIndexProp.SetValue(gvWnd, index, null);
        }
        #endregion

        #region 绘制方法
        /// <summary>
        /// 通用方法：绘制开关
        /// </summary>
        private void DrawToggle(string title, SerializedProperty prop, float width, XGUIToggleStyle style = XGUIToggleStyle.实体, Color color_bg_on = default, Color color_bg_off = default, Color color_on = default, Color color_off = default, Action<bool> act_on_changed = null)
        {
            XGUI.layout_toggle(
                title: title,
                title_size: XGUIFontSize.M,
                title_font_style: FontStyle.Normal,
                title_padding: new RectOffset(0, 10, 0, 0),
                title_width: width,
                prop: prop,
                tog_style: style,
                tog_padding: new RectOffset(5, 8, 0, 0),
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
                tog_mixed_bg_color_gui: XHud_Dashboard.Theme_Primary,
                act_on_changed: act_on_changed);
        }
        #endregion
    }
}