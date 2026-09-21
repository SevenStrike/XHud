/*
 * ============================================================================
 * ⚠️ 版权声明（禁止删除、禁止修改、衍生作品必须保留此注释）⚠️
 * ============================================================================
 * 版权声明 Copyright (C) 2025-Present Nanjing SevenStrike Media Co., Ltd.
 * 中文名称：南京塞维斯传媒有限公司
 * 英文名称：SevenStrikeMedia
 * 项目作者：徐寅智
 * 项目名称：XGUI - Unity Editor界面可视化组件工具
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
namespace SevenStrikeModules.XGUI.Editor
{
    using SevenStrikeModules.XGUI.Runtime;
    using System;
    using System.Security.Cryptography;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// 为 Editor 界面提供样式控件
    /// </summary>
    public static partial class XGUI
    {
        public static AnimationCurve gui_curvefield(Rect rect, AnimationCurve prop, string title = null, float title_width = 40, Color title_color = default, string icon = "icon_field_status", Color icon_color = default, string state_title = "当前", string state_value = "-", Color state_value_color = default, Action<AnimationCurve> setcurve = null, bool useutility = true)
        {
            if (icon_color == Color.clear)
                icon_color = Color.white;

            if (state_value_color == Color.clear)
                state_value_color = Color.white;

            if (title_color == Color.clear)
                title_color = Color.white * 0.9f;

            Rect rect_curve = new Rect(rect.x, rect.y, rect.width - (useutility ? 60 : 0), XGUI.GetSingleLineHeight());

            prop = XGUI.gui_inputfield(
                rect: rect_curve,
                title: title,
                prop: prop,
                title_size: XGUIFontSize.M,
                //title_color: Color.white,
                //status_icon: "icon_field_status",
                //status_icon_color: Color.red,
                title_anchor: TextAnchor.MiddleLeft,
                title_width: title_width);

            Rect rect_state = new Rect(rect.x + (rect.width - 50), rect.y + 2, 50, XGUI.GetSingleLineHeight());

            if (useutility)
                if (XGUI.gui_button(
                    rect: rect_state,
                    text: "EASE",
                    tooltip: "fast ease",
                    btn_fill: XGUIFilled.实体,
                    btn_color: XGUIColor.亮白,
                    btn_color_gui: Color.white,
                    btn_text_color: Color.black,
                    press_fill: XGUIFilled.实体,
                    press_color: XGUIColor.深空灰,
                    press_text_color: Color.white,
                    font_size: XGUIFontSize.S,
                    margin: new RectOffset(0, 0, 0, 0),
                    padding: new RectOffset(0, 0, 0, 0)))
                {
                    EaseType[] easeTypes = XGUI_EaseCurveGenerator.EaseTypes_Get();

                    var menu = new GenericMenu();
                    for (int i = 0; i < easeTypes.Length; i++)
                    {
                        // 闭包捕获
                        EaseType type = easeTypes[i];
                        menu.AddItem(new GUIContent($"{type}"), false, () =>
                        {
                            if (setcurve != null)
                            {
                                setcurve(XGUI.GetEaseCurve(type));
                            }
                        });

                    }

                    menu.ShowAsContext();

                }

            // 测试区域
            //XGUI.gui_box(rect_state, Color.red);           

            //prop = curvex;

            return prop;
        }
    }
}