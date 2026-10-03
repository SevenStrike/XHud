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

    [System.Serializable]
    public class ReadyElementInfo
    {
        public string Name;
        public string Indicator;
        public Vector2 Size;
        public Vector3 Pos;
        public Vector3 Rot;
    }

    [System.Serializable]
    public class ReadyLibElement
    {
        [SerializeField]
        public XHud_Module_Element Element;
        [SerializeField]
        public ReadyElementInfo Info;
    }

    public class Editor_XHud_LibrarySetTool_ElementImporter : EditorWindow
    {
        private XHud_Manager HudManager;
        private SerializedObject BaseObject;
        private SerializedProperty sp_library_name, sp_element_list;

        /// <summary>
        /// 数据列表
        /// </summary>
        public ReorderableList ReorderableList;
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
        public int SelectedIndex = 0;

        public List<ReadyLibElement> element_list = new List<ReadyLibElement>();

        private Texture2D icon_libsetter_element;

        string[] libnames;

        [SerializeField]
        public string library_name;
        [SerializeField]
        public string DateTimes;

        Rect LibSelector_rect;

        #region 将工程预制体添加到元素库
        [MenuItem("Assets/XHud/SendToElementLibrary (将元素预制体添加到元素库)", priority = 3000, validate = true)]
        private static bool Valid_OpenElementLibrarySetter()
        {
            // 获取当前选中的对象
            GameObject selectedObject = Selection.activeObject as GameObject;
            if (selectedObject != null)
            {
                XHud_Module_Element element = selectedObject.GetComponent<XHud_Module_Element>();
                if (element != null)
                {
                    // 检查选中的对象是否为 Hud_Element 类型
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        [MenuItem("Assets/XHud/SendToElementLibrary (将元素预制体添加到元素库)", priority = 3000)]
        private static void OpenElementLibrarySetter()
        {
            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

            if (mgr == null)
            {
                XGUI.dialog(
                    type: XGUIDialogType.通知,
                    windowtitle: "XHud - 元素库添加器消息",
                    title: "未发现XHud管理器",
                    msg: "场景中未找到XHudManager管理器！",
                    ok: "明白",
                    PrimaryIndex: 0,
                    themecolor: XHud_Dashboard.Theme_Primary);
                return;
            }
            Transform[] sel_ele_objs = Selection.GetTransforms(SelectionMode.Unfiltered);

            List<XHud_Module_Element> Hud_Elements = new List<XHud_Module_Element>();

            for (int i = 0; i < sel_ele_objs.Length; i++)
            {
                XHud_Module_Element sel_ele = sel_ele_objs[i].GetComponent<XHud_Module_Element>();

                if (mgr.hm_ElementLibrary_GetCount() <= 0)
                {
                    XGUI.dialog(
                        type: XGUIDialogType.通知,
                        windowtitle: "XHud - 元素库添加器消息",
                        title: "元素库未配置",
                        msg: "您未在HudManager中配置元素库！请先至少指定一个元素库后再执行此操作！ ",
                        ok: "明白",
                        PrimaryIndex: 0,
                        themecolor: XHud_Dashboard.Theme_Primary);
                    return;
                }

                Hud_Elements.Add(sel_ele);
            }

            Editor_XHud_LibrarySetTool_ElementImporter window = EditorWindow.GetWindow<Editor_XHud_LibrarySetTool_ElementImporter>(true);
            window.titleContent = new GUIContent("XHud 元素库添加器");
            XGUI.CenterEditorWindow(new Vector2Int(500, 440), window);

            // 将要存入元素库的物体信息发送至窗口
            window.SetElements(Hud_Elements);
            window.Show();
        }

        #endregion

        #region DataList
        Rect scrollview_rect;
        Rect item_rect;
        Rect itemmark_rect;
        Rect itemmarkBg_rect;

        private void Remove(ReorderableList list)
        {
            Debug.Log(1);
        }

        /// <summary>
        /// 绘制元素
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="index"></param>
        /// <param name="isActive"></param>
        /// <param name="isFocused"></param>
        private void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty prop = sp_element_list.GetArrayElementAtIndex(index);
            SerializedProperty prop_Info = prop.FindPropertyRelative("Info");
            SerializedProperty sp_name = prop_Info.FindPropertyRelative("Name");

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
            XGUI.gui_label(
                rect: new Rect(rect.x + 40, rect.y + 3, 150, 20),
                text: new GUIContent(sp_name.stringValue),
                text_color: Color.white,
                size: XGUIFontSize.M,
                clipping: TextClipping.Clip,
                anchor: TextAnchor.MiddleLeft,
                offset: new Vector2(0, 0),
                wrap: true,
                font_style: FontStyle.Bold);
            #endregion
        }

        private void DeleteElementAtIndex(int index)
        {
            if (index < 0 || index >= sp_element_list.arraySize)
                return;

            // 删除项
            sp_element_list.DeleteArrayElementAtIndex(index);
            sp_element_list.serializedObject.ApplyModifiedProperties();

            // 调整选中索引
            if (SelectedIndex >= sp_element_list.arraySize)
            {
                SelectedIndex = sp_element_list.arraySize - 1;
            }

            // 刷新 UI
            Repaint();
        }

        /// <summary>
        /// 绘制列表
        /// </summary>
        private void DrawDataList()
        {
            // 绘制滚动视图
            scrollview_rect = GUILayoutUtility.GetRect(0, visibleItemCount * itemHeight);
            scrollview_rect.x = 18;
            scrollview_rect.width = 220;
            scrollview_rect.height = 290;
            DataList_Scroller = GUI.BeginScrollView(scrollview_rect, DataList_Scroller, new Rect(0, 0, scrollview_rect.width - 50, element_list.Count * itemHeight), false, true);

            // 计算可视区域的起始和结束索引
            int startIndex = Mathf.FloorToInt(DataList_Scroller.y / itemHeight);
            int endIndex = Mathf.CeilToInt((DataList_Scroller.y + scrollview_rect.height) / itemHeight);

            // 只绘制可视区域内的元素
            for (int i = startIndex; i < endIndex && i < ReorderableList.count; i++)
            {
                SerializedProperty prop = sp_element_list.GetArrayElementAtIndex(i);

                item_rect = new Rect(0, i * itemHeight, scrollview_rect.width, itemHeight);

                // 如果当前元素被选中，绘制选中效果
                if (SelectedIndex == i)
                {
                    itemmark_rect = new Rect(item_rect.x + 1, item_rect.y + 11, 5, 5);
                    // 高亮标记表示选中
                    EditorGUI.DrawRect(itemmark_rect, XHud_Dashboard.Theme_Primary);
                    itemmarkBg_rect = new Rect(item_rect.x, item_rect.y, item_rect.width + 20, item_rect.height);
                    // 高亮背景表示选中
                    EditorGUI.DrawRect(itemmarkBg_rect, new Color(0, 0, 0, 0.2f));
                }

                ReorderableList.drawElementCallback.Invoke(item_rect, i, i == ReorderableList.index, true);

                // 检测鼠标是否在当前元素区域内
                if (item_rect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.MouseDown)
                    {
                        // 更新选中项
                        SelectedIndex = i;

                        // 标记界面需要更新
                        GUI.changed = true;
                    }
                }

                // 检查当前事件是否为键盘事件
                if (Event.current.type == EventType.KeyDown)
                {
                    // 检测是否按下 Delete 键
                    if (Event.current.keyCode == KeyCode.Delete)
                    {
                        // 确保有选中的项
                        if (SelectedIndex >= 0 && SelectedIndex < sp_element_list.arraySize)
                        {
                            // 删除选中的项
                            DeleteElementAtIndex(SelectedIndex);

                            // 标记事件已处理，防止其他逻辑重复处理
                            Event.current.Use();
                        }
                    }
                }

                // 绘制元素
                //EditorGUI.PropertyField(item_rect, prop, GUIContent.none);
            }

            GUI.EndScrollView();
        }
        #endregion

        private void OnDisable()
        {

        }

        private void OnEnable()
        {
            BaseObject = new SerializedObject(this);
            sp_library_name = BaseObject.FindProperty("library_name");
            sp_element_list = BaseObject.FindProperty("element_list");

            HudManager = XHud_Dashboard.HudManagerGet();

            icon_libsetter_element = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_element_setter/icon");

            #region ReorderableList
            ReorderableList = new ReorderableList(BaseObject, sp_element_list, true, true, true, true);
            ReorderableList.drawElementCallback = DrawElementCallback;
            ReorderableList.onRemoveCallback = Remove;
            #endregion

            #region 获取首个元素库名称
            /* 默认获取到的第一个元素库的名称，否则后面会报空，因为下拉菜单需要点击不同的项才会有反馈
             * 所以，如果只有一个元素库的时候，sp_library_name 默认是空的，
             * 就会导致后面获取不到，在这里处理一下就可以保证后面能正常获取到目标元素库
             */

            sp_library_name.stringValue = UpdateElementLibraryNames()[0];
            sp_library_name.serializedObject.ApplyModifiedProperties();
            #endregion
        }

        private void OnGUI()
        {
            BaseObject.Update();

            #region 抬头
            Rect rect = new Rect(0, 0, position.width, position.height);

            // 图标
            Rect rect_icon = new Rect(15, 15, icon_libsetter_element.width, icon_libsetter_element.height);
            XGUI.gui_icon(
                rect: rect_icon,
                icon: icon_libsetter_element,
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
                text: new GUIContent("元素库添加器"),
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
                text: new GUIContent("以下列表中的元素为待入库的Hud元素物体，您可以检查或剔除不需要入库的元素项，点击列表可查看元素的基础信息"),
                text_color: Color.white * 0.7f,
                size: XGUIFontSize.M,
                anchor: TextAnchor.UpperLeft,
                wrap: true,
                clipping: TextClipping.Overflow);
            #endregion

            LibSelector_rect = rect;

            string colorhex_value = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary, true);
            string colorhex_title = XGUI_Utilitys.Color_To_HexString(Color.white * 0.65f, true);

            #region 统计数量
            LibSelector_rect.Set(rect.x + rect.width - 120, rect.y + 15, 100, XGUI.GetSingleLineHeight());
            XGUI.gui_label(
               rect: LibSelector_rect,
               text: new GUIContent($"<color={colorhex_value}>{element_list.Count.ToString()}</color> <color={colorhex_title}> 项</color>"),
               text_color: Color.white,
               size: XGUIFontSize.M,
               anchor: TextAnchor.MiddleRight,
               clipping: TextClipping.Overflow);
            #endregion

            #region 目标入库选择           
            LibSelector_rect.Set(rect.x + (rect.width - 240), rect.y + 110, rect.width - 15, XGUI.GetSingleLineHeight());
            sp_library_name.stringValue = XGUI.gui_string_popup(
                  rect: LibSelector_rect,
                  title: "类型",
                  title_color: Color.white,
                  title_size: XGUIFontSize.M,
                  title_font_style: FontStyle.Normal,
                  title_padding: new RectOffset(0, 0, 0, 0),
                  title_width: 40,
                  prop: sp_library_name,
                  options: libnames,
                  opt_text_size: XGUIFontSize.M,
                  opt_text_color: Color.black,
                  opt_text_padding: new RectOffset(10, 10, 0, 0),
                  opt_anchor: TextAnchor.MiddleCenter,
                  opt_font_style: FontStyle.Normal,
                  opt_bg_fill: XGUIFilled.实体,
                  opt_bg_color: XGUIColor.亮白,
                  opt_bg_color_gui: XHud_Dashboard.Theme_Primary,
                  icon_arrow_color: Color.black);
            sp_library_name.serializedObject.ApplyModifiedProperties();
            #endregion

            #region 元素信息标题
            LibSelector_rect.Set(rect.x + (rect.width - 240), rect.y + 160, 100, 15);
            XGUI.gui_label(
                   rect: LibSelector_rect,
                   text: new GUIContent("元素信息"),
                   text_color: Color.white,
                   size: XGUIFontSize.M,
                   clipping: clipping,
                   anchor: TextAnchor.MiddleLeft,
                   offset: new Vector2(0, 0),
                   wrap: true,
                   font_style: FontStyle.Bold);

            if (element_list != null && element_list.Count > 0)
            {
                #region 名称
                LibSelector_rect.Set(rect.x + (rect.width - 240), rect.y + 190, rect.width - 15, 20);
                XGUI.gui_label(
                    rect: LibSelector_rect,
                    text: new GUIContent($"名称： <color={colorhex_value}>{element_list[SelectedIndex].Info.Name}</color>"),
                    text_color: Color.white,
                    size: XGUIFontSize.M,
                    clipping: clipping,
                    anchor: TextAnchor.MiddleLeft,
                    font_style: FontStyle.Normal);
                #endregion

                #region 标识
                LibSelector_rect.Set(rect.x + (rect.width - 240), rect.y + 220, rect.width - 15, 20);
                XGUI.gui_label(
                    rect: LibSelector_rect,
                    text: new GUIContent($"标识： <color={colorhex_value}>{(!string.IsNullOrEmpty(element_list[SelectedIndex].Info.Indicator) ? element_list[SelectedIndex].Info.Indicator : "未设置标识名称")} </color>"),
                    text_color: Color.white,
                    size: XGUIFontSize.M,
                    clipping: clipping,
                    anchor: TextAnchor.MiddleLeft,
                    font_style: FontStyle.Normal);
                #endregion

                #region 尺寸
                LibSelector_rect.Set(rect.x + (rect.width - 240), rect.y + 245, rect.width - 15, 20);
                XGUI.gui_label(
                    rect: LibSelector_rect,
                    text: new GUIContent($"尺寸： <color={colorhex_value}>{element_list[SelectedIndex].Info.Size} </color>"),
                    text_color: Color.white,
                    size: XGUIFontSize.M,
                    clipping: clipping,
                    anchor: TextAnchor.MiddleLeft,
                    font_style: FontStyle.Normal);
                #endregion

                #region 位置
                LibSelector_rect.Set(rect.x + (rect.width - 240), rect.y + 270, rect.width - 15, 20);
                XGUI.gui_label(
                    rect: LibSelector_rect,
                    text: new GUIContent($"位置： <color={colorhex_value}>{element_list[SelectedIndex].Info.Pos} </color>"),
                    text_color: Color.white,
                    size: XGUIFontSize.M,
                    clipping: clipping,
                    anchor: TextAnchor.MiddleLeft,
                    font_style: FontStyle.Normal);
                #endregion

                #region 旋转
                LibSelector_rect.Set(rect.x + (rect.width - 240), rect.y + 295, rect.width - 15, 20);
                XGUI.gui_label(
                    rect: LibSelector_rect,
                    text: new GUIContent($"旋转： <color={colorhex_value}>{element_list[SelectedIndex].Info.Rot} </color>"),
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
            DrawDataList();
            #endregion

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
        /// 发送到元素库
        /// </summary>
        private void SendToLibrary()
        {
            XHud_Library_Element target_library = HudManager.hm_ElementLibrary_GetTargetLibrary(sp_library_name.stringValue);

            string colorhex = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary, true);
            List<XGUIDialogListDatas> valids = new List<XGUIDialogListDatas>();

            for (int s = 0; s < element_list.Count; s++)
            {
                XGUIDialogListDatas item = new XGUIDialogListDatas();


                if (target_library.ElementLibrary_IsExist(element_list[s].Element.name))
                {
                    string res = XGUI.dialog(
                                type: XGUIDialogType.通知,
                                windowtitle: "XHud - 元素库添加器消息",
                                title: "存在重复元素名称",
                                msg: $"名称为<color={colorhex}> {element_list[s].Element.transform.name} </color>的元素已经存在于<color={colorhex}> \" {sp_library_name.stringValue} \" </color>元素库中，是否以当前元素更新替换库中的元素？",
                                ok: "更新替换",
                                cancel: "跳过",
                                PrimaryIndex: 1,
                                themecolor: XHud_Dashboard.Theme_Primary);

                    if (res == "更新替换")
                    {
                        target_library.ElementsLibrary_ReplaceElement(element_list[s].Element);

                        item.Title = element_list[s].Element.transform.name;
                        item.SubTitle = "替换库中元素";
                        item.Message = sp_library_name.stringValue;
                        valids.Add(item);
                    }
                }
                else
                {
                    target_library.ElementsLibrary_Add(element_list[s].Element, 1);

                    item.Title = element_list[s].Element.transform.name;
                    item.SubTitle = "新增元素到库";
                    item.Message = sp_library_name.stringValue;

                    valids.Add(item);
                }
            }

            if (valids.Count > 0)
            {
                XGUI.dialog_listview(
                    datas: valids.ToArray(),
                    type: XGUIDialogType.通知,
                    windowtitle: "XHud - 元素库添加器消息",
                    title: "元素入库",
                    msg: "以下是已被新增入库和被替换已有的元素列表，请您检查核对：",
                    ok: "明白",
                    PrimaryIndex: 0,
                    usemodal: false,
                    themecolor: XHud_Dashboard.Theme_Primary);
            }
        }
        /// <summary>
        /// 设置元素
        /// </summary>
        /// <param name="eles"></param>
        public void SetElements(List<XHud_Module_Element> eles)
        {
            if (element_list == null)
                element_list = new List<ReadyLibElement>();

            for (int i = 0; i < eles.Count; i++)
            {
                ReadyLibElement ready = new ReadyLibElement();
                ready.Element = eles[i];

                ready.Info = new ReadyElementInfo();
                ready.Info.Name = eles[i].name;
                ready.Info.Indicator = eles[i].Indicator;
                ready.Info.Pos = eles[i].RectTransform.anchoredPosition3D;
                ready.Info.Size = eles[i].RectTransform.sizeDelta;
                ready.Info.Rot = eles[i].RectTransform.localEulerAngles;

                element_list.Add(ready);
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
                text: "添加",
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
                SendToLibrary();
                Close();
                return;
            }
            GUI.backgroundColor = Color.white;
            XGUI.SetEnabled(true);

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
        }
        /// <summary>
        /// 获取所有已装配的元素库名称列表
        /// </summary>
        /// <returns></returns>
        private string[] UpdateElementLibraryNames()
        {
            return libnames = HudManager.hm_ElementLibrary_GetAllLibraryNames();
        }
    }
}
