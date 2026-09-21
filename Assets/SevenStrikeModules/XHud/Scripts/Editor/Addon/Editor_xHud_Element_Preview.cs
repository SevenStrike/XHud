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
    using System;
    using UnityEditor;
    using UnityEngine;

    public class XHud_Element_PreviewArgs
    {
        public float DelayWithIn;
        public float DelayWithOut;
        public KeyCode key_Element_In;
        public KeyCode key_Element_Out;

        public bool HideWithStart;

        public Motion_Creator CreateArgs;
        public Motion_Recycler RecycleArgs;

        public KeyCode key_create;
        public KeyCode key_recycle;
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(XHud_Element_Preview))]
    public class Editor_XHud_Element_Preview : Editor
    {
        #region 组件
        private XHud_Element_Preview BaseScript;
        private XHud_Manager HudManager;
        #endregion

        #region 序列化属性
        SerializedProperty IsEnable, DebugState, DurationScaler, Preset_Position, Preset_Euler, create_fold_move, create_fold_rotate, create_fold_alpha, recycle_fold_move, recycle_fold_rotate, recycle_fold_alpha, key_Element_In, key_Element_Out, HideWithStart, Crc_Lib_Name, Rec_Lib_Name, CreateArgs, RecycleArgs, DelayWithIn, DelayWithOut, RMS_Enabled, RMS_Name, HudElement, CreateArgs_MotionAnimateEndState, RecycleArgs_MotionAnimateEndState, previewIsRunning;
        #endregion

        #region 图标
        private Texture2D
            icon_main,
            save_r,
            save_p,
            locate_r,
            locate_p,
            reset_r,
            reset_p;
        #endregion

        #region 批量化操作
        private XHud_Element_Preview[] SelectedObjects;

        private void Targets_Get()
        {
            if (targets.Length > 1)
            {
                SelectedObjects = new XHud_Element_Preview[targets.Length];
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    var t = targets[i];
                    SelectedObjects[i] = (XHud_Element_Preview)t;
                }
            }
            else
            {
                SelectedObjects = new XHud_Element_Preview[targets.Length];
                SelectedObjects[0] = (XHud_Element_Preview)target;
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

            BaseScript = (XHud_Element_Preview)target;

            #region 获取序列化属性
            HudElement = serializedObject.FindProperty("HudElement");
            key_Element_In = serializedObject.FindProperty("key_Element_In");
            key_Element_Out = serializedObject.FindProperty("key_Element_Out");
            CreateArgs = serializedObject.FindProperty("CreateArgs");
            RecycleArgs = serializedObject.FindProperty("RecycleArgs");
            HideWithStart = serializedObject.FindProperty("HideWithStart");
            Crc_Lib_Name = serializedObject.FindProperty("Crc_Lib_Name");
            Rec_Lib_Name = serializedObject.FindProperty("Rec_Lib_Name");
            DelayWithIn = serializedObject.FindProperty("DelayWithIn");
            DelayWithOut = serializedObject.FindProperty("DelayWithOut");
            IsEnable = serializedObject.FindProperty("IsEnable");
            DebugState = serializedObject.FindProperty("DebugState");
            create_fold_move = serializedObject.FindProperty("create_fold_move");
            create_fold_rotate = serializedObject.FindProperty("create_fold_rotate");
            create_fold_alpha = serializedObject.FindProperty("create_fold_alpha");
            recycle_fold_move = serializedObject.FindProperty("recycle_fold_move");
            recycle_fold_rotate = serializedObject.FindProperty("recycle_fold_rotate");
            recycle_fold_alpha = serializedObject.FindProperty("recycle_fold_alpha");
            Preset_Position = serializedObject.FindProperty("Preset_Position");
            Preset_Euler = serializedObject.FindProperty("Preset_Euler");
            DurationScaler = serializedObject.FindProperty("DurationScaler");
            RMS_Enabled = serializedObject.FindProperty("RMS_Enabled");
            RMS_Name = serializedObject.FindProperty("RMS_Name");
            CreateArgs_MotionAnimateEndState = CreateArgs.FindPropertyRelative("MotionAnimateEndState");
            RecycleArgs_MotionAnimateEndState = RecycleArgs.FindPropertyRelative("MotionAnimateEndState");
            previewIsRunning = serializedObject.FindProperty("previewIsRunning");
            #endregion

            #region 获取图标
            icon_main = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_element_preview/icon_main");
            save_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_element_preview/save_r");
            save_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_element_preview/save_p");
            locate_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_element_preview/locate_r");
            locate_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_element_preview/locate_p");
            reset_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_element_preview/reset_r");
            reset_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_element_preview/reset_p");
            #endregion

            Targets_Get();

            CheckRmsNameValid();

            if (Targets_Selected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    if (SelectedObjects[i].HudElement == null)
                    {
                        SelectedObjects[i].HudElement = SelectedObjects[i].GetComponent<XHud_Module_Element>();
                    }
                    else
                    {
                        if (SelectedObjects[i].HudElement.GetInstanceID() != SelectedObjects[i].GetInstanceID())
                        {
                            SelectedObjects[i].HudElement = SelectedObjects[i].GetComponent<XHud_Module_Element>();
                        }
                    }
                }
            }
            else
            {
                if (HudElement.objectReferenceValue == null)
                {
                    HudElement.objectReferenceValue = BaseScript.GetComponent<XHud_Module_Element>();
                    HudElement.serializedObject.ApplyModifiedProperties();
                }
                else
                {
                    if (HudElement.objectReferenceValue.GetInstanceID() != BaseScript.GetInstanceID())
                    {
                        HudElement.objectReferenceValue = BaseScript.GetComponent<XHud_Module_Element>();
                        HudElement.serializedObject.ApplyModifiedProperties();
                    }
                }
            }
        }

        private void OnDisable()
        {

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
             title_text: "XHud  -  元素预览器",
             title_anchor: TextAnchor.MiddleLeft,
             title_style: FontStyle.Normal,
             title_color: Color.white,
             title_size: XGUIFontSize.B,
             title_clipping: clipping,
             bg_margin: new RectOffset(0, 0, 5, 5));
            #endregion

            XGUI.layout_space(5);

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
                padding: new RectOffset(10, 10, 15, 15),
                foldout: BaseScript.fold_param);

            if (!BaseScript.fold_param)
            {
                #region 元素组件
                XGUI.layout_property_field(
                    title: "元素组件",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: HudElement,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                #region 延时进入
                XGUI.layout_property_field(
                    title: "延时进入",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: DelayWithIn,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                #region 延时退出
                XGUI.layout_property_field(
                    title: "延时退出",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: DelayWithOut,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                #region 缩放耗时
                XGUI.layout_property_field(
                    title: "缩放耗时",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: DurationScaler,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 预览按键
            BaseScript.fold_key = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.Theme_Group,
                title: "预览按键",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.Theme_Primary,
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: BaseScript.fold_key);

            if (!BaseScript.fold_key)
            {
                #region 元素进入
                XGUI.layout_property_field(
                    title: "元素进入",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: key_Element_In,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                #region 元素退出
                XGUI.layout_property_field(
                    title: "元素退出",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: key_Element_Out,
                    prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 状态
            BaseScript.fold_state = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.Theme_Group,
                title: "状态",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.Theme_Primary,
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: BaseScript.fold_state);

            if (!BaseScript.fold_state)
            {
                #region 动效状态
                XGUI.layout_state_displayer_text(
                    title: "动效状态",
                    title_size: XGUIFontSize.M,
                    subtitle: previewIsRunning.boolValue ? "动效中" : "待命中",
                    subtitle_size: XGUIFontSize.M,
                    subtitle_color: previewIsRunning.boolValue ? XHud_Dashboard.Theme_Primary : Color.gray,
                    margin: new RectOffset(5, 5, 0, 5));
                #endregion
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 选项
            BaseScript.fold_options = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.Theme_Group,
                title: "选项",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.Theme_Primary,
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: BaseScript.fold_options);

            if (!BaseScript.fold_options)
            {
                DrawToggle("状态调试", DebugState, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, (b) => { });
                DrawToggle("预览开关", IsEnable, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, (b) =>
                {

                });
                DrawToggle("开始时隐藏", HideWithStart, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, (b) =>
                {

                });
                DrawToggle("R M S 模式", RMS_Enabled, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, (b) =>
                {
                    CheckRmsNameValid();
                });

                #region RMS 说明
                if (RMS_Enabled.boolValue)
                {
                    string msg = $"请注意！如果您的预览目标<color={hexcol}><b>元素自身的 RMS 未开启或无效</b></color>，则不会根据 RMS的参数<color={hexcol}><b>（指定且存在RMS配置） </b></color>进行元素的<color={hexcol}><b>变换姿态设定</b></color>赋值！";

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
                        clipping: TextClipping.Clip,
                        wrap: true,
                        font: XGUI.GetFont("xg-regular"),
                        font_style: FontStyle.Normal);
                }
                #endregion
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 预设变换信息
            BaseScript.fold_preset_trans = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.Theme_Group,
                title: "预设变换信息",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.Theme_Primary,
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: BaseScript.fold_preset_trans);

            if (!BaseScript.fold_preset_trans)
            {
                DrawParamField("预设原始位置", Preset_Position, 100, Color.clear);
                DrawParamField("预设原始角度", Preset_Euler, 100, Color.clear);

                #region 记录当前位置 & 角度
                if (XGUI.layout_button(
                    text: "记录当前位置 & 角度",
                    tooltip: "",
                    bg_fill: XGUIFilled.实体,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.Theme_Primary,
                    button_text_color: Color.black,
                    press_fill: XGUIFilled.实体,
                    press_color: XGUIColor.深空灰,
                    press_text_color: Color.white,
                    font_size: XGUIFontSize.M,
                    anchor: TextAnchor.MiddleCenter,
                    margin: new RectOffset(0, 0, 15, 0),
                    padding: new RectOffset(0, 0, 0, 0),
                    height: 20,
                    button_text_font: XGUI.GetFont("xg-medium")))
                {
                    if (Targets_Selected())
                    {
                        for (int i = 0; i < SelectedObjects.Length; i++)
                        {
                            if (SelectedObjects[i].HudElement == null)
                                continue;
                            SelectedObjects[i].Preset_Position = SelectedObjects[i].HudElement.RectTransform.anchoredPosition3D;
                            SelectedObjects[i].Preset_Euler = SelectedObjects[i].HudElement.RectTransform.localEulerAngles;
                        }
                    }
                    else
                    {
                        if (BaseScript.HudElement == null)
                            return;
                        Preset_Position.vector3Value = BaseScript.HudElement.RectTransform.anchoredPosition3D;
                        Preset_Euler.vector3Value = BaseScript.HudElement.RectTransform.localEulerAngles;
                        Preset_Position.serializedObject.ApplyModifiedProperties();
                        Preset_Euler.serializedObject.ApplyModifiedProperties();
                    }
                }
                #endregion
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region RMS布局信息
            if (RMS_Enabled.boolValue)
            {
                BaseScript.fold_rms = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIFilled.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.Theme_Group,
                    title: "RMS 布局信息",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.Theme_Primary,
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: BaseScript.fold_rms);

                if (!BaseScript.fold_rms)
                {
                    #region 分辨率方案列表
                    ScreenResolutionNode[] nodes = HudManager.hm_RMS_GetResolutionNodes();
                    if (nodes.Length > 0)
                    {
                        string[] nodesName = HudManager.hm_RMS_GetResolutionNodeNames();
                        RMS_Name.stringValue = XGUI.layout_string_popup(
                          title: "画布尺寸",
                          title_width: 60,
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
                          icon_arrow_color: Color.black,
                          margin: new RectOffset(0, 0, 5, 5),
                          padding: new RectOffset(5, 5, 0, 0),
                          title_margin: new RectOffset(0, 0, 0, 0),
                          act_on_changed: (value) =>
                          {

                          });
                        RMS_Name.serializedObject.ApplyModifiedProperties();

                        XGUI.layout_seperator(
                            thickness: 1,
                            color: XHud_Dashboard.Theme_SeperateLine,
                            margin: new RectOffset(15, 15, 15, 15));

                        Element_RMS_LayoutData info = BaseScript.HudElement.elelemt_RMS_Get(RMS_Name.stringValue);

                        if (info != null)
                        {
                            #region 锚点
                            XGUI.layout_state_displayer_text(
                                title: "锚点",
                                title_size: XGUIFontSize.M,
                                subtitle: info.Anchor.ToString(),
                                subtitle_size: XGUIFontSize.M,
                                subtitle_color: XHud_Dashboard.Theme_Primary,
                                margin: new RectOffset(5, 5, 0, 5));
                            #endregion

                            #region 最小锚点
                            XGUI.layout_state_displayer_text(
                                title: "最小锚点",
                                title_size: XGUIFontSize.M,
                                subtitle: info.AnchorMin.ToString(),
                                subtitle_size: XGUIFontSize.M,
                                subtitle_color: XHud_Dashboard.Theme_Primary,
                                margin: new RectOffset(5, 5, 0, 5));
                            #endregion

                            #region 最大锚点
                            XGUI.layout_state_displayer_text(
                                title: "最大锚点",
                                title_size: XGUIFontSize.M,
                                subtitle: info.AnchorMax.ToString(),
                                subtitle_size: XGUIFontSize.M,
                                subtitle_color: XHud_Dashboard.Theme_Primary,
                                margin: new RectOffset(5, 5, 0, 5));
                            #endregion

                            #region 位置
                            XGUI.layout_state_displayer_text(
                                title: "位置",
                                title_size: XGUIFontSize.M,
                                subtitle: info.Position.ToString(),
                                subtitle_size: XGUIFontSize.M,
                                subtitle_color: XHud_Dashboard.Theme_Primary,
                                margin: new RectOffset(5, 5, 0, 5));
                            #endregion

                            #region 角度
                            XGUI.layout_state_displayer_text(
                                title: "角度",
                                title_size: XGUIFontSize.M,
                                subtitle: info.Euler.ToString(),
                                subtitle_size: XGUIFontSize.M,
                                subtitle_color: XHud_Dashboard.Theme_Primary,
                                margin: new RectOffset(5, 5, 0, 5));
                            #endregion

                            #region 缩放
                            XGUI.layout_state_displayer_text(
                                title: "缩放",
                                title_size: XGUIFontSize.M,
                                subtitle: info.Scale.ToString(),
                                subtitle_size: XGUIFontSize.M,
                                subtitle_color: XHud_Dashboard.Theme_Primary,
                                margin: new RectOffset(5, 5, 0, 5));
                            #endregion

                            #region 轴心
                            XGUI.layout_state_displayer_text(
                                title: "轴心",
                                title_size: XGUIFontSize.M,
                                subtitle: info.Pivot.ToString(),
                                subtitle_size: XGUIFontSize.M,
                                subtitle_color: XHud_Dashboard.Theme_Primary,
                                margin: new RectOffset(5, 5, 0, 5));
                            #endregion
                        }
                        else
                        {
                            XGUI.layout_label(
                                text: "暂未在元素上找到 RMS 设计布局信息",
                                size: XGUIFontSize.M,
                                text_color: Color.gray,
                                padding: new RectOffset(0, 0, 0, -10),
                                margin: new RectOffset(0, 0, 15, 35),
                                clipping: TextClipping.Clip,
                                font_style: FontStyle.Normal,
                                anchor: TextAnchor.MiddleCenter);
                        }
                    }
                    else
                    {
                        XGUI.layout_label(
                            text: "暂未在管理器中配置 RMS 布局列表",
                            size: XGUIFontSize.M,
                            text_color: Color.gray,
                            padding: new RectOffset(0, 0, 0, -10),
                            margin: new RectOffset(0, 0, 15, 35),
                            clipping: TextClipping.Clip,
                            font_style: FontStyle.Normal,
                            anchor: TextAnchor.MiddleCenter);
                    }
                    #endregion
                }

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            }
            #endregion

            #region 元素动效参数
            BaseScript.fold_motion = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.Theme_Group,
                title: "元素动效参数",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.Theme_Primary,
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: BaseScript.fold_motion);

            if (!BaseScript.fold_motion)
            {
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
                            padding: new RectOffset(10, 10, 0, 0));

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
                                windowtitle: "XHud - 元素预览器消息",
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
                          title_text: "Hud管理器中未指定动效库，请先配置动效库！",
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
                create_fold_move.boolValue = EditorGUILayout.Foldout(create_fold_move.boolValue, "位移", true);

                if (create_fold_move.boolValue)
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
                create_fold_rotate.boolValue = EditorGUILayout.Foldout(create_fold_rotate.boolValue, "旋转", true);

                if (create_fold_rotate.boolValue)
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
                create_fold_alpha.boolValue = EditorGUILayout.Foldout(create_fold_alpha.boolValue, "透明度", true);

                if (create_fold_alpha.boolValue)
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
                EditorGUI.BeginChangeCheck();
                XGUI.layout_property_field(
                       title: "动效结束时机",
                       title_size: XGUIFontSize.M,
                       title_hover_color: XHud_Dashboard.Theme_Primary,
                       title_width: 90,
                       prop: CreateArgs_MotionAnimateEndState,
                       prop_margin: new RectOffset(0, 0, 15, 0));
                if (EditorGUI.EndChangeCheck())
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
                                    windowtitle: "XHud - 元素预览器消息",
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
                                    windowtitle: "XHud - 元素预览器消息",
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

                XGUI.layout_seperator(
                    thickness: 3,
                    color: XHud_Dashboard.Theme_SeperateLine * 1.5f,
                    margin: new RectOffset(15, 15, 18, 15));

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
                            padding: new RectOffset(10, 10, 0, 0));

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
                                windowtitle: "XHud - 元素预览器消息",
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
                                        Rec_Lib_Name.stringValue = null;
                                        Rec_Lib_Name.serializedObject.ApplyModifiedProperties();
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
                          title_text: "Hud管理器中未指定动效库，请先配置动效库！",
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
                recycle_fold_move.boolValue = EditorGUILayout.Foldout(recycle_fold_move.boolValue, "位移", true);

                if (recycle_fold_move.boolValue)
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
                recycle_fold_rotate.boolValue = EditorGUILayout.Foldout(recycle_fold_rotate.boolValue, "旋转", true);

                if (recycle_fold_rotate.boolValue)
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
                recycle_fold_alpha.boolValue = EditorGUILayout.Foldout(recycle_fold_alpha.boolValue, "透明度", true);

                if (recycle_fold_alpha.boolValue)
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
                EditorGUI.BeginChangeCheck();
                XGUI.layout_property_field(
                    title: "动效结束时机",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: RecycleArgs_MotionAnimateEndState,
                    prop_margin: new RectOffset(0, 0, 15, 0));
                if (EditorGUI.EndChangeCheck())
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
                                    windowtitle: "XHud - 元素预览器消息",
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
                                    windowtitle: "XHud - 元素预览器消息",
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
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 菜单
            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                if (Targets_Selected())
                    return;

                SerializedProperty sp_DelayWithIn = serializedObject.FindProperty("DelayWithIn");
                SerializedProperty sp_DelayWithOut = serializedObject.FindProperty("DelayWithOut");
                SerializedProperty sp_key_Element_In = serializedObject.FindProperty("key_Element_In");
                SerializedProperty sp_key_Element_Out = serializedObject.FindProperty("key_Element_Out");
                SerializedProperty sp_HideWithStart = serializedObject.FindProperty("HideWithStart");

                SerializedProperty sp_CreateArgs = serializedObject.FindProperty("CreateArgs");
                SerializedProperty sp_RecycleArgs = serializedObject.FindProperty("RecycleArgs");


                // 创建右键菜单
                GenericMenu menu = new GenericMenu();

                menu.AddItem(new GUIContent("C (拷贝脚本参数)"), false, () =>
                {
                    XHud_Element_PreviewArgs hpp = new XHud_Element_PreviewArgs();

                    #region CreateArgs
                    Motion_Creator crc = new Motion_Creator();
                    crc.anchor = (XHudAnchor)sp_CreateArgs.FindPropertyRelative("anchor").enumValueIndex;

                    MotionNode_Movement hm_m = new MotionNode_Movement();
                    hm_m.Movement = (HudMotion_Movement)sp_CreateArgs.FindPropertyRelative("Movement.Movement").enumValueIndex;
                    hm_m.Distance = sp_CreateArgs.FindPropertyRelative("Movement.Distance").floatValue;
                    hm_m.Duration = sp_CreateArgs.FindPropertyRelative("Movement.Duration").floatValue;
                    hm_m.Delay = sp_CreateArgs.FindPropertyRelative("Movement.Delay").floatValue;
                    hm_m.Ease = (EaseMode)sp_CreateArgs.FindPropertyRelative("Movement.Ease").enumValueIndex;
                    hm_m.Curve = sp_CreateArgs.FindPropertyRelative("Movement.Curve").animationCurveValue;
                    hm_m.CurveName = sp_CreateArgs.FindPropertyRelative("Movement.CurveName").stringValue;

                    MotionNode_Rotation hm_r = new MotionNode_Rotation();
                    hm_r.Rotation = (HudMotion_Rotation)sp_CreateArgs.FindPropertyRelative("Rotation.Rotation").enumValueIndex;
                    hm_r.Degree = sp_CreateArgs.FindPropertyRelative("Rotation.Degree").floatValue;
                    hm_r.Duration = sp_CreateArgs.FindPropertyRelative("Rotation.Duration").floatValue;
                    hm_r.Delay = sp_CreateArgs.FindPropertyRelative("Rotation.Delay").floatValue;
                    hm_r.Ease = (EaseMode)sp_CreateArgs.FindPropertyRelative("Rotation.Ease").enumValueIndex;
                    hm_r.Curve = sp_CreateArgs.FindPropertyRelative("Rotation.Curve").animationCurveValue;
                    hm_r.CurveName = sp_CreateArgs.FindPropertyRelative("Rotation.CurveName").stringValue;

                    MotionNode_Alpha hm_a = new MotionNode_Alpha();
                    hm_a.Duration = sp_CreateArgs.FindPropertyRelative("Alpha.Duration").floatValue;
                    hm_a.Curve = sp_CreateArgs.FindPropertyRelative("Alpha.Curve").animationCurveValue;
                    hm_a.CurveName = sp_CreateArgs.FindPropertyRelative("Alpha.CurveName").stringValue;
                    hm_a.Ease = (EaseMode)sp_CreateArgs.FindPropertyRelative("Alpha.Ease").enumValueIndex;
                    hm_a.Delay = sp_CreateArgs.FindPropertyRelative("Alpha.Delay").floatValue;

                    crc.Movement = hm_m;
                    crc.Rotation = hm_r;
                    crc.Alpha = hm_a;
                    hpp.CreateArgs = crc;
                    #endregion

                    #region RecycleArgs
                    Motion_Recycler rec = new Motion_Recycler();

                    MotionNode_Movement rec_hm_m = new MotionNode_Movement();
                    rec_hm_m.Movement = (HudMotion_Movement)sp_RecycleArgs.FindPropertyRelative("Movement.Movement").enumValueIndex;
                    rec_hm_m.Distance = sp_RecycleArgs.FindPropertyRelative("Movement.Distance").floatValue;
                    rec_hm_m.Duration = sp_RecycleArgs.FindPropertyRelative("Movement.Duration").floatValue;
                    rec_hm_m.Delay = sp_RecycleArgs.FindPropertyRelative("Movement.Delay").floatValue;
                    rec_hm_m.Ease = (EaseMode)sp_RecycleArgs.FindPropertyRelative("Movement.Ease").enumValueIndex;
                    rec_hm_m.Curve = sp_RecycleArgs.FindPropertyRelative("Movement.Curve").animationCurveValue;
                    rec_hm_m.CurveName = sp_RecycleArgs.FindPropertyRelative("Movement.CurveName").stringValue;

                    MotionNode_Rotation rec_hm_r = new MotionNode_Rotation();
                    rec_hm_r.Rotation = (HudMotion_Rotation)sp_RecycleArgs.FindPropertyRelative("Rotation.Rotation").enumValueIndex;
                    rec_hm_r.Degree = sp_RecycleArgs.FindPropertyRelative("Rotation.Degree").floatValue;
                    rec_hm_r.Duration = sp_RecycleArgs.FindPropertyRelative("Rotation.Duration").floatValue;
                    rec_hm_r.Delay = sp_RecycleArgs.FindPropertyRelative("Rotation.Delay").floatValue;
                    rec_hm_r.Ease = (EaseMode)sp_RecycleArgs.FindPropertyRelative("Rotation.Ease").enumValueIndex;
                    rec_hm_r.Curve = sp_RecycleArgs.FindPropertyRelative("Rotation.Curve").animationCurveValue;
                    rec_hm_r.CurveName = sp_RecycleArgs.FindPropertyRelative("Rotation.CurveName").stringValue;

                    MotionNode_Alpha rec_hm_a = new MotionNode_Alpha();
                    rec_hm_a.Duration = sp_RecycleArgs.FindPropertyRelative("Alpha.Duration").floatValue;
                    rec_hm_a.Curve = sp_RecycleArgs.FindPropertyRelative("Alpha.Curve").animationCurveValue;
                    rec_hm_a.CurveName = sp_RecycleArgs.FindPropertyRelative("Alpha.CurveName").stringValue;
                    rec_hm_a.Ease = (EaseMode)sp_RecycleArgs.FindPropertyRelative("Alpha.Ease").enumValueIndex;
                    rec_hm_a.Delay = sp_RecycleArgs.FindPropertyRelative("Alpha.Delay").floatValue;

                    rec.Movement = rec_hm_m;
                    rec.Rotation = rec_hm_r;
                    rec.Alpha = rec_hm_a;
                    hpp.RecycleArgs = rec;
                    #endregion


                    hpp.DelayWithIn = sp_DelayWithIn.floatValue;
                    hpp.DelayWithOut = sp_DelayWithOut.floatValue;

                    hpp.key_create = (KeyCode)sp_key_Element_In.enumValueIndex;
                    hpp.key_recycle = (KeyCode)sp_key_Element_Out.enumValueIndex;

                    hpp.HideWithStart = sp_HideWithStart.boolValue;


                    string json = JsonUtility.ToJson(hpp);
                    GUIUtility.systemCopyBuffer = json;
                });
                menu.AddItem(new GUIContent("V (粘贴脚本参数)"), false, () =>
                {
                    XHud_Element_PreviewArgs hpp = JsonUtility.FromJson<XHud_Element_PreviewArgs>(GUIUtility.systemCopyBuffer);

                    sp_DelayWithIn.floatValue = hpp.DelayWithIn;
                    sp_DelayWithOut.floatValue = hpp.DelayWithOut;

                    sp_key_Element_In.enumValueIndex = (int)hpp.key_create;
                    sp_key_Element_Out.enumValueIndex = (int)hpp.key_recycle;

                    sp_HideWithStart.boolValue = hpp.HideWithStart;

                    #region CreateArgs
                    sp_CreateArgs.FindPropertyRelative("anchor").enumValueIndex = (int)hpp.CreateArgs.anchor;

                    sp_CreateArgs.FindPropertyRelative("Movement.Movement").enumValueIndex = (int)hpp.CreateArgs.Movement.Movement;
                    sp_CreateArgs.FindPropertyRelative("Movement.Distance").floatValue = hpp.CreateArgs.Movement.Distance;
                    sp_CreateArgs.FindPropertyRelative("Movement.Duration").floatValue = hpp.CreateArgs.Movement.Duration;
                    sp_CreateArgs.FindPropertyRelative("Movement.Delay").floatValue = hpp.CreateArgs.Movement.Delay;
                    sp_CreateArgs.FindPropertyRelative("Movement.Ease").enumValueIndex = (int)hpp.CreateArgs.Movement.Ease;
                    sp_CreateArgs.FindPropertyRelative("Movement.Curve").animationCurveValue = hpp.CreateArgs.Movement.Curve;
                    sp_CreateArgs.FindPropertyRelative("Movement.CurveName").stringValue = hpp.CreateArgs.Movement.CurveName;

                    sp_CreateArgs.FindPropertyRelative("Rotation.Rotation").enumValueIndex = (int)hpp.CreateArgs.Rotation.Rotation;
                    sp_CreateArgs.FindPropertyRelative("Rotation.Degree").floatValue = hpp.CreateArgs.Rotation.Degree;
                    sp_CreateArgs.FindPropertyRelative("Rotation.Duration").floatValue = hpp.CreateArgs.Rotation.Duration;
                    sp_CreateArgs.FindPropertyRelative("Rotation.Delay").floatValue = hpp.CreateArgs.Rotation.Delay;
                    sp_CreateArgs.FindPropertyRelative("Rotation.Ease").enumValueIndex = (int)hpp.CreateArgs.Rotation.Ease;
                    sp_CreateArgs.FindPropertyRelative("Rotation.Curve").animationCurveValue = hpp.CreateArgs.Rotation.Curve;
                    sp_CreateArgs.FindPropertyRelative("Rotation.CurveName").stringValue = hpp.CreateArgs.Rotation.CurveName;

                    sp_CreateArgs.FindPropertyRelative("Alpha.Duration").floatValue = hpp.CreateArgs.Alpha.Duration;
                    sp_CreateArgs.FindPropertyRelative("Alpha.Curve").animationCurveValue = hpp.CreateArgs.Alpha.Curve;
                    sp_CreateArgs.FindPropertyRelative("Alpha.CurveName").stringValue = hpp.CreateArgs.Alpha.CurveName;
                    sp_CreateArgs.FindPropertyRelative("Alpha.Ease").enumValueIndex = (int)hpp.CreateArgs.Alpha.Ease;
                    sp_CreateArgs.FindPropertyRelative("Alpha.Delay").floatValue = hpp.CreateArgs.Alpha.Delay;
                    #endregion

                    #region RecycleArgs
                    sp_RecycleArgs.FindPropertyRelative("Movement.Movement").enumValueIndex = (int)hpp.RecycleArgs.Movement.Movement;
                    sp_RecycleArgs.FindPropertyRelative("Movement.Distance").floatValue = hpp.RecycleArgs.Movement.Distance;
                    sp_RecycleArgs.FindPropertyRelative("Movement.Duration").floatValue = hpp.RecycleArgs.Movement.Duration;
                    sp_RecycleArgs.FindPropertyRelative("Movement.Delay").floatValue = hpp.RecycleArgs.Movement.Delay;
                    sp_RecycleArgs.FindPropertyRelative("Movement.Ease").enumValueIndex = (int)hpp.RecycleArgs.Movement.Ease;
                    sp_RecycleArgs.FindPropertyRelative("Movement.Curve").animationCurveValue = hpp.RecycleArgs.Movement.Curve;
                    sp_RecycleArgs.FindPropertyRelative("Movement.CurveName").stringValue = hpp.RecycleArgs.Movement.CurveName;

                    sp_RecycleArgs.FindPropertyRelative("Rotation.Rotation").enumValueIndex = (int)hpp.RecycleArgs.Rotation.Rotation;
                    sp_RecycleArgs.FindPropertyRelative("Rotation.Degree").floatValue = hpp.RecycleArgs.Rotation.Degree;
                    sp_RecycleArgs.FindPropertyRelative("Rotation.Duration").floatValue = hpp.RecycleArgs.Rotation.Duration;
                    sp_RecycleArgs.FindPropertyRelative("Rotation.Delay").floatValue = hpp.RecycleArgs.Rotation.Delay;
                    sp_RecycleArgs.FindPropertyRelative("Rotation.Ease").enumValueIndex = (int)hpp.RecycleArgs.Rotation.Ease;
                    sp_RecycleArgs.FindPropertyRelative("Rotation.Curve").animationCurveValue = hpp.RecycleArgs.Rotation.Curve;
                    sp_RecycleArgs.FindPropertyRelative("Rotation.CurveName").stringValue = hpp.RecycleArgs.Rotation.CurveName;

                    sp_RecycleArgs.FindPropertyRelative("Alpha.Duration").floatValue = hpp.RecycleArgs.Alpha.Duration;
                    sp_RecycleArgs.FindPropertyRelative("Alpha.Curve").animationCurveValue = hpp.RecycleArgs.Alpha.Curve;
                    sp_RecycleArgs.FindPropertyRelative("Alpha.CurveName").stringValue = hpp.RecycleArgs.Alpha.CurveName;
                    sp_RecycleArgs.FindPropertyRelative("Alpha.Ease").enumValueIndex = (int)hpp.RecycleArgs.Alpha.Ease;
                    sp_RecycleArgs.FindPropertyRelative("Alpha.Delay").floatValue = hpp.RecycleArgs.Alpha.Delay;
                    #endregion


                    sp_CreateArgs.serializedObject.ApplyModifiedProperties();
                    sp_RecycleArgs.serializedObject.ApplyModifiedProperties();

                    sp_DelayWithIn.serializedObject.ApplyModifiedProperties();
                    sp_DelayWithOut.serializedObject.ApplyModifiedProperties();
                    sp_key_Element_In.serializedObject.ApplyModifiedProperties();
                    sp_key_Element_Out.serializedObject.ApplyModifiedProperties();
                    sp_HideWithStart.serializedObject.ApplyModifiedProperties();
                });
                menu.AddSeparator("");
                menu.AddDisabledItem(new GUIContent("动效快速操作"));
                menu.AddItem(new GUIContent("E (复制动效)"), false, () =>
                {
                    string res = XGUI.dialog(
                        type: XGUIDialogType.警告,
                        windowtitle: "XHud - 元素预览器消息",
                        title: "复制动效",
                        msg: "请选择动效参数复制模式！",
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
                            crc.anchor = (XHudAnchor)sp_CreateArgs.FindPropertyRelative("anchor").enumValueIndex;

                            M = new MotionNode_Movement();
                            M.Movement = (HudMotion_Movement)sp_CreateArgs.FindPropertyRelative("Movement.Movement").enumValueIndex;
                            M.Distance = sp_CreateArgs.FindPropertyRelative("Movement.Distance").floatValue;
                            M.Duration = sp_CreateArgs.FindPropertyRelative("Movement.Duration").floatValue;
                            M.Delay = sp_CreateArgs.FindPropertyRelative("Movement.Delay").floatValue;
                            M.Ease = (EaseMode)sp_CreateArgs.FindPropertyRelative("Movement.Ease").enumValueIndex;
                            M.Curve = sp_CreateArgs.FindPropertyRelative("Movement.Curve").animationCurveValue;
                            M.CurveName = sp_CreateArgs.FindPropertyRelative("Movement.CurveName").stringValue;

                            R = new MotionNode_Rotation();
                            R.Rotation = (HudMotion_Rotation)sp_CreateArgs.FindPropertyRelative("Rotation.Rotation").enumValueIndex;
                            R.Degree = sp_CreateArgs.FindPropertyRelative("Rotation.Degree").floatValue;
                            R.Duration = sp_CreateArgs.FindPropertyRelative("Rotation.Duration").floatValue;
                            R.Delay = sp_CreateArgs.FindPropertyRelative("Rotation.Delay").floatValue;
                            R.Ease = (EaseMode)sp_CreateArgs.FindPropertyRelative("Rotation.Ease").enumValueIndex;
                            R.Curve = sp_CreateArgs.FindPropertyRelative("Rotation.Curve").animationCurveValue;
                            R.CurveName = sp_CreateArgs.FindPropertyRelative("Rotation.CurveName").stringValue;

                            A = new MotionNode_Alpha();
                            A.Duration = sp_CreateArgs.FindPropertyRelative("Alpha.Duration").floatValue;
                            A.Curve = sp_CreateArgs.FindPropertyRelative("Alpha.Curve").animationCurveValue;
                            A.CurveName = sp_CreateArgs.FindPropertyRelative("Alpha.CurveName").stringValue;
                            A.Ease = (EaseMode)sp_CreateArgs.FindPropertyRelative("Alpha.Ease").enumValueIndex;
                            A.Delay = sp_CreateArgs.FindPropertyRelative("Alpha.Delay").floatValue;

                            crc.Movement = M;
                            crc.Rotation = R;
                            crc.Alpha = A;

                            json = JsonUtility.ToJson(crc);
                            XGUI.x_Editor_Data_Set_With_String("xData_MotionArgs", json);
                            break;
                        case "回收"://回收
                            Motion_Recycler rec = new Motion_Recycler();

                            M = new MotionNode_Movement();
                            M.Movement = (HudMotion_Movement)sp_RecycleArgs.FindPropertyRelative("Movement.Movement").enumValueIndex;
                            M.Distance = sp_RecycleArgs.FindPropertyRelative("Movement.Distance").floatValue;
                            M.Duration = sp_RecycleArgs.FindPropertyRelative("Movement.Duration").floatValue;
                            M.Delay = sp_RecycleArgs.FindPropertyRelative("Movement.Delay").floatValue;
                            M.Ease = (EaseMode)sp_RecycleArgs.FindPropertyRelative("Movement.Ease").enumValueIndex;
                            M.Curve = sp_RecycleArgs.FindPropertyRelative("Movement.Curve").animationCurveValue;
                            M.CurveName = sp_RecycleArgs.FindPropertyRelative("Movement.CurveName").stringValue;

                            R = new MotionNode_Rotation();
                            R.Rotation = (HudMotion_Rotation)sp_RecycleArgs.FindPropertyRelative("Rotation.Rotation").enumValueIndex;
                            R.Degree = sp_RecycleArgs.FindPropertyRelative("Rotation.Degree").floatValue;
                            R.Duration = sp_RecycleArgs.FindPropertyRelative("Rotation.Duration").floatValue;
                            R.Delay = sp_RecycleArgs.FindPropertyRelative("Rotation.Delay").floatValue;
                            R.Ease = (EaseMode)sp_RecycleArgs.FindPropertyRelative("Rotation.Ease").enumValueIndex;
                            R.Curve = sp_RecycleArgs.FindPropertyRelative("Rotation.Curve").animationCurveValue;
                            R.CurveName = sp_RecycleArgs.FindPropertyRelative("Rotation.CurveName").stringValue;

                            A = new MotionNode_Alpha();
                            A.Duration = sp_RecycleArgs.FindPropertyRelative("Alpha.Duration").floatValue;
                            A.Curve = sp_RecycleArgs.FindPropertyRelative("Alpha.Curve").animationCurveValue;
                            A.CurveName = sp_RecycleArgs.FindPropertyRelative("Alpha.CurveName").stringValue;
                            A.Ease = (EaseMode)sp_RecycleArgs.FindPropertyRelative("Alpha.Ease").enumValueIndex;
                            A.Delay = sp_RecycleArgs.FindPropertyRelative("Alpha.Delay").floatValue;

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
                        type: XGUIDialogType.警告,
                        windowtitle: "XHud - 元素预览器消息",
                        title: "复制动效",
                        msg: $"已复制 \" {mode} \" 到 xData ！",
                        ok: "明白",
                        PrimaryIndex: 0,
                        usemodal: true,
                        themecolor: XHud_Dashboard.Theme_Primary);
                });
                menu.AddItem(new GUIContent("R (粘贴动效)"), false, () =>
                {
                    string buffer = XGUI.x_Editor_Data_Get_With_String("xData_MotionArgs");
                    if (buffer.Contains("anchor"))//粘贴生成参数
                    {
                        string res = XGUI.dialog(
                            type: XGUIDialogType.警告,
                            windowtitle: "XHud - 元素预览器消息",
                            title: "粘贴动效",
                            msg: $"检测到动效参数类型为： \"生成动效\"，确定要使用这个参数吗？",
                            ok: "确定",
                            cancel: "暂不",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.Theme_Primary);

                        if (res == "暂不")
                            return;

                        Motion_Creator crc = JsonUtility.FromJson<Motion_Creator>(buffer);

                        sp_CreateArgs.FindPropertyRelative("anchor").enumValueIndex = (int)crc.anchor;

                        sp_CreateArgs.FindPropertyRelative("Movement.Movement").enumValueIndex = (int)crc.Movement.Movement;
                        sp_CreateArgs.FindPropertyRelative("Movement.Distance").floatValue = crc.Movement.Distance;
                        sp_CreateArgs.FindPropertyRelative("Movement.Duration").floatValue = crc.Movement.Duration;
                        sp_CreateArgs.FindPropertyRelative("Movement.Delay").floatValue = crc.Movement.Delay;
                        sp_CreateArgs.FindPropertyRelative("Movement.Ease").enumValueIndex = (int)crc.Movement.Ease;
                        sp_CreateArgs.FindPropertyRelative("Movement.Curve").animationCurveValue = crc.Movement.Curve;
                        sp_CreateArgs.FindPropertyRelative("Movement.CurveName").stringValue = crc.Movement.CurveName;

                        sp_CreateArgs.FindPropertyRelative("Rotation.Rotation").enumValueIndex = (int)crc.Rotation.Rotation;
                        sp_CreateArgs.FindPropertyRelative("Rotation.Degree").floatValue = crc.Rotation.Degree;
                        sp_CreateArgs.FindPropertyRelative("Rotation.Duration").floatValue = crc.Rotation.Duration;
                        sp_CreateArgs.FindPropertyRelative("Rotation.Delay").floatValue = crc.Rotation.Delay;
                        sp_CreateArgs.FindPropertyRelative("Rotation.Ease").enumValueIndex = (int)crc.Rotation.Ease;
                        sp_CreateArgs.FindPropertyRelative("Rotation.Curve").animationCurveValue = crc.Rotation.Curve;
                        sp_CreateArgs.FindPropertyRelative("Rotation.CurveName").stringValue = crc.Rotation.CurveName;

                        sp_CreateArgs.FindPropertyRelative("Alpha.Duration").floatValue = crc.Alpha.Duration;
                        sp_CreateArgs.FindPropertyRelative("Alpha.Curve").animationCurveValue = crc.Alpha.Curve;
                        sp_CreateArgs.FindPropertyRelative("Alpha.CurveName").stringValue = crc.Alpha.CurveName;
                        sp_CreateArgs.FindPropertyRelative("Alpha.Ease").enumValueIndex = (int)crc.Alpha.Ease;
                        sp_CreateArgs.FindPropertyRelative("Alpha.Delay").floatValue = crc.Alpha.Delay;

                        sp_CreateArgs.serializedObject.ApplyModifiedProperties();

                        XGUI.dialog(
                            type: XGUIDialogType.警告,
                            windowtitle: "XHud - 元素预览器消息",
                            title: "粘贴动效",
                            msg: $"已更新 \"生成\" 动效参数！",
                            ok: "明白",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.Theme_Primary);
                    }
                    else//粘贴回收参数
                    {
                        string res = XGUI.dialog(
                          type: XGUIDialogType.警告,
                          windowtitle: "XHud - 元素预览器消息",
                          title: "粘贴动效",
                          msg: $"检测到动效参数类型为： \"回收动效\"，确定要使用这个参数吗？",
                          ok: "确定",
                          cancel: "暂不",
                          PrimaryIndex: 0,
                          usemodal: true,
                          themecolor: XHud_Dashboard.Theme_Primary);

                        if (res == "暂不")
                            return;

                        Motion_Recycler rec = JsonUtility.FromJson<Motion_Recycler>(buffer);

                        sp_RecycleArgs.FindPropertyRelative("Movement.Movement").enumValueIndex = (int)rec.Movement.Movement;
                        sp_RecycleArgs.FindPropertyRelative("Movement.Distance").floatValue = rec.Movement.Distance;
                        sp_RecycleArgs.FindPropertyRelative("Movement.Duration").floatValue = rec.Movement.Duration;
                        sp_RecycleArgs.FindPropertyRelative("Movement.Delay").floatValue = rec.Movement.Delay;
                        sp_RecycleArgs.FindPropertyRelative("Movement.Ease").enumValueIndex = (int)rec.Movement.Ease;
                        sp_RecycleArgs.FindPropertyRelative("Movement.Curve").animationCurveValue = rec.Movement.Curve;
                        sp_RecycleArgs.FindPropertyRelative("Movement.CurveName").stringValue = rec.Movement.CurveName;

                        sp_RecycleArgs.FindPropertyRelative("Rotation.Rotation").enumValueIndex = (int)rec.Rotation.Rotation;
                        sp_RecycleArgs.FindPropertyRelative("Rotation.Degree").floatValue = rec.Rotation.Degree;
                        sp_RecycleArgs.FindPropertyRelative("Rotation.Duration").floatValue = rec.Rotation.Duration;
                        sp_RecycleArgs.FindPropertyRelative("Rotation.Delay").floatValue = rec.Rotation.Delay;
                        sp_RecycleArgs.FindPropertyRelative("Rotation.Ease").enumValueIndex = (int)rec.Rotation.Ease;
                        sp_RecycleArgs.FindPropertyRelative("Rotation.Curve").animationCurveValue = rec.Rotation.Curve;
                        sp_RecycleArgs.FindPropertyRelative("Rotation.CurveName").stringValue = rec.Rotation.CurveName;

                        sp_RecycleArgs.FindPropertyRelative("Alpha.Duration").floatValue = rec.Alpha.Duration;
                        sp_RecycleArgs.FindPropertyRelative("Alpha.Curve").animationCurveValue = rec.Alpha.Curve;
                        sp_RecycleArgs.FindPropertyRelative("Alpha.CurveName").stringValue = rec.Alpha.CurveName;
                        sp_RecycleArgs.FindPropertyRelative("Alpha.Ease").enumValueIndex = (int)rec.Alpha.Ease;
                        sp_RecycleArgs.FindPropertyRelative("Alpha.Delay").floatValue = rec.Alpha.Delay;

                        sp_RecycleArgs.serializedObject.ApplyModifiedProperties();

                        XGUI.dialog(
                            type: XGUIDialogType.警告,
                            windowtitle: "XHud - 元素预览器消息",
                            title: "粘贴动效",
                            msg: $"已更新 \"回收\" 动效参数！",
                            ok: "明白",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.Theme_Primary);
                    }
                });
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("F (折叠动效参数)"), false, () =>
                {
                    recycle_fold_move.boolValue = false;
                    recycle_fold_rotate.boolValue = false;
                    recycle_fold_alpha.boolValue = false;
                    create_fold_move.boolValue = false;
                    create_fold_rotate.boolValue = false;
                    create_fold_alpha.boolValue = false;

                    recycle_fold_move.serializedObject.ApplyModifiedProperties();
                    recycle_fold_rotate.serializedObject.ApplyModifiedProperties();
                    recycle_fold_alpha.serializedObject.ApplyModifiedProperties();
                    create_fold_move.serializedObject.ApplyModifiedProperties();
                    create_fold_rotate.serializedObject.ApplyModifiedProperties();
                    create_fold_alpha.serializedObject.ApplyModifiedProperties();
                });
                menu.AddItem(new GUIContent("D (展开动效参数)"), false, () =>
                {
                    recycle_fold_move.boolValue = true;
                    recycle_fold_rotate.boolValue = true;
                    recycle_fold_alpha.boolValue = true;
                    create_fold_move.boolValue = true;
                    create_fold_rotate.boolValue = true;
                    create_fold_alpha.boolValue = true;

                    recycle_fold_move.serializedObject.ApplyModifiedProperties();
                    recycle_fold_rotate.serializedObject.ApplyModifiedProperties();
                    recycle_fold_alpha.serializedObject.ApplyModifiedProperties();
                    create_fold_move.serializedObject.ApplyModifiedProperties();
                    create_fold_rotate.serializedObject.ApplyModifiedProperties();
                    create_fold_alpha.serializedObject.ApplyModifiedProperties();
                });
                menu.ShowAsContext(); // 在鼠标位置显示右键菜单
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

        #region 辅助
        /// <summary>
        /// 重置生成与回收的参数到默认
        /// </summary>
        private void ResetMotionParams(string state)
        {
            if (state == "c")
            {
                CreateArgs.FindPropertyRelative("anchor").enumValueIndex = (int)XHudAnchor.中心;
                CreateArgs.FindPropertyRelative("Movement.Movement").enumValueIndex = (int)HudMotion_Movement.S_从下至上;
                CreateArgs.FindPropertyRelative("Movement.Distance").floatValue = 100;
                CreateArgs.FindPropertyRelative("Movement.Duration").floatValue = 1;
                CreateArgs.FindPropertyRelative("Movement.Delay").floatValue = 0;
                CreateArgs.FindPropertyRelative("Movement.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
                CreateArgs.FindPropertyRelative("Movement.CurveName").stringValue = "";
                CreateArgs.FindPropertyRelative("Movement.Ease").enumValueIndex = (int)EaseMode.OutQuart;
                CreateArgs.FindPropertyRelative("Rotation.Rotation").enumValueIndex = (int)HudMotion_Rotation.A_无旋转;
                CreateArgs.FindPropertyRelative("Rotation.Degree").floatValue = 0;
                CreateArgs.FindPropertyRelative("Rotation.Duration").floatValue = 1;
                CreateArgs.FindPropertyRelative("Rotation.Delay").floatValue = 0;
                CreateArgs.FindPropertyRelative("Rotation.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
                CreateArgs.FindPropertyRelative("Rotation.CurveName").stringValue = "";
                CreateArgs.FindPropertyRelative("Rotation.Ease").enumValueIndex = (int)EaseMode.OutQuart;
                CreateArgs.FindPropertyRelative("Alpha.Duration").floatValue = 1;
                CreateArgs.FindPropertyRelative("Alpha.Delay").floatValue = 0;
                CreateArgs.FindPropertyRelative("Alpha.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
                CreateArgs.FindPropertyRelative("Alpha.CurveName").stringValue = "";
                CreateArgs.FindPropertyRelative("Alpha.Ease").enumValueIndex = (int)EaseMode.OutQuart;
                CreateArgs_MotionAnimateEndState.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
                CreateArgs.serializedObject.ApplyModifiedProperties();
            }
            else if (state == "r")
            {
                RecycleArgs.FindPropertyRelative("Movement.Movement").enumValueIndex = (int)HudMotion_Movement.D_从上至下;
                RecycleArgs.FindPropertyRelative("Movement.Distance").floatValue = 100;
                RecycleArgs.FindPropertyRelative("Movement.Duration").floatValue = 1;
                RecycleArgs.FindPropertyRelative("Movement.Delay").floatValue = 0;
                RecycleArgs.FindPropertyRelative("Movement.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
                RecycleArgs.FindPropertyRelative("Movement.CurveName").stringValue = "";
                RecycleArgs.FindPropertyRelative("Movement.Ease").enumValueIndex = (int)EaseMode.OutQuart;
                RecycleArgs.FindPropertyRelative("Rotation.Rotation").enumValueIndex = (int)HudMotion_Rotation.A_无旋转;
                RecycleArgs.FindPropertyRelative("Rotation.Degree").floatValue = 0;
                RecycleArgs.FindPropertyRelative("Rotation.Duration").floatValue = 1;
                RecycleArgs.FindPropertyRelative("Rotation.Delay").floatValue = 0;
                RecycleArgs.FindPropertyRelative("Rotation.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
                RecycleArgs.FindPropertyRelative("Rotation.CurveName").stringValue = "";
                RecycleArgs.FindPropertyRelative("Rotation.Ease").enumValueIndex = (int)EaseMode.OutQuart;
                RecycleArgs.FindPropertyRelative("Alpha.Duration").floatValue = 1;
                RecycleArgs.FindPropertyRelative("Alpha.Delay").floatValue = 0;
                RecycleArgs.FindPropertyRelative("Alpha.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
                RecycleArgs.FindPropertyRelative("Alpha.CurveName").stringValue = "";
                RecycleArgs.FindPropertyRelative("Alpha.Ease").enumValueIndex = (int)EaseMode.OutQuart;
                RecycleArgs_MotionAnimateEndState.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
                RecycleArgs.serializedObject.ApplyModifiedProperties();
            }
        }

        private void CheckRmsNameValid()
        {
            if (!HudManager.RMS_Enabled)
                return;
            if (!RMS_Enabled.boolValue)
                return;
            if (string.IsNullOrEmpty(RMS_Name.stringValue))
            {
                string[] nodes = HudManager.hm_RMS_GetResolutionNodeNames();
                if (nodes.Length <= 0)
                    return;
                RMS_Name.stringValue = nodes[0];
                RMS_Name.serializedObject.ApplyModifiedProperties();
            }
        }

        /// <summary>
        /// 动效库添加器
        /// </summary>
        public void OpenParameterSetter(HudElementMotionType Type)
        {
            Editor_XHud_LibrarySetTool_Motion window = EditorWindow.GetWindow<Editor_XHud_LibrarySetTool_Motion>(true);

            window.titleContent = new GUIContent("XHud - 元素动效资源采集器");
            XGUI.CenterEditorWindow(new Vector2Int(348, Type == HudElementMotionType.Creator ? 850 : 780), window);

            switch (Type)
            {
                case HudElementMotionType.Recycler:
                    Motion_Recycler rec = new Motion_Recycler();
                    rec.MotionAnimateEndState = BaseScript.RecycleArgs.MotionAnimateEndState;
                    rec.Alpha = BaseScript.RecycleArgs.Alpha;

                    rec.Movement = new MotionNode_Movement();
                    rec.Movement.CopyData(BaseScript.RecycleArgs.Movement);

                    rec.Rotation = new MotionNode_Rotation();
                    rec.Rotation.CopyData(BaseScript.RecycleArgs.Rotation);

                    rec.Alpha = new MotionNode_Alpha();
                    rec.Alpha.CopyData(BaseScript.RecycleArgs.Alpha);

                    window.SetElementMotion(rec);
                    break;
                case HudElementMotionType.Creator:
                    Motion_Creator crc = new Motion_Creator();
                    crc.MotionAnimateEndState = BaseScript.CreateArgs.MotionAnimateEndState;
                    crc.anchor = BaseScript.CreateArgs.anchor;

                    crc.Alpha = BaseScript.CreateArgs.Alpha;

                    crc.Movement = new MotionNode_Movement();
                    crc.Movement.CopyData(BaseScript.CreateArgs.Movement);

                    crc.Rotation = new MotionNode_Rotation();
                    crc.Rotation.CopyData(BaseScript.CreateArgs.Rotation);

                    crc.Alpha = new MotionNode_Alpha();
                    crc.Alpha.CopyData(BaseScript.CreateArgs.Alpha);

                    window.SetElementMotion(crc);
                    break;
            }

            window.SetElementMotionType(Type);
            window.SetLibrarySetterMode(LibrarySetterMode.添加到库);
            window.SetButtonText("添加", "取消");
            window.SetTitle("元素动效资源采集器");
            window.SetTarget_Hud_MotionLibrary(HudManager.Hud_Motions);
            //window.ShowModal();
            window.Show();
        }
        #endregion

        #region Draw
        /// <summary>
        /// 通用方法：绘制开关
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
                tog_mixed_bg_color_gui: XHud_Dashboard.Theme_Primary,
                act_on_changed: act_on_changed);
        }
        /// <summary>
        /// 通用方法：绘制参数
        /// </summary>
        private void DrawParamField(string title, SerializedProperty prop, float width, Color status_color)
        {
            XGUI.layout_property_field(
                title: title,
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.Theme_Primary,
                title_width: width,
                //status_icon: status_color == Color.clear ? null : "icon_field_status",
                //status_icon_color: status_color,
                prop: prop,
                prop_margin: new RectOffset(0, 0, 5, 10));
        }
        #endregion
    }
}