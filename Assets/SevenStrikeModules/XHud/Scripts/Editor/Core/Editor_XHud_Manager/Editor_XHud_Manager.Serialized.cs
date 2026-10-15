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
        /// 获取序列化变量。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>通过 <c>serializedObject.FindProperty</c> 按名称查找。</description></item>
        /// </list>
        /// </summary>
        /// <param name="name">属性名。</param>
        /// <returns>对应的序列化属性。</returns>
        private SerializedProperty xHud_GetSerializedProperty(string name)
        {
            return serializedObject.FindProperty(name);
        }
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
    }
}