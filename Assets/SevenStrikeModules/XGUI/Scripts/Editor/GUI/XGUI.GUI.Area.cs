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
        /// 开始一个自定义样式的区域布局。
        /// </summary>
        /// <param name="rect_area">定义区域的位置和尺寸的矩形。</param>
        /// <param name="refer">输出参数，返回实际用于布局的矩形区域（在 Repaint 事件中更新）。</param>
        /// <param name="bg_fill">区域的背景填充样式类型。传入 <see cref="XGUIFilled.无"/> 时不绘制背景。</param>
        /// <param name="bg_color">区域的背景颜色主题，与 <paramref name="bg_fill"/> 配合使用。</param>
        /// <param name="bg_color_gui">区域背景的叠加颜色。传入非默认值时可实现颜色混合效果。</param>
        /// <remarks>
        /// 该方法结合 <see cref="gui_area_end"/> 成对使用，用于创建一个具有自定义背景样式的 GUI 区域。
        /// 内部使用 <see cref="GUILayout.BeginArea(Rect, GUIStyle)"/> 实现区域布局，并自动应用 XGUI 的区域样式。
        /// <para>
        /// 使用流程：
        /// <list type="number">
        /// <item><description>调用 <see cref="gui_area_start"/> 开始区域。</description></item>
        /// <item><description>在区域内绘制任意 GUI 控件。</description></item>
        /// <item><description>调用 <see cref="gui_area_end"/> 结束区域。</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// Rect areaRect = new Rect(10, 10, 300, 200);
        /// Rect usedRect = Rect.zero;
        /// XGUI.gui_area_start(areaRect, ref usedRect, XGUIFilled.实体, XGUIColor.工业蓝);
        /// {
        ///     GUILayout.Label("这是区域内的内容");
        ///     if (GUILayout.Button("点击"))
        ///     {
        ///         Debug.Log("按钮被点击");
        ///     }
        /// }
        /// XGUI.gui_area_end();
        /// </code>
        /// </example>
        public static void gui_area_start(Rect rect_area, ref Rect refer, XGUIFilled bg_fill = XGUIFilled.无, XGUIColor bg_color = XGUIColor.亮白, Color bg_color_gui = default)
        {
            if (bg_color_gui == default)
                bg_color_gui = Color.white;

            // 样式 - 背景
            GUIStyle style_area = new GUIStyle(style_xg_area);
            if (bg_fill != XGUIFilled.无)
                style_area.normal.background = GetFillTexture(bg_fill, bg_color);
            else
                style_area.normal.background = null;

            if (Event.current.type == EventType.Repaint)
            {
                refer = rect_area;
                //Debug.Log($"刷新位置: {refer}");
            }


            GUI.backgroundColor = bg_color_gui;
            GUILayout.BeginArea(refer, style_area);
            GUI.backgroundColor = Color.white;
        }
        /// <summary>
        /// 结束由 <see cref="gui_area_start"/> 开始的区域布局。
        /// </summary>
        /// <remarks>
        /// 该方法执行 <see cref="GUILayout.EndArea"/> 并重置 GUI 背景颜色。
        /// 必须与 <see cref="gui_area_start"/> 成对使用，否则会导致布局错乱。
        /// </remarks>
        public static void gui_area_end()
        {
            GUI.backgroundColor = Color.white;
            GUILayout.EndArea();
        }
    }
}