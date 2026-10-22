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
    using SevenStrikeModules.XGUI.Editor;
    using SevenStrikeModules.XGUI.Runtime;
    using SevenStrikeModules.XHud.Enums;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// SceneView 激活状态标识工具。
    /// <para/>
    /// <list type="bullet">
    /// <item><description>在 SceneView 边缘绘制彩色边框与 Logo / 标题 / 说明文本。</description></item>
    /// <item><description>标识可配置四角锚点、颜色、标题、说明、Logo 与 Logo 尺寸。</description></item>
    /// <item><description>通过 <c>[InitializeOnLoad]</c> 自动订阅 <c>SceneView.duringSceneGui</c>。</description></item>
    /// </list>
    /// </summary>
    [InitializeOnLoad]
    public static class Editor_XHud_Tool_SceneView_Activate_Mark
    {
        #region 字段 - 标识内容
        /// <summary>
        /// 边框颜色。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>由 <c>SetEnabled</c> 重载设置。</description></item>
        /// </list>
        /// </summary>
        public static Color color;
        /// <summary>
        /// 标题文本。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>由 <c>SetEnabled</c> 重载设置。</description></item>
        /// </list>
        /// </summary>
        public static string title;
        /// <summary>
        /// 说明文本。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>由 <c>SetEnabled</c> 重载设置。</description></item>
        /// </list>
        /// </summary>
        public static string msg;
        /// <summary>
        /// 标识 Logo。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>由 <c>SetEnabled</c> 重载设置。</description></item>
        /// </list>
        /// </summary>
        public static Texture2D logo;
        /// <summary>
        /// 标识锚点位置。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>决定 Logo / 标题 / 说明在 SceneView 中的显示位置。</description></item>
        /// </list>
        /// </summary>
        public static XHudSceneActivateMarkAnchor anchor;
        /// <summary>
        /// Logo 显示尺寸。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>默认 48 x 48。</description></item>
        /// </list>
        /// </summary>
        public static Vector2 logo_size = new Vector2(48, 48);
        #endregion

        #region 字段 - 布局缓存
        /// <summary>
        /// Logo 的绘制矩形。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>在 <c>DrawRect</c> 中按锚点更新。</description></item>
        /// </list>
        /// </summary>
        static Rect rect_logo;
        /// <summary>
        /// 标题的绘制矩形。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>在 <c>DrawRect</c> 中按锚点更新。</description></item>
        /// </list>
        /// </summary>
        static Rect rect_title;
        /// <summary>
        /// 说明文本的绘制矩形。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>在 <c>DrawRect</c> 中按锚点更新。</description></item>
        /// </list>
        /// </summary>
        static Rect rect_msg;
        #endregion

        #region 字段 - 状态
        /// <summary>
        /// 是否启用标识显示。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>默认禁用。</description></item>
        /// </list>
        /// </summary>
        private static bool isEnabled = false;
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
        static Editor_XHud_Tool_SceneView_Activate_Mark()
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
            if (!isEnabled) return;

            DrawRect(sceneView);
            sceneView.Repaint();
        }
        /// <summary>
        /// 在 SceneView 上绘制边框与标识。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>绘制四条独立边框，适配 SceneView 的工具栏布局。</description></item>
        /// <item><description>上边、左边、右边紧贴视图边缘；下边按最大化状态动态偏移。</description></item>
        /// <item><description>按锚点绘制 Logo、标题与说明文本。</description></item>
        /// </list>
        /// </summary>
        /// <param name="sv">SceneView 实例。</param>
        private static void DrawRect(SceneView sv)
        {
            Rect rect = sv.position;
            Handles.BeginGUI();

            float borderWidth = 2f;
            Color borderColor = color;

            // 最大化时不需要偏移
            float bottomOffset = sv.maximized ? 30f : 26f;

            #region 边框
            EditorGUI.DrawRect(new Rect(0, 0, rect.width, borderWidth), borderColor);
            EditorGUI.DrawRect(new Rect(0, rect.height - borderWidth - bottomOffset, rect.width, borderWidth), borderColor);
            EditorGUI.DrawRect(new Rect(0, 0, borderWidth, rect.height), borderColor);
            EditorGUI.DrawRect(new Rect(rect.width - borderWidth, 0, borderWidth, rect.height), borderColor);
            #endregion

            #region 绝对定位
            Rect rect_ac_l_t = new Rect(0, 0, 100, 100);
            // 区域显示
            //XGUI.gui_box(rect_ac_l_t, Color.black * 0.5f);
            Rect rect_ac_l_b = new Rect(0, rect.height - 100 - bottomOffset, 100, 100);
            // 区域显示
            //XGUI.gui_box(rect_ac_l_b, Color.black * 0.5f);
            Rect rect_ac_r_t = new Rect(rect.width - 100, 0, 100, 100);
            // 区域显示
            //XGUI.gui_box(rect_ac_r_t, Color.black * 0.5f);
            Rect rect_ac_r_b = new Rect(rect.width - 100, rect.height - 100 - bottomOffset, 100, 100);
            // 区域显示
            //XGUI.gui_box(rect_ac_r_b, Color.black * 0.5f);
            #endregion

            #region 元素坐标调教
            switch (anchor)
            {
                case XHudSceneActivateMarkAnchor.左上:
                    rect_logo.Set(rect_ac_l_t.x + 25, rect_ac_l_t.y + 20, logo_size.x, logo_size.y);
                    rect_title.Set(rect_ac_l_t.x + 85, rect_ac_l_t.y + 34, 140, 22);
                    rect_msg.Set(rect_ac_l_t.x + 30, rect_ac_l_t.y + 65, rect.width - 30, XGUI.GetSingleLineHeight());
                    break;
                case XHudSceneActivateMarkAnchor.左下:
                    rect_logo.Set(rect_ac_l_b.x + 25, rect_ac_l_b.y + 32, logo_size.x, logo_size.y);
                    rect_title.Set(rect_ac_l_b.x + 85, rect_ac_l_b.y + 47, 140, 22);
                    rect_msg.Set(rect_ac_l_b.x + 30, rect_ac_l_b.y + 13, rect.width - 30, XGUI.GetSingleLineHeight());
                    break;
                case XHudSceneActivateMarkAnchor.右上:
                    rect_logo.Set(rect_ac_r_t.x + (rect_ac_r_t.width - logo_size.x - 177), rect_ac_r_t.y + 20, logo_size.x, logo_size.y);
                    rect_title.Set(rect_ac_r_t.x + (rect_ac_r_t.width - 140 - 25), rect_ac_r_t.y + 34, 140, 22);
                    rect_msg.Set(rect_ac_r_t.x - (rect.width - 152), rect_ac_r_t.y + 65, rect.width - 80, XGUI.GetSingleLineHeight());
                    break;
                case XHudSceneActivateMarkAnchor.右下:
                    rect_logo.Set(rect_ac_r_b.x + (rect_ac_r_b.width - logo_size.x - 177), rect_ac_r_b.y + 32, logo_size.x, logo_size.y);
                    rect_title.Set(rect_ac_r_b.x + (rect_ac_r_b.width - 140 - 25), rect_ac_r_b.y + 47, 140, 22);
                    rect_msg.Set(rect_ac_r_b.x - (rect.width - 152), rect_ac_r_b.y + 13, rect.width - 80, XGUI.GetSingleLineHeight());
                    break;
            }
            #endregion

            #region Logo
            if (logo != null)
            {
                XGUI.gui_icon(
                    rect: rect_logo,
                    icon: logo,
                    color: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
            }
            #endregion

            #region 标题
            XGUI.gui_label(
                rect: rect_title,
                bg_fill: XGUIGroup.实体,
                bg_color: XGUIColor.亮白,
                bg_color_gui: Color.white * 0.88f,
                text: new GUIContent(title),
                text_color: Color.black,
                size: XGUIFontSize.BX,
                clipping: TextClipping.Overflow,
                anchor: TextAnchor.MiddleCenter,
                offset: new Vector2(0, -2),
                wrap: true,
                font_style: FontStyle.Normal,
                font: XGUI.GetFont("xg-regular"));
            #endregion

            #region 内容
            XGUI.gui_label(
                rect: rect_msg,
                text: new GUIContent(msg),
                text_color: Color.white,
                size: XGUIFontSize.L,
                clipping: TextClipping.Overflow,
                anchor: anchor == XHudSceneActivateMarkAnchor.右上 || anchor == XHudSceneActivateMarkAnchor.右下 ? TextAnchor.UpperRight : TextAnchor.UpperLeft,
                offset: new Vector2(0, 0),
                wrap: true,
                font_style: FontStyle.Bold);
            #endregion


            Handles.EndGUI();
        }
        #endregion

        #region 外部控制开关
        /// <summary>
        /// 设置标识功能的启用状态。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>禁用时清空所有标识内容并重置锚点。</description></item>
        /// <item><description>仅由带参数的重载内部调用。</description></item>
        /// </list>
        /// </summary>
        /// <param name="enabled">true=启用, false=禁用。</param>
        private static void SetEnabled(bool enabled)
        {
            isEnabled = enabled;

            if (!isEnabled)
            {
                color = Color.clear;
                title = "";
                msg = "";
                logo = null;
                anchor = XHudSceneActivateMarkAnchor.左下;
            }
        }
        /// <summary>
        /// 获取标识功能是否启用。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>直接返回内部开关状态。</description></item>
        /// </list>
        /// </summary>
        /// <returns>true=启用, false=禁用。</returns>
        public static bool GetEnabled()
        {
            return isEnabled;
        }
        /// <summary>
        /// 设置功能的启用状态并更新标识内容。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>未传颜色时使用主题主色；未传说明/标题时使用「-」；未传 Logo 时使用内置图标。</description></item>
        /// <item><description>最终调用 <c>SetEnabled(bool)</c> 应用启用状态。</description></item>
        /// </list>
        /// </summary>
        /// <param name="state">要设置的启用状态。</param>
        /// <param name="x_color">边框颜色。</param>
        /// <param name="x_title">标题文本。</param>
        /// <param name="x_msg">说明文本。</param>
        /// <param name="x_logo">Logo 图标。</param>
        /// <param name="x_size">Logo 尺寸。</param>
        /// <param name="x_anchor">锚点位置。</param>
        public static void SetEnabled(bool state, Color x_color = default, string x_title = "", string x_msg = "", Texture2D x_logo = null, Vector2 x_size = default, XHudSceneActivateMarkAnchor x_anchor = XHudSceneActivateMarkAnchor.左下)
        {
            if (x_color == Color.clear)
                color = XHud_Dashboard.themes_get_color(ThemeColorType.primary);
            else
                color = x_color;

            if (string.IsNullOrEmpty(x_msg))
                msg = "-";
            else
                msg = x_msg;

            if (string.IsNullOrEmpty(x_title))
                title = "-";
            else
                title = x_title;

            if (x_logo == null)
                logo = XGUI.GetBasedIcon("icon_strike");
            else
                logo = x_logo;

            anchor = x_anchor;

            if (x_size != Vector2.zero)
                logo_size = x_size;

            SetEnabled(state);
        }
        #endregion
    }
}