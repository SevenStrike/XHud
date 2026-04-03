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

    [CanEditMultipleObjects]
    [CustomEditor(typeof(XHud_ObjectTracker), true)]
    public class Editor_XHud_ObjectTracker : Editor
    {
        private XHud_ObjectTracker BaseScript;

        private SerializedProperty
            SelfObject,
            RelativeObject,
            TargetObject,
            UseSmoothTracker,
            TrackerOffset,
            SmoothTime;
        private bool OriginalDisplay;
        private Texture2D icon_main;

        #region 批量化操作
        private XHud_ObjectTracker[] SelectedObjects;

        private void GetAllTargets()
        {
            if (targets.Length > 1)
            {
                SelectedObjects = new XHud_ObjectTracker[targets.Length];
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    var t = targets[i];
                    SelectedObjects[i] = (XHud_ObjectTracker)t;
                }
            }
            else
            {
                SelectedObjects = new XHud_ObjectTracker[targets.Length];
                SelectedObjects[0] = (XHud_ObjectTracker)target;
            }
        }

        private bool IsMultiSelected()
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

        void OnEnable()
        {
            BaseScript = (XHud_ObjectTracker)target;

            SelfObject = serializedObject.FindProperty("SelfObject");
            RelativeObject = serializedObject.FindProperty("RelativeObject");
            TargetObject = serializedObject.FindProperty("TargetObject");
            UseSmoothTracker = serializedObject.FindProperty("UseSmoothTracker");
            SmoothTime = serializedObject.FindProperty("SmoothTime");
            TrackerOffset = serializedObject.FindProperty("TrackerOffset");

            icon_main = Editor_XHud_GUI.GetIcon("Icons_XHud_ObjectTracker/icon_main");

            GetAllTargets();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            Editor_XHud_GUI.Gui_Layout_Banner(icon_main, HudFilled.实体, HudColor.深空灰, "XHud - 物体追踪器", Color.white);

            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "参数", XHud_Dashboard.Theme_Primary);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_ObjectTracker>("使用平滑追踪", new string[] { "禁用", "启用" }, ref UseSmoothTracker, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("XHud元素", SelfObject, 90);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("XHud元素父物体", RelativeObject, 90);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("场景目标物体", TargetObject, 90);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            if (UseSmoothTracker.boolValue)
            {
                Editor_XHud_GUI.Gui_Layout_Property_Field("平滑速率", SmoothTime);
            }

            Editor_XHud_GUI.Gui_Layout_Property_Field("位置偏移", TrackerOffset);

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

    }
}