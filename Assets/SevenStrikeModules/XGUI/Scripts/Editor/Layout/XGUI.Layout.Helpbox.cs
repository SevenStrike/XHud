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
        /// 使用自动布局绘制一个帮助框（Helpbox），用于显示通知、警告或错误信息。
        /// </summary>
        /// <param name="bg_height">帮助框的固定高度。为 0 时使用样式默认高度。</param>
        /// <param name="state">帮助框的状态类型（通知、警告、错误），决定显示的图标。</param>
        /// <param name="icon_offset">图标的偏移量。</param>
        /// <param name="icon_margin">图标的外边距。</param>
        /// <param name="icon_padding">图标的内边距。</param>
        /// <param name="title_text">帮助框的标题或消息文本。</param>
        /// <param name="title_anchor">文本的对齐方式。</param>
        /// <param name="title_style">文本的字体样式（正常、粗体、斜体等）。</param>
        /// <param name="title_color">文本的颜色。传入 <see cref="Color.clear"/> 时使用默认颜色。</param>
        /// <param name="title_size">文本的字体大小。</param>
        /// <param name="title_clipping">文本的裁剪方式。</param>
        /// <param name="title_offset">文本的偏移量。</param>
        /// <param name="wrap">是否启用文本自动换行。</param>
        /// <remarks>
        /// 该方法使用 <see cref="layout_group_start"/> 创建水平容器，内部包含状态图标和消息文本。
        /// 帮助框通常用于向用户展示操作结果、验证提示或系统通知。
        /// <para>
        /// 布局结构：
        /// <list type="bullet">
        /// <item><description>水平分组容器（带边框背景）</description></item>
        /// <item><description>左侧状态图标（根据 <paramref name="state"/> 自动选择）</description></item>
        /// <item><description>右侧消息文本（字体为 "xg-medium"）</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// XGUI.layout_helpbox(
        ///     state: XGUIHelboxState.警告,
        ///     title_text: "请确保所有必填字段都已填写",
        ///     title_color: Color.yellow,
        ///     wrap: true
        /// );
        /// </code>
        /// </example>
        public static void layout_helpbox(float bg_height = 0, XGUIHelboxState state = XGUIHelboxState.通知, Vector2 icon_offset = default, RectOffset icon_margin = default, RectOffset icon_padding = default, string title_text = null, TextAnchor title_anchor = TextAnchor.MiddleLeft, FontStyle title_style = FontStyle.Normal, Color title_color = default, XGUIFontSize title_size = XGUIFontSize.B, TextClipping title_clipping = TextClipping.Clip, Vector2 title_offset = default, bool wrap = false)
        {
            if (GUICreator == null)
            {
                Gui_Layout_Initia();
            }

            // 水平容器
            XGUI.layout_group_start(
                type: XGUIContainerType.Horizontal,
                bg_fill: XGUIFilled.纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: Color.white * 0.5f,
                bg_height: bg_height,
                absolute_margin: true,
                absolute_padding: false,
                padding: new RectOffset(10, 0, 0, 10),
                margin: new RectOffset(0, 0, 0, 0));

            // 图标
            XGUI.layout_icon(
                icon: XGUI.GetHelpboxStateIcon(state),
                icon_size: 14,
                icon_offset: new Vector2(icon_offset.x, icon_offset.y + 2),
                icon_margin: new RectOffset(0, 10, 6, 0),
                icon_padding: icon_padding,
                icon_alignment: XGUIIconAlignment.默认);

            //XGUI.gui_box(XGUI.GetLastRect(), Color.red * 0.5f);

            // 标题
            XGUI.layout_label(
                text: title_text,
                bg_fill: XGUIFilled.无,
                bg_color: XGUIColor.无,
                size: title_size,
                anchor: title_anchor,
                text_color: title_color,
                offset: new Vector2(title_offset.x, title_offset.y),
                padding: new RectOffset(10, 10, 0, 0),
                margin: new RectOffset(0, 0, 6, 0),
                clipping: title_clipping,
                wrap: wrap,
                font: XGUI.GetFont("xg-medium"),
                font_style: title_style);

            //XGUI.gui_box(XGUI.GetLastRect(), Color.red * 0.5f);

            XGUI.layout_group_end(XGUIContainerType.Horizontal);
        }
    }
}