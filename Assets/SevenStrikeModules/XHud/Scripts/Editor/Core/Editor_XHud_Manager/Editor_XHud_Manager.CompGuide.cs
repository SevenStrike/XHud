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
                prop: sp_cg_color,
                title: "辅助颜色 - 线",
                title_width: 140,
                //title_color: Color.white,
                //icon: "icon_field_status",
                //icon_color: Color.red,
                state_title: "Hex：",
                state_value: $"#{XGUI_Utilitys.Color_To_HexString(sp_cg_color.colorValue)}",
                state_value_color: sp_cg_color.colorValue);
            #endregion

            #region 构图辅助颜色 - 点
            XGUI.layout_colorfield(
                prop: sp_cg_point_color,
                title: "辅助颜色 - 点",
                title_width: 140,
                //title_color: Color.white,
                //icon: "icon_field_status",
                //icon_color: Color.red,
                state_title: "Hex：",
                state_value: $"#{XGUI_Utilitys.Color_To_HexString(sp_cg_point_color.colorValue)}",
                state_value_color: sp_cg_point_color.colorValue);
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
               prop: sp_cg_mode,
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
                    xHud_OpenGuideHelpPanel(sp_cg_mode.stringValue);
                }
            }

            if (currentWidth > 260)
                XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            #endregion

            XGUI.layout_seperator(
                thickness: 1,
                color: XHud_Dashboard.themes_get_color(ThemeColorType.sepline),
                margin: new RectOffset(15, 15, 15, 10));

            switch (sp_cg_mode.stringValue)
            {
                case "水平对称":
                    XGUI.layout_space(5);
                    string[] dir_lr = new string[3] { "自定义对称分割", "靠上黄金比例", "靠下黄金比例" };
                    string mode_lr = XGUI.layout_string_popup(
                        title: "对称方式",
                        title_width: 60,
                        title_size: XGUIFontSize.M,
                        title_anchor: TextAnchor.MiddleLeft,
                        prop: sp_cg_mirror_ud_golden_mode,
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
                            prop: sp_cg_mirror_ud_offset,
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
                        prop: sp_cg_editor_serializefield_center_point_size,
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
                        sp_cg_mirror_ud_golden_mode.stringValue = "自定义对称分割";
                        sp_cg_mirror_ud_golden_mode.serializedObject.ApplyModifiedProperties();

                        sp_cg_mirror_ud_offset.floatValue = 0;
                        sp_cg_mirror_ud_offset.serializedObject.ApplyModifiedProperties();

                        sp_cg_editor_serializefield_center_point_size.floatValue = 1;
                        sp_cg_editor_serializefield_center_point_size.serializedObject.ApplyModifiedProperties();
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
                        prop: sp_cg_mirror_lr_golden_mode,
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
                            prop: sp_cg_mirror_lr_offset,
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
                        prop: sp_cg_editor_serializefield_center_point_size,
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
                        sp_cg_mirror_lr_golden_mode.stringValue = "自定义对称分割";
                        sp_cg_mirror_lr_golden_mode.serializedObject.ApplyModifiedProperties();

                        sp_cg_mirror_lr_offset.floatValue = 0;
                        sp_cg_mirror_lr_offset.serializedObject.ApplyModifiedProperties();

                        sp_cg_editor_serializefield_center_point_size.floatValue = 1;
                        sp_cg_editor_serializefield_center_point_size.serializedObject.ApplyModifiedProperties();
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
                       prop: sp_cg_fibonacci_mode,
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
                        sp_cg_fibonacci_mode.stringValue = "右上";
                        sp_cg_fibonacci_mode.serializedObject.ApplyModifiedProperties();
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
                        prop: sp_cg_editor_serializefield_corner_lookat_offset_h,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    XGUI.layout_property_field(
                        title: "垂直偏移",
                        title_size: XGUIFontSize.M,
                        //title_color: Color.white,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        //status_icon: "icon_field_status",
                        prop: sp_cg_editor_serializefield_corner_lookat_offset_v,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    XGUI.layout_property_field(
                        title: "中心点尺寸",
                        title_size: XGUIFontSize.M,
                        //title_color: Color.white,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        //status_icon: "icon_field_status",
                        prop: sp_cg_editor_serializefield_center_point_size,
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
                        sp_cg_editor_serializefield_corner_lookat_offset_h.floatValue = 0;
                        sp_cg_editor_serializefield_corner_lookat_offset_h.serializedObject.ApplyModifiedProperties();

                        sp_cg_editor_serializefield_corner_lookat_offset_v.floatValue = 0;
                        sp_cg_editor_serializefield_corner_lookat_offset_v.serializedObject.ApplyModifiedProperties();

                        sp_cg_editor_serializefield_center_point_size.floatValue = 1;
                        sp_cg_editor_serializefield_center_point_size.serializedObject.ApplyModifiedProperties();
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
                        prop: sp_cg_editor_serializefield_three_offset_h,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    XGUI.layout_property_field(
                        title: "垂直伸缩",
                        title_size: XGUIFontSize.M,
                        //title_color: Color.white,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        //status_icon: "icon_field_status",
                        prop: sp_cg_editor_serializefield_three_offset_v,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    XGUI.layout_property_field(
                        title: "整体伸缩",
                        title_size: XGUIFontSize.M,
                        //title_color: Color.white,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        //status_icon: "icon_field_status",
                        prop: sp_cg_editor_serializefield_three_offset_coverage,
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
                        sp_cg_editor_serializefield_three_offset_coverage.floatValue = 1;
                        sp_cg_editor_serializefield_three_offset_coverage.serializedObject.ApplyModifiedProperties();

                        float seg_x = sp_cv_screen_res.vector2Value.x / 3;
                        sp_cg_editor_serializefield_three_offset_h.floatValue = seg_x / sp_cv_screen_res.vector2Value.x * 2f;
                        sp_cg_editor_serializefield_three_offset_h.serializedObject.ApplyModifiedProperties();

                        float seg_y = sp_cv_screen_res.vector2Value.y / 3;
                        sp_cg_editor_serializefield_three_offset_v.floatValue = seg_y / sp_cv_screen_res.vector2Value.y * 2f;
                        sp_cg_editor_serializefield_three_offset_v.serializedObject.ApplyModifiedProperties();
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
                        sp_cg_editor_serializefield_three_offset_h.floatValue = 1;
                        sp_cg_editor_serializefield_three_offset_h.serializedObject.ApplyModifiedProperties();

                        sp_cg_editor_serializefield_three_offset_v.floatValue = 1;
                        sp_cg_editor_serializefield_three_offset_v.serializedObject.ApplyModifiedProperties();

                        sp_cg_editor_serializefield_three_offset_coverage.floatValue = 0.5f;
                        sp_cg_editor_serializefield_three_offset_coverage.serializedObject.ApplyModifiedProperties();

                        sp_cg_editor_serializefield_center_point_size.floatValue = 1;
                        sp_cg_editor_serializefield_center_point_size.serializedObject.ApplyModifiedProperties();
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
                        prop: sp_cg_editor_serializefield_guide_line_base_offset,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    XGUI.layout_property_field(
                        title: "基准水平高度",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: sp_cg_editor_serializefield_guide_line_base_height,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    XGUI.layout_property_field(
                        title: "近景高度",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: sp_cg_editor_serializefield_guide_line_offset_near_height,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    XGUI.layout_property_field(
                        title: "近景伸缩",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: sp_cg_editor_serializefield_guide_line_offset_near,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    XGUI.layout_property_field(
                        title: "远景伸缩",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: sp_cg_editor_serializefield_guide_line_offset_far,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    XGUI.layout_property_field(
                        title: "中心点尺寸",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: sp_cg_editor_serializefield_center_point_size,
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
                        sp_cg_editor_serializefield_guide_line_base_offset.floatValue = 0;
                        sp_cg_editor_serializefield_guide_line_base_offset.serializedObject.ApplyModifiedProperties();

                        sp_cg_editor_serializefield_guide_line_base_height.floatValue = 0.5f;
                        sp_cg_editor_serializefield_guide_line_base_height.serializedObject.ApplyModifiedProperties();

                        sp_cg_editor_serializefield_guide_line_offset_far.floatValue = 0;
                        sp_cg_editor_serializefield_guide_line_offset_far.serializedObject.ApplyModifiedProperties();

                        sp_cg_editor_serializefield_guide_line_offset_near.floatValue = 0;
                        sp_cg_editor_serializefield_guide_line_offset_near.serializedObject.ApplyModifiedProperties();

                        sp_cg_editor_serializefield_guide_line_offset_near_height.floatValue = 0;
                        sp_cg_editor_serializefield_guide_line_offset_near_height.serializedObject.ApplyModifiedProperties();

                        sp_cg_editor_serializefield_center_point_size.floatValue = 1;
                        sp_cg_editor_serializefield_center_point_size.serializedObject.ApplyModifiedProperties();
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
                        prop: sp_cg_editor_serializefield_triangle_top_offset,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    XGUI.layout_property_field(
                        title: "基准顶部高度",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: sp_cg_editor_serializefield_triangle_base_height,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    XGUI.layout_property_field(
                        title: "基准底部高度",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: sp_cg_editor_serializefield_guide_param_triangle_bottom_height,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    XGUI.layout_property_field(
                        title: "左角点偏移",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: sp_cg_editor_serializefield_triangle_offset_left,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    XGUI.layout_property_field(
                        title: "右角点偏移",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: sp_cg_editor_serializefield_triangle_offset_right,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    XGUI.layout_property_field(
                        title: "中心点尺寸",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: sp_cg_editor_serializefield_center_point_size,
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
                        sp_cg_editor_serializefield_triangle_top_offset.floatValue = 0;
                        sp_cg_editor_serializefield_triangle_top_offset.serializedObject.ApplyModifiedProperties();

                        sp_cg_editor_serializefield_triangle_base_height.floatValue = 0.8f;
                        sp_cg_editor_serializefield_triangle_base_height.serializedObject.ApplyModifiedProperties();

                        sp_cg_editor_serializefield_guide_param_triangle_bottom_height.floatValue = 0.2f;
                        sp_cg_editor_serializefield_guide_param_triangle_bottom_height.serializedObject.ApplyModifiedProperties();

                        sp_cg_editor_serializefield_triangle_offset_left.floatValue = 0.5f;
                        sp_cg_editor_serializefield_triangle_offset_left.serializedObject.ApplyModifiedProperties();

                        sp_cg_editor_serializefield_triangle_offset_right.floatValue = 0.5f;
                        sp_cg_editor_serializefield_triangle_offset_right.serializedObject.ApplyModifiedProperties();

                        sp_cg_editor_serializefield_center_point_size.floatValue = 1;
                        sp_cg_editor_serializefield_center_point_size.serializedObject.ApplyModifiedProperties();
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
                        prop: sp_cg_editor_serializefield_ishape_top_height,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    XGUI.layout_property_field(
                        title: "工字底部高度",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: sp_cg_editor_serializefield_ishape_bottom_height,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    XGUI.layout_property_field(
                        title: "工字左偏移",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: sp_cg_editor_serializefield_ishape_offset_left,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    XGUI.layout_property_field(
                        title: "工字右偏移",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: sp_cg_editor_serializefield_ishape_offset_right,
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
                        sp_cg_editor_serializefield_ishape_top_height.floatValue = 0.8f;
                        sp_cg_editor_serializefield_ishape_top_height.serializedObject.ApplyModifiedProperties();

                        sp_cg_editor_serializefield_ishape_bottom_height.floatValue = 0.2f;
                        sp_cg_editor_serializefield_ishape_bottom_height.serializedObject.ApplyModifiedProperties();

                        sp_cg_editor_serializefield_ishape_offset_left.floatValue = 0.5f;
                        sp_cg_editor_serializefield_ishape_offset_left.serializedObject.ApplyModifiedProperties();

                        sp_cg_editor_serializefield_ishape_offset_right.floatValue = 0.5f;
                        sp_cg_editor_serializefield_ishape_offset_right.serializedObject.ApplyModifiedProperties();

                        sp_cg_editor_serializefield_center_point_size.floatValue = 1;
                        sp_cg_editor_serializefield_center_point_size.serializedObject.ApplyModifiedProperties();
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
            sp_op_hud_canvas_group_screen.objectReferenceValue = cavgroup;
            sp_op_hud_canvas_group_screen.serializedObject.ApplyModifiedProperties();
            cavgroup.alpha = 1;
            Anchors.name = "CompGuide";
            Anchors.SetParent(base_script.cv_screen.transform);
            Anchors.localPosition = Vector3.zero;
            Anchors.localScale = Vector3.one;
            Anchors.anchorMin = new Vector2(0, 0);
            Anchors.anchorMax = new Vector2(1, 1);
            Anchors.sizeDelta = new Vector2(0, 0);

            sp_cg_anchor_root.objectReferenceValue = Anchors;
            sp_cg_anchor_root.serializedObject.ApplyModifiedProperties();

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
            if (sp_cg_anchors != null && sp_cg_anchor_root != null)
            {
                Transform anchorRoot = sp_cg_anchor_root.objectReferenceValue as Transform;
                for (int i = 0; i < anchorRoot.childCount; i++)
                {
                    UnityEngine.RectTransform obj = anchorRoot.GetChild(i).GetComponent<UnityEngine.RectTransform>();
                    sp_cg_anchors.InsertArrayElementAtIndex(sp_cg_anchors.arraySize);
                    sp_cg_anchors.GetArrayElementAtIndex(i).FindPropertyRelative(nameof(AuxiliaryAnchor_Layout.Anchor)).objectReferenceValue = obj;
                    sp_cg_anchors.GetArrayElementAtIndex(i).FindPropertyRelative(nameof(AuxiliaryAnchor_Layout.Name)).stringValue = obj.name;
                    string Type = anchorRoot.GetChild(i).name;
                    switch (Type)
                    {
                        case "Up":
                            sp_cg_anchors.GetArrayElementAtIndex(i).FindPropertyRelative(nameof(AuxiliaryAnchor_Layout.Type)).enumValueIndex = 0;
                            break;
                        case "Down":
                            sp_cg_anchors.GetArrayElementAtIndex(i).FindPropertyRelative(nameof(AuxiliaryAnchor_Layout.Type)).enumValueIndex = 1;
                            break;
                        case "Left":
                            sp_cg_anchors.GetArrayElementAtIndex(i).FindPropertyRelative(nameof(AuxiliaryAnchor_Layout.Type)).enumValueIndex = 2;
                            break;
                        case "Right":
                            sp_cg_anchors.GetArrayElementAtIndex(i).FindPropertyRelative(nameof(AuxiliaryAnchor_Layout.Type)).enumValueIndex = 3;
                            break;
                        case "Center":
                            sp_cg_anchors.GetArrayElementAtIndex(i).FindPropertyRelative(nameof(AuxiliaryAnchor_Layout.Type)).enumValueIndex = 4;
                            break;
                        case "Left_Up":
                            sp_cg_anchors.GetArrayElementAtIndex(i).FindPropertyRelative(nameof(AuxiliaryAnchor_Layout.Type)).enumValueIndex = 5;
                            break;
                        case "Left_Down":
                            sp_cg_anchors.GetArrayElementAtIndex(i).FindPropertyRelative(nameof(AuxiliaryAnchor_Layout.Type)).enumValueIndex = 6;
                            break;
                        case "Right_Up":
                            sp_cg_anchors.GetArrayElementAtIndex(i).FindPropertyRelative(nameof(AuxiliaryAnchor_Layout.Type)).enumValueIndex = 7;
                            break;
                        case "Right_Down":
                            sp_cg_anchors.GetArrayElementAtIndex(i).FindPropertyRelative(nameof(AuxiliaryAnchor_Layout.Type)).enumValueIndex = 8;
                            break;
                    }

                    sp_cv_anchors_layout_screen.serializedObject.ApplyModifiedProperties();
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
            sp_cg_anchors.ClearArray();
            sp_cg_anchors.serializedObject.ApplyModifiedProperties();

            UnityEngine.RectTransform obj_auxiliary = sp_cg_anchor_root.objectReferenceValue as UnityEngine.RectTransform;
            DestroyImmediate(obj_auxiliary.gameObject, true);
            sp_cg_anchor_root.objectReferenceValue = null;
            sp_cg_anchor_root.serializedObject.ApplyModifiedProperties();
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
            window.guide_type = (guidetype);
            window.SetGuideType(guidetype);
            window.Show();
        }
        #endregion
    }
}