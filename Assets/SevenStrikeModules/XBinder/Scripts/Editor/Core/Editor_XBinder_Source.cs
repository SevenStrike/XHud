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
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(XBinder_Source), true)]
    public class Editor_XBinder_Source : Editor
    {
        #region 字段 - 组件 / 列表
        public XBinder_Source base_script;
        #endregion

        #region 字段 - 序列化属性
        private SerializedProperty sp_identifier;
        private SerializedProperty sp_request_key;
        private SerializedProperty sp_manual_request;
        private SerializedProperty sp_request_interval;
        private SerializedProperty sp_category;
        #endregion

        #region 字段 - 图标
        /// <summary>
        /// 面板主标题图标
        /// </summary>
        private Texture2D icon_main;
        #endregion

        #region 字段 - 选项文字
        /// <summary>
        /// 开关控件的"混合选项"文字
        /// </summary>
        string[] stroptions_auto = new string[2] { "手动", "自动" };
        #endregion

        #region 字段 - 数据容器视图
        /// <summary>
        /// 数据容器列表的滚动位置
        /// </summary>
        private Vector2 _category_scroll;
        /// <summary>
        /// 缓存的数据容器键列表（避免每帧重建 HashSet）
        /// </summary>
        private List<string> _cached_category_keys = new List<string>();
        /// <summary>
        /// 上次刷新缓存的时间戳
        /// </summary>
        private double _category_cache_time = 0;
        /// <summary>
        /// 数据容器列表的刷新间隔（秒）
        /// </summary>
        private const double CATEGORY_REFRESH_INTERVAL = 0.2;
        #endregion

        #region 字段 - 批量化操作
        /// <summary>
        /// 当前 Inspector 选中的所有目标组件缓存。
        /// </summary>
        private XBinder_Source[] selected_objects;
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
                selected_objects = new XBinder_Source[targets.Length];
                for (int i = 0; i < selected_objects.Length; i++)
                {
                    var t = targets[i];
                    selected_objects[i] = (XBinder_Source)t;
                }
            }
            else
            {
                selected_objects = new XBinder_Source[targets.Length];
                selected_objects[0] = (XBinder_Source)target;
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

        #region 生命周期
        public virtual void OnEnable()
        {
            base_script = (XBinder_Source)target;

            #region 获取序列化字段
            sp_identifier = serializedObject.FindProperty(nameof(XBinder_Source.identifier));
            sp_category = serializedObject.FindProperty(nameof(XBinder_Source.category));
            sp_request_interval = serializedObject.FindProperty(nameof(XBinder_Source.request_interval));
            sp_manual_request = serializedObject.FindProperty(nameof(XBinder_Source.manual_request));
            sp_request_key = serializedObject.FindProperty(nameof(XBinder_Source.request_key));
            #endregion

            #region 图标获取
            icon_main = XGUI.GetCustomIcon($"{XBinder_Dashboard.Get_Path_XBinder_GUIROOT()}gui_binder/icon_main");
            #endregion

            Targets_Get();
        }
        public virtual void OnDisable()
        {
            _cached_category_keys.Clear();
            _category_cache_time = 0;
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
                title_text: string.IsNullOrEmpty(sp_identifier.stringValue) ? "XBinder  -  数据源加载器" : "XBinder - 数据源加载器-> ( " + sp_identifier.stringValue + " )",
                title_anchor: TextAnchor.MiddleLeft,
                title_style: FontStyle.Normal,
                title_color: Color.white,
                title_size: XGUIFontSize.B,
                title_clipping: clipping_ellipsis,
                bg_margin: new RectOffset(0, 0, 5, 5));
            #endregion

            XGUI.layout_space(5);

            #region 快捷操作
            XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XBinder_Dashboard.theme_group,
                title: "快捷操作",
                title_size: XGUIFontSize.M,
                title_text_color: XBinder_Dashboard.theme_primary,
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(5, 5, 20, 15));

            Draw_FastTool();

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

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
                XGUI.ChangedCheck_Start();

                Draw_Params();

                if (XGUI.ChangedCheck_End())
                {

                }
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 数据容器概览
            base_script.fold_category = XGUI.layout_group_start(
             type: XGUIContainerType.Vertical,
             bg_fill: XGUIGroup.缺口纯色边框,
             bg_color: XGUIColor.亮白,
             bg_color_gui: XBinder_Dashboard.theme_group,
             title: "数据源概览",
             title_size: XGUIFontSize.M,
             title_text_color: XBinder_Dashboard.theme_primary,
             title_clipping: TextClipping.Clip,
             padding: new RectOffset(5, 5, 20, 15),
             foldout: base_script.fold_category);

            if (!base_script.fold_category)
            {
                Draw_Category();
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 选项
            base_script.fold_option = XGUI.layout_group_start(
             type: XGUIContainerType.Vertical,
             bg_fill: XGUIGroup.缺口纯色边框,
             bg_color: XGUIColor.亮白,
             bg_color_gui: XBinder_Dashboard.theme_group,
             title: "选项",
             title_size: XGUIFontSize.M,
             title_text_color: XBinder_Dashboard.theme_primary,
             title_clipping: TextClipping.Clip,
             padding: new RectOffset(5, 5, 20, 15),
             foldout: base_script.fold_option);

            if (!base_script.fold_option)
            {
                Draw_Options();
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
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

        #region 面板绘制    
        public virtual void Draw_FastTool()
        {
            if (Application.isPlaying)
            {
                #region 手动绑定源
                if (XGUI.layout_button(
                    text: base_script.category_created ? "卸载数据源" : "加载数据源",
                    height: 26,
                    bg_color_gui: XBinder_Dashboard.theme_primary,
                    font_size: XGUIFontSize.M,
                    press_fill: XGUIGroup.实体,
                    press_color: XGUIColor.深空灰,
                    button_text_color: Color.black,
                    press_text_color: Color.white))
                {
                    if (!base_script.category_created)
                        base_script.category_create();
                    else
                        base_script.category_remove();
                }
                #endregion
            }
            else
            {
                XGUI.layout_label(
                text: "运行时可手动控制数据源的加载与卸载",
                size: XGUIFontSize.S,
                text_color: Color.white * 0.7f,
                margin: new RectOffset(15, 15, 10, 10),
                clipping: TextClipping.Clip,
                wrap: true,
                font_style: FontStyle.Normal,
                anchor: TextAnchor.MiddleCenter);
            }
        }
        public virtual void Draw_Options()
        {
            #region 手动请求数据
            DrawToggle("手动请求数据", sp_manual_request, 120, XGUIToggleStyle.实体, XBinder_Dashboard.theme_primary, Color.white * 0.65f, Color.white, Color.white, stroptions_auto, (b) => { });
            #endregion
        }
        public virtual void Draw_Params()
        {
            #region 创建的数据容器标识名称
            XGUI.layout_property_field(
                title: "数据容器标识",
                title_size: XGUIFontSize.M,
                title_hover_color: XBinder_Dashboard.theme_primary,
                title_width: 120,
                prop: sp_identifier,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            if (sp_manual_request.boolValue)
            {
                #region 手动请求数据按键
                XGUI.layout_property_field(
                    title: "请求数据按键",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XBinder_Dashboard.theme_primary,
                    title_width: 120,
                    prop: sp_request_key,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion
            }
            else
            {
                #region 自动请求数据间隔
                XGUI.layout_property_field(
                    title: "请求数据间隔",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XBinder_Dashboard.theme_primary,
                    title_width: 120,
                    prop: sp_request_interval,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion
            }
        }
        private void Draw_Category()
        {
            var cat = base_script.category;

            if (cat == null)
            {
                XGUI.layout_label(
                    text: "数据源将在运行时（Awake）创建，进入 Play 模式后可查看数据源的内容。",
                    size: XGUIFontSize.S,
                    text_color: Color.white * 0.7f,
                    margin: new RectOffset(15, 15, 10, 10),
                    clipping: TextClipping.Clip,
                    wrap: true,
                    font_style: FontStyle.Normal,
                    anchor: TextAnchor.MiddleCenter);
                return;
            }

            // 低频刷新键缓存，避免每帧 OnInspectorGUI 重建 HashSet
            if (EditorApplication.timeSinceStartup - _category_cache_time >= CATEGORY_REFRESH_INTERVAL)
            {
                _category_cache_time = EditorApplication.timeSinceStartup;

                var keys = cat.get_all_keys();
                _cached_category_keys.Clear();
                foreach (var k in keys)
                    _cached_category_keys.Add(k);

                // 稳定排序，避免列表抖动
                _cached_category_keys.Sort(StringComparer.Ordinal);
            }

            // 顶部统计
            XGUI.layout_state_displayer_text(
                title: "绑定的数据项",
                title_size: XGUIFontSize.M,
                subtitle: $"{cat.get_key_count()}",
                subtitle_size: XGUIFontSize.M,
                subtitle_color: XBinder_Dashboard.theme_primary,
                margin: new RectOffset(5, 5, 0, 0));

            XGUI.layout_state_displayer_text(
                title: "实际数据量",
                title_size: XGUIFontSize.M,
                subtitle: $"{_cached_category_keys.Count}",
                subtitle_size: XGUIFontSize.M,
                subtitle_color: XBinder_Dashboard.theme_primary,
                margin: new RectOffset(5, 5, 0, 5));

            XGUI.layout_label(
                   text: "提示：若两值不同，说明有同名键写入了不同类型数据条目（会导致订阅失效）；若相同则代表所有数据条目类型唯一。",
                   size: XGUIFontSize.S,
                   text_color: Color.white * 0.7f,
                   margin: new RectOffset(15, 15, 10, 10),
                   clipping: TextClipping.Clip,
                   wrap: true,
                   font_style: FontStyle.Normal,
                   anchor: TextAnchor.MiddleLeft);

            if (_cached_category_keys.Count == 0)
            {
                XGUI.layout_label(
                    text: "当前数据源内暂无数据。",
                    size: XGUIFontSize.M,
                    text_color: Color.white * 0.5f,
                    margin: new RectOffset(15, 15, 5, 5),
                    clipping: TextClipping.Clip,
                    font_style: FontStyle.Normal,
                    anchor: TextAnchor.MiddleLeft);
                return;
            }

            XGUI.layout_seperator(
                thickness: 1,
                color: XBinder_Dashboard.theme_sepline,
                margin: new RectOffset(5, 5, 10, 10));

            // 列表
            _category_scroll = EditorGUILayout.BeginScrollView(_category_scroll, GUILayout.MaxHeight(260));
            foreach (var key in _cached_category_keys)
            {
                var type = cat.get_key_type(key);
                var val = cat.get_value_as_object(key);
                int subs = cat.get_subscriber_count(key);

                string typeTag = type.ToString();
                string valStr = FormatCategoryValue(val);

                XGUI.layout_state_displayer_text(
                    title: $"[ {typeTag} ]",
                    title_color: Color.gray,
                    title_size: XGUIFontSize.XS,
                    margin: new RectOffset(0, 0, 0, 0),
                    padding: new RectOffset(0, 0, 0, 0),
                    subtitle: $"已订阅 : {subs}",
                    subtitle_size: XGUIFontSize.S,
                    subtitle_color: subs <= 0 ? Color.gray : Color.yellow);

                XGUI.layout_state_displayer_text(
                    title: key,
                    title_size: XGUIFontSize.BX,
                    margin: new RectOffset(0, 0, 0, 0),
                    padding: new RectOffset(0, 0, 0, 0),
                    subtitle: valStr,
                    subtitle_size: XGUIFontSize.M,
                    subtitle_color: XBinder_Dashboard.theme_primary);

                XGUI.layout_seperator(
                    thickness: 1,
                    color: XBinder_Dashboard.theme_sepline,
                    margin: new RectOffset(5, 5, 10, 10));
            }
            EditorGUILayout.EndScrollView();
        }
        /// <summary>
        /// 将通道值格式化为可读字符串（针对 Unity 类型与集合做特判）
        /// </summary>
        /// <param name="v">通道值</param>
        /// <returns>格式化后的字符串</returns>
        private static string FormatCategoryValue(object v)
        {
            if (v == null) return "null";

            if (v is string s) return s;
            if (v is float f) return f.ToString("F3");
            if (v is int i) return i.ToString();
            if (v is bool b) return b ? "true" : "false";

            if (v is Vector2 v2) return $"({v2.x:F2},{v2.y:F2})";
            if (v is Vector3 v3) return $"({v3.x:F2},{v3.y:F2},{v3.z:F2})";
            if (v is Vector4 v4) return $"({v4.x:F2},{v4.y:F2},{v4.z:F2},{v4.w:F2})";
            if (v is Quaternion q) return $"({q.x:F2},{q.y:F2},{q.z:F2},{q.w:F2})";
            if (v is Rect r) return $"(x:{r.x:F1},y:{r.y:F1},w:{r.width:F1},h:{r.height:F1})";
            if (v is Color c) return $"RGBA({c.r:F2},{c.g:F2},{c.b:F2},{c.a:F2})";

            // Unity 对象：先判 == null（销毁后 == null 为 true），再取 name
            if (v is UnityEngine.Object uo)
                return uo == null ? "null (已销毁)" : $"{uo.GetType().Name}: {uo.name}";

            // 集合：显示长度与前几项类型
            if (v is System.Collections.ICollection col)
                return $"{v.GetType().Name}[{col.Count}]";

            return v.ToString();
        }
        #endregion

        #region 绘制控件
        /// <summary>
        /// 通用方法：绘制带标题的开关控件。
        /// <para/>
        /// 封装 <see cref="XGUI.layout_toggle"/>，统一标题宽度、字体、混合选项样式。
        /// </summary>
        public void DrawToggle(string title, SerializedProperty prop, float width, XGUIToggleStyle style = XGUIToggleStyle.实体, Color color_bg_on = default, Color color_bg_off = default, Color color_on = default, Color color_off = default, string[] options = null, Action<bool> act_on_changed = null)
        {
            XGUI.layout_toggle(
                title: title,
                title_size: XGUIFontSize.M,
                title_font_style: FontStyle.Normal,
                title_padding: new RectOffset(5, 10, 0, 0),
                title_width: width,
                prop: prop,
                tog_style: style,
                tog_padding: new RectOffset(0, 9, 0, 0),
                tog_margin: new RectOffset(0, 0, 0, 5),
                tog_mixed_options: options,
                tog_mixed_text_size: XGUIFontSize.M,
                tog_mixed_text_color: Color.black,
                tog_mixed_text_padding: new RectOffset(10, 10, 0, 0),
                tog_mixed_text_anchor: TextAnchor.MiddleCenter,
                tog_mixed_font_style: FontStyle.Normal,
                tog_bg_off_color: color_bg_off,
                tog_bg_on_color: color_bg_on,
                tog_handler_off_color: color_off,
                tog_handler_on_color: color_on,
                tog_mixed_bg_color_gui: XBinder_Dashboard.theme_primary,
                act_on_changed: act_on_changed);
        }
        #endregion
    }
}