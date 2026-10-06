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

    public partial class Editor_XHud_Module_Primitive_Tween_Tracker
    {
        /// <summary>
        /// 绘制动画数值过渡模式的编辑器主体（左/右数值输入框 + 交换按钮）
        /// </summary>
        private void DrawTweenValueEditor(TweenValueProperties val_propertys, int value_mode_index)
        {
            bool only_end_mode = (value_mode_index == 3);

            if (!only_end_mode)
                XGUI.layout_group_start(
                    type: XGUIContainerType.Horizontal,
                    absolute_margin: true,
                    absolute_padding: true,
                    margin: new RectOffset(0, 0, 0, 0),
                    padding: new RectOffset(0, 0, 0, 0),
                    can_foldout: false);

            if (!only_end_mode)
                XGUI.layout_space(5);

            // 数值流向指示器图表式按钮
            if (!only_end_mode)
                if (XGUI.layout_button(
                tooltip: "点击交换数值",
                tex_release: icon_track_param_connector_status_r,
                tex_press: icon_track_param_connector_status_p,
                tex_gui_color: XHud_Dashboard.Theme_Primary,
                margin: new RectOffset(0, 10, 10, 0),
                border: new RectOffset(0, 0, 3, 8),
                width: icon_track_param_connector_status_r.width,
                height: 92))
                {
                    // 根据模式进行数值交换对调
                    switch (value_mode_index)
                    {
                        case 0:
                            SwapValueProperties(val_propertys.prop_from, val_propertys.prop_origin);
                            break;
                        case 1:
                            SwapValueProperties(val_propertys.prop_origin, val_propertys.prop_end);
                            break;
                        case 2:
                            SwapValueProperties(val_propertys.prop_from, val_propertys.prop_end);
                            break;
                    }
                }

            XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                absolute_margin: true,
                absolute_padding: true,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                can_foldout: false);

            /* value_mode_index = 动画数值过渡模式
             *  0  => 起始 - 默认
             *  1  => 默认 - 结束
             *  2  => 起始 - 结束
             *  3  => 当前 - 结束
             * */

            string title_source = null;
            string title_target = null;

            SerializedProperty prop_source = null;
            SerializedProperty prop_target = null;

            switch (value_mode_index)
            {
                case 0:
                    // 起始 - 默认
                    title_source = "起始";
                    title_target = "默认";
                    prop_source = val_propertys.prop_from;
                    prop_target = val_propertys.prop_origin;
                    break;
                case 1:
                    // 默认 - 结束
                    title_source = "默认";
                    title_target = "结束";
                    prop_source = val_propertys.prop_origin;
                    prop_target = val_propertys.prop_end;
                    break;
                case 2:
                    // 起始 - 结束
                    title_source = "起始";
                    title_target = "结束";
                    prop_source = val_propertys.prop_from;
                    prop_target = val_propertys.prop_end;
                    break;
                case 3:
                    // 当前 - 结束
                    title_source = "结束";
                    prop_source = val_propertys.prop_end;
                    break;
            }

            // 控件绘制 - 起源值
            DrawTweenValueFields(
                title_source,
                prop_source,
                // 按钮动作 - 将目标物体的类型属性数据记录到序列化属性中
                () => { RecordValueFromTarget(prop_source, val_propertys.type); },
                // 按钮动作 - 将序列化属性的值应用到目标物体的类型属性数据上
                () => { ApplyValueToTarget(prop_source, val_propertys.type); },
                // 按钮动作 - 重置序列化属性数据
                () => { ResetValueProperty(prop_source); });

            // 如果 value_mode_index != 3 的时候说明动画都是从“起源值”到“目标值”，所以才会出现这第二个输入框
            if (!only_end_mode)
            {
                XGUI.layout_seperator(
                    thickness: 1,
                    color: XHud_Dashboard.Theme_SeperateLine,
                    margin: new RectOffset(15, 15, 8, 12));

                // 控件绘制 - 目标值
                DrawTweenValueFields(
                    title_target,
                    prop_target,
                    // 按钮动作 - 将目标物体的类型属性数据记录到序列化属性中
                    () => { RecordValueFromTarget(prop_target, val_propertys.type); },
                    // 按钮动作 - 将序列化属性的值应用到目标物体的类型属性数据上
                    () => { ApplyValueToTarget(prop_target, val_propertys.type); },
                    // 按钮动作 - 重置序列化属性数据
                    () => { ResetValueProperty(prop_target); });
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);

            if (!only_end_mode)
                XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
        }
        /// <summary>
        /// 绘制单个数值字段（输入框 + 记录 / 应用 / 归零三个操作按钮）
        /// </summary>
        private void DrawTweenValueFields(string title, SerializedProperty prop, Action act_on_pressed_record = null, Action act_on_pressed_apply = null, Action act_on_pressed_reset = null)
        {
            #region 数值输入框
            XGUI.layout_property_field(
                       title: title,
                       title_size: XGUIFontSize.M,
                       title_hover_color: XHud_Dashboard.Theme_Primary,
                       title_width: 50,
                       title_color: XHud_Dashboard.Theme_Primary,
                       title_offset: new Vector2(0, 0),
                       title_font_style: FontStyle.Normal,
                       prop: prop,
                       prop_padding: new RectOffset(5, 5, 0, 0),
                       prop_margin: new RectOffset(0, 0, 0, 0));
            prop.serializedObject.ApplyModifiedProperties();

            #region 按钮操作区域
            XGUI.layout_group_start(
            type: XGUIContainerType.Horizontal,
            absolute_margin: true,
            absolute_padding: true,
            margin: new RectOffset(0, 0, 10, 0),
            padding: new RectOffset(10, 10, 0, 0));

            #region 记录当前物体
            if (XGUI.layout_button(
                tooltip: "记录当前物体",
                tex_release: icon_track_param_record_r,
                tex_press: icon_track_param_record_p,
                tex_gui_color: Color.white,
                margin: new RectOffset(10, 10, 0, 0),
                border: new RectOffset(0, 0, 0, 0),
                width: 40,
                height: 40))
            {
                if (act_on_pressed_record != null)
                    act_on_pressed_record();
            }
            #endregion

            XGUI.layout_flexspace();

            #region 应用到物体
            if (XGUI.layout_button(
                tooltip: "应用到物体",
                tex_release: icon_track_param_apply_r,
                tex_press: icon_track_param_apply_p,
                tex_gui_color: Color.white,
                margin: new RectOffset(10, 10, 0, 0),
                border: new RectOffset(0, 0, 0, 0),
                width: 40,
                height: 40))
            {
                if (act_on_pressed_apply != null)
                    act_on_pressed_apply();
            }
            #endregion

            XGUI.layout_flexspace();

            #region 当前物体对应类型值归零
            if (XGUI.layout_button(
                tooltip: "当前物体对应类型值归零",
                tex_release: icon_track_param_reset_r,
                tex_press: icon_track_param_reset_p,
                tex_gui_color: Color.white,
                margin: new RectOffset(10, 10, 0, 0),
                border: new RectOffset(0, 0, 0, 0),
                width: 40,
                height: 40))
            {
                if (act_on_pressed_reset != null)
                    act_on_pressed_reset();
            }
            #endregion


            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            #endregion
            #endregion
        }
        /// <summary>
        /// 根据动画类型解析出其对应的「起始 / 结束 / 默认」三组序列化属性
        /// </summary>
        private TweenValueProperties ResolveTweenValueProperties(TweenNodeType type, SerializedProperty prop)
        {
            #region 数值类型序列化获取- Vector4
            SerializedProperty ori_v4 = prop.FindPropertyRelative("Original_Vector4");
            SerializedProperty from_v4 = prop.FindPropertyRelative("From_Vector4");
            SerializedProperty end_v4 = prop.FindPropertyRelative("End_Vector4");
            #endregion

            #region 数值类型序列化获取- Vector3
            SerializedProperty ori_v3 = prop.FindPropertyRelative("Original_Vector3");
            SerializedProperty from_v3 = prop.FindPropertyRelative("From_Vector3");
            SerializedProperty end_v3 = prop.FindPropertyRelative("End_Vector3");
            #endregion

            #region 数值类型序列化获取- Vector2
            SerializedProperty ori_v2 = prop.FindPropertyRelative("Original_Vector2");
            SerializedProperty from_v2 = prop.FindPropertyRelative("From_Vector2");
            SerializedProperty end_v2 = prop.FindPropertyRelative("End_Vector2");
            #endregion

            #region 数值类型序列化获取- Color
            SerializedProperty ori_color = prop.FindPropertyRelative("Original_Color");
            SerializedProperty from_color = prop.FindPropertyRelative("From_Color");
            SerializedProperty end_color = prop.FindPropertyRelative("End_Color");
            #endregion

            #region 数值类型序列化获取- String
            SerializedProperty ori_string = prop.FindPropertyRelative("Original_String");
            SerializedProperty from_string = prop.FindPropertyRelative("From_String");
            SerializedProperty end_string = prop.FindPropertyRelative("End_String");
            #endregion

            #region 数值类型序列化获取- Float
            SerializedProperty ori_float = prop.FindPropertyRelative("Original_Float");
            SerializedProperty from_float = prop.FindPropertyRelative("From_Float");
            SerializedProperty end_float = prop.FindPropertyRelative("End_Float");
            #endregion

            #region 数值类型序列化获取- Int
            SerializedProperty ori_int = prop.FindPropertyRelative("Original_Int");
            SerializedProperty from_int = prop.FindPropertyRelative("From_Int");
            SerializedProperty end_int = prop.FindPropertyRelative("End_Int");
            #endregion

            TweenValueProperties value_propertys = new TweenValueProperties();

            // 第 1 步：统一设置 title（去掉枚举名前两个字符 "x_"）
            value_propertys.title = type.ToString().Substring(2);

            // 第 2 步：按数据类型分组设置值
            switch (type)
            {
                case TweenNodeType.a_位移:
                case TweenNodeType.r_旋转:
                case TweenNodeType.s_缩放:
                    value_propertys.prop_origin = ori_v3;
                    value_propertys.prop_from = from_v3;
                    value_propertys.prop_end = end_v3;
                    break;
                case TweenNodeType.c_颜色:
                    value_propertys.prop_origin = ori_color;
                    value_propertys.prop_from = from_color;
                    value_propertys.prop_end = end_color;
                    break;
                case TweenNodeType.g_淡化:
                case TweenNodeType.f_图像填充:
                    value_propertys.prop_origin = ori_float;
                    value_propertys.prop_from = from_float;
                    value_propertys.prop_end = end_float;
                    break;
                case TweenNodeType.w_打字机:
                    value_propertys.prop_origin = ori_string;
                    value_propertys.prop_from = from_string;
                    value_propertys.prop_end = end_string;
                    break;
                case TweenNodeType.z_尺寸:
                    value_propertys.prop_origin = ori_v2;
                    value_propertys.prop_from = from_v2;
                    value_propertys.prop_end = end_v2;
                    break;
            }

            value_propertys.type = type;

            // 返回的类中包含了目标类型动画的序列化数据（引用）
            return value_propertys;
        }
        /// <summary>
        /// 从目标物体的当前属性读值并写入指定序列化属性
        /// <para/>
        /// 使用方式：参数面板中「记录当前物体」按钮点击时调用。
        /// 与 <see cref="ApplyValueToTarget"/> 互为逆操作：
        /// <list type="bullet">
        /// <item><description><c>RecordValueFromTarget</c>：目标物体 → 序列化属性（本方法）；</description></item>
        /// <item><description><c>ApplyValueToTarget</c>：序列化属性 → 目标物体。</description></item>
        /// </list>
        /// 本方法只读取运行时组件的当前值，不做 Undo 记录（读取不修改任何对象）。
        /// </summary>
        /// <param name="p">目标序列化属性，按 <paramref name="node_type"/> 决定写入的类型分支</param>
        /// <param name="node_type">动画节点类型，决定从哪个组件 / 字段读取当前值</param>
        private void RecordValueFromTarget(SerializedProperty p, TweenNodeType node_type)
        {
            switch (node_type)
            {
                // ── 位移：读取 RectTransform.anchoredPosition3D ──
                case TweenNodeType.a_位移:
                    p.vector3Value = target.controller.mod_Rect.anchoredPosition3D;
                    break;

                // ── 旋转：读取 RectTransform.localEulerAngles（欧拉角）──
                case TweenNodeType.r_旋转:
                    p.vector3Value = target.controller.mod_Rect.localEulerAngles;
                    break;

                // ── 缩放：读取 RectTransform.localScale ──
                case TweenNodeType.s_缩放:
                    p.vector3Value = target.controller.mod_Rect.localScale;
                    break;

                // ── 颜色：读取 Graphic.color ──
                case TweenNodeType.c_颜色:
                    // 通过 Controller 识别目标实际挂载的图形类型（Image / RawImage / Text 等），
                    // RecognizeType() 返回对应的 Graphic 基类实例，供统一取色。
                    Graphic gc = target.controller.RecognizeType();

                    p.colorValue = gc.color;
                    break;

                // ── 淡化：读取 CanvasGroup.alpha ──
                case TweenNodeType.g_淡化:
                    p.floatValue = target.controller.mod_CanvasGroup.alpha;
                    break;

                // ── 打字机：读取 Text / TmpText 的内容 ──
                // 注意：Text 与 TmpText 互斥，优先 Text，回退 TmpText；
                // 两者都为 null 时保持属性原值不变。
                case TweenNodeType.w_打字机:
                    if (target.controller.mod_Text != null)
                    {
                        p.stringValue = target.controller.mod_Text.text;
                    }
                    else if (target.controller.mod_TmpText != null)
                    {
                        p.stringValue = target.controller.mod_TmpText.text;
                    }
                    break;

                // ── 图像填充：读取 Image.fillAmount ──
                case TweenNodeType.f_图像填充:
                    p.floatValue = target.controller.mod_Image.fillAmount;
                    break;

                // ── 尺寸：读取 RectTransform.sizeDelta ──
                case TweenNodeType.z_尺寸:
                    p.vector2Value = target.controller.mod_Rect.sizeDelta;
                    break;
            }

            // 将序列化属性的改动立即写回 SerializedObject，
            // 否则参数面板上的输入框不会同步刷新。
            p.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
        /// <summary>
        /// 将指定序列化属性的值写回到目标物体的对应属性上
        /// <para/>
        /// 使用方式：参数面板中「应用到物体」按钮点击时调用。
        /// 每个分支都会先用 <see cref="Undo.RecordObject"/> 记录被修改对象，
        /// 以支持 Ctrl+Z 撤销；同时按 <paramref name="node_type"/> 决定操作目标。
        /// </summary>
        /// <param name="p">来源序列化属性（已由 <see cref="RecordValueFromTarget"/> 或手动编辑填入值）</param>
        /// <param name="node_type">动画节点类型，决定值应写回哪个组件 / 字段</param>
        private void ApplyValueToTarget(SerializedProperty p, TweenNodeType node_type)
        {
            switch (node_type)
            {
                // ── 位移：写入 RectTransform.anchoredPosition3D ──
                case TweenNodeType.a_位移:
                    if (!IsPreviewing)
                        Undo.RecordObject(target.controller.mod_Rect, "undotransform-position");
                    target.controller.mod_Rect.anchoredPosition3D = p.vector3Value;
                    break;

                // ── 旋转：写入 RectTransform.localEulerAngles（欧拉角）──
                case TweenNodeType.r_旋转:
                    if (!IsPreviewing)
                        Undo.RecordObject(target.controller.mod_Rect, "undotransform-eulerangle");
                    target.controller.mod_Rect.localEulerAngles = p.vector3Value;
                    break;

                // ── 缩放：写入 RectTransform.localScale ──
                case TweenNodeType.s_缩放:
                    if (!IsPreviewing)
                        Undo.RecordObject(target.controller.mod_Rect, "undotransform-localscale");
                    target.controller.mod_Rect.localScale = p.vector3Value;
                    break;

                // ── 颜色：写入 Graphic.color，并同步刷新 Control 上缓存的 OriginalColor ──
                case TweenNodeType.c_颜色:
                    // 通过 Controller 识别目标实际挂载的图形类型（Image / RawImage / Text 等），
                    // RecognizeType() 会返回对应的 Graphic 基类实例，供统一写色。
                    Graphic gc = target.controller.RecognizeType();

                    // 同步记录 Control 内部的 OriginalColor 缓存，避免下次动画运行时
                    // 用旧的 OriginalColor 覆盖用户刚刚写入的颜色。
                    if (!IsPreviewing)
                        Undo.RecordObject(target.controller.pt_Painting, "undocolor-origin");

                    ModuleType x_Type = target.controller.GetModuleType();
                    if (x_Type == ModuleType.Image)
                        target.controller.pt_Painting.OriginalColor = p.colorValue;
                    else if (x_Type == ModuleType.RawImage)
                        target.controller.pt_Painting.OriginalColor = p.colorValue;

                    gc.color = p.colorValue;
                    break;

                // ── 淡化：写入 CanvasGroup.alpha ──
                case TweenNodeType.g_淡化:
                    if (!IsPreviewing)
                        Undo.RecordObject(target.controller.mod_CanvasGroup, "undoAlpha");
                    target.controller.mod_CanvasGroup.alpha = p.floatValue;
                    break;

                // ── 打字机：写入 Text / TmpText 的内容 ──
                // 注意：Text 与 TmpText 互斥，优先 Text，回退 TmpText。
                case TweenNodeType.w_打字机:
                    if (target.controller.mod_Text != null)
                    {
                        if (!IsPreviewing)
                            Undo.RecordObject(target.controller.mod_Text, "undoText");
                        target.controller.mod_Text.txt_Set_Content(p.stringValue);
                    }
                    else if (target.controller.mod_TmpText != null)
                    {
                        if (!IsPreviewing)
                            Undo.RecordObject(target.controller.mod_TmpText, "undoTmpText");
                        target.controller.mod_TmpText.tmp_Set_Content(p.stringValue);
                    }
                    // 文本变更需要刷新编辑器窗口与场景视图，否则预览不会立即更新。
                    Repaint();
                    SceneView.RepaintAll();
                    break;

                // ── 图像填充：写入 Image.fillAmount ──
                case TweenNodeType.f_图像填充:
                    if (!IsPreviewing)
                        Undo.RecordObject(target.controller.mod_Image, "undofill");
                    target.controller.mod_Image.fillAmount = p.floatValue;
                    break;

                // ── 尺寸：写入 RectTransform.sizeDelta ──
                case TweenNodeType.z_尺寸:
                    if (!IsPreviewing)
                        Undo.RecordObject(target.controller.mod_Rect, "undotransform-size");
                    target.controller.mod_Rect.sizeDelta = p.vector2Value;
                    break;
            }
        }
        /// <summary>
        /// 按序列化属性类型将其值重置为「零值」（Vector 零 / Color 透明 / 字符串空 / 数值 0）
        /// </summary>
        private void ResetValueProperty(SerializedProperty prop)
        {
            //Debug.Log($"{prop.propertyType}");

            // 根据类型交换值
            switch (prop.propertyType)
            {
                case SerializedPropertyType.Vector4:
                    prop.vector4Value = Vector4.zero;
                    break;
                case SerializedPropertyType.Vector3:
                    prop.vector3Value = Vector3.zero;
                    break;
                case SerializedPropertyType.Vector2:
                    prop.vector2Value = Vector2.zero;
                    break;
                case SerializedPropertyType.Color:
                    prop.colorValue = Color.clear;
                    break;
                case SerializedPropertyType.String:
                    prop.stringValue = null;
                    break;
                case SerializedPropertyType.Float:
                    prop.floatValue = 0f;
                    break;
                case SerializedPropertyType.Integer:
                    prop.intValue = 0;
                    break;
            }
        }
        /// <summary>
        /// 交换两个同类型序列化属性的值
        /// <para/>
        /// 使用方式：参数面板「数值」分组中，点击数值流向指示器按钮时调用。
        /// 按当前 <c>value_mode_index</c> 决定交换哪两个属性：
        /// <list type="bullet">
        /// <item><description>0 (S-D)：交换 <c>From</c> ↔ <c>Origin</c>；</description></item>
        /// <item><description>1 (D-E)：交换 <c>Origin</c> ↔ <c>End</c>；</description></item>
        /// <item><description>2 (S-E)：交换 <c>From</c> ↔ <c>End</c>。</description></item>
        /// </list>
        /// 本方法只操作 <see cref="SerializedProperty"/>，不直接修改场景对象；
        /// 交换结果由调用方后续的 <c>ApplyModifiedProperties()</c> 落地。
        /// <para/>
        /// 前置条件：两个属性的 <see cref="SerializedProperty.propertyType"/> 必须一致，
        /// 否则直接返回不做任何修改（例如 Vector3 ↔ Color 的误用会被安全拦截）。
        /// </summary>
        /// <param name="prop_primary">主属性（交换后获得 <paramref name="prop_secondary"/> 的原值）</param>
        /// <param name="prop_secondary">次属性（交换后获得 <paramref name="prop_primary"/> 的原值）</param>
        private void SwapValueProperties(SerializedProperty prop_primary, SerializedProperty prop_secondary)
        {
            // 类型不一致时不做任何交换，避免把 Vector3 写进 Color 等类型错配。
            if (prop_primary.propertyType != prop_secondary.propertyType)
                return;

            // 根据类型交换值
            switch (prop_primary.propertyType)
            {
                // ── Vector4：先暂存 secondary，再依次对调 ──
                case SerializedPropertyType.Vector4:
                    Vector4 oriV4 = prop_secondary.vector4Value;
                    prop_secondary.vector4Value = prop_primary.vector4Value;
                    prop_primary.vector4Value = oriV4;
                    break;

                // ── Vector3：位移 / 旋转 / 缩放使用 ──
                case SerializedPropertyType.Vector3:
                    Vector3 oriV3 = prop_secondary.vector3Value;
                    prop_secondary.vector3Value = prop_primary.vector3Value;
                    prop_primary.vector3Value = oriV3;
                    break;

                // ── Vector2：尺寸使用 ──
                case SerializedPropertyType.Vector2:
                    Vector2 oriV2 = prop_secondary.vector2Value;
                    prop_secondary.vector2Value = prop_primary.vector2Value;
                    prop_primary.vector2Value = oriV2;
                    break;

                // ── Color：颜色使用 ──
                case SerializedPropertyType.Color:
                    Color oriColor = prop_secondary.colorValue;
                    prop_secondary.colorValue = prop_primary.colorValue;
                    prop_primary.colorValue = oriColor;
                    break;

                // ── String：打字机文本使用 ──
                case SerializedPropertyType.String:
                    string oriStr = prop_secondary.stringValue;
                    prop_secondary.stringValue = prop_primary.stringValue;
                    prop_primary.stringValue = oriStr;
                    break;

                // ── Float：淡化 / 图像填充使用 ──
                case SerializedPropertyType.Float:
                    float oriFloat = prop_secondary.floatValue;
                    prop_secondary.floatValue = prop_primary.floatValue;
                    prop_primary.floatValue = oriFloat;
                    break;

                // ── Integer：保留分支，当前 TweenNodeType 尚未用到 ──
                case SerializedPropertyType.Integer:
                    int oriInt = prop_secondary.intValue;
                    prop_secondary.intValue = prop_primary.intValue;
                    prop_primary.intValue = oriInt;
                    break;
            }
        }
    }
}
