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
    using SevenStrikeModules.XHud.Utilitys;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    public class Editor_XHud_Tool_ElementPlacer : EditorWindow
    {
        private static void SendObject(XHudAnchor anchor)
        {
            XHud_Manager manager = FindFirstObjectByType<XHud_Manager>();
            if (manager == null)
            {
                XHud_Utilitys.Func_PrintInfo("XHud - PSD Reconstruction 通知", "未找到HudManager管理器！", HudMsgState.错误);
                return;
            }
            for (int i = 0; i < manager.Anchors_Layout_Screen.Count; i++)
            {
                if (manager.Anchors_Layout_Screen[i].Type == anchor)
                {
                    UnityEngine.Object obj = Selection.activeObject;

                    GameObject objins = PrefabUtility.InstantiatePrefab(obj, manager.Anchors_Layout_Screen[i].Anchor.transform) as GameObject;
                    RectTransform rect = objins.GetComponent<RectTransform>();
                    switch (anchor)
                    {
                        case XHudAnchor.底层:
                            rect.pivot = new Vector2(0.5f, 0.5f);
                            rect.anchorMin = new Vector2(0f, 0f);
                            rect.anchorMax = new Vector2(1f, 1f);
                            rect.offsetMin = new Vector2(0, 0);
                            rect.offsetMax = new Vector2(0, 0);
                            rect.anchoredPosition = Vector2.zero;
                            break;
                        case XHudAnchor.上:
                            rect.pivot = new Vector2(0.5f, 1f);
                            rect.anchorMin = new Vector2(0.5f, 1f);
                            rect.anchorMax = new Vector2(0.5f, 1f);
                            rect.anchoredPosition = Vector2.zero;
                            break;
                        case XHudAnchor.下:
                            rect.pivot = new Vector2(0.5f, 0f);
                            rect.anchorMin = new Vector2(0.5f, 0f);
                            rect.anchorMax = new Vector2(0.5f, 0f);
                            rect.anchoredPosition = Vector2.zero;
                            break;
                        case XHudAnchor.左:
                            rect.pivot = new Vector2(0f, 0.5f);
                            rect.anchorMin = new Vector2(0f, 0.5f);
                            rect.anchorMax = new Vector2(0f, 0.5f);
                            rect.anchoredPosition = Vector2.zero;
                            break;
                        case XHudAnchor.右:
                            rect.pivot = new Vector2(1f, 0.5f);
                            rect.anchorMin = new Vector2(1f, 0.5f);
                            rect.anchorMax = new Vector2(1f, 0.5f);
                            rect.anchoredPosition = Vector2.zero;
                            break;
                        case XHudAnchor.中心:
                            rect.pivot = new Vector2(0.5f, 0.5f);
                            rect.anchorMin = new Vector2(0.5f, 0.5f);
                            rect.anchorMax = new Vector2(0.5f, 0.5f);
                            rect.anchoredPosition = Vector2.zero;
                            break;
                        case XHudAnchor.左上:
                            rect.pivot = new Vector2(0f, 1f);
                            rect.anchorMin = new Vector2(0f, 1f);
                            rect.anchorMax = new Vector2(0f, 1f);
                            rect.anchoredPosition = Vector2.zero;
                            break;
                        case XHudAnchor.左下:
                            rect.pivot = new Vector2(0f, 0f);
                            rect.anchorMin = new Vector2(0f, 0f);
                            rect.anchorMax = new Vector2(0f, 0f);
                            rect.anchoredPosition = Vector2.zero;
                            break;
                        case XHudAnchor.右上:
                            rect.pivot = new Vector2(1f, 1f);
                            rect.anchorMin = new Vector2(1f, 1f);
                            rect.anchorMax = new Vector2(1f, 1f);
                            rect.anchoredPosition = Vector2.zero;
                            break;
                        case XHudAnchor.右下:
                            rect.pivot = new Vector2(1f, 0f);
                            rect.anchorMin = new Vector2(1f, 0f);
                            rect.anchorMax = new Vector2(1f, 0f);
                            rect.anchoredPosition = Vector2.zero;
                            break;
                        case XHudAnchor.顶层:
                            rect.pivot = new Vector2(0.5f, 0.5f);
                            rect.anchorMin = new Vector2(0.5f, 0.5f);
                            rect.anchorMax = new Vector2(0.5f, 0.5f);
                            rect.anchoredPosition = Vector2.zero;
                            break;
                        default:
                            break;
                    }

                    Selection.activeObject = objins;
                    break;
                }
            }
        }

        private static void SendObject_World()
        {
            XHud_Manager manager = FindFirstObjectByType<XHud_Manager>();
            if (manager == null)
            {
                XHud_Utilitys.Func_PrintInfo("XHud - PSD Reconstruction 通知", "未找到HudManager管理器！", HudMsgState.错误);
                return;
            }
            UnityEngine.Object obj = Selection.activeObject;

            GameObject objins = PrefabUtility.InstantiatePrefab(obj, manager.HudCanvas_WorldAnchor) as GameObject;
            RectTransform rect = objins.GetComponent<RectTransform>();
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;

            Selection.activeObject = objins;
        }

        ///-------------------------------------------------------------SendAuto

        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /SendAuto(自动识别)", priority = 500, validate = true)]
        private static bool Validate_PlaceToAnchor_Auto()
        {
            // 获取当前选中的对象
            GameObject selectedObject = Selection.activeObject as GameObject;
            if (selectedObject != null)
            {
                XHud_Module_Element element = selectedObject.GetComponent<XHud_Module_Element>();
                if (element != null)
                {
                    // 检查选中的对象是否为 Hud_Element 类型
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /SendAuto(自动识别)", priority = 500)]
        private static void PlaceToAnchor_Auto()
        {
            XHud_Manager manager = FindFirstObjectByType<XHud_Manager>();

            Object[] obj = Selection.objects;
            List<Object> CreatedObjs = new List<Object>();
            for (int i = 0; i < obj.Length; i++)
            {
                GameObject obj_ele = PrefabUtility.InstantiatePrefab(obj[i], null) as GameObject;
                Undo.RegisterCreatedObjectUndo(obj_ele, "CreateObjects");
                XHud_Module_Element ele = obj_ele.GetComponent<XHud_Module_Element>();
                Element_RMS_LayoutData rms = ele.elelemt_RMS_Get(manager.hm_RMS_GetCurrentSolution());
                ele.RectTransform.SetParent(manager.hm_ScreenElement_GetAnchored_RectTransform(rms.Anchor));
                ele.RectTransform.pivot = rms.Pivot;
                ele.RectTransform.anchorMin = rms.AnchorMin;
                ele.RectTransform.anchorMax = rms.AnchorMax;
                ele.RectTransform.anchoredPosition3D = rms.Position;
                ele.RectTransform.localEulerAngles = rms.Euler;
                ele.RectTransform.localScale = rms.Scale;

                CreatedObjs.Add(obj_ele);
            }
            Selection.objects = CreatedObjs.ToArray();
        }

        ///-------------------------------------------------------------Top

        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /Top(顶层)", priority = 500, validate = true)]
        private static bool Validate_PlaceToAnchor_T()
        {
            // 获取当前选中的对象
            GameObject selectedObject = Selection.activeObject as GameObject;
            if (selectedObject != null)
            {
                XHud_Module_Element element = selectedObject.GetComponent<XHud_Module_Element>();
                if (element != null)
                {
                    // 检查选中的对象是否为 Hud_Element 类型
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /Top(顶层)", priority = 500)]
        private static void PlaceToAnchor_T()
        {
            SendObject(XHudAnchor.顶层);
        }

        ///-------------------------------------------------------------Back

        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /Back(底层)", priority = 500, validate = true)]
        private static bool Validate_PlaceToAnchor_B()
        {
            // 获取当前选中的对象
            GameObject selectedObject = Selection.activeObject as GameObject;
            if (selectedObject != null)
            {
                XHud_Module_Element element = selectedObject.GetComponent<XHud_Module_Element>();
                if (element != null)
                {
                    // 检查选中的对象是否为 Hud_Element 类型
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /Back(底层)", priority = 500)]
        private static void PlaceToAnchor_B()
        {
            SendObject(XHudAnchor.底层);
        }

        ///-------------------------------------------------------------Center

        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /Center(中心)", priority = 500, validate = true)]
        private static bool Validate_PlaceToAnchor_C()
        {
            // 获取当前选中的对象
            GameObject selectedObject = Selection.activeObject as GameObject;
            if (selectedObject != null)
            {
                XHud_Module_Element element = selectedObject.GetComponent<XHud_Module_Element>();
                if (element != null)
                {
                    // 检查选中的对象是否为 Hud_Element 类型
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /Center(中心)", priority = 500)]
        private static void PlaceToAnchor_C()
        {
            SendObject(XHudAnchor.中心);
        }

        ///-------------------------------------------------------------Up

        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /Up(上)", priority = 500, validate = true)]
        private static bool Validate_PlaceToAnchor_U()
        {
            // 获取当前选中的对象
            GameObject selectedObject = Selection.activeObject as GameObject;
            if (selectedObject != null)
            {
                XHud_Module_Element element = selectedObject.GetComponent<XHud_Module_Element>();
                if (element != null)
                {
                    // 检查选中的对象是否为 Hud_Element 类型
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /Up(上)", priority = 500)]
        private static void PlaceToAnchor_U()
        {
            SendObject(XHudAnchor.上);
        }

        ///-------------------------------------------------------------Down

        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /Down(下)", priority = 500, validate = true)]
        private static bool Validate_PlaceToAnchor_D()
        {
            // 获取当前选中的对象
            GameObject selectedObject = Selection.activeObject as GameObject;
            if (selectedObject != null)
            {
                XHud_Module_Element element = selectedObject.GetComponent<XHud_Module_Element>();
                if (element != null)
                {
                    // 检查选中的对象是否为 Hud_Element 类型
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /Down(下)", priority = 500)]
        private static void PlaceToAnchor_D()
        {
            SendObject(XHudAnchor.下);
        }

        ///-------------------------------------------------------------Left

        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /Left(左)", priority = 500, validate = true)]
        private static bool Validate_PlaceToAnchor_L()
        {
            // 获取当前选中的对象
            GameObject selectedObject = Selection.activeObject as GameObject;
            if (selectedObject != null)
            {
                XHud_Module_Element element = selectedObject.GetComponent<XHud_Module_Element>();
                if (element != null)
                {
                    // 检查选中的对象是否为 Hud_Element 类型
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /Left(左)", priority = 500)]
        private static void PlaceToAnchor_L()
        {
            SendObject(XHudAnchor.左);
        }

        ///-------------------------------------------------------------Right

        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /Right(右)", priority = 500, validate = true)]
        private static bool Validate_PlaceToAnchor_R()
        {
            // 获取当前选中的对象
            GameObject selectedObject = Selection.activeObject as GameObject;
            if (selectedObject != null)
            {
                XHud_Module_Element element = selectedObject.GetComponent<XHud_Module_Element>();
                if (element != null)
                {
                    // 检查选中的对象是否为 Hud_Element 类型
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /Right(右)", priority = 500)]
        private static void PlaceToAnchor_R()
        {
            SendObject(XHudAnchor.右);
        }

        ///-------------------------------------------------------------LeftUp

        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /LeftUp(左上)", priority = 500, validate = true)]
        private static bool Validate_PlaceToAnchor_LU()
        {
            // 获取当前选中的对象
            GameObject selectedObject = Selection.activeObject as GameObject;
            if (selectedObject != null)
            {
                XHud_Module_Element element = selectedObject.GetComponent<XHud_Module_Element>();
                if (element != null)
                {
                    // 检查选中的对象是否为 Hud_Element 类型
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /LeftUp(左上)", priority = 500)]
        private static void PlaceToAnchor_LU()
        {
            SendObject(XHudAnchor.左上);
        }

        ///-------------------------------------------------------------LeftDown

        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /LeftDown(左下)", priority = 500, validate = true)]
        private static bool Validate_PlaceToAnchor_LD()
        {
            // 获取当前选中的对象
            GameObject selectedObject = Selection.activeObject as GameObject;
            if (selectedObject != null)
            {
                XHud_Module_Element element = selectedObject.GetComponent<XHud_Module_Element>();
                if (element != null)
                {
                    // 检查选中的对象是否为 Hud_Element 类型
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /LeftDown(左下)", priority = 500)]
        private static void PlaceToAnchor_LD()
        {
            SendObject(XHudAnchor.左下);
        }

        ///-------------------------------------------------------------RightUp

        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /RightUp(右上)", priority = 500, validate = true)]
        private static bool Validate_PlaceToAnchor_RU()
        {
            // 获取当前选中的对象
            GameObject selectedObject = Selection.activeObject as GameObject;
            if (selectedObject != null)
            {
                XHud_Module_Element element = selectedObject.GetComponent<XHud_Module_Element>();
                if (element != null)
                {
                    // 检查选中的对象是否为 Hud_Element 类型
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /RightUp(右上)", priority = 500)]
        private static void PlaceToAnchor_RU()
        {
            SendObject(XHudAnchor.右上);
        }

        ///-------------------------------------------------------------RightDown

        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /RightDown(右下)", priority = 500, validate = true)]
        private static bool Validate_PlaceToAnchor_RD()
        {
            // 获取当前选中的对象
            GameObject selectedObject = Selection.activeObject as GameObject;
            if (selectedObject != null)
            {
                XHud_Module_Element element = selectedObject.GetComponent<XHud_Module_Element>();
                if (element != null)
                {
                    // 检查选中的对象是否为 Hud_Element 类型
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /RightDown(右下)", priority = 500)]
        private static void PlaceToAnchor_RD()
        {
            SendObject(XHudAnchor.右下);
        }

        ///-------------------------------------------------------------World

        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /World(世界)", priority = 500, validate = true)]
        private static bool Validate_PlaceToAnchor_World()
        {
            // 获取当前选中的对象
            GameObject selectedObject = Selection.activeObject as GameObject;
            if (selectedObject != null)
            {
                XHud_Module_Element element = selectedObject.GetComponent<XHud_Module_Element>();
                if (element != null)
                {
                    // 检查选中的对象是否为 Hud_Element 类型
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        [MenuItem("Assets/XHud/PlaceToAnchor (放置到锚点) /World(世界)", priority = 500)]
        private static void PlaceToAnchor_World()
        {
            SendObject_World();
        }
    }
}