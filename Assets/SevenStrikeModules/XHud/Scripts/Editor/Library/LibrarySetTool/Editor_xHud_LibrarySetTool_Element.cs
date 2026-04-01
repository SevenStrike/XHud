namespace SevenStrikeModules.XHud.Editor
{
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.Utilitys;
    using System;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    public class Editor_XHud_LibrarySetTool_Element : EditorWindow
    {
        public XHud_LibraryArg_Element_Item Library_Item;
        public XHud_LibraryArg_Element_Item Original_Library_Item;
        public int ModifiedIndex;
        private SerializedObject BaseObject;
        private SerializedProperty sp_PreloadElements, sp_Target, sp_Root, sp_NextIndex, sp_RecycledCount, sp_UsedCount, sp_InitializeCount, sp_Name;

        #region 元素库列表
        /// <summary>
        /// 数据列表
        /// </summary>
        public ReorderableList List_PreloadElements;
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

        #region Dialog按钮参数
        /// <summary>
        /// 按钮宽度
        /// </summary>
        private float ButtonWidth = 90;
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

        public string TargetLibName;
        public XHud_Library_Element TargetLib;

        [SerializeField]
        public string DateTimes;

        private Texture2D icon_logo;

        private void OnDisable()
        {

        }

        private void OnEnable()
        {
            BaseObject = new SerializedObject(this);

            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

            SerializedProperty prop = BaseObject.FindProperty("Library_Item");
            sp_PreloadElements = prop.FindPropertyRelative("PreloadElements");
            sp_Target = prop.FindPropertyRelative("Target");
            sp_Root = prop.FindPropertyRelative("Root");
            sp_NextIndex = prop.FindPropertyRelative("NextIndex");
            sp_RecycledCount = prop.FindPropertyRelative("RecycledCount");
            sp_UsedCount = prop.FindPropertyRelative("UsedCount");
            sp_InitializeCount = prop.FindPropertyRelative("InitializeCount");
            sp_Name = prop.FindPropertyRelative("Name");

            icon_logo = Editor_XHud_GUI.GetIcon("Icons_XHud_Library_Element_Setter/icon");

            #region  获取字体
            Font_Bold = Editor_XHud_GUI.GetFont("SS_Editor_Bold");
            Font_Light = Editor_XHud_GUI.GetFont("SS_Editor_Dialog");
            #endregion

            #region ReorderableList
            List_PreloadElements = new ReorderableList(BaseObject, sp_PreloadElements, false, false, false, false);
            List_PreloadElements.drawElementCallback = List_PreloadElementList_DrawElementCallback;
            #endregion
        }

        #region ElementLibraryInfoList

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
        private void List_PreloadElementList_DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty prop = sp_PreloadElements.GetArrayElementAtIndex(index);
            SerializedProperty sp_ele = prop.FindPropertyRelative("HudElement");

            Editor_XHud_GUI.Gui_Labelfield(new Rect(rect.x + 15, rect.y + 3, 30, 20), index.ToString("D2"), HudFilled.无, HudColor.无, Color.gray, TextAnchor.MiddleLeft, Vector2.zero, 11);

            #region 预生成元素名称
            XHud_Module_Element ele = (XHud_Module_Element)sp_ele.objectReferenceValue as XHud_Module_Element;
            Editor_XHud_GUI.Gui_Labelfield(new Rect(rect.x + 40, rect.y + 3, 150, 20), ele.name, HudFilled.无, HudColor.无, Editor_XHud_GUI.GetColor(HudColor.亮白), TextAnchor.MiddleLeft, Vector2.zero, 12, TextClipping.Ellipsis);
            #endregion
        }

        /// <summary>
        /// 绘制列表
        /// </summary>
        private void List_PreloadElementList_Drawer()
        {
            // 绘制滚动视图
            scrollview_rect = GUILayoutUtility.GetRect(0, visibleItemCount * itemHeight);
            scrollview_rect.x = 26;
            scrollview_rect.width = 220;
            scrollview_rect.height = 310;
            DataList_Scroller = GUI.BeginScrollView(scrollview_rect, DataList_Scroller, new Rect(0, 0, scrollview_rect.width - 50, Library_Item.PreloadElements.Count * itemHeight), false, true);

            // 计算可视区域的起始和结束索引
            int startIndex = Mathf.FloorToInt(DataList_Scroller.y / itemHeight);
            int endIndex = Mathf.CeilToInt((DataList_Scroller.y + scrollview_rect.height) / itemHeight);

            // 只绘制可视区域内的元素
            for (int i = startIndex; i < endIndex && i < List_PreloadElements.count; i++)
            {
                SerializedProperty prop = sp_PreloadElements.GetArrayElementAtIndex(i);

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

                List_PreloadElements.drawElementCallback.Invoke(item_rect, i, i == List_PreloadElements.index, true);

                // 检测鼠标是否在当前元素区域内
                if (item_rect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.MouseDown)
                    {
                        // 更新选中项
                        SelectedLibraryIndex = i;

                        XHud_LibraryArg_Element_Info preload = Library_Item.PreloadElements[i];
                        EditorGUIUtility.PingObject(preload.HudElement);

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
            Editor_XHud_GUI.Gui_Labelfield(Title_rect, "元素库修改器", HudFilled.无, HudColor.无, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 20, Font_Bold);

            Sepline_rect = new Rect(rect.x + 102, rect.y + 60, 200, 1);
            Editor_XHud_GUI.Gui_Box(Sepline_rect, SepLineColor);

            Editor_XHud_GUI.Gui_Labelfield_Thin_WrapClip(new Rect(rect.x + 26, rect.y + 80, rect.width - 45, rect.height), "您可以按照需要查看已生成的预制元素列表，或者修改目标元素的库参数特性！", HudFilled.无, HudColor.无, MessageColor, TextAnchor.UpperLeft, new Vector2(0, 0), 12, true, Font_Light);

            DateTimes = DateTime.Now.ToString("yyyy-MM-dd  HH:mm:ss:ff");
            Date_rect = new Rect(rect.x + 150, rect.y + 15, rect.width - 180, rect.height);
            Editor_XHud_GUI.Gui_Labelfield_Thin_WrapClip(Date_rect, DateTimes, HudFilled.无, HudColor.无, DateTimeColor, TextAnchor.UpperRight, new Vector2(0, 0), 13, true, Font_Light);

            #region 列表
            if (sp_PreloadElements.arraySize > 0)
            {
                Editor_XHud_GUI.Gui_Layout_Space(120);
                List_PreloadElementList_Drawer();
                Editor_XHud_GUI.Gui_Layout_Space(80);
            }
            else
            {
                Editor_XHud_GUI.Gui_Layout_Space(400);

                Editor_XHud_GUI.Gui_Box(new Rect(rect.x + 26, rect.y + 120, 220, 310), Color.black * 0.15f);
                Editor_XHud_GUI.Gui_Labelfield(new Rect(rect.x + 85, rect.y + 260, 100, 20), "暂无预生成元素", HudFilled.无, HudColor.无, Color.gray, TextAnchor.MiddleCenter, Vector2.zero, 12, Font_Light);
            }
            #endregion

            #region 参数

            string colorhex_value = XHud_Utilitys.Color_To_HexColor(XHud_Dashboard.Theme_Primary, true);
            string colorhex_title = XHud_Utilitys.Color_To_HexColor(Color.white * 0.65f, true);

            Editor_XHud_GUI.Gui_Labelfield(new Rect(rect.width - 240, rect.y + 120, 215, 20), $"<color={colorhex_value}>{sp_Name.stringValue}</color>", HudFilled.无, HudColor.无, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 16, Font_Bold);

            Editor_XHud_GUI.Gui_Box(new Rect(rect.width - 240, rect.y + 153, 215, 1), Color.white * 0.65f);

            float AddedHeight = 50;

            if (Application.isPlaying)
                Editor_XHud_GUI.SetEnabled(false);
            else
                Editor_XHud_GUI.SetEnabled(true);
            EditorGUI.BeginChangeCheck();
            Editor_XHud_GUI.Gui_Property_Field(new Rect(rect.width - 240, rect.y + 120 + AddedHeight, 215, 20), "元素源", sp_Target, 0, 60);
            if (EditorGUI.EndChangeCheck())
            {
                if (sp_Target.objectReferenceValue != null)
                {
                    XHud_Module_Element element = (XHud_Module_Element)sp_Target.objectReferenceValue as XHud_Module_Element;
                    sp_Name.stringValue = element.name;
                }
                else
                {
                    sp_Name.stringValue = null;
                }
            }
            Editor_XHud_GUI.SetEnabled(false);
            Editor_XHud_GUI.Gui_Property_Field(new Rect(rect.width - 240, rect.y + 155 + AddedHeight, 215, 20), "根节点", sp_Root, 0, 60);

            if (Application.isPlaying)
                Editor_XHud_GUI.SetEnabled(false);
            else
                Editor_XHud_GUI.SetEnabled(true);
            Editor_XHud_GUI.Gui_Property_Field(new Rect(rect.width - 240, rect.y + 190 + AddedHeight, 215, 20), "预加载数量", sp_InitializeCount, 0, 85);
            Editor_XHud_GUI.SetEnabled(true);

            Editor_XHud_GUI.Gui_Box(new Rect(rect.width - 240, rect.y + 220 + AddedHeight, 215, 1), Color.gray * 0.7f);

            Editor_XHud_GUI.Gui_Labelfield(new Rect(rect.width - 240, rect.y + 235 + AddedHeight, 215, 20), $"<color={colorhex_title}>已被使用  (UsedCount) ： </color> <color={colorhex_value}>{sp_UsedCount.intValue}</color>  个", HudFilled.无, HudColor.无, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 12, Font_Light);

            Editor_XHud_GUI.Gui_Labelfield(new Rect(rect.width - 240, rect.y + 270 + AddedHeight, 215, 20), $"<color={colorhex_title}>已被回收  (RecycledCount) ： </color> <color={colorhex_value}>{sp_RecycledCount.intValue}</color>  个", HudFilled.无, HudColor.无, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 12, Font_Light);

            Editor_XHud_GUI.Gui_Labelfield(new Rect(rect.width - 240, rect.y + 305 + AddedHeight, 215, 20), $"<color={colorhex_title}>下一个索引  (NextIndex) ： </color> <color={colorhex_value}>{sp_NextIndex.intValue}</color>", HudFilled.无, HudColor.无, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 12, Font_Light);
            #endregion

            Repaint();

            DialogType_Buttons();

            Event e = Event.current;
            // 检测点击事件
            if (e.type == EventType.MouseDown)
            {
                // 检查点击位置是否在窗口内
                if (!new Rect(rect.width - 240, rect.y + 190, 215, 20).Contains(e.mousePosition))
                {
                    GUI.FocusControl(null); // 取消所有控件的焦点
                }
            }

            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
            {
                e.Use();
                Close();
            }

            if (BaseObject.targetObject != null)
                BaseObject.ApplyModifiedProperties();
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

            if (Application.isPlaying)
                Editor_XHud_GUI.SetEnabled(false);
            else
                Editor_XHud_GUI.SetEnabled(true);
            GUI.backgroundColor = XHud_Dashboard.Theme_Primary;
            if (Editor_XHud_GUI.Gui_Layout_Button("更新", "", HudFilled.实体, HudColor.亮白, XHud_Utilitys.GetBrightnessLimite(XHud_Dashboard.Theme_Primary) ? Color.black : Color.white, 12, ButtonWidth, ButtonHeight, Font_Light, "更新"))
            {
                Save();
            }
            GUI.backgroundColor = Color.white;
            Editor_XHud_GUI.SetEnabled(true);

            Editor_XHud_GUI.Gui_Layout_Space(25);
            Editor_XHud_GUI.Gui_Layout_Horizontal_End();
        }

        /// <summary>
        /// 设置元素信息
        /// </summary>
        /// <param name="item"></param>
        public void SetLibraryItem(XHud_LibraryArg_Element_Item item)
        {
            Library_Item.Copy(item);
            Original_Library_Item.Copy(item);
        }

        /// <summary>
        /// 设置目标库名称
        /// </summary>
        /// <param name="name"></param>
        internal void SetTargetLibName(string name)
        {
            TargetLibName = name;
        }

        /// <summary>
        /// 设置目标元素库
        /// </summary>
        /// <param name="lib"></param>
        internal void SetTargetLib(XHud_Library_Element lib)
        {
            TargetLib = lib;
        }

        private void Save()
        {
            #region 更新元素库目标元素参数

            TargetLib.ElementsLibrary_ReplaceElement(ModifiedIndex, Library_Item);

            #endregion

            Close();
            return;
        }

    }
}
