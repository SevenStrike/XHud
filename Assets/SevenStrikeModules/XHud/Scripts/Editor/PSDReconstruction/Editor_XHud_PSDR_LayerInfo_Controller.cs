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
    using System;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(XHud_PSDR_LayerInfo_Controller), true)]
    public class Editor_XHud_PSDR_LayerInfo_Controller : Editor
    {
        private XHud_PSDR_LayerInfo_Controller BaseScript;

        private SerializedProperty
            LayerInfos;

        private bool OriginalDisplay;
        private Texture2D icon_main;

        #region 批量化操作
        private XHud_PSDR_LayerInfo_Controller[] SelectedObjects;

        private void Targets_Get()
        {
            if (targets.Length > 1)
            {
                SelectedObjects = new XHud_PSDR_LayerInfo_Controller[targets.Length];
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    var t = targets[i];
                    SelectedObjects[i] = (XHud_PSDR_LayerInfo_Controller)t;
                }
            }
            else
            {
                SelectedObjects = new XHud_PSDR_LayerInfo_Controller[targets.Length];
                SelectedObjects[0] = (XHud_PSDR_LayerInfo_Controller)target;
            }
        }

        private bool Targets_Selected()
        {
            if (SelectedObjects == null)
                return false;
            if (SelectedObjects.Length > 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        private int count_layer;
        private int count_group;
        private int count_lay_shp;
        private int count_lay_pixel;
        private int count_lay_smt;
        private int count_lay_txt;

        void OnEnable()
        {
            BaseScript = (XHud_PSDR_LayerInfo_Controller)target;

            LayerInfos = serializedObject.FindProperty("LayerInfos");

            icon_main = Editor_XHud_GUI.GetIcon("Icons_XHud_PSDR_LayerInfo_Controller/icon_main");

            Targets_Get();
        }

        private void OnDisable()
        {

        }

        public override void OnInspectorGUI()
        {
            Get_AllLayerInfos();

            count_lay_shp = Get_Statistic(XHud_PSDR_LayerType.shp);
            count_lay_pixel = Get_Statistic(XHud_PSDR_LayerType.pix);
            count_lay_smt = Get_Statistic(XHud_PSDR_LayerType.smt);
            count_lay_txt = Get_Statistic(XHud_PSDR_LayerType.txt);

            count_layer = count_lay_shp + count_lay_pixel + count_lay_smt + count_lay_txt;
            count_group = Get_Statistic(XHud_PSDR_LayerType.group);

            serializedObject.Update();

            Editor_XHud_GUI.Gui_Layout_Banner(icon_main, HudFilled.实体, HudColor.深空灰, "XHud - 图层重建控制器", Color.white);

            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "状态", XHud_Dashboard.Theme_Primary);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            // 图层总计
            Editor_XHud_GUI.StatuDisplayer_text(null, 12, new Vector2(0, 7), "总计节点数量：", 12, $"{count_layer + count_group}", XHud_Dashboard.Theme_Primary, 12, false);

            // 图层总计
            Editor_XHud_GUI.StatuDisplayer_text(null, 12, new Vector2(0, 7), "编组节点数量：", 12, $"{count_group}", XHud_Dashboard.Theme_Primary, 12, false);

            Editor_XHud_GUI.Gui_Layout_Seperator(1, XHud_Dashboard.Theme_SeperateLine);

            // 图层总计
            Editor_XHud_GUI.StatuDisplayer_text(null, 12, new Vector2(0, 7), "( 像素 ) 图层节点数量：", 12, $"{count_lay_pixel}", XHud_Dashboard.Theme_Primary, 12, false);

            // 图层总计
            Editor_XHud_GUI.StatuDisplayer_text(null, 12, new Vector2(0, 7), "( 形状 ) 图层节点数量：", 12, $"{count_lay_shp}", XHud_Dashboard.Theme_Primary, 12, false);

            // 图层总计
            Editor_XHud_GUI.StatuDisplayer_text(null, 12, new Vector2(0, 7), "( 智能物体 ) 图层节点数量：", 12, $"{count_lay_smt}", XHud_Dashboard.Theme_Primary, 12, false);

            // 图层总计
            Editor_XHud_GUI.StatuDisplayer_text(null, 12, new Vector2(0, 7), "( 文字 ) 图层节点数量：", 12, $"{count_lay_txt}", XHud_Dashboard.Theme_Primary, 12, false);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Seperator(1, XHud_Dashboard.Theme_SeperateLine);

            string hexcol = XHud_Utilitys.Color_To_HexColor(XHud_Dashboard.Theme_Primary, true);

            if (Editor_XHud_GUI.Gui_Layout_Button("所有图层尺寸自适应", "", HudFilled.实体, HudColor.深空灰, Color.white, 30, new RectOffset(), new Vector2(0, 0)))
            {
                EditorApplication.delayCall += () =>
                {
                    string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 图层重建控制器通知", "所有图层尺寸自适应", $"此操作会将<color={hexcol}> 所有图层的Image组件 </color>执行<color={hexcol}> SetNativeSize </color>操作，UI 图片的矩形框大小，自动<color={hexcol}>（修正）调整为图片素材的原始像素尺寸 </color>！", "暂不", "自适应", 0);

                    if (res == "自适应")
                        NativeSizeAllLayer();
                };
            }

            Editor_XHud_GUI.Gui_Layout_Space(5);

            if (Editor_XHud_GUI.Gui_Layout_Button("清理图层信息节点 & 删除此控制器", "", HudFilled.实体, HudColor.魅力红, Color.black, 30, new RectOffset(), new Vector2(0, 0)))
            {
                EditorApplication.delayCall += () =>
                {
                    string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 图层重建控制器通知", "图层信息节点清理", $"此操作会<color={hexcol}> 清空所有图层上 </color>挂载的<color={hexcol}> 图层节点信息脚本</color>，清除之后您将无法再<color={hexcol}> 校验重建的图层物体 </color>的一些<color={hexcol}> 参数 </color>是否和 PhotoShop 中的一致！请谨慎操作！", "暂不", "清理", 0);

                    if (res == "清理")
                        RemoveAllLayerInfo();
                };
            }

            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();

            #region 源脚本
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 3, "源脚本", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(5);

            #region 原始变量
            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_XHud_GUI.Gui_Layout_Space(10);
            OriginalDisplay = EditorGUILayout.Foldout(OriginalDisplay, "变量/属性", true);
            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Horizontal_End();
            if (OriginalDisplay)
                DrawDefaultInspector();
            #endregion

            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            serializedObject.ApplyModifiedProperties();
        }

        private void NativeSizeAllLayer()
        {
            // 开始一个 Undo 组，所有修改可以一次性撤销
            Undo.IncrementCurrentGroup();
            int group = Undo.GetCurrentGroup();

            for (int i = 0; i < BaseScript.LayerInfos.Count; i++)
            {
                PSDR_Layers lay = BaseScript.LayerInfos[i].Layer;
                Graphic gc = lay.graphic;

                if (gc == null)
                    continue;

                Undo.RecordObject(gc.rectTransform, "Set Native Size");
                gc.SetNativeSize();
                EditorUtility.SetDirty(gc.rectTransform);
            }

            // 设置 Undo 组的名称（会显示在 Edit 菜单中）
            Undo.SetCurrentGroupName("Set Native Size for All Layers");
            Undo.CollapseUndoOperations(group); // 合并所有操作为一个步骤
        }

        private void RemoveAllLayerInfo()
        {
            if (BaseScript == null || BaseScript.LayerInfos == null)
                return;

            int removedCount = 0;

            // 从后往前遍历，避免索引问题
            for (int i = BaseScript.LayerInfos.Count - 1; i >= 0; i--)
            {
                var info = BaseScript.LayerInfos[i];

                if (info != null)
                {
                    // 记录撤销操作
                    Undo.DestroyObjectImmediate(info);
                    removedCount++;
                }
                // 无论是否为 null，都从列表中移除
                BaseScript.LayerInfos.RemoveAt(i);
            }

            // 确保列表被清空（防御性编程）
            if (BaseScript.LayerInfos.Count > 0)
                BaseScript.LayerInfos.Clear();

            // 标记对象为脏，确保更改被保存
            EditorUtility.SetDirty(BaseScript);

            // 删除控制器自身（支持撤销）
            if (BaseScript != null)
            {
                Undo.DestroyObjectImmediate(BaseScript);
            }

            string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 图层重建控制器通知", "图层信息节点清理", $"已清理 {removedCount} 个图层信息节点", "明白", 0);
        }

        private int Get_Statistic(XHud_PSDR_LayerType type)
        {
            int val = 0;

            for (int i = 0; i < BaseScript.LayerInfos.Count; i++)
            {
                XHud_PSDR_LayerInfo t = BaseScript.LayerInfos[i];
                if (t.Layer.enum_type == type)
                {
                    val++;
                }
            }

            return val;
        }

        private void Get_AllLayerInfos()
        {
            BaseScript.LayerInfos.Clear();
            BaseScript.LayerInfos = BaseScript.GetComponentsInChildren<XHud_PSDR_LayerInfo>().ToList();
        }
    }
}