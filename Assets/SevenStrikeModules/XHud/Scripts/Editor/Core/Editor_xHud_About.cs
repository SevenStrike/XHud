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
    using SevenStrikeModules.XHud.Enums;
    using System;
    using System.Diagnostics;
    using UnityEditor;
    using UnityEngine;

    [Serializable]
    /// <summary>
    /// XHUD开发信息
    /// </summary>
    public class XHudDevInfos
    {
        [SerializeField]
        /// <summary>
        /// 
        /// </summary>
        public string ver;
        [SerializeField]
        /// <summary>
        /// 
        /// </summary>
        public string sub;
        [SerializeField]
        /// <summary>
        /// 
        /// </summary>
        public string intro;
        [SerializeField]
        /// <summary>
        /// 
        /// </summary>
        public string website;
        [SerializeField]
        /// <summary>
        /// 
        /// </summary>
        public string document;
    }

    public class Editor_XHud_About : EditorWindow
    {
        static Editor_XHud_About Window;

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
            Window = (Editor_XHud_About)EditorWindow.GetWindow(typeof(Editor_XHud_About), true, "关于XHUD的信息", true);
            Window.minSize = new Vector2(350, 310);
            Window.maxSize = Window.minSize;
            Window.Show();
        }

        private void OnEnable()
        {
            #region 读取Json数据
            Infos = AssetDatabase.LoadAssetAtPath<TextAsset>(XHud_Dashboard.Get_Path_XHUD_CONFIG_Path() + "/XHudDevsInfo.json").text;
            #endregion
            //Debug.Log(Infos);
            #region 解析Json类
            if (HudDevInfos == null)
                HudDevInfos = new XHudDevInfos();
            HudDevInfos = JsonUtility.FromJson<XHudDevInfos>(Infos);
            #endregion

            #region 指定字体
            Font_Bold = Editor_XHud_GUI.GetFont("SS_Editor_Bold");
            Font_Thin = Editor_XHud_GUI.GetFont("SS_Editor_Thin");
            #endregion

            Ver = HudDevInfos.ver;
            Sub = HudDevInfos.sub;
        }

        private void OnGUI()
        {
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.无, HudColor.无);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            #region LOGO
            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无);
            Editor_XHud_GUI.Gui_Layout_Space(5);
            GUILayout.FlexibleSpace();

            Editor_XHud_GUI.Gui_Layout_Labelfield("X  H  U  D", HudFilled.无, HudColor.亮白, Color.white, TextAnchor.MiddleRight, new Vector2(0, 15), 35, Font_Thin);

            Rect rect_logo = GUILayoutUtility.GetLastRect();
            GUILayout.FlexibleSpace();
            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Horizontal_End();
            Editor_XHud_GUI.Gui_Labelfield_Thin(new Rect(rect_logo.x + 64, rect_logo.height + 60, 60, 20), Ver, HudFilled.无, HudColor.亮白, XHud_Dashboard.Theme_Primary, TextAnchor.MiddleRight, new Vector2(0, 0), 11);
            Editor_XHud_GUI.Gui_Labelfield_Thin(new Rect(rect_logo.x, rect_logo.height + 60, 60, 20), Sub, HudFilled.无, HudColor.亮白, Color.gray, TextAnchor.MiddleLeft, new Vector2(0, 0), 11);
            #endregion

            Editor_XHud_GUI.Gui_Layout_Space(80);

            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无);
            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Labelfield("此插件由 （南京塞维斯传媒）SevenStrikeMedia 独家开发", HudFilled.无, HudColor.无, Editor_XHud_GUI.GetColor(HudColor.阴影灰), TextAnchor.MiddleCenter, 12);
            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Horizontal_End();

            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无);
            Editor_XHud_GUI.Gui_Layout_Space(15);
            Editor_XHud_GUI.Gui_Layout_TextArea_Wrap(HudDevInfos.intro, HudFilled.无, HudColor.无, new Color(1, 1, 1, 0.65f), TextAnchor.UpperLeft, 330, 12);
            Editor_XHud_GUI.Gui_Layout_Space(15);
            Editor_XHud_GUI.Gui_Layout_Horizontal_End();

            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无);
            Editor_XHud_GUI.Gui_Layout_Space(15);
            if (Editor_XHud_GUI.Gui_Layout_Button("访问XHud官网", "", HudFilled.实体, HudColor.深空灰, Color.white, 35, new RectOffset(), new Vector2(0, 0)))
            {
                OpenURL(HudDevInfos.website);
            }
            Editor_XHud_GUI.Gui_Layout_Space(10);
            if (Editor_XHud_GUI.Gui_Layout_Button("教程手册", "", HudFilled.实体, HudColor.深空灰, Color.white, 35, new RectOffset(), new Vector2(0, 0)))
            {
                OpenURL(HudDevInfos.document);
            }
            Editor_XHud_GUI.Gui_Layout_Space(15);
            Editor_XHud_GUI.Gui_Layout_Horizontal_End();

            Editor_XHud_GUI.Gui_Layout_Space(15);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
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
