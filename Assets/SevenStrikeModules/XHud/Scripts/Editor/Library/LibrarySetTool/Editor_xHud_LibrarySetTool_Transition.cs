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
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;
    using Object = UnityEngine.Object;

    public class Editor_XHud_LibrarySetTool_Transition : EditorWindow
    {
        #region 字段 - 序列化对象 / 属性
        /// <summary>
        /// 用于序列化当前窗口的自定义字段。
        /// </summary>
        private SerializedObject base_object;
        #endregion

        #region 字段 - 序列化属性
        /// <summary>
        /// 转场节点属性
        /// </summary>
        private SerializedProperty sp_transition_node;
        /// <summary>
        /// 转场名称属性
        /// </summary>
        private SerializedProperty sp_transition_name;
        /// <summary>
        /// 转场说明属性
        /// </summary>
        private SerializedProperty sp_transition_description;
        /// <summary>
        /// 预览背景贴图属性
        /// </summary>
        private SerializedProperty sp_preview_texture_bg;
        /// <summary>
        /// 预览颜色属性
        /// </summary>
        private SerializedProperty sp_preview_color;
        /// <summary>
        /// 背景网格平铺倍数属性
        /// </summary>
        private SerializedProperty sp_bg_grid_tilling_multiply;
        /// <summary>
        /// 背景网格颜色 A 属性
        /// </summary>
        private SerializedProperty sp_bg_grid_color_a;
        /// <summary>
        /// 背景网格颜色 B 属性
        /// </summary>
        private SerializedProperty sp_bg_grid_color_b;
        /// <summary>
        /// 列表项高度属性
        /// </summary>
        private SerializedProperty sp_item_height;
        /// <summary>
        /// 可视区域显示元素数量属性
        /// </summary>
        private SerializedProperty sp_visible_item_count;
        /// <summary>
        /// 选中项索引属性
        /// </summary>
        private SerializedProperty sp_selected_index;
        /// <summary>
        /// 转场信息列表滚动位置属性
        /// </summary>
        private SerializedProperty sp_transition_info_list_scroller;
        /// <summary>
        /// 预览数据索引属性
        /// </summary>
        private SerializedProperty sp_preview_data_index;
        #endregion

        #region 字段 - 列表
        /// <summary>
        /// 序列帧列表（可重排列表）。
        /// </summary>
        private ReorderableList TrasitionInfoList;
        #endregion

        #region 字段 - 数据
        /// <summary>
        /// 当前编辑的转场数据（工作副本）。
        /// </summary>
        [SerializeField]
        private XHud_LibraryArg_Transition transition_node;
        /// <summary>
        /// 转场的原始数据（用于对比）。
        /// </summary>
        [SerializeField]
        private XHud_LibraryArg_Transition OriginTransitionNode;
        /// <summary>
        /// 目标转场库。
        /// </summary>
        private XHud_Library_Transition Target_Hud_TransitionLibrary;
        /// <summary>
        /// 修改的库项索引。
        /// </summary>
        public int ModifiedIndex;
        /// <summary>
        /// 转场名称。
        /// </summary>
        [SerializeField]
        public string transition_name;
        /// <summary>
        /// 日期时间标识。
        /// </summary>
        [SerializeField]
        public string DateTimes;
        /// <summary>
        /// 转场说明。
        /// </summary>
        [SerializeField]
        public string transition_description;
        #endregion

        #region 字段 - 图标
        /// <summary>
        /// 窗口 Logo 图标。
        /// </summary>
        private Texture2D icon;
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
        /// <summary>
        /// 停止按钮释放图标。
        /// </summary>
        private Texture2D stop_r;
        /// <summary>
        /// 停止按钮按下图标。
        /// </summary>
        private Texture2D stop_p;
        /// <summary>
        /// 播放按钮释放图标。
        /// </summary>
        private Texture2D play_r;
        /// <summary>
        /// 播放按钮按下图标。
        /// </summary>
        private Texture2D play_p;
        /// <summary>
        /// 序列帧列表图标。
        /// </summary>
        private Texture2D seq_list_logo;
        #endregion

        #region 字段 - 预览
        /// <summary>
        /// 当前预览索引。
        /// </summary>
        [SerializeField]
        private int preview_data_index;
        /// <summary>
        /// 是否正在播放预览动画。
        /// </summary>
        private bool IsPreviewing;
        /// <summary>
        /// 停止模式（自动复位 / 手动复位）。
        /// </summary>
        private bool StoppedResetMode = true;
        /// <summary>
        /// 每帧预览动画的持续时间间隔。
        /// </summary>
        private float PreviewDataDuration = 0.01f;
        /// <summary>
        /// 反色通道。
        /// </summary>
        private bool InvertChannel = false;
        /// <summary>
        /// 预览材质。
        /// </summary>
        private Material Preview_Material;
        /// <summary>
        /// 预览背景纹理。
        /// </summary>
        [SerializeField]
        private Texture2D preview_texture_bg;
        /// <summary>
        /// 预览颜色。
        /// </summary>
        [SerializeField]
        private Color preview_color = Color.white;
        /// <summary>
        /// 网格平铺倍数（缓存字段，兼容序列化）。
        /// </summary>
        [SerializeField]
#pragma warning disable CS0414
        private float bg_grid_tilling_multiply = 14;
#pragma warning restore CS0414
        /// <summary>
        /// 网格平铺倍数 - 水平。
        /// </summary>
        [SerializeField]
        private float BgGrid_Tilling_x = 14;
        /// <summary>
        /// 网格平铺倍数 - 垂直。
        /// </summary>
        [SerializeField]
        private float BgGrid_Tilling_y = 14;
        /// <summary>
        /// 网格主色。
        /// </summary>
        [SerializeField]
        private Color bg_grid_color_a = Color.white * 0.509f;
        /// <summary>
        /// 网格副色。
        /// </summary>
        [SerializeField]
        private Color bg_grid_color_b = Color.white * 0.376f;
        #endregion

        #region 字段 - 列表参数
        /// <summary>
        /// 列表项高度。
        /// </summary>
        [SerializeField]
        public float item_height = 25;
        /// <summary>
        /// 可视区域显示的元素数量。
        /// </summary>
        [SerializeField]
        public int visible_item_count = 19;
        /// <summary>
        /// 选中项索引号。
        /// </summary>
        [SerializeField]
        public int selected_index;
        /// <summary>
        /// 列表滚动位置。
        /// </summary>
        [SerializeField]
        public Vector2 transition_info_list_scroller;
        #endregion

        #region 字段 - 布局
        /// <summary>
        /// 按钮高度。
        /// </summary>
        private float ButtonHeight = 25;
        /// <summary>
        /// 按钮间距。
        /// </summary>
        private float ButtonDistance = 15;
        /// <summary>
        /// 确认按钮文字。
        /// </summary>
        [SerializeField]
        public string ButtonText_Ok;
        /// <summary>
        /// 取消按钮文字。
        /// </summary>
        [SerializeField]
        public string ButtonText_Cancel;
        /// <summary>
        /// 通用绘制矩形缓存。
        /// </summary>
        private Rect draw_rect;
        /// <summary>
        /// 拖放区矩形缓存。
        /// </summary>
        private Rect dragarea;
        /// <summary>
        /// 窗口标题。
        /// </summary>
        private string Title;
        #endregion

        #region 字段 - 绘制缓存
        /// <summary>
        /// 绘制元素矩形。
        /// </summary>
        Rect drawelement_rect;
        /// <summary>
        /// 滚动视图矩形。
        /// </summary>
        Rect scrollview_rect;
        /// <summary>
        /// 列表项矩形。
        /// </summary>
        Rect item_rect;
        /// <summary>
        /// 选中背景色。
        /// </summary>
        Color SelectedBg = new Color(0, 0, 0, 0.2f);
        #endregion

        #region 生命周期
        /// <summary>
        /// Unity 启用回调。
        /// <para/>
        /// 执行流程：
        /// <list type="number">
        /// <item><description>构建序列化对象并绑定全部属性；</description></item>
        /// <item><description>初始化预览材质与背景纹理；</description></item>
        /// <item><description>恢复上次保存的网格平铺与颜色；</description></item>
        /// <item><description>加载全部图标资源；</description></item>
        /// <item><description>初始化默认名称与说明；</description></item>
        /// <item><description>构造序列帧 ReorderableList。</description></item>
        /// </list>
        /// </summary>
        private void OnEnable()
        {
            base_object = new SerializedObject(this);

            #region 获取序列化属性
            sp_transition_node = base_object.FindProperty(nameof(transition_node));
            sp_transition_name = base_object.FindProperty(nameof(transition_name));
            sp_transition_description = base_object.FindProperty(nameof(transition_description));
            sp_preview_texture_bg = base_object.FindProperty(nameof(preview_texture_bg));
            sp_bg_grid_tilling_multiply = base_object.FindProperty(nameof(bg_grid_tilling_multiply));
            sp_bg_grid_color_a = base_object.FindProperty(nameof(bg_grid_color_a));
            sp_bg_grid_color_b = base_object.FindProperty(nameof(bg_grid_color_b));
            sp_preview_color = base_object.FindProperty(nameof(preview_color));
            sp_item_height = base_object.FindProperty(nameof(item_height));
            sp_visible_item_count = base_object.FindProperty(nameof(visible_item_count));
            sp_selected_index = base_object.FindProperty(nameof(selected_index));
            sp_transition_info_list_scroller = base_object.FindProperty(nameof(transition_info_list_scroller));
            sp_preview_data_index = base_object.FindProperty(nameof(preview_data_index));
            #endregion

            #region 获取图标
            icon = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_transition_setter/logo");
            leftarr_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_transition_setter/left_arrow_p");
            leftarr_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_transition_setter/left_arrow_r");
            rightarr_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_transition_setter/right_arrow_p");
            rightarr_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_transition_setter/right_arrow_r");
            stop_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_transition_setter/prw_stop_r");
            stop_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_transition_setter/prw_stop_p");
            play_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_transition_setter/prw_play_r");
            play_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_transition_setter/prw_play_p");
            seq_list_logo = XGUI.GetBasedIcon("d_RawImage Icon");
            #endregion

            InitializePreviewMaterial();

            LoadPreviewTexture();

            if (XGUI.x_Editor_Data_Has_String("xData_Transition_Bg_Grid_ColorA"))
                sp_bg_grid_color_a.colorValue = XGUI_Utilitys.String_To_Color(XGUI.x_Editor_Data_Get_With_String("xData_Transition_Bg_Grid_ColorA"), false);
            else
                sp_bg_grid_color_a.colorValue = Color.white * 0.509f;

            if (XGUI.x_Editor_Data_Has_String("xData_Transition_Bg_Grid_ColorB"))
                sp_bg_grid_color_b.colorValue = XGUI_Utilitys.String_To_Color(XGUI.x_Editor_Data_Get_With_String("xData_Transition_Bg_Grid_ColorB"), false);
            else
                sp_bg_grid_color_b.colorValue = Color.white * 0.376f;

            if (XGUI.x_Editor_Data_Has_String("xData_Transition_Bg_Grid_Tilling"))
                sp_bg_grid_tilling_multiply.floatValue = float.Parse(XGUI.x_Editor_Data_Get_With_String("xData_Transition_Bg_Grid_Tilling"));
            else
                sp_bg_grid_tilling_multiply.floatValue = 5;

            sp_bg_grid_color_a.serializedObject.ApplyModifiedProperties();
            sp_bg_grid_color_b.serializedObject.ApplyModifiedProperties();
            sp_bg_grid_tilling_multiply.serializedObject.ApplyModifiedProperties();

            transition_description = "转场说明内容";
            transition_name = "转场名称";

            sp_item_height.floatValue = 25;
            sp_item_height.serializedObject.ApplyModifiedProperties();

            sp_visible_item_count.intValue = 15;
            sp_visible_item_count.serializedObject.ApplyModifiedProperties();

            #region ReorderableList - TransitionLibrary
            TrasitionInfoList = new ReorderableList(base_object, sp_transition_node.FindPropertyRelative("frames"), true, true, true, true);
            TrasitionInfoList.drawElementCallback = TransitionInfoList_Original_DrawElementCallback;
            #endregion
        }
        /// <summary>
        /// Unity 禁用回调。
        /// <para/>
        /// 将背景网格颜色 / 平铺 / 背景图路径保存到 xData 中，供下次打开恢复。
        /// </summary>
        private void OnDisable()
        {
            StopPreview();

            XGUI.x_Editor_Data_Set_With_String("xData_Transition_Bg_Grid_ColorA", XGUI_Utilitys.Color_To_String(sp_bg_grid_color_a.colorValue));
            XGUI.x_Editor_Data_Set_With_String("xData_Transition_Bg_Grid_ColorB", XGUI_Utilitys.Color_To_String(sp_bg_grid_color_b.colorValue));
            XGUI.x_Editor_Data_Set_With_String("xData_Transition_Bg_Grid_Tilling", sp_bg_grid_tilling_multiply.floatValue.ToString("F2"));
            XGUI.x_Editor_Data_Set_With_String("xData_Transition_Bg_Path", AssetDatabase.GetAssetPath(sp_preview_texture_bg.objectReferenceValue));
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
        /// 抬头 → 序列帧列表 → 控制区（视觉配置 / 预览操作 / 播放控制） →
        /// 信息区（名称 / 说明 / 帧计数） → 预览（材质渲染 / 帧控制） → 按钮。
        /// <para/>
        /// 支持拖放添加序列帧、键盘上下左右切换帧、Escape 关闭窗口。
        /// </summary>
        private void OnGUI()
        {
            base_object.Update();

#if UNITY_6000_0_OR_NEWERdddd
            TextClipping clipping = TextClipping.Ellipsis;
#else
            TextClipping clipping = TextClipping.Clip;
#endif

            string colorhex = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.themes_get_color(ThemeColorType.primary), true);

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
                color: XHud_Dashboard.themes_get_color(ThemeColorType.sepline),
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0));

            // 小标题
            Rect rect_subtitle = new Rect(rect.x + 18, rect.y + 53, rect.width, 30);
            XGUI.gui_label(
                rect: rect_subtitle,
                text: new GUIContent("转场资源参数与效果的预览，您可以检查和调整转场资源！(请确保导入的序列图片颜色空间匹配您的渲染管线！)"),
                text_color: Color.white * 0.7f,
                size: XGUIFontSize.M,
                anchor: TextAnchor.UpperLeft,
                clipping: clipping);
            #endregion

            SerializedProperty sp_name = sp_transition_node.FindPropertyRelative("name");
            SerializedProperty sp_des = sp_transition_node.FindPropertyRelative("description");
            SerializedProperty sp_TotalFramesCount = sp_transition_node.FindPropertyRelative("total_frames_count");
            SerializedProperty sp_LastFrameIndex = sp_transition_node.FindPropertyRelative("last_frame_index");
            SerializedProperty sp_SkipFrame = sp_transition_node.FindPropertyRelative("skip_frame");
            SerializedProperty sp_Res = sp_transition_node.FindPropertyRelative("res");
            SerializedProperty sp_Frames = sp_transition_node.FindPropertyRelative("frames");

            #region 数值关联
            if (sp_Frames.arraySize > 0)
            {
                sp_Res.vector2IntValue = new Vector2Int(transition_node.frames[0].width, transition_node.frames[0].height);
                sp_TotalFramesCount.intValue = sp_Frames.arraySize;
                sp_LastFrameIndex.intValue = sp_Frames.arraySize - 1;
            }
            #endregion

            #region 序列帧列表
            Rect rect_transitionlist_group = new Rect(rect.x + 20, rect.y + 120, 245, 407);

            XGUI.gui_group_start(
               rect_group: rect_transitionlist_group,
               bg_fill: XGUIGroup.缺口纯色边框,
               bg_color: XGUIColor.亮白,
               bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
               bg_height: 0,
               false,
               false,
               margin: new RectOffset(0, 0, 0, 0),
               padding: new RectOffset(0, 0, 0, 0),
               title: "序列帧列表",
               //title_bg_fill: XGUIFilled.实体,
               //title_bg_color: XGUIColor.亮白,
               //title_bg_color_gui: Color.white,
               title_size: XGUIFontSize.M,
               title_anchor: TextAnchor.MiddleLeft,
               title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
               title_offset: new Vector2(0, 0),
               title_padding: new RectOffset(5, 2, 2, 2),
               title_font: null,
               title_font_style: FontStyle.Normal,
               title_clipping: TextClipping.Clip,
               //icon: seq_list_logo,
               icon: null,
               //icon_color: Color.white,
               icon_padding: new RectOffset(0, 0, 10, 10),
               foldout: false);
            XGUI.gui_group_end();

            DrawTransitionInfoList_Original(rect_transitionlist_group);

            #endregion

            #region 控制区
            XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                absolute_margin: true,
                absolute_padding: true,
                title: "控制区",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                title_manual_offset: true,
                title_manual_offset_space: 5,
                margin: new RectOffset(270, 15, 120, 0),
                padding: new RectOffset(10, 10, 15, 15));

            #region 控件区 - 视觉配置
            XGUI.layout_group_start(
               type: XGUIContainerType.Horizontal,
               //bg_fill: XGUIFilled.缺口纯色边框,
               //bg_color: XGUIColor.亮白,
               //bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
               absolute_margin: true,
               absolute_padding: true,
               //title: "SSS",
               //title_size: XGUIFontSize.M,
               //title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
               //title_clipping: TextClipping.Clip,
               //title_manual_offset: true,
               //title_manual_offset_space: 5,
               margin: new RectOffset(0, 0, 0, 0),
               padding: new RectOffset(0, 0, 0, 0));

            #region 参考图
            XGUI.layout_property_field(
              title: "参考图",
              title_size: XGUIFontSize.M,
              title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
              title_width: 50,
              field_width: 130,
              prop: sp_preview_texture_bg,
              prop_margin: new RectOffset(0, 0, 5, 10));
            #endregion

            #region 跳帧
            XGUI.layout_property_field(
                title: "跳帧",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 30,
                field_width: 35,
                prop: sp_SkipFrame,
                prop_margin: new RectOffset(0, 0, 5, 10));
            #endregion

            #region 平铺
            XGUI.layout_property_field(
                title: "平铺",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_width: 30,
                field_width: 35,
                prop: sp_bg_grid_tilling_multiply,
                prop_margin: new RectOffset(0, 0, 5, 10));
            #endregion

            #region 颜色 - 背景
            sp_preview_color.colorValue = XGUI.layout_colorfield(
                prop: sp_preview_color.colorValue,
                state_title: null,
                field_width: 50);
            sp_preview_color.serializedObject.ApplyModifiedProperties();
            #endregion

            #region 颜色 - 格子 - 前景
            sp_bg_grid_color_a.colorValue = XGUI.layout_colorfield(
                prop: sp_bg_grid_color_a.colorValue,
                state_title: null,
                field_width: 50);
            sp_bg_grid_color_a.serializedObject.ApplyModifiedProperties();
            #endregion

            #region 颜色 - 格子 - 背景
            sp_bg_grid_color_b.colorValue = XGUI.layout_colorfield(
                prop: sp_bg_grid_color_b.colorValue,
                state_title: null,
                field_width: 50);
            sp_bg_grid_color_b.serializedObject.ApplyModifiedProperties();
            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            #endregion

            XGUI.layout_space(316);

            #region 控件区 - 预览操作
            XGUI.layout_group_start(
               type: XGUIContainerType.Horizontal,
               //bg_fill: XGUIFilled.缺口纯色边框,
               //bg_color: XGUIColor.亮白,
               //bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
               absolute_margin: true,
               absolute_padding: true,
               //title: "SSS",
               //title_size: XGUIFontSize.M,
               //title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
               //title_clipping: TextClipping.Clip,
               //title_manual_offset: true,
               //title_manual_offset_space: 5,
               margin: new RectOffset(0, 0, 0, 0),
               padding: new RectOffset(0, 0, 0, 0));

            XGUI.ChangedCheck_Start();
            sp_preview_data_index.intValue = XGUI.layout_slider_int(
                title: "序列进度",
                prop: sp_preview_data_index,
                left: 0,
                right: transition_node.frames.Count - 1,
                title_width: 80,
                title_size: XGUIFontSize.M,
                title_anchor: TextAnchor.MiddleLeft,
                prop_margin: new RectOffset(0, 0, 0, 0));
            if (XGUI.ChangedCheck_End())
            {
                StopPreview();

                sp_selected_index.intValue = sp_preview_data_index.intValue;

                CalculateListScroller();
            }
            sp_preview_data_index.serializedObject.ApplyModifiedProperties();

            XGUI.layout_space(20);

            #region 播放控制
            if (!IsPreviewing && XGUI.layout_button(
                tooltip: "播放",
                tex_release: play_r,
                tex_press: play_p,
                tex_gui_color: Color.white,
                border: new RectOffset(0, 0, 0, 0),
                margin: new RectOffset(0, 0, 3, 0),
                width: 14,
                height: 14))
            {
                StartPreview();
                base_object.ApplyModifiedProperties();
                return;
            }
            if (IsPreviewing && XGUI.layout_button(
               tooltip: "停止",
               tex_release: stop_r,
               tex_press: stop_p,
               tex_gui_color: Color.white,
               border: new RectOffset(0, 0, 0, 0),
               margin: new RectOffset(0, 0, 3, 0),
               width: 14,
               height: 14))
            {
                StopPreview();
                preview_data_index = 0;
                sp_selected_index.intValue = 0;
                sp_preview_data_index.intValue = 0;
                base_object.ApplyModifiedProperties();
                return;
            }
            #endregion

            XGUI.layout_space(20);

            #region 播放模式
            string stopmode = "自动复位";
            if (!StoppedResetMode)
            {
                stopmode = "自动复位";
            }
            else
            {
                stopmode = "手动复位";
            }
            if (XGUI.layout_button(
               text: stopmode,
               tooltip: "",
               bg_fill: XGUIGroup.无,
               bg_color: XGUIColor.亮白,
               bg_color_gui: Color.white,
               button_text_color: Color.white,
               press_fill: XGUIGroup.透明,
               press_color: XGUIColor.无,
               press_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
               font_size: XGUIFontSize.B,
               anchor: TextAnchor.MiddleCenter,
               margin: new RectOffset(0, 0, 0, 0),
               padding: new RectOffset(0, 0, 0, 1),
               layout_min_width: 0,
               layout_width: 80,
               height: ButtonHeight,
               button_text_font: XGUI.GetFont("xg-medium")))
            {
                StoppedResetMode = !StoppedResetMode;
            }
            #endregion

            XGUI.layout_space(10);

            #region 转场方向
            string inverted = "正常";
            if (!InvertChannel)
            {
                inverted = "正常";
            }
            else
            {
                inverted = "反向";
            }
            if (XGUI.layout_button(
               text: inverted,
               tooltip: "",
               bg_fill: XGUIGroup.无,
               bg_color: XGUIColor.亮白,
               bg_color_gui: Color.white,
               button_text_color: Color.white,
               press_fill: XGUIGroup.透明,
               press_color: XGUIColor.无,
               press_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
               font_size: XGUIFontSize.B,
               anchor: TextAnchor.MiddleCenter,
               margin: new RectOffset(0, 0, 0, 0),
               padding: new RectOffset(0, 0, 0, 1),
               layout_min_width: 0,
               layout_width: 80,
               height: ButtonHeight,
               button_text_font: XGUI.GetFont("xg-medium")))
            {
                InvertChannel = !InvertChannel;
                StopPreview();
                preview_data_index = 0;
            }
            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 信息区
            XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                absolute_margin: true,
                absolute_padding: true,
                title: "转场序列帧信息",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                title_manual_offset: true,
                title_manual_offset_space: 5,
                margin: new RectOffset(20, 15, 20, 0),
                padding: new RectOffset(10, 10, 15, 15));

            XGUI.layout_group_start(
                type: XGUIContainerType.Horizontal,
                absolute_margin: true,
                absolute_padding: true,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0));

            #region 转场名称
            if (string.IsNullOrEmpty(sp_transition_name.stringValue))
            {
                sp_transition_name.stringValue = "转场名称";
            }

            sp_transition_name.stringValue = XGUI.layout_inputfield(
                title: "名称",
                prop: sp_transition_name.stringValue,
                text_wrap: false,
                field_fontsize: XGUIFontSize.M,
                field_text_offset: Vector2.zero,
                field_height: 20,
                field_padding: new RectOffset(0, 0, 0, 0),
                field_margin: new RectOffset(0, 0, 5, 0),
                field_text_color: Color.white,
                title_width: 55,
                field_width: 200,
                field_text_font: XGUI.GetFont("xg-medium"),
                field_text_style: FontStyle.Normal,
                field_text_anchor: TextAnchor.MiddleLeft);
            sp_transition_name.serializedObject.ApplyModifiedProperties();
            #endregion

            XGUI.layout_space(15);

            #region 转场说明
            if (string.IsNullOrEmpty(sp_transition_description.stringValue))
            {
                sp_transition_description.stringValue = "转场说明";
            }
            sp_transition_description.stringValue = XGUI.layout_inputfield(
              title: "说明",
              prop: sp_transition_description.stringValue,
              text_wrap: false,
              field_fontsize: XGUIFontSize.M,
              field_text_offset: Vector2.zero,
              field_height: 20,
              field_padding: new RectOffset(0, 0, 0, 0),
              field_margin: new RectOffset(0, 0, 5, 0),
              field_text_color: Color.white,
              title_width: 50,
              field_width: 400,
              field_text_font: XGUI.GetFont("xg-medium"),
              field_text_style: FontStyle.Normal,
              field_text_anchor: TextAnchor.MiddleLeft);
            sp_transition_description.serializedObject.ApplyModifiedProperties();
            #endregion

            XGUI.layout_space(15);

            #region 转场帧信息
            XGUI.layout_label(
                text: $"<color={colorhex}>{preview_data_index.ToString()}</color> / 帧",
                bg_fill: XGUIGroup.无,
                bg_color: XGUIColor.无,
                size: XGUIFontSize.L,
                anchor: TextAnchor.MiddleCenter,
                text_color: Color.white,
                offset: new Vector2(0, 0),
                padding: new RectOffset(10, 10, 0, 0),
                margin: new RectOffset(0, 0, 6, 0),
                clipping: TextClipping.Clip,
                font: XGUI.GetFont("xg-medium"),
                font_style: FontStyle.Bold);
            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 预览

            Rect rect_preview_group = new Rect(rect.width - 530, rect.y + 140, 510, 380);

            if (transition_node.frames != null && transition_node.frames.Count > 0)
            {
                float w = transition_node.frames[0].width;
                float h = transition_node.frames[0].height;

                Rect rect_preview = new Rect(rect.width - 530, rect.y + 140, 510, 298.75f);
                float margin = 10;

                #region 预览序列帧
                Rect rect_canvas = new Rect(rect_preview.x + margin, rect_preview.y + 20 + margin, rect_preview.width - 2 * margin, rect_preview.height - 2 * margin);

                #region 计算纹理的缩放比例
                float scaleX = rect_canvas.width / w;
                float scaleY = rect_canvas.height / h;

                // 选择合适的缩放比例，确保纹理等比缩放并且完全显示在目标矩形内
                float scale;
                if (w > h)
                {
                    scale = scaleX; // 宽度大于高度，以宽度为主
                }
                else
                {
                    scale = scaleY; // 高度大于宽度，以高度为主
                }

                // 计算纹理的绘制矩形
                float textureWidth = w * scale;
                float textureHeight = h * scale;

                // 计算纹理在目标矩形中的居中位置
                float x = rect_canvas.x + (rect_canvas.width - textureWidth) / 2;
                float y = rect_canvas.y + (rect_canvas.height - textureHeight) / 2;
                #endregion

                draw_rect.Set(x, y + 15, textureWidth, textureHeight);

                XGUI.gui_box(draw_rect, Color.black * 0.1f);

                Preview_Material.SetColor("_Color", sp_preview_color.colorValue);
                Preview_Material.SetFloat("_Alpha", sp_preview_color.colorValue.a);

                BgGrid_Tilling_x = sp_bg_grid_tilling_multiply.floatValue;
                BgGrid_Tilling_y = sp_bg_grid_tilling_multiply.floatValue;

                Preview_Material.SetFloat("_Grid_Tilling_x", BgGrid_Tilling_x * textureWidth);
                Preview_Material.SetFloat("_Grid_Tilling_y", BgGrid_Tilling_y * textureHeight);
                Preview_Material.SetColor("_Grid_Color_A", sp_bg_grid_color_a.colorValue);
                Preview_Material.SetColor("_Grid_Color_B", sp_bg_grid_color_b.colorValue);
                Preview_Material.SetTexture("_Texture", preview_texture_bg);
                Preview_Material.SetTexture("_Mask", transition_node.frames[preview_data_index]);
                Preview_Material.SetInt("_Invert", InvertChannel ? 1 : 0);

                #region 绘制预览图片
                GL.PushMatrix();
                {
                    // Correct coordinate system (正向Y轴-up)
                    GL.LoadPixelMatrix(0, position.width, position.height, 0);

                    Preview_Material.SetPass(0);
                    GL.Begin(GL.QUADS);
                    {
                        // Corrected vertex order for 正向Y轴-up
                        GL.TexCoord2(0, 1); GL.Vertex3(draw_rect.x, draw_rect.y, 0);
                        GL.TexCoord2(1, 1); GL.Vertex3(draw_rect.x + draw_rect.width, draw_rect.y, 0);
                        GL.TexCoord2(1, 0); GL.Vertex3(draw_rect.x + draw_rect.width, draw_rect.y + draw_rect.height, 0);
                        GL.TexCoord2(0, 0); GL.Vertex3(draw_rect.x, draw_rect.y + draw_rect.height, 0);
                    }
                    GL.End();
                }
                GL.PopMatrix();
                #endregion
                #endregion

                #region 帧显示
                draw_rect.Set(rect_preview.x + rect_preview.width - 90, rect_preview.y + rect_preview.height - 12, 70, 25);
                XGUI.gui_label(
                    rect: draw_rect,
                    bg_fill: XGUIGroup.实体,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    text: new GUIContent($"{preview_data_index} / {sp_TotalFramesCount.intValue - 1}"),
                    text_color: Color.black,
                    size: XGUIFontSize.M,
                    offset: new Vector2(0, -2),
                    clipping: clipping,
                    anchor: TextAnchor.MiddleCenter,
                    font_style: FontStyle.Bold);
                #endregion

                #region 左一帧
                draw_rect.Set(rect_preview.x + rect_preview.width - 90, rect_preview.y + (rect_preview.height - 40), 14, 14);
                if (XGUI.gui_button(
                    rect: draw_rect,
                    tooltip: "",
                    tex_release: leftarr_r,
                    tex_press: leftarr_p,
                    tex_gui_color: Color.white,
                    margin: new RectOffset(0, 0, 0, 0),
                    padding: new RectOffset(0, 0, 0, 0),
                    focus_name: "prev_btn"))
                {
                    FrameMoveBackward();
                }
                #endregion

                #region 右一帧
                draw_rect.Set(rect_preview.x + rect_preview.width - 35, rect_preview.y + (rect_preview.height - 40), 14, 14);
                if (XGUI.gui_button(
                    rect: draw_rect,
                    tooltip: "",
                    tex_release: rightarr_r,
                    tex_press: rightarr_p,
                    tex_gui_color: Color.white,
                    margin: new RectOffset(0, 0, 0, 0),
                    padding: new RectOffset(0, 0, 0, 0),
                    focus_name: "next_btn"))
                {
                    FrameMoveForward();
                }
                #endregion
            }
            else
            {
                draw_rect.Set(rect.width - 530, rect.y + 330, 510, 20);
                XGUI.gui_label(
                    rect: draw_rect,
                    text: new GUIContent("请先添加序列帧图像"),
                    text_color: Color.gray,
                    size: XGUIFontSize.B,
                    clipping: clipping,
                    anchor: TextAnchor.MiddleCenter,
                    font: XGUI.GetFont("xg-heavy"));
            }

            GUI.backgroundColor = Color.white;
            #endregion

            #region 拖放序列帧
            dragarea.Set(rect_transitionlist_group.x, rect_transitionlist_group.y + 10, rect_transitionlist_group.width, rect_transitionlist_group.height);

            Event e = Event.current;
            if (dragarea.Contains(e.mousePosition))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (e.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    Object[] dropobjs = DragAndDrop.objectReferences;
                    transition_node.frames.Clear();
                    foreach (var item in dropobjs)
                    {
                        if (item is Texture2D == false)
                            continue;

                        Texture2D tex = (Texture2D)item;

                        if (transition_node.frames == null)
                            transition_node.frames = new List<Texture2D>();

                        transition_node.frames.Add(tex);
                    }

                    e.Use();
                    preview_data_index = 0;
                    selected_index = 0;
                }
            }
            #endregion

            XGUI.layout_space(10);

            Buttons();

            #region 检测点击事件以及左右帧进
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
            if (e.type == EventType.KeyDown)
            {
                // 有输入控件被聚焦时，不响应快捷键
                if (!string.IsNullOrEmpty(GUI.GetNameOfFocusedControl()))
                    return;

                bool handled = false;

                // 列表区域：上下键
                if (scrollview_rect.Contains(e.mousePosition))
                {
                    if (e.keyCode == KeyCode.UpArrow)
                    {
                        StopPreview();
                        if (sp_selected_index.intValue <= 0)
                            sp_selected_index.intValue = 0;
                        else
                            sp_selected_index.intValue--;
                        preview_data_index = sp_selected_index.intValue;
                        CalculateListScroller();
                        handled = true;
                    }
                    else if (e.keyCode == KeyCode.DownArrow)
                    {
                        StopPreview();
                        if (sp_selected_index.intValue >= transition_node.frames.Count - 1)
                            sp_selected_index.intValue = transition_node.frames.Count - 1;
                        else
                            sp_selected_index.intValue++;
                        preview_data_index = sp_selected_index.intValue;
                        CalculateListScroller();
                        handled = true;
                    }
                }

                // 预览区域：< / > 步进
                if (rect_preview_group.Contains(e.mousePosition))
                {
                    if (e.keyCode == KeyCode.Period)   // >
                    {
                        FrameMoveForward();
                        handled = true;
                    }
                    else if (e.keyCode == KeyCode.Comma)   // <
                    {
                        FrameMoveBackward();
                        handled = true;
                    }
                }

                // 全局 Escape
                if (e.keyCode == KeyCode.Escape)
                {
                    Close();
                    handled = true;
                }

                if (handled)
                    e.Use();
            }
            #endregion

            Repaint();

            if (base_object.targetObject != null)
                base_object.ApplyModifiedProperties();
        }
        #endregion

        #region 列表绘制
        /// <summary>
        /// 绘制元素回调。
        /// <para/>
        /// 绘制内容：
        /// <list type="bullet">
        /// <item><description>序号（灰色，D2 格式）；</description></item>
        /// <item><description>纹理名称（主题色）；</description></item>
        /// <item><description>纹理缩略图。</description></item>
        /// </list>
        /// </summary>
        /// <param name="rect">绘制矩形</param>
        /// <param name="index">索引</param>
        /// <param name="isActive">是否激活</param>
        /// <param name="isFocused">是否聚焦</param>
        private void TransitionInfoList_Original_DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty prop = sp_transition_node;
            SerializedProperty sp_texlist = prop.FindPropertyRelative("frames");
            SerializedProperty sp_tex = sp_texlist.GetArrayElementAtIndex(index);

            Texture2D tex = sp_tex.objectReferenceValue as Texture2D;

#if UNITY_6000_0_OR_NEWER
            TextClipping clipping = TextClipping.Ellipsis;
#else
    TextClipping clipping = TextClipping.Clip;
#endif

            #region 序号
            drawelement_rect.Set(rect.x + 15, rect.y + 3, 30, 20);
            XGUI.gui_label(
                rect: drawelement_rect,
                text: new GUIContent(index.ToString("D2")),
                text_color: Color.gray,
                size: XGUIFontSize.S,
                clipping: clipping,
                anchor: TextAnchor.MiddleLeft,
                offset: new Vector2(0, 0),
                font_style: FontStyle.Normal);
            #endregion

            #region 名称
            drawelement_rect.Set(rect.x + 40, rect.y, rect.width - 80, rect.height);
            XGUI.gui_label(
             rect: drawelement_rect,
             text: new GUIContent(tex.name),
             text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
             size: XGUIFontSize.M,
             clipping: clipping,
             anchor: TextAnchor.MiddleLeft,
             offset: new Vector2(0, 0),
             font_style: FontStyle.Normal);
            #endregion

            #region 贴图
            drawelement_rect.Set(rect.width - 50, rect.y + rect.height * (0.3f / 2), rect.height * 0.7f, rect.height * 0.7f);
            XGUI.gui_icon(
                 rect: drawelement_rect,
                 icon: tex,
                 color: Color.white);
            #endregion
        }
        /// <summary>
        /// 绘制序列帧列表。
        /// <para/>
        /// 只绘制滚动视图可视区域内的元素，以优化大列表性能。
        /// 单击列表项切换帧，键盘上下键切换帧。
        /// </summary>
        /// <param name="rect">绘制区域矩形</param>
        private void DrawTransitionInfoList_Original(Rect rect)
        {
            // 绘制滚动视图
            scrollview_rect.Set(rect.x + 10, rect.y + 18, rect.width - 20, sp_visible_item_count.intValue * sp_item_height.floatValue);
            sp_transition_info_list_scroller.vector2Value = GUI.BeginScrollView(scrollview_rect, sp_transition_info_list_scroller.vector2Value, new Rect(0, 0, scrollview_rect.width - 50, TrasitionInfoList.count * sp_item_height.floatValue), false, true);

            // 计算可视区域的起始和结束索引
            int startIndex = Mathf.FloorToInt(sp_transition_info_list_scroller.vector2Value.y / sp_item_height.floatValue);
            int endIndex = Mathf.CeilToInt((sp_transition_info_list_scroller.vector2Value.y + scrollview_rect.height) / sp_item_height.floatValue);

            // 只绘制可视区域内的元素
            for (int i = startIndex; i < endIndex && i < TrasitionInfoList.count; i++)
            {
                SerializedProperty sp_texlist = sp_transition_node.FindPropertyRelative("frames");
                SerializedProperty sp_tex = sp_texlist.GetArrayElementAtIndex(i);

                item_rect.Set(0, i * sp_item_height.floatValue, scrollview_rect.width, sp_item_height.floatValue);

                if (sp_selected_index.intValue == i)
                {
                    // 高亮标记表示选中
                    item_rect.Set(1, i * sp_item_height.floatValue + 10, 5, 5);
                    EditorGUI.DrawRect(item_rect, XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                    // 高亮背景表示选中
                    item_rect.Set(0, i * sp_item_height.floatValue, scrollview_rect.width, sp_item_height.floatValue);
                    EditorGUI.DrawRect(item_rect, SelectedBg);
                }

                item_rect.Set(0, i * sp_item_height.floatValue, scrollview_rect.width, sp_item_height.floatValue);

                TrasitionInfoList.drawElementCallback.Invoke(item_rect, i, i == TrasitionInfoList.index, true);

                // 检测鼠标是否在当前元素区域内
                Event e = Event.current;
                if (item_rect.Contains(e.mousePosition))
                {
                    #region 切换序列帧索引
                    if (e.type == EventType.KeyDown)
                    {
                        if (e.keyCode == KeyCode.UpArrow)
                        {
                            StopPreview();
                            IsPreviewing = false;

                            if (sp_selected_index.intValue <= 0)
                            {
                                sp_selected_index.intValue = 0;
                            }
                            else

                            {
                                sp_selected_index.intValue--;
                            }
                            preview_data_index = sp_selected_index.intValue;
                            CalculateListScroller();
                        }
                        if (e.keyCode == KeyCode.DownArrow)
                        {
                            StopPreview();
                            IsPreviewing = false;
                            if (sp_selected_index.intValue >= transition_node.frames.Count - 1)
                            {
                                sp_selected_index.intValue = transition_node.frames.Count - 1;
                            }
                            else

                            {
                                sp_selected_index.intValue++;
                            }
                            preview_data_index = sp_selected_index.intValue;
                            CalculateListScroller();
                        }
                        e.Use();
                    }
                    #endregion

                    #region 选中序列帧索引
                    if (e.type == EventType.MouseDown)
                    {
                        // 更新选中项
                        sp_selected_index.intValue = i;

                        preview_data_index = sp_selected_index.intValue;

                        // 标记界面需要更新
                        GUI.changed = true;
                        e.Use();
                    }
                    #endregion
                }

                // 绘制元素
                //EditorGUI.PropertyValue(item_rect, sp_tex, GUIContent.none);
            }

            GUI.EndScrollView();
        }
        #endregion

        #region 帧操作
        private void FrameMoveForward()
        {
            StopPreview();

            if (transition_node.frames == null || transition_node.frames.Count == 0)
                return;

            int lastIndex = transition_node.frames.Count - 1;
            int skip = Mathf.Max(1, transition_node.skip_frame);

            // 前进 skip 帧
            int newIndex = sp_preview_data_index.intValue + skip;

            // 到末尾就跳回 0
            if (newIndex > lastIndex)
                newIndex = 0;

            sp_preview_data_index.intValue = newIndex;
            sp_selected_index.intValue = newIndex;
            preview_data_index = newIndex;

            CalculateListScroller();
            Repaint();
        }

        private void FrameMoveBackward()
        {
            StopPreview();

            if (transition_node.frames == null || transition_node.frames.Count == 0)
                return;

            int lastIndex = transition_node.frames.Count - 1;
            int skip = Mathf.Max(1, transition_node.skip_frame);

            // 后退 skip 帧
            int newIndex = sp_preview_data_index.intValue - skip;

            // 到 0 以下就跳到末尾
            if (newIndex < 0)
                newIndex = lastIndex;

            sp_preview_data_index.intValue = newIndex;
            sp_selected_index.intValue = newIndex;
            preview_data_index = newIndex;

            CalculateListScroller();
            Repaint();
        }
        #endregion

        #region 工具 - 数据操作
        /// <summary>
        /// 更新到目标转场库。
        /// <para/>
        /// 用当前编辑的名称、说明与转场数据替换目标库中指定索引的转场项，
        /// 然后弹窗提示并关闭窗口。
        /// </summary>
        private void UpdateToLibrary()
        {
            string colorhex = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.themes_get_color(ThemeColorType.primary), true);

            XHud_LibraryArg_Transition info = new XHud_LibraryArg_Transition();
            info.CopyData(transition_node);
            info.name = sp_transition_name.stringValue;
            info.description = sp_transition_description.stringValue;
            Target_Hud_TransitionLibrary.TransitionsLibrary_Replace(ModifiedIndex, info);

            XGUI.dialog(
                type: XGUIDialogType.确认,
                windowtitle: "XHud - 转场资源修改器消息",
                title: "更新完成",
                msg: $"转场资源已更新完成！",
                ok: "明白",
                PrimaryIndex: 0,
                usemodal: true,
                themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));

            Close();
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
            ButtonText_Cancel = cancel;
            ButtonText_Ok = ok;
        }
        /// <summary>
        /// 设置模版基础信息（针对从库源修改更新使用）。
        /// </summary>
        /// <param name="name">转场名称</param>
        /// <param name="decription">转场说明</param>
        public void SetInfo(string name, string decription)
        {
            sp_transition_name.stringValue = name;
            sp_transition_description.stringValue = decription;

            sp_transition_name.serializedObject.ApplyModifiedProperties();
            sp_transition_description.serializedObject.ApplyModifiedProperties();

            preview_data_index = 0;
        }
        /// <summary>
        /// 设置标题。
        /// </summary>
        /// <param name="title">标题文字</param>
        public void SetTitle(string title)
        {
            Title = title;
        }
        /// <summary>
        /// 设置转场节点（工作副本）。
        /// </summary>
        /// <param name="node">转场数据</param>
        public void Set_TransitionNode(XHud_LibraryArg_Transition node)
        {
            transition_node.CopyData(node);
        }
        /// <summary>
        /// 设置目标转场库。
        /// </summary>
        /// <param name="lib">目标转场库</param>
        public void Set_Target_Hud_TransitionLibrary(XHud_Library_Transition lib)
        {
            Target_Hud_TransitionLibrary = lib;
        }
        /// <summary>
        /// 设置转场节点（原始副本）。
        /// </summary>
        /// <param name="node">转场数据</param>
        public void Set_OriginTransitionNode(XHud_LibraryArg_Transition node)
        {
            OriginTransitionNode.CopyData(node);
        }
        #endregion

        #region 按钮
        /// <summary>
        /// 绘制底部按钮区（取消 / 更新）。
        /// <para/>
        /// 点击"更新"调用 <see cref="UpdateToLibrary"/>。
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
                height: ButtonHeight,
                button_text_font: XGUI.GetFont("xg-medium")))
            {
                Close();
            }

            XGUI.layout_space(ButtonDistance);

            if (XGUI.layout_button(
                text: ButtonText_Ok,
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
                height: ButtonHeight,
                button_text_font: XGUI.GetFont("xg-medium")))
            {
                UpdateToLibrary();
                return;
            }

            XGUI.layout_flexspace();

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            #endregion
        }
        #endregion

        #region 工具 - 预览动画
        private void StartPreview()
        {
            if (transition_node.frames == null || transition_node.frames.Count == 0)
                return;

            int skip = Mathf.Max(1, transition_node.skip_frame);
            int lastIndex = transition_node.frames.Count - 1;

            // 从当前帧开始
            int startIndex = Mathf.Clamp(preview_data_index, 0, lastIndex);

            // 剩余帧数（按 skip 步进）
            int remainingSteps = 0;
            for (int i = startIndex; i <= lastIndex; i += skip)
                remainingSteps++;

            if (remainingSteps <= 0)
                remainingSteps = 1;

            var dummyFrames = new List<Texture2D>(remainingSteps);
            for (int i = 0; i < remainingSteps; i++) dummyFrames.Add(null);

            IsPreviewing = true;

            XGUI.EditorPlayer_Sequence.Play(
                frames: dummyFrames,
                interval: PreviewDataDuration,
                onFrame: () =>
                {
                    int step = XGUI.EditorPlayer_Sequence.CurrentIndex;
                    preview_data_index = Mathf.Min(startIndex + step * skip, lastIndex);
                    sp_selected_index.intValue = preview_data_index;

                    if (StoppedResetMode && step >= remainingSteps - 1)
                        IsPreviewing = false;

                    Repaint();
                },
                loopCount: StoppedResetMode ? 0 : -1);
        }

        private void StopPreview()
        {
            XGUI.EditorPlayer_Sequence.Stop();
            IsPreviewing = false;
        }
        /// <summary>
        /// 初始化预览转场材质。
        /// <para/>
        /// 通过 <see cref="Shader.Find(string)"/> 加载 <c>XHud/EditorTransition</c> 并构造材质实例。
        /// </summary>
        private void InitializePreviewMaterial()
        {
            Shader shader = Shader.Find("XHud/EditorTransition");
            Preview_Material = new Material(shader);
        }
        /// <summary>
        /// 初始化预览背景图。
        /// <para/>
        /// 从 xData 中读取上次保存的背景图路径，若资源无效则使用默认背景。
        /// </summary>
        private void LoadPreviewTexture()
        {
            string path = XGUI.x_Editor_Data_Get_With_String("xData_Transition_Bg_Path");
            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (tex == null)
                sp_preview_texture_bg.objectReferenceValue = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_transition_setter/defaultbg");
            else
                sp_preview_texture_bg.objectReferenceValue = tex;
        }
        #endregion

        #region 工具 - 列表滚动
        /// <summary>
        /// 根据当前选中索引计算列表滚动位置。
        /// <para/>
        /// 若选中项超过可视区域，则滚动到接近选中项的位置，否则回到顶部。
        /// </summary>
        private void CalculateListScroller()
        {
            if (selected_index >= visible_item_count)
            {
                sp_transition_info_list_scroller.vector2Value = new Vector2(sp_transition_info_list_scroller.vector2Value.x, (selected_index) * item_height);
            }
            else
                sp_transition_info_list_scroller.vector2Value = Vector2.zero;
        }
        #endregion
    }
}