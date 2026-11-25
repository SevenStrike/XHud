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
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// 为 Editor 界面提供样式控件
    /// </summary>
    public static partial class XGUI
    {
        /// <summary>
        /// 在指定矩形位置绘制一个自定义样式的盒子。
        /// </summary>
        /// <param name="rect">盒子的位置和尺寸。</param>
        /// <param name="bg">盒子的背景纹理。为 <c>null</c> 时使用默认背景。</param>
        /// <param name="bg_color_gui">背景颜色的叠加色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="offset">内容偏移量，影响盒子内部内容的显示位置。</param>
        /// <param name="border">盒子的边框大小。为 <c>null</c> 时使用默认边框（0,0,0,0）。</param>
        /// <param name="margin">盒子的外边距，会叠加到样式的默认边距上。为 <c>null</c> 时不额外增加边距。</param>
        /// <remarks>
        /// 该方法基于 <see cref="style_xg_box"/> 样式绘制盒子，支持自定义背景纹理、边框、边距和偏移量。
        /// 绘制完成后会自动恢复 <see cref="GUI.backgroundColor"/> 到调用前的状态。
        /// </remarks>
        public static void gui_box(Rect rect, Texture2D bg = null, Color bg_color_gui = default, Vector2 offset = default, RectOffset border = default, RectOffset margin = default)
        {
            if (bg_color_gui == Color.clear)
                bg_color_gui = Color.white;

            if (border == null)
                border = new RectOffset(0, 0, 0, 0);

            if (margin == null)
                margin = new RectOffset(0, 0, 0, 0);

            GUIStyle style = new GUIStyle(style_xg_box);
            style.border = border;
            style.normal.background = bg;

            RectOffset _m = style.margin;

            style.margin = new RectOffset(_m.left + margin.left, _m.right + margin.right, _m.top + margin.top, _m.bottom + margin.bottom);

            style.contentOffset = new Vector2(style.contentOffset.x + offset.x, style.contentOffset.y + offset.y);

            Color col = GUI.backgroundColor;
            GUI.backgroundColor = bg_color_gui;
            GUI.Box(rect, GUIContent.none, style);
            GUI.backgroundColor = col;
        }
        /// <summary>
        /// 在指定矩形位置绘制一个纯色盒子。
        /// </summary>
        /// <param name="rect">盒子的位置和尺寸。</param>
        /// <param name="bg_color">盒子的背景颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <remarks>
        /// 该方法使用 <see cref="EditorGUI.DrawRect"/> 直接绘制纯色矩形，适用于需要快速绘制背景块的场景。
        /// </remarks>
        public static void gui_box(Rect rect, Color bg_color = default)
        {
            if (bg_color == Color.clear)
                bg_color = Color.white;

            EditorGUI.DrawRect(rect, bg_color);
        }
    }
}