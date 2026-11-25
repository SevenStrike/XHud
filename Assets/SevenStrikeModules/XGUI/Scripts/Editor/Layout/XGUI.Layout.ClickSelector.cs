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

    public class clickselector_args
    {
        public Texture2D btn_icon_r;
        public Texture2D btn_icon_p;
        public string btn_tooltip;
    }

    /// <summary>
    /// 为 Editor 界面提供样式控件
    /// </summary>
    public static partial class XGUI
    {
        /// <summary>
        /// 使用自动布局绘制一个自定义样式的工具栏（Toolbar），用于在多个选项间切换选择。
        /// </summary>
        /// <param name="index">当前选中项的索引（引用传递，会自动更新）。</param>
        /// <param name="names">工具栏按钮的显示名称数组。</param>
        /// <param name="bg_normal">未选中按钮的背景填充样式。</param>
        /// <param name="bg_selected">选中按钮的背景填充样式。</param>
        /// <param name="bg_color">按钮背景的颜色主题。</param>
        /// <param name="bg_gui_color">按钮背景的叠加颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="text_color_normal">未选中按钮的文本颜色。传入 <see cref="Color.clear"/> 时使用黑色。</param>
        /// <param name="text_color_selected">选中按钮的文本颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="bar_height">工具栏的高度。</param>
        /// <param name="text_anchor">按钮文本的对齐方式。</param>
        /// <param name="text_padding">按钮文本的内边距。</param>
        /// <param name="bar_margin">工具栏的外边距。</param>
        /// <param name="text_offset">按钮文本的偏移量。</param>
        /// <param name="text_font">按钮文本的字体。为 <c>null</c> 时使用样式默认字体。</param>
        /// <param name="text_fontstyle">按钮文本的字体样式。</param>
        /// <param name="navigate_style">是否使用导航样式。为 <c>true</c> 时移除按钮背景，仅显示带边框的容器。</param>
        /// <param name="navigate_style_bg">导航样式下的容器边框样式。</param>
        /// <param name="navigate_style_bg_color">导航样式下容器边框的颜色。传入 <see cref="Color.clear"/> 时使用默认颜色。</param>
        /// <returns>更新后的选中项索引。</returns>
        /// <remarks>
        /// 该方法与 <see cref="gui_toolbar"/> 功能相同，但使用 <see cref="GUILayout"/> 自动布局系统。
        /// 通过 <see cref="GUILayout.Toolbar"/> 绘制工具栏，无需手动指定矩形位置。
        /// <para>
        /// 支持两种样式模式：
        /// <list type="bullet">
        /// <item><description>标准模式：每个按钮有独立的背景填充</description></item>
        /// <item><description>导航模式：整个工具栏显示为带边框的容器，按钮无背景（适合面包屑导航）</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        public static int layout_clickselector(int index, clickselector_args[] icon_btns, Color btn_color_normal = default, Color btn_color_selected = default, float btn_height = 14, RectOffset padding = null, RectOffset margin = null)
        {
            int sel_index = index;

            XGUI.layout_group_start(
               type: XGUIContainerType.Horizontal,
               absolute_margin: true,
               absolute_padding: true,
               margin: margin,
               padding: padding);

            for (int i = 0; i < icon_btns.Length; i++)
            {
                if (XGUI.layout_button(
                    tooltip: icon_btns[i].btn_tooltip,
                    tex_release: icon_btns[i].btn_icon_r,
                    tex_press: icon_btns[i].btn_icon_p,
                    tex_gui_color: index == i ? btn_color_selected : btn_color_normal,
                    border: new RectOffset(0, 0, 0, 0),
                    absolute_margin: true,
                    absolute_padding: true,
                    margin: new RectOffset(0, 0, 0, 0),
                    padding: new RectOffset(0, 0, 0, 0),
                    width: btn_height,
                    height: btn_height))
                {
                    sel_index = i;
                }

                if (i != icon_btns.Length - 1)
                    XGUI.layout_flexspace();
            }

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);

            return sel_index;
        }
    }
}