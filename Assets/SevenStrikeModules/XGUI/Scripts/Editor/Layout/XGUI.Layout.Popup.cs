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
    using System;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// 为 Editor 界面提供样式控件
    /// </summary>
    public static partial class XGUI
    {
        /// <summary>
        /// 使用自动布局绘制一个带有标签的整数下拉选择框（基于 SerializedProperty）。
        /// </summary>
        /// <param name="title">下拉框的标签文本。为 <c>null</c> 或空时不显示标签。</param>
        /// <param name="title_width">标签的固定宽度。</param>
        /// <param name="title_color">标签文本的颜色。传入 <see cref="Color.clear"/> 时使用半透明白色。</param>
        /// <param name="title_size">标签文本的字体大小。</param>
        /// <param name="title_anchor">标签文本的对齐方式。</param>
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
        /// <param name="act_on_changed">当选中项发生改变时的回调委托，参数为新选中的索引值。</param>
        /// <returns>用户选择后的整数值（索引）。</returns>
        /// <remarks>
        /// 该方法与 <see cref="gui_int_popup(Rect, string, Color, XGUIFontSize, FontStyle, RectOffset, float, float, SerializedProperty, string[], Font, XGUIFontSize, Color, RectOffset, TextAnchor, FontStyle, XGUIFilled, XGUIColor, Color, float, Color)"/> 
        /// 功能相同，但使用 <see cref="EditorGUILayout"/> 自动布局系统，无需手动指定矩形位置。
        /// <para>
        /// 支持多选混合状态显示（显示 "—"），当 <paramref name="prop"/> 存在多个不同值时自动切换为混合模式。
        /// </para>
        /// </remarks>
        public static int layout_int_popup(string title = null, float title_width = 80, Color title_color = default, XGUIFontSize title_size = XGUIFontSize.M, TextAnchor title_anchor = TextAnchor.MiddleLeft, SerializedProperty prop = null, string[] options = null, Font font = null, XGUIFontSize opt_text_size = XGUIFontSize.M, Color opt_text_color = default, RectOffset opt_text_padding = default, TextAnchor opt_anchor = TextAnchor.MiddleCenter, FontStyle opt_font_style = FontStyle.Normal, XGUIFilled opt_bg_fill = XGUIFilled.实体, XGUIColor opt_bg_color = XGUIColor.亮白, Color opt_bg_color_gui = default, float added_height = 0, Color icon_arrow_color = default, Action<int> act_on_changed = null, RectOffset padding = default, RectOffset margin = default, RectOffset title_margin = default)
        {
            if (opt_text_padding == null)
                opt_text_padding = new RectOffset(0, 0, 0, 0);

            if (padding == null)
                padding = new RectOffset(0, 0, 0, 0);

            if (margin == null)
                margin = new RectOffset(0, 0, 0, 0);

            if (title_margin == null)
                title_margin = new RectOffset(0, 0, 0, 0);

            if (title_color == Color.clear)
                title_color = Color.white * 0.9f;

            if (opt_text_color == Color.clear)
                opt_text_color = Color.white;

            if (opt_bg_color_gui == Color.clear)
                opt_bg_color_gui = Color.white;

            XGUI.layout_group_start(
                type: XGUIContainerType.Horizontal,
                bg_fill: XGUIFilled.无,
                bg_color: XGUIColor.无,
                title_clipping: TextClipping.Clip,
                absolute_padding: true,
                absolute_margin: true,
                padding: padding,
                margin: margin);

            XGUI.layout_label(
                text: title,
                size: title_size,
                text_color: title_color,
                offset: new Vector2(0, 0),
                margin: title_margin,
                clipping: TextClipping.Clip,
                anchor: title_anchor,
                width: title_width);

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

            EditorGUI.BeginChangeCheck();

            #region 下拉菜单控件
            // 检查是否为多选混合状态
            bool isMixed = prop.hasMultipleDifferentValues;

            Color col = GUI.backgroundColor;
            GUI.backgroundColor = opt_bg_color_gui;
            if (isMixed)
            {
                // 显示带“—”的按钮
                if (GUILayout.Button("—", style, GUILayout.MinWidth(20), GUILayout.Height(XGUI.GetSingleLineHeight())))
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
                int newValue = EditorGUILayout.Popup(prop.intValue, options, style, GUILayout.MinWidth(20), GUILayout.Height(XGUI.GetSingleLineHeight()));
                if (newValue != prop.intValue)
                {
                    prop.intValue = newValue;
                }
            }
            GUI.backgroundColor = col;

            prop.serializedObject.ApplyModifiedProperties();
            #endregion

            if (EditorGUI.EndChangeCheck())
            {
                if (act_on_changed != null)
                    act_on_changed(prop.intValue);
            }

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);

            Rect rect_arrow_last = XGUI.GetLastRect();
            Rect rect_arrow = new Rect(rect_arrow_last.x + rect_arrow_last.width - 25, rect_arrow_last.y + 5, 6, 6);

            // 箭头图标
            XGUI.gui_icon(
                rect: rect_arrow,
                icon: XGUI.GetBasedIcon("icon_arrow"),
                color: icon_arrow_color);

            return prop.intValue;
        }
        /// <summary>
        /// 使用自动布局绘制一个带有标签的字符串下拉选择框（基于 SerializedProperty）。
        /// </summary>
        /// <param name="title">下拉框的标签文本。为 <c>null</c> 或空时不显示标签。</param>
        /// <param name="title_width">标签的固定宽度。</param>
        /// <param name="title_color">标签文本的颜色。传入 <see cref="Color.clear"/> 时使用半透明白色。</param>
        /// <param name="title_size">标签文本的字体大小。</param>
        /// <param name="title_anchor">标签文本的对齐方式。</param>
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
        /// <param name="act_on_changed">当选中项发生改变时的回调委托，参数为新选中的字符串值。</param>
        /// <returns>用户选择后的字符串值。</returns>
        /// <remarks>
        /// 该方法与 <see cref="gui_string_popup(Rect, string, Color, XGUIFontSize, FontStyle, RectOffset, float, float, SerializedProperty, string[], Font, XGUIFontSize, Color, RectOffset, TextAnchor, FontStyle, XGUIFilled, XGUIColor, Color, float, Color)"/> 
        /// 功能相同，但使用 <see cref="EditorGUILayout"/> 自动布局系统，无需手动指定矩形位置。
        /// <para>
        /// 支持多选混合状态显示（显示 "—"），当 <paramref name="prop"/> 存在多个不同值时自动切换为混合模式。
        /// </para>
        /// </remarks>
        public static string layout_string_popup(string title = null, float title_width = 80, Color title_color = default, XGUIFontSize title_size = XGUIFontSize.M, TextAnchor title_anchor = TextAnchor.MiddleLeft, SerializedProperty prop = null, string[] options = null, Font font = null, XGUIFontSize opt_text_size = XGUIFontSize.M, Color opt_text_color = default, RectOffset opt_text_padding = default, TextAnchor opt_anchor = TextAnchor.MiddleCenter, FontStyle opt_font_style = FontStyle.Normal, XGUIFilled opt_bg_fill = XGUIFilled.实体, XGUIColor opt_bg_color = XGUIColor.亮白, Color opt_bg_color_gui = default, float added_height = 0, Color icon_arrow_color = default, Action<string> act_on_changed = null, RectOffset padding = default, RectOffset margin = default, RectOffset title_margin = default)
        {
            if (opt_text_padding == null)
                opt_text_padding = new RectOffset(0, 0, 0, 0);

            if (padding == null)
                padding = new RectOffset(0, 0, 0, 0);

            if (margin == null)
                margin = new RectOffset(0, 0, 0, 0);

            if (title_margin == null)
                title_margin = new RectOffset(0, 0, 0, 0);

            if (title_color == Color.clear)
                title_color = Color.white * 0.9f;

            if (icon_arrow_color == Color.clear)
                icon_arrow_color = Color.white;

            if (opt_text_color == Color.clear)
                opt_text_color = Color.white;

            if (opt_bg_color_gui == Color.clear)
                opt_bg_color_gui = Color.white;

            XGUI.layout_group_start(
                type: XGUIContainerType.Horizontal,
                bg_fill: XGUIFilled.无,
                bg_color: XGUIColor.无,
                title_clipping: TextClipping.Clip,
                absolute_padding: true,
                absolute_margin: true,
                padding: padding,
                margin: margin);

            XGUI.layout_label(
                text: title,
                size: title_size,
                text_color: title_color,
                offset: new Vector2(0, 0),
                margin: title_margin,
                clipping: TextClipping.Clip,
                anchor: title_anchor,
                width: title_width);

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

            int _index = GetStringOptionIndex(prop, options);
            EditorGUI.BeginChangeCheck();

            #region 下拉菜单控件
            // 检查是否为多选混合状态
            bool isMixed = prop.hasMultipleDifferentValues;

            Color col = GUI.backgroundColor;
            GUI.backgroundColor = opt_bg_color_gui;
            if (isMixed)
            {
                // 显示带“—”的按钮
                if (GUILayout.Button("—", style, GUILayout.MinWidth(20), GUILayout.Height(XGUI.GetSingleLineHeight())))
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
                int x_index = EditorGUILayout.Popup(_index, options, style, GUILayout.MinWidth(20), GUILayout.Height(XGUI.GetSingleLineHeight()));
                if (x_index != _index)
                {
                    SetStringOption(prop, options[x_index]);
                }
            }
            GUI.backgroundColor = col;

            prop.serializedObject.ApplyModifiedProperties();
            #endregion

            if (EditorGUI.EndChangeCheck())
            {
                if (act_on_changed != null)
                    act_on_changed(prop.stringValue);
            }

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);

            Rect rect_arrow_last = XGUI.GetLastRect();
            Rect rect_arrow = new Rect(rect_arrow_last.x + rect_arrow_last.width - 25, rect_arrow_last.y + 5, 6, 6);

            // 箭头图标
            XGUI.gui_icon(
                rect: rect_arrow,
                icon: XGUI.GetBasedIcon("icon_arrow"),
                color: icon_arrow_color);

            return prop.stringValue;
        }
        /// <summary>
        /// 设置序列化属性的整数值（用于泛型菜单回调）。
        /// </summary>
        /// <param name="prop">要设置的序列化属性。</param>
        /// <param name="value">要设置的值。</param>
        private static void SetPopupValue(SerializedProperty prop, int value)
        {
            prop.intValue = value;
            prop.serializedObject.ApplyModifiedProperties();
        }
        /// <summary>
        /// 设置序列化属性的字符串值（用于泛型菜单回调）。
        /// </summary>
        /// <param name="prop">要设置的序列化属性。</param>
        /// <param name="value">要设置的值。</param>
        private static void SetPopupValue(SerializedProperty prop, string value)
        {
            prop.stringValue = value;
            prop.serializedObject.ApplyModifiedProperties();
        }
        /// <summary>
        /// 获取字符串选项在选项数组中的索引位置。
        /// </summary>
        /// <param name="prop">包含字符串值的序列化属性。</param>
        /// <param name="options">选项字符串数组。</param>
        /// <returns>匹配的索引，若未找到则返回 0。</returns>
        private static int GetStringOptionIndex(SerializedProperty prop, string[] options)
        {
            string value = prop.stringValue;
            for (int i = 0; i < options.Length; i++)
            {
                if (options[i] == value)
                    return i;
            }

            return 0;
        }
        /// <summary>
        /// 获取字符串选项在选项数组中的索引位置。
        /// </summary>
        /// <param name="prop">要查找的字符串值。</param>
        /// <param name="options">选项字符串数组。</param>
        /// <returns>匹配的索引，若未找到则返回 0。</returns>
        private static int GetStringOptionIndex(string prop, string[] options)
        {
            string value = prop;
            for (int i = 0; i < options.Length; i++)
            {
                if (options[i] == value)
                    return i;
            }

            return 0;
        }
        /// <summary>
        /// 设置序列化属性的字符串值。
        /// </summary>
        /// <param name="prop">要设置的序列化属性。</param>
        /// <param name="value">要设置的值。</param>
        private static void SetStringOption(SerializedProperty prop, string value)
        {
            prop.stringValue = value;
            prop.serializedObject.ApplyModifiedProperties();
        }
        /// <summary>
        /// 设置字符串变量的值（用于泛型菜单回调）。
        /// </summary>
        /// <param name="prop">要设置的字符串变量（引用传递）。</param>
        /// <param name="value">要设置的值。</param>
        private static void SetStringOption(ref string prop, string value)
        {
            prop = value;
        }
    }
}