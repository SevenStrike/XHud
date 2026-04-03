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
 */namespace SevenStrikeModules.XHud.Editor
{
    using SevenStrikeModules.XHud.Utilitys;
    using SoftMasking;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;

    public class Editor_XHud_MenuItemsAction_HierarchyUtility : EditorWindow
    {
        [MenuItem("GameObject/XHud/Utilitys (实用工具)/ChildCountGet (获取子物体数量)", priority = -1)]
        private static void util_GetChildCount()
        {
            GameObject seleobject = Selection.activeGameObject;
            string hexcol = XHud_Utilitys.Color_To_HexColor(XHud_Dashboard.Theme_Primary, true);

            Editor_XHud_GUI.Open(XHud_DialogType.警告, $"XHud - 实用工具消息", "获取子物体数量", $"当前物体的子物体数量为： <color={hexcol}> {seleobject.transform.childCount} </color>", "明白", 0);

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
                XHud_Module_Text txt = selectedObject.GetComponent<XHud_Module_Text>();
                XHud_Module_TmpText tmptxt = selectedObject.GetComponent<XHud_Module_TmpText>();
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
            string res = Editor_XHud_GUI.Open(XHud_DialogType.帮助, "XHud - 遮罩体消息", "创建遮罩体", $"确定要为 {obj.name} 创建遮罩体吗？", "创建", "暂不", 0);
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