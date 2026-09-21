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
        /// 使用自动布局绘制一个带有丰富样式配置的标签。
        /// </summary>
        /// <param name="text">要显示的文本内容。</param>
        /// <param name="bg_fill">标签背景的填充样式。为 <see cref="XGUIFilled.无"/> 时不绘制背景。</param>
        /// <param name="bg_color">标签背景的颜色主题。</param>
        /// <param name="bg_color_gui">标签背景的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="size">文本的字体大小。</param>
        /// <param name="anchor">文本的对齐方式。</param>
        /// <param name="text_color">文本的颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="offset">文本内容的偏移量。</param>
        /// <param name="clipping">文本的裁剪方式（裁剪、溢出等）。</param>
        /// <param name="margin">标签的外边距，叠加到样式默认边距上。</param>
        /// <param name="padding">标签的内边距，叠加到样式默认内边距上。</param>
        /// <param name="font">标签文本的字体。为 <c>null</c> 时使用样式默认字体。</param>
        /// <param name="font_style">标签文本的字体样式（正常、粗体、斜体等）。</param>
        /// <param name="wrap">是否启用文本自动换行。</param>
        /// <param name="width">标签的固定宽度。为 0 时使用样式默认宽度。</param>
        /// <param name="height">标签的固定高度。为 0 时根据 <paramref name="singleLineHeight"/> 决定。</param>
        /// <param name="singleLineHeight">是否强制使用单行高度。为 <c>true</c> 且 <paramref name="wrap"/> 为 <c>false</c> 时使用 <see cref="GetSingleLineHeight"/>。</param>
        /// <param name="tooltip">标签的提示文本，鼠标悬停时显示。</param>
        /// <remarks>
        /// 该方法与 <see cref="gui_label(Rect, string, XGUIFilled, XGUIColor, Color, XGUIFontSize, TextAnchor, Color, Vector2, TextClipping, RectOffset, RectOffset, Font, FontStyle, bool, float, float)"/> 
        /// 功能相同，但使用 <see cref="GUILayout"/> 自动布局系统，无需手动指定矩形位置。
        /// <para>
        /// 高度行为说明：
        /// <list type="bullet">
        /// <item><description>如果 <paramref name="height"/> 不为 0，使用指定高度</description></item>
        /// <item><description>如果 <paramref name="wrap"/> 为 <c>true</c>，让内容自动撑开高度</description></item>
        /// <item><description>否则使用 <see cref="GetSingleLineHeight"/> 作为高度</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        public static void layout_label(string text, XGUIFilled bg_fill = XGUIFilled.无, XGUIColor bg_color = XGUIColor.无, Color bg_color_gui = default, XGUIFontSize size = XGUIFontSize.B, TextAnchor anchor = TextAnchor.MiddleLeft, Color text_color = default, Vector2 offset = default, TextClipping clipping = TextClipping.Clip, RectOffset margin = default, RectOffset padding = default, Font font = null, FontStyle font_style = FontStyle.Normal, bool wrap = false, float width = 0, float height = 0, bool singleLineHeight = true, string tooltip = null, Action act_on_press_label = null)
        {
            if (text_color == Color.clear)
                text_color = Color.white;

            if (offset == default)
                offset = Vector2.zero;

            if (bg_color_gui == Color.clear)
                bg_color_gui = Color.white;

            if (padding == null)
                padding = new RectOffset(0, 0, 0, 0);

            if (margin == null)
                margin = new RectOffset(0, 0, 0, 0);

            GUIStyle style = new GUIStyle(style_xg_label);
            style.normal.background = GetFillTexture(bg_fill, bg_color);
            style.normal.textColor = text_color;
            style.fontSize = XGUI.GetFontSize(size);

            RectOffset _p = style.padding;
            RectOffset _m = style.margin;

            style.padding = new RectOffset(_p.left + padding.left, _p.right + padding.right, _p.top + padding.top, _p.bottom + padding.bottom);

            style.margin = new RectOffset(_m.left + margin.left, _m.right + margin.right, _m.top + margin.top, _m.bottom + margin.bottom);

            style.contentOffset = new Vector2(style.contentOffset.x + offset.x, style.contentOffset.y + offset.y);

            style.alignment = anchor;
            if (font != null)
                style.font = font;

            style.fontStyle = font_style;

            style.wordWrap = wrap;
            style.clipping = clipping;

            Color col = GUI.backgroundColor;
            GUI.backgroundColor = bg_color_gui;

            GUIContent content = new GUIContent();
            content.text = text;
            content.tooltip = string.IsNullOrEmpty(tooltip) ? "" : tooltip;

            if (width != 0)
            {
                if (height != 0)
                    GUILayout.Label(content, style, GUILayout.Width(width), GUILayout.MinWidth(0), GUILayout.Height(height));
                else
                {
                    if (wrap)
                        // 换行模式：不设置固定高度，让内容自动撑开
                        GUILayout.Label(content, style, GUILayout.Width(width), GUILayout.MinWidth(0));
                    else
                        GUILayout.Label(content, style, GUILayout.Width(width), GUILayout.MinWidth(0), GUILayout.Height(XGUI.GetSingleLineHeight()));
                }
            }
            else
            {
                if (height != 0)
                    GUILayout.Label(content, style, GUILayout.MinWidth(0), GUILayout.Height(height));
                else if (wrap)
                    // 换行模式：不设置固定高度，让内容自动撑开
                    GUILayout.Label(content, style, GUILayout.MinWidth(0));
                else
                    GUILayout.Label(content, style, GUILayout.MinWidth(0), GUILayout.Height(XGUI.GetSingleLineHeight()));
            }

            GUI.backgroundColor = col;

            Rect rect = XGUI.GetLastRect();
            //XGUI.gui_box(rect, Color.red);

            Event e = Event.current;
            if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
            {
                if (act_on_press_label != null)
                    act_on_press_label();

                XGUI.RemoveFocus();

                EditorWindow.focusedWindow?.Repaint();
            }
        }
    }
}