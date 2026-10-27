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
    using SevenStrikeModules.XHud.Enums;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    #region 数据模型
    /// <summary>
    /// 待入库元素的简要信息。
    /// <para/>
    /// 用于在元素库添加器中展示元素的基础参数。
    /// </summary>
    [System.Serializable]
    public class ReadyElementInfo
    {
        /// <summary>
        /// 元素名称。
        /// </summary>
        public string name;
        /// <summary>
        /// 元素标识名称。
        /// </summary>
        public string indicator;
        /// <summary>
        /// 元素尺寸（RectTransform.sizeDelta）。
        /// </summary>
        public Vector2 size;
        /// <summary>
        /// 元素位置（anchoredPosition3D）。
        /// </summary>
        public Vector3 pos;
        /// <summary>
        /// 元素旋转（localEulerAngles）。
        /// </summary>
        public Vector3 rot;
    }
    /// <summary>
    /// 待入库元素项。
    /// <para/>
    /// 组合元素引用与简要信息，构成添加器列表的单项数据。
    /// </summary>
    [System.Serializable]
    public class ReadyLibElement
    {
        /// <summary>
        /// 元素组件引用。
        /// </summary>
        [SerializeField]
        public XHud_Module_Element element;
        /// <summary>
        /// 元素简要信息。
        /// </summary>
        [SerializeField]
        public ReadyElementInfo info;
    }
    #endregion

    public class Editor_XHud_LibrarySetTool_ElementImporter : EditorWindow
    {
        #region 字段 - 数据
        /// <summary>
        /// XHud 管理器实例。
        /// <para/>
        /// 用于获取元素库集合与执行入库操作。
        /// </summary>
        private XHud_Manager hud_manager;
        /// <summary>
        /// 待入库元素列表。
        /// </summary>
        public List<ReadyLibElement> element_list = new List<ReadyLibElement>();
        /// <summary>
        /// 目标元素库名称。
        /// </summary>
        [SerializeField]
        public string library_name;
        /// <summary>
        /// 日期时间标识。
        /// </summary>
        [SerializeField]
        public string date_times;
        /// <summary>
        /// 元素库名称缓存。
        /// </summary>
        string[] libnames;
        #endregion

        #region 字段 - 序列化对象 / 属性
        /// <summary>
        /// 用于序列化当前窗口的自定义字段。
        /// </summary>
        private SerializedObject base_object;
        /// <summary>
        /// 目标元素库名称的序列化属性。
        /// </summary>
        private SerializedProperty sp_library_name;
        /// <summary>
        /// 待入库元素列表的序列化属性。
        /// </summary>
        private SerializedProperty sp_element_list;
        #endregion

        #region 字段 - 列表
        /// <summary>
        /// 待入库元素列表（可重排列表）。
        /// </summary>
        public ReorderableList ReorderableList;
        /// <summary>
        /// 列表项高度。
        /// </summary>
        public float itemHeight = 25;
        /// <summary>
        /// 可视区域显示的元素数量。
        /// </summary>
        public int visibleItemCount = 8;
        /// <summary>
        /// 列表滚动位置。
        /// </summary>
        public Vector2 DataList_Scroller;
        /// <summary>
        /// 选中项索引号。
        /// </summary>
        public int SelectedIndex = 0;
        #endregion

        #region 字段 - 图标
        /// <summary>
        /// 窗口 Logo 图标。
        /// </summary>
        private Texture2D icon_libsetter_element;
        #endregion

        #region 字段 - 绘制缓存
        /// <summary>
        /// 元素库选择区矩形。
        /// </summary>
        Rect LibSelector_rect;
        /// <summary>
        /// 滚动视图矩形。
        /// </summary>
        Rect scrollview_rect;
        /// <summary>
        /// 列表项矩形。
        /// </summary>
        Rect item_rect;
        /// <summary>
        /// 列表项选中标记矩形。
        /// </summary>
        Rect itemmark_rect;
        /// <summary>
        /// 列表项选中背景矩形。
        /// </summary>
        Rect itemmarkBg_rect;
        #endregion

        #region 菜单项
        /// <summary>
        /// 校验右键菜单是否可用。
        /// <para/>
        /// 仅当选中对象包含 <see cref="XHud_Module_Element"/> 组件时返回 true。
        /// </summary>
        /// <returns>true 表示可以显示菜单项</returns>
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
        /// <summary>
        /// 打开元素库添加器窗口。
        /// <para/>
        /// 执行流程：
        /// <list type="number">
        /// <item><description>校验 XHud 管理器与元素库是否配置；</description></item>
        /// <item><description>收集所有选中对象上的 <see cref="XHud_Module_Element"/>；</description></item>
        /// <item><description>打开并初始化添加器窗口。</description></item>
        /// </list>
        /// </summary>
        [MenuItem("Assets/XHud/SendToElementLibrary (将元素预制体添加到元素库)", priority = 3000)]
        private static void OpenElementLibrarySetter()
        {
            XHud_Manager mgr = XHud_Dashboard.get_manager();

            if (mgr == null)
            {
                XGUI.dialog(
                    type: XGUIDialogType.通知,
                    windowtitle: "XHud - 元素库添加器消息",
                    title: "未发现XHud管理器",
                    msg: "场景中未找到XHudManager管理器！",
                    ok: "明白",
                    PrimaryIndex: 0,
                    themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
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
                        themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
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

        #region 生命周期
        /// <summary>
        /// Unity 启用回调。
        /// <para/>
        /// 执行流程：
        /// <list type="number">
        /// <item><description>构建序列化对象并绑定属性；</description></item>
        /// <item><description>获取 XHud 管理器；</description></item>
        /// <item><description>加载窗口 Logo 图标；</description></item>
        /// <item><description>构造待入库元素列表；</description></item>
        /// <item><description>初始化默认目标元素库名称。</description></item>
        /// </list>
        /// </summary>
        private void OnEnable()
        {
            base_object = new SerializedObject(this);
            sp_library_name = base_object.FindProperty(nameof(library_name));
            sp_element_list = base_object.FindProperty(nameof(element_list));

            hud_manager = XHud_Dashboard.get_manager();

            icon_libsetter_element = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_element_setter/icon");

            #region ReorderableList
            ReorderableList = new ReorderableList(base_object, sp_element_list, true, true, true, true);
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
        /// <summary>
        /// Unity 禁用回调。
        /// <para/>
        /// 当前实现为空，保留以支持后续清理逻辑。
        /// </summary>
        private void OnDisable()
        {

        }
        /// <summary>
        /// 绘制窗口 GUI。
        /// <para/>
        /// 按自上而下的顺序绘制：
        /// 抬头 → 目标入库选择 → 元素信息（名称 / 标识 / 尺寸 / 位置 / 旋转） → 待入库列表 → 按钮。
        /// <para/>
        /// 支持 Escape 键关闭窗口。
        /// </summary>
        private void OnGUI()
        {
            base_object.Update();

            TextClipping clipping_ellipsis = XGUI.TryEllipsisClipping();

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


            // 大标题
            Rect rect_title = new Rect(rect.x + 70, rect.y + 10, rect.width - 80, 30);
            XGUI.gui_label(
                rect: rect_title,
                text: new GUIContent("元素库添加器"),
                text_color: Color.white,
                size: XGUIFontSize.L,
                clipping: clipping_ellipsis,
                font: XGUI.GetFont("xg-heavy"));

            // 分割线
            Rect rect_seperate = new Rect(rect.x + 68, rect.y + 43, 200, 1);
            XGUI.gui_seperator(
                rect: rect_seperate,
                thickness: 1,
                color: XHud_Dashboard.themes_get_color(ThemeColorType.sepline),
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

            string colorhex_value = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.themes_get_color(ThemeColorType.primary), true);
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
            LibSelector_rect.Set(rect.x + (rect.width - 240), rect.y + 110, rect.width - (rect.x + (rect.width - 220)), XGUI.GetSingleLineHeight());
            sp_library_name.stringValue = XGUI.gui_string_popup(
                  rect: LibSelector_rect,
                  title: "目标库",
                  title_color: Color.white,
                  title_size: XGUIFontSize.M,
                  title_font_style: FontStyle.Normal,
                  title_padding: new RectOffset(0, 0, 0, 0),
                  title_width: 45,
                  prop: sp_library_name,
                  options: libnames,
                  opt_text_size: XGUIFontSize.M,
                  opt_text_color: Color.black,
                  opt_text_padding: new RectOffset(10, 10, 0, 0),
                  opt_anchor: TextAnchor.MiddleCenter,
                  opt_font_style: FontStyle.Normal,
                  opt_bg_fill: XGUIGroup.实体,
                  opt_bg_color: XGUIColor.亮白,
                  opt_bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                  icon_arrow_color: Color.black,
                  usearrow: true,
                  width_limite: 100);
            sp_library_name.serializedObject.ApplyModifiedProperties();
            #endregion

            // 分割线
            rect_seperate.Set(rect.x + (rect.width - 240), rect.y + 145, 200, 1);
            XGUI.gui_seperator(
                rect: rect_seperate,
                thickness: 1,
                color: XHud_Dashboard.themes_get_color(ThemeColorType.sepline),
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0));

            #region 元素信息标题
            LibSelector_rect.Set(rect.x + (rect.width - 240), rect.y + 160, 100, 15);
            XGUI.gui_label(
                   rect: LibSelector_rect,
                   text: new GUIContent("元素信息"),
                   text_color: Color.white,
                   size: XGUIFontSize.M,
                   clipping: clipping_ellipsis,
                   anchor: TextAnchor.MiddleLeft,
                   offset: new Vector2(0, 0),
                   wrap: true,
                   font_style: FontStyle.Bold);

            float margin = 165;
            float step = 30;

            if (element_list != null && element_list.Count > 0)
            {
                #region 名称
                margin += step;
                LibSelector_rect.Set(rect.x + (rect.width - 240), rect.y + margin, rect.width - 15, 20);
                XGUI.gui_label(
                    rect: LibSelector_rect,
                    text: new GUIContent($"名称： <color={colorhex_value}>{element_list[SelectedIndex].info.name}</color>"),
                    text_color: Color.white,
                    size: XGUIFontSize.M,
                    clipping: clipping_ellipsis,
                    anchor: TextAnchor.MiddleLeft,
                    font_style: FontStyle.Normal);
                #endregion

                #region 标识
                margin += step;
                LibSelector_rect.Set(rect.x + (rect.width - 240), rect.y + margin, rect.width - 15, 20);
                XGUI.gui_label(
                    rect: LibSelector_rect,
                    text: new GUIContent($"标识： <color={colorhex_value}>{(!string.IsNullOrEmpty(element_list[SelectedIndex].info.indicator) ? element_list[SelectedIndex].info.indicator : "未设置标识名称")} </color>"),
                    text_color: Color.white,
                    size: XGUIFontSize.M,
                    clipping: clipping_ellipsis,
                    anchor: TextAnchor.MiddleLeft,
                    font_style: FontStyle.Normal);
                #endregion

                #region 尺寸
                margin += step;
                LibSelector_rect.Set(rect.x + (rect.width - 240), rect.y + margin, rect.width - 15, 20);
                XGUI.gui_label(
                    rect: LibSelector_rect,
                    text: new GUIContent($"尺寸： <color={colorhex_value}>{element_list[SelectedIndex].info.size} </color>"),
                    text_color: Color.white,
                    size: XGUIFontSize.M,
                    clipping: clipping_ellipsis,
                    anchor: TextAnchor.MiddleLeft,
                    font_style: FontStyle.Normal);
                #endregion

                #region 位置
                margin += step;
                LibSelector_rect.Set(rect.x + (rect.width - 240), rect.y + margin, rect.width - 15, 20);
                XGUI.gui_label(
                    rect: LibSelector_rect,
                    text: new GUIContent($"位置： <color={colorhex_value}>{element_list[SelectedIndex].info.pos} </color>"),
                    text_color: Color.white,
                    size: XGUIFontSize.M,
                    clipping: clipping_ellipsis,
                    anchor: TextAnchor.MiddleLeft,
                    font_style: FontStyle.Normal);
                #endregion

                #region 旋转
                margin += step;
                LibSelector_rect.Set(rect.x + (rect.width - 240), rect.y + margin, rect.width - 15, 20);
                XGUI.gui_label(
                    rect: LibSelector_rect,
                    text: new GUIContent($"旋转： <color={colorhex_value}>{element_list[SelectedIndex].info.rot} </color>"),
                    text_color: Color.white,
                    size: XGUIFontSize.M,
                    clipping: clipping_ellipsis,
                    anchor: TextAnchor.MiddleLeft,
                    font_style: FontStyle.Normal);
                #endregion
            }
            #endregion

            XGUI.layout_space(110);

            #region 列表
            DrawDataList();
            #endregion

            base_object.ApplyModifiedProperties();

            Repaint();

            XGUI.layout_space(50);

            bottom_buttons();

            #region 检测事件
            Event e = Event.current;
            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
            {
                e.Use();
                Close();
            }
            #endregion
        }
        #endregion

        #region 列表绘制
        /// <summary>
        /// 列表移除回调。
        /// <para/>
        /// 当前实现为调试占位。
        /// </summary>
        /// <param name="list">可重排列表</param>
        private void Remove(ReorderableList list)
        {
            Debug.Log(1);
        }
        /// <summary>
        /// 绘制元素回调。
        /// <para/>
        /// 绘制内容：序号（灰色，D2 格式）+ 待入库元素名称。
        /// </summary>
        /// <param name="rect">绘制矩形</param>
        /// <param name="index">索引</param>
        /// <param name="isActive">是否激活</param>
        /// <param name="isFocused">是否聚焦</param>
        private void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty prop = sp_element_list.GetArrayElementAtIndex(index);
            SerializedProperty prop_Info = prop.FindPropertyRelative("info");
            SerializedProperty sp_name = prop_Info.FindPropertyRelative("name");

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
        /// <summary>
        /// 删除待入库列表中的指定索引项。
        /// <para/>
        /// 删除后自动调整当前选中索引，并刷新界面。
        /// </summary>
        /// <param name="index">要删除的索引</param>
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
        /// 绘制待入库元素列表。
        /// <para/>
        /// 只绘制滚动视图可视区域内的元素，以优化大列表性能。
        /// 单击列表项时更新右侧元素信息。
        /// 支持 Delete 键从列表中移除选中项。
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
                    EditorGUI.DrawRect(itemmark_rect, XHud_Dashboard.themes_get_color(ThemeColorType.primary));
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

        #region 工具 - 元素入库
        /// <summary>
        /// 将待入库元素列表发送到目标元素库。
        /// <para/>
        /// 执行流程：
        /// <list type="number">
        /// <item><description>获取目标元素库；</description></item>
        /// <item><description>遍历待入库元素，重复项弹窗询问更新替换或跳过；</description></item>
        /// <item><description>收集操作结果并在结束时弹窗显示。</description></item>
        /// </list>
        /// </summary>
        private void SendToLibrary()
        {
            XHud_Library_Element target_library = hud_manager.hm_ElementLibrary_GetTargetLibrary(sp_library_name.stringValue);

            string colorhex = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.themes_get_color(ThemeColorType.primary), true);
            List<XGUIDialogListDatas> valids = new List<XGUIDialogListDatas>();

            for (int s = 0; s < element_list.Count; s++)
            {
                XGUIDialogListDatas item = new XGUIDialogListDatas();


                if (target_library.ElementLibrary_IsExist(element_list[s].element.name))
                {
                    string res = XGUI.dialog(
                                type: XGUIDialogType.通知,
                                windowtitle: "XHud - 元素库添加器消息",
                                title: "存在重复元素名称",
                                msg: $"名称为<color={colorhex}> {element_list[s].element.transform.name} </color>的元素已经存在于<color={colorhex}> \" {sp_library_name.stringValue} \" </color>元素库中，是否以当前元素更新替换库中的元素？",
                                ok: "更新替换",
                                cancel: "跳过",
                                PrimaryIndex: 1,
                                themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));

                    if (res == "更新替换")
                    {
                        target_library.ElementsLibrary_ReplaceElement(element_list[s].element);

                        item.Title = element_list[s].element.transform.name;
                        item.SubTitle = "替换库中元素";
                        item.Message = sp_library_name.stringValue;
                        valids.Add(item);
                    }
                }
                else
                {
                    target_library.ElementsLibrary_Add(element_list[s].element, 1);

                    item.Title = element_list[s].element.transform.name;
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
                    themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
            }
        }
        #endregion

        #region 设置接口
        /// <summary>
        /// 设置待入库元素列表。
        /// <para/>
        /// 从传入的元素组件中采集名称 / 标识 / 尺寸 / 位置 / 旋转信息，
        /// 并填充到 <see cref="element_list"/> 中。
        /// </summary>
        /// <param name="eles">待入库元素组件列表</param>
        public void SetElements(List<XHud_Module_Element> eles)
        {
            if (element_list == null)
                element_list = new List<ReadyLibElement>();

            for (int i = 0; i < eles.Count; i++)
            {
                ReadyLibElement ready = new ReadyLibElement();
                ready.element = eles[i];

                ready.info = new ReadyElementInfo();
                ready.info.name = eles[i].name;
                ready.info.indicator = eles[i].indicator;
                ready.info.pos = eles[i].rect_transform.anchoredPosition3D;
                ready.info.size = eles[i].rect_transform.sizeDelta;
                ready.info.rot = eles[i].rect_transform.localEulerAngles;

                element_list.Add(ready);
            }
        }
        #endregion

        #region 按钮
        /// <summary>
        /// 绘制底部按钮区（关闭 / 添加）。
        /// <para/>
        /// 播放模式下禁用添加按钮，防止运行时修改元素库数据。
        /// </summary>
        private void bottom_buttons()
        {
            XGUI.layout_group_start(
                type: XGUIContainerType.Horizontal,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(250, 15, 2, 15));

            if (XGUI.layout_button(
                text: "关闭",
                tooltip: "",
                bg_fill: XGUIGroup.实体,
                bg_color: XGUIColor.亮白,
                bg_color_gui: Color.white,
                button_text_color: Color.black,
                press_fill: XGUIGroup.实体,
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
                bg_fill: XGUIGroup.实体,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                button_text_color: Color.black,
                press_fill: XGUIGroup.实体,
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
        #endregion

        #region 工具 - 元素库
        /// <summary>
        /// 获取所有已装配的元素库名称列表。
        /// <para/>
        /// 结果同时被缓存到 <see cref="libnames"/> 中供下拉菜单使用。
        /// </summary>
        /// <returns>元素库名称数组</returns>
        private string[] UpdateElementLibraryNames()
        {
            return libnames = hud_manager.hm_ElementLibrary_GetAllLibraryNames();
        }
        #endregion
    }
}