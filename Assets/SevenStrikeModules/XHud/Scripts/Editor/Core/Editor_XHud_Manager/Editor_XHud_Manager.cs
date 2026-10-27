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
         * 本编辑器由 22 个文件共同构成（主文件 + 21 个 partial 文件），主文件为
         * 「调度中心 + 公共设施」，其余文件各自负责一块面板的绘制或一类工具方法。
         *
         * ----------------------------------------------------------------------------
         * 一、文件职责清单
         * ----------------------------------------------------------------------------
         *
         *   Editor_XHud_Manager.cs（主文件）
         *   ├─ 字段区：所有 partial 共享的字段全部集中于此
         *   ├─ 生命周期：Awake / OnEnable / OnDisable / OnDestroy
         *   │            / OnUndoRedoPerformed / OnInspectorGUI / OnSceneGUI
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
         *   │  Motion.cs          → Draw_MotionDefault()    「动效默认参数」面板     │
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
         *   │  Motion.cs          → motion_save / motion_edit                       │
         *   │  Theme.cs           → xHud_ResetThemesColor / xHud_Themes_ColorUpdate │
         *   │                       / xHud_Themes_ColorUpdateAll                    │
         *   │                       / xHud_Themes_SliderValueSet                    │
         *   │                       / xHud_Themes_SolutionReset                     │
         *   │                       / xHud_ThemeControl                             │
         *   │  Update.cs          → xHud_EditorUpdate_*                             │
         *   │                       + EditorApplication_EditorManagerUpdate         │
         *   │                       + xHud_GroupFolds + SetHighFrequencyMode        │
         *   │  Serialized.cs      → xHud_SerializedAllVariables                     │
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
         *      ├─ if (initialized.boolValue)
         *      │     ├─ 分辨率信息（主文件内联）
         *      │     │
         *      │     ├─ Draw_Options()          ← Options.cs
         *      │     ├─ Draw_Canvas()           ← Canvas.cs
         *      │     ├─ Draw_Camera()           ← Camera.cs
         *      │     ├─ Draw_RatioReference()   ← RatioReference.cs
         *      │     ├─ if (sp_cg_use.boolValue)
         *      │     │     └─ Draw_CompGuide()  ← CompGuide.cs
         *      │     ├─ Draw_Libraries()        ← Libraries.cs
         *      │     ├─ Draw_ElementLibs()      ← ElementLibs.cs
         *      │     ├─ Draw_SoundsPool()       ← SoundsPool.cs
         *      │     ├─ Draw_Mask()             ← Mask.cs
         *      │     ├─ Draw_Global()           ← Global.cs
         *      │     ├─ Draw_Tool()             ← Tool.cs
         *      │     ├─ Draw_Layout()           ← Layout.cs
         *      │     ├─ if (sp_sf_use.boolValue)
         *      │     │     └─ Draw_SafeFrame()  ← SafeFrame.cs
         *      │     ├─ if (sp_bp_mode.boolValue)
         *      │     │     └─ Draw_BluePrint()  ← BluePrint.cs
         *      │     ├─ if (sp_rms_enabled.boolValue)
         *      │     │     └─ Draw_RMS()        ← RMS.cs
         *      │     ├─ Draw_MotionDefault()    ← Motion.cs
         *      │     ├─ Draw_Theme()            ← Theme.cs
         *      │     └─ DrawDefaultInspector()  （主文件内联，源脚本）
         *      │
         *      └─ 右键菜单（主文件内联）
         *
         * ----------------------------------------------------------------------------
         * 三、OnEnable 调用流向
         * ----------------------------------------------------------------------------
         *
         *   Awake（主文件）
         *      │
         *      └─ 获取预制主题色名称列表（XHud_Dashboard.themes_get_presetnames_*）
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
         *      ├─ base_script.hm_BluePrintRootFirstSibling()
         *      ├─ EditorApplication.hierarchyChanged 订阅（主文件）
         *      ├─ xHud_InitiaGameViewResolutions()    （主文件：GameView 尺寸）
         *      ├─ xHud_EnsureLayerExists("XHud")      ← Tool.cs
         *      ├─ xHud_EnsureLayerExists("XHud_World")← Tool.cs
         *      └─ xHud_RatioReference_Update()        ← RatioReference.cs
         *
         * ----------------------------------------------------------------------------
         * 四、关键约定
         * ----------------------------------------------------------------------------
         *   1. 字段集中定义在主文件，各 partial 直接读写；方法按职责归属各自文件。
         *   2. 各 Partial 文件独立 using，只引自身需要的命名空间。
         *   3. 条件判断保留在主文件 OnInspectorGUI（sp_cg_use / sp_bp_mode /
         *      sp_rms_enabled / sp_sf_use），各 Draw_XXX 只负责画。
         *   4. ReorderableList 回调统一具名化：<对象>_<动作>。
         *   5. 主文件不定义具体面板内容，仅做调度与公共设施。
         *   6. Motion 面板文件名为 Motion.cs，绘制方法名为 Draw_MotionDefault()。
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
        public XHud_Manager base_script;
        /// <summary>
        /// 可重排列表集合。
        /// <para/>
        /// <list type="bullet">
        /// <item><description><c>list_element_librarys</c>：元素库列表。</description></item>
        /// <item><description><c>list_rms</c>：RMS 分辨率节点列表。</description></item>
        /// <item><description><c>list_soundpools</c>：音效池列表。</description></item>
        /// </list>
        /// </summary>
        private ReorderableList
            list_element_librarys,
            list_rms,
            list_soundpools;
        #endregion

        #region 字段 - GUI 参数
        /// <summary>
        /// 单行行高缓存。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>用于列表元素与控件布局计算。</description></item>
        /// </list>
        /// </summary>
        private float line_height;
        /// <summary>
        /// GameView 分组方法反射引用。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>用于获取 GameView 尺寸。</description></item>
        /// </list>
        /// </summary>
        static MethodInfo get_group;
        /// <summary>
        /// GameView 尺寸实例。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>用于获取 GameView 尺寸。</description></item>
        /// </list>
        /// </summary>
        static object game_view_sizes_instance;
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
        private double update_keeptime = 0.6f;
        /// <summary>
        /// 每秒最低刷新次数。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>默认 1 次/秒。</description></item>
        /// </list>
        /// </summary>
        private double update_interval = 1;
        /// <summary>
        /// 低频刷新间隔。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>默认 0.033 秒。</description></item>
        /// </list>
        /// </summary>
        private double update_interval_low = 0.033f;
        /// <summary>
        /// 高频刷新间隔。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>默认 0.005 秒。</description></item>
        /// </list>
        /// </summary>
        private double update_interval_high = 0.005f;
        #endregion

        #region 字段 - 序列化属性
        #region 初始化与库引用
        /// <summary>
        /// 是否已完成初始化
        /// </summary>
        SerializedProperty initialized;
        /// <summary>
        /// 元素库中的元素列表
        /// </summary>
        SerializedProperty sp_ele_library_elements;
        /// <summary>
        /// 音效对象池
        /// </summary>
        SerializedProperty sp_pool_sounder;
        /// <summary>
        /// 音效对象池数量
        /// </summary>
        SerializedProperty sp_pool_sounder_count;
        /// <summary>
        /// 颜色库
        /// </summary>
        SerializedProperty sp_library_colors;
        /// <summary>
        /// 曲线库
        /// </summary>
        SerializedProperty sp_library_curves;
        /// <summary>
        /// 音效库
        /// </summary>
        SerializedProperty sp_library_sounds;
        /// <summary>
        /// 文本样式库
        /// </summary>
        SerializedProperty sp_library_textstyles;
        /// <summary>
        /// 动效库
        /// </summary>
        SerializedProperty sp_library_motions;
        /// <summary>
        /// 转场库
        /// </summary>
        SerializedProperty sp_library_transitions;
        #endregion

        #region 画布与相机
        /// <summary>
        /// UI 相机
        /// </summary>
        SerializedProperty sp_camera_ui;
        /// <summary>
        /// 场景相机
        /// </summary>
        SerializedProperty sp_camera_scene;
        /// <summary>
        /// 屏幕 HUD 画布
        /// </summary>
        SerializedProperty sp_cv_hudcanvas_screen_anchor;
        /// <summary>
        /// 世界 HUD 画布
        /// </summary>
        SerializedProperty sp_cv_hudcanvas_world;
        /// <summary>
        /// 世界 HUD 画布锚点
        /// </summary>
        SerializedProperty sp_cv_hudcanvas_world_anchor;
        /// <summary>
        /// HUD 画布锚点
        /// </summary>
        SerializedProperty sp_cv_hudcanvas_anchor;
        /// <summary>
        /// HUD 画布锚点索引
        /// </summary>
        SerializedProperty sp_cv_hudcanvas_anchor_index;
        /// <summary>
        /// 画布缩放器
        /// </summary>
        SerializedProperty sp_cv_scaler;
        /// <summary>
        /// 画布缩放模式索引
        /// </summary>
        SerializedProperty sp_cv_scaler_mode_index;
        /// <summary>
        /// 画布参考分辨率
        /// </summary>
        SerializedProperty sp_cv_screen;
        /// <summary>
        /// 画布匹配方向
        /// </summary>
        SerializedProperty sp_cv_match_dir;
        /// <summary>
        /// 画布距离
        /// </summary>
        SerializedProperty sp_cv_distance;
        /// <summary>
        /// 画布屏幕分辨率
        /// </summary>
        SerializedProperty sp_cv_screen_res;
        /// <summary>
        /// 画布边距
        /// </summary>
        SerializedProperty sp_cv_margins;
        /// <summary>
        /// 画布水平边距
        /// </summary>
        SerializedProperty sp_cv_margin_horizontal;
        /// <summary>
        /// 画布垂直边距
        /// </summary>
        SerializedProperty sp_cv_margin_vertical;
        /// <summary>
        /// 画布边距倍数
        /// </summary>
        SerializedProperty sp_cv_margin_multiply;
        /// <summary>
        /// 画布锚点布局屏幕
        /// </summary>
        SerializedProperty sp_cv_anchors_layout_screen;
        /// <summary>
        /// 是否支持世界 UI
        /// </summary>
        SerializedProperty sp_cv_support_world_ui;
        /// <summary>
        /// 是否使用自动完美像素
        /// </summary>
        SerializedProperty sp_cv_use_auto_perfect_pixel;
        /// <summary>
        /// 是否每帧更新完美像素
        /// </summary>
        SerializedProperty sp_cv_use_perfect_pixel_update;
        /// <summary>
        /// UI 相机正交尺寸
        /// </summary>
        SerializedProperty sp_camera_ui_orthographic_size;
        /// <summary>
        /// UI 相机视野
        /// </summary>
        SerializedProperty sp_camera_ui_fov;
        /// <summary>
        /// UI 相机近裁剪面
        /// </summary>
        SerializedProperty sp_camera_ui_cutter_near;
        /// <summary>
        /// UI 相机远裁剪面
        /// </summary>
        SerializedProperty sp_camera_ui_cutter_far;
        /// <summary>
        /// 相机正交模式
        /// </summary>
        SerializedProperty sp_camera_othograpic_mode;
        #endregion

        #region 屏幕适配与安全框
        /// <summary>
        /// 字体大小倍数
        /// </summary>
        SerializedProperty sp_ft_font_size_multiply;
        /// <summary>
        /// 画布缩放器屏幕尺寸
        /// </summary>
        SerializedProperty sp_sf_canvas_scaler_screen_size;
        /// <summary>
        /// 安全框是否启用
        /// </summary>
        SerializedProperty sp_sf_use;
        /// <summary>
        /// 安全框结构显示器
        /// </summary>
        SerializedProperty sp_sf_structure_displayer;
        /// <summary>
        /// 安全框
        /// </summary>
        SerializedProperty sp_sf_safeframe;
        /// <summary>
        /// 安全框标记尺寸
        /// </summary>
        SerializedProperty sp_sf_mark_size;
        /// <summary>
        /// 安全框布局锚点标记颜色
        /// </summary>
        SerializedProperty sp_sf_color_layout_anchor_mark;
        /// <summary>
        /// 安全框边框线颜色
        /// </summary>
        SerializedProperty sp_sf_color_frame_line;
        /// <summary>
        /// 安全框线宽
        /// </summary>
        SerializedProperty sp_sf_safe_frame_line_width;
        /// <summary>
        /// 安全框线边距
        /// </summary>
        SerializedProperty sp_sf_line_margins;
        /// <summary>
        /// 分隔线颜色
        /// </summary>
        SerializedProperty sp_sf_color_seperater_line;
        /// <summary>
        /// 分隔线长度
        /// </summary>
        SerializedProperty sp_sf_seperater_length;
        /// <summary>
        /// 中心标记长度
        /// </summary>
        SerializedProperty sp_sf_center_mark_length;
        /// <summary>
        /// 中心标记宽度
        /// </summary>
        SerializedProperty sp_sf_center_mark_width;
        /// <summary>
        /// 中心标记颜色
        /// </summary>
        SerializedProperty sp_sf_color_center_mark;
        /// <summary>
        /// 中心标记间距
        /// </summary>
        SerializedProperty sp_sf_center_mark_distance;
        #endregion

        #region 掩码与不透明度
        /// <summary>
        /// 掩码
        /// </summary>
        SerializedProperty sp_mk_mask;
        /// <summary>
        /// 掩码透明度
        /// </summary>
        SerializedProperty sp_mk_alpha;
        /// <summary>
        /// 掩码贴图
        /// </summary>
        SerializedProperty sp_mk_texture;
        /// <summary>
        /// 掩码射线检测透明度阈值
        /// </summary>
        SerializedProperty sp_mk_raycast_alpha_threshold;
        /// <summary>
        /// 掩码射线检测是否启用
        /// </summary>
        SerializedProperty sp_mk_raycast_enabled;
        /// <summary>
        /// 掩码颜色
        /// </summary>
        SerializedProperty sp_mk_color;
        /// <summary>
        /// 屏幕 HUD 画布组不透明度
        /// </summary>
        SerializedProperty sp_op_hud_canvas_group_screen;
        /// <summary>
        /// 世界 HUD 画布组不透明度
        /// </summary>
        SerializedProperty sp_op_hud_canvas_group_world;
        /// <summary>
        /// 屏幕内容不透明度
        /// </summary>
        SerializedProperty sp_op_content_opacity_screen;
        /// <summary>
        /// 世界内容不透明度
        /// </summary>
        SerializedProperty sp_op_content_opacity_world;
        #endregion

        #region 全局开关
        /// <summary>
        /// 是否使用单例
        /// </summary>
        SerializedProperty sp_use_instance;
        /// <summary>
        /// 是否使用调试模式
        /// </summary>
        SerializedProperty sp_use_debug;
        /// <summary>
        /// 全局音量
        /// </summary>
        SerializedProperty sp_global_volume;
        /// <summary>
        /// 全局音量静音
        /// </summary>
        SerializedProperty sp_global_volume_mute;
        /// <summary>
        /// 全局缓动时长
        /// </summary>
        SerializedProperty sp_tween_global_duration;
        #endregion

        #region 鼠标与转场控制器
        /// <summary>
        /// 自定义光标
        /// </summary>
        SerializedProperty sp_mos_custom_cursor;
        /// <summary>
        /// 鼠标光标
        /// </summary>
        SerializedProperty sp_mos_mouse_cursor;
        /// <summary>
        /// 自定义转场
        /// </summary>
        SerializedProperty sp_trans_custom_transition;
        /// <summary>
        /// 转场控制器
        /// </summary>
        SerializedProperty sp_trans_transition_controller;
        #endregion

        #region 蓝图
        /// <summary>
        /// 蓝图根节点
        /// </summary>
        SerializedProperty sp_bp_root;
        /// <summary>
        /// 蓝图模式
        /// </summary>
        SerializedProperty sp_bp_mode;
        /// <summary>
        /// 蓝图网格尺寸
        /// </summary>
        SerializedProperty sp_bp_grid_size;
        /// <summary>
        /// 蓝图网格颜色
        /// </summary>
        SerializedProperty sp_bp_grid_color;
        /// <summary>
        /// 蓝图背景颜色
        /// </summary>
        SerializedProperty sp_bp_bg_color;
        /// <summary>
        /// 蓝图背景贴花颜色
        /// </summary>
        SerializedProperty sp_bp_bg_decal_color;
        /// <summary>
        /// 蓝图标记尺寸
        /// </summary>
        SerializedProperty sp_bp_mark_size;
        /// <summary>
        /// 蓝图标记不透明度
        /// </summary>
        SerializedProperty sp_bp_mark_opacity;
        /// <summary>
        /// 蓝图线宽
        /// </summary>
        SerializedProperty sp_bp_linewidth;
        /// <summary>
        /// 蓝图标题内容
        /// </summary>
        SerializedProperty sp_bp_title_content;
        /// <summary>
        /// 蓝图副标题内容
        /// </summary>
        SerializedProperty sp_bp_subtitle_content;
        /// <summary>
        /// 蓝图标记标题颜色
        /// </summary>
        SerializedProperty sp_bp_marktitle_color;
        /// <summary>
        /// 蓝图标记副标题颜色
        /// </summary>
        SerializedProperty sp_bp_marksubtitle_color;
        /// <summary>
        /// 蓝图标记边距
        /// </summary>
        SerializedProperty sp_bp_mark_margin;
        /// <summary>
        /// 蓝图标记锚点
        /// </summary>
        SerializedProperty sp_bp_mark_anchors;
        /// <summary>
        /// 蓝图标记间距
        /// </summary>
        SerializedProperty sp_bp_mark_space;
        /// <summary>
        /// 蓝图不透明度
        /// </summary>
        SerializedProperty sp_bp_opacity;
        /// <summary>
        /// 蓝图动画时长
        /// </summary>
        SerializedProperty sp_bp_animation_duration;
        /// <summary>
        /// 蓝图是否已显示
        /// </summary>
        SerializedProperty sp_bp_displayed;
        /// <summary>
        /// 蓝图开始时是否隐藏
        /// </summary>
        SerializedProperty sp_bp_on_start_hide;
        /// <summary>
        /// 蓝图网格动画缓动
        /// </summary>
        SerializedProperty sp_bp_grid_animation_ease;
        /// <summary>
        /// 蓝图网格不透明度
        /// </summary>
        SerializedProperty sp_bp_grid_opacity;
        /// <summary>
        /// 蓝图网格动画时长
        /// </summary>
        SerializedProperty sp_bp_grid_animation_duration;
        /// <summary>
        /// 蓝图背景淡入缓动
        /// </summary>
        SerializedProperty sp_bp_bg_animation_ease_in;
        /// <summary>
        /// 蓝图背景淡出缓动
        /// </summary>
        SerializedProperty sp_bp_bg_animation_ease_out;
        /// <summary>
        /// 蓝图背景不透明度
        /// </summary>
        SerializedProperty sp_bp_bg_opacity;
        /// <summary>
        /// 蓝图背景淡入动画延迟
        /// </summary>
        SerializedProperty sp_bp_bg_fade_animation_delay;
        /// <summary>
        /// 蓝图网格长度百分比
        /// </summary>
        SerializedProperty sp_bp_grid_length_percentage;
        /// <summary>
        /// 蓝图网格层级高度
        /// </summary>
        SerializedProperty sp_bp_grid_level_height;
        /// <summary>
        /// 蓝图网格结束位置
        /// </summary>
        SerializedProperty sp_bp_grid_end;
        /// <summary>
        /// 蓝图背景平铺
        /// </summary>
        SerializedProperty sp_bp_bg_tilling;
        /// <summary>
        /// 蓝图背景名称
        /// </summary>
        SerializedProperty sp_bp_bg_name;
        /// <summary>
        /// 蓝图背景平铺索引
        /// </summary>
        SerializedProperty sp_bp_bg_usetilling_index;
        /// <summary>
        /// 蓝图背景方形比例索引
        /// </summary>
        SerializedProperty sp_bp_bg_usesquareratio_index;
        /// <summary>
        /// 蓝图背景贴图不透明度
        /// </summary>
        SerializedProperty sp_bp_bg_map_opacity;
        /// <summary>
        /// 蓝图特效网格
        /// </summary>
        SerializedProperty sp_bp_eft_grid;
        /// <summary>
        /// 蓝图特效网格淡入
        /// </summary>
        SerializedProperty sp_bp_eft_grid_fade;
        /// <summary>
        /// 蓝图特效背景
        /// </summary>
        SerializedProperty sp_bp_eft_bg;
        /// <summary>
        /// 蓝图特效标记
        /// </summary>
        SerializedProperty sp_bp_eft_mark;
        #endregion

        #region RMS 与主题
        /// <summary>
        /// RMS 是否启用
        /// </summary>
        SerializedProperty sp_rms_enabled;
        /// <summary>
        /// RMS 当前方案
        /// </summary>
        SerializedProperty sp_rms_current_solution;
        /// <summary>
        /// RMS 节点列表
        /// </summary>
        SerializedProperty sp_rms_nodes;
        /// <summary>
        /// 主题主色调
        /// </summary>
        SerializedProperty sp_theme_color_primary;
        /// <summary>
        /// 主题编组色
        /// </summary>
        SerializedProperty sp_theme_color_group;
        /// <summary>
        /// 主题分割线色
        /// </summary>
        SerializedProperty sp_theme_color_sepline;
        /// <summary>
        /// 主题主色调方案
        /// </summary>
        SerializedProperty sp_theme_solution_primary;
        /// <summary>
        /// 主题编组色方案
        /// </summary>
        SerializedProperty sp_theme_solution_group;
        /// <summary>
        /// 主题分割线色方案
        /// </summary>
        SerializedProperty sp_theme_solution_sepline;
        #endregion

        #region 事件系统与全局动效
        /// <summary>
        /// HUD 事件系统
        /// </summary>
        SerializedProperty sp_hud_event_system;
        /// <summary>
        /// HUD 输入系统 UI 输入模块
        /// </summary>
        SerializedProperty sp_hud_input_system_ui_input_module;
        /// <summary>
        /// 元素全局生成动效库索引名称
        /// </summary>
        SerializedProperty sp_ele_global_motion_create_library_index_name;
        /// <summary>
        /// 元素全局回收动效库索引名称
        /// </summary>
        SerializedProperty sp_ele_global_motion_recycle_library_index_name;
        #endregion

        #region 分辨率匹配
        /// <summary>
        /// 物理屏幕尺寸
        /// </summary>
        SerializedProperty sp_hmr_physics_screen_size;
        /// <summary>
        /// 参考图
        /// </summary>
        SerializedProperty sp_hmr_reference_image;
        /// <summary>
        /// 是否使用比例参考
        /// </summary>
        SerializedProperty sp_hmr_use_ratio_reference;
        /// <summary>
        /// 比例参考是否为局部
        /// </summary>
        SerializedProperty sp_hmr_ratio_reference_is_part;
        /// <summary>
        /// 参考图颜色
        /// </summary>
        SerializedProperty sp_hmr_reference_image_color;
        /// <summary>
        /// 完整分辨率
        /// </summary>
        SerializedProperty sp_hmr_res_full;
        /// <summary>
        /// 局部分辨率
        /// </summary>
        SerializedProperty sp_hmr_res_part;
        /// <summary>
        /// 参考形状比例尺寸
        /// </summary>
        SerializedProperty sp_hmr_refer_shape_ratio_size;
        /// <summary>
        /// 参考形状比例容差
        /// </summary>
        SerializedProperty sp_hmr_refer_shape_ratio_tolerance;
        #endregion

        #region 折叠与参考线
        /// <summary>
        /// 是否折叠所有禁用面板
        /// </summary>
        SerializedProperty sp_fold_all_panel_with_disabled;
        /// <summary>
        /// 参考线锚点
        /// </summary>
        SerializedProperty sp_cg_anchors;
        /// <summary>
        /// 参考线锚点根节点
        /// </summary>
        SerializedProperty sp_cg_anchor_root;
        /// <summary>
        /// 参考线是否启用
        /// </summary>
        SerializedProperty sp_cg_use;
        /// <summary>
        /// 参考线模式
        /// </summary>
        SerializedProperty sp_cg_mode;
        /// <summary>
        /// 参考线颜色
        /// </summary>
        SerializedProperty sp_cg_color;
        /// <summary>
        /// 参考线点颜色
        /// </summary>
        SerializedProperty sp_cg_point_color;
        /// <summary>
        /// 参考线左右镜像偏移
        /// </summary>
        SerializedProperty sp_cg_mirror_lr_offset;
        /// <summary>
        /// 参考线上下镜像偏移
        /// </summary>
        SerializedProperty sp_cg_mirror_ud_offset;
        /// <summary>
        /// 参考线左右镜像黄金模式
        /// </summary>
        SerializedProperty sp_cg_mirror_lr_golden_mode;
        /// <summary>
        /// 参考线上下镜像黄金模式
        /// </summary>
        SerializedProperty sp_cg_mirror_ud_golden_mode;
        /// <summary>
        /// 参考线斐波那契模式
        /// </summary>
        SerializedProperty sp_cg_fibonacci_mode;
        /// <summary>
        /// 角形视点水平偏移
        /// </summary>
        SerializedProperty sp_cg_editor_serializefield_corner_lookat_offset_h;
        /// <summary>
        /// 角形视点垂直偏移
        /// </summary>
        SerializedProperty sp_cg_editor_serializefield_corner_lookat_offset_v;
        /// <summary>
        /// 三分法水平偏移
        /// </summary>
        SerializedProperty sp_cg_editor_serializefield_three_offset_h;
        /// <summary>
        /// 三分法偏移覆盖范围
        /// </summary>
        SerializedProperty sp_cg_editor_serializefield_three_offset_coverage;
        /// <summary>
        /// 三分法垂直偏移
        /// </summary>
        SerializedProperty sp_cg_editor_serializefield_three_offset_v;
        /// <summary>
        /// 引导线基准高度
        /// </summary>
        SerializedProperty sp_cg_editor_serializefield_guide_line_base_height;
        /// <summary>
        /// 引导线近端偏移
        /// </summary>
        SerializedProperty sp_cg_editor_serializefield_guide_line_offset_near;
        /// <summary>
        /// 引导线远端偏移
        /// </summary>
        SerializedProperty sp_cg_editor_serializefield_guide_line_offset_far;
        /// <summary>
        /// 引导线近端高度偏移
        /// </summary>
        SerializedProperty sp_cg_editor_serializefield_guide_line_offset_near_height;
        /// <summary>
        /// 三角形基准高度
        /// </summary>
        SerializedProperty sp_cg_editor_serializefield_triangle_base_height;
        /// <summary>
        /// 三角形底部高度
        /// </summary>
        SerializedProperty sp_cg_editor_serializefield_guide_param_triangle_bottom_height;
        /// <summary>
        /// 三角形左侧偏移
        /// </summary>
        SerializedProperty sp_cg_editor_serializefield_triangle_offset_left;
        /// <summary>
        /// 三角形右侧偏移
        /// </summary>
        SerializedProperty sp_cg_editor_serializefield_triangle_offset_right;
        /// <summary>
        /// 三角形顶部偏移
        /// </summary>
        SerializedProperty sp_cg_editor_serializefield_triangle_top_offset;
        /// <summary>
        /// 中心点尺寸
        /// </summary>
        SerializedProperty sp_cg_editor_serializefield_center_point_size;
        /// <summary>
        /// I 形顶部高度
        /// </summary>
        SerializedProperty sp_cg_editor_serializefield_ishape_top_height;
        /// <summary>
        /// I 形底部高度
        /// </summary>
        SerializedProperty sp_cg_editor_serializefield_ishape_bottom_height;
        /// <summary>
        /// I 形左侧偏移
        /// </summary>
        SerializedProperty sp_cg_editor_serializefield_ishape_offset_left;
        /// <summary>
        /// I 形右侧偏移
        /// </summary>
        SerializedProperty sp_cg_editor_serializefield_ishape_offset_right;
        /// <summary>
        /// 引导线基准偏移
        /// </summary>
        SerializedProperty sp_cg_editor_serializefield_guide_line_base_offset;
        #endregion

        #region 图元可视化
        /// <summary>
        /// 元素可视化放置器
        /// </summary>
        SerializedProperty sp_ele_visual_placer;
        /// <summary>
        /// 图元 ID 标签行高
        /// </summary>
        SerializedProperty sp_primitive_id_label_line_height;
        /// <summary>
        /// 图元 ID 标签行颜色
        /// </summary>
        SerializedProperty sp_primitive_id_label_line_color;
        /// <summary>
        /// 图元 ID 标签字体大小
        /// </summary>
        SerializedProperty sp_primitive_id_label_font_size;
        /// <summary>
        /// 图元 ID 标签字体颜色
        /// </summary>
        SerializedProperty sp_primitive_id_label_font_color;
        #endregion
        #endregion

        #region 字段 - 图标
        /// <summary>
        /// 状态图标
        /// </summary>
        public Texture2D status;
        /// <summary>
        /// RMS 图标
        /// </summary>
        public Texture2D rmsicon;
        /// <summary>
        /// 鼠标光标启用图标
        /// </summary>
        public Texture2D icon_MC_Use;
        /// <summary>
        /// 鼠标光标禁用图标
        /// </summary>
        public Texture2D icon_MC_None;
        /// <summary>
        /// 音效播放中图标
        /// </summary>
        public Texture2D soundplaying;
        /// <summary>
        /// 面板图标 - 选项
        /// </summary>
        public Texture2D panel_option;
        /// <summary>
        /// 面板图标 - 画布
        /// </summary>
        public Texture2D panel_canvas;
        /// <summary>
        /// 面板图标 - 相机
        /// </summary>
        public Texture2D panel_camera;
        /// <summary>
        /// 面板图标 - 库
        /// </summary>
        public Texture2D panel_libs;
        /// <summary>
        /// 面板图标 - 元素库
        /// </summary>
        public Texture2D panel_elementlibs;
        /// <summary>
        /// 面板图标 - 安全框
        /// </summary>
        public Texture2D panel_frame;
        /// <summary>
        /// 面板图标 - 辅助
        /// </summary>
        public Texture2D panel_assist;
        /// <summary>
        /// 面板图标 - 蓝图
        /// </summary>
        public Texture2D panel_bulueprint;
        /// <summary>
        /// 面板图标 - 分辨率匹配
        /// </summary>
        public Texture2D panel_matchres;
        /// <summary>
        /// 面板图标 - 音频
        /// </summary>
        public Texture2D panel_audios;
        /// <summary>
        /// 面板图标 - 掩码
        /// </summary>
        public Texture2D panel_mask;
        /// <summary>
        /// 面板图标 - 全局
        /// </summary>
        public Texture2D panel_global;
        /// <summary>
        /// 面板图标 - 工具
        /// </summary>
        public Texture2D panel_tools;
        /// <summary>
        /// 面板图标 - 元素参数回收自动
        /// </summary>
        public Texture2D panel_eleparamrecycle_auto;
        /// <summary>
        /// 面板图标 - 元素参数动效默认
        /// </summary>
        public Texture2D panel_eleparammotion_default;
        /// <summary>
        /// 面板图标 - 主题
        /// </summary>
        public Texture2D panel_theme;
        /// <summary>
        /// 面板图标 - 本地化
        /// </summary>
        public Texture2D panel_localized;
        /// <summary>
        /// 面板图标 - 物理比例
        /// </summary>
        public Texture2D panel_physicsratio;
        /// <summary>
        /// 保存按钮常态图标
        /// </summary>
        public Texture2D save_r;
        /// <summary>
        /// 保存按钮按下图标
        /// </summary>
        public Texture2D save_p;
        /// <summary>
        /// 定位按钮常态图标
        /// </summary>
        public Texture2D locate_r;
        /// <summary>
        /// 定位按钮按下图标
        /// </summary>
        public Texture2D locate_p;
        /// <summary>
        /// 重置按钮常态图标
        /// </summary>
        public Texture2D reset_r;
        /// <summary>
        /// 重置按钮按下图标
        /// </summary>
        public Texture2D reset_p;
        /// <summary>
        /// 编辑按钮常态图标
        /// </summary>
        public Texture2D edit_r;
        /// <summary>
        /// 编辑按钮按下图标
        /// </summary>
        public Texture2D edit_p;
        /// <summary>
        /// 初始化按钮常态图标
        /// </summary>
        public Texture2D icon_initia_r;
        /// <summary>
        /// 初始化按钮按下图标
        /// </summary>
        public Texture2D icon_initia_p;
        /// <summary>
        /// 创建库按钮常态图标
        /// </summary>
        public Texture2D icon_libs_create_r;
        /// <summary>
        /// 创建库按钮按下图标
        /// </summary>
        public Texture2D icon_libs_create_p;
        /// <summary>
        /// 安全框可视化按钮常态图标
        /// </summary>
        public Texture2D icon_safeframevisual_r;
        /// <summary>
        /// 安全框可视化按钮按下图标
        /// </summary>
        public Texture2D icon_safeframevisual_p;
        /// <summary>
        /// 状态检查按钮常态图标
        /// </summary>
        public Texture2D icon_checkstate_r;
        /// <summary>
        /// 状态检查按钮按下图标
        /// </summary>
        public Texture2D icon_checkstate_p;
        /// <summary>
        /// 射线常态图标
        /// </summary>
        public Texture2D icon_ray_normal;
        /// <summary>
        /// 射线遮罩图标
        /// </summary>
        public Texture2D icon_ray_masked;
        /// <summary>
        /// 面板图标 - 组件引导
        /// </summary>
        public Texture2D panel_comp_guide;
        /// <summary>
        /// 引导帮助常态图标
        /// </summary>
        public Texture2D GuideHelp_Released;
        /// <summary>
        /// 引导帮助按下图标
        /// </summary>
        public Texture2D GuideHelp_Press;
        /// <summary>
        /// Logo 背景
        /// </summary>
        public Texture2D logo_bg;
        /// <summary>
        /// Logo
        /// </summary>
        public Texture2D logo;
        /// <summary>
        /// 主题标记
        /// </summary>
        public Texture2D themes_mark;
        #endregion

        #region 字段 - 蓝图
        /// <summary>
        /// 蓝图背景贴图列表。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>在 <c>OnEnable</c> 中扫描蓝图背景目录填充。</description></item>
        /// </list>
        /// </summary>
        List<Texture2D> blue_print_bgs = new List<Texture2D>();
        /// <summary>
        /// 蓝图背景贴图名称数组。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>与 <c>BluePrintBgs</c> 一一对应。</description></item>
        /// </list>
        /// </summary>
        string[] blue_print_bgs_name;
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
        XHud_Manager[] selected_objects;
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

            base_script = (XHud_Manager)target;
            line_height = EditorGUIUtility.singleLineHeight;

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
            edit_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/edit_r");
            edit_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_manager/edit_p");
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
                if (sp_cv_hudcanvas_screen_anchor.objectReferenceValue != null)
                {
                    if (sp_cv_anchors_layout_screen.arraySize <= 0)
                    {
                        UnityEngine.RectTransform root = sp_cv_hudcanvas_screen_anchor.objectReferenceValue as UnityEngine.RectTransform;
                        for (int i = 0; i < root.childCount; i++)
                        {
                            if (root.GetChild(i).name.Contains("Anchor"))
                            {
                                UnityEngine.RectTransform obj = root.GetChild(i).GetComponent<UnityEngine.RectTransform>();
                                sp_cv_anchors_layout_screen.InsertArrayElementAtIndex(sp_cv_anchors_layout_screen.arraySize);
                                sp_cv_anchors_layout_screen.GetArrayElementAtIndex(i).FindPropertyRelative("Name").stringValue = obj.name;
                                sp_cv_anchors_layout_screen.GetArrayElementAtIndex(i).FindPropertyRelative("Anchor").objectReferenceValue = obj;
                                if (obj.childCount > 0)
                                {
                                    for (int c = 0; c < obj.childCount; c++)
                                    {
                                        if (obj.GetChild(c).name == "Mark")
                                        {
                                            Image mark = obj.GetChild(c).GetComponent<Image>();
                                            sp_cv_anchors_layout_screen.GetArrayElementAtIndex(i).FindPropertyRelative("Mark").objectReferenceValue = mark;
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
            sp_cv_hudcanvas_screen_anchor.objectReferenceValue = base_script.transform.Find("Screen/Anchors");
            sp_cv_hudcanvas_screen_anchor.serializedObject.ApplyModifiedProperties();

            xHud_GetElementPoolIndicators();

            Init_RMSList();

            Init_ElementLibList();

            Init_SoundsPoolList();

            #region 获取所有蓝图背景贴图
            Texture2D[] AllTextures = XGUI.LoadAllAssetsAtPathWithPattern<Texture2D>($"{XHud_Dashboard.Get_Path_XHUD_TEXTURES_Path()}BluePrints/", ".png").ToArray();

            blue_print_bgs_name = new string[AllTextures.Length];
            for (int i = 0; i < AllTextures.Length; i++)
            {
                blue_print_bgs.Add(AllTextures[i]);
                blue_print_bgs_name[i] = AllTextures[i].name;
            }
            #endregion

            base_script.hm_BluePrintRootFirstSibling();

            EditorApplication.hierarchyChanged -= EditorApplication_EditorManagerUpdate;
            EditorApplication.hierarchyChanged += EditorApplication_EditorManagerUpdate;

            xHud_InitiaGameViewResolutions();

            // 创建图层 - XHud
            xHud_EnsureLayerExists("XHud");

            // 创建图层 - XHud_World
            xHud_EnsureLayerExists("XHud_World");

            //设置XHud管理器图层为XHud
            base_script.gameObject.layer = LayerMask.NameToLayer("XHud");

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

            base_script.hm_BluePrintRootFirstSibling();

            if (sp_fold_all_panel_with_disabled.boolValue)
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

            if (initialized.boolValue)
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
                lib_color.library_name = $"NewColorsLibrary";
                lib_color.ColorsLibrary_AddColor("DefaultColor", Color.white);

                //创建曲线库
                XHud_Library_Curves lib_curve = ScriptableObject.CreateInstance<XHud_Library_Curves>();
                lib_curve.library_name = $"NewCurveLibrary";
                lib_curve.CurveLibrary_AddCurve("DefaultCurve", AnimationCurve.EaseInOut(0, 0, 1, 1));

                //创建音效库
                XHud_Library_Sounds lib_sound = ScriptableObject.CreateInstance<XHud_Library_Sounds>();
                lib_sound.library_name = $"NewSoundLibrary";
                lib_sound.SoundLibrary_AddSound("DefaultSound", AssetDatabase.LoadAssetAtPath<AudioClip>($"{XHud_Dashboard.Get_Path_XHUD_SOUND_Path()}DefaultSound.wav"), "wav");

                //创建元素库
                XHud_Library_Element lib_element = ScriptableObject.CreateInstance<XHud_Library_Element>();
                lib_element.library_name = $"NewElementLibrary";
                Transform obj_XHudSample = AssetDatabase.LoadAssetAtPath<Transform>($"{XHud_Dashboard.Get_Path_XHUD_PREFABS_Path()}XHudIcon.prefab");
                lib_element.ElementsLibrary_Add(new XHud_LibraryArg_Element_Item(1, obj_XHudSample.GetComponent<XHud_Module_Element>()));

                //创建字体库
                XHud_Library_TextStyle lib_textstyle = ScriptableObject.CreateInstance<XHud_Library_TextStyle>();
                lib_textstyle.library_name = $"NewTextStyleLibrary";

                XHud_LibraryArg_TextStyle info_text = new XHud_LibraryArg_TextStyle();
                info_text.type = xHud_TextType.Text;
                info_text.rich_text = true;
                info_text.maskable = false;
                info_text.raycast = false;
                info_text.best_fit = false;
                info_text.geometre_align = false;
                info_text.font = AssetDatabase.LoadAssetAtPath<Font>($"{XHud_Dashboard.Get_Path_XHUD_FONTS_Path()}Text/sx_light.otf");
                info_text.font_color = Color.white;
                info_text.size = 50;
                info_text.line_height = 1;
                info_text.name = "NewTextStyle_HudText";
                info_text.description = "这是一个用于HudText文字组件的默认字体样式";
                info_text.style = FontStyle.Normal;
                info_text.overflow_horizon = HorizontalWrapMode.Overflow;
                info_text.overflow_vertical = VerticalWrapMode.Overflow;
                info_text.content_anchor = ContentAnchor.中心;

                XHud_LibraryArg_TextStyle info_tmptext = new XHud_LibraryArg_TextStyle();
                info_tmptext.type = xHud_TextType.TmpText;
                info_tmptext.tmp_rich = true;
                info_tmptext.maskable = false;
                info_tmptext.raycast = false;
                info_tmptext.tmp_enable_auto_sizing = false;
                info_tmptext.tmp_font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>($"{XHud_Dashboard.Get_Path_XHUD_FONTS_Path()}Tmp/sx_light SDF.asset");
                info_tmptext.tmp_color = Color.white;
                info_tmptext.size = 50;
                info_tmptext.tmp_overflow = TextOverflowModes.Overflow;
                info_tmptext.tmp_contentwrap = TextWrappingModes.NoWrap;
                info_tmptext.tmp_anchor = TmpContentAnchor.中心;
                info_tmptext.tmp_space_lineheight = 1;
                info_tmptext.name = "NewTextStyle_HudTmpText";
                info_tmptext.description = "这是一个用于HudTmpText文字组件的默认字体样式";
                info_tmptext.content_anchor = ContentAnchor.中心;

                lib_textstyle.TextStyle_Library_Add(info_text);
                lib_textstyle.TextStyle_Library_Add(info_tmptext);

                //创建动效库
                XHud_Library_Motion lib_motion = ScriptableObject.CreateInstance<XHud_Library_Motion>();
                lib_motion.library_name = $"NewMotionLibrary";

                #region 预制动效
                Motion_Creator crc = new Motion_Creator();
                crc.anchor = XHudAnchor.中心;
                crc.Movement = new MotionNode_Movement();
                crc.Movement.Movement = HudMotion_Movement.S_从下至上;
                crc.Movement.Distance = 100f;
                crc.Movement.Duration = 1f;
                crc.Movement.Delay = 0f;
                crc.Movement.CurveName = "";
                crc.Movement.Curve = base_script.library_curves != null ? base_script.library_curves.CurveLibrary_GetCurve(crc.Movement.CurveName) : null;
                crc.Movement.Ease = EaseMode.OutQuart;
                crc.Rotation = new MotionNode_Rotation();
                crc.Rotation.Rotation = HudMotion_Rotation.A_无旋转;
                crc.Rotation.Degree = 0f;
                crc.Rotation.Duration = 1f;
                crc.Rotation.Delay = 0f;
                crc.Rotation.CurveName = "";
                crc.Rotation.Curve = base_script.library_curves != null ? base_script.library_curves.CurveLibrary_GetCurve(crc.Rotation.CurveName) : null;
                crc.Rotation.Ease = EaseMode.OutQuart;
                crc.Alpha = new MotionNode_Alpha();
                crc.Alpha.Duration = 1f;
                crc.Alpha.Delay = 0f;
                crc.Alpha.CurveName = "";
                crc.Alpha.Curve = base_script.library_curves != null ? base_script.library_curves.CurveLibrary_GetCurve(crc.Alpha.CurveName) : null;
                crc.Alpha.Ease = EaseMode.OutQuart;

                Motion_Recycler rec = new Motion_Recycler();
                rec.Movement = new MotionNode_Movement();
                rec.Movement.Movement = HudMotion_Movement.D_从上至下;
                rec.Movement.Distance = 100f;
                rec.Movement.Duration = 1f;
                rec.Movement.Delay = 0f;
                rec.Movement.CurveName = "";
                rec.Movement.Curve = base_script.library_curves != null ? base_script.library_curves.CurveLibrary_GetCurve(rec.Movement.CurveName) : null;
                rec.Movement.Ease = EaseMode.OutQuart;
                rec.Rotation = new MotionNode_Rotation();
                rec.Rotation.Rotation = HudMotion_Rotation.A_无旋转;
                rec.Rotation.Degree = 0f;
                rec.Rotation.Duration = 1f;
                rec.Rotation.Delay = 0f;
                rec.Rotation.CurveName = "";
                rec.Rotation.Curve = base_script.library_curves != null ? base_script.library_curves.CurveLibrary_GetCurve(rec.Rotation.CurveName) : null;
                rec.Rotation.Ease = EaseMode.OutQuart;
                rec.Alpha = new MotionNode_Alpha();
                rec.Alpha.Duration = 1f;
                rec.Alpha.Delay = 0f;
                rec.Alpha.CurveName = "";
                rec.Alpha.Curve = base_script.library_curves != null ? base_script.library_curves.CurveLibrary_GetCurve(rec.Alpha.CurveName) : null;
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
                lib_transition.library_name = $"NewTransitionLibrary";
                List<Texture2D> texs_list = XGUI.LoadAllAssetsAtPathWithPattern<Texture2D>($"{XHud_Dashboard.Get_Path_XHUD_TEXTURES_Path()}Transitions/Ink/", ".jpg");
                // 调用排序方法
                xHud_SortTexturesByNameSuffix(texs_list);
                lib_transition.TransitionLibrary_Add("InkPaint", texs_list.ToArray(), 1);
                XHud_LibraryArg_Transition node = lib_transition.TransitionLibrary_Get("InkPaint");
                #region 分析转场数据
                if (node.frames == null || node.frames.Count <= 0)
                {
                    node.total_frames_count = 0;
                    node.last_frame_index = 0;
                    node.res = Vector2Int.zero;
                }
                else
                {
                    node.total_frames_count = node.frames.Count;
                    node.last_frame_index = node.frames.Count - 1;
                    for (int s = 0; s < node.frames.Count; s++)
                    {
                        // 获取纹理的导入设置
                        TextureImporter importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(node.frames[s])) as TextureImporter;

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
                    node.res = new Vector2Int(node.frames[0].width, node.frames[0].height);
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
                    base_script.library_colors = lib_color;
                    base_script.library_curves = lib_curve;
                    base_script.library_sounds = lib_sound;
                    base_script.library_textstyles = lib_textstyle;
                    base_script.library_motions = lib_motion;
                    base_script.library_transitions = lib_transition;
                    base_script.ele_library_elements.Add(lib_element);
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
                if (sp_camera_scene.objectReferenceValue == null)
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

                sp_sf_use.boolValue = !sp_sf_use.boolValue;
                sp_sf_use.serializedObject.ApplyModifiedProperties();

                if (!sp_sf_use.boolValue)
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
            if (initialized.boolValue)
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

            if (initialized.boolValue)
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

                string resinfo = string.Format("{0} x {1}", sp_cv_screen_res.vector2Value.x, sp_cv_screen_res.vector2Value.y);
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

                if (sp_rms_nodes.arraySize > 0)
                {
                    XGUI.layout_label(
                        text: "RMS： " + sp_rms_current_solution.stringValue,
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
                    Draw_Options();
                }

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                #endregion

                #region 画布 
                base_script.fold_canvasset = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIGroup.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                    title: "画布",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: base_script.fold_canvasset);

                if (!base_script.fold_canvasset)
                {
                    Draw_Canvas();
                }

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                #endregion

                #region 相机 
                base_script.fold_camera = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIGroup.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                    title: "相机",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: base_script.fold_camera);

                if (!base_script.fold_camera)
                {
                    Draw_Camera();
                }

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                #endregion

                #region 比例图例 
                base_script.fold_ratiorefer = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIGroup.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                    title: "比例图例 (单位 cm)",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: base_script.fold_ratiorefer);

                if (!base_script.fold_ratiorefer)
                {
                    Draw_RatioReference();
                }

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                #endregion

                #region 构图参考 
                if (sp_cg_use.boolValue)
                {
                    base_script.fold_compguid = XGUI.layout_group_start(
                        type: XGUIContainerType.Vertical,
                        bg_fill: XGUIGroup.缺口纯色边框,
                        bg_color: XGUIColor.亮白,
                        bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                        title: "构图参考",
                        title_size: XGUIFontSize.M,
                        title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_clipping: TextClipping.Clip,
                        padding: new RectOffset(10, 10, 15, 15),
                        foldout: base_script.fold_compguid);

                    if (!base_script.fold_compguid)
                    {
                        Draw_CompGuide();
                    }
                    XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                }
                #endregion

                #region 资源库 
                base_script.fold_reslibs = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIGroup.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                    title: "资源库",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: base_script.fold_reslibs);

                if (!base_script.fold_reslibs)
                {
                    Draw_Libraries();
                }

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                #endregion

                #region 元素库 
                base_script.fold_elementlibs = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIGroup.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                    title: "元素库",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: base_script.fold_elementlibs);

                if (!base_script.fold_elementlibs)
                {
                    Draw_ElementLibs();
                }

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                #endregion

                #region 音效池 
                base_script.fold_soundslib = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIGroup.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                    title: "音效池",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: base_script.fold_soundslib);

                if (!base_script.fold_soundslib)
                {
                    Draw_SoundsPool();
                }

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                #endregion

                #region 遮罩 / 内容透明度 
                base_script.fold_masks = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIGroup.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                    title: "遮罩 / 内容透明度",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: base_script.fold_masks);

                if (!base_script.fold_masks)
                {
                    Draw_Mask();
                }

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                #endregion

                #region 全局
                base_script.fold_global = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIGroup.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                    title: "全局",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: base_script.fold_global);

                if (!base_script.fold_global)
                {
                    Draw_Global();
                }

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                #endregion

                #region 工具
                base_script.fold_tool = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIGroup.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                    title: "工具",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: base_script.fold_tool);

                if (!base_script.fold_tool)
                {
                    Draw_Tool();
                }

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                #endregion

                #region 布局
                base_script.fold_layout = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIGroup.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                    title: "布局",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: base_script.fold_layout);

                if (!base_script.fold_layout)
                {
                    Draw_Layout();
                }

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                #endregion

                #region 辅助
                if (sp_sf_use.boolValue)
                {
                    base_script.fold_visualsafe = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIGroup.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                    title: "辅助",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: base_script.fold_visualsafe);

                    if (!base_script.fold_visualsafe)
                    {
                        Draw_SafeFrame();
                    }

                    XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                }
                #endregion

                #region 蓝图
                if (sp_bp_mode.boolValue)
                {
                    base_script.fold_blueprint = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIGroup.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                    title: "蓝图",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: base_script.fold_blueprint);

                    if (!base_script.fold_blueprint)
                    {
                        Draw_BluePrint();
                    }

                    if (!Application.isPlaying)
                    {
                        base_script.hm_BluePrint_Update();
                    }

                    XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                }
                #endregion

                #region RMS
                if (sp_rms_enabled.boolValue)
                {
                    base_script.fold_RMS = XGUI.layout_group_start(
                        type: XGUIContainerType.Vertical,
                        bg_fill: XGUIGroup.缺口纯色边框,
                        bg_color: XGUIColor.亮白,
                        bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                        title: "RMS",
                        title_size: XGUIFontSize.M,
                        title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_clipping: TextClipping.Clip,
                        padding: new RectOffset(10, 10, 15, 15),
                        foldout: base_script.fold_RMS);

                    if (!base_script.fold_RMS)
                    {
                        Draw_RMS();
                    }

                    XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                }
                #endregion

                #region 动效默认参数
                base_script.fold_motion_default = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIGroup.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                    title: "动效默认参数",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: base_script.fold_motion_default);

                if (!base_script.fold_motion_default)
                {
                    Draw_MotionDefault();
                }

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                #endregion

                #region 主题色
                base_script.fold_themes = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIGroup.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                    title: "主题色",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: base_script.fold_themes);

                if (!base_script.fold_themes)
                {
                    Draw_Theme();
                }

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
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
            }

            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                // 创建右键菜单
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Q (折叠所有项)"), false, () =>
                {
                    xHud_GroupFolds(true);
                });
                menu.AddItem(new GUIContent("W (展开所有项)"), false, () =>
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
            if (EditorApplication.timeSinceStartup - _lastUserActionTime > update_keeptime)
            {
                update_interval = update_interval_low;
            }

            // 频率限制
            if (EditorApplication.timeSinceStartup - _lastSceneUpdate < update_interval)
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
                selected_objects = new XHud_Manager[targets.Length];
                for (int i = 0; i < selected_objects.Length; i++)
                {
                    var t = targets[i];
                    selected_objects[i] = (XHud_Manager)t;
                }
            }
            else
            {
                selected_objects = new XHud_Manager[targets.Length];
                selected_objects[0] = (XHud_Manager)target;
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
            get_group = sizesType.GetMethod("GetGroup");
            game_view_sizes_instance = instanceProp.GetValue(null, null);
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
            return get_group.Invoke(game_view_sizes_instance, new object[] { (int)type });
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
            var addCustomSize = get_group.ReturnType.GetMethod("AddCustomSize");
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