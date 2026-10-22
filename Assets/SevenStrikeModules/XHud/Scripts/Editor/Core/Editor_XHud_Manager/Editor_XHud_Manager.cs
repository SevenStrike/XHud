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
    using UnityEngine.UI;

    [CustomEditor(typeof(XHud_Manager))]
    public partial class Editor_XHud_Manager : Editor
    {
        /*
         * ============================================================================
         * Editor_XHud_Manager - Partial 文件组总览
         * ============================================================================
         * 本编辑器由 22 个 partial 文件共同构成，主文件为「调度中心 + 公共设施」，
         * 其余文件各自负责一块面板的绘制或一类工具方法。
         *
         * ----------------------------------------------------------------------------
         * 一、文件职责清单
         * ----------------------------------------------------------------------------
         *
         *   Editor_XHud_Manager.cs（主文件）
         *   ├─ 字段区：所有 partial 共享的字段全部集中于此
         *   ├─ 生命周期：OnEnable / OnDisable / OnDestroy / OnUndoRedoPerformed
         *   │            / OnInspectorGUI / OnSceneGUI
         *   ├─ OnInspectorGUI：保留 Logo、基础栏、分辨率信息栏、右键菜单，
         *   │                  其余面板通过 Draw_XXX() 调度
         *   ├─ 工具 - 批量化操作：Targets_Get / Targets_Selected
         *   ├─ 绘制控件：DrawToggle（供各面板复用的通用开关）
         *   ├─ 工具 - GameView 尺寸：反射读写 GameView 分辨率
         *   └─ 源脚本：内联 DrawDefaultInspector()
         *
         *   ┌──────────────────────────────────────────────────────────────────────┐
         *   │ 绘制面板（每个文件一个 Draw_XXX，由主文件 OnInspectorGUI 调度）        │
         *   ├──────────────────────────────────────────────────────────────────────┤
         *   │  Options.cs         → Draw_Options()          「选项」面板            │
         *   │  Canvas.cs          → Draw_Canvas()           「画布」面板            │
         *   │  Camera.cs          → Draw_Camera()           「相机」面板            │
         *   │  RatioReference.cs  → Draw_RatioReference()   「比例图例」面板        │
         *   │  CompGuide.cs       → Draw_CompGuide()        「构图参考」面板        │
         *   │  Libraries.cs       → Draw_Libraries()        「资源库」面板          │
         *   │  ElementLibs.cs     → Draw_ElementLibs()      「元素库」面板          │
         *   │  SoundsPool.cs      → Draw_SoundsPool()       「音效池」面板          │
         *   │  Mask.cs            → Draw_Mask()             「遮罩/内容透明度」面板  │
         *   │  Global.cs          → Draw_Global()           「全局」面板            │
         *   │  Tool.cs            → Draw_Tool()             「工具」面板            │
         *   │  Layout.cs          → Draw_Layout()           「布局」面板            │
         *   │  SafeFrame.cs       → Draw_SafeFrame()        「辅助」面板            │
         *   │  BluePrint.cs       → Draw_BluePrint()        「蓝图」面板            │
         *   │  RMS.cs             → Draw_RMS()              「RMS」面板             │
         *   │  MotionDefault.cs   → Draw_MotionDefault()    「动效默认参数」面板     │
         *   │  Theme.cs           → Draw_Theme()            「主题色」面板          │
         *   └──────────────────────────────────────────────────────────────────────┘
         *
         *   ┌──────────────────────────────────────────────────────────────────────┐
         *   │ 工具 / 支撑方法（不直接对应面板，由主文件或其它 partial 调用）          │
         *   ├──────────────────────────────────────────────────────────────────────┤
         *   │  RatioReference.cs  → xHud_RatioReference_Calculate/Create/Destroy    │
         *   │                       /Switch/Update + xHud_CreateReferenceShape      │
         *   │  CompGuide.cs       → xHud_CompGuide_Structure_Create/Destroy         │
         *   │                       + xHud_OpenGuideHelpPanel                       │
         *   │  ElementLibs.cs     → Init_ElementLibList() + List_ElementLib 回调     │
         *   │  SoundsPool.cs      → Init_SoundsPoolList() + List_SoundsPool 回调     │
         *   │  RMS.cs             → Init_RMSList() + List_RMS 回调                  │
         *   │                       + RefreshResolution + xHud_SwitchResolution     │
         *   │  Tool.cs            → xHud_StructureInitialize                        │
         *   │                       + xHud_EnsureLayerExists                        │
         *   │                       + xHud_SortTexturesByNameSuffix                 │
         *   │                       + xHud_ExtractNumberFromName                    │
         *   │  SafeFrame.cs       → xHud_HiddenStructureDisplayer                   │
         *   │  MotionDefault.cs   → ResetMotionParams + xHud_OpenParameterSetter    │
         *   │  Theme.cs           → xHud_LoadThemesColor / ResetThemesColor         │
         *   │                       / UpdateThemeColor(_Gp/_Sep)                    │
         *   │  Update.cs          → xHud_EditorUpdate_*                             │
         *   │                       + EditorApplication_EditorManagerUpdate         │
         *   │                       + xHud_GroupFolds + SetHighFrequencyMode        │
         *   │  Serialized.cs      → xHud_SerializedAllVariables                     │
         *   │                       + xHud_GetSerializedProperty                    │
         *   │  Diagnostic.cs      → xHud_Diagnostic                                 │
         *   │  ElementPool.cs     → xHud_GetElementPoolIndicators                   │
         *   └──────────────────────────────────────────────────────────────────────┘
         *
         * ----------------------------------------------------------------------------
         * 二、OnInspectorGUI 调用流向
         * ----------------------------------------------------------------------------
         *
         *   OnInspectorGUI（主文件）
         *      │
         *      ├─ serializedObject.Update()
         *      │
         *      ├─ Logo 背景 / 主体（主文件内联）
         *      ├─ 基础栏（主文件内联）
         *      │
         *      ├─ if (IsInitialized)
         *      │     ├─ 分辨率信息（主文件内联）
         *      │     │
         *      │     ├─ Draw_Options()          ← Options.cs
         *      │     ├─ Draw_Canvas()           ← Canvas.cs
         *      │     ├─ Draw_Camera()           ← Camera.cs
         *      │     ├─ Draw_RatioReference()   ← RatioReference.cs
         *      │     ├─ if (UseCompGuide)
         *      │     │     └─ Draw_CompGuide()  ← CompGuide.cs
         *      │     ├─ Draw_Libraries()        ← Libraries.cs
         *      │     ├─ Draw_ElementLibs()      ← ElementLibs.cs
         *      │     ├─ Draw_SoundsPool()       ← SoundsPool.cs
         *      │     ├─ Draw_Mask()             ← Mask.cs
         *      │     ├─ Draw_Global()           ← Global.cs
         *      │     ├─ Draw_Tool()             ← Tool.cs
         *      │     ├─ Draw_Layout()           ← Layout.cs
         *      │     ├─ if (UseSafeFrame)
         *      │     │     └─ Draw_SafeFrame()  ← SafeFrame.cs
         *      │     ├─ if (BluePrintMode)
         *      │     │     └─ Draw_BluePrint()  ← BluePrint.cs
         *      │     ├─ if (RMS_Enabled)
         *      │     │     └─ Draw_RMS()        ← RMS.cs
         *      │     ├─ Draw_MotionDefault()    ← MotionDefault.cs
         *      │     ├─ Draw_Theme()            ← Theme.cs
         *      │     └─ DrawDefaultInspector()  （主文件内联，源脚本）
         *      │
         *      └─ 右键菜单（主文件内联）
         *
         * ----------------------------------------------------------------------------
         * 三、OnEnable 调用流向
         * ----------------------------------------------------------------------------
         *
         *   OnEnable（主文件）
         *      │
         *      ├─ 缓存 BaseScript / LineHeight
         *      ├─ Targets_Get()                       （主文件：批量化操作）
         *      ├─ 获取图标（主文件内联）
         *      ├─ xHud_SerializedAllVariables()       ← Serialized.cs
         *      ├─ 获取 Mask 遮罩层（主文件内联）
         *      ├─ 寻找布局节点根物体（主文件内联）
         *      ├─ xHud_GetElementPoolIndicators()     ← ElementPool.cs
         *      ├─ Init_RMSList()                      ← RMS.cs
         *      ├─ Init_ElementLibList()               ← ElementLibs.cs
         *      ├─ Init_SoundsPoolList()               ← SoundsPool.cs
         *      ├─ 获取所有蓝图背景贴图（主文件内联）
         *      ├─ EditorApplication.hierarchyChanged 订阅（主文件）
         *      ├─ xHud_InitiaGameViewResolutions()    （主文件：GameView 尺寸）
         *      ├─ xHud_LoadThemesColor()              ← Theme.cs
         *      ├─ xHud_EnsureLayerExists("XHud")      ← Tool.cs
         *      ├─ xHud_EnsureLayerExists("XHud_World")← Tool.cs
         *      └─ xHud_RatioReference_Update()        ← RatioReference.cs
         *
         * ----------------------------------------------------------------------------
         * 四、关键约定
         * ----------------------------------------------------------------------------
         *   1. 字段与方法同文件；字段集中定义在主文件，各 partial 直接读写。
         *   2. 各 Partial 文件独立 using，只引自身需要的命名空间。
         *   3. 条件判断保留在主文件 OnInspectorGUI（UseCompGuide / BluePrintMode
         *      / RMS_Enabled / UseSafeFrame），各 Draw_XXX 只负责画。
         *   4. ReorderableList 回调统一具名化：<对象>_<动作>。
         *   5. 主文件不定义具体面板内容，仅做调度与公共设施。
         * ============================================================================
         */

        #region 字段 - 组件 / 列表
        /// <summary>
        /// 当前编辑的目标脚本实例。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>在 <c>OnEnable</c> 中由 <c>target</c> 强制转换获得。</description></item>
        /// </list>
        /// </summary>
        public XHud_Manager BaseScript;
        /// <summary>
        /// 可重排列表集合。
        /// <para/>
        /// <list type="bullet">
        /// <item><description><c>List_ElementLib</c>：元素库列表。</description></item>
        /// <item><description><c>List_RMS</c>：RMS 分辨率节点列表。</description></item>
        /// <item><description><c>List_SoundsPool</c>：音效池列表。</description></item>
        /// </list>
        /// </summary>
        private ReorderableList
            List_ElementLib,
            List_RMS,
            List_SoundsPool;
        #endregion

        #region 字段 - GUI 参数
        /// <summary>
        /// 单行行高缓存。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>用于列表元素与控件布局计算。</description></item>
        /// </list>
        /// </summary>
        private float LineHeight;
        /// <summary>
        /// GameView 分组方法反射引用。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>用于获取 GameView 尺寸。</description></item>
        /// </list>
        /// </summary>
        static MethodInfo getGroup;
        /// <summary>
        /// GameView 尺寸实例。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>用于获取 GameView 尺寸。</description></item>
        /// </list>
        /// </summary>
        static object gameViewSizesInstance;
        #endregion

        #region 字段 - 频率更新
        /// <summary>
        /// 上次场景刷新时间。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>用于自动降频判断。</description></item>
        /// </list>
        /// </summary>
        private double _lastSceneUpdate;
        /// <summary>
        /// 上次层级刷新时间。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>用于自动降频判断。</description></item>
        /// </list>
        /// </summary>
        private double _lastHierarchyUpdate;
        /// <summary>
        /// 上次用户操作时间。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>用于自动降频判断。</description></item>
        /// </list>
        /// </summary>
        private double _lastUserActionTime;
        /// <summary>
        /// 自动降频阈值。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>默认 0.6 秒。</description></item>
        /// </list>
        /// </summary>
        private double UPDATE_KEEPTIME = 0.6f;
        /// <summary>
        /// 每秒最低刷新次数。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>默认 1 次/秒。</description></item>
        /// </list>
        /// </summary>
        private double UPDATE_INTERVAL = 1;
        /// <summary>
        /// 低频刷新间隔。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>默认 0.033 秒。</description></item>
        /// </list>
        /// </summary>
        private double UPDATE_INTERVAL_Low = 0.033f;
        /// <summary>
        /// 高频刷新间隔。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>默认 0.005 秒。</description></item>
        /// </list>
        /// </summary>
        private double UPDATE_INTERVAL_High = 0.005f;
        #endregion

        #region 字段 - 键
        /// <summary>
        /// 元素可视化放置器菜单键。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>用于 EditorPrefs 存储。</description></item>
        /// </list>
        /// </summary>
        private const string MENU_KEY_VISUAL_PLACER = "XHud_ElementVisualPlacer_Enabled";
        #endregion

        #region 字段 - 序列化属性
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
            theme_color_primary,
            theme_color_group,
            theme_color_sepline,
            theme_solution_primary,
            theme_solution_group,
            theme_solution_sepline,
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

        #region 字段 - 图标
        /// <summary>
        /// 本编辑器所使用的图标资源集合。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>状态与 RMS 图标。</description></item>
        /// <item><description>面板分类图标（选项/画布/相机/库/元素库/安全框/辅助/蓝图/匹配分辨率/音频/掩码/全局/工具/主题/本地化/物理比例）。</description></item>
        /// <item><description>按钮常态/按下图标（保存/定位/重置/初始化/创建库/安全框可视化/状态检查）。</description></item>
        /// <item><description>射线图标、组件引导图标、Logo 与主题标记。</description></item>
        /// </list>
        /// </summary>
        public Texture2D status, rmsicon, icon_MC_Use, icon_MC_None, soundplaying, panel_option, panel_canvas, panel_camera, panel_libs, panel_elementlibs, panel_frame, panel_assist, panel_bulueprint, panel_matchres, panel_audios, panel_mask, panel_global, panel_tools, panel_eleparamrecycle_auto, panel_eleparammotion_default, panel_theme, panel_localized, panel_physicsratio, save_r, save_p, locate_r, locate_p, reset_r, reset_p, icon_initia_r, icon_initia_p, icon_libs_create_r, icon_libs_create_p, icon_safeframevisual_r, icon_safeframevisual_p, icon_checkstate_r, icon_checkstate_p, icon_ray_normal, icon_ray_masked, panel_comp_guide, GuideHelp_Released, GuideHelp_Press, logo_bg, logo, themes_mark;
        #endregion

        #region 字段 - 开关
        /// <summary>
        /// 默认动效参数折叠状态。
        /// <para/>
        /// <list type="bullet">
        /// <item><description><c>def_recycle_fold_move / def_recycle_fold_rotate / def_recycle_fold_alpha</c>：回收参数折叠状态。</description></item>
        /// <item><description><c>def_create_fold_move / def_create_fold_rotate / def_create_fold_alpha</c>：生成参数折叠状态。</description></item>
        /// </list>
        /// </summary>
        private bool def_recycle_fold_move, def_recycle_fold_rotate, def_recycle_fold_alpha, def_create_fold_move, def_create_fold_rotate, def_create_fold_alpha;
        #endregion

        #region 字段 - 蓝图
        /// <summary>
        /// 蓝图背景贴图列表。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>在 <c>OnEnable</c> 中扫描蓝图背景目录填充。</description></item>
        /// </list>
        /// </summary>
        List<Texture2D> BluePrintBgs = new List<Texture2D>();
        /// <summary>
        /// 蓝图背景贴图名称数组。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>与 <c>BluePrintBgs</c> 一一对应。</description></item>
        /// </list>
        /// </summary>
        string[] BluePrintBgs_Name;
        #endregion

        #region 字段 - 选项文字
        /// <summary>
        /// 启用开关选项文字。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>索引 0：关闭；索引 1：开启。</description></item>
        /// </list>
        /// </summary>
        string[] stroptions_enabled = new string[2] { "关闭", "开启" };
        /// <summary>
        /// 画布尺寸模式选项文字。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>索引 0：固定像素尺寸；索引 1：屏幕尺寸；索引 2：固定物理尺寸。</description></item>
        /// </list>
        /// </summary>
        string[] stroptions_canvassize = new string[3] { "固定像素尺寸", "屏幕尺寸", "固定物理尺寸" };
        /// <summary>
        /// 画布锚点模式选项文字。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>由 <c>CanvasCameraAttachment</c> 枚举名生成。</description></item>
        /// </list>
        /// </summary>
        string[] stroptions_canvasanchor = System.Enum.GetNames(typeof(CanvasCameraAttachment));
        /// <summary>
        /// 组件引导模式选项文字。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>水平对称、垂直对称、黄金螺旋、对角线、三分线、引导线、三角、工字型。</description></item>
        /// </list>
        /// </summary>
        string[] compguid_modes = new string[8] { "水平对称", "垂直对称", "黄金螺旋", "对角线", "三分线", "引导线", "三角", "工字型" };
        /// <summary>
        /// 主题主色调方案名称
        /// </summary>
        string[] theme_primary_presetnames;
        /// <summary>
        /// 主题编组配色方案名称
        /// </summary>
        string[] theme_group_presetnames;
        /// <summary>
        /// 主题分割线配色方案名称
        /// </summary>
        string[] theme_sepline_presetnames;
        #endregion

        #region 字段 - 批量化操作
        /// <summary>
        /// 批量选中的管理器对象数组。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>在 <c>Targets_Get</c> 中依据 <c>targets</c> 填充。</description></item>
        /// <item><description>由 <c>Targets_Selected</c> 判断是否为多选状态。</description></item>
        /// </list>
        /// </summary>
        XHud_Manager[] SelectedObjects;
        #endregion

        #region 字段 - 颜色
        /// <summary>
        /// 重要提示颜色。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>用于强调文本或警告状态。</description></item>
        /// </list>
        /// </summary>
        private Color color_important = new Color(1, 0.4f, 0.4f);
        #endregion

        #region 生命周期
        private void Awake()
        {
            #region 获取预制主题色名称列表
            theme_primary_presetnames = XHud_Dashboard.themes_get_presetnames_primary();
            theme_group_presetnames = XHud_Dashboard.themes_get_presetnames_group();
            theme_sepline_presetnames = XHud_Dashboard.themes_get_presetnames_sepline();
            #endregion
        }
        /// <summary>
        /// 编辑器启用时初始化。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>订阅撤销重做事件。</description></item>
        /// <item><description>缓存目标脚本与行高。</description></item>
        /// <item><description>加载全部图标资源。</description></item>
        /// <item><description>序列化全部属性并初始化各子模块。</description></item>
        /// <item><description>构建 ReorderableList、蓝图背景、GameView 分辨率与主题色。</description></item>
        /// </list>
        /// </summary>
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

            Init_RMSList();

            Init_ElementLibList();

            Init_SoundsPoolList();

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

            // 创建图层 - XHud
            xHud_EnsureLayerExists("XHud");

            // 创建图层 - XHud_World
            xHud_EnsureLayerExists("XHud_World");

            //设置XHud管理器图层为XHud
            BaseScript.gameObject.layer = LayerMask.NameToLayer("XHud");

            xHud_RatioReference_Update();
        }
        /// <summary>
        /// 编辑器禁用时清理。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>取消撤销重做事件订阅。</description></item>
        /// <item><description>将蓝图根节点置于首位。</description></item>
        /// <item><description>按需折叠所有面板。</description></item>
        /// </list>
        /// </summary>
        private void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoRedoPerformed;

            BaseScript.hm_BluePrintRootFirstSibling();

            if (FoldAllPanelWithDisabled.boolValue)
                xHud_GroupFolds(false);
        }
        /// <summary>
        /// 编辑器销毁时清理。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>取消层级变更事件订阅。</description></item>
        /// </list>
        /// </summary>
        private void OnDestroy()
        {
            EditorApplication.hierarchyChanged -= EditorApplication_EditorManagerUpdate;
        }
        /// <summary>
        /// 撤销重做回调。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>重新同步序列化属性。</description></item>
        /// <item><description>刷新辅助视觉、画布相机、掩码、内容透明度与相机参数。</description></item>
        /// <item><description>触发 Inspector 重绘。</description></item>
        /// </list>
        /// </summary>
        private void OnUndoRedoPerformed()
        {
            // 重新同步序列化属性
            serializedObject.Update();

            xHud_EditorUpdate_HelperVisual();
            xHud_EditorUpdate_Canvas_Camera();
            xHud_EditorUpdate_Mask();
            xHud_EditorUpdate_ContentOpacity();
            xHud_EditorUpdate_CameraArgs();

            // 撤销或前进步骤时更新主题色
            xHud_Themes_ColorUpdateAll();

            // 重绘 Inspector
            Repaint();
        }
        /// <summary>
        /// 绘制 Inspector 主界面。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>绘制 Logo 背景与主体。</description></item>
        /// <item><description>绘制基础栏、分辨率信息栏。</description></item>
        /// <item><description>调度各功能面板的 <c>Draw_XXX</c> 绘制方法。</description></item>
        /// <item><description>处理右键菜单与序列化应用。</description></item>
        /// </list>
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            string hexcol = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.themes_get_color(ThemeColorType.primary), true);

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
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "基础",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
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
                    bg_fill: XGUIGroup.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                    title: "分辨率",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(20, 20, 15, 15));

                string resinfo = string.Format("{0} x {1}", ScreenRes.vector2Value.x, ScreenRes.vector2Value.y);
                XGUI.layout_label(
                    text: resinfo,
                    size: XGUIFontSize.M,
                    text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
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
                    bg_fill: XGUIGroup.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                    title: "选项",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: BaseScript.fold_options);

                if (!BaseScript.fold_options)
                {
                    Draw_Options();
                }

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                #endregion

                #region 画布 
                BaseScript.fold_canvasset = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIGroup.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                    title: "画布",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: BaseScript.fold_canvasset);

                if (!BaseScript.fold_canvasset)
                {
                    Draw_Canvas();
                }

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                #endregion

                #region 相机 
                BaseScript.fold_camera = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIGroup.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                    title: "相机",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: BaseScript.fold_camera);

                if (!BaseScript.fold_camera)
                {
                    Draw_Camera();
                }

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                #endregion

                #region 比例图例 
                BaseScript.fold_ratiorefer = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIGroup.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                    title: "比例图例 (单位 cm)",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: BaseScript.fold_ratiorefer);

                if (!BaseScript.fold_ratiorefer)
                {
                    Draw_RatioReference();
                }

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                #endregion

                #region 构图参考 
                if (UseCompGuide.boolValue)
                {
                    BaseScript.fold_compguid = XGUI.layout_group_start(
                        type: XGUIContainerType.Vertical,
                        bg_fill: XGUIGroup.缺口纯色边框,
                        bg_color: XGUIColor.亮白,
                        bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                        title: "构图参考",
                        title_size: XGUIFontSize.M,
                        title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_clipping: TextClipping.Clip,
                        padding: new RectOffset(10, 10, 15, 15),
                        foldout: BaseScript.fold_compguid);

                    if (!BaseScript.fold_compguid)
                    {
                        Draw_CompGuide();
                    }
                    XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                }
                #endregion

                #region 资源库 
                BaseScript.fold_reslibs = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIGroup.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                    title: "资源库",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: BaseScript.fold_reslibs);

                if (!BaseScript.fold_reslibs)
                {
                    Draw_Libraries();
                }

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                #endregion

                #region 元素库 
                BaseScript.fold_elementlibs = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIGroup.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                    title: "元素库",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: BaseScript.fold_elementlibs);

                if (!BaseScript.fold_elementlibs)
                {
                    Draw_ElementLibs();
                }

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                #endregion

                #region 音效池 
                BaseScript.fold_soundslib = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIGroup.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                    title: "音效池",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: BaseScript.fold_soundslib);

                if (!BaseScript.fold_soundslib)
                {
                    Draw_SoundsPool();
                }

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                #endregion

                #region 遮罩 / 内容透明度 
                BaseScript.fold_masks = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIGroup.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                    title: "遮罩 / 内容透明度",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: BaseScript.fold_masks);

                if (!BaseScript.fold_masks)
                {
                    Draw_Mask();
                }

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                #endregion

                #region 全局
                BaseScript.fold_global = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIGroup.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                    title: "全局",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: BaseScript.fold_global);

                if (!BaseScript.fold_global)
                {
                    Draw_Global();
                }

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                #endregion

                #region 工具
                BaseScript.fold_tool = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIGroup.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                    title: "工具",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: BaseScript.fold_tool);

                if (!BaseScript.fold_tool)
                {
                    Draw_Tool();
                }

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                #endregion

                #region 布局
                BaseScript.fold_layout = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIGroup.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                    title: "布局",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: BaseScript.fold_layout);

                if (!BaseScript.fold_layout)
                {
                    Draw_Layout();
                }

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                #endregion

                #region 辅助
                if (UseSafeFrame.boolValue)
                {
                    BaseScript.fold_visualsafe = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIGroup.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                    title: "辅助",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: BaseScript.fold_visualsafe);

                    if (!BaseScript.fold_visualsafe)
                    {
                        Draw_SafeFrame();
                    }

                    XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                }
                #endregion

                #region 蓝图
                if (BluePrintMode.boolValue)
                {
                    BaseScript.fold_blueprint = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIGroup.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                    title: "蓝图",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: BaseScript.fold_blueprint);

                    if (!BaseScript.fold_blueprint)
                    {
                        Draw_BluePrint();
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
                        bg_fill: XGUIGroup.缺口纯色边框,
                        bg_color: XGUIColor.亮白,
                        bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                        title: "RMS",
                        title_size: XGUIFontSize.M,
                        title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_clipping: TextClipping.Clip,
                        padding: new RectOffset(10, 10, 15, 15),
                        foldout: BaseScript.fold_RMS);

                    if (!BaseScript.fold_RMS)
                    {
                        Draw_RMS();
                    }

                    XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                }
                #endregion

                #region 动效默认参数
                BaseScript.fold_motion_default = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIGroup.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                    title: "动效默认参数",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: BaseScript.fold_motion_default);

                if (!BaseScript.fold_motion_default)
                {
                    Draw_MotionDefault();
                }

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                #endregion

                #region 主题色
                BaseScript.fold_themes = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIGroup.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                    title: "主题色",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: BaseScript.fold_themes);

                if (!BaseScript.fold_themes)
                {
                    Draw_Theme();
                }

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                #endregion

                #region 源脚本
                BaseScript.fold_based = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIGroup.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                    title: "源脚本",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
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
        /// <summary>
        /// 绘制场景视图回调。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>按刷新间隔降频执行辅助视觉与画布相机刷新。</description></item>
        /// </list>
        /// </summary>
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
        #endregion

        #region 工具 - 批量化操作
        /// <summary>
        /// 获取所有选中的目标对象。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>多选时逐个填充 <c>SelectedObjects</c>。</description></item>
        /// <item><description>单选时仅填充索引 0。</description></item>
        /// </list>
        /// </summary>
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
        /// <summary>
        /// 判断是否为多选状态。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>选中对象数量大于 1 时返回 <c>true</c>。</description></item>
        /// </list>
        /// </summary>
        /// <returns>多选返回 <c>true</c>，否则返回 <c>false</c>。</returns>
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

        #region 绘制控件
        /// <summary>
        /// 通用方法：绘制开关。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>封装 <c>XGUI.layout_toggle</c> 的常用参数。</description></item>
        /// <item><description>支持自定义开/关背景色、手柄色与变更回调。</description></item>
        /// </list>
        /// </summary>
        /// <param name="title">开关标题。</param>
        /// <param name="prop">绑定的序列化属性。</param>
        /// <param name="width">标题宽度。</param>
        /// <param name="style">开关样式。</param>
        /// <param name="color_bg_on">开启时背景色。</param>
        /// <param name="color_bg_off">关闭时背景色。</param>
        /// <param name="color_on">开启时手柄色。</param>
        /// <param name="color_off">关闭时手柄色。</param>
        /// <param name="act_on_changed">值变更回调。</param>
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
                tog_mixed_bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                act_on_changed: act_on_changed);
        }
        #endregion

        #region 工具 - GameView 尺寸
        /// <summary>
        /// 获取主 GameView 的当前尺寸。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>通过反射调用 Unity 内部 <c>GetSizeOfMainGameView</c>。</description></item>
        /// </list>
        /// </summary>
        /// <returns>主 GameView 尺寸。</returns>
        public static Vector2 xHud_GetMainGameViewSize()
        {
            System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
            System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            System.Object Res = GetSizeOfMainGameView.Invoke(null, null);
            return (Vector2)Res;
        }
        /// <summary>
        /// 初始化 GameView 分辨率反射引用。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>缓存 <c>GameViewSizes</c> 单例与 <c>GetGroup</c> 方法。</description></item>
        /// </list>
        /// </summary>
        private static void xHud_InitiaGameViewResolutions()
        {
            var sizesType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizes");
            var singleType = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
            var instanceProp = singleType.GetProperty("instance");
            getGroup = sizesType.GetMethod("GetGroup");
            gameViewSizesInstance = instanceProp.GetValue(null, null);
        }
        /// <summary>
        /// 获取指定分组的 GameView 尺寸对象。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>通过反射调用缓存的 <c>GetGroup</c>。</description></item>
        /// </list>
        /// </summary>
        /// <param name="type">尺寸分组类型。</param>
        /// <returns>分组对象。</returns>
        static object GetGroup(GameViewSizeGroupType type)
        {
            return getGroup.Invoke(gameViewSizesInstance, new object[] { (int)type });
        }
        /// <summary>
        /// 按显示文本查找尺寸索引。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>遍历显示文本并剥离括号后缀后比较。</description></item>
        /// </list>
        /// </summary>
        /// <param name="sizeGroupType">尺寸分组类型。</param>
        /// <param name="text">显示文本。</param>
        /// <returns>匹配的索引，未找到返回 -1。</returns>
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
        /// <summary>
        /// 按宽高查找尺寸索引。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>遍历内置与自定义尺寸并比较宽高。</description></item>
        /// </list>
        /// </summary>
        /// <param name="sizeGroupType">尺寸分组类型。</param>
        /// <param name="width">宽度。</param>
        /// <param name="height">高度。</param>
        /// <returns>匹配的索引，未找到返回 -1。</returns>
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
        /// <summary>
        /// 判断指定宽高尺寸是否存在。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>基于 <c>FindSize</c> 判断。</description></item>
        /// </list>
        /// </summary>
        /// <param name="sizeGroupType">尺寸分组类型。</param>
        /// <param name="width">宽度。</param>
        /// <param name="height">高度。</param>
        /// <returns>存在返回 <c>true</c>。</returns>
        public static bool SizeExists(GameViewSizeGroupType sizeGroupType, int width, int height)
        {
            return FindSize(sizeGroupType, width, height) != -1;
        }
        /// <summary>
        /// 判断指定文本尺寸是否存在。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>基于 <c>FindSize</c> 判断。</description></item>
        /// </list>
        /// </summary>
        /// <param name="sizeGroupType">尺寸分组类型。</param>
        /// <param name="text">显示文本。</param>
        /// <returns>存在返回 <c>true</c>。</returns>
        public static bool SizeExists(GameViewSizeGroupType sizeGroupType, string text)
        {
            return FindSize(sizeGroupType, text) != -1;
        }
        /// <summary>
        /// 添加自定义尺寸。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>通过反射构造 <c>GameViewSize</c> 并调用 <c>AddCustomSize</c>。</description></item>
        /// </list>
        /// </summary>
        /// <param name="viewSizeType">尺寸类型。</param>
        /// <param name="sizeGroupType">尺寸分组类型。</param>
        /// <param name="width">宽度。</param>
        /// <param name="height">高度。</param>
        /// <param name="text">显示文本。</param>
        public static void AddCustomSize(GameViewSizeType viewSizeType, GameViewSizeGroupType sizeGroupType, int width, int height, string text)
        {
            var group = GetGroup(sizeGroupType);
            var addCustomSize = getGroup.ReturnType.GetMethod("AddCustomSize");
            var gvsType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSize");
            var ctor = gvsType.GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int), typeof(string) });
            var newSize = ctor.Invoke(new object[] { (int)viewSizeType, width, height, text });
            addCustomSize.Invoke(group, new object[] { newSize });
        }
        /// <summary>
        /// 设置当前 GameView 的尺寸索引。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>通过反射设置 <c>selectedSizeIndex</c>。</description></item>
        /// </list>
        /// </summary>
        /// <param name="index">尺寸索引。</param>
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