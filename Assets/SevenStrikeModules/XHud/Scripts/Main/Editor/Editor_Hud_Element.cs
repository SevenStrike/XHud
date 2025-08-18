namespace SevenStrikeModules.XHud.Hud
{
    using DG.DOTweenEditor;
    using DG.Tweening;
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.GuiLib;
    using SevenStrikeModules.XHud.Utilitys;
    using System.Collections;
    using System.Collections.Generic;
    using Unity.EditorCoroutines.Editor;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEditorInternal;
    using UnityEngine;
    using Color = UnityEngine.Color;
    using Image = UnityEngine.UI.Image;
    using Object = UnityEngine.Object;
    using Random = UnityEngine.Random;

    public enum ElementStatu
    {
        None = 0,
        InProject = 1,
        InScene = 2,
        InSceneNotPrefab = 3
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(Hud_Element), true)]
    public class Editor_Hud_Element : Editor
    {
        #region 组件 / 列表
        private Hud_Element BaseScript;
        private ReorderableList SounderList, AnimatorsList, ButtonList, OptionList, SliderList, ProgressList, ContainerList, ToggleList, TextList, TmpTextList;
        #endregion

        private float LineHeight;
        private bool BasicVars;

        #region 序列化属性
        private SerializedProperty SounderNodes, AnimatorNodes, ContainerNodes, OptionNodes, ButtonNodes, TextIsFold, TmpTextIsFold, SliderNodes, TextNodes, TmpTextNodes, DebugState, ProgressNodes, ToggleNodes, eve_on_element_in_start, eve_on_element_in_end, eve_on_element_out_start, eve_on_element_out_end, Indicator, AnimatorsMaxDuration, CanvasGroup, RectTransform, TriggerAction, ObjectTracker, CreateState, AnimateState, Element_Animators_GlobalDuration, OriginPoolName, AnimatorsIsFold, ButtonIsFold, OptionIsFold, SliderIsFold, SounderIsFold, ProgressIsFold, ContainerIsFold, ToggleIsFold, EventIsFold, AutoPlayAnimators, AutoPlayContainersAnimators, AutoStopPreview, Alpha, RMS_Enabled, RMS_InfoList, RMS_Name, CurrentPivot, OriginalName;
        #endregion

        #region Preview - Animator
        private bool Preivew_Animator_PlayingState;
        private List<Tweener> Preivew_Animator_TweenList = new List<Tweener>();
        private List<EditorCoroutine> Preivew_Animator_CoroutineList_Play = new List<EditorCoroutine>();
        private EditorCoroutine Preivew_Animator_Coroutine_Stop;
        #endregion

        #region Preview - AnimatorSound
        public List<AudioSource> Preivew_AnimatorSound_SoundList = new List<AudioSource>();
        private EditorCoroutine Preivew_AnimatorSound_Coroutine_Play;
        private List<EditorCoroutine> Preivew_AnimatorSound_CoroutineList_Stop = new List<EditorCoroutine>();
        #endregion

        #region Preview - HudSounder
        private List<AudioSource> Preivew_HudSounder_SoundList = new List<AudioSource>();
        private List<EditorCoroutine> Preivew_HudSounder_CoroutineList_Stop = new List<EditorCoroutine>();
        #endregion

        #region 图标
        private Texture2D icon_main, icon_sound, icon_anim, icon_button, icon_option, icon_slider, icon_progress, icon_container, icon_toggle, icon_text, icon_tmptext, icon_scan_r, icon_scan_p, icon_record_rms_r, icon_record_rms_p, resetanchorpos_r, resetanchorpos_p, prw_play_r, prw_play_p, prw_stop_r, prw_stop_p, usestate, animstate, dutation, comp_tracking, elelibsource, locate_r, locate_p, rms_move, rms_rotate, rms_anchor, rms_anchor_center, rms_scale, comp_alpha, comp_transform, comp_trigger, status, left_arrow_r, left_arrow_p, right_arrow_r, right_arrow_p;
        #endregion

        #region 批量模式查看索引
        private int ElementStatu_Index;
        private int ElementStatistic_Index;
        #endregion

        #region 选项文字
        string[] stroptions_enabled = new string[2] { "关闭", "开启" }, stroptions_debug = new string[2] { "关闭", "调试" }, stroptions_auto = new string[2] { "手动", "自动" };
        #endregion

        #region 批量化操作

        private Hud_Element[] SelectedObjects;

        private void GetAllTargets()
        {
            if (targets.Length > 1)
            {
                SelectedObjects = new Hud_Element[targets.Length];
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    var t = targets[i];
                    SelectedObjects[i] = (Hud_Element)t;
                }
            }
            else
            {
                SelectedObjects = new Hud_Element[targets.Length];
                SelectedObjects[0] = (Hud_Element)target;
            }
        }

        private bool IsMultiSelected()
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
            BaseScript = (Hud_Element)target;

            SerializedAllVariables();

            GetComponents();

            LineHeight = EditorGUIUtility.singleLineHeight;

            Vector2 ButtonSize = new Vector2(14, 14);

            ClearEmptys();

            #region 图标获取
            icon_main = util_XHUDGUI.GetIcon("Icons_Hud_Element/icon_main");
            icon_sound = util_XHUDGUI.GetIcon("Icons_Hud_Element/icon_sound");
            icon_button = util_XHUDGUI.GetIcon("Icons_Hud_Element/icon_button");
            icon_option = util_XHUDGUI.GetIcon("Icons_Hud_Element/icon_option");
            icon_slider = util_XHUDGUI.GetIcon("Icons_Hud_Element/icon_slider");
            icon_progress = util_XHUDGUI.GetIcon("Icons_Hud_Element/icon_progress");
            icon_container = util_XHUDGUI.GetIcon("Icons_Hud_Element/icon_container");
            icon_toggle = util_XHUDGUI.GetIcon("Icons_Hud_Element/icon_toggle");
            icon_text = util_XHUDGUI.GetIcon("Icons_Hud_Element/icon_text");
            icon_tmptext = util_XHUDGUI.GetIcon("Icons_Hud_Element/icon_tmptext");
            icon_anim = util_XHUDGUI.GetIcon("Icons_Hud_Element/icon_anim");
            icon_scan_r = util_XHUDGUI.GetIcon("Icons_Hud_Element/icon_scan_r");
            icon_scan_p = util_XHUDGUI.GetIcon("Icons_Hud_Element/icon_scan_p");
            icon_record_rms_r = util_XHUDGUI.GetIcon("Icons_Hud_Element/icon_record_rms_r");
            icon_record_rms_p = util_XHUDGUI.GetIcon("Icons_Hud_Element/icon_record_rms_p");
            resetanchorpos_r = util_XHUDGUI.GetIcon("Icons_Hud_Element/resetanchorpos_r");
            resetanchorpos_p = util_XHUDGUI.GetIcon("Icons_Hud_Element/resetanchorpos_p");
            prw_play_r = util_XHUDGUI.GetIcon("Icons_Hud_Element/prw_play_r");
            prw_play_p = util_XHUDGUI.GetIcon("Icons_Hud_Element/prw_play_p");
            prw_stop_r = util_XHUDGUI.GetIcon("Icons_Hud_Element/prw_stop_r");
            prw_stop_p = util_XHUDGUI.GetIcon("Icons_Hud_Element/prw_stop_p");
            usestate = util_XHUDGUI.GetIcon("Icons_Hud_Element/usestate");
            animstate = util_XHUDGUI.GetIcon("Icons_Hud_Element/animstate");
            dutation = util_XHUDGUI.GetIcon("Icons_Hud_Element/dutation");
            elelibsource = util_XHUDGUI.GetIcon("Icons_Hud_Element/elelibsource");
            locate_r = util_XHUDGUI.GetIcon("Icons_Hud_Element/locate_r");
            locate_p = util_XHUDGUI.GetIcon("Icons_Hud_Element/locate_p");
            rms_move = util_XHUDGUI.GetIcon("Icons_Hud_Element/rms_move");
            rms_rotate = util_XHUDGUI.GetIcon("Icons_Hud_Element/rms_rotate");
            rms_anchor = util_XHUDGUI.GetIcon("Icons_Hud_Element/rms_anchor");
            rms_scale = util_XHUDGUI.GetIcon("Icons_Hud_Element/rms_scale");
            rms_anchor_center = util_XHUDGUI.GetIcon("Icons_Hud_Element/rms_anchor_center");
            comp_alpha = util_XHUDGUI.GetIcon("Icons_Hud_Element/comp_alpha");
            comp_transform = util_XHUDGUI.GetIcon("Icons_Hud_Element/comp_transform");
            comp_trigger = util_XHUDGUI.GetIcon("Icons_Hud_Element/comp_trigger");
            comp_tracking = util_XHUDGUI.GetIcon("Icons_Hud_Element/comp_tracking");
            status = util_XHUDGUI.GetIcon("Icons_Hud_Element/status");
            left_arrow_r = util_XHUDGUI.GetIcon("Icons_Hud_Element/left_arrow_r");
            left_arrow_p = util_XHUDGUI.GetIcon("Icons_Hud_Element/left_arrow_p");
            right_arrow_r = util_XHUDGUI.GetIcon("Icons_Hud_Element/right_arrow_r");
            right_arrow_p = util_XHUDGUI.GetIcon("Icons_Hud_Element/right_arrow_p");
            #endregion

            Hud_Manager mgr = util_Dashboard.HudManagerGet();

            #region ReorderableList - Sounder
            SounderList = new ReorderableList(serializedObject, SounderNodes)
            {
                displayAdd = false,
                displayRemove = true,
                draggable = true,

                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "元素音效列表");
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    float titleheight = rect.y + 6;
                    float baseheight = rect.y + (rect.height - 25);

                    SerializedProperty sp_root = SounderNodes.GetArrayElementAtIndex(index);
                    if (sp_root != null)
                    {
                        SerializedProperty sp_sounder = sp_root.FindPropertyRelative("Sounder");
                        Hud_Sounder sounder = (Hud_Sounder)sp_sounder.objectReferenceValue;
                        if (sounder != null)
                        {
                            string title = "";
                            if (!string.IsNullOrEmpty(sounder.Indicator))
                                title += sounder.Indicator;
                            else
                                title += sounder.gameObject.name;
                            util_XHUDGUI.Gui_Labelfield(new Rect(rect.width - (rect.width - 90), titleheight - 2, (rect.width * 0.45f) - 20, LineHeight), title, HudFilled.无, HudColor.亮白, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 11, TextClipping.Clip);

                            GUI.color = util_Dashboard.Theme_Primary;
                            util_XHUDGUI.Gui_Icon(new Rect(rect.width - (rect.width - 60), titleheight, 10, 10), icon_sound);

                            SerializedObject so_sounder = new SerializedObject(sounder);

                            SerializedProperty sp_PitchMin = so_sounder.FindProperty("Pitch_Min");
                            SerializedProperty sp_PitchMax = so_sounder.FindProperty("Pitch_Max");
                            SerializedProperty sp_Delay = so_sounder.FindProperty("DelayTime");
                            SerializedProperty sp_Vol = so_sounder.FindProperty("Volume");
                            SerializedProperty sp_SoundName = so_sounder.FindProperty("SoundName");

                            so_sounder.Update();

                            GUI.color = Color.white;

                            #region 最小音高
                            util_XHUDGUI.Gui_Property_Field(new Rect(rect.width - 40, rect.y + 4, 30, 19), "音高", sp_PitchMin, 0, 60, LineHeight, 30);
                            #endregion

                            #region 最大音高                         
                            util_XHUDGUI.Gui_Property_Field(new Rect(rect.width + 25, rect.y + 4, 30, 19), "", sp_PitchMax, 0, 25, LineHeight, 5);
                            #endregion

                            #region 音量          
                            float fieldwidth = EditorGUIUtility.fieldWidth;
                            EditorGUIUtility.fieldWidth = 40;
                            EditorGUI.Slider(new Rect(rect.x + 5, rect.y + 30, 120, 19), sp_Vol, 0, 1, "");
                            EditorGUIUtility.fieldWidth = fieldwidth;
                            so_sounder.ApplyModifiedProperties();
                            #endregion

                            #region 延迟                         
                            util_XHUDGUI.Gui_Property_Field(new Rect(rect.width - 40, rect.y + 30, 30, 19), "延迟", sp_Delay, 0, 60, LineHeight, 30);
                            #endregion

                            #region 音效列表
                            if (mgr.Hud_Sounds != null)
                            {
                                string[] collist = mgr.Hud_Sounds.SoundLibrary_GetSoundNames();
                                Color bgcol = GUI.color;
                                GUI.color = util_Dashboard.Theme_Primary;
                                util_XHUDGUI.Gui_PopupWithString(new Rect(rect.width + 25, rect.y + 30, 30, 19), ref sp_SoundName, collist, HudFilled.实体, HudColor.亮白, Color.black);
                                GUI.color = bgcol;
                            }
                            #endregion

                            so_sounder.ApplyModifiedProperties();
                        }
                    }
                },
                onSelectCallback = (ReorderableList list) =>
                {
                    SerializedProperty sp_sounder = SounderNodes.GetArrayElementAtIndex(list.index).FindPropertyRelative("Sounder");
                    EditorGUIUtility.PingObject(sp_sounder.objectReferenceValue);

                    #region 播放音效
                    SerializedObject so_sod = new SerializedObject((Hud_Sounder)sp_sounder.objectReferenceValue);
                    SerializedProperty sp_name = so_sod.FindProperty("SoundName");
                    SerializedProperty sp_vol = so_sod.FindProperty("Volume");
                    SerializedProperty sp_delay = so_sod.FindProperty("DelayTime");
                    SerializedProperty sp_pit_min = so_sod.FindProperty("Pitch_Min");
                    SerializedProperty sp_pit_max = so_sod.FindProperty("Pitch_Max");
                    SerializedProperty sp_userandom = so_sod.FindProperty("UseRandomPitch");

                    float x_vol = sp_vol.floatValue;
                    float x_pit_min = sp_pit_min.floatValue;
                    float x_pit_max = sp_pit_max.floatValue;
                    float x_delay = sp_delay.floatValue;
                    bool x_userandom = false;
                    if (sp_userandom.intValue == 1)
                        x_userandom = true;

                    AudioClip x_clip = mgr.Hud_Sounds.SoundLibrary_GetSound(sp_name.stringValue);
                    Preivew_HudSounder_CoroutineList_Stop.Add(EditorCoroutineUtility.StartCoroutineOwnerless(Preview_HudSounder_Play(x_vol, x_pit_min, x_pit_max, x_userandom, x_clip, x_delay)));
                    #endregion
                },
                elementHeightCallback = index =>
                {
                    return 2.8f * LineHeight;
                }
            };
            #endregion

            #region ReorderableList - Animator
            AnimatorsList = new ReorderableList(serializedObject, AnimatorNodes)
            {
                displayAdd = false,
                displayRemove = true,
                draggable = true,

                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "动画器列表");
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    if (AnimatorNodes == null)
                        return;
                    if (AnimatorNodes.arraySize <= 0)
                        return;
                    SerializedProperty sp_root = AnimatorNodes.GetArrayElementAtIndex(index);
                    if (sp_root != null)
                    {
                        SerializedProperty sp_animator = sp_root.FindPropertyRelative("Animator");
                        SerializedProperty sp_delay = sp_root.FindPropertyRelative("DelayTime");
                        Hud_Animator animator = (Hud_Animator)sp_animator.objectReferenceValue;

                        if (animator != null)
                        {
                            float titleheight = rect.y + 7;
                            float baseheight = rect.y + 25;

                            #region 类型图标         
                            GUI.color = util_Dashboard.Theme_Primary;
                            util_XHUDGUI.Gui_Icon(new Rect(rect.width - (rect.width - 60), titleheight + 1, 10, 10), icon_anim);
                            GUI.color = Color.white;
                            #endregion

                            #region 标题文字
                            string title = "";
                            string indicator = animator.GetIndicator();
                            if (!string.IsNullOrEmpty(indicator))
                                title += indicator;
                            else
                                title += animator.gameObject.name;
                            util_XHUDGUI.Gui_Labelfield(new Rect(rect.width - (rect.width - 90), titleheight - 2, (rect.width * 0.45f) - 20, LineHeight), title, HudFilled.无, HudColor.亮白, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 11, TextClipping.Clip);
                            #endregion

                            #region 延迟
                            util_XHUDGUI.Gui_Property_Field(new Rect(rect.width - 5, rect.y + 4, 30, 19), "D", sp_delay, 10, 40, LineHeight, 15);
                            #endregion

                            #region 速率                         
                            SerializedObject so_anim = new SerializedObject(animator);
                            so_anim.Update();

                            SerializedProperty sp_glodur = so_anim.FindProperty("Animator_GlobalDuration");
                            SerializedProperty sp_maxdur = so_anim.FindProperty("MaxTimerWithGlobalDuration");

                            util_XHUDGUI.Gui_Property_Field(new Rect(rect.width - 50, rect.y + 4, 30, 19), "G", sp_glodur, 10, 40, LineHeight, 15);

                            util_XHUDGUI.Gui_Labelfield_Thin(new Rect(rect.width - 80, rect.y + 4, 30, 19), $"{sp_maxdur.floatValue.ToString()} s", HudFilled.无, HudColor.无, util_Dashboard.Theme_Primary, TextAnchor.MiddleCenter, Vector2.zero, 11);

                            so_anim.ApplyModifiedProperties();
                            #endregion

                            sp_animator.serializedObject.ApplyModifiedProperties();
                            sp_root.serializedObject.ApplyModifiedProperties();
                        }
                    }
                },
                onSelectCallback = (ReorderableList list) =>
                {
                    if (!Application.isPlaying)
                    {
                        Preview_Animator_Stop();
                    }

                    Preview_Animator_PlayAt(list.index);
                    SerializedProperty sp_root = AnimatorNodes.GetArrayElementAtIndex(list.index);
                    if (sp_root != null)
                    {
                        SerializedProperty sp_animator = sp_root.FindPropertyRelative("Animator");
                        EditorGUIUtility.PingObject(sp_animator.objectReferenceValue);
                    }
                },
                elementHeightCallback = index =>
                {
                    return 1.5f * LineHeight;
                }
            };
            #endregion

            #region ReorderableList - Button
            ButtonList = new ReorderableList(serializedObject, ButtonNodes)
            {
                displayAdd = false,
                displayRemove = true,
                draggable = true,

                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "按钮列表");
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    float titleheight = rect.y + 7;
                    float baseheight = rect.y + (rect.height - 25);

                    SerializedProperty sp_root = ButtonNodes.GetArrayElementAtIndex(index);
                    if (sp_root != null)
                    {
                        SerializedProperty sp_btn = sp_root.FindPropertyRelative("Button");
                        Hud_Button btn = (Hud_Button)sp_btn.objectReferenceValue;
                        if (btn != null)
                        {
                            string title = "";
                            if (!string.IsNullOrEmpty(btn.Indicator))
                                title += btn.Indicator;
                            else
                                title += btn.gameObject.name;
                            util_XHUDGUI.Gui_Labelfield(new Rect(rect.width - (rect.width - 90), titleheight - 2, (rect.width * 0.45f) - 20, LineHeight), title, HudFilled.无, HudColor.亮白, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 11, TextClipping.Clip);

                            GUI.color = util_Dashboard.Theme_Primary;
                            util_XHUDGUI.Gui_Icon(new Rect(rect.width - (rect.width - 60), titleheight + 1, 10, 10), icon_button);

                            GUI.color = Color.white;
                        }
                    }
                },
                onSelectCallback = (ReorderableList list) =>
                {
                    SerializedProperty sp_btn = ButtonNodes.GetArrayElementAtIndex(list.index).FindPropertyRelative("Button");
                    //Hud_Button btn = (Hud_Button)sp_btn.objectReferenceValue;
                    EditorGUIUtility.PingObject(sp_btn.objectReferenceValue);
                },
                elementHeightCallback = index =>
                {
                    return 1.5f * LineHeight;
                }
            };
            #endregion

            #region ReorderableList - Option
            OptionList = new ReorderableList(serializedObject, OptionNodes)
            {
                displayAdd = false,
                displayRemove = true,
                draggable = true,

                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "选项列表");
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    float titleheight = rect.y + 7;
                    float baseheight = rect.y + (rect.height - 25);

                    SerializedProperty sp_root = OptionNodes.GetArrayElementAtIndex(index);
                    if (sp_root != null)
                    {
                        SerializedProperty sp_opt = sp_root.FindPropertyRelative("Option");
                        Hud_Option opt = (Hud_Option)sp_opt.objectReferenceValue;
                        if (opt != null)
                        {
                            string title = "";
                            if (!string.IsNullOrEmpty(opt.Indicator))
                                title += opt.Indicator;
                            else
                                title += opt.gameObject.name;
                            util_XHUDGUI.Gui_Labelfield(new Rect(rect.width - (rect.width - 90), titleheight - 2, (rect.width * 0.45f) - 20, LineHeight), title, HudFilled.无, HudColor.亮白, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 11, TextClipping.Clip);

                            GUI.color = util_Dashboard.Theme_Primary;
                            util_XHUDGUI.Gui_Icon(new Rect(rect.width - (rect.width - 60), titleheight + 1, 10, 10), icon_option);

                            GUI.color = Color.white;
                        }
                    }
                },
                onSelectCallback = (ReorderableList list) =>
                {
                    SerializedProperty sp_opt = OptionNodes.GetArrayElementAtIndex(list.index).FindPropertyRelative("Option");
                    EditorGUIUtility.PingObject(sp_opt.objectReferenceValue);
                },
                elementHeightCallback = index =>
                {
                    return 1.5f * LineHeight;
                }
            };
            #endregion

            #region ReorderableList - Slider
            SliderList = new ReorderableList(serializedObject, SliderNodes)
            {
                displayAdd = false,
                displayRemove = true,
                draggable = true,

                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "滑动条列表");
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    float titleheight = rect.y + 7;
                    float baseheight = rect.y + (rect.height - 25);

                    SerializedProperty sp_root = SliderNodes.GetArrayElementAtIndex(index);
                    if (sp_root != null)
                    {
                        SerializedProperty sp_sli = sp_root.FindPropertyRelative("Slider");
                        Hud_Slider sli = (Hud_Slider)sp_sli.objectReferenceValue;
                        if (sli != null)
                        {
                            string title = "";
                            if (!string.IsNullOrEmpty(sli.Indicator))
                                title += sli.Indicator;
                            else
                                title += sli.gameObject.name;
                            util_XHUDGUI.Gui_Labelfield(new Rect(rect.width - (rect.width - 90), titleheight - 2, (rect.width * 0.45f) - 20, LineHeight), title, HudFilled.无, HudColor.亮白, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 11, TextClipping.Clip);

                            GUI.color = util_Dashboard.Theme_Primary;
                            util_XHUDGUI.Gui_Icon(new Rect(rect.width - (rect.width - 60), titleheight + 1, 10, 10), icon_slider);

                            GUI.color = Color.white;
                        }
                    }
                },
                onSelectCallback = (ReorderableList list) =>
                {
                    SerializedProperty sp_opt = SliderNodes.GetArrayElementAtIndex(list.index).FindPropertyRelative("Slider");
                    EditorGUIUtility.PingObject(sp_opt.objectReferenceValue);
                },
                elementHeightCallback = index =>
                {
                    return 1.5f * LineHeight;
                }
            };
            #endregion

            #region ReorderableList - Progress
            ProgressList = new ReorderableList(serializedObject, ProgressNodes)
            {
                displayAdd = false,
                displayRemove = true,
                draggable = true,

                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "进度条列表");
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    float titleheight = rect.y + 7;
                    float baseheight = rect.y + (rect.height - 25);

                    SerializedProperty sp_root = ProgressNodes.GetArrayElementAtIndex(index);
                    if (sp_root != null)
                    {
                        SerializedProperty sp_pro = sp_root.FindPropertyRelative("Progress");
                        Hud_Progress pro = (Hud_Progress)sp_pro.objectReferenceValue;
                        if (pro != null)
                        {
                            string title = "";
                            if (!string.IsNullOrEmpty(pro.Indicator))
                                title += pro.Indicator;
                            else
                                title += pro.gameObject.name;
                            util_XHUDGUI.Gui_Labelfield(new Rect(rect.width - (rect.width - 90), titleheight - 2, (rect.width * 0.45f) - 20, LineHeight), title, HudFilled.无, HudColor.亮白, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 11, TextClipping.Clip);

                            GUI.color = util_Dashboard.Theme_Primary;
                            util_XHUDGUI.Gui_Icon(new Rect(rect.width - (rect.width - 60), titleheight + 1, 10, 10), icon_progress);

                            GUI.color = Color.white;
                        }
                    }
                },
                onSelectCallback = (ReorderableList list) =>
                {
                    SerializedProperty sp_opt = ProgressNodes.GetArrayElementAtIndex(list.index).FindPropertyRelative("Progress");
                    EditorGUIUtility.PingObject(sp_opt.objectReferenceValue);
                },
                elementHeightCallback = index =>
                {
                    return 1.5f * LineHeight;
                }
            };
            #endregion

            #region ReorderableList - Container
            ContainerList = new ReorderableList(serializedObject, ContainerNodes)
            {
                displayAdd = false,
                displayRemove = true,
                draggable = true,

                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "容器列表");
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    float titleheight = rect.y + 7;
                    float baseheight = rect.y + (rect.height - 25);

                    SerializedProperty sp_root = ContainerNodes.GetArrayElementAtIndex(index);
                    if (sp_root != null)
                    {
                        SerializedProperty sp_con = sp_root.FindPropertyRelative("Container");
                        Hud_Container con = (Hud_Container)sp_con.objectReferenceValue;
                        if (con != null)
                        {
                            int count = con.Con_Get_ItemsCount();
                            string title = "";
                            if (!string.IsNullOrEmpty(con.Indicator))
                                title += con.Indicator;
                            else
                                title += con.gameObject.name;
                            util_XHUDGUI.Gui_Labelfield(new Rect(rect.width - (rect.width - 90), titleheight - 2, (rect.width * 0.45f) - 20, LineHeight), title, HudFilled.无, HudColor.亮白, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 11, TextClipping.Clip);

                            GUI.color = util_Dashboard.Theme_Primary;
                            util_XHUDGUI.Gui_Icon(new Rect(rect.width - (rect.width - 60), titleheight + 1, 10, 10), icon_container);

                            util_XHUDGUI.Gui_Labelfield(new Rect(rect.width - 20, titleheight - 2, 60, LineHeight), count.ToString() + " 项", HudFilled.无, HudColor.亮白, Color.white, TextAnchor.MiddleRight, Vector2.zero, 11, TextClipping.Clip);

                            GUI.color = Color.white;
                        }
                    }
                },
                onSelectCallback = (ReorderableList list) =>
                {
                    SerializedProperty sp_con = ContainerNodes.GetArrayElementAtIndex(list.index).FindPropertyRelative("Container");
                    EditorGUIUtility.PingObject(sp_con.objectReferenceValue);
                },
                elementHeightCallback = index =>
                {
                    return 1.5f * LineHeight;
                }
            };
            #endregion

            #region ReorderableList - Toggle
            ToggleList = new ReorderableList(serializedObject, ToggleNodes)
            {
                displayAdd = false,
                displayRemove = true,
                draggable = true,

                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "开关列表");
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    float titleheight = rect.y + 7;
                    float baseheight = rect.y + (rect.height - 25);

                    SerializedProperty sp_root = ToggleNodes.GetArrayElementAtIndex(index);
                    if (sp_root != null)
                    {
                        SerializedProperty sp_tog = sp_root.FindPropertyRelative("Toggle");
                        Hud_Toggle tog = (Hud_Toggle)sp_tog.objectReferenceValue;
                        if (tog != null)
                        {
                            string title = "";
                            if (!string.IsNullOrEmpty(tog.Indicator))
                                title += tog.Indicator;
                            else
                                title += tog.gameObject.name;
                            util_XHUDGUI.Gui_Labelfield(new Rect(rect.width - (rect.width - 90), titleheight - 2, (rect.width * 0.45f) - 20, LineHeight), title, HudFilled.无, HudColor.亮白, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 11, TextClipping.Clip);

                            GUI.color = util_Dashboard.Theme_Primary;
                            util_XHUDGUI.Gui_Icon(new Rect(rect.width - (rect.width - 60), titleheight + 1, 10, 10), icon_toggle);

                            GUI.color = Color.white;
                        }
                    }
                },
                onSelectCallback = (ReorderableList list) =>
                {
                    SerializedProperty sp_opt = ToggleNodes.GetArrayElementAtIndex(list.index).FindPropertyRelative("Toggle");
                    EditorGUIUtility.PingObject(sp_opt.objectReferenceValue);
                },
                elementHeightCallback = index =>
                {
                    return 1.5f * LineHeight;
                }
            };
            #endregion

            #region ReorderableList - Text
            TextList = new ReorderableList(serializedObject, TextNodes)
            {
                displayAdd = false,
                displayRemove = true,
                draggable = true,

                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "文字列表");
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    float titleheight = rect.y + 7;
                    float baseheight = rect.y + (rect.height - 25);

                    SerializedProperty sp_root = TextNodes.GetArrayElementAtIndex(index);
                    if (sp_root != null)
                    {
                        SerializedProperty sp_text = sp_root.FindPropertyRelative("Text");
                        Hud_Text txt = (Hud_Text)sp_text.objectReferenceValue;
                        if (txt != null)
                        {
                            string title = "";
                            if (!string.IsNullOrEmpty(txt.Indicator))
                                title += txt.Indicator;
                            else
                                title += txt.gameObject.name;
                            util_XHUDGUI.Gui_Labelfield(new Rect(rect.width - (rect.width - 90), titleheight - 2, (rect.width * 0.45f) - 20, LineHeight), title, HudFilled.无, HudColor.亮白, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 11, TextClipping.Clip);

                            GUI.color = util_Dashboard.Theme_Primary;
                            util_XHUDGUI.Gui_Icon(new Rect(rect.width - (rect.width - 60), titleheight + 1, 10, 10), icon_text);

                            GUI.color = Color.white;
                        }
                    }
                },
                onSelectCallback = (ReorderableList list) =>
                {
                    SerializedProperty sp_text = TextNodes.GetArrayElementAtIndex(list.index).FindPropertyRelative("Text");
                    EditorGUIUtility.PingObject(sp_text.objectReferenceValue);
                },
                elementHeightCallback = index =>
                {
                    return 1.5f * LineHeight;
                }
            };
            #endregion

            #region ReorderableList - TmpText
            TmpTextList = new ReorderableList(serializedObject, TmpTextNodes)
            {
                displayAdd = false,
                displayRemove = true,
                draggable = true,

                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "Tmp文字列表");
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    float titleheight = rect.y + 7;
                    float baseheight = rect.y + (rect.height - 25);

                    SerializedProperty sp_root = TmpTextNodes.GetArrayElementAtIndex(index);
                    if (sp_root != null)
                    {
                        SerializedProperty sp_text = sp_root.FindPropertyRelative("TmpText");
                        Hud_TmpText txt = (Hud_TmpText)sp_text.objectReferenceValue;
                        if (txt != null)
                        {
                            string title = "";
                            if (!string.IsNullOrEmpty(txt.Indicator))
                                title += txt.Indicator;
                            else
                                title += txt.gameObject.name;
                            util_XHUDGUI.Gui_Labelfield(new Rect(rect.width - (rect.width - 90), titleheight - 2, (rect.width * 0.45f) - 20, LineHeight), title, HudFilled.无, HudColor.亮白, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 11, TextClipping.Clip);

                            GUI.color = util_Dashboard.Theme_Primary;
                            util_XHUDGUI.Gui_Icon(new Rect(rect.width - (rect.width - 60), titleheight + 1, 10, 10), icon_tmptext);

                            GUI.color = Color.white;
                        }
                    }
                },
                onSelectCallback = (ReorderableList list) =>
                {
                    SerializedProperty sp_text = TmpTextNodes.GetArrayElementAtIndex(list.index).FindPropertyRelative("TmpText");
                    EditorGUIUtility.PingObject(sp_text.objectReferenceValue);
                },
                elementHeightCallback = index =>
                {
                    return 1.5f * LineHeight;
                }
            };
            #endregion

            if (mgr != null)
            {
                if (mgr.hm_RMS_GetResolutionNodes().Length <= 0 && RMS_InfoList.arraySize > 0)
                {
                    string res = util_XHUDGUI.Open(XHudDialogType.警告, "HudElement消息", "RMS残留信息", "发现残留匹配分辨率信息列表，是否需要清空？请谨慎此操作！", "清空", "暂不", 1);
                    if (res == "清空")
                    {
                        RMS_InfoList.ClearArray();
                        RMS_InfoList.serializedObject.ApplyModifiedProperties();
                    }
                }
            }

            GetAllTargets();
        }

        private void OnDisable()
        {
            if (!Application.isPlaying)
            {
                if (BaseScript != null)
                {
                    RMS_Redraw();

                    Preview_Animator_Stop();

                    Preview_AnimatorSound_CoroutineList_Stop();

                    Preview_HudSounder_Coroutine_Stop();
                }
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (string.IsNullOrEmpty(BaseScript.Indicator))
                util_XHUDGUI.Gui_Layout_Banner(icon_main, HudFilled.实体, HudColor.深空灰, "Hud - 元素", Color.white, 20, 20);
            else
                util_XHUDGUI.Gui_Layout_Banner(icon_main, HudFilled.实体, HudColor.深空灰, Indicator.stringValue, Color.white, 20, 20);

            Hud_Manager mgr = util_Dashboard.HudManagerGet();

            #region 快捷功能
            util_XHUDGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "快捷功能", util_Dashboard.Theme_Primary);
            util_XHUDGUI.Gui_Layout_Space(10);

            util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            util_XHUDGUI.Gui_Layout_Space(10);

            #region 扫描子元素
            GUI.enabled = true;
            if (util_XHUDGUI.Gui_Layout_Button(15, "扫描子元素", icon_scan_r, icon_scan_p))
            {
                if (Application.isPlaying)
                {
                    util_XHUDGUI.Open(XHudDialogType.警告, "HudElement消息", "扫描子元素", "程序正在运行，无法在运行期间执行此功能！", "明白");
                    return;
                }
                GetAllAnimators();
                GetAllText();
                GetAllTmpText();
                GetAllSounder();
                GetAllButtons();
                GetAllOptions();
                GetAllSliders();
                GetAllProgress();
                GetAllContainer();
                GetAllToggle();
                GetModulesResult();
                return;
            }
            #endregion

            GUILayout.FlexibleSpace();

            #region 记录设计布局
            GUI.enabled = true;
            if (RMS_Enabled.boolValue)
            {
                if (util_XHUDGUI.Gui_Layout_Button(15, "记录RMS布局信息", icon_record_rms_r, icon_record_rms_p))
                {
                    if (Application.isPlaying)
                    {
                        util_XHUDGUI.Open(XHudDialogType.警告, "HudElement消息", "记录RMS布局信息", "程序正在运行，无法在运行期间执行此功能！", "明白");
                        return;
                    }

                    RMS_Record();

                    return;
                }
                GUILayout.FlexibleSpace();
            }
            #endregion

            #region 元素重置到锚点初始位置
            GUI.enabled = true;
            if (util_XHUDGUI.Gui_Layout_Button(15, "元素重置到锚点初始位置", resetanchorpos_r, resetanchorpos_p))
            {
                if (Application.isPlaying)
                {
                    util_XHUDGUI.Open(XHudDialogType.警告, "HudElement消息", "锚点初始化", "程序正在运行，无法在运行期间执行此功能！", "明白");
                    return;
                }

                RestoreToAnchorPosition();

                return;
            }
            #endregion

            if (!IsMultiSelected() && AnimatorNodes.arraySize > 0 &&
                (GetPrefabStatus(BaseScript.gameObject) == ElementStatu.InScene ||
                GetPrefabStatus(BaseScript.gameObject) == ElementStatu.InSceneNotPrefab))
            {
                GUILayout.FlexibleSpace();

                #region 预览动画
                if (!IsMultiSelected())
                {
                    util_XHUDGUI.SetEnabled(true);
                }
                else
                {
                    util_XHUDGUI.SetEnabled(false);
                }
                if (!Preivew_Animator_PlayingState)
                {
                    if (util_XHUDGUI.Gui_Layout_Button(15, "播放所有动画器预览", prw_play_r, prw_play_p))
                    {
                        if (Application.isPlaying)
                        {
                            util_XHUDGUI.Open(XHudDialogType.警告, "HudElement消息", "预览动画", "程序正在运行，无法在运行期间执行此功能！", "明白");
                            return;
                        }
                        Preview_Animator_Play();
                    }
                }
                else
                {
                    if (util_XHUDGUI.Gui_Layout_Button(15, "停止所有动画器预览", prw_stop_r, prw_stop_p))
                    {
                        if (Application.isPlaying)
                        {
                            util_XHUDGUI.Open(XHudDialogType.警告, "HudElement消息", "停止预览动画", "程序正在运行，无法在运行期间执行此功能！", "明白");
                            return;
                        }
                        Preview_Animator_Stop();
                    }
                }
                util_XHUDGUI.SetEnabled(true);
                #endregion
            }
            util_XHUDGUI.Gui_Layout_Space(10);
            util_XHUDGUI.Gui_Layout_Horizontal_End();
            GUI.enabled = true;

            util_XHUDGUI.Gui_Layout_Space(10);
            util_XHUDGUI.Gui_Layout_Vertical_End();
            #endregion

            #region 参数
            util_XHUDGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "参数", util_Dashboard.Theme_Primary);
            util_XHUDGUI.Gui_Layout_Space(5);

            ///---标识名称     
            EditorGUI.BeginChangeCheck();
            util_XHUDGUI.Gui_Layout_Property_Field("标识名称", Indicator);
            if (EditorGUI.EndChangeCheck())
            {
                if (IsMultiSelected())
                {
                    for (int i = 0; i < SelectedObjects.Length; i++)
                    {
                        SerializedObject so_ele = new SerializedObject(SelectedObjects[i]);
                        SerializedProperty sp_indicator = so_ele.FindProperty("Indicator");
                        so_ele.Update();
                        sp_indicator.stringValue = Indicator.stringValue;
                        sp_indicator.serializedObject.ApplyModifiedProperties();
                        so_ele.ApplyModifiedProperties();
                    }
                }
            }

            util_XHUDGUI.Gui_Layout_Space(5);

            ///---透明度_Alpha     
            EditorGUI.BeginChangeCheck();
            util_XHUDGUI.Gui_Layout_Property_Field("透明度", Alpha);
            if (EditorGUI.EndChangeCheck())
            {
                ChangeAlpha();
            }

            util_XHUDGUI.Gui_Layout_Space(5);

            ///---动画倍乘系数     
            util_XHUDGUI.Gui_Layout_Property_Field("速率倍增", Element_Animators_GlobalDuration);

            util_XHUDGUI.Gui_Layout_Space(5);

            ///---锚点偏移     
            util_XHUDGUI.Gui_Layout_Property_Field("当前锚点", CurrentPivot);

            util_XHUDGUI.Gui_Layout_Space(10);
            util_XHUDGUI.Gui_Layout_Vertical_End();
            #endregion

            #region 选项
            util_XHUDGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "选项", util_Dashboard.Theme_Primary);
            util_XHUDGUI.Gui_Layout_Space(5);

            #region 状态调试
            util_XHUDGUI.Gui_Layout_Toggle<bool, Hud_Element>("状态调试", stroptions_debug, ref DebugState, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
            #endregion

            #region 自动停止预览
            util_XHUDGUI.Gui_Layout_Toggle<bool, Hud_Element>("自动停止预览", stroptions_enabled, ref AutoStopPreview, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
            #endregion

            #region 元素下动画器自动播放
            util_XHUDGUI.Gui_Layout_Toggle<bool, Hud_Element>("动画器自动播放", stroptions_auto, ref AutoPlayAnimators, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
            #endregion

            #region 元素下容器动画自动播放
            util_XHUDGUI.Gui_Layout_Toggle<bool, Hud_Element>("容器动画自动播放", stroptions_auto, ref AutoPlayContainersAnimators, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
            #endregion

            #region 生成时使用设计布局
            util_XHUDGUI.Gui_Layout_Toggle<bool, Hud_Element>("使用设计布局", stroptions_enabled, ref RMS_Enabled, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
            #endregion

            util_XHUDGUI.Gui_Layout_Space(10);
            util_XHUDGUI.Gui_Layout_Vertical_End();
            #endregion

            #region 状态
            string statu_title = "状态";
            if (IsMultiSelected())
                statu_title = "状态 - ( 批量模式 )";
            util_XHUDGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, statu_title, util_Dashboard.Theme_Primary);
            util_XHUDGUI.Gui_Layout_Space(5);

            if (!IsMultiSelected())
            {
                #region 使用状态     
                util_XHUDGUI.StatuDisplayer_text(usestate, 12, new Vector2(0, 7), "使用状态", 12, (HudElementCreateState)CreateState.enumValueIndex == HudElementCreateState.Created ? "已被生成" : "已被回收", CreateState.boolValue ? util_Dashboard.Theme_Primary : Color.gray, 11);
                #endregion

                #region 动画相关
                if (AnimatorNodes != null && AnimatorNodes.arraySize > 0)
                {
                    #region 动画状态     
                    util_XHUDGUI.StatuDisplayer_text(animstate, 12, new Vector2(0, 7), "动画状态", 12, (HudElementAnimateState)AnimateState.enumValueIndex == HudElementAnimateState.Animating ? "动画中" : "静止状态", AnimateState.enumValueIndex == 1 ? util_Dashboard.Theme_Primary : Color.gray, 11);
                    #endregion

                    #region 最大耗时     
                    util_XHUDGUI.StatuDisplayer_text(dutation, 12, new Vector2(0, 7), "最大耗时<color=#909090>（速率倍增）</color>", 12, AnimatorsMaxDuration.floatValue.ToString() + " 秒", util_Dashboard.Theme_Primary, 11);
                    #endregion

                    #region 最大耗时        
                    util_XHUDGUI.StatuDisplayer_text(dutation, 12, new Vector2(0, 7), "最大耗时<color=#909090>（XHUD倍增）</color>", 12, (mgr.DurationMultiply * AnimatorsMaxDuration.floatValue).ToString() + " 秒", util_Dashboard.Theme_Primary, 11);
                    #endregion
                }
                #endregion

                #region 物体跟踪
                if (ObjectTracker.objectReferenceValue != null)
                {
                    Hud_ObjectTracker tracker = (Hud_ObjectTracker)ObjectTracker.objectReferenceValue;
                    if (tracker.SelfObject != null && tracker.TargetObject != null)
                    {
                        util_XHUDGUI.StatuDisplayer_text(comp_tracking, 12, new Vector2(0, 7), "物体跟踪状态", 12, tracker.SelfObject.name + "  >  " + tracker.TargetObject.name, util_Dashboard.Theme_Primary, 11);
                    }
                }
                #endregion

                #region 元素库源
                if (Application.isPlaying)
                {
                    bool ElementInLib = false;

                    if (!string.IsNullOrEmpty(OriginPoolName.stringValue))
                    {
                        //遍历元素库集合找到目标元素库
                        foreach (Hud_ElementLibrary lib in mgr.Hud_ElementLibrarys)
                        {
                            if (lib.LibraryName == OriginPoolName.stringValue)
                            {
                                if (lib.ElementLibrary_IsExist(OriginalName.stringValue))
                                {
                                    ElementInLib = true;
                                    break;
                                }
                                break;
                            }
                        }
                    }

                    if (util_XHUDGUI.StatuDisplayer_btn(elelibsource, 12, new Vector2(0, 7), "元素库源", 12, ElementInLib ? OriginPoolName.stringValue : "非元素库资源", util_Dashboard.Theme_Primary, 11, util_Dashboard.Theme_Primary, 14, "定位到元素库", locate_r, locate_p, 4))
                    {
                        if (!ElementInLib)
                        {
                            util_XHUDGUI.Open(XHudDialogType.警告, "HudElement消息", "定位元素", "此元素并非由元素库生成，无法为其进行定位！", "明白");
                            return;
                        }
                        //打开目标元素库
                        util_OpenLibrarys.open_target_elements(OriginPoolName.stringValue).ElementLibrary_Location_Find(OriginalName.stringValue);
                    }
                }
                else
                {
                    //编辑器模式下，通过遍历所有元素库来找到当前这个元素在哪个库里
                }
                #endregion

                util_XHUDGUI.Gui_Layout_Space(10);
                EditorGUILayout.HelpBox(RMS_Enabled.boolValue ? "此元素在生成时会优先使用自身记录的设计布局信息来进行定位和位置尺寸的匹配" : "如果启用设计布局那么在用代码调用生成时则会主动使用该元素自身记录的设计布局信息来进行定位和位置尺寸等相关参数的匹配（但前提是在您制作好此元素时需要手动点击\"记录布局信息\"按钮）", MessageType.Info);
            }
            else
            {
                #region 批量控件
                util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                util_XHUDGUI.Gui_Layout_Space(10);
                if (util_XHUDGUI.Gui_Layout_Button($"{SelectedObjects[ElementStatu_Index].name} ( {SelectedObjects[ElementStatu_Index].Indicator} )", "", HudFilled.透明, HudColor.无, Color.gray, 20))
                {
                    EditorGUIUtility.PingObject(SelectedObjects[ElementStatu_Index]);
                }
                util_XHUDGUI.Gui_Layout_FlexSpace();
                if (util_XHUDGUI.Gui_Layout_Button(12, "", left_arrow_r, left_arrow_p))
                {
                    if (ElementStatu_Index <= 0)
                    {
                        ElementStatu_Index = SelectedObjects.Length - 1;
                    }
                    else
                    {
                        ElementStatu_Index--;
                    }
                    EditorGUIUtility.PingObject(SelectedObjects[ElementStatu_Index]);
                }
                util_XHUDGUI.Gui_Layout_Space(16);
                if (util_XHUDGUI.Gui_Layout_Button(12, "", right_arrow_r, right_arrow_p))
                {
                    if (ElementStatu_Index >= SelectedObjects.Length - 1)
                    {
                        ElementStatu_Index = 0;
                    }
                    else
                    {
                        ElementStatu_Index++;
                    }
                    EditorGUIUtility.PingObject(SelectedObjects[ElementStatu_Index]);
                }
                util_XHUDGUI.Gui_Layout_Space(5);
                util_XHUDGUI.Gui_Layout_Horizontal_End();
                #endregion

                #region 使用状态     
                util_XHUDGUI.StatuDisplayer_text(usestate, 12, new Vector2(0, 7), "使用状态", 12, SelectedObjects[ElementStatu_Index].CreateState == HudElementCreateState.Created ? "已被生成" : "已被回收", CreateState.boolValue ? util_Dashboard.Theme_Primary : Color.gray, 11);
                #endregion

                #region 动画相关
                if (SelectedObjects[ElementStatu_Index].AnimatorNodes != null && SelectedObjects[ElementStatu_Index].AnimatorNodes.Count > 0)
                {
                    #region 动画状态     
                    util_XHUDGUI.StatuDisplayer_text(animstate, 12, new Vector2(0, 7), "动画状态", 12, SelectedObjects[ElementStatu_Index].AnimateState == HudElementAnimateState.Animating ? "动画中" : "静止状态", AnimateState.enumValueIndex == 1 ? util_Dashboard.Theme_Primary : Color.gray, 11);
                    #endregion

                    SelectedObjects[ElementStatu_Index].AnimatorsMaxDuration = Animators_GetAnimatorsMaxDuration(SelectedObjects[ElementStatu_Index].AnimatorNodes, SelectedObjects[ElementStatu_Index].Element_Animators_GlobalDuration);

                    #region 最大耗时     
                    util_XHUDGUI.StatuDisplayer_text(dutation, 12, new Vector2(0, 7), "最大耗时<color=#909090>（元素倍增）</color>", 12, SelectedObjects[ElementStatu_Index].AnimatorsMaxDuration.ToString() + " 秒", util_Dashboard.Theme_Primary, 11);
                    #endregion

                    #region 最大耗时        
                    util_XHUDGUI.StatuDisplayer_text(dutation, 12, new Vector2(0, 7), "最大耗时<color=#909090>（XHUD倍增）</color>", 12, (mgr.DurationMultiply * SelectedObjects[ElementStatu_Index].AnimatorsMaxDuration).ToString() + " 秒", util_Dashboard.Theme_Primary, 11);
                    #endregion
                }
                #endregion

                #region 物体跟踪
                if (SelectedObjects[ElementStatu_Index].ObjectTracker != null)
                {
                    Hud_ObjectTracker tracker = SelectedObjects[ElementStatu_Index].ObjectTracker;
                    if (tracker.SelfObject != null && tracker.TargetObject != null)
                    {
                        util_XHUDGUI.StatuDisplayer_text(comp_tracking, 12, new Vector2(0, 7), "物体跟踪状态", 12, tracker.SelfObject.name + "  >  " + tracker.TargetObject.name, util_Dashboard.Theme_Primary, 11);
                    }
                }
                #endregion

                #region 元素库源
                bool ElementInLib = false;

                if (!string.IsNullOrEmpty(SelectedObjects[ElementStatu_Index].OriginPoolName))
                {
                    //遍历元素库集合找到目标元素库
                    foreach (Hud_ElementLibrary lib in mgr.Hud_ElementLibrarys)
                    {
                        if (lib.LibraryName == SelectedObjects[ElementStatu_Index].OriginPoolName)
                        {
                            if (lib.ElementLibrary_IsExist(SelectedObjects[ElementStatu_Index].OriginalName))
                            {
                                ElementInLib = true;
                                break;
                            }
                            break;
                        }
                    }
                }

                if (util_XHUDGUI.StatuDisplayer_btn(elelibsource, 12, new Vector2(0, 7), "元素库源", 12, ElementInLib ? SelectedObjects[ElementStatu_Index].OriginPoolName : "非元素库资源", util_Dashboard.Theme_Primary, 11, util_Dashboard.Theme_Primary, 14, "定位到元素库", locate_r, locate_p, 4))
                {
                    if (!ElementInLib)
                    {
                        util_XHUDGUI.Open(XHudDialogType.警告, "HudElement消息", "定位元素", "此元素并非由元素库生成，无法为其进行定位！", "明白");
                        return;
                    }
                    //打开目标元素库
                    util_OpenLibrarys.open_target_elements(SelectedObjects[ElementStatu_Index].OriginPoolName).ElementLibrary_Location_Find(SelectedObjects[ElementStatu_Index].OriginalName);
                }
                #endregion

                util_XHUDGUI.Gui_Layout_Space(10);
                EditorGUILayout.HelpBox(SelectedObjects[ElementStatu_Index].RMS_Enabled ? "此元素在生成时会优先使用自身记录的设计布局信息来进行定位和位置尺寸的匹配" : "如果启用设计布局那么在用代码调用生成时则会主动使用该元素自身记录的设计布局信息来进行定位和位置尺寸等相关参数的匹配（但前提是在您制作好此元素时需要手动点击\"记录布局信息\"按钮）", MessageType.Info);
            }

            util_XHUDGUI.Gui_Layout_Space(5);
            util_XHUDGUI.Gui_Layout_Vertical_End();
            #endregion

            #region RMS布局信息
            if (RMS_Enabled.boolValue)
            {

                util_XHUDGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "RMS布局信息", util_Dashboard.Theme_Primary);
                util_XHUDGUI.Gui_Layout_Space(5);

                #region 分辨率方案列表
                if (mgr != null)
                {
                    //ScreenResolutionNode[] nodes = mgr.hm_RMS_GetResolutionNodes();
                    string[] nodesName = mgr.hm_RMS_GetResolutionNodeNames();
                    if (nodesName.Length > 0)
                    {
                        util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                        util_XHUDGUI.Gui_Layout_Space(10);
                        EditorGUILayout.HelpBox("RMS方案列表仅用于切换和预览，在取消选择元素后会自动回到Hud管理器当前选中的RMS方案", MessageType.Info);
                        util_XHUDGUI.Gui_Layout_Space(5);
                        util_XHUDGUI.Gui_Layout_Horizontal_End();

                        EditorGUI.BeginChangeCheck();
                        util_XHUDGUI.Gui_Layout_Popup<string, Hud_Element>("R M S 方案", nodesName, ref RMS_Name, HudFilled.实体, 120, 22, SelectedObjects);

                        #region RMS方案信息查看

                        if (IsMultiSelected())
                        {
                            util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                            util_XHUDGUI.Gui_Layout_Space(10);
                            EditorGUILayout.HelpBox("RMS方案信息不支持多项查看", MessageType.Warning);
                            util_XHUDGUI.Gui_Layout_Space(5);
                            util_XHUDGUI.Gui_Layout_Horizontal_End();
                        }
                        else
                        {
                            if (RMS_InfoList.arraySize <= 0)
                            {
                                util_XHUDGUI.Gui_Layout_Labelfield("暂无布局设计数据", HudFilled.无, HudColor.无, util_XHUDGUI.GetColor(HudColor.阴影灰), TextAnchor.MiddleCenter);
                            }
                            else
                            {
                                for (int i = 0; i < RMS_InfoList.arraySize; i++)
                                {
                                    SerializedProperty sp_item = RMS_InfoList.GetArrayElementAtIndex(i);
                                    SerializedProperty sp_item_Name = sp_item.FindPropertyRelative("LayoutName");
                                    SerializedProperty sp_item_Anchor = sp_item.FindPropertyRelative("Anchor");
                                    SerializedProperty sp_item_Position = sp_item.FindPropertyRelative("Position");
                                    SerializedProperty sp_item_Euler = sp_item.FindPropertyRelative("Euler");
                                    SerializedProperty sp_item_Scale = sp_item.FindPropertyRelative("Scale");
                                    SerializedProperty sp_item_AnchorMin = sp_item.FindPropertyRelative("AnchorMin");
                                    SerializedProperty sp_item_AnchorMax = sp_item.FindPropertyRelative("AnchorMax");
                                    SerializedProperty sp_item_Pivot = sp_item.FindPropertyRelative("Pivot");

                                    if (sp_item_Name.stringValue == RMS_Name.stringValue)
                                    {
                                        int titlesize = 12;
                                        int contensize = 10;
                                        float iconsize = 12;
                                        Color valuecol = util_XHUDGUI.GetColor(HudColor.阴影灰);

                                        util_XHUDGUI.StatuDisplayer_text(rms_anchor, iconsize, new Vector2(0, 7), "锚点", titlesize, ((HudAnchor)sp_item_Anchor.enumValueIndex).ToString(), valuecol, contensize);
                                        util_XHUDGUI.StatuDisplayer_text(rms_anchor, iconsize, new Vector2(0, 7), "最小锚点", titlesize, sp_item_AnchorMin.vector2Value.ToString(), valuecol, contensize);
                                        util_XHUDGUI.StatuDisplayer_text(rms_anchor, iconsize, new Vector2(0, 7), "最大锚点", titlesize, sp_item_AnchorMax.vector2Value.ToString(), valuecol, contensize);
                                        util_XHUDGUI.StatuDisplayer_text(rms_move, iconsize, new Vector2(0, 7), "位置", titlesize, sp_item_Position.vector3Value.ToString(), valuecol, contensize);
                                        util_XHUDGUI.StatuDisplayer_text(rms_rotate, iconsize, new Vector2(0, 7), "角度", titlesize, sp_item_Euler.vector3Value.ToString(), valuecol, contensize);
                                        util_XHUDGUI.StatuDisplayer_text(rms_scale, iconsize, new Vector2(0, 7), "缩放", titlesize, sp_item_Scale.vector3Value.ToString(), valuecol, contensize);
                                        util_XHUDGUI.StatuDisplayer_text(rms_anchor_center, iconsize, new Vector2(0, 7), "轴心", titlesize, sp_item_Pivot.vector2Value.ToString(), valuecol, contensize);
                                    }
                                }

                                util_XHUDGUI.Gui_Layout_Space(5);

                                util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);

                                if (RMS_InfoList.arraySize > 0)
                                {
                                    if (!Application.isPlaying)
                                        if (util_XHUDGUI.Gui_Layout_Button("清空所有方案", "清空所有分辨率匹配方案列表", HudFilled.实体, HudColor.深空灰, util_XHUDGUI.GetColor(HudColor.魅力红), 25, new RectOffset(), new Vector2(0, 0)))
                                        {
                                            string res = util_XHUDGUI.Open(XHudDialogType.警告, "HudElement消息", "RMS方案操作", "如果清空匹配分辨率信息列表，会导致您之前为不同分辨率记录的坐标信息全部清空，请谨慎此操作！", "清空", "暂不", 1);
                                            if (res == "清空")
                                            {
                                                RMS_InfoList.ClearArray();
                                                RMS_InfoList.serializedObject.ApplyModifiedProperties();
                                            }
                                            return;
                                        }
                                }

                                if (RMS_IsExist())
                                {
                                    if (!Application.isPlaying)
                                        util_XHUDGUI.Gui_Layout_Space(5);
                                    if (util_XHUDGUI.Gui_Layout_Button("移除当前方案", "清空当前选择的分辨率匹配方案", HudFilled.实体, HudColor.深空灰, Color.white, 25, new RectOffset(), new Vector2(0, 0)))
                                    {
                                        string res = util_XHUDGUI.Open(XHudDialogType.警告, "HudElement消息", "RMS方案操作", "此操作会导致您为当前分辨率匹配的方案会被移除，请谨慎此操作！", "清空", "暂不", 1);
                                        if (res == "暂不")
                                            return;
                                        for (int i = 0; i < RMS_InfoList.arraySize; i++)
                                        {
                                            SerializedProperty sp_Name = RMS_InfoList.GetArrayElementAtIndex(i).FindPropertyRelative("LayoutName");
                                            if (sp_Name.stringValue == RMS_Name.stringValue)
                                            {
                                                RMS_InfoList.DeleteArrayElementAtIndex(i);
                                                RMS_InfoList.serializedObject.ApplyModifiedProperties();
                                                break;
                                            }
                                        }
                                        return;
                                    }
                                }

                                util_XHUDGUI.Gui_Layout_Horizontal_End();
                            }
                        }
                        #endregion

                        if (EditorGUI.EndChangeCheck())
                        {
                            if (!Application.isPlaying)
                            {
                                if (IsMultiSelected())
                                {
                                    for (int i = 0; i < SelectedObjects.Length; i++)
                                    {
                                        if (!string.IsNullOrEmpty(SelectedObjects[i].gameObject.scene.name))
                                            RMS_Preview(SelectedObjects[i], SelectedObjects[i].Alpha, Vector3.zero, SelectedObjects[i].RMS_Name, true);
                                    }
                                }
                                if (!string.IsNullOrEmpty(BaseScript.gameObject.scene.name))
                                    RMS_Preview(BaseScript, Alpha.floatValue, Vector3.zero, RMS_Name.stringValue, true);
                            }
                        }
                    }
                    else
                    {
                        util_XHUDGUI.Gui_Layout_Labelfield("暂未在管理器中配置 R M S 列表", HudFilled.无, HudColor.无, util_XHUDGUI.GetColor(HudColor.阴影灰), TextAnchor.MiddleCenter);
                    }
                }
                else
                {
                    util_XHUDGUI.Gui_Layout_Labelfield("暂未发现Hud管理器", HudFilled.无, HudColor.无, util_XHUDGUI.GetColor(HudColor.阴影灰), TextAnchor.MiddleCenter);
                }
                #endregion

                util_XHUDGUI.Gui_Layout_Space(5);
                util_XHUDGUI.Gui_Layout_Vertical_End();
            }
            #endregion

            #region 列表
            util_XHUDGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "事件/列表", util_Dashboard.Theme_Primary);
            util_XHUDGUI.Gui_Layout_Space(5);

            #region 音效列表
            if (SounderNodes.arraySize > 0)
            {
                if (IsMultiSelected())
                {
                    util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    util_XHUDGUI.Gui_Layout_Space(10);
                    EditorGUILayout.HelpBox("音效器列表不支持多项操作", MessageType.Warning);
                    util_XHUDGUI.Gui_Layout_Space(5);
                    util_XHUDGUI.Gui_Layout_Horizontal_End();
                }
                else
                {
                    util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    util_XHUDGUI.Gui_Layout_Space(10);
                    SounderIsFold.boolValue = EditorGUILayout.Foldout(SounderIsFold.boolValue, "音效器", true);
                    util_XHUDGUI.Gui_Layout_Space(5);
                    util_XHUDGUI.Gui_Layout_Horizontal_End();

                    util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    util_XHUDGUI.Gui_Layout_Space(5);
                    if (SounderIsFold.boolValue)
                        SounderList.DoLayoutList();
                    util_XHUDGUI.Gui_Layout_Space(5);
                    util_XHUDGUI.Gui_Layout_Horizontal_End();
                }
            }
            #endregion

            #region 动画器列表
            if (AnimatorNodes.arraySize > 0)
            {
                if (IsMultiSelected())
                {
                    util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    util_XHUDGUI.Gui_Layout_Space(10);
                    EditorGUILayout.HelpBox("动画器列表不支持多项操作", MessageType.Warning);
                    //util_EditorGuiLib.Gui_Layout_Labelfield("动画器列表不支持多项操作", HudFilled.无_None, HudColor.无_None, util_EditorGuiLib.GetColor(HudColor.阴影灰), TextAnchor.MiddleLeft);
                    util_XHUDGUI.Gui_Layout_Space(5);
                    util_XHUDGUI.Gui_Layout_Horizontal_End();
                }
                else
                {
                    util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    util_XHUDGUI.Gui_Layout_Space(10);
                    AnimatorsIsFold.boolValue = EditorGUILayout.Foldout(AnimatorsIsFold.boolValue, "动画器", true);
                    AnimatorsIsFold.serializedObject.ApplyModifiedProperties();
                    util_XHUDGUI.Gui_Layout_Space(5);
                    util_XHUDGUI.Gui_Layout_Horizontal_End();

                    util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    util_XHUDGUI.Gui_Layout_Space(5);

                    if (AnimatorsIsFold.boolValue)
                        AnimatorsList.DoLayoutList();

                    util_XHUDGUI.Gui_Layout_Space(5);
                    util_XHUDGUI.Gui_Layout_Horizontal_End();
                }
            }
            #endregion

            #region 按钮列表
            if (ButtonNodes.arraySize > 0)
            {
                if (IsMultiSelected())
                {
                    util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    util_XHUDGUI.Gui_Layout_Space(10);
                    EditorGUILayout.HelpBox("按钮列表不支持多项操作", MessageType.Warning);
                    util_XHUDGUI.Gui_Layout_Space(5);
                    util_XHUDGUI.Gui_Layout_Horizontal_End();
                }
                else
                {
                    util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    util_XHUDGUI.Gui_Layout_Space(10);
                    ButtonIsFold.boolValue = EditorGUILayout.Foldout(ButtonIsFold.boolValue, "按钮", true);
                    util_XHUDGUI.Gui_Layout_Space(5);
                    util_XHUDGUI.Gui_Layout_Horizontal_End();

                    util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    util_XHUDGUI.Gui_Layout_Space(5);
                    if (ButtonIsFold.boolValue)
                        ButtonList.DoLayoutList();
                    util_XHUDGUI.Gui_Layout_Space(5);
                    util_XHUDGUI.Gui_Layout_Horizontal_End();
                }
            }
            #endregion

            #region 文字列表
            if (TextNodes.arraySize > 0)
            {
                if (IsMultiSelected())
                {
                    util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    util_XHUDGUI.Gui_Layout_Space(10);
                    EditorGUILayout.HelpBox("文字列表不支持多项操作", MessageType.Warning);
                    util_XHUDGUI.Gui_Layout_Space(5);
                    util_XHUDGUI.Gui_Layout_Horizontal_End();
                }
                else
                {
                    util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    util_XHUDGUI.Gui_Layout_Space(10);
                    TextIsFold.boolValue = EditorGUILayout.Foldout(TextIsFold.boolValue, "文字", true);
                    util_XHUDGUI.Gui_Layout_Space(5);
                    util_XHUDGUI.Gui_Layout_Horizontal_End();

                    util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    util_XHUDGUI.Gui_Layout_Space(5);
                    if (TextIsFold.boolValue)
                        TextList.DoLayoutList();
                    util_XHUDGUI.Gui_Layout_Space(5);
                    util_XHUDGUI.Gui_Layout_Horizontal_End();
                }
            }
            #endregion

            #region Tmp文字列表
            if (TmpTextNodes.arraySize > 0)
            {
                if (IsMultiSelected())
                {
                    util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    util_XHUDGUI.Gui_Layout_Space(10);
                    EditorGUILayout.HelpBox("Tmp文字列表不支持多项操作", MessageType.Warning);
                    util_XHUDGUI.Gui_Layout_Space(5);
                    util_XHUDGUI.Gui_Layout_Horizontal_End();
                }
                else
                {
                    util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    util_XHUDGUI.Gui_Layout_Space(10);
                    TmpTextIsFold.boolValue = EditorGUILayout.Foldout(TmpTextIsFold.boolValue, "Tmp文字", true);
                    util_XHUDGUI.Gui_Layout_Space(5);
                    util_XHUDGUI.Gui_Layout_Horizontal_End();

                    util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    util_XHUDGUI.Gui_Layout_Space(5);
                    if (TmpTextIsFold.boolValue)
                        TmpTextList.DoLayoutList();
                    util_XHUDGUI.Gui_Layout_Space(5);
                    util_XHUDGUI.Gui_Layout_Horizontal_End();
                }
            }
            #endregion

            #region 选项列表
            if (OptionNodes.arraySize > 0)
            {
                if (IsMultiSelected())
                {
                    util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    util_XHUDGUI.Gui_Layout_Space(10);
                    EditorGUILayout.HelpBox("选项列表不支持多项操作", MessageType.Warning);
                    util_XHUDGUI.Gui_Layout_Space(5);
                    util_XHUDGUI.Gui_Layout_Horizontal_End();
                }
                else
                {
                    util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    util_XHUDGUI.Gui_Layout_Space(10);
                    OptionIsFold.boolValue = EditorGUILayout.Foldout(OptionIsFold.boolValue, "选项", true);
                    util_XHUDGUI.Gui_Layout_Space(5);
                    util_XHUDGUI.Gui_Layout_Horizontal_End();

                    util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    util_XHUDGUI.Gui_Layout_Space(5);
                    if (OptionIsFold.boolValue)
                        OptionList.DoLayoutList();
                    util_XHUDGUI.Gui_Layout_Space(5);
                    util_XHUDGUI.Gui_Layout_Horizontal_End();
                }
            }
            #endregion

            #region 滑动条列表
            if (SliderNodes.arraySize > 0)
            {
                if (IsMultiSelected())
                {
                    util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    util_XHUDGUI.Gui_Layout_Space(10);
                    EditorGUILayout.HelpBox("滑动条列表不支持多项操作", MessageType.Warning);
                    util_XHUDGUI.Gui_Layout_Space(5);
                    util_XHUDGUI.Gui_Layout_Horizontal_End();
                }
                else
                {
                    util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    util_XHUDGUI.Gui_Layout_Space(10);
                    SliderIsFold.boolValue = EditorGUILayout.Foldout(SliderIsFold.boolValue, "滑动条", true);
                    util_XHUDGUI.Gui_Layout_Space(5);
                    util_XHUDGUI.Gui_Layout_Horizontal_End();

                    util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    util_XHUDGUI.Gui_Layout_Space(5);
                    if (SliderIsFold.boolValue)
                        SliderList.DoLayoutList();
                    util_XHUDGUI.Gui_Layout_Space(5);
                    util_XHUDGUI.Gui_Layout_Horizontal_End();
                }
            }
            #endregion

            #region 进度条列表
            if (ProgressNodes.arraySize > 0)
            {
                if (IsMultiSelected())
                {
                    util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    util_XHUDGUI.Gui_Layout_Space(10);
                    EditorGUILayout.HelpBox("进度条列表不支持多项操作", MessageType.Warning);
                    util_XHUDGUI.Gui_Layout_Space(5);
                    util_XHUDGUI.Gui_Layout_Horizontal_End();
                }
                else
                {
                    util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    util_XHUDGUI.Gui_Layout_Space(10);
                    ProgressIsFold.boolValue = EditorGUILayout.Foldout(ProgressIsFold.boolValue, "进度条", true);
                    util_XHUDGUI.Gui_Layout_Space(5);
                    util_XHUDGUI.Gui_Layout_Horizontal_End();

                    util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    util_XHUDGUI.Gui_Layout_Space(5);
                    if (ProgressIsFold.boolValue)
                        ProgressList.DoLayoutList();
                    util_XHUDGUI.Gui_Layout_Space(5);
                    util_XHUDGUI.Gui_Layout_Horizontal_End();
                }
            }
            #endregion

            #region 容器列表
            if (ContainerNodes.arraySize > 0)
            {
                if (IsMultiSelected())
                {
                    util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    util_XHUDGUI.Gui_Layout_Space(10);
                    EditorGUILayout.HelpBox("容器列表不支持多项操作", MessageType.Warning);
                    util_XHUDGUI.Gui_Layout_Space(5);
                    util_XHUDGUI.Gui_Layout_Horizontal_End();
                }
                else
                {
                    util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    util_XHUDGUI.Gui_Layout_Space(10);
                    ContainerIsFold.boolValue = EditorGUILayout.Foldout(ContainerIsFold.boolValue, "容器", true);
                    util_XHUDGUI.Gui_Layout_Space(5);
                    util_XHUDGUI.Gui_Layout_Horizontal_End();

                    util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    util_XHUDGUI.Gui_Layout_Space(5);
                    if (ContainerIsFold.boolValue)
                        ContainerList.DoLayoutList();
                    util_XHUDGUI.Gui_Layout_Space(5);
                    util_XHUDGUI.Gui_Layout_Horizontal_End();
                }
            }
            #endregion

            #region 开关列表
            if (ToggleNodes.arraySize > 0)
            {
                if (IsMultiSelected())
                {
                    util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    util_XHUDGUI.Gui_Layout_Space(10);
                    EditorGUILayout.HelpBox("开关列表不支持多项操作", MessageType.Warning);
                    util_XHUDGUI.Gui_Layout_Space(5);
                    util_XHUDGUI.Gui_Layout_Horizontal_End();
                }
                else
                {
                    util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    util_XHUDGUI.Gui_Layout_Space(10);
                    ToggleIsFold.boolValue = EditorGUILayout.Foldout(ToggleIsFold.boolValue, "开关", true);
                    util_XHUDGUI.Gui_Layout_Space(5);
                    util_XHUDGUI.Gui_Layout_Horizontal_End();

                    util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    util_XHUDGUI.Gui_Layout_Space(5);
                    if (ToggleIsFold.boolValue)
                        ToggleList.DoLayoutList();
                    util_XHUDGUI.Gui_Layout_Space(5);
                    util_XHUDGUI.Gui_Layout_Horizontal_End();
                }
            }
            #endregion

            #region 事件列表
            if (IsMultiSelected())
            {
                util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                util_XHUDGUI.Gui_Layout_Space(10);
                EditorGUILayout.HelpBox("事件不支持多项操作", MessageType.Warning);
                util_XHUDGUI.Gui_Layout_Space(5);
                util_XHUDGUI.Gui_Layout_Horizontal_End();
            }
            else
            {
                util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                util_XHUDGUI.Gui_Layout_Space(10);
                EventIsFold.boolValue = EditorGUILayout.Foldout(EventIsFold.boolValue, "事件", true);
                util_XHUDGUI.Gui_Layout_Space(5);
                util_XHUDGUI.Gui_Layout_Horizontal_End();

                if (EventIsFold.boolValue)
                {
                    util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    util_XHUDGUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_element_in_start);
                    eve_on_element_in_start.serializedObject.ApplyModifiedProperties();
                    util_XHUDGUI.Gui_Layout_Space(5);
                    util_XHUDGUI.Gui_Layout_Horizontal_End();

                    util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    util_XHUDGUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_element_in_end);
                    eve_on_element_in_end.serializedObject.ApplyModifiedProperties();
                    util_XHUDGUI.Gui_Layout_Space(5);
                    util_XHUDGUI.Gui_Layout_Horizontal_End();

                    util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    util_XHUDGUI.Gui_Layout_Space(5);

                    EditorGUILayout.PropertyField(eve_on_element_out_start);
                    eve_on_element_out_start.serializedObject.ApplyModifiedProperties();
                    util_XHUDGUI.Gui_Layout_Space(5);
                    util_XHUDGUI.Gui_Layout_Horizontal_End();

                    util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    util_XHUDGUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_element_out_end);
                    eve_on_element_out_end.serializedObject.ApplyModifiedProperties();
                    util_XHUDGUI.Gui_Layout_Space(5);
                    util_XHUDGUI.Gui_Layout_Horizontal_End();
                }
            }
            #endregion

            util_XHUDGUI.Gui_Layout_Space(5);
            util_XHUDGUI.Gui_Layout_Vertical_End();
            #endregion

            #region 统计
            string statu_statistic = "统计";
            if (IsMultiSelected())
                statu_statistic = "统计 - ( 批量模式 )";
            util_XHUDGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, statu_statistic, util_Dashboard.Theme_Primary);
            util_XHUDGUI.Gui_Layout_Space(5);

            if (!IsMultiSelected())
            {
                if (SounderNodes.arraySize <= 0 && AnimatorNodes.arraySize <= 0 && ButtonNodes.arraySize <= 0 && OptionNodes.arraySize <= 0 &&
                    SliderNodes.arraySize <= 0 && ProgressNodes.arraySize <= 0 && ToggleNodes.arraySize <= 0 && ContainerNodes.arraySize <= 0 &&
                    TextNodes.arraySize <= 0 && TmpTextNodes.arraySize <= 0)
                {
                    util_XHUDGUI.Gui_Layout_Labelfield("暂无统计数据", HudFilled.无, HudColor.无, util_XHUDGUI.GetColor(HudColor.阴影灰), TextAnchor.MiddleCenter);
                }
                else
                {
                    #region 组件数量 - 音效器
                    if (SounderNodes.arraySize > 0)
                    {
                        util_XHUDGUI.StatuDisplayer_text(icon_sound, 12, new Vector2(0, 7), "音效器", 12, SounderNodes.arraySize.ToString() + " 个", util_Dashboard.Theme_Primary, 11);
                    }
                    #endregion

                    #region 组件数量 - 动画器
                    if (AnimatorNodes.arraySize > 0)
                    {
                        util_XHUDGUI.StatuDisplayer_text(icon_anim, 12, new Vector2(0, 7), "动画器", 12, AnimatorNodes.arraySize.ToString() + " 个", util_Dashboard.Theme_Primary, 11);
                    }
                    #endregion

                    #region 组件数量 - 按钮
                    if (ButtonNodes.arraySize > 0)
                    {
                        util_XHUDGUI.StatuDisplayer_text(icon_button, 12, new Vector2(0, 7), "按钮", 12, ButtonNodes.arraySize.ToString() + " 个", util_Dashboard.Theme_Primary, 11);
                    }
                    #endregion

                    #region 组件数量 - 选项
                    if (OptionNodes.arraySize > 0)
                    {
                        util_XHUDGUI.StatuDisplayer_text(icon_option, 12, new Vector2(0, 7), "选项", 12, OptionNodes.arraySize.ToString() + " 个", util_Dashboard.Theme_Primary, 11);
                    }
                    #endregion

                    #region 组件数量 - 滑动条
                    if (SliderNodes.arraySize > 0)
                    {
                        util_XHUDGUI.StatuDisplayer_text(icon_slider, 12, new Vector2(0, 7), "滑动条", 12, SliderNodes.arraySize.ToString() + " 个", util_Dashboard.Theme_Primary, 11);
                    }
                    #endregion

                    #region 组件数量 - 进度条
                    if (ProgressNodes.arraySize > 0)
                    {
                        util_XHUDGUI.StatuDisplayer_text(icon_progress, 12, new Vector2(0, 7), "进度条", 12, ProgressNodes.arraySize.ToString() + " 个", util_Dashboard.Theme_Primary, 11);
                    }
                    #endregion

                    #region 组件数量 - 容器
                    if (ContainerNodes.arraySize > 0)
                    {
                        util_XHUDGUI.StatuDisplayer_text(icon_container, 12, new Vector2(0, 7), "容器", 12, ContainerNodes.arraySize.ToString() + " 个", util_Dashboard.Theme_Primary, 11);
                    }
                    #endregion

                    #region 组件数量 - 开关
                    if (ToggleNodes.arraySize > 0)
                    {
                        util_XHUDGUI.StatuDisplayer_text(icon_toggle, 12, new Vector2(0, 7), "开关", 12, ToggleNodes.arraySize.ToString() + " 个", util_Dashboard.Theme_Primary, 11);
                    }
                    #endregion

                    #region 组件数量 - 文字
                    if (TextNodes.arraySize > 0)
                    {
                        util_XHUDGUI.StatuDisplayer_text(icon_text, 12, new Vector2(0, 7), "文字", 12, TextNodes.arraySize.ToString() + " 个", util_Dashboard.Theme_Primary, 11);
                    }
                    #endregion

                    #region 组件数量 - Tmp文字
                    if (TmpTextNodes.arraySize > 0)
                    {
                        util_XHUDGUI.StatuDisplayer_text(icon_tmptext, 12, new Vector2(0, 7), "Tmp文字", 12, TmpTextNodes.arraySize.ToString() + " 个", util_Dashboard.Theme_Primary, 11);
                    }
                    #endregion
                }
            }
            else
            {
                #region 批量控件
                util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                util_XHUDGUI.Gui_Layout_Space(10);
                if (util_XHUDGUI.Gui_Layout_Button($"{SelectedObjects[ElementStatistic_Index].name} ( {SelectedObjects[ElementStatistic_Index].Indicator} )", "", HudFilled.透明, HudColor.无, Color.gray, 20))
                {
                    EditorGUIUtility.PingObject(SelectedObjects[ElementStatistic_Index]);
                }
                util_XHUDGUI.Gui_Layout_FlexSpace();
                if (util_XHUDGUI.Gui_Layout_Button(12, "", left_arrow_r, left_arrow_p))
                {
                    if (ElementStatistic_Index <= 0)
                    {
                        ElementStatistic_Index = SelectedObjects.Length - 1;
                    }
                    else
                    {
                        ElementStatistic_Index--;
                    }
                    EditorGUIUtility.PingObject(SelectedObjects[ElementStatistic_Index]);
                }
                util_XHUDGUI.Gui_Layout_Space(16);
                if (util_XHUDGUI.Gui_Layout_Button(12, "", right_arrow_r, right_arrow_p))
                {
                    if (ElementStatistic_Index >= SelectedObjects.Length - 1)
                    {
                        ElementStatistic_Index = 0;
                    }
                    else
                    {
                        ElementStatistic_Index++;
                    }
                    EditorGUIUtility.PingObject(SelectedObjects[ElementStatistic_Index]);
                }
                util_XHUDGUI.Gui_Layout_Space(5);
                util_XHUDGUI.Gui_Layout_Horizontal_End();
                #endregion

                if (SelectedObjects[ElementStatistic_Index].SounderNodes.Count <= 0 && SelectedObjects[ElementStatistic_Index].AnimatorNodes.Count <= 0 && SelectedObjects[ElementStatistic_Index].ButtonNodes.Count <= 0 && OptionNodes.arraySize <= 0 &&
                   SelectedObjects[ElementStatistic_Index].SliderNodes.Count <= 0 && SelectedObjects[ElementStatistic_Index].ProgressNodes.Count <= 0 && SelectedObjects[ElementStatistic_Index].ToggleNodes.Count <= 0 && ContainerNodes.arraySize <= 0 &&
                   SelectedObjects[ElementStatistic_Index].TextNodes.Count <= 0 && SelectedObjects[ElementStatistic_Index].TmpTextNodes.Count <= 0)
                {
                    util_XHUDGUI.Gui_Layout_Labelfield("暂无统计数据", HudFilled.无, HudColor.无, util_XHUDGUI.GetColor(HudColor.阴影灰), TextAnchor.MiddleCenter);
                }
                else
                {
                    #region 组件数量 - 音效器
                    if (SelectedObjects[ElementStatistic_Index].SounderNodes.Count > 0)
                    {
                        util_XHUDGUI.StatuDisplayer_text(icon_sound, 12, new Vector2(0, 7), "音效器", 12, SelectedObjects[ElementStatistic_Index].SounderNodes.Count.ToString() + " 个", util_Dashboard.Theme_Primary, 11);
                    }
                    #endregion

                    #region 组件数量 - 动画器
                    if (SelectedObjects[ElementStatistic_Index].AnimatorNodes.Count > 0)
                    {
                        util_XHUDGUI.StatuDisplayer_text(icon_anim, 12, new Vector2(0, 7), "动画器", 12, SelectedObjects[ElementStatistic_Index].AnimatorNodes.Count.ToString() + " 个", util_Dashboard.Theme_Primary, 11);
                    }
                    #endregion

                    #region 组件数量 - 按钮
                    if (SelectedObjects[ElementStatistic_Index].ButtonNodes.Count > 0)
                    {
                        util_XHUDGUI.StatuDisplayer_text(icon_button, 12, new Vector2(0, 7), "按钮", 12, SelectedObjects[ElementStatistic_Index].ButtonNodes.Count.ToString() + " 个", util_Dashboard.Theme_Primary, 11);
                    }
                    #endregion

                    #region 组件数量 - 选项
                    if (SelectedObjects[ElementStatistic_Index].OptionNodes.Count > 0)
                    {
                        util_XHUDGUI.StatuDisplayer_text(icon_option, 12, new Vector2(0, 7), "选项", 12, SelectedObjects[ElementStatistic_Index].OptionNodes.Count.ToString() + " 个", util_Dashboard.Theme_Primary, 11);
                    }
                    #endregion

                    #region 组件数量 - 滑动条
                    if (SelectedObjects[ElementStatistic_Index].SliderNodes.Count > 0)
                    {
                        util_XHUDGUI.StatuDisplayer_text(icon_slider, 12, new Vector2(0, 7), "滑动条", 12, SelectedObjects[ElementStatistic_Index].SliderNodes.Count.ToString() + " 个", util_Dashboard.Theme_Primary, 11);
                    }
                    #endregion

                    #region 组件数量 - 进度条
                    if (SelectedObjects[ElementStatistic_Index].ProgressNodes.Count > 0)
                    {
                        util_XHUDGUI.StatuDisplayer_text(icon_progress, 12, new Vector2(0, 7), "进度条", 12, SelectedObjects[ElementStatistic_Index].ProgressNodes.Count.ToString() + " 个", util_Dashboard.Theme_Primary, 11);
                    }
                    #endregion

                    #region 组件数量 - 容器
                    if (SelectedObjects[ElementStatistic_Index].ContainerNodes.Count > 0)
                    {
                        util_XHUDGUI.StatuDisplayer_text(icon_container, 12, new Vector2(0, 7), "容器", 12, SelectedObjects[ElementStatistic_Index].ContainerNodes.Count.ToString() + " 个", util_Dashboard.Theme_Primary, 11);
                    }
                    #endregion

                    #region 组件数量 - 开关
                    if (SelectedObjects[ElementStatistic_Index].ToggleNodes.Count > 0)
                    {
                        util_XHUDGUI.StatuDisplayer_text(icon_toggle, 12, new Vector2(0, 7), "开关", 12, SelectedObjects[ElementStatistic_Index].ToggleNodes.Count.ToString() + " 个", util_Dashboard.Theme_Primary, 11);
                    }
                    #endregion

                    #region 组件数量 - 文字
                    if (SelectedObjects[ElementStatistic_Index].TextNodes.Count > 0)
                    {
                        util_XHUDGUI.StatuDisplayer_text(icon_text, 12, new Vector2(0, 7), "文字", 12, SelectedObjects[ElementStatistic_Index].TextNodes.Count.ToString() + " 个", util_Dashboard.Theme_Primary, 11);
                    }
                    #endregion

                    #region 组件数量 - Tmp文字
                    if (SelectedObjects[ElementStatistic_Index].TmpTextNodes.Count > 0)
                    {
                        util_XHUDGUI.StatuDisplayer_text(icon_tmptext, 12, new Vector2(0, 7), "Tmp文字", 12, SelectedObjects[ElementStatistic_Index].TmpTextNodes.Count.ToString() + " 个", util_Dashboard.Theme_Primary, 11);
                    }
                    #endregion
                }
            }

            util_XHUDGUI.Gui_Layout_Space(5);
            util_XHUDGUI.Gui_Layout_Vertical_End();
            #endregion

            #region 组件

            util_XHUDGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "组件", util_Dashboard.Theme_Primary);
            util_XHUDGUI.Gui_Layout_Space(10);
            if (IsMultiSelected())
            {
                util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                util_XHUDGUI.Gui_Layout_Space(10);
                EditorGUILayout.HelpBox("组件不支持多项操作", MessageType.Warning);
                util_XHUDGUI.Gui_Layout_Space(5);
                util_XHUDGUI.Gui_Layout_Horizontal_End();
            }
            else
            {
                ///---CanvasGroup组件                
                util_XHUDGUI.StatuDisplayer_Object(comp_alpha, 12, new Vector2(0, 1), "透明度组件", 12, new Vector2(0, -7), status, new Vector2(0, 3), CanvasGroup.objectReferenceValue == null ? false : true, util_Dashboard.Theme_Primary, Color.black * 0.7f, CanvasGroup);

                ///---RectTransform
                util_XHUDGUI.StatuDisplayer_Object(comp_transform, 12, new Vector2(0, 1), "变换组件", 12, new Vector2(0, -7), status, new Vector2(0, 3), RectTransform.objectReferenceValue == null ? false : true, util_Dashboard.Theme_Primary, Color.black * 0.7f, RectTransform);

                ///---TriggerAction
                util_XHUDGUI.StatuDisplayer_Object(comp_trigger, 12, new Vector2(0, 1), "触发器", 12, new Vector2(0, -7), status, new Vector2(0, 3), TriggerAction.objectReferenceValue == null ? false : true, util_Dashboard.Theme_Primary, Color.black * 0.7f, TriggerAction);

                ///---ObjectTracker
                util_XHUDGUI.StatuDisplayer_Object(comp_tracking, 12, new Vector2(0, 1), "追踪器", 12, new Vector2(0, -7), status, new Vector2(0, 3), ObjectTracker.objectReferenceValue == null ? false : true, util_Dashboard.Theme_Primary, Color.black * 0.7f, ObjectTracker);

            }
            util_XHUDGUI.Gui_Layout_Space(10);
            util_XHUDGUI.Gui_Layout_Vertical_End();
            #endregion

            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                // 创建右键菜单
                GenericMenu menu = new GenericMenu();
                menu.AddDisabledItem(new GUIContent("组件"));
                menu.AddItem(new GUIContent("X (扫描)"), false, () =>
                {
                    if (Application.isPlaying)
                    {
                        util_XHUDGUI.Open(XHudDialogType.警告, "HudElement消息", "扫描元素", "程序正在运行，无法在运行期间执行此功能！", "明白");
                        return;
                    }
                    GetAllAnimators();
                    GetAllText();
                    GetAllTmpText();
                    GetAllSounder();
                    GetAllButtons();
                    GetAllOptions();
                    GetAllSliders();
                    GetAllProgress();
                    GetAllContainer();
                    GetAllToggle();
                    GetModulesResult();
                    return;
                });
                menu.AddSeparator("");
                menu.AddDisabledItem(new GUIContent("元素RMS布局信息"));
                if (RMS_Enabled.boolValue)
                {
                    menu.AddItem(new GUIContent("D (记录)"), false, () =>
                    {
                        if (Application.isPlaying)
                        {
                            util_XHUDGUI.Open(XHudDialogType.警告, "HudElement消息", "记录RMS方案信息", "程序正在运行，无法在运行期间执行此功能！", "明白");
                            return;
                        }

                        RMS_Record();
                    });
                    menu.AddItem(new GUIContent("C (清空)"), false, () =>
                    {
                        if (Application.isPlaying)
                        {
                            util_XHUDGUI.Open(XHudDialogType.警告, "HudElement消息", "清空RMS方案信息", "程序正在运行，无法在运行期间执行此功能！", "明白");
                            return;
                        }

                        string res = util_XHUDGUI.Open(XHudDialogType.警告, "HudElement消息", "RMS方案操作", "如果清空匹配分辨率信息列表，会导致您之前为不同分辨率记录的坐标信息全部清空，请谨慎此操作！", "清空", "暂不", 1);
                        if (res == "清空")
                        {
                            RMS_InfoList.ClearArray();
                            RMS_InfoList.serializedObject.ApplyModifiedProperties();
                        }
                        return;
                    });
                }
                menu.AddDisabledItem(new GUIContent("坐标"));
                menu.AddItem(new GUIContent("R (回到锚点)"), false, () =>
                {
                    if (Application.isPlaying)
                    {
                        util_XHUDGUI.Open(XHudDialogType.警告, "HudElement消息", "锚点初始化", "程序正在运行，无法在运行期间执行此功能！", "明白");
                        return;
                    }
                    RestoreToAnchorPosition();
                });
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("F (折叠组件列表)"), false, () =>
                {
                    AllListFoldState(false);
                });
                menu.AddItem(new GUIContent("V (展开组件列表)"), false, () =>
                {
                    AllListFoldState(true);
                });
                if (!IsMultiSelected())
                {
                    menu.AddSeparator("");
                    menu.AddDisabledItem(new GUIContent("预览"));
                    if (!Preivew_Animator_PlayingState)
                        menu.AddItem(new GUIContent("S (开始)"), false, () =>
                        {
                            if (Application.isPlaying)
                            {
                                util_XHUDGUI.Open(XHudDialogType.警告, "HudElement消息", "预览动画", "程序正在运行，无法在运行期间执行此功能！", "明白");
                                return;
                            }
                            Preview_Animator_Play();
                        });
                    else
                        menu.AddItem(new GUIContent("S (停止)"), false, () =>
                        {
                            if (Application.isPlaying)
                            {
                                util_XHUDGUI.Open(XHudDialogType.警告, "HudElement消息", "停止预览动画", "程序正在运行，无法在运行期间执行此功能！", "明白");
                                return;
                            }
                            Preview_Animator_Stop();
                        });
                }

                if (PrefabUtility.IsAnyPrefabInstanceRoot(BaseScript.gameObject))
                {
                    menu.AddSeparator("");
                    menu.AddDisabledItem(new GUIContent("预制体"));
                    menu.AddItem(new GUIContent("A (应用)"), false, () =>
                    {
                        if (Application.isPlaying)
                        {
                            util_XHUDGUI.Open(XHudDialogType.警告, "HudElement消息", "预制体应用", "程序正在运行，无法在运行期间执行此功能！", "明白");
                            return;
                        }
                        string path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(BaseScript.gameObject);
                        PrefabUtility.SaveAsPrefabAssetAndConnect(BaseScript.gameObject, path, InteractionMode.AutomatedAction);
                    });
                    menu.AddItem(new GUIContent("Z (定位)"), false, () =>
                    {
                        if (Application.isPlaying)
                        {
                            util_XHUDGUI.Open(XHudDialogType.警告, "HudElement消息", "预制体定位", "程序正在运行，无法在运行期间执行此功能！", "明白");
                            return;
                        }

                        string path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(BaseScript.gameObject);
                        //Selection.activeObject = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
                        Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
                        EditorGUIUtility.PingObject(obj);
                    });
                }
                menu.ShowAsContext(); // 在鼠标位置显示右键菜单
            }

            CheckModulesValid();

            #region 源脚本
            util_XHUDGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 3, "源脚本", util_Dashboard.Theme_Primary);
            util_XHUDGUI.Gui_Layout_Space(5);

            #region 原始变量
            util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            util_XHUDGUI.Gui_Layout_Space(10);
            BasicVars = EditorGUILayout.Foldout(BasicVars, "变量/属性", true);
            util_XHUDGUI.Gui_Layout_Space(5);
            util_XHUDGUI.Gui_Layout_Horizontal_End();
            if (BasicVars)
                DrawDefaultInspector();
            #endregion

            util_XHUDGUI.Gui_Layout_Space(5);
            util_XHUDGUI.Gui_Layout_Vertical_End();
            #endregion

            CalculateAnimatorMaxDuration(BaseScript.AnimatorNodes, Element_Animators_GlobalDuration.floatValue);

            serializedObject.ApplyModifiedProperties();
        }

        #region RMS

        /// <summary>
        /// 根据当前元素所选定的布局方案进行布局重绘
        /// </summary>
        private void RMS_Redraw()
        {
            Hud_Manager mgr = util_Dashboard.HudManagerGet();

            //--判断当前元素是否在场景中
            if (!IsInPrefabStageMode())
            {
                if (!string.IsNullOrEmpty(BaseScript.gameObject.scene.name))
                {
                    if (RMS_Enabled.boolValue)
                    {
                        #region 以Hud管理器中指定的RMS布局方案来对此元素进行坐标信息复位

                        string solution = mgr.hm_RMS_GetCurrentSolution();

                        if (IsMultiSelected())
                        {
                            for (int i = 0; i < SelectedObjects.Length; i++)
                            {
                                RMS_Preview(SelectedObjects[i], SelectedObjects[i].Alpha, Vector3.zero, solution, true);
                            }
                        }
                        else
                        {
                            RMS_Preview(BaseScript, Alpha.floatValue, Vector3.zero, solution, true);
                        }
                        #endregion
                    }
                }
            }
        }

        /// <summary>
        /// 布局信息是否存在
        /// </summary>
        /// <returns></returns>
        private bool RMS_IsExist()
        {
            bool isExist = false;
            for (int i = 0; i < RMS_InfoList.arraySize; i++)
            {
                SerializedProperty sp_name = RMS_InfoList.GetArrayElementAtIndex(i).FindPropertyRelative("LayoutName");

                if (sp_name.stringValue == RMS_Name.stringValue)
                {
                    isExist = true;
                    break;
                }
            }
            return isExist;
        }

        /// <summary>
        /// 获取当前元素的RMS状态信息
        /// </summary>
        /// <returns></returns>
        private OriginalLayoutInfo RMS_GetCurrentInfo()
        {
            OriginalLayoutInfo info = new OriginalLayoutInfo();

            HudAnchor anchor = HudAnchor.中心;
            string ParentName = BaseScript.transform.parent.name;

            if (ParentName == "Anchor_B")
            {
                anchor = HudAnchor.底层;
            }
            if (ParentName == "Anchor_C")
            {
                anchor = HudAnchor.中心;
            }
            if (ParentName == "Anchor_L")
            {
                anchor = HudAnchor.左;
            }
            if (ParentName == "Anchor_R")
            {
                anchor = HudAnchor.右;
            }
            if (ParentName == "Anchor_U")
            {
                anchor = HudAnchor.上;
            }
            if (ParentName == "Anchor_D")
            {
                anchor = HudAnchor.下;
            }
            if (ParentName == "Anchor_L_U")
            {
                anchor = HudAnchor.左上;
            }
            if (ParentName == "Anchor_L_D")
            {
                anchor = HudAnchor.左下;
            }
            if (ParentName == "Anchor_R_U")
            {
                anchor = HudAnchor.右上;
            }
            if (ParentName == "Anchor_R_D")
            {
                anchor = HudAnchor.右下;
            }
            if (ParentName == "Anchor_T")
            {
                anchor = HudAnchor.顶层;
            }

            info.Anchor = anchor;
            info.AnchorMin = BaseScript.RectTransform.anchorMin;
            info.AnchorMax = BaseScript.RectTransform.anchorMax;
            info.Pivot = BaseScript.RectTransform.pivot;
            info.Position = BaseScript.RectTransform.anchoredPosition3D;
            info.Euler = BaseScript.RectTransform.localEulerAngles;
            info.Scale = BaseScript.RectTransform.localScale;

            return info;
        }

        /// <summary>
        /// RMS设置
        /// </summary>
        /// <param name="property"></param>
        private void RMS_Set(SerializedProperty property)
        {
            SerializedProperty sp_Anchor = property.FindPropertyRelative("Anchor");
            SerializedProperty sp_Position = property.FindPropertyRelative("Position");
            SerializedProperty sp_Euler = property.FindPropertyRelative("Euler");
            SerializedProperty sp_Scale = property.FindPropertyRelative("Scale");
            SerializedProperty sp_AnchorMin = property.FindPropertyRelative("AnchorMin");
            SerializedProperty sp_AnchorMax = property.FindPropertyRelative("AnchorMax");
            SerializedProperty sp_Pivot = property.FindPropertyRelative("Pivot");
            SerializedProperty sp_LayoutName = property.FindPropertyRelative("LayoutName");

            OriginalLayoutInfo layout_info = RMS_GetCurrentInfo();

            sp_Anchor.enumValueIndex = (int)layout_info.Anchor;
            sp_Position.vector3Value = layout_info.Position;
            sp_Euler.vector3Value = layout_info.Euler;
            sp_Scale.vector3Value = layout_info.Scale;
            sp_AnchorMin.vector2Value = layout_info.AnchorMin;
            sp_AnchorMax.vector2Value = layout_info.AnchorMax;
            sp_Pivot.vector2Value = layout_info.Pivot;
            sp_LayoutName.stringValue = RMS_Name.stringValue;

            property.serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// RMS记录
        /// </summary>
        private void RMS_Record()
        {
            Hud_Manager mgr = util_Dashboard.HudManagerGet();

            if (!mgr.RMS_Enabled)
            {
                util_XHUDGUI.Open(XHudDialogType.警告, "HudElement消息", "RMS记录布局", "Hud管理器中RMS未开启！无法执行记录布局信息操作！", "明白");
                return;
            }

            if (mgr.hm_RMS_IsEmpty())
            {
                string res = util_XHUDGUI.Open(XHudDialogType.警告, "HudElement消息", "RMS记录布局", "并未发现您在HudManager里配置RMS信息！请先配置RMS列表！", "去配置", "暂不", 0);
                if (res == "去配置")
                {
                    Selection.activeGameObject = mgr.gameObject;
                    EditorGUIUtility.PingObject(mgr);
                    return;
                }
                if (res == "暂不")
                {
                    return;
                }
            }

            if (IsMultiSelected())
            {
                util_XHUDGUI.Open(XHudDialogType.警告, "HudElement消息", "RMS批量记录", "因为考虑到每个元素当前设计布局可能不一致，因此不支持批量记录设计布局信息！", "明白");
                return;
            }
            else
            {
                if (!mgr.RMS_Enabled)
                    return;

                if (RMS_InfoList.arraySize <= 0)
                {
                    RMS_InfoList.InsertArrayElementAtIndex(RMS_InfoList.arraySize);
                    SerializedProperty newLayoutInfo = RMS_InfoList.GetArrayElementAtIndex(RMS_InfoList.arraySize - 1);
                    RMS_Set(newLayoutInfo);
                }
                else
                {
                    bool IsExist = false;
                    for (int i = 0; i < RMS_InfoList.arraySize; i++)
                    {
                        SerializedProperty sp_list_cur_solution = RMS_InfoList.GetArrayElementAtIndex(i);
                        SerializedProperty sp_list_cur_solution_name = sp_list_cur_solution.FindPropertyRelative("LayoutName");

                        if (sp_list_cur_solution_name.stringValue == RMS_Name.stringValue)
                        {
                            RMS_Set(sp_list_cur_solution);
                            IsExist = true;
                            break;
                        }
                    }
                    if (!IsExist)
                    {
                        RMS_InfoList.InsertArrayElementAtIndex(RMS_InfoList.arraySize);
                        SerializedProperty newLayoutInfo = RMS_InfoList.GetArrayElementAtIndex(RMS_InfoList.arraySize - 1);
                        RMS_Set(newLayoutInfo);
                    }
                }
            }

            util_XHUDGUI.Open(XHudDialogType.确认, "HudElement消息", "RMS记录布局", "已记录  \"" + RMS_Name.stringValue + "\"  设计布局信息！", "明白");
        }

        /// <summary>
        /// RMS 预览
        /// </summary>
        /// <param name="element"></param>
        /// <param name="alpha"></param>
        /// <param name="offset"></param>
        /// <param name="solution"></param>
        /// <param name="DontCreateID"></param>
        private void RMS_Preview(Hud_Element element, float alpha, Vector3 offset, string solution, bool DontCreateID)
        {
            Hud_Manager mgr = util_Dashboard.HudManagerGet();
            mgr.hm_HudElement_Initialize_ByDesignLayout_For_Screen(element, alpha, offset, solution, DontCreateID);
        }
        #endregion

        #region 辅助
        /// <summary>
        /// 序列化变量
        /// </summary>
        private SerializedProperty GetSerializedProperty(string name)
        {
            return serializedObject.FindProperty(name);
        }

        /// <summary>
        /// 获取序列化变量
        /// </summary>
        private void SerializedAllVariables()
        {
            SounderNodes = GetSerializedProperty("SounderNodes");
            AnimatorNodes = GetSerializedProperty("AnimatorNodes");
            ButtonNodes = GetSerializedProperty("ButtonNodes");
            OptionNodes = GetSerializedProperty("OptionNodes");
            SliderNodes = GetSerializedProperty("SliderNodes");
            ProgressNodes = GetSerializedProperty("ProgressNodes");
            TextNodes = GetSerializedProperty("TextNodes");
            TmpTextNodes = GetSerializedProperty("TmpTextNodes");
            ToggleNodes = GetSerializedProperty("ToggleNodes");
            ContainerNodes = GetSerializedProperty("ContainerNodes");
            DebugState = GetSerializedProperty("DebugState");
            CanvasGroup = GetSerializedProperty("CanvasGroup");
            RectTransform = GetSerializedProperty("RectTransform");
            TriggerAction = GetSerializedProperty("TriggerAction");
            ObjectTracker = GetSerializedProperty("ObjectTracker");
            AutoPlayAnimators = GetSerializedProperty("AutoPlayAnimators");
            AutoPlayContainersAnimators = GetSerializedProperty("AutoPlayContainersAnimators");
            AutoStopPreview = GetSerializedProperty("AutoStopPreview");
            eve_on_element_in_start = GetSerializedProperty("eve_on_element_in_start");
            eve_on_element_in_end = GetSerializedProperty("eve_on_element_in_end");
            eve_on_element_out_start = GetSerializedProperty("eve_on_element_out_start");
            eve_on_element_out_end = GetSerializedProperty("eve_on_element_out_end");
            TextIsFold = GetSerializedProperty("TextIsFold");
            TmpTextIsFold = GetSerializedProperty("TmpTextIsFold");
            Indicator = GetSerializedProperty("Indicator");
            AnimatorsMaxDuration = GetSerializedProperty("AnimatorsMaxDuration");
            CreateState = GetSerializedProperty("CreateState");
            AnimateState = GetSerializedProperty("AnimateState");
            OriginPoolName = GetSerializedProperty("OriginPoolName");
            OriginalName = GetSerializedProperty("OriginalName");
            Alpha = GetSerializedProperty("Alpha");
            CurrentPivot = GetSerializedProperty("CurrentPivot");
            Element_Animators_GlobalDuration = GetSerializedProperty("Element_Animators_GlobalDuration");
            AnimatorsIsFold = GetSerializedProperty("AnimatorsIsFold");
            ButtonIsFold = GetSerializedProperty("ButtonIsFold");
            OptionIsFold = GetSerializedProperty("OptionIsFold");
            SliderIsFold = GetSerializedProperty("SliderIsFold");
            ProgressIsFold = GetSerializedProperty("ProgressIsFold");
            ContainerIsFold = GetSerializedProperty("ContainerIsFold");
            ToggleIsFold = GetSerializedProperty("ToggleIsFold");
            EventIsFold = GetSerializedProperty("EventIsFold");
            SounderIsFold = GetSerializedProperty("SounderIsFold");
            RMS_Enabled = GetSerializedProperty("RMS_Enabled");
            RMS_InfoList = GetSerializedProperty("RMS_InfoList");
            RMS_Name = GetSerializedProperty("RMS_Name");
        }

        /// <summary>
        /// 元素回到锚点初始位置
        /// </summary>
        private void RestoreToAnchorPosition()
        {
            string res = util_XHUDGUI.Open(XHudDialogType.帮助, "HudElement消息", "锚点初始化", "是否需要将元素回到当前所在的锚点初始位置！", "确定", "暂不", 1);
            if (res == "暂不")
            {
                return;
            }
            if (IsMultiSelected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    SerializedObject so = new SerializedObject(SelectedObjects[i]);
                    Hud_Element ele = (Hud_Element)so.targetObject;
                    Undo.RegisterCompleteObjectUndo(ele.transform, "MoveToAnchorPosition" + i);
                    ele.element_PositionResetZero();
                    so.Update();

                    so.ApplyModifiedProperties();
                }
            }
            else
            {
                Undo.RegisterCompleteObjectUndo(BaseScript.transform, "MoveToAnchorPosition");
                BaseScript.element_PositionResetZero();
            }
        }

        /// <summary>
        /// 判断模组列表是否存在
        /// </summary>
        private void CheckModulesValid()
        {
            #region Container
            for (int i = 0; i < ContainerNodes.arraySize; i++)
            {
                SerializedProperty sp_root = ContainerNodes.GetArrayElementAtIndex(i);
                if (sp_root != null)
                {
                    SerializedProperty sp_con = sp_root.FindPropertyRelative("Container");
                    Hud_Container con = (Hud_Container)sp_con.objectReferenceValue;
                    if (con == null)
                    {
                        ContainerNodes.DeleteArrayElementAtIndex(i);
                    }
                }
            }
            #endregion

            #region Button
            for (int i = 0; i < ButtonNodes.arraySize; i++)
            {
                SerializedProperty sp_root = ButtonNodes.GetArrayElementAtIndex(i);
                if (sp_root != null)
                {
                    SerializedProperty sp_con = sp_root.FindPropertyRelative("Button");
                    Hud_Button con = (Hud_Button)sp_con.objectReferenceValue;
                    if (con == null)
                    {
                        ButtonNodes.DeleteArrayElementAtIndex(i);
                    }
                }
            }
            #endregion

            #region Toggle
            for (int i = 0; i < ToggleNodes.arraySize; i++)
            {
                SerializedProperty sp_root = ToggleNodes.GetArrayElementAtIndex(i);
                if (sp_root != null)
                {
                    SerializedProperty sp_con = sp_root.FindPropertyRelative("Toggle");
                    Hud_Toggle con = (Hud_Toggle)sp_con.objectReferenceValue;
                    if (con == null)
                    {
                        ToggleNodes.DeleteArrayElementAtIndex(i);
                    }
                }
            }
            #endregion

            #region Progress
            for (int i = 0; i < ProgressNodes.arraySize; i++)
            {
                SerializedProperty sp_root = ProgressNodes.GetArrayElementAtIndex(i);
                if (sp_root != null)
                {
                    SerializedProperty sp_con = sp_root.FindPropertyRelative("Progress");
                    Hud_Progress con = (Hud_Progress)sp_con.objectReferenceValue;
                    if (con == null)
                    {
                        ProgressNodes.DeleteArrayElementAtIndex(i);
                    }
                }
            }
            #endregion

            #region Slider
            for (int i = 0; i < SliderNodes.arraySize; i++)
            {
                SerializedProperty sp_root = SliderNodes.GetArrayElementAtIndex(i);
                if (sp_root != null)
                {
                    SerializedProperty sp_con = sp_root.FindPropertyRelative("Slider");
                    Hud_Slider con = (Hud_Slider)sp_con.objectReferenceValue;
                    if (con == null)
                    {
                        SliderNodes.DeleteArrayElementAtIndex(i);
                    }
                }
            }
            #endregion

            #region Option
            for (int i = 0; i < OptionNodes.arraySize; i++)
            {
                SerializedProperty sp_root = OptionNodes.GetArrayElementAtIndex(i);
                if (sp_root != null)
                {
                    SerializedProperty sp_con = sp_root.FindPropertyRelative("Option");
                    Hud_Option con = (Hud_Option)sp_con.objectReferenceValue;
                    if (con == null)
                    {
                        OptionNodes.DeleteArrayElementAtIndex(i);
                    }
                }
            }
            #endregion

            #region Sounder
            for (int i = 0; i < SounderNodes.arraySize; i++)
            {
                SerializedProperty sp_root = SounderNodes.GetArrayElementAtIndex(i);
                if (sp_root != null)
                {
                    SerializedProperty sp_con = sp_root.FindPropertyRelative("Sounder");
                    Hud_Sounder con = (Hud_Sounder)sp_con.objectReferenceValue;
                    if (con == null)
                    {
                        SounderNodes.DeleteArrayElementAtIndex(i);
                    }
                }
            }
            #endregion

            #region Animator
            for (int i = 0; i < AnimatorNodes.arraySize; i++)
            {
                SerializedProperty sp_root = AnimatorNodes.GetArrayElementAtIndex(i);
                if (sp_root != null)
                {
                    SerializedProperty sp_con = sp_root.FindPropertyRelative("Animator");
                    Hud_Animator con = (Hud_Animator)sp_con.objectReferenceValue;
                    if (con == null)
                    {
                        AnimatorNodes.DeleteArrayElementAtIndex(i);
                    }
                }
            }
            #endregion
        }

        /// <summary>
        /// 弹窗显示获取组件结果
        /// </summary>
        private void GetModulesResult()
        {
            List<XHudDialogListDatas> Datas = new List<XHudDialogListDatas>();
            if (IsMultiSelected())
            {
                for (int s = 0; s < SelectedObjects.Length; s++)
                {
                    bool existcomp = false;

                    Hud_Element ele = SelectedObjects[s];
                    //-扫描 - 动画器 
                    for (int i = 0; i < ele.AnimatorNodes.Count; i++)
                    {
                        XHudDialogListDatas dataitem = new XHudDialogListDatas();
                        dataitem.Title = $"{SelectedObjects[s].name} ( {SelectedObjects[s].Indicator} )";
                        dataitem.SubTitle = "已收集 / 动画器";
                        dataitem.Message = $"{ele.AnimatorNodes[i].Animator.name} ( {ele.AnimatorNodes[i].Animator.Indicator} )";
                        Datas.Add(dataitem);
                        existcomp = true;
                    }
                    //-扫描 - 按钮
                    for (int i = 0; i < ele.ButtonNodes.Count; i++)
                    {
                        XHudDialogListDatas dataitem = new XHudDialogListDatas();
                        dataitem.Title = $"{SelectedObjects[s].name} ( {SelectedObjects[s].Indicator} )";
                        dataitem.SubTitle = "已收集 / 按钮";
                        dataitem.Message = $"{ele.ButtonNodes[i].Button.name} ( {ele.ButtonNodes[i].Button.Indicator} )";
                        Datas.Add(dataitem);
                        existcomp = true;
                    }
                    //-扫描 - 选项器
                    for (int i = 0; i < ele.OptionNodes.Count; i++)
                    {
                        XHudDialogListDatas dataitem = new XHudDialogListDatas();
                        dataitem.Title = $"{SelectedObjects[s].name} ( {SelectedObjects[s].Indicator} )";
                        dataitem.SubTitle = "已收集 / 选项";
                        dataitem.Message = $"{ele.OptionNodes[i].Option.name} ( {ele.OptionNodes[i].Option.Indicator} )";
                        Datas.Add(dataitem);
                        existcomp = true;
                    }
                    //-扫描 - 滑动条
                    for (int i = 0; i < ele.SliderNodes.Count; i++)
                    {
                        XHudDialogListDatas dataitem = new XHudDialogListDatas();
                        dataitem.Title = $"{SelectedObjects[s].name} ( {SelectedObjects[s].Indicator} )";
                        dataitem.SubTitle = "已收集 / 滑动条";
                        dataitem.Message = $"{ele.SliderNodes[i].Slider.name} ( {ele.SliderNodes[i].Slider.Indicator} )";
                        Datas.Add(dataitem);
                        existcomp = true;
                    }
                    //-扫描 - 进度条
                    for (int i = 0; i < ele.ProgressNodes.Count; i++)
                    {
                        XHudDialogListDatas dataitem = new XHudDialogListDatas();
                        dataitem.Title = $"{SelectedObjects[s].name} ( {SelectedObjects[s].Indicator} )";
                        dataitem.SubTitle = "已收集 / 进度条";
                        dataitem.Message = $"{ele.ProgressNodes[i].Progress.name} ( {ele.ProgressNodes[i].Progress.Indicator} )";
                        Datas.Add(dataitem);
                        existcomp = true;
                    }
                    //-扫描 - 开关
                    for (int i = 0; i < ele.ToggleNodes.Count; i++)
                    {
                        XHudDialogListDatas dataitem = new XHudDialogListDatas();
                        dataitem.Title = $"{SelectedObjects[s].name} ( {SelectedObjects[s].Indicator} )";
                        dataitem.SubTitle = "已收集 / 开关";
                        dataitem.Message = $"{ele.ToggleNodes[i].Toggle.name} ( {ele.ToggleNodes[i].Toggle.Indicator} )";
                        Datas.Add(dataitem);
                        existcomp = true;
                    }
                    //-扫描 - 声音
                    for (int i = 0; i < ele.SounderNodes.Count; i++)
                    {
                        XHudDialogListDatas dataitem = new XHudDialogListDatas();
                        dataitem.Title = $"{SelectedObjects[s].name} ( {SelectedObjects[s].Indicator} )";
                        dataitem.SubTitle = "已收集 / 音效器";
                        dataitem.Message = $"{ele.SounderNodes[i].Sounder.name} ( {ele.SounderNodes[i].Sounder.Indicator} )";
                        Datas.Add(dataitem);
                        existcomp = true;
                    }
                    //-扫描 - 容器
                    for (int i = 0; i < ele.ContainerNodes.Count; i++)
                    {
                        XHudDialogListDatas dataitem = new XHudDialogListDatas();
                        dataitem.Title = $"{SelectedObjects[s].name} ( {SelectedObjects[s].Indicator} )";
                        dataitem.SubTitle = "已收集 / 容器";
                        dataitem.Message = $"{ele.ContainerNodes[i].Container.name} ( {ele.ContainerNodes[i].Container.Indicator} )";
                        Datas.Add(dataitem);
                        existcomp = true;
                    }
                    //-扫描 - Text文字
                    for (int i = 0; i < ele.TextNodes.Count; i++)
                    {
                        XHudDialogListDatas dataitem = new XHudDialogListDatas();
                        dataitem.Title = $"{SelectedObjects[s].name} ( {SelectedObjects[s].Indicator} )";
                        dataitem.SubTitle = "已收集 / Text文字";
                        dataitem.Message = $"{ele.TextNodes[i].Text.name} ( {ele.TextNodes[i].Text.Indicator} )";
                        Datas.Add(dataitem);
                        existcomp = true;
                    }
                    //-扫描 - Tmp文字
                    for (int i = 0; i < ele.TmpTextNodes.Count; i++)
                    {
                        XHudDialogListDatas dataitem = new XHudDialogListDatas();
                        dataitem.Title = $"{SelectedObjects[s].name} ( {SelectedObjects[s].Indicator} )";
                        dataitem.SubTitle = "已收集 / Text文字";
                        dataitem.Message = $"{ele.TmpTextNodes[i].TmpText.name} ( {ele.TmpTextNodes[i].TmpText.Indicator} )";
                        Datas.Add(dataitem);
                        existcomp = true;
                    }

                    if (!existcomp)
                    {
                        XHudDialogListDatas dataitem = new XHudDialogListDatas();
                        dataitem.Title = $"{SelectedObjects[s].name} ( {SelectedObjects[s].Indicator} )";
                        dataitem.SubTitle = "";
                        dataitem.Message = "未发现任何存在的子组件";
                        Datas.Add(dataitem);
                    }
                }
                util_XHUDGUI.Open(Datas.ToArray(), XHudDialogType.确认, "HudElement消息", "批量扫描元素子组件", "以下是批量扫描到的所有元素子组件列表，请您检查核对：", "明白");
            }
            else
            {
                //-扫描 - 动画器
                for (int i = 0; i < BaseScript.AnimatorNodes.Count; i++)
                {
                    XHudDialogListDatas dataitem = new XHudDialogListDatas();
                    dataitem.Title = $"{BaseScript.name} ({BaseScript.Indicator} )";
                    dataitem.SubTitle = "已收集 / 动画器";
                    dataitem.Message = $"{BaseScript.AnimatorNodes[i].Animator.name} ({BaseScript.AnimatorNodes[i].Animator.Indicator} )";
                    Datas.Add(dataitem);
                }
                //-扫描 - 按钮
                for (int i = 0; i < BaseScript.ButtonNodes.Count; i++)
                {
                    XHudDialogListDatas dataitem = new XHudDialogListDatas();
                    dataitem.Title = $"{BaseScript.name} ({BaseScript.Indicator} )";
                    dataitem.SubTitle = "已收集 / 按钮";
                    dataitem.Message = $"{BaseScript.ButtonNodes[i].Button.name} ({BaseScript.ButtonNodes[i].Button.Indicator} )";
                    Datas.Add(dataitem);
                }
                //-扫描 - 选项器
                for (int i = 0; i < BaseScript.OptionNodes.Count; i++)
                {
                    XHudDialogListDatas dataitem = new XHudDialogListDatas();
                    dataitem.Title = $"{BaseScript.name} ({BaseScript.Indicator} )";
                    dataitem.SubTitle = "已收集 / 选项";
                    dataitem.Message = $"{BaseScript.OptionNodes[i].Option.name} ({BaseScript.OptionNodes[i].Option.Indicator} )";
                    Datas.Add(dataitem);
                }
                //-扫描 - 滑动条
                for (int i = 0; i < BaseScript.SliderNodes.Count; i++)
                {
                    XHudDialogListDatas dataitem = new XHudDialogListDatas();
                    dataitem.Title = $"{BaseScript.name} ({BaseScript.Indicator} )";
                    dataitem.SubTitle = "已收集 / 滑动条";
                    dataitem.Message = $"{BaseScript.SliderNodes[i].Slider.name} ({BaseScript.SliderNodes[i].Slider.Indicator} )";
                    Datas.Add(dataitem);
                }
                //-扫描 - 进度条
                for (int i = 0; i < BaseScript.ProgressNodes.Count; i++)
                {
                    XHudDialogListDatas dataitem = new XHudDialogListDatas();
                    dataitem.Title = $"{BaseScript.name} ({BaseScript.Indicator} )";
                    dataitem.SubTitle = "已收集 / 进度条";
                    dataitem.Message = $"{BaseScript.ProgressNodes[i].Progress.name} ({BaseScript.ProgressNodes[i].Progress.Indicator} )";
                    Datas.Add(dataitem);
                }
                //-扫描 - 开关
                for (int i = 0; i < BaseScript.ToggleNodes.Count; i++)
                {
                    XHudDialogListDatas dataitem = new XHudDialogListDatas();
                    dataitem.Title = $"{BaseScript.name} ({BaseScript.Indicator} )";
                    dataitem.SubTitle = "已收集 / 开关";
                    dataitem.Message = $"{BaseScript.ToggleNodes[i].Toggle.name} ({BaseScript.ToggleNodes[i].Toggle.Indicator} )";
                    Datas.Add(dataitem);
                }
                //-扫描 - 声音
                for (int i = 0; i < BaseScript.SounderNodes.Count; i++)
                {
                    XHudDialogListDatas dataitem = new XHudDialogListDatas();
                    dataitem.Title = $"{BaseScript.name} ({BaseScript.Indicator} )";
                    dataitem.SubTitle = "已收集 / 音效器";
                    dataitem.Message = $"{BaseScript.SounderNodes[i].Sounder.name} ({BaseScript.SounderNodes[i].Sounder.Indicator} )";
                    Datas.Add(dataitem);
                }
                //-扫描 - 容器
                for (int i = 0; i < BaseScript.ContainerNodes.Count; i++)
                {
                    XHudDialogListDatas dataitem = new XHudDialogListDatas();
                    dataitem.Title = $"{BaseScript.name} ({BaseScript.Indicator} )";
                    dataitem.SubTitle = "已收集 / 容器";
                    dataitem.Message = $"{BaseScript.ContainerNodes[i].Container.name} ({BaseScript.ContainerNodes[i].Container.Indicator} )";
                    Datas.Add(dataitem);
                }
                //-扫描 - Text文字
                for (int i = 0; i < BaseScript.TextNodes.Count; i++)
                {
                    XHudDialogListDatas dataitem = new XHudDialogListDatas();
                    dataitem.Title = $"{BaseScript.name} ({BaseScript.Indicator} )";
                    dataitem.SubTitle = "已收集 / Text文字";
                    dataitem.Message = $"{BaseScript.TextNodes[i].Text.name} ({BaseScript.TextNodes[i].Text.Indicator} )";
                    Datas.Add(dataitem);
                }
                //-扫描 - Tmp文字
                for (int i = 0; i < BaseScript.TmpTextNodes.Count; i++)
                {
                    XHudDialogListDatas dataitem = new XHudDialogListDatas();
                    dataitem.Title = $"{BaseScript.name} ({BaseScript.Indicator} )";
                    dataitem.SubTitle = "已收集 / Text文字";
                    dataitem.Message = $"{BaseScript.TmpTextNodes[i].TmpText.name} ({BaseScript.TmpTextNodes[i].TmpText.Indicator} )";
                    Datas.Add(dataitem);
                }

                if (BaseScript.AnimatorNodes.Count <= 0 && BaseScript.ButtonNodes.Count <= 0 && BaseScript.OptionNodes.Count <= 0 && BaseScript.SliderNodes.Count <= 0 && BaseScript.ProgressNodes.Count <= 0 && BaseScript.ToggleNodes.Count <= 0 && BaseScript.SounderNodes.Count <= 0 && BaseScript.ContainerNodes.Count <= 0 && BaseScript.TextNodes.Count <= 0 && BaseScript.TmpTextNodes.Count <= 0)
                {
                    string hexcol = util_Tools.Color_To_HexColor(util_Dashboard.Theme_Primary, true);
                    util_XHUDGUI.Open(XHudDialogType.警告, "HudElement消息", "扫描元素子组件", $"未扫描到任何子元素，请您检查子物体中是否有物体包含以下组件： <color={hexcol}>Hud_Animator、Hud_Button、Hud_Progress、Hud_Slider、Hud_Option、HudSound、Hud_Container、Hud_Text、Hud_TmpText、HudToggle</color>", "明白");
                }
                else
                    util_XHUDGUI.Open(Datas.ToArray(), XHudDialogType.确认, "HudElement消息", "扫描元素子组件", "以下是扫描到的元素所有子组件列表，请您检查核对：", "明白");
            }
        }

        /// <summary>
        /// 获取组件
        /// </summary>
        private void GetComponents()
        {
            if (RectTransform.objectReferenceValue == null)
            {
                RectTransform.objectReferenceValue = BaseScript.GetComponent<RectTransform>();
                RectTransform.serializedObject.ApplyModifiedProperties();
            }
            if (CanvasGroup.objectReferenceValue == null)
            {
                CanvasGroup.objectReferenceValue = BaseScript.GetComponent<CanvasGroup>();
                CanvasGroup.serializedObject.ApplyModifiedProperties();
            }
            if (TriggerAction.objectReferenceValue == null)
            {
                TriggerAction.objectReferenceValue = BaseScript.GetComponent<Hud_ElementTriggerAction>();
                TriggerAction.serializedObject.ApplyModifiedProperties();
            }
            if (ObjectTracker.objectReferenceValue == null)
            {
                ObjectTracker.objectReferenceValue = BaseScript.GetComponent<Hud_ObjectTracker>();
                ObjectTracker.serializedObject.ApplyModifiedProperties();
            }
        }

        /// <summary>
        /// 改变元素透明度
        /// </summary>
        private void ChangeAlpha()
        {
            if (IsMultiSelected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    SerializedObject so_ele = new SerializedObject(SelectedObjects[i]);
                    SerializedProperty sp_alpha = so_ele.FindProperty("Alpha");
                    SerializedProperty sp_cgp = so_ele.FindProperty("CanvasGroup");

                    CanvasGroup cgp = (CanvasGroup)sp_cgp.objectReferenceValue;


                    so_ele.Update();
                    sp_alpha.floatValue = Alpha.floatValue;
                    Undo.RecordObject(cgp, "ChangeAlpha");
                    cgp.alpha = Alpha.floatValue;

                    sp_cgp.serializedObject.ApplyModifiedProperties();
                    sp_alpha.serializedObject.ApplyModifiedProperties();
                    so_ele.ApplyModifiedProperties();
                }
            }
            else
            {
                CanvasGroup cgp = (CanvasGroup)CanvasGroup.objectReferenceValue;
                Undo.RecordObject(cgp, "ChangeAlpha");
                cgp.alpha = Alpha.floatValue;
                CanvasGroup.serializedObject.ApplyModifiedProperties();
            }
        }

        /// <summary>
        /// 折叠列表
        /// </summary>
        /// <param name="state"></param>
        private void AllListFoldState(bool state)
        {
            if (IsMultiSelected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    if (SelectedObjects[i] == null)
                        continue;
                    SerializedObject so_ele = new SerializedObject(SelectedObjects[i]);
                    SerializedProperty sp_anim_isfold = so_ele.FindProperty("AnimatorsIsFold");
                    SerializedProperty sp_btn_isfold = so_ele.FindProperty("ButtonIsFold");
                    SerializedProperty sp_progress_isfold = so_ele.FindProperty("ProgressIsFold");
                    SerializedProperty sp_container_isfold = so_ele.FindProperty("ContainerIsFold");
                    SerializedProperty sp_tog_isfold = so_ele.FindProperty("ToggleIsFold");
                    SerializedProperty sp_txt_isfold = so_ele.FindProperty("TextIsFold");
                    SerializedProperty sp_tmp_txt_isfold = so_ele.FindProperty("TmpTextIsFold");
                    SerializedProperty sp_opt_isfold = so_ele.FindProperty("OptionIsFold");
                    SerializedProperty sp_sli_isfold = so_ele.FindProperty("SliderIsFold");
                    SerializedProperty sp_act_isfold = so_ele.FindProperty("EventIsFold");
                    SerializedProperty sp_sod_isfold = so_ele.FindProperty("SounderIsFold");

                    so_ele.Update();

                    sp_anim_isfold.boolValue = state;
                    sp_btn_isfold.boolValue = state;
                    sp_act_isfold.boolValue = state;
                    sp_sli_isfold.boolValue = state;
                    sp_opt_isfold.boolValue = state;
                    sp_sod_isfold.boolValue = state;
                    sp_txt_isfold.boolValue = state;
                    sp_tmp_txt_isfold.boolValue = state;
                    sp_progress_isfold.boolValue = state;
                    sp_container_isfold.boolValue = state;
                    sp_tog_isfold.boolValue = state;

                    sp_anim_isfold.serializedObject.ApplyModifiedProperties();
                    sp_btn_isfold.serializedObject.ApplyModifiedProperties();
                    sp_sli_isfold.serializedObject.ApplyModifiedProperties();
                    sp_opt_isfold.serializedObject.ApplyModifiedProperties();
                    sp_act_isfold.serializedObject.ApplyModifiedProperties();
                    sp_sod_isfold.serializedObject.ApplyModifiedProperties();
                    sp_txt_isfold.serializedObject.ApplyModifiedProperties();
                    sp_tmp_txt_isfold.serializedObject.ApplyModifiedProperties();
                    sp_progress_isfold.serializedObject.ApplyModifiedProperties();
                    sp_container_isfold.serializedObject.ApplyModifiedProperties();
                    sp_tog_isfold.serializedObject.ApplyModifiedProperties();

                    so_ele.ApplyModifiedProperties();
                }
            }
            else
            {
                if (target != null)
                {
                    AnimatorsIsFold.boolValue = state;
                    ButtonIsFold.boolValue = state;
                    OptionIsFold.boolValue = state;
                    SliderIsFold.boolValue = state;
                    EventIsFold.boolValue = state;
                    SounderIsFold.boolValue = state;
                    ToggleIsFold.boolValue = state;
                    TextIsFold.boolValue = state;
                    TmpTextIsFold.boolValue = state;
                    ProgressIsFold.boolValue = state;
                    ContainerIsFold.boolValue = state;

                    AnimatorsIsFold.serializedObject.ApplyModifiedProperties();
                    ButtonIsFold.serializedObject.ApplyModifiedProperties();
                    OptionIsFold.serializedObject.ApplyModifiedProperties();
                    SliderIsFold.serializedObject.ApplyModifiedProperties();
                    EventIsFold.serializedObject.ApplyModifiedProperties();
                    SounderIsFold.serializedObject.ApplyModifiedProperties();
                    ToggleIsFold.serializedObject.ApplyModifiedProperties();
                    TextIsFold.serializedObject.ApplyModifiedProperties();
                    TmpTextIsFold.serializedObject.ApplyModifiedProperties();
                    ProgressIsFold.serializedObject.ApplyModifiedProperties();
                    ContainerIsFold.serializedObject.ApplyModifiedProperties();

                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        private void CalculateAnimatorMaxDuration(List<AnimatorNode> list, float globaldur)
        {
            AnimatorsMaxDuration.floatValue = Animators_GetAnimatorsMaxDuration(list, globaldur);
            AnimatorsMaxDuration.serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 从所有子动画器中获取最大耗时
        /// </summary>
        /// <returns></returns>
        public float Animators_GetAnimatorsMaxDuration(List<AnimatorNode> list, float globaldur)
        {
            if (list.Count <= 0)
                return 0;
            float[] x_list = new float[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Animator == null)
                {
                    x_list[i] = 0;
                    continue;
                }
                list[i].Animator.TweenNodeTimersGet();
                x_list[i] = list[i].Animator.MaxTimerWithGlobalDuration + list[i].DelayTime;
            }
            float v = util_Tools.Array_MaxValue(x_list);
            return v * globaldur;
        }

        /// <summary>
        /// 获取动画器中是否存在循环模式
        /// </summary>
        /// <returns></returns>
        public bool Animators_HasLoopMode()
        {
            bool hasLoop = false;

            List<int> loops = new List<int>();
            for (int i = 0; i < AnimatorNodes.arraySize; i++)
            {
                SerializedProperty sp_animator = AnimatorNodes.GetArrayElementAtIndex(i).FindPropertyRelative("Animator");
                Hud_Animator anim = (Hud_Animator)sp_animator.objectReferenceValue;
                if (anim == null)
                {
                    continue;
                }
                if (anim == null && anim.AnimateTweenNodes == null || anim.AnimateTweenNodes.Count <= 0)
                {
                    continue;
                }
                else
                {
                    for (int s = 0; s < anim.AnimateTweenNodes.Count; s++)
                    {
                        loops.Add(anim.AnimateTweenNodes[s].LoopCount);
                    }
                }
            }

            for (int i = 0; i < loops.Count; i++)
            {
                if (loops[i] == -1)
                {
                    hasLoop = true;
                    break;
                }
            }
            return hasLoop;
        }

        /// <summary>
        /// 获取动画器中是否存在循环模式
        /// </summary>
        /// <returns></returns>
        public bool AnimatorTweenNodes_HasLoopMode(List<TweenNode> tweenlist)
        {
            bool hasLoop = false;

            for (int s = 0; s < tweenlist.Count; s++)
            {
                if (tweenlist[s].LoopCount == -1)
                {
                    hasLoop = true;
                    break;
                }
            }
            return hasLoop;
        }

        /// <summary>
        /// 判断当前物体及是否处于预制体独立场景模式中
        /// </summary>
        /// <returns></returns>
        private bool IsInPrefabStageMode()
        {
            bool IsInPrefabStage = false;
            PrefabStage stage = PrefabStageUtility.GetCurrentPrefabStage();
            if (stage == null)
            {
                IsInPrefabStage = false;
            }
            else
            {
                if (stage.mode == PrefabStage.Mode.InIsolation)
                    IsInPrefabStage = true;
                else if (stage.mode == PrefabStage.Mode.InContext)
                    IsInPrefabStage = true;
            }

            return IsInPrefabStage;
        }

        /// <summary>
        /// 还原Animator姿态
        /// </summary>
        private void OriginalPoseState_Load(Hud_Animator animator)
        {
            if (animator == null)
                return;

            SerializedObject so = new SerializedObject(animator);
            so.Update();
            SerializedProperty origin = so.FindProperty("OriginalAnimatorPoser");
            SerializedProperty m_rect = so.FindProperty("mod_RectTransform");
            SerializedProperty m_img = so.FindProperty("mod_Image");
            SerializedProperty m_text = so.FindProperty("mod_Text");
            SerializedProperty m_tmptext = so.FindProperty("mod_TmpText");
            SerializedProperty m_cav = so.FindProperty("CanvasGroup");

            SerializedProperty pos = origin.FindPropertyRelative("Position");
            SerializedProperty eur = origin.FindPropertyRelative("Euler");
            SerializedProperty sca = origin.FindPropertyRelative("Scale");
            SerializedProperty size = origin.FindPropertyRelative("Size");
            SerializedProperty alp = origin.FindPropertyRelative("Alpha");
            SerializedProperty col = origin.FindPropertyRelative("Color");
            SerializedProperty fil = origin.FindPropertyRelative("Fill");

            RectTransform rect = (RectTransform)m_rect.objectReferenceValue;

            rect.anchoredPosition3D = pos.vector3Value;
            rect.localEulerAngles = eur.vector3Value;
            rect.localScale = sca.vector3Value;
            rect.sizeDelta = size.vector2Value;
            m_rect.serializedObject.ApplyModifiedProperties();

            ((CanvasGroup)m_cav.objectReferenceValue).alpha = alp.floatValue;
            m_cav.serializedObject.ApplyModifiedProperties();

            if (m_img.objectReferenceValue != null)
            {
                Image img = (Image)m_img.objectReferenceValue;
                img.color = col.colorValue;
                img.fillAmount = fil.floatValue;
                m_img.serializedObject.ApplyModifiedProperties();
            }
            if (m_text.objectReferenceValue != null)
            {
                ((Hud_Text)m_text.objectReferenceValue).color = col.colorValue;
                m_text.serializedObject.ApplyModifiedProperties();
            }
            if (m_tmptext.objectReferenceValue != null)
            {
                ((Hud_TmpText)m_tmptext.objectReferenceValue).color = col.colorValue;
                m_tmptext.serializedObject.ApplyModifiedProperties();
            }

            so.ApplyModifiedProperties();
        }

        private void ClearEmptys()
        {
            for (int i = 0; i < BaseScript.TmpTextNodes.Count; i++)
            {
                // 检查是否为 null 或类型不匹配
                if (BaseScript.TmpTextNodes[i].TmpText == null)
                {
                    TmpTextNodes.DeleteArrayElementAtIndex(i);
                }
            }
            TmpTextNodes.serializedObject.ApplyModifiedProperties();

            for (int i = 0; i < BaseScript.TextNodes.Count; i++)
            {
                // 检查是否为 null 或类型不匹配
                if (BaseScript.TextNodes[i].Text == null)
                {
                    TextNodes.DeleteArrayElementAtIndex(i);
                }
            }
            TextNodes.serializedObject.ApplyModifiedProperties();
        }

        public ElementStatu GetPrefabStatus(GameObject gameObject)
        {
            if (PrefabUtility.IsPartOfPrefabAsset(gameObject))
            {
                return ElementStatu.InProject;
            }
            else if (PrefabUtility.IsPartOfPrefabInstance(gameObject))
            {
                return ElementStatu.InScene;
            }
            else
            {
                return ElementStatu.InSceneNotPrefab;
            }
        }
        #endregion

        #region 预览

        #region Animator 动画预览
        /// <summary>
        /// 预览动画
        /// </summary>
        private void Preview_Animator_Play()
        {
            Hud_Manager mgr = util_Dashboard.HudManagerGet();

            Preivew_Animator_PlayingState = true;
            AnimateState.enumValueIndex = 1;
            AnimateState.serializedObject.ApplyModifiedProperties();

            ///---用来存放调用动画器的动画效果的延迟时间
            List<float> delaytimes = new List<float>();

            ///---动画逻辑
            for (int i = 0; i < BaseScript.AnimatorNodes.Count; i++)
            {
                AnimatorNode node = BaseScript.AnimatorNodes[i];
                ///---判断动画器是否为有效
                if (node.Animator.AnimateTweenNodes == null || node.Animator.AnimateTweenNodes.Count <= 0)
                    continue;

                ///---循环第 i 个动画器节点的第 s 个动画效果是否开启
                for (int s = 0; s < node.Animator.AnimateTweenNodes.Count; s++)
                {
                    TweenNode twnnode = node.Animator.AnimateTweenNodes[s];
                    ///---如果动画效果未开启则跳过
                    if (twnnode.Enabled)
                    {
                        if (twnnode.ActivateOnlyToEnd)
                            continue;

                        if (mgr == null)
                            Preivew_Animator_TweenList.Add(node.Animator.Tweener_Play(twnnode, 1 * Element_Animators_GlobalDuration.floatValue * node.Animator.Animator_GlobalDuration));
                        else
                            Preivew_Animator_TweenList.Add(node.Animator.Tweener_Play(twnnode, mgr.DurationMultiply * Element_Animators_GlobalDuration.floatValue * node.Animator.Animator_GlobalDuration));
                        delaytimes.Add(node.DelayTime);
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            ///---预备动画器
            for (int i = 0; i < BaseScript.AnimatorNodes.Count; i++)
            {
                AnimatorNode node = BaseScript.AnimatorNodes[i];
                ///---判断动画器是否为有效
                if (node.Animator.AnimateTweenNodes == null || node.Animator.AnimateTweenNodes.Count <= 0)
                    continue;
                node.Animator.RewindAllTweenNode();
            }

            //同步启动所有Animator的所有音效播放
            Preivew_AnimatorSound_Coroutine_Play = EditorCoroutineUtility.StartCoroutineOwnerless(Preview_AnimatorSound_Play());

            ///---播放预览动画
            if (Preivew_Animator_TweenList != null && Preivew_Animator_TweenList.Count > 0)
            {
                for (int i = 0; i < Preivew_Animator_TweenList.Count; i++)
                {
                    Preivew_Animator_CoroutineList_Play.Add(EditorCoroutineUtility.StartCoroutineOwnerless(Preview_Animator_Coroutine_Play(Preivew_Animator_TweenList[i], delaytimes[i])));
                }
                if (!Animators_HasLoopMode())
                {
                    if (AutoStopPreview.boolValue)
                        Preivew_Animator_Coroutine_Stop = EditorCoroutineUtility.StartCoroutine(Preview_Animator_Coroutine_Stop(AnimatorsMaxDuration.floatValue), this);
                }
                DOTweenEditorPreview.Start();
            }
        }

        /// <summary>
        /// 延迟预览指定动画
        /// </summary>
        IEnumerator Preview_Animator_Coroutine_Play(Tweener tween, float delay)
        {
            yield return new EditorWaitForSeconds(delay);
            DOTweenEditorPreview.PrepareTweenForPreview(tween, true, true, true);
        }

        /// <summary>
        /// 预览指定Animator的动画
        /// </summary>
        private void Preview_Animator_PlayAt(int index)
        {
            Hud_Manager mgr = util_Dashboard.HudManagerGet();

            Preview_Animator_Stop();

            AnimatorNode node = BaseScript.AnimatorNodes[index];
            Hud_Animator anim = node.Animator;

            if (anim == null)
                return;

            if (anim.TweenNode_GetCount() <= 0)
                return;

            Preivew_Animator_PlayingState = true;
            AnimateState.enumValueIndex = 1;
            AnimateState.serializedObject.ApplyModifiedProperties();

            ///---用来存放调用动画器的动画效果的延迟时间
            float delaytime = 0;

            if (anim.AnimateTweenNodes == null || anim.AnimateTweenNodes.Count <= 0)
                return;

            for (int i = 0; i < anim.AnimateTweenNodes.Count; i++)
            {
                if (anim.AnimateTweenNodes[i].Enabled)
                {
                    TweenNode twnnode = anim.AnimateTweenNodes[i];
                    if (twnnode.ActivateOnlyToEnd)
                        continue;

                    Preivew_Animator_TweenList.Add(anim.Tweener_Play(twnnode, mgr.DurationMultiply * anim.Animator_GlobalDuration));
                    delaytime = node.DelayTime;

                    //播放动画节点包含的音效
                    for (int k = 0; k < twnnode.TweenSounds.Count; k++)
                    {
                        TweenSound sod = twnnode.TweenSounds[k];
                        #region 播放音效
                        AudioClip x_clip = sod.Sound;
                        //此处考虑到延迟计算 / 音效占动画耗时的百分比
                        float delay = sod.Percentage * ((twnnode.Duration * anim.Animator_GlobalDuration)) + twnnode.Delay + node.DelayTime;

                        Preivew_AnimatorSound_CoroutineList_Stop.Add(EditorCoroutineUtility.StartCoroutineOwnerless(Preview_AnimatorSound_Play_At(x_clip, sod.Volume, sod.MinPitch, sod.MaxPitch, anim.MutePlay, delay)));
                        #endregion
                    }
                }
                else
                {
                    continue;
                }
            }

            if (Preivew_Animator_TweenList != null && Preivew_Animator_TweenList.Count > 0)
            {
                for (int i = 0; i < Preivew_Animator_TweenList.Count; i++)
                {
                    Preivew_Animator_CoroutineList_Play.Add(EditorCoroutineUtility.StartCoroutineOwnerless(Preview_Animator_Coroutine_Play(Preivew_Animator_TweenList[i], delaytime)));
                }
                if (!AnimatorTweenNodes_HasLoopMode(anim.AnimateTweenNodes))
                {
                    if (AutoStopPreview.boolValue)
                        Preivew_Animator_Coroutine_Stop = EditorCoroutineUtility.StartCoroutine(Preview_Animator_Coroutine_Stop(anim.MaxTimerWithGlobalDuration + delaytime), this);
                }
                DOTweenEditorPreview.Start();
            }
        }

        /// <summary>
        /// 停止预览动画
        /// </summary>
        private void Preview_Animator_Stop()
        {
            Preivew_Animator_PlayingState = false;

            if (target != null)
            {
                AnimateState.enumValueIndex = 0;
                AnimateState.serializedObject.ApplyModifiedProperties();

                #region 杀死所有预览的节点的动画
                if (Preivew_Animator_TweenList != null && Preivew_Animator_TweenList.Count > 0)
                {
                    for (int i = 0; i < Preivew_Animator_TweenList.Count; i++)
                    {
                        if (Preivew_Animator_TweenList[i] != null)
                        {
                            Preivew_Animator_TweenList[i].Complete();
                            Preivew_Animator_TweenList[i].Kill();
                            Preivew_Animator_TweenList[i].Rewind();
                        }
                    }
                }
                #endregion

                #region 复位所有节点的动画
                if (BaseScript.AnimatorNodes != null && BaseScript.AnimatorNodes.Count > 0)
                {
                    for (int c = 0; c < BaseScript.AnimatorNodes.Count; c++)
                    {
                        Hud_Animator animator = BaseScript.AnimatorNodes[c].Animator;
                        List<TweenNode> TwnNodes = animator.AnimateTweenNodes;

                        if (TwnNodes != null && TwnNodes.Count > 0)
                        {
                            for (int i = 0; i < TwnNodes.Count; i++)
                            {
                                TweenNode twnnode = TwnNodes[i];
                                if (!twnnode.Enabled)
                                    continue;
                                else
                                {
                                    if (twnnode.ActivateOnlyToEnd)
                                        continue;
                                    animator.Tweener_Rewind(twnnode);
                                }
                            }
                        }

                        ///---还原姿态
                        OriginalPoseState_Load(animator);
                    }
                }
                #endregion

                #region 清空 Animator 动画预览列表
                Preivew_Animator_TweenList.Clear();
                for (int i = 0; i < Preivew_Animator_CoroutineList_Play.Count; i++)
                {
                    EditorCoroutineUtility.StopCoroutine(Preivew_Animator_CoroutineList_Play[i]);
                }
                Preivew_Animator_CoroutineList_Play.Clear();
                #endregion

                #region 停止协程 - Animator 动画停止预览
                if (Preivew_Animator_Coroutine_Stop != null)
                {
                    EditorCoroutineUtility.StopCoroutine(Preivew_Animator_Coroutine_Stop);
                    Preivew_Animator_Coroutine_Stop = null;
                }
                #endregion

                #region 停止协程 - AnimatorSound 音效预览播放
                if (Preivew_AnimatorSound_Coroutine_Play != null)
                {
                    EditorCoroutineUtility.StopCoroutine(Preivew_AnimatorSound_Coroutine_Play);
                    Preivew_AnimatorSound_Coroutine_Play = null;
                }
                #endregion

                #region 停止播放并清空 AnimatorSound 预览列表与生成的音效物体
                if (Preivew_AnimatorSound_SoundList != null)
                {
                    for (int i = 0; i < Preivew_AnimatorSound_SoundList.Count; i++)
                    {
                        if (Preivew_AnimatorSound_SoundList[i] != null)
                        {
                            //因为考虑到音效长度如果大于动画长度，那么得由生成的音效自己决定音效播放完后销毁的动作，否会出现动画放完而音效未放完被强行终止而销毁的BUG，所以这里只要清空音效预览列表即可，但如果你需要在动画放完时音效也都跟着一起销毁就取消注释下面得代码块

                            //Preivew_AnimatorSound_SoundList[i].Stop();
                            //DestroyImmediate(Preivew_AnimatorSound_SoundList[i].gameObject, true);
                            Preivew_AnimatorSound_SoundList[i] = null;
                        }
                    }
                    Preivew_AnimatorSound_SoundList.Clear();
                }
                #endregion

                #region 停止播放并清空 HudSounder 预览列表与生成的音效物体
                if (Preivew_HudSounder_SoundList != null)
                {
                    for (int i = 0; i < Preivew_HudSounder_SoundList.Count; i++)
                    {
                        if (Preivew_HudSounder_SoundList[i] != null)
                        {
                            //因为考虑到音效长度如果大于动画长度，那么得由生成的音效自己决定音效播放完后销毁的动作，否会出现动画放完而音效未放完被强行终止而销毁的BUG，所以这里只要清空音效预览列表即可，但如果你需要在动画放完时音效也都跟着一起销毁就取消注释下面得代码块

                            //Preivew_HudSounder_SoundList[i].Stop();
                            //DestroyImmediate(Preivew_HudSounder_SoundList[i].gameObject, true);
                            Preivew_HudSounder_SoundList[i] = null;
                        }
                    }
                    Preivew_HudSounder_SoundList.Clear();
                }
                #endregion
            }
            DOTweenEditorPreview.Stop();
        }

        /// <summary>
        /// 延迟停止
        /// </summary>
        IEnumerator Preview_Animator_Coroutine_Stop(float stopdelay)
        {
            Hud_Manager mgr = util_Dashboard.HudManagerGet();

            yield return new EditorWaitForSeconds(stopdelay * mgr.DurationMultiply);
            if (!Application.isPlaying)
            {
                Preview_Animator_Stop();
                Repaint();
            }
        }
        #endregion

        #region Animator 音效预览
        /// <summary>
        /// Animator 音效预览
        /// </summary>
        /// <returns></returns>
        IEnumerator Preview_AnimatorSound_Play()
        {
            while (BaseScript.AnimateState == HudElementAnimateState.Animating)
            {
                for (int w = 0; w < BaseScript.AnimatorNodes.Count; w++)
                {
                    AnimatorNode a_node = BaseScript.AnimatorNodes[w];
                    for (int i = 0; i < a_node.Animator.AnimateTweenNodes.Count; i++)
                    {
                        TweenNode tweenNode = a_node.Animator.AnimateTweenNodes[i];
                        Tweener tweener = tweenNode.Tweener;

                        if (tweener != null && tweener.IsActive() && tweener.IsPlaying())
                        {
                            if (tweenNode.Progress > 0.985f)
                                tweenNode.Progress = 1;
                            else
                                tweenNode.Progress = tweener.Elapsed(true) / tweener.Duration(true);
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
                                        Preivew_AnimatorSound_SoundList.Add(Preview_AnimatorSound_Creator(clip, vol, pitch_min, pitch_max, a_node.Animator.MutePlay));
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
                                        Preivew_AnimatorSound_SoundList.Add(Preview_AnimatorSound_Creator(clip, vol, pitch_min, pitch_max, a_node.Animator.MutePlay));
                                    }
                                }
                                else
                                {
                                    tweenSound.IsPlayed = false;
                                }
                            }
                        }
                    }
                }
                Repaint();
                yield return null;
            }
        }

        /// <summary>
        /// 创建 Animator 预览声音
        /// </summary>
        /// <param name="clip"></param>
        public AudioSource Preview_AnimatorSound_Creator(AudioClip clip, float vol, float pitch_min, float pitch_max, bool ismute)
        {
            Hud_Manager mgr = util_Dashboard.HudManagerGet();

            if (ismute)
                return null;
            GameObject obj = new GameObject();
            obj.name = "AnimatorSound_Previewer-" + "[" + clip.length.ToString("F2") + " s]-" + "[" + clip.channels + " ch]-" + "[" + clip.frequency + " hz]";
            AudioSource au = obj.AddComponent<AudioSource>();
            au.volume = mgr.Volume * 0.01f * vol;
            au.mute = mgr.VolumeMute;
            au.pitch = Random.Range(pitch_min, pitch_max);
            au.clip = clip;
            au.Play();
            util_AudioStoper sp = au.gameObject.AddComponent<util_AudioStoper>();
            sp.SetAudioSource(au);
            return au;
        }

        /// <summary>
        /// 创建 Animator 预览指定声音协程
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="sp_vol"></param>
        /// <param name="sp_pitch_min"></param>
        /// <param name="sp_pitch_max"></param>
        /// <param name="sp_ismute"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        IEnumerator Preview_AnimatorSound_Play_At(AudioClip clip, float sp_vol, float sp_pitch_min, float sp_pitch_max, bool sp_ismute, float delay)
        {
            yield return new EditorWaitForSeconds(delay);
            Preivew_AnimatorSound_SoundList.Add(Preview_AnimatorSound_Creator(clip, sp_vol, sp_pitch_min, sp_pitch_max, sp_ismute));
            AudioSource au = Preivew_AnimatorSound_SoundList[Preivew_AnimatorSound_SoundList.Count - 1];
            while (true)
            {
                if (au != null && !au.isPlaying)
                {
                    break;
                }
                yield return null;
            }
            DestroyImmediate(au.gameObject, true);
        }

        /// <summary>
        ///  停止协程列表 - AnimatorSound 音效预览播放 / 停止播放并清空 AnimatorSound 预览列表与生成的音效物体
        /// </summary>
        private void Preview_AnimatorSound_CoroutineList_Stop()
        {
            for (int i = 0; i < Preivew_AnimatorSound_CoroutineList_Stop.Count; i++)
            {
                if (Preivew_AnimatorSound_CoroutineList_Stop[i] != null)
                    EditorCoroutineUtility.StopCoroutine(Preivew_AnimatorSound_CoroutineList_Stop[i]);
            }
            Preivew_AnimatorSound_CoroutineList_Stop.Clear();

            if (Preivew_AnimatorSound_SoundList != null)
            {
                for (int i = 0; i < Preivew_AnimatorSound_SoundList.Count; i++)
                {
                    if (Preivew_AnimatorSound_SoundList[i] != null)
                    {
                        Preivew_AnimatorSound_SoundList[i].Stop();
                        DestroyImmediate(Preivew_AnimatorSound_SoundList[i].gameObject, true);
                        Preivew_AnimatorSound_SoundList[i] = null;
                    }
                }
                Preivew_AnimatorSound_SoundList.Clear();
            }

            SceneView.RepaintAll();
        }
        #endregion

        #region HudSounder 音效器 音效预览

        /// <summary>
        ///  HudSounder 音效预览
        /// </summary>
        IEnumerator Preview_HudSounder_Play(float sp_vol, float sp_pitch_min, float sp_pitch_max, bool sp_userandom, AudioClip clip, float delay)
        {
            yield return new EditorWaitForSeconds(delay);
            Preivew_HudSounder_SoundList.Add(Preview_HudSounder_CreateSound(sp_vol, sp_pitch_min, sp_pitch_max, sp_userandom, clip));
            AudioSource au = Preivew_HudSounder_SoundList[Preivew_HudSounder_SoundList.Count - 1];
            while (true)
            {
                if (au != null && !au.isPlaying)
                {
                    break;
                }
                yield return null;
            }
            DestroyImmediate(au.gameObject, true);
        }

        /// <summary>
        /// 创建 HudSounder 预览指定声音
        /// </summary>
        /// <param name="sp_vol"></param>
        /// <param name="sp_pitch_min"></param>
        /// <param name="sp_pitch_max"></param>
        /// <param name="sp_userandom"></param>
        /// <param name="clip"></param>
        /// <returns></returns>
        public AudioSource Preview_HudSounder_CreateSound(float sp_vol, float sp_pitch_min, float sp_pitch_max, bool sp_userandom, AudioClip clip)
        {
            GameObject obj = new GameObject();
            obj.name = "HudSound_Previewer-" + "[" + clip.length.ToString("F2") + " s]-" + "[" + clip.channels + " ch]-" + "[" + clip.frequency + " hz]";
            AudioSource au = obj.AddComponent<AudioSource>();
            au.clip = clip;
            au.volume = sp_vol;
            if (sp_userandom)
            {
                au.pitch = Random.Range(sp_pitch_min, sp_pitch_max);
            }
            else
            {
                au.pitch = 1.0f;
            }
            au.Play();
            util_AudioStoper sp = au.gameObject.AddComponent<util_AudioStoper>();
            sp.SetAudioSource(au);
            return au;
        }

        /// <summary>
        ///  停止协程列表 - HudSounder 音效预览播放 / 停止播放并清空 HudSounder 预览列表与生成的音效物体
        /// </summary>
        private void Preview_HudSounder_Coroutine_Stop()
        {
            for (int i = 0; i < Preivew_HudSounder_CoroutineList_Stop.Count; i++)
            {
                if (Preivew_HudSounder_CoroutineList_Stop[i] != null)
                    EditorCoroutineUtility.StopCoroutine(Preivew_HudSounder_CoroutineList_Stop[i]);
            }
            Preivew_HudSounder_CoroutineList_Stop.Clear();

            if (Preivew_HudSounder_SoundList != null)
            {
                for (int i = 0; i < Preivew_HudSounder_SoundList.Count; i++)
                {
                    if (Preivew_HudSounder_SoundList[i] != null)
                    {
                        Preivew_HudSounder_SoundList[i].Stop();
                        DestroyImmediate(Preivew_HudSounder_SoundList[i].gameObject, true);
                        Preivew_HudSounder_SoundList[i] = null;
                    }
                }
                Preivew_HudSounder_SoundList.Clear();
            }

            SceneView.RepaintAll();
        }

        #endregion

        #endregion

        #region 获取组件

        /// <summary>
        /// 获取所有动画器
        /// </summary>
        private void GetAllAnimators()
        {
            if (IsMultiSelected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    SerializedObject so_ele = new SerializedObject(SelectedObjects[i]);
                    SerializedProperty sp_nodes = so_ele.FindProperty("AnimatorNodes");
                    so_ele.Update();
                    sp_nodes.ClearArray();
                    #region 获取所有Animator并过滤条件
                    Hud_Animator[] animators = SelectedObjects[i].GetComponentsInChildren<Hud_Animator>();

                    List<Hud_Animator> animators_fillter = new List<Hud_Animator>();
                    for (int g = 0; g < animators.Length; g++)
                    {
                        Hud_Button hud_Button = animators[g].GetComponentInParent<Hud_Button>();
                        Hud_Progress hud_Progress = animators[g].GetComponentInParent<Hud_Progress>();
                        Hud_Slider hud_Slider = animators[g].GetComponentInParent<Hud_Slider>();
                        Hud_Option hud_optselector = animators[g].GetComponentInParent<Hud_Option>();
                        Hud_Toggle hud_tog = animators[g].GetComponentInParent<Hud_Toggle>();
                        Hud_Container hud_dat = animators[g].GetComponentInParent<Hud_Container>();
                        if (hud_Button != null)
                            continue;
                        if (hud_Progress != null)
                            continue;
                        if (hud_Slider != null)
                            continue;
                        if (hud_optselector != null)
                            continue;
                        if (hud_tog != null)
                            continue;
                        if (hud_dat != null)
                            continue;
                        if (animators[g].IgnoreElementAnimationPlay)
                            continue;
                        animators_fillter.Add(animators[g]);
                    }

                    #endregion

                    Hud_Animator[] animators_confirm = animators_fillter.ToArray();
                    for (int m = 0; m < animators_confirm.Length; m++)
                    {
                        #region 判断是否已存在Animator
                        bool isrepeat = false;

                        for (int s = 0; s < sp_nodes.arraySize; s++)
                        {
                            SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                            SerializedProperty sp_node_anim = sp_node.FindPropertyRelative("Animator");
                            Hud_Animator sp_anim = (Hud_Animator)sp_node_anim.objectReferenceValue;
                            if (animators_confirm[m] == sp_anim)
                            {
                                isrepeat = true;
                            }
                        }

                        if (!isrepeat)
                        {
                            int index = 0;

                            if (sp_nodes.arraySize <= 0)
                                index = 0;
                            else
                                index = sp_nodes.arraySize;

                            sp_nodes.InsertArrayElementAtIndex(index);
                            SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(index);

                            #region 赋值Animator
                            SerializedProperty sp_Animator = sp_node.FindPropertyRelative("Animator");
                            sp_Animator.objectReferenceValue = animators_confirm[m];
                            sp_Animator.serializedObject.ApplyModifiedProperties();
                            #endregion

                            sp_node.serializedObject.ApplyModifiedProperties();
                        }
                        #endregion
                    }

                    sp_nodes.serializedObject.ApplyModifiedProperties();
                    so_ele.ApplyModifiedProperties();
                }
            }
            else
            {
                SerializedProperty sp_nodes = serializedObject.FindProperty("AnimatorNodes");
                sp_nodes.ClearArray();
                #region 获取所有Animator并过滤条件
                Hud_Animator[] animators = BaseScript.GetComponentsInChildren<Hud_Animator>();

                List<Hud_Animator> animators_fillter = new List<Hud_Animator>();
                for (int i = 0; i < animators.Length; i++)
                {
                    Hud_Button hud_Button = animators[i].GetComponentInParent<Hud_Button>();
                    Hud_Progress hud_Progress = animators[i].GetComponentInParent<Hud_Progress>();
                    Hud_Slider hud_Slider = animators[i].GetComponentInParent<Hud_Slider>();
                    Hud_Option hud_optselector = animators[i].GetComponentInParent<Hud_Option>();
                    Hud_Toggle hud_tog = animators[i].GetComponentInParent<Hud_Toggle>();
                    Hud_Container hud_dat = animators[i].GetComponentInParent<Hud_Container>();
                    if (hud_Button != null)
                        continue;
                    if (hud_Progress != null)
                        continue;
                    if (hud_Slider != null)
                        continue;
                    if (hud_optselector != null)
                        continue;
                    if (hud_tog != null)
                        continue;
                    if (hud_dat != null)
                        continue;
                    if (animators[i].IgnoreElementAnimationPlay)
                        continue;
                    animators_fillter.Add(animators[i]);
                }

                #endregion

                Hud_Animator[] animators_confirm = animators_fillter.ToArray();
                for (int i = 0; i < animators_confirm.Length; i++)
                {
                    #region 判断是否已存在Animator
                    bool isrepeat = false;

                    for (int s = 0; s < sp_nodes.arraySize; s++)
                    {
                        SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                        SerializedProperty sp_node_anim = sp_node.FindPropertyRelative("Animator");
                        Hud_Animator sp_anim = (Hud_Animator)sp_node_anim.objectReferenceValue;
                        if (animators_confirm[i] == sp_anim)
                        {
                            isrepeat = true;
                        }
                    }

                    if (!isrepeat)
                    {
                        int index = 0;

                        if (sp_nodes.arraySize <= 0)
                            index = 0;
                        else
                            index = sp_nodes.arraySize;

                        sp_nodes.InsertArrayElementAtIndex(index);
                        SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(index);

                        #region 赋值Animator
                        SerializedProperty sp_Animator = sp_node.FindPropertyRelative("Animator");
                        sp_Animator.objectReferenceValue = animators_confirm[i];
                        sp_Animator.serializedObject.ApplyModifiedProperties();
                        #endregion

                        sp_node.serializedObject.ApplyModifiedProperties();
                    }
                    #endregion
                }
                sp_nodes.serializedObject.ApplyModifiedProperties();
            }
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 获取所有文字
        /// </summary>
        private void GetAllText()
        {
            if (IsMultiSelected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    SerializedObject so_ele = new SerializedObject(SelectedObjects[i]);
                    SerializedProperty sp_nodes = so_ele.FindProperty("TextNodes");
                    so_ele.Update();

                    #region 获取所有Animator并过滤条件
                    Hud_Text[] texts = SelectedObjects[i].GetComponentsInChildren<Hud_Text>();

                    List<Hud_Text> texts_fillter = new List<Hud_Text>();
                    for (int g = 0; g < texts.Length; g++)
                    {
                        Hud_Button hud_Button = texts[g].GetComponentInParent<Hud_Button>();
                        Hud_Progress hud_Progress = texts[g].GetComponentInParent<Hud_Progress>();
                        Hud_Slider hud_Slider = texts[g].GetComponentInParent<Hud_Slider>();
                        Hud_Option hud_optselector = texts[g].GetComponentInParent<Hud_Option>();
                        Hud_Toggle hud_tog = texts[g].GetComponentInParent<Hud_Toggle>();
                        Hud_Container hud_dat = texts[g].GetComponentInParent<Hud_Container>();
                        if (hud_Button != null)
                            continue;
                        if (hud_Progress != null)
                            continue;
                        if (hud_Slider != null)
                            continue;
                        if (hud_optselector != null)
                            continue;
                        if (hud_tog != null)
                            continue;
                        if (hud_dat != null)
                            continue;
                        texts_fillter.Add(texts[g]);
                    }

                    #endregion

                    Hud_Text[] texts_confirm = texts_fillter.ToArray();
                    for (int m = 0; m < texts_confirm.Length; m++)
                    {
                        #region 判断是否已存在Text
                        bool isrepeat = false;

                        for (int s = 0; s < sp_nodes.arraySize; s++)
                        {
                            SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                            SerializedProperty sp_node_text = sp_node.FindPropertyRelative("Text");
                            Hud_Text sp_text = (Hud_Text)sp_node_text.objectReferenceValue;
                            if (texts_confirm[m] == sp_text)
                            {
                                isrepeat = true;
                            }
                        }

                        if (!isrepeat)
                        {
                            int index = 0;

                            if (sp_nodes.arraySize <= 0)
                                index = 0;
                            else
                                index = sp_nodes.arraySize;

                            sp_nodes.InsertArrayElementAtIndex(index);
                            SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(index);

                            #region 赋值Animator
                            SerializedProperty sp_Text = sp_node.FindPropertyRelative("Text");
                            sp_Text.objectReferenceValue = texts_confirm[m];
                            sp_Text.serializedObject.ApplyModifiedProperties();
                            #endregion

                            sp_node.serializedObject.ApplyModifiedProperties();
                        }
                        #endregion
                    }

                    sp_nodes.serializedObject.ApplyModifiedProperties();
                    so_ele.ApplyModifiedProperties();
                }
            }
            else
            {
                SerializedProperty sp_nodes = serializedObject.FindProperty("TextNodes");

                #region 获取所有Animator并过滤条件
                Hud_Text[] texts = BaseScript.GetComponentsInChildren<Hud_Text>();

                List<Hud_Text> texts_fillter = new List<Hud_Text>();
                for (int i = 0; i < texts.Length; i++)
                {
                    Hud_Button hud_Button = texts[i].GetComponentInParent<Hud_Button>();
                    Hud_Progress hud_Progress = texts[i].GetComponentInParent<Hud_Progress>();
                    Hud_Slider hud_Slider = texts[i].GetComponentInParent<Hud_Slider>();
                    Hud_Option hud_optselector = texts[i].GetComponentInParent<Hud_Option>();
                    Hud_Toggle hud_tog = texts[i].GetComponentInParent<Hud_Toggle>();
                    Hud_Container hud_dat = texts[i].GetComponentInParent<Hud_Container>();
                    if (hud_Button != null)
                        continue;
                    if (hud_Progress != null)
                        continue;
                    if (hud_Slider != null)
                        continue;
                    if (hud_optselector != null)
                        continue;
                    if (hud_tog != null)
                        continue;
                    if (hud_dat != null)
                        continue;
                    texts_fillter.Add(texts[i]);
                }

                #endregion

                Hud_Text[] texts_confirm = texts_fillter.ToArray();
                for (int i = 0; i < texts_confirm.Length; i++)
                {
                    #region 判断是否已存在Text
                    bool isrepeat = false;

                    for (int s = 0; s < sp_nodes.arraySize; s++)
                    {
                        SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                        SerializedProperty sp_node_text = sp_node.FindPropertyRelative("Text");
                        Hud_Text sp_text = (Hud_Text)sp_node_text.objectReferenceValue;
                        if (texts_confirm[i] == sp_text)
                        {
                            isrepeat = true;
                        }
                    }

                    if (!isrepeat)
                    {
                        int index = 0;

                        if (sp_nodes.arraySize <= 0)
                            index = 0;
                        else
                            index = sp_nodes.arraySize;

                        sp_nodes.InsertArrayElementAtIndex(index);
                        SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(index);

                        #region 赋值Text
                        SerializedProperty sp_text = sp_node.FindPropertyRelative("Text");
                        sp_text.objectReferenceValue = texts_confirm[i];
                        sp_text.serializedObject.ApplyModifiedProperties();
                        #endregion

                        sp_node.serializedObject.ApplyModifiedProperties();
                    }
                    #endregion
                }
                sp_nodes.serializedObject.ApplyModifiedProperties();
            }
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 获取所有Tmp文字
        /// </summary>
        private void GetAllTmpText()
        {
            if (IsMultiSelected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    SerializedObject so_ele = new SerializedObject(SelectedObjects[i]);
                    SerializedProperty sp_nodes = so_ele.FindProperty("TmpTextNodes");
                    so_ele.Update();

                    #region 获取所有Animator并过滤条件
                    Hud_TmpText[] texts = SelectedObjects[i].GetComponentsInChildren<Hud_TmpText>();

                    List<Hud_TmpText> texts_fillter = new List<Hud_TmpText>();
                    for (int g = 0; g < texts.Length; g++)
                    {
                        Hud_Button hud_Button = texts[g].GetComponentInParent<Hud_Button>();
                        Hud_Progress hud_Progress = texts[g].GetComponentInParent<Hud_Progress>();
                        Hud_Slider hud_Slider = texts[g].GetComponentInParent<Hud_Slider>();
                        Hud_Option hud_optselector = texts[g].GetComponentInParent<Hud_Option>();
                        Hud_Toggle hud_tog = texts[g].GetComponentInParent<Hud_Toggle>();
                        Hud_Container hud_dat = texts[g].GetComponentInParent<Hud_Container>();
                        if (hud_Button != null)
                            continue;
                        if (hud_Progress != null)
                            continue;
                        if (hud_Slider != null)
                            continue;
                        if (hud_optselector != null)
                            continue;
                        if (hud_tog != null)
                            continue;
                        if (hud_dat != null)
                            continue;
                        texts_fillter.Add(texts[g]);
                    }

                    #endregion

                    Hud_TmpText[] texts_confirm = texts_fillter.ToArray();
                    for (int m = 0; m < texts_confirm.Length; m++)
                    {
                        #region 判断是否已存在Text
                        bool isrepeat = false;

                        for (int s = 0; s < sp_nodes.arraySize; s++)
                        {
                            SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                            SerializedProperty sp_node_text = sp_node.FindPropertyRelative("TmpText");
                            Hud_TmpText sp_text = (Hud_TmpText)sp_node_text.objectReferenceValue;
                            if (texts_confirm[m] == sp_text)
                            {
                                isrepeat = true;
                            }
                        }

                        if (!isrepeat)
                        {
                            int index = 0;

                            if (sp_nodes.arraySize <= 0)
                                index = 0;
                            else
                                index = sp_nodes.arraySize;

                            sp_nodes.InsertArrayElementAtIndex(index);
                            SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(index);

                            #region 赋值Animator
                            SerializedProperty sp_Text = sp_node.FindPropertyRelative("TmpText");
                            sp_Text.objectReferenceValue = texts_confirm[m];
                            sp_Text.serializedObject.ApplyModifiedProperties();
                            #endregion

                            sp_node.serializedObject.ApplyModifiedProperties();
                        }
                        #endregion
                    }

                    sp_nodes.serializedObject.ApplyModifiedProperties();
                    so_ele.ApplyModifiedProperties();
                }
            }
            else
            {
                SerializedProperty sp_nodes = serializedObject.FindProperty("TmpTextNodes");

                #region 获取所有Animator并过滤条件
                Hud_TmpText[] texts = BaseScript.GetComponentsInChildren<Hud_TmpText>();

                List<Hud_TmpText> texts_fillter = new List<Hud_TmpText>();
                for (int i = 0; i < texts.Length; i++)
                {
                    Hud_Button hud_Button = texts[i].GetComponentInParent<Hud_Button>();
                    Hud_Progress hud_Progress = texts[i].GetComponentInParent<Hud_Progress>();
                    Hud_Slider hud_Slider = texts[i].GetComponentInParent<Hud_Slider>();
                    Hud_Option hud_optselector = texts[i].GetComponentInParent<Hud_Option>();
                    Hud_Toggle hud_tog = texts[i].GetComponentInParent<Hud_Toggle>();
                    Hud_Container hud_dat = texts[i].GetComponentInParent<Hud_Container>();
                    if (hud_Button != null)
                        continue;
                    if (hud_Progress != null)
                        continue;
                    if (hud_Slider != null)
                        continue;
                    if (hud_optselector != null)
                        continue;
                    if (hud_tog != null)
                        continue;
                    if (hud_dat != null)
                        continue;
                    texts_fillter.Add(texts[i]);
                }

                #endregion

                Hud_TmpText[] texts_confirm = texts_fillter.ToArray();
                for (int i = 0; i < texts_confirm.Length; i++)
                {
                    #region 判断是否已存在Text
                    bool isrepeat = false;

                    for (int s = 0; s < sp_nodes.arraySize; s++)
                    {
                        SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                        SerializedProperty sp_node_text = sp_node.FindPropertyRelative("TmpText");
                        Hud_TmpText sp_text = (Hud_TmpText)sp_node_text.objectReferenceValue;
                        if (texts_confirm[i] == sp_text)
                        {
                            isrepeat = true;
                        }
                    }

                    if (!isrepeat)
                    {
                        int index = 0;

                        if (sp_nodes.arraySize <= 0)
                            index = 0;
                        else
                            index = sp_nodes.arraySize;

                        sp_nodes.InsertArrayElementAtIndex(index);
                        SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(index);

                        #region 赋值Text
                        SerializedProperty sp_text = sp_node.FindPropertyRelative("TmpText");
                        sp_text.objectReferenceValue = texts_confirm[i];
                        sp_text.serializedObject.ApplyModifiedProperties();
                        #endregion

                        sp_node.serializedObject.ApplyModifiedProperties();
                    }
                    #endregion
                }
                sp_nodes.serializedObject.ApplyModifiedProperties();
            }
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 获取所有选项
        /// </summary>
        private void GetAllOptions()
        {
            if (IsMultiSelected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    SerializedObject so_ele = new SerializedObject(SelectedObjects[i]);
                    SerializedProperty sp_nodes = so_ele.FindProperty("OptionNodes");
                    so_ele.Update();

                    Hud_Option[] allopts = SelectedObjects[i].GetComponentsInChildren<Hud_Option>();

                    List<Hud_Option> Filter = new List<Hud_Option>();
                    for (int s = 0; s < allopts.Length; s++)
                    {
                        Filter.Add(allopts[s]);
                    }

                    Hud_Option[] gettedOpts = Filter.ToArray();

                    for (int x = 0; x < gettedOpts.Length; x++)
                    {
                        bool repeat = false;

                        if (sp_nodes.arraySize > 0)
                        {
                            for (int s = 0; s < sp_nodes.arraySize; s++)
                            {
                                SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                                SerializedProperty sp_node_opt = sp_node.FindPropertyRelative("Option");
                                Hud_Option sp_opt = (Hud_Option)sp_node_opt.objectReferenceValue;
                                if (sp_opt == gettedOpts[x])
                                    repeat = true;
                            }
                        }

                        if (!repeat)
                        {
                            int index = 0;

                            if (sp_nodes.arraySize <= 0)
                                index = 0;
                            else
                                index = sp_nodes.arraySize - 1;

                            sp_nodes.InsertArrayElementAtIndex(index);

                            SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(index);
                            SerializedProperty sp_opt = sp_node.FindPropertyRelative("Option");
                            sp_opt.objectReferenceValue = gettedOpts[x];

                            sp_opt.serializedObject.ApplyModifiedProperties();
                            sp_node.serializedObject.ApplyModifiedProperties();
                        }
                    }
                    sp_nodes.serializedObject.ApplyModifiedProperties();
                    so_ele.ApplyModifiedProperties();
                }
            }
            else
            {
                SerializedProperty sp_nodes = serializedObject.FindProperty("OptionNodes");

                Hud_Option[] allanimators = BaseScript.GetComponentsInChildren<Hud_Option>();

                List<Hud_Option> Filter = new List<Hud_Option>();
                for (int i = 0; i < allanimators.Length; i++)
                {
                    Filter.Add(allanimators[i]);
                }

                Hud_Option[] gettedOpts = Filter.ToArray();

                for (int i = 0; i < gettedOpts.Length; i++)
                {
                    bool repeat = false;

                    if (sp_nodes.arraySize > 0)
                    {
                        for (int s = 0; s < sp_nodes.arraySize; s++)
                        {
                            SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                            SerializedProperty sp_node_opt = sp_node.FindPropertyRelative("Option");
                            Hud_Option sp_opt = (Hud_Option)sp_node_opt.objectReferenceValue;
                            if (sp_opt == gettedOpts[i])
                                repeat = true;
                        }
                    }

                    if (!repeat)
                    {
                        int index = 0;

                        if (sp_nodes.arraySize <= 0)
                            index = 0;
                        else
                            index = sp_nodes.arraySize - 1;

                        sp_nodes.InsertArrayElementAtIndex(index);

                        SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(index);
                        SerializedProperty sp_Option = sp_node.FindPropertyRelative("Option");
                        sp_Option.objectReferenceValue = gettedOpts[i];

                        sp_Option.serializedObject.ApplyModifiedProperties();
                        sp_node.serializedObject.ApplyModifiedProperties();
                    }
                }
                sp_nodes.serializedObject.ApplyModifiedProperties();
            }
        }

        /// <summary>
        /// 获取所有滑动条
        /// </summary>
        private void GetAllSliders()
        {
            if (IsMultiSelected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    SerializedObject so_ele = new SerializedObject(SelectedObjects[i]);
                    SerializedProperty sp_nodes = so_ele.FindProperty("SliderNodes");
                    so_ele.Update();

                    Hud_Slider[] allSliders = SelectedObjects[i].GetComponentsInChildren<Hud_Slider>();

                    List<Hud_Slider> Filter = new List<Hud_Slider>();
                    for (int s = 0; s < allSliders.Length; s++)
                    {
                        Filter.Add(allSliders[s]);
                    }

                    Hud_Slider[] gettedSliders = Filter.ToArray();

                    for (int x = 0; x < gettedSliders.Length; x++)
                    {
                        bool repeat = false;

                        if (sp_nodes.arraySize > 0)
                        {
                            for (int s = 0; s < sp_nodes.arraySize; s++)
                            {
                                SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                                SerializedProperty sp_node_Slider = sp_node.FindPropertyRelative("Slider");
                                Hud_Slider sp_opt = (Hud_Slider)sp_node_Slider.objectReferenceValue;
                                if (sp_opt == gettedSliders[x])
                                    repeat = true;
                            }
                        }

                        if (!repeat)
                        {
                            int index = 0;

                            if (sp_nodes.arraySize <= 0)
                                index = 0;
                            else
                                index = sp_nodes.arraySize - 1;

                            sp_nodes.InsertArrayElementAtIndex(index);

                            SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(index);
                            SerializedProperty sp_Slider = sp_node.FindPropertyRelative("Slider");
                            sp_Slider.objectReferenceValue = gettedSliders[x];

                            sp_Slider.serializedObject.ApplyModifiedProperties();
                            sp_node.serializedObject.ApplyModifiedProperties();
                        }
                    }
                    sp_nodes.serializedObject.ApplyModifiedProperties();
                    so_ele.ApplyModifiedProperties();
                }
            }
            else
            {
                SerializedProperty sp_nodes = serializedObject.FindProperty("SliderNodes");

                Hud_Slider[] allslider = BaseScript.GetComponentsInChildren<Hud_Slider>();

                List<Hud_Slider> Filter = new List<Hud_Slider>();
                for (int i = 0; i < allslider.Length; i++)
                {
                    Filter.Add(allslider[i]);
                }

                Hud_Slider[] gettedSliders = Filter.ToArray();

                for (int i = 0; i < gettedSliders.Length; i++)
                {
                    bool repeat = false;

                    if (sp_nodes.arraySize > 0)
                    {
                        for (int s = 0; s < sp_nodes.arraySize; s++)
                        {
                            SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                            SerializedProperty sp_node_sli = sp_node.FindPropertyRelative("Slider");
                            Hud_Slider sp_opt = (Hud_Slider)sp_node_sli.objectReferenceValue;
                            if (sp_opt == gettedSliders[i])
                                repeat = true;
                        }
                    }

                    if (!repeat)
                    {
                        int index = 0;

                        if (sp_nodes.arraySize <= 0)
                            index = 0;
                        else
                            index = sp_nodes.arraySize - 1;

                        sp_nodes.InsertArrayElementAtIndex(index);

                        SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(index);
                        SerializedProperty sp_Slider = sp_node.FindPropertyRelative("Slider");
                        sp_Slider.objectReferenceValue = gettedSliders[i];

                        sp_Slider.serializedObject.ApplyModifiedProperties();
                        sp_node.serializedObject.ApplyModifiedProperties();
                    }
                }
                sp_nodes.serializedObject.ApplyModifiedProperties();
            }
        }

        /// <summary>
        /// 获取所有按钮
        /// </summary>
        private void GetAllButtons()
        {
            if (IsMultiSelected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    SerializedObject so_ele = new SerializedObject(SelectedObjects[i]);
                    SerializedProperty sp_nodes = so_ele.FindProperty("ButtonNodes");
                    so_ele.Update();

                    Hud_Button[] allbtns = SelectedObjects[i].GetComponentsInChildren<Hud_Button>();

                    List<Hud_Button> Filter = new List<Hud_Button>();
                    for (int s = 0; s < allbtns.Length; s++)
                    {
                        Hud_Option hud_optselector = allbtns[s].GetComponentInParent<Hud_Option>();
                        if (hud_optselector != null)
                            continue;
                        Filter.Add(allbtns[s]);
                    }

                    Hud_Button[] gettedBtns = Filter.ToArray();

                    for (int x = 0; x < gettedBtns.Length; x++)
                    {
                        bool repeat = false;

                        if (sp_nodes.arraySize > 0)
                        {
                            for (int s = 0; s < sp_nodes.arraySize; s++)
                            {
                                SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                                SerializedProperty sp_node_btn = sp_node.FindPropertyRelative("Button");
                                Hud_Button sp_btn = (Hud_Button)sp_node_btn.objectReferenceValue;
                                if (sp_btn == gettedBtns[x])
                                    repeat = true;
                            }
                        }

                        if (!repeat)
                        {
                            int index = 0;

                            if (sp_nodes.arraySize <= 0)
                                index = 0;
                            else
                                index = sp_nodes.arraySize - 1;

                            sp_nodes.InsertArrayElementAtIndex(index);

                            SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(index);
                            SerializedProperty sp_btn = sp_node.FindPropertyRelative("Button");
                            sp_btn.objectReferenceValue = gettedBtns[x];

                            sp_btn.serializedObject.ApplyModifiedProperties();
                            sp_node.serializedObject.ApplyModifiedProperties();
                        }
                    }
                    sp_nodes.serializedObject.ApplyModifiedProperties();
                    so_ele.ApplyModifiedProperties();
                }
            }
            else
            {
                SerializedProperty sp_nodes = serializedObject.FindProperty("ButtonNodes");

                Hud_Button[] allanimators = BaseScript.GetComponentsInChildren<Hud_Button>();

                List<Hud_Button> Filter = new List<Hud_Button>();
                for (int i = 0; i < allanimators.Length; i++)
                {
                    Hud_Option hud_optselector = allanimators[i].GetComponentInParent<Hud_Option>();
                    if (hud_optselector != null)
                        continue;
                    Filter.Add(allanimators[i]);
                }

                Hud_Button[] gettedBtns = Filter.ToArray();

                for (int i = 0; i < gettedBtns.Length; i++)
                {
                    bool repeat = false;

                    if (sp_nodes.arraySize > 0)
                    {
                        for (int s = 0; s < sp_nodes.arraySize; s++)
                        {
                            SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                            SerializedProperty sp_node_btn = sp_node.FindPropertyRelative("Button");
                            Hud_Button sp_btn = (Hud_Button)sp_node_btn.objectReferenceValue;
                            if (sp_btn == gettedBtns[i])
                                repeat = true;
                        }
                    }

                    if (!repeat)
                    {
                        int index = 0;

                        if (sp_nodes.arraySize <= 0)
                            index = 0;
                        else
                            index = sp_nodes.arraySize - 1;

                        sp_nodes.InsertArrayElementAtIndex(index);

                        SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(index);
                        SerializedProperty sp_Animator = sp_node.FindPropertyRelative("Button");
                        sp_Animator.objectReferenceValue = gettedBtns[i];

                        sp_Animator.serializedObject.ApplyModifiedProperties();
                        sp_node.serializedObject.ApplyModifiedProperties();
                    }
                }
                sp_nodes.serializedObject.ApplyModifiedProperties();
            }
        }

        /// <summary>
        /// 获取所有进度条
        /// </summary>
        private void GetAllProgress()
        {
            if (IsMultiSelected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    SerializedObject so_ele = new SerializedObject(SelectedObjects[i]);
                    SerializedProperty sp_nodes = so_ele.FindProperty("ProgressNodes");
                    so_ele.Update();

                    Hud_Progress[] allpros = SelectedObjects[i].GetComponentsInChildren<Hud_Progress>();

                    List<Hud_Progress> Filter = new List<Hud_Progress>();
                    for (int s = 0; s < allpros.Length; s++)
                    {
                        Filter.Add(allpros[s]);
                    }

                    Hud_Progress[] gettedPros = Filter.ToArray();

                    for (int x = 0; x < gettedPros.Length; x++)
                    {
                        bool repeat = false;

                        if (sp_nodes.arraySize > 0)
                        {
                            for (int s = 0; s < sp_nodes.arraySize; s++)
                            {
                                SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                                SerializedProperty sp_node_pro = sp_node.FindPropertyRelative("Progress");
                                Hud_Progress sp_pro = (Hud_Progress)sp_node_pro.objectReferenceValue;
                                if (sp_pro == gettedPros[x])
                                    repeat = true;
                            }
                        }

                        if (!repeat)
                        {
                            int index = 0;

                            if (sp_nodes.arraySize <= 0)
                                index = 0;
                            else
                                index = sp_nodes.arraySize - 1;

                            sp_nodes.InsertArrayElementAtIndex(index);

                            SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(index);
                            SerializedProperty sp_pro = sp_node.FindPropertyRelative("Progress");
                            sp_pro.objectReferenceValue = gettedPros[x];

                            sp_pro.serializedObject.ApplyModifiedProperties();
                            sp_node.serializedObject.ApplyModifiedProperties();
                        }
                    }
                    sp_nodes.serializedObject.ApplyModifiedProperties();
                    so_ele.ApplyModifiedProperties();
                }
            }
            else
            {
                SerializedProperty sp_nodes = serializedObject.FindProperty("ProgressNodes");

                Hud_Progress[] allpros = BaseScript.GetComponentsInChildren<Hud_Progress>();

                List<Hud_Progress> Filter = new List<Hud_Progress>();
                for (int i = 0; i < allpros.Length; i++)
                {
                    Filter.Add(allpros[i]);
                }

                Hud_Progress[] gettedPros = Filter.ToArray();

                for (int i = 0; i < gettedPros.Length; i++)
                {
                    bool repeat = false;

                    if (sp_nodes.arraySize > 0)
                    {
                        for (int s = 0; s < sp_nodes.arraySize; s++)
                        {
                            SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                            SerializedProperty sp_node_pro = sp_node.FindPropertyRelative("Progress");
                            Hud_Progress sp_pro = (Hud_Progress)sp_node_pro.objectReferenceValue;
                            if (sp_pro == gettedPros[i])
                                repeat = true;
                        }
                    }

                    if (!repeat)
                    {
                        int index = 0;

                        if (sp_nodes.arraySize <= 0)
                            index = 0;
                        else
                            index = sp_nodes.arraySize - 1;

                        sp_nodes.InsertArrayElementAtIndex(index);

                        SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(index);
                        SerializedProperty sp_pro = sp_node.FindPropertyRelative("Progress");
                        sp_pro.objectReferenceValue = gettedPros[i];

                        sp_pro.serializedObject.ApplyModifiedProperties();
                        sp_node.serializedObject.ApplyModifiedProperties();
                    }
                }
                sp_nodes.serializedObject.ApplyModifiedProperties();
            }
        }

        /// <summary>
        /// 获取所有容器
        /// </summary>
        private void GetAllContainer()
        {
            if (IsMultiSelected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    SerializedObject so_ele = new SerializedObject(SelectedObjects[i]);
                    SerializedProperty sp_nodes = so_ele.FindProperty("ContainerNodes");
                    so_ele.Update();

                    Hud_Container[] allcons = SelectedObjects[i].GetComponentsInChildren<Hud_Container>();

                    List<Hud_Container> Filter = new List<Hud_Container>();
                    for (int s = 0; s < allcons.Length; s++)
                    {
                        Filter.Add(allcons[s]);
                    }

                    Hud_Container[] gettedCons = Filter.ToArray();

                    for (int x = 0; x < gettedCons.Length; x++)
                    {
                        bool repeat = false;

                        if (sp_nodes.arraySize > 0)
                        {
                            for (int s = 0; s < sp_nodes.arraySize; s++)
                            {
                                SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                                SerializedProperty sp_node_con = sp_node.FindPropertyRelative("Container");
                                Hud_Container sp_con = (Hud_Container)sp_node_con.objectReferenceValue;
                                if (sp_con == gettedCons[x])
                                    repeat = true;
                            }
                        }

                        if (!repeat)
                        {
                            int index = 0;

                            if (sp_nodes.arraySize <= 0)
                                index = 0;
                            else
                                index = sp_nodes.arraySize - 1;

                            sp_nodes.InsertArrayElementAtIndex(index);

                            SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(index);
                            SerializedProperty sp_con = sp_node.FindPropertyRelative("Container");
                            sp_con.objectReferenceValue = gettedCons[x];

                            sp_con.serializedObject.ApplyModifiedProperties();
                            sp_node.serializedObject.ApplyModifiedProperties();
                        }
                    }
                    sp_nodes.serializedObject.ApplyModifiedProperties();
                    so_ele.ApplyModifiedProperties();
                }
            }
            else
            {
                SerializedProperty sp_nodes = serializedObject.FindProperty("ContainerNodes");

                Hud_Container[] allcons = BaseScript.GetComponentsInChildren<Hud_Container>();

                List<Hud_Container> Filter = new List<Hud_Container>();
                for (int i = 0; i < allcons.Length; i++)
                {
                    Filter.Add(allcons[i]);
                }

                Hud_Container[] gettedCons = Filter.ToArray();

                for (int i = 0; i < gettedCons.Length; i++)
                {
                    bool repeat = false;

                    if (sp_nodes.arraySize > 0)
                    {
                        for (int s = 0; s < sp_nodes.arraySize; s++)
                        {
                            SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                            SerializedProperty sp_node_con = sp_node.FindPropertyRelative("Container");
                            Hud_Container sp_con = (Hud_Container)sp_node_con.objectReferenceValue;
                            if (sp_con == gettedCons[i])
                                repeat = true;
                        }
                    }

                    if (!repeat)
                    {
                        int index = 0;

                        if (sp_nodes.arraySize <= 0)
                            index = 0;
                        else
                            index = sp_nodes.arraySize - 1;

                        sp_nodes.InsertArrayElementAtIndex(index);

                        SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(index);
                        SerializedProperty sp_con = sp_node.FindPropertyRelative("Container");
                        sp_con.objectReferenceValue = gettedCons[i];

                        sp_con.serializedObject.ApplyModifiedProperties();
                        sp_node.serializedObject.ApplyModifiedProperties();
                    }
                }
                sp_nodes.serializedObject.ApplyModifiedProperties();
            }
        }

        /// <summary>
        /// 获取所有开关
        /// </summary>
        private void GetAllToggle()
        {
            if (IsMultiSelected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    SerializedObject so_ele = new SerializedObject(SelectedObjects[i]);
                    SerializedProperty sp_nodes = so_ele.FindProperty("ToggleNodes");
                    so_ele.Update();

                    Hud_Toggle[] alltog = SelectedObjects[i].GetComponentsInChildren<Hud_Toggle>();

                    List<Hud_Toggle> Filter = new List<Hud_Toggle>();
                    for (int s = 0; s < alltog.Length; s++)
                    {
                        Filter.Add(alltog[s]);
                    }

                    Hud_Toggle[] gettedtogs = Filter.ToArray();

                    for (int x = 0; x < gettedtogs.Length; x++)
                    {
                        bool repeat = false;

                        if (sp_nodes.arraySize > 0)
                        {
                            for (int s = 0; s < sp_nodes.arraySize; s++)
                            {
                                SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                                SerializedProperty sp_node_tog = sp_node.FindPropertyRelative("Toggle");
                                Hud_Toggle sp_tog = (Hud_Toggle)sp_node_tog.objectReferenceValue;
                                if (sp_tog == gettedtogs[x])
                                    repeat = true;
                            }
                        }

                        if (!repeat)
                        {
                            int index = 0;

                            if (sp_nodes.arraySize <= 0)
                                index = 0;
                            else
                                index = sp_nodes.arraySize - 1;

                            sp_nodes.InsertArrayElementAtIndex(index);

                            SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(index);
                            SerializedProperty sp_tog = sp_node.FindPropertyRelative("Toggle");
                            sp_tog.objectReferenceValue = gettedtogs[x];

                            sp_tog.serializedObject.ApplyModifiedProperties();
                            sp_node.serializedObject.ApplyModifiedProperties();
                        }
                    }
                    sp_nodes.serializedObject.ApplyModifiedProperties();
                    so_ele.ApplyModifiedProperties();
                }
            }
            else
            {
                SerializedProperty sp_nodes = serializedObject.FindProperty("ToggleNodes");

                Hud_Toggle[] alltog = BaseScript.GetComponentsInChildren<Hud_Toggle>();

                List<Hud_Toggle> Filter = new List<Hud_Toggle>();
                for (int i = 0; i < alltog.Length; i++)
                {
                    Filter.Add(alltog[i]);
                }

                Hud_Toggle[] gettedtogs = Filter.ToArray();

                for (int i = 0; i < gettedtogs.Length; i++)
                {
                    bool repeat = false;

                    if (sp_nodes.arraySize > 0)
                    {
                        for (int s = 0; s < sp_nodes.arraySize; s++)
                        {
                            SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                            SerializedProperty sp_node_tog = sp_node.FindPropertyRelative("Toggle");
                            Hud_Toggle sp_tog = (Hud_Toggle)sp_node_tog.objectReferenceValue;
                            if (sp_tog == gettedtogs[i])
                                repeat = true;
                        }
                    }

                    if (!repeat)
                    {
                        int index = 0;

                        if (sp_nodes.arraySize <= 0)
                            index = 0;
                        else
                            index = sp_nodes.arraySize - 1;

                        sp_nodes.InsertArrayElementAtIndex(index);

                        SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(index);
                        SerializedProperty sp_tog = sp_node.FindPropertyRelative("Toggle");
                        sp_tog.objectReferenceValue = gettedtogs[i];

                        sp_tog.serializedObject.ApplyModifiedProperties();
                        sp_node.serializedObject.ApplyModifiedProperties();
                    }
                }
                sp_nodes.serializedObject.ApplyModifiedProperties();
            }
        }

        /// <summary>
        /// 获取所有音效器
        /// </summary>
        private void GetAllSounder()
        {
            if (IsMultiSelected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    SerializedObject so_ele = new SerializedObject(SelectedObjects[i]);
                    SerializedProperty sp_nodes = so_ele.FindProperty("SounderNodes");
                    so_ele.Update();

                    Hud_Sounder[] allsod = SelectedObjects[i].GetComponentsInChildren<Hud_Sounder>();

                    List<Hud_Sounder> Filter = new List<Hud_Sounder>();
                    for (int s = 0; s < allsod.Length; s++)
                    {
                        Filter.Add(allsod[s]);
                    }

                    Hud_Sounder[] gettedsods = Filter.ToArray();

                    for (int x = 0; x < gettedsods.Length; x++)
                    {
                        bool repeat = false;

                        if (sp_nodes.arraySize > 0)
                        {
                            for (int s = 0; s < sp_nodes.arraySize; s++)
                            {
                                SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                                SerializedProperty sp_node_sod = sp_node.FindPropertyRelative("Sounder");
                                Hud_Sounder sp_sod = (Hud_Sounder)sp_node_sod.objectReferenceValue;
                                if (sp_sod == gettedsods[x])
                                    repeat = true;
                            }
                        }

                        if (!repeat)
                        {
                            int index = 0;

                            if (sp_nodes.arraySize <= 0)
                                index = 0;
                            else
                                index = sp_nodes.arraySize;

                            sp_nodes.InsertArrayElementAtIndex(index);

                            SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(index);
                            SerializedProperty sp_tog = sp_node.FindPropertyRelative("Sounder");
                            sp_tog.objectReferenceValue = gettedsods[x];

                            sp_tog.serializedObject.ApplyModifiedProperties();
                            sp_node.serializedObject.ApplyModifiedProperties();
                        }
                    }
                    sp_nodes.serializedObject.ApplyModifiedProperties();
                    so_ele.ApplyModifiedProperties();
                }
            }
            else
            {
                SerializedProperty sp_nodes = serializedObject.FindProperty("SounderNodes");

                Hud_Sounder[] allsod = BaseScript.GetComponentsInChildren<Hud_Sounder>();

                List<Hud_Sounder> Filter = new List<Hud_Sounder>();
                for (int i = 0; i < allsod.Length; i++)
                {
                    Hud_Button hud_Button = allsod[i].GetComponentInParent<Hud_Button>();
                    Hud_Progress hud_Progress = allsod[i].GetComponentInParent<Hud_Progress>();
                    Hud_Slider hud_Slider = allsod[i].GetComponentInParent<Hud_Slider>();
                    Hud_Option hud_optselector = allsod[i].GetComponentInParent<Hud_Option>();
                    Hud_Toggle hud_tog = allsod[i].GetComponentInParent<Hud_Toggle>();
                    Hud_Container hud_dat = allsod[i].GetComponentInParent<Hud_Container>();
                    if (hud_Button != null)
                        continue;
                    if (hud_Progress != null)
                        continue;
                    if (hud_Slider != null)
                        continue;
                    if (hud_optselector != null)
                        continue;
                    if (hud_tog != null)
                        continue;
                    if (hud_dat != null)
                        continue;

                    Filter.Add(allsod[i]);
                }

                Hud_Sounder[] gettedsods = Filter.ToArray();

                for (int i = 0; i < gettedsods.Length; i++)
                {
                    bool repeat = false;

                    if (sp_nodes.arraySize > 0)
                    {
                        for (int s = 0; s < sp_nodes.arraySize; s++)
                        {
                            SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                            SerializedProperty sp_node_sod = sp_node.FindPropertyRelative("Sounder");
                            Hud_Sounder sp_tog = (Hud_Sounder)sp_node_sod.objectReferenceValue;
                            if (sp_tog == gettedsods[i])
                                repeat = true;
                        }
                    }

                    if (!repeat)
                    {
                        int index = 0;

                        if (sp_nodes.arraySize <= 0)
                            index = 0;
                        else
                            index = sp_nodes.arraySize;

                        sp_nodes.InsertArrayElementAtIndex(index);

                        SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(index);
                        SerializedProperty sp_sod = sp_node.FindPropertyRelative("Sounder");
                        sp_sod.objectReferenceValue = gettedsods[i];

                        sp_sod.serializedObject.ApplyModifiedProperties();
                        sp_node.serializedObject.ApplyModifiedProperties();
                    }
                }
                sp_nodes.serializedObject.ApplyModifiedProperties();
            }
        }

        #endregion
    }
}