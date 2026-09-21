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
        /// 使用自动布局绘制一个带有标签和状态图标的滑块（基于 SerializedProperty）。
        /// </summary>
        /// <param name="title">滑块左侧显示的标签文本。为 <c>null</c> 或空时不显示标签。</param>
        /// <param name="title_size">标签文本的字体大小。</param>
        /// <param name="title_anchor">标签文本的对齐方式。</param>
        /// <param name="title_color">标签文本的颜色。传入 <see cref="Color.clear"/> 时使用半透明白色。</param>
        /// <param name="title_width">标签的固定宽度。为 0 时自动计算宽度。</param>
        /// <param name="status_icon">状态图标的名称。为 <c>null</c> 时不显示图标。</param>
        /// <param name="status_icon_color">状态图标的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="prop">包含当前浮点值的序列化属性。</param>
        /// <param name="left">滑块的最小值。</param>
        /// <param name="right">滑块的最大值。</param>
        /// <param name="prop_margin">滑块的外边距，覆盖样式默认值。</param>
        /// <param name="prop_padding">滑块的内边距，覆盖样式默认值。</param>
        /// <returns>用户调整后的浮点数值。</returns>
        /// <remarks>
        /// 该方法与 <see cref="gui_slider"/> 功能相同，但使用 <see cref="EditorGUILayout"/> 自动布局系统。
        /// 内部使用水平分组容器将标签和滑块并排显示，并支持在滑块右侧显示状态图标。
        /// <para>
        /// 标签字体固定使用 "xg-medium"，以保持界面一致性。
        /// </para>
        /// </remarks>
        public static float layout_slider(string title = null, XGUIFontSize title_size = XGUIFontSize.M, TextAnchor title_anchor = TextAnchor.MiddleLeft, Color title_color = default, float title_width = 0, string status_icon = null, Color status_icon_color = default, SerializedProperty prop = null, float left = 0, float right = 1, RectOffset prop_margin = default, RectOffset prop_padding = default)
        {
            if (title_color == Color.clear)
                title_color = Color.white * 0.9f;

            if (prop_margin == null)
                prop_margin = new RectOffset(0, 0, 0, 0);

            if (prop_padding == null)
                prop_padding = new RectOffset(0, 0, 0, 0);

            XGUI.layout_group_start(
               type: XGUIContainerType.Horizontal,
               absolute_margin: true,
               margin: prop_margin,
               padding: prop_padding);

            #region 标题
            XGUI.layout_label(
                text: title,
                size: title_size,
                text_color: title_color,
                margin: new RectOffset(0, 0, 0, 0),
                offset: new Vector2(0, 1),
                clipping: TextClipping.Clip,
                font_style: FontStyle.Normal,
                width: title_width,
                anchor: title_anchor,
                font: XGUI.GetFont("xg-medium"));
            #endregion

            EditorGUILayout.Slider(prop, left, right, GUIContent.none, GUILayout.MinWidth(0));

            prop.serializedObject.ApplyModifiedProperties();

            if (status_icon != null)
            {
                XGUI.layout_icon(
                    icon: XGUI.GetBasedIcon(status_icon),
                    width: 5,
                    height: 5,
                    icon_offset: new Vector2(0, 9),
                    icon_color: status_icon_color,
                    icon_margin: new RectOffset(10, 0, 0, 0),
                    icon_alignment: XGUIIconAlignment.默认);
            }

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);

            return prop.floatValue;
        }
        /// <summary>
        /// 使用自动布局绘制一个带有标签和状态图标的滑块（基于 SerializedProperty）。
        /// </summary>
        /// <param name="title">滑块左侧显示的标签文本。为 <c>null</c> 或空时不显示标签。</param>
        /// <param name="title_size">标签文本的字体大小。</param>
        /// <param name="title_anchor">标签文本的对齐方式。</param>
        /// <param name="title_color">标签文本的颜色。传入 <see cref="Color.clear"/> 时使用半透明白色。</param>
        /// <param name="title_width">标签的固定宽度。为 0 时自动计算宽度。</param>
        /// <param name="status_icon">状态图标的名称。为 <c>null</c> 时不显示图标。</param>
        /// <param name="status_icon_color">状态图标的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="prop">包含当前浮点值的序列化属性。</param>
        /// <param name="left">滑块的最小值。</param>
        /// <param name="right">滑块的最大值。</param>
        /// <param name="prop_margin">滑块的外边距，覆盖样式默认值。</param>
        /// <param name="prop_padding">滑块的内边距，覆盖样式默认值。</param>
        /// <returns>用户调整后的浮点数值。</returns>
        /// <remarks>
        /// 该方法与 <see cref="gui_slider"/> 功能相同，但使用 <see cref="EditorGUILayout"/> 自动布局系统。
        /// 内部使用水平分组容器将标签和滑块并排显示，并支持在滑块右侧显示状态图标。
        /// <para>
        /// 标签字体固定使用 "xg-medium"，以保持界面一致性。
        /// </para>
        /// </remarks>
        public static int layout_slider_int(string title = null, XGUIFontSize title_size = XGUIFontSize.M, TextAnchor title_anchor = TextAnchor.MiddleLeft, Color title_color = default, float title_width = 0, string status_icon = null, Color status_icon_color = default, SerializedProperty prop = null, int left = 0, int right = 1, RectOffset prop_margin = default, RectOffset prop_padding = default)
        {
            if (title_color == Color.clear)
                title_color = Color.white * 0.9f;

            if (prop_margin == null)
                prop_margin = new RectOffset(0, 0, 0, 0);

            if (prop_padding == null)
                prop_padding = new RectOffset(0, 0, 0, 0);

            XGUI.layout_group_start(
               type: XGUIContainerType.Horizontal,
               absolute_margin: true,
               margin: prop_margin,
               padding: prop_padding);

            #region 标题
            XGUI.layout_label(
                text: title,
                size: title_size,
                text_color: title_color,
                margin: new RectOffset(0, 0, 0, 0),
                offset: new Vector2(0, 1),
                clipping: TextClipping.Clip,
                font_style: FontStyle.Normal,
                width: title_width,
                anchor: title_anchor,
                font: XGUI.GetFont("xg-medium"));
            #endregion

            EditorGUILayout.IntSlider(prop, left, right, GUIContent.none, GUILayout.MinWidth(0));

            prop.serializedObject.ApplyModifiedProperties();

            if (status_icon != null)
            {
                XGUI.layout_icon(
                    icon: XGUI.GetBasedIcon(status_icon),
                    width: 5,
                    height: 5,
                    icon_offset: new Vector2(0, 9),
                    icon_color: status_icon_color,
                    icon_margin: new RectOffset(10, 0, 0, 0),
                    icon_alignment: XGUIIconAlignment.默认);
            }

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);

            return prop.intValue;
        }
        /// <summary>
        /// 使用自动布局绘制一个带有标签、最小值和最大值输入框的范围滑块（MinMaxSlider）。
        /// </summary>
        /// <param name="ref_min">范围最小值的引用（输入/输出）。</param>
        /// <param name="ref_max">范围最大值的引用（输入/输出）。</param>
        /// <param name="title">滑块左侧显示的标签文本。为 <c>null</c> 或空时不显示标签。</param>
        /// <param name="title_size">标签文本的字体大小。</param>
        /// <param name="title_anchor">标签文本的对齐方式。</param>
        /// <param name="title_color">标签文本的颜色。传入 <see cref="Color.clear"/> 时使用半透明白色。</param>
        /// <param name="title_width">标签的固定宽度。为 0 时自动计算宽度。</param>
        /// <param name="status_icon">状态图标的名称。为 <c>null</c> 时不显示图标。</param>
        /// <param name="status_icon_color">状态图标的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="control_limite">显示数值输入框的窗口宽度阈值。当前窗口宽度大于该值时显示独立的最小/最大输入框，否则显示合并的文本摘要。</param>
        /// <param name="min_field_title_color">最小值输入框标签的颜色。传入 <see cref="Color.clear"/> 时使用半透明白色。</param>
        /// <param name="max_field_title_color">最大值输入框标签的颜色。传入 <see cref="Color.clear"/> 时使用半透明白色。</param>
        /// <param name="min_field_color">最小值输入框文本的颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="max_field_color">最大值输入框文本的颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="shrink_text_color">窄屏模式下合并摘要文本的颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="prop_margin">滑块的外边距，覆盖样式默认值。</param>
        /// <param name="prop_padding">滑块的内边距，覆盖样式默认值。</param>
        /// <param name="min_limite">范围的最小界限值。</param>
        /// <param name="max_limite">范围的最大界限值。</param>
        /// <returns>包含最小值和最大值的 <see cref="xgui_minmax_value"/> 结构体。</returns>
        /// <remarks>
        /// 该方法与 <see cref="gui_slider_min_max"/> 功能相同，但使用 <see cref="EditorGUILayout"/> 自动布局系统。
        /// <para>
        /// 布局结构：
        /// <list type="number">
        /// <item><description>第一行（水平布局）：标签 + MinMaxSlider + 状态图标</description></item>
        /// <item><description>第二行（水平布局）：</description></item>
        /// <item><description>   - 宽屏模式：显示"最小"标签 + 最小值输入框 + 间隔 + "最大"标签 + 最大值输入框</description></item>
        /// <item><description>   - 窄屏模式：显示合并的摘要文本，如 "最小: 0.00 | 最大: 100.00"</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        public static xgui_minmax_value layout_slider_min_max(ref float ref_min, ref float ref_max, string title = null, XGUIFontSize title_size = XGUIFontSize.M, TextAnchor title_anchor = TextAnchor.MiddleLeft, Color title_color = default, float title_width = 0, string status_icon = null, Color status_icon_color = default, float control_limite = 210, Color min_field_title_color = default, Color max_field_title_color = default, Color min_field_color = default, Color max_field_color = default, Color shrink_text_color = default, RectOffset prop_margin = default, RectOffset prop_padding = default, float min_limite = 0, float max_limite = 100)
        {
            if (title_color == Color.clear)
                title_color = Color.white * 0.9f;

            if (min_field_color == Color.clear)
                min_field_color = Color.white * 0.9f;

            if (max_field_color == Color.clear)
                max_field_color = Color.white * 0.9f;

            if (shrink_text_color == Color.clear)
                shrink_text_color = Color.white;

            if (min_field_title_color == Color.clear)
                min_field_title_color = Color.white * 0.9f;

            if (max_field_title_color == Color.clear)
                max_field_title_color = Color.white * 0.9f;

            if (prop_margin == null)
                prop_margin = new RectOffset(0, 0, 0, 0);

            if (prop_padding == null)
                prop_padding = new RectOffset(0, 0, 0, 0);

            XGUI.layout_group_start(
              type: XGUIContainerType.Horizontal,
              absolute_margin: true,
              margin: prop_margin,
              padding: prop_padding);

            #region 标题
            XGUI.layout_label(
                text: title,
                size: title_size,
                text_color: title_color,
                margin: new RectOffset(0, 0, 0, 0),
                offset: new Vector2(0, 1),
                clipping: TextClipping.Clip,
                font_style: FontStyle.Normal,
                width: title_width,
                anchor: title_anchor,
                font: XGUI.GetFont("xg-medium"));
            #endregion

            EditorGUILayout.MinMaxSlider(GUIContent.none, ref ref_min, ref ref_max, min_limite, max_limite, GUILayout.MinWidth(0));

            if (status_icon != null)
            {
                XGUI.layout_icon(
                    icon: XGUI.GetBasedIcon(status_icon),
                    width: 5,
                    height: 5,
                    icon_offset: new Vector2(0, 9),
                    icon_color: status_icon_color,
                    icon_margin: new RectOffset(10, 0, 0, 0),
                    icon_alignment: XGUIIconAlignment.默认);
            }

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);

            XGUI.layout_group_start(
              type: XGUIContainerType.Horizontal,
              absolute_margin: true,
              margin: prop_margin,
              padding: prop_padding);

            if (XGUI.GetCurrentWindowWidth() > control_limite)
            {
                XGUI.layout_label(
                    text: "最小",
                    size: title_size,
                    text_color: min_field_title_color,
                    margin: new RectOffset(0, 0, 0, 0),
                    offset: new Vector2(0, 1),
                    clipping: TextClipping.Clip,
                    font_style: FontStyle.Normal,
                    width: title_width,
                    anchor: title_anchor);

                ref_min = XGUI.layout_inputfield(
                    prop: ref_min,
                    field_fontsize: XGUIFontSize.M,
                    field_text_offset: Vector2.zero,
                    field_height: 20,
                    field_padding: new RectOffset(0, 0, 0, 0),
                    field_margin: new RectOffset(0, 0, 0, 0),
                    field_text_color: min_field_color,
                    field_text_font: XGUI.GetFont("xg-medium"),
                    field_text_style: FontStyle.Normal,
                    field_text_anchor: TextAnchor.MiddleCenter);

                XGUI.layout_space(10);

                XGUI.layout_label(
                    text: "最大",
                    size: title_size,
                    text_color: max_field_title_color,
                    margin: new RectOffset(0, 0, 0, 0),
                    offset: new Vector2(0, 1),
                    clipping: TextClipping.Clip,
                    font_style: FontStyle.Normal,
                    width: title_width,
                    anchor: title_anchor);

                ref_max = XGUI.layout_inputfield(
                    prop: ref_max,
                    field_fontsize: XGUIFontSize.M,
                    field_text_offset: Vector2.zero,
                    field_height: 20,
                    field_padding: new RectOffset(0, 0, 0, 0),
                    field_margin: new RectOffset(0, 0, 0, 0),
                    field_text_color: max_field_color,
                    field_text_font: XGUI.GetFont("xg-medium"),
                    field_text_style: FontStyle.Normal,
                    field_text_anchor: TextAnchor.MiddleCenter);
            }
            else
            {
                string v = $"最小: {ref_min.ToString("F2")}   |   最大: {ref_max.ToString("F2")}";
                XGUI.layout_label(
                    text: v,
                    size: XGUIFontSize.M,
                    text_color: shrink_text_color,
                    wrap: true,
                    offset: new Vector2(0, -2),
                    margin: new RectOffset(15, 0, 0, 0),
                    clipping: TextClipping.Overflow,
                    font_style: FontStyle.Normal,
                    anchor: TextAnchor.MiddleCenter);
            }
            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);

            return new xgui_minmax_value(ref_min, ref_max);
        }
    }
}