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
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// 为 Editor 界面提供样式控件
    /// </summary>
    public static partial class XGUI
    {
        /// <summary>
        /// 绘制一个带有标签和状态图标的单值滑块。
        /// </summary>
        /// <param name="rect">滑块的位置和尺寸。</param>
        /// <param name="title">滑块左侧显示的标签文本。为 <c>null</c> 或空时不显示标签。</param>
        /// <param name="title_size">标签文本的字体大小。</param>
        /// <param name="title_anchor">标签文本的对齐方式。</param>
        /// <param name="title_color">标签文本的颜色。传入 <see cref="Color.clear"/> 时使用半透明白色。</param>
        /// <param name="title_width">标签的固定宽度。为 0 时自动计算宽度。</param>
        /// <param name="status_icon">状态图标的名称（通过 <see cref="GetBasedIcon"/> 加载）。为 <c>null</c> 时不显示图标。</param>
        /// <param name="status_icon_color">状态图标的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="prop">滑块的当前值。</param>
        /// <param name="left">滑块的最小值。</param>
        /// <param name="right">滑块的最大值。</param>
        /// <param name="slider_height">滑块的高度。为 0 时使用默认行高。</param>
        /// <param name="limite_width">显示标签的窗口宽度阈值。当前窗口宽度大于该值时显示标签，否则隐藏。</param>
        /// <returns>用户调整后的浮点数值。</returns>
        /// <remarks>
        /// 该方法使用 <see cref="EditorGUI.Slider"/> 绘制滑块控件，支持在滑块左侧显示自定义标签，
        /// 并在右侧显示状态图标。标签在窗口宽度不足时会自动隐藏以适应窄屏布局。
        /// </remarks>
        /// <example>
        /// <code>
        /// Rect sliderRect = new Rect(10, 10, 300, 20);
        /// float value = 0.5f;
        /// value = XGUI.gui_slider(
        ///     rect: sliderRect,
        ///     title: "音量",
        ///     title_width: 60,
        ///     prop: value,
        ///     left: 0f,
        ///     right: 1f,
        ///     status_icon: "icon_volume"
        /// );
        /// </code>
        /// </example>
        public static float gui_slider(Rect rect = default, string title = null, XGUIFontSize title_size = XGUIFontSize.M, TextAnchor title_anchor = TextAnchor.MiddleLeft, Color title_color = default, float title_width = 0, Vector2 title_offset = default, string status_icon = null, Color status_icon_color = default, float prop = 0, float left = 0, float right = 1, float slider_height = 20, float limite_width = 230)
        {
            if (title_color == Color.clear)
                title_color = Color.white * 0.9f;

            bool exp = false;
            if (XGUI.GetCurrentWindowWidth() > limite_width)
                exp = true;
            else
                exp = false;

            // 测试区域
            //XGUI.gui_box(rect, Color.red * 0.5f);

            if (!string.IsNullOrEmpty(title) && exp)
            {
                #region 标题
                Rect rect_title = new Rect(rect.x, rect.y, exp ? title_width : rect.width * 0.1f, slider_height);
                XGUI.gui_label(
                    rect: rect_title,
                    text: new GUIContent(title),
                    text_color: title_color,
                    size: title_size,
                    clipping: TextClipping.Overflow,
                    anchor: title_anchor,
                    offset: title_offset,
                    wrap: false,
                    font_style: FontStyle.Normal);
                #endregion
            }

            float field_x = exp ? (!string.IsNullOrEmpty(title) ? rect.x + title_width + 10 : rect.x) : rect.x;
            float field_y = rect.y;
            float field_w = exp ? (!string.IsNullOrEmpty(title) ? rect.width - (title_width + 10 + (status_icon != null ? 10 : 0)) : rect.width - (status_icon != null ? 10 : 0)) : rect.width - (status_icon != null ? 10 : 0);
            float field_h = XGUI.GetSingleLineHeight();
            Rect rect_field = new Rect(field_x, field_y, field_w, field_h);

            prop = EditorGUI.Slider(rect_field, GUIContent.none, prop, left, right);

            if (status_icon != null)
            {
                float x = rect.x + rect.width;
                float y = rect.y + 6;
                float w = 5;
                float h = 5;

                Rect rect_icon = new Rect(x, y, w, h);

                XGUI.gui_icon(
                    rect: rect_icon,
                    icon: XGUI.GetBasedIcon(status_icon),
                    color: status_icon_color);
            }

            return prop;
        }
        /// <summary>
        /// 绘制一个带有标签、最小值和最大值输入框的范围滑块（MinMaxSlider）。
        /// </summary>
        /// <param name="ref_min">范围最小值的引用（输入/输出）。</param>
        /// <param name="ref_max">范围最大值的引用（输入/输出）。</param>
        /// <param name="rect">滑块的位置和尺寸。</param>
        /// <param name="title">滑块左侧显示的标签文本。为 <c>null</c> 或空时不显示标签。</param>
        /// <param name="title_size">标签文本的字体大小。</param>
        /// <param name="title_anchor">标签文本的对齐方式。</param>
        /// <param name="title_color">标签文本的颜色。传入 <see cref="Color.clear"/> 时使用半透明白色。</param>
        /// <param name="title_width">标签的固定宽度。为 0 时自动计算宽度。</param>
        /// <param name="status_icon">状态图标的名称。为 <c>null</c> 时不显示图标。</param>
        /// <param name="status_icon_color">状态图标的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="limite_width">显示标签的窗口宽度阈值。</param>
        /// <param name="slider_height">滑块的高度。为 0 时使用默认行高。</param>
        /// <param name="min_limite">范围的最小界限值。</param>
        /// <param name="max_limite">范围的最大界限值。</param>
        /// <param name="min_field_text">最小值输入框的标签文本。</param>
        /// <param name="max_field_text">最大值输入框的标签文本。</param>
        /// <returns>包含最小值和最大值的 <see cref="xgui_minmax_value"/> 结构体。</returns>
        /// <remarks>
        /// 该控件结合了 <see cref="EditorGUI.MinMaxSlider"/> 和两个数值输入框，用户既可以通过拖拽滑块调整范围，
        /// 也可以直接在输入框中输入精确数值。适用于调整取值范围（如颜色通道、音效频率等）的场景。
        /// <para>
        /// 布局结构：
        /// <list type="bullet">
        /// <item><description>第一行：标签 + MinMaxSlider + 状态图标</description></item>
        /// <item><description>第二行：最小值输入框 + 最大值输入框</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// float minVal = 0f;
        /// float maxVal = 100f;
        /// Rect sliderRect = new Rect(10, 10, 300, 20);
        /// 
        /// xtween_minmax_value range = XGUI.gui_slider_min_max(
        ///     ref minVal,
        ///     ref maxVal,
        ///     rect: sliderRect,
        ///     title: "范围",
        ///     title_width: 60,
        ///     min_limite: 0f,
        ///     max_limite: 200f,
        ///     min_field_text: "最小",
        ///     max_field_text: "最大"
        /// );
        /// // 使用 range.min 和 range.max
        /// </code>
        /// </example>
        public static xgui_minmax_value gui_slider_min_max(ref float ref_min, ref float ref_max, Rect rect = default, string title = null, XGUIFontSize title_size = XGUIFontSize.M, TextAnchor title_anchor = TextAnchor.MiddleLeft, Color title_color = default, float title_width = 0, string status_icon = null, Color status_icon_color = default, float limite_width = 230, float slider_height = 20, float min_limite = 0, float max_limite = 100, string min_field_text = "最小", string max_field_text = "最大", bool displaystate = true)
        {
            if (title_color == Color.clear)
                title_color = Color.white * 0.9f;
            bool exp = false;
            if (XGUI.GetCurrentWindowWidth() > limite_width)
                exp = true;
            else
                exp = false;

            // 测试区域
            //XGUI.gui_box(rect, Color.red * 0.5f);

            if (!string.IsNullOrEmpty(title) && exp)
            {
                #region 标题
                Rect rect_title = new Rect(rect.x, rect.y, exp ? title_width : rect.width * 0.1f, slider_height);
                XGUI.gui_label(
                    rect: rect_title,
                    text: new GUIContent(title),
                    text_color: title_color,
                    size: title_size,
                    clipping: TextClipping.Overflow,
                    anchor: title_anchor,
                    offset: new Vector2(0, 0),
                    wrap: false,
                    font_style: FontStyle.Normal);
                #endregion
            }

            float field_x = exp ? (!string.IsNullOrEmpty(title) ? rect.x + title_width + 10 : rect.x) : rect.x;
            float field_y = rect.y;
            float field_w = exp ? (!string.IsNullOrEmpty(title) ? rect.width - (title_width + 10 + (status_icon != null ? 10 : 0)) : rect.width - (status_icon != null ? 10 : 0)) : rect.width - (status_icon != null ? 10 : 0);
            float field_h = XGUI.GetSingleLineHeight();
            Rect rect_field = new Rect(field_x, field_y, field_w, field_h);

            EditorGUI.MinMaxSlider(rect_field, GUIContent.none, ref ref_min, ref ref_max, min_limite, max_limite);

            if (status_icon != null)
            {
                float x = rect.x + rect.width;
                float y = rect.y + 6;
                float w = 5;
                float h = 5;

                Rect rect_icon = new Rect(x, y, w, h);

                XGUI.gui_icon(
                    rect: rect_icon,
                    icon: XGUI.GetBasedIcon(status_icon),
                    color: status_icon_color);
            }

            if (displaystate)
            {
                Rect rect_field_min = new Rect(rect.x, rect.y + slider_height + 5, rect.width / 2 - 10, 20);
                ref_min = XGUI.gui_inputfield(
                    rect: rect_field_min,
                    title: min_field_text,
                    prop: ref_min,
                    field_fontsize: XGUIFontSize.M,
                    field_text_offset: Vector2.zero,
                    field_height: 20,
                    field_text_color: Color.white,
                    title_width: 40,
                    field_text_font: XGUI.GetFont("xg-medium"),
                    field_text_style: FontStyle.Normal,
                    field_text_anchor: TextAnchor.MiddleLeft,
                    //status_icon: "icon_field_status",
                    //status_icon_color: Color.red,
                    field_padding: new RectOffset(5, 5, 0, 0),
                    field_margin: new RectOffset(0, 0, 0, 0));

                Rect rect_field_max = new Rect(rect.x + (rect.width / 2) + 10, rect.y + slider_height + 5, rect.width / 2 - 10, 20);
                ref_max = XGUI.gui_inputfield(
                    rect: rect_field_max,
                    title: max_field_text,
                    prop: ref_max,
                    field_fontsize: XGUIFontSize.M,
                    field_text_offset: Vector2.zero,
                    field_height: 20,
                    field_text_color: Color.white,
                    title_width: 40,
                    field_text_font: XGUI.GetFont("xg-medium"),
                    field_text_style: FontStyle.Normal,
                    field_text_anchor: TextAnchor.MiddleLeft,
                    //status_icon: "icon_field_status",
                    //status_icon_color: Color.red,
                    field_padding: new RectOffset(5, 5, 0, 0),
                    field_margin: new RectOffset(0, 0, 0, 0));
            }

            return new xgui_minmax_value(ref_min, ref_max);
        }
    }
}