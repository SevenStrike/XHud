namespace SevenStrikeModules.XHud.Utilitys
{
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.GuiLib;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    public class Editor_XHud_Tool_RotatorStructureCreator : EditorWindow
    {
        private static Editor_XHud_Tool_RotatorStructureCreator Window;

        public RectTransform Rot_Center;
        public RectTransform[] Rot_Objects;

        public Vector2 ListScroll;

        SerializedObject so;
        SerializedProperty sp_center;

        [MenuItem("Tools/XHud/RotatorStructureCreator #r")]
        static void Init()
        {
            Window = (Editor_XHud_Tool_RotatorStructureCreator)EditorWindow.GetWindow(typeof(Editor_XHud_Tool_RotatorStructureCreator), false, "创建XHUD旋转性结构", true);
            Window.minSize = new Vector2(350, 500);
            Window.Show();
        }

        private void OnDisable()
        {

        }

        private void OnEnable()
        {

        }

        private void OnGUI()
        {
            so = new SerializedObject(Window);
            sp_center = so.FindProperty("Rot_Center");

            so.Update();
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.无, HudColor.无);
            Editor_XHud_GUI.Gui_Layout_Space(5);

            #region 元素动效模版名称
            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Property_Field("旋转中心", sp_center, 60);
            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Horizontal_End();
            #endregion

            #region 拖放操作

            Rect dragarea = GUILayoutUtility.GetLastRect();
            Rect drag = new Rect((dragarea.width / 2) - 150, dragarea.y + 30, 300, 33);

            Event evt = Event.current;
            if (drag.Contains(evt.mousePosition))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    List<RectTransform> recttransforms = new List<RectTransform>();
                    DragAndDrop.AcceptDrag();
                    Object[] dropobjs = DragAndDrop.objectReferences;
                    string names = "";
                    foreach (var item in dropobjs)
                    {
                        GameObject el = (GameObject)item;
                        RectTransform rect = el.GetComponent<RectTransform>();
                        recttransforms.Add(rect);
                    }
                    Rot_Objects = recttransforms.ToArray();
                    EditorUtility.DisplayDialog("提示", "所选图形 : \n\n" + names + "\n已添加到列表中！", "明白");
                }
            }

            Editor_XHud_GUI.Gui_Box_Style(drag, HudFilled.实体, HudColor.深空灰);
            Editor_XHud_GUI.Gui_Labelfield(drag, "拖放旋转图形到此处", HudFilled.无, HudColor.无, Color.gray, TextAnchor.MiddleCenter, new Vector2(0, 0), 11);
            #endregion

            Editor_XHud_GUI.Gui_Layout_Space(45);

            if (Rot_Objects != null)
            {
                Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_XHud_GUI.Gui_Layout_Space(10);
                // 创建滚动视图
                ListScroll = GUILayout.BeginScrollView(ListScroll);
                // 显示 Transform 列表
                for (int i = 0; i < Rot_Objects.Length; i++)
                {
                    EditorGUILayout.ObjectField("Element " + i, Rot_Objects[i], typeof(RectTransform), true);
                }

                GUILayout.EndScrollView();
                Editor_XHud_GUI.Gui_Layout_Space(10);
                Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                #region 创建按钮
                Editor_XHud_GUI.Gui_Layout_Space(10);
                Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_XHud_GUI.Gui_Layout_Space(10);
                if (Editor_XHud_GUI.Gui_Layout_Button("创建旋转结构", "将所有旋转图形以旋转中心为中心点创建UI旋转体结构", HudFilled.实体, HudColor.工业蓝, Color.black, 35, new RectOffset(), new Vector2(0, 0)))
                {
                    for (int i = 0; i < Rot_Objects.Length; i++)
                    {
                        // 将目标位置转换为目标RectTransform的局部坐标系中的位置
                        Vector2 positionInTarget = Rot_Objects[i].InverseTransformPoint(Rot_Center.position);

                        // 获取目标RectTransform的宽度和高度
                        float width = Rot_Objects[i].rect.width;
                        float height = Rot_Objects[i].rect.height;

                        // 计算新的Pivot
                        Vector2 newPivot = new Vector2(positionInTarget.x / width + 0.5f, positionInTarget.y / height + 0.5f);

                        // 设置RectTransform的新Pivot
                        Rot_Objects[i].pivot = newPivot;
                        Rot_Objects[i].anchoredPosition = Vector3.zero;
                    }
                }
                Editor_XHud_GUI.Gui_Layout_Space(10);
                Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                Editor_XHud_GUI.Gui_Layout_Space(5);
                #endregion
            }

            Editor_XHud_GUI.Gui_Layout_Space(10);

            #region 选中信息
            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Labelfield("提示： 请将旋转中心和所有旋转物体放在同一层级下", HudFilled.无, HudColor.无, Color.gray, TextAnchor.MiddleCenter, 11);
            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Horizontal_End();
            Editor_XHud_GUI.Gui_Layout_Space(5);
            #endregion

            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();

            so.ApplyModifiedProperties();
            //this.Close();
        }
    }
}