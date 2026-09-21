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
        /// 使用自动布局绘制一个图标，支持对齐方式和位置控制。
        /// </summary>
        /// <param name="icon">要绘制的图标纹理。</param>
        /// <param name="icon_color">图标的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="icon_size">图标的尺寸（宽度和高度）。</param>
        /// <param name="icon_offset">图标内容的偏移量。</param>
        /// <param name="icon_border">图标的边框大小。</param>
        /// <param name="icon_margin">图标的外边距。</param>
        /// <param name="icon_padding">图标的内边距。</param>
        /// <param name="icon_alignment">图标在容器中的对齐方式（左、右、中心）。</param>
        /// <param name="width">图标的固定宽度。为 0 时使用 <paramref name="icon_size"/>。</param>
        /// <param name="height">图标的固定高度。为 0 时使用 <paramref name="icon_size"/>。</param>
        /// <param name="layout_margin">外层布局容器的外边距（仅在启用对齐时生效）。</param>
        /// <remarks>
        /// 该方法使用 <see cref="GUILayout"/> 自动布局系统，支持图标在水平方向上的对齐控制：
        /// <list type="bullet">
        /// <item><description><see cref="XGUIIconAlignment.左"/>：图标左对齐，右侧填充弹性空间</description></item>
        /// <item><description><see cref="XGUIIconAlignment.右"/>：图标右对齐，左侧填充弹性空间</description></item>
        /// <item><description><see cref="XGUIIconAlignment.中心"/>：图标水平居中，两侧填充弹性空间</description></item>
        /// <item><description><see cref="XGUIIconAlignment.默认"/>：图标按正常流排列，不添加弹性空间</description></item>
        /// </list>
        /// 当启用对齐时，内部会自动创建水平分组容器，并在适当位置插入 <see cref="layout_flexspace"/> 实现弹性布局。
        /// </remarks>
        /// <example>
        /// <code>
        /// Texture2D myIcon = XGUI.GetBasedIcon("icon_settings");
        /// XGUI.layout_icon(
        ///     icon: myIcon,
        ///     icon_color: Color.cyan,
        ///     icon_size: 24,
        ///     icon_alignment: XGUIIconAlignment.中心
        /// );
        /// </code>
        /// </example>
        public static void layout_icon(Texture2D icon, Color icon_color = default, float icon_size = 16, Vector2 icon_offset = default, RectOffset icon_border = default, RectOffset icon_margin = default, RectOffset icon_padding = default, XGUIIconAlignment icon_alignment = XGUIIconAlignment.左, float width = 0, float height = 0, RectOffset layout_margin = default)
        {
            if (icon_offset == default)
                icon_offset = Vector2.zero;

            if (icon_color == Color.clear)
                icon_color = Color.white;

            if (icon_padding == null)
                icon_padding = new RectOffset(0, 0, 0, 0);

            if (icon_margin == null)
                icon_margin = new RectOffset(0, 0, 0, 0);

            if (icon_border == null)
                icon_border = new RectOffset(0, 0, 0, 0);

            if (layout_margin == null)
                layout_margin = new RectOffset(0, 0, 0, 0);

            GUIStyle style = new GUIStyle(style_xg_icon);

            if (width != 0)
            {
                style.fixedWidth = width;
            }

            if (height != 0)
            {
                style.fixedHeight = height;
            }

            if (width == 0 && height == 0)
            {
                if (icon_size != 0)
                {
                    style.fixedWidth = icon_size;
                    style.fixedHeight = icon_size;
                }
            }

            RectOffset _p = style.padding;
            RectOffset _m = style.margin;

            style.padding = new RectOffset(_p.left + icon_padding.left, _p.right + icon_padding.right, _p.top + icon_padding.top, _p.bottom + icon_padding.bottom);

            style.margin = new RectOffset(_m.left + icon_margin.left, _m.right + icon_margin.right, _m.top + icon_margin.top, _m.bottom + icon_margin.bottom);

            style.contentOffset = new Vector2(style.contentOffset.x + icon_offset.x, style.contentOffset.y + icon_offset.y);

            if (icon_alignment == XGUIIconAlignment.右 || icon_alignment == XGUIIconAlignment.左 || icon_alignment == XGUIIconAlignment.中心)
            {
                XGUI.layout_group_start(
                bg_fill: XGUIFilled.无,
                type: XGUIContainerType.Horizontal,
                padding: new RectOffset(0, 0, 0, 0),
                margin: layout_margin,
                absolute_margin: true,
                absolute_padding: true);
            }

            if (icon_alignment == XGUIIconAlignment.右 || icon_alignment == XGUIIconAlignment.中心)
            {
                XGUI.layout_flexspace();
            }

            Color bgcol = GUI.color;
            GUI.color = icon_color;

            GUILayout.Box(icon, style, GUILayout.MinWidth(0));


            GUI.color = bgcol;

            if (icon_alignment == XGUIIconAlignment.左 || icon_alignment == XGUIIconAlignment.中心)
            {
                XGUI.layout_flexspace();
            }

            if (icon_alignment == XGUIIconAlignment.右 || icon_alignment == XGUIIconAlignment.左 || icon_alignment == XGUIIconAlignment.中心)
            {
                XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            }
        }
    }
}