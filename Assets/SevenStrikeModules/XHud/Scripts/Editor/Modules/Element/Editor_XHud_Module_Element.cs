namespace SevenStrikeModules.XHud
{
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.GuiLib;
    using SevenStrikeModules.XHud.Utilitys;
    using SevenStrikeModules.XTween;
    using System.Collections;
    using System.Collections.Generic;
    using Unity.EditorCoroutines.Editor;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.UI;
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
    [CustomEditor(typeof(XHud_Module_Element), true)]
    public partial class Editor_XHud_Module_Element : Editor
    {
        #region 组件 / 列表
        private XHud_Module_Element BaseScript;
        #endregion

        private float LineHeight;
        /// <summary>
        /// 原始脚本参数显示开关
        /// </summary>
        private bool OriginalDisplay;

        #region 序列化属性
        private SerializedProperty
            CanvasGroup, SounderNodes, AnimatorNodes, ContainerNodes, OptionNodes, ButtonNodes, TextIsFold, TmpTextIsFold, SliderNodes, TextNodes, TmpTextNodes, DebugState, PreviewPrimitivesTween, ProgressNodes, ToggleNodes, Indicator, AnimatorsMaxDuration, RectTransform, TriggerAction, ObjectTracker, CreateState, AnimateState, Element_Animators_GlobalDuration, OriginPoolName, AnimatorsIsFold, ButtonIsFold, OptionIsFold, SliderIsFold, SounderIsFold, ProgressIsFold, ContainerIsFold, ToggleIsFold, EventIsFold, AutoPlayAnimators, AutoPlayContainersAnimators, Tween_Preview_AutoStop, Alpha, RMS_Enabled, RMS_LayoutDatas, RMS_Name, CurrentPivot, OriginalName, eve_on_element_in_start, eve_on_element_in_end, eve_on_element_out_start, eve_on_element_out_end, TweensPreivew_In_State, TweensPreivew_Out_State, PrimitivePreivew_State;
        #endregion

        #region Preview - Animator
        private bool Tween_Preivewing_Animators;
        private List<XTween_Interface> Preivew_Animator_TweenList = new List<XTween_Interface>();
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
        private Texture2D icon_main, icon_sound, icon_anim, icon_button, icon_option, icon_slider, icon_progress, icon_container, icon_toggle, icon_text, icon_tmptext, icon_scan_r, icon_scan_p, icon_record_rms_r, icon_record_rms_p, resetanchorpos_r, resetanchorpos_p, prw_play_r, prw_play_p, prw_stop_r, prw_stop_p, usestate, animstate, dutation, comp_tracking, elelibsource, locate_r, locate_p, rms_move, rms_rotate, rms_anchor, rms_anchor_center, rms_scale, comp_alpha, comp_transform, comp_trigger, status, left_arrow_r, left_arrow_p, right_arrow_r, right_arrow_p, prw_play_out_r, prw_play_out_p, prw_play_in_p, prw_play_in_r;
        #endregion

        #region 批量模式查看索引
        private int ElementStatu_Index;
        private int ElementStatistic_Index;
        #endregion

        #region 选项文字
        string[] stroptions_enabled = new string[2] { "关闭", "开启" }, stroptions_debug = new string[2] { "关闭", "调试" }, stroptions_auto = new string[2] { "手动", "自动" }, stroptions_tweenmode = new string[2] { "元素", "图元" };
        #endregion

        #region 批量化操作

        private XHud_Module_Element[] SelectedObjects;

        private void Targets_Get()
        {
            if (targets.Length > 1)
            {
                SelectedObjects = new XHud_Module_Element[targets.Length];
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    var t = targets[i];
                    SelectedObjects[i] = (XHud_Module_Element)t;
                }
            }
            else
            {
                SelectedObjects = new XHud_Module_Element[targets.Length];
                SelectedObjects[0] = (XHud_Module_Element)target;
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
            BaseScript = (XHud_Module_Element)target;

            // 获取序列化字段
            GetSerializeFields();

            // 获取组件
            GetComponents();

            LineHeight = EditorGUIUtility.singleLineHeight;

            Vector2 ButtonSize = new Vector2(14, 14);

            ClearEmptyNodes();

            #region 图标获取
            icon_main = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/icon_main");
            icon_sound = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/icon_sound");
            icon_button = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/icon_button");
            icon_option = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/icon_option");
            icon_slider = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/icon_slider");
            icon_progress = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/icon_progress");
            icon_container = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/icon_container");
            icon_toggle = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/icon_toggle");
            icon_text = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/icon_text");
            icon_tmptext = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/icon_tmptext");
            icon_anim = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/icon_anim");
            icon_scan_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/icon_scan_r");
            icon_scan_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/icon_scan_p");
            icon_record_rms_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/icon_record_rms_r");
            icon_record_rms_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/icon_record_rms_p");
            resetanchorpos_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/resetanchorpos_r");
            resetanchorpos_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/resetanchorpos_p");
            prw_play_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/prw_play_r");
            prw_play_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/prw_play_p");
            prw_stop_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/prw_stop_r");
            prw_stop_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/prw_stop_p");
            usestate = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/usestate");
            animstate = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/animstate");
            dutation = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/dutation");
            elelibsource = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/elelibsource");
            locate_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/locate_r");
            locate_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/locate_p");
            rms_move = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/rms_move");
            rms_rotate = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/rms_rotate");
            rms_anchor = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/rms_anchor");
            rms_scale = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/rms_scale");
            rms_anchor_center = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/rms_anchor_center");
            comp_alpha = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/comp_alpha");
            comp_transform = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/comp_transform");
            comp_trigger = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/comp_trigger");
            comp_tracking = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/comp_tracking");
            status = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/status");
            left_arrow_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/left_arrow_r");
            left_arrow_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/left_arrow_p");
            right_arrow_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/right_arrow_r");
            right_arrow_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/right_arrow_p");
            prw_play_out_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/prw_play_out_r");
            prw_play_out_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/prw_play_out_p");
            prw_play_in_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/prw_play_in_p");
            prw_play_in_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/prw_play_in_r");

            #endregion

            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

            // 控件列表绘制
            ReorderableList_Draw_Sounder(mgr);
            ReorderableList_Draw_Animator(mgr);
            ReorderableList_Draw_Button(mgr);
            ReorderableList_Draw_Option(mgr);
            ReorderableList_Draw_Slider(mgr);
            ReorderableList_Draw_Progress(mgr);
            ReorderableList_Draw_Container(mgr);
            ReorderableList_Draw_Toggle(mgr);
            ReorderableList_Draw_Text(mgr);
            ReorderableList_Draw_TmpText(mgr);

            // RMS 残留信息清理
            if (mgr != null)
            {
                if (mgr.hm_RMS_GetResolutionNodes().Length <= 0 && RMS_LayoutDatas.arraySize > 0)
                {
                    string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 元素消息", "RMS残留信息", "发现残留匹配分辨率信息列表，是否需要清空？请谨慎此操作！", "清空", "暂不", 1);
                    if (res == "清空")
                    {
                        RMS_LayoutDatas.ClearArray();
                        RMS_LayoutDatas.serializedObject.ApplyModifiedProperties();
                    }
                }
            }

            Targets_Get();
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

                    StopAllPreviewState();
                }
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (string.IsNullOrEmpty(Indicator.stringValue))
                Editor_XHud_GUI.Gui_Layout_Banner(icon_main, HudFilled.实体, HudColor.深空灰, "XHud - 元素", Color.white);
            else
                Editor_XHud_GUI.Gui_Layout_Banner(icon_main, HudFilled.实体, HudColor.深空灰, "XHud - 元素 -> ( " + Indicator.stringValue + " )", Color.white);

            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

            #region 快捷功能
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "快捷功能", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            #region 扫描子元素
            GUI.enabled = true;
            if (Editor_XHud_GUI.Gui_Layout_Button(15, "扫描子元素", icon_scan_r, icon_scan_p))
            {
                if (Application.isPlaying)
                {
                    Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 元素消息", "扫描子元素", "程序正在运行，无法在运行期间执行此功能！", "明白");
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
                if (Editor_XHud_GUI.Gui_Layout_Button(15, "记录RMS布局信息", icon_record_rms_r, icon_record_rms_p))
                {
                    if (Application.isPlaying)
                    {
                        Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 元素消息", "记录RMS布局信息", "程序正在运行，无法在运行期间执行此功能！", "明白");
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
            if (Editor_XHud_GUI.Gui_Layout_Button(15, "元素重置到锚点初始位置", resetanchorpos_r, resetanchorpos_p))
            {
                if (Application.isPlaying)
                {
                    Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 元素消息", "锚点初始化", "程序正在运行，无法在运行期间执行此功能！", "明白");
                    return;
                }

                RestoreToAnchorPosition();

                return;
            }
            #endregion

            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Horizontal_End();
            GUI.enabled = true;

            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 预览
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "预览", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            if (!PreviewPrimitivesTween.boolValue)
            {
                // 动画预览按钮
                if (!TweensPreivew_In_State.boolValue)
                {
                    if (Editor_XHud_GUI.Gui_Layout_Button(15, "元素 - 进入 - 播放", prw_play_in_r, prw_play_in_p))
                    {
                        ElementTweens_Preview_In_Play();
                    }
                }
                else
                {
                    if (Editor_XHud_GUI.Gui_Layout_Button(15, "元素 - 进入 - 停止", prw_play_in_p, prw_play_in_p))
                    {
                        ElementTweens_Preview_In_Stop();
                    }
                }

                Editor_XHud_GUI.Gui_Layout_Space(30);

                // 动画预览按钮
                if (!TweensPreivew_Out_State.boolValue)
                {
                    if (Editor_XHud_GUI.Gui_Layout_Button(15, "元素 - 退出 - 播放", prw_play_out_r, prw_play_out_p))
                    {
                        ElementTweens_Preview_Out_Play();
                    }
                }
                else
                {
                    if (Editor_XHud_GUI.Gui_Layout_Button(15, "元素 - 退出 - 停止", prw_play_out_p, prw_play_out_p))
                    {
                        ElementTweens_Preview_Out_Stop();
                    }
                }
                Editor_XHud_GUI.Gui_Layout_Space(20);
            }
            else
            {
                // 动画预览按钮
                if (!PrimitivePreivew_State.boolValue)
                {
                    if (Editor_XHud_GUI.Gui_Layout_Button(15, "图元 - 播放", prw_play_r, prw_play_p))
                    {
                        PrimitiveTweens_Preview_Play();
                    }
                }
                else
                {
                    if (Editor_XHud_GUI.Gui_Layout_Button(15, "图元 - 停止", prw_stop_r, prw_stop_p))
                    {
                        PrimitiveTweens_Preview_Stop();
                    }
                }
                Editor_XHud_GUI.Gui_Layout_Space(65);
            }

            TweensPreivew_In_State.serializedObject.ApplyModifiedProperties();
            TweensPreivew_Out_State.serializedObject.ApplyModifiedProperties();
            PrimitivePreivew_State.serializedObject.ApplyModifiedProperties();

            // 动画预览模式选择
            EditorGUI.BeginChangeCheck();
            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_Element>("", stroptions_tweenmode, ref PreviewPrimitivesTween, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
            if (EditorGUI.EndChangeCheck())
            {
                if (Targets_Selected())
                {
                    for (int i = 0; i < SelectedObjects.Length; i++)
                    {
                        SelectedObjects[i].TweensPreivew_In_State = false;
                        SelectedObjects[i].TweensPreivew_Out_State = false;
                        SelectedObjects[i].PrimitivePreivew_State = false;
                    }
                }
                else
                {
                    TweensPreivew_In_State.boolValue = false;
                    TweensPreivew_Out_State.boolValue = false;
                    PrimitivePreivew_State.boolValue = false;

                    TweensPreivew_In_State.serializedObject.ApplyModifiedProperties();
                    TweensPreivew_Out_State.serializedObject.ApplyModifiedProperties();
                    PrimitivePreivew_State.serializedObject.ApplyModifiedProperties();
                }
            }

            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Horizontal_End();

            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 参数
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "参数", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(5);

            ///---标识名称     
            EditorGUI.BeginChangeCheck();
            Editor_XHud_GUI.Gui_Layout_Property_Field("标识名称", Indicator);
            if (EditorGUI.EndChangeCheck())
            {
                if (Targets_Selected())
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

            Editor_XHud_GUI.Gui_Layout_Space(5);

            ///---透明度_Alpha     
            EditorGUI.BeginChangeCheck();
            Editor_XHud_GUI.Gui_Layout_Property_Field("透明度", Alpha);
            if (EditorGUI.EndChangeCheck())
            {
                ChangeAlpha();
            }

            Editor_XHud_GUI.Gui_Layout_Space(5);

            ///---动画倍乘系数     
            Editor_XHud_GUI.Gui_Layout_Property_Field("速率倍增", Element_Animators_GlobalDuration);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            ///---锚点偏移     
            Editor_XHud_GUI.Gui_Layout_Property_Field("当前锚点", CurrentPivot);

            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 选项
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "选项", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(5);

            #region 状态调试
            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_Element>("状态调试", stroptions_debug, ref DebugState, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
            #endregion

            #region 自动停止预览
            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_Element>("自动停止预览", stroptions_enabled, ref Tween_Preview_AutoStop, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
            #endregion

            #region 元素下动画器自动播放
            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_Element>("动画器自动播放", stroptions_auto, ref AutoPlayAnimators, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
            #endregion

            #region 元素下容器动画自动播放
            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_Element>("容器动画自动播放", stroptions_auto, ref AutoPlayContainersAnimators, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
            #endregion

            #region 生成时使用设计布局
            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_Element>("使用设计布局", stroptions_enabled, ref RMS_Enabled, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
            #endregion

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
                #region 使用状态     
                Editor_XHud_GUI.StatuDisplayer_text(usestate, 12, new Vector2(0, 7), "使用状态", 12, (HudElementCreateState)CreateState.enumValueIndex == HudElementCreateState.Created ? "已被生成" : "已被回收", CreateState.boolValue ? XHud_Dashboard.Theme_Primary : Color.gray, 11);
                #endregion

                #region 动画相关
                if (AnimatorNodes != null && AnimatorNodes.arraySize > 0)
                {
                    #region 动画状态     
                    Editor_XHud_GUI.StatuDisplayer_text(animstate, 12, new Vector2(0, 7), "动画状态", 12, (HudElementAnimateState)AnimateState.enumValueIndex == HudElementAnimateState.Animating ? "动画中" : "静止状态", AnimateState.enumValueIndex == 1 ? XHud_Dashboard.Theme_Primary : Color.gray, 11);
                    #endregion

                    #region 最大耗时     
                    Editor_XHud_GUI.StatuDisplayer_text(dutation, 12, new Vector2(0, 7), "最大耗时<color=#909090>（速率倍增）</color>", 12, AnimatorsMaxDuration.floatValue.ToString() + " 秒", XHud_Dashboard.Theme_Primary, 11);
                    #endregion

                    #region 最大耗时        
                    if (mgr != null)
                        Editor_XHud_GUI.StatuDisplayer_text(dutation, 12, new Vector2(0, 7), "最大耗时<color=#909090>（XHUD倍增）</color>", 12, (mgr.DurationMultiply * AnimatorsMaxDuration.floatValue).ToString() + " 秒", XHud_Dashboard.Theme_Primary, 11);
                    #endregion
                }
                #endregion

                #region 物体跟踪
                if (ObjectTracker.objectReferenceValue != null)
                {
                    XHud_ObjectTracker tracker = (XHud_ObjectTracker)ObjectTracker.objectReferenceValue;
                    if (tracker.SelfObject != null && tracker.TargetObject != null)
                    {
                        Editor_XHud_GUI.StatuDisplayer_text(comp_tracking, 12, new Vector2(0, 7), "物体跟踪状态", 12, tracker.SelfObject.name + "  >  " + tracker.TargetObject.name, XHud_Dashboard.Theme_Primary, 11);
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
                        foreach (XHud_Library_Element lib in mgr.Hud_ElementLibrarys)
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

                    if (Editor_XHud_GUI.StatuDisplayer_btn(elelibsource, 12, new Vector2(0, 7), "元素库源", 12, ElementInLib ? OriginPoolName.stringValue : "非元素库资源", XHud_Dashboard.Theme_Primary, 11, XHud_Dashboard.Theme_Primary, 14, "定位到元素库", locate_r, locate_p, 4))
                    {
                        if (!ElementInLib)
                        {
                            Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 元素消息", "定位元素", "此元素并非由元素库生成，无法为其进行定位！", "明白");
                            return;
                        }
                        //打开目标元素库
                        Editor_XHud_MenuItemsAction_OpenLibrary.open_target_elements(OriginPoolName.stringValue).ElementLibrary_Location_Find(OriginalName.stringValue);
                    }
                }
                else
                {
                    //编辑器模式下，通过遍历所有元素库来找到当前这个元素在哪个库里
                }
                #endregion

                Editor_XHud_GUI.Gui_Layout_Space(10);
                EditorGUILayout.HelpBox(RMS_Enabled.boolValue ? "此元素在生成时会优先使用自身记录的设计布局信息来进行定位和位置尺寸的匹配" : "如果启用设计布局那么在用代码调用生成时则会主动使用该元素自身记录的设计布局信息来进行定位和位置尺寸等相关参数的匹配（但前提是在您制作好此元素时需要手动点击\"记录布局信息\"按钮）", MessageType.Info);
            }
            else
            {
                #region 批量控件
                Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_XHud_GUI.Gui_Layout_Space(10);
                if (Editor_XHud_GUI.Gui_Layout_Button($"{SelectedObjects[ElementStatu_Index].name} ( {SelectedObjects[ElementStatu_Index].Indicator} )", "", HudFilled.透明, HudColor.无, Color.gray, 20))
                {
                    EditorGUIUtility.PingObject(SelectedObjects[ElementStatu_Index]);
                }
                Editor_XHud_GUI.Gui_Layout_FlexSpace();
                if (Editor_XHud_GUI.Gui_Layout_Button(12, "", left_arrow_r, left_arrow_p))
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
                Editor_XHud_GUI.Gui_Layout_Space(16);
                if (Editor_XHud_GUI.Gui_Layout_Button(12, "", right_arrow_r, right_arrow_p))
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
                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                #endregion

                #region 使用状态     
                Editor_XHud_GUI.StatuDisplayer_text(usestate, 12, new Vector2(0, 7), "使用状态", 12, SelectedObjects[ElementStatu_Index].CreateState == HudElementCreateState.Created ? "已被生成" : "已被回收", CreateState.boolValue ? XHud_Dashboard.Theme_Primary : Color.gray, 11);
                #endregion

                #region 动画相关
                if (SelectedObjects[ElementStatu_Index].AnimatorNodes != null && SelectedObjects[ElementStatu_Index].AnimatorNodes.Count > 0)
                {
                    #region 动画状态     
                    Editor_XHud_GUI.StatuDisplayer_text(animstate, 12, new Vector2(0, 7), "动画状态", 12, SelectedObjects[ElementStatu_Index].AnimateState == HudElementAnimateState.Animating ? "动画中" : "静止状态", AnimateState.enumValueIndex == 1 ? XHud_Dashboard.Theme_Primary : Color.gray, 11);
                    #endregion

                    SelectedObjects[ElementStatu_Index].AnimatorsMaxDuration = Animators_GetAnimatorsMaxDuration(SelectedObjects[ElementStatu_Index].AnimatorNodes, SelectedObjects[ElementStatu_Index].Element_Animators_GlobalDuration);

                    #region 最大耗时     
                    Editor_XHud_GUI.StatuDisplayer_text(dutation, 12, new Vector2(0, 7), "最大耗时<color=#909090>（元素倍增）</color>", 12, SelectedObjects[ElementStatu_Index].AnimatorsMaxDuration.ToString() + " 秒", XHud_Dashboard.Theme_Primary, 11);
                    #endregion

                    #region 最大耗时        
                    Editor_XHud_GUI.StatuDisplayer_text(dutation, 12, new Vector2(0, 7), "最大耗时<color=#909090>（XHUD倍增）</color>", 12, (mgr.DurationMultiply * SelectedObjects[ElementStatu_Index].AnimatorsMaxDuration).ToString() + " 秒", XHud_Dashboard.Theme_Primary, 11);
                    #endregion
                }
                #endregion

                #region 物体跟踪
                if (SelectedObjects[ElementStatu_Index].ObjectTracker != null)
                {
                    XHud_ObjectTracker tracker = SelectedObjects[ElementStatu_Index].ObjectTracker;
                    if (tracker.SelfObject != null && tracker.TargetObject != null)
                    {
                        Editor_XHud_GUI.StatuDisplayer_text(comp_tracking, 12, new Vector2(0, 7), "物体跟踪状态", 12, tracker.SelfObject.name + "  >  " + tracker.TargetObject.name, XHud_Dashboard.Theme_Primary, 11);
                    }
                }
                #endregion

                #region 元素库源
                bool ElementInLib = false;

                if (!string.IsNullOrEmpty(SelectedObjects[ElementStatu_Index].OriginPoolName))
                {
                    //遍历元素库集合找到目标元素库
                    foreach (XHud_Library_Element lib in mgr.Hud_ElementLibrarys)
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

                if (Editor_XHud_GUI.StatuDisplayer_btn(elelibsource, 12, new Vector2(0, 7), "元素库源", 12, ElementInLib ? SelectedObjects[ElementStatu_Index].OriginPoolName : "非元素库资源", XHud_Dashboard.Theme_Primary, 11, XHud_Dashboard.Theme_Primary, 14, "定位到元素库", locate_r, locate_p, 4))
                {
                    if (!ElementInLib)
                    {
                        Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 元素消息", "定位元素", "此元素并非由元素库生成，无法为其进行定位！", "明白");
                        return;
                    }
                    //打开目标元素库
                    Editor_XHud_MenuItemsAction_OpenLibrary.open_target_elements(SelectedObjects[ElementStatu_Index].OriginPoolName).ElementLibrary_Location_Find(SelectedObjects[ElementStatu_Index].OriginalName);
                }
                #endregion

                Editor_XHud_GUI.Gui_Layout_Space(10);
                EditorGUILayout.HelpBox(SelectedObjects[ElementStatu_Index].RMS_Enabled ? "此元素在生成时会优先使用自身记录的设计布局信息来进行定位和位置尺寸的匹配" : "如果启用设计布局那么在用代码调用生成时则会主动使用该元素自身记录的设计布局信息来进行定位和位置尺寸等相关参数的匹配（但前提是在您制作好此元素时需要手动点击\"记录布局信息\"按钮）", MessageType.Info);
            }

            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region RMS布局信息
            if (RMS_Enabled.boolValue)
            {

                Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "RMS布局信息", XHud_Dashboard.Theme_Primary);
                Editor_XHud_GUI.Gui_Layout_Space(5);

                #region 分辨率方案列表
                if (mgr != null)
                {
                    //ScreenResolutionNode[] nodes = mgr.hm_RMS_GetResolutionNodes();
                    string[] nodesName = mgr.hm_RMS_GetResolutionNodeNames();
                    if (nodesName.Length > 0)
                    {
                        Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                        Editor_XHud_GUI.Gui_Layout_Space(10);
                        EditorGUILayout.HelpBox("RMS方案列表仅用于切换和预览，在取消选择元素后会自动回到Hud管理器当前选中的RMS方案", MessageType.Info);
                        Editor_XHud_GUI.Gui_Layout_Space(5);
                        Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                        EditorGUI.BeginChangeCheck();
                        Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_Module_Element>("R M S 方案", nodesName, ref RMS_Name, HudFilled.实体, 120, 22, SelectedObjects);

                        #region RMS方案信息查看

                        if (Targets_Selected())
                        {
                            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                            Editor_XHud_GUI.Gui_Layout_Space(10);
                            EditorGUILayout.HelpBox("RMS方案信息不支持多项查看", MessageType.Warning);
                            Editor_XHud_GUI.Gui_Layout_Space(5);
                            Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                        }
                        else
                        {
                            if (RMS_LayoutDatas.arraySize <= 0)
                            {
                                Editor_XHud_GUI.Gui_Layout_Labelfield("暂无布局设计数据", HudFilled.无, HudColor.无, Editor_XHud_GUI.GetColor(HudColor.阴影灰), TextAnchor.MiddleCenter);
                            }
                            else
                            {
                                for (int i = 0; i < RMS_LayoutDatas.arraySize; i++)
                                {
                                    SerializedProperty sp_item = RMS_LayoutDatas.GetArrayElementAtIndex(i);
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
                                        Color valuecol = Editor_XHud_GUI.GetColor(HudColor.阴影灰);

                                        Editor_XHud_GUI.StatuDisplayer_text(rms_anchor, iconsize, new Vector2(0, 7), "锚点", titlesize, ((XHudAnchor)sp_item_Anchor.enumValueIndex).ToString(), valuecol, contensize);
                                        Editor_XHud_GUI.StatuDisplayer_text(rms_anchor, iconsize, new Vector2(0, 7), "最小锚点", titlesize, sp_item_AnchorMin.vector2Value.ToString(), valuecol, contensize);
                                        Editor_XHud_GUI.StatuDisplayer_text(rms_anchor, iconsize, new Vector2(0, 7), "最大锚点", titlesize, sp_item_AnchorMax.vector2Value.ToString(), valuecol, contensize);
                                        Editor_XHud_GUI.StatuDisplayer_text(rms_move, iconsize, new Vector2(0, 7), "位置", titlesize, sp_item_Position.vector3Value.ToString(), valuecol, contensize);
                                        Editor_XHud_GUI.StatuDisplayer_text(rms_rotate, iconsize, new Vector2(0, 7), "角度", titlesize, sp_item_Euler.vector3Value.ToString(), valuecol, contensize);
                                        Editor_XHud_GUI.StatuDisplayer_text(rms_scale, iconsize, new Vector2(0, 7), "缩放", titlesize, sp_item_Scale.vector3Value.ToString(), valuecol, contensize);
                                        Editor_XHud_GUI.StatuDisplayer_text(rms_anchor_center, iconsize, new Vector2(0, 7), "轴心", titlesize, sp_item_Pivot.vector2Value.ToString(), valuecol, contensize);
                                    }
                                }

                                Editor_XHud_GUI.Gui_Layout_Space(5);

                                Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);

                                if (RMS_LayoutDatas.arraySize > 0)
                                {
                                    if (!Application.isPlaying)
                                        if (Editor_XHud_GUI.Gui_Layout_Button("清空所有方案", "清空所有分辨率匹配方案列表", HudFilled.实体, HudColor.深空灰, Editor_XHud_GUI.GetColor(HudColor.魅力红), 25, new RectOffset(), new Vector2(0, 0)))
                                        {
                                            string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 元素消息", "RMS方案操作", "如果清空匹配分辨率信息列表，会导致您之前为不同分辨率记录的坐标信息全部清空，请谨慎此操作！", "清空", "暂不", 1);
                                            if (res == "清空")
                                            {
                                                RMS_LayoutDatas.ClearArray();
                                                RMS_LayoutDatas.serializedObject.ApplyModifiedProperties();
                                            }
                                            return;
                                        }
                                }

                                if (RMS_IsExist())
                                {
                                    if (!Application.isPlaying)
                                        Editor_XHud_GUI.Gui_Layout_Space(5);
                                    if (Editor_XHud_GUI.Gui_Layout_Button("移除当前方案", "清空当前选择的分辨率匹配方案", HudFilled.实体, HudColor.深空灰, Color.white, 25, new RectOffset(), new Vector2(0, 0)))
                                    {
                                        string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 元素消息", "RMS方案操作", "此操作会导致您为当前分辨率匹配的方案会被移除，请谨慎此操作！", "清空", "暂不", 1);
                                        if (res == "暂不")
                                            return;
                                        for (int i = 0; i < RMS_LayoutDatas.arraySize; i++)
                                        {
                                            SerializedProperty sp_Name = RMS_LayoutDatas.GetArrayElementAtIndex(i).FindPropertyRelative("LayoutName");
                                            if (sp_Name.stringValue == RMS_Name.stringValue)
                                            {
                                                RMS_LayoutDatas.DeleteArrayElementAtIndex(i);
                                                RMS_LayoutDatas.serializedObject.ApplyModifiedProperties();
                                                break;
                                            }
                                        }
                                        return;
                                    }
                                }

                                Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                            }
                        }
                        #endregion

                        if (EditorGUI.EndChangeCheck())
                        {
                            if (!Application.isPlaying)
                            {
                                if (Targets_Selected())
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
                        Editor_XHud_GUI.Gui_Layout_Labelfield("暂未在管理器中配置 R M S 列表", HudFilled.无, HudColor.无, Editor_XHud_GUI.GetColor(HudColor.阴影灰), TextAnchor.MiddleCenter);
                    }
                }
                else
                {
                    Editor_XHud_GUI.Gui_Layout_Labelfield("暂未发现Hud管理器", HudFilled.无, HudColor.无, Editor_XHud_GUI.GetColor(HudColor.阴影灰), TextAnchor.MiddleCenter);
                }
                #endregion

                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Vertical_End();
            }
            #endregion

            #region 列表
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "事件/列表", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(5);

            #region 音效列表
            if (SounderNodes.arraySize > 0)
            {
                if (Targets_Selected())
                {
                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(10);
                    EditorGUILayout.HelpBox("音效器列表不支持多项操作", MessageType.Warning);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                }
                else
                {
                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(10);
                    SounderIsFold.boolValue = EditorGUILayout.Foldout(SounderIsFold.boolValue, "音效器", true);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    if (SounderIsFold.boolValue)
                        SounderList.DoLayoutList();
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                }
            }
            #endregion

            #region 动画器列表
            if (AnimatorNodes.arraySize > 0)
            {
                if (Targets_Selected())
                {
                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(10);
                    EditorGUILayout.HelpBox("动画器列表不支持多项操作", MessageType.Warning);
                    //util_EditorGuiLib.Gui_Layout_Labelfield("动画器列表不支持多项操作", HudFilled.无_None, HudColor.无_None, util_EditorGuiLib.GetColor(HudColor.阴影灰), TextAnchor.MiddleLeft);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                }
                else
                {
                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(10);
                    AnimatorsIsFold.boolValue = EditorGUILayout.Foldout(AnimatorsIsFold.boolValue, "动画器", true);
                    AnimatorsIsFold.serializedObject.ApplyModifiedProperties();
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    if (AnimatorsIsFold.boolValue)
                        AnimatorsList.DoLayoutList();

                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                }
            }
            #endregion

            #region 按钮列表
            if (ButtonNodes.arraySize > 0)
            {
                if (Targets_Selected())
                {
                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(10);
                    EditorGUILayout.HelpBox("按钮列表不支持多项操作", MessageType.Warning);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                }
                else
                {
                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(10);
                    ButtonIsFold.boolValue = EditorGUILayout.Foldout(ButtonIsFold.boolValue, "按钮", true);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    if (ButtonIsFold.boolValue)
                        ButtonList.DoLayoutList();
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                }
            }
            #endregion

            #region 文字列表
            if (TextNodes.arraySize > 0)
            {
                if (Targets_Selected())
                {
                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(10);
                    EditorGUILayout.HelpBox("文字列表不支持多项操作", MessageType.Warning);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                }
                else
                {
                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(10);
                    TextIsFold.boolValue = EditorGUILayout.Foldout(TextIsFold.boolValue, "文字", true);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    if (TextIsFold.boolValue)
                        TextList.DoLayoutList();
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                }
            }
            #endregion

            #region Tmp文字列表
            if (TmpTextNodes.arraySize > 0)
            {
                if (Targets_Selected())
                {
                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(10);
                    EditorGUILayout.HelpBox("Tmp文字列表不支持多项操作", MessageType.Warning);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                }
                else
                {
                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(10);
                    TmpTextIsFold.boolValue = EditorGUILayout.Foldout(TmpTextIsFold.boolValue, "Tmp文字", true);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    if (TmpTextIsFold.boolValue)
                        TmpTextList.DoLayoutList();
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                }
            }
            #endregion

            #region 选项列表
            if (OptionNodes.arraySize > 0)
            {
                if (Targets_Selected())
                {
                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(10);
                    EditorGUILayout.HelpBox("选项列表不支持多项操作", MessageType.Warning);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                }
                else
                {
                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(10);
                    OptionIsFold.boolValue = EditorGUILayout.Foldout(OptionIsFold.boolValue, "选项", true);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    if (OptionIsFold.boolValue)
                        OptionList.DoLayoutList();
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                }
            }
            #endregion

            #region 滑动条列表
            if (SliderNodes.arraySize > 0)
            {
                if (Targets_Selected())
                {
                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(10);
                    EditorGUILayout.HelpBox("滑动条列表不支持多项操作", MessageType.Warning);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                }
                else
                {
                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(10);
                    SliderIsFold.boolValue = EditorGUILayout.Foldout(SliderIsFold.boolValue, "滑动条", true);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    if (SliderIsFold.boolValue)
                        SliderList.DoLayoutList();
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                }
            }
            #endregion

            #region 进度条列表
            if (ProgressNodes.arraySize > 0)
            {
                if (Targets_Selected())
                {
                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(10);
                    EditorGUILayout.HelpBox("进度条列表不支持多项操作", MessageType.Warning);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                }
                else
                {
                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(10);
                    ProgressIsFold.boolValue = EditorGUILayout.Foldout(ProgressIsFold.boolValue, "进度条", true);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    if (ProgressIsFold.boolValue)
                        ProgressList.DoLayoutList();
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                }
            }
            #endregion

            #region 容器列表
            if (ContainerNodes.arraySize > 0)
            {
                if (Targets_Selected())
                {
                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(10);
                    EditorGUILayout.HelpBox("容器列表不支持多项操作", MessageType.Warning);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                }
                else
                {
                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(10);
                    ContainerIsFold.boolValue = EditorGUILayout.Foldout(ContainerIsFold.boolValue, "容器", true);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    if (ContainerIsFold.boolValue)
                        ContainerList.DoLayoutList();
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                }
            }
            #endregion

            #region 开关列表
            if (ToggleNodes.arraySize > 0)
            {
                if (Targets_Selected())
                {
                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(10);
                    EditorGUILayout.HelpBox("开关列表不支持多项操作", MessageType.Warning);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                }
                else
                {
                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(10);
                    ToggleIsFold.boolValue = EditorGUILayout.Foldout(ToggleIsFold.boolValue, "开关", true);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    if (ToggleIsFold.boolValue)
                        ToggleList.DoLayoutList();
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
                EventIsFold.boolValue = EditorGUILayout.Foldout(EventIsFold.boolValue, "事件", true);
                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                if (EventIsFold.boolValue)
                {
                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_element_in_start);
                    eve_on_element_in_start.serializedObject.ApplyModifiedProperties();
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_element_in_end);
                    eve_on_element_in_end.serializedObject.ApplyModifiedProperties();
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    EditorGUILayout.PropertyField(eve_on_element_out_start);
                    eve_on_element_out_start.serializedObject.ApplyModifiedProperties();
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_element_out_end);
                    eve_on_element_out_end.serializedObject.ApplyModifiedProperties();
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                }
            }
            #endregion

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
                if (SounderNodes.arraySize <= 0 && AnimatorNodes.arraySize <= 0 && ButtonNodes.arraySize <= 0 && OptionNodes.arraySize <= 0 &&
                    SliderNodes.arraySize <= 0 && ProgressNodes.arraySize <= 0 && ToggleNodes.arraySize <= 0 && ContainerNodes.arraySize <= 0 &&
                    TextNodes.arraySize <= 0 && TmpTextNodes.arraySize <= 0)
                {
                    Editor_XHud_GUI.Gui_Layout_Labelfield("暂无统计数据", HudFilled.无, HudColor.无, Editor_XHud_GUI.GetColor(HudColor.阴影灰), TextAnchor.MiddleCenter);
                }
                else
                {
                    #region 组件数量 - 音效器
                    if (SounderNodes.arraySize > 0)
                    {
                        Editor_XHud_GUI.StatuDisplayer_text(icon_sound, 12, new Vector2(0, 7), "音效器", 12, SounderNodes.arraySize.ToString() + " 个", XHud_Dashboard.Theme_Primary, 11);
                    }
                    #endregion

                    #region 组件数量 - 动画器
                    if (AnimatorNodes.arraySize > 0)
                    {
                        Editor_XHud_GUI.StatuDisplayer_text(icon_anim, 12, new Vector2(0, 7), "动画器", 12, AnimatorNodes.arraySize.ToString() + " 个", XHud_Dashboard.Theme_Primary, 11);
                    }
                    #endregion

                    #region 组件数量 - 按钮
                    if (ButtonNodes.arraySize > 0)
                    {
                        Editor_XHud_GUI.StatuDisplayer_text(icon_button, 12, new Vector2(0, 7), "按钮", 12, ButtonNodes.arraySize.ToString() + " 个", XHud_Dashboard.Theme_Primary, 11);
                    }
                    #endregion

                    #region 组件数量 - 选项
                    if (OptionNodes.arraySize > 0)
                    {
                        Editor_XHud_GUI.StatuDisplayer_text(icon_option, 12, new Vector2(0, 7), "选项", 12, OptionNodes.arraySize.ToString() + " 个", XHud_Dashboard.Theme_Primary, 11);
                    }
                    #endregion

                    #region 组件数量 - 滑动条
                    if (SliderNodes.arraySize > 0)
                    {
                        Editor_XHud_GUI.StatuDisplayer_text(icon_slider, 12, new Vector2(0, 7), "滑动条", 12, SliderNodes.arraySize.ToString() + " 个", XHud_Dashboard.Theme_Primary, 11);
                    }
                    #endregion

                    #region 组件数量 - 进度条
                    if (ProgressNodes.arraySize > 0)
                    {
                        Editor_XHud_GUI.StatuDisplayer_text(icon_progress, 12, new Vector2(0, 7), "进度条", 12, ProgressNodes.arraySize.ToString() + " 个", XHud_Dashboard.Theme_Primary, 11);
                    }
                    #endregion

                    #region 组件数量 - 容器
                    if (ContainerNodes.arraySize > 0)
                    {
                        Editor_XHud_GUI.StatuDisplayer_text(icon_container, 12, new Vector2(0, 7), "容器", 12, ContainerNodes.arraySize.ToString() + " 个", XHud_Dashboard.Theme_Primary, 11);
                    }
                    #endregion

                    #region 组件数量 - 开关
                    if (ToggleNodes.arraySize > 0)
                    {
                        Editor_XHud_GUI.StatuDisplayer_text(icon_toggle, 12, new Vector2(0, 7), "开关", 12, ToggleNodes.arraySize.ToString() + " 个", XHud_Dashboard.Theme_Primary, 11);
                    }
                    #endregion

                    #region 组件数量 - 文字
                    if (TextNodes.arraySize > 0)
                    {
                        Editor_XHud_GUI.StatuDisplayer_text(icon_text, 12, new Vector2(0, 7), "文字", 12, TextNodes.arraySize.ToString() + " 个", XHud_Dashboard.Theme_Primary, 11);
                    }
                    #endregion

                    #region 组件数量 - Tmp文字
                    if (TmpTextNodes.arraySize > 0)
                    {
                        Editor_XHud_GUI.StatuDisplayer_text(icon_tmptext, 12, new Vector2(0, 7), "Tmp文字", 12, TmpTextNodes.arraySize.ToString() + " 个", XHud_Dashboard.Theme_Primary, 11);
                    }
                    #endregion
                }
            }
            else
            {
                #region 批量控件
                Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_XHud_GUI.Gui_Layout_Space(10);
                if (Editor_XHud_GUI.Gui_Layout_Button($"{SelectedObjects[ElementStatistic_Index].name} ( {SelectedObjects[ElementStatistic_Index].Indicator} )", "", HudFilled.透明, HudColor.无, Color.gray, 20))
                {
                    EditorGUIUtility.PingObject(SelectedObjects[ElementStatistic_Index]);
                }
                Editor_XHud_GUI.Gui_Layout_FlexSpace();
                if (Editor_XHud_GUI.Gui_Layout_Button(12, "", left_arrow_r, left_arrow_p))
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
                Editor_XHud_GUI.Gui_Layout_Space(16);
                if (Editor_XHud_GUI.Gui_Layout_Button(12, "", right_arrow_r, right_arrow_p))
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
                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                #endregion

                if (SelectedObjects[ElementStatistic_Index].SounderNodes.Count <= 0 && SelectedObjects[ElementStatistic_Index].AnimatorNodes.Count <= 0 && SelectedObjects[ElementStatistic_Index].ButtonNodes.Count <= 0 && OptionNodes.arraySize <= 0 &&
                   SelectedObjects[ElementStatistic_Index].SliderNodes.Count <= 0 && SelectedObjects[ElementStatistic_Index].ProgressNodes.Count <= 0 && SelectedObjects[ElementStatistic_Index].ToggleNodes.Count <= 0 && ContainerNodes.arraySize <= 0 &&
                   SelectedObjects[ElementStatistic_Index].TextNodes.Count <= 0 && SelectedObjects[ElementStatistic_Index].TmpTextNodes.Count <= 0)
                {
                    Editor_XHud_GUI.Gui_Layout_Labelfield("暂无统计数据", HudFilled.无, HudColor.无, Editor_XHud_GUI.GetColor(HudColor.阴影灰), TextAnchor.MiddleCenter);
                }
                else
                {
                    #region 组件数量 - 音效器
                    if (SelectedObjects[ElementStatistic_Index].SounderNodes.Count > 0)
                    {
                        Editor_XHud_GUI.StatuDisplayer_text(icon_sound, 12, new Vector2(0, 7), "音效器", 12, SelectedObjects[ElementStatistic_Index].SounderNodes.Count.ToString() + " 个", XHud_Dashboard.Theme_Primary, 11);
                    }
                    #endregion

                    #region 组件数量 - 动画器
                    if (SelectedObjects[ElementStatistic_Index].AnimatorNodes.Count > 0)
                    {
                        Editor_XHud_GUI.StatuDisplayer_text(icon_anim, 12, new Vector2(0, 7), "动画器", 12, SelectedObjects[ElementStatistic_Index].AnimatorNodes.Count.ToString() + " 个", XHud_Dashboard.Theme_Primary, 11);
                    }
                    #endregion

                    #region 组件数量 - 按钮
                    if (SelectedObjects[ElementStatistic_Index].ButtonNodes.Count > 0)
                    {
                        Editor_XHud_GUI.StatuDisplayer_text(icon_button, 12, new Vector2(0, 7), "按钮", 12, SelectedObjects[ElementStatistic_Index].ButtonNodes.Count.ToString() + " 个", XHud_Dashboard.Theme_Primary, 11);
                    }
                    #endregion

                    #region 组件数量 - 选项
                    if (SelectedObjects[ElementStatistic_Index].OptionNodes.Count > 0)
                    {
                        Editor_XHud_GUI.StatuDisplayer_text(icon_option, 12, new Vector2(0, 7), "选项", 12, SelectedObjects[ElementStatistic_Index].OptionNodes.Count.ToString() + " 个", XHud_Dashboard.Theme_Primary, 11);
                    }
                    #endregion

                    #region 组件数量 - 滑动条
                    if (SelectedObjects[ElementStatistic_Index].SliderNodes.Count > 0)
                    {
                        Editor_XHud_GUI.StatuDisplayer_text(icon_slider, 12, new Vector2(0, 7), "滑动条", 12, SelectedObjects[ElementStatistic_Index].SliderNodes.Count.ToString() + " 个", XHud_Dashboard.Theme_Primary, 11);
                    }
                    #endregion

                    #region 组件数量 - 进度条
                    if (SelectedObjects[ElementStatistic_Index].ProgressNodes.Count > 0)
                    {
                        Editor_XHud_GUI.StatuDisplayer_text(icon_progress, 12, new Vector2(0, 7), "进度条", 12, SelectedObjects[ElementStatistic_Index].ProgressNodes.Count.ToString() + " 个", XHud_Dashboard.Theme_Primary, 11);
                    }
                    #endregion

                    #region 组件数量 - 容器
                    if (SelectedObjects[ElementStatistic_Index].ContainerNodes.Count > 0)
                    {
                        Editor_XHud_GUI.StatuDisplayer_text(icon_container, 12, new Vector2(0, 7), "容器", 12, SelectedObjects[ElementStatistic_Index].ContainerNodes.Count.ToString() + " 个", XHud_Dashboard.Theme_Primary, 11);
                    }
                    #endregion

                    #region 组件数量 - 开关
                    if (SelectedObjects[ElementStatistic_Index].ToggleNodes.Count > 0)
                    {
                        Editor_XHud_GUI.StatuDisplayer_text(icon_toggle, 12, new Vector2(0, 7), "开关", 12, SelectedObjects[ElementStatistic_Index].ToggleNodes.Count.ToString() + " 个", XHud_Dashboard.Theme_Primary, 11);
                    }
                    #endregion

                    #region 组件数量 - 文字
                    if (SelectedObjects[ElementStatistic_Index].TextNodes.Count > 0)
                    {
                        Editor_XHud_GUI.StatuDisplayer_text(icon_text, 12, new Vector2(0, 7), "文字", 12, SelectedObjects[ElementStatistic_Index].TextNodes.Count.ToString() + " 个", XHud_Dashboard.Theme_Primary, 11);
                    }
                    #endregion

                    #region 组件数量 - Tmp文字
                    if (SelectedObjects[ElementStatistic_Index].TmpTextNodes.Count > 0)
                    {
                        Editor_XHud_GUI.StatuDisplayer_text(icon_tmptext, 12, new Vector2(0, 7), "Tmp文字", 12, SelectedObjects[ElementStatistic_Index].TmpTextNodes.Count.ToString() + " 个", XHud_Dashboard.Theme_Primary, 11);
                    }
                    #endregion
                }
            }

            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 组件

            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "组件", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(10);
            if (Targets_Selected())
            {
                Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_XHud_GUI.Gui_Layout_Space(10);
                EditorGUILayout.HelpBox("组件不支持多项操作", MessageType.Warning);
                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Horizontal_End();
            }
            else
            {
                ///---CanvasGroup组件                
                Editor_XHud_GUI.StatuDisplayer_Object(comp_alpha, 12, new Vector2(0, 1), "基础图像组件", 12, new Vector2(0, -7), status, new Vector2(0, 3), CanvasGroup.objectReferenceValue == null ? false : true, XHud_Dashboard.Theme_Primary, Color.black * 0.7f, CanvasGroup);

                ///---RectTransform
                Editor_XHud_GUI.StatuDisplayer_Object(comp_transform, 12, new Vector2(0, 1), "变换组件", 12, new Vector2(0, -7), status, new Vector2(0, 3), RectTransform.objectReferenceValue == null ? false : true, XHud_Dashboard.Theme_Primary, Color.black * 0.7f, RectTransform);

                ///---TriggerAction
                Editor_XHud_GUI.StatuDisplayer_Object(comp_trigger, 12, new Vector2(0, 1), "触发器", 12, new Vector2(0, -7), status, new Vector2(0, 3), TriggerAction.objectReferenceValue == null ? false : true, XHud_Dashboard.Theme_Primary, Color.black * 0.7f, TriggerAction);

                ///---ObjectTracker
                Editor_XHud_GUI.StatuDisplayer_Object(comp_tracking, 12, new Vector2(0, 1), "追踪器", 12, new Vector2(0, -7), status, new Vector2(0, 3), ObjectTracker.objectReferenceValue == null ? false : true, XHud_Dashboard.Theme_Primary, Color.black * 0.7f, ObjectTracker);

            }
            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
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
                        Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 元素消息", "扫描元素", "程序正在运行，无法在运行期间执行此功能！", "明白");
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
                            Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 元素消息", "记录RMS方案信息", "程序正在运行，无法在运行期间执行此功能！", "明白");
                            return;
                        }

                        RMS_Record();
                    });
                    menu.AddItem(new GUIContent("C (清空)"), false, () =>
                    {
                        if (Application.isPlaying)
                        {
                            Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 元素消息", "清空RMS方案信息", "程序正在运行，无法在运行期间执行此功能！", "明白");
                            return;
                        }

                        string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 元素消息", "RMS方案操作", "如果清空匹配分辨率信息列表，会导致您之前为不同分辨率记录的坐标信息全部清空，请谨慎此操作！", "清空", "暂不", 1);
                        if (res == "清空")
                        {
                            RMS_LayoutDatas.ClearArray();
                            RMS_LayoutDatas.serializedObject.ApplyModifiedProperties();
                        }
                        return;
                    });
                }
                menu.AddDisabledItem(new GUIContent("坐标"));
                menu.AddItem(new GUIContent("R (回到锚点)"), false, () =>
                {
                    if (Application.isPlaying)
                    {
                        Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 元素消息", "锚点初始化", "程序正在运行，无法在运行期间执行此功能！", "明白");
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
                if (!Targets_Selected())
                {
                    menu.AddSeparator("");
                    //menu.AddDisabledItem(new GUIContent("预览"));
                    //if (!Tween_Preivewing_Element)
                    //    menu.AddItem(new GUIContent("S (开始)"), false, () =>
                    //    {
                    //        if (Application.isPlaying)
                    //        {
                    //            Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 元素消息", "预览动画", "程序正在运行，无法在运行期间执行此功能！", "明白");
                    //            return;
                    //        }
                    //        Preview_Animator_Play();
                    //    });
                    //else
                    //    menu.AddItem(new GUIContent("S (停止)"), false, () =>
                    //    {
                    //        if (Application.isPlaying)
                    //        {
                    //            Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 元素消息", "停止预览动画", "程序正在运行，无法在运行期间执行此功能！", "明白");
                    //            return;
                    //        }
                    //        Preview_Animator_Stop();
                    //    });
                }

                if (PrefabUtility.IsAnyPrefabInstanceRoot(BaseScript.gameObject))
                {
                    menu.AddSeparator("");
                    menu.AddDisabledItem(new GUIContent("预制体"));
                    menu.AddItem(new GUIContent("A (应用)"), false, () =>
                    {
                        if (Application.isPlaying)
                        {
                            Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 元素消息", "预制体应用", "程序正在运行，无法在运行期间执行此功能！", "明白");
                            return;
                        }
                        string path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(BaseScript.gameObject);
                        PrefabUtility.SaveAsPrefabAssetAndConnect(BaseScript.gameObject, path, InteractionMode.AutomatedAction);
                    });
                    menu.AddItem(new GUIContent("Z (定位)"), false, () =>
                    {
                        if (Application.isPlaying)
                        {
                            Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 元素消息", "预制体定位", "程序正在运行，无法在运行期间执行此功能！", "明白");
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

            CalculateAnimatorMaxDuration(BaseScript.AnimatorNodes, Element_Animators_GlobalDuration.floatValue);

            serializedObject.ApplyModifiedProperties();
        }

        #region 辅助
        /// <summary>
        /// 元素回到锚点初始位置
        /// </summary>
        private void RestoreToAnchorPosition()
        {
            string res = Editor_XHud_GUI.Open(XHud_DialogType.帮助, "XHud - 元素消息", "锚点初始化", "是否需要将元素回到当前所在的锚点初始位置！", "确定", "暂不", 1);
            if (res == "暂不")
            {
                return;
            }
            if (Targets_Selected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    SerializedObject so = new SerializedObject(SelectedObjects[i]);
                    XHud_Module_Element ele = (XHud_Module_Element)so.targetObject;
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
                    XHud_Module_Container con = (XHud_Module_Container)sp_con.objectReferenceValue;
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
                    XHud_Module_Button con = (XHud_Module_Button)sp_con.objectReferenceValue;
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
                    XHud_Module_Toggle con = (XHud_Module_Toggle)sp_con.objectReferenceValue;
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
                    XHud_Module_Progress con = (XHud_Module_Progress)sp_con.objectReferenceValue;
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
                    XHud_Module_Slider con = (XHud_Module_Slider)sp_con.objectReferenceValue;
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
                    XHud_Module_Option con = (XHud_Module_Option)sp_con.objectReferenceValue;
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
                    XHud_Element_Sounder con = (XHud_Element_Sounder)sp_con.objectReferenceValue;
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
                    XHud_Module_Animator con = (XHud_Module_Animator)sp_con.objectReferenceValue;
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
            List<XHud_GUI_Dialog_ListDatas> Datas = new List<XHud_GUI_Dialog_ListDatas>();
            if (Targets_Selected())
            {
                for (int s = 0; s < SelectedObjects.Length; s++)
                {
                    bool existcomp = false;

                    XHud_Module_Element ele = SelectedObjects[s];
                    //-扫描 - 动画器 
                    for (int i = 0; i < ele.AnimatorNodes.Count; i++)
                    {
                        XHud_GUI_Dialog_ListDatas dataitem = new XHud_GUI_Dialog_ListDatas();
                        dataitem.Title = $"{SelectedObjects[s].name} ( {SelectedObjects[s].Indicator} )";
                        dataitem.SubTitle = "已收集 / 动画器";
                        dataitem.Message = $"{ele.AnimatorNodes[i].Animator.name} ( {ele.AnimatorNodes[i].Animator.Indicator} )";
                        Datas.Add(dataitem);
                        existcomp = true;
                    }
                    //-扫描 - 按钮
                    for (int i = 0; i < ele.ButtonNodes.Count; i++)
                    {
                        XHud_GUI_Dialog_ListDatas dataitem = new XHud_GUI_Dialog_ListDatas();
                        dataitem.Title = $"{SelectedObjects[s].name} ( {SelectedObjects[s].Indicator} )";
                        dataitem.SubTitle = "已收集 / 按钮";
                        dataitem.Message = $"{ele.ButtonNodes[i].Button.name} ( {ele.ButtonNodes[i].Button.Indicator} )";
                        Datas.Add(dataitem);
                        existcomp = true;
                    }
                    //-扫描 - 选项器
                    for (int i = 0; i < ele.OptionNodes.Count; i++)
                    {
                        XHud_GUI_Dialog_ListDatas dataitem = new XHud_GUI_Dialog_ListDatas();
                        dataitem.Title = $"{SelectedObjects[s].name} ( {SelectedObjects[s].Indicator} )";
                        dataitem.SubTitle = "已收集 / 选项";
                        dataitem.Message = $"{ele.OptionNodes[i].Option.name} ( {ele.OptionNodes[i].Option.Indicator} )";
                        Datas.Add(dataitem);
                        existcomp = true;
                    }
                    //-扫描 - 滑动条
                    for (int i = 0; i < ele.SliderNodes.Count; i++)
                    {
                        XHud_GUI_Dialog_ListDatas dataitem = new XHud_GUI_Dialog_ListDatas();
                        dataitem.Title = $"{SelectedObjects[s].name} ( {SelectedObjects[s].Indicator} )";
                        dataitem.SubTitle = "已收集 / 滑动条";
                        dataitem.Message = $"{ele.SliderNodes[i].Slider.name} ( {ele.SliderNodes[i].Slider.Indicator} )";
                        Datas.Add(dataitem);
                        existcomp = true;
                    }
                    //-扫描 - 进度条
                    for (int i = 0; i < ele.ProgressNodes.Count; i++)
                    {
                        XHud_GUI_Dialog_ListDatas dataitem = new XHud_GUI_Dialog_ListDatas();
                        dataitem.Title = $"{SelectedObjects[s].name} ( {SelectedObjects[s].Indicator} )";
                        dataitem.SubTitle = "已收集 / 进度条";
                        dataitem.Message = $"{ele.ProgressNodes[i].Progress.name} ( {ele.ProgressNodes[i].Progress.Indicator} )";
                        Datas.Add(dataitem);
                        existcomp = true;
                    }
                    //-扫描 - 开关
                    for (int i = 0; i < ele.ToggleNodes.Count; i++)
                    {
                        XHud_GUI_Dialog_ListDatas dataitem = new XHud_GUI_Dialog_ListDatas();
                        dataitem.Title = $"{SelectedObjects[s].name} ( {SelectedObjects[s].Indicator} )";
                        dataitem.SubTitle = "已收集 / 开关";
                        dataitem.Message = $"{ele.ToggleNodes[i].Toggle.name} ( {ele.ToggleNodes[i].Toggle.Indicator} )";
                        Datas.Add(dataitem);
                        existcomp = true;
                    }
                    //-扫描 - 声音
                    for (int i = 0; i < ele.SounderNodes.Count; i++)
                    {
                        XHud_GUI_Dialog_ListDatas dataitem = new XHud_GUI_Dialog_ListDatas();
                        dataitem.Title = $"{SelectedObjects[s].name} ( {SelectedObjects[s].Indicator} )";
                        dataitem.SubTitle = "已收集 / 音效器";
                        dataitem.Message = $"{ele.SounderNodes[i].Sounder.name} ( {ele.SounderNodes[i].Sounder.Indicator} )";
                        Datas.Add(dataitem);
                        existcomp = true;
                    }
                    //-扫描 - 容器
                    for (int i = 0; i < ele.ContainerNodes.Count; i++)
                    {
                        XHud_GUI_Dialog_ListDatas dataitem = new XHud_GUI_Dialog_ListDatas();
                        dataitem.Title = $"{SelectedObjects[s].name} ( {SelectedObjects[s].Indicator} )";
                        dataitem.SubTitle = "已收集 / 容器";
                        dataitem.Message = $"{ele.ContainerNodes[i].Container.name} ( {ele.ContainerNodes[i].Container.Indicator} )";
                        Datas.Add(dataitem);
                        existcomp = true;
                    }
                    //-扫描 - Text文字
                    for (int i = 0; i < ele.TextNodes.Count; i++)
                    {
                        XHud_GUI_Dialog_ListDatas dataitem = new XHud_GUI_Dialog_ListDatas();
                        dataitem.Title = $"{SelectedObjects[s].name} ( {SelectedObjects[s].Indicator} )";
                        dataitem.SubTitle = "已收集 / Text文字";
                        dataitem.Message = $"{ele.TextNodes[i].Text.name} ( {ele.TextNodes[i].Text.Indicator} )";
                        Datas.Add(dataitem);
                        existcomp = true;
                    }
                    //-扫描 - Tmp文字
                    for (int i = 0; i < ele.TmpTextNodes.Count; i++)
                    {
                        XHud_GUI_Dialog_ListDatas dataitem = new XHud_GUI_Dialog_ListDatas();
                        dataitem.Title = $"{SelectedObjects[s].name} ( {SelectedObjects[s].Indicator} )";
                        dataitem.SubTitle = "已收集 / Text文字";
                        dataitem.Message = $"{ele.TmpTextNodes[i].TmpText.name} ( {ele.TmpTextNodes[i].TmpText.Indicator} )";
                        Datas.Add(dataitem);
                        existcomp = true;
                    }

                    if (!existcomp)
                    {
                        XHud_GUI_Dialog_ListDatas dataitem = new XHud_GUI_Dialog_ListDatas();
                        dataitem.Title = $"{SelectedObjects[s].name} ( {SelectedObjects[s].Indicator} )";
                        dataitem.SubTitle = "";
                        dataitem.Message = "未发现任何存在的子组件";
                        Datas.Add(dataitem);
                    }
                }
                Editor_XHud_GUI.Open(Datas.ToArray(), XHud_DialogType.确认, "XHud - 元素消息", "批量扫描元素子组件", "以下是批量扫描到的所有元素子组件列表，请您检查核对：", "明白");
            }
            else
            {
                //-扫描 - 动画器
                for (int i = 0; i < BaseScript.AnimatorNodes.Count; i++)
                {
                    XHud_GUI_Dialog_ListDatas dataitem = new XHud_GUI_Dialog_ListDatas();
                    dataitem.Title = $"{BaseScript.name} ({BaseScript.Indicator} )";
                    dataitem.SubTitle = "已收集 / 动画器";
                    dataitem.Message = $"{BaseScript.AnimatorNodes[i].Animator.name} ({BaseScript.AnimatorNodes[i].Animator.Indicator} )";
                    Datas.Add(dataitem);
                }
                //-扫描 - 按钮
                for (int i = 0; i < BaseScript.ButtonNodes.Count; i++)
                {
                    XHud_GUI_Dialog_ListDatas dataitem = new XHud_GUI_Dialog_ListDatas();
                    dataitem.Title = $"{BaseScript.name} ({BaseScript.Indicator} )";
                    dataitem.SubTitle = "已收集 / 按钮";
                    dataitem.Message = $"{BaseScript.ButtonNodes[i].Button.name} ({BaseScript.ButtonNodes[i].Button.Indicator} )";
                    Datas.Add(dataitem);
                }
                //-扫描 - 选项器
                for (int i = 0; i < BaseScript.OptionNodes.Count; i++)
                {
                    XHud_GUI_Dialog_ListDatas dataitem = new XHud_GUI_Dialog_ListDatas();
                    dataitem.Title = $"{BaseScript.name} ({BaseScript.Indicator} )";
                    dataitem.SubTitle = "已收集 / 选项";
                    dataitem.Message = $"{BaseScript.OptionNodes[i].Option.name} ({BaseScript.OptionNodes[i].Option.Indicator} )";
                    Datas.Add(dataitem);
                }
                //-扫描 - 滑动条
                for (int i = 0; i < BaseScript.SliderNodes.Count; i++)
                {
                    XHud_GUI_Dialog_ListDatas dataitem = new XHud_GUI_Dialog_ListDatas();
                    dataitem.Title = $"{BaseScript.name} ({BaseScript.Indicator} )";
                    dataitem.SubTitle = "已收集 / 滑动条";
                    dataitem.Message = $"{BaseScript.SliderNodes[i].Slider.name} ({BaseScript.SliderNodes[i].Slider.Indicator} )";
                    Datas.Add(dataitem);
                }
                //-扫描 - 进度条
                for (int i = 0; i < BaseScript.ProgressNodes.Count; i++)
                {
                    XHud_GUI_Dialog_ListDatas dataitem = new XHud_GUI_Dialog_ListDatas();
                    dataitem.Title = $"{BaseScript.name} ({BaseScript.Indicator} )";
                    dataitem.SubTitle = "已收集 / 进度条";
                    dataitem.Message = $"{BaseScript.ProgressNodes[i].Progress.name} ({BaseScript.ProgressNodes[i].Progress.Indicator} )";
                    Datas.Add(dataitem);
                }
                //-扫描 - 开关
                for (int i = 0; i < BaseScript.ToggleNodes.Count; i++)
                {
                    XHud_GUI_Dialog_ListDatas dataitem = new XHud_GUI_Dialog_ListDatas();
                    dataitem.Title = $"{BaseScript.name} ({BaseScript.Indicator} )";
                    dataitem.SubTitle = "已收集 / 开关";
                    dataitem.Message = $"{BaseScript.ToggleNodes[i].Toggle.name} ({BaseScript.ToggleNodes[i].Toggle.Indicator} )";
                    Datas.Add(dataitem);
                }
                //-扫描 - 声音
                for (int i = 0; i < BaseScript.SounderNodes.Count; i++)
                {
                    XHud_GUI_Dialog_ListDatas dataitem = new XHud_GUI_Dialog_ListDatas();
                    dataitem.Title = $"{BaseScript.name} ({BaseScript.Indicator} )";
                    dataitem.SubTitle = "已收集 / 音效器";
                    dataitem.Message = $"{BaseScript.SounderNodes[i].Sounder.name} ({BaseScript.SounderNodes[i].Sounder.Indicator} )";
                    Datas.Add(dataitem);
                }
                //-扫描 - 容器
                for (int i = 0; i < BaseScript.ContainerNodes.Count; i++)
                {
                    XHud_GUI_Dialog_ListDatas dataitem = new XHud_GUI_Dialog_ListDatas();
                    dataitem.Title = $"{BaseScript.name} ({BaseScript.Indicator} )";
                    dataitem.SubTitle = "已收集 / 容器";
                    dataitem.Message = $"{BaseScript.ContainerNodes[i].Container.name} ({BaseScript.ContainerNodes[i].Container.Indicator} )";
                    Datas.Add(dataitem);
                }
                //-扫描 - Text文字
                for (int i = 0; i < BaseScript.TextNodes.Count; i++)
                {
                    XHud_GUI_Dialog_ListDatas dataitem = new XHud_GUI_Dialog_ListDatas();
                    dataitem.Title = $"{BaseScript.name} ({BaseScript.Indicator} )";
                    dataitem.SubTitle = "已收集 / Text文字";
                    dataitem.Message = $"{BaseScript.TextNodes[i].Text.name} ({BaseScript.TextNodes[i].Text.Indicator} )";
                    Datas.Add(dataitem);
                }
                //-扫描 - Tmp文字
                for (int i = 0; i < BaseScript.TmpTextNodes.Count; i++)
                {
                    XHud_GUI_Dialog_ListDatas dataitem = new XHud_GUI_Dialog_ListDatas();
                    dataitem.Title = $"{BaseScript.name} ({BaseScript.Indicator} )";
                    dataitem.SubTitle = "已收集 / Text文字";
                    dataitem.Message = $"{BaseScript.TmpTextNodes[i].TmpText.name} ({BaseScript.TmpTextNodes[i].TmpText.Indicator} )";
                    Datas.Add(dataitem);
                }

                if (BaseScript.AnimatorNodes.Count <= 0 && BaseScript.ButtonNodes.Count <= 0 && BaseScript.OptionNodes.Count <= 0 && BaseScript.SliderNodes.Count <= 0 && BaseScript.ProgressNodes.Count <= 0 && BaseScript.ToggleNodes.Count <= 0 && BaseScript.SounderNodes.Count <= 0 && BaseScript.ContainerNodes.Count <= 0 && BaseScript.TextNodes.Count <= 0 && BaseScript.TmpTextNodes.Count <= 0)
                {
                    string hexcol = XHud_Utilitys.Color_To_HexColor(XHud_Dashboard.Theme_Primary, true);
                    Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 元素消息", "扫描元素子组件", $"未扫描到任何子元素，请您检查子物体中是否有物体包含以下组件： <color={hexcol}>Hud_Animator、Hud_Button、Hud_Progress、Hud_Slider、Hud_Option、HudSound、Hud_Container、Hud_Text、Hud_TmpText、HudToggle</color>", "明白");
                }
                else
                    Editor_XHud_GUI.Open(Datas.ToArray(), XHud_DialogType.确认, "XHud - 元素消息", "扫描元素子组件", "以下是扫描到的元素所有子组件列表，请您检查核对：", "明白");
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
                TriggerAction.objectReferenceValue = BaseScript.GetComponent<XHud_Element_TriggerAction>();
                TriggerAction.serializedObject.ApplyModifiedProperties();
            }
            if (ObjectTracker.objectReferenceValue == null)
            {
                ObjectTracker.objectReferenceValue = BaseScript.GetComponent<XHud_ObjectTracker>();
                ObjectTracker.serializedObject.ApplyModifiedProperties();
            }
        }
        /// <summary>
        /// 改变元素透明度
        /// </summary>
        private void ChangeAlpha()
        {
            if (Targets_Selected())
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
            if (Targets_Selected())
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
        private void CalculateAnimatorMaxDuration(List<ElementNode_Animator> list, float globaldur)
        {
            AnimatorsMaxDuration.floatValue = Animators_GetAnimatorsMaxDuration(list, globaldur);
            AnimatorsMaxDuration.serializedObject.ApplyModifiedProperties();
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
            for (int i = 0; i < AnimatorNodes.arraySize; i++)
            {
                SerializedProperty sp_animator = AnimatorNodes.GetArrayElementAtIndex(i).FindPropertyRelative("Animator");
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
        private void AnimatorFeature_Load(XHud_Module_Animator animator)
        {
            if (animator == null)
                return;

            SerializedObject so = new SerializedObject(animator);
            so.Update();
            SerializedProperty feature = so.FindProperty("AnimatorFeature");
            SerializedProperty m_rect = so.FindProperty("mod_RectTransform");
            SerializedProperty m_img = so.FindProperty("mod_Image");
            SerializedProperty m_rawimg = so.FindProperty("mod_RawImage");
            SerializedProperty m_text = so.FindProperty("mod_Text");
            SerializedProperty m_tmptext = so.FindProperty("mod_TmpText");

            SerializedProperty pos = feature.FindPropertyRelative("Position");
            SerializedProperty eur = feature.FindPropertyRelative("Euler");
            SerializedProperty sca = feature.FindPropertyRelative("Scale");
            SerializedProperty size = feature.FindPropertyRelative("Size");
            SerializedProperty alp = feature.FindPropertyRelative("Alpha");
            SerializedProperty col = feature.FindPropertyRelative("Color");
            SerializedProperty fil = feature.FindPropertyRelative("Fill");

            RectTransform rect = m_rect.objectReferenceValue as RectTransform;
            Image image = m_img.objectReferenceValue as Image;
            RawImage rawimage = m_rawimg.objectReferenceValue as RawImage;
            XHud_Module_Text text = m_text.objectReferenceValue as XHud_Module_Text;
            XHud_Module_TmpText tmp = m_tmptext.objectReferenceValue as XHud_Module_TmpText;

            if (rect != null)
            {
                rect.anchoredPosition3D = pos.vector3Value;
                rect.localEulerAngles = eur.vector3Value;
                rect.localScale = sca.vector3Value;
                rect.sizeDelta = size.vector2Value;
                //m_rect.serializedObject.ApplyModifiedProperties();
            }

            if (image != null)
            {
                //Image img = (Image)m_img.objectReferenceValue;
                image.color = col.colorValue;
                image.fillAmount = fil.floatValue;
                Color cc = image.color;
                cc.a = alp.floatValue;
                image.color = cc;

                //m_img.serializedObject.ApplyModifiedProperties();
            }
            else if (text != null)
            {
                text.color = col.colorValue;
                //m_text.serializedObject.ApplyModifiedProperties();
            }
            else if (tmp != null)
            {
                tmp.color = col.colorValue;
                //m_tmptext.serializedObject.ApplyModifiedProperties();
            }
            else if (rawimage != null)
            {
                //RawImage rawimg = (RawImage)m_rawimg.objectReferenceValue;
                rawimage.color = col.colorValue;
                Color cc = rawimage.color;
                cc.a = alp.floatValue;
                rawimage.color = cc;

                //m_img.serializedObject.ApplyModifiedProperties();
            }

            so.ApplyModifiedProperties();
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
        /// <summary>
        /// 获取序列化变量
        /// </summary>
        private void GetSerializeFields()
        {
            SounderNodes = serializedObject.FindProperty("SounderNodes");
            AnimatorNodes = serializedObject.FindProperty("AnimatorNodes");
            ButtonNodes = serializedObject.FindProperty("ButtonNodes");
            OptionNodes = serializedObject.FindProperty("OptionNodes");
            SliderNodes = serializedObject.FindProperty("SliderNodes");
            ProgressNodes = serializedObject.FindProperty("ProgressNodes");
            TextNodes = serializedObject.FindProperty("TextNodes");
            TmpTextNodes = serializedObject.FindProperty("TmpTextNodes");
            ToggleNodes = serializedObject.FindProperty("ToggleNodes");
            ContainerNodes = serializedObject.FindProperty("ContainerNodes");
            DebugState = serializedObject.FindProperty("DebugState");
            CanvasGroup = serializedObject.FindProperty("CanvasGroup");
            Alpha = serializedObject.FindProperty("Alpha");
            RectTransform = serializedObject.FindProperty("RectTransform");
            PreviewPrimitivesTween = serializedObject.FindProperty("PreviewPrimitivesTween");
            TriggerAction = serializedObject.FindProperty("TriggerAction");
            ObjectTracker = serializedObject.FindProperty("ObjectTracker");
            AutoPlayAnimators = serializedObject.FindProperty("AutoPlayAnimators");
            AutoPlayContainersAnimators = serializedObject.FindProperty("AutoPlayContainersAnimators");
            Tween_Preview_AutoStop = serializedObject.FindProperty("Tween_Preview_AutoStop");
            eve_on_element_in_start = serializedObject.FindProperty("eve_on_element_in_start");
            eve_on_element_in_end = serializedObject.FindProperty("eve_on_element_in_end");
            eve_on_element_out_start = serializedObject.FindProperty("eve_on_element_out_start");
            eve_on_element_out_end = serializedObject.FindProperty("eve_on_element_out_end");
            TextIsFold = serializedObject.FindProperty("TextIsFold");
            TmpTextIsFold = serializedObject.FindProperty("TmpTextIsFold");
            Indicator = serializedObject.FindProperty("Indicator");
            AnimatorsMaxDuration = serializedObject.FindProperty("AnimatorsMaxDuration");
            CreateState = serializedObject.FindProperty("CreateState");
            AnimateState = serializedObject.FindProperty("AnimateState");
            OriginPoolName = serializedObject.FindProperty("OriginPoolName");
            OriginalName = serializedObject.FindProperty("OriginalName");
            CurrentPivot = serializedObject.FindProperty("CurrentPivot");
            Element_Animators_GlobalDuration = serializedObject.FindProperty("Element_Animators_GlobalDuration");
            AnimatorsIsFold = serializedObject.FindProperty("AnimatorsIsFold");
            ButtonIsFold = serializedObject.FindProperty("ButtonIsFold");
            OptionIsFold = serializedObject.FindProperty("OptionIsFold");
            SliderIsFold = serializedObject.FindProperty("SliderIsFold");
            ProgressIsFold = serializedObject.FindProperty("ProgressIsFold");
            ContainerIsFold = serializedObject.FindProperty("ContainerIsFold");
            ToggleIsFold = serializedObject.FindProperty("ToggleIsFold");
            EventIsFold = serializedObject.FindProperty("EventIsFold");
            SounderIsFold = serializedObject.FindProperty("SounderIsFold");
            RMS_Enabled = serializedObject.FindProperty("RMS_Enabled");
            RMS_LayoutDatas = serializedObject.FindProperty("RMS_LayoutDatas");
            RMS_Name = serializedObject.FindProperty("RMS_Name");
            TweensPreivew_In_State = serializedObject.FindProperty("TweensPreivew_In_State");
            TweensPreivew_Out_State = serializedObject.FindProperty("TweensPreivew_Out_State");
            PrimitivePreivew_State = serializedObject.FindProperty("PrimitivePreivew_State");
        }
        #endregion

        #region 预览

        #region Animator 动画预览
        /// <summary>
        /// 预览动画
        /// </summary>
        private void Preview_Animator_Play()
        {
            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

            Tween_Preivewing_Animators = true;
            AnimateState.enumValueIndex = 1;
            AnimateState.serializedObject.ApplyModifiedProperties();

            ///---用来存放调用动画器的动画效果的延迟时间
            List<float> delaytimes = new List<float>();

            ///---动画逻辑
            for (int i = 0; i < BaseScript.AnimatorNodes.Count; i++)
            {
                ElementNode_Animator node = BaseScript.AnimatorNodes[i];
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
                ElementNode_Animator node = BaseScript.AnimatorNodes[i];
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
                    if (Tween_Preview_AutoStop.boolValue)
                        Preivew_Animator_Coroutine_Stop = EditorCoroutineUtility.StartCoroutine(Preview_Animator_Coroutine_Stop(AnimatorsMaxDuration.floatValue), this);
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
        /// 预览指定Animator的动画
        /// </summary>
        private void Preview_Animator_PlayAt(int index)
        {
            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

            Preview_Animator_Stop();

            ElementNode_Animator node = BaseScript.AnimatorNodes[index];
            XHud_Module_Animator anim = node.Animator;

            if (anim == null)
                return;

            if (anim.TweenNode_GetCount() <= 0)
                return;

            Tween_Preivewing_Animators = true;
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
                    if (Tween_Preview_AutoStop.boolValue)
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
            Tween_Preivewing_Animators = false;

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
                if (BaseScript.AnimatorNodes != null && BaseScript.AnimatorNodes.Count > 0)
                {
                    for (int c = 0; c < BaseScript.AnimatorNodes.Count; c++)
                    {
                        XHud_Module_Animator animator = BaseScript.AnimatorNodes[c].Animator;
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
                        AnimatorFeature_Load(animator);
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
                for (int w = 0; w < BaseScript.AnimatorNodes.Count; w++)
                {
                    ElementNode_Animator a_node = BaseScript.AnimatorNodes[w];
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
            XHud_AudioStoper sp = au.gameObject.AddComponent<XHud_AudioStoper>();
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

    }
}