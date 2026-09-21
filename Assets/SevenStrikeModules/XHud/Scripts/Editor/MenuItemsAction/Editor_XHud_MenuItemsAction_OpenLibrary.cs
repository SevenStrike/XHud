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
    using UnityEditor;
    using UnityEngine;

    public class Editor_XHud_MenuItemsAction_OpenLibrary : EditorWindow
    {
        [MenuItem(("Tools/XHud/Library/CurrentLibrary-Color #F1"))]
        public static void open_col()
        {
            XHud_Library_Colors lib = (XHud_Library_Colors)FindFirstObjectByType<XHud_Manager>().Hud_Colors;
            if (lib == null)
            {
                EditorApplication.delayCall += () =>
                {
                    string res = XGUI.dialog(
                        type: XGUIDialogType.警告,
                        windowtitle: "XHud - 色卡库消息",
                        title: "未指定色卡库",
                        msg: "未在XHudManager中配置色卡库！请先前往XHud管理器指定一个色卡库！",
                        ok: "明白",
                        cancel: "前往",
                        PrimaryIndex: 1,
                        themecolor: XHud_Dashboard.Theme_Primary);
                    if (res == "前往")
                    {
                        Transform man = FindFirstObjectByType<XHud_Manager>().transform;
                        EditorGUIUtility.PingObject(man);
                    }
                    return;
                };
            }
            EditorUtility.OpenPropertyEditor(lib);
        }
        [MenuItem(("Tools/XHud/Library/CurrentLibrary-Curve #F2"))]
        public static void open_curve()
        {
            XHud_Library_Curves lib = (XHud_Library_Curves)FindFirstObjectByType<XHud_Manager>().Hud_Curves;
            if (lib == null)
            {
                EditorApplication.delayCall += () =>
                {
                    string res = XGUI.dialog(
                      type: XGUIDialogType.警告,
                      windowtitle: "XHud - 曲线库消息",
                      title: "未指定曲线库",
                      msg: "未在XHudManager中配置曲线库！请先前往XHud管理器指定一个曲线库！",
                      ok: "明白",
                      cancel: "前往",
                      PrimaryIndex: 1,
                      themecolor: XHud_Dashboard.Theme_Primary);
                    if (res == "前往")
                    {
                        Transform man = FindFirstObjectByType<XHud_Manager>().transform;
                        EditorGUIUtility.PingObject(man);
                    }
                    return;
                };
            }
            EditorUtility.OpenPropertyEditor(lib);
        }
        [MenuItem(("Tools/XHud/Library/CurrentLibrary-Sound #F3"))]
        public static void open_sound()
        {
            XHud_Library_Sounds lib = (XHud_Library_Sounds)FindFirstObjectByType<XHud_Manager>().Hud_Sounds;
            if (lib == null)
            {
                EditorApplication.delayCall += () =>
                {
                    string res = XGUI.dialog(
                        type: XGUIDialogType.警告,
                        windowtitle: "XHud - 音效库消息",
                        title: "未指定音效库",
                        msg: "未在XHudManager中配置音效库！请先前往XHud管理器指定一个音效库！",
                        ok: "明白",
                        cancel: "前往",
                        PrimaryIndex: 1,
                        themecolor: XHud_Dashboard.Theme_Primary);
                    if (res == "前往")
                    {
                        Transform man = FindFirstObjectByType<XHud_Manager>().transform;
                        EditorGUIUtility.PingObject(man);
                    }
                    return;
                };
            }
            EditorUtility.OpenPropertyEditor(lib);
        }
        [MenuItem(("Tools/XHud/Library/CurrentLibrary-Element #F4"))]
        public static void open_elements()
        {
            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

            if (mgr.Hud_ElementLibrarys.Count <= 0)
            {
                EditorApplication.delayCall += () =>
                {
                    string res = XGUI.dialog(
                        type: XGUIDialogType.警告,
                        windowtitle: "XHud - 资源库消息",
                        title: "未指定元素库",
                        msg: "未在XHudManager中配置元素库！请先前往XHud管理器指定一个元素库！",
                        ok: "明白",
                        cancel: "前往",
                        PrimaryIndex: 1,
                        themecolor: XHud_Dashboard.Theme_Primary);
                    if (res == "前往")
                    {
                        Transform man = FindFirstObjectByType<XHud_Manager>().transform;
                        EditorGUIUtility.PingObject(man);
                    }
                    return;
                };
            }
            else
            {
                if (mgr.Hud_ElementLibrarys.Count == 1)
                {
                    XHud_Library_Element lib = mgr.Hud_ElementLibrarys[0];
                    EditorUtility.OpenPropertyEditor(lib);
                }
                else
                {
                    Editor_XHud_LibrarySetTool_ElementLib window = EditorWindow.GetWindow<Editor_XHud_LibrarySetTool_ElementLib>(true);
                    window.titleContent = new GUIContent("当前已部署的XHud元素库列表");
                    XGUI.CenterEditorWindow(new Vector2Int(500, 440), window);

                    window.Show();
                }
            }
        }
        public static XHud_Library_Element open_target_elements(string libname)
        {
            XHud_Library_Element ele_lib = null;

            XHud_Manager man = XHud_Dashboard.HudManagerGet();
            for (int i = 0; i < man.Hud_ElementLibrarys.Count; i++)
            {
                XHud_Library_Element lib = man.Hud_ElementLibrarys[i];
                if (lib.LibraryName == libname)
                {
                    ele_lib = lib;
                    EditorUtility.OpenPropertyEditor(lib);
                }
            }

            return ele_lib;
        }
        [MenuItem(("Tools/XHud/Library/CurrentLibrary-TextStyleLibrary #F5"))]
        public static void open_textstyle()
        {
            XHud_Library_TextStyle lib = (XHud_Library_TextStyle)FindFirstObjectByType<XHud_Manager>().Hud_TextStyleLibrary;
            if (lib == null)
            {
                EditorApplication.delayCall += () =>
                {
                    string res = XGUI.dialog(
                        type: XGUIDialogType.警告,
                        windowtitle: "XHud - 文字样式库消息",
                        title: "未指定文字样式库",
                        msg: "未在XHudManager中配置字体库！请先前往XHud管理器指定一个文字样式库！",
                        ok: "明白",
                        cancel: "前往",
                        PrimaryIndex: 1,
                        themecolor: XHud_Dashboard.Theme_Primary);
                    if (res == "前往")
                    {
                        Transform man = FindFirstObjectByType<XHud_Manager>().transform;
                        EditorGUIUtility.PingObject(man);
                    }
                    return;
                };
            }
            EditorUtility.OpenPropertyEditor(lib);
        }
        [MenuItem(("Tools/XHud/Library/CurrentLibrary-ElementMotionLibrary #F6"))]
        public static void open_elementmotion()
        {
            XHud_Library_Motion lib = (XHud_Library_Motion)FindFirstObjectByType<XHud_Manager>().Hud_Motions;
            if (lib == null)
            {
                EditorApplication.delayCall += () =>
                {
                    string res = XGUI.dialog(
                       type: XGUIDialogType.警告,
                       windowtitle: "XHud - 元素动效库消息",
                       title: "未指定元素动效库",
                       msg: "未在XHudManager中配置元素动效库！请先前往XHud管理器指定一个元素动效库！",
                       ok: "明白",
                       cancel: "前往",
                       PrimaryIndex: 1,
                       themecolor: XHud_Dashboard.Theme_Primary);
                    if (res == "前往")
                    {
                        Transform man = FindFirstObjectByType<XHud_Manager>().transform;
                        EditorGUIUtility.PingObject(man);
                    }
                    return;
                };
            }
            EditorUtility.OpenPropertyEditor(lib);
        }
        [MenuItem(("Tools/XHud/Library/CurrentLibrary-TransitionLibrary #F7"))]
        public static void open_transition()
        {
            XHud_Library_Transition lib = (XHud_Library_Transition)FindFirstObjectByType<XHud_Manager>().Hud_TransitionLib;
            if (lib == null)
            {
                EditorApplication.delayCall += () =>
                {
                    string res = XGUI.dialog(
                       type: XGUIDialogType.警告,
                       windowtitle: "XHud - 转场库",
                       title: "未指定转场库",
                       msg: "未在XHudManager中配置转场库！请先前往XHud管理器指定一个转场库！",
                       ok: "明白",
                       cancel: "前往",
                       PrimaryIndex: 1,
                       themecolor: XHud_Dashboard.Theme_Primary);
                    if (res == "前往")
                    {
                        Transform man = FindFirstObjectByType<XHud_Manager>().transform;
                        EditorGUIUtility.PingObject(man);
                    }
                    return;
                };
            }
            EditorUtility.OpenPropertyEditor(lib);
        }
    }
}