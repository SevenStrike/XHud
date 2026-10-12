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
        /// 使用自动布局绘制一个带有文本标签的按钮。
        /// </summary>
        /// <param name="text">按钮上显示的文本内容。</param>
        /// <param name="tooltip">按钮的提示文本，鼠标悬停时显示。</param>
        /// <param name="button_text_color">按钮正常状态下的文本颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="press_text_color">按钮按下状态下的文本颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="font_size">按钮文本的字体大小。</param>
        /// <param name="anchor">按钮文本的对齐方式。</param>
        /// <param name="margin">按钮的外边距，叠加到样式默认边距上。</param>
        /// <param name="padding">按钮的内边距，叠加到样式默认内边距上。</param>
        /// <param name="fixed_width">按钮的固定宽度。为 0 时使用样式默认宽度。</param>
        /// <param name="layout_width">按钮的布局最大宽度限制（<see cref="GUILayout.MaxWidth"/>）。为 0 时不限制。</param>
        /// <param name="fixed_height">按钮的固定高度。为 0 时使用样式默认高度。</param>
        /// <param name="button_text_font">按钮文本的字体。为 <c>null</c> 时使用样式默认字体。</param>
        /// <param name="focus_name">按钮的控件名称，用于 <see cref="GUI.FocusControl"/> 和键盘导航。</param>
        /// <returns>如果按钮被点击则返回 <c>true</c>，否则返回 <c>false</c>。</returns>
        /// <remarks>
        /// 该方法与 <see cref="gui_button(Rect, string, string, XGUIGroup, XGUIColor, Color, Color, XGUIGroup, XGUIColor, Color, XGUIFontSize, TextAnchor, RectOffset, RectOffset, float, float, Font, string)"/> 
        /// 功能相同，但使用 <see cref="GUILayout"/> 自动布局系统，无需手动指定矩形位置。
        /// </remarks>
        public static bool toolbar_button(string text = "button", string tooltip = "clicked", Color button_text_color = default, Color press_text_color = default, Color press_bg_color = default, XGUIFontSize font_size = XGUIFontSize.M, TextAnchor anchor = TextAnchor.MiddleCenter, RectOffset margin = default, RectOffset padding = default, float layout_width = 0, float layout_min_width = 0, float fixed_height = 0, Font button_text_font = null, string focus_name = null, FontStyle button_text_style = FontStyle.Normal, Vector2 content_offset = default, Color sepline_color = default, XGUIEdge edge = XGUIEdge.全部)
        {
            if (button_text_color == Color.clear)
                button_text_color = Color.white;

            if (press_bg_color == Color.clear)
                press_bg_color = Color.white * 0.45f;

            if (sepline_color == Color.clear)
                sepline_color = Color.black * 0.55f;

            if (press_text_color == Color.clear)
                press_text_color = Color.white;

            if (margin == null)
                margin = new RectOffset(0, 0, 0, 0);

            if (padding == null)
                padding = new RectOffset(0, 0, 0, 0);

            GUIStyle style = new GUIStyle(style_xg_button);

            style.normal.background = null;
            style.normal.textColor = button_text_color;
            style.active.background = XGUI.GetFill();
            style.active.textColor = press_text_color;
            if (button_text_font != null)
                style.font = button_text_font;
            style.fontSize = XGUI.GetFontSize(font_size);
            style.alignment = anchor;
            style.fontStyle = button_text_style;
            style.padding = padding;
            style.contentOffset = content_offset;

            if (fixed_height != 0)
                style.fixedHeight = fixed_height;

            GUIContent content = new GUIContent();
            content.text = text;
            content.tooltip = tooltip;

            if ((edge == XGUIEdge.左 || edge == XGUIEdge.全部) && edge != XGUIEdge.无)
                #region 分割线
                XGUI.layout_seperator(
                    thickness: 1,
                    dir: XGUISeplineDir.垂直,
                    color: sepline_color,
                    padding: new RectOffset(0, 0, 0, 0),
                    margin: new RectOffset(margin.left, 0, margin.top, margin.bottom));
            #endregion

            // 分配GUIControl名称
            GUI.SetNextControlName(focus_name);

            Color ori_color = GUI.backgroundColor;
            GUI.backgroundColor = press_bg_color;

            bool sw = GUILayout.Button(content, style, GUILayout.MinWidth(layout_min_width), GUILayout.MaxWidth(layout_width));

            if (sw)
                XGUI.RemoveFocus();
            GUI.backgroundColor = ori_color;

            if ((edge == XGUIEdge.右 || edge == XGUIEdge.全部) && edge != XGUIEdge.无)
                #region 分割线
                XGUI.layout_seperator(
                thickness: 1,
                dir: XGUISeplineDir.垂直,
                color: sepline_color,
                padding: new RectOffset(0, 0, 0, 0),
                margin: new RectOffset(0, margin.right, margin.top, margin.bottom));
            #endregion

            return sw;
        }
        /// <summary>
        /// 使用自动布局绘制一个带有图标的按钮。
        /// </summary>
        /// <param name="tooltip">按钮的提示文本，鼠标悬停时显示。</param>
        /// <param name="icon_release">按钮正常状态的图标</param>
        /// <param name="icon_press">按钮按下状态的图标。</param>
        /// <param name="margin">按钮的外边距，叠加到样式默认边距上。</param>
        /// <param name="padding">按钮的内边距，叠加到样式默认内边距上。</param>
        /// <param name="fixed_width">按钮的固定宽度。为 0 时使用样式默认宽度。</param>
        /// <param name="layout_width">按钮的布局最大宽度限制（<see cref="GUILayout.MaxWidth"/>）。为 0 时不限制。</param>
        /// <param name="fixed_height">按钮的固定高度。为 0 时使用样式默认高度。</param>
        /// <param name="focus_name">按钮的控件名称，用于 <see cref="GUI.FocusControl"/> 和键盘导航。</param>
        /// <returns>如果按钮被点击则返回 <c>true</c>，否则返回 <c>false</c>。</returns>
        /// <remarks>
        /// 该方法与 <see cref="gui_button(Rect, string, string, XGUIGroup, XGUIColor, Color, Color, XGUIGroup, XGUIColor, Color, XGUIFontSize, TextAnchor, RectOffset, RectOffset, float, float, Font, string)"/> 
        /// 功能相同，但使用 <see cref="GUILayout"/> 自动布局系统，无需手动指定矩形位置。
        /// </remarks>
        public static bool toolbar_button(string tooltip = "clicked", Texture2D icon_release = null, Texture2D icon_press = null, Color press_icon_color = default, RectOffset margin = default, RectOffset padding = default, float layout_width = 0, float layout_min_width = 0, float layout_height = 0, float layout_min_height = 0, string focus_name = null, Vector2 content_offset = default, Color sepline_color = default, XGUIEdge edge = XGUIEdge.全部)
        {
            if (press_icon_color == Color.clear)
                press_icon_color = Color.white * 0.45f;

            if (sepline_color == Color.clear)
                sepline_color = Color.black * 0.55f;

            if (margin == null)
                margin = new RectOffset(0, 0, 0, 0);

            if (padding == null)
                padding = new RectOffset(0, 0, 0, 0);

            GUIStyle style = new GUIStyle(style_xg_button);

            style.normal.background = icon_release;
            style.active.background = icon_press;

            style.alignment = TextAnchor.MiddleCenter;
            style.padding = padding;
            style.contentOffset = content_offset;
            style.stretchWidth = false;
            style.stretchHeight = true;
            style.border = new RectOffset(0, 0, 0, 0);
            GUIContent content = new GUIContent();
            content.tooltip = tooltip;

            if ((edge == XGUIEdge.左 || edge == XGUIEdge.全部) && edge != XGUIEdge.无)
                #region 分割线
                XGUI.layout_seperator(
                    thickness: 1,
                    dir: XGUISeplineDir.垂直,
                    color: sepline_color,
                    padding: new RectOffset(0, 0, 0, 0),
                    margin: new RectOffset(margin.left, 0, margin.top, margin.bottom));
            #endregion

            // 分配GUIControl名称
            GUI.SetNextControlName(focus_name);

            Color ori_color = GUI.backgroundColor;
            GUI.backgroundColor = press_icon_color;
            bool sw = false;

            sw = GUILayout.Button(content, style, GUILayout.Width(layout_width));

            if (sw)
                XGUI.RemoveFocus();
            GUI.backgroundColor = ori_color;

            if ((edge == XGUIEdge.右 || edge == XGUIEdge.全部) && edge != XGUIEdge.无)
                #region 分割线
                XGUI.layout_seperator(
                thickness: 1,
                dir: XGUISeplineDir.垂直,
                color: sepline_color,
                padding: new RectOffset(0, 0, 0, 0),
                margin: new RectOffset(0, margin.right, margin.top, margin.bottom));
            #endregion

            return sw;
        }
    }
}