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
        /// 使用自动布局绘制一条水平分割线。
        /// </summary>
        /// <param name="thickness">分割线的粗细（高度）。</param>
        /// <param name="color">分割线的颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="margin">分割线的外边距，覆盖样式默认值。</param>
        /// <param name="padding">分割线的内边距，覆盖样式默认值。</param>
        /// <remarks>
        /// 该方法与 <see cref="gui_seperator"/> 功能相同，但使用 <see cref="GUILayout"/> 自动布局系统。
        /// 通过 <see cref="GUILayout.Box(string, GUIStyle, GUILayoutOption[])"/> 绘制分割线，无需手动指定矩形位置。
        /// <para>
        /// 绘制完成后会自动恢复 <see cref="GUI.backgroundColor"/> 到白色。
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// XGUI.layout_seperator(thickness: 2, color: new Color(0.5f, 0.5f, 0.5f, 0.5f));
        /// </code>
        /// </example>
        public static void layout_seperator(float thickness = 1, XGUISeplineDir dir = XGUISeplineDir.水平, Color color = default, RectOffset margin = default, RectOffset padding = default)
        {
            if (color == Color.clear)
                color = Color.white;

            if (margin == null)
                margin = new RectOffset(0, 0, 0, 0);

            if (padding == null)
                padding = new RectOffset(0, 0, 0, 0);

            GUIStyle style = new GUIStyle(style_xg_seperate);
            if (dir == XGUISeplineDir.水平)
            {
                style.fixedHeight = thickness;
                style.stretchHeight = false;
                style.stretchWidth = true;
                style.fixedWidth = 0;
            }
            else
            {
                style.fixedHeight = 0;
                style.stretchHeight = true;
                style.stretchWidth = false;
                style.fixedWidth = thickness;
            }
            style.margin = margin;
            style.padding = padding;

            GUI.backgroundColor = color;
            GUILayout.Box("", style);
            GUI.backgroundColor = Color.white;
        }
    }
}