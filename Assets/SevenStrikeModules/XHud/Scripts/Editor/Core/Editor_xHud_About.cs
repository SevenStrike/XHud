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
    using System;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// XHud 关于窗口。
    /// <para/>
    /// <list type="bullet">
    /// <item><description>通过菜单 <c>Tools/XHud/About #a</c> 打开。</description></item>
    /// <item><description>展示 Logo、版本信息、简介、官网与教程手册按钮。</description></item>
    /// </list>
    /// </summary>
    public class Editor_XHud_About : EditorWindow
    {
        #region 字段 - 组件 / 列表
        /// <summary>
        /// 当前窗口实例。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>在 <c>Init</c> 中赋值。</description></item>
        /// <item><description>用于设置窗口尺寸与显示。</description></item>
        /// </list>
        /// </summary>
        static Editor_XHud_About Window;
        #endregion

        #region 字段 - 序列化属性
        /// <summary>
        /// 版本信息数据。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>在 <c>OnEnable</c> 中从 JSON 反序列化。</description></item>
        /// <item><description>用于在 <c>OnGUI</c> 中展示版本、简介等信息。</description></item>
        /// </list>
        /// </summary>
        public versioninfo infos;
        #endregion

        #region 字段 - 图标
        /// <summary>
        /// 关于窗口 Logo 图标。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>在 <c>OnEnable</c> 中加载。</description></item>
        /// </list>
        /// </summary>
        private Texture2D icon_logo;
        #endregion

        #region 生命周期   
        [MenuItem("Tools/XHud/About #a")]
        static void Init()
        {
            Window = (Editor_XHud_About)EditorWindow.GetWindow(typeof(Editor_XHud_About), true, "关于XHud的信息", true);
            Window.minSize = new Vector2(350, 430);
            Window.maxSize = Window.minSize;
            Window.Show();
        }

        private void OnEnable()
        {
            infos = XHud_Dashboard.get_xhud_info();

            #region 获取图标
            icon_logo = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_about/logo");
            #endregion
        }

        private void OnGUI()
        {
            XGUI.layout_icon(
                icon: icon_logo,
                icon_color: Color.white,
                //icon_size: 16,
                width: icon_logo.width,
                height: icon_logo.height,
                icon_offset: new Vector2(0, 0),
                icon_margin: new RectOffset(0, 0, 50, 0),
                icon_padding: new RectOffset(0, 0, 0, 0),
                icon_alignment: XGUIIconAlignment.中心);
            XGUI.layout_label(
                text: $"{infos.sub}  /  Version : {infos.version}",
                size: XGUIFontSize.M,
                text_color: Color.white * 0.65f,
                margin: new RectOffset(0, 0, 20, 0),
                offset: new Vector2(0, 1),
                clipping: TextClipping.Clip,
                font_style: FontStyle.Normal,
                anchor: TextAnchor.MiddleCenter,
                font: XGUI.GetFont("xg-medium"));
            XGUI.layout_seperator(
                thickness: 1,
                color: Color.white * 0.5f,
                margin: new RectOffset(30, 30, 30, 30),
                padding: new RectOffset(0, 0, 0, 0));
            XGUI.layout_label(
                text: infos.support,
                size: XGUIFontSize.M,
                text_color: Color.white * 0.7f,
                margin: new RectOffset(0, 0, 0, 10),
                offset: new Vector2(0, 1),
                clipping: TextClipping.Clip,
                font_style: FontStyle.Normal,
                anchor: TextAnchor.MiddleCenter,
                font: XGUI.GetFont("xg-medium"));
            XGUI.layout_label(
                text: infos.intro,
                size: XGUIFontSize.M,
                text_color: Color.white * 0.9f,
                margin: new RectOffset(20, 20, 0, 0),
                offset: new Vector2(0, 1),
                clipping: TextClipping.Clip,
                wrap: true,
                font_style: FontStyle.Normal,
                anchor: TextAnchor.MiddleLeft,
                font: XGUI.GetFont("xg-medium"));
            XGUI.layout_group_start(
                type: XGUIContainerType.Horizontal,
                absolute_margin: true,
                margin: new RectOffset(15, 15, 40, 0),
                padding: new RectOffset(0, 0, 0, 0));
            if (XGUI.layout_button(
                text: "访问XHud官网",
                tooltip: "",
                bg_fill: XGUIGroup.实体,
                bg_color: XGUIColor.亮白,
                bg_color_gui: Color.white,
                button_text_color: Color.black,
                press_fill: XGUIGroup.实体,
                press_color: XGUIColor.深空灰,
                press_text_color: Color.white,
                font_size: XGUIFontSize.B,
                anchor: TextAnchor.MiddleCenter,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                height: 25,
                button_text_font: XGUI.GetFont("xg-medium")))
            {
                XGUI_Utilitys.OpenURL(infos.website);
            }
            XGUI.layout_space(10);
            if (XGUI.layout_button(
                text: "教程手册",
                tooltip: "",
                bg_fill: XGUIGroup.实体,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                button_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(XHud_Dashboard.themes_get_color(ThemeColorType.primary)) ? Color.black : Color.white,
                press_fill: XGUIGroup.实体,
                press_color: XGUIColor.深空灰,
                press_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(XHud_Dashboard.themes_get_color(ThemeColorType.primary)) ? Color.white : Color.black,
                font_size: XGUIFontSize.B,
                anchor: TextAnchor.MiddleCenter,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                height: 25,
                button_text_font: XGUI.GetFont("xg-medium")))
            {
                XGUI_Utilitys.OpenURL(infos.document);
            }
            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
        }
        #endregion
    }
}