namespace SevenStrikeModules.XHud
{
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.GuiLib;
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
