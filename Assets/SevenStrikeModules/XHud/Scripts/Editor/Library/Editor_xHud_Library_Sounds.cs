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
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;
    using Object = UnityEngine.Object;
    using Random = UnityEngine.Random;

    #region 编辑器主体
    /// <summary>
    /// XHud 音效库的自定义编辑器。
    /// <para/>
    /// 提供音效列表的可视化编辑、拖放添加、对象选择器添加、预览试听、随机音高、扫描参数、增删清空等功能。
    /// </summary>
    [CustomEditor(typeof(XHud_Library_Sounds))]
    public class Editor_XHud_Library_Sounds : Editor
    {
        #region 字段 - 组件 / 列表
        /// <summary>
        /// 目标脚本实例。
        /// <para/>
        /// 在 <see cref="OnEnable"/> 中缓存。
        /// </summary>
        private XHud_Library_Sounds BaseScript;
        /// <summary>
        /// 音效列表（可重排列表）。
        /// <para/>
        /// 用于驱动 ReorderableList 的绘制与交互。
        /// </summary>
        public ReorderableList SoundInfoList;
        #endregion

        #region 字段 - 序列化属性
        /// <summary>
        /// 本 Inspector 用到的全部序列化属性引用。
        /// <para/>
        /// 统一在 <see cref="OnEnable"/> 里初始化。
        /// <para/>
        /// 覆盖：
        /// <list type="bullet">
        /// <item><description>音效库列表；</description></item>
        /// <item><description>随机音高开关与音高范围；</description></item>
        /// <item><description>选中 / 定位索引；</description></item>
        /// <item><description>过滤 / 查找字符串；</description></item>
        /// <item><description>列表滚动位置与项高；</description></item>
        /// <item><description>库名称。</description></item>
        /// </list>
        /// </summary>
        private SerializedProperty sp_SoundInfoList_Original_Scroller, sp_itemHeight, sp_visibleItemCount, sp_SoundList, sp_UseRandomPitch, sp_Pitch_Min, sp_Pitch_Max, sp_SelectedIndex, sp_LocationSelectedIndex, sp_Highlight, sp_Find, sp_LibraryName;
        #endregion

        #region 字段 - 参数
        /// <summary>
        /// 单行高度缓存。
        /// </summary>
        private float LineHeight;
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

        #region 字段 - 图标
        /// <summary>
        /// 标题 / 按钮 / 图标资源缓存。
        /// <para/>
        /// 统一在 <see cref="OnEnable"/> 里加载。
        /// </summary>
        private Texture2D scan_p, scan_r, stoppreview_p, stoppreview_r, clear_p, clear_r, create_p, create_r, delete_p, delete_r;
        #endregion

        #region 字段 - 预览 / 选择器
        /// <summary>
        /// 当前 Unity 命令名称。
        /// <para/>
        /// 用于检测对象选择器状态变化。
        /// </summary>
        string commandName;
        /// <summary>
        /// 是否已经从对象选择器选中过对象。
        /// </summary>
        private bool selectobject;
        /// <summary>
        /// 预览音效使用的 AudioSource 列表。
        /// <para/>
        /// 用于停止预览与播放结束后回收。
        /// </summary>
        List<AudioSource> PreviewAudioList;
        /// <summary>
        /// 是否处于对象选择器打开状态。
        /// </summary>
        private bool IsObjectSelector;
        /// <summary>
        /// 对象选择器是否发生更新。
        /// </summary>
        private bool IsObjectSelectorUpdated;
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
        XHud_Library_Sounds[] SelectedObjects;
        #endregion

        #region 工具 - 批量化操作
        /// <summary>
        /// 缓存当前 Inspector 选中的所有目标组件到 <see cref="SelectedObjects"/>。
        /// <para/>
        /// 多选时逐个转换，单选时只填充一个元素。
        /// </summary>
        private void Targets_Get()
        {
            if (targets.Length > 1)
            {
                SelectedObjects = new XHud_Library_Sounds[targets.Length];
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    var t = targets[i];
                    SelectedObjects[i] = (XHud_Library_Sounds)t;
                }
            }
            else
            {
                SelectedObjects = new XHud_Library_Sounds[targets.Length];
                SelectedObjects[0] = (XHud_Library_Sounds)target;
            }
        }
        /// <summary>
        /// 判断当前是否处于多选状态。
        /// </summary>
        /// <returns>true 表示选中了多个组件</returns>
        private bool Targets_Selected()
        {
            if (SelectedObjects == null)
                return false;
            if (SelectedObjects.Length > 1)
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
            BaseScript = (XHud_Library_Sounds)target;

            #region 获取序列化属性
            sp_SoundList = serializedObject.FindProperty("SoundLibrary");
            sp_UseRandomPitch = serializedObject.FindProperty("UseRandomPitch");
            sp_Pitch_Min = serializedObject.FindProperty("Pitch_Min");
            sp_Pitch_Max = serializedObject.FindProperty("Pitch_Max");
            sp_LibraryName = serializedObject.FindProperty("LibraryName");
            sp_SelectedIndex = serializedObject.FindProperty("SelectedIndex");
            sp_Highlight = serializedObject.FindProperty("Highlight");
            sp_LocationSelectedIndex = serializedObject.FindProperty("LocationSelectedIndex");
            sp_SoundInfoList_Original_Scroller = serializedObject.FindProperty("SoundInfoList_Original_Scroller");
            sp_itemHeight = serializedObject.FindProperty("itemHeight");
            sp_visibleItemCount = serializedObject.FindProperty("visibleItemCount");
            sp_Find = serializedObject.FindProperty("Find");
            #endregion

            LineHeight = EditorGUIUtility.singleLineHeight;

            Targets_Get();

            //设定列表项高度值
            sp_itemHeight.floatValue = 50;
            sp_itemHeight.serializedObject.ApplyModifiedProperties();

            sp_visibleItemCount.intValue = 8;
            sp_visibleItemCount.serializedObject.ApplyModifiedProperties();

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

            #region ReorderableList - SoundInfoList
            SoundInfoList = new ReorderableList(serializedObject, sp_SoundList, true, true, true, true);
            SoundInfoList.drawElementCallback = SoundInfoList_Original_DrawElementCallback;
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

            sp_SoundInfoList_Original_Scroller.vector2Value = Vector2.zero;
            sp_SoundInfoList_Original_Scroller.serializedObject.ApplyModifiedProperties();

            sp_LocationSelectedIndex.intValue = -1;
            sp_LocationSelectedIndex.serializedObject.ApplyModifiedProperties();

            sp_SelectedIndex.intValue = -1;
            sp_SelectedIndex.serializedObject.ApplyModifiedProperties();

            sp_Find.stringValue = string.Empty;
            sp_Find.serializedObject.ApplyModifiedProperties();

            sp_Highlight.stringValue = string.Empty;
            sp_Highlight.serializedObject.ApplyModifiedProperties();

            RecycleAudio();
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

#if UNITY_6000_0_OR_NEWER
            TextClipping clipping = TextClipping.Ellipsis;
#else
    TextClipping clipping = TextClipping.Clip;
#endif

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
                title_clipping: clipping,
                title_offset: new Vector2(-10, 0),
                bg_margin: new RectOffset(0, 0, 5, 5));
            #endregion

            XGUI.layout_space(5);

            #region 参数区
            XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIGroup.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.Theme_Group,
                    title: "参数",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.Theme_Primary,
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15));

            #region 库名称
            XGUI.layout_property_field(
                title: "库名称",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.Theme_Primary,
                title_width: 90,
                prop: sp_LibraryName,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 过滤（包含）
            XGUI.ChangedCheck_Start();
            XGUI.layout_property_field(
                title: "过滤（包含）",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.Theme_Primary,
                title_width: 90,
                prop: sp_Highlight,
                prop_margin: new RectOffset(0, 0, 5, 0));
            if (XGUI.ChangedCheck_End())
            {
                sp_LocationSelectedIndex.intValue = -1;
                sp_LocationSelectedIndex.serializedObject.ApplyModifiedProperties();
            }
            #endregion

            #region 查找（精确）
            XGUI.ChangedCheck_Start();
            XGUI.layout_property_field(
                title: "查找（精确）",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.Theme_Primary,
                title_width: 90,
                prop: sp_Find,
                prop_margin: new RectOffset(0, 0, 5, 0));
            if (XGUI.ChangedCheck_End())
            {
                if (!string.IsNullOrEmpty(sp_Find.stringValue))
                    BaseScript.SoundLibrary_Location_Find(sp_Find.stringValue);
                else
                {
                    sp_SelectedIndex.intValue = -1;
                    sp_SelectedIndex.serializedObject.ApplyModifiedProperties();

                    sp_LocationSelectedIndex.intValue = -1;
                    sp_LocationSelectedIndex.serializedObject.ApplyModifiedProperties();
                }
            }
            #endregion

            XGUI.layout_seperator(
                thickness: 1,
                color: XHud_Dashboard.Theme_SeperateLine,
                margin: new RectOffset(15, 15, 20, 15));

            #region 随机音高
            DrawToggle("随机音高", sp_UseRandomPitch, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, (b) => { });
            #endregion

            if (sp_UseRandomPitch.boolValue)
            {
                #region 最小音高
                XGUI.layout_property_field(
                    title: "最小音高",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: sp_Pitch_Min,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                #region 最大音高
                XGUI.layout_property_field(
                    title: "最大音高",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: sp_Pitch_Max,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 快捷功能
            if (string.IsNullOrEmpty(sp_Highlight.stringValue))
            {
                if (!Application.isPlaying)
                {
                    XGUI.layout_group_start(
                        type: XGUIContainerType.Horizontal,
                        bg_fill: XGUIGroup.缺口纯色边框,
                        bg_color: XGUIColor.亮白,
                        bg_color_gui: XHud_Dashboard.Theme_Group,
                        title: "快捷功能",
                        title_size: XGUIFontSize.M,
                        title_text_color: XHud_Dashboard.Theme_Primary,
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
                        for (int i = 0; i < sp_SoundList.arraySize; i++)
                        {
                            SerializedProperty item = sp_SoundList.GetArrayElementAtIndex(i);
                            SerializedProperty x_Clip = item.FindPropertyRelative("Clip");
                            SerializedProperty x_Frequency = item.FindPropertyRelative("Frequency");
                            SerializedProperty x_Channel = item.FindPropertyRelative("Channel");
                            SerializedProperty x_Length = item.FindPropertyRelative("Length");
                            SerializedProperty x_Format = item.FindPropertyRelative("Format");

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

                    if (BaseScript.SoundLibrary != null && BaseScript.SoundLibrary.Count > 0)
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
                        RecycleAudio();
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
                            themecolor: XHud_Dashboard.Theme_Primary);
                        if (res == "暂不")
                            return;

                        sp_SoundList.ClearArray();
                        sp_SoundList.serializedObject.ApplyModifiedProperties();
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
                        SoundInfoList_Original_Add(SoundInfoList);
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
                        SoundInfoList_Original_Remove(SoundInfoList);
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
               bg_color_gui: XHud_Dashboard.Theme_Group,
               title: "音效列表",
               title_size: XGUIFontSize.M,
               title_text_color: XHud_Dashboard.Theme_Primary,
               title_clipping: TextClipping.Clip,
               padding: new RectOffset(10, 10, 15, 15));

            DrawSoundInfoList_Original();

            ///------------选择音效
            commandName = Event.current.commandName;
            if (commandName == "ObjectSelectorUpdated")
            {
                UpdateSoundParams();
                if (IsObjectSelectorUpdated)
                {
                    if (BaseScript.act_on_SoundChanged != null)
                        BaseScript.act_on_SoundChanged();
                }
                IsObjectSelectorUpdated = false;
                selectobject = false;
            }
            else if (commandName == "ObjectSelectorClosed")
            {
                if (!IsObjectSelector)
                    return;

                Object obj = EditorGUIUtility.GetObjectPickerObject();

                if (obj != null)
                {
                    if (!selectobject)
                    {
                        IsObjectSelector = false;
                        selectobject = true;
                        if (BaseScript.SoundLibrary == null)
                            BaseScript.SoundLibrary = new List<XHud_LibraryArg_Sound>();
                        AudioClip clip = obj as AudioClip;
                        string format = Path.GetExtension(AssetDatabase.GetAssetPath(clip)).ToLower();
                        BaseScript.SoundLibrary.Add(new XHud_LibraryArg_Sound(clip.name, clip, format));
                        if (BaseScript.act_on_SoundAdded != null)
                            BaseScript.act_on_SoundAdded(clip.name, clip);
                    }
                }
            }
            else
            {
                IsObjectSelectorUpdated = true;
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
                        it.Clip = (AudioClip)item;
                        if (it.Clip != null)
                        {
                            bool isExist = false;
                            for (int s = 0; s < sp_SoundList.arraySize; s++)
                            {
                                SerializedProperty ser = sp_SoundList.GetArrayElementAtIndex(s);
                                SerializedProperty ser_target = ser.FindPropertyRelative("Clip");
                                if (ser_target.objectReferenceValue.name == it.Clip.name)
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
                                    msg: $"此音效已存在于该库中！请勿重复添加！重复音效名称： {it.Clip.name}",
                                    ok: "明白",
                                    PrimaryIndex: 0,
                                    usemodal: true,
                                    themecolor: XHud_Dashboard.Theme_Primary);
                                continue;
                            }
                            else
                            {
                                string item_format = Path.GetExtension(AssetDatabase.GetAssetPath(item)).ToLower();

                                it.Name = it.Clip.name;
                                it.Format = item_format;

                                sp_SoundList.InsertArrayElementAtIndex(sp_SoundList.arraySize);
                                SerializedProperty root = sp_SoundList.GetArrayElementAtIndex(sp_SoundList.arraySize - 1);
                                SerializedProperty x_Clip = root.FindPropertyRelative("Clip");
                                SerializedProperty x_Name = root.FindPropertyRelative("Name");
                                SerializedProperty x_Frequency = root.FindPropertyRelative("Frequency");
                                SerializedProperty x_Channel = root.FindPropertyRelative("Channel");
                                SerializedProperty x_Length = root.FindPropertyRelative("Length");
                                SerializedProperty x_Format = root.FindPropertyRelative("Format");

                                x_Clip.objectReferenceValue = it.Clip;
                                x_Name.stringValue = it.Name;
                                x_Channel.intValue = it.Clip.channels;
                                x_Frequency.intValue = it.Clip.frequency;
                                x_Length.floatValue = it.Clip.length;
                                x_Format.stringValue = it.Format;

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
                          themecolor: XHud_Dashboard.Theme_Primary);
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
        /// 若名称不包含 <see cref="sp_Highlight"/> 的当前值，则禁用 GUI。
        /// </summary>
        /// <param name="name">待检查的名称</param>
        private void BlockGUI(string name)
        {
            if (!name.Contains(sp_Highlight.stringValue))
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

            SerializedProperty prop = sp_SoundList.GetArrayElementAtIndex(index);

            SerializedProperty sp_name = prop.FindPropertyRelative("Name");
            SerializedProperty sp_clip = prop.FindPropertyRelative("Clip");
            SerializedProperty sp_length = prop.FindPropertyRelative("Length");
            SerializedProperty sp_chan = prop.FindPropertyRelative("Channel");
            SerializedProperty sp_freq = prop.FindPropertyRelative("Frequency");
            SerializedProperty sp_format = prop.FindPropertyRelative("Format");

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
            drawelement_rect.Set(rect.x + 35, titleheight, rect.width - 180, LineHeight);
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
            drawelement_rect.Set(rect.width - 120, titleheight, 100, LineHeight);
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
            if (BaseScript.SoundLibrary[index].Channel == 2)
            {
                xxc = sp_chan.intValue + " ch (Stereo)";
            }
            else
            {
                xxc = sp_chan.intValue + " ch (Mono)";
            }
            string fsr = sp_freq.intValue + " hz";

            string res = cxs + " | " + xxc + " | " + fsr + " | " + sp_format.stringValue;

            drawelement_rect.Set(rect.x + 36, rect.y + 28, rect.width - 10, LineHeight);
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

            if (!sp_name.stringValue.Contains(sp_Highlight.stringValue))
            {
                drawelement_rect.Set(rect.x - 20, rect.y, rect.width + 20, rect.height);
                EditorGUI.DrawRect(drawelement_rect, blocked_col);
            }

            if (index == sp_LocationSelectedIndex.intValue)
            {
                drawelement_rect.Set(rect.width - 80, rect.y + 25, 58, 20);
                XGUI.gui_label(
                    rect: drawelement_rect,
                    text: new GUIContent("已定位"),
                    text_color: XHud_Dashboard.Theme_Primary,
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
            scrollview_rect = GUILayoutUtility.GetRect(0, sp_visibleItemCount.intValue * sp_itemHeight.floatValue);
            sp_SoundInfoList_Original_Scroller.vector2Value = GUI.BeginScrollView(scrollview_rect, sp_SoundInfoList_Original_Scroller.vector2Value, new Rect(0, 0, scrollview_rect.width - 50, SoundInfoList.count * sp_itemHeight.floatValue), false, true);

            // 计算可视区域的起始和结束索引
            int startIndex = Mathf.FloorToInt(sp_SoundInfoList_Original_Scroller.vector2Value.y / sp_itemHeight.floatValue);
            int endIndex = Mathf.CeilToInt((sp_SoundInfoList_Original_Scroller.vector2Value.y + scrollview_rect.height) / sp_itemHeight.floatValue);

            // 只绘制可视区域内的元素
            for (int i = startIndex; i < endIndex && i < SoundInfoList.count; i++)
            {
                SerializedProperty prop = sp_SoundList.GetArrayElementAtIndex(i);
                SerializedProperty sp_color = prop.FindPropertyRelative("Color");
                SerializedProperty sp_name = prop.FindPropertyRelative("Name");

                item_rect.Set(0, i * sp_itemHeight.floatValue, scrollview_rect.width, sp_itemHeight.floatValue);

                // 如果当前元素被选中，绘制选中效果
                if (sp_name.stringValue.Contains(sp_Highlight.stringValue))
                {
                    if (sp_SelectedIndex.intValue == i)
                    {
                        item_rect.Set(1, i * sp_itemHeight.floatValue + 13, 5, 5);
                        // 高亮标记表示选中
                        EditorGUI.DrawRect(item_rect, XHud_Dashboard.Theme_Primary);
                        item_rect.Set(0, i * sp_itemHeight.floatValue, scrollview_rect.width, sp_itemHeight.floatValue);
                        // 高亮背景表示选中
                        EditorGUI.DrawRect(item_rect, SelectedBg);
                    }
                }

                item_rect.Set(0, i * sp_itemHeight.floatValue, scrollview_rect.width, sp_itemHeight.floatValue);
                SoundInfoList.drawElementCallback.Invoke(item_rect, i, i == SoundInfoList.index, true);

                // 检测鼠标是否在当前元素区域内
                if (item_rect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.MouseDown)
                    {
                        if (sp_name.stringValue.Contains(sp_Highlight.stringValue))
                        {
                            // 更新选中项
                            sp_SelectedIndex.intValue = i;

                            if (!Application.isPlaying)
                            {
                                PreviewSound(BaseScript.SoundLibrary[i].Clip);
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

            sp_SoundList.DeleteArrayElementAtIndex(list.index);

            if (BaseScript.act_on_SoundRemoved != null)
                BaseScript.act_on_SoundRemoved();
        }
        /// <summary>
        /// 列表增加项。
        /// <para/>
        /// 打开 Unity 对象选择器，让用户选择 <see cref="AudioClip"/> 后添加到音效库。
        /// </summary>
        /// <param name="list">可重排列表</param>
        private void SoundInfoList_Original_Add(ReorderableList list)
        {
            IsObjectSelector = true;
            EditorGUIUtility.ShowObjectPicker<AudioClip>(SelectedClip, false, "", 0);
        }
        #endregion

        #region 工具 - 音效预览
        /// <summary>
        /// 刷新音效参数。
        /// <para/>
        /// 遍历音效库，用音频剪辑的实际参数更新声道、采样率与时长。
        /// </summary>
        private void UpdateSoundParams()
        {
            if (BaseScript.SoundLibrary != null && BaseScript.SoundLibrary.Count > 0)
            {
                for (int i = 0; i < BaseScript.SoundLibrary.Count; i++)
                {
                    if (BaseScript.SoundLibrary[i].Clip != null)
                    {
                        BaseScript.SoundLibrary[i].Channel = BaseScript.SoundLibrary[i].Clip.channels;
                        BaseScript.SoundLibrary[i].Frequency = BaseScript.SoundLibrary[i].Clip.frequency;
                        BaseScript.SoundLibrary[i].Length = BaseScript.SoundLibrary[i].Clip.length;
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
                RecycleAudio();

                for (int i = 0; i < PreviewAudioList.Count; i++)
                {
                    if (PreviewAudioList[i] != null)
                    {
                        DestroyImmediate(PreviewAudioList[i].gameObject, true);
                        PreviewAudioList[i] = null;
                        PreviewAudioList.RemoveAt(i);
                    }
                }
                PreviewAudioList.Clear();
            }
        }
        /// <summary>
        /// 回收预览音效。
        /// <para/>
        /// 停止所有正在播放的预览 AudioSource。
        /// </summary>
        private void RecycleAudio()
        {
            if (PreviewAudioList != null)
            {
                for (int i = 0; i < PreviewAudioList.Count; i++)
                {
                    if (PreviewAudioList[i] != null)
                    {
                        PreviewAudioList[i].Stop();
                    }
                }
            }
        }
        /// <summary>
        /// 预览声音。
        /// <para/>
        /// 创建一个临时 GameObject 与 AudioSource，根据随机音高设置播放音频剪辑。
        /// 该临时对象会被记录到 <see cref="PreviewAudioList"/> 中以便后续回收。
        /// </summary>
        /// <param name="clip">要预览的音频剪辑</param>
        private void PreviewSound(AudioClip clip)
        {
            GameObject obj = new GameObject();
            obj.name = "SoundPreviewer-" + "[" + clip.length.ToString("F2") + " s]-" + "[" + clip.channels + " ch]-" + "[" + clip.frequency + " hz]";
            AudioSource au = obj.AddComponent<AudioSource>();

            if (PreviewAudioList == null)
                PreviewAudioList = new List<AudioSource>();
            PreviewAudioList.Add(au);

            au.clip = clip;
            if (sp_UseRandomPitch.boolValue)
                au.pitch = Random.Range(sp_Pitch_Min.floatValue, sp_Pitch_Max.floatValue);
            else
                au.pitch = 1f;
            au.Play();
            //XHud_AudioStoper sp = au.gameObject.AddComponent<XHud_AudioStoper>();
            //sp.AudioSource = au;
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
                tog_mixed_bg_color_gui: XHud_Dashboard.Theme_Primary,
                act_on_changed: act_on_changed);
        }
        #endregion
    }
    #endregion
}