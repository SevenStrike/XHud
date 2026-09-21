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
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Rendering.Universal;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(XHud_CameraCapture))]
    public class Editor_XHud_CameraCapture : Editor
    {
        #region 组件 / 列表
        private XHud_CameraCapture BaseScript;
        #endregion

        #region 序列化属性
        private SerializedProperty
            TargetCamera,
            x_sizemode,
            x_mode,
            x_type,
            x_bgtype,
            Pixels,
            x_path,
            x_name,
            x_UICamera_bgcolor,
            x_bg_alpha;
        #endregion

        #region 图标
        private Texture2D
            icon_main,
            icon_bg_alpha_full,
            icon_bg_alpha_half,
            icon_bg_alpha_zero;
        #endregion

        #region 选项文字
        string[] str_x_mode = new string[2] { "场景相机", "UI相机" };
        string[] str_x_type = new string[3] { "JPG", "PNG", "TGA" };
        string[] str_x_size = new string[3] { "相机尺寸", "屏幕分辨率", "固定尺寸" };
        string[] str_x_bgtype = new string[3] { "天空盒", "颜色", "透明" };
        #endregion

        #region 批量化操作
        private XHud_CameraCapture[] SelectedObjects;

        private void GetAllTargets()
        {
            if (targets.Length > 1)
            {
                SelectedObjects = new XHud_CameraCapture[targets.Length];
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    var t = targets[i];
                    SelectedObjects[i] = (XHud_CameraCapture)t;
                }
            }
            else
            {
                SelectedObjects = new XHud_CameraCapture[targets.Length];
                SelectedObjects[0] = (XHud_CameraCapture)target;
            }
        }

        private bool IsMultiSelected()
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
            BaseScript = (XHud_CameraCapture)target;

            GetAllTargets();

            #region 获取变量
            TargetCamera = serializedObject.FindProperty("TargetCamera");
            x_sizemode = serializedObject.FindProperty("x_sizemode");
            x_mode = serializedObject.FindProperty("x_mode");
            x_type = serializedObject.FindProperty("x_type");
            Pixels = serializedObject.FindProperty("Pixels");
            x_path = serializedObject.FindProperty("x_path");
            x_bgtype = serializedObject.FindProperty("x_bgtype");
            x_name = serializedObject.FindProperty("x_name");
            x_UICamera_bgcolor = serializedObject.FindProperty("x_UICamera_bgcolor");
            x_bg_alpha = serializedObject.FindProperty("x_bg_alpha");
            #endregion

            #region 获取图标
            icon_main = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_camera_capture/icon_main");
            icon_bg_alpha_full = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_camera_capture/icon_bg_alpha_full");
            icon_bg_alpha_half = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_camera_capture/icon_bg_alpha_half");
            icon_bg_alpha_zero = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_camera_capture/icon_bg_alpha_zero");
            #endregion
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            #region 标题
            XGUI.layout_banner(
              bg_fill: XGUIFilled.实体,
              bg_color: XGUIColor.深空灰,
              bg_height: 30,
              icon: icon_main,
              icon_color: XHud_Dashboard.Theme_Primary,
              title_text: "XHud  -  相机截图器",
              title_anchor: TextAnchor.MiddleLeft,
              title_style: FontStyle.Normal,
              title_color: Color.white,
              title_size: XGUIFontSize.B,
              title_clipping: TextClipping.Ellipsis,
              bg_margin: new RectOffset(0, 0, 5, 5));
            #endregion

            XGUI.layout_space(5);

            #region 相机指定
            BaseScript.fold_camera = XGUI.layout_group_start(
                 type: XGUIContainerType.Vertical,
                 bg_fill: XGUIFilled.缺口纯色边框,
                 bg_color: XGUIColor.亮白,
                 bg_color_gui: XHud_Dashboard.Theme_Group,
                 title: "相机",
                 title_size: XGUIFontSize.M,
                 title_text_color: XHud_Dashboard.Theme_Primary,
                 title_clipping: TextClipping.Clip,
                 padding: new RectOffset(10, 10, 15, 15),
                 foldout: BaseScript.fold_camera);

            if (!BaseScript.fold_camera)
            {
                XGUI.layout_property_field(
                    title: "截图相机",
                    title_size: XGUIFontSize.M,
                    //title_color: Color.white,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 100,
                    //status_icon: "icon_field_status",
                    //status_icon_color: CameraCutter_Near.floatValue != 0, XHud_Dashboard.Theme_Primary : Color.red,
                    prop: TargetCamera,
                    prop_margin: new RectOffset(0, 0, 5, 0));

                XGUI.layout_space(10);

                if (x_mode.enumValueIndex == (int)CaptureCameraType.场景相机)
                    XGUI.layout_helpbox(
                        state: XGUIHelboxState.通知,
                        title_text: "注意！如果您开启了PostProcessing模式则场景背景则会丢失透明通道！",
                        title_size: XGUIFontSize.M,
                        title_style: FontStyle.Normal,
                        wrap: true,
                        title_clipping: TextClipping.Clip,
                        title_color: Color.white * 0.75f);
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
                XGUI.layout_int_popup(
                    title: "相机类型",
                    title_width: 60,
                    title_size: XGUIFontSize.M,
                    title_anchor: TextAnchor.MiddleLeft,
                    prop: x_mode,
                    options: str_x_mode,
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
                        x_mode.enumValueIndex = value;
                        x_mode.serializedObject.ApplyModifiedProperties();
                    });

                XGUI.layout_int_popup(
                    title: "截图格式",
                    title_width: 60,
                    title_size: XGUIFontSize.M,
                    title_anchor: TextAnchor.MiddleLeft,
                    prop: x_type,
                    options: str_x_type,
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
                        x_type.enumValueIndex = value;
                        x_type.serializedObject.ApplyModifiedProperties();
                    });

                XGUI.layout_int_popup(
                    title: "截图背景",
                    title_width: 60,
                    title_size: XGUIFontSize.M,
                    title_anchor: TextAnchor.MiddleLeft,
                    prop: x_bgtype,
                    options: str_x_bgtype,
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
                        x_bgtype.enumValueIndex = value;
                        x_bgtype.serializedObject.ApplyModifiedProperties();
                    });

                XGUI.layout_int_popup(
                    title: "截图尺寸",
                    title_width: 60,
                    title_size: XGUIFontSize.M,
                    title_anchor: TextAnchor.MiddleLeft,
                    prop: x_sizemode,
                    options: str_x_size,
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
                        x_sizemode.enumValueIndex = value;
                        x_sizemode.serializedObject.ApplyModifiedProperties();
                    });
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
             padding: new RectOffset(10, 10, 15, 15),
             foldout: BaseScript.fold_param);

            if (!BaseScript.fold_param)
            {
                #region UI截图背景颜色
                float scale = 0.5f;
                XGUI.layout_icon(
                    icon: x_bg_alpha.intValue == 2 ? icon_bg_alpha_full : (x_bg_alpha.intValue == 1 ? icon_bg_alpha_half : icon_bg_alpha_zero),
                    width: icon_bg_alpha_full.width * scale,
                    height: icon_bg_alpha_full.height * scale,
                    icon_alignment: XGUIIconAlignment.中心,
                    layout_margin: new RectOffset(0, 0, 20, 15));

                XGUI.layout_space(10);

                x_bg_alpha.intValue = XGUI.layout_toolbar(
                        index: x_bg_alpha.intValue,
                        names: new string[] { "全透明", "半透明", "不透明" },
                        bg_normal: XGUIFilled.无,
                        bg_selected: XGUIFilled.实体,
                        bg_color: XGUIColor.亮白,
                        bg_gui_color: Color.black * 0.5f,
                        text_color_normal: Color.white,
                        text_color_selected: XHud_Dashboard.Theme_Primary,
                        bar_height: 25,
                        bar_offset: new Vector2(0, 15),
                        text_anchor: TextAnchor.MiddleCenter,
                        text_padding: new RectOffset(10, 10, 0, 0),
                        bar_margin: new RectOffset(0, 0, 5, 15),
                        text_offset: new Vector2(0, -2),
                        text_size: XGUIFontSize.M,
                        text_font: XGUI.GetFont("xg-medium"),
                        text_fontstyle: FontStyle.Bold,
                        navigate_style: true,
                        navigate_style_bg: XGUIFilled.纯色边框,
                        navigate_style_bg_color: Color.black * 0.5f);

                x_bg_alpha.serializedObject.ApplyModifiedProperties();

                Color colorValue = x_UICamera_bgcolor.colorValue;

                switch (BaseScript.x_bg_alpha)
                {
                    case 0:
                        colorValue.a = 0;
                        x_UICamera_bgcolor.colorValue = colorValue;
                        break;
                    case 1:
                        colorValue.a = 0.5f;
                        x_UICamera_bgcolor.colorValue = colorValue;
                        break;
                    case 2:
                        colorValue.a = 1;
                        x_UICamera_bgcolor.colorValue = colorValue;
                        break;
                }
                #endregion

                #region 固定尺寸
                if (x_sizemode.intValue == 2)
                {
                    XGUI.layout_space(5);

                    XGUI.layout_property_field(
                        title: "固定尺寸",
                        title_size: XGUIFontSize.M,
                        //title_color: Color.white,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 100,
                        //status_icon: "icon_field_status",
                        //status_icon_color: CameraCutter_Near.floatValue != 0, XHud_Dashboard.Theme_Primary : Color.red,
                        prop: Pixels,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                }
                #endregion

                #region 截图背景色
                XGUI.layout_property_field(
                     title: "截图背景色",
                     title_size: XGUIFontSize.M,
                     //title_color: Color.white,
                     title_hover_color: XHud_Dashboard.Theme_Primary,
                     title_width: 100,
                     //status_icon: "icon_field_status",
                     //status_icon_color: CameraCutter_Near.floatValue != 0, XHud_Dashboard.Theme_Primary : Color.red,
                     prop: x_UICamera_bgcolor,
                     prop_margin: new RectOffset(0, 0, 5, 10));
                #endregion

                #region 截图路径
                XGUI.layout_path_selector(
                    path_type: XGUIPathType.文件夹,
                    prop: ref BaseScript.x_path,
                    title: "截图路径",
                    //title_color: Color.white,
                    title_width: 162,
                    text_wrap: false,
                    field_fontsize: XGUIFontSize.M,
                    field_text_offset: new Vector2(0, 0),
                    field_margin: new RectOffset(0, 0, 0, 0),
                    field_padding: new RectOffset(0, 0, 0, 0),
                    field_text_color: Color.white,
                    field_text_style: FontStyle.Normal,
                    field_text_anchor: TextAnchor.MiddleLeft,
                    filter: null,
                    filter_title: "选择文件夹",
                    btn_color: XHud_Dashboard.Theme_Primary,
                    btn_fontsize: XGUIFontSize.M,
                    default_name: "default",
                    status_icon: "icon_field_status",
                    status_icon_color: XGUI_Utilitys.PathExists(BaseScript.x_path) ? Color.green : Color.red,
                    on_path_changed: (val) =>
                    {
                        BaseScript.x_path = val;
                    });
                #endregion

                #region 文件名
                XGUI.layout_property_field(
                     title: "文件名",
                     title_size: XGUIFontSize.M,
                     //title_color: Color.white,
                     title_hover_color: XHud_Dashboard.Theme_Primary,
                     title_width: 100,
                     //status_icon: "icon_field_status",
                     //status_icon_color: CameraCutter_Near.floatValue != 0, XHud_Dashboard.Theme_Primary : Color.red,
                     prop: x_name,
                     prop_margin: new RectOffset(0, 0, 5, 0));
                #endregion

                #region 立即截图
                XGUI.layout_space(5);
                if (XGUI.layout_button(
                    text: "立即截图",
                    tooltip: "",
                    bg_fill: XGUIFilled.实体,
                    bg_color: XGUIColor.深空灰,
                    bg_color_gui: Color.white,
                    button_text_color: Color.white,
                    press_fill: XGUIFilled.实体,
                    press_color: XGUIColor.深空灰,
                    press_text_color: Color.gray,
                    font_size: XGUIFontSize.M,
                    anchor: TextAnchor.MiddleCenter,
                    margin: new RectOffset(0, 0, 0, 0),
                    padding: new RectOffset(0, 0, 0, 0),
                    height: 30,
                    button_text_font: XGUI.GetFont("xg-medium")))
                {
                    if (TargetCamera.objectReferenceValue == null)
                    {
                        XGUI.dialog(
                            type: XGUIDialogType.警告,
                            windowtitle: "XHud - 相机截图器消息",
                            title: "相机缺失",
                            msg: "您是否忘了指定目标相机了？",
                            ok: "明白",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.Theme_Primary);
                        return;
                    }
                    bool validstate = true;
                    Camera cam = (Camera)TargetCamera.objectReferenceValue;

                    UniversalAdditionalCameraData universal = cam.GetComponent<UniversalAdditionalCameraData>();
                    if (universal.renderType == CameraRenderType.Base && x_mode.enumValueIndex == (int)CaptureCameraType.UI相机)
                    {
                        validstate = false;
                        string res = XGUI.dialog(
                            type: XGUIDialogType.警告,
                            windowtitle: "XHud - 相机截图器消息",
                            title: "类型匹配错误",
                            msg: "目标相机和您选择的要进行截图的相机类型选项不匹配！目标相机模式为 \"Base\" 基础态，但是您选择的 \"相机类型\" 是 \"UI相机\"！",
                            ok: "快速修正",
                            cancel: "暂不",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.Theme_Primary,
                            on_selected: (d) =>
                            {
                                if (d == "快速修正")
                                {
                                    x_mode.enumValueIndex = (int)CaptureCameraType.场景相机;
                                    x_mode.serializedObject.ApplyModifiedProperties();
                                }
                                else
                                {
                                    return;
                                }
                            });
                    }
                    if (universal.renderType == CameraRenderType.Overlay && x_mode.enumValueIndex == (int)CaptureCameraType.场景相机)
                    {
                        validstate = false;

                        string res = XGUI.dialog(
                           type: XGUIDialogType.警告,
                           windowtitle: "XHud - 相机截图器消息",
                           title: "类型匹配错误",
                           msg: "目标相机和您选择的要进行截图的相机类型选项不匹配！目标相机模式为 \"Overlay\" 叠加态，但是您选择的 \"相机类型\" 是 \"场景相机\"！",
                           ok: "快速修正",
                           cancel: "暂不",
                           PrimaryIndex: 0,
                           usemodal: true,
                           themecolor: XHud_Dashboard.Theme_Primary,
                           on_selected: (d) =>
                           {
                               if (d == "快速修正")
                               {
                                   x_mode.enumValueIndex = (int)CaptureCameraType.UI相机;
                                   x_mode.serializedObject.ApplyModifiedProperties();
                               }
                               else
                               {
                                   return;
                               }
                           });
                    }

                    if (validstate)
                    {
                        BaseScript.CaptureNow();

                        XGUI.dialog(
                            type: XGUIDialogType.警告,
                            windowtitle: "XHud - 相机截图器消息",
                            title: "截图捕捉完成",
                            msg: $"已将 {str_x_mode[x_mode.intValue]} 的画面数据按照 {str_x_size[x_sizemode.intValue]} 模式保存格式为 {str_x_type[x_type.intValue]} 图片到路径： {x_path.stringValue}",
                            ok: "明白",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.Theme_Primary);
                    }
                }
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
                DrawDefaultInspector();

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            serializedObject.ApplyModifiedProperties();
        }
    }
}