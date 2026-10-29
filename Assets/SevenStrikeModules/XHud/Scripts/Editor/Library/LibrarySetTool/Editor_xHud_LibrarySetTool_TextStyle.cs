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
    using TMPro;
    using UnityEditor;
    using UnityEngine;

    public class Editor_XHud_LibrarySetTool_TextStyle : EditorWindow
    {
        #region 字段 - 序列化对象 / 属性
        /// <summary>
        /// 用于序列化当前窗口的自定义字段。
        /// </summary>
        private SerializedObject base_object;
        #endregion

        #region 字段 - 序列化属性
        /// <summary>
        /// 样式名称属性
        /// </summary>
        private SerializedProperty sp_lib_name;
        /// <summary>
        /// 样式说明属性
        /// </summary>
        private SerializedProperty sp_description;
        /// <summary>
        /// 样式数据属性
        /// </summary>
        private SerializedProperty sp_style_info;
        #endregion

        #region 字段 - 数据
        /// <summary>
        /// 当前编辑的文字样式数据（工作副本）。
        /// </summary>
        [SerializeField]
        private XHud_LibraryArg_TextStyle style_info;
        /// <summary>
        /// 文字样式的原始数据（用于恢复）。
        /// </summary>
        [SerializeField]
        private XHud_LibraryArg_TextStyle origin_style_info;
        /// <summary>
        /// 样式名称。
        /// </summary>
        [SerializeField]
        public string lib_name;
        /// <summary>
        /// 样式说明。
        /// </summary>
        [SerializeField]
        public string description;
        /// <summary>
        /// 原始样式名称。
        /// </summary>
        [SerializeField]
        public string origin_lib_name;
        /// <summary>
        /// 修改的库项索引。
        /// </summary>
        public int modified_index;
        /// <summary>
        /// 目标文字样式库。
        /// </summary>
        private XHud_Library_TextStyle target_hud_text_style_library;
        /// <summary>
        /// 库设置模式（添加到库 / 修改库源参数）。
        /// </summary>
        public LibrarySetterMode library_setter_mode;
        #endregion

        #region 字段 - 组件
        /// <summary>
        /// 源 Text 组件。
        /// <para/>
        /// 用于实时预览并反向写入样式修改。
        /// </summary>
        public XHud_Module_Text component_text;
        /// <summary>
        /// 源 TmpText 组件。
        /// <para/>
        /// 用于实时预览并反向写入样式修改。
        /// </summary>
        public XHud_Module_TmpText component_tmptext;
        #endregion

        #region 字段 - 图标
        /// <summary>
        /// 窗口 Logo 图标。
        /// </summary>
        private Texture2D icon_logo;
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
        public string button_text_cancel;
        /// <summary>
        /// 窗口标题。
        /// </summary>
        string title_primary;
        /// <summary>
        /// 窗口副标题。
        /// </summary>
        string title_sub = "以下为目标文字样式的参数，可根据需要进行调整";
        /// <summary>
        /// 参数面板工具栏当前索引。
        /// </summary>
        private int toolbar_index;
        /// <summary>
        /// 窗口是否正在关闭。
        /// <para/>
        /// 用于跳过关闭阶段的实时预览反向写入。
        /// </summary>
        private bool is_closing;
        #endregion

        #region 字段 - 选项文字
        /// <summary>
        /// 类型选项文字（Text / TmpText）。
        /// </summary>
        string[] optnames_a = new string[] { "Text", "TmpText" };
        /// <summary>
        /// 水平溢出选项文字。
        /// </summary>
        string[] optnames_d = new string[] { "换行", "溢出" };
        /// <summary>
        /// 垂直溢出选项文字。
        /// </summary>
        string[] optnames_e = new string[] { "截断", "溢出" };
        /// <summary>
        /// 字体样式选项文字。
        /// </summary>
        string[] optnames_f = new string[] { "常规", "加粗", "斜体", "斜粗" };
        /// <summary>
        /// 渐变模式选项文字。
        /// </summary>
        string[] optnames_g = new string[] { "单色", "水平", "垂直", "四角" };
        #endregion

        #region 生命周期
        /// <summary>
        /// Unity 禁用回调。
        /// <para/>
        /// 标记窗口正在关闭，用于跳过关闭阶段的实时预览反向写入。
        /// </summary>
        private void OnDisable()
        {
            is_closing = true;
        }
        /// <summary>
        /// Unity 启用回调。
        /// <para/>
        /// 执行流程：
        /// <list type="number">
        /// <item><description>构建序列化对象并绑定属性；</description></item>
        /// <item><description>加载窗口 Logo 图标；</description></item>
        /// <item><description>初始化默认名称与说明文字。</description></item>
        /// </list>
        /// </summary>
        private void OnEnable()
        {
            base_object = new SerializedObject(this);

            #region 获取序列化属性
            sp_lib_name = base_object.FindProperty(nameof(lib_name));
            sp_style_info = base_object.FindProperty(nameof(style_info));
            sp_description = base_object.FindProperty(nameof(description));
            #endregion

            #region 获取图标
            icon_logo = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_text_style_setter/icon_logo");
            #endregion

            description = "文字样式参数说明内容";
            lib_name = "文字样式名称";
        }
        /// <summary>
        /// Unity 销毁回调。
        /// <para/>
        /// 若当前处于添加到库模式且样式数据已变更，
        /// 弹窗询问用户恢复原有样式或保持现有样式效果。
        /// </para>
        /// 最后让场景视图重新获得焦点。
        /// </summary>
        private void OnDestroy()
        {
            #region 是否恢复到未改动的原有参数，还是保持调整后的状态
            if (library_setter_mode == LibrarySetterMode.添加到库)
            {
                bool EqualsData = origin_style_info.EqualsData(style_info);

                if (!EqualsData)
                {
                    EditorApplication.delayCall += () =>
                    {
                        string res = XGUI.dialog(
                            type: XGUIDialogType.警告,
                            windowtitle: "XHud - 文字样式库采集器消息",
                            title: "恢复原有样式",
                            msg: $"检测到您修正了样式参数，是否要恢复原有样式或者保持现有样式效果？",
                            ok: "保持",
                            cancel: "恢复",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                        if (res == "恢复")
                        {
                            if (component_text != null)
                            {
                                component_text.txt_Set_Style_ForSetter(origin_style_info);
                            }
                            else if (component_tmptext != null)
                            {
                                component_tmptext.tmp_Update_Style_LibrarySetter(origin_style_info);
                            }
                        }
                    };
                }
            }
            #endregion

            GetWindow<SceneView>().Focus();
        }
        /// <summary>
        /// 绘制窗口 GUI。
        /// <para/>
        /// 按自上而下的顺序绘制：
        /// 抬头 → 样式信息 → 参数（Text / TmpText 分支） → 选项 → 按钮。
        /// <para/>
        /// 实时将样式变更反向写入源 Text / TmpText 组件，实现即时预览。
        /// 支持 Escape 键关闭窗口与点击空白区域失焦。
        /// </summary>
        private void OnGUI()
        {
            base_object.Update();

            TextClipping clipping_ellipsis = TextClipping.Ellipsis;

            #region 抬头
            Rect rect = new Rect(0, 0, position.width, position.height);

            // 图标
            Rect rect_icon = new Rect(15, 15, icon_logo.width, icon_logo.height);
            XGUI.gui_icon(
                rect: rect_icon,
                icon: icon_logo,
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

            #region 参数
            SerializedProperty prop_Type = sp_style_info.FindPropertyRelative(nameof(XHud_LibraryArg_TextStyle.type));
            SerializedProperty prop_Raycast = sp_style_info.FindPropertyRelative(nameof(XHud_LibraryArg_TextStyle.raycast));
            SerializedProperty prop_Maskable = sp_style_info.FindPropertyRelative(nameof(XHud_LibraryArg_TextStyle.maskable));

            #region Text属性
            SerializedProperty prop_overflow_h = sp_style_info.FindPropertyRelative(nameof(XHud_LibraryArg_TextStyle.overflow_horizon));
            SerializedProperty prop_overflow_v = sp_style_info.FindPropertyRelative(nameof(XHud_LibraryArg_TextStyle.overflow_vertical));
            SerializedProperty prop_autosize = sp_style_info.FindPropertyRelative(nameof(XHud_LibraryArg_TextStyle.best_fit));
            SerializedProperty prop_rich = sp_style_info.FindPropertyRelative(nameof(XHud_LibraryArg_TextStyle.rich_text));
            SerializedProperty prop_alignGEO = sp_style_info.FindPropertyRelative(nameof(XHud_LibraryArg_TextStyle.geometre_align));
            SerializedProperty prop_style = sp_style_info.FindPropertyRelative(nameof(XHud_LibraryArg_TextStyle.style));
            SerializedProperty prop_Font = sp_style_info.FindPropertyRelative(nameof(XHud_LibraryArg_TextStyle.font));
            SerializedProperty prop_ContentAnchor = sp_style_info.FindPropertyRelative(nameof(XHud_LibraryArg_TextStyle.content_anchor));
            SerializedProperty prop_Size = sp_style_info.FindPropertyRelative(nameof(XHud_LibraryArg_TextStyle.size));
            SerializedProperty prop_Fit_Min = sp_style_info.FindPropertyRelative(nameof(XHud_LibraryArg_TextStyle.fit_min));
            SerializedProperty prop_Fit_Max = sp_style_info.FindPropertyRelative(nameof(XHud_LibraryArg_TextStyle.fit_max));
            SerializedProperty prop_LineHeight = sp_style_info.FindPropertyRelative(nameof(XHud_LibraryArg_TextStyle.line_height));
            SerializedProperty prop_FontColor = sp_style_info.FindPropertyRelative(nameof(XHud_LibraryArg_TextStyle.font_color));
            #endregion

            #region TmpText属性
            SerializedProperty prop_tmp_rich = sp_style_info.FindPropertyRelative(nameof(XHud_LibraryArg_TextStyle.tmp_rich));
            SerializedProperty prop_tmp_contentwrap = sp_style_info.FindPropertyRelative(nameof(XHud_LibraryArg_TextStyle.tmp_contentwrap));
            SerializedProperty prop_tmp_overflow = sp_style_info.FindPropertyRelative(nameof(XHud_LibraryArg_TextStyle.tmp_overflow));
            SerializedProperty prop_tmp_style = sp_style_info.FindPropertyRelative(nameof(XHud_LibraryArg_TextStyle.tmp_style));
            SerializedProperty prop_tmp_anchor = sp_style_info.FindPropertyRelative(nameof(XHud_LibraryArg_TextStyle.tmp_anchor));
            SerializedProperty prop_tmp_font = sp_style_info.FindPropertyRelative(nameof(XHud_LibraryArg_TextStyle.tmp_font));
            SerializedProperty prop_tmp_size = sp_style_info.FindPropertyRelative(nameof(XHud_LibraryArg_TextStyle.tmp_size));
            SerializedProperty prop_tmp_space_character = sp_style_info.FindPropertyRelative(nameof(XHud_LibraryArg_TextStyle.tmp_space_character));
            SerializedProperty prop_tmp_space_word = sp_style_info.FindPropertyRelative(nameof(XHud_LibraryArg_TextStyle.tmp_space_word));
            SerializedProperty prop_tmp_space_lineheight = sp_style_info.FindPropertyRelative(nameof(XHud_LibraryArg_TextStyle.tmp_space_lineheight));
            SerializedProperty prop_tmp_space_paragraph = sp_style_info.FindPropertyRelative(nameof(XHud_LibraryArg_TextStyle.tmp_space_paragraph));
            SerializedProperty prop_tmp_WrappingRatios = sp_style_info.FindPropertyRelative(nameof(XHud_LibraryArg_TextStyle.tmp_wrapping_ratios));
            SerializedProperty prop_tmp_contentmargin = sp_style_info.FindPropertyRelative(nameof(XHud_LibraryArg_TextStyle.tmp_content_margin));
            SerializedProperty prop_tmp_color = sp_style_info.FindPropertyRelative(nameof(XHud_LibraryArg_TextStyle.tmp_color));
            SerializedProperty prop_tmp_EnableAutoSizing = sp_style_info.FindPropertyRelative(nameof(XHud_LibraryArg_TextStyle.tmp_enable_auto_sizing));
            SerializedProperty prop_tmp_FontSizeMin = sp_style_info.FindPropertyRelative(nameof(XHud_LibraryArg_TextStyle.tmp_font_size_min));
            SerializedProperty prop_tmp_FontSizeMax = sp_style_info.FindPropertyRelative(nameof(XHud_LibraryArg_TextStyle.tmp_font_size_max));
            SerializedProperty prop_tmp_CharWidthMaxAdj = sp_style_info.FindPropertyRelative(nameof(XHud_LibraryArg_TextStyle.tmp_char_width_max_adj));
            SerializedProperty prop_tmp_LineSpacingMax = sp_style_info.FindPropertyRelative(nameof(XHud_LibraryArg_TextStyle.tmp_line_spacing_max));
            SerializedProperty prop_gra_A = sp_style_info.FindPropertyRelative(nameof(XHud_LibraryArg_TextStyle.gra_a));
            SerializedProperty prop_gra_B = sp_style_info.FindPropertyRelative(nameof(XHud_LibraryArg_TextStyle.gra_b));
            SerializedProperty prop_gra_C = sp_style_info.FindPropertyRelative(nameof(XHud_LibraryArg_TextStyle.gra_c));
            SerializedProperty prop_gra_D = sp_style_info.FindPropertyRelative(nameof(XHud_LibraryArg_TextStyle.gra_d));
            SerializedProperty prop_gra_Invert = sp_style_info.FindPropertyRelative(nameof(XHud_LibraryArg_TextStyle.gra_invert));
            SerializedProperty prop_gra_Used = sp_style_info.FindPropertyRelative(nameof(XHud_LibraryArg_TextStyle.gra_used));
            SerializedProperty prop_gra_ColorMode = sp_style_info.FindPropertyRelative(nameof(XHud_LibraryArg_TextStyle.gra_color_mode));
            SerializedProperty prop_gra_ColorModeName = sp_style_info.FindPropertyRelative(nameof(XHud_LibraryArg_TextStyle.gra_color_mode_name));
            #endregion

            xHud_TextType textType = (xHud_TextType)prop_Type.enumValueIndex;

            // 类型标识
            Rect rect_mark = new Rect(rect.x + (rect.width - 80), rect.y + 15, 65, XGUI.GetSingleLineHeight());
            XGUI.gui_label(
                rect: rect_mark,
                bg_fill: XGUIGroup.实体,
                bg_color: XGUIColor.亮白,
                bg_color_gui: Color.white,
                text: new GUIContent(textType.ToString()),
                text_color: Color.black,
                size: XGUIFontSize.S,
                anchor: TextAnchor.MiddleCenter,
                offset: new Vector2(0, -2),
                clipping: clipping_ellipsis);

            XGUI.layout_space(100);

            #region 样式信息
            XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "样式信息",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                margin: new RectOffset(10, 10, 0, 0),
                padding: new RectOffset(10, 10, 15, 15));

            Rect Description_Rect = XGUI.GetLastRect();

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

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            if (textType == xHud_TextType.Text)
            {
                #region 参数
                XGUI.layout_group_start(
                  type: XGUIContainerType.Vertical,
                  bg_fill: XGUIGroup.缺口纯色边框,
                  bg_color: XGUIColor.亮白,
                  bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                  title: "参数",
                  title_size: XGUIFontSize.M,
                  title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                  title_clipping: TextClipping.Clip,
                  margin: new RectOffset(10, 10, 0, 0),
                  padding: new RectOffset(10, 10, 15, 15));

                #region 类型
                XGUI.layout_int_popup(
                    title: "类型",
                    title_width: 60,
                    title_size: XGUIFontSize.M,
                    title_anchor: TextAnchor.MiddleLeft,
                    prop: prop_Type,
                    options: optnames_a,
                    opt_text_size: XGUIFontSize.M,
                    opt_text_color: Color.black,
                    opt_text_padding: new RectOffset(10, 10, 0, 0),
                    opt_anchor: TextAnchor.MiddleLeft,
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
                        prop_Type.enumValueIndex = value;
                        prop_Type.serializedObject.ApplyModifiedProperties();

                        if (prop_Type.enumValueIndex == (int)xHud_TextType.Text)
                        {
                            this.minSize = new Vector2(this.maxSize.x, 830);
                            maxSize = minSize;
                        }
                        if (prop_Type.enumValueIndex == (int)xHud_TextType.TmpText)
                        {
                            this.minSize = new Vector2(this.maxSize.x, 1054);
                            maxSize = minSize;
                        }
                    });
                #endregion

                #region 水平溢出
                XGUI.layout_int_popup(
                    title: "水平溢出",
                    title_width: 60,
                    title_size: XGUIFontSize.M,
                    title_anchor: TextAnchor.MiddleLeft,
                    prop: prop_overflow_h,
                    options: optnames_d,
                    opt_text_size: XGUIFontSize.M,
                    opt_text_color: Color.black,
                    opt_text_padding: new RectOffset(10, 10, 0, 0),
                    opt_anchor: TextAnchor.MiddleLeft,
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
                        prop_overflow_h.enumValueIndex = value;
                        prop_overflow_h.serializedObject.ApplyModifiedProperties();
                    });
                #endregion

                #region 垂直溢出
                XGUI.layout_int_popup(
                    title: "垂直溢出",
                    title_width: 60,
                    title_size: XGUIFontSize.M,
                    title_anchor: TextAnchor.MiddleLeft,
                    prop: prop_overflow_v,
                    options: optnames_e,
                    opt_text_size: XGUIFontSize.M,
                    opt_text_color: Color.black,
                    opt_text_padding: new RectOffset(10, 10, 0, 0),
                    opt_anchor: TextAnchor.MiddleLeft,
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
                        prop_overflow_v.enumValueIndex = value;
                        prop_overflow_v.serializedObject.ApplyModifiedProperties();
                    });
                #endregion

                #region 样式
                XGUI.layout_int_popup(
                    title: "样式",
                    title_width: 60,
                    title_size: XGUIFontSize.M,
                    title_anchor: TextAnchor.MiddleLeft,
                    prop: prop_style,
                    options: optnames_f,
                    opt_text_size: XGUIFontSize.M,
                    opt_text_color: Color.black,
                    opt_text_padding: new RectOffset(10, 10, 0, 0),
                    opt_anchor: TextAnchor.MiddleLeft,
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
                        prop_style.enumValueIndex = value;
                        prop_style.serializedObject.ApplyModifiedProperties();
                    });
                #endregion

                #region 字体
                XGUI.layout_property_field(
                    title: "字体",
                    title_size: XGUIFontSize.M,
                    title_hover_color: Color.white,
                    title_width: 60,
                    prop: prop_Font,
                    prop_padding: new RectOffset(0, 0, 0, 0),
                    prop_margin: new RectOffset(0, 0, 0, 10));
                #endregion

                #region 锚点
                XGUI.layout_property_field(
                    title: "锚点",
                    title_size: XGUIFontSize.M,
                    title_hover_color: Color.white,
                    title_width: 60,
                    prop: prop_ContentAnchor,
                    prop_padding: new RectOffset(0, 0, 0, 0),
                    prop_margin: new RectOffset(0, 0, 0, 10));
                #endregion

                #region 尺寸
                XGUI.layout_property_field(
                    title: "尺寸",
                    title_size: XGUIFontSize.M,
                    title_hover_color: Color.white,
                    title_width: 60,
                    prop: prop_Size,
                    prop_padding: new RectOffset(0, 0, 0, 0),
                    prop_margin: new RectOffset(0, 0, 0, 10));
                #endregion

                #region 最小尺寸
                XGUI.layout_property_field(
                    title: "最小尺寸",
                    title_size: XGUIFontSize.M,
                    title_hover_color: Color.white,
                    title_width: 60,
                    prop: prop_Fit_Min,
                    prop_padding: new RectOffset(0, 0, 0, 0),
                    prop_margin: new RectOffset(0, 0, 0, 10));
                #endregion

                #region 最大尺寸
                XGUI.layout_property_field(
                    title: "最大尺寸",
                    title_size: XGUIFontSize.M,
                    title_hover_color: Color.white,
                    title_width: 60,
                    prop: prop_Fit_Max,
                    prop_padding: new RectOffset(0, 0, 0, 0),
                    prop_margin: new RectOffset(0, 0, 0, 10));
                #endregion

                #region 行高
                XGUI.layout_property_field(
                    title: "行高",
                    title_size: XGUIFontSize.M,
                    title_hover_color: Color.white,
                    title_width: 60,
                    prop: prop_LineHeight,
                    prop_padding: new RectOffset(0, 0, 0, 0),
                    prop_margin: new RectOffset(0, 0, 0, 10));
                #endregion

                #region 颜色
                XGUI.layout_property_field(
                    title: "颜色",
                    title_size: XGUIFontSize.M,
                    title_hover_color: Color.white,
                    title_width: 60,
                    prop: prop_FontColor,
                    prop_padding: new RectOffset(0, 0, 0, 0),
                    prop_margin: new RectOffset(0, 0, 0, 10));
                #endregion

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                #endregion

                #region 选项
                XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIGroup.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                    title: "选项",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_clipping: TextClipping.Clip,
                    margin: new RectOffset(10, 10, 0, 0),
                    padding: new RectOffset(10, 10, 15, 15));

                #region 自动尺寸
                XGUI.layout_toggle(
                    title: "自动尺寸",
                    title_size: XGUIFontSize.M,
                    title_font_style: FontStyle.Normal,
                    title_padding: new RectOffset(0, 10, 0, 0),
                    title_width: 60,
                    prop: prop_autosize,
                    tog_style: XGUIToggleStyle.实体,
                    tog_padding: new RectOffset(5, 8, 0, 0),
                    tog_margin: new RectOffset(0, 0, 0, 5),
                    tog_mixed_options: new string[] { "禁用", "启用" },
                    tog_mixed_text_size: XGUIFontSize.M,
                    tog_mixed_text_color: Color.black,
                    tog_mixed_text_padding: new RectOffset(10, 10, 0, 0),
                    tog_mixed_text_anchor: TextAnchor.MiddleCenter,
                    tog_mixed_font_style: FontStyle.Normal,
                    tog_bg_off_color: Color.white * 0.65f,
                    tog_bg_on_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    tog_handler_off_color: Color.white,
                    tog_handler_on_color: Color.white,
                    tog_mixed_bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    act_on_changed: null);
                #endregion

                #region 射线检测
                XGUI.layout_toggle(
                    title: "射线检测",
                    title_size: XGUIFontSize.M,
                    title_font_style: FontStyle.Normal,
                    title_padding: new RectOffset(0, 10, 0, 0),
                    title_width: 60,
                    prop: prop_Raycast,
                    tog_style: XGUIToggleStyle.实体,
                    tog_padding: new RectOffset(5, 8, 0, 0),
                    tog_margin: new RectOffset(0, 0, 0, 5),
                    tog_mixed_options: new string[] { "禁用", "启用" },
                    tog_mixed_text_size: XGUIFontSize.M,
                    tog_mixed_text_color: Color.black,
                    tog_mixed_text_padding: new RectOffset(10, 10, 0, 0),
                    tog_mixed_text_anchor: TextAnchor.MiddleCenter,
                    tog_mixed_font_style: FontStyle.Normal,
                    tog_bg_off_color: Color.white * 0.65f,
                    tog_bg_on_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    tog_handler_off_color: Color.white,
                    tog_handler_on_color: Color.white,
                    tog_mixed_bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    act_on_changed: null);
                #endregion

                #region 遮罩
                XGUI.layout_toggle(
                    title: "遮罩",
                    title_size: XGUIFontSize.M,
                    title_font_style: FontStyle.Normal,
                    title_padding: new RectOffset(0, 10, 0, 0),
                    title_width: 60,
                    prop: prop_Maskable,
                    tog_style: XGUIToggleStyle.实体,
                    tog_padding: new RectOffset(5, 8, 0, 0),
                    tog_margin: new RectOffset(0, 0, 0, 5),
                    tog_mixed_options: new string[] { "禁用", "启用" },
                    tog_mixed_text_size: XGUIFontSize.M,
                    tog_mixed_text_color: Color.black,
                    tog_mixed_text_padding: new RectOffset(10, 10, 0, 0),
                    tog_mixed_text_anchor: TextAnchor.MiddleCenter,
                    tog_mixed_font_style: FontStyle.Normal,
                    tog_bg_off_color: Color.white * 0.65f,
                    tog_bg_on_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    tog_handler_off_color: Color.white,
                    tog_handler_on_color: Color.white,
                    tog_mixed_bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    act_on_changed: null);
                #endregion

                #region 富文本
                XGUI.layout_toggle(
                    title: "富文本",
                    title_size: XGUIFontSize.M,
                    title_font_style: FontStyle.Normal,
                    title_padding: new RectOffset(0, 10, 0, 0),
                    title_width: 60,
                    prop: prop_rich,
                    tog_style: XGUIToggleStyle.实体,
                    tog_padding: new RectOffset(5, 8, 0, 0),
                    tog_margin: new RectOffset(0, 0, 0, 5),
                    tog_mixed_options: new string[] { "禁用", "启用" },
                    tog_mixed_text_size: XGUIFontSize.M,
                    tog_mixed_text_color: Color.black,
                    tog_mixed_text_padding: new RectOffset(10, 10, 0, 0),
                    tog_mixed_text_anchor: TextAnchor.MiddleCenter,
                    tog_mixed_font_style: FontStyle.Normal,
                    tog_bg_off_color: Color.white * 0.65f,
                    tog_bg_on_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    tog_handler_off_color: Color.white,
                    tog_handler_on_color: Color.white,
                    tog_mixed_bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    act_on_changed: null);
                #endregion

                #region 几何对齐
                XGUI.layout_toggle(
                    title: "几何对齐",
                    title_size: XGUIFontSize.M,
                    title_font_style: FontStyle.Normal,
                    title_padding: new RectOffset(0, 10, 0, 0),
                    title_width: 60,
                    prop: prop_alignGEO,
                    tog_style: XGUIToggleStyle.实体,
                    tog_padding: new RectOffset(5, 8, 0, 0),
                    tog_margin: new RectOffset(0, 0, 0, 5),
                    tog_mixed_options: new string[] { "禁用", "启用" },
                    tog_mixed_text_size: XGUIFontSize.M,
                    tog_mixed_text_color: Color.black,
                    tog_mixed_text_padding: new RectOffset(10, 10, 0, 0),
                    tog_mixed_text_anchor: TextAnchor.MiddleCenter,
                    tog_mixed_font_style: FontStyle.Normal,
                    tog_bg_off_color: Color.white * 0.65f,
                    tog_bg_on_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    tog_handler_off_color: Color.white,
                    tog_handler_on_color: Color.white,
                    tog_mixed_bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    act_on_changed: null);
                #endregion

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                #endregion
            }
            else
            {
                #region 参数
                XGUI.layout_group_start(
                  type: XGUIContainerType.Vertical,
                  bg_fill: XGUIGroup.缺口纯色边框,
                  bg_color: XGUIColor.亮白,
                  bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                  title: "参数",
                  title_size: XGUIFontSize.M,
                  title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                  title_clipping: TextClipping.Clip,
                  margin: new RectOffset(10, 10, 0, 0),
                  padding: new RectOffset(10, 10, 15, 15));

                toolbar_index = XGUI.layout_toolbar(
                    index: ref toolbar_index,
                    names: new string[] { "特征", "间距", "渐变色" },
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

                switch (toolbar_index)
                {
                    case 0:
                        #region 类型
                        XGUI.layout_int_popup(
                            title: "类型",
                            title_width: 60,
                            title_size: XGUIFontSize.M,
                            title_anchor: TextAnchor.MiddleLeft,
                            prop: prop_Type,
                            options: optnames_a,
                            opt_text_size: XGUIFontSize.M,
                            opt_text_color: Color.black,
                            opt_text_padding: new RectOffset(10, 10, 0, 0),
                            opt_anchor: TextAnchor.MiddleLeft,
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
                                prop_Type.enumValueIndex = value;
                                prop_Type.serializedObject.ApplyModifiedProperties();

                                if (prop_Type.enumValueIndex == (int)xHud_TextType.Text)
                                {
                                    this.minSize = new Vector2(this.maxSize.x, 830);
                                    maxSize = minSize;
                                }
                                if (prop_Type.enumValueIndex == (int)xHud_TextType.TmpText)
                                {
                                    this.minSize = new Vector2(this.maxSize.x, 1054);
                                    maxSize = minSize;
                                }
                            });
                        #endregion

                        #region 字体
                        XGUI.layout_property_field(
                            title: "字体",
                            title_size: XGUIFontSize.M,
                            title_hover_color: Color.white,
                            title_width: 60,
                            prop: prop_tmp_font,
                            prop_padding: new RectOffset(0, 0, 0, 0),
                            prop_margin: new RectOffset(0, 0, 0, 10));
                        #endregion

                        XGUI.layout_seperator(
                            thickness: 1,
                            color: XHud_Dashboard.themes_get_color(ThemeColorType.sepline),
                            margin: new RectOffset(15, 15, 15, 15));

                        XGUI.layout_group_start(
                         type: XGUIContainerType.Horizontal,
                         absolute_margin: true,
                         absolute_padding: true,
                         margin: new RectOffset(0, 0, 0, 0),
                         padding: new RectOffset(0, 0, 0, 0));

                        #region 包裹
                        XGUI.layout_property_field(
                            title: "包裹",
                            title_size: XGUIFontSize.M,
                            title_hover_color: Color.white,
                            title_width: 140,
                            prop: prop_tmp_contentwrap,
                            prop_padding: new RectOffset(0, 0, 0, 0),
                            prop_margin: new RectOffset(0, 0, 0, 10));
                        #endregion

                        #region 溢出
                        XGUI.layout_property_field(
                            title: "溢出",
                            title_size: XGUIFontSize.M,
                            title_hover_color: Color.white,
                            title_width: 140,
                            prop: prop_tmp_overflow,
                            prop_padding: new RectOffset(0, 0, 0, 0),
                            prop_margin: new RectOffset(0, 0, 0, 10));
                        #endregion

                        XGUI.layout_group_end(type: XGUIContainerType.Horizontal);

                        XGUI.layout_group_start(
                         type: XGUIContainerType.Horizontal,
                         absolute_margin: true,
                         absolute_padding: true,
                         margin: new RectOffset(0, 0, 0, 0),
                         padding: new RectOffset(0, 0, 0, 0));

                        #region 样式
                        XGUI.layout_property_field(
                            title: "样式",
                            title_size: XGUIFontSize.M,
                            title_hover_color: Color.white,
                            title_width: 140,
                            prop: prop_tmp_style,
                            prop_padding: new RectOffset(0, 0, 0, 0),
                            prop_margin: new RectOffset(0, 0, 0, 10));
                        #endregion

                        #region 锚点
                        XGUI.layout_property_field(
                            title: "锚点",
                            title_size: XGUIFontSize.M,
                            title_hover_color: Color.white,
                            title_width: 140,
                            prop: prop_tmp_anchor,
                            prop_padding: new RectOffset(0, 0, 0, 0),
                            prop_margin: new RectOffset(0, 0, 0, 10));
                        #endregion

                        XGUI.layout_group_end(type: XGUIContainerType.Horizontal);

                        XGUI.layout_group_start(
                         type: XGUIContainerType.Horizontal,
                         absolute_margin: true,
                         absolute_padding: true,
                         margin: new RectOffset(0, 0, 0, 0),
                         padding: new RectOffset(0, 0, 0, 0));

                        #region 尺寸
                        XGUI.layout_property_field(
                            title: "尺寸",
                            title_size: XGUIFontSize.M,
                            title_hover_color: Color.white,
                            title_width: 140,
                            prop: prop_tmp_size,
                            prop_padding: new RectOffset(0, 0, 0, 0),
                            prop_margin: new RectOffset(0, 0, 0, 10));
                        #endregion

                        #region 颜色
                        XGUI.layout_property_field(
                            title: "颜色",
                            title_size: XGUIFontSize.M,
                            title_hover_color: Color.white,
                            title_width: 140,
                            prop: prop_tmp_color,
                            prop_padding: new RectOffset(0, 0, 0, 0),
                            prop_margin: new RectOffset(0, 0, 0, 10));
                        #endregion

                        XGUI.layout_group_end(type: XGUIContainerType.Horizontal);

                        #region 包裹比例
                        XGUI.layout_property_field(
                            title: "包裹比例",
                            title_size: XGUIFontSize.M,
                            title_hover_color: Color.white,
                            title_width: 60,
                            prop: prop_tmp_WrappingRatios,
                            prop_padding: new RectOffset(0, 0, 0, 0),
                            prop_margin: new RectOffset(0, 0, 0, 10));
                        #endregion

                        if (!prop_tmp_EnableAutoSizing.boolValue)
                            XGUI.SetEnabled(false);

                        XGUI.layout_group_start(
                           type: XGUIContainerType.Horizontal,
                           absolute_margin: true,
                           absolute_padding: true,
                           margin: new RectOffset(0, 0, 0, 0),
                           padding: new RectOffset(0, 0, 0, 0));

                        #region 最小尺寸
                        XGUI.layout_property_field(
                            title: "最小尺寸",
                            title_size: XGUIFontSize.M,
                            title_hover_color: Color.white,
                            title_width: 140,
                            prop: prop_tmp_FontSizeMin,
                            prop_padding: new RectOffset(0, 0, 0, 0),
                            prop_margin: new RectOffset(0, 0, 0, 10));
                        #endregion

                        #region 最大尺寸
                        XGUI.layout_property_field(
                            title: "最大尺寸",
                            title_size: XGUIFontSize.M,
                            title_hover_color: Color.white,
                            title_width: 140,
                            prop: prop_tmp_FontSizeMax,
                            prop_padding: new RectOffset(0, 0, 0, 0),
                            prop_margin: new RectOffset(0, 0, 0, 10));
                        #endregion

                        XGUI.layout_group_end(type: XGUIContainerType.Horizontal);

                        XGUI.layout_group_start(
                           type: XGUIContainerType.Horizontal,
                           absolute_margin: true,
                           absolute_padding: true,
                           margin: new RectOffset(0, 0, 0, 0),
                           padding: new RectOffset(0, 0, 0, 0));

                        #region 最大字符宽度
                        XGUI.layout_property_field(
                            title: "最大字符宽度",
                            title_size: XGUIFontSize.M,
                            title_hover_color: Color.white,
                            title_width: 140,
                            prop: prop_tmp_CharWidthMaxAdj,
                            prop_padding: new RectOffset(0, 0, 0, 0),
                            prop_margin: new RectOffset(0, 0, 0, 10));
                        #endregion

                        #region 最大行高
                        XGUI.layout_property_field(
                            title: "最大行高",
                            title_size: XGUIFontSize.M,
                            title_hover_color: Color.white,
                            title_width: 140,
                            prop: prop_tmp_LineSpacingMax,
                            prop_padding: new RectOffset(0, 0, 0, 0),
                            prop_margin: new RectOffset(0, 0, 0, 10));
                        #endregion

                        XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
                        break;
                    case 1:
                        XGUI.SetEnabled(true);

                        XGUI.layout_group_start(
                            type: XGUIContainerType.Horizontal,
                            absolute_margin: true,
                            absolute_padding: true,
                            margin: new RectOffset(0, 0, 0, 0),
                            padding: new RectOffset(0, 0, 0, 0));

                        #region 字符间距
                        XGUI.layout_property_field(
                            title: "字符间距",
                            title_size: XGUIFontSize.M,
                            title_hover_color: Color.white,
                            title_width: 140,
                            prop: prop_tmp_space_character,
                            prop_padding: new RectOffset(0, 0, 0, 0),
                            prop_margin: new RectOffset(0, 0, 0, 10));
                        #endregion

                        #region 单词间距
                        XGUI.layout_property_field(
                            title: "单词间距",
                            title_size: XGUIFontSize.M,
                            title_hover_color: Color.white,
                            title_width: 140,
                            prop: prop_tmp_space_word,
                            prop_padding: new RectOffset(0, 0, 0, 0),
                            prop_margin: new RectOffset(0, 0, 0, 10));
                        #endregion

                        XGUI.layout_group_end(type: XGUIContainerType.Horizontal);

                        XGUI.layout_group_start(
                            type: XGUIContainerType.Horizontal,
                            absolute_margin: true,
                            absolute_padding: true,
                            margin: new RectOffset(0, 0, 0, 0),
                            padding: new RectOffset(0, 0, 0, 0));
                        #region 行高间距
                        XGUI.layout_property_field(
                            title: "行高间距",
                            title_size: XGUIFontSize.M,
                            title_hover_color: Color.white,
                            title_width: 140,
                            prop: prop_tmp_space_lineheight,
                            prop_padding: new RectOffset(0, 0, 0, 0),
                            prop_margin: new RectOffset(0, 0, 0, 10));
                        #endregion

                        #region 段落间距
                        XGUI.layout_property_field(
                            title: "段落间距",
                            title_size: XGUIFontSize.M,
                            title_hover_color: Color.white,
                            title_width: 140,
                            prop: prop_tmp_space_paragraph,
                            prop_padding: new RectOffset(0, 0, 0, 0),
                            prop_margin: new RectOffset(0, 0, 0, 10));
                        #endregion
                        XGUI.layout_group_end(type: XGUIContainerType.Horizontal);

                        XGUI.layout_group_start(
                            type: XGUIContainerType.Horizontal,
                            absolute_margin: true,
                            absolute_padding: true,
                            margin: new RectOffset(0, 0, 0, 0),
                            padding: new RectOffset(0, 0, 0, 0));
                        #region 左边距
                        XGUI.layout_property_field(
                            title: "左边距",
                            title_size: XGUIFontSize.M,
                            title_hover_color: Color.white,
                            title_width: 140,
                            prop: prop_tmp_contentmargin.FindPropertyRelative(nameof(Vector4.x)),
                            prop_padding: new RectOffset(0, 0, 0, 0),
                            prop_margin: new RectOffset(0, 0, 0, 10));
                        #endregion

                        #region 上边距
                        XGUI.layout_property_field(
                            title: "上边距",
                            title_size: XGUIFontSize.M,
                            title_hover_color: Color.white,
                            title_width: 140,
                            prop: prop_tmp_contentmargin.FindPropertyRelative(nameof(Vector4.y)),
                            prop_padding: new RectOffset(0, 0, 0, 0),
                            prop_margin: new RectOffset(0, 0, 0, 10));
                        #endregion
                        XGUI.layout_group_end(type: XGUIContainerType.Horizontal);

                        XGUI.layout_group_start(
                            type: XGUIContainerType.Horizontal,
                            absolute_margin: true,
                            absolute_padding: true,
                            margin: new RectOffset(0, 0, 0, 0),
                            padding: new RectOffset(0, 0, 0, 0));
                        #region 右边距
                        XGUI.layout_property_field(
                            title: "右边距",
                            title_size: XGUIFontSize.M,
                            title_hover_color: Color.white,
                            title_width: 140,
                            prop: prop_tmp_contentmargin.FindPropertyRelative(nameof(Vector4.z)),
                            prop_padding: new RectOffset(0, 0, 0, 0),
                            prop_margin: new RectOffset(0, 0, 0, 10));
                        #endregion

                        #region 下边距
                        XGUI.layout_property_field(
                            title: "下边距",
                            title_size: XGUIFontSize.M,
                            title_hover_color: Color.white,
                            title_width: 140,
                            prop: prop_tmp_contentmargin.FindPropertyRelative(nameof(Vector4.w)),
                            prop_padding: new RectOffset(0, 0, 0, 0),
                            prop_margin: new RectOffset(0, 0, 0, 10));
                        #endregion
                        XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
                        break;
                    case 2:
                        #region 渐变色支持
                        XGUI.layout_toggle(
                            title: "渐变色支持",
                            title_size: XGUIFontSize.M,
                            title_font_style: FontStyle.Normal,
                            title_padding: new RectOffset(0, 10, 0, 0),
                            title_width: 100,
                            prop: prop_gra_Used,
                            tog_style: XGUIToggleStyle.实体,
                            tog_padding: new RectOffset(5, 8, 0, 0),
                            tog_margin: new RectOffset(0, 0, 0, 5),
                            tog_mixed_options: new string[] { "禁用", "启用" },
                            tog_mixed_text_size: XGUIFontSize.M,
                            tog_mixed_text_color: Color.black,
                            tog_mixed_text_padding: new RectOffset(10, 10, 0, 0),
                            tog_mixed_text_anchor: TextAnchor.MiddleCenter,
                            tog_mixed_font_style: FontStyle.Normal,
                            tog_bg_off_color: Color.white * 0.65f,
                            tog_bg_on_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                            tog_handler_off_color: Color.white,
                            tog_handler_on_color: Color.white,
                            tog_mixed_bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                            act_on_changed: null);

                        ColorMode gra_mode = (ColorMode)prop_gra_ColorMode.enumValueIndex;
                        switch (gra_mode)
                        {
                            case ColorMode.Single:
                                prop_gra_ColorModeName.stringValue = "单色";
                                break;
                            case ColorMode.HorizontalGradient:
                                prop_gra_ColorModeName.stringValue = "水平渐变";
                                break;
                            case ColorMode.VerticalGradient:
                                prop_gra_ColorModeName.stringValue = "垂直渐变";
                                break;
                            case ColorMode.FourCornersGradient:
                                prop_gra_ColorModeName.stringValue = "四角渐变";
                                break;
                        }
                        #endregion

                        #region 渐变方向
                        if (!prop_gra_Used.boolValue)
                            XGUI.SetEnabled(false);
                        XGUI.layout_toggle(
                            title: "渐变方向",
                            title_size: XGUIFontSize.M,
                            title_font_style: FontStyle.Normal,
                            title_padding: new RectOffset(0, 10, 0, 0),
                            title_width: 60,
                            prop: prop_gra_Invert,
                            tog_style: XGUIToggleStyle.实体,
                            tog_padding: new RectOffset(5, 8, 0, 0),
                            tog_margin: new RectOffset(0, 0, 0, 5),
                            tog_mixed_options: new string[] { "禁用", "启用" },
                            tog_mixed_text_size: XGUIFontSize.M,
                            tog_mixed_text_color: Color.black,
                            tog_mixed_text_padding: new RectOffset(10, 10, 0, 0),
                            tog_mixed_text_anchor: TextAnchor.MiddleCenter,
                            tog_mixed_font_style: FontStyle.Normal,
                            tog_bg_off_color: Color.white * 0.65f,
                            tog_bg_on_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                            tog_handler_off_color: Color.white,
                            tog_handler_on_color: Color.white,
                            tog_mixed_bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                            act_on_changed: null);
                        #endregion

                        #region 渐变模式
                        XGUI.layout_int_popup(
                            title: "渐变模式",
                            title_width: 100,
                            title_size: XGUIFontSize.M,
                            title_anchor: TextAnchor.MiddleLeft,
                            prop: prop_gra_ColorMode,
                            options: optnames_g,
                            opt_text_size: XGUIFontSize.M,
                            opt_text_color: Color.black,
                            opt_text_padding: new RectOffset(10, 10, 0, 0),
                            opt_anchor: TextAnchor.MiddleLeft,
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
                                prop_gra_ColorMode.enumValueIndex = value;
                                prop_gra_ColorMode.serializedObject.ApplyModifiedProperties();
                            });
                        #endregion

                        XGUI.layout_group_start(
                            type: XGUIContainerType.Horizontal,
                            absolute_margin: true,
                            absolute_padding: true,
                            margin: new RectOffset(0, 0, 0, 0),
                            padding: new RectOffset(0, 0, 0, 0));

                        #region 主色
                        XGUI.layout_property_field(
                            title: "主色",
                            title_size: XGUIFontSize.M,
                            title_hover_color: Color.white,
                            title_width: 100,
                            prop: prop_gra_A,
                            prop_padding: new RectOffset(0, 0, 0, 0),
                            prop_margin: new RectOffset(0, 0, 0, 10));
                        #endregion

                        #region 副色
                        XGUI.layout_property_field(
                            title: "副色",
                            title_size: XGUIFontSize.M,
                            title_hover_color: Color.white,
                            title_width: 100,
                            prop: prop_gra_B,
                            prop_padding: new RectOffset(0, 0, 0, 0),
                            prop_margin: new RectOffset(0, 0, 0, 10));
                        #endregion

                        XGUI.layout_group_end(type: XGUIContainerType.Horizontal);

                        XGUI.layout_group_start(
                            type: XGUIContainerType.Horizontal,
                            absolute_margin: true,
                            absolute_padding: true,
                            margin: new RectOffset(0, 0, 0, 0),
                            padding: new RectOffset(0, 0, 0, 0));

                        #region 次色
                        XGUI.layout_property_field(
                            title: "次色",
                            title_size: XGUIFontSize.M,
                            title_hover_color: Color.white,
                            title_width: 100,
                            prop: prop_gra_C,
                            prop_padding: new RectOffset(0, 0, 0, 0),
                            prop_margin: new RectOffset(0, 0, 0, 10));
                        #endregion

                        #region 尾色
                        XGUI.layout_property_field(
                            title: "尾色",
                            title_size: XGUIFontSize.M,
                            title_hover_color: Color.white,
                            title_width: 100,
                            prop: prop_gra_D,
                            prop_padding: new RectOffset(0, 0, 0, 0),
                            prop_margin: new RectOffset(0, 0, 0, 10));
                        #endregion

                        XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
                        break;
                }
                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                #endregion

                XGUI.SetEnabled(true);

                #region 选项
                XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIGroup.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                    title: "选项",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_clipping: TextClipping.Clip,
                    margin: new RectOffset(10, 10, 0, 0),
                    padding: new RectOffset(10, 10, 15, 15));

                #region 自动尺寸
                XGUI.layout_toggle(
                    title: "自动尺寸",
                    title_size: XGUIFontSize.M,
                    title_font_style: FontStyle.Normal,
                    title_padding: new RectOffset(0, 10, 0, 0),
                    title_width: 60,
                    prop: prop_tmp_EnableAutoSizing,
                    tog_style: XGUIToggleStyle.实体,
                    tog_padding: new RectOffset(5, 8, 0, 0),
                    tog_margin: new RectOffset(0, 0, 0, 5),
                    tog_mixed_options: new string[] { "禁用", "启用" },
                    tog_mixed_text_size: XGUIFontSize.M,
                    tog_mixed_text_color: Color.black,
                    tog_mixed_text_padding: new RectOffset(10, 10, 0, 0),
                    tog_mixed_text_anchor: TextAnchor.MiddleCenter,
                    tog_mixed_font_style: FontStyle.Normal,
                    tog_bg_off_color: Color.white * 0.65f,
                    tog_bg_on_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    tog_handler_off_color: Color.white,
                    tog_handler_on_color: Color.white,
                    tog_mixed_bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    act_on_changed: null);
                #endregion

                #region 射线检测
                XGUI.layout_toggle(
                    title: "射线检测",
                    title_size: XGUIFontSize.M,
                    title_font_style: FontStyle.Normal,
                    title_padding: new RectOffset(0, 10, 0, 0),
                    title_width: 60,
                    prop: prop_Raycast,
                    tog_style: XGUIToggleStyle.实体,
                    tog_padding: new RectOffset(5, 8, 0, 0),
                    tog_margin: new RectOffset(0, 0, 0, 5),
                    tog_mixed_options: new string[] { "禁用", "启用" },
                    tog_mixed_text_size: XGUIFontSize.M,
                    tog_mixed_text_color: Color.black,
                    tog_mixed_text_padding: new RectOffset(10, 10, 0, 0),
                    tog_mixed_text_anchor: TextAnchor.MiddleCenter,
                    tog_mixed_font_style: FontStyle.Normal,
                    tog_bg_off_color: Color.white * 0.65f,
                    tog_bg_on_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    tog_handler_off_color: Color.white,
                    tog_handler_on_color: Color.white,
                    tog_mixed_bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    act_on_changed: null);
                #endregion

                #region 遮罩
                XGUI.layout_toggle(
                    title: "遮罩",
                    title_size: XGUIFontSize.M,
                    title_font_style: FontStyle.Normal,
                    title_padding: new RectOffset(0, 10, 0, 0),
                    title_width: 60,
                    prop: prop_Maskable,
                    tog_style: XGUIToggleStyle.实体,
                    tog_padding: new RectOffset(5, 8, 0, 0),
                    tog_margin: new RectOffset(0, 0, 0, 5),
                    tog_mixed_options: new string[] { "禁用", "启用" },
                    tog_mixed_text_size: XGUIFontSize.M,
                    tog_mixed_text_color: Color.black,
                    tog_mixed_text_padding: new RectOffset(10, 10, 0, 0),
                    tog_mixed_text_anchor: TextAnchor.MiddleCenter,
                    tog_mixed_font_style: FontStyle.Normal,
                    tog_bg_off_color: Color.white * 0.65f,
                    tog_bg_on_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    tog_handler_off_color: Color.white,
                    tog_handler_on_color: Color.white,
                    tog_mixed_bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    act_on_changed: null);
                #endregion

                #region 富文本
                XGUI.layout_toggle(
                    title: "富文本",
                    title_size: XGUIFontSize.M,
                    title_font_style: FontStyle.Normal,
                    title_padding: new RectOffset(0, 10, 0, 0),
                    title_width: 60,
                    prop: prop_tmp_rich,
                    tog_style: XGUIToggleStyle.实体,
                    tog_padding: new RectOffset(5, 8, 0, 0),
                    tog_margin: new RectOffset(0, 0, 0, 5),
                    tog_mixed_options: new string[] { "禁用", "启用" },
                    tog_mixed_text_size: XGUIFontSize.M,
                    tog_mixed_text_color: Color.black,
                    tog_mixed_text_padding: new RectOffset(10, 10, 0, 0),
                    tog_mixed_text_anchor: TextAnchor.MiddleCenter,
                    tog_mixed_font_style: FontStyle.Normal,
                    tog_bg_off_color: Color.white * 0.65f,
                    tog_bg_on_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    tog_handler_off_color: Color.white,
                    tog_handler_on_color: Color.white,
                    tog_mixed_bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    act_on_changed: null);
                #endregion

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                #endregion

                //this.minSize = new Vector2(this.maxSize.x, 620);
            }
            #endregion

            base_object.ApplyModifiedProperties();

            Repaint();

            XGUI.layout_space(10);

            bottom_buttons();

            #region 检测事件
            Event e = Event.current;
            // 检测点击事件
            if (e.type == EventType.MouseDown)
            {
                // 检查点击位置是否在窗口内
                if (!Description_Rect.Contains(e.mousePosition))
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

            if (is_closing)
                return;
            if (GUI.changed)
            {
                //反向更新源文字效果
                if (component_text != null)
                    component_text.txt_Set_Style_ForSetter(style_info);
                if (component_tmptext != null)
                    component_tmptext.tmp_Update_Style_LibrarySetter(style_info);
            }
        }
        #endregion

        #region 工具 - 数据操作
        /// <summary>
        /// 发送文字样式到目标样式库（添加到库模式）。
        /// <para/>
        /// 执行流程：
        /// <list type="number">
        /// <item><description>校验名称是否为空；</description></item>
        /// <item><description>检测名称是否重复；</description></item>
        /// <item><description>构造样式数据并添加到目标库；</description></item>
        /// <item><description>弹窗提示添加完成并关闭窗口。</description></item>
        /// </list>
        /// </summary>
        private void SendToLibrary()
        {
            XHud_Manager mgr = XHud_Dashboard.get_manager();

            XHud_LibraryArg_TextStyle info = new XHud_LibraryArg_TextStyle();
            info.CopyData(style_info);
            info.name = sp_lib_name.stringValue;
            info.description = sp_description.stringValue;

            if (sp_lib_name.stringValue == "文字样式名称")
            {
                XGUI.dialog(
                    type: XGUIDialogType.警告,
                    windowtitle: "XHud - 文字样式库采集器消息",
                    title: "未填写名称",
                    msg: $"请为文字样式模版添加一个名称！",
                    ok: "明白",
                    PrimaryIndex: 0,
                    usemodal: true,
                    themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                return;
            }

            string colorhex = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.themes_get_color(ThemeColorType.primary), true);

            bool exist = target_hud_text_style_library.TextStyle_Library_NameIsValid(sp_lib_name.stringValue, info.type);

            if (exist)
            {
                XGUI.dialog(
                    type: XGUIDialogType.警告,
                    windowtitle: "XHud - 文字样式库采集器消息",
                    title: "存在重复文字样式名称",
                    msg: $"名称为<color={colorhex}> {sp_lib_name.stringValue} </color>的已经存在于文字样式库中！",
                    ok: "重命名",
                    PrimaryIndex: 0,
                    usemodal: true,
                    themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                return;
            }
            else
            {
                target_hud_text_style_library.TextStyle_Library_Add(info);

                XGUI.dialog(
                    type: XGUIDialogType.确认,
                    windowtitle: "XHud - 文字样式库采集器消息",
                    title: "已添加到文字样式库",
                    msg: $"已将名称为<color={colorhex}> {sp_lib_name.stringValue} </color>的文字样式参数添加到文字样式库中！",
                    ok: "明白",
                    PrimaryIndex: 0,
                    usemodal: true,
                    themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                Close();
            }
        }
        /// <summary>
        /// 更新到目标样式库（修改库源参数模式）。
        /// <para/>
        /// 弹窗确认后，用当前编辑的名称、类型、说明与样式数据替换目标库中指定索引的样式项，然后关闭窗口。
        /// </summary>
        private void UpdateToLibrary()
        {
            string colorhex = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.themes_get_color(ThemeColorType.primary), true);

            EditorApplication.delayCall += () =>
            {
                string res = XGUI.dialog(
                        type: XGUIDialogType.警告,
                        windowtitle: "XHud - 文字样式库采集器消息",
                        title: "更新文字样式模版",
                        msg: $"即将更新文字样式库中名称为： <color={colorhex}> {origin_style_info.name} </color> 且类型为： {origin_style_info.type} 的文字样式模版参数，确认更新参数吗？",
                        ok: "更新",
                        cancel: "暂不",
                        PrimaryIndex: 0,
                        usemodal: true,
                        themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));

                if (res == "更新")
                {
                    XHud_LibraryArg_TextStyle info = new XHud_LibraryArg_TextStyle();
                    info.CopyData_Ignored_LibraryToggle(style_info);
                    info.name = sp_lib_name.stringValue;
                    info.type = style_info.type;
                    info.description = sp_description.stringValue;
                    target_hud_text_style_library.TextStyle_Library_Replace(modified_index, info);

                    Close();
                }
            };
        }
        #endregion

        #region 设置接口
        /// <summary>
        /// 设置按钮文字（确认 / 取消）。
        /// </summary>
        /// <param name="ok">确认按钮文字</param>
        /// <param name="cancel">取消按钮文字</param>
        public void SetButtonText(string ok, string cancel)
        {
            button_text_cancel = cancel;
            button_text_ok = ok;
        }
        /// <summary>
        /// 设置模版基础信息（针对从库源修改更新使用）。
        /// </summary>
        /// <param name="name">样式名称</param>
        /// <param name="description">样式说明</param>
        public void SetInfo(string name, string description)
        {
            lib_name = name;
            this.description = description;
        }
        /// <summary>
        /// 设置模版样式信息（工作副本）。
        /// </summary>
        /// <param name="info">样式数据</param>
        public void SetStyle(XHud_LibraryArg_TextStyle info)
        {
            style_info.CopyData(info);
        }
        /// <summary>
        /// 设置模版样式信息（原始副本）。
        /// </summary>
        /// <param name="info">样式数据</param>
        public void SetOriginStyle(XHud_LibraryArg_TextStyle info)
        {
            origin_style_info.CopyData(info);
        }
        /// <summary>
        /// 设置源 Text 组件。
        /// </summary>
        /// <param name="com">Text 组件</param>
        public void SetComponent(XHud_Module_Text com)
        {
            component_text = com;
        }
        /// <summary>
        /// 设置源 TmpText 组件。
        /// </summary>
        /// <param name="com">TmpText 组件</param>
        public void SetComponent(XHud_Module_TmpText com)
        {
            component_tmptext = com;
        }
        /// <summary>
        /// 设置标题。
        /// </summary>
        /// <param name="title">标题文字</param>
        public void SetTitle(string title)
        {
            title_primary = title;
        }
        /// <summary>
        /// 设置库设置模式（添加到库 / 修改库源参数）。
        /// </summary>
        /// <param name="mode">库设置模式</param>
        public void SetLibrarySetterMode(LibrarySetterMode mode)
        {
            library_setter_mode = mode;
        }
        /// <summary>
        /// 设置目标文字样式库。
        /// </summary>
        /// <param name="lib">目标文字样式库</param>
        public void Set_Target_Hud_TextStyleLibrary(XHud_Library_TextStyle lib)
        {
            target_hud_text_style_library = lib;
        }
        #endregion

        #region 按钮
        /// <summary>
        /// 绘制底部按钮区（取消 / 确认）。
        /// <para/>
        /// 确认按钮根据 <see cref="library_setter_mode"/> 决定调用
        /// <see cref="SendToLibrary"/> 或 <see cref="UpdateToLibrary"/>。
        /// </summary>
        private void bottom_buttons()
        {
            #region 按钮
            XGUI.layout_group_start(
                type: XGUIContainerType.Horizontal,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(10, 10, 0, 0));

            XGUI.layout_flexspace();

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
                Close();
            }

            XGUI.layout_space(button_distance);

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
                if (library_setter_mode == LibrarySetterMode.添加到库)
                {
                    SendToLibrary();
                }
                else if (library_setter_mode == LibrarySetterMode.修改库源参数)
                {
                    UpdateToLibrary();
                    XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
                }
                return;
            }

            XGUI.layout_flexspace();

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            #endregion
        }
        #endregion
    }
}