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
        private XHud_Manager HudManager;
        private SerializedObject BaseObject;
        private SerializedProperty sp_ElementLibrarys;

        #region 元素库列表
        public List<ElementLibraryInfo> ElementLibrarys = new List<ElementLibraryInfo>();
        /// <summary>
        /// 数据列表
        /// </summary>
        public ReorderableList List_ElementLibrarys;
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

        #region Dialog标题颜色
        Color SepLineColor = new Color(1, 1, 1, 0.15f);
        Color MessageColor = new Color(1, 1, 1, 0.75f);
        Color DateTimeColor = new Color(1, 1, 1, 0.42f);
        #endregion

        #region Dialog 控件坐标
        Rect Sepline_rect;
        Rect Title_rect;
        Rect Date_rect;
        Rect Icon_rect;
        Rect TotalCount_rect;
        #endregion

        Rect LibSelector_rect;

        private void OnDisable()
        {

        }

        private void OnEnable()
        {
            BaseObject = new SerializedObject(this);

            HudManager = XHud_Dashboard.HudManagerGet();

            icon_logo = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_elements_selector/icon");

            #region 获取所有已部署到HudManager的元素库
            XHud_Library_Element[] libs = HudManager.hm_ElementLibrary_GetArray();

            if (ElementLibrarys == null)
                ElementLibrarys = new List<ElementLibraryInfo>();
            ElementLibrarys.Clear();

            for (int i = 0; i < libs.Length; i++)
            {
                ElementLibraryInfo info = new ElementLibraryInfo();
                if (libs[i] == null)
                {
                    XGUI_Utilitys.Console("XHud - 元素库消息", "元素库列表中存在无效项！请前往XHud管理器中的元素库列表查看并解决！", XGUIMsgState.错误);
                    continue;
                }
                info.lib_name = libs[i].name;
                info.lib_description = libs[i].LibraryDescription;
                info.lib_count = libs[i].ElementsLibrary_GetElementsCount();
                info.lib_used_count = libs[i].ElementsLibrary_GetElementsUsedCount();
                info.lib_initiated_count = libs[i].ElementsLibrary_GetElementsInitiatedCount();
                info.lib_recycled_count = libs[i].ElementsLibrary_GetElementsRecycledCount();
                ElementLibrarys.Add(info);
            }
            #endregion

            sp_ElementLibrarys = BaseObject.FindProperty("ElementLibrarys");

            #region ReorderableList
            List_ElementLibrarys = new ReorderableList(BaseObject, sp_ElementLibrarys, false, false, false, false);
            List_ElementLibrarys.drawElementCallback = List_ElementLibraryInfo_DrawElementCallback;
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

            SerializedProperty prop = sp_ElementLibrarys.GetArrayElementAtIndex(index);
            SerializedProperty sp_lib_name = prop.FindPropertyRelative("lib_name");

            #region 元素库索引号
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

            #region 元素库名称
            XGUI.gui_label(
                rect: new Rect(rect.x + 40, rect.y + 3, 150, 20),
                text: new GUIContent(sp_lib_name.stringValue),
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
        private void List_ElementLibraryInfo_Drawer()
        {
            // 绘制滚动视图
            scrollview_rect = GUILayoutUtility.GetRect(0, visibleItemCount * itemHeight);
            scrollview_rect.x = 18;
            scrollview_rect.width = 220;
            scrollview_rect.height = 290;
            DataList_Scroller = GUI.BeginScrollView(scrollview_rect, DataList_Scroller, new Rect(0, 0, scrollview_rect.width - 50, ElementLibrarys.Count * itemHeight), false, true);

            // 计算可视区域的起始和结束索引
            int startIndex = Mathf.FloorToInt(DataList_Scroller.y / itemHeight);
            int endIndex = Mathf.CeilToInt((DataList_Scroller.y + scrollview_rect.height) / itemHeight);

            // 只绘制可视区域内的元素
            for (int i = startIndex; i < endIndex && i < List_ElementLibrarys.count; i++)
            {
                SerializedProperty prop = sp_ElementLibrarys.GetArrayElementAtIndex(i);

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

                List_ElementLibrarys.drawElementCallback.Invoke(item_rect, i, i == List_ElementLibrarys.index, true);

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

            string colorhex_value = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary, true);
            string colorhex_title = XGUI_Utilitys.Color_To_HexString(Color.white * 0.65f, true);

#if UNITY_6000_0_OR_NEWER
            TextClipping clipping = TextClipping.Ellipsis;
#else
    TextClipping clipping = TextClipping.Clip;
#endif

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

            // 大标题
            Rect rect_title = new Rect(rect.x + 70, rect.y + 10, rect.width - 80, 30);
            XGUI.gui_label(
                rect: rect_title,
                text: new GUIContent("元素库选择器"),
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
                text: new GUIContent("您可以按照需要打开选择的目标元素库，点击每一项可查看此项元素库的信息"),
                text_color: Color.white * 0.7f,
                size: XGUIFontSize.M,
                anchor: TextAnchor.UpperLeft,
                wrap: true,
                clipping: TextClipping.Overflow);
            #endregion

            #region 统计数量
            LibSelector_rect.Set(rect.x + rect.width - 120, rect.y + 15, 100, XGUI.GetSingleLineHeight());
            XGUI.gui_label(
                rect: LibSelector_rect,
                text: new GUIContent($"<color={colorhex_value}>{ElementLibrarys.Count.ToString()}</color> <color={colorhex_title}> 项</color>"),
                text_color: Color.white,
                size: XGUIFontSize.M,
                anchor: TextAnchor.MiddleRight,
                clipping: TextClipping.Overflow);
            #endregion

            #region 元素信息标题
            LibSelector_rect.Set(rect.x + (rect.width - 240), rect.y + 110, 100, 15);
            XGUI.gui_label(
                   rect: LibSelector_rect,
                   text: new GUIContent("元素库信息"),
                   text_color: Color.white,
                   size: XGUIFontSize.M,
                   clipping: clipping,
                   anchor: TextAnchor.MiddleLeft,
                   offset: new Vector2(0, 0),
                   wrap: true,
            font_style: FontStyle.Bold);

            if (ElementLibrarys != null && ElementLibrarys.Count > 0)
            {
                SerializedProperty prop = sp_ElementLibrarys.GetArrayElementAtIndex(SelectedLibraryIndex);
                SerializedProperty sp_lib_name = prop.FindPropertyRelative("lib_name");
                SerializedProperty sp_lib_description = prop.FindPropertyRelative("lib_description");
                SerializedProperty sp_lib_count = prop.FindPropertyRelative("lib_count");
                SerializedProperty sp_lib_used_count = prop.FindPropertyRelative("lib_used_count");
                SerializedProperty sp_lib_recycled_count = prop.FindPropertyRelative("lib_recycled_count");
                SerializedProperty sp_lib_initiated_count = prop.FindPropertyRelative("lib_initiated_count");

                #region 名称
                LibSelector_rect.Set(rect.x + (rect.width - 240), rect.y + 140, rect.width - 15, 20);
                XGUI.gui_label(
                    rect: LibSelector_rect,
                    text: new GUIContent($"名称： <color={colorhex_value}>{sp_lib_name.stringValue}</color>"),
                    text_color: Color.white,
                    size: XGUIFontSize.M,
                    clipping: clipping,
                    anchor: TextAnchor.MiddleLeft,
                    font_style: FontStyle.Normal);
                #endregion

                #region 说明
                LibSelector_rect.Set(rect.x + (rect.width - 240), rect.y + 165, rect.width - 15, 20);
                XGUI.gui_label(
                    rect: LibSelector_rect,
                    text: new GUIContent($"说明： <color={colorhex_value}>{sp_lib_description.stringValue} </color>"),
                    text_color: Color.white,
                    size: XGUIFontSize.M,
                    clipping: clipping,
                    anchor: TextAnchor.MiddleLeft,
                    font_style: FontStyle.Normal);
                #endregion

                #region 总元素
                LibSelector_rect.Set(rect.x + (rect.width - 240), rect.y + 190, rect.width - 15, 20);
                XGUI.gui_label(
                    rect: LibSelector_rect,
                    text: new GUIContent($"总元素： <color={colorhex_value}>{sp_lib_count.intValue} 个</color>"),
                    text_color: Color.white,
                    size: XGUIFontSize.M,
                    clipping: clipping,
                    anchor: TextAnchor.MiddleLeft,
                    font_style: FontStyle.Normal);
                #endregion

                #region 正在使用元素
                LibSelector_rect.Set(rect.x + (rect.width - 240), rect.y + 215, rect.width - 15, 20);
                XGUI.gui_label(
                    rect: LibSelector_rect,
                    text: new GUIContent($"正在使用元素： <color={colorhex_value}>{sp_lib_used_count.intValue} 个</color>"),
                    text_color: Color.white,
                    size: XGUIFontSize.M,
                    clipping: clipping,
                    anchor: TextAnchor.MiddleLeft,
                    font_style: FontStyle.Normal);
                #endregion

                #region 已回收元素
                LibSelector_rect.Set(rect.x + (rect.width - 240), rect.y + 240, rect.width - 15, 20);
                XGUI.gui_label(
                    rect: LibSelector_rect,
                    text: new GUIContent($"已回收元素： <color={colorhex_value}>{sp_lib_recycled_count.intValue} 个</color>"),
                    text_color: Color.white,
                    size: XGUIFontSize.M,
                    clipping: clipping,
                    anchor: TextAnchor.MiddleLeft,
                    font_style: FontStyle.Normal);
                #endregion

                #region 预初始化元素
                LibSelector_rect.Set(rect.x + (rect.width - 240), rect.y + 265, rect.width - 15, 20);
                XGUI.gui_label(
                    rect: LibSelector_rect,
                    text: new GUIContent($"预初始化元素： <color={colorhex_value}>{sp_lib_initiated_count.intValue} 个</color>"),
                    text_color: Color.white,
                    size: XGUIFontSize.M,
                    clipping: clipping,
                    anchor: TextAnchor.MiddleLeft,
                    font_style: FontStyle.Normal);
                #endregion
            }
            else
            {
                #region 未部署元素库
                LibSelector_rect.Set(rect.width - 195, rect.y + 250, 200, 15);
                XGUI.gui_label(
                    rect: LibSelector_rect,
                    text: new GUIContent($"未部署元素库"),
                    text_color: Color.white,
                    size: XGUIFontSize.M,
                    clipping: clipping,
                    anchor: TextAnchor.MiddleLeft,
                    font_style: FontStyle.Normal);
                #endregion
            }
            #endregion

            XGUI.layout_space(110);

            #region 列表
            List_ElementLibraryInfo_Drawer();
            #endregion

            if (BaseObject.targetObject != null)
                BaseObject.ApplyModifiedProperties();

            Repaint();

            XGUI.layout_space(50);

            Buttons();

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
                text: "选择",
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
                EditorUtility.OpenPropertyEditor(HudManager.hm_ElementLibrary_GetArray()[SelectedLibraryIndex]);
                Close();
                return;
            }
            GUI.backgroundColor = Color.white;
            XGUI.SetEnabled(true);

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
        }
    }
}
