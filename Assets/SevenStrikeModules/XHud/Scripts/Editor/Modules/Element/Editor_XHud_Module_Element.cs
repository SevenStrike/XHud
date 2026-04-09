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
    using SevenStrikeModules.XTween.Editor;
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
        private XHud_Manager HudManager;
        #endregion

        #region GUI 参数
        /// <summary>
        /// 系统默认GUI行高
        /// </summary>
        private float LineHeight;
        /// <summary>
        /// 原始脚本参数显示开关
        /// </summary>
        private bool OriginalDisplay;
        #endregion

        #region 序列化属性
        private SerializedProperty
            CanvasGroup, SounderNodes, PrimitiveControllerNodes, OptionNodes, ButtonNodes, TextIsFold, TmpTextIsFold, SliderNodes, TextNodes, TmpTextNodes, DebugState, PreviewPrimitivesTween, ProgressNodes, ToggleNodes, Indicator, PrimitivesTweenMaxDuration, RectTransform, TriggerAction, ObjectTracker, CreateState, AnimateState, PrimitivesTweenGlobalDuration, OriginPoolName, PrimitivesIsFold, ButtonIsFold, OptionIsFold, SliderIsFold, SounderIsFold, ProgressIsFold, ToggleIsFold, EventIsFold, AutoPlayPrimitivesTween, AutoKillPreviewTweens, Alpha, RMS_Enabled, RMS_LayoutDatas, RMS_Name, CurrentPivot, OriginalName, eve_on_element_in_start, eve_on_element_in_end, eve_on_element_out_start, eve_on_element_out_end, TweensPreivew_In_State, TweensPreivew_Out_State, PrimitivePreivew_State, RewindPreviewTweensWithKill, ClearPreviewTweensWithKill, CreateArgs, RecycleArgs, Crc_Lib_Name, Rec_Lib_Name, CreateArgs_MotionAnimateEndState, RecycleArgs_MotionAnimateEndState, create_fold_move, create_fold_rotate, create_fold_alpha, recycle_fold_move, recycle_fold_rotate, recycle_fold_alpha;
        #endregion       

        #region 图标
        private Texture2D icon_main, icon_sound, icon_anim, icon_button, icon_option, icon_slider, icon_progress, icon_toggle, icon_text, icon_tmptext, icon_scan_r, icon_scan_p, icon_record_rms_r, icon_record_rms_p, resetanchorpos_r, resetanchorpos_p, prw_play_r, prw_play_p, prw_stop_r, prw_stop_p, usestate, animstate, dutation, comp_tracking, elelibsource, locate_r, locate_p, rms_move, rms_rotate, rms_anchor, rms_anchor_center, rms_scale, comp_alpha, comp_transform, comp_trigger, status, left_arrow_r, left_arrow_p, right_arrow_r, right_arrow_p, prw_play_out_r, prw_play_out_p, prw_play_in_p, prw_play_in_r, save_r, save_p, reset_r, reset_p;

        #endregion

        private string PrefsKeyFold_Option = "XHUD-ELEMENT-FOLD-MOTIONARGS";

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
            HudManager = XHud_Dashboard.HudManagerGet();

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
            save_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/save_r");
            save_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/save_p");
            reset_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/reset_r");
            reset_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Element/reset_p");

            #endregion

            // 控件列表绘制
            ReorderableList_Draw_Sounder();
            ReorderableList_Draw_PrimitiveControllerNodes();
            ReorderableList_Draw_Button();
            ReorderableList_Draw_Option();
            ReorderableList_Draw_Slider();
            ReorderableList_Draw_Progress();
            ReorderableList_Draw_Toggle();
            ReorderableList_Draw_Text();
            ReorderableList_Draw_TmpText();

            // RMS 残留信息清理
            if (HudManager != null)
            {
                if (HudManager.hm_RMS_GetResolutionNodes().Length <= 0 && RMS_LayoutDatas.arraySize > 0)
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

            LoadXHudElementPreviewConfig();
        }

        private void OnDisable()
        {
            if (!Application.isPlaying)
            {
                if (BaseScript != null)
                {
                    RMS_Redraw();
                    Preview_XHudSounder_Coroutine_Stop();
                    XTween_Preview_Kill();
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
                GetPrimitivesTween();
                GetAllText();
                GetAllTmpText();
                GetAllSounder();
                GetAllButtons();
                GetAllOptions();
                GetAllSliders();
                GetAllProgress();
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

            #region 预览控制
            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            if (!PreviewPrimitivesTween.boolValue)
            {

                Editor_XHud_GUI.SetEnabled(!TweensPreivew_Out_State.boolValue);

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

                Editor_XHud_GUI.Gui_Layout_Space(40);

                Editor_XHud_GUI.SetEnabled(!TweensPreivew_In_State.boolValue);
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
                Editor_XHud_GUI.SetEnabled(true);
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
                Editor_XHud_GUI.Gui_Layout_Space(75);
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
            #endregion

            Editor_XHud_GUI.Gui_Layout_Space(5);

            #region 元素预览动效参数
            string hexcol = XHud_Utilitys.Color_To_HexColor(XHud_Dashboard.Theme_Primary, true);

            bool sw_option = FunctionGroup("预览动效参数", 5, HudFilled.纯色边框, HudColor.亮白, XHud_Dashboard.Theme_Primary, XHud_Dashboard.Theme_Primary, Color.gray, new RectOffset(0, 0, 0, 0), new Vector2(20, 0), PrefsKeyFold_Option, null);
            if (sw_option)
            {
                //Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 3, "预览动效参数", XHud_Dashboard.Theme_Primary);
                //Editor_XHud_GUI.Gui_Layout_Space(10);

                #region 模版库                             
                //确保动效库存在
                if (HudManager.Hud_Motions != null)
                {
                    //确保动效库不是空的
                    if (HudManager.Hud_Motions.ElementMotionList != null && HudManager.Hud_Motions.ElementMotionList.Count > 0)
                    {
                        Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);

                        //动效列表
                        string[] motnames = HudManager.Hud_Motions.ElementMotion_GetAllName_With_Create();
                        EditorGUI.BeginChangeCheck();
                        Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_Module_Element>("生成", motnames, ref Crc_Lib_Name, HudFilled.实体, 120, 22, SelectedObjects);
                        if (EditorGUI.EndChangeCheck())
                        {
                            Motion_Creator crc = HudManager.Hud_Motions.ElementMotion_GetElementCreator_At_Create(Crc_Lib_Name.stringValue);

                            CreateArgs.FindPropertyRelative("anchor").enumValueIndex = (int)crc.anchor;
                            CreateArgs.FindPropertyRelative("Movement.Movement").enumValueIndex = (int)crc.Movement.Movement;
                            CreateArgs.FindPropertyRelative("Movement.Distance").floatValue = crc.Movement.Distance;
                            CreateArgs.FindPropertyRelative("Movement.Duration").floatValue = crc.Movement.Duration;
                            CreateArgs.FindPropertyRelative("Movement.Delay").floatValue = crc.Movement.Delay;
                            CreateArgs.FindPropertyRelative("Movement.Curve").animationCurveValue = crc.Movement.Curve;
                            CreateArgs.FindPropertyRelative("Movement.CurveName").stringValue = crc.Movement.CurveName;
                            CreateArgs.FindPropertyRelative("Movement.Ease").enumValueIndex = (int)crc.Movement.Ease;
                            CreateArgs.FindPropertyRelative("Rotation.Rotation").enumValueIndex = (int)crc.Rotation.Rotation;
                            CreateArgs.FindPropertyRelative("Rotation.Degree").floatValue = crc.Rotation.Degree;
                            CreateArgs.FindPropertyRelative("Rotation.Duration").floatValue = crc.Rotation.Duration;
                            CreateArgs.FindPropertyRelative("Rotation.Delay").floatValue = crc.Rotation.Delay;
                            CreateArgs.FindPropertyRelative("Rotation.Curve").animationCurveValue = crc.Rotation.Curve;
                            CreateArgs.FindPropertyRelative("Rotation.CurveName").stringValue = crc.Rotation.CurveName;
                            CreateArgs.FindPropertyRelative("Rotation.Ease").enumValueIndex = (int)crc.Rotation.Ease;
                            CreateArgs.FindPropertyRelative("Alpha.Duration").floatValue = crc.Alpha.Duration;
                            CreateArgs.FindPropertyRelative("Alpha.Delay").floatValue = crc.Alpha.Delay;
                            CreateArgs.FindPropertyRelative("Alpha.Curve").animationCurveValue = crc.Alpha.Curve;
                            CreateArgs.FindPropertyRelative("Alpha.CurveName").stringValue = crc.Alpha.CurveName;
                            CreateArgs.FindPropertyRelative("Alpha.Ease").enumValueIndex = (int)crc.Alpha.Ease;
                            CreateArgs.serializedObject.ApplyModifiedProperties();
                        }

                        #region 保存 & 定位模板
                        Editor_XHud_GUI.Gui_Layout_Space(10);

                        if (Editor_XHud_GUI.Gui_Layout_Button(14, "保存", save_r, save_p, 2))
                        {
                            OpenParameterSetter(HudElementMotionType.Creator);
                            return;
                        }

                        Editor_XHud_GUI.Gui_Layout_Space(10);

                        if (Editor_XHud_GUI.Gui_Layout_Button(14, "定位", locate_r, locate_p, 2))
                        {
                            if (!HudManager.Hud_Motions.ElementMotion_IsExist(Crc_Lib_Name.stringValue))
                                return;
                            Editor_XHud_MenuItemsAction_OpenLibrary.open_elementmotion();
                            HudManager.Hud_Motions.ElementMotionLibrary_Location(Crc_Lib_Name.stringValue);
                            return;
                        }

                        Editor_XHud_GUI.Gui_Layout_Space(10);

                        if (Editor_XHud_GUI.Gui_Layout_Button(14, "重置", reset_r, reset_p, 2))
                        {
                            string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 元素预览器消息", "重置动效参数", "确定要将动效参数重置吗？您将丢失当前的动效参数！", "重置", "暂不", 0);
                            if (res == "重置")
                                ResetMotionParams("CreateArgs");
                            return;
                        }

                        Editor_XHud_GUI.Gui_Layout_Space(5);
                        #endregion

                        Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("未在动效库中发现任何动效资源，请先为其添加动效资源!", MessageType.Warning);
                        Editor_XHud_GUI.Gui_Layout_Space(5);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("Hud管理器中未指定动效库，请先配置动效库!", MessageType.Warning);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                }
                #endregion

                #region 位移
                Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_XHud_GUI.Gui_Layout_Space(10);
                create_fold_move.boolValue = EditorGUILayout.Foldout(create_fold_move.boolValue, "位移", true);
                create_fold_move.serializedObject.ApplyModifiedProperties();
                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                if (create_fold_move.boolValue)
                {
                    SerializedProperty sp_move_type = CreateArgs.FindPropertyRelative("Movement.Movement");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("方式", sp_move_type);

                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    SerializedProperty sp_move_dis = CreateArgs.FindPropertyRelative("Movement.Distance");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("距离", sp_move_dis);

                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    SerializedProperty sp_move_dur = CreateArgs.FindPropertyRelative("Movement.Duration");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("耗时", sp_move_dur);

                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    SerializedProperty sp_move_delay = CreateArgs.FindPropertyRelative("Movement.Delay");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("延迟", sp_move_delay);

                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    SerializedProperty sp_move_ease = CreateArgs.FindPropertyRelative("Movement.Ease");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("缓动", sp_move_ease);

                    if ((EaseMode)sp_move_ease.enumValueIndex == EaseMode.None)
                    {
                        Editor_XHud_GUI.Gui_Layout_Space(5);

                        SerializedProperty sp_move_curve = CreateArgs.FindPropertyRelative("Movement.Curve");
                        Editor_XHud_GUI.Gui_Layout_Property_Field("曲线", sp_move_curve);

                        Editor_XHud_GUI.Gui_Layout_Space(5);

                        #region 曲线列表
                        SerializedProperty sp_CurveName = CreateArgs.FindPropertyRelative("Movement.CurveName");
                        SerializedProperty sp_Curve = CreateArgs.FindPropertyRelative("Movement.Curve");
                        if (HudManager.Hud_Curves != null)
                        {
                            if (HudManager.Hud_Curves.CurveLibrary != null && HudManager.Hud_Curves.CurveLibrary.Count > 0)
                            {
                                string[] names = HudManager.Hud_Curves.CurveLibrary_GetCurveNames();
                                Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_Module_Element>("曲线样式", names, ref sp_CurveName, HudFilled.实体, 120, 22, SelectedObjects, (comps) => { }, (res) =>
                                {
                                    sp_Curve.animationCurveValue = HudManager.Hud_Curves.CurveLibrary_GetCurve(res);
                                    sp_Curve.serializedObject.ApplyModifiedProperties();
                                });
                            }
                            else
                            {
                                EditorGUILayout.HelpBox("未在动效库中发现任何曲线资源，请先为其添加曲线资源!", MessageType.Warning);
                                Editor_XHud_GUI.Gui_Layout_Space(5);
                            }
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("Hud管理器中未指定曲线库，请先配置曲线库!", MessageType.Warning);
                            Editor_XHud_GUI.Gui_Layout_Space(5);
                        }
                        #endregion
                    }
                }
                #endregion

                #region 旋转
                Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_XHud_GUI.Gui_Layout_Space(10);
                create_fold_rotate.boolValue = EditorGUILayout.Foldout(create_fold_rotate.boolValue, "旋转", true);
                create_fold_rotate.serializedObject.ApplyModifiedProperties();
                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                if (create_fold_rotate.boolValue)
                {
                    SerializedProperty sp_rot_type = CreateArgs.FindPropertyRelative("Rotation.Rotation");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("方式", sp_rot_type);

                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    SerializedProperty sp_rot_deg = CreateArgs.FindPropertyRelative("Rotation.Degree");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("角度", sp_rot_deg);

                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    SerializedProperty sp_rot_dur = CreateArgs.FindPropertyRelative("Rotation.Duration");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("耗时", sp_rot_dur);

                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    SerializedProperty sp_rot_delay = CreateArgs.FindPropertyRelative("Rotation.Delay");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("延迟", sp_rot_delay);

                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    SerializedProperty sp_rot_ease = CreateArgs.FindPropertyRelative("Rotation.Ease");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("缓动", sp_rot_ease);

                    Editor_XHud_GUI.Gui_Layout_Space(5);


                    if ((EaseMode)sp_rot_ease.enumValueIndex == EaseMode.None)
                    {
                        Editor_XHud_GUI.Gui_Layout_Space(5);

                        SerializedProperty sp_rot_curve = CreateArgs.FindPropertyRelative("Rotation.Curve");
                        Editor_XHud_GUI.Gui_Layout_Property_Field("曲线", sp_rot_curve);

                        Editor_XHud_GUI.Gui_Layout_Space(5);

                        #region 曲线列表
                        SerializedProperty sp_CurveName = CreateArgs.FindPropertyRelative("Rotation.CurveName");
                        SerializedProperty sp_Curve = CreateArgs.FindPropertyRelative("Rotation.Curve");
                        if (HudManager.Hud_Curves != null)
                        {
                            if (HudManager.Hud_Curves.CurveLibrary != null && HudManager.Hud_Curves.CurveLibrary.Count > 0)
                            {
                                string[] names = HudManager.Hud_Curves.CurveLibrary_GetCurveNames();
                                Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_Module_Element>("曲线样式", names, ref sp_CurveName, HudFilled.实体, 120, 22, SelectedObjects, (comps) => { }, (res) =>
                                {
                                    sp_Curve.animationCurveValue = HudManager.Hud_Curves.CurveLibrary_GetCurve(res);
                                    sp_Curve.serializedObject.ApplyModifiedProperties();
                                });
                            }
                            else
                            {
                                EditorGUILayout.HelpBox("未在动效库中发现任何曲线资源，请先为其添加曲线资源!", MessageType.Warning);
                                Editor_XHud_GUI.Gui_Layout_Space(5);
                            }
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("Hud管理器中未指定曲线库，请先配置曲线库!", MessageType.Warning);
                            Editor_XHud_GUI.Gui_Layout_Space(5);
                        }
                        #endregion
                    }
                }
                #endregion

                #region 透明度
                Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_XHud_GUI.Gui_Layout_Space(10);
                create_fold_alpha.boolValue = EditorGUILayout.Foldout(create_fold_alpha.boolValue, "透明度", true);
                create_fold_alpha.serializedObject.ApplyModifiedProperties();
                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                if (create_fold_alpha.boolValue)
                {
                    SerializedProperty sp_alpha_type = CreateArgs.FindPropertyRelative("Alpha.Duration");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("耗时", sp_alpha_type);

                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    SerializedProperty sp_alpha_delay = CreateArgs.FindPropertyRelative("Alpha.Delay");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("延迟", sp_alpha_delay);

                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    SerializedProperty sp_alpha_ease = CreateArgs.FindPropertyRelative("Alpha.Ease");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("缓动", sp_alpha_ease);

                    Editor_XHud_GUI.Gui_Layout_Space(5);


                    if ((EaseMode)sp_alpha_ease.enumValueIndex == EaseMode.None)
                    {
                        Editor_XHud_GUI.Gui_Layout_Space(5);

                        SerializedProperty sp_alpha_curve = CreateArgs.FindPropertyRelative("Alpha.Curve");
                        Editor_XHud_GUI.Gui_Layout_Property_Field("曲线", sp_alpha_curve);

                        Editor_XHud_GUI.Gui_Layout_Space(5);

                        #region 曲线列表
                        SerializedProperty sp_CurveName = CreateArgs.FindPropertyRelative("Alpha.CurveName");
                        SerializedProperty sp_Curve = CreateArgs.FindPropertyRelative("Alpha.Curve");
                        if (HudManager.Hud_Curves != null)
                        {
                            if (HudManager.Hud_Curves.CurveLibrary != null && HudManager.Hud_Curves.CurveLibrary.Count > 0)
                            {
                                string[] names = HudManager.Hud_Curves.CurveLibrary_GetCurveNames();
                                Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_Module_Element>("曲线样式", names, ref sp_CurveName, HudFilled.实体, 120, 22, SelectedObjects, (comps) => { }, (res) =>
                                {
                                    sp_Curve.animationCurveValue = HudManager.Hud_Curves.CurveLibrary_GetCurve(res);
                                    sp_Curve.serializedObject.ApplyModifiedProperties();
                                });
                            }
                            else
                            {
                                EditorGUILayout.HelpBox("未在动效库中发现任何曲线资源，请先为其添加曲线资源!", MessageType.Warning);
                                Editor_XHud_GUI.Gui_Layout_Space(5);
                            }
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("Hud管理器中未指定曲线库，请先配置曲线库!", MessageType.Warning);
                            Editor_XHud_GUI.Gui_Layout_Space(5);
                        }
                        #endregion
                    }
                }
                #endregion

                EditorGUI.BeginChangeCheck();
                Editor_XHud_GUI.Gui_Layout_Property_Field("动效结束时机", CreateArgs_MotionAnimateEndState, 85);
                if (EditorGUI.EndChangeCheck())
                {
                    MotionAnimateEndState state = (MotionAnimateEndState)CreateArgs_MotionAnimateEndState.enumValueIndex;
                    switch (state)
                    {
                        case MotionAnimateEndState.以_移动为准:
                            HudMotion_Movement m = (HudMotion_Movement)CreateArgs.FindPropertyRelative("Movement.Movement").enumValueIndex;
                            if (m == HudMotion_Movement.A_无运动)
                            {
                                Editor_XHud_GUI.Open(XHud_DialogType.警告, $"XHud - 元素预览器消息", "设定动画结束时机", $"当前位移方式为 <color={hexcol}> A_无运动 </color>，将发生<color={hexcol}> 动效动画无法正常结束</color>从而导致<color={hexcol}> 相应的事件和动作委托 </color>不能正确触发的情况 </color>！请将<color={hexcol}> 位移方式 </color>改为<color={hexcol}> 非无运动方式 </color>！", "明白", 0);
                                CreateArgs_MotionAnimateEndState.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
                            }
                            break;
                        case MotionAnimateEndState.以_旋转为准:
                            HudMotion_Rotation r = (HudMotion_Rotation)CreateArgs.FindPropertyRelative("Rotation.Rotation").enumValueIndex;
                            if (r == HudMotion_Rotation.A_无旋转)
                            {
                                Editor_XHud_GUI.Open(XHud_DialogType.警告, $"XHud - 元素预览器消息", "设定动画结束时机", $"当前旋转方式为 <color={hexcol}> A_无旋转 </color>，将发生<color={hexcol}> 动效动画无法正常结束</color>从而导致<color={hexcol}> 相应的事件和动作委托 </color>不能正确触发的情况 </color>！请将<color={hexcol}> 旋转方式 </color>改为<color={hexcol}> 非无旋转方式 </color>！", "明白", 0);
                                CreateArgs_MotionAnimateEndState.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
                            }
                            break;
                    }
                }

                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Seperator(1, XHud_Dashboard.Theme_SeperateLine);
                Editor_XHud_GUI.Gui_Layout_Space(5);

                #region 模版库                             
                //确保动效库存在
                if (HudManager.Hud_Motions != null)
                {
                    //确保动效库不是空的
                    if (HudManager.Hud_Motions.ElementMotionList != null && HudManager.Hud_Motions.ElementMotionList.Count > 0)
                    {
                        Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);

                        //动效列表
                        string[] motnames = HudManager.Hud_Motions.ElementMotion_GetAllName_With_Recycle();
                        EditorGUI.BeginChangeCheck();
                        Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_Module_Element>("回收", motnames, ref Rec_Lib_Name, HudFilled.实体, 120, 22, SelectedObjects);
                        if (EditorGUI.EndChangeCheck())
                        {
                            Motion_Recycler rec = HudManager.Hud_Motions.ElementMotion_GetElementCreator_At_Recycle(Rec_Lib_Name.stringValue);

                            RecycleArgs.FindPropertyRelative("Movement.Movement").enumValueIndex = (int)rec.Movement.Movement;
                            RecycleArgs.FindPropertyRelative("Movement.Distance").floatValue = rec.Movement.Distance;
                            RecycleArgs.FindPropertyRelative("Movement.Duration").floatValue = rec.Movement.Duration;
                            RecycleArgs.FindPropertyRelative("Movement.Delay").floatValue = rec.Movement.Delay;
                            RecycleArgs.FindPropertyRelative("Movement.Curve").animationCurveValue = rec.Movement.Curve;
                            RecycleArgs.FindPropertyRelative("Movement.CurveName").stringValue = rec.Movement.CurveName;
                            RecycleArgs.FindPropertyRelative("Movement.Ease").enumValueIndex = (int)rec.Movement.Ease;
                            RecycleArgs.FindPropertyRelative("Rotation.Rotation").enumValueIndex = (int)rec.Rotation.Rotation;
                            RecycleArgs.FindPropertyRelative("Rotation.Degree").floatValue = rec.Rotation.Degree;
                            RecycleArgs.FindPropertyRelative("Rotation.Duration").floatValue = rec.Rotation.Duration;
                            RecycleArgs.FindPropertyRelative("Rotation.Delay").floatValue = rec.Rotation.Delay;
                            RecycleArgs.FindPropertyRelative("Rotation.Curve").animationCurveValue = rec.Rotation.Curve;
                            RecycleArgs.FindPropertyRelative("Rotation.CurveName").stringValue = rec.Rotation.CurveName;
                            RecycleArgs.FindPropertyRelative("Rotation.Ease").enumValueIndex = (int)rec.Rotation.Ease;
                            RecycleArgs.FindPropertyRelative("Alpha.Duration").floatValue = rec.Alpha.Duration;
                            RecycleArgs.FindPropertyRelative("Alpha.Delay").floatValue = rec.Alpha.Delay;
                            RecycleArgs.FindPropertyRelative("Alpha.Curve").animationCurveValue = rec.Alpha.Curve;
                            RecycleArgs.FindPropertyRelative("Alpha.CurveName").stringValue = rec.Alpha.CurveName;
                            RecycleArgs.FindPropertyRelative("Alpha.Ease").enumValueIndex = (int)rec.Alpha.Ease;
                            RecycleArgs.serializedObject.ApplyModifiedProperties();
                        }

                        #region 保存 & 定位模板
                        Editor_XHud_GUI.Gui_Layout_Space(10);

                        if (Editor_XHud_GUI.Gui_Layout_Button(14, "保存", save_r, save_p, 2))
                        {
                            OpenParameterSetter(HudElementMotionType.Recycler);
                            return;
                        }

                        Editor_XHud_GUI.Gui_Layout_Space(10);

                        if (Editor_XHud_GUI.Gui_Layout_Button(14, "定位", locate_r, locate_p, 2))
                        {
                            if (!HudManager.Hud_Motions.ElementMotion_IsExist(Rec_Lib_Name.stringValue, HudElementMotionType.Recycler))
                                return;
                            Editor_XHud_MenuItemsAction_OpenLibrary.open_elementmotion();
                            HudManager.Hud_Motions.ElementMotionLibrary_Location(Rec_Lib_Name.stringValue);
                            return;
                        }

                        Editor_XHud_GUI.Gui_Layout_Space(10);

                        if (Editor_XHud_GUI.Gui_Layout_Button(14, "重置", reset_r, reset_p, 2))
                        {
                            string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 元素预览器消息", "重置动效参数", "确定要将动效参数重置吗？您将丢失当前的动效参数！", "重置", "暂不", 0);
                            if (res == "重置")
                                ResetMotionParams("RecycleArgs");
                            return;
                        }

                        Editor_XHud_GUI.Gui_Layout_Space(5);
                        #endregion

                        Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("未在动效库中发现任何动效资源，请先为其添加动效资源!", MessageType.Warning);
                        Editor_XHud_GUI.Gui_Layout_Space(5);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("Hud管理器中未指定动效库，请先配置动效库!", MessageType.Warning);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                }
                #endregion

                #region 位移
                Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_XHud_GUI.Gui_Layout_Space(10);
                recycle_fold_move.boolValue = EditorGUILayout.Foldout(recycle_fold_move.boolValue, "位移", true);
                recycle_fold_move.serializedObject.ApplyModifiedProperties();
                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                if (recycle_fold_move.boolValue)
                {
                    SerializedProperty sp_move_type = RecycleArgs.FindPropertyRelative("Movement.Movement");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("方式", sp_move_type);

                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    SerializedProperty sp_move_dis = RecycleArgs.FindPropertyRelative("Movement.Distance");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("距离", sp_move_dis);

                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    SerializedProperty sp_move_dur = RecycleArgs.FindPropertyRelative("Movement.Duration");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("耗时", sp_move_dur);

                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    SerializedProperty sp_move_delay = RecycleArgs.FindPropertyRelative("Movement.Delay");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("延迟", sp_move_delay);

                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    SerializedProperty sp_move_ease = RecycleArgs.FindPropertyRelative("Movement.Ease");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("缓动", sp_move_ease);

                    if ((EaseMode)sp_move_ease.enumValueIndex == EaseMode.None)
                    {
                        Editor_XHud_GUI.Gui_Layout_Space(5);

                        SerializedProperty sp_move_curve = RecycleArgs.FindPropertyRelative("Movement.Curve");
                        Editor_XHud_GUI.Gui_Layout_Property_Field("曲线", sp_move_curve);

                        Editor_XHud_GUI.Gui_Layout_Space(5);

                        #region 曲线列表
                        SerializedProperty sp_CurveName = RecycleArgs.FindPropertyRelative("Movement.CurveName");
                        SerializedProperty sp_Curve = RecycleArgs.FindPropertyRelative("Movement.Curve");
                        if (HudManager.Hud_Curves != null)
                        {
                            if (HudManager.Hud_Curves.CurveLibrary != null && HudManager.Hud_Curves.CurveLibrary.Count > 0)
                            {
                                string[] names = HudManager.Hud_Curves.CurveLibrary_GetCurveNames();
                                Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_Module_Element>("曲线样式", names, ref sp_CurveName, HudFilled.实体, 120, 22, SelectedObjects, (comps) => { }, (res) =>
                                {
                                    sp_Curve.animationCurveValue = HudManager.Hud_Curves.CurveLibrary_GetCurve(res);
                                    sp_Curve.serializedObject.ApplyModifiedProperties();
                                });
                            }
                            else
                            {
                                EditorGUILayout.HelpBox("未在动效库中发现任何曲线资源，请先为其添加曲线资源!", MessageType.Warning);
                                Editor_XHud_GUI.Gui_Layout_Space(5);
                            }
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("Hud管理器中未指定曲线库，请先配置曲线库!", MessageType.Warning);
                            Editor_XHud_GUI.Gui_Layout_Space(5);
                        }
                        #endregion
                    }
                }
                #endregion

                #region 旋转
                Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_XHud_GUI.Gui_Layout_Space(10);
                recycle_fold_rotate.boolValue = EditorGUILayout.Foldout(recycle_fold_rotate.boolValue, "旋转", true);
                recycle_fold_rotate.serializedObject.ApplyModifiedProperties();
                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                if (recycle_fold_rotate.boolValue)
                {
                    SerializedProperty sp_rot_type = RecycleArgs.FindPropertyRelative("Rotation.Rotation");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("方式", sp_rot_type);

                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    SerializedProperty sp_rot_deg = RecycleArgs.FindPropertyRelative("Rotation.Degree");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("角度", sp_rot_deg);

                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    SerializedProperty sp_rot_dur = RecycleArgs.FindPropertyRelative("Rotation.Duration");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("耗时", sp_rot_dur);

                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    SerializedProperty sp_rot_delay = RecycleArgs.FindPropertyRelative("Rotation.Delay");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("延迟", sp_rot_delay);

                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    SerializedProperty sp_rot_ease = RecycleArgs.FindPropertyRelative("Rotation.Ease");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("缓动", sp_rot_ease);

                    if ((EaseMode)sp_rot_ease.enumValueIndex == EaseMode.None)
                    {
                        Editor_XHud_GUI.Gui_Layout_Space(5);

                        SerializedProperty sp_rot_curve = RecycleArgs.FindPropertyRelative("Rotation.Curve");
                        Editor_XHud_GUI.Gui_Layout_Property_Field("曲线", sp_rot_curve);

                        Editor_XHud_GUI.Gui_Layout_Space(5);

                        #region 曲线列表
                        SerializedProperty sp_CurveName = RecycleArgs.FindPropertyRelative("Rotation.CurveName");
                        SerializedProperty sp_Curve = RecycleArgs.FindPropertyRelative("Rotation.Curve");
                        if (HudManager.Hud_Curves != null)
                        {
                            if (HudManager.Hud_Curves.CurveLibrary != null && HudManager.Hud_Curves.CurveLibrary.Count > 0)
                            {
                                string[] names = HudManager.Hud_Curves.CurveLibrary_GetCurveNames();
                                Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_Module_Element>("曲线样式", names, ref sp_CurveName, HudFilled.实体, 120, 22, SelectedObjects, (comps) => { }, (res) =>
                                {
                                    sp_Curve.animationCurveValue = HudManager.Hud_Curves.CurveLibrary_GetCurve(res);
                                    sp_Curve.serializedObject.ApplyModifiedProperties();
                                });
                            }
                            else
                            {
                                EditorGUILayout.HelpBox("未在动效库中发现任何曲线资源，请先为其添加曲线资源!", MessageType.Warning);
                                Editor_XHud_GUI.Gui_Layout_Space(5);
                            }
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("Hud管理器中未指定曲线库，请先配置曲线库!", MessageType.Warning);
                            Editor_XHud_GUI.Gui_Layout_Space(5);
                        }
                        #endregion
                    }
                }
                #endregion

                #region 透明度
                Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_XHud_GUI.Gui_Layout_Space(10);
                recycle_fold_alpha.boolValue = EditorGUILayout.Foldout(recycle_fold_alpha.boolValue, "透明度", true);
                recycle_fold_alpha.serializedObject.ApplyModifiedProperties();
                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                if (recycle_fold_alpha.boolValue)
                {
                    SerializedProperty sp_alpha_type = RecycleArgs.FindPropertyRelative("Alpha.Duration");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("耗时", sp_alpha_type);

                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    SerializedProperty sp_alpha_delay = RecycleArgs.FindPropertyRelative("Alpha.Delay");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("延迟", sp_alpha_delay);

                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    SerializedProperty sp_alpha_ease = RecycleArgs.FindPropertyRelative("Alpha.Ease");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("缓动", sp_alpha_ease);

                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    if ((EaseMode)sp_alpha_ease.enumValueIndex == EaseMode.None)
                    {
                        Editor_XHud_GUI.Gui_Layout_Space(5);

                        SerializedProperty sp_alpha_curve = RecycleArgs.FindPropertyRelative("Alpha.Curve");
                        Editor_XHud_GUI.Gui_Layout_Property_Field("曲线", sp_alpha_curve);

                        Editor_XHud_GUI.Gui_Layout_Space(5);

                        #region 曲线列表
                        SerializedProperty sp_CurveName = RecycleArgs.FindPropertyRelative("Alpha.CurveName");
                        SerializedProperty sp_Curve = RecycleArgs.FindPropertyRelative("Alpha.Curve");
                        if (HudManager.Hud_Curves != null)
                        {
                            if (HudManager.Hud_Curves.CurveLibrary != null && HudManager.Hud_Curves.CurveLibrary.Count > 0)
                            {
                                string[] names = HudManager.Hud_Curves.CurveLibrary_GetCurveNames();
                                Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_Module_Element>("曲线样式", names, ref sp_CurveName, HudFilled.实体, 120, 22, SelectedObjects, (comps) => { }, (res) =>
                                {
                                    sp_Curve.animationCurveValue = HudManager.Hud_Curves.CurveLibrary_GetCurve(res);
                                    sp_Curve.serializedObject.ApplyModifiedProperties();
                                });
                            }
                            else
                            {
                                EditorGUILayout.HelpBox("未在动效库中发现任何曲线资源，请先为其添加曲线资源!", MessageType.Warning);
                                Editor_XHud_GUI.Gui_Layout_Space(5);
                            }
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("Hud管理器中未指定曲线库，请先配置曲线库!", MessageType.Warning);
                            Editor_XHud_GUI.Gui_Layout_Space(5);
                        }
                        #endregion
                    }
                }
                #endregion

                EditorGUI.BeginChangeCheck();
                Editor_XHud_GUI.Gui_Layout_Property_Field("动效结束时机", RecycleArgs_MotionAnimateEndState, 85);
                if (EditorGUI.EndChangeCheck())
                {
                    MotionAnimateEndState state = (MotionAnimateEndState)RecycleArgs_MotionAnimateEndState.enumValueIndex;
                    switch (state)
                    {
                        case MotionAnimateEndState.以_移动为准:
                            HudMotion_Movement m = (HudMotion_Movement)RecycleArgs.FindPropertyRelative("Movement.Movement").enumValueIndex;
                            if (m == HudMotion_Movement.A_无运动)
                            {
                                Editor_XHud_GUI.Open(XHud_DialogType.警告, $"XHud - 元素预览器消息", "设定动画结束时机", $"当前位移方式为 <color={hexcol}> A_无运动 </color>，将发生<color={hexcol}> 动效动画无法正常结束</color>从而导致<color={hexcol}> 相应的事件和动作委托 </color>不能正确触发的情况 </color>！请将<color={hexcol}> 位移方式 </color>改为<color={hexcol}> 非无运动方式 </color>！", "明白", 0);
                                RecycleArgs_MotionAnimateEndState.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
                            }
                            break;
                        case MotionAnimateEndState.以_旋转为准:
                            HudMotion_Rotation r = (HudMotion_Rotation)RecycleArgs.FindPropertyRelative("Rotation.Rotation").enumValueIndex;
                            if (r == HudMotion_Rotation.A_无旋转)
                            {
                                Editor_XHud_GUI.Open(XHud_DialogType.警告, $"XHud - 元素预览器消息", "设定动画结束时机", $"当前旋转方式为 <color={hexcol}> A_无旋转 </color>，将发生<color={hexcol}> 动效动画无法正常结束</color>从而导致<color={hexcol}> 相应的事件和动作委托 </color>不能正确触发的情况 </color>！请将<color={hexcol}> 旋转方式 </color>改为<color={hexcol}> 非无旋转方式 </color>！", "明白", 0);
                                RecycleArgs_MotionAnimateEndState.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
                            }
                            break;
                    }
                }
            }
            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            Editor_XHud_GUI.Gui_Layout_Space(10);

            if (Editor_XHud_GUI.Gui_Layout_Button("添加运行时预览组件", "", HudFilled.实体, HudColor.深空灰, Color.white, 25))
            {
                XHud_Element_Preview xHud_Element_Preview = BaseScript.GetComponent<XHud_Element_Preview>();
                if (xHud_Element_Preview == null)
                {
                    xHud_Element_Preview = Undo.AddComponent<XHud_Element_Preview>(BaseScript.gameObject);
                    EditorApplication.delayCall += () =>
                    {
                        // 收集原始姿态数据
                        xHud_Element_Preview.OriginalDataCollect();

                        xHud_Element_Preview.CreateArgs.Movement.Movement = HudMotion_Movement.S_从下至上;
                        xHud_Element_Preview.CreateArgs.Movement.Distance = 100;
                        xHud_Element_Preview.RecycleArgs.Movement.Movement = HudMotion_Movement.D_从上至下;
                        xHud_Element_Preview.RecycleArgs.Movement.Distance = 100;
                    };
                }
            }

            Editor_XHud_GUI.Gui_Layout_Space(10);
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
            Editor_XHud_GUI.Gui_Layout_Property_Field("速率倍增", PrimitivesTweenGlobalDuration);

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
            EditorGUI.BeginChangeCheck();
            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_Element>("预览自动停止", stroptions_enabled, ref AutoKillPreviewTweens, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
            if (EditorGUI.EndChangeCheck())
            {
                XHud_Dashboard.Set_PreviewOption_AutoKillPreviewTweens(AutoKillPreviewTweens.boolValue);
                Editor_XTween_Previewer.AutoKillWithDuration = AutoKillPreviewTweens.boolValue;

                // 如果为自动杀死动画则会强制开启：杀死前重置动画 / 杀死后清空预览列表
                if (AutoKillPreviewTweens.boolValue)
                {
                    // 强制开启：杀死前重置动画 / 杀死后清空预览列表
                    RewindPreviewTweensWithKill.boolValue = true;
                    RewindPreviewTweensWithKill.serializedObject.ApplyModifiedProperties();
                    ClearPreviewTweensWithKill.boolValue = true;
                    ClearPreviewTweensWithKill.serializedObject.ApplyModifiedProperties();

                    // 保存配置数据：杀死前重置动画
                    XHud_Dashboard.Set_PreviewOption_RewindPreviewTweensWithKill(RewindPreviewTweensWithKill.boolValue);

                    // 预览器设置：杀死预览动画前重置动画
                    Editor_XTween_Previewer.BeforeKillRewind = RewindPreviewTweensWithKill.boolValue;

                    // 保存配置数据：杀死后清空预览列表
                    XHud_Dashboard.Set_PreviewOption_ClearPreviewTweensWithKill(ClearPreviewTweensWithKill.boolValue);

                    // 预览器设置：杀死后清空预览列表
                    Editor_XTween_Previewer.AfterKillClear = ClearPreviewTweensWithKill.boolValue;
                }

                // 保存预览自动停止开关状态
                AutoKillPreviewTweens.serializedObject.ApplyModifiedProperties();

                // 保存XHud元素的动画预览配置数据
                XHud_Dashboard.ElementPreviewOptionsConfig_Save();
            }
            #endregion

            #region 预览杀死前先重置
            if (!AutoKillPreviewTweens.boolValue)
            {
                EditorGUI.BeginChangeCheck();
                Editor_XTween_GUI.Gui_Layout_Toggle<bool, XHud_Module_Element>("预览杀死前先重置动画", new string[2] { "禁用", "启用" }, ref RewindPreviewTweensWithKill, XTweenGUIFilled.无, XTweenGUIFilled.实体, Color.white, 120, 22, SelectedObjects);
                if (EditorGUI.EndChangeCheck())
                {
                    // 保存配置数据：杀死前重置动画
                    XHud_Dashboard.Set_PreviewOption_RewindPreviewTweensWithKill(RewindPreviewTweensWithKill.boolValue);

                    // 预览器设置：杀死预览动画前重置动画
                    Editor_XTween_Previewer.BeforeKillRewind = RewindPreviewTweensWithKill.boolValue;

                    // 保存XHud元素的动画预览配置数据
                    XHud_Dashboard.ElementPreviewOptionsConfig_Save();
                }
            }
            #endregion

            #region 预览杀死后清空预览器列表
            if (!AutoKillPreviewTweens.boolValue)
            {
                EditorGUI.BeginChangeCheck();
                Editor_XTween_GUI.Gui_Layout_Toggle<bool, XHud_Module_Element>("预览杀死后清空预览器列表", new string[2] { "禁用", "启用" }, ref ClearPreviewTweensWithKill, XTweenGUIFilled.无, XTweenGUIFilled.实体, Color.white, 120, 22, SelectedObjects);
                if (EditorGUI.EndChangeCheck())
                {
                    // 保存配置数据：杀死后清空预览列表
                    XHud_Dashboard.Set_PreviewOption_ClearPreviewTweensWithKill(ClearPreviewTweensWithKill.boolValue);

                    // 预览器设置：杀死后清空预览列表
                    Editor_XTween_Previewer.AfterKillClear = ClearPreviewTweensWithKill.boolValue;

                    // 保存XHud元素的动画预览配置数据
                    XHud_Dashboard.ElementPreviewOptionsConfig_Save();
                }
            }
            #endregion

            #region 元素下动画器自动播放
            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_Element>("动画器自动播放", stroptions_auto, ref AutoPlayPrimitivesTween, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
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
                Editor_XHud_GUI.StatuDisplayer_text(usestate, 12, new Vector2(0, 7), "使用状态", 12, (XHudElementCreateState)CreateState.enumValueIndex == XHudElementCreateState.Created ? "已被生成" : "已被回收", CreateState.boolValue ? XHud_Dashboard.Theme_Primary : Color.gray, 11);
                #endregion

                #region 动画相关
                if (PrimitiveControllerNodes != null && PrimitiveControllerNodes.arraySize > 0)
                {
                    #region 动画状态     
                    Editor_XHud_GUI.StatuDisplayer_text(animstate, 12, new Vector2(0, 7), "动画状态", 12, (XHudElementAnimateState)AnimateState.enumValueIndex == XHudElementAnimateState.Animating ? "动画中" : "静止状态", AnimateState.enumValueIndex == 1 ? XHud_Dashboard.Theme_Primary : Color.gray, 11);
                    #endregion

                    #region 最大耗时     
                    Editor_XHud_GUI.StatuDisplayer_text(dutation, 12, new Vector2(0, 7), "最大耗时<color=#909090>（速率倍增）</color>", 12, PrimitivesTweenMaxDuration.floatValue.ToString() + " 秒", XHud_Dashboard.Theme_Primary, 11);
                    #endregion

                    #region 最大耗时        
                    if (HudManager != null)
                        Editor_XHud_GUI.StatuDisplayer_text(dutation, 12, new Vector2(0, 7), "最大耗时<color=#909090>（XHUD倍增）</color>", 12, (HudManager.DurationMultiply * PrimitivesTweenMaxDuration.floatValue).ToString() + " 秒", XHud_Dashboard.Theme_Primary, 11);
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
                        foreach (XHud_Library_Element lib in HudManager.Hud_ElementLibrarys)
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
                Editor_XHud_GUI.StatuDisplayer_text(usestate, 12, new Vector2(0, 7), "使用状态", 12, SelectedObjects[ElementStatu_Index].CreateState == XHudElementCreateState.Created ? "已被生成" : "已被回收", CreateState.boolValue ? XHud_Dashboard.Theme_Primary : Color.gray, 11);
                #endregion

                #region 动画相关
                if (SelectedObjects[ElementStatu_Index].PrimitiveControllerNodes != null && SelectedObjects[ElementStatu_Index].PrimitiveControllerNodes.Count > 0)
                {
                    #region 动画状态     
                    Editor_XHud_GUI.StatuDisplayer_text(animstate, 12, new Vector2(0, 7), "动画状态", 12, SelectedObjects[ElementStatu_Index].AnimateState == XHudElementAnimateState.Animating ? "动画中" : "静止状态", AnimateState.enumValueIndex == 1 ? XHud_Dashboard.Theme_Primary : Color.gray, 11);
                    #endregion

                    SelectedObjects[ElementStatu_Index].PrimitivesTweenMaxDuration = Animators_GetAnimatorsMaxDuration(SelectedObjects[ElementStatu_Index].PrimitiveControllerNodes, SelectedObjects[ElementStatu_Index].PrimitivesTweenGlobalDuration);

                    #region 最大耗时     
                    Editor_XHud_GUI.StatuDisplayer_text(dutation, 12, new Vector2(0, 7), "最大耗时<color=#909090>（元素倍增）</color>", 12, SelectedObjects[ElementStatu_Index].PrimitivesTweenMaxDuration.ToString() + " 秒", XHud_Dashboard.Theme_Primary, 11);
                    #endregion

                    #region 最大耗时        
                    Editor_XHud_GUI.StatuDisplayer_text(dutation, 12, new Vector2(0, 7), "最大耗时<color=#909090>（XHUD倍增）</color>", 12, (HudManager.DurationMultiply * SelectedObjects[ElementStatu_Index].PrimitivesTweenMaxDuration).ToString() + " 秒", XHud_Dashboard.Theme_Primary, 11);
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
                    foreach (XHud_Library_Element lib in HudManager.Hud_ElementLibrarys)
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
                if (HudManager != null)
                {
                    //ScreenResolutionNode[] nodes = HudManager.hm_RMS_GetResolutionNodes();
                    string[] nodesName = HudManager.hm_RMS_GetResolutionNodeNames();
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
            if (PrimitiveControllerNodes.arraySize > 0)
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
                    PrimitivesIsFold.boolValue = EditorGUILayout.Foldout(PrimitivesIsFold.boolValue, "动画器", true);
                    PrimitivesIsFold.serializedObject.ApplyModifiedProperties();
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    if (PrimitivesIsFold.boolValue)
                        PrimitivesTweenList.DoLayoutList();

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
                if (SounderNodes.arraySize <= 0 && PrimitiveControllerNodes.arraySize <= 0 && ButtonNodes.arraySize <= 0 && OptionNodes.arraySize <= 0 &&
                    SliderNodes.arraySize <= 0 && ProgressNodes.arraySize <= 0 && ToggleNodes.arraySize <= 0 && TextNodes.arraySize <= 0 && TmpTextNodes.arraySize <= 0)
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
                    if (PrimitiveControllerNodes.arraySize > 0)
                    {
                        Editor_XHud_GUI.StatuDisplayer_text(icon_anim, 12, new Vector2(0, 7), "动画器", 12, PrimitiveControllerNodes.arraySize.ToString() + " 个", XHud_Dashboard.Theme_Primary, 11);
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

                if (SelectedObjects[ElementStatistic_Index].SounderNodes.Count <= 0 && SelectedObjects[ElementStatistic_Index].PrimitiveControllerNodes.Count <= 0 && SelectedObjects[ElementStatistic_Index].ButtonNodes.Count <= 0 && OptionNodes.arraySize <= 0 &&
                   SelectedObjects[ElementStatistic_Index].SliderNodes.Count <= 0 && SelectedObjects[ElementStatistic_Index].ProgressNodes.Count <= 0 && SelectedObjects[ElementStatistic_Index].ToggleNodes.Count <= 0 && SelectedObjects[ElementStatistic_Index].TextNodes.Count <= 0 && SelectedObjects[ElementStatistic_Index].TmpTextNodes.Count <= 0)
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
                    if (SelectedObjects[ElementStatistic_Index].PrimitiveControllerNodes.Count > 0)
                    {
                        Editor_XHud_GUI.StatuDisplayer_text(icon_anim, 12, new Vector2(0, 7), "动画器", 12, SelectedObjects[ElementStatistic_Index].PrimitiveControllerNodes.Count.ToString() + " 个", XHud_Dashboard.Theme_Primary, 11);
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

            #region 弹出菜单
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
                    GetPrimitivesTween();
                    GetAllText();
                    GetAllTmpText();
                    GetAllSounder();
                    GetAllButtons();
                    GetAllOptions();
                    GetAllSliders();
                    GetAllProgress();
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
                menu.AddDisabledItem(new GUIContent("动效快速操作"));
                menu.AddItem(new GUIContent("E (复制动效)"), false, () =>
                {
                    string res = Editor_XHud_GUI.Open(XHud_DialogType.修改, "XHud - 元素预览器消息", "复制预览动效", "请选择动效参数复制模式！", "取消", "生成", "回收", 0);
                    if (res == "取消")
                        return;

                    MotionNode_Movement M = null;
                    MotionNode_Rotation R = null;
                    MotionNode_Alpha A = null;
                    string json = "";

                    switch (res)
                    {
                        case "生成"://生成
                            Motion_Creator crc = new Motion_Creator();
                            crc.anchor = (XHudAnchor)CreateArgs.FindPropertyRelative("anchor").enumValueIndex;

                            M = new MotionNode_Movement();
                            M.Movement = (HudMotion_Movement)CreateArgs.FindPropertyRelative("Movement.Movement").enumValueIndex;
                            M.Distance = CreateArgs.FindPropertyRelative("Movement.Distance").floatValue;
                            M.Duration = CreateArgs.FindPropertyRelative("Movement.Duration").floatValue;
                            M.Delay = CreateArgs.FindPropertyRelative("Movement.Delay").floatValue;
                            M.Ease = (EaseMode)CreateArgs.FindPropertyRelative("Movement.Ease").enumValueIndex;
                            M.Curve = CreateArgs.FindPropertyRelative("Movement.Curve").animationCurveValue;
                            M.CurveName = CreateArgs.FindPropertyRelative("Movement.CurveName").stringValue;

                            R = new MotionNode_Rotation();
                            R.Rotation = (HudMotion_Rotation)CreateArgs.FindPropertyRelative("Rotation.Rotation").enumValueIndex;
                            R.Degree = CreateArgs.FindPropertyRelative("Rotation.Degree").floatValue;
                            R.Duration = CreateArgs.FindPropertyRelative("Rotation.Duration").floatValue;
                            R.Delay = CreateArgs.FindPropertyRelative("Rotation.Delay").floatValue;
                            R.Ease = (EaseMode)CreateArgs.FindPropertyRelative("Rotation.Ease").enumValueIndex;
                            R.Curve = CreateArgs.FindPropertyRelative("Rotation.Curve").animationCurveValue;
                            R.CurveName = CreateArgs.FindPropertyRelative("Rotation.CurveName").stringValue;

                            A = new MotionNode_Alpha();
                            A.Duration = CreateArgs.FindPropertyRelative("Alpha.Duration").floatValue;
                            A.Curve = CreateArgs.FindPropertyRelative("Alpha.Curve").animationCurveValue;
                            A.CurveName = CreateArgs.FindPropertyRelative("Alpha.CurveName").stringValue;
                            A.Ease = (EaseMode)CreateArgs.FindPropertyRelative("Alpha.Ease").enumValueIndex;
                            A.Delay = CreateArgs.FindPropertyRelative("Alpha.Delay").floatValue;

                            crc.Movement = M;
                            crc.Rotation = R;
                            crc.Alpha = A;

                            json = JsonUtility.ToJson(crc);
                            GUIUtility.systemCopyBuffer = json;
                            break;
                        case "回收"://回收
                            Motion_Recycler rec = new Motion_Recycler();

                            M = new MotionNode_Movement();
                            M.Movement = (HudMotion_Movement)RecycleArgs.FindPropertyRelative("Movement.Movement").enumValueIndex;
                            M.Distance = RecycleArgs.FindPropertyRelative("Movement.Distance").floatValue;
                            M.Duration = RecycleArgs.FindPropertyRelative("Movement.Duration").floatValue;
                            M.Delay = RecycleArgs.FindPropertyRelative("Movement.Delay").floatValue;
                            M.Ease = (EaseMode)RecycleArgs.FindPropertyRelative("Movement.Ease").enumValueIndex;
                            M.Curve = RecycleArgs.FindPropertyRelative("Movement.Curve").animationCurveValue;
                            M.CurveName = RecycleArgs.FindPropertyRelative("Movement.CurveName").stringValue;

                            R = new MotionNode_Rotation();
                            R.Rotation = (HudMotion_Rotation)RecycleArgs.FindPropertyRelative("Rotation.Rotation").enumValueIndex;
                            R.Degree = RecycleArgs.FindPropertyRelative("Rotation.Degree").floatValue;
                            R.Duration = RecycleArgs.FindPropertyRelative("Rotation.Duration").floatValue;
                            R.Delay = RecycleArgs.FindPropertyRelative("Rotation.Delay").floatValue;
                            R.Ease = (EaseMode)RecycleArgs.FindPropertyRelative("Rotation.Ease").enumValueIndex;
                            R.Curve = RecycleArgs.FindPropertyRelative("Rotation.Curve").animationCurveValue;
                            R.CurveName = RecycleArgs.FindPropertyRelative("Rotation.CurveName").stringValue;

                            A = new MotionNode_Alpha();
                            A.Duration = RecycleArgs.FindPropertyRelative("Alpha.Duration").floatValue;
                            A.Curve = RecycleArgs.FindPropertyRelative("Alpha.Curve").animationCurveValue;
                            A.CurveName = RecycleArgs.FindPropertyRelative("Alpha.CurveName").stringValue;
                            A.Ease = (EaseMode)RecycleArgs.FindPropertyRelative("Alpha.Ease").enumValueIndex;
                            A.Delay = RecycleArgs.FindPropertyRelative("Alpha.Delay").floatValue;

                            rec.Movement = M;
                            rec.Rotation = R;
                            rec.Alpha = A;

                            json = JsonUtility.ToJson(rec);
                            GUIUtility.systemCopyBuffer = json;
                            break;
                    }

                    string mode = "";

                    if (res == "生成")
                        mode = "生成动效参数";
                    else if (res == "回收")
                        mode = "回收动效参数";

                    Editor_XHud_GUI.Open(XHud_DialogType.确认, "XHud - 元素预览器消息", "复制预览动效", $"已复制 \" {mode} \" 到系统剪贴板 ！", "明白", 0);
                });
                menu.AddItem(new GUIContent("R (粘贴动效)"), false, () =>
                {
                    string buffer = GUIUtility.systemCopyBuffer;
                    if (buffer.Contains("anchor"))//粘贴生成参数
                    {
                        string res = Editor_XHud_GUI.Open(XHud_DialogType.修改, "XHud - 元素预览器消息", "粘贴预览动效", "检测到动效参数类型为： \"生成动效\"，确定要使用这个参数吗？", "确定", "暂不", 0);
                        if (res == "暂不")
                            return;

                        Motion_Creator crc = JsonUtility.FromJson<Motion_Creator>(GUIUtility.systemCopyBuffer);

                        CreateArgs.FindPropertyRelative("anchor").enumValueIndex = (int)crc.anchor;

                        CreateArgs.FindPropertyRelative("Movement.Movement").enumValueIndex = (int)crc.Movement.Movement;
                        CreateArgs.FindPropertyRelative("Movement.Distance").floatValue = crc.Movement.Distance;
                        CreateArgs.FindPropertyRelative("Movement.Duration").floatValue = crc.Movement.Duration;
                        CreateArgs.FindPropertyRelative("Movement.Delay").floatValue = crc.Movement.Delay;
                        CreateArgs.FindPropertyRelative("Movement.Ease").enumValueIndex = (int)crc.Movement.Ease;
                        CreateArgs.FindPropertyRelative("Movement.Curve").animationCurveValue = crc.Movement.Curve;
                        CreateArgs.FindPropertyRelative("Movement.CurveName").stringValue = crc.Movement.CurveName;

                        CreateArgs.FindPropertyRelative("Rotation.Rotation").enumValueIndex = (int)crc.Rotation.Rotation;
                        CreateArgs.FindPropertyRelative("Rotation.Degree").floatValue = crc.Rotation.Degree;
                        CreateArgs.FindPropertyRelative("Rotation.Duration").floatValue = crc.Rotation.Duration;
                        CreateArgs.FindPropertyRelative("Rotation.Delay").floatValue = crc.Rotation.Delay;
                        CreateArgs.FindPropertyRelative("Rotation.Ease").enumValueIndex = (int)crc.Rotation.Ease;
                        CreateArgs.FindPropertyRelative("Rotation.Curve").animationCurveValue = crc.Rotation.Curve;
                        CreateArgs.FindPropertyRelative("Rotation.CurveName").stringValue = crc.Rotation.CurveName;

                        CreateArgs.FindPropertyRelative("Alpha.Duration").floatValue = crc.Alpha.Duration;
                        CreateArgs.FindPropertyRelative("Alpha.Curve").animationCurveValue = crc.Alpha.Curve;
                        CreateArgs.FindPropertyRelative("Alpha.CurveName").stringValue = crc.Alpha.CurveName;
                        CreateArgs.FindPropertyRelative("Alpha.Ease").enumValueIndex = (int)crc.Alpha.Ease;
                        CreateArgs.FindPropertyRelative("Alpha.Delay").floatValue = crc.Alpha.Delay;

                        CreateArgs.serializedObject.ApplyModifiedProperties();

                        Editor_XHud_GUI.Open(XHud_DialogType.确认, "XHud - 元素预览器消息", "粘贴预览动效", "已更新 \"生成\" 动效参数!", "明白", 0);
                    }
                    else//粘贴回收参数
                    {
                        string res = Editor_XHud_GUI.Open(XHud_DialogType.修改, "XHud - 元素预览器消息", "粘贴预览动效", "检测到动效参数类型为： \"回收动效\"，确定要使用这个参数吗？", "确定", "暂不", 0);
                        if (res == "暂不")
                            return;

                        Motion_Recycler rec = JsonUtility.FromJson<Motion_Recycler>(buffer);

                        RecycleArgs.FindPropertyRelative("Movement.Movement").enumValueIndex = (int)rec.Movement.Movement;
                        RecycleArgs.FindPropertyRelative("Movement.Distance").floatValue = rec.Movement.Distance;
                        RecycleArgs.FindPropertyRelative("Movement.Duration").floatValue = rec.Movement.Duration;
                        RecycleArgs.FindPropertyRelative("Movement.Delay").floatValue = rec.Movement.Delay;
                        RecycleArgs.FindPropertyRelative("Movement.Ease").enumValueIndex = (int)rec.Movement.Ease;
                        RecycleArgs.FindPropertyRelative("Movement.Curve").animationCurveValue = rec.Movement.Curve;
                        RecycleArgs.FindPropertyRelative("Movement.CurveName").stringValue = rec.Movement.CurveName;

                        RecycleArgs.FindPropertyRelative("Rotation.Rotation").enumValueIndex = (int)rec.Rotation.Rotation;
                        RecycleArgs.FindPropertyRelative("Rotation.Degree").floatValue = rec.Rotation.Degree;
                        RecycleArgs.FindPropertyRelative("Rotation.Duration").floatValue = rec.Rotation.Duration;
                        RecycleArgs.FindPropertyRelative("Rotation.Delay").floatValue = rec.Rotation.Delay;
                        RecycleArgs.FindPropertyRelative("Rotation.Ease").enumValueIndex = (int)rec.Rotation.Ease;
                        RecycleArgs.FindPropertyRelative("Rotation.Curve").animationCurveValue = rec.Rotation.Curve;
                        RecycleArgs.FindPropertyRelative("Rotation.CurveName").stringValue = rec.Rotation.CurveName;

                        RecycleArgs.FindPropertyRelative("Alpha.Duration").floatValue = rec.Alpha.Duration;
                        RecycleArgs.FindPropertyRelative("Alpha.Curve").animationCurveValue = rec.Alpha.Curve;
                        RecycleArgs.FindPropertyRelative("Alpha.CurveName").stringValue = rec.Alpha.CurveName;
                        RecycleArgs.FindPropertyRelative("Alpha.Ease").enumValueIndex = (int)rec.Alpha.Ease;
                        RecycleArgs.FindPropertyRelative("Alpha.Delay").floatValue = rec.Alpha.Delay;

                        RecycleArgs.serializedObject.ApplyModifiedProperties();

                        Editor_XHud_GUI.Open(XHud_DialogType.确认, "XHud - 元素预览器消息", "粘贴预览动效", "已更新 \"回收\" 动效参数!", "明白", 0);
                    }
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

                menu.AddSeparator("");
                menu.AddDisabledItem(new GUIContent("生成预览"));
                if (!PrimitivePreivew_State.boolValue && !TweensPreivew_Out_State.boolValue)
                {
                    if (!TweensPreivew_In_State.boolValue)
                    {
                        menu.AddItem(new GUIContent("S (生成预览 - 开始)"), false, () =>
                        {
                            if (Application.isPlaying)
                            {
                                Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 元素消息", "预览动画", "程序正在运行，无法在运行期间执行此功能！", "明白");
                                return;
                            }
                            ElementTweens_Preview_In_Play();
                        });
                    }
                    else
                    {
                        menu.AddItem(new GUIContent("S (生成预览 - 停止)"), false, () =>
                        {
                            if (Application.isPlaying)
                            {
                                Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 元素消息", "停止预览动画", "程序正在运行，无法在运行期间执行此功能！", "明白");
                                return;
                            }
                            ElementTweens_Preview_In_Stop();
                        });
                    }
                }

                if (!PrimitivePreivew_State.boolValue && !TweensPreivew_In_State.boolValue)
                {
                    menu.AddSeparator("");
                    menu.AddDisabledItem(new GUIContent("回收预览"));
                    if (!TweensPreivew_Out_State.boolValue)
                    {
                        menu.AddItem(new GUIContent("D (回收预览 - 开始)"), false, () =>
                        {
                            if (Application.isPlaying)
                            {
                                Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 元素消息", "预览动画", "程序正在运行，无法在运行期间执行此功能！", "明白");
                                return;
                            }
                            ElementTweens_Preview_Out_Play();
                        });
                    }
                    else
                    {
                        menu.AddItem(new GUIContent("D (回收预览 - 停止)"), false, () =>
                        {
                            if (Application.isPlaying)
                            {
                                Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 元素消息", "停止预览动画", "程序正在运行，无法在运行期间执行此功能！", "明白");
                                return;
                            }
                            ElementTweens_Preview_Out_Stop();
                        });
                    }
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
            #endregion

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

            CalculateAnimatorMaxDuration(BaseScript.PrimitiveControllerNodes, PrimitivesTweenGlobalDuration.floatValue);

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

            #region PrimitiveControllers
            for (int i = 0; i < PrimitiveControllerNodes.arraySize; i++)
            {
                SerializedProperty sp_node = PrimitiveControllerNodes.GetArrayElementAtIndex(i);
                if (sp_node != null)
                {
                    SerializedProperty sp_node_con = sp_node.FindPropertyRelative("Controller");
                    XHud_Module_Primitive_Controller sp_con = (XHud_Module_Primitive_Controller)sp_node_con.objectReferenceValue;
                    if (sp_con == null)
                    {
                        PrimitiveControllerNodes.DeleteArrayElementAtIndex(i);
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
                    for (int i = 0; i < ele.PrimitiveControllerNodes.Count; i++)
                    {
                        XHud_GUI_Dialog_ListDatas dataitem = new XHud_GUI_Dialog_ListDatas();
                        dataitem.Title = $"{SelectedObjects[s].name} ( {SelectedObjects[s].Indicator} )";
                        dataitem.SubTitle = "已收集 / 动画器";
                        dataitem.Message = $"{ele.PrimitiveControllerNodes[i].Controller.name} ( {ele.PrimitiveControllerNodes[i].Controller.Indicator} )";
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
                for (int i = 0; i < BaseScript.PrimitiveControllerNodes.Count; i++)
                {
                    XHud_GUI_Dialog_ListDatas dataitem = new XHud_GUI_Dialog_ListDatas();
                    dataitem.Title = $"{BaseScript.name} ({BaseScript.Indicator} )";
                    dataitem.SubTitle = "已收集 / 动画器";
                    dataitem.Message = $"{BaseScript.PrimitiveControllerNodes[i].Controller.name} ({BaseScript.PrimitiveControllerNodes[i].Controller.Indicator} )";
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

                if (BaseScript.PrimitiveControllerNodes.Count <= 0 && BaseScript.ButtonNodes.Count <= 0 && BaseScript.OptionNodes.Count <= 0 && BaseScript.SliderNodes.Count <= 0 && BaseScript.ProgressNodes.Count <= 0 && BaseScript.ToggleNodes.Count <= 0 && BaseScript.SounderNodes.Count <= 0 && BaseScript.TextNodes.Count <= 0 && BaseScript.TmpTextNodes.Count <= 0)
                {
                    string hexcol = XHud_Utilitys.Color_To_HexColor(XHud_Dashboard.Theme_Primary, true);
                    Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 元素消息", "扫描元素子组件", $"未扫描到任何子元素，请您检查子物体中是否有物体包含以下组件： <color={hexcol}>Hud_Animator、Hud_Button、Hud_Progress、Hud_Slider、Hud_Option、HudSound、Hud_Text、Hud_TmpText、HudToggle</color>", "明白");
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
                    sp_tog_isfold.serializedObject.ApplyModifiedProperties();

                    so_ele.ApplyModifiedProperties();
                }
            }
            else
            {
                if (target != null)
                {
                    PrimitivesIsFold.boolValue = state;
                    ButtonIsFold.boolValue = state;
                    OptionIsFold.boolValue = state;
                    SliderIsFold.boolValue = state;
                    EventIsFold.boolValue = state;
                    SounderIsFold.boolValue = state;
                    ToggleIsFold.boolValue = state;
                    TextIsFold.boolValue = state;
                    TmpTextIsFold.boolValue = state;
                    ProgressIsFold.boolValue = state;

                    PrimitivesIsFold.serializedObject.ApplyModifiedProperties();
                    ButtonIsFold.serializedObject.ApplyModifiedProperties();
                    OptionIsFold.serializedObject.ApplyModifiedProperties();
                    SliderIsFold.serializedObject.ApplyModifiedProperties();
                    EventIsFold.serializedObject.ApplyModifiedProperties();
                    SounderIsFold.serializedObject.ApplyModifiedProperties();
                    ToggleIsFold.serializedObject.ApplyModifiedProperties();
                    TextIsFold.serializedObject.ApplyModifiedProperties();
                    TmpTextIsFold.serializedObject.ApplyModifiedProperties();
                    ProgressIsFold.serializedObject.ApplyModifiedProperties();

                    serializedObject.ApplyModifiedProperties();
                }
            }
        }
        private void CalculateAnimatorMaxDuration(List<PrimitiveControllerNode> list, float globaldur)
        {
            PrimitivesTweenMaxDuration.floatValue = Animators_GetAnimatorsMaxDuration(list, globaldur);
            PrimitivesTweenMaxDuration.serializedObject.ApplyModifiedProperties();
        }
        /// <summary>
        /// 从所有子动画器中获取最大耗时
        /// </summary>
        /// <returns></returns>
        public float Animators_GetAnimatorsMaxDuration(List<PrimitiveControllerNode> list, float globaldur)
        {
            if (list.Count <= 0)
                return 0;
            float[] x_list = new float[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Controller == null)
                {
                    x_list[i] = 0;
                    continue;
                }
                list[i].Controller.pt_Tween.TweenNode_GetTimers();
                x_list[i] = list[i].Controller.pt_Tween.MaxTimerWithGlobalDuration + list[i].DelayTime;
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
            for (int i = 0; i < PrimitiveControllerNodes.arraySize; i++)
            {
                SerializedProperty sp_node_con = PrimitiveControllerNodes.GetArrayElementAtIndex(i).FindPropertyRelative("Controller");
                XHud_Module_Primitive_Controller sp_con = (XHud_Module_Primitive_Controller)sp_node_con.objectReferenceValue;
                if (sp_con == null)
                {
                    continue;
                }
                if (sp_con == null && sp_con.pt_Tween.PrimitiveTweenNodes == null || sp_con.pt_Tween.PrimitiveTweenNodes.Count <= 0)
                {
                    continue;
                }
                else
                {
                    for (int s = 0; s < sp_con.pt_Tween.PrimitiveTweenNodes.Count; s++)
                    {
                        loops.Add(sp_con.pt_Tween.PrimitiveTweenNodes[s].LoopCount);
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
        /// 标题面板
        /// </summary>
        /// <param name="title"></param>
        /// <param name="margin"></param>
        /// <param name="Fill"></param>
        /// <param name="color"></param>
        /// <param name="titlecolor"></param>
        /// <param name="titlecolor_hover"></param>
        /// <param name="titlecolor_active"></param>
        /// <param name="margin_btn"></param>
        /// <param name="offset"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool FunctionGroup(string title, float margin, HudFilled Fill, HudColor color, Color titlecolor, Color titlecolor_hover, Color titlecolor_active, RectOffset margin_btn, Vector2 offset, string key, Texture2D icon)
        {
            int inspectorwidth = Screen.width;
            //sp_DebugMode.Log(inspectorwidth);

            bool sw_option = XHud_Utilitys.PlayerPrefs_ReadValue_Bool_ForEditor(key);
            sw_option = Editor_XHud_GUI.Gui_Layout_Vertical_Start_WithFolder(Fill, color, margin, title, titlecolor, titlecolor_hover, titlecolor_active, margin_btn, offset, icon, sw_option, inspectorwidth);
            XHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(key, sw_option);
            return sw_option;
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
            ButtonNodes = serializedObject.FindProperty("ButtonNodes");
            OptionNodes = serializedObject.FindProperty("OptionNodes");
            SliderNodes = serializedObject.FindProperty("SliderNodes");
            ProgressNodes = serializedObject.FindProperty("ProgressNodes");
            TextNodes = serializedObject.FindProperty("TextNodes");
            TmpTextNodes = serializedObject.FindProperty("TmpTextNodes");
            ToggleNodes = serializedObject.FindProperty("ToggleNodes");
            DebugState = serializedObject.FindProperty("DebugState");
            CanvasGroup = serializedObject.FindProperty("CanvasGroup");
            Alpha = serializedObject.FindProperty("Alpha");
            RectTransform = serializedObject.FindProperty("RectTransform");
            TriggerAction = serializedObject.FindProperty("TriggerAction");
            ObjectTracker = serializedObject.FindProperty("ObjectTracker");
            AutoKillPreviewTweens = serializedObject.FindProperty("AutoKillPreviewTweens");
            eve_on_element_in_start = serializedObject.FindProperty("eve_on_element_in_start");
            eve_on_element_in_end = serializedObject.FindProperty("eve_on_element_in_end");
            eve_on_element_out_start = serializedObject.FindProperty("eve_on_element_out_start");
            eve_on_element_out_end = serializedObject.FindProperty("eve_on_element_out_end");
            TextIsFold = serializedObject.FindProperty("TextIsFold");
            TmpTextIsFold = serializedObject.FindProperty("TmpTextIsFold");
            Indicator = serializedObject.FindProperty("Indicator");
            CreateState = serializedObject.FindProperty("CreateState");
            AnimateState = serializedObject.FindProperty("AnimateState");
            OriginPoolName = serializedObject.FindProperty("OriginPoolName");
            OriginalName = serializedObject.FindProperty("OriginalName");
            CurrentPivot = serializedObject.FindProperty("CurrentPivot");
            ButtonIsFold = serializedObject.FindProperty("ButtonIsFold");
            OptionIsFold = serializedObject.FindProperty("OptionIsFold");
            SliderIsFold = serializedObject.FindProperty("SliderIsFold");
            ProgressIsFold = serializedObject.FindProperty("ProgressIsFold");
            ToggleIsFold = serializedObject.FindProperty("ToggleIsFold");
            EventIsFold = serializedObject.FindProperty("EventIsFold");
            SounderIsFold = serializedObject.FindProperty("SounderIsFold");
            RMS_Enabled = serializedObject.FindProperty("RMS_Enabled");
            RMS_LayoutDatas = serializedObject.FindProperty("RMS_LayoutDatas");
            RMS_Name = serializedObject.FindProperty("RMS_Name");
            TweensPreivew_In_State = serializedObject.FindProperty("TweensPreivew_In_State");
            TweensPreivew_Out_State = serializedObject.FindProperty("TweensPreivew_Out_State");
            RewindPreviewTweensWithKill = serializedObject.FindProperty("RewindPreviewTweensWithKill");
            ClearPreviewTweensWithKill = serializedObject.FindProperty("ClearPreviewTweensWithKill");
            CreateArgs = serializedObject.FindProperty("CreateArgs");
            RecycleArgs = serializedObject.FindProperty("RecycleArgs");
            Crc_Lib_Name = serializedObject.FindProperty("Crc_Lib_Name");
            Rec_Lib_Name = serializedObject.FindProperty("Rec_Lib_Name");
            CreateArgs_MotionAnimateEndState = CreateArgs.FindPropertyRelative("MotionAnimateEndState");
            RecycleArgs_MotionAnimateEndState = RecycleArgs.FindPropertyRelative("MotionAnimateEndState");
            create_fold_move = serializedObject.FindProperty("create_fold_move");
            create_fold_rotate = serializedObject.FindProperty("create_fold_rotate");
            create_fold_alpha = serializedObject.FindProperty("create_fold_alpha");
            recycle_fold_move = serializedObject.FindProperty("recycle_fold_move");
            recycle_fold_rotate = serializedObject.FindProperty("recycle_fold_rotate");
            recycle_fold_alpha = serializedObject.FindProperty("recycle_fold_alpha");

            PreviewPrimitivesTween = serializedObject.FindProperty("PreviewPrimitivesTween");
            AutoPlayPrimitivesTween = serializedObject.FindProperty("AutoPlayPrimitivesTween");
            PrimitivesTweenMaxDuration = serializedObject.FindProperty("PrimitivesTweenMaxDuration");
            PrimitivesTweenGlobalDuration = serializedObject.FindProperty("PrimitivesTweenGlobalDuration");
            PrimitivesIsFold = serializedObject.FindProperty("PrimitivesIsFold");
            PrimitivePreivew_State = serializedObject.FindProperty("PrimitivePreivew_State");
            PrimitiveControllerNodes = serializedObject.FindProperty("PrimitiveControllerNodes");
        }
        #endregion      
    }
}