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
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    [CustomEditor(typeof(XHud_Library_Element))]
    public class Editor_XHud_Library_Element : Editor
    {
        #region 组件 / 列表
        private XHud_Library_Element BaseScript;
        #endregion

        #region 序列化属性
        private SerializedProperty sp_ElementLibrary, sp_LibraryName, sp_SelectedIndex, sp_LocationSelectedIndex, sp_ElementInfoList_Original_Scroller, sp_itemHeight, sp_visibleItemCount, sp_Find, sp_Highlight, sp_LibraryDescription;
        #endregion

        #region 图标
        private Texture2D warnIcon, normalIcon, Icon_Using, btn_icon_edit_released, btn_icon_edit_press, clean_p, clean_r, clear_p, clear_r, create_p, create_r, delete_p, delete_r, tip_r, tip_p, fastcount_r, fastcount_p;
        #endregion

        /// <summary>
        /// 元素列表
        /// </summary>
        public ReorderableList ElementInfoList;

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
            BaseScript = (XHud_Library_Element)target;

            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

            #region 获取序列化属性
            sp_ElementLibrary = serializedObject.FindProperty("ElementLibrary");
            sp_LibraryName = serializedObject.FindProperty("LibraryName");
            sp_LibraryDescription = serializedObject.FindProperty("LibraryDescription");
            sp_Highlight = serializedObject.FindProperty("Highlight");
            sp_SelectedIndex = serializedObject.FindProperty("SelectedIndex");
            sp_LocationSelectedIndex = serializedObject.FindProperty("LocationSelectedIndex");
            sp_ElementInfoList_Original_Scroller = serializedObject.FindProperty("ElementInfoList_Original_Scroller");
            sp_itemHeight = serializedObject.FindProperty("itemHeight");
            sp_visibleItemCount = serializedObject.FindProperty("visibleItemCount");
            sp_Find = serializedObject.FindProperty("Find");
            #endregion

            blocked_col = new Color(0, 0, 0, blocked_alp);

            //设定列表项高度值
            sp_itemHeight.floatValue = 50;
            sp_itemHeight.serializedObject.ApplyModifiedProperties();

            sp_visibleItemCount.intValue = 9;
            sp_visibleItemCount.serializedObject.ApplyModifiedProperties();

            #region 获取图标
            warnIcon = XGUI.GetBuiltInIcon("console.warnicon");
            normalIcon = XGUI.GetBuiltInIcon("d_Button Icon");
            Icon_Using = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_elements/Icon_ele_using");
            btn_icon_edit_released = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_elements/edit_r");
            btn_icon_edit_press = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_elements/edit_p");
            clean_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_elements/clean_p");
            clean_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_elements/clean_r");
            clear_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_elements/clear_p");
            clear_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_elements/clear_r");
            create_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_elements/create_p");
            create_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_elements/create_r");
            delete_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_elements/delete_p");
            delete_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_elements/delete_r");
            tip_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_elements/tip_r");
            tip_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_elements/tip_p");
            fastcount_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_elements/fastcount_r");
            fastcount_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_library_elements/fastcount_p");
            #endregion

            if (!Application.isPlaying)
            {
                ClearLibraryPreloads();
            }

            #region ReorderableList - ElementInfoList
            ElementInfoList = new ReorderableList(serializedObject, sp_ElementLibrary, true, true, true, true);
            ElementInfoList.drawElementCallback = ElementInfoList_Original_DrawElementCallback;
            #endregion
        }

        private void OnDisable()
        {
            if (target != null)
            {
                if (!Application.isPlaying)
                {
                    ClearLibraryPreloads();
                }

                sp_ElementInfoList_Original_Scroller.vector2Value = Vector2.zero;
                sp_ElementInfoList_Original_Scroller.serializedObject.ApplyModifiedProperties();

                sp_LocationSelectedIndex.intValue = -1;
                sp_LocationSelectedIndex.serializedObject.ApplyModifiedProperties();

                sp_SelectedIndex.intValue = -1;
                sp_SelectedIndex.serializedObject.ApplyModifiedProperties();

                sp_Find.stringValue = string.Empty;
                sp_Find.serializedObject.ApplyModifiedProperties();

                sp_Highlight.stringValue = string.Empty;
                sp_Highlight.serializedObject.ApplyModifiedProperties();
            }
        }

        #region ElementInfoList_Original      

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
        private void ElementInfoList_Original_DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

            SerializedProperty prop = sp_ElementLibrary.GetArrayElementAtIndex(index);
            SerializedProperty sp_name = prop.FindPropertyRelative("Name");
            SerializedProperty sp_initalcount = prop.FindPropertyRelative("InitializeCount");
            SerializedProperty sp_usedcount = prop.FindPropertyRelative("UsedCount");
            SerializedProperty sp_recyclecount = prop.FindPropertyRelative("RecycledCount");
            SerializedProperty sp_Index = prop.FindPropertyRelative("NextIndex");
            SerializedProperty sp_target = prop.FindPropertyRelative("Target");

            if (sp_target.objectReferenceValue != null)
            {
                sp_name.stringValue = sp_target.objectReferenceValue.name;
            }

            #region 序号
            drawelement_rect.Set(rect.x + 15, rect.y + 5, 30, 20);
            XGUI.gui_label(
              rect: drawelement_rect,
              text: new GUIContent(index.ToString("D2")),
              text_color: Color.gray,
              size: XGUIFontSize.S,
              clipping: TextClipping.Clip,
              anchor: TextAnchor.MiddleLeft,
              offset: new Vector2(0, 0),
              font_style: FontStyle.Normal);
            #endregion

            #region 元素名称
            drawelement_rect.Set(rect.x + 39, rect.y + 5, rect.width - 80, 20);
            XGUI.gui_label(
              rect: drawelement_rect,
              text: new GUIContent($"<color=#8F8F8F>元素： </color>{sp_name.stringValue}"),
              text_color: Color.white,
              size: XGUIFontSize.M,
              clipping: TextClipping.Clip,
              anchor: TextAnchor.MiddleLeft,
              offset: new Vector2(0, 0),
              font_style: FontStyle.Normal);
            #endregion

            #region 是否正在使用中图标           
            if (sp_usedcount.intValue > 0)
            {
                if (XGUI.GetCurrentWindowWidth() >= 400)
                {
                    drawelement_rect.Set(rect.width - 80, rect.y + 17, 15, 15);
                    XGUI.gui_icon(
                      rect: drawelement_rect,
                      icon: Icon_Using,
                      color: XHud_Dashboard.Theme_Primary);
                }
            }
            #endregion

            #region 元素库状态概要
            drawelement_rect.Set(rect.x + 40, rect.y + 28, rect.width - 80, 20);
            string info = $"<color=#8F8F8F>I :  </color> {sp_initalcount.intValue}  <color=#8F8F8F>  U :  </color> {sp_usedcount.intValue}  <color=#8F8F8F>  R :  </color> {sp_recyclecount.intValue}  <color=#8F8F8F>  D :  </color> {sp_Index.intValue}";
            XGUI.gui_label(
             rect: drawelement_rect,
             text: new GUIContent(info),
             text_color: Color.white,
             size: XGUIFontSize.M,
             clipping: TextClipping.Clip,
             anchor: TextAnchor.MiddleLeft,
             offset: new Vector2(0, 0),
             font_style: FontStyle.Normal);

            drawelement_rect.Set(rect.x + 15, rect.y + 33, 12, 12);
            XGUI.gui_icon(
              rect: drawelement_rect,
              icon: sp_target.objectReferenceValue == null ? warnIcon : normalIcon,
              color: sp_target.objectReferenceValue == null ? Color.yellow : XHud_Dashboard.Theme_Primary);

            #endregion

            BlockGUI(sp_name.stringValue);

            #region 辅助菜单
            // 检测鼠标点击事件
            Event e = Event.current;
            drawelement_rect.Set(rect.x, rect.y, rect.width, rect.height);
            // 检测鼠标点击事件                        
            if (e.type == EventType.MouseDown && e.button == 1 && drawelement_rect.Contains(e.mousePosition))
            {
                sp_SelectedIndex.intValue = index;

                // 创建右键菜单
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("D (克隆元素)"), false, () =>
                {
                    sp_ElementLibrary.InsertArrayElementAtIndex(index);
                    sp_ElementLibrary.serializedObject.ApplyModifiedProperties();
                });
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("E (修改元素项)"), false, () =>
                {
                    XHud_LibraryArg_Element_Item item = BaseScript.ElementLibrary[sp_SelectedIndex.intValue];
                    Open_Hud_Library_Element_Setter(item, index);
                });
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("S (根据自身布局参数发送到场景锚点)"), false, () =>
                {
                    XHud_Module_Element source_ele = BaseScript.ElementLibrary[index].Target;
                    if (source_ele.RMS_Enabled)
                    {
                        GameObject obj_ele = (GameObject)PrefabUtility.InstantiatePrefab(BaseScript.ElementLibrary[index].Target.gameObject, null);
                        Undo.RegisterCreatedObjectUndo(obj_ele, "CreateElement");
                        XHud_Module_Element ele = obj_ele.GetComponent<XHud_Module_Element>();

                        Element_RMS_LayoutData rms = ele.elelemt_RMS_Get(mgr.hm_RMS_GetCurrentSolution());
                        ele.RectTransform.SetParent(mgr.hm_ScreenElement_GetAnchored_RectTransform(rms.Anchor));
                        ele.RectTransform.pivot = rms.Pivot;
                        ele.RectTransform.anchorMin = rms.AnchorMin;
                        ele.RectTransform.anchorMax = rms.AnchorMax;
                        ele.RectTransform.anchoredPosition3D = rms.Position;
                        ele.RectTransform.localEulerAngles = rms.Euler;
                        ele.RectTransform.localScale = rms.Scale;
                        Selection.activeGameObject = obj_ele;
                    }
                    else
                    {
                        XGUI.dialog(
                            type: XGUIDialogType.警告,
                            windowtitle: "XHud - 元素库消息",
                            title: "目标元素RMS未开启",
                            msg: "您选择的目标元素并未开启 R M S 功能，无法执行此操作！",
                            ok: "明白",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.Theme_Primary);
                    }
                });
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("C (拷贝元素名称)"), false, () =>
                {
                    GUIUtility.systemCopyBuffer = sp_name.stringValue;
                });
                // 创建右键菜单
                menu.AddItem(new GUIContent("F (定位元素源)"), false, () =>
                {
                    EditorGUIUtility.PingObject(prop.FindPropertyRelative("Target").objectReferenceValue);
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
                tex_release: btn_icon_edit_released,
                tex_press: btn_icon_edit_press,
                tex_gui_color: Color.white,
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0),
                focus_name: "fav_btn"))
            {
                XHud_LibraryArg_Element_Item item = BaseScript.ElementLibrary[index];
                Open_Hud_Library_Element_Setter(item, index);
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
                if (XGUI.GetCurrentWindowWidth() >= 400)
                {
                    drawelement_rect.Set(rect.width - 135, rect.y + 15, 50, 20);
                    XGUI.gui_label(
                        rect: drawelement_rect,
                        text: new GUIContent("已定位"),
                        text_color: XHud_Dashboard.Theme_Primary,
                        size: XGUIFontSize.S,
                        clipping: TextClipping.Clip,
                        anchor: TextAnchor.MiddleLeft,
                        offset: new Vector2(0, 0),
                        font_style: FontStyle.Normal);
                }
            }
        }

        /// <summary>
        /// 绘制列表
        /// </summary>
        private void DrawElementInfoList_Original()
        {
            // 绘制滚动视图
            scrollview_rect = GUILayoutUtility.GetRect(0, sp_visibleItemCount.intValue * sp_itemHeight.floatValue);
            sp_ElementInfoList_Original_Scroller.vector2Value = GUI.BeginScrollView(scrollview_rect, sp_ElementInfoList_Original_Scroller.vector2Value, new Rect(0, 0, scrollview_rect.width - 50, ElementInfoList.count * sp_itemHeight.floatValue), false, true);

            // 计算可视区域的起始和结束索引
            int startIndex = Mathf.FloorToInt(sp_ElementInfoList_Original_Scroller.vector2Value.y / sp_itemHeight.floatValue);
            int endIndex = Mathf.CeilToInt((sp_ElementInfoList_Original_Scroller.vector2Value.y + scrollview_rect.height) / sp_itemHeight.floatValue);

            // 只绘制可视区域内的元素
            for (int i = startIndex; i < endIndex && i < ElementInfoList.count; i++)
            {
                SerializedProperty prop = sp_ElementLibrary.GetArrayElementAtIndex(i);
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

                ElementInfoList.drawElementCallback.Invoke(item_rect, i, i == ElementInfoList.index, true);

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
        private void ElementInfoList_Original_Remove(ReorderableList list)
        {
            if (list.count <= 0)
                return;
            sp_ElementLibrary.DeleteArrayElementAtIndex(list.index);
            sp_ElementLibrary.serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 列表增加
        /// </summary>
        /// <param name="list"></param>
        private void ElementInfoList_Original_Add(ReorderableList list)
        {
            if (sp_ElementLibrary.arraySize <= 0)
            {
                sp_ElementLibrary.InsertArrayElementAtIndex(0);
                sp_ElementLibrary.GetArrayElementAtIndex(0).FindPropertyRelative("Name").stringValue = "NewElement";
                sp_ElementLibrary.GetArrayElementAtIndex(0).FindPropertyRelative("Target").objectReferenceValue = null;
                sp_ElementLibrary.GetArrayElementAtIndex(0).FindPropertyRelative("InitializeCount").intValue = 1;
            }
            else
            {
                sp_ElementLibrary.InsertArrayElementAtIndex(list.index);

                sp_ElementLibrary.GetArrayElementAtIndex(list.index).FindPropertyRelative("Name").stringValue = "NewElement";
                sp_ElementLibrary.GetArrayElementAtIndex(list.index).FindPropertyRelative("Target").objectReferenceValue = null;
                sp_ElementLibrary.GetArrayElementAtIndex(list.index).FindPropertyRelative("InitializeCount").intValue = 1;
            }
            sp_ElementLibrary.serializedObject.ApplyModifiedProperties();
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
                title_text: "XHud  -  元素库",
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
                    BaseScript.ElementLibrary_Location_Find(sp_Find.stringValue);
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

                    #region 清理所有元素的队列
                    if (XGUI.layout_button(
                        tooltip: "清理所有元素的队列",
                        tex_release: clean_r,
                        tex_press: clean_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: 14,
                        height: 14))
                    {
                        string res = XGUI.dialog(
                            type: XGUIDialogType.警告,
                            windowtitle: "XHud - 元素库消息",
                            title: "清空队列元素",
                            msg: "您是否确认要清空所有元素项下的预生成队列元素？",
                            ok: "暂不",
                            cancel: "清空",
                            PrimaryIndex: 1,
                            usemodal: true,
                            themecolor: XHud_Dashboard.Theme_Primary);

                        if (res == "暂不")
                            return;
                        ClearLibraryPreloads();
                    }
                    #endregion

                    GUILayout.FlexibleSpace();

                    #region 清空所有元素项
                    if (XGUI.layout_button(
                        tooltip: "清空所有元素项",
                        tex_release: clear_r,
                        tex_press: clear_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: 14,
                        height: 14))
                    {
                        string res = XGUI.dialog(
                            type: XGUIDialogType.警告,
                            windowtitle: "XHud - 元素库消息",
                            title: "清空元素项",
                            msg: "您是否确认要清空所有元素项？",
                            ok: "暂不",
                            cancel: "清空",
                            PrimaryIndex: 1,
                            usemodal: true,
                            themecolor: XHud_Dashboard.Theme_Primary);

                        if (res == "暂不")
                            return;

                        for (int i = 0; i < sp_ElementLibrary.arraySize; i++)
                        {
                            SerializedProperty preloads = sp_ElementLibrary.GetArrayElementAtIndex(i).FindPropertyRelative("PreloadElements");
                            preloads.ClearArray();
                            preloads.serializedObject.ApplyModifiedProperties();
                        }

                        sp_ElementLibrary.ClearArray();
                        sp_ElementLibrary.serializedObject.ApplyModifiedProperties();
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
                        ElementInfoList_Original_Add(ElementInfoList);
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
                        ElementInfoList_Original_Remove(ElementInfoList);
                    }
                    #endregion

                    GUILayout.FlexibleSpace();

                    #region 帮助
                    if (XGUI.layout_button(
                        tooltip: "帮助",
                        tex_release: tip_r,
                        tex_press: tip_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: 14,
                        height: 14))
                    {
                        string h_color = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary, true);
                        string msg = $"<b><color={h_color}>I</color></b>  -  在对象池初始化时，会预先实例化这么多个元素放入池中 \n" +
                            $"<b><color={h_color}>U</color></b>  -  已使用的数量（正在活跃使用的元素数） \n" +
                            $"<b><color={h_color}>R</color></b>  -  已回收的数量（空闲可重新被分配使用的元素数） \n" +
                            $"<b><color={h_color}>D</color></b>  -  一个要分配的元素的索引，每次从池中获取元素时，指向下一个要分配的元素索引";

                        EditorApplication.delayCall += () =>
                        {
                            XGUI.dialog(
                                type: XGUIDialogType.警告,
                                windowtitle: "XHud - 元素库消息",
                                title: "状态标记说明",
                                msg: msg,
                                ok: "明白",
                                PrimaryIndex: 0,
                                usemodal: true,
                                themecolor: XHud_Dashboard.Theme_Primary);
                        };
                    }
                    #endregion

                    GUILayout.FlexibleSpace();

                    #region 批量数量设定
                    if (XGUI.layout_button(
                        tooltip: "批量数量设定",
                        tex_release: fastcount_r,
                        tex_press: fastcount_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: 14,
                        height: 14))
                    {
                        Open_Hud_Library_Element_PreloadCount_Setter();
                    }
                    #endregion

                    XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
                }
            }
            #endregion

            #region 元素库列表
            XGUI.layout_group_start(
               type: XGUIContainerType.Vertical,
               bg_fill: XGUIFilled.缺口纯色边框,
               bg_color: XGUIColor.亮白,
               bg_color_gui: XHud_Dashboard.Theme_Group,
               title: "元素库列表",
               title_size: XGUIFontSize.M,
               title_text_color: XHud_Dashboard.Theme_Primary,
               title_clipping: TextClipping.Clip,
               padding: new RectOffset(10, 10, 15, 15));

            DrawElementInfoList_Original();

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 拖放操作

            Event evt = Event.current;
            if (scrollview_rect.Contains(evt.mousePosition))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    Object[] dropobjs = DragAndDrop.objectReferences;
                    List<XGUIDialogListDatas> valids = new List<XGUIDialogListDatas>();
                    foreach (var item in dropobjs)
                    {
                        GameObject el = (GameObject)item;
                        XHud_LibraryArg_Element_Item it = new XHud_LibraryArg_Element_Item();
                        it.Target = el.GetComponent<XHud_Module_Element>();
                        if (it.Target != null)
                        {
                            bool isExist = false;
                            for (int s = 0; s < sp_ElementLibrary.arraySize; s++)
                            {
                                SerializedProperty ser = sp_ElementLibrary.GetArrayElementAtIndex(s);
                                SerializedProperty ser_target = ser.FindPropertyRelative("Target");
                                if (ser_target.objectReferenceValue.name == el.name)
                                {
                                    isExist = true;
                                    break;
                                }
                            }
                            if (isExist)
                            {
                                XGUI.dialog(
                                    type: XGUIDialogType.警告,
                                    windowtitle: "XHud - 元素库消息",
                                    title: "清空元素项",
                                    msg: $"此元素预制体已存在于该库中！请勿重复添加！预制体名称： {el.name}  标识名称： {it.Target.Indicator}",
                                    ok: "明白",
                                    PrimaryIndex: 0,
                                    usemodal: true,
                                    themecolor: XHud_Dashboard.Theme_Primary);
                            }
                            else
                            {
                                it.InitializeCount = 2;
                                it.Name = it.Target.name;

                                sp_ElementLibrary.InsertArrayElementAtIndex(sp_ElementLibrary.arraySize);
                                SerializedProperty root = sp_ElementLibrary.GetArrayElementAtIndex(sp_ElementLibrary.arraySize - 1);
                                SerializedProperty sp_target = root.FindPropertyRelative("Target");
                                SerializedProperty sp_Initiacount = root.FindPropertyRelative("InitializeCount");
                                SerializedProperty sp_Name = root.FindPropertyRelative("Name");
                                sp_target.objectReferenceValue = it.Target;
                                sp_Initiacount.intValue = it.InitializeCount;
                                sp_Name.stringValue = it.Name;

                                valids.Add(new XGUIDialogListDatas("添加到元素库", sp_Name.stringValue, "成功添加"));
                            }
                        }
                    }

                    if (valids.Count > 0)
                    {
                        XGUI.dialog_listview(
                            datas: valids.ToArray(),
                            type: XGUIDialogType.通知,
                            windowtitle: "示例弹窗对话框",
                            title: "这是一个列表项弹窗",
                            msg: "这是一个模态列表项弹窗",
                            ok: "明白",
                            PrimaryIndex: 0,
                            usemodal: false,
                            themecolor: XHud_Dashboard.Theme_Primary);
                    }
                }
            }
            #endregion

            serializedObject.ApplyModifiedProperties();
        }

        private void ClearLibraryPreloads()
        {
            for (int i = 0; i < sp_ElementLibrary.arraySize; i++)
            {
                SerializedProperty prop = sp_ElementLibrary.GetArrayElementAtIndex(i);
                SerializedProperty preloads = prop.FindPropertyRelative("PreloadElements");
                SerializedProperty UsedCount = prop.FindPropertyRelative("UsedCount");
                SerializedProperty RecycledCount = prop.FindPropertyRelative("RecycledCount");
                SerializedProperty NextIndex = prop.FindPropertyRelative("NextIndex");
                SerializedProperty Root = prop.FindPropertyRelative("Root");

                UsedCount.intValue = 0;
                RecycledCount.intValue = 0;
                NextIndex.intValue = 0;
                preloads.ClearArray();
                Root.objectReferenceValue = null;
                prop.serializedObject.ApplyModifiedProperties();
            }
        }

        #region 辅助
        /// <summary>
        /// 打开元素库项修改器
        /// </summary>
        public void Open_Hud_Library_Element_Setter(XHud_LibraryArg_Element_Item item, int index)
        {
            Editor_XHud_LibrarySetTool_Element window = EditorWindow.GetWindow<Editor_XHud_LibrarySetTool_Element>(true);

            window.titleContent = new GUIContent("XHud 元素项修改器");
            XGUI.CenterEditorWindow(new Vector2Int(500, 440), window);
            window.SetLibraryItem(item);
            window.SetTargetLibName(sp_LibraryName.stringValue);
            window.SetTargetLib(BaseScript);
            window.ModifiedIndex = index;
            window.Show();
        }

        /// <summary>
        /// 打开元素库预加载数量修改器
        /// </summary>
        public void Open_Hud_Library_Element_PreloadCount_Setter()
        {
            Editor_XHud_LibrarySetTool_ElementLib_CountsSet window = EditorWindow.GetWindow<Editor_XHud_LibrarySetTool_ElementLib_CountsSet>(true);
            window.titleContent = new GUIContent("XHud 元素库预加载数量修改器");
            XGUI.CenterEditorWindow(new Vector2Int(300, 260), window);
            window.SetTargetLib(BaseScript);
            window.Show();
        }
        #endregion
    }
}