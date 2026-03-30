namespace SevenStrikeModules.XHud.Utilitys
{
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.GuiLib;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    public class Editor_XHud_Tool_Diagnostic : EditorWindow
    {
        private SerializedObject BaseObject;

        private static Editor_XHud_Tool_Diagnostic window;

        [SerializeField]
        public List<Color> ColorGradient = new List<Color>();

        private Texture2D logo, dot, icon_element, icon_animator, icon_sounder, icon_container, icon_button, icon_toggle, icon_progress, icon_slider, icon_option, icon_text, icon_tmptext, sizemark_h_r, sizemark_h_l, sizemark_v_u, sizemark_v_d, sizebg, perspective, orthorgrphic, cursobg, falseicon, systemcursor;

        Color SepLineColor = new Color(1, 1, 1, 0.15f);
        Color MessageColor = new Color(1, 1, 1, 0.62f);

        Rect Sepline_rect;
        Rect Title_rect;
        Rect Icon_rect;

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
        /// 字体 - 粗体
        /// </summary>
        Font Font_Bold;
        /// <summary>
        /// 字体 - 细体
        /// </summary>
        Font Font_Light;

        [MenuItem("Tools/XHud/Diagnostic ^#d")]
        static void Init()
        {
            if (XHud_Dashboard.HudManagerGet() == null)
                return;
            window = (Editor_XHud_Tool_Diagnostic)EditorWindow.GetWindow(typeof(Editor_XHud_Tool_Diagnostic), true, "XHUD 诊断/概况面板", true);
            Editor_XHud_GUI.CenterEditorWindow(new Vector2Int(1000, 765), window);
            window.Show();
        }

        private void OnDisable()
        {

        }

        private void OnEnable()
        {
            BaseObject = new SerializedObject(this);

            logo = Editor_XHud_GUI.GetIcon("Icons_XHudDiagnostic/logo");
            dot = Editor_XHud_GUI.GetIcon("Icons_XHudDiagnostic/dot");
            icon_element = Editor_XHud_GUI.GetIcon("Icons_XHudDiagnostic/icon_element");
            icon_animator = Editor_XHud_GUI.GetIcon("Icons_XHudDiagnostic/icon_animator");
            icon_sounder = Editor_XHud_GUI.GetIcon("Icons_XHudDiagnostic/icon_sounder");
            icon_container = Editor_XHud_GUI.GetIcon("Icons_XHudDiagnostic/icon_container");
            icon_button = Editor_XHud_GUI.GetIcon("Icons_XHudDiagnostic/icon_button");
            icon_toggle = Editor_XHud_GUI.GetIcon("Icons_XHudDiagnostic/icon_toggle");
            icon_progress = Editor_XHud_GUI.GetIcon("Icons_XHudDiagnostic/icon_progress");
            icon_slider = Editor_XHud_GUI.GetIcon("Icons_XHudDiagnostic/icon_slider");
            icon_option = Editor_XHud_GUI.GetIcon("Icons_XHudDiagnostic/icon_option");
            icon_text = Editor_XHud_GUI.GetIcon("Icons_XHudDiagnostic/icon_text");
            icon_tmptext = Editor_XHud_GUI.GetIcon("Icons_XHudDiagnostic/icon_tmptext");
            sizemark_h_r = Editor_XHud_GUI.GetIcon("Icons_XHudDiagnostic/sizemark_h_r");
            sizemark_h_l = Editor_XHud_GUI.GetIcon("Icons_XHudDiagnostic/sizemark_h_l");
            sizemark_v_u = Editor_XHud_GUI.GetIcon("Icons_XHudDiagnostic/sizemark_v_u");
            sizemark_v_d = Editor_XHud_GUI.GetIcon("Icons_XHudDiagnostic/sizemark_v_d");
            sizebg = Editor_XHud_GUI.GetIcon("Icons_XHudDiagnostic/sizebg");
            perspective = Editor_XHud_GUI.GetIcon("Icons_XHudDiagnostic/perspective");
            orthorgrphic = Editor_XHud_GUI.GetIcon("Icons_XHudDiagnostic/orthorgrphic");
            cursobg = Editor_XHud_GUI.GetIcon("Icons_XHudDiagnostic/cursobg");
            falseicon = Editor_XHud_GUI.GetIcon("Icons_XHudDiagnostic/falseicon");
            systemcursor = Editor_XHud_GUI.GetIcon("Icons_XHudDiagnostic/systemcursor");

            Font_Bold = Editor_XHud_GUI.GetFont("SS_Editor_Bold");
            Font_Light = Editor_XHud_GUI.GetFont("SS_Editor_Light");
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

            Icon_rect = new Rect(15, 15, 48, 48);

            Editor_XHud_GUI.Gui_Icon(Icon_rect, logo);

            Title_rect = new Rect(rect.x + 85, rect.y + 15, rect.width - 80, 30);
            Editor_XHud_GUI.Gui_Labelfield(Title_rect, "XHUD 诊断/概况面板", HudFilled.无, HudColor.无, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 20, Font_Bold);

            Sepline_rect = new Rect(rect.x + 85, rect.y + 60, 200, 1);
            Editor_XHud_GUI.Gui_Box(Sepline_rect, SepLineColor);

            Editor_XHud_GUI.Gui_Labelfield_Thin_WrapClip(new Rect(rect.x + 18, rect.y + 80, rect.width - 38, rect.height), "此工具可根据当前的XHUD的配置状态来进行诊断，确保您正确配置XHUD！同时也可在此查看组件使用状态等杂项信息", HudFilled.无, HudColor.无, MessageColor, TextAnchor.UpperLeft, new Vector2(0, 0), 12, true, Font_Light);
            #endregion

            Editor_XHud_GUI.Gui_Layout_Space(140);

            Rect re = Editor_XHud_GUI.Gui_GetLastRect();

            Color err = Color.red;
            Color war = new Color(1, 0.6627f, 0);
            Color nor = XHud_Dashboard.Theme_Primary;

            #region 基础信息 / 框架状态检测
            GUILayout.BeginArea(new Rect(re.position.x + 15, re.position.y + 140, 170, 600));

            #region 组件状态检测

            #region 管理器的创建            
            if (mgr == null)
                Editor_XHud_GUI.StatuDisplayer_icon(null, 12, new Vector2(0, 7), "XHud管理器未创建", 12, err, dot, 12, new Vector2(0, 4), false);
            else
                Editor_XHud_GUI.StatuDisplayer_icon(null, 12, new Vector2(0, 7), "XHud管理器已创建", 12, nor, dot, 12, new Vector2(0, 4), false);
            #endregion

            #region 分辨率设置
            if (mgr != null)
            {
                if (mgr.ScreenRes == Vector2.zero)
                    Editor_XHud_GUI.StatuDisplayer_icon(null, 12, new Vector2(0, 7), "未指定目标分辨率", 12, err, dot, 12, new Vector2(0, 4), false);
                else
                    Editor_XHud_GUI.StatuDisplayer_icon(null, 12, new Vector2(0, 7), "目标分辨率已设定", 12, nor, dot, 12, new Vector2(0, 4), false);
            }
            #endregion

            #region 场景相机
            if (mgr != null)
            {
                if (mgr.SceneCamera == null)
                    Editor_XHud_GUI.StatuDisplayer_icon(null, 12, new Vector2(0, 7), "未指定场景相机", 12, err, dot, 12, new Vector2(0, 4), false);
                else
                    Editor_XHud_GUI.StatuDisplayer_icon(null, 12, new Vector2(0, 7), "场景相机已指定", 12, nor, dot, 12, new Vector2(0, 4), false);
            }
            #endregion

            #region 资源库 - 色卡库
            if (mgr != null)
            {
                if (mgr.Hud_Colors == null)
                    Editor_XHud_GUI.StatuDisplayer_icon(null, 12, new Vector2(0, 7), "色卡库   -   未部署", 12, war, dot, 12, new Vector2(0, 4), false);
                else
                    Editor_XHud_GUI.StatuDisplayer_icon(null, 12, new Vector2(0, 7), "色卡库   -   已部署", 12, nor, dot, 12, new Vector2(0, 4), false);
            }
            #endregion

            #region 资源库 - 曲线库
            if (mgr != null)
            {
                if (mgr.Hud_Curves == null)
                    Editor_XHud_GUI.StatuDisplayer_icon(null, 12, new Vector2(0, 7), "曲线库   -   未部署", 12, war, dot, 12, new Vector2(0, 4), false);
                else
                    Editor_XHud_GUI.StatuDisplayer_icon(null, 12, new Vector2(0, 7), "曲线库   -   已部署", 12, nor, dot, 12, new Vector2(0, 4), false);
            }
            #endregion

            #region 资源库 - 音效库
            if (mgr != null)
            {
                if (mgr.Hud_Sounds == null)
                    Editor_XHud_GUI.StatuDisplayer_icon(null, 12, new Vector2(0, 7), "音效库   -   未部署", 12, war, dot, 12, new Vector2(0, 4), false);
                else
                    Editor_XHud_GUI.StatuDisplayer_icon(null, 12, new Vector2(0, 7), "音效库   -   已部署", 12, nor, dot, 12, new Vector2(0, 4), false);
            }
            #endregion

            #region 资源库 - 字体库
            if (mgr != null)
            {
                if (mgr.Hud_TextStyleLibrary == null)
                    Editor_XHud_GUI.StatuDisplayer_icon(null, 12, new Vector2(0, 7), "字体库   -   未部署", 12, war, dot, 12, new Vector2(0, 4), false);
                else
                    Editor_XHud_GUI.StatuDisplayer_icon(null, 12, new Vector2(0, 7), "字体库   -   已部署", 12, nor, dot, 12, new Vector2(0, 4), false);
            }
            #endregion

            #region 资源库 - 动效库
            if (mgr != null)
            {
                if (mgr.Hud_ElementMotion == null)
                    Editor_XHud_GUI.StatuDisplayer_icon(null, 12, new Vector2(0, 7), "动效库   -   未部署", 12, war, dot, 12, new Vector2(0, 4), false);
                else
                    Editor_XHud_GUI.StatuDisplayer_icon(null, 12, new Vector2(0, 7), "动效库   -   已部署", 12, nor, dot, 12, new Vector2(0, 4), false);
            }
            #endregion

            #region 资源库 - 转场库
            if (mgr != null)
            {
                if (mgr.Hud_TransitionLib == null)
                    Editor_XHud_GUI.StatuDisplayer_icon(null, 12, new Vector2(0, 7), "转场库   -   未部署", 12, war, dot, 12, new Vector2(0, 4), false);
                else
                    Editor_XHud_GUI.StatuDisplayer_icon(null, 12, new Vector2(0, 7), "转场库   -   已部署", 12, nor, dot, 12, new Vector2(0, 4), false);
            }
            #endregion

            #region 资源库 - 元素库
            if (mgr != null)
            {
                if (mgr.Hud_ElementLibrarys == null || mgr.Hud_ElementLibrarys.Count <= 0)
                    Editor_XHud_GUI.StatuDisplayer_icon(null, 12, new Vector2(0, 7), "元素库   -   未部署", 12, war, dot, 12, new Vector2(0, 4), false);
                else
                    Editor_XHud_GUI.StatuDisplayer_icon(null, 12, new Vector2(0, 7), "元素库   -   已部署", 12, nor, dot, 12, new Vector2(0, 4), false);
            }
            #endregion

            #endregion

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Seperator(1, XHud_Dashboard.Theme_SeperateLine);

            #region 库统计
            int count_col = 0;
            if (XHud_Dashboard.HudManagerGet().Hud_Colors != null)
                count_col = XHud_Dashboard.HudManagerGet().Hud_Colors.ColorLibrary.Count;
            int count_cur = 0;
            if (XHud_Dashboard.HudManagerGet().Hud_Colors != null)
                count_cur = XHud_Dashboard.HudManagerGet().Hud_Curves.CurveLibrary.Count;
            int count_sod = 0;
            if (XHud_Dashboard.HudManagerGet().Hud_Colors != null)
                count_sod = XHud_Dashboard.HudManagerGet().Hud_Sounds.SoundLibrary.Count;
            int count_fontstyle = 0;
            if (XHud_Dashboard.HudManagerGet().Hud_Colors != null)
                count_fontstyle = XHud_Dashboard.HudManagerGet().Hud_TextStyleLibrary.TextStyleLibrary.Count;
            int count_motion = 0;
            if (XHud_Dashboard.HudManagerGet().Hud_Colors != null)
                count_motion = XHud_Dashboard.HudManagerGet().Hud_ElementMotion.ElementMotionList.Count;
            int count_transition = 0;
            if (XHud_Dashboard.HudManagerGet().Hud_Colors != null)
                count_transition = XHud_Dashboard.HudManagerGet().Hud_TransitionLib.TransitionLibrary.Count;
            int count_elelibs = 0;
            if (XHud_Dashboard.HudManagerGet().Hud_Colors != null)
                count_elelibs = XHud_Dashboard.HudManagerGet().Hud_ElementLibrarys.Count;
            int count_sodlist = 0;
            if (XHud_Dashboard.HudManagerGet().Hud_Colors != null)
                count_sodlist = XHud_Dashboard.HudManagerGet().Pool_Sounder.Length;

            Editor_XHud_GUI.StatuDisplayer_text(null, 12, new Vector2(0, 8), "色卡库", 12, $"{count_col}", XHud_Dashboard.Theme_Primary, 11, false);
            Editor_XHud_GUI.Gui_Layout_FlexSpace();
            Editor_XHud_GUI.StatuDisplayer_text(null, 12, new Vector2(0, 8), "曲线库", 12, $"{count_cur}", XHud_Dashboard.Theme_Primary, 11, false);
            Editor_XHud_GUI.Gui_Layout_FlexSpace();
            Editor_XHud_GUI.StatuDisplayer_text(null, 12, new Vector2(0, 8), "音效库", 12, $"{count_sod}", XHud_Dashboard.Theme_Primary, 11, false);
            Editor_XHud_GUI.Gui_Layout_FlexSpace();
            Editor_XHud_GUI.StatuDisplayer_text(null, 12, new Vector2(0, 8), "字体库", 12, $"{count_fontstyle}", XHud_Dashboard.Theme_Primary, 11, false);
            Editor_XHud_GUI.Gui_Layout_FlexSpace();
            Editor_XHud_GUI.StatuDisplayer_text(null, 12, new Vector2(0, 8), "动效库", 12, $"{count_motion}", XHud_Dashboard.Theme_Primary, 11, false);
            Editor_XHud_GUI.Gui_Layout_FlexSpace();
            Editor_XHud_GUI.StatuDisplayer_text(null, 12, new Vector2(0, 8), "转场库", 12, $"{count_transition}", XHud_Dashboard.Theme_Primary, 11, false);
            Editor_XHud_GUI.Gui_Layout_FlexSpace();
            Editor_XHud_GUI.StatuDisplayer_text(null, 12, new Vector2(0, 8), "音效池", 12, $"{count_sodlist}", XHud_Dashboard.Theme_Primary, 11, false);
            Editor_XHud_GUI.Gui_Layout_FlexSpace();
            Editor_XHud_GUI.StatuDisplayer_text(null, 12, new Vector2(0, 8), "元素库 （堆栈）", 12, $"{count_elelibs}", XHud_Dashboard.Theme_Primary, 11, false);
            #endregion

            GUILayout.EndArea();
            #endregion

            Editor_XHud_GUI.Gui_Box(new Rect(rect.x + 220, rect.y + 160, 1, 560), Color.gray * 0.75f);

            #region 正在使用的组件状态检测
            Editor_XHud_GUI.Gui_Labelfield(new Rect(re.x + 260, re.y + 140, 200, 20), "正在使用的组件数量统计（包含世界空间）", HudFilled.无, HudColor.无, Color.white * 0.85f, TextAnchor.MiddleLeft, 13, Font_Light);

            XHudComponentStatistic statistic = mgr.hm_GetXHudComponentsCount();

            float startpos = 260;
            float distance = 75;
            float distance_row_start = 170;
            float distance_row = 55;
            float areawidth = 120;

            ComponentStatistic(re, new Vector2(areawidth, 50), new Vector2(startpos, distance_row_start), icon_element, "元素", "Element", statistic.elements.ToString());
            ComponentStatistic(re, new Vector2(areawidth, 50), new Vector2(startpos + areawidth + distance, distance_row_start), icon_animator, "动画器", "Animator", statistic.animators.ToString());
            ComponentStatistic(re, new Vector2(areawidth, 50), new Vector2(startpos + (areawidth * 2) + (distance * 2), distance_row_start), icon_sounder, "音效器", "Sounder", statistic.sounders.ToString());
            ComponentStatistic(re, new Vector2(areawidth, 50), new Vector2(startpos + (areawidth * 3) + (distance * 3), distance_row_start), icon_container, "容器", "Container", statistic.containers.ToString());

            ComponentStatistic(re, new Vector2(areawidth, 50), new Vector2(startpos, distance_row_start + distance_row), icon_button, "按钮", "Button", statistic.buttons.ToString());
            ComponentStatistic(re, new Vector2(areawidth, 50), new Vector2(startpos + areawidth + distance, distance_row_start + distance_row), icon_toggle, "开关", "Toggle", statistic.toggles.ToString());
            ComponentStatistic(re, new Vector2(areawidth, 50), new Vector2(startpos + (areawidth * 2) + (distance * 2), distance_row_start + distance_row), icon_progress, "进度条", "Progress", statistic.elements.ToString());
            ComponentStatistic(re, new Vector2(areawidth, 50), new Vector2(startpos + (areawidth * 3) + (distance * 3), distance_row_start + distance_row), icon_slider, "滑动条", "Slider", statistic.animators.ToString());

            ComponentStatistic(re, new Vector2(areawidth, 50), new Vector2(startpos, distance_row_start + (distance_row * 2)), icon_option, "选项器", "Option", statistic.sounders.ToString());
            ComponentStatistic(re, new Vector2(areawidth, 50), new Vector2(startpos + areawidth + distance, distance_row_start + (distance_row * 2)), icon_text, "文字", "Text", statistic.containers.ToString());
            ComponentStatistic(re, new Vector2(areawidth, 50), new Vector2(startpos + (areawidth * 2) + (distance * 2), distance_row_start + (distance_row * 2)), icon_tmptext, "文字", "TmpText", statistic.buttons.ToString());
            #endregion

            Editor_XHud_GUI.Gui_Box(new Rect(rect.x + 260, rect.y + 343, 705, 1), Color.gray * 0.75f);

            #region 分辨率参数概览
            Editor_XHud_GUI.Gui_Labelfield(new Rect(re.x + 260, re.y + 362, 200, 20), "UI 分辨率 / 相机参数概览", HudFilled.无, HudColor.无, Color.white * 0.85f, TextAnchor.MiddleLeft, 13, Font_Light);

            float originalWidth = mgr.ScreenRes.x;
            float originalHeight = mgr.ScreenRes.y;

            #region 计算等比缩放
            float targetWidth = 300;
            float targetHeight = 180;

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

            #region 屏幕分辨率信息可视化
            Rect res = new Rect(re.x + 258, re.y + 425, calc_Width, calc_Height);

            Editor_XHud_GUI.Gui_Icon(new Rect(res.x, res.y, targetWidth, targetHeight), sizebg, Color.white * 0.12f);

            if (calc_Width >= 50)
            {
                Editor_XHud_GUI.Gui_Icon(new Rect(res.x + 2, res.y - 30, 50, sizemark_h_l.height), sizemark_h_l, new RectOffset(15, 4, 0, 0), Color.gray * 0.75f);
                Editor_XHud_GUI.Gui_Icon(new Rect(res.x + 2 + res.width - 50 - 2, res.y - 30, 50, sizemark_h_r.height), sizemark_h_r, new RectOffset(4, 15, 0, 0), Color.gray * 0.75f);
            }

            Editor_XHud_GUI.Gui_Labelfield(new Rect(res.x + res.width / 2 - 100, res.y - 30, 200, 20), $"宽：{mgr.ScreenRes.x}", HudFilled.无, HudColor.无, Color.white * 0.85f, TextAnchor.MiddleCenter, 12, Font_Light);

            if (calc_Height >= 30)
            {
                Editor_XHud_GUI.Gui_Icon(new Rect(res.x + targetWidth - sizemark_v_u.width + 25, res.y, sizemark_v_u.width, 30), sizemark_v_u, new RectOffset(0, 0, 15, 4), Color.gray * 0.75f);
                Editor_XHud_GUI.Gui_Icon(new Rect(res.x + targetWidth - sizemark_v_u.width + 25, res.y + res.height - 30, sizemark_v_d.width, 30), sizemark_v_d, new RectOffset(0, 0, 4, 15), Color.gray * 0.75f);
            }

            Editor_XHud_GUI.Gui_Labelfield(new Rect(res.x + targetWidth + 10, res.y + res.height / 2 - 10, 50, 20), $"高：{mgr.ScreenRes.y}", HudFilled.无, HudColor.无, Color.white * 0.85f, TextAnchor.MiddleLeft, 12, Font_Light);

            Editor_XHud_GUI.Gui_Icon(res, Editor_XHud_GUI.GetFillTexture(HudFilled.纯色边框, HudColor.亮白), new RectOffset(15, 15, 15, 15), Color.gray);

            Editor_XHud_GUI.Gui_Labelfield(new Rect(res.x + res.width / 2 - 50, res.y + res.height / 2 - 10, 100, 20), calc_Width < 40 || calc_Height < 30 ? $"{(mgr.ScreenRes.x / mgr.ScreenRes.y).ToString("F2")}" : $"宽高比：{(mgr.ScreenRes.x / mgr.ScreenRes.y).ToString("F2")}", HudFilled.无, HudColor.无, XHud_Dashboard.Theme_Primary, TextAnchor.MiddleCenter, 12, Font_Light);
            #endregion
            #endregion

            #region 相机参数概览
            Rect cams = new Rect(re.x + 662, re.y + 425, 300, 180);

            Editor_XHud_GUI.Gui_Icon(new Rect(cams.x, cams.y, 300, 180), sizebg, Color.white * 0.12f);

            string hexcol = XHud_Utilitys.Color_To_HexColor(XHud_Dashboard.Theme_Primary, true);

            if (mgr.CameraOthograpicMode)
                Editor_XHud_GUI.Gui_Icon(new Rect(cams.x, cams.y, 300, 180), orthorgrphic, Color.white * 0.7f);
            else
                Editor_XHud_GUI.Gui_Icon(new Rect(cams.x, cams.y, 300, 180), perspective, Color.white * 0.7f);
            if (!mgr.CameraOthograpicMode)
                Editor_XHud_GUI.Gui_Labelfield(new Rect(cams.x + 1, cams.y - 30, 80, 18), $"视场角：<color={hexcol}>{mgr.CameraFov}</color> °", HudFilled.无, HudColor.无, Color.white, TextAnchor.MiddleLeft, 12, Font_Light);
            else
                Editor_XHud_GUI.Gui_Labelfield(new Rect(cams.x + 1, cams.y - 30, 80, 18), $"视场大小：<color={hexcol}>{mgr.CameraOrthographicSize}</color> x", HudFilled.无, HudColor.无, Color.white, TextAnchor.MiddleLeft, 12, Font_Light);

            Editor_XHud_GUI.Gui_Labelfield(new Rect(cams.x + cams.width - 120, cams.y + cams.height - 25, 100, 18), $"远距：<color={hexcol}>{mgr.CameraCutter_Far}</color>", HudFilled.无, HudColor.无, Color.white, TextAnchor.MiddleRight, 12, Font_Light);
            Editor_XHud_GUI.Gui_Labelfield(new Rect(cams.x + 25, cams.y + cams.height - 25, 100, 18), $"近距：<color={hexcol}>{mgr.CameraCutter_Near}</color>", HudFilled.无, HudColor.无, Color.white, TextAnchor.MiddleLeft, 12, Font_Light);
            Editor_XHud_GUI.Gui_Labelfield(new Rect(cams.x + 156, cams.y + 60, 100, 18), $"画布距离：<color={hexcol}>{mgr.CanvasDistance}</color>", HudFilled.无, HudColor.无, Color.white, TextAnchor.MiddleLeft, 12, Font_Light);
            #endregion

            Editor_XHud_GUI.Gui_Box(new Rect(rect.x + 260, rect.y + 633, 705, 1), Color.gray * 0.75f);

            #region 当前使用的光标
            Rect curso = new Rect(re.x + 260, re.y + 670, 30, 30);

            Editor_XHud_GUI.Gui_Labelfield(new Rect(curso.x, curso.y, 100, 20), "正在使用的光标样式", HudFilled.无, HudColor.无, Color.white * 0.85f, TextAnchor.MiddleLeft, 13, Font_Light);
            Editor_XHud_GUI.Gui_Icon(new Rect(curso.x, curso.y + 35, 30, 30), cursobg, Color.white * 0.15f);
            if (mgr.Hud_MouseCursor != null && mgr.Hud_MouseCursor.UseCustomCursor)
            {
                if (mgr.Hud_MouseCursor.CursorImager != null && mgr.Hud_MouseCursor.CursorImager.sprite != null)
                {
                    Editor_XHud_GUI.Gui_Icon(new Rect(curso.x, curso.y + 35, 30, 30), mgr.Hud_MouseCursor.CursorImager.sprite.texture, Color.white);
                    Editor_XHud_GUI.Gui_Labelfield(new Rect(curso.x + 40, curso.y + 30, 100, 20), $"{mgr.Hud_MouseCursor.CursorImager.sprite.name}", HudFilled.无, HudColor.无, XHud_Dashboard.Theme_Primary, TextAnchor.MiddleLeft, 12, Font_Bold);
                    Editor_XHud_GUI.Gui_Labelfield(new Rect(curso.x + 40, curso.y + 50, 100, 20), $"{mgr.Hud_MouseCursor.CursorSize.ToString("F2")}", HudFilled.无, HudColor.无, Color.gray, TextAnchor.MiddleLeft, 11, Font_Bold);
                }
                else
                {
                    Editor_XHud_GUI.Gui_Icon(new Rect(curso.x, curso.y + 35, 30, 30), falseicon, Color.white);
                    Editor_XHud_GUI.Gui_Labelfield(new Rect(curso.x + 40, curso.y + 30, 100, 20), $"未选择", HudFilled.无, HudColor.无, XHud_Dashboard.Theme_Primary, TextAnchor.MiddleLeft, 12, Font_Bold);
                    Editor_XHud_GUI.Gui_Labelfield(new Rect(curso.x + 40, curso.y + 50, 100, 20), $"-", HudFilled.无, HudColor.无, Color.gray, TextAnchor.MiddleLeft, 11, Font_Bold);
                }
            }
            else
            {
                Editor_XHud_GUI.Gui_Icon(new Rect(curso.x + 6, curso.y + 40, 20, 20), systemcursor, Color.white);
                Editor_XHud_GUI.Gui_Labelfield(new Rect(curso.x + 40, curso.y + 30, 100, 20), $"跟随系统", HudFilled.无, HudColor.无, XHud_Dashboard.Theme_Primary, TextAnchor.MiddleLeft, 12, Font_Bold);
                Editor_XHud_GUI.Gui_Labelfield(new Rect(curso.x + 40, curso.y + 50, 100, 20), $"-", HudFilled.无, HudColor.无, Color.gray, TextAnchor.MiddleLeft, 11, Font_Bold);
            }
            #endregion

            Editor_XHud_GUI.Gui_Box(new Rect(rect.x + 420, rect.y + 680, 1, 60), Color.gray * 0.75f);

            #region 当前使用的转场
            Rect transition = new Rect(re.x + 460, re.y + 670, 30, 30);

            Editor_XHud_GUI.Gui_Labelfield(new Rect(transition.x, transition.y, 100, 20), "正在使用的转场", HudFilled.无, HudColor.无, Color.white * 0.85f, TextAnchor.MiddleLeft, 13, Font_Light);
            if (mgr.Hud_TransitionController != null && !string.IsNullOrEmpty(mgr.Hud_TransitionController.CurrentTransitionNode.Name))
            {
                Editor_XHud_GUI.Gui_Labelfield(new Rect(transition.x, transition.y + 30, 100, 20), $"{mgr.Hud_TransitionController.CurrentTransitionNode.Name}", HudFilled.无, HudColor.无, XHud_Dashboard.Theme_Primary, TextAnchor.MiddleLeft, 12, Font_Bold);
            }
            else
            {
                Editor_XHud_GUI.Gui_Labelfield(new Rect(transition.x, transition.y + 30, 100, 20), "未使用转场", HudFilled.无, HudColor.无, XHud_Dashboard.Theme_Primary, TextAnchor.MiddleLeft, 12, Font_Bold);
            }
            #endregion

            BaseObject.ApplyModifiedProperties();

            Event e = Event.current;
            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
            {
                e.Use();
                Close();
            }
        }

        private void ComponentStatistic(Rect re, Vector2 size, Vector2 bodyoffset, Texture2D icon, string title, string title_script, string value)
        {
            Rect x = new Rect(re.position.x + bodyoffset.x, re.position.y + bodyoffset.y, size.x, size.y);

            GUILayout.BeginArea(x);
            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无);
            Editor_XHud_GUI.Gui_Layout_Icon(20, icon, new Vector2(0, 12));
            Editor_XHud_GUI.Gui_Layout_Space(12);
            Editor_XHud_GUI.Gui_Layout_Labelfield(title, HudFilled.无, HudColor.无, Color.white, TextAnchor.MiddleLeft, 12, Font_Light);
            Editor_XHud_GUI.Gui_Layout_Horizontal_End();

            Editor_XHud_GUI.Gui_Layout_Labelfield(title_script, HudFilled.无, HudColor.无, Color.white * 0.65f, TextAnchor.MiddleLeft, new Vector2(32, -2), 11, Font_Light);
            GUILayout.EndArea();

            Rect s = new Rect(x.x + x.width - 80, x.y, 80, 50);
            Editor_XHud_GUI.Gui_Labelfield(s, value, HudFilled.无, HudColor.无, XHud_Dashboard.Theme_Primary, TextAnchor.MiddleRight, 15, Font_Light);
        }
    }
}