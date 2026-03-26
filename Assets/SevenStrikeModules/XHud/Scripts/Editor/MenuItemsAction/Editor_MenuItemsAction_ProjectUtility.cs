namespace SevenStrikeModules.XHud.Hud
{
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.GuiLib;
    using SevenStrikeModules.XHud.Utilitys;
    using TMPro;
    using UnityEditor;
    using UnityEngine;

    public class Editor_MenuItemsAction_ProjectUtility : EditorWindow
    {
        [MenuItem("Assets/XHud/ClearTmpFontAssetData (清理TMP字体资源图集缓存内容)", priority = 2000, validate = true)]
        private static bool ValidateClearTmpFontAssetContent()
        {
            // 获取当前选中的对象
            Object selectedObject = Selection.activeObject;

            // 检查选中的对象是否为 TMP_FontAsset 类型
            return selectedObject is TMP_FontAsset;
        }
        [MenuItem("Assets/XHud/ClearTmpFontAssetData (清理TMP字体资源图集缓存内容)", priority = 2000)]
        private static void ClearTmpFontAssetContent()
        {
            Object FontAsset = Selection.activeObject;

            string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud TmpFontAssets清理器消息", "清理TmpFontAssetAtlas", $"您确认要将 {FontAsset.name} 图集内容清空吗？清空后字体图集将保持纯净状态！此操作不可逆，请谨慎操作！", "清空", "暂不", 1);
            if (res == "暂不")
            {
                return;
            }
            else
            {
                try
                {
                    TMP_FontAsset sd = (TMP_FontAsset)FontAsset;

                    sd.ClearFontAssetData();
                }
                catch (System.Exception err)
                {
                    Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud TmpFontAssets清理器消息", "清理TmpFontAssetAtlas", $"您选中的物体 {FontAsset.name} 并非是TmpFontAsset类型！,详细信息： {err.Message}", "明白");
                }
            }
        }
        [MenuItem("Assets/XHud/AssignLibrary (设置为当前使用的资源库)", priority = 1000, validate = true)]
        private static bool ValidateSetAssetLibrary()
        {
            // 获取当前选中的对象
            Object selectedObject = Selection.activeObject;

            bool valid = false;

            if (selectedObject is XHud_Library_Colors)
                valid = true;
            if (selectedObject is XHud_Library_Curves)
                valid = true;
            if (selectedObject is XHud_Library_Element)
                valid = true;
            if (selectedObject is XHud_Library_Motion)
                valid = true;
            if (selectedObject is XHud_Library_Sounds)
                valid = true;
            if (selectedObject is XHud_Library_TextStyle)
                valid = true;
            if (selectedObject is XHud_Library_Transition)
                valid = true;

            // 检查选中的对象是否为 Material 类型
            return valid;
        }
        [MenuItem("Assets/XHud/AssignLibrary (设置为当前使用的资源库)", priority = 1000)]
        private static void SetAssetLibrary()
        {
            XHud_Manager manager = FindFirstObjectByType<XHud_Manager>();

            Object obj = Selection.activeObject;

            if (manager != null)
            {
                string type = obj.GetType().ToString();

                if (type == "SevenStrikeModules.XHud.Hud_SoundsLibrary")
                {
                    manager.Hud_Sounds = obj as XHud_Library_Sounds;
                    XHud_Utilitys.Func_PrintInfo("XHud库通知", "已将名称为： " + manager.Hud_Sounds.LibraryName + " 的音效库设为当前使用！", HudMsgState.通知);
                }
                else if (type == "SevenStrikeModules.XHud.Hud_TextStyleLibrary")
                {
                    manager.Hud_TextStyleLibrary = obj as XHud_Library_TextStyle;
                    XHud_Utilitys.Func_PrintInfo("XHud库通知", "已将名称为： " + manager.Hud_TextStyleLibrary.LibraryName + " 的字体库设为当前使用！", HudMsgState.通知);
                }
                else if (type == "SevenStrikeModules.XHud.Hud_TransitionLibrary")
                {
                    manager.Hud_TransitionLib = obj as XHud_Library_Transition;
                    XHud_Utilitys.Func_PrintInfo("XHud库通知", "已将名称为： " + manager.Hud_TransitionLib.LibraryName + " 的转场库设为当前使用！", HudMsgState.通知);
                }
                else if (type == "SevenStrikeModules.XHud.Hud_MotionLibrary")
                {
                    manager.Hud_ElementMotion = obj as XHud_Library_Motion;
                    XHud_Utilitys.Func_PrintInfo("XHud库通知", "已将名称为： " + manager.Hud_ElementMotion.name + " 的动效库设为当前使用！", HudMsgState.通知);
                }
                else if (type == "SevenStrikeModules.XHud.Hud_ElementLibrary")
                {
                    XHud_Library_Element lib = obj as XHud_Library_Element;
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
                            XHud_Utilitys.Func_PrintInfo("XHud库通知", "已将名称为： " + lib.LibraryName + " 的元素库添加到元素库列表中！", HudMsgState.通知);
                        }
                        else
                        {
                            XHud_Utilitys.Func_PrintInfo("XHud库通知", "元素库列表中已存在名称为： " + lib.LibraryName + " 的元素库！", HudMsgState.警告);
                        }
                    }
                    else
                    {
                        manager.Hud_ElementLibrarys.Add(lib);
                        XHud_Utilitys.Func_PrintInfo("XHud库通知", "已将名称为： " + lib.LibraryName + " 的元素库添加到元素库列表中！", HudMsgState.通知);
                    }
                }
                else if (type == "SevenStrikeModules.XHud.Hud_CurvesLibrary")
                {
                    manager.Hud_Curves = obj as XHud_Library_Curves;
                    XHud_Utilitys.Func_PrintInfo("XHud库通知", "已将名称为： " + manager.Hud_Curves.LibraryName + " 的曲线库设为当前使用！", HudMsgState.通知);
                }
                else if (type == "SevenStrikeModules.XHud.Hud_ColorsLibrary")
                {
                    manager.Hud_Colors = obj as XHud_Library_Colors;
                    XHud_Utilitys.Func_PrintInfo("XHud库通知", "已将名称为： " + manager.Hud_Colors.LibraryName + " 的颜色库设为当前使用！", HudMsgState.通知);
                }
            }
        }
    }
}