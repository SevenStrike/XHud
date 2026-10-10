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
    using SevenStrikeModules.XTween;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    public partial class Editor_XHud_Module_Primitive_Tween_Tracker
    {
        /// <summary>
        /// 绘制选中音效的参数字段。
        /// <para/>
        /// 与 <see cref="DrawNodeParameterFields"/> 对称，但字段集是音效专属的：
        /// Sound / Path / Timings / Delay / Volume / MinPitch / MaxPitch。
        /// </summary>
        private void DrawSoundParameterFields(Rect area, TweenSound sound)
        {
            //  换成缓存入口
            SerializedObject so = GetParameterSerializedObject();
            if (so == null) return;

            so.Update();

            //  复用缓存好的数组属性，不要每帧 FindProperty
            SerializedProperty prop_sounds = cachedSoundsProp;
            if (prop_sounds == null || selectedIndex < 0 || selectedIndex >= prop_sounds.arraySize)
                return;

            SerializedProperty prop_sound = prop_sounds.GetArrayElementAtIndex(selectedIndex);
            SerializedProperty ser_sound = prop_sound.FindPropertyRelative("Sound");
            SerializedProperty ser_path = prop_sound.FindPropertyRelative("Path");
            SerializedProperty ser_timing = prop_sound.FindPropertyRelative("Timing");
            SerializedProperty ser_delay = prop_sound.FindPropertyRelative("Delay");
            SerializedProperty ser_volume = prop_sound.FindPropertyRelative("Volume");
            SerializedProperty ser_minPitch = prop_sound.FindPropertyRelative("MinPitch");
            SerializedProperty ser_maxPitch = prop_sound.FindPropertyRelative("MaxPitch");


            XGUI.ChangedCheck_Start();

            #region 参数
            XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.Theme_Group,
                title: "参数",
                title_text_color: XHud_Dashboard.Theme_Primary,
                title_clipping: XGUI.TryEllipsisClipping(),
                title_size: XGUIFontSize.M,
                title_manual_offset: true,
                title_manual_offset_space: 5,
                absolute_margin: true,
                absolute_padding: true,
                margin: new RectOffset(10, 10, 25, 0),
                padding: new RectOffset(10, 10, 15, 15));

            #region 音效资源
            XGUI.layout_property_field(
                title: "音效",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.Theme_Primary,
                title_width: 80,
                prop: ser_sound,
                prop_padding: new RectOffset(5, 5, 0, 5),
                prop_margin: new RectOffset(5, 5, 0, 0));
            ser_sound.serializedObject.ApplyModifiedProperties();
            #endregion

            #region 路径（只读显示 + 按钮：从音效资源反查）
            XGUI.layout_property_field(
                title: "路径",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.Theme_Primary,
                title_width: 80,
                prop: ser_path,
                prop_padding: new RectOffset(5, 5, 0, 5),
                prop_margin: new RectOffset(5, 5, 0, 0));
            ser_path.serializedObject.ApplyModifiedProperties();
            #endregion

            #region 时机
            string[] TimingTypes = null;

            if (HudButton != null)
            {
                TimingTypes = new string[9] { "无", "鼠标进入", "鼠标退出", "鼠标按下", "鼠标松开", "鼠标长按", "鼠标点击", "鼠标选中", "鼠标取消选中" };
            }
            else if (HudProgress != null)
            {
                TimingTypes = new string[5] { "无", "进度开始时", "进度变化时", "进度结束时", "进度重置时" };
            }
            else if (HudToggle != null)
            {
                TimingTypes = new string[7] { "无", "开关打开时", "开关关闭时", "开关按下时", "开关抬起时", "开关变化时", "开关变化中" };
            }
            else if (HudSlider != null)
            {
                TimingTypes = new string[5] { "无", "按下滑动条", "松开滑动条", "滑动条数值改变", "滑动条数值变化中" };
            }
            else if (HudOption != null)
            {
                TimingTypes = new string[5] { "无", "点击选项", "光标移动开始", "光标移动结束", "光标位置改变" };
            }
            else
            {
                TimingTypes = new string[4] { "元素进入时", "元素进入后", "元素退出时", "自定义" };
            }

            if (string.IsNullOrEmpty(ser_timing.stringValue))
            {
                ser_timing.stringValue = TimingTypes[0];
            }

            ser_timing.stringValue = XGUI.layout_string_popup(
                   title: "时机",
                   title_width: 80,
                   title_size: XGUIFontSize.M,
                   title_anchor: TextAnchor.MiddleLeft,
                   prop: ser_timing,
                   options: TimingTypes,
                   opt_text_size: XGUIFontSize.M,
                   opt_text_color: Color.black,
                   opt_text_padding: new RectOffset(10, 10, 0, 0),
                   opt_anchor: TextAnchor.MiddleLeft,
                   opt_font_style: FontStyle.Normal,
                   opt_bg_fill: XGUIFilled.实体,
                   opt_bg_color: XGUIColor.亮白,
                   opt_bg_color_gui: XHud_Dashboard.Theme_Primary,
                   margin: new RectOffset(0, 0, 0, 0),
                   padding: new RectOffset(10, 9, 0, 5),
                   title_margin: new RectOffset(0, 0, 0, 0),
                   icon_arrow_color: Color.black);

            ser_timing.serializedObject.ApplyModifiedProperties();
            #endregion

            #region 延迟
            XGUI.layout_property_field(
                title: "延迟",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.Theme_Primary,
                title_width: 80,
                prop: ser_delay,
                prop_padding: new RectOffset(5, 5, 0, 5),
                prop_margin: new RectOffset(5, 5, 0, 0));
            ser_delay.serializedObject.ApplyModifiedProperties();
            #endregion

            #region 音量
            XGUI.layout_property_field(
                title: "音量",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.Theme_Primary,
                title_width: 80,
                prop: ser_volume,
                prop_padding: new RectOffset(5, 5, 0, 5),
                prop_margin: new RectOffset(5, 5, 0, 0));
            ser_volume.serializedObject.ApplyModifiedProperties();
            #endregion

            #region 音高
            XGUI.layout_property_field(
                title: "最小音高",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.Theme_Primary,
                title_width: 80,
                prop: ser_minPitch,
                prop_padding: new RectOffset(5, 5, 0, 5),
                prop_margin: new RectOffset(5, 5, 0, 0));
            ser_minPitch.serializedObject.ApplyModifiedProperties();

            XGUI.layout_property_field(
                title: "最大音高",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.Theme_Primary,
                title_width: 80,
                prop: ser_maxPitch,
                prop_padding: new RectOffset(5, 5, 0, 5),
                prop_margin: new RectOffset(5, 5, 0, 0));
            ser_maxPitch.serializedObject.ApplyModifiedProperties();
            #endregion

            XGUI.layout_space(5);

            #region 静音
            sound.Mute = DrawLabeledToggle("静音", sound.Mute, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, new string[] { "禁用", "启用" }, (b) => { });
            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 时长（只读）
            XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.Theme_Group,
                title: "信息",
                title_text_color: XHud_Dashboard.Theme_Primary,
                title_size: XGUIFontSize.M,
                title_manual_offset: true,
                title_manual_offset_space: 5,
                absolute_margin: true,
                absolute_padding: true,
                margin: new RectOffset(10, 10, 20, 0),
                padding: new RectOffset(10, 10, 15, 15));

            string lenText = sound.Sound != null ? $"{sound.Sound.length:F3} 秒" : "—";
            XGUI.layout_state_displayer_text(
                title: "音频时长",
                title_size: XGUIFontSize.M,
                subtitle: lenText,
                subtitle_size: XGUIFontSize.M,
                subtitle_color: XHud_Dashboard.Theme_Primary,
                margin: new RectOffset(5, 5, 0, 5));

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            XGUI.layout_space(6);

            so.ApplyModifiedProperties();

            if (XGUI.ChangedCheck_End())
            {
                if (dragMode == DragMode.无 && !IsPreviewing)
                    Undo.RecordObject(target, "Edit Tween Sound");
                sound.GetSoundPath();
                EditorUtility.SetDirty(target);
                Repaint();
            }
        }
        /// <summary> 
        ///绘制选中节点的参数字段
        ///<para/>使用 EditorGUILayout 自动布局；按 <see cref="TweenNode.Type"/> 动态显示起始值 / 结束值
        /// </summary>
        private void DrawNodeParameterFields(Rect area, Texture2D type_icon, TweenNode node)
        {
            //  换成缓存入口
            SerializedObject so = GetParameterSerializedObject();
            if (so == null) return;

            so.Update();

            //  复用缓存好的数组属性
            SerializedProperty prop_nodes = cachedNodesProp;
            if (prop_nodes == null || selectedIndex < 0 || selectedIndex >= prop_nodes.arraySize)
                return;

            SerializedProperty prop_node = prop_nodes.GetArrayElementAtIndex(selectedIndex);
            SerializedProperty ser_indicator = prop_node.FindPropertyRelative("Indicator");
            SerializedProperty ser_type = prop_node.FindPropertyRelative("Type");
            SerializedProperty ser_duration = prop_node.FindPropertyRelative("Duration");
            SerializedProperty ser_delay = prop_node.FindPropertyRelative("Delay");
            SerializedProperty ser_timing = prop_node.FindPropertyRelative("Timings");
            SerializedProperty ser_loop_type = prop_node.FindPropertyRelative("LoopType");
            SerializedProperty ser_loop_count = prop_node.FindPropertyRelative("LoopCount");
            SerializedProperty ser_ease = prop_node.FindPropertyRelative("Ease");
            SerializedProperty ser_curve = prop_node.FindPropertyRelative("Curve");
            SerializedProperty ser_rotate_mode = prop_node.FindPropertyRelative("RotateMode");
            SerializedProperty ser_written_extended = prop_node.FindPropertyRelative("TextExtended");
            SerializedProperty ser_written_text_cursor = prop_node.FindPropertyRelative("TextCursor");
            SerializedProperty ser_written_text_cursorblinkspeed = prop_node.FindPropertyRelative("TextCursorBlinkSpeed");


            #region 类型图标
            //XGUI.layout_group_start(
            //    type: XGUIContainerType.Horizontal,
            //    bg_fill: XGUIFilled.透明,
            //    bg_color: XGUIColor.亮白,
            //    absolute_margin: true,
            //    absolute_padding: true,
            //    margin: new RectOffset(10, 10, 25, 10),
            //    padding: new RectOffset(0, 0, 0, 0));

            //XGUI.layout_icon(
            //    icon: type_icon,
            //    icon_color: Color.white * 0.7f,
            //    width: 32,
            //    height: 32,
            //    icon_border: new RectOffset(0, 0, 0, 0),
            //    icon_offset: new Vector2(0, 0),
            //    icon_margin: new RectOffset(0, 0, 0, 0),
            //    icon_padding: new RectOffset(0, 0, 0, 0),
            //    icon_alignment: XGUIIconAlignment.中心);

            //XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            #endregion

            XGUI.ChangedCheck_Start();

            #region 参数
            XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.Theme_Group,
                title: "参数",
                title_text_color: XHud_Dashboard.Theme_Primary,
                title_clipping: XGUI.TryEllipsisClipping(),
                title_size: XGUIFontSize.M,
                title_manual_offset: true,
                title_manual_offset_space: 5,
                absolute_margin: true,
                absolute_padding: true,
                margin: new RectOffset(10, 10, 25, 0),
                padding: new RectOffset(10, 10, 15, 15));

            #region 标识
            XGUI.layout_property_field(
              title: "标识",
              title_size: XGUIFontSize.M,
              title_hover_color: XHud_Dashboard.Theme_Primary,
              title_width: 80,
              prop: ser_indicator,
              prop_padding: new RectOffset(5, 5, 0, 5),
              prop_margin: new RectOffset(5, 5, 0, 0));
            ser_indicator.serializedObject.ApplyModifiedProperties();
            #endregion

            #region 类型
            XGUI.layout_property_field(
               title: "类型",
               title_size: XGUIFontSize.M,
               title_hover_color: XHud_Dashboard.Theme_Primary,
               title_width: 80,
               prop: ser_type,
               prop_padding: new RectOffset(5, 5, 0, 5),
              prop_margin: new RectOffset(5, 5, 0, 0));
            ser_type.serializedObject.ApplyModifiedProperties();
            #endregion

            #region 时机
            string[] TimingTypes = null;

            if (HudButton != null)
            {
                TimingTypes = new string[9] { "无", "鼠标进入", "鼠标退出", "鼠标按下", "鼠标松开", "鼠标长按", "鼠标点击", "鼠标选中", "鼠标取消选中" };
            }
            else if (HudProgress != null)
            {
                TimingTypes = new string[5] { "无", "进度开始时", "进度变化时", "进度结束时", "进度重置时" };
            }
            else if (HudToggle != null)
            {
                TimingTypes = new string[7] { "无", "开关打开时", "开关关闭时", "开关按下时", "开关抬起时", "开关变化时", "开关变化中" };
            }
            else if (HudSlider != null)
            {
                TimingTypes = new string[5] { "无", "按下滑动条", "松开滑动条", "滑动条数值改变", "滑动条数值变化中" };
            }
            else if (HudOption != null)
            {
                TimingTypes = new string[5] { "无", "点击选项", "光标移动开始", "光标移动结束", "光标位置改变" };
            }
            else
            {
                TimingTypes = new string[4] { "元素进入时", "元素进入后", "元素退出时", "自定义" };
            }

            ser_timing.stringValue = XGUI.layout_string_popup(
                   title: "时机",
                   title_width: 80,
                   title_size: XGUIFontSize.M,
                   title_anchor: TextAnchor.MiddleLeft,
                   prop: ser_timing,
                   options: TimingTypes,
                   opt_text_size: XGUIFontSize.M,
                   opt_text_color: Color.black,
                   opt_text_padding: new RectOffset(10, 10, 0, 0),
                   opt_anchor: TextAnchor.MiddleLeft,
                   opt_font_style: FontStyle.Normal,
                   opt_bg_fill: XGUIFilled.实体,
                   opt_bg_color: XGUIColor.亮白,
                   opt_bg_color_gui: XHud_Dashboard.Theme_Primary,
                   margin: new RectOffset(0, 0, 0, 0),
                   padding: new RectOffset(10, 9, 0, 5),
                   title_margin: new RectOffset(0, 0, 0, 0),
                   icon_arrow_color: Color.black);

            ser_timing.serializedObject.ApplyModifiedProperties();
            #endregion

            #region 耗时
            XGUI.layout_property_field(
                title: "耗时",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.Theme_Primary,
                title_width: 80,
                prop: ser_duration,
                prop_padding: new RectOffset(5, 5, 0, 5),
                prop_margin: new RectOffset(5, 5, 0, 0));
            ser_duration.serializedObject.ApplyModifiedProperties();
            #endregion

            #region 延迟
            XGUI.layout_property_field(
                title: "延迟",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.Theme_Primary,
                title_width: 80,
                prop: ser_delay,
                prop_padding: new RectOffset(5, 5, 0, 5),
                prop_margin: new RectOffset(5, 5, 0, 0));
            ser_delay.serializedObject.ApplyModifiedProperties();
            #endregion

            #region 缓动
            XGUI.layout_property_field(
                title: "缓动",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.Theme_Primary,
                title_width: 80,
                prop: ser_ease,
                prop_padding: new RectOffset(5, 5, 0, 5),
                prop_margin: new RectOffset(0, 0, 0, 0));
            ser_ease.serializedObject.ApplyModifiedProperties();
            #endregion

            #region 旋转模式
            if (node.Type == TweenNodeType.r_旋转)
            {
                XGUI.layout_property_field(
                    title: "旋转模式",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 80,
                    prop: ser_rotate_mode,
                    prop_padding: new RectOffset(5, 5, 0, 5),
                    prop_margin: new RectOffset(0, 0, 0, 0));
                ser_rotate_mode.serializedObject.ApplyModifiedProperties();
            }
            #endregion

            #region 循环  
            XGUI.layout_property_field(
                title: "循环",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.Theme_Primary,
                title_width: 80,
                prop: ser_loop_type,
                prop_padding: new RectOffset(5, 5, 0, 5),
                prop_margin: new RectOffset(0, 0, 0, 0));
            ser_loop_type.serializedObject.ApplyModifiedProperties();
            #endregion

            #region 循环次数
            XGUI.layout_property_field(
                title: "循环次数",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.Theme_Primary,
                title_width: 80,
                prop: ser_loop_count,
                prop_padding: new RectOffset(5, 5, 0, 5),
                prop_margin: new RectOffset(0, 0, 0, 0));
            ser_loop_type.serializedObject.ApplyModifiedProperties();
            #endregion

            #region 曲线
            if (node.Ease == EaseMode.None)
            {
                XGUI.layout_property_field(
                    title: "曲线",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 80,
                    prop: ser_curve,
                    prop_padding: new RectOffset(5, 5, 0, 5),
                    prop_margin: new RectOffset(0, 0, 0, 0));
                ser_curve.serializedObject.ApplyModifiedProperties();
            }
            #endregion

            #region 光标字符
            if (node.Type == TweenNodeType.w_打字机)
                XGUI.layout_property_field(
                title: "光标字符",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.Theme_Primary,
                title_width: 80,
                prop: ser_written_text_cursor,
                prop_padding: new RectOffset(5, 5, 0, 5),
                prop_margin: new RectOffset(5, 5, 0, 0));
            ser_written_text_cursor.serializedObject.ApplyModifiedProperties();
            #endregion

            #region 光标字符闪烁频率
            if (node.Type == TweenNodeType.w_打字机)
                XGUI.layout_property_field(
                    title: "光标字符闪烁频率",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 80,
                    prop: ser_written_text_cursorblinkspeed,
                    prop_padding: new RectOffset(5, 5, 0, 5),
                    prop_margin: new RectOffset(5, 5, 0, 0));
            ser_written_text_cursorblinkspeed.serializedObject.ApplyModifiedProperties();
            #endregion
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 起始值 / 结束值（按类型动态显示）
            XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.Theme_Group,
                title: "数值",
                title_text_color: XHud_Dashboard.Theme_Primary,
                title_clipping: XGUI.TryEllipsisClipping(),
                title_size: XGUIFontSize.M,
                title_manual_offset: true,
                title_manual_offset_space: 5,
                absolute_margin: true,
                absolute_padding: true,
                margin: new RectOffset(10, 10, 20, 0),
                padding: new RectOffset(10, 10, 15, 10),
                can_foldout: false);

            #region 动画数值过渡模式选项卡
            int value_mode_index = node.ValueModeIndex = XGUI.layout_toolbar(
                      index: node.ValueModeIndex,
                      names: new string[] { "S - D", "D - E", "S - E", "C - E" },
                      bg_normal: XGUIFilled.纯色边框,
                      bg_selected: XGUIFilled.实体,
                      bg_color: XGUIColor.亮白,
                      bg_gui_color: Color.black * 0.5f,
                      text_color_normal: Color.white,
                      text_color_selected: XHud_Dashboard.Theme_Primary,
                      bar_height: 25,
                      text_anchor: TextAnchor.MiddleCenter,
                      text_padding: new RectOffset(10, 10, 0, 0),
                      bar_margin: new RectOffset(0, 0, 5, 5),
                      text_offset: new Vector2(0, -2),
                      text_font: XGUI.GetFont("xg-regular"),
                      text_fontstyle: FontStyle.Bold,
                      bg_width_offset: 5,
                      bg_height_offset: 2,
                      navigate_style: true,
                      navigate_style_bg: XGUIFilled.纯色边框,
                      navigate_style_bg_color: Color.black * 0.5f);
            #endregion

            // 数值模式枚举值同步
            node.TweenValueMode = (TweenValueMode)value_mode_index;

            #region 模式说明按钮（弹出说明弹窗）
            Rect rect_toolbar = XGUI.GetLastRect();
            if (XGUI.gui_button(
                rect: new Rect(rect_toolbar.x + (rect_toolbar.width - icon_help_r.width) + 5, rect_toolbar.y - icon_help_r.height - 2, icon_help_r.width - 5, icon_help_r.height - 5),
                tooltip: "",
                tex_release: icon_help_r,
                tex_press: icon_help_p,
                tex_gui_color: Color.white,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0)))
            {
                EditorApplication.delayCall += () =>
                {
                    List<XGUIDialogListDatas> infos = new List<XGUIDialogListDatas>();

                    infos.Add(new XGUIDialogListDatas($"起始 - 默认", $"<b><color=#{Theme_PrimaryColor}>S</color></b>  -  <b><color=#{Theme_PrimaryColor}>D</color></b>", $"<color=#c1c1c1>从</color>  起始值  <color=#c1c1c1>到</color>  默认值  <color=#c1c1c1>的动画</color>"));
                    infos.Add(new XGUIDialogListDatas($"默认 - 结束", $"<b><color=#{Theme_PrimaryColor}>D</color></b>  -  <b><color=#{Theme_PrimaryColor}>E</color></b>", $"<color=#c1c1c1>从</color>  默认值  <color=#c1c1c1>到</color>  结束值  <color=#c1c1c1>的动画</color>"));
                    infos.Add(new XGUIDialogListDatas($"起始 - 结束", $"<b><color=#{Theme_PrimaryColor}>S</color></b>  -  <b><color=#{Theme_PrimaryColor}>E</color></b>", $"<color=#c1c1c1>从</color>  起始值  <color=#c1c1c1>到</color>  结束值  <color=#c1c1c1>的动画</color>"));
                    infos.Add(new XGUIDialogListDatas($"当前 - 结束", $"<b><color=#{Theme_PrimaryColor}>C</color></b>  -  <b><color=#{Theme_PrimaryColor}>E</color></b>", $"<color=#c1c1c1>从</color>  当前值  <color=#c1c1c1>到</color>  结束值  <color=#c1c1c1>的动画</color>"));

                    XGUI.dialog_listview(
                        datas: infos.ToArray(),
                        type: XGUIDialogType.通知,
                        windowtitle: "XHud - 图元动画时间线编辑器消息",
                        title: "数值模式说明",
                        msg: "以下是数值模式的简码对应的解释",
                        ok: "明白",
                        PrimaryIndex: 0,
                        show_index: false,
                        usemodal: false,
                        themecolor: XHud_Dashboard.Theme_Primary);
                };
            }
            #endregion

            XGUI.layout_space(5);

            // 获取动画类型
            TweenNodeType node_type = (TweenNodeType)ser_type.enumValueIndex;

            // 绘制动画数值控件逻辑
            DrawTweenValueEditor(ResolveTweenValueProperties(node_type, prop_node), value_mode_index);

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 选项
            XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.Theme_Group,
                title: "选项",
                title_text_color: XHud_Dashboard.Theme_Primary,
                title_clipping: XGUI.TryEllipsisClipping(),
                title_size: XGUIFontSize.M,
                title_manual_offset: true,
                title_manual_offset_space: 5,
                absolute_margin: true,
                absolute_padding: true,
                margin: new RectOffset(10, 10, 25, 0),
                padding: new RectOffset(10, 10, 15, 15));

            #region 动画开关
            node.Enabled = DrawLabeledToggle("动画开关", node.Enabled, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, new string[] { "禁用", "启用" }, (b) => { });
            #endregion

            #region 延伸内容
            if (node.Type == TweenNodeType.w_打字机 && node.TweenValueMode == TweenValueMode.当前到结束_C_E)
                node.TextExtended = DrawLabeledToggle("延伸内容", node.TextExtended, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, new string[] { "禁用", "启用" }, (b) => { });
            #endregion

            #region 重置设为起始值
            node.Rewind_Set_Startvalue = DrawLabeledToggle("重置设为起始值", node.Rewind_Set_Startvalue, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, new string[] { "禁用", "启用" }, (b) => { });
            #endregion

            #region 完成设为结束值
            node.Complete_Set_Endvalue = DrawLabeledToggle("完成设为结束值", node.Complete_Set_Endvalue, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, new string[] { "禁用", "启用" }, (b) => { });
            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            XGUI.layout_space(6);

            so.ApplyModifiedProperties();

            if (XGUI.ChangedCheck_End())
            {
                if (dragMode == DragMode.无)
                {
                    if (!IsPreviewing)
                        Undo.RecordObject(target, "Edit Tween Node");
                }
                EditorUtility.SetDirty(target);
                Repaint();
            }
        }
        /// <summary>
        /// 获取参数面板专用的 SerializedObject（带缓存）。
        /// <para/>同一 target、同一 selectedKind、同一 selectedIndex 时复用；
        /// 任一变化则重建并刷新子属性缓存。
        /// </summary>
        /// <returns>缓存的 SerializedObject；target 为空时返回 null</returns>
        private SerializedObject GetParameterSerializedObject()
        {
            if (target == null || target.Equals(null)) return null;

            // ── 判断是否需要重建 SO ──
            bool needRebuildSO = cachedSO == null
                || cachedSO_target != target
                || cachedSO.targetObject == null;

            if (needRebuildSO)
            {
                cachedSO = new SerializedObject(target);
                cachedSO_target = target;
                cachedNodesProp = null;
                cachedSoundsProp = null;
                cachedSOKind = TrackKind.无;
                cachedSOIndex = -1;
            }

            // ── 判断是否需要刷新子属性缓存 ──
            bool needRebuildSubProps = cachedSOKind != selectedKind
                || cachedSOIndex != selectedIndex
                || (selectedKind == TrackKind.Node && cachedNodesProp == null)
                || (selectedKind == TrackKind.Sound && cachedSoundsProp == null);

            if (needRebuildSubProps)
            {
                if (selectedKind == TrackKind.Node)
                    cachedNodesProp = cachedSO.FindProperty("PrimitiveTweenNodes");
                else if (selectedKind == TrackKind.Sound)
                    cachedSoundsProp = cachedSO.FindProperty("PrimitiveTweenSounds");

                cachedSOKind = selectedKind;
                cachedSOIndex = selectedIndex;
            }

            return cachedSO;
        }
    }
}
