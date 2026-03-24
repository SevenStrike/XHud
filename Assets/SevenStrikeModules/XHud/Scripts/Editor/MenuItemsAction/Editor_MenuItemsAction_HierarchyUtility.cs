namespace SevenStrikeModules.XHud.Hud
{
    using SevenStrikeModules.XHud.GuiLib;
    using SevenStrikeModules.XHud.Utilitys;
    using SoftMasking;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;

    public class Editor_MenuItemsAction_HierarchyUtility : EditorWindow
    {
        [MenuItem("GameObject/XHud/Utilitys (实用工具)/ChildCountGet (获取子物体数量)", priority = -1)]
        private static void util_GetChildCount()
        {
            GameObject seleobject = Selection.activeGameObject;
            string hexcol = xHud_Utilitys.Color_To_HexColor(xHud_Dashboard.Theme_Primary, true);

            Editor_xHudGUI.Open(xHudDialogType.警告, $"XHud实用工具消息", "获取子物体数量", $"当前物体的子物体数量为： <color={hexcol}> {seleobject.transform.childCount} </color>", "明白", 0);

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
        [MenuItem("GameObject/XHud/Maskable（遮罩体） #m", priority = 2000, validate = true)]
        private static bool Validate_CreateMaskable()
        {
            // 获取当前选中的对象
            GameObject selectedObject = Selection.activeObject as GameObject;
            bool valid = false;
            if (selectedObject != null)
            {
                Image img = selectedObject.GetComponent<Image>();
                RawImage rawimg = selectedObject.GetComponent<RawImage>();
                xHud_Module_Text txt = selectedObject.GetComponent<xHud_Module_Text>();
                xHud_Module_TmpText tmptxt = selectedObject.GetComponent<xHud_Module_TmpText>();
                SoftMask mask = selectedObject.GetComponentInParent<SoftMask>();

                if (img != null)
                {
                    valid = true;
                }
                if (rawimg != null)
                {
                    valid = true;
                }
                if (txt != null)
                {
                    valid = true;
                }
                if (tmptxt != null)
                {
                    valid = true;
                }
                if (mask != null)
                {
                    valid = false;
                }
            }
            return valid;
        }
        [MenuItem("GameObject/XHud/Maskable（遮罩体） #m", priority = 2000)]
        private static void CreateMaskable()
        {
            RectTransform obj = Selection.activeTransform.GetComponent<RectTransform>();
            string res = Editor_xHudGUI.Open(xHudDialogType.帮助, "HudMaskable 消息", "创建遮罩体", $"确定要为 {obj.name} 创建遮罩体吗？", "创建", "暂不", 0);
            if (res == "暂不")
                return;
            GameObject obj_maskRoot = new GameObject();
            RectTransform rect_mask = obj_maskRoot.AddComponent<RectTransform>();
            SoftMask mask = obj_maskRoot.AddComponent<SoftMask>();
            rect_mask.SetParent(obj.parent);
            rect_mask.sizeDelta = obj.sizeDelta;
            rect_mask.position = obj.position;
            rect_mask.localScale = Vector3.one;
            rect_mask.name = "Maskable_" + obj.name;
            rect_mask.transform.SetSiblingIndex(obj.GetSiblingIndex());
            obj.SetParent(rect_mask);

            GameObject obj_mask = new GameObject();
            obj_mask.name = "Mask";
            RectTransform img_mask_rect = obj_mask.AddComponent<RectTransform>();
            Selection.activeTransform = img_mask_rect;
            img_mask_rect.SetParent(rect_mask);
            img_mask_rect.SetAsFirstSibling();
            img_mask_rect.sizeDelta = obj.sizeDelta;
            img_mask_rect.position = obj.position;
            img_mask_rect.localScale = Vector3.one;
            Image img_mask = Undo.AddComponent<Image>(obj_mask);
            img_mask.raycastTarget = false;
            img_mask.maskable = false;
            Color color = img_mask.color;
            color.a = 0;
            img_mask.color = color;
            mask.separateMask = img_mask_rect;
        }
    }
}