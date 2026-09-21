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
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// 为 Editor 界面提供样式控件
    /// </summary>
    public static partial class XGUI
    {
        public static Color gui_colorfield(Rect rect, Color prop = default, string title = null, float title_width = 40, Color title_color = default, string icon = "icon_field_status", Color icon_color = default, string state_title = "当前", string state_value = "-", Color state_value_color = default, bool display_hex = false)
        {
            if (prop == Color.clear)
                prop = Color.white;

            if (icon_color == Color.clear)
                icon_color = Color.white;

            if (state_value_color == Color.clear)
                state_value_color = Color.white;

            if (title_color == Color.clear)
                title_color = Color.white * 0.9f;

            Color cc = XGUI.gui_inputfield(
                rect: rect,
                title: title,
                prop: prop,
                title_size: XGUIFontSize.M,
                title_color: title_color,
                title_anchor: TextAnchor.MiddleLeft,
                status_icon: icon,
                status_icon_color: icon_color,
                title_width: title_width,
                show_eyedropper: false,
                show_alpha: true,
                hdr: false);

            if (display_hex)
            {
                Rect rect_state = new Rect(rect.x, rect.y + rect.height, rect.width, XGUI.GetSingleLineHeight());

                XGUI.gui_state_displayer_text(
                    rect: rect_state,
                    title: new GUIContent(state_title),
                    title_size: XGUIFontSize.M,
                    title_color: Color.white * 0.75f,
                    subtitle: new GUIContent(state_value, "按住 Alt 键点击可复制 Hex 颜色值"),
                    subtitle_size: XGUIFontSize.M,
                    subtitle_color: state_value_color,
                    margin: new RectOffset(5, 5, 0, 5));

                // 测试区域
                //XGUI.gui_box(rect_state, Color.red);

                Event e = Event.current;
                if (e.type == EventType.MouseDown && (e.modifiers & EventModifiers.Alt) != 0 && rect_state.Contains(e.mousePosition))
                {
                    string hex = XGUI_Utilitys.Color_To_HexString(cc);
                    EditorGUIUtility.systemCopyBuffer = hex;
                    XGUI.dialog(
                        type: XGUIDialogType.确认,
                        windowtitle: "XGUI 控件消息",
                        title: "已复制 Hex格式 颜色值",
                        msg: $"已将\"<color=#{hex}> {hex} </color>\"颜色值复制到系统剪贴板！",
                        ok: "明白",
                        PrimaryIndex: 0);
                    e.Use();
                }
            }

            return cc;
        }
    }
}