namespace SevenStrikeModules.XHud.Hud
{
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.GuiLib;
    using System.Collections.Generic;
    using System.IO;
    using TMPro;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    public class ExportTextStyles
    {
        public List<XHud_LibraryArg_TextStyle> TextStyleLibrary = new List<XHud_LibraryArg_TextStyle>();
    }

    [CustomEditor(typeof(XHud_Library_TextStyle))]
    public class Editor_XHud_Library_TextStyle : Editor
    {
        #region 组件 / 列表
        private XHud_Library_TextStyle BaseScript;
        private ReorderableList sp_TextStyleLibraryList;
        #endregion

        #region 序列化属性
        private SerializedProperty
            sp_TextStyleLibrary,
            sp_PreviewFontColor,
            sp_Preview_Font,
            sp_PreviewTypeText,
            sp_LibraryName,
            sp_itemHeight,
            sp_visibleItemCount,
            sp_ColorInfoList_Original_Scroller,
            sp_LocationSelectedIndex,
            sp_SelectedIndex,
            sp_Find,
            sp_Highlight,
            sp_PreviewFontStyle,
            sp_PreviewContent;
        #endregion

        #region 图标
        private Texture2D warnIcon, btn_icon_details_released, btn_icon_details_press, import_p, import_r, export_p, export_r, clear_p, clear_r, create_p, create_r, delete_p, delete_r;
        #endregion

        private GUIContent warning_content;

        /// <summary>
        /// 屏蔽颜色
        /// </summary>
        private Color blocked_col;
        /// <summary>
        /// 屏蔽颜色透明度
        /// </summary>
        private float blocked_alp = 0.45f;

        #region 字体
        /// <summary>
        /// 字体 - 粗体
        /// </summary>
        Font Font_Bold;
        /// <summary>
        /// 字体 - 细体
        /// </summary>
        Font Font_Light;
        #endregion

        bool sws;

        private void OnEnable()
        {
            BaseScript = (XHud_Library_TextStyle)target;

            #region 获取序列化属性
            sp_TextStyleLibrary = serializedObject.FindProperty("TextStyleLibrary");
            sp_LibraryName = serializedObject.FindProperty("LibraryName");
            sp_PreviewContent = serializedObject.FindProperty("PreviewContent");
            sp_PreviewFontColor = serializedObject.FindProperty("PreviewFontColor");
            sp_itemHeight = serializedObject.FindProperty("itemHeight");
            sp_visibleItemCount = serializedObject.FindProperty("visibleItemCount");
            sp_ColorInfoList_Original_Scroller = serializedObject.FindProperty("ColorInfoList_Original_Scroller");
            sp_LocationSelectedIndex = serializedObject.FindProperty("LocationSelectedIndex");
            sp_SelectedIndex = serializedObject.FindProperty("SelectedIndex");
            sp_Find = serializedObject.FindProperty("Find");
            sp_Highlight = serializedObject.FindProperty("Highlight");
            sp_Preview_Font = serializedObject.FindProperty("Preview_Font");
            sp_PreviewTypeText = serializedObject.FindProperty("PreviewTypeText");
            sp_PreviewFontStyle = serializedObject.FindProperty("PreviewStyle");
            #endregion

            blocked_col = new Color(0, 0, 0, blocked_alp);

            #region 获取图标
            btn_icon_details_released = Editor_XHud_GUI.GetIcon("Icons_Hud_TextStyleLibrary/detail_r");
            btn_icon_details_press = Editor_XHud_GUI.GetIcon("Icons_Hud_TextStyleLibrary/detail_p");
            import_p = Editor_XHud_GUI.GetIcon("Icons_Hud_TextStyleLibrary/import_p");
            import_r = Editor_XHud_GUI.GetIcon("Icons_Hud_TextStyleLibrary/import_r");
            export_p = Editor_XHud_GUI.GetIcon("Icons_Hud_TextStyleLibrary/export_p");
            export_r = Editor_XHud_GUI.GetIcon("Icons_Hud_TextStyleLibrary/export_r");
            clear_p = Editor_XHud_GUI.GetIcon("Icons_Hud_TextStyleLibrary/clear_p");
            clear_r = Editor_XHud_GUI.GetIcon("Icons_Hud_TextStyleLibrary/clear_r");
            create_p = Editor_XHud_GUI.GetIcon("Icons_Hud_TextStyleLibrary/create_p");
            create_r = Editor_XHud_GUI.GetIcon("Icons_Hud_TextStyleLibrary/create_r");
            delete_p = Editor_XHud_GUI.GetIcon("Icons_Hud_TextStyleLibrary/delete_p");
            delete_r = Editor_XHud_GUI.GetIcon("Icons_Hud_TextStyleLibrary/delete_r");
            Font_Bold = Editor_XHud_GUI.GetFont("MotionMarkFont");
            Font_Light = Editor_XHud_GUI.GetFont("SS_Editor_Light");
            warnIcon = EditorGUIUtility.IconContent("console.warnicon").image as Texture2D;
            #endregion

            sp_PreviewTypeText.stringValue = "-";
            sp_PreviewTypeText.serializedObject.ApplyModifiedProperties();

            //设定列表项高度值
            sp_itemHeight.floatValue = 60;
            sp_itemHeight.serializedObject.ApplyModifiedProperties();

            sp_visibleItemCount.intValue = 9;
            sp_visibleItemCount.serializedObject.ApplyModifiedProperties();

            #region ReorderableList - TextStyleInfoList
            sp_TextStyleLibraryList = new ReorderableList(serializedObject, sp_TextStyleLibrary, true, true, true, true);
            sp_TextStyleLibraryList.drawElementCallback = TextStyleInfoList_Original_DrawElementCallback;
            sp_TextStyleLibraryList.onAddCallback = TextStyleInfoList_Original_AddCallback;
            sp_TextStyleLibraryList.onRemoveCallback = TextStyleInfoList_Original_RemoveCallback;
            #endregion
        }

        private void OnDisable()
        {
            sp_ColorInfoList_Original_Scroller.vector2Value = Vector2.zero;
            sp_ColorInfoList_Original_Scroller.serializedObject.ApplyModifiedProperties();

            sp_SelectedIndex.intValue = -1;
            sp_SelectedIndex.serializedObject.ApplyModifiedProperties();

            sp_LocationSelectedIndex.intValue = -1;
            sp_LocationSelectedIndex.serializedObject.ApplyModifiedProperties();

            sp_Find.stringValue = string.Empty;
            sp_Find.serializedObject.ApplyModifiedProperties();

            sp_Highlight.stringValue = string.Empty;
            sp_Highlight.serializedObject.ApplyModifiedProperties();

            sp_PreviewTypeText.stringValue = string.Empty;
            sp_PreviewTypeText.serializedObject.ApplyModifiedProperties();

            sp_PreviewFontColor.colorValue = Color.white;
            sp_PreviewFontColor.serializedObject.ApplyModifiedProperties();

            sp_PreviewFontStyle.enumValueIndex = (int)FontStyle.Normal;
            sp_PreviewFontStyle.serializedObject.ApplyModifiedProperties();
        }

        #region TextStyleInfoList_Original      

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
        /// 添加样式
        /// </summary>
        /// <param name="list"></param>
        private void TextStyleInfoList_Original_AddCallback(ReorderableList list)
        {
            if (BaseScript.act_on_TextStyleChanged != null)
                BaseScript.act_on_TextStyleChanged();
        }

        /// <summary>
        /// 移除样式
        /// </summary>
        /// <param name="list"></param>
        private void TextStyleInfoList_Original_RemoveCallback(ReorderableList list)
        {
            if (BaseScript.act_on_TextStyleChanged != null)
                BaseScript.act_on_TextStyleChanged();
        }

        /// <summary>
        /// 绘制元素
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="index"></param>
        /// <param name="isActive"></param>
        /// <param name="isFocused"></param>
        private void TextStyleInfoList_Original_DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty prop = sp_TextStyleLibrary.GetArrayElementAtIndex(index);

            SerializedProperty sp_name = prop.FindPropertyRelative("Name");
            SerializedProperty sp_type = prop.FindPropertyRelative("Type");
            SerializedProperty sp_tmpfont = prop.FindPropertyRelative("tmp_font");
            SerializedProperty sp_tmpgra = prop.FindPropertyRelative("gra_Used");
            SerializedProperty sp_tmp_gramode = prop.FindPropertyRelative("gra_ColorMode");
            SerializedProperty sp_tmp_gra_col_a = prop.FindPropertyRelative("gra_A");
            SerializedProperty sp_tmp_gra_col_b = prop.FindPropertyRelative("gra_B");
            SerializedProperty sp_tmp_gra_col_c = prop.FindPropertyRelative("gra_C");
            SerializedProperty sp_tmp_gra_col_d = prop.FindPropertyRelative("gra_D");
            SerializedProperty sp_font = prop.FindPropertyRelative("Font");
            SerializedProperty sp_descrpt = prop.FindPropertyRelative("Description");
            SerializedProperty sp_fontcolor = prop.FindPropertyRelative("FontColor");
            SerializedProperty sp_tmpfontcolor = prop.FindPropertyRelative("tmp_color");

            xHud_TextType ttype = (xHud_TextType)sp_type.enumValueIndex;

            drawelement_rect.Set(rect.x + 15, rect.y + 8, 30, 20);
            Editor_XHud_GUI.Gui_Labelfield(drawelement_rect, index.ToString("D2"), HudFilled.无, HudColor.无, Color.gray, TextAnchor.MiddleLeft, Vector2.zero, 11);

            #region 标识名称
            drawelement_rect.Set(rect.x + 75, rect.y + 8, rect.width - 155, 20);
            Editor_XHud_GUI.Gui_Labelfield(drawelement_rect, sp_name.stringValue, HudFilled.无, HudColor.无, XHud_Dashboard.Theme_Primary, TextAnchor.MiddleLeft, Vector2.zero, 12, true, TextClipping.Ellipsis, true, Font_Bold);
            #endregion

            drawelement_rect.Set(rect.x + 38, rect.y + 33, rect.width - 160, 22);
            Editor_XHud_GUI.Gui_Labelfield(drawelement_rect, sp_descrpt.stringValue, HudFilled.无, HudColor.无, Color.gray, TextAnchor.MiddleLeft, Vector2.zero, 11, true, TextClipping.Ellipsis, true, Font_Light);

            drawelement_rect.Set(rect.width - 41.5f, rect.y + 30, 15, 2);
            EditorGUI.DrawRect(drawelement_rect, ttype == xHud_TextType.Text ? sp_fontcolor.colorValue : sp_tmpfontcolor.colorValue);

            #region 渐变色块
            if (ttype == xHud_TextType.TmpText)
            {
                if (sp_tmpgra.boolValue)
                {
                    ColorMode col_mod = (ColorMode)sp_tmp_gramode.enumValueIndex;
                    switch (col_mod)
                    {
                        case ColorMode.Single:
                            drawelement_rect.Set(rect.width - 41.5f, rect.y + 40, 15, 2);
                            EditorGUI.DrawRect(drawelement_rect, sp_tmp_gra_col_a.colorValue);
                            break;
                        case ColorMode.HorizontalGradient:
                            drawelement_rect.Set(rect.width - 41.5f, rect.y + 40, 10, 2);
                            EditorGUI.DrawRect(drawelement_rect, sp_tmp_gra_col_a.colorValue);
                            drawelement_rect.Set(rect.width - 34, rect.y + 40, 7.5f, 2);
                            EditorGUI.DrawRect(drawelement_rect, sp_tmp_gra_col_b.colorValue);
                            break;
                        case ColorMode.VerticalGradient:
                            drawelement_rect.Set(rect.width - 41.5f, rect.y + 40, 7.5f, 2);
                            EditorGUI.DrawRect(drawelement_rect, sp_tmp_gra_col_a.colorValue);
                            drawelement_rect.Set(rect.width - 34, rect.y + 40, 7.5f, 2);
                            EditorGUI.DrawRect(drawelement_rect, sp_tmp_gra_col_b.colorValue);
                            break;
                        case ColorMode.FourCornersGradient:
                            drawelement_rect.Set(rect.width - 41.5f, rect.y + 40, 3.75f, 2);
                            EditorGUI.DrawRect(drawelement_rect, sp_tmp_gra_col_a.colorValue);
                            drawelement_rect.Set(rect.width - 37.75f, rect.y + 40, 3.75f, 2);
                            EditorGUI.DrawRect(drawelement_rect, sp_tmp_gra_col_b.colorValue);
                            drawelement_rect.Set(rect.width - 34f, rect.y + 40, 3.75f, 2);
                            EditorGUI.DrawRect(drawelement_rect, sp_tmp_gra_col_c.colorValue);
                            drawelement_rect.Set(rect.width - 30.25f, rect.y + 40, 3.75f, 2);
                            EditorGUI.DrawRect(drawelement_rect, sp_tmp_gra_col_d.colorValue);
                            break;
                    }
                }
            }
            #endregion

            #region 类型标识物
            drawelement_rect.Set(rect.x + 35, rect.y + 8, 25, 20);
            if (ttype == xHud_TextType.Text)
            {
                Editor_XHud_GUI.Gui_Labelfield(drawelement_rect, "T", HudFilled.实体, HudColor.深空灰, Editor_XHud_GUI.GetColor(HudColor.警示黄), TextAnchor.MiddleCenter, Vector2.zero, 12, Font_Bold);
            }
            else
            {
                Editor_XHud_GUI.Gui_Labelfield(drawelement_rect, "M", HudFilled.实体, HudColor.深空灰, Editor_XHud_GUI.GetColor(HudColor.工业蓝), TextAnchor.MiddleCenter, Vector2.zero, 12, Font_Bold);
            }
            #endregion

            BlockGUI(sp_name.stringValue);

            warning_content = new GUIContent();
            if (ttype == xHud_TextType.Text)
            {
                if (sp_font.objectReferenceValue == null)
                {
                    warning_content.image = warnIcon;
                    warning_content.tooltip = "已丢失字体资源";
                }
            }
            else if (ttype == xHud_TextType.TmpText)
            {
                if (sp_tmpfont.objectReferenceValue == null && sp_font.objectReferenceValue == null)
                {
                    warning_content.image = warnIcon;
                    warning_content.tooltip = "已丢失Tmp字体资源";
                }
            }
            drawelement_rect.Set(rect.x + 15, rect.y + 35, 15, 15);
            Editor_XHud_GUI.Gui_Icon(drawelement_rect, warning_content);

            #region 查看详细信息
            drawelement_rect.Set(rect.width - 45, rect.y + 4, 22, 22);
            if (Editor_XHud_GUI.Gui_Button(drawelement_rect, btn_icon_details_released, btn_icon_details_press, true, "", "", Color.white))
            {
                XHud_LibraryArg_TextStyle info = new XHud_LibraryArg_TextStyle();
                info.CopyData(BaseScript.TextStyleLibrary[index]);
                Open_Hud_Library_TextStyle_Setter(info, index);
                return;
            }
            #endregion

            GUI.enabled = true;

            BlockGUI(sp_name.stringValue);

            drawelement_rect.Set(rect.x, rect.y, rect.width - 50, rect.height);
            // 检测鼠标点击事件
            Event e = Event.current;
            if (e.type == EventType.MouseDown && e.button == 1 && drawelement_rect.Contains(e.mousePosition))
            {
                // 创建右键菜单
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("E (修改样式)"), false, () =>
                {
                    XHud_LibraryArg_TextStyle info = new XHud_LibraryArg_TextStyle();
                    info.CopyData(BaseScript.TextStyleLibrary[index]);
                    Open_Hud_Library_TextStyle_Setter(info, index);
                    return;
                });
                menu.AddItem(new GUIContent("C (获取样式)"), false, () =>
                {
                    string json = JsonUtility.ToJson(BaseScript.TextStyleLibrary[index]);
                    Editor_XHud_GUI.EditorData_Set_With_String("XED_HudTextStyleLibrary_Get_TextStyle", json);

                    Editor_XHud_GUI.Open(XHud_DialogType.确认, "HudTextStyleLibrary 文字样式库消息", "文字样式复制", $"已将当前文字样式 {sp_name.stringValue} 存入 XHudEditorData (XED)！", "好的");
                });
                menu.AddItem(new GUIContent("V (粘贴样式)"), false, () =>
                {
                    XHud_LibraryArg_TextStyle info = JsonUtility.FromJson<XHud_LibraryArg_TextStyle>(Editor_XHud_GUI.EditorData_Get_With_String("XED_HudTextStyleLibrary_Get_TextStyle"));

                    // 使用 Undo.RecordObject 来记录对目标对象的修改
                    Undo.RecordObject(BaseScript, "Paste Style");

                    BaseScript.TextStyleLibrary[index].CopyData_Ignored_LibraryToggle(info);

                    Editor_XHud_GUI.Open(XHud_DialogType.确认, "HudTextStyleLibrary 文字样式库消息", "文字样式粘贴", $"已从 XHudEditorData (XED) 中获取文字样式并覆盖到当前文字样式项！", "好的");
                });

                menu.AddSeparator("");

                menu.AddItem(new GUIContent("S (克隆样式)"), false, () =>
                {
                    // 使用 Undo.RecordObject 来记录对目标对象的修改
                    Undo.RecordObject(sp_TextStyleLibrary.serializedObject.targetObject, "Clone Style");

                    sp_TextStyleLibrary.InsertArrayElementAtIndex(index);
                    sp_TextStyleLibrary.serializedObject.ApplyModifiedProperties();
                    Repaint();
                });
                // 显示右键菜单
                menu.ShowAsContext();

                e.Use(); // 标记事件已被处理，防止其他操作处理该事件
            }

            GUI.enabled = true;

            if (!sp_name.stringValue.Contains(sp_Highlight.stringValue))
            {
                drawelement_rect.Set(rect.x - 20, rect.y, rect.width + 20, rect.height);
                EditorGUI.DrawRect(drawelement_rect, blocked_col);
            }

            if (index == sp_LocationSelectedIndex.intValue)
            {
                drawelement_rect.Set(rect.width - 95, rect.y + 8, 50, 20);
                Editor_XHud_GUI.Gui_Labelfield(drawelement_rect, "已定位", HudFilled.无, HudColor.亮白, XHud_Dashboard.Theme_Primary, TextAnchor.MiddleCenter, Vector2.zero, 11);
            }
        }

        /// <summary>
        /// 绘制列表
        /// </summary>
        private void DrawTextStyleInfoList_Original()
        {
            // 绘制滚动视图
            scrollview_rect = GUILayoutUtility.GetRect(0, sp_visibleItemCount.intValue * sp_itemHeight.floatValue);
            sp_ColorInfoList_Original_Scroller.vector2Value = GUI.BeginScrollView(scrollview_rect, sp_ColorInfoList_Original_Scroller.vector2Value, new Rect(0, 0, scrollview_rect.width - 50, sp_TextStyleLibraryList.count * sp_itemHeight.floatValue), false, true);

            // 计算可视区域的起始和结束索引
            int startIndex = Mathf.FloorToInt(sp_ColorInfoList_Original_Scroller.vector2Value.y / sp_itemHeight.floatValue);
            int endIndex = Mathf.CeilToInt((sp_ColorInfoList_Original_Scroller.vector2Value.y + scrollview_rect.height) / sp_itemHeight.floatValue);

            // 只绘制可视区域内的元素
            for (int i = startIndex; i < endIndex && i < sp_TextStyleLibraryList.count; i++)
            {
                SerializedProperty prop = sp_TextStyleLibrary.GetArrayElementAtIndex(i);
                SerializedProperty sp_name = prop.FindPropertyRelative("Name");
                SerializedProperty sp_type = prop.FindPropertyRelative("Type");
                SerializedProperty sp_fontcolor = prop.FindPropertyRelative("FontColor");
                SerializedProperty sp_tmpfontcolor = prop.FindPropertyRelative("tmp_color");
                SerializedProperty sp_size = prop.FindPropertyRelative("Size");
                SerializedProperty sp_tmp_size = prop.FindPropertyRelative("tmp_size");
                SerializedProperty sp_font = prop.FindPropertyRelative("Font");
                SerializedProperty sp_Style = prop.FindPropertyRelative("Style");

                item_rect.Set(0, i * sp_itemHeight.floatValue, scrollview_rect.width, sp_itemHeight.floatValue);

                // 如果当前元素被选中，绘制选中效果
                if (sp_name.stringValue.Contains(sp_Highlight.stringValue))
                {
                    if (sp_SelectedIndex.intValue == i)
                    {
                        // 高亮标记表示选中
                        item_rect.Set(1, i * sp_itemHeight.floatValue + 15, 5, 5);
                        //itemmark_rect = new Rect(item_rect.x + 1, item_rect.y + 15, 5, 5);
                        EditorGUI.DrawRect(item_rect, XHud_Dashboard.Theme_Primary);
                        // 高亮背景表示选中
                        item_rect.Set(0, i * sp_itemHeight.floatValue, scrollview_rect.width + 20, sp_itemHeight.floatValue);
                        //itemmarkBg_rect = new Rect(item_rect.x, item_rect.y, item_rect.width + 20, item_rect.height);
                        EditorGUI.DrawRect(item_rect, SelectedBg);
                        sws = true;
                    }
                }

                item_rect.Set(0, i * sp_itemHeight.floatValue, scrollview_rect.width, sp_itemHeight.floatValue);

                sp_TextStyleLibraryList.drawElementCallback.Invoke(item_rect, i, i == sp_TextStyleLibraryList.index, true);

                // 检测鼠标是否在当前元素区域内
                if (item_rect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.MouseDown)
                    {
                        if (sp_name.stringValue.Contains(sp_Highlight.stringValue))
                        {
                            // 更新选中项
                            sp_SelectedIndex.intValue = i;

                            if ((xHud_TextType)sp_type.enumValueIndex == xHud_TextType.Text)
                            {
                                sp_PreviewFontColor.colorValue = sp_fontcolor.colorValue;
                                sp_PreviewTypeText.stringValue = "Hud Text";
                                sp_PreviewFontStyle.enumValueIndex = sp_Style.enumValueIndex;
                            }
                            else if ((xHud_TextType)sp_type.enumValueIndex == xHud_TextType.TmpText)
                            {
                                sp_PreviewFontColor.colorValue = sp_tmpfontcolor.colorValue;
                                sp_PreviewTypeText.stringValue = "Hud TmpText";
                                sp_PreviewFontStyle.enumValueIndex = (int)FontStyle.Normal;
                            }
                            sp_Preview_Font.objectReferenceValue = sp_font.objectReferenceValue;
                            sp_Preview_Font.serializedObject.ApplyModifiedProperties();

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
        private void TextStyleInfoList_Original_Remove(ReorderableList list)
        {
            if (list.count <= 0)
                return;
            sp_TextStyleLibrary.DeleteArrayElementAtIndex(list.index);
            sp_TextStyleLibrary.serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 列表增加
        /// </summary>
        /// <param name="list"></param>
        private void TextStyleInfoList_Original_Add(ReorderableList list)
        {
            string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "HudTextStyleLibrary 字体样式库消息", "创建样式类型", "您希望创建那种文字样式模版？", "Text", "TmpText", 1, true);
            SerializedProperty prop = null;
            if (list.count <= 0)
            {
                sp_TextStyleLibrary.InsertArrayElementAtIndex(0);
                prop = sp_TextStyleLibrary.GetArrayElementAtIndex(0);
            }
            else
            {
                sp_TextStyleLibrary.InsertArrayElementAtIndex(list.count);
                prop = sp_TextStyleLibrary.GetArrayElementAtIndex(list.count - 1);
            }

            SerializedProperty sp_Name = prop.FindPropertyRelative("Name");
            SerializedProperty sp_Des = prop.FindPropertyRelative("Description");
            SerializedProperty sp_Type = prop.FindPropertyRelative("Type");

            string path = AssetDatabase.GetAssetPath(BaseScript);
            string font_Path = path.Substring(0, path.IndexOf("XHud") + 4);


            if (res == "Text")
                sp_Type.enumValueIndex = (int)xHud_TextType.Text;
            else
                sp_Type.enumValueIndex = (int)xHud_TextType.TmpText;

            xHud_TextType type = (xHud_TextType)sp_Type.enumValueIndex;

            sp_Name.stringValue = $"NewTextStyle_{list.index}_{type.ToString()}";
            sp_Des.stringValue = $"这里是样式说明文字内容...";

            if (type == xHud_TextType.Text)
            {
                ///---------------Text
                SerializedProperty sp_Font = prop.FindPropertyRelative("Font");
                SerializedProperty sp_Font_AssetPath = prop.FindPropertyRelative("Font_AssetPath");
                SerializedProperty sp_FontSize = prop.FindPropertyRelative("Size");
                SerializedProperty sp_FontStyle = prop.FindPropertyRelative("Style");
                SerializedProperty sp_ContentAnchor = prop.FindPropertyRelative("ContentAnchor");
                SerializedProperty sp_LineHeight = prop.FindPropertyRelative("LineHeight");
                SerializedProperty sp_Overflow_Horizon = prop.FindPropertyRelative("Overflow_Horizon");
                SerializedProperty sp_Overflow_Vertical = prop.FindPropertyRelative("Overflow_Vertical");
                SerializedProperty sp_RichText = prop.FindPropertyRelative("RichText");
                SerializedProperty sp_BestFit = prop.FindPropertyRelative("BestFit");
                SerializedProperty sp_Fit_Min = prop.FindPropertyRelative("Fit_Min");
                SerializedProperty sp_Fit_Max = prop.FindPropertyRelative("Fit_Max");
                SerializedProperty sp_Color = prop.FindPropertyRelative("FontColor");

                sp_Font.objectReferenceValue = AssetDatabase.LoadAssetAtPath<Font>(Path.Combine(font_Path, "Fonts/Text/SevenStrikeFont_Normal.ttf"));
                sp_FontSize.floatValue = 50;
                sp_FontStyle.enumValueIndex = (int)FontStyle.Normal;
                sp_ContentAnchor.enumValueIndex = (int)ContentAnchor.中心;
                sp_LineHeight.floatValue = 1;
                sp_Overflow_Horizon.enumValueIndex = (int)HorizontalWrapMode.Overflow;
                sp_Overflow_Vertical.enumValueIndex = (int)VerticalWrapMode.Overflow;
                sp_RichText.boolValue = true;
                sp_BestFit.boolValue = false;
                sp_Fit_Min.intValue = 0;
                sp_Fit_Max.intValue = 100;
                sp_Color.colorValue = Color.white;
            }
            else
            {
                ///---------------TmpText
                SerializedProperty sp_tmp_rich = prop.FindPropertyRelative("tmp_rich");
                SerializedProperty sp_tmp_contentwrap = prop.FindPropertyRelative("tmp_contentwrap");
                SerializedProperty sp_tmp_overflow = prop.FindPropertyRelative("tmp_overflow");
                SerializedProperty sp_tmp_style = prop.FindPropertyRelative("tmp_style");
                SerializedProperty sp_tmp_anchor = prop.FindPropertyRelative("tmp_anchor");
                SerializedProperty sp_tmp_font = prop.FindPropertyRelative("tmp_font");
                SerializedProperty sp_tmp_fontasset_assetpath = prop.FindPropertyRelative("tmp_fontasset_assetpath");
                SerializedProperty sp_tmp_size = prop.FindPropertyRelative("tmp_size");
                SerializedProperty sp_tmp_space_character = prop.FindPropertyRelative("tmp_space_character");
                SerializedProperty sp_tmp_space_word = prop.FindPropertyRelative("tmp_space_word");
                SerializedProperty sp_tmp_space_lineheight = prop.FindPropertyRelative("tmp_space_lineheight");
                SerializedProperty sp_tmp_space_paragraph = prop.FindPropertyRelative("tmp_space_paragraph");
                SerializedProperty sp_tmp_WrappingRatios = prop.FindPropertyRelative("tmp_WrappingRatios");
                SerializedProperty sp_tmp_contentmargin = prop.FindPropertyRelative("tmp_contentmargin");
                SerializedProperty sp_tmp_color = prop.FindPropertyRelative("tmp_color");

                sp_tmp_rich.boolValue = true;
                sp_tmp_contentwrap.enumValueIndex = (int)TextWrappingModes.Normal;
                sp_tmp_overflow.enumValueIndex = (int)TextOverflowModes.Overflow;
                sp_tmp_style.enumValueIndex = (int)FontStyles.Normal;
                sp_tmp_anchor.enumValueIndex = (int)TmpContentAnchor.中心;
                sp_tmp_font.objectReferenceValue = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(Path.Combine(font_Path, "Fonts/Tmp/SevenStrikeFont_Normal SDF.asset"));
                sp_tmp_size.floatValue = 50;
                sp_tmp_space_character.floatValue = 0;
                sp_tmp_space_word.floatValue = 0;
                sp_tmp_space_lineheight.floatValue = 0;
                sp_tmp_space_paragraph.floatValue = 0;
                sp_tmp_WrappingRatios.floatValue = 0;
                sp_tmp_contentmargin.vector4Value = Vector4.zero;
                sp_tmp_color.colorValue = Color.white;
            }

            prop.serializedObject.ApplyModifiedProperties();
        }
        #endregion

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            Editor_XHud_GUI.Gui_Layout_Banner(HudFilled.实体, HudColor.亮白, "Hud - 字体样式库", Color.black);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_XHud_GUI.Gui_Layout_Property_Field("字体样式库名称", sp_LibraryName);
            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Horizontal_End();

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_XHud_GUI.Gui_Layout_Property_Field("预览文字内容", sp_PreviewContent);
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
                    BaseScript.TextStyleLibrary_Location_Find(sp_Find.stringValue);
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
            Editor_XHud_GUI.Gui_Layout_Seperator(1, XHud_Dashboard.Theme_SeperateLine);
            Editor_XHud_GUI.Gui_Layout_Space(5);

            if (string.IsNullOrEmpty(sp_Highlight.stringValue))
            {
                if (!Application.isPlaying)
                {
                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_FlexSpace();
                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "导入字体样式模版", import_r, import_p, 4))
                    {
                        ImportTextStyles();
                    }
                    Editor_XHud_GUI.Gui_Layout_Space(35);
                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "导出字体样式模版", export_r, export_p, 4))
                    {
                        ExportTextStyles();
                    }
                    Editor_XHud_GUI.Gui_Layout_Space(35);
                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "清空所有文字样式模版", clear_r, clear_p, 4))
                    {
                        string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "HudTextStyleLibrary 字体样式库消息", "清空所有字体样式", "是否清空所有字体样式项？请注意！如果您的场景中或是预制体中的文字组件用到了该字体样式库中的字体样式，清空后会导致组件的字体样式信息丢失，请谨慎操作！", "清空", "暂不", 1);
                        if (res == "暂不")
                            return;

                        sp_TextStyleLibrary.ClearArray();
                        sp_TextStyleLibrary.serializedObject.ApplyModifiedProperties();
                    }
                    Editor_XHud_GUI.Gui_Layout_Space(35);
                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "添加项", create_r, create_p, 4))
                    {
                        TextStyleInfoList_Original_Add(sp_TextStyleLibraryList);
                    }
                    Editor_XHud_GUI.Gui_Layout_Space(35);
                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "删除项", delete_r, delete_p, 4))
                    {
                        TextStyleInfoList_Original_Remove(sp_TextStyleLibraryList);
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

            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "样式列表", Color.white);
            Editor_XHud_GUI.Gui_Layout_Space(5);

            DrawTextStyleInfoList_Original();

            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            serializedObject.ApplyModifiedProperties();
        }

        #region 辅助
        /// <summary>
        /// 导出文字样式表
        /// </summary>
        private void ExportTextStyles()
        {
            string path = EditorUtility.SaveFilePanel("", Application.dataPath, "TextStyleLibrary", "json");
            if (string.IsNullOrEmpty(path))
                return;

            string ext_path = Path.GetDirectoryName(path);
            string folder_path = Directory.CreateDirectory(ext_path + "/TextStyleLibrary").FullName;
            string dynamic_folder_path = Directory.CreateDirectory(ext_path + "/TextStyleLibrary/Dynamic").FullName;
            string tmpasset_folder_path = Directory.CreateDirectory(ext_path + "/TextStyleLibrary/TmpAsset").FullName;
            string filename = Path.GetFileName(path);
            string save_path = Path.Combine(folder_path, filename);

            ExportTextStyles styles = new ExportTextStyles();
            styles.TextStyleLibrary = BaseScript.TextStyleLibrary;

            for (int i = 0; i < styles.TextStyleLibrary.Count; i++)
            {
                if (styles.TextStyleLibrary[i].Type == xHud_TextType.Text)
                {
                    if (styles.TextStyleLibrary[i].Font != null)
                    {
                        string asset_path = AssetDatabase.GetAssetPath(styles.TextStyleLibrary[i].Font);
                        string ab_name = Path.GetFileName(asset_path);
                        string font_path = Path.Combine(dynamic_folder_path, ab_name);
                        string font_source_path = Application.dataPath + asset_path.Replace("Assets", "");
                        styles.TextStyleLibrary[i].Font_AssetPath = font_path;
                        File.Copy(font_source_path, font_path, true);
                    }
                }
                else
                {
                    if (styles.TextStyleLibrary[i].tmp_font != null)
                    {
                        string asset_path = AssetDatabase.GetAssetPath(styles.TextStyleLibrary[i].tmp_font);
                        string ab_name = Path.GetFileName(asset_path);
                        string font_path = Path.Combine(tmpasset_folder_path, ab_name);
                        string font_source_path = Application.dataPath + asset_path.Replace("Assets", "");
                        styles.TextStyleLibrary[i].tmp_fontasset_assetpath = font_path;
                        File.Copy(font_source_path, font_path, true);
                    }
                }
            }

            string json = JsonUtility.ToJson(styles);

            File.WriteAllText(save_path, json);
        }

        /// <summary>
        /// 导入文字样式表
        /// </summary>
        private void ImportTextStyles()
        {
            string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "HudTextStyleLibrary 字体样式库消息", "读取字体样式数据", "根据您的需要选择导入字体样式数据的方式，如果是追加则会在当前字体样式库的基础上后续叠加导入的字体样式项，如果是替换则会完全替换当前字体样式库的所有字体样式项！", "追加", "替换", "暂不", 2);
            if (res == "暂不")
            {
                return;
            }
            string path = EditorUtility.OpenFilePanel("请选择要导入的文字样式库Json文件", Application.dataPath, "json");

            if (string.IsNullOrEmpty(path))
                return;

            string filename = Path.GetFileNameWithoutExtension(path);

            ExportTextStyles styles = new ExportTextStyles();

            switch (res)
            {
                case "追加":
                    if (string.IsNullOrEmpty(path))
                    {
                        return;
                    }
                    else
                    {
                        string json = File.ReadAllText(path);
                        styles = JsonUtility.FromJson<ExportTextStyles>(json);

                        for (int i = 0; i < styles.TextStyleLibrary.Count; i++)
                        {
                            BaseScript.TextStyleLibrary.Add(styles.TextStyleLibrary[i]);
                        }
                    }
                    break;
                case "替换":
                    if (!string.IsNullOrEmpty(path))
                    {
                        string json = File.ReadAllText(path);
                        styles = JsonUtility.FromJson<ExportTextStyles>(json);
                        BaseScript.TextStyleLibrary = styles.TextStyleLibrary;
                    }
                    break;
            }

            string res_tp = Editor_XHud_GUI.Open(XHud_DialogType.警告, "HudTextStyleLibrary 字体样式库消息", "导入字体", "是否需要导入文字样式附带的Dynamic字体文件或者是Tmp字体资源？", "不需要", "导入", 2);
            if (res_tp == "不需要")
            {
                return;
            }

            string importfont_save_path = EditorUtility.SaveFolderPanel("字体文件或资源的保存路径", Application.dataPath, "");
            if (string.IsNullOrEmpty(importfont_save_path))
            {
                Repaint();

                AssetDatabase.Refresh();
                return;
            }

            string importfont_save_folder = importfont_save_path.Substring(importfont_save_path.IndexOf("Assets"));

            for (int i = 0; i < styles.TextStyleLibrary.Count; i++)
            {
                if (styles.TextStyleLibrary[i].Type == xHud_TextType.Text)
                {
                    string font_absolute_path = styles.TextStyleLibrary[i].Font_AssetPath;
                    string font_absolute_filename = Path.GetFileName(styles.TextStyleLibrary[i].Font_AssetPath);
                    string imp_font_filepath = Application.dataPath.Replace("Assets", "") + importfont_save_folder + "/" + font_absolute_filename;
                    if (File.Exists(font_absolute_path))
                        File.Copy(font_absolute_path, imp_font_filepath, true);
                }
                else
                {
                    string fontasset_absolute_path = styles.TextStyleLibrary[i].tmp_fontasset_assetpath;
                    string fontasset_absolute_filename = Path.GetFileName(styles.TextStyleLibrary[i].tmp_fontasset_assetpath);
                    string imp_fontasset_filepath = Application.dataPath.Replace("Assets", "") + importfont_save_folder + "/" + fontasset_absolute_filename;
                    if (File.Exists(fontasset_absolute_path))
                        File.Copy(fontasset_absolute_path, imp_fontasset_filepath, true);
                }
            }

            Repaint();
            AssetDatabase.Refresh();
        }
        #endregion

        #region 打开字体样式修改器
        /// <summary>
        /// 字体库修改器
        /// </summary>
        public void Open_Hud_Library_TextStyle_Setter(XHud_LibraryArg_TextStyle info, int index)
        {
            Editor_XHud_LibrarySetTool_TextStyle window = EditorWindow.GetWindow<Editor_XHud_LibrarySetTool_TextStyle>(true);

            window.titleContent = new GUIContent("XHud 文字样式库修改器");
            Editor_XHud_GUI.CenterEditorWindow(new Vector2Int(850, 640), window);

            window.SetStyle(info);
            window.ModifiedIndex = index;
            window.SetOriginStyle(info);
            window.OriginLibName = info.Name;
            window.SetInfo(info.Name, info.Description);
            window.SetLibrarySetterMode(LibrarySetterMode.修改库源参数);
            window.SetButtonText("更新", "取消");
            window.SetTitle("XHud 文字样式库修改器");
            window.Set_Target_Hud_TextStyleLibrary(BaseScript);
            //window.ShowModal();
            window.Show();
        }
        #endregion

        #region 预览
        public override bool HasPreviewGUI()
        {
            return sws;
        }

        public override GUIContent GetPreviewTitle()
        {
            return new GUIContent("HudTextStyle");
        }

        public override void OnPreviewSettings()
        {
            base.OnPreviewSettings();
        }

        public override void OnPreviewGUI(Rect rect, GUIStyle background)
        {
            //util_XHUDGUI.Gui_Icon(new Rect(rect.x, rect.y, rect.height * 0.75f, rect.height * 0.75f), util_XHUDGUI.GetIcon("Icon_Assets_Font_Iso"));
        }

        public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
        {
            Font font = (Font)sp_Preview_Font.objectReferenceValue;

            Editor_XHud_GUI.Gui_Labelfield(r, sp_PreviewContent.stringValue, HudFilled.无, HudColor.无, sp_PreviewFontColor.colorValue, TextAnchor.MiddleCenter, Vector2.zero, 20, true, TextClipping.Ellipsis, true, font);

            Editor_XHud_GUI.Gui_Labelfield_Thin(new Rect((r.width / 2) - 50, r.height - 10, 100, 25), sp_PreviewTypeText.stringValue, HudFilled.实体, HudColor.深空灰, Color.white, TextAnchor.MiddleCenter, Vector2.zero, 12);
        }
        #endregion
    }
}