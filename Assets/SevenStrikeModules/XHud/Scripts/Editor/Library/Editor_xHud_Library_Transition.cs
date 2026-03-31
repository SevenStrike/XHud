namespace SevenStrikeModules.XHud
{
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.GuiLib;
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

        #region 字体
        private Font Font_Bold;
        private Font Font_Light;
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
            warnIcon = EditorGUIUtility.IconContent("console.warnicon").image as Texture2D;
            btn_icon_details_released = Editor_XHud_GUI.GetIcon("Icons_XHud_TransitionLibrary/detail_r");
            btn_icon_details_press = Editor_XHud_GUI.GetIcon("Icons_XHud_TransitionLibrary/detail_p");
            analyze_p = Editor_XHud_GUI.GetIcon("Icons_XHud_TransitionLibrary/analyze_p");
            analyze_r = Editor_XHud_GUI.GetIcon("Icons_XHud_TransitionLibrary/analyze_r");
            clear_p = Editor_XHud_GUI.GetIcon("Icons_XHud_TransitionLibrary/clear_p");
            clear_r = Editor_XHud_GUI.GetIcon("Icons_XHud_TransitionLibrary/clear_r");
            create_p = Editor_XHud_GUI.GetIcon("Icons_XHud_TransitionLibrary/create_p");
            create_r = Editor_XHud_GUI.GetIcon("Icons_XHud_TransitionLibrary/create_r");
            delete_p = Editor_XHud_GUI.GetIcon("Icons_XHud_TransitionLibrary/delete_p");
            delete_r = Editor_XHud_GUI.GetIcon("Icons_XHud_TransitionLibrary/delete_r");
            skip = Editor_XHud_GUI.GetIcon("Icons_XHud_TransitionLibrary/skip");
            total = Editor_XHud_GUI.GetIcon("Icons_XHud_TransitionLibrary/total");
            last = Editor_XHud_GUI.GetIcon("Icons_XHud_TransitionLibrary/last");
            #endregion

            //设定列表项高度值
            sp_itemHeight.floatValue = 75;
            sp_itemHeight.serializedObject.ApplyModifiedProperties();

            sp_visibleItemCount.intValue = 8;
            sp_visibleItemCount.serializedObject.ApplyModifiedProperties();

            blocked_col = new Color(0, 0, 0, blocked_alp);

            Font_Bold = Editor_XHud_GUI.GetFont("SS_Editor_Bold");
            Font_Light = Editor_XHud_GUI.GetFont("SS_Editor_Light");

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

            drawelement_rect.Set(rect.x + 15, rect.y + 5, 30, 20);
            Editor_XHud_GUI.Gui_Labelfield(drawelement_rect, index.ToString("D2"), HudFilled.无, HudColor.无, Color.gray, TextAnchor.MiddleLeft, Vector2.zero, 11);

            #region 标识名称
            drawelement_rect.Set(rect.x + 40, rect.y + 5, rect.width - 140, 20);
            Editor_XHud_GUI.Gui_Labelfield(drawelement_rect, sp_name.stringValue, HudFilled.无, HudColor.无, XHud_Dashboard.Theme_Primary, TextAnchor.MiddleLeft, Vector2.zero, 12, TextClipping.Ellipsis);
            #endregion

            #region 说明
            drawelement_rect.Set(rect.x + 40, rect.y + 25, rect.width - 160, 20);
            Editor_XHud_GUI.Gui_Labelfield_Thin_WithClipping(drawelement_rect, sp_des.stringValue, HudFilled.无, HudColor.无, Color.white * 0.65f, TextAnchor.MiddleLeft, Vector2.zero, 11, true, true, true, TextClipping.Ellipsis);
            #endregion

            #region 帧信息

            float AddedHeight = 16;

            drawelement_rect.Set(rect.x + 40, rect.y + 35 + AddedHeight, 14, 14);
            Editor_XHud_GUI.Gui_Icon(drawelement_rect, total);

            drawelement_rect.Set(rect.x + 110, rect.y + 35 + AddedHeight, 14, 14);
            Editor_XHud_GUI.Gui_Icon(drawelement_rect, last);

            drawelement_rect.Set(rect.x + 180, rect.y + 35 + AddedHeight, 14, 14);
            Editor_XHud_GUI.Gui_Icon(drawelement_rect, skip);

            drawelement_rect.Set(rect.x + 70, rect.y + 33 + AddedHeight, 80, 20);
            Editor_XHud_GUI.Gui_Labelfield(drawelement_rect, sp_TotalFramesCount.intValue.ToString(), HudFilled.无, HudColor.无, Color.white * 0.9f, TextAnchor.MiddleLeft, Vector2.zero, 11, true, TextClipping.Ellipsis, true, Font_Light);

            drawelement_rect.Set(rect.x + 140, rect.y + 33 + AddedHeight, 80, 20);
            Editor_XHud_GUI.Gui_Labelfield(drawelement_rect, sp_LastFrameIndex.intValue.ToString(), HudFilled.无, HudColor.无, Color.white * 0.9f, TextAnchor.MiddleLeft, Vector2.zero, 11, true, TextClipping.Ellipsis, true, Font_Light);

            drawelement_rect.Set(rect.x + 210, rect.y + 33 + AddedHeight, 80, 20);
            Editor_XHud_GUI.Gui_Labelfield(drawelement_rect, sp_SkipFrame.intValue.ToString(), HudFilled.无, HudColor.无, Color.white * 0.9f, TextAnchor.MiddleLeft, Vector2.zero, 11, true, TextClipping.Ellipsis, true, Font_Light);
            #endregion

            #region 转场项的序列帧状态
            if (sp_Frames.arraySize <= 0)
            {
                GUIContent content = new GUIContent();
                content.image = warnIcon;
                content.tooltip = "未找到任何序列帧图像";
                drawelement_rect.Set(rect.x + 15, rect.y + 35, 15, 15);
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
            if (Editor_XHud_GUI.Gui_Button(drawelement_rect, btn_icon_details_released, btn_icon_details_press, true, "", "", Color.white))
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
                Editor_XHud_GUI.Gui_Labelfield(drawelement_rect, "已定位", HudFilled.无, HudColor.亮白, XHud_Dashboard.Theme_Primary, TextAnchor.MiddleRight, Vector2.zero, 11);
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
            Editor_XHud_GUI.Gui_Layout_Banner(HudFilled.实体, HudColor.亮白, "XHud - 转场库", Color.black);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_XHud_GUI.Gui_Layout_Property_Field("转场库名称", sp_LibraryName);
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
                    BaseScript.TransitionLibrary_Location_Find(sp_Find.stringValue);
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

                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "分析帧数据", analyze_r, analyze_p, 4))
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
                    }
                    Editor_XHud_GUI.Gui_Layout_Space(35);
                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "清空所有转场项", clear_r, clear_p, 4))
                    {
                        string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 转场库消息", "清空所有转场", "是否清空所有转场项？请注意！如果您的场景中转场组件用到了该转场库中的转场效果项，清空后会导致转场信息丢失，请谨慎操作！", "清空", "暂不", 1);
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
                    }
                    Editor_XHud_GUI.Gui_Layout_Space(35);
                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "添加项", create_r, create_p, 4))
                    {
                        TransitionInfoList_Original_Add(TrasitionInfoList);
                    }
                    Editor_XHud_GUI.Gui_Layout_Space(35);
                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "删除项", delete_r, delete_p, 4))
                    {
                        TransitionInfoList_Original_Remove(TrasitionInfoList);
                    }
                    GUILayout.FlexibleSpace();
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Seperator(1, XHud_Dashboard.Theme_SeperateLine);
                    Editor_XHud_GUI.Gui_Layout_Space(10);
                }
            }
            else
            {
                Editor_XHud_GUI.Gui_Layout_LabelfieldThin("当前为过滤筛选状态", HudFilled.无, HudColor.无, XHud_Dashboard.Theme_Primary, TextAnchor.MiddleCenter, new Vector2(0, 0), 12);
            }

            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "转场库列表", Color.white);
            Editor_XHud_GUI.Gui_Layout_Space(5);

            DrawTransitionInfoList_Original();

            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();

            serializedObject.ApplyModifiedProperties();
        }

        #region 辅助
        /// <summary>
        /// 转场库修改器
        /// </summary>
        public void OpenParameterSetter(XHud_LibraryArg_Transition info, int index)
        {
            Editor_XHud_LibrarySetTool_Transition window = EditorWindow.GetWindow<Editor_XHud_LibrarySetTool_Transition>(true);

            window.titleContent = new GUIContent("XHud 转场库修改器");
            Editor_XHud_GUI.CenterEditorWindow(new Vector2Int(800, 680), window);
            window.SetTitle("XHud 转场库修改器");
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