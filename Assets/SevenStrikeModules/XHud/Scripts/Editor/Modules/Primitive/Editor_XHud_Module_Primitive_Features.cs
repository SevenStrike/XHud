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
    [CustomEditor(typeof(XHud_Module_Primitive_Feature))]
    public class Editor_XHud_Module_Primitive_Features : Editor
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
        private XHud_Module_Primitive_Feature base_script;
        #endregion

        #region 字段 - 序列化属性
        /// <summary>
        /// 调试开关属性
        /// </summary>
        private SerializedProperty sp_debug;
        /// <summary>
        /// 图元特性数据属性
        /// </summary>
        private SerializedProperty sp_primitive_features;
        /// <summary>
        /// 首次记录标记属性
        /// </summary>
        private SerializedProperty sp_first_save_features;
        #endregion

        #region 字段 - GUI 参数
        /// <summary>
        /// 多选模式下当前查看的特性索引。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>取值范围为 <c>0</c> 到 <c>SelectedObjects.Length - 1</c>。</description></item>
        /// <item><description>通过上一个/下一个按钮循环切换。</description></item>
        /// </list>
        /// </summary>
        private int multi_primitive_feature_index;
        #endregion

        #region 字段 - 选项文字
        /// <summary>
        /// 调试开关的选项文字。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>索引 0：关闭。</description></item>
        /// <item><description>索引 1：调试。</description></item>
        /// </list>
        /// </summary>
        string[] stroptions_debug = new string[2] { "关闭", "调试" };
        #endregion

        #region 字段 - 图标
        /// <summary>
        /// 标题主图标
        /// </summary>
        private Texture2D icon_main;
        /// <summary>
        /// 位置字段图标
        /// </summary>
        private Texture2D f_pos;
        /// <summary>
        /// 旋转字段图标
        /// </summary>
        private Texture2D f_rot;
        /// <summary>
        /// 缩放字段图标
        /// </summary>
        private Texture2D f_sca;
        /// <summary>
        /// 尺寸字段图标
        /// </summary>
        private Texture2D f_size;
        /// <summary>
        /// 透明度字段图标
        /// </summary>
        private Texture2D f_alp;
        /// <summary>
        /// 填充字段图标
        /// </summary>
        private Texture2D f_fill;
        /// <summary>
        /// 颜色字段图标
        /// </summary>
        private Texture2D f_color;
        /// <summary>
        /// 上一个按钮常态图标
        /// </summary>
        private Texture2D left_arrow_r;
        /// <summary>
        /// 上一个按钮按下图标
        /// </summary>
        private Texture2D left_arrow_p;
        /// <summary>
        /// 下一个按钮常态图标
        /// </summary>
        private Texture2D right_arrow_r;
        /// <summary>
        /// 下一个按钮按下图标
        /// </summary>
        private Texture2D right_arrow_p;
        /// <summary>
        /// 记录特性按钮常态图标
        /// </summary>
        private Texture2D f_save_r;
        /// <summary>
        /// 记录特性按钮按下图标
        /// </summary>
        private Texture2D f_save_p;
        /// <summary>
        /// 读取特性按钮常态图标
        /// </summary>
        private Texture2D f_load_r;
        /// <summary>
        /// 读取特性按钮按下图标
        /// </summary>
        private Texture2D f_load_p;
        /// <summary>
        /// 颜色标记图标
        /// </summary>
        private Texture2D icon_col_mark;
        #endregion

        #region 字段 - 批量化操作
        /// <summary>
        /// 批量选择脚本数组。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>在 <c>Targets_Get</c> 中根据 <c>targets</c> 填充。</description></item>
        /// <item><description>单选时长度为 1，多选时长度等于选中对象数量。</description></item>
        /// </list>
        /// </summary>
        XHud_Module_Primitive_Feature[] selected_objects;
        /// <summary>
        /// 采集的特性数据数组。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>在 <c>PrimitiveFeature_Capture</c> 中根据批量目标填充。</description></item>
        /// <item><description>仅用于缓存当前批量目标的特性快照。</description></item>
        /// </list>
        /// </summary>
        private PrimitiveFeatures[] primitive_features;
        #endregion

        #region 生命周期
        private void OnEnable()
        {
            base_script = (XHud_Module_Primitive_Feature)target;

            Targets_Get();

            #region 获取所有序列化字段
            sp_primitive_features = serializedObject.FindProperty(nameof(XHud_Module_Primitive_Feature.primitive_features));
            sp_debug = serializedObject.FindProperty(nameof(XHud_Module_Primitive_Feature.debug));
            sp_first_save_features = serializedObject.FindProperty(nameof(XHud_Module_Primitive_Feature.first_save_features));
            #endregion

            #region 获取图标          
            icon_main = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_features/icon_main");
            f_pos = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_features/f_pos");
            f_rot = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_features/f_rot");
            f_sca = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_features/f_sca");
            f_size = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_features/f_size");
            f_alp = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_features/f_alp");
            f_fill = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_features/f_fill");
            f_color = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_features/f_color");
            left_arrow_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_features/left_arrow_r");
            left_arrow_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_features/left_arrow_p");
            right_arrow_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_features/right_arrow_r");
            right_arrow_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_features/right_arrow_p");
            f_save_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_features/f_save_r");
            f_save_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_features/f_save_p");
            f_load_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_features/f_load_r");
            f_load_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_features/f_load_p");
            icon_col_mark = XGUI.GetBasedIcon("icon_stop_r");
            #endregion

            CheckFirstCreatedFeature();

            PrimitiveFeature_Capture(selected_objects);
        }

        private void OnDisable()
        {
            if (!Application.isPlaying)
            {
                if (EditorWindow.HasOpenInstances<Editor_XHud_Module_Primitive_Tween_Tracker>())
                    return;

                #region 记录特性
                if (Targets_Selected())
                {
                    for (int i = 0; i < selected_objects.Length; i++)
                    {
                        if (selected_objects[i] != null)
                        {
                            if (!selected_objects[i].controller.pt_tween.tween_is_previewing)
                            {
                                PrimitiveFeature_Save(selected_objects[i]);
                            }
                            else
                            {
                                PrimitiveFeature_Load(selected_objects[i]);
                            }
                        }
                    }
                }
                else
                {
                    if (target != null)
                    {
                        if (!base_script.controller.pt_tween.tween_is_previewing)
                        {
                            PrimitiveFeature_Save(base_script);
                            serializedObject.ApplyModifiedProperties();
                        }
                        else
                        {
                            PrimitiveFeature_Load(base_script);
                        }
                    }
                }
                #endregion
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            TextClipping clipping = XGUI.TryEllipsisClipping();

            #region 标题
            XGUI.layout_banner(
                bg_fill: XGUIGroup.实体,
                bg_color: XGUIColor.深空灰,
                bg_height: 30,
                icon: icon_main,
                icon_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_text: "XHud  -  图元  >  特性",
                title_anchor: TextAnchor.MiddleLeft,
                title_style: FontStyle.Normal,
                title_color: Color.white,
                title_size: XGUIFontSize.B,
                title_clipping: clipping,
                bg_margin: new RectOffset(0, 0, 5, 5));
            #endregion

            Rect rect = XGUI.GetLastRect();

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

            #region 特性记录
            if (XGUI.layout_button(
                tooltip: "特性记录",
                tex_release: f_save_r,
                tex_press: f_save_p,
                tex_gui_color: Color.white,
                border: new RectOffset(0, 0, 0, 0),
                width: 14,
                height: 14))
            {
                PrimitiveFeature_Dual_Save_WithDialog();
            }
            #endregion

            XGUI.layout_flexspace();

            #region 特性读取
            if (XGUI.layout_button(
                tooltip: "特性读取",
                tex_release: f_load_r,
                tex_press: f_load_p,
                tex_gui_color: Color.white,
                border: new RectOffset(0, 0, 0, 0),
                width: 14,
                height: 14))
            {
                PrimitiveFeature_Dual_Load_WithDialog();
            }
            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            #endregion

            #region 特性参数
            string statu_title = "特性参数";
            if (Targets_Selected())
                statu_title = "特性参数 - ( 批量模式 )";

            base_script.fold_param = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: statu_title,
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: base_script.fold_param);

            if (!base_script.fold_param)
            {
                if (!Targets_Selected())
                {
                    SerializedProperty pos = sp_primitive_features.FindPropertyRelative(nameof(PrimitiveFeatures.position));
                    SerializedProperty eur = sp_primitive_features.FindPropertyRelative(nameof(PrimitiveFeatures.euler));
                    SerializedProperty sca = sp_primitive_features.FindPropertyRelative(nameof(PrimitiveFeatures.scale));
                    SerializedProperty size = sp_primitive_features.FindPropertyRelative(nameof(PrimitiveFeatures.size));
                    SerializedProperty alp = sp_primitive_features.FindPropertyRelative(nameof(PrimitiveFeatures.alpha));
                    SerializedProperty col = sp_primitive_features.FindPropertyRelative(nameof(PrimitiveFeatures.color));
                    SerializedProperty fil = sp_primitive_features.FindPropertyRelative(nameof(PrimitiveFeatures.fill));

                    #region 位置
                    XGUI.layout_state_displayer_text(
                        title: "位置",
                        title_size: XGUIFontSize.M,
                        subtitle: pos.vector3Value.ToString(),
                        subtitle_size: XGUIFontSize.M,
                        subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        margin: new RectOffset(5, 5, 0, 5));
                    #endregion

                    #region 角度
                    XGUI.layout_state_displayer_text(
                       title: "角度",
                       title_size: XGUIFontSize.M,
                       subtitle: eur.vector3Value.ToString(),
                       subtitle_size: XGUIFontSize.M,
                       subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                       margin: new RectOffset(5, 5, 0, 5));
                    #endregion

                    #region 缩放
                    XGUI.layout_state_displayer_text(
                        title: "缩放",
                        title_size: XGUIFontSize.M,
                        subtitle: sca.vector3Value.ToString(),
                        subtitle_size: XGUIFontSize.M,
                        subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        margin: new RectOffset(5, 5, 0, 5));
                    #endregion

                    #region 尺寸
                    XGUI.layout_state_displayer_text(
                       title: "尺寸",
                       title_size: XGUIFontSize.M,
                       subtitle: size.vector2Value.ToString(),
                       subtitle_size: XGUIFontSize.M,
                       subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                       margin: new RectOffset(5, 5, 0, 5));
                    #endregion

                    #region 透明度
                    XGUI.layout_state_displayer_text(
                       title: "透明度",
                       title_size: XGUIFontSize.M,
                       subtitle: alp.floatValue.ToString(),
                       subtitle_size: XGUIFontSize.M,
                       subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                       margin: new RectOffset(5, 5, 0, 5));
                    #endregion

                    #region 填充
                    XGUI.layout_state_displayer_text(
                       title: "填充",
                       title_size: XGUIFontSize.M,
                       subtitle: fil.floatValue.ToString(),
                       subtitle_size: XGUIFontSize.M,
                       subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                       margin: new RectOffset(5, 5, 0, 5));
                    #endregion

                    #region 颜色
                    XGUI.layout_state_displayer_icon(
                     title: "颜色",
                     title_size: XGUIFontSize.M,
                     icon: icon_col_mark,
                     icon_size: new Vector2(14, 14),
                     icon_offset: new Vector2(0, 0),
                     icon_color: col.colorValue,
                     padding: new RectOffset(0, 0, 0, 0),
                     margin: new RectOffset(5, 5, 0, 5));
                    #endregion
                }
                else
                {
                    #region 控制区
                    XGUI.layout_group_start(
                        type: XGUIContainerType.Horizontal,
                        bg_fill: XGUIGroup.透明,
                        bg_color: XGUIColor.亮白,
                        bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                        absolute_margin: true,
                        absolute_padding: true,
                        margin: new RectOffset(0, 0, 0, 0),
                        padding: new RectOffset(15, 15, 5, 10));

                    #region 标题按钮
                    if (XGUI.layout_button(
                        text: $"{selected_objects[multi_primitive_feature_index].name}",
                        tooltip: "",
                        bg_fill: XGUIGroup.透明,
                        button_text_color: Color.gray,
                        press_fill: XGUIGroup.透明,
                        press_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        font_size: XGUIFontSize.M,
                        anchor: TextAnchor.MiddleLeft,
                        margin: new RectOffset(0, 0, 0, 0),
                        padding: new RectOffset(0, 0, 0, 0),
                        layout_width: 150,
                        button_text_font: XGUI.GetFont("xg-medium")))
                    {
                        EditorGUIUtility.PingObject(selected_objects[multi_primitive_feature_index]);
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
                        if (multi_primitive_feature_index <= 0)
                        {
                            multi_primitive_feature_index = selected_objects.Length - 1;
                        }
                        else
                        {
                            multi_primitive_feature_index--;
                        }
                        EditorGUIUtility.PingObject(selected_objects[multi_primitive_feature_index]);
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
                        if (multi_primitive_feature_index >= selected_objects.Length - 1)
                        {
                            multi_primitive_feature_index = 0;
                        }
                        else
                        {
                            multi_primitive_feature_index++;
                        }
                        EditorGUIUtility.PingObject(selected_objects[multi_primitive_feature_index]);
                    }
                    #endregion

                    XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
                    #endregion

                    XGUI.layout_seperator(
                            thickness: 1,
                            color: XHud_Dashboard.themes_get_color(ThemeColorType.sepline),
                            margin: new RectOffset(15, 15, 5, 25));

                    #region 位置
                    XGUI.layout_state_displayer_text(
                        title: "位置",
                    title_size: XGUIFontSize.M,
                        subtitle: selected_objects[multi_primitive_feature_index].primitive_features.position.ToString(),
                        subtitle_size: XGUIFontSize.M,
                        subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        margin: new RectOffset(5, 5, 0, 5));
                    #endregion

                    #region 角度
                    XGUI.layout_state_displayer_text(
                       title: "角度",
                       title_size: XGUIFontSize.M,
                       subtitle: selected_objects[multi_primitive_feature_index].primitive_features.euler.ToString(),
                       subtitle_size: XGUIFontSize.M,
                       subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                       margin: new RectOffset(5, 5, 0, 5));
                    #endregion

                    #region 缩放
                    XGUI.layout_state_displayer_text(
                        title: "缩放",
                        title_size: XGUIFontSize.M,
                        subtitle: selected_objects[multi_primitive_feature_index].primitive_features.scale.ToString(),
                        subtitle_size: XGUIFontSize.M,
                        subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        margin: new RectOffset(5, 5, 0, 5));
                    #endregion

                    #region 尺寸
                    XGUI.layout_state_displayer_text(
                       title: "尺寸",
                       title_size: XGUIFontSize.M,
                       subtitle: selected_objects[multi_primitive_feature_index].primitive_features.size.ToString(),
                       subtitle_size: XGUIFontSize.M,
                       subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                       margin: new RectOffset(5, 5, 0, 5));
                    #endregion

                    #region 透明度
                    XGUI.layout_state_displayer_text(
                       title: "透明度",
                       title_size: XGUIFontSize.M,
                       subtitle: selected_objects[multi_primitive_feature_index].primitive_features.alpha.ToString(),
                       subtitle_size: XGUIFontSize.M,
                       subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                       margin: new RectOffset(5, 5, 0, 5));
                    #endregion

                    #region 填充
                    XGUI.layout_state_displayer_text(
                       title: "填充",
                       title_size: XGUIFontSize.M,
                       subtitle: selected_objects[multi_primitive_feature_index].primitive_features.fill.ToString(),
                       subtitle_size: XGUIFontSize.M,
                       subtitle_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                       margin: new RectOffset(5, 5, 0, 5));
                    #endregion

                    #region 颜色
                    XGUI.layout_state_displayer_icon(
                     title: "颜色",
                     title_size: XGUIFontSize.M,
                     icon: icon_col_mark,
                     icon_size: new Vector2(14, 14),
                     icon_offset: new Vector2(0, 0),
                     icon_color: selected_objects[multi_primitive_feature_index].primitive_features.color,
                     padding: new RectOffset(0, 0, 0, 0),
                     margin: new RectOffset(5, 5, 0, 5));
                    #endregion
                }
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 选项
            base_script.fold_option = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "选项",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: base_script.fold_option);

            if (!base_script.fold_option)
            {
                #region 状态调试
                DrawToggle("状态调试", sp_debug, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, stroptions_debug, (b) => { });
                #endregion
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 右键菜单
            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("S (记录特性)"), false, () =>
                {
                    PrimitiveFeature_Dual_Save_WithDialog();
                });
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("X (读取特性)"), false, () =>
                {
                    PrimitiveFeature_Dual_Load_WithDialog();
                });
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Q (折叠编组)"), false, () =>
                {
                    if (Targets_Selected())
                    {
                        for (int i = 0; i < selected_objects.Length; i++)
                        {
                            selected_objects[i].GroupFold(true);
                        }
                    }
                    else
                    {
                        base_script.GroupFold(true);
                    }
                });
                menu.AddItem(new GUIContent("W (展开编组)"), false, () =>
                {
                    if (Targets_Selected())
                    {
                        for (int i = 0; i < selected_objects.Length; i++)
                        {
                            selected_objects[i].GroupFold(false);
                        }
                    }
                    else
                    {
                        base_script.GroupFold(false);
                    }
                });
                menu.ShowAsContext();
            }
            #endregion

            #region 源脚本
            base_script.fold_based = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "源脚本",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: base_script.fold_based);

            if (!base_script.fold_based)
            {
                DrawDefaultInspector();
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            serializedObject.ApplyModifiedProperties();
        }
        #endregion

        #region 工具 - 特性状态
        /// <summary>
        /// 检测图元特性首次初始状态。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>多选时逐个检查并首次记录特性。</description></item>
        /// <item><description>单选时检查并首次记录特性。</description></item>
        /// </list>
        /// </summary>
        private void CheckFirstCreatedFeature()
        {
            if (Targets_Selected())
            {
                for (int i = 0; i < selected_objects.Length; i++)
                {
                    XHud_Module_Primitive_Feature feature = selected_objects[i];
                    SerializedObject so = new SerializedObject(feature);
                    so.Update();
                    SerializedProperty prop_FirstSaveFeatures = so.FindProperty(nameof(XHud_Module_Primitive_Feature.first_save_features));
                    if (!prop_FirstSaveFeatures.boolValue)
                    {
                        prop_FirstSaveFeatures.boolValue = true;
                        // 首次记录姿态信息
                        PrimitiveFeature_Save(feature);
                    }
                    so.ApplyModifiedProperties();
                }
            }
            else
            {
                if (!sp_first_save_features.boolValue)
                {
                    sp_first_save_features.boolValue = true;
                    serializedObject.ApplyModifiedProperties();

                    // 首次记录姿态信息
                    PrimitiveFeature_Save(base_script);
                }
            }
        }
        #endregion

        #region 工具 - 特性参数
        /// <summary>
        /// 记录 PrimitiveFeature 特性参数。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>保存 Rect 的位置、角度、缩放、尺寸。</description></item>
        /// <item><description>保存 CanvasGroup 的透明度。</description></item>
        /// <item><description>保存 Image/Text/TmpText/RawImage 的颜色与填充。</description></item>
        /// </list>
        /// </summary>
        private void PrimitiveFeature_Save(XHud_Module_Primitive_Feature features)
        {
            // 添加空值检查
            if (features == null || features.controller == null)
                return;

            using (SerializedObject so = new SerializedObject(features))
            {
                so.Update();

                // 获取特性类
                SerializedProperty feature = so.FindProperty(nameof(XHud_Module_Primitive_Feature.primitive_features));
                // 获取特性类的字段
                SerializedProperty pos = feature.FindPropertyRelative(nameof(PrimitiveFeatures.position));
                SerializedProperty eur = feature.FindPropertyRelative(nameof(PrimitiveFeatures.euler));
                SerializedProperty sca = feature.FindPropertyRelative(nameof(PrimitiveFeatures.scale));
                SerializedProperty size = feature.FindPropertyRelative(nameof(PrimitiveFeatures.size));
                SerializedProperty alp = feature.FindPropertyRelative(nameof(PrimitiveFeatures.alpha));
                SerializedProperty col = feature.FindPropertyRelative(nameof(PrimitiveFeatures.color));
                SerializedProperty fil = feature.FindPropertyRelative(nameof(PrimitiveFeatures.fill));

                // 保存特性：Rect
                if (features.controller.mod_rect != null)
                {
                    pos.vector3Value = features.controller.mod_rect.anchoredPosition3D;
                    eur.vector3Value = features.controller.mod_rect.localEulerAngles;
                    sca.vector3Value = features.controller.mod_rect.localScale;
                    size.vector2Value = features.controller.mod_rect.sizeDelta;
                }
                // 保存特性：CanvasGroup
                if (features.controller.mod_canvas_group != null)
                {
                    alp.floatValue = features.controller.mod_canvas_group.alpha;
                }
                // 保存特性：Image
                if (features.controller.mod_image != null)
                {
                    col.colorValue = features.controller.mod_image.color;
                    fil.floatValue = features.controller.mod_image.fillAmount;
                }
                // 保存特性：Text
                else if (features.controller.mod_text != null)
                {
                    col.colorValue = features.controller.mod_text.color;
                }
                // 保存特性：TmpText
                else if (features.controller.mod_tmp_text != null)
                {
                    col.colorValue = features.controller.mod_tmp_text.color;
                }
                // 保存特性：_RawImage
                else if (features.controller.mod_raw_image != null)
                {
                    col.colorValue = features.controller.mod_raw_image.color;
                }

                so.ApplyModifiedProperties();
            }
        }
        /// <summary>
        /// 还原 PrimitiveFeature 特性参数。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>读取 Rect 的位置、角度、缩放、尺寸。</description></item>
        /// <item><description>读取 CanvasGroup 的透明度。</description></item>
        /// <item><description>读取 Image/Text/TmpText/RawImage 的颜色与填充。</description></item>
        /// </list>
        /// </summary>
        private void PrimitiveFeature_Load(XHud_Module_Primitive_Feature features)
        {
            // 添加空值检查
            if (features == null || features.controller == null)
                return;

            using (SerializedObject so = new SerializedObject(features))
            {
                so.Update();

                // 获取特性类
                SerializedProperty feature = so.FindProperty(nameof(XHud_Module_Primitive_Feature.primitive_features));
                // 获取特性类的字段
                SerializedProperty pos = feature.FindPropertyRelative(nameof(PrimitiveFeatures.position));
                SerializedProperty eur = feature.FindPropertyRelative(nameof(PrimitiveFeatures.euler));
                SerializedProperty sca = feature.FindPropertyRelative(nameof(PrimitiveFeatures.scale));
                SerializedProperty size = feature.FindPropertyRelative(nameof(PrimitiveFeatures.size));
                SerializedProperty alp = feature.FindPropertyRelative(nameof(PrimitiveFeatures.alpha));
                SerializedProperty col = feature.FindPropertyRelative(nameof(PrimitiveFeatures.color));
                SerializedProperty fil = feature.FindPropertyRelative(nameof(PrimitiveFeatures.fill));

                // 读取特性：Rect
                if (features.controller.mod_rect != null)
                {
                    features.controller.mod_rect.anchoredPosition3D = pos.vector3Value;
                    features.controller.mod_rect.localEulerAngles = eur.vector3Value;
                    features.controller.mod_rect.localScale = sca.vector3Value;
                    features.controller.mod_rect.sizeDelta = size.vector2Value;
                }
                // 读取特性：CanvasGroup
                if (features.controller.mod_canvas_group != null)
                {
                    features.controller.mod_canvas_group.alpha = alp.floatValue;
                }
                // 读取特性：Image
                if (features.controller.mod_image != null)
                {
                    features.controller.pt_painting.UpdateColor(col.colorValue);
                    features.controller.mod_image.color = col.colorValue;
                    features.controller.mod_image.fillAmount = fil.floatValue;
                }
                // 读取特性：Text
                else if (features.controller.mod_text != null)
                {
                    features.controller.mod_text.color = col.colorValue;
                }
                // 读取特性：TmpText
                else if (features.controller.mod_tmp_text != null)
                {
                    features.controller.mod_tmp_text.color = col.colorValue;
                }
                // 读取特性：RawImage
                else if (features.controller.mod_raw_image != null)
                {
                    features.controller.pt_painting.UpdateColor(col.colorValue);
                    features.controller.mod_raw_image.color = col.colorValue;
                }

                so.ApplyModifiedProperties();
            }
        }
        /// <summary>
        /// 采集 PrimitiveFeature 特性参数。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>根据传入的脚本数组创建特性快照。</description></item>
        /// <item><description>缓存到 <c>PrimitiveFeatures</c> 字段。</description></item>
        /// </list>
        /// </summary>
        private void PrimitiveFeature_Capture(XHud_Module_Primitive_Feature[] features)
        {
            primitive_features = new PrimitiveFeatures[features.Length];
            for (int i = 0; i < features.Length; i++)
            {
                primitive_features[i] = features[i].primitive_features;
            }
        }
        /// <summary>
        /// 批处理特性保存（含确认弹窗）。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>多选时逐个保存并弹出列表通知。</description></item>
        /// <item><description>单选时保存并弹出确认对话框。</description></item>
        /// </list>
        /// </summary>
        private void PrimitiveFeature_Dual_Save_WithDialog()
        {
            // 多选时
            if (Targets_Selected())
            {
                List<XGUIDialogListDatas> Datas = new List<XGUIDialogListDatas>();

                for (int i = 0; i < selected_objects.Length; i++)
                {
                    PrimitiveFeature_Save(selected_objects[i]);
                    XGUIDialogListDatas data = new XGUIDialogListDatas();

                    data.Title = string.IsNullOrEmpty(selected_objects[i].controller.indicator) ? selected_objects[i].name : selected_objects[i].controller.indicator;
                    data.SubTitle = "";
                    data.Message = "已记录特性";
                    Datas.Add(data);
                }
                EditorApplication.delayCall += () =>
                {
                    XGUI.dialog_listview(
                       datas: Datas.ToArray(),
                       type: XGUIDialogType.通知,
                       windowtitle: "XHud - 图元特性消息",
                       title: "批量记录图元特性",
                       msg: "以下是批量已记录特性的所有图元列表，请您检查核对：",
                       ok: "明白",
                       PrimaryIndex: 0,
                       show_index: false,
                       usemodal: false,
                       themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                };
            }
            // 单选时
            else
            {
                PrimitiveFeature_Save(base_script);
                EditorApplication.delayCall += () =>
                {
                    XGUI.dialog(
                        type: XGUIDialogType.确认,
                        windowtitle: "XHud - 图元特性消息",
                        title: "图元特性记录",
                        msg: $"已记录图元的特性参数！",
                        ok: "明白",
                        PrimaryIndex: 0,
                        usemodal: true,
                        themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                };
            }
        }
        /// <summary>
        /// 批处理特性读取（含确认弹窗）。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>多选时逐个读取并弹出列表通知。</description></item>
        /// <item><description>单选时读取并弹出确认对话框。</description></item>
        /// </list>
        /// </summary>
        private void PrimitiveFeature_Dual_Load_WithDialog()
        {
            // 多选时
            if (Targets_Selected())
            {
                List<XGUIDialogListDatas> Datas = new List<XGUIDialogListDatas>();

                for (int i = 0; i < selected_objects.Length; i++)
                {
                    PrimitiveFeature_Load(selected_objects[i]);
                    XGUIDialogListDatas data = new XGUIDialogListDatas();

                    data.Title = string.IsNullOrEmpty(selected_objects[i].controller.indicator) ? selected_objects[i].name : selected_objects[i].controller.indicator;
                    data.SubTitle = "";
                    data.Message = "已读取特性";
                    Datas.Add(data);
                }

                EditorApplication.delayCall += () =>
                {
                    XGUI.dialog_listview(
                        datas: Datas.ToArray(),
                        type: XGUIDialogType.通知,
                        windowtitle: "XHud - 图元特性消息",
                        title: "批量读取图元特性",
                        msg: "以下是批量已读取特性的所有图元列表，请您检查核对：",
                        ok: "明白",
                        PrimaryIndex: 0,
                        show_index: false,
                        usemodal: false,
                        themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                };
            }
            // 单选时
            else
            {
                PrimitiveFeature_Load(base_script);
                EditorApplication.delayCall += () =>
                {
                    XGUI.dialog(
                        type: XGUIDialogType.确认,
                        windowtitle: "XHud - 图元特性消息",
                        title: "图元特性读取",
                        msg: $"已读取图元的特性参数！",
                        ok: "明白",
                        PrimaryIndex: 0,
                        usemodal: true,
                        themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                };
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
                selected_objects = new XHud_Module_Primitive_Feature[targets.Length];
                for (int i = 0; i < selected_objects.Length; i++)
                {
                    var t = targets[i];
                    selected_objects[i] = (XHud_Module_Primitive_Feature)t;
                }
            }
            else
            {
                selected_objects = new XHud_Module_Primitive_Feature[targets.Length];
                selected_objects[0] = (XHud_Module_Primitive_Feature)target;
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
            if (selected_objects == null)
                return false;
            if (selected_objects.Length > 1)
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
        /// <item><description>提供标题、宽度、样式、颜色、选项等参数。</description></item>
        /// </list>
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
                tog_mixed_bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                act_on_changed: act_on_changed);
        }
        #endregion
    }
}