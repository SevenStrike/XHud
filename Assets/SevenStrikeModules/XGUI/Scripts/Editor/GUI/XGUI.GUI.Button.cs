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
        /// 绘制一个带有文本标签和丰富样式配置的按钮。
        /// </summary>
        /// <param name="rect">按钮的位置和尺寸。传入 <see cref="Rect"/> 默认值时使用自动布局。</param>
        /// <param name="text">按钮上显示的文本内容。</param>
        /// <param name="tooltip">按钮的提示文本，鼠标悬停时显示。</param>
        /// <param name="btn_fill">按钮正常状态的填充样式。</param>
        /// <param name="btn_color">按钮正常状态的颜色主题。</param>
        /// <param name="btn_color_gui">按钮背景的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="btn_text_color">按钮正常状态下的文本颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="press_fill">按钮按下状态的填充样式。</param>
        /// <param name="press_color">按钮按下状态的颜色主题。</param>
        /// <param name="press_text_color">按钮按下状态下的文本颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="font_size">按钮文本的字体大小。</param>
        /// <param name="anchor">按钮文本的对齐方式。</param>
        /// <param name="margin">按钮的外边距，叠加到样式默认边距上。</param>
        /// <param name="padding">按钮的内边距，叠加到样式默认内边距上。</param>
        /// <param name="width">按钮的固定宽度。为 0 时使用样式默认宽度。</param>
        /// <param name="height">按钮的固定高度。为 0 时使用样式默认高度。</param>
        /// <param name="button_text_font">按钮文本的字体。为 <c>null</c> 时使用样式默认字体。</param>
        /// <param name="focus_name">按钮的控件名称，用于 <see cref="GUI.FocusControl"/> 和键盘导航。</param>
        /// <returns>如果按钮被点击则返回 <c>true</c>，否则返回 <c>false</c>。</returns>
        /// <remarks>
        /// 该方法提供完整的按钮样式定制能力，包括正常和按下两种状态的填充样式、颜色和文本颜色。
        /// 按钮的边距和内边距在样式默认值基础上进行叠加，便于微调布局。
        /// </remarks>
        public static bool gui_button(Rect rect = default, string text = "button", string tooltip = "clicked", XGUIFilled btn_fill = XGUIFilled.实体, XGUIColor btn_color = XGUIColor.亮白, Color btn_color_gui = default, Color btn_text_color = default, XGUIFilled press_fill = XGUIFilled.实体, XGUIColor press_color = XGUIColor.阴影灰, Color press_text_color = default, XGUIFontSize font_size = XGUIFontSize.B, TextAnchor anchor = TextAnchor.MiddleCenter, RectOffset margin = default, RectOffset padding = default, float width = 0, float height = 0, Font button_text_font = null, string focus_name = null)
        {
            if (btn_text_color == Color.clear)
                btn_text_color = Color.white;

            if (btn_color_gui == Color.clear)
                btn_color_gui = Color.white;

            if (press_text_color == Color.clear)
                press_text_color = Color.white;

            if (margin == null)
                margin = new RectOffset(0, 0, 0, 0);

            if (padding == null)
                padding = new RectOffset(0, 0, 0, 0);

            GUIStyle style = new GUIStyle(style_xg_button);
            if (btn_fill != XGUIFilled.无)
                style.normal.background = GetBtnFillTexture(btn_fill, btn_color);
            else
                style.normal.background = null;

            if (press_fill != XGUIFilled.无)
            {
                style.active.background = GetBtnFillTexture(press_fill, press_color);
                style.active.textColor = press_text_color;
            }
            else
                style.active.background = null;

            style.normal.textColor = btn_text_color;
            if (button_text_font != null)
                style.font = button_text_font;
            style.fontSize = XGUI.GetFontSize(font_size);
            style.alignment = anchor;

            RectOffset _p = style.padding;
            RectOffset _m = style.margin;

            style.padding = new RectOffset(_p.left + padding.left, _p.right + padding.right, _p.top + padding.top, _p.bottom + padding.bottom + 4);

            style.margin = new RectOffset(_m.left + margin.left, _m.right + margin.right, _m.top + margin.top, _m.bottom + margin.bottom);

            if (width != 0)
                style.fixedWidth = width;
            if (height != 0)
                style.fixedHeight = height;

            GUIContent content = new GUIContent();
            content.text = text;
            content.tooltip = tooltip;

            // 分配GUIControl名称
            GUI.SetNextControlName(focus_name);

            Color col = GUI.backgroundColor;
            GUI.backgroundColor = btn_color_gui;
            bool sw = GUI.Button(rect, content, style);
            GUI.backgroundColor = col;

            return sw;
        }

        /// <summary>
        /// 绘制一个使用自定义纹理作为背景的图标按钮。
        /// </summary>
        /// <param name="rect">按钮的位置和尺寸。传入 <see cref="Rect"/> 默认值时使用自动布局。</param>
        /// <param name="tooltip">按钮的提示文本，鼠标悬停时显示。</param>
        /// <param name="tex_release">按钮正常状态下的背景纹理。</param>
        /// <param name="tex_press">按钮按下状态下的背景纹理。</param>
        /// <param name="tex_gui_color">按钮背景的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="margin">按钮的外边距，叠加到样式默认边距上。</param>
        /// <param name="padding">按钮的内边距，叠加到样式默认内边距上。</param>
        /// <param name="width">按钮的固定宽度。为 0 时使用样式默认宽度。</param>
        /// <param name="height">按钮的固定高度。为 0 时使用样式默认高度。</param>
        /// <param name="focus_name">按钮的控件名称，用于 <see cref="GUI.FocusControl"/> 和键盘导航。</param>
        /// <returns>如果按钮被点击则返回 <c>true</c>，否则返回 <c>false</c>。</returns>
        /// <remarks>
        /// 该方法适用于纯图标按钮的场景，通过 <paramref name="tex_release"/> 和 <paramref name="tex_press"/>
        /// 分别指定正常和按下状态的背景纹理。不显示文本内容，仅显示工具提示。
        /// </remarks>
        public static bool gui_button(Rect rect = default, string tooltip = "clicked", Texture2D tex_release = null, Texture2D tex_press = null, Color tex_gui_color = default, RectOffset margin = default, RectOffset border = default, RectOffset padding = default, float width = 0, float height = 0, string focus_name = null)
        {
            if (tex_gui_color == Color.clear)
                tex_gui_color = Color.white;

            if (margin == null)
                margin = new RectOffset(0, 0, 0, 0);

            if (padding == null)
                padding = new RectOffset(0, 0, 0, 0);

            if (border == null)
                border = new RectOffset(0, 0, 0, 0);

            GUIStyle style = new GUIStyle(style_xg_button);
            if (tex_release != null)
                style.normal.background = tex_release;

            if (tex_press != null)
                style.active.background = tex_press;

            RectOffset _p = style.padding;
            RectOffset _m = style.margin;

            style.padding = new RectOffset(_p.left + padding.left, _p.right + padding.right, _p.top + padding.top, _p.bottom + padding.bottom);

            style.margin = new RectOffset(_m.left + margin.left, _m.right + margin.right, _m.top + margin.top, _m.bottom + margin.bottom);

            if (width != 0)
                style.fixedWidth = width;
            if (height != 0)
                style.fixedHeight = height;
            style.border = border;
            GUIContent content = new GUIContent();
            content.tooltip = tooltip;

            // 分配GUIControl名称
            GUI.SetNextControlName(focus_name);

            Color col = GUI.backgroundColor;
            GUI.backgroundColor = tex_gui_color;
            bool sw = GUI.Button(rect, content, style);
            GUI.backgroundColor = col;

            return sw;
        }
    }
}