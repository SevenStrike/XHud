namespace SevenStrikeModules.XHud.Editor
{
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.Utilitys;
    using SevenStrikeModules.XTween;
    using System.Collections;
    using System.Collections.Generic;
    using Unity.EditorCoroutines.Editor;
    using UnityEditor;
    using UnityEditor.UI;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.UI;

    public class XHud_ModuleArg_Toggle
    {
        public Color Color_Normal;
        public Color Color_Highlight;
        public Color Color_Press;
        public Color Color_Select;
        public Color Color_Disable;
        public float ColorMultiplier;
        public float ColorFadeDuration;
        public bool IsOn;
        public string Indicator;
        public bool DebugState;
        public string ToggleName;
        public Color Tog_Color_Bg_Unchecked;
        public Color Tog_Color_Bg_Checked;
        public Color Tog_Color_Handle_Unchecked;
        public Color Tog_Color_Handle_Checked;
        public bool EaseMotion;
        public bool BgCanToggle;
        public bool TitleCanToggle;
        public float HandleProgress;
        public float HandleProgressDuration;
        public float ColorDuration;
        public float ToggleAnimatorSpeedMultiply;
        public EaseMode ProgressEase;
        public EaseMode ColorEase;
        public Vector2 HandlePosRange;
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(XHud_Module_Toggle), true)]
    public class Editor_XHud_Module_Toggle : ToggleEditor
    {
        #region 组件 / 列表
        private ReorderableList ToggleAnimatorList;
        private XHud_Module_Toggle BaseScript;
        #endregion

        /// <summary>
        /// 原始脚本参数显示开关
        /// </summary>
        private bool OriginalDisplay;

        private float LineHeight;

        #region 序列化属性
        private SerializedProperty sp_Indicator, ToggleAnimatorListIsFold, sp_debugstate, eve_on_Checked, eve_on_UnChecked, eve_on_ValueChanged, ChangingInterval, ToggleName, ToggleAnimatorMaxDuration, ToggleText, ToggleTmpText, Tog_Bg, AnimateState, BgCanToggle, TitleCanToggle, HandlePosRange, HandleProgress, Tog_Handle, Tog_Color_Bg_Unchecked, Tog_Color_Bg_Checked, Tog_Color_Handle_Unchecked, Tog_Color_Handle_Checked, HandleProgressDuration, ProgressEase, ColorDuration, ColorEase, EaseMotion, eve_on_Press, eve_on_Released, Toggle_Animators_GlobalDuration, ToggleEventIsFold, ToggleOriginalIsFold, ToggleAnimatorNodes, AutoStopPreview, ToggleIsChecked;
        #endregion

        #region 图标                                                                                                                                     
        private Texture2D icon_main, find_r, find_p, play_r, play_p, stop_r, stop_p, clear_r, clear_p, icon_button, animstate, dutation, longpressmarker, left_arrow_r, left_arrow_p, right_arrow_r, right_arrow_p, icon_anim;
        #endregion

        #region Preview - Animator
        private bool Preivew_Animator_PlayingState;
        private List<XTween_Interface> Preivew_Animator_TweenList = new List<XTween_Interface>();
        private List<EditorCoroutine> Preivew_Animator_CoroutineList_Play = new List<EditorCoroutine>();
        private EditorCoroutine Preivew_Animator_Coroutine_Stop;
        #endregion

        #region Preview - AnimatorSound
        public List<AudioSource> Preivew_AnimatorSound_SoundList = new List<AudioSource>();
        private EditorCoroutine Preivew_AnimatorSound_Coroutine_Play;
        private List<EditorCoroutine> Preivew_AnimatorSound_CoroutineList_Stop = new List<EditorCoroutine>();
        #endregion

        #region 批量模式查看索引
        private int ToggleStatu_Index;
        private int ToggleStatistic_Index;
        #endregion

        #region 选项文字
        string[] stroptions_enabled = new string[2] { "关闭", "开启" }, stroptions_debug = new string[2] { "关闭", "调试" }, stroptions_hide = new string[2] { "隐藏", "显示" };
        #endregion

        #region 批量化操作
        XHud_Module_Toggle[] SelectedObjects;

        private void Targets_Get()
        {
            if (targets.Length > 1)
            {
                SelectedObjects = new XHud_Module_Toggle[targets.Length];
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    var t = targets[i];
                    SelectedObjects[i] = (XHud_Module_Toggle)t;
                }
            }
            else
            {
                SelectedObjects = new XHud_Module_Toggle[targets.Length];
                SelectedObjects[0] = (XHud_Module_Toggle)target;
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

        protected override void OnEnable()
        {
            base.OnEnable();
            BaseScript = (XHud_Module_Toggle)target;

            // 获取序列化属性
            GetSerializeFields();

            #region 获取图标
            icon_main = Editor_XHud_GUI.GetIcon("Icons_XHud_Toggle/icon_main");
            find_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Toggle/find_r");
            find_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Toggle/find_p");
            play_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Toggle/play_r");
            play_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Toggle/play_p");
            stop_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Toggle/stop_r");
            stop_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Toggle/stop_p");
            clear_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Toggle/clear_r");
            clear_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Toggle/clear_p");
            icon_button = Editor_XHud_GUI.GetIcon("Icons_XHud_Toggle/icon_button");
            animstate = Editor_XHud_GUI.GetIcon("Icons_XHud_Toggle/animstate");
            dutation = Editor_XHud_GUI.GetIcon("Icons_XHud_Toggle/dutation");
            longpressmarker = Editor_XHud_GUI.GetIcon("Icons_XHud_Toggle/longpressmarker");
            left_arrow_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Toggle/left_arrow_r");
            left_arrow_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Toggle/left_arrow_p");
            right_arrow_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Toggle/right_arrow_r");
            right_arrow_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Toggle/right_arrow_p");
            icon_anim = Editor_XHud_GUI.GetIcon("Icons_XHud_Toggle/icon_anim");
            #endregion

            LineHeight = EditorGUIUtility.singleLineHeight;

            #region ReorderableList - ToggleAnimators
            ToggleAnimatorList = new ReorderableList(serializedObject, ToggleAnimatorNodes)
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

                    SerializedProperty sp_root = ToggleAnimatorNodes.GetArrayElementAtIndex(index);
                    SerializedProperty sp_animator = sp_root.FindPropertyRelative("Animator");
                    SerializedProperty sp_delay = sp_root.FindPropertyRelative("DelayTime");
                    XHud_Module_Animator animator = (XHud_Module_Animator)sp_animator.objectReferenceValue;

                    if (sp_animator.objectReferenceValue != null)
                    {
                        #region 标题
                        string title = "";
                        string indicator = animator.GetIndicator();
                        if (!string.IsNullOrEmpty(indicator))
                            title += indicator;
                        else
                            title += animator.gameObject.name;
                        Editor_XHud_GUI.Gui_Labelfield(new Rect(rect.width - (rect.width - 90), titleheight - 2, 180, LineHeight), title, HudFilled.无, HudColor.亮白, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 11, TextClipping.Clip);
                        #endregion

                        #region 类型图标         
                        GUI.color = XHud_Dashboard.Theme_Primary;
                        Editor_XHud_GUI.Gui_Icon(new Rect(rect.width - (rect.width - 60), titleheight + 1, 10, 10), icon_anim);
                        GUI.color = Color.white;
                        #endregion

                        #region 延迟
                        Editor_XHud_GUI.Gui_Property_Field(new Rect(rect.width - 5, rect.y + 4, 30, 19), "D", sp_delay, 10, 40, LineHeight, 15);
                        #endregion

                        #region 速率   
                        SerializedObject so_anim = new SerializedObject(animator);
                        so_anim.Update();

                        SerializedProperty sp_glodur = so_anim.FindProperty("Animator_GlobalDuration");
                        SerializedProperty sp_maxdur = so_anim.FindProperty("MaxTimerWithGlobalDuration");

                        Editor_XHud_GUI.Gui_Property_Field(new Rect(rect.width - 50, rect.y + 4, 30, 19), "G", sp_glodur, 10, 40, LineHeight, 15);

                        Editor_XHud_GUI.Gui_Labelfield_Thin(new Rect(rect.width - 80, rect.y + 4, 30, 19), $"{sp_maxdur.floatValue.ToString()} s", HudFilled.无, HudColor.无, XHud_Dashboard.Theme_Primary, TextAnchor.MiddleCenter, Vector2.zero, 11);
                        #endregion

                        sp_animator.serializedObject.ApplyModifiedProperties();
                        sp_root.serializedObject.ApplyModifiedProperties();
                    }
                    else
                    {
                        ToggleAnimatorNodes.DeleteArrayElementAtIndex(index);
                        ToggleAnimatorNodes.serializedObject.ApplyModifiedProperties();
                    }
                },
                onSelectCallback = (ReorderableList list) =>
                {
                    if (!Application.isPlaying)
                    {
                        Preview_Animator_Stop();
                    }

                    SerializedProperty sp_root = ToggleAnimatorNodes.GetArrayElementAtIndex(list.index);
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

            Targets_Get();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (!Application.isPlaying)
            {
                Preview_Animator_Stop();

                Preview_AnimatorSound_CoroutineList_Stop();
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            #region 标题
            if (string.IsNullOrEmpty(sp_Indicator.stringValue))
                Editor_XHud_GUI.Gui_Layout_Banner(icon_main, HudFilled.实体, HudColor.深空灰, "XHud - 开关", Color.white);
            else
                Editor_XHud_GUI.Gui_Layout_Banner(icon_main, HudFilled.实体, HudColor.深空灰, "XHud - 开关 -> ( " + sp_Indicator.stringValue + " )", Color.white);
            #endregion

            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

            #region 控制区
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "快捷功能", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            #region 扫描
            GUI.enabled = true;
            if (Editor_XHud_GUI.Gui_Layout_Button(14, "扫描动画组件", find_r, find_p))
            {
                GetAllAnimators();

                GetAnimatorResults();
                return;
            }
            #endregion

            #region 预览
            if (!Targets_Selected() && ToggleAnimatorNodes.arraySize > 0)
            {
                GUILayout.FlexibleSpace();

                #region 预览动画
                if (!Preivew_Animator_PlayingState)
                {
                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "播放所有动画器预览", play_r, play_p))
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
                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "停止所有动画器预览", stop_r, stop_p))
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

            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Horizontal_End();
            GUI.enabled = true;

            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 选项
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "选项", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_Toggle>("原生参数", stroptions_hide, ref ToggleOriginalIsFold, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_Toggle>("状态调试", stroptions_debug, ref sp_debugstate, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_Toggle>("自动停止预览", stroptions_enabled, ref AutoStopPreview, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_Toggle>("缓动模式", stroptions_enabled, ref EaseMotion, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_Toggle>("背景交互", stroptions_enabled, ref BgCanToggle, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_Toggle>("标题交互", stroptions_enabled, ref TitleCanToggle, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 原生参数
            if (ToggleOriginalIsFold.boolValue)
            {
                Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "原生", XHud_Dashboard.Theme_Primary);
                base.OnInspectorGUI();
                Editor_XHud_GUI.Gui_Layout_Vertical_End();
            }
            #endregion

            #region 参数
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "参数", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            Editor_XHud_GUI.Gui_Layout_Property_Field("文本组件", ToggleText, 100);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("文本(TMP)组件", ToggleTmpText, 100);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("开关背景", Tog_Bg, 100);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("开关控制柄", Tog_Handle, 100);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("值变化中步进值", ChangingInterval, 100);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("标识名称", sp_Indicator, 100);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("开关文字", ToggleName, 100);
            if (EditorGUI.EndChangeCheck())
            {
                if (Targets_Selected())
                {
                    for (int i = 0; i < SelectedObjects.Length; i++)
                    {
                        SerializedObject so_ele = new SerializedObject(SelectedObjects[i]);
                        SerializedProperty ele_sp_ToggleName = so_ele.FindProperty("ToggleName");
                        SerializedProperty ele_sp_ToggleText = so_ele.FindProperty("ToggleText");
                        SerializedProperty ele_sp_ToggleTmpText = so_ele.FindProperty("ToggleTmpText");

                        so_ele.Update();

                        if (ele_sp_ToggleText.objectReferenceValue != null)
                        {
                            XHud_Module_Text tt = (XHud_Module_Text)ele_sp_ToggleText.objectReferenceValue;
                            tt.text = ele_sp_ToggleName.stringValue;
                            ele_sp_ToggleText.serializedObject.ApplyModifiedProperties();
                        }
                        if (ele_sp_ToggleTmpText.objectReferenceValue != null)
                        {
                            XHud_Module_TmpText tt = (XHud_Module_TmpText)ele_sp_ToggleTmpText.objectReferenceValue;
                            tt.text = ele_sp_ToggleName.stringValue;
                            ele_sp_ToggleTmpText.serializedObject.ApplyModifiedProperties();
                        }

                        so_ele.ApplyModifiedProperties();
                    }
                }
                else
                {
                    if (ToggleText.objectReferenceValue != null)
                    {
                        XHud_Module_Text tt = (XHud_Module_Text)ToggleText.objectReferenceValue;
                        tt.text = ToggleName.stringValue;
                    }
                    if (ToggleTmpText.objectReferenceValue != null)
                    {
                        XHud_Module_TmpText tt = (XHud_Module_TmpText)ToggleTmpText.objectReferenceValue;
                        tt.text = ToggleName.stringValue;
                    }
                }
            }

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("控制柄范围", HandlePosRange, 100);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("控制柄进度", HandleProgress, 100);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("控制柄运动耗时", HandleProgressDuration, 100);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("控制柄运动缓动", ProgressEase, 100);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("速率倍增", Toggle_Animators_GlobalDuration, 100);

            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 颜色
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "颜色", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            Editor_XHud_GUI.Gui_Layout_Property_Field("开启时的背景", Tog_Color_Bg_Checked, 100);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("开启时的控制柄", Tog_Color_Handle_Checked, 100);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("关闭时的背景", Tog_Color_Bg_Unchecked, 100);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("关闭时的控制柄", Tog_Color_Handle_Unchecked, 100);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("缓动耗时", ColorDuration, 100);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("颜色缓动", ColorEase, 100);

            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 状态
            string statu_title = "状态";
            if (Targets_Selected())
                statu_title = "状态 - ( 批量模式 )";
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, statu_title, XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(5);

            if (!Targets_Selected())
            {
                #region 按钮状态              
                Editor_XHud_GUI.StatuDisplayer_text(icon_button, 12, new Vector2(0, 7), "开关状态", 12, ToggleIsChecked.boolValue ? "开启" : "关闭", XHud_Dashboard.Theme_Primary, 11);
                #endregion

                #region 动画相关
                if (ToggleAnimatorNodes != null && ToggleAnimatorNodes.arraySize > 0)
                {
                    #region 动画状态     
                    Editor_XHud_GUI.StatuDisplayer_text(animstate, 12, new Vector2(0, 7), "动画状态", 12, (HudElementAnimateState)AnimateState.enumValueIndex == HudElementAnimateState.Animating ? "动画中" : "静止状态", AnimateState.enumValueIndex == 1 ? XHud_Dashboard.Theme_Primary : Color.gray, 11);
                    #endregion

                    #region 最大耗时     
                    Editor_XHud_GUI.StatuDisplayer_text(dutation, 12, new Vector2(0, 7), "最大耗时<color=#909090>（速率倍增）</color>", 12, ToggleAnimatorMaxDuration.floatValue.ToString() + "秒", XHud_Dashboard.Theme_Primary, 11);
                    #endregion

                    #region 最大耗时        
                    Editor_XHud_GUI.StatuDisplayer_text(dutation, 12, new Vector2(0, 7), "最大耗时<color=#909090>（XHUD倍增）</color>", 12, (mgr.DurationMultiply * ToggleAnimatorMaxDuration.floatValue).ToString() + "秒", XHud_Dashboard.Theme_Primary, 11);
                    #endregion
                }
                #endregion

                Editor_XHud_GUI.Gui_Layout_Space(10);
            }
            else
            {
                #region 批量控件
                Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_XHud_GUI.Gui_Layout_Space(10);
                if (Editor_XHud_GUI.Gui_Layout_Button($"{SelectedObjects[ToggleStatu_Index].name} ( {SelectedObjects[ToggleStatu_Index].Indicator} )", "", HudFilled.透明, HudColor.无, Color.gray, 20))
                {
                    EditorGUIUtility.PingObject(SelectedObjects[ToggleStatu_Index]);
                }
                Editor_XHud_GUI.Gui_Layout_FlexSpace();
                if (Editor_XHud_GUI.Gui_Layout_Button(12, "", left_arrow_r, left_arrow_p))
                {
                    if (ToggleStatu_Index <= 0)
                    {
                        ToggleStatu_Index = SelectedObjects.Length - 1;
                    }
                    else
                    {
                        ToggleStatu_Index--;
                    }
                    EditorGUIUtility.PingObject(SelectedObjects[ToggleStatu_Index]);
                }
                Editor_XHud_GUI.Gui_Layout_Space(16);
                if (Editor_XHud_GUI.Gui_Layout_Button(12, "", right_arrow_r, right_arrow_p))
                {
                    if (ToggleStatu_Index >= SelectedObjects.Length - 1)
                    {
                        ToggleStatu_Index = 0;
                    }
                    else
                    {
                        ToggleStatu_Index++;
                    }
                    EditorGUIUtility.PingObject(SelectedObjects[ToggleStatu_Index]);
                }
                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                #endregion

                #region 开关状态
                Editor_XHud_GUI.StatuDisplayer_text(icon_button, 12, new Vector2(0, 7), "开关状态", 12, SelectedObjects[ToggleStatu_Index].ToggleIsChecked ? "开启" : "关闭", XHud_Dashboard.Theme_Primary, 11);
                #endregion

                #region 动画相关
                if (SelectedObjects[ToggleStatu_Index].ToggleAnimatorNodes != null && SelectedObjects[ToggleStatu_Index].ToggleAnimatorNodes.Count > 0)
                {
                    #region 动画状态     
                    Editor_XHud_GUI.StatuDisplayer_text(animstate, 12, new Vector2(0, 7), "动画状态", 12, SelectedObjects[ToggleStatu_Index].AnimateState == HudElementAnimateState.Animating ? "动画中" : "静止状态", SelectedObjects[ToggleStatu_Index].AnimateState == HudElementAnimateState.Animating ? XHud_Dashboard.Theme_Primary : Color.gray, 11);
                    #endregion

                    SelectedObjects[ToggleStatu_Index].ToggleAnimatorMaxDuration = Animators_GetAnimatorsMaxDuration(SelectedObjects[ToggleStatu_Index].ToggleAnimatorNodes, SelectedObjects[ToggleStatu_Index].Toggle_Animators_GlobalDuration);

                    #region 最大耗时     
                    Editor_XHud_GUI.StatuDisplayer_text(dutation, 12, new Vector2(0, 7), "最大耗时<color=#909090>（速率倍增）</color>", 12, SelectedObjects[ToggleStatu_Index].ToggleAnimatorMaxDuration.ToString() + "秒", XHud_Dashboard.Theme_Primary, 11);
                    #endregion

                    #region 最大耗时        
                    Editor_XHud_GUI.StatuDisplayer_text(dutation, 12, new Vector2(0, 7), "最大耗时<color=#909090>（XHUD倍增）</color>", 12, (mgr.DurationMultiply * SelectedObjects[ToggleStatu_Index].ToggleAnimatorMaxDuration).ToString() + "秒", XHud_Dashboard.Theme_Primary, 11);
                    #endregion
                }
                #endregion
            }

            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 统计
            string statu_statistic = "统计";
            if (Targets_Selected())
                statu_statistic = "统计 - ( 批量模式 )";
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, statu_statistic, XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(5);

            if (!Targets_Selected())
            {
                if (ToggleAnimatorNodes.arraySize <= 0)
                {
                    Editor_XHud_GUI.Gui_Layout_Labelfield("暂无统计数据", HudFilled.无, HudColor.无, Editor_XHud_GUI.GetColor(HudColor.阴影灰), TextAnchor.MiddleCenter);
                }
                else
                {
                    #region 组件数量 - 动画器
                    if (ToggleAnimatorNodes.arraySize > 0)
                    {
                        Editor_XHud_GUI.StatuDisplayer_text(icon_anim, 12, new Vector2(0, 7), "动画器", 12, ToggleAnimatorNodes.arraySize.ToString() + " 个", XHud_Dashboard.Theme_Primary, 11);
                    }
                    #endregion
                }
            }
            else
            {
                #region 批量控件
                Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_XHud_GUI.Gui_Layout_Space(10);
                if (Editor_XHud_GUI.Gui_Layout_Button($"{SelectedObjects[ToggleStatistic_Index].name} ( {SelectedObjects[ToggleStatistic_Index].Indicator} )", "", HudFilled.透明, HudColor.无, Color.gray, 20))
                {
                    EditorGUIUtility.PingObject(SelectedObjects[ToggleStatistic_Index]);
                }
                Editor_XHud_GUI.Gui_Layout_FlexSpace();
                if (Editor_XHud_GUI.Gui_Layout_Button(12, "", left_arrow_r, left_arrow_p))
                {
                    if (ToggleStatistic_Index <= 0)
                    {
                        ToggleStatistic_Index = SelectedObjects.Length - 1;
                    }
                    else
                    {
                        ToggleStatistic_Index--;
                    }
                    EditorGUIUtility.PingObject(SelectedObjects[ToggleStatistic_Index]);
                }
                Editor_XHud_GUI.Gui_Layout_Space(16);
                if (Editor_XHud_GUI.Gui_Layout_Button(12, "", right_arrow_r, right_arrow_p))
                {
                    if (ToggleStatistic_Index >= SelectedObjects.Length - 1)
                    {
                        ToggleStatistic_Index = 0;
                    }
                    else
                    {
                        ToggleStatistic_Index++;
                    }
                    EditorGUIUtility.PingObject(SelectedObjects[ToggleStatistic_Index]);
                }
                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                #endregion

                if (SelectedObjects[ToggleStatistic_Index].ToggleAnimatorNodes.Count <= 0)
                {
                    Editor_XHud_GUI.Gui_Layout_Labelfield("暂无统计数据", HudFilled.无, HudColor.无, Editor_XHud_GUI.GetColor(HudColor.阴影灰), TextAnchor.MiddleCenter);
                }
                else
                {
                    #region 组件数量 - 动画器
                    if (SelectedObjects[ToggleStatistic_Index].ToggleAnimatorNodes.Count > 0)
                    {
                        Editor_XHud_GUI.StatuDisplayer_text(icon_anim, 12, new Vector2(0, 7), "动画器", 12, SelectedObjects[ToggleStatistic_Index].ToggleAnimatorNodes.Count.ToString() + " 个", XHud_Dashboard.Theme_Primary, 11);
                    }
                    #endregion
                }
            }

            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 检查是否有无效的动画器
            CheckAnimatorsValid();
            #endregion

            #region 事件列表
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "事件/列表", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(5);

            #region 动画器列表
            if (ToggleAnimatorNodes.arraySize > 0)
            {
                if (Targets_Selected())
                {
                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(10);
                    EditorGUILayout.HelpBox("动画器列表不支持多项操作", MessageType.Warning);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                }
                else
                {
                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(10);
                    ToggleAnimatorListIsFold.boolValue = EditorGUILayout.Foldout(ToggleAnimatorListIsFold.boolValue, "动画器", true);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    if (ToggleAnimatorListIsFold.boolValue)
                    {
                        ToggleAnimatorList.DoLayoutList();
                    }
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                }
            }
            #endregion

            #region 事件列表
            if (Targets_Selected())
            {
                Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_XHud_GUI.Gui_Layout_Space(10);
                EditorGUILayout.HelpBox("事件不支持多项操作", MessageType.Warning);
                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Horizontal_End();
            }
            else
            {
                Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_XHud_GUI.Gui_Layout_Space(10);
                ToggleEventIsFold.boolValue = EditorGUILayout.Foldout(ToggleEventIsFold.boolValue, "事件", true);
                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                if (ToggleEventIsFold.boolValue)
                {
                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_Press);
                    eve_on_Press.serializedObject.ApplyModifiedProperties();
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_Released);
                    eve_on_Released.serializedObject.ApplyModifiedProperties();
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_Checked);
                    eve_on_Checked.serializedObject.ApplyModifiedProperties();
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_UnChecked);
                    eve_on_UnChecked.serializedObject.ApplyModifiedProperties();
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_ValueChanged);
                    eve_on_ValueChanged.serializedObject.ApplyModifiedProperties();
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                }
            }
            #endregion

            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 实时预览颜色

            Image img_bg = (Image)Tog_Bg.objectReferenceValue;
            Image img_handle = (Image)Tog_Handle.objectReferenceValue;

            if (HandleProgress.floatValue >= 1)
            {
                if (img_handle != null)
                    img_handle.color = Tog_Color_Handle_Checked.colorValue;
                if (img_bg != null)
                    img_bg.color = Tog_Color_Bg_Checked.colorValue;
            }
            else
            {
                if (img_handle != null)
                    img_handle.color = Tog_Color_Handle_Unchecked.colorValue;
                if (img_bg != null)
                    img_bg.color = Tog_Color_Bg_Unchecked.colorValue;
            }

            #endregion

            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                SerializedProperty sp_col_normal = serializedObject.FindProperty("m_Colors.m_NormalColor");
                SerializedProperty sp_col_highlight = serializedObject.FindProperty("m_Colors.m_HighlightedColor");
                SerializedProperty sp_col_press = serializedObject.FindProperty("m_Colors.m_PressedColor");
                SerializedProperty sp_col_selected = serializedObject.FindProperty("m_Colors.m_SelectedColor");
                SerializedProperty sp_col_disable = serializedObject.FindProperty("m_Colors.m_DisabledColor");
                SerializedProperty sp_col_mul = serializedObject.FindProperty("m_Colors.m_ColorMultiplier");
                SerializedProperty sp_col_dur = serializedObject.FindProperty("m_Colors.m_FadeDuration");

                SerializedProperty sp_Indicator = serializedObject.FindProperty("Indicator");
                SerializedProperty sp_debugstate = serializedObject.FindProperty("DebugState");
                SerializedProperty sp_ToggleName = serializedObject.FindProperty("ToggleName");

                SerializedProperty sp_Tog_Color_Bg_Unchecked = serializedObject.FindProperty("Tog_Color_Bg_Unchecked");
                SerializedProperty sp_Tog_Color_Bg_Checked = serializedObject.FindProperty("Tog_Color_Bg_Checked");
                SerializedProperty sp_Tog_Color_Handle_Unchecked = serializedObject.FindProperty("Tog_Color_Handle_Unchecked");
                SerializedProperty sp_Tog_Color_Handle_Checked = serializedObject.FindProperty("Tog_Color_Handle_Checked");

                SerializedProperty sp_Toggle_Animators_GlobalDuration = serializedObject.FindProperty("Toggle_Animators_GlobalDuration");

                SerializedProperty sp_HandlePosRange = serializedObject.FindProperty("HandlePosRange");
                SerializedProperty sp_HandleProgress = serializedObject.FindProperty("HandleProgress");
                SerializedProperty sp_HandleProgressDuration = serializedObject.FindProperty("HandleProgressDuration");
                SerializedProperty sp_ProgressEase = serializedObject.FindProperty("ProgressEase");
                SerializedProperty sp_ColorDuration = serializedObject.FindProperty("ColorDuration");

                SerializedProperty sp_ColorEase = serializedObject.FindProperty("ColorEase");
                SerializedProperty sp_EaseMotion = serializedObject.FindProperty("EaseMotion");

                SerializedProperty sp_BgCanToggle = serializedObject.FindProperty("BgCanToggle");
                SerializedProperty sp_TitleCanToggle = serializedObject.FindProperty("TitleCanToggle");

                // 创建右键菜单
                GenericMenu menu = new GenericMenu();
                menu.AddDisabledItem(new GUIContent("脚本参数"));
                if (!Targets_Selected())
                {
                    menu.AddItem(new GUIContent("C (拷贝)"), false, () =>
                    {
                        XHud_ModuleArg_Toggle htp = new XHud_ModuleArg_Toggle();

                        htp.Color_Normal = sp_col_normal.colorValue;
                        htp.Color_Highlight = sp_col_highlight.colorValue;
                        htp.Color_Press = sp_col_press.colorValue;
                        htp.Color_Select = sp_col_selected.colorValue;
                        htp.Color_Disable = sp_col_disable.colorValue;
                        htp.ColorMultiplier = sp_col_mul.floatValue;
                        htp.ColorFadeDuration = sp_col_dur.floatValue;

                        htp.Indicator = sp_Indicator.stringValue;
                        htp.DebugState = sp_debugstate.boolValue;
                        htp.ToggleName = sp_ToggleName.stringValue;
                        htp.Tog_Color_Bg_Unchecked = sp_Tog_Color_Bg_Unchecked.colorValue;
                        htp.Tog_Color_Bg_Checked = sp_Tog_Color_Bg_Checked.colorValue;
                        htp.Tog_Color_Handle_Unchecked = sp_Tog_Color_Handle_Unchecked.colorValue;
                        htp.Tog_Color_Handle_Checked = sp_Tog_Color_Handle_Checked.colorValue;

                        htp.ToggleAnimatorSpeedMultiply = sp_Toggle_Animators_GlobalDuration.floatValue;
                        htp.HandlePosRange = sp_HandlePosRange.vector2Value;
                        htp.HandleProgress = sp_HandleProgress.floatValue;
                        htp.HandleProgressDuration = sp_HandleProgressDuration.floatValue;
                        htp.ProgressEase = (EaseMode)sp_ProgressEase.enumValueIndex;
                        htp.ColorDuration = sp_ColorDuration.floatValue;
                        htp.ColorEase = (EaseMode)sp_ColorEase.enumValueIndex;
                        htp.EaseMotion = sp_EaseMotion.boolValue;
                        htp.BgCanToggle = sp_BgCanToggle.boolValue;
                        htp.TitleCanToggle = sp_TitleCanToggle.boolValue;

                        string json = JsonUtility.ToJson(htp);
                        GUIUtility.systemCopyBuffer = json;
                    });
                }
                menu.AddItem(new GUIContent("V (粘贴)"), false, () =>
                {
                    XHud_ModuleArg_Toggle htp = JsonUtility.FromJson<XHud_ModuleArg_Toggle>(GUIUtility.systemCopyBuffer);

                    if (!Targets_Selected())
                    {
                        sp_col_normal.colorValue = htp.Color_Normal;
                        sp_col_highlight.colorValue = htp.Color_Highlight;
                        sp_col_press.colorValue = htp.Color_Press;
                        sp_col_selected.colorValue = htp.Color_Select;
                        sp_col_disable.colorValue = htp.Color_Disable;
                        sp_col_mul.floatValue = htp.ColorMultiplier;
                        sp_col_dur.floatValue = htp.ColorFadeDuration;
                        sp_Indicator.stringValue = htp.Indicator;
                        sp_debugstate.boolValue = htp.DebugState;
                        sp_ToggleName.stringValue = htp.ToggleName;
                        sp_Tog_Color_Bg_Unchecked.colorValue = htp.Tog_Color_Bg_Unchecked;
                        sp_Tog_Color_Bg_Checked.colorValue = htp.Tog_Color_Bg_Checked;
                        sp_Tog_Color_Handle_Unchecked.colorValue = htp.Tog_Color_Handle_Unchecked;
                        sp_Tog_Color_Handle_Checked.colorValue = htp.Tog_Color_Handle_Checked;
                        sp_Toggle_Animators_GlobalDuration.floatValue = htp.ToggleAnimatorSpeedMultiply;
                        sp_HandlePosRange.vector2Value = htp.HandlePosRange;
                        sp_HandleProgress.floatValue = htp.HandleProgress;
                        sp_HandleProgressDuration.floatValue = htp.HandleProgressDuration;
                        sp_ProgressEase.enumValueIndex = (int)htp.ProgressEase;
                        sp_ColorDuration.floatValue = htp.ColorDuration;
                        sp_ColorEase.enumValueIndex = (int)htp.ColorEase;
                        sp_EaseMotion.boolValue = htp.EaseMotion;
                        sp_BgCanToggle.boolValue = htp.BgCanToggle;
                        sp_TitleCanToggle.boolValue = htp.TitleCanToggle;

                        sp_col_normal.serializedObject.ApplyModifiedProperties();
                        sp_col_highlight.serializedObject.ApplyModifiedProperties();
                        sp_col_press.serializedObject.ApplyModifiedProperties();
                        sp_col_selected.serializedObject.ApplyModifiedProperties();
                        sp_col_disable.serializedObject.ApplyModifiedProperties();
                        sp_col_mul.serializedObject.ApplyModifiedProperties();
                        sp_col_dur.serializedObject.ApplyModifiedProperties();
                        sp_Indicator.serializedObject.ApplyModifiedProperties();
                        sp_debugstate.serializedObject.ApplyModifiedProperties();
                        sp_ToggleName.serializedObject.ApplyModifiedProperties();
                        sp_Tog_Color_Bg_Unchecked.serializedObject.ApplyModifiedProperties();
                        sp_Tog_Color_Bg_Checked.serializedObject.ApplyModifiedProperties();
                        sp_Tog_Color_Handle_Unchecked.serializedObject.ApplyModifiedProperties();
                        sp_Tog_Color_Handle_Checked.serializedObject.ApplyModifiedProperties();
                        sp_Toggle_Animators_GlobalDuration.serializedObject.ApplyModifiedProperties();
                        sp_HandlePosRange.serializedObject.ApplyModifiedProperties();
                        sp_HandleProgress.serializedObject.ApplyModifiedProperties();
                        sp_HandleProgressDuration.serializedObject.ApplyModifiedProperties();
                        sp_ProgressEase.serializedObject.ApplyModifiedProperties();
                        sp_ColorDuration.serializedObject.ApplyModifiedProperties();
                        sp_ColorEase.serializedObject.ApplyModifiedProperties();
                        sp_EaseMotion.serializedObject.ApplyModifiedProperties();
                        sp_BgCanToggle.serializedObject.ApplyModifiedProperties();
                        sp_TitleCanToggle.serializedObject.ApplyModifiedProperties();
                    }
                    else
                    {
                        for (int i = 0; i < SelectedObjects.Length; i++)
                        {
                            SerializedObject so_tog = new SerializedObject(SelectedObjects[i]);

                            SerializedProperty c_sp_col_normal = so_tog.FindProperty("m_Colors.m_NormalColor");
                            SerializedProperty c_sp_col_highlight = so_tog.FindProperty("m_Colors.m_HighlightedColor");
                            SerializedProperty c_sp_col_press = so_tog.FindProperty("m_Colors.m_PressedColor");
                            SerializedProperty c_sp_col_selected = so_tog.FindProperty("m_Colors.m_SelectedColor");
                            SerializedProperty c_sp_col_disable = so_tog.FindProperty("m_Colors.m_DisabledColor");
                            SerializedProperty c_sp_col_mul = so_tog.FindProperty("m_Colors.m_ColorMultiplier");
                            SerializedProperty c_sp_col_dur = so_tog.FindProperty("m_Colors.m_FadeDuration");
                            SerializedProperty c_sp_Indicator = so_tog.FindProperty("Indicator");
                            SerializedProperty c_sp_debugstate = so_tog.FindProperty("DebugState");
                            SerializedProperty c_sp_ToggleName = so_tog.FindProperty("ToggleName");
                            SerializedProperty c_sp_Tog_Color_Bg_Unchecked = so_tog.FindProperty("Tog_Color_Bg_Unchecked");
                            SerializedProperty c_sp_Tog_Color_Bg_Checked = so_tog.FindProperty("Tog_Color_Bg_Checked");
                            SerializedProperty c_sp_Tog_Color_Handle_Unchecked = so_tog.FindProperty("Tog_Color_Handle_Unchecked");
                            SerializedProperty c_sp_Tog_Color_Handle_Checked = so_tog.FindProperty("Tog_Color_Handle_Checked");
                            SerializedProperty c_sp_Toggle_Animators_GlobalDuration = so_tog.FindProperty("Toggle_Animators_GlobalDuration");
                            SerializedProperty c_sp_HandlePosRange = so_tog.FindProperty("HandlePosRange");
                            SerializedProperty c_sp_HandleProgress = so_tog.FindProperty("HandleProgress");
                            SerializedProperty c_sp_HandleProgressDuration = so_tog.FindProperty("HandleProgressDuration");
                            SerializedProperty c_sp_ProgressEase = so_tog.FindProperty("ProgressEase");
                            SerializedProperty c_sp_ColorDuration = so_tog.FindProperty("ColorDuration");
                            SerializedProperty c_sp_ColorEase = so_tog.FindProperty("ColorEase");
                            SerializedProperty c_sp_EaseMotion = so_tog.FindProperty("EaseMotion");
                            SerializedProperty c_sp_BgCanToggle = so_tog.FindProperty("BgCanToggle");
                            SerializedProperty c_sp_TitleCanToggle = so_tog.FindProperty("TitleCanToggle");

                            c_sp_col_normal.colorValue = htp.Color_Normal;
                            c_sp_col_highlight.colorValue = htp.Color_Highlight;
                            c_sp_col_press.colorValue = htp.Color_Press;
                            c_sp_col_selected.colorValue = htp.Color_Select;
                            c_sp_col_disable.colorValue = htp.Color_Disable;
                            c_sp_col_mul.floatValue = htp.ColorMultiplier;
                            c_sp_col_dur.floatValue = htp.ColorFadeDuration;
                            c_sp_Indicator.stringValue = htp.Indicator;
                            c_sp_debugstate.boolValue = htp.DebugState;
                            c_sp_ToggleName.stringValue = htp.ToggleName;
                            c_sp_Tog_Color_Bg_Unchecked.colorValue = htp.Tog_Color_Bg_Unchecked;
                            c_sp_Tog_Color_Bg_Checked.colorValue = htp.Tog_Color_Bg_Checked;
                            c_sp_Tog_Color_Handle_Unchecked.colorValue = htp.Tog_Color_Handle_Unchecked;
                            c_sp_Tog_Color_Handle_Checked.colorValue = htp.Tog_Color_Handle_Checked;
                            c_sp_Toggle_Animators_GlobalDuration.floatValue = htp.ToggleAnimatorSpeedMultiply;
                            c_sp_HandlePosRange.vector2Value = htp.HandlePosRange;
                            c_sp_HandleProgress.floatValue = htp.HandleProgress;
                            c_sp_HandleProgressDuration.floatValue = htp.HandleProgressDuration;
                            c_sp_ProgressEase.enumValueIndex = (int)htp.ProgressEase;
                            c_sp_ColorDuration.floatValue = htp.ColorDuration;
                            c_sp_ColorEase.enumValueIndex = (int)htp.ColorEase;
                            c_sp_EaseMotion.boolValue = htp.EaseMotion;
                            c_sp_BgCanToggle.boolValue = htp.BgCanToggle;
                            c_sp_TitleCanToggle.boolValue = htp.TitleCanToggle;

                            c_sp_col_normal.serializedObject.ApplyModifiedProperties();
                            c_sp_col_highlight.serializedObject.ApplyModifiedProperties();
                            c_sp_col_press.serializedObject.ApplyModifiedProperties();
                            c_sp_col_selected.serializedObject.ApplyModifiedProperties();
                            c_sp_col_disable.serializedObject.ApplyModifiedProperties();
                            c_sp_col_mul.serializedObject.ApplyModifiedProperties();
                            c_sp_col_dur.serializedObject.ApplyModifiedProperties();
                            c_sp_Indicator.serializedObject.ApplyModifiedProperties();
                            c_sp_debugstate.serializedObject.ApplyModifiedProperties();
                            c_sp_ToggleName.serializedObject.ApplyModifiedProperties();
                            c_sp_Tog_Color_Bg_Unchecked.serializedObject.ApplyModifiedProperties();
                            c_sp_Tog_Color_Bg_Checked.serializedObject.ApplyModifiedProperties();
                            c_sp_Tog_Color_Handle_Unchecked.serializedObject.ApplyModifiedProperties();
                            c_sp_Tog_Color_Handle_Checked.serializedObject.ApplyModifiedProperties();
                            c_sp_Toggle_Animators_GlobalDuration.serializedObject.ApplyModifiedProperties();
                            c_sp_HandlePosRange.serializedObject.ApplyModifiedProperties();
                            c_sp_HandleProgress.serializedObject.ApplyModifiedProperties();
                            c_sp_HandleProgressDuration.serializedObject.ApplyModifiedProperties();
                            c_sp_ProgressEase.serializedObject.ApplyModifiedProperties();
                            c_sp_ColorDuration.serializedObject.ApplyModifiedProperties();
                            c_sp_ColorEase.serializedObject.ApplyModifiedProperties();
                            c_sp_EaseMotion.serializedObject.ApplyModifiedProperties();
                            c_sp_BgCanToggle.serializedObject.ApplyModifiedProperties();
                            c_sp_TitleCanToggle.serializedObject.ApplyModifiedProperties();

                        }
                    }

                });
                menu.AddSeparator("");
                menu.AddDisabledItem(new GUIContent("动画器"));
                menu.AddItem(new GUIContent("X (扫描)"), false, () =>
                {
                    if (Application.isPlaying)
                        return;

                    GetAllAnimators();

                    GetAnimatorResults();
                    return;
                });
                menu.AddSeparator("");
                menu.AddDisabledItem(new GUIContent("基础"));
                menu.AddItem(new GUIContent("A (自动标识)"), false, () =>
                {
                    if (Targets_Selected())
                    {
                        for (int i = 0; i < SelectedObjects.Length; i++)
                        {
                            XHud_Module_Toggle btn = SelectedObjects[i];
                            btn.Indicator = btn.gameObject.name;
                            EditorUtility.SetDirty(btn);
                        }
                    }
                    else
                    {
                        sp_Indicator.stringValue = BaseScript.gameObject.name;
                        sp_Indicator.serializedObject.ApplyModifiedProperties();
                        Editor_XHud_GUI.Open(XHud_DialogType.修改, "XHud - 开关消息", "标识自身", "将标识名称参数更改为物体的名称！", "明白");
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
                if (!Targets_Selected())
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

            CalculateAnimatorMaxDuration(BaseScript.ToggleAnimatorNodes, Toggle_Animators_GlobalDuration.floatValue);

            serializedObject.ApplyModifiedProperties();
        }

        #region 辅助
        /// <summary>
        /// 检查是否存在无效的动画器
        /// </summary>
        private void CheckAnimatorsValid()
        {
            for (int i = 0; i < ToggleAnimatorNodes.arraySize; i++)
            {
                SerializedProperty sp_animotrNode = ToggleAnimatorNodes.GetArrayElementAtIndex(i);
                SerializedProperty sp_animator = sp_animotrNode.FindPropertyRelative("Animator");

                if (sp_animator.objectReferenceValue == null)
                {
                    ToggleAnimatorNodes.DeleteArrayElementAtIndex(i);
                }
            }
        }
        private void CalculateAnimatorMaxDuration(List<ElementNode_Animator> list, float globaldur)
        {
            ToggleAnimatorMaxDuration.floatValue = Animators_GetAnimatorsMaxDuration(list, globaldur);
            ToggleAnimatorMaxDuration.serializedObject.ApplyModifiedProperties();
        }
        /// <summary>
        /// 从所有子动画器中获取最大耗时
        /// </summary>
        /// <returns></returns>
        public float Animators_GetAnimatorsMaxDuration(List<ElementNode_Animator> list, float globaldur)
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
            float v = XHud_Utilitys.Array_MaxValue(x_list);
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
            for (int i = 0; i < ToggleAnimatorNodes.arraySize; i++)
            {
                SerializedProperty sp_animator = ToggleAnimatorNodes.GetArrayElementAtIndex(i).FindPropertyRelative("Animator");
                XHud_Module_Animator anim = (XHud_Module_Animator)sp_animator.objectReferenceValue;
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
        private void AllListFoldState(bool state)
        {
            if (Targets_Selected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    if (SelectedObjects[i] == null)
                        continue;
                    SerializedObject so_ele = new SerializedObject(SelectedObjects[i]);
                    SerializedProperty sp_anim_isfold = so_ele.FindProperty("ToggleAnimatorListIsFold");
                    SerializedProperty sp_event_isfold = so_ele.FindProperty("ToggleEventIsFold");
                    so_ele.Update();

                    sp_anim_isfold.boolValue = state;
                    sp_event_isfold.boolValue = state;
                    sp_event_isfold.serializedObject.ApplyModifiedProperties();
                    sp_anim_isfold.serializedObject.ApplyModifiedProperties();

                    so_ele.ApplyModifiedProperties();
                }
            }
            else
            {
                if (target != null)
                {
                    ToggleEventIsFold.boolValue = state;
                    ToggleAnimatorListIsFold.boolValue = state;
                    ToggleEventIsFold.serializedObject.ApplyModifiedProperties();
                    ToggleAnimatorListIsFold.serializedObject.ApplyModifiedProperties();
                }
            }
        }

        #region 获取组件

        /// <summary>
        /// 扫描所有Animator动画
        /// </summary>
        private void GetAllAnimators()
        {
            if (Targets_Selected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    SerializedObject so_ele = new SerializedObject(SelectedObjects[i]);
                    SerializedProperty sp_nodes = so_ele.FindProperty("ToggleAnimatorNodes");
                    so_ele.Update();
                    sp_nodes.ClearArray();
                    XHud_Module_Animator[] gettedAnims = SelectedObjects[i].GetComponentsInChildren<XHud_Module_Animator>();
                    for (int x = 0; x < gettedAnims.Length; x++)
                    {
                        bool repeat = false;

                        if (sp_nodes.arraySize > 0)
                        {
                            for (int s = 0; s < sp_nodes.arraySize; s++)
                            {
                                SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                                SerializedProperty sp_node_anim = sp_node.FindPropertyRelative("Animator");
                                XHud_Module_Animator anim = (XHud_Module_Animator)sp_node_anim.objectReferenceValue;
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
                SerializedProperty sp_nodes = serializedObject.FindProperty("ToggleAnimatorNodes");
                sp_nodes.ClearArray();
                XHud_Module_Animator[] gettedAnims = BaseScript.GetComponentsInChildren<XHud_Module_Animator>();
                for (int i = 0; i < gettedAnims.Length; i++)
                {
                    bool repeat = false;

                    if (sp_nodes.arraySize > 0)
                    {
                        for (int s = 0; s < sp_nodes.arraySize; s++)
                        {
                            SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                            SerializedProperty sp_node_anim = sp_node.FindPropertyRelative("Animator");
                            XHud_Module_Animator sp_anim = (XHud_Module_Animator)sp_node_anim.objectReferenceValue;
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
        /// 扫描动画器的结果报告
        /// </summary>
        private void GetAnimatorResults()
        {
            List<XHud_GUI_Dialog_ListDatas> Datas = new List<XHud_GUI_Dialog_ListDatas>();
            if (Targets_Selected())
            {
                var s = targets;
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    XHud_Module_Toggle tog = SelectedObjects[i];
                    if (tog.ToggleAnimatorNodes.Count > 0)
                    {
                        for (int c = 0; c < tog.ToggleAnimatorNodes.Count; c++)
                        {
                            XHud_GUI_Dialog_ListDatas dataitem = new XHud_GUI_Dialog_ListDatas();

                            dataitem.Title = $"{tog.name} ( {tog.Indicator} )";
                            dataitem.SubTitle = $"扫描到动画器";
                            dataitem.Message = $"{tog.ToggleAnimatorNodes[c].Animator.name} ( {tog.ToggleAnimatorNodes[c].Animator.Indicator} )";

                            Datas.Add(dataitem);
                        }
                    }
                }
                Editor_XHud_GUI.Open(Datas.ToArray(), XHud_DialogType.确认, "XHud - 开关消息", "批量扫描动画器组件", "以下是批量扫描到的所有动画器组件列表，请您检查核对：", "明白");
            }
            else
            {
                if (BaseScript.ToggleAnimatorNodes.Count > 0)
                {
                    for (int i = 0; i < BaseScript.ToggleAnimatorNodes.Count; i++)
                    {
                        XHud_GUI_Dialog_ListDatas dataitem = new XHud_GUI_Dialog_ListDatas();

                        dataitem.Title = $"{BaseScript.name} ( {BaseScript.Indicator} )";
                        dataitem.SubTitle = $"扫描到动画器";
                        dataitem.Message = $"{BaseScript.ToggleAnimatorNodes[i].Animator.name} ( {BaseScript.ToggleAnimatorNodes[i].Animator.Indicator} )";
                        Datas.Add(dataitem);
                    }
                    Editor_XHud_GUI.Open(Datas.ToArray(), XHud_DialogType.确认, "XHud - 开关消息", "扫描动画器组件", "以下是扫描到的所有动画器组件列表，请您检查核对：", "明白");
                }
                else
                {
                    Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 开关消息", "扫描动画器组件", "未扫描到任何动画器组件！", "明白");
                }
            }
        }

        #endregion

        /// <summary>
        /// 还原Animator姿态
        /// </summary>
        private void OriginalPoseState_Load(XHud_Module_Animator animator)
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
                ((XHud_Module_Text)m_text.objectReferenceValue).color = col.colorValue;
                m_text.serializedObject.ApplyModifiedProperties();
            }
            if (m_tmptext.objectReferenceValue != null)
            {
                ((XHud_Module_TmpText)m_tmptext.objectReferenceValue).color = col.colorValue;
                m_tmptext.serializedObject.ApplyModifiedProperties();
            }

            so.ApplyModifiedProperties();
        }
        /// <summary>
        /// 获取序列化属性
        /// </summary>
        private void GetSerializeFields()
        {
            sp_Indicator = serializedObject.FindProperty("Indicator");
            sp_debugstate = serializedObject.FindProperty("DebugState");
            eve_on_Checked = serializedObject.FindProperty("eve_on_Checked");
            eve_on_UnChecked = serializedObject.FindProperty("eve_on_UnChecked");
            eve_on_ValueChanged = serializedObject.FindProperty("eve_on_ValueChanged");
            eve_on_Press = serializedObject.FindProperty("eve_on_Press");
            eve_on_Released = serializedObject.FindProperty("eve_on_Released");
            ToggleName = serializedObject.FindProperty("ToggleName");
            ToggleText = serializedObject.FindProperty("ToggleText");
            ToggleTmpText = serializedObject.FindProperty("ToggleTmpText");
            Tog_Bg = serializedObject.FindProperty("Tog_Bg");
            Tog_Handle = serializedObject.FindProperty("Tog_Handle");
            Tog_Color_Bg_Unchecked = serializedObject.FindProperty("Tog_Color_Bg_Unchecked");
            Tog_Color_Bg_Checked = serializedObject.FindProperty("Tog_Color_Bg_Checked");
            Tog_Color_Handle_Unchecked = serializedObject.FindProperty("Tog_Color_Handle_Unchecked");
            Tog_Color_Handle_Checked = serializedObject.FindProperty("Tog_Color_Handle_Checked");
            Toggle_Animators_GlobalDuration = serializedObject.FindProperty("Toggle_Animators_GlobalDuration");
            ToggleAnimatorNodes = serializedObject.FindProperty("ToggleAnimatorNodes");
            HandlePosRange = serializedObject.FindProperty("HandlePosRange");
            HandleProgress = serializedObject.FindProperty("HandleProgress");
            HandleProgressDuration = serializedObject.FindProperty("HandleProgressDuration");
            ProgressEase = serializedObject.FindProperty("ProgressEase");
            ColorEase = serializedObject.FindProperty("ColorEase");
            EaseMotion = serializedObject.FindProperty("EaseMotion");
            BgCanToggle = serializedObject.FindProperty("BgCanToggle");
            TitleCanToggle = serializedObject.FindProperty("TitleCanToggle");
            ColorDuration = serializedObject.FindProperty("ColorDuration");
            ToggleAnimatorMaxDuration = serializedObject.FindProperty("ToggleAnimatorMaxDuration");
            AnimateState = serializedObject.FindProperty("AnimateState");
            ToggleAnimatorListIsFold = serializedObject.FindProperty("ToggleAnimatorListIsFold");
            ToggleEventIsFold = serializedObject.FindProperty("ToggleEventIsFold");
            ToggleOriginalIsFold = serializedObject.FindProperty("ToggleOriginalIsFold");
            ChangingInterval = serializedObject.FindProperty("ChangingInterval");
            AutoStopPreview = serializedObject.FindProperty("AutoStopPreview");
            ToggleIsChecked = serializedObject.FindProperty("ToggleIsChecked");
        }
        #endregion

        #region Animator 动画预览

        /// <summary>
        /// 预览动画
        /// </summary>
        private void Preview_Animator_Play()
        {
            if (ToggleAnimatorNodes.arraySize <= 0)
                return;
            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

            Preivew_Animator_PlayingState = true;
            AnimateState.enumValueIndex = 1;
            AnimateState.serializedObject.ApplyModifiedProperties();

            ///---用来存放调用动画器的动画效果的延迟时间
            List<float> delaytimes = new List<float>();

            ///---动画逻辑
            for (int i = 0; i < BaseScript.ToggleAnimatorNodes.Count; i++)
            {
                ElementNode_Animator node = BaseScript.ToggleAnimatorNodes[i];
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
                            Preivew_Animator_TweenList.Add(node.Animator.Tweener_Play(twnnode, 1 * Toggle_Animators_GlobalDuration.floatValue * node.Animator.Animator_GlobalDuration));
                        else
                            Preivew_Animator_TweenList.Add(node.Animator.Tweener_Play(twnnode, mgr.DurationMultiply * Toggle_Animators_GlobalDuration.floatValue * node.Animator.Animator_GlobalDuration));
                        delaytimes.Add(node.DelayTime);
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            ///---预备动画器
            for (int i = 0; i < BaseScript.ToggleAnimatorNodes.Count; i++)
            {
                ElementNode_Animator node = BaseScript.ToggleAnimatorNodes[i];
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
                        Preivew_Animator_Coroutine_Stop = EditorCoroutineUtility.StartCoroutine(Preview_Animator_Coroutine_Stop(ToggleAnimatorMaxDuration.floatValue), this);
                }
                //DOTweenEditorPreview.Start();
            }
        }

        /// <summary>
        /// 延迟预览指定动画
        /// </summary>
        IEnumerator Preview_Animator_Coroutine_Play(XTween_Interface tween, float delay)
        {
            yield return new EditorWaitForSeconds(delay);
            //DOTweenEditorPreview.PrepareTweenForPreview(tween, true, true, true);
        }

        /// <summary>
        /// 预览动画
        /// </summary>
        private void Preview_Animator_PlayAt(int index)
        {
            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

            Preview_Animator_Stop();

            ElementNode_Animator node = BaseScript.ToggleAnimatorNodes[index];
            XHud_Module_Animator anim = node.Animator;

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
                //DOTweenEditorPreview.Start();
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
                            //Preivew_Animator_TweenList[i].Complete();
                            Preivew_Animator_TweenList[i].Kill();
                            Preivew_Animator_TweenList[i].Rewind();
                        }
                    }
                }
                #endregion

                #region 复位所有节点的动画
                if (BaseScript.ToggleAnimatorNodes != null && BaseScript.ToggleAnimatorNodes.Count > 0)
                {
                    for (int c = 0; c < BaseScript.ToggleAnimatorNodes.Count; c++)
                    {
                        XHud_Module_Animator animator = BaseScript.ToggleAnimatorNodes[c].Animator;
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
            //DOTweenEditorPreview.Stop();
        }


        /// <summary>
        /// 延迟停止
        /// </summary>
        IEnumerator Preview_Animator_Coroutine_Stop(float stopdelay)
        {
            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

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
                for (int w = 0; w < BaseScript.ToggleAnimatorNodes.Count; w++)
                {
                    ElementNode_Animator a_node = BaseScript.ToggleAnimatorNodes[w];
                    for (int i = 0; i < a_node.Animator.AnimateTweenNodes.Count; i++)
                    {
                        TweenNode tweenNode = a_node.Animator.AnimateTweenNodes[i];
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
            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

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
            XHud_AudioStoper sp = au.gameObject.AddComponent<XHud_AudioStoper>();
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
    }
}