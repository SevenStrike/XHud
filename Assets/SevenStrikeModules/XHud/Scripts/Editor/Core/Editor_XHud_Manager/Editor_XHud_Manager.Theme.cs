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
        #region 绘制 - 主题色
        /// <summary>
        /// 绘制「主题色」面板内容。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>提供「重置主题色」按钮。</description></item>
        /// <item><description>绘制主题色、边框色下拉选项。</description></item>
        /// <item><description>绘制主色、编组色、分割线色三组 HSV 滑块与色相渐变条。</description></item>
        /// </list>
        /// </summary>
        private void Draw_Theme()
        {
            #region 重置主题色
            if (XGUI.layout_button(
                text: "重置主题色",
                tooltip: "",
                bg_fill: XGUIGroup.实体,
                bg_color: XGUIColor.深空灰,
                bg_color_gui: Color.white,
                button_text_color: Color.white,
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
                xHud_ResetThemesColor();
            }
            #endregion

            XGUI.layout_space(10);

            #region 主题色
            XGUI.layout_string_popup(
               title: "主题色",
               title_width: 100,
               title_size: XGUIFontSize.M,
               title_anchor: TextAnchor.MiddleLeft,
               prop: theme_solution_primary,
               options: theme_primary_presetnames,
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
                   theme_solution_primary.stringValue = value;
                   theme_solution_primary.serializedObject.ApplyModifiedProperties();

                   xHud_Themes_SliderValueSet(theme_color_primary, XHud_Dashboard.themes_set_primary_color(value));

                   Editor_XHud_Tool_SceneView_Activate_Aimer.reload();
               });
            #endregion

            #region 编组色
            XGUI.layout_string_popup(
            title: "编组色",
            title_width: 100,
            title_size: XGUIFontSize.M,
            title_anchor: TextAnchor.MiddleLeft,
            prop: theme_solution_group,
            options: theme_group_presetnames,
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
                theme_solution_group.stringValue = value;
                theme_solution_group.serializedObject.ApplyModifiedProperties();

                xHud_Themes_SliderValueSet(theme_color_group, XHud_Dashboard.themes_set_group_color(value));

                Editor_XHud_Tool_SceneView_Activate_Aimer.reload();
            });
            #endregion

            #region 分割线色
            XGUI.layout_string_popup(
            title: "分割线色",
            title_width: 100,
            title_size: XGUIFontSize.M,
            title_anchor: TextAnchor.MiddleLeft,
            prop: theme_solution_sepline,
            options: theme_sepline_presetnames,
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
                theme_solution_sepline.stringValue = value;
                theme_solution_sepline.serializedObject.ApplyModifiedProperties();

                xHud_Themes_SliderValueSet(theme_color_sepline, XHud_Dashboard.themes_set_sepline_color(value));

                Editor_XHud_Tool_SceneView_Activate_Aimer.reload();
            });
            #endregion

            XGUI.layout_space(10);

            // 绘制色带基准坐标
            Rect color_rect_primary = XGUI.GetControlRect(false, 1);
            xHud_ThemeControl(color_rect_primary, theme_color_primary, ThemeColorType.primary);

            XGUI.layout_seperator(
                thickness: 1,
                color: XHud_Dashboard.themes_get_color(ThemeColorType.sepline),
                margin: new RectOffset(15, 15, 15, 15));

            // 绘制色带基准坐标
            Rect color_rect_group = XGUI.GetControlRect(false, 1);
            xHud_ThemeControl(color_rect_group, theme_color_group, ThemeColorType.group);

            XGUI.layout_seperator(
                thickness: 1,
                color: XHud_Dashboard.themes_get_color(ThemeColorType.sepline),
                margin: new RectOffset(15, 15, 15, 15));

            // 绘制色带基准坐标
            Rect color_rect_sepline = XGUI.GetControlRect(false, 1);
            xHud_ThemeControl(color_rect_sepline, theme_color_sepline, ThemeColorType.sepline);
        }
        #endregion

        #region 工具 - 主题颜色
        /// <summary>
        /// 重置主题颜色。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>清除 EditorPrefs 中三组主题色键值。</description></item>
        /// <item><description>将主题下拉重置为「默认」，并重新加载主题色。</description></item>
        /// </list>
        /// </summary>
        private void xHud_ResetThemesColor()
        {
            xHud_Themes_SolutionReset();

            XHud_Dashboard.themes_reset_color();

            xHud_Themes_SliderValueSet(theme_color_primary, XHud_Dashboard.themes_get_color(ThemeColorType.primary));
            xHud_Themes_SliderValueSet(theme_color_group, XHud_Dashboard.themes_get_color(ThemeColorType.group));
            xHud_Themes_SliderValueSet(theme_color_sepline, XHud_Dashboard.themes_get_color(ThemeColorType.sepline));

            Editor_XHud_Tool_SceneView_Activate_Aimer.reload();
        }
        private void xHud_Themes_ColorUpdate(ThemeColorType type, Vector3 val)
        {
            switch (type)
            {
                case ThemeColorType.primary:
                    XHud_Dashboard.themes_set_color(ThemeColorType.primary, Color.HSVToRGB(val.x, val.y, val.z));
                    break;
                case ThemeColorType.group:
                    XHud_Dashboard.themes_set_color(ThemeColorType.group, Color.HSVToRGB(val.x, val.y, val.z));
                    break;
                case ThemeColorType.sepline:
                    XHud_Dashboard.themes_set_color(ThemeColorType.sepline, Color.HSVToRGB(val.x, val.y, val.z));
                    break;
            }
        }
        private void xHud_Themes_ColorUpdateAll()
        {
            xHud_Themes_ColorUpdate(ThemeColorType.primary, theme_color_primary.vector3Value);
            xHud_Themes_ColorUpdate(ThemeColorType.group, theme_color_group.vector3Value);
            xHud_Themes_ColorUpdate(ThemeColorType.sepline, theme_color_sepline.vector3Value);

            Editor_XHud_Tool_SceneView_Activate_Aimer.reload();
        }
        private void xHud_Themes_SliderValueSet(SerializedProperty prop, Color col)
        {
            SerializedProperty H = prop.FindPropertyRelative("x");
            SerializedProperty S = prop.FindPropertyRelative("y");
            SerializedProperty L = prop.FindPropertyRelative("z");

            float eH;
            float eS;
            float eL;

            Color.RGBToHSV(col, out eH, out eS, out eL);

            H.floatValue = eH;
            S.floatValue = eS;
            L.floatValue = eL;

            prop.serializedObject.ApplyModifiedProperties();
        }
        private void xHud_Themes_SolutionReset()
        {
            theme_solution_primary.stringValue = "默认";
            theme_solution_group.stringValue = "默认";
            theme_solution_sepline.stringValue = "默认";

            theme_solution_primary.serializedObject.ApplyModifiedProperties();
            theme_solution_group.serializedObject.ApplyModifiedProperties();
            theme_solution_sepline.serializedObject.ApplyModifiedProperties();
        }
        #endregion

        #region 绘制 - 控件组
        private void xHud_ThemeControl(Rect rect, SerializedProperty prop, ThemeColorType type)
        {
            SerializedProperty H = prop.FindPropertyRelative("x");
            SerializedProperty S = prop.FindPropertyRelative("y");
            SerializedProperty L = prop.FindPropertyRelative("z");

            XGUI.ChangedCheck_Start();

            #region 色调
            H.floatValue = XGUI.layout_slider(
                title: "色调",
                prop: H,
                left: 0,
                right: 1,
                title_width: 100,
                title_size: XGUIFontSize.M,
                title_anchor: TextAnchor.MiddleLeft,
                prop_margin: new RectOffset(0, 0, 25, 0));
            #endregion

            #region 饱和
            S.floatValue = XGUI.layout_slider(
                title: "饱和",
                prop: S,
                left: 0,
                right: 1,
                title_width: 100,
                title_size: XGUIFontSize.M,
                title_anchor: TextAnchor.MiddleLeft,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 明度
            L.floatValue = XGUI.layout_slider(
                title: "明度",
                prop: L,
                left: 0,
                right: 1,
                title_width: 100,
                title_size: XGUIFontSize.M,
                title_anchor: TextAnchor.MiddleLeft,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            prop.serializedObject.ApplyModifiedProperties();

            XGUI.gui_hue_gradient(
                rect: rect,
                prop: prop,
                mark_color: Color.white * L.floatValue,
                mark: themes_mark,
                mark_height_offset: -6,
                mark_size: new Vector2(8, 8));

            if (XGUI.ChangedCheck_End())
            {
                xHud_Themes_ColorUpdate(type, prop.vector3Value);
            }
        }
        #endregion
    }
}