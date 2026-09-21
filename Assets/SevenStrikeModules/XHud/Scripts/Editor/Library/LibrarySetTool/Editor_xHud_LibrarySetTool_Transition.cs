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
    using System.Collections;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;
    using Object = UnityEngine.Object;

    public class Editor_XHud_LibrarySetTool_Transition : EditorWindow
    {
        private SerializedObject BaseObject;
        private SerializedProperty
            sp_TransitionNode,
            sp_TransitionName,
            sp_TransitionDescription,
            sp_Preview_Texture_Bg,
            sp_Preview_Color,
            sp_BgGrid_Tilling_multiply,
            sp_BgGrid_Color_A,
            sp_BgGrid_Color_B,
            sp_itemHeight,
            sp_visibleItemCount,
            sp_SelectedIndex,
            sp_TransitionInfoList_Scroller,
            sp_PreviewData_Index;

        private ReorderableList TrasitionInfoList;

        private Texture2D icon;

        private Texture2D leftarr_r;
        private Texture2D leftarr_p;
        private Texture2D rightarr_r;
        private Texture2D rightarr_p;
        private Texture2D stop_r;
        private Texture2D stop_p;
        private Texture2D play_r;
        private Texture2D play_p;
        private Texture2D seq_list_logo;

        [SerializeField]
        private int PreviewData_Index;
        private bool IsPreviewing;
        private bool StoppedResetMode = true;
        private XCoroutine PreviewCoroutine;
        private float PreviewDataDuration = 0.035f;
        /// <summary>
        /// 反色通道
        /// </summary>
        private bool InvertChannel = false;

        private Material Preview_Material;
        [SerializeField]
        private Texture2D Preview_Texture_Bg;
        [SerializeField]
        private Color Preview_Color = Color.white;
        [SerializeField]
#pragma warning disable CS0414
        private float BgGrid_Tilling_multiply = 14;
#pragma warning restore CS0414
        [SerializeField]
        private float BgGrid_Tilling_x = 14;
        [SerializeField]
        private float BgGrid_Tilling_y = 14;
        [SerializeField]
        private Color BgGrid_Color_A = Color.white * 0.509f;
        [SerializeField]
        private Color BgGrid_Color_B = Color.white * 0.376f;

        [SerializeField]
        /// <summary>
        /// Editor列表项高度
        /// </summary>
        public float itemHeight = 25;
        [SerializeField]
        /// <summary>
        /// 可视区域显示的元素数量
        /// </summary>
        public int visibleItemCount = 19;
        [SerializeField]
        /// <summary>
        /// 选中项索引号
        /// </summary>
        public int SelectedIndex;
        [SerializeField]
        /// <summary>
        /// 列表滚动位置
        /// </summary>
        public Vector2 TransitionInfoList_Scroller;

        public int ModifiedIndex;

        [SerializeField]
        public string TransitionName;
        [SerializeField]
        public string DateTimes;
        [SerializeField]
        public string TransitionDescription;

        /// <summary>
        /// 按钮高度
        /// </summary>
        private float ButtonHeight = 25;
        /// <summary>
        /// 按钮间距
        /// </summary>
        private float ButtonDistance = 15;

        [SerializeField]
        public string ButtonText_Ok;
        [SerializeField]
        public string ButtonText_Cancel;

        [SerializeField]
        private XHud_LibraryArg_Transition TransitionNode;
        [SerializeField]
        private XHud_LibraryArg_Transition OriginTransitionNode;

        private Rect draw_rect;
        private Rect dragarea;
        private XHud_Library_Transition Target_Hud_TransitionLibrary;
        private string Title;

        private void OnDisable()
        {
            XGUI.x_Editor_Data_Set_With_String("xData_Transition_Bg_Grid_ColorA", XGUI_Utilitys.Color_To_String(sp_BgGrid_Color_A.colorValue));
            XGUI.x_Editor_Data_Set_With_String("xData_Transition_Bg_Grid_ColorB", XGUI_Utilitys.Color_To_String(sp_BgGrid_Color_B.colorValue));
            XGUI.x_Editor_Data_Set_With_String("xData_Transition_Bg_Grid_Tilling", sp_BgGrid_Tilling_multiply.floatValue.ToString("F2"));
            XGUI.x_Editor_Data_Set_With_String("xData_Transition_Bg_Path", AssetDatabase.GetAssetPath(sp_Preview_Texture_Bg.objectReferenceValue));
        }

        private void OnEnable()
        {
            BaseObject = new SerializedObject(this);

            sp_TransitionNode = BaseObject.FindProperty("TransitionNode");
            sp_TransitionName = BaseObject.FindProperty("TransitionName");
            sp_TransitionDescription = BaseObject.FindProperty("TransitionDescription");

            sp_Preview_Texture_Bg = BaseObject.FindProperty("Preview_Texture_Bg");
            sp_BgGrid_Tilling_multiply = BaseObject.FindProperty("BgGrid_Tilling_multiply");
            sp_BgGrid_Color_A = BaseObject.FindProperty("BgGrid_Color_A");
            sp_BgGrid_Color_B = BaseObject.FindProperty("BgGrid_Color_B");
            sp_Preview_Color = BaseObject.FindProperty("Preview_Color");

            sp_itemHeight = BaseObject.FindProperty("itemHeight");
            sp_visibleItemCount = BaseObject.FindProperty("visibleItemCount");
            sp_SelectedIndex = BaseObject.FindProperty("SelectedIndex");
            sp_TransitionInfoList_Scroller = BaseObject.FindProperty("TransitionInfoList_Scroller");
            sp_PreviewData_Index = BaseObject.FindProperty("PreviewData_Index");

            InitializePreviewMaterial();

            LoadPreviewTexture();

            if (XGUI.x_Editor_Data_Has_String("xData_Transition_Bg_Grid_ColorA"))
                sp_BgGrid_Color_A.colorValue = XGUI_Utilitys.String_To_Color(XGUI.x_Editor_Data_Get_With_String("xData_Transition_Bg_Grid_ColorA"), false);
            else
                sp_BgGrid_Color_A.colorValue = Color.white * 0.509f;

            if (XGUI.x_Editor_Data_Has_String("xData_Transition_Bg_Grid_ColorB"))
                sp_BgGrid_Color_B.colorValue = XGUI_Utilitys.String_To_Color(XGUI.x_Editor_Data_Get_With_String("xData_Transition_Bg_Grid_ColorB"), false);
            else
                sp_BgGrid_Color_B.colorValue = Color.white * 0.376f;

            if (XGUI.x_Editor_Data_Has_String("xData_Transition_Bg_Grid_Tilling"))
                sp_BgGrid_Tilling_multiply.floatValue = float.Parse(XGUI.x_Editor_Data_Get_With_String("xData_Transition_Bg_Grid_Tilling"));
            else
                sp_BgGrid_Tilling_multiply.floatValue = 5;

            sp_BgGrid_Color_A.serializedObject.ApplyModifiedProperties();
            sp_BgGrid_Color_B.serializedObject.ApplyModifiedProperties();
            sp_BgGrid_Tilling_multiply.serializedObject.ApplyModifiedProperties();

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

            TransitionDescription = "转场说明内容";
            TransitionName = "转场名称";

            sp_itemHeight.floatValue = 25;
            sp_itemHeight.serializedObject.ApplyModifiedProperties();

            sp_visibleItemCount.intValue = 15;
            sp_visibleItemCount.serializedObject.ApplyModifiedProperties();

            #region ReorderableList - TransitionLibrary
            TrasitionInfoList = new ReorderableList(BaseObject, sp_TransitionNode.FindPropertyRelative("Frames"), true, true, true, true);
            TrasitionInfoList.drawElementCallback = TransitionInfoList_Original_DrawElementCallback;
            #endregion
        }

        private void OnDestroy()
        {
            GetWindow<SceneView>().Focus();
        }

        #region TransitionInfoList_Original      

        Rect drawelement_rect;
        Rect scrollview_rect;
        Rect item_rect;
        Color SelectedBg = new Color(0, 0, 0, 0.2f);

        /// <summary>
        /// 绘制元素
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="index"></param>
        /// <param name="isActive"></param>
        /// <param name="isFocused"></param>
        private void TransitionInfoList_Original_DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty prop = sp_TransitionNode;
            SerializedProperty sp_texlist = prop.FindPropertyRelative("Frames");
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
             text_color: XHud_Dashboard.Theme_Primary,
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
        /// 绘制列表
        /// </summary>
        private void DrawTransitionInfoList_Original(Rect rect)
        {
            // 绘制滚动视图
            scrollview_rect.Set(rect.x + 10, rect.y + 18, rect.width - 20, sp_visibleItemCount.intValue * sp_itemHeight.floatValue);
            sp_TransitionInfoList_Scroller.vector2Value = GUI.BeginScrollView(scrollview_rect, sp_TransitionInfoList_Scroller.vector2Value, new Rect(0, 0, scrollview_rect.width - 50, TrasitionInfoList.count * sp_itemHeight.floatValue), false, true);

            // 计算可视区域的起始和结束索引
            int startIndex = Mathf.FloorToInt(sp_TransitionInfoList_Scroller.vector2Value.y / sp_itemHeight.floatValue);
            int endIndex = Mathf.CeilToInt((sp_TransitionInfoList_Scroller.vector2Value.y + scrollview_rect.height) / sp_itemHeight.floatValue);

            // 只绘制可视区域内的元素
            for (int i = startIndex; i < endIndex && i < TrasitionInfoList.count; i++)
            {
                SerializedProperty sp_texlist = sp_TransitionNode.FindPropertyRelative("Frames");
                SerializedProperty sp_tex = sp_texlist.GetArrayElementAtIndex(i);

                item_rect.Set(0, i * sp_itemHeight.floatValue, scrollview_rect.width, sp_itemHeight.floatValue);

                if (sp_SelectedIndex.intValue == i)
                {
                    // 高亮标记表示选中
                    item_rect.Set(1, i * sp_itemHeight.floatValue + 10, 5, 5);
                    EditorGUI.DrawRect(item_rect, XHud_Dashboard.Theme_Primary);
                    // 高亮背景表示选中
                    item_rect.Set(0, i * sp_itemHeight.floatValue, scrollview_rect.width, sp_itemHeight.floatValue);
                    EditorGUI.DrawRect(item_rect, SelectedBg);
                }

                item_rect.Set(0, i * sp_itemHeight.floatValue, scrollview_rect.width, sp_itemHeight.floatValue);

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
                            StopPreviewUpdate();
                            IsPreviewing = false;

                            if (sp_SelectedIndex.intValue <= 0)
                            {
                                sp_SelectedIndex.intValue = 0;
                            }
                            else

                            {
                                sp_SelectedIndex.intValue--;
                            }
                            PreviewData_Index = sp_SelectedIndex.intValue;
                            CalculateListScroller();
                        }
                        if (e.keyCode == KeyCode.DownArrow)
                        {
                            StopPreviewUpdate();
                            IsPreviewing = false;
                            if (sp_SelectedIndex.intValue >= TransitionNode.Frames.Count - 1)
                            {
                                sp_SelectedIndex.intValue = TransitionNode.Frames.Count - 1;
                            }
                            else

                            {
                                sp_SelectedIndex.intValue++;
                            }
                            PreviewData_Index = sp_SelectedIndex.intValue;
                            CalculateListScroller();
                        }
                        e.Use();
                    }
                    #endregion

                    #region 选中序列帧索引
                    if (e.type == EventType.MouseDown)
                    {
                        // 更新选中项
                        sp_SelectedIndex.intValue = i;

                        PreviewData_Index = sp_SelectedIndex.intValue;

                        // 标记界面需要更新
                        GUI.changed = true;
                        e.Use();
                    }
                    #endregion
                }

                // 绘制元素
                //EditorGUI.PropertyField(item_rect, sp_tex, GUIContent.none);
            }

            GUI.EndScrollView();
        }

        #endregion

        private void OnGUI()
        {
            BaseObject.Update();

#if UNITY_6000_0_OR_NEWERdddd
            TextClipping clipping = TextClipping.Ellipsis;
#else
            TextClipping clipping = TextClipping.Clip;
#endif

            string colorhex = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary, true);

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
                color: XHud_Dashboard.Theme_SeperateLine,
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

            SerializedProperty sp_name = sp_TransitionNode.FindPropertyRelative("Name");
            SerializedProperty sp_des = sp_TransitionNode.FindPropertyRelative("Description");
            SerializedProperty sp_TotalFramesCount = sp_TransitionNode.FindPropertyRelative("TotalFramesCount");
            SerializedProperty sp_LastFrameIndex = sp_TransitionNode.FindPropertyRelative("LastFrameIndex");
            SerializedProperty sp_SkipFrame = sp_TransitionNode.FindPropertyRelative("SkipFrame");
            SerializedProperty sp_Res = sp_TransitionNode.FindPropertyRelative("Res");
            SerializedProperty sp_Frames = sp_TransitionNode.FindPropertyRelative("Frames");

            #region 数值关联
            if (sp_Frames.arraySize > 0)
            {
                sp_Res.vector2IntValue = new Vector2Int(TransitionNode.Frames[0].width, TransitionNode.Frames[0].height);
                sp_TotalFramesCount.intValue = sp_Frames.arraySize;
                sp_LastFrameIndex.intValue = sp_Frames.arraySize - 1;
            }
            #endregion

            #region 序列帧列表
            Rect rect_transitionlist_group = new Rect(rect.x + 20, rect.y + 120, 245, 407);

            XGUI.gui_group_start(
               rect_group: rect_transitionlist_group,
               bg_fill: XGUIFilled.缺口纯色边框,
               bg_color: XGUIColor.亮白,
               bg_color_gui: XHud_Dashboard.Theme_Group,
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
               title_text_color: XHud_Dashboard.Theme_Primary,
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
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.Theme_Group,
                absolute_margin: true,
                absolute_padding: true,
                title: "控制区",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.Theme_Primary,
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
               //bg_color_gui: XHud_Dashboard.Theme_Group,
               absolute_margin: true,
               absolute_padding: true,
               //title: "SSS",
               //title_size: XGUIFontSize.M,
               //title_text_color: XHud_Dashboard.Theme_Primary,
               //title_clipping: TextClipping.Clip,
               //title_manual_offset: true,
               //title_manual_offset_space: 5,
               margin: new RectOffset(0, 0, 0, 0),
               padding: new RectOffset(0, 0, 0, 0));

            #region 参考图
            XGUI.layout_property_field(
              title: "参考图",
              title_size: XGUIFontSize.M,
              title_hover_color: XHud_Dashboard.Theme_Primary,
              title_width: 50,
              field_width: 130,
              prop: sp_Preview_Texture_Bg,
              prop_margin: new RectOffset(0, 0, 5, 10));
            #endregion

            #region 跳帧
            XGUI.layout_property_field(
                title: "跳帧",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.Theme_Primary,
                title_width: 30,
                field_width: 35,
                prop: sp_SkipFrame,
                prop_margin: new RectOffset(0, 0, 5, 10));
            #endregion

            #region 平铺
            XGUI.layout_property_field(
                title: "平铺",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.Theme_Primary,
                title_width: 30,
                field_width: 35,
                prop: sp_BgGrid_Tilling_multiply,
                prop_margin: new RectOffset(0, 0, 5, 10));
            #endregion

            #region 颜色 - 背景
            sp_Preview_Color.colorValue = XGUI.layout_colorfield(
                prop: sp_Preview_Color.colorValue,
                state_title: null,
                field_width: 50);
            sp_Preview_Color.serializedObject.ApplyModifiedProperties();
            #endregion

            #region 颜色 - 格子 - 前景
            sp_BgGrid_Color_A.colorValue = XGUI.layout_colorfield(
                prop: sp_BgGrid_Color_A.colorValue,
                state_title: null,
                field_width: 50);
            sp_BgGrid_Color_A.serializedObject.ApplyModifiedProperties();
            #endregion

            #region 颜色 - 格子 - 背景
            sp_BgGrid_Color_B.colorValue = XGUI.layout_colorfield(
                prop: sp_BgGrid_Color_B.colorValue,
                state_title: null,
                field_width: 50);
            sp_BgGrid_Color_B.serializedObject.ApplyModifiedProperties();
            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            #endregion

            XGUI.layout_space(316);

            #region 控件区 - 预览操作
            XGUI.layout_group_start(
               type: XGUIContainerType.Horizontal,
               //bg_fill: XGUIFilled.缺口纯色边框,
               //bg_color: XGUIColor.亮白,
               //bg_color_gui: XHud_Dashboard.Theme_Group,
               absolute_margin: true,
               absolute_padding: true,
               //title: "SSS",
               //title_size: XGUIFontSize.M,
               //title_text_color: XHud_Dashboard.Theme_Primary,
               //title_clipping: TextClipping.Clip,
               //title_manual_offset: true,
               //title_manual_offset_space: 5,
               margin: new RectOffset(0, 0, 0, 0),
               padding: new RectOffset(0, 0, 0, 0));

            XGUI.ChangedCheck_Start();
            sp_PreviewData_Index.intValue = XGUI.layout_slider_int(
                title: "序列进度",
                prop: sp_PreviewData_Index,
                left: 0,
                right: TransitionNode.Frames.Count - 1,
                title_width: 80,
                title_size: XGUIFontSize.M,
                title_anchor: TextAnchor.MiddleLeft,
                prop_margin: new RectOffset(0, 0, 0, 0));
            if (XGUI.ChangedCheck_End())
            {
                StopPreviewUpdate();
                IsPreviewing = false;

                sp_SelectedIndex.intValue = sp_PreviewData_Index.intValue;

                CalculateListScroller();
            }
            sp_PreviewData_Index.serializedObject.ApplyModifiedProperties();

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
                IsPreviewing = true;
                StartPreviewUpdate();
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
                IsPreviewing = false;
                StopPreviewUpdate();
                PreviewData_Index = 0;
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
               bg_fill: XGUIFilled.无,
               bg_color: XGUIColor.亮白,
               bg_color_gui: Color.white,
               button_text_color: Color.white,
               press_fill: XGUIFilled.透明,
               press_color: XGUIColor.无,
               press_text_color: XHud_Dashboard.Theme_Primary,
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
               bg_fill: XGUIFilled.无,
               bg_color: XGUIColor.亮白,
               bg_color_gui: Color.white,
               button_text_color: Color.white,
               press_fill: XGUIFilled.透明,
               press_color: XGUIColor.无,
               press_text_color: XHud_Dashboard.Theme_Primary,
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
                StopPreviewUpdate();
                IsPreviewing = false;

                PreviewData_Index = 0;
            }
            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 信息区
            XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.Theme_Group,
                absolute_margin: true,
                absolute_padding: true,
                title: "转场序列帧信息",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.Theme_Primary,
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
            if (string.IsNullOrEmpty(sp_TransitionName.stringValue))
            {
                sp_TransitionName.stringValue = "转场名称";
            }

            sp_TransitionName.stringValue = XGUI.layout_inputfield(
                title: "名称",
                prop: sp_TransitionName.stringValue,
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
            sp_TransitionName.serializedObject.ApplyModifiedProperties();
            #endregion

            XGUI.layout_space(15);

            #region 转场说明
            if (string.IsNullOrEmpty(sp_TransitionDescription.stringValue))
            {
                sp_TransitionDescription.stringValue = "转场说明";
            }
            sp_TransitionDescription.stringValue = XGUI.layout_inputfield(
              title: "说明",
              prop: sp_TransitionDescription.stringValue,
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
            sp_TransitionDescription.serializedObject.ApplyModifiedProperties();
            #endregion

            XGUI.layout_space(15);

            #region 转场帧信息
            XGUI.layout_label(
                text: $"<color={colorhex}>{PreviewData_Index.ToString()}</color> / 帧",
                bg_fill: XGUIFilled.无,
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

            if (TransitionNode.Frames != null && TransitionNode.Frames.Count > 0)
            {
                float w = TransitionNode.Frames[0].width;
                float h = TransitionNode.Frames[0].height;

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

                Preview_Material.SetColor("_Color", sp_Preview_Color.colorValue);
                Preview_Material.SetFloat("_Alpha", sp_Preview_Color.colorValue.a);

                BgGrid_Tilling_x = sp_BgGrid_Tilling_multiply.floatValue;
                BgGrid_Tilling_y = sp_BgGrid_Tilling_multiply.floatValue;

                Preview_Material.SetFloat("_Grid_Tilling_x", BgGrid_Tilling_x * textureWidth);
                Preview_Material.SetFloat("_Grid_Tilling_y", BgGrid_Tilling_y * textureHeight);
                Preview_Material.SetColor("_Grid_Color_A", sp_BgGrid_Color_A.colorValue);
                Preview_Material.SetColor("_Grid_Color_B", sp_BgGrid_Color_B.colorValue);
                Preview_Material.SetTexture("_Texture", Preview_Texture_Bg);
                Preview_Material.SetTexture("_Mask", TransitionNode.Frames[PreviewData_Index]);
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
                    bg_fill: XGUIFilled.实体,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.Theme_Primary,
                    text: new GUIContent($"{PreviewData_Index} / {sp_TotalFramesCount.intValue - 1}"),
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
                    TransitionNode.Frames.Clear();
                    foreach (var item in dropobjs)
                    {
                        if (item is Texture2D == false)
                            continue;

                        Texture2D tex = (Texture2D)item;

                        if (TransitionNode.Frames == null)
                            TransitionNode.Frames = new List<Texture2D>();

                        TransitionNode.Frames.Add(tex);
                    }

                    e.Use();
                    PreviewData_Index = 0;
                    SelectedIndex = 0;
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
                if (rect_preview_group.Contains(e.mousePosition))
                {
                    if (e.keyCode == KeyCode.RightArrow)
                    {
                        FrameMoveForward();
                        CalculateListScroller();
                    }
                    if (e.keyCode == KeyCode.LeftArrow)
                    {
                        FrameMoveBackward();
                        CalculateListScroller();
                    }
                }
                if (e.keyCode == KeyCode.Escape)
                {
                    Close();
                }
                e.Use();
            }
            #endregion

            Repaint();

            if (BaseObject.targetObject != null)
                BaseObject.ApplyModifiedProperties();
        }

        private void FrameMoveForward()
        {
            StopPreviewUpdate();
            IsPreviewing = false;

            if (sp_PreviewData_Index.intValue >= TransitionNode.Frames.Count - 1)
            {
                sp_PreviewData_Index.intValue = TransitionNode.Frames.Count - 1;
            }
            else

            {
                sp_PreviewData_Index.intValue++;
            }

            sp_SelectedIndex.intValue = sp_PreviewData_Index.intValue;

            Repaint();
        }

        private void FrameMoveBackward()
        {
            StopPreviewUpdate();
            IsPreviewing = false;

            if (sp_PreviewData_Index.intValue <= 0)
            {
                sp_PreviewData_Index.intValue = 0;
            }
            else

            {
                sp_PreviewData_Index.intValue--;
            }

            sp_SelectedIndex.intValue = sp_PreviewData_Index.intValue;

            Repaint();
        }

        #region 辅助
        private void UpdateToLibrary()
        {
            string colorhex = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary, true);

            XHud_LibraryArg_Transition info = new XHud_LibraryArg_Transition();
            info.CopyData(TransitionNode);
            info.Name = sp_TransitionName.stringValue;
            info.Description = sp_TransitionDescription.stringValue;
            Target_Hud_TransitionLibrary.TransitionsLibrary_Replace(ModifiedIndex, info);

            XGUI.dialog(
                type: XGUIDialogType.确认,
                windowtitle: "XHud - 转场资源修改器消息",
                title: "更新完成",
                msg: $"转场资源已更新完成！",
                ok: "明白",
                PrimaryIndex: 0,
                usemodal: true,
                themecolor: XHud_Dashboard.Theme_Primary);

            Close();
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
        /// <param name="name"></param>
        /// <param name="decription"></param>
        /// <param name="color"></param>
        public void SetInfo(string name, string decription)
        {
            sp_TransitionName.stringValue = name;
            sp_TransitionDescription.stringValue = decription;

            sp_TransitionName.serializedObject.ApplyModifiedProperties();
            sp_TransitionDescription.serializedObject.ApplyModifiedProperties();

            PreviewData_Index = 0;
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
                UpdateToLibrary();
                return;
            }

            XGUI.layout_flexspace();

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            #endregion
        }

        public void Set_TransitionNode(XHud_LibraryArg_Transition node)
        {
            TransitionNode.CopyData(node);
        }

        public void Set_Target_Hud_TransitionLibrary(XHud_Library_Transition lib)
        {
            Target_Hud_TransitionLibrary = lib;
        }

        public void Set_OriginTransitionNode(XHud_LibraryArg_Transition node)
        {
            OriginTransitionNode.CopyData(node);
        }

        /// <summary>
        /// 开始预览
        /// </summary>
        public void StartPreviewUpdate()
        {
            PreviewCoroutine = XCoroutineUtility.xec_StartCoroutineOwnerless(PreviewUpdater());
        }

        /// <summary>
        /// 停止预览
        /// </summary>
        public void StopPreviewUpdate()
        {
            if (PreviewCoroutine == null)
                return;
            XCoroutineUtility.xec_StopCoroutine(PreviewCoroutine);
        }

        IEnumerator PreviewUpdater()
        {
            var waitForOneSecond = new XCoroutineWaitForSeconds(PreviewDataDuration);
            while (true)
            {
                if (PreviewData_Index >= TransitionNode.Frames.Count - 1 - TransitionNode.SkipFrame)
                {
                    if (!StoppedResetMode)
                    {
                        IsPreviewing = false;
                        PreviewData_Index = 0;
                    }
                    else
                    {
                        PreviewData_Index = TransitionNode.Frames.Count - 1;
                    }
                    StopPreviewUpdate();
                    break;
                }
                else
                {
                    PreviewData_Index += TransitionNode.SkipFrame;
                }
                SelectedIndex = PreviewData_Index;

                yield return waitForOneSecond;
            }
        }

        /// <summary>
        /// 初始化预览转场材质
        /// </summary>
        private void InitializePreviewMaterial()
        {
            Shader shader = Shader.Find("XHud/EditorTransition");
            Preview_Material = new Material(shader);
        }

        /// <summary>
        /// 初始化预览背景图
        /// </summary>
        private void LoadPreviewTexture()
        {
            string path = XGUI.x_Editor_Data_Get_With_String("xData_Transition_Bg_Path");
            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (tex == null)
                sp_Preview_Texture_Bg.objectReferenceValue = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_transition_setter/defaultbg");
            else
                sp_Preview_Texture_Bg.objectReferenceValue = tex;
        }

        private void CalculateListScroller()
        {
            if (SelectedIndex >= visibleItemCount)
            {
                sp_TransitionInfoList_Scroller.vector2Value = new Vector2(sp_TransitionInfoList_Scroller.vector2Value.x, (SelectedIndex) * itemHeight);
            }
            else
                sp_TransitionInfoList_Scroller.vector2Value = Vector2.zero;
        }
        #endregion
    }
}