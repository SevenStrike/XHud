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
        /// 开始一个可折叠的、带有标题和图标的自定义分组布局。
        /// </summary>
        /// <param name="rect_group">分组区域的位置和尺寸。</param>
        /// <param name="bg_fill">分组背景的填充样式。</param>
        /// <param name="bg_color">分组背景的颜色主题。</param>
        /// <param name="bg_color_gui">分组背景的叠加颜色。传入默认值时使用白色。</param>
        /// <param name="bg_height">分组的固定高度。为 0 时使用样式默认高度。</param>
        /// <param name="absolute_padding">是否使用绝对内边距。为 <c>true</c> 时直接使用 <paramref name="padding"/> 覆盖样式默认值。</param>
        /// <param name="absolute_margin">是否使用绝对外边距。为 <c>true</c> 时直接使用 <paramref name="margin"/> 覆盖样式默认值。</param>
        /// <param name="margin">分组的外边距。</param>
        /// <param name="padding">分组的内边距。</param>
        /// <param name="title">分组的标题文本。为 <c>null</c> 或空时不显示标题。</param>
        /// <param name="title_bg_fill">标题背景的填充样式。为 <see cref="XGUIFilled.无"/> 时不绘制标题背景。</param>
        /// <param name="title_bg_color">标题背景的颜色主题。</param>
        /// <param name="title_bg_color_gui">标题背景的叠加颜色。传入默认值时使用白色。</param>
        /// <param name="title_size">标题文本的字体大小。</param>
        /// <param name="title_anchor">标题文本的对齐方式。</param>
        /// <param name="title_text_color">标题文本的颜色。传入 <see cref="Color.clear"/> 时使用默认颜色。</param>
        /// <param name="title_offset">标题位置的偏移量。</param>
        /// <param name="title_padding">标题的内边距。</param>
        /// <param name="title_font">标题文本的字体。为 <c>null</c> 时使用分组样式的默认字体。</param>
        /// <param name="title_font_style">标题文本的字体样式（正常、粗体、斜体等）。</param>
        /// <param name="title_clipping">标题文本的裁剪方式。</param>
        /// <param name="icon">标题左侧显示的图标纹理。为 <c>null</c> 时不显示图标。</param>
        /// <param name="icon_color">图标的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="icon_padding">图标的内边距。</param>
        /// <param name="foldout">分组的折叠状态。传入 <c>true</c> 表示展开，<c>false</c> 表示折叠。</param>
        /// <returns>更新后的折叠状态。点击标题或图标区域时会切换状态。</returns>
        /// <remarks>
        /// 该方法与 <see cref="gui_group_end"/> 成对使用。支持可折叠功能，点击标题文字或图标区域可切换折叠状态。
        /// 当 <paramref name="title_bg_fill"/> 不为 <see cref="XGUIFilled.无"/> 时，标题背景宽度会根据文本长度自动计算。
        /// 返回的折叠状态应传递给下次调用，以实现状态持久化。
        /// </remarks>
        /// <example>
        /// <code>
        /// bool isExpanded = true;
        /// Rect groupRect = new Rect(10, 10, 400, 200);
        /// 
        /// isExpanded = XGUI.gui_group_start(
        ///     groupRect,
        ///     bg_fill: XGUIFilled.实体,
        ///     bg_color: XGUIColor.深空灰,
        ///     title: "高级设置",
        ///     title_bg_fill: XGUIFilled.实体,
        ///     title_bg_color: XGUIColor.工业蓝,
        ///     icon: myIcon,
        ///     foldout: isExpanded
        /// );
        /// 
        /// if (isExpanded)
        /// {
        ///     GUILayout.Label("分组内容");
        ///     GUILayout.Button("点击");
        /// }
        /// 
        /// XGUI.gui_group_end();
        /// </code>
        /// </example>
        public static bool gui_group_start(Rect rect_group, XGUIFilled bg_fill = XGUIFilled.无, XGUIColor bg_color = XGUIColor.亮白, Color bg_color_gui = default, float bg_height = 0, bool absolute_padding = false, bool absolute_margin = false, RectOffset margin = default, RectOffset padding = default, string title = null, XGUIFilled title_bg_fill = XGUIFilled.无, XGUIColor title_bg_color = XGUIColor.无, Color title_bg_color_gui = default, XGUIFontSize title_size = XGUIFontSize.B, TextAnchor title_anchor = TextAnchor.MiddleLeft, Color title_text_color = default, Vector2 title_offset = default, RectOffset title_padding = default, Font title_font = null, FontStyle title_font_style = FontStyle.Normal, TextClipping title_clipping = TextClipping.Clip, Texture2D icon = null, Color icon_color = default, RectOffset icon_padding = default, bool foldout = true)
        {
            #region group
            if (bg_color_gui == default)
                bg_color_gui = Color.white;

            if (padding == null)
                padding = new RectOffset(0, 0, 0, 0);

            if (margin == null)
                margin = new RectOffset(0, 0, 0, 0);

            // 样式 - 背景
            GUIStyle style_group = new GUIStyle(style_xg_group);
            if (bg_fill != XGUIFilled.无)
                style_group.normal.background = GetFillTexture(bg_fill, bg_color);
            else
                style_group.normal.background = null;

            if (bg_height != 0)
                style_group.fixedHeight = bg_height;

            if (!absolute_padding)
            {
                RectOffset _p = style_group.padding;
                style_group.padding = new RectOffset(_p.left + padding.left, _p.right + padding.right, _p.top + padding.top, _p.bottom + padding.bottom);
            }
            else
            {
                style_group.padding = padding;
            }

            if (!absolute_margin)
            {
                RectOffset _m = style_group.margin;
                style_group.margin = new RectOffset(_m.left + margin.left, _m.right + margin.right, _m.top + margin.top, _m.bottom + margin.bottom);
            }
            else
            {
                style_group.margin = margin;
            }

            #region title
            if (!string.IsNullOrEmpty(title))
            {
                if (title_padding == null)
                    title_padding = new RectOffset(0, 0, 0, 0);

                Rect rect_title = rect_group;

                // 如果设定为有标题背景则使用根据标题文字长度自动计算宽度，否则使用窗口宽度减去阈值
                if (title_bg_fill != XGUIFilled.无)
                {
                    GUIStyle style = new GUIStyle(style_xg_label);  // 直接使用
                    style.fontSize = XGUI.GetFontSize(title_size);
                    style.clipping = TextClipping.Clip;
                    style.padding = title_padding;
                    Vector2 size = style.CalcSize(new GUIContent(title));  // 得到正确宽度

                    if (XGUI.GetCurrentWindowWidth() > size.x + 80)
                    {
                        rect_title = new Rect(icon == null ? 30 : 65, rect_group.y - style_group.padding.top - 13, size.x + 20, size.y);
                    }
                    else
                    {
                        rect_title = new Rect(XGUI.GetCurrentWindowWidth() + 100, 0, 0, 0);
                    }
                }
                else
                {
                    rect_title = new Rect(icon == null ? 30 : 58, rect_group.y - style_group.padding.top - 17, XGUI.GetCurrentWindowWidth() - (style_group.margin.right + style_group.padding.right) - 50, 25);
                }

                XGUI.gui_label(
                    rect: rect_title,
                    text: new GUIContent(title),
                    bg_fill: title_bg_fill,
                    bg_color: title_bg_color,
                    bg_color_gui: title_bg_color_gui,
                    size: title_size,
                    anchor: title_bg_fill != XGUIFilled.无 ? TextAnchor.MiddleCenter : title_anchor,
                    text_color: title_text_color,
                    offset: title_offset + new Vector2(0, -1),
                    padding: title_bg_fill != XGUIFilled.无 ? new RectOffset(11 + title_padding.left, 11 + title_padding.right, title_padding.top, title_padding.bottom) : new RectOffset(5 + title_padding.left, 5 + title_padding.right, 0 + title_padding.top, 0 + title_padding.bottom),
                    clipping: title_clipping,
                    font: title_font != null ? title_font : style_group.font,
                    font_style: title_font_style, act_on_press_label: () =>
                    {
                        XGUI.RemoveFocus();
                    });

                // 测试区域显示
                //XGUI.Gui_Box(rect_title, Color.red * 0.8f);

                // 检测标题区域的点击事件
                Event currentEvent = Event.current;
                if (currentEvent.type == EventType.MouseDown && rect_title.Contains(currentEvent.mousePosition))
                {
                    foldout = !foldout;
                    currentEvent.Use(); // 消耗事件，防止穿透
                }
            }
            #endregion

            if (icon != null)
            {
                Rect rect_icon = rect_group;
                rect_icon = new Rect(rect_icon.x + 20, rect_group.y - style_group.padding.top - 13, 16, 16);
                XGUI.gui_icon(
                    rect: rect_icon,
                    icon: icon,
                    padding: icon_padding,
                    color: icon_color
                    );

                // 测试区域显示
                //XGUI.Gui_Box(rect_icon, Color.red * 0.8f);

                // 如果需要图标也可点击，取消下面的注释
                if (icon != null)
                {
                    Event currentEvent = Event.current;
                    if (currentEvent.type == EventType.MouseDown && rect_icon.Contains(currentEvent.mousePosition))
                    {
                        foldout = !foldout;
                        currentEvent.Use();
                    }
                }
            }

            Color col = GUI.backgroundColor;
            GUI.backgroundColor = bg_color_gui;
            GUI.BeginGroup(rect_group, style_group);
            GUI.backgroundColor = col;
            #endregion         

            return foldout;
        }
        /// <summary>
        /// 结束由 <see cref="gui_group_start"/> 开始的分组布局。
        /// </summary>
        /// <remarks>
        /// 该方法执行 <see cref="GUI.EndGroup"/> 并重置 GUI 背景颜色。
        /// 必须与 <see cref="gui_group_start"/> 成对使用，否则会导致布局错乱。
        /// </remarks>
        public static void gui_group_end()
        {
            GUI.backgroundColor = Color.white;
            GUI.EndGroup();
        }
    }
}