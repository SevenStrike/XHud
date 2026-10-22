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
        #region 绘制 - 构图参考
        /// <summary>
        /// 绘制「构图参考」面板内容。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>绘制辅助颜色（线/点）字段。</description></item>
        /// <item><description>绘制构图模式下拉与帮助按钮。</description></item>
        /// <item><description>按当前构图模式绘制对应的参数子面板与复位按钮。</description></item>
        /// </list>
        /// </summary>
        private void Draw_CompGuide()
        {
            float currentWidth = XGUI.GetCurrentWindowWidth();

            #region 构图辅助颜色 - 线
            XGUI.layout_colorfield(
                prop: GuideColor,
                title: "辅助颜色 - 线",
                title_width: 140,
                //title_color: Color.white,
                //icon: "icon_field_status",
                //icon_color: Color.red,
                state_title: "Hex：",
                state_value: $"#{XGUI_Utilitys.Color_To_HexString(GuideColor.colorValue)}",
                state_value_color: GuideColor.colorValue);
            #endregion

            #region 构图辅助颜色 - 点
            XGUI.layout_colorfield(
                prop: GuidePointColor,
                title: "辅助颜色 - 点",
                title_width: 140,
                //title_color: Color.white,
                //icon: "icon_field_status",
                //icon_color: Color.red,
                state_title: "Hex：",
                state_value: $"#{XGUI_Utilitys.Color_To_HexString(GuidePointColor.colorValue)}",
                state_value_color: GuidePointColor.colorValue);
            #endregion

            XGUI.layout_seperator(
                thickness: 1,
                color: XHud_Dashboard.themes_get_color(ThemeColorType.sepline),
                margin: new RectOffset(15, 15, 5, 15));

            #region 构图模式       
            if (currentWidth > 260)
            {
                XGUI.layout_group_start(
                    type: XGUIContainerType.Horizontal,
                    absolute_margin: true,
                    absolute_padding: true,
                    margin: new RectOffset(0, 0, 0, 0),
                    padding: new RectOffset(0, 0, 0, 0));
            }

            XGUI.layout_string_popup(
               title: "构图模式",
               title_width: 60,
               title_size: XGUIFontSize.M,
               title_anchor: TextAnchor.MiddleLeft,
               prop: CompGuideMode,
               options: compguid_modes,
               opt_text_size: XGUIFontSize.M,
               opt_text_color: Color.black,
               opt_text_padding: new RectOffset(10, 10, 0, 0),
               opt_anchor: TextAnchor.MiddleCenter,
               opt_font_style: FontStyle.Normal,
               opt_bg_fill: XGUIGroup.实体,
               opt_bg_color: XGUIColor.亮白,
               opt_bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
               icon_arrow_color: Color.black,
               margin: new RectOffset(0, 0, 5, 5),
               padding: new RectOffset(5, 5, 0, 0),
               title_margin: new RectOffset(0, 0, 0, 0),
               act_on_changed: (value) =>
               {

               });

            if (currentWidth > 260)
            {
                if (XGUI.layout_button(
                    tooltip: "查看",
                    tex_release: GuideHelp_Released,
                    tex_press: GuideHelp_Press,
                    tex_gui_color: Color.white,
                    border: new RectOffset(0, 0, 0, 0),
                    margin: new RectOffset(5, 0, 1, 0),
                    width: 15,
                    height: 15))
                {
                    xHud_OpenGuideHelpPanel(CompGuideMode.stringValue);
                }
            }

            if (currentWidth > 260)
                XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            #endregion

            XGUI.layout_seperator(
                thickness: 1,
                color: XHud_Dashboard.themes_get_color(ThemeColorType.sepline),
                margin: new RectOffset(15, 15, 15, 10));

            switch (CompGuideMode.stringValue)
            {
                case "水平对称":
                    XGUI.layout_space(5);
                    string[] dir_lr = new string[3] { "自定义对称分割", "靠上黄金比例", "靠下黄金比例" };
                    string mode_lr = XGUI.layout_string_popup(
                        title: "对称方式",
                        title_width: 60,
                        title_size: XGUIFontSize.M,
                        title_anchor: TextAnchor.MiddleLeft,
                        prop: GuideParam_Mirror_UD_GoldenMode,
                        options: dir_lr,
                        opt_text_size: XGUIFontSize.M,
                        opt_text_color: Color.black,
                        opt_text_padding: new RectOffset(10, 10, 0, 0),
                        opt_anchor: TextAnchor.MiddleCenter,
                        opt_font_style: FontStyle.Normal,
                        opt_bg_fill: XGUIGroup.实体,
                        opt_bg_color: XGUIColor.亮白,
                        opt_bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        icon_arrow_color: Color.black,
                        margin: new RectOffset(0, 0, 5, 5),
                        padding: new RectOffset(5, 5, 0, 0),
                        title_margin: new RectOffset(0, 0, 0, 0),
                        act_on_changed: (value) =>
                        {

                        });

                    if (mode_lr == "自定义对称分割")
                    {
                        XGUI.layout_space(5);
                        XGUI.layout_property_field(
                            title: "对称偏移校正",
                            title_size: XGUIFontSize.M,
                            //title_color: Color.white,
                            title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                            title_width: 90,
                            //status_icon: "icon_field_status",
                            prop: GuideParam_Mirror_UD_Offset,
                            prop_margin: new RectOffset(0, 0, 5, 0));
                    }

                    XGUI.layout_space(5);

                    XGUI.layout_property_field(
                        title: "中心点尺寸",
                        title_size: XGUIFontSize.M,
                        //title_color: Color.white,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        //status_icon: "icon_field_status",
                        prop: GuideParam_CenterPointSize,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(15);

                    #region 复位按钮
                    if (XGUI.layout_button(
                        text: "复位校正",
                        tooltip: "",
                        bg_fill: XGUIGroup.实体,
                        bg_color: XGUIColor.深空灰,
                        bg_color_gui: Color.white,
                        button_text_color: color_important,
                        press_fill: XGUIGroup.实体,
                        press_color: XGUIColor.深空灰,
                        press_text_color: Color.white,
                        font_size: XGUIFontSize.M,
                        anchor: TextAnchor.MiddleCenter,
                        margin: new RectOffset(0, 0, 0, 0),
                        padding: new RectOffset(0, 0, 0, 0),
                        height: 25,
                        button_text_font: XGUI.GetFont("xg-medium")))
                    {
                        GuideParam_Mirror_UD_GoldenMode.stringValue = "自定义对称分割";
                        GuideParam_Mirror_UD_GoldenMode.serializedObject.ApplyModifiedProperties();

                        GuideParam_Mirror_UD_Offset.floatValue = 0;
                        GuideParam_Mirror_UD_Offset.serializedObject.ApplyModifiedProperties();

                        GuideParam_CenterPointSize.floatValue = 1;
                        GuideParam_CenterPointSize.serializedObject.ApplyModifiedProperties();
                    }
                    #endregion
                    break;
                case "垂直对称":
                    XGUI.layout_space(5);

                    string[] dir_ud = new string[3] { "自定义对称分割", "靠左黄金比例", "靠右黄金比例" };

                    string mode_ud = XGUI.layout_string_popup(
                        title: "对称方式",
                        title_width: 60,
                        title_size: XGUIFontSize.M,
                        title_anchor: TextAnchor.MiddleLeft,
                        prop: GuideParam_Mirror_LR_GoldenMode,
                        options: dir_ud,
                        opt_text_size: XGUIFontSize.M,
                        opt_text_color: Color.black,
                        opt_text_padding: new RectOffset(10, 10, 0, 0),
                        opt_anchor: TextAnchor.MiddleCenter,
                        opt_font_style: FontStyle.Normal,
                        opt_bg_fill: XGUIGroup.实体,
                        opt_bg_color: XGUIColor.亮白,
                        opt_bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        icon_arrow_color: Color.black,
                        margin: new RectOffset(0, 0, 5, 5),
                        padding: new RectOffset(5, 5, 0, 0),
                        title_margin: new RectOffset(0, 0, 0, 0),
                        act_on_changed: (value) =>
                        {

                        });

                    if (mode_ud == "自定义对称分割")
                    {
                        XGUI.layout_space(5);

                        XGUI.layout_property_field(
                            title: "对称偏移校正",
                            title_size: XGUIFontSize.M,
                            //title_color: Color.white,
                            title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                            title_width: 90,
                            //status_icon: "icon_field_status",
                            prop: GuideParam_Mirror_LR_Offset,
                            prop_margin: new RectOffset(0, 0, 5, 0));
                    }

                    XGUI.layout_space(5);

                    XGUI.layout_property_field(
                        title: "中心点尺寸",
                        title_size: XGUIFontSize.M,
                        //title_color: Color.white,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        //status_icon: "icon_field_status",
                        prop: GuideParam_CenterPointSize,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(15);

                    #region 复位按钮
                    if (XGUI.layout_button(
                        text: "复位校正",
                        tooltip: "",
                        bg_fill: XGUIGroup.实体,
                        bg_color: XGUIColor.深空灰,
                        bg_color_gui: Color.white,
                        button_text_color: color_important,
                        press_fill: XGUIGroup.实体,
                        press_color: XGUIColor.深空灰,
                        press_text_color: Color.white,
                        font_size: XGUIFontSize.M,
                        anchor: TextAnchor.MiddleCenter,
                        margin: new RectOffset(0, 0, 0, 0),
                        padding: new RectOffset(0, 0, 0, 0),
                        height: 25,
                        button_text_font: XGUI.GetFont("xg-medium")))
                    {
                        GuideParam_Mirror_LR_GoldenMode.stringValue = "自定义对称分割";
                        GuideParam_Mirror_LR_GoldenMode.serializedObject.ApplyModifiedProperties();

                        GuideParam_Mirror_LR_Offset.floatValue = 0;
                        GuideParam_Mirror_LR_Offset.serializedObject.ApplyModifiedProperties();

                        GuideParam_CenterPointSize.floatValue = 1;
                        GuideParam_CenterPointSize.serializedObject.ApplyModifiedProperties();
                    }
                    #endregion
                    break;
                case "黄金螺旋":
                    XGUI.layout_space(5);

                    string[] dir_fb = new string[4] { "右上", "左上", "右下", "左下" };

                    string mode_fb = XGUI.layout_string_popup(
                       title: "对称方式",
                       title_width: 60,
                       title_size: XGUIFontSize.M,
                       title_anchor: TextAnchor.MiddleLeft,
                       prop: GuideParam_Fibonacci_Mode,
                       options: dir_fb,
                       opt_text_size: XGUIFontSize.M,
                       opt_text_color: Color.black,
                       opt_text_padding: new RectOffset(10, 10, 0, 0),
                       opt_anchor: TextAnchor.MiddleCenter,
                       opt_font_style: FontStyle.Normal,
                       opt_bg_fill: XGUIGroup.实体,
                       opt_bg_color: XGUIColor.亮白,
                       opt_bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                       icon_arrow_color: Color.black,
                       margin: new RectOffset(0, 0, 5, 5),
                       padding: new RectOffset(5, 5, 0, 0),
                       title_margin: new RectOffset(0, 0, 0, 0),
                       act_on_changed: (value) =>
                       {

                       });

                    XGUI.layout_space(15);

                    #region 复位按钮
                    if (XGUI.layout_button(
                        text: "复位校正",
                        tooltip: "",
                        bg_fill: XGUIGroup.实体,
                        bg_color: XGUIColor.深空灰,
                        bg_color_gui: Color.white,
                        button_text_color: color_important,
                        press_fill: XGUIGroup.实体,
                        press_color: XGUIColor.深空灰,
                        press_text_color: Color.white,
                        font_size: XGUIFontSize.M,
                        anchor: TextAnchor.MiddleCenter,
                        margin: new RectOffset(0, 0, 0, 0),
                        padding: new RectOffset(0, 0, 0, 0),
                        height: 25,
                        button_text_font: XGUI.GetFont("xg-medium")))
                    {
                        GuideParam_Fibonacci_Mode.stringValue = "右上";
                        GuideParam_Fibonacci_Mode.serializedObject.ApplyModifiedProperties();
                    }
                    #endregion
                    break;
                case "对角线":
                    XGUI.layout_space(5);

                    XGUI.layout_property_field(
                        title: "水平偏移",
                        title_size: XGUIFontSize.M,
                        //title_color: Color.white,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        //status_icon: "icon_field_status",
                        prop: GuideParam_CornerLookat_Offset_H,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    XGUI.layout_property_field(
                        title: "垂直偏移",
                        title_size: XGUIFontSize.M,
                        //title_color: Color.white,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        //status_icon: "icon_field_status",
                        prop: GuideParam_CornerLookat_Offset_V,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    XGUI.layout_property_field(
                        title: "中心点尺寸",
                        title_size: XGUIFontSize.M,
                        //title_color: Color.white,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        //status_icon: "icon_field_status",
                        prop: GuideParam_CenterPointSize,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(15);

                    #region 复位按钮
                    if (XGUI.layout_button(
                        text: "复位校正",
                        tooltip: "",
                        bg_fill: XGUIGroup.实体,
                        bg_color: XGUIColor.深空灰,
                        bg_color_gui: Color.white,
                        button_text_color: color_important,
                        press_fill: XGUIGroup.实体,
                        press_color: XGUIColor.深空灰,
                        press_text_color: Color.white,
                        font_size: XGUIFontSize.M,
                        anchor: TextAnchor.MiddleCenter,
                        margin: new RectOffset(0, 0, 0, 0),
                        padding: new RectOffset(0, 0, 0, 0),
                        height: 25,
                        button_text_font: XGUI.GetFont("xg-medium")))
                    {
                        GuideParam_CornerLookat_Offset_H.floatValue = 0;
                        GuideParam_CornerLookat_Offset_H.serializedObject.ApplyModifiedProperties();

                        GuideParam_CornerLookat_Offset_V.floatValue = 0;
                        GuideParam_CornerLookat_Offset_V.serializedObject.ApplyModifiedProperties();

                        GuideParam_CenterPointSize.floatValue = 1;
                        GuideParam_CenterPointSize.serializedObject.ApplyModifiedProperties();
                    }
                    #endregion
                    break;
                case "三分线":
                    XGUI.layout_space(5);

                    XGUI.layout_property_field(
                        title: "水平伸缩",
                        title_size: XGUIFontSize.M,
                        //title_color: Color.white,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        //status_icon: "icon_field_status",
                        prop: GuideParam_Three_Offset_H,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    XGUI.layout_property_field(
                        title: "垂直伸缩",
                        title_size: XGUIFontSize.M,
                        //title_color: Color.white,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        //status_icon: "icon_field_status",
                        prop: GuideParam_Three_Offset_V,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    XGUI.layout_property_field(
                        title: "整体伸缩",
                        title_size: XGUIFontSize.M,
                        //title_color: Color.white,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        //status_icon: "icon_field_status",
                        prop: GuideParam_Three_Offset_Coverage,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    #region 平均等分
                    if (XGUI.layout_button(
                        text: "平均等分",
                        tooltip: "",
                        bg_fill: XGUIGroup.实体,
                        bg_color: XGUIColor.深空灰,
                        bg_color_gui: Color.white,
                        button_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        press_fill: XGUIGroup.实体,
                        press_color: XGUIColor.深空灰,
                        press_text_color: Color.white,
                        font_size: XGUIFontSize.M,
                        anchor: TextAnchor.MiddleCenter,
                        margin: new RectOffset(0, 0, 0, 0),
                        padding: new RectOffset(0, 0, 0, 0),
                        height: 25,
                        button_text_font: XGUI.GetFont("xg-medium")))
                    {
                        GuideParam_Three_Offset_Coverage.floatValue = 1;
                        GuideParam_Three_Offset_Coverage.serializedObject.ApplyModifiedProperties();

                        float seg_x = ScreenRes.vector2Value.x / 3;
                        GuideParam_Three_Offset_H.floatValue = seg_x / ScreenRes.vector2Value.x * 2f;
                        GuideParam_Three_Offset_H.serializedObject.ApplyModifiedProperties();

                        float seg_y = ScreenRes.vector2Value.y / 3;
                        GuideParam_Three_Offset_V.floatValue = seg_y / ScreenRes.vector2Value.y * 2f;
                        GuideParam_Three_Offset_V.serializedObject.ApplyModifiedProperties();
                    }
                    #endregion

                    XGUI.layout_space(15);

                    #region 复位按钮
                    if (XGUI.layout_button(
                        text: "复位校正",
                        tooltip: "",
                        bg_fill: XGUIGroup.实体,
                        bg_color: XGUIColor.深空灰,
                        bg_color_gui: Color.white,
                        button_text_color: color_important,
                        press_fill: XGUIGroup.实体,
                        press_color: XGUIColor.深空灰,
                        press_text_color: Color.white,
                        font_size: XGUIFontSize.M,
                        anchor: TextAnchor.MiddleCenter,
                        margin: new RectOffset(0, 0, 0, 0),
                        padding: new RectOffset(0, 0, 0, 0),
                        height: 25,
                        button_text_font: XGUI.GetFont("xg-medium")))
                    {
                        GuideParam_Three_Offset_H.floatValue = 1;
                        GuideParam_Three_Offset_H.serializedObject.ApplyModifiedProperties();

                        GuideParam_Three_Offset_V.floatValue = 1;
                        GuideParam_Three_Offset_V.serializedObject.ApplyModifiedProperties();

                        GuideParam_Three_Offset_Coverage.floatValue = 0.5f;
                        GuideParam_Three_Offset_Coverage.serializedObject.ApplyModifiedProperties();

                        GuideParam_CenterPointSize.floatValue = 1;
                        GuideParam_CenterPointSize.serializedObject.ApplyModifiedProperties();
                    }
                    #endregion
                    break;
                case "引导线":
                    XGUI.layout_space(5);

                    XGUI.layout_property_field(
                        title: "基准水平位移",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: GuideParam_GuideLine_BaseOffset,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    XGUI.layout_property_field(
                        title: "基准水平高度",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: GuideParam_GuideLine_BaseHeight,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    XGUI.layout_property_field(
                        title: "近景高度",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: GuideParam_GuideLine_Offset_NearHeight,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    XGUI.layout_property_field(
                        title: "近景伸缩",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: GuideParam_GuideLine_Offset_Near,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    XGUI.layout_property_field(
                        title: "远景伸缩",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: GuideParam_GuideLine_Offset_Far,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    XGUI.layout_property_field(
                        title: "中心点尺寸",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: GuideParam_CenterPointSize,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(15);

                    #region 复位按钮
                    if (XGUI.layout_button(
                        text: "复位校正",
                        tooltip: "",
                        bg_fill: XGUIGroup.实体,
                        bg_color: XGUIColor.深空灰,
                        bg_color_gui: Color.white,
                        button_text_color: color_important,
                        press_fill: XGUIGroup.实体,
                        press_color: XGUIColor.深空灰,
                        press_text_color: Color.white,
                        font_size: XGUIFontSize.M,
                        anchor: TextAnchor.MiddleCenter,
                        margin: new RectOffset(0, 0, 0, 0),
                        padding: new RectOffset(0, 0, 0, 0),
                        height: 25,
                        button_text_font: XGUI.GetFont("xg-medium")))
                    {
                        GuideParam_GuideLine_BaseOffset.floatValue = 0;
                        GuideParam_GuideLine_BaseOffset.serializedObject.ApplyModifiedProperties();

                        GuideParam_GuideLine_BaseHeight.floatValue = 0.5f;
                        GuideParam_GuideLine_BaseHeight.serializedObject.ApplyModifiedProperties();

                        GuideParam_GuideLine_Offset_Far.floatValue = 0;
                        GuideParam_GuideLine_Offset_Far.serializedObject.ApplyModifiedProperties();

                        GuideParam_GuideLine_Offset_Near.floatValue = 0;
                        GuideParam_GuideLine_Offset_Near.serializedObject.ApplyModifiedProperties();

                        GuideParam_GuideLine_Offset_NearHeight.floatValue = 0;
                        GuideParam_GuideLine_Offset_NearHeight.serializedObject.ApplyModifiedProperties();

                        GuideParam_CenterPointSize.floatValue = 1;
                        GuideParam_CenterPointSize.serializedObject.ApplyModifiedProperties();
                    }
                    #endregion
                    break;
                case "三角":
                    XGUI.layout_space(5);

                    XGUI.layout_property_field(
                        title: "基准顶部位移",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: GuideParam_Triangle_TopOffset,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    XGUI.layout_property_field(
                        title: "基准顶部高度",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: GuideParam_Triangle_BaseHeight,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    XGUI.layout_property_field(
                        title: "基准底部高度",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: GuideParam_Triangle_BottomHeight,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    XGUI.layout_property_field(
                        title: "左角点偏移",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: GuideParam_Triangle_Offset_Left,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    XGUI.layout_property_field(
                        title: "右角点偏移",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: GuideParam_Triangle_Offset_Right,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    XGUI.layout_property_field(
                        title: "中心点尺寸",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: GuideParam_CenterPointSize,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(15);

                    #region 复位按钮
                    if (XGUI.layout_button(
                        text: "复位校正",
                        tooltip: "",
                        bg_fill: XGUIGroup.实体,
                        bg_color: XGUIColor.深空灰,
                        bg_color_gui: Color.white,
                        button_text_color: color_important,
                        press_fill: XGUIGroup.实体,
                        press_color: XGUIColor.深空灰,
                        press_text_color: Color.white,
                        font_size: XGUIFontSize.M,
                        anchor: TextAnchor.MiddleCenter,
                        margin: new RectOffset(0, 0, 0, 0),
                        padding: new RectOffset(0, 0, 0, 0),
                        height: 25,
                        button_text_font: XGUI.GetFont("xg-medium")))
                    {
                        GuideParam_Triangle_TopOffset.floatValue = 0;
                        GuideParam_Triangle_TopOffset.serializedObject.ApplyModifiedProperties();

                        GuideParam_Triangle_BaseHeight.floatValue = 0.8f;
                        GuideParam_Triangle_BaseHeight.serializedObject.ApplyModifiedProperties();

                        GuideParam_Triangle_BottomHeight.floatValue = 0.2f;
                        GuideParam_Triangle_BottomHeight.serializedObject.ApplyModifiedProperties();

                        GuideParam_Triangle_Offset_Left.floatValue = 0.5f;
                        GuideParam_Triangle_Offset_Left.serializedObject.ApplyModifiedProperties();

                        GuideParam_Triangle_Offset_Right.floatValue = 0.5f;
                        GuideParam_Triangle_Offset_Right.serializedObject.ApplyModifiedProperties();

                        GuideParam_CenterPointSize.floatValue = 1;
                        GuideParam_CenterPointSize.serializedObject.ApplyModifiedProperties();
                    }
                    #endregion
                    break;
                case "工字型":
                    XGUI.layout_space(5);

                    XGUI.layout_property_field(
                        title: "工字顶部高度",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: GuideParam_IShape_TopHeight,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    XGUI.layout_property_field(
                        title: "工字底部高度",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: GuideParam_IShape_BottomHeight,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    XGUI.layout_property_field(
                        title: "工字左偏移",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: GuideParam_IShape_Offset_Left,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    XGUI.layout_property_field(
                        title: "工字右偏移",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: GuideParam_IShape_Offset_Right,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(15);

                    #region 复位按钮
                    if (XGUI.layout_button(
                        text: "复位校正",
                        tooltip: "",
                        bg_fill: XGUIGroup.实体,
                        bg_color: XGUIColor.深空灰,
                        bg_color_gui: Color.white,
                        button_text_color: color_important,
                        press_fill: XGUIGroup.实体,
                        press_color: XGUIColor.深空灰,
                        press_text_color: Color.gray,
                        font_size: XGUIFontSize.M,
                        anchor: TextAnchor.MiddleCenter,
                        margin: new RectOffset(0, 0, 0, 0),
                        padding: new RectOffset(0, 0, 0, 0),
                        height: 25,
                        button_text_font: XGUI.GetFont("xg-medium")))
                    {
                        GuideParam_IShape_TopHeight.floatValue = 0.8f;
                        GuideParam_IShape_TopHeight.serializedObject.ApplyModifiedProperties();

                        GuideParam_IShape_BottomHeight.floatValue = 0.2f;
                        GuideParam_IShape_BottomHeight.serializedObject.ApplyModifiedProperties();

                        GuideParam_IShape_Offset_Left.floatValue = 0.5f;
                        GuideParam_IShape_Offset_Left.serializedObject.ApplyModifiedProperties();

                        GuideParam_IShape_Offset_Right.floatValue = 0.5f;
                        GuideParam_IShape_Offset_Right.serializedObject.ApplyModifiedProperties();

                        GuideParam_CenterPointSize.floatValue = 1;
                        GuideParam_CenterPointSize.serializedObject.ApplyModifiedProperties();
                    }
                    #endregion
                    break;
            }
        }
        #endregion

        #region 工具 - 构图参考结构
        /// <summary>
        /// 初始化构图参考的结构。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>创建 <c>CompGuide</c> 根节点及其 9 个锚点子节点。</description></item>
        /// <item><description>将子节点信息写入 <c>CompGuide_Anchors</c> 序列化数组。</description></item>
        /// </list>
        /// </summary>
        private void xHud_CompGuide_Structure_Create()
        {
            ///-------创建 HudAuxiliary_Anchor
            GameObject obj_Anchors = new GameObject();
            obj_Anchors.layer = LayerMask.NameToLayer("XHud");
            UnityEngine.RectTransform Anchors = obj_Anchors.AddComponent<UnityEngine.RectTransform>();
            CanvasGroup cavgroup = obj_Anchors.AddComponent<CanvasGroup>();
            HudCanvasGroup_Screen.objectReferenceValue = cavgroup;
            HudCanvasGroup_Screen.serializedObject.ApplyModifiedProperties();
            cavgroup.alpha = 1;
            Anchors.name = "CompGuide";
            Anchors.SetParent(BaseScript.HudCanvas_Screen.transform);
            Anchors.localPosition = Vector3.zero;
            Anchors.localScale = Vector3.one;
            Anchors.anchorMin = new Vector2(0, 0);
            Anchors.anchorMax = new Vector2(1, 1);
            Anchors.sizeDelta = new Vector2(0, 0);

            CompGuide_AnchorRoot.objectReferenceValue = Anchors;
            CompGuide_AnchorRoot.serializedObject.ApplyModifiedProperties();

            for (int i = 0; i < 9; i++)
            {
                GameObject obj_Layanchor = new GameObject();
                obj_Layanchor.layer = LayerMask.NameToLayer("XHud");
                UnityEngine.RectTransform layanchor = obj_Layanchor.AddComponent<UnityEngine.RectTransform>();
                layanchor.SetParent(Anchors);
                if (i == 0)
                {
                    layanchor.name = "Up";
                    layanchor.anchorMin = new Vector2(0.5f, 1);
                    layanchor.anchorMax = new Vector2(0.5f, 1);
                    layanchor.pivot = new Vector2(0.5f, 1);
                    layanchor.sizeDelta = new Vector2(20, 20);
                }
                if (i == 1)
                {
                    layanchor.name = "Down";
                    layanchor.anchorMin = new Vector2(0.5f, 0);
                    layanchor.anchorMax = new Vector2(0.5f, 0);
                    layanchor.pivot = new Vector2(0.5f, 0);
                    layanchor.sizeDelta = new Vector2(20, 20);
                }
                if (i == 2)
                {
                    layanchor.name = "Left";
                    layanchor.anchorMin = new Vector2(0, 0.5f);
                    layanchor.anchorMax = new Vector2(0, 0.5f);
                    layanchor.pivot = new Vector2(0, 0.5f);
                    layanchor.sizeDelta = new Vector2(20, 20);
                }
                if (i == 3)
                {
                    layanchor.name = "Right";
                    layanchor.anchorMin = new Vector2(1, 0.5f);
                    layanchor.anchorMax = new Vector2(1, 0.5f);
                    layanchor.pivot = new Vector2(1, 0.5f);
                    layanchor.sizeDelta = new Vector2(20, 20);
                }
                if (i == 4)
                {
                    layanchor.name = "Center";
                    layanchor.anchorMin = new Vector2(0.5f, 0.5f);
                    layanchor.anchorMax = new Vector2(0.5f, 0.5f);
                    layanchor.pivot = new Vector2(0.5f, 0.5f);
                    layanchor.sizeDelta = new Vector2(20, 20);
                }
                if (i == 5)
                {
                    layanchor.name = "Left_Up";
                    layanchor.anchorMin = new Vector2(0, 1);
                    layanchor.anchorMax = new Vector2(0, 1);
                    layanchor.pivot = new Vector2(0, 1);
                    layanchor.sizeDelta = new Vector2(20, 20);
                }
                if (i == 6)
                {
                    layanchor.name = "Left_Down";
                    layanchor.anchorMin = new Vector2(0, 0);
                    layanchor.anchorMax = new Vector2(0, 0);
                    layanchor.pivot = new Vector2(0, 0);
                    layanchor.sizeDelta = new Vector2(20, 20);
                }
                if (i == 7)
                {
                    layanchor.name = "Right_Up";
                    layanchor.anchorMin = new Vector2(1, 1);
                    layanchor.anchorMax = new Vector2(1, 1);
                    layanchor.pivot = new Vector2(1, 1);
                    layanchor.sizeDelta = new Vector2(20, 20);
                }
                if (i == 8)
                {
                    layanchor.name = "Right_Down";
                    layanchor.anchorMin = new Vector2(1, 0);
                    layanchor.anchorMax = new Vector2(1, 0);
                    layanchor.pivot = new Vector2(1, 0);
                    layanchor.sizeDelta = new Vector2(20, 20);
                }

                layanchor.anchoredPosition3D = Vector3.zero;
                layanchor.localScale = Vector3.one;
            }

            #region 获取所有锚点结构 - 布局
            if (CompGuide_Anchors != null && CompGuide_AnchorRoot != null)
            {
                Transform anchorRoot = CompGuide_AnchorRoot.objectReferenceValue as Transform;
                for (int i = 0; i < anchorRoot.childCount; i++)
                {
                    UnityEngine.RectTransform obj = anchorRoot.GetChild(i).GetComponent<UnityEngine.RectTransform>();
                    CompGuide_Anchors.InsertArrayElementAtIndex(CompGuide_Anchors.arraySize);
                    CompGuide_Anchors.GetArrayElementAtIndex(i).FindPropertyRelative("Anchor").objectReferenceValue = obj;
                    CompGuide_Anchors.GetArrayElementAtIndex(i).FindPropertyRelative("Name").stringValue = obj.name;
                    string Type = anchorRoot.GetChild(i).name;
                    switch (Type)
                    {
                        case "Up":
                            CompGuide_Anchors.GetArrayElementAtIndex(i).FindPropertyRelative("Type").enumValueIndex = 0;
                            break;
                        case "Down":
                            CompGuide_Anchors.GetArrayElementAtIndex(i).FindPropertyRelative("Type").enumValueIndex = 1;
                            break;
                        case "Left":
                            CompGuide_Anchors.GetArrayElementAtIndex(i).FindPropertyRelative("Type").enumValueIndex = 2;
                            break;
                        case "Right":
                            CompGuide_Anchors.GetArrayElementAtIndex(i).FindPropertyRelative("Type").enumValueIndex = 3;
                            break;
                        case "Center":
                            CompGuide_Anchors.GetArrayElementAtIndex(i).FindPropertyRelative("Type").enumValueIndex = 4;
                            break;
                        case "Left_Up":
                            CompGuide_Anchors.GetArrayElementAtIndex(i).FindPropertyRelative("Type").enumValueIndex = 5;
                            break;
                        case "Left_Down":
                            CompGuide_Anchors.GetArrayElementAtIndex(i).FindPropertyRelative("Type").enumValueIndex = 6;
                            break;
                        case "Right_Up":
                            CompGuide_Anchors.GetArrayElementAtIndex(i).FindPropertyRelative("Type").enumValueIndex = 7;
                            break;
                        case "Right_Down":
                            CompGuide_Anchors.GetArrayElementAtIndex(i).FindPropertyRelative("Type").enumValueIndex = 8;
                            break;
                    }

                    Anchors_Layout_Screen.serializedObject.ApplyModifiedProperties();
                }
            }

            #endregion
        }
        /// <summary>
        /// 移除构图参考的结构。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>清空 <c>CompGuide_Anchors</c> 序列化数组。</description></item>
        /// <item><description>销毁 <c>CompGuide</c> 根节点并清空引用。</description></item>
        /// </list>
        /// </summary>
        private void xHud_CompGuide_Structure_Destroy()
        {
            CompGuide_Anchors.ClearArray();
            CompGuide_Anchors.serializedObject.ApplyModifiedProperties();

            UnityEngine.RectTransform obj_auxiliary = CompGuide_AnchorRoot.objectReferenceValue as UnityEngine.RectTransform;
            DestroyImmediate(obj_auxiliary.gameObject, true);
            CompGuide_AnchorRoot.objectReferenceValue = null;
            CompGuide_AnchorRoot.serializedObject.ApplyModifiedProperties();
        }
        #endregion

        #region 工具 - 打开辅助窗口
        /// <summary>
        /// 打开构图参考解释窗口。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>创建并显示 <c>Editor_XHud_Tool_GuideReference</c> 窗口。</description></item>
        /// <item><description>按传入类型设置窗口的引导显示内容。</description></item>
        /// </list>
        /// </summary>
        /// <param name="guidetype">构图模式类型。</param>
        public void xHud_OpenGuideHelpPanel(string guidetype)
        {
            Editor_XHud_Tool_GuideReference window = (Editor_XHud_Tool_GuideReference)EditorWindow.GetWindow(typeof(Editor_XHud_Tool_GuideReference), false, "XHud 构图参考说明书", true);
            XGUI.CenterEditorWindow(new Vector2Int(370, 670), window, false);
            window.GuideType = (guidetype);
            window.SetGuideType(guidetype);
            window.Show();
        }
        #endregion
    }
}