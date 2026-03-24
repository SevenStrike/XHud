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
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.UI;
    using Random = UnityEngine.Random;

    public class xHud_ModuleArg_Option
    {
        public string Indicator;
        public float OptionSelector_Animators_GlobalDuration;
        public bool RepeatAnimatorPlay;
        public bool UseBlinked;
        public bool UseEaseMotion;
        public float LerpSpeed;
        public float TweenSpeed;
        public Ease SelectorTweenMotion;
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(xHud_Module_Option), true)]
    public class Editor_xHud_Module_Option : Editor
    {
        private Vector2 lastSize;

        #region 组件 / 列表
        private xHud_Module_Option BaseScript;
        private ReorderableList OptionButtonList;
        private ReorderableList SelectorAnimatorList;
        private xHud_Module_Animator selectormark_animator;
        #endregion

        #region 序列化属性
        private SerializedProperty DebugState, SelectorTweenMotion, SelectorOffset, SelectorOffsetAdded, Indicator, LerpSpeed, Pos_Destination, ChangingInterval, sm_Pos_Destination, TweenSpeed, OptionSelector, RepeatAnimatorPlay, OptionSelector_Animators_GlobalDuration, SelectorAnimatorNodes, UseEaseMotion, OptionButtonNodes, AnimatorsMaxDuration, CurrentOptionName, OptionIndex, SelectorMark, OptionRoot, UseBlinked, AnimateState, AnimatorsIsFold, ButtonIsFold, EventIsFold, eve_on_selector_position_changed, eve_on_option_clicked, eve_on_option_clicked_with_indicator, eve_on_option_clicked_with_index, eve_on_option_clicked_with_position, eve_on_selector_position_started, eve_on_selector_position_complete, AutoStopPreview;
        #endregion

        #region 图标                                                                                                                                     
        private Texture2D icon_main, find_r, find_p, play_r, play_p, stop_r, stop_p, clear_r, clear_p, icon_button, animstate, dutation, longpressmarker, left_arrow_r, left_arrow_p, right_arrow_r, right_arrow_p, icon_anim, icon_option;
        #endregion

        private float LineHeight;
        private bool BasicVars;

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

        #region 批量模式查看索引
        private int OptionStatu_Index;
        private int OptionStatistic_Index;
        #endregion

        #region 选项文字
        string[] stroptions_enabled = new string[2] { "关闭", "开启" }, stroptions_debug = new string[2] { "关闭", "调试" }, stroptions_blinked = new string[2] { "平滑", "闪现" }, stroptions_easemode = new string[2] { "差值", "缓动" };
        #endregion

        #region 批量化操作
        xHud_Module_Option[] SelectedObjects;

        private void GetAllTargets()
        {
            if (targets.Length > 1)
            {
                SelectedObjects = new xHud_Module_Option[targets.Length];
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    var t = targets[i];
                    SelectedObjects[i] = (xHud_Module_Option)t;
                }
            }
            else
            {
                SelectedObjects = new xHud_Module_Option[targets.Length];
                SelectedObjects[0] = (xHud_Module_Option)target;
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

        void OnEnable()
        {
            BaseScript = (xHud_Module_Option)target;

            #region 获取序列化属性
            OptionButtonNodes = serializedObject.FindProperty("OptionButtonNodes");
            SelectorAnimatorNodes = serializedObject.FindProperty("SelectorAnimatorNodes");
            UseEaseMotion = serializedObject.FindProperty("UseEaseMotion");
            LerpSpeed = serializedObject.FindProperty("LerpSpeed");
            SelectorTweenMotion = serializedObject.FindProperty("SelectorTweenMotion");
            SelectorOffset = serializedObject.FindProperty("SelectorOffset");
            SelectorOffsetAdded = serializedObject.FindProperty("SelectorOffsetAdded");
            RepeatAnimatorPlay = serializedObject.FindProperty("RepeatAnimatorPlay");
            TweenSpeed = serializedObject.FindProperty("TweenSpeed");
            OptionSelector = serializedObject.FindProperty("SelectorMark");
            CurrentOptionName = serializedObject.FindProperty("CurrentOptionName");
            OptionIndex = serializedObject.FindProperty("OptionIndex");
            SelectorMark = serializedObject.FindProperty("SelectorMark");
            OptionSelector_Animators_GlobalDuration = serializedObject.FindProperty("OptionSelector_Animators_GlobalDuration");
            Pos_Destination = serializedObject.FindProperty("Pos_Destination");
            OptionRoot = serializedObject.FindProperty("OptionRoot");
            sm_Pos_Destination = serializedObject.FindProperty("sm_Pos_Destination");
            UseBlinked = serializedObject.FindProperty("UseBlinked");
            eve_on_option_clicked = serializedObject.FindProperty("eve_on_option_clicked");
            eve_on_option_clicked_with_indicator = serializedObject.FindProperty("eve_on_option_clicked_with_indicator");
            eve_on_option_clicked_with_index = serializedObject.FindProperty("eve_on_option_clicked_with_index");
            eve_on_option_clicked_with_position = serializedObject.FindProperty("eve_on_option_clicked_with_position");
            eve_on_selector_position_changed = serializedObject.FindProperty("eve_on_selector_position_changed");
            eve_on_selector_position_started = serializedObject.FindProperty("eve_on_selector_position_started");
            eve_on_selector_position_complete = serializedObject.FindProperty("eve_on_selector_position_complete");
            AnimatorsMaxDuration = serializedObject.FindProperty("AnimatorsMaxDuration");
            AnimateState = serializedObject.FindProperty("AnimateState");
            Indicator = serializedObject.FindProperty("Indicator");
            AnimatorsIsFold = serializedObject.FindProperty("AnimatorsIsFold");
            ButtonIsFold = serializedObject.FindProperty("ButtonIsFold");
            EventIsFold = serializedObject.FindProperty("EventIsFold");
            DebugState = serializedObject.FindProperty("DebugState");
            ChangingInterval = serializedObject.FindProperty("ChangingInterval");
            AutoStopPreview = serializedObject.FindProperty("AutoStopPreview");
            #endregion

            #region 获取图标
            icon_main = Editor_xHudGUI.GetIcon("Icons_Hud_Option/icon_main");
            find_r = Editor_xHudGUI.GetIcon("Icons_Hud_Option/find_r");
            find_p = Editor_xHudGUI.GetIcon("Icons_Hud_Option/find_p");
            play_r = Editor_xHudGUI.GetIcon("Icons_Hud_Option/play_r");
            play_p = Editor_xHudGUI.GetIcon("Icons_Hud_Option/play_p");
            stop_r = Editor_xHudGUI.GetIcon("Icons_Hud_Option/stop_r");
            stop_p = Editor_xHudGUI.GetIcon("Icons_Hud_Option/stop_p");
            clear_r = Editor_xHudGUI.GetIcon("Icons_Hud_Option/clear_r");
            clear_p = Editor_xHudGUI.GetIcon("Icons_Hud_Option/clear_p");
            icon_button = Editor_xHudGUI.GetIcon("Icons_Hud_Option/icon_button");
            animstate = Editor_xHudGUI.GetIcon("Icons_Hud_Option/animstate");
            dutation = Editor_xHudGUI.GetIcon("Icons_Hud_Option/dutation");
            longpressmarker = Editor_xHudGUI.GetIcon("Icons_Hud_Option/longpressmarker");
            left_arrow_r = Editor_xHudGUI.GetIcon("Icons_Hud_Option/left_arrow_r");
            left_arrow_p = Editor_xHudGUI.GetIcon("Icons_Hud_Option/left_arrow_p");
            right_arrow_r = Editor_xHudGUI.GetIcon("Icons_Hud_Option/right_arrow_r");
            right_arrow_p = Editor_xHudGUI.GetIcon("Icons_Hud_Option/right_arrow_p");
            icon_anim = Editor_xHudGUI.GetIcon("Icons_Hud_Option/icon_anim");
            icon_option = Editor_xHudGUI.GetIcon("Icons_Hud_Option/icon_option");
            #endregion

            if (SelectorMark.objectReferenceValue != null)
            {
                SelectorOffsetAdded.vector3Value = ((RectTransform)SelectorMark.objectReferenceValue).anchoredPosition3D;
                SelectorOffsetAdded.serializedObject.ApplyModifiedProperties();
            }

            LineHeight = EditorGUIUtility.singleLineHeight;

            Vector2 ButtonSize = new Vector2(18, 18);

            #region ReorderableList - SelectorAnimators
            SelectorAnimatorList = new ReorderableList(serializedObject, SelectorAnimatorNodes)
            {
                displayAdd = true,
                displayRemove = true,
                draggable = true,

                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "动画器列表");
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    float titleheight = rect.y + 6;
                    float baseheight = rect.y + (rect.height - 25);

                    SerializedProperty sp_root = SelectorAnimatorNodes.GetArrayElementAtIndex(index);
                    SerializedProperty sp_animator = sp_root.FindPropertyRelative("Animator");
                    SerializedProperty sp_delay = sp_root.FindPropertyRelative("DelayTime");
                    xHud_Module_Animator animator = (xHud_Module_Animator)sp_animator.objectReferenceValue;

                    if (sp_animator.objectReferenceValue != null)
                    {
                        #region 标题
                        string title = "";
                        string indicator = animator.GetIndicator();
                        if (!string.IsNullOrEmpty(indicator))
                            title += indicator;
                        else
                            title += animator.gameObject.name;
                        Editor_xHudGUI.Gui_Labelfield(new Rect(rect.width - (rect.width - 90), titleheight - 2, 180, LineHeight), title, HudFilled.无, HudColor.亮白, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 11, TextClipping.Clip);
                        #endregion

                        #region 类型图标         
                        GUI.color = xHud_Dashboard.Theme_Primary;
                        Editor_xHudGUI.Gui_Icon(new Rect(rect.width - (rect.width - 60), titleheight + 1, 10, 10), icon_anim);
                        GUI.color = Color.white;
                        #endregion

                        #region 延迟
                        Editor_xHudGUI.Gui_Property_Field(new Rect(rect.width - 5, rect.y + 4, 30, 19), "D", sp_delay, 10, 40, LineHeight, 15);
                        #endregion

                        #region 速率   
                        SerializedObject so_anim = new SerializedObject(animator);
                        so_anim.Update();

                        SerializedProperty sp_glodur = so_anim.FindProperty("Animator_GlobalDuration");
                        SerializedProperty sp_maxdur = so_anim.FindProperty("MaxTimerWithGlobalDuration");

                        Editor_xHudGUI.Gui_Property_Field(new Rect(rect.width - 50, rect.y + 4, 30, 19), "G", sp_glodur, 10, 40, LineHeight, 15);

                        Editor_xHudGUI.Gui_Labelfield_Thin(new Rect(rect.width - 80, rect.y + 4, 30, 19), $"{sp_maxdur.floatValue.ToString()} s", HudFilled.无, HudColor.无, xHud_Dashboard.Theme_Primary, TextAnchor.MiddleCenter, Vector2.zero, 11);
                        #endregion

                        sp_animator.serializedObject.ApplyModifiedProperties();
                        sp_root.serializedObject.ApplyModifiedProperties();
                    }
                    else
                    {
                        SelectorAnimatorNodes.DeleteArrayElementAtIndex(index);
                        SelectorAnimatorNodes.serializedObject.ApplyModifiedProperties();
                    }
                },
                onSelectCallback = (ReorderableList list) =>
                {
                    if (!Application.isPlaying)
                    {
                        Preview_Animator_Stop();
                    }

                    SerializedProperty sp_root = SelectorAnimatorNodes.GetArrayElementAtIndex(list.index);
                    SerializedProperty sp_animator = sp_root.FindPropertyRelative("Animator");

                    if (sp_animator != null)
                    {
                        EditorGUIUtility.PingObject(sp_animator.objectReferenceValue);
                        Preview_Animator_PlayAt(list.index);
                    }
                },
                elementHeightCallback = index =>
                {
                    return 1.5f * LineHeight;
                }
            };
            #endregion

            #region ReorderableList - ButtonOptions
            OptionButtonList = new ReorderableList(serializedObject, OptionButtonNodes)
            {
                displayAdd = true,
                displayRemove = true,
                draggable = true,

                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "按钮列表");
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    float titleheight = rect.y + 6;
                    float baseheight = rect.y + (rect.height - 25);

                    SerializedProperty sp_button = OptionButtonNodes.GetArrayElementAtIndex(index).FindPropertyRelative("Button");

                    if (sp_button.objectReferenceValue != null)
                    {
                        xHud_Module_Button btn = (xHud_Module_Button)sp_button.objectReferenceValue;
                        SerializedProperty sp_isopt = OptionButtonNodes.GetArrayElementAtIndex(index).FindPropertyRelative("IsOptional");

                        string title = "";
                        if (!string.IsNullOrEmpty(btn.Indicator))
                            title += btn.Indicator;
                        else
                            title += btn.gameObject.name;
                        Editor_xHudGUI.Gui_Labelfield(new Rect(rect.width - (rect.width - 90), titleheight - 2, (rect.width * 0.45f) - 20, LineHeight), title, HudFilled.无, HudColor.亮白, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 11, TextClipping.Clip);

                        GUI.color = xHud_Dashboard.Theme_Primary;
                        Editor_xHudGUI.Gui_Icon(new Rect(rect.width - (rect.width - 60), titleheight, 10, 10), icon_button);

                        #region 类型图标
                        if (sp_isopt.boolValue)
                        {
                            if (OptionIndex.intValue == index)
                            {
                                GUI.color = xHud_Dashboard.Theme_Primary;
                            }
                            else
                            {
                                GUI.color = Editor_xHudGUI.GetColor(HudColor.枪灰);
                            }
                            Editor_xHudGUI.Gui_Icon(new Rect(rect.width + 30, titleheight + 1.8f, 10, 10), icon_option);
                        }
                        GUI.color = Color.white;
                        #endregion
                    }
                    else
                    {
                        EditorGUI.LabelField(new Rect(rect.x + 5, titleheight - 2, 180, LineHeight), "未指定的空项");
                    }
                },
                onSelectCallback = (ReorderableList list) =>
                {
                    if (SelectorMark.objectReferenceValue == null)
                    {
                        Editor_xHudGUI.Open(xHudDialogType.警告, "HudOption选项器消息", "缺失光标组件", "您并未为选项器设置一个有效光标组件！请检查参数面板中的 “光标指示器”是否为空！", "明白");
                        return;
                    }
                    else
                    {
                        SerializedProperty sp_button = OptionButtonNodes.GetArrayElementAtIndex(list.index).FindPropertyRelative("Button");
                        xHud_Module_Button sp_btn = (xHud_Module_Button)sp_button.objectReferenceValue;
                        SerializedProperty sp_indicator = OptionButtonNodes.GetArrayElementAtIndex(list.index).FindPropertyRelative("Indicator");

                        OptionIndex.intValue = list.index;
                        OptionIndex.serializedObject.ApplyModifiedProperties();
                        CurrentOptionName.stringValue = sp_btn.Indicator;

                        RectTransform sel_mark = (RectTransform)SelectorMark.objectReferenceValue;

                        for (int i = 0; i < OptionButtonNodes.arraySize; i++)
                        {
                            SerializedProperty sp_x_button = OptionButtonNodes.GetArrayElementAtIndex(i).FindPropertyRelative("Button");
                            xHud_Module_Button sp_x_btn = (xHud_Module_Button)sp_x_button.objectReferenceValue;
                            if (sp_x_btn.Indicator == CurrentOptionName.stringValue)
                            {

                                RectTransform sel_opts = (RectTransform)OptionRoot.objectReferenceValue;
                                xHud_Module_Button btn = (xHud_Module_Button)OptionButtonNodes.GetArrayElementAtIndex(i).FindPropertyRelative("Button").objectReferenceValue;
                                Vector3 target = sel_opts.parent.InverseTransformPoint(btn.RectTransform.position);
                                SelectorOffsetAdded.vector3Value = target;
                                Pos_Destination.vector3Value = target;
                                sm_Pos_Destination.vector3Value = target;
                                OriginalPoseState_Record(sel_mark);

                                SelectorOffsetAdded.serializedObject.ApplyModifiedProperties();
                                Pos_Destination.serializedObject.ApplyModifiedProperties();
                                sm_Pos_Destination.serializedObject.ApplyModifiedProperties();

                                sel_mark.anchoredPosition3D = new Vector3(Pos_Destination.vector3Value.x, Pos_Destination.vector3Value.y, sel_mark.anchoredPosition3D.z) + SelectorOffset.vector3Value;

                                SelectorMark.serializedObject.ApplyModifiedProperties();
                            }
                        }
                        //#endregion

                        EditorGUIUtility.PingObject(sp_button.objectReferenceValue);
                    }
                },
                elementHeightCallback = index =>
                {
                    return 1.5f * LineHeight;
                }
            };
            #endregion

            GetAllTargets();
        }

        private void OnDisable()
        {
            if (!Application.isPlaying)
            {
                Preview_Animator_Stop();

                Preview_AnimatorSound_CoroutineList_Stop();
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            Editor_xHudGUI.Gui_Layout_Banner(icon_main, HudFilled.实体, HudColor.深空灰, "Hud - 选项器", Color.white);

            xHud_Manager mgr = xHud_Dashboard.HudManagerGet();

            #region 快捷功能
            Editor_xHudGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "快捷功能", xHud_Dashboard.Theme_Primary);
            Editor_xHudGUI.Gui_Layout_Space(10);

            Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_xHudGUI.Gui_Layout_Space(10);

            #region 扫描
            if (Editor_xHudGUI.Gui_Layout_Button(15, "获取选项按钮与动画组件", find_r, find_p))
            {
                string res = Editor_xHudGUI.Open(xHudDialogType.警告, "HudOption消息", "获取子组件", "请确保子级中按钮的选项模式已开启！否则无法被扫描到并作为选项按钮！", "立即扫描", "再检查下", 1);
                if (res == "再检查下")
                {
                    return;
                }

                GetAllOptionsButton();
                GetAllAnimators();
                GetAnimatorAndButtonResults();
                return;
            }
            #endregion

            GUILayout.FlexibleSpace();

            #region 预览
            if (!IsMultiSelected() && SelectorAnimatorNodes.arraySize > 0)
            {
                GUILayout.FlexibleSpace();

                #region 预览动画
                if (!Preivew_Animator_PlayingState)
                {
                    if (Editor_xHudGUI.Gui_Layout_Button(14, "播放所有动画器预览", play_r, play_p))
                    {
                        if (!Application.isPlaying)
                        {
                            Preview_Animator_Play();
                            return;
                        }
                    }
                }
                else
                {
                    if (Editor_xHudGUI.Gui_Layout_Button(14, "停止所有动画器预览", stop_r, stop_p))
                    {
                        if (!Application.isPlaying)
                        {
                            Preview_Animator_Stop();
                            return;
                        }
                    }
                }
                #endregion
            }
            #endregion

            Editor_xHudGUI.Gui_Layout_Space(10);
            Editor_xHudGUI.Gui_Layout_Horizontal_End();
            GUI.enabled = true;

            Editor_xHudGUI.Gui_Layout_Space(10);
            Editor_xHudGUI.Gui_Layout_Vertical_End();
            #endregion

            #region 选项
            Editor_xHudGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "选项", xHud_Dashboard.Theme_Primary);
            Editor_xHudGUI.Gui_Layout_Space(5);

            Editor_xHudGUI.Gui_Layout_Toggle<bool, xHud_Module_Option>("状态调试", stroptions_debug, ref DebugState, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

            Editor_xHudGUI.Gui_Layout_Toggle<bool, xHud_Module_Option>("选择时重复动画", stroptions_enabled, ref RepeatAnimatorPlay, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

            Editor_xHudGUI.Gui_Layout_Toggle<bool, xHud_Module_Option>("选择器闪现", stroptions_blinked, ref UseBlinked, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

            if (!UseBlinked.boolValue)
            {
                Editor_xHudGUI.Gui_Layout_Toggle<bool, xHud_Module_Option>("选择器缓动", stroptions_easemode, ref UseEaseMotion, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
            }

            Editor_xHudGUI.Gui_Layout_Space(5);
            Editor_xHudGUI.Gui_Layout_Vertical_End();
            #endregion

            #region 参数
            Editor_xHudGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "参数", xHud_Dashboard.Theme_Primary);
            Editor_xHudGUI.Gui_Layout_Space(10);

            #region 选项按钮说明

            Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_xHudGUI.Gui_Layout_Space(10);
            EditorGUILayout.HelpBox("注意：您可以根据需要增加选项，但同时要保证所有按钮的 \"标识\" 必须被设置不为空，并确保没有重名的按钮标识", MessageType.Info);
            Editor_xHudGUI.Gui_Layout_Space(5);
            Editor_xHudGUI.Gui_Layout_Horizontal_End();

            #endregion

            Editor_xHudGUI.Gui_Layout_Property_Field("标识名称", Indicator, 100);

            Editor_xHudGUI.Gui_Layout_Space(5);

            Editor_xHudGUI.Gui_Layout_Property_Field("光标指示器", OptionSelector, 100);

            Editor_xHudGUI.Gui_Layout_Space(5);

            Editor_xHudGUI.Gui_Layout_Property_Field("组根物体", OptionRoot, 100);

            Editor_xHudGUI.Gui_Layout_Space(5);

            Editor_xHudGUI.Gui_Layout_Property_Field("速率倍增", OptionSelector_Animators_GlobalDuration, 100);

            Editor_xHudGUI.Gui_Layout_Space(5);

            EditorGUI.BeginChangeCheck();
            Editor_xHudGUI.Gui_Layout_Property_Field("光标偏移", SelectorOffset, 100);
            SelectorOffset.serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
            {
                if (!IsMultiSelected())
                {
                    RectTransform selectorMarksel = (RectTransform)SelectorMark.objectReferenceValue;
                    selectorMarksel.anchoredPosition3D = SelectorOffsetAdded.vector3Value + SelectorOffset.vector3Value;
                    OriginalPoseState_Record(selectorMarksel);
                }
                else
                {
                    for (int i = 0; i < SelectedObjects.Length; i++)
                    {
                        RectTransform selectorMarksel = SelectedObjects[i].SelectorMark;
                        selectorMarksel.anchoredPosition3D = SelectedObjects[i].SelectorOffsetAdded + SelectedObjects[i].SelectorOffset;
                        OriginalPoseState_Record(selectorMarksel);
                    }
                }
            }

            if (!UseBlinked.boolValue)
            {
                if (!UseEaseMotion.boolValue)
                {
                    Editor_xHudGUI.Gui_Layout_Space(5);

                    Editor_xHudGUI.Gui_Layout_Property_Field("差值平滑速率", LerpSpeed, 100);
                }
                else
                {
                    Editor_xHudGUI.Gui_Layout_Space(5);

                    Editor_xHudGUI.Gui_Layout_Property_Field("缓动方式", SelectorTweenMotion, 100);

                    Editor_xHudGUI.Gui_Layout_Space(5);

                    Editor_xHudGUI.Gui_Layout_Property_Field("缓动耗时", TweenSpeed, 100);
                }
            }

            Editor_xHudGUI.Gui_Layout_Space(10);
            Editor_xHudGUI.Gui_Layout_Vertical_End();
            #endregion

            #region 状态
            string statu_title = "状态";
            if (IsMultiSelected())
                statu_title = "状态 - ( 批量模式 )";
            Editor_xHudGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, statu_title, xHud_Dashboard.Theme_Primary);
            Editor_xHudGUI.Gui_Layout_Space(5);

            if (!IsMultiSelected())
            {
                #region 选项状态
                Editor_xHudGUI.StatuDisplayer_text(icon_button, 12, new Vector2(0, 7), "选项状态", 12, CurrentOptionName.stringValue, xHud_Dashboard.Theme_Primary, 11);
                #endregion

                #region 动画相关
                if (SelectorAnimatorNodes != null && SelectorAnimatorNodes.arraySize > 0)
                {
                    #region 动画状态     
                    Editor_xHudGUI.StatuDisplayer_text(animstate, 12, new Vector2(0, 7), "动画状态", 12, (HudElementAnimateState)AnimateState.enumValueIndex == HudElementAnimateState.Animating ? "动画中" : "静止状态", AnimateState.enumValueIndex == 1 ? xHud_Dashboard.Theme_Primary : Color.gray, 11);
                    #endregion

                    #region 最大耗时     
                    Editor_xHudGUI.StatuDisplayer_text(dutation, 12, new Vector2(0, 7), "最大耗时<color=#909090>（速率倍增）</color>", 12, AnimatorsMaxDuration.floatValue.ToString() + "秒", xHud_Dashboard.Theme_Primary, 11);
                    #endregion

                    #region 最大耗时        
                    Editor_xHudGUI.StatuDisplayer_text(dutation, 12, new Vector2(0, 7), "最大耗时<color=#909090>（XHUD倍增）</color>", 12, (mgr.DurationMultiply * AnimatorsMaxDuration.floatValue).ToString() + "秒", xHud_Dashboard.Theme_Primary, 11);
                    #endregion
                }
                #endregion

                Editor_xHudGUI.Gui_Layout_Space(5);
                Editor_xHudGUI.Gui_Layout_Seperator(1, xHud_Dashboard.Theme_SeperateLine);
                Editor_xHudGUI.Gui_Layout_Space(10);

                #region 当前选择的选项
                if (!Application.isPlaying)
                {
                    if (OptionButtonNodes != null && OptionButtonNodes.arraySize > 0)
                    {
                        string[] btn_names = BaseScript.opt_GetOptionButtonNames();
                        EditorGUI.BeginChangeCheck();
                        Editor_xHudGUI.Gui_Layout_Popup<int, xHud_Module_Option>("当前选项", btn_names, ref OptionIndex, HudFilled.实体, 120, 22, SelectedObjects);
                        if (EditorGUI.EndChangeCheck())
                        {
                            if (SelectorMark.objectReferenceValue == null)
                            {
                                Editor_xHudGUI.Open(xHudDialogType.警告, "HudOption选项器消息", "缺失光标组件", "您并未为选项器设置一个有效光标组件！请检查参数面板中的 “光标指示器”是否为空！", "明白");
                                return;
                            }
                            SelectorCorrection();
                        }
                    }
                }
                #endregion

                Editor_xHudGUI.Gui_Layout_Space(10);
            }
            else
            {
                #region 批量控件
                Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_xHudGUI.Gui_Layout_Space(10);
                if (Editor_xHudGUI.Gui_Layout_Button($"{SelectedObjects[OptionStatu_Index].name} ( {SelectedObjects[OptionStatu_Index].Indicator} )", "", HudFilled.透明, HudColor.无, Color.gray, 20))
                {
                    EditorGUIUtility.PingObject(SelectedObjects[OptionStatu_Index]);
                }
                Editor_xHudGUI.Gui_Layout_FlexSpace();
                if (Editor_xHudGUI.Gui_Layout_Button(12, "", left_arrow_r, left_arrow_p))
                {
                    if (OptionStatu_Index <= 0)
                    {
                        OptionStatu_Index = SelectedObjects.Length - 1;
                    }
                    else
                    {
                        OptionStatu_Index--;
                    }
                    EditorGUIUtility.PingObject(SelectedObjects[OptionStatu_Index]);
                }
                Editor_xHudGUI.Gui_Layout_Space(16);
                if (Editor_xHudGUI.Gui_Layout_Button(12, "", right_arrow_r, right_arrow_p))
                {
                    if (OptionStatu_Index >= SelectedObjects.Length - 1)
                    {
                        OptionStatu_Index = 0;
                    }
                    else
                    {
                        OptionStatu_Index++;
                    }
                    EditorGUIUtility.PingObject(SelectedObjects[OptionStatu_Index]);
                }
                Editor_xHudGUI.Gui_Layout_Space(5);
                Editor_xHudGUI.Gui_Layout_Horizontal_End();
                #endregion

                #region 按钮状态
                Editor_xHudGUI.StatuDisplayer_text(icon_button, 12, new Vector2(0, 7), "选项状态", 12, SelectedObjects[OptionStatu_Index].CurrentOptionName, xHud_Dashboard.Theme_Primary, 11);
                #endregion

                #region 动画相关
                if (SelectedObjects[OptionStatu_Index].SelectorAnimatorNodes != null && SelectedObjects[OptionStatu_Index].SelectorAnimatorNodes.Count > 0)
                {
                    #region 动画状态     
                    Editor_xHudGUI.StatuDisplayer_text(animstate, 12, new Vector2(0, 7), "动画状态", 12, SelectedObjects[OptionStatu_Index].AnimateState == HudElementAnimateState.Animating ? "动画中" : "静止状态", SelectedObjects[OptionStatu_Index].AnimateState == HudElementAnimateState.Animating ? xHud_Dashboard.Theme_Primary : Color.gray, 11);
                    #endregion

                    SelectedObjects[OptionStatu_Index].AnimatorsMaxDuration = Animators_GetAnimatorsMaxDuration(SelectedObjects[OptionStatu_Index].SelectorAnimatorNodes, SelectedObjects[OptionStatu_Index].OptionSelector_Animators_GlobalDuration);

                    #region 最大耗时     
                    Editor_xHudGUI.StatuDisplayer_text(dutation, 12, new Vector2(0, 7), "最大耗时<color=#909090>（速率倍增）</color>", 12, SelectedObjects[OptionStatu_Index].AnimatorsMaxDuration.ToString() + "秒", xHud_Dashboard.Theme_Primary, 11);
                    #endregion

                    #region 最大耗时        
                    Editor_xHudGUI.StatuDisplayer_text(dutation, 12, new Vector2(0, 7), "最大耗时<color=#909090>（XHUD倍增）</color>", 12, (mgr.DurationMultiply * SelectedObjects[OptionStatu_Index].AnimatorsMaxDuration).ToString() + "秒", xHud_Dashboard.Theme_Primary, 11);
                    #endregion
                }
                #endregion               
            }

            Editor_xHudGUI.Gui_Layout_Space(5);
            Editor_xHudGUI.Gui_Layout_Vertical_End();
            #endregion

            #region 统计
            string statu_statistic = "统计";
            if (IsMultiSelected())
                statu_statistic = "统计 - ( 批量模式 )";
            Editor_xHudGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, statu_statistic, xHud_Dashboard.Theme_Primary);
            Editor_xHudGUI.Gui_Layout_Space(5);

            if (!IsMultiSelected())
            {
                if (SelectorAnimatorNodes.arraySize <= 0)
                {
                    Editor_xHudGUI.Gui_Layout_Labelfield("暂无统计数据", HudFilled.无, HudColor.无, Editor_xHudGUI.GetColor(HudColor.阴影灰), TextAnchor.MiddleCenter);
                }
                else
                {
                    #region 组件数量 - 动画器
                    if (SelectorAnimatorNodes.arraySize > 0)
                    {
                        Editor_xHudGUI.StatuDisplayer_text(icon_anim, 12, new Vector2(0, 7), "动画器", 12, SelectorAnimatorNodes.arraySize.ToString() + " 个", xHud_Dashboard.Theme_Primary, 11);
                    }
                    #endregion

                    #region 组件数量 - 选项按钮
                    if (OptionButtonNodes.arraySize > 0)
                    {
                        Editor_xHudGUI.StatuDisplayer_text(icon_button, 12, new Vector2(0, 7), "选项按钮", 12, OptionButtonNodes.arraySize.ToString() + " 个", xHud_Dashboard.Theme_Primary, 11);
                    }
                    #endregion
                }
            }
            else
            {
                #region 批量控件
                Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_xHudGUI.Gui_Layout_Space(10);
                if (Editor_xHudGUI.Gui_Layout_Button($"{SelectedObjects[OptionStatistic_Index].name} ( {SelectedObjects[OptionStatistic_Index].Indicator} )", "", HudFilled.透明, HudColor.无, Color.gray, 20))
                {
                    EditorGUIUtility.PingObject(SelectedObjects[OptionStatistic_Index]);
                }
                Editor_xHudGUI.Gui_Layout_FlexSpace();
                if (Editor_xHudGUI.Gui_Layout_Button(12, "", left_arrow_r, left_arrow_p))
                {
                    if (OptionStatistic_Index <= 0)
                    {
                        OptionStatistic_Index = SelectedObjects.Length - 1;
                    }
                    else
                    {
                        OptionStatistic_Index--;
                    }
                    EditorGUIUtility.PingObject(SelectedObjects[OptionStatistic_Index]);
                }
                Editor_xHudGUI.Gui_Layout_Space(16);
                if (Editor_xHudGUI.Gui_Layout_Button(12, "", right_arrow_r, right_arrow_p))
                {
                    if (OptionStatistic_Index >= SelectedObjects.Length - 1)
                    {
                        OptionStatistic_Index = 0;
                    }
                    else
                    {
                        OptionStatistic_Index++;
                    }
                    EditorGUIUtility.PingObject(SelectedObjects[OptionStatistic_Index]);
                }
                Editor_xHudGUI.Gui_Layout_Space(5);
                Editor_xHudGUI.Gui_Layout_Horizontal_End();
                #endregion

                #region 组件数量 - 动画器
                if (SelectedObjects[OptionStatistic_Index].SelectorAnimatorNodes.Count > 0)
                {
                    Editor_xHudGUI.StatuDisplayer_text(icon_anim, 12, new Vector2(0, 7), "动画器", 12, SelectedObjects[OptionStatistic_Index].SelectorAnimatorNodes.Count + " 个", xHud_Dashboard.Theme_Primary, 11);
                }
                #endregion

                #region 组件数量 - 选项按钮
                if (SelectedObjects[OptionStatistic_Index].OptionButtonNodes.Count > 0)
                {
                    Editor_xHudGUI.StatuDisplayer_text(icon_button, 12, new Vector2(0, 7), "选项按钮", 12, SelectedObjects[OptionStatistic_Index].OptionButtonNodes.Count + " 个", xHud_Dashboard.Theme_Primary, 11);
                }
                #endregion
            }
            Editor_xHudGUI.Gui_Layout_Space(5);
            Editor_xHudGUI.Gui_Layout_Vertical_End();
            #endregion

            #region 检查是否有无效的动画器
            CheckAnimatorsValid();
            #endregion

            #region 检查是否有无效的选项按钮
            CheckButtonsValid();
            #endregion

            #region 事件/列表
            Editor_xHudGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "事件/列表", xHud_Dashboard.Theme_Primary);
            Editor_xHudGUI.Gui_Layout_Space(5);

            #region 动画器列表
            if (SelectorAnimatorNodes.arraySize > 0)
            {
                if (IsMultiSelected())
                {
                    Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_xHudGUI.Gui_Layout_Space(10);
                    EditorGUILayout.HelpBox("动画列表不支持多项操作", MessageType.Warning);
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    Editor_xHudGUI.Gui_Layout_Horizontal_End();
                }
                else
                {
                    Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_xHudGUI.Gui_Layout_Space(10);
                    AnimatorsIsFold.boolValue = EditorGUILayout.Foldout(AnimatorsIsFold.boolValue, "动画器", true);
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    Editor_xHudGUI.Gui_Layout_Horizontal_End();

                    Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    if (AnimatorsIsFold.boolValue)
                        SelectorAnimatorList.DoLayoutList();
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    Editor_xHudGUI.Gui_Layout_Horizontal_End();
                }
            }
            #endregion

            #region 按钮列表
            if (OptionButtonNodes.arraySize > 0)
            {
                if (IsMultiSelected())
                {
                    Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_xHudGUI.Gui_Layout_Space(10);
                    EditorGUILayout.HelpBox("选项按钮列表不支持多项操作", MessageType.Warning);
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    Editor_xHudGUI.Gui_Layout_Horizontal_End();
                }
                else
                {
                    Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_xHudGUI.Gui_Layout_Space(10);
                    ButtonIsFold.boolValue = EditorGUILayout.Foldout(ButtonIsFold.boolValue, "按钮", true);
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    Editor_xHudGUI.Gui_Layout_Horizontal_End();

                    Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    if (ButtonIsFold.boolValue)
                        OptionButtonList.DoLayoutList();
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    Editor_xHudGUI.Gui_Layout_Horizontal_End();
                }
            }
            #endregion

            #region 事件列表
            if (IsMultiSelected())
            {
                Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_xHudGUI.Gui_Layout_Space(10);
                EditorGUILayout.HelpBox("事件列表不支持多项操作", MessageType.Warning);
                Editor_xHudGUI.Gui_Layout_Space(5);
                Editor_xHudGUI.Gui_Layout_Horizontal_End();
            }
            else
            {
                Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_xHudGUI.Gui_Layout_Space(10);
                EventIsFold.boolValue = EditorGUILayout.Foldout(EventIsFold.boolValue, "事件", true);
                Editor_xHudGUI.Gui_Layout_Space(5);
                Editor_xHudGUI.Gui_Layout_Horizontal_End();

                if (EventIsFold.boolValue)
                {
                    Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_option_clicked);
                    eve_on_option_clicked.serializedObject.ApplyModifiedProperties();
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    Editor_xHudGUI.Gui_Layout_Horizontal_End();

                    Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_option_clicked_with_indicator);
                    eve_on_option_clicked_with_indicator.serializedObject.ApplyModifiedProperties();
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    Editor_xHudGUI.Gui_Layout_Horizontal_End();

                    Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_option_clicked_with_index);
                    eve_on_option_clicked_with_index.serializedObject.ApplyModifiedProperties();
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    Editor_xHudGUI.Gui_Layout_Horizontal_End();

                    Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_option_clicked_with_position);
                    eve_on_option_clicked_with_position.serializedObject.ApplyModifiedProperties();
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    Editor_xHudGUI.Gui_Layout_Horizontal_End();

                    Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_selector_position_changed);
                    eve_on_selector_position_changed.serializedObject.ApplyModifiedProperties();
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    Editor_xHudGUI.Gui_Layout_Horizontal_End();

                    Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_selector_position_started);
                    eve_on_selector_position_started.serializedObject.ApplyModifiedProperties();
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    Editor_xHudGUI.Gui_Layout_Horizontal_End();

                    Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_selector_position_complete);
                    eve_on_selector_position_complete.serializedObject.ApplyModifiedProperties();
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    Editor_xHudGUI.Gui_Layout_Horizontal_End();
                }
            }
            #endregion

            Editor_xHudGUI.Gui_Layout_Space(5);
            Editor_xHudGUI.Gui_Layout_Vertical_End();
            #endregion

            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                SerializedProperty sp_OptionSelector_Animators_GlobalDuration = serializedObject.FindProperty("OptionSelector_Animators_GlobalDuration");
                SerializedProperty sp_RepeatAnimatorPlay = serializedObject.FindProperty("RepeatAnimatorPlay");
                SerializedProperty sp_UseBlinked = serializedObject.FindProperty("UseBlinked");
                SerializedProperty sp_UseEaseMotion = serializedObject.FindProperty("UseEaseMotion");
                SerializedProperty sp_LerpSpeed = serializedObject.FindProperty("LerpSpeed");
                SerializedProperty sp_TweenSpeed = serializedObject.FindProperty("TweenSpeed");
                SerializedProperty sp_SelectorTweenMotion = serializedObject.FindProperty("SelectorTweenMotion");

                // 创建右键菜单
                GenericMenu menu = new GenericMenu();
                menu.AddDisabledItem(new GUIContent("脚本参数"));
                if (!IsMultiSelected())
                {
                    menu.AddItem(new GUIContent("C (拷贝)"), false, () =>
                    {
                        xHud_ModuleArg_Option hop = new xHud_ModuleArg_Option();

                        hop.OptionSelector_Animators_GlobalDuration = sp_OptionSelector_Animators_GlobalDuration.floatValue;
                        hop.RepeatAnimatorPlay = sp_RepeatAnimatorPlay.boolValue;
                        hop.UseBlinked = sp_UseBlinked.boolValue;
                        hop.UseEaseMotion = sp_UseEaseMotion.boolValue;
                        hop.LerpSpeed = sp_LerpSpeed.floatValue;
                        hop.TweenSpeed = sp_TweenSpeed.floatValue;
                        hop.SelectorTweenMotion = (Ease)sp_SelectorTweenMotion.enumValueIndex;

                        string json = JsonUtility.ToJson(hop);
                        GUIUtility.systemCopyBuffer = json;
                    });
                }
                menu.AddItem(new GUIContent("V (粘贴)"), false, () =>
                {
                    xHud_ModuleArg_Option hop = JsonUtility.FromJson<xHud_ModuleArg_Option>(GUIUtility.systemCopyBuffer);

                    if (IsMultiSelected())
                    {
                        for (int i = 0; i < SelectedObjects.Length; i++)
                        {
                            SerializedObject so_option = new SerializedObject(SelectedObjects[i]);

                            SerializedProperty c_sp_OptionSelector_Animators_GlobalDuration = so_option.FindProperty("OptionSelector_Animators_GlobalDuration");
                            SerializedProperty c_sp_RepeatAnimatorPlay = so_option.FindProperty("RepeatAnimatorPlay");
                            SerializedProperty c_sp_UseBlinked = so_option.FindProperty("UseBlinked");
                            SerializedProperty c_sp_UseEaseMotion = so_option.FindProperty("UseEaseMotion");
                            SerializedProperty c_sp_LerpSpeed = so_option.FindProperty("LerpSpeed");
                            SerializedProperty c_sp_TweenSpeed = so_option.FindProperty("TweenSpeed");
                            SerializedProperty c_sp_SelectorTweenMotion = so_option.FindProperty("SelectorTweenMotion");

                            so_option.Update();

                            c_sp_OptionSelector_Animators_GlobalDuration.floatValue = hop.OptionSelector_Animators_GlobalDuration;
                            c_sp_RepeatAnimatorPlay.boolValue = hop.RepeatAnimatorPlay;
                            c_sp_UseBlinked.boolValue = hop.UseBlinked;
                            c_sp_UseEaseMotion.boolValue = hop.UseEaseMotion;
                            c_sp_LerpSpeed.floatValue = hop.LerpSpeed;
                            c_sp_TweenSpeed.floatValue = hop.TweenSpeed;
                            c_sp_SelectorTweenMotion.enumValueIndex = (int)hop.SelectorTweenMotion;

                            c_sp_OptionSelector_Animators_GlobalDuration.serializedObject.ApplyModifiedProperties();
                            c_sp_RepeatAnimatorPlay.serializedObject.ApplyModifiedProperties();
                            c_sp_UseBlinked.serializedObject.ApplyModifiedProperties();
                            c_sp_UseEaseMotion.serializedObject.ApplyModifiedProperties();
                            c_sp_LerpSpeed.serializedObject.ApplyModifiedProperties();
                            c_sp_TweenSpeed.serializedObject.ApplyModifiedProperties();
                            c_sp_SelectorTweenMotion.serializedObject.ApplyModifiedProperties();

                            so_option.ApplyModifiedProperties();
                        }
                    }
                    else
                    {
                        sp_OptionSelector_Animators_GlobalDuration.floatValue = hop.OptionSelector_Animators_GlobalDuration;
                        sp_RepeatAnimatorPlay.boolValue = hop.RepeatAnimatorPlay;
                        sp_UseBlinked.boolValue = hop.UseBlinked;
                        sp_UseEaseMotion.boolValue = hop.UseEaseMotion;
                        sp_LerpSpeed.floatValue = hop.LerpSpeed;
                        sp_TweenSpeed.floatValue = hop.TweenSpeed;
                        sp_SelectorTweenMotion.enumValueIndex = (int)hop.SelectorTweenMotion;

                        sp_OptionSelector_Animators_GlobalDuration.serializedObject.ApplyModifiedProperties();
                        sp_RepeatAnimatorPlay.serializedObject.ApplyModifiedProperties();
                        sp_UseBlinked.serializedObject.ApplyModifiedProperties();
                        sp_UseEaseMotion.serializedObject.ApplyModifiedProperties();
                        sp_LerpSpeed.serializedObject.ApplyModifiedProperties();
                        sp_TweenSpeed.serializedObject.ApplyModifiedProperties();
                        sp_SelectorTweenMotion.serializedObject.ApplyModifiedProperties();
                    }
                });
                menu.AddSeparator("");
                menu.AddDisabledItem(new GUIContent("动画器"));
                menu.AddItem(new GUIContent("X (扫描)"), false, () =>
                {
                    string res = Editor_xHudGUI.Open(xHudDialogType.警告, "HudOption消息", "获取子组件", "请确保子级中按钮的选项模式已开启！否则无法被扫描到并作为选项按钮！", "立即扫描", "再检查下", 1);
                    if (res == "再检查下")
                    {
                        return;
                    }

                    GetAllOptionsButton();
                    GetAllAnimators();
                    GetAnimatorAndButtonResults();
                    return;
                });
                menu.AddDisabledItem(new GUIContent("基础"));
                menu.AddItem(new GUIContent("A (自动标识)"), false, () =>
                {
                    if (IsMultiSelected())
                    {
                        for (int i = 0; i < SelectedObjects.Length; i++)
                        {
                            xHud_Module_Option opt = SelectedObjects[i];
                            opt.Indicator = opt.gameObject.name;
                            EditorUtility.SetDirty(opt);
                        }
                    }
                    else
                    {
                        Indicator.stringValue = BaseScript.gameObject.name;
                        Indicator.serializedObject.ApplyModifiedProperties();
                        Editor_xHudGUI.Open(xHudDialogType.修改, "HudOptionx消息", "标识自身", "将标识名称参数更改为物体的名称！", "明白");
                    }
                });
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("F (折叠组件列表)"), false, () =>
                {
                    AllListFoldState(false);
                });
                menu.AddItem(new GUIContent("G (展开组件列表)"), false, () =>
                {
                    AllListFoldState(true);
                });
                if (!IsMultiSelected())
                {
                    menu.AddSeparator("");
                    menu.AddDisabledItem(new GUIContent("预览"));
                    if (!Preivew_Animator_PlayingState)
                    {
                        menu.AddItem(new GUIContent("S (开始)"), false, () =>
                        {
                            if (!Application.isPlaying)
                            {
                                Preview_Animator_Play();
                            }
                        });
                    }
                    else
                    {
                        menu.AddItem(new GUIContent("S (停止)"), false, () =>
                        {
                            if (!Application.isPlaying)
                            {
                                Preview_Animator_Stop();
                            }
                        });
                    }
                }
                menu.ShowAsContext(); // 在鼠标位置显示右键菜单
            }

            #region 源脚本
            Editor_xHudGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 3, "源脚本", xHud_Dashboard.Theme_Primary);
            Editor_xHudGUI.Gui_Layout_Space(5);

            #region 原始变量
            Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_xHudGUI.Gui_Layout_Space(10);
            BasicVars = EditorGUILayout.Foldout(BasicVars, "变量/属性", true);
            Editor_xHudGUI.Gui_Layout_Space(5);
            Editor_xHudGUI.Gui_Layout_Horizontal_End();
            if (BasicVars)
                DrawDefaultInspector();
            #endregion

            Editor_xHudGUI.Gui_Layout_Space(5);
            Editor_xHudGUI.Gui_Layout_Vertical_End();
            #endregion

            CalculateAnimatorMaxDuration(BaseScript.SelectorAnimatorNodes, OptionSelector_Animators_GlobalDuration.floatValue);

            CorrectionSelectorWhenRectChangeSized();

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 当选项器尺寸发生变化时校正光标位置
        /// </summary>
        private void CorrectionSelectorWhenRectChangeSized()
        {
            Vector2 currentSize = ((RectTransform)(BaseScript.transform)).sizeDelta;

            if (currentSize != lastSize)
            {
                SelectorCorrection();
            }

            lastSize = currentSize;
        }

        /// <summary>
        /// 校正光标位置
        /// </summary>
        private void SelectorCorrection()
        {
            if (OptionButtonNodes != null && OptionButtonNodes.arraySize > 0)
            {
                if (SelectorMark.objectReferenceValue != null)
                {
                    string[] btn_names = BaseScript.opt_GetOptionButtonNames();
                    RectTransform sel_mark = (RectTransform)SelectorMark.objectReferenceValue;
                    RectTransform sel_opt_root = (RectTransform)OptionRoot.objectReferenceValue;

                    for (int i = 0; i < BaseScript.OptionButtonNodes.Count; i++)
                    {
                        OptionButtonNode node = BaseScript.OptionButtonNodes[i];

                        //如果按钮列表有同名的按钮则计算光标位置到按钮坐标
                        if (node.Indicator == btn_names[OptionIndex.intValue])
                        {
                            Vector3 target = sel_opt_root.parent.InverseTransformPoint(node.Button.RectTransform.position);

                            CurrentOptionName.stringValue = node.Indicator;

                            SelectorOffsetAdded.vector3Value = target;
                            Pos_Destination.vector3Value = target;
                            sm_Pos_Destination.vector3Value = target;


                            sel_mark.anchoredPosition3D = new Vector3(Pos_Destination.vector3Value.x, Pos_Destination.vector3Value.y, sel_mark.anchoredPosition3D.z) + SelectorOffset.vector3Value;

                            OriginalPoseState_Record(sel_mark);
                        }
                    }
                }
            }
        }

        #region 辅助
        /// <summary>
        /// 检查是否存在无效的动画器
        /// </summary>
        private void CheckAnimatorsValid()
        {
            for (int i = 0; i < SelectorAnimatorNodes.arraySize; i++)
            {
                SerializedProperty sp_animotrNode = SelectorAnimatorNodes.GetArrayElementAtIndex(i);
                SerializedProperty sp_animator = sp_animotrNode.FindPropertyRelative("Animator");

                if (sp_animator.objectReferenceValue == null)
                {
                    SelectorAnimatorNodes.DeleteArrayElementAtIndex(i);
                }
            }
        }

        /// <summary>
        /// 检查是否存在无效的选项按钮
        /// </summary>
        private void CheckButtonsValid()
        {
            for (int i = 0; i < SelectorAnimatorNodes.arraySize; i++)
            {
                SerializedProperty sp_buttonNode = OptionButtonNodes.GetArrayElementAtIndex(i);
                SerializedProperty sp_button = sp_buttonNode.FindPropertyRelative("Button");

                if (sp_button.objectReferenceValue == null)
                {
                    OptionButtonNodes.DeleteArrayElementAtIndex(i);
                }
            }
        }

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
                    SerializedProperty sp_event_isfold = so_ele.FindProperty("EventIsFold");
                    so_ele.Update();

                    sp_anim_isfold.boolValue = state;
                    sp_btn_isfold.boolValue = state;
                    sp_event_isfold.boolValue = state;
                    sp_event_isfold.serializedObject.ApplyModifiedProperties();
                    sp_anim_isfold.serializedObject.ApplyModifiedProperties();
                    sp_btn_isfold.serializedObject.ApplyModifiedProperties();
                    so_ele.ApplyModifiedProperties();
                }
            }
            else
            {
                if (target != null)
                {
                    AnimatorsIsFold.boolValue = state;
                    ButtonIsFold.boolValue = state;
                    EventIsFold.boolValue = state;
                    EventIsFold.serializedObject.ApplyModifiedProperties();
                    AnimatorsIsFold.serializedObject.ApplyModifiedProperties();
                    ButtonIsFold.serializedObject.ApplyModifiedProperties();
                }
            }
        }

        /// <summary>
        /// 还原Animator姿态
        /// </summary>
        private void OriginalPoseState_Load(xHud_Module_Animator animator)
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
                ((xHud_Module_Text)m_text.objectReferenceValue).color = col.colorValue;
                m_text.serializedObject.ApplyModifiedProperties();
            }
            if (m_tmptext.objectReferenceValue != null)
            {
                ((xHud_Module_TmpText)m_tmptext.objectReferenceValue).color = col.colorValue;
                m_tmptext.serializedObject.ApplyModifiedProperties();
            }

            so.ApplyModifiedProperties();
        }

        /// <summary>
        /// 记录Animator姿态
        /// </summary>
        private void OriginalPoseState_Record(RectTransform img)
        {
            if (img == null)
                return;

            selectormark_animator = img.GetComponent<xHud_Module_Animator>();

            if (selectormark_animator == null)
            {
                return;
            }

            SerializedObject so = new SerializedObject(selectormark_animator);
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

            pos.vector3Value = rect.anchoredPosition3D;
            eur.vector3Value = rect.localEulerAngles;
            sca.vector3Value = rect.localScale;
            size.vector2Value = rect.sizeDelta;
            m_rect.serializedObject.ApplyModifiedProperties();

            alp.floatValue = ((CanvasGroup)m_cav.objectReferenceValue).alpha;
            m_cav.serializedObject.ApplyModifiedProperties();

            if (m_img.objectReferenceValue != null)
            {
                Image x_img = (Image)m_img.objectReferenceValue;
                col.colorValue = x_img.color;
                fil.floatValue = x_img.fillAmount;
                m_img.serializedObject.ApplyModifiedProperties();
            }
            if (m_text.objectReferenceValue != null)
            {
                col.colorValue = ((xHud_Module_Text)m_text.objectReferenceValue).color;
                m_text.serializedObject.ApplyModifiedProperties();
            }
            if (m_tmptext.objectReferenceValue != null)
            {
                col.colorValue = ((xHud_Module_TmpText)m_tmptext.objectReferenceValue).color;
                m_tmptext.serializedObject.ApplyModifiedProperties();
            }

            so.ApplyModifiedProperties();
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
            float v = xHud_Utilitys.Array_MaxValue(x_list);
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
            for (int i = 0; i < SelectorAnimatorNodes.arraySize; i++)
            {
                SerializedProperty sp_animator = SelectorAnimatorNodes.GetArrayElementAtIndex(i).FindPropertyRelative("Animator");
                xHud_Module_Animator anim = (xHud_Module_Animator)sp_animator.objectReferenceValue;
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

        #endregion

        #region Animator 动画预览

        /// <summary>
        /// 预览动画
        /// </summary>
        private void Preview_Animator_Play()
        {
            if (SelectorAnimatorNodes.arraySize <= 0)
                return;
            xHud_Manager mgr = xHud_Dashboard.HudManagerGet();

            Preivew_Animator_PlayingState = true;
            AnimateState.enumValueIndex = 1;
            AnimateState.serializedObject.ApplyModifiedProperties();

            ///---用来存放调用动画器的动画效果的延迟时间
            List<float> delaytimes = new List<float>();

            ///---动画逻辑
            for (int i = 0; i < BaseScript.SelectorAnimatorNodes.Count; i++)
            {
                AnimatorNode node = BaseScript.SelectorAnimatorNodes[i];

                ///---判断动画器是否为有效
                if (node.Animator.AnimateTweenNodes == null || node.Animator.AnimateTweenNodes.Count <= 0)
                    continue;

                ///---循环第 i 个动画器节点的第 s 个动画效果是否开启
                for (int s = 0; s < node.Animator.AnimateTweenNodes.Count; s++)
                {
                    TweenNode twnnode = node.Animator.AnimateTweenNodes[s];
                    if (twnnode.ActivateOnlyToEnd)
                        continue;

                    ///---如果动画效果未开启则跳过
                    if (twnnode.Enabled)
                    {
                        if (mgr == null)
                            Preivew_Animator_TweenList.Add(node.Animator.Tweener_Play(twnnode, 1 * OptionSelector_Animators_GlobalDuration.floatValue * node.Animator.Animator_GlobalDuration));
                        else
                            Preivew_Animator_TweenList.Add(node.Animator.Tweener_Play(twnnode, mgr.DurationMultiply * OptionSelector_Animators_GlobalDuration.floatValue * node.Animator.Animator_GlobalDuration));
                        delaytimes.Add(node.DelayTime);
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            ///---预备动画器
            for (int i = 0; i < BaseScript.SelectorAnimatorNodes.Count; i++)
            {
                AnimatorNode node = BaseScript.SelectorAnimatorNodes[i];
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
        /// 预览动画
        /// </summary>
        private void Preview_Animator_PlayAt(int index)
        {
            xHud_Manager mgr = xHud_Dashboard.HudManagerGet();

            Preview_Animator_Stop();

            AnimatorNode node = BaseScript.SelectorAnimatorNodes[index];
            xHud_Module_Animator anim = node.Animator;

            if (node.Animator == null)
                return;

            if (node.Animator.TweenNode_GetCount() <= 0)
                return;

            Preivew_Animator_PlayingState = true;
            AnimateState.enumValueIndex = 1;
            AnimateState.serializedObject.ApplyModifiedProperties();

            ///---用来存放调用动画器的动画效果的延迟时间
            float delaytime = 0;

            if (node.Animator.AnimateTweenNodes == null || node.Animator.AnimateTweenNodes.Count <= 0)
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
                if (BaseScript.SelectorAnimatorNodes != null && BaseScript.SelectorAnimatorNodes.Count > 0)
                {
                    for (int c = 0; c < BaseScript.SelectorAnimatorNodes.Count; c++)
                    {
                        xHud_Module_Animator animator = BaseScript.SelectorAnimatorNodes[c].Animator;
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
            }
            DOTweenEditorPreview.Stop();
        }


        /// <summary>
        /// 延迟停止
        /// </summary>
        IEnumerator Preview_Animator_Coroutine_Stop(float stopdelay)
        {
            xHud_Manager mgr = xHud_Dashboard.HudManagerGet();

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
                for (int w = 0; w < BaseScript.SelectorAnimatorNodes.Count; w++)
                {
                    AnimatorNode a_node = BaseScript.SelectorAnimatorNodes[w];
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
            xHud_Manager mgr = xHud_Dashboard.HudManagerGet();

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
            xHud_AudioStoper sp = au.gameObject.AddComponent<xHud_AudioStoper>();
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

        #region 获取组件

        /// <summary>
        /// 扫描所有Animator动画
        /// </summary>
        private void GetAllAnimators()
        {
            if (IsMultiSelected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    SerializedObject so_ele = new SerializedObject(SelectedObjects[i]);
                    so_ele.Update();
                    SerializedProperty sp_nodes = so_ele.FindProperty("SelectorAnimatorNodes");
                    sp_nodes.ClearArray();
                    xHud_Module_Animator[] gettedAnims = SelectedObjects[i].GetComponentsInChildren<xHud_Module_Animator>();
                    for (int x = 0; x < gettedAnims.Length; x++)
                    {
                        bool repeat = false;

                        if (sp_nodes.arraySize > 0)
                        {
                            for (int s = 0; s < sp_nodes.arraySize; s++)
                            {
                                SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                                SerializedProperty sp_node_anim = sp_node.FindPropertyRelative("Animator");
                                xHud_Module_Animator anim = (xHud_Module_Animator)sp_node_anim.objectReferenceValue;
                                if (anim == gettedAnims[x])
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
                            SerializedProperty sp_Animator = sp_node.FindPropertyRelative("Animator");
                            sp_Animator.objectReferenceValue = gettedAnims[x];

                            sp_Animator.serializedObject.ApplyModifiedProperties();
                            sp_node.serializedObject.ApplyModifiedProperties();
                        }
                    }
                    sp_nodes.serializedObject.ApplyModifiedProperties();
                    so_ele.ApplyModifiedProperties();
                }
            }
            else
            {
                SerializedProperty sp_nodes = serializedObject.FindProperty("SelectorAnimatorNodes");
                sp_nodes.ClearArray();
                xHud_Module_Animator[] gettedAnims = BaseScript.GetComponentsInChildren<xHud_Module_Animator>();
                for (int i = 0; i < gettedAnims.Length; i++)
                {
                    bool repeat = false;

                    if (sp_nodes.arraySize > 0)
                    {
                        for (int s = 0; s < sp_nodes.arraySize; s++)
                        {
                            SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                            SerializedProperty sp_node_anim = sp_node.FindPropertyRelative("Animator");
                            xHud_Module_Animator sp_anim = (xHud_Module_Animator)sp_node_anim.objectReferenceValue;
                            if (sp_anim == gettedAnims[i])
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
                        SerializedProperty sp_Animator = sp_node.FindPropertyRelative("Animator");
                        sp_Animator.objectReferenceValue = gettedAnims[i];

                        sp_Animator.serializedObject.ApplyModifiedProperties();
                        sp_node.serializedObject.ApplyModifiedProperties();
                    }
                }
                sp_nodes.serializedObject.ApplyModifiedProperties();
            }
        }

        /// <summary>
        /// 扫描所有选项按钮
        /// </summary>
        private void GetAllOptionsButton()
        {
            if (IsMultiSelected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    SerializedObject so_ele = new SerializedObject(SelectedObjects[i]);
                    so_ele.Update();
                    SerializedProperty sp_nodes = so_ele.FindProperty("OptionButtonNodes");
                    SerializedProperty sp_optionindex = so_ele.FindProperty("OptionIndex");
                    SerializedProperty sp_currentOptionName = so_ele.FindProperty("CurrentOptionName");

                    xHud_Module_Button[] allbtns = SelectedObjects[i].GetComponentsInChildren<xHud_Module_Button>();

                    List<xHud_Module_Button> Filter = new List<xHud_Module_Button>();
                    for (int s = 0; s < allbtns.Length; s++)
                    {
                        if (allbtns[i].IsOptionButton)
                            Filter.Add(allbtns[s]);
                    }
                    if (Filter.Count <= 0)
                        continue;
                    xHud_Module_Button[] gettedBtns = Filter.ToArray();

                    for (int c = 0; c < gettedBtns.Length; c++)
                    {
                        bool repeat = false;

                        if (sp_nodes.arraySize > 0)
                        {
                            for (int s = 0; s < sp_nodes.arraySize; s++)
                            {
                                SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                                SerializedProperty sp_node_btn = sp_node.FindPropertyRelative("Button");
                                xHud_Module_Button sp_btn = (xHud_Module_Button)sp_node_btn.objectReferenceValue;
                                if (sp_btn == gettedBtns[c])
                                    repeat = true;
                            }
                        }

                        if (!repeat && gettedBtns[c].IsOptionButton)
                        {
                            int index = 0;

                            if (sp_nodes.arraySize <= 0)
                                index = 0;
                            else
                                index = sp_nodes.arraySize;

                            sp_nodes.InsertArrayElementAtIndex(index);

                            SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(index);
                            SerializedProperty sp_btn = sp_node.FindPropertyRelative("Button");
                            SerializedProperty sp_indicator = sp_node.FindPropertyRelative("Indicator");
                            SerializedProperty sp_isoptional = sp_node.FindPropertyRelative("IsOptional");

                            sp_btn.objectReferenceValue = gettedBtns[c];
                            sp_btn.serializedObject.ApplyModifiedProperties();

                            sp_indicator.stringValue = gettedBtns[c].Indicator;
                            sp_indicator.serializedObject.ApplyModifiedProperties();

                            sp_isoptional.boolValue = gettedBtns[c].IsOptionButton;
                            sp_isoptional.serializedObject.ApplyModifiedProperties();

                            sp_node.serializedObject.ApplyModifiedProperties();
                        }
                    }

                    sp_optionindex.intValue = 0;
                    sp_optionindex.serializedObject.ApplyModifiedProperties();

                    SerializedProperty sp_nodesin = sp_nodes.GetArrayElementAtIndex(sp_optionindex.intValue).FindPropertyRelative("Button");
                    xHud_Module_Button sp_s_btn = (xHud_Module_Button)sp_nodesin.objectReferenceValue;
                    sp_currentOptionName.stringValue = sp_s_btn.Indicator;
                    sp_currentOptionName.serializedObject.ApplyModifiedProperties();

                    sp_nodes.serializedObject.ApplyModifiedProperties();
                    so_ele.ApplyModifiedProperties();
                }
            }
            else
            {
                OptionButtonNodes.ClearArray();
                OptionButtonNodes.serializedObject.ApplyModifiedProperties();

                SerializedProperty sp_nodes = serializedObject.FindProperty("OptionButtonNodes");

                xHud_Module_Button[] allbtns = BaseScript.GetComponentsInChildren<xHud_Module_Button>();

                List<xHud_Module_Button> Filter = new List<xHud_Module_Button>();
                for (int i = 0; i < allbtns.Length; i++)
                {
                    if (allbtns[i].IsOptionButton)
                        Filter.Add(allbtns[i]);
                }
                if (Filter.Count <= 0)
                    return;
                xHud_Module_Button[] gettedBtns = Filter.ToArray();
                for (int i = 0; i < gettedBtns.Length; i++)
                {
                    bool repeat = false;

                    if (sp_nodes.arraySize > 0)
                    {
                        for (int s = 0; s < sp_nodes.arraySize; s++)
                        {
                            SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                            SerializedProperty sp_node_btn = sp_node.FindPropertyRelative("Button");
                            xHud_Module_Button sp_btn = (xHud_Module_Button)sp_node_btn.objectReferenceValue;
                            if (sp_btn == gettedBtns[i])
                                repeat = true;
                        }
                    }

                    if (!repeat && gettedBtns[i].IsOptionButton)
                    {
                        int index;

                        if (sp_nodes.arraySize <= 0)
                            index = 0;
                        else
                            index = sp_nodes.arraySize;

                        sp_nodes.InsertArrayElementAtIndex(index);

                        SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(index);
                        SerializedProperty sp_Button = sp_node.FindPropertyRelative("Button");
                        SerializedProperty sp_Indicator = sp_node.FindPropertyRelative("Indicator");
                        SerializedProperty sp_IsOptional = sp_node.FindPropertyRelative("IsOptional");

                        sp_Button.objectReferenceValue = gettedBtns[i];
                        sp_Button.serializedObject.ApplyModifiedProperties();


                        sp_Indicator.stringValue = gettedBtns[i].Indicator;
                        sp_Indicator.serializedObject.ApplyModifiedProperties();

                        sp_IsOptional.boolValue = gettedBtns[i].IsOptionButton;
                        sp_IsOptional.serializedObject.ApplyModifiedProperties();

                        sp_node.serializedObject.ApplyModifiedProperties();
                    }
                }
                sp_nodes.serializedObject.ApplyModifiedProperties();

                SerializedProperty sp_optionindex = serializedObject.FindProperty("OptionIndex");
                SerializedProperty sp_currentOptionName = serializedObject.FindProperty("CurrentOptionName");

                sp_optionindex.intValue = 0;
                sp_optionindex.serializedObject.ApplyModifiedProperties();

                SerializedProperty sp_nodesin = sp_nodes.GetArrayElementAtIndex(sp_optionindex.intValue).FindPropertyRelative("Button");
                xHud_Module_Button sp_s_btn = (xHud_Module_Button)sp_nodesin.objectReferenceValue;
                sp_currentOptionName.stringValue = sp_s_btn.Indicator;
                sp_currentOptionName.serializedObject.ApplyModifiedProperties();
            }
        }

        /// <summary>
        /// 扫描动画器的结果报告
        /// </summary>
        private void GetAnimatorAndButtonResults()
        {
            List<xHudGUI_Dialog_ListDatas> Datas = new List<xHudGUI_Dialog_ListDatas>();
            if (IsMultiSelected())
            {
                for (int s = 0; s < SelectedObjects.Length; s++)
                {
                    xHud_Module_Option opt = SelectedObjects[s];
                    if (opt.SelectorAnimatorNodes.Count > 0)
                    {
                        for (int i = 0; i < opt.SelectorAnimatorNodes.Count; i++)
                        {
                            xHudGUI_Dialog_ListDatas dataitem = new xHudGUI_Dialog_ListDatas();
                            dataitem.Title = $"{opt.name} ( {opt.Indicator} )";
                            dataitem.SubTitle = $"扫描到动画器";
                            dataitem.Message = $"{opt.SelectorAnimatorNodes[i].Animator.name} ( {opt.SelectorAnimatorNodes[i].Animator.Indicator} )";
                            Datas.Add(dataitem);
                        }
                    }
                    if (opt.OptionButtonNodes.Count > 0)
                    {
                        for (int i = 0; i < opt.OptionButtonNodes.Count; i++)
                        {
                            xHudGUI_Dialog_ListDatas dataitem = new xHudGUI_Dialog_ListDatas();

                            if (!opt.OptionButtonNodes[i].IsOptional)
                                continue;
                            dataitem.Title = $"{opt.name} ( {opt.Indicator} )";
                            dataitem.SubTitle = $"扫描到选项按钮";
                            dataitem.Message = $"{opt.OptionButtonNodes[i].Button.name} ( {opt.OptionButtonNodes[i].Button.Indicator} )";
                            Datas.Add(dataitem);
                        }
                    }
                }

                if (Datas.Count <= 0)
                {
                    Editor_xHudGUI.Open(xHudDialogType.警告, "HudOption选项器消息", "扫描动选项器子组件", "未扫描到任何有效的子组件！", "明白");
                }
                else
                {
                    Editor_xHudGUI.Open(Datas.ToArray(), xHudDialogType.确认, "HudOption选项器消息", "扫描动选项器子组件", "以下是扫描到的所有选项子组件列表，请您检查核对：", "明白");
                }
            }
            else
            {
                if (BaseScript.SelectorAnimatorNodes.Count > 0)
                {
                    for (int i = 0; i < BaseScript.SelectorAnimatorNodes.Count; i++)
                    {
                        xHudGUI_Dialog_ListDatas dataitem = new xHudGUI_Dialog_ListDatas();
                        dataitem.Title = $"{BaseScript.name} ( {BaseScript.Indicator} )";
                        dataitem.SubTitle = $"扫描到动画器";
                        dataitem.Message = $"{BaseScript.SelectorAnimatorNodes[i].Animator.name} ( {BaseScript.SelectorAnimatorNodes[i].Animator.Indicator} )";
                        Datas.Add(dataitem);
                    }
                }
                if (BaseScript.OptionButtonNodes.Count > 0)
                {
                    for (int i = 0; i < BaseScript.OptionButtonNodes.Count; i++)
                    {
                        xHudGUI_Dialog_ListDatas dataitem = new xHudGUI_Dialog_ListDatas();

                        if (!BaseScript.OptionButtonNodes[i].IsOptional)
                            continue;
                        dataitem.Title = $"{BaseScript.name} ( {BaseScript.Indicator} )";
                        dataitem.SubTitle = $"扫描到选项按钮";
                        dataitem.Message = $"{BaseScript.OptionButtonNodes[i].Button.name} ( {BaseScript.OptionButtonNodes[i].Button.Indicator} )";
                        Datas.Add(dataitem);
                    }
                }

                if (Datas.Count <= 0)
                {
                    Editor_xHudGUI.Open(xHudDialogType.警告, "HudOption选项器消息", "扫描动选项器子组件", "未扫描到任何有效的子组件！", "明白");
                }
                else
                {
                    Editor_xHudGUI.Open(Datas.ToArray(), xHudDialogType.确认, "HudOption选项器消息", "扫描选项按钮组件", "以下是扫描到的所有选项按钮组件列表，请您检查核对：", "明白");
                }
            }
        }

        #endregion
    }
}