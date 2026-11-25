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
    using SevenStrikeModules.XGUI.Editor;
    using SevenStrikeModules.XGUI.Runtime;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(XBinder_Adapter_Multi), true)]
    public class Editor_XBinder_Adapter_Multi : Editor
    {
        #region 字段 - 组件 / 列表
        public XBinder_Adapter_Multi base_script;
        /// <summary>
        /// 绑定项列表（可重排列表）。
        /// <para/>
        /// 用于驱动 ReorderableList 的绘制与交互。
        /// </summary>
        public ReorderableList bind_item_list;
        #endregion

        #region 字段 - 序列化属性
        private SerializedProperty sp_entries;
        private SerializedProperty sp_global_category;
        private SerializedProperty sp_identifier;
        #endregion

        #region 字段 - 图标
        /// <summary>
        /// 面板主标题图标
        /// </summary>
        private Texture2D icon_main;
        private Texture2D icon_dot;
        #endregion

        #region 字段 - 批量化操作
        /// <summary>
        /// 当前 Inspector 选中的所有目标组件缓存。
        /// </summary>
        private XBinder_Adapter_Multi[] selected_objects;
        #endregion

        #region 工具 - 批量化操作
        /// <summary>
        /// 缓存当前 Inspector 选中的所有目标组件到 <see cref="selected_objects"/>。
        /// <para/>
        /// 多选时逐个转换，单选时只填充一个元素。
        /// </summary>
        private void Targets_Get()
        {
            if (targets.Length > 1)
            {
                selected_objects = new XBinder_Adapter_Multi[targets.Length];
                for (int i = 0; i < selected_objects.Length; i++)
                {
                    var t = targets[i];
                    selected_objects[i] = (XBinder_Adapter_Multi)t;
                }
            }
            else
            {
                selected_objects = new XBinder_Adapter_Multi[targets.Length];
                selected_objects[0] = (XBinder_Adapter_Multi)target;
            }
        }
        /// <summary>
        /// 判断当前是否处于多选状态。
        /// </summary>
        /// <returns>true 表示选中了多个组件</returns>
        private bool Targets_Selected()
        {
            if (selected_objects == null)
                return false;
            if (selected_objects.Length > 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 字段 - 列表绘制
        /// <summary>
        /// 绘制元素矩形。
        /// </summary>
        private Rect bind_item_list_drawelement_rect;
        /// <summary>
        /// 滚动视图矩形。
        /// </summary>
        private Rect bind_item_list_scrollview_rect;
        /// <summary>
        /// 列表项矩形。
        /// </summary>
        private Rect bind_item_list_rect;
        /// <summary>
        /// 选中背景色。
        /// </summary>
        private Color bind_item_list_selected_bg = new Color(0, 0, 0, 0.2f);
        private float bind_item_list_item_height;
        private int bind_item_list_visible_count;
        private Vector2 bind_item_list_scroller;
        private int bind_item_list_selected_index;
        #endregion

        #region 生命周期
        public virtual void OnEnable()
        {
            base_script = (XBinder_Adapter_Multi)target;

            #region 获取序列化字段
            sp_identifier = serializedObject.FindProperty(nameof(XBinder_Adapter_Multi.identifier));
            sp_entries = serializedObject.FindProperty(nameof(XBinder_Adapter_Multi.entries));
            sp_global_category = serializedObject.FindProperty(nameof(XBinder_Adapter_Multi.global_category));
            #endregion

            #region 图标获取
            icon_main = XGUI.GetCustomIcon($"{XBinder_Dashboard.get_path_xbinder_gui()}gui_adapter_multi/icon_main");
            icon_dot = XGUI.GetBasedIcon($"icon_dot");
            #endregion

            //设定列表项高度值
            bind_item_list_item_height = 60;
            bind_item_list_visible_count = 10;

            #region ReorderableList
            bind_item_list = new ReorderableList(serializedObject, sp_entries, true, true, true, true);
            bind_item_list.drawElementCallback = bind_item_list_DrawElementCallback;
            #endregion

            Targets_Get();
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            string hexcol = XGUI_Utilitys.Color_To_HexString(Color.white, true);

            TextClipping clipping_ellipsis = XGUI.TryEllipsisClipping();

            #region 标题
            XGUI.layout_banner(
                bg_fill: XGUIGroup.实体,
                bg_color: XGUIColor.深空灰,
                bg_height: 30,
                icon: icon_main,
                icon_color: XBinder_Dashboard.theme_primary,
                title_text: string.IsNullOrEmpty(sp_identifier.stringValue) ? "XBinder  -  绑定适配器（批量）" : "XBinder - 绑定适配器（批量）-> ( " + sp_identifier.stringValue + " )",
                title_anchor: TextAnchor.MiddleLeft,
                title_style: FontStyle.Normal,
                title_color: Color.white,
                title_size: XGUIFontSize.B,
                title_clipping: clipping_ellipsis,
                bg_margin: new RectOffset(0, 0, 5, 5));
            #endregion

            XGUI.layout_space(5);

            #region 参数
            base_script.fold_param = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XBinder_Dashboard.theme_group,
                title: "参数",
                title_size: XGUIFontSize.M,
                title_text_color: XBinder_Dashboard.theme_primary,
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(5, 5, 20, 15),
                foldout: base_script.fold_param);

            if (!base_script.fold_param)
            {
                Draw_Params();
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 绑定项列表
            base_script.fold_list = XGUI.layout_group_start(
                  type: XGUIContainerType.Vertical,
                  bg_fill: XGUIGroup.缺口纯色边框,
                  bg_color: XGUIColor.亮白,
                  bg_color_gui: XBinder_Dashboard.theme_group,
                  title: "绑定项列表",
                  title_size: XGUIFontSize.M,
                  title_text_color: XBinder_Dashboard.theme_primary,
                  title_clipping: TextClipping.Clip,
                  padding: new RectOffset(5, 5, 20, 15),
                  foldout: base_script.fold_list);

            if (!base_script.fold_list)
            {
                bind_item_list_Draw();
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 扩展面板
            Draw_Extended();
            #endregion

            #region 源脚本
            base_script.fold_based = XGUI.layout_group_start(
            type: XGUIContainerType.Vertical,
            bg_fill: XGUIGroup.缺口纯色边框,
            bg_color: XGUIColor.亮白,
            bg_color_gui: XBinder_Dashboard.theme_group,
            title: "源脚本",
            title_size: XGUIFontSize.M,
            title_text_color: XBinder_Dashboard.theme_primary,
            title_clipping: TextClipping.Clip,
            padding: new RectOffset(10, 10, 15, 15),
            foldout: base_script.fold_based);

            if (!base_script.fold_based)
            {
                DrawDefaultInspector();
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            serializedObject.ApplyModifiedProperties();
        }
        public override bool HasPreviewGUI()
        {
            return false;
        }
        #endregion

        #region 列表绘制
        private void bind_item_list_DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty item = sp_entries.GetArrayElementAtIndex(index);

            SerializedProperty sp_key = item.FindPropertyRelative(nameof(XBinder_Adapter_BindEntry.target_key));
            SerializedProperty sp_cat = item.FindPropertyRelative(nameof(XBinder_Adapter_BindEntry.target_category));
            SerializedProperty sp_type = item.FindPropertyRelative(nameof(XBinder_Adapter_BindEntry.value_type));

            //Debug.Log(rect.width);

            bool isshirink = rect.width <= 125;
            TextClipping clipping_ellipsis = XGUI.TryEllipsisClipping();
            float field_height = XGUI.GetSingleLineHeight() + 3;

            #region 序号
            if (!isshirink)
            {
                bind_item_list_drawelement_rect.Set(rect.x + 20, rect.y + 4, 30, 20);
                XGUI.gui_label(
                    rect: bind_item_list_drawelement_rect,
                    text: new GUIContent(index.ToString("D2")),
                    text_color: Color.gray,
                    size: XGUIFontSize.S,
                    clipping: TextClipping.Clip,
                    anchor: TextAnchor.MiddleLeft,
                    offset: new Vector2(0, 0),
                    font_style: FontStyle.Normal);
            }
            #endregion

            #region 图标
            if (!isshirink)
            {
                bind_item_list_drawelement_rect.Set(rect.x + 7, rect.y + 12, icon_dot.width, icon_dot.height);
                XGUI.gui_icon(
                    rect: bind_item_list_drawelement_rect,
                    icon: icon_dot,
                    color: bind_item_list_selected_index == index ? XBinder_Dashboard.theme_primary : Color.black * 0.3f);
            }
            #endregion

            #region 键
            bind_item_list_drawelement_rect.Set(rect.x + (isshirink ? 5 : 42), rect.y + 4, rect.width / 2 - (isshirink ? 10 : 20), field_height);
            sp_key.stringValue = XGUI.gui_inputfield(
                  rect: bind_item_list_drawelement_rect,
                  title: "键：",
                  title_width: 20,
                  prop: sp_key.stringValue,
                  text_wrap: false,
                  field_fontsize: XGUIFontSize.M,
                  field_text_offset: Vector2.zero,
                  field_height: field_height,
                  field_text_color: Color.white,
                  field_text_style: FontStyle.Normal,
                  field_text_anchor: TextAnchor.MiddleLeft,
                  field_padding: new RectOffset(5, 5, 0, 0),
                  field_margin: new RectOffset(0, 0, 0, 0));
            #endregion

            #region 类型
            bind_item_list_drawelement_rect.Set((rect.width / 2) + (isshirink ? 0 : 26), rect.y + 6, (rect.width / 2) - (isshirink ? 20 : 45), XGUI.GetSingleLineHeight() + 3);
            XGUI.gui_property_field(
             rect: bind_item_list_drawelement_rect,
             prop: sp_type);
            #endregion

            #region 目标源
            GUI.backgroundColor = string.IsNullOrEmpty(sp_cat.stringValue) ? Color.white * 0.8f : Color.white;
            bind_item_list_drawelement_rect.Set(rect.x + (isshirink ? 5 : 42), rect.y + rect.height - field_height - 10, rect.width - (rect.x + (isshirink ? 5 : 42)) - (isshirink ? 20 : 20), field_height);
            sp_cat.stringValue = XGUI.gui_inputfield(
                  rect: bind_item_list_drawelement_rect,
                  title: "目标数据源：",
                  title_width: 65,
                  title_color: string.IsNullOrEmpty(sp_cat.stringValue) ? Color.gray * 0.85f : Color.white * 0.9f,
                  prop: sp_cat.stringValue,
                  text_wrap: false,
                  field_fontsize: XGUIFontSize.M,
                  field_text_offset: Vector2.zero,
                  field_height: field_height,
                  field_text_color: Color.white,
                  field_text_style: FontStyle.Normal,
                  field_text_anchor: TextAnchor.MiddleLeft,
                  field_padding: new RectOffset(5, 5, 0, 0),
                  field_margin: new RectOffset(0, 0, 0, 0));
            GUI.backgroundColor = Color.white;
            #endregion

            if (index < bind_item_list.count)
            {
                bind_item_list_drawelement_rect.Set(rect.x, rect.y + rect.height - 5, rect.width, 1);
                XGUI.gui_seperator(
                    rect: bind_item_list_drawelement_rect,
                    thickness: 1,
                    color: Color.gray * 0.7f);
            }

            #region 检测鼠标点击事件
            Event e = Event.current;
            bind_item_list_drawelement_rect.Set(rect.x, rect.y, rect.width - 50, rect.height);
            if (e.type == EventType.MouseDown && e.button == 1 && bind_item_list_drawelement_rect.Contains(e.mousePosition))
            {
                // 更新选中项
                bind_item_list_selected_index = index;
            }

            GUI.enabled = true;
            #endregion

            sp_key.serializedObject.ApplyModifiedProperties();
            sp_cat.serializedObject.ApplyModifiedProperties();
            sp_type.serializedObject.ApplyModifiedProperties();
        }
        /// <summary>
        /// 绘制绑定项列表
        /// <para/>
        /// 只绘制滚动视图可视区域内的元素，以优化大列表性能。
        /// </summary>
        private void bind_item_list_Draw()
        {
            // 绘制滚动视图
            bind_item_list_scrollview_rect = GUILayoutUtility.GetRect(0, bind_item_list_visible_count * bind_item_list_item_height);
            bind_item_list_scroller = GUI.BeginScrollView(bind_item_list_scrollview_rect, bind_item_list_scroller, new Rect(0, 0, bind_item_list_scrollview_rect.width - 50, bind_item_list.count * bind_item_list_item_height), false, true);

            // 计算可视区域的起始和结束索引
            int startIndex = Mathf.FloorToInt(bind_item_list_scroller.y / bind_item_list_item_height);
            int endIndex = Mathf.CeilToInt((bind_item_list_scroller.y + bind_item_list_scrollview_rect.height) / bind_item_list_item_height);

            bool isshirink = bind_item_list_scrollview_rect.width <= 125;

            // 只绘制可视区域内的元素
            for (int i = startIndex; i < endIndex && i < bind_item_list.count; i++)
            {
                SerializedProperty prop = sp_entries.GetArrayElementAtIndex(i);

                // 如果当前元素被选中，绘制选中效果
                if (bind_item_list_selected_index == i)
                {
                    // 高亮背景表示选中
                    bind_item_list_rect.Set(1, i * bind_item_list_item_height, bind_item_list_scrollview_rect.width + 20, bind_item_list_item_height - 5);
                    EditorGUI.DrawRect(bind_item_list_rect, bind_item_list_selected_bg);
                }

                bind_item_list_rect.Set(0, i * bind_item_list_item_height, bind_item_list_scrollview_rect.width, bind_item_list_item_height);

                bind_item_list.drawElementCallback.Invoke(bind_item_list_rect, i, i == bind_item_list.index, true);

                // 检测鼠标是否在当前元素区域内
                if (bind_item_list_rect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.MouseDown)
                    {
                        // 更新选中项
                        bind_item_list_selected_index = i;

                        // 标记界面需要更新
                        GUI.changed = true;
                    }
                }

                // 绘制元素
                //EditorGUI.PropertyField(bind_item_list_rect, prop, GUIContent.none);
            }

            GUI.EndScrollView();
        }
        #endregion

        #region 面板绘制   
        public virtual void Draw_Params()
        {
            #region 标识名称
            XGUI.layout_property_field(
            title: "标识名称",
            title_size: XGUIFontSize.M,
            title_hover_color: XBinder_Dashboard.theme_primary,
            title_width: 120,
            prop: sp_identifier,
            prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 全局数据源
            XGUI.layout_property_field(
                title: "全局数据源",
                title_size: XGUIFontSize.M,
                title_hover_color: XBinder_Dashboard.theme_primary,
                title_width: 120,
                prop: sp_global_category,
                prop_margin: new RectOffset(0, 0, 5, 0));

            XGUI.layout_label(
               text: "如果绑定项列表中的数据源未指定\n则会使用该值作为备用数据源！",
               size: XGUIFontSize.S,
               text_color: Color.white * 0.7f,
               margin: new RectOffset(15, 15, 10, 10),
               clipping: TextClipping.Clip,
               height: 45,
               wrap: false,
               font_style: FontStyle.Normal,
               anchor: TextAnchor.MiddleCenter);
            #endregion
        }
        public virtual void Draw_Extended()
        {

        }
        #endregion
    }
}