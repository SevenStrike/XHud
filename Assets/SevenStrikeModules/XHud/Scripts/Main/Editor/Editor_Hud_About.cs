namespace SevenStrikeModules.XHud
{
    using Newtonsoft.Json;
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.GuiLib;
    using System.Diagnostics;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// XHUD开发信息
    /// </summary>
    public class XHudDevInfos
    {
        /// <summary>
        /// 
        /// </summary>
        public string ver { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string sub { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string intro { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string website { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string document { get; set; }
    }

    public class Editor_Hud_About : EditorWindow
    {
        static Editor_Hud_About Window;

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

        /// <summary>
        /// 版本
        /// </summary>
        string Ver;
        /// <summary>
        /// 副标题
        /// </summary>
        string Sub;
        /// <summary>
        /// 信息
        /// </summary>
        string Infos;

        public XHudDevInfos HudDevInfos;

        [MenuItem("Tools/XHud/About #a")]
        static void Init()
        {
            Window = (Editor_Hud_About)EditorWindow.GetWindow(typeof(Editor_Hud_About), true, "关于XHUD的信息", true);
            Window.minSize = new Vector2(350, 310);
            Window.maxSize = Window.minSize;
            Window.Show();
        }

        private void OnEnable()
        {
            #region 读取Json数据
            Infos = AssetDatabase.LoadAssetAtPath<TextAsset>(util_Dashboard.Get_GUIRoot_Path() + "/XHudDevsInfo.json").text;
            #endregion

            #region 解析Json类
            if (HudDevInfos == null)
                HudDevInfos = new XHudDevInfos();
            HudDevInfos = JsonConvert.DeserializeObject<XHudDevInfos>(Infos);
            #endregion

            #region 指定字体
            Font_Bold = util_XHUDGUI.GetFont("SS_Editor_Bold");
            Font_Thin = util_XHUDGUI.GetFont("SS_Editor_Thin");
            #endregion

            Ver = HudDevInfos.ver;
            Sub = HudDevInfos.sub;
        }

        private void OnGUI()
        {
            util_XHUDGUI.Gui_Layout_Vertical_Start(HudFilled.无, HudColor.无);
            util_XHUDGUI.Gui_Layout_Space(10);

            #region LOGO
            util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无);
            util_XHUDGUI.Gui_Layout_Space(5);
            GUILayout.FlexibleSpace();

            util_XHUDGUI.Gui_Layout_Labelfield("X  H  U  D", HudFilled.无, HudColor.亮白, Color.white, TextAnchor.MiddleRight, new Vector2(0, 15), 35, Font_Thin);

            Rect rect_logo = GUILayoutUtility.GetLastRect();
            GUILayout.FlexibleSpace();
            util_XHUDGUI.Gui_Layout_Space(5);
            util_XHUDGUI.Gui_Layout_Horizontal_End();
            util_XHUDGUI.Gui_Labelfield_Thin(new Rect(rect_logo.x + 64, rect_logo.height + 60, 60, 20), Ver, HudFilled.无, HudColor.亮白, util_Dashboard.Theme_Primary, TextAnchor.MiddleRight, new Vector2(0, 0), 11);
            util_XHUDGUI.Gui_Labelfield_Thin(new Rect(rect_logo.x, rect_logo.height + 60, 60, 20), Sub, HudFilled.无, HudColor.亮白, Color.gray, TextAnchor.MiddleLeft, new Vector2(0, 0), 11);
            #endregion

            util_XHUDGUI.Gui_Layout_Space(80);

            util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无);
            util_XHUDGUI.Gui_Layout_Space(10);
            util_XHUDGUI.Gui_Layout_Labelfield("此插件由 （南京塞维斯传媒）SevenStrikeMedia 独家开发", HudFilled.无, HudColor.无, util_XHUDGUI.GetColor(HudColor.阴影灰), TextAnchor.MiddleCenter, 12);
            util_XHUDGUI.Gui_Layout_Space(10);
            util_XHUDGUI.Gui_Layout_Horizontal_End();

            util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无);
            util_XHUDGUI.Gui_Layout_Space(15);
            util_XHUDGUI.Gui_Layout_TextArea_Wrap(HudDevInfos.intro, HudFilled.无, HudColor.无, new Color(1, 1, 1, 0.65f), TextAnchor.UpperLeft, 330, 12);
            util_XHUDGUI.Gui_Layout_Space(15);
            util_XHUDGUI.Gui_Layout_Horizontal_End();

            util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无);
            util_XHUDGUI.Gui_Layout_Space(15);
            if (util_XHUDGUI.Gui_Layout_Button("访问XHud官网", "", HudFilled.实体, HudColor.深空灰, Color.white, 35, new RectOffset(), new Vector2(0, 0)))
            {
                OpenURL(HudDevInfos.website);
            }
            util_XHUDGUI.Gui_Layout_Space(10);
            if (util_XHUDGUI.Gui_Layout_Button("教程手册", "", HudFilled.实体, HudColor.深空灰, Color.white, 35, new RectOffset(), new Vector2(0, 0)))
            {
                OpenURL(HudDevInfos.document);
            }
            util_XHUDGUI.Gui_Layout_Space(15);
            util_XHUDGUI.Gui_Layout_Horizontal_End();

            util_XHUDGUI.Gui_Layout_Space(15);
            util_XHUDGUI.Gui_Layout_Vertical_End();
        }

        /// <summary>
        /// 打开网址
        /// </summary>
        /// <param name="url"></param>
        private void OpenURL(string url)
        {
            // 使用Process.Start打开URL
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
    }
}
