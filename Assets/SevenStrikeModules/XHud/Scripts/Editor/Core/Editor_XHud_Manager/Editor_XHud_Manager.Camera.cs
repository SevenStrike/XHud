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

    public partial class Editor_XHud_Manager : Editor
    {
        #region 绘制 - 相机
        /// <summary>
        /// 绘制「相机」面板内容。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>绘制正交尺寸、近距剪切、远距剪切属性字段。</description></item>
        /// <item><description>非正交模式下绘制视场角字段。</description></item>
        /// <item><description>绘制场景相机字段并在变更时弹出确认对话框。</description></item>
        /// <item><description>场景相机为空时显示错误帮助框。</description></item>
        /// </list>
        /// </summary>
        private void Draw_Camera()
        {
            XGUI.ChangedCheck_Start();

            #region 正交尺寸
            XGUI.layout_property_field(
                title: "正交尺寸",
                title_size: XGUIFontSize.M,
                //title_color: Color.white,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 100,
                //status_icon: "icon_field_status",
                //status_icon_color: CameraOrthographicSize.floatValue != 0 ? XHud_Dashboard.themes_get_color(ThemeColorType.primary) : Color.red,
                prop: sp_camera_ui_orthographic_size,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            XGUI.layout_space(5);

            #region 近距剪切
            XGUI.layout_property_field(
                title: "近距剪切",
                title_size: XGUIFontSize.M,
                //title_color: Color.white,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 100,
                //status_icon: "icon_field_status",
                //status_icon_color: CameraCutter_Near.floatValue != 0, XHud_Dashboard.themes_get_color(ThemeColorType.primary) : Color.red,
                prop: sp_camera_ui_cutter_near,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            XGUI.layout_space(5);

            #region 远距剪切
            XGUI.layout_property_field(
                title: "远距剪切",
                title_size: XGUIFontSize.M,
                //title_color: Color.white,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 100,
                //status_icon: "icon_field_status",
                //status_icon_color: CameraCutter_Far.floatValue != 0 ? XHud_Dashboard.themes_get_color(ThemeColorType.primary) : Color.red,
                prop: sp_camera_ui_cutter_far,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            XGUI.layout_space(5);

            #region 视场角
            if (!sp_camera_othograpic_mode.boolValue)
            {
                XGUI.layout_space(5);

                XGUI.layout_property_field(
                title: "视场角",
                title_size: XGUIFontSize.M,
                //title_color: Color.white,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 100,
                //status_icon: "icon_field_status",
                //status_icon_color: CameraFov.floatValue != 0 ? XHud_Dashboard.themes_get_color(ThemeColorType.primary) : Color.red,
                prop: sp_camera_ui_fov,
                prop_margin: new RectOffset(0, 0, 5, 0));
            }
            #endregion

            if (XGUI.ChangedCheck_End())
            {
                SetHighFrequencyMode();
                xHud_EditorUpdate_CameraArgs();
            }

            #region 场景相机
            XGUI.ChangedCheck_Start();
            XGUI.layout_property_field(
                title: "场景相机",
                title_size: XGUIFontSize.M,
                //title_color: Color.white,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 100,
                status_icon: "icon_field_status",
                status_icon_color: sp_camera_scene.objectReferenceValue != null ? XHud_Dashboard.themes_get_color(ThemeColorType.primary) : Color.red,
                prop: sp_camera_scene,
                prop_margin: new RectOffset(0, 0, 10, 10));
            if (XGUI.ChangedCheck_End())
            {
                if (!Application.isPlaying)
                {
                    Camera cam = (Camera)sp_camera_scene.objectReferenceValue;

                    EditorApplication.delayCall += () =>
                    {
                        XGUI.dialog(
                            type: XGUIDialogType.警告,
                            windowtitle: "XHud - 管理器消息",
                            title: "指定场景相机",
                            msg: $"确认要将 {cam.name} 相机指定为XHud的主场景相机吗？ 如果不指定场景主相机则您将无法正常使用XHud！",
                            ok: "指定",
                            cancel: "暂不",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                            on_selected: (d) =>
                            {
                                if (d == "指定")
                                {
                                    if (cam != null && cam.GetUniversalAdditionalCameraData().renderType == CameraRenderType.Base)
                                    {
                                        base_script.hm_SceneCam_CheckStack((Camera)sp_camera_scene.objectReferenceValue);
                                    }
                                    else
                                    {
                                        sp_camera_scene.objectReferenceValue = null;
                                    }

                                    sp_camera_scene.serializedObject.ApplyModifiedProperties();

                                    // 获取 "XHUD" 图层的 LayerMask
                                    int xhudMask = LayerMask.GetMask("XHud");
                                    // 取消勾选 "XHUD" 图层
                                    cam.cullingMask &= ~xhudMask;

                                    EditorApplication.delayCall += () =>
                                    {
                                        XGUI.dialog(
                                            type: XGUIDialogType.警告,
                                            windowtitle: "XHud - 管理器消息",
                                            title: "指定场景相机",
                                            msg: "已取消勾选主场景相机的 CullingMask 中的 'XHud' 图层！",
                                            ok: "明白 ",
                                            PrimaryIndex: 0,
                                            usemodal: true,
                                            themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                                    };
                                }
                                else
                                {
                                    sp_camera_scene.objectReferenceValue = null;
                                    sp_camera_scene.serializedObject.ApplyModifiedProperties();
                                }
                                return;
                            });
                    };
                }
            }

            if (sp_camera_scene.objectReferenceValue == null)
                XGUI.layout_helpbox(
                    state: XGUIHelboxState.错误,
                    title_text: "请指定场景主相机！",
                    title_size: XGUIFontSize.M,
                    title_style: FontStyle.Normal,
                    title_color: Color.white * 0.75f);
            #endregion
        }
        #endregion
    }
}