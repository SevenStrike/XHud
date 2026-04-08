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
        private ReorderableList PrimitivesTweenList;
        private XHud_Module_Toggle BaseScript;
        #endregion

        #region 序列化属性
        private SerializedProperty sp_Indicator, PrimitivesIsFold, sp_debugstate, eve_on_Checked, eve_on_UnChecked, eve_on_ValueChanged, ChangingInterval, ToggleName, PrimitivesTweenMaxDuration, ToggleText, ToggleTmpText, Tog_Bg, AnimateState, BgCanToggle, TitleCanToggle, HandlePosRange, HandleProgress, Tog_Handle, Tog_Color_Bg_Unchecked, Tog_Color_Bg_Checked, Tog_Color_Handle_Unchecked, Tog_Color_Handle_Checked, HandleProgressDuration, ProgressEase, ColorDuration, ColorEase, EaseMotion, eve_on_Press, eve_on_Released, PrimitivesTweenGlobalDuration, ToggleEventIsFold, ToggleOriginalIsFold, PrimitiveControllerNodes, AutoStopPreview, ToggleIsChecked;
        #endregion

        #region 图标                                                                                                                                     
        private Texture2D icon_main, find_r, find_p, play_r, play_p, stop_r, stop_p, clear_r, clear_p, icon_button, animstate, dutation, longpressmarker, left_arrow_r, left_arrow_p, right_arrow_r, right_arrow_p, icon_anim;
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

        #region Preview - Animator
        private bool Preivew_Animator_PlayingState;
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

            #region ReorderableList - PrimitivesTween
            PrimitivesTweenList = new ReorderableList(serializedObject, PrimitiveControllerNodes)
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
                        SerializedObject so_tween = new SerializedObject(sp_con.pt_Tween);
                        so_tween.Update();

                        SerializedProperty sp_glodur = so_tween.FindProperty("GlobalDuration");
                        SerializedProperty sp_maxdur = so_tween.FindProperty("MaxTimerWithGlobalDuration");

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

            Targets_Get();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (!Application.isPlaying)
            {

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
                GetPrimitivesTween();

                GetPrimitivesTweenResults();
                return;
            }
            #endregion

            #region 预览
            if (!Targets_Selected() && PrimitiveControllerNodes.arraySize > 0)
            {
                GUILayout.FlexibleSpace();

                #region 预览动画
                if (!Preivew_Animator_PlayingState)
                {
                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "播放所有动画器预览", play_r, play_p))
                    {
                        if (!Application.isPlaying)
                        {

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

            Editor_XHud_GUI.Gui_Layout_Property_Field("速率倍增", PrimitivesTweenGlobalDuration, 100);

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
                if (PrimitiveControllerNodes != null && PrimitiveControllerNodes.arraySize > 0)
                {
                    #region 动画状态     
                    Editor_XHud_GUI.StatuDisplayer_text(animstate, 12, new Vector2(0, 7), "动画状态", 12, (XHudElementAnimateState)AnimateState.enumValueIndex == XHudElementAnimateState.Animating ? "动画中" : "静止状态", AnimateState.enumValueIndex == 1 ? XHud_Dashboard.Theme_Primary : Color.gray, 11);
                    #endregion

                    #region 最大耗时     
                    Editor_XHud_GUI.StatuDisplayer_text(dutation, 12, new Vector2(0, 7), "最大耗时<color=#909090>（速率倍增）</color>", 12, PrimitivesTweenMaxDuration.floatValue.ToString() + "秒", XHud_Dashboard.Theme_Primary, 11);
                    #endregion

                    #region 最大耗时        
                    Editor_XHud_GUI.StatuDisplayer_text(dutation, 12, new Vector2(0, 7), "最大耗时<color=#909090>（XHUD倍增）</color>", 12, (mgr.DurationMultiply * PrimitivesTweenMaxDuration.floatValue).ToString() + "秒", XHud_Dashboard.Theme_Primary, 11);
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
                if (SelectedObjects[ToggleStatu_Index].PrimitiveControllerNodes != null && SelectedObjects[ToggleStatu_Index].PrimitiveControllerNodes.Count > 0)
                {
                    #region 动画状态     
                    Editor_XHud_GUI.StatuDisplayer_text(animstate, 12, new Vector2(0, 7), "动画状态", 12, SelectedObjects[ToggleStatu_Index].AnimateState == XHudElementAnimateState.Animating ? "动画中" : "静止状态", SelectedObjects[ToggleStatu_Index].AnimateState == XHudElementAnimateState.Animating ? XHud_Dashboard.Theme_Primary : Color.gray, 11);
                    #endregion

                    SelectedObjects[ToggleStatu_Index].PrimitivesTweenMaxDuration = Animators_GetAnimatorsMaxDuration(SelectedObjects[ToggleStatu_Index].PrimitiveControllerNodes, SelectedObjects[ToggleStatu_Index].PrimitivesTweenGlobalDuration);

                    #region 最大耗时     
                    Editor_XHud_GUI.StatuDisplayer_text(dutation, 12, new Vector2(0, 7), "最大耗时<color=#909090>（速率倍增）</color>", 12, SelectedObjects[ToggleStatu_Index].PrimitivesTweenMaxDuration.ToString() + "秒", XHud_Dashboard.Theme_Primary, 11);
                    #endregion

                    #region 最大耗时        
                    Editor_XHud_GUI.StatuDisplayer_text(dutation, 12, new Vector2(0, 7), "最大耗时<color=#909090>（XHUD倍增）</color>", 12, (mgr.DurationMultiply * SelectedObjects[ToggleStatu_Index].PrimitivesTweenMaxDuration).ToString() + "秒", XHud_Dashboard.Theme_Primary, 11);
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
                    #region 组件数量 - 动画器
                    if (PrimitiveControllerNodes.arraySize > 0)
                    {
                        Editor_XHud_GUI.StatuDisplayer_text(icon_anim, 12, new Vector2(0, 7), "动画器", 12, PrimitiveControllerNodes.arraySize.ToString() + " 个", XHud_Dashboard.Theme_Primary, 11);
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

                if (SelectedObjects[ToggleStatistic_Index].PrimitiveControllerNodes.Count <= 0)
                {
                    Editor_XHud_GUI.Gui_Layout_Labelfield("暂无统计数据", HudFilled.无, HudColor.无, Editor_XHud_GUI.GetColor(HudColor.阴影灰), TextAnchor.MiddleCenter);
                }
                else
                {
                    #region 组件数量 - 动画器
                    if (SelectedObjects[ToggleStatistic_Index].PrimitiveControllerNodes.Count > 0)
                    {
                        Editor_XHud_GUI.StatuDisplayer_text(icon_anim, 12, new Vector2(0, 7), "动画器", 12, SelectedObjects[ToggleStatistic_Index].PrimitiveControllerNodes.Count.ToString() + " 个", XHud_Dashboard.Theme_Primary, 11);
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
            if (PrimitiveControllerNodes.arraySize > 0)
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
                    PrimitivesIsFold.boolValue = EditorGUILayout.Foldout(PrimitivesIsFold.boolValue, "动画器", true);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    if (PrimitivesIsFold.boolValue)
                    {
                        PrimitivesTweenList.DoLayoutList();
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

            if (HandleProgress.floatValue >= 0.999f)
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

                    GetPrimitivesTween();

                    GetPrimitivesTweenResults();
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

            CalculateAnimatorMaxDuration(BaseScript.PrimitiveControllerNodes, PrimitivesTweenGlobalDuration.floatValue);

            serializedObject.ApplyModifiedProperties();
        }

        #region 辅助
        /// <summary>
        /// 检查是否存在无效的动画器
        /// </summary>
        private void CheckAnimatorsValid()
        {
            for (int i = 0; i < PrimitiveControllerNodes.arraySize; i++)
            {
                SerializedProperty sp_animotrNode = PrimitiveControllerNodes.GetArrayElementAtIndex(i);
                SerializedProperty sp_animator = sp_animotrNode.FindPropertyRelative("Controller");

                if (sp_animator.objectReferenceValue == null)
                {
                    PrimitiveControllerNodes.DeleteArrayElementAtIndex(i);
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
                    PrimitivesIsFold.boolValue = state;
                    ToggleEventIsFold.serializedObject.ApplyModifiedProperties();
                    PrimitivesIsFold.serializedObject.ApplyModifiedProperties();
                }
            }
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
            HandlePosRange = serializedObject.FindProperty("HandlePosRange");
            HandleProgress = serializedObject.FindProperty("HandleProgress");
            HandleProgressDuration = serializedObject.FindProperty("HandleProgressDuration");
            ProgressEase = serializedObject.FindProperty("ProgressEase");
            ColorEase = serializedObject.FindProperty("ColorEase");
            EaseMotion = serializedObject.FindProperty("EaseMotion");
            BgCanToggle = serializedObject.FindProperty("BgCanToggle");
            TitleCanToggle = serializedObject.FindProperty("TitleCanToggle");
            ColorDuration = serializedObject.FindProperty("ColorDuration");
            AnimateState = serializedObject.FindProperty("AnimateState");
            ToggleEventIsFold = serializedObject.FindProperty("ToggleEventIsFold");
            ToggleOriginalIsFold = serializedObject.FindProperty("ToggleOriginalIsFold");
            ChangingInterval = serializedObject.FindProperty("ChangingInterval");
            AutoStopPreview = serializedObject.FindProperty("AutoStopPreview");
            ToggleIsChecked = serializedObject.FindProperty("ToggleIsChecked");

            PrimitiveControllerNodes = serializedObject.FindProperty("PrimitiveControllerNodes");
            PrimitivesTweenGlobalDuration = serializedObject.FindProperty("PrimitivesTweenGlobalDuration");
            PrimitivesTweenMaxDuration = serializedObject.FindProperty("PrimitivesTweenMaxDuration");
            PrimitivesIsFold = serializedObject.FindProperty("PrimitivesIsFold");
        }
        #endregion

        #region 获取 PrimitivesTween
        /// <summary>
        /// 获取所有图元动画器
        /// </summary>
        private void GetPrimitivesTween()
        {
            if (Targets_Selected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    SerializedObject so_ele = new SerializedObject(SelectedObjects[i]);
                    SerializedProperty sp_nodes = so_ele.FindProperty("PrimitiveControllerNodes");
                    so_ele.Update();
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
                XHud_Module_Primitive_Controller[] pri_controller = BaseScript.GetComponentsInChildren<XHud_Module_Primitive_Controller>();
                for (int i = 0; i < pri_controller.Length; i++)
                {
                    bool repeat = false;

                    if (sp_nodes.arraySize > 0)
                    {
                        for (int s = 0; s < sp_nodes.arraySize; s++)
                        {
                            SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                            SerializedProperty sp_node_con = sp_node.FindPropertyRelative("Controller");
                            XHud_Module_Primitive_Controller sp_con = (XHud_Module_Primitive_Controller)sp_node_con.objectReferenceValue;
                            if (sp_con == pri_controller[i])
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
                        sp_node_con.objectReferenceValue = pri_controller[i];

                        sp_node_con.serializedObject.ApplyModifiedProperties();
                        sp_node.serializedObject.ApplyModifiedProperties();
                    }
                }
                sp_nodes.serializedObject.ApplyModifiedProperties();
            }
        }
        /// <summary>
        /// 扫描图元动画器的结果报告
        /// </summary>
        private void GetPrimitivesTweenResults()
        {
            List<XHud_GUI_Dialog_ListDatas> Datas = new List<XHud_GUI_Dialog_ListDatas>();
            if (Targets_Selected())
            {
                var s = targets;
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    XHud_Module_Toggle tog = SelectedObjects[i];
                    if (tog.PrimitiveControllerNodes.Count > 0)
                    {
                        for (int c = 0; c < tog.PrimitiveControllerNodes.Count; c++)
                        {
                            XHud_GUI_Dialog_ListDatas dataitem = new XHud_GUI_Dialog_ListDatas();

                            dataitem.Title = $"{tog.name} ( {tog.Indicator} )";
                            dataitem.SubTitle = $"扫描到图元动画器";
                            dataitem.Message = $"{tog.PrimitiveControllerNodes[c].Controller.name} ( {tog.PrimitiveControllerNodes[c].Controller.Indicator} )";

                            Datas.Add(dataitem);
                        }
                    }
                }
                Editor_XHud_GUI.Open(Datas.ToArray(), XHud_DialogType.确认, "XHud - 开关消息", "批量扫描图元动画器组件", "以下是批量扫描到的所有图元动画器组件列表，请您检查核对：", "明白");
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
                    Editor_XHud_GUI.Open(Datas.ToArray(), XHud_DialogType.确认, "XHud - 开关消息", "扫描图元动画器组件", "以下是扫描到的所有图元动画器组件列表，请您检查核对：", "明白");
                }
                else
                {
                    Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 开关消息", "扫描图元动画器组件", "未扫描到任何图元动画器组件！", "明白");
                }
            }
        }
        #endregion
    }
}