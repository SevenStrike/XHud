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
        #region 绘制 - 遮罩 / 内容透明度
        /// <summary>
        /// 绘制「遮罩 / 内容透明度」面板内容。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>绘制射线阻挡/穿透状态图标。</description></item>
        /// <item><description>绘制透明度、遮挡阈值、颜色、图形字段。</description></item>
        /// <item><description>绘制屏幕/世界内容透明度字段，并在变更时刷新。</description></item>
        /// </list>
        /// </summary>
        private void Draw_Mask()
        {
            XGUI.ChangedCheck_Start();

            XGUI.layout_state_displayer_icon(
                  title: MaskRaycastEnabled.boolValue ? "射线阻挡" : "射线穿透",
                  title_size: XGUIFontSize.M,
                  icon: MaskRaycastEnabled.boolValue ? icon_ray_masked : icon_ray_normal,
                  icon_size: new Vector2(12, 12),
                  icon_offset: new Vector2(0, 0),
                  icon_color: MaskRaycastEnabled.boolValue ? Color.red : XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                  padding: new RectOffset(0, 0, 0, 0),
                  margin: new RectOffset(5, 5, 0, 5));

            #region 透明度
            XGUI.layout_property_field(
             title: "透明度",
             title_size: XGUIFontSize.M,
             title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
             title_width: 90,
             prop: MaskAlpha,
             prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 遮挡阈值
            XGUI.layout_property_field(
            title: "遮挡阈值",
            title_size: XGUIFontSize.M,
            title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
            title_width: 90,
            prop: MaskRaycastAlphaThreshold,
            prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 颜色
            XGUI.layout_colorfield(
                prop: MaskColor,
                title: "颜色",
                title_width: 90,
                state_title: "Hex：",
                state_value: $"#{XGUI_Utilitys.Color_To_HexString(MaskColor.colorValue)}",
                state_value_color: MaskColor.colorValue);
            #endregion

            #region 图形
            XGUI.layout_property_field(
            title: "图形",
            title_size: XGUIFontSize.M,
            title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
            title_width: 90,
            prop: MaskTexture,
            prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            XGUI.layout_seperator(
               thickness: 1,
               color: XHud_Dashboard.themes_get_color(ThemeColorType.sepline),
               margin: new RectOffset(15, 15, 15, 15));

            #region 透明度 - 屏幕
            XGUI.layout_property_field(
            title: "透明度-屏幕",
            title_size: XGUIFontSize.M,
            title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
            title_width: 90,
            prop: ContentOpacity_Screen,
            prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 透明度 - 世界
            XGUI.layout_property_field(
            title: "透明度-世界",
            title_size: XGUIFontSize.M,
            title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
            title_width: 90,
            prop: ContentOpacity_World,
            prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            if (XGUI.ChangedCheck_End())
            {
                SetHighFrequencyMode();
                xHud_EditorUpdate_Mask();
                xHud_EditorUpdate_ContentOpacity();
            }
        }
        #endregion
    }
}