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
        /// 绘制一个带有标签和状态图标的序列化属性字段。
        /// </summary>
        /// <param name="rect">属性字段的布局矩形区域。</param>
        /// <param name="title">属性标签的内容。为 <c>null</c> 时不显示标签。</param>
        /// <param name="title_size">标签文本的字体大小。</param>
        /// <param name="title_anchor">标签文本的对齐方式。</param>
        /// <param name="title_color">标签文本的颜色。传入 <see cref="Color.clear"/> 时使用半透明白色。</param>
        /// <param name="title_width">标签的固定宽度。为 0 时自动计算宽度。</param>
        /// <param name="status_icon">状态图标的名称（通过 <see cref="GetBasedIcon"/> 加载）。为 <c>null</c> 时不显示图标。</param>
        /// <param name="status_icon_color">状态图标的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="prop">要绘制的序列化属性。</param>
        /// <param name="limite_width">显示标签的窗口宽度阈值。当前窗口宽度大于该值时显示标签，否则隐藏。</param>
        /// <param name="title_hover_color">预留参数，暂未使用。</param>
        /// <param name="prop_margin">预留参数，暂未使用。</param>
        /// <param name="prop_padding">预留参数，暂未使用。</param>
        /// <remarks>
        /// 该方法使用 <see cref="EditorGUI.PropertyField"/> 绘制序列化属性字段，支持在字段左侧显示自定义标签，
        /// 并在右侧显示状态图标（如验证状态指示器）。
        /// <para>
        /// 标签在窗口宽度大于 <paramref name="limite_width"/> 时显示，否则隐藏，适用于响应式布局场景。
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// SerializedProperty myProperty = serializedObject.FindProperty("myField");
        /// Rect fieldRect = new Rect(10, 10, 300, 20);
        /// XGUI.gui_property_field(
        ///     rect: fieldRect,
        ///     title: new GUIContent("我的字段"),
        ///     title_width: 80,
        ///     status_icon: "icon_check",
        ///     status_icon_color: Color.green,
        ///     prop: myProperty
        /// );
        /// </code>
        /// </example>
        public static void gui_property_field(Rect rect, GUIContent title = null, XGUIFontSize title_size = XGUIFontSize.M, TextAnchor title_anchor = TextAnchor.MiddleLeft, Color title_color = default, float title_width = 0, string status_icon = null, Color status_icon_color = default, SerializedProperty prop = null, float limite_width = 230, Color title_hover_color = default, RectOffset prop_margin = default, RectOffset prop_padding = default)
        {
            if (title_color == Color.clear)
                title_color = Color.white * 0.9f;

            if (title_hover_color == Color.clear)
                title_hover_color = Color.white;

            if (prop_margin == null)
                prop_margin = new RectOffset(0, 0, 0, 0);

            if (prop_padding == null)
                prop_padding = new RectOffset(0, 0, 0, 0);

            bool exp = false;
            if (XGUI.GetCurrentWindowWidth() > limite_width)
                exp = true;
            else
                exp = false;

            #region 标题
            //XGUI.gui_box(rect, Color.red * 0.5f);
            if (title != null && exp)
            {
                XGUI.gui_label(
                    rect: new Rect(rect.x, rect.y, exp ? title_width : title_width * 0.1f, XGUI.GetSingleLineHeight()),
                    text: title,
                    text_color: title_color,
                    size: title_size,
                    clipping: TextClipping.Clip,
                    anchor: title_anchor,
                    offset: new Vector2(0, 0),
                    font_style: FontStyle.Normal);
            }
            #endregion

            float field_x = exp ? (title != null ? rect.x + title_width + 10 : rect.x) : rect.x;
            float field_y = rect.y;
            float field_w = exp ? (title != null ? rect.width - (title_width + 10 + (status_icon != null ? 10 : 0)) : rect.width - (status_icon != null ? 10 : 0)) : rect.width - (status_icon != null ? 10 : 0);
            float field_h = XGUI.GetSingleLineHeight();
            Rect rect_field = new Rect(field_x, field_y, field_w, field_h);

            //XGUI.gui_box(rect_field, Color.green * 0.8f);

            EditorGUI.PropertyField(rect_field, prop, GUIContent.none, true);

            if (status_icon != null)
            {
                float x = rect.x + rect.width;
                float y = rect.y + 6;
                float w = 5;
                float h = 5;

                Rect rect_icon = new Rect(x, y, w, h);

                // 状态图标
                XGUI.gui_icon(
                    rect: rect_icon,
                    icon: XGUI.GetBasedIcon(status_icon),
                    color: status_icon_color);
            }

            prop.serializedObject.ApplyModifiedProperties();
        }
    }
}