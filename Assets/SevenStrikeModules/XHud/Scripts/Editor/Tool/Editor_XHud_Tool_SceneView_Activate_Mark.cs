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

    [InitializeOnLoad]
    public static class Editor_XHud_Tool_SceneView_Activate_Mark
    {
        public static Color color;
        public static string title;
        public static string msg;
        public static Texture2D logo;
        public static XHudSceneActivateMarkAnchor anchor;
        public static Vector2 logo_size = new Vector2(48, 48);

        static Rect rect_logo;
        static Rect rect_title;
        static Rect rect_msg;

        /// <summary>
        /// 静态初始化
        /// </summary>
        static Editor_XHud_Tool_SceneView_Activate_Mark()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        /// <summary>
        /// Scene视图绘制事件回调
        /// </summary>
        /// <param name="sceneView">当前的Scene视图实例</param>
        private static void OnSceneGUI(SceneView sceneView)
        {
            // 如果功能被禁用，不处理任何事件
            if (!isEnabled) return;

            DrawRect(sceneView);
            sceneView.Repaint();
        }

        #region Draw
        /// <summary>
        /// 在Scene视图上绘制红色矩形边框
        /// </summary>
        /// <param name="sv">Scene视图实例，用于获取视图位置和状态信息</param>
        /// <remarks>
        /// 绘制四条独立边框，适配Scene视图的工具栏布局：
        /// <list type="bullet">
        /// <item><description>上边、左边、右边：紧贴视图边缘绘制</description></item>
        /// <item><description>下边：根据视图最大化状态动态调整偏移量，避免被底部工具栏遮挡</description></item>
        /// </list>
        /// </remarks>
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
                    color: XHud_Dashboard.Theme_Primary);
            }
            #endregion

            #region 标题
            XGUI.gui_label(
                rect: rect_title,
                bg_fill: XGUIFilled.实体,
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
        /// 是否启动拖拽放置功能
        /// </summary>
        private static bool isEnabled = false;
        /// <summary>
        /// 设置拖拽功能是否启用
        /// </summary>
        /// <param name="enabled">true=启用, false=禁用</param>
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
        /// 获取拖拽功能是否启用
        /// </summary>
        /// <returns>true=启用, false=禁用</returns>
        public static bool GetEnabled()
        {
            return isEnabled;
        }
        /// <summary>
        /// 设置功能的启用状态，并更新边框颜色
        /// </summary>
        /// <param name="state">要设置的启用状态（true=启用，false=禁用）</param>
        /// <param name="x_color">要设置的边框颜色</param>
        /// <remarks>
        /// 将启用状态设置为指定值，同时将边框颜色更新为指定颜色
        /// </remarks>
        public static void SetEnabled(bool state, Color x_color = default, string x_title = "", string x_msg = "", Texture2D x_logo = null, Vector2 x_size = default, XHudSceneActivateMarkAnchor x_anchor = XHudSceneActivateMarkAnchor.左下)
        {
            if (x_color == Color.clear)
                color = XHud_Dashboard.Theme_Primary;
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