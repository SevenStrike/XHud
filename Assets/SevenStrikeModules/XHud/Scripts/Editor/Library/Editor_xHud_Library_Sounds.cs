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
    using UnityEditorInternal;
    using UnityEngine;
    using Object = UnityEngine.Object;
    using Random = UnityEngine.Random;

    [CustomEditor(typeof(XHud_Library_Sounds))]
    public class Editor_XHud_Library_Sounds : Editor
    {
        #region 字段 - 组件 / 列表
        /// <summary>
        /// 目标脚本实例。
        /// <para/>
        /// 在 <see cref="OnEnable"/> 中缓存。
        /// </summary>
        private XHud_Library_Sounds base_script;
        /// <summary>
        /// 音效列表（可重排列表）。
        /// <para/>
        /// 用于驱动 ReorderableList 的绘制与交互。
        /// </summary>
        public ReorderableList sound_info_list;
        #endregion

        #region 字段 - 序列化属性

        /// <summary>
        /// 音效信息列表滚动位置属性
        /// </summary>
        private SerializedProperty sp_sound_Info_list_original_scroller;
        /// <summary>
        /// 列表项高度属性
        /// </summary>
        private SerializedProperty sp_item_height;
        /// <summary>
        /// 可视区域显示元素数量属性
        /// </summary>
        private SerializedProperty sp_visible_item_count;
        /// <summary>
        /// 音效库列表属性
        /// </summary>
        private SerializedProperty sp_library_sounds;
        /// <summary>
        /// 随机音高开关属性
        /// </summary>
        private SerializedProperty sp_use_random_pitch;
        /// <summary>
        /// 音高下限属性
        /// </summary>
        private SerializedProperty sp_pitch_min;
        /// <summary>
        /// 音高上限属性
        /// </summary>
        private SerializedProperty sp_pitch_max;
        /// <summary>
        /// 选中项索引属性
        /// </summary>
        private SerializedProperty sp_selected_index;
        /// <summary>
        /// 定位选中项索引属性
        /// </summary>
        private SerializedProperty sp_location_selected_index;
        /// <summary>
        /// 筛选高亮字符串属性
        /// </summary>
        private SerializedProperty sp_highlight;
        /// <summary>
        /// 查找字符串属性
        /// </summary>
        private SerializedProperty sp_find;
        /// <summary>
        /// 库名称属性
        /// </summary>
        private SerializedProperty sp_library_name;
        #endregion

        #region 字段 - 图标
        /// <summary>
        /// 标题 / 按钮 / 图标资源缓存。
        /// <para/>
        /// 统一在 <see cref="OnEnable"/> 里加载。
        /// </summary>
        private Texture2D scan_p, scan_r, stoppreview_p, stoppreview_r, clear_p, clear_r, create_p, create_r, delete_p, delete_r;
        #endregion

        #region 字段 - 参数
        /// <summary>
        /// 单行高度缓存。
        /// </summary>
        private float line_height;
        /// <summary>
        /// 当前选中的音频剪辑。
        /// <para/>
        /// 用于对象选择器添加音效时作为初始对象。
        /// </summary>
        private AudioClip SelectedClip;
        /// <summary>
        /// 屏蔽颜色。
        /// <para/>
        /// 用于在过滤模式下叠加在非匹配项上的半透明遮罩。
        /// </summary>
        private Color blocked_col;
        /// <summary>
        /// 屏蔽颜色透明度。
        /// </summary>
        private float blocked_alp = 0.45f;
        #endregion

        #region 字段 - 预览 / 选择器
        /// <summary>
        /// 当前 Unity 命令名称。
        /// <para/>
        /// 用于检测对象选择器状态变化。
        /// </summary>
        string command_name;
        /// <summary>
        /// 是否已经从对象选择器选中过对象。
        /// </summary>
        private bool select_object;
        /// <summary>
        /// 是否处于对象选择器打开状态。
        /// </summary>
        private bool is_object_selector;
        /// <summary>
        /// 对象选择器是否发生更新。
        /// </summary>
        private bool is_object_selector_updated;
        #endregion

        #region 字段 - 绘制缓存
        /// <summary>
        /// 绘制元素矩形。
        /// </summary>
        private Rect drawelement_rect;
        /// <summary>
        /// 滚动视图矩形。
        /// </summary>
        private Rect scrollview_rect;
        /// <summary>
        /// 列表项矩形。
        /// </summary>
        private Rect item_rect;
        /// <summary>
        /// 选中背景色。
        /// </summary>
        private Color SelectedBg = new Color(0, 0, 0, 0.2f);
        #endregion

        #region 字段 - 批量化操作
        /// <summary>
        /// 当前 Inspector 选中的所有目标组件缓存。
        /// </summary>
        XHud_Library_Sounds[] selected_objects;
        #endregion

        #region 工具 - 批量化操作
        /// <summary>
        /// 缓存当前 Inspector 选中的所有目标组件到 <see cref="selected_objects"/>。
        /// <para/>
        /// 多选时逐个转换，单选时只填充一个元素。
        /// </summary>
        private void Targets_Get()
        {
            if (targets.Length > 1)
            {
                selected_objects = new XHud_Library_Sounds[targets.Length];
                for (int i = 0; i < selected_objects.Length; i++)
                {
                    var t = targets[i];
                    selected_objects[i] = (XHud_Library_Sounds)t;
                }
            }
            else
            {
                selected_objects = new XHud_Library_Sounds[targets.Length];
                selected_objects[0] = (XHud_Library_Sounds)target;
            }
        }
        /// <summary>
        /// 判断当前是否处于多选状态。
        /// </summary>
        /// <returns>true 表示选中了多个组件</returns>
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

        #region 生命周期
        /// <summary>
        /// Unity 启用回调。
        /// <para/>
        /// 执行流程：
        /// <list type="number">
        /// <item><description>缓存目标脚本实例；</description></item>
        /// <item><description>获取全部序列化属性；</description></item>
        /// <item><description>缓存单行高度；</description></item>
        /// <item><description>缓存所有选中组件；</description></item>
        /// <item><description>初始化列表项高度与可见数量；</description></item>
        /// <item><description>加载全部图标资源；</description></item>
        /// <item><description>构造 ReorderableList；</description></item>
        /// <item><description>注册播放模式切换事件；</description></item>
        /// <item><description>刷新音效参数。</description></item>
        /// </list>
        /// </summary>
        private void OnEnable()
        {
            base_script = (XHud_Library_Sounds)target;

            #region 获取序列化属性
            sp_library_sounds = serializedObject.FindProperty(nameof(XHud_Library_Sounds.library_sounds));
            sp_use_random_pitch = serializedObject.FindProperty(nameof(XHud_Library_Sounds.use_random_pitch));
            sp_pitch_min = serializedObject.FindProperty(nameof(XHud_Library_Sounds.pitch_min));
            sp_pitch_max = serializedObject.FindProperty(nameof(XHud_Library_Sounds.pitch_max));
            sp_library_name = serializedObject.FindProperty(nameof(XHud_Library_Sounds.library_name));
            sp_selected_index = serializedObject.FindProperty(nameof(XHud_Library_Sounds.selected_index));
            sp_highlight = serializedObject.FindProperty(nameof(XHud_Library_Sounds.highlight));
            sp_location_selected_index = serializedObject.FindProperty(nameof(XHud_Library_Sounds.location_selected_index));
            sp_sound_Info_list_original_scroller = serializedObject.FindProperty(nameof(XHud_Library_Sounds.sound_Info_list_original_scroller));
            sp_item_height = serializedObject.FindProperty(nameof(XHud_Library_Sounds.item_height));
            sp_visible_item_count = serializedObject.FindProperty(nameof(XHud_Library_Sounds.visible_item_count));
            sp_find = serializedObject.FindProperty(nameof(XHud_Library_Sounds.find));
            #endregion

            line_height = EditorGUIUtility.singleLineHeight;

            Targets_Get();

            //设定列表项高度值
            sp_item_height.floatValue = 50;
            sp_item_height.serializedObject.ApplyModifiedProperties();

            sp_visible_item_count.intValue = 8;
            sp_visible_item_count.serializedObject.ApplyModifiedProperties();

            #region 获取图标
            scan_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_sound/scan_p");
            scan_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_sound/scan_r");
            stoppreview_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_sound/stoppreview_p");
            stoppreview_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_sound/stoppreview_r");
            clear_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_sound/clear_p");
            clear_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_sound/clear_r");
            create_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_sound/create_p");
            create_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_sound/create_r");
            delete_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_sound/delete_p");
            delete_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_sound/delete_r");
            #endregion

            blocked_col = new Color(0, 0, 0, blocked_alp);

            Vector2 ButtonSize = new Vector2(16, 16);

            #region ReorderableList
            sound_info_list = new ReorderableList(serializedObject, sp_library_sounds, true, true, true, true);
            sound_info_list.drawElementCallback = SoundInfoList_Original_DrawElementCallback;
            #endregion

            EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;
            UpdateSoundParams();
        }
        /// <summary>
        /// Unity 禁用回调。
        /// <para/>
        /// 清理运行时状态：
        /// <list type="bullet">
        /// <item><description>取消播放模式切换事件；</description></item>
        /// <item><description>重置滚动位置；</description></item>
        /// <item><description>清空选中索引与定位索引；</description></item>
        /// <item><description>清空查找与过滤字符串；</description></item>
        /// <item><description>回收所有预览音效。</description></item>
        /// </list>
        /// </summary>
        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= EditorApplication_playModeStateChanged;

            sp_sound_Info_list_original_scroller.vector2Value = Vector2.zero;
            sp_sound_Info_list_original_scroller.serializedObject.ApplyModifiedProperties();

            sp_location_selected_index.intValue = -1;
            sp_location_selected_index.serializedObject.ApplyModifiedProperties();

            sp_selected_index.intValue = -1;
            sp_selected_index.serializedObject.ApplyModifiedProperties();

            sp_find.stringValue = string.Empty;
            sp_find.serializedObject.ApplyModifiedProperties();

            sp_highlight.stringValue = string.Empty;
            sp_highlight.serializedObject.ApplyModifiedProperties();

            Preview_Sound_Stop();
        }
        /// <summary>
        /// 绘制 Inspector 主入口。
        /// <para/>
        /// 按自上而下的顺序绘制所有面板：
        /// 标题 → 参数（含随机音高） → 快捷功能 → 音效列表 → 拖放操作。
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            TextClipping clipping_ellipsis = XGUI.TryEllipsisClipping();

            #region 标题
            XGUI.layout_banner(
                bg_fill: XGUIGroup.实体,
                bg_color: XGUIColor.深空灰,
                bg_height: 30,
                title_text: "XHud  -  音效库",
                title_anchor: TextAnchor.MiddleLeft,
                title_style: FontStyle.Normal,
                title_color: Color.white,
                title_size: XGUIFontSize.B,
                title_clipping: clipping_ellipsis,
                title_offset: new Vector2(-10, 0),
                bg_margin: new RectOffset(0, 0, 5, 5));
            #endregion

            XGUI.layout_space(5);

            #region 参数区
            XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIGroup.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                    title: "参数",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15));

            #region 库名称
            XGUI.layout_property_field(
                title: "库名称",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 90,
                prop: sp_library_name,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 过滤（包含）
            XGUI.ChangedCheck_Start();
            XGUI.layout_property_field(
                title: "过滤（包含）",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 90,
                prop: sp_highlight,
                prop_margin: new RectOffset(0, 0, 5, 0));
            if (XGUI.ChangedCheck_End())
            {
                sp_location_selected_index.intValue = -1;
                sp_location_selected_index.serializedObject.ApplyModifiedProperties();
            }
            #endregion

            #region 查找（精确）
            XGUI.ChangedCheck_Start();
            XGUI.layout_property_field(
                title: "查找（精确）",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 90,
                prop: sp_find,
                prop_margin: new RectOffset(0, 0, 5, 0));
            if (XGUI.ChangedCheck_End())
            {
                if (!string.IsNullOrEmpty(sp_find.stringValue))
                    base_script.SoundLibrary_Location_Find(sp_find.stringValue);
                else
                {
                    sp_selected_index.intValue = -1;
                    sp_selected_index.serializedObject.ApplyModifiedProperties();

                    sp_location_selected_index.intValue = -1;
                    sp_location_selected_index.serializedObject.ApplyModifiedProperties();
                }
            }
            #endregion

            XGUI.layout_seperator(
                thickness: 1,
                color: XHud_Dashboard.themes_get_color(ThemeColorType.sepline),
                margin: new RectOffset(15, 15, 20, 15));

            #region 随机音高
            DrawToggle("随机音高", sp_use_random_pitch, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, (b) => { });
            #endregion

            if (sp_use_random_pitch.boolValue)
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
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 快捷功能
            if (string.IsNullOrEmpty(sp_highlight.stringValue))
            {
                if (!Application.isPlaying)
                {
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

                    #region 扫描取所有音效参数
                    if (XGUI.layout_button(
                        tooltip: "扫描取所有音效参数",
                        tex_release: scan_r,
                        tex_press: scan_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: 14,
                        height: 14))
                    {
                        for (int i = 0; i < sp_library_sounds.arraySize; i++)
                        {
                            SerializedProperty item = sp_library_sounds.GetArrayElementAtIndex(i);
                            SerializedProperty x_Clip = item.FindPropertyRelative(nameof(XHud_LibraryArg_Sound.clip));
                            SerializedProperty x_Frequency = item.FindPropertyRelative(nameof(XHud_LibraryArg_Sound.frequency));
                            SerializedProperty x_Channel = item.FindPropertyRelative(nameof(XHud_LibraryArg_Sound.channel));
                            SerializedProperty x_Length = item.FindPropertyRelative(nameof(XHud_LibraryArg_Sound.length));
                            SerializedProperty x_Format = item.FindPropertyRelative(nameof(XHud_LibraryArg_Sound.format));

                            if (x_Clip.objectReferenceValue != null)
                            {
                                AudioClip ac = (AudioClip)x_Clip.objectReferenceValue;

                                x_Channel.intValue = ac.channels;
                                x_Frequency.intValue = ac.frequency;
                                x_Length.floatValue = ac.length;

                                string format = Path.GetExtension(AssetDatabase.GetAssetPath(ac)).ToLower();
                                x_Format.stringValue = format;

                                x_Format.serializedObject.ApplyModifiedProperties();
                                x_Length.serializedObject.ApplyModifiedProperties();
                                x_Frequency.serializedObject.ApplyModifiedProperties();
                                x_Channel.serializedObject.ApplyModifiedProperties();
                                item.serializedObject.ApplyModifiedProperties();
                            }
                        }
                    }
                    #endregion

                    if (base_script.library_sounds != null && base_script.library_sounds.Count > 0)
                        GUI.enabled = true;
                    else
                        GUI.enabled = false;

                    GUILayout.FlexibleSpace();

                    #region 停止并删除所有预览音效
                    if (XGUI.layout_button(
                    tooltip: "停止并删除所有预览音效",
                        tex_release: stoppreview_r,
                        tex_press: stoppreview_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: 14,
                        height: 14))
                    {
                        if (Application.isPlaying)
                            return;
                        Preview_Sound_Stop();
                    }
                    #endregion

                    GUI.enabled = true;

                    GUILayout.FlexibleSpace();

                    #region 清空所有音效项
                    if (XGUI.layout_button(
                        tooltip: "清空所有音效项",
                        tex_release: clear_r,
                        tex_press: clear_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: 14,
                        height: 14))
                    {
                        string res = XGUI.dialog(
                            type: XGUIDialogType.警告,
                            windowtitle: "XHud - 音效库消息",
                            title: "清空所有音效项",
                            msg: "是否确认要清空当前音效库的所有音效项？",
                            ok: "清空",
                            cancel: "暂不",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                        if (res == "暂不")
                            return;

                        sp_library_sounds.ClearArray();
                        sp_library_sounds.serializedObject.ApplyModifiedProperties();
                    }
                    #endregion

                    GUILayout.FlexibleSpace();

                    #region 添加项
                    if (XGUI.layout_button(
                        tooltip: "添加项",
                        tex_release: create_r,
                        tex_press: create_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: 14,
                        height: 14))
                    {
                        SoundInfoList_Original_Add(sound_info_list);
                    }
                    #endregion

                    GUILayout.FlexibleSpace();

                    #region 删除项
                    if (XGUI.layout_button(
                        tooltip: "删除项",
                        tex_release: delete_r,
                        tex_press: delete_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: 14,
                        height: 14))
                    {
                        SoundInfoList_Original_Remove(sound_info_list);
                    }
                    #endregion

                    XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
                }
            }
            #endregion

            #region 音效列表
            XGUI.layout_group_start(
               type: XGUIContainerType.Vertical,
               bg_fill: XGUIGroup.缺口纯色边框,
               bg_color: XGUIColor.亮白,
               bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
               title: "音效列表",
               title_size: XGUIFontSize.M,
               title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
               title_clipping: TextClipping.Clip,
               padding: new RectOffset(10, 10, 15, 15));

            DrawSoundInfoList_Original();

            ///------------选择音效
            command_name = Event.current.commandName;
            if (command_name == "ObjectSelectorUpdated")
            {
                UpdateSoundParams();
                if (is_object_selector_updated)
                {
                    if (base_script.act_on_sound_changed != null)
                        base_script.act_on_sound_changed();
                }
                is_object_selector_updated = false;
                select_object = false;
            }
            else if (command_name == "ObjectSelectorClosed")
            {
                if (!is_object_selector)
                    return;

                Object obj = EditorGUIUtility.GetObjectPickerObject();

                if (obj != null)
                {
                    if (!select_object)
                    {
                        is_object_selector = false;
                        select_object = true;
                        if (base_script.library_sounds == null)
                            base_script.library_sounds = new List<XHud_LibraryArg_Sound>();
                        AudioClip clip = obj as AudioClip;
                        string format = Path.GetExtension(AssetDatabase.GetAssetPath(clip)).ToLower();
                        base_script.library_sounds.Add(new XHud_LibraryArg_Sound(clip.name, clip, format));
                        if (base_script.act_on_sound_added != null)
                            base_script.act_on_sound_added(clip.name, clip);
                    }
                }
            }
            else
            {
                is_object_selector_updated = true;
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 拖放操作
            Event evt = Event.current;
            if (scrollview_rect.Contains(evt.mousePosition))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    Object[] dropobjs = DragAndDrop.objectReferences;
                    string names = "";
                    foreach (var item in dropobjs)
                    {
                        XHud_LibraryArg_Sound it = new XHud_LibraryArg_Sound();
                        it.clip = (AudioClip)item;
                        if (it.clip != null)
                        {
                            bool isExist = false;
                            for (int s = 0; s < sp_library_sounds.arraySize; s++)
                            {
                                SerializedProperty ser = sp_library_sounds.GetArrayElementAtIndex(s);
                                SerializedProperty ser_target = ser.FindPropertyRelative(nameof(XHud_LibraryArg_Sound.clip));
                                if (ser_target.objectReferenceValue.name == it.clip.name)
                                {
                                    isExist = true;
                                    break;
                                }
                            }
                            if (isExist)
                            {
                                XGUI.dialog(
                                    type: XGUIDialogType.警告,
                                    windowtitle: "XHud - 音效库消息",
                                    title: "重复添加音效",
                                    msg: $"此音效已存在于该库中！请勿重复添加！重复音效名称： {it.clip.name}",
                                    ok: "明白",
                                    PrimaryIndex: 0,
                                    usemodal: true,
                                    themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                                continue;
                            }
                            else
                            {
                                string item_format = Path.GetExtension(AssetDatabase.GetAssetPath(item)).ToLower();

                                it.name = it.clip.name;
                                it.format = item_format;

                                sp_library_sounds.InsertArrayElementAtIndex(sp_library_sounds.arraySize);
                                SerializedProperty root = sp_library_sounds.GetArrayElementAtIndex(sp_library_sounds.arraySize - 1);
                                SerializedProperty x_Clip = root.FindPropertyRelative(nameof(XHud_LibraryArg_Sound.clip));
                                SerializedProperty x_Name = root.FindPropertyRelative(nameof(XHud_LibraryArg_Sound.name));
                                SerializedProperty x_Frequency = root.FindPropertyRelative(nameof(XHud_LibraryArg_Sound.frequency));
                                SerializedProperty x_Channel = root.FindPropertyRelative(nameof(XHud_LibraryArg_Sound.channel));
                                SerializedProperty x_Length = root.FindPropertyRelative(nameof(XHud_LibraryArg_Sound.length));
                                SerializedProperty x_Format = root.FindPropertyRelative(nameof(XHud_LibraryArg_Sound.format));

                                x_Clip.objectReferenceValue = it.clip;
                                x_Name.stringValue = it.name;
                                x_Channel.intValue = it.clip.channels;
                                x_Frequency.intValue = it.clip.frequency;
                                x_Length.floatValue = it.clip.length;
                                x_Format.stringValue = it.format;

                                root.serializedObject.ApplyModifiedProperties();

                                names += x_Name.stringValue + " / ";
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(names))
                    {
                        XGUI.dialog(
                          type: XGUIDialogType.警告,
                          windowtitle: "XHud - 音效库消息",
                          title: "添加音效完成",
                          msg: $"所选有效音效 : {names}已添加到音效库中！",
                          ok: "明白",
                          PrimaryIndex: 0,
                          usemodal: true,
                          themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                    }
                }
            }
            #endregion

            serializedObject.ApplyModifiedProperties();
        }
        #endregion

        #region 列表绘制
        /// <summary>
        /// 根据高亮过滤条件屏蔽或启用 GUI。
        /// <para/>
        /// 若名称不包含 <see cref="sp_highlight"/> 的当前值，则禁用 GUI。
        /// </summary>
        /// <param name="name">待检查的名称</param>
        private void BlockGUI(string name)
        {
            if (!name.Contains(sp_highlight.stringValue))
            {
                GUI.enabled = false;
            }
            else
            {
                GUI.enabled = true;
            }
        }
        /// <summary>
        /// 绘制元素回调。
        /// <para/>
        /// 绘制内容：
        /// <list type="bullet">
        /// <item><description>序号（灰色，D2 格式）；</description></item>
        /// <item><description>音效名称输入框；</description></item>
        /// <item><description>音频剪辑属性字段；</description></item>
        /// <item><description>音效信息（时长 / 声道 / 采样率 / 格式）；</description></item>
        /// <item><description>过滤遮罩与定位标记。</description></item>
        /// </list>
        /// </summary>
        /// <param name="rect">绘制矩形</param>
        /// <param name="index">索引</param>
        /// <param name="isActive">是否激活</param>
        /// <param name="isFocused">是否聚焦</param>
        private void SoundInfoList_Original_DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            float titleheight = rect.y + 8;

            SerializedProperty prop = sp_library_sounds.GetArrayElementAtIndex(index);

            SerializedProperty sp_name = prop.FindPropertyRelative(nameof(XHud_LibraryArg_Sound.name));
            SerializedProperty sp_clip = prop.FindPropertyRelative(nameof(XHud_LibraryArg_Sound.clip));
            SerializedProperty sp_length = prop.FindPropertyRelative(nameof(XHud_LibraryArg_Sound.length));
            SerializedProperty sp_chan = prop.FindPropertyRelative(nameof(XHud_LibraryArg_Sound.channel));
            SerializedProperty sp_freq = prop.FindPropertyRelative(nameof(XHud_LibraryArg_Sound.frequency));
            SerializedProperty sp_format = prop.FindPropertyRelative(nameof(XHud_LibraryArg_Sound.format));

            #region 序号
            drawelement_rect.Set(rect.x + 16, rect.y + 6, 30, 20);
            XGUI.gui_label(
                rect: drawelement_rect,
                text: new GUIContent(index.ToString("D2")),
                text_color: Color.gray,
                size: XGUIFontSize.S,
                clipping: TextClipping.Clip,
                anchor: TextAnchor.MiddleLeft,
                offset: new Vector2(0, 0),
                font_style: FontStyle.Normal);
            #endregion

            BlockGUI(sp_name.stringValue);

            #region 音效名称
            drawelement_rect.Set(rect.x + 35, titleheight, rect.width - 180, line_height);
            sp_name.stringValue = XGUI.gui_inputfield(
                     rect: drawelement_rect,
                     //title: "Bk",
                     prop: sp_name.stringValue,
                     text_wrap: true,
                     field_fontsize: XGUIFontSize.M,
                     field_text_offset: Vector2.zero,
                     field_height: XGUI.GetSingleLineHeight(),
                     field_text_color: Color.white,
                     //title_width: 40,
                     //status_icon: "icon_field_status",
                     //status_icon_color: Color.magenta,
                     field_text_font: XGUI.GetFont("xg-medium"),
                     field_text_style: FontStyle.Normal,
                     field_text_anchor: TextAnchor.UpperLeft,
                     field_padding: new RectOffset(5, 5, 0, 0),
                     field_margin: new RectOffset(0, 0, 0, 0));
            sp_name.serializedObject.ApplyModifiedProperties();
            #endregion

            BlockGUI(sp_name.stringValue);

            #region 音效
            drawelement_rect.Set(rect.width - 120, titleheight, 100, line_height);
            XGUI.gui_property_field(
                           rect: drawelement_rect,
                           //status_icon: "icon_field_status",
                           //status_icon_color: Color.red,
                           prop: sp_clip);
            #endregion

            BlockGUI(sp_name.stringValue);

            #region 音效信息
            string cxs = sp_length.floatValue.ToString("F2") + " s";
            string xxc = "";
            if (base_script.library_sounds[index].channel == 2)
            {
                xxc = sp_chan.intValue + " ch (Stereo)";
            }
            else
            {
                xxc = sp_chan.intValue + " ch (Mono)";
            }
            string fsr = sp_freq.intValue + " hz";

            string res = cxs + " | " + xxc + " | " + fsr + " | " + sp_format.stringValue;

            drawelement_rect.Set(rect.x + 36, rect.y + 28, rect.width - 10, line_height);
            XGUI.gui_label(
                rect: drawelement_rect,
                text: new GUIContent(res),
                text_color: Color.white * 0.75f,
                size: XGUIFontSize.S,
                clipping: TextClipping.Clip,
                anchor: TextAnchor.MiddleLeft,
                offset: new Vector2(0, 0),
                font_style: FontStyle.Normal
                //font: XGUI.GetFont("xg-medium")
                );
            #endregion

            GUI.enabled = true;

            if (!sp_name.stringValue.Contains(sp_highlight.stringValue))
            {
                drawelement_rect.Set(rect.x - 20, rect.y, rect.width + 20, rect.height);
                EditorGUI.DrawRect(drawelement_rect, blocked_col);
            }

            if (index == sp_location_selected_index.intValue)
            {
                drawelement_rect.Set(rect.width - 80, rect.y + 25, 58, 20);
                XGUI.gui_label(
                    rect: drawelement_rect,
                    text: new GUIContent("已定位"),
                    text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    size: XGUIFontSize.S,
                    clipping: TextClipping.Clip,
                    anchor: TextAnchor.MiddleRight,
                    offset: new Vector2(0, 0),
                    font_style: FontStyle.Normal);
            }
        }
        /// <summary>
        /// 绘制音效列表。
        /// <para/>
        /// 只绘制滚动视图可视区域内的元素，以优化大列表性能。
        /// 单击列表项时自动触发预览试听。
        /// </summary>
        private void DrawSoundInfoList_Original()
        {
            // 绘制滚动视图
            scrollview_rect = GUILayoutUtility.GetRect(0, sp_visible_item_count.intValue * sp_item_height.floatValue);
            sp_sound_Info_list_original_scroller.vector2Value = GUI.BeginScrollView(scrollview_rect, sp_sound_Info_list_original_scroller.vector2Value, new Rect(0, 0, scrollview_rect.width - 50, sound_info_list.count * sp_item_height.floatValue), false, true);

            // 计算可视区域的起始和结束索引
            int startIndex = Mathf.FloorToInt(sp_sound_Info_list_original_scroller.vector2Value.y / sp_item_height.floatValue);
            int endIndex = Mathf.CeilToInt((sp_sound_Info_list_original_scroller.vector2Value.y + scrollview_rect.height) / sp_item_height.floatValue);

            // 只绘制可视区域内的元素
            for (int i = startIndex; i < endIndex && i < sound_info_list.count; i++)
            {
                SerializedProperty prop = sp_library_sounds.GetArrayElementAtIndex(i);
                SerializedProperty sp_name = prop.FindPropertyRelative(nameof(XHud_LibraryArg_Sound.name));

                item_rect.Set(0, i * sp_item_height.floatValue, scrollview_rect.width, sp_item_height.floatValue);

                // 如果当前元素被选中，绘制选中效果
                if (sp_name.stringValue.Contains(sp_highlight.stringValue))
                {
                    if (sp_selected_index.intValue == i)
                    {
                        item_rect.Set(1, i * sp_item_height.floatValue + 13, 5, 5);
                        // 高亮标记表示选中
                        EditorGUI.DrawRect(item_rect, XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                        item_rect.Set(0, i * sp_item_height.floatValue, scrollview_rect.width, sp_item_height.floatValue);
                        // 高亮背景表示选中
                        EditorGUI.DrawRect(item_rect, SelectedBg);
                    }
                }

                item_rect.Set(0, i * sp_item_height.floatValue, scrollview_rect.width, sp_item_height.floatValue);
                sound_info_list.drawElementCallback.Invoke(item_rect, i, i == sound_info_list.index, true);

                // 检测鼠标是否在当前元素区域内
                if (item_rect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.MouseDown)
                    {
                        if (sp_name.stringValue.Contains(sp_highlight.stringValue))
                        {
                            // 更新选中项
                            sp_selected_index.intValue = i;

                            if (!Application.isPlaying)
                            {
                                Preview_Sound_Play(base_script.library_sounds[i]);
                            }
                            // 标记界面需要更新
                            GUI.changed = true;
                        }
                    }
                }

                // 绘制元素
                EditorGUI.PropertyField(item_rect, prop, GUIContent.none);
            }

            GUI.EndScrollView();
        }
        /// <summary>
        /// 列表移除项。
        /// <para/>
        /// 移除当前 ReorderableList 索引对应的音效项，并触发移除回调。
        /// </summary>
        /// <param name="list">可重排列表</param>
        private void SoundInfoList_Original_Remove(ReorderableList list)
        {
            if (list.count <= 0)
                return;

            sp_library_sounds.DeleteArrayElementAtIndex(list.index);

            if (base_script.act_on_sound_removed != null)
                base_script.act_on_sound_removed();
        }
        /// <summary>
        /// 列表增加项。
        /// <para/>
        /// 打开 Unity 对象选择器，让用户选择 <see cref="AudioClip"/> 后添加到音效库。
        /// </summary>
        /// <param name="list">可重排列表</param>
        private void SoundInfoList_Original_Add(ReorderableList list)
        {
            is_object_selector = true;
            EditorGUIUtility.ShowObjectPicker<AudioClip>(SelectedClip, false, "", 0);
        }
        #endregion

        #region 工具 - 音效分析
        /// <summary>
        /// 刷新音效参数。
        /// <para/>
        /// 遍历音效库，用音频剪辑的实际参数更新声道、采样率与时长。
        /// </summary>
        private void UpdateSoundParams()
        {
            if (base_script.library_sounds != null && base_script.library_sounds.Count > 0)
            {
                for (int i = 0; i < base_script.library_sounds.Count; i++)
                {
                    if (base_script.library_sounds[i].clip != null)
                    {
                        base_script.library_sounds[i].channel = base_script.library_sounds[i].clip.channels;
                        base_script.library_sounds[i].frequency = base_script.library_sounds[i].clip.frequency;
                        base_script.library_sounds[i].length = base_script.library_sounds[i].clip.length;
                    }
                }
            }
        }
        /// <summary>
        /// 播放模式切换回调。
        /// <para/>
        /// 在退出编辑模式时清理所有预览音效对象。
        /// </summary>
        /// <param name="state">播放模式状态</param>
        private void EditorApplication_playModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                Preview_Sound_Stop();
            }
        }
        #endregion

        #region 工具 - 音效预览
        /// <summary>
        /// 预览音效（统一入口）。
        /// <para/>
        /// 本方法只负责"决定要预览哪些 Sounder"，具体参数计算与入队交给 <see cref="EnqueueSound"/>。
        /// <para/>
        /// 【多选行为】多选时逐个 Sounder 各自取 SoundName / DelayTime / Volume / Pitch，
        /// 不再共用当前查看对象的 SerializedProperty 值（修正原实现的隐藏 bug）。
        /// </summary>
        private void Preview_Sound_Play(XHud_LibraryArg_Sound sounder)
        {
            XHud_Manager mgr = XHud_Dashboard.get_manager();
            if (mgr.library_sounds == null)
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
                                Transform man = mgr.transform;
                                EditorGUIUtility.PingObject(man);
                            }
                        });
                };
            }
            else
            {
                EnqueueSound(sounder);
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
        private void EnqueueSound(XHud_LibraryArg_Sound sounder)
        {
            XHud_Manager mgr = XHud_Dashboard.get_manager();

            if (sounder == null) return;
            if (string.IsNullOrEmpty(sounder.name)) return;
            if (mgr == null || mgr.library_sounds == null) return;
            AudioClip clip = mgr.library_sounds.SoundLibrary_GetSound(sounder.name);
            if (clip == null) return;
            // ── 算好参数 ──
            float delay = Mathf.Max(0f, 0);
            float masterVol = (mgr != null) ? mgr.global_volume * 0.01f : 1f;
            bool masterMute = (mgr != null) && mgr.global_volume_mute;
            float volume = masterVol * Mathf.Clamp01(1);
            bool mute = masterMute;
            bool useRandomPitch = sp_use_random_pitch.boolValue;
            float pMin = sp_pitch_min.floatValue;
            float pMax = sp_pitch_max.floatValue;
            XGUI.EditorPlayer_Sound.Enqueue(
                clip, delay, volume, mute, useRandomPitch, pMin, pMax);
        }
        /// <summary>
        /// 停止所有音效预览。
        /// <para/>
        /// 委托给 <see cref="XGUI_Player_Sound.Stop()"/>，
        /// 语义完全一致：待触发全丢 + 正在播立即掐断并销毁。
        /// </summary>
        private void Preview_Sound_Stop()
        {
            XGUI.EditorPlayer_Sound.Stop();
        }
        #endregion

        #region 绘制控件
        /// <summary>
        /// 通用方法：绘制带标题的开关控件。
        /// <para/>
        /// 封装 <see cref="XGUI.layout_toggle"/>，统一标题宽度、字体、混合选项样式。
        /// </summary>
        private void DrawToggle(string title, SerializedProperty prop, float width, XGUIToggleStyle style = XGUIToggleStyle.实体, Color color_bg_on = default, Color color_bg_off = default, Color color_on = default, Color color_off = default, Action<bool> act_on_changed = null)
        {
            XGUI.layout_toggle(
                title: title,
                title_size: XGUIFontSize.M,
                title_font_style: FontStyle.Normal,
                title_padding: new RectOffset(0, 10, 0, 0),
                title_width: width,
                prop: prop,
                tog_style: style,
                tog_padding: new RectOffset(5, 6, 0, 0),
                tog_margin: new RectOffset(0, 0, 0, 5),
                tog_mixed_options: new string[] { "禁用", "启用" },
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