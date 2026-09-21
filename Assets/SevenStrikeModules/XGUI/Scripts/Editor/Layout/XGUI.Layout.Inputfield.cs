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
        /// 使用自动布局绘制一个带有标签的文本输入框。
        /// </summary>
        /// <param name="prop">输入框的当前文本内容（引用传递，直接修改）。</param>
        /// <param name="title">输入框的标签文本。为 <c>null</c> 或空时不显示标签。</param>
        /// <param name="title_size">标签文本的字体大小。</param>
        /// <param name="title_anchor">标签文本的对齐方式。</param>
        /// <param name="title_color">标签文本的颜色。传入 <see cref="Color.clear"/> 时使用默认颜色。</param>
        /// <param name="title_width">标签的固定宽度。为 0 时自动计算宽度。</param>
        /// <param name="text_wrap">是否启用文本换行。</param>
        /// <param name="field_fontsize">输入字段的字体大小。</param>
        /// <param name="field_text_offset">输入字段文本的偏移量。</param>
        /// <param name="field_height">输入字段的高度。为 0 时使用默认行高。</param>
        /// <param name="field_padding">输入字段的内边距，覆盖样式默认值。</param>
        /// <param name="field_margin">输入字段的外边距，覆盖样式默认值。</param>
        /// <param name="field_text_color">输入字段文本的颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="field_text_font">输入字段的字体。为 <c>null</c> 时使用样式默认字体。</param>
        /// <param name="field_text_style">输入字段的字体样式。</param>
        /// <param name="field_text_anchor">输入字段文本的对齐方式。</param>
        /// <param name="status_icon">状态图标的名称。为 <c>null</c> 时不显示图标。</param>
        /// <param name="status_icon_color">状态图标的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <returns>用户输入后的文本内容。</returns>
        /// <remarks>
        /// 该方法与 <see cref="gui_inputfield(Rect, string, string, XGUIFontSize, TextAnchor, Color, float, bool, XGUIFontSize, Vector2, float, Color, Font, FontStyle, TextAnchor, string, Color, float, RectOffset, RectOffset)"/> 
        /// 功能相同，但使用 <see cref="EditorGUILayout"/> 自动布局系统，无需手动指定矩形位置。
        /// </remarks>
        public static string layout_inputfield(string prop = null, string title = null, XGUIFontSize title_size = XGUIFontSize.M, TextAnchor title_anchor = TextAnchor.MiddleLeft, Color title_color = default, float title_width = 0, bool text_wrap = true, XGUIFontSize field_fontsize = XGUIFontSize.M, Vector2 field_text_offset = default, float field_height = 20, RectOffset field_padding = default, RectOffset field_margin = default, Color field_text_color = default, Font field_text_font = null, FontStyle field_text_style = FontStyle.Normal, TextAnchor field_text_anchor = TextAnchor.UpperLeft, string status_icon = null, Color status_icon_color = default, float field_width = 0)
        {
            if (field_padding == null)
                field_padding = new RectOffset(0, 0, 0, 0);

            if (field_margin == null)
                field_margin = new RectOffset(0, 0, 0, 0);

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

            if (field_text_font != null)
                style.font = field_text_font;

            XGUI.layout_group_start(
                type: XGUIContainerType.Horizontal,
                absolute_margin: true,
                margin: field_margin,
                group_width: field_width,
                padding: field_padding);

            if (!string.IsNullOrEmpty(title))
            {
                #region 标题
                XGUI.layout_label(
                    text: title,
                    size: title_size,
                    text_color: title_color,
                    margin: new RectOffset(0, 0, 0, 0),
                    offset: new Vector2(0, 1),
                    clipping: TextClipping.Clip,
                    font_style: FontStyle.Normal,
                    width: title_width,
                    anchor: title_anchor,
                    font: XGUI.GetFont("xg-medium"));
                #endregion
            }

            prop = EditorGUILayout.TextField(prop, style, GUILayout.Height(field_height), GUILayout.MinWidth(0));

            if (status_icon != null)
            {
                XGUI.layout_icon(
                    icon: XGUI.GetBasedIcon(status_icon),
                    width: 5,
                    height: 5,
                    icon_offset: new Vector2(0, 9),
                    icon_color: status_icon_color,
                    icon_margin: new RectOffset(10, 0, 0, 0),
                    icon_alignment: XGUIIconAlignment.默认);
            }

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);

            return prop;
        }
        /// <summary>
        /// 使用自动布局绘制一个带有标签的浮点数输入框。
        /// </summary>
        /// <param name="prop">输入框的当前浮点数值。</param>
        /// <param name="title">输入框的标签文本。为 <c>null</c> 或空时不显示标签。</param>
        /// <param name="title_size">标签文本的字体大小。</param>
        /// <param name="title_anchor">标签文本的对齐方式。</param>
        /// <param name="title_color">标签文本的颜色。传入 <see cref="Color.clear"/> 时使用半透明白色。</param>
        /// <param name="title_width">标签的固定宽度。为 0 时自动计算宽度。</param>
        /// <param name="field_fontsize">输入字段的字体大小。</param>
        /// <param name="field_text_offset">输入字段文本的偏移量。</param>
        /// <param name="field_height">输入字段的高度。为 0 时使用默认行高。</param>
        /// <param name="field_padding">输入字段的内边距，覆盖样式默认值。</param>
        /// <param name="field_margin">输入字段的外边距，覆盖样式默认值。</param>
        /// <param name="field_text_color">输入字段文本的颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="field_text_font">输入字段的字体。为 <c>null</c> 时使用样式默认字体。</param>
        /// <param name="field_text_style">输入字段的字体样式。</param>
        /// <param name="field_text_anchor">输入字段文本的对齐方式。</param>
        /// <param name="status_icon">状态图标的名称。为 <c>null</c> 时不显示图标。</param>
        /// <param name="status_icon_color">状态图标的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="decimals">浮点数保留的小数位数，取值范围 0-6。</param>
        /// <returns>用户输入后的浮点数值，并自动舍入到指定小数位数。</returns>
        public static float layout_inputfield(float prop = 0, string title = null, XGUIFontSize title_size = XGUIFontSize.M, TextAnchor title_anchor = TextAnchor.MiddleLeft, Color title_color = default, float title_width = 0, XGUIFontSize field_fontsize = XGUIFontSize.M, Vector2 field_text_offset = default, float field_height = 20, RectOffset field_padding = default, RectOffset field_margin = default, Color field_text_color = default, Font field_text_font = null, FontStyle field_text_style = FontStyle.Normal, TextAnchor field_text_anchor = TextAnchor.UpperLeft, string status_icon = null, Color status_icon_color = default, int decimals = 2, float field_width = 0)
        {
            if (field_padding == null)
                field_padding = new RectOffset(0, 0, 0, 0);

            if (field_margin == null)
                field_margin = new RectOffset(0, 0, 0, 0);

            if (field_text_color == Color.clear)
                field_text_color = Color.white;

            if (title_color == Color.clear)
                title_color = Color.white * 0.9f;

            GUIStyle style = new GUIStyle(style_xg_input);
            style.fontSize = XGUI.GetFontSize(field_fontsize);

            style.normal.textColor = field_text_color;
            style.focused.textColor = field_text_color;
            style.active.textColor = field_text_color;
            style.fontStyle = field_text_style;
            style.contentOffset = field_text_offset;
            style.alignment = field_text_anchor;

            if (field_text_font != null)
                style.font = field_text_font;

            XGUI.layout_group_start(
                type: XGUIContainerType.Horizontal,
                absolute_margin: true,
                group_width: field_width,
                margin: field_margin,
                padding: field_padding);

            if (!string.IsNullOrEmpty(title))
            {
                #region 标题
                XGUI.layout_label(
                    text: title,
                    size: title_size,
                    text_color: title_color,
                    margin: new RectOffset(0, 0, 0, 0),
                    offset: new Vector2(0, 1),
                    clipping: TextClipping.Clip,
                    font_style: FontStyle.Normal,
                    width: title_width,
                    anchor: title_anchor,
                    font: XGUI.GetFont("xg-medium"));
                #endregion
            }

            prop = EditorGUILayout.FloatField(prop, style, GUILayout.Height(field_height), GUILayout.MinWidth(0));

            if (status_icon != null)
            {
                XGUI.layout_icon(
                    icon: XGUI.GetBasedIcon(status_icon),
                    width: 5,
                    height: 5,
                    icon_offset: new Vector2(0, 9),
                    icon_color: status_icon_color,
                    icon_margin: new RectOffset(10, 0, 0, 0),
                    icon_alignment: XGUIIconAlignment.默认);
            }

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);

            // 舍入保护
            decimals = Mathf.Clamp(decimals, 0, 6);

            return (float)Math.Round(prop, decimals, MidpointRounding.AwayFromZero);
        }
        /// <summary>
        /// 使用自动布局绘制一个带有标签的整数输入框。
        /// </summary>
        /// <param name="prop">输入框的当前整数值。</param>
        /// <param name="title">输入框的标签文本。为 <c>null</c> 或空时不显示标签。</param>
        /// <param name="title_size">标签文本的字体大小。</param>
        /// <param name="title_anchor">标签文本的对齐方式。</param>
        /// <param name="title_color">标签文本的颜色。传入 <see cref="Color.clear"/> 时使用半透明白色。</param>
        /// <param name="title_width">标签的固定宽度。为 0 时自动计算宽度。</param>
        /// <param name="field_fontsize">输入字段的字体大小。</param>
        /// <param name="field_text_offset">输入字段文本的偏移量。</param>
        /// <param name="field_height">输入字段的高度。为 0 时使用默认行高。</param>
        /// <param name="field_padding">输入字段的内边距，覆盖样式默认值。</param>
        /// <param name="field_margin">输入字段的外边距，覆盖样式默认值。</param>
        /// <param name="field_text_color">输入字段文本的颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="field_text_font">输入字段的字体。为 <c>null</c> 时使用样式默认字体。</param>
        /// <param name="field_text_style">输入字段的字体样式。</param>
        /// <param name="field_text_anchor">输入字段文本的对齐方式。</param>
        /// <param name="status_icon">状态图标的名称。为 <c>null</c> 时不显示图标。</param>
        /// <param name="status_icon_color">状态图标的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <returns>用户输入后的整数值。</returns>
        public static int layout_inputfield(int prop = 0, string title = null, XGUIFontSize title_size = XGUIFontSize.M, TextAnchor title_anchor = TextAnchor.MiddleLeft, Color title_color = default, float title_width = 0, XGUIFontSize field_fontsize = XGUIFontSize.M, Vector2 field_text_offset = default, float field_height = 20, RectOffset field_padding = default, RectOffset field_margin = default, Color field_text_color = default, Font field_text_font = null, FontStyle field_text_style = FontStyle.Normal, TextAnchor field_text_anchor = TextAnchor.UpperLeft, string status_icon = null, Color status_icon_color = default, float field_width = 0)
        {
            if (field_padding == null)
                field_padding = new RectOffset(0, 0, 0, 0);

            if (field_margin == null)
                field_margin = new RectOffset(0, 0, 0, 0);

            if (field_text_color == Color.clear)
                field_text_color = Color.white;

            if (title_color == Color.clear)
                title_color = Color.white * 0.9f;

            GUIStyle style = new GUIStyle(style_xg_input);
            style.fontSize = XGUI.GetFontSize(field_fontsize);

            style.normal.textColor = field_text_color;
            style.focused.textColor = field_text_color;
            style.active.textColor = field_text_color;
            style.fontStyle = field_text_style;
            style.contentOffset = field_text_offset;
            style.alignment = field_text_anchor;

            if (field_text_font != null)
                style.font = field_text_font;

            XGUI.layout_group_start(
                type: XGUIContainerType.Horizontal,
                absolute_margin: true,
                group_width: field_width,
                margin: field_margin,
                padding: field_padding);

            if (!string.IsNullOrEmpty(title))
            {
                #region 标题
                XGUI.layout_label(
                    text: title,
                    size: title_size,
                    text_color: title_color,
                    margin: new RectOffset(0, 0, 0, 0),
                    offset: new Vector2(0, 1),
                    clipping: TextClipping.Clip,
                    font_style: FontStyle.Normal,
                    width: title_width,
                    anchor: title_anchor,
                    font: XGUI.GetFont("xg-medium"));
                #endregion
            }

            prop = EditorGUILayout.IntField(prop, style, GUILayout.Height(field_height), GUILayout.MinWidth(0));

            if (status_icon != null)
            {
                XGUI.layout_icon(
                    icon: XGUI.GetBasedIcon(status_icon),
                    width: 5,
                    height: 5,
                    icon_offset: new Vector2(0, 9),
                    icon_color: status_icon_color,
                    icon_margin: new RectOffset(10, 0, 0, 0),
                    icon_alignment: XGUIIconAlignment.默认);
            }

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);

            return prop;
        }
        /// <summary>
        /// 使用自动布局绘制一个带有标签的二维向量（Vector2）输入框。
        /// </summary>
        /// <param name="prop">输入框的当前 Vector2 值。</param>
        /// <param name="title">输入框的标签文本。为 <c>null</c> 或空时不显示标签。</param>
        /// <param name="title_size">标签文本的字体大小。</param>
        /// <param name="title_anchor">标签文本的对齐方式。</param>
        /// <param name="title_color">标签文本的颜色。传入 <see cref="Color.clear"/> 时使用默认颜色。</param>
        /// <param name="title_width">标签的固定宽度。为 0 时自动计算宽度。</param>
        /// <param name="field_height">输入字段的高度。为 0 时使用默认行高。</param>
        /// <param name="field_padding">输入字段的内边距，覆盖样式默认值。</param>
        /// <param name="field_margin">输入字段的外边距，覆盖样式默认值。</param>
        /// <param name="status_icon">状态图标的名称。为 <c>null</c> 时不显示图标。</param>
        /// <param name="status_icon_color">状态图标的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <returns>用户输入后的 Vector2 值。</returns>
        public static Vector2 layout_inputfield(Vector2 prop = default, string title = null, XGUIFontSize title_size = XGUIFontSize.M, TextAnchor title_anchor = TextAnchor.MiddleLeft, Color title_color = default, float title_width = 0, float field_height = 20, RectOffset field_padding = default, RectOffset field_margin = default, string status_icon = null, Color status_icon_color = default, float field_width = 0)
        {
            if (field_padding == null)
                field_padding = new RectOffset(0, 0, 0, 0);

            if (field_margin == null)
                field_margin = new RectOffset(0, 0, 0, 0);

            XGUI.layout_group_start(
                type: XGUIContainerType.Horizontal,
                absolute_margin: true,
                group_width: field_width,
                margin: field_margin,
                padding: field_padding);

            if (!string.IsNullOrEmpty(title))
            {
                #region 标题
                XGUI.layout_label(
                    text: title,
                    size: title_size,
                    text_color: title_color,
                    margin: new RectOffset(0, 0, 0, 0),
                    offset: new Vector2(0, 1),
                    clipping: TextClipping.Clip,
                    font_style: FontStyle.Normal,
                    width: title_width,
                    anchor: title_anchor,
                    font: XGUI.GetFont("xg-medium"));
                #endregion
            }

            prop = EditorGUILayout.Vector2Field(GUIContent.none, prop, GUILayout.Height(field_height), GUILayout.MinWidth(0));

            if (status_icon != null)
            {
                XGUI.layout_icon(
                    icon: XGUI.GetBasedIcon(status_icon),
                    width: 5,
                    height: 5,
                    icon_offset: new Vector2(0, 9),
                    icon_color: status_icon_color,
                    icon_margin: new RectOffset(10, 0, 0, 0),
                    icon_alignment: XGUIIconAlignment.默认);
            }

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);

            return prop;
        }
        /// <summary>
        /// 使用自动布局绘制一个带有标签的三维向量（Vector3）输入框。
        /// </summary>
        /// <param name="prop">输入框的当前 Vector3 值。</param>
        /// <param name="title">输入框的标签文本。为 <c>null</c> 或空时不显示标签。</param>
        /// <param name="title_size">标签文本的字体大小。</param>
        /// <param name="title_anchor">标签文本的对齐方式。</param>
        /// <param name="title_color">标签文本的颜色。传入 <see cref="Color.clear"/> 时使用默认颜色。</param>
        /// <param name="title_width">标签的固定宽度。为 0 时自动计算宽度。</param>
        /// <param name="field_height">输入字段的高度。为 0 时使用默认行高。</param>
        /// <param name="field_padding">输入字段的内边距，覆盖样式默认值。</param>
        /// <param name="field_margin">输入字段的外边距，覆盖样式默认值。</param>
        /// <param name="status_icon">状态图标的名称。为 <c>null</c> 时不显示图标。</param>
        /// <param name="status_icon_color">状态图标的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <returns>用户输入后的 Vector3 值。</returns>
        public static Vector3 layout_inputfield(Vector3 prop = default, string title = null, XGUIFontSize title_size = XGUIFontSize.M, TextAnchor title_anchor = TextAnchor.MiddleLeft, Color title_color = default, float title_width = 0, float field_height = 20, RectOffset field_padding = default, RectOffset field_margin = default, string status_icon = null, Color status_icon_color = default, float field_width = 0)
        {
            if (field_padding == null)
                field_padding = new RectOffset(0, 0, 0, 0);

            if (field_margin == null)
                field_margin = new RectOffset(0, 0, 0, 0);

            XGUI.layout_group_start(
                type: XGUIContainerType.Horizontal,
                absolute_margin: true,
                group_width: field_width,
                margin: field_margin,
                padding: field_padding);

            if (!string.IsNullOrEmpty(title))
            {
                #region 标题
                XGUI.layout_label(
                    text: title,
                    size: title_size,
                    text_color: title_color,
                    margin: new RectOffset(0, 0, 0, 0),
                    offset: new Vector2(0, 1),
                    clipping: TextClipping.Clip,
                    font_style: FontStyle.Normal,
                    width: title_width,
                    anchor: title_anchor,
                    font: XGUI.GetFont("xg-medium"));
                #endregion
            }

            prop = EditorGUILayout.Vector3Field(GUIContent.none, prop, GUILayout.Height(field_height), GUILayout.MinWidth(0));

            if (status_icon != null)
            {
                XGUI.layout_icon(
                    icon: XGUI.GetBasedIcon(status_icon),
                    width: 5,
                    height: 5,
                    icon_offset: new Vector2(0, 9),
                    icon_color: status_icon_color,
                    icon_margin: new RectOffset(10, 0, 0, 0),
                    icon_alignment: XGUIIconAlignment.默认);
            }

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);

            return prop;
        }
        /// <summary>
        /// 使用自动布局绘制一个带有标签的四维向量（Vector4）输入框。
        /// </summary>
        /// <param name="prop">输入框的当前 Vector4 值。</param>
        /// <param name="title">输入框的标签文本。为 <c>null</c> 或空时不显示标签。</param>
        /// <param name="title_size">标签文本的字体大小。</param>
        /// <param name="title_anchor">标签文本的对齐方式。</param>
        /// <param name="title_color">标签文本的颜色。传入 <see cref="Color.clear"/> 时使用默认颜色。</param>
        /// <param name="title_width">标签的固定宽度。为 0 时自动计算宽度。</param>
        /// <param name="field_height">输入字段的高度。为 0 时使用默认行高。</param>
        /// <param name="field_padding">输入字段的内边距，覆盖样式默认值。</param>
        /// <param name="field_margin">输入字段的外边距，覆盖样式默认值。</param>
        /// <param name="status_icon">状态图标的名称。为 <c>null</c> 时不显示图标。</param>
        /// <param name="status_icon_color">状态图标的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <returns>用户输入后的 Vector4 值。</returns>
        public static Vector4 layout_inputfield(Vector4 prop = default, string title = null, XGUIFontSize title_size = XGUIFontSize.M, TextAnchor title_anchor = TextAnchor.MiddleLeft, Color title_color = default, float title_width = 0, float field_height = 20, RectOffset field_padding = default, RectOffset field_margin = default, string status_icon = null, Color status_icon_color = default, float field_width = 0)
        {
            if (field_padding == null)
                field_padding = new RectOffset(0, 0, 0, 0);

            if (field_margin == null)
                field_margin = new RectOffset(0, 0, 0, 0);

            XGUI.layout_group_start(
                type: XGUIContainerType.Horizontal,
                absolute_margin: true,
                group_width: field_width,
                margin: field_margin,
                padding: field_padding);

            if (!string.IsNullOrEmpty(title))
            {
                #region 标题
                XGUI.layout_label(
                    text: title,
                    size: title_size,
                    text_color: title_color,
                    margin: new RectOffset(0, 0, 0, 0),
                    offset: new Vector2(0, 1),
                    clipping: TextClipping.Clip,
                    font_style: FontStyle.Normal,
                    width: title_width,
                    anchor: title_anchor,
                    font: XGUI.GetFont("xg-medium"));
                #endregion
            }

            prop = EditorGUILayout.Vector4Field(GUIContent.none, prop, GUILayout.Height(field_height), GUILayout.MinWidth(0));

            if (status_icon != null)
            {
                XGUI.layout_icon(
                    icon: XGUI.GetBasedIcon(status_icon),
                    width: 5,
                    height: 5,
                    icon_offset: new Vector2(0, 9),
                    icon_color: status_icon_color,
                    icon_margin: new RectOffset(10, 0, 0, 0),
                    icon_alignment: XGUIIconAlignment.默认);
            }

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);

            return prop;
        }
        /// <summary>
        /// 使用自动布局绘制一个带有标签的颜色选择器输入框。
        /// </summary>
        /// <param name="prop">输入框的当前 Color 值。</param>
        /// <param name="title">输入框的标签文本。为 <c>null</c> 或空时不显示标签。</param>
        /// <param name="title_size">标签文本的字体大小。</param>
        /// <param name="title_anchor">标签文本的对齐方式。</param>
        /// <param name="title_color">标签文本的颜色。传入 <see cref="Color.clear"/> 时使用默认颜色。</param>
        /// <param name="title_width">标签的固定宽度。为 0 时自动计算宽度。</param>
        /// <param name="field_height">输入字段的高度。为 0 时使用默认行高。</param>
        /// <param name="field_padding">输入字段的内边距，覆盖样式默认值。</param>
        /// <param name="field_margin">输入字段的外边距，覆盖样式默认值。</param>
        /// <param name="status_icon">状态图标的名称。为 <c>null</c> 时不显示图标。</param>
        /// <param name="status_icon_color">状态图标的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <returns>用户选择后的 Color 值。</returns>
        public static Color layout_inputfield(Color prop = default, string title = null, XGUIFontSize title_size = XGUIFontSize.M, TextAnchor title_anchor = TextAnchor.MiddleLeft, Color title_color = default, float title_width = 0, float field_height = 20, RectOffset field_padding = default, RectOffset field_margin = default, string status_icon = null, Color status_icon_color = default, float field_width = 0)
        {
            if (field_padding == null)
                field_padding = new RectOffset(0, 0, 0, 0);

            if (field_margin == null)
                field_margin = new RectOffset(0, 0, 0, 0);

            XGUI.layout_group_start(
                type: XGUIContainerType.Horizontal,
                absolute_margin: true,
                group_width: field_width,
                margin: field_margin,
                padding: field_padding);

            if (!string.IsNullOrEmpty(title))
            {
                #region 标题
                XGUI.layout_label(
                    text: title,
                    size: title_size,
                    text_color: title_color,
                    margin: new RectOffset(0, 0, 0, 0),
                    offset: new Vector2(0, 1),
                    clipping: TextClipping.Clip,
                    font_style: FontStyle.Normal,
                    width: title_width,
                    anchor: title_anchor,
                    font: XGUI.GetFont("xg-medium"));
                #endregion
            }
            XGUI.layout_space(1);

            prop = EditorGUILayout.ColorField(GUIContent.none, prop, GUILayout.Height(field_height), GUILayout.MinWidth(0));

            if (status_icon != null)
            {
                XGUI.layout_icon(
                    icon: XGUI.GetBasedIcon(status_icon),
                    width: 5,
                    height: 5,
                    icon_offset: new Vector2(0, 9),
                    icon_color: status_icon_color,
                    icon_margin: new RectOffset(10, 0, 0, 0),
                    icon_alignment: XGUIIconAlignment.默认);
            }

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);

            return prop;
        }
        /// <summary>
        /// 使用自动布局绘制一个带有标签的动画曲线编辑输入框。
        /// </summary>
        /// <param name="prop">输入框的当前 AnimationCurve 值。</param>
        /// <param name="title">输入框的标签文本。为 <c>null</c> 或空时不显示标签。</param>
        /// <param name="title_size">标签文本的字体大小。</param>
        /// <param name="title_anchor">标签文本的对齐方式。</param>
        /// <param name="title_color">标签文本的颜色。传入 <see cref="Color.clear"/> 时使用默认颜色。</param>
        /// <param name="title_width">标签的固定宽度。为 0 时自动计算宽度。</param>
        /// <param name="field_height">输入字段的高度。为 0 时使用默认行高。</param>
        /// <param name="field_padding">输入字段的内边距，覆盖样式默认值。</param>
        /// <param name="field_margin">输入字段的外边距，覆盖样式默认值。</param>
        /// <param name="status_icon">状态图标的名称。为 <c>null</c> 时不显示图标。</param>
        /// <param name="status_icon_color">状态图标的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <returns>用户编辑后的 AnimationCurve 值。</returns>
        public static AnimationCurve layout_inputfield(AnimationCurve prop = default, string title = null, XGUIFontSize title_size = XGUIFontSize.M, TextAnchor title_anchor = TextAnchor.MiddleLeft, Color title_color = default, float title_width = 0, float field_height = 20, RectOffset field_padding = default, RectOffset field_margin = default, string status_icon = null, Color status_icon_color = default, float field_width = 0)
        {
            if (field_padding == null)
                field_padding = new RectOffset(0, 0, 0, 0);

            if (field_margin == null)
                field_margin = new RectOffset(0, 0, 0, 0);

            XGUI.layout_group_start(
                type: XGUIContainerType.Horizontal,
                absolute_margin: true,
                group_width: field_width,
                margin: field_margin,
                padding: field_padding);

            if (!string.IsNullOrEmpty(title))
            {
                #region 标题
                XGUI.layout_label(
                    text: title,
                    size: title_size,
                    text_color: title_color,
                    margin: new RectOffset(0, 0, 0, 0),
                    offset: new Vector2(0, 1),
                    clipping: TextClipping.Clip,
                    font_style: FontStyle.Normal,
                    width: title_width,
                    anchor: title_anchor,
                    font: XGUI.GetFont("xg-medium"));
                #endregion
            }

            prop = EditorGUILayout.CurveField(GUIContent.none, prop, GUILayout.Height(field_height), GUILayout.MinWidth(0));

            if (status_icon != null)
            {
                XGUI.layout_icon(
                    icon: XGUI.GetBasedIcon(status_icon),
                    width: 5,
                    height: 5,
                    icon_offset: new Vector2(0, 9),
                    icon_color: status_icon_color,
                    icon_margin: new RectOffset(10, 0, 0, 0),
                    icon_alignment: XGUIIconAlignment.默认);
            }

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);

            return prop;
        }
    }
}