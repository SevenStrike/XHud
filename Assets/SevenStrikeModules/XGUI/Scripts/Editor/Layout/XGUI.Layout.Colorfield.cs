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
        /// 使用自动布局绘制一个颜色选择器字段，包含颜色拾取器、当前颜色值显示和 Hex 复制功能（基于 SerializedProperty）。
        /// </summary>
        /// <param name="prop">包含当前颜色值的序列化属性。</param>
        /// <param name="title">颜色选择器的标题文本。为 <c>null</c> 或空时使用默认标题。</param>
        /// <param name="title_width">标题的固定宽度。</param>
        /// <param name="title_color">标题的颜色。传入 <see cref="Color.clear"/> 时使用半透明白色。</param>
        /// <param name="icon">状态图标的名称。</param>
        /// <param name="icon_color">状态图标的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="state_title">状态显示器的标题文本。</param>
        /// <param name="state_value">状态显示器的值文本（显示当前颜色值）。</param>
        /// <param name="state_value_color">状态值文本的颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <returns>用户选择或编辑后的颜色值。</returns>
        /// <remarks>
        /// 该控件结合了颜色输入字段和状态展示器，提供完整的颜色选择体验：
        /// <list type="bullet">
        /// <item><description>通过 <see cref="XGUI.layout_inputfield(Color, string, XGUIFontSize, TextAnchor, Color, float, float, RectOffset, RectOffset, string, Color)"/> 绘制颜色拾取器</description></item>
        /// <item><description>通过 <see cref="layout_state_displayer_text"/> 显示当前颜色值</description></item>
        /// <item><description>按住 Alt 键点击状态值区域可将 Hex 颜色值复制到系统剪贴板</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        public static Color layout_colorfield(SerializedProperty prop = null, string title = null, float title_width = 40, Color title_color = default, string icon = null, Color icon_color = default, string state_title = "当前", string state_value = "-", Color state_value_color = default, Color state_dialog_theme_color = default, float field_width = 0)
        {
            if (icon_color == Color.clear)
                icon_color = Color.white;

            if (state_dialog_theme_color == Color.clear)
                state_dialog_theme_color = Color.white;

            if (state_value_color == Color.clear)
                state_value_color = Color.white;

            if (title_color == Color.clear)
                title_color = Color.white * 0.9f;

            Color cc = XGUI.layout_inputfield(
                title: title,
                prop: prop.colorValue,
                title_size: XGUIFontSize.M,
                title_anchor: TextAnchor.MiddleLeft,
                title_color: title_color,
                title_width: title_width,
                field_padding: new RectOffset(0, 0, 0, 0),
                field_margin: new RectOffset(0, 0, 5, 8),
                field_width: field_width,
                status_icon: icon,
                status_icon_color: icon_color);

            if (!string.IsNullOrEmpty(state_title))
            {
                XGUI.layout_state_displayer_text(
                title: state_title,
                title_size: XGUIFontSize.M,
                subtitle: state_value,
                subtitle_size: XGUIFontSize.M,
                subtitle_color: state_value_color,
                padding: new RectOffset(0, 0, 0, 5),
                subtitle_tooltip: "按住 Alt 键点击可复制 Hex 颜色值");
            }

            Rect rect_state = XGUI.GetLastRect();

            // 测试区域
            //XGUI.gui_box(rect_state, Color.red);

            Event e = Event.current;
            if (e.type == EventType.MouseDown && (e.modifiers & EventModifiers.Alt) != 0 && rect_state.Contains(e.mousePosition))
            {
                string hex = XGUI_Utilitys.Color_To_HexString(cc);
                EditorGUIUtility.systemCopyBuffer = hex;
                EditorApplication.delayCall += () =>
                {
                    XGUI.dialog(
                        type: XGUIDialogType.确认,
                        windowtitle: "XGUI 控件消息",
                        title: "已复制 Hex格式 颜色值",
                        msg: $"已将\"<color=#{hex}> {hex} </color>\"颜色值复制到系统剪贴板！",
                        ok: "明白",
                        PrimaryIndex: 0,
                        themecolor: state_dialog_theme_color);
                };
                e.Use();
            }
            prop.colorValue = cc;
            prop.serializedObject.ApplyModifiedProperties();
            return cc;
        }

        /// <summary>
        /// 使用自动布局绘制一个颜色选择器字段，包含颜色拾取器、当前颜色值显示和 Hex 复制功能（基于值类型）。
        /// </summary>
        /// <param name="prop">当前颜色值。</param>
        /// <param name="title">颜色选择器的标题文本。为 <c>null</c> 或空时使用默认标题。</param>
        /// <param name="title_width">标题的固定宽度。</param>
        /// <param name="title_color">标题的颜色。传入 <see cref="Color.clear"/> 时使用半透明白色。</param>
        /// <param name="icon">状态图标的名称。</param>
        /// <param name="icon_color">状态图标的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="state_title">状态显示器的标题文本。</param>
        /// <param name="state_value">状态显示器的值文本（显示当前颜色值）。</param>
        /// <param name="state_value_color">状态值文本的颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <returns>用户选择或编辑后的颜色值。</returns>
        /// <remarks>
        /// 该版本基于值类型（Color），适用于非序列化属性的场景。
        /// 功能与 <see cref="layout_colorfield(SerializedProperty, string, float, Color, string, Color, string, string, Color)"/> 相同。
        /// <para>
        /// 交互特性：
        /// <list type="bullet">
        /// <item><description>点击颜色拾取器打开系统颜色选择对话框</description></item>
        /// <item><description>状态区域显示当前颜色的 Hex 格式值</description></item>
        /// <item><description>按住 Alt 键点击状态区域复制 Hex 值到剪贴板</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        public static Color layout_colorfield(Color prop = default, string title = null, float title_width = 40, Color title_color = default, string icon = null, Color icon_color = default, string state_title = "当前", string state_value = "-", Color state_value_color = default, Color state_dialog_theme_color = default, float field_width = 0)
        {
            if (icon_color == Color.clear)
                icon_color = Color.white;

            if (state_dialog_theme_color == Color.clear)
                state_dialog_theme_color = Color.white;

            if (state_value_color == Color.clear)
                state_value_color = Color.white;

            if (title_color == Color.clear)
                title_color = Color.white * 0.9f;

            Color cc = XGUI.layout_inputfield(
                title: title,
                prop: prop,
                title_size: XGUIFontSize.M,
                title_anchor: TextAnchor.MiddleLeft,
                title_color: title_color,
                title_width: title_width,
                field_padding: new RectOffset(0, 0, 0, 0),
                field_margin: new RectOffset(0, 0, 5, 8),
                field_width: field_width,
                status_icon: icon,
                status_icon_color: icon_color);

            if (!string.IsNullOrEmpty(state_title))
            {
                XGUI.layout_state_displayer_text(
                    title: state_title,
                    title_size: XGUIFontSize.M,
                    subtitle: state_value,
                    subtitle_size: XGUIFontSize.M,
                    subtitle_color: state_value_color,
                    padding: new RectOffset(0, 0, 0, 5),
                    subtitle_tooltip: "按住 Alt 键点击可复制 Hex 颜色值");
            }

            Rect rect_state = XGUI.GetLastRect();

            // 测试区域
            //XGUI.gui_box(rect_state, Color.red);

            Event e = Event.current;
            if (e.type == EventType.MouseDown && (e.modifiers & EventModifiers.Alt) != 0 && rect_state.Contains(e.mousePosition))
            {
                string hex = XGUI_Utilitys.Color_To_HexString(cc);
                EditorGUIUtility.systemCopyBuffer = hex;
                EditorApplication.delayCall += () =>
                {
                    XGUI.dialog(
                        type: XGUIDialogType.确认,
                        windowtitle: "XGUI 控件消息",
                        title: "已复制 Hex格式 颜色值",
                        msg: $"已将\"<color=#{hex}> {hex} </color>\"颜色值复制到系统剪贴板！",
                        ok: "明白",
                        PrimaryIndex: 0,
                        themecolor: state_dialog_theme_color);
                };
                e.Use();
            }
            prop = cc;
            return cc;
        }
    }
}