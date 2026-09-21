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
    [CustomEditor(typeof(XHud_Module_Primitive_Controller))]
    public class Editor_XHud_Module_Primitive_Controller : Editor
    {
        #region 组件 / 列表
        private XHud_Module_Primitive_Controller BaseScript;
        #endregion

        #region 序列化属性
        private SerializedProperty
            sp_ID,
            sp_Indicator,
            sp_ModuleType,
            sp_Debug,
            sp_IsInitial;
        #endregion

        #region GUI 参数
        /// <summary>
        /// 原始脚本参数显示开关
        /// </summary>
        private bool OriginalDisplay;
        /// <summary>
        /// 系统默认GUI行高
        /// </summary>
        private float LineHeight;
        #endregion

        #region 选项文字
        string[] stroptions_debug = new string[2] { "关闭", "调试" },
            stroptions_mute = new string[] { "正常", "静音" },
            stroptions_control = new string[] { "可控", "忽略" };
        #endregion

        #region 图标
        private Texture2D icon_type, icon_text, icon_tmptext, icon_image, icon_rawimage, icon_main, icon_initial_r, icon_initial_p, icon_recreate_id_r, icon_recreate_id_p, icon_clear_mods_r, icon_clear_mods_p;
        #endregion

        #region 批量化操作
        /// <summary>
        /// 批量选择脚本数组
        /// </summary>
        XHud_Module_Primitive_Controller[] SelectedObjects;
        /// <summary>
        /// 获取所有批量脚本目标
        /// </summary>
        private void Targets_Get()
        {
            if (targets.Length > 1)
            {
                SelectedObjects = new XHud_Module_Primitive_Controller[targets.Length];
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    var t = targets[i];
                    SelectedObjects[i] = (XHud_Module_Primitive_Controller)t;
                }
            }
            else
            {
                SelectedObjects = new XHud_Module_Primitive_Controller[targets.Length];
                SelectedObjects[0] = (XHud_Module_Primitive_Controller)target;
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
            BaseScript = (XHud_Module_Primitive_Controller)target;

            Targets_Get();

            GetSerializeFields();

            // 收集被控组件
            BaseScript.GetControlComponent();
            // 识别类型
            BaseScript.RecognizeType();

            #region 获取图标          
            icon_main = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_controller/icon_main");
            icon_text = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_controller/icon_text");
            icon_tmptext = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_controller/icon_tmptext");
            icon_image = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_controller/icon_image");
            icon_rawimage = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_controller/icon_rawimage");
            icon_initial_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_controller/icon_initial_r");
            icon_initial_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_controller/icon_initial_p");
            icon_recreate_id_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_controller/icon_recreate_id_r");
            icon_recreate_id_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_controller/icon_recreate_id_p");
            icon_clear_mods_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_controller/icon_clear_mods_r");
            icon_clear_mods_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_controller/icon_clear_mods_p");
            #endregion
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            #region 类型识别
            ModuleType m_type = (ModuleType)sp_ModuleType.enumValueIndex;
            switch (m_type)
            {
                case ModuleType.Text:
                    icon_type = icon_text;
                    break;
                case ModuleType.TmpText:
                    icon_type = icon_tmptext;
                    break;
                case ModuleType.Image:
                    icon_type = icon_image;
                    break;
                case ModuleType.RawImage:
                    icon_type = icon_rawimage;
                    break;
            }
            #endregion

            string h_color = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary);

            TextClipping clipping = XGUI.TryEllipsisClipping();

            #region 标题
            XGUI.layout_banner(
                bg_fill: XGUIFilled.实体,
                bg_color: XGUIColor.深空灰,
                bg_height: 30,
                icon: icon_main,
                icon_color: XHud_Dashboard.Theme_Primary,
                title_text: string.IsNullOrEmpty(sp_ID.stringValue) ? "XHud  -  图元  >  控制器" : "XHud - 图元  >  控制器  ( " + sp_ID.stringValue + " )",
                title_anchor: TextAnchor.MiddleLeft,
                title_style: FontStyle.Normal,
                title_color: Color.white,
                title_size: XGUIFontSize.B,
                title_clipping: clipping,
                bg_margin: new RectOffset(0, 0, 5, 5));
            #endregion

            #region 类型图标
            Rect rect_banner = XGUI.GetLastRect();
            XGUI.gui_icon(
                rect: new Rect(rect_banner.x + (rect_banner.width - 30), rect_banner.y + 8, icon_type.width, icon_type.height),
                icon: icon_type,
                color: Color.white * 0.65f);
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

            #region 建立图元扩展
            if (XGUI.layout_button(
                tooltip: "建立图元扩展",
                tex_release: icon_initial_r,
                tex_press: icon_initial_p,
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
                        data.Title = string.IsNullOrEmpty(SelectedObjects[i].Indicator) ? SelectedObjects[i].name : SelectedObjects[i].Indicator;
                        data.SubTitle = "即将初始化图元结构";
                        data.Message = "";
                        datas.Add(data);
                    }

                    EditorApplication.delayCall += () =>
                    {
                        if (datas.Count > 0)
                        {
                            string res_x = XGUI.dialog_listview(
                                datas: datas.ToArray(),
                                type: XGUIDialogType.通知,
                                windowtitle: "XHud - 图元控制器消息",
                                title: "批量初始化图元结构",
                                msg: "是否需要批量为图元建立控制脚本结构吗？",
                                ok: "建立",
                                cancel: "暂不",
                                PrimaryIndex: 0,
                                usemodal: false,
                                themecolor: XHud_Dashboard.Theme_Primary);

                            if (res_x == "建立")
                            {
                                for (int i = 0; i < SelectedObjects.Length; i++)
                                {
                                    if (SelectedObjects[i].IsInitial)
                                    {
                                        string csd = XGUI.dialog(
                                            type: XGUIDialogType.警告,
                                            windowtitle: "XHud - 图元控制器消息",
                                            title: "重新初始化",
                                            msg: $"确认为 {SelectedObjects[i].gameObject.name} 图元重新建立控制脚本结构吗？",
                                            ok: "重建",
                                            cancel: "暂不",
                                            PrimaryIndex: 0,
                                            usemodal: true,
                                            themecolor: XHud_Dashboard.Theme_Primary);

                                        if (csd == "重建")
                                        {
                                            SelectedObjects[i].ClearComponents_For_Editor();
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }
                                    SelectedObjects[i].InitialComponents_For_Editor();
                                }
                            }
                        }
                    };
                }
                else
                {
                    EditorApplication.delayCall += () =>
                    {
                        string res = XGUI.dialog(
                                          type: XGUIDialogType.警告,
                                          windowtitle: "XHud - 图元控制器消息",
                                          title: "初始化图元结构",
                                          msg: $"确认为此图元建立控制脚本结构吗？",
                                          ok: "建立",
                                          cancel: "暂不",
                                          PrimaryIndex: 0,
                                          usemodal: true,
                                          themecolor: XHud_Dashboard.Theme_Primary);

                        if (res == "建立")
                        {
                            if (sp_IsInitial.boolValue)
                            {
                                string csd = XGUI.dialog(
                                    type: XGUIDialogType.警告,
                                    windowtitle: "XHud - 图元控制器消息",
                                    title: "重新初始化",
                                    msg: $"此图元已经创建了模块结构，确认重新创建模块结构吗？",
                                    ok: "重建",
                                    cancel: "暂不",
                                    PrimaryIndex: 0,
                                    usemodal: true,
                                    themecolor: XHud_Dashboard.Theme_Primary);

                                if (csd == "重建")
                                {
                                    BaseScript.ClearComponents_For_Editor();
                                }
                                else
                                {
                                    return;
                                }
                            }
                            BaseScript.InitialComponents_For_Editor();
                        }
                    };
                }
                return;
            }
            #endregion

            XGUI.layout_flexspace();

            #region 重分配图元控制器ID
            if (XGUI.layout_button(
                tooltip: "重分配图元控制器ID",
                tex_release: icon_recreate_id_r,
                tex_press: icon_recreate_id_p,
                tex_gui_color: Color.white,
                border: new RectOffset(0, 0, 0, 0),
                width: 14,
                height: 14))
            {
                if (Targets_Selected())
                {
                    List<XGUIDialogListDatas> Datas = new List<XGUIDialogListDatas>();

                    for (int i = 0; i < SelectedObjects.Length; i++)
                    {
                        XGUIDialogListDatas data = new XGUIDialogListDatas();
                        data.Title = string.IsNullOrEmpty(SelectedObjects[i].Indicator) ? SelectedObjects[i].name : SelectedObjects[i].Indicator;
                        data.SubTitle = "即将批量更新图元控制器ID";
                        data.Message = "";
                        Datas.Add(data);
                    }

                    EditorApplication.delayCall += () =>
                    {
                        string res_x = XGUI.dialog_listview(
                            datas: Datas.ToArray(),
                            type: XGUIDialogType.通知,
                            windowtitle: "XHud - 图元控制器消息",
                            title: "批量控制器ID更新",
                            msg: "是否需要批量更新控制器的ID吗？",
                            ok: "更新",
                            cancel: "暂不",
                            PrimaryIndex: 0,
                            usemodal: false,
                            themecolor: XHud_Dashboard.Theme_Primary);

                        if (res_x == "更新")
                        {
                            for (int i = 0; i < SelectedObjects.Length; i++)
                            {
                                Undo.RecordObject(SelectedObjects[i], "更新所有图元控制器的ID");

                                XHud_Module_Primitive_Controller con = SelectedObjects[i];

                                #region 从父物体找元素并获取元素下所有图元控制器的ID
                                XHud_Module_Element parent_ele = con.GetComponentInParent<XHud_Module_Element>();

                                if (parent_ele == null)
                                    continue;

                                if (parent_ele.PrimitiveControllerNodes == null && parent_ele.PrimitiveControllerNodes.Count <= 0)
                                    continue;

                                List<string> list = new List<string>();
                                for (int s = 0; s < parent_ele.PrimitiveControllerNodes.Count; s++)
                                {
                                    list.Add(parent_ele.PrimitiveControllerNodes[s].Controller.ID);
                                }
                                #endregion

                                con.ID = XGUI_Utilitys.GenerateUniqueId(list.ToArray());
                            }
                        }
                    };
                }
                else
                {
                    EditorApplication.delayCall += () =>
                    {
                        string res = XGUI.dialog(
                                 type: XGUIDialogType.警告,
                                 windowtitle: "XHud - 图元控制器消息",
                                 title: "控制器ID更新",
                                 msg: $"确定要更新当前图元控制器的ID吗？",
                                 ok: "更新 ID",
                                 cancel: "暂不",
                                 PrimaryIndex: 0,
                                 usemodal: true,
                                 themecolor: XHud_Dashboard.Theme_Primary);

                        if (res == "暂不")
                            return;

                        Undo.RecordObject(BaseScript, "刷新图元控制器ID");
                        BaseScript.ID = XGUI_Utilitys.GenerateUniqueId(CollectIDs());
                    };
                }
                return;
            }
            #endregion

            XGUI.layout_flexspace();

            #region 清空图元结构
            if (XGUI.layout_button(
                tooltip: "清空图元结构",
                tex_release: icon_clear_mods_r,
                tex_press: icon_clear_mods_p,
                tex_gui_color: Color.white,
                border: new RectOffset(0, 0, 0, 0),
                width: 14,
                height: 14))
            {
                if (Targets_Selected())
                {
                    List<XGUIDialogListDatas> Datas = new List<XGUIDialogListDatas>();

                    for (int i = 0; i < SelectedObjects.Length; i++)
                    {
                        XGUIDialogListDatas data = new XGUIDialogListDatas();
                        data.Title = string.IsNullOrEmpty(SelectedObjects[i].Indicator) ? SelectedObjects[i].name : SelectedObjects[i].Indicator;
                        data.SubTitle = "即将移除图元控制器建立的扩展结构";
                        data.Message = "";
                        Datas.Add(data);
                    }

                    EditorApplication.delayCall += () =>
                    {
                        string res_x = XGUI.dialog_listview(
                         datas: Datas.ToArray(),
                         type: XGUIDialogType.通知,
                         windowtitle: "XHud - 图元控制器消息",
                         title: "批量移除建立的扩展结构",
                         msg: "是否需要批量移除图元控制器建立的扩展结构吗？",
                         ok: "移除",
                         cancel: "暂不",
                         PrimaryIndex: 0,
                         usemodal: false,
                         themecolor: XHud_Dashboard.Theme_Primary);

                        if (res_x == "移除")
                        {
                            for (int i = 0; i < SelectedObjects.Length; i++)
                            {
                                XHud_Module_Primitive_Controller con = SelectedObjects[i];

                                if (con.pt_Feature != null)
                                    Undo.DestroyObjectImmediate(con.pt_Feature);

                                XHud_Module_Primitive_Painting_Synchronizer Synchronizer = con.GetComponent<XHud_Module_Primitive_Painting_Synchronizer>();

                                if (Synchronizer != null)
                                    Undo.DestroyObjectImmediate(Synchronizer);

                                if (con.pt_Painting != null)
                                    Undo.DestroyObjectImmediate(con.pt_Painting);

                                if (con.pt_Tween != null)
                                    Undo.DestroyObjectImmediate(con.pt_Tween);

                                if (con.mod_CanvasGroup != null)
                                    Undo.DestroyObjectImmediate(con.mod_CanvasGroup);

                                con.IsInitial = false;
                                con.pt_Feature = null;
                                con.pt_Painting = null;
                                con.pt_Tween = null;
                                con.mod_CanvasGroup = null;
                            }
                        }
                    };
                }
                else
                {
                    EditorApplication.delayCall += () =>
                    {
                        string res = XGUI.dialog(
                                type: XGUIDialogType.警告,
                                windowtitle: "XHud - 图元控制器消息",
                                title: "移除建立的扩展结构",
                                msg: $"确定要移除图元控制器建立的扩展结构吗？",
                                ok: "移除",
                                cancel: "暂不",
                                PrimaryIndex: 0,
                                usemodal: true,
                                themecolor: XHud_Dashboard.Theme_Primary);

                        if (res == "暂不")
                            return;

                        if (BaseScript.pt_Feature != null)
                            Undo.DestroyObjectImmediate(BaseScript.pt_Feature);

                        XHud_Module_Primitive_Painting_Synchronizer Synchronizer = BaseScript.GetComponent<XHud_Module_Primitive_Painting_Synchronizer>();

                        if (Synchronizer != null)
                            Undo.DestroyObjectImmediate(Synchronizer);

                        if (BaseScript.pt_Painting != null)
                            Undo.DestroyObjectImmediate(BaseScript.pt_Painting);

                        if (BaseScript.pt_Tween != null)
                            Undo.DestroyObjectImmediate(BaseScript.pt_Tween);

                        if (BaseScript.mod_CanvasGroup != null)
                            Undo.DestroyObjectImmediate(BaseScript.mod_CanvasGroup);

                        BaseScript.IsInitial = false;
                        BaseScript.pt_Feature = null;
                        BaseScript.pt_Painting = null;
                        BaseScript.pt_Tween = null;
                        BaseScript.mod_CanvasGroup = null;
                    };
                }
                return;
            }
            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
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
                #region ID
                XGUI.layout_property_field(
                    title: "ID",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: sp_ID,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                #region 标识
                XGUI.layout_property_field(
                    title: "标识",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: sp_Indicator,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                #region 类型
                XGUI.layout_property_field(
                    title: "类型",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: sp_ModuleType,
                    prop_margin: new RectOffset(0, 0, 5, 0));
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
                DrawToggle("状态调试", sp_Debug, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, stroptions_debug, (b) => { });
                #endregion          
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
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

        private string[] CollectIDs()
        {
            XHud_Module_Element parent_ele = BaseScript.GetComponentInParent<XHud_Module_Element>();

            if (parent_ele == null)
                return null;

            if (parent_ele.PrimitiveControllerNodes == null && parent_ele.PrimitiveControllerNodes.Count <= 0)
                return null;

            List<string> list = new List<string>();
            for (int i = 0; i < parent_ele.PrimitiveControllerNodes.Count; i++)
            {
                list.Add(parent_ele.PrimitiveControllerNodes[i].Controller.ID);
            }

            return list.ToArray();
        }

        /// <summary>
        /// 获取所有序列化字段
        /// </summary>
        private void GetSerializeFields()
        {
            sp_ID = serializedObject.FindProperty("ID");
            sp_Indicator = serializedObject.FindProperty("Indicator");
            sp_ModuleType = serializedObject.FindProperty("ModuleType");
            sp_Debug = serializedObject.FindProperty("Debug");
            sp_IsInitial = serializedObject.FindProperty("IsInitial");
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
