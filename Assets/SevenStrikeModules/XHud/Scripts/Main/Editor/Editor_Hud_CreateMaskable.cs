namespace SevenStrikeModules.XHud.Hud
{
    using SevenStrikeModules.XHud.GuiLib;
    using SoftMasking;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;

    public class Editor_Hud_CreateMaskable : EditorWindow
    {
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
                Hud_Text txt = selectedObject.GetComponent<Hud_Text>();
                Hud_TmpText tmptxt = selectedObject.GetComponent<Hud_TmpText>();
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
            string res = util_XHUDGUI.Open(XHudDialogType.帮助, "HudMaskable 消息", "创建遮罩体", $"确定要为 {obj.name} 创建遮罩体吗？", "创建", "暂不", 0);
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