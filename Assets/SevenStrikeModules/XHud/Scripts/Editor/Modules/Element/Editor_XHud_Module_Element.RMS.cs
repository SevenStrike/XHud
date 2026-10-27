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
    using SevenStrikeModules.XGUI.Editor;
    using SevenStrikeModules.XGUI.Runtime;
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
            //--判断当前元素是否在场景中
            if (!IsInPrefabStageMode())
            {
                if (!string.IsNullOrEmpty(base_script.gameObject.scene.name))
                {
                    if (sp_rms_enabled.boolValue)
                    {
                        #region 以Hud管理器中指定的RMS布局方案来对此元素进行坐标信息复位

                        string solution = hud_manager.hm_RMS_GetCurrentSolution();

                        if (Targets_Selected())
                        {
                            for (int i = 0; i < selected_objects.Length; i++)
                            {
                                RMS_Preview(selected_objects[i], selected_objects[i].alpha, Vector3.zero, solution, true);
                            }
                        }
                        else
                        {
                            RMS_Preview(base_script, sp_alpha.floatValue, Vector3.zero, solution, true);
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
            for (int i = 0; i < sp_rms_layout_datas.arraySize; i++)
            {
                SerializedProperty sp_name = sp_rms_layout_datas.GetArrayElementAtIndex(i).FindPropertyRelative(nameof(Element_RMS_LayoutData.layout_name));

                if (sp_name.stringValue == sp_rms_name.stringValue)
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
            string ParentName = base_script.transform.parent.name;

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

            info.anchor = anchor;
            info.anchor_min = base_script.rect_transform.anchorMin;
            info.anchor_max = base_script.rect_transform.anchorMax;
            info.pivot = base_script.rect_transform.pivot;
            info.position = base_script.rect_transform.anchoredPosition3D;
            info.euler = base_script.rect_transform.localEulerAngles;
            info.scale = base_script.rect_transform.localScale;

            return info;
        }

        /// <summary>
        /// RMS设置
        /// </summary>
        /// <param name="property"></param>
        private void RMS_Set(SerializedProperty property)
        {
            SerializedProperty sp_anchor = property.FindPropertyRelative(nameof(Element_RMS_LayoutData.anchor));
            SerializedProperty sp_position = property.FindPropertyRelative(nameof(Element_RMS_LayoutData.position));
            SerializedProperty sp_euler = property.FindPropertyRelative(nameof(Element_RMS_LayoutData.euler));
            SerializedProperty sp_scale = property.FindPropertyRelative(nameof(Element_RMS_LayoutData.scale));
            SerializedProperty sp_anchor_min = property.FindPropertyRelative(nameof(Element_RMS_LayoutData.anchor_min));
            SerializedProperty sp_anchor_max = property.FindPropertyRelative(nameof(Element_RMS_LayoutData.anchor_max));
            SerializedProperty sp_pivot = property.FindPropertyRelative(nameof(Element_RMS_LayoutData.pivot));
            SerializedProperty sp_layout_name = property.FindPropertyRelative(nameof(Element_RMS_LayoutData.layout_name));

            Element_RMS_LayoutData layout_info = RMS_GetCurrentInfo();

            sp_anchor.enumValueIndex = (int)layout_info.anchor;
            sp_position.vector3Value = layout_info.position;
            sp_euler.vector3Value = layout_info.euler;
            sp_scale.vector3Value = layout_info.scale;
            sp_anchor_min.vector2Value = layout_info.anchor_min;
            sp_anchor_max.vector2Value = layout_info.anchor_max;
            sp_pivot.vector2Value = layout_info.pivot;
            sp_layout_name.stringValue = sp_rms_name.stringValue;

            property.serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// RMS记录
        /// </summary>
        private void RMS_Record()
        {
            if (!hud_manager.rms_enabled)
            {
                XGUI.dialog(
                           type: XGUIDialogType.警告,
                           windowtitle: "XHud - 元素消息",
                           title: "RMS记录布局",
                           msg: $"XHud管理器中RMS未开启！无法执行记录布局信息操作！",
                           ok: "明白",
                           PrimaryIndex: 0,
                           usemodal: true,
                           themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));

                return;
            }

            if (hud_manager.hm_RMS_IsEmpty())
            {
                string res = XGUI.dialog(
                    type: XGUIDialogType.警告,
                    windowtitle: "XHud - 元素消息",
                    title: "RMS记录布局",
                    msg: $"并未发现您在 XHudManager 里配置RMS信息！请先配置RMS列表！",
                    ok: "去配置",
                    cancel: "暂不",
                    PrimaryIndex: 0,
                    usemodal: true,
                    themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));

                if (res == "去配置")
                {
                    Selection.activeGameObject = hud_manager.gameObject;
                    EditorGUIUtility.PingObject(hud_manager);
                    return;
                }
                if (res == "暂不")
                {
                    return;
                }
            }

            if (Targets_Selected())
            {
                XGUI.dialog(
                    type: XGUIDialogType.警告,
                    windowtitle: "XHud - 元素消息",
                    title: "RMS记录布局",
                    msg: "因为考虑到每个元素当前设计布局可能不一致，因此不支持批量记录设计布局信息！",
                    ok: "明白",
                    PrimaryIndex: 0,
                    usemodal: true,
                    themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));

                return;
            }
            else
            {
                if (!hud_manager.rms_enabled)
                    return;

                if (sp_rms_layout_datas.arraySize <= 0)
                {
                    sp_rms_layout_datas.InsertArrayElementAtIndex(sp_rms_layout_datas.arraySize);
                    SerializedProperty newLayoutInfo = sp_rms_layout_datas.GetArrayElementAtIndex(sp_rms_layout_datas.arraySize - 1);
                    RMS_Set(newLayoutInfo);
                }
                else
                {
                    bool IsExist = false;
                    for (int i = 0; i < sp_rms_layout_datas.arraySize; i++)
                    {
                        SerializedProperty sp_list_cur_solution = sp_rms_layout_datas.GetArrayElementAtIndex(i);
                        SerializedProperty sp_list_cur_solution_name = sp_list_cur_solution.FindPropertyRelative(nameof(Element_RMS_LayoutData.layout_name));

                        if (sp_list_cur_solution_name.stringValue == sp_rms_name.stringValue)
                        {
                            RMS_Set(sp_list_cur_solution);
                            IsExist = true;
                            break;
                        }
                    }
                    if (!IsExist)
                    {
                        sp_rms_layout_datas.InsertArrayElementAtIndex(sp_rms_layout_datas.arraySize);
                        SerializedProperty newLayoutInfo = sp_rms_layout_datas.GetArrayElementAtIndex(sp_rms_layout_datas.arraySize - 1);
                        RMS_Set(newLayoutInfo);
                    }
                }
            }

            XGUI.dialog(
                type: XGUIDialogType.警告,
                windowtitle: "XHud - 元素消息",
                title: "RMS记录布局",
                msg: $"已记录 {sp_rms_name.stringValue} 设计布局信息！",
                ok: "明白",
                PrimaryIndex: 0,
                usemodal: true,
                themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
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
            hud_manager.hm_ScreenElement_Initialize_By_RMS(element, solution, DontCreateID);
        }
    }
}