namespace SevenStrikeModules.XHud.Editor
{
    using SevenStrikeModules.XHud.Enums;
    using UnityEditor;
    using UnityEditor.EventSystems;
    using UnityEngine;
    using UnityEngine.UI;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(XHud_Element_TriggerAction), true)]
    public class Editor_XHud_Element_TriggerAction : EventTriggerEditor
    {
        private XHud_Element_TriggerAction BaseScript;

        private bool OriginalDisplay;

        #region 序列化属性
        private SerializedProperty AutoClearActionsAndEvents;
        #endregion

        #region 图标
        private Texture2D icon_main;
        #endregion

        #region 批量化操作
        private XHud_Element_TriggerAction[] SelectedObjects;

        private void GetAllTargets()
        {
            if (targets.Length > 1)
            {
                SelectedObjects = new XHud_Element_TriggerAction[targets.Length];
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    var t = targets[i];
                    SelectedObjects[i] = (XHud_Element_TriggerAction)t;
                }
            }
            else
            {
                SelectedObjects = new XHud_Element_TriggerAction[targets.Length];
                SelectedObjects[0] = (XHud_Element_TriggerAction)target;
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

        protected override void OnEnable()
        {
            base.OnEnable();

            BaseScript = (XHud_Element_TriggerAction)target;

            AutoClearActionsAndEvents = serializedObject.FindProperty("AutoClearActionsAndEvents");

            if (BaseScript.TriggerImage == null)
                BaseScript.TriggerImage = BaseScript.GetComponent<Image>();
            if (BaseScript.HudElement == null)
                BaseScript.HudElement = BaseScript.GetComponent<XHud_Module_Element>();

            icon_main = Editor_XHud_GUI.GetIcon("Icons_XHud_ElementTriggerAction/icon_main");

            Vector2 ButtonSize = new Vector2(18, 18);

            GetAllTargets();
        }

        private void OnDisable()
        {

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            Editor_XHud_GUI.Gui_Layout_Banner(icon_main, HudFilled.实体, HudColor.深空灰, "XHud - 元素动作器", Color.white);

            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "参数", XHud_Dashboard.Theme_Primary);

            #region 控制
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.无, HudColor.无);

            if (Application.isPlaying)
            {
                Editor_XHud_GUI.Gui_Layout_Labelfield("程序运行中无法操作", HudFilled.无, HudColor.无, Color.gray, TextAnchor.MiddleCenter);
            }
            else
            {
                if (BaseScript.TriggerImage != null)
                    BaseScript.TriggerImage.color = Color.clear;
            }

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Element_TriggerAction>("回收时清空事件和委托", new string[2] { "禁用", "启用" }, ref AutoClearActionsAndEvents, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Seperator(1, XHud_Dashboard.Theme_SeperateLine);
            Editor_XHud_GUI.Gui_Layout_Space(5);

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