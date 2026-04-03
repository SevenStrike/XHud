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
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.Utilitys;
    using System;
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

        private Texture2D icon_libsetter_text;

        public LibrarySetterMode LibrarySetterMode;

        /// <summary>
        /// 字体 - 粗体
        /// </summary>
        Font Font_Bold;
        /// <summary>
        /// 字体 - 细体
        /// </summary>
        Font Font_Light;

        public int ModifiedIndex;

        #region 渐变色预览
        private Texture2D GradientTexture;
        private int gra_TexWidth = 110;
        private int gra_TexHeight = 110;
        #endregion

        [SerializeField]
        public string LibName;
        [SerializeField]
        public string DateTimes;
        [SerializeField]
        public string Description;
        [SerializeField]
        public string OriginLibName;
        [SerializeField]
        public xHud_TextType OriginType;

        /// <summary>
        /// 按钮宽度
        /// </summary>
        private float ButtonWidth = 110;
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

        Color SepLineColor = new Color(1, 1, 1, 0.15f);
        Color MessageColor = new Color(1, 1, 1, 0.62f);
        Color DateTimeColor = new Color(1, 1, 1, 0.42f);

        Rect Sepline_rect;
        Rect Title_rect;
        Rect Date_rect;
        Rect Icon_rect;

        public Motion_Creator CreateArgs;
        public Motion_Recycler RecycleArgs;

        string Title;

        private bool isClosing;

        Rect rect_group;
        Rect rect_param;

        string[] optnames_a = new string[] { "Text", "TmpText" };
        string[] optnames_b = new string[] { "原生", "接管" };
        string[] optnames_c = new string[] { "禁用", "启用" };
        string[] optnames_d = new string[] { "换行", "溢出" };
        string[] optnames_e = new string[] { "截断", "溢出" };
        string[] optnames_f = new string[] { "常规", "加粗", "斜体", "斜粗" };
        string[] optnames_g = new string[] { "单色", "水平", "垂直", "四角" };
        string[] optnames_h = new string[] { "正常", "翻转" };

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

            icon_libsetter_text = Editor_XHud_GUI.GetIcon("LibSetter/icon_libsetter_textstyle");

            Font_Bold = Editor_XHud_GUI.GetFont("SS_Editor_Bold");
            Font_Light = Editor_XHud_GUI.GetFont("SS_Editor_Dialog");

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
                    string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 文字样式库采集器消息", "恢复原有样式", "检测到您修正了样式参数，是否要恢复原有样式或者保持现有样式效果？", "保持", "恢复", 0);
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
                }
            }
            #endregion

            GetWindow<SceneView>().Focus();
        }

        private void OnGUI()
        {
            BaseObject.Update();

            Rect rect = new Rect(0, 0, position.width, position.height);

            Icon_rect = new Rect(26, 15, 48, 48);

            Editor_XHud_GUI.Gui_Icon(Icon_rect, icon_libsetter_text);

            Title_rect = new Rect(rect.x + 100, rect.y + 15, rect.width - 80, 30);
            Editor_XHud_GUI.Gui_Labelfield(Title_rect, Title, HudFilled.无, HudColor.无, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 20, Font_Bold);

            Sepline_rect = new Rect(rect.x + 102, rect.y + 60, 200, 1);
            Editor_XHud_GUI.Gui_Box(Sepline_rect, SepLineColor);

            Editor_XHud_GUI.Gui_Labelfield_Thin_WrapClip(new Rect(rect.x + 26, rect.y + 80, rect.width - 45, rect.height), "以下为待入库的文字样式参数概览，您可以检查每项参数是否符合您的要求，每项参数均可手动校正调整！", HudFilled.无, HudColor.无, MessageColor, TextAnchor.UpperLeft, new Vector2(0, 0), 12, true, Font_Light);

            DateTimes = DateTime.Now.ToString("yyyy-MM-dd  HH:mm:ss:ff");
            Date_rect = new Rect(rect.x + 150, rect.y + 15, rect.width - 180, rect.height);
            Editor_XHud_GUI.Gui_Labelfield_Thin_WrapClip(Date_rect, DateTimes, HudFilled.无, HudColor.无, DateTimeColor, TextAnchor.UpperRight, new Vector2(0, 0), 13, true, Font_Light);

            #region 样式信息
            string colorhex = XHud_Utilitys.Color_To_HexColor(XHud_Dashboard.Theme_Primary, true);

            #region 参数

            SerializedProperty prop_Type = sp_StyleInfo.FindPropertyRelative("Type");
            SerializedProperty prop_Raycast = sp_StyleInfo.FindPropertyRelative("Raycast");
            SerializedProperty prop_Maskable = sp_StyleInfo.FindPropertyRelative("Maskable");
            SerializedProperty prop_SyncAnimatorColor = sp_StyleInfo.FindPropertyRelative("SyncAnimatorColor");

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

            Color se_color = XHud_Utilitys.GetBrightnessLimite(XHud_Dashboard.Theme_Primary) ? Color.black : Color.white;

            xHud_TextType textType = (xHud_TextType)prop_Type.enumValueIndex;


            if (textType == xHud_TextType.Text)
            {
                rect_group = rect;

                #region 样式参数 - 类型
                rect_group.Set(rect.x + 25, rect.y + 145, 160, 50);
                Editor_XHud_GUI.Gui_Group(rect_group, HudFilled.纯色边框, HudColor.亮白, "类型", new Vector2(25, -8), XHud_Dashboard.Theme_Primary, Font_Light);
                prop_Type.enumValueIndex = Editor_XHud_GUI.Gui_ToolBar(new Rect(rect_group.x + 10, rect_group.y + 13, rect_group.width - 20, rect_group.height), optnames_a, prop_Type.enumValueIndex, HudFilled.纯色边框, HudFilled.实体, Color.white, Color.white, se_color);
                #endregion

                #region 样式参数 - 颜色接管
                rect_group.Set(rect.x + 25, rect.y + 210, 160, 50);
                Editor_XHud_GUI.Gui_Group(rect_group, HudFilled.纯色边框, HudColor.亮白, "颜色接管", new Vector2(25, -8), XHud_Dashboard.Theme_Primary, Font_Light);
                prop_SyncAnimatorColor.boolValue = Editor_XHud_GUI.Gui_Toggle(new Rect(rect_group.x + 10, rect_group.y + 13, rect_group.width - 20, rect_group.height), false, optnames_b, prop_SyncAnimatorColor.boolValue, HudFilled.无, HudColor.无, HudFilled.实体, XHud_Dashboard.Theme_Primary, Color.white, se_color);
                #endregion

                #region 样式参数 - 水平溢出
                rect_group.Set(rect.x + 25, rect.y + 275, 160, 50);
                Editor_XHud_GUI.Gui_Group(rect_group, HudFilled.纯色边框, HudColor.亮白, "水平溢出", new Vector2(25, -8), XHud_Dashboard.Theme_Primary, Font_Light);
                prop_overflow_h.enumValueIndex = Editor_XHud_GUI.Gui_ToolBar(new Rect(rect_group.x + 10, rect_group.y + 13, rect_group.width - 20, rect_group.height), optnames_d, prop_overflow_h.enumValueIndex, HudFilled.纯色边框, HudFilled.实体, Color.white, Color.white, se_color);
                #endregion

                #region 样式参数 - 垂直溢出
                rect_group.Set(rect.x + 25, rect.y + 340, 160, 50);
                Editor_XHud_GUI.Gui_Group(rect_group, HudFilled.纯色边框, HudColor.亮白, "垂直溢出", new Vector2(25, -8), XHud_Dashboard.Theme_Primary, Font_Light);
                prop_overflow_v.enumValueIndex = Editor_XHud_GUI.Gui_ToolBar(new Rect(rect_group.x + 10, rect_group.y + 13, rect_group.width - 20, rect_group.height), optnames_e, prop_overflow_v.enumValueIndex, HudFilled.纯色边框, HudFilled.实体, Color.white, Color.white, se_color);
                #endregion

                #region 样式参数 - 自动尺寸
                rect_group.Set(rect.x + 25, rect.y + 405, 160, 50);
                Editor_XHud_GUI.Gui_Group(rect_group, HudFilled.纯色边框, HudColor.亮白, "自动尺寸", new Vector2(25, -8), XHud_Dashboard.Theme_Primary, Font_Light);
                prop_autosize.boolValue = Editor_XHud_GUI.Gui_Toggle(new Rect(rect_group.x + 10, rect_group.y + 13, rect_group.width - 20, rect_group.height), false, optnames_c, prop_autosize.boolValue, HudFilled.无, HudColor.无, HudFilled.实体, XHud_Dashboard.Theme_Primary, Color.white, se_color);
                #endregion

                #region 样式参数 - 射线检测
                rect_group.Set(rect.x + 190, rect.y + 145, 160, 50);
                Editor_XHud_GUI.Gui_Group(rect_group, HudFilled.纯色边框, HudColor.亮白, "射线检测", new Vector2(25, -8), XHud_Dashboard.Theme_Primary, Font_Light);
                prop_Raycast.boolValue = Editor_XHud_GUI.Gui_Toggle(new Rect(rect_group.x + 10, rect_group.y + 13, rect_group.width - 20, rect_group.height), false, optnames_c, prop_Raycast.boolValue, HudFilled.无, HudColor.无, HudFilled.实体, XHud_Dashboard.Theme_Primary, Color.white, se_color);
                #endregion

                #region 样式参数 - 遮罩
                rect_group.Set(rect.x + 190, rect.y + 210, 160, 50);
                Editor_XHud_GUI.Gui_Group(rect_group, HudFilled.纯色边框, HudColor.亮白, "遮罩", new Vector2(25, -8), XHud_Dashboard.Theme_Primary, Font_Light);
                prop_Maskable.boolValue = Editor_XHud_GUI.Gui_Toggle(new Rect(rect_group.x + 10, rect_group.y + 13, rect_group.width - 20, rect_group.height), false, optnames_c, prop_Maskable.boolValue, HudFilled.无, HudColor.无, HudFilled.实体, XHud_Dashboard.Theme_Primary, Color.white, se_color);
                #endregion

                #region 样式参数 - 富文本
                rect_group.Set(rect.x + 190, rect.y + 275, 160, 50);
                Editor_XHud_GUI.Gui_Group(rect_group, HudFilled.纯色边框, HudColor.亮白, "富文本", new Vector2(25, -8), XHud_Dashboard.Theme_Primary, Font_Light);
                prop_rich.boolValue = Editor_XHud_GUI.Gui_Toggle(new Rect(rect_group.x + 10, rect_group.y + 13, rect_group.width - 20, rect_group.height), false, optnames_c, prop_rich.boolValue, HudFilled.无, HudColor.无, HudFilled.实体, XHud_Dashboard.Theme_Primary, Color.white, se_color);
                #endregion

                #region 样式参数 - 几何对齐
                rect_group.Set(rect.x + 190, rect.y + 340, 160, 50);
                Editor_XHud_GUI.Gui_Group(rect_group, HudFilled.纯色边框, HudColor.亮白, "几何对齐", new Vector2(25, -8), XHud_Dashboard.Theme_Primary, Font_Light);
                prop_alignGEO.boolValue = Editor_XHud_GUI.Gui_Toggle(new Rect(rect_group.x + 10, rect_group.y + 13, rect_group.width - 20, rect_group.height), false, optnames_c, prop_alignGEO.boolValue, HudFilled.无, HudColor.无, HudFilled.实体, XHud_Dashboard.Theme_Primary, Color.white, se_color);
                #endregion

                #region 样式参数 - 样式
                rect_group.Set(rect.x + 190, rect.y + 405, rect.width - 190 - 25, 50);
                Editor_XHud_GUI.Gui_Group(rect_group, HudFilled.纯色边框, HudColor.亮白, "字体样式", new Vector2(25, -8), XHud_Dashboard.Theme_Primary, Font_Light);
                prop_style.enumValueIndex = Editor_XHud_GUI.Gui_ToolBar(new Rect(rect_group.x + 10, rect_group.y + 13, rect_group.width - 20, rect_group.height), optnames_f, prop_style.enumValueIndex, HudFilled.纯色边框, HudFilled.实体, Color.white, Color.white, se_color);
                #endregion

                #region 样式参数 - 参数
                float offset = 18;
                float lineheight = 30;
                rect_group.Set(rect.x + 355, rect.y + 145, rect.width - 355 - 25, 245);
                Editor_XHud_GUI.Gui_Group(rect_group, HudFilled.纯色边框, HudColor.亮白, "参数", new Vector2(25, -8), XHud_Dashboard.Theme_Primary, Font_Light);

                rect_param = rect_group;
                ParamDisplayer(rect_param, prop_Font, "字体", 10, 210, 40, offset);
                offset += lineheight;
                ParamDisplayer(rect_param, prop_ContentAnchor, "锚点", 10, 210, 40, offset);
                offset += lineheight;
                ParamDisplayer(rect_param, prop_Size, "尺寸", 10, 210, 40, offset);
                offset += lineheight;
                if (!prop_autosize.boolValue)
                    Editor_XHud_GUI.SetEnabled(false);
                ParamDisplayer(rect_param, prop_Fit_Min, "最小尺寸", 10, 210, 60, offset);
                offset += lineheight;
                ParamDisplayer(rect_param, prop_Fit_Max, "最大尺寸", 10, 210, 60, offset);
                offset += lineheight;
                Editor_XHud_GUI.SetEnabled(true);
                ParamDisplayer(rect_param, prop_LineHeight, "行高", 10, 210, 40, offset);
                offset += lineheight;
                ParamDisplayer(rect_param, prop_FontColor, "颜色", 10, 210, 40, offset);
                offset += lineheight;
                #endregion

                this.minSize = new Vector2(630, 620);
                this.maxSize = this.minSize;
            }
            else
            {
                rect_group = rect;

                #region 样式参数 - 类型
                rect_group.Set(rect.x + 25, rect.y + 145, 160, 50);
                Editor_XHud_GUI.Gui_Group(rect_group, HudFilled.纯色边框, HudColor.亮白, "类型", new Vector2(25, -8), XHud_Dashboard.Theme_Primary, Font_Light);
                prop_Type.enumValueIndex = Editor_XHud_GUI.Gui_ToolBar(new Rect(rect_group.x + 10, rect_group.y + 13, rect_group.width - 20, rect_group.height), optnames_a, prop_Type.enumValueIndex, HudFilled.纯色边框, HudFilled.实体, Color.white, Color.white, se_color);
                #endregion

                #region 样式参数 - 颜色接管
                rect_group.Set(rect.x + 25, rect.y + 210, 160, 50);
                Editor_XHud_GUI.Gui_Group(rect_group, HudFilled.纯色边框, HudColor.亮白, "颜色接管", new Vector2(25, -8), XHud_Dashboard.Theme_Primary, Font_Light);
                prop_SyncAnimatorColor.boolValue = Editor_XHud_GUI.Gui_Toggle(new Rect(rect_group.x + 10, rect_group.y + 13, rect_group.width - 20, rect_group.height), false, optnames_b, prop_SyncAnimatorColor.boolValue, HudFilled.无, HudColor.无, HudFilled.实体, XHud_Dashboard.Theme_Primary, Color.white, se_color);
                #endregion

                #region 样式参数 - 自动尺寸
                rect_group.Set(rect.x + 25, rect.y + 275, 160, 50);
                Editor_XHud_GUI.Gui_Group(rect_group, HudFilled.纯色边框, HudColor.亮白, "自动尺寸", new Vector2(25, -8), XHud_Dashboard.Theme_Primary, Font_Light);
                prop_tmp_EnableAutoSizing.boolValue = Editor_XHud_GUI.Gui_Toggle(new Rect(rect_group.x + 10, rect_group.y + 13, rect_group.width - 20, rect_group.height), false, optnames_c, prop_tmp_EnableAutoSizing.boolValue, HudFilled.无, HudColor.无, HudFilled.实体, XHud_Dashboard.Theme_Primary, Color.white, se_color);
                #endregion

                #region 样式参数 - 渐变色方向
                rect_group.Set(rect.x + 25, rect.y + 340, 160, 50);
                Editor_XHud_GUI.Gui_Group(rect_group, HudFilled.纯色边框, HudColor.亮白, "渐变色方向", new Vector2(25, -8), XHud_Dashboard.Theme_Primary, Font_Light);
                if (!prop_gra_Used.boolValue)
                    Editor_XHud_GUI.SetEnabled(false);
                prop_gra_Invert.boolValue = Editor_XHud_GUI.Gui_Toggle(new Rect(rect_group.x + 10, rect_group.y + 13, rect_group.width - 20, rect_group.height), false, optnames_h, prop_gra_Invert.boolValue, HudFilled.无, HudColor.无, HudFilled.实体, XHud_Dashboard.Theme_Primary, Color.white, se_color);
                Editor_XHud_GUI.SetEnabled(true);
                #endregion

                #region 样式参数 - 射线检测
                rect_group.Set(rect.x + 190, rect.y + 145, 160, 50);
                Editor_XHud_GUI.Gui_Group(rect_group, HudFilled.纯色边框, HudColor.亮白, "射线检测", new Vector2(25, -8), XHud_Dashboard.Theme_Primary, Font_Light);
                prop_Raycast.boolValue = Editor_XHud_GUI.Gui_Toggle(new Rect(rect_group.x + 10, rect_group.y + 13, rect_group.width - 20, rect_group.height), false, optnames_c, prop_Raycast.boolValue, HudFilled.无, HudColor.无, HudFilled.实体, XHud_Dashboard.Theme_Primary, Color.white, se_color);
                #endregion

                #region 样式参数 - 遮罩
                rect_group.Set(rect.x + 190, rect.y + 210, 160, 50);
                Editor_XHud_GUI.Gui_Group(rect_group, HudFilled.纯色边框, HudColor.亮白, "遮罩", new Vector2(25, -8), XHud_Dashboard.Theme_Primary, Font_Light);
                prop_Maskable.boolValue = Editor_XHud_GUI.Gui_Toggle(new Rect(rect_group.x + 10, rect_group.y + 13, rect_group.width - 20, rect_group.height), false, optnames_c, prop_Maskable.boolValue, HudFilled.无, HudColor.无, HudFilled.实体, XHud_Dashboard.Theme_Primary, Color.white, se_color);
                #endregion

                #region 样式参数 - 富文本
                rect_group.Set(rect.x + 190, rect.y + 275, 160, 50);
                Editor_XHud_GUI.Gui_Group(rect_group, HudFilled.纯色边框, HudColor.亮白, "富文本", new Vector2(25, -8), XHud_Dashboard.Theme_Primary, Font_Light);
                prop_tmp_rich.boolValue = Editor_XHud_GUI.Gui_Toggle(new Rect(rect_group.x + 10, rect_group.y + 13, rect_group.width - 20, rect_group.height), false, optnames_c, prop_tmp_rich.boolValue, HudFilled.无, HudColor.无, HudFilled.实体, XHud_Dashboard.Theme_Primary, Color.white, se_color);
                #endregion

                #region 样式参数 - 渐变色支持
                rect_group.Set(rect.x + 190, rect.y + 340, 160, 50);
                Editor_XHud_GUI.Gui_Group(rect_group, HudFilled.纯色边框, HudColor.亮白, "渐变色支持", new Vector2(25, -8), XHud_Dashboard.Theme_Primary, Font_Light);
                prop_gra_Used.boolValue = Editor_XHud_GUI.Gui_Toggle(new Rect(rect_group.x + 10, rect_group.y + 13, rect_group.width - 20, rect_group.height), false, optnames_c, prop_gra_Used.boolValue, HudFilled.无, HudColor.无, HudFilled.实体, XHud_Dashboard.Theme_Primary, Color.white, se_color);
                #endregion

                #region 样式参数 - 渐变色模式
                rect_group.Set(rect.x + 25, rect.y + 405, 325, 50);
                Editor_XHud_GUI.Gui_Group(rect_group, HudFilled.纯色边框, HudColor.亮白, "渐变色模式", new Vector2(25, -8), XHud_Dashboard.Theme_Primary, Font_Light);
                if (!prop_gra_Used.boolValue)
                    Editor_XHud_GUI.SetEnabled(false);
                prop_gra_ColorMode.enumValueIndex = Editor_XHud_GUI.Gui_ToolBar(new Rect(rect_group.x + 10, rect_group.y + 13, rect_group.width - 20, rect_group.height), optnames_g, prop_gra_ColorMode.enumValueIndex, HudFilled.纯色边框, HudFilled.实体, Color.white, Color.white, se_color);
                Editor_XHud_GUI.SetEnabled(true);
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

                #region 样式参数 - 参数
                float offset = 18;
                float lineheight = 28;

                rect_group.Set(rect.x + 355, rect.y + 145, rect.width - 355 - 20, 310);
                Editor_XHud_GUI.Gui_Group(rect_group, HudFilled.纯色边框, HudColor.亮白, "参数", new Vector2(25, -8), XHud_Dashboard.Theme_Primary, Font_Light);

                rect_param = rect_group;
                ParamDisplayer(rect_param, prop_tmp_font, "字体", 10, 150, 40, offset);
                offset += lineheight;
                ParamDisplayer(rect_param, prop_tmp_contentwrap, "包裹", 10, 150, 40, offset);
                offset += lineheight;
                ParamDisplayer(rect_param, prop_tmp_overflow, "溢出", 10, 150, 40, offset);
                offset += lineheight;
                ParamDisplayer(rect_param, prop_tmp_style, "样式", 10, 150, 40, offset);
                offset += lineheight;
                ParamDisplayer(rect_param, prop_tmp_anchor, "锚点", 10, 150, 40, offset);
                offset += lineheight;
                ParamDisplayer(rect_param, prop_tmp_size, "尺寸", 10, 150, 40, offset);
                offset += lineheight;
                ParamDisplayer(rect_param, prop_tmp_color, "颜色", 10, 150, 40, offset);
                offset += lineheight;
                ParamDisplayer(rect_param, prop_tmp_WrappingRatios, "包裹比例", 10, 150, 65, offset);
                offset += lineheight;
                if (!prop_tmp_EnableAutoSizing.boolValue)
                    Editor_XHud_GUI.SetEnabled(false);
                ParamDisplayer(rect_param, prop_tmp_FontSizeMin, "最小尺寸", 10, 150, 65, offset);
                offset += lineheight;
                ParamDisplayer(rect_param, prop_tmp_FontSizeMax, "最大尺寸", 10, 150, 65, offset);
                offset = 18;
                ParamDisplayer(rect_param, prop_tmp_CharWidthMaxAdj, "最大字符宽度", 175, 150, 90, offset);
                offset += lineheight;
                ParamDisplayer(rect_param, prop_tmp_LineSpacingMax, "最大行高", 175, 150, 65, offset);
                Editor_XHud_GUI.SetEnabled(true);
                offset += lineheight;
                ParamDisplayer(rect_param, prop_tmp_space_character, "字符间距", 175, 150, 65, offset);
                offset += lineheight;
                ParamDisplayer(rect_param, prop_tmp_space_word, "单词间距", 175, 150, 65, offset);
                offset += lineheight;
                ParamDisplayer(rect_param, prop_tmp_space_lineheight, "行高间距", 175, 150, 65, offset);
                offset += lineheight;
                ParamDisplayer(rect_param, prop_tmp_space_paragraph, "段落间距", 175, 150, 65, offset);
                offset += lineheight;
                ParamDisplayer(rect_param, prop_tmp_contentmargin.FindPropertyRelative("x"), "左边距", 175, 150, 65, offset);
                offset += lineheight;
                ParamDisplayer(rect_param, prop_tmp_contentmargin.FindPropertyRelative("y"), "上边距", 175, 150, 65, offset);
                offset += lineheight;
                ParamDisplayer(rect_param, prop_tmp_contentmargin.FindPropertyRelative("z"), "右边距", 175, 150, 65, offset);
                offset += lineheight;
                ParamDisplayer(rect_param, prop_tmp_contentmargin.FindPropertyRelative("w"), "下边距", 175, 150, 65, offset);
                offset += lineheight;
                #endregion

                #region 渐变色

                if (prop_gra_Used.boolValue)
                {
                    CreateGradientTexture((ColorMode)prop_gra_ColorMode.enumValueIndex, prop_gra_Invert.boolValue, prop_gra_A.colorValue, prop_gra_B.colorValue, prop_gra_C.colorValue, prop_gra_D.colorValue);
                    rect_group.Set(rect.width - 145, rect.y + 163, gra_TexWidth, gra_TexHeight);
                    Editor_XHud_GUI.Gui_Icon(rect_group, GradientTexture);

                    switch (prop_gra_ColorModeName.stringValue)
                    {
                        case "单色":
                            rect_group.Set(rect.width - 145, rect.y + 300, gra_TexWidth, 20);
                            Editor_XHud_GUI.Gui_ColorField(rect_group, prop_gra_A);
                            break;
                        case "水平渐变":
                            rect_group.Set(rect.width - 145, rect.y + 300, gra_TexWidth, 20);
                            Editor_XHud_GUI.Gui_ColorField(rect_group, prop_gra_A);
                            rect_group.Set(rect.width - 145, rect.y + 330, gra_TexWidth, 20);
                            Editor_XHud_GUI.Gui_ColorField(rect_group, prop_gra_B);
                            break;
                        case "垂直渐变":
                            rect_group.Set(rect.width - 145, rect.y + 300, gra_TexWidth, 20);
                            Editor_XHud_GUI.Gui_ColorField(rect_group, prop_gra_A);
                            rect_group.Set(rect.width - 145, rect.y + 330, gra_TexWidth, 20);
                            Editor_XHud_GUI.Gui_ColorField(rect_group, prop_gra_B);
                            break;
                        case "四角渐变":
                            rect_group.Set(rect.width - 145, rect.y + 300, gra_TexWidth, 20);
                            Editor_XHud_GUI.Gui_ColorField(rect_group, prop_gra_A);
                            rect_group.Set(rect.width - 145, rect.y + 330, gra_TexWidth, 20);
                            Editor_XHud_GUI.Gui_ColorField(rect_group, prop_gra_B);
                            rect_group.Set(rect.width - 145, rect.y + 360, gra_TexWidth, 20);
                            Editor_XHud_GUI.Gui_ColorField(rect_group, prop_gra_C);
                            rect_group.Set(rect.width - 145, rect.y + 390, gra_TexWidth, 20);
                            Editor_XHud_GUI.Gui_ColorField(rect_group, prop_gra_D);
                            break;
                    }
                }

                #endregion

                if (prop_gra_Used.boolValue)
                    this.minSize = new Vector2(850, 620);
                else
                    this.minSize = new Vector2(730, 620);
                this.maxSize = this.minSize;
            }

            #endregion

            #endregion

            #region 入库名称
            Color LibName_color = Color.white;
            if (string.IsNullOrEmpty(sp_LibName.stringValue))
            {
                LibName_color = Color.gray;
                sp_LibName.stringValue = "文字样式名称";
            }
            else
            {
                if (sp_LibName.stringValue == "文字样式名称")
                    LibName_color = Color.gray;
                else
                    LibName_color = Color.white;
            }

            Rect LibName_Rect = new Rect(rect.x + 25, rect.height - 155, (rect.width / 2) - 100, 70);
            sp_LibName.stringValue = Editor_XHud_GUI.Gui_TextField(LibName_Rect, sp_LibName.stringValue, LibName_color, 12);
            sp_LibName.serializedObject.ApplyModifiedProperties();
            #endregion

            #region 说明文字
            Color Description_Color = Color.white;
            if (string.IsNullOrEmpty(sp_Description.stringValue))
            {
                Description_Color = Color.gray;
                sp_Description.stringValue = "文字样式参数说明内容";
            }
            else
            {
                if (sp_Description.stringValue == "文字样式参数说明内容")
                    Description_Color = Color.gray;
                else
                    Description_Color = Color.white;
            }

            Rect Description_Rect = new Rect((rect.width / 2) - 65, rect.height - 155, (rect.width / 2) + 45, 70);
            sp_Description.stringValue = Editor_XHud_GUI.Gui_TextField(Description_Rect, sp_Description.stringValue, Description_Color, 12);
            sp_Description.serializedObject.ApplyModifiedProperties();

            #endregion

            #region 样式类型
            Editor_XHud_GUI.Gui_Labelfield(new Rect(rect.x + 25, rect.height - 70, 200, 15), $"文字样式类型：<color={colorhex}>{textType.ToString()}</color>", HudFilled.无, HudColor.无, Color.white, false, Color.blue, TextAnchor.UpperLeft, Vector2.zero, 16, Font_Bold);

            if (textType == xHud_TextType.Text)
                Editor_XHud_GUI.Gui_Labelfield(new Rect(rect.x + 25, rect.height - 40, 200, 15), "该参数用于HudText的文字样式", HudFilled.无, HudColor.无, Color.white * 0.7f, false, Color.blue, TextAnchor.UpperLeft, Vector2.zero, 12, Font_Light);
            else
                Editor_XHud_GUI.Gui_Labelfield(new Rect(rect.x + 25, rect.height - 40, 200, 15), "该参数用于HudTmpText的文字样式", HudFilled.无, HudColor.无, Color.white * 0.7f, false, Color.blue, TextAnchor.UpperLeft, Vector2.zero, 12, Font_Light);
            #endregion

            BaseObject.ApplyModifiedProperties();

            Repaint();

            Editor_XHud_GUI.Gui_Layout_Space(580);

            DialogType_Buttons();

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

                // 检查点击位置是否在窗口内
                if (!LibName_Rect.Contains(e.mousePosition))
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
        /// 文字参数显示器
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="Title"></param>
        /// <param name="propname"></param>
        /// <param name="label_distance"></param>
        /// <param name="offset"></param>
        private void ParamDisplayer(Rect rect, SerializedProperty prop, string Title, float x_margin, float width, float label_distance, float offset)
        {
            rect.Set(rect_group.x + x_margin, rect_group.y + offset, width, 20);

            Editor_XHud_GUI.Gui_Property_Field(rect, Title, prop, 10, label_distance);
        }

        /// <summary>
        /// 创建渐变贴图的方法
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="invert"></param>
        /// <param name="col_start"></param>
        /// <param name="col_end"></param>
        /// <param name="col_start_cor"></param>
        /// <param name="col_end_cor"></param>
        private void CreateGradientTexture(ColorMode mode, bool invert, Color col_start, Color col_end, Color col_start_cor, Color col_end_cor)
        {
            // 创建一个新的Texture2D对象
            GradientTexture = new Texture2D(gra_TexWidth, gra_TexHeight);
            Color pixelColor = Color.white;
            Color verticalBlend = Color.white;
            Color horizontalBlend = Color.white;
            Color colorBlend = Color.white;

            switch (mode)
            {
                case ColorMode.Single:
                    for (int x = 0; x < gra_TexWidth; x++)
                    {
                        for (int y = 0; y < gra_TexHeight; y++)
                        {
                            pixelColor = col_start;
                            GradientTexture.SetPixel(x, y, pixelColor);
                        }
                    }
                    break;
                case ColorMode.HorizontalGradient:
                    // 遍历每个像素并设置颜色
                    for (int x = 0; x < gra_TexWidth; x++)
                    {
                        for (int y = 0; y < gra_TexHeight; y++)
                        {
                            // 计算水平渐变的插值
                            float gradientValue = (float)x / (gra_TexWidth - 1);

                            if (invert)
                                pixelColor = Color.Lerp(col_end, col_start, gradientValue);
                            else
                                pixelColor = Color.Lerp(col_start, col_end, gradientValue);

                            GradientTexture.SetPixel(x, y, pixelColor);
                        }
                    }
                    break;
                case ColorMode.VerticalGradient:
                    for (int y = 0; y < gra_TexHeight; y++)
                    {
                        for (int x = 0; x < gra_TexWidth; x++)
                        {
                            // 计算垂直渐变的插值
                            float gradientValue = (float)y / (gra_TexHeight - 1);
                            if (invert)
                                pixelColor = Color.Lerp(col_end, col_start, gradientValue);
                            else
                                pixelColor = Color.Lerp(col_start, col_end, gradientValue);
                            GradientTexture.SetPixel(x, y, pixelColor);
                        }
                    }
                    break;
                case ColorMode.FourCornersGradient:
                    for (int y = 0; y < gra_TexHeight; y++)
                    {
                        for (int x = 0; x < gra_TexWidth; x++)
                        {
                            // 归一化坐标
                            float u = x / (float)(gra_TexWidth - 1);
                            float v = y / (float)(gra_TexHeight - 1);

                            // 计算四个角的颜色插值
                            if (invert)
                            {
                                verticalBlend = Color.Lerp(col_end, col_start, u);        // 顶部水平插值
                                horizontalBlend = Color.Lerp(col_end_cor, col_start_cor, u); // 底部水平插值
                                colorBlend = Color.Lerp(horizontalBlend, verticalBlend, v);       // 垂直插值  
                            }
                            else
                            {
                                verticalBlend = Color.Lerp(col_start, col_end, u);        // 顶部水平插值
                                horizontalBlend = Color.Lerp(col_start_cor, col_end_cor, u); // 底部水平插值
                                colorBlend = Color.Lerp(horizontalBlend, verticalBlend, v);       // 垂直插值
                            }
                            // 计算当前像素到中心的距离（归一化）
                            float distanceToCenter = Mathf.Sqrt((u - 0.5f) * (u - 0.5f) + (v - 0.5f) * (v - 0.5f));
                            float normalizedDistance = 1.0f - (distanceToCenter * 2); // 从外到内，值从1到0

                            // 根据距离中心的距离调整颜色
                            Color finalColor = Color.Lerp(colorBlend, Color.white, normalizedDistance); // 中心颜色为白色

                            GradientTexture.SetPixel(x, y, finalColor);
                        }
                    }
                    break;
            }

            // 应用修改并压缩贴图
            GradientTexture.Apply();
        }

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
                Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 文字样式库采集器消息", "未填写名称", "请为文字样式模版添加一个名称！", "明白");
                return;
            }

            string colorhex = XHud_Utilitys.Color_To_HexColor(XHud_Dashboard.Theme_Primary, true);

            bool exist = Target_Hud_TextStyleLibrary.TextStyle_Library_NameIsValid(sp_LibName.stringValue, info.Type);

            if (exist)
            {
                string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 文字样式库采集器消息", "存在重复文字样式名称", $"名称为<color={colorhex}> {sp_LibName.stringValue} </color>的已经存在于文字样式库中！", "重命名", 1);
                return;
            }
            else
            {
                Target_Hud_TextStyleLibrary.TextStyle_Library_Add(info);

                Editor_XHud_GUI.Open(XHud_DialogType.确认, "XHud - 文字样式库采集器消息", "已添加到文字样式库", $"已将名称为<color={colorhex}> {sp_LibName.stringValue} </color>的文字样式参数添加到文字样式库中！", "明白");
                Close();
            }
        }

        /// <summary>
        /// 从库更新
        /// </summary>
        private void UpdateToLibrary()
        {
            string colorhex = XHud_Utilitys.Color_To_HexColor(XHud_Dashboard.Theme_Primary, true);

            string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 文字样式库采集器消息", "更新文字样式模版", $"即将更新文字样式库中名称为： <color={colorhex}> {OriginStyleInfo.Name} </color> 且类型为： {OriginStyleInfo.Type} 的文字样式模版参数，确认更新参数吗？", "更新", "暂不", 1);
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
        private void DialogType_Buttons()
        {
            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_XHud_GUI.Gui_Layout_FlexSpace();

            if (Editor_XHud_GUI.Gui_Layout_Button(ButtonText_Cancel, "", HudFilled.实体, HudColor.亮白, Color.black, 12, ButtonWidth, ButtonHeight, Font_Light, ButtonText_Cancel))
            {
                Close();
                return;
            }
            Editor_XHud_GUI.Gui_Layout_Space(ButtonDistance);
            GUI.backgroundColor = XHud_Dashboard.Theme_Primary;
            if (Editor_XHud_GUI.Gui_Layout_Button(ButtonText_Ok, "", HudFilled.实体, HudColor.亮白, XHud_Utilitys.GetBrightnessLimite(XHud_Dashboard.Theme_Primary) ? Color.black : Color.white, 12, ButtonWidth, ButtonHeight, Font_Light, ButtonText_Ok))
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
            GUI.backgroundColor = Color.white;

            Editor_XHud_GUI.Gui_Layout_Space(25);
            Editor_XHud_GUI.Gui_Layout_Horizontal_End();
        }
        #endregion

    }
}
