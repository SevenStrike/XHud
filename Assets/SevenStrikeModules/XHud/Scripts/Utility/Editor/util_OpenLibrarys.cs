namespace SevenStrikeModules.XHud.Hud
{
    using SevenStrikeModules.XHud.GuiLib;
    using UnityEditor;
    using UnityEngine;

    public class util_OpenLibrarys : EditorWindow
    {
        [MenuItem(("Tools/XHud/Library/CurrentLibrary-Color #F1"))]
        public static void open_col()
        {
            Hud_ColorsLibrary lib = (Hud_ColorsLibrary)FindFirstObjectByType<Hud_Manager>().Hud_Colors;
            if (lib == null)
            {
                string res = util_XHUDGUI.Open(XHudDialogType.警告, "XHud资源库消息", "未指定色卡库", "未在HudManager中配置色卡库！请先前往XHud管理器指定一个色卡库！", "明白", "前往", 1);
                if (res == "前往")
                {
                    Transform man = FindFirstObjectByType<Hud_Manager>().transform;
                    EditorGUIUtility.PingObject(man);
                }
                return;
            }
            EditorUtility.OpenPropertyEditor(lib);
        }

        [MenuItem(("Tools/XHud/Library/CurrentLibrary-Curve #F2"))]
        public static void open_curve()
        {
            Hud_CurvesLibrary lib = (Hud_CurvesLibrary)FindFirstObjectByType<Hud_Manager>().Hud_Curves;
            if (lib == null)
            {
                string res = util_XHUDGUI.Open(XHudDialogType.警告, "XHud资源库消息", "未指定曲线库", "未在HudManager中配置曲线库！请先前往XHud管理器指定一个曲线库！", "明白", "前往", 1);
                if (res == "前往")
                {
                    Transform man = FindFirstObjectByType<Hud_Manager>().transform;
                    EditorGUIUtility.PingObject(man);
                }
                return;
            }
            EditorUtility.OpenPropertyEditor(lib);
        }

        [MenuItem(("Tools/XHud/Library/CurrentLibrary-Sound #F3"))]
        public static void open_sound()
        {
            Hud_SoundsLibrary lib = (Hud_SoundsLibrary)FindFirstObjectByType<Hud_Manager>().Hud_Sounds;
            if (lib == null)
            {
                string res = util_XHUDGUI.Open(XHudDialogType.警告, "XHud资源库消息", "未指定音效库", "未在HudManager中配置音效库！请先前往XHud管理器指定一个音效库！", "明白", "前往", 1);
                if (res == "前往")
                {
                    Transform man = FindFirstObjectByType<Hud_Manager>().transform;
                    EditorGUIUtility.PingObject(man);
                }
                return;
            }
            EditorUtility.OpenPropertyEditor(lib);
        }

        [MenuItem(("Tools/XHud/Library/CurrentLibrary-Element #F4"))]
        public static void open_elements()
        {
            Hud_Manager mgr = util_Dashboard.HudManagerGet();
            if (mgr.Hud_ElementLibrarys.Count <= 0)
            {
                string res = util_XHUDGUI.Open(XHudDialogType.警告, "XHud资源库消息", "未指定元素库", "未在HudManager中配置元素库！请先前往XHud管理器指定一个元素库！", "明白", "前往", 1);
                if (res == "前往")
                {
                    Transform man = FindFirstObjectByType<Hud_Manager>().transform;
                    EditorGUIUtility.PingObject(man);
                }
                return;
            }

            if (mgr.Hud_ElementLibrarys.Count == 1)
            {
                Hud_ElementLibrary lib = mgr.Hud_ElementLibrarys[0];
                EditorUtility.OpenPropertyEditor(lib);
            }
            else
            {
                util_Hud_Library_ElementLib_Selector Window = (util_Hud_Library_ElementLib_Selector)EditorWindow.GetWindow(typeof(util_Hud_Library_ElementLib_Selector), true, "当前部署的Hud元素库列表", true);
                Window.minSize = new Vector2(620, 450);
                Window.maxSize = Window.minSize;
                //Window.mode = "reference";
                Window.Show();
            }
        }

        public static Hud_ElementLibrary open_target_elements(string libname)
        {
            Hud_ElementLibrary ele_lib = null;

            Hud_Manager man = util_Dashboard.HudManagerGet();
            for (int i = 0; i < man.Hud_ElementLibrarys.Count; i++)
            {
                Hud_ElementLibrary lib = man.Hud_ElementLibrarys[i];
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
            Hud_TextStyleLibrary lib = (Hud_TextStyleLibrary)FindFirstObjectByType<Hud_Manager>().Hud_TextStyleLibrary;
            if (lib == null)
            {
                string res = util_XHUDGUI.Open(XHudDialogType.警告, "XHud资源库消息", "未指定字体库", "未在HudManager中配置字体库！请先前往XHud管理器指定一个字体库！", "明白", "前往", 1);
                if (res == "前往")
                {
                    Transform man = FindFirstObjectByType<Hud_Manager>().transform;
                    EditorGUIUtility.PingObject(man);
                }
                return;
            }
            EditorUtility.OpenPropertyEditor(lib);
        }

        [MenuItem(("Tools/XHud/Library/CurrentLibrary-ElementMotionLibrary #F6"))]
        public static void open_elementmotion()
        {
            Hud_MotionLibrary lib = (Hud_MotionLibrary)FindFirstObjectByType<Hud_Manager>().Hud_ElementMotion;
            if (lib == null)
            {
                string res = util_XHUDGUI.Open(XHudDialogType.警告, "XHud资源库消息", "未指定动效库", "未在HudManager中配置动效库！请先前往XHud管理器指定一个动效库！", "明白", "前往", 1);
                if (res == "前往")
                {
                    Transform man = FindFirstObjectByType<Hud_Manager>().transform;
                    EditorGUIUtility.PingObject(man);
                }
                return;
            }
            EditorUtility.OpenPropertyEditor(lib);
        }

        [MenuItem(("Tools/XHud/Library/CurrentLibrary-TransitionLibrary #F7"))]
        public static void open_transition()
        {
            Hud_TransitionLibrary lib = (Hud_TransitionLibrary)FindFirstObjectByType<Hud_Manager>().Hud_TransitionLib;
            if (lib == null)
            {
                string res = util_XHUDGUI.Open(XHudDialogType.警告, "XHud资源库消息", "未指定转场库", "未在HudManager中配置转场库！请先前往XHud管理器指定一个转场库！", "明白", "前往", 1);
                if (res == "前往")
                {
                    Transform man = FindFirstObjectByType<Hud_Manager>().transform;
                    EditorGUIUtility.PingObject(man);
                }
                return;
            }
            EditorUtility.OpenPropertyEditor(lib);
        }
    }
}