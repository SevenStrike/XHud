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
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.Utilitys;
    using SevenStrikeModules.XTween;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using Unity.EditorCoroutines.Editor;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.UI;
    using Debug = UnityEngine.Debug;
    using Random = UnityEngine.Random;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(XHud_Module_Animator))]
    public class Editor_XHud_Module_Animator : Editor
    {
        #region 组件 / 列表
        private XHud_Module_Animator BaseScript;
        private ReorderableList AnimateTweenNodesList;
        #endregion

        #region 序列化属性
        private SerializedProperty ID, Indicator, AnimatorModuleType, eve_on_Animator_Play, eve_on_Animator_PlayAt, eve_on_Animator_Rewind, eve_on_Animator_Ready, eve_on_Animator_Initialized, eve_on_Animator_SoundPlay, eve_on_Animator_AnimatingState, DebugState, CustomValue_Int, CustomValue_Float, CustomValue_Vector2, CustomValue_Vector3, CustomValue_Vector4, CustomValue_Color, AnimateTweenNodes, Animator_GlobalDuration, mod_RectTransform, mod_Image, mod_RawImage, mod_Text, mod_TmpText, CanvasGroup, MutePlay, sp_AnimatorFeature, ColoriseName, FirstCreateRecordOriginalState, OriginalColor, SyncLibraryColor, TweenIsPreviewing, MaxTimer, MinTimer, MinTimerWithGlobalDuration, MaxTimerWithGlobalDuration, IgnoreElementAnimationPlay;
        #endregion

        private float LineHeight;
        private bool OriginalDisplay;
        private bool EventIsFold;
        Rect draw_rect;

        #region 必要组件
        private XHud_Module_Text HudText;
        private XHud_Module_TmpText HudTmpText;
        private XHud_Module_Button HudButton;
        private XHud_Module_Progress HudProgress;
        private XHud_Module_Toggle HudToggle;
        private XHud_Module_Slider HudSlider;
        private XHud_Module_Option HudOption;
        #endregion

        /// <summary>
        /// 音效设置器
        /// </summary>
        private Editor_XHud_PrimitiveTweenSoundSetTool Editor_Hud_Animator_SounderViewer;

        #region 预览动画
        private List<EditorCoroutine> Preivew_Animator_CoroutineList_Play = new List<EditorCoroutine>();
        private EditorCoroutine Preivew_Animator_Coroutine_ProgressCalcToSound;
        private EditorCoroutine Preivew_Animator_Coroutine_Stop;
        private List<XTween_Interface> Preview_Animator_TweenList = new List<XTween_Interface>();
        public List<AudioSource> Preview_Animator_SoundList = new List<AudioSource>();
        #endregion

        #region Animator特性参数数组
        public AnimatorFeatures[] AnimatorFeatures;
        #endregion

        #region 批量模式查看索引
        private int AnimatorOriginalPoserInfos_Index, AnimatorAnimationStatu_Index, AnimatorAnimationStatistic_Index;
        #endregion

        #region 字体
        /// <summary>
        /// 字体 - 粗体
        /// </summary>
        Font Font_Bold;
        /// <summary>
        /// 字体 - 细体
        /// </summary>
        Font Font_Light;
        #endregion

        #region 图标
        private Texture2D LogoState, col_lib_r, col_lib_p, col_ori_r, col_ori_p, col_mix_r, col_mix_p, prw_play_r, prw_play_p, prw_stop_r, prw_stop_p, Add_r, Add_p, locate_r, locate_p, left_arrow_r, left_arrow_p, right_arrow_r, right_arrow_p, statu, count, timer_min, timer_max, pos, rot, sca, size, alp, fill, color, anim_dir_bak, anim_dir_for, anim_dir_for_long, anim_dir_bak_p, anim_dir_for_p, anim_dir_for_long_p, anim_dot, anim_dot_dark, anim_type_mover, anim_type_rotator, anim_type_scale, anim_type_color, anim_type_fade, anim_type_writter, anim_type_fill, anim_type_size, anim_type_custom_int, anim_type_custom_float, anim_type_custom_vector2, anim_type_custom_vector3, anim_type_custom_vector4, anim_type_custom_color, icon_unfold_r, icon_unfold_p, anim_fold_r, anim_fold_p, anim_change_id_r, anim_change_id_p, anim_sound_r, anim_sound_p, anim_dir_war_r, anim_dir_war_p, icon_text, icon_tmptext, icon_image, icon_rawimage, icon_trans, icon_main;
        #endregion

        #region 选项文字
        string[] stroptions_debug = new string[2] { "关闭", "调试" }, stroptions_mute = new string[] { "正常", "静音" }, stroptions_control = new string[] { "可控", "忽略" };
        #endregion

        #region 批量化操作
        XHud_Module_Animator[] SelectedObjects;

        private void GetAllTargets()
        {
            if (targets.Length > 1)
            {
                SelectedObjects = new XHud_Module_Animator[targets.Length];
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    var t = targets[i];
                    SelectedObjects[i] = (XHud_Module_Animator)t;
                }
            }
            else
            {
                SelectedObjects = new XHud_Module_Animator[targets.Length];
                SelectedObjects[0] = (XHud_Module_Animator)target;
            }
        }

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

        private void OnEnable()
        {
            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

            BaseScript = (XHud_Module_Animator)target;

            GetAllTargets();

            #region 获取变量

            AnimateTweenNodes = serializedObject.FindProperty("AnimateTweenNodes");
            ID = serializedObject.FindProperty("ID");
            Indicator = serializedObject.FindProperty("Indicator");
            CustomValue_Int = serializedObject.FindProperty("CustomValue_Int");
            CustomValue_Float = serializedObject.FindProperty("CustomValue_Float");
            CustomValue_Vector2 = serializedObject.FindProperty("CustomValue_Vector2");
            CustomValue_Vector3 = serializedObject.FindProperty("CustomValue_Vector3");
            CustomValue_Vector4 = serializedObject.FindProperty("CustomValue_Vector4");
            CustomValue_Color = serializedObject.FindProperty("CustomValue_Color");
            mod_RectTransform = serializedObject.FindProperty("mod_RectTransform");
            mod_Image = serializedObject.FindProperty("mod_Image");
            mod_RawImage = serializedObject.FindProperty("mod_RawImage");
            mod_Text = serializedObject.FindProperty("mod_Text");
            mod_TmpText = serializedObject.FindProperty("mod_TmpText");
            FirstCreateRecordOriginalState = serializedObject.FindProperty("FirstCreateRecordOriginalState");
            ColoriseName = serializedObject.FindProperty("ColoriseName");
            OriginalColor = serializedObject.FindProperty("OriginalColor");
            MaxTimer = serializedObject.FindProperty("MaxTimer");
            MinTimer = serializedObject.FindProperty("MinTimer");
            MinTimerWithGlobalDuration = serializedObject.FindProperty("MinTimerWithGlobalDuration");
            MaxTimerWithGlobalDuration = serializedObject.FindProperty("MaxTimerWithGlobalDuration");
            SyncLibraryColor = serializedObject.FindProperty("SyncLibraryColor");
            TweenIsPreviewing = serializedObject.FindProperty("TweenIsPreviewing");
            sp_AnimatorFeature = serializedObject.FindProperty("AnimatorFeature");
            CanvasGroup = serializedObject.FindProperty("CanvasGroup");
            Animator_GlobalDuration = serializedObject.FindProperty("Animator_GlobalDuration");
            DebugState = serializedObject.FindProperty("DebugState");
            MutePlay = serializedObject.FindProperty("MutePlay");
            eve_on_Animator_Play = serializedObject.FindProperty("eve_on_Animator_Play");
            eve_on_Animator_PlayAt = serializedObject.FindProperty("eve_on_Animator_PlayAt");
            eve_on_Animator_Rewind = serializedObject.FindProperty("eve_on_Animator_Rewind");
            eve_on_Animator_Ready = serializedObject.FindProperty("eve_on_Animator_Ready");
            eve_on_Animator_Initialized = serializedObject.FindProperty("eve_on_Animator_Initialized");
            eve_on_Animator_SoundPlay = serializedObject.FindProperty("eve_on_Animator_SoundPlay");
            eve_on_Animator_AnimatingState = serializedObject.FindProperty("eve_on_Animator_AnimatingState");
            AnimatorModuleType = serializedObject.FindProperty("AnimatorModuleType");
            IgnoreElementAnimationPlay = serializedObject.FindProperty("IgnoreElementAnimationPlay");
            #endregion

            #region 获取图标
            col_lib_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/col_lib_r");
            col_lib_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/col_lib_p");
            col_ori_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/col_ori_r");
            col_ori_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/col_ori_p");
            col_mix_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/col_mix_r");
            col_mix_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/col_mix_p");
            prw_play_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/prw_play_r");
            prw_play_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/prw_play_p");
            prw_stop_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/prw_stop_r");
            prw_stop_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/prw_stop_p");
            Add_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/Add_r");
            Add_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/Add_p");
            locate_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/locate_r");
            locate_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/locate_p");
            left_arrow_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/left_arrow_r");
            left_arrow_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/left_arrow_p");
            right_arrow_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/right_arrow_r");
            right_arrow_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/right_arrow_p");
            statu = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/statu");
            count = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/count");
            timer_min = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/timer_min");
            timer_max = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/timer_max");
            pos = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/pos");
            rot = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/rot");
            sca = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/sca");
            size = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/size");
            alp = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/alp");
            fill = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/fill");
            color = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/color");
            anim_dir_bak = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/anim_dir_bak");
            anim_dir_for = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/anim_dir_for");
            anim_dir_for_long = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/anim_dir_for_long");
            anim_dot = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/anim_dot");
            anim_dot_dark = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/anim_dot_dark");
            anim_type_color = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/anim_type_color");
            anim_type_custom_color = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/anim_type_custom_color");
            anim_type_custom_float = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/anim_type_custom_float");
            anim_type_custom_int = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/anim_type_custom_int");
            anim_type_custom_vector2 = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/anim_type_custom_vector2");
            anim_type_custom_vector3 = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/anim_type_custom_vector3");
            anim_type_custom_vector4 = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/anim_type_custom_vector4");
            anim_type_fade = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/anim_type_fade");
            anim_type_fill = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/anim_type_fill");
            anim_type_mover = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/anim_type_move");
            anim_type_rotator = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/anim_type_rotator");
            anim_type_scale = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/anim_type_scale");
            anim_type_size = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/anim_type_size");
            anim_type_writter = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/anim_type_writter");
            anim_fold_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/anim_fold_r");
            anim_fold_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/anim_fold_p");
            icon_main = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/icon_main");
            icon_unfold_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/icon_unfold_r");
            icon_unfold_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/icon_unfold_p");
            anim_sound_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/anim_sound_r");
            anim_sound_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/anim_sound_p");
            anim_change_id_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/anim_change_id_r");
            anim_change_id_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/anim_change_id_p");
            icon_text = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/icon_text");
            icon_tmptext = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/icon_tmptext");
            icon_image = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/icon_image");
            icon_rawimage = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/icon_rawimage");
            icon_trans = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/icon_trans");
            anim_dir_war_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/anim_dir_war_r");
            anim_dir_war_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/anim_dir_war_p");

            anim_dir_bak_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/anim_dir_bak_p");
            anim_dir_for_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/anim_dir_for_p");
            anim_dir_for_long_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Animator/anim_dir_for_long_p");

            #endregion

            #region 获取父物体组件以判断类型
            HudText = BaseScript.GetComponentInParent<XHud_Module_Text>();
            HudTmpText = BaseScript.GetComponentInParent<XHud_Module_TmpText>();
            HudButton = BaseScript.GetComponentInParent<XHud_Module_Button>();
            HudProgress = BaseScript.GetComponentInParent<XHud_Module_Progress>();
            HudToggle = BaseScript.GetComponentInParent<XHud_Module_Toggle>();
            HudSlider = BaseScript.GetComponentInParent<XHud_Module_Slider>();
            HudOption = BaseScript.GetComponentInParent<XHud_Module_Option>();
            #endregion

            #region 识别类型并赋值组件            
            if (!Application.isPlaying)
            {
                RecognizeElementType();
            }
            #endregion

            #region 获取系统GUI单行单位高度
            LineHeight = EditorGUIUtility.singleLineHeight;
            #endregion

            #region 获取字体
            Font_Bold = Editor_XHud_GUI.GetFont("SS_Editor_Bold");
            Font_Light = Editor_XHud_GUI.GetFont("SS_Editor_Light");
            #endregion

            #region 动画列表

            AnimateTweenNodesList = new ReorderableList(serializedObject, AnimateTweenNodes)
            {
                displayAdd = true,
                displayRemove = true,
                draggable = true,
                drawHeaderCallback = rect =>
                {
                },
                drawElementBackgroundCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    if (index >= 0)
                    {
                        if (isFocused)
                            EditorGUI.DrawRect(new Rect(rect.x + (rect.width / 2) + 36, rect.y + 25, 5, 5), XHud_Dashboard.Theme_Primary);
                    }
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    SerializedProperty sp_Root = AnimateTweenNodes.GetArrayElementAtIndex(index);

                    SerializedProperty sp_id = sp_Root.FindPropertyRelative("ID");
                    SerializedProperty sp_dur = sp_Root.FindPropertyRelative("Duration");
                    SerializedProperty sp_tweensounds = sp_Root.FindPropertyRelative("TweenSounds");
                    SerializedProperty sp_delay = sp_Root.FindPropertyRelative("Delay");
                    SerializedProperty sp_Ease = sp_Root.FindPropertyRelative("Ease");
                    SerializedProperty sp_CurveName = sp_Root.FindPropertyRelative("AnimationCurveName");
                    SerializedProperty sp_Curve = sp_Root.FindPropertyRelative("Curve");
                    SerializedProperty sp_IsFold = sp_Root.FindPropertyRelative("IsFold");

                    SerializedProperty sp_Timings = sp_Root.FindPropertyRelative("Timings");
                    SerializedProperty sp_dir_from = sp_Root.FindPropertyRelative("ActivateFrom");
                    SerializedProperty sp_dir_end = sp_Root.FindPropertyRelative("ActivateEnd");
                    SerializedProperty sp_dir_onlyend = sp_Root.FindPropertyRelative("ActivateOnlyToEnd");
                    SerializedProperty sp_Cycle = sp_Root.FindPropertyRelative("LoopType");
                    SerializedProperty sp_cyclecount = sp_Root.FindPropertyRelative("LoopCount");

                    SerializedProperty sp_progress = sp_Root.FindPropertyRelative("Progress");
                    SerializedProperty sp_nodetype = sp_Root.FindPropertyRelative("Type");


                    float topheight = 10;

                    #region 开关
                    draw_rect.Set(rect.x + 30, rect.y + 8 + topheight, 85, LineHeight);
                    SerializedProperty sp_Enabled = AnimateTweenNodes.GetArrayElementAtIndex(index).FindPropertyRelative("Enabled");
                    sp_Enabled.boolValue = Editor_XHud_GUI.Gui_Toggle(draw_rect, true, null, sp_Enabled.boolValue);
                    sp_Enabled.serializedObject.ApplyModifiedProperties();
                    #endregion

                    if (!sp_Enabled.boolValue)
                    {
                        draw_rect.Set(rect.x - 8, rect.y + 12 + topheight, 1, 10);
                        Editor_XHud_GUI.Gui_Box(draw_rect, Color.red);
                    }

                    #region 名称
                    draw_rect.Set(rect.x + 55, rect.y + 7 + topheight, (rect.width / 2) - 40, LineHeight);
                    SerializedProperty sp_Name = AnimateTweenNodes.GetArrayElementAtIndex(index).FindPropertyRelative("Indicator");
                    Editor_XHud_GUI.Gui_Property_Field(draw_rect, "", sp_Name, 0, 0);
                    sp_Name.serializedObject.ApplyModifiedProperties();
                    #endregion

                    #region 类型标签

                    string[] TweenNodeTypes = System.Enum.GetNames(typeof(TweenNodeType));
                    Color bgscol = GUI.color;
                    GUI.color = XHud_Dashboard.Theme_Primary;
                    draw_rect.Set(rect.width / 2 + 90, rect.y + 8 + topheight, rect.width / 2 - 80, LineHeight);
                    sp_nodetype.intValue = Editor_XHud_GUI.Gui_Popup(draw_rect, sp_nodetype.intValue, TweenNodeTypes, HudFilled.实体, HudColor.亮白, Color.black);

                    GUI.color = bgscol;
                    sp_nodetype.serializedObject.ApplyModifiedProperties();
                    #endregion

                    #region 类型图标
                    Texture2D typeicon = null;
                    TweenNodeType nodetype = (TweenNodeType)sp_nodetype.enumValueIndex;
                    switch (nodetype)
                    {
                        case TweenNodeType.位移:
                            typeicon = anim_type_mover;
                            break;
                        case TweenNodeType.旋转:
                            typeicon = anim_type_rotator;
                            break;
                        case TweenNodeType.缩放:
                            typeicon = anim_type_scale;
                            break;
                        case TweenNodeType.颜色:
                            typeicon = anim_type_color;
                            break;
                        case TweenNodeType.淡化:
                            typeicon = anim_type_fade;
                            break;
                        case TweenNodeType.打字机:
                            typeicon = anim_type_writter;
                            break;
                        case TweenNodeType.图像填充:
                            typeicon = anim_type_fill;
                            break;
                        case TweenNodeType.尺寸:
                            typeicon = anim_type_size;
                            break;
                    }
                    GUI.color = Color.white;
                    draw_rect.Set(rect.width + 28, rect.y + 12 + topheight, 12, 12);
                    Editor_XHud_GUI.Gui_Icon(draw_rect, typeicon);
                    GUI.color = Color.white;
                    #endregion

                    float baseheight = rect.y + 80;
                    if (!sp_IsFold.boolValue)
                    {
                        draw_rect.Set(rect.x + 10, rect.y + 10 + topheight, 12, 12);
                        if (Editor_XHud_GUI.Gui_Button(draw_rect, icon_unfold_r, icon_unfold_p, true, "", "", Color.white))
                        {
                            sp_IsFold.boolValue = !sp_IsFold.boolValue;
                            sp_IsFold.serializedObject.ApplyModifiedProperties();
                        }
                    }
                    else
                    {
                        draw_rect.Set(rect.x + 10, rect.y + 10 + topheight, 12, 12);
                        if (Editor_XHud_GUI.Gui_Button(draw_rect, anim_fold_r, anim_fold_p, true, "", "", Color.white))
                        {
                            sp_IsFold.boolValue = !sp_IsFold.boolValue;
                            sp_IsFold.serializedObject.ApplyModifiedProperties();
                        }
                    }
                    if (!sp_IsFold.boolValue)
                    {
                        #region 判断文字组件是否为库同步样式状态
                        bool IsTextColorMode = false;
                        ModuleType animtype = (ModuleType)AnimatorModuleType.enumValueIndex;
                        if (nodetype == TweenNodeType.颜色)
                        {
                            if (animtype == Enums.ModuleType.Text || animtype == Enums.ModuleType.TmpText)
                            {
                                XHud_Module_Text text = (XHud_Module_Text)mod_Text.objectReferenceValue;
                                XHud_Module_TmpText tmptext = (XHud_Module_TmpText)mod_TmpText.objectReferenceValue;
                                if ((text && text.StyleLibSynching && text.TextStyleInfo.LibStyle_Effect_color) ||
                                (tmptext && tmptext.StyleLibSynching && tmptext.TextStyleInfo.LibStyle_Effect_color))
                                {
                                    IsTextColorMode = true;
                                }
                            }
                        }
                        #endregion
                        if (!IsTextColorMode)
                        {
                            #region ID
                            Color bgcol = GUI.color;
                            GUI.color = XHud_Dashboard.Theme_Primary;
                            draw_rect.Set(rect.x + 5, baseheight - 28f, 100, 15);
                            Editor_XHud_GUI.Gui_Labelfield(draw_rect, "ID： " + sp_id.intValue, HudFilled.无, HudColor.无, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 11);
                            GUI.color = bgcol;
                            draw_rect.Set(rect.width + 25, baseheight - 28f, 15, 15);
                            if (Editor_XHud_GUI.Gui_Button(draw_rect, anim_change_id_r, anim_change_id_p, true, "", "", Color.white))
                            {
                                int id = BaseScript.TweenNode_ID_Create();
                                sp_id.intValue = id;
                                sp_id.serializedObject.ApplyModifiedProperties();
                            }
                            #endregion

                            #region 耗时
                            draw_rect.Set(rect.x, baseheight, rect.width / 2 - 10, LineHeight);
                            Editor_XHud_GUI.Gui_Property_Field(draw_rect, "耗时", sp_dur, 5, 30);
                            #endregion

                            #region 延迟
                            draw_rect.Set(rect.width / 2 + 50, baseheight, rect.width / 2 - 10, LineHeight);
                            Editor_XHud_GUI.Gui_Property_Field(draw_rect, "延迟", sp_delay, 5, 30);
                            sp_delay.serializedObject.ApplyModifiedProperties();
                            #endregion

                            #region 缓动
                            if ((EaseMode)sp_Ease.enumValueIndex == EaseMode.None)
                            {
                                draw_rect.Set(rect.x, baseheight + 30, rect.width / 2 + 20, LineHeight);
                                Editor_XHud_GUI.Gui_Property_Field(draw_rect, "缓动", sp_Ease, 5, 30);
                            }
                            else
                            {
                                draw_rect.Set(rect.x, baseheight + 30, rect.width - 10, LineHeight);
                                Editor_XHud_GUI.Gui_Property_Field(draw_rect, "缓动", sp_Ease, 5, 30);
                            }
                            sp_Ease.serializedObject.ApplyModifiedProperties();
                            #endregion

                            float hh = -30;

                            #region 曲线
                            if (mgr != null)
                            {
                                if (mgr.Hud_Curves != null && !mgr.Hud_Curves.CurveLibrary_IsEmpty() && (EaseMode)sp_Ease.enumValueIndex == EaseMode.None)
                                {
                                    hh = 0;
                                    string[] names = mgr.Hud_Curves.CurveLibrary_GetCurveNames();
                                    EditorGUI.BeginChangeCheck();
                                    draw_rect.Set(rect.width / 2 + 80, baseheight + 31, rect.width / 2 - 34, 17);
                                    Editor_XHud_GUI.Gui_PopupWithString(draw_rect, ref sp_CurveName, names, HudFilled.实体, HudColor.亮白, Color.black);
                                    sp_CurveName.serializedObject.ApplyModifiedProperties();
                                    if (!mgr.Hud_Curves.CurvesLibrary_NameIsValid(sp_CurveName.stringValue))
                                    {
                                        sp_CurveName.stringValue = "";
                                    }
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        sp_Curve.animationCurveValue = mgr.Hud_Curves.CurveLibrary_GetCurve(sp_CurveName.stringValue);
                                    }
                                    draw_rect.Set(rect.x, baseheight + 60, rect.width - 10, LineHeight);
                                    Editor_XHud_GUI.Gui_Property_Field(draw_rect, "曲线", sp_Curve, 5, 30);

                                    sp_Curve.serializedObject.ApplyModifiedProperties();
                                    sp_CurveName.serializedObject.ApplyModifiedProperties();
                                }
                            }
                            #endregion

                            #region 时机
                            string[] TimingType = null;

                            if (HudButton != null)
                            {
                                TimingType = new string[9] { "无", "鼠标进入", "鼠标退出", "鼠标按下", "鼠标松开", "鼠标长按", "鼠标点击", "鼠标选中", "鼠标取消选中" };
                            }
                            else if (HudProgress != null)
                            {
                                TimingType = new string[5] { "无", "进度开始时", "进度变化时", "进度结束时", "进度变化中" };
                            }
                            else if (HudToggle != null)
                            {
                                TimingType = new string[7] { "无", "开关打开时", "开关关闭时", "开关按下时", "开关抬起时", "开关变化时", "开关变化中" };
                            }
                            else if (HudSlider != null)
                            {
                                TimingType = new string[5] { "无", "按下滑动条", "松开滑动条", "滑动条数值改变", "滑动条数值变化中" };
                            }
                            else if (HudOption != null)
                            {
                                TimingType = new string[5] { "无", "点击选项", "光标移动开始", "光标移动结束", "光标位置改变" };
                            }
                            else
                            {
                                TimingType = new string[4] { "元素进入时", "元素进入后", "元素退出时", "自定义" };

                            }

                            draw_rect.Set(rect.x + 5, baseheight + 90 + hh, 30, LineHeight);
                            Editor_XHud_GUI.Gui_Labelfield_Thin(draw_rect, "时机", HudFilled.无, HudColor.无, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 11);
                            Color bgstcol = GUI.color;
                            GUI.color = XHud_Dashboard.Theme_Primary;
                            draw_rect.Set(rect.x + 35, baseheight + 90 + hh, rect.width / 2 - 40, LineHeight);

                            Editor_XHud_GUI.Gui_PopupWithString(draw_rect, ref sp_Timings, TimingType, HudFilled.实体, HudColor.亮白, Color.black);
                            GUI.color = bgstcol;
                            #endregion

                            #region 动向
                            draw_rect.Set(rect.x + rect.width / 2 + 3, baseheight + 90 + hh, 30, LineHeight);
                            Editor_XHud_GUI.Gui_Labelfield_Thin(draw_rect, "动向", HudFilled.无, HudColor.无, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 11);
                            int dir_index = 0;

                            if (sp_dir_from.boolValue && !sp_dir_end.boolValue && !sp_dir_onlyend.boolValue)
                            {
                                dir_index = 0;
                            }
                            if (!sp_dir_from.boolValue && sp_dir_end.boolValue && !sp_dir_onlyend.boolValue)
                            {
                                dir_index = 1;
                            }
                            if (sp_dir_from.boolValue && sp_dir_end.boolValue && !sp_dir_onlyend.boolValue)
                            {
                                dir_index = 2;
                            }
                            if (!sp_dir_from.boolValue && !sp_dir_end.boolValue && sp_dir_onlyend.boolValue)
                            {
                                dir_index = 3;
                            }

                            string[] directionTexts = new string[4] { "起始 -> 默认", "默认 -> 结束", "起始 -> 结束", "当前 -> 结束" };

                            Color bgscol_dir = GUI.color;
                            GUI.color = XHud_Dashboard.Theme_Primary;
                            draw_rect.Set(rect.width / 2 + 83, baseheight + 90 + hh, rect.width / 2 - 70, LineHeight);
                            dir_index = Editor_XHud_GUI.Gui_Popup(draw_rect, dir_index, directionTexts, HudFilled.实体, HudColor.亮白, Color.black);
                            GUI.color = bgscol_dir;

                            if (dir_index == 0)
                            {
                                sp_dir_from.boolValue = true;
                                sp_dir_end.boolValue = false;
                                sp_dir_onlyend.boolValue = false;
                            }
                            if (dir_index == 1)
                            {
                                sp_dir_from.boolValue = false;
                                sp_dir_end.boolValue = true;
                                sp_dir_onlyend.boolValue = false;
                            }
                            if (dir_index == 2)
                            {
                                sp_dir_from.boolValue = true;
                                sp_dir_end.boolValue = true;
                                sp_dir_onlyend.boolValue = false;
                            }
                            if (dir_index == 3)
                            {
                                sp_dir_from.boolValue = false;
                                sp_dir_end.boolValue = false;
                                sp_dir_onlyend.boolValue = true;
                            }
                            sp_dir_from.serializedObject.ApplyModifiedProperties();
                            sp_dir_end.serializedObject.ApplyModifiedProperties();
                            sp_dir_onlyend.serializedObject.ApplyModifiedProperties();
                            #endregion

                            #region 动向使用警告
                            draw_rect.Set(rect.width + 23, baseheight + 91 + hh, 14, 14);
                            if (Editor_XHud_GUI.Gui_Button(draw_rect, anim_dir_war_r, anim_dir_war_p, true, "", "", Color.white))
                            {
                                string hexcol = XHud_Utilitys.Color_To_HexColor(XHud_Dashboard.Theme_Primary, true);
                                string nav = "";
                                switch (dir_index)
                                {
                                    case 0:
                                        nav = $"该动向模式是指一个<color={hexcol}>  目标值  </color>从<color={hexcol}>  起始值  </color>开始向<color={hexcol}>  默认值  </color>进行变化，在该动向模式下如果<color={hexcol}>  调用Rewind方法  </color>则会让<color={hexcol}>  目标值  </color>回退到<color={hexcol}>  起始值  </color>的状态！\n\n<color={hexcol}>变化方向：</color>起始值  -->  默认值\n<color={hexcol}>适用场景：</color>既定数值的变化状态";
                                        break;
                                    case 1:
                                        nav = $"该动向模式是指一个<color={hexcol}>  目标值  <color>从<color={hexcol}>  默认值  <color>开始向<color={hexcol}>  结束值  <color>进行变化，在该动向模式下如果<color={hexcol}>  调用Rewind方法  <color>则会让<color={hexcol}>  目标值  <color>回退到<color={hexcol}>  默认值  <color>的状态！\n\n<color={hexcol}>变化方向：</color>默认值  -->  结束值\n<color={hexcol}>适用场景：</color>既定数值的变化状态";
                                        break;
                                    case 2:
                                        nav = $"该动向模式是指一个<color={hexcol}>  目标值  <color>从<color={hexcol}>  起始值  <color>开始向<color={hexcol}>  结束值  <color>进行变化，在该动向模式下如果<color={hexcol}>  调用Rewind方法  <color>则会让<color={hexcol}>  目标值  <color>回退到<color={hexcol}>  起始值  <color>的状态！\n\n<color={hexcol}>变化方向：起始值  -->  结束值\n适用场景：既定数值的变化状态";
                                        break;
                                    case 3:
                                        nav = $"该动向模式较为特殊，是指一个<color={hexcol}>  目标值  <color>从<color={hexcol}>  自身当前值  <color>开始向<color={hexcol}>  结束值  <color>进行变化，在该动向模式下如果<color={hexcol}>  调用Rewind方法  <color>则不会影响任何数值变化的状态！\n\n<color={hexcol}>变化方向：</color>自身当前值  -->  结束值\n<color={hexcol}>适用场景：</color>动态指定数值的变化状态";
                                        break;
                                }
                                Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 动画器消息", "动向模式解释", nav, "明白", 0, false);

                                return;
                            }
                            #endregion

                            #region 循环
                            draw_rect.Set(rect.x, baseheight + 120 + hh, rect.width / 2 + 20, LineHeight);
                            Editor_XHud_GUI.Gui_Property_Field(draw_rect, "循环", sp_Cycle, 5, 30);
                            #endregion

                            #region 循环次数
                            draw_rect.Set(rect.width / 2 + 78, baseheight + 120 + hh, rect.width / 2 - 38, LineHeight);
                            Editor_XHud_GUI.Gui_Property_Field(draw_rect, "次数", sp_cyclecount, 5, 30);
                            #endregion

                            #region 音效
                            if (sp_cyclecount.intValue != -1)
                            {
                                draw_rect.Set(rect.width - 10, baseheight - 29, 15, 15);
                                if (Editor_XHud_GUI.Gui_Button(draw_rect, anim_sound_r, anim_sound_p, true, "", "", Color.white))
                                {
                                    Editor_Hud_Animator_SounderViewer = (Editor_XHud_PrimitiveTweenSoundSetTool)EditorWindow.GetWindow(typeof(Editor_XHud_PrimitiveTweenSoundSetTool), false, "Hud动画器节点音效设置器", true);
                                    Editor_Hud_Animator_SounderViewer.minSize = new Vector2(360, 500);
                                    Editor_Hud_Animator_SounderViewer.maxSize = Editor_Hud_Animator_SounderViewer.minSize;
                                    Editor_Hud_Animator_SounderViewer.Show();
                                    PrimitiveTweenSoundNode node = new PrimitiveTweenSoundNode();
                                    //node.Tween = BaseScript;
                                    node.Index = index;
                                    node.Name = sp_Name.stringValue;
                                    node.Type = nodetype;
                                    node.MaxDuration = (sp_dur.floatValue * Animator_GlobalDuration.floatValue) + sp_delay.floatValue;
                                    node.TweenSounds = new List<TweenSound>();
                                    for (int i = 0; i < sp_tweensounds.arraySize; i++)
                                    {
                                        TweenSound ts = new TweenSound();
                                        ts.Sound = (AudioClip)sp_tweensounds.GetArrayElementAtIndex(i).FindPropertyRelative("Sound").objectReferenceValue;
                                        ts.Percentage = sp_tweensounds.GetArrayElementAtIndex(i).FindPropertyRelative("Percentage").floatValue;
                                        ts.Volume = sp_tweensounds.GetArrayElementAtIndex(i).FindPropertyRelative("Volume").floatValue;
                                        ts.MaxPitch = sp_tweensounds.GetArrayElementAtIndex(i).FindPropertyRelative("MaxPitch").floatValue;
                                        ts.MinPitch = sp_tweensounds.GetArrayElementAtIndex(i).FindPropertyRelative("MinPitch").floatValue;
                                        node.TweenSounds.Add(ts);
                                    }
                                    Editor_Hud_Animator_SounderViewer.PrimitiveTweenSoundNode = node;
                                    Editor_Hud_Animator_SounderViewer.Repaint();
                                }
                            }
                            GUI.color = Color.white;
                            #endregion

                            GUI.backgroundColor = new Color(255, 255, 255, 0.3f);
                            draw_rect.Set(rect.x, baseheight + 150 + hh, rect.width, 1);
                            GUI.Box(draw_rect, "");
                            GUI.backgroundColor = Color.white;

                            Rect rect_valuepanel = new Rect(draw_rect.x, draw_rect.y + 10, rect.width, 110);

                            #region 分类
                            switch (nodetype)
                            {
                                case TweenNodeType.位移:
                                    SerializedProperty sp_pos_ori = sp_Root.FindPropertyRelative("Original_Vector3");
                                    SerializedProperty sp_pos_from = sp_Root.FindPropertyRelative("From_Vector3");
                                    SerializedProperty sp_pos_end = sp_Root.FindPropertyRelative("End_Vector3");
                                    Param_Transform(rect_valuepanel, dir_index, sp_pos_ori, sp_pos_from, sp_pos_end, nodetype, sp_dir_onlyend.boolValue);
                                    break;
                                case TweenNodeType.旋转:
                                    SerializedProperty sp_rot_ori = sp_Root.FindPropertyRelative("Original_Vector3");
                                    SerializedProperty sp_rot_from = sp_Root.FindPropertyRelative("From_Vector3");
                                    SerializedProperty sp_rot_end = sp_Root.FindPropertyRelative("End_Vector3");
                                    Param_Transform(rect_valuepanel, dir_index, sp_rot_ori, sp_rot_from, sp_rot_end, nodetype, sp_dir_onlyend.boolValue);
                                    break;
                                case TweenNodeType.缩放:
                                    SerializedProperty sp_scale_ori = sp_Root.FindPropertyRelative("Original_Vector3");
                                    SerializedProperty sp_scale_from = sp_Root.FindPropertyRelative("From_Vector3");
                                    SerializedProperty sp_scale_end = sp_Root.FindPropertyRelative("End_Vector3");
                                    Param_Transform(rect_valuepanel, dir_index, sp_scale_ori, sp_scale_from, sp_scale_end, nodetype, sp_dir_onlyend.boolValue);
                                    break;
                                case TweenNodeType.颜色:
                                    SerializedProperty sp_color_ori = sp_Root.FindPropertyRelative("Original_Color");
                                    SerializedProperty sp_color_from = sp_Root.FindPropertyRelative("From_Color");
                                    SerializedProperty sp_color_end = sp_Root.FindPropertyRelative("End_Color");
                                    Param_Color(rect_valuepanel, dir_index, sp_color_ori, sp_color_from, sp_color_end, nodetype, sp_dir_onlyend.boolValue);
                                    break;
                                case TweenNodeType.淡化:
                                    SerializedProperty sp_fade_ori = sp_Root.FindPropertyRelative("Original_Float");
                                    SerializedProperty sp_fade_from = sp_Root.FindPropertyRelative("From_Float");
                                    SerializedProperty sp_fade_end = sp_Root.FindPropertyRelative("End_Float");
                                    Param_Fader(rect_valuepanel, dir_index, sp_fade_ori, sp_fade_from, sp_fade_end, nodetype, sp_dir_onlyend.boolValue);
                                    break;
                                case TweenNodeType.打字机:
                                    SerializedProperty sp_text_ori = sp_Root.FindPropertyRelative("Original_String");
                                    SerializedProperty sp_text_from = sp_Root.FindPropertyRelative("From_String");
                                    SerializedProperty sp_text_end = sp_Root.FindPropertyRelative("End_String");
                                    Param_TextWritter(rect_valuepanel, dir_index, sp_text_ori, sp_text_from, sp_text_end, nodetype, sp_dir_onlyend.boolValue);
                                    break;
                                case TweenNodeType.图像填充:
                                    SerializedProperty sp_fill_ori = sp_Root.FindPropertyRelative("Original_Float");
                                    SerializedProperty sp_fill_from = sp_Root.FindPropertyRelative("From_Float");
                                    SerializedProperty sp_fill_end = sp_Root.FindPropertyRelative("End_Float");
                                    Param_Fill(rect_valuepanel, dir_index, sp_fill_ori, sp_fill_from, sp_fill_end, nodetype, sp_dir_onlyend.boolValue);
                                    break;
                                case TweenNodeType.尺寸:
                                    SerializedProperty sp_size_ori = sp_Root.FindPropertyRelative("Original_Vector2");
                                    SerializedProperty sp_size_from = sp_Root.FindPropertyRelative("From_Vector2");
                                    SerializedProperty sp_size_end = sp_Root.FindPropertyRelative("End_Vector2");
                                    Param_Size(rect_valuepanel, dir_index, sp_size_ori, sp_size_from, sp_size_end, nodetype, sp_dir_onlyend.boolValue);
                                    break;
                            }
                            #endregion
                        }
                        else
                        {
                            #region 文字变色不支持提示
                            draw_rect.Set(rect.x, baseheight - 36f, rect.width, 35);
                            EditorGUI.HelpBox(draw_rect, "当文字使用  \"库样式同步\" 状态时不支持颜色动画", MessageType.Warning);
                            #endregion
                        }
                    }

                    #region 动画进度条
                    draw_rect.Set(rect.x + 10, rect.y + 6, (rect.width - 20), 1);
                    EditorGUI.DrawRect(draw_rect, Editor_XHud_GUI.GetColor(HudColor.深空灰));
                    draw_rect.Set(rect.x + 10, rect.y + 6, (rect.width - 20) * sp_progress.floatValue, 1);
                    EditorGUI.DrawRect(draw_rect, Editor_XHud_GUI.GetColor(HudColor.工业蓝));
                    ///---音效触发点
                    for (int p = 0; p < sp_tweensounds.arraySize; p++)
                    {
                        SerializedProperty sp_soundnode = sp_tweensounds.GetArrayElementAtIndex(p);
                        SerializedProperty sp_soundper = sp_soundnode.FindPropertyRelative("Percentage");
                        draw_rect.Set(rect.x + 10 + ((rect.width - 20) * sp_soundper.floatValue), rect.y + 6, 3, 3);
                        EditorGUI.DrawRect(draw_rect, Editor_XHud_GUI.GetColor(HudColor.亮金色));
                    }
                    draw_rect.Set(rect.x + 10 + ((rect.width - 20) * sp_progress.floatValue), rect.y + 3, 2, 8);
                    EditorGUI.DrawRect(draw_rect, Editor_XHud_GUI.GetColor(HudColor.亮白));
                    #endregion

                    sp_Root.serializedObject.ApplyModifiedProperties();

                    AnimateTweenNodes.serializedObject.ApplyModifiedProperties();
                },
                onAddCallback = (ReorderableList list) =>
                {
                    AnimateTweenNodes.InsertArrayElementAtIndex(list.count);

                    SerializedProperty sp_Root = AnimateTweenNodes.GetArrayElementAtIndex(list.count - 1);

                    sp_Root.FindPropertyRelative("Indicator").stringValue = "NewTween";
                    sp_Root.FindPropertyRelative("ID").intValue = BaseScript.TweenNode_ID_Create();
                    sp_Root.FindPropertyRelative("Type").enumValueIndex = 0;
                    sp_Root.FindPropertyRelative("Enabled").boolValue = true;
                    sp_Root.FindPropertyRelative("Timings").stringValue = "无";
                    sp_Root.FindPropertyRelative("Duration").floatValue = 1;
                    sp_Root.FindPropertyRelative("Ease").enumValueIndex = 12;
                    sp_Root.FindPropertyRelative("Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
                    sp_Root.FindPropertyRelative("Original_Color").colorValue = Color.white;
                    sp_Root.FindPropertyRelative("From_Color").colorValue = Color.white;
                    sp_Root.FindPropertyRelative("End_Color").colorValue = Color.white;
                    sp_Root.FindPropertyRelative("IsFold").boolValue = false;
                    sp_Root.FindPropertyRelative("TweenSounds").ClearArray();
                    sp_Root.FindPropertyRelative("ActivateFrom").boolValue = true;
                    sp_Root.FindPropertyRelative("ActivateEnd").boolValue = false;
                    sp_Root.FindPropertyRelative("ActivateOnlyToEnd").boolValue = false;
                    sp_Root.FindPropertyRelative("LoopType").enumValueIndex = (int)XTween_LoopType.Restart;
                    sp_Root.FindPropertyRelative("LoopCount").intValue = 0;
                    sp_Root.FindPropertyRelative("Progress").floatValue = 0;
                    sp_Root.serializedObject.ApplyModifiedProperties();
                },
                onRemoveCallback = (ReorderableList list) =>
                {
                    AnimateTweenNodes.DeleteArrayElementAtIndex(list.index);
                    AnimateTweenNodes.serializedObject.ApplyModifiedProperties();
                },
                elementHeightCallback = index =>
                {
                    SerializedProperty sp_Root = AnimateTweenNodes.GetArrayElementAtIndex(index);
                    SerializedProperty sp_type = sp_Root.FindPropertyRelative("Type");
                    var types = Enum.GetValues(typeof(TweenNodeType)).GetValue(sp_type.enumValueIndex);
                    SerializedProperty sp_Ease = sp_Root.FindPropertyRelative("Ease");
                    SerializedProperty sp_IsFold = sp_Root.FindPropertyRelative("IsFold");
                    SerializedProperty sp_dir_onlyend = sp_Root.FindPropertyRelative("ActivateOnlyToEnd");

                    float height = 3;
                    float add = 0;

                    if ((EaseMode)sp_Ease.enumValueIndex == EaseMode.None)
                    {
                        add = 1.2f;
                    }
                    else
                    {
                        add = -0.3f;
                    }

                    #region 分类
                    switch (types)
                    {
                        case TweenNodeType.位移:
                            if (sp_dir_onlyend.boolValue)
                                height = 13.5f + add;
                            else
                                height = 16.2f + add;
                            break;
                        case TweenNodeType.旋转:
                            if (sp_dir_onlyend.boolValue)
                                height = 13.5f + add;
                            else
                                height = 16.2f + add;
                            break;
                        case TweenNodeType.缩放:
                            if (sp_dir_onlyend.boolValue)
                                height = 13.5f + add;
                            else
                                height = 16.2f + add;
                            break;
                        case TweenNodeType.颜色:
                            bool IsTextColorMode = false;

                            #region 判断文字组件是否为库同步样式状态
                            ModuleType animtype = (ModuleType)AnimatorModuleType.enumValueIndex;
                            if (animtype == ModuleType.Text || animtype == ModuleType.TmpText)
                            {
                                XHud_Module_Text text = (XHud_Module_Text)mod_Text.objectReferenceValue;
                                XHud_Module_TmpText tmptext = (XHud_Module_TmpText)mod_TmpText.objectReferenceValue;
                                if ((text && text.StyleLibSynching && text.TextStyleInfo.LibStyle_Effect_color) ||
                                (tmptext && tmptext.StyleLibSynching && tmptext.TextStyleInfo.LibStyle_Effect_color))
                                {
                                    IsTextColorMode = true;
                                }
                            }
                            #endregion

                            if (IsTextColorMode)
                            {
                                height = 5f + add;
                            }
                            else
                            {
                                if (sp_dir_onlyend.boolValue)
                                    height = 13.5f + add;
                                else
                                    height = 16.2f + add;
                            }
                            break;
                        case TweenNodeType.淡化:
                            if (sp_dir_onlyend.boolValue)
                                height = 13.5f + add;
                            else
                                height = 16.2f + add;
                            break;
                        case TweenNodeType.打字机:
                            if (sp_dir_onlyend.boolValue)
                                height = 13.5f + add;
                            else
                                height = 16.2f + add;
                            break;
                        case TweenNodeType.图像填充:
                            if (sp_dir_onlyend.boolValue)
                                height = 13.5f + add;
                            else
                                height = 16.2f + add;
                            break;
                        case TweenNodeType.尺寸:
                            if (sp_dir_onlyend.boolValue)
                                height = 13.5f + add;
                            else
                                height = 16.2f + add;
                            break;
                    }
                    #endregion

                    if (sp_IsFold.boolValue)
                    {
                        height = 2.6f;
                    }
                    return height * LineHeight;
                }
            };
            #endregion

            #region 检测首次初始状态 （包含首次姿态信息记录）（此方法只有第一次创建激活动画器时才会调用）
            CheckFirstCreatedAnimator();
            #endregion

            AnimatorFeatures_Capture(SelectedObjects);
        }

        private void OnDisable()
        {
            if (!Application.isPlaying)
            {
                //--此方法内包含了原始形态读取功能 （将形态信息恢复到动画器）
                Preview_Animator_Stop(TweenIsPreviewing.boolValue);

                #region 记录原始形态（将形态信息记录下来）
                if (Targets_Selected())
                {
                    for (int i = 0; i < SelectedObjects.Length; i++)
                    {
                        if (SelectedObjects[i] != null)
                            AnimatorFeatures_Save(SelectedObjects[i]);
                    }
                }
                else
                {
                    if (target != null)
                    {
                        AnimatorFeatures_Save(BaseScript);
                        serializedObject.ApplyModifiedProperties();
                    }
                }
                #endregion

                if (Editor_Hud_Animator_SounderViewer != null)
                    Editor_Hud_Animator_SounderViewer.Close();
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            #region 类型识别
            int animtype = 0;
            if (mod_Image.objectReferenceValue != null)
            {
                LogoState = icon_image;
                animtype = (int)ModuleType.Image;
            }
            else if (mod_RawImage.objectReferenceValue != null)
            {
                LogoState = icon_rawimage;
                animtype = (int)ModuleType.RawImage;
            }
            else if (mod_Text.objectReferenceValue != null)
            {
                LogoState = icon_text;
                animtype = (int)ModuleType.Text;
            }
            else if (mod_TmpText.objectReferenceValue != null)
            {
                LogoState = icon_tmptext;
                animtype = (int)ModuleType.TmpText;
            }
            else
            {
                LogoState = icon_trans;
                animtype = (int)ModuleType.RectTransform;
            }
            AnimatorModuleType.enumValueIndex = animtype;
            #endregion

            ModuleType type = (ModuleType)AnimatorModuleType.enumValueIndex;

            #region 标题
            string titlename = "";
            if (string.IsNullOrEmpty(Indicator.stringValue))
                titlename = "Hud - 动画器";
            else
                titlename = Indicator.stringValue;
            Editor_XHud_GUI.Gui_Layout_Banner(icon_main, HudFilled.实体, HudColor.深空灰, titlename, Color.white, LogoState, type.ToString(), 20, 20);
            Rect rect = GUILayoutUtility.GetLastRect();
            #endregion

            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

            bool AllModeSame = true;
            bool firstmode = SelectedObjects[0].SyncLibraryColor;
            if (Targets_Selected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    if (SelectedObjects[i].SyncLibraryColor != firstmode)
                    {
                        AllModeSame = false;
                        break;
                    }
                }
            }

            Editor_XHud_GUI.Gui_Layout_Space(5);

            #region 快捷功能
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 0, "快捷功能", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            #region 快捷按钮
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);

            #region 默认配色 / 色卡库的模式切换按钮
            if (type != ModuleType.RectTransform)
            {
                Texture2D icon_r = null;
                Texture2D icon_p = null;

                #region 根据模式类型显示不同的内容与图标

                if (AllModeSame)
                {
                    if (firstmode)
                    {
                        icon_r = col_lib_r;
                        icon_p = col_lib_p;
                    }
                    else
                    {
                        icon_r = col_ori_r;
                        icon_p = col_ori_p;
                    }
                }
                else
                {
                    icon_r = col_mix_r;
                    icon_p = col_mix_p;
                }

                #endregion

                ///---切换模式按钮
                if (Editor_XHud_GUI.Gui_Layout_Button(14, "", icon_r, icon_p, 4))
                {
                    if (TweenIsPreviewing.boolValue)
                    {
                        Debug.Log("动画预览中不允许记录初始状态！因为这样做会破坏动画的结构！");
                        return;
                    }

                    if (Targets_Selected())
                    {
                        List<XHud_GUI_Dialog_ListDatas> Datas = new List<XHud_GUI_Dialog_ListDatas>();

                        for (int i = 0; i < SelectedObjects.Length; i++)
                        {
                            XHud_GUI_Dialog_ListDatas data = new XHud_GUI_Dialog_ListDatas();
                            data.Title = string.IsNullOrEmpty(SelectedObjects[i].Indicator) ? SelectedObjects[i].name : SelectedObjects[i].Indicator;
                            data.SubTitle = "当前颜色模式";
                            data.Message = SelectedObjects[i].SyncLibraryColor ? "色卡库" : "原始色";
                            Datas.Add(data);
                        }

                        string res_x = Editor_XHud_GUI.Open(Datas.ToArray(), XHud_DialogType.警告, "XHud - 动画器消息", "批量切换颜色模式", "是否需要批量切换以下列表中的动画器物体的颜色模式？", "暂不", "切换", 0);
                        if (res_x == "切换")
                        {
                            string res_y = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 动画器消息", "切换颜色模式", "需要切换为那种模式？", "色卡库", "原始色", 1);
                            if (res_y == "原始色")
                            {
                                for (int i = 0; i < SelectedObjects.Length; i++)
                                {
                                    SelectedObjects[i].SyncLibraryColor = false;
                                }
                            }
                            else
                            {
                                for (int i = 0; i < SelectedObjects.Length; i++)
                                {
                                    SelectedObjects[i].SyncLibraryColor = true;

                                    if (!Application.isPlaying)
                                    {
                                        //--检查色卡名称是否失效
                                        if (!mgr.Hud_Colors.ColorsLibrary_IsExist(SelectedObjects[i].ColoriseName) || string.IsNullOrEmpty(SelectedObjects[i].ColoriseName))
                                        {
                                            SelectedObjects[i].ColoriseName = mgr.Hud_Colors.ColorsLibrary_GetColorName(0);
                                        }
                                    }
                                    else
                                    {
                                        //--检查色卡名称是否失效
                                        if (!mgr.Hud_Colors.ColorsLibrary_IsExist(SelectedObjects[i].ColoriseName) || string.IsNullOrEmpty(SelectedObjects[i].ColoriseName))
                                        {
                                            SelectedObjects[i].ColoriseName = mgr.Hud_Colors.ColorsLibrary_GetColorName(0);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 动画器消息", "切换颜色模式", $"需要将 {(string.IsNullOrEmpty(BaseScript.Indicator) ? BaseScript.name : BaseScript.Indicator)} 颜色显示切换为那种模式？", "色卡库", "原始色", 1);
                        if (res == "原始色")
                        {
                            SyncLibraryColor.boolValue = false;
                            SyncLibraryColor.serializedObject.ApplyModifiedProperties();
                        }
                        else
                        {
                            SyncLibraryColor.boolValue = true;
                            SyncLibraryColor.serializedObject.ApplyModifiedProperties();

                            if (!Application.isPlaying)
                            {
                                //--检查色卡名称是否失效
                                if (!mgr.Hud_Colors.ColorsLibrary_IsExist(ColoriseName.stringValue) || string.IsNullOrEmpty(ColoriseName.stringValue))
                                {
                                    ColoriseName.stringValue = mgr.Hud_Colors.ColorsLibrary_GetColorName(0);
                                    ColoriseName.serializedObject.ApplyModifiedProperties();
                                }
                            }
                            else
                            {
                                //--检查色卡名称是否失效
                                if (!mgr.Hud_Colors.ColorsLibrary_IsExist(ColoriseName.stringValue) || string.IsNullOrEmpty(ColoriseName.stringValue))
                                {
                                    ColoriseName.stringValue = mgr.Hud_Colors.ColorsLibrary_GetColorName(0);
                                    ColoriseName.serializedObject.ApplyModifiedProperties();
                                }
                            }
                        }
                    }
                    return;
                }
            }
            #endregion

            #region 预览按钮
            if (!Application.isPlaying)
            {
                Editor_XHud_GUI.Gui_Layout_FlexSpace();

                if (!Targets_Selected())
                {
                    Editor_XHud_GUI.SetEnabled(true);
                }
                else
                {
                    Editor_XHud_GUI.SetEnabled(false);
                }
                if (!TweenIsPreviewing.boolValue)
                {
                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "预览", prw_play_r, prw_play_p, 4))
                    {
                        Preview_Animator_Play();
                    }
                }
                else
                {
                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "停止", prw_stop_r, prw_stop_p, 4))
                    {
                        Preview_Animator_Stop();
                    }
                }
                Editor_XHud_GUI.SetEnabled(true);
            }
            #endregion

            GUILayout.Space(10);
            GUILayout.EndHorizontal();

            #endregion

            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 配色参数
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 0, "配色参数", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            bool TextSyncing = false;

            #region 如果存在文字组件

            if (HudText != null)
            {
                if (HudText.StyleLibSynching)
                {
                    if (HudText.TextStyleInfo.LibStyle_Effect_color)
                    {
                        TextSyncing = true;
                    }
                }
            }
            if (HudTmpText != null)
            {
                if (HudTmpText.StyleLibSynching)
                {
                    if (HudTmpText.TextStyleInfo.LibStyle_Effect_color)
                    {
                        TextSyncing = true;
                    }
                }
            }
            #endregion

            if (!TextSyncing)
            {
                #region 配色调整
                if (type != ModuleType.RectTransform)
                {
                    if (!SyncLibraryColor.boolValue)
                    {
                        Editor_XHud_GUI.Gui_Layout_Space(5);
                        GUILayout.BeginHorizontal();
                        if (!Targets_Selected())
                        {
                            Editor_XHud_GUI.Gui_Layout_Space(10);
                            if (Editor_XHud_GUI.Gui_Layout_Button(14, "将当前颜色添加到色卡库中", Add_r, Add_p, 4))
                            {
                                xHud_LibraryArg_Color info = new xHud_LibraryArg_Color();
                                //info.Painting = BaseScript;
                                info.Color = OriginalColor.colorValue;
                                Open_Hud_Library_Color_Setter(info);
                            }
                        }
                        EditorGUI.BeginChangeCheck();
                        Editor_XHud_GUI.Gui_Layout_Property_Field("原始配色", OriginalColor);
                        EditorGUI.EndChangeCheck();
                        GUILayout.EndHorizontal();
                    }
                    else
                    {
                        #region 色卡库选项列表
                        if (mgr != null)
                        {
                            if (mgr.Hud_Colors != null && !mgr.Hud_Colors.ColorsLibrary_IsEmpty())
                            {
                                Editor_XHud_GUI.Gui_Layout_Space(5);
                                GUILayout.BeginHorizontal();

                                if (!Targets_Selected())
                                {
                                    Editor_XHud_GUI.Gui_Layout_Space(10);
                                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "定位色卡", locate_r, locate_p, 0))
                                    {
                                        if (!Application.isPlaying)
                                        {
                                            if (!mgr.Hud_Colors.ColorsLibrary_IsExist(ColoriseName.stringValue))
                                                return;
                                            Editor_XHud_MenuItemsAction_OpenLibrary.open_col();
                                            mgr.Hud_Colors.ColorsLibrary_Location(ColoriseName.stringValue);
                                        }
                                        else
                                        {
                                            if (!mgr.Hud_Colors.ColorsLibrary_IsExist(ColoriseName.stringValue))
                                                return;
                                            Editor_XHud_MenuItemsAction_OpenLibrary.open_col();
                                            mgr.Hud_Colors.ColorsLibrary_Location(ColoriseName.stringValue);
                                        }
                                    }
                                    Editor_XHud_GUI.Gui_Layout_Space(20);
                                }
                                else
                                {
                                    Editor_XHud_GUI.Gui_Layout_Space(10);
                                }

                                #region 控件列表文字 （色卡库列表下拉菜单）
                                string[] collist = mgr.Hud_Colors.ColorsLibrary_GetColorNames();
                                #endregion

                                #region 控件背景色
                                Color cc = Color.white;
                                bool SameColor = true;
                                string FirstColoriseName = SelectedObjects[0].ColoriseName;
                                if (!Targets_Selected())
                                {
                                    cc = mgr.Hud_Colors.ColorsLibrary_GetColor(ColoriseName.stringValue);
                                }
                                else
                                {
                                    for (int k = 1; k < SelectedObjects.Length; k++)
                                    {
                                        if (SelectedObjects[k].ColoriseName != FirstColoriseName)
                                        {
                                            SameColor = false;
                                            break;
                                        }
                                    }
                                }

                                if (SameColor)
                                {
                                    cc = mgr.Hud_Colors.ColorsLibrary_GetColor(FirstColoriseName);
                                }
                                else
                                {
                                    cc = Color.gray;
                                }
                                #endregion

                                if (Targets_Selected())
                                {
                                    for (int i = 0; i < SelectedObjects.Length; i++)
                                    {
                                        // 使用 Undo.RecordObject 来记录对目标对象的修改
                                        Undo.RecordObject(SelectedObjects[i], "Selected ColoriseName");
                                    }
                                }
                                else
                                {
                                    // 使用 Undo.RecordObject 来记录对目标对象的修改
                                    Undo.RecordObject(ColoriseName.serializedObject.targetObject, "Selected ColoriseName");
                                }

                                string xx = Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_Module_Animator>("色卡", collist, ref ColoriseName, HudFilled.实体, cc, 400, 20, SelectedObjects);
                                GUILayout.EndHorizontal();
                            }
                            else
                            {
                                Editor_XHud_GUI.Gui_Layout_Space(10);
                                GUILayout.BeginHorizontal();
                                Editor_XHud_GUI.Gui_Layout_Space(10);
                                Editor_XHud_GUI.Gui_Layout_Labelfield("未找到色卡库", HudFilled.无, HudColor.无, Editor_XHud_GUI.GetColor(HudColor.魅力红), TextAnchor.MiddleCenter, new Vector2(0, 0), 11);
                                Editor_XHud_GUI.Gui_Layout_Space(10);
                                GUILayout.EndHorizontal();
                                Editor_XHud_GUI.Gui_Layout_Space(10);
                            }
                        }
                        #endregion
                    }
                }
                #endregion
            }
            else
            {
                Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_XHud_GUI.Gui_Layout_Space(10);
                EditorGUILayout.HelpBox("字体颜色当前正在和字体样式库同步中，如果需要接管文字颜色请先让文字组件的同步样式关闭！", MessageType.Warning);
                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Horizontal_End();
            }
            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 基础参数
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 0, "基础参数", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            #region ID 
            Editor_XHud_GUI.Gui_Layout_Property_Field("ID", ID);
            #endregion

            Editor_XHud_GUI.Gui_Layout_Space(5);

            #region 标识
            Editor_XHud_GUI.Gui_Layout_Property_Field("标识", Indicator);
            Indicator.serializedObject.ApplyModifiedProperties();
            #endregion

            Editor_XHud_GUI.Gui_Layout_Space(5);

            #region 动画全局耗时
            Editor_XHud_GUI.Gui_Layout_Property_Field("速率倍增", Animator_GlobalDuration);
            #endregion

            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 动画节点列表
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 0, "动画节点列表", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(10);
            if (Targets_Selected())
            {
                Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_XHud_GUI.Gui_Layout_Space(10);
                EditorGUILayout.HelpBox("动画节点不支持多项操作", MessageType.Warning);
                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Horizontal_End();
            }
            else
            {
                AnimateTweenNodesList.DoLayoutList();
                AnimateTweenNodes.serializedObject.ApplyModifiedProperties();
            }
            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 原始姿态

            string info_title = "原始姿态";
            if (Targets_Selected())
                info_title = "原始姿态 - ( 批量模式 )";
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, info_title, XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(5);

            if (!Targets_Selected())
            {
                SerializedProperty pos = sp_AnimatorFeature.FindPropertyRelative("Position");
                SerializedProperty eur = sp_AnimatorFeature.FindPropertyRelative("Euler");
                SerializedProperty sca = sp_AnimatorFeature.FindPropertyRelative("Scale");
                SerializedProperty size = sp_AnimatorFeature.FindPropertyRelative("Size");
                SerializedProperty alp = sp_AnimatorFeature.FindPropertyRelative("Alpha");
                SerializedProperty col = sp_AnimatorFeature.FindPropertyRelative("Color");
                SerializedProperty fil = sp_AnimatorFeature.FindPropertyRelative("Fill");

                AnimatorFeaturesDisplayer(pos.vector3Value, eur.vector3Value, sca.vector3Value, size.vector2Value, alp.floatValue, fil.floatValue, col.colorValue);
            }
            else
            {
                Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_XHud_GUI.Gui_Layout_Space(10);
                if (Editor_XHud_GUI.Gui_Layout_Button($"{SelectedObjects[AnimatorOriginalPoserInfos_Index].name} ( {SelectedObjects[AnimatorOriginalPoserInfos_Index].Indicator} )", "", HudFilled.透明, HudColor.无, SelectedObjects[AnimatorOriginalPoserInfos_Index].OriginalColor, 20))
                {
                    EditorGUIUtility.PingObject(SelectedObjects[AnimatorOriginalPoserInfos_Index]);
                }
                Editor_XHud_GUI.Gui_Layout_FlexSpace();
                if (Editor_XHud_GUI.Gui_Layout_Button(12, "", left_arrow_r, left_arrow_p))
                {
                    if (AnimatorOriginalPoserInfos_Index <= 0)
                    {
                        AnimatorOriginalPoserInfos_Index = AnimatorFeatures.Length - 1;
                    }
                    else
                    {
                        AnimatorOriginalPoserInfos_Index--;
                    }
                    EditorGUIUtility.PingObject(SelectedObjects[AnimatorOriginalPoserInfos_Index]);
                }
                Editor_XHud_GUI.Gui_Layout_Space(16);
                if (Editor_XHud_GUI.Gui_Layout_Button(12, "", right_arrow_r, right_arrow_p))
                {
                    if (AnimatorOriginalPoserInfos_Index >= AnimatorFeatures.Length - 1)
                    {
                        AnimatorOriginalPoserInfos_Index = 0;
                    }
                    else
                    {
                        AnimatorOriginalPoserInfos_Index++;
                    }
                    EditorGUIUtility.PingObject(SelectedObjects[AnimatorOriginalPoserInfos_Index]);
                }
                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                AnimatorFeaturesDisplayer(AnimatorFeatures[AnimatorOriginalPoserInfos_Index].Position, AnimatorFeatures[AnimatorOriginalPoserInfos_Index].Euler, AnimatorFeatures[AnimatorOriginalPoserInfos_Index].Scale, AnimatorFeatures[AnimatorOriginalPoserInfos_Index].Size, AnimatorFeatures[AnimatorOriginalPoserInfos_Index].Alpha, AnimatorFeatures[AnimatorOriginalPoserInfos_Index].Fill, AnimatorFeatures[AnimatorOriginalPoserInfos_Index].Color);
            }

            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 状态
            string statu_title = "状态";
            if (Targets_Selected())
                statu_title = "状态 - ( 批量模式 )";
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, statu_title, XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(5);

            #region 使用状态     
            if (!Targets_Selected())
            {
                Editor_XHud_GUI.StatuDisplayer_text(statu, 12, new Vector2(0, 8), "动画状态", 12, TweenIsPreviewing.boolValue ? "运动" : "静止", XHud_Dashboard.Theme_Primary, 12);
            }
            else
            {
                #region 批量控件
                Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_XHud_GUI.Gui_Layout_Space(10);
                if (Editor_XHud_GUI.Gui_Layout_Button($"{SelectedObjects[AnimatorAnimationStatu_Index].name} ( {SelectedObjects[AnimatorAnimationStatu_Index].Indicator} )", "", HudFilled.透明, HudColor.无, Color.gray, 20))
                {
                    EditorGUIUtility.PingObject(SelectedObjects[AnimatorAnimationStatu_Index]);
                }
                Editor_XHud_GUI.Gui_Layout_FlexSpace();
                if (Editor_XHud_GUI.Gui_Layout_Button(12, "", left_arrow_r, left_arrow_p))
                {
                    if (AnimatorAnimationStatu_Index <= 0)
                    {
                        AnimatorAnimationStatu_Index = SelectedObjects.Length - 1;
                    }
                    else
                    {
                        AnimatorAnimationStatu_Index--;
                    }
                    EditorGUIUtility.PingObject(SelectedObjects[AnimatorAnimationStatu_Index]);
                }
                Editor_XHud_GUI.Gui_Layout_Space(16);
                if (Editor_XHud_GUI.Gui_Layout_Button(12, "", right_arrow_r, right_arrow_p))
                {
                    if (AnimatorAnimationStatu_Index >= SelectedObjects.Length - 1)
                    {
                        AnimatorAnimationStatu_Index = 0;
                    }
                    else
                    {
                        AnimatorAnimationStatu_Index++;
                    }
                    EditorGUIUtility.PingObject(SelectedObjects[AnimatorAnimationStatu_Index]);
                }
                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                #endregion

                Editor_XHud_GUI.StatuDisplayer_text(statu, 12, new Vector2(0, 8), "动画状态", 12, SelectedObjects[AnimatorAnimationStatu_Index].TweenIsPreviewing ? "运动" : "静止", XHud_Dashboard.Theme_Primary, 12);
            }
            #endregion

            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 统计
            string statistic_title = "统计";
            if (Targets_Selected())
                statistic_title = "统计 - ( 批量模式 )";
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, statistic_title, XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(5);

            AnimationsTimerStatistic();

            if (!Targets_Selected())
            {
                #region 组件数量 - 动画节点
                if (AnimateTweenNodes.arraySize <= 0)
                {
                    Editor_XHud_GUI.Gui_Layout_Labelfield("暂无统计数据", HudFilled.无, HudColor.无, Editor_XHud_GUI.GetColor(HudColor.阴影灰), TextAnchor.MiddleCenter, 11);
                }
                else
                {
                    Editor_XHud_GUI.StatuDisplayer_text(count, 12, new Vector2(0, 7), "动画节点", 12, AnimateTweenNodes.arraySize.ToString() + " 个", XHud_Dashboard.Theme_Primary, 11);
                    Editor_XHud_GUI.StatuDisplayer_text(timer_min, 12, new Vector2(0, 7), "最小耗时", 12, MinTimer.floatValue.ToString() + " 秒", XHud_Dashboard.Theme_Primary, 11);
                    Editor_XHud_GUI.StatuDisplayer_text(timer_max, 12, new Vector2(0, 7), "最大耗时", 12, MaxTimer.floatValue.ToString() + " 秒", XHud_Dashboard.Theme_Primary, 11);
                    Editor_XHud_GUI.StatuDisplayer_text(timer_min, 12, new Vector2(0, 7), "最小耗时<color=#909090>（动画器倍增）</color>", 12, MinTimerWithGlobalDuration.floatValue.ToString() + " 秒", XHud_Dashboard.Theme_Primary, 11);
                    Editor_XHud_GUI.StatuDisplayer_text(timer_max, 12, new Vector2(0, 7), "最大耗时<color=#909090>（动画器倍增）</color>", 12, MaxTimerWithGlobalDuration.floatValue.ToString() + " 秒", XHud_Dashboard.Theme_Primary, 11);
                    Editor_XHud_GUI.StatuDisplayer_text(timer_max, 12, new Vector2(0, 7), "最大耗时<color=#909090>（XHUD倍增）</color>", 12, (MaxTimerWithGlobalDuration.floatValue * mgr.DurationMultiply).ToString() + " 秒", XHud_Dashboard.Theme_Primary, 11);
                }
                #endregion
            }
            else
            {
                #region 批量控件
                Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_XHud_GUI.Gui_Layout_Space(10);
                if (Editor_XHud_GUI.Gui_Layout_Button($"{SelectedObjects[AnimatorAnimationStatistic_Index].name} ( {SelectedObjects[AnimatorAnimationStatistic_Index].Indicator} )", "", HudFilled.透明, HudColor.无, Color.gray, 20))
                {
                    EditorGUIUtility.PingObject(SelectedObjects[AnimatorAnimationStatistic_Index]);
                }
                Editor_XHud_GUI.Gui_Layout_FlexSpace();
                if (Editor_XHud_GUI.Gui_Layout_Button(12, "", left_arrow_r, left_arrow_p))
                {
                    if (AnimatorAnimationStatistic_Index <= 0)
                    {
                        AnimatorAnimationStatistic_Index = SelectedObjects.Length - 1;
                    }
                    else
                    {
                        AnimatorAnimationStatistic_Index--;
                    }
                    EditorGUIUtility.PingObject(SelectedObjects[AnimatorAnimationStatistic_Index]);
                }
                Editor_XHud_GUI.Gui_Layout_Space(16);
                if (Editor_XHud_GUI.Gui_Layout_Button(12, "", right_arrow_r, right_arrow_p))
                {
                    if (AnimatorAnimationStatistic_Index >= SelectedObjects.Length - 1)
                    {
                        AnimatorAnimationStatistic_Index = 0;
                    }
                    else
                    {
                        AnimatorAnimationStatistic_Index++;
                    }
                    EditorGUIUtility.PingObject(SelectedObjects[AnimatorAnimationStatistic_Index]);
                }
                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                #endregion

                #region 组件数量 - 动画节点
                if (SelectedObjects[AnimatorAnimationStatistic_Index].AnimateTweenNodes.Count <= 0)
                {
                    Editor_XHud_GUI.Gui_Layout_Labelfield("暂无统计数据", HudFilled.无, HudColor.无, Editor_XHud_GUI.GetColor(HudColor.阴影灰), TextAnchor.MiddleCenter, 11);
                }
                else
                {
                    Editor_XHud_GUI.StatuDisplayer_text(count, 12, new Vector2(0, 7), "动画节点数", 12, SelectedObjects[AnimatorAnimationStatistic_Index].AnimateTweenNodes.Count + " 个", XHud_Dashboard.Theme_Primary, 10);
                    Editor_XHud_GUI.StatuDisplayer_text(timer_min, 12, new Vector2(0, 7), "最小耗时", 12, SelectedObjects[AnimatorAnimationStatistic_Index].MinTimer.ToString() + " 秒", XHud_Dashboard.Theme_Primary, 10);
                    Editor_XHud_GUI.StatuDisplayer_text(timer_max, 12, new Vector2(0, 7), "最大耗时", 12, SelectedObjects[AnimatorAnimationStatistic_Index].MaxTimer.ToString() + " 秒", XHud_Dashboard.Theme_Primary, 10);
                    Editor_XHud_GUI.StatuDisplayer_text(timer_min, 12, new Vector2(0, 7), "最小耗时<color=#909090>（动画器倍增）</color>", 12, SelectedObjects[AnimatorAnimationStatistic_Index].MinTimerWithGlobalDuration.ToString() + " 秒", XHud_Dashboard.Theme_Primary, 10);
                    Editor_XHud_GUI.StatuDisplayer_text(timer_max, 12, new Vector2(0, 7), "最大耗时<color=#909090>（动画器倍增）</color>", 12, SelectedObjects[AnimatorAnimationStatistic_Index].MaxTimerWithGlobalDuration.ToString() + " 秒", XHud_Dashboard.Theme_Primary, 10);
                    Editor_XHud_GUI.StatuDisplayer_text(timer_max, 12, new Vector2(0, 7), "最大耗时<color=#909090>（XHUD倍增）</color>", 12, (SelectedObjects[AnimatorAnimationStatistic_Index].MaxTimerWithGlobalDuration * mgr.DurationMultiply).ToString() + " 秒", XHud_Dashboard.Theme_Primary, 10);
                }
                #endregion
            }

            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 选项
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "选项", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_Animator>("状态调试", stroptions_debug, ref DebugState, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_Animator>("动画静音", stroptions_mute, ref MutePlay, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_Animator>("元素控制动画", stroptions_control, ref IgnoreElementAnimationPlay, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 事件
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "事件", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(5);

            #region 事件列表
            if (Targets_Selected())
            {
                Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_XHud_GUI.Gui_Layout_Space(10);
                EditorGUILayout.HelpBox("事件列表不支持多项操作", MessageType.Warning);
                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Horizontal_End();
            }
            else
            {
                Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_XHud_GUI.Gui_Layout_Space(10);
                EventIsFold = EditorGUILayout.Foldout(EventIsFold, "动画器事件", true);
                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                if (EventIsFold)
                {
                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_Animator_Play);
                    eve_on_Animator_Play.serializedObject.ApplyModifiedProperties();
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_Animator_PlayAt);
                    eve_on_Animator_PlayAt.serializedObject.ApplyModifiedProperties();
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_Animator_Rewind);
                    eve_on_Animator_Rewind.serializedObject.ApplyModifiedProperties();
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_Animator_Ready);
                    eve_on_Animator_Ready.serializedObject.ApplyModifiedProperties();
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_Animator_Initialized);
                    eve_on_Animator_Initialized.serializedObject.ApplyModifiedProperties();
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_Animator_SoundPlay);
                    eve_on_Animator_SoundPlay.serializedObject.ApplyModifiedProperties();
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_Animator_AnimatingState);
                    eve_on_Animator_AnimatingState.serializedObject.ApplyModifiedProperties();
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                }
            }
            #endregion

            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 运行时刷新动画进度条
            if (Application.isPlaying)
            {
                if (TweenIsPreviewing.boolValue)
                {
                    Repaint();
                }
            }
            #endregion

            #region 菜单功能
            Rect rect_MenuArea = new Rect(0, rect.y, EditorGUIUtility.currentViewWidth, 260);

            float foldvalue = 0;
            for (int i = 0; i < BaseScript.AnimateTweenNodes.Count; i++)
            {
                if (BaseScript.AnimateTweenNodes[i].IsFold)
                {
                    foldvalue += 48.8f;
                }
                else
                {
                    foldvalue += 288.2f;
                }
            }

            Rect rect_MenuArea_down = new Rect(0, rect.y + 370 + (foldvalue), EditorGUIUtility.currentViewWidth, 650);

            if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && (rect_MenuArea.Contains(Event.current.mousePosition) || rect_MenuArea_down.Contains(Event.current.mousePosition)))
            {
                // 创建右键菜单
                GenericMenu menu = new GenericMenu();
                menu.AddDisabledItem(new GUIContent("色卡操作"));
                menu.AddItem(new GUIContent("R (识别色卡)"), false, () =>
                {
                    string buff = Editor_XHud_GUI.EditorData_Get_With_String("XED_ColorLibrary_Get_ColorInfo");

                    CopyHudColor copyHudColor = JsonUtility.FromJson<CopyHudColor>(buff);
                    string hexcol = XHud_Utilitys.Color_To_HexColor(XHud_Utilitys.Color_From_String(copyHudColor.Color, false), true);

                    try
                    {
                        if (copyHudColor != null)
                        {
                            if (Targets_Selected())
                            {
                                List<XHud_GUI_Dialog_ListDatas> Datas = new List<XHud_GUI_Dialog_ListDatas>();
                                for (int i = 0; i < SelectedObjects.Length; i++)
                                {
                                    if (!SelectedObjects[i].SyncLibraryColor)
                                    {
                                        string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 动画器消息", "批量识别色卡信息", $"侦测到 {SelectedObjects[i].name} ( {SelectedObjects[i].Indicator} ) 处于原始色模式！是否将其转换为色卡模式？", "跳过", "转换", 1);
                                        if (res == "跳过")
                                            continue;
                                        else
                                        {
                                            SelectedObjects[i].SyncLibraryColor = true;
                                        }
                                    }
                                    SelectedObjects[i].ColoriseName = copyHudColor.Name;
                                    XHud_GUI_Dialog_ListDatas dataitem = new XHud_GUI_Dialog_ListDatas();

                                    dataitem.Title = $"色卡 <color={hexcol}>{SelectedObjects[i].ColoriseName} </color>";
                                    dataitem.SubTitle = "已应用到动画器";
                                    dataitem.Message = $"{SelectedObjects[i].name} ( {SelectedObjects[i].Indicator} )";

                                    Datas.Add(dataitem);
                                }

                                Editor_XHud_GUI.Open(Datas.ToArray(), XHud_DialogType.确认, "XHud - 动画器消息", "批量识别色卡信息", "以下是已应用识别的色卡参数的动画器列表，请您检查核对：", "明白");
                            }
                            else
                            {
                                BaseScript.ColoriseName = copyHudColor.Name;
                                Editor_XHud_GUI.Open(XHud_DialogType.确认, "XHud - 动画器消息", "识别色卡信息", $"已识别 XHudEditorData (XED) 中的色卡信息！您从色卡库中获取的色卡名称为： <color={hexcol}>{copyHudColor.Name} </color>", "没错");
                            }
                        }
                    }
                    catch (System.Exception e)
                    {
                        string msg = e.Message;
                        Editor_XHud_GUI.Open(XHud_DialogType.错误, "XHud - 动画器消息", "识别色卡信息", "未能识别的参数！请在色卡库的其中一项上点击右键并选择 \"获取色卡信息\" 后再试！", "明白", 0);
                    }
                });
                menu.AddItem(new GUIContent("D (拷贝色卡)"), false, () =>
                {
                    Editor_XHud_GUI.EditorData_Set_With_String("XED_HudAnimator_Get_ColoriseName", BaseScript.ColoriseName);
                    string hexcol = XHud_Utilitys.Color_To_HexColor(OriginalColor.colorValue, true);
                    Editor_XHud_GUI.Open(XHud_DialogType.确认, "XHud - 动画器消息", "色板信息拷贝", $"已将当前动画器的色卡信息<color={hexcol}> {BaseScript.ColoriseName} </color>XHudEditorData (XED)！", "好的");
                });
                menu.AddItem(new GUIContent("W (粘贴色卡)"), false, () =>
                {
                    string data = Editor_XHud_GUI.EditorData_Get_With_String("XED_HudAnimator_Get_ColoriseName");
                    if (Targets_Selected())
                    {
                        #region 询问
                        List<XHud_GUI_Dialog_ListDatas> Datas = new List<XHud_GUI_Dialog_ListDatas>();
                        for (int i = 0; i < SelectedObjects.Length; i++)
                        {
                            XHud_GUI_Dialog_ListDatas dataitem = new XHud_GUI_Dialog_ListDatas();
                            dataitem.Title = $"色卡信息 {data}";
                            dataitem.SubTitle = "即将粘贴到";
                            dataitem.Message = $"{SelectedObjects[i].name} ({SelectedObjects[i].Indicator})";
                            Datas.Add(dataitem);
                        }
                        string res_mul = Editor_XHud_GUI.Open(Datas.ToArray(), XHud_DialogType.警告, "XHud - 动画器消息", "批量色卡信息粘贴", $"确认要将 XHudEditorData (XED) 中的色卡信息粘贴到列表中的动画器中吗？", "粘贴", "暂不", 1);
                        if (res_mul == "暂不")
                            return;
                        #endregion

                        #region 粘贴色卡信息
                        Datas.Clear();
                        for (int i = 0; i < SelectedObjects.Length; i++)
                        {
                            SelectedObjects[i].ColoriseName = data;

                            XHud_GUI_Dialog_ListDatas dataitem = new XHud_GUI_Dialog_ListDatas();
                            dataitem.Title = "色卡参数";
                            dataitem.SubTitle = "已粘贴到";
                            dataitem.Message = $"{BaseScript.name} ({BaseScript.Indicator})";

                            Datas.Add(dataitem);
                        }
                        Editor_XHud_GUI.Open(Datas.ToArray(), XHud_DialogType.确认, "XHud - 动画器消息", "批量色卡信息粘贴", "以下是已粘贴色卡信息的动画器列表，请您检查核对：", "明白");
                        #endregion
                    }
                    else
                    {
                        #region 询问
                        string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 动画器消息", "粘贴色卡信息", $"确认要将 XHudEditorData (XED) 中的色卡信息粘贴到  {BaseScript.name} ( {BaseScript.Indicator} ) 动画器中吗？", "粘贴", "暂不", 1);
                        if (res == "暂不")
                            return;
                        #endregion

                        #region 粘贴色卡信息
                        BaseScript.ColoriseName = data;
                        Editor_XHud_GUI.Open(XHud_DialogType.确认, "XHud - 动画器消息", "粘贴色卡信息", $"已将色卡信息粘贴到动画器 {BaseScript.name} ({BaseScript.Indicator}) ！", "好的");
                        #endregion
                    }
                });
                menu.AddSeparator("");
                menu.AddDisabledItem(new GUIContent("基础"));
                menu.AddItem(new GUIContent("F (自动标识)"), false, () =>
                {
                    string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 动画器消息", "自动标识名称", "根据物体自身名称快速进行标识，如果执行此操作选中的所有动画器的标识名称即将会被修改，是否需要继续？请谨慎此操作！因为如果您在程序中调用是通过标识名称作为基准的话请记得同步修改您的程序！", "标识", "暂不", 1);
                    if (res == "暂不")
                    {
                        return;
                    }

                    if (Targets_Selected())
                    {
                        List<XHud_GUI_Dialog_ListDatas> Datas = new List<XHud_GUI_Dialog_ListDatas>();
                        for (int i = 0; i < SelectedObjects.Length; i++)
                        {
                            SelectedObjects[i].Indicator = SelectedObjects[i].name;

                            XHud_GUI_Dialog_ListDatas dataitem = new XHud_GUI_Dialog_ListDatas();

                            dataitem.Title = $"{SelectedObjects[i].name} ( {SelectedObjects[i].Indicator} )";
                            dataitem.SubTitle = "已自动识别标识名称为";
                            dataitem.Message = $"{SelectedObjects[i].Indicator}";

                            Datas.Add(dataitem);
                        }

                        Editor_XHud_GUI.Open(Datas.ToArray(), XHud_DialogType.确认, "XHud - 动画器消息", "批量识别标识名称", "以下是已识别的动画器标识名称列表，请您检查核对：", "明白");
                    }
                    else
                    {
                        BaseScript.Indicator = BaseScript.name;
                        Editor_XHud_GUI.Open(XHud_DialogType.确认, "XHud - 动画器消息", "识别标识名称", $"已自动识别 {BaseScript.name} ( {BaseScript.Indicator} ) 动画器的标识名称为：{BaseScript.Indicator}", "明白");
                    }
                });
                menu.AddItem(new GUIContent("X (记录姿态)"), false, () =>
                {
                    if (Targets_Selected())
                    {
                        List<XHud_GUI_Dialog_ListDatas> Datas = new List<XHud_GUI_Dialog_ListDatas>();

                        for (int i = 0; i < SelectedObjects.Length; i++)
                        {
                            AnimatorFeatures_Save(SelectedObjects[i]);
                            XHud_GUI_Dialog_ListDatas dataitem = new XHud_GUI_Dialog_ListDatas();

                            dataitem.Title = string.IsNullOrEmpty(SelectedObjects[i].Indicator) ? SelectedObjects[i].name : SelectedObjects[i].Indicator;
                            dataitem.SubTitle = "";
                            dataitem.Message = "已记录姿态信息";
                            Datas.Add(dataitem);
                        }

                        Editor_XHud_GUI.Open(Datas.ToArray(), XHud_DialogType.确认, "XHud - 动画器消息", "批量记录动画器姿态", "以下是批量已记录姿态的所有动画器列表，请您检查核对：", "明白");
                    }
                    else
                    {
                        AnimatorFeatures_Save(BaseScript);
                        Editor_XHud_GUI.Open(XHud_DialogType.确认, "XHud - 动画器消息", "动画器姿态记录", "已记录动画器的姿态参数！", "明白");
                    }
                });
                menu.AddSeparator("");
                menu.AddDisabledItem(new GUIContent("动画效果"));
                if (!Targets_Selected())
                {
                    menu.AddItem(new GUIContent("A (拷贝)"), false, () =>
                    {
                        TweenNodeArray tnc = new TweenNodeArray();
                        tnc.TweenNodeList = new List<TweenNode>();

                        for (int i = 0; i < BaseScript.AnimateTweenNodes.Count; i++)
                        {
                            tnc.TweenNodeList.Add(BaseScript.AnimateTweenNodes[i]);
                        }

                        string json = JsonUtility.ToJson(tnc);

                        Editor_XHud_GUI.EditorData_Set_With_String("XED_HudAnimator_Copied_TweenNodes", json);
                        Editor_XHud_GUI.Open(XHud_DialogType.确认, "XHud - 动画器消息", "动画节点数据", $"已拷贝  {BaseScript.name} ( {BaseScript.Indicator} ) 动画器的动画节点数据！", "明白");
                    });
                }
                menu.AddItem(new GUIContent("C (粘贴)"), false, () =>
                {
                    string json = Editor_XHud_GUI.EditorData_Get_With_String("XED_HudAnimator_Copied_TweenNodes");
                    TweenNodeArray tnc = JsonUtility.FromJson<TweenNodeArray>(json);

                    if (Targets_Selected())
                    {
                        #region 询问
                        List<XHud_GUI_Dialog_ListDatas> Datas = new List<XHud_GUI_Dialog_ListDatas>();
                        for (int i = 0; i < SelectedObjects.Length; i++)
                        {
                            XHud_GUI_Dialog_ListDatas dataitem = new XHud_GUI_Dialog_ListDatas();
                            dataitem.Title = $"动画节点数据";
                            dataitem.SubTitle = "即将粘贴到";
                            dataitem.Message = $"{SelectedObjects[i].name} ({SelectedObjects[i].Indicator})";
                            Datas.Add(dataitem);
                        }
                        string res_mul = Editor_XHud_GUI.Open(Datas.ToArray(), XHud_DialogType.警告, "XHud - 动画器消息", "批量粘贴动画节点数据", $"确认要将 XHudEditorData (XED) 中的动画节点数据粘贴到列表中的动画器中吗？", "粘贴", "暂不", 1);
                        if (res_mul == "暂不")
                            return;
                        #endregion

                        #region 批量粘贴动画节点数据
                        Datas.Clear();
                        for (int s = 0; s < SelectedObjects.Length; s++)
                        {
                            SelectedObjects[s].AnimateTweenNodes = tnc.TweenNodeList;

                            XHud_GUI_Dialog_ListDatas dataitem = new XHud_GUI_Dialog_ListDatas();
                            dataitem.Title = "动画节点数据";
                            dataitem.SubTitle = "已粘贴到动画器";
                            dataitem.Message = $"{SelectedObjects[s].name} ( {SelectedObjects[s].Indicator} )";

                            Datas.Add(dataitem);
                        }
                        Editor_XHud_GUI.Open(Datas.ToArray(), XHud_DialogType.确认, "XHud - 动画器消息", "批量粘贴动画节点数据", "以下是已粘贴动画节点数据的动画器列表，请您检查核对：", "明白");
                        #endregion
                    }
                    else
                    {
                        #region 询问
                        string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 动画器消息", "粘贴动画节点数据", $"确认要将 XHudEditorData (XED) 中的动画节点数据粘贴到  {BaseScript.name} ( {BaseScript.Indicator} ) 动画器中吗？", "粘贴", "暂不", 1);
                        if (res == "暂不")
                            return;
                        #endregion

                        #region 粘贴动画节点数据
                        BaseScript.AnimateTweenNodes = tnc.TweenNodeList;
                        Editor_XHud_GUI.Open(XHud_DialogType.确认, "XHud - 动画器消息", "粘贴动画节点", $"已将动画节点粘贴到： {BaseScript.name} ( {BaseScript.Indicator} )", "明白");
                        #endregion
                    }
                });
                menu.AddItem(new GUIContent("E (清空)"), false, () =>
                {
                    string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 动画器消息", "清空动画效果列表", "快速清空动画效果列表此操作不可逆，是否需要清空？清空后您为此动画器做的动画效果参数将全部丢失，请谨慎此操作！", "清空", "暂不", 1);
                    if (res == "暂不")
                    {
                        return;
                    }

                    if (Targets_Selected())
                    {
                        List<XHud_GUI_Dialog_ListDatas> Datas = new List<XHud_GUI_Dialog_ListDatas>();
                        for (int s = 0; s < SelectedObjects.Length; s++)
                        {
                            Undo.RecordObject(SelectedObjects[s], "PasteAnimationTweenNodes");
                            for (int i = 1; i < SelectedObjects.Length; i++)
                            {
                                SelectedObjects[s].AnimateTweenNodes.Clear();

                                XHud_GUI_Dialog_ListDatas dataitem = new XHud_GUI_Dialog_ListDatas();
                                dataitem.Title = $"动画器";
                                dataitem.SubTitle = $"{SelectedObjects[s].name} ( {SelectedObjects[s].Indicator} )";
                                dataitem.Message = $"动画效果已清空";

                                Datas.Add(dataitem);
                            }
                        }
                        Editor_XHud_GUI.Open(Datas.ToArray(), XHud_DialogType.确认, "XHud - 动画器消息", "批量清空动画效果列表", "以下是已清空动画效果列表的动画器列表，请您检查核对：", "明白");
                    }
                    else
                    {
                        BaseScript.AnimateTweenNodes.Clear();

                        Editor_XHud_GUI.Open(XHud_DialogType.确认, "XHud - 动画器消息", "清空动画效果列表", $"已将  {BaseScript.name} ( {BaseScript.Indicator} ) 动画器的动画效果列表清空！", "明白");
                    }
                });
                if (!Targets_Selected())
                {
                    menu.AddSeparator("");
                    menu.AddDisabledItem(new GUIContent("预览"));
                    if (!TweenIsPreviewing.boolValue)
                        menu.AddItem(new GUIContent("S (开始)"), false, () =>
                        {
                            if (!Application.isPlaying)
                            {
                                Preview_Animator_Play();
                            }
                        });
                    else
                        menu.AddItem(new GUIContent("S (停止)"), false, () =>
                        {
                            if (!Application.isPlaying)
                            {
                                Preview_Animator_Stop();
                            }
                        });
                }
                menu.ShowAsContext(); // 在鼠标位置显示右键菜单
            }
            #endregion

            #region 源脚本
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 3, "源脚本", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(5);

            #region 原始变量
            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_XHud_GUI.Gui_Layout_Space(10);
            OriginalDisplay = EditorGUILayout.Foldout(OriginalDisplay, "变量/属性", true);
            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Horizontal_End();
            if (OriginalDisplay)
                DrawDefaultInspector();
            #endregion

            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            serializedObject.ApplyModifiedProperties();
        }

        #region 辅助
        /// <summary>
        /// 检测动画器首次初始状态
        /// </summary>
        private void CheckFirstCreatedAnimator()
        {
            if (Targets_Selected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    XHud_Module_Animator anim = SelectedObjects[i];
                    SerializedObject so = new SerializedObject(anim);
                    so.Update();
                    SerializedProperty prop_firstcreate = so.FindProperty("FirstCreateRecordOriginalState");
                    if (!prop_firstcreate.boolValue)
                    {
                        prop_firstcreate.boolValue = true;
                        // 首次记录姿态信息
                        AnimatorFeatures_Save(anim);
                    }
                    so.ApplyModifiedProperties();
                }
            }
            else
            {
                if (!FirstCreateRecordOriginalState.boolValue)
                {
                    FirstCreateRecordOriginalState.boolValue = true;
                    serializedObject.ApplyModifiedProperties();

                    // 首次记录姿态信息
                    AnimatorFeatures_Save(BaseScript);
                }
            }
        }

        /// <summary>
        /// 颜色参数修改器
        /// </summary>
        public void Open_Hud_Library_Color_Setter(xHud_LibraryArg_Color info)
        {
            Editor_XHud_LibrarySetTool_Color window = EditorWindow.GetWindow<Editor_XHud_LibrarySetTool_Color>(true);

            window.titleContent = new GUIContent("XHud 色卡库采集器");
            Editor_XHud_GUI.CenterEditorWindow(new Vector2Int(620, 530), window);

            window.SetLibrarySetterMode(LibrarySetterMode.添加到库);
            window.SetTitle("XHud 色卡库采集器");
            window.SetInfo(info.Name, info.Description, info.Color);
            window.SetPainting(info.Painting);
            window.SetButtonText("添加", "取消");
            //window.ShowModal();
            window.Show();
        }

        /// <summary>
        /// 获取动画器中是否存在循环模式
        /// </summary>
        /// <returns></returns>
        public bool HasLoopMode()
        {
            bool hasLoop = false;
            for (int i = 0; i < AnimateTweenNodes.arraySize; i++)
            {
                SerializedProperty sp_tween_loopcount = AnimateTweenNodes.GetArrayElementAtIndex(i).FindPropertyRelative("LoopCount");
                if (sp_tween_loopcount.intValue == -1)
                {
                    hasLoop = true;
                    break;
                }
            }
            return hasLoop;
        }

        /// <summary>
        /// 自动识别UI元素类型
        /// </summary>
        private void RecognizeElementType()
        {
            if (Targets_Selected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    XHud_Module_Animator anim = SelectedObjects[i];
                    RectTransform rect = anim.GetComponent<RectTransform>();
                    TextMeshProUGUI tmp = anim.GetComponent<TextMeshProUGUI>();
                    Text tt = anim.GetComponent<Text>();
                    Image img = anim.GetComponent<Image>();
                    RawImage rawimg = anim.GetComponent<RawImage>();
                    //CanvasGroup canvas = anim.GetComponent<CanvasGroup>();

                    SerializedObject so = new SerializedObject(anim);

                    so.Update();

                    SerializedProperty prop_mod_RectTransform = so.FindProperty("mod_RectTransform");
                    SerializedProperty prop_mod_TmpText = so.FindProperty("mod_TmpText");
                    SerializedProperty prop_mod_Text = so.FindProperty("mod_Text");
                    SerializedProperty prop_mod_Image = so.FindProperty("mod_Image");
                    SerializedProperty prop_mod_RawImage = so.FindProperty("mod_RawImage");
                    SerializedProperty prop_AnimatorModuleType = so.FindProperty("AnimatorModuleType");
                    //SerializedProperty prop_CanvasGroup = so.FindProperty("CanvasGroup");

                    if (rect != null)
                    {
                        prop_mod_RectTransform.objectReferenceValue = rect;
                    }
                    if (tmp != null)
                    {
                        prop_mod_TmpText.objectReferenceValue = tmp;
                    }
                    if (tt != null)
                    {
                        prop_mod_Text.objectReferenceValue = tt;
                    }
                    if (img != null)
                    {
                        prop_mod_Image.objectReferenceValue = img;
                    }
                    if (rawimg != null)
                    {
                        prop_mod_RawImage.objectReferenceValue = rawimg;
                    }
                    //if (canvas != null)
                    //{
                    //    prop_CanvasGroup.objectReferenceValue = canvas;
                    //}

                    so.ApplyModifiedProperties();
                }
            }
            else
            {
                RectTransform rect = BaseScript.GetComponent<RectTransform>();
                XHud_Module_TmpText tmp = BaseScript.GetComponent<XHud_Module_TmpText>();
                XHud_Module_Text tt = BaseScript.GetComponent<XHud_Module_Text>();
                Image img = BaseScript.GetComponent<Image>();
                RawImage rawimg = BaseScript.GetComponent<RawImage>();
                //CanvasGroup canvas = BaseScript.GetComponent<CanvasGroup>();

                if (rect != null)
                {
                    mod_RectTransform.objectReferenceValue = rect;
                }
                if (tmp != null)
                {
                    mod_TmpText.objectReferenceValue = tmp;
                }
                if (tt != null)
                {
                    mod_Text.objectReferenceValue = tt;
                }
                if (img != null)
                {
                    mod_Image.objectReferenceValue = img;
                }
                if (rawimg != null)
                {
                    mod_RawImage.objectReferenceValue = rawimg;
                }
                //if (canvas != null)
                //{
                //    CanvasGroup.objectReferenceValue = canvas;
                //}
            }
        }

        /// <summary>
        /// 去除数据左右小括号
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string TrimParentheses(string str)
        {
            if (str.StartsWith("(") && str.EndsWith(")"))
            {
                return str.Substring(1, str.Length - 2);
            }
            return str;
        }

        /// <summary>
        /// 获取最新的动画耗时信息
        /// </summary>
        private void AnimationsTimerStatistic()
        {
            if (!Targets_Selected())
            {
                BaseScript.TweenNodeTimersGet();
            }
            else
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    SelectedObjects[i].TweenNodeTimersGet();
                }
            }
        }

        #endregion

        #region 特性参数
        /// <summary>
        /// Animator特性参数控件绘制
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="eur"></param>
        /// <param name="sca"></param>
        /// <param name="size"></param>
        /// <param name="alp"></param>
        /// <param name="col"></param>
        /// <param name="fil"></param>
        private void AnimatorFeaturesDisplayer(Vector3 pos, Vector3 eur, Vector3 sca, Vector2 size, float alp, float fil, Color col)
        {
            int titlesize = 12;
            int contensize = 10;
            float iconsize = 12;
            Color valuecol = Editor_XHud_GUI.GetColor(HudColor.阴影灰);

            Editor_XHud_GUI.StatuDisplayer_text(this.pos, iconsize, new Vector2(0, 7), "位置", titlesize, TrimParentheses(pos.ToString()), valuecol, contensize);
            Editor_XHud_GUI.StatuDisplayer_text(rot, iconsize, new Vector2(0, 7), "旋转", titlesize, TrimParentheses(eur.ToString()), valuecol, contensize);
            Editor_XHud_GUI.StatuDisplayer_text(this.sca, iconsize, new Vector2(0, 7), "缩放", titlesize, TrimParentheses(sca.ToString()), valuecol, contensize);
            Editor_XHud_GUI.StatuDisplayer_text(this.size, iconsize, new Vector2(0, 7), "尺寸", titlesize, TrimParentheses(size.ToString()), valuecol, contensize);
            Editor_XHud_GUI.StatuDisplayer_text(this.alp, iconsize, new Vector2(0, 7), "透明度", titlesize, TrimParentheses(alp.ToString()), valuecol, contensize);
            Editor_XHud_GUI.StatuDisplayer_text(fill, iconsize, new Vector2(0, 7), "填充度", titlesize, TrimParentheses(fil.ToString()), valuecol, contensize);
            Editor_XHud_GUI.StatuDisplayer_color(color, iconsize, new Vector2(0, 7), "颜色", titlesize, col);
        }
        /// <summary>
        /// 记录Animator特性参数
        /// </summary>
        private void AnimatorFeatures_Save(XHud_Module_Animator animator)
        {
            using (SerializedObject so = new SerializedObject(animator))
            {
                so.Update();

                SerializedProperty feature = so.FindProperty("AnimatorFeature");

                SerializedProperty sp_mod_RectTransform = so.FindProperty("mod_RectTransform");
                SerializedProperty sp_mod_Image = so.FindProperty("mod_Image");
                SerializedProperty sp_mod_Text = so.FindProperty("mod_Text");
                SerializedProperty sp_mod_TmpText = so.FindProperty("mod_TmpText");

                SerializedProperty pos = feature.FindPropertyRelative("Position");
                SerializedProperty eur = feature.FindPropertyRelative("Euler");
                SerializedProperty sca = feature.FindPropertyRelative("Scale");
                SerializedProperty size = feature.FindPropertyRelative("Size");
                SerializedProperty alp = feature.FindPropertyRelative("Alpha");
                SerializedProperty col = feature.FindPropertyRelative("Color");
                SerializedProperty fil = feature.FindPropertyRelative("Fill");

                RectTransform rect = sp_mod_RectTransform.objectReferenceValue as RectTransform;
                Image image = sp_mod_Image.objectReferenceValue as Image;
                RawImage rawimage = sp_mod_Image.objectReferenceValue as RawImage;
                XHud_Module_Text text = sp_mod_Text.objectReferenceValue as XHud_Module_Text;
                XHud_Module_TmpText tmp = sp_mod_TmpText.objectReferenceValue as XHud_Module_TmpText;

                if (rect != null)
                {
                    pos.vector3Value = rect.anchoredPosition3D;
                    eur.vector3Value = rect.localEulerAngles;
                    sca.vector3Value = rect.localScale;
                    size.vector2Value = rect.sizeDelta;
                }

                if (image != null)
                {
                    col.colorValue = image.color;
                    fil.floatValue = image.fillAmount;
                    alp.floatValue = image.color.a;
                }
                else if (text != null)
                {
                    col.colorValue = text.color;
                }
                else if (tmp != null)
                {
                    col.colorValue = tmp.color;
                }
                else if (rawimage != null)
                {
                    alp.floatValue = rawimage.color.a;
                }

                so.ApplyModifiedProperties();
            }
        }
        /// <summary>
        /// 还原Animator特性参数
        /// </summary>
        private void AnimatorFeatures_Load(XHud_Module_Animator animator)
        {
            using (SerializedObject so = new SerializedObject(animator))
            {
                so.Update();

                SerializedProperty feature = so.FindProperty("AnimatorFeature");
                SerializedProperty pos = feature.FindPropertyRelative("Position");
                SerializedProperty eur = feature.FindPropertyRelative("Euler");
                SerializedProperty sca = feature.FindPropertyRelative("Scale");
                SerializedProperty size = feature.FindPropertyRelative("Size");
                SerializedProperty alp = feature.FindPropertyRelative("Alpha");
                SerializedProperty col = feature.FindPropertyRelative("Color");
                SerializedProperty fil = feature.FindPropertyRelative("Fill");

                SerializedProperty sp_mod_RectTransform = so.FindProperty("mod_RectTransform");
                SerializedProperty sp_mod_Image = so.FindProperty("mod_Image");
                SerializedProperty sp_mod_Text = so.FindProperty("mod_Text");
                SerializedProperty sp_mod_TmpText = so.FindProperty("mod_TmpText");

                RectTransform rect = sp_mod_RectTransform.objectReferenceValue as RectTransform;
                Image image = sp_mod_Image.objectReferenceValue as Image;
                RawImage rawimage = sp_mod_Image.objectReferenceValue as RawImage;
                XHud_Module_Text text = sp_mod_Text.objectReferenceValue as XHud_Module_Text;
                XHud_Module_TmpText tmp = sp_mod_TmpText.objectReferenceValue as XHud_Module_TmpText;

                if (rect != null)
                {
                    rect.anchoredPosition3D = pos.vector3Value;
                    rect.localEulerAngles = eur.vector3Value;
                    rect.localScale = sca.vector3Value;
                    rect.sizeDelta = size.vector2Value;
                }

                if (image != null)
                {
                    image.color = col.colorValue;
                    image.fillAmount = fil.floatValue;

                    Color cc = image.color;
                    cc.a = alp.floatValue;
                    image.color = cc;
                }
                else if (text != null)
                {
                    text.color = col.colorValue;
                }
                else if (tmp != null)
                {
                    tmp.color = col.colorValue;
                }
                else if (rawimage != null)
                {
                    rawimage.color = col.colorValue;

                    Color cc = rawimage.color;
                    cc.a = alp.floatValue;
                    rawimage.color = cc;
                }

                so.ApplyModifiedProperties();
            }
        }
        /// <summary>
        /// 采集Animator特性参数
        /// </summary>
        private void AnimatorFeatures_Capture(XHud_Module_Animator[] animators)
        {
            AnimatorFeatures = new AnimatorFeatures[animators.Length];
            for (int i = 0; i < animators.Length; i++)
            {
                AnimatorFeatures[i] = animators[i].AnimatorFeature;
            }
        }
        #endregion

        #region 动画预览
        /// <summary>
        /// 预览动画
        /// </summary>
        private void Preview_Animator_Play()
        {
            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

            if (AnimateTweenNodes == null || AnimateTweenNodes.arraySize <= 0)
                return;

            //先停止之前的动画预览
            Preview_Animator_Stop();

            //将动画预览中的开关打开
            TweenIsPreviewing.boolValue = true;
            TweenIsPreviewing.serializedObject.ApplyModifiedProperties();

            for (int i = 0; i < AnimateTweenNodes.arraySize; i++)
            {
                SerializedProperty twnNode = AnimateTweenNodes.GetArrayElementAtIndex(i);
                TweenNode tweenNode = TweenNodeConvert_From_SerialProperty(twnNode);
                BaseScript.AnimateTweenNodes[i] = tweenNode;
                if (!tweenNode.Enabled)
                {
                    continue;
                }
                else
                {
                    if (tweenNode.ActivateOnlyToEnd)
                        continue;
                    else
                    {
                        XTween_Interface tweener;
                        if (mgr == null)
                            tweener = BaseScript.Tweener_Play(tweenNode, 1 * Animator_GlobalDuration.floatValue, null, null, 0);
                        else
                            tweener = BaseScript.Tweener_Play(tweenNode, mgr.DurationMultiply * Animator_GlobalDuration.floatValue, null, null, 0);
                        Preview_Animator_TweenList.Add(tweener);
                    }
                }
            }

            //同步启动音效播放
            Preivew_Animator_Coroutine_ProgressCalcToSound = EditorCoroutineUtility.StartCoroutineOwnerless(Preview_Animator_Coroutine_ProgressCalcSound());

            if (Preview_Animator_TweenList != null && Preview_Animator_TweenList.Count > 0)
            {
                for (int i = 0; i < Preview_Animator_TweenList.Count; i++)
                {
                    Preivew_Animator_CoroutineList_Play.Add(EditorCoroutineUtility.StartCoroutineOwnerless(Preview_Animator_Coroutine_Play(i)));
                }
                if (!HasLoopMode())
                    Preivew_Animator_Coroutine_Stop = EditorCoroutineUtility.StartCoroutine(Preview_Animator_Coroutine_Stop(), this);
                //DOTweenEditorPreview.Start();
            }
        }
        /// <summary>
        /// 监测所有动画进度
        /// </summary>
        /// <returns></returns>
        IEnumerator Preview_Animator_Coroutine_ProgressCalcSound()
        {
            while (TweenIsPreviewing.boolValue)
            {
                for (int i = 0; i < BaseScript.AnimateTweenNodes.Count; i++)
                {
                    TweenNode tweenNode = BaseScript.AnimateTweenNodes[i];
                    XTween_Interface tweener = tweenNode.Tweener;

                    if (tweener != null && tweener.IsActive && tweener.IsPlaying)
                    {
                        if (tweenNode.Progress > 0.985f)
                            tweenNode.Progress = 1;
                        else
                            tweenNode.Progress = tweener.ElapsedTime / tweener.Duration;
                    }

                    for (int s = 0; s < tweenNode.TweenSounds.Count; s++)
                    {
                        ///---如果动画是循环模式则不会播放音效，以为初始化时动画的Progress为0，此时程序会判定已到达播放音效的触点位置，则会误判发出音效
                        if (tweenNode.LoopCount == -1)
                            continue;

                        TweenSound tweenSound = tweenNode.TweenSounds[s];

                        if (tweenSound.Sound == null)
                            continue;

                        if (tweenSound.Percentage >= 1)
                        {
                            if (tweenNode.Progress >= tweenSound.Percentage)
                            {
                                if (!tweenSound.IsPlayed)
                                {
                                    tweenSound.IsPlayed = true;

                                    AudioClip clip = tweenSound.Sound;
                                    float vol = tweenSound.Volume;
                                    float pitch_min = tweenSound.MinPitch;
                                    float pitch_max = tweenSound.MaxPitch;
                                    Preview_Animator_SoundList.Add(Preview_AnimatorSound_Creator(clip, vol, pitch_min, pitch_max));
                                }
                            }
                            else
                            {
                                tweenSound.IsPlayed = false;
                            }
                        }
                        else if (tweenSound.Percentage < 1)
                        {
                            if (tweenNode.Progress > tweenSound.Percentage)
                            {
                                if (!tweenSound.IsPlayed)
                                {
                                    tweenSound.IsPlayed = true;

                                    AudioClip clip = tweenSound.Sound;
                                    float vol = tweenSound.Volume;
                                    float pitch_min = tweenSound.MinPitch;
                                    float pitch_max = tweenSound.MaxPitch;
                                    Preview_Animator_SoundList.Add(Preview_AnimatorSound_Creator(clip, vol, pitch_min, pitch_max));
                                }
                            }
                            else
                            {
                                tweenSound.IsPlayed = false;
                            }
                        }
                    }
                }
                Repaint();
                yield return null;
            }
        }
        /// <summary>
        /// 预览动画协程播放
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IEnumerator Preview_Animator_Coroutine_Play(int index)
        {
            //DOTweenEditorPreview.PrepareTweenForPreview(Preview_Animator_TweenList[index], true, true, true);
            yield return null;
        }
        /// <summary>
        /// 停止预览动画
        /// </summary>
        private void Preview_Animator_Stop(bool LoadOriginalState = true)
        {
            if (target != null)
            {
                TweenIsPreviewing.boolValue = false;
                TweenIsPreviewing.serializedObject.ApplyModifiedProperties();

                #region 杀死所有预览的节点的动画
                if (Preview_Animator_TweenList != null && Preview_Animator_TweenList.Count > 0)
                {
                    for (int i = 0; i < Preview_Animator_TweenList.Count; i++)
                    {
                        if (Preview_Animator_TweenList[i] != null)
                        {
                            //Preview_Animator_TweenList[i].Complete();
                            Preview_Animator_TweenList[i].Kill();
                            if (LoadOriginalState)
                                Preview_Animator_TweenList[i].Rewind();
                        }
                    }
                }
                #endregion

                #region 复位所有节点的动画
                for (int i = 0; i < AnimateTweenNodes.arraySize; i++)
                {
                    SerializedProperty twnNode = AnimateTweenNodes.GetArrayElementAtIndex(i);
                    TweenNode tweenNode = TweenNodeConvert_From_SerialProperty(twnNode);
                    tweenNode.Progress = 0;

                    if (!tweenNode.Enabled)
                        continue;
                    else
                    {
                        if (tweenNode.ActivateOnlyToEnd)
                            continue;

                        if (LoadOriginalState)
                        {
                            BaseScript.Tweener_Rewind(tweenNode);
                        }
                        TweenNodeConvert_From_Class(twnNode, tweenNode);
                    }
                }
                #endregion

                #region 清空预览列表

                for (int i = 0; i < Preivew_Animator_CoroutineList_Play.Count; i++)
                {
                    EditorCoroutineUtility.StopCoroutine(Preivew_Animator_CoroutineList_Play[i]);
                }
                Preivew_Animator_CoroutineList_Play.Clear();

                if (Preivew_Animator_Coroutine_Stop != null)
                {
                    EditorCoroutineUtility.StopCoroutine(Preivew_Animator_Coroutine_Stop);
                    Preivew_Animator_Coroutine_Stop = null;
                }

                if (Preivew_Animator_Coroutine_ProgressCalcToSound != null)
                {
                    EditorCoroutineUtility.StopCoroutine(Preivew_Animator_Coroutine_ProgressCalcToSound);
                    Preivew_Animator_Coroutine_ProgressCalcToSound = null;
                }

                if (Preview_Animator_SoundList != null)
                {
                    for (int i = 0; i < Preview_Animator_SoundList.Count; i++)
                    {
                        if (Preview_Animator_SoundList[i] != null)
                        {
                            Preview_Animator_SoundList[i].Stop();
                            DestroyImmediate(Preview_Animator_SoundList[i].gameObject, true);
                            Preview_Animator_SoundList[i] = null;
                        }
                    }
                    Preview_Animator_SoundList.Clear();
                }
                #endregion

                Preview_Animator_TweenList.Clear();
                //DOTweenEditorPreview.Stop();

                if (LoadOriginalState && target != null)
                    AnimatorFeatures_Load(BaseScript);
            }
        }
        /// <summary>
        /// 延迟停止
        /// </summary>
        IEnumerator Preview_Animator_Coroutine_Stop()
        {
            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

            float dur = Animator_GlobalDuration.floatValue;
            yield return new EditorWaitForSeconds(dur * MaxTimer.floatValue * mgr.DurationMultiply);
            if (!Application.isPlaying)
            {
                while (Preview_AllAnimatorSound_IsStop())
                {
                    yield return null;
                }
                Preview_Animator_Stop();
            }
        }
        #endregion

        #region 音效预览
        /// <summary>
        /// 预览声音
        /// </summary>
        /// <param name="clip"></param>
        public AudioSource Preview_AnimatorSound_Creator(AudioClip clip, float vol, float pitch_min, float pitch_max)
        {
            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();
            if (MutePlay.boolValue)
                return null;
            GameObject obj = new GameObject();
            obj.name = "SoundPreviewer-" + "[" + clip.length.ToString("F2") + " s]-" + "[" + clip.channels + " ch]-" + "[" + clip.frequency + " hz]";
            AudioSource au = obj.AddComponent<AudioSource>();
            au.volume = mgr.Volume * 0.01f * vol;
            au.mute = mgr.VolumeMute;
            au.pitch = Random.Range(pitch_min, pitch_max);
            au.clip = clip;
            au.Play();
            XHud_AudioStoper sp = au.gameObject.AddComponent<XHud_AudioStoper>();
            sp.AudioSource = au;
            return au;
        }
        /// <summary>
        /// 所有的节点音效是否都停止了？
        /// </summary>
        /// <returns></returns>
        private bool Preview_AllAnimatorSound_IsStop()
        {
            bool sw = false;
            for (int i = 0; i < Preview_Animator_SoundList.Count; i++)
            {
                if (Preview_Animator_SoundList[i] != null && Preview_Animator_SoundList[i].isPlaying)
                {
                    sw = true;
                }
            }
            return sw;
        }
        #endregion

        #region 分类参数面板
        Rect param_rect;
        /// <summary>
        /// 面板 - 位置_Position
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="baseheight"></param>
        /// <param name="origin"></param>
        /// <param name="from"></param>
        /// <param name="end"></param>
        private void Param_Transform(Rect rect, int dir_index, SerializedProperty origin, SerializedProperty from, SerializedProperty end, TweenNodeType type, bool onlyToEnd)
        {
            if (!onlyToEnd)
            {
                ValueControl_ConnectorLine(rect, onlyToEnd, dir_index, origin, from, end, false);
                ValueControl_PropertyDrawer(rect, origin, dir_index, "默认", new Vector2(80, 0), type);
                ValueControl_PropertyDrawer(rect, from, dir_index, "起始", new Vector2(80, 25), type);
                ValueControl_PropertyDrawer(rect, end, dir_index, "结束", new Vector2(80, 50), type);
            }
            else
            {
                ValueControl_PropertyDrawer(rect, end, dir_index, "结束", new Vector2(80, 0), type);
            }
        }
        /// <summary>
        /// 面板 - 颜色_Color
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="baseheight"></param>        
        private void Param_Color(Rect rect, int dir_index, SerializedProperty origin, SerializedProperty from, SerializedProperty end, TweenNodeType type, bool onlyToEnd)
        {
            if (!onlyToEnd)
            {
                ValueControl_ConnectorLine(rect, onlyToEnd, dir_index, origin, from, end, false);
                ValueControl_PropertyDrawer(rect, origin, dir_index, "默认", new Vector2(80, 0), type);
                ValueControl_PropertyDrawer(rect, from, dir_index, "起始", new Vector2(80, 25), type);
                ValueControl_PropertyDrawer(rect, end, dir_index, "结束", new Vector2(80, 50), type);
            }
            else
            {
                ValueControl_PropertyDrawer(rect, end, dir_index, "结束", new Vector2(80, 0), type);
            }
        }
        /// <summary>
        /// 面板 - 淡化
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="baseheight"></param>        
        private void Param_Fader(Rect rect, int dir_index, SerializedProperty origin, SerializedProperty from, SerializedProperty end, TweenNodeType type, bool onlyToEnd)
        {
            if (!onlyToEnd)
            {
                ValueControl_ConnectorLine(rect, onlyToEnd, dir_index, origin, from, end, false);
                ValueControl_PropertyDrawer(rect, origin, dir_index, "默认", new Vector2(80, 0), type);
                ValueControl_PropertyDrawer(rect, from, dir_index, "起始", new Vector2(80, 25), type);
                ValueControl_PropertyDrawer(rect, end, dir_index, "结束", new Vector2(80, 50), type);
            }
            else
            {
                ValueControl_PropertyDrawer(rect, end, dir_index, "结束", new Vector2(80, 0), type);
            }
        }
        /// <summary>
        /// 面板 - 尺寸_Size
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="baseheight"></param>        
        private void Param_Size(Rect rect, int dir_index, SerializedProperty origin, SerializedProperty from, SerializedProperty end, TweenNodeType type, bool onlyToEnd)
        {
            if (!onlyToEnd)
            {
                ValueControl_ConnectorLine(rect, onlyToEnd, dir_index, origin, from, end, false);
                ValueControl_PropertyDrawer(rect, origin, dir_index, "默认", new Vector2(80, 0), type);
                ValueControl_PropertyDrawer(rect, from, dir_index, "起始", new Vector2(80, 25), type);
                ValueControl_PropertyDrawer(rect, end, dir_index, "结束", new Vector2(80, 50), type);
            }
            else
            {
                ValueControl_PropertyDrawer(rect, end, dir_index, "结束", new Vector2(80, 0), type);
            }
        }
        /// <summary>
        /// 面板 - 图形填充
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="baseheight"></param>        
        private void Param_Fill(Rect rect, int dir_index, SerializedProperty origin, SerializedProperty from, SerializedProperty end, TweenNodeType type, bool onlyToEnd)
        {
            if (!onlyToEnd)
            {
                ValueControl_ConnectorLine(rect, onlyToEnd, dir_index, origin, from, end, false);
                ValueControl_PropertyDrawer(rect, origin, dir_index, "默认", new Vector2(80, 0), type);
                ValueControl_PropertyDrawer(rect, from, dir_index, "起始", new Vector2(80, 25), type);
                ValueControl_PropertyDrawer(rect, end, dir_index, "结束", new Vector2(80, 50), type);
            }
            else
            {
                ValueControl_PropertyDrawer(rect, end, dir_index, "结束", new Vector2(80, 0), type);
            }
        }
        /// <summary>
        /// 面板 - 打字机
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="baseheight"></param>        
        private void Param_TextWritter(Rect rect, int dir_index, SerializedProperty origin, SerializedProperty from, SerializedProperty end, TweenNodeType type, bool onlyToEnd)
        {
            if (!onlyToEnd)
            {
                ValueControl_ConnectorLine(rect, onlyToEnd, dir_index, origin, from, end, false);
                ValueControl_PropertyDrawer(rect, origin, dir_index, "默认", new Vector2(80, 0), type);
                ValueControl_PropertyDrawer(rect, from, dir_index, "起始", new Vector2(80, 25), type);
                ValueControl_PropertyDrawer(rect, end, dir_index, "结束", new Vector2(80, 50), type);
            }
            else
            {
                ValueControl_PropertyDrawer(rect, end, dir_index, "结束", new Vector2(80, 0), type);
            }
        }
        /// <summary>
        /// 面板 - 自定义整数
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="baseheight"></param>        
        private void Param_Custom_Int(Rect rect, int dir_index, SerializedProperty origin, SerializedProperty from, SerializedProperty end, TweenNodeType type, bool onlyToEnd)
        {
            ValueControl_PropertyDrawer(rect, CustomValue_Int, dir_index, "当前", new Vector2(80, 0));

            if (!onlyToEnd)
            {
                ValueControl_ConnectorLine(rect, onlyToEnd, dir_index, origin, from, end);
                ValueControl_PropertyDrawer(rect, origin, dir_index, "默认", new Vector2(80, 25), type);
                ValueControl_PropertyDrawer(rect, from, dir_index, "起始", new Vector2(80, 50), type);
                ValueControl_PropertyDrawer(rect, end, dir_index, "结束", new Vector2(80, 75), type);
            }
            else
            {
                ValueControl_PropertyDrawer(rect, end, dir_index, "结束", new Vector2(80, 25), type);
            }
        }
        /// <summary>
        /// 面板 - 自定义浮点数
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="baseheight"></param>        
        private void Param_Custom_Float(Rect rect, int dir_index, SerializedProperty origin, SerializedProperty from, SerializedProperty end, TweenNodeType type, bool onlyToEnd)
        {
            ValueControl_PropertyDrawer(rect, CustomValue_Float, dir_index, "当前", new Vector2(80, 0));

            if (!onlyToEnd)
            {
                ValueControl_ConnectorLine(rect, onlyToEnd, dir_index, origin, from, end);
                ValueControl_PropertyDrawer(rect, origin, dir_index, "默认", new Vector2(80, 25), type);
                ValueControl_PropertyDrawer(rect, from, dir_index, "起始", new Vector2(80, 50), type);
                ValueControl_PropertyDrawer(rect, end, dir_index, "结束", new Vector2(80, 75), type);
            }
            else
            {
                ValueControl_PropertyDrawer(rect, end, dir_index, "结束", new Vector2(80, 25), type);
            }
        }
        /// <summary>
        /// 面板 - 自定义Vector2
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="baseheight"></param>        
        private void Param_Custom_Vector2(Rect rect, int dir_index, SerializedProperty origin, SerializedProperty from, SerializedProperty end, TweenNodeType type, bool onlyToEnd)
        {
            ValueControl_PropertyDrawer(rect, CustomValue_Vector2, dir_index, "当前", new Vector2(80, 0));

            if (!onlyToEnd)
            {
                ValueControl_ConnectorLine(rect, onlyToEnd, dir_index, origin, from, end);
                ValueControl_PropertyDrawer(rect, origin, dir_index, "默认", new Vector2(80, 25), type);
                ValueControl_PropertyDrawer(rect, from, dir_index, "起始", new Vector2(80, 50), type);
                ValueControl_PropertyDrawer(rect, end, dir_index, "结束", new Vector2(80, 75), type);
            }
            else
            {
                ValueControl_PropertyDrawer(rect, end, dir_index, "结束", new Vector2(80, 25), type);
            }
        }
        /// <summary>
        /// 面板 - 自定义Vector3
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="baseheight"></param>        
        private void Param_Custom_Vector3(Rect rect, int dir_index, SerializedProperty origin, SerializedProperty from, SerializedProperty end, TweenNodeType type, bool onlyToEnd)
        {
            ValueControl_PropertyDrawer(rect, CustomValue_Vector3, dir_index, "当前", new Vector2(80, 0));

            if (!onlyToEnd)
            {
                ValueControl_ConnectorLine(rect, onlyToEnd, dir_index, origin, from, end);
                ValueControl_PropertyDrawer(rect, origin, dir_index, "默认", new Vector2(80, 25), type);
                ValueControl_PropertyDrawer(rect, from, dir_index, "起始", new Vector2(80, 50), type);
                ValueControl_PropertyDrawer(rect, end, dir_index, "结束", new Vector2(80, 75), type);
            }
            else
            {
                ValueControl_PropertyDrawer(rect, end, dir_index, "结束", new Vector2(80, 25), type);
            }
        }
        /// <summary>
        /// 面板 - 自定义Vector4
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="baseheight"></param>        
        private void Param_Custom_Vector4(Rect rect, int dir_index, SerializedProperty origin, SerializedProperty from, SerializedProperty end, TweenNodeType type, bool onlyToEnd)
        {
            ValueControl_PropertyDrawer(rect, CustomValue_Vector4, dir_index, "当前", new Vector2(80, 0));

            if (!onlyToEnd)
            {
                ValueControl_ConnectorLine(rect, onlyToEnd, dir_index, origin, from, end);
                ValueControl_PropertyDrawer(rect, origin, dir_index, "默认", new Vector2(80, 25), type);
                ValueControl_PropertyDrawer(rect, from, dir_index, "起始", new Vector2(80, 50), type);
                ValueControl_PropertyDrawer(rect, end, dir_index, "结束", new Vector2(80, 75), type);
            }
            else
            {
                ValueControl_PropertyDrawer(rect, end, dir_index, "结束", new Vector2(80, 25), type);
            }
        }
        /// <summary>
        /// 面板 - 自定义颜色
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="baseheight"></param>        
        private void Param_Custom_Color(Rect rect, int dir_index, SerializedProperty origin, SerializedProperty from, SerializedProperty end, TweenNodeType type, bool onlyToEnd)
        {
            ValueControl_PropertyDrawer(rect, CustomValue_Color, dir_index, "当前", new Vector2(80, 0));

            if (!onlyToEnd)
            {
                ValueControl_ConnectorLine(rect, onlyToEnd, dir_index, origin, from, end);
                ValueControl_PropertyDrawer(rect, origin, dir_index, "默认", new Vector2(80, 25), type);
                ValueControl_PropertyDrawer(rect, from, dir_index, "起始", new Vector2(80, 50), type);
                ValueControl_PropertyDrawer(rect, end, dir_index, "结束", new Vector2(80, 75), type);
            }
            else
            {
                ValueControl_PropertyDrawer(rect, end, dir_index, "结束", new Vector2(80, 25), type);
            }
        }
        #endregion

        #region 动画值控件
        /// <summary>
        /// 动画值标记按钮颜色
        /// </summary>
        /// <param name="dir_index"></param>
        /// <param name="state"></param>
        private static void ValueControl_Color(int dir_index, string state)
        {
            switch (state)
            {
                case "当前":
                    GUI.color = Editor_XHud_GUI.GetColor(HudColor.神秘紫);
                    break;
                case "默认":
                    if (dir_index == 0) { GUI.color = Editor_XHud_GUI.GetColor(HudColor.魅力红); }
                    if (dir_index == 1) { GUI.color = Editor_XHud_GUI.GetColor(HudColor.魅力红); }
                    if (dir_index == 2) { GUI.color = Editor_XHud_GUI.GetColor(HudColor.深空灰); }
                    break;
                case "起始":
                    if (dir_index == 0) { GUI.color = Editor_XHud_GUI.GetColor(HudColor.工业蓝); }
                    if (dir_index == 1) { GUI.color = Editor_XHud_GUI.GetColor(HudColor.深空灰); }
                    if (dir_index == 2) { GUI.color = Editor_XHud_GUI.GetColor(HudColor.工业蓝); }
                    break;
                case "结束":
                    if (dir_index == 0) { GUI.color = Editor_XHud_GUI.GetColor(HudColor.深空灰); }
                    if (dir_index == 1) { GUI.color = Editor_XHud_GUI.GetColor(HudColor.警示黄); }
                    if (dir_index == 2) { GUI.color = Editor_XHud_GUI.GetColor(HudColor.警示黄); }
                    break;
            }
        }
        /// <summary>
        /// 动画值标记按钮控件绘制
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="prop"></param>
        /// <param name="dir_index"></param>
        /// <param name="title"></param>
        /// <param name="offset"></param>
        /// <param name="type"></param>
        private void ValueControl_PropertyDrawer(Rect rect, SerializedProperty prop, int dir_index, string title, Vector2 offset, TweenNodeType type = TweenNodeType.位移)
        {
            Rect re = new Rect(rect.x + offset.x, rect.y + offset.y, rect.width - (5 + offset.x), LineHeight);

            #region 根据类型显示属性框
            if (prop.type == "Vector4")
            {
                prop.vector4Value = Editor_XHud_GUI.Gui_InputField_Vector4(re, prop.vector4Value);
                prop.serializedObject.ApplyModifiedProperties();
            }
            else if (prop.type == "Vector3")
            {
                prop.vector3Value = Editor_XHud_GUI.Gui_InputField_Vector3(re, prop.vector3Value);
                prop.serializedObject.ApplyModifiedProperties();
            }
            else if (prop.type == "Vector2")
            {
                prop.vector2Value = Editor_XHud_GUI.Gui_InputField_Vector2(re, prop.vector2Value);
                prop.serializedObject.ApplyModifiedProperties();
            }
            else if (prop.type == "Color")
                Editor_XHud_GUI.Gui_Property_Field(re, "", prop, 0, 0);
            else if (prop.type == "float")
                Editor_XHud_GUI.Gui_Property_Field(re, "", prop, 0, 0);
            else if (prop.type == "int")
                Editor_XHud_GUI.Gui_Property_Field(re, "", prop, 0, 0);
            else if (prop.type == "string")
                Editor_XHud_GUI.Gui_Property_Field(re, "", prop, 0, 0);
            #endregion

            ValueControl_Color(dir_index, title);
            re.Set(rect.x + 5, rect.y + 3 + offset.y, 12, 12);

            #region 按下圆点按钮根据类型获取值
            if (Editor_XHud_GUI.Gui_Button(re, anim_dot, anim_dot_dark, true, "", "", Color.white))
            {
                Event eve = Event.current;
                switch (type)
                {
                    case TweenNodeType.位移:
                        if (eve.button == 0)
                        {
                            RectTransform trs = (RectTransform)mod_RectTransform.objectReferenceValue;
                            prop.vector3Value = trs.anchoredPosition3D;
                            prop.serializedObject.ApplyModifiedProperties();
                            AnimatorFeatures_Save(BaseScript);
                        }
                        else if (eve.button == 2)
                        {
                            prop.vector3Value = Vector3.zero;
                            prop.serializedObject.ApplyModifiedProperties();
                        }
                        else if (eve.button == 1)
                        {
                            RectTransform trs = (RectTransform)mod_RectTransform.objectReferenceValue;
                            Undo.RecordObject(mod_RectTransform.objectReferenceValue, "undotransform-position");
                            trs.anchoredPosition3D = prop.vector3Value;
                            AnimatorFeatures_Save(BaseScript);
                        }
                        break;
                    case TweenNodeType.旋转:
                        if (eve.button == 0)
                        {
                            RectTransform trs = (RectTransform)mod_RectTransform.objectReferenceValue;
                            prop.vector3Value = trs.localEulerAngles;
                            prop.serializedObject.ApplyModifiedProperties();
                            AnimatorFeatures_Save(BaseScript);
                        }
                        else if (eve.button == 2)
                        {
                            prop.vector3Value = Vector3.zero;
                            prop.serializedObject.ApplyModifiedProperties();
                        }
                        else if (eve.button == 1)
                        {
                            RectTransform trs = (RectTransform)mod_RectTransform.objectReferenceValue;
                            Undo.RecordObject(mod_RectTransform.objectReferenceValue, "undotransform-eulerangle");
                            trs.localEulerAngles = prop.vector3Value;
                            AnimatorFeatures_Save(BaseScript);
                        }
                        break;
                    case TweenNodeType.缩放:
                        if (eve.button == 0)
                        {
                            RectTransform trs = (RectTransform)mod_RectTransform.objectReferenceValue;
                            prop.vector3Value = trs.localScale;
                            prop.serializedObject.ApplyModifiedProperties();
                            AnimatorFeatures_Save(BaseScript);
                        }
                        else if (eve.button == 2)
                        {
                            prop.vector3Value = Vector3.zero;
                            prop.serializedObject.ApplyModifiedProperties();
                        }
                        else if (eve.button == 1)
                        {
                            RectTransform trs = (RectTransform)mod_RectTransform.objectReferenceValue;
                            Undo.RecordObject(mod_RectTransform.objectReferenceValue, "undotransform-localscale");
                            trs.localScale = prop.vector3Value;
                            AnimatorFeatures_Save(BaseScript);
                        }
                        break;
                    case TweenNodeType.颜色:
                        Graphic gc = BaseScript.Modules_RecognitionType();

                        if (eve.button == 0)
                        {
                            prop.colorValue = gc.color;
                            prop.serializedObject.ApplyModifiedProperties();
                            AnimatorFeatures_Save(BaseScript);
                        }
                        else if (eve.button == 2)
                        {
                            prop.colorValue = Color.white;
                            prop.serializedObject.ApplyModifiedProperties();
                        }
                        else if (eve.button == 1)
                        {
                            Undo.RecordObject(mod_RectTransform.objectReferenceValue, "undocolor-origin");
                            OriginalColor.colorValue = prop.colorValue;
                            AnimatorFeatures_Save(BaseScript);
                        }
                        break;
                    case TweenNodeType.淡化:
                        if (eve.button == 0)
                        {
                            prop.floatValue = BaseScript.mod_Image.color.a;
                            prop.serializedObject.ApplyModifiedProperties();
                            AnimatorFeatures_Save(BaseScript);
                        }
                        else if (eve.button == 2)
                        {
                            prop.floatValue = 0;
                            prop.serializedObject.ApplyModifiedProperties();
                        }
                        else if (eve.button == 1)
                        {
                            Undo.RecordObject(mod_RectTransform.objectReferenceValue, "undoAlpha-origin");
                            Color cc = BaseScript.mod_Image.color;
                            cc.a = prop.floatValue;
                            BaseScript.mod_Image.color = cc;
                            AnimatorFeatures_Save(BaseScript);
                        }
                        break;
                    case TweenNodeType.打字机:
                        if (eve.button == 0)
                        {
                            if (mod_Text.objectReferenceValue != null)
                            {
                                prop.stringValue = ((XHud_Module_Text)mod_Text.objectReferenceValue).text;
                                prop.serializedObject.ApplyModifiedProperties();
                            }
                            else if (mod_TmpText.objectReferenceValue != null)
                            {
                                prop.stringValue = ((XHud_Module_TmpText)mod_TmpText.objectReferenceValue).text;
                                prop.serializedObject.ApplyModifiedProperties();
                            }
                        }
                        else if (eve.button == 1)
                        {
                            if (mod_Text.objectReferenceValue != null)
                            {
                                XHud_Module_Text tt = ((XHud_Module_Text)mod_Text.objectReferenceValue);
                                Undo.RecordObject(tt, "undoText-origin");
                                tt.text = prop.stringValue;
                                mod_TmpText.serializedObject.ApplyModifiedProperties();
                            }
                            else if (mod_TmpText.objectReferenceValue != null)
                            {
                                XHud_Module_TmpText tmp = ((XHud_Module_TmpText)mod_TmpText.objectReferenceValue);
                                Undo.RecordObject(tmp, "undoTmpText-origin");
                                tmp.text = prop.stringValue;
                                mod_TmpText.serializedObject.ApplyModifiedProperties();
                            }
                            Repaint();
                            SceneView.RepaintAll();
                        }
                        else if (eve.button == 2)
                        {
                            prop.stringValue = null;
                            prop.serializedObject.ApplyModifiedProperties();
                        }
                        break;
                    case TweenNodeType.图像填充:
                        if (eve.button == 0)
                        {
                            Image img = (Image)mod_Image.objectReferenceValue;
                            prop.floatValue = img.fillAmount;
                            prop.serializedObject.ApplyModifiedProperties();
                            AnimatorFeatures_Save(BaseScript);
                        }
                        else if (eve.button == 2)
                        {
                            prop.floatValue = 0;
                            prop.serializedObject.ApplyModifiedProperties();
                        }
                        else if (eve.button == 1)
                        {
                            Undo.RecordObject(mod_RectTransform.objectReferenceValue, "undofill-end");
                            Image img = (Image)mod_Image.objectReferenceValue;
                            img.fillAmount = prop.floatValue;
                            AnimatorFeatures_Save(BaseScript);
                        }
                        break;
                    case TweenNodeType.尺寸:
                        if (eve.button == 0)
                        {
                            RectTransform trs = (RectTransform)mod_RectTransform.objectReferenceValue;
                            prop.vector2Value = trs.sizeDelta;
                            prop.serializedObject.ApplyModifiedProperties();
                            AnimatorFeatures_Save(BaseScript);
                        }
                        else if (eve.button == 2)
                        {
                            prop.vector2Value = Vector2.zero;
                            prop.serializedObject.ApplyModifiedProperties();
                        }
                        else if (eve.button == 1)
                        {
                            Undo.RecordObject(mod_RectTransform.objectReferenceValue, "undosize-origin");
                            RectTransform trs = (RectTransform)mod_RectTransform.objectReferenceValue;
                            trs.sizeDelta = prop.vector2Value;
                            AnimatorFeatures_Save(BaseScript);
                        }
                        break;
                }
                eve.Use();
                return;
            }
            #endregion

            GUI.color = Color.white;
            re.Set(rect.x + 28, rect.y + 2 + offset.y, 48, 15);
            Editor_XHud_GUI.Gui_Labelfield(re, title, HudFilled.无, HudColor.无, Color.white * 0.88f, TextAnchor.MiddleLeft, Vector2.zero, 12, Font_Light);
        }
        /// <summary>
        /// 动画值标记按钮之间的关系示意连线
        /// </summary>
        /// <param name="valuepanel_rect"></param>
        /// <param name="onlyToEnd"></param>
        /// <param name="dir_index"></param>
        /// <param name="original"></param>
        /// <param name="from"></param>
        /// <param name="end"></param>
        /// <param name="isCustomValue"></param>
        private void ValueControl_ConnectorLine(Rect valuepanel_rect, bool onlyToEnd, int dir_index, SerializedProperty original, SerializedProperty from, SerializedProperty end, bool isCustomValue = true)
        {
            Rect rect = valuepanel_rect;
            float Added = -25;

            #region 按钮图片设置
            Texture2D arw = null;
            Texture2D arw_p = null;

            float offset = 0;

            if (dir_index == 0)
            {
                offset = 31 + (!isCustomValue ? Added : 0);
                arw = anim_dir_bak;
                arw_p = anim_dir_bak_p;
            }
            if (dir_index == 1)
            {
                offset = 33 + (!isCustomValue ? Added : 0);
                arw = anim_dir_for_long;
                arw_p = anim_dir_for_long_p;
            }
            if (dir_index == 2)
            {
                offset = 59 + (!isCustomValue ? Added : 0);
                arw = anim_dir_for;
                arw_p = anim_dir_for_p;
            }
            #endregion

            #region 动向按钮
            rect.Set(valuepanel_rect.x - 10, rect.y + offset, arw.width, arw.height);
            if (Editor_XHud_GUI.Gui_Button(rect, arw, arw_p, true, "", "", Color.white))
            {
                Color tmp_color;
                Vector4 tmp_vector4;
                Vector3 tmp_vector3;
                Vector2 tmp_vector2;
                float tmp_float;
                int tmp_int;
                string tmp_string;

                switch (dir_index)
                {
                    case 0:
                        if (from.type == "Color") { tmp_color = from.colorValue; from.colorValue = original.colorValue; original.colorValue = tmp_color; }
                        if (from.type == "Vector4") { tmp_vector4 = from.vector4Value; from.vector4Value = original.vector4Value; original.vector4Value = tmp_vector4; }
                        if (from.type == "Vector3") { tmp_vector3 = from.vector3Value; from.vector3Value = original.vector3Value; original.vector3Value = tmp_vector3; }
                        if (from.type == "Vector2") { tmp_vector2 = from.vector2Value; from.vector2Value = original.vector2Value; original.vector2Value = tmp_vector2; }
                        if (from.type == "float") { tmp_float = from.floatValue; from.floatValue = original.floatValue; original.floatValue = tmp_float; }
                        if (from.type == "int") { tmp_int = from.intValue; from.intValue = original.intValue; original.intValue = tmp_int; }
                        if (from.type == "string") { tmp_string = from.stringValue; from.stringValue = original.stringValue; original.stringValue = tmp_string; }
                        break;
                    case 1:
                        if (from.type == "Color") { tmp_color = end.colorValue; end.colorValue = original.colorValue; original.colorValue = tmp_color; }
                        if (from.type == "Vector4") { tmp_vector4 = end.vector4Value; end.vector4Value = original.vector4Value; original.vector4Value = tmp_vector4; }
                        if (from.type == "Vector3") { tmp_vector3 = end.vector3Value; end.vector3Value = original.vector3Value; original.vector3Value = tmp_vector3; }
                        if (from.type == "Vector2") { tmp_vector2 = end.vector2Value; end.vector2Value = original.vector2Value; original.vector2Value = tmp_vector2; }
                        if (from.type == "float") { tmp_float = end.floatValue; end.floatValue = original.floatValue; original.floatValue = tmp_float; }
                        if (from.type == "int") { tmp_int = end.intValue; end.intValue = original.intValue; original.intValue = tmp_int; }
                        if (from.type == "string") { tmp_string = end.stringValue; end.stringValue = original.stringValue; original.stringValue = tmp_string; }
                        break;
                    case 2:
                        if (from.type == "Color") { tmp_color = from.colorValue; from.colorValue = end.colorValue; end.colorValue = tmp_color; }
                        if (from.type == "Vector4") { tmp_vector4 = from.vector4Value; from.vector4Value = end.vector4Value; end.vector4Value = tmp_vector4; }
                        if (from.type == "Vector3") { tmp_vector3 = from.vector3Value; from.vector3Value = end.vector3Value; end.vector3Value = tmp_vector3; }
                        if (from.type == "Vector2") { tmp_vector2 = from.vector2Value; from.vector2Value = end.vector2Value; end.vector2Value = tmp_vector2; }
                        if (from.type == "float") { tmp_float = from.floatValue; from.floatValue = end.floatValue; end.floatValue = tmp_float; }
                        if (from.type == "int") { tmp_int = from.intValue; from.intValue = end.intValue; end.intValue = tmp_int; }
                        if (from.type == "string") { tmp_string = from.stringValue; from.stringValue = end.stringValue; end.stringValue = tmp_string; }
                        break;
                }
            }
            #endregion
        }
        #endregion

        #region 解析TweenerNode

        private TweenNode TweenNodeConvert_From_SerialProperty(SerializedProperty args)
        {
            TweenNode node = new TweenNode();
            node.Indicator = args.FindPropertyRelative("Indicator").stringValue;
            node.ID = args.FindPropertyRelative("ID").intValue;
            node.Enabled = args.FindPropertyRelative("Enabled").boolValue;
            node.Type = (TweenNodeType)args.FindPropertyRelative("Type").enumValueIndex;
            node.Timings = args.FindPropertyRelative("Timings").stringValue;
            node.Duration = args.FindPropertyRelative("Duration").floatValue;
            node.Progress = args.FindPropertyRelative("Progress").floatValue;
            node.Delay = args.FindPropertyRelative("Delay").floatValue;
            node.Ease = (EaseMode)args.FindPropertyRelative("Ease").enumValueIndex;
            node.AnimationCurveName = args.FindPropertyRelative("AnimationCurveName").stringValue;
            node.Curve = args.FindPropertyRelative("Curve").animationCurveValue;
            node.Original_Int = args.FindPropertyRelative("Original_Int").intValue;
            node.Original_Float = args.FindPropertyRelative("Original_Float").floatValue;
            node.Original_Vector2 = args.FindPropertyRelative("Original_Vector2").vector2Value;
            node.Original_Vector3 = args.FindPropertyRelative("Original_Vector3").vector3Value;
            node.Original_Vector4 = args.FindPropertyRelative("Original_Vector4").vector4Value;
            node.Original_Color = args.FindPropertyRelative("Original_Color").colorValue;
            node.Original_String = args.FindPropertyRelative("Original_String").stringValue;
            node.ActivateFrom = args.FindPropertyRelative("ActivateFrom").boolValue;
            node.From_Int = args.FindPropertyRelative("From_Int").intValue;
            node.From_Float = args.FindPropertyRelative("From_Float").floatValue;
            node.From_Vector2 = args.FindPropertyRelative("From_Vector2").vector2Value;
            node.From_Vector3 = args.FindPropertyRelative("From_Vector3").vector3Value;
            node.From_Vector4 = args.FindPropertyRelative("From_Vector4").vector4Value;
            node.From_Color = args.FindPropertyRelative("From_Color").colorValue;
            node.From_String = args.FindPropertyRelative("From_String").stringValue;
            node.ActivateEnd = args.FindPropertyRelative("ActivateEnd").boolValue;
            node.ActivateOnlyToEnd = args.FindPropertyRelative("ActivateOnlyToEnd").boolValue;
            node.End_Int = args.FindPropertyRelative("End_Int").intValue;
            node.End_Float = args.FindPropertyRelative("End_Float").floatValue;
            node.End_Vector2 = args.FindPropertyRelative("End_Vector2").vector2Value;
            node.End_Vector3 = args.FindPropertyRelative("End_Vector3").vector3Value;
            node.End_Vector4 = args.FindPropertyRelative("End_Vector4").vector4Value;
            node.End_Color = args.FindPropertyRelative("End_Color").colorValue;
            node.End_String = args.FindPropertyRelative("End_String").stringValue;
            node.RotateMode = (XTweenRotationMode)args.FindPropertyRelative("RotateMode").enumValueIndex;
            node.LoopType = (XTween_LoopType)args.FindPropertyRelative("LoopType").enumValueIndex;
            node.LoopCount = args.FindPropertyRelative("LoopCount").intValue;
            node.IsFold = args.FindPropertyRelative("IsFold").boolValue;
            SerializedProperty sp_TweenSounds = args.FindPropertyRelative("TweenSounds");

            node.TweenSounds = new List<TweenSound>();

            for (int i = 0; i < sp_TweenSounds.arraySize; i++)
            {
                SerializedProperty sp_sounditem = sp_TweenSounds.GetArrayElementAtIndex(i);

                TweenSound s_node = new TweenSound();
                s_node.IsPlayed = sp_sounditem.FindPropertyRelative("IsPlayed").boolValue;
                s_node.Path = sp_sounditem.FindPropertyRelative("Path").stringValue;
                s_node.Percentage = sp_sounditem.FindPropertyRelative("Percentage").floatValue;
                s_node.Sound = (AudioClip)sp_sounditem.FindPropertyRelative("Sound").objectReferenceValue;
                s_node.Volume = sp_sounditem.FindPropertyRelative("Volume").floatValue;
                s_node.MaxPitch = sp_sounditem.FindPropertyRelative("MaxPitch").floatValue;
                s_node.MinPitch = sp_sounditem.FindPropertyRelative("MinPitch").floatValue;
                node.TweenSounds.Add(s_node);
            }

            return node;
        }

        private void TweenNodeConvert_From_Class(SerializedProperty property, TweenNode node)
        {
            property.FindPropertyRelative("Indicator").stringValue = node.Indicator;
            property.FindPropertyRelative("ID").intValue = node.ID;
            property.FindPropertyRelative("Enabled").boolValue = node.Enabled;
            property.FindPropertyRelative("Type").enumValueIndex = (int)node.Type;
            property.FindPropertyRelative("Timings").stringValue = node.Timings;
            property.FindPropertyRelative("Duration").floatValue = node.Duration;
            property.FindPropertyRelative("Progress").floatValue = node.Progress;
            property.FindPropertyRelative("Delay").floatValue = node.Delay;
            property.FindPropertyRelative("Ease").enumValueIndex = (int)node.Ease;
            property.FindPropertyRelative("AnimationCurveName").stringValue = node.AnimationCurveName;
            property.FindPropertyRelative("Curve").animationCurveValue = node.Curve;
            property.FindPropertyRelative("Original_Int").intValue = node.Original_Int;
            property.FindPropertyRelative("Original_Float").floatValue = node.Original_Float;
            property.FindPropertyRelative("Original_Vector2").vector2Value = node.Original_Vector2;
            property.FindPropertyRelative("Original_Vector3").vector3Value = node.Original_Vector3;
            property.FindPropertyRelative("Original_Vector4").vector4Value = node.Original_Vector4;
            property.FindPropertyRelative("Original_Color").colorValue = node.Original_Color;
            property.FindPropertyRelative("Original_String").stringValue = node.Original_String;
            property.FindPropertyRelative("ActivateFrom").boolValue = node.ActivateFrom;
            property.FindPropertyRelative("ActivateOnlyToEnd").boolValue = node.ActivateOnlyToEnd;
            property.FindPropertyRelative("From_Int").intValue = node.From_Int;
            property.FindPropertyRelative("From_Float").floatValue = node.From_Float;
            property.FindPropertyRelative("From_Vector2").vector2Value = node.From_Vector2;
            property.FindPropertyRelative("From_Vector3").vector3Value = node.From_Vector3;
            property.FindPropertyRelative("From_Vector4").vector4Value = node.From_Vector4;
            property.FindPropertyRelative("From_Color").colorValue = node.From_Color;
            property.FindPropertyRelative("From_String").stringValue = node.From_String;
            property.FindPropertyRelative("ActivateEnd").boolValue = node.ActivateEnd;
            property.FindPropertyRelative("End_Int").intValue = node.End_Int;
            property.FindPropertyRelative("End_Float").floatValue = node.End_Float;
            property.FindPropertyRelative("End_Vector2").vector2Value = node.End_Vector2;
            property.FindPropertyRelative("End_Vector3").vector3Value = node.End_Vector3;
            property.FindPropertyRelative("End_Vector4").vector4Value = node.End_Vector4;
            property.FindPropertyRelative("End_Color").colorValue = node.End_Color;
            property.FindPropertyRelative("End_String").stringValue = node.End_String;
            property.FindPropertyRelative("RotateMode").enumValueIndex = (int)node.RotateMode;
            property.FindPropertyRelative("LoopType").enumValueIndex = (int)node.LoopType;
            property.FindPropertyRelative("LoopCount").intValue = node.LoopCount;
            property.FindPropertyRelative("IsFold").boolValue = node.IsFold;
            SerializedProperty sp_TweenSounds = property.FindPropertyRelative("TweenSounds");

            for (int i = 0; i < sp_TweenSounds.arraySize; i++)
            {
                SerializedProperty sp_sounditem = sp_TweenSounds.GetArrayElementAtIndex(i);

                sp_sounditem.FindPropertyRelative("IsPlayed").boolValue = node.TweenSounds[i].IsPlayed;
                sp_sounditem.FindPropertyRelative("Path").stringValue = node.TweenSounds[i].Path;
                sp_sounditem.FindPropertyRelative("Percentage").floatValue = node.TweenSounds[i].Percentage;
                sp_sounditem.FindPropertyRelative("Sound").objectReferenceValue = node.TweenSounds[i].Sound;
                sp_sounditem.FindPropertyRelative("Volume").floatValue = node.TweenSounds[i].Volume;
                sp_sounditem.FindPropertyRelative("MaxPitch").floatValue = node.TweenSounds[i].MaxPitch;
                sp_sounditem.FindPropertyRelative("MinPitch").floatValue = node.TweenSounds[i].MinPitch;

                sp_sounditem.serializedObject.ApplyModifiedProperties();
            }
            sp_TweenSounds.serializedObject.ApplyModifiedProperties();
            property.serializedObject.ApplyModifiedProperties();
        }

        #endregion
    }
}