namespace SevenStrikeModules.XHud.Utilitys
{
    using SevenStrikeModules.XHud.GuiLib;
    using SevenStrikeModules.XHud.Hud;
    using UnityEditor;
    using UnityEngine;

    public static class util_Utilitys
    {
        [MenuItem("GameObject/XHud/Utilitys (实用工具)/ChildCountGet (获取子物体数量)", priority = -1)]
        private static void util_GetChildCount()
        {
            GameObject seleobject = Selection.activeGameObject;
            string hexcol = util_Tools.Color_To_HexColor(util_Dashboard.Theme_Primary, true);

            util_XHUDGUI.Open(XHudDialogType.警告, $"XHud实用工具消息", "获取子物体数量", $"当前物体的子物体数量为： <color={hexcol}> {seleobject.transform.childCount} </color>", "明白", 0);

        }

        [MenuItem("GameObject/XHud/Utilitys (实用工具)/SelectedAllChild (选中所有子物体)", priority = -10)]
        private static void util_SelectedAllChild()
        {
            GameObject seleobject = Selection.activeGameObject;
            GameObject[] arr = new GameObject[seleobject.transform.childCount];
            for (int i = 0; i < arr.Length; i++)
            {
                GameObject obj = seleobject.transform.GetChild(i).gameObject;
                if (obj.activeSelf)
                    arr[i] = obj;
            }
            Selection.objects = arr;
        }
    }
}