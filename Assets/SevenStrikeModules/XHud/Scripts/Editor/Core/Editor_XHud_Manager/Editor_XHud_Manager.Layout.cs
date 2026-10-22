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
        #region 绘制 - 布局
        /// <summary>
        /// 绘制「布局」面板内容。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>绘制上下左右边距字段。</description></item>
        /// <item><description>绘制水平、垂直、整体缩放字段。</description></item>
        /// <item><description>绘制图元ID标识高度、尺寸字段与线/文字颜色。</description></item>
        /// </list>
        /// </summary>
        private void Draw_Layout()
        {
            #region 边距
            SerializedProperty sp_mar_x = Margins.FindPropertyRelative("x");
            SerializedProperty sp_mar_y = Margins.FindPropertyRelative("y");
            SerializedProperty sp_mar_z = Margins.FindPropertyRelative("z");
            SerializedProperty sp_mar_w = Margins.FindPropertyRelative("w");

            XGUI.layout_property_field(
                title: "上边距",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 110,
                prop: sp_mar_x,
                prop_margin: new RectOffset(0, 0, 5, 0));

            XGUI.layout_property_field(
                title: "下边距",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 110,
                prop: sp_mar_y,
                prop_margin: new RectOffset(0, 0, 5, 0));

            XGUI.layout_property_field(
                title: "左边距",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 110,
                prop: sp_mar_z,
                prop_margin: new RectOffset(0, 0, 5, 0));

            XGUI.layout_property_field(
                title: "右边距",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 110,
                prop: sp_mar_w,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 水平缩放
            XGUI.layout_property_field(
                title: "水平缩放",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 110,
                prop: MarginHorizontal,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 垂直缩放
            XGUI.layout_property_field(
                title: "垂直缩放",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 110,
                prop: MarginVertical,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 整体缩放
            XGUI.layout_property_field(
                title: "整体缩放",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 110,
                prop: MarginMultiply,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 图元ID标识高度
            XGUI.layout_property_field(
                title: "图元ID标识高度",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 110,
                prop: PrimtiveID_LabelLine_Height,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 图元ID标识尺寸
            XGUI.layout_property_field(
                title: "图元ID标识尺寸",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 110,
                prop: PrimtiveID_LabelFont_Size,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion                  

            XGUI.layout_space(10);

            #region 图元ID标识颜色 - 线
            XGUI.layout_colorfield(
                prop: PrimtiveID_LabelLine_Color,
                title: "图元ID标识颜色 - 线",
                title_width: 140,
                //title_color: Color.white,
                //icon: "icon_field_status",
                //icon_color: Color.red,
                state_title: "Hex：",
                state_value: $"#{XGUI_Utilitys.Color_To_HexString(PrimtiveID_LabelLine_Color.colorValue)}",
                state_value_color: PrimtiveID_LabelLine_Color.colorValue);
            #endregion

            #region 图元ID标识颜色 - 文字
            XGUI.layout_colorfield(
                prop: PrimtiveID_LabelFont_Color,
                title: "图元ID标识颜色 - 文字",
                title_width: 140,
                //title_color: Color.white,
                //icon: "icon_field_status",
                //icon_color: Color.red,
                state_title: "Hex：",
                state_value: $"#{XGUI_Utilitys.Color_To_HexString(PrimtiveID_LabelFont_Color.colorValue)}",
                state_value_color: PrimtiveID_LabelFont_Color.colorValue);
            #endregion
        }
        #endregion
    }
}