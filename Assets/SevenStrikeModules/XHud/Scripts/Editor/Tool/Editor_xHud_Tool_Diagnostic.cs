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
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEditor.Callbacks;
    using UnityEngine;

    public class Editor_XHud_Tool_Diagnostic : EditorWindow
    {
        private SerializedObject BaseObject;

        private static Editor_XHud_Tool_Diagnostic window;

        [SerializeField]
        public List<Color> ColorGradient = new List<Color>();

        private Vector2 list_scrollPosition;
        private List<float> list_cachedItemHeights;
        private bool list_heightsDirty = true;

        private Texture2D logo, icon_element, icon_primitive, icon_sounder, icon_button, icon_toggle, icon_progress, icon_slider, icon_option, icon_text, icon_tmptext, sizebg, perspective, orthorgrphic, status_dot;

        /// <summary>
        /// Editor列表项高度
        /// </summary>
        public float itemHeight = 26;
        /// <summary>
        /// 可视区域显示的元素数量
        /// </summary>
        public int visibleItemCount = 20;
        /// <summary>
        /// 列表滚动位置
        /// </summary>
        public Vector2 GradientColors_Scroller;

        /// <summary>
        /// 选中项索引号
        /// </summary>
        public int SelectedIndex = 0;
        /// <summary>
        /// 状态面板索引号
        /// </summary>
        public int diagnostic_panel_index = 0;

        #region 模组检测开关
        // 图元遮罩
        private static bool mod_isexist_softmask;
        // 模糊遮罩
        private static bool mod_isexist_blurmask;
        // PSD 重建器
        private static bool mod_isexist_psd;
        // 色调提取器
        private static bool mod_isexist_color_capture;
        // 渐变色卡
        private static bool mod_isexist_gradient;
        // 旋转轴心矫正器
        private static bool mod_isexist_rotate_correct;
        // 白模生成器
        private static bool mod_isexist_mod_create;
        // 材质清理器
        private static bool mod_isexist_mat_clean;
        #endregion

        public string HexPrimaryColor;

        [MenuItem("Tools/XHud/Diagnostic ^#d")]
        static void Init()
        {
            if (XHud_Dashboard.HudManagerGet() == null)
                return;
            window = (Editor_XHud_Tool_Diagnostic)EditorWindow.GetWindow(typeof(Editor_XHud_Tool_Diagnostic), false, "XHUD 诊断/概况面板", true);
            XGUI.CenterEditorWindow(new Vector2Int(300, 560), window, false);
            window.Show();
        }

        private void OnDisable()
        {

        }

        private void OnEnable()
        {
            BaseObject = new SerializedObject(this);

            CheckToolsExists();

            logo = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_diagnostic/logo");
            icon_element = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_diagnostic/icon_element");
            icon_primitive = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_diagnostic/icon_primitive");
            icon_sounder = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_diagnostic/icon_sounder");
            icon_button = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_diagnostic/icon_button");
            icon_toggle = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_diagnostic/icon_toggle");
            icon_progress = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_diagnostic/icon_progress");
            icon_slider = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_diagnostic/icon_slider");
            icon_option = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_diagnostic/icon_option");
            icon_text = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_diagnostic/icon_text");
            icon_tmptext = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_diagnostic/icon_tmptext");
            sizebg = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_diagnostic/sizebg");
            perspective = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_diagnostic/perspective");
            orthorgrphic = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_diagnostic/orthorgrphic");
            status_dot = XGUI.GetBasedIcon("icon_field_status");

            HexPrimaryColor = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary, false);
        }

        private void OnDestroy()
        {
            GetWindow<SceneView>().Focus();
        }

        private void OnGUI()
        {
            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

            BaseObject.Update();

            #region 抬头
            Rect rect = new Rect(0, 0, position.width, position.height);

            // 图标
            Rect rect_icon = new Rect(15, 15, logo.width, logo.height);
            XGUI.gui_icon(
                rect: rect_icon,
                icon: logo,
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
                text: new GUIContent("XHud 诊断/概况面板"),
                text_color: Color.white,
                size: XGUIFontSize.L,
                clipping: clipping,
                font: XGUI.GetFont("xg-heavy"));

            // 分割线
            Rect rect_seperate = new Rect(rect.x + 68, rect.y + 43, 200, 1);
            XGUI.gui_seperator(
                rect: rect_seperate,
                thickness: 1,
                color: XHud_Dashboard.Theme_SeperateLine,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0));

            // 小标题
            Rect rect_subtitle = new Rect(rect.x + 18, rect.y + 52, rect.width - 18, 30);
            XGUI.gui_label(
                rect: rect_subtitle,
                text: new GUIContent("此工具可根据当前的XHUD的配置状态来进行诊断，确保您正确配置XHUD！同时也可在此查看组件使用状态等杂项信息"),
                text_color: Color.white * 0.7f,
                size: XGUIFontSize.M,
                wrap: true,
                anchor: TextAnchor.UpperLeft,
                clipping: TextClipping.Overflow,
                padding: new RectOffset(0, 10, 0, 0));
            #endregion

            XGUI.layout_space(120);

            diagnostic_panel_index = XGUI.layout_toolbar(
               index: ref diagnostic_panel_index,
               names: new string[] { "诊断", "库配置", "分辨率", "统计" },
               bg_normal: XGUIFilled.纯色边框,
               bg_selected: XGUIFilled.实体,
               bg_color: XGUIColor.亮白,
               bg_gui_color: Color.black * 0.5f,
               text_color_normal: Color.white,
               text_color_selected: XHud_Dashboard.Theme_Primary,
               bar_height: 25,
               text_anchor: TextAnchor.MiddleCenter,
               text_padding: new RectOffset(10, 10, 0, 0),
               bar_margin: new RectOffset(15, 15, 0, 0),
               bar_offset: new Vector2(16, 120),
               text_offset: new Vector2(0, 0),
               text_font: XGUI.GetFont("xg-regular"),
               text_fontstyle: FontStyle.Normal,
               bg_width_offset: 28,
               navigate_style: true,
               navigate_style_bg: XGUIFilled.纯色边框,
               navigate_style_bg_color: Color.black * 0.5f);

            XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.无,
                bg_color: XGUIColor.无,
                bg_color_gui: XHud_Dashboard.Theme_Group,
                title: "",
                title_size: XGUIFontSize.M,
                title_text_color: Color.white,
                title_clipping: TextClipping.Overflow,
                padding: new RectOffset(5, 0, 0, 20));

            switch (diagnostic_panel_index)
            {
                case 0:
                    state_displayer("管理器已创建", status_dot, mgr == null ? Color.black : XHud_Dashboard.Theme_Primary);
                    state_displayer("单例模式", status_dot, mgr.UseInstanceMode ? XHud_Dashboard.Theme_Primary : Color.black);
                    state_displayer("画布尺寸指定", status_dot, mgr.ScreenRes == Vector2.zero ? Color.black : XHud_Dashboard.Theme_Primary);
                    state_displayer("场景相机指定", status_dot, mgr.SceneCamera == null ? Color.black : XHud_Dashboard.Theme_Primary);
                    state_displayer("世界坐标支持", status_dot, mgr.SupportWorldUI ? XHud_Dashboard.Theme_Primary : Color.black);

                    XGUI.layout_seperator(thickness: 1,
                        color: Color.white * 0.5f,
                        margin: new RectOffset(15, 15, 10, 10));

                    state_displayer($"{(mod_isexist_blurmask ? "已" : "未")}装配：模糊遮罩", status_dot, mod_isexist_blurmask ? XHud_Dashboard.Theme_Primary : Color.black);
                    state_displayer($"{(mod_isexist_softmask ? "已" : "未")}装配：图元遮罩", status_dot, mod_isexist_softmask ? XHud_Dashboard.Theme_Primary : Color.black);
                    state_displayer($"{(mod_isexist_psd ? "已" : "未")}装配：PSD 重建器", status_dot, mod_isexist_psd ? XHud_Dashboard.Theme_Primary : Color.black);
                    state_displayer($"{(mod_isexist_color_capture ? "已" : "未")}装配：色调提取器", status_dot, mod_isexist_color_capture ? XHud_Dashboard.Theme_Primary : Color.black);
                    state_displayer($"{(mod_isexist_gradient ? "已" : "未")}装配：渐变色卡", status_dot, mod_isexist_gradient ? XHud_Dashboard.Theme_Primary : Color.black);
                    state_displayer($"{(mod_isexist_rotate_correct ? "已" : "未")}装配：旋转轴心矫正器", status_dot, mod_isexist_rotate_correct ? XHud_Dashboard.Theme_Primary : Color.black);
                    state_displayer($"{(mod_isexist_mod_create ? "已" : "未")}装配：白模生成器", status_dot, mod_isexist_mod_create ? XHud_Dashboard.Theme_Primary : Color.black);
                    state_displayer($"{(mod_isexist_mat_clean ? "已" : "未")}装配：材质清理器", status_dot, mod_isexist_mat_clean ? XHud_Dashboard.Theme_Primary : Color.black);
                    break;
                case 1:
                    state_displayer($"{(mgr.Hud_Colors != null ? "已" : "未")}装配：色卡库", status_dot, mgr.Hud_Colors != null ? XHud_Dashboard.Theme_Primary : Color.black);
                    state_displayer($"{(mgr.Hud_Curves != null ? "已" : "未")}装配：曲线库", status_dot, mgr.Hud_Curves != null ? XHud_Dashboard.Theme_Primary : Color.black);
                    state_displayer($"{(mgr.Hud_Sounds != null ? "已" : "未")}装配：音效库", status_dot, mgr.Hud_Sounds != null ? XHud_Dashboard.Theme_Primary : Color.black);
                    state_displayer($"{(mgr.Hud_TextStyleLibrary != null ? "已" : "未")}装配：字体库", status_dot, mgr.Hud_TextStyleLibrary != null ? XHud_Dashboard.Theme_Primary : Color.black);
                    state_displayer($"{(mgr.Hud_Motions != null ? "已" : "未")}装配：动效库", status_dot, mgr.Hud_Motions != null ? XHud_Dashboard.Theme_Primary : Color.black);
                    state_displayer($"{(mgr.Hud_TransitionLib != null ? "已" : "未")}装配：转场库", status_dot, mgr.Hud_TransitionLib != null ? XHud_Dashboard.Theme_Primary : Color.black);

                    XGUI.layout_seperator(thickness: 1,
                   color: Color.white * 0.5f,
                   margin: new RectOffset(15, 15, 10, 10));

                    Rect list_root = XGUI.GetControlRect(false, position.height - 350 - 20);

                    //XGUI.gui_box(list_root, Color.red * 0.2f);

                    bool listoverflow = true;

                    if (list_root.height > 65 * mgr.Hud_ElementLibrarys.Count)
                        listoverflow = false;

                    list_scrollPosition = XGUI.gui_scrollview<XHud_Library_Element>(
                        rect: new Rect(list_root.x, list_root.y, list_root.width - 3, list_root.height),
                        scroll: list_scrollPosition,
                        list: mgr.Hud_ElementLibrarys,
                        cachedItemHeights: ref list_cachedItemHeights,
                        heightsDirty: ref list_heightsDirty,
                        fixheight: 65,
                        onscroller: (rect, type, index) =>
                        {
                            Rect rect_content = new Rect(rect.x, rect.y, listoverflow ? rect.width : rect.width + 17, rect.height - 1);
                            #region 列表项

                            Rect rect_bg = new Rect(rect_content.x, rect_content.y, rect_content.width, rect_content.height);

                            #region 背景
                            XGUI.gui_box(
                                rect: rect_bg,
                                bg: XGUI.GetFillTexture(XGUIFilled.实体, XGUIColor.亮白),
                                bg_color_gui: Color.black * 0.33f,
                                offset: Vector2.zero,
                                border: new RectOffset(10, 10, 10, 10),
                                margin: new RectOffset(0, 0, 0, 0));
                            #endregion

                            #region 图标
                            XGUI.gui_box(
                                rect: new Rect(rect_bg.x + 8, rect_bg.y + 8, status_dot.width / 2, status_dot.height / 2),
                                bg: status_dot,
                                bg_color_gui: Color.white * 0.8f,
                                offset: Vector2.zero,
                                margin: new RectOffset(0, 0, 0, 0));
                            #endregion

                            #region 名称
                            XGUI.gui_label(
                                rect: new Rect(rect_bg.x + 18, rect_bg.y + 8, rect_bg.width, XGUI.GetSingleLineHeight()),
                                text: new GUIContent($"{type.LibraryName}"),
                                text_color: Color.white,
                                size: XGUIFontSize.M,
                                anchor: TextAnchor.MiddleLeft,
                                clipping: TextClipping.Ellipsis);
                            #endregion                          

                            #region 数量
                            XGUI.gui_label(
                                rect: new Rect(rect_bg.x + 18, rect_bg.y + 32, rect_bg.width, XGUI.GetSingleLineHeight()),
                                text: new GUIContent($"<color=#808080>元素数量：</color><color=#{HexPrimaryColor}>{type.ElementsLibrary_GetElementsCount()}</color>  个"),
                                text_color: Color.white,
                                size: XGUIFontSize.S,
                                anchor: TextAnchor.MiddleLeft,
                                clipping: TextClipping.Ellipsis);
                            #endregion

                            #region 打开
                            if (XGUI.gui_button(
                                rect: new Rect(rect_bg.x + (rect_bg.width - 70), rect_bg.y + (rect_bg.height / 2 - (XGUI.GetSingleLineHeight() / 2)), 60, 22),
                                text: "查看",
                                tooltip: "查看元素库",
                                btn_fill: XGUIFilled.实体,
                                btn_color: XGUIColor.亮白,
                                btn_color_gui: Color.white * 0.9f,
                                btn_text_color: Color.black,
                                press_fill: XGUIFilled.实体,
                                press_color: XGUIColor.深空灰,
                                press_text_color: Color.white,
                                font_size: XGUIFontSize.S,
                                margin: new RectOffset(0, 0, 0, 0),
                                padding: new RectOffset(0, 0, 0, 0)))
                            {
                                Editor_XHud_MenuItemsAction_OpenLibrary.open_target_elements(type.LibraryName);
                            }

                            #endregion   
                            #endregion
                        });

                    //Repaint();
                    break;
                case 2:
                    #region 分辨率参数概览
                    float originalWidth = mgr.ScreenRes.x;
                    float originalHeight = mgr.ScreenRes.y;

                    #region 计算等比缩放
                    float targetWidth = position.width - 28;
                    float targetHeight = 140;

                    // 计算原始宽高比和目标宽高比
                    float originalRatio = originalWidth / originalHeight;
                    float targetRatio = targetWidth / targetHeight;

                    float calc_Width, calc_Height;

                    // 判断是宽屏还是竖屏
                    if (originalRatio > targetRatio)
                    {
                        // 原始图像比目标更宽（宽屏），以宽度为准
                        float widthScale = targetWidth / originalWidth;
                        calc_Width = targetWidth;
                        calc_Height = originalHeight * widthScale;
                    }
                    else
                    {
                        // 原始图像比目标更高（竖屏），以高度为准
                        float heightScale = targetHeight / originalHeight;
                        calc_Width = originalWidth * heightScale;
                        calc_Height = targetHeight;
                    }
                    #endregion

                    string hexcol = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary, true);

                    #region 屏幕分辨率信息可视化
                    XGUI.layout_space(25);

                    Rect res = XGUI.GetControlRect(false, calc_Height);

                    XGUI.gui_label(
                        rect: new Rect(res.x + 1, res.y - 25, res.width - 15, XGUI.GetSingleLineHeight()),
                        text: new GUIContent($"当前分辨率：<color={hexcol}>{mgr.ScreenRes.x} x {mgr.ScreenRes.y}</color>"),
                        text_color: Color.white,
                        size: XGUIFontSize.M,
                        clipping: TextClipping.Overflow,
                        anchor: TextAnchor.MiddleLeft,
                        offset: new Vector2(0, 0),
                        wrap: true,
                        font_style: FontStyle.Normal);

                    res.Set(res.x, res.y, calc_Width, calc_Height);
                    XGUI.gui_icon(
                        rect: new Rect(res.x, res.y, targetWidth, targetHeight),
                        icon: sizebg,
                        color: Color.white * 0.12f);

                    XGUI.gui_icon(
                        rect: res,
                        border: new RectOffset(15, 15, 15, 15),
                        icon: XGUI.GetFillTexture(XGUIFilled.纯色边框, XGUIColor.亮白),
                        color: Color.gray);

                    XGUI.gui_label(
                      rect: new Rect(res.x + res.width / 2 - 50, res.y + res.height / 2 - 10, 100, XGUI.GetSingleLineHeight()),
                      text: new GUIContent($"宽高比：{(mgr.ScreenRes.x / mgr.ScreenRes.y).ToString("F2")}"),
                      text_color: XHud_Dashboard.Theme_Primary,
                      size: XGUIFontSize.M,
                      clipping: TextClipping.Overflow,
                      anchor: TextAnchor.MiddleCenter,
                      offset: new Vector2(0, 0),
                      wrap: true,
                      font_style: FontStyle.Normal);
                    #endregion
                    #endregion

                    #region 相机参数概览
                    XGUI.layout_space(50);

                    Rect cams = XGUI.GetControlRect(false, calc_Height);

                    XGUI.gui_icon(
                      rect: new Rect(cams.x, cams.y, targetWidth, targetHeight + 15),
                      icon: sizebg,
                      color: Color.white * 0.12f);

                    if (mgr.CameraOthograpicMode)
                        XGUI.gui_icon(
                            rect: new Rect(cams.x, cams.y, 300, 180),
                            icon: orthorgrphic,
                            color: Color.white * 0.7f);
                    else
                        XGUI.gui_icon(
                            rect: new Rect(cams.x, cams.y, 300, 180),
                            icon: perspective,
                            color: Color.white * 0.7f);

                    if (!mgr.CameraOthograpicMode)
                        XGUI.gui_label(
                            rect: new Rect(cams.x + 1, cams.y - 26, 80, XGUI.GetSingleLineHeight()),
                            text: new GUIContent($"视场角：<color={hexcol}>{mgr.CameraFov}</color> °"),
                            text_color: Color.white,
                            size: XGUIFontSize.M,
                            clipping: TextClipping.Overflow,
                            anchor: TextAnchor.MiddleLeft,
                            offset: new Vector2(0, 0),
                            wrap: true,
                            font_style: FontStyle.Normal);
                    else
                        XGUI.gui_label(
                            rect: new Rect(cams.x + 1, cams.y - 26, 80, XGUI.GetSingleLineHeight()),
                            text: new GUIContent($"视场大小：<color={hexcol}>{mgr.CameraOrthographicSize}</color> °"),
                            text_color: Color.white,
                            size: XGUIFontSize.M,
                            clipping: TextClipping.Overflow,
                            anchor: TextAnchor.MiddleLeft,
                            offset: new Vector2(0, 0),
                            wrap: true,
                            font_style: FontStyle.Normal);

                    XGUI.gui_label(
                        rect: new Rect(cams.x + cams.width - 120, cams.y + cams.height - 25, 100, 18),
                        text: new GUIContent($"远距：<color={hexcol}>{mgr.CameraCutter_Far}</color>"),
                        text_color: Color.white,
                        size: XGUIFontSize.S,
                        clipping: TextClipping.Overflow,
                        anchor: TextAnchor.MiddleRight,
                        offset: new Vector2(0, 0),
                        wrap: true,
                        font_style: FontStyle.Normal);

                    XGUI.gui_label(
                        rect: new Rect(cams.x + 25, cams.y + cams.height - 25, 100, 18),
                        text: new GUIContent($"近距：<color={hexcol}>{mgr.CameraCutter_Near}</color>"),
                        text_color: Color.white,
                        size: XGUIFontSize.S,
                        clipping: TextClipping.Overflow,
                        anchor: TextAnchor.MiddleLeft,
                        offset: new Vector2(0, 0),
                        wrap: true,
                        font_style: FontStyle.Normal);

                    XGUI.gui_label(
                        rect: new Rect(cams.x + 156, cams.y + 60, 100, 18),
                        text: new GUIContent($"画布距离：<color={hexcol}>{mgr.CanvasDistance}</color>"),
                        text_color: Color.white,
                        size: XGUIFontSize.S,
                        clipping: TextClipping.Overflow,
                        anchor: TextAnchor.MiddleLeft,
                        offset: new Vector2(0, 0),
                        wrap: true,
                        font_style: FontStyle.Normal);
                    #endregion
                    break;
                case 3:
                    XHudElementsStatistic statistics = mgr.hm_Element_GetStatistic();

                    state_displayer(icon_element, "元素", statistics.count_elements.ToString(), XHud_Dashboard.Theme_Primary);
                    state_displayer(icon_primitive, "图元", statistics.count_primitives.ToString(), XHud_Dashboard.Theme_Primary);
                    state_displayer(icon_sounder, "音效", statistics.count_sounders.ToString(), XHud_Dashboard.Theme_Primary);
                    state_displayer(icon_option, "选项", statistics.count_options.ToString(), XHud_Dashboard.Theme_Primary);
                    state_displayer(icon_button, "按钮", statistics.count_buttons.ToString(), XHud_Dashboard.Theme_Primary);
                    state_displayer(icon_toggle, "开关", statistics.count_toggles.ToString(), XHud_Dashboard.Theme_Primary);
                    state_displayer(icon_progress, "进度条", statistics.count_progresses.ToString(), XHud_Dashboard.Theme_Primary);
                    state_displayer(icon_slider, "滑动条", statistics.count_sliders.ToString(), XHud_Dashboard.Theme_Primary);
                    state_displayer(icon_text, "文字", statistics.count_texts.ToString(), XHud_Dashboard.Theme_Primary);
                    state_displayer(icon_tmptext, "TMP 文字", statistics.count_tmptexts.ToString(), XHud_Dashboard.Theme_Primary);
                    break;
            }

            XGUI.layout_group_end(XGUIContainerType.Vertical);

            Event e = Event.current;
            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
            {
                e.Use();
                Close();
            }
        }

        #region Draw
        public void state_displayer(string title, Texture2D icon, Color col_icon)
        {
            XGUI.layout_state_displayer_icon(
                title: title,
                title_size: XGUIFontSize.M,
                icon: icon,
                icon_size: new Vector2(12, 12),
                icon_offset: new Vector2(0, 0),
                icon_color: col_icon,
                padding: new RectOffset(0, 0, 0, 0),
                margin: new RectOffset(0, 0, 0, 5));
        }
        public void state_displayer(Texture2D icon, string title, string content, Color col_icon)
        {
            XGUI.layout_group_start(
                type: XGUIContainerType.Horizontal,
                bg_fill: XGUIFilled.无,
                bg_color: XGUIColor.无,
                bg_color_gui: XHud_Dashboard.Theme_Group,
                title: "",
                title_size: XGUIFontSize.M,
                title_text_color: Color.white,
                title_clipping: TextClipping.Overflow,
                absolute_padding: true,
                absolute_margin: true,
                margin: new RectOffset(0, 0, 0, 10),
                padding: new RectOffset(10, 0, 0, 0));

            XGUI.layout_icon(
                icon: icon,
                width: 12,
                height: 12,
                icon_offset: new Vector2(0, 4),
                icon_color: Color.white * 0.7f,
                icon_margin: new RectOffset(10, 0, 0, 0),
                icon_alignment: XGUIIconAlignment.默认);

            XGUI.layout_state_displayer_text(
                title: title,
                title_color: Color.white,
                title_size: XGUIFontSize.M,
                subtitle: content,
                subtitle_size: XGUIFontSize.M,
                subtitle_color: col_icon,
                margin: new RectOffset(5, 10, 0, 5)); XGUI.layout_group_end(XGUIContainerType.Horizontal);
        }

        #endregion

        #region 辅助
        [DidReloadScripts]
        public static void CheckToolsExists()
        {
            string path = Application.dataPath + "/SevenStrikeModules/XHud/Editor_XHud_BlurMask.cs";
            mod_isexist_blurmask = File.Exists(path);

            string mod_isexist_softmask_path = Application.dataPath + "/SevenStrikeModules/XHud/ThirdPlugin/SoftMask/Scripts/SoftMask.cs";
            mod_isexist_softmask = File.Exists(mod_isexist_softmask_path);

            string mod_isexist_psd_path = Application.dataPath + "/SevenStrikeModules/XHud/Scripts/Editor/PSDReconstruction/Editor_XHud_PSDReconstruction.cs";
            mod_isexist_psd = File.Exists(mod_isexist_psd_path);

            string mod_isexist_color_capture_path = Application.dataPath + "/SevenStrikeModules/XHud/Scripts/Editor/Tool/Editor_XHud_Tool_ColorCapture.cs";
            mod_isexist_color_capture = File.Exists(mod_isexist_color_capture_path);

            string mod_isexist_gradient_path = Application.dataPath + "/SevenStrikeModules/XHud/Scripts/Editor/Tool/Editor_XHud_Tool_GradientColor.cs";
            mod_isexist_gradient = File.Exists(mod_isexist_gradient_path);

            string mod_isexist_rotate_correct_path = Application.dataPath + "/SevenStrikeModules/XHud/Scripts/Editor/Tool/Editor_XHud_Tool_PrimitivesRotatorPivotCorrection.cs";
            mod_isexist_rotate_correct = File.Exists(mod_isexist_rotate_correct_path);

            string mod_isexist_mod_create_path = Application.dataPath + "/SevenStrikeModules/XHud/Scripts/Editor/Tool/Editor_XHud_Tool_RandomBlockCreator.cs";
            mod_isexist_mod_create = File.Exists(mod_isexist_mod_create_path);

            string mod_isexist_mat_clean_path = Application.dataPath + "/SevenStrikeModules/XHud/Scripts/Editor/Tool/Editor_XHud_Tool_MaterialPropertieCleaner.cs";
            mod_isexist_mat_clean = File.Exists(mod_isexist_mat_clean_path);
        }
        #endregion
    }
}