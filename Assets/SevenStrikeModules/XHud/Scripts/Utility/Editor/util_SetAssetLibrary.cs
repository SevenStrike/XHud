namespace SevenStrikeModules.XHud.Utilitys
{
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.Hud;
    using UnityEditor;
    using UnityEngine;

    public class util_SetAssetLibrary : EditorWindow
    {
        [MenuItem("Assets/XHud/AssignLibrary (设置为当前使用的资源库)", priority = 1000, validate = true)]
        private static bool ValidateSetAssetLibrary()
        {
            // 获取当前选中的对象
            Object selectedObject = Selection.activeObject;

            bool valid = false;

            if (selectedObject is Hud_ColorsLibrary)
                valid = true;
            if (selectedObject is Hud_CurvesLibrary)
                valid = true;
            if (selectedObject is Hud_ElementLibrary)
                valid = true;
            if (selectedObject is Hud_MotionLibrary)
                valid = true;
            if (selectedObject is Hud_SoundsLibrary)
                valid = true;
            if (selectedObject is Hud_TextStyleLibrary)
                valid = true;
            if (selectedObject is Hud_TransitionLibrary)
                valid = true;

            // 检查选中的对象是否为 Material 类型
            return valid;
        }

        [MenuItem("Assets/XHud/AssignLibrary (设置为当前使用的资源库)", priority = 1000)]
        private static void SetAssetLibrary()
        {
            Hud_Manager manager = FindFirstObjectByType<Hud_Manager>();

            Object obj = Selection.activeObject;

            if (manager != null)
            {
                string type = obj.GetType().ToString();

                if (type == "SevenStrikeModules.XHud.Hud_SoundsLibrary")
                {
                    manager.Hud_Sounds = obj as Hud_SoundsLibrary;
                    util_Tools.Func_PrintInfo("XHud库通知", "已将名称为： " + manager.Hud_Sounds.LibraryName + " 的音效库设为当前使用！", HudMsgState.通知);
                }
                else if (type == "SevenStrikeModules.XHud.Hud_TextStyleLibrary")
                {
                    manager.Hud_TextStyleLibrary = obj as Hud_TextStyleLibrary;
                    util_Tools.Func_PrintInfo("XHud库通知", "已将名称为： " + manager.Hud_TextStyleLibrary.LibraryName + " 的字体库设为当前使用！", HudMsgState.通知);
                }
                else if (type == "SevenStrikeModules.XHud.Hud_TransitionLibrary")
                {
                    manager.Hud_TransitionLib = obj as Hud_TransitionLibrary;
                    util_Tools.Func_PrintInfo("XHud库通知", "已将名称为： " + manager.Hud_TransitionLib.LibraryName + " 的转场库设为当前使用！", HudMsgState.通知);
                }
                else if (type == "SevenStrikeModules.XHud.Hud_MotionLibrary")
                {
                    manager.Hud_ElementMotion = obj as Hud_MotionLibrary;
                    util_Tools.Func_PrintInfo("XHud库通知", "已将名称为： " + manager.Hud_ElementMotion.name + " 的动效库设为当前使用！", HudMsgState.通知);
                }
                else if (type == "SevenStrikeModules.XHud.Hud_ElementLibrary")
                {
                    Hud_ElementLibrary lib = obj as Hud_ElementLibrary;
                    if (manager.Hud_ElementLibrarys.Count > 0)
                    {
                        bool isexist = false;
                        for (int i = 0; i < manager.Hud_ElementLibrarys.Count; i++)
                        {
                            if (manager.Hud_ElementLibrarys[i].LibraryName == lib.LibraryName || manager.Hud_ElementLibrarys[i].GetInstanceID() == lib.GetInstanceID())
                            {
                                isexist = true;
                            }
                        }
                        if (!isexist)
                        {
                            manager.Hud_ElementLibrarys.Add(lib);
                            util_Tools.Func_PrintInfo("XHud库通知", "已将名称为： " + lib.LibraryName + " 的元素库添加到元素库列表中！", HudMsgState.通知);
                        }
                        else
                        {
                            util_Tools.Func_PrintInfo("XHud库通知", "元素库列表中已存在名称为： " + lib.LibraryName + " 的元素库！", HudMsgState.警告);
                        }
                    }
                    else
                    {
                        manager.Hud_ElementLibrarys.Add(lib);
                        util_Tools.Func_PrintInfo("XHud库通知", "已将名称为： " + lib.LibraryName + " 的元素库添加到元素库列表中！", HudMsgState.通知);
                    }
                }
                else if (type == "SevenStrikeModules.XHud.Hud_CurvesLibrary")
                {
                    manager.Hud_Curves = obj as Hud_CurvesLibrary;
                    util_Tools.Func_PrintInfo("XHud库通知", "已将名称为： " + manager.Hud_Curves.LibraryName + " 的曲线库设为当前使用！", HudMsgState.通知);
                }
                else if (type == "SevenStrikeModules.XHud.Hud_ColorsLibrary")
                {
                    manager.Hud_Colors = obj as Hud_ColorsLibrary;
                    util_Tools.Func_PrintInfo("XHud库通知", "已将名称为： " + manager.Hud_Colors.LibraryName + " 的颜色库设为当前使用！", HudMsgState.通知);
                }
            }
        }
    }
}