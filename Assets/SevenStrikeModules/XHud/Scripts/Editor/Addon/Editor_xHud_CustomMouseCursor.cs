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
    using UnityEditorInternal;
    using UnityEngine;
    using Image = UnityEngine.UI.Image;

    [CustomEditor(typeof(XHud_CustomMouseCursor), true)]
    public class Editor_XHud_CustomMouseCursor : Editor
    {
        #region 字段 - 组件 / 列表
        /// <summary>
        /// 当前编辑的目标脚本实例。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>在 <c>OnEnable</c> 中由 <c>target</c> 强制转换获得。</description></item>
        /// <item><description>仅用于单选模式下的直接读写操作。</description></item>
        /// </list>
        /// </summary>
        private XHud_CustomMouseCursor base_script;
        /// <summary>
        /// 鼠标样式列表（可重排列表）。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>在 <c>OnEnable</c> 中构造。</description></item>
        /// <item><description>所有回调均使用具名方法。</description></item>
        /// </list>
        /// </summary>
        private ReorderableList stylelist;
        #endregion

        #region 字段 - 序列化属性
        /// <summary>
        /// 光标焦点物体属性
        /// </summary>
        private SerializedProperty sp_cursor_rect;
        /// <summary>
        /// 光标透明组件属性
        /// </summary>
        private SerializedProperty sp_cursor_canvas_group;
        /// <summary>
        /// 光标图像组件属性
        /// </summary>
        private SerializedProperty sp_cursor_imager;
        /// <summary>
        /// 自定义光标开关属性
        /// </summary>
        private SerializedProperty sp_use_custom_cursor;
        /// <summary>
        /// 鼠标样式列表属性
        /// </summary>
        private SerializedProperty sp_mouse_styles;
        /// <summary>
        /// 光标尺寸缓动时长属性
        /// </summary>
        private SerializedProperty sp_cursor_size_tween_duration;
        /// <summary>
        /// 光标尺寸缓动曲线属性
        /// </summary>
        private SerializedProperty sp_cursor_size_tween_ease;
        /// <summary>
        /// 光标尺寸差值速率属性
        /// </summary>
        private SerializedProperty sp_cursor_imager_size_smooth;
        /// <summary>
        /// 光标透明度缓动时长属性
        /// </summary>
        private SerializedProperty sp_cursor_opacity_tween_duration;
        /// <summary>
        /// 光标透明度缓动曲线属性
        /// </summary>
        private SerializedProperty sp_cursor_opacity_tween_ease;
        /// <summary>
        /// 光标尺寸属性
        /// </summary>
        private SerializedProperty sp_cursor_size;
        /// <summary>
        /// 光标颜色属性
        /// </summary>
        private SerializedProperty sp_cursor_color;
        /// <summary>
        /// 光标颜色平滑值属性
        /// </summary>
        private SerializedProperty sp_cursor_color_smooth;
        /// <summary>
        /// 光标透明度属性
        /// </summary>
        private SerializedProperty sp_cursor_opacity;
        /// <summary>
        /// 鼠标响应类型属性
        /// </summary>
        private SerializedProperty sp_index_mouse_type;
        /// <summary>
        /// 尺寸平滑方式属性
        /// </summary>
        private SerializedProperty sp_use_lerp_cursor_size;
        /// <summary>
        /// 当前光标样式属性
        /// </summary>
        private SerializedProperty sp_current_cursor_style;
        #endregion

        #region 字段 - 图标
        /// <summary>
        /// 标题主图标
        /// </summary>
        private Texture2D icon_main;
        /// <summary>
        /// 无效光标占位图标
        /// </summary>
        private Texture2D icon_invalid_tex;
        #endregion

        #region 字段 - GUI 参数
        /// <summary>
        /// 单行行高缓存。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>在 <c>OnEnable</c> 中由 <c>XGUI.GetSingleLineHeight()</c> 获取。</description></item>
        /// <item><description>用于计算列表元素内控件布局。</description></item>
        /// </list>
        /// </summary>
        private float lineheight;
        #endregion

        #region 字段 - 批量化操作
        /// <summary>
        /// 批量选择脚本数组。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>在 <c>Targets_Get</c> 中根据 <c>targets</c> 填充。</description></item>
        /// <item><description>单选时长度为 1，多选时长度等于选中对象数量。</description></item>
        /// </list>
        /// </summary>
        private XHud_CustomMouseCursor[] selected_objects;
        #endregion

        #region 生命周期
        void OnEnable()
        {
            base_script = (XHud_CustomMouseCursor)target;

            #region 获取序列化属性
            sp_cursor_rect = serializedObject.FindProperty(nameof(XHud_CustomMouseCursor.cursor_rect));
            sp_cursor_canvas_group = serializedObject.FindProperty(nameof(XHud_CustomMouseCursor.cursor_canvas_group));
            sp_use_custom_cursor = serializedObject.FindProperty(nameof(XHud_CustomMouseCursor.use_custom_cursor));
            sp_cursor_opacity = serializedObject.FindProperty(nameof(XHud_CustomMouseCursor.cursor_opacity));
            sp_cursor_imager = serializedObject.FindProperty(nameof(XHud_CustomMouseCursor.cursor_imager));
            sp_mouse_styles = serializedObject.FindProperty(nameof(XHud_CustomMouseCursor.mouse_styles));
            sp_cursor_size_tween_duration = serializedObject.FindProperty(nameof(XHud_CustomMouseCursor.cursor_size_tween_duration));
            sp_cursor_size_tween_ease = serializedObject.FindProperty(nameof(XHud_CustomMouseCursor.cursor_size_tween_ease));
            sp_cursor_imager_size_smooth = serializedObject.FindProperty(nameof(XHud_CustomMouseCursor.cursor_imager_size_smooth));
            sp_cursor_opacity_tween_duration = serializedObject.FindProperty(nameof(XHud_CustomMouseCursor.cursor_opacity_tween_duration));
            sp_cursor_opacity_tween_ease = serializedObject.FindProperty(nameof(XHud_CustomMouseCursor.cursor_opacity_tween_ease));
            sp_cursor_size = serializedObject.FindProperty(nameof(XHud_CustomMouseCursor.cursor_size));
            sp_cursor_color = serializedObject.FindProperty(nameof(XHud_CustomMouseCursor.cursor_color));
            sp_cursor_color_smooth = serializedObject.FindProperty(nameof(XHud_CustomMouseCursor.cursor_color_smooth));
            sp_current_cursor_style = serializedObject.FindProperty(nameof(XHud_CustomMouseCursor.current_cursor_style));
            sp_index_mouse_type = serializedObject.FindProperty(nameof(XHud_CustomMouseCursor.index_mouse_type));
            sp_use_lerp_cursor_size = serializedObject.FindProperty(nameof(XHud_CustomMouseCursor.use_lerp_cursor_size));
            #endregion

            #region 获取图标
            icon_main = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_mouse_cursor/icon_main");
            icon_invalid_tex = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_mouse_cursor/icon_invalid");
            #endregion

            base_script.cursor_rect = base_script.GetComponent<RectTransform>();
            base_script.cursor_canvas_group = base_script.GetComponent<CanvasGroup>();

            if (base_script.transform.childCount <= 0)
            {
                GameObject imager = new GameObject();
                imager.transform.SetParent(base_script.transform);
                imager.transform.localPosition = Vector3.zero;
                imager.transform.localEulerAngles = Vector3.zero;
                imager.transform.localScale = Vector3.one;
                imager.name = "Imager";
                imager.layer = LayerMask.NameToLayer("XHud");
                RectTransform rect = imager.AddComponent<RectTransform>();
                rect.sizeDelta = new Vector2(10, 10);
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                Image img = imager.AddComponent<Image>();
                img.raycastTarget = false;
                base_script.cursor_imager = img;
            }
            else
            {
                base_script.cursor_imager = base_script.transform.GetChild(0).GetComponent<Image>();
            }

            lineheight = XGUI.GetSingleLineHeight();

            Targets_Get();

            #region 列表 - 鼠标样式表
            stylelist = new ReorderableList(serializedObject, sp_mouse_styles)
            {
                displayAdd = true,
                displayRemove = true,
                draggable = true,
                drawHeaderCallback = MouseStyles_DrawHeader,
                drawElementBackgroundCallback = MouseStyles_DrawElementBackground,
                drawElementCallback = MouseStyles_DrawElement,
                onAddCallback = MouseStyles_OnAdd,
                onRemoveCallback = MouseStyles_OnRemove,
                onSelectCallback = MouseStyles_OnSelect,
                elementHeightCallback = MouseStyles_ElementHeight
            };
            #endregion
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            TextClipping clipping_ellipsis = XGUI.TryEllipsisClipping();

            #region 标题
            XGUI.layout_banner(
             bg_fill: XGUIGroup.实体,
             bg_color: XGUIColor.深空灰,
             bg_height: 30,
             icon: icon_main,
             icon_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
             title_text: "XHud  -  鼠标样式",
             title_anchor: TextAnchor.MiddleLeft,
             title_style: FontStyle.Normal,
             title_color: Color.white,
             title_size: XGUIFontSize.B,
             title_clipping: clipping_ellipsis,
             bg_margin: new RectOffset(0, 0, 5, 5));
            #endregion

            XGUI.layout_space(5);

            #region 当前光标预览
            XGUI.layout_group_start(
               type: XGUIContainerType.Vertical,
               bg_fill: XGUIGroup.缺口纯色边框,
               bg_color: XGUIColor.亮白,
               bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
               title: "当前光标预览",
               title_size: XGUIFontSize.M,
               title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
               title_clipping: TextClipping.Clip,
               padding: new RectOffset(10, 10, 15, 15));
            if (!Application.isPlaying)
            {
                CanvasGroup cg = (CanvasGroup)sp_cursor_canvas_group.objectReferenceValue;
                Image imgsursor = (Image)sp_cursor_imager.objectReferenceValue;
                if (cg != null)
                    cg.alpha = sp_cursor_opacity.floatValue;
                sp_cursor_canvas_group.serializedObject.ApplyModifiedProperties();
                if (imgsursor != null)
                {
                    imgsursor.rectTransform.sizeDelta = Vector2.one * sp_cursor_size.floatValue;
                }
                sp_cursor_imager.serializedObject.ApplyModifiedProperties();
            }
            if (sp_use_custom_cursor.boolValue)
            {
                SerializedProperty index_sprite = sp_current_cursor_style.FindPropertyRelative(nameof(MouseStyles.Sprite));
                Sprite spr = (Sprite)index_sprite.objectReferenceValue;
                #region 当前选择的光标图标
                XGUI.layout_icon(
                    icon: spr != null && spr.texture != null ? spr.texture : icon_invalid_tex,
                    icon_color: sp_cursor_color.colorValue,
                    width: 15,
                    height: 15,
                    icon_alignment: XGUIIconAlignment.中心,
                    layout_margin: new RectOffset(0, 0, 15, 15));
                #endregion
            }
            #region 光标位置
            if (Application.isPlaying)
            {
                Vector3 mpos = Input.mousePosition;
                XGUI.layout_group_start(
                    type: XGUIContainerType.Horizontal,
                    padding: new RectOffset(10, 10, 0, 0));
                XGUI.layout_label(
                    text: "光标  X轴： " + mpos.x,
                    bg_fill: XGUIGroup.无,
                    bg_color: XGUIColor.无,
                    size: XGUIFontSize.S,
                    anchor: TextAnchor.MiddleLeft,
                    text_color: Color.white * 0.7f,
                    offset: new Vector2(0, 0),
                    padding: new RectOffset(15, 10, 0, 0),
                    margin: new RectOffset(0, 0, 5, 0),
                    clipping: TextClipping.Clip,
                    font: XGUI.GetFont("xg-bold"),
                    font_style: FontStyle.Normal);
                XGUI.layout_label(
                    text: "光标  Y轴： " + mpos.y,
                    bg_fill: XGUIGroup.无,
                    bg_color: XGUIColor.无,
                    size: XGUIFontSize.S,
                    anchor: TextAnchor.MiddleRight,
                    text_color: Color.white * 0.7f,
                    offset: new Vector2(0, 0),
                    padding: new RectOffset(15, 10, 0, 0),
                    margin: new RectOffset(0, 0, 5, 0),
                    clipping: TextClipping.Clip,
                    font: XGUI.GetFont("xg-bold"),
                    font_style: FontStyle.Normal);
                XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            }
            #endregion
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 选项         
            base_script.fold_options = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "选项",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: base_script.fold_options);
            if (!base_script.fold_options)
            {
                DrawToggle("自定义光标", sp_use_custom_cursor, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, (b) => { });
                string[] actionlist = System.Enum.GetNames(typeof(MouseType));
                XGUI.layout_string_popup(
                    title: "鼠标响应",
                    title_width: 100,
                    title_size: XGUIFontSize.M,
                    title_anchor: TextAnchor.MiddleLeft,
                    prop: sp_index_mouse_type,
                    options: actionlist,
                    opt_text_size: XGUIFontSize.M,
                    opt_text_color: Color.black,
                    opt_text_padding: new RectOffset(10, 10, 0, 0),
                    opt_anchor: TextAnchor.MiddleLeft,
                    opt_font_style: FontStyle.Normal,
                    opt_bg_fill: XGUIGroup.实体,
                    opt_bg_color: XGUIColor.亮白,
                    opt_bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    margin: new RectOffset(0, 0, 5, 5),
                    padding: new RectOffset(5, 5, 0, 0),
                    title_margin: new RectOffset(0, 0, 0, 0),
                    icon_arrow_color: Color.black,
                    act_on_changed: (d) =>
                    {
                        base_script.mouse_click_type = (MouseType)System.Enum.Parse(typeof(MouseType), d);
                    });
                string[] smoothsizelist = new string[2] { "缓动模式", "差值模式" };
                XGUI.layout_string_popup(
                    title: "光标尺寸平滑方式",
                    title_width: 100,
                    title_size: XGUIFontSize.M,
                    title_anchor: TextAnchor.MiddleLeft,
                    prop: sp_use_lerp_cursor_size,
                    options: smoothsizelist,
                    opt_text_size: XGUIFontSize.M,
                    opt_text_color: Color.black,
                    opt_text_padding: new RectOffset(10, 10, 0, 0),
                    opt_anchor: TextAnchor.MiddleLeft,
                    opt_font_style: FontStyle.Normal,
                    opt_bg_fill: XGUIGroup.实体,
                    opt_bg_color: XGUIColor.亮白,
                    opt_bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    margin: new RectOffset(0, 0, 5, 5),
                    padding: new RectOffset(5, 5, 0, 0),
                    title_margin: new RectOffset(0, 0, 0, 0),
                    icon_arrow_color: Color.black);
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 参数
            base_script.fold_params = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "参数",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(5, 10, 15, 15),
                foldout: base_script.fold_params);
            Image img_cursor = (Image)sp_cursor_imager.objectReferenceValue;
            if (sp_use_custom_cursor.boolValue)
            {
                if (!base_script.fold_params)
                {
                    #region 焦点物体
                    XGUI.layout_property_field(
                        title: "焦点物体",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: sp_cursor_rect,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion
                    #region 透明组件
                    XGUI.layout_property_field(
                        title: "透明组件",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: sp_cursor_canvas_group,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion
                    #region 光标组件
                    XGUI.layout_property_field(
                        title: "光标组件",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: sp_cursor_imager,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion
                    #region 尺寸
                    XGUI.layout_property_field(
                        title: "尺寸",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: sp_cursor_size,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion
                    #region 颜色
                    EditorGUI.BeginChangeCheck();
                    XGUI.layout_property_field(
                        title: "颜色",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: sp_cursor_color,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    if (EditorGUI.EndChangeCheck())
                    {
                        base_script.cursor_imager.color = sp_cursor_color.colorValue;
                    }
                    #endregion
                    #region 颜色平滑系数
                    XGUI.layout_property_field(
                        title: "颜色平滑系数",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: sp_cursor_color_smooth,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion
                    #region 透明度
                    XGUI.layout_property_field(
                        title: "透明度",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: sp_cursor_opacity,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion
                    #region 透明度缓动方式
                    XGUI.layout_property_field(
                        title: "透明度缓动方式",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: sp_cursor_opacity_tween_ease,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion
                    #region 透明度变化耗时
                    XGUI.layout_property_field(
                        title: "透明度变化耗时",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: sp_cursor_opacity_tween_duration,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion
                    if (sp_use_lerp_cursor_size.stringValue == "缓动模式")
                    {
                        #region 光标尺寸缓动耗时
                        XGUI.layout_property_field(
                            title: "光标尺寸缓动耗时",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                            title_width: 90,
                            prop: sp_cursor_size_tween_duration,
                            prop_margin: new RectOffset(0, 0, 5, 0));
                        #endregion
                        #region 光标尺寸缓动方式
                        XGUI.layout_property_field(
                            title: "光标尺寸缓动方式",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                            title_width: 90,
                            prop: sp_cursor_size_tween_ease,
                            prop_margin: new RectOffset(0, 0, 5, 0));
                        #endregion
                    }
                    else
                    {
                        #region 光标尺寸差值速率
                        XGUI.layout_property_field(
                            title: "光标尺寸差值速率",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                            title_width: 90,
                            prop: sp_cursor_imager_size_smooth,
                            prop_margin: new RectOffset(0, 0, 5, 0));
                        #endregion
                    }
                }
            }
            else
            {
                if (img_cursor != null)
                    img_cursor.enabled = false;
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 光标样式表
            base_script.fold_styles = XGUI.layout_group_start(
              type: XGUIContainerType.Vertical,
              bg_fill: XGUIGroup.缺口纯色边框,
              bg_color: XGUIColor.亮白,
              bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
              title: "光标样式表",
              title_size: XGUIFontSize.M,
              title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
              title_clipping: TextClipping.Clip,
              padding: new RectOffset(10, 10, 15, 15),
              foldout: base_script.fold_styles);
            if (!base_script.fold_styles)
            {
                stylelist.DoLayoutList();
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 弹出菜单
            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                // 创建右键菜单
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Q (折叠编组)"), false, () =>
                {
                    base_script.GroupFold(true);
                });
                menu.AddItem(new GUIContent("W (展开编组)"), false, () =>
                {
                    base_script.GroupFold(false);
                });
                menu.ShowAsContext();

                Event.current.Use();
            }
            #endregion

            #region 源脚本
            base_script.fold_based = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "源脚本",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: base_script.fold_based);
            if (!base_script.fold_based)
            {
                DrawDefaultInspector();
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            serializedObject.ApplyModifiedProperties();
        }
        #endregion

        #region 工具 - 批量化操作
        /// <summary>
        /// 获取所有批量脚本目标。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>多选时创建与 <c>targets</c> 等长的数组并逐个填充。</description></item>
        /// <item><description>单选时创建长度为 1 的数组并填充当前目标。</description></item>
        /// </list>
        /// </summary>
        private void Targets_Get()
        {
            if (targets.Length > 1)
            {
                selected_objects = new XHud_CustomMouseCursor[targets.Length];
                for (int i = 0; i < selected_objects.Length; i++)
                {
                    var t = targets[i];
                    selected_objects[i] = (XHud_CustomMouseCursor)t;
                }
            }
            else
            {
                selected_objects = new XHud_CustomMouseCursor[targets.Length];
                selected_objects[0] = (XHud_CustomMouseCursor)target;
            }
        }
        /// <summary>
        /// 判断是否是多选状态。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>当 <c>SelectedObjects</c> 为 null 时返回 false。</description></item>
        /// <item><description>当 <c>SelectedObjects.Length > 1</c> 时返回 true。</description></item>
        /// </list>
        /// </summary>
        /// <returns>是否为多选状态。</returns>
        private bool Targets_Selected()
        {
            if (selected_objects == null)
                return false;
            if (selected_objects.Length > 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 绘制 - 鼠标样式表
        /// <summary>
        /// 绘制鼠标样式列表头。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>在列表顶部绘制标题文字。</description></item>
        /// </list>
        /// </summary>
        private void MouseStyles_DrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "鼠标样式表");
        }
        /// <summary>
        /// 绘制鼠标样式列表元素背景。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>当元素处于焦点状态时，绘制左侧主题色标记条。</description></item>
        /// </list>
        /// </summary>
        private void MouseStyles_DrawElementBackground(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (isFocused)
                XGUI.gui_box(new Rect(rect.x + 22, rect.y + 7, 2, 10), XHud_Dashboard.themes_get_color(ThemeColorType.primary));
        }
        /// <summary>
        /// 绘制鼠标样式列表元素内容。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>绘制标识输入框。</description></item>
        /// <item><description>绘制光标贴图字段。</description></item>
        /// <item><description>绘制位置相对偏移字段。</description></item>
        /// <item><description>预览光标贴图。</description></item>
        /// </list>
        /// </summary>
        private void MouseStyles_DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            float titleheight = rect.y + 2;
            float baseheight = rect.y + (rect.height - 25);
            float current_width = XGUI.GetCurrentWindowWidth();

            SerializedProperty sp_name = sp_mouse_styles.GetArrayElementAtIndex(index).FindPropertyRelative(nameof(MouseStyles.Name));
            SerializedProperty index_sprite = sp_mouse_styles.GetArrayElementAtIndex(index).FindPropertyRelative(nameof(MouseStyles.Sprite));
            Sprite spr = (Sprite)index_sprite.objectReferenceValue;
            SerializedProperty pivot = sp_mouse_styles.GetArrayElementAtIndex(index).FindPropertyRelative(nameof(MouseStyles.Pivot));

            #region 光标名称
            if (current_width > 130)
            {
                Rect rect_name = new Rect(rect.x + 12, titleheight, rect.width - 16, lineheight);
                sp_name.stringValue = XGUI.gui_inputfield(
                    rect: rect_name,
                    title: "标识",
                    prop: sp_name.stringValue,
                    text_wrap: false,
                    field_fontsize: XGUIFontSize.M,
                    field_text_offset: Vector2.zero,
                    field_height: 20,
                    field_text_color: Color.white,
                    title_width: 25,
                    //status_icon: "icon_field_status",
                    //status_icon_color: Color.green,
                    field_text_font: XGUI.GetFont("xg-medium"),
                    field_text_style: FontStyle.Normal,
                    field_text_anchor: TextAnchor.MiddleLeft,
                    field_padding: new RectOffset(5, 5, 0, 0),
                    field_margin: new RectOffset(0, 0, 0, 0));
                sp_name.serializedObject.ApplyModifiedProperties();
            }
            #endregion

            #region 光标贴图
            if (current_width > 130)
            {
                Rect rect_sprite = new Rect(rect.x + 47, titleheight + 30, rect.width - 50, lineheight);
                XGUI.gui_property_field(
                    rect: rect_sprite,
                    title: null,
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 80,
                    prop: index_sprite);
                index_sprite.serializedObject.ApplyModifiedProperties();
            }
            #endregion

            #region 位置相对偏移
            if (current_width > 130)
            {
                Rect rect_offset = new Rect(rect.x + 47, titleheight + 55, rect.width - 50, lineheight);
                pivot.vector2Value = XGUI.gui_inputfield(
                    rect: rect_offset,
                    title: null,
                    prop: pivot.vector2Value,
                    title_size: XGUIFontSize.M,
                    //title_color: Color.white,
                    title_anchor: TextAnchor.MiddleLeft,
                    title_width: 60);
            }
            pivot.serializedObject.ApplyModifiedProperties();
            SerializedProperty cur_sprite = sp_current_cursor_style.FindPropertyRelative(nameof(MouseStyles.Sprite));
            Sprite cur_spr = (Sprite)cur_sprite.objectReferenceValue;
            if (cur_spr == spr)
            {
                Image img_cursor = (Image)sp_cursor_imager.objectReferenceValue;
                img_cursor.rectTransform.pivot = pivot.vector2Value;
                sp_cursor_imager.serializedObject.ApplyModifiedProperties();
            }
            #endregion

            #region 预览光标
            titleheight += 30;
            Sprite icon_spr = (Sprite)index_sprite.objectReferenceValue;
            Rect rect_icon = new Rect(rect.x + (current_width <= 130 ? 0 : 10), titleheight, 15, 15);
            XGUI.gui_icon(
                rect: rect_icon,
                icon: icon_spr != null ? icon_spr.texture : icon_invalid_tex,
                color: Color.white);
            #endregion
        }
        /// <summary>
        /// 鼠标样式列表添加回调。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>在列表末尾插入新元素。</description></item>
        /// <item><description>初始化标识为 <c>NewCursor</c>。</description></item>
        /// <item><description>初始化位置偏移为 <c>(0, 1)</c>。</description></item>
        /// </list>
        /// </summary>
        private void MouseStyles_OnAdd(ReorderableList list)
        {
            sp_mouse_styles.InsertArrayElementAtIndex(list.count);
            sp_mouse_styles.GetArrayElementAtIndex(list.count - 1).FindPropertyRelative(nameof(MouseStyles.Name)).stringValue = "NewCursor";
            sp_mouse_styles.GetArrayElementAtIndex(list.count - 1).FindPropertyRelative(nameof(MouseStyles.Pivot)).vector2Value = new Vector2(0, 1);
        }
        /// <summary>
        /// 鼠标样式列表移除回调。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>删除当前选中索引处的元素。</description></item>
        /// </list>
        /// </summary>
        private void MouseStyles_OnRemove(ReorderableList list)
        {
            sp_mouse_styles.DeleteArrayElementAtIndex(list.index);
        }
        /// <summary>
        /// 鼠标样式列表选中回调。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>将选中元素的样式写入当前光标样式。</description></item>
        /// <item><description>同步更新光标图像的贴图与轴心。</description></item>
        /// </list>
        /// </summary>
        private void MouseStyles_OnSelect(ReorderableList list)
        {
            SerializedProperty sp_style = sp_mouse_styles.GetArrayElementAtIndex(list.index);
            MouseStyles style = new MouseStyles();
            style.Name = sp_style.FindPropertyRelative(nameof(MouseStyles.Name)).stringValue;
            style.Pivot = sp_style.FindPropertyRelative(nameof(MouseStyles.Pivot)).vector2Value;
            style.Sprite = (Sprite)sp_style.FindPropertyRelative(nameof(MouseStyles.Sprite)).objectReferenceValue;

            sp_current_cursor_style.FindPropertyRelative(nameof(MouseStyles.Name)).stringValue = style.Name;
            sp_current_cursor_style.FindPropertyRelative(nameof(MouseStyles.Pivot)).vector2Value = style.Pivot;
            sp_current_cursor_style.FindPropertyRelative(nameof(MouseStyles.Sprite)).objectReferenceValue = style.Sprite;
            Image img = (Image)sp_cursor_imager.objectReferenceValue;
            img.sprite = style.Sprite;
            img.rectTransform.pivot = sp_current_cursor_style.FindPropertyRelative(nameof(MouseStyles.Pivot)).vector2Value;
        }
        /// <summary>
        /// 鼠标样式列表元素高度回调。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>返回固定高度 <c>4.8f * LineHeight</c>。</description></item>
        /// </list>
        /// </summary>
        private float MouseStyles_ElementHeight(int index)
        {
            return 4.8f * lineheight;
        }
        #endregion

        #region 绘制
        /// <summary>
        /// 通用方法：绘制开关。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>封装 <c>XGUI.layout_toggle</c> 调用。</description></item>
        /// <item><description>提供标题、宽度、样式、颜色等参数。</description></item>
        /// </list>
        /// </summary>
        private void DrawToggle(string title, SerializedProperty prop, float width, XGUIToggleStyle style = XGUIToggleStyle.实体, Color color_bg_on = default, Color color_bg_off = default, Color color_on = default, Color color_off = default, Action<bool> act_on_changed = null)
        {
            XGUI.layout_toggle(
                title: title,
                title_size: XGUIFontSize.M,
                title_font_style: FontStyle.Normal,
                title_padding: new RectOffset(5, 10, 0, 0),
                title_width: width,
                prop: prop,
                tog_style: style,
                tog_padding: new RectOffset(0, 9, 0, 0),
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
                tog_mixed_bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                act_on_changed: act_on_changed);
        }
        #endregion
    }
}