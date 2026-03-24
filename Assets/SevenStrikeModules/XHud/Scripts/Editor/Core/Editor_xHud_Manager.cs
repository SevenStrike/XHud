namespace SevenStrikeModules.XHud.Hud
{
    using DG.Tweening;
    using Newtonsoft.Json;
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.GuiLib;
    using SevenStrikeModules.XHud.Utilitys;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using TMPro;
    using Unity.EditorCoroutines.Editor;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.InputSystem.UI;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;
    using UnityEngine.UI;

    [CustomEditor(typeof(xHud_Manager))]
    public class Editor_xHud_Manager : Editor
    {
        #region 组件 / 列表
        public xHud_Manager BaseScript;
        private ReorderableList
            List_ElementLib,
            List_RMM,
            List_SoundsLib;
        #endregion

        private float LineHeight;
        private string Ver, Sub;
        static MethodInfo getGroup;
        static object gameViewSizesInstance;

        #region 序列化属性
        SerializedProperty IsInitialized, Lib_ElementLibrarys, SoundLibrary, SounderPoolCount, Anchors_Layout_Screen, HudCanvas_ScreenAnchor, Lib_Color, Lib_Curve, Lib_Sound, Lib_TextStyleLibrary, Lib_ElementMotion, Lib_Transition, HudCamera, FontSizeMultiply, CanvasScalerModeIndex, CanvasScalerScreenSize, CanvasMatchDir, HudCanvas_Screen, HudCanvas_World, HudCanvasScaler, HudCanvas_WorldAnchor, Mask, HudCanvasGroup_Screen, HudCanvasGroup_World, UseInstanceMode, UseDebug, UseSafeFrame, UseAutoPerfectPixel, UsePerfectPixelUpdate, SafeFrameStructureDisplayer, CameraOthograpicMode, CustomCursor, CustomTransition, SupportWorldUI, Safe_Frame, Margins, MarginHorizontal, MarginMultiply, MarginVertical, MarkSize, Color_LayoutAnchorMark, Color_FrameLine, ScreenRes, Safe_FrameLine_Width, Safe_FrameLine_Margins, Color_SeperaterLine, Safe_Seperater_Length, Safe_CenterMarkLength, Safe_CenterMarkWidth, Color_CenterMark, Safe_CenterMarkDistance, CameraOrthographicSize, CameraFov, CameraCutter_Near, CameraCutter_Far, HudCanvasAnchor, CanvasDistance, HudCanvasAnchorIndex, MaskAlpha, MaskTexture, MaskRaycastAlphaThreshold, MaskRaycastEnabled, MaskColor, BlurMask, BlurMaskAlpha, BlurMaskColor, BlurMaskTexture, BlurMaskRaycastAlphaThreshold, BlurMaskRaycastEnabled, ContentAlpha_Screen, ContentAlpha_World, DurationMultiply, Hud_MouseCursor, Hud_TransitionController, RecycleArgs_Default, CreateArgs_Default, SceneCamera, Volume, VolumeMute, BluePrint_root, BluePrintMode, BluePrint_grid_size, BluePrint_grid_color, BluePrint_bg_color, BluePrint_bg_decal_color, BluePrint_mark_size, BluePrint_mark_opacity, BluePrint_linewidth, BluePrint_title_content, BluePrint_subtitle_content, BluePrint_marktitle_color, BluePrint_marksubtitle_color, BluePrint_mark_margin, BluePrint_MarkAnchors, BluePrint_mark_space, BluePrint_opacity, BluePrint_AnimationDuration, BluePrint_Displayed, BluePrint_OnStartHide, BluePrint_Grid_AnimationEase, BluePrint_grid_Opacity, BluePrint_Grid_AnimationDuration, BluePrint_Bg_AnimationEase_In, BluePrint_Bg_AnimationEase_Out, BluePrint_bg_opacity, BluePrint_Bg_FadeAnimationDelay, BluePrint_Grid_LengthPercentage, BluePrint_Grid_LevelHeight, BluePrint_GridEnd, BluePrint_bg_tilling, BluePrint_bg_name, BluePrint_bg_usetilling_index, BluePrint_bg_usesquareratio_index, BluePrint_bg_mapOpacity, UniversalFeature_Blur_Intensity, Eft_Grid, Eft_GridFade, Eft_Bg, Eft_Mark, RMS_Enabled, RMS_CurrentSolution, RMS_Nodes, theme_color, theme_color_gp, theme_color_sep, EnabledLedEffect, ThemeSolution, ThemeEdgeSolution, Hud_EventSystem, Hud_InputSystemUIInputModule, CreateArgs_MotionAnimateEndState, RecycleArgs_MotionAnimateEndState, Crc_Lib_Name, Rec_Lib_Name, UseLocalization, sp_PhysicsScreenSize, sp_Reference_Image, UseRatioReference, RatioReferenceIsPart, sp_Reference_Image_Color, sp_Res_Full, sp_Res_Part, sp_ReferShape_RatioSize, sp_ReferShape_RatioTolerance, FoldAllPanelWithDisabled, CompGuide_Anchors, CompGuide_AnchorRoot, UseCompGuide, CompGuideMode, GuideColor, GuidePointColor, GuideParam_Mirror_LR_Offset, GuideParam_Mirror_UD_Offset, GuideParam_Mirror_LR_GoldenMode, GuideParam_Mirror_UD_GoldenMode, GuideParam_Fibonacci_Mode, GuideParam_CornerLookat_Offset_H, GuideParam_CornerLookat_Offset_V, GuideParam_Three_Offset_H, GuideParam_Three_Offset_Coverage, GuideParam_Three_Offset_V, GuideParam_GuideLine_BaseHeight, GuideParam_GuideLine_Offset_Near, GuideParam_GuideLine_Offset_Far, GuideParam_GuideLine_Offset_NearHeight, GuideParam_Triangle_BaseHeight, GuideParam_Triangle_BottomHeight, GuideParam_Triangle_Offset_Left, GuideParam_Triangle_Offset_Right, GuideParam_Triangle_TopOffset, GuideParam_CenterPointSize, GuideParam_IShape_TopHeight, GuideParam_IShape_BottomHeight, GuideParam_IShape_Offset_Left, GuideParam_IShape_Offset_Right, GuideParam_GuideLine_BaseOffset;
        #endregion

        #region 图标
        public Texture2D status, rmsicon, icon_MC_Use, icon_MC_None, soundplaying, panel_option, panel_canvas, panel_camera, panel_libs, panel_elementlibs, panel_frame, panel_assist, panel_bulueprint, panel_matchres, panel_audios, panel_mask, panel_global, panel_tools, panel_eleparamrecycle_auto, panel_eleparammotion_default, panel_theme, panel_localized, panel_physicsratio, save_r, save_p, locate_r, locate_p, reset_r, reset_p, icon_initia_r, icon_initia_p, icon_fastmaskswitch_r, icon_fastmaskswitch_p, icon_safeframevisual_r, icon_safeframevisual_p, icon_checkstate_r, icon_checkstate_p, panel_comp_guide, GuideHelp_Released, GuideHelp_Press;
        #endregion

        #region 开关
        private bool FrameColors, FrameStyles, def_recycle_fold_move, def_recycle_fold_rotate, def_recycle_fold_alpha, def_create_fold_move, def_create_fold_rotate, def_create_fold_alpha, BasicVars;
        #endregion

        #region 蓝图
        List<Texture2D> BluePrintBgs = new List<Texture2D>();
        string[] BluePrintBgs_Name;
        #endregion

        #region 键
        private string PrefsKeyFold_Option = "XHUD-MANAGER-FOLD-OPTION", PrefsKeyFold_CanvasCamera = "XHUD-MANAGER-FOLD-CANVAS", PrefsKeyFold_Camera = "XHUD-MANAGER-FOLD-CAMERA", PrefsKeyFold_Libs = "XHUD-MANAGER-FOLD-LIBRARYS", PrefsKeyFold_FrameLayout = "XHUD-MANAGER-FOLD-FRAMELAYOUT", PrefsKeyFold_Assist = "XHUD-MANAGER-FOLD-ASSIST", PrefsKeyFold_BluePrint = "XHUD-MANAGER-FOLD-BLUEPRINT", PrefsKeyFold_LayoutMatchRes = "XHUD-MANAGER-FOLD-LAYOUTMATCHRESOLUTION", PrefsKeyFold_ElementLibs = "XHUD-MANAGER-FOLD-ELEMENTLIBS", PrefsKeyFold_Audios = "XHUD-MANAGER-FOLD-AUDIOS", PrefsKeyFold_Mask = "XHUD-MANAGER-FOLD-MASK", PrefsKeyFold_BlurMask = "XHUD-MANAGER-FOLD-BLURMASK", PrefsKeyFold_Global = "XHUD-MANAGER-FOLD-GLOBAL", PrefsKeyFold_Tools = "XHUD-MANAGER-FOLD-TOOLS", PrefsKeyFold_PhysicsRatio = "XHUD-MANAGER-FOLD-PHYSICS", PrefsKeyFold_ElementParam_DefaultMotion = "XHUD-MANAGER-FOLD-ELEMENTPARAM_DEFAULTCREATOR", PrefsKeyFold_Theme = "XHUD-MANAGER-FOLD-THEME", PrefsKeyFold_Localized = "XHUD-MANAGER-FOLD-LOCALIZED", PrefsKeyFold_Comp_Guide = "XHUD-MANAGER-FOLD-COMPGUIDE";

        private static string PrefsKeyColor_Theme = "XHUD-MANAGER-COLOR-THEME";
        private static string PrefsKeyColor_Theme_GP = "XHUD-MANAGER-COLOR-THEME-GROUP";
        private static string PrefsKeyColor_Theme_SEP = "XHUD-MANAGER-COLOR-THEME-SEPERATE";
        #endregion

        #region Led参数
        private EditorCoroutine Coroutine_LedBlink;
        // 增加速度
        public float increaseSpeed = 0.3f;
        // 减少速度
        public float decreaseSpeed = 0.1f;
        // 亮度上限
        public float led_higher = 1.2f;
        // 亮度下限
        public float led_lower = -0.2f;
        // 亮度刷新速度
        public float led_rate = 0.05f;
        // Led亮度值
        private float LedAlpha;
        // 贴图 - 开启
        public Texture2D icon_led_on;
        // 贴图 - 关闭
        public Texture2D icon_led_off;
        // 贴图 - 光晕
        public Texture2D icon_led_halo;
        #endregion

        #region 选项文字
        string[] stroptions_enabled = new string[2] { "关闭", "开启" }, stroptions_debug = new string[2] { "关闭", "调试" }, stroptions_hide = new string[2] { "隐藏", "显示" }, stroptions_align = new string[2] { "关闭", "对齐" }, stroptions_useful = new string[2] { "关闭", "使用" }, stroptions_refertype = new string[2] { "整体", "局部" }, stroptions_perspective = new string[2] { "透视", "正交" }, stroptions_mute = new string[2] { "关闭", "静音" }, stroptions_mouse = new string[2] { "原生", "自定" }, stroptions_world = new string[2] { "禁用", "支持" }, stroptions_support = new string[2] { "禁用", "支持" }, stroptions_effect = new string[2] { "关闭", "影响" }, stroptions_canvassize = new string[3] { "固定像素尺寸", "屏幕尺寸", "固定物理尺寸" }, stroptions_canvasanchor = System.Enum.GetNames(typeof(CanvasAnchor)), ThemeSolutionNames = new string[] { "默认", "白色", "黑色", "乳白", "浅灰", "沙漠灰", "科幻青", "液晶绿", "橄榄绿", "湖蓝", "天蓝", "胭脂粉", "灵动粉", "秋叶黄", "警示黄", "高亮橘", "烈焰红", }, ThemeEdgeSolutionNames = new string[] { "默认", "白色", "浅灰", "深灰", "黑色", "极简", "珊瑚红", "落叶黄", "烟灰蓝", "青苔绿", "荧光绿", "淡粉" };
        #endregion

        #region 字体
        /// <summary>
        /// 字体 - 粗体
        /// </summary>
        Font Font_Bold;
        /// <summary>
        /// 字体 - 细体
        /// </summary>
        Font Font_Thin;
        #endregion

        #region 批量化操作
        xHud_Manager[] SelectedObjects;


        private void GetAllTargets()
        {
            if (targets.Length > 1)
            {
                SelectedObjects = new xHud_Manager[targets.Length];
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    var t = targets[i];
                    SelectedObjects[i] = (xHud_Manager)t;
                }
            }
            else
            {
                SelectedObjects = new xHud_Manager[targets.Length];
                SelectedObjects[0] = (xHud_Manager)target;
            }
        }

        private bool IsMultiSelected()
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

        private void OnEnable()
        {
            FunctionGroupPrefsCheck();

            BaseScript = (xHud_Manager)target;
            LineHeight = EditorGUIUtility.singleLineHeight;

            GetAllTargets();

            Font_Bold = Editor_xHudGUI.GetFont("SS_Editor_Bold");
            Font_Thin = Editor_xHudGUI.GetFont("SS_Editor_Thin");

            var infos = AssetDatabase.LoadAssetAtPath<TextAsset>(xHud_Dashboard.Get_GUIRoot_Path() + "/XHudDevsInfo.json");
            xHudDevInfos info = JsonConvert.DeserializeObject<xHudDevInfos>(infos.ToString());

            Ver = info.ver;
            Sub = info.sub;

            #region 获取图标       
            icon_MC_Use = Editor_xHudGUI.GetIcon("Icons_Hud_Manager/Icon_Anchor_MC_Use");
            icon_MC_None = Editor_xHudGUI.GetIcon("Icons_Hud_Manager/Icon_Anchor_MC_None");
            status = Editor_xHudGUI.GetIcon("Icons_Hud_Manager/status");
            rmsicon = Editor_xHudGUI.GetIcon("Icons_Hud_Manager/rmsicon");
            soundplaying = Editor_xHudGUI.GetIcon("Icons_Hud_Manager/soundplaying");
            save_r = Editor_xHudGUI.GetIcon("Icons_Hud_Manager/save_r");
            save_p = Editor_xHudGUI.GetIcon("Icons_Hud_Manager/save_p");
            locate_r = Editor_xHudGUI.GetIcon("Icons_Hud_Manager/locate_r");
            locate_p = Editor_xHudGUI.GetIcon("Icons_Hud_Manager/locate_p");
            reset_r = Editor_xHudGUI.GetIcon("Icons_Hud_Manager/reset_r");
            reset_p = Editor_xHudGUI.GetIcon("Icons_Hud_Manager/reset_p");

            #region 标题图标
            panel_option = Editor_xHudGUI.GetIcon("Icons_Hud_Manager/panel_option");
            panel_canvas = Editor_xHudGUI.GetIcon("Icons_Hud_Manager/panel_canvas");
            panel_camera = Editor_xHudGUI.GetIcon("Icons_Hud_Manager/panel_camera");
            panel_libs = Editor_xHudGUI.GetIcon("Icons_Hud_Manager/panel_libs");
            panel_elementlibs = Editor_xHudGUI.GetIcon("Icons_Hud_Manager/panel_elementlibs");
            panel_frame = Editor_xHudGUI.GetIcon("Icons_Hud_Manager/panel_frame");
            panel_assist = Editor_xHudGUI.GetIcon("Icons_Hud_Manager/panel_assist");
            panel_bulueprint = Editor_xHudGUI.GetIcon("Icons_Hud_Manager/panel_bulueprint");
            panel_matchres = Editor_xHudGUI.GetIcon("Icons_Hud_Manager/panel_matchres");
            panel_audios = Editor_xHudGUI.GetIcon("Icons_Hud_Manager/panel_audios");
            panel_mask = Editor_xHudGUI.GetIcon("Icons_Hud_Manager/panel_mask");
            panel_global = Editor_xHudGUI.GetIcon("Icons_Hud_Manager/panel_global");
            panel_tools = Editor_xHudGUI.GetIcon("Icons_Hud_Manager/panel_tools");
            panel_eleparamrecycle_auto = Editor_xHudGUI.GetIcon("Icons_Hud_Manager/panel_eleparamrecycle_auto");
            panel_eleparammotion_default = Editor_xHudGUI.GetIcon("Icons_Hud_Manager/panel_eleparammotion_default");
            panel_localized = Editor_xHudGUI.GetIcon("Icons_Hud_Manager/panel_localized");
            panel_theme = Editor_xHudGUI.GetIcon("Icons_Hud_Manager/panel_theme");
            panel_physicsratio = Editor_xHudGUI.GetIcon("Icons_Hud_Manager/panel_physics");
            panel_comp_guide = Editor_xHudGUI.GetIcon("Icons_Hud_Manager/panel_CompGuide");
            #endregion

            icon_initia_r = Editor_xHudGUI.GetIcon("Icons_Hud_Manager/icon_initia_r");
            icon_initia_p = Editor_xHudGUI.GetIcon("Icons_Hud_Manager/icon_initia_p");
            icon_fastmaskswitch_r = Editor_xHudGUI.GetIcon("Icons_Hud_Manager/icon_fastmaskswitch_r");
            icon_fastmaskswitch_p = Editor_xHudGUI.GetIcon("Icons_Hud_Manager/icon_fastmaskswitch_p");
            icon_safeframevisual_r = Editor_xHudGUI.GetIcon("Icons_Hud_Manager/icon_safeframevisual_r");
            icon_safeframevisual_p = Editor_xHudGUI.GetIcon("Icons_Hud_Manager/icon_safeframevisual_p");
            icon_checkstate_r = Editor_xHudGUI.GetIcon("Icons_Hud_Manager/icon_checkstate_r");
            icon_checkstate_p = Editor_xHudGUI.GetIcon("Icons_Hud_Manager/icon_checkstate_p");

            GuideHelp_Released = Editor_xHudGUI.GetIcon("Icons_Hud_Manager/GuideHelp_Released");
            GuideHelp_Press = Editor_xHudGUI.GetIcon("Icons_Hud_Manager/GuideHelp_Press");

            #endregion

            #region 获取Led图标
            icon_led_on = Editor_xHudGUI.GetIcon("Led/led_on");
            icon_led_off = Editor_xHudGUI.GetIcon("Led/led_off");
            icon_led_halo = Editor_xHudGUI.GetIcon("Led/led_halo");
            #endregion

            SerializedAllVariables();

            // 目录图标视觉刷新
            HierarachyVisualUpdate();

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

            GetElementPoolIndicators();

            #region 分辨率库
            List_RMM = new ReorderableList(serializedObject, RMS_Nodes)
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
                        Editor_xHudGUI.Gui_Property_Field(new Rect(rect.x + 30, rect.y + 3, 65, 20), "", sp_node_indicator, 0, 0);
                        Editor_xHudGUI.Gui_Property_Field(new Rect(rect.x + 110, rect.y + 3, rect.width - 110, 20), "", sp_node_res, 0, 0);

                        if (ScreenRes.vector2Value != sp_node_res.vector2Value)
                        {
                            GUI.color = Color.gray;
                        }
                        else
                        {
                            GUI.color = xHud_Dashboard.Theme_Primary;
                        }
                        Editor_xHudGUI.Gui_Icon(new Rect(rect.x + 5, rect.y + 3, 14, 14), rmsicon);
                        GUI.color = Color.white;
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
                    SwitchResolution(indicator.stringValue);
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
                    xHud_Library_Element element = sp_elementlib.objectReferenceValue as xHud_Library_Element;

                    Editor_xHudGUI.Gui_Property_Field(new Rect(rect.x, rect.y + 3, rect.width - 5, 20), "", sp_elementlib, 0, 30);
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

            #region 音效库
            List_SoundsLib = new ReorderableList(serializedObject, SoundLibrary)
            {
                draggable = !Application.isPlaying,
                displayAdd = !Application.isPlaying,
                displayRemove = !Application.isPlaying,
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "音效库列表");
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    SerializedProperty sp_Soundlib = SoundLibrary.GetArrayElementAtIndex(index);
                    SerializedProperty player = sp_Soundlib.FindPropertyRelative("Player");
                    SerializedProperty isplaying = sp_Soundlib.FindPropertyRelative("IsPlaying");


                    if (isplaying.boolValue)
                    {
                        GUI.backgroundColor = xHud_Dashboard.Theme_Primary;
                    }
                    else
                    {
                        GUI.backgroundColor = Color.gray;
                    }
                    Editor_xHudGUI.Gui_Icon(new Rect(rect.x + 5, rect.y + 6, 14, 14), soundplaying);
                    GUI.backgroundColor = Color.white;

                    Editor_xHudGUI.Gui_Property_Field(new Rect(rect.x + 28, rect.y + 5, rect.width - 25, 20), player.name, player, 0, 100);
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
            Texture2D[] AllTextures = LoadAllAssetsAtPathWithIO<Texture2D>("SevenStrikeModules/XHud/Textures/BluePrints/", ".png").ToArray();

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


            InitiaGameViewResolutions();

            LoadThemesColor();

            // 创建图层 - XHud
            EnsureLayerExists("XHud");

            // 创建图层 - XHud_World
            EnsureLayerExists("XHud_World");

            //设置XHud管理器图层为XHud
            BaseScript.gameObject.layer = LayerMask.NameToLayer("XHud");

            //启动Led闪烁效果
            Coroutine_LedBlink = EditorCoroutineUtility.StartCoroutine(LedBlinker(), this);

            RatioReference_Update();
        }

        private void EditorApplication_EditorManagerUpdate()
        {
            EditorManagerUpdate();
        }

        private void OnDisable()
        {
            // 目录图标视觉刷新
            HierarachyVisualUpdate();

            BaseScript.hm_BluePrintRootFirstSibling();

            if (FoldAllPanelWithDisabled.boolValue)
                FunctionGroupPrefsSet(false);

            //停止Led闪烁效果
            EditorCoroutineUtility.StopCoroutine(Coroutine_LedBlink);
        }

        private void OnDestroy()
        {
            EditorApplication.hierarchyChanged -= EditorApplication_EditorManagerUpdate;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            Editor_xHudGUI.Gui_Layout_Space(15);

            #region LOGO
            Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_xHudGUI.Gui_Layout_Space(5);
            GUILayout.FlexibleSpace();

            Editor_xHudGUI.Gui_Layout_Labelfield("X   H   U   D", HudFilled.无, HudColor.亮白, Color.white, TextAnchor.MiddleRight, new Vector2(0, 30), 35, Font_Thin);

            Rect rect_logo = GUILayoutUtility.GetLastRect();
            GUILayout.FlexibleSpace();
            Editor_xHudGUI.Gui_Layout_Space(5);
            Editor_xHudGUI.Gui_Layout_Horizontal_End();

            Editor_xHudGUI.Gui_Labelfield_Thin(new Rect(rect_logo.x + 83, rect_logo.height + 85, 60, 20), Ver, HudFilled.无, HudColor.亮白, xHud_Dashboard.Theme_Primary, TextAnchor.MiddleRight, new Vector2(0, 0), 11);
            Editor_xHudGUI.Gui_Labelfield_Thin(new Rect(rect_logo.x + 1, rect_logo.height + 85, 60, 20), Sub, HudFilled.无, HudColor.亮白, Color.gray, TextAnchor.MiddleLeft, new Vector2(0, 0), 12);
            #endregion

            if (IsInitialized.boolValue)
            {
                if (Diagnostic())
                {
                    Rect rect_diagnostic = GUILayoutUtility.GetLastRect();
                    GUI.backgroundColor = Color.red;
                    Editor_xHudGUI.Gui_Icon(new Rect(rect_diagnostic.width - 4, rect_diagnostic.y + 128, 10, 10), status);
                    GUI.backgroundColor = Color.white;
                }
            }

            if (EnabledLedEffect.boolValue)
                LedBlink(rect_logo);

            Editor_xHudGUI.Gui_Layout_Space(90);

            #region 基础选项
            Editor_xHudGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 3, "基础", xHud_Dashboard.Theme_Primary);
            Editor_xHudGUI.Gui_Layout_Space(15);

            Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无);
            Editor_xHudGUI.Gui_Layout_Space(10);

            #region 初始化结构
            if (Application.isPlaying)
                GUI.enabled = false;
            else
                GUI.enabled = true;
            if (Editor_xHudGUI.Gui_Layout_Button(15, "初始化结构", icon_initia_r, icon_initia_p, 0))
            {
                EditorApplication.hierarchyChanged -= EditorApplication_EditorManagerUpdate;
                EditorApplication.hierarchyChanged += EditorApplication_EditorManagerUpdate;

                HudStructure_Create();
                return;
            }
            GUI.enabled = true;
            #endregion

            Editor_xHudGUI.Gui_Layout_FlexSpace();

            #region 生成默认资源库
            if (Application.isPlaying)
                GUI.enabled = false;
            else
                GUI.enabled = true;
            if (Editor_xHudGUI.Gui_Layout_Button(15, "生成默认资源库", icon_fastmaskswitch_r, icon_fastmaskswitch_p, 0))
            {
                string path = EditorUtility.OpenFolderPanel("", Application.dataPath, "");

                if (string.IsNullOrEmpty(path))
                    return;

                string path_folder = path.Substring(Application.dataPath.Length - 6);

                //创建色卡库
                xHud_Library_Colors lib_color = ScriptableObject.CreateInstance<xHud_Library_Colors>();
                lib_color.LibraryName = $"NewColorsLibrary";
                lib_color.ColorsLibrary_AddColor("DefaultColor", Color.white, "这是一个默认示例色卡");

                //创建曲线库
                xHud_Library_Curves lib_curve = ScriptableObject.CreateInstance<xHud_Library_Curves>();
                lib_curve.LibraryName = $"NewCurveLibrary";
                lib_curve.CurveLibrary_AddCurve("DefaultCurve", AnimationCurve.EaseInOut(0, 0, 1, 1));

                //创建音效库
                xHud_Library_Sounds lib_sound = ScriptableObject.CreateInstance<xHud_Library_Sounds>();
                lib_sound.LibraryName = $"NewSoundLibrary";
                lib_sound.SoundLibrary_AddSound("DefaultSound", AssetDatabase.LoadAssetAtPath<AudioClip>($"{xHud_Dashboard.Get_XHudRoot_Path()}Sound/hud_sound_1.wav"));

                //创建元素库
                xHud_Library_Element lib_element = ScriptableObject.CreateInstance<xHud_Library_Element>();
                lib_element.LibraryName = $"NewElementLibrary";

                Transform obj_Corners = AssetDatabase.LoadAssetAtPath<Transform>($"{xHud_Dashboard.Get_XHudRoot_Path()}Prefabs/Corners.prefab");
                Transform obj_Dots = AssetDatabase.LoadAssetAtPath<Transform>($"{xHud_Dashboard.Get_XHudRoot_Path()}Prefabs/Dots.prefab");
                Transform obj_Clicker = AssetDatabase.LoadAssetAtPath<Transform>($"{xHud_Dashboard.Get_XHudRoot_Path()}Prefabs/Clicker.prefab");
                Transform obj_Logo = AssetDatabase.LoadAssetAtPath<Transform>($"{xHud_Dashboard.Get_XHudRoot_Path()}Prefabs/Logo.prefab");

                lib_element.ElementsLibrary_Add(new Library_Item(1, obj_Corners.GetComponent<xHud_Module_Element>()));
                lib_element.ElementsLibrary_Add(new Library_Item(1, obj_Dots.GetComponent<xHud_Module_Element>()));
                lib_element.ElementsLibrary_Add(new Library_Item(1, obj_Clicker.GetComponent<xHud_Module_Element>()));
                lib_element.ElementsLibrary_Add(new Library_Item(1, obj_Logo.GetComponent<xHud_Module_Element>()));

                //创建字体库
                xHud_Library_TextStyle lib_textstyle = ScriptableObject.CreateInstance<xHud_Library_TextStyle>();
                lib_textstyle.LibraryName = $"NewTextStyleLibrary";

                xHud_LibraryArg_TextStyle info_text = new xHud_LibraryArg_TextStyle();
                info_text.Type = xHud_TextType.Text;
                info_text.RichText = true;
                info_text.Maskable = false;
                info_text.Raycast = false;
                info_text.SyncAnimatorColor = false;
                info_text.BestFit = false;
                info_text.GeometreAlign = false;
                info_text.Font = AssetDatabase.LoadAssetAtPath<Font>($"{xHud_Dashboard.Get_XHudRoot_Path()}Fonts/Text/SevenBlack-Light.ttf");
                info_text.FontColor = Color.white;
                info_text.Size = 50;
                info_text.LineHeight = 1;
                info_text.Name = "NewTextStyle_HudText";
                info_text.Description = "这是一个用于HudText文字组件的默认字体样式";
                info_text.Style = FontStyle.Normal;
                info_text.Overflow_Horizon = HorizontalWrapMode.Overflow;
                info_text.Overflow_Vertical = VerticalWrapMode.Overflow;
                info_text.ContentAnchor = ContentAnchor.中心;

                xHud_LibraryArg_TextStyle info_tmptext = new xHud_LibraryArg_TextStyle();
                info_tmptext.Type = xHud_TextType.TmpText;
                info_tmptext.tmp_rich = true;
                info_tmptext.Maskable = false;
                info_tmptext.Raycast = false;
                info_tmptext.SyncAnimatorColor = false;
                info_tmptext.tmp_EnableAutoSizing = false;
                info_tmptext.tmp_font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>($"{xHud_Dashboard.Get_XHudRoot_Path()}Fonts/Tmp/SevenBlack-Light SDF.asset");
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
                xHud_Library_Motion lib_motion = ScriptableObject.CreateInstance<xHud_Library_Motion>();
                lib_motion.LibraryName = $"NewMotionLibrary";

                #region 预制动效
                Motion_Creator crc = new Motion_Creator();
                crc.anchor = HudAnchor.中心;
                crc.Movement = new MotionNode_Movement();
                crc.Movement.Movement = HudMotion_Movement.S_从下至上;
                crc.Movement.Distance = 100f;
                crc.Movement.Duration = 1f;
                crc.Movement.Delay = 0f;
                crc.Movement.CurveName = "";
                crc.Movement.Curve = xHud_Dashboard.HudManagerGet().Hud_Curves != null ? xHud_Dashboard.HudManagerGet().Hud_Curves.CurveLibrary_GetCurve(crc.Movement.CurveName) : null;
                crc.Movement.Ease = Ease.OutQuart;
                crc.Rotation = new MotionNode_Rotation();
                crc.Rotation.Rotation = HudMotion_Rotation.A_无旋转;
                crc.Rotation.Degree = 0f;
                crc.Rotation.Duration = 1f;
                crc.Rotation.Delay = 0f;
                crc.Rotation.CurveName = "";
                crc.Rotation.Curve = xHud_Dashboard.HudManagerGet().Hud_Curves != null ? xHud_Dashboard.HudManagerGet().Hud_Curves.CurveLibrary_GetCurve(crc.Rotation.CurveName) : null;
                crc.Rotation.Ease = Ease.OutQuart;
                crc.Alpha = new MotionNode_Alpha();
                crc.Alpha.Duration = 1f;
                crc.Alpha.Delay = 0f;
                crc.Alpha.CurveName = "";
                crc.Alpha.Curve = xHud_Dashboard.HudManagerGet().Hud_Curves != null ? xHud_Dashboard.HudManagerGet().Hud_Curves.CurveLibrary_GetCurve(crc.Alpha.CurveName) : null;
                crc.Alpha.Ease = Ease.OutQuart;

                Motion_Recycler rec = new Motion_Recycler();
                rec.Movement = new MotionNode_Movement();
                rec.Movement.Movement = HudMotion_Movement.D_从上至下;
                rec.Movement.Distance = 100f;
                rec.Movement.Duration = 1f;
                rec.Movement.Delay = 0f;
                rec.Movement.CurveName = "";
                rec.Movement.Curve = xHud_Dashboard.HudManagerGet().Hud_Curves != null ? xHud_Dashboard.HudManagerGet().Hud_Curves.CurveLibrary_GetCurve(rec.Movement.CurveName) : null;
                rec.Movement.Ease = Ease.OutQuart;
                rec.Rotation = new MotionNode_Rotation();
                rec.Rotation.Rotation = HudMotion_Rotation.A_无旋转;
                rec.Rotation.Degree = 0f;
                rec.Rotation.Duration = 1f;
                rec.Rotation.Delay = 0f;
                rec.Rotation.CurveName = "";
                rec.Rotation.Curve = xHud_Dashboard.HudManagerGet().Hud_Curves != null ? xHud_Dashboard.HudManagerGet().Hud_Curves.CurveLibrary_GetCurve(rec.Rotation.CurveName) : null;
                rec.Rotation.Ease = Ease.OutQuart;
                rec.Alpha = new MotionNode_Alpha();
                rec.Alpha.Duration = 1f;
                rec.Alpha.Delay = 0f;
                rec.Alpha.CurveName = "";
                rec.Alpha.Curve = xHud_Dashboard.HudManagerGet().Hud_Curves != null ? xHud_Dashboard.HudManagerGet().Hud_Curves.CurveLibrary_GetCurve(rec.Alpha.CurveName) : null;
                rec.Alpha.Ease = Ease.OutQuart;
                #endregion

                XHud.xHud_LibraryArg_Motion mot_crc = new XHud.xHud_LibraryArg_Motion();
                mot_crc.Name = "Motion_Element_Create";
                mot_crc.Des = "这是一个用于元素载入时的动效";
                mot_crc.Mode = 0;
                mot_crc.Crc = crc;
                mot_crc.Rec = rec;

                XHud.xHud_LibraryArg_Motion mot_rec = new XHud.xHud_LibraryArg_Motion();
                mot_rec.Name = "Motion_Element_Recycle";
                mot_rec.Des = "这是一个用于元素回收时的动效";
                mot_rec.Mode = 1;
                mot_rec.Crc = crc;
                mot_rec.Rec = rec;

                lib_motion.ElementMotion_Add(mot_crc);
                lib_motion.ElementMotion_Add(mot_rec);

                //创建转场库
                xHud_Library_Transition lib_transition = ScriptableObject.CreateInstance<xHud_Library_Transition>();
                lib_transition.LibraryName = $"NewTransitionLibrary";
                List<Texture2D> texs_list = LoadAllAssetsAtPathWithIO<Texture2D>("SevenStrikeModules/XHud/Textures/Transitions/Ink/", ".jpg");
                // 调用排序方法
                SortTexturesByNameSuffix(texs_list);
                lib_transition.TransitionLibrary_Add("InkPaint", texs_list.ToArray(), 1);
                xHud_LibraryArg_Transition node = lib_transition.TransitionLibrary_Get("InkPaint");
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

                string res = Editor_xHudGUI.Open(xHudDialogType.警告, "XHud管理器消息", "资源库引用", "是否要将创建的资源库全部指定到管理器中？", "暂不", "指定", 0);
                if (res == "指定")
                {
                    xHud_Dashboard.HudManagerGet().Hud_Colors = lib_color;
                    xHud_Dashboard.HudManagerGet().Hud_Curves = lib_curve;
                    xHud_Dashboard.HudManagerGet().Hud_Sounds = lib_sound;
                    xHud_Dashboard.HudManagerGet().Hud_TextStyleLibrary = lib_textstyle;
                    xHud_Dashboard.HudManagerGet().Hud_ElementMotion = lib_motion;
                    xHud_Dashboard.HudManagerGet().Hud_TransitionLib = lib_transition;
                    xHud_Dashboard.HudManagerGet().Hud_ElementLibrarys.Add(lib_element);
                }

                return;
            }
            GUI.enabled = true;
            #endregion

            Editor_xHudGUI.Gui_Layout_FlexSpace();

            #region 辅助视觉切换
            if (Application.isPlaying)
                GUI.enabled = false;
            else
                GUI.enabled = true;
            if (Editor_xHudGUI.Gui_Layout_Button(15, "辅助视觉切换", icon_safeframevisual_r, icon_safeframevisual_p, 0))
            {
                if (SceneCamera.objectReferenceValue == null)
                {
                    Editor_xHudGUI.Open(xHudDialogType.警告, "XHud管理器消息", "辅助视觉", "当前未指定场景相机，无法启用辅助视觉！！", "明白", 0);
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
                return;
            }
            GUI.enabled = true;
            #endregion

            Editor_xHudGUI.Gui_Layout_FlexSpace();

            #region 检查配置状态
            if (Application.isPlaying)
                GUI.enabled = false;
            else
                GUI.enabled = true;
            if (Editor_xHudGUI.Gui_Layout_Button(15, "检查配置状态", icon_checkstate_r, icon_checkstate_p, 0))
            {
                Editor_xHud_Tool_Diagnostic window = (Editor_xHud_Tool_Diagnostic)EditorWindow.GetWindow(typeof(Editor_xHud_Tool_Diagnostic), true, "XHUD 诊断/概况面板", true);
                Editor_xHudGUI.CenterEditorWindow(new Vector2Int(1000, 765), window);
                window.Show();
                return;
            }
            GUI.enabled = true;
            #endregion

            Editor_xHudGUI.Gui_Layout_Space(10);
            Editor_xHudGUI.Gui_Layout_Horizontal_End();

            Editor_xHudGUI.Gui_Layout_Space(10);
            Editor_xHudGUI.Gui_Layout_Vertical_End();
            #endregion

            if (IsInitialized.boolValue)
            {
                #region 分辨率
                Editor_xHudGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 3, "分辨率", xHud_Dashboard.Theme_Primary);
                Editor_xHudGUI.Gui_Layout_Space(5);

                #region 屏幕尺寸
                Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_xHudGUI.Gui_Layout_Space(10);
                string resinfo = string.Format("{0} x {1}", ScreenRes.vector2Value.x, ScreenRes.vector2Value.y);
                Editor_xHudGUI.Gui_Layout_Labelfield(resinfo, HudFilled.无, HudColor.亮白, xHud_Dashboard.Theme_Primary, TextAnchor.MiddleCenter, 11);
                if (RMS_Nodes.arraySize > 0)
                {
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    Editor_xHudGUI.Gui_Layout_Labelfield("RMS： " + RMS_CurrentSolution.stringValue, HudFilled.无, HudColor.亮白, xHud_Dashboard.Theme_Primary, TextAnchor.MiddleCenter, 11);
                }
                Editor_xHudGUI.Gui_Layout_Space(5);
                Editor_xHudGUI.Gui_Layout_Horizontal_End();
                #endregion

                Editor_xHudGUI.Gui_Layout_Space(5);
                Editor_xHudGUI.Gui_Layout_Vertical_End();
                #endregion

                #region 选项
                bool sw_option = FunctionGroup("选项", 5, HudFilled.纯色边框, HudColor.亮白, xHud_Dashboard.Theme_Primary, xHud_Dashboard.Theme_Primary, Color.gray, new RectOffset(0, 0, 0, 0), new Vector2(20, 0), PrefsKeyFold_Option, panel_option);
                if (sw_option)
                {
                    #region 单例模式
                    Editor_xHudGUI.Gui_Layout_Toggle<bool, xHud_Manager>("单例模式", stroptions_enabled, ref UseInstanceMode, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                    #endregion

                    #region 调试模式
                    Editor_xHudGUI.Gui_Layout_Toggle<bool, xHud_Manager>("调试模式", stroptions_debug, ref UseDebug, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                    #endregion

                    #region 辅助结构
                    Editor_xHudGUI.Gui_Layout_Toggle<bool, xHud_Manager>("辅助结构", stroptions_hide, ref SafeFrameStructureDisplayer, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                    #endregion

                    Editor_xHudGUI.Gui_Layout_Seperator(1, xHud_Dashboard.Theme_SeperateLine);

                    #region 辅助视觉
                    if (SceneCamera.objectReferenceValue != null)
                    {
                        EditorGUI.BeginChangeCheck();
                        Editor_xHudGUI.Gui_Layout_Toggle<bool, xHud_Manager>("辅助视觉", stroptions_enabled, ref UseSafeFrame, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                        if (EditorGUI.EndChangeCheck())
                        {
                            if (SceneCamera.objectReferenceValue == null)
                            {
                                UseSafeFrame.boolValue = false;
                                UseSafeFrame.serializedObject.ApplyModifiedProperties();
                                Editor_xHudGUI.Open(xHudDialogType.警告, "XHud管理器消息", "辅助视觉", "当前未指定场景相机，无法启用辅助视觉！！", "明白", 0);
                                return;
                            }
                            if (!UseSafeFrame.boolValue)
                            {
                                BaseScript.hm_AnchorMarks_Destroy();
                            }
                            else
                            {
                                BaseScript.hm_AnchorMarks_Create();
                            }
                        }
                    }
                    #endregion

                    #region 构图参考视觉
                    if (SceneCamera.objectReferenceValue != null)
                    {
                        EditorGUI.BeginChangeCheck();
                        Editor_xHudGUI.Gui_Layout_Toggle<bool, xHud_Manager>("构图参考视觉", stroptions_enabled, ref UseCompGuide, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                        if (EditorGUI.EndChangeCheck())
                        {
                            if (UseCompGuide.boolValue)
                            {
                                if (CompGuide_AnchorRoot.objectReferenceValue != null)
                                {
                                    return;
                                }
                                CompGuide_Structure_Create();
                            }
                            else
                            {
                                if (CompGuide_AnchorRoot.objectReferenceValue == null)
                                {
                                    return;
                                }
                                CompGuide_Structure_Destroy();
                            }
                        }
                    }
                    #endregion

                    #region 蓝图视觉
                    EditorGUI.BeginChangeCheck();
                    Editor_xHudGUI.Gui_Layout_Toggle<bool, xHud_Manager>("蓝图视觉", stroptions_enabled, ref BluePrintMode, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (!BluePrintMode.boolValue)
                        {
                            string res = Editor_xHudGUI.Open(xHudDialogType.警告, "XHud管理器消息", "蓝图模式", "此操作会退出蓝图模式，请确实是否继续该操作？", "退出", "暂不", 0);
                            if (res == "退出")
                            {
                                BluePrintMode.boolValue = false;
                                BluePrintMode.serializedObject.ApplyModifiedProperties();
                                BaseScript.hm_BluePrint_Remove();
                            }
                            else
                            {
                                BluePrintMode.boolValue = true;
                                BluePrintMode.serializedObject.ApplyModifiedProperties();
                                return;
                            }
                        }
                        else
                        {
                            Material mat = AssetDatabase.LoadAssetAtPath<Material>($"{xHud_Dashboard.Get_XHudRoot_Path()}Materials/BluePrints/BluePrint.mat");
                            TMP_FontAsset title = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>($"{xHud_Dashboard.Get_XHudRoot_Path()}Fonts/Tmp/SevenBlack-Bold SDF.asset");
                            TMP_FontAsset subtitle = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>($"{xHud_Dashboard.Get_XHudRoot_Path()}Fonts/Tmp/SevenBlack-Light SDF.asset");
                            #region 指定叠加背景
                            if (string.IsNullOrEmpty(BluePrint_bg_name.stringValue))
                                BluePrint_bg_name.stringValue = BluePrintBgs_Name[BluePrintBgs_Name.Length - 1];
                            BluePrint_bg_name.serializedObject.ApplyModifiedProperties();
                            BaseScript.hm_BluePrint_Create(true, mat, title, subtitle);

                            #endregion

                            return;
                        }
                    }
                    #endregion

                    #region Led闪烁效果
                    Editor_xHudGUI.Gui_Layout_Toggle<bool, xHud_Manager>("LED 星闪特效", stroptions_enabled, ref EnabledLedEffect, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                    #endregion

                    #region 关闭面板后是否折叠所有选项卡
                    Editor_xHudGUI.Gui_Layout_Toggle<bool, xHud_Manager>("关闭时折叠所有", stroptions_enabled, ref FoldAllPanelWithDisabled, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                    #endregion

                    Editor_xHudGUI.Gui_Layout_Seperator(1, xHud_Dashboard.Theme_SeperateLine);

                    #region 像素对齐
                    EditorGUI.BeginChangeCheck();
                    Editor_xHudGUI.Gui_Layout_Toggle<bool, xHud_Manager>("像素对齐", stroptions_align, ref UsePerfectPixelUpdate, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Canvas canvas = (Canvas)HudCanvas_Screen.objectReferenceValue;
                        canvas.pixelPerfect = UsePerfectPixelUpdate.boolValue;
                    }
                    #endregion

                    #region 自动像素对齐
                    Editor_xHudGUI.Gui_Layout_Toggle<bool, xHud_Manager>("自动像素对齐", stroptions_useful, ref UseAutoPerfectPixel, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                    #endregion

                    Editor_xHudGUI.Gui_Layout_Seperator(1, xHud_Dashboard.Theme_SeperateLine);

                    #region 相机投影
                    EditorGUI.BeginChangeCheck();
                    Editor_xHudGUI.Gui_Layout_Toggle<bool, xHud_Manager>("相机投影", stroptions_perspective, ref CameraOthograpicMode, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Camera cam = (Camera)HudCamera.objectReferenceValue;
                        cam.orthographic = CameraOthograpicMode.boolValue;
                    }
                    #endregion

                    #region 音效静音
                    Editor_xHudGUI.Gui_Layout_Toggle<bool, xHud_Manager>("音效静音", stroptions_mute, ref VolumeMute, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                    #endregion                   

                    #region 自定光标
                    EditorGUI.BeginChangeCheck();
                    Editor_xHudGUI.Gui_Layout_Toggle<bool, xHud_Manager>("个性光标", stroptions_mouse, ref CustomCursor, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (!CustomCursor.boolValue)
                        {
                            if (Hud_MouseCursor.objectReferenceValue != null)
                            {
                                xHud_CustomMouseCursor mouseCursor = (xHud_CustomMouseCursor)Hud_MouseCursor.objectReferenceValue;

                                string res = Editor_xHudGUI.Open(xHudDialogType.警告, "XHud管理器消息", "自定义光标", "当前已经存在自定义鼠标样式器，如果选择禁用，那么将会立即移除它，并且您为其配置好的参数也将一并丢失，确认要这样操作吗？", "是的", "暂不", 0);
                                if (res == "是的")
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
                            xHud_CustomMouseCursor cursor = MouseStyler.AddComponent<xHud_CustomMouseCursor>();
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
                    }
                    #endregion

                    #region 转场特效
                    EditorGUI.BeginChangeCheck();
                    Editor_xHudGUI.Gui_Layout_Toggle<bool, xHud_Manager>("转场特效", stroptions_enabled, ref CustomTransition, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (!CustomTransition.boolValue)
                        {
                            if (Hud_TransitionController.objectReferenceValue != null)
                            {
                                xHud_TransitionController transition = (xHud_TransitionController)Hud_TransitionController.objectReferenceValue;

                                string res = Editor_xHudGUI.Open(xHudDialogType.警告, "XHud管理器消息", "转场器", "当前已经存在转场器，如果选择禁用，那么将会立即移除它，并且您为其配置好的参数也将一并丢失，确认要这样操作吗？", "禁用", "暂不", 0);
                                if (res == "禁用")
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
                            xHud_TransitionController trans = NewTransition.AddComponent<xHud_TransitionController>();

                            UnityEngine.RectTransform rect = NewTransition.AddComponent<UnityEngine.RectTransform>();
                            rect.SetParent(canvas.transform);
                            rect.SetAsLastSibling();
                            rect.localScale = Vector3.one;
                            rect.anchorMin = Vector2.zero;
                            rect.anchorMax = Vector2.one;
                            rect.pivot = Vector2.one * 0.5f;
                            rect.sizeDelta = Vector2.one * 4;
                            rect.anchoredPosition3D = Vector3.zero;

                            Material mat = AssetDatabase.LoadAssetAtPath<Material>($"{xHud_Dashboard.Get_XHudRoot_Path()}Materials/Transitions/Transition.mat");
                            Image img = NewTransition.AddComponent<Image>();
                            img.raycastTarget = false;
                            img.color = Color.white;
                            img.material = mat;

                            trans.TransitionMat = mat;
                            trans.TransitionImage = img;
                            Hud_TransitionController.objectReferenceValue = trans;
                        }
                    }
                    #endregion

                    Editor_xHudGUI.Gui_Layout_Seperator(1, xHud_Dashboard.Theme_SeperateLine);

                    #region 支持世界元素
                    EditorGUI.BeginChangeCheck();
                    Editor_xHudGUI.Gui_Layout_Toggle<bool, xHud_Manager>("支持世界元素", stroptions_world, ref SupportWorldUI, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (!SupportWorldUI.boolValue)
                        {
                            if (HudCanvas_World.objectReferenceValue == null)
                                return;
                            string res = Editor_xHudGUI.Open(xHudDialogType.警告, "XHud管理器消息", "支持世界元素", "此操作会移除World节点以及其下所有物体并禁用支持世界UI的特性，请确实是否继续该操作？", "继续", "暂不", 0);
                            if (res == "继续")
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
                    }
                    #endregion

                    #region RMS
                    Editor_xHudGUI.Gui_Layout_Toggle<bool, xHud_Manager>("R M S 模块", stroptions_enabled, ref RMS_Enabled, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                    #endregion

                    Editor_xHudGUI.Gui_Layout_Seperator(1, xHud_Dashboard.Theme_SeperateLine);

                    Editor_xHudGUI.Gui_Layout_Space(5);

                    #region 画布尺寸
                    EditorGUI.BeginChangeCheck();
                    Editor_xHudGUI.Gui_Layout_Popup<int, xHud_Manager>("画布尺寸", stroptions_canvassize, ref CanvasScalerModeIndex, HudFilled.实体, 120, 22, SelectedObjects);
                    if (EditorGUI.EndChangeCheck())
                    {
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
                    }
                    #endregion

                    #region 画布距离
                    EditorGUI.BeginChangeCheck();
                    Editor_xHudGUI.Gui_Layout_Popup<int, xHud_Manager>("画布距离", stroptions_canvasanchor, ref HudCanvasAnchorIndex, HudFilled.实体, 120, 22, SelectedObjects);
                    if (EditorGUI.EndChangeCheck())
                    {
                        HudCanvasAnchor.enumValueIndex = HudCanvasAnchorIndex.intValue;
                    }
                    #endregion
                }
                Editor_xHudGUI.Gui_Layout_Space(5);
                Editor_xHudGUI.Gui_Layout_Vertical_End();
                #endregion

                #region 画布设定
                bool sw_canvascamera = FunctionGroup("画布", 5, HudFilled.纯色边框, HudColor.亮白, xHud_Dashboard.Theme_Primary, xHud_Dashboard.Theme_Primary, Color.gray, new RectOffset(0, 0, 0, 0), new Vector2(20, 5), PrefsKeyFold_CanvasCamera, panel_canvas);
                if (sw_canvascamera)
                {
                    Editor_xHudGUI.Gui_Layout_Space(5);

                    #region 画布尺寸     
                    EditorGUI.BeginChangeCheck();
                    Editor_xHudGUI.Gui_Layout_Property_Field("画布尺寸", CanvasScalerScreenSize, 20, true);
                    if (EditorGUI.EndChangeCheck())
                    {
                        CanvasScaler scaler = (CanvasScaler)HudCanvasScaler.objectReferenceValue;
                        scaler.referenceResolution = CanvasScalerScreenSize.vector2Value;
                        ScreenRes.vector2Value = CanvasScalerScreenSize.vector2Value;
                    }
                    #endregion

                    Editor_xHudGUI.Gui_Layout_Space(5);

                    #region 高度优先     
                    EditorGUI.BeginChangeCheck();
                    Editor_xHudGUI.Gui_Layout_Property_Field("高度优先", CanvasMatchDir);
                    if (EditorGUI.EndChangeCheck())
                    {
                        CanvasScaler scaler = (CanvasScaler)HudCanvasScaler.objectReferenceValue;
                        scaler.matchWidthOrHeight = CanvasMatchDir.floatValue;
                    }
                    #endregion

                    if (HudCanvasAnchorIndex.intValue == 2)
                    {
                        Editor_xHudGUI.Gui_Layout_Space(5);

                        #region 画布距离     
                        Editor_xHudGUI.Gui_Layout_Property_Field("画布距离", CanvasDistance);
                        #endregion
                    }

                    Editor_xHudGUI.Gui_Layout_Space(5);

                    if (Editor_xHudGUI.Gui_Layout_Button("获取分辨率", "", HudFilled.实体, HudColor.深空灰, xHud_Dashboard.Theme_Primary, 25))
                    {
                        CanvasScalerScreenSize.vector2Value = GetMainGameViewSize();
                        CanvasScalerScreenSize.serializedObject.ApplyModifiedProperties();

                        CanvasScaler scaler = (CanvasScaler)HudCanvasScaler.objectReferenceValue;
                        scaler.referenceResolution = CanvasScalerScreenSize.vector2Value;

                        ScreenRes.vector2Value = CanvasScalerScreenSize.vector2Value;

                        RatioReference_Update();
                    }
                }
                Editor_xHudGUI.Gui_Layout_Space(10);
                Editor_xHudGUI.Gui_Layout_Vertical_End();
                #endregion

                #region 相机设定

                bool sw_camera = FunctionGroup("相机", 5, HudFilled.纯色边框, HudColor.亮白, xHud_Dashboard.Theme_Primary, xHud_Dashboard.Theme_Primary, Color.gray, new RectOffset(0, 0, 0, 0), new Vector2(20, 5), PrefsKeyFold_Camera, panel_camera);
                if (sw_camera)
                {
                    Editor_xHudGUI.Gui_Layout_Space(5);

                    #region 正交尺寸                                             
                    Editor_xHudGUI.Gui_Layout_Property_Field("正交尺寸", CameraOrthographicSize);
                    #endregion

                    Editor_xHudGUI.Gui_Layout_Space(5);

                    #region 近距剪切                                             
                    Editor_xHudGUI.Gui_Layout_Property_Field("近距剪切", CameraCutter_Near);
                    #endregion

                    Editor_xHudGUI.Gui_Layout_Space(5);

                    #region 远距剪切                                             
                    Editor_xHudGUI.Gui_Layout_Property_Field("远距剪切", CameraCutter_Far);
                    #endregion

                    if (!CameraOthograpicMode.boolValue)
                    {
                        #region 视场角                                                             
                        Editor_xHudGUI.Gui_Layout_Property_Field("视场角", CameraFov);
                        #endregion
                    }

                    if (HudCanvasAnchorIndex.intValue == 2)
                    {
                        #region 画布距离                                                     
                        Editor_xHudGUI.Gui_Layout_Property_Field("画布距离", CanvasDistance);
                        #endregion
                    }

                    Editor_xHudGUI.Gui_Layout_Space(5);

                    #region 场景相机     

                    if (SceneCamera.objectReferenceValue == null)
                        EditorGUILayout.HelpBox("请指定场景主相机", MessageType.Error);

                    EditorGUI.BeginChangeCheck();
                    Editor_xHudGUI.StatuDisplayer_Object(null, 12, new Vector2(0, 1), "场景相机", 12, new Vector2(0, -7), status, new Vector2(0, 3), SceneCamera.objectReferenceValue == null ? false : true, xHud_Dashboard.Theme_Primary, Color.black * 0.7f, SceneCamera, false);

                    if (!Application.isPlaying)
                    {
                        if (EditorGUI.EndChangeCheck())
                        {
                            Camera cam = (Camera)SceneCamera.objectReferenceValue;
                            string res = Editor_xHudGUI.Open(xHudDialogType.警告, "XHud管理器消息", "指定场景相机", $"确认要将 {cam.name} 相机指定为XHud的主场景相机吗？ 如果不指定场景主相机则您将无法正常使用XHud！", "暂不", "指定", 0);
                            if (res == "指定")
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

                                Editor_xHudGUI.Open(xHudDialogType.确认, "XHud管理器消息", "指定场景相机", "已取消勾选主场景相机的 CullingMask 中的 'XHud' 图层！", "明白");
                            }
                            else
                            {
                                SceneCamera.objectReferenceValue = null;
                                SceneCamera.serializedObject.ApplyModifiedProperties();
                            }
                            return;
                        }
                    }

                    Editor_xHudGUI.Gui_Layout_Space(5);

                    #endregion
                }
                Editor_xHudGUI.Gui_Layout_Space(5);
                Editor_xHudGUI.Gui_Layout_Vertical_End();
                #endregion

                #region 比例参考
                bool sw_referratio = FunctionGroup("比例参考", 5, HudFilled.纯色边框, HudColor.亮白, xHud_Dashboard.Theme_Primary, xHud_Dashboard.Theme_Primary, Color.gray, new RectOffset(0, 0, 0, 0), new Vector2(20, 0), PrefsKeyFold_PhysicsRatio, panel_physicsratio);
                if (sw_referratio)
                {
                    EditorGUI.BeginChangeCheck();
                    Editor_xHudGUI.Gui_Layout_Toggle<bool, xHud_Manager>("比例参考", stroptions_useful, ref UseRatioReference, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                    if (EditorGUI.EndChangeCheck())
                    {
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

                            RatioReference_Create(spr);
                        }
                        else
                        {
                            RatioReference_Destroy();
                        }
                    }

                    EditorGUI.BeginChangeCheck();
                    Editor_xHudGUI.Gui_Layout_Toggle<bool, xHud_Manager>("比例类型", stroptions_refertype, ref RatioReferenceIsPart, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                    if (EditorGUI.EndChangeCheck())
                    {
                        RatioReference_Switch();
                        RatioReference_Update();
                    }

                    Editor_xHudGUI.Gui_Layout_Space(5);
                    Editor_xHudGUI.Gui_Layout_Seperator(1, xHud_Dashboard.Theme_SeperateLine);
                    Editor_xHudGUI.Gui_Layout_Space(10);
                    EditorGUI.BeginChangeCheck();
                    Editor_xHudGUI.Gui_Layout_Property_Field("物理屏幕可视尺寸 (单位 cm)", sp_PhysicsScreenSize, 20, true);

                    Editor_xHudGUI.Gui_Layout_Space(5);

                    Editor_xHudGUI.Gui_Layout_Property_Field("参考物颜色", sp_Reference_Image_Color, 100);


                    Editor_xHudGUI.Gui_Layout_Space(5);
                    Editor_xHudGUI.Gui_Layout_Seperator(1, xHud_Dashboard.Theme_SeperateLine);
                    Editor_xHudGUI.Gui_Layout_Space(10);

                    Editor_xHudGUI.Gui_Layout_Property_Field("参考物 - 整体 (单位 cm)", sp_Res_Full, 100);

                    Editor_xHudGUI.Gui_Layout_Space(5);

                    Editor_xHudGUI.Gui_Layout_Property_Field("参考物 - 局部 (单位 cm)", sp_Res_Part, 100);

                    if (EditorGUI.EndChangeCheck())
                    {
                        RatioReference_Switch();
                        RatioReference_Update();
                    }

                    Editor_xHudGUI.Gui_Layout_Space(10);

                    if (Editor_xHudGUI.Gui_Layout_Button("更新参考比例视觉", "", HudFilled.实体, HudColor.深空灰, xHud_Dashboard.Theme_Primary, 30))
                    {
                        RatioReference_Update();
                        RatioReference_Switch();
                        return;
                    }

                    Editor_xHudGUI.Gui_Layout_Space(5);
                    Editor_xHudGUI.Gui_Layout_Seperator(1, xHud_Dashboard.Theme_SeperateLine);
                    Editor_xHudGUI.Gui_Layout_Space(10);

                    Editor_xHudGUI.Gui_Layout_Property_Field("比例参考图形 - 尺寸", sp_ReferShape_RatioSize, 20, true);

                    Editor_xHudGUI.Gui_Layout_Space(5);

                    Editor_xHudGUI.Gui_Layout_Property_Field("比例参考图形 - 公差", sp_ReferShape_RatioTolerance, 20, true);

                    Editor_xHudGUI.Gui_Layout_Space(10);

                    if (Editor_xHudGUI.Gui_Layout_Button("创建比例参考图形", "", HudFilled.实体, HudColor.深空灰, xHud_Dashboard.Theme_Primary, 30))
                    {
                        CreateReferenceShape(sp_ReferShape_RatioSize.vector2Value.x, sp_ReferShape_RatioSize.vector2Value.y, sp_ReferShape_RatioTolerance.vector2Value.x, sp_ReferShape_RatioTolerance.vector2Value.y);
                        return;
                    }
                }
                Editor_xHudGUI.Gui_Layout_Space(10);
                Editor_xHudGUI.Gui_Layout_Vertical_End();
                #endregion

                #region 构图参考
                if (UseCompGuide.boolValue)
                {
                    bool sw_CompGuide = FunctionGroup("构图参考", 5, HudFilled.纯色边框, HudColor.亮白, xHud_Dashboard.Theme_Primary, xHud_Dashboard.Theme_Primary, Color.gray, new RectOffset(0, 0, 0, 0), new Vector2(20, 0), PrefsKeyFold_Comp_Guide, panel_comp_guide);
                    if (sw_CompGuide)
                    {
                        Editor_xHudGUI.Gui_Layout_Property_Field("构图辅助线颜色", GuideColor, 90);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        Editor_xHudGUI.Gui_Layout_Property_Field("构图辅助点颜色", GuidePointColor, 90);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无);
                        string[] modes = new string[8] { "水平对称", "垂直对称", "黄金螺旋", "对角线", "三分线", "引导线", "三角", "工字型" };
                        Editor_xHudGUI.Gui_Layout_Popup<string, xHud_Manager>("构图模式", modes, ref CompGuideMode, HudFilled.实体, 100, 22, SelectedObjects);
                        Editor_xHudGUI.Gui_Layout_Space(5);
                        if (Editor_xHudGUI.Gui_Layout_Button(15, "", GuideHelp_Released, GuideHelp_Press, 0))
                        {
                            OpenGuideHelpPanel(CompGuideMode.stringValue);
                        }
                        Editor_xHudGUI.Gui_Layout_Horizontal_End(5);

                        switch (CompGuideMode.stringValue)
                        {
                            case "水平对称":
                                Editor_xHudGUI.Gui_Layout_Space(5);
                                string[] dir_lr = new string[3] { "自定义对称分割", "靠右黄金比例", "靠左黄金比例" };
                                string mode_lr = Editor_xHudGUI.Gui_Layout_Popup<string, xHud_Manager>("对称方式", dir_lr, ref GuideParam_Mirror_LR_GoldenMode, HudFilled.实体, 100, 22, SelectedObjects);

                                if (mode_lr == "自定义对称分割")
                                {
                                    Editor_xHudGUI.Gui_Layout_Space(5);
                                    Editor_xHudGUI.Gui_Layout_Property_Field("对称偏移校正", GuideParam_Mirror_LR_Offset, 90);
                                }

                                Editor_xHudGUI.Gui_Layout_Space(5);

                                Editor_xHudGUI.Gui_Layout_Property_Field("中心点尺寸", GuideParam_CenterPointSize, 90);

                                Editor_xHudGUI.Gui_Layout_Space(5);

                                if (Editor_xHudGUI.Gui_Layout_Button("复位校正", "", HudFilled.实体, HudColor.深空灰, xHud_Dashboard.Theme_Primary, 25))
                                {
                                    GuideParam_Mirror_LR_GoldenMode.stringValue = "自定义对称分割";
                                    GuideParam_Mirror_LR_GoldenMode.serializedObject.ApplyModifiedProperties();

                                    GuideParam_Mirror_LR_Offset.floatValue = 0;
                                    GuideParam_Mirror_LR_Offset.serializedObject.ApplyModifiedProperties();

                                    GuideParam_CenterPointSize.floatValue = 1;
                                    GuideParam_CenterPointSize.serializedObject.ApplyModifiedProperties();
                                }
                                break;
                            case "垂直对称":
                                Editor_xHudGUI.Gui_Layout_Space(5);
                                string[] dir_ud = new string[3] { "自定义对称分割", "靠上黄金比例", "靠下黄金比例" };
                                string mode_ud = Editor_xHudGUI.Gui_Layout_Popup<string, xHud_Manager>("对称方式", dir_ud, ref GuideParam_Mirror_UD_GoldenMode, HudFilled.实体, 100, 22, SelectedObjects);

                                if (mode_ud == "自定义对称分割")
                                {
                                    Editor_xHudGUI.Gui_Layout_Space(5);
                                    Editor_xHudGUI.Gui_Layout_Property_Field("对称偏移校正", GuideParam_Mirror_UD_Offset, 90);
                                }

                                Editor_xHudGUI.Gui_Layout_Space(5);

                                Editor_xHudGUI.Gui_Layout_Property_Field("中心点尺寸", GuideParam_CenterPointSize, 90);

                                Editor_xHudGUI.Gui_Layout_Space(5);

                                if (Editor_xHudGUI.Gui_Layout_Button("复位校正", "", HudFilled.实体, HudColor.深空灰, xHud_Dashboard.Theme_Primary, 25))
                                {
                                    GuideParam_Mirror_UD_GoldenMode.stringValue = "自定义对称分割";
                                    GuideParam_Mirror_UD_GoldenMode.serializedObject.ApplyModifiedProperties();

                                    GuideParam_Mirror_UD_Offset.floatValue = 0;
                                    GuideParam_Mirror_UD_Offset.serializedObject.ApplyModifiedProperties();

                                    GuideParam_CenterPointSize.floatValue = 1;
                                    GuideParam_CenterPointSize.serializedObject.ApplyModifiedProperties();
                                }
                                break;
                            case "黄金螺旋":
                                Editor_xHudGUI.Gui_Layout_Space(5);
                                string[] dir_fb = new string[4] { "右上", "左上", "右下", "左下" };
                                string mode_fb = Editor_xHudGUI.Gui_Layout_Popup<string, xHud_Manager>("对称方式", dir_fb, ref GuideParam_Fibonacci_Mode, HudFilled.实体, 100, 22, SelectedObjects);

                                Editor_xHudGUI.Gui_Layout_Space(5);

                                if (Editor_xHudGUI.Gui_Layout_Button("复位校正", "", HudFilled.实体, HudColor.深空灰, xHud_Dashboard.Theme_Primary, 25))
                                {
                                    GuideParam_Fibonacci_Mode.stringValue = "右上";
                                    GuideParam_Fibonacci_Mode.serializedObject.ApplyModifiedProperties();
                                }
                                break;
                            case "对角线":
                                Editor_xHudGUI.Gui_Layout_Space(5);
                                Editor_xHudGUI.Gui_Layout_Property_Field("水平偏移", GuideParam_CornerLookat_Offset_H, 90);

                                Editor_xHudGUI.Gui_Layout_Space(5);

                                Editor_xHudGUI.Gui_Layout_Property_Field("垂直偏移", GuideParam_CornerLookat_Offset_V, 90);

                                Editor_xHudGUI.Gui_Layout_Space(5);

                                Editor_xHudGUI.Gui_Layout_Property_Field("中心点尺寸", GuideParam_CenterPointSize, 90);

                                Editor_xHudGUI.Gui_Layout_Space(5);

                                if (Editor_xHudGUI.Gui_Layout_Button("复位校正", "", HudFilled.实体, HudColor.深空灰, xHud_Dashboard.Theme_Primary, 25))
                                {
                                    GuideParam_CornerLookat_Offset_H.floatValue = 0;
                                    GuideParam_CornerLookat_Offset_H.serializedObject.ApplyModifiedProperties();

                                    GuideParam_CornerLookat_Offset_V.floatValue = 0;
                                    GuideParam_CornerLookat_Offset_V.serializedObject.ApplyModifiedProperties();

                                    GuideParam_CenterPointSize.floatValue = 1;
                                    GuideParam_CenterPointSize.serializedObject.ApplyModifiedProperties();
                                }
                                break;
                            case "三分线":
                                Editor_xHudGUI.Gui_Layout_Space(5);
                                Editor_xHudGUI.Gui_Layout_Property_Field("水平伸缩", GuideParam_Three_Offset_H, 90);

                                Editor_xHudGUI.Gui_Layout_Space(5);

                                Editor_xHudGUI.Gui_Layout_Property_Field("垂直伸缩", GuideParam_Three_Offset_V, 90);

                                Editor_xHudGUI.Gui_Layout_Space(5);

                                Editor_xHudGUI.Gui_Layout_Property_Field("整体伸缩", GuideParam_Three_Offset_Coverage, 90);

                                Editor_xHudGUI.Gui_Layout_Space(5);

                                Editor_xHudGUI.Gui_Layout_Property_Field("中心点尺寸", GuideParam_CenterPointSize, 90);

                                Editor_xHudGUI.Gui_Layout_Space(5);

                                if (Editor_xHudGUI.Gui_Layout_Button("平均等分", "", HudFilled.实体, HudColor.深空灰, xHud_Dashboard.Theme_Primary, 25))
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

                                Editor_xHudGUI.Gui_Layout_Space(5);

                                if (Editor_xHudGUI.Gui_Layout_Button("复位校正", "", HudFilled.实体, HudColor.深空灰, xHud_Dashboard.Theme_Primary, 25))
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
                                break;
                            case "引导线":
                                Editor_xHudGUI.Gui_Layout_Space(5);

                                Editor_xHudGUI.Gui_Layout_Property_Field("基准水平位移", GuideParam_GuideLine_BaseOffset, 90);

                                Editor_xHudGUI.Gui_Layout_Space(5);

                                Editor_xHudGUI.Gui_Layout_Property_Field("基准水平高度", GuideParam_GuideLine_BaseHeight, 90);

                                Editor_xHudGUI.Gui_Layout_Space(5);

                                Editor_xHudGUI.Gui_Layout_Property_Field("近景高度", GuideParam_GuideLine_Offset_NearHeight, 90);

                                Editor_xHudGUI.Gui_Layout_Space(5);

                                Editor_xHudGUI.Gui_Layout_Property_Field("近景伸缩", GuideParam_GuideLine_Offset_Near, 90);

                                Editor_xHudGUI.Gui_Layout_Space(5);

                                Editor_xHudGUI.Gui_Layout_Property_Field("远景伸缩", GuideParam_GuideLine_Offset_Far, 90);

                                Editor_xHudGUI.Gui_Layout_Space(5);

                                Editor_xHudGUI.Gui_Layout_Property_Field("中心点尺寸", GuideParam_CenterPointSize, 90);

                                Editor_xHudGUI.Gui_Layout_Space(5);

                                if (Editor_xHudGUI.Gui_Layout_Button("复位校正", "", HudFilled.实体, HudColor.深空灰, xHud_Dashboard.Theme_Primary, 25))
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
                                break;
                            case "三角":
                                Editor_xHudGUI.Gui_Layout_Space(5);

                                Editor_xHudGUI.Gui_Layout_Property_Field("基准顶部位移", GuideParam_Triangle_TopOffset, 90);

                                Editor_xHudGUI.Gui_Layout_Space(5);

                                Editor_xHudGUI.Gui_Layout_Property_Field("基准顶部高度", GuideParam_Triangle_BaseHeight, 90);

                                Editor_xHudGUI.Gui_Layout_Space(5);

                                Editor_xHudGUI.Gui_Layout_Property_Field("基准底部高度", GuideParam_Triangle_BottomHeight, 90);

                                Editor_xHudGUI.Gui_Layout_Space(5);

                                Editor_xHudGUI.Gui_Layout_Property_Field("左角点偏移", GuideParam_Triangle_Offset_Left, 90);

                                Editor_xHudGUI.Gui_Layout_Space(5);

                                Editor_xHudGUI.Gui_Layout_Property_Field("右角点偏移", GuideParam_Triangle_Offset_Right, 90);

                                Editor_xHudGUI.Gui_Layout_Space(5);

                                Editor_xHudGUI.Gui_Layout_Property_Field("中心点尺寸", GuideParam_CenterPointSize, 90);

                                Editor_xHudGUI.Gui_Layout_Space(5);

                                if (Editor_xHudGUI.Gui_Layout_Button("复位校正", "", HudFilled.实体, HudColor.深空灰, xHud_Dashboard.Theme_Primary, 25))
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
                                break;
                            case "工字型":
                                Editor_xHudGUI.Gui_Layout_Space(5);

                                Editor_xHudGUI.Gui_Layout_Property_Field("工字顶部高度", GuideParam_IShape_TopHeight, 90);

                                Editor_xHudGUI.Gui_Layout_Space(5);

                                Editor_xHudGUI.Gui_Layout_Property_Field("工字底部高度", GuideParam_IShape_BottomHeight, 90);

                                Editor_xHudGUI.Gui_Layout_Space(5);

                                Editor_xHudGUI.Gui_Layout_Property_Field("工字左偏移", GuideParam_IShape_Offset_Left, 90);

                                Editor_xHudGUI.Gui_Layout_Space(5);

                                Editor_xHudGUI.Gui_Layout_Property_Field("工字右偏移", GuideParam_IShape_Offset_Right, 90);

                                Editor_xHudGUI.Gui_Layout_Space(5);

                                if (Editor_xHudGUI.Gui_Layout_Button("复位校正", "", HudFilled.实体, HudColor.深空灰, xHud_Dashboard.Theme_Primary, 25))
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
                                break;
                        }
                    }
                    Editor_xHudGUI.Gui_Layout_Space(10);
                    Editor_xHudGUI.Gui_Layout_Vertical_End();
                }
                #endregion

                #region 资源库

                bool sw_lib = FunctionGroup("资源库", 5, HudFilled.纯色边框, HudColor.亮白, xHud_Dashboard.Theme_Primary, xHud_Dashboard.Theme_Primary, Color.gray, new RectOffset(0, 0, 0, 0), new Vector2(20, 0), PrefsKeyFold_Libs, panel_libs);
                if (sw_lib)
                {
                    #region 颜色库     
                    Editor_xHudGUI.StatuDisplayer_Object(null, 12, new Vector2(0, 1), "色卡库", 12, new Vector2(0, -7), status, new Vector2(0, 3), Lib_Color.objectReferenceValue == null ? false : true, xHud_Dashboard.Theme_Primary, Color.black * 0.7f, Lib_Color, false);
                    #endregion

                    #region 曲线库     
                    Editor_xHudGUI.StatuDisplayer_Object(null, 12, new Vector2(0, 1), "曲线库", 12, new Vector2(0, -7), status, new Vector2(0, 3), Lib_Curve.objectReferenceValue == null ? false : true, xHud_Dashboard.Theme_Primary, Color.black * 0.7f, Lib_Curve, false);
                    #endregion

                    #region 音效库     
                    Editor_xHudGUI.StatuDisplayer_Object(null, 12, new Vector2(0, 1), "音效库", 12, new Vector2(0, -7), status, new Vector2(0, 3), Lib_Sound.objectReferenceValue == null ? false : true, xHud_Dashboard.Theme_Primary, Color.black * 0.7f, Lib_Sound, false);
                    #endregion

                    #region 文字样式库     
                    Editor_xHudGUI.StatuDisplayer_Object(null, 12, new Vector2(0, 1), "字体库", 12, new Vector2(0, -7), status, new Vector2(0, 3), Lib_TextStyleLibrary.objectReferenceValue == null ? false : true, xHud_Dashboard.Theme_Primary, Color.black * 0.7f, Lib_TextStyleLibrary, false);
                    #endregion

                    #region 动效模版库     
                    Editor_xHudGUI.StatuDisplayer_Object(null, 12, new Vector2(0, 1), "动效库", 12, new Vector2(0, -7), status, new Vector2(0, 3), Lib_ElementMotion.objectReferenceValue == null ? false : true, xHud_Dashboard.Theme_Primary, Color.black * 0.7f, Lib_ElementMotion, false);
                    #endregion

                    #region 转场特效库     
                    Editor_xHudGUI.StatuDisplayer_Object(null, 12, new Vector2(0, 1), "转场库", 12, new Vector2(0, -7), status, new Vector2(0, 3), Lib_Transition.objectReferenceValue == null ? false : true, xHud_Dashboard.Theme_Primary, Color.black * 0.7f, Lib_Transition, false);
                    #endregion
                }
                Editor_xHudGUI.Gui_Layout_Space(5);
                Editor_xHudGUI.Gui_Layout_Vertical_End();
                #endregion

                #region 元素库
                bool sw_elementlibs = FunctionGroup("元素库", 5, HudFilled.纯色边框, HudColor.亮白, xHud_Dashboard.Theme_Primary, xHud_Dashboard.Theme_Primary, Color.gray, new RectOffset(0, 0, 0, 0), new Vector2(20, 0), PrefsKeyFold_ElementLibs, panel_elementlibs);
                if (sw_elementlibs)
                {
                    #region 元素库
                    if (Lib_ElementLibrarys.arraySize <= 0)
                        EditorGUILayout.HelpBox("请至少指定一个元素库资源", MessageType.Warning);

                    Editor_xHudGUI.Gui_Layout_Space(5);

                    List_ElementLib.DoLayoutList();
                    #endregion
                }
                Editor_xHudGUI.Gui_Layout_Space(5);
                Editor_xHudGUI.Gui_Layout_Vertical_End();

                #endregion

                #region 音效池
                bool sw_audiospool = FunctionGroup("音效池", 5, HudFilled.纯色边框, HudColor.亮白, xHud_Dashboard.Theme_Primary, xHud_Dashboard.Theme_Primary, Color.gray, new RectOffset(0, 0, 0, 0), new Vector2(20, 0), PrefsKeyFold_Audios, panel_audios);
                if (sw_audiospool)
                {
                    #region 音效池
                    EditorGUILayout.HelpBox("音效池为所有元素音效提供了回环队列播放音效的能力，如果您的项目不涉及音效则可忽略该警告！", MessageType.Info);

                    if (Application.isPlaying)
                        GUI.enabled = false;
                    else
                        GUI.enabled = true;
                    Editor_xHudGUI.Gui_Layout_Property_Field("预加载数量", SounderPoolCount, 100);
                    GUI.enabled = true;

                    Editor_xHudGUI.Gui_Layout_Space(10);

                    List_SoundsLib.DoLayoutList();
                    #endregion
                }
                Editor_xHudGUI.Gui_Layout_Space(5);
                Editor_xHudGUI.Gui_Layout_Vertical_End();

                #endregion

                #region 遮罩/内容可见度  

                bool sw_mask = FunctionGroup("遮罩/内容", 5, HudFilled.纯色边框, HudColor.亮白, xHud_Dashboard.Theme_Primary, xHud_Dashboard.Theme_Primary, Color.gray, new RectOffset(0, 0, 0, 0), new Vector2(20, 0), PrefsKeyFold_Mask, panel_mask);
                if (sw_mask)
                {
                    #region 遮罩

                    #region 射线遮挡阈值状态     
                    Editor_xHudGUI.StatuDisplayer_icon(null, 12, new Vector2(0, 7), MaskRaycastEnabled.boolValue ? "射线阻挡" : "射线穿透", 12, MaskRaycastEnabled.boolValue ? Color.red : xHud_Dashboard.Theme_Primary, status, 12, new Vector2(0, 4), false);
                    #endregion

                    Editor_xHudGUI.Gui_Layout_Space(5);

                    #region 透明度     
                    Editor_xHudGUI.Gui_Layout_Property_Field("透明度", MaskAlpha, 80);
                    #endregion

                    Editor_xHudGUI.Gui_Layout_Space(5);

                    #region 遮挡阈值    
                    Editor_xHudGUI.Gui_Layout_Property_Field("遮挡阈值", MaskRaycastAlphaThreshold, 80);
                    #endregion

                    Editor_xHudGUI.Gui_Layout_Space(5);

                    #region 颜色     
                    Editor_xHudGUI.Gui_Layout_Property_Field("颜色", MaskColor, 80);
                    #endregion

                    Editor_xHudGUI.Gui_Layout_Space(5);

                    #region 图形     
                    Editor_xHudGUI.Gui_Layout_Property_Field("图形", MaskTexture, 80);
                    #endregion
                    #endregion

                    #region 内容
                    Editor_xHudGUI.Gui_Layout_Space(10);

                    #region 内容透明度     
                    Editor_xHudGUI.Gui_Layout_Property_Field("透明度-屏幕", ContentAlpha_Screen, 80);
                    #endregion

                    Editor_xHudGUI.Gui_Layout_Space(5);

                    #region 内容透明度     
                    Editor_xHudGUI.Gui_Layout_Property_Field("透明度-世界", ContentAlpha_World, 80);
                    #endregion

                    #endregion
                }
                Editor_xHudGUI.Gui_Layout_Space(10);
                Editor_xHudGUI.Gui_Layout_Vertical_End();

                #endregion

                #region 散焦遮罩

                bool sw_blurmask = FunctionGroup("散焦遮罩", 5, HudFilled.纯色边框, HudColor.亮白, xHud_Dashboard.Theme_Primary, xHud_Dashboard.Theme_Primary, Color.gray, new RectOffset(0, 0, 0, 0), new Vector2(20, 0), PrefsKeyFold_BlurMask, panel_mask);
                if (sw_blurmask)
                {
                    #region 射线遮挡阈值状态     
                    Editor_xHudGUI.StatuDisplayer_icon(null, 12, new Vector2(0, 7), BlurMaskRaycastEnabled.boolValue ? "射线阻挡" : "射线穿透", 12, BlurMaskRaycastEnabled.boolValue ? Color.red : xHud_Dashboard.Theme_Primary, status, 12, new Vector2(0, 4), false);
                    #endregion

                    Editor_xHudGUI.Gui_Layout_Space(5);

                    EditorGUI.BeginChangeCheck();
                    Editor_xHudGUI.Gui_Layout_Property_Field("强度", UniversalFeature_Blur_Intensity, 85);
                    if (EditorGUI.EndChangeCheck())
                    {
                        BaseScript.hm_UniversalFeature_Blur_FastTo_ForEditor(UniversalFeature_Blur_Intensity.floatValue);

                        EditorUtility.SetDirty(BaseScript.UniversalFeature_Blur);
                    }

                    Editor_xHudGUI.Gui_Layout_Space(5);

                    Editor_xHudGUI.Gui_Layout_Property_Field("遮挡阈值", BlurMaskRaycastAlphaThreshold, 85);

                    Editor_xHudGUI.Gui_Layout_Space(5);

                    Editor_xHudGUI.Gui_Layout_Property_Field("透明度", BlurMaskAlpha, 85);

                    Editor_xHudGUI.Gui_Layout_Space(5);

                    Editor_xHudGUI.Gui_Layout_Property_Field("颜色", BlurMaskColor, 85);

                    Editor_xHudGUI.Gui_Layout_Space(5);

                    Editor_xHudGUI.Gui_Layout_Property_Field("散焦面", BlurMask, 85);

                    Editor_xHudGUI.Gui_Layout_Space(5);

                    Editor_xHudGUI.Gui_Layout_Property_Field("图形", BlurMaskTexture, 85);
                }
                Editor_xHudGUI.Gui_Layout_Space(10);
                Editor_xHudGUI.Gui_Layout_Vertical_End();

                #endregion

                #region 全局         
                bool sw_global = FunctionGroup("全局", 5, HudFilled.纯色边框, HudColor.亮白, xHud_Dashboard.Theme_Primary, xHud_Dashboard.Theme_Primary, Color.gray, new RectOffset(0, 0, 0, 0), new Vector2(20, 0), PrefsKeyFold_Global, panel_global);
                if (sw_global)
                {
                    Editor_xHudGUI.Gui_Layout_Property_Field("音效音量", Volume, 80);

                    Editor_xHudGUI.Gui_Layout_Space(5);

                    Editor_xHudGUI.Gui_Layout_Property_Field("动画速率", DurationMultiply, 80);

                    Editor_xHudGUI.Gui_Layout_Space(5);

                    EditorGUI.BeginChangeCheck();
                    Editor_xHudGUI.Gui_Layout_Property_Field("字体尺寸", FontSizeMultiply, 80);
                    if (EditorGUI.EndChangeCheck())
                    {
                        BaseScript.hm_ChangeFontGlobalSize(FontSizeMultiply.floatValue);
                    }

                    Editor_xHudGUI.Gui_Layout_Space(5);

                    Editor_xHudGUI.StatuDisplayer_Object(null, 12, new Vector2(0, 1), "光标指针", 12, new Vector2(0, -7), status, new Vector2(0, 3), Hud_MouseCursor.objectReferenceValue == null ? false : true, xHud_Dashboard.Theme_Primary, Color.black * 0.7f, Hud_MouseCursor, false);

                    Editor_xHudGUI.StatuDisplayer_Object(null, 12, new Vector2(0, 1), "转场控制器", 12, new Vector2(0, -7), status, new Vector2(0, 3), Hud_TransitionController.objectReferenceValue == null ? false : true, xHud_Dashboard.Theme_Primary, Color.black * 0.7f, Hud_TransitionController, false);

                    Editor_xHudGUI.Gui_Layout_Space(5);

                }
                Editor_xHudGUI.Gui_Layout_Space(5);
                Editor_xHudGUI.Gui_Layout_Vertical_End();
                #endregion

                #region 工具         
                bool sw_tools = FunctionGroup("工具", 5, HudFilled.纯色边框, HudColor.亮白, xHud_Dashboard.Theme_Primary, xHud_Dashboard.Theme_Primary, Color.gray, new RectOffset(0, 0, 0, 0), new Vector2(20, 0), PrefsKeyFold_Tools, panel_tools);
                if (sw_tools)
                {
                    #region 创建工具
                    if (Application.isPlaying)
                        GUI.enabled = false;
                    else
                        GUI.enabled = true;
                    Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无);
                    if (Editor_xHudGUI.Gui_Layout_Button("截屏工具", "", HudFilled.实体, HudColor.深空灰, Color.white, 30, new RectOffset(), new Vector2(0, 0)))
                    {
                        xHud_CameraCapture cc = FindFirstObjectByType<xHud_CameraCapture>();
                        if (cc == null)
                        {
                            GameObject obj_cc = new GameObject();
                            obj_cc.name = "CameraCapture";
                            obj_cc.AddComponent<xHud_CameraCapture>();
                        }
                        else
                        {
                            string res = Editor_xHudGUI.Open(xHudDialogType.警告, "XHud管理器消息", "创建截屏工具", "场景中已存在CameraCapture！", "定位", "暂不", 0);
                            if (res == "定位")
                                Selection.activeGameObject = cc.gameObject;
                        }
                        return;
                    }
                    GUI.enabled = true;
                    if (Editor_xHudGUI.Gui_Layout_Button("数据监视器", "", HudFilled.实体, HudColor.深空灰, Color.white, 30, new RectOffset(), new Vector2(0, 0)))
                    {
                        xHud_PerformanceMonitor cc = FindFirstObjectByType<xHud_PerformanceMonitor>();
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
                            xHud_PerformanceMonitor monitor = obj_cc.AddComponent<xHud_PerformanceMonitor>();
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
                            img_header_bg.sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{xHud_Dashboard.Get_path_XHUD_SPRITES_Path()}Others/BtnRect_Pure.png");
                            img_header_bg.raycastTarget = false;
                            img_header_bg.color = xHud_Dashboard.Theme_Primary;
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
                            img_header_icon.sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{xHud_Dashboard.Get_path_XHUD_SPRITES_Path()}Others/Eye.png");
                            img_header_icon.raycastTarget = false;
                            img_header_icon.color = Color.black;
                            img_header_icon.type = Image.Type.Simple;
                            #endregion

                            #region  创建标题文字
                            GameObject obj_HeaderText = new GameObject();
                            obj_HeaderText.layer = LayerMask.NameToLayer("XHud");
                            obj_HeaderText.name = "Title";
                            RectTransform rect_header_title = obj_HeaderText.AddComponent<RectTransform>();
                            xHud_Module_TmpText txt_header_title = obj_HeaderText.AddComponent<xHud_Module_TmpText>();
                            rect_header_title.SetParent(rect_header_bg);
                            rect_header_title.sizeDelta = new Vector2(0, 30);
                            rect_header_title.anchorMin = new Vector2(0, 0.5f);
                            rect_header_title.anchorMax = new Vector2(1, 0.5f);
                            rect_header_title.pivot = new Vector2(0.5f, 0.5f);
                            rect_header_title.anchoredPosition3D = Vector3.zero;
                            rect_header_title.localEulerAngles = Vector3.zero;
                            rect_header_title.localScale = Vector3.one;
                            txt_header_title.TextStyleInfo.tmp_Set_FontAsset(AssetDatabase.LoadAssetAtPath<TMP_FontAsset>($"{xHud_Dashboard.Get_path_XHUD_FONTS_Path()}Tmp/SevenStrikeFont_Bold_PerformanceMonitor_ SDF.asset"));
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
                                monitornode_bg.sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{xHud_Dashboard.Get_path_XHUD_SPRITES_Path()}Others/BtnRect_3Dot.png");
                                monitornode_bg.color = new Color(0.099f, 0.099f, 0.099f, 0.5960785f);
                                monitornode_bg.type = Image.Type.Sliced;
                                xHud_PerfomanceMonitor_Node monitornode = obj_Header_ListNode.AddComponent<xHud_PerfomanceMonitor_Node>();
                                monitornode.title = node_names[i];
                                monitornode.value = "-";

                                #region  创建标题文字
                                GameObject obj_NodeText_Title = new GameObject();
                                obj_NodeText_Title.layer = LayerMask.NameToLayer("XHud");
                                obj_NodeText_Title.name = "Title";
                                RectTransform rect_node_title = obj_NodeText_Title.AddComponent<RectTransform>();
                                xHud_Module_TmpText txt_node_title = obj_NodeText_Title.AddComponent<xHud_Module_TmpText>();
                                rect_node_title.SetParent(rect_monitornode);

                                rect_node_title.anchorMin = new Vector2(0, 0);
                                rect_node_title.anchorMax = new Vector2(1, 1);
                                rect_node_title.pivot = new Vector2(0.5f, 0.5f);
                                rect_node_title.anchoredPosition3D = new Vector3(0, 0, 0);  // Y位置
                                rect_node_title.offsetMin = new Vector2(20, 30);  // Left:20px
                                rect_node_title.offsetMax = new Vector2(-25, -10); // Right:50px
                                rect_node_title.localEulerAngles = Vector3.zero;
                                rect_node_title.localScale = Vector3.one;
                                txt_node_title.TextStyleInfo.tmp_Set_FontAsset(AssetDatabase.LoadAssetAtPath<TMP_FontAsset>($"{xHud_Dashboard.Get_path_XHUD_FONTS_Path()}Tmp/SevenBlack-Bold SDF.asset"));
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
                                xHud_Module_TmpText txt_node_value = obj_NodeText_Value.AddComponent<xHud_Module_TmpText>();
                                rect_node_value.SetParent(rect_monitornode);
                                rect_node_value.anchorMin = new Vector2(0, 0);
                                rect_node_value.anchorMax = new Vector2(1, 1);
                                rect_node_value.pivot = new Vector2(0.5f, 0.5f);
                                rect_node_value.anchoredPosition3D = new Vector3(0, 0, 0);  // Y位置
                                rect_node_value.offsetMin = new Vector2(50, 10);  // Left:20px
                                rect_node_value.offsetMax = new Vector2(-20, -30); // Right:50px
                                rect_node_value.localEulerAngles = Vector3.zero;
                                rect_node_value.localScale = Vector3.one;
                                txt_node_value.TextStyleInfo.tmp_Set_FontAsset(AssetDatabase.LoadAssetAtPath<TMP_FontAsset>($"{xHud_Dashboard.Get_path_XHUD_FONTS_Path()}Tmp/SevenBlack-Regular SDF.asset"));
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
                            string res = Editor_xHudGUI.Open(xHudDialogType.警告, "XHud管理器消息", "创建效能面板", "场景中已存在效能面板！无需重复添加！", "定位", "暂不", 0);
                            if (res == "定位")
                                Selection.activeGameObject = cc.gameObject;
                        }
                        return;
                    }
                    Editor_xHudGUI.Gui_Layout_Horizontal_End();
                    #endregion
                }
                Editor_xHudGUI.Gui_Layout_Space(5);
                Editor_xHudGUI.Gui_Layout_Vertical_End();
                #endregion

                #region 布局        

                bool sw_frame = FunctionGroup("布局", 5, HudFilled.纯色边框, HudColor.亮白, xHud_Dashboard.Theme_Primary, xHud_Dashboard.Theme_Primary, Color.gray, new RectOffset(0, 0, 0, 0), new Vector2(20, 0), PrefsKeyFold_FrameLayout, panel_frame);
                if (sw_frame)
                {
                    Editor_xHudGUI.Gui_Layout_Space(5);

                    SerializedProperty sp_mar_x = Margins.FindPropertyRelative("x");
                    SerializedProperty sp_mar_y = Margins.FindPropertyRelative("y");
                    SerializedProperty sp_mar_z = Margins.FindPropertyRelative("z");
                    SerializedProperty sp_mar_w = Margins.FindPropertyRelative("w");

                    Editor_xHudGUI.Gui_Layout_Property_Field("上边距", sp_mar_x);

                    Editor_xHudGUI.Gui_Layout_Space(5);

                    Editor_xHudGUI.Gui_Layout_Property_Field("下边距", sp_mar_y);

                    Editor_xHudGUI.Gui_Layout_Space(5);

                    Editor_xHudGUI.Gui_Layout_Property_Field("左边距", sp_mar_z);

                    Editor_xHudGUI.Gui_Layout_Space(5);

                    Editor_xHudGUI.Gui_Layout_Property_Field("右边距", sp_mar_w);

                    Editor_xHudGUI.Gui_Layout_Space(5);

                    Editor_xHudGUI.Gui_Layout_Property_Field("水平缩放", MarginHorizontal);

                    Editor_xHudGUI.Gui_Layout_Space(5);

                    Editor_xHudGUI.Gui_Layout_Property_Field("垂直缩放", MarginVertical);

                    Editor_xHudGUI.Gui_Layout_Space(5);

                    Editor_xHudGUI.Gui_Layout_Property_Field("整体缩放", MarginMultiply);

                    Editor_xHudGUI.Gui_Layout_Space(5);


                }
                Editor_xHudGUI.Gui_Layout_Space(5);
                Editor_xHudGUI.Gui_Layout_Vertical_End();
                #endregion

                #region 辅助          

                if (UseSafeFrame.boolValue)
                {
                    bool sw_assist = FunctionGroup("辅助", 5, HudFilled.纯色边框, HudColor.亮白, xHud_Dashboard.Theme_Primary, xHud_Dashboard.Theme_Primary, Color.gray, new RectOffset(0, 0, 0, 0), new Vector2(20, 0), PrefsKeyFold_Assist, panel_assist);
                    if (sw_assist)
                    {
                        Editor_xHudGUI.Gui_Layout_Space(5);

                        #region 颜色
                        Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                        Editor_xHudGUI.Gui_Layout_Space(10);
                        FrameColors = EditorGUILayout.Foldout(FrameColors, "颜色配置", true);
                        Editor_xHudGUI.Gui_Layout_Space(5);
                        Editor_xHudGUI.Gui_Layout_Horizontal_End();

                        if (FrameColors)
                        {
                            Editor_xHudGUI.Gui_Layout_Property_Field("角点标记", Color_LayoutAnchorMark, 90);

                            Editor_xHudGUI.Gui_Layout_Space(5);

                            Editor_xHudGUI.Gui_Layout_Property_Field("中心标记", Color_CenterMark, 90);

                            Editor_xHudGUI.Gui_Layout_Space(5);

                            Editor_xHudGUI.Gui_Layout_Property_Field("安全框", Color_FrameLine, 90);

                            Editor_xHudGUI.Gui_Layout_Space(5);

                            Editor_xHudGUI.Gui_Layout_Property_Field("分割标记", Color_SeperaterLine, 90);

                            Editor_xHudGUI.Gui_Layout_Space(5);

                        }
                        #endregion

                        #region 样式
                        Editor_xHudGUI.Gui_Layout_Space(10);

                        Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                        Editor_xHudGUI.Gui_Layout_Space(10);
                        FrameStyles = EditorGUILayout.Foldout(FrameStyles, "样式配置", true);
                        Editor_xHudGUI.Gui_Layout_Space(5);
                        Editor_xHudGUI.Gui_Layout_Horizontal_End();

                        if (FrameStyles)
                        {
                            Editor_xHudGUI.Gui_Layout_Property_Field("角点标记尺寸", MarkSize, 110);

                            Editor_xHudGUI.Gui_Layout_Space(5);

                            Editor_xHudGUI.Gui_Layout_Property_Field("安全框粗细", Safe_FrameLine_Width, 110);

                            Editor_xHudGUI.Gui_Layout_Space(5);

                            Editor_xHudGUI.Gui_Layout_Property_Field("安全中分线长度", Safe_Seperater_Length, 110);

                            Editor_xHudGUI.Gui_Layout_Space(5);

                            Editor_xHudGUI.Gui_Layout_Property_Field("安全框边距", Safe_FrameLine_Margins, 110);

                            Editor_xHudGUI.Gui_Layout_Space(5);

                            Editor_xHudGUI.Gui_Layout_Property_Field("中心标记间距", Safe_CenterMarkDistance, 110);

                            Editor_xHudGUI.Gui_Layout_Space(5);

                            Editor_xHudGUI.Gui_Layout_Property_Field("中心标记宽度", Safe_CenterMarkWidth, 110);

                            Editor_xHudGUI.Gui_Layout_Space(5);

                            Editor_xHudGUI.Gui_Layout_Property_Field("中心标记长度", Safe_CenterMarkLength, 110);
                        }
                        #endregion

                        Editor_xHudGUI.Gui_Layout_Space(10);

                        if (Editor_xHudGUI.Gui_Layout_Button("重置参数", "", HudFilled.实体, HudColor.深空灰, xHud_Dashboard.Theme_Primary, 25))
                        {
                            Color_LayoutAnchorMark.colorValue = Color.white;
                            Color_CenterMark.colorValue = xHud_Dashboard.Theme_Primary;
                            Color_FrameLine.colorValue = Color.white * 0.7f;
                            Color_SeperaterLine.colorValue = xHud_Dashboard.Theme_Primary;
                            MarkSize.floatValue = 10;
                            Safe_FrameLine_Width.floatValue = 0.5f;
                            Safe_Seperater_Length.floatValue = 30f;
                            Safe_FrameLine_Margins.vector2Value = new Vector2(50, 50);
                            Safe_CenterMarkDistance.floatValue = 40f;
                            Safe_CenterMarkWidth.floatValue = 1f;
                            Safe_CenterMarkLength.floatValue = 12f;
                        }
                    }
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    Editor_xHudGUI.Gui_Layout_Vertical_End();
                }

                #endregion

                #region 蓝图    
                if (BluePrintMode.boolValue)
                {
                    bool sw_blueprint = FunctionGroup("蓝图", 5, HudFilled.纯色边框, HudColor.亮白, xHud_Dashboard.Theme_Primary, xHud_Dashboard.Theme_Primary, Color.gray, new RectOffset(0, 0, 0, 0), new Vector2(20, 0), PrefsKeyFold_BluePrint, panel_bulueprint);
                    if (sw_blueprint)
                    {
                        Editor_xHudGUI.Gui_Layout_Space(5);

                        #region 蓝图网格状态
                        Editor_xHudGUI.StatuDisplayer_text(null, 12, new Vector2(0, 7), "蓝图激活状态", 12, BluePrint_Displayed.boolValue ? "激活" : "未激活", xHud_Dashboard.Theme_Primary, 11, false);
                        #endregion

                        #region 蓝图参数
                        Editor_xHudGUI.Gui_Layout_Property_Field("整体透明度", BluePrint_opacity, 100);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        Editor_xHudGUI.Gui_Layout_Property_Field("背景透明度", BluePrint_bg_opacity, 100);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        Editor_xHudGUI.Gui_Layout_Property_Field("背景动画缓动 - 入场", BluePrint_Bg_AnimationEase_In, 140);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        Editor_xHudGUI.Gui_Layout_Property_Field("背景动画缓动 - 出场", BluePrint_Bg_AnimationEase_Out, 140);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        Editor_xHudGUI.Gui_Layout_Property_Field("背景淡化退场延迟", BluePrint_Bg_FadeAnimationDelay, 140);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        Editor_xHudGUI.Gui_Layout_Property_Field("背景色", BluePrint_bg_color, 100);

                        Editor_xHudGUI.Gui_Layout_Space(5);
                        Editor_xHudGUI.Gui_Layout_Seperator(1, xHud_Dashboard.Theme_SeperateLine);
                        Editor_xHudGUI.Gui_Layout_Space(5);

                        #region 叠加图像透明度                        
                        Editor_xHudGUI.Gui_Layout_Property_Field("叠加图像透明度", BluePrint_bg_mapOpacity, 120);
                        #endregion

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        #region 叠加图像颜色                             
                        Editor_xHudGUI.Gui_Layout_Property_Field("叠加图像颜色", BluePrint_bg_decal_color, 100);
                        #endregion

                        Editor_xHudGUI.Gui_Layout_Space(10);

                        EditorGUI.BeginChangeCheck();
                        Editor_xHudGUI.Gui_Layout_Popup<string, xHud_Manager>("叠加图像", BluePrintBgs_Name, ref BluePrint_bg_name, HudFilled.实体, 120, 22, SelectedObjects);
                        if (EditorGUI.EndChangeCheck())
                        {
                            #region 指定叠加背景
                            string texName = "";

                            for (int i = 0; i < BluePrintBgs_Name.Length; i++)
                            {
                                if (BluePrint_bg_name.stringValue == BluePrintBgs_Name[i])
                                {
                                    texName = BluePrintBgs_Name[i];
                                    break;
                                }
                            }

                            BluePrint_bg_name.stringValue = texName;
                            BluePrint_bg_name.serializedObject.ApplyModifiedProperties();
                            Texture2D texselected = AssetDatabase.LoadAssetAtPath<Texture2D>($"{xHud_Dashboard.Get_XHudRoot_Path()}Textures/BluePrints/{texName}.png");
                            if (texselected != null)
                            {
                                BaseScript.hm_BluePrint_SetBgTexture(texselected);
                            }
                            #endregion
                        }

                        Editor_xHudGUI.Gui_Layout_Popup<int, xHud_Manager>("叠加图像 - 平铺模式", stroptions_enabled, ref BluePrint_bg_usetilling_index, HudFilled.实体, 120, 22, SelectedObjects);

                        Editor_xHudGUI.Gui_Layout_Popup<int, xHud_Manager>("叠加图像 - 方形比例", stroptions_enabled, ref BluePrint_bg_usesquareratio_index, HudFilled.实体, 120, 22, SelectedObjects);

                        #region 叠加图像平铺
                        if (BluePrint_bg_usetilling_index.intValue == 1)
                        {
                            Editor_xHudGUI.Gui_Layout_Property_Field("叠加图像平铺", BluePrint_bg_tilling, 100);
                        }
                        #endregion

                        Editor_xHudGUI.Gui_Layout_Space(5);
                        Editor_xHudGUI.Gui_Layout_Seperator(1, xHud_Dashboard.Theme_SeperateLine);
                        Editor_xHudGUI.Gui_Layout_Space(5);

                        Editor_xHudGUI.Gui_Layout_Property_Field("网格色", BluePrint_grid_color, 100);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        Editor_xHudGUI.Gui_Layout_Property_Field("网格透明度", BluePrint_grid_Opacity, 100);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        EditorGUI.BeginChangeCheck();
                        Editor_xHudGUI.Gui_Layout_Property_Field("网格尺寸", BluePrint_grid_size, 100);
                        if (EditorGUI.EndChangeCheck())
                        {
                            if (!Application.isPlaying)
                            {
                                BaseScript.hm_BluePrint_Remove();

                                Material mat = AssetDatabase.LoadAssetAtPath<Material>($"{xHud_Dashboard.Get_Materials_Path()}BluePrints/Mat/BluePrint.mat");
                                TMP_FontAsset title = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>($"{xHud_Dashboard.Get_path_XHUD_FONTS_Path()}Fonts/Tmp/SevenBlack-Bold SDF.asset");
                                TMP_FontAsset subtitle = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>($"{xHud_Dashboard.Get_path_XHUD_FONTS_Path()}Fonts/Tmp/SevenBlack-Light SDF.asset");
                                BaseScript.hm_BluePrint_Create(true, mat, title, subtitle);
                            }
                        }

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        Editor_xHudGUI.Gui_Layout_Property_Field("网格线宽度", BluePrint_linewidth, 100);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        Editor_xHudGUI.Gui_Layout_Property_Field("网格线百分比", BluePrint_Grid_LengthPercentage, 100);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        Editor_xHudGUI.Gui_Layout_Property_Field("网格分级高差", BluePrint_Grid_LevelHeight, 100);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        Editor_xHudGUI.Gui_Layout_Property_Field("网格动画结束%", BluePrint_GridEnd, 100);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        Editor_xHudGUI.Gui_Layout_Property_Field("网格动画速率", BluePrint_Grid_AnimationDuration, 100);

                        Editor_xHudGUI.Gui_Layout_Space(5);
                        Editor_xHudGUI.Gui_Layout_Seperator(1, xHud_Dashboard.Theme_SeperateLine);
                        Editor_xHudGUI.Gui_Layout_Space(5);

                        Editor_xHudGUI.Gui_Layout_Property_Field("大标题色", BluePrint_marktitle_color, 100);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        Editor_xHudGUI.Gui_Layout_Property_Field("小标题色", BluePrint_marksubtitle_color, 100);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        Editor_xHudGUI.Gui_Layout_Property_Field("水印大标题", BluePrint_title_content, 100);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        Editor_xHudGUI.Gui_Layout_Property_Field("水印小标题", BluePrint_subtitle_content, 100);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        Editor_xHudGUI.Gui_Layout_Property_Field("水印透明度", BluePrint_mark_opacity, 100);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        Editor_xHudGUI.Gui_Layout_Property_Field("水印尺寸", BluePrint_mark_size, 100);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        Editor_xHudGUI.Gui_Layout_Property_Field("水印间距", BluePrint_mark_space, 100);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        Editor_xHudGUI.Gui_Layout_Property_Field("水印锚点", BluePrint_MarkAnchors, 100);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        EditorGUILayout.HelpBox("边距说明：X: 左   Y: 上   Z: 右   W: 下", MessageType.Info);
                        Editor_xHudGUI.Gui_Layout_Property_Field("水印边距", BluePrint_mark_margin, 100);

                        Editor_xHudGUI.Gui_Layout_Space(5);
                        Editor_xHudGUI.Gui_Layout_Seperator(1, xHud_Dashboard.Theme_SeperateLine);
                        Editor_xHudGUI.Gui_Layout_Space(5);

                        Editor_xHudGUI.Gui_Layout_Property_Field("总动画速度", BluePrint_AnimationDuration, 100);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        Editor_xHudGUI.Gui_Layout_Property_Field("网格动画缓动", BluePrint_Grid_AnimationEase, 100);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        Editor_xHudGUI.Gui_Layout_Toggle<bool, xHud_Manager>("开始时退场状态", stroptions_enabled, ref BluePrint_OnStartHide, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

                        Editor_xHudGUI.Gui_Layout_Toggle<bool, xHud_Manager>("入/退场影响网格", stroptions_effect, ref Eft_Grid, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

                        Editor_xHudGUI.Gui_Layout_Toggle<bool, xHud_Manager>("入/退场影响网格透明度", stroptions_effect, ref Eft_GridFade, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

                        Editor_xHudGUI.Gui_Layout_Toggle<bool, xHud_Manager>("入/退场影响背景", stroptions_effect, ref Eft_Bg, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

                        Editor_xHudGUI.Gui_Layout_Toggle<bool, xHud_Manager>("入/退场影响水印", stroptions_effect, ref Eft_Mark, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);


                        if (!Application.isPlaying)
                        {
                            BaseScript.hm_BluePrint_Update();
                        }
                        #endregion
                    }
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    Editor_xHudGUI.Gui_Layout_Vertical_End();

                }
                #endregion

                #region RMS
                if (RMS_Enabled.boolValue)
                {
                    bool sw_resmatch = FunctionGroup("RMS", 5, HudFilled.纯色边框, HudColor.亮白, xHud_Dashboard.Theme_Primary, xHud_Dashboard.Theme_Primary, Color.gray, new RectOffset(0, 0, 0, 0), new Vector2(20, 0), PrefsKeyFold_LayoutMatchRes, panel_matchres);
                    if (sw_resmatch)
                    {
                        if (Application.isPlaying)
                            GUI.enabled = false;
                        else
                            GUI.enabled = true;

                        #region 分辨率列表

                        if (RMS_Nodes.arraySize <= 0)
                            EditorGUILayout.HelpBox("如果开发的应用布局元素需要同时针对多个分辨率进行匹配和定位，那么请在此添加目标分辨率", MessageType.Info);
                        Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                        Editor_xHudGUI.Gui_Layout_Space(5);
                        List_RMM.DoLayoutList();
                        Editor_xHudGUI.Gui_Layout_Space(5);
                        Editor_xHudGUI.Gui_Layout_Horizontal_End();

                        #endregion

                        if (Editor_xHudGUI.Gui_Layout_Button("清空列表", "清空分辨率列表", HudFilled.实体, HudColor.深空灰, Color.white, 25, new RectOffset(), new Vector2(0, 0)))
                        {
                            string res = Editor_xHudGUI.Open(xHudDialogType.警告, "XHud管理器消息", "RMS列表", "如果清空该列表可能会导致元素匹配分辨率数据异常！请确定是否继续该操作？！", "定位", "暂不", 0);
                            if (res == "暂不")
                                return;

                            RMS_Nodes.ClearArray();
                        }
                        GUI.enabled = true;
                    }
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    Editor_xHudGUI.Gui_Layout_Vertical_End();
                }
                #endregion

                #region 动效默认参数
                string hexcol = xHud_Utilitys.Color_To_HexColor(xHud_Dashboard.Theme_Primary, true);

                bool sw_eleparam_creator = FunctionGroup("默认动效参数", 5, HudFilled.纯色边框, HudColor.亮白, xHud_Dashboard.Theme_Primary, xHud_Dashboard.Theme_Primary, Color.gray, new RectOffset(0, 0, 0, 0), new Vector2(20, 0), PrefsKeyFold_ElementParam_DefaultMotion, panel_eleparammotion_default);
                if (sw_eleparam_creator)
                {
                    #region 模版库                             
                    //确保动效库存在
                    if (BaseScript.Hud_ElementMotion != null)
                    {
                        //确保动效库不是空的
                        if (BaseScript.Hud_ElementMotion.ElementMotionList != null && BaseScript.Hud_ElementMotion.ElementMotionList.Count > 0)
                        {
                            Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);

                            //动效列表
                            string[] motnames = BaseScript.Hud_ElementMotion.ElementMotion_GetAllName_With_Create();
                            EditorGUI.BeginChangeCheck();
                            Editor_xHudGUI.Gui_Layout_Popup<string, xHud_Manager>("生成", motnames, ref Crc_Lib_Name, HudFilled.实体, 120, 22, SelectedObjects);
                            if (EditorGUI.EndChangeCheck())
                            {
                                Motion_Creator crc = BaseScript.Hud_ElementMotion.ElementMotion_GetElementCreator_At_Create(Crc_Lib_Name.stringValue);

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
                                CreateArgs_Default.serializedObject.ApplyModifiedProperties();
                            }

                            #region 保存 & 定位模板
                            Editor_xHudGUI.Gui_Layout_Space(10);

                            if (Editor_xHudGUI.Gui_Layout_Button(14, "保存", save_r, save_p, 2))
                            {
                                OpenParameterSetter(HudElementMotionType.Creator);
                                return;
                            }

                            Editor_xHudGUI.Gui_Layout_Space(10);

                            if (Editor_xHudGUI.Gui_Layout_Button(14, "定位", locate_r, locate_p, 2))
                            {
                                if (!BaseScript.Hud_ElementMotion.ElementMotion_IsExist(Crc_Lib_Name.stringValue))
                                    return;
                                Editor_MenuItemsAction_OpenLibrary.open_elementmotion();
                                BaseScript.Hud_ElementMotion.ElementMotionLibrary_Location(Crc_Lib_Name.stringValue);
                                return;
                            }

                            Editor_xHudGUI.Gui_Layout_Space(10);

                            if (Editor_xHudGUI.Gui_Layout_Button(14, "重置", reset_r, reset_p, 2))
                            {
                                string res = Editor_xHudGUI.Open(xHudDialogType.警告, "XHud管理器消息", "重置动效参数", "确定要将动效参数重置吗？您将丢失当前的动效参数！", "重置", "暂不", 0);
                                if (res == "重置")
                                    ResetMotionParams("CreateArgs");
                                return;
                            }
                            Editor_xHudGUI.Gui_Layout_Space(5);
                            #endregion

                            Editor_xHudGUI.Gui_Layout_Horizontal_End();
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("未在动效库中发现任何动效资源，请先为其添加动效资源!", MessageType.Warning);
                            Editor_xHudGUI.Gui_Layout_Space(5);
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("Hud管理器中未指定动效库，请先配置动效库!", MessageType.Warning);
                        Editor_xHudGUI.Gui_Layout_Space(5);
                    }
                    #endregion

                    SerializedProperty sp_def_create_anchor = CreateArgs_Default.FindPropertyRelative("anchor");
                    Editor_xHudGUI.Gui_Layout_Property_Field("锚点", sp_def_create_anchor);

                    #region 位移
                    Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_xHudGUI.Gui_Layout_Space(10);
                    def_create_fold_move = EditorGUILayout.Foldout(def_create_fold_move, "位移", true);
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    Editor_xHudGUI.Gui_Layout_Horizontal_End();

                    if (def_create_fold_move)
                    {
                        SerializedProperty sp_def_create_move_type = CreateArgs_Default.FindPropertyRelative("Movement.Movement");
                        Editor_xHudGUI.Gui_Layout_Property_Field("方式", sp_def_create_move_type);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        SerializedProperty sp_def_create_move_dis = CreateArgs_Default.FindPropertyRelative("Movement.Distance");
                        Editor_xHudGUI.Gui_Layout_Property_Field("距离", sp_def_create_move_dis);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        SerializedProperty sp_def_create_move_dur = CreateArgs_Default.FindPropertyRelative("Movement.Duration");
                        Editor_xHudGUI.Gui_Layout_Property_Field("耗时", sp_def_create_move_dur);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        SerializedProperty sp_def_create_move_delay = CreateArgs_Default.FindPropertyRelative("Movement.Delay");
                        Editor_xHudGUI.Gui_Layout_Property_Field("延迟", sp_def_create_move_delay);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        SerializedProperty sp_def_create_move_curve = CreateArgs_Default.FindPropertyRelative("Movement.Curve");
                        Editor_xHudGUI.Gui_Layout_Property_Field("曲线", sp_def_create_move_curve);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        SerializedProperty sp_def_create_move_ease = CreateArgs_Default.FindPropertyRelative("Movement.Ease");
                        Editor_xHudGUI.Gui_Layout_Property_Field("缓动", sp_def_create_move_ease);
                    }
                    #endregion

                    #region 旋转
                    Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_xHudGUI.Gui_Layout_Space(10);
                    def_create_fold_rotate = EditorGUILayout.Foldout(def_create_fold_rotate, "旋转", true);
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    Editor_xHudGUI.Gui_Layout_Horizontal_End();

                    if (def_create_fold_rotate)
                    {
                        SerializedProperty sp_def_create_rot_type = CreateArgs_Default.FindPropertyRelative("Rotation.Rotation");
                        Editor_xHudGUI.Gui_Layout_Property_Field("方式", sp_def_create_rot_type);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        SerializedProperty sp_def_create_rot_deg = CreateArgs_Default.FindPropertyRelative("Rotation.Degree");
                        Editor_xHudGUI.Gui_Layout_Property_Field("角度", sp_def_create_rot_deg);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        SerializedProperty sp_def_create_rot_dur = CreateArgs_Default.FindPropertyRelative("Rotation.Duration");
                        Editor_xHudGUI.Gui_Layout_Property_Field("耗时", sp_def_create_rot_dur);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        SerializedProperty sp_def_create_rot_delay = CreateArgs_Default.FindPropertyRelative("Rotation.Delay");
                        Editor_xHudGUI.Gui_Layout_Property_Field("延迟", sp_def_create_rot_delay);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        SerializedProperty sp_def_create_rot_curve = CreateArgs_Default.FindPropertyRelative("Rotation.Curve");
                        Editor_xHudGUI.Gui_Layout_Property_Field("曲线", sp_def_create_rot_curve);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        SerializedProperty sp_def_create_rot_ease = CreateArgs_Default.FindPropertyRelative("Rotation.Ease");
                        Editor_xHudGUI.Gui_Layout_Property_Field("缓动", sp_def_create_rot_ease);
                    }
                    #endregion

                    #region 透明度
                    Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_xHudGUI.Gui_Layout_Space(10);
                    def_create_fold_alpha = EditorGUILayout.Foldout(def_create_fold_alpha, "透明度", true);
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    Editor_xHudGUI.Gui_Layout_Horizontal_End();

                    if (def_create_fold_alpha)
                    {
                        SerializedProperty sp_def_create_alpha_type = CreateArgs_Default.FindPropertyRelative("Alpha.Duration");
                        Editor_xHudGUI.Gui_Layout_Property_Field("耗时", sp_def_create_alpha_type);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        SerializedProperty sp_def_create_alpha_delay = CreateArgs_Default.FindPropertyRelative("Alpha.Delay");
                        Editor_xHudGUI.Gui_Layout_Property_Field("延迟", sp_def_create_alpha_delay);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        SerializedProperty sp_def_create_alpha_curve = CreateArgs_Default.FindPropertyRelative("Alpha.Curve");
                        Editor_xHudGUI.Gui_Layout_Property_Field("曲线", sp_def_create_alpha_curve);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        SerializedProperty sp_def_create_alpha_ease = CreateArgs_Default.FindPropertyRelative("Alpha.Ease");
                        Editor_xHudGUI.Gui_Layout_Property_Field("缓动", sp_def_create_alpha_ease);
                    }
                    #endregion

                    EditorGUI.BeginChangeCheck();
                    Editor_xHudGUI.Gui_Layout_Property_Field("动效结束时机", CreateArgs_MotionAnimateEndState, 85);
                    if (EditorGUI.EndChangeCheck())
                    {
                        MotionAnimateEndState state = (MotionAnimateEndState)CreateArgs_MotionAnimateEndState.enumValueIndex;
                        switch (state)
                        {
                            case MotionAnimateEndState.以_移动为准:
                                HudMotion_Movement m = (HudMotion_Movement)CreateArgs_Default.FindPropertyRelative("Movement.Movement").enumValueIndex;
                                if (m == HudMotion_Movement.A_无运动)
                                {
                                    Editor_xHudGUI.Open(xHudDialogType.警告, $"XHud管理器消息", "设定动画结束时机", $"当前位移方式为 <color={hexcol}> A_无运动 </color>，将发生<color={hexcol}> 动效动画无法正常结束</color>从而导致<color={hexcol}> 相应的事件和动作委托 </color>不能正确触发的情况 </color>！请将<color={hexcol}> 位移方式 </color>改为<color={hexcol}> 非无运动方式 </color>！", "明白", 0);
                                    CreateArgs_MotionAnimateEndState.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
                                }
                                break;
                            case MotionAnimateEndState.以_旋转为准:
                                HudMotion_Rotation r = (HudMotion_Rotation)CreateArgs_Default.FindPropertyRelative("Rotation.Rotation").enumValueIndex;
                                if (r == HudMotion_Rotation.A_无旋转)
                                {
                                    Editor_xHudGUI.Open(xHudDialogType.警告, $"XHud管理器消息", "设定动画结束时机", $"当前旋转方式为 <color={hexcol}> A_无旋转 </color>，将发生<color={hexcol}> 动效动画无法正常结束</color>从而导致<color={hexcol}> 相应的事件和动作委托 </color>不能正确触发的情况 </color>！请将<color={hexcol}> 旋转方式 </color>改为<color={hexcol}> 非无旋转方式 </color>！", "明白", 0);
                                    CreateArgs_MotionAnimateEndState.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
                                }
                                break;
                        }
                    }

                    Editor_xHudGUI.Gui_Layout_Space(5);
                    Editor_xHudGUI.Gui_Layout_Seperator(1, xHud_Dashboard.Theme_SeperateLine);
                    Editor_xHudGUI.Gui_Layout_Space(5);

                    #region 模版库                             
                    //确保动效库存在
                    if (BaseScript.Hud_ElementMotion != null)
                    {
                        //确保动效库不是空的
                        if (BaseScript.Hud_ElementMotion.ElementMotionList != null && BaseScript.Hud_ElementMotion.ElementMotionList.Count > 0)
                        {
                            Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);

                            //动效列表
                            string[] motnames = BaseScript.Hud_ElementMotion.ElementMotion_GetAllName_With_Recycle();
                            EditorGUI.BeginChangeCheck();
                            Editor_xHudGUI.Gui_Layout_Popup<string, xHud_Manager>("回收", motnames, ref Rec_Lib_Name, HudFilled.实体, 120, 22, SelectedObjects);
                            if (EditorGUI.EndChangeCheck())
                            {
                                Motion_Recycler rec = BaseScript.Hud_ElementMotion.ElementMotion_GetElementCreator_At_Recycle(Rec_Lib_Name.stringValue);

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
                                RecycleArgs_Default.serializedObject.ApplyModifiedProperties();
                            }

                            #region 保存 & 定位模板
                            Editor_xHudGUI.Gui_Layout_Space(10);

                            if (Editor_xHudGUI.Gui_Layout_Button(14, "保存", save_r, save_p, 2))
                            {
                                OpenParameterSetter(HudElementMotionType.Recycler);
                                return;
                            }

                            Editor_xHudGUI.Gui_Layout_Space(10);

                            if (Editor_xHudGUI.Gui_Layout_Button(14, "定位", locate_r, locate_p, 2))
                            {
                                if (!BaseScript.Hud_ElementMotion.ElementMotion_IsExist(Rec_Lib_Name.stringValue))
                                    return;
                                Editor_MenuItemsAction_OpenLibrary.open_elementmotion();
                                BaseScript.Hud_ElementMotion.ElementMotionLibrary_Location(Rec_Lib_Name.stringValue);
                                return;
                            }

                            Editor_xHudGUI.Gui_Layout_Space(10);

                            if (Editor_xHudGUI.Gui_Layout_Button(14, "重置", reset_r, reset_p, 2))
                            {
                                string res = Editor_xHudGUI.Open(xHudDialogType.警告, "XHud管理器消息", "重置动效参数", "确定要将动效参数重置吗？您将丢失当前的动效参数！", "重置", "暂不", 0);
                                if (res == "重置")
                                    ResetMotionParams("RecycleArgs");
                                return;
                            }
                            Editor_xHudGUI.Gui_Layout_Space(5);
                            #endregion

                            Editor_xHudGUI.Gui_Layout_Horizontal_End();
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("未在动效库中发现任何动效资源，请先为其添加动效资源!", MessageType.Warning);
                            Editor_xHudGUI.Gui_Layout_Space(5);
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("Hud管理器中未指定动效库，请先配置动效库!", MessageType.Warning);
                        Editor_xHudGUI.Gui_Layout_Space(5);
                    }
                    #endregion

                    #region 位移
                    Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_xHudGUI.Gui_Layout_Space(10);
                    def_recycle_fold_move = EditorGUILayout.Foldout(def_recycle_fold_move, "位移", true);
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    Editor_xHudGUI.Gui_Layout_Horizontal_End();

                    if (def_recycle_fold_move)
                    {
                        SerializedProperty sp_def_recycle_move_type = RecycleArgs_Default.FindPropertyRelative("Movement.Movement");
                        Editor_xHudGUI.Gui_Layout_Property_Field("方式", sp_def_recycle_move_type);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        SerializedProperty sp_def_recycle_move_dis = RecycleArgs_Default.FindPropertyRelative("Movement.Distance");
                        Editor_xHudGUI.Gui_Layout_Property_Field("距离", sp_def_recycle_move_dis);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        SerializedProperty sp_def_recycle_move_dur = RecycleArgs_Default.FindPropertyRelative("Movement.Duration");
                        Editor_xHudGUI.Gui_Layout_Property_Field("耗时", sp_def_recycle_move_dur);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        SerializedProperty sp_def_recycle_move_delay = RecycleArgs_Default.FindPropertyRelative("Movement.Delay");
                        Editor_xHudGUI.Gui_Layout_Property_Field("延迟", sp_def_recycle_move_delay);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        SerializedProperty sp_def_recycle_move_curve = RecycleArgs_Default.FindPropertyRelative("Movement.Curve");
                        Editor_xHudGUI.Gui_Layout_Property_Field("曲线", sp_def_recycle_move_curve);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        SerializedProperty sp_def_recycle_move_ease = RecycleArgs_Default.FindPropertyRelative("Movement.Ease");
                        Editor_xHudGUI.Gui_Layout_Property_Field("缓动", sp_def_recycle_move_ease);
                    }
                    #endregion

                    #region 旋转
                    Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_xHudGUI.Gui_Layout_Space(10);
                    def_recycle_fold_rotate = EditorGUILayout.Foldout(def_recycle_fold_rotate, "旋转", true);
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    Editor_xHudGUI.Gui_Layout_Horizontal_End();

                    if (def_recycle_fold_rotate)
                    {
                        SerializedProperty sp_def_recycle_rot_type = RecycleArgs_Default.FindPropertyRelative("Rotation.Rotation");
                        Editor_xHudGUI.Gui_Layout_Property_Field("方式", sp_def_recycle_rot_type);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        SerializedProperty sp_def_recycle_rot_deg = RecycleArgs_Default.FindPropertyRelative("Rotation.Degree");
                        Editor_xHudGUI.Gui_Layout_Property_Field("角度", sp_def_recycle_rot_deg);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        SerializedProperty sp_def_recycle_rot_dur = RecycleArgs_Default.FindPropertyRelative("Rotation.Duration");
                        Editor_xHudGUI.Gui_Layout_Property_Field("耗时", sp_def_recycle_rot_dur);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        SerializedProperty sp_def_recycle_rot_delay = RecycleArgs_Default.FindPropertyRelative("Rotation.Delay");
                        Editor_xHudGUI.Gui_Layout_Property_Field("延迟", sp_def_recycle_rot_delay);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        SerializedProperty sp_def_recycle_rot_curve = RecycleArgs_Default.FindPropertyRelative("Rotation.Curve");
                        Editor_xHudGUI.Gui_Layout_Property_Field("曲线", sp_def_recycle_rot_curve);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        SerializedProperty sp_def_recycle_rot_ease = RecycleArgs_Default.FindPropertyRelative("Rotation.Ease");
                        Editor_xHudGUI.Gui_Layout_Property_Field("缓动", sp_def_recycle_rot_ease);
                    }
                    #endregion

                    #region 透明度
                    Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_xHudGUI.Gui_Layout_Space(10);
                    def_recycle_fold_alpha = EditorGUILayout.Foldout(def_recycle_fold_alpha, "透明度", true);
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    Editor_xHudGUI.Gui_Layout_Horizontal_End();

                    if (def_recycle_fold_alpha)
                    {
                        SerializedProperty sp_def_recycle_alpha_type = RecycleArgs_Default.FindPropertyRelative("Alpha.Duration");
                        Editor_xHudGUI.Gui_Layout_Property_Field("耗时", sp_def_recycle_alpha_type);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        SerializedProperty sp_def_recycle_alpha_delay = RecycleArgs_Default.FindPropertyRelative("Alpha.Delay");
                        Editor_xHudGUI.Gui_Layout_Property_Field("延迟", sp_def_recycle_alpha_delay);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        SerializedProperty sp_def_recycle_alpha_curve = RecycleArgs_Default.FindPropertyRelative("Alpha.Curve");
                        Editor_xHudGUI.Gui_Layout_Property_Field("曲线", sp_def_recycle_alpha_curve);

                        Editor_xHudGUI.Gui_Layout_Space(5);

                        SerializedProperty sp_def_recycle_alpha_ease = RecycleArgs_Default.FindPropertyRelative("Alpha.Ease");
                        Editor_xHudGUI.Gui_Layout_Property_Field("缓动", sp_def_recycle_alpha_ease);
                    }
                    #endregion

                    EditorGUI.BeginChangeCheck();
                    Editor_xHudGUI.Gui_Layout_Property_Field("动效结束时机", RecycleArgs_MotionAnimateEndState, 85);
                    if (EditorGUI.EndChangeCheck())
                    {
                        MotionAnimateEndState state = (MotionAnimateEndState)RecycleArgs_MotionAnimateEndState.enumValueIndex;
                        switch (state)
                        {
                            case MotionAnimateEndState.以_移动为准:
                                HudMotion_Movement m = (HudMotion_Movement)RecycleArgs_Default.FindPropertyRelative("Movement.Movement").enumValueIndex;
                                if (m == HudMotion_Movement.A_无运动)
                                {
                                    Editor_xHudGUI.Open(xHudDialogType.警告, $"XHud管理器消息", "设定动画结束时机", $"当前位移方式为 <color={hexcol}> A_无运动 </color>，将发生<color={hexcol}> 动效动画无法正常结束</color>从而导致<color={hexcol}> 相应的事件和动作委托 </color>不能正确触发的情况 </color>！请将<color={hexcol}> 位移方式 </color>改为<color={hexcol}> 非无运动方式 </color>！", "明白", 0);
                                    RecycleArgs_MotionAnimateEndState.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
                                }
                                break;
                            case MotionAnimateEndState.以_旋转为准:
                                HudMotion_Rotation r = (HudMotion_Rotation)RecycleArgs_Default.FindPropertyRelative("Rotation.Rotation").enumValueIndex;
                                if (r == HudMotion_Rotation.A_无旋转)
                                {
                                    Editor_xHudGUI.Open(xHudDialogType.警告, $"XHud管理器消息", "设定动画结束时机", $"当前旋转方式为 <color={hexcol}> A_无旋转 </color>，将发生<color={hexcol}> 动效动画无法正常结束</color>从而导致<color={hexcol}> 相应的事件和动作委托 </color>不能正确触发的情况 </color>！请将<color={hexcol}> 旋转方式 </color>改为<color={hexcol}> 非无旋转方式 </color>！", "明白", 0);
                                    RecycleArgs_MotionAnimateEndState.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
                                }
                                break;
                        }
                    }
                }
                Editor_xHudGUI.Gui_Layout_Space(10);
                Editor_xHudGUI.Gui_Layout_Vertical_End();
                #endregion

                #region 多语言
                bool sw_localized = FunctionGroup("多语言", 5, HudFilled.纯色边框, HudColor.亮白, xHud_Dashboard.Theme_Primary, xHud_Dashboard.Theme_Primary, Color.gray, new RectOffset(0, 0, 0, 0), new Vector2(20, 0), PrefsKeyFold_Localized, panel_localized);
                if (sw_localized)
                {
                    #region 单例模式
                    Editor_xHudGUI.Gui_Layout_Toggle<bool, xHud_Manager>("支持多语言", stroptions_support, ref UseLocalization, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                    #endregion
                }
                Editor_xHudGUI.Gui_Layout_Space(5);
                Editor_xHudGUI.Gui_Layout_Vertical_End();
                #endregion

                #region 主题色
                Rect lastRect = GUILayoutUtility.GetLastRect();
                // 获取上一个控件的Rect
                bool sw_themeslider = FunctionGroup("主题色调", 5, HudFilled.纯色边框, HudColor.亮白, xHud_Dashboard.Theme_Primary, xHud_Dashboard.Theme_Primary, Color.gray, new RectOffset(0, 0, 0, 0), new Vector2(20, 0), PrefsKeyFold_Theme, panel_theme);
                if (sw_themeslider)
                {
                    Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, Color.white, 10);
                    if (Editor_xHudGUI.Gui_Layout_Button("重置主题色调", "重置XHud主题色调", HudFilled.实体, HudColor.深空灰, Color.white, 30, new RectOffset(), new Vector2(0, 0)))
                    {
                        ResetThemesColor();
                    }
                    Editor_xHudGUI.Gui_Layout_Horizontal_End(15);

                    Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, Color.white, 5);
                    EditorGUI.BeginChangeCheck();
                    string res_solution = Editor_xHudGUI.Gui_Layout_Popup<string, xHud_Manager>("主题色", ThemeSolutionNames, ref ThemeSolution, HudFilled.实体, 120, 22, new xHud_Manager[] { BaseScript });
                    if (EditorGUI.EndChangeCheck())
                    {
                        Color col = Color.white;
                        switch (res_solution)
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
                        UpdateThemeColor(theme_color.vector3Value);

                        if (xHud_Utilitys.PlayerPrefs_KeyIsExist_ForEditor(PrefsKeyColor_Theme))
                        {
                            Color x = Color.HSVToRGB(h, s, v);
                            xHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyColor_Theme, $"{x.r},{x.g},{x.b}");
                        }
                        #endregion
                    }

                    Editor_xHudGUI.Gui_Layout_Space(10);

                    EditorGUI.BeginChangeCheck();
                    string res_edge_solution = Editor_xHudGUI.Gui_Layout_Popup<string, xHud_Manager>("边框色", ThemeEdgeSolutionNames, ref ThemeEdgeSolution, HudFilled.实体, 120, 22, new xHud_Manager[] { BaseScript });
                    if (EditorGUI.EndChangeCheck())
                    {
                        Color col = Color.white;
                        switch (res_edge_solution)
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
                        UpdateThemeColor_Gp(theme_color_gp.vector3Value);

                        if (xHud_Utilitys.PlayerPrefs_KeyIsExist_ForEditor(PrefsKeyColor_Theme_GP))
                        {
                            Color x = Color.HSVToRGB(h, s, v);
                            xHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyColor_Theme_GP, $"{x.r},{x.g},{x.b}");
                        }
                        #endregion
                    }
                    Editor_xHudGUI.Gui_Layout_Horizontal_End(7);

                    // 定义滑动条的范围
                    float minValue = 0f;
                    float maxValue = 1f;

                    // 绘制滑动条基准坐标
                    Rect sliderRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
                    Vector3 v3 = Vector3.zero;

                    float baseheight = 20;

                    #region Theme_Color
                    v3 = theme_color.vector3Value;
                    EditorGUI.BeginChangeCheck();
                    Editor_xHudGUI.Gui_Labelfield(new Rect(sliderRect.x + 10, sliderRect.y + 10 + baseheight, 48, 15), "主题", HudFilled.无, HudColor.无, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 13);
                    Editor_xHudGUI.Gui_Labelfield(new Rect(sliderRect.x + 10, sliderRect.y + 40 + baseheight, 48, 15), "色调", HudFilled.无, HudColor.无, Color.gray, TextAnchor.MiddleLeft, Vector2.zero, 11);
                    Editor_xHudGUI.Gui_Labelfield(new Rect(sliderRect.x + 10, sliderRect.y + 70 + baseheight, 48, 15), "饱和", HudFilled.无, HudColor.无, Color.gray, TextAnchor.MiddleLeft, Vector2.zero, 11);
                    Editor_xHudGUI.Gui_Labelfield(new Rect(sliderRect.x + 10, sliderRect.y + 100 + baseheight, 48, 15), "明度", HudFilled.无, HudColor.无, Color.gray, TextAnchor.MiddleLeft, Vector2.zero, 11);
                    v3.x = Editor_xHudGUI.Gui_Slider(new Rect(sliderRect.x + 50, sliderRect.y + 40 + baseheight, sliderRect.width - 50, 20), "色调", v3.x, minValue, maxValue);
                    v3.y = Editor_xHudGUI.Gui_Slider(new Rect(sliderRect.x + 50, sliderRect.y + 70 + baseheight, sliderRect.width - 50, 20), "饱和", v3.y, minValue, maxValue);
                    v3.z = Editor_xHudGUI.Gui_Slider(new Rect(sliderRect.x + 50, sliderRect.y + 100 + baseheight, sliderRect.width - 50, 20), "明度", v3.z, minValue, maxValue);
                    theme_color.vector3Value = v3;
                    theme_color.serializedObject.ApplyModifiedProperties();
                    ThemeColorMark(sliderRect, 6 + baseheight, theme_color);
                    if (EditorGUI.EndChangeCheck())
                    {
                        UpdateThemeColor(theme_color.vector3Value);

                        if (xHud_Utilitys.PlayerPrefs_KeyIsExist_ForEditor(PrefsKeyColor_Theme))
                        {
                            Color x = Color.HSVToRGB(v3.x, v3.y, v3.z);
                            xHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyColor_Theme, $"{x.r},{x.g},{x.b}");
                        }
                    }
                    #endregion

                    #region Theme_Color_Group
                    EditorGUI.BeginChangeCheck();
                    v3 = theme_color_gp.vector3Value;
                    Editor_xHudGUI.Gui_Labelfield(new Rect(sliderRect.x + 10, sliderRect.y + 140 + baseheight, 48, 15), "边框", HudFilled.无, HudColor.无, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 13);
                    Editor_xHudGUI.Gui_Labelfield(new Rect(sliderRect.x + 10, sliderRect.y + 170 + baseheight, 48, 15), "色调", HudFilled.无, HudColor.无, Color.gray, TextAnchor.MiddleLeft, Vector2.zero, 11);
                    Editor_xHudGUI.Gui_Labelfield(new Rect(sliderRect.x + 10, sliderRect.y + 200 + baseheight, 48, 15), "饱和", HudFilled.无, HudColor.无, Color.gray, TextAnchor.MiddleLeft, Vector2.zero, 11);
                    Editor_xHudGUI.Gui_Labelfield(new Rect(sliderRect.x + 10, sliderRect.y + 230 + baseheight, 48, 15), "明度", HudFilled.无, HudColor.无, Color.gray, TextAnchor.MiddleLeft, Vector2.zero, 11);
                    v3.x = Editor_xHudGUI.Gui_Slider(new Rect(sliderRect.x + 50, sliderRect.y + 170 + baseheight, sliderRect.width - 50, 20), "色调", v3.x, minValue, maxValue);
                    v3.y = Editor_xHudGUI.Gui_Slider(new Rect(sliderRect.x + 50, sliderRect.y + 200 + baseheight, sliderRect.width - 50, 20), "饱和", v3.y, minValue, maxValue);
                    v3.z = Editor_xHudGUI.Gui_Slider(new Rect(sliderRect.x + 50, sliderRect.y + 230 + baseheight, sliderRect.width - 50, 20), "明度", v3.z, minValue, maxValue);
                    theme_color_gp.vector3Value = v3;
                    theme_color_gp.serializedObject.ApplyModifiedProperties();
                    ThemeColorMark(sliderRect, 136 + baseheight, theme_color_gp);
                    if (EditorGUI.EndChangeCheck())
                    {
                        UpdateThemeColor_Gp(theme_color_gp.vector3Value);

                        if (xHud_Utilitys.PlayerPrefs_KeyIsExist_ForEditor(PrefsKeyColor_Theme_GP))
                        {
                            Color x = Color.HSVToRGB(v3.x, v3.y, v3.z);
                            xHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyColor_Theme_GP, $"{x.r},{x.g},{x.b}");
                        }
                    }
                    #endregion

                    #region Theme_Color_Sep
                    EditorGUI.BeginChangeCheck();
                    v3 = theme_color_sep.vector3Value;
                    Editor_xHudGUI.Gui_Labelfield(new Rect(sliderRect.x + 10, sliderRect.y + 270 + baseheight, 48, 15), "分割", HudFilled.无, HudColor.无, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 13);
                    Editor_xHudGUI.Gui_Labelfield(new Rect(sliderRect.x + 10, sliderRect.y + 300 + baseheight, 48, 15), "色调", HudFilled.无, HudColor.无, Color.gray, TextAnchor.MiddleLeft, Vector2.zero, 11);
                    Editor_xHudGUI.Gui_Labelfield(new Rect(sliderRect.x + 10, sliderRect.y + 330 + baseheight, 48, 15), "饱和", HudFilled.无, HudColor.无, Color.gray, TextAnchor.MiddleLeft, Vector2.zero, 11);
                    Editor_xHudGUI.Gui_Labelfield(new Rect(sliderRect.x + 10, sliderRect.y + 360 + baseheight, 48, 15), "明度", HudFilled.无, HudColor.无, Color.gray, TextAnchor.MiddleLeft, Vector2.zero, 11);
                    v3.x = Editor_xHudGUI.Gui_Slider(new Rect(sliderRect.x + 50, sliderRect.y + 300 + baseheight, sliderRect.width - 50, 20), "色调", v3.x, minValue, maxValue);
                    v3.y = Editor_xHudGUI.Gui_Slider(new Rect(sliderRect.x + 50, sliderRect.y + 330 + baseheight, sliderRect.width - 50, 20), "饱和", v3.y, minValue, maxValue);
                    v3.z = Editor_xHudGUI.Gui_Slider(new Rect(sliderRect.x + 50, sliderRect.y + 360 + baseheight, sliderRect.width - 50, 20), "明度", v3.z, minValue, maxValue);
                    theme_color_sep.vector3Value = v3;
                    theme_color_sep.serializedObject.ApplyModifiedProperties();
                    ThemeColorMark(sliderRect, 266 + baseheight, theme_color_sep);
                    if (EditorGUI.EndChangeCheck())
                    {
                        UpdateThemeColor_Sep(theme_color_sep.vector3Value);

                        if (xHud_Utilitys.PlayerPrefs_KeyIsExist_ForEditor(PrefsKeyColor_Theme_SEP))
                        {
                            Color x = Color.HSVToRGB(v3.x, v3.y, v3.z);
                            xHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyColor_Theme_SEP, $"{x.r},{x.g},{x.b}");
                        }
                    }
                    #endregion



                    Editor_xHudGUI.Gui_Layout_Space(370 + baseheight);
                }
                Editor_xHudGUI.Gui_Layout_Space(5);
                Editor_xHudGUI.Gui_Layout_Vertical_End();
                #endregion

                #region 源脚本
                Editor_xHudGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 3, "源脚本", xHud_Dashboard.Theme_Primary);
                Editor_xHudGUI.Gui_Layout_Space(5);

                #region 原始变量
                Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_xHudGUI.Gui_Layout_Space(10);
                BasicVars = EditorGUILayout.Foldout(BasicVars, "变量/属性", true);
                Editor_xHudGUI.Gui_Layout_Space(5);
                Editor_xHudGUI.Gui_Layout_Horizontal_End();
                if (BasicVars)
                    DrawDefaultInspector();
                #endregion

                Editor_xHudGUI.Gui_Layout_Space(5);
                Editor_xHudGUI.Gui_Layout_Vertical_End();
                #endregion
            }

            // 刷新
            if (!Application.isPlaying)
            {
                EditorManagerUpdate();
            }

            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                // 创建右键菜单
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("F (折叠所有项)"), false, () =>
                            {
                                FunctionGroupPrefsSet(false);
                            });
                menu.ShowAsContext(); // 在鼠标位置显示右键菜单
                Event.current.Use();
            }

            serializedObject.ApplyModifiedProperties();


        }

        #region 主题颜色逻辑
        /// <summary>
        /// 主题颜色标记
        /// </summary>
        /// <param name="sliderRect"></param>
        /// <param name="offset"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private Color ThemeColorMark(Rect sliderRect, float offset, SerializedProperty col)
        {
            // 绘制颜色渐变背景
            Rect gradientRect = new Rect(sliderRect.x, sliderRect.y, sliderRect.width - 80, 2);

            // 绘制完整的颜色渐变背景
            for (int i = 0; i < gradientRect.width; i++)
            {
                float ratio = (float)i / gradientRect.width;
                Color color = Color.HSVToRGB(ratio, 1f, 1f); // 生成HSV颜色
                Rect lineRect = new Rect(gradientRect.x + i + 80, gradientRect.y + 10 + offset, 1, gradientRect.height);
                EditorGUI.DrawRect(lineRect, color);
            }

            // 绘制滑块位置的标记
            Rect handleRect = new Rect(gradientRect.x + gradientRect.width * col.vector3Value.x - 2 + 80, gradientRect.y + 2 + offset, 1, gradientRect.height + 6);
            EditorGUI.DrawRect(handleRect, Color.white);

            return Color.HSVToRGB(col.vector3Value.x, col.vector3Value.y, col.vector3Value.z);
        }

        /// <summary>
        /// 读取主题颜色
        /// </summary>
        private void LoadThemesColor()
        {
            string colorval = xHud_Utilitys.PlayerPrefs_ReadValue_String_ForEditor(PrefsKeyColor_Theme);
            Vector3 ori_col = xHud_Utilitys.Vector3_From_String(colorval);
            Color col = xHud_Utilitys.Color_From_RGBA(ori_col.x, ori_col.y, ori_col.z, 1, false);
            Color ccc = Color.white;
            Color.RGBToHSV(col, out ccc.r, out ccc.g, out ccc.b);
            theme_color.vector3Value = new Vector3(ccc.r, ccc.g, ccc.b);
            theme_color.serializedObject.ApplyModifiedProperties();
            UpdateThemeColor(theme_color.vector3Value);

            string colorval_gp = xHud_Utilitys.PlayerPrefs_ReadValue_String_ForEditor(PrefsKeyColor_Theme_GP);
            Vector3 ori_col_gp = xHud_Utilitys.Vector3_From_String(colorval_gp);
            Color col_gp = xHud_Utilitys.Color_From_RGBA(ori_col_gp.x, ori_col_gp.y, ori_col_gp.z, 1, false);
            Color ccc_gp = Color.white;
            Color.RGBToHSV(col_gp, out ccc_gp.r, out ccc_gp.g, out ccc_gp.b);
            theme_color_gp.vector3Value = new Vector3(ccc_gp.r, ccc_gp.g, ccc_gp.b);
            theme_color_gp.serializedObject.ApplyModifiedProperties();
            UpdateThemeColor_Gp(theme_color_gp.vector3Value);


            string colorval_sep = xHud_Utilitys.PlayerPrefs_ReadValue_String_ForEditor(PrefsKeyColor_Theme_SEP);
            Vector3 ori_col_sep = xHud_Utilitys.Vector3_From_String(colorval_sep);
            Color col_sep = xHud_Utilitys.Color_From_RGBA(ori_col_sep.x, ori_col_sep.y, ori_col_sep.z, 1, false);
            Color ccc_sep = Color.white;
            Color.RGBToHSV(col_sep, out ccc_sep.r, out ccc_sep.g, out ccc_sep.b);
            theme_color_sep.vector3Value = new Vector3(ccc_sep.r, ccc_sep.g, ccc_sep.b);
            theme_color_sep.serializedObject.ApplyModifiedProperties();
            UpdateThemeColor_Sep(theme_color_sep.vector3Value);
        }

        /// <summary>
        /// 重置主题颜色
        /// </summary>
        private void ResetThemesColor()
        {
            EditorPrefs.DeleteKey(PrefsKeyColor_Theme);
            EditorPrefs.DeleteKey(PrefsKeyColor_Theme_GP);
            EditorPrefs.DeleteKey(PrefsKeyColor_Theme_SEP);

            ThemeSolution.stringValue = "默认";
            ThemeEdgeSolution.stringValue = "默认";

            ThemeSolution.serializedObject.ApplyModifiedProperties();
            ThemeEdgeSolution.serializedObject.ApplyModifiedProperties();

            xHud_Dashboard.LoadThemes();

            LoadThemesColor();
        }

        /// <summary>
        /// 更新主题颜色 - 主颜色
        /// </summary>
        /// <param name="col"></param>
        private void UpdateThemeColor(Vector3 col)
        {
            xHud_Dashboard.Theme_Primary = Color.HSVToRGB(col.x, col.y, col.z);
        }

        /// <summary>
        /// 更新主题颜色 - 编组色
        /// </summary>
        /// <param name="col"></param>
        private void UpdateThemeColor_Gp(Vector3 col)
        {
            xHud_Dashboard.Theme_Group = Color.HSVToRGB(col.x, col.y, col.z);
        }

        /// <summary>
        /// 更新主题颜色 - 分割线
        /// </summary>
        /// <param name="col"></param>
        private void UpdateThemeColor_Sep(Vector3 col)
        {
            xHud_Dashboard.Theme_SeperateLine = Color.HSVToRGB(col.x, col.y, col.z);
        }

        #endregion

        #region 辅助

        /// <summary>
        /// 获取目标文件夹下的指定类型所有资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="Pattern"></param>
        /// <returns></returns>
        List<T> LoadAllAssetsAtPathWithIO<T>(string path, string Pattern) where T : UnityEngine.Object
        {
            List<T> _out = new();

            string root_path = Application.dataPath + "/" + path;
            //sp_DebugMode.Log(root_path);

            if (!Directory.Exists(root_path))
            {
                Debug.LogWarning("Path doesn't exist");
                return _out;
            }

            string[] fileEntries = Directory.GetFiles(root_path, $"*{Pattern}");

            foreach (string FileName in fileEntries)
            {
                string[] filepath = FileName.Split(Application.dataPath);
                //sp_DebugMode.Log("Assets" + filepath[1]);
                _out.Add(AssetDatabase.LoadAssetAtPath<T>("Assets" + filepath[1]));
            }

            return _out;
        }
        /// <summary>
        /// 目录图标视觉
        /// </summary>
        private void HierarachyVisualUpdate()
        {
            if (BaseScript.Hud_MouseCursor != null)
            {
                //VHierarchy.VHierarchy.SetIcon(BaseScript.Hud_MouseCursor.gameObject, "Grid.Default");
            }
            if (BaseScript.Hud_TransitionController != null)
            {
                //VHierarchy.VHierarchy.SetIcon(BaseScript.Hud_TransitionController.gameObject, "Canvas Icon");
            }
            if (BaseScript.HudCanvas_World != null)
            {
                //VHierarchy.VHierarchy.SetIcon(BaseScript.HudCanvas_World.gameObject, "CanvasScaler Icon");
            }
            if (BaseScript.HudCanvas_WorldAnchor != null)
            {
                //VHierarchy.VHierarchy.SetIcon(BaseScript.HudCanvas_WorldAnchor.gameObject, "PolygonCollider2D Icon");
            }
            if (target != null)
            {
                //VHierarchy.VHierarchy.SetColor(BaseScript.gameObject, 5);
            }
            if (BaseScript.HudCamera != null)
            {
                //VHierarchy.VHierarchy.SetIcon(BaseScript.HudCamera.gameObject, "SceneViewCamera On");
            }
            if (BaseScript.HudCanvas_Screen != null)
            {
                //VHierarchy.VHierarchy.SetIcon(BaseScript.HudCanvas_Screen.gameObject, "CanvasScaler Icon");
            }
            if (BaseScript.HudCanvas_ScreenAnchor != null)
            {
                //VHierarchy.VHierarchy.SetIcon(BaseScript.HudCanvas_ScreenAnchor.gameObject, "PolygonCollider2D Icon");
            }
            for (int i = 0; i < BaseScript.Anchors_Layout_Screen.Count; i++)
            {
                if (BaseScript.Anchors_Layout_Screen[i].Anchor != null)
                {
                    //VHierarchy.VHierarchy.SetIcon(BaseScript.Anchors_Layout_Screen[i].Anchor.gameObject, "VisualQueryBuilder");
                }
            }
            if (BaseScript.Mask != null)
            {
                //VHierarchy.VHierarchy.SetIcon(BaseScript.Mask.gameObject, "AspectRatioFitter Icon");
            }
            if (BaseScript.BlurMask != null)
            {
                //VHierarchy.VHierarchy.SetIcon(BaseScript.BlurMask.gameObject, "AspectRatioFitter Icon");
            }
            if (BaseScript.Hud_EventSystem != null)
            {
                //VHierarchy.VHierarchy.SetIcon(BaseScript.Hud_EventSystem.gameObject, "Settings Icon");
            }
        }
        /// <summary>
        /// 重置生成与回收的参数到默认
        /// </summary>
        private void ResetMotionParams(string state)
        {
            if (state == "CreateArgs")
            {
                CreateArgs_Default.FindPropertyRelative("anchor").enumValueIndex = (int)HudAnchor.中心;
                CreateArgs_Default.FindPropertyRelative("Movement.Movement").enumValueIndex = (int)HudMotion_Movement.S_从下至上;
                CreateArgs_Default.FindPropertyRelative("Movement.Distance").floatValue = 100;
                CreateArgs_Default.FindPropertyRelative("Movement.Duration").floatValue = 1;
                CreateArgs_Default.FindPropertyRelative("Movement.Delay").floatValue = 0;
                CreateArgs_Default.FindPropertyRelative("Movement.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
                CreateArgs_Default.FindPropertyRelative("Movement.CurveName").stringValue = "";
                CreateArgs_Default.FindPropertyRelative("Movement.Ease").enumValueIndex = (int)Ease.OutQuart;
                CreateArgs_Default.FindPropertyRelative("Rotation.Rotation").enumValueIndex = (int)HudMotion_Rotation.A_无旋转;
                CreateArgs_Default.FindPropertyRelative("Rotation.Degree").floatValue = 0;
                CreateArgs_Default.FindPropertyRelative("Rotation.Duration").floatValue = 1;
                CreateArgs_Default.FindPropertyRelative("Rotation.Delay").floatValue = 0;
                CreateArgs_Default.FindPropertyRelative("Rotation.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
                CreateArgs_Default.FindPropertyRelative("Rotation.CurveName").stringValue = "";
                CreateArgs_Default.FindPropertyRelative("Rotation.Ease").enumValueIndex = (int)Ease.OutQuart;
                CreateArgs_Default.FindPropertyRelative("Alpha.Duration").floatValue = 1;
                CreateArgs_Default.FindPropertyRelative("Alpha.Delay").floatValue = 0;
                CreateArgs_Default.FindPropertyRelative("Alpha.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
                CreateArgs_Default.FindPropertyRelative("Alpha.CurveName").stringValue = "";
                CreateArgs_Default.FindPropertyRelative("Alpha.Ease").enumValueIndex = (int)Ease.OutQuart;
                CreateArgs_MotionAnimateEndState.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
                CreateArgs_Default.serializedObject.ApplyModifiedProperties();
            }
            else if (state == "RecycleArgs")
            {
                RecycleArgs_Default.FindPropertyRelative("Movement.Movement").enumValueIndex = (int)HudMotion_Movement.D_从上至下;
                RecycleArgs_Default.FindPropertyRelative("Movement.Distance").floatValue = 100;
                RecycleArgs_Default.FindPropertyRelative("Movement.Duration").floatValue = 1;
                RecycleArgs_Default.FindPropertyRelative("Movement.Delay").floatValue = 0;
                RecycleArgs_Default.FindPropertyRelative("Movement.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
                RecycleArgs_Default.FindPropertyRelative("Movement.CurveName").stringValue = "";
                RecycleArgs_Default.FindPropertyRelative("Movement.Ease").enumValueIndex = (int)Ease.OutQuart;
                RecycleArgs_Default.FindPropertyRelative("Rotation.Rotation").enumValueIndex = (int)HudMotion_Rotation.A_无旋转;
                RecycleArgs_Default.FindPropertyRelative("Rotation.Degree").floatValue = 0;
                RecycleArgs_Default.FindPropertyRelative("Rotation.Duration").floatValue = 1;
                RecycleArgs_Default.FindPropertyRelative("Rotation.Delay").floatValue = 0;
                RecycleArgs_Default.FindPropertyRelative("Rotation.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
                RecycleArgs_Default.FindPropertyRelative("Rotation.CurveName").stringValue = "";
                RecycleArgs_Default.FindPropertyRelative("Rotation.Ease").enumValueIndex = (int)Ease.OutQuart;
                RecycleArgs_Default.FindPropertyRelative("Alpha.Duration").floatValue = 1;
                RecycleArgs_Default.FindPropertyRelative("Alpha.Delay").floatValue = 0;
                RecycleArgs_Default.FindPropertyRelative("Alpha.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
                RecycleArgs_Default.FindPropertyRelative("Alpha.CurveName").stringValue = "";
                RecycleArgs_Default.FindPropertyRelative("Alpha.Ease").enumValueIndex = (int)Ease.OutQuart;
                RecycleArgs_MotionAnimateEndState.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
                RecycleArgs_Default.serializedObject.ApplyModifiedProperties();
            }
        }
        /// <summary>
        /// LED闪烁效果
        /// </summary>
        /// <returns></returns>
        IEnumerator LedBlinker()
        {
            // 控制是否增加或减少
            bool isIncreasing = true;

            while (true)
            {
                if (EnabledLedEffect.boolValue)
                {
                    if (!Application.isPlaying)
                    {
                        if (isIncreasing)
                        {
                            // 增加阶段
                            LedAlpha += increaseSpeed;
                            if (LedAlpha >= led_higher)
                            {
                                LedAlpha = led_higher; // 确保不超过1
                                isIncreasing = false; // 切换到减少阶段
                            }
                        }
                        else
                        {
                            // 减少阶段
                            LedAlpha -= decreaseSpeed;
                            if (LedAlpha <= led_lower)
                            {
                                LedAlpha = led_lower; // 确保不超过0
                                isIncreasing = true; // 切换到增加阶段
                            }
                        }
                        Repaint();
                    }
                    else
                    {
                        isIncreasing = true;
                        LedAlpha = 1;
                    }
                }
                else
                {
                    isIncreasing = true;
                    LedAlpha = 1;
                }
                yield return new EditorWaitForSeconds(led_rate);
            }
        }
        /// <summary>
        /// Led闪烁布局
        /// </summary>
        private void LedBlink(Rect rect)
        {
            Editor_xHudGUI.Gui_Icon(new Rect(rect.x + 112, rect.y - 5, 32, 32), icon_led_off);
            if (!Application.isPlaying)
            {
                GUI.backgroundColor = new Color(xHud_Dashboard.Theme_Primary.r, xHud_Dashboard.Theme_Primary.g, xHud_Dashboard.Theme_Primary.b, LedAlpha);
            }
            else
                GUI.backgroundColor = xHud_Dashboard.Theme_Primary;
            Editor_xHudGUI.Gui_Icon(new Rect(rect.x + 112, rect.y - 5, 32, 32), icon_led_halo);

            if (!Application.isPlaying)
            {
                GUI.backgroundColor = new Color(1, 1, 1, LedAlpha);
            }
            else
                GUI.backgroundColor = Color.white;
            Editor_xHudGUI.Gui_Icon(new Rect(rect.x + 112, rect.y - 5, 32, 32), icon_led_on);

            GUI.backgroundColor = Color.white;
        }
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
        /// 刷新
        /// </summary>
        private void EditorManagerUpdate()
        {
            if (SceneCamera.objectReferenceValue != null && ScreenRes.vector2Value != Vector2.zero && UseSafeFrame.boolValue)
                HiddenStructureDisplayer(SafeFrameStructureDisplayer.boolValue);

            BaseScript.hm_Layout_Update();

            BaseScript.hm_SafeFrameUpdate();

            BaseScript.hm_MaskUpdate();

            BaseScript.hm_BlurMaskUpdate();

            BaseScript.hm_TransitionTopView();

            BaseScript.hm_ContentAlpha_Update();

            BaseScript.hm_Layout_CanvasDistance(BaseScript.HudCanvasAnchor);

            BaseScript.hm_CameraCutterRange(BaseScript.CameraCutter_Near, BaseScript.CameraCutter_Far);

            BaseScript.hm_CameraOrthographicProjection(CameraOthograpicMode.boolValue);

            BaseScript.hm_CameraOrthographicSize(BaseScript.CameraOrthographicSize);

            BaseScript.hm_CameraPerspectiveFov(BaseScript.CameraFov);

            BaseScript.hm_Layout_CanvasDistance_Update();

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
        /// <summary>
        /// 标题面板
        /// </summary>
        /// <param name="title"></param>
        /// <param name="margin"></param>
        /// <param name="Fill"></param>
        /// <param name="color"></param>
        /// <param name="titlecolor"></param>
        /// <param name="titlecolor_hover"></param>
        /// <param name="titlecolor_active"></param>
        /// <param name="margin_btn"></param>
        /// <param name="offset"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool FunctionGroup(string title, float margin, HudFilled Fill, HudColor color, Color titlecolor, Color titlecolor_hover, Color titlecolor_active, RectOffset margin_btn, Vector2 offset, string key, Texture2D icon)
        {
            int inspectorwidth = Screen.width;
            //sp_DebugMode.Log(inspectorwidth);

            bool sw_option = xHud_Utilitys.PlayerPrefs_ReadValue_Bool_ForEditor(key);
            sw_option = Editor_xHudGUI.Gui_Layout_Vertical_Start_WithFolder(Fill, color, margin, title, titlecolor, titlecolor_hover, titlecolor_active, margin_btn, offset, icon, sw_option, inspectorwidth);
            xHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(key, sw_option);
            return sw_option;
        }
        /// <summary>
        /// 面板折叠键值检查
        /// </summary>
        private void FunctionGroupPrefsCheck()
        {
            if (!xHud_Utilitys.PlayerPrefs_KeyIsExist_ForEditor(PrefsKeyFold_Option))
            {
                xHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyFold_Option, false);
            }
            if (!xHud_Utilitys.PlayerPrefs_KeyIsExist_ForEditor(PrefsKeyFold_CanvasCamera))
            {
                xHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyFold_CanvasCamera, false);
            }
            if (!xHud_Utilitys.PlayerPrefs_KeyIsExist_ForEditor(PrefsKeyFold_Camera))
            {
                xHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyFold_Camera, false);
            }
            if (!xHud_Utilitys.PlayerPrefs_KeyIsExist_ForEditor(PrefsKeyFold_Libs))
            {
                xHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyFold_Libs, false);
            }
            if (!xHud_Utilitys.PlayerPrefs_KeyIsExist_ForEditor(PrefsKeyFold_FrameLayout))
            {
                xHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyFold_FrameLayout, false);
            }
            if (!xHud_Utilitys.PlayerPrefs_KeyIsExist_ForEditor(PrefsKeyFold_Assist))
            {
                xHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyFold_Assist, false);
            }
            if (!xHud_Utilitys.PlayerPrefs_KeyIsExist_ForEditor(PrefsKeyFold_BluePrint))
            {
                xHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyFold_BluePrint, false);
            }
            if (!xHud_Utilitys.PlayerPrefs_KeyIsExist_ForEditor(PrefsKeyFold_LayoutMatchRes))
            {
                xHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyFold_LayoutMatchRes, false);
            }
            if (!xHud_Utilitys.PlayerPrefs_KeyIsExist_ForEditor(PrefsKeyFold_Audios))
            {
                xHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyFold_Audios, false);
            }
            if (!xHud_Utilitys.PlayerPrefs_KeyIsExist_ForEditor(PrefsKeyFold_ElementLibs))
            {
                xHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyFold_ElementLibs, false);
            }
            if (!xHud_Utilitys.PlayerPrefs_KeyIsExist_ForEditor(PrefsKeyFold_Mask))
            {
                xHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyFold_Mask, false);
            }
            if (!xHud_Utilitys.PlayerPrefs_KeyIsExist_ForEditor(PrefsKeyFold_BlurMask))
            {
                xHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyFold_BlurMask, false);
            }
            if (!xHud_Utilitys.PlayerPrefs_KeyIsExist_ForEditor(PrefsKeyFold_Global))
            {
                xHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyFold_Global, false);
            }
            if (!xHud_Utilitys.PlayerPrefs_KeyIsExist_ForEditor(PrefsKeyFold_Tools))
            {
                xHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyFold_Tools, false);
            }
            if (!xHud_Utilitys.PlayerPrefs_KeyIsExist_ForEditor(PrefsKeyFold_PhysicsRatio))
            {
                xHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyFold_PhysicsRatio, false);
            }
            if (!xHud_Utilitys.PlayerPrefs_KeyIsExist_ForEditor(PrefsKeyFold_Localized))
            {
                xHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyFold_Localized, false);
            }
            if (!xHud_Utilitys.PlayerPrefs_KeyIsExist_ForEditor(PrefsKeyFold_Comp_Guide))
            {
                xHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyFold_Comp_Guide, false);
            }
            if (!xHud_Utilitys.PlayerPrefs_KeyIsExist_ForEditor(PrefsKeyFold_ElementParam_DefaultMotion))
            {
                xHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyFold_ElementParam_DefaultMotion, false);
            }
            if (!xHud_Utilitys.PlayerPrefs_KeyIsExist_ForEditor(PrefsKeyFold_Theme))
            {
                xHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyFold_Theme, false);
            }
            if (!xHud_Utilitys.PlayerPrefs_KeyIsExist_ForEditor(PrefsKeyColor_Theme))
            {
                xHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyColor_Theme, "1.000, 1.000, 1.000, 1.000");
            }
            if (!xHud_Utilitys.PlayerPrefs_KeyIsExist_ForEditor(PrefsKeyColor_Theme_GP))
            {
                xHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyColor_Theme_GP, "1.000, 1.000, 1.000, 1.000");
            }
            if (!xHud_Utilitys.PlayerPrefs_KeyIsExist_ForEditor(PrefsKeyColor_Theme_SEP))
            {
                xHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyColor_Theme_SEP, "1.000, 1.000, 1.000, 1.000");
            }
        }
        /// <summary>
        /// 面板折叠键值设置
        /// </summary>
        private void FunctionGroupPrefsSet(bool state)
        {
            xHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyFold_Option, state);
            xHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyFold_CanvasCamera, state);
            xHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyFold_Camera, state);
            xHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyFold_Libs, state);
            xHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyFold_FrameLayout, state);
            xHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyFold_Assist, state);
            xHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyFold_BluePrint, state);
            xHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyFold_LayoutMatchRes, state);
            xHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyFold_Audios, state);
            xHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyFold_ElementLibs, state);
            xHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyFold_Mask, state);
            xHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyFold_BlurMask, state);
            xHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyFold_Global, state);
            xHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyFold_Tools, state);
            xHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyFold_PhysicsRatio, state);
            xHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyFold_Localized, state);
            xHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyFold_Comp_Guide, state);
            xHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyFold_ElementParam_DefaultMotion, state);
            xHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyFold_Theme, state);

            Repaint();
        }
        /// <summary>
        /// 序列化变量
        /// </summary>
        private SerializedProperty GetSerializedProperty(string name)
        {
            return serializedObject.FindProperty(name);
        }
        /// <summary>
        /// 获取序列化变量
        /// </summary>
        private void SerializedAllVariables()
        {
            IsInitialized = GetSerializedProperty("IsInitialized");
            HudCamera = GetSerializedProperty("HudCamera");
            HudCanvas_Screen = GetSerializedProperty("HudCanvas_Screen");
            HudCanvasScaler = GetSerializedProperty("HudCanvasScaler");
            Mask = GetSerializedProperty("Mask");
            HudCanvasGroup_Screen = GetSerializedProperty("HudCanvasGroup_Screen");
            HudCanvas_ScreenAnchor = GetSerializedProperty("HudCanvas_ScreenAnchor");
            Anchors_Layout_Screen = GetSerializedProperty("Anchors_Layout_Screen");
            Lib_ElementLibrarys = GetSerializedProperty("Hud_ElementLibrarys");
            Lib_Color = GetSerializedProperty("Hud_Colors");
            Lib_Curve = GetSerializedProperty("Hud_Curves");
            Lib_Sound = GetSerializedProperty("Hud_Sounds");
            Lib_TextStyleLibrary = GetSerializedProperty("Hud_TextStyleLibrary");
            Lib_ElementMotion = GetSerializedProperty("Hud_ElementMotion");
            Lib_Transition = GetSerializedProperty("Hud_TransitionLib");
            SoundLibrary = GetSerializedProperty("Pool_Sounder");
            SounderPoolCount = GetSerializedProperty("SounderPoolCount");
            UseInstanceMode = GetSerializedProperty("UseInstanceMode");
            UseDebug = GetSerializedProperty("UseDebug");
            UseSafeFrame = GetSerializedProperty("UseSafeFrame");
            UsePerfectPixelUpdate = GetSerializedProperty("UsePerfectPixelUpdate");
            UseAutoPerfectPixel = GetSerializedProperty("UseAutoPerfectPixel");
            SafeFrameStructureDisplayer = GetSerializedProperty("SafeFrameStructureDisplayer");
            CameraOthograpicMode = GetSerializedProperty("CameraOthograpicMode");
            CanvasScalerModeIndex = GetSerializedProperty("CanvasScalerModeIndex");
            CanvasScalerScreenSize = GetSerializedProperty("CanvasScalerScreenSize");
            CanvasMatchDir = GetSerializedProperty("CanvasMatchDir");
            SupportWorldUI = GetSerializedProperty("SupportWorldUI");
            BluePrintMode = GetSerializedProperty("BluePrintMode");
            Safe_Frame = GetSerializedProperty("Safe_Frame");
            Margins = GetSerializedProperty("Margins");
            MarginHorizontal = GetSerializedProperty("MarginHorizontal");
            MarginMultiply = GetSerializedProperty("MarginMultiply");
            MarginVertical = GetSerializedProperty("MarginVertical");
            MarkSize = GetSerializedProperty("MarkSize");
            Color_LayoutAnchorMark = GetSerializedProperty("Color_LayoutAnchorMark");
            Color_FrameLine = GetSerializedProperty("Color_FrameLine");
            ScreenRes = GetSerializedProperty("ScreenRes");
            Safe_FrameLine_Width = GetSerializedProperty("Safe_FrameLine_Width");
            Safe_FrameLine_Margins = GetSerializedProperty("Safe_FrameLine_Margins");
            Color_SeperaterLine = GetSerializedProperty("Color_SeperaterLine");
            Safe_Seperater_Length = GetSerializedProperty("Safe_Seperater_Length");
            Safe_CenterMarkLength = GetSerializedProperty("Safe_CenterMarkLength");
            Safe_CenterMarkWidth = GetSerializedProperty("Safe_CenterMarkWidth");
            Color_CenterMark = GetSerializedProperty("Color_CenterMark");
            Safe_CenterMarkDistance = GetSerializedProperty("Safe_CenterMarkDistance");
            CameraOrthographicSize = GetSerializedProperty("CameraOrthographicSize");
            CameraFov = GetSerializedProperty("CameraFov");
            CameraCutter_Near = GetSerializedProperty("CameraCutter_Near");
            CameraCutter_Far = GetSerializedProperty("CameraCutter_Far");
            HudCanvasAnchor = GetSerializedProperty("HudCanvasAnchor");
            CanvasDistance = GetSerializedProperty("CanvasDistance");
            HudCanvasAnchorIndex = GetSerializedProperty("HudCanvasAnchorIndex");
            MaskAlpha = GetSerializedProperty("MaskAlpha");
            MaskTexture = GetSerializedProperty("MaskTexture");
            MaskRaycastAlphaThreshold = GetSerializedProperty("MaskRaycastAlphaThreshold");
            MaskColor = GetSerializedProperty("MaskColor");
            BlurMask = GetSerializedProperty("BlurMask");
            BlurMaskAlpha = GetSerializedProperty("BlurMaskAlpha");
            BlurMaskColor = GetSerializedProperty("BlurMaskColor");
            BlurMaskTexture = GetSerializedProperty("BlurMaskTexture");
            BlurMaskRaycastAlphaThreshold = GetSerializedProperty("BlurMaskRaycastAlphaThreshold");
            BlurMaskRaycastEnabled = GetSerializedProperty("BlurMaskRaycastEnabled");
            ContentAlpha_Screen = GetSerializedProperty("ContentAlpha_Screen");
            ContentAlpha_World = GetSerializedProperty("ContentAlpha_World");
            DurationMultiply = GetSerializedProperty("DurationMultiply");
            CustomCursor = GetSerializedProperty("CustomCursor");
            CustomTransition = GetSerializedProperty("CustomTransition");
            Hud_MouseCursor = GetSerializedProperty("Hud_MouseCursor");
            Hud_TransitionController = GetSerializedProperty("Hud_TransitionController");
            SceneCamera = GetSerializedProperty("SceneCamera");
            MaskRaycastEnabled = GetSerializedProperty("MaskRaycastEnabled");
            Volume = GetSerializedProperty("Volume");
            VolumeMute = GetSerializedProperty("VolumeMute");
            HudCanvas_World = GetSerializedProperty("HudCanvas_World");
            HudCanvas_WorldAnchor = GetSerializedProperty("HudCanvas_WorldAnchor");
            HudCanvasGroup_World = GetSerializedProperty("HudCanvasGroup_World");
            RecycleArgs_Default = GetSerializedProperty("RecycleArgs_Default");
            CreateArgs_Default = GetSerializedProperty("CreateArgs_Default");
            FontSizeMultiply = GetSerializedProperty("FontSizeMultiply");
            BluePrint_root = GetSerializedProperty("BluePrint_root");
            BluePrint_grid_size = GetSerializedProperty("BluePrint_grid_size");
            BluePrint_linewidth = GetSerializedProperty("BluePrint_linewidth");
            BluePrint_grid_color = GetSerializedProperty("BluePrint_grid_color");
            BluePrint_bg_color = GetSerializedProperty("BluePrint_bg_color");
            BluePrint_bg_decal_color = GetSerializedProperty("BluePrint_bg_decal_color");
            BluePrint_marktitle_color = GetSerializedProperty("BluePrint_marktitle_color");
            BluePrint_marksubtitle_color = GetSerializedProperty("BluePrint_marksubtitle_color");
            BluePrint_mark_size = GetSerializedProperty("BluePrint_mark_size");
            BluePrint_mark_opacity = GetSerializedProperty("BluePrint_mark_opacity");
            BluePrint_mark_margin = GetSerializedProperty("BluePrint_mark_margin");
            BluePrint_mark_space = GetSerializedProperty("BluePrint_mark_space");
            BluePrint_title_content = GetSerializedProperty("BluePrint_title_content");
            BluePrint_subtitle_content = GetSerializedProperty("BluePrint_subtitle_content");
            BluePrint_MarkAnchors = GetSerializedProperty("BluePrint_MarkAnchors");
            BluePrint_opacity = GetSerializedProperty("BluePrint_opacity");
            BluePrint_Grid_AnimationEase = GetSerializedProperty("BluePrint_Grid_AnimationEase");
            BluePrint_Bg_AnimationEase_In = GetSerializedProperty("BluePrint_Bg_AnimationEase_In");
            BluePrint_Bg_AnimationEase_Out = GetSerializedProperty("BluePrint_Bg_AnimationEase_Out");
            BluePrint_Grid_LengthPercentage = GetSerializedProperty("BluePrint_Grid_LengthPercentage");
            BluePrint_Grid_LevelHeight = GetSerializedProperty("BluePrint_Grid_LevelHeight");
            BluePrint_AnimationDuration = GetSerializedProperty("BluePrint_AnimationDuration");
            BluePrint_Displayed = GetSerializedProperty("BluePrint_Displayed");
            BluePrint_OnStartHide = GetSerializedProperty("BluePrint_OnStartHide");
            BluePrint_bg_opacity = GetSerializedProperty("BluePrint_bg_opacity");
            BluePrint_bg_mapOpacity = GetSerializedProperty("BluePrint_bg_mapOpacity");
            BluePrint_Bg_FadeAnimationDelay = GetSerializedProperty("BluePrint_Bg_FadeAnimationDelay");
            BluePrint_GridEnd = GetSerializedProperty("BluePrint_GridEnd");
            BluePrint_bg_tilling = GetSerializedProperty("BluePrint_bg_tilling");
            BluePrint_bg_name = GetSerializedProperty("BluePrint_bg_name");
            BluePrint_bg_usetilling_index = GetSerializedProperty("BluePrint_bg_usetilling_index");
            BluePrint_bg_usesquareratio_index = GetSerializedProperty("BluePrint_bg_usesquareratio_index");
            BluePrint_grid_Opacity = GetSerializedProperty("BluePrint_grid_Opacity");
            BluePrint_Grid_AnimationDuration = GetSerializedProperty("BluePrint_Grid_AnimationDuration");
            Eft_Grid = GetSerializedProperty("Eft_Grid");
            Eft_GridFade = GetSerializedProperty("Eft_GridFade");
            Eft_Bg = GetSerializedProperty("Eft_Bg");
            Eft_Mark = GetSerializedProperty("Eft_Mark");
            RMS_Enabled = GetSerializedProperty("RMS_Enabled");
            RMS_Nodes = GetSerializedProperty("RMS_Nodes");
            RMS_CurrentSolution = GetSerializedProperty("RMS_CurrentSolution");
            UniversalFeature_Blur_Intensity = GetSerializedProperty("UniversalFeature_Blur_Intensity");
            theme_color = GetSerializedProperty("theme_color");
            theme_color_gp = GetSerializedProperty("theme_color_gp");
            theme_color_sep = GetSerializedProperty("theme_color_sep");
            EnabledLedEffect = GetSerializedProperty("EnabledLedEffect");
            ThemeSolution = GetSerializedProperty("ThemeSolution");
            ThemeEdgeSolution = GetSerializedProperty("ThemeEdgeSolution");
            Hud_EventSystem = GetSerializedProperty("Hud_EventSystem");
            Hud_InputSystemUIInputModule = GetSerializedProperty("Hud_InputSystemUIInputModule");
            CreateArgs_MotionAnimateEndState = CreateArgs_Default.FindPropertyRelative("MotionAnimateEndState");
            RecycleArgs_MotionAnimateEndState = RecycleArgs_Default.FindPropertyRelative("MotionAnimateEndState");
            sp_PhysicsScreenSize = GetSerializedProperty("PhysicsScreenSize");
            sp_Reference_Image = GetSerializedProperty("Reference_Image");
            UseRatioReference = GetSerializedProperty("UseRatioReference");
            RatioReferenceIsPart = GetSerializedProperty("RatioReferenceIsPart");
            sp_Reference_Image_Color = GetSerializedProperty("Reference_Image_Color");
            sp_Res_Full = GetSerializedProperty("Res_Full");
            sp_Res_Part = GetSerializedProperty("Res_Part");
            sp_ReferShape_RatioSize = GetSerializedProperty("ReferShape_RatioSize");
            sp_ReferShape_RatioTolerance = GetSerializedProperty("ReferShape_RatioTolerance");
            FoldAllPanelWithDisabled = GetSerializedProperty("FoldAllPanelWithDisabled");
            CompGuide_Anchors = GetSerializedProperty("CompGuide_Anchors");
            CompGuide_AnchorRoot = GetSerializedProperty("CompGuide_AnchorRoot");

            UseCompGuide = GetSerializedProperty("UseCompGuide");
            CompGuideMode = GetSerializedProperty("CompGuideMode");
            GuideColor = GetSerializedProperty("GuideColor");
            GuidePointColor = GetSerializedProperty("GuidePointColor");

            GuideParam_Mirror_LR_Offset = GetSerializedProperty("GuideParam_Mirror_LR_Offset");
            GuideParam_Mirror_LR_GoldenMode = GetSerializedProperty("GuideParam_Mirror_LR_GoldenMode");

            GuideParam_Mirror_UD_Offset = GetSerializedProperty("GuideParam_Mirror_UD_Offset");
            GuideParam_Mirror_UD_GoldenMode = GetSerializedProperty("GuideParam_Mirror_UD_GoldenMode");
            GuideParam_Fibonacci_Mode = GetSerializedProperty("GuideParam_Fibonacci_Mode");
            GuideParam_CornerLookat_Offset_H = GetSerializedProperty("GuideParam_CornerLookat_Offset_H");
            GuideParam_CornerLookat_Offset_V = GetSerializedProperty("GuideParam_CornerLookat_Offset_V");

            GuideParam_Three_Offset_H = GetSerializedProperty("GuideParam_Three_Offset_H");
            GuideParam_Three_Offset_V = GetSerializedProperty("GuideParam_Three_Offset_V");
            GuideParam_Three_Offset_Coverage = GetSerializedProperty("GuideParam_Three_Offset_Coverage");

            GuideParam_GuideLine_BaseOffset = GetSerializedProperty("GuideParam_GuideLine_BaseOffset");
            GuideParam_GuideLine_BaseHeight = GetSerializedProperty("GuideParam_GuideLine_BaseHeight");
            GuideParam_GuideLine_Offset_Near = GetSerializedProperty("GuideParam_GuideLine_Offset_Near");
            GuideParam_GuideLine_Offset_Far = GetSerializedProperty("GuideParam_GuideLine_Offset_Far");
            GuideParam_GuideLine_Offset_NearHeight = GetSerializedProperty("GuideParam_GuideLine_Offset_NearHeight");

            GuideParam_Triangle_TopOffset = GetSerializedProperty("GuideParam_Triangle_TopOffset");
            GuideParam_Triangle_BaseHeight = GetSerializedProperty("GuideParam_Triangle_BaseHeight");
            GuideParam_Triangle_BottomHeight = GetSerializedProperty("GuideParam_Triangle_BottomHeight");
            GuideParam_Triangle_Offset_Left = GetSerializedProperty("GuideParam_Triangle_Offset_Left");
            GuideParam_Triangle_Offset_Right = GetSerializedProperty("GuideParam_Triangle_Offset_Right");
            GuideParam_CenterPointSize = GetSerializedProperty("GuideParam_CenterPointSize");

            GuideParam_IShape_TopHeight = GetSerializedProperty("GuideParam_IShape_TopHeight");
            GuideParam_IShape_BottomHeight = GetSerializedProperty("GuideParam_IShape_BottomHeight");
            GuideParam_IShape_Offset_Left = GetSerializedProperty("GuideParam_IShape_Offset_Left");
            GuideParam_IShape_Offset_Right = GetSerializedProperty("GuideParam_IShape_Offset_Right");


            Crc_Lib_Name = GetSerializedProperty("Crc_Lib_Name");
            Rec_Lib_Name = GetSerializedProperty("Rec_Lib_Name");

            UseLocalization = GetSerializedProperty("UseLocalization");
        }
        /// <summary>
        /// 初始化HudManager的子结构
        /// </summary>
        private void HudStructure_Create()
        {
            RenderPipelineAsset currentPipelineAsset = GraphicsSettings.currentRenderPipeline;
            if (currentPipelineAsset == null)
            {
                Editor_xHudGUI.Open(xHudDialogType.警告, "XHud管理器消息", "渲染管线异常", "请先指定URP渲染管线资源后再试！", "明白", 0);
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
                Editor_xHudGUI.Open(xHudDialogType.警告, "XHud管理器消息", "渲染管线异常", "请确认当前渲染管线资源文件的渲染器列表的有效性！", "明白", 0);
            }

            if (IsInitialized.boolValue)
            {
                string res = Editor_xHudGUI.Open(xHudDialogType.警告, "XHud管理器消息", "初始化XHud管理器", "接下来会自动创建HudManager的所有子结构组成，在这之前会先清空所有现存的子物体，请确定是否继续该操作？", "初始化", "暂不", 0);
                if (res == "暂不")
                {
                    return;
                }

                Anchors_Layout_Screen.ClearArray();
                Anchors_Layout_Screen.serializedObject.ApplyModifiedProperties();

                // 删除所有子物体
                foreach (Transform child in BaseScript.transform)
                {
                    DestroyImmediate(child.gameObject, true);
                }

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

            ///-------Cam
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

            ///-------Canvas-Screen
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
            HudCanvasAnchor.enumValueIndex = (int)CanvasAnchor.CameraFar;
            HudCanvasAnchor.serializedObject.ApplyModifiedProperties();

            ///-------Anchors
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

            ///-------ScreenMask
            GameObject obj_Mask = new GameObject();
            obj_Mask.layer = LayerMask.NameToLayer("XHud");
            UnityEngine.RectTransform m_Mask = obj_Mask.AddComponent<UnityEngine.RectTransform>();
            Image Mask_img = obj_Mask.AddComponent<Image>();
            Mask_img.color = Color.black;
            m_Mask.name = "Mask";
            m_Mask.SetParent(cav.transform);
            m_Mask.localPosition = Vector3.zero;
            m_Mask.localScale = Vector3.one;
            m_Mask.anchorMin = new Vector2(0, 0);
            m_Mask.anchorMax = new Vector2(1, 1);
            m_Mask.sizeDelta = new Vector2(0, 0);
            Mask.objectReferenceValue = Mask_img;
            Mask.serializedObject.ApplyModifiedProperties();

            ///-------BlurMask
            GameObject obj_BlurMask = new GameObject();
            obj_BlurMask.layer = LayerMask.NameToLayer("XHud");
            UnityEngine.RectTransform m_BlurMask = obj_BlurMask.AddComponent<UnityEngine.RectTransform>();
            Image BlurMask_img = obj_BlurMask.AddComponent<Image>();
            BlurMask_img.color = Color.black;
            BlurMask_img.material = AssetDatabase.LoadAssetAtPath<Material>($"Assets/SevenStrikeModules/XHud/ThirdPlugin/Universal-Blur/Materials/UniversalBlur.mat");

            m_BlurMask.name = "BlurMask";
            m_BlurMask.SetParent(cav.transform);
            m_BlurMask.localPosition = Vector3.zero;
            m_BlurMask.localScale = Vector3.one;
            m_BlurMask.anchorMin = new Vector2(0, 0);
            m_BlurMask.anchorMax = new Vector2(1, 1);
            m_BlurMask.sizeDelta = new Vector2(0, 0);
            BlurMask.objectReferenceValue = BlurMask_img;
            BlurMask.serializedObject.ApplyModifiedProperties();

            ///-------EventSystem
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
            sp_res_full_sprite.objectReferenceValue = AssetDatabase.LoadAssetAtPath<Sprite>($"{xHud_Dashboard.Get_GUIStyle_Path()}Icon/Icons_Hud_Manager/HumanRefer/Body_Solid.png");
            sp_res_full_size.vector2Value = new Vector2(47, 172);
            sp_Res_Full.serializedObject.ApplyModifiedProperties();

            SerializedProperty sp_res_part_sprite = sp_Res_Part.FindPropertyRelative("sprite");
            SerializedProperty sp_res_part_size = sp_Res_Part.FindPropertyRelative("size");
            sp_res_part_sprite.objectReferenceValue = AssetDatabase.LoadAssetAtPath<Sprite>($"{xHud_Dashboard.Get_GUIStyle_Path()}Icon/Icons_Hud_Manager/HumanRefer/Head_Solid.png");
            sp_res_part_size.vector2Value = new Vector2(19, 30);
            sp_Res_Part.serializedObject.ApplyModifiedProperties();

            sp_Reference_Image_Color.colorValue = new Color(0.6f, 0.6f, 0.6f, 0.05f);
            sp_Reference_Image_Color.serializedObject.ApplyModifiedProperties();
            #endregion

            #region 创建XTween动画管理器
            GameObject xtween_obj = new GameObject();
            xtween_obj.name = "XTween(Manager)";
            xtween_obj.layer = LayerMask.NameToLayer("XHud");
            xtween_obj.transform.SetParent(BaseScript.transform);
            xtween_obj.transform.localPosition = Vector3.zero;
            xtween_obj.transform.localEulerAngles = Vector3.zero;
            xtween_obj.transform.localScale = Vector3.one;
            xtween_obj.transform.SetSiblingIndex(BaseScript.transform.childCount - 2);
            #endregion

            IsInitialized.boolValue = true;
            IsInitialized.serializedObject.ApplyModifiedProperties();
        }
        /// <summary>
        /// 初始化布局辅助线的结构
        /// </summary>
        private void CompGuide_Structure_Create()
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
        private void CompGuide_Structure_Destroy()
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
        private void HiddenStructureDisplayer(bool state)
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
        /// <summary>
        /// 获取元素池的每项元素的标识名称
        /// </summary>
        private void GetElementPoolIndicators()
        {
            if (BaseScript.Hud_ElementLibrarys != null && BaseScript.Hud_ElementLibrarys.Count > 0)
            {
                for (int i = 0; i < BaseScript.Hud_ElementLibrarys.Count; i++)
                {
                    xHud_Library_Element lib = BaseScript.Hud_ElementLibrarys[i];
                    if (lib != null)
                    {
                        for (int s = 0; s < lib.ElementLibrary.Count; s++)
                        {
                            Library_Item item = lib.ElementLibrary[s];
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
        /// <summary>
        /// 切换分辨率
        /// </summary>
        /// <param name="indicator"></param>
        /// <param name="mode"></param>
        public void SwitchResolution(string indicator)
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
                EditorManagerUpdate();
            }
        }
        /// <summary>
        /// 确保图层存在
        /// </summary>
        /// <param name="layerName"></param>
        private void EnsureLayerExists(string layerName)
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
        /// 动效库添加器
        /// </summary>
        public void OpenParameterSetter(HudElementMotionType Type)
        {
            Editor_xHud_LibrarySetTool_Motion window = EditorWindow.GetWindow<Editor_xHud_LibrarySetTool_Motion>(true);

            window.titleContent = new GUIContent("XHud 动效库采集器");
            Editor_xHudGUI.CenterEditorWindow(new Vector2Int(600, 530), window);
            // 将要存入元素库的物体信息发送至窗口
            switch (Type)
            {
                case HudElementMotionType.Recycler:
                    Motion_Recycler rec = new Motion_Recycler();
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
            window.SetLibrarySetterMode(LibrarySetterMode.添加到库);
            window.SetButtonText("添加", "取消");
            window.SetTitle("XHud 动效库采集器");
            window.SetTarget_Hud_MotionLibrary(xHud_Dashboard.HudManagerGet().Hud_ElementMotion);
            //window.ShowModal();
            window.Show();
        }
        /// <summary>
        /// 构图参考解释窗口
        /// </summary>
        public void OpenGuideHelpPanel(string guidetype)
        {
            Editor_xHud_Tool_GuideReference window = EditorWindow.GetWindow<Editor_xHud_Tool_GuideReference>(true);

            window.titleContent = new GUIContent("XHud 构图参考说明书");
            Editor_xHudGUI.CenterEditorWindow(new Vector2Int(935, 660), window);
            window.GuideType = (guidetype);
            window.Show();
        }
        // 公共方法，用于对List<Texture2D>进行排序
        public static void SortTexturesByNameSuffix(List<Texture2D> textures)
        {
            // 使用Sort方法，并传入自定义的比较器
            textures.Sort((tex1, tex2) =>
            {
                // 获取贴图名称
                string name1 = tex1.name;
                string name2 = tex2.name;

                // 提取名称中的数字后缀
                int num1 = ExtractNumberFromName(name1);
                int num2 = ExtractNumberFromName(name2);

                // 按照数字后缀进行比较
                return num1.CompareTo(num2);
            });
        }
        // 辅助方法：从贴图名称中提取数字后缀
        private static int ExtractNumberFromName(string name)
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
        /// <summary>
        /// 诊断
        /// </summary>
        /// <returns></returns>
        private bool Diagnostic()
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

            if (Lib_ElementMotion.objectReferenceValue == null)
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
        public void RatioReference_Calculate()
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
        public void RatioReference_Create(Sprite spr)
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
            RatioReference_Update();
        }
        /// <summary>
        /// 比例参考标识物体 - 销毁
        /// </summary>
        public void RatioReference_Destroy()
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
        public void RatioReference_Switch()
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
        public void RatioReference_Update()
        {
            if (!UseRatioReference.boolValue)
                return;
            if (sp_Reference_Image.objectReferenceValue != null)
            {
                BaseScript.Reference_Image.color = sp_Reference_Image_Color.colorValue;
                BaseScript.Reference_Image.rectTransform.anchoredPosition3D = Vector3.zero;
                RatioReference_Calculate();
            }
        }
        #endregion

        #region 生成比例参考标记
        private void CreateReferenceShape(float width, float height, float tolerance_w = 0, float tolerance_h = 0)
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
            xHud_Module_Element obj_ele = obj.AddComponent<xHud_Module_Element>();
            obj_ele.Indicator = "RealWorldReferShape";
            UnityEngine.RectTransform obj_ele_rect = obj.AddComponent<UnityEngine.RectTransform>();
            obj_ele_rect.transform.SetParent(BaseScript.hm_Layout_GetAnchor(HudAnchor.顶层));
            obj_ele_rect.anchoredPosition3D = Vector3.zero;
            obj_ele_rect.localEulerAngles = Vector3.zero;
            obj_ele_rect.localScale = Vector3.one;

            // 创建 Image组件 对象
            GameObject image = new GameObject($"ReferShape");
            Image img = image.AddComponent<Image>();
            img.raycastTarget = false;
            img.maskable = false;
            img.sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{xHud_Dashboard.Get_GUIStyle_Path()}Icon/Icons_Hud_Manager/ReferShapeBG.png");
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

            Font font = AssetDatabase.LoadAssetAtPath<Font>($"{xHud_Dashboard.Get_XHudRoot_Path()}Fonts/Text/SevenBlack-Light.ttf");

            UnityEngine.RectTransform rect = textobj.AddComponent<UnityEngine.RectTransform>();
            rect.sizeDelta = new Vector2(120, 30);
            rect.anchorMin = Vector2.one * 0.5f;
            rect.anchorMax = Vector2.one * 0.5f;
            rect.pivot = Vector2.one * 0.5f;
            rect.anchoredPosition3D = new Vector3(0, rect.anchoredPosition3D.y + img.rectTransform.sizeDelta.y / 2 + 30, 0);

            xHud_Module_Text hud_text = textobj.AddComponent<xHud_Module_Text>();
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

        #region GameView分辨率操作
        public static Vector2 GetMainGameViewSize()
        {
            System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
            System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            System.Object Res = GetSizeOfMainGameView.Invoke(null, null);
            return (Vector2)Res;
        }
        private static void InitiaGameViewResolutions()
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
    }
}