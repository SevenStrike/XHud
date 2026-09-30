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

    public class Editor_XHud_LibrarySetTool_Motion : EditorWindow
    {
        private SerializedObject BaseObject;
        private SerializedProperty sp_LibName, sp_Description, sp_CreateArgs, sp_RecycleArgs, sp_Type;

        [SerializeField]
        private HudElementMotionType Type;
        private Texture2D icon;

        private Texture2D icon_libsetter_motion_movement_r;
        private Texture2D icon_libsetter_motion_movement_p;
        private Texture2D icon_libsetter_motion_rotator_r;
        private Texture2D icon_libsetter_motion_rotator_p;
        private Texture2D icon_libsetter_motion_alpha_r;
        private Texture2D icon_libsetter_motion_alpha_p;

        public LibrarySetterMode LibrarySetterMode;

        [SerializeField]
        public string LibName;
        [SerializeField]
        public string Description;

        /// <summary>
        /// 按钮高度
        /// </summary>
        private float ButtonHeight = 25;
        /// <summary>
        /// 按钮间距
        /// </summary>
        private float ButtonDistance = 15;

        public string ButtonText_Ok;
        public string ButtonText_Cancel;

        private float IconSize = 35;
        private int ToolbarIndex = 0;

        public Motion_Creator CreateArgs;
        public Motion_Recycler RecycleArgs;

        public int ModifiedIndex;
        private XHud_Library_Motion Target_Hud_MotionLibrary;
        string Title;

        Rect rect_info;

        private void OnDisable()
        {

        }

        private void OnEnable()
        {
            BaseObject = new SerializedObject(this);
            sp_LibName = BaseObject.FindProperty("LibName");
            sp_Description = BaseObject.FindProperty("Description");
            sp_CreateArgs = BaseObject.FindProperty("CreateArgs");
            sp_RecycleArgs = BaseObject.FindProperty("RecycleArgs");
            sp_Type = BaseObject.FindProperty("Type");

            icon = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_motion_setter/icon");

            icon_libsetter_motion_movement_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_motion_setter/icon_libsetter_motion_movement_r");
            icon_libsetter_motion_rotator_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_motion_setter/icon_libsetter_motion_rotator_r");
            icon_libsetter_motion_alpha_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_motion_setter/icon_libsetter_motion_alpha_r");
            icon_libsetter_motion_movement_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_motion_setter/icon_libsetter_motion_movement_p");
            icon_libsetter_motion_rotator_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_motion_setter/icon_libsetter_motion_rotator_p");
            icon_libsetter_motion_alpha_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_motion_setter/icon_libsetter_motion_alpha_p");

            Description = "动效参数说明内容";
            LibName = "动效名称";
        }

        private void OnDestroy()
        {
            GetWindow<SceneView>().Focus();
        }

        private void OnGUI()
        {
            BaseObject.Update();

#if UNITY_6000_0_OR_NEWER
            TextClipping clipping = TextClipping.Ellipsis;
#else
    TextClipping clipping = TextClipping.Clip;
#endif

            #region 抬头
            Rect rect = new Rect(0, 0, position.width, position.height);

            // 图标
            Rect rect_icon = new Rect(15, 15, icon.width, icon.height);
            XGUI.gui_icon(
                rect: rect_icon,
                icon: icon,
                padding: new RectOffset(0, 0, 0, 0),
                border: new RectOffset(0, 0, 0, 0),
                color: Color.white);

            // 大标题
            Rect rect_title = new Rect(rect.x + 70, rect.y + 10, rect.width - 80, 30);
            XGUI.gui_label(
                rect: rect_title,
                text: new GUIContent(Title),
                text_color: Color.white,
                size: XGUIFontSize.L,
                clipping: clipping,
                font: XGUI.GetFont("xg-heavy"));

            // 分割线
            Rect rect_seperate = new Rect(rect.x + 68, rect.y + 43, 200, 1);
            XGUI.gui_seperator(
                rect: rect_seperate,
                thickness: 1,
                color: XHud_Dashboard.Theme_SeperateLine,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0));

            // 小标题
            Rect rect_subtitle = new Rect(rect.x + 18, rect.y + 53, rect.width, 30);
            XGUI.gui_label(
                rect: rect_subtitle,
                text: new GUIContent("以下为目标元素动效模版的参数，可根据需要进行调整"),
                text_color: Color.white * 0.7f,
                size: XGUIFontSize.M,
                anchor: TextAnchor.UpperLeft,
                clipping: clipping);
            #endregion

            Rect rect_mark = new Rect(rect.x + (rect.width - 80), rect.y + 15, 65, XGUI.GetSingleLineHeight());
            XGUI.gui_label(
                rect: rect_mark,
                bg_fill: XGUIFilled.实体,
                bg_color: XGUIColor.亮白,
                bg_color_gui: Color.white,
                text: new GUIContent(Type.ToString()),
                text_color: Color.black,
                size: XGUIFontSize.S,
                anchor: TextAnchor.MiddleCenter,
                offset: new Vector2(0, -2),
                clipping: clipping);

            //动效动画结束基准状态
            SerializedProperty sp_MotionAnimateEndState_crc = sp_CreateArgs.FindPropertyRelative("MotionAnimateEndState");
            MotionAnimateEndState maes_crc = (MotionAnimateEndState)sp_MotionAnimateEndState_crc.enumValueIndex;

            SerializedProperty sp_MotionAnimateEndState_rec = sp_RecycleArgs.FindPropertyRelative("MotionAnimateEndState");
            MotionAnimateEndState maes_rec = (MotionAnimateEndState)sp_MotionAnimateEndState_rec.enumValueIndex;

            XGUI.layout_space(100);

            #region 基准按钮
            XGUI.layout_group_start(
                type: XGUIContainerType.Horizontal,
                absolute_margin: true,
                absolute_padding: true,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0));

            XGUI.layout_flexspace();

            if (XGUI.layout_button(
                    tooltip: "以位移结束为准",
                    tex_release: icon_libsetter_motion_movement_r,
                    tex_press: icon_libsetter_motion_movement_p,
                    tex_gui_color: (Type == HudElementMotionType.Creator ? (maes_crc == MotionAnimateEndState.以_移动为准 ? XHud_Dashboard.Theme_Primary : Color.gray) : (maes_rec == MotionAnimateEndState.以_移动为准 ? XHud_Dashboard.Theme_Primary : Color.gray)),
                    border: new RectOffset(0, 0, 0, 0),
                    width: IconSize,
                    height: IconSize))
            {
                if (Type == HudElementMotionType.Creator)
                {
                    sp_MotionAnimateEndState_crc.enumValueIndex = (int)MotionAnimateEndState.以_移动为准;
                }
                else
                {
                    sp_MotionAnimateEndState_rec.enumValueIndex = (int)MotionAnimateEndState.以_移动为准;
                }
                sp_MotionAnimateEndState_crc.serializedObject.ApplyModifiedProperties();
                sp_MotionAnimateEndState_rec.serializedObject.ApplyModifiedProperties();

                sp_CreateArgs.serializedObject.ApplyModifiedProperties();
                sp_RecycleArgs.serializedObject.ApplyModifiedProperties();
            }

            XGUI.layout_space(80);

            if (XGUI.layout_button(
                     tooltip: "以旋转结束为准",
                     tex_release: icon_libsetter_motion_rotator_r,
                     tex_press: icon_libsetter_motion_rotator_p,
                     tex_gui_color: (Type == HudElementMotionType.Creator ? (maes_crc == MotionAnimateEndState.以_旋转为准 ? XHud_Dashboard.Theme_Primary : Color.gray) : (maes_rec == MotionAnimateEndState.以_旋转为准 ? XHud_Dashboard.Theme_Primary : Color.gray)),
                     border: new RectOffset(0, 0, 0, 0),
                     width: IconSize,
                     height: IconSize))
            {
                if (Type == HudElementMotionType.Creator)
                {
                    sp_MotionAnimateEndState_crc.enumValueIndex = (int)MotionAnimateEndState.以_旋转为准;
                }
                else
                {
                    sp_MotionAnimateEndState_rec.enumValueIndex = (int)MotionAnimateEndState.以_旋转为准;
                }
                sp_MotionAnimateEndState_crc.serializedObject.ApplyModifiedProperties();
                sp_MotionAnimateEndState_rec.serializedObject.ApplyModifiedProperties();

                sp_CreateArgs.serializedObject.ApplyModifiedProperties();
                sp_RecycleArgs.serializedObject.ApplyModifiedProperties();
            }

            XGUI.layout_space(80);

            if (XGUI.layout_button(
                    tooltip: "以透明度结束为准",
                    tex_release: icon_libsetter_motion_alpha_r,
                    tex_press: icon_libsetter_motion_alpha_p,
                    tex_gui_color: (Type == HudElementMotionType.Creator ? (maes_crc == MotionAnimateEndState.以_透明度为准 ? XHud_Dashboard.Theme_Primary : Color.gray) : (maes_rec == MotionAnimateEndState.以_透明度为准 ? XHud_Dashboard.Theme_Primary : Color.gray)),
                    border: new RectOffset(0, 0, 0, 0),
                    width: IconSize,
                    height: IconSize))
            {
                if (Type == HudElementMotionType.Creator)
                {
                    sp_MotionAnimateEndState_crc.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
                }
                else
                {
                    sp_MotionAnimateEndState_rec.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
                }

                sp_MotionAnimateEndState_crc.serializedObject.ApplyModifiedProperties();
                sp_MotionAnimateEndState_rec.serializedObject.ApplyModifiedProperties();

                sp_CreateArgs.serializedObject.ApplyModifiedProperties();
                sp_RecycleArgs.serializedObject.ApplyModifiedProperties();
            }

            XGUI.layout_flexspace();

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            #endregion

            XGUI.layout_space(15);

            #region 模版信息
            if (LibrarySetterMode != LibrarySetterMode.修改生成器项参数)
            {
                XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.Theme_Group,
                title: "模版信息",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.Theme_Primary,
                title_clipping: TextClipping.Clip,
                margin: new RectOffset(10, 10, 0, 0),
                padding: new RectOffset(10, 10, 15, 15));

                rect_info = XGUI.GetLastRect();

                if (string.IsNullOrEmpty(sp_LibName.stringValue))
                {
                    sp_LibName.stringValue = "文字样式名称";
                }

                sp_LibName.stringValue = XGUI.layout_inputfield(
                       title: "名称",
                       prop: sp_LibName.stringValue,
                       text_wrap: false,
                       field_fontsize: XGUIFontSize.M,
                       field_text_offset: Vector2.zero,
                       field_height: 20,
                       field_padding: new RectOffset(0, 0, 0, 0),
                       field_margin: new RectOffset(0, 0, 5, 0),
                       field_text_color: Color.white,
                       title_width: 40,
                       field_text_font: XGUI.GetFont("xg-medium"),
                       field_text_style: FontStyle.Normal,
                       field_text_anchor: TextAnchor.MiddleLeft);

                sp_LibName.serializedObject.ApplyModifiedProperties();

                if (string.IsNullOrEmpty(sp_Description.stringValue))
                {
                    sp_Description.stringValue = "文字样式参数说明内容";
                }

                sp_Description.stringValue = XGUI.layout_inputfield(
                    title: "说明",
                    prop: sp_Description.stringValue,
                    text_wrap: true,
                    field_fontsize: XGUIFontSize.M,
                    field_text_offset: Vector2.zero,
                    field_height: 80,
                    field_padding: new RectOffset(0, 0, 0, 0),
                    field_margin: new RectOffset(0, 0, 5, 0),
                    field_text_color: Color.white,
                    title_width: 40,
                    field_text_font: XGUI.GetFont("xg-medium"),
                    field_text_style: FontStyle.Normal,
                    field_text_anchor: TextAnchor.UpperLeft);

                sp_Description.serializedObject.ApplyModifiedProperties();

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            }
            #endregion

            #region 动效类型
            XGUI.layout_group_start(
           type: XGUIContainerType.Vertical,
           bg_fill: XGUIFilled.缺口纯色边框,
           bg_color: XGUIColor.亮白,
           bg_color_gui: XHud_Dashboard.Theme_Group,
           title: "动效类型",
           title_size: XGUIFontSize.M,
           title_text_color: XHud_Dashboard.Theme_Primary,
           title_clipping: TextClipping.Clip,
           margin: new RectOffset(10, 10, 0, 0),
           padding: new RectOffset(10, 10, 15, 15));

            #region 类型
            XGUI.layout_property_field(
                title: "类型",
                title_size: XGUIFontSize.M,
                title_hover_color: Color.white,
                title_width: 60,
                prop: sp_Type,
                prop_padding: new RectOffset(0, 0, 0, 0),
                prop_margin: new RectOffset(0, 0, 0, 10));

            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            if (Type == HudElementMotionType.Creator)
            {
                #region 生成锚点
                XGUI.layout_group_start(
               type: XGUIContainerType.Vertical,
               bg_fill: XGUIFilled.缺口纯色边框,
               bg_color: XGUIColor.亮白,
               bg_color_gui: XHud_Dashboard.Theme_Group,
               title: "生成锚点",
               title_size: XGUIFontSize.M,
               title_text_color: XHud_Dashboard.Theme_Primary,
               title_clipping: TextClipping.Clip,
               margin: new RectOffset(10, 10, 0, 0),
               padding: new RectOffset(10, 10, 15, 15));

                #region 锚点
                XGUI.layout_property_field(
                    title: "锚点",
                    title_size: XGUIFontSize.M,
                    title_hover_color: Color.white,
                    title_width: 60,
                    prop: sp_CreateArgs.FindPropertyRelative("anchor"),
                    prop_padding: new RectOffset(0, 0, 0, 0),
                    prop_margin: new RectOffset(0, 0, 0, 10));
                #endregion

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                #endregion
            }

            #region 参数
            XGUI.layout_group_start(
               type: XGUIContainerType.Vertical,
               bg_fill: XGUIFilled.缺口纯色边框,
               bg_color: XGUIColor.亮白,
               bg_color_gui: XHud_Dashboard.Theme_Group,
               title: "位移参数",
               title_size: XGUIFontSize.M,
               title_text_color: XHud_Dashboard.Theme_Primary,
               title_clipping: TextClipping.Clip,
               margin: new RectOffset(10, 10, 0, 0),
               padding: new RectOffset(10, 10, 15, 15));

            ToolbarIndex = XGUI.layout_toolbar(
                index: ref ToolbarIndex,
                names: new string[] { "位移", "旋转", "透明度" },
                bg_normal: XGUIFilled.无,
                bg_selected: XGUIFilled.实体,
                bg_color: XGUIColor.亮白,
                bg_gui_color: Color.black * 0.5f,
                text_color_normal: Color.white,
                text_color_selected: XHud_Dashboard.Theme_Primary,
                bar_height: 25,
                text_anchor: TextAnchor.MiddleCenter,
                text_padding: new RectOffset(10, 10, 0, 0),
                bar_margin: new RectOffset(0, 0, 5, 15),
                text_offset: new Vector2(0, -2),
                text_size: XGUIFontSize.M,
                text_font: XGUI.GetFont("xg-medium"),
                text_fontstyle: FontStyle.Bold,
                bg_width_offset: 5,
                bg_height_offset: 2,
                navigate_style: true,
                navigate_style_bg: XGUIFilled.纯色边框,
                navigate_style_bg_color: Color.black * 0.5f);

            switch (ToolbarIndex)
            {
                // -----------  位移
                case 0:
                    #region 方式
                    XGUI.layout_property_field(
                        title: "方式",
                        title_size: XGUIFontSize.M,
                        title_hover_color: Color.white,
                        title_width: 60,
                        prop: Type == HudElementMotionType.Creator ? sp_CreateArgs.FindPropertyRelative("Movement.Movement") : sp_RecycleArgs.FindPropertyRelative("Movement.Movement"),
                        prop_padding: new RectOffset(0, 0, 0, 0),
                        prop_margin: new RectOffset(0, 0, 0, 10));
                    #endregion

                    #region 距离
                    XGUI.layout_property_field(
                        title: "距离",
                        title_size: XGUIFontSize.M,
                        title_hover_color: Color.white,
                        title_width: 60,
                        prop: Type == HudElementMotionType.Creator ? sp_CreateArgs.FindPropertyRelative("Movement.Distance") : sp_RecycleArgs.FindPropertyRelative("Movement.Distance"),
                        prop_padding: new RectOffset(0, 0, 0, 0),
                        prop_margin: new RectOffset(0, 0, 0, 10));
                    #endregion

                    #region 耗时
                    XGUI.layout_property_field(
                        title: "耗时",
                        title_size: XGUIFontSize.M,
                        title_hover_color: Color.white,
                        title_width: 60,
                        prop: Type == HudElementMotionType.Creator ? sp_CreateArgs.FindPropertyRelative("Movement.Duration") : sp_RecycleArgs.FindPropertyRelative("Movement.Duration"),
                        prop_padding: new RectOffset(0, 0, 0, 0),
                        prop_margin: new RectOffset(0, 0, 0, 10));
                    #endregion

                    #region 延迟
                    XGUI.layout_property_field(
                        title: "延迟",
                        title_size: XGUIFontSize.M,
                        title_hover_color: Color.white,
                        title_width: 60,
                        prop: Type == HudElementMotionType.Creator ? sp_CreateArgs.FindPropertyRelative("Movement.Delay") : sp_RecycleArgs.FindPropertyRelative("Movement.Delay"),
                        prop_padding: new RectOffset(0, 0, 0, 0),
                        prop_margin: new RectOffset(0, 0, 0, 10));
                    #endregion

                    #region 曲线
                    XGUI.layout_property_field(
                        title: "曲线",
                        title_size: XGUIFontSize.M,
                        title_hover_color: Color.white,
                        title_width: 60,
                        prop: Type == HudElementMotionType.Creator ? sp_CreateArgs.FindPropertyRelative("Movement.Curve") : sp_RecycleArgs.FindPropertyRelative("Movement.Curve"),
                        prop_padding: new RectOffset(0, 0, 0, 0),
                        prop_margin: new RectOffset(0, 0, 0, 10));
                    #endregion

                    #region 缓动
                    XGUI.layout_property_field(
                        title: "缓动",
                        title_size: XGUIFontSize.M,
                        title_hover_color: Color.white,
                        title_width: 60,
                        prop: Type == HudElementMotionType.Creator ? sp_CreateArgs.FindPropertyRelative("Movement.Ease") : sp_RecycleArgs.FindPropertyRelative("Movement.Ease"),
                        prop_padding: new RectOffset(0, 0, 0, 0),
                        prop_margin: new RectOffset(0, 0, 0, 10));
                    #endregion
                    break;
                // -----------  旋转
                case 1:
                    #region 方式
                    XGUI.layout_property_field(
                        title: "方式",
                        title_size: XGUIFontSize.M,
                        title_hover_color: Color.white,
                        title_width: 60,
                        prop: Type == HudElementMotionType.Creator ? sp_CreateArgs.FindPropertyRelative("Rotation.Rotation") : sp_RecycleArgs.FindPropertyRelative("Rotation.Rotation"),
                        prop_padding: new RectOffset(0, 0, 0, 0),
                        prop_margin: new RectOffset(0, 0, 0, 10));
                    #endregion

                    #region 角度
                    XGUI.layout_property_field(
                        title: "角度",
                        title_size: XGUIFontSize.M,
                        title_hover_color: Color.white,
                        title_width: 60,
                        prop: Type == HudElementMotionType.Creator ? sp_CreateArgs.FindPropertyRelative("Rotation.Degree") : sp_RecycleArgs.FindPropertyRelative("Rotation.Degree"),
                        prop_padding: new RectOffset(0, 0, 0, 0),
                        prop_margin: new RectOffset(0, 0, 0, 10));
                    #endregion

                    #region 耗时
                    XGUI.layout_property_field(
                        title: "耗时",
                        title_size: XGUIFontSize.M,
                        title_hover_color: Color.white,
                        title_width: 60,
                        prop: Type == HudElementMotionType.Creator ? sp_CreateArgs.FindPropertyRelative("Rotation.Duration") : sp_RecycleArgs.FindPropertyRelative("Rotation.Duration"),
                        prop_padding: new RectOffset(0, 0, 0, 0),
                        prop_margin: new RectOffset(0, 0, 0, 10));
                    #endregion

                    #region 延迟
                    XGUI.layout_property_field(
                        title: "延迟",
                        title_size: XGUIFontSize.M,
                        title_hover_color: Color.white,
                        title_width: 60,
                        prop: Type == HudElementMotionType.Creator ? sp_CreateArgs.FindPropertyRelative("Rotation.Delay") : sp_RecycleArgs.FindPropertyRelative("Rotation.Delay"),
                        prop_padding: new RectOffset(0, 0, 0, 0),
                        prop_margin: new RectOffset(0, 0, 0, 10));
                    #endregion

                    #region 曲线
                    XGUI.layout_property_field(
                        title: "曲线",
                        title_size: XGUIFontSize.M,
                        title_hover_color: Color.white,
                        title_width: 60,
                        prop: Type == HudElementMotionType.Creator ? sp_CreateArgs.FindPropertyRelative("Rotation.Curve") : sp_RecycleArgs.FindPropertyRelative("Rotation.Curve"),
                        prop_padding: new RectOffset(0, 0, 0, 0),
                        prop_margin: new RectOffset(0, 0, 0, 10));
                    #endregion

                    #region 缓动
                    XGUI.layout_property_field(
                        title: "缓动",
                        title_size: XGUIFontSize.M,
                        title_hover_color: Color.white,
                        title_width: 60,
                        prop: Type == HudElementMotionType.Creator ? sp_CreateArgs.FindPropertyRelative("Rotation.Ease") : sp_RecycleArgs.FindPropertyRelative("Rotation.Ease"),
                        prop_padding: new RectOffset(0, 0, 0, 0),
                        prop_margin: new RectOffset(0, 0, 0, 10));
                    #endregion
                    break;
                // -----------  透明度
                case 2:
                    #region 耗时
                    XGUI.layout_property_field(
                        title: "耗时",
                        title_size: XGUIFontSize.M,
                        title_hover_color: Color.white,
                        title_width: 60,
                        prop: Type == HudElementMotionType.Creator ? sp_CreateArgs.FindPropertyRelative("Alpha.Duration") : sp_RecycleArgs.FindPropertyRelative("Alpha.Duration"),
                        prop_padding: new RectOffset(0, 0, 0, 0),
                        prop_margin: new RectOffset(0, 0, 0, 10));
                    #endregion

                    #region 延迟
                    XGUI.layout_property_field(
                        title: "延迟",
                        title_size: XGUIFontSize.M,
                        title_hover_color: Color.white,
                        title_width: 60,
                        prop: Type == HudElementMotionType.Creator ? sp_CreateArgs.FindPropertyRelative("Alpha.Delay") : sp_RecycleArgs.FindPropertyRelative("Alpha.Delay"),
                        prop_padding: new RectOffset(0, 0, 0, 0),
                        prop_margin: new RectOffset(0, 0, 0, 10));
                    #endregion

                    #region 曲线
                    XGUI.layout_property_field(
                        title: "曲线",
                        title_size: XGUIFontSize.M,
                        title_hover_color: Color.white,
                        title_width: 60,
                        prop: Type == HudElementMotionType.Creator ? sp_CreateArgs.FindPropertyRelative("Alpha.Curve") : sp_RecycleArgs.FindPropertyRelative("Alpha.Curve"),
                        prop_padding: new RectOffset(0, 0, 0, 0),
                        prop_margin: new RectOffset(0, 0, 0, 10));
                    #endregion

                    #region 缓动
                    XGUI.layout_property_field(
                        title: "缓动",
                        title_size: XGUIFontSize.M,
                        title_hover_color: Color.white,
                        title_width: 60,
                        prop: Type == HudElementMotionType.Creator ? sp_CreateArgs.FindPropertyRelative("Alpha.Ease") : sp_RecycleArgs.FindPropertyRelative("Alpha.Ease"),
                        prop_padding: new RectOffset(0, 0, 0, 0),
                        prop_margin: new RectOffset(0, 0, 0, 10));
                    #endregion
                    break;
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 动效类型
            XGUI.layout_group_start(
            type: XGUIContainerType.Vertical,
            bg_fill: XGUIFilled.缺口纯色边框,
            bg_color: XGUIColor.亮白,
            bg_color_gui: XHud_Dashboard.Theme_Group,
            title: "回收 & 生成动作",
            title_size: XGUIFontSize.M,
            title_text_color: XHud_Dashboard.Theme_Primary,
            title_clipping: TextClipping.Clip,
            margin: new RectOffset(10, 10, 0, 0),
            padding: new RectOffset(10, 10, 15, 15));

            #region 结束时机
            XGUI.layout_property_field(
                title: "结束时机",
                title_size: XGUIFontSize.M,
                title_hover_color: Color.white,
                title_width: 60,
                prop: Type == HudElementMotionType.Creator ? sp_MotionAnimateEndState_crc : sp_MotionAnimateEndState_rec,
                prop_padding: new RectOffset(0, 0, 0, 0),
                prop_margin: new RectOffset(0, 0, 0, 10));
            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            XGUI.layout_space(10);

            BaseObject.ApplyModifiedProperties();

            Repaint();

            Buttons();

            #region 检测点击事件
            Event e = Event.current;
            if (e.type == EventType.MouseDown)
            {
                // 检查点击位置是否在窗口内
                if (!rect_info.Contains(e.mousePosition))
                {
                    GUI.FocusControl(null); // 取消所有控件的焦点
                    Repaint(); // 重新绘制窗口
                }
            }

            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
            {
                e.Use();
                Close();
            }
            #endregion
        }

        /// <summary>
        /// 发送到库
        /// </summary>
        private void SendToLibrary()
        {
            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

            if (sp_LibName.stringValue == "动效名称")
            {
                XGUI.dialog(
                   type: XGUIDialogType.警告,
                   windowtitle: "XHud - 元素动效库采集器消息",
                   title: "未填写名称",
                   msg: $"请为动效模版添加一个名称！",
                   ok: "明白",
                   PrimaryIndex: 0,
                   usemodal: true,
                   themecolor: XHud_Dashboard.Theme_Primary);
                return;
            }

            string colorhex = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary, true);

            bool exist = Target_Hud_MotionLibrary.ElementMotion_IsExist(sp_LibName.stringValue, Type);

            if (exist)
            {
                string res = XGUI.dialog(
                    type: XGUIDialogType.警告,
                    windowtitle: "XHud - 元素动效库采集器消息",
                    title: "存在重复动效名称",
                    msg: $"名称为<color={colorhex}> {sp_LibName.stringValue} </color>的已经存在于动效库中！",
                    ok: "重命名",
                    PrimaryIndex: 0,
                    usemodal: true,
                    themecolor: XHud_Dashboard.Theme_Primary);

                if (res == "重命名")
                {
                    return;
                }
            }
            else
            {
                XHud_LibraryArg_Motion motion = new XHud_LibraryArg_Motion();
                motion.Name = sp_LibName.stringValue;
                motion.Crc = CreateArgs.Clone();
                motion.Rec = RecycleArgs.Clone();
                motion.Des = sp_Description.stringValue;
                motion.Mode = (int)Type;
                Target_Hud_MotionLibrary.ElementMotion_Add(motion);

                EditorApplication.delayCall += () =>
                {
                    XGUI.dialog(
                        type: XGUIDialogType.确认,
                        windowtitle: "XHud - 元素动效库采集器消息",
                        title: "已添加到动效库",
                        msg: $"已将名称为<color={colorhex}> {sp_LibName.stringValue} </color>的动效参数添加到动效库中！",
                        ok: "明白",
                        PrimaryIndex: 0,
                        usemodal: true,
                        themecolor: XHud_Dashboard.Theme_Primary);
                };
                Close();
            }
        }

        /// <summary>
        /// 从库更新
        /// </summary>
        private void UpdateToLibrary()
        {
            string colorhex = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary, true);

            XHud_LibraryArg_Motion motion = new XHud_LibraryArg_Motion(sp_LibName.stringValue, (int)Type, sp_Description.stringValue, CreateArgs.Clone(), RecycleArgs.Clone());
            Target_Hud_MotionLibrary.ElementMotion_ReplaceMotion(ModifiedIndex, motion);

            Close();
        }

        /// <summary>
        /// 设置动效- 类型
        /// </summary>
        /// <param name="eles"></param>
        public void SetElementMotionType(HudElementMotionType type)
        {
            Type = type;
        }

        /// <summary>
        /// 设置动效- 生成
        /// </summary>
        /// <param name="eles"></param>
        public void SetElementMotion(Motion_Creator creator)
        {
            CreateArgs = creator;
        }

        /// <summary>
        /// 设置动效 - 回收
        /// </summary>
        /// <param name="eles"></param>
        public void SetElementMotion(Motion_Recycler recycle)
        {
            RecycleArgs = recycle;
        }

        /// <summary>
        /// 设置动效 - 全部
        /// </summary>
        public void SetElementMotion(Motion_Creator creator, Motion_Recycler recycle)
        {
            CreateArgs = creator;
            RecycleArgs = recycle;
        }

        /// <summary>
        /// 设置按钮文字
        /// </summary>
        /// <param name="ok"></param>
        /// <param name="cancel"></param>
        public void SetButtonText(string ok, string cancel)
        {
            ButtonText_Cancel = cancel;
            ButtonText_Ok = ok;
        }

        /// <summary>
        /// 设置按钮文字
        /// </summary>
        /// <param name="ok"></param>
        public void SetButtonText(string ok)
        {
            ButtonText_Ok = ok;
        }

        /// <summary>
        /// 设置模版基础信息（针对从库源修改更新使用）
        /// </summary>
        /// <param name="libname"></param>
        /// <param name="decription"></param>
        public void SetInfo(string libname, string decription)
        {
            LibName = libname;
            Description = decription;
        }

        /// <summary>
        /// 设置标题
        /// </summary>
        /// <param name="title"></param>
        public void SetTitle(string title)
        {
            Title = title;
        }

        /// <summary>
        /// 设置为添加到库模式还是修改库源参数模式
        /// </summary>
        /// <param name="mode"></param>
        public void SetLibrarySetterMode(LibrarySetterMode mode)
        {
            LibrarySetterMode = mode;
        }

        /// <summary>
        /// 控件按钮
        /// </summary>
        private void Buttons()
        {
            #region 按钮
            XGUI.layout_group_start(
                type: XGUIContainerType.Horizontal,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(10, 10, 0, 0));

            XGUI.layout_flexspace();

            if (LibrarySetterMode != LibrarySetterMode.修改生成器项参数)
            {
                if (XGUI.layout_button(
                text: ButtonText_Cancel,
                tooltip: "",
                bg_fill: XGUIFilled.实体,
                bg_color: XGUIColor.亮白,
                bg_color_gui: Color.white,
                button_text_color: Color.black,
                press_fill: XGUIFilled.实体,
                press_color: XGUIColor.深空灰,
                press_text_color: Color.white,
                font_size: XGUIFontSize.B,
                anchor: TextAnchor.MiddleCenter,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                layout_min_width: 0,
                layout_width: 200,
                height: ButtonHeight,
                button_text_font: XGUI.GetFont("xg-medium")))
                {
                    Close();
                }

                XGUI.layout_space(ButtonDistance);
            }

            if (XGUI.layout_button(
                text: ButtonText_Ok,
                tooltip: "",
                bg_fill: XGUIFilled.实体,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.Theme_Primary,
                button_text_color: Color.black,
                press_fill: XGUIFilled.实体,
                press_color: XGUIColor.深空灰,
                press_text_color: Color.white,
                font_size: XGUIFontSize.B,
                anchor: TextAnchor.MiddleCenter,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                layout_min_width: 0,
                layout_width: 200,
                height: ButtonHeight,
                button_text_font: XGUI.GetFont("xg-medium")))
            {
                if (LibrarySetterMode == LibrarySetterMode.添加到库)
                {
                    SendToLibrary();
                }
                else if (LibrarySetterMode == LibrarySetterMode.修改库源参数)
                {
                    UpdateToLibrary();
                }
                else if (LibrarySetterMode == LibrarySetterMode.修改生成器项参数)
                {
                    Close();
                }
            }

            XGUI.layout_flexspace();

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            #endregion
        }

        public void SetTarget_Hud_MotionLibrary(XHud_Library_Motion lib)
        {
            Target_Hud_MotionLibrary = lib;
        }
    }
}
