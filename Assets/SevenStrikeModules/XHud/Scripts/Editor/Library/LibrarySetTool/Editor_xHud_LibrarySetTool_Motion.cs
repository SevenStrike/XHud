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

    public class Editor_XHud_LibrarySetTool_Motion : EditorWindow
    {
        #region 字段 - 序列化对象 / 属性
        /// <summary>
        /// 用于序列化当前窗口的自定义字段。
        /// </summary>
        private SerializedObject base_object;
        #endregion

        #region 字段 - 序列化属性
        /// <summary>
        /// 动效名称属性
        /// </summary>
        private SerializedProperty sp_lib_name;
        /// <summary>
        /// 动效说明属性
        /// </summary>
        private SerializedProperty sp_description;
        /// <summary>
        /// 生成参数属性
        /// </summary>
        private SerializedProperty sp_motion_create_args;
        /// <summary>
        /// 回收参数属性
        /// </summary>
        private SerializedProperty sp_motion_recycle_args;
        /// <summary>
        /// 动效类型属性
        /// </summary>
        private SerializedProperty sp_type;
        #endregion

        #region 字段 - 数据
        /// <summary>
        /// 动效类型（生成器 / 回收器）。
        /// </summary>
        [SerializeField]
        private HudElementMotionType type;
        /// <summary>
        /// 动效名称。
        /// </summary>
        [SerializeField]
        public string library_name;
        /// <summary>
        /// 动效说明文字。
        /// </summary>
        [SerializeField]
        public string description;
        /// <summary>
        /// 目标组件的ID（用于目标物体上存在动效设置入口的脚本组件的识别）
        /// </summary>
        [SerializeField]
        public string preedit_target_id;
        /// <summary>
        /// 生成动效参数。
        /// </summary>
        public Motion_Creator motion_create_args;
        /// <summary>
        /// 回收动效参数。
        /// </summary>
        public Motion_Recycler motion_recycle_args;
        /// <summary>
        /// 修改的库项索引。
        /// </summary>
        public int modified_index;
        /// <summary>
        /// 目标元素动效库。
        /// </summary>
        private XHud_Library_Motion target_hud_motion_library;
        /// <summary>
        /// 库设置模式（添加到库 / 修改库源参数 / 修改生成器项参数）。
        /// </summary>
        public LibrarySetterMode library_setter_mode;
        public XHudSpace target_layout_spawner_space;
        public LayoutSpawnerMotionTarget target_layout_spawner_motion_mode;
        public int target_layout_spawner_listindex;
        #endregion

        #region 字段 - 图标
        /// <summary>
        /// 窗口 Logo 图标。
        /// </summary>
        private Texture2D icon;
        /// <summary>
        /// 位移基准释放图标。
        /// </summary>
        private Texture2D icon_libsetter_motion_movement_r;
        /// <summary>
        /// 位移基准按下图标。
        /// </summary>
        private Texture2D icon_libsetter_motion_movement_p;
        /// <summary>
        /// 旋转基准释放图标。
        /// </summary>
        private Texture2D icon_libsetter_motion_rotator_r;
        /// <summary>
        /// 旋转基准按下图标。
        /// </summary>
        private Texture2D icon_libsetter_motion_rotator_p;
        /// <summary>
        /// 透明度基准释放图标。
        /// </summary>
        private Texture2D icon_libsetter_motion_alpha_r;
        /// <summary>
        /// 透明度基准按下图标。
        /// </summary>
        private Texture2D icon_libsetter_motion_alpha_p;
        #endregion

        #region 字段 - 布局
        /// <summary>
        /// 按钮高度。
        /// </summary>
        private float button_height = 25;
        /// <summary>
        /// 按钮间距。
        /// </summary>
        private float button_distance = 15;
        /// <summary>
        /// 确认按钮文字。
        /// </summary>
        public string button_text_ok;
        /// <summary>
        /// 取消按钮文字。
        /// </summary>
        public string button_text_cancel = "取消";
        /// <summary>
        /// 基准按钮图标尺寸。
        /// </summary>
        private float icon_size = 35;
        /// <summary>
        /// 参数面板工具栏当前索引。
        /// </summary>
        private int toolbar_index = 0;
        /// <summary>
        /// 窗口标题。
        /// </summary>
        string title_primary;
        /// <summary>
        /// 窗口副标题。
        /// </summary>
        string title_sub = "以下为目标元素动效模版的参数，可根据需要进行调整";
        /// <summary>
        /// 模版信息区矩形缓存。
        /// </summary>
        Rect rect_info;
        #endregion

        #region 生命周期
        /// <summary>
        /// Unity 启用回调。
        /// <para/>
        /// 执行流程：
        /// <list type="number">
        /// <item><description>构建序列化对象并绑定属性；</description></item>
        /// <item><description>加载全部图标资源；</description></item>
        /// <item><description>初始化默认名称与说明文字。</description></item>
        /// </list>
        /// </summary>
        private void OnEnable()
        {
            base_object = new SerializedObject(this);

            #region 获取序列化属性
            sp_lib_name = base_object.FindProperty(nameof(library_name));
            sp_description = base_object.FindProperty(nameof(description));
            sp_motion_create_args = base_object.FindProperty(nameof(motion_create_args));
            sp_motion_recycle_args = base_object.FindProperty(nameof(motion_recycle_args));
            sp_type = base_object.FindProperty(nameof(type));
            #endregion

            #region 获取图标
            icon = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_motion_setter/icon");
            icon_libsetter_motion_movement_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_motion_setter/icon_libsetter_motion_movement_r");
            icon_libsetter_motion_rotator_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_motion_setter/icon_libsetter_motion_rotator_r");
            icon_libsetter_motion_alpha_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_motion_setter/icon_libsetter_motion_alpha_r");
            icon_libsetter_motion_movement_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_motion_setter/icon_libsetter_motion_movement_p");
            icon_libsetter_motion_rotator_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_motion_setter/icon_libsetter_motion_rotator_p");
            icon_libsetter_motion_alpha_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_motion_setter/icon_libsetter_motion_alpha_p");
            #endregion

            description = "动效参数说明内容";
            library_name = "动效名称";
        }
        /// <summary>
        /// Unity 禁用回调。
        /// <para/>
        /// 当前实现为空，保留以支持后续清理逻辑。
        /// </summary>
        private void OnDisable()
        {

        }
        /// <summary>
        /// Unity 销毁回调。
        /// <para/>
        /// 关闭窗口时让场景视图重新获得焦点。
        /// </summary>
        private void OnDestroy()
        {
            GetWindow<SceneView>().Focus();
        }
        /// <summary>
        /// 绘制窗口 GUI。
        /// <para/>
        /// 按自上而下的顺序绘制：
        /// 抬头 → 结束基准切换 → 模版信息 → 动效类型 → 生成锚点（生成器模式） →
        /// 位移 / 旋转 / 透明度参数（Tab 工具栏） → 结束时机 → 按钮。
        /// <para/>
        /// 支持 Escape 键关闭窗口与点击空白区域失焦。
        /// </summary>
        private void OnGUI()
        {
            base_object.Update();

            TextClipping clipping_ellipsis = XGUI.TryEllipsisClipping();

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
                text: new GUIContent(title_primary),
                text_color: Color.white,
                size: XGUIFontSize.L,
                clipping: clipping_ellipsis,
                font: XGUI.GetFont("xg-heavy"));

            // 分割线
            Rect rect_seperate = new Rect(rect.x + 68, rect.y + 43, 200, 1);
            XGUI.gui_seperator(
                rect: rect_seperate,
                thickness: 1,
                color: XHud_Dashboard.themes_get_color(ThemeColorType.sepline),
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0));

            // 小标题
            Rect rect_subtitle = new Rect(rect.x + 18, rect.y + 53, rect.width, 30);
            XGUI.gui_label(
                rect: rect_subtitle,
                text: new GUIContent(title_sub),
                text_color: Color.white * 0.7f,
                size: XGUIFontSize.M,
                anchor: TextAnchor.UpperLeft,
                clipping: clipping_ellipsis);
            #endregion

            #region 动效类型文字标签
            Rect rect_mark = new Rect(rect.x + (rect.width - 80), rect.y + 15, 65, XGUI.GetSingleLineHeight());
            XGUI.gui_label(
                rect: rect_mark,
                bg_fill: XGUIGroup.实体,
                bg_color: XGUIColor.亮白,
                bg_color_gui: Color.white,
                text: new GUIContent(type.ToString()),
                text_color: Color.black,
                size: XGUIFontSize.S,
                anchor: TextAnchor.MiddleCenter,
                offset: new Vector2(0, -2),
                clipping: clipping_ellipsis);
            #endregion

            #region 动效结束基准状态
            SerializedProperty sp_MotionAnimateEndState_crc = sp_motion_create_args.FindPropertyRelative("MotionAnimateEndState");
            MotionAnimateEndState maes_crc = (MotionAnimateEndState)sp_MotionAnimateEndState_crc.enumValueIndex;

            SerializedProperty sp_MotionAnimateEndState_rec = sp_motion_recycle_args.FindPropertyRelative("MotionAnimateEndState");
            MotionAnimateEndState maes_rec = (MotionAnimateEndState)sp_MotionAnimateEndState_rec.enumValueIndex;
            #endregion

            XGUI.layout_space(100);

            #region 动效结束基准状态 - 选项按钮
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
                    tex_gui_color: (type == HudElementMotionType.Creator ? (maes_crc == MotionAnimateEndState.以_移动为准 ? XHud_Dashboard.themes_get_color(ThemeColorType.primary) : Color.gray) : (maes_rec == MotionAnimateEndState.以_移动为准 ? XHud_Dashboard.themes_get_color(ThemeColorType.primary) : Color.gray)),
                    border: new RectOffset(0, 0, 0, 0),
                    width: icon_size,
                    height: icon_size))
            {
                if (type == HudElementMotionType.Creator)
                {
                    sp_MotionAnimateEndState_crc.enumValueIndex = (int)MotionAnimateEndState.以_移动为准;
                }
                else
                {
                    sp_MotionAnimateEndState_rec.enumValueIndex = (int)MotionAnimateEndState.以_移动为准;
                }
                sp_MotionAnimateEndState_crc.serializedObject.ApplyModifiedProperties();
                sp_MotionAnimateEndState_rec.serializedObject.ApplyModifiedProperties();

                sp_motion_create_args.serializedObject.ApplyModifiedProperties();
                sp_motion_recycle_args.serializedObject.ApplyModifiedProperties();

                CheckMotionEnderValid();
            }

            XGUI.layout_space(80);

            if (XGUI.layout_button(
                     tooltip: "以旋转结束为准",
                     tex_release: icon_libsetter_motion_rotator_r,
                     tex_press: icon_libsetter_motion_rotator_p,
                     tex_gui_color: (type == HudElementMotionType.Creator ? (maes_crc == MotionAnimateEndState.以_旋转为准 ? XHud_Dashboard.themes_get_color(ThemeColorType.primary) : Color.gray) : (maes_rec == MotionAnimateEndState.以_旋转为准 ? XHud_Dashboard.themes_get_color(ThemeColorType.primary) : Color.gray)),
                     border: new RectOffset(0, 0, 0, 0),
                     width: icon_size,
                     height: icon_size))
            {
                if (type == HudElementMotionType.Creator)
                {
                    sp_MotionAnimateEndState_crc.enumValueIndex = (int)MotionAnimateEndState.以_旋转为准;
                }
                else
                {
                    sp_MotionAnimateEndState_rec.enumValueIndex = (int)MotionAnimateEndState.以_旋转为准;
                }
                sp_MotionAnimateEndState_crc.serializedObject.ApplyModifiedProperties();
                sp_MotionAnimateEndState_rec.serializedObject.ApplyModifiedProperties();

                sp_motion_create_args.serializedObject.ApplyModifiedProperties();
                sp_motion_recycle_args.serializedObject.ApplyModifiedProperties();

                CheckMotionEnderValid();
            }

            XGUI.layout_space(80);

            if (XGUI.layout_button(
                    tooltip: "以透明度结束为准",
                    tex_release: icon_libsetter_motion_alpha_r,
                    tex_press: icon_libsetter_motion_alpha_p,
                    tex_gui_color: (type == HudElementMotionType.Creator ? (maes_crc == MotionAnimateEndState.以_透明度为准 ? XHud_Dashboard.themes_get_color(ThemeColorType.primary) : Color.gray) : (maes_rec == MotionAnimateEndState.以_透明度为准 ? XHud_Dashboard.themes_get_color(ThemeColorType.primary) : Color.gray)),
                    border: new RectOffset(0, 0, 0, 0),
                    width: icon_size,
                    height: icon_size))
            {
                if (type == HudElementMotionType.Creator)
                {
                    sp_MotionAnimateEndState_crc.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
                }
                else
                {
                    sp_MotionAnimateEndState_rec.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
                }

                sp_MotionAnimateEndState_crc.serializedObject.ApplyModifiedProperties();
                sp_MotionAnimateEndState_rec.serializedObject.ApplyModifiedProperties();

                sp_motion_create_args.serializedObject.ApplyModifiedProperties();
                sp_motion_recycle_args.serializedObject.ApplyModifiedProperties();

                CheckMotionEnderValid();
            }

            XGUI.layout_flexspace();

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            #endregion

            XGUI.layout_space(15);

            #region 动效信息 - 只有模式为“添加到库”和 “修改库源参数”这两种状态才会显示
            if (library_setter_mode == LibrarySetterMode.添加到库 || library_setter_mode == LibrarySetterMode.修改库源参数)
            {
                XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "模版信息",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                margin: new RectOffset(10, 10, 0, 0),
                padding: new RectOffset(10, 10, 15, 15));

                rect_info = XGUI.GetLastRect();

                #region 名称
                if (string.IsNullOrEmpty(sp_lib_name.stringValue))
                {
                    sp_lib_name.stringValue = "文字样式名称";
                }
                sp_lib_name.stringValue = XGUI.layout_inputfield(
                    title: "名称",
                    prop: sp_lib_name.stringValue,
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
                sp_lib_name.serializedObject.ApplyModifiedProperties();
                #endregion

                #region 说明
                if (string.IsNullOrEmpty(sp_description.stringValue))
                {
                    sp_description.stringValue = "文字样式参数说明内容";
                }
                sp_description.stringValue = XGUI.layout_inputfield(
                    title: "说明",
                    prop: sp_description.stringValue,
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
                sp_description.serializedObject.ApplyModifiedProperties();
                #endregion

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            }
            #endregion            

            #region 动效类型下拉菜单
            XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "动效类型",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                margin: new RectOffset(10, 10, 0, 0),
                padding: new RectOffset(10, 10, 15, 15));

            #region 动效类型
            XGUI.layout_property_field(
                title: "动效类型",
                title_size: XGUIFontSize.M,
                title_hover_color: Color.white,
                title_width: 60,
                prop: sp_type,
                prop_padding: new RectOffset(0, 0, 0, 0),
                prop_margin: new RectOffset(0, 0, 0, 10));
            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 生成锚点 - 只有动效类型是“生成”才会显示
            if (type == HudElementMotionType.Creator)
            {
                XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIGroup.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                    title: "生成锚点",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_clipping: TextClipping.Clip,
                    margin: new RectOffset(10, 10, 0, 0),
                    padding: new RectOffset(10, 10, 15, 15));

                #region 锚点
                XGUI.layout_property_field(
                    title: "锚点",
                    title_size: XGUIFontSize.M,
                    title_hover_color: Color.white,
                    title_width: 60,
                    prop: sp_motion_create_args.FindPropertyRelative("anchor"),
                    prop_padding: new RectOffset(0, 0, 0, 0),
                    prop_margin: new RectOffset(0, 0, 0, 10));
                #endregion

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            }
            #endregion

            #region 动效参数
            XGUI.layout_group_start(
               type: XGUIContainerType.Vertical,
               bg_fill: XGUIGroup.缺口纯色边框,
               bg_color: XGUIColor.亮白,
               bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
               title: "动效参数",
               title_size: XGUIFontSize.M,
               title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
               title_clipping: TextClipping.Clip,
               margin: new RectOffset(10, 10, 0, 0),
               padding: new RectOffset(10, 10, 15, 15));

            #region 动效参数分类面板
            toolbar_index = XGUI.layout_toolbar(
                index: ref toolbar_index,
                names: new string[] { "位移", "旋转", "透明度" },
                bg_normal: XGUIGroup.无,
                bg_selected: XGUIGroup.实体,
                bg_color: XGUIColor.亮白,
                bg_gui_color: Color.black * 0.5f,
                text_color_normal: Color.white,
                text_color_selected: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
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
                navigate_style_bg: XGUIGroup.纯色边框,
                navigate_style_bg_color: Color.black * 0.5f);

            // 临时转成枚举值便于阅读 Switch 代码逻辑
            HudElementMotionMode mode = (HudElementMotionMode)toolbar_index;

            switch (mode)
            {
                // 位移
                case HudElementMotionMode.Movement:
                    #region 方式
                    XGUI.layout_property_field(
                        title: "方式",
                        title_size: XGUIFontSize.M,
                        title_hover_color: Color.white,
                        title_width: 60,
                        prop: type == HudElementMotionType.Creator ? sp_motion_create_args.FindPropertyRelative("Movement.Movement") : sp_motion_recycle_args.FindPropertyRelative("Movement.Movement"),
                        prop_padding: new RectOffset(0, 0, 0, 0),
                        prop_margin: new RectOffset(0, 0, 0, 10));
                    #endregion

                    #region 距离
                    XGUI.layout_property_field(
                        title: "距离",
                        title_size: XGUIFontSize.M,
                        title_hover_color: Color.white,
                        title_width: 60,
                        prop: type == HudElementMotionType.Creator ? sp_motion_create_args.FindPropertyRelative("Movement.Distance") : sp_motion_recycle_args.FindPropertyRelative("Movement.Distance"),
                        prop_padding: new RectOffset(0, 0, 0, 0),
                        prop_margin: new RectOffset(0, 0, 0, 10));
                    #endregion

                    #region 耗时
                    XGUI.layout_property_field(
                        title: "耗时",
                        title_size: XGUIFontSize.M,
                        title_hover_color: Color.white,
                        title_width: 60,
                        prop: type == HudElementMotionType.Creator ? sp_motion_create_args.FindPropertyRelative("Movement.Duration") : sp_motion_recycle_args.FindPropertyRelative("Movement.Duration"),
                        prop_padding: new RectOffset(0, 0, 0, 0),
                        prop_margin: new RectOffset(0, 0, 0, 10));
                    #endregion

                    #region 延迟
                    XGUI.layout_property_field(
                        title: "延迟",
                        title_size: XGUIFontSize.M,
                        title_hover_color: Color.white,
                        title_width: 60,
                        prop: type == HudElementMotionType.Creator ? sp_motion_create_args.FindPropertyRelative("Movement.Delay") : sp_motion_recycle_args.FindPropertyRelative("Movement.Delay"),
                        prop_padding: new RectOffset(0, 0, 0, 0),
                        prop_margin: new RectOffset(0, 0, 0, 10));
                    #endregion

                    #region 曲线
                    XGUI.layout_property_field(
                        title: "曲线",
                        title_size: XGUIFontSize.M,
                        title_hover_color: Color.white,
                        title_width: 60,
                        prop: type == HudElementMotionType.Creator ? sp_motion_create_args.FindPropertyRelative("Movement.Curve") : sp_motion_recycle_args.FindPropertyRelative("Movement.Curve"),
                        prop_padding: new RectOffset(0, 0, 0, 0),
                        prop_margin: new RectOffset(0, 0, 0, 10));
                    #endregion

                    #region 缓动
                    XGUI.layout_property_field(
                        title: "缓动",
                        title_size: XGUIFontSize.M,
                        title_hover_color: Color.white,
                        title_width: 60,
                        prop: type == HudElementMotionType.Creator ? sp_motion_create_args.FindPropertyRelative("Movement.Ease") : sp_motion_recycle_args.FindPropertyRelative("Movement.Ease"),
                        prop_padding: new RectOffset(0, 0, 0, 0),
                        prop_margin: new RectOffset(0, 0, 0, 10));
                    #endregion
                    break;
                // 旋转
                case HudElementMotionMode.Rotation:
                    #region 方式
                    XGUI.layout_property_field(
                        title: "方式",
                        title_size: XGUIFontSize.M,
                        title_hover_color: Color.white,
                        title_width: 60,
                        prop: type == HudElementMotionType.Creator ? sp_motion_create_args.FindPropertyRelative("Rotation.Rotation") : sp_motion_recycle_args.FindPropertyRelative("Rotation.Rotation"),
                        prop_padding: new RectOffset(0, 0, 0, 0),
                        prop_margin: new RectOffset(0, 0, 0, 10));
                    #endregion

                    #region 角度
                    XGUI.layout_property_field(
                        title: "角度",
                        title_size: XGUIFontSize.M,
                        title_hover_color: Color.white,
                        title_width: 60,
                        prop: type == HudElementMotionType.Creator ? sp_motion_create_args.FindPropertyRelative("Rotation.Degree") : sp_motion_recycle_args.FindPropertyRelative("Rotation.Degree"),
                        prop_padding: new RectOffset(0, 0, 0, 0),
                        prop_margin: new RectOffset(0, 0, 0, 10));
                    #endregion

                    #region 耗时
                    XGUI.layout_property_field(
                        title: "耗时",
                        title_size: XGUIFontSize.M,
                        title_hover_color: Color.white,
                        title_width: 60,
                        prop: type == HudElementMotionType.Creator ? sp_motion_create_args.FindPropertyRelative("Rotation.Duration") : sp_motion_recycle_args.FindPropertyRelative("Rotation.Duration"),
                        prop_padding: new RectOffset(0, 0, 0, 0),
                        prop_margin: new RectOffset(0, 0, 0, 10));
                    #endregion

                    #region 延迟
                    XGUI.layout_property_field(
                        title: "延迟",
                        title_size: XGUIFontSize.M,
                        title_hover_color: Color.white,
                        title_width: 60,
                        prop: type == HudElementMotionType.Creator ? sp_motion_create_args.FindPropertyRelative("Rotation.Delay") : sp_motion_recycle_args.FindPropertyRelative("Rotation.Delay"),
                        prop_padding: new RectOffset(0, 0, 0, 0),
                        prop_margin: new RectOffset(0, 0, 0, 10));
                    #endregion

                    #region 曲线
                    XGUI.layout_property_field(
                        title: "曲线",
                        title_size: XGUIFontSize.M,
                        title_hover_color: Color.white,
                        title_width: 60,
                        prop: type == HudElementMotionType.Creator ? sp_motion_create_args.FindPropertyRelative("Rotation.Curve") : sp_motion_recycle_args.FindPropertyRelative("Rotation.Curve"),
                        prop_padding: new RectOffset(0, 0, 0, 0),
                        prop_margin: new RectOffset(0, 0, 0, 10));
                    #endregion

                    #region 缓动
                    XGUI.layout_property_field(
                        title: "缓动",
                        title_size: XGUIFontSize.M,
                        title_hover_color: Color.white,
                        title_width: 60,
                        prop: type == HudElementMotionType.Creator ? sp_motion_create_args.FindPropertyRelative("Rotation.Ease") : sp_motion_recycle_args.FindPropertyRelative("Rotation.Ease"),
                        prop_padding: new RectOffset(0, 0, 0, 0),
                        prop_margin: new RectOffset(0, 0, 0, 10));
                    #endregion
                    break;
                // 透明度
                case HudElementMotionMode.Alpha:
                    #region 耗时
                    XGUI.layout_property_field(
                        title: "耗时",
                        title_size: XGUIFontSize.M,
                        title_hover_color: Color.white,
                        title_width: 60,
                        prop: type == HudElementMotionType.Creator ? sp_motion_create_args.FindPropertyRelative("Alpha.Duration") : sp_motion_recycle_args.FindPropertyRelative("Alpha.Duration"),
                        prop_padding: new RectOffset(0, 0, 0, 0),
                        prop_margin: new RectOffset(0, 0, 0, 10));
                    #endregion

                    #region 延迟
                    XGUI.layout_property_field(
                        title: "延迟",
                        title_size: XGUIFontSize.M,
                        title_hover_color: Color.white,
                        title_width: 60,
                        prop: type == HudElementMotionType.Creator ? sp_motion_create_args.FindPropertyRelative("Alpha.Delay") : sp_motion_recycle_args.FindPropertyRelative("Alpha.Delay"),
                        prop_padding: new RectOffset(0, 0, 0, 0),
                        prop_margin: new RectOffset(0, 0, 0, 10));
                    #endregion

                    #region 曲线
                    XGUI.layout_property_field(
                        title: "曲线",
                        title_size: XGUIFontSize.M,
                        title_hover_color: Color.white,
                        title_width: 60,
                        prop: type == HudElementMotionType.Creator ? sp_motion_create_args.FindPropertyRelative("Alpha.Curve") : sp_motion_recycle_args.FindPropertyRelative("Alpha.Curve"),
                        prop_padding: new RectOffset(0, 0, 0, 0),
                        prop_margin: new RectOffset(0, 0, 0, 10));
                    #endregion

                    #region 缓动
                    XGUI.layout_property_field(
                        title: "缓动",
                        title_size: XGUIFontSize.M,
                        title_hover_color: Color.white,
                        title_width: 60,
                        prop: type == HudElementMotionType.Creator ? sp_motion_create_args.FindPropertyRelative("Alpha.Ease") : sp_motion_recycle_args.FindPropertyRelative("Alpha.Ease"),
                        prop_padding: new RectOffset(0, 0, 0, 0),
                        prop_margin: new RectOffset(0, 0, 0, 10));
                    #endregion
                    break;
            }
            #endregion

            #region 参数重置按钮
            if (XGUI.layout_button(
                text: "重置",
                tooltip: "",
                bg_fill: XGUIGroup.实体,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                button_text_color: Color.black,
                press_fill: XGUIGroup.实体,
                press_color: XGUIColor.深空灰,
                press_text_color: Color.white,
                font_size: XGUIFontSize.B,
                anchor: TextAnchor.MiddleCenter,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                height: button_height,
                button_text_font: XGUI.GetFont("xg-medium")))
            {
                if (type == HudElementMotionType.Creator)
                {
                    motion_reset(sp_motion_create_args);
                }
                else
                {
                    motion_reset(sp_motion_recycle_args);
                }
            }
            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 动效回收 & 生成动作 - 时机选择
            XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "回收 & 生成动作",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                margin: new RectOffset(10, 10, 0, 0),
                padding: new RectOffset(10, 10, 15, 15));

            #region 结束时机
            XGUI.ChangedCheck_Start();

            XGUI.layout_property_field(
                title: "结束时机",
                title_size: XGUIFontSize.M,
                title_hover_color: Color.white,
                title_width: 60,
                prop: type == HudElementMotionType.Creator ? sp_MotionAnimateEndState_crc : sp_MotionAnimateEndState_rec,
                prop_padding: new RectOffset(0, 0, 0, 0),
                prop_margin: new RectOffset(0, 0, 0, 10));

            if (XGUI.ChangedCheck_End())
            {
                CheckMotionEnderValid();
            }
            #endregion            

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            XGUI.layout_space(10);

            base_object.ApplyModifiedProperties();

            Repaint();

            bottom_buttons();

            #region 检测事件
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
        #endregion

        #region 辅助
        private void CheckMotionEnderValid()
        {
            string hexcol = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.themes_get_color(ThemeColorType.primary), true);

            SerializedProperty sp_ender = type == HudElementMotionType.Creator ?
                sp_motion_create_args.FindPropertyRelative("MotionAnimateEndState") :
                sp_motion_recycle_args.FindPropertyRelative("MotionAnimateEndState");

            MotionAnimateEndState state = type == HudElementMotionType.Creator ? (MotionAnimateEndState)sp_ender.enumValueIndex : (MotionAnimateEndState)sp_ender.enumValueIndex;
            switch (state)
            {
                case MotionAnimateEndState.以_移动为准:
                    HudMotion_Movement m_m = type == HudElementMotionType.Creator ? (HudMotion_Movement)sp_motion_create_args.FindPropertyRelative("Movement.Movement").enumValueIndex : (HudMotion_Movement)sp_motion_recycle_args.FindPropertyRelative("Movement.Movement").enumValueIndex;

                    if (m_m == HudMotion_Movement.A_无运动)
                    {

                        if (type == HudElementMotionType.Creator)
                            sp_ender.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
                        else
                            sp_ender.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;

                        EditorApplication.delayCall += () =>
                        {
                            XGUI.dialog(
                                type: XGUIDialogType.警告,
                                windowtitle: "XHud - 元素动效工具消息",
                                title: "设定动画结束时机",
                                msg: $"当前位移方式为 <color={hexcol}> A_无运动 </color>，将发生<color={hexcol}> 动效动画无法正常结束</color>从而导致<color={hexcol}> 相应的事件和动作委托 </color>不能正确触发的情况 </color>！请将<color={hexcol}> 位移方式 </color>改为<color={hexcol}> 非无运动方式 </color>！",
                                ok: "明白",
                                PrimaryIndex: 0,
                                usemodal: true,
                                themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                        };
                    }
                    break;
                case MotionAnimateEndState.以_旋转为准:
                    HudMotion_Rotation m_r = type == HudElementMotionType.Creator ? (HudMotion_Rotation)sp_motion_create_args.FindPropertyRelative("Rotation.Rotation").enumValueIndex : (HudMotion_Rotation)sp_motion_recycle_args.FindPropertyRelative("Rotation.Rotation").enumValueIndex;
                    if (m_r == HudMotion_Rotation.A_无旋转)
                    {
                        if (type == HudElementMotionType.Creator)
                            sp_ender.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
                        else
                            sp_ender.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;

                        EditorApplication.delayCall += () =>
                        {
                            XGUI.dialog(
                              type: XGUIDialogType.警告,
                              windowtitle: "XHud - 元素动效工具消息",
                              title: "设定动画结束时机",
                              msg: $"当前旋转方式为 <color={hexcol}> A_无旋转 </color>，将发生<color={hexcol}> 动效动画无法正常结束</color>从而导致<color={hexcol}> 相应的事件和动作委托 </color>不能正确触发的情况 </color>！请将<color={hexcol}> 旋转方式 </color>改为<color={hexcol}> 非无旋转方式 </color>！",
                              ok: "明白",
                              PrimaryIndex: 0,
                              usemodal: true,
                              themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                        };
                    }
                    break;
            }
        }
        #endregion

        #region 工具 - 数据操作
        /// <summary>
        /// 发送动效到目标动效库（添加到库模式）。
        /// <para/>
        /// 执行流程：
        /// <list type="number">
        /// <item><description>校验名称是否为空；</description></item>
        /// <item><description>检测名称是否重复；</description></item>
        /// <item><description>构造动效数据并添加到目标库；</description></item>
        /// <item><description>弹窗提示添加完成并关闭窗口。</description></item>
        /// </list>
        /// </summary>
        private void send_to_library()
        {
            XHud_Manager mgr = XHud_Dashboard.get_manager();

            if (sp_lib_name.stringValue == "动效名称")
            {
                XGUI.dialog(
                   type: XGUIDialogType.警告,
                   windowtitle: "XHud - 元素动效库采集器消息",
                   title: "未填写名称",
                   msg: $"请为动效模版添加一个名称！",
                   ok: "明白",
                   PrimaryIndex: 0,
                   usemodal: true,
                   themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                return;
            }

            string colorhex = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.themes_get_color(ThemeColorType.primary), true);

            bool exist = target_hud_motion_library.ElementMotion_IsExist(sp_lib_name.stringValue, type);

            if (exist)
            {
                string res = XGUI.dialog(
                    type: XGUIDialogType.警告,
                    windowtitle: "XHud - 元素动效库采集器消息",
                    title: "存在重复动效名称",
                    msg: $"名称为<color={colorhex}> {sp_lib_name.stringValue} </color>的已经存在于动效库中！",
                    ok: "重命名",
                    PrimaryIndex: 0,
                    usemodal: true,
                    themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));

                if (res == "重命名")
                {
                    return;
                }
            }
            else
            {
                XHud_LibraryArg_Motion motion = new XHud_LibraryArg_Motion();
                motion.Name = sp_lib_name.stringValue;
                motion.Crc = motion_create_args.Clone();
                motion.Rec = motion_recycle_args.Clone();
                motion.Des = sp_description.stringValue;
                motion.Mode = (int)type;
                target_hud_motion_library.ElementMotion_Add(motion);

                EditorApplication.delayCall += () =>
                {
                    XGUI.dialog(
                        type: XGUIDialogType.确认,
                        windowtitle: "XHud - 元素动效库采集器消息",
                        title: "已添加到动效库",
                        msg: $"已将名称为<color={colorhex}> {sp_lib_name.stringValue} </color>的动效参数添加到动效库中！",
                        ok: "明白",
                        PrimaryIndex: 0,
                        usemodal: true,
                        themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                };
                Close();
            }
        }
        /// <summary>
        /// 更新到目标动效库（修改库源参数模式）。
        /// <para/>
        /// 用当前编辑的名称、类型、说明与动效参数替换目标库中指定索引的动效项，然后关闭窗口。
        /// </summary>
        private void update_to_library()
        {
            string colorhex = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.themes_get_color(ThemeColorType.primary), true);

            XHud_LibraryArg_Motion motion = new XHud_LibraryArg_Motion(sp_lib_name.stringValue, (int)type, sp_description.stringValue, motion_create_args.Clone(), motion_recycle_args.Clone());
            target_hud_motion_library.ElementMotion_ReplaceMotion(modified_index, motion);

            Close();
        }
        /// <summary>
        /// 重置动效的参数到默认
        /// </summary>
        private void motion_reset(SerializedProperty prop)
        {
            if (prop.type == "Motion_Creator")
                prop.FindPropertyRelative("anchor").enumValueIndex = (int)XHudAnchor.中心;

            prop.FindPropertyRelative("Movement.Movement").enumValueIndex = prop.type == "Motion_Creator" ? (int)HudMotion_Movement.S_从下至上 : (int)HudMotion_Movement.D_从上至下;
            prop.FindPropertyRelative("Movement.Distance").floatValue = 100;
            prop.FindPropertyRelative("Movement.Duration").floatValue = 1;
            prop.FindPropertyRelative("Movement.Delay").floatValue = 0;
            prop.FindPropertyRelative("Movement.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
            prop.FindPropertyRelative("Movement.CurveName").stringValue = "";
            prop.FindPropertyRelative("Movement.Ease").enumValueIndex = (int)EaseMode.InOutCubic;

            prop.FindPropertyRelative("Rotation.Rotation").enumValueIndex = (int)HudMotion_Rotation.A_无旋转;
            prop.FindPropertyRelative("Rotation.Degree").floatValue = 0;
            prop.FindPropertyRelative("Rotation.Duration").floatValue = 1;
            prop.FindPropertyRelative("Rotation.Delay").floatValue = 0;
            prop.FindPropertyRelative("Rotation.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
            prop.FindPropertyRelative("Rotation.CurveName").stringValue = "";
            prop.FindPropertyRelative("Rotation.Ease").enumValueIndex = (int)EaseMode.InOutCubic;

            prop.FindPropertyRelative("Alpha.Duration").floatValue = 1;
            prop.FindPropertyRelative("Alpha.Delay").floatValue = 0;
            prop.FindPropertyRelative("Alpha.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
            prop.FindPropertyRelative("Alpha.CurveName").stringValue = "";
            prop.FindPropertyRelative("Alpha.Ease").enumValueIndex = (int)EaseMode.InOutCubic;

            prop.FindPropertyRelative("MotionAnimateEndState").enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
            prop.serializedObject.ApplyModifiedProperties();
        }
        #endregion

        #region 设置接口
        /// <summary>
        /// 设置动效类型。
        /// </summary>
        /// <param name="type">动效类型（生成器 / 回收器）</param>
        public void set_motion_type(HudElementMotionType type)
        {
            this.type = type;
        }
        /// <summary>
        /// 同时设置生成与回收动效参数。
        /// </summary>
        /// <param name="creator">生成动效参数</param>
        /// <param name="recycle">回收动效参数</param>
        public void set_motion(Motion_Creator creator, Motion_Recycler recycle)
        {
            motion_create_args = creator;
            motion_recycle_args = recycle;
        }
        /// <summary>
        /// 设置生成动效参数。
        /// </summary>
        /// <param name="creator">生成动效参数</param>
        public void set_motion(Motion_Creator creator)
        {
            type = HudElementMotionType.Creator;
            motion_create_args = creator;
        }
        /// <summary>
        /// 设置回收动效参数。
        /// </summary>
        /// <param name="recycle">回收动效参数</param>
        public void set_motion(Motion_Recycler recycle)
        {
            type = HudElementMotionType.Recycler;
            motion_recycle_args = recycle;
        }
        /// <summary>
        /// 设置按钮文字（确认 / 取消）。
        /// </summary>
        /// <param name="ok">确认按钮文字</param>
        /// <param name="cancel">取消按钮文字</param>
        public void set_button_text(string ok, string cancel)
        {
            button_text_cancel = cancel;
            button_text_ok = ok;
        }
        /// <summary>
        /// 设置确认按钮文字。
        /// </summary>
        /// <param name="ok">确认按钮文字</param>
        public void set_button_text(string ok)
        {
            button_text_ok = ok;
        }
        /// <summary>
        /// 设置模版基础信息（针对从库源修改更新使用）。
        /// </summary>
        /// <param name="libname">动效名称</param>
        /// <param name="decription">动效说明</param>
        public void set_info(string libname, string decription)
        {
            library_name = libname;
            description = decription;
        }
        /// <summary>
        /// 设置标题。
        /// </summary>
        /// <param name="title">标题文字</param>
        public void set_title(string title)
        {
            title_primary = title;
        }
        /// <summary>
        /// 目标组件的ID设置
        /// </summary>
        /// <param name="id">id</param>
        public void set_preedit_target_id(string id)
        {
            preedit_target_id = id;
        }
        /// <summary>
        /// 设置库设置模式（添加到库 / 修改库源参数 / 修改生成器项参数）。
        /// </summary>
        /// <param name="mode">库设置模式</param>
        public void set_library_setter_mode(LibrarySetterMode mode)
        {
            library_setter_mode = mode;
        }
        public void set_target_index(int index)
        {
            target_layout_spawner_listindex = index;
        }
        public void set_target_space(XHudSpace space)
        {
            target_layout_spawner_space = space;
        }
        public void set_target_motion_mode(LayoutSpawnerMotionTarget mode)
        {
            target_layout_spawner_motion_mode = mode;
        }
        /// <summary>
        /// 设置目标元素动效库。
        /// </summary>
        /// <param name="lib">目标动效库</param>
        public void set_target_motion_library(XHud_Library_Motion lib)
        {
            target_hud_motion_library = lib;
        }
        #endregion

        #region 底部按钮
        /// <summary>
        /// 绘制底部按钮区（取消 / 确认）。
        /// <para/>
        /// 确认按钮根据 <see cref="library_setter_mode"/> 决定调用
        /// <see cref="send_to_library"/> / <see cref="update_to_library"/> 或直接关闭窗口。
        /// </summary>
        private void bottom_buttons()
        {
            #region 底部按钮
            XGUI.layout_group_start(
                type: XGUIContainerType.Horizontal,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(10, 10, 0, 0));

            XGUI.layout_flexspace();

            #region 取消按钮
            if (XGUI.layout_button(
            text: button_text_cancel,
            tooltip: "",
            bg_fill: XGUIGroup.实体,
            bg_color: XGUIColor.亮白,
            bg_color_gui: Color.white,
            button_text_color: Color.black,
            press_fill: XGUIGroup.实体,
            press_color: XGUIColor.深空灰,
            press_text_color: Color.white,
            font_size: XGUIFontSize.B,
            anchor: TextAnchor.MiddleCenter,
            margin: new RectOffset(0, 0, 0, 0),
            padding: new RectOffset(0, 0, 0, 0),
            layout_min_width: 0,
            layout_width: 200,
            height: button_height,
            button_text_font: XGUI.GetFont("xg-medium")))
            {
                switch (library_setter_mode)
                {
                    case LibrarySetterMode.添加到库:
                        Close();
                        break;
                    case LibrarySetterMode.修改库源参数:
                        Close();
                        break;
                    case LibrarySetterMode.修改动效参数:
                        Close();
                        break;
                }
            }
            #endregion

            XGUI.layout_space(button_distance);

            #region 确认按钮
            if (XGUI.layout_button(
                text: button_text_ok,
                tooltip: "",
                bg_fill: XGUIGroup.实体,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                button_text_color: Color.black,
                press_fill: XGUIGroup.实体,
                press_color: XGUIColor.深空灰,
                press_text_color: Color.white,
                font_size: XGUIFontSize.B,
                anchor: TextAnchor.MiddleCenter,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                layout_min_width: 0,
                layout_width: 200,
                height: button_height,
                button_text_font: XGUI.GetFont("xg-medium")))
            {
                switch (library_setter_mode)
                {
                    case LibrarySetterMode.添加到库:
                        send_to_library();
                        break;
                    case LibrarySetterMode.修改库源参数:
                        update_to_library();
                        break;
                    case LibrarySetterMode.修改动效参数:
                        Object target = EditorUtility.InstanceIDToObject(int.Parse(preedit_target_id));

                        #region 根据不同的类型将新的动效参数赋值回去
                        // - XHud_Spawner
                        if (target is XHud_Spawner spawner)
                        {
                            Undo.RecordObject(spawner, "修改元素生成器动效参数");

                            if (type == HudElementMotionType.Creator)
                                spawner.motion_create_args = motion_create_args.Clone();
                            else
                                spawner.motion_recycle_args = motion_recycle_args.Clone();

                            EditorUtility.SetDirty(spawner);

                            if (PrefabUtility.IsPartOfPrefabInstance(spawner))
                                PrefabUtility.RecordPrefabInstancePropertyModifications(spawner);

                            EditorApplication.delayCall += () =>
                            {
                                string hexcol = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.themes_get_color(ThemeColorType.primary), true);

                                XGUI.dialog(
                                    type: XGUIDialogType.确认,
                                    windowtitle: "XHud - 动效设置工具消息",
                                    title: "动效更新完成",
                                    msg: $"您刚刚更新了 <color={hexcol}>{spawner.gameObject.name} {(string.IsNullOrEmpty(spawner.spawner_indicator) ? "" : " - " + spawner.spawner_indicator)} </color>元素生成器的 <color={hexcol}>{type} </color>参数！",
                                    ok: "明白",
                                    PrimaryIndex: 0,
                                    usemodal: true,
                                    themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                            };
                        }

                        // - XHud_Module_Element
                        else if (target is XHud_Module_Element ele)
                        {
                            Undo.RecordObject(ele, "修改元素动效参数");

                            if (type == HudElementMotionType.Creator)
                                ele.motion_create_args = motion_create_args.Clone();
                            else
                                ele.motion_recycle_args = motion_recycle_args.Clone();

                            EditorUtility.SetDirty(ele);

                            if (PrefabUtility.IsPartOfPrefabInstance(ele))
                                PrefabUtility.RecordPrefabInstancePropertyModifications(ele);

                            EditorApplication.delayCall += () =>
                            {
                                string hexcol = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.themes_get_color(ThemeColorType.primary), true);

                                XGUI.dialog(
                                    type: XGUIDialogType.确认,
                                    windowtitle: "XHud - 动效设置工具消息",
                                    title: "动效更新完成",
                                    msg: $"您刚刚更新了 <color={hexcol}>{ele.gameObject.name} {(string.IsNullOrEmpty(ele.indicator) ? "" : " - " + ele.indicator)} </color>元素的 <color={hexcol}>{type} </color>参数！",
                                    ok: "明白",
                                    PrimaryIndex: 0,
                                    usemodal: true,
                                    themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                            };
                        }

                        // - XHud_Element_Preview
                        else if (target is XHud_Element_Preview ele_preview)
                        {
                            Undo.RecordObject(ele_preview, "修改元素预览器动效参数");

                            if (type == HudElementMotionType.Creator)
                                ele_preview.motion_create_args = motion_create_args.Clone();
                            else
                                ele_preview.motion_recycle_args = motion_recycle_args.Clone();

                            EditorUtility.SetDirty(ele_preview);

                            if (PrefabUtility.IsPartOfPrefabInstance(ele_preview))
                                PrefabUtility.RecordPrefabInstancePropertyModifications(ele_preview);

                            EditorApplication.delayCall += () =>
                            {
                                string hexcol = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.themes_get_color(ThemeColorType.primary), true);

                                XGUI.dialog(
                                    type: XGUIDialogType.确认,
                                    windowtitle: "XHud - 动效设置工具消息",
                                    title: "动效更新完成",
                                    msg: $"您刚刚更新了 <color={hexcol}>{ele_preview.gameObject.name} </color> 元素预览器的动效 <color={hexcol}>{type} </color> 参数！",
                                    ok: "明白",
                                    PrimaryIndex: 0,
                                    usemodal: true,
                                    themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                            };
                        }

                        // - XHud_LayoutSpawner
                        else if (target is XHud_LayoutSpawner layout_spawner)
                        {
                            Undo.RecordObject(layout_spawner, "修改布局元素生成器动效参数");

                            if (target_layout_spawner_motion_mode == LayoutSpawnerMotionTarget.自身动效)
                            {
                                layout_spawner.motion_create_args = motion_create_args.Clone();
                                layout_spawner.motion_recycle_args = motion_recycle_args.Clone();
                            }
                            else
                            {
                                if (target_layout_spawner_space == XHudSpace.屏幕空间)
                                {
                                    layout_spawner.spawn_item_list_screen[target_layout_spawner_listindex].CreateArgs = motion_create_args.Clone();
                                    layout_spawner.spawn_item_list_screen[target_layout_spawner_listindex].RecycleArgs = motion_recycle_args.Clone();
                                }
                                else
                                {
                                    layout_spawner.spawn_item_list_world[target_layout_spawner_listindex].CreateArgs = motion_create_args.Clone();
                                    layout_spawner.spawn_item_list_world[target_layout_spawner_listindex].RecycleArgs = motion_recycle_args.Clone();
                                }
                            }
                            EditorUtility.SetDirty(layout_spawner);

                            if (PrefabUtility.IsPartOfPrefabInstance(layout_spawner))
                                PrefabUtility.RecordPrefabInstancePropertyModifications(layout_spawner);

                            EditorApplication.delayCall += () =>
                            {
                                string hexcol = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.themes_get_color(ThemeColorType.primary), true);

                                string mode = target_layout_spawner_space == XHudSpace.屏幕空间 ? "屏幕元素列表项" : "世界元素列表项";

                                XGUI.dialog(
                                    type: XGUIDialogType.确认,
                                    windowtitle: "XHud - 动效设置工具消息",
                                    title: "动效更新完成",
                                    msg: $"您刚刚更新了 <color={hexcol}>{layout_spawner.gameObject.name} {(string.IsNullOrEmpty(layout_spawner.spawner_indicator) ? "" : " - " + layout_spawner.spawner_indicator)} </color>布局元素生成器的 <color={hexcol}>{(target_layout_spawner_motion_mode == LayoutSpawnerMotionTarget.自身动效 ? type : "")} </color>{(target_layout_spawner_motion_mode == LayoutSpawnerMotionTarget.自身动效 ? "" : mode)}的动效参数！",
                                    ok: "明白",
                                    PrimaryIndex: 0,
                                    usemodal: true,
                                    themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                            };
                        }

                        // - XHud_Manager
                        else if (target is XHud_Manager mgr)
                        {
                            Undo.RecordObject(mgr, "修改 XHud 管理器的全局默认动效参数");

                            if (type == HudElementMotionType.Creator)
                                mgr.global_motion_create_args = motion_create_args.Clone();
                            else
                                mgr.global_motion_recycle_args = motion_recycle_args.Clone();

                            EditorUtility.SetDirty(mgr);

                            if (PrefabUtility.IsPartOfPrefabInstance(mgr))
                                PrefabUtility.RecordPrefabInstancePropertyModifications(mgr);

                            EditorApplication.delayCall += () =>
                            {
                                string hexcol = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.themes_get_color(ThemeColorType.primary), true);

                                XGUI.dialog(
                                    type: XGUIDialogType.确认,
                                    windowtitle: "XHud - 动效设置工具消息",
                                    title: "动效更新完成",
                                    msg: $"您刚刚更新了 <color={hexcol}>{mgr.gameObject.name} </color> 管理器的全局默认动效 <color={hexcol}>{type} </color> 参数！",
                                    ok: "明白",
                                    PrimaryIndex: 0,
                                    usemodal: true,
                                    themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                            };
                        }
                        #endregion

                        EditorGUIUtility.PingObject(target);

                        Close();
                        break;
                }
            }
            #endregion

            XGUI.layout_flexspace();

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            #endregion
        }
        #endregion
    }
}