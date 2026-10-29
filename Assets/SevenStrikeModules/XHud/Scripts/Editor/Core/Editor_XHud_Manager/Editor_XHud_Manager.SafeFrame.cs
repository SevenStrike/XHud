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
    using UnityEngine.UI;

    public partial class Editor_XHud_Manager : Editor
    {
        #region 绘制 - 辅助
        /// <summary>
        /// 绘制「辅助」面板内容。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>绘制角点标记、中心标记、安全框、分割标记颜色字段。</description></item>
        /// <item><description>绘制角点标记尺寸、安全框粗细、安全中分线长度、安全框边距等尺寸字段。</description></item>
        /// <item><description>绘制中心标记间距/宽度/长度字段与重置参数按钮。</description></item>
        /// </list>
        /// </summary>
        private void Draw_SafeFrame()
        {
            XGUI.ChangedCheck_Start();

            #region 角点标记
            XGUI.layout_property_field(
                title: "角点标记",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 110,
                prop: sp_sf_color_layout_anchor_mark,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 中心标记
            XGUI.layout_property_field(
                title: "中心标记",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 110,
                prop: sp_sf_color_center_mark,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 安全框
            XGUI.layout_property_field(
                title: "安全框",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 110,
                prop: sp_sf_color_frame_line,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 分割标记
            XGUI.layout_property_field(
                title: "分割标记",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 110,
                prop: sp_sf_color_seperater_line,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            XGUI.layout_seperator(
                thickness: 1,
                color: XHud_Dashboard.themes_get_color(ThemeColorType.sepline),
                margin: new RectOffset(15, 15, 20, 15));

            #region 角点标记尺寸
            XGUI.layout_property_field(
                title: "角点标记尺寸",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 110,
                prop: sp_sf_mark_size,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 安全框粗细
            XGUI.layout_property_field(
                title: "安全框粗细",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 110,
                prop: sp_sf_safe_frame_line_width,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 安全中分线长度
            XGUI.layout_property_field(
                title: "安全中分线长度",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 110,
                prop: sp_sf_seperater_length,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 安全框边距
            XGUI.layout_property_field(
                title: "安全框边距",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 110,
                prop: sp_sf_line_margins,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 中心标记间距
            XGUI.layout_property_field(
                title: "中心标记间距",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 110,
                prop: sp_sf_center_mark_distance,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 中心标记宽度
            XGUI.layout_property_field(
                title: "中心标记宽度",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 110,
                prop: sp_sf_center_mark_width,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 中心标记长度
            XGUI.layout_property_field(
                title: "中心标记长度",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 110,
                prop: sp_sf_center_mark_length,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            XGUI.layout_space(15);

            if (XGUI.layout_button(
                text: "重置参数",
                tooltip: "",
                bg_fill: XGUIGroup.实体,
                bg_color: XGUIColor.深空灰,
                bg_color_gui: Color.white,
                button_text_color: new Color(1, 0.4f, 0.4f),
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
                sp_sf_color_layout_anchor_mark.colorValue = Color.white;
                sp_sf_color_center_mark.colorValue = XHud_Dashboard.themes_get_color(ThemeColorType.primary);
                sp_sf_color_frame_line.colorValue = Color.white * 0.7f;
                sp_sf_color_seperater_line.colorValue = XHud_Dashboard.themes_get_color(ThemeColorType.primary);
                sp_sf_mark_size.floatValue = 10;
                sp_sf_safe_frame_line_width.floatValue = 0.5f;
                sp_sf_seperater_length.floatValue = 30f;
                sp_sf_line_margins.vector2Value = new Vector2(50, 50);
                sp_sf_center_mark_distance.floatValue = 40f;
                sp_sf_center_mark_width.floatValue = 1f;
                sp_sf_center_mark_length.floatValue = 12f;

                xHud_EditorUpdate_HelperVisual();
                xHud_EditorUpdate_Canvas_Camera();
            }

            if (XGUI.ChangedCheck_End())
            {
                SetHighFrequencyMode();
            }
        }
        #endregion

        #region 工具 - 物体结构显示
        /// <summary>
        /// 物体结构显示。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>按 <paramref name="state"/> 切换安全框、蓝图根节点与锚点标记的 <c>HideFlags</c>。</description></item>
        /// </list>
        /// </summary>
        /// <param name="state">为 <c>true</c> 时显示，为 <c>false</c> 时隐藏。</param>
        private void xHud_HiddenStructureDisplayer(bool state)
        {
            Transform SafeFrame = (Transform)sp_sf_safeframe.objectReferenceValue;
            Transform BluePrintRoot = (Transform)sp_bp_root.objectReferenceValue;
            if (!state)
            {
                if (SafeFrame != null)
                    SafeFrame.gameObject.hideFlags = HideFlags.HideInHierarchy;
                if (BluePrintRoot != null)
                    BluePrintRoot.gameObject.hideFlags = HideFlags.HideInHierarchy;

                if (sp_cv_anchors_layout_screen != null && sp_cv_anchors_layout_screen.arraySize > 0)
                    for (int i = 0; i < sp_cv_anchors_layout_screen.arraySize; i++)
                    {
                        SerializedProperty sp_anchorlay = sp_cv_anchors_layout_screen.GetArrayElementAtIndex(i);
                        if (sp_anchorlay != null)
                        {
                            Image obj_anchorlay = (Image)sp_anchorlay.FindPropertyRelative(nameof(Anchor_Layout.Mark)).objectReferenceValue;
                            //sp_DebugMode.Log(obj_anchorlay);
                            if (obj_anchorlay != null)
                            {
                                obj_anchorlay.gameObject.hideFlags = HideFlags.HideInHierarchy;
                            }
                        }
                    }
            }
            else
            {
                if (SafeFrame != null)
                    SafeFrame.gameObject.hideFlags = HideFlags.None;
                if (BluePrintRoot != null)
                    BluePrintRoot.gameObject.hideFlags = HideFlags.None;

                if (sp_cv_anchors_layout_screen != null && sp_cv_anchors_layout_screen.arraySize > 0)
                    for (int i = 0; i < sp_cv_anchors_layout_screen.arraySize; i++)
                    {
                        Image obj_anchorlay = (Image)sp_cv_anchors_layout_screen.GetArrayElementAtIndex(i).FindPropertyRelative(nameof(Anchor_Layout.Mark)).objectReferenceValue;

                        if (obj_anchorlay != null)
                        {
                            obj_anchorlay.gameObject.hideFlags = HideFlags.None;
                        }
                    }
            }
        }
        #endregion
    }
}