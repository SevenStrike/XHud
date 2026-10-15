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
              title_width: 60,
              title_size: XGUIFontSize.M,
              title_anchor: TextAnchor.MiddleLeft,
              prop: ThemeSolution,
              options: ThemeSolutionNames,
              opt_text_size: XGUIFontSize.M,
              opt_text_color: Color.black,
              opt_text_padding: new RectOffset(10, 10, 0, 0),
              opt_anchor: TextAnchor.MiddleCenter,
              opt_font_style: FontStyle.Normal,
              opt_bg_fill: XGUIGroup.实体,
              opt_bg_color: XGUIColor.亮白,
              opt_bg_color_gui: XHud_Dashboard.Theme_Primary,
              icon_arrow_color: Color.black,
              margin: new RectOffset(0, 0, 5, 5),
              padding: new RectOffset(5, 5, 0, 0),
              title_margin: new RectOffset(0, 0, 0, 0),
              act_on_changed: (value) =>
              {
                  ThemeSolution.stringValue = value;
                  ThemeSolution.serializedObject.ApplyModifiedProperties();

                  Color col = Color.white;
                  switch (value)
                  {
                      case "白色":
                          col = new Color(0.8196079f, 0.8196079f, 0.8196079f);
                          break;
                      case "黑色":
                          col = new Color(0, 0, 0);
                          break;
                      case "乳白":
                          col = new Color(0.8235295f, 0.7882354f, 0.7411765f);
                          break;
                      case "浅灰":
                          col = new Color(0.5254902f, 0.5254902f, 0.5254902f);
                          break;
                      case "沙漠灰":
                          col = new Color(0.9176471f, 0.8000001f, 0.5843138f);
                          break;
                      case "科幻青":
                          col = new Color(0.2313726f, 0.9921569f, 0.6039216f);
                          break;
                      case "液晶绿":
                          col = new Color(0.7254902f, 0.7882354f, 0.3568628f);
                          break;
                      case "橄榄绿":
                          col = new Color(0.7176471f, 0.8235295f, 0.2039216f);
                          break;
                      case "湖蓝":
                          col = new Color(0.2470588f, 0.6352941f, 0.9960785f);
                          break;
                      case "天蓝":
                          col = new Color(0.4f, 0.6745098f, 0.854902f);
                          break;
                      case "胭脂粉":
                          col = new Color(0.8588236f, 0.4901961f, 0.7294118f);
                          break;
                      case "灵动粉":
                          col = new Color(0.882353f, 0.4705883f, 0.5215687f);
                          break;
                      case "秋叶黄":
                          col = new Color(0.8352942f, 0.6117647f, 0.3411765f);
                          break;
                      case "警示黄":
                          col = new Color(0.9960785f, 0.7686275f, 0);
                          break;
                      case "高亮橘":
                          col = new Color(0.9960785f, 0.5176471f, 0.1686275f);
                          break;
                      case "烈焰红":
                          col = new Color(0.9960785f, 0.3529412f, 0.2156863f);
                          break;
                  }

                  #region 更新主题色
                  float h = 0; float s = 0; float v = 0;
                  Color.RGBToHSV(col, out h, out s, out v);
                  theme_color.vector3Value = new Vector3(h, s, v);
                  xHud_UpdateThemeColor(theme_color.vector3Value);

                  if (XGUI.x_Editor_Data_Has_String(PrefsKeyColor_Theme))
                  {
                      Color x = Color.HSVToRGB(h, s, v);
                      XGUI.x_Editor_Data_Set_With_String(PrefsKeyColor_Theme, $"{x.r},{x.g},{x.b}");
                  }
                  #endregion
              });
            #endregion

            #region 边框色
            XGUI.layout_string_popup(
            title: "边框色",
            title_width: 60,
            title_size: XGUIFontSize.M,
            title_anchor: TextAnchor.MiddleLeft,
            prop: ThemeEdgeSolution,
            options: ThemeEdgeSolutionNames,
            opt_text_size: XGUIFontSize.M,
            opt_text_color: Color.black,
            opt_text_padding: new RectOffset(10, 10, 0, 0),
            opt_anchor: TextAnchor.MiddleCenter,
            opt_font_style: FontStyle.Normal,
            opt_bg_fill: XGUIGroup.实体,
            opt_bg_color: XGUIColor.亮白,
            opt_bg_color_gui: XHud_Dashboard.Theme_Primary,
            icon_arrow_color: Color.black,
            margin: new RectOffset(0, 0, 5, 5),
            padding: new RectOffset(5, 5, 0, 0),
            title_margin: new RectOffset(0, 0, 0, 0),
            act_on_changed: (value) =>
            {
                ThemeEdgeSolution.stringValue = value;
                ThemeEdgeSolution.serializedObject.ApplyModifiedProperties();

                Color col = Color.white;
                switch (value)
                {
                    case "白色":
                        col = new Color(0.5568628f, 0.5568628f, 0.5568628f);
                        break;
                    case "浅灰":
                        col = new Color(0.4f, 0.4f, 0.4f);
                        break;
                    case "深灰":
                        col = new Color(0.1176471f, 0.1176471f, 0.1176471f);
                        break;
                    case "黑色":
                        col = new Color(0, 0, 0);
                        break;
                    case "极简":
                        col = new Color(0.2196079f, 0.2196079f, 0.2196079f);
                        break;
                    case "珊瑚红":
                        col = new Color(0.5529412f, 0.3058824f, 0.3058824f);
                        break;
                    case "落叶黄":
                        col = new Color(0.5529412f, 0.4470589f, 0.3058824f);
                        break;
                    case "烟灰蓝":
                        col = new Color(0.2980392f, 0.3568628f, 0.427451f);
                        break;
                    case "青苔绿":
                        col = new Color(0.3529412f, 0.4039216f, 0.2470588f);
                        break;
                    case "荧光绿":
                        col = new Color(0.3058824f, 0.427451f, 0.2235294f);
                        break;
                    case "淡粉":
                        col = new Color(0.427451f, 0.3058824f, 0.4078432f);
                        break;
                }

                #region 更新主题边框色
                float h = 0; float s = 0; float v = 0;
                Color.RGBToHSV(col, out h, out s, out v);
                theme_color_gp.vector3Value = new Vector3(h, s, v);
                xHud_UpdateThemeColor_Gp(theme_color_gp.vector3Value);

                if (XGUI.x_Editor_Data_Has_String(PrefsKeyColor_Theme_GP))
                {
                    Color x = Color.HSVToRGB(h, s, v);
                    XGUI.x_Editor_Data_Set_With_String(PrefsKeyColor_Theme_GP, $"{x.r},{x.g},{x.b}");
                }
                #endregion
            });
            #endregion

            XGUI.layout_space(10);

            #region Theme_Color
            // 绘制滑动条基准坐标
            Rect color_rect = XGUI.GetControlRect(false, 1);

            //XGUI.gui_box(color_rect, Color.red * 0.2f);

            XGUI.layout_space(18);

            SerializedProperty prop_theme_hue = theme_color.FindPropertyRelative("x");
            SerializedProperty prop_theme_sat = theme_color.FindPropertyRelative("y");
            SerializedProperty prop_theme_lgt = theme_color.FindPropertyRelative("z");

            XGUI.ChangedCheck_Start();

            #region 色调
            prop_theme_hue.floatValue = XGUI.layout_slider(
                title: "色调",
                prop: prop_theme_hue,
                left: 0,
                right: 1,
                title_width: 100,
                title_size: XGUIFontSize.M,
                title_anchor: TextAnchor.MiddleLeft,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 饱和
            prop_theme_sat.floatValue = XGUI.layout_slider(
                title: "饱和",
                prop: prop_theme_sat,
                left: 0,
                right: 1,
                title_width: 100,
                title_size: XGUIFontSize.M,
                title_anchor: TextAnchor.MiddleLeft,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 明度
            prop_theme_lgt.floatValue = XGUI.layout_slider(
                title: "明度",
                prop: prop_theme_lgt,
                left: 0,
                right: 1,
                title_width: 100,
                title_size: XGUIFontSize.M,
                title_anchor: TextAnchor.MiddleLeft,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            theme_color.serializedObject.ApplyModifiedProperties();

            XGUI.gui_hue_gradient(
                rect: color_rect,
                prop: theme_color,
                mark_color: Color.white,
                mark: themes_mark,
                mark_height_offset: -6,
                mark_size: new Vector2(8, 8));

            if (XGUI.ChangedCheck_End())
            {
                xHud_UpdateThemeColor(theme_color.vector3Value);

                if (XGUI.x_Editor_Data_Has_String(PrefsKeyColor_Theme))
                {
                    Color x = Color.HSVToRGB(prop_theme_hue.floatValue, prop_theme_sat.floatValue, prop_theme_lgt.floatValue);
                    XGUI.x_Editor_Data_Set_With_String(PrefsKeyColor_Theme, $"{x.r},{x.g},{x.b}");
                }
            }
            #endregion

            XGUI.layout_seperator(
                thickness: 1,
                color: XHud_Dashboard.Theme_SeperateLine,
                margin: new RectOffset(15, 15, 15, 15));

            #region Theme_Color_Gp
            // 绘制滑动条基准坐标
            Rect color_rect_gp = XGUI.GetControlRect(false, 1);

            //XGUI.gui_box(color_rect, Color.red * 0.2f);

            XGUI.layout_space(18);

            SerializedProperty theme_color_gp_hue = theme_color_gp.FindPropertyRelative("x");
            SerializedProperty theme_color_gp_sat = theme_color_gp.FindPropertyRelative("y");
            SerializedProperty theme_color_gp_lgt = theme_color_gp.FindPropertyRelative("z");

            XGUI.ChangedCheck_Start();

            #region 色调
            theme_color_gp_hue.floatValue = XGUI.layout_slider(
                title: "色调",
                prop: theme_color_gp_hue,
                left: 0,
                right: 1,
                title_width: 100,
                title_size: XGUIFontSize.M,
                title_anchor: TextAnchor.MiddleLeft,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 饱和
            theme_color_gp_sat.floatValue = XGUI.layout_slider(
                title: "饱和",
                prop: theme_color_gp_sat,
                left: 0,
                right: 1,
                title_width: 100,
                title_size: XGUIFontSize.M,
                title_anchor: TextAnchor.MiddleLeft,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 明度
            theme_color_gp_lgt.floatValue = XGUI.layout_slider(
                title: "明度",
                prop: theme_color_gp_lgt,
                left: 0,
                right: 1,
                title_width: 100,
                title_size: XGUIFontSize.M,
                title_anchor: TextAnchor.MiddleLeft,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            theme_color_gp.serializedObject.ApplyModifiedProperties();

            XGUI.gui_hue_gradient(
                rect: color_rect_gp,
                prop: theme_color_gp,
                mark_color: Color.white,
                mark: themes_mark,
                mark_height_offset: -6,
                mark_size: new Vector2(8, 8));

            if (XGUI.ChangedCheck_End())
            {
                xHud_UpdateThemeColor_Gp(theme_color_gp.vector3Value);

                if (XGUI.x_Editor_Data_Has_String(PrefsKeyColor_Theme_GP))
                {
                    Color x = Color.HSVToRGB(theme_color_gp_hue.floatValue, theme_color_gp_sat.floatValue, theme_color_gp_lgt.floatValue);
                    XGUI.x_Editor_Data_Set_With_String(PrefsKeyColor_Theme_GP, $"{x.r},{x.g},{x.b}");
                }
            }
            #endregion

            XGUI.layout_seperator(
                thickness: 1,
                color: XHud_Dashboard.Theme_SeperateLine,
                margin: new RectOffset(15, 15, 15, 15));

            #region Theme_Color_Sep
            // 绘制滑动条基准坐标
            Rect color_rect_sep = XGUI.GetControlRect(false, 1);

            //XGUI.gui_box(color_rect, Color.red * 0.2f);

            XGUI.layout_space(18);

            SerializedProperty theme_color_sep_hue = theme_color_sep.FindPropertyRelative("x");
            SerializedProperty theme_color_sep_sat = theme_color_sep.FindPropertyRelative("y");
            SerializedProperty theme_color_sep_lgt = theme_color_sep.FindPropertyRelative("z");

            XGUI.ChangedCheck_Start();

            #region 色调
            theme_color_sep_hue.floatValue = XGUI.layout_slider(
                title: "色调",
                prop: theme_color_sep_hue,
                left: 0,
                right: 1,
                title_width: 100,
                title_size: XGUIFontSize.M,
                title_anchor: TextAnchor.MiddleLeft,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 饱和
            theme_color_sep_sat.floatValue = XGUI.layout_slider(
                title: "饱和",
                prop: theme_color_sep_sat,
                left: 0,
                right: 1,
                title_width: 100,
                title_size: XGUIFontSize.M,
                title_anchor: TextAnchor.MiddleLeft,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 明度
            theme_color_sep_lgt.floatValue = XGUI.layout_slider(
                title: "明度",
                prop: theme_color_sep_lgt,
                left: 0,
                right: 1,
                title_width: 100,
                title_size: XGUIFontSize.M,
                title_anchor: TextAnchor.MiddleLeft,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            theme_color_sep.serializedObject.ApplyModifiedProperties();

            XGUI.gui_hue_gradient(
                rect: color_rect_sep,
                prop: theme_color_sep,
                mark_color: Color.white,
                mark: themes_mark,
                mark_height_offset: -6,
                mark_size: new Vector2(8, 8));

            if (XGUI.ChangedCheck_End())
            {
                xHud_UpdateThemeColor_Sep(theme_color_sep.vector3Value);

                if (XGUI.x_Editor_Data_Has_String(PrefsKeyColor_Theme_SEP))
                {
                    Color x = Color.HSVToRGB(theme_color_sep_hue.floatValue, theme_color_sep_sat.floatValue, theme_color_sep_lgt.floatValue);
                    XGUI.x_Editor_Data_Set_With_String(PrefsKeyColor_Theme_SEP, $"{x.r},{x.g},{x.b}");
                }
            }
            #endregion
        }
        #endregion

        #region 工具 - 主题颜色
        /// <summary>
        /// 读取主题颜色。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>从 EditorPrefs 读取主色、编组色、分割线色并写入对应序列化属性。</description></item>
        /// <item><description>同步刷新 <c>XHud_Dashboard</c> 中的主题色。</description></item>
        /// </list>
        /// </summary>
        private void xHud_LoadThemesColor()
        {
            string colorval = XGUI.x_Editor_Data_Get_With_String(PrefsKeyColor_Theme);
            Vector3 ori_col = XGUI_Utilitys.String_To_Vector3(colorval);
            Color col = XGUI_Utilitys.RGBA_To_Color(ori_col.x, ori_col.y, ori_col.z, 1, false);
            Color ccc = Color.white;
            Color.RGBToHSV(col, out ccc.r, out ccc.g, out ccc.b);
            theme_color.vector3Value = new Vector3(ccc.r, ccc.g, ccc.b);
            theme_color.serializedObject.ApplyModifiedProperties();
            xHud_UpdateThemeColor(theme_color.vector3Value);

            string colorval_gp = XGUI.x_Editor_Data_Get_With_String(PrefsKeyColor_Theme_GP);
            Vector3 ori_col_gp = XGUI_Utilitys.String_To_Vector3(colorval_gp);
            Color col_gp = XGUI_Utilitys.RGBA_To_Color(ori_col_gp.x, ori_col_gp.y, ori_col_gp.z, 1, false);
            Color ccc_gp = Color.white;
            Color.RGBToHSV(col_gp, out ccc_gp.r, out ccc_gp.g, out ccc_gp.b);
            theme_color_gp.vector3Value = new Vector3(ccc_gp.r, ccc_gp.g, ccc_gp.b);
            theme_color_gp.serializedObject.ApplyModifiedProperties();
            xHud_UpdateThemeColor_Gp(theme_color_gp.vector3Value);


            string colorval_sep = XGUI.x_Editor_Data_Get_With_String(PrefsKeyColor_Theme_SEP);
            Vector3 ori_col_sep = XGUI_Utilitys.String_To_Vector3(colorval_sep);
            Color col_sep = XGUI_Utilitys.RGBA_To_Color(ori_col_sep.x, ori_col_sep.y, ori_col_sep.z, 1, false);
            Color ccc_sep = Color.white;
            Color.RGBToHSV(col_sep, out ccc_sep.r, out ccc_sep.g, out ccc_sep.b);
            theme_color_sep.vector3Value = new Vector3(ccc_sep.r, ccc_sep.g, ccc_sep.b);
            theme_color_sep.serializedObject.ApplyModifiedProperties();
            xHud_UpdateThemeColor_Sep(theme_color_sep.vector3Value);
        }
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
            EditorPrefs.DeleteKey(PrefsKeyColor_Theme);
            EditorPrefs.DeleteKey(PrefsKeyColor_Theme_GP);
            EditorPrefs.DeleteKey(PrefsKeyColor_Theme_SEP);

            ThemeSolution.stringValue = "默认";
            ThemeEdgeSolution.stringValue = "默认";

            ThemeSolution.serializedObject.ApplyModifiedProperties();
            ThemeEdgeSolution.serializedObject.ApplyModifiedProperties();

            XHud_Dashboard.LoadThemes();

            xHud_LoadThemesColor();
        }
        /// <summary>
        /// 更新主题颜色 - 主颜色。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>将 HSV 分量转换为 <c>Color</c> 并写入 <c>XHud_Dashboard.Theme_Primary</c>。</description></item>
        /// </list>
        /// </summary>
        /// <param name="col">HSV 三分量。</param>
        private void xHud_UpdateThemeColor(Vector3 col)
        {
            XHud_Dashboard.Theme_Primary = Color.HSVToRGB(col.x, col.y, col.z);
        }
        /// <summary>
        /// 更新主题颜色 - 编组色。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>将 HSV 分量转换为 <c>Color</c> 并写入 <c>XHud_Dashboard.Theme_Group</c>。</description></item>
        /// </list>
        /// </summary>
        /// <param name="col">HSV 三分量。</param>
        private void xHud_UpdateThemeColor_Gp(Vector3 col)
        {
            XHud_Dashboard.Theme_Group = Color.HSVToRGB(col.x, col.y, col.z);
        }
        /// <summary>
        /// 更新主题颜色 - 分割线。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>将 HSV 分量转换为 <c>Color</c> 并写入 <c>XHud_Dashboard.Theme_SeperateLine</c>。</description></item>
        /// </list>
        /// </summary>
        /// <param name="col">HSV 三分量。</param>
        private void xHud_UpdateThemeColor_Sep(Vector3 col)
        {
            XHud_Dashboard.Theme_SeperateLine = Color.HSVToRGB(col.x, col.y, col.z);
        }
        #endregion
    }
}