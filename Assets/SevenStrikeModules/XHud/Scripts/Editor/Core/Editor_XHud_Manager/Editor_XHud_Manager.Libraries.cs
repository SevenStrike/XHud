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

    public partial class Editor_XHud_Manager : Editor
    {
        #region 绘制 - 资源库
        /// <summary>
        /// 绘制「资源库」面板内容。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>绘制色卡库、曲线库、音效库字段。</description></item>
        /// <item><description>绘制字体库、动效库、转场库字段。</description></item>
        /// </list>
        /// </summary>
        private void Draw_Libraries()
        {
            XGUI.layout_property_field(
                title: "色卡库",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 100,
                prop: sp_library_colors,
                status_icon: "icon_field_status",
                status_icon_color: sp_library_colors.objectReferenceValue != null ? XHud_Dashboard.themes_get_color(ThemeColorType.primary) : Color.black,
                prop_margin: new RectOffset(0, 0, 10, 10));

            XGUI.layout_property_field(
                title: "曲线库",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 100,
                prop: sp_library_curves,
                status_icon: "icon_field_status",
                status_icon_color: sp_library_curves.objectReferenceValue != null ? XHud_Dashboard.themes_get_color(ThemeColorType.primary) : Color.black,
                prop_margin: new RectOffset(0, 0, 10, 10));

            XGUI.layout_property_field(
                title: "音效库",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 100,
                prop: sp_library_sounds,
                status_icon: "icon_field_status",
                status_icon_color: sp_library_sounds.objectReferenceValue != null ? XHud_Dashboard.themes_get_color(ThemeColorType.primary) : Color.black,
                prop_margin: new RectOffset(0, 0, 10, 10));

            XGUI.layout_property_field(
                title: "字体库",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 100,
                prop: sp_library_textstyles,
                status_icon: "icon_field_status",
                status_icon_color: sp_library_textstyles.objectReferenceValue != null ? XHud_Dashboard.themes_get_color(ThemeColorType.primary) : Color.black,
                prop_margin: new RectOffset(0, 0, 10, 10));

            XGUI.layout_property_field(
                title: "动效库",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 100,
                prop: sp_library_motions,
                status_icon: "icon_field_status",
                status_icon_color: sp_library_motions.objectReferenceValue != null ? XHud_Dashboard.themes_get_color(ThemeColorType.primary) : Color.black,
                prop_margin: new RectOffset(0, 0, 10, 10));

            XGUI.layout_property_field(
                title: "转场库",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 100,
                prop: sp_library_transitions,
                status_icon: "icon_field_status",
                status_icon_color: sp_library_transitions.objectReferenceValue != null ? XHud_Dashboard.themes_get_color(ThemeColorType.primary) : Color.black,
                prop_margin: new RectOffset(0, 0, 10, 10));

            XGUI.layout_space(5);
        }
        #endregion
    }
}