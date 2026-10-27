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

    public partial class Editor_XHud_Manager : Editor
    {
        #region 绘制 - 动效参数
        /// <summary>
        /// 绘制「动效默认参数」面板内容。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>绘制生成模板库选择与保存/定位/重置按钮。</description></item>
        /// <item><description>绘制生成位移/旋转/透明度参数折叠面板与动画结束时机。</description></item>
        /// <item><description>绘制回收模板库选择与保存/定位/重置按钮。</description></item>
        /// <item><description>绘制回收位移/旋转/透明度参数折叠面板与动画结束时机。</description></item>
        /// </list>
        /// </summary>
        private void Draw_MotionDefault()
        {
            #region 生成
            #region 动效模板
            if (base_script.library_motions != null)
            {
                //确保动效库不是空的
                if (base_script.library_motions.motion_list == null || base_script.library_motions.motion_list.Count <= 0)
                {
                    XGUI.layout_label(
                        text: "未在动效库中发现任何动效资源，请先为其添加动效资源！",
                        bg_fill: XGUIGroup.实体,
                        bg_color: XGUIColor.深空灰,
                        size: XGUIFontSize.S,
                        anchor: TextAnchor.MiddleLeft,
                        text_color: Color.white * 0.85f,
                        offset: new Vector2(0, 0),
                        padding: new RectOffset(10, 10, 10, 10),
                        margin: new RectOffset(0, 0, 6, 0),
                        clipping: TextClipping.Clip,
                        wrap: true,
                        font_style: FontStyle.Normal);
                }
                else
                {
                    #region 模板下拉菜单
                    string[] motnames = base_script.library_motions.ElementMotion_GetAllName_With_Create();
                    XGUI.layout_string_popup(
                        title: XGUI.CurrentWindowWidthThreshold(">", 240) ? "模板 - 生成" : null,
                        title_width: 120,
                        title_size: XGUIFontSize.M,
                        title_anchor: TextAnchor.MiddleLeft,
                        prop: sp_ele_global_motion_create_library_index_name,
                        options: motnames,
                        opt_text_size: XGUIFontSize.M,
                        opt_text_color: Color.black,
                        opt_text_padding: new RectOffset(10, 10, 0, 0),
                        opt_anchor: TextAnchor.MiddleCenter,
                        opt_font_style: FontStyle.Normal,
                        opt_bg_fill: XGUIGroup.实体,
                        opt_bg_color: XGUIColor.亮白,
                        opt_bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        icon_arrow_color: Color.black,
                        margin: new RectOffset(0, 0, 5, 5),
                        padding: new RectOffset(5, 5, 0, 0),
                        title_margin: new RectOffset(0, 0, 0, 0),
                        act_on_changed: (value) =>
                        {
                            sp_ele_global_motion_create_library_index_name.stringValue = value;
                            sp_ele_global_motion_create_library_index_name.serializedObject.ApplyModifiedProperties();

                            Motion_Creator crc = base_script.library_motions.ElementMotion_GetElementCreator_At_Create(sp_ele_global_motion_create_library_index_name.stringValue);

                            base_script.global_motion_create_args = crc.Clone();
                        });
                    #endregion
                }
            }
            else
            {
                XGUI.layout_label(
                   text: "XHud 管理器中未指定动效库，请先配置动效库！",
                   bg_fill: XGUIGroup.实体,
                   bg_color: XGUIColor.深空灰,
                   size: XGUIFontSize.S,
                   anchor: TextAnchor.MiddleLeft,
                   text_color: Color.white * 0.85f,
                   offset: new Vector2(0, 0),
                   padding: new RectOffset(10, 10, 10, 10),
                   margin: new RectOffset(0, 0, 6, 0),
                   clipping: TextClipping.Clip,
                   wrap: true,
                   font_style: FontStyle.Normal);
            }
            #endregion

            XGUI.layout_group_start(
                type: XGUIContainerType.Horizontal,
                title_clipping: TextClipping.Clip,
                absolute_padding: true,
                absolute_margin: true,
                margin: new RectOffset(0, 0, 12, 0),
                padding: new RectOffset(5, 10, 0, 0));

            #region 保存
            XGUI.SetEnabled(base_script.library_motions != null);
            if (XGUI.layout_button(
                tooltip: "保存",
                tex_release: save_r,
                tex_press: save_p,
                tex_gui_color: Color.white,
                border: new RectOffset(0, 0, 0, 0),
                width: save_r.width,
                height: save_r.height))
            {
                motion_save(HudElementMotionType.Creator);
                return;
            }
            #endregion

            XGUI.layout_flexspace();

            #region 定位
            XGUI.SetEnabled(base_script.library_motions != null);
            if (XGUI.layout_button(
              tooltip: "定位",
              tex_release: locate_r,
              tex_press: locate_p,
              tex_gui_color: Color.white,
              border: new RectOffset(0, 0, 0, 0),
              width: locate_r.width,
              height: locate_r.height))
            {
                if (!base_script.library_motions.ElementMotion_IsExist(sp_ele_global_motion_create_library_index_name.stringValue))
                    return;
                Editor_XHud_MenuItemsAction_OpenLibrary.open_elementmotion();
                base_script.library_motions.ElementMotionLibrary_Location(sp_ele_global_motion_create_library_index_name.stringValue);
                return;
            }
            XGUI.SetEnabled(true);
            #endregion

            XGUI.layout_flexspace();

            #region 修改
            if (XGUI.layout_button(
              tooltip: "修改",
              tex_release: edit_r,
              tex_press: edit_p,
              tex_gui_color: Color.white,
              border: new RectOffset(0, 0, 0, 0),
              width: edit_r.width,
              height: edit_r.height))
            {
                motion_edit(new XHud_Arg_MotionGroup(base_script.global_motion_create_args));
                return;
            }
            #endregion

            XGUI.layout_flexspace();

            #region 重置
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
                    windowtitle: "XHud - 元素生成器消息",
                    title: "重置动效参数",
                    msg: "确定要将动效参数重置吗？您将丢失当前的动效参数！",
                    ok: "重置",
                    cancel: "暂不",
                    PrimaryIndex: 0,
                    usemodal: true,
                    themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    on_selected: (d) =>
                    {
                        if (d == "重置")
                        {
                            sp_ele_global_motion_create_library_index_name.stringValue = null;
                            sp_ele_global_motion_create_library_index_name.serializedObject.ApplyModifiedProperties();
                            base_script.global_motion_create_args.Reset();
                        }
                    });
                return;
            }
            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            #endregion

            XGUI.layout_seperator(
                thickness: 1,
                color: XHud_Dashboard.themes_get_color(ThemeColorType.sepline),
                margin: new RectOffset(15, 15, 15, 15));

            #region 回收
            #region 动效模板
            if (base_script.library_motions != null)
            {
                //确保动效库不是空的
                if (base_script.library_motions.motion_list == null || base_script.library_motions.motion_list.Count <= 0)
                {
                    XGUI.layout_label(
                        text: "未在动效库中发现任何动效资源，请先为其添加动效资源！",
                        bg_fill: XGUIGroup.实体,
                        bg_color: XGUIColor.深空灰,
                        size: XGUIFontSize.S,
                        anchor: TextAnchor.MiddleLeft,
                        text_color: Color.white * 0.85f,
                        offset: new Vector2(0, 0),
                        padding: new RectOffset(10, 10, 10, 10),
                        margin: new RectOffset(0, 0, 6, 0),
                        clipping: TextClipping.Clip,
                        wrap: true,
                        font_style: FontStyle.Normal);
                }
                else
                {
                    #region 模板下拉菜单
                    string[] motnames = base_script.library_motions.ElementMotion_GetAllName_With_Recycle();
                    XGUI.layout_string_popup(
                        title: XGUI.CurrentWindowWidthThreshold(">", 240) ? "模板 - 回收" : null,
                        title_width: 120,
                        title_size: XGUIFontSize.M,
                        title_anchor: TextAnchor.MiddleLeft,
                        prop: sp_ele_global_motion_recycle_library_index_name,
                        options: motnames,
                        opt_text_size: XGUIFontSize.M,
                        opt_text_color: Color.black,
                        opt_text_padding: new RectOffset(10, 10, 0, 0),
                        opt_anchor: TextAnchor.MiddleCenter,
                        opt_font_style: FontStyle.Normal,
                        opt_bg_fill: XGUIGroup.实体,
                        opt_bg_color: XGUIColor.亮白,
                        opt_bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        icon_arrow_color: Color.black,
                        margin: new RectOffset(0, 0, 5, 5),
                        padding: new RectOffset(5, 5, 0, 0),
                        title_margin: new RectOffset(0, 0, 0, 0),
                        act_on_changed: (value) =>
                        {
                            sp_ele_global_motion_recycle_library_index_name.stringValue = value;
                            sp_ele_global_motion_recycle_library_index_name.serializedObject.ApplyModifiedProperties();

                            Motion_Recycler rec = base_script.library_motions.ElementMotion_GetElementCreator_At_Recycle(sp_ele_global_motion_recycle_library_index_name.stringValue);
                            base_script.global_motion_recycle_args = rec.Clone();
                        });
                    #endregion
                }
            }
            else
            {
                XGUI.layout_label(
                   text: "XHud 管理器中未指定动效库，请先配置动效库！",
                   bg_fill: XGUIGroup.实体,
                   bg_color: XGUIColor.深空灰,
                   size: XGUIFontSize.S,
                   anchor: TextAnchor.MiddleLeft,
                   text_color: Color.white * 0.85f,
                   offset: new Vector2(0, 0),
                   padding: new RectOffset(10, 10, 10, 10),
                   margin: new RectOffset(0, 0, 6, 0),
                   clipping: TextClipping.Clip,
                   wrap: true,
                   font_style: FontStyle.Normal);
            }
            #endregion

            XGUI.layout_group_start(
                type: XGUIContainerType.Horizontal,
                title_clipping: TextClipping.Clip,
                absolute_padding: true,
                absolute_margin: true,
                margin: new RectOffset(0, 0, 12, 0),
                padding: new RectOffset(5, 10, 0, 0));

            #region 保存
            XGUI.SetEnabled(base_script.library_motions != null);
            if (XGUI.layout_button(
                tooltip: "保存",
                tex_release: save_r,
                tex_press: save_p,
                tex_gui_color: Color.white,
                border: new RectOffset(0, 0, 0, 0),
                width: save_r.width,
                height: save_r.height))
            {
                motion_save(HudElementMotionType.Recycler);
                return;
            }
            #endregion

            XGUI.layout_flexspace();

            #region 定位
            XGUI.SetEnabled(base_script.library_motions != null);
            if (XGUI.layout_button(
              tooltip: "定位",
              tex_release: locate_r,
              tex_press: locate_p,
              tex_gui_color: Color.white,
              border: new RectOffset(0, 0, 0, 0),
              width: locate_r.width,
              height: locate_r.height))
            {
                if (!base_script.library_motions.ElementMotion_IsExist(sp_ele_global_motion_recycle_library_index_name.stringValue))
                    return;
                Editor_XHud_MenuItemsAction_OpenLibrary.open_elementmotion();
                base_script.library_motions.ElementMotionLibrary_Location(sp_ele_global_motion_recycle_library_index_name.stringValue);
                return;
            }
            XGUI.SetEnabled(true);
            #endregion

            XGUI.layout_flexspace();

            #region 修改
            if (XGUI.layout_button(
              tooltip: "修改",
              tex_release: edit_r,
              tex_press: edit_p,
              tex_gui_color: Color.white,
              border: new RectOffset(0, 0, 0, 0),
              width: edit_r.width,
              height: edit_r.height))
            {
                motion_edit(new XHud_Arg_MotionGroup(base_script.global_motion_recycle_args));
                return;
            }
            #endregion

            XGUI.layout_flexspace();

            #region 重置
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
                    windowtitle: "XHud - 元素生成器消息",
                    title: "重置动效参数",
                    msg: "确定要将动效参数重置吗？您将丢失当前的动效参数！",
                    ok: "重置",
                    cancel: "暂不",
                    PrimaryIndex: 0,
                    usemodal: true,
                    themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    on_selected: (d) =>
                    {
                        if (d == "重置")
                        {
                            sp_ele_global_motion_recycle_library_index_name.stringValue = null;
                            sp_ele_global_motion_recycle_library_index_name.serializedObject.ApplyModifiedProperties();
                            base_script.global_motion_recycle_args.Reset();
                        }
                    });
                return;
            }
            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            #endregion
        }
        #endregion

        #region 动效设置
        /// <summary>
        /// 打开元素动效资源采集器窗口，用于把当前生成或回收动效保存到动效库
        /// </summary>
        /// <param name="Type">
        /// 动效类型（生成动效或回收动效）
        /// </param>
        public void motion_save(HudElementMotionType Type)
        {
            Editor_XHud_LibrarySetTool_Motion window = EditorWindow.GetWindow<Editor_XHud_LibrarySetTool_Motion>(true);

            window.titleContent = new GUIContent("XHud - 元素动效资源采集器");
            XGUI.CenterEditorWindow(new Vector2Int(348, Type == HudElementMotionType.Creator ? 870 : 800), window);

            switch (Type)
            {
                case HudElementMotionType.Creator:
                    window.set_motion(base_script.global_motion_create_args.Clone());
                    break;
                case HudElementMotionType.Recycler:
                    window.set_motion(base_script.global_motion_recycle_args.Clone());
                    break;
            }

            window.set_motion_type(Type);
            window.set_library_setter_mode(LibrarySetterMode.添加到库);
            window.set_button_text("添加", "取消");
            window.set_title("元素动效资源采集器");
            window.set_target_motion_library(XHud_Dashboard.get_manager().library_motions);
            //window.ShowModal();
            window.Show();
        }
        /// <summary>
        /// 打开动效资源修改器窗口，用于编辑当前生成或回收动效参数
        /// </summary>
        /// <param name="group">
        /// 待编辑的动效参数分组
        /// </param>
        public void motion_edit(XHud_Arg_MotionGroup group)
        {
            Editor_XHud_LibrarySetTool_Motion window = EditorWindow.GetWindow<Editor_XHud_LibrarySetTool_Motion>(true);

            window.titleContent = new GUIContent("XHud - 动效资源修改器");
            XGUI.CenterEditorWindow(new Vector2Int(348, group.Type == HudElementMotionType.Creator ? 710 : 640), window);
            window.set_motion_type(group.Type);
            window.set_preedit_target_id(base_script.GetInstanceID().ToString());
            window.set_motion(group.Creator, group.Recycler);
            window.set_library_setter_mode(LibrarySetterMode.修改动效参数);
            window.set_button_text("更新");

            string hexcol = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.themes_get_color(ThemeColorType.primary), true);
            window.set_title($"<color={hexcol}>{base_script.gameObject.name} </color>");

            //window.ShowModal();
            window.Show();
        }
        #endregion
    }
}