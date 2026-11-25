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
        /// 绘制一个带有标签的文本输入框。
        /// </summary>
        /// <param name="rect">输入框的位置和尺寸。</param>
        /// <param name="prop">输入框的当前文本内容（引用传递，直接修改）。</param>
        /// <param name="title">输入框的标签文本。为 <c>null</c> 或空时不显示标签。</param>
        /// <param name="title_size">标签文本的字体大小。</param>
        /// <param name="title_anchor">标签文本的对齐方式。</param>
        /// <param name="title_color">标签文本的颜色。传入 <see cref="Color.clear"/> 时使用半透明白色。</param>
        /// <param name="title_width">标签的固定宽度。为 0 时自动计算宽度。</param>
        /// <param name="text_wrap">是否启用文本换行。</param>
        /// <param name="field_fontsize">输入字段的字体大小。</param>
        /// <param name="field_text_offset">输入字段文本的偏移量。</param>
        /// <param name="field_height">输入字段的高度。为 0 时使用默认行高。</param>
        /// <param name="field_text_color">输入字段文本的颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="field_text_font">输入字段的字体。为 <c>null</c> 时使用样式默认字体。</param>
        /// <param name="field_text_style">输入字段的字体样式。</param>
        /// <param name="field_text_anchor">输入字段文本的对齐方式。</param>
        /// <param name="status_icon">状态图标的名称（通过 <see cref="GetBasedIcon"/> 加载）。为 <c>null</c> 时不显示图标。</param>
        /// <param name="status_icon_color">状态图标的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="limite_width">显示标签的窗口宽度阈值。当前窗口宽度大于该值时显示标签，否则隐藏。</param>
        /// <param name="field_padding">输入字段的内边距，覆盖样式默认值。</param>
        /// <param name="field_margin">输入字段的外边距，覆盖样式默认值。</param>
        /// <returns>用户输入后的文本内容。</returns>
        /// <remarks>
        /// 该方法是带有标签的增强版输入框，标签在窗口宽度足够时显示在输入框左侧。
        /// 支持状态图标显示在输入框右侧，用于指示输入状态（如验证通过/失败）。
        /// </remarks>
        public static string gui_inputfield(Rect rect = default, string prop = null, string title = null, XGUIFontSize title_size = XGUIFontSize.M, TextAnchor title_anchor = TextAnchor.MiddleLeft, Color title_color = default, float title_width = 0, bool text_wrap = true, XGUIFontSize field_fontsize = XGUIFontSize.M, Vector2 field_text_offset = default, float field_height = 20, Color field_text_color = default, Font field_text_font = null, FontStyle field_text_style = FontStyle.Normal, TextAnchor field_text_anchor = TextAnchor.UpperLeft, string status_icon = null, Color status_icon_color = default, float limite_width = 230, RectOffset field_padding = default, RectOffset field_margin = default)
        {
            if (field_padding == null)
                field_padding = new RectOffset(0, 0, 0, 0);

            if (field_margin == null)
                field_margin = new RectOffset(0, 0, 0, 0);

            if (title_color == Color.clear)
                title_color = Color.white * 0.9f;

            if (field_text_color == Color.clear)
                field_text_color = Color.white;

            GUIStyle style = new GUIStyle(style_xg_input);
            style.fontSize = XGUI.GetFontSize(field_fontsize);
            style.wordWrap = text_wrap;

            style.normal.textColor = field_text_color;
            style.focused.textColor = field_text_color;
            style.active.textColor = field_text_color;
            style.fontStyle = field_text_style;
            style.contentOffset = field_text_offset;
            style.alignment = field_text_anchor;

            style.padding = field_padding;
            style.margin = field_margin;

            if (field_text_font != null)
                style.font = field_text_font;

            bool exp = false;
            if (XGUI.GetCurrentWindowWidth() > limite_width)
                exp = true;
            else
                exp = false;

            // 测试区域
            //XGUI.gui_box(rect, Color.red * 0.5f);

            if (!string.IsNullOrEmpty(title) && exp)
            {
                #region 标题
                Rect rect_title = new Rect(rect.x, rect.y, title_width, field_height);
                XGUI.gui_label(
                    rect: rect_title,
                    text: new GUIContent(title),
                    text_color: title_color,
                    size: title_size,
                    clipping: TextClipping.Overflow,
                    anchor: title_anchor,
                    offset: new Vector2(0, 0),
                    wrap: text_wrap,
                    font_style: FontStyle.Normal);
                #endregion
            }

            float field_x = exp ? (!string.IsNullOrEmpty(title) ? rect.x + title_width + 10 : rect.x) : rect.x;
            float field_y = rect.y;
            float field_w = exp ? (!string.IsNullOrEmpty(title) ? rect.width - (title_width + 10 + (status_icon != null ? 10 : 0)) : rect.width - (status_icon != null ? 10 : 0)) : rect.width - (status_icon != null ? 10 : 0);
            float field_h = field_height == 0 ? XGUI.GetSingleLineHeight() : field_height;
            Rect rect_field = new Rect(field_x, field_y, field_w, field_h);
            prop = EditorGUI.TextField(rect_field, GUIContent.none, prop, style);

            if (status_icon != null)
            {
                float x = rect.x + rect.width;
                float y = rect.y + 6;
                float w = 5;
                float h = 5;

                Rect rect_icon = new Rect(x, y, w, h);

                XGUI.gui_icon(
                    rect: rect_icon,
                    icon: XGUI.GetBasedIcon(status_icon),
                    color: status_icon_color);
            }

            return prop;
        }
        /// <summary>
        /// 绘制一个带有标签的浮点数输入框。
        /// </summary>
        /// <param name="rect">输入框的位置和尺寸。</param>
        /// <param name="prop">输入框的当前浮点数值。</param>
        /// <param name="title">输入框的标签文本。为 <c>null</c> 或空时不显示标签。</param>
        /// <param name="title_size">标签文本的字体大小。</param>
        /// <param name="title_anchor">标签文本的对齐方式。</param>
        /// <param name="title_color">标签文本的颜色。传入 <see cref="Color.clear"/> 时使用半透明白色。</param>
        /// <param name="title_width">标签的固定宽度。为 0 时自动计算宽度。</param>
        /// <param name="field_fontsize">输入字段的字体大小。</param>
        /// <param name="field_text_offset">输入字段文本的偏移量。</param>
        /// <param name="field_height">输入字段的高度。为 0 时使用默认行高。</param>
        /// <param name="field_text_color">输入字段文本的颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="field_text_font">输入字段的字体。为 <c>null</c> 时使用样式默认字体。</param>
        /// <param name="field_text_style">输入字段的字体样式。</param>
        /// <param name="field_text_anchor">输入字段文本的对齐方式。</param>
        /// <param name="status_icon">状态图标的名称。为 <c>null</c> 时不显示图标。</param>
        /// <param name="status_icon_color">状态图标的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="limite_width">显示标签的窗口宽度阈值。</param>
        /// <param name="field_padding">输入字段的内边距，覆盖样式默认值。</param>
        /// <param name="field_margin">输入字段的外边距，覆盖样式默认值。</param>
        /// <param name="decimals">浮点数保留的小数位数，取值范围 0-6。</param>
        /// <returns>用户输入后的浮点数值，并自动舍入到指定小数位数。</returns>
        public static float gui_inputfield(Rect rect = default, float prop = 0, string title = null, XGUIFontSize title_size = XGUIFontSize.M, TextAnchor title_anchor = TextAnchor.MiddleLeft, Color title_color = default, float title_width = 0, XGUIFontSize field_fontsize = XGUIFontSize.M, Vector2 field_text_offset = default, float field_height = 20, Color field_text_color = default, Font field_text_font = null, FontStyle field_text_style = FontStyle.Normal, TextAnchor field_text_anchor = TextAnchor.UpperLeft, string status_icon = null, Color status_icon_color = default, float limite_width = 230, RectOffset field_padding = default, RectOffset field_margin = default, int decimals = 2)
        {
            if (field_padding == null)
                field_padding = new RectOffset(0, 0, 0, 0);

            if (field_margin == null)
                field_margin = new RectOffset(0, 0, 0, 0);

            if (title_color == Color.clear)
                title_color = Color.white * 0.9f;

            if (field_text_color == Color.clear)
                field_text_color = Color.white;

            GUIStyle style = new GUIStyle(style_xg_input);
            style.fontSize = XGUI.GetFontSize(field_fontsize);
            style.wordWrap = false;

            style.normal.textColor = field_text_color;
            style.focused.textColor = field_text_color;
            style.active.textColor = field_text_color;
            style.fontStyle = field_text_style;
            style.contentOffset = field_text_offset;
            style.alignment = field_text_anchor;
            style.padding = field_padding;
            style.margin = field_margin;

            if (field_text_font != null)
                style.font = field_text_font;

            bool exp = false;
            if (XGUI.GetCurrentWindowWidth() > limite_width)
                exp = true;
            else
                exp = false;

            // 测试区域
            //XGUI.gui_box(rect, Color.red * 0.5f);

            if (!string.IsNullOrEmpty(title) && exp)
            {
                #region 标题
                Rect rect_title = new Rect(rect.x, rect.y, title_width, field_height);
                XGUI.gui_label(
                    rect: rect_title,
                    text: new GUIContent(title),
                    text_color: title_color,
                    size: title_size,
                    clipping: TextClipping.Overflow,
                    anchor: title_anchor,
                    offset: new Vector2(0, 0),
                    wrap: true,
                    font_style: FontStyle.Normal);
                #endregion
            }

            float field_x = exp ? (!string.IsNullOrEmpty(title) ? rect.x + title_width + 10 : rect.x) : rect.x;
            float field_y = rect.y;
            float field_w = exp ? (!string.IsNullOrEmpty(title) ? rect.width - (title_width + 10 + (status_icon != null ? 10 : 0)) : rect.width - (status_icon != null ? 10 : 0)) : rect.width - (status_icon != null ? 10 : 0);
            float field_h = XGUI.GetSingleLineHeight();
            Rect rect_field = new Rect(field_x, field_y, field_w, field_h);
            prop = EditorGUI.FloatField(rect_field, GUIContent.none, prop, style);

            if (status_icon != null)
            {
                float x = rect.x + rect.width;
                float y = rect.y + 6;
                float w = 5;
                float h = 5;

                Rect rect_icon = new Rect(x, y, w, h);

                XGUI.gui_icon(
                    rect: rect_icon,
                    icon: XGUI.GetBasedIcon(status_icon),
                    color: status_icon_color);
            }

            // 舍入保护
            decimals = Mathf.Clamp(decimals, 0, 6);

            return (float)Math.Round(prop, decimals, MidpointRounding.AwayFromZero);
        }
        /// <summary>
        /// 绘制一个带有标签的整数输入框。
        /// </summary>
        /// <param name="rect">输入框的位置和尺寸。</param>
        /// <param name="prop">输入框的当前整数值。</param>
        /// <param name="title">输入框的标签文本。为 <c>null</c> 或空时不显示标签。</param>
        /// <param name="title_size">标签文本的字体大小。</param>
        /// <param name="title_anchor">标签文本的对齐方式。</param>
        /// <param name="title_color">标签文本的颜色。传入 <see cref="Color.clear"/> 时使用半透明白色。</param>
        /// <param name="title_width">标签的固定宽度。为 0 时自动计算宽度。</param>
        /// <param name="field_fontsize">输入字段的字体大小。</param>
        /// <param name="field_text_offset">输入字段文本的偏移量。</param>
        /// <param name="field_height">输入字段的高度。为 0 时使用默认行高。</param>
        /// <param name="field_text_color">输入字段文本的颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="field_text_font">输入字段的字体。为 <c>null</c> 时使用样式默认字体。</param>
        /// <param name="field_text_style">输入字段的字体样式。</param>
        /// <param name="field_text_anchor">输入字段文本的对齐方式。</param>
        /// <param name="status_icon">状态图标的名称。为 <c>null</c> 时不显示图标。</param>
        /// <param name="status_icon_color">状态图标的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="limite_width">显示标签的窗口宽度阈值。</param>
        /// <param name="field_padding">输入字段的内边距，覆盖样式默认值。</param>
        /// <param name="field_margin">输入字段的外边距，覆盖样式默认值。</param>
        /// <returns>用户输入后的整数值。</returns>
        public static int gui_inputfield(Rect rect = default, int prop = 0, string title = null, XGUIFontSize title_size = XGUIFontSize.M, TextAnchor title_anchor = TextAnchor.MiddleLeft, Color title_color = default, float title_width = 0, XGUIFontSize field_fontsize = XGUIFontSize.M, Vector2 field_text_offset = default, float field_height = 20, Color field_text_color = default, Font field_text_font = null, FontStyle field_text_style = FontStyle.Normal, TextAnchor field_text_anchor = TextAnchor.UpperLeft, string status_icon = null, Color status_icon_color = default, float limite_width = 230, RectOffset field_padding = default, RectOffset field_margin = default)
        {
            if (field_padding == null)
                field_padding = new RectOffset(0, 0, 0, 0);

            if (field_margin == null)
                field_margin = new RectOffset(0, 0, 0, 0);

            if (title_color == Color.clear)
                title_color = Color.white * 0.9f;

            if (field_text_color == Color.clear)
                field_text_color = Color.white;

            GUIStyle style = new GUIStyle(style_xg_input);
            style.fontSize = XGUI.GetFontSize(field_fontsize);
            style.wordWrap = false;

            style.normal.textColor = field_text_color;
            style.focused.textColor = field_text_color;
            style.active.textColor = field_text_color;
            style.fontStyle = field_text_style;
            style.contentOffset = field_text_offset;
            style.alignment = field_text_anchor;
            style.padding = field_padding;
            style.margin = field_margin;

            if (field_text_font != null)
                style.font = field_text_font;

            bool exp = false;
            if (XGUI.GetCurrentWindowWidth() > limite_width)
                exp = true;
            else
                exp = false;

            // 测试区域
            //XGUI.gui_box(rect, Color.red * 0.5f);

            if (!string.IsNullOrEmpty(title) && exp)
            {
                #region 标题
                Rect rect_title = new Rect(rect.x, rect.y, title_width, field_height);
                XGUI.gui_label(
                    rect: rect_title,
                    text: new GUIContent(title),
                    text_color: title_color,
                    size: title_size,
                    clipping: TextClipping.Overflow,
                    anchor: title_anchor,
                    offset: new Vector2(0, 0),
                    wrap: true,
                    font_style: FontStyle.Normal);
                #endregion
            }

            float field_x = exp ? (!string.IsNullOrEmpty(title) ? rect.x + title_width + 10 : rect.x) : rect.x;
            float field_y = rect.y;
            float field_w = exp ? (!string.IsNullOrEmpty(title) ? rect.width - (title_width + 10 + (status_icon != null ? 10 : 0)) : rect.width - (status_icon != null ? 10 : 0)) : rect.width - (status_icon != null ? 10 : 0);
            float field_h = XGUI.GetSingleLineHeight();
            Rect rect_field = new Rect(field_x, field_y, field_w, field_h);
            prop = EditorGUI.IntField(rect_field, GUIContent.none, prop, style);

            if (status_icon != null)
            {
                float x = rect.x + rect.width;
                float y = rect.y + 6;
                float w = 5;
                float h = 5;

                Rect rect_icon = new Rect(x, y, w, h);

                XGUI.gui_icon(
                    rect: rect_icon,
                    icon: XGUI.GetBasedIcon(status_icon),
                    color: status_icon_color);
            }

            return prop;
        }
        /// <summary>
        /// 绘制一个带有标签的二维向量（Vector2）输入框。
        /// </summary>
        /// <param name="rect">输入框的位置和尺寸。</param>
        /// <param name="prop">输入框的当前 Vector2 值。</param>
        /// <param name="title">输入框的标签文本。为 <c>null</c> 或空时不显示标签。</param>
        /// <param name="title_size">标签文本的字体大小。</param>
        /// <param name="title_anchor">标签文本的对齐方式。</param>
        /// <param name="title_color">标签文本的颜色。传入 <see cref="Color.clear"/> 时使用半透明白色。</param>
        /// <param name="title_width">标签的固定宽度。为 0 时自动计算宽度。</param>
        /// <param name="field_height">输入字段的高度。为 0 时使用默认行高。</param>
        /// <param name="status_icon">状态图标的名称。为 <c>null</c> 时不显示图标。</param>
        /// <param name="status_icon_color">状态图标的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="limite_width">显示标签的窗口宽度阈值。</param>
        /// <returns>用户输入后的 Vector2 值。</returns>
        public static Vector2 gui_inputfield(Rect rect = default, Vector2 prop = default, string title = null, XGUIFontSize title_size = XGUIFontSize.M, TextAnchor title_anchor = TextAnchor.MiddleLeft, Color title_color = default, float title_width = 0, float field_height = 20, string status_icon = null, Color status_icon_color = default, float limite_width = 230)
        {
            if (title_color == Color.clear)
                title_color = Color.white * 0.9f;

            bool exp = false;
            if (XGUI.GetCurrentWindowWidth() > limite_width)
                exp = true;
            else
                exp = false;

            // 测试区域
            //XGUI.gui_box(rect, Color.red * 0.5f);

            if (!string.IsNullOrEmpty(title) && exp)
            {
                #region 标题
                Rect rect_title = new Rect(rect.x, rect.y, title_width, field_height);
                XGUI.gui_label(
                    rect: rect_title,
                    text: new GUIContent(title),
                    text_color: title_color,
                    size: title_size,
                    clipping: TextClipping.Overflow,
                    anchor: title_anchor,
                    offset: new Vector2(0, 0),
                    wrap: true,
                    font_style: FontStyle.Normal);
                #endregion
            }

            float field_x = exp ? (!string.IsNullOrEmpty(title) ? rect.x + title_width + 10 : rect.x) : rect.x;
            float field_y = rect.y;
            float field_w = exp ? (!string.IsNullOrEmpty(title) ? rect.width - (title_width + 10 + (status_icon != null ? 10 : 0)) : rect.width - (status_icon != null ? 10 : 0)) : rect.width - (status_icon != null ? 10 : 0);
            float field_h = XGUI.GetSingleLineHeight();
            Rect rect_field = new Rect(field_x, field_y, field_w, field_h);
            prop = EditorGUI.Vector2Field(rect_field, GUIContent.none, prop);

            if (status_icon != null)
            {
                float x = rect.x + rect.width;
                float y = rect.y + 6;
                float w = 5;
                float h = 5;

                Rect rect_icon = new Rect(x, y, w, h);

                XGUI.gui_icon(
                    rect: rect_icon,
                    icon: XGUI.GetBasedIcon(status_icon),
                    color: status_icon_color);
            }

            return prop;
        }
        /// <summary>
        /// 绘制一个带有标签的三维向量（Vector3）输入框。
        /// </summary>
        /// <param name="rect">输入框的位置和尺寸。</param>
        /// <param name="prop">输入框的当前 Vector3 值。</param>
        /// <param name="title">输入框的标签文本。为 <c>null</c> 或空时不显示标签。</param>
        /// <param name="title_size">标签文本的字体大小。</param>
        /// <param name="title_anchor">标签文本的对齐方式。</param>
        /// <param name="title_color">标签文本的颜色。传入 <see cref="Color.clear"/> 时使用半透明白色。</param>
        /// <param name="title_width">标签的固定宽度。为 0 时自动计算宽度。</param>
        /// <param name="field_height">输入字段的高度。为 0 时使用默认行高。</param>
        /// <param name="status_icon">状态图标的名称。为 <c>null</c> 时不显示图标。</param>
        /// <param name="status_icon_color">状态图标的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="limite_width">显示标签的窗口宽度阈值。</param>
        /// <returns>用户输入后的 Vector3 值。</returns>
        public static Vector3 gui_inputfield(Rect rect = default, Vector3 prop = default, string title = null, XGUIFontSize title_size = XGUIFontSize.M, TextAnchor title_anchor = TextAnchor.MiddleLeft, Color title_color = default, float title_width = 0, float field_height = 20, string status_icon = null, Color status_icon_color = default, float limite_width = 230)
        {
            if (title_color == Color.clear)
                title_color = Color.white * 0.9f;

            bool exp = false;
            if (XGUI.GetCurrentWindowWidth() > limite_width)
                exp = true;
            else
                exp = false;

            // 测试区域
            //XGUI.gui_box(rect, Color.red * 0.5f);

            if (!string.IsNullOrEmpty(title) && exp)
            {
                #region 标题
                Rect rect_title = new Rect(rect.x, rect.y, title_width, field_height);
                XGUI.gui_label(
                    rect: rect_title,
                    text: new GUIContent(title),
                    text_color: title_color,
                    size: title_size,
                    clipping: TextClipping.Overflow,
                    anchor: title_anchor,
                    offset: new Vector2(0, 0),
                    wrap: true,
                    font_style: FontStyle.Normal);
                #endregion
            }

            float field_x = exp ? (!string.IsNullOrEmpty(title) ? rect.x + title_width + 10 : rect.x) : rect.x;
            float field_y = rect.y;
            float field_w = exp ? (!string.IsNullOrEmpty(title) ? rect.width - (title_width + 10 + (status_icon != null ? 10 : 0)) : rect.width - (status_icon != null ? 10 : 0)) : rect.width - (status_icon != null ? 10 : 0);
            float field_h = XGUI.GetSingleLineHeight();
            Rect rect_field = new Rect(field_x, field_y, field_w, field_h);
            prop = EditorGUI.Vector3Field(rect_field, GUIContent.none, prop);

            if (status_icon != null)
            {
                float x = rect.x + rect.width;
                float y = rect.y + 6;
                float w = 5;
                float h = 5;

                Rect rect_icon = new Rect(x, y, w, h);

                XGUI.gui_icon(
                    rect: rect_icon,
                    icon: XGUI.GetBasedIcon(status_icon),
                    color: status_icon_color);
            }

            return prop;
        }
        /// <summary>
        /// 绘制一个带有标签的四维向量（Vector4）输入框。
        /// </summary>
        /// <param name="rect">输入框的位置和尺寸。</param>
        /// <param name="prop">输入框的当前 Vector4 值。</param>
        /// <param name="title">输入框的标签文本。为 <c>null</c> 或空时不显示标签。</param>
        /// <param name="title_size">标签文本的字体大小。</param>
        /// <param name="title_anchor">标签文本的对齐方式。</param>
        /// <param name="title_color">标签文本的颜色。传入 <see cref="Color.clear"/> 时使用半透明白色。</param>
        /// <param name="title_width">标签的固定宽度。为 0 时自动计算宽度。</param>
        /// <param name="field_height">输入字段的高度。为 0 时使用默认行高。</param>
        /// <param name="status_icon">状态图标的名称。为 <c>null</c> 时不显示图标。</param>
        /// <param name="status_icon_color">状态图标的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="limite_width">显示标签的窗口宽度阈值。</param>
        /// <returns>用户输入后的 Vector4 值。</returns>
        public static Vector4 gui_inputfield(Rect rect = default, Vector4 prop = default, string title = null, XGUIFontSize title_size = XGUIFontSize.M, TextAnchor title_anchor = TextAnchor.MiddleLeft, Color title_color = default, float title_width = 0, float field_height = 20, string status_icon = null, Color status_icon_color = default, float limite_width = 230)
        {
            if (title_color == Color.clear)
                title_color = Color.white * 0.9f;

            bool exp = false;
            if (XGUI.GetCurrentWindowWidth() > limite_width)
                exp = true;
            else
                exp = false;

            // 测试区域
            //XGUI.gui_box(rect, Color.red * 0.5f);

            if (!string.IsNullOrEmpty(title) && exp)
            {
                #region 标题
                Rect rect_title = new Rect(rect.x, rect.y, title_width, field_height);
                XGUI.gui_label(
                    rect: rect_title,
                    text: new GUIContent(title),
                    text_color: title_color,
                    size: title_size,
                    clipping: TextClipping.Overflow,
                    anchor: title_anchor,
                    offset: new Vector2(0, 0),
                    wrap: true,
                    font_style: FontStyle.Normal);
                #endregion
            }

            float field_x = exp ? (!string.IsNullOrEmpty(title) ? rect.x + title_width + 10 : rect.x) : rect.x;
            float field_y = rect.y;
            float field_w = exp ? (!string.IsNullOrEmpty(title) ? rect.width - (title_width + 10 + (status_icon != null ? 10 : 0)) : rect.width - (status_icon != null ? 10 : 0)) : rect.width - (status_icon != null ? 10 : 0);
            float field_h = XGUI.GetSingleLineHeight();
            Rect rect_field = new Rect(field_x, field_y, field_w, field_h);
            prop = EditorGUI.Vector4Field(rect_field, GUIContent.none, prop);

            if (status_icon != null)
            {
                float x = rect.x + rect.width;
                float y = rect.y + 6;
                float w = 5;
                float h = 5;

                Rect rect_icon = new Rect(x, y, w, h);

                XGUI.gui_icon(
                    rect: rect_icon,
                    icon: XGUI.GetBasedIcon(status_icon),
                    color: status_icon_color);
            }

            return prop;
        }
        /// <summary>
        /// 绘制一个带有标签的四元数（Quaternion）输入框。
        /// </summary>
        /// <param name="rect">输入框的位置和尺寸。</param>
        /// <param name="prop">输入框的当前 Quaternion 值。</param>
        /// <param name="title">输入框的标签文本。为 <c>null</c> 或空时不显示标签。</param>
        /// <param name="title_size">标签文本的字体大小。</param>
        /// <param name="title_anchor">标签文本的对齐方式。</param>
        /// <param name="title_color">标签文本的颜色。传入 <see cref="Color.clear"/> 时使用半透明白色。</param>
        /// <param name="title_width">标签的固定宽度。为 0 时自动计算宽度。</param>
        /// <param name="field_height">输入字段的高度。为 0 时使用默认行高。</param>
        /// <param name="status_icon">状态图标的名称。为 <c>null</c> 时不显示图标。</param>
        /// <param name="status_icon_color">状态图标的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="limite_width">显示标签的窗口宽度阈值。</param>
        /// <returns>用户输入后的 Quaternion 值。</returns>
        /// <remarks>
        /// 内部通过 <see cref="EditorGUI.Vector4Field"/> 实现，将四元数的 x、y、z、w 分量分别显示为四个输入字段。
        /// </remarks>
        public static Quaternion gui_inputfield(Rect rect = default, Quaternion prop = default, string title = null, XGUIFontSize title_size = XGUIFontSize.M, TextAnchor title_anchor = TextAnchor.MiddleLeft, Color title_color = default, float title_width = 0, float field_height = 20, string status_icon = null, Color status_icon_color = default, float limite_width = 230)
        {
            if (title_color == Color.clear)
                title_color = Color.white * 0.9f;

            bool exp = false;
            if (XGUI.GetCurrentWindowWidth() > limite_width)
                exp = true;
            else
                exp = false;

            // 测试区域
            //XGUI.gui_box(rect, Color.red * 0.5f);

            if (!string.IsNullOrEmpty(title) && exp)
            {
                #region 标题
                Rect rect_title = new Rect(rect.x, rect.y, title_width, field_height);
                XGUI.gui_label(
                    rect: rect_title,
                    text: new GUIContent(title),
                    text_color: title_color,
                    size: title_size,
                    clipping: TextClipping.Overflow,
                    anchor: title_anchor,
                    offset: new Vector2(0, 0),
                    wrap: true,
                    font_style: FontStyle.Normal);
                #endregion
            }

            float field_x = exp ? (!string.IsNullOrEmpty(title) ? rect.x + title_width + 10 : rect.x) : rect.x;
            float field_y = rect.y;
            float field_w = exp ? (!string.IsNullOrEmpty(title) ? rect.width - (title_width + 10 + (status_icon != null ? 10 : 0)) : rect.width - (status_icon != null ? 10 : 0)) : rect.width - (status_icon != null ? 10 : 0);
            float field_h = XGUI.GetSingleLineHeight();
            Rect rect_field = new Rect(field_x, field_y, field_w, field_h);

            Vector4 val = new Vector4();
            val.Set(prop.x, prop.y, prop.z, prop.w);

            val = EditorGUI.Vector4Field(rect_field, GUIContent.none, val);

            prop.Set(val.x, val.y, val.z, val.w);

            if (status_icon != null)
            {
                float x = rect.x + rect.width;
                float y = rect.y + 6;
                float w = 5;
                float h = 5;

                Rect rect_icon = new Rect(x, y, w, h);

                XGUI.gui_icon(
                    rect: rect_icon,
                    icon: XGUI.GetBasedIcon(status_icon),
                    color: status_icon_color);
            }

            return prop;
        }
        /// <summary>
        /// 绘制一个带有标签的颜色选择器输入框。
        /// </summary>
        /// <param name="rect">输入框的位置和尺寸。</param>
        /// <param name="prop">输入框的当前 Color 值。</param>
        /// <param name="title">输入框的标签文本。为 <c>null</c> 或空时不显示标签。</param>
        /// <param name="title_size">标签文本的字体大小。</param>
        /// <param name="title_anchor">标签文本的对齐方式。</param>
        /// <param name="title_color">标签文本的颜色。传入 <see cref="Color.clear"/> 时使用半透明白色。</param>
        /// <param name="title_width">标签的固定宽度。为 0 时自动计算宽度。</param>
        /// <param name="field_height">输入字段的高度。为 0 时使用默认行高。</param>
        /// <param name="status_icon">状态图标的名称。为 <c>null</c> 时不显示图标。</param>
        /// <param name="status_icon_color">状态图标的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="limite_width">显示标签的窗口宽度阈值。</param>
        /// <returns>用户选择后的 Color 值。</returns>
        /// <remarks>
        /// 使用 <see cref="EditorGUI.ColorField"/> 实现，支持 RGBA 各通道的独立编辑和取色器拾取。
        /// </remarks>
        public static Color gui_inputfield(Rect rect = default, Color prop = default, string title = null, XGUIFontSize title_size = XGUIFontSize.M, TextAnchor title_anchor = TextAnchor.MiddleLeft, Color title_color = default, float title_width = 0, float field_height = 20, string status_icon = null, Color status_icon_color = default, float limite_width = 230, bool show_eyedropper = true, bool show_alpha = true, bool hdr = true)
        {
            if (title_color == Color.clear)
                title_color = Color.white * 0.9f;

            bool exp = false;
            if (XGUI.GetCurrentWindowWidth() > limite_width)
                exp = true;
            else
                exp = false;

            // 测试区域
            //XGUI.gui_box(rect, Color.red * 0.5f);

            if (!string.IsNullOrEmpty(title) && exp)
            {
                #region 标题
                Rect rect_title = new Rect(rect.x, rect.y, title_width, field_height);
                XGUI.gui_label(
                    rect: rect_title,
                    text: new GUIContent(title),
                    text_color: title_color,
                    size: title_size,
                    clipping: TextClipping.Overflow,
                    anchor: title_anchor,
                    offset: new Vector2(0, 0),
                    wrap: true,
                    font_style: FontStyle.Normal);
                #endregion
            }

            float field_x = exp ? (!string.IsNullOrEmpty(title) ? rect.x + title_width + 10 : rect.x) : rect.x;
            float field_y = rect.y;
            float field_w = exp ? (!string.IsNullOrEmpty(title) ? rect.width - (title_width + 10 + (status_icon != null ? 10 : 0)) : rect.width - (status_icon != null ? 10 : 0)) : rect.width - (status_icon != null ? 10 : 0);
            float field_h = XGUI.GetSingleLineHeight();
            Rect rect_field = new Rect(field_x, field_y, field_w, field_h);
            prop = EditorGUI.ColorField(rect_field, GUIContent.none, prop, show_eyedropper, show_alpha, hdr);

            if (status_icon != null)
            {
                float x = rect.x + rect.width;
                float y = rect.y + 6;
                float w = 5;
                float h = 5;

                Rect rect_icon = new Rect(x, y, w, h);

                XGUI.gui_icon(
                    rect: rect_icon,
                    icon: XGUI.GetBasedIcon(status_icon),
                    color: status_icon_color);
            }

            return prop;
        }
        /// <summary>
        /// 绘制一个带有标签的动画曲线编辑输入框。
        /// </summary>
        /// <param name="rect">输入框的位置和尺寸。</param>
        /// <param name="prop">输入框的当前 AnimationCurve 值。</param>
        /// <param name="title">输入框的标签文本。为 <c>null</c> 或空时不显示标签。</param>
        /// <param name="title_size">标签文本的字体大小。</param>
        /// <param name="title_anchor">标签文本的对齐方式。</param>
        /// <param name="title_color">标签文本的颜色。传入 <see cref="Color.clear"/> 时使用半透明白色。</param>
        /// <param name="title_width">标签的固定宽度。为 0 时自动计算宽度。</param>
        /// <param name="field_height">输入字段的高度。为 0 时使用默认行高。</param>
        /// <param name="status_icon">状态图标的名称。为 <c>null</c> 时不显示图标。</param>
        /// <param name="status_icon_color">状态图标的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="limite_width">显示标签的窗口宽度阈值。</param>
        /// <returns>用户编辑后的 AnimationCurve 值。</returns>
        /// <remarks>
        /// 使用 <see cref="EditorGUI.CurveField"/> 实现，提供可视化的曲线编辑界面，
        /// 支持添加、删除和拖拽关键帧。
        /// </remarks>
        public static AnimationCurve gui_inputfield(Rect rect = default, AnimationCurve prop = default, string title = null, XGUIFontSize title_size = XGUIFontSize.M, TextAnchor title_anchor = TextAnchor.MiddleLeft, Color title_color = default, float title_width = 0, float field_height = 20, string status_icon = null, Color status_icon_color = default, float limite_width = 230)
        {
            if (title_color == Color.clear)
                title_color = Color.white * 0.9f;

            bool exp = false;
            if (XGUI.GetCurrentWindowWidth() > limite_width)
                exp = true;
            else
                exp = false;

            // 测试区域
            //XGUI.gui_box(rect, Color.red * 0.5f);

            if (!string.IsNullOrEmpty(title) && exp)
            {
                #region 标题
                Rect rect_title = new Rect(rect.x, rect.y, title_width, field_height);
                XGUI.gui_label(
                    rect: rect_title,
                    text: new GUIContent(title),
                    text_color: title_color,
                    size: title_size,
                    clipping: TextClipping.Overflow,
                    anchor: title_anchor,
                    offset: new Vector2(0, 0),
                    wrap: true,
                    font_style: FontStyle.Normal);
                #endregion
            }

            float field_x = exp ? (!string.IsNullOrEmpty(title) ? rect.x + title_width + 10 : rect.x) : rect.x;
            float field_y = rect.y;
            float field_w = exp ? (!string.IsNullOrEmpty(title) ? rect.width - (title_width + 10 + (status_icon != null ? 10 : 0)) : rect.width - (status_icon != null ? 10 : 0)) : rect.width - (status_icon != null ? 10 : 0);
            float field_h = XGUI.GetSingleLineHeight();
            Rect rect_field = new Rect(field_x, field_y, field_w, field_h);
            prop = EditorGUI.CurveField(rect_field, GUIContent.none, prop);

            if (status_icon != null)
            {
                float x = rect.x + rect.width;
                float y = rect.y + 6;
                float w = 5;
                float h = 5;

                Rect rect_icon = new Rect(x, y, w, h);

                XGUI.gui_icon(
                    rect: rect_icon,
                    icon: XGUI.GetBasedIcon(status_icon),
                    color: status_icon_color);
            }

            return prop;
        }
    }
}