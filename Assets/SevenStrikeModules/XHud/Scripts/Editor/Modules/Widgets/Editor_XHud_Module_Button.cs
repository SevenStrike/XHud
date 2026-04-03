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
    using System.Collections;
    using System.Collections.Generic;
    using Unity.EditorCoroutines.Editor;
    using UnityEditor;
    using UnityEditor.UI;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.UI;

    public class XHud_ModuleArg_Button
    {
        public Color Color_Normal;
        public Color Color_Highlight;
        public Color Color_Press;
        public Color Color_Select;
        public Color Color_Disable;
        public float ColorMultiplier;
        public float ColorFadeDuration;
        public string Indicator;
        public bool DebugState;
        public bool IsOptionButton;
        public string ButtonName;
        public float LongPress_Threahold;
        public float LongPress_Percent;
        public bool LongPress_SmoothRewind;
        public bool LongPress_UseRewind;
        public bool TextColorSyncFade;
        public bool IconColorSyncFade;
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(XHud_Module_Button), true)]
    public class Editor_XHud_Module_Button : ButtonEditor
    {
        #region 组件 / 列表
        private XHud_Module_Button BaseScript;
        private ReorderableList BtnAnimatorList;
        #endregion

        /// <summary>
        /// 原始脚本参数显示开关
        /// </summary>
        private bool OriginalDisplay;

        private float LineHeight;

        #region 序列化属性
        private SerializedProperty Indicator, DebugState, AnimatorsMaxDuration, AnimateState, ButtonActionTiming, IsOptionButton, TextColorSyncFade, IconColorSyncFade, IconImage, LongPress_Step, Button_Animators_GlobalDuration, eve_on_Deselect, eve_on_Select, eve_on_Click, eve_on_Press, eve_on_Enter, eve_on_Exit, eve_on_Release, eve_on_LongPressed, eve_on_LongpressPer, LongPress_Percent, BtnAnimatorNodes, ButtonName, AnimatorsIsFold, ClickDelayTimeThreadhold, EventIsFold, ToggleOriginalIsFold, ButtonText, ButtonTmpText, LongPress_Threshold, LongPress_SmoothRewind, LongPress_UseRewind, HudButtonState_Clicked, HudButtonState_LongPressed, HudButtonState_Selector, HudButtonState_Holder, HudButtonState_Pressed, AutoStopPreview, BgColorSyncFade, BgImage;
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
        private int ButtonStatu_Index;
        private int ButtonStatistic_Index;
        #endregion

        #region 选项文字
        string[] stroptions_enabled = new string[2] { "关闭", "开启" }, stroptions_debug = new string[2] { "关闭", "调试" }, stroptions_hide = new string[2] { "隐藏", "显示" }, stroptions_smoothback = new string[2] { "快速", "平滑" }, stroptions_threholdback = new string[2] { "释放", "阈值" }, str_clickthreadhold = new string[9] { "无", "鼠标进入", "鼠标退出", "鼠标按下", "鼠标松开", "鼠标长按", "鼠标点击", "鼠标选中", "鼠标取消选中" };
        #endregion

        #region 批量化操作
        XHud_Module_Button[] SelectedObjects;

        private void Targets_Get()
        {
            if (targets.Length > 1)
            {
                SelectedObjects = new XHud_Module_Button[targets.Length];
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    var t = targets[i];
                    SelectedObjects[i] = (XHud_Module_Button)t;
                }
            }
            else
            {
                SelectedObjects = new XHud_Module_Button[targets.Length];
                SelectedObjects[0] = (XHud_Module_Button)target;
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
            BaseScript = (XHud_Module_Button)target;

            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

            // 获取序列化属性
            GetSerializeFields();

            #region 获取图标
            icon_main = Editor_XHud_GUI.GetIcon("Icons_XHud_Button/icon_main");
            find_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Button/find_r");
            find_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Button/find_p");
            play_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Button/play_r");
            play_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Button/play_p");
            stop_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Button/stop_r");
            stop_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Button/stop_p");
            clear_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Button/clear_r");
            clear_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Button/clear_p");
            icon_button = Editor_XHud_GUI.GetIcon("Icons_XHud_Button/icon_button");
            animstate = Editor_XHud_GUI.GetIcon("Icons_XHud_Button/animstate");
            dutation = Editor_XHud_GUI.GetIcon("Icons_XHud_Button/dutation");
            longpressmarker = Editor_XHud_GUI.GetIcon("Icons_XHud_Button/longpressmarker");
            left_arrow_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Button/left_arrow_r");
            left_arrow_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Button/left_arrow_p");
            right_arrow_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Button/right_arrow_r");
            right_arrow_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Button/right_arrow_p");
            icon_anim = Editor_XHud_GUI.GetIcon("Icons_XHud_Button/icon_anim");
            #endregion

            LineHeight = EditorGUIUtility.singleLineHeight;

            #region ReorderableList - Animators
            BtnAnimatorList = new ReorderableList(serializedObject, BtnAnimatorNodes)
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

                    SerializedProperty sp_root = BtnAnimatorNodes.GetArrayElementAtIndex(index);
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
                        BtnAnimatorNodes.DeleteArrayElementAtIndex(index);
                        BtnAnimatorNodes.serializedObject.ApplyModifiedProperties();
                    }
                },
                onSelectCallback = (ReorderableList list) =>
                {
                    if (!Application.isPlaying)
                    {
                        Preview_Animator_Stop();
                    }

                    SerializedProperty sp_root = BtnAnimatorNodes.GetArrayElementAtIndex(list.index);
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

            if (string.IsNullOrEmpty(BaseScript.Indicator))
                Editor_XHud_GUI.Gui_Layout_Banner(icon_main, HudFilled.实体, HudColor.深空灰, "XHud - 按钮", Color.white);
            else
                Editor_XHud_GUI.Gui_Layout_Banner(icon_main, HudFilled.实体, HudColor.深空灰, "XHud - 按钮 -> ( " + BaseScript.Indicator + " )", Color.white);

            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

            #region 快捷功能
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
            if (!Targets_Selected() && BtnAnimatorNodes.arraySize > 0)
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

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_Button>("原生参数", stroptions_hide, ref ToggleOriginalIsFold, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_Button>("状态调试", stroptions_debug, ref DebugState, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_Button>("自动停止预览", stroptions_enabled, ref AutoStopPreview, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_Button>("选项模式", stroptions_enabled, ref IsOptionButton, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

            if (IsOptionButton.boolValue)
                Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_Module_Button>("作为选项按钮激活条件", str_clickthreadhold, ref ButtonActionTiming, HudFilled.实体, 120, 22, SelectedObjects);

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_Button>("文字变色", stroptions_enabled, ref TextColorSyncFade, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_Button>("图标变色", stroptions_enabled, ref IconColorSyncFade, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_Button>("背景变色", stroptions_enabled, ref BgColorSyncFade, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_Button>("长按回退机制", stroptions_threholdback, ref LongPress_UseRewind, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_Button>("长按回退方式", stroptions_smoothback, ref LongPress_SmoothRewind, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

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

            #region 长按指示器
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "长按进度指示器", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            Rect rect_longpress = GUILayoutUtility.GetLastRect();

            EditorGUI.DrawRect(new Rect(rect_longpress.x + 5, rect_longpress.y + 22, (EditorGUIUtility.currentViewWidth - 65), 1), Color.black * 0.3f);

            EditorGUI.DrawRect(new Rect(rect_longpress.x + 5, rect_longpress.y + 22, (EditorGUIUtility.currentViewWidth - 65) * LongPress_Percent.floatValue, 1), XHud_Dashboard.Theme_Primary);

            Editor_XHud_GUI.Gui_Labelfield_Thin(new Rect(rect_longpress.x + 5, rect_longpress.y + 8, 50, 6), "ClickArea", HudFilled.无, HudColor.无, XHud_Dashboard.Theme_Primary, TextAnchor.MiddleLeft, new Vector2(0, 0), 9);

            Editor_XHud_GUI.Gui_Labelfield_Thin(new Rect(rect_longpress.x + (EditorGUIUtility.currentViewWidth - 110), rect_longpress.y + 8, 50, 6), "LongPress", HudFilled.无, HudColor.无, Color.gray, TextAnchor.MiddleRight, new Vector2(0, 0), 9);

            EditorGUI.DrawRect(new Rect((rect_longpress.x + 5), rect_longpress.y + 20, 1, 6), Color.gray);

            EditorGUI.DrawRect(new Rect((rect_longpress.x + (EditorGUIUtility.currentViewWidth - 60)), rect_longpress.y + 20, 1, 6), Color.gray);

            Editor_XHud_GUI.Gui_Icon(new Rect(((rect_longpress.x + 1) + (EditorGUIUtility.currentViewWidth - 65) * ClickDelayTimeThreadhold.floatValue), rect_longpress.y + 6, 8, 8), longpressmarker);

            EditorGUI.DrawRect(new Rect(((rect_longpress.x + 5) + (EditorGUIUtility.currentViewWidth - 65) * ClickDelayTimeThreadhold.floatValue), rect_longpress.y + 18, 1, 10), Color.red);

            Editor_XHud_GUI.Gui_Layout_Space(25);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();

            if (Application.isPlaying)
            {
                Repaint();
            }
            #endregion

            #region 参数
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "参数", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Property_Field("背景组件", BgImage, 100);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("图标组件", IconImage, 100);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("文本组件", ButtonText, 100);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("文本(TMP)组件", ButtonTmpText, 100);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("标识名称", Indicator, 100);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            EditorGUI.BeginChangeCheck();
            Editor_XHud_GUI.Gui_Layout_Property_Field("按钮文字", ButtonName, 100);
            if (EditorGUI.EndChangeCheck())
            {
                if (Targets_Selected())
                {
                    for (int i = 0; i < SelectedObjects.Length; i++)
                    {
                        SerializedObject so_ele = new SerializedObject(SelectedObjects[i]);
                        SerializedProperty ele_sp_ButtonName = so_ele.FindProperty("ButtonName");
                        SerializedProperty ele_sp_ButtonText = so_ele.FindProperty("OptionItemText");
                        SerializedProperty ele_sp_ButtonTmpText = so_ele.FindProperty("OptionItemTmpText");

                        so_ele.Update();

                        if (ele_sp_ButtonText.objectReferenceValue != null)
                        {
                            XHud_Module_Text tt = (XHud_Module_Text)ele_sp_ButtonText.objectReferenceValue;
                            tt.text = ele_sp_ButtonName.stringValue;
                            ele_sp_ButtonText.serializedObject.ApplyModifiedProperties();
                        }
                        if (ele_sp_ButtonTmpText.objectReferenceValue != null)
                        {
                            XHud_Module_TmpText tt = (XHud_Module_TmpText)ele_sp_ButtonTmpText.objectReferenceValue;
                            tt.text = ele_sp_ButtonName.stringValue;
                            ele_sp_ButtonTmpText.serializedObject.ApplyModifiedProperties();
                        }

                        so_ele.ApplyModifiedProperties();
                    }
                }
                else
                {
                    if (ButtonText.objectReferenceValue != null)
                    {
                        XHud_Module_Text tt = (XHud_Module_Text)ButtonText.objectReferenceValue;
                        tt.text = ButtonName.stringValue;
                    }
                    if (ButtonTmpText.objectReferenceValue != null)
                    {
                        XHud_Module_TmpText tt = (XHud_Module_TmpText)ButtonTmpText.objectReferenceValue;
                        tt.text = ButtonName.stringValue;
                    }
                }
            }

            Editor_XHud_GUI.Gui_Layout_Space(10);

            EditorGUILayout.HelpBox("点击动作阈值是指当用户进行长按操作时，如果 \"长按进度值\" 小于 \"此值\" 则判定为点击按钮，否则不执行按钮点击动作！）", MessageType.Info);

            Editor_XHud_GUI.Gui_Layout_Space(10);

            Editor_XHud_GUI.Gui_Layout_Property_Field("点击动作阈值", ClickDelayTimeThreadhold, 100);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("长按阈值", LongPress_Threshold, 100);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("长按步进值", LongPress_Step, 100);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("长按进度", LongPress_Percent, 100);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("速率倍增", Button_Animators_GlobalDuration, 100);

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
                Editor_XHud_GUI.StatuDisplayer_text(icon_button, 12, new Vector2(0, 7), "点击状态", 12, ((Hud_ButtonAction)HudButtonState_Clicked.enumValueIndex).ToString(), XHud_Dashboard.Theme_Primary, 11);
                Editor_XHud_GUI.StatuDisplayer_text(icon_button, 12, new Vector2(0, 7), "选择状态", 12, ((Hud_ButtonAction)HudButtonState_Selector.enumValueIndex).ToString(), XHud_Dashboard.Theme_Primary, 11);
                Editor_XHud_GUI.StatuDisplayer_text(icon_button, 12, new Vector2(0, 7), "按下状态", 12, ((Hud_ButtonAction)HudButtonState_Pressed.enumValueIndex).ToString(), XHud_Dashboard.Theme_Primary, 11);
                Editor_XHud_GUI.StatuDisplayer_text(icon_button, 12, new Vector2(0, 7), "悬停状态", 12, ((Hud_ButtonAction)HudButtonState_Holder.enumValueIndex).ToString(), XHud_Dashboard.Theme_Primary, 11);
                Editor_XHud_GUI.StatuDisplayer_text(icon_button, 12, new Vector2(0, 7), "长按状态", 12, ((Hud_ButtonAction)HudButtonState_LongPressed.enumValueIndex).ToString(), XHud_Dashboard.Theme_Primary, 11);
                #endregion

                #region 动画相关
                if (BtnAnimatorNodes != null && BtnAnimatorNodes.arraySize > 0)
                {
                    #region 动画状态     
                    Editor_XHud_GUI.StatuDisplayer_text(animstate, 12, new Vector2(0, 7), "动画状态", 12, (XHudElementAnimateState)AnimateState.enumValueIndex == XHudElementAnimateState.Animating ? "动画中" : "静止状态", AnimateState.enumValueIndex == 1 ? XHud_Dashboard.Theme_Primary : Color.gray, 11);
                    #endregion

                    #region 最大耗时     
                    Editor_XHud_GUI.StatuDisplayer_text(dutation, 12, new Vector2(0, 7), "最大耗时<color=#909090>（速率倍增）</color>", 12, AnimatorsMaxDuration.floatValue.ToString() + "秒", XHud_Dashboard.Theme_Primary, 11);
                    #endregion

                    #region 最大耗时        
                    Editor_XHud_GUI.StatuDisplayer_text(dutation, 12, new Vector2(0, 7), "最大耗时<color=#909090>（XHUD倍增）</color>", 12, (mgr.DurationMultiply * AnimatorsMaxDuration.floatValue).ToString() + "秒", XHud_Dashboard.Theme_Primary, 11);
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
                if (Editor_XHud_GUI.Gui_Layout_Button($"{SelectedObjects[ButtonStatu_Index].name} ( {SelectedObjects[ButtonStatu_Index].Indicator} )", "", HudFilled.透明, HudColor.无, Color.gray, 20))
                {
                    EditorGUIUtility.PingObject(SelectedObjects[ButtonStatu_Index]);
                }
                Editor_XHud_GUI.Gui_Layout_FlexSpace();
                if (Editor_XHud_GUI.Gui_Layout_Button(12, "", left_arrow_r, left_arrow_p))
                {
                    if (ButtonStatu_Index <= 0)
                    {
                        ButtonStatu_Index = SelectedObjects.Length - 1;
                    }
                    else
                    {
                        ButtonStatu_Index--;
                    }
                    EditorGUIUtility.PingObject(SelectedObjects[ButtonStatu_Index]);
                }
                Editor_XHud_GUI.Gui_Layout_Space(16);
                if (Editor_XHud_GUI.Gui_Layout_Button(12, "", right_arrow_r, right_arrow_p))
                {
                    if (ButtonStatu_Index >= SelectedObjects.Length - 1)
                    {
                        ButtonStatu_Index = 0;
                    }
                    else
                    {
                        ButtonStatu_Index++;
                    }
                    EditorGUIUtility.PingObject(SelectedObjects[ButtonStatu_Index]);
                }
                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                #endregion

                #region 按钮状态
                Editor_XHud_GUI.StatuDisplayer_text(icon_button, 12, new Vector2(0, 7), "点击状态", 12, SelectedObjects[ButtonStatu_Index].HudButtonState_Clicked.ToString(), XHud_Dashboard.Theme_Primary, 11);
                Editor_XHud_GUI.StatuDisplayer_text(icon_button, 12, new Vector2(0, 7), "选择状态", 12, SelectedObjects[ButtonStatu_Index].HudButtonState_Selector.ToString(), XHud_Dashboard.Theme_Primary, 11);
                Editor_XHud_GUI.StatuDisplayer_text(icon_button, 12, new Vector2(0, 7), "按下状态", 12, SelectedObjects[ButtonStatu_Index].HudButtonState_Pressed.ToString(), XHud_Dashboard.Theme_Primary, 11);
                Editor_XHud_GUI.StatuDisplayer_text(icon_button, 12, new Vector2(0, 7), "悬停状态", 12, SelectedObjects[ButtonStatu_Index].HudButtonState_Holder.ToString(), XHud_Dashboard.Theme_Primary, 11);
                Editor_XHud_GUI.StatuDisplayer_text(icon_button, 12, new Vector2(0, 7), "长按状态", 12, SelectedObjects[ButtonStatu_Index].HudButtonState_LongPressed.ToString(), XHud_Dashboard.Theme_Primary, 11);
                #endregion

                #region 动画相关
                if (SelectedObjects[ButtonStatu_Index].BtnAnimatorNodes != null && SelectedObjects[ButtonStatu_Index].BtnAnimatorNodes.Count > 0)
                {
                    #region 动画状态     
                    Editor_XHud_GUI.StatuDisplayer_text(animstate, 12, new Vector2(0, 7), "动画状态", 12, SelectedObjects[ButtonStatu_Index].AnimateState == XHudElementAnimateState.Animating ? "动画中" : "静止状态", SelectedObjects[ButtonStatu_Index].AnimateState == XHudElementAnimateState.Animating ? XHud_Dashboard.Theme_Primary : Color.gray, 11);
                    #endregion

                    SelectedObjects[ButtonStatu_Index].AnimatorsMaxDuration = Animators_GetAnimatorsMaxDuration(SelectedObjects[ButtonStatu_Index].BtnAnimatorNodes, SelectedObjects[ButtonStatu_Index].Button_Animators_GlobalDuration);

                    #region 最大耗时     
                    Editor_XHud_GUI.StatuDisplayer_text(dutation, 12, new Vector2(0, 7), "最大耗时<color=#909090>（速率倍增）</color>", 12, SelectedObjects[ButtonStatu_Index].AnimatorsMaxDuration.ToString() + "秒", XHud_Dashboard.Theme_Primary, 11);
                    #endregion

                    #region 最大耗时        
                    Editor_XHud_GUI.StatuDisplayer_text(dutation, 12, new Vector2(0, 7), "最大耗时<color=#909090>（XHUD倍增）</color>", 12, (mgr.DurationMultiply * SelectedObjects[ButtonStatu_Index].AnimatorsMaxDuration).ToString() + "秒", XHud_Dashboard.Theme_Primary, 11);
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
                if (BtnAnimatorNodes.arraySize <= 0)
                {
                    Editor_XHud_GUI.Gui_Layout_Labelfield("暂无统计数据", HudFilled.无, HudColor.无, Editor_XHud_GUI.GetColor(HudColor.阴影灰), TextAnchor.MiddleCenter);
                }
                else
                {
                    #region 组件数量 - 动画器
                    if (BtnAnimatorNodes.arraySize > 0)
                    {
                        Editor_XHud_GUI.StatuDisplayer_text(icon_anim, 12, new Vector2(0, 7), "动画器", 12, BtnAnimatorNodes.arraySize.ToString() + " 个", XHud_Dashboard.Theme_Primary, 11);
                    }
                    #endregion
                }
            }
            else
            {
                #region 批量控件
                Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_XHud_GUI.Gui_Layout_Space(10);
                if (Editor_XHud_GUI.Gui_Layout_Button($"{SelectedObjects[ButtonStatistic_Index].name} ( {SelectedObjects[ButtonStatistic_Index].Indicator} )", "", HudFilled.透明, HudColor.无, Color.gray, 20))
                {
                    EditorGUIUtility.PingObject(SelectedObjects[ButtonStatistic_Index]);
                }
                Editor_XHud_GUI.Gui_Layout_FlexSpace();
                if (Editor_XHud_GUI.Gui_Layout_Button(12, "", left_arrow_r, left_arrow_p))
                {
                    if (ButtonStatistic_Index <= 0)
                    {
                        ButtonStatistic_Index = SelectedObjects.Length - 1;
                    }
                    else
                    {
                        ButtonStatistic_Index--;
                    }
                    EditorGUIUtility.PingObject(SelectedObjects[ButtonStatistic_Index]);
                }
                Editor_XHud_GUI.Gui_Layout_Space(16);
                if (Editor_XHud_GUI.Gui_Layout_Button(12, "", right_arrow_r, right_arrow_p))
                {
                    if (ButtonStatistic_Index >= SelectedObjects.Length - 1)
                    {
                        ButtonStatistic_Index = 0;
                    }
                    else
                    {
                        ButtonStatistic_Index++;
                    }
                    EditorGUIUtility.PingObject(SelectedObjects[ButtonStatistic_Index]);
                }
                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                #endregion

                if (SelectedObjects[ButtonStatistic_Index].BtnAnimatorNodes.Count <= 0)
                {
                    Editor_XHud_GUI.Gui_Layout_Labelfield("暂无统计数据", HudFilled.无, HudColor.无, Editor_XHud_GUI.GetColor(HudColor.阴影灰), TextAnchor.MiddleCenter);
                }
                else
                {
                    #region 组件数量 - 动画器
                    if (SelectedObjects[ButtonStatistic_Index].BtnAnimatorNodes.Count > 0)
                    {
                        Editor_XHud_GUI.StatuDisplayer_text(icon_anim, 12, new Vector2(0, 7), "动画器", 12, SelectedObjects[ButtonStatistic_Index].BtnAnimatorNodes.Count.ToString() + " 个", XHud_Dashboard.Theme_Primary, 11);
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

            #region 事件/列表
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "事件/列表", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(5);

            #region 选择器动画列表
            if (BtnAnimatorNodes.arraySize > 0)
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

                    AnimatorsIsFold.boolValue = EditorGUILayout.Foldout(AnimatorsIsFold.boolValue, "动画器", true);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    if (AnimatorsIsFold.boolValue)
                        BtnAnimatorList.DoLayoutList();
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                }
            }
            #endregion

            #region 按钮事件
            if (Targets_Selected())
            {
                Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_XHud_GUI.Gui_Layout_Space(10);
                EditorGUILayout.HelpBox("按钮事件不支持多项操作", MessageType.Warning);
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
                    EditorGUILayout.PropertyField(eve_on_Deselect);
                    eve_on_Deselect.serializedObject.ApplyModifiedProperties();
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_Select);
                    eve_on_Select.serializedObject.ApplyModifiedProperties();
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_Click);
                    eve_on_Click.serializedObject.ApplyModifiedProperties();
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_Press);
                    eve_on_Press.serializedObject.ApplyModifiedProperties();
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_Enter);
                    eve_on_Enter.serializedObject.ApplyModifiedProperties();
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_Exit);
                    eve_on_Exit.serializedObject.ApplyModifiedProperties();
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_Release);
                    eve_on_Release.serializedObject.ApplyModifiedProperties();
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_LongPressed);
                    eve_on_LongPressed.serializedObject.ApplyModifiedProperties();
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_LongpressPer);
                    eve_on_LongpressPer.serializedObject.ApplyModifiedProperties();
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
                SerializedProperty sp_col_normal = serializedObject.FindProperty("m_Colors.m_NormalColor");
                SerializedProperty sp_col_highlight = serializedObject.FindProperty("m_Colors.m_HighlightedColor");
                SerializedProperty sp_col_press = serializedObject.FindProperty("m_Colors.m_PressedColor");
                SerializedProperty sp_col_selected = serializedObject.FindProperty("m_Colors.m_SelectedColor");
                SerializedProperty sp_col_disable = serializedObject.FindProperty("m_Colors.m_DisabledColor");
                SerializedProperty sp_col_mul = serializedObject.FindProperty("m_Colors.m_ColorMultiplier");
                SerializedProperty sp_col_dur = serializedObject.FindProperty("m_Colors.m_FadeDuration");
                SerializedProperty sp_Indicator = serializedObject.FindProperty("Indicator");
                SerializedProperty sp_DebugState = serializedObject.FindProperty("DebugState");
                SerializedProperty sp_IsOptionButton = serializedObject.FindProperty("IsOptionButton");
                SerializedProperty sp_ButtonName = serializedObject.FindProperty("ButtonName");
                SerializedProperty sp_LongPress_Threshold = serializedObject.FindProperty("LongPress_Threshold");
                SerializedProperty sp_LongPress_Percent = serializedObject.FindProperty("LongPress_Percent");
                SerializedProperty sp_LongPress_SmoothRewind = serializedObject.FindProperty("LongPress_SmoothRewind");
                SerializedProperty sp_LongPress_UseRewind = serializedObject.FindProperty("LongPress_UseRewind");
                SerializedProperty sp_TextColorSyncFade = serializedObject.FindProperty("TextColorSyncFade");
                SerializedProperty sp_IconColorSyncFade = serializedObject.FindProperty("IconColorSyncFade");

                // 创建右键菜单
                GenericMenu menu = new GenericMenu();

                menu.AddDisabledItem(new GUIContent("脚本参数"));
                if (!Targets_Selected())
                {
                    menu.AddItem(new GUIContent("C (拷贝)"), false, () =>
                    {
                        XHud_ModuleArg_Button hbp = new XHud_ModuleArg_Button();
                        hbp.Color_Normal = sp_col_normal.colorValue;
                        hbp.Color_Highlight = sp_col_highlight.colorValue;
                        hbp.Color_Press = sp_col_press.colorValue;
                        hbp.Color_Select = sp_col_selected.colorValue;
                        hbp.Color_Disable = sp_col_disable.colorValue;
                        hbp.ColorFadeDuration = sp_col_dur.floatValue;
                        hbp.ColorMultiplier = sp_col_mul.floatValue;
                        hbp.Indicator = sp_Indicator.stringValue;
                        hbp.DebugState = sp_DebugState.boolValue;
                        hbp.IsOptionButton = sp_IsOptionButton.boolValue;
                        hbp.ButtonName = sp_ButtonName.stringValue;
                        hbp.LongPress_Threahold = sp_LongPress_Threshold.floatValue;
                        hbp.LongPress_Percent = sp_LongPress_Percent.floatValue;
                        hbp.LongPress_SmoothRewind = sp_LongPress_SmoothRewind.boolValue;
                        hbp.LongPress_UseRewind = sp_LongPress_UseRewind.boolValue;
                        hbp.TextColorSyncFade = sp_TextColorSyncFade.boolValue;
                        hbp.IconColorSyncFade = sp_IconColorSyncFade.boolValue;

                        string json = JsonUtility.ToJson(hbp);
                        GUIUtility.systemCopyBuffer = json;

                        Editor_XHud_GUI.Open(XHud_DialogType.通知, "XHud - 按钮消息", "脚本参数操作", "已将HudButton脚本参数拷贝至剪贴板！", "明白");
                    });
                }
                menu.AddItem(new GUIContent("V (粘贴)"), false, () =>
                {
                    XHud_ModuleArg_Button hbp = JsonUtility.FromJson<XHud_ModuleArg_Button>(GUIUtility.systemCopyBuffer);

                    if (!Targets_Selected())
                    {
                        sp_col_normal.colorValue = hbp.Color_Normal;
                        sp_col_highlight.colorValue = hbp.Color_Highlight;
                        sp_col_press.colorValue = hbp.Color_Press;
                        sp_col_selected.colorValue = hbp.Color_Select;
                        sp_col_disable.colorValue = hbp.Color_Disable;
                        sp_col_dur.floatValue = hbp.ColorFadeDuration;
                        sp_col_mul.floatValue = hbp.ColorMultiplier;
                        sp_Indicator.stringValue = hbp.Indicator;
                        sp_DebugState.boolValue = hbp.DebugState;
                        sp_IsOptionButton.boolValue = hbp.IsOptionButton;
                        sp_ButtonName.stringValue = hbp.ButtonName;
                        sp_LongPress_Threshold.floatValue = hbp.LongPress_Threahold;
                        sp_LongPress_Percent.floatValue = hbp.LongPress_Percent;
                        sp_LongPress_SmoothRewind.boolValue = hbp.LongPress_SmoothRewind;
                        sp_LongPress_UseRewind.boolValue = hbp.LongPress_UseRewind;
                        sp_TextColorSyncFade.boolValue = hbp.TextColorSyncFade;
                        sp_IconColorSyncFade.boolValue = hbp.IconColorSyncFade;

                        sp_col_normal.serializedObject.ApplyModifiedProperties();
                        sp_col_highlight.serializedObject.ApplyModifiedProperties();
                        sp_col_press.serializedObject.ApplyModifiedProperties();
                        sp_col_selected.serializedObject.ApplyModifiedProperties();
                        sp_col_disable.serializedObject.ApplyModifiedProperties();
                        sp_col_dur.serializedObject.ApplyModifiedProperties();
                        sp_col_mul.serializedObject.ApplyModifiedProperties();
                        sp_Indicator.serializedObject.ApplyModifiedProperties();
                        sp_DebugState.serializedObject.ApplyModifiedProperties();
                        sp_IsOptionButton.serializedObject.ApplyModifiedProperties();
                        sp_ButtonName.serializedObject.ApplyModifiedProperties();
                        sp_LongPress_Threshold.serializedObject.ApplyModifiedProperties();
                        sp_LongPress_Percent.serializedObject.ApplyModifiedProperties();
                        sp_LongPress_SmoothRewind.serializedObject.ApplyModifiedProperties();
                        sp_LongPress_UseRewind.serializedObject.ApplyModifiedProperties();
                        sp_TextColorSyncFade.serializedObject.ApplyModifiedProperties();
                        sp_IconColorSyncFade.serializedObject.ApplyModifiedProperties();
                    }
                    else
                    {
                        for (int i = 0; i < SelectedObjects.Length; i++)
                        {
                            SerializedObject so_btn = new SerializedObject(SelectedObjects[i]);

                            SerializedProperty c_sp_col_normal = so_btn.FindProperty("m_Colors.m_NormalColor");
                            SerializedProperty c_sp_col_highlight = so_btn.FindProperty("m_Colors.m_HighlightedColor");
                            SerializedProperty c_sp_col_press = so_btn.FindProperty("m_Colors.m_PressedColor");
                            SerializedProperty c_sp_col_selected = so_btn.FindProperty("m_Colors.m_SelectedColor");
                            SerializedProperty c_sp_col_disable = so_btn.FindProperty("m_Colors.m_DisabledColor");
                            SerializedProperty c_sp_col_mul = so_btn.FindProperty("m_Colors.m_ColorMultiplier");
                            SerializedProperty c_sp_col_dur = so_btn.FindProperty("m_Colors.m_FadeDuration");
                            SerializedProperty c_sp_Indicator = so_btn.FindProperty("Indicator");
                            SerializedProperty c_sp_DebugState = so_btn.FindProperty("DebugState");
                            SerializedProperty c_sp_IsOptionButton = so_btn.FindProperty("IsOptionButton");
                            SerializedProperty c_sp_ButtonName = so_btn.FindProperty("ButtonName");
                            SerializedProperty c_sp_LongPress_Threshold = so_btn.FindProperty("LongPress_Threshold");
                            SerializedProperty c_sp_LongPress_Percent = so_btn.FindProperty("LongPress_Percent");
                            SerializedProperty c_sp_LongPress_SmoothRewind = so_btn.FindProperty("LongPress_SmoothRewind");
                            SerializedProperty c_sp_LongPress_UseRewind = so_btn.FindProperty("LongPress_UseRewind");
                            SerializedProperty c_sp_TextColorSyncFade = so_btn.FindProperty("TextColorSyncFade");
                            SerializedProperty c_sp_IconColorSyncFade = so_btn.FindProperty("IconColorSyncFade");

                            c_sp_col_normal.colorValue = hbp.Color_Normal;
                            c_sp_col_highlight.colorValue = hbp.Color_Highlight;
                            c_sp_col_press.colorValue = hbp.Color_Press;
                            c_sp_col_selected.colorValue = hbp.Color_Select;
                            c_sp_col_disable.colorValue = hbp.Color_Disable;
                            c_sp_col_dur.floatValue = hbp.ColorFadeDuration;
                            c_sp_col_mul.floatValue = hbp.ColorMultiplier;
                            c_sp_Indicator.stringValue = hbp.Indicator;
                            c_sp_DebugState.boolValue = hbp.DebugState;
                            c_sp_IsOptionButton.boolValue = hbp.IsOptionButton;
                            c_sp_ButtonName.stringValue = hbp.ButtonName;
                            c_sp_LongPress_Threshold.floatValue = hbp.LongPress_Threahold;
                            c_sp_LongPress_Percent.floatValue = hbp.LongPress_Percent;
                            c_sp_LongPress_SmoothRewind.boolValue = hbp.LongPress_SmoothRewind;
                            c_sp_LongPress_UseRewind.boolValue = hbp.LongPress_UseRewind;
                            c_sp_TextColorSyncFade.boolValue = hbp.TextColorSyncFade;
                            c_sp_IconColorSyncFade.boolValue = hbp.IconColorSyncFade;

                            c_sp_col_normal.serializedObject.ApplyModifiedProperties();
                            c_sp_col_highlight.serializedObject.ApplyModifiedProperties();
                            c_sp_col_press.serializedObject.ApplyModifiedProperties();
                            c_sp_col_selected.serializedObject.ApplyModifiedProperties();
                            c_sp_col_disable.serializedObject.ApplyModifiedProperties();
                            c_sp_col_dur.serializedObject.ApplyModifiedProperties();
                            c_sp_col_mul.serializedObject.ApplyModifiedProperties();
                            c_sp_Indicator.serializedObject.ApplyModifiedProperties();
                            c_sp_DebugState.serializedObject.ApplyModifiedProperties();
                            c_sp_IsOptionButton.serializedObject.ApplyModifiedProperties();
                            c_sp_ButtonName.serializedObject.ApplyModifiedProperties();
                            c_sp_LongPress_Threshold.serializedObject.ApplyModifiedProperties();
                            c_sp_LongPress_Percent.serializedObject.ApplyModifiedProperties();
                            c_sp_LongPress_SmoothRewind.serializedObject.ApplyModifiedProperties();
                            c_sp_LongPress_UseRewind.serializedObject.ApplyModifiedProperties();
                            c_sp_TextColorSyncFade.serializedObject.ApplyModifiedProperties();
                            c_sp_IconColorSyncFade.serializedObject.ApplyModifiedProperties();

                        }
                    }
                    Editor_XHud_GUI.Open(XHud_DialogType.修改, "XHud - 按钮消息", "脚本参数操作", "已将剪贴板的参数粘贴至当前HudButton脚本！", "明白");
                });
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
                            XHud_Module_Button btn = SelectedObjects[i];
                            btn.Indicator = btn.gameObject.name;
                            EditorUtility.SetDirty(btn);
                        }
                    }
                    else
                    {
                        Indicator.stringValue = BaseScript.gameObject.name;
                        Indicator.serializedObject.ApplyModifiedProperties();
                        Editor_XHud_GUI.Open(XHud_DialogType.修改, "XHud - 按钮消息", "标识自身", "将标识名称参数更改为物体的名称！", "明白");
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
                menu.AddSeparator("");
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
                menu.ShowAsContext();
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

            CalculateAnimatorMaxDuration(BaseScript.BtnAnimatorNodes, Button_Animators_GlobalDuration.floatValue);

            serializedObject.ApplyModifiedProperties();
        }

        #region 辅助
        /// <summary>
        /// 检查是否存在无效的动画器
        /// </summary>
        private void CheckAnimatorsValid()
        {
            for (int i = 0; i < BtnAnimatorNodes.arraySize; i++)
            {
                SerializedProperty sp_animotrNode = BtnAnimatorNodes.GetArrayElementAtIndex(i);
                SerializedProperty sp_animator = sp_animotrNode.FindPropertyRelative("Animator");

                if (sp_animator.objectReferenceValue == null)
                {
                    BtnAnimatorNodes.DeleteArrayElementAtIndex(i);
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
                    SerializedProperty sp_anim_isfold = so_ele.FindProperty("AnimatorsIsFold");
                    SerializedProperty sp_event_isfold = so_ele.FindProperty("EventIsFold");
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
                    AnimatorsIsFold.boolValue = state;
                    EventIsFold.boolValue = state;
                    EventIsFold.serializedObject.ApplyModifiedProperties();
                    AnimatorsIsFold.serializedObject.ApplyModifiedProperties();
                }
            }
        }
        /// <summary>
        /// 计算最大耗时
        /// </summary>
        /// <param name="list"></param>
        /// <param name="globaldur"></param>
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
            for (int i = 0; i < BtnAnimatorNodes.arraySize; i++)
            {
                SerializedProperty sp_animator = BtnAnimatorNodes.GetArrayElementAtIndex(i).FindPropertyRelative("Animator");
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
        /// 获取序列化字段
        /// </summary>
        private void GetSerializeFields()
        {
            Indicator = serializedObject.FindProperty("Indicator");
            DebugState = serializedObject.FindProperty("DebugState");
            IsOptionButton = serializedObject.FindProperty("IsOptionButton");
            ClickDelayTimeThreadhold = serializedObject.FindProperty("ClickDelayTimeThreadhold");
            eve_on_Deselect = serializedObject.FindProperty("eve_on_Deselect");
            eve_on_Select = serializedObject.FindProperty("eve_on_Select");
            eve_on_Click = serializedObject.FindProperty("eve_on_Click");
            eve_on_Press = serializedObject.FindProperty("eve_on_Press");
            eve_on_Enter = serializedObject.FindProperty("eve_on_Enter");
            eve_on_Exit = serializedObject.FindProperty("eve_on_Exit");
            eve_on_Release = serializedObject.FindProperty("eve_on_Release");
            eve_on_LongPressed = serializedObject.FindProperty("eve_on_Longpressed");
            eve_on_LongpressPer = serializedObject.FindProperty("eve_on_LongpressPer");
            LongPress_Threshold = serializedObject.FindProperty("LongPress_Threshold");
            LongPress_Percent = serializedObject.FindProperty("LongPress_Percent");
            ButtonName = serializedObject.FindProperty("ButtonName");
            ButtonText = serializedObject.FindProperty("ButtonText");
            ButtonTmpText = serializedObject.FindProperty("ButtonTmpText");
            LongPress_SmoothRewind = serializedObject.FindProperty("LongPress_SmoothRewind");
            LongPress_UseRewind = serializedObject.FindProperty("LongPress_UseRewind");
            TextColorSyncFade = serializedObject.FindProperty("TextColorSyncFade");
            IconColorSyncFade = serializedObject.FindProperty("IconColorSyncFade");
            BtnAnimatorNodes = serializedObject.FindProperty("BtnAnimatorNodes");
            IconImage = serializedObject.FindProperty("IconImage");
            Button_Animators_GlobalDuration = serializedObject.FindProperty("Button_Animators_GlobalDuration");
            AnimatorsMaxDuration = serializedObject.FindProperty("AnimatorsMaxDuration");
            AnimateState = serializedObject.FindProperty("AnimateState");
            AnimatorsIsFold = serializedObject.FindProperty("AnimatorsIsFold");
            EventIsFold = serializedObject.FindProperty("EventIsFold");
            ToggleOriginalIsFold = serializedObject.FindProperty("ToggleOriginalIsFold");
            LongPress_Step = serializedObject.FindProperty("LongPress_Step");
            ButtonActionTiming = serializedObject.FindProperty("ButtonActionTiming");
            HudButtonState_Clicked = serializedObject.FindProperty("HudButtonState_Clicked");
            HudButtonState_Selector = serializedObject.FindProperty("HudButtonState_Selector");
            HudButtonState_Holder = serializedObject.FindProperty("HudButtonState_Holder");
            HudButtonState_Pressed = serializedObject.FindProperty("HudButtonState_Pressed");
            HudButtonState_LongPressed = serializedObject.FindProperty("HudButtonState_LongPressed");
            AutoStopPreview = serializedObject.FindProperty("AutoStopPreview");
            BgColorSyncFade = serializedObject.FindProperty("BgColorSyncFade");
            BgImage = serializedObject.FindProperty("BgImage");
        }
        #endregion

        #region Animator 动画预览

        /// <summary>
        /// 预览动画
        /// </summary>
        private void Preview_Animator_Play()
        {
            if (BtnAnimatorNodes.arraySize <= 0)
                return;
            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

            Preivew_Animator_PlayingState = true;
            AnimateState.enumValueIndex = 1;
            AnimateState.serializedObject.ApplyModifiedProperties();

            ///---用来存放调用动画器的动画效果的延迟时间
            List<float> delaytimes = new List<float>();

            ///---动画逻辑
            for (int i = 0; i < BaseScript.BtnAnimatorNodes.Count; i++)
            {
                ElementNode_Animator node = BaseScript.BtnAnimatorNodes[i];
                ///---判断动画器是否为有效
                if (node.Animator.AnimateTweenNodes == null || node.Animator.AnimateTweenNodes.Count <= 0)
                    continue;

                ///---循环第 i 个动画器节点的第 s 个动画效果是否开启
                for (int s = 0; s < node.Animator.AnimateTweenNodes.Count; s++)
                {
                    TweenNode twnnode = node.Animator.AnimateTweenNodes[s];

                    if (twnnode.ActivateOnlyToEnd)
                        continue;

                    ///---如果动画效果开启
                    if (twnnode.Enabled)
                    {
                        if (mgr == null)
                            Preivew_Animator_TweenList.Add(node.Animator.Tweener_Play(twnnode, 1 * Button_Animators_GlobalDuration.floatValue * node.Animator.Animator_GlobalDuration));
                        else
                            Preivew_Animator_TweenList.Add(node.Animator.Tweener_Play(twnnode, mgr.DurationMultiply * Button_Animators_GlobalDuration.floatValue * node.Animator.Animator_GlobalDuration));
                        delaytimes.Add(node.DelayTime);
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            ///---预备动画器
            for (int i = 0; i < BaseScript.BtnAnimatorNodes.Count; i++)
            {
                ElementNode_Animator node = BaseScript.BtnAnimatorNodes[i];
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

            ElementNode_Animator node = BaseScript.BtnAnimatorNodes[index];
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
                if (BaseScript.BtnAnimatorNodes != null && BaseScript.BtnAnimatorNodes.Count > 0)
                {
                    for (int c = 0; c < BaseScript.BtnAnimatorNodes.Count; c++)
                    {
                        XHud_Module_Animator animator = BaseScript.BtnAnimatorNodes[c].Animator;
                        List<TweenNode> TwnNodes = animator.AnimateTweenNodes;

                        if (TwnNodes != null && TwnNodes.Count > 0)
                        {
                            for (int i = 0; i < TwnNodes.Count; i++)
                            {
                                TweenNode twnnode = TwnNodes[i];
                                twnnode.Progress = 0;
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
            while (BaseScript.AnimateState == XHudElementAnimateState.Animating)
            {
                for (int w = 0; w < BaseScript.BtnAnimatorNodes.Count; w++)
                {
                    ElementNode_Animator a_node = BaseScript.BtnAnimatorNodes[w];
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
                    SerializedProperty sp_nodes = so_ele.FindProperty("BtnAnimatorNodes");
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
                SerializedProperty sp_nodes = serializedObject.FindProperty("BtnAnimatorNodes");
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
                    XHud_Module_Button btn = SelectedObjects[i];
                    if (btn.BtnAnimatorNodes.Count > 0)
                    {
                        for (int c = 0; c < btn.BtnAnimatorNodes.Count; c++)
                        {
                            XHud_GUI_Dialog_ListDatas dataitem = new XHud_GUI_Dialog_ListDatas();

                            dataitem.Title = $"{btn.name} ( {btn.Indicator} )";
                            dataitem.SubTitle = $"扫描到动画器";
                            dataitem.Message = $"{btn.BtnAnimatorNodes[c].Animator.name} ( {btn.BtnAnimatorNodes[c].Animator.Indicator} )";

                            Datas.Add(dataitem);
                        }
                    }
                }
                Editor_XHud_GUI.Open(Datas.ToArray(), XHud_DialogType.确认, "XHud - 按钮消息", "批量扫描动画器组件", "以下是批量扫描到的所有动画器组件列表，请您检查核对：", "明白");
            }
            else
            {
                if (BaseScript.BtnAnimatorNodes.Count > 0)
                {
                    for (int i = 0; i < BaseScript.BtnAnimatorNodes.Count; i++)
                    {
                        XHud_GUI_Dialog_ListDatas dataitem = new XHud_GUI_Dialog_ListDatas();

                        dataitem.Title = $"{BaseScript.name} ( {BaseScript.Indicator} )";
                        dataitem.SubTitle = $"扫描到动画器";
                        dataitem.Message = $"{BaseScript.BtnAnimatorNodes[i].Animator.name} ( {BaseScript.BtnAnimatorNodes[i].Animator.Indicator} )";
                        Datas.Add(dataitem);
                    }
                    Editor_XHud_GUI.Open(Datas.ToArray(), XHud_DialogType.确认, "XHud - 按钮消息", "扫描动画器组件", "以下是扫描到的所有动画器组件列表，请您检查核对：", "明白");
                }
                else
                {
                    Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 按钮消息", "扫描动画器组件", "未扫描到任何动画器组件！", "明白");
                }
            }
        }

        #endregion
    }
}