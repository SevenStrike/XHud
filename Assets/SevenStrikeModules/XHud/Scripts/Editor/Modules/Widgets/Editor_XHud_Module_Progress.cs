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
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    public class XHud_ModuleArg_Progress
    {
        public string Indicator;
        public bool DebugState;
        public string con_title;
        public string con_subtitle;
        public int ProgressPrecision;
        public string ProgressUnit;
        public float ProgressTweenSpeedMultiply;
        public bool EaseMotion;
        public bool ProgressIsFinished;
        public float ProgressValue;
        public float sm_ProgressValue;
        public float ProgressValueDuration;
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(XHud_Module_Progress), true)]
    public class Editor_XHud_Module_Progress : Editor
    {
        #region 组件 / 列表
        private XHud_Module_Progress BaseScript;
        private ReorderableList PrimitivesTweenList;
        private XHud_Manager HudManager;
        #endregion

        #region 序列化属性
        private SerializedProperty sp_Indicator, sp_debugstate, PrimitivesIsFold, eve_on_ValueChanged, eve_on_ValueStart, eve_on_ValueEnd, pro_Text_Title, pro_TmpText_Title, pro_Text_Subtitle, pro_TmpText_Subtitle, pro_Text_Percent, pro_TmpText_Percent, pro_Bg, pro_Fore, ProgressPrecision, ProgressUnit, pro_Icon, con_title, con_subtitle, ProgressValue, RectTransform, pro_Handle, ProgressValueDuration, PrimitiveControllerNodes, EventIsFold, AnimateState, PrimitivesTweenMaxDuration, PrimitivesTweenGlobalDuration, LerpMotion, AutoStopPreview, Display_ProgressRect_Fore, Display_ProgressRect_Bg, Display_ProgressRect_Handle, Display_Icon, Display_Title, Display_SubTitle, Display_Value;
        #endregion

        #region Preview - Tween
        private bool Preivew_Tween_PlayingState;
        #endregion

        #region 图标                                                                                                                                     
        private Texture2D icon_main, find_r, find_p, play_r, play_p, stop_r, stop_p, clear_r, clear_p, icon_button, animstate, dutation, longpressmarker, left_arrow_r, left_arrow_p, right_arrow_r, right_arrow_p, icon_anim;
        #endregion

        #region 批量模式查看索引
        private int ProgressStatu_Index;
        private int ProgressStatistic_Index;
        #endregion

        #region 选项文字
        string[] stroptions_debug = new string[2] { "关闭", "调试" }, stroptions_smooth = new string[2] { "闪现", "平滑" }, stroptions_enabled = new string[2] { "关闭", "开启" };
        #endregion

        #region 批量化操作
        XHud_Module_Progress[] SelectedObjects;

        private void Targets_Get()
        {
            if (targets.Length > 1)
            {
                SelectedObjects = new XHud_Module_Progress[targets.Length];
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    var t = targets[i];
                    SelectedObjects[i] = (XHud_Module_Progress)t;
                }
            }
            else
            {
                SelectedObjects = new XHud_Module_Progress[targets.Length];
                SelectedObjects[0] = (XHud_Module_Progress)target;
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

        void OnEnable()
        {
            HudManager = XHud_Dashboard.HudManagerGet();

            BaseScript = (XHud_Module_Progress)target;

            RectTransform = serializedObject.FindProperty("RectTransform");
            if (RectTransform.objectReferenceValue == null)
            {
                RectTransform.objectReferenceValue = BaseScript.GetComponent<RectTransform>();
                RectTransform.serializedObject.ApplyModifiedProperties();
            }

            // 获取序列化属性
            GetSerializeFields();

            #region 获取图标
            icon_main = Editor_XHud_GUI.GetIcon("Icons_XHud_Progress/icon_main");
            find_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Progress/find_r");
            find_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Progress/find_p");
            play_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Progress/play_r");
            play_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Progress/play_p");
            stop_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Progress/stop_r");
            stop_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Progress/stop_p");
            clear_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Progress/clear_r");
            clear_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Progress/clear_p");
            icon_button = Editor_XHud_GUI.GetIcon("Icons_XHud_Progress/icon_button");
            animstate = Editor_XHud_GUI.GetIcon("Icons_XHud_Progress/animstate");
            dutation = Editor_XHud_GUI.GetIcon("Icons_XHud_Progress/dutation");
            longpressmarker = Editor_XHud_GUI.GetIcon("Icons_XHud_Progress/longpressmarker");
            left_arrow_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Progress/left_arrow_r");
            left_arrow_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Progress/left_arrow_p");
            right_arrow_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Progress/right_arrow_r");
            right_arrow_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Progress/right_arrow_p");
            icon_anim = Editor_XHud_GUI.GetIcon("Icons_XHud_Progress/icon_anim");
            #endregion

            LineHeight = EditorGUIUtility.singleLineHeight;
            Vector2 ButtonSize = new Vector2(18, 18);

            #region ReorderableList - PrimitivesTween
            PrimitivesTweenList = new ReorderableList(serializedObject, PrimitiveControllerNodes)
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
                    SerializedProperty sp_node_con = PrimitiveControllerNodes.GetArrayElementAtIndex(index).FindPropertyRelative("Controller");
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
                        #region 速率   
                        SerializedObject so_anim = new SerializedObject(sp_con.pt_Tween);
                        so_anim.Update();

                        SerializedProperty sp_glodur = so_anim.FindProperty("GlobalDuration");
                        SerializedProperty sp_maxdur = so_anim.FindProperty("MaxTimerWithGlobalDuration");

                        Editor_XHud_GUI.Gui_Property_Field(new Rect(rect.width - 50, rect.y + 4, 30, 19), "G", sp_glodur, 10, 40, LineHeight, 15);

                        Editor_XHud_GUI.Gui_Labelfield_Thin(new Rect(rect.width - 80, rect.y + 4, 30, 19), $"{sp_maxdur.floatValue.ToString()} s", HudFilled.无, HudColor.无, XHud_Dashboard.Theme_Primary, TextAnchor.MiddleCenter, Vector2.zero, 11);
                        #endregion
                        #endregion
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

        void OnDisable()
        {
            if (!Application.isPlaying)
            {

            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (string.IsNullOrEmpty(BaseScript.Indicator))
                Editor_XHud_GUI.Gui_Layout_Banner(icon_main, HudFilled.实体, HudColor.深空灰, "Hud - 进度条", Color.white);
            else
                Editor_XHud_GUI.Gui_Layout_Banner(icon_main, HudFilled.实体, HudColor.深空灰, "Hud - 进度条-> ( " + BaseScript.Indicator + " )", Color.white);

            #region 快捷功能
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "快捷功能", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            #region 扫描动画组件
            if (Editor_XHud_GUI.Gui_Layout_Button(14, "扫描动画组件", find_r, find_p))
            {
                GetPrimitivesTween();
                GetPrimitivesTweenResults();
                return;
            }
            #endregion

            #region 预览动画
            if (!Targets_Selected() && PrimitiveControllerNodes.arraySize > 0)
            {
                GUILayout.FlexibleSpace();

                if (!Preivew_Tween_PlayingState)
                {
                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "播放光标动画预览", play_r, play_p))
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

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_Progress>("状态调试", stroptions_debug, ref sp_debugstate, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_Progress>("自动停止预览", stroptions_enabled, ref AutoStopPreview, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_Progress>("平滑模式", stroptions_smooth, ref LerpMotion, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 参数
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "参数", XHud_Dashboard.Theme_Primary);

            EditorGUI.BeginChangeCheck();
            Editor_XHud_GUI.Gui_Layout_Property_Field("标识名称", sp_Indicator, 100);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("标题名称", con_title, 100);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("副标题名称", con_subtitle, 100);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("进度值精度", ProgressPrecision, 100);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("后缀单位", ProgressUnit, 100);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("进度值", ProgressValue, 100);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("进度条平滑速率", ProgressValueDuration, 100);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("速率倍增", PrimitivesTweenGlobalDuration, 100);

            if (EditorGUI.EndChangeCheck())
            {
                BaseScript.pro_TitleSet(con_title.stringValue);
                BaseScript.pro_SubTitleSet(con_subtitle.stringValue);
                UpdateProgress();
            }
            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 显示组件
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "内容显示组件", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            Editor_XHud_GUI.Gui_Layout_Property_Field("标题 (Text)", pro_Text_Title, 100);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("标题 (TmpText)", pro_TmpText_Title, 100);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("副标题 (Text)", pro_Text_Subtitle, 100);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("副标题 (TmpText)", pro_TmpText_Subtitle, 100);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("百分比 (Text)", pro_Text_Percent, 100);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("百分比 (TmpText)", pro_TmpText_Percent, 100);

            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 进度条组件
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "进度条组件", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            Editor_XHud_GUI.Gui_Layout_Property_Field("前景", pro_Fore, 100);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("背景", pro_Bg, 100);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("图标", pro_Icon, 100);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("控制柄", pro_Handle, 100);

            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 可视化开关
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "组件可视化开关", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(5);

            EditorGUI.BeginChangeCheck();
            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_Progress>("前景", stroptions_enabled, ref Display_ProgressRect_Fore, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_Progress>("背景", stroptions_enabled, ref Display_ProgressRect_Bg, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_Progress>("标记", stroptions_enabled, ref Display_ProgressRect_Handle, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_Progress>("图标", stroptions_enabled, ref Display_Icon, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_Progress>("标题", stroptions_enabled, ref Display_Title, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_Progress>("副标题", stroptions_enabled, ref Display_SubTitle, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_Progress>("进度值", stroptions_enabled, ref Display_Value, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

            if (EditorGUI.EndChangeCheck())
            {
                if (Targets_Selected())
                {
                    for (int i = 0; i < SelectedObjects.Length; i++)
                    {
                        SelectedObjects[i].pro_SetDisplay_ProgressRect_Fore(SelectedObjects[i].Display_ProgressRect_Fore);
                        SelectedObjects[i].pro_SetDisplay_ProgressRect_Bg(SelectedObjects[i].Display_ProgressRect_Bg);
                        SelectedObjects[i].pro_SetDisplay_ProgressRect_Handle(SelectedObjects[i].Display_ProgressRect_Handle);
                        SelectedObjects[i].pro_SetDisplay_Icon(SelectedObjects[i].Display_Icon);
                        SelectedObjects[i].pro_SetDisplay_Title(SelectedObjects[i].Display_Title);
                        SelectedObjects[i].pro_SetDisplay_SubTitle(SelectedObjects[i].Display_SubTitle);
                        SelectedObjects[i].pro_SetDisplay_Value(SelectedObjects[i].Display_Value);
                    }
                }
                else
                {
                    BaseScript.pro_SetDisplay_ProgressRect_Fore(BaseScript.Display_ProgressRect_Fore);
                    BaseScript.pro_SetDisplay_ProgressRect_Bg(BaseScript.Display_ProgressRect_Bg);
                    BaseScript.pro_SetDisplay_ProgressRect_Handle(BaseScript.Display_ProgressRect_Handle);
                    BaseScript.pro_SetDisplay_Icon(BaseScript.Display_Icon);
                    BaseScript.pro_SetDisplay_Title(BaseScript.Display_Title);
                    BaseScript.pro_SetDisplay_SubTitle(BaseScript.Display_SubTitle);
                    BaseScript.pro_SetDisplay_Value(BaseScript.Display_Value);
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
                #region 进度状态
                Editor_XHud_GUI.StatuDisplayer_text(icon_button, 12, new Vector2(0, 7), "进度状态", 12, ProgressValue.floatValue.ToString(), XHud_Dashboard.Theme_Primary, 11);
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

                Editor_XHud_GUI.Gui_Layout_Space(10);
            }
            else
            {
                #region 批量控件
                Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_XHud_GUI.Gui_Layout_Space(10);
                if (Editor_XHud_GUI.Gui_Layout_Button($"{SelectedObjects[ProgressStatu_Index].name} ( {SelectedObjects[ProgressStatu_Index].Indicator} )", "", HudFilled.透明, HudColor.无, Color.gray, 20))
                {
                    EditorGUIUtility.PingObject(SelectedObjects[ProgressStatu_Index]);
                }
                Editor_XHud_GUI.Gui_Layout_FlexSpace();
                if (Editor_XHud_GUI.Gui_Layout_Button(12, "", left_arrow_r, left_arrow_p))
                {
                    if (ProgressStatu_Index <= 0)
                    {
                        ProgressStatu_Index = SelectedObjects.Length - 1;
                    }
                    else
                    {
                        ProgressStatu_Index--;
                    }
                    EditorGUIUtility.PingObject(SelectedObjects[ProgressStatu_Index]);
                }
                Editor_XHud_GUI.Gui_Layout_Space(16);
                if (Editor_XHud_GUI.Gui_Layout_Button(12, "", right_arrow_r, right_arrow_p))
                {
                    if (ProgressStatu_Index >= SelectedObjects.Length - 1)
                    {
                        ProgressStatu_Index = 0;
                    }
                    else
                    {
                        ProgressStatu_Index++;
                    }
                    EditorGUIUtility.PingObject(SelectedObjects[ProgressStatu_Index]);
                }
                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                #endregion

                #region 进度条状态
                Editor_XHud_GUI.StatuDisplayer_text(icon_button, 12, new Vector2(0, 7), "进度状态", 12, SelectedObjects[ProgressStatu_Index].ProgressValue.ToString(), XHud_Dashboard.Theme_Primary, 11);
                #endregion

                #region 动画相关
                if (SelectedObjects[ProgressStatu_Index].PrimitiveControllerNodes != null && SelectedObjects[ProgressStatu_Index].PrimitiveControllerNodes.Count > 0)
                {
                    #region 动画状态     
                    Editor_XHud_GUI.StatuDisplayer_text(animstate, 12, new Vector2(0, 7), "动画状态", 12, SelectedObjects[ProgressStatu_Index].AnimateState == XHudElementAnimateState.Animating ? "动画中" : "静止状态", SelectedObjects[ProgressStatu_Index].AnimateState == XHudElementAnimateState.Animating ? XHud_Dashboard.Theme_Primary : Color.gray, 11);
                    #endregion

                    SelectedObjects[ProgressStatu_Index].PrimitivesTweenMaxDuration = PrimitiveTweens_MaxDuration_Get(SelectedObjects[ProgressStatu_Index].PrimitiveControllerNodes, SelectedObjects[ProgressStatu_Index].PrimitivesTweenGlobalDuration);

                    #region 最大耗时     
                    Editor_XHud_GUI.StatuDisplayer_text(dutation, 12, new Vector2(0, 7), "最大耗时<color=#909090>（速率倍增）</color>", 12, SelectedObjects[ProgressStatu_Index].PrimitivesTweenMaxDuration.ToString() + "秒", XHud_Dashboard.Theme_Primary, 11);
                    #endregion

                    #region 最大耗时        
                    Editor_XHud_GUI.StatuDisplayer_text(dutation, 12, new Vector2(0, 7), "最大耗时<color=#909090>（XHUD倍增）</color>", 12, (HudManager.DurationMultiply * SelectedObjects[ProgressStatu_Index].PrimitivesTweenMaxDuration).ToString() + "秒", XHud_Dashboard.Theme_Primary, 11);
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
                }
            }
            else
            {
                #region 批量控件
                Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_XHud_GUI.Gui_Layout_Space(10);
                if (Editor_XHud_GUI.Gui_Layout_Button($"{SelectedObjects[ProgressStatistic_Index].name} ( {SelectedObjects[ProgressStatistic_Index].Indicator} )", "", HudFilled.透明, HudColor.无, Color.gray, 20))
                {
                    EditorGUIUtility.PingObject(SelectedObjects[ProgressStatistic_Index]);
                }
                Editor_XHud_GUI.Gui_Layout_FlexSpace();
                if (Editor_XHud_GUI.Gui_Layout_Button(12, "", left_arrow_r, left_arrow_p))
                {
                    if (ProgressStatistic_Index <= 0)
                    {
                        ProgressStatistic_Index = SelectedObjects.Length - 1;
                    }
                    else
                    {
                        ProgressStatistic_Index--;
                    }
                    EditorGUIUtility.PingObject(SelectedObjects[ProgressStatistic_Index]);
                }
                Editor_XHud_GUI.Gui_Layout_Space(16);
                if (Editor_XHud_GUI.Gui_Layout_Button(12, "", right_arrow_r, right_arrow_p))
                {
                    if (ProgressStatistic_Index >= SelectedObjects.Length - 1)
                    {
                        ProgressStatistic_Index = 0;
                    }
                    else
                    {
                        ProgressStatistic_Index++;
                    }
                    EditorGUIUtility.PingObject(SelectedObjects[ProgressStatistic_Index]);
                }
                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                #endregion

                if (SelectedObjects[ProgressStatistic_Index].PrimitiveControllerNodes.Count <= 0)
                {
                    Editor_XHud_GUI.Gui_Layout_Labelfield("暂无统计数据", HudFilled.无, HudColor.无, Editor_XHud_GUI.GetColor(HudColor.阴影灰), TextAnchor.MiddleCenter);
                }
                else
                {
                    #region 组件数量 - 图元动画器
                    if (SelectedObjects[ProgressStatistic_Index].PrimitiveControllerNodes.Count > 0)
                    {
                        Editor_XHud_GUI.StatuDisplayer_text(icon_anim, 12, new Vector2(0, 7), "图元动画器", 12, SelectedObjects[ProgressStatistic_Index].PrimitiveControllerNodes.Count.ToString() + " 个", XHud_Dashboard.Theme_Primary, 11);
                    }
                    #endregion
                }
            }

            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 检查是否有无效的图元动画器
            CheckTweensValid();
            #endregion

            #region 事件/列表
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "事件/列表", XHud_Dashboard.Theme_Primary);

            #region 图元动画器列表
            if (PrimitiveControllerNodes.arraySize > 0)
            {
                if (Targets_Selected())
                {
                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(10);
                    EditorGUILayout.HelpBox("图元动画器列表不支持多项操作", MessageType.Warning);
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
                    {
                        PrimitivesTweenList.DoLayoutList();
                    }
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                }
            }
            #endregion

            #region 事件
            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_XHud_GUI.Gui_Layout_Space(10);
            EventIsFold.boolValue = EditorGUILayout.Foldout(EventIsFold.boolValue, "事件", true);
            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Horizontal_End();

            if (EventIsFold.boolValue)
            {
                Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_XHud_GUI.Gui_Layout_Space(5);
                EditorGUILayout.PropertyField(eve_on_ValueStart);
                eve_on_ValueStart.serializedObject.ApplyModifiedProperties();
                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_XHud_GUI.Gui_Layout_Space(5);
                EditorGUILayout.PropertyField(eve_on_ValueChanged);
                eve_on_ValueChanged.serializedObject.ApplyModifiedProperties();
                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_XHud_GUI.Gui_Layout_Space(5);
                EditorGUILayout.PropertyField(eve_on_ValueEnd);
                eve_on_ValueEnd.serializedObject.ApplyModifiedProperties();
                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Horizontal_End();
            }
            #endregion

            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                SerializedProperty sp_Indicator = serializedObject.FindProperty("Indicator");
                SerializedProperty sp_debugstate = serializedObject.FindProperty("DebugState");
                SerializedProperty sp_con_title = serializedObject.FindProperty("con_title");
                SerializedProperty sp_con_subtitle = serializedObject.FindProperty("con_subtitle");
                SerializedProperty sp_ProgressPrecision = serializedObject.FindProperty("ProgressPrecision");
                SerializedProperty sp_ProgressUnit = serializedObject.FindProperty("ProgressUnit");
                SerializedProperty sp_ProgressTweenSpeedMultiply = serializedObject.FindProperty("ProgressTweenSpeedMultiply");
                SerializedProperty sp_ToggleTweenSpeedMultiply = serializedObject.FindProperty("ToggleTweenSpeedMultiply");
                SerializedProperty sp_LerpMotion = serializedObject.FindProperty("LerpMotion");
                SerializedProperty sp_ProgressIsFinished = serializedObject.FindProperty("ProgressIsFinished");
                SerializedProperty sp_ProgressValue = serializedObject.FindProperty("ProgressValue");
                SerializedProperty sp_sm_ProgressValue = serializedObject.FindProperty("sm_ProgressValue");
                SerializedProperty sp_ProgressValueDuration = serializedObject.FindProperty("ProgressValueDuration");

                // 创建右键菜单
                GenericMenu menu = new GenericMenu();
                menu.AddDisabledItem(new GUIContent("脚本参数"));
                if (!Targets_Selected())
                {
                    menu.AddItem(new GUIContent("C (拷贝)"), false, () =>
                    {
                        XHud_ModuleArg_Progress hpp = new XHud_ModuleArg_Progress();

                        hpp.Indicator = sp_Indicator.stringValue;
                        hpp.DebugState = sp_debugstate.boolValue;
                        hpp.con_title = sp_con_title.stringValue;
                        hpp.ProgressPrecision = sp_ProgressPrecision.intValue;
                        hpp.ProgressUnit = sp_ProgressUnit.stringValue;
                        hpp.ProgressTweenSpeedMultiply = sp_ProgressTweenSpeedMultiply.floatValue;
                        hpp.EaseMotion = sp_LerpMotion.boolValue;
                        hpp.ProgressIsFinished = sp_ProgressIsFinished.boolValue;
                        hpp.ProgressValue = sp_ProgressValue.floatValue;
                        hpp.sm_ProgressValue = sp_sm_ProgressValue.floatValue;
                        hpp.ProgressValueDuration = sp_ProgressValueDuration.floatValue;

                        string json = JsonUtility.ToJson(hpp);
                        GUIUtility.systemCopyBuffer = json;
                    });
                }
                menu.AddItem(new GUIContent("V (粘贴)"), false, () =>
                {
                    XHud_ModuleArg_Progress hpp = JsonUtility.FromJson<XHud_ModuleArg_Progress>(GUIUtility.systemCopyBuffer);

                    sp_Indicator.stringValue = hpp.Indicator;
                    sp_debugstate.boolValue = hpp.DebugState;
                    sp_con_title.stringValue = hpp.con_title;
                    sp_ProgressPrecision.intValue = hpp.ProgressPrecision;
                    sp_ProgressUnit.stringValue = hpp.ProgressUnit;
                    sp_ProgressTweenSpeedMultiply.floatValue = hpp.ProgressTweenSpeedMultiply;
                    sp_LerpMotion.boolValue = hpp.EaseMotion;
                    sp_ProgressIsFinished.boolValue = hpp.ProgressIsFinished;
                    sp_ProgressValue.floatValue = hpp.ProgressValue;
                    sp_sm_ProgressValue.floatValue = hpp.sm_ProgressValue;
                    sp_ProgressValueDuration.floatValue = hpp.ProgressValueDuration;

                    sp_Indicator.serializedObject.ApplyModifiedProperties();
                    sp_debugstate.serializedObject.ApplyModifiedProperties();
                    sp_con_title.serializedObject.ApplyModifiedProperties();
                    sp_ProgressPrecision.serializedObject.ApplyModifiedProperties();
                    sp_ProgressUnit.serializedObject.ApplyModifiedProperties();
                    sp_ProgressTweenSpeedMultiply.serializedObject.ApplyModifiedProperties();
                    sp_LerpMotion.serializedObject.ApplyModifiedProperties();
                    sp_ProgressIsFinished.serializedObject.ApplyModifiedProperties();
                    sp_ProgressValue.serializedObject.ApplyModifiedProperties();
                    sp_sm_ProgressValue.serializedObject.ApplyModifiedProperties();
                    sp_ProgressValueDuration.serializedObject.ApplyModifiedProperties();
                });
                menu.AddSeparator("");
                menu.AddDisabledItem(new GUIContent("图元动画器"));
                menu.AddItem(new GUIContent("X (扫描)"), false, () =>
                {
                    GetPrimitivesTween();
                    GetPrimitivesTweenResults();
                    return;
                });
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("A (自动标识)"), false, () =>
                {
                    if (Targets_Selected())
                    {
                        for (int i = 0; i < SelectedObjects.Length; i++)
                        {
                            XHud_Module_Progress btn = SelectedObjects[i];
                            btn.Indicator = btn.gameObject.name;
                            EditorUtility.SetDirty(btn);
                        }
                    }
                    else
                    {
                        sp_Indicator.stringValue = BaseScript.gameObject.name;
                        sp_Indicator.serializedObject.ApplyModifiedProperties();
                        Editor_XHud_GUI.Open(XHud_DialogType.修改, "XHud - 进度条消息", "标识自身", "将标识名称参数更改为物体的名称！", "明白");
                    }
                });
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

            serializedObject.ApplyModifiedProperties();
        }

        #region 辅助
        private void UpdateProgress()
        {
            if (Targets_Selected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    XHud_Module_Progress pro = SelectedObjects[i];
                    pro.pro_SetProgressValue(pro.ProgressValue);
                    pro.UpdateProgressValueDisplay(true);
                }
            }
            else
            {
                BaseScript.pro_SetProgressValue(ProgressValue.floatValue);
                BaseScript.UpdateProgressValueDisplay(true);
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
                    SerializedProperty sp_event_isfold = so_ele.FindProperty("EventIsFold");
                    so_ele.Update();

                    sp_tweens_isfold.boolValue = state;
                    sp_event_isfold.boolValue = state;
                    sp_event_isfold.serializedObject.ApplyModifiedProperties();
                    sp_tweens_isfold.serializedObject.ApplyModifiedProperties();

                    so_ele.ApplyModifiedProperties();
                }
            }
            else
            {
                if (target != null)
                {
                    EventIsFold.boolValue = state;
                    PrimitivesIsFold.boolValue = state;
                    EventIsFold.serializedObject.ApplyModifiedProperties();
                    PrimitivesIsFold.serializedObject.ApplyModifiedProperties();
                }
            }
        }
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
        /// 图元控制器 - 创建ID编号
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public virtual int PrimitiveController_CreateID(List<PrimitiveControllerNode> nodes)
        {
            List<int> ids = new List<int>();
            for (int i = 0; i < nodes.Count; i++)
            {
                ids.Add(nodes[i].Controller.GetID());
            }

            int ran_id = Random.Range(1111, 9999);

            while (true)
            {
                if (ids.Contains(ran_id))
                {
                    ran_id = Random.Range(1111, 9999);
                }
                else
                {
                    return ran_id;
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
            eve_on_ValueChanged = serializedObject.FindProperty("eve_on_ValueChanged");
            eve_on_ValueStart = serializedObject.FindProperty("eve_on_ValueStart");
            eve_on_ValueEnd = serializedObject.FindProperty("eve_on_ValueEnd");
            pro_Text_Title = serializedObject.FindProperty("pro_Text_Title");
            pro_TmpText_Title = serializedObject.FindProperty("pro_TmpText_Title");
            pro_Text_Subtitle = serializedObject.FindProperty("pro_Text_Subtitle");
            pro_TmpText_Subtitle = serializedObject.FindProperty("pro_TmpText_Subtitle");
            pro_Text_Percent = serializedObject.FindProperty("pro_Text_Percent");
            pro_TmpText_Percent = serializedObject.FindProperty("pro_TmpText_Percent");
            pro_Bg = serializedObject.FindProperty("pro_Bg");
            pro_Fore = serializedObject.FindProperty("pro_Fore");
            pro_Icon = serializedObject.FindProperty("pro_Icon");
            pro_Handle = serializedObject.FindProperty("pro_Handle");
            ProgressValue = serializedObject.FindProperty("ProgressValue");
            ProgressValueDuration = serializedObject.FindProperty("ProgressValueDuration");
            ProgressPrecision = serializedObject.FindProperty("ProgressPrecision");
            con_title = serializedObject.FindProperty("con_title");
            con_subtitle = serializedObject.FindProperty("con_subtitle");
            LerpMotion = serializedObject.FindProperty("LerpMotion");
            ProgressUnit = serializedObject.FindProperty("ProgressUnit");
            EventIsFold = serializedObject.FindProperty("EventIsFold");
            AnimateState = serializedObject.FindProperty("AnimateState");
            AutoStopPreview = serializedObject.FindProperty("AutoStopPreview");
            Display_ProgressRect_Fore = serializedObject.FindProperty("Display_ProgressRect_Fore");
            Display_ProgressRect_Bg = serializedObject.FindProperty("Display_ProgressRect_Bg");
            Display_ProgressRect_Handle = serializedObject.FindProperty("Display_ProgressRect_Handle");
            Display_Icon = serializedObject.FindProperty("Display_Icon");
            Display_Title = serializedObject.FindProperty("Display_Title");
            Display_SubTitle = serializedObject.FindProperty("Display_SubTitle");
            Display_Value = serializedObject.FindProperty("Display_Value");

            PrimitiveControllerNodes = serializedObject.FindProperty("PrimitiveControllerNodes");
            PrimitivesTweenMaxDuration = serializedObject.FindProperty("PrimitivesTweenMaxDuration");
            PrimitivesTweenGlobalDuration = serializedObject.FindProperty("PrimitivesTweenGlobalDuration");
            PrimitivesIsFold = serializedObject.FindProperty("PrimitivesIsFold");
        }
        #endregion

        #region 获取 PrimitivesTween
        /// <summary>
        /// 扫描所有图元动画器
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
                    XHud_Module_Progress progress = SelectedObjects[i];
                    if (progress.PrimitiveControllerNodes.Count > 0)
                    {
                        for (int c = 0; c < progress.PrimitiveControllerNodes.Count; c++)
                        {
                            XHud_GUI_Dialog_ListDatas dataitem = new XHud_GUI_Dialog_ListDatas();

                            dataitem.Title = $"{progress.name} ( {progress.Indicator} )";
                            dataitem.SubTitle = $"扫描到图元动画器";
                            dataitem.Message = $"{progress.PrimitiveControllerNodes[c].Controller.name} ( {progress.PrimitiveControllerNodes[c].Controller.Indicator} )";

                            Datas.Add(dataitem);
                        }
                    }
                }
                Editor_XHud_GUI.Open(Datas.ToArray(), XHud_DialogType.确认, "XHud - 进度条消息", "批量扫描图元动画器组件", "以下是批量扫描到的所有图元动画器组件列表，请您检查核对：", "明白");
            }
            else
            {
                if (BaseScript.PrimitiveControllerNodes.Count > 0)
                {
                    for (int i = 0; i < BaseScript.PrimitiveControllerNodes.Count; i++)
                    {
                        XHud_GUI_Dialog_ListDatas dataitem = new XHud_GUI_Dialog_ListDatas();

                        dataitem.Title = $"{BaseScript.name} ( {BaseScript.Indicator} )";
                        dataitem.SubTitle = $"扫描到动画器";
                        dataitem.Message = $"{BaseScript.PrimitiveControllerNodes[i].Controller.name} ( {BaseScript.PrimitiveControllerNodes[i].Controller.Indicator} )";
                        Datas.Add(dataitem);
                    }
                    Editor_XHud_GUI.Open(Datas.ToArray(), XHud_DialogType.确认, "XHud - 进度条消息", "扫描图元动画器组件", "以下是扫描到的所有图元动画器组件列表，请您检查核对：", "明白");
                }
                else
                {
                    Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 进度条消息", "扫描图元动画器组件", "未扫描到任何图元动画器组件！", "明白");
                }
            }
        }
        #endregion
    }
}