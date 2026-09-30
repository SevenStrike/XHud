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
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(XHud_Module_Primitive_Painting))]
    public class Editor_XHud_Module_Primitive_Painting : Editor
    {
        #region 组件 / 列表
        private XHud_Module_Primitive_Painting BaseScript;
        private XHud_Manager HudManager;
        #endregion

        #region 序列化属性
        private SerializedProperty
            sp_OriginalColor,
            sp_ColoriseName,
            sp_Debug,
            sp_SyncLibraryColor,
            sp_SyncImageColor;
        #endregion

        #region 选项文字
        string[] stroptions_debug = new string[2] { "关闭", "调试" };
        string[] stroptions_sync = new string[2] { "关闭", "接管" };
        #endregion

        #region 图标
        private Texture2D icon_main, colormode_lib_r, colormode_lib_p, colormode_ori_r, colormode_ori_p, colormode_mix_r, colormode_mix_p, Add_r, Add_p, locate_r, locate_p, copycolor_r, copycolor_p, recogcolor_r, recogcolor_p;
        #endregion

        #region 批量化操作
        /// <summary>
        /// 批量选择脚本数组
        /// </summary>
        XHud_Module_Primitive_Painting[] SelectedObjects;
        /// <summary>
        /// 获取所有批量脚本目标
        /// </summary>
        private void Targets_Get()
        {
            if (targets.Length > 1)
            {
                SelectedObjects = new XHud_Module_Primitive_Painting[targets.Length];
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    var t = targets[i];
                    SelectedObjects[i] = (XHud_Module_Primitive_Painting)t;
                }
            }
            else
            {
                SelectedObjects = new XHud_Module_Primitive_Painting[targets.Length];
                SelectedObjects[0] = (XHud_Module_Primitive_Painting)target;
            }
        }
        /// <summary>
        /// 判断是否是多选状态
        /// </summary>
        /// <returns></returns>
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

            BaseScript = (XHud_Module_Primitive_Painting)target;

            Targets_Get();

            // 获取所有序列化字段
            GetSerializeFields();

            #region 获取图标          
            icon_main = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_painting/icon_main");
            colormode_lib_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_painting/col_lib_r");
            colormode_lib_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_painting/col_lib_p");
            colormode_ori_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_painting/col_ori_r");
            colormode_ori_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_painting/col_ori_p");
            colormode_mix_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_painting/col_mix_r");
            colormode_mix_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_painting/col_mix_p");
            Add_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_painting/Add_r");
            Add_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_painting/Add_p");
            locate_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_painting/locate_r");
            locate_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_painting/locate_p");
            copycolor_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_painting/copycolor_r");
            copycolor_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_painting/copycolor_p");
            recogcolor_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_painting/recogcolor_r");
            recogcolor_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_painting/recogcolor_p");
            #endregion
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            TextClipping clipping = XGUI.TryEllipsisClipping();

            #region 标题
            XGUI.layout_banner(
                bg_fill: XGUIFilled.实体,
                bg_color: XGUIColor.深空灰,
                bg_height: 30,
                icon: icon_main,
                icon_color: XHud_Dashboard.Theme_Primary,
                title_text: "XHud  -  图元  >  配色器",
                title_anchor: TextAnchor.MiddleLeft,
                title_style: FontStyle.Normal,
                title_color: Color.white,
                title_size: XGUIFontSize.B,
                title_clipping: clipping,
                bg_margin: new RectOffset(0, 0, 5, 5));
            #endregion

            Rect rect = GUILayoutUtility.GetLastRect();

            #region 检测多选模式下的颜色模式是否一致
            // 该字段用于检测多选情况下每个Painting的颜色模式是否是一致的
            bool MultiColorModeSame = true;
            bool mode = SelectedObjects[0].SyncLibraryColor;
            if (Targets_Selected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    if (SelectedObjects[i].SyncLibraryColor != mode)
                    {
                        MultiColorModeSame = false;
                        break;
                    }
                }
            }
            #endregion

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

            #region 切换模式
            if (XGUI.layout_button(
                tooltip: "切换模式",
                tex_release: MultiColorModeSame ? (mode ? colormode_lib_r : colormode_ori_r) : colormode_mix_r,
                tex_press: MultiColorModeSame ? (mode ? colormode_lib_p : colormode_ori_p) : colormode_mix_p,
                tex_gui_color: Color.white,
                border: new RectOffset(0, 0, 0, 0),
                width: 14,
                height: 14))
            {
                if (Targets_Selected())
                {

                    List<XGUIDialogListDatas> datas = new List<XGUIDialogListDatas>();

                    for (int i = 0; i < SelectedObjects.Length; i++)
                    {
                        XGUIDialogListDatas data = new XGUIDialogListDatas();
                        data.Title = string.IsNullOrEmpty(SelectedObjects[i].controller.Indicator) ? SelectedObjects[i].name : SelectedObjects[i].controller.Indicator;
                        data.SubTitle = "当前颜色模式";
                        data.Message = SelectedObjects[i].SyncLibraryColor ? "色卡库" : "原始色";
                        datas.Add(data);
                    }
                    if (datas.Count > 0)
                    {
                        string res_x = XGUI.dialog_listview(
                            datas: datas.ToArray(),
                            type: XGUIDialogType.通知,
                            windowtitle: "XHud - 图元配色器消息",
                            title: "批量切换颜色模式",
                            msg: "是否需要批量切换以下列表中的图元控制器的配色器物体的颜色模式？",
                            ok: "切换",
                            cancel: "暂不",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.Theme_Primary);

                        if (res_x == "切换")
                        {
                            string res_y = XGUI.dialog(
                                type: XGUIDialogType.警告,
                                windowtitle: "XHud - 图元配色器消息",
                                title: "切换颜色模式",
                                msg: "需要切换为那种模式？",
                                ok: "色卡库",
                                cancel: "原始色",
                                PrimaryIndex: 1,
                                usemodal: true,
                                themecolor: XHud_Dashboard.Theme_Primary);

                            if (res_y == "原始色")
                            {
                                for (int i = 0; i < SelectedObjects.Length; i++)
                                {
                                    SelectedObjects[i].SyncLibraryColor = false;
                                }
                            }
                            else
                            {
                                for (int i = 0; i < SelectedObjects.Length; i++)
                                {
                                    SelectedObjects[i].SyncLibraryColor = true;

                                    if (!Application.isPlaying)
                                    {
                                        //--检查色卡名称是否失效
                                        if (!HudManager.Hud_Colors.ColorsLibrary_IsExist(SelectedObjects[i].ColoriseName) || string.IsNullOrEmpty(SelectedObjects[i].ColoriseName))
                                        {
                                            SelectedObjects[i].ColoriseName = HudManager.Hud_Colors.ColorsLibrary_GetColorName(0);
                                        }
                                    }
                                    else
                                    {
                                        //--检查色卡名称是否失效
                                        if (!HudManager.Hud_Colors.ColorsLibrary_IsExist(SelectedObjects[i].ColoriseName) || string.IsNullOrEmpty(SelectedObjects[i].ColoriseName))
                                        {
                                            SelectedObjects[i].ColoriseName = HudManager.Hud_Colors.ColorsLibrary_GetColorName(0);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
                else
                {
                    string res = XGUI.dialog(
                              type: XGUIDialogType.警告,
                              windowtitle: "XHud - 图元配色器消息",
                              title: "切换颜色模式",
                              msg: $"需要将 {(string.IsNullOrEmpty(BaseScript.controller.Indicator) ? BaseScript.name : BaseScript.controller.Indicator)} 颜色显示切换为那种模式？",
                              ok: "色卡库",
                              cancel: "原始色",
                              PrimaryIndex: 1,
                              usemodal: true,
                              themecolor: XHud_Dashboard.Theme_Primary);

                    if (res == "原始色")
                    {
                        sp_SyncLibraryColor.boolValue = false;
                        sp_SyncLibraryColor.serializedObject.ApplyModifiedProperties();
                    }
                    else
                    {
                        sp_SyncLibraryColor.boolValue = true;
                        sp_SyncLibraryColor.serializedObject.ApplyModifiedProperties();

                        if (!Application.isPlaying)
                        {
                            //--检查色卡名称是否失效
                            if (!HudManager.Hud_Colors.ColorsLibrary_IsExist(sp_ColoriseName.stringValue) || string.IsNullOrEmpty(sp_ColoriseName.stringValue))
                            {
                                sp_ColoriseName.stringValue = HudManager.Hud_Colors.ColorsLibrary_GetColorName(0);
                                sp_ColoriseName.serializedObject.ApplyModifiedProperties();
                            }
                        }
                        else
                        {
                            //--检查色卡名称是否失效
                            if (!HudManager.Hud_Colors.ColorsLibrary_IsExist(sp_ColoriseName.stringValue) || string.IsNullOrEmpty(sp_ColoriseName.stringValue))
                            {
                                sp_ColoriseName.stringValue = HudManager.Hud_Colors.ColorsLibrary_GetColorName(0);
                                sp_ColoriseName.serializedObject.ApplyModifiedProperties();
                            }
                        }
                    }
                }
                return;
            }
            #endregion

            XGUI.layout_flexspace();

            #region 拷贝色卡
            if (sp_SyncLibraryColor.boolValue && !Targets_Selected())
            {
                if (XGUI.layout_button(
                    tooltip: "拷贝色卡",
                    tex_release: copycolor_r,
                    tex_press: copycolor_p,
                    tex_gui_color: Color.white,
                    border: new RectOffset(0, 0, 0, 0),
                    width: 14,
                    height: 14))
                {
                    if (sp_SyncLibraryColor.boolValue)
                    {
                        Copy_Color();
                    }
                    return;
                }
            }
            #endregion

            XGUI.layout_flexspace();

            #region 识别色卡
            if (XGUI.layout_button(
                tooltip: "识别色卡",
                tex_release: recogcolor_r,
                tex_press: recogcolor_p,
                tex_gui_color: Color.white,
                border: new RectOffset(0, 0, 0, 0),
                width: 14,
                height: 14))
            {
                Recognize_Color();
                return;
            }
            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            #endregion

            #region 配色参数          
            BaseScript.fold_param = XGUI.layout_group_start(
                  type: XGUIContainerType.Horizontal,
                  bg_fill: XGUIFilled.缺口纯色边框,
                  bg_color: XGUIColor.亮白,
                  bg_color_gui: XHud_Dashboard.Theme_Group,
                  title: "配色参数",
                  title_size: XGUIFontSize.M,
                  title_text_color: XHud_Dashboard.Theme_Primary,
                  title_clipping: TextClipping.Clip,
                  padding: new RectOffset(15, 10, 20, 15),
                  foldout: BaseScript.fold_param);

            bool TextSyncing = false;

            XHud_Module_Primitive_Controller con = BaseScript.controller;

            #region 如果存在文字组件
            // 如果是 XHud_Text 受控组件类型，则获取文字组件的“同步库颜色状态开关”到 TextSyncing 状态开关
            // 表明文字组件的文字颜色是否是使用同步到库状态
            if (con.ModuleType == ModuleType.Text)
            {
                if (con.mod_Text.StyleLibSynching)
                {
                    if (con.mod_Text.TextStyleInfo.LibStyle_Effect_color)
                    {
                        TextSyncing = true;
                    }
                }
            }
            // 如果是 XHud_TmpText 受控组件类型，则获取文字组件的“同步库颜色状态开关”到 TextSyncing 状态开关
            // 表明文字组件的文字颜色是否是使用同步到库状态
            if (con.ModuleType == ModuleType.TmpText)
            {
                if (con.mod_TmpText.StyleLibSynching)
                {
                    if (con.mod_TmpText.TextStyleInfo.LibStyle_Effect_color)
                    {
                        TextSyncing = true;
                    }
                }
            }
            #endregion

            if (!BaseScript.fold_param)
            {
                if (!TextSyncing)
                {
                    if (!sp_SyncLibraryColor.boolValue)
                    {
                        #region 原始色模式
                        XGUI.layout_group_start(
                            type: XGUIContainerType.Horizontal,
                            bg_fill: XGUIFilled.透明,
                            bg_color: XGUIColor.无,
                            bg_color_gui: XHud_Dashboard.Theme_Group,
                            absolute_padding: true,
                            absolute_margin: true,
                            margin: new RectOffset(0, 0, 0, 0),
                            padding: new RectOffset(0, 0, 5, 5));

                        #region 添加到库按钮
                        if (!Targets_Selected())
                        {
                            XGUI.layout_group_start(
                                type: XGUIContainerType.Vertical,
                                bg_fill: XGUIFilled.透明,
                                bg_color: XGUIColor.无,
                                bg_color_gui: XHud_Dashboard.Theme_Group,
                                absolute_padding: true,
                                absolute_margin: true,
                                margin: new RectOffset(0, 0, 7, 0),
                                padding: new RectOffset(0, 0, 0, 0));
                            if (XGUI.layout_button(
                                tooltip: "将当前颜色添加到色卡库中",
                                tex_release: Add_r,
                                tex_press: Add_p,
                                tex_gui_color: Color.white,
                                border: new RectOffset(0, 0, 0, 0),
                                width: Add_r.width,
                                height: Add_p.height))
                            {
                                xHud_LibraryArg_Color info = new xHud_LibraryArg_Color();
                                info.Painting = BaseScript;
                                info.Color = sp_OriginalColor.colorValue;
                                OpenLibrarySetTool(info);
                            }
                            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                        }
                        #endregion

                        XGUI.layout_space(20);

                        #region 颜色
                        XGUI.ChangedCheck_Start();
                        XGUI.layout_property_field(
                             title: "原始色",
                             title_size: XGUIFontSize.M,
                             title_hover_color: XHud_Dashboard.Theme_Primary,
                             title_width: 45,
                             prop: sp_OriginalColor,
                             prop_margin: new RectOffset(0, 0, 0, 0));
                        if (XGUI.ChangedCheck_End())
                        {
                            if (Targets_Selected())
                            {
                                for (int i = 0; i < SelectedObjects.Length; i++)
                                {
                                    SelectedObjects[i].UpdateColor(sp_OriginalColor.colorValue);
                                }
                            }
                        }
                        #endregion

                        XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
                        #endregion
                    }
                    else
                    {
                        #region 色卡库模式
                        if (HudManager != null)
                        {
                            if (HudManager.Hud_Colors != null && !HudManager.Hud_Colors.ColorsLibrary_IsEmpty())
                            {
                                XGUI.layout_group_start(
                                    type: XGUIContainerType.Horizontal,
                                    bg_fill: XGUIFilled.透明,
                                    bg_color: XGUIColor.无,
                                    bg_color_gui: XHud_Dashboard.Theme_Group,
                                    absolute_padding: true,
                                    absolute_margin: true,
                                    margin: new RectOffset(0, 0, 0, 0),
                                    padding: new RectOffset(0, 0, 5, 5));

                                #region 定位到库按钮
                                if (!Targets_Selected())
                                {
                                    XGUI.layout_group_start(
                                        type: XGUIContainerType.Vertical,
                                        bg_fill: XGUIFilled.透明,
                                        bg_color: XGUIColor.无,
                                        bg_color_gui: XHud_Dashboard.Theme_Group,
                                        absolute_padding: true,
                                        absolute_margin: true,
                                        margin: new RectOffset(0, 0, 7, 0),
                                        padding: new RectOffset(0, 0, 0, 0));

                                    if (XGUI.layout_button(
                                        tooltip: "定位色卡",
                                        tex_release: locate_r,
                                        tex_press: locate_p,
                                        tex_gui_color: Color.white,
                                        border: new RectOffset(0, 0, 0, 0),
                                        width: locate_r.width,
                                        height: locate_p.height))
                                    {
                                        if (!Application.isPlaying)
                                        {
                                            if (!HudManager.Hud_Colors.ColorsLibrary_IsExist(sp_ColoriseName.stringValue))
                                                return;
                                            Editor_XHud_MenuItemsAction_OpenLibrary.open_col();
                                            HudManager.Hud_Colors.ColorsLibrary_Location(sp_ColoriseName.stringValue);
                                        }
                                        else
                                        {
                                            if (!HudManager.Hud_Colors.ColorsLibrary_IsExist(sp_ColoriseName.stringValue))
                                                return;
                                            Editor_XHud_MenuItemsAction_OpenLibrary.open_col();
                                            HudManager.Hud_Colors.ColorsLibrary_Location(sp_ColoriseName.stringValue);
                                        }
                                    }

                                    XGUI.layout_group_end(type: XGUIContainerType.Vertical);

                                    XGUI.layout_space(20);
                                }
                                #endregion

                                #region 色卡下拉菜单
                                string[] collist = HudManager.Hud_Colors.ColorsLibrary_GetColorNames();

                                #region 单/多选控件背景色逻辑
                                Color cc = Color.white;
                                bool SameColor = true;
                                string FirstColoriseName = SelectedObjects[0].ColoriseName;
                                if (!Targets_Selected())
                                {
                                    cc = HudManager.Hud_Colors.ColorsLibrary_GetColor(sp_ColoriseName.stringValue);
                                }
                                else
                                {
                                    for (int k = 1; k < SelectedObjects.Length; k++)
                                    {
                                        if (SelectedObjects[k].ColoriseName != FirstColoriseName)
                                        {
                                            SameColor = false;
                                            break;
                                        }
                                    }
                                }

                                if (SameColor)
                                {
                                    cc = HudManager.Hud_Colors.ColorsLibrary_GetColor(FirstColoriseName);
                                }
                                else
                                {
                                    cc = Color.gray;
                                }
                                #endregion

                                #region 记录撤销行为
                                if (Targets_Selected())
                                {
                                    for (int i = 0; i < SelectedObjects.Length; i++)
                                    {
                                        // 使用 Undo.RecordObject 来记录对目标对象的修改
                                        Undo.RecordObject(SelectedObjects[i], "Selected ColoriseName");
                                    }
                                }
                                else
                                {
                                    // 使用 Undo.RecordObject 来记录对目标对象的修改
                                    Undo.RecordObject(sp_ColoriseName.serializedObject.targetObject, "Selected ColoriseName");
                                }
                                #endregion

                                #region 下拉菜单
                                sp_ColoriseName.stringValue = XGUI.layout_string_popup(
                                    title: "色卡",
                                    title_width: 60,
                                    title_color: Color.white,
                                    title_size: XGUIFontSize.M,
                                    title_anchor: TextAnchor.MiddleLeft,
                                    prop: sp_ColoriseName,
                                    options: collist,
                                    opt_text_size: XGUIFontSize.M,
                                    opt_text_color: Color.black,
                                    opt_text_padding: new RectOffset(10, 10, 0, 0),
                                    opt_anchor: TextAnchor.MiddleLeft,
                                    opt_font_style: FontStyle.Normal,
                                    opt_bg_fill: XGUIFilled.实体,
                                    opt_bg_color: XGUIColor.亮白,
                                    opt_bg_color_gui: cc,
                                    margin: new RectOffset(0, 0, 5, 5),
                                    padding: new RectOffset(5, 0, 0, 0),
                                    title_margin: new RectOffset(0, 0, 0, 0),
                                    icon_arrow_color: Color.black);
                                sp_ColoriseName.serializedObject.ApplyModifiedProperties();
                                #endregion

                                #endregion

                                XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
                            }
                            else
                            {
                                XGUI.layout_label(
                                    text: "未配置色卡库",
                                    size: XGUIFontSize.M,
                                    text_color: Color.gray,
                                    margin: new RectOffset(15, 15, 0, 0),
                                    clipping: TextClipping.Clip,
                                    font_style: FontStyle.Normal,
                                    anchor: TextAnchor.MiddleCenter);
                            }
                        }
                        #endregion
                    }
                }
                else
                {
                    XGUI.layout_helpbox(
                           state: XGUIHelboxState.警告,
                           title_text: "字体颜色当前正在和字体样式库同步中，如果需要接管文字颜色请先让文字组件的同步样式关闭！",
                           title_size: XGUIFontSize.M,
                           title_style: FontStyle.Normal,
                           wrap: true,
                           title_color: Color.white * 0.75f);
                }
            }

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
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
                #region 接管状态
                DrawToggle("接管状态", sp_SyncImageColor, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, stroptions_sync, (b) => { });
                #endregion

                #region 状态调试
                DrawToggle("状态调试", sp_Debug, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, stroptions_debug, (b) => { });
                #endregion
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 菜单功能
            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                GenericMenu menu = new GenericMenu();
                menu.AddDisabledItem(new GUIContent("色卡操作"));
                menu.AddItem(new GUIContent("R (识别色卡)"), false, () =>
                {
                    Recognize_Color();
                });
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("D (拷贝色卡)"), false, () =>
                {
                    Copy_Color();
                });
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Q (折叠编组)"), false, () =>
                {
                    if (Targets_Selected())
                    {
                        for (int i = 0; i < SelectedObjects.Length; i++)
                        {
                            SelectedObjects[i].GroupFold(true);
                        }
                    }
                    else
                    {
                        BaseScript.GroupFold(true);
                    }
                });
                menu.AddItem(new GUIContent("W (展开编组)"), false, () =>
                {
                    if (Targets_Selected())
                    {
                        for (int i = 0; i < SelectedObjects.Length; i++)
                        {
                            SelectedObjects[i].GroupFold(false);
                        }
                    }
                    else
                    {
                        BaseScript.GroupFold(false);
                    }
                });
                menu.ShowAsContext();
            }
            #endregion

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

            serializedObject.ApplyModifiedProperties();
        }

        private void Copy_Color()
        {
            CopyHudColor chc = new CopyHudColor();
            chc.color = sp_OriginalColor.colorValue.r + "," + sp_OriginalColor.colorValue.g + "," + sp_OriginalColor.colorValue.b + "," + sp_OriginalColor.colorValue.a;
            chc.index = HudManager.Hud_Colors.ColorsLibrary_GetColorIndex(sp_ColoriseName.stringValue);
            chc.name = sp_ColoriseName.stringValue;
            chc.type = "HudCopyColor";
            string json = JsonUtility.ToJson(chc);

            XGUI.x_Editor_Data_Set_With_String("xData_ColorInfo", json);

            string hexcol = XGUI_Utilitys.Color_To_HexString(sp_OriginalColor.colorValue, true);

            XGUI.dialog(
               type: XGUIDialogType.确认,
               windowtitle: "XHud - 图元配色器消息",
               title: "获取色卡信息",
               msg: $"<color={hexcol}>{sp_ColoriseName.stringValue} </color>色卡信息已就绪！请选择需要识别的图元配色器后右键菜单点击识别色卡即可应用该色卡颜色！",
               ok: "明白",
               PrimaryIndex: 0,
               usemodal: true,
               themecolor: XHud_Dashboard.Theme_Primary);
        }

        private void Recognize_Color()
        {
            string buff = XGUI.x_Editor_Data_Get_With_String("xData_ColorInfo");

            CopyHudColor copyHudColor = JsonUtility.FromJson<CopyHudColor>(buff);
            string hexcol = XGUI_Utilitys.Color_To_HexString(XGUI_Utilitys.String_To_Color(copyHudColor.color, false), true);

            try
            {
                if (copyHudColor != null)
                {
                    if (Targets_Selected())
                    {
                        List<XGUIDialogListDatas> datas = new List<XGUIDialogListDatas>();
                        for (int i = 0; i < SelectedObjects.Length; i++)
                        {
                            // 如果不是色卡库模式，则询问是否要转换为色卡库模式，否则就是接赋值原始颜色值
                            if (!SelectedObjects[i].SyncLibraryColor)
                            {
                                string res = XGUI.dialog(
                               type: XGUIDialogType.警告,
                               windowtitle: "XHud - 图元配色器消息",
                               title: "批量识别色卡信息",
                               msg: $"侦测到 {SelectedObjects[i].name} ( {SelectedObjects[i].controller.Indicator} ) 处于原始色模式！是否将其转换为色卡模式？",
                               ok: "原始颜色",
                               cancel: "转换",
                               PrimaryIndex: 1,
                               usemodal: true,
                               themecolor: XHud_Dashboard.Theme_Primary);

                                if (res == "原始颜色")
                                {
                                    SelectedObjects[i].OriginalColor = XGUI_Utilitys.String_To_Color(copyHudColor.color, false);
                                }
                                else
                                {
                                    SelectedObjects[i].SyncLibraryColor = true;
                                    SelectedObjects[i].ColoriseName = copyHudColor.name;
                                }
                            }
                            else
                            {
                                SelectedObjects[i].SyncLibraryColor = true;
                                SelectedObjects[i].ColoriseName = copyHudColor.name;
                            }

                            XGUIDialogListDatas dataitem = new XGUIDialogListDatas();

                            dataitem.Title = $"色卡 <color={hexcol}>{SelectedObjects[i].ColoriseName} </color>";
                            dataitem.SubTitle = "已应用到图元配色器";
                            dataitem.Message = $"{SelectedObjects[i].name} ( {SelectedObjects[i].controller.Indicator} )";

                            datas.Add(dataitem);
                        }

                        if (datas.Count > 0)
                        {
                            string res_x = XGUI.dialog_listview(
                                datas: datas.ToArray(),
                                type: XGUIDialogType.通知,
                                windowtitle: "XHud - 图元配色器消息",
                                title: "批量识别色卡信息",
                                msg: "以下是已应用识别的色卡参数的图元控制器的配色器列表，请您检查核对：",
                                ok: "明白",
                                PrimaryIndex: 0,
                                usemodal: false,
                                themecolor: XHud_Dashboard.Theme_Primary);
                        }
                    }
                    else
                    {
                        if (sp_SyncLibraryColor.boolValue)
                        {
                            sp_ColoriseName.stringValue = copyHudColor.name;
                            sp_ColoriseName.serializedObject.ApplyModifiedProperties();
                        }
                        else
                        {
                            string res = XGUI.dialog(
                                type: XGUIDialogType.警告,
                                windowtitle: "XHud - 图元配色器消息",
                                title: "识别色卡信息",
                                msg: $"侦测到 {BaseScript.gameObject.name} ( {BaseScript.controller.Indicator} ) 处于原始色模式！是否将其转换为色卡模式？",
                                ok: "直接赋值原始颜色",
                                cancel: "转换",
                                PrimaryIndex: 1,
                                usemodal: true,
                                themecolor: XHud_Dashboard.Theme_Primary);

                            if (res == "直接赋值原始颜色")
                            {
                                sp_OriginalColor.colorValue = XGUI_Utilitys.String_To_Color(copyHudColor.color, false);
                                sp_OriginalColor.serializedObject.ApplyModifiedProperties();
                            }
                            else
                            {
                                sp_SyncLibraryColor.boolValue = true;
                                sp_SyncLibraryColor.serializedObject.ApplyModifiedProperties();
                                sp_ColoriseName.stringValue = copyHudColor.name;
                                sp_ColoriseName.serializedObject.ApplyModifiedProperties();
                            }
                        }

                        XGUI.dialog(
                            type: XGUIDialogType.确认,
                            windowtitle: "XHud - 图元配色器消息",
                            title: "识别色卡信息",
                            msg: $"已识别 xData 中的色卡信息！获取的色卡名称为： <color={hexcol}>{copyHudColor.name} </color>",
                            ok: "明白",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.Theme_Primary);
                    }
                }
            }
            catch (System.Exception e)
            {
                string msg = e.Message;

                XGUI.dialog(
                    type: XGUIDialogType.确认,
                    windowtitle: "XHud - 图元配色器消息",
                    title: "识别色卡信息",
                    msg: $"未能识别的参数！请在色卡库的其中一项上右键单击并选择 \"获取色卡信息\" 或是单击图元配色器的快捷功能按钮中的 \"拷贝色卡\" 后再试！",
                    ok: "明白",
                    PrimaryIndex: 0,
                    usemodal: true,
                    themecolor: XHud_Dashboard.Theme_Primary);
            }
        }

        /// <summary>
        /// 获取所有序列化字段
        /// </summary>
        private void GetSerializeFields()
        {
            sp_OriginalColor = serializedObject.FindProperty("OriginalColor");
            sp_ColoriseName = serializedObject.FindProperty("ColoriseName");
            sp_Debug = serializedObject.FindProperty("Debug");
            sp_SyncLibraryColor = serializedObject.FindProperty("SyncLibraryColor");
            sp_SyncImageColor = serializedObject.FindProperty("SyncImageColor");
        }

        /// <summary>
        /// 色卡库采集设置器
        /// </summary>
        public void OpenLibrarySetTool(xHud_LibraryArg_Color info)
        {
            Editor_XHud_LibrarySetTool_Color window = EditorWindow.GetWindow<Editor_XHud_LibrarySetTool_Color>(true);

            window.titleContent = new GUIContent("色卡库采集器");
            XGUI.CenterEditorWindow(new Vector2Int(348, 730), window);

            window.SetLibrarySetterMode(LibrarySetterMode.添加到库);
            window.SetInfo(info.Name, info.Color);
            window.SetPainting(info.Painting);
            window.SetButtonText("添加", "取消");
            window.SetTarget_Hud_ColorsLibrary(HudManager.Hud_Colors);
            //window.ShowModal();
            window.Show();
            window.SetTitle("色卡库采集器");
        }

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
