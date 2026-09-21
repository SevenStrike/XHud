/*
 * ============================================================================
 * ⚠️ 版权声明（禁止删除、禁止修改、衍生作品必须保留此注释）⚠️
 * ============================================================================
 * 版权声明 Copyright (C) 2025-Present Nanjing SevenStrike Media Co., Ltd.
 * 中文名称：南京塞维斯传媒有限公司
 * 英文名称：SevenStrikeMedia
 * 项目作者：徐寅智
 * 项目名称：XTween - Unity 高性能动画架构插件
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
namespace SevenStrikeModules.XTween.Editor
{
    using SevenStrikeModules.XGUI.Editor;
    using SevenStrikeModules.XGUI.Runtime;
    using System;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    /// <summary>
    /// 自定义编辑器类，用于在 Unity 编辑器中可视化和编辑 XTween_PathTool 路径工具
    /// 提供路径点的绘制、编辑、删除以及路径的可视化功能
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(XTween_PathTool))]
    public class Editor_XTween_PathTool : Editor
    {
        /// <summary>
        /// 当前正在编辑的路径工具实例
        /// </summary>
        private XTween_PathTool BaseScript;
        /// <summary>
        /// 路径点列表
        /// </summary>
        private ReorderableList PathPoints;
        /// <summary>
        /// 序列化属性
        /// </summary>
        private SerializedProperty sp_PathPoints, sp_PathType, sp_PathOrientation, sp_PathOrientationVector, sp_LookAtObject, sp_LookAtPosition, sp_LookAtPoints, sp_IsWorldMode, sp_StartPosition, sp_PathLength, sp_SegmentsPerCurve, sp_IsClosed, sp_DisplayPath, sp_DisplayIndex, sp_Color_Path, sp_Color_PathPoint, sp_Color_PathPoint_Selected, sp_Color_BezierControl, sp_Color_BezierControl_Selected, sp_Color_Index, sp_Color_IndexLength, sp_Color_LookAtLine, sp_ControlLineStyle, sp_PathPointSize, sp_BezierControlSize, sp_PathWidth, sp_IndexSize, sp_IndexLengthHeight, sp_IndexOffset, sp_AddedDistance, sp_LookAtLine, sp_LookAtLineWidth, sp_PathMarksTexture, sp_PathMarksSize, sp_PathMarksSample, sp_PathMarksMode, sp_PathPointsIsFold, sp_IndexPathType, sp_IndexPathOrientation, sp_IndexPathOrientationVector, sp_IndexControlLineStyle, sp_IndexPathMarkMode, sp_PathLimitePercent, sp_PathParent, sp_PathMarksColor, sp_PathMarksGroup;
        /// <summary>
        /// 图标
        /// </summary>
        private Texture2D icon_main;
        /// <summary>
        /// 当前选中的锚点索引，用于路径点的编辑
        /// </summary>
        private int selectedAnchorIndex = -1;
        /// <summary>
        /// 当前选中的控制点索引，用于贝塞尔曲线控制点的编辑
        /// </summary>
        private int selectedControlIndex = -1;
        /// <summary>
        /// GUI单行高度
        /// </summary>
        private float LineHeight;
        /// <summary>
        /// 是否正在编辑控制点
        /// </summary>
        private bool isInControl = false;
        private bool fold_raw = false;
        /// <summary>
        /// 用于绘制路径点索引的样式
        /// </summary>
        private GUIStyle IndexStyle;

        string[] enums_name_path_type;
        string[] enums_name_path_orient;
        string[] enums_name_path_orient_vector;
        string[] enums_name_line_style;
        string[] enums_name_path_marks;

        #region 批量化操作
        private XTween_PathTool[] SelectedObjects;
        private void GetAllTargets()
        {
            if (targets.Length > 1)
            {
                SelectedObjects = new XTween_PathTool[targets.Length];
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    var t = targets[i];
                    SelectedObjects[i] = (XTween_PathTool)t;
                }
            }
            else
            {
                SelectedObjects = new XTween_PathTool[targets.Length];
                SelectedObjects[0] = (XTween_PathTool)target;
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

        /// <summary>
        /// 在编辑器启用时初始化路径工具和相关事件
        /// </summary>
        private void OnEnable()
        {
            BaseScript = (XTween_PathTool)target;
            SceneView.duringSceneGui -= DuringSceneGUI;
            SceneView.duringSceneGui += DuringSceneGUI;

            #region 获取序列化属性
            sp_PathPoints = serializedObject.FindProperty("PathPoints");
            sp_PathType = serializedObject.FindProperty("PathType");
            sp_PathOrientation = serializedObject.FindProperty("PathOrientation");
            sp_PathOrientationVector = serializedObject.FindProperty("PathOrientationVector");
            sp_LookAtObject = serializedObject.FindProperty("LookAtObject");
            sp_LookAtPosition = serializedObject.FindProperty("LookAtPosition");
            sp_LookAtPoints = serializedObject.FindProperty("LookAtPoints");
            sp_IsWorldMode = serializedObject.FindProperty("IsWorldMode");
            sp_StartPosition = serializedObject.FindProperty("StartPosition");
            sp_PathLength = serializedObject.FindProperty("PathLength");
            sp_SegmentsPerCurve = serializedObject.FindProperty("SegmentsPerCurve");
            sp_IsClosed = serializedObject.FindProperty("IsClosed");
            sp_DisplayPath = serializedObject.FindProperty("DisplayPath");
            sp_DisplayIndex = serializedObject.FindProperty("DisplayIndex");
            sp_Color_Path = serializedObject.FindProperty("Color_Path");
            sp_Color_PathPoint = serializedObject.FindProperty("Color_PathPoint");
            sp_Color_PathPoint_Selected = serializedObject.FindProperty("Color_PathPoint_Selected");
            sp_Color_BezierControl = serializedObject.FindProperty("Color_BezierControl");
            sp_Color_BezierControl_Selected = serializedObject.FindProperty("Color_BezierControl_Selected");
            sp_Color_Index = serializedObject.FindProperty("Color_Index");
            sp_Color_IndexLength = serializedObject.FindProperty("Color_IndexLength");
            sp_Color_LookAtLine = serializedObject.FindProperty("Color_LookAtLine");
            sp_ControlLineStyle = serializedObject.FindProperty("ControlLineStyle");
            sp_PathPointSize = serializedObject.FindProperty("PathPointSize");
            sp_BezierControlSize = serializedObject.FindProperty("BezierControlSize");
            sp_PathWidth = serializedObject.FindProperty("PathWidth");
            sp_IndexSize = serializedObject.FindProperty("IndexSize");
            sp_IndexLengthHeight = serializedObject.FindProperty("IndexLengthHeight");
            sp_IndexOffset = serializedObject.FindProperty("IndexOffset");
            sp_AddedDistance = serializedObject.FindProperty("AddedDistance");
            sp_LookAtLine = serializedObject.FindProperty("LookAtLine");
            sp_LookAtLineWidth = serializedObject.FindProperty("LookAtLineWidth");
            sp_PathMarksTexture = serializedObject.FindProperty("PathMarksTexture");
            sp_PathMarksSize = serializedObject.FindProperty("PathMarksSize");
            sp_PathMarksSample = serializedObject.FindProperty("PathMarksSample");
            sp_PathMarksMode = serializedObject.FindProperty("PathMarksMode");
            sp_PathPointsIsFold = serializedObject.FindProperty("PathPointsIsFold");
            sp_IndexPathType = serializedObject.FindProperty("IndexPathType");
            sp_IndexPathOrientation = serializedObject.FindProperty("IndexPathOrientation");
            sp_IndexPathOrientationVector = serializedObject.FindProperty("IndexPathOrientationVector");
            sp_IndexControlLineStyle = serializedObject.FindProperty("IndexControlLineStyle");
            sp_IndexPathMarkMode = serializedObject.FindProperty("IndexPathMarkMode");
            sp_PathLimitePercent = serializedObject.FindProperty("PathLimitePercent");
            sp_PathParent = serializedObject.FindProperty("PathParent");
            sp_PathMarksColor = serializedObject.FindProperty("PathMarksColor");
            sp_PathMarksGroup = serializedObject.FindProperty("PathMarksGroup");
            #endregion

            #region 图标获取
            icon_main = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_path/icon_main");
            #endregion

            enums_name_path_type = Enum.GetNames(typeof(XTween_PathType));
            enums_name_path_orient = Enum.GetNames(typeof(XTween_PathOrientation));
            enums_name_path_orient_vector = Enum.GetNames(typeof(XTween_PathOrientationVector));
            enums_name_line_style = Enum.GetNames(typeof(XTween_LineStyle));
            enums_name_path_marks = Enum.GetNames(typeof(XTween_PathMarksMode));

            LineHeight = XGUI.GetSingleLineHeight();

            // 初始化索引样式
            IndexStyle = new GUIStyle();
            IndexStyle.font = AssetDatabase.LoadAssetAtPath<Font>(XTween_Dashboard.Get_XTween_GUIRoot_Path() + "EditorFonts/sx_bold.otf");
            GetAllTargets();

            sp_PathParent.objectReferenceValue = BaseScript.transform.parent;
            sp_PathParent.serializedObject.ApplyModifiedProperties();

            SetTransformCenter();

            #region ReorderableList - PathPoints
            PathPoints = new ReorderableList(serializedObject, sp_PathPoints)
            {
                displayAdd = false,
                displayRemove = true,
                draggable = true,

                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "路径点信息列表");
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    float titleheight = rect.y + 6;

                    SerializedProperty sp_root = sp_PathPoints.GetArrayElementAtIndex(index);
                    if (sp_root != null)
                    {
#if UNITY_6000_0_OR_NEWER
                        // Unity 6+ 使用 Ellipsis
                        TextClipping clipping = TextClipping.Ellipsis;
#else
    // Unity 2021.1 之前使用 Clip
    TextClipping clipping = TextClipping.Clip;
#endif

                        XGUI.gui_label(
                            rect: new Rect(rect.x + 25, titleheight - 3.5f, (rect.width * 0.45f) - 20, LineHeight),
                            text: new GUIContent($"#  路径点 {index.ToString()}"),
                            text_color: Color.white,
                            size: XGUIFontSize.M,
                            clipping: clipping,
                            anchor: TextAnchor.MiddleLeft,
                            offset: new Vector2(0, 0),
                            font_style: FontStyle.Bold,
                            font: XGUI.GetFont("xg-medium"));

                        XGUI.gui_icon(
                            rect: new Rect(rect.x + 5, titleheight, 10, 10),
                            icon: XGUI.GetBasedIcon("icon_locate"),
                            padding: new RectOffset(0, 0, 0, 0),
                            color: XTween_Dashboard.Theme_Primary);

                        SerializedProperty sp_relative = sp_root.FindPropertyRelative("relative");
                        SerializedProperty sp_world = sp_root.FindPropertyRelative("world");
                        SerializedProperty sp_anchored = sp_root.FindPropertyRelative("anchored");
                        SerializedProperty sp_bezier_in = sp_root.FindPropertyRelative("bezier_in");
                        SerializedProperty sp_bezier_out = sp_root.FindPropertyRelative("bezier_out");
                        SerializedProperty sp_bezier_in_world = sp_root.FindPropertyRelative("bezier_in_world");
                        SerializedProperty sp_bezier_out_world = sp_root.FindPropertyRelative("bezier_out_world");

                        string hexcol = XGUI_Utilitys.Color_To_HexString(XTween_Dashboard.Theme_Primary, true);

                        #region 路径点数据
                        DrawPointInfo(
                            rect: new Rect(rect.x + 5, titleheight + 20, rect.width - 15, 19),
                            info: $"<color={hexcol}> -   Relative   :   </color><color=#c2c2c2>" + XGUI_Utilitys.Vector3_To_String(sp_relative.vector3Value) + "</color>",
                            clipping: clipping);

                        DrawPointInfo(
                            rect: new Rect(rect.x + 5, titleheight + 40, rect.width - 15, 19),
                            info: $"<color=#c2c2c2> -   World   :   </color><color=#c2c2c2>" + XGUI_Utilitys.Vector3_To_String(sp_world.vector3Value) + "</color>",
                            clipping: clipping);

                        DrawPointInfo(
                            rect: new Rect(rect.x + 5, titleheight + 60, rect.width - 15, 19),
                            info: $"<color={hexcol}> -   Anchored   :   </color><color=#c2c2c2>" + XGUI_Utilitys.Vector3_To_String(sp_anchored.vector3Value) + "</color>",
                            clipping: clipping);

                        DrawPointInfo(
                            rect: new Rect(rect.x + 5, titleheight + 80, rect.width - 15, 19),
                            info: $"<color=#c2c2c2> -   Bezier_In   :   </color><color=#c2c2c2>" + XGUI_Utilitys.Vector3_To_String(sp_bezier_in.vector3Value) + "</color>",
                            clipping: clipping);

                        DrawPointInfo(
                            rect: new Rect(rect.x + 5, titleheight + 100, rect.width - 15, 19),
                            info: $"<color={hexcol}> -   Bezier_Out   :   </color><color=#c2c2c2>" + XGUI_Utilitys.Vector3_To_String(sp_bezier_out.vector3Value) + "</color>",
                            clipping: clipping);

                        DrawPointInfo(
                            rect: new Rect(rect.x + 5, titleheight + 120, rect.width - 15, 19),
                            info: $"<color=#c2c2c2> -   BezierWorld_In   :   </color><color=#c2c2c2>" + XGUI_Utilitys.Vector3_To_String(sp_bezier_in_world.vector3Value) + "</color>",
                            clipping: clipping);

                        DrawPointInfo(
                            rect: new Rect(rect.x + 5, titleheight + 140, rect.width - 15, 19),
                            info: $"<color={hexcol}> -   BezierWorld_Out   :   </color><color=#c2c2c2>" + XGUI_Utilitys.Vector3_To_String(sp_bezier_out_world.vector3Value) + "</color>",
                            clipping: clipping);
                        #endregion
                    }
                },
                onSelectCallback = (ReorderableList list) =>
                {
                    selectedAnchorIndex = list.index;
                },
                elementHeightCallback = index =>
                {
                    return 9.5f * LineHeight;
                }
            };
            #endregion
        }
        /// <summary>
        /// 在编辑器禁用时移除相关事件
        /// </summary>
        private void OnDisable()
        {
            SetTransformCenter();
            SceneView.duringSceneGui -= DuringSceneGUI;
        }
        /// <summary>
        /// 在编辑器销毁时移除相关事件
        /// </summary>
        private void OnDestroy()
        {
            SceneView.duringSceneGui -= DuringSceneGUI;
        }
        /// <summary>
        /// 在 Inspector 中绘制路径工具的属性和按钮
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            Event currentEvent = Event.current;
            if (currentEvent.type == EventType.MouseDown)
            {
                // 取消当前拥有键盘焦点的控件
                GUI.FocusControl(null);
                Repaint();
            }

            XGUI.layout_banner(
               bg_fill: XGUIFilled.实体,
               bg_color: XGUIColor.深空灰,
               bg_height: 30,
               icon: icon_main,
               icon_color: XTween_Dashboard.Theme_Primary,
               title_text: "XTween  -  路径工具",
               title_anchor: TextAnchor.MiddleLeft,
               title_style: FontStyle.Normal,
               title_color: Color.white,
               title_size: XGUIFontSize.B,
               title_clipping: TextClipping.Ellipsis);

            #region 快捷功能
            XGUI.layout_group_start(
                type: XGUIContainerType.Horizontal,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XTween_Dashboard.Theme_Group,
                title: "快捷功能",
                title_size: XGUIFontSize.M,
                title_text_color: XTween_Dashboard.Theme_Primary,
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(20, 20, 20, 20));

            #region 添加路径点 
            if (XGUI.layout_button(
                tooltip: "添加路径点",
                tex_release: XGUI.GetBasedIcon("icon_add_r"),
                tex_press: XGUI.GetBasedIcon("icon_add_p"),
                tex_gui_color: Color.white,
                width: 15,
                height: 15))
            {
                PathPoints_Add();
                return;
            }
            #endregion

            GUILayout.FlexibleSpace();

            #region 清空路径点 
            if (XGUI.layout_button(
                tooltip: "清空路径点",
                tex_release: XGUI.GetBasedIcon("icon_clear_r"),
                tex_press: XGUI.GetBasedIcon("icon_clear_p"),
                tex_gui_color: Color.white,
                width: 15,
                height: 15))
            {
                PathPoints_Clear();
                return;
            }
            #endregion

            GUILayout.FlexibleSpace();

            #region 路径点深度归零 
            if (XGUI.layout_button(
                tooltip: "路径点深度归零",
                tex_release: XGUI.GetBasedIcon("icon_rewind_r"),
                tex_press: XGUI.GetBasedIcon("icon_rewind_p"),
                tex_gui_color: Color.white,
                width: 15,
                height: 15))
            {
                PathPoints_ZAxis_ToZero();
                return;
            }
            #endregion

            GUILayout.FlexibleSpace();

            #region 生成路径标记 
            if (XGUI.layout_button(
                tooltip: "生成路径标记",
                tex_release: XGUI.GetBasedIcon("icon_path_r"),
                tex_press: XGUI.GetBasedIcon("icon_path_p"),
                tex_gui_color: Color.white,
                width: 15,
                height: 15))
            {
                PathMarksCreator();
                return;
            }
            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            #endregion

            #region 路径列表
            BaseScript.fold_points = XGUI.layout_group_start(
                 type: XGUIContainerType.Vertical,
                 bg_fill: XGUIFilled.缺口纯色边框,
                 bg_color: XGUIColor.亮白,
                 bg_color_gui: XTween_Dashboard.Theme_Group,
                 title: "路径列表",
                 title_size: XGUIFontSize.M,
                 title_text_color: XTween_Dashboard.Theme_Primary,
                 title_clipping: TextClipping.Clip,
                 padding: new RectOffset(5, 5, 10, 10),
                 foldout: BaseScript.fold_points);

            if (BaseScript.fold_points)
            {
                if (sp_PathPoints.arraySize > 0)
                {
                    if (IsMultiSelected())
                    {
                        XGUI.layout_helpbox(
                            state: XGUIHelboxState.警告,
                            title_text: "路径列表不支持多项操作",
                            title_size: XGUIFontSize.M,
                            title_style: FontStyle.Normal,
                            title_color: Color.white * 0.75f);
                    }
                    else
                    {
                        PathPoints.DoLayoutList();
                    }
                }
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 有效路径百分比
            XGUI.layout_group_start(
               type: XGUIContainerType.Vertical,
               bg_fill: XGUIFilled.缺口纯色边框,
               bg_color: XGUIColor.亮白,
               bg_color_gui: XTween_Dashboard.Theme_Group,
               title: "有效路径百分比",
               title_size: XGUIFontSize.M,
               title_text_color: XTween_Dashboard.Theme_Primary,
               title_clipping: TextClipping.Clip,
               padding: new RectOffset(5, 5, 10, 10));

            XGUI.layout_progress(
                height: 30,
                title: "起点",
                title_size: XGUIFontSize.XS,
                title_offset: new Vector2(0, 0),
                title_color: Color.white,
                subtitle: "终点",
                subtitle_size: XGUIFontSize.XS,
                subtitle_offset: new Vector2(0, 0),
                subtitle_color: Color.white,
                line_left_color: Color.white * 0.5f,
                line_right_color: Color.white * 0.5f,
                line_center_color: Color.white * 0.5f,
                progress_fg_color: XTween_Dashboard.Theme_Primary,
                progress_bg_color: Color.black * 0.5f,
                indicator_color: Color.white,
                icon_indicator: XGUI.GetBasedIcon("icon_mark_arrow_up"),
                value: sp_PathLimitePercent.floatValue,
                title_distance: 0,
                thickness: 2);

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 选项
            BaseScript.fold_options = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XTween_Dashboard.Theme_Group,
                title: "选项",
                title_size: XGUIFontSize.M,
                title_text_color: XTween_Dashboard.Theme_Primary,
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 5, 15, 15),
                foldout: BaseScript.fold_options);

            if (!BaseScript.fold_options)
            {
                #region 动画类型           
                XGUI.layout_string_popup(
                   title: "路径样式",
                   title_width: 100,
                   title_size: XGUIFontSize.M,
                   title_anchor: TextAnchor.MiddleLeft,
                   prop: sp_IndexPathType,
                   options: enums_name_path_type,
                   opt_text_size: XGUIFontSize.M,
                   opt_text_color: Color.black,
                   opt_text_padding: new RectOffset(10, 10, 0, 0),
                   opt_anchor: TextAnchor.MiddleLeft,
                   opt_font_style: FontStyle.Normal,
                   opt_bg_fill: XGUIFilled.实体,
                   opt_bg_color: XGUIColor.亮白,
                   opt_bg_color_gui: XTween_Dashboard.Theme_Primary,
                   icon_arrow_color: Color.black,
                   margin: new RectOffset(0, 0, 5, 5),
                   padding: new RectOffset(5, 5, 0, 0),
                   title_margin: new RectOffset(0, 0, 0, 0),
                   act_on_changed: (value) =>
                   {
                       sp_PathType.enumValueIndex = (int)(XTween_PathType)System.Enum.Parse(typeof(XTween_PathType), value);
                   });
                #endregion

                #region 朝向模式           
                XGUI.layout_string_popup(
                   title: "朝向模式",
                   title_width: 100,
                   title_size: XGUIFontSize.M,
                   title_anchor: TextAnchor.MiddleLeft,
                   prop: sp_IndexPathOrientation,
                   options: enums_name_path_orient,
                   opt_text_size: XGUIFontSize.M,
                   opt_text_color: Color.black,
                   opt_text_padding: new RectOffset(10, 10, 0, 0),
                   opt_anchor: TextAnchor.MiddleLeft,
                   opt_font_style: FontStyle.Normal,
                   opt_bg_fill: XGUIFilled.实体,
                   opt_bg_color: XGUIColor.亮白,
                   opt_bg_color_gui: XTween_Dashboard.Theme_Primary,
                   icon_arrow_color: Color.black,
                   margin: new RectOffset(0, 0, 5, 5),
                   padding: new RectOffset(5, 5, 0, 0),
                   title_margin: new RectOffset(0, 0, 0, 0),
                   act_on_changed: (value) =>
                   {
                       sp_PathOrientation.enumValueIndex = (int)(XTween_PathOrientation)System.Enum.Parse(typeof(XTween_PathOrientation), value);
                   });
                #endregion

                #region 朝向轴向           
                if (BaseScript.PathOrientation != XTween_PathOrientation.无)
                {
                    XGUI.layout_string_popup(
                   title: "朝向轴向",
                   title_width: 100,
                   title_size: XGUIFontSize.M,
                   title_anchor: TextAnchor.MiddleLeft,
                   prop: sp_IndexPathOrientationVector,
                   options: enums_name_path_orient_vector,
                   opt_text_size: XGUIFontSize.M,
                   opt_text_color: Color.black,
                   opt_text_padding: new RectOffset(10, 10, 0, 0),
                   opt_anchor: TextAnchor.MiddleLeft,
                   opt_font_style: FontStyle.Normal,
                   opt_bg_fill: XGUIFilled.实体,
                   opt_bg_color: XGUIColor.亮白,
                   opt_bg_color_gui: XTween_Dashboard.Theme_Primary,
                   icon_arrow_color: Color.black,
                   margin: new RectOffset(0, 0, 5, 5),
                   padding: new RectOffset(5, 5, 0, 0),
                   title_margin: new RectOffset(0, 0, 0, 0),
                   act_on_changed: (value) =>
                   {
                       sp_PathOrientationVector.enumValueIndex = (int)(XTween_PathOrientationVector)System.Enum.Parse(typeof(XTween_PathOrientationVector), value);
                   });
                }
                #endregion

                #region 控制点样式           
                XGUI.layout_string_popup(
                   title: "控制点样式",
                   title_width: 100,
                   title_size: XGUIFontSize.M,
                   title_anchor: TextAnchor.MiddleLeft,
                   prop: sp_IndexControlLineStyle,
                   options: enums_name_line_style,
                   opt_text_size: XGUIFontSize.M,
                   opt_text_color: Color.black,
                   opt_text_padding: new RectOffset(10, 10, 0, 0),
                   opt_anchor: TextAnchor.MiddleLeft,
                   opt_font_style: FontStyle.Normal,
                   opt_bg_fill: XGUIFilled.实体,
                   opt_bg_color: XGUIColor.亮白,
                   opt_bg_color_gui: XTween_Dashboard.Theme_Primary,
                   icon_arrow_color: Color.black,
                   margin: new RectOffset(0, 0, 5, 5),
                   padding: new RectOffset(5, 5, 0, 0),
                   title_margin: new RectOffset(0, 0, 0, 0),
                   act_on_changed: (value) =>
                   {
                       sp_ControlLineStyle.enumValueIndex = (int)(XTween_LineStyle)System.Enum.Parse(typeof(XTween_LineStyle), value);
                   });
                #endregion

                #region 生成标记方式           
                XGUI.layout_string_popup(
                   title: "生成标记方式",
                   title_width: 100,
                   title_size: XGUIFontSize.M,
                   title_anchor: TextAnchor.MiddleLeft,
                   prop: sp_IndexPathMarkMode,
                   options: enums_name_path_marks,
                   opt_text_size: XGUIFontSize.M,
                   opt_text_color: Color.black,
                   opt_text_padding: new RectOffset(10, 10, 0, 0),
                   opt_anchor: TextAnchor.MiddleLeft,
                   opt_font_style: FontStyle.Normal,
                   opt_bg_fill: XGUIFilled.实体,
                   opt_bg_color: XGUIColor.亮白,
                   opt_bg_color_gui: XTween_Dashboard.Theme_Primary,
                   icon_arrow_color: Color.black,
                   margin: new RectOffset(0, 0, 5, 5),
                   padding: new RectOffset(5, 5, 0, 0),
                   title_margin: new RectOffset(0, 0, 0, 0),
                   act_on_changed: (value) =>
                   {
                       sp_PathMarksMode.enumValueIndex = (int)(XTween_PathMarksMode)System.Enum.Parse(typeof(XTween_PathMarksMode), value);
                   });
                #endregion

                XGUI.layout_seperator(
                    thickness: 1,
                    color: XTween_Dashboard.Theme_SeperateLine,
                    margin: new RectOffset(0, 0, 15, 13),
                    padding: new RectOffset(0, 0, 0, 0));

                // 调试信息
                DrawToggle(
                    title: "路径闭合",
                    prop: sp_IsClosed,
                    width: 120,
                    options: new string[] { "闭合", "开放" },
                    act_on_changed: (b) =>
                {

                });

                // 显示路径
                DrawToggle(
                    title: "显示路径",
                    prop: sp_DisplayPath,
                    width: 120,
                    options: new string[] { "闭合", "开放" },
                    act_on_changed: (b) =>
                    {

                    });

                // 显示路径点信息
                DrawToggle(
                    title: "显示路径点信息",
                    prop: sp_DisplayIndex,
                    width: 120,
                    options: new string[] { "开放", "闭合" },
                    act_on_changed: (b) =>
                    {

                    });

                // 注视线可视化
                DrawToggle(
                    title: "注视线可视化",
                    prop: sp_LookAtLine,
                    width: 120,
                    options: new string[] { "闭合", "开放" },
                    act_on_changed: (b) =>
                    {

                    });
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 注视参数
            if (BaseScript.PathOrientation == XTween_PathOrientation.注视目标物体 || BaseScript.PathOrientation == XTween_PathOrientation.注视目标位置)
            {
                XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XTween_Dashboard.Theme_Group,
                title: "注视参数",
                title_size: XGUIFontSize.M,
                title_text_color: XTween_Dashboard.Theme_Primary,
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 5, 15, 15));

                if (BaseScript.PathOrientation == XTween_PathOrientation.注视目标物体)
                {
                    DrawParamField("目标物体", sp_LookAtObject, 100);
                }

                if (BaseScript.PathOrientation == XTween_PathOrientation.注视目标位置)
                {
                    DrawParamField("目标位置", sp_LookAtPosition, 100);
                }

                XGUI.SetEnabled(false);
                DrawParamField("连线坐标数组", sp_LookAtPoints, 100);
                XGUI.SetEnabled(true);

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            }
            #endregion

            #region 路径状态
            BaseScript.fold_status = XGUI.layout_group_start(
                   type: XGUIContainerType.Vertical,
                   bg_fill: XGUIFilled.缺口纯色边框,
                   bg_color: XGUIColor.亮白,
                   bg_color_gui: XTween_Dashboard.Theme_Group,
                   title: "路径状态",
                   title_size: XGUIFontSize.M,
                   title_text_color: XTween_Dashboard.Theme_Primary,
                   title_clipping: TextClipping.Clip,
                   padding: new RectOffset(10, 5, 15, 15),
                   foldout: BaseScript.fold_status);

            if (BaseScript.fold_status)
            {
                XGUI.layout_state_displayer_text(
                    title: "世界状态",
                    title_size: XGUIFontSize.M,
                    subtitle: sp_IsWorldMode.boolValue ? "启用" : "禁用",
                    subtitle_size: XGUIFontSize.M,
                    subtitle_color: Color.white * 0.7f,
                    padding: new RectOffset(0, 0, 0, 0));

                XGUI.layout_state_displayer_text(
                    title: "初始坐标",
                    title_size: XGUIFontSize.M,
                    subtitle: sp_StartPosition.vector3Value.ToString(),
                    subtitle_size: XGUIFontSize.M,
                    subtitle_color: XTween_Dashboard.Theme_Primary,
                    padding: new RectOffset(0, 0, 0, 0));

                XGUI.layout_state_displayer_text(
                    title: "路径长度",
                    title_size: XGUIFontSize.M,
                    subtitle: sp_PathLength.floatValue.ToString("F2"),
                    subtitle_size: XGUIFontSize.M,
                    subtitle_color: XTween_Dashboard.Theme_Primary,
                    padding: new RectOffset(0, 0, 0, 0));

                XGUI.layout_property_field(
                    title: "路径父级物体",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XTween_Dashboard.Theme_Primary,
                    title_width: 90,
                    status_icon: "icon_field_status",
                    status_icon_color: sp_PathParent.objectReferenceValue != null ? XTween_Dashboard.Theme_Primary : Color.black * 0.7f,
                    prop: sp_PathParent,
                    prop_margin: new RectOffset(0, 0, 0, 10));
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 路径参数
            BaseScript.fold_params = XGUI.layout_group_start(
            type: XGUIContainerType.Vertical,
            bg_fill: XGUIFilled.缺口纯色边框,
            bg_color: XGUIColor.亮白,
            bg_color_gui: XTween_Dashboard.Theme_Group,
            title: "路径参数",
            title_size: XGUIFontSize.M,
            title_text_color: XTween_Dashboard.Theme_Primary,
            title_clipping: TextClipping.Clip,
            padding: new RectOffset(10, 5, 15, 15),
            foldout: BaseScript.fold_params);

            if (BaseScript.fold_params)
            {
                DrawParamField("有效路径百分比", sp_PathLimitePercent, 100);
                DrawParamField("路径步数细分", sp_SegmentsPerCurve, 100);
                DrawParamField("添加路径初始距离", sp_AddedDistance, 100);
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 路径样式
            BaseScript.fold_style = XGUI.layout_group_start(
                   type: XGUIContainerType.Vertical,
                   bg_fill: XGUIFilled.缺口纯色边框,
                   bg_color: XGUIColor.亮白,
                   bg_color_gui: XTween_Dashboard.Theme_Group,
                   title: "路径样式",
                   title_size: XGUIFontSize.M,
                   title_text_color: XTween_Dashboard.Theme_Primary,
                   title_clipping: TextClipping.Clip,
                   padding: new RectOffset(10, 5, 15, 15),
                   foldout: BaseScript.fold_style);

            if (BaseScript.fold_style)
            {
                DrawParamField("路径点尺寸", sp_PathPointSize, 100);
                DrawParamField("贝塞尔控制点尺寸", sp_BezierControlSize, 100);
                DrawParamField("路径宽度", sp_PathWidth, 100);
                DrawParamField("路径信息尺寸", sp_IndexSize, 100);
                DrawParamField("路径信息高度", sp_IndexLengthHeight, 100);
                DrawParamField("路径信息偏移", sp_IndexOffset, 100);
                DrawParamField("注视线宽度", sp_LookAtLineWidth, 100);
                DrawParamField("路径颜色", sp_Color_Path, 100);
                DrawParamField("路径点颜色", sp_Color_PathPoint, 100);
                DrawParamField("路径点选中颜色", sp_Color_PathPoint_Selected, 100);
                DrawParamField("控制点颜色", sp_Color_BezierControl, 100);
                DrawParamField("控制点选中颜色", sp_Color_BezierControl_Selected, 100);
                DrawParamField("路径点序号颜色", sp_Color_Index, 100);
                DrawParamField("路径点信息颜色", sp_Color_IndexLength, 100);
                DrawParamField("注视线颜色", sp_Color_LookAtLine, 100);
                DrawParamField("标记颜色", sp_PathMarksColor, 100);
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 路径标记生成
            BaseScript.fold_marks = XGUI.layout_group_start(
                   type: XGUIContainerType.Vertical,
                   bg_fill: XGUIFilled.缺口纯色边框,
                   bg_color: XGUIColor.亮白,
                   bg_color_gui: XTween_Dashboard.Theme_Group,
                   title: "路径标记生成",
                   title_size: XGUIFontSize.M,
                   title_text_color: XTween_Dashboard.Theme_Primary,
                   title_clipping: TextClipping.Clip,
                   padding: new RectOffset(10, 5, 15, 15),
                   foldout: BaseScript.fold_marks);

            if (BaseScript.fold_marks)
            {
                DrawParamField("标记样式", sp_PathMarksTexture, 100);
                DrawParamField("标记尺寸", sp_PathMarksSize, 100);
                DrawParamField("标记采样", sp_PathMarksSample, 100);
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            Event e = Event.current;
            if (e.type == EventType.MouseDown && e.button == 1)
            {
                // 创建右键菜单
                GenericMenu menu = new GenericMenu();
                menu.AddDisabledItem(new GUIContent("路径点"));
                menu.AddItem(new GUIContent("A (添加路径点)"), false, () =>
                {
                    PathPoints_Add();
                    return;
                });
                menu.AddItem(new GUIContent("X (清空路径点)"), false, () =>
                {
                    PathPoints_Clear();
                    return;
                });
                menu.AddSeparator("");
                menu.AddDisabledItem(new GUIContent("标记路径"));
                if (sp_PathMarksGroup.objectReferenceValue == null)
                {
                    menu.AddItem(new GUIContent("D (标记)"), false, () =>
                    {
                        string res = XGUI.dialog(
                            type: XGUIDialogType.警告,
                            windowtitle: "XTweenPath消息",
                            title: "创建标记物",
                            msg: "此操作会根据当前的路径生成标记物！",
                            ok: "创建",
                            cancel: "暂不",
                            PrimaryIndex: 0);
                        if (res == "创建")
                        {
                            BaseScript.PathMarks_Create();
                        }
                    });
                }
                else
                {
                    menu.AddItem(new GUIContent("D (清除)"), false, () =>
                    {
                        string res_de = XGUI.dialog(type: XGUIDialogType.警告, windowtitle: "XTweenPath消息", title: "已存在标记物", msg: "此操作会将当前的所有路径标记物全部清除！", ok: "清除", cancel: "暂不", PrimaryIndex: 0);
                        if (res_de == "清除")
                        {
                            BaseScript.PathMarks_Clear();
                        }
                    });
                }
                menu.AddSeparator("");
                menu.AddDisabledItem(new GUIContent("坐标"));
                menu.AddItem(new GUIContent("R (路径点Z轴归零)"), false, () =>
                {
                    PathPoints_ZAxis_ToZero();
                });
                menu.ShowAsContext(); // 在鼠标位置显示右键菜单
                e.Use();
            }

            serializedObject.ApplyModifiedProperties();

            #region 原始类
            fold_raw = XGUI.layout_group_start(
                 type: XGUIContainerType.Vertical,
                 bg_fill: XGUIFilled.缺口纯色边框,
                 bg_color: XGUIColor.亮白,
                 bg_color_gui: XTween_Dashboard.Theme_Group,
                 title: "原始类",
                 title_size: XGUIFontSize.M,
                 title_text_color: XTween_Dashboard.Theme_Primary,
                 title_clipping: TextClipping.Clip,
                 padding: new RectOffset(10, 5, 15, 15),
                 foldout: fold_raw);

            if (fold_raw)
            {
                DrawDefaultInspector();
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion
        }

        #region 路径点
        /// <summary>
        /// 增加路径点
        /// </summary>
        private void PathPoints_Add()
        {
            Undo.RecordObject(BaseScript, "Add Path Point");

            // 创建一个点位，如果路径点列表不是空的，那么就获取最后的点的坐标向右平移 0.3 单位的距离创建新点位，否则就创建在脚本物体的位置
            Vector3 InitialPosition = BaseScript.PathPoints.Count > 0 ? BaseScript.Get_WorldPosition(BaseScript.PathPoints.Count - 1) + Vector3.right * BaseScript.AddedDistance : BaseScript.transform.position;

            // 初始化一个点位的贝塞尔曲线类
            Vector3 LocalPoint = BaseScript.transform.InverseTransformPoint(InitialPosition);
            var newPoint = new XTween_BezierPathPoint(LocalPoint, Vector3.zero, Vector3.zero);

            // 如果当前路径模式为 手动贝塞尔 或 平衡贝塞尔 那么切线点要做相应的调整
            //if (BaseScript.XTween_PathType == XTween_PathType.手动贝塞尔 || BaseScript.XTween_PathType == XTween_PathType.平衡贝塞尔)
            //{

            // 首先算出距离：0.2 除以自身的 正向X轴 轴缩放
            float distance = 0.1f / BaseScript.transform.lossyScale.x;
            // 入点向左移动 distance 距离
            newPoint.bezier_in = Vector3.left * distance;

            // 如果是 Balance 模式那么出点为入点的反向位置，否则向右移动 distance 距离
            newPoint.bezier_out = BaseScript.PathType == XTween_PathType.平衡贝塞尔 ? -newPoint.bezier_in : Vector3.right * distance;

            //}

            // 将新建的路径点加入列表中
            BaseScript.PathPoints.Add(newPoint);
            // 选中创建的点位
            selectedAnchorIndex = BaseScript.PathPoints.Count - 1;
        }
        /// <summary>
        /// 清空路径点
        /// </summary>
        private void PathPoints_Clear()
        {
            string res = XGUI.dialog(
                type: XGUIDialogType.警告,
                windowtitle: "XTweenPath消息",
                title: "清空路径点",
                msg: "此操作将清空当前已设置的所有路径点！请谨慎操作！",
                ok: "清空",
                cancel: "暂不",
                PrimaryIndex: 1,
                themecolor: XTween_Dashboard.Theme_Primary);
            if (res == "清空")
            {
                Undo.RecordObject(BaseScript, "Clear All Path Points");
                BaseScript.PathPoints.Clear();
                selectedAnchorIndex = -1;
                selectedControlIndex = -1;
            }
        }
        /// <summary>
        /// 将所有路径点的Z轴归零到画布
        /// </summary>
        private void PathPoints_ZAxis_ToZero()
        {
            string res = XGUI.dialog(
                type: XGUIDialogType.警告,
                windowtitle: "XTweenPath消息",
                title: "路径点Z轴归零",
                msg: "此操作会将当前已设置的所有路径点的Z轴坐标归零到画布！请谨慎操作！",
                ok: "归零",
                cancel: "暂不",
                PrimaryIndex: 1,
                themecolor: XTween_Dashboard.Theme_Primary);
            if (res == "归零")
            {
                Undo.RecordObject(BaseScript, "Path Points ResetToZero");
                for (int i = 0; i < BaseScript.PathPoints.Count; i++)
                {
                    BaseScript.PathPoints[i].relative.z = 0;
                }
            }
        }
        #endregion

        /// <summary>
        /// 标记点创建/清除
        /// </summary>
        private void PathMarksCreator()
        {
            if (sp_PathMarksGroup.objectReferenceValue != null)
            {
                string res_de = XGUI.dialog(
                    type: XGUIDialogType.警告,
                    windowtitle: "XTweenPath消息",
                    title: "已存在标记物",
                    msg: "此操作会将当前的所有路径标记物全部清除！",
                    ok: "清除",
                    cancel: "暂不",
                    PrimaryIndex: 0,
                    themecolor: XTween_Dashboard.Theme_Primary);
                if (res_de == "清除")
                {
                    BaseScript.PathMarks_Clear();
                }
            }
            else
            {
                string res = XGUI.dialog(
                    type: XGUIDialogType.警告,
                    windowtitle: "XTweenPath消息",
                    title: "创建标记物",
                    msg: "此操作会根据当前的路径生成标记物！",
                    ok: "创建",
                    cancel: "暂不",
                    PrimaryIndex: 0,
                    themecolor: XTween_Dashboard.Theme_Primary);
                if (res == "创建")
                {
                    BaseScript.PathMarks_Create();
                }
            }
        }

        /// <summary>
        /// 在场景视图中绘制路径和路径点
        /// </summary>
        private void DuringSceneGUI(SceneView sceneView)
        {
            if (BaseScript.PathPoints == null)
                return;

            // 根据脚本物体转换出对应的 world 和 anchored 位置信息
            if (!BaseScript.IsWorldMode)
                BaseScript.ConvertPositions();

            // 计算路径总长
            BaseScript.PathLength = BaseScript.CalculateTotalLength();

            // 绘制路径点和线的可视化
            DrawPath();

            Event e = Event.current;

            for (int i = 0; i < BaseScript.PathPoints.Count; i++)
            {
                DrawPoint_Path(i, e);
                DrawPoints_TangentControl(i, e);

                if (BaseScript.DisplayPath && BaseScript.DisplayIndex)
                {
                    // 在每个锚点旁边绘制序号
                    Vector3 pos = BaseScript.Get_WorldPosition(i);
                    // 字体设置
                    IndexStyle.fontSize = BaseScript.IndexSize;
                    IndexStyle.normal.textColor = BaseScript.Color_Index;
                    IndexStyle.alignment = TextAnchor.MiddleCenter;
                    IndexStyle.contentOffset = new Vector2(BaseScript.IndexOffset.x, 1 - BaseScript.IndexOffset.y - 1);
                    // 显示路径点序号
                    Handles.Label(pos, $"X {i}", IndexStyle);

                    // 如果不是最后一个点，绘制线段长度
                    if (i < BaseScript.PathPoints.Count - 1 || BaseScript.IsClosed)
                    {
                        // 计算当前线段的长度
                        int nextIndex = (i + 1) % BaseScript.PathPoints.Count;
                        float segmentLength = BaseScript.CalculateSegmentLength(i, nextIndex, 10);

                        // 计算线段中点位置
                        Vector3 midPoint = (BaseScript.Get_WorldPosition(i) + BaseScript.Get_WorldPosition(nextIndex)) * 0.5f;

                        IndexStyle.fontSize = BaseScript.IndexSize - 2;
                        IndexStyle.normal.textColor = BaseScript.Color_IndexLength;
                        IndexStyle.contentOffset = new Vector2(BaseScript.IndexOffset.x, 1 - BaseScript.IndexOffset.y - 1 - BaseScript.IndexLengthHeight);

                        float per = segmentLength / BaseScript.PathLength * 100;
                        // 显示线段长度（使用更友好的单位）
                        Handles.Label(pos, $"{segmentLength:F2} / {per:F1}%", IndexStyle);
                    }
                }
            }

            PathPointKeyboardEdit(e);

            if (BaseScript.act_on_pathChanged != null)
                BaseScript.act_on_pathChanged?.Invoke();
        }

        #region 绘制类
        /// <summary>
        /// 绘制路径
        /// </summary>
        private void DrawPath()
        {
            if (!BaseScript.DisplayPath)
                return;
            if (!BaseScript.gameObject.activeSelf)
                return;
            if (BaseScript.PathPoints.Count < 2)
                return;
            Handles.color = BaseScript.Color_Path;

            switch (BaseScript.PathType)
            {
                case XTween_PathType.线性:
                    Vector3[] linearPoints = new Vector3[BaseScript.PathPoints.Count + (BaseScript.IsClosed ? 1 : 0)];
                    for (int i = 0; i < BaseScript.PathPoints.Count; i++)
                    {
                        // 获取路径点（世界坐标）
                        linearPoints[i] = BaseScript.Get_WorldPosition(i);
                    }
                    if (BaseScript.IsClosed)
                    {
                        linearPoints[BaseScript.PathPoints.Count] = BaseScript.Get_WorldPosition(0);
                    }
                    // DrawAAPolyLine 填入的位置参数是世界坐标
                    Handles.DrawAAPolyLine(BaseScript.PathWidth, linearPoints);
                    break;
                case XTween_PathType.自动贝塞尔:
                    for (int i = 0; i < BaseScript.PathPoints.Count - 1; i++)
                    {
                        Vector3 start_before_tangent = (i > 0) ? BaseScript.Get_WorldPosition(i - 1) : (BaseScript.IsClosed ? BaseScript.Get_WorldPosition(BaseScript.PathPoints.Count - 1) : BaseScript.Get_WorldPosition(i));
                        Vector3 start = BaseScript.Get_WorldPosition(i);
                        Vector3 end = BaseScript.Get_WorldPosition(i + 1);
                        Vector3 end_after_tangent = (i < BaseScript.PathPoints.Count - 2) ? BaseScript.Get_WorldPosition(i + 2) : (BaseScript.IsClosed ? BaseScript.Get_WorldPosition((i + 2) % BaseScript.PathPoints.Count) : end);

                        Vector3 tangent_start = 0.5f * (end - start_before_tangent);
                        Vector3 tangent_end = 0.5f * (end_after_tangent - start);

                        // DrawBezier 的参数均为世界坐标数值
                        Handles.DrawBezier(start, end, start + tangent_start * 0.3f, end - tangent_end * 0.3f, BaseScript.Color_Path, null, BaseScript.PathWidth);
                    }

                    // 绘制闭合段
                    if (BaseScript.IsClosed && BaseScript.PathPoints.Count > 2)
                    {
                        int i = BaseScript.PathPoints.Count - 1;
                        Vector3 start_before_tangent = BaseScript.Get_WorldPosition(i - 1);
                        Vector3 start = BaseScript.Get_WorldPosition(i);
                        Vector3 end = BaseScript.Get_WorldPosition(0);
                        Vector3 end_after_tangent = BaseScript.Get_WorldPosition(1);

                        Vector3 tangent_start = 0.5f * (end - start_before_tangent);
                        Vector3 tangent_end = 0.5f * (end_after_tangent - start);

                        // DrawBezier 的参数均为世界坐标数值
                        Handles.DrawBezier(
                            start, end,
                            start + tangent_start * 0.3f,
                            end - tangent_end * 0.3f,
                            BaseScript.Color_Path,
                            null,
                            BaseScript.PathWidth
                        );
                    }
                    break;
                case XTween_PathType.手动贝塞尔:
                case XTween_PathType.平衡贝塞尔:
                    for (int i = 0; i < BaseScript.PathPoints.Count - 1; i++)
                    {
                        // DrawBezier 的参数均为世界坐标数值
                        Handles.DrawBezier(
                            BaseScript.Get_WorldPosition(i),
                            BaseScript.Get_WorldPosition(i + 1),
                            BaseScript.Get_Out_Position(i),
                            BaseScript.Get_In_Position(i + 1),
                            BaseScript.Color_Path,
                            null,
                            BaseScript.PathWidth
                        );
                    }

                    // 绘制闭合段
                    if (BaseScript.IsClosed && BaseScript.PathPoints.Count > 2)
                    {
                        int i = BaseScript.PathPoints.Count - 1;
                        // DrawBezier 的参数均为世界坐标数值
                        Handles.DrawBezier(
                            BaseScript.Get_WorldPosition(i),
                            BaseScript.Get_WorldPosition(0),
                            BaseScript.Get_Out_Position(i),
                            BaseScript.Get_In_Position(0),
                            BaseScript.Color_Path,
                            null,
                            BaseScript.PathWidth
                        );
                    }
                    break;
            }
        }
        /// <summary>
        /// 绘制路径点
        /// </summary>
        /// <param name="index">路径点的索引</param>
        /// param name="e">当前事件</param>
        private void DrawPoint_Path(int index, Event e)
        {
            if (!BaseScript.DisplayPath)
                return;
            if (!BaseScript.gameObject.activeSelf)
                return;
            if (BaseScript.IsWorldMode)
                return;

            Vector3 w_pos = BaseScript.Get_WorldPosition(index);

            bool isSelected = (index == selectedAnchorIndex);
            Handles.color = isSelected ? BaseScript.Color_PathPoint_Selected : BaseScript.Color_PathPoint;

            float handleSize = HandleUtility.GetHandleSize(w_pos);
            float screenSize = handleSize * (BaseScript.PathPointSize * 0.1f) * 0.5f; // 关键调整：乘以0.5使视觉尺寸匹配碰撞检测

            Handles.SphereHandleCap(
                0,
                w_pos,
                Quaternion.identity,
                screenSize,
                EventType.Repaint
            );

            int anchorControlID = GUIUtility.GetControlID(FocusType.Passive);
            if (e.type == EventType.Layout)
            {
                HandleUtility.AddControl(anchorControlID, HandleUtility.DistanceToCircle(w_pos, screenSize));
            }

            if (e.type == EventType.MouseDown && e.button == 0 && !e.alt && !e.control && !e.shift && HandleUtility.nearestControl == anchorControlID)
            {
                selectedAnchorIndex = index;
                selectedControlIndex = -1;
                GUIUtility.hotControl = anchorControlID;
                e.Use();
            }

            if (isSelected)
            {
                EditorGUI.BeginChangeCheck();
                // 通过拖动路径点产生新位置的世界坐标空间的位置值
                Vector3 handle_pos = Handles.PositionHandle(w_pos, Quaternion.identity);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(BaseScript, "MovePathPoint");
                    // 拖动路径点的时候将点的世界坐标空间转换到 relative 局部坐标空间值中
                    BaseScript.PathPoints[index].relative = BaseScript.transform.InverseTransformPoint(handle_pos);
                    BaseScript.ConvertPositions();
                }
            }
        }
        /// <summary>
        /// 绘制切线控制点
        /// </summary>
        /// <param name="index">路径点的索引</param>
        /// <param name="e">当前事件</param>
        private void DrawPoints_TangentControl(int index, Event e)
        {
            if (!BaseScript.DisplayPath)
                return;
            if (!BaseScript.gameObject.activeSelf)
                return;
            if (BaseScript.IsWorldMode)
                return;
            if (BaseScript.PathType != XTween_PathType.手动贝塞尔 && BaseScript.PathType != XTween_PathType.平衡贝塞尔)
                return;

            // 绘制入控制点(对除第一个点外的所有点)
            if (index > 0 || (BaseScript.IsClosed && index == 0))
            {
                DrawPoint_Tangent(
                    index,
                    BaseScript.Get_In_Position(index),
                    isInControl: true,
                    e
                );
            }

            // 绘制出控制点(对除最后一个点外的所有点，或闭合时的最后一个点)
            if (index < BaseScript.PathPoints.Count - 1 || (BaseScript.IsClosed && index == BaseScript.PathPoints.Count - 1))
            {
                DrawPoint_Tangent(
                    index,
                    BaseScript.Get_Out_Position(index),
                    isInControl: false,
                    e
                );
            }
        }
        /// <summary>
        /// 绘制切线控制点
        /// </summary>
        /// <param name="index">路径点的索引</param>
        /// <param name="pos">切线控制点的位置</param>
        /// <param name="isInControl">是否是入控制点</param>
        /// <param name="e">当前事件</param>
        private void DrawPoint_Tangent(int index, Vector3 pos, bool isInControl, Event e)
        {
            if (!BaseScript.gameObject.activeSelf)
                return;

            bool isSelected = (index == selectedControlIndex) && ((isInControl && this.isInControl) || (!isInControl && !this.isInControl));

            Handles.color = isSelected ? BaseScript.Color_BezierControl_Selected : BaseScript.Color_BezierControl;

            Vector3 anchorPos = BaseScript.Get_WorldPosition(index);
            if (BaseScript.ControlLineStyle == XTween_LineStyle.虚线)
                Handles.DrawDottedLine(anchorPos, pos, 2f);
            else
                Handles.DrawAAPolyLine(anchorPos, pos);

            float handleSize = HandleUtility.GetHandleSize(anchorPos);
            float screenSize = handleSize * (BaseScript.BezierControlSize * 0.1f) * 0.5f; // 关键调整：乘以0.5使视觉尺寸匹配碰撞检测

            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            if (e.type == EventType.Layout)
            {
                HandleUtility.AddControl(controlID, HandleUtility.DistanceToCircle(pos, screenSize));
            }

            // 切线控制点
            Handles.SphereHandleCap(controlID, pos, Quaternion.identity, screenSize, EventType.Repaint);

            if (e.type == EventType.MouseDown && e.button == 0 && !e.alt && !e.control && !e.shift && HandleUtility.nearestControl == controlID)
            {
                selectedControlIndex = index;
                selectedAnchorIndex = -1;
                this.isInControl = isInControl;
                GUIUtility.hotControl = controlID;
                e.Use();
            }

            if (isSelected)
            {
                Tools.current = Tool.Move;

                EditorGUI.BeginChangeCheck();
                Vector3 pos_world = Handles.PositionHandle(pos, Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(BaseScript, "Move Control Point");
                    Vector3 localAnchor = BaseScript.PathPoints[index].relative;
                    Vector3 localPos = BaseScript.transform.InverseTransformPoint(pos_world);

                    if (BaseScript.PathType == XTween_PathType.平衡贝塞尔)
                    {
                        if (isInControl)
                        {
                            BaseScript.PathPoints[index].bezier_in = localPos - localAnchor;
                            BaseScript.PathPoints[index].bezier_out = -BaseScript.PathPoints[index].bezier_in;
                        }
                        else
                        {
                            BaseScript.PathPoints[index].bezier_out = localPos - localAnchor;
                            BaseScript.PathPoints[index].bezier_in = -BaseScript.PathPoints[index].bezier_out;
                        }
                    }
                    else
                    {
                        if (isInControl)
                        {
                            BaseScript.PathPoints[index].bezier_in = localPos - localAnchor;
                        }
                        else
                        {
                            BaseScript.PathPoints[index].bezier_out = localPos - localAnchor;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 通用方法：绘制开关
        /// </summary>
        private void DrawToggle(string title = null, SerializedProperty prop = null, float width = 100, string[] options = null, Action<bool> act_on_changed = null)
        {
            XGUI.layout_toggle(
                title: title,
                title_size: XGUIFontSize.M,
                title_font_style: FontStyle.Normal,
                title_padding: new RectOffset(0, 10, 0, 0),
                title_width: width,
                prop: prop,
                tog_style: XGUIToggleStyle.实体,
                tog_padding: new RectOffset(5, 8, 0, 0),
                tog_margin: new RectOffset(0, 0, 0, 5),
                tog_mixed_options: options == null ? new string[] { "禁用", "启用" } : options,
                tog_mixed_text_size: XGUIFontSize.M,
                tog_mixed_text_color: Color.black,
                tog_mixed_text_padding: new RectOffset(10, 10, 0, 0),
                tog_mixed_text_anchor: TextAnchor.MiddleCenter,
                tog_mixed_font_style: FontStyle.Normal,
                tog_bg_off_color: new Color(0.38f, 0.38f, 0.38f),
                tog_bg_on_color: XTween_Dashboard.Theme_Primary,
                tog_handler_off_color: Color.white,
                tog_handler_on_color: Color.white,
                tog_mixed_bg_color_gui: XTween_Dashboard.Theme_Primary,
                act_on_changed: act_on_changed);
        }
        /// <summary>
        /// 通用方法：绘制参数
        /// </summary>
        private void DrawParamField(string title, SerializedProperty prop, float width)
        {
            XGUI.layout_property_field(
                title: title,
                title_size: XGUIFontSize.M,
                title_hover_color: XTween_Dashboard.Theme_Primary,
                title_width: width,
                //status_icon: "icon_field_status",
                //status_icon_color: Color.green,
                prop: prop,
                prop_margin: new RectOffset(0, 0, 0, 5));
        }
        private void DrawPointInfo(Rect rect, string info, TextClipping clipping)
        {
            XGUI.gui_label(
                rect: rect,
                text: new GUIContent(info),
                text_color: Color.white,
                size: XGUIFontSize.S,
                clipping: clipping,
                anchor: TextAnchor.MiddleLeft,
                offset: new Vector2(0, 0),
                font_style: FontStyle.Normal,
                font: XGUI.GetFont("xg-regular"));
        }
        #endregion

        /// <summary>
        /// 对路径点进行键盘操作，按下 Delete 键时删除选中的路径点或重置控制点
        /// </summary>
        /// <param name="e">当前事件</param>
        private void PathPointKeyboardEdit(Event e)
        {
            if (!BaseScript.gameObject.activeSelf)
                return;
            if (BaseScript.IsWorldMode)
                return;

            if (e.type == EventType.KeyDown)
            {
                if (e.keyCode == KeyCode.Delete)
                {
                    if (selectedAnchorIndex >= 0)
                    {
                        Undo.RecordObject(BaseScript, "Delete Point");
                        BaseScript.PathPoints.RemoveAt(selectedAnchorIndex);
                        selectedAnchorIndex = Mathf.Clamp(selectedAnchorIndex, 0, BaseScript.PathPoints.Count - 1);
                        e.Use();
                    }
                    else if (selectedControlIndex >= 0)
                    {
                        Undo.RecordObject(BaseScript, "Reset Control Point");
                        if (isInControl)
                        {
                            BaseScript.PathPoints[selectedControlIndex].bezier_in = Vector3.zero;
                            if (BaseScript.PathType == XTween_PathType.平衡贝塞尔)
                                BaseScript.PathPoints[selectedControlIndex].bezier_out = Vector3.zero;
                        }
                        else
                        {
                            BaseScript.PathPoints[selectedControlIndex].bezier_out = Vector3.zero;
                            if (BaseScript.PathType == XTween_PathType.平衡贝塞尔)
                                BaseScript.PathPoints[selectedControlIndex].bezier_in = Vector3.zero;
                        }
                        e.Use();
                    }
                }
            }
        }

        /// <summary>
        /// 因为路径动画的特殊性必须将其轴心点设为中心对齐
        /// </summary>
        private void SetTransformCenter()
        {
            if (target == null)
                return;
            RectTransform rect = BaseScript.GetComponent<RectTransform>();
            if (rect == null)
                return;
            Vector3 ori_pos = rect.localPosition;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.localPosition = ori_pos;
        }
    }
}