/*
 * ============================================================================
 * ⚠️ 版权声明（禁止删除、禁止修改、衍生作品必须保留此注释）⚠️
 * ============================================================================
 * 版权声明 Copyright (C) 2025-Present Nanjing SevenStrike Media Co., Ltd.
 * 中文名称：南京塞维斯传媒有限公司
 * 英文名称：SevenStrikeMedia
 * 项目作者：徐寅智
 * 项目名称：XGUI - Unity Editor界面可视化组件工具
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
namespace SevenStrikeModules.XGUI.Editor
{
    using SevenStrikeModules.XGUI.Runtime;
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(XGUI_Samples))]
    public class Editor_XGUI_Samples : Editor
    {
        private XGUI_Samples BaseScript;
        private SerializedProperty
            Hardware,
            Stones,
            Types,
            EaseMode,
            param_string,
            param_float,
            param_int,
            param_color,
            param_vector2,
            param_vector3,
            param_vector4,
            param_rectoffset,
            param_gameobject,
            param_curve,
            param_slider_hz,
            param_slider_volume,
            param_slider_distance,
            ThemeColor_Primary,
            ThemeColor_Secondary,
            ThemeColor_Group,
            state_toggle,
            state_toggle_A,
            state_toggle_B,
            state_toggle_C,
            state_toggle_D,
            state_toggle_E;

        private Texture2D[] icons_hardware, icons_stone, icon_avatars;
        private Texture2D icons_slider;

        private string hexcolor_primary;
        private string hexcolor_secondary;
        private string hexcolor_group;

        private string[] EaseNames;

        private Color[] colors;

        private Vector2 list_scrollPosition;
        private List<float> list_cachedItemHeights;
        private bool list_heightsDirty = true;
        public Texture2D list_IndexIcon;

        private void OnEnable()
        {
            BaseScript = (XGUI_Samples)target;

            hexcolor_primary = XGUI_Utilitys.Color_To_HexString(BaseScript.ThemeColor_Primary, true);
            hexcolor_secondary = XGUI_Utilitys.Color_To_HexString(BaseScript.ThemeColor_Secondary, true);
            hexcolor_group = XGUI_Utilitys.Color_To_HexString(BaseScript.ThemeColor_Group, true);

            Hardware = serializedObject.FindProperty("Hardware");
            Stones = serializedObject.FindProperty("Stones");
            Types = serializedObject.FindProperty("Types");
            EaseMode = serializedObject.FindProperty("EaseMode");
            param_string = serializedObject.FindProperty("param_string");
            param_float = serializedObject.FindProperty("param_float");
            param_int = serializedObject.FindProperty("param_int");
            param_color = serializedObject.FindProperty("param_color");
            param_vector2 = serializedObject.FindProperty("param_vector2");
            param_vector3 = serializedObject.FindProperty("param_vector3");
            param_vector4 = serializedObject.FindProperty("param_vector4");
            param_rectoffset = serializedObject.FindProperty("param_rectoffset");
            param_gameobject = serializedObject.FindProperty("param_gameobject");
            param_curve = serializedObject.FindProperty("param_curve");
            ThemeColor_Primary = serializedObject.FindProperty("ThemeColor_Primary");
            ThemeColor_Secondary = serializedObject.FindProperty("ThemeColor_Secondary");
            ThemeColor_Group = serializedObject.FindProperty("ThemeColor_Group");
            state_toggle = serializedObject.FindProperty("state_toggle");
            state_toggle_A = serializedObject.FindProperty("state_toggle_A");
            state_toggle_B = serializedObject.FindProperty("state_toggle_B");
            state_toggle_C = serializedObject.FindProperty("state_toggle_C");
            state_toggle_D = serializedObject.FindProperty("state_toggle_D");
            state_toggle_E = serializedObject.FindProperty("state_toggle_E");

            param_slider_hz = serializedObject.FindProperty("param_slider_hz");
            param_slider_volume = serializedObject.FindProperty("param_slider_volume");
            param_slider_distance = serializedObject.FindProperty("param_slider_distance");

            icons_hardware = XGUI.LoadAllAssetsAtPathWithPattern<Texture2D>($"{XGUI_Dashboard.get_path_xgui_icons()}Samples/Hardware/", ".png").ToArray();
            icons_stone = XGUI.LoadAllAssetsAtPathWithPattern<Texture2D>($"{XGUI_Dashboard.get_path_xgui_icons()}Samples/Stones/", ".png").ToArray();

            icons_slider = XGUI.GetIcon("Samples/Icons/icon_slider");

            list_IndexIcon = XGUI.GetBasedIcon("icon_field_status");

            EaseNames = new string[] { "InBack", "InBounce", "InCirc", "InCubic", "InElastic", "InExpo", "InOutBack", "InOutBounce", "InOutCirc", "InOutCubic", "InOutElastic", "InOutExpo", "InOutQuad", "InOutQuart", "InOutQuint", "InOutSine", "InQuad", "InQuart", "InQuint", "InSine", "Linear", "None", "OutBack", "OutBounce", "OutCirc", "OutCubic", "OutElastic", "OutExpo", "OutQuad", "OutQuart", "OutQuint", "OutSine" };

            #region 随机参数颜色
            colors = new Color[10];
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = XGUI_Utilitys.RandomRGB();
            }
            #endregion

            #region 创建列表测试数据
            BaseScript.CreateSampleData();

            List<Texture2D> avas = new List<Texture2D>();
            for (int i = 0; i < BaseScript.PlayerDatas.Count; i++)
            {
                Texture2D tex_ava = XGUI.GetIcon(BaseScript.PlayerDatas[i]._avatarpath);
                avas.Add(tex_ava);
            }
            icon_avatars = avas.ToArray();
            #endregion

            #region 创建卡片测试数据
            #region Edge
            BaseScript.CardDataWrap_Edge = new CardDataWrap();
            if (BaseScript.CardDataWrap_Edge._datas == null)
            {
                BaseScript.CardDataWrap_Edge._datas = new CardData[10];

                for (int i = 0; i < BaseScript.CardDataWrap_Edge._datas.Length; i++)
                {
                    BaseScript.CardDataWrap_Edge._datas[i] = new CardData();
                    BaseScript.CardDataWrap_Edge._datas[i]._title = "测试标题：" + i;
                    BaseScript.CardDataWrap_Edge._datas[i]._data = "测试数据：" + i;
                    Color col_card = XGUI_Utilitys.SetSaturation(XGUI_Utilitys.RandomRGB(), 0.7f);

                    BaseScript.CardDataWrap_Edge._datas[i]._bgcolor = col_card;
                    //BaseScript.CardDataWrap_Edge._datas[i]._bgcolor = Color.white;
                }
            }
            #endregion

            #region Solid
            BaseScript.CardDataWrap_Solid = new CardDataWrap();
            if (BaseScript.CardDataWrap_Solid._datas == null)
            {
                BaseScript.CardDataWrap_Solid._datas = new CardData[10];

                for (int i = 0; i < BaseScript.CardDataWrap_Solid._datas.Length; i++)
                {
                    BaseScript.CardDataWrap_Solid._datas[i] = new CardData();
                    BaseScript.CardDataWrap_Solid._datas[i]._title = "测试标题：" + i;
                    BaseScript.CardDataWrap_Solid._datas[i]._data = "测试数据：" + i;
                    Color col_card = XGUI_Utilitys.SetSaturation(XGUI_Utilitys.RandomRGB(), 0.7f);

                    BaseScript.CardDataWrap_Solid._datas[i]._bgcolor = col_card;
                    //BaseScript.CardDataWrap_Solid._datas[i]._bgcolor = Color.white;
                }
            }
            #endregion

            #region Solid Manual
            BaseScript.CardDataWrap_Solid_Manual = new CardDataWrap();
            if (BaseScript.CardDataWrap_Solid_Manual._datas == null)
            {
                BaseScript.CardDataWrap_Solid_Manual._datas = new CardData[10];

                for (int i = 0; i < BaseScript.CardDataWrap_Solid_Manual._datas.Length; i++)
                {
                    BaseScript.CardDataWrap_Solid_Manual._datas[i] = new CardData();
                    BaseScript.CardDataWrap_Solid_Manual._datas[i]._title = "测试标题：" + i;
                    BaseScript.CardDataWrap_Solid_Manual._datas[i]._data = "测试数据：" + i;
                    Color col_card = XGUI_Utilitys.SetSaturation(XGUI_Utilitys.RandomRGB(), 0.7f);

                    BaseScript.CardDataWrap_Solid_Manual._datas[i]._bgcolor = col_card;
                    //BaseScript.CardDataWrap_Solid._datas[i]._bgcolor = Color.white;
                }
            }
            #endregion
            #endregion
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            float w_width = XGUI.GetCurrentWindowWidth();

            #region 标题
            XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.无,
                bg_color: XGUIColor.无,
                bg_color_gui: BaseScript.ThemeColor_Group,
                title: "",
                title_size: XGUIFontSize.M,
                title_text_color: Color.white,
                title_clipping: TextClipping.Overflow,
                padding: new RectOffset(0, 0, 0, 20));

            XGUI.layout_label(
                text: "XGUI",
                size: XGUIFontSize.XXXL,
                text_color: Color.white,
                padding: new RectOffset(0, 0, 0, -10),
                margin: new RectOffset(0, 0, 0, 35),
                clipping: TextClipping.Clip,
                font: XGUI.GetFont("xg-bold"),
                font_style: FontStyle.Bold,
                anchor: TextAnchor.MiddleLeft);

            XGUI.layout_label(
                text: $"<b>XGUI</b>  是一款为  <b><color={hexcolor_primary}>Unity Editor</color></b>  量身打造的专业级界面可视化组件工具，由  <b><color={hexcolor_primary}>南京塞维斯传媒有限公司</color></b> （SevenStrikeMedia）开发。该工具提供了丰富、美观、易用的  <b><color={hexcolor_primary}>Editor GUI</color></b>  控件库，帮助开发者快速构建风格统一、交互友好的编辑器扩展界面",
                size: XGUIFontSize.B,
                text_color: Color.white * 0.85f,
                margin: new RectOffset(0, 0, 0, 0),
                clipping: TextClipping.Overflow,
                wrap: false,
                font_style: FontStyle.Normal,
                anchor: TextAnchor.UpperLeft);

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 标题下拉菜单
            Rect rect_title_popup = new Rect(150, 23, w_width - 20, XGUI.GetSingleLineHeight());

            XGUI.gui_string_popup(
                rect: rect_title_popup,
                title: "类型",
                title_color: Color.white,
                title_size: XGUIFontSize.M,
                title_font_style: FontStyle.Normal,
                title_padding: new RectOffset(0, 0, 0, 0),
                title_width: 40,
                prop: Types,
                options: new string[] { "标签", "按钮", "输入框", "文本框", "开关", "滑动条" },
                opt_text_size: XGUIFontSize.M,
                opt_text_color: Color.black,
                opt_text_padding: new RectOffset(10, 10, 0, 0),
                opt_anchor: TextAnchor.MiddleCenter,
                opt_font_style: FontStyle.Normal,
                opt_bg_fill: XGUIFilled.实体,
                opt_bg_color: XGUIColor.亮白,
                opt_bg_color_gui: BaseScript.ThemeColor_Primary,
                icon_arrow_color: Color.black);
            #endregion

            #region 组件标题
            XGUI.layout_banner(
              bg_fill: XGUIFilled.实体,
              bg_color: XGUIColor.深空灰,
              bg_height: 30,
              icon: XGUI.GetBasedIcon("icon_recycle_r"),
              icon_color: BaseScript.ThemeColor_Primary,
              title_text: "XGUI  -  可视化组件",
              title_anchor: TextAnchor.MiddleLeft,
              title_style: FontStyle.Normal,
              title_color: Color.white,
              title_size: XGUIFontSize.B,
              title_clipping: TextClipping.Ellipsis,
              bg_margin: new RectOffset(0, 0, 5, 5));

            XGUI.layout_banner(
                bg_fill: XGUIFilled.实体,
                bg_color: XGUIColor.警示黄,
                bg_height: 30,
                icon: XGUI.GetBasedIcon("icon_rewind_r"),
                icon_color: Color.black,
                title_text: "自定义组件标题",
                title_anchor: TextAnchor.MiddleRight,
                title_style: FontStyle.Normal,
                title_color: Color.black,
                title_size: XGUIFontSize.B,
                title_clipping: TextClipping.Ellipsis,
                bg_margin: new RectOffset(0, 0, 5, 5));

            XGUI.layout_banner(
                bg_fill: XGUIFilled.实体,
                bg_color: XGUIColor.深空灰,
                bg_height: 30,
                //icon: XGUI.GetBasedIcon("icon_recycle_r"),
                //icon_color: BaseScript.ThemeColor_Primary,
                title_text: "居中组件标题",
                title_anchor: TextAnchor.MiddleCenter,
                title_style: FontStyle.Normal,
                title_color: Color.white,
                title_size: XGUIFontSize.B,
                title_clipping: TextClipping.Ellipsis,
                bg_margin: new RectOffset(0, 0, 5, 5));
            #endregion

            #region 手动布局
            Rect rect_last = XGUI.GetLastRect();
            XGUI.gui_group_start(
               rect_group: new Rect(rect_last.x, rect_last.y + 55, rect_last.width, 80),
               bg_fill: XGUIFilled.缺口纯色边框,
               bg_color: XGUIColor.亮白,
               bg_color_gui: BaseScript.ThemeColor_Group,
               bg_height: 0,
               false,
               false,
               margin: new RectOffset(0, 0, 0, 0),
               padding: new RectOffset(0, 0, 0, 0),
               title: "手动编组布局",
               title_bg_fill: XGUIFilled.实体,
               title_bg_color: XGUIColor.亮白,
               title_bg_color_gui: Color.black,
               title_size: XGUIFontSize.M,
               title_anchor: TextAnchor.MiddleLeft,
               title_text_color: BaseScript.ThemeColor_Primary,
               title_offset: new Vector2(0, 0),
               title_padding: new RectOffset(0, 0, 2, 2),
               title_font: null,
               title_font_style: FontStyle.Normal,
               title_clipping: TextClipping.Clip,
               icon: XGUI.GetBasedIcon("icon_warning"),
               //icon: null,
               icon_color: Color.white,
               icon_padding: new RectOffset(0, 0, 10, 10),
               foldout: false);

            #region 分段点
            int seg = 5;
            float divide = (float)(XGUI.GetCurrentWindowWidth() / seg);
            for (int i = 0; i < seg; i++)
            {
                if (i == seg)
                    continue;
                XGUI.gui_box(
                    rect: new Rect(divide * i + 10, 37, 5, 5),
                    bg_color: BaseScript.ThemeColor_Secondary);
            }
            #endregion

            XGUI.gui_group_end();
            XGUI.layout_space(100);
            #endregion

            #region 按钮容器
            XGUI.layout_group_start(
                type: XGUIContainerType.Horizontal,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: BaseScript.ThemeColor_Group,
                title: "按钮容器",
                title_size: XGUIFontSize.M,
                title_text_color: Color.white,
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(15, 15, 20, 15));

            #region 播放 & 停止
            if (BaseScript.playing)
            {
                if (XGUI.layout_button(
                    tooltip: "停止",
                    tex_release: XGUI.GetBasedIcon("icon_stop_r"),
                    tex_press: XGUI.GetBasedIcon("icon_stop_p"),
                    tex_gui_color: Color.white,
                    width: 15,
                    height: 15))
                {
                    BaseScript.playing = false;
                }
            }
            else
            {
                if (XGUI.layout_button(
                    tooltip: "播放",
                    tex_release: XGUI.GetBasedIcon("icon_play_r"),
                    tex_press: XGUI.GetBasedIcon("icon_play_p"),
                    tex_gui_color: Color.white,
                    width: 15,
                    height: 15))
                {
                    BaseScript.playing = true;
                }
            }
            #endregion

            GUILayout.FlexibleSpace();

            #region 回收
            if (XGUI.layout_button(
                tooltip: "回收",
                tex_release: XGUI.GetBasedIcon("icon_recycle_r"),
                tex_press: XGUI.GetBasedIcon("icon_recycle_p"),
                tex_gui_color: BaseScript.ThemeColor_Primary,
                width: 15,
                height: 15))
            { }
            #endregion

            GUILayout.FlexibleSpace();

            #region 3项弹窗
            if (XGUI.layout_button(
                tooltip: "打开弹窗",
                tex_release: XGUI.GetBasedIcon("icon_dialog_r"),
                tex_press: XGUI.GetBasedIcon("icon_dialog_p"),
                tex_gui_color: BaseScript.ThemeColor_Primary,
                width: 15,
                height: 15))
            {
                string res = XGUI.dialog(
                    type: XGUIDialogType.警告,
                    windowtitle: "示例弹窗对话框",
                    title: "这是一个 3 项弹窗",
                    msg: "这是一个模态弹窗，模态弹窗弹出后，用户必须关闭它才能继续操作主窗口。它的作用就是强制用户关注并立即做出决定",
                    ok: "确认 ",
                    cancel: "取消",
                    alt: "辅助",
                    PrimaryIndex: 1,
                    usemodal: true,
                    themecolor: BaseScript.ThemeColor_Primary);

                Debug.Log(res);
            }
            #endregion

            GUILayout.FlexibleSpace();

            #region 列表弹窗
            if (XGUI.layout_button(
                tooltip: "打开弹窗",
                tex_release: XGUI.GetBasedIcon("icon_dialog_r"),
                tex_press: XGUI.GetBasedIcon("icon_dialog_p"),
                tex_gui_color: Color.white,
                width: 15,
                height: 15))
            {
                XGUIDialogListDatas[] dats = new XGUIDialogListDatas[50];
                for (int i = 0; i < dats.Length; i++)
                {
                    dats[i] = new XGUIDialogListDatas();
                    dats[i].Title = "title index" + i;
                    dats[i].SubTitle = "sub index" + i;
                    dats[i].Message = "msg index" + i;
                    dats[i].SourceObject = BaseScript;
                }

                string res = XGUI.dialog_listview(
                    datas: dats,
                    type: XGUIDialogType.警告,
                    windowtitle: "示例弹窗对话框",
                    title: "这是一个列表项弹窗",
                    msg: "这是一个模态列表项弹窗",
                    ok: "明白",
                    cancel: "检查",
                    PrimaryIndex: 0,
                    usemodal: true,
                    themecolor: BaseScript.ThemeColor_Primary,
                    on_selected: (res) =>
                    {
                        Debug.Log(res);
                    }
                    );

                //Debug.Log(res);
            }
            #endregion

            GUILayout.FlexibleSpace();

            #region 参数提交弹窗
            if (XGUI.layout_button(
                tooltip: "提交参数",
                tex_release: XGUI.GetBasedIcon("icon_dialog_r"),
                tex_press: XGUI.GetBasedIcon("icon_dialog_p"),
                tex_gui_color: BaseScript.ThemeColor_Secondary,
                width: 15,
                height: 15))
            {
                dialog_listdata res = XGUI.dialog_submit(
                    type: XGUIDialogType.修改,
                    windowtitle: "参数提交弹窗对话框",
                    title: "这是一个参数提交弹窗",
                    msg: "是否确认要将当前的参数提交 & 保存到数据库中？",
                    ok: "保存",
                    cancel: "暂不",
                    arg_name_text: "预设名称",
                    arg_data_text: "预设说明",
                    PrimaryIndex: 1,
                    usemodal: true,
                    themecolor: BaseScript.ThemeColor_Primary);

                if (string.IsNullOrEmpty(res.name) && string.IsNullOrEmpty(res.description))
                {
                    return;
                }
                if (res.state == "暂不")
                {
                    return;
                }
                Debug.Log($"{res.name}  /  {res.description}");
            }
            #endregion
            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            #endregion

            #region 工具条（自动布局）
            BaseScript.fold_toolbar_layout = XGUI.layout_group_start(
              type: XGUIContainerType.Vertical,
              bg_fill: XGUIFilled.缺口纯色边框,
              bg_color: XGUIColor.亮白,
              bg_color_gui: BaseScript.ThemeColor_Group,
              title: "工具条（自动布局）",
              title_size: XGUIFontSize.M,
              title_text_color: BaseScript.ThemeColor_Primary,
              title_clipping: TextClipping.Clip,
              padding: new RectOffset(10, 10, 15, 15),
              foldout: BaseScript.fold_toolbar_layout);

            // 测试区域
            //XGUI.gui_box(rect_field_root, Color.green * 0.5f);

            if (BaseScript.fold_toolbar_layout)
            {
                BaseScript.toolbar_index = XGUI.layout_toolbar(
                    index: ref BaseScript.toolbar_index,
                    names: new string[] { "菜单", "配置", "系统", "导出" },
                    bg_normal: XGUIFilled.纯色边框,
                    bg_selected: XGUIFilled.实体,
                    bg_color: XGUIColor.亮白,
                    bg_gui_color: Color.black * 0.5f,
                    text_color_normal: Color.white,
                    text_color_selected: BaseScript.ThemeColor_Primary,
                    bar_height: 25,
                    text_anchor: TextAnchor.MiddleCenter,
                    text_padding: new RectOffset(10, 10, 0, 0),
                    bar_margin: new RectOffset(0, 0, 0, 0),
                    text_offset: new Vector2(0, -2),
                    text_font: XGUI.GetFont("xg-regular"),
                    text_fontstyle: FontStyle.Normal,
                    bg_width_offset: 5,
                    bg_height_offset: 2,
                    navigate_style: true,
                    navigate_style_bg: XGUIFilled.纯色边框,
                    navigate_style_bg_color: Color.black * 0.5f);
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 工具条（手动布局）
            BaseScript.fold_toolbar_gui = XGUI.layout_group_start(
              type: XGUIContainerType.Vertical,
              bg_fill: XGUIFilled.缺口纯色边框,
              bg_color: XGUIColor.亮白,
              bg_color_gui: BaseScript.ThemeColor_Group,
              title: "工具条（手动布局）",
              title_size: XGUIFontSize.M,
              title_text_color: BaseScript.ThemeColor_Primary,
              title_clipping: TextClipping.Clip,
              padding: new RectOffset(10, 10, 15, 15),
              foldout: BaseScript.fold_toolbar_gui);

            // 测试区域
            //XGUI.gui_box(rect_field_root, Color.green * 0.5f);

            if (BaseScript.fold_toolbar_gui)
            {
                Rect rect_bar = XGUI.GetControlRect(false, 20);
                rect_bar.Set(rect_bar.x, rect_bar.y, rect_bar.width, rect_bar.height);

                //XGUI.gui_box(rect_bar, Color.red * 0.5f);

                BaseScript.toolbar_manual_index = XGUI.gui_toolbar(
                    rect: rect_bar,
                    index: ref BaseScript.toolbar_manual_index,
                    names: new string[] { "菜单", "配置", "系统" },
                    bg_normal: XGUIFilled.纯色边框,
                    bg_selected: XGUIFilled.实体,
                    bg_color: XGUIColor.亮白,
                    bg_gui_color: Color.black * 0.5f,
                    text_color_normal: Color.white,
                    text_color_selected: BaseScript.ThemeColor_Primary,
                    bar_height: 25,
                    text_anchor: TextAnchor.MiddleCenter,
                    text_padding: new RectOffset(10, 10, 0, 0),
                    bar_margin: new RectOffset(0, 0, 0, 0),
                    text_offset: new Vector2(0, -2),
                    text_font: XGUI.GetFont("xg-regular"),
                    text_fontstyle: FontStyle.Normal,
                    navigate_style: false,
                    navigate_style_bg: XGUIFilled.纯色边框,
                    navigate_style_bg_color: Color.black * 0.5f);
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 路径选择（自动布局）
            BaseScript.fold_pathselector_layout = XGUI.layout_group_start(
              type: XGUIContainerType.Vertical,
              bg_fill: XGUIFilled.缺口纯色边框,
              bg_color: XGUIColor.亮白,
              bg_color_gui: BaseScript.ThemeColor_Group,
              title: "路径选择（自动布局）",
              title_size: XGUIFontSize.M,
              title_text_color: BaseScript.ThemeColor_Primary,
              title_clipping: TextClipping.Clip,
              padding: new RectOffset(10, 10, 15, 15),
              foldout: BaseScript.fold_pathselector_layout);

            // 测试区域
            //XGUI.gui_box(rect_field_root, Color.green * 0.5f);

            if (BaseScript.fold_pathselector_layout)
            {
                // 保存文件选择
                XGUI.layout_path_selector(
                path_type: XGUIPathType.保存文件,
                prop: ref BaseScript._configPath,
                title: "导出路径",
                //title_color: Color.white,
                title_width: 100,
                text_wrap: false,
                field_fontsize: XGUIFontSize.M,
                field_text_offset: new Vector2(0, 0),
                field_margin: new RectOffset(0, 0, 0, 0),
                field_padding: new RectOffset(0, 0, 0, 0),
                field_text_color: Color.white,
                field_text_style: FontStyle.Normal,
                field_text_anchor: TextAnchor.MiddleLeft,
                filter: "json",
                filter_title: "配置文件",
                btn_color: BaseScript.ThemeColor_Primary,
                btn_fontsize: XGUIFontSize.M,
                default_name: "default",
                status_icon: "icon_field_status",
                status_icon_color: XGUI_Utilitys.PathExists(BaseScript._configPath) ? Color.green : Color.red,
                on_path_changed: (val) =>
                {
                    BaseScript._configPath = val;
                });

                XGUI.layout_space(10);

                // 文件选择
                XGUI.layout_path_selector(
                    path_type: XGUIPathType.文件,
                    prop: ref BaseScript._filePath,
                    title: "选择文件",
                    //title_color: Color.white,
                    title_width: 100,
                    text_wrap: false,
                    field_fontsize: XGUIFontSize.M,
                    field_text_offset: new Vector2(0, 0),
                    field_margin: new RectOffset(0, 0, 0, 0),
                    field_padding: new RectOffset(0, 0, 0, 0),
                    field_text_color: Color.white,
                    field_text_style: FontStyle.Normal,
                    field_text_anchor: TextAnchor.MiddleLeft,
                    filter: "jpg",
                    filter_title: "示例图片",
                    btn_color: BaseScript.ThemeColor_Primary,
                    btn_fontsize: XGUIFontSize.M,
                    default_name: null,
                    status_icon: "icon_field_status",
                    status_icon_color: XGUI_Utilitys.PathExists(BaseScript._filePath) ? Color.green : Color.red,
                    on_path_changed: (val) =>
                    {
                        BaseScript._filePath = val;
                    });

                XGUI.layout_space(10);

                // 文件夹选择
                XGUI.layout_path_selector(
                    path_type: XGUIPathType.文件夹,
                    prop: ref BaseScript._outputPath,
                    title: "文件夹选择",
                    //title_color: Color.white,
                    title_width: 100,
                    text_wrap: false,
                    field_fontsize: XGUIFontSize.M,
                    field_text_offset: new Vector2(0, 0),
                    field_margin: new RectOffset(0, 0, 0, 0),
                    field_padding: new RectOffset(0, 0, 0, 0),
                    field_text_color: Color.white,
                    field_text_style: FontStyle.Normal,
                    field_text_anchor: TextAnchor.MiddleLeft,
                    filter: null,
                    filter_title: "选择文件夹",
                    btn_color: BaseScript.ThemeColor_Primary,
                    btn_fontsize: XGUIFontSize.M,
                    default_name: null,
                    status_icon: "icon_field_status",
                    status_icon_color: XGUI_Utilitys.PathExists(BaseScript._outputPath) ? Color.green : Color.red,
                    on_path_changed: (val) =>
                    {
                        BaseScript._outputPath = val;
                    });

            }

            XGUI.layout_group_end(XGUIContainerType.Vertical);


            #endregion

            #region 路径选择（手动布局）
            BaseScript.fold_pathselector_manual = XGUI.layout_group_start(
              type: XGUIContainerType.Vertical,
              bg_fill: XGUIFilled.缺口纯色边框,
              bg_color: XGUIColor.亮白,
              bg_color_gui: BaseScript.ThemeColor_Group,
              title: "路径选择（手动布局）",
              title_size: XGUIFontSize.M,
              title_text_color: BaseScript.ThemeColor_Primary,
              title_clipping: TextClipping.Clip,
              padding: new RectOffset(10, 10, 15, 15),
              foldout: BaseScript.fold_pathselector_manual);

            // 测试区域
            //XGUI.gui_box(rect_field_root, Color.green * 0.5f);

            if (BaseScript.fold_pathselector_manual)
            {
                Rect rect = XGUI.GetControlRect(false, 55);

                //测试显示
                //XGUI.gui_box(rect, Color.red);

                float margin = 28;
                Rect rect_field = new Rect(rect.x + 5, rect.y, rect.width - 10, XGUI.GetSingleLineHeight());

                // 保存文件选择
                XGUI.gui_path_selector(
                    rect: rect_field,
                    path_type: XGUIPathType.保存文件,
                    prop: ref BaseScript._filePath_manual,
                    title: "导出路径",
                    title_color: Color.white,
                    title_width: 60,
                    text_wrap: false,
                    field_fontsize: XGUIFontSize.M,
                    field_text_offset: new Vector2(0, 0),
                    field_margin: new RectOffset(0, 0, 0, 0),
                    field_padding: new RectOffset(0, 0, 0, 0),
                    field_text_color: Color.white,
                    field_text_style: FontStyle.Normal,
                    field_text_anchor: TextAnchor.MiddleLeft,
                    filter: "json",
                    filter_title: "配置文件",
                    btn_color: BaseScript.ThemeColor_Primary,
                    btn_fontsize: XGUIFontSize.M,
                    default_name: "default",
                    status_icon: "icon_field_status",
                    status_icon_color: XGUI_Utilitys.PathExists(BaseScript._filePath_manual) ? Color.yellow : Color.red,
                    on_path_changed: (val) =>
                    {
                        BaseScript._filePath_manual = val;
                        Repaint();
                    });

                rect_field.y += margin;

                // 文件选择
                XGUI.gui_path_selector(
                    rect: rect_field,
                    path_type: XGUIPathType.文件,
                    prop: ref BaseScript._configPath_manual,
                    title: "选择文件",
                    //title_color: Color.white,
                    title_width: 60,
                    text_wrap: false,
                    field_fontsize: XGUIFontSize.M,
                    field_text_offset: new Vector2(0, 0),
                    field_margin: new RectOffset(0, 0, 0, 0),
                    field_padding: new RectOffset(0, 0, 0, 0),
                    field_text_color: Color.white,
                    field_text_style: FontStyle.Normal,
                    field_text_anchor: TextAnchor.MiddleLeft,
                    filter: "jpg",
                    filter_title: "示例图片",
                    btn_color: BaseScript.ThemeColor_Primary,
                    btn_fontsize: XGUIFontSize.M,
                    default_name: null,
                    status_icon: "icon_field_status",
                    status_icon_color: XGUI_Utilitys.PathExists(BaseScript._configPath_manual) ? Color.yellow : Color.red,
                    on_path_changed: (val) =>
                    {
                        BaseScript._configPath_manual = val;
                        Repaint();
                    });
            }

            XGUI.layout_group_end(XGUIContainerType.Vertical);


            #endregion

            #region 状态
            BaseScript.fold_tweenstate_gui = XGUI.layout_group_start(
                 type: XGUIContainerType.Horizontal,
                 bg_fill: XGUIFilled.缺口纯色边框,
                 bg_color: XGUIColor.亮白,
                 bg_color_gui: BaseScript.ThemeColor_Group,
                 title: "状态",
                 title_size: XGUIFontSize.M,
                 title_text_color: BaseScript.ThemeColor_Primary,
                 title_clipping: TextClipping.Clip,
                 margin: new RectOffset(0, 0, 0, 0),
                 padding: new RectOffset(0, 0, 0, 25),
                 foldout: BaseScript.fold_tweenstate_gui);

            if (BaseScript.fold_tweenstate_gui)
            {
                XGUI.layout_space(0);
                string hexcol = XGUI_Utilitys.Color_To_HexString(BaseScript.ThemeColor_Primary, true);
                string hexcol_gray = XGUI_Utilitys.Color_To_HexString(Color.white * 0.8f, true);

                Rect rect_last_liquid = XGUI.GetLastRect();
                Rect rect_ease = rect_last_liquid;

                #region Ease模式下拉菜单

                rect_ease.Set(140, rect_last_liquid.y + 105, w_width - 18, XGUI.GetSingleLineHeight());

                XGUI.gui_string_popup(
                    rect: rect_ease,
                    title: "类型",
                    title_color: Color.white,
                    title_size: XGUIFontSize.M,
                    title_font_style: FontStyle.Normal,
                    title_padding: new RectOffset(0, 0, 0, 0),
                    title_width: 40,
                    prop: EaseMode,
                    options: EaseNames,
                    opt_text_size: XGUIFontSize.M,
                    opt_text_color: Color.black,
                    opt_text_padding: new RectOffset(10, 10, 0, 0),
                    opt_anchor: TextAnchor.MiddleCenter,
                    opt_font_style: FontStyle.Normal,
                    opt_bg_fill: XGUIFilled.实体,
                    opt_bg_color: XGUIColor.亮白,
                    opt_bg_color_gui: BaseScript.ThemeColor_Primary,
                    icon_arrow_color: Color.black);
                #endregion

                #region EaseGraph图形
                if (w_width > 230)
                {
                    rect_ease.Set(w_width - 120, rect_last_liquid.y + 141, 100, 65);
                    XGUI.gui_icon(
                        rect: rect_ease,
                        icon: XGUI.GetBasedIcon("EaseGraph/bg"),
                        color: Color.white * 0.8f);

                    rect_ease.Set(w_width - 120, rect_last_liquid.y + 141, 100, 65);
                    XGUI.gui_icon(
                        rect: rect_ease,
                        icon: XGUI.GetBasedIcon($"EaseGraph/{EaseMode.stringValue}"),
                        color: BaseScript.ThemeColor_Primary);
                }
                #endregion

                #region 文字内容
                XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                                bg_fill: XGUIFilled.无,
                                bg_color: XGUIColor.无,
                                margin: new RectOffset(0, 0, 0, 0),
                                padding: new RectOffset(0, 0, 0, 0));

                XGUI.layout_label(
                    text: "XGUI Graphics",
                    size: XGUIFontSize.L,
                    text_color: Color.white,
                    margin: new RectOffset(15, 0, 0, 0),
                    clipping: TextClipping.Clip,
                    font_style: FontStyle.Bold,
                    anchor: TextAnchor.MiddleLeft);

                #region 状态开关
                if (w_width >= 204)
                {
                    Rect rect_toggle_last = XGUI.GetLastRect();
                    Rect rect_toggle = new Rect(rect_toggle_last.x + 150, rect_toggle_last.y, rect_toggle_last.width - 150, XGUI.GetSingleLineHeight());
                    XGUI.gui_toggle(
                        rect: rect_toggle,
                                        title: "开关",
                                        title_color: Color.white,
                                        title_size: XGUIFontSize.M,
                                        title_font_style: FontStyle.Normal,
                                        title_padding: new RectOffset(0, 0, 0, 0),
                                        title_width: 20,
                                        tog_interval: 10,
                                        prop: state_toggle,
                                        tog_style: XGUIToggleStyle.实体,
                                        tog_mixed_options: new string[] { "关闭", "开启" },
                                        tog_mixed_text_size: XGUIFontSize.M,
                                        tog_mixed_text_color: Color.black,
                                        tog_mixed_text_padding: new RectOffset(0, 0, 0, 0),
                                        tog_mixed_text_anchor: TextAnchor.MiddleCenter,
                                        tog_mixed_text_font_style: FontStyle.Normal,
                                        tog_bg_off_color: Color.gray,
                                        tog_bg_on_color: ThemeColor_Primary.colorValue,
                                        tog_handler_off_color: Color.white,
                                        tog_handler_on_color: Color.white,
                                        tog_mixed_bg_color_gui: ThemeColor_Primary.colorValue);
                }
                #endregion

                XGUI.layout_label(
                    text: $"<color={hexcol_gray}>GUID :  </color>a595258f-bdc8-47ad-b203-df0006aaa5c8",
                    size: XGUIFontSize.M,
                    text_color: Color.white * 0.8f,
                    margin: new RectOffset(15, 0, 10, 0),
                    clipping: TextClipping.Clip,
                    font_style: FontStyle.Normal,
                    anchor: TextAnchor.MiddleLeft);

                XGUI.layout_label(
                    text: $"<color={hexcol_gray}>Mark :  </color>paN5Et_o",
                    size: XGUIFontSize.M,
                    text_color: Color.white,
                    margin: new RectOffset(15, 0, 10, 0),
                    clipping: TextClipping.Clip,
                    font_style: FontStyle.Normal,
                    anchor: TextAnchor.MiddleLeft);

                XGUI.layout_label(
                    text: $"Ease:   <color={hexcol}>0.28</color>",
                    size: XGUIFontSize.M,
                    text_color: Color.white,
                    margin: new RectOffset(15, 0, 10, 0),
                    clipping: TextClipping.Clip,
                    font_style: FontStyle.Normal,
                    anchor: TextAnchor.MiddleLeft);

                XGUI.layout_seperator(thickness: 1,
                    color: Color.white * 0.5f,
                    margin: new RectOffset(15, 30, 12, 0));

                XGUI.layout_label(
                    text: $"LoopMode:   Restart  /  Looped",
                    size: XGUIFontSize.M,
                    text_color: Color.white * 0.7f,
                    margin: new RectOffset(15, w_width < 230 ? 15 : 125, 18, 0),
                    clipping: TextClipping.Clip,
                    font_style: FontStyle.Normal,
                    anchor: TextAnchor.MiddleLeft);

                XGUI.layout_label(
                    text: $"Relative:   False  /  From:   True",
                    size: XGUIFontSize.M,
                    text_color: Color.white * 0.7f,
                    margin: new RectOffset(15, w_width < 230 ? 15 : 125, 10, 0),
                    clipping: TextClipping.Clip,
                    font_style: FontStyle.Normal,
                    anchor: TextAnchor.MiddleLeft);

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                #endregion
            }

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            #endregion

            #region 列表
            BaseScript.fold_listview = XGUI.layout_group_start(
              type: XGUIContainerType.Vertical,
              bg_fill: XGUIFilled.缺口纯色边框,
              bg_color: XGUIColor.亮白,
              bg_color_gui: BaseScript.ThemeColor_Group,
              title: "列表",
              title_size: XGUIFontSize.M,
              title_text_color: Color.white,
              title_clipping: TextClipping.Clip,
              padding: new RectOffset(10, 10, 15, 15),
              foldout: BaseScript.fold_listview);

            if (BaseScript.fold_listview)
            {
                Rect list_root = XGUI.GetControlRect(false, 250);

                list_scrollPosition = XGUI.gui_scrollview<PlayerData>(
                    rect: new Rect(list_root.x, list_root.y, list_root.width, list_root.height),
                    scroll: list_scrollPosition,
                    list: BaseScript.PlayerDatas,
                    cachedItemHeights: ref list_cachedItemHeights,
                    heightsDirty: ref list_heightsDirty,
                    fixheight: 80,
                    onscroller: (rect, type, index) =>
                    {
                        Rect rect_content = rect;

                        #region 列表项
                        #region 背景
                        rect_content.Set(rect.x, rect.y, rect.width, rect.height - 1);
                        XGUI.gui_box(
                            rect: rect_content,
                            bg: XGUI.GetFillTexture(XGUIFilled.实体, XGUIColor.亮白),
                            bg_color_gui: Color.black * 0.33f,
                            offset: Vector2.zero,
                            border: new RectOffset(10, 10, 10, 10),
                            margin: new RectOffset(0, 0, 0, 0));
                        #endregion

                        #region 图标
                        rect_content.Set(rect.x + 15, rect.y + 15, list_IndexIcon.width, list_IndexIcon.height);
                        XGUI.gui_box(
                            rect: rect_content,
                            bg: list_IndexIcon,
                            bg_color_gui: Color.white * 0.8f,
                            offset: Vector2.zero,
                            margin: new RectOffset(0, 0, 0, 0));
                        #endregion

                        #region 名称
                        rect_content.Set(rect.x + 20 + list_IndexIcon.width + 5, rect.y + 8, rect.width - (10 + list_IndexIcon.width + 10), XGUI.GetSingleLineHeight());
                        XGUI.gui_label(
                            rect: rect_content,
                            text: new GUIContent($"{type._name}"),
                            text_color: Color.white,
                            size: XGUIFontSize.B,
                            anchor: TextAnchor.MiddleLeft,
                            clipping: TextClipping.Ellipsis);
                        #endregion

                        #region 所在区域
                        rect_content.Set(rect.x + 20 + list_IndexIcon.width + 5, rect.y + 28, rect.width - (10 + list_IndexIcon.width + 10), XGUI.GetSingleLineHeight());
                        XGUI.gui_label(
                            rect: rect_content,
                            text: new GUIContent($"{type._region}"),
                            text_color: BaseScript.ThemeColor_Primary,
                            size: XGUIFontSize.M,
                            anchor: TextAnchor.MiddleLeft,
                            font: XGUI.GetFont("xg-medium"),
                            clipping: TextClipping.Ellipsis);
                        #endregion

                        #region 武器
                        rect_content.Set(rect.x + 20 + list_IndexIcon.width + 5, rect.y + 52, rect.width - (10 + list_IndexIcon.width + 10), XGUI.GetSingleLineHeight());
                        XGUI.gui_label(
                            rect: rect_content,
                            text: new GUIContent($"{type._weapon}"),
                            text_color: BaseScript.ThemeColor_Secondary,
                            size: XGUIFontSize.M,
                            anchor: TextAnchor.MiddleLeft,
                            font: XGUI.GetFont("xg-bold"),
                            clipping: TextClipping.Ellipsis);
                        #endregion

                        #region Group容器
                        rect_content.Set(rect.x + (rect.width - 80), rect.y + 1, 100, rect.height - 6);
                        XGUI.gui_group_start(
                            rect_group: rect_content,
                            bg_fill: XGUIFilled.无,
                            bg_color: XGUIColor.无,
                            bg_color_gui: Color.red * 0.3f,
                            bg_height: 0,
                            false,
                            false,
                            margin: new RectOffset(0, 0, 0, 0),
                            padding: new RectOffset(0, 0, 0, 0));

                        #region 头像
                        float scale_tex = 0.45f;
                        if (XGUI.gui_button(
                            rect: new Rect(8, 10, icon_avatars[index].width * scale_tex, icon_avatars[index].height * scale_tex),
                            tooltip: $"{type._name}  |  {type._region}  |  {type._weapon}  |  {type._level}  |  {type._ammo}  |  {type._cdpercent}  |  {type._health}  |  {type._avatarpath}",
                            tex_release: icon_avatars[index],
                            tex_press: icon_avatars[index],
                            tex_gui_color: Color.white,
                            margin: new RectOffset(0, 0, 0, 0),
                            padding: new RectOffset(0, 0, 0, 0),
                            focus_name: "fav_btn"))
                        {
                            XGUI.dialog(
                                type: XGUIDialogType.通知,
                                windowtitle: "玩家信息",
                                title: "玩家信息",
                                msg: $"{type._name}  |  {type._region}  |  {type._weapon}  |  {type._level}  |  {type._ammo}  |  {type._cdpercent}  |  {type._health}  |  {type._avatarpath}",
                                ok: "明白",
                                PrimaryIndex: 0,
                                themecolor: BaseScript.ThemeColor_Primary);
                        }
                        #endregion

                        #region 等级
                        rect_content.Set(48, 52, 30, XGUI.GetSingleLineHeight());
                        XGUI.gui_label(
                            rect: rect_content,
                            text: new GUIContent($"{type._level}"),
                            text_color: Color.black,
                            bg_fill: XGUIFilled.实体,
                            bg_color: XGUIColor.亮白,
                            bg_color_gui: Color.white,
                            size: XGUIFontSize.L,
                            anchor: TextAnchor.MiddleCenter,
                            font_style: FontStyle.Bold,
                            font: XGUI.GetFont("xg-bold"),
                            padding: new RectOffset(5, 4, 2, 2),
                            offset: new Vector2(0, -2),
                            clipping: TextClipping.Clip);
                        #endregion

                        XGUI.gui_group_end();
                        #endregion
                        #endregion
                    });
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 卡片（自动布局）
            BaseScript.fold_card = XGUI.layout_group_start(
              type: XGUIContainerType.Vertical,
              bg_fill: XGUIFilled.缺口纯色边框,
              bg_color: XGUIColor.亮白,
              bg_color_gui: BaseScript.ThemeColor_Group,
              title: "卡片（自动布局）",
              title_size: XGUIFontSize.M,
              title_text_color: Color.white,
              title_clipping: TextClipping.Clip,
              padding: new RectOffset(10, 10, 15, 15),
              foldout: BaseScript.fold_card);

            if (BaseScript.fold_card)
            {
                Rect rect_card_edge = XGUI.layout_card(
                    forward: false,
                    style: XGUICardStyle.边框,
                    card_height: 90,
                    card_arrow_region_width: 80,
                    data: BaseScript.CardDataWrap_Edge,
                    color_arrow_l: Color.white,
                    color_arrow_r: Color.white,
                    on_press_left: () =>
                    {
                        if (BaseScript.CardDataWrap_Edge.index > 0)
                            BaseScript.CardDataWrap_Edge.index--;
                        else
                            BaseScript.CardDataWrap_Edge.index = 0;
                    },
                    on_press_right: () =>
                    {
                        if (BaseScript.CardDataWrap_Edge.index < BaseScript.CardDataWrap_Edge._datas.Length - 1)
                            BaseScript.CardDataWrap_Edge.index++;
                        else
                            BaseScript.CardDataWrap_Edge.index = BaseScript.CardDataWrap_Edge._datas.Length - 1;
                    });

                XGUI.layout_space(10);

                XGUI.layout_label(
                    text: "鼠标滚轮可以滚动切换（反向）",
                    size: XGUIFontSize.M,
                    text_color: Color.gray,
                    margin: new RectOffset(0, 0, 0, 0),
                    offset: new Vector2(0, 0),
                    clipping: TextClipping.Clip,
                    font_style: FontStyle.Normal,
                    anchor: TextAnchor.MiddleCenter,
                    font: XGUI.GetFont("xg-medium"));

                XGUI.layout_space(15);

                Rect rect_card_solid = XGUI.layout_card(
                    forward: true,
                   style: XGUICardStyle.实体,
                   card_height: 150,
                   card_arrow_region_width: 80,
                   data: BaseScript.CardDataWrap_Solid,
                   //color_arrow_l: Color.black,
                   //color_arrow_r: Color.black,
                   on_press_left: () =>
                   {
                       if (BaseScript.CardDataWrap_Solid.index > 0)
                           BaseScript.CardDataWrap_Solid.index--;
                       else
                           BaseScript.CardDataWrap_Solid.index = 0;
                   },
                   on_press_right: () =>
                   {
                       if (BaseScript.CardDataWrap_Solid.index < BaseScript.CardDataWrap_Solid._datas.Length - 1)
                           BaseScript.CardDataWrap_Solid.index++;
                       else
                           BaseScript.CardDataWrap_Solid.index = BaseScript.CardDataWrap_Solid._datas.Length - 1;
                   });

                XGUI.layout_space(10);

                XGUI.layout_label(
                    text: "鼠标滚轮可以滚动切换 （正向）",
                    size: XGUIFontSize.M,
                    text_color: Color.gray,
                    margin: new RectOffset(0, 0, 0, 0),
                    offset: new Vector2(0, 0),
                    clipping: TextClipping.Clip,
                    font_style: FontStyle.Normal,
                    anchor: TextAnchor.MiddleCenter,
                    font: XGUI.GetFont("xg-medium"));

                XGUI.layout_space(15);

                Repaint();
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 卡片（手动布局）
            BaseScript.fold_card_manual = XGUI.layout_group_start(
              type: XGUIContainerType.Vertical,
              bg_fill: XGUIFilled.缺口纯色边框,
              bg_color: XGUIColor.亮白,
              bg_color_gui: BaseScript.ThemeColor_Group,
              title: "卡片（手动布局）",
              title_size: XGUIFontSize.M,
              title_text_color: Color.white,
              title_clipping: TextClipping.Clip,
              padding: new RectOffset(10, 10, 15, 15),
              foldout: BaseScript.fold_card_manual);

            if (BaseScript.fold_card_manual)
            {
                Rect rect = XGUI.GetControlRect(false, 100);

                Rect rect_card_manual = XGUI.gui_card(
                 rect: new Rect(rect.x, rect.y, rect.width, rect.height),
                 forward: false,
                 style: XGUICardStyle.实体,
                 card_arrow_region_width: 80,
                 data: BaseScript.CardDataWrap_Solid_Manual,
                 color_arrow_l: Color.white,
                 color_arrow_r: Color.white,
                 on_press_left: () =>
                 {
                     if (BaseScript.CardDataWrap_Solid_Manual.index > 0)
                         BaseScript.CardDataWrap_Solid_Manual.index--;
                     else
                         BaseScript.CardDataWrap_Solid_Manual.index = 0;
                 },
                 on_press_right: () =>
                 {
                     if (BaseScript.CardDataWrap_Solid_Manual.index < BaseScript.CardDataWrap_Solid_Manual._datas.Length - 1)
                         BaseScript.CardDataWrap_Solid_Manual.index++;
                     else
                         BaseScript.CardDataWrap_Solid_Manual.index = BaseScript.CardDataWrap_Solid_Manual._datas.Length - 1;
                 });

                XGUI.layout_space(15);

                XGUI.layout_label(
                    text: "鼠标滚轮可以滚动切换 （反向）",
                    size: XGUIFontSize.M,
                    text_color: Color.gray,
                    margin: new RectOffset(0, 0, 0, 0),
                    offset: new Vector2(0, 0),
                    clipping: TextClipping.Clip,
                    font_style: FontStyle.Normal,
                    anchor: TextAnchor.MiddleCenter,
                    font: XGUI.GetFont("xg-medium"));

                XGUI.layout_space(15);
            }


            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 下拉选择菜单
            BaseScript.fold_pops_layout = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: BaseScript.ThemeColor_Group,
                title: "下拉选择菜单",
                title_size: XGUIFontSize.M,
                title_text_color: Color.white,
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: BaseScript.fold_pops_layout);

            if (BaseScript.fold_pops_layout)
            {
                XGUI.layout_string_popup(
                    title: "选择硬件",
                    title_width: 60,
                    title_color: BaseScript.ThemeColor_Secondary,
                    title_size: XGUIFontSize.M,
                    title_anchor: TextAnchor.MiddleLeft,
                    prop: Hardware,
                    options: new string[] { "CPU芯片", "硬盘", "极限配置主机", "高端显卡", "分布式主板", "轨迹球鼠标", "键盘", "弧面显示器" },
                    opt_text_size: XGUIFontSize.M,
                    opt_text_color: Color.black,
                    opt_text_padding: new RectOffset(10, 10, 0, 0),
                    opt_anchor: TextAnchor.MiddleLeft,
                    opt_font_style: FontStyle.Normal,
                    opt_bg_fill: XGUIFilled.实体,
                    opt_bg_color: XGUIColor.亮白,
                    opt_bg_color_gui: BaseScript.ThemeColor_Primary,
                    margin: new RectOffset(0, 0, 5, 5),
                    padding: new RectOffset(5, 5, 0, 0),
                    title_margin: new RectOffset(0, 0, 0, 0),
                    icon_arrow_color: Color.black);

                float scale = 0.7f;

                XGUI.layout_icon(
                    icon: GetTexture(icons_hardware, Hardware.stringValue),
                    width: 256 * scale,
                    height: 96 * scale,
                    icon_alignment: XGUIIconAlignment.中心,
                    layout_margin: new RectOffset(0, 0, 15, 15));

                XGUI.layout_string_popup(
                    title: "选择城市",
                    title_width: 60,
                    title_color: BaseScript.ThemeColor_Secondary,
                    title_size: XGUIFontSize.M,
                    title_anchor: TextAnchor.MiddleLeft,
                    prop: Stones,
                    options: new string[] { "Agate", "Azurite", "Calcite", "Fluorite", "Malachite", "Pyrite", "RutilatedQuartz", "WhiteCrystalCluster" },
                    opt_text_size: XGUIFontSize.M,
                    opt_text_color: Color.black,
                    opt_text_padding: new RectOffset(10, 10, 0, 0),
                    opt_anchor: TextAnchor.MiddleLeft,
                    opt_font_style: FontStyle.Normal,
                    opt_bg_fill: XGUIFilled.实体,
                    opt_bg_color: XGUIColor.亮白,
                    opt_bg_color_gui: BaseScript.ThemeColor_Primary,
                    margin: new RectOffset(0, 0, 5, 5),
                    padding: new RectOffset(5, 5, 0, 0),
                    title_margin: new RectOffset(0, 0, 0, 0),
                    icon_arrow_color: Color.black);

                XGUI.layout_icon(
                    icon: GetTexture(icons_stone, Stones.stringValue),
                    width: 256 * scale,
                    height: 96 * scale,
                    icon_alignment: XGUIIconAlignment.左,
                    layout_margin: new RectOffset(0, 0, 15, 15));
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 可折叠参数 - 自动布局
            BaseScript.fold_param_layout = XGUI.layout_group_start(
                  type: XGUIContainerType.Vertical,
                  bg_fill: XGUIFilled.缺口纯色边框,
                  bg_color: XGUIColor.亮白,
                  bg_color_gui: BaseScript.ThemeColor_Group,
                  title: "可折叠参数 - 自动布局",
                  title_size: XGUIFontSize.M,
                  title_text_color: BaseScript.ThemeColor_Primary,
                  title_clipping: TextClipping.Clip,
                  padding: new RectOffset(10, 10, 15, 15),
                  foldout: BaseScript.fold_param_layout);

            if (BaseScript.fold_param_layout)
            {
                DrawParamField("主题色 - 组", ThemeColor_Group, 100, Color.clear);
                DrawParamField("主题色 - 主要", ThemeColor_Primary, 100, Color.clear);
                DrawParamField("主题色 - 次要", ThemeColor_Secondary, 100, Color.clear);
                DrawParamField("字符串", param_string, 100, colors[0]);
                DrawParamField("浮点数", param_float, 100, colors[1]);
                DrawParamField("整数", param_int, 100, colors[2]);
                DrawParamField("颜色", param_color, 100, colors[3]);
                DrawParamField("Vector2", param_vector2, 100, colors[4]);
                DrawParamField("Vector3", param_vector3, 100, colors[5]);
                DrawParamField("Vector4", param_vector4, 100, colors[6]);
                DrawParamField("RectOffset", param_rectoffset, 100, colors[7]);
                DrawParamField("GameObject", param_gameobject, 100, colors[8]);
                DrawParamField("Curve", param_curve, 100, colors[9]);
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 可折叠参数 - 手动布局
            BaseScript.fold_param_gui = XGUI.layout_group_start(
                  type: XGUIContainerType.Vertical,
                  bg_fill: XGUIFilled.缺口纯色边框,
                  bg_color: XGUIColor.亮白,
                  bg_color_gui: BaseScript.ThemeColor_Group,
                  title: "可折叠参数 - 手动布局",
                  title_size: XGUIFontSize.M,
                  title_text_color: BaseScript.ThemeColor_Primary,
                  title_clipping: TextClipping.Clip,
                  padding: new RectOffset(10, 10, 15, 15),
                  foldout: BaseScript.fold_param_gui);

            if (BaseScript.fold_param_gui)
            {
                Rect rect_pam = XGUI.GetControlRect(false, 530);
                float margin = 28;
                DrawParamField(new Rect(rect_pam.x + 5, rect_pam.y, rect_pam.width - 10, 20), new GUIContent("主题色 - 组"), ThemeColor_Group, 100);
                DrawParamField(new Rect(rect_pam.x + 5, rect_pam.y + margin, rect_pam.width - 10, 20), new GUIContent("主题色 - 主要"), ThemeColor_Primary, 100);
                DrawParamField(new Rect(rect_pam.x + 5, rect_pam.y + margin * 2, rect_pam.width - 10, 20), new GUIContent("主题色 - 次要"), ThemeColor_Secondary, 100);
                DrawParamField(new Rect(rect_pam.x + 5, rect_pam.y + margin * 3, rect_pam.width - 10, 20), new GUIContent("字符串"), param_string, 100);
                DrawParamField(new Rect(rect_pam.x + 5, rect_pam.y + margin * 4, rect_pam.width - 10, 20), new GUIContent("浮点数"), param_float, 100);
                DrawParamField(new Rect(rect_pam.x + 5, rect_pam.y + margin * 5, rect_pam.width - 10, 20), new GUIContent("整数"), param_int, 100);
                DrawParamField(new Rect(rect_pam.x + 5, rect_pam.y + margin * 6, rect_pam.width - 10, 20), new GUIContent("颜色"), param_color, 100);
                DrawParamField(new Rect(rect_pam.x + 5, rect_pam.y + margin * 7, rect_pam.width - 10, 20), new GUIContent("Vector2"), param_vector2, 100);
                DrawParamField(new Rect(rect_pam.x + 5, rect_pam.y + margin * 8, rect_pam.width - 10, 20), new GUIContent("Vector3"), param_vector3, 100);
                DrawParamField(new Rect(rect_pam.x + 5, rect_pam.y + margin * 9, rect_pam.width - 10, 20), new GUIContent("Vector4"), param_vector4, 100);
                DrawParamField(new Rect(rect_pam.x + 5, rect_pam.y + margin * 13, rect_pam.width - 10, 20), new GUIContent("RectOffset"), param_rectoffset, 100);
                DrawParamField(new Rect(rect_pam.x + 5, rect_pam.y + margin * 18, rect_pam.width - 10, 20), new GUIContent("GameObject"), param_gameobject, 100);
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 状态控件 - 自动布局
            BaseScript.fold_state_layout = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: BaseScript.ThemeColor_Group,
                title: "状态控件 - 自动布局",
                title_size: XGUIFontSize.M,
                title_text_color: BaseScript.ThemeColor_Primary,
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: BaseScript.fold_state_layout);

            if (BaseScript.fold_state_layout)
            {
                XGUI.layout_state_displayer_text(
                      title: "世界状态",
                      title_size: XGUIFontSize.M,
                      subtitle: "启用",
                      subtitle_size: XGUIFontSize.M,
                      subtitle_color: Color.white * 0.7f,
                      margin: new RectOffset(5, 5, 0, 5));

                XGUI.layout_state_displayer_icon(
                      title: "数值状态",
                      title_size: XGUIFontSize.M,
                      icon: XGUI.GetBasedIcon("icon_recycle_r"),
                      icon_size: new Vector2(12, 12),
                      icon_offset: new Vector2(0, 0),
                      icon_color: BaseScript.ThemeColor_Secondary,
                      padding: new RectOffset(0, 0, 0, 0),
                      margin: new RectOffset(5, 5, 0, 5));

                XGUI.layout_state_displayer_btn(
                    title: "检测状态",
                    title_size: XGUIFontSize.M,
                    btn_bg: XGUIFilled.实体,
                    btn_color: XGUIColor.亮白,
                    btn_text: "clicked",
                    btn_tooltip: "tooltip",
                    btn_text_size: XGUIFontSize.M,
                    btn_layout_width: 120,
                    btn_gui_color: Color.white,
                    padding: new RectOffset(0, 0, 0, 0),
                    callback_clicked: () =>
                    {
                        Debug.Log("检测完成！");
                    });
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 状态控件 - 手动布局
            BaseScript.fold_state_gui = XGUI.layout_group_start(
               type: XGUIContainerType.Vertical,
               bg_fill: XGUIFilled.缺口纯色边框,
               bg_color: XGUIColor.亮白,
               bg_color_gui: BaseScript.ThemeColor_Group,
               title: "状态控件 - 手动布局（无折叠）",
               title_size: XGUIFontSize.M,
               title_text_color: Color.green,
               title_clipping: TextClipping.Clip,
               padding: new RectOffset(10, 10, 15, 15),
               foldout: BaseScript.fold_state_gui);

            if (BaseScript.fold_state_gui)
            {
                float height_offset = 28;
                Rect rect_root = XGUI.GetControlRect(false, 86);

                // 测试区域
                //XGUI.gui_box(new Rect(rect_root.x, rect_root.y, rect_root.width, rect_root.height), Color.green);

                Rect rect_prop = new Rect(rect_root.x + 5, rect_root.y, rect_root.width - 5, 25);

                XGUI.gui_state_displayer_text(
                    rect: rect_prop,
                    title: new GUIContent("耗时："),
                    title_size: XGUIFontSize.M,
                    subtitle: new GUIContent("9999.999 s"),
                    subtitle_size: XGUIFontSize.M,
                    subtitle_color: Color.white,
                    margin: new RectOffset(5, 5, 0, 5));

                rect_prop.y += height_offset;

                XGUI.gui_state_displayer_icon(
                    rect: rect_prop,
                    title: new GUIContent("运行效率："),
                    title_size: XGUIFontSize.M,
                    icon: XGUI.GetBasedIcon("icon_recycle_r"),
                    icon_size: new Vector2(15, 15),
                    icon_color: Color.white,
                    icon_offset: new Vector2(0, 0),
                    padding: new RectOffset(0, 0, 0, 0),
                    margin: new RectOffset(5, 5, 0, 5));

                rect_prop.y += height_offset;

                XGUI.gui_state_displayer_btn(
                    rect: rect_prop,
                    title: new GUIContent("检测状态："),
                    title_size: XGUIFontSize.M,
                    btn_bg: XGUIFilled.实体,
                    btn_color: XGUIColor.亮白,
                    btn_width: 100,
                    btn_gui_color: Color.white,
                    btn_text: "点击检测",
                    btn_text_color: Color.black,
                    btn_text_size: XGUIFontSize.M,
                    btn_press_bg: XGUIFilled.实体,
                    btn_press_color: XGUIColor.深空灰,
                    btn_press_text_color: Color.white,
                    btn_tooltip: "点击检测状态",
                    padding: new RectOffset(0, 0, 0, 0),
                    margin: new RectOffset(5, 5, 0, 5),
                    callback_clicked: () =>
                    {
                        Debug.Log("检测状态完成！");
                    });
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 开关 - 自动布局
            BaseScript.fold_toggle_layout = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: BaseScript.ThemeColor_Group,
                title: "开关 - 自动布局",
                title_size: XGUIFontSize.M,
                title_text_color: BaseScript.ThemeColor_Primary,
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: BaseScript.fold_toggle_layout);

            if (BaseScript.fold_toggle_layout)
            {
                DrawToggle("开关 - 嵌入", state_toggle_A, 120, XGUIToggleStyle.嵌入, Color.white, Color.white, Color.white, Color.white, (b) =>
                {

                });
                DrawToggle("开关 - 实体", state_toggle_B, 120, XGUIToggleStyle.实体, BaseScript.ThemeColor_Primary, Color.white * 0.65f, Color.white, Color.white, (b) =>
                {

                });
                DrawToggle("开关 - 边框", state_toggle_C, 120, XGUIToggleStyle.边框, BaseScript.ThemeColor_Primary, Color.white * 0.65f, Color.white, Color.white, (b) =>
                {

                });
                DrawToggle("开关 - 实体", state_toggle_D, 120, XGUIToggleStyle.嵌入, Color.white, Color.white, BaseScript.ThemeColor_Primary, Color.white, (b) =>
                {

                });
                DrawToggle("开关 - 边框", state_toggle_E, 120, XGUIToggleStyle.边框, Color.white * 0.65f, Color.white * 0.65f, BaseScript.ThemeColor_Primary, Color.white, (b) =>
                {

                });

            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 滑动条
            BaseScript.fold_sliders_layout = XGUI.layout_group_start(
              type: XGUIContainerType.Vertical,
              bg_fill: XGUIFilled.缺口纯色边框,
              bg_color: XGUIColor.亮白,
              bg_color_gui: BaseScript.ThemeColor_Group,
              title: "滑动条",
              title_size: XGUIFontSize.M,
              title_text_color: BaseScript.ThemeColor_Primary,
              title_clipping: TextClipping.Clip,
              padding: new RectOffset(10, 10, 15, 15),
              foldout: BaseScript.fold_sliders_layout);

            if (BaseScript.fold_sliders_layout)
            {
                XGUI.layout_slider(
                    title: "数值平衡",
                    prop: param_slider_volume,
                    left: 0,
                    right: 100,
                    title_width: 100,
                    title_size: XGUIFontSize.M,
                    title_anchor: TextAnchor.MiddleLeft,
                    //title_color: Color.white,
                    status_icon: "icon_field_status",
                    status_icon_color: Color.green,
                    prop_margin: new RectOffset(0, 0, 5, 0));

                XGUI.layout_slider(
                    title: "频率范围",
                    prop: param_slider_hz,
                    left: 6000,
                    right: 48000,
                    title_width: 100,
                    title_size: XGUIFontSize.M,
                    title_anchor: TextAnchor.MiddleLeft,
                    //title_color: Color.white,
                    status_icon: "icon_field_status",
                    status_icon_color: Color.red,
                    prop_margin: new RectOffset(0, 0, 5, 0));

                XGUI.layout_slider(
                    title: "距离衰减",
                    prop: param_slider_distance,
                    left: 0,
                    right: 140,
                    title_width: 100,
                    title_size: XGUIFontSize.M,
                    title_anchor: TextAnchor.MiddleLeft,
                    //title_color: Color.white,
                    status_icon: "icon_field_status",
                    status_icon_color: Color.cyan,
                    prop_margin: new RectOffset(0, 0, 5, 0));

                xgui_minmax_value minmax = XGUI.layout_slider_min_max(
                  title: "距离衰减",
                  ref_min: ref BaseScript.param_slider_min,
                  ref_max: ref BaseScript.param_slider_max,
                  min_limite: 0,
                  max_limite: 140,
                  title_width: 100,
                  title_size: XGUIFontSize.M,
                  title_anchor: TextAnchor.MiddleLeft,
                  //title_color: Color.white,
                  min_field_color: Color.white,
                  max_field_color: Color.white,
                  status_icon: "icon_field_status",
                  status_icon_color: Color.yellow,
                  shrink_text_color: Color.white * 0.65f,
                  control_limite: 210,
                  prop_margin: new RectOffset(0, 0, 5, 0));
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 输入框
            BaseScript.fold_inputfield_layout = XGUI.layout_group_start(
              type: XGUIContainerType.Vertical,
              bg_fill: XGUIFilled.缺口纯色边框,
              bg_color: XGUIColor.亮白,
              bg_color_gui: BaseScript.ThemeColor_Group,
              title: "输入框",
              title_size: XGUIFontSize.M,
              title_text_color: BaseScript.ThemeColor_Primary,
              title_clipping: TextClipping.Clip,
              padding: new RectOffset(10, 10, 15, 15),
              foldout: BaseScript.fold_inputfield_layout);

            if (BaseScript.fold_inputfield_layout)
            {
                BaseScript.field_string = XGUI.layout_inputfield(
                    title: "Am",
                    prop: BaseScript.field_string,
                    text_wrap: false,
                    field_fontsize: XGUIFontSize.M,
                    field_text_offset: Vector2.zero,
                    field_height: 20,
                    field_padding: new RectOffset(0, 0, 0, 0),
                    field_margin: new RectOffset(0, 0, 5, 0),
                    field_text_color: Color.white,
                    title_width: 80,
                    field_text_font: XGUI.GetFont("xg-medium"),
                    field_text_style: FontStyle.Normal,
                    field_text_anchor: TextAnchor.MiddleLeft,
                    status_icon: "icon_field_status",
                    status_icon_color: Color.green);

                BaseScript.field_string = XGUI.layout_inputfield(
                    title: "Bk",
                    prop: BaseScript.field_string,
                    text_wrap: true,
                    field_fontsize: XGUIFontSize.M,
                    field_text_offset: Vector2.zero,
                    field_height: 80,
                    field_padding: new RectOffset(0, 0, 0, 0),
                    field_margin: new RectOffset(0, 0, 5, 0),
                    field_text_color: Color.white,
                    title_width: 80,
                    field_text_font: XGUI.GetFont("xg-medium"),
                    field_text_style: FontStyle.Normal,
                    field_text_anchor: TextAnchor.UpperLeft,
                    status_icon: "icon_field_status",
                    status_icon_color: Color.green);

                BaseScript.field_float = XGUI.layout_inputfield(
                    title: "Cz",
                    prop: BaseScript.field_float,
                    field_fontsize: XGUIFontSize.M,
                    field_text_offset: Vector2.zero,
                    field_height: 20,
                    field_padding: new RectOffset(0, 0, 0, 0),
                    field_margin: new RectOffset(0, 0, 5, 0),
                    field_text_color: Color.white,
                    title_width: 80,
                    field_text_font: XGUI.GetFont("xg-medium"),
                    field_text_style: FontStyle.Normal,
                    field_text_anchor: TextAnchor.MiddleLeft,
                    status_icon: "icon_field_status",
                    status_icon_color: Color.green);

                BaseScript.field_int = XGUI.layout_inputfield(
                    title: "Dn",
                    prop: BaseScript.field_int,
                    field_fontsize: XGUIFontSize.M,
                    field_text_offset: Vector2.zero,
                    field_height: 20,
                    field_padding: new RectOffset(0, 0, 0, 0),
                    field_margin: new RectOffset(0, 0, 5, 0),
                    field_text_color: Color.white,
                    title_width: 80,
                    field_text_font: XGUI.GetFont("xg-medium"),
                    field_text_style: FontStyle.Normal,
                    field_text_anchor: TextAnchor.MiddleLeft,
                    status_icon: "icon_field_status",
                    status_icon_color: Color.green);

                BaseScript.field_vector2 = XGUI.layout_inputfield(
                    title: "Eg",
                    prop: BaseScript.field_vector2,
                    title_size: XGUIFontSize.M,
                    title_anchor: TextAnchor.MiddleLeft,
                    title_color: Color.white,
                    title_width: 80,
                    field_padding: new RectOffset(0, 0, 0, 0),
                    field_margin: new RectOffset(0, 0, 5, 0),
                    status_icon: "icon_field_status",
                    status_icon_color: Color.green);

                BaseScript.field_vector3 = XGUI.layout_inputfield(
                   title: "Fr",
                   prop: BaseScript.field_vector3,
                   title_size: XGUIFontSize.M,
                   title_anchor: TextAnchor.MiddleLeft,
                   title_color: Color.white,
                   title_width: 80,
                   field_padding: new RectOffset(0, 0, 0, 0),
                   field_margin: new RectOffset(0, 0, 5, 0),
                   status_icon: "icon_field_status",
                    status_icon_color: Color.green);

                BaseScript.field_vector4 = XGUI.layout_inputfield(
                    title: "Gs",
                    prop: BaseScript.field_vector4,
                    title_size: XGUIFontSize.M,
                    title_anchor: TextAnchor.MiddleLeft,
                    title_color: Color.white,
                    title_width: 80,
                    field_padding: new RectOffset(0, 0, 0, 0),
                    field_margin: new RectOffset(0, 0, 5, 0),
                    status_icon: "icon_field_status",
                    status_icon_color: Color.green);

                BaseScript.field_color = XGUI.layout_inputfield(
                    title: "Hz",
                    prop: BaseScript.field_color,
                    title_size: XGUIFontSize.M,
                    title_anchor: TextAnchor.MiddleLeft,
                    title_color: Color.white,
                    title_width: 80,
                    field_padding: new RectOffset(0, 0, 0, 0),
                    field_margin: new RectOffset(0, 0, 5, 0),
                    status_icon: "icon_field_status",
                    status_icon_color: Color.green);

                BaseScript.field_curve = XGUI.layout_inputfield(
                    title: "Sd",
                    prop: BaseScript.field_curve,
                    title_size: XGUIFontSize.M,
                    title_anchor: TextAnchor.MiddleLeft,
                    title_color: Color.white,
                    title_width: 80,
                    field_padding: new RectOffset(0, 0, 0, 0),
                    field_margin: new RectOffset(0, 0, 5, 0),
                    status_icon: "icon_field_status",
                    status_icon_color: Color.green);
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 输入框（手动布局）
            BaseScript.fold_inputfield_Manual = XGUI.layout_group_start(
              type: XGUIContainerType.Vertical,
              bg_fill: XGUIFilled.缺口纯色边框,
              bg_color: XGUIColor.亮白,
              bg_color_gui: BaseScript.ThemeColor_Group,
              title: "输入框 （手动布局）",
              title_size: XGUIFontSize.M,
              title_text_color: BaseScript.ThemeColor_Primary,
              title_clipping: TextClipping.Clip,
              padding: new RectOffset(10, 10, 15, 15),
              foldout: BaseScript.fold_inputfield_Manual);

            // 测试区域
            //XGUI.gui_box(rect_field_root, Color.green * 0.5f);

            if (BaseScript.fold_inputfield_Manual)
            {
                Rect rect_field_root = XGUI.GetControlRect(false, 340);

                float margin = 28;
                Rect rect_field = new Rect(rect_field_root.x + 5, rect_field_root.y, rect_field_root.width - 10, XGUI.GetSingleLineHeight());

                BaseScript.field_string = XGUI.gui_inputfield(
                    rect: rect_field,
                    title: "Am",
                    prop: BaseScript.field_string,
                    text_wrap: false,
                    field_fontsize: XGUIFontSize.M,
                    field_text_offset: Vector2.zero,
                    field_height: 20,
                    field_text_color: Color.white,
                    title_width: 40,
                    status_icon: "icon_field_status",
                    status_icon_color: Color.green,
                    field_text_font: XGUI.GetFont("xg-medium"),
                    field_text_style: FontStyle.Normal,
                    field_text_anchor: TextAnchor.MiddleLeft,
                    field_padding: new RectOffset(5, 5, 0, 0),
                    field_margin: new RectOffset(0, 0, 0, 0));

                rect_field.y += margin;

                BaseScript.field_string = XGUI.gui_inputfield(
                     rect: rect_field,
                     title: "Bk",
                     prop: BaseScript.field_string,
                     text_wrap: true,
                     field_fontsize: XGUIFontSize.M,
                     field_text_offset: Vector2.zero,
                     field_height: 80,
                     field_text_color: Color.white,
                     title_width: 40,
                     status_icon: "icon_field_status",
                     status_icon_color: Color.magenta,
                     field_text_font: XGUI.GetFont("xg-medium"),
                     field_text_style: FontStyle.Normal,
                     field_text_anchor: TextAnchor.UpperLeft,
                     field_padding: new RectOffset(5, 5, 0, 0),
                     field_margin: new RectOffset(0, 0, 0, 0));

                rect_field.y += margin + 60;

                BaseScript.field_float = XGUI.gui_inputfield(
                    rect: rect_field,
                    title: "Cz",
                    prop: BaseScript.field_float,
                    field_fontsize: XGUIFontSize.M,
                    field_text_offset: Vector2.zero,
                    field_height: 20,
                    field_text_color: Color.white,
                    title_width: 40,
                    field_text_font: XGUI.GetFont("xg-medium"),
                    field_text_style: FontStyle.Normal,
                    field_text_anchor: TextAnchor.MiddleLeft,
                    status_icon: "icon_field_status",
                    status_icon_color: Color.red,
                    field_padding: new RectOffset(5, 5, 0, 0),
                    field_margin: new RectOffset(0, 0, 0, 0));

                rect_field.y += margin;

                BaseScript.field_int = XGUI.gui_inputfield(
                     rect: rect_field,
                     title: "Dn",
                     prop: BaseScript.field_int,
                     field_fontsize: XGUIFontSize.M,
                     field_text_offset: Vector2.zero,
                     field_height: 20,
                     field_text_color: Color.white,
                     title_width: 40,
                     field_text_font: XGUI.GetFont("xg-medium"),
                     field_text_style: FontStyle.Normal,
                     field_text_anchor: TextAnchor.MiddleLeft,
                     status_icon: "icon_field_status",
                     status_icon_color: Color.red,
                     field_padding: new RectOffset(5, 5, 0, 0),
                     field_margin: new RectOffset(0, 0, 0, 0));

                rect_field.y += margin;

                BaseScript.field_vector2 = XGUI.gui_inputfield(
                     rect: rect_field,
                     title: "Eg",
                     prop: BaseScript.field_vector2,
                     title_size: XGUIFontSize.M,
                     //title_color: Color.white,
                     title_anchor: TextAnchor.MiddleLeft,
                     title_width: 40,
                     status_icon: "icon_field_status",
                     status_icon_color: Color.red);

                rect_field.y += margin;

                BaseScript.field_vector3 = XGUI.gui_inputfield(
                     rect: rect_field,
                     title: "Fr",
                     prop: BaseScript.field_vector3,
                     title_size: XGUIFontSize.M,
                     //title_color: Color.white,
                     title_anchor: TextAnchor.MiddleLeft,
                     title_width: 40,
                     status_icon: "icon_field_status",
                     status_icon_color: Color.red);

                rect_field.y += margin;

                BaseScript.field_vector4 = XGUI.gui_inputfield(
                     rect: rect_field,
                     title: "Gs",
                     prop: BaseScript.field_vector4,
                     title_size: XGUIFontSize.M,
                     //title_color: Color.white,
                     title_anchor: TextAnchor.MiddleLeft,
                     title_width: 40,
                     status_icon: "icon_field_status",
                     status_icon_color: Color.red);

                rect_field.y += margin;

                BaseScript.field_quaternion = XGUI.gui_inputfield(
                     rect: rect_field,
                     title: "Pe",
                     prop: BaseScript.field_quaternion,
                     title_size: XGUIFontSize.M,
                     //title_color: Color.white,
                     title_anchor: TextAnchor.MiddleLeft,
                     title_width: 40,
                     status_icon: "icon_field_status",
                     status_icon_color: Color.red);

                rect_field.y += margin;

                BaseScript.field_color = XGUI.gui_inputfield(
                     rect: rect_field,
                     title: "Hz",
                     prop: BaseScript.field_color,
                     title_size: XGUIFontSize.M,
                     //title_color: Color.white,
                     title_anchor: TextAnchor.MiddleLeft,
                     title_width: 40,
                     status_icon: "icon_field_status",
                     status_icon_color: Color.red);

                rect_field.y += margin;

                BaseScript.field_curve = XGUI.gui_inputfield(
                     rect: rect_field,
                     title: "Ng",
                     prop: BaseScript.field_curve,
                     title_size: XGUIFontSize.M,
                     //title_color: Color.white,
                     title_anchor: TextAnchor.MiddleLeft,
                     title_width: 40,
                     status_icon: "icon_field_status",
                     status_icon_color: Color.red);
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 滑动条（手动布局）
            BaseScript.fold_sliders_gui = XGUI.layout_group_start(
              type: XGUIContainerType.Vertical,
              bg_fill: XGUIFilled.缺口纯色边框,
              bg_color: XGUIColor.亮白,
              bg_color_gui: BaseScript.ThemeColor_Group,
              title: "滑动条 （手动布局）",
              title_size: XGUIFontSize.M,
              title_text_color: BaseScript.ThemeColor_Primary,
              title_clipping: TextClipping.Clip,
              padding: new RectOffset(10, 10, 15, 15),
              foldout: BaseScript.fold_sliders_gui);

            // 测试区域
            //XGUI.gui_box(rect_field_root, Color.green * 0.5f);

            if (BaseScript.fold_sliders_gui)
            {
                Rect rect_field_root = XGUI.GetControlRect(false, 235);

                float margin = 60;
                Rect rect_slider = new Rect(rect_field_root.x + 5, rect_field_root.y, rect_field_root.width - 10, XGUI.GetSingleLineHeight());

                #region 手动布局滑动条                

                float per = BaseScript.param_slider_manual / 100;

                XGUI.gui_icon(
                  rect: new Rect(rect_slider.x + (rect_slider.width / 2) - (35 / 2), rect_slider.y + 5, 35, 35),
                  icon: icons_slider,
                  color: Color.black * 0.35f);

                XGUI.gui_icon(
                    rect: new Rect(rect_slider.x + (rect_slider.width / 2) - (35 / 2), rect_slider.y + 5, 35, 35),
                    icon: icons_slider,
                    color: new Color(BaseScript.ThemeColor_Primary.r, BaseScript.ThemeColor_Primary.g, BaseScript.ThemeColor_Primary.b, per));

                rect_slider.y += margin;
                BaseScript.param_slider_manual = XGUI.gui_slider(
                    rect: rect_slider,
                    title: "Fire",
                    title_size: XGUIFontSize.M,
                    title_anchor: TextAnchor.MiddleLeft,
                    title_color: Color.white,
                    title_width: 60,
                    status_icon: "icon_field_status",
                    status_icon_color: Color.red,
                    prop: BaseScript.param_slider_manual,
                    left: 0,
                    right: 100,
                    slider_height: 20,
                    limite_width: 230);

                float per_left = BaseScript.param_slider_manual_min / 255;
                float per_right = BaseScript.param_slider_manual_max / 255;

                XGUI.gui_icon(
                    rect: new Rect(rect_slider.x + (rect_slider.width / 2) - (35 / 2), rect_slider.y + margin, 35, 35),
                    icon: icons_slider,
                    color: new Color(per_left, per_right, BaseScript.ThemeColor_Primary.b, 1));

                rect_slider.y += margin * 2;

                BaseScript.param_slider_manual_val = XGUI.gui_slider_min_max(
                    ref_min: ref BaseScript.param_slider_manual_min,
                    ref_max: ref BaseScript.param_slider_manual_max,
                    rect: rect_slider,
                    title: "KSIT",
                    title_size: XGUIFontSize.M,
                    title_anchor: TextAnchor.MiddleLeft,
                    title_color: Color.white,
                    title_width: 60,
                    status_icon: "icon_field_status",
                    status_icon_color: Color.green,
                    limite_width: 230,
                    slider_height: 20,
                    min_limite: 0,
                    max_limite: 255);

                #endregion
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 颜色（自动布局）
            BaseScript.fold_color_gui_layout = XGUI.layout_group_start(
              type: XGUIContainerType.Vertical,
              bg_fill: XGUIFilled.缺口纯色边框,
              bg_color: XGUIColor.亮白,
              bg_color_gui: BaseScript.ThemeColor_Group,
              title: "颜色（自动布局）",
              title_size: XGUIFontSize.M,
              title_text_color: BaseScript.ThemeColor_Primary,
              title_clipping: TextClipping.Clip,
              padding: new RectOffset(10, 10, 15, 15),
              foldout: BaseScript.fold_color_gui_layout);

            // 测试区域
            //XGUI.gui_box(rect_field_root, Color.green * 0.5f);

            if (BaseScript.fold_color_gui_layout)
            {
                BaseScript.param_color = XGUI.layout_colorfield(
                    prop: ThemeColor_Secondary,
                    title: "颜色-次级",
                    title_width: 80,
                    //title_color: Color.white,
                    icon: "icon_field_status",
                    icon_color: Color.red,
                    state_title: "Hex：",
                    state_value: $"#{XGUI_Utilitys.Color_To_HexString(BaseScript.param_color)}",
                    state_value_color: BaseScript.param_color);
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 颜色（手动布局）
            BaseScript.fold_color_gui = XGUI.layout_group_start(
              type: XGUIContainerType.Vertical,
              bg_fill: XGUIFilled.缺口纯色边框,
              bg_color: XGUIColor.亮白,
              bg_color_gui: BaseScript.ThemeColor_Group,
              title: "颜色（手动布局）",
              title_size: XGUIFontSize.M,
              title_text_color: BaseScript.ThemeColor_Primary,
              title_clipping: TextClipping.Clip,
              padding: new RectOffset(10, 10, 15, 15),
              foldout: BaseScript.fold_color_gui);

            // 测试区域
            //XGUI.gui_box(rect_field_root, Color.green * 0.5f);

            if (BaseScript.fold_color_gui)
            {
                Rect rect_col = XGUI.GetControlRect(false, 110);
                rect_col.Set(rect_col.x, rect_col.y, rect_col.width, rect_col.height);

                Rect rect_color = new Rect(rect_col.x, rect_col.y, rect_col.width, 25);
                BaseScript.param_color = XGUI.gui_colorfield(
                    rect: rect_color,
                    prop: BaseScript.param_color,
                    title: "颜色-主体",
                    title_width: 80,
                    //title_color: Color.white,
                    icon: "icon_field_status",
                    icon_color: Color.red,
                    state_title: "Hex：",
                    state_value: $"#{XGUI_Utilitys.Color_To_HexString(BaseScript.param_color)}",
                    state_value_color: BaseScript.param_color,
                    display_hex: true);

                Rect rect_color2 = new Rect(rect_col.x, rect_col.y + 60, rect_col.width, 25);
                BaseScript.field_color = XGUI.gui_colorfield(
                    rect: rect_color2,
                    prop: BaseScript.field_color,
                    title: "颜色-侧翼",
                    title_width: 80,
                    //title_color: Color.white,
                    icon: "icon_field_status",
                    icon_color: Color.red,
                    state_title: "Hex：",
                    state_value: $"#{XGUI_Utilitys.Color_To_HexString(BaseScript.field_color)}",
                    state_value_color: BaseScript.field_color,
                    display_hex: true);
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 曲线（手动布局）
            BaseScript.fold_curve_gui = XGUI.layout_group_start(
              type: XGUIContainerType.Vertical,
              bg_fill: XGUIFilled.缺口纯色边框,
              bg_color: XGUIColor.亮白,
              bg_color_gui: BaseScript.ThemeColor_Group,
              title: "曲线（手动布局）",
              title_size: XGUIFontSize.M,
              title_text_color: BaseScript.ThemeColor_Primary,
              title_clipping: TextClipping.Clip,
              padding: new RectOffset(10, 10, 15, 15),
              foldout: BaseScript.fold_curve_gui);

            // 测试区域
            //XGUI.gui_box(rect_field_root, Color.green * 0.5f);

            if (BaseScript.fold_curve_gui)
            {
                Rect rect_col = XGUI.GetControlRect(false, 120);
                rect_col.Set(rect_col.x, rect_col.y, rect_col.width, rect_col.height);

                Rect rect_color = new Rect(rect_col.x, rect_col.y + 10, rect_col.width, 25);
                BaseScript.gui_curve_A = XGUI.gui_curvefield(
                    rect: rect_color,
                    prop: BaseScript.gui_curve_A,
                    title: "曲线-运动",
                    title_width: 60,
                    //title_color: Color.white,
                    icon: "icon_field_status",
                    icon_color: Color.green,
                    state_title: "说明：",
                    state_value: $"#{XGUI_Utilitys.Color_To_HexString(BaseScript.param_color)}",
                    state_value_color: BaseScript.ThemeColor_Primary,
                    setcurve: (curve) =>
                    {
                        BaseScript.gui_curve_A = curve;
                    });

                Rect rect_color2 = new Rect(rect_col.x, rect_col.y + 70, rect_col.width, 25);
                BaseScript.gui_curve_B = XGUI.gui_curvefield(
                    rect: rect_color2,
                    prop: BaseScript.gui_curve_B,
                    title: "曲线-转向",
                    title_width: 60,
                    //title_color: Color.white,
                    icon: "icon_field_status",
                    icon_color: Color.green,
                    state_title: "说明：",
                    state_value: $"#{XGUI_Utilitys.Color_To_HexString(BaseScript.field_color)}",
                    state_value_color: BaseScript.ThemeColor_Primary,
                     setcurve: (curve) =>
                     {
                         BaseScript.gui_curve_B = curve;
                     });
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            Event e = Event.current;
            if (e.type == EventType.MouseDown && e.keyCode == KeyCode.Mouse1)
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("S 切换折叠编组"), false, () =>
                {
                    BaseScript.fold_param_layout = !BaseScript.fold_param_layout;
                    BaseScript.fold_param_gui = !BaseScript.fold_param_gui;
                    BaseScript.fold_tweenstate_gui = !BaseScript.fold_tweenstate_gui;
                    BaseScript.fold_pops_layout = !BaseScript.fold_pops_layout;
                    BaseScript.fold_sliders_layout = !BaseScript.fold_sliders_layout;
                    BaseScript.fold_sliders_gui = !BaseScript.fold_sliders_gui;
                    BaseScript.fold_inputfield_layout = !BaseScript.fold_inputfield_layout;
                    BaseScript.fold_inputfield_Manual = !BaseScript.fold_inputfield_Manual;
                    BaseScript.fold_state_layout = !BaseScript.fold_state_layout;
                    BaseScript.fold_state_gui = !BaseScript.fold_state_gui;
                    BaseScript.fold_toolbar_layout = !BaseScript.fold_toolbar_layout;
                    BaseScript.fold_pathselector_layout = !BaseScript.fold_pathselector_layout;
                    BaseScript.fold_pathselector_manual = !BaseScript.fold_pathselector_manual;
                    BaseScript.fold_toolbar_gui = !BaseScript.fold_toolbar_gui;
                    BaseScript.fold_color_gui = !BaseScript.fold_color_gui;
                    BaseScript.fold_curve_gui = !BaseScript.fold_curve_gui;
                    BaseScript.fold_toggle_layout = !BaseScript.fold_toggle_layout;
                    BaseScript.fold_listview = !BaseScript.fold_listview;
                    BaseScript.fold_card = !BaseScript.fold_card;
                    BaseScript.fold_card_manual = !BaseScript.fold_card_manual;

                    Repaint();
                });
                menu.ShowAsContext();

                e.Use();
            }
            serializedObject.ApplyModifiedProperties();
        }

        #region 获取类
        private Texture2D GetTexture(Texture2D[] texs, string name)
        {
            Texture2D tex = null;
            for (int i = 0; i < texs.Length; i++)
            {
                if (name == texs[i].name)
                {
                    tex = texs[i];
                }
            }

            return tex;
        }
        #endregion

        #region 绘制方法
        /// <summary>
        /// 通用方法：绘制参数
        /// </summary>
        private void DrawParamField(string title, SerializedProperty prop, float width, Color status_color)
        {
            XGUI.layout_property_field(
                title: title,
                title_size: XGUIFontSize.M,
                title_hover_color: BaseScript.ThemeColor_Primary,
                title_width: width,
                status_icon: status_color == Color.clear ? null : "icon_field_status",
                status_icon_color: status_color,
                prop: prop,
                prop_margin: new RectOffset(0, 0, 5, 10));
        }
        /// <summary>
        /// 通用方法：绘制参数
        /// </summary>
        private void DrawParamField(Rect rect, GUIContent title, SerializedProperty prop, float width)
        {
            XGUI.gui_property_field(
                rect: rect,
                title: title,
                title_size: XGUIFontSize.M,
                title_hover_color: BaseScript.ThemeColor_Primary,
                title_width: 80,
                status_icon: "icon_field_status",
                status_icon_color: Color.red,
                prop: prop);
        }
        /// <summary>
        /// 通用方法：绘制开关
        /// </summary>
        private void DrawToggle(string title, SerializedProperty prop, float width, XGUIToggleStyle style = XGUIToggleStyle.实体, Color color_bg_on = default, Color color_bg_off = default, Color color_on = default, Color color_off = default, Action<bool> act_on_changed = null)
        {
            XGUI.layout_toggle(
                title: title,
                title_size: XGUIFontSize.M,
                title_font_style: FontStyle.Normal,
                title_padding: new RectOffset(0, 10, 0, 0),
                title_width: width,
                prop: prop,
                tog_style: style,
                tog_padding: new RectOffset(5, 0, 0, 0),
                tog_margin: new RectOffset(0, 0, 0, 5),
                tog_mixed_options: new string[] { "禁用", "启用" },
                tog_mixed_text_size: XGUIFontSize.M,
                tog_mixed_text_color: Color.black,
                tog_mixed_text_padding: new RectOffset(10, 10, 0, 0),
                tog_mixed_text_anchor: TextAnchor.MiddleCenter,
                tog_mixed_font_style: FontStyle.Normal,
                tog_bg_off_color: color_bg_off,
                tog_bg_on_color: color_bg_on,
                tog_handler_off_color: color_off,
                tog_handler_on_color: color_on,
                tog_mixed_bg_color_gui: BaseScript.ThemeColor_Primary,
                act_on_changed: act_on_changed);
        }
        #endregion    
    }
}