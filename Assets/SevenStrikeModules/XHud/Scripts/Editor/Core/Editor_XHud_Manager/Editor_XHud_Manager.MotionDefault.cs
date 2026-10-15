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
    using UnityEditor;
    using UnityEngine;

    public partial class Editor_XHud_Manager : Editor
    {
        #region 绘制 - 动效默认参数
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
            string hexcol = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary, true);

            #region 生成
            #region 模版库 - 生成
            if (BaseScript.Hud_Motions != null)
            {
                //确保动效库不是空的
                if (BaseScript.Hud_Motions.ElementMotionList != null && BaseScript.Hud_Motions.ElementMotionList.Count > 0)
                {
                    //动效列表
                    string[] motnames = BaseScript.Hud_Motions.ElementMotion_GetAllName_With_Create();
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
                        opt_bg_fill: XGUIGroup.实体,
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

                            Motion_Creator crc = BaseScript.Hud_Motions.ElementMotion_GetElementCreator_At_Create(Crc_Lib_Name.stringValue);

                            CreateArgs_Default.FindPropertyRelative("anchor").enumValueIndex = (int)crc.anchor;
                            CreateArgs_Default.FindPropertyRelative("Movement.Movement").enumValueIndex = (int)crc.Movement.Movement;
                            CreateArgs_Default.FindPropertyRelative("Movement.Distance").floatValue = crc.Movement.Distance;
                            CreateArgs_Default.FindPropertyRelative("Movement.Duration").floatValue = crc.Movement.Duration;
                            CreateArgs_Default.FindPropertyRelative("Movement.Delay").floatValue = crc.Movement.Delay;
                            CreateArgs_Default.FindPropertyRelative("Movement.Curve").animationCurveValue = crc.Movement.Curve;
                            CreateArgs_Default.FindPropertyRelative("Movement.CurveName").stringValue = crc.Movement.CurveName;
                            CreateArgs_Default.FindPropertyRelative("Movement.Ease").enumValueIndex = (int)crc.Movement.Ease;
                            CreateArgs_Default.FindPropertyRelative("Rotation.Rotation").enumValueIndex = (int)crc.Rotation.Rotation;
                            CreateArgs_Default.FindPropertyRelative("Rotation.Degree").floatValue = crc.Rotation.Degree;
                            CreateArgs_Default.FindPropertyRelative("Rotation.Duration").floatValue = crc.Rotation.Duration;
                            CreateArgs_Default.FindPropertyRelative("Rotation.Delay").floatValue = crc.Rotation.Delay;
                            CreateArgs_Default.FindPropertyRelative("Rotation.Curve").animationCurveValue = crc.Rotation.Curve;
                            CreateArgs_Default.FindPropertyRelative("Rotation.CurveName").stringValue = crc.Rotation.CurveName;
                            CreateArgs_Default.FindPropertyRelative("Rotation.Ease").enumValueIndex = (int)crc.Rotation.Ease;
                            CreateArgs_Default.FindPropertyRelative("Alpha.Duration").floatValue = crc.Alpha.Duration;
                            CreateArgs_Default.FindPropertyRelative("Alpha.Delay").floatValue = crc.Alpha.Delay;
                            CreateArgs_Default.FindPropertyRelative("Alpha.Curve").animationCurveValue = crc.Alpha.Curve;
                            CreateArgs_Default.FindPropertyRelative("Alpha.CurveName").stringValue = crc.Alpha.CurveName;
                            CreateArgs_Default.FindPropertyRelative("Alpha.Ease").enumValueIndex = (int)crc.Alpha.Ease;
                            CreateArgs_Default.FindPropertyRelative("MotionAnimateEndState").enumValueIndex = (int)crc.MotionAnimateEndState;
                            CreateArgs_Default.serializedObject.ApplyModifiedProperties();
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
                        xHud_OpenParameterSetter(HudElementMotionType.Creator);
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
                        if (!BaseScript.Hud_Motions.ElementMotion_IsExist(Crc_Lib_Name.stringValue))
                            return;
                        Editor_XHud_MenuItemsAction_OpenLibrary.open_elementmotion();
                        BaseScript.Hud_Motions.ElementMotionLibrary_Location(Crc_Lib_Name.stringValue);
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
                            windowtitle: "XHud - 管理器消息",
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
            SerializedProperty sp_def_create_anchor = CreateArgs_Default.FindPropertyRelative("anchor");

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
                SerializedProperty sp_def_create_move_type = CreateArgs_Default.FindPropertyRelative("Movement.Movement");
                XGUI.layout_property_field(
                    title: "方式",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: sp_def_create_move_type,
                    prop_margin: new RectOffset(0, 0, 5, 0));

                XGUI.layout_space(5);

                SerializedProperty sp_def_create_move_dis = CreateArgs_Default.FindPropertyRelative("Movement.Distance");
                XGUI.layout_property_field(
                    title: "距离",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: sp_def_create_move_dis,
                    prop_margin: new RectOffset(0, 0, 5, 0));

                XGUI.layout_space(5);

                SerializedProperty sp_def_create_move_dur = CreateArgs_Default.FindPropertyRelative("Movement.Duration");
                XGUI.layout_property_field(
                    title: "耗时",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: sp_def_create_move_dur,
                    prop_margin: new RectOffset(0, 0, 5, 0));

                XGUI.layout_space(5);

                SerializedProperty sp_def_create_move_delay = CreateArgs_Default.FindPropertyRelative("Movement.Delay");
                XGUI.layout_property_field(
                    title: "延迟",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: sp_def_create_move_delay,
                    prop_margin: new RectOffset(0, 0, 5, 0));

                XGUI.layout_space(5);

                SerializedProperty sp_def_create_move_curve = CreateArgs_Default.FindPropertyRelative("Movement.Curve");
                XGUI.layout_property_field(
                    title: "曲线",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: sp_def_create_move_curve,
                    prop_margin: new RectOffset(0, 0, 5, 0));

                XGUI.layout_space(5);

                SerializedProperty sp_def_create_move_ease = CreateArgs_Default.FindPropertyRelative("Movement.Ease");
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
                SerializedProperty sp_def_create_rot_type = CreateArgs_Default.FindPropertyRelative("Rotation.Rotation");
                XGUI.layout_property_field(
                  title: "方式",
                  title_size: XGUIFontSize.M,
                  title_hover_color: XHud_Dashboard.Theme_Primary,
                  title_width: 90,
                  prop: sp_def_create_rot_type,
                  prop_margin: new RectOffset(0, 0, 5, 0));

                XGUI.layout_space(5);

                SerializedProperty sp_def_create_rot_deg = CreateArgs_Default.FindPropertyRelative("Rotation.Degree");
                XGUI.layout_property_field(
                  title: "角度",
                  title_size: XGUIFontSize.M,
                  title_hover_color: XHud_Dashboard.Theme_Primary,
                  title_width: 90,
                  prop: sp_def_create_rot_deg,
                  prop_margin: new RectOffset(0, 0, 5, 0));

                XGUI.layout_space(5);

                SerializedProperty sp_def_create_rot_dur = CreateArgs_Default.FindPropertyRelative("Rotation.Duration");
                XGUI.layout_property_field(
                  title: "耗时",
                  title_size: XGUIFontSize.M,
                  title_hover_color: XHud_Dashboard.Theme_Primary,
                  title_width: 90,
                  prop: sp_def_create_rot_dur,
                  prop_margin: new RectOffset(0, 0, 5, 0));

                XGUI.layout_space(5);

                SerializedProperty sp_def_create_rot_delay = CreateArgs_Default.FindPropertyRelative("Rotation.Delay");
                XGUI.layout_property_field(
                  title: "延迟",
                  title_size: XGUIFontSize.M,
                  title_hover_color: XHud_Dashboard.Theme_Primary,
                  title_width: 90,
                  prop: sp_def_create_rot_delay,
                  prop_margin: new RectOffset(0, 0, 5, 0));

                XGUI.layout_space(5);

                SerializedProperty sp_def_create_rot_curve = CreateArgs_Default.FindPropertyRelative("Rotation.Curve");
                XGUI.layout_property_field(
                  title: "曲线",
                  title_size: XGUIFontSize.M,
                  title_hover_color: XHud_Dashboard.Theme_Primary,
                  title_width: 90,
                  prop: sp_def_create_rot_curve,
                  prop_margin: new RectOffset(0, 0, 5, 0));

                XGUI.layout_space(5);

                SerializedProperty sp_def_create_rot_ease = CreateArgs_Default.FindPropertyRelative("Rotation.Ease");
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
                SerializedProperty sp_def_create_alpha_type = CreateArgs_Default.FindPropertyRelative("Alpha.Duration");
                XGUI.layout_property_field(
                    title: "耗时",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: sp_def_create_alpha_type,
                    prop_margin: new RectOffset(0, 0, 5, 0));

                XGUI.layout_space(5);

                SerializedProperty sp_def_create_alpha_delay = CreateArgs_Default.FindPropertyRelative("Alpha.Delay");
                XGUI.layout_property_field(
                    title: "延迟",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: sp_def_create_alpha_delay,
                    prop_margin: new RectOffset(0, 0, 5, 0));

                XGUI.layout_space(5);

                SerializedProperty sp_def_create_alpha_curve = CreateArgs_Default.FindPropertyRelative("Alpha.Curve");
                XGUI.layout_property_field(
                    title: "曲线",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: sp_def_create_alpha_curve,
                    prop_margin: new RectOffset(0, 0, 5, 0));

                XGUI.layout_space(5);

                SerializedProperty sp_def_create_alpha_ease = CreateArgs_Default.FindPropertyRelative("Alpha.Ease");
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
                        HudMotion_Movement m = (HudMotion_Movement)CreateArgs_Default.FindPropertyRelative("Movement.Movement").enumValueIndex;
                        if (m == HudMotion_Movement.A_无运动)
                        {
                            XGUI.dialog(
                                type: XGUIDialogType.警告,
                                windowtitle: "XHud - 管理器消息",
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
                        HudMotion_Rotation r = (HudMotion_Rotation)CreateArgs_Default.FindPropertyRelative("Rotation.Rotation").enumValueIndex;
                        if (r == HudMotion_Rotation.A_无旋转)
                        {
                            XGUI.dialog(
                                type: XGUIDialogType.警告,
                                windowtitle: "XHud - 管理器消息",
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
            if (BaseScript.Hud_Motions != null)
            {
                //确保动效库不是空的
                if (BaseScript.Hud_Motions.ElementMotionList != null && BaseScript.Hud_Motions.ElementMotionList.Count > 0)
                {
                    //动效列表
                    string[] motnames = BaseScript.Hud_Motions.ElementMotion_GetAllName_With_Recycle();
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
                        opt_bg_fill: XGUIGroup.实体,
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

                            Motion_Recycler rec = BaseScript.Hud_Motions.ElementMotion_GetElementCreator_At_Recycle(Rec_Lib_Name.stringValue);

                            RecycleArgs_Default.FindPropertyRelative("Movement.Movement").enumValueIndex = (int)rec.Movement.Movement;
                            RecycleArgs_Default.FindPropertyRelative("Movement.Distance").floatValue = rec.Movement.Distance;
                            RecycleArgs_Default.FindPropertyRelative("Movement.Duration").floatValue = rec.Movement.Duration;
                            RecycleArgs_Default.FindPropertyRelative("Movement.Delay").floatValue = rec.Movement.Delay;
                            RecycleArgs_Default.FindPropertyRelative("Movement.Curve").animationCurveValue = rec.Movement.Curve;
                            RecycleArgs_Default.FindPropertyRelative("Movement.CurveName").stringValue = rec.Movement.CurveName;
                            RecycleArgs_Default.FindPropertyRelative("Movement.Ease").enumValueIndex = (int)rec.Movement.Ease;
                            RecycleArgs_Default.FindPropertyRelative("Rotation.Rotation").enumValueIndex = (int)rec.Rotation.Rotation;
                            RecycleArgs_Default.FindPropertyRelative("Rotation.Degree").floatValue = rec.Rotation.Degree;
                            RecycleArgs_Default.FindPropertyRelative("Rotation.Duration").floatValue = rec.Rotation.Duration;
                            RecycleArgs_Default.FindPropertyRelative("Rotation.Delay").floatValue = rec.Rotation.Delay;
                            RecycleArgs_Default.FindPropertyRelative("Rotation.Curve").animationCurveValue = rec.Rotation.Curve;
                            RecycleArgs_Default.FindPropertyRelative("Rotation.CurveName").stringValue = rec.Rotation.CurveName;
                            RecycleArgs_Default.FindPropertyRelative("Rotation.Ease").enumValueIndex = (int)rec.Rotation.Ease;
                            RecycleArgs_Default.FindPropertyRelative("Alpha.Duration").floatValue = rec.Alpha.Duration;
                            RecycleArgs_Default.FindPropertyRelative("Alpha.Delay").floatValue = rec.Alpha.Delay;
                            RecycleArgs_Default.FindPropertyRelative("Alpha.Curve").animationCurveValue = rec.Alpha.Curve;
                            RecycleArgs_Default.FindPropertyRelative("Alpha.CurveName").stringValue = rec.Alpha.CurveName;
                            RecycleArgs_Default.FindPropertyRelative("Alpha.Ease").enumValueIndex = (int)rec.Alpha.Ease;
                            RecycleArgs_Default.FindPropertyRelative("MotionAnimateEndState").enumValueIndex = (int)rec.MotionAnimateEndState;
                            RecycleArgs_Default.serializedObject.ApplyModifiedProperties();
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
                        xHud_OpenParameterSetter(HudElementMotionType.Recycler);
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
                        if (!BaseScript.Hud_Motions.ElementMotion_IsExist(Rec_Lib_Name.stringValue))
                            return;
                        Editor_XHud_MenuItemsAction_OpenLibrary.open_elementmotion();
                        BaseScript.Hud_Motions.ElementMotionLibrary_Location(Rec_Lib_Name.stringValue);
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
                            windowtitle: "XHud - 管理器消息",
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
            def_recycle_fold_move = EditorGUILayout.Foldout(def_recycle_fold_move, "位移", true);

            if (def_recycle_fold_move)
            {
                SerializedProperty sp_def_recycle_move_type = RecycleArgs_Default.FindPropertyRelative("Movement.Movement");
                XGUI.layout_property_field(
                    title: "方式",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: sp_def_recycle_move_type,
                    prop_margin: new RectOffset(0, 0, 5, 0));

                XGUI.layout_space(5);

                SerializedProperty sp_def_recycle_move_dis = RecycleArgs_Default.FindPropertyRelative("Movement.Distance");
                XGUI.layout_property_field(
                    title: "距离",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: sp_def_recycle_move_dis,
                    prop_margin: new RectOffset(0, 0, 5, 0));

                XGUI.layout_space(5);

                SerializedProperty sp_def_recycle_move_dur = RecycleArgs_Default.FindPropertyRelative("Movement.Duration");
                XGUI.layout_property_field(
                    title: "耗时",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: sp_def_recycle_move_dur,
                    prop_margin: new RectOffset(0, 0, 5, 0));

                XGUI.layout_space(5);

                SerializedProperty sp_def_recycle_move_delay = RecycleArgs_Default.FindPropertyRelative("Movement.Delay");
                XGUI.layout_property_field(
                    title: "延迟",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: sp_def_recycle_move_delay,
                    prop_margin: new RectOffset(0, 0, 5, 0));

                XGUI.layout_space(5);

                SerializedProperty sp_def_recycle_move_curve = RecycleArgs_Default.FindPropertyRelative("Movement.Curve");
                XGUI.layout_property_field(
                    title: "曲线",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: sp_def_recycle_move_curve,
                    prop_margin: new RectOffset(0, 0, 5, 0));

                XGUI.layout_space(5);

                SerializedProperty sp_def_recycle_move_ease = RecycleArgs_Default.FindPropertyRelative("Movement.Ease");
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
                SerializedProperty sp_def_recycle_rot_type = RecycleArgs_Default.FindPropertyRelative("Rotation.Rotation");
                XGUI.layout_property_field(
                    title: "方式",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: sp_def_recycle_rot_type,
                    prop_margin: new RectOffset(0, 0, 5, 0));

                XGUI.layout_space(5);

                SerializedProperty sp_def_recycle_rot_deg = RecycleArgs_Default.FindPropertyRelative("Rotation.Degree");
                XGUI.layout_property_field(
                    title: "角度",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: sp_def_recycle_rot_deg,
                    prop_margin: new RectOffset(0, 0, 5, 0));

                XGUI.layout_space(5);

                SerializedProperty sp_def_recycle_rot_dur = RecycleArgs_Default.FindPropertyRelative("Rotation.Duration");
                XGUI.layout_property_field(
                    title: "耗时",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: sp_def_recycle_rot_dur,
                    prop_margin: new RectOffset(0, 0, 5, 0));

                XGUI.layout_space(5);

                SerializedProperty sp_def_recycle_rot_delay = RecycleArgs_Default.FindPropertyRelative("Rotation.Delay");
                XGUI.layout_property_field(
                    title: "延迟",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: sp_def_recycle_rot_delay,
                    prop_margin: new RectOffset(0, 0, 5, 0));

                XGUI.layout_space(5);

                SerializedProperty sp_def_recycle_rot_curve = RecycleArgs_Default.FindPropertyRelative("Rotation.Curve");
                XGUI.layout_property_field(
                    title: "曲线",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: sp_def_recycle_rot_curve,
                    prop_margin: new RectOffset(0, 0, 5, 0));

                XGUI.layout_space(5);

                SerializedProperty sp_def_recycle_rot_ease = RecycleArgs_Default.FindPropertyRelative("Rotation.Ease");
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
                SerializedProperty sp_def_recycle_alpha_type = RecycleArgs_Default.FindPropertyRelative("Alpha.Duration");
                XGUI.layout_property_field(
                    title: "耗时",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: sp_def_recycle_alpha_type,
                    prop_margin: new RectOffset(0, 0, 5, 0));

                XGUI.layout_space(5);

                SerializedProperty sp_def_recycle_alpha_delay = RecycleArgs_Default.FindPropertyRelative("Alpha.Delay");
                XGUI.layout_property_field(
                    title: "延迟",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: sp_def_recycle_alpha_delay,
                    prop_margin: new RectOffset(0, 0, 5, 0));

                XGUI.layout_space(5);

                SerializedProperty sp_def_recycle_alpha_curve = RecycleArgs_Default.FindPropertyRelative("Alpha.Curve");
                XGUI.layout_property_field(
                    title: "曲线",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.Theme_Primary,
                    title_width: 90,
                    prop: sp_def_recycle_alpha_curve,
                    prop_margin: new RectOffset(0, 0, 5, 0));

                XGUI.layout_space(5);

                SerializedProperty sp_def_recycle_alpha_ease = RecycleArgs_Default.FindPropertyRelative("Alpha.Ease");
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
                        HudMotion_Movement m = (HudMotion_Movement)RecycleArgs_Default.FindPropertyRelative("Movement.Movement").enumValueIndex;
                        if (m == HudMotion_Movement.A_无运动)
                        {
                            XGUI.dialog(
                                type: XGUIDialogType.警告,
                                windowtitle: "XHud - 管理器消息",
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
                        HudMotion_Rotation r = (HudMotion_Rotation)RecycleArgs_Default.FindPropertyRelative("Rotation.Rotation").enumValueIndex;
                        if (r == HudMotion_Rotation.A_无旋转)
                        {
                            XGUI.dialog(
                                type: XGUIDialogType.警告,
                                windowtitle: "XHud - 管理器消息",
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
        }
        #endregion

        #region 工具 - 动效参数
        /// <summary>
        /// 重置生成与回收的参数到默认。
        /// <para/>
        /// <list type="bullet">
        /// <item><description><paramref name="state"/> 为 <c>"c"</c> 时重置生成参数。</description></item>
        /// <item><description><paramref name="state"/> 为 <c>"r"</c> 时重置回收参数。</description></item>
        /// </list>
        /// </summary>
        /// <param name="state">重置目标，<c>"c"</c> 生成 / <c>"r"</c> 回收。</param>
        private void ResetMotionParams(string state)
        {
            if (state == "c")
            {
                CreateArgs_Default.FindPropertyRelative("anchor").enumValueIndex = (int)XHudAnchor.中心;
                CreateArgs_Default.FindPropertyRelative("Movement.Movement").enumValueIndex = (int)HudMotion_Movement.S_从下至上;
                CreateArgs_Default.FindPropertyRelative("Movement.Distance").floatValue = 100;
                CreateArgs_Default.FindPropertyRelative("Movement.Duration").floatValue = 1;
                CreateArgs_Default.FindPropertyRelative("Movement.Delay").floatValue = 0;
                CreateArgs_Default.FindPropertyRelative("Movement.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
                CreateArgs_Default.FindPropertyRelative("Movement.CurveName").stringValue = "";
                CreateArgs_Default.FindPropertyRelative("Movement.Ease").enumValueIndex = (int)EaseMode.InOutCubic;
                CreateArgs_Default.FindPropertyRelative("Rotation.Rotation").enumValueIndex = (int)HudMotion_Rotation.A_无旋转;
                CreateArgs_Default.FindPropertyRelative("Rotation.Degree").floatValue = 0;
                CreateArgs_Default.FindPropertyRelative("Rotation.Duration").floatValue = 1;
                CreateArgs_Default.FindPropertyRelative("Rotation.Delay").floatValue = 0;
                CreateArgs_Default.FindPropertyRelative("Rotation.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
                CreateArgs_Default.FindPropertyRelative("Rotation.CurveName").stringValue = "";
                CreateArgs_Default.FindPropertyRelative("Rotation.Ease").enumValueIndex = (int)EaseMode.InOutCubic;
                CreateArgs_Default.FindPropertyRelative("Alpha.Duration").floatValue = 1;
                CreateArgs_Default.FindPropertyRelative("Alpha.Delay").floatValue = 0;
                CreateArgs_Default.FindPropertyRelative("Alpha.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
                CreateArgs_Default.FindPropertyRelative("Alpha.CurveName").stringValue = "";
                CreateArgs_Default.FindPropertyRelative("Alpha.Ease").enumValueIndex = (int)EaseMode.InOutCubic;
                CreateArgs_MotionAnimateEndState.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
                CreateArgs_Default.serializedObject.ApplyModifiedProperties();

                Crc_Lib_Name.stringValue = null;
                Crc_Lib_Name.serializedObject.ApplyModifiedProperties();
            }
            else if (state == "r")
            {
                RecycleArgs_Default.FindPropertyRelative("Movement.Movement").enumValueIndex = (int)HudMotion_Movement.D_从上至下;
                RecycleArgs_Default.FindPropertyRelative("Movement.Distance").floatValue = 100;
                RecycleArgs_Default.FindPropertyRelative("Movement.Duration").floatValue = 1;
                RecycleArgs_Default.FindPropertyRelative("Movement.Delay").floatValue = 0;
                RecycleArgs_Default.FindPropertyRelative("Movement.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
                RecycleArgs_Default.FindPropertyRelative("Movement.CurveName").stringValue = "";
                RecycleArgs_Default.FindPropertyRelative("Movement.Ease").enumValueIndex = (int)EaseMode.InOutCubic;
                RecycleArgs_Default.FindPropertyRelative("Rotation.Rotation").enumValueIndex = (int)HudMotion_Rotation.A_无旋转;
                RecycleArgs_Default.FindPropertyRelative("Rotation.Degree").floatValue = 0;
                RecycleArgs_Default.FindPropertyRelative("Rotation.Duration").floatValue = 1;
                RecycleArgs_Default.FindPropertyRelative("Rotation.Delay").floatValue = 0;
                RecycleArgs_Default.FindPropertyRelative("Rotation.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
                RecycleArgs_Default.FindPropertyRelative("Rotation.CurveName").stringValue = "";
                RecycleArgs_Default.FindPropertyRelative("Rotation.Ease").enumValueIndex = (int)EaseMode.InOutCubic;
                RecycleArgs_Default.FindPropertyRelative("Alpha.Duration").floatValue = 1;
                RecycleArgs_Default.FindPropertyRelative("Alpha.Delay").floatValue = 0;
                RecycleArgs_Default.FindPropertyRelative("Alpha.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
                RecycleArgs_Default.FindPropertyRelative("Alpha.CurveName").stringValue = "";
                RecycleArgs_Default.FindPropertyRelative("Alpha.Ease").enumValueIndex = (int)EaseMode.InOutCubic;
                RecycleArgs_MotionAnimateEndState.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
                RecycleArgs_Default.serializedObject.ApplyModifiedProperties();

                Rec_Lib_Name.stringValue = null;
                Rec_Lib_Name.serializedObject.ApplyModifiedProperties();
            }
        }
        #endregion

        #region 工具 - 打开辅助窗口
        /// <summary>
        /// 动效库添加器。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>创建并显示 <c>Editor_XHud_LibrarySetTool_Motion</c> 窗口。</description></item>
        /// <item><description>按传入类型把当前生成/回收参数拷贝到窗口中。</description></item>
        /// </list>
        /// </summary>
        /// <param name="Type">动效类型。</param>
        public void xHud_OpenParameterSetter(HudElementMotionType Type)
        {
            Editor_XHud_LibrarySetTool_Motion window = EditorWindow.GetWindow<Editor_XHud_LibrarySetTool_Motion>(true);

            window.titleContent = new GUIContent("XHud - 元素动效资源采集器");
            XGUI.CenterEditorWindow(new Vector2Int(348, Type == HudElementMotionType.Creator ? 850 : 780), window);

            switch (Type)
            {
                case HudElementMotionType.Recycler:
                    Motion_Recycler rec = new Motion_Recycler();
                    rec.MotionAnimateEndState = BaseScript.RecycleArgs_Default.MotionAnimateEndState;
                    rec.Alpha = BaseScript.RecycleArgs_Default.Alpha;

                    rec.Movement = new MotionNode_Movement();
                    rec.Movement.CopyData(BaseScript.RecycleArgs_Default.Movement);

                    rec.Rotation = new MotionNode_Rotation();
                    rec.Rotation.CopyData(BaseScript.RecycleArgs_Default.Rotation);

                    rec.Alpha = new MotionNode_Alpha();
                    rec.Alpha.CopyData(BaseScript.RecycleArgs_Default.Alpha);

                    window.SetElementMotion(rec);
                    break;
                case HudElementMotionType.Creator:
                    Motion_Creator crc = new Motion_Creator();
                    crc.MotionAnimateEndState = BaseScript.CreateArgs_Default.MotionAnimateEndState;
                    crc.anchor = BaseScript.CreateArgs_Default.anchor;

                    crc.Alpha = BaseScript.CreateArgs_Default.Alpha;

                    crc.Movement = new MotionNode_Movement();
                    crc.Movement.CopyData(BaseScript.CreateArgs_Default.Movement);

                    crc.Rotation = new MotionNode_Rotation();
                    crc.Rotation.CopyData(BaseScript.CreateArgs_Default.Rotation);

                    crc.Alpha = new MotionNode_Alpha();
                    crc.Alpha.CopyData(BaseScript.CreateArgs_Default.Alpha);

                    window.SetElementMotion(crc);
                    break;
            }

            window.SetElementMotionType(Type);
            window.SetLibrarySetterMode(LibrarySetterMode.添加到库);
            window.SetButtonText("添加", "取消");
            window.SetTitle("元素动效资源采集器");
            window.SetTarget_Hud_MotionLibrary(BaseScript.Hud_Motions);
            //window.ShowModal();
            window.Show();
        }
        #endregion
    }
}