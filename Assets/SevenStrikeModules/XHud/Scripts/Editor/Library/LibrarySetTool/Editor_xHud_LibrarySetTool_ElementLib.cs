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
    /// 元素库简要信息。
    /// <para/>
    /// 用于在元素库选择器中展示每个元素库的名称、说明与统计信息。
    /// </summary>
    [SerializeField]
    [System.Serializable]
    public class ElementLibraryInfo
    {
        /// <summary>
        /// 元素库名称。
        /// </summary>
        [SerializeField]
        public string lib_name;
        /// <summary>
        /// 元素库说明。
        /// </summary>
        [SerializeField]
        public string lib_description;
        /// <summary>
        /// 元素库中元素总数。
        /// </summary>
        [SerializeField]
        public int lib_count;
        /// <summary>
        /// 正在使用中的元素数。
        /// </summary>
        [SerializeField]
        public int lib_used_count;
        /// <summary>
        /// 已回收的元素数。
        /// </summary>
        [SerializeField]
        public int lib_recycled_count;
        /// <summary>
        /// 预初始化的元素数。
        /// </summary>
        [SerializeField]
        public int lib_initiated_count;
    }
    #endregion

    #region 编辑器窗口
    /// <summary>
    /// XHud 元素库选择器窗口。
    /// <para/>
    /// 展示所有已部署到 XHud 管理器的元素库，并可查看每个库的详细统计信息，
    /// 支持一键定位 / 打开目标元素库。
    /// </summary>
    public class Editor_XHud_LibrarySetTool_ElementLib : EditorWindow
    {
        #region 字段 - 组件 / 数据
        /// <summary>
        /// XHud 管理器实例。
        /// <para/>
        /// 用于获取元素库集合与执行打开操作。
        /// </summary>
        private XHud_Manager HudManager;
        /// <summary>
        /// 元素库简要信息列表。
        /// </summary>
        public List<ElementLibraryInfo> ElementLibrarys = new List<ElementLibraryInfo>();
        #endregion

        #region 字段 - 序列化对象 / 属性
        /// <summary>
        /// 用于序列化当前窗口的自定义字段。
        /// </summary>
        private SerializedObject BaseObject;
        /// <summary>
        /// 元素库简要信息列表的序列化属性。
        /// </summary>
        private SerializedProperty sp_ElementLibrarys;
        #endregion

        #region 字段 - 列表
        /// <summary>
        /// 元素库简要信息列表（只读可重排列表）。
        /// </summary>
        public ReorderableList List_ElementLibrarys;
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
        public int SelectedLibraryIndex = 0;
        #endregion

        #region 字段 - 图标
        /// <summary>
        /// 窗口 Logo 图标。
        /// </summary>
        private Texture2D icon_logo;
        #endregion

        #region 字段 - 样式
        /// <summary>
        /// 分隔线颜色。
        /// </summary>
        Color SepLineColor = new Color(1, 1, 1, 0.15f);
        /// <summary>
        /// 提示信息文字颜色。
        /// </summary>
        Color MessageColor = new Color(1, 1, 1, 0.75f);
        /// <summary>
        /// 日期时间文字颜色。
        /// </summary>
        Color DateTimeColor = new Color(1, 1, 1, 0.42f);
        #endregion

        #region 字段 - 绘制缓存
        /// <summary>
        /// 分隔线矩形。
        /// </summary>
        Rect Sepline_rect;
        /// <summary>
        /// 标题矩形。
        /// </summary>
        Rect Title_rect;
        /// <summary>
        /// 日期矩形。
        /// </summary>
        Rect Date_rect;
        /// <summary>
        /// 图标矩形。
        /// </summary>
        Rect Icon_rect;
        /// <summary>
        /// 总数量矩形。
        /// </summary>
        Rect TotalCount_rect;
        /// <summary>
        /// 元素库选择区矩形。
        /// </summary>
        Rect LibSelector_rect;
        /// <summary>
        /// 列表序号矩形。
        /// </summary>
        Rect index_rect;
        /// <summary>
        /// 列表标识矩形。
        /// </summary>
        Rect indicator_rect;
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
        /// Unity 禁用回调。
        /// <para/>
        /// 当前实现为空，保留以支持后续清理逻辑。
        /// </summary>
        private void OnDisable()
        {

        }
        /// <summary>
        /// Unity 启用回调。
        /// <para/>
        /// 执行流程：
        /// <list type="number">
        /// <item><description>构建序列化对象；</description></item>
        /// <item><description>获取 XHud 管理器；</description></item>
        /// <item><description>加载窗口 Logo 图标；</description></item>
        /// <item><description>采集所有已部署元素库的简要信息；</description></item>
        /// <item><description>绑定序列化属性并构造只读列表。</description></item>
        /// </list>
        /// </summary>
        private void OnEnable()
        {
            BaseObject = new SerializedObject(this);

            HudManager = XHud_Dashboard.get_manager();

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
        /// <summary>
        /// 绘制窗口 GUI。
        /// <para/>
        /// 按自上而下的顺序绘制：
        /// 抬头 → 统计数量 → 元素库信息（名称 / 说明 / 各状态统计） → 元素库列表 → 按钮。
        /// <para/>
        /// 支持 Escape 键关闭窗口。
        /// </summary>
        private void OnGUI()
        {
            BaseObject.Update();

            string colorhex_value = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.themes_get_color(ThemeColorType.primary), true);
            string colorhex_title = XGUI_Utilitys.Color_To_HexString(Color.white * 0.65f, true);

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
                text: new GUIContent("元素库选择器"),
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
                   clipping: clipping_ellipsis,
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
                    clipping: clipping_ellipsis,
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
                    clipping: clipping_ellipsis,
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
                    clipping: clipping_ellipsis,
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
                    clipping: clipping_ellipsis,
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
                    clipping: clipping_ellipsis,
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
                    clipping: clipping_ellipsis,
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
                    clipping: clipping_ellipsis,
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
        #endregion

        #region 列表绘制
        /// <summary>
        /// 绘制元素回调。
        /// <para/>
        /// 绘制内容：序号（灰色，D2 格式）+ 元素库名称。
        /// </summary>
        /// <param name="rect">绘制矩形</param>
        /// <param name="index">索引</param>
        /// <param name="isActive">是否激活</param>
        /// <param name="isFocused">是否聚焦</param>
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
        /// 绘制元素库列表。
        /// <para/>
        /// 只绘制滚动视图可视区域内的元素，以优化大列表性能。
        /// 单击列表项时更新右侧元素库信息。
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
                    EditorGUI.DrawRect(itemmark_rect, XHud_Dashboard.themes_get_color(ThemeColorType.primary));
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

        #region 按钮
        /// <summary>
        /// 绘制底部按钮区（关闭 / 选择）。
        /// <para/>
        /// 点击"选择"按钮会打开选中元素库的 Inspector 并关闭窗口。
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
                text: "选择",
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
                EditorUtility.OpenPropertyEditor(HudManager.hm_ElementLibrary_GetArray()[SelectedLibraryIndex]);
                Close();
                return;
            }
            GUI.backgroundColor = Color.white;
            XGUI.SetEnabled(true);

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
        }
        #endregion
    }
    #endregion
}