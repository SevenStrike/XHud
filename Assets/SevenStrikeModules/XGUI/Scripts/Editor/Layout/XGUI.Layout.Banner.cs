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
        /// 绘制一个水平布局的横幅（Banner），包含图标和标题。
        /// </summary>
        /// <param name="bg_fill">横幅背景的填充样式。</param>
        /// <param name="bg_color">横幅背景的颜色主题。</param>
        /// <param name="bg_height">横幅的固定高度。为 0 时使用样式默认高度。</param>
        /// <param name="bg_margin">横幅的外边距。</param>
        /// <param name="icon">横幅左侧显示的图标纹理。为 <c>null</c> 时不显示图标。</param>
        /// <param name="icon_color">图标的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="icon_offset">图标的偏移量。</param>
        /// <param name="icon_margin">图标的外边距。</param>
        /// <param name="icon_padding">图标的内边距。</param>
        /// <param name="title_text">横幅的标题文本。</param>
        /// <param name="title_anchor">标题文本的对齐方式。</param>
        /// <param name="title_style">标题文本的字体样式（正常、粗体、斜体等）。</param>
        /// <param name="title_color">标题文本的颜色。传入 <see cref="Color.clear"/> 时使用默认颜色。</param>
        /// <param name="title_size">标题文本的字体大小。</param>
        /// <param name="title_clipping">标题文本的裁剪方式。</param>
        /// <param name="title_offset">标题文本的偏移量。</param>
        /// <remarks>
        /// 该方法使用 <see cref="layout_group_start"/> 创建水平容器，内部包含图标和标题两个元素。
        /// 横幅通常用于页眉区域，用于展示功能模块的名称和标识图标。
        /// <para>
        /// 布局结构：
        /// <list type="bullet">
        /// <item><description>水平分组容器（带背景）</description></item>
        /// <item><description>左侧图标（使用 <see cref="layout_icon"/>）</description></item>
        /// <item><description>右侧标题文本（使用 <see cref="layout_label"/>，字体为 "xg-bold"）</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// XGUI.layout_banner(
        ///     bg_fill: XGUIFilled.实体,
        ///     bg_color: XGUIColor.深空灰,
        ///     icon: XGUI.GetBasedIcon("icon_settings"),
        ///     icon_color: Color.cyan,
        ///     title_text: "项目设置",
        ///     title_color: Color.white,
        ///     title_size: XGUIFontSize.L
        /// );
        /// </code>
        /// </example>
        public static void layout_banner(XGUIFilled bg_fill = XGUIFilled.实体, XGUIColor bg_color = XGUIColor.深空灰, float bg_height = 0, RectOffset bg_margin = default, Texture2D icon = null, Color icon_color = default, Vector2 icon_offset = default, RectOffset icon_margin = default, RectOffset icon_padding = default, string title_text = null, TextAnchor title_anchor = TextAnchor.MiddleLeft, FontStyle title_style = FontStyle.Normal, Color title_color = default, XGUIFontSize title_size = XGUIFontSize.B, TextClipping title_clipping = TextClipping.Clip, Vector2 title_offset = default)
        {
            if (bg_margin == null)
                bg_margin = new RectOffset(0, 0, 0, 0);

            if (GUICreator == null)
            {
                Gui_Layout_Initia();
            }

            // 水平容器
            XGUI.layout_group_start(
                type: XGUIContainerType.Horizontal,
                bg_fill: bg_fill,
                bg_color: bg_color,
                bg_height: bg_height,
                absolute_margin: true,
                absolute_padding: true,
                padding: new RectOffset(15, 0, 0, 0),
                margin: bg_margin);

            if (icon != null)
            {
                // 图标
                XGUI.layout_icon(
                    icon: icon,
                    icon_color: icon_color,
                    icon_size: 16,
                    icon_offset: new Vector2(icon_offset.x, icon_offset.y),
                    icon_margin: new RectOffset(0, 0, 6, 0),
                    icon_padding: icon_padding,
                    icon_alignment: XGUIIconAlignment.默认);
            }

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
                padding: new RectOffset(15, 10, 0, 0),
                margin: new RectOffset(0, 0, 5, 0),
                clipping: title_clipping,
                font: XGUI.GetFont("xg-bold"),
                font_style: title_style);

            //XGUI.gui_box(XGUI.GetLastRect(), Color.red * 0.5f);

            XGUI.layout_group_end(XGUIContainerType.Horizontal);
        }
    }
}