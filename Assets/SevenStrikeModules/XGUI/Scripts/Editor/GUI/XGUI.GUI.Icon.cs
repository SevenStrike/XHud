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
    using UnityEngine;

    /// <summary>
    /// 为 Editor 界面提供样式控件
    /// </summary>
    public static partial class XGUI
    {
        /// <summary>
        /// 在指定矩形位置绘制一个图标。
        /// </summary>
        /// <param name="rect">图标的位置和尺寸。</param>
        /// <param name="gui_content">要绘制的图标纹理。为 <c>null</c> 时不显示任何内容。</param>
        /// <param name="padding">图标的内边距，叠加到样式默认内边距上。</param>
        /// <param name="border">图标的边框大小，覆盖样式默认边框。</param>
        /// <param name="color">图标的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色，可用于对图标进行着色。</param>
        /// <remarks>
        /// 该方法基于 <see cref="style_xg_icon"/> 样式绘制图标，支持自定义内边距、边框和颜色叠加。
        /// 绘制完成后会自动恢复 <see cref="GUI.backgroundColor"/> 到调用前的状态。
        /// </remarks>
        /// <example>
        /// <code>
        /// Texture2D myIcon = XGUI.GetBasedIcon("icon_settings");
        /// Rect iconRect = new Rect(10, 10, 32, 32);
        /// XGUI.gui_icon(iconRect, myIcon, color: Color.cyan);
        /// </code>
        /// </example>
        public static void gui_icon(Rect rect, GUIContent gui_content = null, RectOffset padding = default, RectOffset border = default, Color color = default)
        {
            if (color == Color.clear)
                color = Color.white;

            if (padding == null)
                padding = new RectOffset(0, 0, 0, 0);

            if (border == null)
                border = new RectOffset(0, 0, 0, 0);

            GUIStyle style = new GUIStyle(style_xg_icon);
            style.normal.background = (Texture2D)gui_content.image;

            RectOffset _p = style.padding;
            style.padding = new RectOffset(_p.left + padding.left, _p.right + padding.right, _p.top + padding.top, _p.bottom + padding.bottom);

            style.border = border;

            Color bg_col = GUI.backgroundColor;
            GUI.backgroundColor = color;
            GUI.Box(rect, gui_content, style);
            GUI.backgroundColor = bg_col;
        }

        /// <summary>
        /// 在指定矩形位置绘制一个图标。
        /// </summary>
        /// <param name="rect">图标的位置和尺寸。</param>
        /// <param name="icon">要绘制的图标纹理。为 <c>null</c> 时不显示任何内容。</param>
        /// <param name="padding">图标的内边距，叠加到样式默认内边距上。</param>
        /// <param name="border">图标的边框大小，覆盖样式默认边框。</param>
        /// <param name="color">图标的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色，可用于对图标进行着色。</param>
        /// <remarks>
        /// 该方法基于 <see cref="style_xg_icon"/> 样式绘制图标，支持自定义内边距、边框和颜色叠加。
        /// 绘制完成后会自动恢复 <see cref="GUI.backgroundColor"/> 到调用前的状态。
        /// </remarks>
        /// <example>
        /// <code>
        /// Texture2D myIcon = XGUI.GetBasedIcon("icon_settings");
        /// Rect iconRect = new Rect(10, 10, 32, 32);
        /// XGUI.gui_icon(iconRect, myIcon, color: Color.cyan);
        /// </code>
        /// </example>
        public static void gui_icon(Rect rect, Texture2D icon = null, RectOffset padding = default, RectOffset border = default, Color color = default)
        {
            if (color == Color.clear)
                color = Color.white;

            if (padding == null)
                padding = new RectOffset(0, 0, 0, 0);

            if (border == null)
                border = new RectOffset(0, 0, 0, 0);

            GUIStyle style = new GUIStyle(style_xg_icon);
            style.normal.background = icon;

            RectOffset _p = style.padding;
            style.padding = new RectOffset(_p.left + padding.left, _p.right + padding.right, _p.top + padding.top, _p.bottom + padding.bottom);

            style.border = border;

            Color bg_col = GUI.backgroundColor;
            GUI.backgroundColor = color;
            GUI.Box(rect, "", style);
            GUI.backgroundColor = bg_col;
        }
    }
}