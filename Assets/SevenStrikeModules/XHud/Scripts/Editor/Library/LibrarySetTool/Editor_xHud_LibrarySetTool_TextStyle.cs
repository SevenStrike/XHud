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
        private SerializedObject BaseObject;
        private SerializedProperty sp_LibName, sp_Description, sp_StyleInfo;

        [SerializeField]
        private XHud_LibraryArg_TextStyle StyleInfo;
        [SerializeField]
        private XHud_LibraryArg_TextStyle OriginStyleInfo;

        private Texture2D icon_logo;

        public LibrarySetterMode LibrarySetterMode;

        public int ModifiedIndex;

        [SerializeField]
        public string LibName;
        [SerializeField]
        public string Description;
        [SerializeField]
        public string OriginLibName;

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

        public XHud_Module_Text Component_Text;
        public XHud_Module_TmpText Component_TmpText;

        public Motion_Creator CreateArgs;
        public Motion_Recycler RecycleArgs;

        string Title;
        private int ToolbarIndex;
        private bool isClosing;

        string[] optnames_a = new string[] { "Text", "TmpText" };
        string[] optnames_d = new string[] { "换行", "溢出" };
        string[] optnames_e = new string[] { "截断", "溢出" };
        string[] optnames_f = new string[] { "常规", "加粗", "斜体", "斜粗" };
        string[] optnames_g = new string[] { "单色", "水平", "垂直", "四角" };

        private XHud_Library_TextStyle Target_Hud_TextStyleLibrary;

        private void OnDisable()
        {
            isClosing = true;
        }

        private void OnEnable()
        {
            BaseObject = new SerializedObject(this);
            sp_LibName = BaseObject.FindProperty("LibName");
            sp_StyleInfo = BaseObject.FindProperty("StyleInfo");
            sp_Description = BaseObject.FindProperty("Description");

            icon_logo = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_text_style_setter/icon_logo");

            Description = "文字样式参数说明内容";
            LibName = "文字样式名称";
        }

        private void OnDestroy()
        {
            #region 是否恢复到未改动的原有参数，还是保持调整后的状态
            if (LibrarySetterMode == LibrarySetterMode.添加到库)
            {
                bool EqualsData = OriginStyleInfo.EqualsData(StyleInfo);

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
                            themecolor: XHud_Dashboard.Theme_Primary);
                        if (res == "恢复")
                        {
                            if (Component_Text != null)
                            {
                                Component_Text.txt_Set_Style_ForSetter(OriginStyleInfo);
                            }
                            else if (Component_TmpText != null)
                            {
                                Component_TmpText.tmp_Update_Style_LibrarySetter(OriginStyleInfo);
                            }
                        }
                    };
                }
            }
            #endregion

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
                text: new GUIContent("以下为目标文字样式的参数，可根据需要进行调整"),
                text_color: Color.white * 0.7f,
                size: XGUIFontSize.M,
                anchor: TextAnchor.UpperLeft,
                clipping: clipping);
            #endregion

            #region 参数
            SerializedProperty prop_Type = sp_StyleInfo.FindPropertyRelative("Type");
            SerializedProperty prop_Raycast = sp_StyleInfo.FindPropertyRelative("Raycast");
            SerializedProperty prop_Maskable = sp_StyleInfo.FindPropertyRelative("Maskable");
            SerializedProperty prop_SyncPrimitivePaintingColor = sp_StyleInfo.FindPropertyRelative("SyncPrimitivePaintingColor");

            #region Text属性
            SerializedProperty prop_overflow_h = sp_StyleInfo.FindPropertyRelative("Overflow_Horizon");
            SerializedProperty prop_overflow_v = sp_StyleInfo.FindPropertyRelative("Overflow_Vertical");
            SerializedProperty prop_autosize = sp_StyleInfo.FindPropertyRelative("BestFit");
            SerializedProperty prop_rich = sp_StyleInfo.FindPropertyRelative("RichText");
            SerializedProperty prop_alignGEO = sp_StyleInfo.FindPropertyRelative("GeometreAlign");
            SerializedProperty prop_style = sp_StyleInfo.FindPropertyRelative("Style");
            SerializedProperty prop_Font = sp_StyleInfo.FindPropertyRelative("Font");
            SerializedProperty prop_ContentAnchor = sp_StyleInfo.FindPropertyRelative("ContentAnchor");
            SerializedProperty prop_Size = sp_StyleInfo.FindPropertyRelative("Size");
            SerializedProperty prop_Fit_Min = sp_StyleInfo.FindPropertyRelative("Fit_Min");
            SerializedProperty prop_Fit_Max = sp_StyleInfo.FindPropertyRelative("Fit_Max");
            SerializedProperty prop_LineHeight = sp_StyleInfo.FindPropertyRelative("LineHeight");
            SerializedProperty prop_FontColor = sp_StyleInfo.FindPropertyRelative("FontColor");
            #endregion

            #region TmpText属性
            SerializedProperty prop_tmp_rich = sp_StyleInfo.FindPropertyRelative("tmp_rich");
            SerializedProperty prop_tmp_contentwrap = sp_StyleInfo.FindPropertyRelative("tmp_contentwrap");
            SerializedProperty prop_tmp_overflow = sp_StyleInfo.FindPropertyRelative("tmp_overflow");
            SerializedProperty prop_tmp_style = sp_StyleInfo.FindPropertyRelative("tmp_style");
            SerializedProperty prop_tmp_anchor = sp_StyleInfo.FindPropertyRelative("tmp_anchor");
            SerializedProperty prop_tmp_font = sp_StyleInfo.FindPropertyRelative("tmp_font");
            SerializedProperty prop_tmp_size = sp_StyleInfo.FindPropertyRelative("tmp_size");
            SerializedProperty prop_tmp_space_character = sp_StyleInfo.FindPropertyRelative("tmp_space_character");
            SerializedProperty prop_tmp_space_word = sp_StyleInfo.FindPropertyRelative("tmp_space_word");
            SerializedProperty prop_tmp_space_lineheight = sp_StyleInfo.FindPropertyRelative("tmp_space_lineheight");
            SerializedProperty prop_tmp_space_paragraph = sp_StyleInfo.FindPropertyRelative("tmp_space_paragraph");
            SerializedProperty prop_tmp_WrappingRatios = sp_StyleInfo.FindPropertyRelative("tmp_WrappingRatios");
            SerializedProperty prop_tmp_contentmargin = sp_StyleInfo.FindPropertyRelative("tmp_contentmargin");
            SerializedProperty prop_tmp_color = sp_StyleInfo.FindPropertyRelative("tmp_color");
            SerializedProperty prop_tmp_EnableAutoSizing = sp_StyleInfo.FindPropertyRelative("tmp_EnableAutoSizing");
            SerializedProperty prop_tmp_FontSizeMin = sp_StyleInfo.FindPropertyRelative("tmp_FontSizeMin");
            SerializedProperty prop_tmp_FontSizeMax = sp_StyleInfo.FindPropertyRelative("tmp_FontSizeMax");
            SerializedProperty prop_tmp_CharWidthMaxAdj = sp_StyleInfo.FindPropertyRelative("tmp_CharWidthMaxAdj");
            SerializedProperty prop_tmp_LineSpacingMax = sp_StyleInfo.FindPropertyRelative("tmp_LineSpacingMax");
            SerializedProperty prop_gra_A = sp_StyleInfo.FindPropertyRelative("gra_A");
            SerializedProperty prop_gra_B = sp_StyleInfo.FindPropertyRelative("gra_B");
            SerializedProperty prop_gra_C = sp_StyleInfo.FindPropertyRelative("gra_C");
            SerializedProperty prop_gra_D = sp_StyleInfo.FindPropertyRelative("gra_D");
            SerializedProperty prop_gra_Invert = sp_StyleInfo.FindPropertyRelative("gra_Invert");
            SerializedProperty prop_gra_Used = sp_StyleInfo.FindPropertyRelative("gra_Used");
            SerializedProperty prop_gra_ColorMode = sp_StyleInfo.FindPropertyRelative("gra_ColorMode");
            SerializedProperty prop_gra_ColorModeName = sp_StyleInfo.FindPropertyRelative("gra_ColorModeName");
            #endregion

            xHud_TextType textType = (xHud_TextType)prop_Type.enumValueIndex;

            // 类型标识
            Rect rect_mark = new Rect(rect.x + (rect.width - 80), rect.y + 15, 65, XGUI.GetSingleLineHeight());
            XGUI.gui_label(
                rect: rect_mark,
                bg_fill: XGUIFilled.实体,
                bg_color: XGUIColor.亮白,
                bg_color_gui: Color.white,
                text: new GUIContent(textType.ToString()),
                text_color: Color.black,
                size: XGUIFontSize.S,
                anchor: TextAnchor.MiddleCenter,
                offset: new Vector2(0, -2),
                clipping: clipping);

            XGUI.layout_space(100);

            #region 样式信息
            XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.Theme_Group,
                title: "样式信息",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.Theme_Primary,
                title_clipping: TextClipping.Clip,
                margin: new RectOffset(10, 10, 0, 0),
                padding: new RectOffset(10, 10, 15, 15));

            Rect Description_Rect = XGUI.GetLastRect();

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
            #endregion

            if (textType == xHud_TextType.Text)
            {
                #region 参数
                XGUI.layout_group_start(
                  type: XGUIContainerType.Vertical,
                  bg_fill: XGUIFilled.缺口纯色边框,
                  bg_color: XGUIColor.亮白,
                  bg_color_gui: XHud_Dashboard.Theme_Group,
                  title: "参数",
                  title_size: XGUIFontSize.M,
                  title_text_color: XHud_Dashboard.Theme_Primary,
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
                    opt_bg_fill: XGUIFilled.实体,
                    opt_bg_color: XGUIColor.亮白,
                    opt_bg_color_gui: XHud_Dashboard.Theme_Primary,
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
                    opt_bg_fill: XGUIFilled.实体,
                    opt_bg_color: XGUIColor.亮白,
                    opt_bg_color_gui: XHud_Dashboard.Theme_Primary,
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
                    opt_bg_fill: XGUIFilled.实体,
                    opt_bg_color: XGUIColor.亮白,
                    opt_bg_color_gui: XHud_Dashboard.Theme_Primary,
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
                    opt_bg_fill: XGUIFilled.实体,
                    opt_bg_color: XGUIColor.亮白,
                    opt_bg_color_gui: XHud_Dashboard.Theme_Primary,
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
                    bg_fill: XGUIFilled.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.Theme_Group,
                    title: "选项",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.Theme_Primary,
                    title_clipping: TextClipping.Clip,
                    margin: new RectOffset(10, 10, 0, 0),
                    padding: new RectOffset(10, 10, 15, 15));

                #region 颜色接管
                XGUI.layout_toggle(
                    title: "颜色接管",
                    title_size: XGUIFontSize.M,
                    title_font_style: FontStyle.Normal,
                    title_padding: new RectOffset(0, 10, 0, 0),
                    title_width: 60,
                    prop: prop_SyncPrimitivePaintingColor,
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
                    tog_bg_on_color: XHud_Dashboard.Theme_Primary,
                    tog_handler_off_color: Color.white,
                    tog_handler_on_color: Color.white,
                    tog_mixed_bg_color_gui: XHud_Dashboard.Theme_Primary,
                    act_on_changed: null);
                #endregion

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
                    tog_bg_on_color: XHud_Dashboard.Theme_Primary,
                    tog_handler_off_color: Color.white,
                    tog_handler_on_color: Color.white,
                    tog_mixed_bg_color_gui: XHud_Dashboard.Theme_Primary,
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
                    tog_bg_on_color: XHud_Dashboard.Theme_Primary,
                    tog_handler_off_color: Color.white,
                    tog_handler_on_color: Color.white,
                    tog_mixed_bg_color_gui: XHud_Dashboard.Theme_Primary,
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
                    tog_bg_on_color: XHud_Dashboard.Theme_Primary,
                    tog_handler_off_color: Color.white,
                    tog_handler_on_color: Color.white,
                    tog_mixed_bg_color_gui: XHud_Dashboard.Theme_Primary,
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
                    tog_bg_on_color: XHud_Dashboard.Theme_Primary,
                    tog_handler_off_color: Color.white,
                    tog_handler_on_color: Color.white,
                    tog_mixed_bg_color_gui: XHud_Dashboard.Theme_Primary,
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
                    tog_bg_on_color: XHud_Dashboard.Theme_Primary,
                    tog_handler_off_color: Color.white,
                    tog_handler_on_color: Color.white,
                    tog_mixed_bg_color_gui: XHud_Dashboard.Theme_Primary,
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
                  bg_fill: XGUIFilled.缺口纯色边框,
                  bg_color: XGUIColor.亮白,
                  bg_color_gui: XHud_Dashboard.Theme_Group,
                  title: "参数",
                  title_size: XGUIFontSize.M,
                  title_text_color: XHud_Dashboard.Theme_Primary,
                  title_clipping: TextClipping.Clip,
                  margin: new RectOffset(10, 10, 0, 0),
                  padding: new RectOffset(10, 10, 15, 15));

                ToolbarIndex = XGUI.layout_toolbar(
                    index: ref ToolbarIndex,
                    names: new string[] { "特征", "间距", "渐变色" },
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
                    navigate_style: true,
                    navigate_style_bg: XGUIFilled.纯色边框,
                    navigate_style_bg_color: Color.black * 0.5f);

                switch (ToolbarIndex)
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
                            opt_bg_fill: XGUIFilled.实体,
                            opt_bg_color: XGUIColor.亮白,
                            opt_bg_color_gui: XHud_Dashboard.Theme_Primary,
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
                            color: XHud_Dashboard.Theme_SeperateLine,
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
                            prop: prop_tmp_contentmargin.FindPropertyRelative("x"),
                            prop_padding: new RectOffset(0, 0, 0, 0),
                            prop_margin: new RectOffset(0, 0, 0, 10));
                        #endregion

                        #region 上边距
                        XGUI.layout_property_field(
                            title: "上边距",
                            title_size: XGUIFontSize.M,
                            title_hover_color: Color.white,
                            title_width: 140,
                            prop: prop_tmp_contentmargin.FindPropertyRelative("y"),
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
                            prop: prop_tmp_contentmargin.FindPropertyRelative("z"),
                            prop_padding: new RectOffset(0, 0, 0, 0),
                            prop_margin: new RectOffset(0, 0, 0, 10));
                        #endregion

                        #region 下边距
                        XGUI.layout_property_field(
                            title: "下边距",
                            title_size: XGUIFontSize.M,
                            title_hover_color: Color.white,
                            title_width: 140,
                            prop: prop_tmp_contentmargin.FindPropertyRelative("w"),
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
                            tog_bg_on_color: XHud_Dashboard.Theme_Primary,
                            tog_handler_off_color: Color.white,
                            tog_handler_on_color: Color.white,
                            tog_mixed_bg_color_gui: XHud_Dashboard.Theme_Primary,
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
                            tog_bg_on_color: XHud_Dashboard.Theme_Primary,
                            tog_handler_off_color: Color.white,
                            tog_handler_on_color: Color.white,
                            tog_mixed_bg_color_gui: XHud_Dashboard.Theme_Primary,
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
                            opt_bg_fill: XGUIFilled.实体,
                            opt_bg_color: XGUIColor.亮白,
                            opt_bg_color_gui: XHud_Dashboard.Theme_Primary,
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
                    bg_fill: XGUIFilled.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.Theme_Group,
                    title: "选项",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.Theme_Primary,
                    title_clipping: TextClipping.Clip,
                    margin: new RectOffset(10, 10, 0, 0),
                    padding: new RectOffset(10, 10, 15, 15));

                #region 颜色接管
                XGUI.layout_toggle(
                    title: "颜色接管",
                    title_size: XGUIFontSize.M,
                    title_font_style: FontStyle.Normal,
                    title_padding: new RectOffset(0, 10, 0, 0),
                    title_width: 60,
                    prop: prop_SyncPrimitivePaintingColor,
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
                    tog_bg_on_color: XHud_Dashboard.Theme_Primary,
                    tog_handler_off_color: Color.white,
                    tog_handler_on_color: Color.white,
                    tog_mixed_bg_color_gui: XHud_Dashboard.Theme_Primary,
                    act_on_changed: null);
                #endregion

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
                    tog_bg_on_color: XHud_Dashboard.Theme_Primary,
                    tog_handler_off_color: Color.white,
                    tog_handler_on_color: Color.white,
                    tog_mixed_bg_color_gui: XHud_Dashboard.Theme_Primary,
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
                    tog_bg_on_color: XHud_Dashboard.Theme_Primary,
                    tog_handler_off_color: Color.white,
                    tog_handler_on_color: Color.white,
                    tog_mixed_bg_color_gui: XHud_Dashboard.Theme_Primary,
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
                    tog_bg_on_color: XHud_Dashboard.Theme_Primary,
                    tog_handler_off_color: Color.white,
                    tog_handler_on_color: Color.white,
                    tog_mixed_bg_color_gui: XHud_Dashboard.Theme_Primary,
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
                    tog_bg_on_color: XHud_Dashboard.Theme_Primary,
                    tog_handler_off_color: Color.white,
                    tog_handler_on_color: Color.white,
                    tog_mixed_bg_color_gui: XHud_Dashboard.Theme_Primary,
                    act_on_changed: null);
                #endregion

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                #endregion

                //this.minSize = new Vector2(this.maxSize.x, 620);
            }
            #endregion

            BaseObject.ApplyModifiedProperties();

            Repaint();

            XGUI.layout_space(10);

            Buttons();

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

            if (isClosing)
                return;
            if (GUI.changed)
            {
                //反向更新源文字效果
                if (Component_Text != null)
                    Component_Text.txt_Set_Style_ForSetter(StyleInfo);
                if (Component_TmpText != null)
                    Component_TmpText.tmp_Update_Style_LibrarySetter(StyleInfo);
            }
        }

        #region 辅助
        /// <summary>
        /// 发送到库
        /// </summary>
        private void SendToLibrary()
        {
            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

            XHud_LibraryArg_TextStyle info = new XHud_LibraryArg_TextStyle();
            info.CopyData(StyleInfo);
            info.Name = sp_LibName.stringValue;
            info.Description = sp_Description.stringValue;

            if (sp_LibName.stringValue == "文字样式名称")
            {
                XGUI.dialog(
                    type: XGUIDialogType.警告,
                    windowtitle: "XHud - 文字样式库采集器消息",
                    title: "未填写名称",
                    msg: $"请为文字样式模版添加一个名称！",
                    ok: "明白",
                    PrimaryIndex: 0,
                    usemodal: true,
                    themecolor: XHud_Dashboard.Theme_Primary);
                return;
            }

            string colorhex = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary, true);

            bool exist = Target_Hud_TextStyleLibrary.TextStyle_Library_NameIsValid(sp_LibName.stringValue, info.Type);

            if (exist)
            {
                XGUI.dialog(
                    type: XGUIDialogType.警告,
                    windowtitle: "XHud - 文字样式库采集器消息",
                    title: "存在重复文字样式名称",
                    msg: $"名称为<color={colorhex}> {sp_LibName.stringValue} </color>的已经存在于文字样式库中！",
                    ok: "重命名",
                    PrimaryIndex: 0,
                    usemodal: true,
                    themecolor: XHud_Dashboard.Theme_Primary);
                return;
            }
            else
            {
                Target_Hud_TextStyleLibrary.TextStyle_Library_Add(info);

                XGUI.dialog(
                    type: XGUIDialogType.确认,
                    windowtitle: "XHud - 文字样式库采集器消息",
                    title: "已添加到文字样式库",
                    msg: $"已将名称为<color={colorhex}> {sp_LibName.stringValue} </color>的文字样式参数添加到文字样式库中！",
                    ok: "明白",
                    PrimaryIndex: 0,
                    usemodal: true,
                    themecolor: XHud_Dashboard.Theme_Primary);
                Close();
            }
        }

        /// <summary>
        /// 从库更新
        /// </summary>
        private void UpdateToLibrary()
        {
            string colorhex = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary, true);

            string res = XGUI.dialog(
                type: XGUIDialogType.警告,
                windowtitle: "XHud - 文字样式库采集器消息",
                title: "更新文字样式模版",
                msg: $"即将更新文字样式库中名称为： <color={colorhex}> {OriginStyleInfo.Name} </color> 且类型为： {OriginStyleInfo.Type} 的文字样式模版参数，确认更新参数吗？",
                ok: "更新",
                cancel: "暂不",
                PrimaryIndex: 0,
                usemodal: true,
                themecolor: XHud_Dashboard.Theme_Primary);

            if (res == "更新")
            {
                XHud_LibraryArg_TextStyle info = new XHud_LibraryArg_TextStyle();
                info.CopyData_Ignored_LibraryToggle(StyleInfo);
                info.Name = sp_LibName.stringValue;
                info.Type = StyleInfo.Type;
                info.Description = sp_Description.stringValue;
                Target_Hud_TextStyleLibrary.TextStyle_Library_Replace(ModifiedIndex, info);

                Close();
            }
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
        /// 设置模版基础信息（针对从库源修改更新使用）
        /// </summary>
        /// <param name="libname"></param>
        /// <param name="decription"></param>
        public void SetInfo(string name, string description)
        {
            LibName = name;
            Description = description;
        }

        /// <summary>
        /// 设置模版样式信息
        /// </summary>
        /// <param name="info"></param>        
        public void SetStyle(XHud_LibraryArg_TextStyle info)
        {
            StyleInfo.CopyData(info);
        }

        /// <summary>
        /// 设置模版样式信息
        /// </summary>
        /// <param name="info"></param>        
        public void SetOriginStyle(XHud_LibraryArg_TextStyle info)
        {
            OriginStyleInfo.CopyData(info);
        }

        /// <summary>
        /// 设置组件
        /// </summary>
        /// <param name="info"></param>        
        public void SetComponent(XHud_Module_Text com)
        {
            Component_Text = com;
        }

        /// <summary>
        /// 设置组件
        /// </summary>
        /// <param name="info"></param>        
        public void SetComponent(XHud_Module_TmpText com)
        {
            Component_TmpText = com;
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

        public void Set_Target_Hud_TextStyleLibrary(XHud_Library_TextStyle lib)
        {
            Target_Hud_TextStyleLibrary = lib;
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
                return;
            }

            XGUI.layout_flexspace();

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            #endregion
        }
        #endregion
    }
}