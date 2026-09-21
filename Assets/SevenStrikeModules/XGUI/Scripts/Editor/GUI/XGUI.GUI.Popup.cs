/*
 * ============================================================================
 * ⚠️ 版权声明（禁止删除、禁止修改、衍生作品必须保留此注释）⚠️
 * ============================================================================
 * 版权声明 Copyright (C) 2025-Present Nanjing SevenStrike Media Co., Ltd.
 * 中文名称：南京塞维斯传媒有限公司
 * 英文名称：SevenStrikeMedia
 * 项目作者：徐寅智
 * 项目名称：XGUI - Unity Editor界面可视化组件工具
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
namespace SevenStrikeModules.XGUI.Editor
{
    using SevenStrikeModules.XGUI.Runtime;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// 为 Editor 界面提供样式控件
    /// </summary>
    public static partial class XGUI
    {
        /// <summary>
        /// 绘制一个带有标签的整数下拉选择框（基于 SerializedProperty）。
        /// </summary>
        /// <param name="rect">下拉框的位置和尺寸。</param>
        /// <param name="title">标签文本。为 <c>null</c> 或空时不显示标签。</param>
        /// <param name="title_color">标签文本的颜色。传入 <see cref="Color.clear"/> 时使用半透明白色。</param>
        /// <param name="title_size">标签文本的字体大小。</param>
        /// <param name="title_font_style">标签文本的字体样式。</param>
        /// <param name="title_padding">标签的内边距。</param>
        /// <param name="title_width">标签的固定宽度。</param>
        /// <param name="interval">标签与下拉框之间的间距。</param>
        /// <param name="prop">包含当前选中索引的序列化整数属性。</param>
        /// <param name="options">下拉选项的显示文本数组。</param>
        /// <param name="font">下拉框文本的字体。为 <c>null</c> 时使用样式默认字体。</param>
        /// <param name="opt_text_size">下拉框选项文本的字体大小。</param>
        /// <param name="opt_text_color">下拉框选项文本的颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="opt_text_padding">下拉框文本的内边距。</param>
        /// <param name="opt_anchor">下拉框选项文本的对齐方式。</param>
        /// <param name="opt_font_style">下拉框选项文本的字体样式。</param>
        /// <param name="opt_bg_fill">下拉框背景的填充样式。</param>
        /// <param name="opt_bg_color">下拉框背景的颜色主题。</param>
        /// <param name="opt_bg_color_gui">下拉框背景的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="added_height">下拉框额外增加的高度。</param>
        /// <param name="icon_arrow_color">下拉箭头图标的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <returns>用户选择后的整数值（索引）。</returns>
        /// <remarks>
        /// 该版本基于 <see cref="SerializedProperty"/>，支持多选混合状态显示（显示 "—"）。
        public static int gui_int_popup(Rect rect = default, string title = null, Color title_color = default, XGUIFontSize title_size = XGUIFontSize.M, FontStyle title_font_style = FontStyle.Normal, RectOffset title_padding = default, float title_width = 60, float interval = 20, SerializedProperty prop = null, string[] options = null, Font font = null, XGUIFontSize opt_text_size = XGUIFontSize.M, Color opt_text_color = default, RectOffset opt_text_padding = default, TextAnchor opt_anchor = TextAnchor.MiddleLeft, FontStyle opt_font_style = FontStyle.Normal, XGUIFilled opt_bg_fill = XGUIFilled.实体, XGUIColor opt_bg_color = XGUIColor.亮白, Color opt_bg_color_gui = default, float added_height = 0, Color icon_arrow_color = default)
        {
            if (title_color == Color.clear)
                title_color = Color.white * 0.9f;

            if (icon_arrow_color == Color.clear)
                icon_arrow_color = Color.white;

            if (title_padding == null)
                title_padding = new RectOffset(0, 0, 0, 0);

            if (opt_text_padding == null)
                opt_text_padding = new RectOffset(0, 0, 0, 0);

            if (opt_text_color == Color.clear)
                opt_text_color = Color.white;

            if (opt_bg_color_gui == Color.clear)
                opt_bg_color_gui = Color.white;

            rect.Set(rect.x, rect.y, rect.width - rect.x - 5, rect.height);

            Rect rect_title = new Rect(rect.x, rect.y, rect.width < 130 ? rect.width * 0.5f : title_width + interval, rect.height);

            XGUI.gui_label(
                rect: rect_title,
                text: new GUIContent(title),
                text_color: title_color,
                offset: new Vector2(0, -1),
                size: title_size,
                padding: title_padding,
                clipping: TextClipping.Clip);

            Rect rect_popup = new Rect(rect.width < (title_width + interval + 40) ? rect.x + rect.width - 40 : rect.x + (title_width + interval), rect.y, rect.width < (title_width + interval + 40) ? 40 : rect.width - (title_width + interval), rect.height);

            #region 下拉菜单样式
            GUIStyle style = new GUIStyle(XGUI.style_xg_popup);
            style.normal.background = GetFillTexture(opt_bg_fill, opt_bg_color);
            style.normal.textColor = opt_text_color;
            style.fontSize = XGUI.GetFontSize(opt_text_size);

            if (font != null)
                style.font = font;

            RectOffset _p = style.padding;
            style.padding = new RectOffset(_p.left + opt_text_padding.left, _p.right + opt_text_padding.right, _p.top + opt_text_padding.top, _p.bottom + opt_text_padding.bottom);

            if (added_height != 0)
                style.fixedHeight += added_height;
            style.alignment = opt_anchor;
            #endregion

            #region 下拉菜单控件
            // 检查是否为多选混合状态
            bool isMixed = prop.hasMultipleDifferentValues;

            Color col = GUI.backgroundColor;
            GUI.backgroundColor = opt_bg_color_gui;
            if (isMixed)
            {
                // 显示带“—”的按钮
                if (GUI.Button(rect_popup, "—", style))
                {
                    // 点击时可以选择应用到所有
                    GenericMenu menu = new GenericMenu();
                    for (int i = 0; i < options.Length; i++)
                    {
                        int index = i; // 关键：创建局部变量捕获当前值
                        menu.AddItem(new GUIContent(options[i]), false, () => SetPopupValue(prop, index));
                    }
                    menu.ShowAsContext();
                }
            }
            else
            {
                // 正常显示 Popup
                int newValue = EditorGUI.Popup(rect_popup, prop.intValue, options, style);
                if (newValue != prop.intValue)
                {
                    prop.intValue = newValue;
                }
            }
            GUI.backgroundColor = col;

            prop.serializedObject.ApplyModifiedProperties();
            #endregion

            Rect rect_arrow = new Rect(rect_popup.x + rect_popup.width - 25, rect_popup.y + 5, 6, 6);

            // 箭头图标
            XGUI.gui_icon(
                rect: rect_arrow,
                icon: XGUI.GetBasedIcon("icon_arrow"),
                color: icon_arrow_color);

            return prop.intValue;
        }
        /// <summary>
        /// 绘制一个带有标签的整数下拉选择框（基于值类型）。
        /// </summary>
        /// <param name="rect">下拉框的位置和尺寸。</param>
        /// <param name="title">标签文本。为 <c>null</c> 或空时不显示标签。</param>
        /// <param name="title_color">标签文本的颜色。传入 <see cref="Color.clear"/> 时使用半透明白色。</param>
        /// <param name="title_size">标签文本的字体大小。</param>
        /// <param name="title_font_style">标签文本的字体样式。</param>
        /// <param name="title_padding">标签的内边距。</param>
        /// <param name="title_width">标签的固定宽度。</param>
        /// <param name="interval">标签与下拉框之间的间距。</param>
        /// <param name="prop">当前选中的整数值（索引）。</param>
        /// <param name="options">下拉选项的显示文本数组。</param>
        /// <param name="font">下拉框文本的字体。为 <c>null</c> 时使用样式默认字体。</param>
        /// <param name="opt_text_size">下拉框选项文本的字体大小。</param>
        /// <param name="opt_text_color">下拉框选项文本的颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="opt_text_padding">下拉框文本的内边距。</param>
        /// <param name="opt_anchor">下拉框选项文本的对齐方式。</param>
        /// <param name="opt_font_style">下拉框选项文本的字体样式。</param>
        /// <param name="opt_bg_fill">下拉框背景的填充样式。</param>
        /// <param name="opt_bg_color">下拉框背景的颜色主题。</param>
        /// <param name="opt_bg_color_gui">下拉框背景的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="added_height">下拉框额外增加的高度。</param>
        /// <param name="icon_arrow_color">下拉箭头图标的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <returns>用户选择后的整数值（索引）。</returns>
        /// <remarks>
        /// 该版本基于值类型（int），适用于非序列化属性的场景，不处理多选混合状态。
        /// </remarks>
        public static int gui_int_popup(Rect rect = default, string title = null, Color title_color = default, XGUIFontSize title_size = XGUIFontSize.M, FontStyle title_font_style = FontStyle.Normal, RectOffset title_padding = default, float title_width = 60, float interval = 20, int prop = 0, string[] options = null, Font font = null, XGUIFontSize opt_text_size = XGUIFontSize.M, Color opt_text_color = default, RectOffset opt_text_padding = default, TextAnchor opt_anchor = TextAnchor.MiddleLeft, FontStyle opt_font_style = FontStyle.Normal, XGUIFilled opt_bg_fill = XGUIFilled.实体, XGUIColor opt_bg_color = XGUIColor.亮白, Color opt_bg_color_gui = default, float added_height = 0, Color icon_arrow_color = default)
        {
            if (title_color == Color.clear)
                title_color = Color.white * 0.9f;

            if (icon_arrow_color == Color.clear)
                icon_arrow_color = Color.white;

            if (title_padding == null)
                title_padding = new RectOffset(0, 0, 0, 0);

            if (opt_text_padding == null)
                opt_text_padding = new RectOffset(0, 0, 0, 0);

            if (opt_text_color == Color.clear)
                opt_text_color = Color.white;

            if (opt_bg_color_gui == Color.clear)
                opt_bg_color_gui = Color.white;

            rect.Set(rect.x, rect.y, rect.width - rect.x - 5, rect.height);

            Rect rect_title = new Rect(rect.x, rect.y, rect.width < 130 ? rect.width * 0.5f : title_width + interval, rect.height);

            XGUI.gui_label(
                rect: rect_title,
                text: new GUIContent(title),
                text_color: title_color,
                offset: new Vector2(0, -1),
                size: title_size,
                padding: title_padding,
                clipping: TextClipping.Clip);

            Rect rect_popup = new Rect(rect.width < (title_width + interval + 40) ? rect.x + rect.width - 40 : rect.x + (title_width + interval), rect.y, rect.width < (title_width + interval + 40) ? 40 : rect.width - (title_width + interval), rect.height);

            #region 下拉菜单样式
            GUIStyle style = new GUIStyle(XGUI.style_xg_popup);
            style.normal.background = GetFillTexture(opt_bg_fill, opt_bg_color);
            style.normal.textColor = opt_text_color;
            style.fontSize = XGUI.GetFontSize(opt_text_size);

            if (font != null)
                style.font = font;

            RectOffset _p = style.padding;
            style.padding = new RectOffset(_p.left + opt_text_padding.left, _p.right + opt_text_padding.right, _p.top + opt_text_padding.top, _p.bottom + opt_text_padding.bottom);

            if (added_height != 0)
                style.fixedHeight += added_height;
            style.alignment = opt_anchor;
            #endregion

            #region 下拉菜单控件
            Color col = GUI.backgroundColor;
            GUI.backgroundColor = opt_bg_color_gui;

            // 正常显示 Popup
            int newValue = EditorGUI.Popup(rect_popup, prop, options, style);
            if (newValue != prop)
            {
                prop = newValue;
            }

            GUI.backgroundColor = col;

            #endregion

            Rect rect_arrow = new Rect(rect_popup.x + rect_popup.width - 25, rect_popup.y + 5, 6, 6);

            // 箭头图标
            XGUI.gui_icon(
                rect: rect_arrow,
                icon: XGUI.GetBasedIcon("icon_arrow"),
                color: icon_arrow_color);

            return prop;
        }
        /// <summary>
        /// 绘制一个带有标签的字符串下拉选择框（基于 SerializedProperty）。
        /// </summary>
        /// <param name="rect">下拉框的位置和尺寸。</param>
        /// <param name="title">标签文本。为 <c>null</c> 或空时不显示标签。</param>
        /// <param name="title_color">标签文本的颜色。传入 <see cref="Color.clear"/> 时使用半透明白色。</param>
        /// <param name="title_size">标签文本的字体大小。</param>
        /// <param name="title_font_style">标签文本的字体样式。</param>
        /// <param name="title_padding">标签的内边距。</param>
        /// <param name="title_width">标签的固定宽度。</param>
        /// <param name="interval">标签与下拉框之间的间距。</param>
        /// <param name="prop">包含当前选中字符串值的序列化属性。</param>
        /// <param name="options">下拉选项的显示文本数组。</param>
        /// <param name="font">下拉框文本的字体。为 <c>null</c> 时使用样式默认字体。</param>
        /// <param name="opt_text_size">下拉框选项文本的字体大小。</param>
        /// <param name="opt_text_color">下拉框选项文本的颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="opt_text_padding">下拉框文本的内边距。</param>
        /// <param name="opt_anchor">下拉框选项文本的对齐方式。</param>
        /// <param name="opt_font_style">下拉框选项文本的字体样式。</param>
        /// <param name="opt_bg_fill">下拉框背景的填充样式。</param>
        /// <param name="opt_bg_color">下拉框背景的颜色主题。</param>
        /// <param name="opt_bg_color_gui">下拉框背景的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="added_height">下拉框额外增加的高度。</param>
        /// <param name="icon_arrow_color">下拉箭头图标的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <returns>用户选择后的字符串值。</returns>
        /// <remarks>
        /// 该版本基于 <see cref="SerializedProperty"/>，支持多选混合状态显示（显示 "—"）。
        /// 通过将字符串值与选项数组匹配来确定当前选中项，选中后通过 <see cref="SerializedProperty.stringValue"/> 设置值。
        /// </remarks>
        public static string gui_string_popup(Rect rect = default, string title = null, Color title_color = default, XGUIFontSize title_size = XGUIFontSize.M, FontStyle title_font_style = FontStyle.Normal, RectOffset title_padding = default, float title_width = 60, float interval = 20, SerializedProperty prop = null, string[] options = null, Font font = null, XGUIFontSize opt_text_size = XGUIFontSize.M, Color opt_text_color = default, RectOffset opt_text_padding = default, TextAnchor opt_anchor = TextAnchor.MiddleCenter, FontStyle opt_font_style = FontStyle.Normal, XGUIFilled opt_bg_fill = XGUIFilled.实体, XGUIColor opt_bg_color = XGUIColor.亮白, Color opt_bg_color_gui = default, float added_height = 0, Color icon_arrow_color = default)
        {
            if (title_color == Color.clear)
                title_color = Color.white * 0.9f;

            if (icon_arrow_color == Color.clear)
                icon_arrow_color = Color.white;

            if (title_padding == null)
                title_padding = new RectOffset(0, 0, 0, 0);

            if (opt_text_padding == null)
                opt_text_padding = new RectOffset(0, 0, 0, 0);

            if (opt_text_color == Color.clear)
                opt_text_color = Color.white;

            if (opt_bg_color_gui == Color.clear)
                opt_bg_color_gui = Color.white;

            rect.Set(rect.x, rect.y, rect.width - rect.x - 5, rect.height);

            Rect rect_title = new Rect(rect.x, rect.y, rect.width < 130 ? rect.width * 0.5f : title_width + interval, rect.height);

            XGUI.gui_label(
                rect: rect_title,
                text: new GUIContent(title),
                text_color: title_color,
                offset: new Vector2(0, -1),
                size: title_size,
                padding: title_padding,
                clipping: TextClipping.Clip);

            Rect rect_popup = new Rect(rect.width < (title_width + interval + 40) ? rect.x + rect.width - 40 : rect.x + (title_width + interval), rect.y, rect.width < (title_width + interval + 40) ? 40 : rect.width - (title_width + interval), rect.height);

            #region 下拉菜单样式
            GUIStyle style = new GUIStyle(XGUI.style_xg_popup);
            style.normal.background = GetFillTexture(opt_bg_fill, opt_bg_color);
            style.normal.textColor = opt_text_color;
            style.fontSize = XGUI.GetFontSize(opt_text_size);

            if (font != null)
                style.font = font;

            RectOffset _p = style.padding;
            style.padding = new RectOffset(_p.left + opt_text_padding.left, _p.right + opt_text_padding.right, _p.top + opt_text_padding.top, _p.bottom + opt_text_padding.bottom);

            if (added_height != 0)
                style.fixedHeight = rect.height + added_height;
            else
                style.fixedHeight = rect.height;

            style.alignment = opt_anchor;
            #endregion

            int _index = GetStringOptionIndex(prop, options);

            #region 下拉菜单控件
            // 检查是否为多选混合状态
            bool isMixed = prop.hasMultipleDifferentValues;

            Color col = GUI.backgroundColor;
            GUI.backgroundColor = opt_bg_color_gui;
            if (isMixed)
            {
                // 显示带“—”的按钮
                if (GUI.Button(rect_popup, "—", style))
                {
                    // 点击时可以选择应用到所有
                    GenericMenu menu = new GenericMenu();
                    for (int i = 0; i < options.Length; i++)
                    {
                        int index = i; // 关键：创建局部变量捕获当前值
                        menu.AddItem(new GUIContent(options[i]), false, () => SetPopupValue(prop, options[index]));
                    }
                    menu.ShowAsContext();
                }
            }
            else
            {
                // 正常显示 Popup
                int x_index = EditorGUI.Popup(rect_popup, _index, options, style);
                if (x_index != _index)
                {
                    SetStringOption(prop, options[x_index]);
                }
            }
            GUI.backgroundColor = col;

            prop.serializedObject.ApplyModifiedProperties();
            #endregion

            Rect rect_arrow = new Rect(rect_popup.x + rect_popup.width - 25, rect_popup.y + 5, 6, 6);

            // 箭头图标
            XGUI.gui_icon(
                rect: rect_arrow,
                icon: XGUI.GetBasedIcon("icon_arrow"),
                color: icon_arrow_color);

            return prop.stringValue;
        }
        /// <summary>
        /// 绘制一个带有标签的字符串下拉选择框（基于值类型）。
        /// </summary>
        /// <param name="rect">下拉框的位置和尺寸。</param>
        /// <param name="title">标签文本。为 <c>null</c> 或空时不显示标签。</param>
        /// <param name="title_color">标签文本的颜色。传入 <see cref="Color.clear"/> 时使用半透明白色。</param>
        /// <param name="title_size">标签文本的字体大小。</param>
        /// <param name="title_font_style">标签文本的字体样式。</param>
        /// <param name="title_padding">标签的内边距。</param>
        /// <param name="title_width">标签的固定宽度。</param>
        /// <param name="interval">标签与下拉框之间的间距。</param>
        /// <param name="prop">当前选中的字符串值（引用传递，直接修改）。</param>
        /// <param name="options">下拉选项的显示文本数组。</param>
        /// <param name="font">下拉框文本的字体。为 <c>null</c> 时使用样式默认字体。</param>
        /// <param name="opt_text_size">下拉框选项文本的字体大小。</param>
        /// <param name="opt_text_color">下拉框选项文本的颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="opt_text_padding">下拉框文本的内边距。</param>
        /// <param name="opt_anchor">下拉框选项文本的对齐方式。</param>
        /// <param name="opt_font_style">下拉框选项文本的字体样式。</param>
        /// <param name="opt_bg_fill">下拉框背景的填充样式。</param>
        /// <param name="opt_bg_color">下拉框背景的颜色主题。</param>
        /// <param name="opt_bg_color_gui">下拉框背景的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="added_height">下拉框额外增加的高度。</param>
        /// <param name="icon_arrow_color">下拉箭头图标的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <returns>用户选择后的字符串值。</returns>
        /// <remarks>
        /// 该版本基于值类型（string），适用于非序列化属性的场景，不处理多选混合状态。
        /// 通过将当前值与选项数组匹配来确定选中索引，用户选择后更新值并返回。
        /// </remarks>
        public static string gui_string_popup(Rect rect = default, string title = null, Color title_color = default, XGUIFontSize title_size = XGUIFontSize.M, FontStyle title_font_style = FontStyle.Normal, RectOffset title_padding = default, float title_width = 60, float interval = 20, string prop = null, string[] options = null, Font font = null, XGUIFontSize opt_text_size = XGUIFontSize.M, Color opt_text_color = default, RectOffset opt_text_padding = default, TextAnchor opt_anchor = TextAnchor.MiddleCenter, FontStyle opt_font_style = FontStyle.Normal, XGUIFilled opt_bg_fill = XGUIFilled.实体, XGUIColor opt_bg_color = XGUIColor.亮白, Color opt_bg_color_gui = default, float added_height = 0, Color icon_arrow_color = default)
        {
            if (title_color == Color.clear)
                title_color = Color.white * 0.9f;

            if (icon_arrow_color == Color.clear)
                icon_arrow_color = Color.white;

            if (title_padding == null)
                title_padding = new RectOffset(0, 0, 0, 0);

            if (opt_text_padding == null)
                opt_text_padding = new RectOffset(0, 0, 0, 0);

            if (opt_text_color == Color.clear)
                opt_text_color = Color.white;

            if (opt_bg_color_gui == Color.clear)
                opt_bg_color_gui = Color.white;

            rect.Set(rect.x, rect.y, rect.width + 2, rect.height);


            float _x = rect.x;
            float _y = rect.y;
            float _w = title_width + interval;
            float _h = rect.height;
            Rect rect_title = new Rect(rect.x, rect.y, _w, _h);

            XGUI.gui_label(
                rect: rect_title,
                text: new GUIContent(title),
                text_color: title_color,
                offset: new Vector2(0, -1),
                size: title_size,
                padding: title_padding,
                clipping: TextClipping.Clip);

            float p_x = rect.x + (title_width + interval);
            float p_y = rect.y;
            float p_w = rect.width - (title_width + interval);
            float p_h = rect.height;
            Rect rect_popup = new Rect(p_x, p_y, p_w, p_h);

            #region 下拉菜单样式
            GUIStyle style = new GUIStyle(XGUI.style_xg_popup);
            style.normal.background = GetFillTexture(opt_bg_fill, opt_bg_color);
            style.normal.textColor = opt_text_color;
            style.fontSize = XGUI.GetFontSize(opt_text_size);

            if (font != null)
                style.font = font;

            RectOffset _p = style.padding;
            style.padding = new RectOffset(_p.left + opt_text_padding.left, _p.right + opt_text_padding.right, _p.top + opt_text_padding.top, _p.bottom + opt_text_padding.bottom);

            if (added_height != 0)
                style.fixedHeight = rect.height + added_height;
            else
                style.fixedHeight = rect.height;

            style.alignment = opt_anchor;
            #endregion

            int _index = GetStringOptionIndex(prop, options);

            #region 下拉菜单控件
            // 检查是否为多选混合状态
            Color col = GUI.backgroundColor;
            GUI.backgroundColor = opt_bg_color_gui;

            // 正常显示 Popup
            int x_index = EditorGUI.Popup(rect_popup, _index, options, style);
            if (x_index != _index)
            {
                SetStringOption(ref prop, options[x_index]);
            }
            GUI.backgroundColor = col;

            #endregion

            Rect rect_arrow = new Rect(rect_popup.x + rect_popup.width - 25, rect_popup.y + 5, 6, 6);

            // 箭头图标
            XGUI.gui_icon(
                rect: rect_arrow,
                icon: XGUI.GetBasedIcon("icon_arrow"),
                color: icon_arrow_color);

            //XGUI.gui_box(rect, Color.green * 0.8f);
            return prop;
        }
    }
}