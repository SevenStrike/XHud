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
        /// 使用自动布局绘制一个带有标签和状态图标的序列化属性字段。
        /// </summary>
        /// <param name="title">属性标签的文本。为 <c>null</c> 或空时不显示标签。</param>
        /// <param name="title_size">标签文本的字体大小。</param>
        /// <param name="title_anchor">标签文本的对齐方式。</param>
        /// <param name="title_color">标签文本的颜色。传入 <see cref="Color.clear"/> 时使用半透明白色。</param>
        /// <param name="title_width">标签的固定宽度。为 0 时自动计算宽度。</param>
        /// <param name="status_icon">状态图标的名称（通过 <see cref="GetBasedIcon"/> 加载）。为 <c>null</c> 时不显示图标。</param>
        /// <param name="status_icon_color">状态图标的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="prop">要绘制的序列化属性。</param>
        /// <param name="title_hover_color">预留参数，暂未使用。</param>
        /// <param name="prop_margin">属性字段的外边距，覆盖样式默认值。</param>
        /// <param name="prop_padding">属性字段的内边距，覆盖样式默认值。</param>
        /// <remarks>
        /// 该方法与 <see cref="gui_property_field"/> 功能相同，但使用 <see cref="EditorGUILayout"/> 自动布局系统。
        /// 内部使用水平分组容器将标签和属性字段并排显示，并支持在字段右侧显示状态图标。
        /// <para>
        /// 标签字体固定使用 "xg-medium"，以保持界面一致性。
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// SerializedProperty myProperty = serializedObject.FindProperty("myField");
        /// XGUI.layout_property_field(
        ///     title: "我的字段",
        ///     title_width: 80,
        ///     status_icon: "icon_check",
        ///     status_icon_color: Color.green,
        ///     prop: myProperty
        /// );
        /// </code>
        /// </example>
        public static void layout_property_field(string title = null, XGUIFontSize title_size = XGUIFontSize.M, TextAnchor title_anchor = TextAnchor.MiddleLeft, Color title_color = default, float title_width = 0, string status_icon = null, Color status_icon_color = default, SerializedProperty prop = null, Color title_hover_color = default, RectOffset prop_margin = default, RectOffset prop_padding = default, float field_width = 0)
        {
            if (title_color == Color.clear)
                title_color = Color.white * 0.9f;

            if (title_hover_color == Color.clear)
                title_hover_color = Color.white;

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

            if (field_width == 0)
                EditorGUILayout.PropertyField(prop, GUIContent.none, true, GUILayout.MinWidth(10));
            else
                EditorGUILayout.PropertyField(prop, GUIContent.none, true, GUILayout.MinWidth(10), GUILayout.Width(field_width));


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

            prop.serializedObject.ApplyModifiedProperties();
        }
    }
}