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
    using System;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;
    using Image = UnityEngine.UI.Image;

    [CustomEditor(typeof(XHud_CustomMouseCursor), true)]
    public class Editor_XHud_CustomMouseCursor : Editor
    {
        #region 组件 / 列表
        private XHud_CustomMouseCursor BaseScript;
        private ReorderableList StyleList;
        #endregion

        #region 序列化属性
        private SerializedProperty CursorRect, CursorCanvasGroup, CursorImager, UseCustomCursor, MouseStyles, CursorSize_TweenDuration, CursorSize_TweenEase, CursorImagerSizeSmooth, CursorOpacity_TweenDuration, CursorOpacity_TweenEase, CursorSize, CursorColor, CursorColorSmooth, CursorOpacity, IndexMouseType, UseLerpCursorSize, CurrentCursorStyle;
        #endregion

        private float LineHeight;

        #region 图标
        private Texture2D icon_main;
        private Texture2D icon_invalid_tex;
        #endregion

        #region 批量化操作
        private XHud_CustomMouseCursor[] SelectedObjects;

        private void GetAllTargets()
        {
            if (targets.Length > 1)
            {
                SelectedObjects = new XHud_CustomMouseCursor[targets.Length];
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    var t = targets[i];
                    SelectedObjects[i] = (XHud_CustomMouseCursor)t;
                }
            }
            else
            {
                SelectedObjects = new XHud_CustomMouseCursor[targets.Length];
                SelectedObjects[0] = (XHud_CustomMouseCursor)target;
            }
        }

        private bool IsMultiSelected()
        {
            if (SelectedObjects == null)
                return false;
            if (SelectedObjects.Length > 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        void OnEnable()
        {
            BaseScript = (XHud_CustomMouseCursor)target;

            #region 获取序列化属性
            CursorRect = serializedObject.FindProperty("CursorRect");
            CursorCanvasGroup = serializedObject.FindProperty("CursorCanvasGroup");
            UseCustomCursor = serializedObject.FindProperty("UseCustomCursor");
            CursorOpacity = serializedObject.FindProperty("CursorOpacity");
            CursorImager = serializedObject.FindProperty("CursorImager");
            MouseStyles = serializedObject.FindProperty("MouseStyles");
            CursorSize_TweenDuration = serializedObject.FindProperty("CursorSize_TweenDuration");
            CursorSize_TweenEase = serializedObject.FindProperty("CursorSize_TweenEase");
            CursorImagerSizeSmooth = serializedObject.FindProperty("CursorImagerSizeSmooth");
            CursorOpacity_TweenDuration = serializedObject.FindProperty("CursorOpacity_TweenDuration");
            CursorOpacity_TweenEase = serializedObject.FindProperty("CursorOpacity_TweenEase");
            CursorSize = serializedObject.FindProperty("CursorSize");
            CursorColor = serializedObject.FindProperty("CursorColor");
            CursorColorSmooth = serializedObject.FindProperty("CursorColorSmooth");
            CurrentCursorStyle = serializedObject.FindProperty("CurrentCursorStyle");
            IndexMouseType = serializedObject.FindProperty("IndexMouseType");
            UseLerpCursorSize = serializedObject.FindProperty("UseLerpCursorSize");
            #endregion

            #region 获取图标
            icon_main = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_mouse_cursor/icon_main");
            icon_invalid_tex = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_mouse_cursor/icon_invalid");
            #endregion

            BaseScript.CursorRect = BaseScript.GetComponent<RectTransform>();
            BaseScript.CursorCanvasGroup = BaseScript.GetComponent<CanvasGroup>();

            if (BaseScript.transform.childCount <= 0)
            {
                GameObject imager = new GameObject();
                imager.transform.SetParent(BaseScript.transform);
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
                BaseScript.CursorImager = img;
            }
            else
            {
                BaseScript.CursorImager = BaseScript.transform.GetChild(0).GetComponent<Image>();
            }

            LineHeight = XGUI.GetSingleLineHeight();

            GetAllTargets();

            #region ReorderableList - MouseStyles
            StyleList = new ReorderableList(serializedObject, MouseStyles)
            {
                displayAdd = true,
                displayRemove = true,
                draggable = true,

                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "鼠标样式表");
                },
                drawElementBackgroundCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    if (isFocused)
                        XGUI.gui_box(new Rect(rect.x + 22, rect.y + 7, 2, 10), XHud_Dashboard.Theme_Primary);
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    float titleheight = rect.y + 2;
                    float baseheight = rect.y + (rect.height - 25);
                    float current_width = XGUI.GetCurrentWindowWidth();

                    SerializedProperty sp_name = MouseStyles.GetArrayElementAtIndex(index).FindPropertyRelative("Name");

                    SerializedProperty index_sprite = MouseStyles.GetArrayElementAtIndex(index).FindPropertyRelative("Sprite");
                    Sprite spr = (Sprite)index_sprite.objectReferenceValue;

                    SerializedProperty pivot = MouseStyles.GetArrayElementAtIndex(index).FindPropertyRelative("Pivot");

                    #region 光标名称
                    if (current_width > 130)
                    {
                        Rect rect_name = new Rect(rect.x + 12, titleheight, rect.width - 16, LineHeight);
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
                        Rect rect_sprite = new Rect(rect.x + 47, titleheight + 30, rect.width - 50, LineHeight);
                        XGUI.gui_property_field(
                            rect: rect_sprite,
                            title: null,
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 80,
                            prop: index_sprite);

                        index_sprite.serializedObject.ApplyModifiedProperties();
                    }
                    #endregion

                    #region 位置相对偏移
                    if (current_width > 130)
                    {
                        Rect rect_offset = new Rect(rect.x + 47, titleheight + 55, rect.width - 50, LineHeight);

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

                    SerializedProperty cur_sprite = CurrentCursorStyle.FindPropertyRelative("Sprite");
                    Sprite cur_spr = (Sprite)cur_sprite.objectReferenceValue;

                    if (cur_spr == spr)
                    {
                        Image img_cursor = (Image)CursorImager.objectReferenceValue;
                        img_cursor.rectTransform.pivot = pivot.vector2Value;
                        CursorImager.serializedObject.ApplyModifiedProperties();
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
                },
                onAddCallback = (ReorderableList list) =>
                {
                    MouseStyles.InsertArrayElementAtIndex(list.count);

                    MouseStyles.GetArrayElementAtIndex(list.count - 1).FindPropertyRelative("Name").stringValue = "NewCursor";
                    MouseStyles.GetArrayElementAtIndex(list.count - 1).FindPropertyRelative("Pivot").vector2Value = new Vector2(0, 1);
                },
                onRemoveCallback = (ReorderableList list) =>
                {
                    MouseStyles.DeleteArrayElementAtIndex(list.index);
                },
                onSelectCallback = (ReorderableList list) =>
                {
                    SerializedProperty sp_style = MouseStyles.GetArrayElementAtIndex(list.index);

                    MouseStyles style = new MouseStyles();

                    style.Name = sp_style.FindPropertyRelative("Name").stringValue;
                    style.Pivot = sp_style.FindPropertyRelative("Pivot").vector2Value;
                    style.Sprite = (Sprite)sp_style.FindPropertyRelative("Sprite").objectReferenceValue;

                    CurrentCursorStyle.FindPropertyRelative("Name").stringValue = style.Name;
                    CurrentCursorStyle.FindPropertyRelative("Pivot").vector2Value = style.Pivot;
                    CurrentCursorStyle.FindPropertyRelative("Sprite").objectReferenceValue = style.Sprite;

                    Image img = (Image)CursorImager.objectReferenceValue;
                    img.sprite = style.Sprite;
                    img.rectTransform.pivot = CurrentCursorStyle.FindPropertyRelative("Pivot").vector2Value;
                },
                elementHeightCallback = index =>
                {
                    return 4.8f * LineHeight;
                }
            };
            #endregion
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

#if UNITY_6000_0_OR_NEWER
            TextClipping clipping = TextClipping.Ellipsis;
#else
    TextClipping clipping = TextClipping.Clip;
#endif

            #region 标题
            XGUI.layout_banner(
             bg_fill: XGUIFilled.实体,
             bg_color: XGUIColor.深空灰,
             bg_height: 30,
             icon: icon_main,
             icon_color: XHud_Dashboard.Theme_Primary,
             title_text: "XHud  -  鼠标样式",
             title_anchor: TextAnchor.MiddleLeft,
             title_style: FontStyle.Normal,
             title_color: Color.white,
             title_size: XGUIFontSize.B,
             title_clipping: clipping,
             bg_margin: new RectOffset(0, 0, 5, 5));
            #endregion

            XGUI.layout_space(5);

            #region 当前光标预览
            XGUI.layout_group_start(
               type: XGUIContainerType.Vertical,
               bg_fill: XGUIFilled.缺口纯色边框,
               bg_color: XGUIColor.亮白,
               bg_color_gui: XHud_Dashboard.Theme_Group,
               title: "当前光标预览",
               title_size: XGUIFontSize.M,
               title_text_color: XHud_Dashboard.Theme_Primary,
               title_clipping: TextClipping.Clip,
               padding: new RectOffset(10, 10, 15, 15));

            if (!Application.isPlaying)
            {
                CanvasGroup cg = (CanvasGroup)CursorCanvasGroup.objectReferenceValue;
                Image imgsursor = (Image)CursorImager.objectReferenceValue;

                if (cg != null)
                    cg.alpha = CursorOpacity.floatValue;
                CursorCanvasGroup.serializedObject.ApplyModifiedProperties();

                if (imgsursor != null)
                {
                    imgsursor.rectTransform.sizeDelta = Vector2.one * CursorSize.floatValue;
                }
                CursorImager.serializedObject.ApplyModifiedProperties();
            }

            if (UseCustomCursor.boolValue)
            {
                SerializedProperty index_sprite = CurrentCursorStyle.FindPropertyRelative("Sprite");
                Sprite spr = (Sprite)index_sprite.objectReferenceValue;

                #region 当前选择的光标图标
                XGUI.layout_icon(
                    icon: spr != null && spr.texture != null ? spr.texture : icon_invalid_tex,
                    icon_color: CursorColor.colorValue,
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
                    bg_fill: XGUIFilled.无,
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
                    bg_fill: XGUIFilled.无,
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
            BaseScript.fold_options = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.Theme_Group,
                title: "选项",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.Theme_Primary,
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: BaseScript.fold_options);

            if (!BaseScript.fold_options)
            {
                DrawToggle("自定义光标", UseCustomCursor, 120, XGUIToggleStyle.实体, XHud_Dashboard.Theme_Primary, Color.white * 0.65f, Color.white, Color.white, (b) => { });

                string[] actionlist = System.Enum.GetNames(typeof(MouseType));
                XGUI.layout_string_popup(
                    title: "鼠标响应",
                    title_width: 100,
                    title_size: XGUIFontSize.M,
                    title_anchor: TextAnchor.MiddleLeft,
                    prop: IndexMouseType,
                    options: actionlist,
                    opt_text_size: XGUIFontSize.M,
                    opt_text_color: Color.black,
                    opt_text_padding: new RectOffset(10, 10, 0, 0),
                    opt_anchor: TextAnchor.MiddleLeft,
                    opt_font_style: FontStyle.Normal,
                    opt_bg_fill: XGUIFilled.实体,
                    opt_bg_color: XGUIColor.亮白,
                    opt_bg_color_gui: XHud_Dashboard.Theme_Primary,
                    margin: new RectOffset(0, 0, 5, 5),
                    padding: new RectOffset(5, 5, 0, 0),
                    title_margin: new RectOffset(0, 0, 0, 0),
                    icon_arrow_color: Color.black,
                    act_on_changed: (d) =>
                     {
                         BaseScript.MouseClickType = (MouseType)System.Enum.Parse(typeof(MouseType), d);
                     });

                string[] smoothsizelist = new string[2] { "缓动模式", "差值模式" };
                XGUI.layout_string_popup(
                    title: "光标尺寸平滑方式",
                    title_width: 100,
                    title_size: XGUIFontSize.M,
                    title_anchor: TextAnchor.MiddleLeft,
                    prop: UseLerpCursorSize,
                    options: smoothsizelist,
                    opt_text_size: XGUIFontSize.M,
                    opt_text_color: Color.black,
                    opt_text_padding: new RectOffset(10, 10, 0, 0),
                    opt_anchor: TextAnchor.MiddleLeft,
                    opt_font_style: FontStyle.Normal,
                    opt_bg_fill: XGUIFilled.实体,
                    opt_bg_color: XGUIColor.亮白,
                    opt_bg_color_gui: XHud_Dashboard.Theme_Primary,
                    margin: new RectOffset(0, 0, 5, 5),
                    padding: new RectOffset(5, 5, 0, 0),
                    title_margin: new RectOffset(0, 0, 0, 0),
                    icon_arrow_color: Color.black);
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 参数
            BaseScript.fold_params = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.Theme_Group,
                title: "参数",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.Theme_Primary,
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(5, 10, 15, 15),
                foldout: BaseScript.fold_params);

            Image img_cursor = (Image)CursorImager.objectReferenceValue;

            if (UseCustomCursor.boolValue)
            {
                if (!BaseScript.fold_params)
                {
                    #region 焦点物体
                    XGUI.layout_property_field(
                        title: "焦点物体",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 90,
                        prop: CursorRect,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion

                    #region 透明组件
                    XGUI.layout_property_field(
                        title: "透明组件",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 90,
                        prop: CursorCanvasGroup,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion

                    #region 光标组件
                    XGUI.layout_property_field(
                        title: "光标组件",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 90,
                        prop: CursorImager,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion

                    #region 尺寸
                    XGUI.layout_property_field(
                        title: "尺寸",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 90,
                        prop: CursorSize,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion

                    #region 颜色
                    EditorGUI.BeginChangeCheck();
                    XGUI.layout_property_field(
                        title: "颜色",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 90,
                        prop: CursorColor,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    if (EditorGUI.EndChangeCheck())
                    {
                        BaseScript.CursorImager.color = CursorColor.colorValue;
                    }

                    #endregion

                    #region 颜色平滑系数
                    XGUI.layout_property_field(
                        title: "颜色平滑系数",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 90,
                        prop: CursorColorSmooth,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion

                    #region 透明度
                    XGUI.layout_property_field(
                        title: "透明度",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 90,
                        prop: CursorOpacity,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion

                    #region 透明度缓动方式
                    XGUI.layout_property_field(
                        title: "透明度缓动方式",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 90,
                        prop: CursorOpacity_TweenEase,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion

                    #region 透明度变化耗时
                    XGUI.layout_property_field(
                        title: "透明度变化耗时",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.Theme_Primary,
                        title_width: 90,
                        prop: CursorOpacity_TweenDuration,
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    #endregion

                    if (UseLerpCursorSize.stringValue == "缓动模式")
                    {
                        #region 光标尺寸缓动耗时
                        XGUI.layout_property_field(
                            title: "光标尺寸缓动耗时",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 90,
                            prop: CursorSize_TweenDuration,
                            prop_margin: new RectOffset(0, 0, 5, 0));
                        #endregion

                        #region 光标尺寸缓动方式
                        XGUI.layout_property_field(
                            title: "光标尺寸缓动方式",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 90,
                            prop: CursorSize_TweenEase,
                            prop_margin: new RectOffset(0, 0, 5, 0));
                        #endregion
                    }
                    else
                    {
                        #region 光标尺寸差值速率
                        XGUI.layout_property_field(
                            title: "光标尺寸差值速率",
                            title_size: XGUIFontSize.M,
                            title_hover_color: XHud_Dashboard.Theme_Primary,
                            title_width: 90,
                            prop: CursorImagerSizeSmooth,
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
            BaseScript.fold_styles = XGUI.layout_group_start(
              type: XGUIContainerType.Vertical,
              bg_fill: XGUIFilled.缺口纯色边框,
              bg_color: XGUIColor.亮白,
              bg_color_gui: XHud_Dashboard.Theme_Group,
              title: "光标样式表",
              title_size: XGUIFontSize.M,
              title_text_color: XHud_Dashboard.Theme_Primary,
              title_clipping: TextClipping.Clip,
              padding: new RectOffset(10, 10, 15, 15),
              foldout: BaseScript.fold_styles);

            if (!BaseScript.fold_styles)
            {
                StyleList.DoLayoutList();
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 源脚本
            BaseScript.fold_based = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.Theme_Group,
                title: "源脚本",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.Theme_Primary,
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: BaseScript.fold_based);

            if (!BaseScript.fold_based)
            {
                DrawDefaultInspector();
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            serializedObject.ApplyModifiedProperties();
        }

        #region Draw
        /// <summary>
        /// 通用方法：绘制开关
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
                tog_mixed_bg_color_gui: XHud_Dashboard.Theme_Primary,
                act_on_changed: act_on_changed);
        }
        #endregion
    }
}