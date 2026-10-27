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
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEngine;
    using Object = UnityEngine.Object;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(XHud_Element_Sounder))]
    public class Editor_XHud_Element_Sounder : Editor
    {
        #region 字段 - 组件 / 列表
        /// <summary>
        /// 当前编辑的目标脚本实例。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>在 <c>OnEnable</c> 中由 <c>target</c> 强制转换获得。</description></item>
        /// <item><description>仅用于单选模式下的直接读写操作。</description></item>
        /// </list>
        /// </summary>
        private XHud_Element_Sounder base_script;
        /// <summary>
        /// HUD 管理器引用。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>在 <c>OnEnable</c> 中通过 <c>XHud_Dashboard.HudManagerGet()</c> 获取。</description></item>
        /// <item><description>用于访问音效库等全局数据。</description></item>
        /// </list>
        /// </summary>
        private XHud_Manager hud_manager;
        #endregion

        #region 字段 - 序列化属性
        /// <summary>
        /// 随机音高开关属性
        /// </summary>
        SerializedProperty sp_use_random_pitch;
        /// <summary>
        /// 循环播放开关属性
        /// </summary>
        SerializedProperty sp_is_loop;
        /// <summary>
        /// 音高下限属性
        /// </summary>
        SerializedProperty sp_pitch_min;
        /// <summary>
        /// 音高上限属性
        /// </summary>
        SerializedProperty sp_pitch_max;
        /// <summary>
        /// 调试开关属性
        /// </summary>
        SerializedProperty sp_debug_state;
        /// <summary>
        /// 音量属性
        /// </summary>
        SerializedProperty sp_volume;
        /// <summary>
        /// 音效标识属性
        /// </summary>
        SerializedProperty sp_indicator;
        /// <summary>
        /// 延迟时间属性
        /// </summary>
        SerializedProperty sp_delay_time;
        /// <summary>
        /// 预览快捷键属性
        /// </summary>
        SerializedProperty sp_preview_key;
        /// <summary>
        /// 取消预览快捷键属性
        /// </summary>
        SerializedProperty sp_un_preview_key;
        /// <summary>
        /// 音效名称属性
        /// </summary>
        SerializedProperty sp_sound_name;
        /// <summary>
        /// 关联元素组件属性
        /// </summary>
        SerializedProperty sp_element;
        /// <summary>
        /// 关联开关组件属性
        /// </summary>
        SerializedProperty sp_toggle;
        /// <summary>
        /// 关联滑动条组件属性
        /// </summary>
        SerializedProperty sp_slider;
        /// <summary>
        /// 关联进度条组件属性
        /// </summary>
        SerializedProperty sp_progress;
        /// <summary>
        /// 关联选项组件属性
        /// </summary>
        SerializedProperty sp_option;
        /// <summary>
        /// 关联按钮组件属性
        /// </summary>
        SerializedProperty sp_button;
        /// <summary>
        /// 播放时机属性
        /// </summary>
        SerializedProperty sp_timings;
        #endregion

        #region 字段 - 图标
        /// <summary>
        /// 上一个按钮常态图标
        /// </summary>
        private Texture2D left_arrow_r;
        /// <summary>
        /// 上一个按钮按下图标
        /// </summary>
        private Texture2D left_arrow_p;
        /// <summary>
        /// 下一个按钮常态图标
        /// </summary>
        private Texture2D right_arrow_r;
        /// <summary>
        /// 下一个按钮按下图标
        /// </summary>
        private Texture2D right_arrow_p;
        /// <summary>
        /// 定位按钮常态图标
        /// </summary>
        private Texture2D locate_r;
        /// <summary>
        /// 定位按钮按下图标
        /// </summary>
        private Texture2D locate_p;
        /// <summary>
        /// 预览按钮常态图标
        /// </summary>
        private Texture2D play_r;
        /// <summary>
        /// 预览按钮按下图标
        /// </summary>
        private Texture2D play_p;
        /// <summary>
        /// 添加按钮常态图标
        /// </summary>
        private Texture2D add_r;
        /// <summary>
        /// 添加按钮按下图标
        /// </summary>
        private Texture2D add_p;
        /// <summary>
        /// 标题主图标
        /// </summary>
        private Texture2D icon_main;
        /// <summary>
        /// 批量预览按钮常态图标
        /// </summary>
        private Texture2D play_r_mul;
        /// <summary>
        /// 批量预览按钮按下图标
        /// </summary>
        private Texture2D play_p_mul;
        #endregion

        #region 字段 - 选择弹窗音效文件
        /// <summary>
        /// 当前编辑器命令名称。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>在 <c>OnInspectorGUI</c> 中由 <c>Event.current.commandName</c> 获取。</description></item>
        /// <item><description>用于判断对象选择器状态。</description></item>
        /// </list>
        /// </summary>
        string command_name;
        /// <summary>
        /// 是否已进行过选择。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>防止同一对象被重复加入音效库。</description></item>
        /// </list>
        /// </summary>
        private bool is_selection;
        /// <summary>
        /// 是否处于对象选择器模式。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>点击"添加音效到库"时置为 true。</description></item>
        /// </list>
        /// </summary>
        private bool is_object_selector;
        /// <summary>
        /// 当前选中的音频剪辑。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>作为对象选择器的初始选中项。</description></item>
        /// </list>
        /// </summary>
        private AudioClip selected_clip;
        #endregion

        #region 字段 - 批量模式查看索引
        /// <summary>
        /// 批量模式下当前查看的音效器索引。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>取值范围为 <c>0</c> 到 <c>SelectedObjects.Length - 1</c>。</description></item>
        /// <item><description>通过上一个/下一个按钮循环切换。</description></item>
        /// </list>
        /// </summary>
        private int sounder_statistic_index;
        #endregion

        #region 字段 - 选项文字
        /// <summary>
        /// 随机音高选项文字。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>索引 0：规律。</description></item>
        /// <item><description>索引 1：随机。</description></item>
        /// </list>
        /// </summary>
        string[] stroptions_random = new string[2] { "规律", "随机" };
        /// <summary>
        /// 调试开关选项文字。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>索引 0：关闭。</description></item>
        /// <item><description>索引 1：调试。</description></item>
        /// </list>
        /// </summary>
        string[] stroptions_debug = new string[2] { "关闭", "调试" };
        /// <summary>
        /// 循环播放选项文字。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>索引 0：单次。</description></item>
        /// <item><description>索引 1：循环。</description></item>
        /// </list>
        /// </summary>
        string[] stroptions_cycle = new string[2] { "单次", "循环" };
        #endregion

        #region 字段 - 批量化操作
        /// <summary>
        /// 批量选择脚本数组。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>在 <c>Targets_Get</c> 中根据 <c>targets</c> 填充。</description></item>
        /// <item><description>单选时长度为 1，多选时长度等于选中对象数量。</description></item>
        /// </list>
        /// </summary>
        private XHud_Element_Sounder[] selected_objects;
        #endregion

        #region 生命周期
        private void OnEnable()
        {
            hud_manager = XHud_Dashboard.get_manager();
            base_script = (XHud_Element_Sounder)target;

            #region 获取序列化属性
            sp_use_random_pitch = serializedObject.FindProperty(nameof(XHud_Element_Sounder.use_random_pitch));
            sp_pitch_min = serializedObject.FindProperty(nameof(XHud_Element_Sounder.pitch_min));
            sp_pitch_max = serializedObject.FindProperty(nameof(XHud_Element_Sounder.pitch_max));
            sp_volume = serializedObject.FindProperty(nameof(XHud_Element_Sounder.volume));
            sp_debug_state = serializedObject.FindProperty(nameof(XHud_Element_Sounder.debug_state));
            sp_indicator = serializedObject.FindProperty(nameof(XHud_Element_Sounder.indicator));
            sp_delay_time = serializedObject.FindProperty(nameof(XHud_Element_Sounder.delay_time));
            sp_timings = serializedObject.FindProperty(nameof(XHud_Element_Sounder.timings));
            sp_sound_name = serializedObject.FindProperty(nameof(XHud_Element_Sounder.sound_name));
            sp_is_loop = serializedObject.FindProperty(nameof(XHud_Element_Sounder.is_loop));
            sp_element = serializedObject.FindProperty(nameof(XHud_Element_Sounder.element));
            sp_toggle = serializedObject.FindProperty(nameof(XHud_Element_Sounder.toggle));
            sp_slider = serializedObject.FindProperty(nameof(XHud_Element_Sounder.slider));
            sp_progress = serializedObject.FindProperty(nameof(XHud_Element_Sounder.progress));
            sp_option = serializedObject.FindProperty(nameof(XHud_Element_Sounder.option));
            sp_button = serializedObject.FindProperty(nameof(XHud_Element_Sounder.button));
            sp_preview_key = serializedObject.FindProperty(nameof(XHud_Element_Sounder.preview_key));
            sp_un_preview_key = serializedObject.FindProperty(nameof(XHud_Element_Sounder.un_preview_key));
            #endregion

            #region 获取图标
            icon_main = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_element_sounder/icon_main");
            locate_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_element_sounder/locate_r");
            locate_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_element_sounder/locate_p");
            play_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_element_sounder/play_r");
            play_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_element_sounder/play_p");
            add_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_element_sounder/add_r");
            add_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_element_sounder/add_p");
            left_arrow_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_element_sounder/left_arrow_r");
            left_arrow_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_element_sounder/left_arrow_p");
            right_arrow_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_element_sounder/right_arrow_r");
            right_arrow_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_element_sounder/right_arrow_p");
            play_r_mul = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_element_sounder/play_r_mul");
            play_p_mul = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_element_sounder/play_p_mul");
            #endregion

            Targets_Get();

            #region 获取目标组件
            if (sp_element.objectReferenceValue == null)
            {
                sp_element.objectReferenceValue = base_script.GetComponent<XHud_Module_Element>();
                sp_element.serializedObject.ApplyModifiedProperties();
            }
            if (sp_toggle.objectReferenceValue == null)
            {
                sp_toggle.objectReferenceValue = base_script.GetComponent<XHud_Module_Toggle>();
                sp_toggle.serializedObject.ApplyModifiedProperties();
            }
            if (sp_slider.objectReferenceValue == null)
            {
                sp_slider.objectReferenceValue = base_script.GetComponent<XHud_Module_Slider>();
                sp_slider.serializedObject.ApplyModifiedProperties();
            }
            if (sp_progress.objectReferenceValue == null)
            {
                sp_progress.objectReferenceValue = base_script.GetComponent<XHud_Module_Progress>();
                sp_progress.serializedObject.ApplyModifiedProperties();
            }
            if (sp_option.objectReferenceValue == null)
            {
                sp_option.objectReferenceValue = base_script.GetComponent<XHud_Module_Option>();
                sp_option.serializedObject.ApplyModifiedProperties();
            }
            if (sp_button.objectReferenceValue == null)
            {
                sp_button.objectReferenceValue = base_script.GetComponent<XHud_Module_Button>();
                sp_button.serializedObject.ApplyModifiedProperties();
            }
            #endregion

            #region 如果音效名称是空的检查音效名称是否有效
            if (!Targets_Selected())
            {
                if (string.IsNullOrEmpty(sp_sound_name.stringValue))
                {
                    if (hud_manager.library_sounds != null)
                    {
                        if (!hud_manager.library_sounds.SoundLibrary_IsEmpty())
                        {
                            sp_sound_name.stringValue = hud_manager.library_sounds.SoundLibrary_GetSoundNames()[0];
                            sp_sound_name.serializedObject.ApplyModifiedProperties();
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < selected_objects.Length; i++)
                {
                    if (string.IsNullOrEmpty(selected_objects[i].sound_name))
                    {
                        selected_objects[i].sound_name = hud_manager.library_sounds.SoundLibrary_GetSoundNames()[0];
                    }
                }
            }
            #endregion
        }

        private void OnDisable()
        {
            Preview_HudSound_Stop();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            string hexcol = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.themes_get_color(ThemeColorType.primary), true);

            TextClipping clipping_ellipsis = XGUI.TryEllipsisClipping();

            #region 标题
            XGUI.layout_banner(
             bg_fill: XGUIGroup.实体,
             bg_color: XGUIColor.深空灰,
             bg_height: 30,
             icon: icon_main,
             icon_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
             title_text: "XHud  -  音效器",
             title_anchor: TextAnchor.MiddleLeft,
             title_style: FontStyle.Normal,
             title_color: Color.white,
             title_size: XGUIFontSize.B,
             title_clipping: clipping_ellipsis,
             bg_margin: new RectOffset(0, 0, 5, 5));
            #endregion

            XGUI.layout_space(5);

            #region 快捷功能
            XGUI.layout_group_start(
                type: XGUIContainerType.Horizontal,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "快捷功能",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(15, 15, 20, 15));
            #region 预览当前声音
            if (Application.isPlaying)
                XGUI.SetEnabled(false);
            else
                XGUI.SetEnabled(true);
            if (XGUI.layout_button(
                tooltip: "预览当前声音",
                tex_release: Targets_Selected() ? play_r_mul : play_r,
                tex_press: Targets_Selected() ? play_p_mul : play_p,
                tex_gui_color: Color.white,
                border: new RectOffset(0, 0, 0, 0),
                width: 14,
                height: 14))
            {
                Preview_HudSound_Play();
            }
            XGUI.SetEnabled(true);
            #endregion
            GUILayout.FlexibleSpace();
            #region 定位到音效资源
            if (XGUI.layout_button(
                tooltip: "定位到音效资源",
                tex_release: locate_r,
                tex_press: locate_p,
                tex_gui_color: Color.white,
                border: new RectOffset(0, 0, 0, 0),
                width: 14,
                height: 14))
            {
                if (Application.isPlaying)
                {
                    Debug.Log("程序正在运行，无法在运行期间执行此功能！");
                    return;
                }
                Editor_XHud_MenuItemsAction_OpenLibrary.open_sound();
                if (hud_manager.library_sounds == null)
                    return;
                hud_manager.library_sounds.SoundLibrary_Location(sp_sound_name.stringValue);
            }
            #endregion
            GUILayout.FlexibleSpace();
            #region 添加音效到库
            if (XGUI.layout_button(
                tooltip: "添加音效到库",
                tex_release: add_r,
                tex_press: add_p,
                tex_gui_color: Color.white,
                border: new RectOffset(0, 0, 0, 0),
                width: 14,
                height: 14))
            {
                if (Application.isPlaying)
                {
                    Debug.Log("程序正在运行，无法在运行期间执行此功能！");
                    return;
                }
                is_object_selector = true;
                EditorGUIUtility.ShowObjectPicker<AudioClip>(selected_clip, false, "", 0);
            }
            #endregion
            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            #endregion

            #region 选项
            base_script.fold_option = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "选项",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: base_script.fold_option);
            if (!base_script.fold_option)
            {
                DrawToggle("状态调试", sp_debug_state, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, (b) => { }, stroptions_debug);
                DrawToggle("随机音高", sp_use_random_pitch, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, (b) => { }, stroptions_random);
                DrawToggle("循环播放", sp_is_loop, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, (b) => { }, stroptions_cycle);
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 统计
            base_script.fold_state = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "统计",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: base_script.fold_state);
            if (!base_script.fold_state)
            {
                if (hud_manager.library_sounds == null)
                {
                    XGUI.layout_label(
                        text: "暂未在管理器中配置音效库",
                        size: XGUIFontSize.M,
                        text_color: Color.gray,
                        padding: new RectOffset(0, 0, 0, -10),
                        margin: new RectOffset(0, 0, 15, 35),
                        clipping: TextClipping.Clip,
                        font_style: FontStyle.Normal,
                        anchor: TextAnchor.MiddleCenter);
                }
                else
                {
                    if (hud_manager.library_sounds.library_sounds.Count <= 0)
                    {
                        XGUI.layout_label(
                           text: "暂未在音效库中找到任何音效资源",
                           size: XGUIFontSize.M,
                           text_color: Color.gray,
                           padding: new RectOffset(0, 0, 0, -10),
                           margin: new RectOffset(0, 0, 15, 35),
                           clipping: TextClipping.Clip,
                           font_style: FontStyle.Normal,
                           anchor: TextAnchor.MiddleCenter);
                    }
                    else
                    {
                        if (!Targets_Selected())
                        {
                            XHud_LibraryArg_Sound soundInfo = hud_manager.library_sounds.SoundLibrary_GetSoundInfo(sp_sound_name.stringValue);
                            if (soundInfo != null)
                            {
                                #region 名称
                                XGUI.layout_state_displayer_text(
                                    title: "名称",
                                    title_size: XGUIFontSize.M,
                                    subtitle: soundInfo.name,
                                    subtitle_size: XGUIFontSize.M,
                                    subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                    margin: new RectOffset(5, 5, 0, 5));
                                #endregion

                                #region 声道
                                XGUI.layout_state_displayer_text(
                                    title: "声道",
                                    title_size: XGUIFontSize.M,
                                    subtitle: soundInfo.channel == 1 ? "单声道" : "立体声",
                                    subtitle_size: XGUIFontSize.M,
                                    subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                    margin: new RectOffset(5, 5, 0, 5));
                                #endregion

                                #region 频率
                                XGUI.layout_state_displayer_text(
                                    title: "频率",
                                    title_size: XGUIFontSize.M,
                                    subtitle: soundInfo.frequency.ToString() + "hz",
                                    subtitle_size: XGUIFontSize.M,
                                    subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                    margin: new RectOffset(5, 5, 0, 5));
                                #endregion

                                #region 时长
                                XGUI.layout_state_displayer_text(
                                    title: "时长",
                                    title_size: XGUIFontSize.M,
                                    subtitle: soundInfo.length.ToString("F2") + " 秒",
                                    subtitle_size: XGUIFontSize.M,
                                    subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                    margin: new RectOffset(5, 5, 0, 5));
                                #endregion
                            }
                        }
                        else
                        {
                            #region 批量控件
                            XGUI.layout_group_start(
                                type: XGUIContainerType.Horizontal,
                                absolute_margin: true,
                                absolute_padding: true,
                                margin: new RectOffset(0, 0, 0, 0),
                                padding: new RectOffset(0, 0, 10, 15),
                                foldout: base_script.fold_option);
                            if (XGUI.layout_button(
                                text: $"{selected_objects[sounder_statistic_index].name} ( {selected_objects[sounder_statistic_index].indicator} )",
                                tooltip: "",
                                bg_fill: XGUIGroup.透明,
                                bg_color: XGUIColor.亮白,
                                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                button_text_color: Color.white,
                                press_fill: XGUIGroup.透明,
                                press_color: XGUIColor.深空灰,
                                press_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                font_size: XGUIFontSize.B,
                                anchor: TextAnchor.MiddleLeft,
                                margin: new RectOffset(0, 0, 0, 0),
                                padding: new RectOffset(0, 0, 0, 0),
                                button_text_style: FontStyle.Bold))
                            {
                                EditorGUIUtility.PingObject(selected_objects[sounder_statistic_index]);
                            }
                            if (XGUI.layout_button(
                                tooltip: "",
                                tex_release: left_arrow_r,
                                tex_press: left_arrow_p,
                                tex_gui_color: Color.white,
                                border: new RectOffset(0, 0, 0, 0),
                                margin: new RectOffset(20, 0, 5, 0),
                                width: left_arrow_r.width,
                                height: left_arrow_r.height))
                            {
                                if (sounder_statistic_index <= 0)
                                {
                                    sounder_statistic_index = selected_objects.Length - 1;
                                }
                                else
                                {
                                    sounder_statistic_index--;
                                }
                                EditorGUIUtility.PingObject(selected_objects[sounder_statistic_index]);
                            }
                            if (XGUI.layout_button(
                                tooltip: "",
                                tex_release: right_arrow_r,
                                tex_press: right_arrow_p,
                                tex_gui_color: Color.white,
                                border: new RectOffset(0, 0, 0, 0),
                                margin: new RectOffset(20, 0, 5, 0),
                                width: right_arrow_r.width,
                                height: right_arrow_r.height))
                            {
                                if (sounder_statistic_index >= selected_objects.Length - 1)
                                {
                                    sounder_statistic_index = 0;
                                }
                                else
                                {
                                    sounder_statistic_index++;
                                }
                                EditorGUIUtility.PingObject(selected_objects[sounder_statistic_index]);
                            }
                            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
                            #endregion

                            XHud_LibraryArg_Sound soundInfo = hud_manager.library_sounds.SoundLibrary_GetSoundInfo(selected_objects[sounder_statistic_index].sound_name);

                            #region 名称
                            XGUI.layout_state_displayer_text(
                                title: "名称",
                                title_size: XGUIFontSize.M,
                                subtitle: soundInfo.name,
                                subtitle_size: XGUIFontSize.M,
                                subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                margin: new RectOffset(5, 5, 0, 5));
                            #endregion

                            #region 声道
                            XGUI.layout_state_displayer_text(
                                title: "声道",
                                title_size: XGUIFontSize.M,
                                subtitle: soundInfo.channel == 1 ? "单声道" : "立体声",
                                subtitle_size: XGUIFontSize.M,
                                subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                margin: new RectOffset(5, 5, 0, 5));
                            #endregion

                            #region 频率
                            XGUI.layout_state_displayer_text(
                                title: "频率",
                                title_size: XGUIFontSize.M,
                                subtitle: soundInfo.frequency.ToString() + "hz",
                                subtitle_size: XGUIFontSize.M,
                                subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                margin: new RectOffset(5, 5, 0, 5));
                            #endregion

                            #region 时长
                            XGUI.layout_state_displayer_text(
                                title: "时长",
                                title_size: XGUIFontSize.M,
                                subtitle: soundInfo.length.ToString("F2") + " 秒",
                                subtitle_size: XGUIFontSize.M,
                                subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                                margin: new RectOffset(5, 5, 0, 5));
                            #endregion
                        }
                    }
                }
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 参数
            base_script.fold_param = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "参数",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: base_script.fold_param);
            if (!base_script.fold_param)
            {
                #region 播放时机
                string[] str_tims = new string[1] { "无" };

                #region Element
                if (sp_element.objectReferenceValue != null)
                {
                    str_tims = new string[4] { "元素进入时", "元素进入后", "元素退出时", "自定义" };
                }
                #endregion

                #region Slider
                if (sp_slider.objectReferenceValue != null)
                {
                    str_tims = new string[] { "无", "按下滑动条", "松开滑动条", "滑动条数值改变" };
                }
                #endregion

                #region Toggle
                if (sp_toggle.objectReferenceValue != null)
                {
                    str_tims = new string[] { "无", "开关打开时", "开关关闭时", "开关按下时", "开关抬起时", "开关变化时" };
                }
                #endregion

                #region Progress
                if (sp_progress.objectReferenceValue != null)
                {
                    str_tims = new string[] { "无", "进度开始时", "进度变化时", "进度结束时" };
                }
                #endregion

                #region Button
                if (sp_button.objectReferenceValue != null)
                {
                    str_tims = new string[] { "无", "鼠标进入", "鼠标退出", "鼠标按下", "鼠标松开", "鼠标长按", "鼠标点击", "鼠标选中", "鼠标取消选中" };
                }
                #endregion

                #region Option
                if (sp_option.objectReferenceValue != null)
                {
                    str_tims = new string[] { "无", "点击选项", "光标移动开始", "光标移动结束", "光标位置改变" };
                }
                #endregion

                sp_timings.stringValue = XGUI.layout_string_popup(
                    title: "播放时机",
                    title_width: 100,
                    title_size: XGUIFontSize.M,
                    title_anchor: TextAnchor.MiddleLeft,
                    prop: sp_timings,
                    options: str_tims,
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

                    });
                sp_timings.serializedObject.ApplyModifiedProperties();
                #endregion

                #region 音效列表
                if (hud_manager.library_sounds != null)
                {
                    string[] collist = hud_manager.library_sounds.SoundLibrary_GetSoundNames();
                    sp_sound_name.stringValue = XGUI.layout_string_popup(
                    title: "音效列表",
                    title_width: 100,
                    title_size: XGUIFontSize.M,
                    title_anchor: TextAnchor.MiddleLeft,
                    prop: sp_sound_name,
                    options: collist,
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

                    });
                    sp_sound_name.serializedObject.ApplyModifiedProperties();
                }
                #endregion

                XGUI.layout_seperator(
                    thickness: 1,
                    color: XHud_Dashboard.themes_get_color(ThemeColorType.sepline),
                    margin: new RectOffset(15, 15, 15, 15));

                #region 音效标识
                XGUI.layout_property_field(
                    title: "音效标识",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: sp_indicator,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                #region 音效音量
                XGUI.layout_property_field(
                    title: "音效音量",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: sp_volume,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                if (sp_use_random_pitch.intValue == 1)
                {
                    #region 最小音高
                    XGUI.layout_property_field(
                    title: "最小音高",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: sp_pitch_min,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion

                    #region 最大音高
                    XGUI.layout_property_field(
                        title: "最大音高",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: sp_pitch_max,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion

                    #region 音高范围
                    xgui_minmax_value minmax = XGUI.layout_slider_min_max(
                     title: "音高范围",
                     ref_min: ref base_script.pitch_min,
                     ref_max: ref base_script.pitch_max,
                     min_limite: -2f,
                     max_limite: 3f,
                     title_width: 100,
                     title_size: XGUIFontSize.M,
                     title_anchor: TextAnchor.MiddleLeft,
                     min_field_color: Color.white,
                     max_field_color: Color.white,
                     shrink_text_color: Color.white * 0.65f,
                     control_limite: 210,
                     prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion
                }

                #region 延迟时间
                XGUI.layout_property_field(
                    title: "延迟时间",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: sp_delay_time,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                #region 开始预览
                XGUI.layout_property_field(
                    title: "开始预览",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: sp_preview_key,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                #region 停止预览
                XGUI.layout_property_field(
                    title: "停止预览",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: sp_un_preview_key,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            ///------------选择音效
            command_name = Event.current.commandName;

            if (command_name == "ObjectSelectorUpdated")
            {
                UpdateSoundParams();
                is_selection = false;
            }
            else if (command_name == "ObjectSelectorClosed")
            {
                if (!is_object_selector)
                    return;
                Object obj = EditorGUIUtility.GetObjectPickerObject();
                if (obj != null)
                {
                    if (!is_selection)
                    {
                        is_object_selector = false;
                        is_selection = true;
                        if (hud_manager.library_sounds.library_sounds == null)
                            hud_manager.library_sounds.library_sounds = new List<XHud_LibraryArg_Sound>();
                        AudioClip clip = obj as AudioClip;
                        string clip_format = Path.GetExtension(AssetDatabase.GetAssetPath(clip)).ToLower();
                        hud_manager.library_sounds.library_sounds.Add(new XHud_LibraryArg_Sound(clip.name, clip, clip_format));
                        UpdateSoundParams();
                    }
                }
            }

            #region 弹出菜单
            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                // 创建右键菜单
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Q (折叠编组)"), false, () =>
                {
                    base_script.GroupFold(true);
                });
                menu.AddItem(new GUIContent("W (展开编组)"), false, () =>
                {
                    base_script.GroupFold(false);
                });
                menu.ShowAsContext();

                Event.current.Use();
            }
            #endregion

            #region 源脚本
            base_script.fold_based = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "源脚本",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: base_script.fold_based);
            if (!base_script.fold_based)
            {
                DrawDefaultInspector();
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            serializedObject.ApplyModifiedProperties();
        }
        #endregion

        #region 工具 - 音效库更新
        /// <summary>
        /// 更新音效库的每一项音效信息。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>遍历音效库并刷新声道、频率、时长。</description></item>
        /// <item><description>在 <c>HudManager</c> 或音效库为 null 时直接返回。</description></item>
        /// </list>
        /// </summary>
        private void UpdateSoundParams()
        {
            if (hud_manager == null)
                return;
            if (hud_manager.library_sounds == null)
                return;
            if (hud_manager.library_sounds.library_sounds != null && hud_manager.library_sounds.library_sounds.Count > 0)
            {
                for (int i = 0; i < hud_manager.library_sounds.library_sounds.Count; i++)
                {
                    if (hud_manager.library_sounds.library_sounds[i].clip != null)
                    {
                        hud_manager.library_sounds.library_sounds[i].channel = hud_manager.library_sounds.library_sounds[i].clip.channels;
                        hud_manager.library_sounds.library_sounds[i].frequency = hud_manager.library_sounds.library_sounds[i].clip.frequency;
                        hud_manager.library_sounds.library_sounds[i].length = hud_manager.library_sounds.library_sounds[i].clip.length;
                    }
                }
            }
        }
        #endregion

        #region 工具 - 音效预览
        /// <summary>
        /// 预览音效（统一入口）。
        /// <para/>
        /// 本方法只负责"决定要预览哪些 Sounder"，具体参数计算与入队交给 <see cref="EnqueueSounderSound"/>。
        /// <para/>
        /// 【多选行为】多选时逐个 Sounder 各自取 SoundName / DelayTime / Volume / Pitch，
        /// 不再共用当前查看对象的 SerializedProperty 值（修正原实现的隐藏 bug）。
        /// </summary>
        private void Preview_HudSound_Play()
        {
            if (hud_manager.library_sounds == null)
            {
                EditorApplication.delayCall += () =>
                {
                    string res = XGUI.dialog(
                        type: XGUIDialogType.警告,
                        windowtitle: "XHud - 音效器消息",
                        title: "未指定音效库",
                        msg: "未在 XHudManager 中配置音效库！请先前往 XHud管理器 指定一个音效库！",
                        ok: "前往",
                        cancel: "明白",
                        PrimaryIndex: 0,
                        usemodal: true,
                        themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        on_selected: (d) =>
                        {
                            if (d == "前往")
                            {
                                Transform man = FindFirstObjectByType<XHud_Manager>().transform;
                                EditorGUIUtility.PingObject(man);
                            }
                        });
                };
            }
            else
            {
                if (Targets_Selected())
                {
                    for (int i = 0; i < selected_objects.Length; i++)
                    {
                        EnqueueSounderSound(selected_objects[i]);
                    }
                }
                else
                {
                    EnqueueSounderSound(base_script);
                }
            }
        }
        /// <summary>
        /// 把单个 Sounder 的预览音效"算好参数"后交给 helper 入队。
        /// <para/>
        /// 【参数来源】
        /// <list type="bullet">
        /// <item><description>clip：从音效库按 SoundName 取；取不到则跳过。</description></item>
        /// <item><description>delay：sounder.DelayTime 原值（不乘任何倍增系数）。</description></item>
        /// <item><description>volume：sounder.Volume × HudManager.Volume（对齐运行时 DelayPlay 算法）。</description></item>
        /// <item><description>mute：HudManager.VolumeMute（对齐运行时）。</description></item>
        /// <item><description>pitch：sounder.UseRandomPitch 为 true 时在 [Pitch_Min, Pitch_Max] 随机，否则 1。</description></item>
        /// </list>
        /// </summary>
        private void EnqueueSounderSound(XHud_Element_Sounder sounder)
        {
            if (sounder == null) return;
            if (string.IsNullOrEmpty(sounder.sound_name)) return;
            if (hud_manager == null || hud_manager.library_sounds == null) return;
            AudioClip clip = hud_manager.library_sounds.SoundLibrary_GetSound(sounder.sound_name);
            if (clip == null) return;
            // ── 算好参数 ──
            float delay = Mathf.Max(0f, sounder.delay_time);
            float masterVol = (hud_manager != null) ? hud_manager.global_volume * 0.01f : 1f;
            bool masterMute = (hud_manager != null) && hud_manager.global_volume_mute;
            float volume = masterVol * Mathf.Clamp01(sounder.volume);
            bool mute = masterMute;
            bool useRandomPitch = sounder.use_random_pitch;
            float pMin = sounder.pitch_min;
            float pMax = sounder.pitch_max;
            XGUI.EditorPlayer_Sound.Enqueue(
                clip, delay, volume, mute, useRandomPitch, pMin, pMax);
        }
        /// <summary>
        /// 停止所有音效预览。
        /// <para/>
        /// 委托给 <see cref="XGUI_Player_Sound.Stop()"/>，
        /// 语义完全一致：待触发全丢 + 正在播立即掐断并销毁。
        /// </summary>
        private void Preview_HudSound_Stop()
        {
            XGUI.EditorPlayer_Sound.Stop();
        }
        #endregion

        #region 工具 - 批量化操作
        /// <summary>
        /// 获取所有批量脚本目标。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>多选时创建与 <c>targets</c> 等长的数组并逐个填充。</description></item>
        /// <item><description>单选时创建长度为 1 的数组并填充当前目标。</description></item>
        /// </list>
        /// </summary>
        private void Targets_Get()
        {
            if (targets.Length > 1)
            {
                selected_objects = new XHud_Element_Sounder[targets.Length];
                for (int i = 0; i < selected_objects.Length; i++)
                {
                    var t = targets[i];
                    selected_objects[i] = (XHud_Element_Sounder)t;
                }
            }
            else
            {
                selected_objects = new XHud_Element_Sounder[targets.Length];
                selected_objects[0] = (XHud_Element_Sounder)target;
            }
        }
        /// <summary>
        /// 判断是否是多选状态。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>当 <c>SelectedObjects</c> 为 null 时返回 false。</description></item>
        /// <item><description>当 <c>SelectedObjects.Length > 1</c> 时返回 true。</description></item>
        /// </list>
        /// </summary>
        /// <returns>是否为多选状态。</returns>
        private bool Targets_Selected()
        {
            if (selected_objects == null)
                return false;
            if (selected_objects.Length > 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 绘制控件
        /// <summary>
        /// 通用方法：绘制开关。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>封装 <c>XGUI.layout_toggle</c> 调用。</description></item>
        /// <item><description>提供标题、宽度、样式、颜色、选项等参数。</description></item>
        /// </list>
        /// </summary>
        private void DrawToggle(string title, SerializedProperty prop, float width, XGUIToggleStyle style = XGUIToggleStyle.实体, Color color_bg_on = default, Color color_bg_off = default, Color color_on = default, Color color_off = default, Action<bool> act_on_changed = null, string[] options = null)
        {
            XGUI.layout_toggle(
                title: title,
                title_size: XGUIFontSize.M,
                title_font_style: FontStyle.Normal,
                title_padding: new RectOffset(5, 10, 0, 0),
                title_width: width,
                prop: prop,
                tog_style: style,
                tog_padding: new RectOffset(0, 9, 0, 0),
                tog_margin: new RectOffset(0, 0, 0, 5),
                tog_mixed_options: options,
                tog_mixed_text_size: XGUIFontSize.M,
                tog_mixed_text_color: Color.black,
                tog_mixed_text_padding: new RectOffset(10, 10, 0, 0),
                tog_mixed_text_anchor: TextAnchor.MiddleCenter,
                tog_mixed_font_style: FontStyle.Normal,
                tog_bg_off_color: color_bg_off,
                tog_bg_on_color: color_bg_on,
                tog_handler_off_color: color_off,
                tog_handler_on_color: color_on,
                tog_mixed_bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                act_on_changed: act_on_changed);
        }
        #endregion
    }
}