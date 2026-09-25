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
    using SevenStrikeModules.XHud.Utilitys;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.UIElements;
    using Random = UnityEngine.Random;

    [System.Serializable]
    public class PrimitiveTweenSoundNode
    {
        public XHud_Module_Primitive_Tween Tween;
        public int Index;
        public string Name;
        public TweenNodeType Type;
        public float MaxDuration;
        public List<TweenSound> TweenSounds = new List<TweenSound>();
    }

    public class Editor_XHud_PrimitiveTweenSoundSetTool : EditorWindow
    {
        #region 组件 / 列表
        /// <summary>
        /// 序列化物体
        /// </summary>
        private SerializedObject serializedObject;
        [SerializeField]
        public PrimitiveTweenSoundNode PrimitiveTweenSoundNode;
        private ReorderableList ListSounds;
        private XHud_Manager HudManager;
        #endregion

        private float LineHeight = EditorGUIUtility.singleLineHeight;

        #region 序列化属性
        private SerializedProperty sp_PrimitiveTweenSoundNodes;
        private SerializedProperty sp_TweenSounds;
        #endregion

        #region 选中的预览属性
        private string Selected_Name = "-";
        private string Selected_Hz = "- Hz";
        private string Selected_Length = "0.0 s";
        private string Selected_Steo = "-";
        #endregion

        private XCoroutine Coroutine_Preview;
        private Vector2 Scroll;
        public List<AudioSource> PreviewAudioList;

        private float Timer = 0f;
        private float Interval = 4f;

        #region 图标
        private Texture2D Logo_Add_Release, Logo_Add_Press, Logo_Play_Release, Logo_Play_Press, Logo_Apply_Release, Logo_Apply_Press, Icon_twn_sound_percent, Icon_twn_sound_volume, Icon_twn_sound_pitch, anim_type_move, anim_type_rotator, anim_type_scale, anim_type_color, anim_type_fade, anim_type_writter, anim_type_fill, anim_type_size;
        #endregion

        private void OnDisable()
        {
            StopAllPreviewAudio();
        }

        private void Update()
        {
            SceneViewUpdate();
        }

        private void SceneViewUpdate()
        {
            Timer += Time.deltaTime;
            if (Timer >= Interval)
            {
                SceneView.RepaintAll();
                Timer = 0f;
            }
        }

        private void OnEnable()
        {
            serializedObject = new SerializedObject(this);

            sp_PrimitiveTweenSoundNodes = serializedObject.FindProperty("PrimitiveTweenSoundNode");
            sp_TweenSounds = sp_PrimitiveTweenSoundNodes.FindPropertyRelative("TweenSounds");

            HudManager = XHud_Dashboard.HudManagerGet();

            #region 获取图标
            Logo_Add_Release = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primtive_tween_sound_setter/Logo_Add_Release");
            Logo_Add_Press = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primtive_tween_sound_setter/Logo_Add_Press");
            Logo_Play_Release = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primtive_tween_sound_setter/Logo_Play_Release");
            Logo_Play_Press = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primtive_tween_sound_setter/Logo_Play_Press");
            Logo_Apply_Release = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primtive_tween_sound_setter/Logo_Apply_Release");
            Logo_Apply_Press = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primtive_tween_sound_setter/Logo_Apply_Press");
            Icon_twn_sound_percent = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primtive_tween_sound_setter/Icon_twn_sound_percent");
            Icon_twn_sound_volume = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primtive_tween_sound_setter/Icon_twn_sound_volume");
            Icon_twn_sound_pitch = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primtive_tween_sound_setter/Icon_twn_sound_pitch");

            anim_type_move = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primtive_tween_sound_setter/anim_type_move");
            anim_type_rotator = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primtive_tween_sound_setter/anim_type_rotator");
            anim_type_scale = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primtive_tween_sound_setter/anim_type_scale");
            anim_type_color = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primtive_tween_sound_setter/anim_type_color");
            anim_type_fade = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primtive_tween_sound_setter/anim_type_fade");
            anim_type_writter = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primtive_tween_sound_setter/anim_type_writter");
            anim_type_fill = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primtive_tween_sound_setter/anim_type_fill");
            anim_type_size = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primtive_tween_sound_setter/anim_type_size");
            #endregion

            #region 动画列表
            ListSounds = new ReorderableList(serializedObject, sp_TweenSounds)
            {
                displayAdd = false,
                displayRemove = true,
                draggable = true,

                drawHeaderCallback = rect =>
                {

                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
#if UNITY_6000_0_OR_NEWER
                    TextClipping clipping = TextClipping.Ellipsis;
#else
    TextClipping clipping = TextClipping.Clip;
#endif

                    SerializedProperty sp_clip = sp_TweenSounds.GetArrayElementAtIndex(index).FindPropertyRelative("Sound");
                    SerializedProperty sp_clippath = sp_TweenSounds.GetArrayElementAtIndex(index).FindPropertyRelative("Path");

                    #region Audioclip
                    XGUI.ChangedCheck_Start();
                    XGUI.gui_property_field(
                        rect: new Rect(rect.width - 130, rect.y + 2.5f, 150, 15),
                        title: null,
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 0,
                        prop: sp_clip);
                    if (XGUI.ChangedCheck_End())
                    {
                        sp_clippath.stringValue = AssetDatabase.GetAssetPath(sp_clip.objectReferenceValue);
                        sp_clippath.serializedObject.ApplyModifiedProperties();
                    }
                    #endregion

                    #region 名称
                    AudioClip clip = (AudioClip)sp_clip.objectReferenceValue;
                    string clipname = "";
                    if (clip != null)
                        clipname = clip.name;
                    else
                        clipname = "未指定";

                    XGUI.gui_label(
                        rect: new Rect(rect.x + 5, rect.y + 3, rect.width - 165, 15),
                        text: new GUIContent(clipname),
                        text_color: Color.white,
                        size: XGUIFontSize.M,
                        anchor: TextAnchor.MiddleLeft,
                        font_style: FontStyle.Bold,
                        padding: new RectOffset(0, 0, 0, 0),
                        offset: new Vector2(0, 0),
                        clipping: clipping);
                    #endregion

                    float offset = 5;

                    #region 触点
                    SerializedProperty sp_per = sp_TweenSounds.GetArrayElementAtIndex(index).FindPropertyRelative("Percentage");

                    XGUI.gui_icon(
                        rect: new Rect(rect.x + 5, rect.y + offset + 25, 15, 15),
                        icon: Icon_twn_sound_percent,
                        color: Color.white * 0.6f);

                    XGUI.gui_label(
                       rect: new Rect(rect.x + 35, rect.y + offset + 22, 60, 20),
                       text: new GUIContent(sp_per.floatValue.ToString("F2") + " %"),
                       text_color: Color.white,
                       size: XGUIFontSize.M,
                       anchor: TextAnchor.MiddleLeft,
                       font_style: FontStyle.Bold,
                       padding: new RectOffset(0, 0, 0, 0),
                       offset: new Vector2(0, 0),
                       clipping: clipping);

                    sp_per.floatValue = XGUI.gui_slider(
                        rect: new Rect(rect.x + 120, rect.y + offset + 25, rect.width - 120, 20),
                        title: null,
                        title_size: XGUIFontSize.M,
                        title_anchor: TextAnchor.MiddleLeft,
                        title_color: Color.white,
                        title_width: 60,
                        //status_icon: "icon_field_status",
                        //status_icon_color: Color.red,
                        prop: sp_per.floatValue,
                        left: 0,
                        right: 100,
                        slider_height: 20,
                        limite_width: 230);

                    sp_per.serializedObject.ApplyModifiedProperties();
                    #endregion

                    offset += 25;

                    #region 音量
                    SerializedProperty sp_vol = sp_TweenSounds.GetArrayElementAtIndex(index).FindPropertyRelative("Volume");
                    float vol = sp_vol.floatValue * 100;

                    XGUI.gui_icon(
                        rect: new Rect(rect.x + 5, rect.y + offset + 25, 15, 15),
                        icon: Icon_twn_sound_volume,
                        color: Color.white * 0.6f);

                    XGUI.gui_label(
                       rect: new Rect(rect.x + 35, rect.y + offset + 22, 60, 20),
                       text: new GUIContent(vol.ToString("F0") + " %"),
                       text_color: Color.white,
                       size: XGUIFontSize.M,
                       anchor: TextAnchor.MiddleLeft,
                       font_style: FontStyle.Bold,
                       padding: new RectOffset(0, 0, 0, 0),
                       offset: new Vector2(0, 0),
                       clipping: clipping);

                    sp_vol.floatValue = XGUI.gui_slider(
                        rect: new Rect(rect.x + 120, rect.y + offset + 25, rect.width - 120, 20),
                        title: null,
                        title_size: XGUIFontSize.M,
                        title_anchor: TextAnchor.MiddleLeft,
                        title_color: Color.white,
                        title_width: 60,
                        //status_icon: "icon_field_status",
                        //status_icon_color: Color.red,
                        prop: sp_vol.floatValue,
                        left: 0,
                        right: 100,
                        slider_height: 20,
                        limite_width: 230);

                    sp_vol.serializedObject.ApplyModifiedProperties();
                    #endregion

                    offset += 25;

                    #region 音高
                    SerializedProperty sp_min_pitch = sp_TweenSounds.GetArrayElementAtIndex(index).FindPropertyRelative("MinPitch");
                    SerializedProperty sp_max_pitch = sp_TweenSounds.GetArrayElementAtIndex(index).FindPropertyRelative("MaxPitch");

                    float minpitch = sp_min_pitch.floatValue;
                    float maxpitch = sp_max_pitch.floatValue;

                    XGUI.gui_icon(
                        rect: new Rect(rect.x + 5, rect.y + offset + 25, 15, 15),
                        icon: Icon_twn_sound_pitch,
                        color: Color.white * 0.6f);

                    XGUI.gui_label(
                       rect: new Rect(rect.x + 35, rect.y + offset + 22, 60, 20),
                       text: new GUIContent(vol.ToString("F0") + " %"),
                       text_color: Color.white,
                       size: XGUIFontSize.M,
                       anchor: TextAnchor.MiddleLeft,
                       font_style: FontStyle.Bold,
                       padding: new RectOffset(0, 0, 0, 0),
                       offset: new Vector2(0, 0),
                       clipping: clipping);

                    xgui_minmax_value minmax_value = XGUI.gui_slider_min_max(
                        ref_min: ref minpitch,
                        ref_max: ref maxpitch,
                        rect: new Rect(rect.x + 120, rect.y + offset + 25, rect.width - 210, 20),
                        title: null,
                        title_size: XGUIFontSize.M,
                        title_anchor: TextAnchor.MiddleLeft,
                        title_color: Color.white,
                        title_width: 60,
                        //status_icon: "icon_field_status",
                        //status_icon_color: Color.green,
                        limite_width: 50,
                        slider_height: 20,
                        min_limite: -1,
                        max_limite: 2,
                        displaystate: false);

                    sp_min_pitch.floatValue = minmax_value.min;
                    sp_max_pitch.floatValue = minmax_value.max;

                    sp_min_pitch.floatValue = XGUI.gui_inputfield(
                        rect: new Rect(rect.width - 60, rect.y + 80, 40, 20),
                        title: null,
                        prop: sp_min_pitch.floatValue,
                        field_fontsize: XGUIFontSize.M,
                        field_text_offset: Vector2.zero,
                        field_height: 20,
                        field_text_color: Color.white,
                        title_width: 40,
                        field_text_font: XGUI.GetFont("xg-medium"),
                        field_text_style: FontStyle.Normal,
                        field_text_anchor: TextAnchor.MiddleCenter,
                        field_padding: new RectOffset(5, 5, 0, 0),
                        field_margin: new RectOffset(0, 0, 0, 0));

                    sp_max_pitch.floatValue = XGUI.gui_inputfield(
                        rect: new Rect(rect.width - 18, rect.y + 80, 40, 20),
                        title: null,
                        prop: sp_max_pitch.floatValue,
                        field_fontsize: XGUIFontSize.M,
                        field_text_offset: Vector2.zero,
                        field_height: 20,
                        field_text_color: Color.white,
                        title_width: 40,
                        field_text_font: XGUI.GetFont("xg-medium"),
                        field_text_style: FontStyle.Normal,
                        field_text_anchor: TextAnchor.MiddleCenter,
                        field_padding: new RectOffset(5, 5, 0, 0),
                        field_margin: new RectOffset(0, 0, 0, 0));

                    sp_min_pitch.serializedObject.ApplyModifiedProperties();
                    sp_max_pitch.serializedObject.ApplyModifiedProperties();
                    #endregion
                },
                onRemoveCallback = (ReorderableList list) =>
                {
                    sp_TweenSounds.DeleteArrayElementAtIndex(list.index);
                    if (sp_TweenSounds.arraySize <= 0)
                    {
                        Selected_Name = null;
                        Selected_Length = null;
                        Selected_Steo = null;
                        Selected_Hz = null;
                    }
                },
                onSelectCallback = (ReorderableList list) =>
                {
                    SerializedProperty sp_clip = sp_TweenSounds.GetArrayElementAtIndex(list.index).FindPropertyRelative("Sound");
                    SerializedProperty sp_vol = sp_TweenSounds.GetArrayElementAtIndex(list.index).FindPropertyRelative("Volume");
                    SerializedProperty sp_pit_min = sp_TweenSounds.GetArrayElementAtIndex(list.index).FindPropertyRelative("MinPitch");
                    SerializedProperty sp_pit_max = sp_TweenSounds.GetArrayElementAtIndex(list.index).FindPropertyRelative("MaxPitch");
                    AudioClip clip = (AudioClip)sp_clip.objectReferenceValue;
                    if (clip != null)
                    {
                        Selected_Name = clip.name;
                        Selected_Hz = clip.frequency.ToString() + "Hz";
                        Selected_Length = clip.length.ToString("F2") + " s";
                        if (clip.channels == 1)
                            Selected_Steo = "单声道";
                        else
                            Selected_Steo = "立体声";

                        CreatePreviewSound(clip, sp_vol.floatValue, sp_pit_min.floatValue, sp_pit_max.floatValue);
                    }
                },
                elementHeightCallback = index =>
                {
                    return LineHeight * 6.2f;
                }
            };
            #endregion
        }

        private void OnGUI()
        {
            serializedObject.Update();

#if UNITY_6000_0_OR_NEWER
            TextClipping clipping = TextClipping.Ellipsis;
#else
    TextClipping clipping = TextClipping.Clip;
#endif

            #region 标题
            XGUI.layout_group_start(
                type: XGUIContainerType.Horizontal,
                title_clipping: TextClipping.Clip,
                absolute_padding: true,
                absolute_margin: true,
                padding: new RectOffset(15, 15, 15, 5));

            XGUI.layout_state_displayer_text(
                title: string.IsNullOrEmpty(PrimitiveTweenSoundNode.Name) ? "未命名动画" : PrimitiveTweenSoundNode.Name,
                title_size: XGUIFontSize.L,
                title_color: XHud_Dashboard.Theme_Primary,
                subtitle: PrimitiveTweenSoundNode.Type.ToString(),
                subtitle_size: XGUIFontSize.M,
                subtitle_color: Color.white * 0.75f,
                margin: new RectOffset(0, 0, 0, 0));

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            #endregion

            XGUI.layout_seperator(
                      thickness: 1,
                      color: XHud_Dashboard.Theme_SeperateLine,
                      margin: new RectOffset(15, 15, 10, 20));

            #region 快捷功能
            XGUI.layout_group_start(
                type: XGUIContainerType.Horizontal,
                title_clipping: TextClipping.Clip,
                absolute_padding: true,
                absolute_margin: true,
                padding: new RectOffset(20, 20, 0, 15));

            #region 新增
            if (XGUI.layout_button(
                tooltip: "新增",
                tex_release: Logo_Add_Release,
                tex_press: Logo_Add_Press,
                tex_gui_color: Color.white,
                border: new RectOffset(0, 0, 0, 0),
                width: 14,
                height: 14))
            {
                int index = 0;
                if (sp_TweenSounds.arraySize <= 0)
                    index = 0;
                else
                    index = sp_TweenSounds.arraySize;

                sp_TweenSounds.InsertArrayElementAtIndex(index);

                SerializedProperty sp_soundinfo = sp_TweenSounds.GetArrayElementAtIndex(index);
                sp_soundinfo.FindPropertyRelative("Sound").objectReferenceValue = null;
                sp_soundinfo.FindPropertyRelative("Volume").floatValue = 1;
                sp_soundinfo.FindPropertyRelative("MaxPitch").floatValue = 1;
                sp_soundinfo.FindPropertyRelative("MinPitch").floatValue = 1;
                sp_TweenSounds.serializedObject.ApplyModifiedProperties();
            }
            #endregion

            GUILayout.FlexibleSpace();

            #region 播放
            if (XGUI.layout_button(
                tooltip: "播放",
                tex_release: Logo_Play_Release,
                tex_press: Logo_Play_Press,
                tex_gui_color: Color.white,
                border: new RectOffset(0, 0, 0, 0),
                width: 14,
                height: 14))
            {
                if (Application.isPlaying)
                {
                    Debug.Log("程序运行时无法预览音效！");
                    return;
                }
                for (int i = 0; i < PrimitiveTweenSoundNode.TweenSounds.Count; i++)
                {
                    // 计算终极耗时
                    XHud_Module_Primitive_Tween tween = PrimitiveTweenSoundNode.Tween;
                    float w = tween.GlobalDuration;
                    float a = tween.TweenNode_GetByIndex(PrimitiveTweenSoundNode.Index).Duration;
                    float s = PrimitiveTweenSoundNode.TweenSounds[i].Percentage;

                    float x = s * (w * a);

                    // 预览音效
                    AudioClip clip = PrimitiveTweenSoundNode.TweenSounds[i].Sound;
                    float vol = PrimitiveTweenSoundNode.TweenSounds[i].Volume;
                    float pit_min = PrimitiveTweenSoundNode.TweenSounds[i].MinPitch;
                    float pit_max = PrimitiveTweenSoundNode.TweenSounds[i].MaxPitch;
                    PreviewSounds(clip, x, vol, pit_min, pit_max);
                }
            }
            #endregion

            GUILayout.FlexibleSpace();

            #region 应用
            if (XGUI.layout_button(
                tooltip: "应用",
                tex_release: Logo_Apply_Release,
                tex_press: Logo_Apply_Press,
                tex_gui_color: Color.white,
                border: new RectOffset(0, 0, 0, 0),
                width: 14,
                height: 14))
            {
                #region 获取动画节点
                SerializedProperty sp_node = sp_PrimitiveTweenSoundNodes.FindPropertyRelative("Tween");
                SerializedProperty sp_index = sp_PrimitiveTweenSoundNodes.FindPropertyRelative("Index");

                XHud_Module_Primitive_Tween tween = (XHud_Module_Primitive_Tween)sp_node.objectReferenceValue;

                SerializedObject so_anim = new SerializedObject(tween);
                so_anim.Update();
                SerializedProperty sp_tweennodes = so_anim.FindProperty("PrimitiveTweenNodes");
                SerializedProperty sp_indexnode = sp_tweennodes.GetArrayElementAtIndex(sp_index.intValue);

                SerializedProperty sp_sounds = sp_indexnode.FindPropertyRelative("TweenSounds");

                sp_sounds.ClearArray();

                so_anim.ApplyModifiedProperties();
                #endregion

                for (int i = 0; i < sp_TweenSounds.arraySize; i++)
                {
                    SerializedProperty sp_sound = sp_TweenSounds.GetArrayElementAtIndex(i).FindPropertyRelative("Sound");
                    SerializedProperty sp_percent = sp_TweenSounds.GetArrayElementAtIndex(i).FindPropertyRelative("Percentage");
                    SerializedProperty sp_path = sp_TweenSounds.GetArrayElementAtIndex(i).FindPropertyRelative("Path");
                    SerializedProperty sp_vol = sp_TweenSounds.GetArrayElementAtIndex(i).FindPropertyRelative("Volume");
                    SerializedProperty sp_minpitch = sp_TweenSounds.GetArrayElementAtIndex(i).FindPropertyRelative("MinPitch");
                    SerializedProperty sp_maxpitch = sp_TweenSounds.GetArrayElementAtIndex(i).FindPropertyRelative("MaxPitch");

                    sp_sounds.InsertArrayElementAtIndex(i);
                    SerializedProperty sp_soundnode = sp_sounds.GetArrayElementAtIndex(i);
                    SerializedProperty sp_x_sound = sp_soundnode.FindPropertyRelative("Sound");
                    SerializedProperty sp_x_percent = sp_soundnode.FindPropertyRelative("Percentage");
                    SerializedProperty sp_x_path = sp_soundnode.FindPropertyRelative("Path");
                    SerializedProperty sp_x_vol = sp_soundnode.FindPropertyRelative("Volume");
                    SerializedProperty sp_x_min_pitch = sp_soundnode.FindPropertyRelative("MinPitch");
                    SerializedProperty sp_x_max_pitch = sp_soundnode.FindPropertyRelative("MaxPitch");

                    sp_x_sound.objectReferenceValue = sp_sound.objectReferenceValue;
                    sp_x_percent.floatValue = sp_percent.floatValue;
                    sp_x_path.stringValue = sp_path.stringValue;
                    sp_x_vol.floatValue = sp_vol.floatValue;
                    sp_x_min_pitch.floatValue = sp_minpitch.floatValue;
                    sp_x_max_pitch.floatValue = sp_maxpitch.floatValue;

                    sp_x_sound.serializedObject.ApplyModifiedProperties();
                    sp_x_percent.serializedObject.ApplyModifiedProperties();
                    sp_x_path.serializedObject.ApplyModifiedProperties();
                    sp_x_vol.serializedObject.ApplyModifiedProperties();
                    sp_x_min_pitch.serializedObject.ApplyModifiedProperties();
                    sp_x_max_pitch.serializedObject.ApplyModifiedProperties();
                }

                #region 判断音效库中是否存在音效，如果没有则加入
                if (HudManager.Hud_Sounds != null)
                {
                    if (PrimitiveTweenSoundNode.TweenSounds != null && PrimitiveTweenSoundNode.TweenSounds.Count > 0)
                    {
                        for (int i = 0; i < PrimitiveTweenSoundNode.TweenSounds.Count; i++)
                        {
                            if (PrimitiveTweenSoundNode.TweenSounds[i] != null)
                            {
                                if (PrimitiveTweenSoundNode.TweenSounds[i].Sound != null)
                                {
                                    AudioClip sod = PrimitiveTweenSoundNode.TweenSounds[i].Sound;
                                    int libcount = HudManager.Hud_Sounds.SoundLibrary_GetSoundCount();

                                    bool repeat = false;
                                    for (int s = 0; s < libcount; s++)
                                    {
                                        if (HudManager.Hud_Sounds.SoundLibrary_GetSound(s).name == sod.name &&
                                            HudManager.Hud_Sounds.SoundLibrary_GetSoundArg(s).Name == sod.name)
                                        {
                                            repeat = true;
                                            break;
                                        }
                                    }
                                    if (!repeat)
                                    {
                                        string sod_format = Path.GetExtension(AssetDatabase.GetAssetPath(sod)).ToLower();
                                        HudManager.Hud_Sounds.SoundLibrary_AddSound(sod.name, sod, sod_format);
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion
            }
            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            #endregion

            #region 动画类型图标
            Texture2D icon = null;
            switch (PrimitiveTweenSoundNode.Type)
            {
                case TweenNodeType.a_位移:
                    icon = anim_type_move;
                    break;
                case TweenNodeType.r_旋转:
                    icon = anim_type_rotator;
                    break;
                case TweenNodeType.s_缩放:
                    icon = anim_type_scale;
                    break;
                case TweenNodeType.c_颜色:
                    icon = anim_type_color;
                    break;
                case TweenNodeType.g_淡化:
                    icon = anim_type_fade;
                    break;
                case TweenNodeType.w_打字机:
                    icon = anim_type_writter;
                    break;
                case TweenNodeType.f_图像填充:
                    icon = anim_type_fill;
                    break;
                case TweenNodeType.z_尺寸:
                    icon = anim_type_size;
                    break;
            }

            Rect rect_icon = new Rect(position.size.x - 80, position.size.y - 80, 64, 64);
            XGUI.gui_icon(
                  rect: rect_icon,
                  icon: icon,
                  color: new Color(1, 1, 1, 0.06f));
            #endregion

            #region 列表
            XGUI.layout_group_start(
                type: XGUIContainerType.Horizontal,
                title_clipping: TextClipping.Clip,
                absolute_padding: true,
                absolute_margin: true,
                padding: new RectOffset(15, 15, 15, 100));

            Scroll = EditorGUILayout.BeginScrollView(Scroll, GUILayout.Width(position.width - 25), GUILayout.ExpandWidth(true));
            ListSounds.DoLayoutList();
            EditorGUILayout.EndScrollView();

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            #endregion

            #region 选中的音效信息
            Rect rect_info_name = new Rect(15, position.size.y - 80, position.size.x - 120, 22);
            Rect rect_info_hz = new Rect(15, position.size.y - 58, position.size.x, 22);
            Rect rect_info_channel = new Rect(15, position.size.y - 36, 70, 22);
            Rect rect_info_length = new Rect(70, position.size.y - 36, position.size.x, 22);

            XGUI.gui_label(
                rect: rect_info_name,
                text: new GUIContent(Selected_Name),
                text_color: XHud_Dashboard.Theme_Primary,
                size: XGUIFontSize.B,
                clipping: clipping,
                anchor: TextAnchor.MiddleLeft,
                offset: new Vector2(0, 0),
                wrap: true,
                font_style: FontStyle.Normal);

            XGUI.gui_label(
                rect: rect_info_hz,
                text: new GUIContent(Selected_Hz),
                text_color: Color.gray,
                size: XGUIFontSize.M,
                clipping: clipping,
                anchor: TextAnchor.MiddleLeft,
                offset: new Vector2(0, 0),
                wrap: true,
                font_style: FontStyle.Normal);

            XGUI.gui_label(
                rect: rect_info_channel,
                text: new GUIContent(Selected_Steo),
                text_color: Color.gray,
                size: XGUIFontSize.M,
                clipping: clipping,
                anchor: TextAnchor.MiddleLeft,
                offset: new Vector2(0, 0),
                wrap: true,
                font_style: FontStyle.Normal);

            XGUI.gui_label(
                rect: rect_info_length,
                text: new GUIContent(Selected_Length),
                text_color: Color.gray,
                size: XGUIFontSize.M,
                clipping: clipping,
                anchor: TextAnchor.MiddleLeft,
                offset: new Vector2(0, 0),
                wrap: true,
                font_style: FontStyle.Normal);
            #endregion

            serializedObject.ApplyModifiedProperties();

            Event e = Event.current;
            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
            {
                this.Close();
            }
        }

        private void PreviewSounds(AudioClip clip, float time, float vol, float pitch_min, float pitch_max)
        {
            Coroutine_Preview = XCoroutineUtility.xec_StartCoroutine(PreviewSound(clip, time, vol, pitch_min, pitch_max), this);
        }

        IEnumerator PreviewSound(AudioClip clip, float time, float vol, float pitch_min, float pitch_max)
        {
            yield return new XCoroutineWaitForSeconds(time);

            if (PreviewAudioList == null)
                PreviewAudioList = new List<AudioSource>();
            PreviewAudioList.Add(CreatePreviewSound(clip, vol, pitch_min, pitch_max));
        }

        /// <summary>
        /// 预览声音
        /// </summary>
        /// <param name="clip"></param>
        public AudioSource CreatePreviewSound(AudioClip clip, float vol, float pit_min, float pit_max)
        {
            GameObject obj = new GameObject();
            obj.name = "SoundPreviewer-" + "[" + clip.length.ToString("F2") + " s]-" + "[" + clip.channels + " ch]-" + "[" + clip.frequency + " hz]";
            AudioSource au = obj.AddComponent<AudioSource>();
            au.clip = clip;
            au.volume = vol;
            au.pitch = Random.Range(pit_min, pit_max);
            au.Play();
            XHud_AudioStoper sp = au.gameObject.AddComponent<XHud_AudioStoper>();
            sp.AudioSource = au;
            return au;
        }

        private void StopAllPreviewAudio()
        {
            if (Coroutine_Preview != null)
            {
                XCoroutineUtility.xec_StopCoroutine(Coroutine_Preview);
                Coroutine_Preview = null;
            }
            if (PreviewAudioList != null)
            {
                for (int i = 0; i < PreviewAudioList.Count; i++)
                {
                    if (PreviewAudioList[i] != null)
                    {
                        PreviewAudioList[i].Stop();
                        DestroyImmediate(PreviewAudioList[i].gameObject, true);
                        PreviewAudioList[i] = null;
                    }
                }
                PreviewAudioList.Clear();
            }

            SceneView.RepaintAll();
        }
    }
}
