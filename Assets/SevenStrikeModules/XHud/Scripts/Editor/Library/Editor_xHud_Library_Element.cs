namespace SevenStrikeModules.XHud.Editor
{
    using SevenStrikeModules.XHud.Enums;
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
        private Texture2D warnIcon, Icon_Using, btn_icon_details_released, btn_icon_details_press, clean_p, clean_r, clear_p, clear_r, create_p, create_r, delete_p, delete_r;
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

            sp_visibleItemCount.intValue = 10;
            sp_visibleItemCount.serializedObject.ApplyModifiedProperties();

            #region 获取图标
            warnIcon = EditorGUIUtility.IconContent("console.warnicon").image as Texture2D;
            Icon_Using = Editor_XHud_GUI.GetIcon("Icons_XHud_ElementLibrary/Icon_ele_using");
            btn_icon_details_released = Editor_XHud_GUI.GetIcon("Icons_XHud_ElementLibrary/detail_r");
            btn_icon_details_press = Editor_XHud_GUI.GetIcon("Icons_XHud_ElementLibrary/detail_p");
            clean_p = Editor_XHud_GUI.GetIcon("Icons_XHud_ElementLibrary/clean_p");
            clean_r = Editor_XHud_GUI.GetIcon("Icons_XHud_ElementLibrary/clean_r");
            clear_p = Editor_XHud_GUI.GetIcon("Icons_XHud_ElementLibrary/clear_p");
            clear_r = Editor_XHud_GUI.GetIcon("Icons_XHud_ElementLibrary/clear_r");
            create_p = Editor_XHud_GUI.GetIcon("Icons_XHud_ElementLibrary/create_p");
            create_r = Editor_XHud_GUI.GetIcon("Icons_XHud_ElementLibrary/create_r");
            delete_p = Editor_XHud_GUI.GetIcon("Icons_XHud_ElementLibrary/delete_p");
            delete_r = Editor_XHud_GUI.GetIcon("Icons_XHud_ElementLibrary/delete_r");
            #endregion

            #region ReorderableList - ElementInfoList
            ElementInfoList = new ReorderableList(serializedObject, sp_ElementLibrary, true, true, true, true);
            ElementInfoList.drawElementCallback = ElementInfoList_Original_DrawElementCallback;
            #endregion
        }

        private void OnDisable()
        {
            if (target != null)
            {
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
        Rect x_dragarea;
        Rect dragarea;
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

            drawelement_rect.Set(rect.x + 15, rect.y + 5, 30, 20);
            Editor_XHud_GUI.Gui_Labelfield(drawelement_rect, index.ToString("D2"), HudFilled.无, HudColor.无, Color.gray, TextAnchor.MiddleLeft, Vector2.zero, 11);

            #region 标识名称
            drawelement_rect.Set(rect.x + 39, rect.y + 5, 35, 20);
            Editor_XHud_GUI.Gui_Labelfield(drawelement_rect, "元素：", HudFilled.无, HudColor.无, Editor_XHud_GUI.GetColor(HudColor.亮白), TextAnchor.MiddleLeft, Vector2.zero, 12);
            drawelement_rect.Set(rect.x + 77, rect.y + 5, rect.width - 140, 20);

            Editor_XHud_GUI.Gui_Labelfield(drawelement_rect, sp_name.stringValue, HudFilled.无, HudColor.无, XHud_Dashboard.Theme_Primary, TextAnchor.MiddleLeft, Vector2.zero, 12, TextClipping.Ellipsis);
            #endregion

            #region 是否正在使用中图标           
            if (sp_usedcount.intValue > 0)
            {
                GUIContent content_used = new GUIContent();
                content_used.image = Icon_Using;
                content_used.tooltip = "正在使用";
                drawelement_rect.Set(rect.width - 90, rect.y + 14, 22, 22);
                Editor_XHud_GUI.Gui_Icon(drawelement_rect, content_used);
            }
            #endregion

            #region 元素库状态概要
            drawelement_rect.Set(rect.x + 39, rect.y + 28, 40, 20);
            Editor_XHud_GUI.Gui_Labelfield_Thin(drawelement_rect, $"<b>I</b>  <color=#ffffff>{sp_initalcount.intValue}</color>", HudFilled.无, HudColor.无, Color.gray, TextAnchor.MiddleLeft, Vector2.zero, 11, false, false, true);
            drawelement_rect.Set(rect.x + 94, rect.y + 28, 40, 20);
            Editor_XHud_GUI.Gui_Labelfield_Thin(drawelement_rect, $"<b>U</b>  <color=#ffffff>{sp_usedcount.intValue}</color>", HudFilled.无, HudColor.无, Color.gray, TextAnchor.MiddleLeft, Vector2.zero, 11, false, false, true);
            drawelement_rect.Set(rect.x + 149, rect.y + 28, 40, 20);
            Editor_XHud_GUI.Gui_Labelfield_Thin(drawelement_rect, $"<b>R</b>  <color=#ffffff>{sp_recyclecount.intValue}</color>", HudFilled.无, HudColor.无, Color.gray, TextAnchor.MiddleLeft, Vector2.zero, 11, false, false, true);
            drawelement_rect.Set(rect.x + 204, rect.y + 28, 40, 20);
            Editor_XHud_GUI.Gui_Labelfield_Thin(drawelement_rect, $"<b>D</b>  <color=#ffffff>{sp_Index.intValue}</color>", HudFilled.无, HudColor.无, Color.gray, TextAnchor.MiddleLeft, Vector2.zero, 11, false, false, true);

            if (sp_target.objectReferenceValue == null)
            {
                GUIContent content = new GUIContent();
                content.image = warnIcon;
                content.tooltip = "已丢失元素源物体";
                drawelement_rect.Set(rect.x + 15, rect.y + 28, 15, 15);
                Editor_XHud_GUI.Gui_Icon(drawelement_rect, content);
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
                        ele.RectTransform.SetParent(mgr.hm_Layout_GetAnchor(rms.Anchor));
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
                        Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 元素库消息", "目标元素RMS未开启", "您选择的目标元素并未开启 R M S 功能，无法执行此操作！", "明白", 0);
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
            if (Editor_XHud_GUI.Gui_Button(drawelement_rect, btn_icon_details_released, btn_icon_details_press, true, "", "", Color.white))
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
                drawelement_rect.Set(rect.width - 150, rect.y + 15, 50, 20);
                Editor_XHud_GUI.Gui_Labelfield(drawelement_rect, "已定位", HudFilled.无, HudColor.亮白, XHud_Dashboard.Theme_Primary, TextAnchor.MiddleRight, Vector2.zero, 11);
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
            Editor_XHud_GUI.Gui_Layout_Banner(HudFilled.实体, HudColor.亮白, "XHud - 元素库", Color.black);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_XHud_GUI.Gui_Layout_Property_Field("元素库名称", sp_LibraryName);
            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Horizontal_End();

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_XHud_GUI.Gui_Layout_Property_Field("元素库说明", sp_LibraryDescription);
            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Horizontal_End();

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            EditorGUI.BeginChangeCheck();
            Editor_XHud_GUI.Gui_Layout_Property_Field("过滤（包含）", sp_Highlight);
            if (EditorGUI.EndChangeCheck())
            {
                sp_LocationSelectedIndex.intValue = -1;
                sp_LocationSelectedIndex.serializedObject.ApplyModifiedProperties();
            }
            Editor_XHud_GUI.Gui_Layout_Space(5);
            EditorGUI.BeginChangeCheck();
            Editor_XHud_GUI.Gui_Layout_Property_Field("查找（精确）", sp_Find);
            if (EditorGUI.EndChangeCheck())
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
            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Horizontal_End();

            Editor_XHud_GUI.Gui_Layout_Space(5);

            if (string.IsNullOrEmpty(sp_Highlight.stringValue))
            {
                if (!Application.isPlaying)
                {
                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_FlexSpace();

                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "清理所有元素的队列", clean_r, clean_p, 4))
                    {
                        string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 元素库消息", "清空队列元素", "您是否确认要清空所有元素项下的预生成队列元素？", "暂不", "清空", 1);
                        if (res == "暂不")
                            return;
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
                    Editor_XHud_GUI.Gui_Layout_Space(35);
                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "清空所有元素项", clear_r, clear_p, 4))
                    {
                        string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 元素库消息", "清空元素项", "您是否确认要清空所有元素项？", "暂不", "清空", 0);
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
                    Editor_XHud_GUI.Gui_Layout_Space(35);
                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "添加项", create_r, create_p, 4))
                    {
                        ElementInfoList_Original_Add(ElementInfoList);
                    }
                    Editor_XHud_GUI.Gui_Layout_Space(35);
                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "删除项", delete_r, delete_p, 4))
                    {
                        ElementInfoList_Original_Remove(ElementInfoList);
                    }
                    GUILayout.FlexibleSpace();
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Seperator(1, XHud_Dashboard.Theme_SeperateLine);
                    x_dragarea = GUILayoutUtility.GetLastRect();
                    Editor_XHud_GUI.Gui_Layout_Space(10);

                    #region 拖放操作

                    dragarea = new Rect((x_dragarea.width / 2) - 65, x_dragarea.y + 10, 200, 33);

                    Event evt = Event.current;
                    if (dragarea.Contains(evt.mousePosition))
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                        if (evt.type == EventType.DragPerform)
                        {
                            DragAndDrop.AcceptDrag();
                            Object[] dropobjs = DragAndDrop.objectReferences;
                            string names = "";
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
                                        EditorUtility.DisplayDialog("提示", "此元素预制体已存在于该库中！请勿重复添加！\n\n" + "预制体名称：" + el.name + "\n标识名称：" + it.Target.Indicator, "明白");
                                        continue;
                                    }
                                    else
                                    {
                                        it.InitializeCount = 1;
                                        it.Name = it.Target.name;

                                        sp_ElementLibrary.InsertArrayElementAtIndex(sp_ElementLibrary.arraySize);
                                        SerializedProperty root = sp_ElementLibrary.GetArrayElementAtIndex(sp_ElementLibrary.arraySize - 1);
                                        SerializedProperty sp_target = root.FindPropertyRelative("Target");
                                        SerializedProperty sp_Initiacount = root.FindPropertyRelative("InitializeCount");
                                        SerializedProperty sp_Name = root.FindPropertyRelative("Name");
                                        sp_target.objectReferenceValue = it.Target;
                                        sp_Initiacount.intValue = it.InitializeCount;
                                        sp_Name.stringValue = it.Name;

                                        names += sp_Name.stringValue + "\n";
                                    }
                                }
                            }

                            if (!string.IsNullOrEmpty(names))
                                EditorUtility.DisplayDialog("提示", "所选有效元素 : \n\n" + names + "\n已添加到元素库中！", "明白");
                        }
                    }

                    Editor_XHud_GUI.Gui_Box_Style(dragarea, HudFilled.实体, HudColor.深空灰);
                    Editor_XHud_GUI.Gui_Labelfield(dragarea, "拖放Hud元素到此处快速创建", HudFilled.无, HudColor.无, Color.gray, TextAnchor.MiddleCenter, new Vector2(0, 0), 11);
                    #endregion
                }
            }
            else
            {
                Editor_XHud_GUI.Gui_Layout_LabelfieldThin("当前为过滤筛选状态", HudFilled.无, HudColor.无, XHud_Dashboard.Theme_Primary, TextAnchor.MiddleCenter, new Vector2(0, 0), 12);
            }

            Editor_XHud_GUI.Gui_Layout_Space(30);

            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "元素库列表", Color.white);
            Editor_XHud_GUI.Gui_Layout_Space(5);

            DrawElementInfoList_Original();

            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            serializedObject.ApplyModifiedProperties();
        }

        #region 辅助

        /// <summary>
        /// 打开元素库项修改器
        /// </summary>
        public void Open_Hud_Library_Element_Setter(XHud_LibraryArg_Element_Item item, int index)
        {
            Editor_XHud_LibrarySetTool_Element window = EditorWindow.GetWindow<Editor_XHud_LibrarySetTool_Element>(true);

            window.titleContent = new GUIContent("XHud 元素库修改器");
            Editor_XHud_GUI.CenterEditorWindow(new Vector2Int(500, 450), window);
            window.SetLibraryItem(item);
            window.SetTargetLibName(sp_LibraryName.stringValue);
            window.SetTargetLib(BaseScript);
            window.ModifiedIndex = index;
            window.Show();
        }
        #endregion
    }
}