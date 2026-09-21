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
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    [CustomEditor(typeof(XHud_Library_Transition))]
    public class Editor_XHud_Library_Transition : Editor
    {
        #region 组件 / 列表
        private XHud_Library_Transition BaseScript;
        private ReorderableList TrasitionInfoList;
        #endregion

        #region 序列化属性
        private SerializedProperty sp_TransitionLibrary, sp_LibraryName, sp_SelectedIndex, sp_LocationSelectedIndex, sp_TransitionInfoList_Original_Scroller, sp_itemHeight, sp_visibleItemCount, sp_Find, sp_Highlight;
        #endregion

        #region 图标
        private Texture2D warnIcon, btn_icon_details_released, btn_icon_details_press, analyze_p, analyze_r, clear_p, clear_r, create_p, create_r, delete_p, delete_r, skip, total, last;
        #endregion

        /// <summary>
        /// 屏蔽颜色
        /// </summary>
        private Color blocked_col;
        /// <summary>
        /// 屏蔽颜色透明度
        /// </summary>
        private float blocked_alp = 0.45f;

        private void OnEnable()
        {
            BaseScript = (XHud_Library_Transition)target;

            #region 获取序列帧属性
            sp_TransitionLibrary = serializedObject.FindProperty("TransitionLibrary");
            sp_LibraryName = serializedObject.FindProperty("LibraryName");
            sp_Highlight = serializedObject.FindProperty("Highlight");
            sp_SelectedIndex = serializedObject.FindProperty("SelectedIndex");
            sp_LocationSelectedIndex = serializedObject.FindProperty("LocationSelectedIndex");
            sp_TransitionInfoList_Original_Scroller = serializedObject.FindProperty("TransitionInfoList_Original_Scroller");
            sp_itemHeight = serializedObject.FindProperty("itemHeight");
            sp_visibleItemCount = serializedObject.FindProperty("visibleItemCount");
            sp_Find = serializedObject.FindProperty("Find");
            #endregion

            #region 获取图标
            warnIcon = XGUI.GetBuiltInIcon("console.warnicon");
            btn_icon_details_released = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_transition/detail_r");
            btn_icon_details_press = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_transition/detail_p");
            analyze_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_transition/analyze_p");
            analyze_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_transition/analyze_r");
            clear_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_transition/clear_p");
            clear_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_transition/clear_r");
            create_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_transition/create_p");
            create_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_transition/create_r");
            delete_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_transition/delete_p");
            delete_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_transition/delete_r");
            skip = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_transition/skip");
            total = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_transition/total");
            last = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_transition/last");
            #endregion

            //设定列表项高度值
            sp_itemHeight.floatValue = 75;
            sp_itemHeight.serializedObject.ApplyModifiedProperties();

            sp_visibleItemCount.intValue = 6;
            sp_visibleItemCount.serializedObject.ApplyModifiedProperties();

            blocked_col = new Color(0, 0, 0, blocked_alp);

            #region ReorderableList - TransitionLibrary
            TrasitionInfoList = new ReorderableList(serializedObject, sp_TransitionLibrary, true, true, true, true);
            TrasitionInfoList.drawElementCallback = TransitionInfoList_Original_DrawElementCallback;
            #endregion
        }

        private void OnDisable()
        {
            if (target != null)
            {
                sp_TransitionInfoList_Original_Scroller.vector2Value = Vector2.zero;
                sp_TransitionInfoList_Original_Scroller.serializedObject.ApplyModifiedProperties();

                sp_SelectedIndex.intValue = -1;
                sp_SelectedIndex.serializedObject.ApplyModifiedProperties();

                sp_LocationSelectedIndex.intValue = -1;
                sp_LocationSelectedIndex.serializedObject.ApplyModifiedProperties();

                sp_Find.stringValue = string.Empty;
                sp_Find.serializedObject.ApplyModifiedProperties();

                sp_Highlight.stringValue = string.Empty;
                sp_Highlight.serializedObject.ApplyModifiedProperties();
            }
        }

        #region TransitionInfoList_Original      

        Rect drawelement_rect;
        Rect scrollview_rect;
        Rect item_rect;
        Color SelectedBg = new Color(0, 0, 0, 0.2f);

        private void BlockGUI(string name)
        {
            if (!name.Contains(sp_Highlight.stringValue))
            {
                GUI.enabled = false;
            }
            else
            {
                GUI.enabled = true;
            }
        }

        /// <summary>
        /// 绘制元素
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="index"></param>
        /// <param name="isActive"></param>
        /// <param name="isFocused"></param>
        private void TransitionInfoList_Original_DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty prop = sp_TransitionLibrary.GetArrayElementAtIndex(index);
            SerializedProperty sp_name = prop.FindPropertyRelative("Name");
            SerializedProperty sp_des = prop.FindPropertyRelative("Description");
            SerializedProperty sp_TotalFramesCount = prop.FindPropertyRelative("TotalFramesCount");
            SerializedProperty sp_LastFrameIndex = prop.FindPropertyRelative("LastFrameIndex");
            SerializedProperty sp_SkipFrame = prop.FindPropertyRelative("SkipFrame");
            SerializedProperty sp_Res = prop.FindPropertyRelative("Res");
            SerializedProperty sp_Frames = prop.FindPropertyRelative("Frames");

#if UNITY_6000_0_OR_NEWER
            TextClipping clipping = TextClipping.Ellipsis;
#else
    TextClipping clipping = TextClipping.Clip;
#endif

            #region 序号
            drawelement_rect.Set(rect.x + 15, rect.y + 5, 30, 20);
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

            #region 标识名称
            drawelement_rect.Set(rect.x + 40, rect.y + 5, rect.width - 140, 20);
            XGUI.gui_label(
                rect: drawelement_rect,
                text: new GUIContent(sp_name.stringValue),
                text_color: XHud_Dashboard.Theme_Primary,
                size: XGUIFontSize.M,
                clipping: clipping,
                anchor: TextAnchor.MiddleLeft,
                offset: new Vector2(0, 0),
                font_style: FontStyle.Normal);
            #endregion

            #region 动效说明
            drawelement_rect.Set(rect.x + 40, rect.y + 25, rect.width - 160, 20);
            XGUI.gui_label(
                rect: drawelement_rect,
                text: new GUIContent(sp_des.stringValue),
                text_color: Color.white * 0.65f,
                size: XGUIFontSize.M,
                clipping: clipping,
                anchor: TextAnchor.MiddleLeft,
                offset: new Vector2(0, 0),
                font_style: FontStyle.Normal);
            #endregion


            #region 帧信息

            float AddedHeight = 16;

            drawelement_rect.Set(rect.x + 40, rect.y + 36 + AddedHeight, 14, 14);
            XGUI.gui_icon(
                  rect: drawelement_rect,
                  icon: total,
                  color: Color.white);

            drawelement_rect.Set(rect.x + 110, rect.y + 36 + AddedHeight, 14, 14);
            XGUI.gui_icon(
                  rect: drawelement_rect,
                  icon: last,
                  color: Color.white);

            drawelement_rect.Set(rect.x + 180, rect.y + 36 + AddedHeight, 14, 14);
            XGUI.gui_icon(
                  rect: drawelement_rect,
                  icon: skip,
                  color: Color.white);

            drawelement_rect.Set(rect.x + 70, rect.y + 33 + AddedHeight, 80, 20);
            XGUI.gui_label(
                rect: drawelement_rect,
                text: new GUIContent(sp_TotalFramesCount.intValue.ToString()),
                text_color: Color.white,
                size: XGUIFontSize.M,
                clipping: clipping,
                anchor: TextAnchor.MiddleLeft,
                offset: new Vector2(0, 0),
                font_style: FontStyle.Normal);

            drawelement_rect.Set(rect.x + 140, rect.y + 33 + AddedHeight, 80, 20);
            XGUI.gui_label(
                rect: drawelement_rect,
                text: new GUIContent(sp_LastFrameIndex.intValue.ToString()),
                text_color: Color.white,
                size: XGUIFontSize.M,
                clipping: clipping,
                anchor: TextAnchor.MiddleLeft,
                offset: new Vector2(0, 0),
                font_style: FontStyle.Normal);

            drawelement_rect.Set(rect.x + 210, rect.y + 33 + AddedHeight, 80, 20);
            XGUI.gui_label(
                rect: drawelement_rect,
                text: new GUIContent(sp_SkipFrame.intValue.ToString()),
                text_color: Color.white,
                size: XGUIFontSize.M,
                clipping: clipping,
                anchor: TextAnchor.MiddleLeft,
                offset: new Vector2(0, 0),
                font_style: FontStyle.Normal);
            #endregion

            #region 转场项的序列帧状态
            if (sp_Frames.arraySize <= 0)
            {
                GUIContent content = new GUIContent();
                content.image = warnIcon;
                content.tooltip = "未找到任何序列帧图像";
                drawelement_rect.Set(rect.x + 15, rect.y + 35, 15, 15);

                XGUI.gui_label(
                    rect: drawelement_rect,
                    text: content,
                    text_color: Color.white,
                    size: XGUIFontSize.M,
                    clipping: clipping,
                    anchor: TextAnchor.MiddleLeft,
                    offset: new Vector2(0, 0),
                    font_style: FontStyle.Normal);
            }
            #endregion

            BlockGUI(sp_name.stringValue);

            #region 辅助菜单
            // 检测鼠标点击事件
            Event e = Event.current;
            drawelement_rect.Set(rect.x, rect.y, rect.width, rect.height);
            // 检测鼠标点击事件                        
            if (e.type == EventType.MouseDown && e.button == 1 && drawelement_rect.Contains(e.mousePosition))
            {
                // 创建右键菜单
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("E (修改转场资源)"), false, () =>
                {
                    OpenParameterSetter(BaseScript.TransitionLibrary[index], index);
                });
                menu.AddItem(new GUIContent("C (克隆转场资源)"), false, () =>
                {
                    sp_TransitionLibrary.InsertArrayElementAtIndex(index);
                    sp_TransitionLibrary.serializedObject.ApplyModifiedProperties();
                });
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("S (获取转场资源)"), false, () =>
                {
                    //GUIUtility.systemCopyBuffer = sp_name.stringValue;
                });
                // 显示右键菜单
                menu.ShowAsContext();

                e.Use();
            }
            #endregion

            #region 查看详细信息
            drawelement_rect.Set(rect.width - 50, rect.y + 14, 22, 22);
            if (XGUI.gui_button(
                rect: drawelement_rect,
                tooltip: $"查看详细信息",
                tex_release: btn_icon_details_released,
                tex_press: btn_icon_details_press,
                tex_gui_color: Color.white,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                focus_name: "fav_btn"))
            {
                OpenParameterSetter(BaseScript.TransitionLibrary[index], index);
            }
            #endregion

            GUI.enabled = true;

            if (!sp_name.stringValue.Contains(sp_Highlight.stringValue))
            {
                drawelement_rect.Set(rect.x - 20, rect.y, rect.width + 20, rect.height);
                EditorGUI.DrawRect(drawelement_rect, blocked_col);
            }

            if (index == sp_LocationSelectedIndex.intValue)
            {
                drawelement_rect.Set(rect.width - 125, rect.y + 15, 50, 20);
                XGUI.gui_label(
                    rect: drawelement_rect,
                    text: new GUIContent("已定位"),
                    text_color: XHud_Dashboard.Theme_Primary,
                    size: XGUIFontSize.S,
                    clipping: TextClipping.Clip,
                    anchor: TextAnchor.MiddleCenter,
                    offset: new Vector2(0, 0),
                    font_style: FontStyle.Normal);
            }
        }

        /// <summary>
        /// 绘制列表
        /// </summary>
        private void DrawTransitionInfoList_Original()
        {
            // 绘制滚动视图
            scrollview_rect = GUILayoutUtility.GetRect(0, sp_visibleItemCount.intValue * sp_itemHeight.floatValue);
            sp_TransitionInfoList_Original_Scroller.vector2Value = GUI.BeginScrollView(scrollview_rect, sp_TransitionInfoList_Original_Scroller.vector2Value, new Rect(0, 0, scrollview_rect.width - 50, TrasitionInfoList.count * sp_itemHeight.floatValue), false, true);

            // 计算可视区域的起始和结束索引
            int startIndex = Mathf.FloorToInt(sp_TransitionInfoList_Original_Scroller.vector2Value.y / sp_itemHeight.floatValue);
            int endIndex = Mathf.CeilToInt((sp_TransitionInfoList_Original_Scroller.vector2Value.y + scrollview_rect.height) / sp_itemHeight.floatValue);

            // 只绘制可视区域内的元素
            for (int i = startIndex; i < endIndex && i < TrasitionInfoList.count; i++)
            {
                SerializedProperty prop = sp_TransitionLibrary.GetArrayElementAtIndex(i);
                SerializedProperty sp_name = prop.FindPropertyRelative("Name");

                item_rect.Set(0, i * sp_itemHeight.floatValue, scrollview_rect.width, sp_itemHeight.floatValue);

                // 如果当前元素被选中，绘制选中效果
                if (sp_name.stringValue.Contains(sp_Highlight.stringValue))
                {
                    if (sp_SelectedIndex.intValue == i)
                    {
                        // 高亮标记表示选中
                        item_rect.Set(1, i * sp_itemHeight.floatValue + 12, 5, 5);
                        EditorGUI.DrawRect(item_rect, XHud_Dashboard.Theme_Primary);
                        // 高亮背景表示选中
                        item_rect.Set(0, i * sp_itemHeight.floatValue, scrollview_rect.width, sp_itemHeight.floatValue);
                        EditorGUI.DrawRect(item_rect, SelectedBg);
                    }
                }

                item_rect.Set(0, i * sp_itemHeight.floatValue, scrollview_rect.width, sp_itemHeight.floatValue);

                TrasitionInfoList.drawElementCallback.Invoke(item_rect, i, i == TrasitionInfoList.index, true);

                // 检测鼠标是否在当前元素区域内
                if (item_rect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.MouseDown)
                    {
                        if (sp_name.stringValue.Contains(sp_Highlight.stringValue))
                        {
                            // 更新选中项
                            sp_SelectedIndex.intValue = i;

                            // 标记界面需要更新
                            GUI.changed = true;
                        }
                    }
                }

                // 绘制元素
                EditorGUI.PropertyField(item_rect, prop, GUIContent.none);
            }

            GUI.EndScrollView();
        }

        /// <summary>
        /// 列表移除
        /// </summary>
        /// <param name="list"></param>
        private void TransitionInfoList_Original_Remove(ReorderableList list)
        {
            if (list.count <= 0)
                return;
            sp_TransitionLibrary.DeleteArrayElementAtIndex(sp_SelectedIndex.intValue);
            sp_TransitionLibrary.serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 列表增加
        /// </summary>
        /// <param name="list"></param>
        private void TransitionInfoList_Original_Add(ReorderableList list)
        {
            SerializedProperty prop = null;
            if (sp_TransitionLibrary.arraySize <= 0)
            {
                sp_TransitionLibrary.InsertArrayElementAtIndex(0);
                prop = sp_TransitionLibrary.GetArrayElementAtIndex(0);
            }
            else
            {
                sp_TransitionLibrary.InsertArrayElementAtIndex(list.index);
                prop = sp_TransitionLibrary.GetArrayElementAtIndex(list.index);
            }

            prop.FindPropertyRelative("Name").stringValue = "NewTransition";
            prop.FindPropertyRelative("Res").vector2IntValue = Vector2Int.zero;
            prop.FindPropertyRelative("Frames").ClearArray();
            prop.FindPropertyRelative("Description").stringValue = "";
            prop.FindPropertyRelative("TotalFramesCount").intValue = 0;
            prop.FindPropertyRelative("LastFrameIndex").intValue = 0;
            prop.FindPropertyRelative("SkipFrame").intValue = 1;

            sp_TransitionLibrary.serializedObject.ApplyModifiedProperties();
        }
        #endregion

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
                title_text: "XHud  -  转场库",
                title_anchor: TextAnchor.MiddleLeft,
                title_style: FontStyle.Normal,
                title_color: Color.white,
                title_size: XGUIFontSize.B,
                title_clipping: clipping,
                title_offset: new Vector2(-10, 0),
                bg_margin: new RectOffset(0, 0, 5, 5));
            #endregion

            XGUI.layout_space(5);

            #region 参数区
            XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIFilled.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XHud_Dashboard.Theme_Group,
                    title: "参数",
                    title_size: XGUIFontSize.M,
                    title_text_color: XHud_Dashboard.Theme_Primary,
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15));

            #region 库名称
            XGUI.layout_property_field(
                title: "库名称",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.Theme_Primary,
                title_width: 90,
                prop: sp_LibraryName,
                prop_margin: new RectOffset(0, 0, 5, 0));
            #endregion

            #region 过滤（包含）
            XGUI.ChangedCheck_Start();
            XGUI.layout_property_field(
                title: "过滤（包含）",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.Theme_Primary,
                title_width: 90,
                prop: sp_Highlight,
                prop_margin: new RectOffset(0, 0, 5, 0));
            if (XGUI.ChangedCheck_End())
            {
                sp_LocationSelectedIndex.intValue = -1;
                sp_LocationSelectedIndex.serializedObject.ApplyModifiedProperties();
            }
            #endregion

            #region 查找（精确）
            XGUI.ChangedCheck_Start();
            XGUI.layout_property_field(
                title: "查找（精确）",
                title_size: XGUIFontSize.M,
                title_hover_color: XHud_Dashboard.Theme_Primary,
                title_width: 90,
                prop: sp_Find,
                prop_margin: new RectOffset(0, 0, 5, 0));
            if (XGUI.ChangedCheck_End())
            {
                if (!string.IsNullOrEmpty(sp_Find.stringValue))
                    BaseScript.TransitionLibrary_Location_Find(sp_Find.stringValue);
                else
                {
                    sp_SelectedIndex.intValue = -1;
                    sp_SelectedIndex.serializedObject.ApplyModifiedProperties();

                    sp_LocationSelectedIndex.intValue = -1;
                    sp_LocationSelectedIndex.serializedObject.ApplyModifiedProperties();
                }
            }
            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 快捷功能
            if (string.IsNullOrEmpty(sp_Highlight.stringValue))
            {
                if (!Application.isPlaying)
                {
                    XGUI.layout_group_start(
                        type: XGUIContainerType.Horizontal,
                        bg_fill: XGUIFilled.缺口纯色边框,
                        bg_color: XGUIColor.亮白,
                        bg_color_gui: XHud_Dashboard.Theme_Group,
                        title: "快捷功能",
                        title_size: XGUIFontSize.M,
                        title_text_color: XHud_Dashboard.Theme_Primary,
                        title_clipping: TextClipping.Clip,
                        padding: new RectOffset(15, 15, 20, 15));

                    #region 分析帧数据
                    if (XGUI.layout_button(
                        tooltip: "分析帧数据",
                        tex_release: analyze_r,
                        tex_press: analyze_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: 14,
                        height: 14))
                    {
                        for (int i = 0; i < BaseScript.TransitionLibrary.Count; i++)
                        {
                            XHud_LibraryArg_Transition node = BaseScript.TransitionLibrary[i];
                            if (node.Frames == null || node.Frames.Count <= 0)
                            {
                                node.TotalFramesCount = 0;
                                node.LastFrameIndex = 0;
                                node.Res = Vector2Int.zero;
                                continue;
                            }
                            else
                            {
                                node.TotalFramesCount = node.Frames.Count;
                                node.LastFrameIndex = node.Frames.Count - 1;
                                for (int s = 0; s < node.Frames.Count; s++)
                                {
                                    // 获取纹理的导入设置
                                    TextureImporter importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(node.Frames[s])) as TextureImporter;

                                    if (importer != null)
                                    {
                                        if (importer.npotScale != TextureImporterNPOTScale.None)
                                        {
                                            // 修改 m_NPOTScale 设置为 无_None
                                            importer.npotScale = TextureImporterNPOTScale.None;

                                            // 保存修改后的设置
                                            importer.SaveAndReimport();
                                        }
                                    }
                                }
                                node.Res = new Vector2Int(node.Frames[0].width, node.Frames[0].height);
                            }
                        }
                        return;
                    }
                    #endregion

                    GUILayout.FlexibleSpace();

                    #region 导出元素动效参数模版
                    if (XGUI.layout_button(
                        tooltip: "导出元素动效参数模版",
                        tex_release: clear_r,
                        tex_press: clear_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: 14,
                        height: 14))
                    {
                        string res = XGUI.dialog(
                         type: XGUIDialogType.警告,
                         windowtitle: "XHud - 转场库消息",
                         title: "清空所有转场",
                         msg: "是否清空所有转场项？请注意！如果您的场景中转场组件用到了该转场库中的转场效果项，清空后会导致转场信息丢失，请谨慎操作！",
                         ok: "清空",
                         cancel: "暂不",
                         PrimaryIndex: 0,
                         usemodal: true,
                         themecolor: XHud_Dashboard.Theme_Primary);

                        if (res == "暂不")
                            return;

                        for (int i = 0; i < sp_TransitionLibrary.arraySize; i++)
                        {
                            SerializedProperty frames = sp_TransitionLibrary.GetArrayElementAtIndex(i).FindPropertyRelative("Frames");
                            frames.ClearArray();
                            frames.serializedObject.ApplyModifiedProperties();
                        }

                        sp_TransitionLibrary.ClearArray();
                        sp_TransitionLibrary.serializedObject.ApplyModifiedProperties();
                        return;
                    }
                    #endregion

                    GUILayout.FlexibleSpace();

                    #region 添加项
                    if (XGUI.layout_button(
                        tooltip: "添加项",
                        tex_release: create_r,
                        tex_press: create_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: 14,
                        height: 14))
                    {
                        TransitionInfoList_Original_Add(TrasitionInfoList);
                        return;
                    }
                    #endregion

                    GUILayout.FlexibleSpace();

                    #region 删除项
                    if (XGUI.layout_button(
                        tooltip: "删除项",
                        tex_release: delete_r,
                        tex_press: delete_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: 14,
                        height: 14))
                    {
                        TransitionInfoList_Original_Remove(TrasitionInfoList);
                        return;
                    }
                    #endregion

                    XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
                }
            }
            #endregion

            #region 转场库列表
            XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.Theme_Group,
                title: "转场库列表",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.Theme_Primary,
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15));

            DrawTransitionInfoList_Original();

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            serializedObject.ApplyModifiedProperties();
        }

        #region 辅助
        /// <summary>
        /// 转场库修改器
        /// </summary>
        public void OpenParameterSetter(XHud_LibraryArg_Transition info, int index)
        {
            Editor_XHud_LibrarySetTool_Transition window = EditorWindow.GetWindow<Editor_XHud_LibrarySetTool_Transition>(true);

            window.titleContent = new GUIContent("XHud - 转场资源修改器");
            XGUI.CenterEditorWindow(new Vector2Int(800, 680), window);
            window.SetTitle("转场资源修改器");
            window.SetInfo(info.Name, info.Description);
            window.Set_TransitionNode(info);
            window.Set_OriginTransitionNode(info);
            window.ModifiedIndex = index;
            window.SetButtonText("更新", "取消");
            window.Set_Target_Hud_TransitionLibrary(BaseScript);
            //window.ShowModal();
            window.Show();
        }
        #endregion
    }
}