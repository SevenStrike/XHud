namespace SevenStrikeModules.XHud.Editor
{
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
                    string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 资源库消息", "未指定色卡库", "未在HudManager中配置色卡库！请先前往XHud管理器指定一个色卡库！", "明白", "前往", 1);
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
                    string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 资源库消息", "未指定曲线库", "未在HudManager中配置曲线库！请先前往XHud管理器指定一个曲线库！", "明白", "前往", 1);
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
                    string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 资源库消息", "未指定音效库", "未在HudManager中配置音效库！请先前往XHud管理器指定一个音效库！", "明白", "前往", 1);
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
                    string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 资源库消息", "未指定元素库", "未在HudManager中配置元素库！请先前往XHud管理器指定一个元素库！", "明白", "前往", 1);
                    if (res == "前往")
                    {
                        Transform man = FindFirstObjectByType<XHud_Manager>().transform;
                        EditorGUIUtility.PingObject(man);
                    }
                    return;
                };
            }

            if (mgr.Hud_ElementLibrarys.Count == 1)
            {
                XHud_Library_Element lib = mgr.Hud_ElementLibrarys[0];
                EditorUtility.OpenPropertyEditor(lib);
            }
            else
            {
                Editor_XHud_LibrarySetTool_ElementLib Window = (Editor_XHud_LibrarySetTool_ElementLib)EditorWindow.GetWindow(typeof(Editor_XHud_LibrarySetTool_ElementLib), true, "当前部署的Hud元素库列表", true);
                Window.minSize = new Vector2(620, 450);
                Window.maxSize = Window.minSize;
                //Window.mode = "reference";
                Window.Show();
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
        public static void open_font()
        {
            XHud_Library_TextStyle lib = (XHud_Library_TextStyle)FindFirstObjectByType<XHud_Manager>().Hud_TextStyleLibrary;
            if (lib == null)
            {
                EditorApplication.delayCall += () =>
                {
                    string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 资源库消息", "未指定字体库", "未在HudManager中配置字体库！请先前往XHud管理器指定一个字体库！", "明白", "前往", 1);
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
                    string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 资源库消息", "未指定动效库", "未在HudManager中配置动效库！请先前往XHud管理器指定一个动效库！", "明白", "前往", 1);
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
                    string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 资源库消息", "未指定转场库", "未在HudManager中配置转场库！请先前往XHud管理器指定一个转场库！", "明白", "前往", 1);
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