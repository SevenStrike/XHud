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
    using UnityEditor;
    using UnityEngine;

    public partial class Editor_XHud_Module_Element : Editor
    {
        /// <summary>
        /// 根据当前元素所选定的布局方案进行布局重绘
        /// </summary>
        private void RMS_Redraw()
        {
            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

            //--判断当前元素是否在场景中
            if (!IsInPrefabStageMode())
            {
                if (!string.IsNullOrEmpty(BaseScript.gameObject.scene.name))
                {
                    if (RMS_Enabled.boolValue)
                    {
                        #region 以Hud管理器中指定的RMS布局方案来对此元素进行坐标信息复位

                        string solution = mgr.hm_RMS_GetCurrentSolution();

                        if (Targets_Selected())
                        {
                            for (int i = 0; i < SelectedObjects.Length; i++)
                            {
                                RMS_Preview(SelectedObjects[i], SelectedObjects[i].Alpha, Vector3.zero, solution, true);
                            }
                        }
                        else
                        {
                            RMS_Preview(BaseScript, Alpha.floatValue, Vector3.zero, solution, true);
                        }
                        #endregion
                    }
                }
            }
        }

        /// <summary>
        /// 布局信息是否存在
        /// </summary>
        /// <returns></returns>
        private bool RMS_IsExist()
        {
            bool isExist = false;
            for (int i = 0; i < RMS_LayoutDatas.arraySize; i++)
            {
                SerializedProperty sp_name = RMS_LayoutDatas.GetArrayElementAtIndex(i).FindPropertyRelative("LayoutName");

                if (sp_name.stringValue == RMS_Name.stringValue)
                {
                    isExist = true;
                    break;
                }
            }
            return isExist;
        }

        /// <summary>
        /// 获取当前元素的RMS状态信息
        /// </summary>
        /// <returns></returns>
        private Element_RMS_LayoutData RMS_GetCurrentInfo()
        {
            Element_RMS_LayoutData info = new Element_RMS_LayoutData();

            XHudAnchor anchor = XHudAnchor.中心;
            string ParentName = BaseScript.transform.parent.name;

            if (ParentName == "Anchor_B")
            {
                anchor = XHudAnchor.底层;
            }
            if (ParentName == "Anchor_C")
            {
                anchor = XHudAnchor.中心;
            }
            if (ParentName == "Anchor_L")
            {
                anchor = XHudAnchor.左;
            }
            if (ParentName == "Anchor_R")
            {
                anchor = XHudAnchor.右;
            }
            if (ParentName == "Anchor_U")
            {
                anchor = XHudAnchor.上;
            }
            if (ParentName == "Anchor_D")
            {
                anchor = XHudAnchor.下;
            }
            if (ParentName == "Anchor_L_U")
            {
                anchor = XHudAnchor.左上;
            }
            if (ParentName == "Anchor_L_D")
            {
                anchor = XHudAnchor.左下;
            }
            if (ParentName == "Anchor_R_U")
            {
                anchor = XHudAnchor.右上;
            }
            if (ParentName == "Anchor_R_D")
            {
                anchor = XHudAnchor.右下;
            }
            if (ParentName == "Anchor_T")
            {
                anchor = XHudAnchor.顶层;
            }

            info.Anchor = anchor;
            info.AnchorMin = BaseScript.RectTransform.anchorMin;
            info.AnchorMax = BaseScript.RectTransform.anchorMax;
            info.Pivot = BaseScript.RectTransform.pivot;
            info.Position = BaseScript.RectTransform.anchoredPosition3D;
            info.Euler = BaseScript.RectTransform.localEulerAngles;
            info.Scale = BaseScript.RectTransform.localScale;

            return info;
        }

        /// <summary>
        /// RMS设置
        /// </summary>
        /// <param name="property"></param>
        private void RMS_Set(SerializedProperty property)
        {
            SerializedProperty sp_Anchor = property.FindPropertyRelative("Anchor");
            SerializedProperty sp_Position = property.FindPropertyRelative("Position");
            SerializedProperty sp_Euler = property.FindPropertyRelative("Euler");
            SerializedProperty sp_Scale = property.FindPropertyRelative("Scale");
            SerializedProperty sp_AnchorMin = property.FindPropertyRelative("AnchorMin");
            SerializedProperty sp_AnchorMax = property.FindPropertyRelative("AnchorMax");
            SerializedProperty sp_Pivot = property.FindPropertyRelative("Pivot");
            SerializedProperty sp_LayoutName = property.FindPropertyRelative("LayoutName");

            Element_RMS_LayoutData layout_info = RMS_GetCurrentInfo();

            sp_Anchor.enumValueIndex = (int)layout_info.Anchor;
            sp_Position.vector3Value = layout_info.Position;
            sp_Euler.vector3Value = layout_info.Euler;
            sp_Scale.vector3Value = layout_info.Scale;
            sp_AnchorMin.vector2Value = layout_info.AnchorMin;
            sp_AnchorMax.vector2Value = layout_info.AnchorMax;
            sp_Pivot.vector2Value = layout_info.Pivot;
            sp_LayoutName.stringValue = RMS_Name.stringValue;

            property.serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// RMS记录
        /// </summary>
        private void RMS_Record()
        {
            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

            if (!mgr.RMS_Enabled)
            {
                Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 元素消息", "RMS记录布局", "Hud管理器中RMS未开启！无法执行记录布局信息操作！", "明白");
                return;
            }

            if (mgr.hm_RMS_IsEmpty())
            {
                string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 元素消息", "RMS记录布局", "并未发现您在HudManager里配置RMS信息！请先配置RMS列表！", "去配置", "暂不", 0);
                if (res == "去配置")
                {
                    Selection.activeGameObject = mgr.gameObject;
                    EditorGUIUtility.PingObject(mgr);
                    return;
                }
                if (res == "暂不")
                {
                    return;
                }
            }

            if (Targets_Selected())
            {
                Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 元素消息", "RMS批量记录", "因为考虑到每个元素当前设计布局可能不一致，因此不支持批量记录设计布局信息！", "明白");
                return;
            }
            else
            {
                if (!mgr.RMS_Enabled)
                    return;

                if (RMS_LayoutDatas.arraySize <= 0)
                {
                    RMS_LayoutDatas.InsertArrayElementAtIndex(RMS_LayoutDatas.arraySize);
                    SerializedProperty newLayoutInfo = RMS_LayoutDatas.GetArrayElementAtIndex(RMS_LayoutDatas.arraySize - 1);
                    RMS_Set(newLayoutInfo);
                }
                else
                {
                    bool IsExist = false;
                    for (int i = 0; i < RMS_LayoutDatas.arraySize; i++)
                    {
                        SerializedProperty sp_list_cur_solution = RMS_LayoutDatas.GetArrayElementAtIndex(i);
                        SerializedProperty sp_list_cur_solution_name = sp_list_cur_solution.FindPropertyRelative("LayoutName");

                        if (sp_list_cur_solution_name.stringValue == RMS_Name.stringValue)
                        {
                            RMS_Set(sp_list_cur_solution);
                            IsExist = true;
                            break;
                        }
                    }
                    if (!IsExist)
                    {
                        RMS_LayoutDatas.InsertArrayElementAtIndex(RMS_LayoutDatas.arraySize);
                        SerializedProperty newLayoutInfo = RMS_LayoutDatas.GetArrayElementAtIndex(RMS_LayoutDatas.arraySize - 1);
                        RMS_Set(newLayoutInfo);
                    }
                }
            }

            Editor_XHud_GUI.Open(XHud_DialogType.确认, "XHud - 元素消息", "RMS记录布局", "已记录  \"" + RMS_Name.stringValue + "\"  设计布局信息！", "明白");
        }

        /// <summary>
        /// RMS 预览
        /// </summary>
        /// <param name="element"></param>
        /// <param name="alpha"></param>
        /// <param name="offset"></param>
        /// <param name="solution"></param>
        /// <param name="DontCreateID"></param>
        private void RMS_Preview(XHud_Module_Element element, float alpha, Vector3 offset, string solution, bool DontCreateID)
        {
            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();
            mgr.hm_ScreenElement_Initialize_By_RMS(element, alpha, offset, solution, DontCreateID);
        }
    }
}