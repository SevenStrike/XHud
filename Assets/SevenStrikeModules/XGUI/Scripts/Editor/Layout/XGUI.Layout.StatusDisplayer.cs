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
        /// 使用自动布局绘制一个显示标题和副标题文本的状态展示器。
        /// </summary>
        /// <param name="title">主标题文本。</param>
        /// <param name="title_size">主标题的字体大小。</param>
        /// <param name="title_color">主标题的颜色。传入 <see cref="Color.clear"/> 时使用半透明白色。</param>
        /// <param name="subtitle">副标题文本（显示在右侧）。</param>
        /// <param name="subtitle_size">副标题的字体大小。</param>
        /// <param name="subtitle_color">副标题的颜色。传入 <see cref="Color.clear"/> 时使用默认颜色。</param>
        /// <param name="padding">展示器的内边距。</param>
        /// <param name="margin">展示器的外边距。</param>
        /// <param name="subtitle_tooltip">副标题的提示文本，鼠标悬停时显示。</param>
        /// <remarks>
        /// 该方法与 <see cref="gui_state_displayer_text"/> 功能相同，但使用 <see cref="GUILayout"/> 自动布局系统。
        /// 内部使用水平分组容器，左侧显示主标题（左对齐），右侧显示副标题（右对齐），中间通过 <see cref="layout_flexspace"/> 填充剩余空间。
        /// <para>
        /// 适用于显示状态信息、统计数值或键值对数据。
        /// </para>
        /// </remarks>
        public static void layout_state_displayer_text(string title = null, XGUIFontSize title_size = XGUIFontSize.M, Color title_color = default, string subtitle = null, XGUIFontSize subtitle_size = XGUIFontSize.M, Color subtitle_color = default, RectOffset padding = null, RectOffset margin = null, string subtitle_tooltip = null, Action<string> act_clicked = null)
        {
            if (title_color == Color.clear)
                title_color = Color.white * 0.9f;

            if (padding == null)
                padding = new RectOffset(0, 0, 0, 0);

            if (margin == null)
                margin = new RectOffset(0, 0, 0, 0);

            XGUI.layout_group_start(
                   type: XGUIContainerType.Horizontal,
                   absolute_margin: true,
                   absolute_padding: true,
                   margin: margin,
                   padding: new RectOffset(padding.left + 5, padding.right + 5, padding.top + 0, padding.bottom + 5));

            XGUI.layout_label(
                text: title,
                size: title_size,
                text_color: title_color,
                margin: new RectOffset(15, 0, 0, 0),
                clipping: TextClipping.Clip,
                font_style: FontStyle.Normal,
                //font: XGUI.GetFont("xg-regular"),
                anchor: TextAnchor.MiddleLeft);

            XGUI.layout_flexspace();

            XGUI.layout_label(
                text: subtitle,
                size: subtitle_size,
                text_color: subtitle_color,
                margin: new RectOffset(15, 0, 0, 0),
                clipping: TextClipping.Clip,
                font_style: FontStyle.Normal,
                anchor: TextAnchor.MiddleRight,
                tooltip: subtitle_tooltip);

            Rect rect_clicked = XGUI.GetLastRect();

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);

            #region 右键菜单
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.alt && rect_clicked.Contains(Event.current.mousePosition))
            {
                if (act_clicked != null)
                {
                    act_clicked(subtitle);
                }
            }
            #endregion
        }
        /// <summary>
        /// 使用自动布局绘制一个带有图标的标题状态展示器。
        /// </summary>
        /// <param name="title">标题文本。</param>
        /// <param name="title_size">标题的字体大小。</param>
        /// <param name="title_color">标题的颜色。传入 <see cref="Color.clear"/> 时使用半透明白色。</param>
        /// <param name="icon">右侧显示的图标纹理。</param>
        /// <param name="icon_size">图标尺寸。为 <see cref="Vector2.zero"/> 时使用图标原始尺寸。</param>
        /// <param name="icon_offset">图标的偏移量。</param>
        /// <param name="icon_color">图标的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="padding">展示器的内边距。</param>
        /// <param name="margin">展示器的外边距。</param>
        /// <remarks>
        /// 该方法与 <see cref="gui_state_displayer_icon"/> 功能相同，但使用 <see cref="GUILayout"/> 自动布局系统。
        /// 内部使用水平分组容器，左侧显示标题文本（左对齐），右侧显示图标，中间通过 <see cref="layout_flexspace"/> 填充剩余空间。
        /// </remarks>
        public static void layout_state_displayer_icon(string title = null, XGUIFontSize title_size = XGUIFontSize.M, Color title_color = default, Texture2D icon = null, Vector2 icon_size = default, Vector2 icon_offset = default, Color icon_color = default, RectOffset padding = null, RectOffset margin = null, Action<string, Texture2D> act_clicked = null)
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

            XGUI.layout_group_start(
                   type: XGUIContainerType.Horizontal,
                   absolute_margin: true,
                   absolute_padding: true,
                   margin: margin,
                   padding: new RectOffset(padding.left + 5, padding.right + 0, padding.top + 0, padding.bottom + 5));

            XGUI.layout_label(
                text: title,
                size: title_size,
                text_color: title_color,
                margin: new RectOffset(15, 0, 0, 0),
                clipping: TextClipping.Clip,
                font_style: FontStyle.Normal,
                anchor: TextAnchor.MiddleLeft);

            XGUI.layout_flexspace();

            // 图标
            XGUI.layout_icon(
                icon: icon,
                icon_color: icon_color,
                width: icon_size.x == 0 ? icon.width : icon_size.x,
                height: icon_size.y == 0 ? icon.height : icon_size.y,
                icon_offset: icon_offset,
                icon_margin: new RectOffset(0, 5, 4, 0),
                icon_padding: new RectOffset(0, 0, 0, 0),
                icon_alignment: XGUIIconAlignment.默认);

            Rect rect_clicked = XGUI.GetLastRect();

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);

            #region 右键菜单
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.alt && rect_clicked.Contains(Event.current.mousePosition))
            {
                if (act_clicked != null)
                {
                    act_clicked(title_color.ToString(), icon);
                }
            }
            #endregion
        }
        /// <summary>
        /// 使用自动布局绘制一个带有操作按钮的标题状态展示器。
        /// </summary>
        /// <param name="title">标题文本。</param>
        /// <param name="title_size">标题的字体大小。</param>
        /// <param name="title_color">标题的颜色。传入 <see cref="Color.clear"/> 时使用半透明白色。</param>
        /// <param name="btn_bg">按钮正常状态的填充样式。</param>
        /// <param name="btn_color">按钮正常状态的颜色主题。</param>
        /// <param name="btn_text">按钮上显示的文本。</param>
        /// <param name="btn_tooltip">按钮的提示文本。</param>
        /// <param name="btn_text_size">按钮文本的字体大小。</param>
        /// <param name="btn_width">按钮的固定宽度。为 0 时使用样式默认宽度。</param>
        /// <param name="btn_layout_width">按钮的布局最大宽度限制（<see cref="GUILayout.MaxWidth"/>）。为 0 时不限制。</param>
        /// <param name="btn_gui_color">按钮背景的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="padding">展示器的内边距。</param>
        /// <param name="margin">展示器的外边距。</param>
        /// <param name="callback_clicked">按钮点击时触发的回调委托。</param>
        /// <remarks>
        /// 该方法与 <see cref="gui_state_displayer_btn"/> 功能相同，但使用 <see cref="GUILayout"/> 自动布局系统。
        /// 内部使用水平分组容器，左侧显示标题文本（左对齐），右侧显示操作按钮，中间通过 <see cref="layout_flexspace"/> 填充剩余空间。
        /// <para>
        /// 按钮文本颜色会根据背景亮度自动调整（亮背景使用黑色文本，暗背景使用白色文本），确保可读性。
        /// </para>
        /// </remarks>
        public static void layout_state_displayer_btn(string title = null, XGUIFontSize title_size = XGUIFontSize.M, Color title_color = default, XGUIFilled btn_bg = XGUIFilled.实体, XGUIColor btn_color = XGUIColor.亮白, string btn_text = null, string btn_tooltip = null, XGUIFontSize btn_text_size = XGUIFontSize.M, float btn_width = 0, float btn_layout_width = 0, Color btn_gui_color = default, RectOffset padding = null, RectOffset margin = null, Action callback_clicked = null)
        {
            if (title_color == Color.clear)
                title_color = Color.white * 0.9f;

            if (btn_gui_color == Color.clear)
                btn_gui_color = Color.white;

            if (padding == null)
                padding = new RectOffset(0, 0, 0, 0);

            if (margin == null)
                margin = new RectOffset(0, 0, 0, 0);

            XGUI.layout_group_start(
                   type: XGUIContainerType.Horizontal,
                   absolute_margin: true,
                   absolute_padding: true,
                   margin: margin,
                   padding: new RectOffset(padding.left + 5, padding.right + 0, padding.top + 5, padding.bottom + 5));

            XGUI.layout_label(
                text: title,
                size: title_size,
                text_color: title_color,
                offset: new Vector2(0, -2),
                margin: new RectOffset(15, 0, 0, 0),
                clipping: TextClipping.Clip,
                font_style: FontStyle.Normal,
                anchor: TextAnchor.MiddleLeft);

            XGUI.layout_flexspace();

            if (XGUI.layout_button(
                text: btn_text,
                tooltip: btn_tooltip,
                bg_fill: btn_bg,
                bg_color: btn_color,
                bg_color_gui: btn_gui_color,
                button_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(btn_gui_color) ? Color.black : Color.white,
                press_fill: XGUIFilled.实体,
                press_color: XGUIColor.深空灰,
                press_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(btn_gui_color) ? Color.white : Color.black,
                font_size: btn_text_size,
                anchor: TextAnchor.MiddleCenter,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(5, 5, 0, 0),
                width: btn_width,
                layout_width: btn_layout_width,
                height: XGUI.GetSingleLineHeight(),
                button_text_font: XGUI.GetFont("xg-medium"),
                focus_name: "state_displayer_" + btn_text))
            {
                if (callback_clicked != null)
                    callback_clicked();
            }

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
        }
    }
}