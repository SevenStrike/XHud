/*
 * ============================================================================
 * ⚠️ 版权声明（禁止删除、禁止修改、衍生作品必须保留此注释）⚠️
 * ============================================================================
 * 版权声明 Copyright (C) 2025-Present Nanjing SevenStrike Media Co., Ltd.
 * 中文名称：南京塞维斯传媒有限公司
 * 英文名称：SevenStrikeMedia
 * 项目作者：徐寅智
 * 项目名称：XTween - Unity 高性能动画架构插件
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
namespace SevenStrikeModules.XTween.Editor
{
    using SevenStrikeModules.XGUI.Editor;
    using SevenStrikeModules.XGUI.Runtime;
    using System;
    using UnityEditor;
    using UnityEngine;

    public class Editor_XTween_Configurator : EditorWindow
    {
        private static Editor_XTween_Configurator window;

        /// <summary>
        /// 图标
        /// </summary>
        private Texture2D
            logo_bg,
            liquid_style_pure_press,
            liquid_style_scan_press,
            liquid_style_dirty_press,
            liquid_style_pure_released,
            liquid_style_scan_released,
            liquid_style_dirty_released,
            liquid_bg_pure,
            liquid_bg_scan,
            liquid_bg_dirty,
            liquid_status_playing,
            liquid_status_idle,
            liquid_status_performance_idle,
            liquid_status_performance_playing,
            liquid_highlight,
            liquid_plug,
            liquid_metal_grid;

        public bool IsPressed;

        public XTween_Config Config;

        SerializedObject serializedobject;
        SerializedProperty sp_LiquidColor_Idle, sp_LiquidColor_Playing, sp_Theme_Primary, sp_Theme_Group, sp_Theme_SeperateLine, sp_PoolCount_Int, sp_PoolCount_Float, sp_PoolCount_String, sp_PoolCount_Vector2, sp_PoolCount_Vector3, sp_PoolCount_Vector4, sp_PoolCount_Quaternion, sp_PoolCount_Color;

        [MenuItem("Assets/XTween/C 配置面板（Configurator)")]
        public static void Editor_Open_XTween_Configurator()
        {
            window = (Editor_XTween_Configurator)EditorWindow.GetWindow(typeof(Editor_XTween_Configurator), true, "XTween 配置面板", true);
            XGUI.CenterEditorWindow(new Vector2Int(380, 940), window);
            window.maxSize = window.minSize;
            window.Show();
        }

        [MenuItem("Assets/XTween/V 创建配置文件 (Configurator Create)")]
        public static void CreateConfigAsset()
        {
            XTween_Config con = XTween_Dashboard.GetXTweenConfig();

            EditorApplication.delayCall += () =>
            {
                if (con != null)
                {
                    string resUpdate = XGUI.dialog(
                          type: XGUIDialogType.警告,
                          windowtitle: "XTween 创建配置文件",
                          title: "配置文件已存在",
                          msg: "是否需要覆盖配置文件？",
                          ok: "覆盖 ",
                          cancel: "暂不",
                          PrimaryIndex: 0,
                          usemodal: true,
                          themecolor: XTween_Dashboard.Theme_Primary);

                    if (string.IsNullOrEmpty(resUpdate))
                    {
                        return;
                    }
                    if (resUpdate == "暂不")
                    {
                        return;
                    }

                    // 先同步删除旧资源
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(con));
                    // 或者使用 DestroyImmediate(con, true); 但推荐用 AssetDatabase.DeleteAsset 更彻底
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }

                // 创建 ScriptableObject 实例
                XTween_Config config = ScriptableObject.CreateInstance<XTween_Config>();

                // 初始化 TweenConfigData
                config.Datas = new ConfigDatas
                {
                    Theme_Primary = new Color(0.2313726f, 0.9960784f, 0.6078432f, 1),
                    Theme_Group = new Color(0.1176471f, 0.1176471f, 0.1176471f, 1),
                    Theme_SeperateLine = new Color(0.3254902f, 0.3254902f, 0.3254902f, 1),
                    LiquidScanStyle = true,
                    LiquidDirty = true,
                    LiquidBlinker = 1,
                    LiquidColor_Playing = new Color(0.5803922f, 0.6745098f, 0.3490196f, 1),
                    LiquidColor_Idle = new Color(0.4666667f, 0.5176471f, 0.3372549f, 1),
                    PoolCount_Int = 250,
                    PoolCount_Float = 250,
                    PoolCount_String = 250,
                    PoolCount_Vector2 = 250,
                    PoolCount_Vector3 = 250,
                    PoolCount_Vector4 = 250,
                    PoolCount_Quaternion = 250,
                    PoolCount_Color = 250,
                    PoolRecyleAllOnSceneUnloaded = true,
                    PoolRecyleAllOnSceneLoaded = false,
                    PerformanceLiquidMode = false,
                    PreviewOption_AutoKillPreviewTweens = true,
                    PreviewOption_RewindPreviewTweensWithKill = true,
                    PreviewOption_ClearPreviewTweensWithKill = true,
                    PresetInFavouriteMode = false,
                    PresetSelectionMark_LastTypeName = "Position",
                    PresetSelectionMark_LastRect = new Rect(x: 57, y: 191, width: 19, height: 19),
                    PresetCentralWindowSize = new Vector2(1023, 800)
                };

                // 保存路径
                string path = XTween_Dashboard.Get_path_XTween_Config_Path() + "XTweenConfig.asset";

                // 确保路径唯一
                path = AssetDatabase.GenerateUniqueAssetPath(path);

                // 创建资源文件
                AssetDatabase.CreateAsset(config, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                XTween_Dashboard.GetXTweenConfig();
                XTween_Dashboard.LoadColors_Themes();
                XTween_Dashboard.LoadColors_Liquid();

                // 选中创建的文件
                Selection.activeObject = config;
                EditorUtility.FocusProjectWindow();

                EditorApplication.delayCall += () =>
                {
                    XGUI.dialog(
                    type: XGUIDialogType.确认,
                    windowtitle: "XTween 创建配置文件",
                    title: "创建配置文件",
                    msg: $"XTween 配置文件已创建！请前往目录：{path} 中查看！",
                    ok: "明白",
                    PrimaryIndex: 0,
                    usemodal: true,
                    themecolor: XTween_Dashboard.Theme_Primary);
                };
            };
        }

        private void OnEnable()
        {
            #region 图标获取
            logo_bg = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_config/logo");

            liquid_style_pure_press = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_config/liquid_style_pure_press");
            liquid_style_pure_released = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_config/liquid_style_pure_released");

            liquid_style_scan_press = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_config/liquid_style_scan_press");
            liquid_style_scan_released = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_config/liquid_style_scan_released");

            liquid_style_dirty_press = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_config/liquid_style_dirty_press");
            liquid_style_dirty_released = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_config/liquid_style_dirty_released");

            liquid_bg_pure = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_config/liquid_bg_pure");
            liquid_bg_scan = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_config/liquid_bg_scan");
            liquid_bg_dirty = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_config/liquid_bg_dirty");

            liquid_status_playing = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_config/liquid_status_playing");
            liquid_status_idle = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_config/liquid_status_idle");

            liquid_status_performance_idle = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_config/liquid_status_performance_idle");
            liquid_status_performance_playing = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_config/liquid_status_performance_playing");

            liquid_highlight = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_config/liquid_highlight");

            liquid_plug = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_liquid/plug/liquid_plug_red");
            liquid_metal_grid = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_liquid/liquid_metal_grid");
            #endregion

            // 创建副本
            Config = ScriptableObject.CreateInstance<XTween_Config>();
            // 获取配置文件并克隆参数到副本
            Config.Datas = XTween_Dashboard.GetXTweenConfig().Datas.Clone();

            // 应用修改液晶 & 主题颜色
            XTween_Dashboard.LoadColors_Themes();
            XTween_Dashboard.LoadColors_Liquid();

            serializedobject = new SerializedObject(Config);

            sp_LiquidColor_Idle = serializedobject.FindProperty("Datas").FindPropertyRelative("LiquidColor_Idle");
            sp_LiquidColor_Playing = serializedobject.FindProperty("Datas").FindPropertyRelative("LiquidColor_Playing");
            sp_Theme_Primary = serializedobject.FindProperty("Datas").FindPropertyRelative("Theme_Primary");
            sp_Theme_Group = serializedobject.FindProperty("Datas").FindPropertyRelative("Theme_Group");
            sp_Theme_SeperateLine = serializedobject.FindProperty("Datas").FindPropertyRelative("Theme_SeperateLine");
            sp_PoolCount_Int = serializedobject.FindProperty("Datas").FindPropertyRelative("PoolCount_Int");
            sp_PoolCount_Float = serializedobject.FindProperty("Datas").FindPropertyRelative("PoolCount_Float");
            sp_PoolCount_String = serializedobject.FindProperty("Datas").FindPropertyRelative("PoolCount_String");
            sp_PoolCount_Vector2 = serializedobject.FindProperty("Datas").FindPropertyRelative("PoolCount_Vector2");
            sp_PoolCount_Vector3 = serializedobject.FindProperty("Datas").FindPropertyRelative("PoolCount_Vector3");
            sp_PoolCount_Vector4 = serializedobject.FindProperty("Datas").FindPropertyRelative("PoolCount_Vector4");
            sp_PoolCount_Quaternion = serializedobject.FindProperty("Datas").FindPropertyRelative("PoolCount_Quaternion");
            sp_PoolCount_Color = serializedobject.FindProperty("Datas").FindPropertyRelative("PoolCount_Color");
        }

        private void OnDisable()
        {
            // ✅ 销毁临时对象
            if (Config != null)
            {
                DestroyImmediate(Config, true);
                Config = null;
                serializedobject = null;
            }
        }

        private void OnGUI()
        {
            serializedobject.Update();

            Color color_theme = Config.Datas.Theme_Primary;
            Color color_group = Config.Datas.Theme_Group;
            Color color_sep = Config.Datas.Theme_SeperateLine;

            #region 抬头
            Rect rect = new Rect(0, 0, position.width, position.height);

            // 图标
            Rect rect_icon = new Rect(15, 15, logo_bg.width, logo_bg.height);
            XGUI.gui_icon(
                rect: rect_icon,
                icon: logo_bg,
                padding: new RectOffset(0, 0, 0, 0),
                border: new RectOffset(0, 0, 0, 0),
                color: Color.white);

#if UNITY_6000_0_OR_NEWER
            TextClipping clipping = TextClipping.Ellipsis;
#else
    TextClipping clipping = TextClipping.Clip;
#endif

            // 大标题
            Rect rect_title = new Rect(rect.x + 70, rect.y + 10, rect.width - 80, 30);
            XGUI.gui_label(
                rect: rect_title,
                text: new GUIContent("XTween 配置面板"),
                text_color: Color.white,
                size: XGUIFontSize.L,
                clipping: clipping,
                font: XGUI.GetFont("xg-heavy"));

            // 分割线
            Rect rect_seperate = new Rect(rect.x + 68, rect.y + 43, 200, 1);
            XGUI.gui_seperator(
                rect: rect_seperate,
                thickness: 1,
                color: color_sep,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0));

            // 小标题
            Rect rect_subtitle = new Rect(rect.x + 18, rect.y + 45, rect.width, 30);
            XGUI.gui_label(
                rect: rect_subtitle,
                text: new GUIContent("此面板可管理XTween的外观以及配置参数！"),
                text_color: Color.white * 0.7f,
                size: XGUIFontSize.M,
                clipping: clipping);
            #endregion

            Event ce = Event.current;
            if (ce.type == EventType.MouseDown)
            {
                // 取消当前拥有键盘焦点的控件
                GUI.FocusControl(null);
                Repaint();
            }

            XGUI.layout_space(100);

            #region 动画面板配置预览
            XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: color_group,
                title: "动画面板配置预览",
                title_size: XGUIFontSize.M,
                title_bg_fill: XGUIFilled.无,
                title_bg_color: XGUIColor.无,
                title_bg_color_gui: Color.white,
                title_text_color: color_theme,
                title_clipping: TextClipping.Clip,
                margin: new RectOffset(10, 10, 0, 0),
                padding: new RectOffset(10, 10, 15, 15));

            #region 面板样式预览
            Rect rect_liquid_previewer = XGUI.GetControlRect(false, liquid_bg_pure.height + 20);

            float width_cs = Config.Datas.PerformanceLiquidMode ? liquid_status_performance_idle.width : liquid_bg_pure.width;
            float height_cs = Config.Datas.PerformanceLiquidMode ? liquid_status_performance_idle.height : liquid_bg_pure.height;

            Rect rect_liquid = new Rect(rect_liquid_previewer.x - 5, rect_liquid_previewer.y + 3, width_cs, height_cs);

            // 测试区域
            //XGUI.gui_box(rect_liquid, Color.red);

            #region 检测鼠标事件
            Event currentEvent = Event.current;
            if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0 && rect_liquid.Contains(currentEvent.mousePosition))
            {
                IsPressed = true;
                currentEvent.Use();
            }
            if (currentEvent.type == EventType.MouseUp && currentEvent.button == 0 && rect_liquid.Contains(currentEvent.mousePosition))
            {
                IsPressed = false;
                currentEvent.Use();
            }
            #endregion

            #region 液晶背景
            Texture2D bg_style_original = (Config.Datas.LiquidScanStyle ? (Config.Datas.LiquidDirty ? liquid_bg_dirty : liquid_bg_scan) : liquid_bg_pure);
            Color col_playing = Config.Datas.LiquidColor_Playing;
            Color col_idle = Config.Datas.LiquidColor_Idle;
            Color col_liquid = IsPressed ? col_playing : col_idle;
            XGUI.gui_box(
                rect: rect_liquid,
                bg: Config.Datas.PerformanceLiquidMode ? null : bg_style_original,
                bg_color_gui: col_liquid,
                offset: new Vector2(0, 0),
                border: new RectOffset(0, 0, 0, 0),
                margin: new RectOffset(0, 0, 0, 0));
            #endregion

            #region  液晶屏内容
            float width_gf = Config.Datas.PerformanceLiquidMode ? liquid_status_performance_idle.width : liquid_status_playing.width;
            float height_gf = Config.Datas.PerformanceLiquidMode ? liquid_status_performance_idle.height : liquid_status_playing.height;

            Rect rect_liquid_content = new Rect(rect_liquid.x + 14, rect_liquid.y + 14, width_gf, height_gf);

            // 测试区域
            //XGUI.gui_box(rect_liquid_content, Color.red);

            Texture2D liquid_content = IsPressed ? (Config.Datas.PerformanceLiquidMode ? liquid_status_performance_playing : liquid_status_playing) : (Config.Datas.PerformanceLiquidMode ? liquid_status_performance_idle : liquid_status_idle);

            XGUI.gui_box(
                rect: rect_liquid_content,
                bg: liquid_content,
                bg_color_gui: Color.white,
                offset: new Vector2(0, 0),
                border: new RectOffset(0, 0, 0, 0),
                margin: new RectOffset(0, 0, 0, 0)
                );
            #endregion

            if (!Config.Datas.PerformanceLiquidMode)
            {
                // 液晶屏高光
                if (Config.Datas.LiquidScanStyle || Config.Datas.LiquidDirty)
                {
                    Rect rect_liquid_highlight = new Rect(rect_liquid.x, rect_liquid.y, liquid_highlight.width, liquid_highlight.height);
                    XGUI.gui_box(
                        rect: rect_liquid_highlight,
                        bg: liquid_highlight,
                        bg_color_gui: Color.white,
                        offset: new Vector2(0, 0),
                        border: new RectOffset(0, 0, 0, 0),
                        margin: new RectOffset(0, 0, 0, 0));
                }

                //  液晶屏接口
                Rect rect_liquid_plug = new Rect(rect_liquid.x + (rect_liquid.width / 2) - (liquid_plug.width / 2), rect_liquid.y + rect_liquid.height, liquid_plug.width, liquid_plug.height);
                XGUI.gui_box(
                    rect: rect_liquid_plug,
                    bg: liquid_plug,
                    bg_color_gui: Color.white,
                    offset: new Vector2(0, 0),
                    border: new RectOffset(0, 0, 0, 0),
                    margin: new RectOffset(0, 0, 0, 0));

                // 液晶屏金属网格角
                Rect rect_liquid_metal_grid = new Rect(rect_liquid.x + (rect_liquid.width - liquid_metal_grid.width) + 8, rect_liquid.y + (rect_liquid.height - liquid_metal_grid.height) + 10, liquid_metal_grid.width, liquid_metal_grid.height);
                XGUI.gui_box(
                    rect: rect_liquid_metal_grid,
                    bg: liquid_metal_grid,
                    bg_color_gui: Color.white,
                    offset: new Vector2(0, 0),
                    border: new RectOffset(0, 0, 0, 0),
                    margin: new RectOffset(0, 0, 0, 0));
            }
            #endregion

            #region 样式切换
            if (!Config.Datas.PerformanceLiquidMode)
            {
                float mar = 0;
                //  纯色
                Rect rect_liquid_style_pure = new Rect(rect_liquid.x + rect_liquid.width + 10, rect_liquid.y + mar, liquid_style_pure_released.width, liquid_style_pure_released.height);
                if (XGUI.gui_button(
                    rect: rect_liquid_style_pure,
                    tooltip: "纯色风格",
                    tex_release: liquid_style_pure_released,
                    tex_press: liquid_style_pure_press,
                    tex_gui_color: Config.Datas.LiquidColor_Idle,
                    margin: new RectOffset(0, 0, 0, 0),
                    padding: new RectOffset(0, 0, 0, 0),
                    focus_name: "liquid_style_pure_btn"))
                {
                    Config.Datas.LiquidScanStyle = false;
                    Config.Datas.LiquidDirty = false;
                }

                mar += 55;
                // 扫描线
                Rect rect_liquid_style_scan = new Rect(rect_liquid.x + rect_liquid.width + 10, rect_liquid.y + mar, liquid_style_scan_released.width, liquid_style_scan_released.height);
                if (XGUI.gui_button(
                    rect: rect_liquid_style_scan,
                    tooltip: "扫描线风格",
                    tex_release: liquid_style_scan_released,
                    tex_press: liquid_style_scan_press,
                    tex_gui_color: Config.Datas.LiquidColor_Idle,
                    margin: new RectOffset(0, 0, 0, 0),
                    padding: new RectOffset(0, 0, 0, 0),
                    focus_name: "liquid_style_pure_btn"))
                {
                    Config.Datas.LiquidScanStyle = true;
                    Config.Datas.LiquidDirty = false;
                }

                mar += 55;
                //  肮脏
                Rect rect_liquid_style_dirty = new Rect(rect_liquid.x + rect_liquid.width + 10, rect_liquid.y + mar, liquid_style_dirty_released.width, liquid_style_dirty_released.height);
                if (XGUI.gui_button(
                    rect: rect_liquid_style_dirty,
                    tooltip: "污迹的扫描线风格",
                    tex_release: liquid_style_dirty_released,
                    tex_press: liquid_style_dirty_press,
                    tex_gui_color: Config.Datas.LiquidColor_Idle,
                    margin: new RectOffset(0, 0, 0, 0),
                    padding: new RectOffset(0, 0, 0, 0),
                    focus_name: "liquid_style_pure_btn"))
                {
                    Config.Datas.LiquidScanStyle = true;
                    Config.Datas.LiquidDirty = true;
                }
            }
            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 主题颜色样式
            XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: color_group,
                title: "主题颜色样式",
                title_size: XGUIFontSize.M,
                title_bg_fill: XGUIFilled.无,
                title_bg_color: XGUIColor.无,
                title_bg_color_gui: Color.white,
                title_text_color: color_theme,
                title_clipping: TextClipping.Clip,
                margin: new RectOffset(10, 10, 0, 0),
                padding: new RectOffset(10, 10, 15, 15));

            DrawParamField_Colors("就绪时液晶色", sp_LiquidColor_Idle, 120);
            DrawParamField_Colors("播放时液晶色", sp_LiquidColor_Playing, 120);
            DrawParamField_Colors("主题颜色", sp_Theme_Primary, 120);
            DrawParamField_Colors("编组颜色", sp_Theme_Group, 120);
            DrawParamField_Colors("分割线颜色", sp_Theme_SeperateLine, 120);

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 动画池配置
            XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: color_group,
                title: "动画池配置",
                title_size: XGUIFontSize.M,
                title_bg_fill: XGUIFilled.无,
                title_bg_color: XGUIColor.无,
                title_bg_color_gui: Color.white,
                title_text_color: color_theme,
                title_clipping: TextClipping.Clip,
                margin: new RectOffset(10, 10, 0, 0),
                padding: new RectOffset(10, 10, 15, 15));

            DrawParamField_Colors("整型 - Integer", sp_PoolCount_Int, 120);
            DrawParamField_Colors("浮点 - Float", sp_PoolCount_Float, 120);
            DrawParamField_Colors("字符串 - String", sp_PoolCount_String, 120);
            DrawParamField_Colors("颜色 - Color", sp_PoolCount_Color, 120);
            DrawParamField_Colors("四元数 - Quaternion", sp_PoolCount_Quaternion, 120);
            DrawParamField_Colors("二维向量 - Vector2", sp_PoolCount_Vector2, 120);
            DrawParamField_Colors("三维向量 - Vector3", sp_PoolCount_Vector3, 120);
            DrawParamField_Colors("四维向量 - Vector4", sp_PoolCount_Vector4, 120);

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 选项
            XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: color_group,
                title: "选项",
                title_size: XGUIFontSize.M,
                title_bg_fill: XGUIFilled.无,
                title_bg_color: XGUIColor.无,
                title_bg_color_gui: Color.white,
                title_text_color: color_theme,
                title_clipping: TextClipping.Clip,
                margin: new RectOffset(10, 10, 0, 0),
                padding: new RectOffset(10, 10, 15, 15));

            // 调试信息
            Config.Datas.PerformanceLiquidMode = DrawToggle("极简化动画面板风格", Config.Datas.PerformanceLiquidMode, 200, (b) =>
            {

            });

            // 调试信息
            bool sw_blinker = Config.Datas.LiquidBlinker == 0 ? false : true;
            sw_blinker = DrawToggle("动画指示器闪烁", sw_blinker, 200, (b) =>
            {
                Config.Datas.LiquidBlinker = b ? 1 : 0;
            });

            // 调试信息
            Config.Datas.PoolRecyleAllOnSceneLoaded = DrawToggle("场景加载时动画池回收", Config.Datas.PoolRecyleAllOnSceneLoaded, 200, (b) =>
            {

            });

            // 调试信息
            Config.Datas.PoolRecyleAllOnSceneUnloaded = DrawToggle("场景卸载时动画池回收", Config.Datas.PoolRecyleAllOnSceneUnloaded, 200, (b) =>
            {

            });

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 配置文件操作
            XGUI.layout_group_start(
                type: XGUIContainerType.Horizontal,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: color_group,
                title: "配置文件操作",
                title_size: XGUIFontSize.M,
                title_bg_fill: XGUIFilled.无,
                title_bg_color: XGUIColor.无,
                title_bg_color_gui: Color.white,
                title_text_color: color_theme,
                title_clipping: TextClipping.Clip,
                margin: new RectOffset(10, 10, 0, 0),
                padding: new RectOffset(10, 10, 15, 15));

            if (XGUI.layout_button(
                          text: "复位",
                          tooltip: "",
                          bg_fill: XGUIFilled.实体,
                          bg_color: XGUIColor.警示黄,
                          bg_color_gui: Color.white,
                          button_text_color: Color.black,
                          press_fill: XGUIFilled.实体,
                          press_color: XGUIColor.深空灰,
                          press_text_color: Color.white,
                          font_size: XGUIFontSize.B,
                          anchor: TextAnchor.MiddleCenter,
                          margin: new RectOffset(0, 0, 0, 0),
                          padding: new RectOffset(0, 0, 0, 0),
                          //width: 20,
                          height: XGUI.GetSingleLineHeight() + 10,
                          button_text_font: XGUI.GetFont("xg-medium")))
            {
                // 使用延迟调用避免GUI布局冲突
                EditorApplication.delayCall += () =>
                {
                    string resReset = XGUI.dialog(
                        type: XGUIDialogType.警告,
                        windowtitle: "XTween配置面板消息",
                        title: "XTween配置参数复位",
                        msg: "接下来会将XTween的配置参数全部复位，此操作不可逆！请确定是否继续该操作？",
                        ok: "复位 ",
                        cancel: "暂不",
                        PrimaryIndex: 0,
                        usemodal: true,
                        themecolor: XTween_Dashboard.Theme_Primary);

                    if (string.IsNullOrEmpty(resReset))
                    {
                        return;
                    }
                    if (resReset == "暂不")
                    {
                        return;
                    }

                    Config.Datas.Theme_Primary = new Color(0.2313726f, 0.9960784f, 0.6078432f, 1);
                    Config.Datas.Theme_Group = new Color(0.1176471f, 0.1176471f, 0.1176471f, 1);
                    Config.Datas.Theme_SeperateLine = new Color(0.3254902f, 0.3254902f, 0.3254902f, 1);
                    Config.Datas.PerformanceLiquidMode = false;
                    Config.Datas.LiquidScanStyle = true;
                    Config.Datas.LiquidDirty = true;
                    Config.Datas.LiquidBlinker = 1;
                    Config.Datas.LiquidColor_Playing = new Color(0.5803922f, 0.6745098f, 0.3490196f, 1);
                    Config.Datas.LiquidColor_Idle = new Color(0.4666667f, 0.5176471f, 0.3372549f, 1);
                    Config.Datas.PoolCount_Int = 250;
                    Config.Datas.PoolCount_Float = 250;
                    Config.Datas.PoolCount_String = 250;
                    Config.Datas.PoolCount_Vector2 = 250;
                    Config.Datas.PoolCount_Vector3 = 250;
                    Config.Datas.PoolCount_Vector4 = 250;
                    Config.Datas.PoolCount_Quaternion = 250;
                    Config.Datas.PoolCount_Color = 250;
                    Config.Datas.PoolRecyleAllOnSceneUnloaded = true;
                    Config.Datas.PoolRecyleAllOnSceneLoaded = false;

                    sp_LiquidColor_Playing.colorValue = Config.Datas.LiquidColor_Playing;
                    sp_LiquidColor_Idle.colorValue = Config.Datas.LiquidColor_Idle;
                };
            }

            XGUI.layout_space(10);

            if (XGUI.layout_button(
                         text: "保存",
                         tooltip: "",
                         bg_fill: XGUIFilled.实体,
                         bg_color: XGUIColor.深空灰,
                         bg_color_gui: Color.white,
                         button_text_color: Color.white,
                         press_fill: XGUIFilled.实体,
                         press_color: XGUIColor.亮白,
                         press_text_color: Color.black,
                         font_size: XGUIFontSize.B,
                         anchor: TextAnchor.MiddleCenter,
                         margin: new RectOffset(0, 0, 0, 0),
                         padding: new RectOffset(0, 0, 0, 0),
                         //width: 20,
                         height: XGUI.GetSingleLineHeight() + 10,
                         button_text_font: XGUI.GetFont("xg-medium")))
            {
                if (!Application.isPlaying)
                {
                    // 使用延迟调用避免GUI布局冲突
                    EditorApplication.delayCall += () =>
                    {
                        string resUpdate = XGUI.dialog(
                          type: XGUIDialogType.警告,
                          windowtitle: "XTween配置面板消息",
                          title: "XTween配置参数更新",
                          msg: "接下来会将XTween的配置参数全部更新，此操作不可逆！请确定是否继续该操作？",
                          ok: "更新 ",
                          cancel: "暂不",
                          PrimaryIndex: 0,
                          usemodal: true,
                          themecolor: XTween_Dashboard.Theme_Primary);

                        if (string.IsNullOrEmpty(resUpdate))
                        {
                            return;
                        }
                        if (resUpdate == "暂不")
                        {
                            return;
                        }
                        SaveConfig();
                    };
                }
                else
                {
                    // 使用延迟调用避免GUI布局冲突
                    EditorApplication.delayCall += () =>
                    {
                        XGUI.dialog(
                            type: XGUIDialogType.警告,
                            windowtitle: "XTween配置面板消息",
                            title: "应用正在运行",
                            msg: "为避免出问题目前只有在非运行时期才可以更新配置参数哦！",
                            ok: "明白 ",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XTween_Dashboard.Theme_Primary);
                    };
                }
            }

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            #endregion

            Repaint();

            serializedobject.ApplyModifiedProperties();
        }

        private void Update()
        {
            if (Application.isPlaying)
            {
                Repaint();
            }
        }

        /// <summary>
        /// 保存配置到文件
        /// </summary>
        private void SaveConfig()
        {
            // ✅ 更安全的方式
            var original = XTween_Dashboard.GetXTweenConfig();
            if (original != null)
            {
                original.Datas = Config.Datas.Clone();
                EditorUtility.SetDirty(original);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                // 应用修改液晶 & 主题颜色
                XTween_Dashboard.LoadColors_Themes();
                XTween_Dashboard.LoadColors_Liquid();
            }

            XGUI.dialog(
                type: XGUIDialogType.确认,
                windowtitle: "XTween配置面板消息",
                title: "配置更新完成",
                msg: "已更新 XTween 的配置参数！",
                ok: "明白",
                PrimaryIndex: 0,
                usemodal: true,
                themecolor: XTween_Dashboard.Theme_Primary);
        }

        /// <summary>
        /// 通用方法：绘制参数
        /// </summary>
        private void DrawParamField_Colors(string title, SerializedProperty prop, float width)
        {
            XGUI.layout_property_field(
                title: title,
                title_size: XGUIFontSize.M,
                title_anchor: TextAnchor.MiddleLeft,
                title_hover_color: XTween_Dashboard.Theme_Primary,
                title_width: width,
                //status_icon: "icon_field_status",
                //status_icon_color: Color.green,
                prop: prop,
                prop_margin: new RectOffset(0, 0, 5, 0));
        }

        /// <summary>
        /// 通用方法：绘制开关
        /// </summary>
        private bool DrawToggle(string title, bool prop, float width, Action<bool> act_on_changed = null)
        {
            return XGUI.layout_toggle(
                  title: title,
                  title_size: XGUIFontSize.M,
                  title_font_style: FontStyle.Normal,
                  title_padding: new RectOffset(0, 10, 0, 0),
                  title_width: width,
                  prop: prop,
                  tog_style: XGUIToggleStyle.实体,
                  tog_padding: new RectOffset(5, 0, 0, 0),
                  tog_margin: new RectOffset(0, 0, 0, 5),
                  tog_mixed_options: new string[] { "禁用", "启用" },
                  tog_mixed_text_size: XGUIFontSize.M,
                  tog_mixed_text_color: Color.black,
                  tog_mixed_text_padding: new RectOffset(10, 10, 0, 0),
                  tog_mixed_text_anchor: TextAnchor.MiddleCenter,
                  tog_mixed_font_style: FontStyle.Normal,
                  tog_bg_off_color: new Color(0.38f, 0.38f, 0.38f),
                  tog_bg_on_color: XTween_Dashboard.Theme_Primary,
                  tog_handler_off_color: Color.white,
                  tog_handler_on_color: Color.white,
                  tog_mixed_bg_color_gui: XTween_Dashboard.Theme_Primary,
                  act_on_changed: act_on_changed);
        }
    }
}