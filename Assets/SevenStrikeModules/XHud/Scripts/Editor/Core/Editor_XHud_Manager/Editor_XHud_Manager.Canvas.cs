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
    using UnityEngine.UI;

    public partial class Editor_XHud_Manager : Editor
    {
        #region 绘制 - 画布
        /// <summary>
        /// 绘制「画布」面板内容。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>绘制画布尺寸、高度优先、画布距离等属性字段。</description></item>
        /// <item><description>提供「获取分辨率」按钮，读取主 GameView 尺寸并同步到 CanvasScaler。</description></item>
        /// <item><description>绘制关于超大分辨率的说明标签。</description></item>
        /// </list>
        /// </summary>
        private void Draw_Canvas()
        {
            string hexcol = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary, true);

            #region 画布尺寸    
            XGUI.ChangedCheck_Start();
            XGUI.layout_property_field(
             title: "画布尺寸",
             title_size: XGUIFontSize.M,
             //title_color: Color.white,
             title_hover_color: XHud_Dashboard.Theme_Primary,
             title_width: 100,
             status_icon: "icon_field_status",
             status_icon_color: CanvasScalerScreenSize.vector2Value != Vector2.zero ? XHud_Dashboard.Theme_Primary : Color.red,
             prop: CanvasScalerScreenSize,
             prop_margin: new RectOffset(0, 0, 10, 10));
            if (XGUI.ChangedCheck_End())
            {
                CanvasScaler scaler = (CanvasScaler)HudCanvasScaler.objectReferenceValue;
                scaler.referenceResolution = CanvasScalerScreenSize.vector2Value;
                ScreenRes.vector2Value = CanvasScalerScreenSize.vector2Value;
            }
            #endregion

            #region 高度优先    
            XGUI.ChangedCheck_Start();
            XGUI.layout_property_field(
             title: "高度优先",
             title_size: XGUIFontSize.M,
             //title_color: Color.white,
             title_hover_color: XHud_Dashboard.Theme_Primary,
             title_width: 100,
             //status_icon: "icon_field_status",
             //status_icon_color: CanvasMatchDir.floatValue != 0 ? XHud_Dashboard.Theme_Primary : Color.red,
             prop: CanvasMatchDir,
             prop_margin: new RectOffset(0, 0, 10, 10));
            if (XGUI.ChangedCheck_End())
            {
                CanvasScaler scaler = (CanvasScaler)HudCanvasScaler.objectReferenceValue;
                scaler.matchWidthOrHeight = CanvasMatchDir.floatValue;
            }
            #endregion

            #region 画布距离     
            if (HudCanvasAnchorIndex.intValue == 2)
            {
                XGUI.layout_space(5);

                XGUI.layout_property_field(
                    title: "画布距离",
                    title_size: XGUIFontSize.M,
                    //title_color: Color.white,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 100,
                    prop: CanvasDistance,
                    prop_margin: new RectOffset(0, 0, 10, 10));
            }
            #endregion

            #region 获取分辨率
            if (XGUI.layout_button(
                text: "获取分辨率",
                tooltip: "",
                bg_fill: XGUIGroup.实体,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.Theme_Primary,
                button_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(XHud_Dashboard.Theme_Primary) ? Color.black : Color.white,
                press_fill: XGUIGroup.实体,
                press_color: XGUIColor.深空灰,
                press_text_color: Color.white,
                font_size: XGUIFontSize.M,
                anchor: TextAnchor.MiddleCenter,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                button_text_font: XGUI.GetFont("xg-medium")))
            {
                CanvasScalerScreenSize.vector2Value = xHud_GetMainGameViewSize();
                CanvasScalerScreenSize.serializedObject.ApplyModifiedProperties();

                CanvasScaler scaler = (CanvasScaler)HudCanvasScaler.objectReferenceValue;
                scaler.referenceResolution = CanvasScalerScreenSize.vector2Value;

                ScreenRes.vector2Value = CanvasScalerScreenSize.vector2Value;

                xHud_RatioReference_Update();
            }
            #endregion

            #region 说明
            string msg = $"请注意！如果您的目标分辨率尺寸是<b><color={hexcol}> 大于 8192 x 8192 </color></b>的情况下，建议<b><color={hexcol}> 手动输入画布尺寸 </color></b>为您的目标分辨率尺寸！因为当您设置大于8192的分辨率时，Unity内部会将其钳位或缩放到可显示范围内！";

            XGUI.layout_label(
                text: msg,
                bg_fill: XGUIGroup.实体,
                bg_color: XGUIColor.深空灰,
                size: XGUIFontSize.S,
                anchor: TextAnchor.MiddleLeft,
                text_color: Color.white * 0.85f,
                offset: new Vector2(0, 0),
                padding: new RectOffset(10, 10, 10, 10),
                margin: new RectOffset(0, 0, 6, 0),
                clipping: TextClipping.Clip,
                wrap: true,
                font: XGUI.GetFont("xg-regular"),
                font_style: FontStyle.Normal);
            #endregion
        }
        #endregion
    }
}