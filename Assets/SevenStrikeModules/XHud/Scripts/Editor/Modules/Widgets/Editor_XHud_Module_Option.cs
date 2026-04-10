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
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.UI;

    public class XHud_ModuleArg_Option
    {
        public string Indicator;
        public float OptionSelector_Tween_GlobalDuration;
        public bool RepeatTweenPlay;
        public bool UseBlinked;
        public bool UseEaseMotion;
        public float LerpSpeed;
        public float TweenSpeed;
        public EaseMode SelectorTweenMotion;
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(XHud_Module_Option), true)]
    public class Editor_XHud_Module_Option : Editor
    {
        private Vector2 lastSize;

        #region 组件 / 列表
        private XHud_Module_Option BaseScript;
        private ReorderableList OptionButtonList;
        private ReorderableList SelectorTweenList;
        private XHud_Manager HudManager;
        #endregion

        #region 序列化属性
        private SerializedProperty DebugState, SelectorTweenMotion, SelectorOffset, SelectorOffsetAdded, Indicator, LerpSpeed, Pos_Destination, ChangingInterval, sm_Pos_Destination, TweenSpeed, OptionSelector, RepeatTweenPlay, PrimitivesTweenGlobalDuration, PrimitiveControllerNodes, UseEaseMotion, OptionButtonNodes, PrimitivesTweenMaxDuration, CurrentOptionName, OptionIndex, SelectorMark, OptionRoot, UseBlinked, AnimateState, PrimitivesIsFold, ButtonIsFold, EventIsFold, eve_on_selector_position_changed, eve_on_option_clicked, eve_on_option_clicked_with_indicator, eve_on_option_clicked_with_index, eve_on_option_clicked_with_position, eve_on_selector_position_started, eve_on_selector_position_complete, AutoStopPreview;
        #endregion

        #region 图标                                                                                                                                     
        private Texture2D icon_main, find_r, find_p, play_r, play_p, stop_r, stop_p, clear_r, clear_p, icon_button, animstate, dutation, longpressmarker, left_arrow_r, left_arrow_p, right_arrow_r, right_arrow_p, icon_anim, icon_option;
        #endregion

        #region GUI 参数
        /// <summary>
        /// 原始脚本参数显示开关
        /// </summary>
        private bool OriginalDisplay;
        /// <summary>
        /// 行高
        /// </summary>
        private float LineHeight;
        #endregion

        #region Preview - Tween
        private bool Preivew_Tween_PlayingState;
        #endregion

        #region 批量模式查看索引
        private int OptionStatu_Index;
        private int OptionStatistic_Index;
        #endregion

        #region 选项文字
        string[] stroptions_enabled = new string[2] { "关闭", "开启" }, stroptions_debug = new string[2] { "关闭", "调试" }, stroptions_blinked = new string[2] { "平滑", "闪现" }, stroptions_easemode = new string[2] { "差值", "缓动" };
        #endregion

        #region 批量化操作
        XHud_Module_Option[] SelectedObjects;

        private void Targets_Get()
        {
            if (targets.Length > 1)
            {
                SelectedObjects = new XHud_Module_Option[targets.Length];
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    var t = targets[i];
                    SelectedObjects[i] = (XHud_Module_Option)t;
                }
            }
            else
            {
                SelectedObjects = new XHud_Module_Option[targets.Length];
                SelectedObjects[0] = (XHud_Module_Option)target;
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

        void OnEnable()
        {
            HudManager = XHud_Dashboard.HudManagerGet();

            BaseScript = (XHud_Module_Option)target;

            // 获取序列化属性
            GetSerializeFields();

            #region 获取图标
            icon_main = Editor_XHud_GUI.GetIcon("Icons_XHud_Option/icon_main");
            find_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Option/find_r");
            find_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Option/find_p");
            play_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Option/play_r");
            play_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Option/play_p");
            stop_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Option/stop_r");
            stop_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Option/stop_p");
            clear_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Option/clear_r");
            clear_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Option/clear_p");
            icon_button = Editor_XHud_GUI.GetIcon("Icons_XHud_Option/icon_button");
            animstate = Editor_XHud_GUI.GetIcon("Icons_XHud_Option/animstate");
            dutation = Editor_XHud_GUI.GetIcon("Icons_XHud_Option/dutation");
            longpressmarker = Editor_XHud_GUI.GetIcon("Icons_XHud_Option/longpressmarker");
            left_arrow_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Option/left_arrow_r");
            left_arrow_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Option/left_arrow_p");
            right_arrow_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Option/right_arrow_r");
            right_arrow_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Option/right_arrow_p");
            icon_anim = Editor_XHud_GUI.GetIcon("Icons_XHud_Option/icon_anim");
            icon_option = Editor_XHud_GUI.GetIcon("Icons_XHud_Option/icon_option");
            #endregion

            if (SelectorMark.objectReferenceValue != null)
            {
                SelectorOffsetAdded.vector3Value = ((RectTransform)SelectorMark.objectReferenceValue).anchoredPosition3D;
                SelectorOffsetAdded.serializedObject.ApplyModifiedProperties();
            }

            LineHeight = EditorGUIUtility.singleLineHeight;

            Vector2 ButtonSize = new Vector2(18, 18);

            #region ReorderableList - PrimitivesTween
            SelectorTweenList = new ReorderableList(serializedObject, PrimitiveControllerNodes)
            {
                displayAdd = true,
                displayRemove = true,
                draggable = true,

                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "图元动画器列表");
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    float titleheight = rect.y + 6;
                    float baseheight = rect.y + (rect.height - 25);

                    SerializedProperty sp_node = PrimitiveControllerNodes.GetArrayElementAtIndex(index);
                    SerializedProperty sp_node_con = sp_node.FindPropertyRelative("Controller");
                    SerializedProperty sp_delay = sp_node.FindPropertyRelative("DelayTime");
                    XHud_Module_Primitive_Controller sp_con = (XHud_Module_Primitive_Controller)sp_node_con.objectReferenceValue;

                    if (sp_node_con.objectReferenceValue != null)
                    {
                        #region 标题
                        string title = "";
                        string indicator = sp_con.GetIndicator();
                        if (!string.IsNullOrEmpty(indicator))
                            title += indicator;
                        else
                            title += sp_con.gameObject.name;
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
                        SerializedObject so_anim = new SerializedObject(sp_con.pt_Tween);
                        so_anim.Update();

                        SerializedProperty sp_glodur = so_anim.FindProperty("GlobalDuration");
                        SerializedProperty sp_maxdur = so_anim.FindProperty("MaxTimerWithGlobalDuration");

                        Editor_XHud_GUI.Gui_Property_Field(new Rect(rect.width - 50, rect.y + 4, 30, 19), "G", sp_glodur, 10, 40, LineHeight, 15);

                        Editor_XHud_GUI.Gui_Labelfield_Thin(new Rect(rect.width - 80, rect.y + 4, 30, 19), $"{sp_maxdur.floatValue.ToString()} s", HudFilled.无, HudColor.无, XHud_Dashboard.Theme_Primary, TextAnchor.MiddleCenter, Vector2.zero, 11);
                        #endregion

                        sp_node_con.serializedObject.ApplyModifiedProperties();
                        sp_node.serializedObject.ApplyModifiedProperties();
                    }
                    else
                    {
                        PrimitiveControllerNodes.DeleteArrayElementAtIndex(index);
                        PrimitiveControllerNodes.serializedObject.ApplyModifiedProperties();
                    }
                },
                onSelectCallback = (ReorderableList list) =>
                {
                    if (!Application.isPlaying)
                    {

                    }

                    SerializedProperty sp_node = PrimitiveControllerNodes.GetArrayElementAtIndex(list.index);
                    SerializedProperty sp_node_con = sp_node.FindPropertyRelative("Controller");

                    if (sp_node_con != null)
                    {
                        EditorGUIUtility.PingObject(sp_node_con.objectReferenceValue);

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
                        XHud_Module_Button btn = (XHud_Module_Button)sp_button.objectReferenceValue;
                        SerializedProperty sp_isopt = OptionButtonNodes.GetArrayElementAtIndex(index).FindPropertyRelative("IsOptional");

                        string title = "";
                        if (!string.IsNullOrEmpty(btn.Indicator))
                            title += btn.Indicator;
                        else
                            title += btn.gameObject.name;
                        Editor_XHud_GUI.Gui_Labelfield(new Rect(rect.width - (rect.width - 90), titleheight - 2, (rect.width * 0.45f) - 20, LineHeight), title, HudFilled.无, HudColor.亮白, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 11, TextClipping.Clip);

                        GUI.color = XHud_Dashboard.Theme_Primary;
                        Editor_XHud_GUI.Gui_Icon(new Rect(rect.width - (rect.width - 60), titleheight, 10, 10), icon_button);

                        #region 类型图标
                        if (sp_isopt.boolValue)
                        {
                            if (OptionIndex.intValue == index)
                            {
                                GUI.color = XHud_Dashboard.Theme_Primary;
                            }
                            else
                            {
                                GUI.color = Editor_XHud_GUI.GetColor(HudColor.枪灰);
                            }
                            Editor_XHud_GUI.Gui_Icon(new Rect(rect.width + 30, titleheight + 1.8f, 10, 10), icon_option);
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
                        Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 选项器消息", "缺失光标组件", "您并未为选项器设置一个有效光标组件！请检查参数面板中的 “光标指示器”是否为空！", "明白");
                        return;
                    }
                    else
                    {
                        SerializedProperty sp_button = OptionButtonNodes.GetArrayElementAtIndex(list.index).FindPropertyRelative("Button");
                        XHud_Module_Button sp_btn = (XHud_Module_Button)sp_button.objectReferenceValue;
                        SerializedProperty sp_indicator = OptionButtonNodes.GetArrayElementAtIndex(list.index).FindPropertyRelative("Indicator");

                        OptionIndex.intValue = list.index;
                        OptionIndex.serializedObject.ApplyModifiedProperties();
                        CurrentOptionName.stringValue = sp_btn.Indicator;

                        RectTransform sel_mark = (RectTransform)SelectorMark.objectReferenceValue;

                        for (int i = 0; i < OptionButtonNodes.arraySize; i++)
                        {
                            SerializedProperty sp_x_button = OptionButtonNodes.GetArrayElementAtIndex(i).FindPropertyRelative("Button");
                            XHud_Module_Button sp_x_btn = (XHud_Module_Button)sp_x_button.objectReferenceValue;
                            if (sp_x_btn.Indicator == CurrentOptionName.stringValue)
                            {

                                RectTransform sel_opts = (RectTransform)OptionRoot.objectReferenceValue;
                                XHud_Module_Button btn = (XHud_Module_Button)OptionButtonNodes.GetArrayElementAtIndex(i).FindPropertyRelative("Button").objectReferenceValue;
                                Vector3 target = sel_opts.parent.InverseTransformPoint(btn.RectTransform.position);
                                SelectorOffsetAdded.vector3Value = target;
                                Pos_Destination.vector3Value = target;
                                sm_Pos_Destination.vector3Value = target;

                                SelectorOffsetAdded.serializedObject.ApplyModifiedProperties();
                                Pos_Destination.serializedObject.ApplyModifiedProperties();
                                sm_Pos_Destination.serializedObject.ApplyModifiedProperties();

                                sel_mark.anchoredPosition3D = new Vector3(Pos_Destination.vector3Value.x, Pos_Destination.vector3Value.y, sel_mark.anchoredPosition3D.z) + SelectorOffset.vector3Value;

                                SelectorMark.serializedObject.ApplyModifiedProperties();

                                // 记录光标在因选项选择后改变的图元特性姿态
                                SaveSelectorMarkFeature(sel_mark);
                            }
                        }

                        EditorGUIUtility.PingObject(sp_button.objectReferenceValue);
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

        private void OnDisable()
        {
            if (!Application.isPlaying)
            {

            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (string.IsNullOrEmpty(BaseScript.Indicator))
                Editor_XHud_GUI.Gui_Layout_Banner(icon_main, HudFilled.实体, HudColor.深空灰, "XHud - 选项器", Color.white);
            else
                Editor_XHud_GUI.Gui_Layout_Banner(icon_main, HudFilled.实体, HudColor.深空灰, "XHud - 选项器 -> ( " + BaseScript.Indicator + " )", Color.white);

            #region 快捷功能
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "快捷功能", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            #region 扫描
            if (Editor_XHud_GUI.Gui_Layout_Button(15, "获取选项按钮与动画组件", find_r, find_p))
            {
                string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 选项器消息", "获取子组件", "请确保子级中按钮的选项模式已开启！否则无法被扫描到并作为选项按钮！", "立即扫描", "再检查下", 1);
                if (res == "再检查下")
                {
                    return;
                }

                GetAllOptionsButton();
                GetPrimitivesTween();
                GetPrimitivesTweenResults();
                return;
            }
            #endregion

            GUILayout.FlexibleSpace();

            #region 预览
            if (!Targets_Selected() && PrimitiveControllerNodes.arraySize > 0)
            {
                GUILayout.FlexibleSpace();

                #region 预览动画
                if (!Preivew_Tween_PlayingState)
                {
                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "播放所有图元动画器预览", play_r, play_p))
                    {
                        if (!Application.isPlaying)
                        {

                            return;
                        }
                    }
                }
                else
                {
                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "停止所有图元动画器预览", stop_r, stop_p))
                    {
                        if (!Application.isPlaying)
                        {

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

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_Option>("状态调试", stroptions_debug, ref DebugState, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_Option>("选择时重复动画", stroptions_enabled, ref RepeatTweenPlay, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_Option>("选择器闪现", stroptions_blinked, ref UseBlinked, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

            if (!UseBlinked.boolValue)
            {
                Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_Option>("选择器缓动", stroptions_easemode, ref UseEaseMotion, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
            }

            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 参数
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "参数", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            #region 选项按钮说明

            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_XHud_GUI.Gui_Layout_Space(10);
            EditorGUILayout.HelpBox("注意：您可以根据需要增加选项，但同时要保证所有按钮的 \"标识\" 必须被设置不为空，并确保没有重名的按钮标识", MessageType.Info);
            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Horizontal_End();

            #endregion

            Editor_XHud_GUI.Gui_Layout_Property_Field("标识名称", Indicator, 100);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("光标指示器", OptionSelector, 100);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("组根物体", OptionRoot, 100);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("速率倍增", PrimitivesTweenGlobalDuration, 100);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            EditorGUI.BeginChangeCheck();
            Editor_XHud_GUI.Gui_Layout_Property_Field("光标偏移", SelectorOffset, 100);
            SelectorOffset.serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
            {
                if (!Targets_Selected())
                {
                    RectTransform sel_mark = (RectTransform)SelectorMark.objectReferenceValue;
                    sel_mark.anchoredPosition3D = SelectorOffsetAdded.vector3Value + SelectorOffset.vector3Value;

                    // 记录光标在因选项选择后改变的图元特性姿态
                    SaveSelectorMarkFeature(sel_mark);
                }
                else
                {
                    for (int i = 0; i < SelectedObjects.Length; i++)
                    {
                        RectTransform sel_mark = SelectedObjects[i].SelectorMark;
                        sel_mark.anchoredPosition3D = SelectedObjects[i].SelectorOffsetAdded + SelectedObjects[i].SelectorOffset;

                        // 记录光标在因选项选择后改变的图元特性姿态
                        SaveSelectorMarkFeature(sel_mark);
                    }
                }
            }

            if (!UseBlinked.boolValue)
            {
                if (!UseEaseMotion.boolValue)
                {
                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    Editor_XHud_GUI.Gui_Layout_Property_Field("差值平滑速率", LerpSpeed, 100);
                }
                else
                {
                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    Editor_XHud_GUI.Gui_Layout_Property_Field("缓动方式", SelectorTweenMotion, 100);

                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    Editor_XHud_GUI.Gui_Layout_Property_Field("缓动耗时", TweenSpeed, 100);
                }
            }

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
                #region 选项状态
                Editor_XHud_GUI.StatuDisplayer_text(icon_button, 12, new Vector2(0, 7), "选项状态", 12, CurrentOptionName.stringValue, XHud_Dashboard.Theme_Primary, 11);
                #endregion

                #region 动画相关
                if (PrimitiveControllerNodes != null && PrimitiveControllerNodes.arraySize > 0)
                {
                    #region 动画状态     
                    Editor_XHud_GUI.StatuDisplayer_text(animstate, 12, new Vector2(0, 7), "动画状态", 12, (XHudElementAnimateState)AnimateState.enumValueIndex == XHudElementAnimateState.Animating ? "动画中" : "静止状态", AnimateState.enumValueIndex == 1 ? XHud_Dashboard.Theme_Primary : Color.gray, 11);
                    #endregion

                    #region 最大耗时     
                    Editor_XHud_GUI.StatuDisplayer_text(dutation, 12, new Vector2(0, 7), "最大耗时<color=#909090>（速率倍增）</color>", 12, PrimitivesTweenMaxDuration.floatValue.ToString() + "秒", XHud_Dashboard.Theme_Primary, 11);
                    #endregion

                    #region 最大耗时        
                    Editor_XHud_GUI.StatuDisplayer_text(dutation, 12, new Vector2(0, 7), "最大耗时<color=#909090>（XHUD倍增）</color>", 12, (HudManager.DurationMultiply * PrimitivesTweenMaxDuration.floatValue).ToString() + "秒", XHud_Dashboard.Theme_Primary, 11);
                    #endregion
                }
                #endregion

                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Seperator(1, XHud_Dashboard.Theme_SeperateLine);
                Editor_XHud_GUI.Gui_Layout_Space(10);

                #region 当前选择的选项
                if (!Application.isPlaying)
                {
                    if (OptionButtonNodes != null && OptionButtonNodes.arraySize > 0)
                    {
                        string[] btn_names = BaseScript.opt_GetOptionButtonNames();
                        EditorGUI.BeginChangeCheck();
                        Editor_XHud_GUI.Gui_Layout_Popup<int, XHud_Module_Option>("当前选项", btn_names, ref OptionIndex, HudFilled.实体, 120, 22, SelectedObjects);
                        if (EditorGUI.EndChangeCheck())
                        {
                            if (SelectorMark.objectReferenceValue == null)
                            {
                                Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 选项器消息", "缺失光标组件", "您并未为选项器设置一个有效光标组件！请检查参数面板中的 “光标指示器”是否为空！", "明白");
                                return;
                            }
                            SelectorCorrection();
                        }
                    }
                }
                #endregion

                Editor_XHud_GUI.Gui_Layout_Space(10);
            }
            else
            {
                #region 批量控件
                Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_XHud_GUI.Gui_Layout_Space(10);
                if (Editor_XHud_GUI.Gui_Layout_Button($"{SelectedObjects[OptionStatu_Index].name} ( {SelectedObjects[OptionStatu_Index].Indicator} )", "", HudFilled.透明, HudColor.无, Color.gray, 20))
                {
                    EditorGUIUtility.PingObject(SelectedObjects[OptionStatu_Index]);
                }
                Editor_XHud_GUI.Gui_Layout_FlexSpace();
                if (Editor_XHud_GUI.Gui_Layout_Button(12, "", left_arrow_r, left_arrow_p))
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
                Editor_XHud_GUI.Gui_Layout_Space(16);
                if (Editor_XHud_GUI.Gui_Layout_Button(12, "", right_arrow_r, right_arrow_p))
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
                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                #endregion

                #region 按钮状态
                Editor_XHud_GUI.StatuDisplayer_text(icon_button, 12, new Vector2(0, 7), "选项状态", 12, SelectedObjects[OptionStatu_Index].CurrentOptionName, XHud_Dashboard.Theme_Primary, 11);
                #endregion

                #region 动画相关
                if (SelectedObjects[OptionStatu_Index].PrimitiveControllerNodes != null && SelectedObjects[OptionStatu_Index].PrimitiveControllerNodes.Count > 0)
                {
                    #region 动画状态     
                    Editor_XHud_GUI.StatuDisplayer_text(animstate, 12, new Vector2(0, 7), "动画状态", 12, SelectedObjects[OptionStatu_Index].AnimateState == XHudElementAnimateState.Animating ? "动画中" : "静止状态", SelectedObjects[OptionStatu_Index].AnimateState == XHudElementAnimateState.Animating ? XHud_Dashboard.Theme_Primary : Color.gray, 11);
                    #endregion

                    SelectedObjects[OptionStatu_Index].PrimitivesTweenMaxDuration = PrimitiveTweens_MaxDuration_Get(SelectedObjects[OptionStatu_Index].PrimitiveControllerNodes, SelectedObjects[OptionStatu_Index].PrimitivesTweenGlobalDuration);

                    #region 最大耗时     
                    Editor_XHud_GUI.StatuDisplayer_text(dutation, 12, new Vector2(0, 7), "最大耗时<color=#909090>（速率倍增）</color>", 12, SelectedObjects[OptionStatu_Index].PrimitivesTweenMaxDuration.ToString() + "秒", XHud_Dashboard.Theme_Primary, 11);
                    #endregion

                    #region 最大耗时        
                    Editor_XHud_GUI.StatuDisplayer_text(dutation, 12, new Vector2(0, 7), "最大耗时<color=#909090>（XHUD倍增）</color>", 12, (HudManager.DurationMultiply * SelectedObjects[OptionStatu_Index].PrimitivesTweenMaxDuration).ToString() + "秒", XHud_Dashboard.Theme_Primary, 11);
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
                if (PrimitiveControllerNodes.arraySize <= 0)
                {
                    Editor_XHud_GUI.Gui_Layout_Labelfield("暂无统计数据", HudFilled.无, HudColor.无, Editor_XHud_GUI.GetColor(HudColor.阴影灰), TextAnchor.MiddleCenter);
                }
                else
                {
                    #region 组件数量 - 图元动画器
                    if (PrimitiveControllerNodes.arraySize > 0)
                    {
                        Editor_XHud_GUI.StatuDisplayer_text(icon_anim, 12, new Vector2(0, 7), "图元动画器", 12, PrimitiveControllerNodes.arraySize.ToString() + " 个", XHud_Dashboard.Theme_Primary, 11);
                    }
                    #endregion

                    #region 组件数量 - 选项按钮
                    if (OptionButtonNodes.arraySize > 0)
                    {
                        Editor_XHud_GUI.StatuDisplayer_text(icon_button, 12, new Vector2(0, 7), "选项按钮", 12, OptionButtonNodes.arraySize.ToString() + " 个", XHud_Dashboard.Theme_Primary, 11);
                    }
                    #endregion
                }
            }
            else
            {
                #region 批量控件
                Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_XHud_GUI.Gui_Layout_Space(10);
                if (Editor_XHud_GUI.Gui_Layout_Button($"{SelectedObjects[OptionStatistic_Index].name} ( {SelectedObjects[OptionStatistic_Index].Indicator} )", "", HudFilled.透明, HudColor.无, Color.gray, 20))
                {
                    EditorGUIUtility.PingObject(SelectedObjects[OptionStatistic_Index]);
                }
                Editor_XHud_GUI.Gui_Layout_FlexSpace();
                if (Editor_XHud_GUI.Gui_Layout_Button(12, "", left_arrow_r, left_arrow_p))
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
                Editor_XHud_GUI.Gui_Layout_Space(16);
                if (Editor_XHud_GUI.Gui_Layout_Button(12, "", right_arrow_r, right_arrow_p))
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
                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                #endregion

                #region 组件数量 - 图元动画器
                if (SelectedObjects[OptionStatistic_Index].PrimitiveControllerNodes.Count > 0)
                {
                    Editor_XHud_GUI.StatuDisplayer_text(icon_anim, 12, new Vector2(0, 7), "图元动画器", 12, SelectedObjects[OptionStatistic_Index].PrimitiveControllerNodes.Count + " 个", XHud_Dashboard.Theme_Primary, 11);
                }
                #endregion

                #region 组件数量 - 选项按钮
                if (SelectedObjects[OptionStatistic_Index].OptionButtonNodes.Count > 0)
                {
                    Editor_XHud_GUI.StatuDisplayer_text(icon_button, 12, new Vector2(0, 7), "选项按钮", 12, SelectedObjects[OptionStatistic_Index].OptionButtonNodes.Count + " 个", XHud_Dashboard.Theme_Primary, 11);
                }
                #endregion
            }
            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 检查是否有无效的图元动画器
            CheckTweensValid();
            #endregion

            #region 检查是否有无效的选项按钮
            CheckButtonsValid();
            #endregion

            #region 事件/列表
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "事件/列表", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(5);

            #region 图元动画器列表
            if (PrimitiveControllerNodes.arraySize > 0)
            {
                if (Targets_Selected())
                {
                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(10);
                    EditorGUILayout.HelpBox("动画列表不支持多项操作", MessageType.Warning);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                }
                else
                {
                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(10);
                    PrimitivesIsFold.boolValue = EditorGUILayout.Foldout(PrimitivesIsFold.boolValue, "图元动画器", true);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    if (PrimitivesIsFold.boolValue)
                        SelectorTweenList.DoLayoutList();
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                }
            }
            #endregion

            #region 按钮列表
            if (OptionButtonNodes.arraySize > 0)
            {
                if (Targets_Selected())
                {
                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(10);
                    EditorGUILayout.HelpBox("选项按钮列表不支持多项操作", MessageType.Warning);
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
                        OptionButtonList.DoLayoutList();
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
                EditorGUILayout.HelpBox("事件列表不支持多项操作", MessageType.Warning);
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
                    EditorGUILayout.PropertyField(eve_on_option_clicked);
                    eve_on_option_clicked.serializedObject.ApplyModifiedProperties();
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_option_clicked_with_indicator);
                    eve_on_option_clicked_with_indicator.serializedObject.ApplyModifiedProperties();
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_option_clicked_with_index);
                    eve_on_option_clicked_with_index.serializedObject.ApplyModifiedProperties();
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_option_clicked_with_position);
                    eve_on_option_clicked_with_position.serializedObject.ApplyModifiedProperties();
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_selector_position_changed);
                    eve_on_selector_position_changed.serializedObject.ApplyModifiedProperties();
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_selector_position_started);
                    eve_on_selector_position_started.serializedObject.ApplyModifiedProperties();
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_selector_position_complete);
                    eve_on_selector_position_complete.serializedObject.ApplyModifiedProperties();
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                }
            }
            #endregion

            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                SerializedProperty sp_OptionSelector_Tween_GlobalDuration = serializedObject.FindProperty("OptionSelector_Tween_GlobalDuration");
                SerializedProperty sp_RepeatTweenPlay = serializedObject.FindProperty("RepeatTweenPlay");
                SerializedProperty sp_UseBlinked = serializedObject.FindProperty("UseBlinked");
                SerializedProperty sp_UseEaseMotion = serializedObject.FindProperty("UseEaseMotion");
                SerializedProperty sp_LerpSpeed = serializedObject.FindProperty("LerpSpeed");
                SerializedProperty sp_TweenSpeed = serializedObject.FindProperty("TweenSpeed");
                SerializedProperty sp_SelectorTweenMotion = serializedObject.FindProperty("SelectorTweenMotion");

                // 创建右键菜单
                GenericMenu menu = new GenericMenu();
                menu.AddDisabledItem(new GUIContent("脚本参数"));
                if (!Targets_Selected())
                {
                    menu.AddItem(new GUIContent("C (拷贝)"), false, () =>
                    {
                        XHud_ModuleArg_Option hop = new XHud_ModuleArg_Option();

                        hop.OptionSelector_Tween_GlobalDuration = sp_OptionSelector_Tween_GlobalDuration.floatValue;
                        hop.RepeatTweenPlay = sp_RepeatTweenPlay.boolValue;
                        hop.UseBlinked = sp_UseBlinked.boolValue;
                        hop.UseEaseMotion = sp_UseEaseMotion.boolValue;
                        hop.LerpSpeed = sp_LerpSpeed.floatValue;
                        hop.TweenSpeed = sp_TweenSpeed.floatValue;
                        hop.SelectorTweenMotion = (EaseMode)sp_SelectorTweenMotion.enumValueIndex;

                        string json = JsonUtility.ToJson(hop);
                        GUIUtility.systemCopyBuffer = json;
                    });
                }
                menu.AddItem(new GUIContent("V (粘贴)"), false, () =>
                {
                    XHud_ModuleArg_Option hop = JsonUtility.FromJson<XHud_ModuleArg_Option>(GUIUtility.systemCopyBuffer);

                    if (Targets_Selected())
                    {
                        for (int i = 0; i < SelectedObjects.Length; i++)
                        {
                            SerializedObject so_option = new SerializedObject(SelectedObjects[i]);

                            SerializedProperty c_sp_OptionSelector_Tween_GlobalDuration = so_option.FindProperty("OptionSelector_Tween_GlobalDuration");
                            SerializedProperty c_sp_RepeatTweenPlay = so_option.FindProperty("RepeatTweenPlay");
                            SerializedProperty c_sp_UseBlinked = so_option.FindProperty("UseBlinked");
                            SerializedProperty c_sp_UseEaseMotion = so_option.FindProperty("UseEaseMotion");
                            SerializedProperty c_sp_LerpSpeed = so_option.FindProperty("LerpSpeed");
                            SerializedProperty c_sp_TweenSpeed = so_option.FindProperty("TweenSpeed");
                            SerializedProperty c_sp_SelectorTweenMotion = so_option.FindProperty("SelectorTweenMotion");

                            so_option.Update();

                            c_sp_OptionSelector_Tween_GlobalDuration.floatValue = hop.OptionSelector_Tween_GlobalDuration;
                            c_sp_RepeatTweenPlay.boolValue = hop.RepeatTweenPlay;
                            c_sp_UseBlinked.boolValue = hop.UseBlinked;
                            c_sp_UseEaseMotion.boolValue = hop.UseEaseMotion;
                            c_sp_LerpSpeed.floatValue = hop.LerpSpeed;
                            c_sp_TweenSpeed.floatValue = hop.TweenSpeed;
                            c_sp_SelectorTweenMotion.enumValueIndex = (int)hop.SelectorTweenMotion;

                            c_sp_OptionSelector_Tween_GlobalDuration.serializedObject.ApplyModifiedProperties();
                            c_sp_RepeatTweenPlay.serializedObject.ApplyModifiedProperties();
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
                        sp_OptionSelector_Tween_GlobalDuration.floatValue = hop.OptionSelector_Tween_GlobalDuration;
                        sp_RepeatTweenPlay.boolValue = hop.RepeatTweenPlay;
                        sp_UseBlinked.boolValue = hop.UseBlinked;
                        sp_UseEaseMotion.boolValue = hop.UseEaseMotion;
                        sp_LerpSpeed.floatValue = hop.LerpSpeed;
                        sp_TweenSpeed.floatValue = hop.TweenSpeed;
                        sp_SelectorTweenMotion.enumValueIndex = (int)hop.SelectorTweenMotion;

                        sp_OptionSelector_Tween_GlobalDuration.serializedObject.ApplyModifiedProperties();
                        sp_RepeatTweenPlay.serializedObject.ApplyModifiedProperties();
                        sp_UseBlinked.serializedObject.ApplyModifiedProperties();
                        sp_UseEaseMotion.serializedObject.ApplyModifiedProperties();
                        sp_LerpSpeed.serializedObject.ApplyModifiedProperties();
                        sp_TweenSpeed.serializedObject.ApplyModifiedProperties();
                        sp_SelectorTweenMotion.serializedObject.ApplyModifiedProperties();
                    }
                });
                menu.AddSeparator("");
                menu.AddDisabledItem(new GUIContent("图元动画器"));
                menu.AddItem(new GUIContent("X (扫描)"), false, () =>
                {
                    string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 选项器消息", "获取子组件", "请确保子级中按钮的选项模式已开启！否则无法被扫描到并作为选项按钮！", "立即扫描", "再检查下", 1);
                    if (res == "再检查下")
                    {
                        return;
                    }

                    GetAllOptionsButton();
                    GetPrimitivesTween();
                    GetPrimitivesTweenResults();
                    return;
                });
                menu.AddDisabledItem(new GUIContent("基础"));
                menu.AddItem(new GUIContent("A (自动标识)"), false, () =>
                {
                    if (Targets_Selected())
                    {
                        for (int i = 0; i < SelectedObjects.Length; i++)
                        {
                            XHud_Module_Option opt = SelectedObjects[i];
                            opt.Indicator = opt.gameObject.name;
                            EditorUtility.SetDirty(opt);
                        }
                    }
                    else
                    {
                        Indicator.stringValue = BaseScript.gameObject.name;
                        Indicator.serializedObject.ApplyModifiedProperties();
                        Editor_XHud_GUI.Open(XHud_DialogType.修改, "XHud - 选项器消息", "标识自身", "将标识名称参数更改为物体的名称！", "明白");
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
                    if (!Preivew_Tween_PlayingState)
                    {
                        menu.AddItem(new GUIContent("S (开始)"), false, () =>
                        {
                            if (!Application.isPlaying)
                            {

                            }
                        });
                    }
                    else
                    {
                        menu.AddItem(new GUIContent("S (停止)"), false, () =>
                        {
                            if (!Application.isPlaying)
                            {

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

            PrimitiveTweens_MaxDuration_Calculate(BaseScript.PrimitiveControllerNodes, PrimitivesTweenGlobalDuration.floatValue);

            CorrectionSelectorWhenRectChangeSized();

            serializedObject.ApplyModifiedProperties();
        }

        #region 辅助
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
                        ElementNode_OptionButton node = BaseScript.OptionButtonNodes[i];

                        //如果按钮列表有同名的按钮则计算光标位置到按钮坐标
                        if (node.Indicator == btn_names[OptionIndex.intValue])
                        {
                            Vector3 target = sel_opt_root.parent.InverseTransformPoint(node.Button.RectTransform.position);

                            CurrentOptionName.stringValue = node.Indicator;

                            SelectorOffsetAdded.vector3Value = target;
                            Pos_Destination.vector3Value = target;
                            sm_Pos_Destination.vector3Value = target;


                            sel_mark.anchoredPosition3D = new Vector3(Pos_Destination.vector3Value.x, Pos_Destination.vector3Value.y, sel_mark.anchoredPosition3D.z) + SelectorOffset.vector3Value;

                            // 记录光标在因选项选择后改变的图元特性姿态
                            SaveSelectorMarkFeature(sel_mark);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 记录光标在因选项选择后改变的图元特性姿态
        /// </summary>
        private void SaveSelectorMarkFeature(RectTransform img)
        {
            if (img == null)
                return;

            XHud_Module_Primitive_Controller con = img.GetComponent<XHud_Module_Primitive_Controller>();

            if (con == null)
            {
                return;
            }

            SerializedObject so = new SerializedObject(con);
            so.Update();
            SerializedProperty sp_con_feature = so.FindProperty("pt_Feature");
            XHud_Module_Primitive_Feature feature = (XHud_Module_Primitive_Feature)sp_con_feature.objectReferenceValue;

            feature.PrimitiveFeature_Save();

            so.ApplyModifiedProperties();
        }
        /// <summary>
        /// 检查是否存在无效的选项按钮
        /// </summary>
        private void CheckButtonsValid()
        {
            for (int i = 0; i < PrimitiveControllerNodes.arraySize; i++)
            {
                SerializedProperty sp_buttonNode = OptionButtonNodes.GetArrayElementAtIndex(i);
                SerializedProperty sp_button = sp_buttonNode.FindPropertyRelative("Button");

                if (sp_button.objectReferenceValue == null)
                {
                    OptionButtonNodes.DeleteArrayElementAtIndex(i);
                }
            }
        }
        /// <summary>
        /// 检查是否存在无效的图元动画器
        /// </summary>
        private void CheckTweensValid()
        {
            for (int i = 0; i < PrimitiveControllerNodes.arraySize; i++)
            {
                SerializedProperty sp_node = PrimitiveControllerNodes.GetArrayElementAtIndex(i);
                SerializedProperty sp_node_con = sp_node.FindPropertyRelative("Controller");

                if (sp_node_con.objectReferenceValue == null)
                {
                    PrimitiveControllerNodes.DeleteArrayElementAtIndex(i);
                }
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
                    SerializedProperty sp_tweens_isfold = so_ele.FindProperty("PrimitivesIsFold");
                    SerializedProperty sp_btn_isfold = so_ele.FindProperty("ButtonIsFold");
                    SerializedProperty sp_event_isfold = so_ele.FindProperty("EventIsFold");
                    so_ele.Update();

                    sp_tweens_isfold.boolValue = state;
                    sp_btn_isfold.boolValue = state;
                    sp_event_isfold.boolValue = state;
                    sp_event_isfold.serializedObject.ApplyModifiedProperties();
                    sp_tweens_isfold.serializedObject.ApplyModifiedProperties();
                    sp_btn_isfold.serializedObject.ApplyModifiedProperties();
                    so_ele.ApplyModifiedProperties();
                }
            }
            else
            {
                if (target != null)
                {
                    PrimitivesIsFold.boolValue = state;
                    ButtonIsFold.boolValue = state;
                    EventIsFold.boolValue = state;
                    EventIsFold.serializedObject.ApplyModifiedProperties();
                    PrimitivesIsFold.serializedObject.ApplyModifiedProperties();
                    ButtonIsFold.serializedObject.ApplyModifiedProperties();
                }
            }
        }
        /// <summary>
        /// 计算最大耗时
        /// </summary>
        /// <param name="list"></param>
        /// <param name="globaldur"></param>
        private void PrimitiveTweens_MaxDuration_Calculate(List<PrimitiveControllerNode> list, float globaldur)
        {
            PrimitivesTweenMaxDuration.floatValue = PrimitiveTweens_MaxDuration_Get(list, globaldur);
            PrimitivesTweenMaxDuration.serializedObject.ApplyModifiedProperties();
        }
        /// <summary>
        /// 从所有子图元动画器中获取最大耗时
        /// </summary>
        /// <returns></returns>
        public float PrimitiveTweens_MaxDuration_Get(List<PrimitiveControllerNode> list, float globaldur)
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
        /// 获取图元动画器中是否存在循环模式
        /// </summary>
        /// <returns></returns>
        public bool PrimitiveTweens_HasLoopMode()
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
        /// 获取图元动画器中是否存在循环模式
        /// </summary>
        /// <returns></returns>
        public bool PrimitiveTweenNodes_HasLoopMode(List<TweenNode> tweenlist)
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
        /// 获取序列化字段
        /// </summary>
        private void GetSerializeFields()
        {
            OptionButtonNodes = serializedObject.FindProperty("OptionButtonNodes");
            UseEaseMotion = serializedObject.FindProperty("UseEaseMotion");
            LerpSpeed = serializedObject.FindProperty("LerpSpeed");
            SelectorTweenMotion = serializedObject.FindProperty("SelectorTweenMotion");
            SelectorOffset = serializedObject.FindProperty("SelectorOffset");
            SelectorOffsetAdded = serializedObject.FindProperty("SelectorOffsetAdded");
            RepeatTweenPlay = serializedObject.FindProperty("RepeatTweenPlay");
            TweenSpeed = serializedObject.FindProperty("TweenSpeed");
            OptionSelector = serializedObject.FindProperty("SelectorMark");
            CurrentOptionName = serializedObject.FindProperty("CurrentOptionName");
            OptionIndex = serializedObject.FindProperty("OptionIndex");
            SelectorMark = serializedObject.FindProperty("SelectorMark");
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
            AnimateState = serializedObject.FindProperty("AnimateState");
            Indicator = serializedObject.FindProperty("Indicator");
            ButtonIsFold = serializedObject.FindProperty("ButtonIsFold");
            EventIsFold = serializedObject.FindProperty("EventIsFold");
            DebugState = serializedObject.FindProperty("DebugState");
            ChangingInterval = serializedObject.FindProperty("ChangingInterval");
            AutoStopPreview = serializedObject.FindProperty("AutoStopPreview");

            PrimitiveControllerNodes = serializedObject.FindProperty("PrimitiveControllerNodes");
            PrimitivesTweenMaxDuration = serializedObject.FindProperty("PrimitivesTweenMaxDuration");
            PrimitivesTweenGlobalDuration = serializedObject.FindProperty("PrimitivesTweenGlobalDuration");
            PrimitivesIsFold = serializedObject.FindProperty("PrimitivesIsFold");
        }
        #endregion

        #region 获取 PrimitivesTween
        /// <summary>
        /// 扫描所有图元动画
        /// </summary>
        private void GetPrimitivesTween()
        {
            if (Targets_Selected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    SerializedObject so_ele = new SerializedObject(SelectedObjects[i]);
                    so_ele.Update();
                    SerializedProperty sp_nodes = so_ele.FindProperty("PrimitiveControllerNodes");
                    sp_nodes.ClearArray();
                    XHud_Module_Primitive_Controller[] cons = SelectedObjects[i].GetComponentsInChildren<XHud_Module_Primitive_Controller>();
                    for (int x = 0; x < cons.Length; x++)
                    {
                        bool repeat = false;

                        if (sp_nodes.arraySize > 0)
                        {
                            for (int s = 0; s < sp_nodes.arraySize; s++)
                            {
                                SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                                SerializedProperty sp_node_con = sp_node.FindPropertyRelative("Controller");
                                XHud_Module_Primitive_Controller sp_con = (XHud_Module_Primitive_Controller)sp_node_con.objectReferenceValue;
                                if (sp_con == cons[x])
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
                            SerializedProperty sp_node_con = sp_node.FindPropertyRelative("Controller");
                            sp_node_con.objectReferenceValue = cons[x];

                            sp_node_con.serializedObject.ApplyModifiedProperties();
                            sp_node.serializedObject.ApplyModifiedProperties();
                        }
                    }
                    sp_nodes.serializedObject.ApplyModifiedProperties();
                    so_ele.ApplyModifiedProperties();
                }
            }
            else
            {
                SerializedProperty sp_nodes = serializedObject.FindProperty("PrimitiveControllerNodes");
                sp_nodes.ClearArray();
                XHud_Module_Primitive_Controller[] cons = BaseScript.GetComponentsInChildren<XHud_Module_Primitive_Controller>();
                for (int i = 0; i < cons.Length; i++)
                {
                    bool repeat = false;

                    if (sp_nodes.arraySize > 0)
                    {
                        for (int s = 0; s < sp_nodes.arraySize; s++)
                        {
                            SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                            SerializedProperty sp_node_con = sp_node.FindPropertyRelative("Controller");
                            XHud_Module_Primitive_Controller sp_con = (XHud_Module_Primitive_Controller)sp_node_con.objectReferenceValue;
                            if (sp_con == cons[i])
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
                        SerializedProperty sp_node_con = sp_node.FindPropertyRelative("Controller");
                        sp_node_con.objectReferenceValue = cons[i];

                        sp_node_con.serializedObject.ApplyModifiedProperties();
                        sp_node.serializedObject.ApplyModifiedProperties();
                    }
                }
                sp_nodes.serializedObject.ApplyModifiedProperties();
            }
        }
        /// <summary>
        /// 扫描图元动画的结果报告
        /// </summary>
        private void GetPrimitivesTweenResults()
        {
            List<XHud_GUI_Dialog_ListDatas> Datas = new List<XHud_GUI_Dialog_ListDatas>();
            if (Targets_Selected())
            {
                for (int s = 0; s < SelectedObjects.Length; s++)
                {
                    XHud_Module_Option opt = SelectedObjects[s];
                    if (opt.PrimitiveControllerNodes.Count > 0)
                    {
                        for (int i = 0; i < opt.PrimitiveControllerNodes.Count; i++)
                        {
                            XHud_GUI_Dialog_ListDatas dataitem = new XHud_GUI_Dialog_ListDatas();
                            dataitem.Title = $"{opt.name} ( {opt.Indicator} )";
                            dataitem.SubTitle = $"扫描到图元动画器";
                            dataitem.Message = $"{opt.PrimitiveControllerNodes[i].Controller.name} ( {opt.PrimitiveControllerNodes[i].Controller.Indicator} )";
                            Datas.Add(dataitem);
                        }
                    }
                    if (opt.OptionButtonNodes.Count > 0)
                    {
                        for (int i = 0; i < opt.OptionButtonNodes.Count; i++)
                        {
                            XHud_GUI_Dialog_ListDatas dataitem = new XHud_GUI_Dialog_ListDatas();

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
                    Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 选项器消息", "扫描动选项器子组件", "未扫描到任何有效的子组件！", "明白");
                }
                else
                {
                    Editor_XHud_GUI.Open(Datas.ToArray(), XHud_DialogType.确认, "XHud - 选项器消息", "扫描动选项器子组件", "以下是扫描到的所有选项子组件列表，请您检查核对：", "明白");
                }
            }
            else
            {
                if (BaseScript.PrimitiveControllerNodes.Count > 0)
                {
                    for (int i = 0; i < BaseScript.PrimitiveControllerNodes.Count; i++)
                    {
                        XHud_GUI_Dialog_ListDatas dataitem = new XHud_GUI_Dialog_ListDatas();
                        dataitem.Title = $"{BaseScript.name} ( {BaseScript.Indicator} )";
                        dataitem.SubTitle = $"扫描到图元动画器";
                        dataitem.Message = $"{BaseScript.PrimitiveControllerNodes[i].Controller.name} ( {BaseScript.PrimitiveControllerNodes[i].Controller.Indicator} )";
                        Datas.Add(dataitem);
                    }
                }
                if (BaseScript.OptionButtonNodes.Count > 0)
                {
                    for (int i = 0; i < BaseScript.OptionButtonNodes.Count; i++)
                    {
                        XHud_GUI_Dialog_ListDatas dataitem = new XHud_GUI_Dialog_ListDatas();

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
                    Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 选项器消息", "扫描动选项器子组件", "未扫描到任何有效的子组件！", "明白");
                }
                else
                {
                    Editor_XHud_GUI.Open(Datas.ToArray(), XHud_DialogType.确认, "XHud - 选项器消息", "扫描选项按钮组件", "以下是扫描到的所有选项按钮组件列表，请您检查核对：", "明白");
                }
            }
        }
        #endregion

        #region 获取 OptionsButton
        /// <summary>
        /// 扫描所有选项按钮
        /// </summary>
        private void GetAllOptionsButton()
        {
            if (Targets_Selected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    SerializedObject so_ele = new SerializedObject(SelectedObjects[i]);
                    so_ele.Update();
                    SerializedProperty sp_nodes = so_ele.FindProperty("OptionButtonNodes");
                    SerializedProperty sp_optionindex = so_ele.FindProperty("OptionIndex");
                    SerializedProperty sp_currentOptionName = so_ele.FindProperty("CurrentOptionName");

                    XHud_Module_Button[] allbtns = SelectedObjects[i].GetComponentsInChildren<XHud_Module_Button>();

                    List<XHud_Module_Button> Filter = new List<XHud_Module_Button>();
                    for (int s = 0; s < allbtns.Length; s++)
                    {
                        if (allbtns[i].IsOptionButton)
                            Filter.Add(allbtns[s]);
                    }
                    if (Filter.Count <= 0)
                        continue;
                    XHud_Module_Button[] gettedBtns = Filter.ToArray();

                    for (int c = 0; c < gettedBtns.Length; c++)
                    {
                        bool repeat = false;

                        if (sp_nodes.arraySize > 0)
                        {
                            for (int s = 0; s < sp_nodes.arraySize; s++)
                            {
                                SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                                SerializedProperty sp_node_btn = sp_node.FindPropertyRelative("Button");
                                XHud_Module_Button sp_btn = (XHud_Module_Button)sp_node_btn.objectReferenceValue;
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
                    XHud_Module_Button sp_s_btn = (XHud_Module_Button)sp_nodesin.objectReferenceValue;
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

                XHud_Module_Button[] allbtns = BaseScript.GetComponentsInChildren<XHud_Module_Button>();

                List<XHud_Module_Button> Filter = new List<XHud_Module_Button>();
                for (int i = 0; i < allbtns.Length; i++)
                {
                    if (allbtns[i].IsOptionButton)
                        Filter.Add(allbtns[i]);
                }
                if (Filter.Count <= 0)
                    return;
                XHud_Module_Button[] gettedBtns = Filter.ToArray();
                for (int i = 0; i < gettedBtns.Length; i++)
                {
                    bool repeat = false;

                    if (sp_nodes.arraySize > 0)
                    {
                        for (int s = 0; s < sp_nodes.arraySize; s++)
                        {
                            SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                            SerializedProperty sp_node_btn = sp_node.FindPropertyRelative("Button");
                            XHud_Module_Button sp_btn = (XHud_Module_Button)sp_node_btn.objectReferenceValue;
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
                XHud_Module_Button sp_s_btn = (XHud_Module_Button)sp_nodesin.objectReferenceValue;
                sp_currentOptionName.stringValue = sp_s_btn.Indicator;
                sp_currentOptionName.serializedObject.ApplyModifiedProperties();
            }
        }
        #endregion
    }
}