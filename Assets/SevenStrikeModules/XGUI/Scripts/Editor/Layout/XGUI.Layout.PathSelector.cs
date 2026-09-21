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
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// 为 Editor 界面提供样式控件
    /// </summary>
    public static partial class XGUI
    {
        /// <summary>
        /// 使用自动布局绘制一个路径选择器控件，包含文本输入框和浏览按钮，支持文件夹或文件路径选择。
        /// </summary>
        /// <param name="prop">当前路径值（引用传递，直接修改）。</param>
        /// <param name="path_type">路径类型（文件夹、打开文件、保存文件），决定浏览对话框的行为和按钮文本。</param>
        /// <param name="title">控件左侧显示的标签文本。为 <c>null</c> 或空时不显示标签。</param>
        /// <param name="title_size">标签文本的字体大小。</param>
        /// <param name="title_anchor">标签文本的对齐方式。</param>
        /// <param name="title_color">标签文本的颜色。传入 <see cref="Color.clear"/> 时使用默认颜色。</param>
        /// <param name="title_width">标签的固定宽度。为 0 时自动计算宽度。</param>
        /// <param name="text_wrap">是否启用文本换行。</param>
        /// <param name="field_fontsize">输入字段的字体大小。</param>
        /// <param name="field_text_offset">输入字段文本的偏移量。</param>
        /// <param name="field_height">输入字段的高度。为 0 时使用默认行高。</param>
        /// <param name="field_padding">输入字段的内边距，覆盖样式默认值。</param>
        /// <param name="field_margin">输入字段的外边距，覆盖样式默认值。</param>
        /// <param name="field_text_color">输入字段文本的颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="btn_fontsize">浏览按钮的字体大小。</param>
        /// <param name="btn_color">浏览按钮的背景颜色。</param>
        /// <param name="field_text_font">输入字段的字体。为 <c>null</c> 时使用样式默认字体。</param>
        /// <param name="field_text_style">输入字段的字体样式。</param>
        /// <param name="field_text_anchor">输入字段文本的对齐方式。</param>
        /// <param name="status_icon">状态图标的名称。为 <c>null</c> 时不显示图标。</param>
        /// <param name="filter">文件选择对话框的过滤器（例如 "png文件,*.png;*.jpg"）。仅对 <see cref="XGUIPathType.打开文件"/> 和 <see cref="XGUIPathType.保存文件"/> 有效。</param>
        /// <param name="filter_title">文件选择对话框的过滤器标题。</param>
        /// <param name="default_name">保存文件时的默认文件名。</param>
        /// <param name="status_icon_color">状态图标的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="on_path_changed">路径发生改变时的回调委托，参数为新的路径字符串。</param>
        /// <remarks>
        /// 该方法与 <see cref="gui_path_selector"/> 功能相同，但使用 <see cref="EditorGUILayout"/> 自动布局系统。
        /// <para>
        /// 布局结构（水平分组容器）：
        /// <list type="bullet">
        /// <item><description>标签（左对齐，可选）</description></item>
        /// <item><description>路径输入框（中间，自动填充剩余空间）</description></item>
        /// <item><description>浏览按钮（右侧）</description></item>
        /// <item><description>状态图标（最右侧，可选）</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// 按钮文本根据 <paramref name="path_type"/> 自动调整：
        /// <list type="bullet">
        /// <item><description><see cref="XGUIPathType.文件夹"/>：显示"浏览"</description></item>
        /// <item><description><see cref="XGUIPathType.打开文件"/>：显示"浏览"</description></item>
        /// <item><description><see cref="XGUIPathType.保存文件"/>：显示"保存"</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// string projectPath = "";
        /// XGUI.layout_path_selector(
        ///     prop: ref projectPath,
        ///     path_type: XGUIPathType.文件夹,
        ///     title: "项目路径",
        ///     title_width: 80,
        ///     filter: "Unity场景,*.unity",
        ///     on_path_changed: (path) => Debug.Log($"路径已更改: {path}")
        /// );
        /// </code>
        /// </example>
        public static void layout_path_selector(ref string prop, XGUIPathType path_type = XGUIPathType.文件夹, string title = null, XGUIFontSize title_size = XGUIFontSize.M, TextAnchor title_anchor = TextAnchor.MiddleLeft, Color title_color = default, float title_width = 0, bool text_wrap = true, XGUIFontSize field_fontsize = XGUIFontSize.M, Vector2 field_text_offset = default, float field_height = 20, RectOffset field_padding = default, RectOffset field_margin = default, Color field_text_color = default, XGUIFontSize btn_fontsize = XGUIFontSize.M, Color btn_color = default, Font field_text_font = null, FontStyle field_text_style = FontStyle.Normal, TextAnchor field_text_anchor = TextAnchor.UpperLeft, string status_icon = null, string filter = null, string filter_title = null, string default_name = null, Color status_icon_color = default, Action<string> on_path_changed = null)
        {
            if (title_color == Color.clear)
                title_color = Color.white * 0.9f;

            if (field_padding == null)
                field_padding = new RectOffset(0, 0, 0, 0);

            if (field_margin == null)
                field_margin = new RectOffset(0, 0, 0, 0);

            if (field_text_color == Color.clear)
                field_text_color = Color.white;

            GUIStyle style = new GUIStyle(style_xg_input);
            style.fontSize = XGUI.GetFontSize(field_fontsize);
            style.wordWrap = text_wrap;

            style.normal.textColor = field_text_color;
            style.focused.textColor = field_text_color;
            style.active.textColor = field_text_color;
            style.fontStyle = field_text_style;
            style.contentOffset = field_text_offset;
            style.alignment = field_text_anchor;

            if (field_text_font != null)
                style.font = field_text_font;

            XGUI.layout_group_start(
                type: XGUIContainerType.Horizontal,
                absolute_margin: true,
                margin: field_margin,
                padding: field_padding);

            if (!string.IsNullOrEmpty(title))
            {
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
                    anchor: title_anchor);
                #endregion
            }

            prop = EditorGUILayout.TextField(prop, style, GUILayout.Height(field_height), GUILayout.MinWidth(0));

            if (XGUI.layout_button(
                text: path_type == XGUIPathType.保存文件 ? "保存" : "浏览",
                tooltip: "",
                bg_fill: XGUIFilled.实体,
                bg_color: XGUIColor.亮白,
                bg_color_gui: btn_color,
                button_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(Color.white) ? Color.black : Color.white,
                press_fill: XGUIFilled.实体,
                press_color: XGUIColor.深空灰,
                press_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(Color.white) ? Color.white : Color.black,
                font_size: btn_fontsize,
                anchor: TextAnchor.MiddleCenter,
                margin: new RectOffset(10, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                layout_width: 100,
                layout_min_width: 10,
                //height: ButtonHeight,
                button_text_font: XGUI.GetFont("xg-medium")))
            {
                string selectedPath = XGUI.BrowsePath(prop, path_type, filter, filter_title, default_name);
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    if (on_path_changed != null)
                        on_path_changed(selectedPath);

                    GUIUtility.ExitGUI();
                }
            }

            if (status_icon != null)
            {
                XGUI.layout_icon(
                    icon: XGUI.GetBasedIcon(status_icon),
                    width: 5,
                    height: 5,
                    icon_offset: new Vector2(0, 7),
                    icon_color: status_icon_color,
                    icon_margin: new RectOffset(10, 0, 0, 0),
                    icon_alignment: XGUIIconAlignment.默认);
            }


            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
        }
    }
}