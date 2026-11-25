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
        /// 绘制一个带有标题、副标题和指示器的自定义进度条。
        /// </summary>
        /// <param name="rect">进度条的布局矩形区域。</param>
        /// <param name="title">进度条左侧显示的标题文本。</param>
        /// <param name="title_width">标题区域的固定宽度。</param>
        /// <param name="title_size">标题文本的字体大小。</param>
        /// <param name="title_offset">标题文本的偏移量。</param>
        /// <param name="title_color">标题文本的颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="subtitle">进度条右侧显示的副标题文本。</param>
        /// <param name="subtitle_width">副标题区域的固定宽度。</param>
        /// <param name="subtitle_size">副标题文本的字体大小。</param>
        /// <param name="subtitle_offset">副标题文本的偏移量。</param>
        /// <param name="subtitle_color">副标题文本的颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="line_left_color">进度条左端竖线的颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="line_center_color">进度条中点竖线的颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="line_right_color">进度条右端竖线的颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="indicator_color">进度指示器（箭头/滑块）的颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="progress_fg_color">进度条前景色（已填充部分）。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="progress_bg_color">进度条背景色（未填充部分）。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="icon_indicator">进度指示器的图标纹理。为 <c>null</c> 时不显示指示器图标。</param>
        /// <param name="value">进度值，取值范围 0 到 1。</param>
        /// <param name="thickness">进度条的粗细（高度）。</param>
        /// <param name="display_arrow">是否显示进度指示器。</param>
        /// <remarks>
        /// 该进度条由多个视觉元素组成：
        /// <list type="bullet">
        /// <item><description>左侧标题和右侧副标题</description></item>
        /// <item><description>进度条背景（完整宽度）和前景（根据 <paramref name="value"/> 填充）</description></item>
        /// <item><description>左端、中点、右端三条竖线标记</description></item>
        /// <item><description>可选的进度指示器（箭头/滑块图标）</description></item>
        /// </list>
        /// 适用于展示任务进度、加载状态或数据占比等场景。
        /// </remarks>
        /// <example>
        /// <code>
        /// Rect progressRect = new Rect(20, 50, 300, 30);
        /// XGUI.gui_progress(
        ///     rect: progressRect,
        ///     title: "加载进度",
        ///     subtitle: "75%",
        ///     value: 0.75f,
        ///     progress_fg_color: Color.green,
        ///     progress_bg_color: new Color(0.2f, 0.2f, 0.2f),
        ///     icon_indicator: XGUI.GetBasedIcon("icon_arrow")
        /// );
        /// </code>
        /// </example>
        public static void gui_progress(Rect rect, string title = null, float title_width = 60, XGUIFontSize title_size = XGUIFontSize.M, Vector2 title_offset = default, Color title_color = default, string subtitle = null, float subtitle_width = 70, XGUIFontSize subtitle_size = XGUIFontSize.M, Vector2 subtitle_offset = default, Color subtitle_color = default, Color line_left_color = default, Color line_center_color = default, Color line_right_color = default, Color indicator_color = default, Color progress_fg_color = default, Color progress_bg_color = default, Texture2D icon_indicator = null, float value = 1, float thickness = 4, bool display_arrow = true)
        {
            if (title_offset == default)
                title_offset = new Vector2(0, 0);

            if (title_color == Color.clear)
                title_color = Color.white;

            if (line_left_color == Color.clear)
                line_left_color = Color.white;

            if (line_center_color == Color.clear)
                line_center_color = Color.white;

            if (line_right_color == Color.clear)
                line_right_color = Color.white;

            if (indicator_color == Color.clear)
                indicator_color = Color.white;

            if (progress_fg_color == Color.clear)
                progress_fg_color = Color.white;

            if (progress_bg_color == Color.clear)
                progress_bg_color = Color.white;

            if (subtitle_offset == default)
                subtitle_offset = new Vector2(0, 0);

            if (subtitle_color == Color.clear)
                subtitle_color = Color.white;

            Rect rect_progress = rect;

            // 背景线
            rect_progress.Set(rect.x, rect.y, rect.width, 1);
            XGUI.gui_box(rect_progress, line_left_color);

            // 进度条背景
            rect_progress.Set(rect.x, rect.y - thickness * 2, rect.width, thickness * 2);
            XGUI.gui_box(rect_progress, progress_bg_color);

            // 进度条
            rect_progress.Set(rect.x, rect.y - thickness, rect.width * value, thickness);
            XGUI.gui_box(rect_progress, progress_fg_color);

            // 标题
            float w_title = title_width;
            rect_progress.Set(rect.x, rect.y - 28, w_title, 20);
            XGUI.gui_label(
                rect: rect_progress,
                text: new GUIContent(title),
                text_color: title_color,
                offset: title_offset,
                anchor: TextAnchor.MiddleLeft,
                size: title_size,
                clipping: TextClipping.Clip);

            // 副标题
            float w_sub = subtitle_width;
            rect_progress.Set(rect.x + rect.width - w_sub, rect.y - 28, w_sub, 20);
            XGUI.gui_label(
                rect: rect_progress,
                text: new GUIContent(subtitle),
                text_color: subtitle_color,
                offset: subtitle_offset,
                anchor: TextAnchor.MiddleRight,
                size: subtitle_size,
                clipping: TextClipping.Clip);

            // 起点线
            rect_progress.Set(rect.x, rect.y - 2, 1, 6);
            gui_box(rect_progress, line_left_color);

            // 终点线
            rect_progress.Set(rect.x + rect.width, rect.y - 2, 1, 6);
            gui_box(rect_progress, line_right_color);

            if (display_arrow)
            {
                // 指示器
                rect_progress.Set(rect.x + (rect.width * value) - 4, rect.y + 3, 8, 8);
                XGUI.gui_icon(rect: rect_progress, icon: icon_indicator, color: indicator_color);
            }

            // 中点线
            rect_progress.Set(rect.x + (rect.width / 2), rect.y - 10, 1, 10);
            gui_box(rect_progress, line_center_color);
        }
    }
}