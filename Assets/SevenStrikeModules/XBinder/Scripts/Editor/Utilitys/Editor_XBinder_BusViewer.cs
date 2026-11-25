/*
 * ============================================================================
 * ⚠ 版权声明（禁止删除、禁止修改、衍生作品必须保留此注释）⚠
 * ============================================================================
 * 版权声明 Copyright (C) 2025-Present Nanjing SevenStrike Media Co., Ltd.
 * 中文名称：南京塞维斯传媒有限公司
 * 英文名称：SevenStrikeMedia
 * 项目作者：徐寅智
 * 项目名称：XBinder - Unity 高级数据绑定管理插件
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
namespace SevenStrikeModules.XBinder.Editor
{
    using SevenStrikeModules.XBinder;
    using SevenStrikeModules.XGUI.Editor;
    using SevenStrikeModules.XGUI.Runtime;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    public class Editor_XBinder_BusViewer : EditorWindow
    {
        //[MenuItem("Tools/XBinder/清空总线数据（仅编辑器）")]
        //private static void ClearBus()
        //{
        //    XBinder_Bus.categories.Clear();
        //    Debug.Log("[XBinder] 总线已清空");
        //}

        #region 字段
        /// <summary>
        /// 左面板滚动位置。
        /// </summary>
        private Vector2 _leftScroll;
        /// <summary>
        /// 右面板滚动位置。
        /// </summary>
        private Vector2 _rightScroll;
        /// <summary>
        /// 左面板当前宽度。
        /// </summary>
        private float _leftWidth = 200;
        /// <summary>
        /// 是否正在拖拽分割线。
        /// </summary>
        private bool _draggingSplitter;
        /// <summary>
        /// 分割线宽度。
        /// </summary>
        private const float SPLITTER_W = 1;
        /// <summary>
        /// 左面板最小宽度。
        /// </summary>
        private const float MIN_PANEL_LEFT = 200;
        /// <summary>
        /// 右面板最小宽度。
        /// </summary>
        private const float MIN_PANEL_RIGHT = 380;
        /// <summary>
        /// 当前选中的数据源。
        /// </summary>
        private XBinder_Category _selected;
        /// <summary>
        /// 左面板搜索词。
        /// </summary>
        private string _search = "";
        /// <summary>
        /// 是否仅查看有订阅者的键。
        /// </summary>
        private bool _showOnlySubscribed;
        /// <summary>
        /// 上一次节流绘制的时间戳。
        /// </summary>
        private double _lastRepaint;
        /// <summary>
        /// 顶部工具栏高度。
        /// </summary>
        private const float TopBarHeight = 23f;
        /// <summary>
        /// 左面板宽度持久化键。
        /// </summary>
        private const string PREF_LEFT_WIDTH = "XBinder_BusViewer_LeftWidth";
        #endregion

        #region 字段：图标
        private Texture2D icon_bus_logo;
        private Texture2D icon_warning;
        private Texture2D icon_item_checked;
        private Texture2D icon_item;
        private Texture2D icon_help_r;
        private Texture2D icon_help_p;
        private Texture2D icon_topbar_update_r;
        private Texture2D icon_topbar_update_p;
        private Texture2D icon_items_snap_r;
        private Texture2D icon_items_snap_p;
        private Texture2D icon_dot;
        #endregion

        #region 列表绘制参数（左：数据源）
        /// <summary>
        /// 左面板过滤后的数据源列表。
        /// </summary>
        private List<XBinder_Category> _leftFilteredCats = new List<XBinder_Category>();
        /// <summary>
        /// 左面板过滤列表对应的高度缓存。
        /// </summary>
        private List<float> _leftFilteredHeights = new List<float>();
        /// <summary>
        /// 左面板高度缓存是否需要重建。
        /// </summary>
        private bool _leftHeightsDirty = true;
        /// <summary>
        /// 左面板过滤结果是否需要重建。
        /// </summary>
        private bool _leftFilterDirty = true;
        /// <summary>
        /// 上一次搜索词，用于检测搜索变化。
        /// </summary>
        private string _leftLastSearch = null;
        #endregion

        #region 列表绘制参数（右：绑定键）
        /// <summary>
        /// 右面板过滤后的键列表。
        /// </summary>
        private List<string> _rightFilteredKeys = new List<string>();
        /// <summary>
        /// 右面板过滤列表对应的高度缓存。
        /// </summary>
        private List<float> _rightFilteredKeyHeights = new List<float>();
        /// <summary>
        /// 右面板高度缓存是否需要重建。
        /// </summary>
        private bool _rightKeyHeightsDirty = true;
        /// <summary>
        /// 右面板过滤结果是否需要重建。
        /// </summary>
        private bool _rightKeyFilterDirty = true;
        #endregion

        #region 打开窗口
        /// <summary>
        /// 打开绑定总线查看器窗口。
        /// </summary>
        [MenuItem("Tools/XBinder/绑定总线查看器")]
        public static void Open()
        {
            Editor_XBinder_BusViewer window = EditorWindow.GetWindow<Editor_XBinder_BusViewer>(false, "XBinder 绑定总线查看器");
            window.titleContent = new GUIContent("总线查看器");
            window.minSize = new Vector2(MIN_PANEL_LEFT + MIN_PANEL_RIGHT + SPLITTER_W, 360);
            window.Show();
        }
        #endregion

        #region 生命周期
        /// <summary>
        /// 窗口启用时初始化事件监听、图标、选中状态与布局宽度。
        /// </summary>
        private void OnEnable()
        {
            #region 监听事件
            EditorApplication.update += RepaintTick;
            XBinder_Bus.on_category_created += OnCategoryChanged;
            XBinder_Bus.on_category_destroyed += OnCategoryChanged;
            AssemblyReloadEvents.afterAssemblyReload += Editor_Recomplier_Rebuild;
            #endregion

            #region 获取图标
            icon_bus_logo = XGUI.GetCustomIcon($"{XBinder_Dashboard.get_path_xbinder_gui()}gui_bus_viewer/icon_main");
            icon_help_r = XGUI.GetCustomIcon($"{XBinder_Dashboard.get_path_xbinder_gui()}gui_bus_viewer/icon_help_r");
            icon_help_p = XGUI.GetCustomIcon($"{XBinder_Dashboard.get_path_xbinder_gui()}gui_bus_viewer/icon_help_p");
            icon_topbar_update_r = XGUI.GetCustomIcon($"{XBinder_Dashboard.get_path_xbinder_gui()}gui_bus_viewer/icon_topbar_update_r");
            icon_topbar_update_p = XGUI.GetCustomIcon($"{XBinder_Dashboard.get_path_xbinder_gui()}gui_bus_viewer/icon_topbar_update_p");
            icon_items_snap_r = XGUI.GetCustomIcon($"{XBinder_Dashboard.get_path_xbinder_gui()}gui_bus_viewer/icon_items_snap_r");
            icon_items_snap_p = XGUI.GetCustomIcon($"{XBinder_Dashboard.get_path_xbinder_gui()}gui_bus_viewer/icon_items_snap_p");
            icon_item_checked = XGUI.GetCustomIcon($"{XBinder_Dashboard.get_path_xbinder_gui()}gui_bus_viewer/icon_item_checked");
            icon_item = XGUI.GetCustomIcon($"{XBinder_Dashboard.get_path_xbinder_gui()}gui_bus_viewer/icon_item");
            icon_warning = XGUI.GetBasedIcon("icon_warning_solid");
            icon_dot = XGUI.GetBasedIcon("icon_dot");
            #endregion

            #region 恢复选中
            string saved = SessionState.GetString("XBinder_Bus_Selected", "");
            if (!string.IsNullOrEmpty(saved))
                _selected = XBinder_Bus.get_category_by_name(saved);
            #endregion

            // 读取窗口分割值
            _leftWidth = EditorPrefs.GetFloat(PREF_LEFT_WIDTH, 200f);
        }
        /// <summary>
        /// 窗口禁用时取消事件监听并保存选中状态与布局宽度。
        /// </summary>
        private void OnDisable()
        {
            #region 取消监听事件
            EditorApplication.update -= RepaintTick;
            XBinder_Bus.on_category_created -= OnCategoryChanged;
            XBinder_Bus.on_category_destroyed -= OnCategoryChanged;
            AssemblyReloadEvents.afterAssemblyReload -= Editor_Recomplier_Rebuild;
            #endregion

            #region 保存选中
            if (_selected != null)
                SessionState.SetString("XBinder_Bus_Selected", _selected.display_name.Replace("xbinder_", ""));
            #endregion

            // 保存窗口分割值
            EditorPrefs.SetFloat(PREF_LEFT_WIDTH, _leftWidth);
        }
        /// <summary>
        /// 窗口主绘制入口。
        /// </summary>
        /// <remarks>
        /// 负责左右面板布局、分割线拖拽与底部工具栏。
        /// </remarks>
        private void OnGUI()
        {
            if (position.width <= 0 || position.height <= 0) return; // 尺寸未就绪才可跳过

            float totalW = position.width;
            float minLeft = MIN_PANEL_LEFT;
            float minRight = MIN_PANEL_RIGHT;
            float maxLeft = totalW - minRight - SPLITTER_W;

            float leftW;
            float rightW;

            // 窗口太窄时，先保右面板，再压缩左面板
            if (maxLeft < minLeft)
            {
                float ratio = minLeft / (minLeft + minRight);
                leftW = totalW * ratio;
                rightW = totalW - leftW - SPLITTER_W;
            }
            else
            {
                leftW = Mathf.Clamp(_leftWidth, minLeft, maxLeft);
                rightW = totalW - leftW - SPLITTER_W;
            }

            DrawTopToolbar();

            // 搜索词或数据源变化时重建左面板过滤列表
            if (_leftLastSearch != _search || _leftFilterDirty)
            {
                _leftLastSearch = _search;
                _leftFilterDirty = false;
                RebuildFiltered_Left_Cats();
            }

            // 左面板
            GUILayout.BeginArea(new Rect(0, (TopBarHeight + 10), leftW, position.height));
            DrawLeftPanel();
            GUILayout.EndArea();

            // 拖拽条（用 GUI.DrawRect，不走 GUILayout）
            DrawSplitter(new Rect(leftW, (TopBarHeight), SPLITTER_W, position.height - (TopBarHeight)));

            // 右面板
            GUILayout.BeginArea(new Rect(leftW + SPLITTER_W, (TopBarHeight + 10), rightW, position.height));
            DrawRightPanel();
            GUILayout.EndArea();

            DrawBottomToolbar(new Rect(0, position.height - TopBarHeight, totalW, TopBarHeight));
        }
        #endregion

        #region 当数据源改变时
        /// <summary>
        /// 数据源创建或销毁时的回调。
        /// </summary>
        /// <remarks>
        /// 标记左右面板过滤结果需要重建。
        /// </remarks>
        /// <param name="_">数据源名称（未使用）。</param>
        private void OnCategoryChanged(string _)
        {
            _leftFilterDirty = true;
            _rightKeyFilterDirty = true;
            Repaint();
        }
        #endregion

        #region 节流绘制
        /// <summary>
        /// 以 0.1 秒节流触发重绘。
        /// </summary>
        /// <remarks>
        /// 避免编辑时持续刷新。
        /// </remarks>
        private void RepaintTick()
        {
            if (EditorApplication.timeSinceStartup - _lastRepaint > 0.1)
            {
                _lastRepaint = EditorApplication.timeSinceStartup;
                Repaint();
            }
        }
        #endregion

        #region 绘制：顶部工具栏
        /// <summary>
        /// 绘制窗口顶部工具栏。
        /// </summary>
        /// <remarks>
        /// 包含 Logo、数据源数量、更新按钮、开发者标识与帮助按钮。
        /// </remarks>
        private void DrawTopToolbar()
        {
            XGUI.layout_group_start(
               type: XGUIContainerType.Horizontal,
               bg_fill: XGUIGroup.透明,
               bg_color: XGUIColor.亮白,
               bg_color_gui: Color.white,
               absolute_margin: true,
               absolute_padding: true,
               bg_height: TopBarHeight,
               margin: new RectOffset(0, 0, 0, 0),
               padding: new RectOffset(10, 10, 0, 0));

            #region 图标
            XGUI.layout_icon(
                icon: icon_bus_logo,
                icon_color: Color.white,
                width: 14,
                height: 14,
                icon_border: new RectOffset(0, 0, 0, 0),
                icon_offset: new Vector2(0, 5),
                icon_margin: new RectOffset(5, 25, 0, 0),
                icon_padding: new RectOffset(0, 0, 0, 0),
                icon_alignment: XGUIIconAlignment.默认);
            #endregion

            #region 已创建的数据源
            string hint = $"已创建数据源   [ {XBinder_Bus.categories.Count} ]";
            XGUI.layout_label(
                text: hint,
                size: XGUIFontSize.S,
                text_color: XBinder_Dashboard.theme_primary,
                margin: new RectOffset(0, 10, 0, 0),
                offset: new Vector2(0, 2),
                clipping: XGUI.TryEllipsisClipping(),
                font_style: FontStyle.Bold,
                anchor: TextAnchor.MiddleLeft,
                width: 150,
                font: XGUI.GetFont("xg-light"));
            #endregion

            #region 按钮 - 更新总线信息
            if (XGUI.toolbar_button(
                tooltip: "rewind",
                icon_release: icon_topbar_update_r,
                icon_press: icon_topbar_update_p,
                press_icon_color: Color.white,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                layout_width: icon_topbar_update_r.width,
                layout_height: icon_topbar_update_r.height,
                content_offset: new Vector2(0, 0),
                sepline_color: Color.black * 0.55f,
                edge: XGUIEdge.全部))
            {
                XBinder_Bus.get_category_names();
            }
            #endregion

            #region 自适应间距
            XGUI.layout_flexspace();
            #endregion

            #region 开发者标识
            XGUI.layout_label(
                bg_fill: XGUIGroup.实体,
                bg_color: XGUIColor.亮白,
                bg_color_gui: Color.white * 0.85f,
                text: "SevenStrike Media",
                size: XGUIFontSize.S,
                text_color: Color.black,
                padding: new RectOffset(15, 15, 0, 0),
                margin: new RectOffset(0, 0, 4, 0),
                offset: new Vector2(0, -2.5f),
                clipping: TextClipping.Clip,
                font_style: FontStyle.Normal,
                anchor: TextAnchor.MiddleRight,
                font: XGUI.GetFont("xg-medium"));
            #endregion

            #region 按钮 - 帮助
            if (XGUI.layout_button(
                tooltip: "帮助",
                tex_release: icon_help_r,
                tex_press: icon_help_p,
                tex_gui_color: Color.white,
                margin: new RectOffset(15, 10, 0, 0),
                border: new RectOffset(0, 0, 0, 0),
                width: TopBarHeight,
                height: TopBarHeight))
            {

            }
            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);

            XGUI.layout_seperator(thickness: 1, color: Color.black * 0.35f, margin: new RectOffset(0, 0, 0, 0));
        }
        #endregion

        #region 绘制：底部工具栏
        /// <summary>
        /// 绘制窗口底部工具栏。
        /// </summary>
        /// <param name="area">底部工具栏在窗口坐标系中的矩形。</param>
        private void DrawBottomToolbar(Rect area)
        {
            EditorGUI.DrawRect(area, XGUI_Utilitys.HexString_To_Color("1e1e1e"));
            XGUI.gui_box(new Rect(area.x, area.y, area.width, 1), Color.black * 0.4f);

            GUILayout.BeginArea(area);

            XGUI.layout_group_start(
                type: XGUIContainerType.Horizontal,
                bg_fill: XGUIGroup.透明,
                bg_color: XGUIColor.亮白,
                bg_color_gui: Color.white,
                absolute_margin: true,
                absolute_padding: true,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(10, 10, 0, 0));

            XGUI.layout_label(
                text: $"统计源数量：{XBinder_Bus.categories.Count} 个",
                size: XGUIFontSize.M,
                text_color: Color.white * 0.85f,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                offset: new Vector2(0, 2),
                clipping: TextClipping.Clip,
                font_style: FontStyle.Normal,
                anchor: TextAnchor.MiddleLeft,
                font: XGUI.GetFont("xg-regular"));

            #region 分割线
            XGUI.layout_seperator(
                thickness: 1,
                dir: XGUISeplineDir.垂直,
                color: Color.black * 0.4f,
                padding: new RectOffset(0, 0, 0, 0),
                margin: new RectOffset(10, 10, 0, 0));
            #endregion

            XGUI.layout_flexspace();

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);

            GUILayout.EndArea();
        }
        #endregion

        #region 绘制：左面板（数据源列表）
        /// <summary>
        /// 根据当前搜索词重建左面板过滤列表。
        /// </summary>
        /// <remarks>
        /// 过滤后同步重建高度缓存，并重置滚动位置，避免停留在旧的最大值处。
        /// </remarks>
        private void RebuildFiltered_Left_Cats()
        {
            _leftFilteredCats.Clear();
            _leftFilteredHeights.Clear();

            bool searching = !string.IsNullOrEmpty(_search);
            var src = XBinder_Bus.categories;

            for (int i = 0; i < src.Count; i++)
            {
                var cat = src[i];
                if (cat == null) continue;

                if (searching &&
                    cat.display_name.IndexOf(_search, System.StringComparison.OrdinalIgnoreCase) < 0)
                    continue;

                _leftFilteredCats.Add(cat);
                _leftFilteredHeights.Add(40f); // 与 fixheight 保持一致
            }

            // 过滤后列表变了，滚动位置归零，避免停在旧的大值上
            _leftScroll.y = 0f;

            // 列表长度/内容变了，告诉滚动视图高度缓存需要刷新
            _leftHeightsDirty = true;
        }
        /// <summary>
        /// 绘制左面板。
        /// </summary>
        /// <remarks>
        /// 包含搜索框与数据源虚拟滚动列表。
        /// </remarks>
        private void DrawLeftPanel()
        {
            XGUI.layout_group_start(
                type: XGUIContainerType.Horizontal,
                bg_fill: XGUIGroup.透明,
                bg_color: XGUIColor.亮白,
                bg_height: 30,
                absolute_margin: true,
                absolute_padding: true,
                padding: new RectOffset(5, 0, 0, 0),
                margin: new RectOffset(0, 0, 0, 0));

            #region 搜索框
            _search = XGUI.layout_inputfield(
                title: "搜索源",
                prop: _search,
                text_wrap: false,
                field_fontsize: XGUIFontSize.M,
                field_text_offset: Vector2.zero,
                field_height: 20,
                field_padding: new RectOffset(0, 0, 0, 0),
                field_margin: new RectOffset(5, 10, 5, 0),
                field_text_color: Color.white,
                title_width: 55,
                title_margin: new RectOffset(0, 10, 0, 0),
                title_padding: new RectOffset(0, 0, 0, 2),
                field_text_font: XGUI.GetFont("xg-medium"),
                field_text_style: FontStyle.Normal,
                field_text_anchor: TextAnchor.MiddleLeft);
            #endregion

            XGUI.layout_group_end(XGUIContainerType.Horizontal);

            XGUI.layout_seperator(thickness: 1, color: Color.black * 0.35f, margin: new RectOffset(10, 10, 15, 10));

            #region 数据源列表
            if (_leftFilteredCats.Count != 0)
            {
                Rect list_root = XGUI.GetControlRect(false, position.height - TopBarHeight - 96);
                bool isfull = (_leftFilteredCats.Count * 40) > list_root.height;
                _leftScroll = XGUI.gui_scrollview<XBinder_Category>(
                    rect: new Rect(list_root.x + 5, list_root.y, list_root.width - (isfull ? 8 : -4), list_root.height),
                    scroll: _leftScroll,
                    list: _leftFilteredCats,
                    cachedItemHeights: ref _leftFilteredHeights,
                    heightsDirty: ref _leftHeightsDirty,
                    fixheight: 40,
                    onscroller: (rect, cat, index) =>
                    {
                        bool isSel = cat == _selected;
                        string shortName = cat.display_name.Replace("xbinder_", "");

                        Rect rect_content = rect;

                        #region 列表项

                        #region 背景
                        rect_content.Set(rect.x, rect.y, rect.width, rect.height - 1);
                        XGUI.gui_box(
                            rect: rect_content,
                            bg: XGUI.GetGroupTexture(XGUIGroup.实体, XGUIColor.亮白),
                            bg_color_gui: Color.black * 0.33f,
                            offset: Vector2.zero,
                            border: new RectOffset(10, 10, 10, 10),
                            margin: new RectOffset(0, 0, 0, 0));
                        #endregion

                        #region 图标
                        rect_content.Set(rect.x + 15, rect.y + 15, icon_dot.width, icon_dot.height);
                        XGUI.gui_box(
                            rect: rect_content,
                            bg: icon_dot,
                            bg_color_gui: Color.white * 0.8f,
                            offset: Vector2.zero,
                            margin: new RectOffset(0, 0, 0, 0));
                        #endregion

                        #region 统计信息
                        // 类型去重数 + 订阅者总数
                        int keyCount = cat.get_key_count();
                        int subTotal = 0;
                        foreach (var k in cat.get_all_keys())
                        {
                            subTotal += cat.get_subscriber_count(k);
                        }

                        rect_content.Set(rect.x + rect.width - (rect.width / 2.3f), rect.y, rect.width / 2.3f, rect.height - 1);
                        XGUI.gui_label(
                            rect: rect_content,
                            text: new GUIContent($"[ {keyCount} 键 / {subTotal} 订阅 ]"),
                            text_color: Color.white * 0.7f,
                            size: XGUIFontSize.S,
                            clipping: XGUI.TryEllipsisClipping(),
                            anchor: TextAnchor.MiddleRight,
                            offset: new Vector2(0, 0),
                            padding: new RectOffset(0, 10, 0, 0),
                            font_style: FontStyle.Normal,
                            font: XGUI.GetFont("xg-regular"));
                        #endregion

                        #region 数据源选择按钮
                        rect_content.Set(rect.x + 25, rect.y, rect.width - rect.x, rect.height - 1);
                        if (XGUI.gui_button(
                            rect: rect_content,
                            text: shortName,
                            tooltip: "选择并查看数据源信息",
                            btn_fill: XGUIGroup.透明,
                            btn_text_color: isSel ? XBinder_Dashboard.theme_primary : Color.white,
                            press_fill: XGUIGroup.透明,
                            press_color: XGUIColor.深空灰,
                            press_text_color: isSel ? Color.black : Color.black,
                            font_size: XGUIFontSize.S,
                            anchor: TextAnchor.MiddleLeft,
                            margin: new RectOffset(0, 0, 0, 0),
                            padding: new RectOffset(0, 0, 0, 0)))
                        {
                            _selected = cat;
                            _rightKeyFilterDirty = true;
                        }
                        #endregion

                        #endregion
                    });
            }
            else
            {
                XGUI.layout_group_start(
                      type: XGUIContainerType.Vertical,
                      bg_fill: XGUIGroup.透明,
                      bg_color: XGUIColor.亮白,
                      absolute_margin: true,
                      absolute_padding: true,
                      padding: new RectOffset(0, 0, 0, 0),
                      margin: new RectOffset(0, 0, 0, 0));

                XGUI.layout_flexspace();

                XGUI.layout_icon(
                    icon: icon_warning,
                    icon_color: Color.gray * 0.8f,
                    icon_size: icon_warning.width * 0.7f,
                    icon_offset: new Vector2(0, -50),
                    icon_margin: new RectOffset(0, 10, 6, 0),
                    icon_padding: new RectOffset(0, 0, 0, 0),
                    icon_alignment: XGUIIconAlignment.中心);

                XGUI.layout_label(
                    text: $"总线上暂无数据源。\n运行  XBinder_Source  后自动注册",
                    size: XGUIFontSize.M,
                    text_color: Color.white * 0.85f,
                    margin: new RectOffset(0, 0, 0, 0),
                    padding: new RectOffset(0, 0, 0, 0),
                    offset: new Vector2(0, -30),
                    wrap: true,
                    clipping: XGUI.TryEllipsisClipping(),
                    font_style: FontStyle.Normal,
                    anchor: TextAnchor.MiddleCenter,
                    font: XGUI.GetFont("xg-regular"));

                XGUI.layout_flexspace();

                XGUI.layout_group_end(XGUIContainerType.Vertical);
            }
            #endregion
        }
        #endregion

        #region 绘制：右面板（选中数据源详情）
        /// <summary>
        /// 根据“仅看有订阅”开关重建右面板过滤键列表。
        /// </summary>
        /// <remarks>
        /// 当 <see cref="_selected"/> 为空时仅清空列表并重置脏标记。
        /// </remarks>
        private void RebuildFiltered_Right_Keys()
        {
            _rightFilteredKeys.Clear();
            _rightFilteredKeyHeights.Clear();

            if (_selected == null) return;

            foreach (var key in _selected.get_all_keys())
            {
                int subCount = _selected.get_subscriber_count(key);
                if (_showOnlySubscribed && subCount == 0) continue;
                _rightFilteredKeys.Add(key);
                _rightFilteredKeyHeights.Add(25f);
            }
            _rightScroll.y = 0f;
            _rightKeyHeightsDirty = true;
        }
        /// <summary>
        /// 绘制右面板。
        /// </summary>
        /// <remarks>
        /// 包含未选中提示、头部信息、工具栏、表头与绑定键虚拟滚动列表。
        /// </remarks>
        private void DrawRightPanel()
        {
            #region 如果未选择数据源的提示信息
            if (_selected == null)
            {
                XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIGroup.透明,
                    bg_color: XGUIColor.亮白,
                    absolute_margin: true,
                    absolute_padding: true,
                    padding: new RectOffset(0, 0, 0, 0),
                    margin: new RectOffset(0, 0, 0, 0));

                XGUI.layout_flexspace();

                XGUI.layout_icon(
                    icon: icon_warning,
                    icon_color: Color.gray * 0.8f,
                    icon_size: icon_warning.width * 0.7f,
                    icon_offset: new Vector2(0, -50),
                    icon_margin: new RectOffset(0, 10, 6, 0),
                    icon_padding: new RectOffset(0, 0, 0, 0),
                    icon_alignment: XGUIIconAlignment.中心);

                XGUI.layout_label(
                    text: $"暂未选择目标数据源，请从左侧数据源列表选择",
                    size: XGUIFontSize.M,
                    text_color: Color.white * 0.85f,
                    margin: new RectOffset(0, 0, 0, 0),
                    padding: new RectOffset(0, 0, 0, 0),
                    offset: new Vector2(0, -30),
                    wrap: true,
                    clipping: XGUI.TryEllipsisClipping(),
                    font_style: FontStyle.Normal,
                    anchor: TextAnchor.MiddleCenter,
                    font: XGUI.GetFont("xg-regular"));

                XGUI.layout_flexspace();

                XGUI.layout_group_end(XGUIContainerType.Vertical);
                return;
            }
            #endregion

            #region 头部信息
            XGUI.layout_group_start(
                type: XGUIContainerType.Horizontal,
                bg_fill: XGUIGroup.透明,
                bg_color: XGUIColor.亮白,
                absolute_margin: true,
                absolute_padding: true,
                bg_height: 80,
                padding: new RectOffset(10, 10, 0, 0),
                margin: new RectOffset(0, 0, 0, 0));

            XGUI.layout_icon(
                icon: icon_item,
                icon_color: Color.white * 0.8f,
                icon_size: icon_item.width,
                icon_offset: new Vector2(0, 0),
                icon_margin: new RectOffset(0, 10, 6, 0),
                icon_padding: new RectOffset(0, 0, 0, 0),
                icon_alignment: XGUIIconAlignment.默认);

            string hex_primary = XGUI_Utilitys.Color_To_HexString(XBinder_Dashboard.theme_primary, true);
            XGUI.layout_label(
                text: $"Binder  [ <color={hex_primary}><b>{_selected.display_name}</b></color> ]",
                size: XGUIFontSize.L,
                text_color: Color.white,
                margin: new RectOffset(15, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                offset: new Vector2(0, 0),
                clipping: XGUI.TryEllipsisClipping(),
                font_style: FontStyle.Normal,
                anchor: TextAnchor.MiddleLeft,
                font: XGUI.GetFont("xg-regular"));

            Rect rect_sub = XGUI.GetLastRect();
            Rect rect_sub_region = new Rect(rect_sub.x, rect_sub.y + 25, (position.width - _leftWidth) - rect_sub.x - 10, XGUI.GetSingleLineHeight());
            XGUI.gui_label(
                rect: rect_sub_region,
                text: new GUIContent($"类型去重数 : {_selected.get_all_keys().Count}    通道条目 : {_selected.get_key_count()}"),
                text_color: Color.white * 0.85f,
                size: XGUIFontSize.M,
                clipping: XGUI.TryEllipsisClipping(),
                anchor: TextAnchor.MiddleLeft,
                offset: new Vector2(0, 0),
                padding: new RectOffset(0, 10, 0, 0),
                font_style: FontStyle.Normal,
                font: XGUI.GetFont("xg-regular"));

            XGUI.layout_group_end(XGUIContainerType.Horizontal);
            #endregion

            #region 工具栏
            XGUI.layout_seperator(thickness: 1, color: Color.black * 0.35f, margin: new RectOffset(0, 0, 0, 0));
            XGUI.layout_group_start(
              type: XGUIContainerType.Horizontal,
              bg_fill: XGUIGroup.透明,
              bg_color: XGUIColor.亮白,
              bg_color_gui: Color.white,
              absolute_margin: true,
              absolute_padding: true,
              bg_height: TopBarHeight,
              margin: new RectOffset(0, 0, 0, 0),
              padding: new RectOffset(0, 0, 0, 0));

            #region 开关 - 仅查看有订阅的绑定项
            _showOnlySubscribed = XGUI.toolbar_toggle(
               tooltip: "仅查看有订阅的绑定项",
               icon: icon_item_checked,
               prop: _showOnlySubscribed,
               anchor: TextAnchor.MiddleCenter,
               margin: new RectOffset(0, 0, 0, 0),
               padding: new RectOffset(0, 0, 0, 0),
               content_offset: new Vector2(0, 0),
               sepline_color: Color.black * 0.55f,
               layout_width: icon_item_checked.width,
               layout_min_width: icon_item_checked.width,
               fixed_height: TopBarHeight,
               edge: XGUIEdge.右,
               on_clicked_toggle: (val) =>
               {
                   _rightKeyFilterDirty = true;
               });
            #endregion

            #region 自适应间距
            XGUI.layout_flexspace();
            #endregion

            #region 按钮 - 复制快照
            if (XGUI.toolbar_button(
                tooltip: "复制绑定信息快照到系统剪贴板",
                icon_release: icon_items_snap_r,
                icon_press: icon_items_snap_p,
                press_icon_color: Color.white,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                layout_width: icon_topbar_update_r.width,
                layout_height: icon_topbar_update_r.height,
                content_offset: new Vector2(0, 0),
                sepline_color: Color.black * 0.55f,
                edge: XGUIEdge.左))
            {
                CopySnapshot();
            }
            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            XGUI.layout_seperator(thickness: 1, color: Color.black * 0.35f, margin: new RectOffset(0, 0, 0, 0));
            #endregion

            // 过滤键列表变化时重建
            if (_rightKeyFilterDirty)
            {
                _rightKeyFilterDirty = false;
                RebuildFiltered_Right_Keys();
            }

            if (_rightFilteredKeys.Count != 0)
            {
                Rect list_toolbar = XGUI.GetLastRect();
                Rect list_root = XGUI.GetControlRect(false, position.height - TopBarHeight - 147);

                float list_titles_Y_added = 5;
                Rect list_titles = list_toolbar;

                bool isfull = (_rightFilteredKeys.Count * 25) > list_root.height;

                #region 抬头
                #region 背景
                list_titles.Set(list_toolbar.x + 8, list_toolbar.y + list_titles_Y_added + 2, list_toolbar.width - (isfull ? 30 : 17), 25);
                XGUI.gui_box(
                    list_titles,
                    bg: XGUI.GetGroupTexture(XGUIGroup.实体, XGUIColor.亮白),
                    bg_color_gui: Color.black * 0.33f,
                    offset: Vector2.zero,
                    border: new RectOffset(10, 10, 10, 10),
                    margin: new RectOffset(0, 0, 0, 0));
                #endregion

                #region 键名
                list_titles.Set(list_toolbar.x + 22, list_toolbar.y + list_titles_Y_added, 50, 25);
                XGUI.gui_label(
                    list_titles,
                    new GUIContent("键名"),
                    size: XGUIFontSize.M,
                    anchor: TextAnchor.MiddleLeft,
                    clipping: XGUI.TryEllipsisClipping());
                #endregion

                #region 类型
                list_titles.Set(list_toolbar.x + 190, list_toolbar.y + list_titles_Y_added, 50, 25);
                XGUI.gui_label(
                    list_titles,
                    new GUIContent("类型"),
                    size: XGUIFontSize.M,
                    anchor: TextAnchor.MiddleLeft,
                    clipping: XGUI.TryEllipsisClipping());
                #endregion

                #region 当前值
                list_titles.Set(list_toolbar.x + 272, list_toolbar.y + list_titles_Y_added, 65, 25);
                XGUI.gui_label(
                    list_titles,
                    new GUIContent("当前值"),
                    size: XGUIFontSize.M,
                    anchor: TextAnchor.MiddleLeft,
                    clipping: XGUI.TryEllipsisClipping());
                #endregion

                #region 订阅数
                list_titles.Set(list_toolbar.x + list_toolbar.width - (isfull ? 104 : 89), list_toolbar.y + list_titles_Y_added, 65, 25);
                XGUI.gui_label(
                    list_titles,
                    new GUIContent("订阅数"),
                    size: XGUIFontSize.M,
                    anchor: TextAnchor.MiddleRight,
                    clipping: XGUI.TryEllipsisClipping());
                #endregion
                #endregion

                _rightScroll = XGUI.gui_scrollview<string>(
                    rect: new Rect(list_root.x + 5, list_root.y + 30, list_root.width - (isfull ? 8 : -4), list_root.height - 30),
                    scroll: _rightScroll,
                    list: _rightFilteredKeys,
                    cachedItemHeights: ref _rightFilteredKeyHeights,
                    heightsDirty: ref _rightKeyHeightsDirty,
                    fixheight: 25,
                    onscroller: (rect, key, index) =>
                    {
                        var vt = _selected.get_key_type(key);
                        int subCount = _selected.get_subscriber_count(key);

                        Rect rect_content = rect;

                        #region 背景
                        rect_content.Set(rect.x, rect.y, rect.width, rect.height - 1);
                        XGUI.gui_box(
                            rect_content,
                            bg: XGUI.GetGroupTexture(XGUIGroup.实体, XGUIColor.亮白),
                            bg_color_gui: Color.black * 0.33f,
                            offset: Vector2.zero,
                            border: new RectOffset(10, 10, 10, 10),
                            margin: new RectOffset(0, 0, 0, 0));
                        #endregion

                        #region 键名
                        rect_content.Set(rect.x + 15, rect.y, 140, rect.height - 1);
                        XGUI.gui_label(
                            rect_content,
                            new GUIContent(key),
                            text_color: Color.white,
                            size: XGUIFontSize.M,
                            anchor: TextAnchor.MiddleLeft,
                            clipping: XGUI.TryEllipsisClipping());
                        #endregion

                        #region 类型
                        Color bgc = Color.white;

                        switch (vt)
                        {
                            case XBinder_ValueType.None:
                                bgc = Color.gray;
                                break;
                            case XBinder_ValueType.Float:
                                bgc = XGUI_Utilitys.HexString_To_Color("#215266");
                                break;
                            case XBinder_ValueType.Int:
                                bgc = XGUI_Utilitys.HexString_To_Color("#29612C");
                                break;
                            case XBinder_ValueType.Bool:
                                bgc = XGUI_Utilitys.HexString_To_Color("#8B4F1A");
                                break;
                            case XBinder_ValueType.String:
                                bgc = XGUI_Utilitys.HexString_To_Color("#502F6C");
                                break;
                            case XBinder_ValueType.Color:
                                bgc = XGUI_Utilitys.HexString_To_Color("#883258");
                                break;
                            case XBinder_ValueType.Vector2:
                                bgc = XGUI_Utilitys.HexString_To_Color("#1D5854");
                                break;
                            case XBinder_ValueType.Vector3:
                                bgc = XGUI_Utilitys.HexString_To_Color("#1D5854");
                                break;
                            case XBinder_ValueType.Vector4:
                                bgc = XGUI_Utilitys.HexString_To_Color("#1D5854");
                                break;
                            case XBinder_ValueType.Quaternion:
                                bgc = XGUI_Utilitys.HexString_To_Color("#586621");
                                break;
                            case XBinder_ValueType.Rect:
                                bgc = XGUI_Utilitys.HexString_To_Color("#434343");
                                break;
                            case XBinder_ValueType.RectOffset:
                                bgc = XGUI_Utilitys.HexString_To_Color("#0f4a78");
                                break;
                            case XBinder_ValueType.Object:
                                bgc = XGUI_Utilitys.HexString_To_Color("#6F5840");
                                break;
                        }

                        float width_type = (vt == XBinder_ValueType.Quaternion ? 70 : (vt == XBinder_ValueType.RectOffset ? 70 : 50));
                        rect_content.Set(rect.x + 170, rect.y + 3, width_type, rect.height - 7);
                        XGUI.gui_label(
                            rect: rect_content,
                            text: new GUIContent(vt.ToString()),
                            padding: new RectOffset(5, 5, 0, 0),
                            bg_fill: XGUIGroup.实体,
                            bg_color: XGUIColor.亮白,
                            bg_color_gui: bgc,
                            offset: new Vector2(0, -2),
                            text_color: Color.white * 0.95f,
                            size: XGUIFontSize.M,
                            anchor: TextAnchor.MiddleCenter,
                            clipping: XGUI.TryEllipsisClipping());
                        #endregion

                        #region 当前值
                        rect_content.Set(rect.x + 265, rect.y, rect.width - 265 - 60, rect.height - 1);
                        DrawValueCellRect(rect_content, _selected.get_value_as_object(key), vt);
                        #endregion

                        #region 订阅数
                        rect_content.Set(rect.x + rect.width - 85, rect.y, 55, rect.height - 1);
                        XGUI.gui_label(
                            rect_content,
                            new GUIContent(subCount.ToString()),
                            text_color: Color.white * 0.85f,
                            size: XGUIFontSize.S,
                            anchor: TextAnchor.MiddleRight,
                            clipping: XGUI.TryEllipsisClipping());
                        #endregion

                        #region 订阅状态
                        rect_content.Set(rect.x + rect.width - 20, rect.y + 10, icon_dot.width * 0.7f, icon_dot.height * 0.7f);
                        XGUI.gui_icon(
                            rect: rect_content,
                            icon: icon_dot,
                            border: new RectOffset(0, 0, 0, 0),
                            color: subCount <= 0 ? Color.red * 0.8f : XBinder_Dashboard.theme_primary * 0.8f);
                        #endregion
                    });
            }
            else
            {
                EditorGUILayout.HelpBox(_showOnlySubscribed ? "没有匹配的订阅项。" : "该数据源暂无任何键。", MessageType.None);
            }
        }
        #endregion

        #region 绘制：分割线
        /// <summary>
        /// 绘制左右面板之间的分割线，并处理拖拽调整左面板宽度。
        /// </summary>
        /// <param name="r">分割线所在矩形。</param>
        private void DrawSplitter(Rect r)
        {
            r.Set(r.x - 5, r.y, r.width + 10, r.height);
            EditorGUIUtility.AddCursorRect(r, MouseCursor.ResizeHorizontal);

            if (Event.current.type == EventType.MouseDown && r.Contains(Event.current.mousePosition))
                _draggingSplitter = true;
            if (_draggingSplitter && Event.current.type == EventType.MouseDrag)
            {
                _leftWidth += Event.current.delta.x;
                _leftWidth = Mathf.Clamp(_leftWidth, MIN_PANEL_LEFT, position.width - MIN_PANEL_RIGHT - SPLITTER_W);
                Repaint();
            }
            if (Event.current.type == EventType.MouseUp && _draggingSplitter)
            {
                _draggingSplitter = false;
                EditorPrefs.SetFloat(PREF_LEFT_WIDTH, _leftWidth);
            }
            if (Event.current.type == EventType.MouseUp)
                _draggingSplitter = false;

            EditorGUI.DrawRect(new Rect(r.x + 5, r.y, r.width - 10, r.height), Color.black * 0.45f);
        }
        #endregion

        #region 绘制：值单元格
        /// <summary>
        /// 在指定矩形内绘制绑定项的当前值。
        /// </summary>
        /// <remarks>
        /// Object 类型给可 ping 的 ObjectField，Color 给色块，其余给文本。
        /// </remarks>
        /// <param name="r">绘制矩形。</param>
        /// <param name="value">当前值对象。</param>
        /// <param name="vt">值类型。</param>
        private void DrawValueCellRect(Rect r, object value, XBinder_ValueType vt)
        {
            if (value == null)
            {
                XGUI.gui_label(
                    r,
                    new GUIContent("Null"),
                    text_color: Color.white * 0.85f,
                    size: XGUIFontSize.S,
                    anchor: TextAnchor.MiddleLeft,
                    clipping: XGUI.TryEllipsisClipping());
                return;
            }

            if (vt == XBinder_ValueType.Object && value is Object uo)
            {
                EditorGUI.ObjectField(r, uo, typeof(Object), true);
                return;
            }

            if (vt == XBinder_ValueType.Color && value is Color c)
            {
                Rect colorRect = new Rect(r.x, r.y + (r.height - 16) / 2, 40, 16);
                EditorGUI.DrawRect(colorRect, c);
                Rect labelRect = new Rect(r.x + 46, r.y, r.width - 46, r.height);
                GUI.Label(labelRect, $"#{ColorUtility.ToHtmlStringRGBA(c)}", EditorStyles.miniLabel);
                return;
            }

            XGUI.gui_label(
                r,
                new GUIContent(value.ToString()),
                text_color: Color.white * 0.85f,
                size: XGUIFontSize.S,
                anchor: TextAnchor.MiddleLeft,
                clipping: XGUI.TryEllipsisClipping());
        }
        #endregion

        #region 复制快照
        /// <summary>
        /// 将当前选中数据源的绑定信息快照复制到系统剪贴板。
        /// </summary>
        private void CopySnapshot()
        {
            if (_selected == null) return;

            var sb = new System.Text.StringBuilder();

            sb.AppendLine($"# {_selected.display_name}");

            foreach (var key in _selected.get_all_keys())
            {
                sb.AppendLine($"{key}\t{_selected.get_key_type(key)}\t{_selected.get_value_as_object(key)}\t{_selected.get_subscriber_count(key)}");
            }

            EditorGUIUtility.systemCopyBuffer = sb.ToString();

            XGUI_Utilitys.Console("XBinder 绑定总线查看器消息", $"快照已复制到剪贴板！", XGUIMsgState.通知);
        }
        #endregion

        public void Editor_Recomplier_Rebuild()
        {
            if (!Application.isPlaying) return;

            XBinder_Source[] sources = FindObjectsByType<XBinder_Source>(FindObjectsSortMode.None);
            for (int i = 0; i < sources.Length; i++)
                sources[i].Editor_Recomplier_Rebuild();

            // 重载后恢复选中并标记重建
            string saved = SessionState.GetString("XBinder_Bus_Selected", "");


            if (!string.IsNullOrEmpty(saved))
                _selected = XBinder_Bus.get_category_by_name(saved);

            if (_selected == null && XBinder_Bus.categories.Count > 0)
                _selected = XBinder_Bus.categories[0];

            _leftFilterDirty = true;
            _rightKeyFilterDirty = true;

            Repaint();
        }
    }
}