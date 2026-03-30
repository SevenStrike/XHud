namespace SevenStrikeModules.XHud
{
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.GuiLib;
    using SevenStrikeModules.XHud.Utilitys;
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    [SerializeField]
    [System.Serializable]
    public class ElementLibraryInfo
    {
        [SerializeField]
        public string lib_name;
        [SerializeField]
        public string lib_description;
        [SerializeField]
        public int lib_count;
        [SerializeField]
        public int lib_used_count;
        [SerializeField]
        public int lib_recycled_count;
        [SerializeField]
        public int lib_initiated_count;
    }

    public class Editor_XHud_LibrarySetTool_ElementLib : EditorWindow
    {
        private SerializedObject BaseObject;
        private SerializedProperty sp_ElementLibraryInfoList;

        #region 元素库列表
        public List<ElementLibraryInfo> ElementLibraryInfoList = new List<ElementLibraryInfo>();
        /// <summary>
        /// 数据列表
        /// </summary>
        public ReorderableList List_ElementLibraryInfo;
        /// <summary>
        /// Editor列表项高度
        /// </summary>
        public float itemHeight = 25;
        /// <summary>
        /// 可视区域显示的元素数量
        /// </summary>
        public int visibleItemCount = 8;
        /// <summary>
        /// 列表滚动位置
        /// </summary>
        public Vector2 DataList_Scroller;
        /// <summary>
        /// 选中项索引号
        /// </summary>
        public int SelectedLibraryIndex = 0;
        #endregion

        private Texture2D icon_logo;

        #region 字体
        /// <summary>
        /// 字体 - 粗体
        /// </summary>
        Font Font_Bold;
        /// <summary>
        /// 字体 - 细体
        /// </summary>
        Font Font_Light;
        #endregion

        [SerializeField]
        public string DateTimes;

        #region Dialog按钮参数
        /// <summary>
        /// 按钮宽度
        /// </summary>
        private float ButtonWidth = 110;
        /// <summary>
        /// 按钮高度
        /// </summary>
        private float ButtonHeight = 25;
        /// <summary>
        /// 按钮间距
        /// </summary>
        private float ButtonDistance = 15;
        #endregion

        #region Dialog标题颜色
        Color SepLineColor = new Color(1, 1, 1, 0.15f);
        Color MessageColor = new Color(1, 1, 1, 0.62f);
        Color DateTimeColor = new Color(1, 1, 1, 0.42f);
        #endregion

        #region Dialog 控件坐标
        Rect Sepline_rect;
        Rect Title_rect;
        Rect Date_rect;
        Rect Icon_rect;
        Rect TotalCount_rect;
        #endregion

        private void OnDisable()
        {

        }

        private void OnEnable()
        {
            BaseObject = new SerializedObject(this);

            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

            icon_logo = Editor_XHud_GUI.GetIcon("Icons_XHud_Library_ElementLib_Selector/icon");

            #region 获取字体
            Font_Bold = Editor_XHud_GUI.GetFont("SS_Editor_Bold");
            Font_Light = Editor_XHud_GUI.GetFont("SS_Editor_Dialog");
            #endregion

            #region 获取所有已部署到HudManager的元素库
            XHud_Library_Element[] libs = mgr.hm_ElementLibrary_GetArray();

            if (ElementLibraryInfoList == null)
                ElementLibraryInfoList = new List<ElementLibraryInfo>();
            ElementLibraryInfoList.Clear();

            for (int i = 0; i < libs.Length; i++)
            {
                ElementLibraryInfo info = new ElementLibraryInfo();
                info.lib_name = libs[i].name;
                info.lib_description = libs[i].LibraryDescription;
                info.lib_count = libs[i].ElementsLibrary_GetElementsCount();
                info.lib_used_count = libs[i].ElementsLibrary_GetElementsUsedCount();
                info.lib_initiated_count = libs[i].ElementsLibrary_GetElementsInitiatedCount();
                info.lib_recycled_count = libs[i].ElementsLibrary_GetElementsRecycledCount();
                ElementLibraryInfoList.Add(info);
            }
            #endregion

            sp_ElementLibraryInfoList = BaseObject.FindProperty("ElementLibraryInfoList");

            #region ReorderableList
            List_ElementLibraryInfo = new ReorderableList(BaseObject, sp_ElementLibraryInfoList, false, false, false, false);
            List_ElementLibraryInfo.drawElementCallback = List_ElementLibraryInfo_DrawElementCallback;
            #endregion
        }

        #region ElementLibraryInfoList

        Rect index_rect;
        Rect indicator_rect;
        Rect scrollview_rect;
        Rect item_rect;
        Rect itemmark_rect;
        Rect itemmarkBg_rect;

        /// <summary>
        /// 绘制元素
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="index"></param>
        /// <param name="isActive"></param>
        /// <param name="isFocused"></param>
        private void List_ElementLibraryInfo_DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            index_rect = new Rect(rect.x + 15, rect.y + 5, 30, 20);
            indicator_rect = new Rect(rect.x + 39, rect.y + 5, 180, 20);

            SerializedProperty prop = sp_ElementLibraryInfoList.GetArrayElementAtIndex(index);
            SerializedProperty sp_lib_name = prop.FindPropertyRelative("lib_name");

            Editor_XHud_GUI.Gui_Labelfield(index_rect, index.ToString("D2"), HudFilled.无, HudColor.无, Color.gray, TextAnchor.MiddleLeft, Vector2.zero, 11);

            #region 标识名称
            Editor_XHud_GUI.Gui_Labelfield(indicator_rect, sp_lib_name.stringValue, HudFilled.无, HudColor.无, Editor_XHud_GUI.GetColor(HudColor.亮白), TextAnchor.MiddleLeft, Vector2.zero, 12, TextClipping.Ellipsis);
            #endregion
        }

        /// <summary>
        /// 绘制列表
        /// </summary>
        private void List_ElementLibraryInfo_Drawer()
        {
            // 绘制滚动视图
            scrollview_rect = GUILayoutUtility.GetRect(0, visibleItemCount * itemHeight);
            scrollview_rect.x = 18;
            scrollview_rect.width = 300;
            scrollview_rect.height = 290;
            DataList_Scroller = GUI.BeginScrollView(scrollview_rect, DataList_Scroller, new Rect(0, 0, scrollview_rect.width - 50, ElementLibraryInfoList.Count * itemHeight), false, true);

            // 计算可视区域的起始和结束索引
            int startIndex = Mathf.FloorToInt(DataList_Scroller.y / itemHeight);
            int endIndex = Mathf.CeilToInt((DataList_Scroller.y + scrollview_rect.height) / itemHeight);

            // 只绘制可视区域内的元素
            for (int i = startIndex; i < endIndex && i < List_ElementLibraryInfo.count; i++)
            {
                SerializedProperty prop = sp_ElementLibraryInfoList.GetArrayElementAtIndex(i);

                item_rect = new Rect(0, i * itemHeight, scrollview_rect.width, itemHeight);

                // 如果当前元素被选中，绘制选中效果
                if (SelectedLibraryIndex == i)
                {
                    itemmark_rect = new Rect(item_rect.x + 1, item_rect.y + 11, 5, 5);
                    // 高亮标记表示选中
                    EditorGUI.DrawRect(itemmark_rect, XHud_Dashboard.Theme_Primary);
                    itemmarkBg_rect = new Rect(item_rect.x, item_rect.y, item_rect.width + 20, item_rect.height);
                    // 高亮背景表示选中
                    EditorGUI.DrawRect(itemmarkBg_rect, new Color(0, 0, 0, 0.2f));
                }

                List_ElementLibraryInfo.drawElementCallback.Invoke(item_rect, i, i == List_ElementLibraryInfo.index, true);

                // 检测鼠标是否在当前元素区域内
                if (item_rect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.MouseDown)
                    {
                        // 更新选中项
                        SelectedLibraryIndex = i;

                        // 标记界面需要更新
                        GUI.changed = true;
                    }
                }

                // 绘制元素
                //EditorGUI.PropertyField(item_rect, prop, GUIContent.none);
            }

            GUI.EndScrollView();
        }
        #endregion

        private void OnGUI()
        {
            BaseObject.Update();

            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

            Rect rect = new Rect(0, 0, position.width, position.height);

            Icon_rect = new Rect(26, 15, 48, 48);

            Editor_XHud_GUI.Gui_Icon(Icon_rect, icon_logo);

            Title_rect = new Rect(rect.x + 100, rect.y + 15, rect.width - 80, 30);
            Editor_XHud_GUI.Gui_Labelfield(Title_rect, "元素库选择器", HudFilled.无, HudColor.无, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 20, Font_Bold);

            Sepline_rect = new Rect(rect.x + 102, rect.y + 60, 200, 1);
            Editor_XHud_GUI.Gui_Box(Sepline_rect, SepLineColor);

            Editor_XHud_GUI.Gui_Labelfield_Thin_WrapClip(new Rect(rect.x + 26, rect.y + 80, rect.width - 45, rect.height), "以下列表中的项为当前在管理器中部署的元素库，您可以按照需要打开选择的目标元素库，点击每一项可查看此项元素库的信息", HudFilled.无, HudColor.无, MessageColor, TextAnchor.UpperLeft, new Vector2(0, 0), 12, true, Font_Light);

            DateTimes = DateTime.Now.ToString("yyyy-MM-dd  HH:mm:ss:ff");
            Date_rect = new Rect(rect.x + 150, rect.y + 15, rect.width - 180, rect.height);
            Editor_XHud_GUI.Gui_Labelfield_Thin_WrapClip(Date_rect, DateTimes, HudFilled.无, HudColor.无, DateTimeColor, TextAnchor.UpperRight, new Vector2(0, 0), 13, true, Font_Light);

            TotalCount_rect = new Rect(rect.width - 300, rect.y + 15, 80, 30);
            Editor_XHud_GUI.Gui_Labelfield(TotalCount_rect, ElementLibraryInfoList.Count.ToString(), HudFilled.无, HudColor.无, XHud_Dashboard.Theme_Primary, TextAnchor.MiddleRight, Vector2.zero, 17, Font_Bold);

            Editor_XHud_GUI.Gui_Labelfield(new Rect(TotalCount_rect.x + TotalCount_rect.width + 7, TotalCount_rect.y + 1, 20, TotalCount_rect.height), " 项", HudFilled.无, HudColor.无, Color.gray, TextAnchor.MiddleLeft, Vector2.zero, 14, Font_Bold);

            #region 选中的元素信息
            string colorhex_value = XHud_Utilitys.Color_To_HexColor(XHud_Dashboard.Theme_Primary, true);
            string colorhex_title = XHud_Utilitys.Color_To_HexColor(Color.white * 0.65f, true);

            Editor_XHud_GUI.Gui_Labelfield(new Rect(rect.width - 270, rect.y + 140, 200, 15), "元素库信息：", HudFilled.无, HudColor.无, Color.white, false, Color.blue, TextAnchor.UpperLeft, Vector2.zero, 16, Font_Bold);

            if (ElementLibraryInfoList != null && ElementLibraryInfoList.Count > 0)
            {
                SerializedProperty prop = sp_ElementLibraryInfoList.GetArrayElementAtIndex(SelectedLibraryIndex);
                SerializedProperty sp_lib_name = prop.FindPropertyRelative("lib_name");
                SerializedProperty sp_lib_description = prop.FindPropertyRelative("lib_description");
                SerializedProperty sp_lib_count = prop.FindPropertyRelative("lib_count");
                SerializedProperty sp_lib_used_count = prop.FindPropertyRelative("lib_used_count");
                SerializedProperty sp_lib_recycled_count = prop.FindPropertyRelative("lib_recycled_count");
                SerializedProperty sp_lib_initiated_count = prop.FindPropertyRelative("lib_initiated_count");

                Editor_XHud_GUI.Gui_Labelfield(new Rect(rect.width - 270, rect.y + 170, 200, 15), $"<color={colorhex_title}>名称：</color><color={colorhex_value}>{sp_lib_name.stringValue}</color>", HudFilled.无, HudColor.无, Color.white, TextAnchor.UpperLeft, Vector2.zero, 12, true, TextClipping.Ellipsis, true, Font_Light);

                Editor_XHud_GUI.Gui_Labelfield(new Rect(rect.width - 270, rect.y + 200, 230, 15), $"<color={colorhex_title}>说明：</color><color={colorhex_value}>{sp_lib_description.stringValue}</color>", HudFilled.无, HudColor.无, Color.white, TextAnchor.UpperLeft, Vector2.zero, 12, true, TextClipping.Ellipsis, true, Font_Light);

                Editor_XHud_GUI.Gui_Labelfield(new Rect(rect.width - 270, rect.y + 230, 200, 15), $"<color={colorhex_title}>总元素：</color><color={colorhex_value}>{sp_lib_count.intValue}</color>  个", HudFilled.无, HudColor.无, Color.white, TextAnchor.UpperLeft, Vector2.zero, 12, true, TextClipping.Ellipsis, true, Font_Light);

                Editor_XHud_GUI.Gui_Labelfield(new Rect(rect.width - 270, rect.y + 260, 200, 15), $"<color={colorhex_title}>正在使用元素：</color><color={colorhex_value}>{sp_lib_used_count.intValue}</color>  个", HudFilled.无, HudColor.无, Color.white, TextAnchor.UpperLeft, Vector2.zero, 12, true, TextClipping.Ellipsis, true, Font_Light);

                Editor_XHud_GUI.Gui_Labelfield(new Rect(rect.width - 270, rect.y + 290, 200, 15), $"<color={colorhex_title}>已回收元素：</color><color={colorhex_value}>{sp_lib_recycled_count.intValue}</color>  个", HudFilled.无, HudColor.无, Color.white, TextAnchor.UpperLeft, Vector2.zero, 12, true, TextClipping.Ellipsis, true, Font_Light);

                Editor_XHud_GUI.Gui_Labelfield(new Rect(rect.width - 270, rect.y + 320, 200, 15), $"<color={colorhex_title}>预初始化元素：</color><color={colorhex_value}>{sp_lib_initiated_count.intValue}</color>  个", HudFilled.无, HudColor.无, Color.white, TextAnchor.UpperLeft, Vector2.zero, 12, true, TextClipping.Ellipsis, true, Font_Light);
            }
            else
            {
                Editor_XHud_GUI.Gui_Labelfield(new Rect(rect.width - 195, rect.y + 250, 200, 15), "未部署元素库！", HudFilled.无, HudColor.无, Color.gray, false, Color.blue, TextAnchor.UpperLeft, Vector2.zero, 12, Font_Light);
            }
            #endregion

            #region 列表

            Editor_XHud_GUI.Gui_Layout_Space(140);

            List_ElementLibraryInfo_Drawer();

            #endregion

            Repaint();

            Editor_XHud_GUI.Gui_Layout_Space(65);
            DialogType_Buttons();

            if (BaseObject.targetObject != null)
                BaseObject.ApplyModifiedProperties();

            Event e = Event.current;
            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
            {
                e.Use();
                Close();
            }
        }

        /// <summary>
        /// 控件按钮
        /// </summary>
        private void DialogType_Buttons()
        {
            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_XHud_GUI.Gui_Layout_FlexSpace();

            if (Editor_XHud_GUI.Gui_Layout_Button("关闭", "", HudFilled.实体, HudColor.亮白, Color.black, 12, ButtonWidth, ButtonHeight, Font_Light, "关闭"))
            {
                Close();
            }
            GUI.backgroundColor = Color.white;
            Editor_XHud_GUI.Gui_Layout_Space(ButtonDistance);
            GUI.backgroundColor = XHud_Dashboard.Theme_Primary;
            if (Editor_XHud_GUI.Gui_Layout_Button("选择", "", HudFilled.实体, HudColor.亮白, XHud_Utilitys.GetBrightnessLimite(XHud_Dashboard.Theme_Primary) ? Color.black : Color.white, 12, ButtonWidth, ButtonHeight, Font_Light, "选择"))
            {
                XHud_Manager mgr = XHud_Dashboard.HudManagerGet();
                EditorUtility.OpenPropertyEditor(mgr.hm_ElementLibrary_GetArray()[SelectedLibraryIndex]);
                Close();
                return;
            }
            GUI.backgroundColor = Color.white;

            Editor_XHud_GUI.Gui_Layout_Space(25);
            Editor_XHud_GUI.Gui_Layout_Horizontal_End();
        }
    }
}
