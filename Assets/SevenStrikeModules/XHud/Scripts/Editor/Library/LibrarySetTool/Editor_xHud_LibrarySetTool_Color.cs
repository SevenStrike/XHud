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
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    #region 数据模型
    /// <summary>
    /// HSV 颜色数据。
    /// <para/>
    /// 用于色相 / 饱和度 / 明度的相互转换与色系判定。
    /// </summary>
    public struct HSV
    {
        /// <summary>
        /// 色相（Hue）。
        /// </summary>
        public float H;
        /// <summary>
        /// 饱和度（Saturation）。
        /// </summary>
        public float S;
        /// <summary>
        /// 明度（Value）。
        /// </summary>
        public float V;
        /// <summary>
        /// 构造 HSV 数据。
        /// </summary>
        /// <param name="hue">色相</param>
        /// <param name="saturation">饱和度</param>
        /// <param name="value">明度</param>
        public HSV(float hue, float saturation, float value)
        {
            H = hue;
            S = saturation;
            V = value;
        }
    }
    #endregion

    public class Editor_XHud_LibrarySetTool_Color : EditorWindow
    {
        #region 字段 - 属性
        /// <summary>
        /// 用于序列化当前窗口的自定义字段。
        /// </summary>
        private SerializedObject base_object;
        #endregion

        #region 字段 - 序列化属性
        /// <summary>
        /// 色卡名称属性
        /// </summary>
        private SerializedProperty sp_color_name;
        /// <summary>
        /// 原始色卡名称属性
        /// </summary>
        private SerializedProperty sp_origin_color_name;
        /// <summary>
        /// 颜色值属性
        /// </summary>
        private SerializedProperty sp_color;
        #endregion

        #region 字段 - 图标
        /// <summary>
        /// 窗口 Logo 图标。
        /// </summary>
        private Texture2D icon_libsetter_color;
        /// <summary>
        /// 左箭头释放图标。
        /// </summary>
        private Texture2D leftarr_r;
        /// <summary>
        /// 左箭头按下图标。
        /// </summary>
        private Texture2D leftarr_p;
        /// <summary>
        /// 右箭头释放图标。
        /// </summary>
        private Texture2D rightarr_r;
        /// <summary>
        /// 右箭头按下图标。
        /// </summary>
        private Texture2D rightarr_p;
        #endregion

        #region 字段 - 预览
        /// <summary>
        /// 每帧预览动画的持续时间间隔（秒）。
        /// </summary>
        private float preview_interval = 0.035f;
        /// <summary>
        /// 当前预览动画数据的索引。
        /// </summary>
        private int preview_data_index = 0;
        /// <summary>
        /// 当前预览纹理（由 SequencePreviewer 的 Current 同步过来）。
        /// </summary>
        private Texture2D preview_tex = null;
        [System.NonSerialized]
        /// <summary>
        /// 预览序列帧数据数组。
        /// </summary>
        public XHud_LibrarySetTool_Color_PreviewData[] preview_datas;
        /// <summary>
        /// 预览区背景色。
        /// </summary>
        Color preview_bg_color = Color.black;
        #endregion

        #region 字段 - 模式与源
        /// <summary>
        /// 库设置模式（添加到库 / 修改库源参数）。
        /// </summary>
        public LibrarySetterMode library_setter_mode;
        /// <summary>
        /// 源图元配色器。
        /// <para/>
        /// 用于从图元配色器中采集颜色或回写色卡信息。
        /// </summary>
        public XHud_Module_Primitive_Painting source_painting;
        /// <summary>
        /// 目标色卡库。
        /// </summary>
        private XHud_Library_Colors target_hud_colors_library;
        #endregion

        #region 字段 - 数据
        /// <summary>
        /// 色卡名称。
        /// </summary>
        [SerializeField]
        public string color_name;
        /// <summary>
        /// 日期时间标识。
        /// </summary>
        [SerializeField]
        public string date_times;
        /// <summary>
        /// 色卡颜色。
        /// </summary>
        [SerializeField]
        public Color color;
        /// <summary>
        /// 原始色卡名称。
        /// <para/>
        /// 修改库源参数模式下用于定位原始项。
        /// </summary>
        [SerializeField]
        public string origin_color_name;
        /// <summary>
        /// 修改的库项索引。
        /// </summary>
        public int modified_index;
        #endregion

        #region 字段 - 布局
        /// <summary>
        /// 按钮高度。
        /// </summary>
        private float button_height = 25;
        /// <summary>
        /// 按钮间距。
        /// </summary>
        private float button_distance = 10;
        /// <summary>
        /// 窗口标题文字。
        /// </summary>
        public string title_primary;
        /// <summary>
        /// 窗口副标题。
        /// </summary>
        string title_sub;
        /// <summary>
        /// 是否使用预览背景。
        /// </summary>
        [SerializeField]
        public bool use_bg = true;
        /// <summary>
        /// 确认按钮文字。
        /// </summary>
        [SerializeField]
        public string button_text_ok;
        /// <summary>
        /// 取消按钮文字。
        /// </summary>
        [SerializeField]
        public string button_text_cancel;
        /// <summary>
        /// 通用绘制矩形缓存。
        /// </summary>
        Rect draw_rect = new Rect(0, 0, 0, 0);
        #endregion

        #region 生命周期
        /// <summary>
        /// Unity 禁用回调。
        /// <para/>
        /// 停止预览动画。
        /// </summary>
        private void OnDisable()
        {
            XGUI.EditorPlayer_Sequence.Stop();
        }
        /// <summary>
        /// Unity 启用回调。
        /// <para/>
        /// 执行流程：
        /// <list type="number">
        /// <item><description>启动预览动画；</description></item>
        /// <item><description>绑定序列化属性；</description></item>
        /// <item><description>加载全部图标资源；</description></item>
        /// <item><description>初始化色卡名称与预览索引。</description></item>
        /// </list>
        /// </summary>
        private void OnEnable()
        {
            base_object = new SerializedObject(this);

            #region 获取序列化属性
            sp_color_name = base_object.FindProperty(nameof(color_name));
            sp_color = base_object.FindProperty(nameof(color));
            sp_origin_color_name = base_object.FindProperty(nameof(origin_color_name));
            #endregion

            #region 获取图片
            icon_libsetter_color = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_color_setter/logo");
            leftarr_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_color_setter/leftarr_p");
            leftarr_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_color_setter/leftarr_r");
            rightarr_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_color_setter/rightarr_p");
            rightarr_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_color_setter/rightarr_r");
            #endregion

            color_name = "颜色名称";

            preview_data_index = XGUI.x_Editor_Data_Get_With_Int("xData_library_color_previewdata_index");

            // 新增：加载 + 启动预览
            LoadPreviewDatas();
            PlayCurrentPreview();
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
        /// 抬头 → 预览（含翻页 / 背景切换） → 参数（标识 / 颜色 / 颜色信息 / RGBA） → 按钮。
        /// <para/>
        /// 支持 Escape 键关闭窗口与点击空白区域失焦。
        /// </summary>
        private void OnGUI()
        {
            base_object.Update();

            Rect rect = new Rect(0, 0, position.width, position.height);

            TextClipping clipping_ellipsis = XGUI.TryEllipsisClipping();

            #region 抬头
            // 图标
            Rect rect_icon = new Rect(15, 15, icon_libsetter_color.width, icon_libsetter_color.height);
            XGUI.gui_icon(
                rect: rect_icon,
                icon: icon_libsetter_color,
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
            Rect rect_subtitle = new Rect(rect.x + 18, rect.y + 45, rect.width, 30);
            XGUI.gui_label(
                rect: rect_subtitle,
                text: new GUIContent(title_sub),
                text_color: Color.white * 0.7f,
                size: XGUIFontSize.M,
                clipping: clipping_ellipsis);
            #endregion

            XGUI.layout_space(100);

            Rect rect_preview = XGUI.GetControlRect(false, 290);

            #region 预览
            rect_preview.Set(rect.x + 14, rect_preview.y, rect_preview.width - 23, rect_preview.height);
            XGUI.gui_group_start(
                rect_group: rect_preview,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                bg_height: 0,
                true,
                true,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                title: "预览",
                title_bg_fill: XGUIGroup.无,
                title_bg_color: XGUIColor.亮白,
                title_bg_color_gui: Color.black,
                title_size: XGUIFontSize.M,
                title_anchor: TextAnchor.MiddleLeft,
                title_text_color: Color.white,
                title_offset: new Vector2(0, 0),
                title_padding: new RectOffset(5, 0, 2, 2),
                title_font: null,
                title_font_style: FontStyle.Normal,
                title_clipping: TextClipping.Clip,
                icon: null,
                icon_color: Color.white,
                icon_padding: new RectOffset(0, 0, 10, 10),
                foldout: false);

            XGUI.gui_group_end();

            draw_rect.Set(rect_preview.x + 10, rect_preview.y + 10, rect_preview.width - 20, rect_preview.height - 20);
            if (use_bg)
            {
                #region 预览背景
                if (XGUI_Utilitys.ColorBrightness_LimiteGet(sp_color.colorValue, 0.2f))
                {
                    preview_bg_color = Color.black;
                }
                else
                {
                    preview_bg_color = Color.white * 0.88f;
                }
                XGUI.gui_box(draw_rect, preview_bg_color);
                #endregion
            }

            #region 预览图片（尺寸300 x 265）
            Texture2D frame = XGUI.EditorPlayer_Sequence.IsPlaying
                ? XGUI.EditorPlayer_Sequence.Current
                : preview_tex;

            draw_rect.Set(rect_preview.x + ((rect_preview.width / 2) - (frame ? frame.width / 2 : 0)), rect_preview.y + 10, 300, 265);

            XGUI.gui_icon(
                rect: draw_rect,
                icon: frame,
                border: new RectOffset(0, 0, 0, 0),
                color: sp_color.colorValue);
            #endregion

            #region 背景切换
            draw_rect.Set(rect_preview.x + ((rect_preview.width / 2) - 32), rect_preview.y + (rect_preview.height - 65), 64, 64);
            string bgstatu = "显示背景";
            if (use_bg)
            {
                bgstatu = "显示背景";
            }
            else
            {
                bgstatu = "无背景";
            }

            if (XGUI.gui_button(
                rect: draw_rect,
                text: bgstatu,
                tooltip: "切换背景",
                btn_fill: XGUIGroup.无,
                btn_color: XGUIColor.亮白,
                btn_color_gui: Color.white,
                btn_text_color: Color.white,
                press_fill: XGUIGroup.无,
                press_color: XGUIColor.深空灰,
                press_text_color: Color.gray,
                font_size: XGUIFontSize.S,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0)))
            {
                use_bg = !use_bg;
            }
            #endregion

            #region 翻页按钮
            #region 左翻页
            draw_rect.Set(rect_preview.x - 5, rect_preview.y + (rect_preview.height - 65), 64, 64);
            if (XGUI.gui_button(
                rect: draw_rect,
                tex_release: leftarr_r,
                tex_press: leftarr_p,
                tex_gui_color: Color.white,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0)))
            {
                if (preview_datas == null || preview_datas.Length == 0)
                {
                    Debug.LogWarning("[ColorPreview] PreviewDatas 为空，无法翻页");
                    return;
                }

                if (preview_data_index <= 0)
                {
                    preview_data_index = preview_datas.Length - 1;
                }
                else
                {
                    preview_data_index--;
                }
                XGUI.x_Editor_Data_Set_With_Int("xData_library_color_previewdata_index", preview_data_index);
                PlayCurrentPreview();
            }
            #endregion

            #region 右翻页
            draw_rect.Set(rect_preview.x + (rect_preview.width - 60), rect_preview.y + (rect_preview.height - 65), 64, 64);
            if (XGUI.gui_button(
               rect: draw_rect,
               tex_release: rightarr_r,
               tex_press: rightarr_p,
               tex_gui_color: Color.white,
               margin: new RectOffset(0, 0, 0, 0),
               padding: new RectOffset(0, 0, 0, 0)))
            {
                if (preview_datas == null || preview_datas.Length == 0)
                {
                    Debug.LogWarning("[ColorPreview] PreviewDatas 为空，无法翻页");
                    return;
                }

                if (preview_data_index >= preview_datas.Length - 1)
                {
                    preview_data_index = 0;
                }
                else
                {
                    preview_data_index++;
                }
                XGUI.x_Editor_Data_Set_With_Int("xData_library_color_previewdata_index", preview_data_index);
                PlayCurrentPreview();
            }
            #endregion
            #endregion

            GUI.backgroundColor = Color.white;
            #endregion

            #region 参数
            XGUI.layout_group_start(
              type: XGUIContainerType.Vertical,
              bg_fill: XGUIGroup.缺口纯色边框,
              bg_color: XGUIColor.亮白,
              bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
              title: "参数",
              title_size: XGUIFontSize.M,
              title_text_color: Color.white,
              title_clipping: clipping_ellipsis,
              margin: new RectOffset(10, 10, 0, 0),
              padding: new RectOffset(10, 10, 15, 15));

            #region 入库名称
            if (string.IsNullOrEmpty(sp_color_name.stringValue))
            {
                sp_color_name.stringValue = "颜色名称";
            }

            sp_color_name.stringValue = XGUI.layout_inputfield(
                title: "标识",
                prop: sp_color_name.stringValue,
                text_wrap: false,
                field_fontsize: XGUIFontSize.M,
                field_text_offset: Vector2.zero,
                field_height: 20,
                field_padding: new RectOffset(0, 0, 0, 0),
                field_margin: new RectOffset(0, 0, 5, 5),
                field_text_color: Color.white,
                title_width: 80,
                field_text_font: XGUI.GetFont("xg-medium"),
                field_text_style: FontStyle.Normal,
                field_text_anchor: TextAnchor.MiddleLeft);
            //status_icon: "icon_field_status",
            //status_icon_color: Color.green);

            sp_color_name.serializedObject.ApplyModifiedProperties();
            #endregion

            #region 颜色
            XGUI.layout_colorfield(
                prop: sp_color,
                title: "颜色",
                title_width: 80,
                title_color: Color.white,
                //icon: "icon_field_status",
                //icon_color: Color.red,
                state_title: "Hex：",
                state_value: $"#{XGUI_Utilitys.Color_To_HexString(sp_color.colorValue)}",
                state_value_color: Color.gray,
                state_dialog_theme_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
            #endregion

            XGUI.layout_seperator(
                thickness: 1,
                color: XHud_Dashboard.themes_get_color(ThemeColorType.sepline),
                margin: new RectOffset(15, 15, 15, 15));

            string col_string = XGUI_Utilitys.Color_To_String(sp_color.colorValue);
            string col_hex = XGUI_Utilitys.Color_To_HexString(sp_color.colorValue, true);
            float[] col_array = XGUI_Utilitys.Color_To_FloatArray(sp_color.colorValue);

            #region 颜色字符串
            XGUI.layout_state_displayer_text(
                  title: "颜色字符串",
                  title_size: XGUIFontSize.M,
                  subtitle: col_string,
                  subtitle_size: XGUIFontSize.M,
                  subtitle_color: Color.white * 0.7f,
                  margin: new RectOffset(5, 5, 0, 5));
            #endregion

            #region 16进制颜色
            XGUI.layout_state_displayer_text(
                title: "16进制颜色",
                title_size: XGUIFontSize.M,
                subtitle: col_hex,
                subtitle_size: XGUIFontSize.M,
                subtitle_color: Color.white * 0.7f,
                margin: new RectOffset(5, 5, 0, 5));
            #endregion

            XGUI.layout_seperator(
                thickness: 1,
                color: XHud_Dashboard.themes_get_color(ThemeColorType.sepline),
                margin: new RectOffset(15, 15, 15, 15));

            #region RGBA
            Rect rect_rgba = XGUI.GetControlRect(false, XGUI.GetSingleLineHeight());
            //XGUI.gui_box(rect_rgba, Color.red * 0.3f);

            float pers = rect_rgba.width / 4 + 3.5f;
            Color col = Color.white;
            string label = "";

            for (int i = 0; i < 4; i++)
            {
                if (i == 0)
                {
                    col = new Color(col_array[i], 0, 0);
                    label = "R";
                }
                if (i == 1)
                {
                    col = new Color(0, col_array[i], 0);
                    label = "G";
                }
                if (i == 2)
                {
                    col = new Color(0, 0, col_array[i]);
                    label = "B";
                }
                if (i == 3)
                {
                    col = new Color(1, 1, 1, col_array[i]);
                    label = "A";
                }

                draw_rect.Set(rect_rgba.x + (i != 0 ? pers : 0) * i + 5, rect_rgba.y, 4, rect_rgba.height);
                XGUI.gui_box(draw_rect, col);

                draw_rect.Set(rect_rgba.x + (i != 0 ? pers : 0) * i + 15, rect_rgba.y, 60, rect_rgba.height);
                XGUI.gui_label(
                    rect: draw_rect,
                    text: new GUIContent($"{label} ：{col_array[i].ToString("F2")}"),
                    text_color: Color.white,
                    font_style: FontStyle.Bold,
                    offset: new Vector2(0, 0),
                    size: XGUIFontSize.S,
                    padding: new RectOffset(0, 0, 0, 0),
                    anchor: TextAnchor.MiddleLeft,
                    clipping: TextClipping.Clip);
            }
            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            base_object.ApplyModifiedProperties();

            XGUI.layout_space(10);

            bottom_buttons();

            #region 检测事件
            Event e = Event.current;
            // 检测点击事件
            if (e.type == EventType.MouseDown)
            {
                draw_rect.Set(rect.width - 280, rect.height - 175, (rect.width / 2) - 55, 60);
                // 检查点击位置是否在窗口内
                if (!draw_rect.Contains(e.mousePosition))
                {
                    GUI.FocusControl(null); // 取消所有控件的焦点
                    Repaint(); // 重新绘制窗口
                }
                draw_rect.Set(rect.width - 280, rect.height - 230, (rect.width / 2) - 55, 50);
                // 检查点击位置是否在窗口内
                if (!draw_rect.Contains(e.mousePosition))
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

        #region 工具 - 数据操作
        /// <summary>
        /// 发送色卡到目标色卡库（添加到库模式）。
        /// <para/>
        /// 执行流程：
        /// <list type="number">
        /// <item><description>校验名称是否为空；</description></item>
        /// <item><description>检测色卡名称是否重复；</description></item>
        /// <item><description>向目标库添加色卡；</description></item>
        /// <item><description>弹窗提示是否打开色卡库；</description></item>
        /// <item><description>询问是否开启源配色器的色卡模式并同步。</description></item>
        /// </list>
        /// </summary>
        private void SendToLibrary()
        {
            if (sp_color_name.stringValue == "颜色名称")
            {
                string res = XGUI.dialog(
                type: XGUIDialogType.警告,
                windowtitle: "XHud - 色卡库采集器消息",
                title: "未填写名称",
                msg: "请为色卡添加一个名称！",
                ok: "明白",
                PrimaryIndex: 0,
                usemodal: true,
                themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                return;
            }

            string colorhex = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.themes_get_color(ThemeColorType.primary), true);
            bool exist = target_hud_colors_library.ColorsLibrary_IsExist(sp_color_name.stringValue);

            if (exist)
            {
                EditorApplication.delayCall += () =>
                {
                    string res = XGUI.dialog(
                    type: XGUIDialogType.警告,
                    windowtitle: "XHud - 色卡库采集器消息",
                    title: "存在重复色卡名称",
                    msg: $"名称为<color={colorhex}> {sp_color_name.stringValue} </color>的色卡已经存在于色卡库中！",
                    ok: "重命名",
                    PrimaryIndex: 0,
                    usemodal: true,
                    themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));

                    if (res == "重命名")
                    {
                        return;
                    }
                };
            }
            else
            {
                xHud_LibraryArg_Color info = new xHud_LibraryArg_Color();
                info.name = sp_color_name.stringValue;
                info.color = sp_color.colorValue;
                target_hud_colors_library.ColorsLibrary_AddColor(info);
                EditorApplication.delayCall += () =>
                {
                    XGUI.dialog(
                         type: XGUIDialogType.确认,
                         windowtitle: "XHud - 色卡库采集器消息",
                         title: "已添加到色卡库",
                         msg: $"已将名称为<color={colorhex}> {sp_color_name.stringValue} </color>的色卡添加到色卡库中！",
                         ok: "明白",
                         PrimaryIndex: 0,
                         usemodal: true,
                         themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                };

                Close();

                EditorApplication.delayCall += () =>
                {
                    string res = XGUI.dialog(
                    type: XGUIDialogType.帮助,
                    windowtitle: "XHud - 色卡库采集器消息",
                    title: "存入色卡库",
                    msg: "色卡存入完成，是否要打开色卡库进行查看？",
                    ok: "不用",
                    cancel: "查看色卡库",
                    PrimaryIndex: 0,
                    usemodal: true,
                    themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));

                    if (res == "查看色卡库")
                    {
                        EditorUtility.OpenPropertyEditor(target_hud_colors_library);
                    }
                };
            }

            EditorApplication.delayCall += () =>
            {
                string indicator = source_painting.controller.GetIndicator();

                string res_saved_turnon = XGUI.dialog(
                    type: XGUIDialogType.帮助,
                    windowtitle: "XHud - 色卡库采集器消息",
                    title: "色卡库模式设定",
                    msg: $"是否要将 {(string.IsNullOrEmpty(indicator) ? source_painting.transform.name : indicator)} 色卡模式开启？",
                    ok: "开启",
                    cancel: "暂不",
                    PrimaryIndex: 0,
                    usemodal: true,
                    themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));

                if (res_saved_turnon == "开启")
                {
                    source_painting.sync_library_color = true;
                    int id = target_hud_colors_library.ColorsLibrary_GetColorCount() - 1;
                    source_painting.colorise_name = target_hud_colors_library.ColorsLibrary_GetColorNames()[id];
                }
            };

            this.Close();
        }
        /// <summary>
        /// 更新到目标色卡库（修改库源参数模式）。
        /// <para/>
        /// 用当前编辑的名称与颜色替换目标库中指定索引与原始名称的色卡项，然后关闭窗口。
        /// </summary>
        private void UpdateToLibrary()
        {
            string colorhex = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.themes_get_color(ThemeColorType.primary), true);
            xHud_LibraryArg_Color info = new xHud_LibraryArg_Color();
            info.name = sp_color_name.stringValue;
            info.color = sp_color.colorValue;
            target_hud_colors_library.ColorsLibrary_ReplaceColorAndName(modified_index, sp_origin_color_name.stringValue, sp_color_name.stringValue, sp_color.colorValue);

            Close();
        }
        #endregion

        #region 工具 - 资源加载
        /// <summary>
        /// 获取目标文件夹下的指定类型所有资源。
        /// <para/>
        /// 相对工程目录搜索，并按扩展名匹配文件。
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="path">相对工程根目录的文件夹路径</param>
        /// <param name="pattern">文件扩展名匹配模式（如 <c>.asset</c>）</param>
        /// <returns>已加载的资源列表</returns>
        List<T> LoadAllAssetsAtPathWithPattern<T>(string path, string pattern) where T : UnityEngine.Object
        {
            List<T> _out = new();

            string root_path = Directory.GetParent(Application.dataPath) + "/" + path;

            if (!Directory.Exists(root_path))
            {
                Debug.LogWarning("Path doesn't exist");
                return _out;
            }

            string[] fileEntries = Directory.GetFiles(root_path, $"*{pattern}");

            foreach (string FileName in fileEntries)
            {
                string[] filepath = FileName.Split(Directory.GetParent(Application.dataPath).FullName + "/");
                _out.Add(AssetDatabase.LoadAssetAtPath<T>(filepath[1]));
            }

            return _out;
        }
        /// <summary>
        /// 加载所有序列帧预览资源（只加载一次，已加载则跳过）。
        /// </summary>
        private void LoadPreviewDatas()
        {
            if (preview_datas != null && preview_datas.Length > 0) return;

            string path = $"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_color_setter/samples";
            preview_datas = LoadAllAssetsAtPathWithPattern<XHud_LibrarySetTool_Color_PreviewData>(path, ".asset").ToArray();
        }

        /// <summary>
        /// 按当前 PreviewData_Index 播放对应的序列帧。
        /// <para/>
        /// - 数据为空 / 索引越界 → 停止播放并清空 PreviewTex；
        /// - 否则交给 Editor_XHud_Tool_SequencePreviewer 播放，回调里同步 PreviewTex 并 Repaint。
        /// </summary>
        private void PlayCurrentPreview()
        {
            if (preview_datas == null || preview_datas.Length == 0)
            {
                XGUI.EditorPlayer_Sequence.Stop();
                preview_tex = null;
                return;
            }

            if (preview_data_index < 0 || preview_data_index >= preview_datas.Length)
            {
                preview_data_index = 0;
            }

            XHud_LibrarySetTool_Color_PreviewData data = preview_datas[preview_data_index];
            if (data == null || data.textures == null || data.textures.Count == 0)
            {
                XGUI.EditorPlayer_Sequence.Stop();
                preview_tex = null;
                return;
            }

            XGUI.EditorPlayer_Sequence.Play(
                frames: data.textures,
                interval: preview_interval,
                loopCount: -1,
                onFrame: () =>
                {
                    preview_tex = XGUI.EditorPlayer_Sequence.Current;
                    Repaint();
                });


            // 立即给一次初值，避免下一帧前 PreviewTex 还是旧值
            preview_tex = XGUI.EditorPlayer_Sequence.Current;
        }
        #endregion

        #region 设置接口
        /// <summary>
        /// 设置按钮文字。
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
        /// <param name="name">色卡名称</param>
        /// <param name="color">色卡颜色</param>
        public void SetInfo(string name, Color color)
        {
            sp_color_name.stringValue = name;
            sp_color.colorValue = color;

            sp_color_name.serializedObject.ApplyModifiedProperties();
            sp_color.serializedObject.ApplyModifiedProperties();
        }
        /// <summary>
        /// 设置标题。
        /// </summary>
        /// <param name="title">标题文字</param>
        public void SetTitle(string title)
        {
            this.title_primary = title;
            Repaint();
        }
        /// <summary>
        /// 设置副标题。
        /// </summary>
        /// <param name="title">标题文字</param>
        public void SetSubTitle(string title)
        {
            this.title_sub = title;
            Repaint();
        }
        /// <summary>
        /// 设置原始名称。
        /// </summary>
        /// <param name="name">原始色卡名称</param>
        public void Set_OriginColorName(string name)
        {
            origin_color_name = name;
        }
        /// <summary>
        /// 设置为添加到库模式还是修改库源参数模式。
        /// </summary>
        /// <param name="mode">库设置模式</param>
        public void SetLibrarySetterMode(LibrarySetterMode mode)
        {
            library_setter_mode = mode;
        }
        /// <summary>
        /// 设置源图元配色器。
        /// </summary>
        /// <param name="painting">源配色器</param>
        public void SetPainting(XHud_Module_Primitive_Painting painting)
        {
            source_painting = painting;
        }
        /// <summary>
        /// 设置目标色卡库。
        /// </summary>
        /// <param name="lib">目标色卡库</param>
        public void SetTarget_Hud_ColorsLibrary(XHud_Library_Colors lib)
        {
            target_hud_colors_library = lib;
        }
        #endregion

        #region 按钮
        /// <summary>
        /// 绘制底部按钮区（取消 / 确认）。
        /// <para/>
        /// 确认按钮根据 <see cref="library_setter_mode"/> 决定调用 <see cref="SendToLibrary"/> 或 <see cref="UpdateToLibrary"/>。
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
                }
            }

            XGUI.layout_flexspace();

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            #endregion
        }
        #endregion

        #region 工具 - 颜色分析
        /// <summary>
        /// 将 RGB 颜色转换为 HSV。
        /// </summary>
        /// <param name="color">源 RGB 颜色</param>
        /// <param name="hue">输出色相</param>
        /// <param name="saturation">输出饱和度</param>
        /// <param name="value">输出明度</param>
        private static void RGBToHSV(Color color, out float hue, out float saturation, out float value)
        {
            float r = color.r;
            float g = color.g;
            float b = color.b;

            float max = Mathf.Max(r, g, b);
            float min = Mathf.Min(r, g, b);
            float delta = max - min;

            hue = 0;
            if (delta != 0)
            {
                if (max == r)
                {
                    hue = (g - b) / delta;
                }
                else if (max == g)
                {
                    hue = 2 + (b - r) / delta;
                }
                else
                {
                    hue = 4 + (r - g) / delta;
                }

                hue *= 60;
                if (hue < 0) hue += 360;
            }

            saturation = max == 0 ? 0 : delta / max;
            value = max;
        }
        /// <summary>
        /// 根据 HSV 判断颜色所属色系（如枫叶红 / 松石绿等）。
        /// <para/>
        /// 灰度高则返回 白色系 / 黑色系 / 灰色系；
        /// 否则按色相 10° 一档细分命名。
        /// </summary>
        /// <param name="color">源颜色</param>
        /// <returns>色系名称</returns>
        public string GetColorTone(Color color)
        {
            float hue, saturation, value;
            RGBToHSV(color, out hue, out saturation, out value);

            // 根据饱和度和明度判断是否为灰色系、黑色系或白色系
            if (saturation <= 0f) // 低饱和度
            {
                if (value > 0.8f) return "白色系"; // 高明度
                if (value < 0.2f) return "黑色系"; // 低明度
                return "灰色系"; // 中等明度
            }

            // 根据色相和色环范围判断颜色的色系
            if (hue >= 0 && hue < 10) return "枫叶红"; // 0°-10°
            if (hue >= 10 && hue < 20) return "石榴红"; // 10°-20°
            if (hue >= 20 && hue < 30) return "红砂岩"; // 20°-30°
            if (hue >= 30 && hue < 40) return "红珊瑚"; // 30°-40°
            if (hue >= 40 && hue < 50) return "橙砂岩"; // 40°-50°
            if (hue >= 50 && hue < 60) return "琥珀橙"; // 50°-60°
            if (hue >= 60 && hue < 70) return "橙珊瑚"; // 60°-70°
            if (hue >= 70 && hue < 80) return "橙石竹"; // 70°-80°
            if (hue >= 80 && hue < 90) return "金菊黄"; // 80°-90°
            if (hue >= 90 && hue < 100) return "玉兰黄"; // 90°-100°
            if (hue >= 100 && hue < 110) return "石竹黄"; // 100°-110°
            if (hue >= 110 && hue < 120) return "水晶黄"; // 110°-120°
            if (hue >= 120 && hue < 130) return "竹影绿"; // 120°-130°
            if (hue >= 130 && hue < 140) return "松石绿"; // 130°-140°
            if (hue >= 140 && hue < 150) return "橄榄绿"; // 140°-150°
            if (hue >= 150 && hue < 160) return "苔藓绿"; // 150°-160°
            if (hue >= 160 && hue < 170) return "青石蓝"; // 160°-170°
            if (hue >= 170 && hue < 180) return "天青蓝"; // 170°-180°
            if (hue >= 180 && hue < 190) return "冬青蓝"; // 180°-190°
            if (hue >= 190 && hue < 200) return "梅青蓝"; // 190°-200°
            if (hue >= 200 && hue < 210) return "莓果蓝"; // 200°-210°
            if (hue >= 210 && hue < 220) return "宝石蓝"; // 210°-220°
            if (hue >= 220 && hue < 230) return "鸢尾蓝"; // 220°-230°
            if (hue >= 230 && hue < 240) return "海藻蓝"; // 230°-240°
            if (hue >= 240 && hue < 250) return "罗兰紫"; // 240°-250°
            if (hue >= 250 && hue < 260) return "紫晶紫"; // 250°-260°
            if (hue >= 260 && hue < 270) return "丁香紫"; // 260°-270°
            if (hue >= 270 && hue < 280) return "玫瑰紫"; // 270°-280°
            if (hue >= 280 && hue < 290) return "粉晶紫"; // 280°-290°
            if (hue >= 290 && hue < 300) return "玫瑰粉"; // 290°-300°
            if (hue >= 300 && hue < 310) return "桃花粉"; // 300°-310°
            if (hue >= 310 && hue < 320) return "热粉红"; // 310°-320°
            if (hue >= 320 && hue < 330) return "草莓粉"; // 320°-330°
            if (hue >= 330 && hue < 340) return "胭脂粉"; // 330°-340°
            if (hue >= 340 && hue < 350) return "肉色粉"; // 340°-350°
            if (hue >= 350 && hue < 360) return "浅粉红"; // 350°-360°

            return "未知色";
        }
        #endregion
    }
}