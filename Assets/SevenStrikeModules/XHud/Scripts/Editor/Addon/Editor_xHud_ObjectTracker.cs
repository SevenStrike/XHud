namespace SevenStrikeModules.XHud
{
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.GuiLib;
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


            serializedObject.ApplyModifiedProperties();
        }

    }
}