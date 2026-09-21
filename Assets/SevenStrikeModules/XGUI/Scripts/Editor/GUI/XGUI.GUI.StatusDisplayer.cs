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
    using UnityEngine;

    /// <summary>
    /// 为 Editor 界面提供样式控件
    /// </summary>
    public static partial class XGUI
    {
        /// <summary>
        /// 绘制一个显示标题和副标题文本的状态展示器。
        /// </summary>
        /// <param name="rect">展示器的位置和尺寸。</param>
        /// <param name="title">主标题文本。</param>
        /// <param name="title_size">主标题的字体大小。</param>
        /// <param name="title_font_style">主标题的字体样式。</param>
        /// <param name="title_color">主标题的颜色。传入 <see cref="Color.clear"/> 时使用半透明白色。</param>
        /// <param name="subtitle">副标题文本（显示在右侧）。</param>
        /// <param name="subtitle_size">副标题的字体大小。</param>
        /// <param name="subtitle_font_style">副标题的字体样式。</param>
        /// <param name="subtitle_color">副标题的颜色。传入 <see cref="Color.clear"/> 时使用默认颜色。</param>
        /// <param name="padding">展示器的内边距。</param>
        /// <param name="margin">展示器的外边距。</param>
        /// <param name="tooltip">提示文本（当前未使用）。</param>
        /// <remarks>
        /// 该方法使用 <see cref="gui_group_start"/> 和 <see cref="gui_group_end"/> 创建分组容器，
        /// 内部包含左对齐的主标题和右对齐的副标题。适用于显示状态信息、统计数值或键值对数据。
        /// </remarks>
        /// <example>
        /// <code>
        /// Rect stateRect = new Rect(10, 10, 300, 25);
        /// XGUI.gui_state_displayer_text(
        ///     rect: stateRect,
        ///     title: "当前状态",
        ///     subtitle: "运行中",
        ///     title_color: Color.cyan,
        ///     subtitle_color: Color.green
        /// );
        /// </code>
        /// </example>
        public static void gui_state_displayer_text(Rect rect = default, GUIContent title = null, XGUIFontSize title_size = XGUIFontSize.M, FontStyle title_font_style = FontStyle.Normal, Color title_color = default, GUIContent subtitle = null, XGUIFontSize subtitle_size = XGUIFontSize.M, FontStyle subtitle_font_style = FontStyle.Normal, Color subtitle_color = default, RectOffset padding = null, RectOffset margin = null, string tooltip = null)
        {
            if (title_color == Color.clear)
                title_color = Color.white * 0.9f;

            if (padding == null)
                padding = new RectOffset(0, 0, 0, 0);

            if (margin == null)
                margin = new RectOffset(0, 0, 0, 0);

            XGUI.gui_group_start(
               rect_group: rect,
              absolute_margin: false,
               absolute_padding: false,
               margin: padding,
               padding: margin,
               foldout: false);

            Rect rect_root = new Rect(0, 0, rect.width, rect.height);

            // 测试区域
            //XGUI.gui_box(rect_root, Color.red * 0.5f);

            XGUI.gui_label(
             rect: new Rect(rect_root.x, rect_root.y, rect_root.width / 2, rect.height),
             text: title,
             text_color: title_color,
             size: title_size,
             clipping: TextClipping.Clip,
             anchor: TextAnchor.MiddleLeft,
             font_style: title_font_style);

            XGUI.gui_label(
            rect: new Rect(rect_root.x + (rect_root.width / 2), rect_root.y, rect_root.width - (rect_root.width / 2), rect.height),
            text: subtitle,
            text_color: subtitle_color,
            size: subtitle_size,
            clipping: TextClipping.Clip,
            anchor: TextAnchor.MiddleRight,
            font_style: subtitle_font_style);

            XGUI.gui_group_end();
        }
        /// <summary>
        /// 绘制一个带有图标的标题状态展示器。
        /// </summary>
        /// <param name="rect">展示器的位置和尺寸。</param>
        /// <param name="title">标题文本。</param>
        /// <param name="title_size">标题的字体大小。</param>
        /// <param name="title_font_style">标题的字体样式。</param>
        /// <param name="title_color">标题的颜色。传入 <see cref="Color.clear"/> 时使用半透明白色。</param>
        /// <param name="icon">右侧显示的图标纹理。</param>
        /// <param name="icon_size">图标尺寸（当前未使用）。</param>
        /// <param name="icon_offset">图标偏移量（当前未使用）。</param>
        /// <param name="icon_color">图标的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="padding">展示器的内边距。</param>
        /// <param name="margin">展示器的外边距。</param>
        /// <remarks>
        /// 该方法在分组容器的左侧显示标题文本，右侧显示图标。适用于状态指示、功能标识等场景。
        /// 图标会保持原始纹理尺寸进行绘制。
        /// </remarks>
        /// <example>
        /// <code>
        /// Texture2D statusIcon = XGUI.GetBasedIcon("icon_check");
        /// Rect iconStateRect = new Rect(10, 40, 300, 25);
        /// XGUI.gui_state_displayer_icon(
        ///     rect: iconStateRect,
        ///     title: "验证通过",
        ///     icon: statusIcon,
        ///     icon_color: Color.green
        /// );
        /// </code>
        /// </example>
        public static void gui_state_displayer_icon(Rect rect = default, GUIContent title = null, XGUIFontSize title_size = XGUIFontSize.M, FontStyle title_font_style = FontStyle.Normal, Color title_color = default, Texture2D icon = null, Vector2 icon_size = default, Vector2 icon_offset = default, Color icon_color = default, RectOffset padding = null, RectOffset margin = null)
        {
            if (title_color == Color.clear)
                title_color = Color.white * 0.9f;

            if (icon_size == Vector2.zero)
                icon_size = Vector2.zero;

            if (icon_offset == Vector2.zero)
                icon_offset = Vector2.zero;

            if (icon_color == Color.clear)
                icon_color = Color.white;

            if (padding == null)
                padding = new RectOffset(0, 0, 0, 0);

            if (margin == null)
                margin = new RectOffset(0, 0, 0, 0);

            XGUI.gui_group_start(
                rect_group: rect,
                absolute_margin: false,
                absolute_padding: false,
                margin: margin,
                padding: padding,
                foldout: false);

            Rect rect_root = new Rect(0, 0, rect.width, rect.height);

            // 测试区域
            //XGUI.gui_box(rect_root, Color.red * 0.5f);

            XGUI.gui_label(
                rect: new Rect(rect_root.x, rect_root.y, rect_root.width / 2, rect.height),
                text: title,
                text_color: title_color,
                size: title_size,
                clipping: TextClipping.Clip,
                anchor: TextAnchor.MiddleLeft,
                font_style: title_font_style);

            XGUI.gui_icon(
                rect: new Rect(rect_root.x + (rect_root.width - icon.width), rect_root.y, icon.width, icon.height),
                icon: icon,
                padding: new RectOffset(0, 0, 0, 0),
                border: new RectOffset(0, 0, 0, 0),
                color: icon_color);

            XGUI.gui_group_end();
        }
        /// <summary>
        /// 绘制一个带有操作按钮的标题状态展示器。
        /// </summary>
        /// <param name="rect">展示器的位置和尺寸。</param>
        /// <param name="title">标题文本。</param>
        /// <param name="title_size">标题的字体大小。</param>
        /// <param name="title_font_style">标题的字体样式。</param>
        /// <param name="title_color">标题的颜色。传入 <see cref="Color.clear"/> 时使用半透明白色。</param>
        /// <param name="btn_width">按钮的固定宽度。</param>
        /// <param name="btn_tooltip">按钮的提示文本。</param>
        /// <param name="btn_text">按钮上显示的文本。</param>
        /// <param name="btn_bg">按钮正常状态的填充样式。</param>
        /// <param name="btn_color">按钮正常状态的颜色主题。</param>
        /// <param name="btn_gui_color">按钮背景的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="btn_text_color">按钮正常状态的文本颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="btn_text_size">按钮文本的字体大小。</param>
        /// <param name="btn_press_bg">按钮按下状态的填充样式。</param>
        /// <param name="btn_press_color">按钮按下状态的颜色主题。</param>
        /// <param name="btn_press_text_color">按钮按下状态的文本颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="padding">展示器的内边距。</param>
        /// <param name="margin">展示器的外边距。</param>
        /// <param name="callback_clicked">按钮点击时触发的回调委托。</param>
        /// <remarks>
        /// 该方法在分组容器的左侧显示标题文本，右侧显示一个操作按钮。
        /// 适用于需要用户进行单一操作的状态展示场景（如"刷新"、"启用"、"重置"等）。
        /// </remarks>
        /// <example>
        /// <code>
        /// Rect btnStateRect = new Rect(10, 70, 300, 25);
        /// XGUI.gui_state_displayer_btn(
        ///     rect: btnStateRect,
        ///     title: "数据同步",
        ///     btn_width: 60,
        ///     btn_text: "刷新",
        ///     btn_bg: XGUIFilled.实体,
        ///     btn_color: XGUIColor.工业蓝,
        ///     callback_clicked: () => { Debug.Log("刷新按钮被点击"); }
        /// );
        /// </code>
        /// </example>
        public static void gui_state_displayer_btn(Rect rect = default, GUIContent title = null, XGUIFontSize title_size = XGUIFontSize.M, FontStyle title_font_style = FontStyle.Normal, Color title_color = default, float btn_width = 0, string btn_tooltip = null, string btn_text = null, XGUIFilled btn_bg = XGUIFilled.实体, XGUIColor btn_color = XGUIColor.亮白, Color btn_gui_color = default, Color btn_text_color = default, XGUIFontSize btn_text_size = XGUIFontSize.M, XGUIFilled btn_press_bg = XGUIFilled.实体, XGUIColor btn_press_color = XGUIColor.亮白, Color btn_press_text_color = default, RectOffset padding = null, RectOffset margin = null, Action callback_clicked = null)
        {
            if (title_color == Color.clear)
                title_color = Color.white * 0.9f;

            if (btn_gui_color == Color.clear)
                btn_gui_color = Color.white;

            if (btn_text_color == Color.clear)
                btn_text_color = Color.white;

            if (btn_press_text_color == Color.clear)
                btn_press_text_color = Color.white;

            if (padding == null)
                padding = new RectOffset(0, 0, 0, 0);

            if (margin == null)
                margin = new RectOffset(0, 0, 0, 0);

            XGUI.gui_group_start(
             rect_group: rect,
             absolute_margin: false,
             absolute_padding: false,
             margin: margin,
             padding: padding,
             foldout: false);

            Rect rect_root = new Rect(0, 0, rect.width, rect.height);

            XGUI.gui_label(
               rect: new Rect(rect_root.x, rect_root.y, rect_root.width / 2, rect.height),
               text: title,
               text_color: title_color,
               size: title_size,
               clipping: TextClipping.Clip,
               anchor: TextAnchor.MiddleLeft,
               font_style: title_font_style);

            if (XGUI.gui_button(
                rect: new Rect(rect_root.x + (rect_root.width - btn_width) + 2, rect_root.y, btn_width, XGUI.GetSingleLineHeight()),
                text: btn_text,
                tooltip: btn_tooltip,
                btn_fill: btn_bg,
                btn_color: btn_color,
                btn_color_gui: btn_gui_color,
                btn_text_color: btn_text_color,
                press_fill: btn_press_bg,
                press_color: btn_press_color,
                press_text_color: btn_press_text_color,
                font_size: btn_text_size,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0)))
            {
                if (callback_clicked != null)
                    callback_clicked();
            }

            XGUI.gui_group_end();
        }
    }
}