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
    using SevenStrikeModules.XGUI.Editor;
    using SevenStrikeModules.XGUI.Runtime;
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

        [SerializeField]
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

            SerializedProperty prop = BaseObject.FindProperty("Library_Item");
            sp_PreloadElements = prop.FindPropertyRelative("PreloadElements");
            sp_Target = prop.FindPropertyRelative("Target");
            sp_Root = prop.FindPropertyRelative("Root");
            sp_NextIndex = prop.FindPropertyRelative("NextIndex");
            sp_RecycledCount = prop.FindPropertyRelative("RecycledCount");
            sp_UsedCount = prop.FindPropertyRelative("UsedCount");
            sp_InitializeCount = prop.FindPropertyRelative("InitializeCount");
            sp_Name = prop.FindPropertyRelative("Name");

            icon_logo = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_element_setter/icon");

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

            #region 预生成元素索引号
            XGUI.gui_label(
                rect: new Rect(rect.x + 15, rect.y + 3, 30, 20),
                text: new GUIContent(index.ToString("D2")),
                text_color: Color.white * 0.75f,
                size: XGUIFontSize.M,
                clipping: TextClipping.Clip,
                anchor: TextAnchor.MiddleLeft,
                offset: new Vector2(0, 0),
                wrap: true,
                font_style: FontStyle.Normal);
            #endregion

            #region 预生成元素名称
            XHud_Module_Element ele = (XHud_Module_Element)sp_ele.objectReferenceValue as XHud_Module_Element;
            XGUI.gui_label(
                rect: new Rect(rect.x + 40, rect.y + 3, 150, 20),
                text: new GUIContent(ele.name),
                text_color: Color.white,
                size: XGUIFontSize.M,
                clipping: TextClipping.Clip,
                anchor: TextAnchor.MiddleLeft,
                offset: new Vector2(0, 0),
                wrap: true,
                font_style: FontStyle.Bold);
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

            #region 抬头
            Rect rect = new Rect(0, 0, position.width, position.height);

            // 图标
            Rect rect_icon = new Rect(15, 15, icon_logo.width, icon_logo.height);
            XGUI.gui_icon(
                rect: rect_icon,
                icon: icon_logo,
                padding: new RectOffset(0, 0, 0, 0),
                border: new RectOffset(0, 0, 0, 0),
                color: Color.white);

#if UNITY_6000_0_OR_NEWER
            TextClipping clipping = TextClipping.Ellipsis;
#else
    TextClipping clipping = TextClipping.Clip;
#endif

            // 大标题
            Rect rect_title = new Rect(rect.x + 70, rect.y + 10, rect.width - 80, 30);
            XGUI.gui_label(
                rect: rect_title,
                text: new GUIContent("元素项修改器"),
                text_color: Color.white,
                size: XGUIFontSize.L,
                clipping: clipping,
                font: XGUI.GetFont("xg-heavy"));

            // 分割线
            Rect rect_seperate = new Rect(rect.x + 68, rect.y + 43, 200, 1);
            XGUI.gui_seperator(
                rect: rect_seperate,
                thickness: 1,
                color: XHud_Dashboard.Theme_SeperateLine,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0));

            // 小标题
            Rect rect_subtitle = new Rect(rect.x + 18, rect.y + 53, rect.width, 30);
            XGUI.gui_label(
                rect: rect_subtitle,
                text: new GUIContent("您可以按照需要查看已生成的预制元素列表，或者修改目标元素的库参数特性！"),
                text_color: Color.white * 0.7f,
                size: XGUIFontSize.M,
                anchor: TextAnchor.UpperLeft,
                clipping: clipping);
            #endregion           

            #region 预生成元素列表
            if (sp_PreloadElements.arraySize > 0)
            {
                XGUI.layout_space(100);
                List_PreloadElementList_Drawer();
                XGUI.layout_space(60);
            }
            else
            {
                XGUI.layout_space(380);

                XGUI.gui_box(
                    rect: new Rect(rect.x + 18, rect.y + 100, 220, 310),
                    bg_color: Color.black * 0.15f);

                XGUI.gui_label(
                    rect: new Rect(rect.x + 80, rect.y + 240, 100, 20),
                    text: new GUIContent(" 暂无预生成元素"),
                    text_color: Color.white * 0.7f,
                    size: XGUIFontSize.M,
                    clipping: clipping,
                    anchor: TextAnchor.MiddleCenter,
                    offset: new Vector2(0, 0),
                    font_style: FontStyle.Normal);

            }
            #endregion

            #region 参数

            string colorhex_value = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary, true);
            string colorhex_title = XGUI_Utilitys.Color_To_HexString(Color.white * 0.65f, true);

            Rect rect_xs = rect;

            #region 目标元素名称
            rect_xs.Set(rect.width - 240, rect.y + 100, 215, 20);
            XGUI.gui_label(
                rect: rect_xs,
                text: new GUIContent(sp_Name.stringValue),
                text_color: Color.white,
                size: XGUIFontSize.L,
                clipping: clipping,
                anchor: TextAnchor.MiddleLeft,
                offset: new Vector2(0, 0),
                wrap: true,
                font_style: FontStyle.Bold);
            #endregion

            #region 分割线
            rect_xs.Set(rect.width - 240, rect.y + 135, 215, 1);
            XGUI.gui_seperator(
                rect: rect_xs,
                thickness: 1,
                color: XHud_Dashboard.Theme_SeperateLine,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0));
            #endregion

            float AddedHeight = 40;

            if (Application.isPlaying)
                XGUI.SetEnabled(false);
            else
                XGUI.SetEnabled(true);

            #region 元素源
            rect_xs.Set(rect.width - 240, rect.y + 110 + AddedHeight, 215, 20);
            XGUI.ChangedCheck_Start();
            XGUI.gui_property_field(
                rect: rect_xs,
                title: new GUIContent("元素源"),
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.Theme_Primary,
                title_width: 80,
                prop: sp_Target);
            if (XGUI.ChangedCheck_End())
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
            #endregion

            XGUI.SetEnabled(false);

            #region 根节点
            rect_xs.Set(rect.width - 240, rect.y + 135 + AddedHeight, 215, 20);
            XGUI.gui_property_field(
                rect: rect_xs,
                title: new GUIContent("根节点"),
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.Theme_Primary,
                title_width: 80,
                prop: sp_Root);
            #endregion

            if (Application.isPlaying)
                XGUI.SetEnabled(false);
            else
                XGUI.SetEnabled(true);

            #region 预加载数量
            rect_xs.Set(rect.width - 240, rect.y + 160 + AddedHeight, 215, 20);
            XGUI.gui_property_field(
                rect: rect_xs,
                title: new GUIContent("预加载数量"),
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.Theme_Primary,
                title_width: 80,
                prop: sp_InitializeCount);
            #endregion

            XGUI.SetEnabled(true);

            #region 分割线
            rect_xs.Set(rect.width - 240, rect.y + 195 + AddedHeight, 215, 1);
            XGUI.gui_seperator(
                rect: rect_xs,
                thickness: 1,
                color: XHud_Dashboard.Theme_SeperateLine,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0));
            #endregion

            #region 已被使用
            rect_xs.Set(rect.width - 240, rect.y + 210 + AddedHeight, 215, 20);
            XGUI.gui_label(
                rect: rect_xs,
                text: new GUIContent($"<color={colorhex_title}>已被使用  (UsedCount) ： </color> <color={colorhex_value}>{sp_UsedCount.intValue}</color>  个"),
                text_color: Color.white,
                size: XGUIFontSize.M,
                clipping: clipping,
                anchor: TextAnchor.MiddleLeft,
                offset: new Vector2(0, 0),
                wrap: true,
                font_style: FontStyle.Normal);
            #endregion

            #region 已被回收
            rect_xs.Set(rect.width - 240, rect.y + 240 + AddedHeight, 215, 20);
            XGUI.gui_label(
                rect: rect_xs,
                text: new GUIContent($"<color={colorhex_title}>已被回收  (RecycledCount) ： </color> <color={colorhex_value}>{sp_RecycledCount.intValue}</color>  个"),
                text_color: Color.white,
                size: XGUIFontSize.M,
                clipping: clipping,
                anchor: TextAnchor.MiddleLeft,
                offset: new Vector2(0, 0),
                wrap: true,
                font_style: FontStyle.Normal);
            #endregion

            #region 下一个索引
            rect_xs.Set(rect.width - 240, rect.y + 270 + AddedHeight, 215, 20);
            XGUI.gui_label(
                rect: rect_xs,
                text: new GUIContent($"<color={colorhex_title}>下一个索引  (NextIndex) ： </color> <color={colorhex_value}>{sp_NextIndex.intValue}</color>"),
                text_color: Color.white,
                size: XGUIFontSize.M,
                clipping: clipping,
                anchor: TextAnchor.MiddleLeft,
                offset: new Vector2(0, 0),
                wrap: true,
                font_style: FontStyle.Normal);
            #endregion
            #endregion

            Repaint();

            Buttons();

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
        private void Buttons()
        {
            XGUI.layout_group_start(
            type: XGUIContainerType.Horizontal,
            margin: new RectOffset(0, 0, 0, 0),
            padding: new RectOffset(250, 15, 2, 15));

            if (XGUI.layout_button(
                text: "关闭",
                tooltip: "",
                bg_fill: XGUIFilled.实体,
                bg_color: XGUIColor.亮白,
                bg_color_gui: Color.white,
                button_text_color: Color.black,
                press_fill: XGUIFilled.实体,
                press_color: XGUIColor.深空灰,
                press_text_color: Color.white,
                font_size: XGUIFontSize.B,
                anchor: TextAnchor.MiddleCenter,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                height: XGUI.GetSingleLineHeight() + 10,
                button_text_font: XGUI.GetFont("xg-medium")))
            {
                Close();
            }

            XGUI.layout_space(10);

            if (Application.isPlaying)
                XGUI.SetEnabled(false);
            else
                XGUI.SetEnabled(true);

            if (XGUI.layout_button(
                text: "更新",
                tooltip: "",
                bg_fill: XGUIFilled.实体,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.Theme_Primary,
                button_text_color: Color.black,
                press_fill: XGUIFilled.实体,
                press_color: XGUIColor.深空灰,
                press_text_color: Color.white,
                font_size: XGUIFontSize.B,
                anchor: TextAnchor.MiddleCenter,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                height: XGUI.GetSingleLineHeight() + 10,
                button_text_font: XGUI.GetFont("xg-medium")))
            {
                Save();
            }
            GUI.backgroundColor = Color.white;
            XGUI.SetEnabled(true);

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
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
