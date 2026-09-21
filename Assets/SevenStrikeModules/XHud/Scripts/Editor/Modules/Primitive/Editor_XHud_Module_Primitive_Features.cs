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
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(XHud_Module_Primitive_Feature))]
    public class Editor_XHud_Module_Primitive_Features : Editor
    {
        #region 组件 / 列表
        private XHud_Module_Primitive_Feature BaseScript;
        #endregion

        #region 序列化属性
        private SerializedProperty
            sp_Debug,
            sp_PrimitiveFeatures,
            FirstSaveFeatures;
        #endregion

        #region GUI 参数
        /// <summary>
        /// 多选特性的索引
        /// </summary>
        private int MultiPrimitiveFeature_Index;
        #endregion

        #region 选项文字
        string[] stroptions_debug = new string[2] { "关闭", "调试" };
        #endregion

        #region 图标
        private Texture2D icon_main, f_pos, f_rot, f_sca, f_size, f_alp, f_fill, f_color, left_arrow_r, left_arrow_p, right_arrow_r, right_arrow_p, f_save_r, f_save_p, f_load_r, f_load_p, icon_col_mark;
        #endregion

        #region 批量化操作
        /// <summary>
        /// 批量选择脚本数组
        /// </summary>
        XHud_Module_Primitive_Feature[] SelectedObjects;
        /// <summary>
        /// 获取所有批量脚本目标
        /// </summary>
        private void Targets_Get()
        {
            if (targets.Length > 1)
            {
                SelectedObjects = new XHud_Module_Primitive_Feature[targets.Length];
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    var t = targets[i];
                    SelectedObjects[i] = (XHud_Module_Primitive_Feature)t;
                }
            }
            else
            {
                SelectedObjects = new XHud_Module_Primitive_Feature[targets.Length];
                SelectedObjects[0] = (XHud_Module_Primitive_Feature)target;
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

        private PrimitiveFeatures[] PrimitiveFeatures;

        private void OnEnable()
        {
            BaseScript = (XHud_Module_Primitive_Feature)target;

            Targets_Get();

            // 获取所有序列化字段
            GetSerializeFields();

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

            PrimitiveFeature_Capture(SelectedObjects);
        }

        private void OnDisable()
        {
            if (!Application.isPlaying)
            {
                #region 记录特性
                if (Targets_Selected())
                {
                    for (int i = 0; i < SelectedObjects.Length; i++)
                    {
                        if (SelectedObjects[i] != null)
                        {
                            if (!SelectedObjects[i].controller.pt_Tween.TweenIsPreviewing)
                            {
                                PrimitiveFeature_Save(SelectedObjects[i]);
                            }
                            else
                            {
                                PrimitiveFeature_Load(SelectedObjects[i]);
                            }
                        }
                    }
                }
                else
                {
                    if (target != null)
                    {
                        if (!BaseScript.controller.pt_Tween.TweenIsPreviewing)
                        {
                            PrimitiveFeature_Save(BaseScript);
                            serializedObject.ApplyModifiedProperties();
                        }
                        else
                        {
                            PrimitiveFeature_Load(BaseScript);
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
                bg_fill: XGUIFilled.实体,
                bg_color: XGUIColor.深空灰,
                bg_height: 30,
                icon: icon_main,
                icon_color: XHud_Dashboard.Theme_Primary,
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
               bg_fill: XGUIFilled.缺口纯色边框,
               bg_color: XGUIColor.亮白,
               bg_color_gui: XHud_Dashboard.Theme_Group,
               title: "快捷功能",
               title_size: XGUIFontSize.M,
               title_text_color: XHud_Dashboard.Theme_Primary,
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

            BaseScript.fold_param = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.Theme_Group,
                title: statu_title,
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.Theme_Primary,
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: BaseScript.fold_param);

            if (!BaseScript.fold_param)
            {
                if (!Targets_Selected())
                {
                    SerializedProperty pos = sp_PrimitiveFeatures.FindPropertyRelative("Position");
                    SerializedProperty eur = sp_PrimitiveFeatures.FindPropertyRelative("Euler");
                    SerializedProperty sca = sp_PrimitiveFeatures.FindPropertyRelative("Scale");
                    SerializedProperty size = sp_PrimitiveFeatures.FindPropertyRelative("Size");
                    SerializedProperty alp = sp_PrimitiveFeatures.FindPropertyRelative("Alpha");
                    SerializedProperty col = sp_PrimitiveFeatures.FindPropertyRelative("Color");
                    SerializedProperty fil = sp_PrimitiveFeatures.FindPropertyRelative("Fill");

                    #region 位置
                    XGUI.layout_state_displayer_text(
                        title: "位置",
                        title_size: XGUIFontSize.M,
                        subtitle: pos.vector3Value.ToString(),
                        subtitle_size: XGUIFontSize.M,
                        subtitle_color: XHud_Dashboard.Theme_Primary,
                        margin: new RectOffset(5, 5, 0, 5));
                    #endregion

                    #region 角度
                    XGUI.layout_state_displayer_text(
                       title: "角度",
                       title_size: XGUIFontSize.M,
                       subtitle: eur.vector3Value.ToString(),
                       subtitle_size: XGUIFontSize.M,
                       subtitle_color: XHud_Dashboard.Theme_Primary,
                       margin: new RectOffset(5, 5, 0, 5));
                    #endregion

                    #region 缩放
                    XGUI.layout_state_displayer_text(
                        title: "缩放",
                        title_size: XGUIFontSize.M,
                        subtitle: sca.vector3Value.ToString(),
                        subtitle_size: XGUIFontSize.M,
                        subtitle_color: XHud_Dashboard.Theme_Primary,
                        margin: new RectOffset(5, 5, 0, 5));
                    #endregion

                    #region 尺寸
                    XGUI.layout_state_displayer_text(
                       title: "尺寸",
                       title_size: XGUIFontSize.M,
                       subtitle: size.vector2Value.ToString(),
                       subtitle_size: XGUIFontSize.M,
                       subtitle_color: XHud_Dashboard.Theme_Primary,
                       margin: new RectOffset(5, 5, 0, 5));
                    #endregion

                    #region 透明度
                    XGUI.layout_state_displayer_text(
                       title: "透明度",
                       title_size: XGUIFontSize.M,
                       subtitle: alp.floatValue.ToString(),
                       subtitle_size: XGUIFontSize.M,
                       subtitle_color: XHud_Dashboard.Theme_Primary,
                       margin: new RectOffset(5, 5, 0, 5));
                    #endregion

                    #region 填充
                    XGUI.layout_state_displayer_text(
                       title: "填充",
                       title_size: XGUIFontSize.M,
                       subtitle: fil.floatValue.ToString(),
                       subtitle_size: XGUIFontSize.M,
                       subtitle_color: XHud_Dashboard.Theme_Primary,
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
                        bg_fill: XGUIFilled.透明,
                        bg_color: XGUIColor.亮白,
                        bg_color_gui: XHud_Dashboard.Theme_Group,
                        absolute_margin: true,
                        absolute_padding: true,
                        margin: new RectOffset(0, 0, 0, 0),
                        padding: new RectOffset(15, 15, 5, 10));

                    #region 标题按钮
                    if (XGUI.layout_button(
                        text: $"{SelectedObjects[MultiPrimitiveFeature_Index].name}",
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
                        EditorGUIUtility.PingObject(SelectedObjects[MultiPrimitiveFeature_Index]);
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
                        if (MultiPrimitiveFeature_Index <= 0)
                        {
                            MultiPrimitiveFeature_Index = SelectedObjects.Length - 1;
                        }
                        else
                        {
                            MultiPrimitiveFeature_Index--;
                        }
                        EditorGUIUtility.PingObject(SelectedObjects[MultiPrimitiveFeature_Index]);
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
                        if (MultiPrimitiveFeature_Index >= SelectedObjects.Length - 1)
                        {
                            MultiPrimitiveFeature_Index = 0;
                        }
                        else
                        {
                            MultiPrimitiveFeature_Index++;
                        }
                        EditorGUIUtility.PingObject(SelectedObjects[MultiPrimitiveFeature_Index]);
                    }
                    #endregion

                    XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
                    #endregion

                    XGUI.layout_seperator(
                            thickness: 1,
                            color: XHud_Dashboard.Theme_SeperateLine,
                            margin: new RectOffset(15, 15, 5, 25));

                    #region 位置
                    XGUI.layout_state_displayer_text(
                        title: "位置",
                    title_size: XGUIFontSize.M,
                        subtitle: SelectedObjects[MultiPrimitiveFeature_Index].PrimitiveFeatures.Position.ToString(),
                        subtitle_size: XGUIFontSize.M,
                        subtitle_color: XHud_Dashboard.Theme_Primary,
                        margin: new RectOffset(5, 5, 0, 5));
                    #endregion

                    #region 角度
                    XGUI.layout_state_displayer_text(
                       title: "角度",
                       title_size: XGUIFontSize.M,
                       subtitle: SelectedObjects[MultiPrimitiveFeature_Index].PrimitiveFeatures.Euler.ToString(),
                       subtitle_size: XGUIFontSize.M,
                       subtitle_color: XHud_Dashboard.Theme_Primary,
                       margin: new RectOffset(5, 5, 0, 5));
                    #endregion

                    #region 缩放
                    XGUI.layout_state_displayer_text(
                        title: "缩放",
                        title_size: XGUIFontSize.M,
                        subtitle: SelectedObjects[MultiPrimitiveFeature_Index].PrimitiveFeatures.Scale.ToString(),
                        subtitle_size: XGUIFontSize.M,
                        subtitle_color: XHud_Dashboard.Theme_Primary,
                        margin: new RectOffset(5, 5, 0, 5));
                    #endregion

                    #region 尺寸
                    XGUI.layout_state_displayer_text(
                       title: "尺寸",
                       title_size: XGUIFontSize.M,
                       subtitle: SelectedObjects[MultiPrimitiveFeature_Index].PrimitiveFeatures.Size.ToString(),
                       subtitle_size: XGUIFontSize.M,
                       subtitle_color: XHud_Dashboard.Theme_Primary,
                       margin: new RectOffset(5, 5, 0, 5));
                    #endregion

                    #region 透明度
                    XGUI.layout_state_displayer_text(
                       title: "透明度",
                       title_size: XGUIFontSize.M,
                       subtitle: SelectedObjects[MultiPrimitiveFeature_Index].PrimitiveFeatures.Alpha.ToString(),
                       subtitle_size: XGUIFontSize.M,
                       subtitle_color: XHud_Dashboard.Theme_Primary,
                       margin: new RectOffset(5, 5, 0, 5));
                    #endregion

                    #region 填充
                    XGUI.layout_state_displayer_text(
                       title: "填充",
                       title_size: XGUIFontSize.M,
                       subtitle: SelectedObjects[MultiPrimitiveFeature_Index].PrimitiveFeatures.Fill.ToString(),
                       subtitle_size: XGUIFontSize.M,
                       subtitle_color: XHud_Dashboard.Theme_Primary,
                       margin: new RectOffset(5, 5, 0, 5));
                    #endregion

                    #region 颜色
                    XGUI.layout_state_displayer_icon(
                     title: "颜色",
                     title_size: XGUIFontSize.M,
                     icon: icon_col_mark,
                     icon_size: new Vector2(14, 14),
                     icon_offset: new Vector2(0, 0),
                     icon_color: SelectedObjects[MultiPrimitiveFeature_Index].PrimitiveFeatures.Color,
                     padding: new RectOffset(0, 0, 0, 0),
                     margin: new RectOffset(5, 5, 0, 5));
                    #endregion
                }
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

            #region 右键菜单
            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                // 创建右键菜单
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

        /// <summary>
        /// 获取所有序列化字段
        /// </summary>
        private void GetSerializeFields()
        {
            sp_PrimitiveFeatures = serializedObject.FindProperty("PrimitiveFeatures");
            sp_Debug = serializedObject.FindProperty("Debug");
            FirstSaveFeatures = serializedObject.FindProperty("FirstSaveFeatures");
        }

        /// <summary>
        /// 检测图元特性首次初始状态
        /// </summary>
        private void CheckFirstCreatedFeature()
        {
            if (Targets_Selected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    XHud_Module_Primitive_Feature feature = SelectedObjects[i];
                    SerializedObject so = new SerializedObject(feature);
                    so.Update();
                    SerializedProperty prop_FirstSaveFeatures = so.FindProperty("FirstSaveFeatures");
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
                if (!FirstSaveFeatures.boolValue)
                {
                    FirstSaveFeatures.boolValue = true;
                    serializedObject.ApplyModifiedProperties();

                    // 首次记录姿态信息
                    PrimitiveFeature_Save(BaseScript);
                }
            }
        }

        #region 特性参数
        /// <summary>
        /// 记录 PrimitiveFeature 特性参数
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
                SerializedProperty feature = so.FindProperty("PrimitiveFeatures");
                // 获取特性类的字段
                SerializedProperty pos = feature.FindPropertyRelative("Position");
                SerializedProperty eur = feature.FindPropertyRelative("Euler");
                SerializedProperty sca = feature.FindPropertyRelative("Scale");
                SerializedProperty size = feature.FindPropertyRelative("Size");
                SerializedProperty alp = feature.FindPropertyRelative("Alpha");
                SerializedProperty col = feature.FindPropertyRelative("Color");
                SerializedProperty fil = feature.FindPropertyRelative("Fill");

                // 保存特性：Rect
                if (features.controller.mod_Rect != null)
                {
                    pos.vector3Value = features.controller.mod_Rect.anchoredPosition3D;
                    eur.vector3Value = features.controller.mod_Rect.localEulerAngles;
                    sca.vector3Value = features.controller.mod_Rect.localScale;
                    size.vector2Value = features.controller.mod_Rect.sizeDelta;
                }
                // 保存特性：CanvasGroup
                if (features.controller.mod_CanvasGroup != null)
                {
                    alp.floatValue = features.controller.mod_CanvasGroup.alpha;
                }
                // 保存特性：Image
                if (features.controller.mod_Image != null)
                {
                    col.colorValue = features.controller.mod_Image.color;
                    fil.floatValue = features.controller.mod_Image.fillAmount;
                }
                // 保存特性：Text
                else if (features.controller.mod_Text != null)
                {
                    col.colorValue = features.controller.mod_Text.color;
                }
                // 保存特性：TmpText
                else if (features.controller.mod_TmpText != null)
                {
                    col.colorValue = features.controller.mod_TmpText.color;
                }
                // 保存特性：_RawImage
                else if (features.controller.mod_RawImage != null)
                {
                    col.colorValue = features.controller.mod_RawImage.color;
                }

                so.ApplyModifiedProperties();
            }
        }
        /// <summary>
        /// 还原 PrimitiveFeature 特性参数
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
                SerializedProperty feature = so.FindProperty("PrimitiveFeatures");
                // 获取特性类的字段
                SerializedProperty pos = feature.FindPropertyRelative("Position");
                SerializedProperty eur = feature.FindPropertyRelative("Euler");
                SerializedProperty sca = feature.FindPropertyRelative("Scale");
                SerializedProperty size = feature.FindPropertyRelative("Size");
                SerializedProperty alp = feature.FindPropertyRelative("Alpha");
                SerializedProperty col = feature.FindPropertyRelative("Color");
                SerializedProperty fil = feature.FindPropertyRelative("Fill");

                // 读取特性：Rect
                if (features.controller.mod_Rect != null)
                {
                    features.controller.mod_Rect.anchoredPosition3D = pos.vector3Value;
                    features.controller.mod_Rect.localEulerAngles = eur.vector3Value;
                    features.controller.mod_Rect.localScale = sca.vector3Value;
                    features.controller.mod_Rect.sizeDelta = size.vector2Value;
                }
                // 读取特性：CanvasGroup
                if (features.controller.mod_CanvasGroup != null)
                {
                    features.controller.mod_CanvasGroup.alpha = alp.floatValue;
                }
                // 读取特性：Image
                if (features.controller.mod_Image != null)
                {
                    features.controller.pt_Painting.UpdateColor(col.colorValue);
                    features.controller.mod_Image.color = col.colorValue;
                    features.controller.mod_Image.fillAmount = fil.floatValue;
                }
                // 读取特性：Text
                else if (features.controller.mod_Text != null)
                {
                    features.controller.mod_Text.color = col.colorValue;
                }
                // 读取特性：TmpText
                else if (features.controller.mod_TmpText != null)
                {
                    features.controller.mod_TmpText.color = col.colorValue;
                }
                // 读取特性：RawImage
                else if (features.controller.mod_RawImage != null)
                {
                    features.controller.pt_Painting.UpdateColor(col.colorValue);
                    features.controller.mod_RawImage.color = col.colorValue;
                }

                so.ApplyModifiedProperties();
            }
        }
        /// <summary>
        /// 采集 PrimitiveFeature 特性参数
        /// </summary>
        private void PrimitiveFeature_Capture(XHud_Module_Primitive_Feature[] features)
        {
            PrimitiveFeatures = new PrimitiveFeatures[features.Length];
            for (int i = 0; i < features.Length; i++)
            {
                PrimitiveFeatures[i] = features[i].PrimitiveFeatures;
            }
        }
        /// <summary>
        /// 批处理特性保存（含确认弹窗）
        /// </summary>
        private void PrimitiveFeature_Dual_Save_WithDialog()
        {
            // 多选时
            if (Targets_Selected())
            {
                List<XHud_GUI_Dialog_ListDatas> Datas = new List<XHud_GUI_Dialog_ListDatas>();

                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    PrimitiveFeature_Save(SelectedObjects[i]);
                    XHud_GUI_Dialog_ListDatas data = new XHud_GUI_Dialog_ListDatas();

                    data.Title = string.IsNullOrEmpty(SelectedObjects[i].controller.Indicator) ? SelectedObjects[i].name : SelectedObjects[i].controller.Indicator;
                    data.SubTitle = "";
                    data.Message = "已记录特性";
                    Datas.Add(data);
                }
                EditorApplication.delayCall += () =>
                {
                    Editor_XHud_GUI.Open(Datas.ToArray(), XHud_DialogType.确认, "XHud - 图元特性消息", "批量记录图元特性", "以下是批量已记录特性的所有图元列表，请您检查核对：", "明白");
                };
            }
            // 单选时
            else
            {
                PrimitiveFeature_Save(BaseScript);
                EditorApplication.delayCall += () =>
                {
                    Editor_XHud_GUI.Open(XHud_DialogType.确认, "XHud - 图元特性消息", "图元特性记录", "已记录图元的特性参数！", "明白");
                };
            }
        }
        /// <summary>
        /// 批处理特性读取（含确认弹窗）
        /// </summary>
        private void PrimitiveFeature_Dual_Load_WithDialog()
        {
            // 多选时
            if (Targets_Selected())
            {
                List<XHud_GUI_Dialog_ListDatas> Datas = new List<XHud_GUI_Dialog_ListDatas>();

                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    PrimitiveFeature_Load(SelectedObjects[i]);
                    XHud_GUI_Dialog_ListDatas data = new XHud_GUI_Dialog_ListDatas();

                    data.Title = string.IsNullOrEmpty(SelectedObjects[i].controller.Indicator) ? SelectedObjects[i].name : SelectedObjects[i].controller.Indicator;
                    data.SubTitle = "";
                    data.Message = "已读取特性";
                    Datas.Add(data);
                }

                EditorApplication.delayCall += () =>
                {
                    Editor_XHud_GUI.Open(Datas.ToArray(), XHud_DialogType.确认, "XHud - 图元特性消息", "批量读取图元特性", "以下是批量已读取特性的所有图元列表，请您检查核对：", "明白");
                };
            }
            // 单选时
            else
            {
                PrimitiveFeature_Load(BaseScript);
                EditorApplication.delayCall += () =>
                {
                    Editor_XHud_GUI.Open(XHud_DialogType.确认, "XHud - 图元特性消息", "图元特性读取", "已读取图元的特性参数！", "明白");
                };
            }
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
