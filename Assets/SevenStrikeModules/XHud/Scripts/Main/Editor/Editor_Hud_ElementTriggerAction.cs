namespace SevenStrikeModules.XHud.Hud
{
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.GuiLib;
    using UnityEditor;
    using UnityEditor.EventSystems;
    using UnityEngine;
    using UnityEngine.UI;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(Hud_ElementTriggerAction), true)]
    public class Editor_Hud_ElementTriggerAction : EventTriggerEditor
    {
        private Hud_ElementTriggerAction BaseScript;

        private bool BasicVars;

        #region 序列化属性
        private SerializedProperty AutoClearActionsAndEvents;
        #endregion

        #region 图标
        private Texture2D icon_main;
        #endregion

        #region 批量化操作
        private Hud_ElementTriggerAction[] SelectedObjects;

        private void GetAllTargets()
        {
            if (targets.Length > 1)
            {
                SelectedObjects = new Hud_ElementTriggerAction[targets.Length];
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    var t = targets[i];
                    SelectedObjects[i] = (Hud_ElementTriggerAction)t;
                }
            }
            else
            {
                SelectedObjects = new Hud_ElementTriggerAction[targets.Length];
                SelectedObjects[0] = (Hud_ElementTriggerAction)target;
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

            BaseScript = (Hud_ElementTriggerAction)target;

            AutoClearActionsAndEvents = serializedObject.FindProperty("AutoClearActionsAndEvents");

            if (BaseScript.TriggerImage == null)
                BaseScript.TriggerImage = BaseScript.GetComponent<Image>();
            if (BaseScript.HudElement == null)
                BaseScript.HudElement = BaseScript.GetComponent<Hud_Element>();

            icon_main = util_XHUDGUI.GetIcon("Icons_Hud_ElementTriggerAction/icon_main");

            Vector2 ButtonSize = new Vector2(18, 18);

            GetAllTargets();
        }

        private void OnDisable()
        {

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            util_XHUDGUI.Gui_Layout_Banner(icon_main, HudFilled.实体, HudColor.深空灰, "Hud - 元素动作器", Color.white);

            util_XHUDGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "参数", util_Dashboard.Theme_Primary);

            #region 控制
            util_XHUDGUI.Gui_Layout_Vertical_Start(HudFilled.无, HudColor.无);

            if (Application.isPlaying)
            {
                util_XHUDGUI.Gui_Layout_Labelfield("程序运行中无法操作", HudFilled.无, HudColor.无, Color.gray, TextAnchor.MiddleCenter);
            }
            else
            {
                if (BaseScript.TriggerImage != null)
                    BaseScript.TriggerImage.color = Color.clear;
            }

            util_XHUDGUI.Gui_Layout_Toggle<bool, Hud_ElementTriggerAction>("回收时清空事件和委托", new string[2] { "禁用", "启用" }, ref AutoClearActionsAndEvents, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
            util_XHUDGUI.Gui_Layout_Vertical_End();
            #endregion

            util_XHUDGUI.Gui_Layout_Space(5);
            util_XHUDGUI.Gui_Layout_Seperator(1, util_Dashboard.Theme_SeperateLine);
            util_XHUDGUI.Gui_Layout_Space(5);

            util_XHUDGUI.Gui_Layout_Space(10);
            util_XHUDGUI.Gui_Layout_Vertical_End();


            #region 源脚本
            util_XHUDGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 3, "源脚本", util_Dashboard.Theme_Primary);
            util_XHUDGUI.Gui_Layout_Space(5);

            #region 原始变量
            util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            util_XHUDGUI.Gui_Layout_Space(10);
            BasicVars = EditorGUILayout.Foldout(BasicVars, "变量/属性", true);
            util_XHUDGUI.Gui_Layout_Space(5);
            util_XHUDGUI.Gui_Layout_Horizontal_End();
            if (BasicVars)
                DrawDefaultInspector();
            #endregion

            util_XHUDGUI.Gui_Layout_Space(5);
            util_XHUDGUI.Gui_Layout_Vertical_End();
            #endregion

            serializedObject.ApplyModifiedProperties();
        }
    }
}