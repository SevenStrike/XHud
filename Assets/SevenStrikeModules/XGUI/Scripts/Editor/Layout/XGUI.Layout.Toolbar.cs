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
    using UnityEngine;

    /// <summary>
    /// 为 Editor 界面提供样式控件
    /// </summary>
    public static partial class XGUI
    {
        /// <summary>
        /// 使用自动布局绘制一个自定义样式的工具栏（Toolbar），用于在多个选项间切换选择。
        /// </summary>
        /// <param name="index">当前选中项的索引（引用传递，会自动更新）。</param>
        /// <param name="names">工具栏按钮的显示名称数组。</param>
        /// <param name="bg_normal">未选中按钮的背景填充样式。</param>
        /// <param name="bg_selected">选中按钮的背景填充样式。</param>
        /// <param name="bg_color">按钮背景的颜色主题。</param>
        /// <param name="bg_gui_color">按钮背景的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="text_color_normal">未选中按钮的文本颜色。传入 <see cref="Color.clear"/> 时使用黑色。</param>
        /// <param name="text_color_selected">选中按钮的文本颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="bar_height">工具栏的高度。</param>
        /// <param name="text_anchor">按钮文本的对齐方式。</param>
        /// <param name="text_padding">按钮文本的内边距。</param>
        /// <param name="bar_margin">工具栏的外边距。</param>
        /// <param name="text_offset">按钮文本的偏移量。</param>
        /// <param name="text_font">按钮文本的字体。为 <c>null</c> 时使用样式默认字体。</param>
        /// <param name="text_fontstyle">按钮文本的字体样式。</param>
        /// <param name="navigate_style">是否使用导航样式。为 <c>true</c> 时移除按钮背景，仅显示带边框的容器。</param>
        /// <param name="navigate_style_bg">导航样式下的容器边框样式。</param>
        /// <param name="navigate_style_bg_color">导航样式下容器边框的颜色。传入 <see cref="Color.clear"/> 时使用默认颜色。</param>
        /// <returns>更新后的选中项索引。</returns>
        /// <remarks>
        /// 该方法与 <see cref="gui_toolbar"/> 功能相同，但使用 <see cref="GUILayout"/> 自动布局系统。
        /// 通过 <see cref="GUILayout.Toolbar"/> 绘制工具栏，无需手动指定矩形位置。
        /// <para>
        /// 支持两种样式模式：
        /// <list type="bullet">
        /// <item><description>标准模式：每个按钮有独立的背景填充</description></item>
        /// <item><description>导航模式：整个工具栏显示为带边框的容器，按钮无背景（适合面包屑导航）</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        public static int layout_toolbar(ref int index, string[] names, XGUIFilled bg_normal = XGUIFilled.实体, XGUIFilled bg_selected = XGUIFilled.实体, XGUIColor bg_color = XGUIColor.亮白, Color bg_gui_color = default, Color text_color_normal = default, Color text_color_selected = default, float bar_height = 25, Vector2 bar_offset = default, TextAnchor text_anchor = TextAnchor.MiddleCenter, RectOffset text_padding = null, RectOffset bar_margin = null, Vector2 text_offset = default, Font text_font = null, FontStyle text_fontstyle = FontStyle.Normal, XGUIFontSize text_size = XGUIFontSize.M, bool navigate_style = false, XGUIFilled navigate_style_bg = XGUIFilled.纯色边框, Color navigate_style_bg_color = default, float bg_width_offset = 0, float bg_height_offset = 0)
        {
            if (text_color_normal == Color.clear)
                text_color_normal = Color.black;

            if (text_color_selected == Color.clear)
                text_color_selected = Color.white;

            if (bg_gui_color == Color.clear)
                bg_gui_color = Color.white;

            if (text_padding == null)
                text_padding = new RectOffset(0, 0, 0, 0);

            if (bar_margin == null)
                bar_margin = new RectOffset(0, 0, 0, 0);

            GUIStyle style = new GUIStyle(style_xg_toolbar);
            if (navigate_style)
                style.normal.background = XGUI.GetFillTexture(XGUIFilled.无, XGUIColor.无);
            else
                style.normal.background = XGUI.GetFillTexture(bg_normal, bg_color);

            style.normal.textColor = text_color_normal;

            style.onNormal.background = XGUI.GetFillTexture(bg_selected, bg_color);
            style.onNormal.textColor = text_color_selected;

            style.alignment = text_anchor;
            style.padding = text_padding;
            style.margin = bar_margin;
            style.contentOffset = text_offset;
            style.fontSize = XGUI.GetFontSize(text_size);
            if (text_font != null)
                style.font = text_font;
            style.fontStyle = text_fontstyle;

            GUI.backgroundColor = bg_gui_color;
            index = GUILayout.Toolbar(index, names, style, GUILayout.MinWidth(0), GUILayout.Height(bar_height));
            GUI.backgroundColor = Color.white;

            if (navigate_style)
            {
                Rect rect = XGUI.GetLastRect();

                Rect rect_bg = new Rect(rect.x - 3 + bar_offset.x, rect.y - bg_height_offset + bar_offset.y, rect.width + bg_width_offset, bar_height + 5);
                XGUI.gui_box(
                    rect: rect_bg,
                    bg: XGUI.GetFillTexture(navigate_style_bg, XGUIColor.亮白),
                    bg_color_gui: navigate_style_bg_color,
                    offset: new Vector2(0, 0),
                    border: new RectOffset(10, 10, 10, 10));
            }

            return index;
        }

        /// <summary>
        /// 使用自动布局绘制一个自定义样式的工具栏（Toolbar），用于在多个选项间切换选择。
        /// </summary>
        /// <param name="index">当前选中项的索引（引用传递，会自动更新）。</param>
        /// <param name="names">工具栏按钮的显示名称数组。</param>
        /// <param name="bg_normal">未选中按钮的背景填充样式。</param>
        /// <param name="bg_selected">选中按钮的背景填充样式。</param>
        /// <param name="bg_color">按钮背景的颜色主题。</param>
        /// <param name="bg_gui_color">按钮背景的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="text_color_normal">未选中按钮的文本颜色。传入 <see cref="Color.clear"/> 时使用黑色。</param>
        /// <param name="text_color_selected">选中按钮的文本颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="bar_height">工具栏的高度。</param>
        /// <param name="text_anchor">按钮文本的对齐方式。</param>
        /// <param name="text_padding">按钮文本的内边距。</param>
        /// <param name="bar_margin">工具栏的外边距。</param>
        /// <param name="text_offset">按钮文本的偏移量。</param>
        /// <param name="text_font">按钮文本的字体。为 <c>null</c> 时使用样式默认字体。</param>
        /// <param name="text_fontstyle">按钮文本的字体样式。</param>
        /// <param name="navigate_style">是否使用导航样式。为 <c>true</c> 时移除按钮背景，仅显示带边框的容器。</param>
        /// <param name="navigate_style_bg">导航样式下的容器边框样式。</param>
        /// <param name="navigate_style_bg_color">导航样式下容器边框的颜色。传入 <see cref="Color.clear"/> 时使用默认颜色。</param>
        /// <returns>更新后的选中项索引。</returns>
        /// <remarks>
        /// 该方法与 <see cref="gui_toolbar"/> 功能相同，但使用 <see cref="GUILayout"/> 自动布局系统。
        /// 通过 <see cref="GUILayout.Toolbar"/> 绘制工具栏，无需手动指定矩形位置。
        /// <para>
        /// 支持两种样式模式：
        /// <list type="bullet">
        /// <item><description>标准模式：每个按钮有独立的背景填充</description></item>
        /// <item><description>导航模式：整个工具栏显示为带边框的容器，按钮无背景（适合面包屑导航）</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        public static int layout_toolbar(int index, string[] names, XGUIFilled bg_normal = XGUIFilled.实体, XGUIFilled bg_selected = XGUIFilled.实体, XGUIColor bg_color = XGUIColor.亮白, Color bg_gui_color = default, Color text_color_normal = default, Color text_color_selected = default, float bar_height = 25, Vector2 bar_offset = default, TextAnchor text_anchor = TextAnchor.MiddleCenter, RectOffset text_padding = null, RectOffset bar_margin = null, Vector2 text_offset = default, Font text_font = null, FontStyle text_fontstyle = FontStyle.Normal, XGUIFontSize text_size = XGUIFontSize.M, bool navigate_style = false, XGUIFilled navigate_style_bg = XGUIFilled.纯色边框, Color navigate_style_bg_color = default, float bg_width_offset = 0, float bg_height_offset = 0)
        {
            if (text_color_normal == Color.clear)
                text_color_normal = Color.black;

            if (text_color_selected == Color.clear)
                text_color_selected = Color.white;

            if (bg_gui_color == Color.clear)
                bg_gui_color = Color.white;

            if (text_padding == null)
                text_padding = new RectOffset(0, 0, 0, 0);

            if (bar_margin == null)
                bar_margin = new RectOffset(0, 0, 0, 0);

            XGUI.layout_space(0);

            GUIStyle style = new GUIStyle(style_xg_toolbar);
            if (navigate_style)
                style.normal.background = XGUI.GetFillTexture(XGUIFilled.无, XGUIColor.无);
            else
                style.normal.background = XGUI.GetFillTexture(bg_normal, bg_color);

            style.normal.textColor = text_color_normal;

            style.onNormal.background = XGUI.GetFillTexture(bg_selected, bg_color);
            style.onNormal.textColor = text_color_selected;

            style.alignment = text_anchor;
            style.padding = text_padding;
            style.margin = bar_margin;
            style.contentOffset = text_offset;
            style.fontSize = XGUI.GetFontSize(text_size);
            if (text_font != null)
                style.font = text_font;
            style.fontStyle = text_fontstyle;

            GUI.backgroundColor = bg_gui_color;
            index = GUILayout.Toolbar(index, names, style, GUILayout.MinWidth(0), GUILayout.Height(bar_height));
            GUI.backgroundColor = Color.white;

            if (navigate_style)
            {
                Rect rect = XGUI.GetLastRect();

                Rect rect_bg = new Rect(rect.x - 3 + bar_offset.x, rect.y - bg_height_offset + bar_offset.y, rect.width + bg_width_offset, bar_height + 5);
                XGUI.gui_box(
                    rect: rect_bg,
                    bg: XGUI.GetFillTexture(navigate_style_bg, XGUIColor.亮白),
                    bg_color_gui: navigate_style_bg_color,
                    offset: new Vector2(0, 0),
                    border: new RectOffset(10, 10, 10, 10));
            }

            return index;
        }
    }
}