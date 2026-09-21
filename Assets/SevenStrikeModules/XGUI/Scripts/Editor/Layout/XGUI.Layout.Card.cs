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
    using UnityEngine;

    /// <summary>
    /// 为 Editor 界面提供样式控件
    /// </summary>
    public static partial class XGUI
    {
        public static Rect layout_card(CardDataWrap data, bool forward = true, XGUICardStyle style = XGUICardStyle.实体, string title = null, float card_height = 40, float card_arrow_region_width = 40, Color color_arrow_l = default, Color color_arrow_r = default, Action on_press_left = null, Action on_press_right = null)
        {
            Color col_near = data._datas[data.index]._bgcolor;
            Color col_mid = data.index <= 0 ? Color.white * 0.4f : data._datas[data.index - 1]._bgcolor * 0.85f;
            Color col_far = data.index <= 0 ? Color.white * 0.4f : (data.index > 1 ? data._datas[data.index - 2]._bgcolor * 0.85f : Color.white * 0.4f);

            Rect rect = XGUI.GetControlRect(false, Mathf.Clamp(card_height, 40, 1000));
            Texture2D near = XGUI.GetCustomIcon($"{XGUI_Dashboard.get_path_xgui_shapes()}Card/{(style == XGUICardStyle.实体 ? "card_near" : "card_near_edge")}");
            Texture2D mid = XGUI.GetCustomIcon($"{XGUI_Dashboard.get_path_xgui_shapes()}Card/{(style == XGUICardStyle.实体 ? "card_mid" : "card_mid_edge")}");
            Texture2D far = XGUI.GetCustomIcon($"{XGUI_Dashboard.get_path_xgui_shapes()}Card/{(style == XGUICardStyle.实体 ? "card_far" : "card_far_edge")}");
            Texture2D arrow_l = XGUI.GetBasedIcon("icon_mark_arrow_left");
            Texture2D arrow_r = XGUI.GetBasedIcon("icon_mark_arrow_right");

            XGUI.gui_box(
               rect: new Rect(rect.x + 30, rect.y, rect.width - 60, far.height),
               bg: far,
               bg_color_gui: col_far,
               offset: new Vector2(0, 0),
               border: new RectOffset(18, 18, 0, 0),
               margin: new RectOffset(0, 0, 0, 0));

            XGUI.gui_box(
                rect: new Rect(rect.x + 12, rect.y + 8, rect.width - 24, mid.height),
                bg: mid,
                bg_color_gui: col_mid,
                offset: new Vector2(0, 0),
                border: new RectOffset(18, 18, 0, 0),
                margin: new RectOffset(0, 0, 0, 0));

            Rect rect_near = new Rect(rect.x, rect.y + 18, rect.width, rect.height - 12);
            XGUI.gui_box(
                rect: rect_near,
                bg: near,
                bg_color_gui: col_near,
                offset: new Vector2(0, 0),
                border: new RectOffset(18, 18, 21, 16),
                margin: new RectOffset(0, 0, 0, 0));

            float clac = Mathf.Clamp(card_arrow_region_width, 20, rect_near.width / 2);

            Event e = Event.current;
            if (rect_near.Contains(e.mousePosition))
            {
                Rect rect_arrow_l_interact = new Rect(rect_near.x, rect_near.y, clac, rect_near.height);
                Rect rect_arrow_l = new Rect(rect_near.x + 15, rect_near.y + (rect_near.height / 2) - (arrow_l.height / 2) + 4, arrow_l.width, arrow_l.height);
                XGUI.gui_box(
                   rect: rect_arrow_l,
                   bg: arrow_l,
                   bg_color_gui: style == XGUICardStyle.边框 ? color_arrow_l : color_arrow_l == Color.clear ? (XGUI_Utilitys.ColorBrightness_LimiteGet(col_near) ? Color.white : Color.black) : color_arrow_l,
                   offset: new Vector2(0, 0),
                   margin: new RectOffset(0, 0, 0, 0));

                Rect rect_arrow_r_interact = new Rect(rect_near.x + (rect_near.width - clac), rect_near.y, clac, rect_near.height);
                Rect rect_arrow_r = new Rect(rect_near.x + (rect_near.width - 30), rect_near.y + (rect_near.height / 2) - (arrow_l.height / 2) + 4, arrow_l.width, arrow_l.height);
                XGUI.gui_box(
                  rect: rect_arrow_r,
                  bg: arrow_r,
                  bg_color_gui: style == XGUICardStyle.边框 ? color_arrow_r : color_arrow_r == Color.clear ? (XGUI_Utilitys.ColorBrightness_LimiteGet(col_near) ? Color.white : Color.black) : color_arrow_r,
                  offset: new Vector2(0, 0),
                  margin: new RectOffset(0, 0, 0, 0));

                //XGUI.gui_box(rect_arrow_l_interact, Color.red);
                //XGUI.gui_box(rect_arrow_r_interact, Color.red);

                if (rect_arrow_l_interact.Contains(e.mousePosition))
                {
                    if (e.type == EventType.MouseDown)
                    {
                        if (on_press_left != null)
                            on_press_left();
                    }
                }
                if (rect_arrow_r_interact.Contains(e.mousePosition))
                {
                    if (e.type == EventType.MouseDown)
                    {
                        if (on_press_right != null)
                            on_press_right();
                    }
                }

                if (e.type == EventType.ScrollWheel)
                {
                    // 获取滚轮滚动增量
                    float scrollDelta = forward ? -e.delta.y : e.delta.y;

                    // 正数表示向上滚动，负数表示向下滚动
                    if (scrollDelta > 0)
                    {
                        if (on_press_right != null)
                            on_press_right();
                    }
                    else if (scrollDelta < 0)
                    {
                        if (on_press_left != null)
                            on_press_left();
                    }

                    //if (data.index != data._datas.Length - 1 && data.index != 0)
                    //{
                    // 使用滚轮事件后，如果需要防止继续传递
                    e.Use();
                    //}
                }
            }

            return rect_near;
        }
    }
}