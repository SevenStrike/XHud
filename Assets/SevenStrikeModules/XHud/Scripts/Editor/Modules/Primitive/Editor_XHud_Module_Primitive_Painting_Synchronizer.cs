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
    using UnityEditor;
    using UnityEngine;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(XHud_Module_Primitive_Painting_Synchronizer))]
    public class Editor_XHud_Module_Primitive_Painting_Synchronizer : Editor
    {
        #region 组件 / 列表
        private XHud_Module_Primitive_Painting_Synchronizer BaseScript;
        #endregion

        #region 图标
        private Texture2D icon_main;
        #endregion

        #region GUI 参数
        /// <summary>
        /// 原始脚本参数显示开关
        /// </summary>
        private bool OriginalDisplay;
        #endregion

        #region 批量化操作
        /// <summary>
        /// 批量选择脚本数组
        /// </summary>
        Editor_XHud_Module_Primitive_Painting_Synchronizer[] SelectedObjects;
        /// <summary>
        /// 获取所有批量脚本目标
        /// </summary>
        private void Targets_Get()
        {
            if (targets.Length > 1)
            {
                SelectedObjects = new Editor_XHud_Module_Primitive_Painting_Synchronizer[targets.Length];
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    var t = targets[i];
                    SelectedObjects[i] = (Editor_XHud_Module_Primitive_Painting_Synchronizer)t;
                }
            }
            else
            {
                SelectedObjects = new Editor_XHud_Module_Primitive_Painting_Synchronizer[targets.Length];
                SelectedObjects[0] = (Editor_XHud_Module_Primitive_Painting_Synchronizer)target;
            }
        }
        /// <summary>
        /// 判断是否是多选状态
        /// </summary>
        /// <returns></returns>
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

        private void OnEnable()
        {
            BaseScript = (XHud_Module_Primitive_Painting_Synchronizer)target;

            #region 获取图标          
            icon_main = Editor_XHud_GUI.GetIcon("Icons_XHud_Module_Primitive_Painting_Synchronizer/icon_main");
            #endregion
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            #region 标题
            string h_color = XHud_Utilitys.Color_To_HexColor(XHud_Dashboard.Theme_Primary);
            string titlename = "XHud - 图元  >  配色更新器";

            Editor_XHud_GUI.Gui_Layout_Banner(icon_main, HudFilled.实体, HudColor.深空灰, titlename, Color.white, null, "", 20, 20);
            Rect rect = GUILayoutUtility.GetLastRect();
            #endregion

            #region 源脚本
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 3, "源脚本", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(5);

            #region 脚本类
            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_XHud_GUI.Gui_Layout_Space(10);
            OriginalDisplay = EditorGUILayout.Foldout(OriginalDisplay, "脚本类", true);
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
    }
}
