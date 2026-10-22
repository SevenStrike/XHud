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
    using SevenStrikeModules.XGUI.Editor;
    using SevenStrikeModules.XGUI.Runtime;
    using SevenStrikeModules.XHud.Enums;
    using System;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;

    [CustomEditor(typeof(XHud_TransitionController))]
    public class Editor_XHud_TransitionController : Editor
    {
        #region 字段 - 组件 / 列表
        /// <summary>
        /// 当前编辑的目标脚本实例。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>在 <c>OnEnable</c> 中由 <c>target</c> 强制转换获得。</description></item>
        /// <item><description>仅用于单选模式下的直接读写操作。</description></item>
        /// </list>
        /// </summary>
        private XHud_TransitionController BaseScript;
        /// <summary>
        /// HUD 管理器引用。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>在 <c>OnEnable</c> 中通过 <c>XHud_Dashboard.HudManagerGet()</c> 获取。</description></item>
        /// <item><description>用于访问转场库等全局数据。</description></item>
        /// </list>
        /// </summary>
        private XHud_Manager HudManager;
        #endregion

        #region 字段 - 序列化属性
        /// <summary>
        /// 序列化属性集合。
        /// <para/>
        /// <list type="bullet">
        /// <item><description><c>TransitionImage</c>：转场组件属性。</description></item>
        /// <item><description><c>TransitionMat</c>：转场材质属性。</description></item>
        /// <item><description><c>Mode / ModeSwitch</c>：转场模式与开关属性。</description></item>
        /// <item><description><c>DebugState</c>：调试开关属性。</description></item>
        /// <item><description><c>TransitionTime</c>：帧间隔速率属性。</description></item>
        /// <item><description><c>TransitionName</c>：转场名称属性。</description></item>
        /// <item><description><c>IsTransiting / IsTransiting_WithStart / IsTransiting_WithEnd</c>：转场状态属性。</description></item>
        /// <item><description><c>TransitionProgress / CurrentFrame</c>：转场进度与当前帧属性。</description></item>
        /// <item><description><c>CurrentTransitionNode</c>：当前转场节点属性。</description></item>
        /// <item><description><c>TransitionPlayKey / TransitionFlip_H_Key / TransitionFlip_V_Key / TransitionMod_Key</c>：控制按键属性。</description></item>
        /// <item><description><c>TransitionOverlayColor / TransitionAlpha</c>：叠加颜色与透明度属性。</description></item>
        /// <item><description><c>LimiteFramePer_Start / LimiteFramePer_End</c>：接近转场帧比例属性。</description></item>
        /// <item><description><c>Flip_Hor / Flip_Ver</c>：翻转开关属性。</description></item>
        /// <item><description><c>UseKeyControl</c>：按键控制开关属性。</description></item>
        /// </list>
        /// </summary>
        private SerializedProperty TransitionImage, TransitionMat, Mode, ModeSwitch, DebugState, TransitionTime, TransitionName, IsTransiting, TransitionProgress, CurrentTransitionNode, TransitionPlayKey, TransitionFlip_H_Key, TransitionFlip_V_Key, TransitionMod_Key, TransitionOverlayColor, LimiteFramePer_Start, LimiteFramePer_End, IsTransiting_WithEnd, IsTransiting_WithStart, TransitionAlpha, CurrentFrame, Flip_Hor, Flip_Ver, UseKeyControl;
        #endregion

        #region 字段 - 图标
        /// <summary>
        /// 本编辑器所使用的图标资源集合。
        /// <para/>
        /// <list type="bullet">
        /// <item><description><c>icon_main</c>：标题主图标。</description></item>
        /// <item><description><c>trans_state</c>：转场状态图标。</description></item>
        /// <item><description><c>trans_first_frame / trans_end_frame</c>：首帧/尾帧图标。</description></item>
        /// <item><description><c>openlib_r / openlib_p</c>：打开库按钮常态/按下图标。</description></item>
        /// <item><description><c>update_r / update_p</c>：刷新按钮常态/按下图标。</description></item>
        /// <item><description><c>locate_r / locate_p</c>：定位按钮常态/按下图标。</description></item>
        /// </list>
        /// </summary>
        private Texture2D icon_main, trans_state, trans_first_frame, trans_end_frame, openlib_r, openlib_p, update_r, update_p, locate_r, locate_p;
        #endregion

        #region 字段 - 选项文字
        /// <summary>
        /// 启用开关选项文字。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>索引 0：关闭。</description></item>
        /// <item><description>索引 1：开启。</description></item>
        /// </list>
        /// </summary>
        string[] stroptions_enabled = new string[2] { "关闭", "开启" };
        /// <summary>
        /// 调试开关选项文字。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>索引 0：关闭。</description></item>
        /// <item><description>索引 1：调试。</description></item>
        /// </list>
        /// </summary>
        string[] stroptions_debug = new string[2] { "关闭", "调试" };
        /// <summary>
        /// 转场模式选项文字。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>索引 0：入场。</description></item>
        /// <item><description>索引 1：出场。</description></item>
        /// </list>
        /// </summary>
        string[] stroptions_transmode = new string[] { "入场", "出场" };
        /// <summary>
        /// 翻转选项文字。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>索引 0：常规。</description></item>
        /// <item><description>索引 1：翻转。</description></item>
        /// </list>
        /// </summary>
        string[] stroptions_flip = new string[] { "常规", "翻转" };
        #endregion

        #region 字段 - 批量化操作
        /// <summary>
        /// 批量选择脚本数组。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>在 <c>GetAllTargets</c> 中根据 <c>targets</c> 填充。</description></item>
        /// <item><description>单选时长度为 1，多选时长度等于选中对象数量。</description></item>
        /// </list>
        /// </summary>
        XHud_TransitionController[] SelectedObjects;
        #endregion

        #region 生命周期    
        void OnEnable()
        {
            BaseScript = (XHud_TransitionController)target;
            #region 获取序列化属性
            TransitionImage = serializedObject.FindProperty("TransitionImage");
            TransitionMat = serializedObject.FindProperty("TransitionMat");
            Mode = serializedObject.FindProperty("Mode");
            ModeSwitch = serializedObject.FindProperty("ModeSwitch");
            TransitionTime = serializedObject.FindProperty("TransitionTime");
            TransitionName = serializedObject.FindProperty("TransitionName");
            IsTransiting = serializedObject.FindProperty("IsTransiting");
            TransitionProgress = serializedObject.FindProperty("TransitionProgress");
            CurrentFrame = serializedObject.FindProperty("CurrentFrame");
            TransitionName = serializedObject.FindProperty("TransitionName");
            CurrentTransitionNode = serializedObject.FindProperty("CurrentTransitionNode");
            TransitionPlayKey = serializedObject.FindProperty("TransitionPlayKey");
            TransitionFlip_H_Key = serializedObject.FindProperty("TransitionFlip_H_Key");
            TransitionFlip_V_Key = serializedObject.FindProperty("TransitionFlip_V_Key");
            TransitionMod_Key = serializedObject.FindProperty("TransitionMod_Key");
            LimiteFramePer_Start = serializedObject.FindProperty("LimiteFramePer_Start");
            LimiteFramePer_End = serializedObject.FindProperty("LimiteFramePer_End");
            IsTransiting_WithEnd = serializedObject.FindProperty("IsTransiting_WithEnd");
            IsTransiting_WithStart = serializedObject.FindProperty("IsTransiting_WithStart");
            DebugState = serializedObject.FindProperty("DebugState");
            TransitionOverlayColor = serializedObject.FindProperty("TransitionOverlayColor");
            TransitionAlpha = serializedObject.FindProperty("TransitionAlpha");
            Flip_Hor = serializedObject.FindProperty("Flip_Hor");
            Flip_Ver = serializedObject.FindProperty("Flip_Ver");
            UseKeyControl = serializedObject.FindProperty("UseKeyControl");
            HudManager = XHud_Dashboard.get_manager();
            #endregion
            #region 获取图标
            icon_main = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_transition_controller/icon_main");
            trans_state = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_transition_controller/trans_state");
            trans_first_frame = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_transition_controller/trans_first_frame");
            trans_end_frame = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_transition_controller/trans_end_frame");
            openlib_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_transition_controller/openlib_r");
            openlib_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_transition_controller/openlib_p");
            update_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_transition_controller/update_r");
            update_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_transition_controller/update_p");
            locate_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_transition_controller/locate_r");
            locate_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_transition_controller/locate_p");
            #endregion
            Targets_Get();
            if ((Image)TransitionImage.objectReferenceValue == null)
            {
                TransitionImage.objectReferenceValue = BaseScript.GetComponent<Image>();
                TransitionImage.serializedObject.ApplyModifiedProperties();
            }
            if (TransitionMat.objectReferenceValue == null)
            {
                TransitionMat.objectReferenceValue = (Material)AssetDatabase.LoadAssetAtPath($"{XHud_Dashboard.Get_Path_XHUD_MATERIALS_Path()}Transitions/Transition.mat", typeof(Material));
            }
            if (TransitionMat.objectReferenceValue != null)
            {
                Material mat = TransitionMat.objectReferenceValue as Material;
                TransitionOverlayColor.colorValue = mat.color;
                TransitionOverlayColor.serializedObject.ApplyModifiedProperties();
            }
            if (string.IsNullOrEmpty(TransitionName.stringValue))
            {
                if (HudManager.Hud_TransitionLib != null && !HudManager.Hud_TransitionLib.TransitionLibrary_IsEmpty())
                {
                    TransitionName.stringValue = HudManager.Hud_TransitionLib.TransitionLibrary_GetAllNames()[0];
                    TransitionName.serializedObject.ApplyModifiedProperties();
                    BaseScript.CurrentTransitionNode = HudManager.Hud_TransitionLib.TransitionLibrary_Get(TransitionName.stringValue);
                }
            }
        }

        void OnDisable()
        {

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
#if UNITY_6000_0_OR_NEWER
            TextClipping clipping = TextClipping.Ellipsis;
#else
            TextClipping clipping = TextClipping.Clip;
#endif
            #region 标题
            XGUI.layout_banner(
             bg_fill: XGUIGroup.实体,
             bg_color: XGUIColor.深空灰,
             bg_height: 30,
             icon: icon_main,
             icon_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
             title_text: "XHud  -  转场控制器",
             title_anchor: TextAnchor.MiddleLeft,
             title_style: FontStyle.Normal,
             title_color: Color.white,
             title_size: XGUIFontSize.B,
             title_clipping: clipping,
             bg_margin: new RectOffset(0, 0, 5, 5));
            #endregion
            XGUI.layout_space(5);
            #region 快捷功能
            XGUI.layout_group_start(
                type: XGUIContainerType.Horizontal,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "快捷功能",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(15, 15, 20, 15));
            #region 查看当前转场库
            if (XGUI.layout_button(
                tooltip: "查看当前转场库",
                tex_release: openlib_r,
                tex_press: openlib_p,
                tex_gui_color: Color.white,
                border: new RectOffset(0, 0, 0, 0),
                width: 14,
                height: 14))
            {
                Editor_XHud_MenuItemsAction_OpenLibrary.open_transition();
            }
            #endregion
            GUILayout.FlexibleSpace();
            #region 快速刷新转场资源
            if (XGUI.layout_button(
                tooltip: "快速刷新转场资源",
                tex_release: update_r,
                tex_press: update_p,
                tex_gui_color: Color.white,
                border: new RectOffset(0, 0, 0, 0),
                width: 14,
                height: 14))
            {
                if (HudManager.Hud_TransitionLib == null)
                    return;
                if (HudManager.Hud_TransitionLib.TransitionLibrary_IsEmpty())
                    return;
                SerializedProperty sp_Name = CurrentTransitionNode.FindPropertyRelative("Name");
                SerializedProperty sp_TotalFramesCount = CurrentTransitionNode.FindPropertyRelative("TotalFramesCount");
                SerializedProperty sp_LastFrameIndex = CurrentTransitionNode.FindPropertyRelative("LastFrameIndex");
                SerializedProperty sp_SkipFrame = CurrentTransitionNode.FindPropertyRelative("SkipFrame");
                SerializedProperty sp_Res = CurrentTransitionNode.FindPropertyRelative("Res");
                SerializedProperty sp_x_frames = CurrentTransitionNode.FindPropertyRelative("Frames");
                XHud_LibraryArg_Transition node = HudManager.Hud_TransitionLib.TransitionLibrary_Get(TransitionName.stringValue);
                sp_Name.stringValue = node.Name;
                sp_TotalFramesCount.intValue = node.TotalFramesCount;
                sp_LastFrameIndex.intValue = node.LastFrameIndex;
                sp_SkipFrame.intValue = node.SkipFrame;
                sp_Res.vector2IntValue = node.Res;
                sp_x_frames.ClearArray();
                for (int i = 0; i < node.Frames.Count; i++)
                {
                    sp_x_frames.InsertArrayElementAtIndex(i);
                    sp_x_frames.GetArrayElementAtIndex(i).objectReferenceValue = node.Frames[i];
                }
                TransitionProgress.floatValue = 0;
                TransitionProgress.serializedObject.ApplyModifiedProperties();
                UpdateTransition(sp_x_frames, sp_LastFrameIndex);
                return;
            }
            #endregion
            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            #endregion
            #region 选项         
            BaseScript.fold_options = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "选项",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: BaseScript.fold_options);
            if (!BaseScript.fold_options)
            {
                DrawToggle("按键控制", UseKeyControl, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, (b) => { });
                DrawToggle("转场模式 (入场 & 出场)", ModeSwitch, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, (b) =>
                {
                    if (ModeSwitch.boolValue)
                        Mode.enumValueIndex = (int)TransitionMode.出场;
                    else
                        Mode.enumValueIndex = (int)TransitionMode.入场;
                    Mode.serializedObject.ApplyModifiedProperties();
                    TransitionProgress.floatValue = 0;
                    TransitionProgress.serializedObject.ApplyModifiedProperties();
                });
                DrawToggle("水平翻转", Flip_Hor, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, (b) =>
                {
                    Material mat = (Material)TransitionMat.objectReferenceValue;
                    mat.SetInt("_Flip_Hor", Flip_Hor.boolValue ? 1 : 0);
                    TransitionMat.serializedObject.ApplyModifiedProperties();
                });
                DrawToggle("垂直翻转", Flip_Ver, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, (b) =>
                {
                    Material mat = (Material)TransitionMat.objectReferenceValue;
                    mat.SetInt("_Flip_Ver", Flip_Ver.boolValue ? 1 : 0);
                    TransitionMat.serializedObject.ApplyModifiedProperties();
                });
                DrawToggle("状态调试", DebugState, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, (b) => { });
                XGUI.layout_group_start(
                    type: XGUIContainerType.Horizontal,
                    absolute_margin: true,
                    absolute_padding: true,
                    padding: new RectOffset(0, 0, 0, 0),
                    margin: new RectOffset(0, 0, 0, 0));
                string[] TransLibNames = HudManager.Hud_TransitionLib.TransitionLibrary_GetAllNames();
                XGUI.layout_string_popup(
                    title: "转场资源库",
                    title_width: 120,
                    title_size: XGUIFontSize.M,
                    title_anchor: TextAnchor.MiddleLeft,
                    prop: TransitionName,
                    options: TransLibNames,
                    opt_text_size: XGUIFontSize.M,
                    opt_text_color: Color.black,
                    opt_text_padding: new RectOffset(10, 10, 0, 0),
                    opt_anchor: TextAnchor.MiddleLeft,
                    opt_font_style: FontStyle.Normal,
                    opt_bg_fill: XGUIGroup.实体,
                    opt_bg_color: XGUIColor.亮白,
                    opt_bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    margin: new RectOffset(0, 0, 5, 5),
                    padding: new RectOffset(5, 5, 0, 0),
                    title_margin: new RectOffset(0, 0, 0, 0),
                    icon_arrow_color: Color.black,
                    act_on_changed: (d) =>
                    {
                        SerializedProperty sp_Name = CurrentTransitionNode.FindPropertyRelative("Name");
                        SerializedProperty sp_TotalFramesCount = CurrentTransitionNode.FindPropertyRelative("TotalFramesCount");
                        SerializedProperty sp_LastFrameIndex = CurrentTransitionNode.FindPropertyRelative("LastFrameIndex");
                        SerializedProperty sp_SkipFrame = CurrentTransitionNode.FindPropertyRelative("SkipFrame");
                        SerializedProperty sp_Res = CurrentTransitionNode.FindPropertyRelative("Res");
                        SerializedProperty sp_x_frames = CurrentTransitionNode.FindPropertyRelative("Frames");
                        XHud_LibraryArg_Transition node = HudManager.Hud_TransitionLib.TransitionLibrary_Get(d);
                        sp_Name.stringValue = node.Name;
                        sp_TotalFramesCount.intValue = node.TotalFramesCount;
                        sp_LastFrameIndex.intValue = node.LastFrameIndex;
                        sp_SkipFrame.intValue = node.SkipFrame;
                        sp_Res.vector2IntValue = node.Res;
                        sp_x_frames.ClearArray();
                        for (int i = 0; i < node.Frames.Count; i++)
                        {
                            sp_x_frames.InsertArrayElementAtIndex(i);
                            sp_x_frames.GetArrayElementAtIndex(i).objectReferenceValue = node.Frames[i];
                        }
                        TransitionProgress.floatValue = 0;
                        UpdateTransition(sp_x_frames, sp_LastFrameIndex);
                    });
                if (!Targets_Selected())
                {
                    XGUI.layout_space(10);
                    if (XGUI.layout_button(
                        tooltip: "定位到转场库",
                        tex_release: locate_r,
                        tex_press: locate_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        margin: new RectOffset(0, 15, 6, 0),
                        width: 14,
                        height: 14))
                    {
                        //获取目标转场的索引号
                        int index = 0;
                        for (int k = 0; k < TransLibNames.Length; k++)
                        {
                            if (TransLibNames[k] == TransitionName.stringValue)
                            {
                                index = k;
                                break;
                            }
                        }
                        //定位到元素库中的对应当前资源
                        //打开目标转场库
                        Editor_XHud_MenuItemsAction_OpenLibrary.open_transition();
                        HudManager.Hud_TransitionLib.TransitionLibrary_Location(TransLibNames[index]);
                    }
                }
                XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion
            #region 状态
            BaseScript.fold_state = XGUI.layout_group_start(
              type: XGUIContainerType.Vertical,
              bg_fill: XGUIGroup.缺口纯色边框,
              bg_color: XGUIColor.亮白,
              bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
              title: "状态",
              title_size: XGUIFontSize.M,
              title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
              title_clipping: TextClipping.Clip,
              padding: new RectOffset(10, 10, 15, 15),
              foldout: BaseScript.fold_state);
            if (!BaseScript.fold_state)
            {
                #region 转场状态
                XGUI.layout_state_displayer_text(
                    title: "转场状态",
                    title_size: XGUIFontSize.M,
                    subtitle: IsTransiting.boolValue ? "正在转场中" : "等待转场",
                    subtitle_size: XGUIFontSize.M,
                    subtitle_color: IsTransiting.boolValue ? XHud_Dashboard.themes_get_color(ThemeColorType.primary) : Color.gray,
                    margin: new RectOffset(5, 5, 0, 5));
                #endregion
                #region 接近首帧状态
                SerializedProperty sp_frames_co = CurrentTransitionNode.FindPropertyRelative("Frames");
                int frameLimite_start = (int)Mathf.Floor(sp_frames_co.arraySize * LimiteFramePer_Start.floatValue);
                XGUI.layout_state_displayer_text(
                    title: "接近首帧状态",
                    title_size: XGUIFontSize.M,
                    subtitle: IsTransiting_WithStart.boolValue ? $"接近首帧状态 ({frameLimite_start})" : $"等待转场 ({frameLimite_start})",
                    subtitle_size: XGUIFontSize.M,
                    subtitle_color: IsTransiting_WithStart.boolValue ? XHud_Dashboard.themes_get_color(ThemeColorType.primary) : Color.gray,
                    margin: new RectOffset(5, 5, 0, 5));
                #endregion
                #region 接近尾帧状态
                int frameLimite_end = (int)Mathf.Floor(sp_frames_co.arraySize * LimiteFramePer_End.floatValue);
                XGUI.layout_state_displayer_text(
                    title: "接近尾帧状态",
                    title_size: XGUIFontSize.M,
                    subtitle: IsTransiting_WithEnd.boolValue ? $"接近尾帧状态 ({frameLimite_end})" : $"等待转场 ({frameLimite_end})",
                    subtitle_size: XGUIFontSize.M,
                    subtitle_color: IsTransiting_WithEnd.boolValue ? XHud_Dashboard.themes_get_color(ThemeColorType.primary) : Color.gray,
                    margin: new RectOffset(5, 5, 0, 5));
                #endregion
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion
            #region 参数
            BaseScript.fold_params = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "参数",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(5, 10, 15, 15),
                foldout: BaseScript.fold_params);
            if (!BaseScript.fold_params)
            {
                #region 转场组件
                XGUI.layout_property_field(
                    title: "转场组件",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: TransitionImage,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion
                #region 转场材质
                EditorGUI.BeginChangeCheck();
                XGUI.layout_property_field(
                  title: "转场材质",
                  title_size: XGUIFontSize.M,
                  title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                  title_width: 90,
                  prop: TransitionMat,
                  prop_margin: new RectOffset(0, 0, 5, 0));
                if (EditorGUI.EndChangeCheck())
                {
                    Image img = (Image)TransitionImage.objectReferenceValue;
                    img.material = (Material)TransitionMat.objectReferenceValue;
                    TransitionImage.serializedObject.ApplyModifiedProperties();
                }
                #endregion
                #region 转场叠加颜色
                EditorGUI.BeginChangeCheck();
                XGUI.layout_property_field(
                  title: "转场叠加颜色",
                  title_size: XGUIFontSize.M,
                  title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                  title_width: 90,
                  prop: TransitionOverlayColor,
                  prop_margin: new RectOffset(0, 0, 5, 0));
                if (EditorGUI.EndChangeCheck())
                {
                    Image img = (Image)TransitionImage.objectReferenceValue;
                    img.color = Color.white;
                    img.material = (Material)TransitionMat.objectReferenceValue;
                    img.material.SetColor("_Color", TransitionOverlayColor.colorValue);
                    TransitionImage.serializedObject.ApplyModifiedProperties();
                }
                #endregion
                #region 转场参数
                XGUI.layout_property_field(
                    title: "转场参数",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: CurrentTransitionNode,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion
                #region 转场进度
                SerializedProperty sp_frames = CurrentTransitionNode.FindPropertyRelative("Frames");
                SerializedProperty sp_lastframeindex = CurrentTransitionNode.FindPropertyRelative("LastFrameIndex");
                if (sp_frames != null && sp_frames.arraySize > 0)
                {
                    EditorGUI.BeginChangeCheck();
                    XGUI.layout_property_field(
                        title: "转场进度",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: TransitionProgress,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    if (EditorGUI.EndChangeCheck())
                    {
                        UpdateTransition(sp_frames, sp_lastframeindex);
                    }
                }
                #endregion
                #region 转场透明度            
                EditorGUI.BeginChangeCheck();
                TransitionAlpha.floatValue = XGUI.layout_slider(
                    title: "透明度",
                    prop: TransitionAlpha,
                    left: 0,
                    right: 1,
                    title_width: 100,
                    title_size: XGUIFontSize.M,
                    title_anchor: TextAnchor.MiddleLeft,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                TransitionAlpha.serializedObject.ApplyModifiedProperties();
                if (EditorGUI.EndChangeCheck())
                {
                    BaseScript.Transition_Set_Alpha(TransitionAlpha.floatValue);
                    if (TransitionMat.objectReferenceValue != null)
                    {
                        Material mat = TransitionMat.objectReferenceValue as Material;
                        mat.SetFloat("_Alpha", TransitionAlpha.floatValue);
                        TransitionOverlayColor.serializedObject.ApplyModifiedProperties();
                    }
                }
                #endregion
                XGUI.layout_seperator(
                    thickness: 1,
                    color: Color.white * 0.5f,
                    margin: new RectOffset(15, 30, 15, 15));
                #region 当前帧
                XGUI.layout_property_field(
                    title: "当前帧",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: CurrentFrame,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion
                #region 帧间隔速率
                XGUI.layout_property_field(
                    title: "帧间隔速率",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: TransitionTime,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion
                XGUI.layout_seperator(
                    thickness: 1,
                    color: Color.white * 0.5f,
                    margin: new RectOffset(15, 30, 15, 15));
                #region 接近开始帧
                xgui_minmax_value minmax = XGUI.layout_slider_min_max(
                 title: "接近转场",
                 ref_min: ref BaseScript.LimiteFramePer_Start,
                 ref_max: ref BaseScript.LimiteFramePer_End,
                 min_limite: 0,
                 max_limite: 1,
                 title_width: 100,
                 title_size: XGUIFontSize.M,
                 title_anchor: TextAnchor.MiddleLeft,
                 min_field_color: Color.white,
                 max_field_color: Color.white,
                 shrink_text_color: Color.white * 0.65f,
                 control_limite: 210,
                 prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion
            #region 控制按键
            BaseScript.fold_key = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "控制按键",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(5, 10, 15, 15),
                foldout: BaseScript.fold_key);
            if (!BaseScript.fold_key)
            {
                #region 转场模式
                XGUI.layout_property_field(
                    title: "转场模式",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: TransitionMod_Key,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion
                #region 按键转场
                XGUI.layout_property_field(
                    title: "按键转场",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: TransitionPlayKey,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion
                #region 水平翻转
                XGUI.layout_property_field(
                    title: "水平翻转",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: TransitionFlip_H_Key,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion
                #region 垂直翻转
                XGUI.layout_property_field(
                    title: "垂直翻转",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: TransitionFlip_V_Key,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion
            #region 源脚本
            BaseScript.fold_based = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "源脚本",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: BaseScript.fold_based);
            if (!BaseScript.fold_based)
            {
                DrawDefaultInspector();
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion
            serializedObject.ApplyModifiedProperties();
        }
        #endregion

        #region 工具 - 转场更新
        /// <summary>
        /// 更新转场数据。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>根据进度计算当前帧。</description></item>
        /// <item><description>按模式设置通道反转。</description></item>
        /// <item><description>设置当前帧的遮罩贴图。</description></item>
        /// </list>
        /// </summary>
        /// <param name="sp_frames">帧贴图数组属性。</param>
        /// <param name="sp_lastframeindex">最后一帧索引属性。</param>
        private void UpdateTransition(SerializedProperty sp_frames, SerializedProperty sp_lastframeindex)
        {
            TransitionMode mode = (TransitionMode)Mode.enumValueIndex;
            CurrentFrame.intValue = (int)(TransitionProgress.floatValue * sp_lastframeindex.intValue);
            if (mode == TransitionMode.入场)
            {
                BaseScript.Transition_Set_ChannelInvert(false);
            }
            else
            {
                BaseScript.Transition_Set_ChannelInvert(true);
            }
            if (sp_frames.arraySize > 0)
            {
                Texture2D tex = (Texture2D)sp_frames.GetArrayElementAtIndex(CurrentFrame.intValue).objectReferenceValue;
                BaseScript.Transition_Set_MaskTexture(tex);
            }
        }
        #endregion

        #region 工具 - 批量化操作
        /// <summary>
        /// 获取所有批量脚本目标。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>多选时创建与 <c>targets</c> 等长的数组并逐个填充。</description></item>
        /// <item><description>单选时创建长度为 1 的数组并填充当前目标。</description></item>
        /// </list>
        /// </summary>
        private void Targets_Get()
        {
            if (targets.Length > 1)
            {
                SelectedObjects = new XHud_TransitionController[targets.Length];
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    var t = targets[i];
                    SelectedObjects[i] = (XHud_TransitionController)t;
                }
            }
            else
            {
                SelectedObjects = new XHud_TransitionController[targets.Length];
                SelectedObjects[0] = (XHud_TransitionController)target;
            }
        }
        /// <summary>
        /// 判断是否是多选状态。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>当 <c>SelectedObjects</c> 为 null 时返回 false。</description></item>
        /// <item><description>当 <c>SelectedObjects.Length > 1</c> 时返回 true。</description></item>
        /// </list>
        /// </summary>
        /// <returns>是否为多选状态。</returns>
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

        #region 绘制控件
        /// <summary>
        /// 通用方法：绘制开关。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>封装 <c>XGUI.layout_toggle</c> 调用。</description></item>
        /// <item><description>提供标题、宽度、样式、颜色等参数。</description></item>
        /// </list>
        /// </summary>
        private void DrawToggle(string title, SerializedProperty prop, float width, XGUIToggleStyle style = XGUIToggleStyle.实体, Color color_bg_on = default, Color color_bg_off = default, Color color_on = default, Color color_off = default, Action<bool> act_on_changed = null)
        {
            XGUI.layout_toggle(
                title: title,
                title_size: XGUIFontSize.M,
                title_font_style: FontStyle.Normal,
                title_padding: new RectOffset(5, 10, 0, 0),
                title_width: width,
                prop: prop,
                tog_style: style,
                tog_padding: new RectOffset(0, 9, 0, 0),
                tog_margin: new RectOffset(0, 0, 0, 5),
                tog_mixed_options: new string[] { "禁用", "启用" },
                tog_mixed_text_size: XGUIFontSize.M,
                tog_mixed_text_color: Color.black,
                tog_mixed_text_padding: new RectOffset(10, 10, 0, 0),
                tog_mixed_text_anchor: TextAnchor.MiddleCenter,
                tog_mixed_font_style: FontStyle.Normal,
                tog_bg_off_color: color_bg_off,
                tog_bg_on_color: color_bg_on,
                tog_handler_off_color: color_off,
                tog_handler_on_color: color_on,
                tog_mixed_bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                act_on_changed: act_on_changed);
        }
        #endregion
    }
}