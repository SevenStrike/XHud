namespace SevenStrikeModules.XHud.Hud
{
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.GuiLib;
    using UnityEditor;
    using UnityEngine;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(xHud_ObjectTracker), true)]
    public class Editor_xHud_ObjectTracker : Editor
    {
        private xHud_ObjectTracker BaseScript;

        private SerializedProperty
            SelfObject,
            RelativeObject,
            TargetObject,
            UseSmoothTracker,
            TrackerOffset,
            SmoothTime;

        private Texture2D icon_main;

        #region 批量化操作
        private xHud_ObjectTracker[] SelectedObjects;

        private void GetAllTargets()
        {
            if (targets.Length > 1)
            {
                SelectedObjects = new xHud_ObjectTracker[targets.Length];
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    var t = targets[i];
                    SelectedObjects[i] = (xHud_ObjectTracker)t;
                }
            }
            else
            {
                SelectedObjects = new xHud_ObjectTracker[targets.Length];
                SelectedObjects[0] = (xHud_ObjectTracker)target;
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
            BaseScript = (xHud_ObjectTracker)target;

            SelfObject = serializedObject.FindProperty("SelfObject");
            RelativeObject = serializedObject.FindProperty("RelativeObject");
            TargetObject = serializedObject.FindProperty("TargetObject");
            UseSmoothTracker = serializedObject.FindProperty("UseSmoothTracker");
            SmoothTime = serializedObject.FindProperty("SmoothTime");
            TrackerOffset = serializedObject.FindProperty("TrackerOffset");

            icon_main = Editor_xHudGUI.GetIcon("Icons_Hud_ObjectTracker/icon_main");

            GetAllTargets();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            Editor_xHudGUI.Gui_Layout_Banner(icon_main, HudFilled.实体, HudColor.深空灰, "Hud - 物体追踪器", Color.white);

            Editor_xHudGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "参数", xHud_Dashboard.Theme_Primary);

            Editor_xHudGUI.Gui_Layout_Space(5);

            Editor_xHudGUI.Gui_Layout_Toggle<bool, xHud_ObjectTracker>("使用平滑追踪", new string[] { "禁用", "启用" }, ref UseSmoothTracker, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

            Editor_xHudGUI.Gui_Layout_Space(5);

            Editor_xHudGUI.Gui_Layout_Property_Field("Hud元素", SelfObject, 90);

            Editor_xHudGUI.Gui_Layout_Space(5);

            Editor_xHudGUI.Gui_Layout_Property_Field("Hud元素父物体", RelativeObject, 90);

            Editor_xHudGUI.Gui_Layout_Space(5);

            Editor_xHudGUI.Gui_Layout_Property_Field("场景目标物体", TargetObject, 90);

            Editor_xHudGUI.Gui_Layout_Space(5);

            if (UseSmoothTracker.boolValue)
            {
                Editor_xHudGUI.Gui_Layout_Property_Field("平滑速率", SmoothTime);
            }

            Editor_xHudGUI.Gui_Layout_Property_Field("位置偏移", TrackerOffset);

            Editor_xHudGUI.Gui_Layout_Space(10);
            Editor_xHudGUI.Gui_Layout_Vertical_End();


            serializedObject.ApplyModifiedProperties();
        }

    }
}