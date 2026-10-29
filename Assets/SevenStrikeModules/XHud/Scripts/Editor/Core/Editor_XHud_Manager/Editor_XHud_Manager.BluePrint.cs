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
    using TMPro;
    using UnityEditor;
    using UnityEngine;

    public partial class Editor_XHud_Manager : Editor
    {
        #region 绘制 - 蓝图
        /// <summary>
        /// 绘制「蓝图」面板内容。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>绘制蓝图激活状态、整体/背景透明度与背景动画参数。</description></item>
        /// <item><description>绘制叠加图像的颜色、图例、平铺模式与方形比例。</description></item>
        /// <item><description>绘制网格颜色、透明度、尺寸、线宽、动画等参数。</description></item>
        /// <item><description>绘制水印的标题、颜色、尺寸、间距、锚点与边距。</description></item>
        /// <item><description>绘制总动画速度、缓动与入/退场影响开关。</description></item>
        /// </list>
        /// </summary>
        private void Draw_BluePrint()
        {
            #region 蓝图激活状态
            XGUI.layout_state_displayer_text(
                title: "蓝图激活状态",
                title_size: XGUIFontSize.M,
                subtitle: sp_bp_displayed.boolValue ? "激活" : "未激活",
                subtitle_size: XGUIFontSize.M,
                subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                padding: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 整体透明度
            XGUI.layout_slider(
                title: "整体透明度",
                prop: sp_bp_opacity,
                left: 0,
                right: 1,
                title_width: 100,
                title_size: XGUIFontSize.M,
                title_anchor: TextAnchor.MiddleLeft,
                //title_color: Color.white,
                //status_icon: "icon_field_status",
                //status_icon_color: Color.green,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 背景透明度
            XGUI.layout_slider(
              title: "背景透明度",
              prop: sp_bp_bg_opacity,
              left: 0,
              right: 1,
              title_width: 100,
              title_size: XGUIFontSize.M,
              title_anchor: TextAnchor.MiddleLeft,
              //title_color: Color.white,
              //status_icon: "icon_field_status",
              //status_icon_color: Color.green,
              prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 背景动画缓动 
            XGUI.layout_property_field(
                title: "背景动画缓动 - 入场",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 120,
                prop: sp_bp_bg_animation_ease_in,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 背景动画缓动 
            XGUI.layout_property_field(
                title: "背景动画缓动 - 出场",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 120,
                prop: sp_bp_bg_animation_ease_out,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 背景淡化退场延迟
            XGUI.layout_property_field(
                title: "背景淡化退场延迟",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 120,
                prop: sp_bp_bg_fade_animation_delay,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 背景色
            XGUI.layout_colorfield(
                prop: sp_bp_bg_color,
                title: "背景色",
                title_width: 140,
                //title_color: Color.white,
                //icon: "icon_field_status",
                //icon_color: Color.red,
                state_title: "Hex：",
                state_value: $"#{XGUI_Utilitys.Color_To_HexString(sp_bp_bg_color.colorValue)}",
                state_value_color: sp_bp_bg_color.colorValue);
            #endregion

            XGUI.layout_seperator(
               thickness: 1,
               color: XHud_Dashboard.themes_get_color(ThemeColorType.sepline),
               margin: new RectOffset(15, 15, 5, 15));

            #region 叠加图像透明度
            XGUI.layout_property_field(
                title: "叠加图像透明度",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 120,
                prop: sp_bp_bg_map_opacity,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            XGUI.layout_colorfield(
            #region 叠加图像颜色
                prop: sp_bp_bg_decal_color,
                title: "叠加图像颜色",
                title_width: 140,
                //title_color: Color.white,
                //icon: "icon_field_status",
                //icon_color: Color.red,
                state_title: "Hex：",
                state_value: $"#{XGUI_Utilitys.Color_To_HexString(sp_bp_bg_decal_color.colorValue)}",
                state_value_color: sp_bp_bg_decal_color.colorValue);
            #endregion

            #region 叠加图像
            XGUI.layout_string_popup(
                title: "叠加图像",
                title_width: 110,
                title_size: XGUIFontSize.M,
                title_anchor: TextAnchor.MiddleLeft,
                prop: sp_bp_bg_name,
                options: blue_print_bgs_name,
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
                    sp_bp_bg_name.stringValue = value;
                    sp_bp_bg_name.serializedObject.ApplyModifiedProperties();

                    #region 指定叠加背景                              
                    Texture2D texselected = AssetDatabase.LoadAssetAtPath<Texture2D>($"{XHud_Dashboard.Get_Path_XHUD_TEXTURES_Path()}BluePrints/{value}.png");
                    if (texselected != null)
                    {
                        base_script.hm_BluePrint_SetBgTexture(texselected);
                    }
                    #endregion
                });
            #endregion

            #region 叠加图像 - 平铺模式 
            XGUI.layout_int_popup(
                title: "叠加图像 - 平铺模式",
                title_width: 110,
                title_size: XGUIFontSize.M,
                title_anchor: TextAnchor.MiddleLeft,
                prop: sp_bp_bg_usetilling_index,
                options: stroptions_enabled,
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
                act_on_changed: (value) => { });
            #endregion

            #region 叠加图像 - 方形比例 
            XGUI.layout_int_popup(
                title: "叠加图像 - 方形比例",
                title_width: 110,
                title_size: XGUIFontSize.M,
                title_anchor: TextAnchor.MiddleLeft,
                prop: sp_bp_bg_usesquareratio_index,
                options: stroptions_enabled,
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
                act_on_changed: (value) => { });
            #endregion


            #region 叠加图像平铺
            if (sp_bp_bg_usetilling_index.intValue == 1)
            {
                XGUI.layout_property_field(
                    title: "叠加图像平铺",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 120,
                    prop: sp_bp_bg_tilling,
                    prop_margin: new RectOffset(0, 0, 5, 0));
            }
            #endregion

            XGUI.layout_seperator(
              thickness: 1,
              color: XHud_Dashboard.themes_get_color(ThemeColorType.sepline),
              margin: new RectOffset(15, 15, 15, 15));

            #region 网格色
            XGUI.layout_colorfield(
                prop: sp_bp_grid_color,
                title: "网格色",
                title_width: 120,
                //title_color: Color.white,
                //icon: "icon_field_status",
                //icon_color: Color.red,
                state_title: "Hex：",
                state_value: $"#{XGUI_Utilitys.Color_To_HexString(sp_bp_grid_color.colorValue)}",
                state_value_color: sp_bp_grid_color.colorValue);
            #endregion

            #region 网格透明度
            XGUI.layout_property_field(
                title: "网格透明度",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 120,
                prop: sp_bp_grid_opacity,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 网格尺寸
            XGUI.ChangedCheck_Start();
            XGUI.layout_property_field(
                title: "网格尺寸",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 120,
                prop: sp_bp_grid_size,
                prop_margin: new RectOffset(0, 0, 5, 0));
            if (XGUI.ChangedCheck_End())
            {
                if (!Application.isPlaying)
                {
                    base_script.hm_BluePrint_Remove();

                    Material mat = AssetDatabase.LoadAssetAtPath<Material>($"{XHud_Dashboard.Get_Path_XHUD_MATERIALS_Path()}BluePrints/Mat/BluePrint.mat");
                    TMP_FontAsset title = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>($"{XHud_Dashboard.Get_Path_XHUD_FONTS_Path()}Fonts/Tmp/sx_bold SDF.asset");
                    TMP_FontAsset subtitle = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>($"{XHud_Dashboard.Get_Path_XHUD_FONTS_Path()}Fonts/Tmp/sx_light SDF.asset");
                    base_script.hm_BluePrint_Create(true, mat, title, subtitle);
                }
            }
            #endregion

            #region 网格线宽度
            XGUI.layout_property_field(
                title: "网格线宽度",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 120,
                prop: sp_bp_linewidth,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 网格线百分比
            XGUI.layout_property_field(
                title: "网格线百分比",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 120,
                prop: sp_bp_grid_length_percentage,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 网格分级高差
            XGUI.layout_property_field(
                title: "网格分级高差",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 120,
                prop: sp_bp_grid_level_height,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 网格动画结束
            XGUI.layout_property_field(
                title: "网格动画结束",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 120,
                prop: sp_bp_grid_end,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 网格动画速率
            XGUI.layout_property_field(
                title: "网格动画速率",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 120,
                prop: sp_bp_grid_animation_duration,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            XGUI.layout_seperator(
                thickness: 1,
                color: XHud_Dashboard.themes_get_color(ThemeColorType.sepline),
                margin: new RectOffset(15, 15, 15, 15));

            #region 大标题色
            XGUI.layout_colorfield(
                prop: sp_bp_marktitle_color,
                title: "大标题色",
                title_width: 140,
                //title_color: Color.white,
                //icon: "icon_field_status",
                //icon_color: Color.red,
                state_title: "Hex：",
                state_value: $"#{XGUI_Utilitys.Color_To_HexString(sp_bp_marktitle_color.colorValue)}",
                state_value_color: sp_bp_marktitle_color.colorValue);
            #endregion

            #region 小标题色
            XGUI.layout_colorfield(
                prop: sp_bp_marksubtitle_color,
                title: "小标题色",
                title_width: 140,
                //title_color: Color.white,
                //icon: "icon_field_status",
                //icon_color: Color.red,
                state_title: "Hex：",
                state_value: $"#{XGUI_Utilitys.Color_To_HexString(sp_bp_marksubtitle_color.colorValue)}",
                state_value_color: sp_bp_marksubtitle_color.colorValue);
            #endregion

            #region 水印大标题
            XGUI.layout_property_field(
                title: "水印大标题",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 120,
                prop: sp_bp_title_content,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 水印小标题
            XGUI.layout_property_field(
                title: "水印小标题",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 120,
                prop: sp_bp_subtitle_content,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 水印透明度
            XGUI.layout_property_field(
                title: "水印透明度",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 120,
                prop: sp_bp_mark_opacity,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 水印尺寸
            XGUI.layout_property_field(
                title: "水印尺寸",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 120,
                prop: sp_bp_mark_size,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 水印间距
            XGUI.layout_property_field(
                title: "水印间距",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 120,
                prop: sp_bp_mark_space,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 水印锚点
            XGUI.layout_property_field(
                title: "水印锚点",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 120,
                prop: sp_bp_mark_anchors,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 水印边距
            SerializedProperty sp_mar_x = sp_bp_mark_margin.FindPropertyRelative(nameof(Vector4.x));
            SerializedProperty sp_mar_y = sp_bp_mark_margin.FindPropertyRelative(nameof(Vector4.y));
            SerializedProperty sp_mar_z = sp_bp_mark_margin.FindPropertyRelative(nameof(Vector4.z));
            SerializedProperty sp_mar_w = sp_bp_mark_margin.FindPropertyRelative(nameof(Vector4.w));

            XGUI.layout_property_field(
                title: "水印 - 上边距",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 110,
                prop: sp_mar_x,
                prop_margin: new RectOffset(0, 0, 5, 0));

            XGUI.layout_property_field(
                title: "水印 - 下边距",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 110,
                prop: sp_mar_y,
                prop_margin: new RectOffset(0, 0, 5, 0));

            XGUI.layout_property_field(
                title: "水印 - 左边距",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 110,
                prop: sp_mar_z,
                prop_margin: new RectOffset(0, 0, 5, 0));

            XGUI.layout_property_field(
                title: "水印 - 右边距",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 110,
                prop: sp_mar_w,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            XGUI.layout_seperator(
                thickness: 1,
                color: XHud_Dashboard.themes_get_color(ThemeColorType.sepline),
                margin: new RectOffset(15, 15, 15, 15));

            #region 总动画速度
            XGUI.layout_property_field(
                title: "总动画速度",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 120,
                prop: sp_bp_animation_duration,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 网格动画缓动
            XGUI.layout_property_field(
                title: "网格动画缓动",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 120,
                prop: sp_bp_grid_animation_ease,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            XGUI.layout_seperator(
                thickness: 1,
                color: XHud_Dashboard.themes_get_color(ThemeColorType.sepline),
                margin: new RectOffset(15, 15, 15, 15));

            #region 开始时退场状态
            DrawToggle("开始时退场状态", sp_bp_on_start_hide, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, (b) => { });
            #endregion

            #region 入/退场影响网格
            DrawToggle("入/退场影响网格", sp_bp_eft_grid, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, (b) => { });
            #endregion

            #region 入/退场影响网格透明度
            DrawToggle("入/退场影响网格透明度", sp_bp_eft_grid_fade, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, (b) => { });
            #endregion

            #region 入/退场影响背景
            DrawToggle("入/退场影响背景", sp_bp_eft_bg, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, (b) => { });
            #endregion

            #region 入/退场影响水印
            DrawToggle("入/退场影响水印", sp_bp_eft_mark, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, (b) => { });
            #endregion
        }
        #endregion
    }
}