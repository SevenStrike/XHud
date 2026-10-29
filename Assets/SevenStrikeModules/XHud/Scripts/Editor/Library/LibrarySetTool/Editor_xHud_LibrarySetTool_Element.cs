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
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    public class Editor_XHud_LibrarySetTool_Element : EditorWindow
    {
        #region 字段 - 数据
        /// <summary>
        /// 当前正在编辑的元素库项（工作副本）。
        /// </summary>
        public XHud_LibraryArg_Element_Item library_item;
        /// <summary>
        /// 元素库项原始数据（用于回退 / 对比）。
        /// </summary>
        public XHud_LibraryArg_Element_Item original_library_item;
        /// <summary>
        /// 被修改的元素库项索引。
        /// </summary>
        public int modified_index;
        /// <summary>
        /// 目标元素库。
        /// </summary>
        public XHud_Library_Element target_lib;
        /// <summary>
        /// 目标元素库名称。
        /// </summary>
        [SerializeField]
        public string target_lib_name;
        /// <summary>
        /// 日期时间标识。
        /// </summary>
        [SerializeField]
        public string date_times;
        #endregion

        #region 字段 - 序列化对象
        /// <summary>
        /// 用于序列化当前窗口的自定义字段
        /// </summary>
        private SerializedObject base_object;
        #endregion

        #region 字段 - 序列化属性
        /// <summary>
        /// 预生成队列属性
        /// </summary>
        private SerializedProperty sp_preload_elements;
        /// <summary>
        /// 目标属性
        /// </summary>
        private SerializedProperty sp_target;
        /// <summary>
        /// 根节点属性
        /// </summary>
        private SerializedProperty sp_root;
        /// <summary>
        /// 下一个索引属性
        /// </summary>
        private SerializedProperty sp_next_index;
        /// <summary>
        /// 已回收数量属性
        /// </summary>
        private SerializedProperty sp_recycled_count;
        /// <summary>
        /// 已使用数量属性
        /// </summary>
        private SerializedProperty sp_used_count;
        /// <summary>
        /// 预加载数量属性
        /// </summary>
        private SerializedProperty sp_initialize_count;
        /// <summary>
        /// 名称属性
        /// </summary>
        private SerializedProperty sp_name;
        #endregion

        #region 字段 - 列表
        /// <summary>
        /// 预生成元素队列列表（只读可重排列表）。
        /// </summary>
        public ReorderableList list_preload_elements;
        /// <summary>
        /// 列表项高度。
        /// </summary>
        public float item_height = 25;
        /// <summary>
        /// 可视区域显示的元素数量。
        /// </summary>
        public int visible_item_count = 8;
        /// <summary>
        /// 列表滚动位置。
        /// </summary>
        public Vector2 data_list_scroller;
        /// <summary>
        /// 选中项索引号。
        /// </summary>
        public int selected_library_index = 0;
        /// <summary>
        /// 窗口标题文字。
        /// </summary>
        public string title_primary = "元素项修改器";
        /// <summary>
        /// 窗口副标题。
        /// </summary>
        string title_sub = "您可以按照需要查看已生成的预制元素列表，或者修改目标元素的库参数特性！";
        #endregion

        #region 字段 - 图标
        /// <summary>
        /// 窗口 Logo 图标。
        /// </summary>
        private Texture2D icon_logo;
        #endregion

        #region 字段 - 绘制缓存
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

        #region 生命周期
        /// <summary>
        /// Unity 启用回调。
        /// <para/>
        /// 执行流程：
        /// <list type="number">
        /// <item><description>构建序列化对象；</description></item>
        /// <item><description>绑定全部序列化属性；</description></item>
        /// <item><description>加载窗口 Logo 图标；</description></item>
        /// <item><description>构造只读的预生成队列列表。</description></item>
        /// </list>
        /// </summary>
        private void OnEnable()
        {
            base_object = new SerializedObject(this);

            #region 获取序列化属性
            SerializedProperty prop = base_object.FindProperty(nameof(library_item));
            sp_preload_elements = prop.FindPropertyRelative(nameof(XHud_LibraryArg_Element_Item.preload_elements));
            sp_target = prop.FindPropertyRelative(nameof(XHud_LibraryArg_Element_Item.target));
            sp_root = prop.FindPropertyRelative(nameof(XHud_LibraryArg_Element_Item.root));
            sp_next_index = prop.FindPropertyRelative(nameof(XHud_LibraryArg_Element_Item.next_index));
            sp_recycled_count = prop.FindPropertyRelative(nameof(XHud_LibraryArg_Element_Item.recycled_count));
            sp_used_count = prop.FindPropertyRelative(nameof(XHud_LibraryArg_Element_Item.used_count));
            sp_initialize_count = prop.FindPropertyRelative(nameof(XHud_LibraryArg_Element_Item.initialize_count));
            sp_name = prop.FindPropertyRelative(nameof(XHud_LibraryArg_Element_Item.name));
            #endregion

            #region 获取图标
            icon_logo = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_element_setter/icon");
            #endregion

            #region ReorderableList
            list_preload_elements = new ReorderableList(base_object, sp_preload_elements, false, false, false, false);
            list_preload_elements.drawElementCallback = List_PreloadElementList_DrawElementCallback;
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
        /// 抬头 → 预生成元素列表 → 参数（目标元素名称 / 元素源 / 根节点 / 预加载数量 / 状态统计） → 按钮。
        /// <para/>
        /// 支持 Escape 键关闭窗口与点击空白区域失焦。
        /// </summary>
        private void OnGUI()
        {
            base_object.Update();

            TextClipping clipping_ellipsis = XGUI.TryEllipsisClipping();

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
                text: new GUIContent(title_primary),
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
                text: new GUIContent(title_sub),
                text_color: Color.white * 0.7f,
                size: XGUIFontSize.M,
                anchor: TextAnchor.UpperLeft,
                clipping: clipping_ellipsis);
            #endregion

            #region 预生成元素列表
            if (sp_preload_elements.arraySize > 0)
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
                    clipping: clipping_ellipsis,
                    anchor: TextAnchor.MiddleCenter,
                    offset: new Vector2(0, 0),
                    font_style: FontStyle.Normal);

            }
            #endregion

            #region 参数

            string colorhex_value = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.themes_get_color(ThemeColorType.primary), true);
            string colorhex_title = XGUI_Utilitys.Color_To_HexString(Color.white * 0.65f, true);

            Rect rect_xs = rect;

            #region 目标元素名称
            rect_xs.Set(rect.width - 240, rect.y + 100, 215, 20);
            XGUI.gui_label(
                rect: rect_xs,
                text: new GUIContent(sp_name.stringValue),
                text_color: Color.white,
                size: XGUIFontSize.L,
                clipping: clipping_ellipsis,
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
                color: XHud_Dashboard.themes_get_color(ThemeColorType.sepline),
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
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 80,
                prop: sp_target);
            if (XGUI.ChangedCheck_End())
            {
                if (sp_target.objectReferenceValue != null)
                {
                    XHud_Module_Element element = (XHud_Module_Element)sp_target.objectReferenceValue as XHud_Module_Element;
                    sp_name.stringValue = element.name;
                }
                else
                {
                    sp_name.stringValue = null;
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
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 80,
                prop: sp_root);
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
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 80,
                prop: sp_initialize_count);
            #endregion

            XGUI.SetEnabled(true);

            #region 分割线
            rect_xs.Set(rect.width - 240, rect.y + 195 + AddedHeight, 215, 1);
            XGUI.gui_seperator(
                rect: rect_xs,
                thickness: 1,
                color: XHud_Dashboard.themes_get_color(ThemeColorType.sepline),
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0));
            #endregion

            #region 已被使用
            rect_xs.Set(rect.width - 240, rect.y + 210 + AddedHeight, 215, 20);
            XGUI.gui_label(
                rect: rect_xs,
                text: new GUIContent($"<color={colorhex_title}>已被使用  (UsedCount) ： </color> <color={colorhex_value}>{sp_used_count.intValue}</color>  个"),
                text_color: Color.white,
                size: XGUIFontSize.M,
                clipping: clipping_ellipsis,
                anchor: TextAnchor.MiddleLeft,
                offset: new Vector2(0, 0),
                wrap: true,
                font_style: FontStyle.Normal);
            #endregion

            #region 已被回收
            rect_xs.Set(rect.width - 240, rect.y + 240 + AddedHeight, 215, 20);
            XGUI.gui_label(
                rect: rect_xs,
                text: new GUIContent($"<color={colorhex_title}>已被回收  (RecycledCount) ： </color> <color={colorhex_value}>{sp_recycled_count.intValue}</color>  个"),
                text_color: Color.white,
                size: XGUIFontSize.M,
                clipping: clipping_ellipsis,
                anchor: TextAnchor.MiddleLeft,
                offset: new Vector2(0, 0),
                wrap: true,
                font_style: FontStyle.Normal);
            #endregion

            #region 下一个索引
            rect_xs.Set(rect.width - 240, rect.y + 270 + AddedHeight, 215, 20);
            XGUI.gui_label(
                rect: rect_xs,
                text: new GUIContent($"<color={colorhex_title}>下一个索引  (NextIndex) ： </color> <color={colorhex_value}>{sp_next_index.intValue}</color>"),
                text_color: Color.white,
                size: XGUIFontSize.M,
                clipping: clipping_ellipsis,
                anchor: TextAnchor.MiddleLeft,
                offset: new Vector2(0, 0),
                wrap: true,
                font_style: FontStyle.Normal);
            #endregion
            #endregion

            Repaint();

            bottom_buttons();

            #region 检测事件
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
            #endregion

            if (base_object.targetObject != null)
                base_object.ApplyModifiedProperties();
        }
        #endregion

        #region 列表绘制
        /// <summary>
        /// 绘制预生成元素列表项。
        /// <para/>
        /// 绘制内容：序号（灰色，D2 格式）+ 预制元素名称。
        /// </summary>
        /// <param name="rect">绘制矩形</param>
        /// <param name="index">索引</param>
        /// <param name="isActive">是否激活</param>
        /// <param name="isFocused">是否聚焦</param>
        private void List_PreloadElementList_DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty prop = sp_preload_elements.GetArrayElementAtIndex(index);
            SerializedProperty sp_ele = prop.FindPropertyRelative(nameof(XHud_LibraryArg_Element_Info.element));

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
        /// 绘制预生成元素队列列表。
        /// <para/>
        /// 只绘制滚动视图可视区域内的元素，以优化大列表性能。
        /// 单击列表项时通过 <see cref="EditorGUIUtility.PingObject(Object)"/> 定位到对应的预制元素。
        /// </summary>
        private void List_PreloadElementList_Drawer()
        {
            // 绘制滚动视图
            scrollview_rect = GUILayoutUtility.GetRect(0, visible_item_count * item_height);
            scrollview_rect.x = 26;
            scrollview_rect.width = 220;
            scrollview_rect.height = 310;
            data_list_scroller = GUI.BeginScrollView(scrollview_rect, data_list_scroller, new Rect(0, 0, scrollview_rect.width - 50, library_item.preload_elements.Count * item_height), false, true);

            // 计算可视区域的起始和结束索引
            int startIndex = Mathf.FloorToInt(data_list_scroller.y / item_height);
            int endIndex = Mathf.CeilToInt((data_list_scroller.y + scrollview_rect.height) / item_height);

            // 只绘制可视区域内的元素
            for (int i = startIndex; i < endIndex && i < list_preload_elements.count; i++)
            {
                SerializedProperty prop = sp_preload_elements.GetArrayElementAtIndex(i);

                item_rect = new Rect(0, i * item_height, scrollview_rect.width, item_height);

                // 如果当前元素被选中，绘制选中效果
                if (selected_library_index == i)
                {
                    itemmark_rect = new Rect(item_rect.x + 1, item_rect.y + 11, 5, 5);
                    // 高亮标记表示选中
                    EditorGUI.DrawRect(itemmark_rect, XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                    itemmarkBg_rect = new Rect(item_rect.x, item_rect.y, item_rect.width + 20, item_rect.height);
                    // 高亮背景表示选中
                    EditorGUI.DrawRect(itemmarkBg_rect, new Color(0, 0, 0, 0.2f));
                }

                list_preload_elements.drawElementCallback.Invoke(item_rect, i, i == list_preload_elements.index, true);

                // 检测鼠标是否在当前元素区域内
                if (item_rect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.MouseDown)
                    {
                        // 更新选中项
                        selected_library_index = i;

                        XHud_LibraryArg_Element_Info preload = library_item.preload_elements[i];
                        EditorGUIUtility.PingObject(preload.element);

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

        #region 按钮
        /// <summary>
        /// 绘制底部按钮区（关闭 / 更新）。
        /// <para/>
        /// 播放模式下禁用更新按钮，防止运行时修改元素库数据。
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
                text: "更新",
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
                Save();
            }
            GUI.backgroundColor = Color.white;
            XGUI.SetEnabled(true);

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
        }
        #endregion

        #region 设置接口
        /// <summary>
        /// 设置元素信息。
        /// <para/>
        /// 将传入的元素项数据同时拷贝到工作副本与原始副本。
        /// </summary>
        /// <param name="item">元素库项数据</param>
        public void SetLibraryItem(XHud_LibraryArg_Element_Item item)
        {
            library_item.Copy(item);
            original_library_item.Copy(item);
        }
        /// <summary>
        /// 设置目标库名称。
        /// </summary>
        /// <param name="name">库名称</param>
        internal void SetTargetLibName(string name)
        {
            target_lib_name = name;
        }
        /// <summary>
        /// 设置目标元素库。
        /// </summary>
        /// <param name="lib">目标元素库</param>
        internal void SetTargetLib(XHud_Library_Element lib)
        {
            target_lib = lib;
        }
        #endregion

        #region 工具 - 保存
        /// <summary>
        /// 保存元素库项修改。
        /// <para/>
        /// 用当前工作副本替换目标元素库中指定索引的项，然后关闭窗口。
        /// </summary>
        private void Save()
        {
            #region 更新元素库目标元素参数

            target_lib.ElementsLibrary_ReplaceElement(modified_index, library_item);

            #endregion

            Close();
            return;
        }
        #endregion
    }
}