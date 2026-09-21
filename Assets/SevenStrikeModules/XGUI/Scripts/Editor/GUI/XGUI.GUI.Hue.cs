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
        public static Color gui_hue_gradient(Rect rect, SerializedProperty prop, Texture2D mark = null, float mark_height_offset = 0, Color mark_color = default, Vector2 mark_size = default)
        {
            // 绘制颜色渐变背景
            Rect gradientRect = new Rect(rect.x, rect.y, rect.width, rect.height);

            // 绘制完整的颜色渐变背景
            for (int i = 0; i < gradientRect.width; i++)
            {
                float ratio = (float)i / gradientRect.width;
                Color color = Color.HSVToRGB(ratio, 1f, 1f); // 生成HSV颜色
                Rect lineRect = new Rect(gradientRect.x + i, gradientRect.y + 10, 1, gradientRect.height);
                EditorGUI.DrawRect(lineRect, color);
            }

            if (mark != null)
            {
                // 绘制滑块位置的标记
                Rect handleRect = new Rect(gradientRect.x + gradientRect.width * prop.vector3Value.x - 2, gradientRect.y + 2 + mark_height_offset, mark_size.x, mark_size.y);
                XGUI.gui_icon(
                    rect: handleRect,
                    icon: mark,
                    padding: new RectOffset(0, 0, 0, 0),
                    color: mark_color);
            }
            return Color.HSVToRGB(prop.vector3Value.x, prop.vector3Value.y, prop.vector3Value.z);
        }
    }
}