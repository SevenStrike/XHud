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
    using SevenStrikeModules.XTween;
    using SevenStrikeModules.XTween.Editor;
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.UI;
    using Color = UnityEngine.Color;
    using Object = UnityEngine.Object;

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

        #region 序列化属性
        private SerializedProperty
            CanvasGroup, SounderNodes, PrimitiveControllerNodes, OptionNodes, ButtonNodes, TextIsFold, TmpTextIsFold, SliderNodes, TextNodes, TmpTextNodes, DebugState, ProgressNodes, ToggleNodes, Indicator, PrimitivesTweenMaxDuration, RectTransform, TriggerAction, ObjectTracker, CreateState, AnimateState, PrimitivesTweenGlobalDuration, OriginPoolName, PrimitivesIsFold, ButtonIsFold, OptionIsFold, SliderIsFold, SounderIsFold, ProgressIsFold, ToggleIsFold, EventIsFold, AutoPlayPrimitivesTween, AutoKillPreviewTweens, Alpha, RMS_Enabled, RMS_LayoutDatas, RMS_Name, CurrentPivot, OriginalName, TweensPreivew_In_State, TweensPreivew_Out_State, RewindPreviewTweensWithKill, ClearPreviewTweensWithKill, CreateArgs, RecycleArgs, Crc_Lib_Name, Rec_Lib_Name, CreateArgs_MotionAnimateEndState, RecycleArgs_MotionAnimateEndState, Crc_Preview_Position, Rec_Preview_Position, Crc_Preview_SetPosition, Rec_Preview_SetPosition, PreviewIncludePrimitivesTween, ClearActions_With_Spawn, ClearEvents_With_Spawn, Highlighter;
        #endregion       

        #region 图标
        private Texture2D icon_main, icon_sound, icon_anim, icon_button, icon_option, icon_slider, icon_progress, icon_toggle, icon_text, icon_tmptext, icon_scan_r, icon_scan_p, icon_record_rms_r, icon_record_rms_p, resetanchorpos_r, resetanchorpos_p, locate_r, locate_p, left_arrow_r, left_arrow_p, right_arrow_r, right_arrow_p, prw_play_out_r, prw_play_out_p, prw_play_in_p, prw_play_in_r, save_r, save_p, reset_r, reset_p, icon_recreateid_r, icon_recreateid_p;

        #endregion

        #region 开关
        private bool
            def_recycle_fold_move,
            def_recycle_fold_rotate,
            def_recycle_fold_alpha,
            def_create_fold_move,
            def_create_fold_rotate,
            def_create_fold_alpha;
        #endregion

        #region 批量模式查看索引
        private int ElementStatu_Index;
        #endregion

        #region 选项文字
        string[] stroptions_enabled = new string[2] { "关闭", "开启" }, stroptions_include = new string[2] { "不包含", "包含" }, stroptions_debug = new string[2] { "关闭", "调试" }, stroptions_auto = new string[2] { "手动", "自动" };
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

            Vector2 ButtonSize = new Vector2(14, 14);

            ClearEmptyNodes();

            #region 图标获取
            icon_main = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/icon_main");
            icon_sound = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/icon_sound");
            icon_button = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/icon_button");
            icon_option = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/icon_option");
            icon_slider = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/icon_slider");
            icon_progress = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/icon_progress");
            icon_toggle = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/icon_toggle");
            icon_text = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/icon_text");
            icon_tmptext = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/icon_tmptext");
            icon_anim = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/icon_anim");
            icon_scan_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/icon_scan_r");
            icon_scan_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/icon_scan_p");
            icon_record_rms_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/icon_record_rms_r");
            icon_record_rms_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/icon_record_rms_p");
            resetanchorpos_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/resetanchorpos_r");
            resetanchorpos_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/resetanchorpos_p");
            locate_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/locate_r");
            locate_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/locate_p");
            left_arrow_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/left_arrow_r");
            left_arrow_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/left_arrow_p");
            right_arrow_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/right_arrow_r");
            right_arrow_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/right_arrow_p");
            prw_play_out_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/prw_play_out_r");
            prw_play_out_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/prw_play_out_p");
            prw_play_in_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/prw_play_in_p");
            prw_play_in_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/prw_play_in_r");
            save_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/save_r");
            save_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/save_p");
            reset_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/reset_r");
            reset_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/reset_p");
            icon_recreateid_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/icon_recreateid_r");
            icon_recreateid_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_element/icon_recreateid_p");
            #endregion

            // 控件列表绘制
            ReorderableList_Draw_Sounder();
            ReorderableList_Draw_PrimitiveController();
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
                    string res = XGUI.dialog(
                        type: XGUIDialogType.警告,
                        windowtitle: "XHud - 元素消息",
                        title: "RMS残留信息",
                        msg: $"发现残留匹配分辨率信息列表，是否需要清空？请谨慎此操作！",
                        ok: "清空",
                        cancel: "暂不",
                        PrimaryIndex: 0,
                        usemodal: true,
                        themecolor: XHud_Dashboard.Theme_Primary);
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

            //AllListFoldState(true);

            Editor_XHud_Tool_SceneView_Activate_Mark.SetEnabled(false);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            string hexcol = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary, true);

#if UNITY_6000_0_OR_NEWER
            TextClipping clipping = TextClipping.Ellipsis;
#else
    TextClipping clipping = TextClipping.Clip;
#endif

            #region 标题
            XGUI.layout_banner(
                bg_fill: XGUIFilled.实体,
                bg_color: XGUIColor.深空灰,
                bg_height: 30,
                icon: icon_main,
                icon_color: XHud_Dashboard.Theme_Primary,
                title_text: string.IsNullOrEmpty(Indicator.stringValue) ? "XHud  -  元素" : "XHud - 元素 -> ( " + Indicator.stringValue + " )",
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
               bg_fill: XGUIFilled.缺口纯色边框,
               bg_color: XGUIColor.亮白,
               bg_color_gui: XHud_Dashboard.Theme_Group,
               title: "快捷功能",
               title_size: XGUIFontSize.M,
               title_text_color: XHud_Dashboard.Theme_Primary,
               title_clipping: TextClipping.Clip,
               padding: new RectOffset(15, 15, 20, 15));

            #region 扫描子组件
            if (XGUI.layout_button(
                tooltip: "扫描子组件",
                tex_release: icon_scan_r,
                tex_press: icon_scan_p,
                tex_gui_color: Color.white,
                border: new RectOffset(0, 0, 0, 0),
                width: 14,
                height: 14))
            {
                if (Application.isPlaying)
                {
                    XGUI.dialog(
                        type: XGUIDialogType.警告,
                        windowtitle: "XHud - 元素消息",
                        title: "扫描子元素",
                        msg: $"程序正在运行，无法在运行期间执行此功能！",
                        ok: "明白",
                        PrimaryIndex: 0,
                        usemodal: true,
                        themecolor: XHud_Dashboard.Theme_Primary);
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

            XGUI.layout_flexspace();

            #region 记录RMS布局信息
            if (RMS_Enabled.boolValue)
            {
                if (XGUI.layout_button(
                tooltip: "记录RMS布局信息",
                tex_release: icon_record_rms_r,
                tex_press: icon_record_rms_p,
                tex_gui_color: Color.white,
                border: new RectOffset(0, 0, 0, 0),
                width: 14,
                height: 14))
                {
                    if (Application.isPlaying)
                    {
                        XGUI.dialog(
                            type: XGUIDialogType.警告,
                            windowtitle: "XHud - 元素消息",
                            title: "记录RMS布局信息",
                            msg: $"程序正在运行，无法在运行期间执行此功能！",
                            ok: "明白",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.Theme_Primary);

                        return;
                    }

                    RMS_Record();

                    return;
                }
                XGUI.layout_flexspace();
            }
            #endregion

            #region 元素重置到锚点初始位置
            if (XGUI.layout_button(
                tooltip: "元素重置到锚点初始位置",
                tex_release: resetanchorpos_r,
                tex_press: resetanchorpos_p,
                tex_gui_color: Color.white,
                border: new RectOffset(0, 0, 0, 0),
                width: 14,
                height: 14))
            {
                if (Application.isPlaying)
                {
                    XGUI.dialog(
                        type: XGUIDialogType.警告,
                        windowtitle: "XHud - 元素消息",
                        title: "锚点初始化",
                        msg: $"程序正在运行，无法在运行期间执行此功能！",
                        ok: "明白",
                        PrimaryIndex: 0,
                        usemodal: true,
                        themecolor: XHud_Dashboard.Theme_Primary);

                    return;
                }

                RestoreToAnchorPosition();

                return;
            }
            #endregion

            #region 刷新所有图元控制的ID
            if (PrimitiveControllerNodes.arraySize > 0)
            {
                XGUI.layout_flexspace();

                if (XGUI.layout_button(
                    tooltip: "刷新所有图元控制的ID",
                    tex_release: icon_recreateid_r,
                    tex_press: icon_recreateid_p,
                    tex_gui_color: Color.white,
                    border: new RectOffset(0, 0, 0, 0),
                    width: 14,
                    height: 14))
                {
                    if (Targets_Selected())
                    {
                        List<XHud_GUI_Dialog_ListDatas> Datas = new List<XHud_GUI_Dialog_ListDatas>();

                        for (int i = 0; i < SelectedObjects.Length; i++)
                        {
                            XHud_GUI_Dialog_ListDatas data = new XHud_GUI_Dialog_ListDatas();
                            data.Title = string.IsNullOrEmpty(SelectedObjects[i].Indicator) ? SelectedObjects[i].name : SelectedObjects[i].Indicator;
                            data.SubTitle = "即将更新所有子级图元控制器的ID";
                            data.Message = "";
                            Datas.Add(data);
                        }

                        EditorApplication.delayCall += () =>
                        {
                            string res_x = XGUI.dialog(
                                type: XGUIDialogType.警告,
                                windowtitle: "XHud - 元素消息",
                                title: "批量元素子级控制器ID刷新",
                                msg: $"是否需要批量刷新所有元素的子级的控制器的ID吗？",
                                ok: "刷新",
                                cancel: "暂不",
                                PrimaryIndex: 0,
                                usemodal: true,
                                themecolor: XHud_Dashboard.Theme_Primary);

                            if (res_x == "刷新")
                            {
                                for (int i = 0; i < SelectedObjects.Length; i++)
                                {
                                    XHud_Module_Element ele = SelectedObjects[i];

                                    // 收集每个元素下所有子级图元控制器的 ID
                                    List<string> list = new List<string>();
                                    for (int k = 0; k < ele.PrimitiveControllerNodes.Count; k++)
                                    {
                                        list.Add(ele.PrimitiveControllerNodes[k].Controller.ID);
                                    }

                                    for (int c = 0; c < ele.PrimitiveControllerNodes.Count; c++)
                                    {
                                        Undo.RecordObject(ele.PrimitiveControllerNodes[c].Controller, "刷新控制器ID");
                                        XHud_Module_Primitive_Controller con = ele.PrimitiveControllerNodes[c].Controller;
                                        // 根据收集的 list 来不重复创建 ID 给每个子级图元控制器
                                        con.ID = XGUI_Utilitys.GenerateUniqueId(list.ToArray());
                                    }
                                }
                            }
                        };
                    }
                    else
                    {
                        EditorApplication.delayCall += () =>
                        {
                            string res_x = XGUI.dialog(
                               type: XGUIDialogType.警告,
                               windowtitle: "XHud - 元素消息",
                               title: "所有图元控制器ID刷新",
                               msg: $"是否需要批量刷新所有图元控制器的ID吗？",
                               ok: "刷新",
                               cancel: "暂不",
                               PrimaryIndex: 0,
                               usemodal: true,
                               themecolor: XHud_Dashboard.Theme_Primary);
                            if (res_x == "刷新")
                            {
                                List<string> list = new List<string>();
                                for (int i = 0; i < BaseScript.PrimitiveControllerNodes.Count; i++)
                                {
                                    XHud_Module_Primitive_Controller con = BaseScript.PrimitiveControllerNodes[i].Controller;
                                    list.Add(con.ID);
                                }

                                for (int i = 0; i < BaseScript.PrimitiveControllerNodes.Count; i++)
                                {
                                    Undo.RecordObject(BaseScript.PrimitiveControllerNodes[i].Controller, "批量刷新ID");
                                    XHud_Module_Primitive_Controller con = BaseScript.PrimitiveControllerNodes[i].Controller;
                                    // 记录每个对象的状态
                                    con.ID = XGUI_Utilitys.GenerateUniqueId(list.ToArray());
                                }
                            }
                        };
                    }
                }
            }
            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            #endregion

            #region 预览          
            BaseScript.fold_preview = XGUI.layout_group_start(
               type: XGUIContainerType.Vertical,
               bg_fill: XGUIFilled.缺口纯色边框,
               bg_color: XGUIColor.亮白,
               bg_color_gui: XHud_Dashboard.Theme_Group,
               title: "预览",
               title_size: XGUIFontSize.M,
               title_text_color: XHud_Dashboard.Theme_Primary,
               title_clipping: TextClipping.Clip,
               padding: new RectOffset(5, 5, 20, 15),
               foldout: BaseScript.fold_preview);

            if (!BaseScript.fold_preview)
            {
                #region 控制按钮
                XGUI.layout_group_start(
                    type: XGUIContainerType.Horizontal,
                    bg_fill: XGUIFilled.透明,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.Theme_Group,
                    absolute_margin: true,
                    absolute_padding: true,
                    margin: new RectOffset(0, 0, 0, 0),
                    padding: new RectOffset(15, 15, 5, 10));

                #region 生成
                XGUI.SetEnabled(!TweensPreivew_Out_State.boolValue);
                if (XGUI.layout_button(
                    tooltip: !TweensPreivew_In_State.boolValue ? "元素 - 进入 - 播放" : "元素 - 进入 - 停止",
                    tex_release: !TweensPreivew_In_State.boolValue ? prw_play_in_r : prw_play_in_p,
                    tex_press: !TweensPreivew_In_State.boolValue ? prw_play_in_p : prw_play_in_p,
                    tex_gui_color: Color.white,
                    border: new RectOffset(0, 0, 0, 0),
                    width: 14,
                    height: 14))
                {
                    if (!TweensPreivew_In_State.boolValue)
                        ElementTweens_Preview_In_Play();
                    else
                        ElementTweens_Preview_In_Stop();
                }
                #endregion

                XGUI.layout_flexspace();

                #region 回收
                XGUI.SetEnabled(!TweensPreivew_In_State.boolValue);
                if (XGUI.layout_button(
                tooltip: !TweensPreivew_Out_State.boolValue ? "元素 - 退出 - 播放" : "元素 - 退出 - 停止",
                tex_release: !TweensPreivew_Out_State.boolValue ? prw_play_out_r : prw_play_out_p,
                tex_press: !TweensPreivew_Out_State.boolValue ? prw_play_out_p : prw_play_out_p,
                tex_gui_color: Color.white,
                border: new RectOffset(0, 0, 0, 0),
                width: 14,
                height: 14))
                {
                    if (!TweensPreivew_Out_State.boolValue)
                        ElementTweens_Preview_Out_Play();
                    else
                        ElementTweens_Preview_Out_Stop();
                }
                XGUI.SetEnabled(true);
                #endregion

                XGUI.layout_group_end(type: XGUIContainerType.Horizontal);

                TweensPreivew_In_State.serializedObject.ApplyModifiedProperties();
                TweensPreivew_Out_State.serializedObject.ApplyModifiedProperties();
                #endregion

                XGUI.layout_seperator(
                        thickness: 1,
                        color: XHud_Dashboard.Theme_SeperateLine,
                        margin: new RectOffset(15, 15, 5, 25));

                #region 选项          
                XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIFilled.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.Theme_Group,
                    title: "选项",
                    title_text_color: XHud_Dashboard.Theme_Primary,
                    title_size: XGUIFontSize.M,
                    title_clipping: TextClipping.Clip,
                    title_offset: new Vector2(10, 0),
                    absolute_margin: true,
                    absolute_padding: true,
                    margin: new RectOffset(0, 0, 20, 15),
                    padding: new RectOffset(15, 0, 20, 20));

                DrawToggle("包含子级图元", PreviewIncludePrimitivesTween, 140, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, stroptions_include, (b) => { });
                DrawToggle("指定位置 - 生成", Crc_Preview_SetPosition, 140, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, stroptions_enabled, (b) => { });
                DrawToggle("指定位置 - 回收", Rec_Preview_SetPosition, 140, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, stroptions_enabled, (b) => { });

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                #endregion

                #region 参数
                if (Crc_Preview_SetPosition.boolValue || Rec_Preview_SetPosition.boolValue)
                {
                    XGUI.layout_group_start(
                        type: XGUIContainerType.Vertical,
                        bg_fill: XGUIFilled.缺口纯色边框,
                        bg_color: XGUIColor.亮白,
                        bg_color_gui: XHud_Dashboard.Theme_Group,
                        title: "参数",
                        title_text_color: XHud_Dashboard.Theme_Primary,
                        title_size: XGUIFontSize.M,
                        title_clipping: TextClipping.Clip,
                        title_offset: new Vector2(10, 0),
                        absolute_margin: true,
                        absolute_padding: true,
                        margin: new RectOffset(0, 0, 10, 15),
                        padding: new RectOffset(15, 10, 20, 20));

                    #region 目标位置
                    if (Crc_Preview_SetPosition.boolValue)
                    {
                        XGUI.layout_property_field(
                            title: "目标位置",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 90,
                            prop: Crc_Preview_Position,
                            prop_margin: new RectOffset(0, 0, 5, 0));
                    }
                    #endregion

                    #region 目标位置
                    if (Rec_Preview_SetPosition.boolValue)
                    {
                        XGUI.layout_property_field(
                            title: "目标位置",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 90,
                            prop: Rec_Preview_Position,
                            prop_margin: new RectOffset(0, 0, 5, 0));
                    }
                    #endregion

                    XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                }
                #endregion

                #region 动效
                XGUI.layout_group_start(
                     type: XGUIContainerType.Vertical,
                        bg_fill: XGUIFilled.缺口纯色边框,
                        bg_color: XGUIColor.亮白,
                        bg_color_gui: XHud_Dashboard.Theme_Group,
                        title: "动效",
                        title_text_color: XHud_Dashboard.Theme_Primary,
                        title_size: XGUIFontSize.M,
                        title_clipping: TextClipping.Clip,
                        title_offset: new Vector2(10, 0),
                        absolute_margin: true,
                        absolute_padding: true,
                        margin: new RectOffset(0, 0, 10, 15),
                        padding: new RectOffset(15, 10, 20, 20));

                #region 生成
                #region 模版库 - 生成
                if (HudManager.Hud_Motions != null)
                {
                    //确保动效库不是空的
                    if (HudManager.Hud_Motions.ElementMotionList != null && HudManager.Hud_Motions.ElementMotionList.Count > 0)
                    {
                        //动效列表
                        string[] motnames = HudManager.Hud_Motions.ElementMotion_GetAllName_With_Create();
                        XGUI.layout_string_popup(
                            title: "生成",
                            title_width: 60,
                            title_size: XGUIFontSize.M,
                            title_anchor: TextAnchor.MiddleLeft,
                            prop: Crc_Lib_Name,
                            options: motnames,
                            opt_text_size: XGUIFontSize.M,
                            opt_text_color: Color.black,
                            opt_text_padding: new RectOffset(10, 10, 0, 0),
                            opt_anchor: TextAnchor.MiddleCenter,
                            opt_font_style: FontStyle.Normal,
                            opt_bg_fill: XGUIFilled.实体,
                            opt_bg_color: XGUIColor.亮白,
                            opt_bg_color_gui: XHud_Dashboard.Theme_Primary,
                            icon_arrow_color: Color.black,
                            margin: new RectOffset(0, 0, 5, 5),
                            padding: new RectOffset(5, 5, 0, 0),
                            title_margin: new RectOffset(0, 0, 0, 0),
                            act_on_changed: (value) =>
                            {
                                Crc_Lib_Name.stringValue = value;
                                Crc_Lib_Name.serializedObject.ApplyModifiedProperties();

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
                                CreateArgs.FindPropertyRelative("MotionAnimateEndState").enumValueIndex = (int)crc.MotionAnimateEndState;
                                CreateArgs.serializedObject.ApplyModifiedProperties();
                            });

                        XGUI.layout_space(12);

                        #region 保存 & 定位模板
                        XGUI.layout_group_start(
                            type: XGUIContainerType.Horizontal,
                            title_clipping: TextClipping.Clip,
                            absolute_padding: true,
                            absolute_margin: true,
                            margin: new RectOffset(0, 0, 0, 0),
                            padding: new RectOffset(5, 10, 0, 0));

                        if (XGUI.layout_button(
                            tooltip: "保存",
                            tex_release: save_r,
                            tex_press: save_p,
                            tex_gui_color: Color.white,
                            border: new RectOffset(0, 0, 0, 0),
                            width: save_r.width,
                            height: save_r.height))
                        {
                            OpenParameterSetter(HudElementMotionType.Creator);
                            return;
                        }

                        GUILayout.FlexibleSpace();

                        if (XGUI.layout_button(
                          tooltip: "定位",
                          tex_release: locate_r,
                          tex_press: locate_p,
                          tex_gui_color: Color.white,
                          border: new RectOffset(0, 0, 0, 0),
                          width: locate_r.width,
                          height: locate_r.height))
                        {
                            if (!HudManager.Hud_Motions.ElementMotion_IsExist(Crc_Lib_Name.stringValue))
                                return;
                            Editor_XHud_MenuItemsAction_OpenLibrary.open_elementmotion();
                            HudManager.Hud_Motions.ElementMotionLibrary_Location(Crc_Lib_Name.stringValue);
                            return;
                        }

                        GUILayout.FlexibleSpace();

                        if (XGUI.layout_button(
                          tooltip: "重置",
                          tex_release: reset_r,
                          tex_press: reset_p,
                          tex_gui_color: Color.white,
                          border: new RectOffset(0, 0, 0, 0),
                          width: reset_r.width,
                          height: reset_r.height))
                        {
                            XGUI.dialog(
                                type: XGUIDialogType.警告,
                                windowtitle: "XHud - 元素消息",
                                title: "重置动效参数",
                                msg: "确定要将动效参数重置吗？您将丢失当前的动效参数！",
                                ok: "重置",
                                cancel: "暂不",
                                PrimaryIndex: 0,
                                usemodal: true,
                                themecolor: XHud_Dashboard.Theme_Primary,
                                on_selected: (d) =>
                                {
                                    if (d == "重置")
                                    {
                                        Crc_Lib_Name.stringValue = null;
                                        Crc_Lib_Name.serializedObject.ApplyModifiedProperties();
                                        ResetMotionParams("c");
                                    }
                                });
                            return;
                        }

                        XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
                        #endregion
                    }
                    else
                    {
                        XGUI.layout_helpbox(
                            state: XGUIHelboxState.警告,
                            title_text: "未在动效库中发现任何动效资源，请先为其添加动效资源！",
                            title_size: XGUIFontSize.M,
                            title_style: FontStyle.Normal,
                            title_color: Color.white * 0.75f);
                    }
                }
                else
                {
                    XGUI.layout_helpbox(
                          state: XGUIHelboxState.警告,
                          title_text: "XHud管理器中未指定动效库，请先配置动效库！",
                          title_size: XGUIFontSize.M,
                          title_style: FontStyle.Normal,
                          title_color: Color.white * 0.75f);
                }
                #endregion

                XGUI.layout_seperator(
                   thickness: 1,
                   color: XHud_Dashboard.Theme_SeperateLine,
                   margin: new RectOffset(15, 15, 18, 15));

                #region 参数 - 生成
                SerializedProperty sp_def_create_anchor = CreateArgs.FindPropertyRelative("anchor");

                #region 锚点
                XGUI.layout_property_field(
                title: "锚点",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.Theme_Primary,
                title_width: 90,
                prop: sp_def_create_anchor,
                prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    absolute_padding: true,
                    absolute_margin: true,
                    margin: new RectOffset(0, 0, 10, 0),
                    padding: new RectOffset(15, 0, 0, 0));

                #region 位移
                def_create_fold_move = EditorGUILayout.Foldout(def_create_fold_move, "位移", true);

                if (def_create_fold_move)
                {
                    SerializedProperty sp_def_create_move_type = CreateArgs.FindPropertyRelative("Movement.Movement");
                    XGUI.layout_property_field(
                        title: "方式",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 90,
                        prop: sp_def_create_move_type,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    SerializedProperty sp_def_create_move_dis = CreateArgs.FindPropertyRelative("Movement.Distance");
                    XGUI.layout_property_field(
                        title: "距离",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 90,
                        prop: sp_def_create_move_dis,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    SerializedProperty sp_def_create_move_dur = CreateArgs.FindPropertyRelative("Movement.Duration");
                    XGUI.layout_property_field(
                        title: "耗时",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 90,
                        prop: sp_def_create_move_dur,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    SerializedProperty sp_def_create_move_delay = CreateArgs.FindPropertyRelative("Movement.Delay");
                    XGUI.layout_property_field(
                        title: "延迟",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 90,
                        prop: sp_def_create_move_delay,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    SerializedProperty sp_def_create_move_curve = CreateArgs.FindPropertyRelative("Movement.Curve");
                    XGUI.layout_property_field(
                        title: "曲线",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 90,
                        prop: sp_def_create_move_curve,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    SerializedProperty sp_def_create_move_ease = CreateArgs.FindPropertyRelative("Movement.Ease");
                    XGUI.layout_property_field(
                        title: "缓动",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 90,
                        prop: sp_def_create_move_ease,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                }
                #endregion

                XGUI.layout_space(5);

                #region 旋转
                def_create_fold_rotate = EditorGUILayout.Foldout(def_create_fold_rotate, "旋转", true);

                if (def_create_fold_rotate)
                {
                    SerializedProperty sp_def_create_rot_type = CreateArgs.FindPropertyRelative("Rotation.Rotation");
                    XGUI.layout_property_field(
                      title: "方式",
                      title_size: XGUIFontSize.M,
                      title_hover_color: XHud_Dashboard.Theme_Primary,
                      title_width: 90,
                      prop: sp_def_create_rot_type,
                      prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    SerializedProperty sp_def_create_rot_deg = CreateArgs.FindPropertyRelative("Rotation.Degree");
                    XGUI.layout_property_field(
                      title: "角度",
                      title_size: XGUIFontSize.M,
                      title_hover_color: XHud_Dashboard.Theme_Primary,
                      title_width: 90,
                      prop: sp_def_create_rot_deg,
                      prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    SerializedProperty sp_def_create_rot_dur = CreateArgs.FindPropertyRelative("Rotation.Duration");
                    XGUI.layout_property_field(
                      title: "耗时",
                      title_size: XGUIFontSize.M,
                      title_hover_color: XHud_Dashboard.Theme_Primary,
                      title_width: 90,
                      prop: sp_def_create_rot_dur,
                      prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    SerializedProperty sp_def_create_rot_delay = CreateArgs.FindPropertyRelative("Rotation.Delay");
                    XGUI.layout_property_field(
                      title: "延迟",
                      title_size: XGUIFontSize.M,
                      title_hover_color: XHud_Dashboard.Theme_Primary,
                      title_width: 90,
                      prop: sp_def_create_rot_delay,
                      prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    SerializedProperty sp_def_create_rot_curve = CreateArgs.FindPropertyRelative("Rotation.Curve");
                    XGUI.layout_property_field(
                      title: "曲线",
                      title_size: XGUIFontSize.M,
                      title_hover_color: XHud_Dashboard.Theme_Primary,
                      title_width: 90,
                      prop: sp_def_create_rot_curve,
                      prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    SerializedProperty sp_def_create_rot_ease = CreateArgs.FindPropertyRelative("Rotation.Ease");
                    XGUI.layout_property_field(
                      title: "缓动",
                      title_size: XGUIFontSize.M,
                      title_hover_color: XHud_Dashboard.Theme_Primary,
                      title_width: 90,
                      prop: sp_def_create_rot_ease,
                      prop_margin: new RectOffset(0, 0, 5, 0));
                }
                #endregion

                XGUI.layout_space(5);

                #region 透明度                   
                def_create_fold_alpha = EditorGUILayout.Foldout(def_create_fold_alpha, "透明度", true);

                if (def_create_fold_alpha)
                {
                    SerializedProperty sp_def_create_alpha_type = CreateArgs.FindPropertyRelative("Alpha.Duration");
                    XGUI.layout_property_field(
                        title: "耗时",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 90,
                        prop: sp_def_create_alpha_type,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    SerializedProperty sp_def_create_alpha_delay = CreateArgs.FindPropertyRelative("Alpha.Delay");
                    XGUI.layout_property_field(
                        title: "延迟",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 90,
                        prop: sp_def_create_alpha_delay,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    SerializedProperty sp_def_create_alpha_curve = CreateArgs.FindPropertyRelative("Alpha.Curve");
                    XGUI.layout_property_field(
                        title: "曲线",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 90,
                        prop: sp_def_create_alpha_curve,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    SerializedProperty sp_def_create_alpha_ease = CreateArgs.FindPropertyRelative("Alpha.Ease");
                    XGUI.layout_property_field(
                        title: "缓动",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 90,
                        prop: sp_def_create_alpha_ease,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                }
                #endregion

                XGUI.editor_layout_group_end(XGUIContainerType.Vertical);

                #region 动效结束时机
                XGUI.ChangedCheck_Start();
                XGUI.layout_property_field(
                       title: "动效结束时机",
                       title_size: XGUIFontSize.M,
                       title_hover_color: XHud_Dashboard.Theme_Primary,
                       title_width: 90,
                       prop: CreateArgs_MotionAnimateEndState,
                       prop_margin: new RectOffset(0, 0, 15, 0));
                if (XGUI.ChangedCheck_End())
                {
                    MotionAnimateEndState state = (MotionAnimateEndState)CreateArgs_MotionAnimateEndState.enumValueIndex;
                    switch (state)
                    {
                        case MotionAnimateEndState.以_移动为准:
                            HudMotion_Movement m = (HudMotion_Movement)CreateArgs.FindPropertyRelative("Movement.Movement").enumValueIndex;
                            if (m == HudMotion_Movement.A_无运动)
                            {
                                XGUI.dialog(
                                    type: XGUIDialogType.警告,
                                    windowtitle: "XHud - 元素消息",
                                    title: "设定动画结束时机",
                                    msg: $"当前位移方式为 <color={hexcol}> A_无运动 </color>，将发生<color={hexcol}> 动效动画无法正常结束</color>从而导致<color={hexcol}> 相应的事件和动作委托 </color>不能正确触发的情况 </color>！请将<color={hexcol}> 位移方式 </color>改为<color={hexcol}> 非无运动方式 </color>！",
                                    ok: "明白",
                                    PrimaryIndex: 0,
                                    usemodal: true,
                                    themecolor: XHud_Dashboard.Theme_Primary);

                                CreateArgs_MotionAnimateEndState.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
                            }
                            break;
                        case MotionAnimateEndState.以_旋转为准:
                            HudMotion_Rotation r = (HudMotion_Rotation)CreateArgs.FindPropertyRelative("Rotation.Rotation").enumValueIndex;
                            if (r == HudMotion_Rotation.A_无旋转)
                            {
                                XGUI.dialog(
                                    type: XGUIDialogType.警告,
                                    windowtitle: "XHud - 元素消息",
                                    title: "设定动画结束时机",
                                    msg: $"当前旋转方式为 <color={hexcol}> A_无旋转 </color>，将发生<color={hexcol}> 动效动画无法正常结束</color>从而导致<color={hexcol}> 相应的事件和动作委托 </color>不能正确触发的情况 </color>！请将<color={hexcol}> 旋转方式 </color>改为<color={hexcol}> 非无旋转方式 </color>！",
                                    ok: "明白",
                                    PrimaryIndex: 0,
                                    usemodal: true,
                                    themecolor: XHud_Dashboard.Theme_Primary);

                                CreateArgs_MotionAnimateEndState.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
                            }
                            break;
                    }
                }
                #endregion
                #endregion
                #endregion

                XGUI.layout_seperator(
                    thickness: 3,
                    color: XHud_Dashboard.Theme_SeperateLine * 1.5f,
                    margin: new RectOffset(15, 15, 18, 15));

                #region 回收
                #region 模版库 - 回收
                if (HudManager.Hud_Motions != null)
                {
                    //确保动效库不是空的
                    if (HudManager.Hud_Motions.ElementMotionList != null && HudManager.Hud_Motions.ElementMotionList.Count > 0)
                    {
                        //动效列表
                        string[] motnames = HudManager.Hud_Motions.ElementMotion_GetAllName_With_Recycle();
                        XGUI.layout_string_popup(
                            title: "回收",
                            title_width: 60,
                            title_size: XGUIFontSize.M,
                            title_anchor: TextAnchor.MiddleLeft,
                            prop: Rec_Lib_Name,
                            options: motnames,
                            opt_text_size: XGUIFontSize.M,
                            opt_text_color: Color.black,
                            opt_text_padding: new RectOffset(10, 10, 0, 0),
                            opt_anchor: TextAnchor.MiddleCenter,
                            opt_font_style: FontStyle.Normal,
                            opt_bg_fill: XGUIFilled.实体,
                            opt_bg_color: XGUIColor.亮白,
                            opt_bg_color_gui: XHud_Dashboard.Theme_Primary,
                            icon_arrow_color: Color.black,
                            margin: new RectOffset(0, 0, 5, 5),
                            padding: new RectOffset(5, 5, 0, 0),
                            title_margin: new RectOffset(0, 0, 0, 0),
                            act_on_changed: (value) =>
                            {
                                Rec_Lib_Name.stringValue = value;
                                Rec_Lib_Name.serializedObject.ApplyModifiedProperties();

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
                                RecycleArgs.FindPropertyRelative("MotionAnimateEndState").enumValueIndex = (int)rec.MotionAnimateEndState;
                                RecycleArgs.serializedObject.ApplyModifiedProperties();
                            });

                        XGUI.layout_space(12);

                        #region 保存 & 定位模板
                        XGUI.layout_group_start(
                            type: XGUIContainerType.Horizontal,
                            title_clipping: TextClipping.Clip,
                            absolute_padding: true,
                            absolute_margin: true,
                            margin: new RectOffset(0, 0, 0, 0),
                            padding: new RectOffset(5, 10, 0, 0));

                        if (XGUI.layout_button(
                            tooltip: "保存",
                            tex_release: save_r,
                            tex_press: save_p,
                            tex_gui_color: Color.white,
                            border: new RectOffset(0, 0, 0, 0),
                            width: save_r.width,
                            height: save_r.height))
                        {
                            OpenParameterSetter(HudElementMotionType.Recycler);
                            return;
                        }

                        GUILayout.FlexibleSpace();

                        if (XGUI.layout_button(
                          tooltip: "定位",
                          tex_release: locate_r,
                          tex_press: locate_p,
                          tex_gui_color: Color.white,
                          border: new RectOffset(0, 0, 0, 0),
                          width: locate_r.width,
                          height: locate_r.height))
                        {
                            if (!HudManager.Hud_Motions.ElementMotion_IsExist(Rec_Lib_Name.stringValue))
                                return;
                            Editor_XHud_MenuItemsAction_OpenLibrary.open_elementmotion();
                            HudManager.Hud_Motions.ElementMotionLibrary_Location(Rec_Lib_Name.stringValue);
                            return;
                        }

                        GUILayout.FlexibleSpace();

                        if (XGUI.layout_button(
                          tooltip: "重置",
                          tex_release: reset_r,
                          tex_press: reset_p,
                          tex_gui_color: Color.white,
                          border: new RectOffset(0, 0, 0, 0),
                          width: reset_r.width,
                          height: reset_r.height))
                        {
                            XGUI.dialog(
                                type: XGUIDialogType.警告,
                                windowtitle: "XHud - 元素消息",
                                title: "重置动效参数",
                                msg: "确定要将动效参数重置吗？您将丢失当前的动效参数！",
                                ok: "重置",
                                cancel: "暂不",
                                PrimaryIndex: 0,
                                usemodal: true,
                                themecolor: XHud_Dashboard.Theme_Primary,
                                on_selected: (d) =>
                                {
                                    if (d == "重置")
                                    {
                                        ResetMotionParams("r");
                                    }
                                });
                            return;
                        }

                        XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
                        #endregion
                    }
                    else
                    {
                        XGUI.layout_helpbox(
                            state: XGUIHelboxState.警告,
                            title_text: "未在动效库中发现任何动效资源，请先为其添加动效资源！",
                            title_size: XGUIFontSize.M,
                            title_style: FontStyle.Normal,
                            title_color: Color.white * 0.75f);
                    }
                }
                else
                {
                    XGUI.layout_helpbox(
                          state: XGUIHelboxState.警告,
                          title_text: "XHud管理器中未指定动效库，请先配置动效库！",
                          title_size: XGUIFontSize.M,
                          title_style: FontStyle.Normal,
                          title_color: Color.white * 0.75f);
                }
                #endregion

                XGUI.layout_seperator(
                    thickness: 1,
                    color: XHud_Dashboard.Theme_SeperateLine,
                    margin: new RectOffset(15, 15, 18, 15));

                #region 参数 - 回收
                XGUI.layout_group_start(
                  type: XGUIContainerType.Vertical,
                  absolute_padding: true,
                  absolute_margin: true,
                  margin: new RectOffset(0, 0, 10, 0),
                  padding: new RectOffset(15, 0, 0, 0));

                #region 位移          
                def_recycle_fold_move = EditorGUILayout.Foldout(def_recycle_fold_move, "位移", true);

                if (def_recycle_fold_move)
                {
                    SerializedProperty sp_def_recycle_move_type = RecycleArgs.FindPropertyRelative("Movement.Movement");
                    XGUI.layout_property_field(
                        title: "方式",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 90,
                        prop: sp_def_recycle_move_type,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    SerializedProperty sp_def_recycle_move_dis = RecycleArgs.FindPropertyRelative("Movement.Distance");
                    XGUI.layout_property_field(
                        title: "距离",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 90,
                        prop: sp_def_recycle_move_dis,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    SerializedProperty sp_def_recycle_move_dur = RecycleArgs.FindPropertyRelative("Movement.Duration");
                    XGUI.layout_property_field(
                        title: "耗时",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 90,
                        prop: sp_def_recycle_move_dur,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    SerializedProperty sp_def_recycle_move_delay = RecycleArgs.FindPropertyRelative("Movement.Delay");
                    XGUI.layout_property_field(
                        title: "延迟",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 90,
                        prop: sp_def_recycle_move_delay,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    SerializedProperty sp_def_recycle_move_curve = RecycleArgs.FindPropertyRelative("Movement.Curve");
                    XGUI.layout_property_field(
                        title: "曲线",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 90,
                        prop: sp_def_recycle_move_curve,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    SerializedProperty sp_def_recycle_move_ease = RecycleArgs.FindPropertyRelative("Movement.Ease");
                    XGUI.layout_property_field(
                        title: "缓动",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 90,
                        prop: sp_def_recycle_move_ease,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                }
                #endregion

                XGUI.layout_space(5);

                #region 旋转      
                def_recycle_fold_rotate = EditorGUILayout.Foldout(def_recycle_fold_rotate, "旋转", true);

                if (def_recycle_fold_rotate)
                {
                    SerializedProperty sp_def_recycle_rot_type = RecycleArgs.FindPropertyRelative("Rotation.Rotation");
                    XGUI.layout_property_field(
                        title: "方式",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 90,
                        prop: sp_def_recycle_rot_type,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    SerializedProperty sp_def_recycle_rot_deg = RecycleArgs.FindPropertyRelative("Rotation.Degree");
                    XGUI.layout_property_field(
                        title: "角度",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 90,
                        prop: sp_def_recycle_rot_deg,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    SerializedProperty sp_def_recycle_rot_dur = RecycleArgs.FindPropertyRelative("Rotation.Duration");
                    XGUI.layout_property_field(
                        title: "耗时",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 90,
                        prop: sp_def_recycle_rot_dur,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    SerializedProperty sp_def_recycle_rot_delay = RecycleArgs.FindPropertyRelative("Rotation.Delay");
                    XGUI.layout_property_field(
                        title: "延迟",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 90,
                        prop: sp_def_recycle_rot_delay,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    SerializedProperty sp_def_recycle_rot_curve = RecycleArgs.FindPropertyRelative("Rotation.Curve");
                    XGUI.layout_property_field(
                        title: "曲线",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 90,
                        prop: sp_def_recycle_rot_curve,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    SerializedProperty sp_def_recycle_rot_ease = RecycleArgs.FindPropertyRelative("Rotation.Ease");
                    XGUI.layout_property_field(
                        title: "缓动",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 90,
                        prop: sp_def_recycle_rot_ease,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                }
                #endregion

                XGUI.layout_space(5);

                #region 透明度      
                def_recycle_fold_alpha = EditorGUILayout.Foldout(def_recycle_fold_alpha, "透明度", true);

                if (def_recycle_fold_alpha)
                {
                    SerializedProperty sp_def_recycle_alpha_type = RecycleArgs.FindPropertyRelative("Alpha.Duration");
                    XGUI.layout_property_field(
                        title: "耗时",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 90,
                        prop: sp_def_recycle_alpha_type,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    SerializedProperty sp_def_recycle_alpha_delay = RecycleArgs.FindPropertyRelative("Alpha.Delay");
                    XGUI.layout_property_field(
                        title: "延迟",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 90,
                        prop: sp_def_recycle_alpha_delay,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    SerializedProperty sp_def_recycle_alpha_curve = RecycleArgs.FindPropertyRelative("Alpha.Curve");
                    XGUI.layout_property_field(
                        title: "曲线",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 90,
                        prop: sp_def_recycle_alpha_curve,
                        prop_margin: new RectOffset(0, 0, 5, 0));

                    XGUI.layout_space(5);

                    SerializedProperty sp_def_recycle_alpha_ease = RecycleArgs.FindPropertyRelative("Alpha.Ease");
                    XGUI.layout_property_field(
                        title: "缓动",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 90,
                        prop: sp_def_recycle_alpha_ease,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                }
                #endregion

                XGUI.editor_layout_group_end(XGUIContainerType.Vertical);

                #region 动效结束时机
                XGUI.ChangedCheck_Start();
                XGUI.layout_property_field(
                    title: "动效结束时机",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: RecycleArgs_MotionAnimateEndState,
                    prop_margin: new RectOffset(0, 0, 15, 0));
                if (XGUI.ChangedCheck_End())
                {
                    MotionAnimateEndState state = (MotionAnimateEndState)RecycleArgs_MotionAnimateEndState.enumValueIndex;
                    switch (state)
                    {
                        case MotionAnimateEndState.以_移动为准:
                            HudMotion_Movement m = (HudMotion_Movement)RecycleArgs.FindPropertyRelative("Movement.Movement").enumValueIndex;
                            if (m == HudMotion_Movement.A_无运动)
                            {
                                XGUI.dialog(
                                    type: XGUIDialogType.警告,
                                    windowtitle: "XHud - 元素消息",
                                    title: "设定动画结束时机",
                                    msg: $"当前位移方式为 <color={hexcol}> A_无运动 </color>，将发生<color={hexcol}> 动效动画无法正常结束</color>从而导致<color={hexcol}> 相应的事件和动作委托 </color>不能正确触发的情况 </color>！请将<color={hexcol}> 位移方式 </color>改为<color={hexcol}> 非无运动方式 </color>！",
                                    ok: "明白",
                                    PrimaryIndex: 0,
                                    usemodal: true,
                                    themecolor: XHud_Dashboard.Theme_Primary);

                                RecycleArgs_MotionAnimateEndState.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
                            }
                            break;
                        case MotionAnimateEndState.以_旋转为准:
                            HudMotion_Rotation r = (HudMotion_Rotation)RecycleArgs.FindPropertyRelative("Rotation.Rotation").enumValueIndex;
                            if (r == HudMotion_Rotation.A_无旋转)
                            {
                                XGUI.dialog(
                                    type: XGUIDialogType.警告,
                                    windowtitle: "XHud - 元素消息",
                                    title: "设定动画结束时机",
                                    msg: $"当前旋转方式为 <color={hexcol}> A_无旋转 </color>，将发生<color={hexcol}> 动效动画无法正常结束</color>从而导致<color={hexcol}> 相应的事件和动作委托 </color>不能正确触发的情况 </color>！请将<color={hexcol}> 旋转方式 </color>改为<color={hexcol}> 非无旋转方式 </color>！",
                                    ok: "明白",
                                    PrimaryIndex: 0,
                                    usemodal: true,
                                    themecolor: XHud_Dashboard.Theme_Primary);

                                RecycleArgs_MotionAnimateEndState.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
                            }
                            break;
                    }
                }
                #endregion
                #endregion
                #endregion

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                #endregion

                #region 添加运行时预览组件
                if (XGUI.layout_button(
                    text: "添加运行时预览组件",
                    tooltip: "",
                    bg_fill: XGUIFilled.实体,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.Theme_Primary,
                    button_text_color: XGUI_Utilitys.ColorBrightness_LimiteGet(XHud_Dashboard.Theme_Primary) ? Color.black : Color.white,
                    press_fill: XGUIFilled.实体,
                    press_color: XGUIColor.深空灰,
                    press_text_color: Color.white,
                    font_size: XGUIFontSize.M,
                    height: 30,
                    anchor: TextAnchor.MiddleCenter,
                    margin: new RectOffset(0, 0, 0, 0),
                    padding: new RectOffset(0, 0, 0, 0),
                    button_text_font: XGUI.GetFont("xg-medium")))
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
                #endregion
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 参数          
            BaseScript.fold_param = XGUI.layout_group_start(
               type: XGUIContainerType.Vertical,
               bg_fill: XGUIFilled.缺口纯色边框,
               bg_color: XGUIColor.亮白,
               bg_color_gui: XHud_Dashboard.Theme_Group,
               title: "参数",
               title_size: XGUIFontSize.M,
               title_text_color: XHud_Dashboard.Theme_Primary,
               title_clipping: TextClipping.Clip,
               padding: new RectOffset(5, 5, 20, 15),
               foldout: BaseScript.fold_param);

            if (!BaseScript.fold_param)
            {
                #region 标识名称
                XGUI.ChangedCheck_Start();
                XGUI.layout_property_field(
                    title: "标识名称",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: Indicator,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                if (XGUI.ChangedCheck_End())
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
                #endregion

                #region 透明度
                XGUI.ChangedCheck_Start();
                XGUI.layout_property_field(
                    title: "透明度",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: Alpha,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                if (XGUI.ChangedCheck_End())
                {
                    ChangeAlpha();
                }
                #endregion

                #region 速率倍增
                XGUI.layout_property_field(
                    title: "速率倍增",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: PrimitivesTweenGlobalDuration,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                #region 当前锚点
                XGUI.SetEnabled(false);
                XGUI.layout_property_field(
                    title: "当前锚点",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: CurrentPivot,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                XGUI.SetEnabled(true);
                #endregion
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 选项
            BaseScript.fold_option = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.Theme_Group,
                title: "选项",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.Theme_Primary,
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: BaseScript.fold_option);

            if (!BaseScript.fold_option)
            {
                #region 状态调试
                DrawToggle("状态调试", DebugState, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, stroptions_debug, (b) => { });
                #endregion

                #region 预览自动停止
                DrawToggle("预览自动停止", AutoKillPreviewTweens, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, stroptions_enabled, (b) =>
                {
                    AutoKillPreviewTweens.boolValue = b;
                    AutoKillPreviewTweens.serializedObject.ApplyModifiedProperties();


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
                });
                #endregion

                #region 预览杀死前先重置
                if (!AutoKillPreviewTweens.boolValue)
                {
                    DrawToggle("预览杀死前先重置", RewindPreviewTweensWithKill, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, new string[2] { "禁用", "启用" }, (b) =>
                {
                    RewindPreviewTweensWithKill.boolValue = b;
                    RewindPreviewTweensWithKill.serializedObject.ApplyModifiedProperties();

                    // 保存配置数据：杀死前重置动画
                    XHud_Dashboard.Set_PreviewOption_RewindPreviewTweensWithKill(RewindPreviewTweensWithKill.boolValue);

                    // 预览器设置：杀死预览动画前重置动画
                    Editor_XTween_Previewer.BeforeKillRewind = RewindPreviewTweensWithKill.boolValue;

                    // 保存XHud元素的动画预览配置数据
                    XHud_Dashboard.ElementPreviewOptionsConfig_Save();

                });
                }
                #endregion

                #region 预览杀死后清空预览器列表
                if (!AutoKillPreviewTweens.boolValue)
                {
                    DrawToggle("预览杀死后清空预览器列表", ClearPreviewTweensWithKill, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, new string[2] { "禁用", "启用" }, (b) =>
                {
                    ClearPreviewTweensWithKill.boolValue = b;
                    ClearPreviewTweensWithKill.serializedObject.ApplyModifiedProperties();

                    // 保存配置数据：杀死后清空预览列表
                    XHud_Dashboard.Set_PreviewOption_ClearPreviewTweensWithKill(ClearPreviewTweensWithKill.boolValue);

                    // 预览器设置：杀死后清空预览列表
                    Editor_XTween_Previewer.AfterKillClear = ClearPreviewTweensWithKill.boolValue;

                    // 保存XHud元素的动画预览配置数据
                    XHud_Dashboard.ElementPreviewOptionsConfig_Save();
                });
                }
                #endregion

                XGUI.layout_seperator(
                    thickness: 1,
                    color: XHud_Dashboard.Theme_SeperateLine,
                    margin: new RectOffset(15, 15, 15, 15));

                #region 图元动画器自动播放
                DrawToggle("图元动画器自动播放", AutoPlayPrimitivesTween, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, stroptions_auto, (b) => { });
                #endregion

                #region 使用设计布局
                DrawToggle("使用设计布局", RMS_Enabled, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, stroptions_enabled, (b) => { });
                #endregion

                #region 说明
                if (RMS_Enabled.boolValue)
                {
                    string msg = "此元素在生成时会优先使用自身记录的设计布局信息来进行定位和位置尺寸的匹配（但前提是在您制作好此元素时需要手动点击\"记录布局信息\"按钮）";

                    XGUI.layout_label(
                        text: msg,
                        bg_fill: XGUIFilled.实体,
                        bg_color: XGUIColor.深空灰,
                        size: XGUIFontSize.S,
                        anchor: TextAnchor.MiddleLeft,
                        text_color: Color.white * 0.85f,
                        offset: new Vector2(0, 0),
                        padding: new RectOffset(10, 10, 10, 10),
                        margin: new RectOffset(0, 0, 6, 0),
                        clipping: clipping,
                        wrap: true,
                        font: XGUI.GetFont("xg-regular"),
                        font_style: FontStyle.Normal);
                }
                #endregion

                XGUI.layout_seperator(
                    thickness: 1,
                    color: XHud_Dashboard.Theme_SeperateLine,
                    margin: new RectOffset(15, 15, 15, 15));

                #region 从库中生成时清空委托
                DrawToggle("从库中生成时清空委托", ClearActions_With_Spawn, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, stroptions_enabled, (b) => { });
                #endregion

                #region 从库中生成时清空事件
                DrawToggle("从库中生成时清空事件", ClearEvents_With_Spawn, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, stroptions_enabled, (b) => { });
                #endregion
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 状态
            string statu_title = "状态";
            if (Targets_Selected())
                statu_title = "状态 - ( 批量模式 )";

            BaseScript.fold_state = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.Theme_Group,
                title: statu_title,
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.Theme_Primary,
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: BaseScript.fold_state);

            if (!BaseScript.fold_state)
            {
                if (!Targets_Selected())
                {
                    #region 使用状态
                    XGUI.layout_state_displayer_text(
                        title: "使用状态",
                        title_size: XGUIFontSize.M,
                        subtitle: (XHudElementCreateState)CreateState.enumValueIndex == XHudElementCreateState.Created ? "已被生成" : "已被回收",
                        subtitle_size: XGUIFontSize.M,
                        subtitle_color: (XHudElementCreateState)CreateState.enumValueIndex == XHudElementCreateState.Created ? XHud_Dashboard.Theme_Primary : Color.gray,
                        margin: new RectOffset(5, 5, 0, 5));
                    #endregion

                    #region 动画相关
                    if (PrimitiveControllerNodes != null && PrimitiveControllerNodes.arraySize > 0)
                    {
                        #region 动画状态
                        XGUI.layout_state_displayer_text(
                          title: "动画状态",
                          title_size: XGUIFontSize.M,
                          subtitle: (XHudElementAnimateState)AnimateState.enumValueIndex == XHudElementAnimateState.Animating ? "动画中" : "静止状态",
                          subtitle_size: XGUIFontSize.M,
                          subtitle_color: AnimateState.enumValueIndex == 1 ? XHud_Dashboard.Theme_Primary : Color.gray,
                          margin: new RectOffset(5, 5, 0, 5));
                        #endregion

                        #region 最大耗时（速率倍增）
                        XGUI.layout_state_displayer_text(
                        title: "最大耗时<color=#909090>（速率倍增）</color>",
                        title_size: XGUIFontSize.M,
                        subtitle: PrimitivesTweenMaxDuration.floatValue.ToString() + " 秒",
                        subtitle_size: XGUIFontSize.M,
                        subtitle_color: (XHudElementCreateState)CreateState.enumValueIndex == XHudElementCreateState.Created ? XHud_Dashboard.Theme_Primary : Color.white * 0.75f,
                        margin: new RectOffset(5, 5, 0, 5));
                        #endregion

                        #region 最大耗时（XHud 倍增）
                        XGUI.layout_state_displayer_text(
                        title: "最大耗时<color=#909090>（XHud 倍增）</color>",
                        title_size: XGUIFontSize.M,
                        subtitle: (HudManager.DurationMultiply * PrimitivesTweenMaxDuration.floatValue).ToString() + " 秒",
                        subtitle_size: XGUIFontSize.M,
                        subtitle_color: (XHudElementCreateState)CreateState.enumValueIndex == XHudElementCreateState.Created ? XHud_Dashboard.Theme_Primary : Color.white * 0.75f,
                        margin: new RectOffset(5, 5, 0, 5));
                    }
                    #endregion

                    #endregion

                    #region 物体跟踪状态
                    if (ObjectTracker.objectReferenceValue != null)
                    {
                        XHud_ObjectTracker tracker = (XHud_ObjectTracker)ObjectTracker.objectReferenceValue;
                        if (tracker.SelfObject != null && tracker.TargetObject != null)
                        {
                            XGUI.layout_state_displayer_text(
                                title: "物体跟踪状态",
                                title_size: XGUIFontSize.M,
                                subtitle: tracker.SelfObject.name + "  >  " + tracker.TargetObject.name,
                                subtitle_size: XGUIFontSize.M,
                                subtitle_color: XHud_Dashboard.Theme_Primary,
                                margin: new RectOffset(5, 5, 0, 5));
                        }
                    }
                    #endregion

                    #region 元素库源
                    if (Application.isPlaying)
                    {
                        bool lib_exist = false;

                        if (!string.IsNullOrEmpty(OriginPoolName.stringValue))
                        {
                            //遍历元素库集合找到目标元素库
                            foreach (XHud_Library_Element lib in HudManager.Hud_ElementLibrarys)
                            {
                                if (lib.LibraryName == OriginPoolName.stringValue)
                                {
                                    if (lib.ElementLibrary_IsExist(OriginalName.stringValue))
                                    {
                                        lib_exist = true;
                                        break;
                                    }
                                    break;
                                }
                            }
                        }

                        if (lib_exist)
                        {
                            XGUI.layout_state_displayer_btn(
                                title: "元素库源",
                                title_size: XGUIFontSize.M,
                                btn_bg: XGUIFilled.实体,
                                btn_color: XGUIColor.亮白,
                                btn_text: OriginPoolName.stringValue,
                                btn_tooltip: "定位",
                                btn_text_size: XGUIFontSize.M,
                                btn_layout_width: 120,
                                btn_gui_color: Color.white,
                                padding: new RectOffset(0, 0, 0, 0),
                                callback_clicked: () =>
                                {
                                    if (!lib_exist)
                                    {
                                        XGUI.dialog(
                                            type: XGUIDialogType.警告,
                                            windowtitle: "XHud - 元素消息",
                                            title: "定位元素",
                                            msg: $"此元素并非由元素库生成，无法为其进行定位！",
                                            ok: "明白",
                                            PrimaryIndex: 0,
                                            usemodal: true,
                                            themecolor: XHud_Dashboard.Theme_Primary);

                                        return;
                                    }
                                    //打开目标元素库
                                    Editor_XHud_MenuItemsAction_OpenLibrary.open_target_elements(OriginPoolName.stringValue).ElementLibrary_Location_Find(OriginalName.stringValue);
                                });
                        }
                        else
                        {
                            XGUI.layout_state_displayer_text(
                                title: "元素库源",
                                title_size: XGUIFontSize.M,
                                subtitle: "非元素库资源",
                                subtitle_size: XGUIFontSize.M,
                                subtitle_color: XHud_Dashboard.Theme_Primary,
                                margin: new RectOffset(5, 5, 0, 5));
                        }
                    }
                    else
                    {
                        //编辑器模式下，通过遍历所有元素库来找到当前这个元素在哪个库里
                    }
                    #endregion
                }
                else
                {
                    #region 控制区
                    XGUI.layout_group_start(
                        type: XGUIContainerType.Horizontal,
                        bg_fill: XGUIFilled.透明,
                        bg_color: XGUIColor.亮白,
                        bg_color_gui: XHud_Dashboard.Theme_Group,
                        absolute_margin: true,
                        absolute_padding: true,
                        margin: new RectOffset(0, 0, 0, 0),
                        padding: new RectOffset(15, 15, 5, 10));

                    #region 标题按钮
                    if (XGUI.layout_button(
                        text: $"{SelectedObjects[ElementStatu_Index].name} ( {SelectedObjects[ElementStatu_Index].Indicator} )",
                        tooltip: "",
                        bg_fill: XGUIFilled.透明,
                        button_text_color: Color.gray,
                        press_fill: XGUIFilled.透明,
                        press_text_color: XHud_Dashboard.Theme_Primary,
                        font_size: XGUIFontSize.M,
                        anchor: TextAnchor.MiddleLeft,
                        margin: new RectOffset(0, 0, 0, 0),
                        padding: new RectOffset(0, 0, 0, 0),
                        layout_width: 150,
                        button_text_font: XGUI.GetFont("xg-medium")))
                    {
                        EditorGUIUtility.PingObject(SelectedObjects[ElementStatu_Index]);
                    }
                    #endregion

                    XGUI.layout_flexspace();

                    #region 上一个
                    if (XGUI.layout_button(
                        tooltip: "上一个",
                        tex_release: left_arrow_r,
                        tex_press: left_arrow_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: 12,
                        height: 12))
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
                    #endregion

                    XGUI.layout_space(20);

                    #region 下一个
                    if (XGUI.layout_button(
                        tooltip: "下一个",
                        tex_release: right_arrow_r,
                        tex_press: right_arrow_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: 12,
                        height: 12))
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
                    #endregion

                    XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
                    #endregion

                    XGUI.layout_seperator(
                            thickness: 1,
                            color: XHud_Dashboard.Theme_SeperateLine,
                            margin: new RectOffset(15, 15, 5, 25));

                    #region 使用状态
                    XGUI.layout_state_displayer_text(
                        title: "使用状态",
                        title_size: XGUIFontSize.M,
                        subtitle: SelectedObjects[ElementStatu_Index].CreateState == XHudElementCreateState.Created ? "已被生成" : "已被回收",
                        subtitle_size: XGUIFontSize.M,
                        subtitle_color: SelectedObjects[ElementStatu_Index].CreateState == XHudElementCreateState.Created ? XHud_Dashboard.Theme_Primary : Color.gray,
                        margin: new RectOffset(5, 5, 0, 5));
                    #endregion

                    #region 动画相关
                    if (PrimitiveControllerNodes != null && PrimitiveControllerNodes.arraySize > 0)
                    {
                        #region 动画状态
                        XGUI.layout_state_displayer_text(
                            title: "动画状态",
                            title_size: XGUIFontSize.M,
                            subtitle: SelectedObjects[ElementStatu_Index].AnimateState == XHudElementAnimateState.Animating ? "动画中" : "静止状态",
                            subtitle_size: XGUIFontSize.M,
                            subtitle_color: (int)SelectedObjects[ElementStatu_Index].AnimateState == 1 ? XHud_Dashboard.Theme_Primary : Color.gray,
                            margin: new RectOffset(5, 5, 0, 5));
                        #endregion

                        #region 最大耗时（速率倍增）
                        XGUI.layout_state_displayer_text(
                            title: "最大耗时<color=#909090>（速率倍增）</color>",
                            title_size: XGUIFontSize.M,
                            subtitle: SelectedObjects[ElementStatu_Index].PrimitivesTweenMaxDuration.ToString() + " 秒",
                            subtitle_size: XGUIFontSize.M,
                            subtitle_color: (XHudElementCreateState)CreateState.enumValueIndex == XHudElementCreateState.Created ? XHud_Dashboard.Theme_Primary : Color.white * 0.75f,
                            margin: new RectOffset(5, 5, 0, 5));
                        #endregion

                        #region 最大耗时（XHud 倍增）
                        XGUI.layout_state_displayer_text(
                            title: "最大耗时<color=#909090>（XHud 倍增）</color>",
                            title_size: XGUIFontSize.M,
                            subtitle: (HudManager.DurationMultiply * SelectedObjects[ElementStatu_Index].PrimitivesTweenMaxDuration).ToString() + " 秒",
                            subtitle_size: XGUIFontSize.M,
                            subtitle_color: (XHudElementCreateState)SelectedObjects[ElementStatu_Index].CreateState == XHudElementCreateState.Created ? XHud_Dashboard.Theme_Primary : Color.white * 0.75f,
                            margin: new RectOffset(5, 5, 0, 5));
                    }
                    #endregion

                    #endregion

                    #region 物体跟踪状态
                    if (ObjectTracker.objectReferenceValue != null)
                    {
                        XHud_ObjectTracker tracker = (XHud_ObjectTracker)SelectedObjects[ElementStatu_Index].ObjectTracker;
                        if (tracker.SelfObject != null && tracker.TargetObject != null)
                        {
                            XGUI.layout_state_displayer_text(
                                title: "物体跟踪状态",
                                title_size: XGUIFontSize.M,
                                subtitle: tracker.SelfObject.name + "  >  " + tracker.TargetObject.name,
                                subtitle_size: XGUIFontSize.M,
                                subtitle_color: XHud_Dashboard.Theme_Primary,
                                margin: new RectOffset(5, 5, 0, 5));
                        }
                    }
                    #endregion

                    #region 元素库源
                    if (Application.isPlaying)
                    {
                        bool lib_exist = false;

                        if (!string.IsNullOrEmpty(SelectedObjects[ElementStatu_Index].OriginPoolName))
                        {
                            //遍历元素库集合找到目标元素库
                            foreach (XHud_Library_Element lib in HudManager.Hud_ElementLibrarys)
                            {
                                if (lib.LibraryName == SelectedObjects[ElementStatu_Index].OriginPoolName)
                                {
                                    if (lib.ElementLibrary_IsExist(SelectedObjects[ElementStatu_Index].OriginalName))
                                    {
                                        lib_exist = true;
                                        break;
                                    }
                                    break;
                                }
                            }
                        }

                        if (lib_exist)
                        {
                            XGUI.layout_state_displayer_btn(
                                title: "元素库源",
                                title_size: XGUIFontSize.M,
                                btn_bg: XGUIFilled.实体,
                                btn_color: XGUIColor.亮白,
                                btn_text: SelectedObjects[ElementStatu_Index].OriginPoolName,
                                btn_tooltip: "定位",
                                btn_text_size: XGUIFontSize.M,
                                btn_layout_width: 120,
                                btn_gui_color: Color.white,
                                padding: new RectOffset(0, 0, 0, 0),
                                callback_clicked: () =>
                                {
                                    if (!lib_exist)
                                    {
                                        XGUI.dialog(
                                            type: XGUIDialogType.警告,
                                            windowtitle: "XHud - 元素消息",
                                            title: "定位元素",
                                            msg: $"此元素并非由元素库生成，无法为其进行定位！",
                                            ok: "明白",
                                            PrimaryIndex: 0,
                                            usemodal: true,
                                            themecolor: XHud_Dashboard.Theme_Primary);

                                        return;
                                    }
                                    //打开目标元素库
                                    Editor_XHud_MenuItemsAction_OpenLibrary.open_target_elements(SelectedObjects[ElementStatu_Index].OriginPoolName).ElementLibrary_Location_Find(SelectedObjects[ElementStatu_Index].OriginalName);
                                });
                        }
                        else
                        {
                            XGUI.layout_state_displayer_text(
                                title: "元素库源",
                                title_size: XGUIFontSize.M,
                                subtitle: "非元素库资源",
                                subtitle_size: XGUIFontSize.M,
                                subtitle_color: XHud_Dashboard.Theme_Primary,
                                margin: new RectOffset(5, 5, 0, 5));
                        }
                    }
                    else
                    {
                        //编辑器模式下，通过遍历所有元素库来找到当前这个元素在哪个库里
                    }
                    #endregion
                }
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region RMS 布局信息
            string rms_lay_title = "RMS 布局信息";
            if (Targets_Selected())
                rms_lay_title = "RMS 布局信息 - ( 批量模式 )";

            BaseScript.fold_rms = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.Theme_Group,
                title: rms_lay_title,
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.Theme_Primary,
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: BaseScript.fold_rms);

            if (!BaseScript.fold_rms)
            {
                if (HudManager != null)
                {
                    string[] nodesName = HudManager.hm_RMS_GetResolutionNodeNames();
                    if (nodesName.Length > 0)
                    {
                        XGUI.layout_helpbox(
                                state: XGUIHelboxState.通知,
                                title_text: "RMS方案列表仅用于切换和预览，在取消选择元素后会自动回到Hud管理器当前选中的RMS方案",
                                title_size: XGUIFontSize.M,
                                title_style: FontStyle.Normal,
                                wrap: true,
                                title_color: Color.white * 0.75f);

                        RMS_Name.stringValue = XGUI.layout_string_popup(
                            title: "R M S 方案",
                            title_width: 120,
                            title_color: Color.white,
                            title_size: XGUIFontSize.M,
                            title_anchor: TextAnchor.MiddleLeft,
                            prop: RMS_Name,
                            options: nodesName,
                            opt_text_size: XGUIFontSize.M,
                            opt_text_color: Color.black,
                            opt_text_padding: new RectOffset(10, 10, 0, 0),
                            opt_anchor: TextAnchor.MiddleLeft,
                            opt_font_style: FontStyle.Normal,
                            opt_bg_fill: XGUIFilled.实体,
                            opt_bg_color: XGUIColor.亮白,
                            opt_bg_color_gui: XHud_Dashboard.Theme_Primary,
                            margin: new RectOffset(0, 0, 5, 5),
                            padding: new RectOffset(5, 5, 0, 0),
                            title_margin: new RectOffset(0, 0, 0, 0),
                            icon_arrow_color: Color.black,
                            act_on_changed: (v) =>
                            {
                                RMS_Name.stringValue = v;
                                RMS_Name.serializedObject.ApplyModifiedProperties();

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
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(BaseScript.gameObject.scene.name))
                                            RMS_Preview(BaseScript, Alpha.floatValue, Vector3.zero, RMS_Name.stringValue, true);
                                    }
                                }
                            });

                        #region RMS方案信息查看
                        if (Targets_Selected())
                        {
                            #region 控制区
                            XGUI.layout_group_start(
                                type: XGUIContainerType.Horizontal,
                                bg_fill: XGUIFilled.透明,
                                bg_color: XGUIColor.亮白,
                                bg_color_gui: XHud_Dashboard.Theme_Group,
                                absolute_margin: true,
                                absolute_padding: true,
                                margin: new RectOffset(0, 0, 0, 0),
                                padding: new RectOffset(15, 15, 5, 10));

                            #region 标题按钮
                            if (XGUI.layout_button(
                                text: $"{SelectedObjects[ElementStatu_Index].name} ( {SelectedObjects[ElementStatu_Index].Indicator} )",
                                tooltip: "",
                                bg_fill: XGUIFilled.透明,
                                button_text_color: Color.gray,
                                press_fill: XGUIFilled.透明,
                                press_text_color: XHud_Dashboard.Theme_Primary,
                                font_size: XGUIFontSize.M,
                                anchor: TextAnchor.MiddleLeft,
                                margin: new RectOffset(0, 0, 0, 0),
                                padding: new RectOffset(0, 0, 0, 0),
                                layout_width: 150,
                                button_text_font: XGUI.GetFont("xg-medium")))
                            {
                                EditorGUIUtility.PingObject(SelectedObjects[ElementStatu_Index]);
                            }
                            #endregion

                            XGUI.layout_flexspace();

                            #region 上一个
                            if (XGUI.layout_button(
                                tooltip: "上一个",
                                tex_release: left_arrow_r,
                                tex_press: left_arrow_p,
                                tex_gui_color: Color.white,
                                border: new RectOffset(0, 0, 0, 0),
                                width: 12,
                                height: 12))
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
                            #endregion

                            XGUI.layout_space(20);

                            #region 下一个
                            if (XGUI.layout_button(
                                tooltip: "下一个",
                                tex_release: right_arrow_r,
                                tex_press: right_arrow_p,
                                tex_gui_color: Color.white,
                                border: new RectOffset(0, 0, 0, 0),
                                width: 12,
                                height: 12))
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
                            #endregion

                            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
                            #endregion

                            XGUI.layout_seperator(
                                    thickness: 1,
                                    color: XHud_Dashboard.Theme_SeperateLine,
                                    margin: new RectOffset(15, 15, 5, 25));

                            if (SelectedObjects[ElementStatu_Index].RMS_LayoutDatas.Count <= 0)
                            {
                                XGUI.layout_label(
                                    text: "暂无对应方案的布局设计数据",
                                    size: XGUIFontSize.S,
                                    anchor: TextAnchor.MiddleCenter,
                                    text_color: Color.white * 0.85f,
                                    offset: new Vector2(0, 0),
                                    padding: new RectOffset(10, 10, 10, 10),
                                    margin: new RectOffset(0, 0, 6, 0),
                                    clipping: clipping,
                                    wrap: true,
                                    font: XGUI.GetFont("xg-regular"),
                                    font_style: FontStyle.Normal);
                            }
                            else
                            {
                                bool exist_lay = false;
                                for (int s = 0; s < SelectedObjects[ElementStatu_Index].RMS_LayoutDatas.Count; s++)
                                {
                                    Element_RMS_LayoutData dat = SelectedObjects[ElementStatu_Index].RMS_LayoutDatas[s];

                                    if (dat.LayoutName == SelectedObjects[ElementStatu_Index].RMS_Name)
                                    {
                                        exist_lay = true;

                                        #region 锚点
                                        XGUI.layout_state_displayer_text(
                                            title: "锚点",
                                            title_size: XGUIFontSize.M,
                                            subtitle: dat.Anchor.ToString(),
                                            subtitle_size: XGUIFontSize.M,
                                            subtitle_color: XHud_Dashboard.Theme_Primary,
                                            margin: new RectOffset(5, 5, 0, 5));
                                        #endregion

                                        #region 最小锚点
                                        XGUI.layout_state_displayer_text(
                                            title: "最小锚点",
                                            title_size: XGUIFontSize.M,
                                            subtitle: dat.AnchorMin.ToString(),
                                            subtitle_size: XGUIFontSize.M,
                                            subtitle_color: XHud_Dashboard.Theme_Primary,
                                            margin: new RectOffset(5, 5, 0, 5));
                                        #endregion

                                        #region 最大锚点
                                        XGUI.layout_state_displayer_text(
                                            title: "最大锚点",
                                            title_size: XGUIFontSize.M,
                                            subtitle: dat.AnchorMax.ToString(),
                                            subtitle_size: XGUIFontSize.M,
                                            subtitle_color: XHud_Dashboard.Theme_Primary,
                                            margin: new RectOffset(5, 5, 0, 5));
                                        #endregion

                                        #region 位置
                                        XGUI.layout_state_displayer_text(
                                            title: "位置",
                                            title_size: XGUIFontSize.M,
                                            subtitle: dat.Position.ToString(),
                                            subtitle_size: XGUIFontSize.M,
                                            subtitle_color: XHud_Dashboard.Theme_Primary,
                                            margin: new RectOffset(5, 5, 0, 5));
                                        #endregion

                                        #region 角度
                                        XGUI.layout_state_displayer_text(
                                            title: "角度",
                                            title_size: XGUIFontSize.M,
                                            subtitle: dat.Euler.ToString(),
                                            subtitle_size: XGUIFontSize.M,
                                            subtitle_color: XHud_Dashboard.Theme_Primary,
                                            margin: new RectOffset(5, 5, 0, 5));
                                        #endregion

                                        #region 缩放
                                        XGUI.layout_state_displayer_text(
                                            title: "缩放",
                                            title_size: XGUIFontSize.M,
                                            subtitle: dat.Scale.ToString(),
                                            subtitle_size: XGUIFontSize.M,
                                            subtitle_color: XHud_Dashboard.Theme_Primary,
                                            margin: new RectOffset(5, 5, 0, 5));
                                        #endregion

                                        #region 轴心
                                        XGUI.layout_state_displayer_text(
                                            title: "轴心",
                                            title_size: XGUIFontSize.M,
                                            subtitle: dat.Pivot.ToString(),
                                            subtitle_size: XGUIFontSize.M,
                                            subtitle_color: XHud_Dashboard.Theme_Primary,
                                            margin: new RectOffset(5, 5, 0, 5));
                                        #endregion
                                    }
                                }

                                if (!exist_lay)
                                {
                                    XGUI.layout_label(
                                        text: $"暂无对应 {SelectedObjects[ElementStatu_Index].RMS_Name} 方案的布局设计数据",
                                        size: XGUIFontSize.S,
                                        anchor: TextAnchor.MiddleCenter,
                                        text_color: Color.white * 0.85f,
                                        offset: new Vector2(0, 0),
                                        padding: new RectOffset(10, 10, 10, 10),
                                        margin: new RectOffset(0, 0, 6, 0),
                                        clipping: clipping,
                                        wrap: true,
                                        font: XGUI.GetFont("xg-regular"),
                                        font_style: FontStyle.Normal);
                                }
                            }
                        }
                        else
                        {
                            if (RMS_LayoutDatas.arraySize <= 0)
                            {
                                XGUI.layout_label(
                                    text: "暂无对应方案的布局设计数据",
                                    size: XGUIFontSize.S,
                                    anchor: TextAnchor.MiddleCenter,
                                    text_color: Color.white * 0.85f,
                                    offset: new Vector2(0, 0),
                                    padding: new RectOffset(10, 10, 10, 10),
                                    margin: new RectOffset(0, 0, 6, 0),
                                    clipping: clipping,
                                    wrap: true,
                                    font: XGUI.GetFont("xg-regular"),
                                    font_style: FontStyle.Normal);
                            }
                            else
                            {
                                for (int i = 0; i < RMS_LayoutDatas.arraySize; i++)
                                {
                                    SerializedProperty sp_item = RMS_LayoutDatas.GetArrayElementAtIndex(i);
                                    SerializedProperty sp_item_Name = sp_item.FindPropertyRelative("LayoutName");

                                    if (sp_item_Name.stringValue == RMS_Name.stringValue)
                                    {
                                        SerializedProperty sp_item_Anchor = sp_item.FindPropertyRelative("Anchor");
                                        SerializedProperty sp_item_Position = sp_item.FindPropertyRelative("Position");
                                        SerializedProperty sp_item_Euler = sp_item.FindPropertyRelative("Euler");
                                        SerializedProperty sp_item_Scale = sp_item.FindPropertyRelative("Scale");
                                        SerializedProperty sp_item_AnchorMin = sp_item.FindPropertyRelative("AnchorMin");
                                        SerializedProperty sp_item_AnchorMax = sp_item.FindPropertyRelative("AnchorMax");
                                        SerializedProperty sp_item_Pivot = sp_item.FindPropertyRelative("Pivot");

                                        #region 锚点
                                        XGUI.layout_state_displayer_text(
                                            title: "锚点",
                                            title_size: XGUIFontSize.M,
                                            subtitle: ((XHudAnchor)sp_item_Anchor.enumValueIndex).ToString(),
                                            subtitle_size: XGUIFontSize.M,
                                            subtitle_color: XHud_Dashboard.Theme_Primary,
                                            margin: new RectOffset(5, 5, 0, 5));
                                        #endregion

                                        #region 最小锚点
                                        XGUI.layout_state_displayer_text(
                                            title: "最小锚点",
                                            title_size: XGUIFontSize.M,
                                            subtitle: sp_item_AnchorMin.vector2Value.ToString(),
                                            subtitle_size: XGUIFontSize.M,
                                            subtitle_color: XHud_Dashboard.Theme_Primary,
                                            margin: new RectOffset(5, 5, 0, 5));
                                        #endregion

                                        #region 最大锚点
                                        XGUI.layout_state_displayer_text(
                                            title: "最大锚点",
                                            title_size: XGUIFontSize.M,
                                            subtitle: sp_item_AnchorMax.vector2Value.ToString(),
                                            subtitle_size: XGUIFontSize.M,
                                            subtitle_color: XHud_Dashboard.Theme_Primary,
                                            margin: new RectOffset(5, 5, 0, 5));
                                        #endregion

                                        #region 位置
                                        XGUI.layout_state_displayer_text(
                                            title: "位置",
                                            title_size: XGUIFontSize.M,
                                            subtitle: sp_item_Position.vector3Value.ToString(),
                                            subtitle_size: XGUIFontSize.M,
                                            subtitle_color: XHud_Dashboard.Theme_Primary,
                                            margin: new RectOffset(5, 5, 0, 5));
                                        #endregion

                                        #region 角度
                                        XGUI.layout_state_displayer_text(
                                            title: "角度",
                                            title_size: XGUIFontSize.M,
                                            subtitle: sp_item_Euler.vector3Value.ToString(),
                                            subtitle_size: XGUIFontSize.M,
                                            subtitle_color: XHud_Dashboard.Theme_Primary,
                                            margin: new RectOffset(5, 5, 0, 5));
                                        #endregion

                                        #region 缩放
                                        XGUI.layout_state_displayer_text(
                                            title: "缩放",
                                            title_size: XGUIFontSize.M,
                                            subtitle: sp_item_Scale.vector3Value.ToString(),
                                            subtitle_size: XGUIFontSize.M,
                                            subtitle_color: XHud_Dashboard.Theme_Primary,
                                            margin: new RectOffset(5, 5, 0, 5));
                                        #endregion

                                        #region 轴心
                                        XGUI.layout_state_displayer_text(
                                            title: "轴心",
                                            title_size: XGUIFontSize.M,
                                            subtitle: sp_item_Pivot.vector2Value.ToString(),
                                            subtitle_size: XGUIFontSize.M,
                                            subtitle_color: XHud_Dashboard.Theme_Primary,
                                            margin: new RectOffset(5, 5, 0, 5));
                                        #endregion

                                        #region 控制按钮
                                        XGUI.layout_group_start(
                                            type: XGUIContainerType.Horizontal,
                                            bg_fill: XGUIFilled.透明,
                                            bg_color: XGUIColor.亮白,
                                            bg_color_gui: XHud_Dashboard.Theme_Group,
                                            absolute_margin: true,
                                            absolute_padding: true,
                                            margin: new RectOffset(0, 0, 0, 0),
                                            padding: new RectOffset(0, 0, 5, 10));

                                        #region 清空所有方案
                                        if (RMS_LayoutDatas.arraySize > 0)
                                        {
                                            if (XGUI.layout_button(
                                            text: "清空所有方案",
                                            tooltip: "清空所有分辨率匹配方案列表",
                                            bg_fill: XGUIFilled.实体,
                                            bg_color: XGUIColor.亮白,
                                            bg_color_gui: Color.red * 0.75f,
                                            button_text_color: Color.white,
                                            press_fill: XGUIFilled.实体,
                                            press_color: XGUIColor.深空灰,
                                            press_text_color: Color.white,
                                            font_size: XGUIFontSize.M,
                                            anchor: TextAnchor.MiddleCenter,
                                            margin: new RectOffset(0, 5, 0, 0),
                                            padding: new RectOffset(0, 0, 0, 0),
                                            //width: ButtonWidth,
                                            height: 20,
                                            button_text_font: XGUI.GetFont("xg-medium")))
                                            {
                                                string res = XGUI.dialog(
                                                    type: XGUIDialogType.警告,
                                                    windowtitle: "XHud - 元素消息",
                                                    title: "RMS方案操作",
                                                    msg: $"如果清空匹配分辨率信息列表，会导致您之前为不同分辨率记录的坐标信息全部清空，请谨慎此操作！",
                                                    ok: "清空",
                                                    cancel: "暂不",
                                                    PrimaryIndex: 0,
                                                    usemodal: true,
                                                    themecolor: XHud_Dashboard.Theme_Primary);

                                                if (res == "清空")
                                                {
                                                    RMS_LayoutDatas.ClearArray();
                                                    RMS_LayoutDatas.serializedObject.ApplyModifiedProperties();
                                                }
                                                return;
                                            }
                                        }
                                        #endregion

                                        //XGUI.layout_flexspace();

                                        #region 移除当前方案
                                        if (RMS_IsExist())
                                        {
                                            if (XGUI.layout_button(
                                                text: "移除当前方案",
                                                tooltip: "移除当前选择的分辨率匹配方案",
                                                bg_fill: XGUIFilled.实体,
                                                bg_color: XGUIColor.亮白,
                                                bg_color_gui: Color.white,
                                                button_text_color: Color.black,
                                                press_fill: XGUIFilled.实体,
                                                press_color: XGUIColor.深空灰,
                                                press_text_color: Color.white,
                                                font_size: XGUIFontSize.M,
                                                anchor: TextAnchor.MiddleCenter,
                                                margin: new RectOffset(5, 0, 0, 0),
                                                padding: new RectOffset(0, 0, 0, 0),
                                                //width: ButtonWidth,
                                                height: 20,
                                                button_text_font: XGUI.GetFont("xg-medium")))
                                            {
                                                string res = XGUI.dialog(
                                                    type: XGUIDialogType.警告,
                                                    windowtitle: "XHud - 元素消息",
                                                    title: "RMS方案操作",
                                                    msg: $"此操作会导致您为当前分辨率匹配的方案会被移除，请谨慎此操作！",
                                                    ok: "清空",
                                                    cancel: "暂不",
                                                    PrimaryIndex: 0,
                                                    usemodal: true,
                                                    themecolor: XHud_Dashboard.Theme_Primary);
                                                if (res == "暂不")
                                                    return;
                                                for (int s = 0; s < RMS_LayoutDatas.arraySize; s++)
                                                {
                                                    SerializedProperty sp_Name = RMS_LayoutDatas.GetArrayElementAtIndex(s).FindPropertyRelative("LayoutName");
                                                    if (sp_Name.stringValue == RMS_Name.stringValue)
                                                    {
                                                        RMS_LayoutDatas.DeleteArrayElementAtIndex(s);
                                                        RMS_LayoutDatas.serializedObject.ApplyModifiedProperties();
                                                        break;
                                                    }
                                                }
                                                return;
                                            }
                                        }
                                        #endregion

                                        XGUI.layout_group_end(type: XGUIContainerType.Horizontal);

                                        TweensPreivew_In_State.serializedObject.ApplyModifiedProperties();
                                        TweensPreivew_Out_State.serializedObject.ApplyModifiedProperties();
                                        #endregion
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        XGUI.layout_label(
                            text: "暂未在管理器中配置 R M S 列表",
                            //bg_fill: XGUIFilled.无,
                            //bg_color: XGUIColor.深空灰,
                            size: XGUIFontSize.S,
                            anchor: TextAnchor.MiddleCenter,
                            text_color: Color.white * 0.85f,
                            offset: new Vector2(0, 0),
                            padding: new RectOffset(10, 10, 10, 10),
                            margin: new RectOffset(0, 0, 6, 0),
                            clipping: clipping,
                            wrap: true,
                            font: XGUI.GetFont("xg-regular"),
                            font_style: FontStyle.Normal);
                    }
                }
                else
                {
                    XGUI.layout_label(
                           text: "暂未发现 XHud 管理器",
                           //bg_fill: XGUIFilled.无,
                           //bg_color: XGUIColor.深空灰,
                           size: XGUIFontSize.S,
                           anchor: TextAnchor.MiddleCenter,
                           text_color: Color.white * 0.85f,
                           offset: new Vector2(0, 0),
                           padding: new RectOffset(10, 10, 10, 10),
                           margin: new RectOffset(0, 0, 6, 0),
                           clipping: clipping,
                           wrap: true,
                           font: XGUI.GetFont("xg-regular"),
                           font_style: FontStyle.Normal);
                }
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 子组件
            BaseScript.fold_list = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.Theme_Group,
                title: "子组件",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.Theme_Primary,
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: BaseScript.fold_list);

            if (!BaseScript.fold_list)
            {
                #region 列表
                if (Targets_Selected())
                {
                    XGUI.layout_label(
                        text: "列表不支持批量操作",
                        size: XGUIFontSize.S,
                        anchor: TextAnchor.MiddleCenter,
                        text_color: Color.white * 0.85f,
                        offset: new Vector2(0, 0),
                        padding: new RectOffset(10, 10, 10, 10),
                        margin: new RectOffset(0, 0, 6, 0),
                        clipping: clipping,
                        wrap: true,
                        font: XGUI.GetFont("xg-regular"),
                        font_style: FontStyle.Normal);
                }
                else
                {
                    if (SounderNodes.arraySize == 0 && PrimitiveControllerNodes.arraySize == 0 && ButtonNodes.arraySize == 0 && TextNodes.arraySize == 0 && TmpTextNodes.arraySize == 0 && OptionNodes.arraySize == 0 && SliderNodes.arraySize == 0 && ProgressNodes.arraySize == 0 && ToggleNodes.arraySize == 0)
                    {
                        XGUI.layout_label(
                            text: "暂无有效子组件列表",
                            size: XGUIFontSize.S,
                            anchor: TextAnchor.MiddleCenter,
                            text_color: Color.white * 0.85f,
                            offset: new Vector2(0, 0),
                            padding: new RectOffset(10, 10, 10, 10),
                            margin: new RectOffset(0, 0, 6, 0),
                            clipping: clipping,
                            wrap: true,
                            font: XGUI.GetFont("xg-regular"),
                            font_style: FontStyle.Normal);
                    }
                    else
                    {
                        XGUI.layout_space(20);

                        #region 图元控制器
                        if (PrimitiveControllerNodes.arraySize > 0)
                        {
                            PrimitivesIsFold.boolValue = XGUI.layout_group_start(
                                type: XGUIContainerType.Vertical,
                                bg_fill: XGUIFilled.缺口纯色边框,
                                bg_color: XGUIColor.亮白,
                                bg_color_gui: XHud_Dashboard.Theme_Group,
                                title: "图元控制器",
                                title_size: XGUIFontSize.M,
                                title_text_color: XHud_Dashboard.Theme_Primary,
                                title_clipping: TextClipping.Clip,
                                title_offset: new Vector2(10, 0),
                                padding: new RectOffset(10, 10, 15, 15),
                                foldout: PrimitivesIsFold.boolValue);

                            if (!PrimitivesIsFold.boolValue)
                                PrimitivesTweenList.DoLayoutList();

                            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                        }
                        #endregion

                        #region 文字
                        if (TextNodes.arraySize > 0)
                        {
                            TextIsFold.boolValue = XGUI.layout_group_start(
                                type: XGUIContainerType.Vertical,
                                bg_fill: XGUIFilled.缺口纯色边框,
                                bg_color: XGUIColor.亮白,
                                bg_color_gui: XHud_Dashboard.Theme_Group,
                                title: "文字",
                                title_size: XGUIFontSize.M,
                                title_text_color: XHud_Dashboard.Theme_Primary,
                                title_clipping: TextClipping.Clip,
                                title_offset: new Vector2(10, 0),
                                padding: new RectOffset(10, 10, 15, 15),
                                foldout: TextIsFold.boolValue);

                            if (!TextIsFold.boolValue)
                                TextList.DoLayoutList();

                            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                        }
                        #endregion

                        #region Tmp 文字
                        if (TmpTextNodes.arraySize > 0)
                        {
                            TmpTextIsFold.boolValue = XGUI.layout_group_start(
                                type: XGUIContainerType.Vertical,
                                bg_fill: XGUIFilled.缺口纯色边框,
                                bg_color: XGUIColor.亮白,
                                bg_color_gui: XHud_Dashboard.Theme_Group,
                                title: "Tmp 文字",
                                title_size: XGUIFontSize.M,
                                title_text_color: XHud_Dashboard.Theme_Primary,
                                title_clipping: TextClipping.Clip,
                                title_offset: new Vector2(10, 0),
                                padding: new RectOffset(10, 10, 15, 15),
                                foldout: TmpTextIsFold.boolValue);

                            if (!TmpTextIsFold.boolValue)
                                TmpTextList.DoLayoutList();

                            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                        }
                        #endregion

                        #region 按钮
                        if (ButtonNodes.arraySize > 0)
                        {
                            ButtonIsFold.boolValue = XGUI.layout_group_start(
                                type: XGUIContainerType.Vertical,
                                bg_fill: XGUIFilled.缺口纯色边框,
                                bg_color: XGUIColor.亮白,
                                bg_color_gui: XHud_Dashboard.Theme_Group,
                                title: "按钮",
                                title_size: XGUIFontSize.M,
                                title_text_color: XHud_Dashboard.Theme_Primary,
                                title_clipping: TextClipping.Clip,
                                title_offset: new Vector2(10, 0),
                                padding: new RectOffset(10, 10, 15, 15),
                                foldout: ButtonIsFold.boolValue);

                            if (!ButtonIsFold.boolValue)
                                ButtonList.DoLayoutList();

                            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                        }
                        #endregion

                        #region 选项
                        if (OptionNodes.arraySize > 0)
                        {
                            OptionIsFold.boolValue = XGUI.layout_group_start(
                                type: XGUIContainerType.Vertical,
                                bg_fill: XGUIFilled.缺口纯色边框,
                                bg_color: XGUIColor.亮白,
                                bg_color_gui: XHud_Dashboard.Theme_Group,
                                title: "选项",
                                title_size: XGUIFontSize.M,
                                title_text_color: XHud_Dashboard.Theme_Primary,
                                title_clipping: TextClipping.Clip,
                                title_offset: new Vector2(10, 0),
                                padding: new RectOffset(10, 10, 15, 15),
                                foldout: OptionIsFold.boolValue);

                            if (!OptionIsFold.boolValue)
                                OptionList.DoLayoutList();

                            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                        }
                        #endregion

                        #region 滑动条
                        if (SliderNodes.arraySize > 0)
                        {
                            SliderIsFold.boolValue = XGUI.layout_group_start(
                                type: XGUIContainerType.Vertical,
                                bg_fill: XGUIFilled.缺口纯色边框,
                                bg_color: XGUIColor.亮白,
                                bg_color_gui: XHud_Dashboard.Theme_Group,
                                title: "滑动条",
                                title_size: XGUIFontSize.M,
                                title_text_color: XHud_Dashboard.Theme_Primary,
                                title_clipping: TextClipping.Clip,
                                title_offset: new Vector2(10, 0),
                                padding: new RectOffset(10, 10, 15, 15),
                                foldout: SliderIsFold.boolValue);

                            if (!SliderIsFold.boolValue)
                                SliderList.DoLayoutList();

                            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                        }
                        #endregion

                        #region 进度条
                        if (ProgressNodes.arraySize > 0)
                        {
                            ProgressIsFold.boolValue = XGUI.layout_group_start(
                                type: XGUIContainerType.Vertical,
                                bg_fill: XGUIFilled.缺口纯色边框,
                                bg_color: XGUIColor.亮白,
                                bg_color_gui: XHud_Dashboard.Theme_Group,
                                title: "进度条",
                                title_size: XGUIFontSize.M,
                                title_text_color: XHud_Dashboard.Theme_Primary,
                                title_clipping: TextClipping.Clip,
                                title_offset: new Vector2(10, 0),
                                padding: new RectOffset(10, 10, 15, 15),
                                foldout: ProgressIsFold.boolValue);

                            if (!ProgressIsFold.boolValue)
                                ProgressList.DoLayoutList();

                            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                        }
                        #endregion

                        #region 开关
                        if (ToggleNodes.arraySize > 0)
                        {
                            ToggleIsFold.boolValue = XGUI.layout_group_start(
                                type: XGUIContainerType.Vertical,
                                bg_fill: XGUIFilled.缺口纯色边框,
                                bg_color: XGUIColor.亮白,
                                bg_color_gui: XHud_Dashboard.Theme_Group,
                                title: "开关",
                                title_size: XGUIFontSize.M,
                                title_text_color: XHud_Dashboard.Theme_Primary,
                                title_clipping: TextClipping.Clip,
                                title_offset: new Vector2(10, 0),
                                padding: new RectOffset(10, 10, 15, 15),
                                foldout: ToggleIsFold.boolValue);

                            if (!ToggleIsFold.boolValue)
                                ToggleList.DoLayoutList();

                            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                        }
                        #endregion

                        #region 音效器
                        if (SounderNodes.arraySize > 0)
                        {
                            SounderIsFold.boolValue = XGUI.layout_group_start(
                                type: XGUIContainerType.Vertical,
                                bg_fill: XGUIFilled.缺口纯色边框,
                                bg_color: XGUIColor.亮白,
                                bg_color_gui: XHud_Dashboard.Theme_Group,
                                title: "音效器",
                                title_size: XGUIFontSize.M,
                                title_text_color: XHud_Dashboard.Theme_Primary,
                                title_clipping: TextClipping.Clip,
                                title_offset: new Vector2(10, 0),
                                padding: new RectOffset(10, 10, 15, 15),
                                foldout: SounderIsFold.boolValue);

                            if (!SounderIsFold.boolValue)
                                SounderList.DoLayoutList();

                            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                        }
                        #endregion

                    }
                }
                #endregion
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 统计
            string statu_statistic = "统计";
            if (Targets_Selected())
                statu_statistic = "统计 - ( 批量模式 )";

            BaseScript.fold_statistic = XGUI.layout_group_start(
              type: XGUIContainerType.Vertical,
              bg_fill: XGUIFilled.缺口纯色边框,
              bg_color: XGUIColor.亮白,
              bg_color_gui: XHud_Dashboard.Theme_Group,
              title: statu_statistic,
              title_size: XGUIFontSize.M,
              title_text_color: XHud_Dashboard.Theme_Primary,
              title_clipping: TextClipping.Clip,
              padding: new RectOffset(10, 10, 15, 15),
              foldout: BaseScript.fold_statistic);

            if (!BaseScript.fold_statistic)
            {
                if (!Targets_Selected())
                {
                    if (SounderNodes.arraySize == 0 && PrimitiveControllerNodes.arraySize == 0 && ButtonNodes.arraySize == 0 && TextNodes.arraySize == 0 && TmpTextNodes.arraySize == 0 && OptionNodes.arraySize == 0 && SliderNodes.arraySize == 0 && ProgressNodes.arraySize == 0 && ToggleNodes.arraySize == 0)
                    {
                        XGUI.layout_label(
                            text: "暂无有效子组件列表",
                            size: XGUIFontSize.S,
                            anchor: TextAnchor.MiddleCenter,
                            text_color: Color.white * 0.85f,
                            offset: new Vector2(0, 0),
                            padding: new RectOffset(10, 10, 10, 10),
                            margin: new RectOffset(0, 0, 6, 0),
                            clipping: clipping,
                            wrap: true,
                            font: XGUI.GetFont("xg-regular"),
                            font_style: FontStyle.Normal);
                    }

                    #region 音效器
                    if (SounderNodes.arraySize > 0)
                        XGUI.layout_state_displayer_text(
                            title: "音效器",
                            title_size: XGUIFontSize.M,
                            subtitle: SounderNodes.arraySize.ToString() + " 个",
                            subtitle_size: XGUIFontSize.M,
                            subtitle_color: XHud_Dashboard.Theme_Primary,
                            margin: new RectOffset(5, 5, 0, 5));
                    #endregion

                    #region 图元控制器
                    if (PrimitiveControllerNodes.arraySize > 0)
                        XGUI.layout_state_displayer_text(
                            title: "图元控制器",
                            title_size: XGUIFontSize.M,
                            subtitle: PrimitiveControllerNodes.arraySize.ToString() + " 个",
                            subtitle_size: XGUIFontSize.M,
                            subtitle_color: XHud_Dashboard.Theme_Primary,
                            margin: new RectOffset(5, 5, 0, 5));
                    #endregion

                    #region 按钮
                    if (ButtonNodes.arraySize > 0)
                        XGUI.layout_state_displayer_text(
                            title: "按钮",
                            title_size: XGUIFontSize.M,
                            subtitle: ButtonNodes.arraySize.ToString() + " 个",
                            subtitle_size: XGUIFontSize.M,
                            subtitle_color: XHud_Dashboard.Theme_Primary,
                            margin: new RectOffset(5, 5, 0, 5));
                    #endregion

                    #region 选项
                    if (OptionNodes.arraySize > 0)
                        XGUI.layout_state_displayer_text(
                            title: "选项",
                            title_size: XGUIFontSize.M,
                            subtitle: OptionNodes.arraySize.ToString() + " 个",
                            subtitle_size: XGUIFontSize.M,
                            subtitle_color: XHud_Dashboard.Theme_Primary,
                            margin: new RectOffset(5, 5, 0, 5));
                    #endregion

                    #region 滑动条
                    if (SliderNodes.arraySize > 0)
                        XGUI.layout_state_displayer_text(
                            title: "滑动条",
                            title_size: XGUIFontSize.M,
                            subtitle: SliderNodes.arraySize.ToString() + " 个",
                            subtitle_size: XGUIFontSize.M,
                            subtitle_color: XHud_Dashboard.Theme_Primary,
                            margin: new RectOffset(5, 5, 0, 5));
                    #endregion

                    #region 进度条
                    if (ProgressNodes.arraySize > 0)
                        XGUI.layout_state_displayer_text(
                            title: "进度条",
                            title_size: XGUIFontSize.M,
                            subtitle: ProgressNodes.arraySize.ToString() + " 个",
                            subtitle_size: XGUIFontSize.M,
                            subtitle_color: XHud_Dashboard.Theme_Primary,
                            margin: new RectOffset(5, 5, 0, 5));
                    #endregion

                    #region 开关
                    if (ToggleNodes.arraySize > 0)
                        XGUI.layout_state_displayer_text(
                            title: "开关",
                            title_size: XGUIFontSize.M,
                            subtitle: ToggleNodes.arraySize.ToString() + " 个",
                            subtitle_size: XGUIFontSize.M,
                            subtitle_color: XHud_Dashboard.Theme_Primary,
                            margin: new RectOffset(5, 5, 0, 5));
                    #endregion

                    #region 文字
                    if (TextNodes.arraySize > 0)
                        XGUI.layout_state_displayer_text(
                            title: "文字",
                            title_size: XGUIFontSize.M,
                            subtitle: TextNodes.arraySize.ToString() + " 个",
                            subtitle_size: XGUIFontSize.M,
                            subtitle_color: XHud_Dashboard.Theme_Primary,
                            margin: new RectOffset(5, 5, 0, 5));
                    #endregion

                    #region Tmp文字
                    if (TmpTextNodes.arraySize > 0)
                        XGUI.layout_state_displayer_text(
                            title: "Tmp文字",
                            title_size: XGUIFontSize.M,
                            subtitle: TmpTextNodes.arraySize.ToString() + " 个",
                            subtitle_size: XGUIFontSize.M,
                            subtitle_color: XHud_Dashboard.Theme_Primary,
                            margin: new RectOffset(5, 5, 0, 5));
                    #endregion
                }
                else
                {
                    #region 批量控件
                    #region 控制区
                    XGUI.layout_group_start(
                        type: XGUIContainerType.Horizontal,
                        bg_fill: XGUIFilled.透明,
                        bg_color: XGUIColor.亮白,
                        bg_color_gui: XHud_Dashboard.Theme_Group,
                        absolute_margin: true,
                        absolute_padding: true,
                        margin: new RectOffset(0, 0, 0, 0),
                        padding: new RectOffset(15, 15, 5, 10));

                    #region 标题按钮
                    if (XGUI.layout_button(
                        text: $"{SelectedObjects[ElementStatu_Index].name} ( {SelectedObjects[ElementStatu_Index].Indicator} )",
                        tooltip: "",
                        bg_fill: XGUIFilled.透明,
                        button_text_color: Color.gray,
                        press_fill: XGUIFilled.透明,
                        press_text_color: XHud_Dashboard.Theme_Primary,
                        font_size: XGUIFontSize.M,
                        anchor: TextAnchor.MiddleLeft,
                        margin: new RectOffset(0, 0, 0, 0),
                        padding: new RectOffset(0, 0, 0, 0),
                        layout_width: 150,
                        button_text_font: XGUI.GetFont("xg-medium")))
                    {
                        EditorGUIUtility.PingObject(SelectedObjects[ElementStatu_Index]);
                    }
                    #endregion

                    XGUI.layout_flexspace();

                    #region 上一个
                    if (XGUI.layout_button(
                        tooltip: "上一个",
                        tex_release: left_arrow_r,
                        tex_press: left_arrow_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: 12,
                        height: 12))
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
                    #endregion

                    XGUI.layout_space(20);

                    #region 下一个
                    if (XGUI.layout_button(
                        tooltip: "下一个",
                        tex_release: right_arrow_r,
                        tex_press: right_arrow_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: 12,
                        height: 12))
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
                    #endregion

                    XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
                    #endregion

                    XGUI.layout_seperator(
                            thickness: 1,
                            color: XHud_Dashboard.Theme_SeperateLine,
                            margin: new RectOffset(15, 15, 5, 25));

                    XHud_Module_Element ele = SelectedObjects[ElementStatu_Index];

                    if (ele.SounderNodes.Count == 0 && ele.PrimitiveControllerNodes.Count == 0 && ele.ButtonNodes.Count == 0 && ele.TextNodes.Count == 0 && ele.TmpTextNodes.Count == 0 && ele.OptionNodes.Count == 0 && ele.SliderNodes.Count == 0 && ele.ProgressNodes.Count == 0 && ele.ToggleNodes.Count == 0)
                    {
                        XGUI.layout_label(
                            text: "暂无有效子组件列表",
                            size: XGUIFontSize.S,
                            anchor: TextAnchor.MiddleCenter,
                            text_color: Color.white * 0.85f,
                            offset: new Vector2(0, 0),
                            padding: new RectOffset(10, 10, 10, 10),
                            margin: new RectOffset(0, 0, 6, 0),
                            clipping: clipping,
                            wrap: true,
                            font: XGUI.GetFont("xg-regular"),
                            font_style: FontStyle.Normal);
                    }

                    #region 音效器
                    if (ele.SounderNodes.Count > 0)
                        XGUI.layout_state_displayer_text(
                            title: "音效器",
                            title_size: XGUIFontSize.M,
                            subtitle: ele.SounderNodes.Count.ToString() + " 个",
                            subtitle_size: XGUIFontSize.M,
                            subtitle_color: XHud_Dashboard.Theme_Primary,
                            margin: new RectOffset(5, 5, 0, 5));
                    #endregion

                    #region 图元控制器
                    if (ele.PrimitiveControllerNodes.Count > 0)
                        XGUI.layout_state_displayer_text(
                            title: "图元控制器",
                            title_size: XGUIFontSize.M,
                            subtitle: ele.PrimitiveControllerNodes.Count.ToString() + " 个",
                            subtitle_size: XGUIFontSize.M,
                            subtitle_color: XHud_Dashboard.Theme_Primary,
                            margin: new RectOffset(5, 5, 0, 5));
                    #endregion

                    #region 按钮
                    if (ele.ButtonNodes.Count > 0)
                        XGUI.layout_state_displayer_text(
                            title: "按钮",
                            title_size: XGUIFontSize.M,
                            subtitle: ele.ButtonNodes.Count.ToString() + " 个",
                            subtitle_size: XGUIFontSize.M,
                            subtitle_color: XHud_Dashboard.Theme_Primary,
                            margin: new RectOffset(5, 5, 0, 5));
                    #endregion

                    #region 选项
                    if (ele.OptionNodes.Count > 0)
                        XGUI.layout_state_displayer_text(
                            title: "选项",
                            title_size: XGUIFontSize.M,
                            subtitle: ele.OptionNodes.Count.ToString() + " 个",
                            subtitle_size: XGUIFontSize.M,
                            subtitle_color: XHud_Dashboard.Theme_Primary,
                            margin: new RectOffset(5, 5, 0, 5));
                    #endregion

                    #region 滑动条
                    if (ele.SliderNodes.Count > 0)
                        XGUI.layout_state_displayer_text(
                            title: "滑动条",
                            title_size: XGUIFontSize.M,
                            subtitle: ele.SliderNodes.Count.ToString() + " 个",
                            subtitle_size: XGUIFontSize.M,
                            subtitle_color: XHud_Dashboard.Theme_Primary,
                            margin: new RectOffset(5, 5, 0, 5));
                    #endregion

                    #region 进度条
                    if (ele.ProgressNodes.Count > 0)
                        XGUI.layout_state_displayer_text(
                            title: "进度条",
                            title_size: XGUIFontSize.M,
                            subtitle: ele.ProgressNodes.Count.ToString() + " 个",
                            subtitle_size: XGUIFontSize.M,
                            subtitle_color: XHud_Dashboard.Theme_Primary,
                            margin: new RectOffset(5, 5, 0, 5));
                    #endregion

                    #region 开关
                    if (ele.ToggleNodes.Count > 0)
                        XGUI.layout_state_displayer_text(
                            title: "开关",
                            title_size: XGUIFontSize.M,
                            subtitle: ele.ToggleNodes.Count.ToString() + " 个",
                            subtitle_size: XGUIFontSize.M,
                            subtitle_color: XHud_Dashboard.Theme_Primary,
                            margin: new RectOffset(5, 5, 0, 5));
                    #endregion

                    #region 文字
                    if (ele.TextNodes.Count > 0)
                        XGUI.layout_state_displayer_text(
                            title: "文字",
                            title_size: XGUIFontSize.M,
                            subtitle: ele.TextNodes.Count.ToString() + " 个",
                            subtitle_size: XGUIFontSize.M,
                            subtitle_color: XHud_Dashboard.Theme_Primary,
                            margin: new RectOffset(5, 5, 0, 5));
                    #endregion

                    #region Tmp文字
                    if (ele.TmpTextNodes.Count > 0)
                        XGUI.layout_state_displayer_text(
                            title: "Tmp文字",
                            title_size: XGUIFontSize.M,
                            subtitle: ele.TmpTextNodes.Count.ToString() + " 个",
                            subtitle_size: XGUIFontSize.M,
                            subtitle_color: XHud_Dashboard.Theme_Primary,
                            margin: new RectOffset(5, 5, 0, 5));
                    #endregion
                    #endregion
                }
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 组件
            BaseScript.fold_component = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.Theme_Group,
                title: "组件",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.Theme_Primary,
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: BaseScript.fold_component);

            if (!BaseScript.fold_component)
            {
                #region 基础图像组件
                XGUI.layout_property_field(
                    title: "基础图像组件",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    status_icon: "icon_field_status",
                    status_icon_color: CanvasGroup.objectReferenceValue == null ? Color.gray : XHud_Dashboard.Theme_Primary,
                    prop: CanvasGroup,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                #region 变换组件
                XGUI.layout_property_field(
                    title: "变换组件",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    status_icon: "icon_field_status",
                    status_icon_color: RectTransform.objectReferenceValue == null ? Color.gray : XHud_Dashboard.Theme_Primary,
                    prop: RectTransform,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                #region 触发器
                XGUI.layout_property_field(
                    title: "触发器",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    status_icon: "icon_field_status",
                    status_icon_color: TriggerAction.objectReferenceValue == null ? Color.gray : XHud_Dashboard.Theme_Primary,
                    prop: TriggerAction,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                #region 追踪器
                XGUI.layout_property_field(
                    title: "追踪器",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    status_icon: "icon_field_status",
                    status_icon_color: ObjectTracker.objectReferenceValue == null ? Color.gray : XHud_Dashboard.Theme_Primary,
                    prop: ObjectTracker,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
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
                        XGUI.dialog(
                            type: XGUIDialogType.警告,
                            windowtitle: "XHud - 元素消息",
                            title: "扫描元素",
                            msg: $"程序正在运行，无法在运行期间执行此功能！",
                            ok: "明白",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.Theme_Primary);

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
                            XGUI.dialog(
                                type: XGUIDialogType.警告,
                                windowtitle: "XHud - 元素消息",
                                title: "记录RMS方案信息",
                                msg: $"程序正在运行，无法在运行期间执行此功能！",
                                ok: "明白",
                                PrimaryIndex: 0,
                                usemodal: true,
                                themecolor: XHud_Dashboard.Theme_Primary);

                            return;
                        }

                        RMS_Record();
                    });
                    menu.AddItem(new GUIContent("C (清空)"), false, () =>
                    {
                        if (Application.isPlaying)
                        {
                            XGUI.dialog(
                                type: XGUIDialogType.警告,
                                windowtitle: "XHud - 元素消息",
                                title: "清空RMS方案信息",
                                msg: $"程序正在运行，无法在运行期间执行此功能！",
                                ok: "明白",
                                PrimaryIndex: 0,
                                usemodal: true,
                                themecolor: XHud_Dashboard.Theme_Primary);

                            return;
                        }

                        string res = XGUI.dialog(
                            type: XGUIDialogType.警告,
                            windowtitle: "XHud - 元素消息",
                            title: "RMS方案操作",
                            msg: $"如果清空匹配分辨率信息列表，会导致您之前为不同分辨率记录的坐标信息全部清空，请谨慎此操作！",
                            ok: "清空",
                            cancel: "暂不",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.Theme_Primary);
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
                        XGUI.dialog(
                            type: XGUIDialogType.警告,
                            windowtitle: "XHud - 元素消息",
                            title: "锚点初始化",
                            msg: $"程序正在运行，无法在运行期间执行此功能！",
                            ok: "明白",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.Theme_Primary);

                        return;
                    }
                    RestoreToAnchorPosition();
                });
                menu.AddDisabledItem(new GUIContent("动效快速操作"));
                menu.AddItem(new GUIContent("E (复制预览动效)"), false, () =>
                {
                    string res = XGUI.dialog(
                            type: XGUIDialogType.修改,
                            windowtitle: "XHud - 元素消息",
                            title: "复制预览动效",
                            msg: $"请选择动效参数复制模式！",
                            ok: "取消",
                            cancel: "生成",
                            alt: "回收",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.Theme_Primary);
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
                            XGUI.x_Editor_Data_Set_With_String("xData_MotionArgs", json);
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
                            XGUI.x_Editor_Data_Set_With_String("xData_MotionArgs", json);
                            break;
                    }

                    string mode = "";

                    if (res == "生成")
                        mode = "生成动效参数";
                    else if (res == "回收")
                        mode = "回收动效参数";

                    XGUI.dialog(
                           type: XGUIDialogType.修改,
                           windowtitle: "XHud - 元素消息",
                           title: "复制预览动效",
                           msg: $"已复制 \" {mode} \" 参数到 xData！",
                           ok: "明白",
                           PrimaryIndex: 0,
                           usemodal: true,
                           themecolor: XHud_Dashboard.Theme_Primary);
                });
                menu.AddItem(new GUIContent("R (粘贴预览动效)"), false, () =>
                {
                    string buffer = XGUI.x_Editor_Data_Get_With_String("xData_MotionArgs");
                    if (buffer.Contains("anchor"))//粘贴生成参数
                    {
                        string res = XGUI.dialog(
                           type: XGUIDialogType.修改,
                           windowtitle: "XHud - 元素消息",
                           title: "粘贴预览动效",
                           msg: $"检测到动效参数类型为： \"生成动效\"，确定要使用这个参数吗？",
                           ok: "确定",
                           cancel: "暂不",
                           PrimaryIndex: 0,
                           usemodal: true,
                           themecolor: XHud_Dashboard.Theme_Primary);
                        if (res == "暂不")
                            return;

                        Motion_Creator crc = JsonUtility.FromJson<Motion_Creator>(buffer);

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

                        XGUI.dialog(
                           type: XGUIDialogType.确认,
                           windowtitle: "XHud - 元素消息",
                           title: "粘贴预览动效",
                           msg: $"已更新 \"生成\" 动效参数！",
                           ok: "明白",
                           PrimaryIndex: 0,
                           usemodal: true,
                           themecolor: XHud_Dashboard.Theme_Primary);
                    }
                    else//粘贴回收参数
                    {
                        string res = XGUI.dialog(
                           type: XGUIDialogType.修改,
                           windowtitle: "XHud - 元素消息",
                           title: "粘贴预览动效",
                           msg: $"检测到动效参数类型为： \"回收动效\"，确定要使用这个参数吗？",
                           ok: "确定",
                           cancel: "暂不",
                           PrimaryIndex: 0,
                           usemodal: true,
                           themecolor: XHud_Dashboard.Theme_Primary);

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

                        XGUI.dialog(
                           type: XGUIDialogType.确认,
                           windowtitle: "XHud - 元素消息",
                           title: "粘贴预览动效",
                           msg: $"已更新 \"回收\" 动效参数！",
                           ok: "明白",
                           PrimaryIndex: 0,
                           usemodal: true,
                           themecolor: XHud_Dashboard.Theme_Primary);
                    }
                });
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("F (折叠组件列表)"), false, () =>
                {
                    AllListFoldState(true);
                });
                menu.AddItem(new GUIContent("V (展开组件列表)"), false, () =>
                {
                    AllListFoldState(false);
                });
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Q (折叠编组)"), false, () =>
                {
                    BaseScript.GroupFold(true);
                });
                menu.AddItem(new GUIContent("W (展开编组)"), false, () =>
                {
                    BaseScript.GroupFold(false);
                });
                menu.AddSeparator("");
                menu.AddDisabledItem(new GUIContent("生成预览"));
                if (!TweensPreivew_Out_State.boolValue)
                {
                    if (!TweensPreivew_In_State.boolValue)
                    {
                        menu.AddItem(new GUIContent("S (生成预览 - 开始)"), false, () =>
                        {
                            if (Application.isPlaying)
                            {
                                XGUI.dialog(
                                    type: XGUIDialogType.警告,
                                    windowtitle: "XHud - 元素消息",
                                    title: "预览动画",
                                    msg: $"程序正在运行，无法在运行期间执行此功能！",
                                    ok: "明白",
                                    PrimaryIndex: 0,
                                    usemodal: true,
                                    themecolor: XHud_Dashboard.Theme_Primary);

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
                                XGUI.dialog(
                                    type: XGUIDialogType.警告,
                                    windowtitle: "XHud - 元素消息",
                                    title: "停止预览",
                                    msg: $"程序正在运行，无法在运行期间执行此功能！",
                                    ok: "明白",
                                    PrimaryIndex: 0,
                                    usemodal: true,
                                    themecolor: XHud_Dashboard.Theme_Primary);

                                return;
                            }
                            ElementTweens_Preview_In_Stop();
                        });
                    }
                }

                if (!TweensPreivew_In_State.boolValue)
                {
                    menu.AddSeparator("");
                    menu.AddDisabledItem(new GUIContent("回收预览"));
                    if (!TweensPreivew_Out_State.boolValue)
                    {
                        menu.AddItem(new GUIContent("D (回收预览 - 开始)"), false, () =>
                        {
                            if (Application.isPlaying)
                            {
                                XGUI.dialog(
                                    type: XGUIDialogType.警告,
                                    windowtitle: "XHud - 元素消息",
                                    title: "预览动画",
                                    msg: $"程序正在运行，无法在运行期间执行此功能！",
                                    ok: "明白",
                                    PrimaryIndex: 0,
                                    usemodal: true,
                                    themecolor: XHud_Dashboard.Theme_Primary);

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
                                XGUI.dialog(
                                    type: XGUIDialogType.警告,
                                    windowtitle: "XHud - 元素消息",
                                    title: "停止预览",
                                    msg: $"程序正在运行，无法在运行期间执行此功能！",
                                    ok: "明白",
                                    PrimaryIndex: 0,
                                    usemodal: true,
                                    themecolor: XHud_Dashboard.Theme_Primary);

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
                            XGUI.dialog(
                                    type: XGUIDialogType.警告,
                                    windowtitle: "XHud - 元素消息",
                                    title: "预制体应用",
                                    msg: $"程序正在运行，无法在运行期间执行此功能！",
                                    ok: "明白",
                                    PrimaryIndex: 0,
                                    usemodal: true,
                                    themecolor: XHud_Dashboard.Theme_Primary);

                            return;
                        }
                        if (Targets_Selected())
                        {
                            for (int i = 0; i < SelectedObjects.Length; i++)
                            {
                                XHud_Module_Element ele = SelectedObjects[i];
                                string path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(ele.gameObject);
                                PrefabUtility.SaveAsPrefabAssetAndConnect(ele.gameObject, path, InteractionMode.AutomatedAction);
                            }
                        }
                        else
                        {
                            string path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(BaseScript.gameObject);
                            PrefabUtility.SaveAsPrefabAssetAndConnect(BaseScript.gameObject, path, InteractionMode.AutomatedAction);
                        }
                    });
                    menu.AddItem(new GUIContent("W (恢复)"), false, () =>
                    {
                        if (Application.isPlaying)
                        {
                            XGUI.dialog(
                                    type: XGUIDialogType.警告,
                                    windowtitle: "XHud - 元素消息",
                                    title: "预制体恢复",
                                    msg: $"程序正在运行，无法在运行期间执行此功能！",
                                    ok: "明白",
                                    PrimaryIndex: 0,
                                    usemodal: true,
                                    themecolor: XHud_Dashboard.Theme_Primary);

                            return;
                        }
                        if (Targets_Selected())
                        {
                            for (int i = 0; i < SelectedObjects.Length; i++)
                            {
                                XHud_Module_Element ele = SelectedObjects[i];
                                string path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(ele.gameObject);
                                PrefabUtility.RevertPrefabInstance(ele.gameObject, InteractionMode.AutomatedAction);
                            }
                        }
                        else
                        {
                            string path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(BaseScript.gameObject);
                            PrefabUtility.RevertPrefabInstance(BaseScript.gameObject, InteractionMode.AutomatedAction);
                        }
                    });
                    menu.AddItem(new GUIContent("Q (定位)"), false, () =>
                    {
                        if (Application.isPlaying)
                        {
                            XGUI.dialog(
                                    type: XGUIDialogType.警告,
                                    windowtitle: "XHud - 元素消息",
                                    title: "预制体定位",
                                    msg: $"程序正在运行，无法在运行期间执行此功能！",
                                    ok: "明白",
                                    PrimaryIndex: 0,
                                    usemodal: true,
                                    themecolor: XHud_Dashboard.Theme_Primary);

                            return;
                        }
                        if (Targets_Selected())
                        {
                            for (int i = 0; i < SelectedObjects.Length; i++)
                            {
                                XHud_Module_Element ele = SelectedObjects[i];
                                string path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(ele.gameObject);
                                Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
                                EditorGUIUtility.PingObject(obj);
                            }
                        }
                        else
                        {
                            string path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(BaseScript.gameObject);
                            Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
                            EditorGUIUtility.PingObject(obj);
                        }
                    });
                }
                menu.ShowAsContext(); // 在鼠标位置显示右键菜单
            }
            #endregion

            CheckModulesValid();

            #region 源脚本
            BaseScript.fold_based = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.Theme_Group,
                title: "源脚本",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.Theme_Primary,
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: BaseScript.fold_based);

            if (!BaseScript.fold_based)
            {
                DrawDefaultInspector();
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            PrimitiveTweens_MaxDuration_Calculate();

            serializedObject.ApplyModifiedProperties();
        }

        #region 辅助
        /// <summary>
        /// 元素回到锚点初始位置
        /// </summary>
        private void RestoreToAnchorPosition()
        {
            string res = XGUI.dialog(
                type: XGUIDialogType.警告,
                windowtitle: "XHud - 元素消息",
                title: "锚点初始化",
                msg: $"是否需要将元素回到当前所在的锚点初始位置？",
                ok: "确定",
                cancel: "暂不",
                PrimaryIndex: 0,
                usemodal: true,
                themecolor: XHud_Dashboard.Theme_Primary);

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
            List<XGUIDialogListDatas> Datas = new List<XGUIDialogListDatas>();
            if (Targets_Selected())
            {
                for (int s = 0; s < SelectedObjects.Length; s++)
                {
                    bool existcomp = false;

                    XHud_Module_Element ele = SelectedObjects[s];
                    //-扫描 - 图元控制器 
                    for (int i = 0; i < ele.PrimitiveControllerNodes.Count; i++)
                    {
                        XGUIDialogListDatas dataitem = new XGUIDialogListDatas();
                        dataitem.Title = $"{SelectedObjects[s].name} ( {SelectedObjects[s].Indicator} )";
                        dataitem.SubTitle = "已收集 / 图元控制器";
                        dataitem.Message = $"{ele.PrimitiveControllerNodes[i].Controller.name} ( {ele.PrimitiveControllerNodes[i].Controller.Indicator} )";
                        Datas.Add(dataitem);
                        existcomp = true;
                    }
                    //-扫描 - 按钮
                    for (int i = 0; i < ele.ButtonNodes.Count; i++)
                    {
                        XGUIDialogListDatas dataitem = new XGUIDialogListDatas();
                        dataitem.Title = $"{SelectedObjects[s].name} ( {SelectedObjects[s].Indicator} )";
                        dataitem.SubTitle = "已收集 / 按钮";
                        dataitem.Message = $"{ele.ButtonNodes[i].Button.name} ( {ele.ButtonNodes[i].Button.Indicator} )";
                        Datas.Add(dataitem);
                        existcomp = true;
                    }
                    //-扫描 - 选项器
                    for (int i = 0; i < ele.OptionNodes.Count; i++)
                    {
                        XGUIDialogListDatas dataitem = new XGUIDialogListDatas();
                        dataitem.Title = $"{SelectedObjects[s].name} ( {SelectedObjects[s].Indicator} )";
                        dataitem.SubTitle = "已收集 / 选项";
                        dataitem.Message = $"{ele.OptionNodes[i].Option.name} ( {ele.OptionNodes[i].Option.Indicator} )";
                        Datas.Add(dataitem);
                        existcomp = true;
                    }
                    //-扫描 - 滑动条
                    for (int i = 0; i < ele.SliderNodes.Count; i++)
                    {
                        XGUIDialogListDatas dataitem = new XGUIDialogListDatas();
                        dataitem.Title = $"{SelectedObjects[s].name} ( {SelectedObjects[s].Indicator} )";
                        dataitem.SubTitle = "已收集 / 滑动条";
                        dataitem.Message = $"{ele.SliderNodes[i].Slider.name} ( {ele.SliderNodes[i].Slider.Indicator} )";
                        Datas.Add(dataitem);
                        existcomp = true;
                    }
                    //-扫描 - 进度条
                    for (int i = 0; i < ele.ProgressNodes.Count; i++)
                    {
                        XGUIDialogListDatas dataitem = new XGUIDialogListDatas();
                        dataitem.Title = $"{SelectedObjects[s].name} ( {SelectedObjects[s].Indicator} )";
                        dataitem.SubTitle = "已收集 / 进度条";
                        dataitem.Message = $"{ele.ProgressNodes[i].Progress.name} ( {ele.ProgressNodes[i].Progress.Indicator} )";
                        Datas.Add(dataitem);
                        existcomp = true;
                    }
                    //-扫描 - 开关
                    for (int i = 0; i < ele.ToggleNodes.Count; i++)
                    {
                        XGUIDialogListDatas dataitem = new XGUIDialogListDatas();
                        dataitem.Title = $"{SelectedObjects[s].name} ( {SelectedObjects[s].Indicator} )";
                        dataitem.SubTitle = "已收集 / 开关";
                        dataitem.Message = $"{ele.ToggleNodes[i].Toggle.name} ( {ele.ToggleNodes[i].Toggle.Indicator} )";
                        Datas.Add(dataitem);
                        existcomp = true;
                    }
                    //-扫描 - 声音
                    for (int i = 0; i < ele.SounderNodes.Count; i++)
                    {
                        XGUIDialogListDatas dataitem = new XGUIDialogListDatas();
                        dataitem.Title = $"{SelectedObjects[s].name} ( {SelectedObjects[s].Indicator} )";
                        dataitem.SubTitle = "已收集 / 音效器";
                        dataitem.Message = $"{ele.SounderNodes[i].Sounder.name} ( {ele.SounderNodes[i].Sounder.Indicator} )";
                        Datas.Add(dataitem);
                        existcomp = true;
                    }
                    //-扫描 - Text文字
                    for (int i = 0; i < ele.TextNodes.Count; i++)
                    {
                        XGUIDialogListDatas dataitem = new XGUIDialogListDatas();
                        dataitem.Title = $"{SelectedObjects[s].name} ( {SelectedObjects[s].Indicator} )";
                        dataitem.SubTitle = "已收集 / Text文字";
                        dataitem.Message = $"{ele.TextNodes[i].Text.name} ( {ele.TextNodes[i].Text.Indicator} )";
                        Datas.Add(dataitem);
                        existcomp = true;
                    }
                    //-扫描 - Tmp文字
                    for (int i = 0; i < ele.TmpTextNodes.Count; i++)
                    {
                        XGUIDialogListDatas dataitem = new XGUIDialogListDatas();
                        dataitem.Title = $"{SelectedObjects[s].name} ( {SelectedObjects[s].Indicator} )";
                        dataitem.SubTitle = "已收集 / Text文字";
                        dataitem.Message = $"{ele.TmpTextNodes[i].TmpText.name} ( {ele.TmpTextNodes[i].TmpText.Indicator} )";
                        Datas.Add(dataitem);
                        existcomp = true;
                    }

                    if (!existcomp)
                    {
                        XGUIDialogListDatas dataitem = new XGUIDialogListDatas();
                        dataitem.Title = $"{SelectedObjects[s].name} ( {SelectedObjects[s].Indicator} )";
                        dataitem.SubTitle = "";
                        dataitem.Message = "未发现任何存在的子组件";
                        Datas.Add(dataitem);
                    }
                }

                XGUI.dialog_listview(
                         datas: Datas.ToArray(),
                         type: XGUIDialogType.通知,
                         windowtitle: "XHud - 元素消息",
                         title: "批量扫描元素子组件",
                         msg: "以下是批量扫描到的所有元素子组件列表，请您检查核对：",
                         ok: "明白",
                         PrimaryIndex: 0,
                         usemodal: false,
                         themecolor: XHud_Dashboard.Theme_Primary);
            }
            else
            {
                //-扫描 - 图元控制器
                for (int i = 0; i < BaseScript.PrimitiveControllerNodes.Count; i++)
                {
                    XGUIDialogListDatas dataitem = new XGUIDialogListDatas();
                    dataitem.Title = $"{BaseScript.name} ({BaseScript.Indicator} )";
                    dataitem.SubTitle = "已收集 / 图元控制器";
                    dataitem.Message = $"{BaseScript.PrimitiveControllerNodes[i].Controller.name} ({BaseScript.PrimitiveControllerNodes[i].Controller.Indicator} )";
                    Datas.Add(dataitem);
                }
                //-扫描 - 按钮
                for (int i = 0; i < BaseScript.ButtonNodes.Count; i++)
                {
                    XGUIDialogListDatas dataitem = new XGUIDialogListDatas();
                    dataitem.Title = $"{BaseScript.name} ({BaseScript.Indicator} )";
                    dataitem.SubTitle = "已收集 / 按钮";
                    dataitem.Message = $"{BaseScript.ButtonNodes[i].Button.name} ({BaseScript.ButtonNodes[i].Button.Indicator} )";
                    Datas.Add(dataitem);
                }
                //-扫描 - 选项器
                for (int i = 0; i < BaseScript.OptionNodes.Count; i++)
                {
                    XGUIDialogListDatas dataitem = new XGUIDialogListDatas();
                    dataitem.Title = $"{BaseScript.name} ({BaseScript.Indicator} )";
                    dataitem.SubTitle = "已收集 / 选项";
                    dataitem.Message = $"{BaseScript.OptionNodes[i].Option.name} ({BaseScript.OptionNodes[i].Option.Indicator} )";
                    Datas.Add(dataitem);
                }
                //-扫描 - 滑动条
                for (int i = 0; i < BaseScript.SliderNodes.Count; i++)
                {
                    XGUIDialogListDatas dataitem = new XGUIDialogListDatas();
                    dataitem.Title = $"{BaseScript.name} ({BaseScript.Indicator} )";
                    dataitem.SubTitle = "已收集 / 滑动条";
                    dataitem.Message = $"{BaseScript.SliderNodes[i].Slider.name} ({BaseScript.SliderNodes[i].Slider.Indicator} )";
                    Datas.Add(dataitem);
                }
                //-扫描 - 进度条
                for (int i = 0; i < BaseScript.ProgressNodes.Count; i++)
                {
                    XGUIDialogListDatas dataitem = new XGUIDialogListDatas();
                    dataitem.Title = $"{BaseScript.name} ({BaseScript.Indicator} )";
                    dataitem.SubTitle = "已收集 / 进度条";
                    dataitem.Message = $"{BaseScript.ProgressNodes[i].Progress.name} ({BaseScript.ProgressNodes[i].Progress.Indicator} )";
                    Datas.Add(dataitem);
                }
                //-扫描 - 开关
                for (int i = 0; i < BaseScript.ToggleNodes.Count; i++)
                {
                    XGUIDialogListDatas dataitem = new XGUIDialogListDatas();
                    dataitem.Title = $"{BaseScript.name} ({BaseScript.Indicator} )";
                    dataitem.SubTitle = "已收集 / 开关";
                    dataitem.Message = $"{BaseScript.ToggleNodes[i].Toggle.name} ({BaseScript.ToggleNodes[i].Toggle.Indicator} )";
                    Datas.Add(dataitem);
                }
                //-扫描 - 声音
                for (int i = 0; i < BaseScript.SounderNodes.Count; i++)
                {
                    XGUIDialogListDatas dataitem = new XGUIDialogListDatas();
                    dataitem.Title = $"{BaseScript.name} ({BaseScript.Indicator} )";
                    dataitem.SubTitle = "已收集 / 音效器";
                    dataitem.Message = $"{BaseScript.SounderNodes[i].Sounder.name} ({BaseScript.SounderNodes[i].Sounder.Indicator} )";
                    Datas.Add(dataitem);
                }
                //-扫描 - Text文字
                for (int i = 0; i < BaseScript.TextNodes.Count; i++)
                {
                    XGUIDialogListDatas dataitem = new XGUIDialogListDatas();
                    dataitem.Title = $"{BaseScript.name} ({BaseScript.Indicator} )";
                    dataitem.SubTitle = "已收集 / Text文字";
                    dataitem.Message = $"{BaseScript.TextNodes[i].Text.name} ({BaseScript.TextNodes[i].Text.Indicator} )";
                    Datas.Add(dataitem);
                }
                //-扫描 - Tmp文字
                for (int i = 0; i < BaseScript.TmpTextNodes.Count; i++)
                {
                    XGUIDialogListDatas dataitem = new XGUIDialogListDatas();
                    dataitem.Title = $"{BaseScript.name} ({BaseScript.Indicator} )";
                    dataitem.SubTitle = "已收集 / Text文字";
                    dataitem.Message = $"{BaseScript.TmpTextNodes[i].TmpText.name} ({BaseScript.TmpTextNodes[i].TmpText.Indicator} )";
                    Datas.Add(dataitem);
                }

                if (BaseScript.PrimitiveControllerNodes.Count <= 0 && BaseScript.ButtonNodes.Count <= 0 && BaseScript.OptionNodes.Count <= 0 && BaseScript.SliderNodes.Count <= 0 && BaseScript.ProgressNodes.Count <= 0 && BaseScript.ToggleNodes.Count <= 0 && BaseScript.SounderNodes.Count <= 0 && BaseScript.TextNodes.Count <= 0 && BaseScript.TmpTextNodes.Count <= 0)
                {
                    string hexcol = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary, true);

                    XGUI.dialog(
                        type: XGUIDialogType.警告,
                        windowtitle: "XHud - 元素消息",
                        title: "扫描元素子组件",
                        msg: $"未扫描到任何子元素，请您检查子物体中是否有物体包含以下组件： <color={hexcol}>XHud_PrimitiveController、XHud_Button、XHud_Progress、XHud_Slider、XHud_Option、XHudSound、XHud_Text、XHud_TmpText、XHudToggle</color>",
                        ok: "明白",
                        PrimaryIndex: 0,
                        usemodal: true,
                        themecolor: XHud_Dashboard.Theme_Primary);
                }
                else
                {
                    XGUI.dialog_listview(
                        datas: Datas.ToArray(),
                        type: XGUIDialogType.通知,
                        windowtitle: "XHud - 元素消息",
                        title: "批量扫描元素子组件",
                        msg: "以下是扫描到的元素所有子组件列表，请您检查核对：",
                        ok: "明白",
                        PrimaryIndex: 0,
                        usemodal: false,
                        themecolor: XHud_Dashboard.Theme_Primary);
                }
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
            if (Highlighter.objectReferenceValue == null)
            {
                Highlighter.objectReferenceValue = BaseScript.GetComponent<Image>();
                Highlighter.serializedObject.ApplyModifiedProperties();
            }

            if (BaseScript.Highlighter != null)
            {
                BaseScript.Highlighter.type = Image.Type.Sliced;
                BaseScript.Highlighter.color = new Color(1, 1, 1, 0);
            }

            if (BaseScript.Highlighter.sprite == null)
            {
                BaseScript.Highlighter.type = Image.Type.Sliced;
                BaseScript.Highlighter.sprite = (Sprite)AssetDatabase.LoadAssetAtPath($"{XHud_Dashboard.Get_Path_XHUD_SPRITES_Path()}Others/Rect_SolidBlur.png", typeof(Sprite));
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
                    SerializedProperty sp_tweens_isfold = so_ele.FindProperty("PrimitivesIsFold");
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

                    sp_tweens_isfold.boolValue = state;
                    sp_btn_isfold.boolValue = state;
                    sp_act_isfold.boolValue = state;
                    sp_sli_isfold.boolValue = state;
                    sp_opt_isfold.boolValue = state;
                    sp_sod_isfold.boolValue = state;
                    sp_txt_isfold.boolValue = state;
                    sp_tmp_txt_isfold.boolValue = state;
                    sp_progress_isfold.boolValue = state;
                    sp_tog_isfold.boolValue = state;

                    sp_tweens_isfold.serializedObject.ApplyModifiedProperties();
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
        private void PrimitiveTweens_MaxDuration_Calculate()
        {
            if (!Targets_Selected())
            {
                PrimitivesTweenMaxDuration.floatValue = PrimitiveTweens_MaxDuration_Get(BaseScript.PrimitiveControllerNodes, PrimitivesTweenGlobalDuration.floatValue);
                PrimitivesTweenMaxDuration.serializedObject.ApplyModifiedProperties();
            }
            else
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    SelectedObjects[i].PrimitivesTweenMaxDuration = PrimitiveTweens_MaxDuration_Get(SelectedObjects[i].PrimitiveControllerNodes, SelectedObjects[i].PrimitivesTweenGlobalDuration);
                }
            }


        }
        /// <summary>
        /// 从所有子图元控制器的动画器中获取最大耗时
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
                if (list[i].Controller.pt_Tween != null)
                    list[i].Controller.pt_Tween.TweenNode_GetTimers();
                x_list[i] = (list[i].Controller.pt_Tween == null ? 0 : list[i].Controller.pt_Tween.MaxTimerWithGlobalDuration) + list[i].DelayTime;
            }
            float v = XGUI_Utilitys.MaxValue(x_list);
            return v * globaldur;
        }
        /// <summary>
        /// 获取图元控制器的动画器中是否存在循环模式
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
        /// 获取图元控制器的动画器中是否存在循环模式
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

            PreviewIncludePrimitivesTween = serializedObject.FindProperty("PreviewIncludePrimitivesTween");
            Crc_Preview_Position = serializedObject.FindProperty("Crc_Preview_Position");
            Rec_Preview_Position = serializedObject.FindProperty("Rec_Preview_Position");
            Crc_Preview_SetPosition = serializedObject.FindProperty("Crc_Preview_SetPosition");
            Rec_Preview_SetPosition = serializedObject.FindProperty("Rec_Preview_SetPosition");

            AutoPlayPrimitivesTween = serializedObject.FindProperty("AutoPlayPrimitivesTween");
            PrimitivesTweenMaxDuration = serializedObject.FindProperty("PrimitivesTweenMaxDuration");
            PrimitivesTweenGlobalDuration = serializedObject.FindProperty("PrimitivesTweenGlobalDuration");
            PrimitivesIsFold = serializedObject.FindProperty("PrimitivesIsFold");
            PrimitiveControllerNodes = serializedObject.FindProperty("PrimitiveControllerNodes");

            ClearActions_With_Spawn = serializedObject.FindProperty("ClearActions_With_Spawn");
            ClearEvents_With_Spawn = serializedObject.FindProperty("ClearEvents_With_Spawn");

            Highlighter = serializedObject.FindProperty("Highlighter");
        }
        #endregion

        #region Draw
        /// <summary>
        /// 通用方法：绘制开关
        /// </summary>
        private void DrawToggle(string title, SerializedProperty prop, float width, XGUIToggleStyle style = XGUIToggleStyle.实体, Color color_bg_on = default, Color color_bg_off = default, Color color_on = default, Color color_off = default, string[] options = null, Action<bool> act_on_changed = null)
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
                tog_mixed_options: options,
                tog_mixed_text_size: XGUIFontSize.M,
                tog_mixed_text_color: Color.black,
                tog_mixed_text_padding: new RectOffset(10, 10, 0, 0),
                tog_mixed_text_anchor: TextAnchor.MiddleCenter,
                tog_mixed_font_style: FontStyle.Normal,
                tog_bg_off_color: color_bg_off,
                tog_bg_on_color: color_bg_on,
                tog_handler_off_color: color_off,
                tog_handler_on_color: color_on,
                tog_mixed_bg_color_gui: XHud_Dashboard.Theme_Primary,
                act_on_changed: act_on_changed);
        }
        #endregion
    }
}