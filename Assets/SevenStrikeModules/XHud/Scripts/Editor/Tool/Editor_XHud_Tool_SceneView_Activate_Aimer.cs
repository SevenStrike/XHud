/*
 * ============================================================================
 * ⚠ 版权声明（禁止删除、禁止修改、衍生作品必须保留此注释）⚠
 * ============================================================================
 * 版权声明 Copyright (C) 2025-Present Nanjing SevenStrike Media Co., Ltd.
 * 中文名称：南京塞维斯传媒有限公司
 * 英文名称：SevenStrikeMedia
 * 项目作者：徐寅智
 * 项目名称：XHud - Unity UGUI 高级管理架构插件
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
namespace SevenStrikeModules.XHud.Editor
{
    using SevenStrikeModules.XHud.Enums;
    using UnityEditor;
    using UnityEditor.Callbacks;
    using UnityEngine;

    [InitializeOnLoad]
    public static class Editor_XHud_Tool_SceneView_Activate_Aimer
    {
        #region 字段 - 标识与尺寸
        /// <summary>
        /// 边框颜色。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>由 <c>SetEnabled</c> 重载设置。</description></item>
        /// </list>
        /// </summary>
        public static Color themecolor;
        /// <summary>
        /// 准星整体缩放系数。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>1 为默认大小，大于 1 放大，小于 1 缩小。</description></item>
        /// <item><description>仅缩放斜线位置与长度，线条粗细、圆点尺寸、两组斜线间距不缩放。</description></item>
        /// </list>
        /// </summary>
        private const float aimer_size = 2f;
        #endregion

        #region 字段 - 状态
        /// <summary>
        /// 是否启用标识显示。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>默认禁用。</description></item>
        /// </list>
        /// </summary>
        private static bool is_enabled = false;
        #endregion

        #region 生命周期
        /// <summary>
        /// 静态构造函数。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>由 <c>[InitializeOnLoad]</c> 触发。</description></item>
        /// <item><description>订阅 <c>SceneView.duringSceneGui</c>。</description></item>
        /// </list>
        /// </summary>
        static Editor_XHud_Tool_SceneView_Activate_Aimer()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }
        #endregion

        #region 绘制 - 边框与标识
        /// <summary>
        /// SceneView 绘制事件回调。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>功能禁用时直接返回。</description></item>
        /// <item><description>调用 <c>DrawRect</c> 绘制并触发 SceneView 重绘。</description></item>
        /// </list>
        /// </summary>
        /// <param name="sceneView">当前的 SceneView 实例。</param>
        private static void OnSceneGUI(SceneView sceneView)
        {
            // 如果功能被禁用，不处理任何事件
            if (!is_enabled) return;

            DrawRect(sceneView);
            sceneView.Repaint();
        }
        /// <summary>
        /// 绘制准星标识（内组四段斜线 + 外组四段斜线 + 中心圆点）。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>使用 <c>Handles.BeginGUI</c> / <c>EndGUI</c> 包裹绘制。</description></item>
        /// <item><description>内组斜线较短，外组斜线更长并向外延伸，两组之间留有间隙。</description></item>
        /// <item><description>斜线位置与长度由 <c>AIMER_SIZE</c> 统一缩放，间距与粗细保持固定。</description></item>
        /// <item><description>中心为实心圆点，整体可随 <c>color</c> 着色。</description></item>
        /// </list>
        /// </summary>
        /// <param name="sv">当前的 SceneView 实例。</param>
        private static void DrawRect(SceneView sv)
        {
            Rect rect = sv.position;

            // 准星中心（视口中心，向上偏移 14 像素）
            Vector2 center = new Vector2(rect.width / 2f, rect.height / 2f - 14f);

            // 基准尺寸（AIMER_SIZE = 1 时的像素值）
            const float baseInnerEnd = 14f;       // 内组斜线外端距中心距离
            const float baseOuterEnd = 30f;       // 外组斜线外端距中心距离

            // 固定尺寸（不随 AIMER_SIZE 缩放）
            const float lineGap = 6f;             // 两组斜线之间的固定间隙
            const float innerThickness = 3f;      // 内组斜线粗细
            const float outerThickness = 1f;    // 外组斜线粗细
            const float dotRadius = 2.5f;         // 中心圆点半径

            // 按整体缩放系数换算斜线位置与长度
            float s = aimer_size;
            float innerEnd = baseInnerEnd * s;
            float outerEnd = baseOuterEnd * s;

            // 内组斜线内端：贴住中心圆点外侧（固定间距）
            float innerGap = dotRadius + lineGap;
            // 外组斜线内端：内组外端 + 固定间隙
            float outerGap = innerEnd + lineGap;

            // 四个 45° 方向（右上、右下、左下、左上）
            Vector2[] dirs =
            {
                new Vector2( 1f,  1f).normalized,
                new Vector2( 1f, -1f).normalized,
                new Vector2(-1f, -1f).normalized,
                new Vector2(-1f,  1f).normalized
            };

            Handles.BeginGUI();

            Color lineColor = themecolor == Color.clear ? Color.white : themecolor;

            // 绘制内组四段斜线
            Handles.color = lineColor;
            foreach (Vector2 dir in dirs)
            {
                Vector2 from = center + dir * innerGap;
                Vector2 to = center + dir * innerEnd;
                Handles.DrawAAPolyLine(innerThickness, from, to);
            }

            // 绘制外组四段斜线（向外延伸）
            Handles.color = lineColor;
            foreach (Vector2 dir in dirs)
            {
                Vector2 from = center + dir * outerGap;
                Vector2 to = center + dir * outerEnd;
                Handles.DrawAAPolyLine(outerThickness, from, to);
            }

            // 绘制中心实心圆点
            Handles.color = Color.white;
            Handles.DrawSolidDisc(center, Vector3.forward, dotRadius);

            Handles.EndGUI();
        }
        #endregion

        #region 外部控制开关
        /// <summary>
        /// 设置启用状态。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>禁用时将颜色重置为透明。</description></item>
        /// </list>
        /// </summary>
        /// <param name="enabled">是否启用标识显示。</param>
        private static void SetEnabled(bool enabled)
        {
            is_enabled = enabled;

            if (!is_enabled)
            {
                themecolor = Color.clear;
            }
        }
        /// <summary>
        /// 获取当前启用状态。
        /// </summary>
        /// <returns>当前是否启用标识显示。</returns>
        public static bool GetEnabled()
        {
            return is_enabled;
        }
        /// <summary>
        /// 设置启用状态及标识外观参数。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>颜色为透明时使用主题主色 <c>XHud_Dashboard.themes_get_color(ThemeColorType.primary)</c>。</description></item>
        /// <item><description>调用 <c>SetEnabled</c> 重载应用状态。</description></item>
        /// </list>
        /// </summary>
        /// <param name="state">是否启用标识显示。</param>
        /// <param name="x_color">边框颜色，默认透明时使用主题主色。</param>
        /// <param name="x_title">标题文本。</param>
        /// <param name="x_msg">消息文本。</param>
        /// <param name="x_logo">Logo 纹理。</param>
        /// <param name="x_size">尺寸。</param>
        /// <param name="x_anchor">锚点位置。</param>
        public static void SetEnabled(bool state, Color x_color = default, string x_title = "", string x_msg = "", Texture2D x_logo = null, Vector2 x_size = default, XHudSceneActivateMarkAnchor x_anchor = XHudSceneActivateMarkAnchor.左下)
        {
            if (x_color == Color.clear)
                themecolor = XHud_Dashboard.themes_get_color(ThemeColorType.primary);
            else
                themecolor = x_color;

            SetEnabled(state);
        }
        #endregion

        [DidReloadScripts]
        public static void reload()
        {
            themecolor = XHud_Dashboard.themes_get_color(ThemeColorType.primary);
            is_enabled = XHud_Dashboard.utilitys_get_util_screeen_aimer();
        }
    }
}