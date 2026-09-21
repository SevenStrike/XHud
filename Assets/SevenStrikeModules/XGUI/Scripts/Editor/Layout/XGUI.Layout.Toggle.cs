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
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// 为 Editor 界面提供样式控件
    /// </summary>
    public static partial class XGUI
    {
        /// <summary>
        /// 使用自动布局绘制一个带有标签的开关控件（Toggle/Switch），基于 SerializedProperty。
        /// </summary>
        /// <param name="title">开关左侧显示的标签文本。为 <c>null</c> 或空时不显示标签。</param>
        /// <param name="title_color">标签文本的颜色。传入 <see cref="Color.clear"/> 时使用半透明白色。</param>
        /// <param name="title_size">标签文本的字体大小。</param>
        /// <param name="title_font_style">标签文本的字体样式。</param>
        /// <param name="title_anchor">标签文本的对齐方式。</param>
        /// <param name="title_width">标签的固定宽度。</param>
        /// <param name="title_font">标签文本的字体。为 <c>null</c> 时使用样式默认字体。</param>
        /// <param name="title_padding">标签的内边距。</param>
        /// <param name="prop">包含当前布尔值的序列化属性。</param>
        /// <param name="tog_padding">开关的内边距。</param>
        /// <param name="tog_margin">开关的外边距。</param>
        /// <param name="tog_style">开关的外观样式（实体、嵌入等）。</param>
        /// <param name="tog_bg_off_color">开关关闭状态背景的颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="tog_bg_on_color">开关开启状态背景的颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="tog_handler_off_color">开关控制柄关闭状态的颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="tog_handler_on_color">开关控制柄开启状态的颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="tog_mixed_bg_fill">混合状态背景的填充样式。</param>
        /// <param name="tog_mixed_bg_color">混合状态背景的颜色主题。</param>
        /// <param name="tog_mixed_bg_color_gui">混合状态背景的叠加颜色。</param>
        /// <param name="tog_mixed_text_size">混合状态按钮文本的字体大小。</param>
        /// <param name="tog_mixed_font_style">混合状态按钮文本的字体样式。</param>
        /// <param name="tog_mixed_text_color">混合状态按钮文本的颜色。</param>
        /// <param name="tog_mixed_text_anchor">混合状态按钮文本的对齐方式。</param>
        /// <param name="tog_mixed_text_padding">混合状态按钮文本的内边距。</param>
        /// <param name="tog_mixed_text_font">混合状态按钮文本的字体。</param>
        /// <param name="tog_mixed_added_height">混合状态按钮额外增加的高度。</param>
        /// <param name="tog_mixed_options">混合状态菜单的选项文本数组。</param>
        /// <param name="act_on_changed">开关状态改变时的回调委托，参数为新的布尔值。</param>
        /// <returns>当前开关的布尔值。</returns>
        /// <remarks>
        /// 该方法与 <see cref="gui_toggle(Rect, string, Color, XGUIFontSize, FontStyle, TextAnchor, float, Font, RectOffset, SerializedProperty, float, XGUIToggleStyle, Color, Color, Color, Color, XGUIFilled, XGUIColor, Color, XGUIFontSize, FontStyle, Color, TextAnchor, RectOffset, Font, float, string[])"/> 
        /// 功能相同，但使用 <see cref="EditorGUILayout"/> 自动布局系统。
        /// <para>
        /// 支持多选混合状态显示（显示 "—"），当 <paramref name="prop"/> 存在多个不同值时自动切换为混合模式。
        /// 标签宽度会根据窗口宽度自适应，窄屏时自动缩小。
        /// </para>
        /// </remarks>
        public static bool layout_toggle(string title = null, Color title_color = default, XGUIFontSize title_size = XGUIFontSize.M, FontStyle title_font_style = FontStyle.Normal, TextAnchor title_anchor = TextAnchor.MiddleLeft, float title_width = 80, Font title_font = null, RectOffset title_padding = default, SerializedProperty prop = null, RectOffset tog_padding = default, RectOffset tog_margin = default, XGUIToggleStyle tog_style = XGUIToggleStyle.实体, Color tog_bg_off_color = default, Color tog_bg_on_color = default, Color tog_handler_off_color = default, Color tog_handler_on_color = default, XGUIFilled tog_mixed_bg_fill = XGUIFilled.实体, XGUIColor tog_mixed_bg_color = XGUIColor.亮白, Color tog_mixed_bg_color_gui = default, XGUIFontSize tog_mixed_text_size = XGUIFontSize.M, FontStyle tog_mixed_font_style = FontStyle.Normal, Color tog_mixed_text_color = default, TextAnchor tog_mixed_text_anchor = TextAnchor.MiddleCenter, RectOffset tog_mixed_text_padding = default, Font tog_mixed_text_font = null, float tog_mixed_added_height = 0, string[] tog_mixed_options = null, Action<bool> act_on_changed = null)
        {
            if (tog_mixed_text_padding == null)
                tog_mixed_text_padding = new RectOffset(0, 0, 0, 0);

            if (tog_padding == null)
                tog_padding = new RectOffset(0, 0, 0, 0);

            if (tog_margin == null)
                tog_margin = new RectOffset(0, 0, 0, 0);

            if (title_color == Color.clear)
                title_color = Color.white * 0.9f;

            if (tog_mixed_text_color == Color.clear)
                tog_mixed_text_color = Color.white;

            if (tog_bg_off_color == Color.clear)
                tog_bg_off_color = Color.white;

            if (tog_bg_on_color == Color.clear)
                tog_bg_on_color = Color.white;

            if (tog_handler_off_color == Color.clear)
                tog_handler_off_color = Color.white;

            if (tog_handler_on_color == Color.clear)
                tog_handler_on_color = Color.white;

            XGUI.layout_group_start(
                type: XGUIContainerType.Horizontal,
                absolute_padding: true,
                absolute_margin: true,
                padding: tog_padding,
                margin: tog_margin);

            float width = XGUI.GetCurrentWindowWidth();
            //Debug.Log(width);
            XGUI.layout_label(
                text: title,
                size: title_size,
                text_color: title_color,
                offset: new Vector2(0, 0),
                padding: title_padding,
                margin: new RectOffset(width > 114 ? 15 : 0, width > 114 ? 10 : 0, 0, 0),
                clipping: TextClipping.Clip,
                anchor: title_anchor,
                width: width > 114 ? title_width : width * 0.1f,
                font: title_font);

            #region 下拉菜单样式
            GUIStyle style = new GUIStyle(XGUI.style_xg_popup);
            style.normal.background = GetFillTexture(tog_mixed_bg_fill, tog_mixed_bg_color);
            style.normal.textColor = tog_mixed_text_color;
            style.fontSize = XGUI.GetFontSize(tog_mixed_text_size);

            if (tog_mixed_text_font != null)
                style.font = tog_mixed_text_font;

            RectOffset _p = style.padding;
            style.padding = new RectOffset(_p.left + tog_mixed_text_padding.left, _p.right + tog_mixed_text_padding.right, _p.top + tog_mixed_text_padding.top, _p.bottom + tog_mixed_text_padding.bottom);

            if (tog_mixed_added_height != 0)
                style.fixedHeight += tog_mixed_added_height;
            style.alignment = tog_mixed_text_anchor;
            #endregion

            #region 开关控件
            // 检查是否为多选混合状态
            bool isMixed = prop.hasMultipleDifferentValues;

            if (isMixed)
            {
                GUI.backgroundColor = tog_mixed_bg_color_gui;
                // 显示带“—”的按钮
                if (GUILayout.Button("—", style, GUILayout.MinWidth(20), GUILayout.Height(XGUI.GetSingleLineHeight())))
                {
                    // 点击时可以选择应用到所有
                    GenericMenu menu = new GenericMenu();
                    for (int i = 0; i < tog_mixed_options.Length; i++)
                    {
                        int index = i; // 关键：创建局部变量捕获当前值

                        // 捕获 act_on_changed 回调
                        var callback = act_on_changed;

                        menu.AddItem(new GUIContent(tog_mixed_options[i]), false, () => SetToggleValue(prop, index, callback));
                    }
                    menu.ShowAsContext();
                }
                GUI.backgroundColor = Color.white;
            }
            else
            {
                #region 开关 - 背景
                GUI.contentColor = (tog_style != XGUIToggleStyle.嵌入 ? (prop.boolValue ? tog_bg_on_color * 0.85f : tog_bg_off_color) : Color.white);
                Texture2D texture_bg = XGUI.GetToggleBgTexture(tog_style);
                GUILayout.Box(new GUIContent(texture_bg), style_xg_toggle, GUILayout.MinWidth(texture_bg.width), GUILayout.MinHeight(texture_bg.height));
                GUI.contentColor = Color.white;
                #endregion

                Rect rect_toggle_bg = XGUI.GetLastRect();

                #region 控制柄阴影
                if (tog_style == XGUIToggleStyle.实体 && prop.boolValue)
                {
                    Texture2D texture_handler_shadow = XGUI.GetToggleHandlerShadowTexture(prop.boolValue ? XGUIToggleHandlerState.开 : XGUIToggleHandlerState.关);
                    float tg = (rect_toggle_bg.x + rect_toggle_bg.width) - texture_handler_shadow.width - (prop.boolValue ? 1 : 4);
                    Rect rect_toggle_handle_shadow = new Rect(tg, rect_toggle_bg.y, texture_handler_shadow.width, texture_handler_shadow.height);
                    XGUI.gui_box(
                        rect: rect_toggle_handle_shadow,
                        bg: texture_handler_shadow,
                        bg_color_gui: Color.white,
                        offset: new Vector2(0, 0),
                        border: new RectOffset(0, 0, 0, 0),
                        margin: new RectOffset(0, 0, 0, 0));
                }
                #endregion

                #region 开关 - 控制柄
                Texture2D texture_handler = XGUI.GetToggleHandlerTexture();
                float _x_false = rect_toggle_bg.x + rect_toggle_bg.width - texture_bg.width + 1;
                float _x_true = rect_toggle_bg.x + rect_toggle_bg.width - (texture_bg.width / 2) - 3;
                float _y = rect_toggle_bg.y;
                float _w = texture_handler.width;
                float _h = rect_toggle_bg.height;
                Rect rect_handler = new Rect(prop.boolValue ? _x_true : _x_false, _y, _w, _h);

                GUI.contentColor = prop.boolValue ? tog_handler_on_color : tog_handler_off_color;
                EditorGUI.LabelField(rect_handler, new GUIContent(texture_handler), style_xg_toggle_handler);
                GUI.contentColor = Color.white;
                #endregion

                #region 开关 - 点击事件
                Event currentEvent = Event.current;

                switch (currentEvent.type)
                {
                    case EventType.MouseUp:
                        if (rect_toggle_bg.Contains(currentEvent.mousePosition))
                        {
                            // 切换bool值
                            prop.boolValue = !prop.boolValue;

                            // 标记GUI为已更改，以便保存
                            EditorUtility.SetDirty(prop.serializedObject.targetObject);

                            if (act_on_changed != null)
                                act_on_changed(prop.boolValue);

                            // 标记事件为已使用，防止穿透
                            currentEvent.Use();
                        }
                        break;
                }
                #endregion
            }

            prop.serializedObject.ApplyModifiedProperties();
            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);

            return prop.boolValue;
        }
        /// <summary>
        /// 设置开关属性的值并触发回调（用于泛型菜单回调）。
        /// </summary>
        /// <param name="prop">要设置的序列化布尔属性。</param>
        /// <param name="value">要设置的值（0 表示 false，1 表示 true）。</param>
        /// <param name="act_on_changed">值改变时的回调委托。</param>
        private static void SetToggleValue(SerializedProperty prop, int value, Action<bool> act_on_changed = null)
        {
            bool newValue = (value == 0 ? false : true);
            prop.boolValue = newValue;
            prop.serializedObject.ApplyModifiedProperties();

            // 调用回调
            if (act_on_changed != null)
                act_on_changed(newValue);
        }
        /// <summary>
        /// 使用自动布局绘制一个带有标签的开关控件（Toggle/Switch），基于值类型。
        /// </summary>
        /// <param name="title">开关左侧显示的标签文本。为 <c>null</c> 或空时不显示标签。</param>
        /// <param name="title_color">标签文本的颜色。传入 <see cref="Color.clear"/> 时使用半透明白色。</param>
        /// <param name="title_size">标签文本的字体大小。</param>
        /// <param name="title_font_style">标签文本的字体样式。</param>
        /// <param name="title_anchor">标签文本的对齐方式。</param>
        /// <param name="title_width">标签的固定宽度。</param>
        /// <param name="title_font">标签文本的字体。为 <c>null</c> 时使用样式默认字体。</param>
        /// <param name="title_padding">标签的内边距。</param>
        /// <param name="prop">当前的布尔值。</param>
        /// <param name="tog_padding">开关的内边距。</param>
        /// <param name="tog_margin">开关的外边距。</param>
        /// <param name="tog_style">开关的外观样式（实体、嵌入等）。</param>
        /// <param name="tog_bg_off_color">开关关闭状态背景的颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="tog_bg_on_color">开关开启状态背景的颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="tog_handler_off_color">开关控制柄关闭状态的颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="tog_handler_on_color">开关控制柄开启状态的颜色。传入 <see cref="Color.clear"/> 时使用白色。</param>
        /// <param name="tog_mixed_bg_fill">混合状态背景的填充样式（值类型版本不启用混合模式，保留参数）。</param>
        /// <param name="tog_mixed_bg_color">混合状态背景的颜色主题（值类型版本不启用混合模式，保留参数）。</param>
        /// <param name="tog_mixed_bg_color_gui">混合状态背景的叠加颜色（值类型版本不启用混合模式，保留参数）。</param>
        /// <param name="tog_mixed_text_size">混合状态按钮文本的字体大小（值类型版本不启用混合模式，保留参数）。</param>
        /// <param name="tog_mixed_font_style">混合状态按钮文本的字体样式（值类型版本不启用混合模式，保留参数）。</param>
        /// <param name="tog_mixed_text_color">混合状态按钮文本的颜色（值类型版本不启用混合模式，保留参数）。</param>
        /// <param name="tog_mixed_text_anchor">混合状态按钮文本的对齐方式（值类型版本不启用混合模式，保留参数）。</param>
        /// <param name="tog_mixed_text_padding">混合状态按钮文本的内边距（值类型版本不启用混合模式，保留参数）。</param>
        /// <param name="tog_mixed_text_font">混合状态按钮文本的字体（值类型版本不启用混合模式，保留参数）。</param>
        /// <param name="tog_mixed_added_height">混合状态按钮额外增加的高度（值类型版本不启用混合模式，保留参数）。</param>
        /// <param name="tog_mixed_options">混合状态菜单的选项文本数组（值类型版本不启用混合模式，保留参数）。</param>
        /// <param name="act_on_changed">开关状态改变时的回调委托，参数为新的布尔值。</param>
        /// <returns>用户切换后的布尔值。</returns>
        /// <remarks>
        /// 该版本基于值类型（bool），适用于非序列化属性的场景，不处理多选混合状态。
        /// 标签宽度会根据窗口宽度自适应，窄屏时自动缩小。
        /// </remarks>
        public static bool layout_toggle(string title = null, Color title_color = default, XGUIFontSize title_size = XGUIFontSize.M, FontStyle title_font_style = FontStyle.Normal, TextAnchor title_anchor = TextAnchor.MiddleLeft, float title_width = 80, Font title_font = null, RectOffset title_padding = default, bool prop = false, RectOffset tog_padding = default, RectOffset tog_margin = default, XGUIToggleStyle tog_style = XGUIToggleStyle.实体, Color tog_bg_off_color = default, Color tog_bg_on_color = default, Color tog_handler_off_color = default, Color tog_handler_on_color = default, XGUIFilled tog_mixed_bg_fill = XGUIFilled.实体, XGUIColor tog_mixed_bg_color = XGUIColor.亮白, Color tog_mixed_bg_color_gui = default, XGUIFontSize tog_mixed_text_size = XGUIFontSize.M, FontStyle tog_mixed_font_style = FontStyle.Normal, Color tog_mixed_text_color = default, TextAnchor tog_mixed_text_anchor = TextAnchor.MiddleCenter, RectOffset tog_mixed_text_padding = default, Font tog_mixed_text_font = null, float tog_mixed_added_height = 0, string[] tog_mixed_options = null, Action<bool> act_on_changed = null)
        {
            if (tog_mixed_text_padding == null)
                tog_mixed_text_padding = new RectOffset(0, 0, 0, 0);

            if (tog_padding == null)
                tog_padding = new RectOffset(0, 0, 0, 0);

            if (tog_margin == null)
                tog_margin = new RectOffset(0, 0, 0, 0);

            if (title_color == Color.clear)
                title_color = Color.white * 0.9f;

            if (tog_mixed_text_color == Color.clear)
                tog_mixed_text_color = Color.white;

            if (tog_bg_off_color == Color.clear)
                tog_bg_off_color = Color.white;

            if (tog_bg_on_color == Color.clear)
                tog_bg_on_color = Color.white;

            if (tog_handler_off_color == Color.clear)
                tog_handler_off_color = Color.white;

            if (tog_handler_on_color == Color.clear)
                tog_handler_on_color = Color.white;

            XGUI.layout_group_start(
                type: XGUIContainerType.Horizontal,
                absolute_padding: true,
                absolute_margin: true,
                padding: tog_padding,
                margin: tog_margin);

            float width = XGUI.GetCurrentWindowWidth();
            //Debug.Log(width);
            XGUI.layout_label(
                text: title,
                size: title_size,
                text_color: title_color,
                offset: new Vector2(0, 0),
                padding: title_padding,
                margin: new RectOffset(width > 114 ? 15 : 0, width > 114 ? 10 : 0, 0, 0),
                clipping: TextClipping.Clip,
                anchor: title_anchor,
                width: width > 114 ? title_width : width * 0.1f,
                font: title_font);

            #region 下拉菜单样式
            GUIStyle style = new GUIStyle(XGUI.style_xg_popup);
            style.normal.background = GetFillTexture(tog_mixed_bg_fill, tog_mixed_bg_color);
            style.normal.textColor = tog_mixed_text_color;
            style.fontSize = XGUI.GetFontSize(tog_mixed_text_size);

            if (tog_mixed_text_font != null)
                style.font = tog_mixed_text_font;

            RectOffset _p = style.padding;
            style.padding = new RectOffset(_p.left + tog_mixed_text_padding.left, _p.right + tog_mixed_text_padding.right, _p.top + tog_mixed_text_padding.top, _p.bottom + tog_mixed_text_padding.bottom);

            if (tog_mixed_added_height != 0)
                style.fixedHeight += tog_mixed_added_height;
            style.alignment = tog_mixed_text_anchor;
            #endregion

            #region 开关控件
            #region 开关 - 背景
            GUI.contentColor = (tog_style != XGUIToggleStyle.嵌入 ? (prop ? tog_bg_on_color * 0.85f : tog_bg_off_color) : Color.white);
            Texture2D texture_bg = XGUI.GetToggleBgTexture(tog_style);
            GUILayout.Box(new GUIContent(texture_bg), style_xg_toggle, GUILayout.MinWidth(texture_bg.width), GUILayout.MinHeight(texture_bg.height));
            GUI.contentColor = Color.white;
            #endregion

            Rect rect_toggle_bg = XGUI.GetLastRect();

            #region 控制柄阴影
            if (tog_style == XGUIToggleStyle.实体 && prop)
            {
                Texture2D texture_handler_shadow = XGUI.GetToggleHandlerShadowTexture(prop ? XGUIToggleHandlerState.开 : XGUIToggleHandlerState.关);
                float tg = (rect_toggle_bg.x + rect_toggle_bg.width) - texture_handler_shadow.width - (prop ? 1 : 4);
                Rect rect_toggle_handle_shadow = new Rect(tg, rect_toggle_bg.y, texture_handler_shadow.width, texture_handler_shadow.height);
                XGUI.gui_box(
                    rect: rect_toggle_handle_shadow,
                    bg: texture_handler_shadow,
                    bg_color_gui: Color.white,
                    offset: new Vector2(0, 0),
                    border: new RectOffset(0, 0, 0, 0),
                    margin: new RectOffset(0, 0, 0, 0));
            }
            #endregion

            #region 开关 - 控制柄
            Texture2D texture_handler = XGUI.GetToggleHandlerTexture();
            float _x_false = rect_toggle_bg.x + rect_toggle_bg.width - texture_bg.width + 1;
            float _x_true = rect_toggle_bg.x + rect_toggle_bg.width - (texture_bg.width / 2) - 3;
            float _y = rect_toggle_bg.y;
            float _w = texture_handler.width;
            float _h = rect_toggle_bg.height;
            Rect rect_handler = new Rect(prop ? _x_true : _x_false, _y, _w, _h);

            GUI.contentColor = prop ? tog_handler_on_color : tog_handler_off_color;
            EditorGUI.LabelField(rect_handler, new GUIContent(texture_handler), style_xg_toggle_handler);
            GUI.contentColor = Color.white;
            #endregion

            #region 开关 - 点击事件
            Event currentEvent = Event.current;

            switch (currentEvent.type)
            {
                case EventType.MouseUp:
                    if (rect_toggle_bg.Contains(currentEvent.mousePosition))
                    {
                        // 切换bool值
                        prop = !prop;

                        if (act_on_changed != null)
                            act_on_changed(prop);

                        // 标记事件为已使用，防止穿透
                        currentEvent.Use();
                    }
                    break;
            }
            #endregion

            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);

            return prop;
        }
    }
}