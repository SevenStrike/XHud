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
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.Utilitys;
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    [System.Serializable]
    public class CopyHudColor
    {
        public string Type;
        public string Color;
        public string Name;
        public int Index;
    }

    [System.Serializable]
    public class ExportItem
    {
        public string color;
        public string mark;
        public string name;
        public string description;
        public string hex;
    }

    [System.Serializable]
    public class ExportColorLib
    {
        public List<ExportItem> ColorsInfo;
    }

    [CustomEditor(typeof(XHud_Library_Colors))]
    public class Editor_XHud_ColorsLibrary : Editor
    {
        #region 组件 / 列表
        private XHud_Library_Colors BaseScript;
        /// <summary>
        /// 颜色列表
        /// </summary>
        public ReorderableList ColorInfoList_Original;
        #endregion

        #region 序列化属性
        private SerializedProperty sp_itemHeight, sp_visibleItemCount, sp_ColorInfoList_Original_Scroller, sp_LocationSelectedIndex, sp_SelectedIndex, sp_ColorLibrary_Original, sp_Find, sp_Highlight, sp_LibraryName;
        #endregion

        int MultipleSize = 16;

        #region 预览
        /// <summary>
        /// 选中的颜色
        /// </summary>
        private Color SelectedColor;
        /// <summary>
        /// 颜色信息
        /// </summary>
        private string ColorInfo;
        /// <summary>
        /// 预览图片
        /// </summary>
        private Texture2D[] ReferImages;
        /// <summary>
        /// 预览标题文字
        /// </summary>
        public string PreviewHeader = "HudColorPreview";
        #endregion

        #region 图标
        private Texture2D PalletIcon, import_p, import_r, export_p, export_r, clear_p, clear_r, create_p, create_r, delete_p, delete_r, random_p, random_r;
        #endregion

        /// <summary>
        /// 屏蔽颜色
        /// </summary>
        private Color blocked_col;
        /// <summary>
        /// 屏蔽颜色透明度
        /// </summary>
        private float blocked_alp = 0.45f;
        private Rect PreviewGUI_rect;

        private void OnEnable()
        {
            BaseScript = (XHud_Library_Colors)target;

            #region 获取序列化属性
            sp_ColorLibrary_Original = serializedObject.FindProperty("ColorLibrary");
            sp_LibraryName = serializedObject.FindProperty("LibraryName");
            sp_Highlight = serializedObject.FindProperty("Highlight");
            sp_SelectedIndex = serializedObject.FindProperty("SelectedIndex");
            sp_LocationSelectedIndex = serializedObject.FindProperty("LocationSelectedIndex");
            sp_itemHeight = serializedObject.FindProperty("itemHeight");
            sp_visibleItemCount = serializedObject.FindProperty("visibleItemCount");
            sp_ColorInfoList_Original_Scroller = serializedObject.FindProperty("ColorInfoList_Original_Scroller");
            sp_Find = serializedObject.FindProperty("Find");
            #endregion

            SelectedColor = Color.white;

            blocked_col = new Color(0, 0, 0, blocked_alp);

            #region 获取图标
            import_p = Editor_XHud_GUI.GetIcon("Icons_XHud_ColorsLibrary/import_p");
            import_r = Editor_XHud_GUI.GetIcon("Icons_XHud_ColorsLibrary/import_r");
            export_p = Editor_XHud_GUI.GetIcon("Icons_XHud_ColorsLibrary/export_p");
            export_r = Editor_XHud_GUI.GetIcon("Icons_XHud_ColorsLibrary/export_r");
            clear_p = Editor_XHud_GUI.GetIcon("Icons_XHud_ColorsLibrary/clear_p");
            clear_r = Editor_XHud_GUI.GetIcon("Icons_XHud_ColorsLibrary/clear_r");
            create_p = Editor_XHud_GUI.GetIcon("Icons_XHud_ColorsLibrary/create_p");
            create_r = Editor_XHud_GUI.GetIcon("Icons_XHud_ColorsLibrary/create_r");
            delete_p = Editor_XHud_GUI.GetIcon("Icons_XHud_ColorsLibrary/delete_p");
            delete_r = Editor_XHud_GUI.GetIcon("Icons_XHud_ColorsLibrary/delete_r");
            random_p = Editor_XHud_GUI.GetIcon("Icons_XHud_ColorsLibrary/random_p");
            random_r = Editor_XHud_GUI.GetIcon("Icons_XHud_ColorsLibrary/random_r");
            #endregion

            //设定列表项高度值
            sp_itemHeight.floatValue = 60;
            sp_itemHeight.serializedObject.ApplyModifiedProperties();

            sp_visibleItemCount.intValue = 10;
            sp_visibleItemCount.serializedObject.ApplyModifiedProperties();

            PalletIcon = Editor_XHud_GUI.GetIcon("Icons_Hud_ColorsLibrary/palletmark");

            #region ReorderableList - ColorInfoList
            ColorInfoList_Original = new ReorderableList(serializedObject, sp_ColorLibrary_Original, true, true, true, true);
            ColorInfoList_Original.drawElementCallback = ColorInfoList_Original_DrawElementCallback;
            #endregion            
        }

        private void OnDisable()
        {
            if (target != null)
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
            }
        }

        #region ColorInfoList_Original      

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
        private void ColorInfoList_Original_DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty prop = sp_ColorLibrary_Original.GetArrayElementAtIndex(index);

            SerializedProperty sp_name = prop.FindPropertyRelative("Name");
            SerializedProperty sp_color = prop.FindPropertyRelative("Color");
            SerializedProperty sp_des = prop.FindPropertyRelative("Description");

            drawelement_rect.Set(rect.x + 15, rect.y + 3, 30, 20);
            Editor_XHud_GUI.Gui_Labelfield(drawelement_rect, index.ToString("D2"), HudFilled.无, HudColor.无, Color.gray, TextAnchor.MiddleLeft, Vector2.zero, 11);

            GUI.backgroundColor = sp_color.colorValue;
            drawelement_rect.Set(rect.x + 15, rect.y + 25, 12, 12);
            Editor_XHud_GUI.Gui_Icon(drawelement_rect, PalletIcon);
            GUI.backgroundColor = Color.white;

            BlockGUI(sp_name.stringValue);

            drawelement_rect.Set(rect.x + 42, rect.y + 3, rect.width - 160, 20);
            Editor_XHud_GUI.Gui_Labelfield(drawelement_rect, sp_name.stringValue, HudFilled.无, HudColor.深空灰, sp_color.colorValue, TextAnchor.MiddleLeft, Vector2.zero, 13, true, TextClipping.Ellipsis, true);

            drawelement_rect.Set(rect.x + 42, rect.y + 25, rect.width - 200, 20);
            Editor_XHud_GUI.Gui_Labelfield_Thin_WithClipping(drawelement_rect, sp_des.stringValue, HudFilled.无, HudColor.无, Color.white * 0.65f, TextAnchor.MiddleLeft, Vector2.zero, 11, true, true, true, TextClipping.Ellipsis);

            BlockGUI(sp_name.stringValue);

            drawelement_rect.Set(rect.width - 80, rect.y + 5, 60, 20);
            sp_color.colorValue = Editor_XHud_GUI.Gui_ColorField(drawelement_rect, sp_color.colorValue);
            sp_color.serializedObject.ApplyModifiedProperties();

            GUI.enabled = true;

            BlockGUI(sp_name.stringValue);

            // 检测鼠标点击事件
            Event e = Event.current;
            drawelement_rect.Set(rect.x, rect.y, rect.width - 50, rect.height);
            if (e.type == EventType.MouseDown && e.button == 1 && drawelement_rect.Contains(e.mousePosition))
            {
                // 更新选中项
                sp_SelectedIndex.intValue = index;

                SelectedColor = sp_color.colorValue;

                ColorInfo = "R: " + SelectedColor.r.ToString("F2") + " | G: " + SelectedColor.g.ToString("F2") + " | B: " + SelectedColor.b.ToString("F2");

                PreviewHeader = sp_name.stringValue;

                // 创建右键菜单
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("S (获取色卡信息)"), false, () =>
                {
                    CopyHudColor chc = new CopyHudColor();
                    chc.Color = sp_color.colorValue.r + "," + sp_color.colorValue.g + "," + sp_color.colorValue.b + "," + sp_color.colorValue.a;
                    chc.Index = index;
                    chc.Name = sp_name.stringValue;
                    chc.Type = "HudCopyColor";
                    string json = JsonUtility.ToJson(chc);
                    Debug.Log(json);
                    Editor_XHud_GUI.EditorData_Set_With_String("XED_ColorLibrary_Get_ColorInfo", json);

                    string hexcol = XHud_Utilitys.Color_To_HexColor(sp_color.colorValue, true);

                    Editor_XHud_GUI.Open(XHud_DialogType.确认, "XHud - 色卡库消息", "获取色卡信息", $"<color={hexcol}>{sp_name.stringValue} </color>色卡信息已就绪！请选择需要识别的图元配色器后右键菜单点击识别色卡即可应用该色卡颜色！", "明白");
                });
                menu.AddItem(new GUIContent("E (修改色卡信息)"), false, () =>
                {
                    xHud_LibraryArg_Color info = new xHud_LibraryArg_Color();
                    info.Name = sp_name.stringValue;
                    info.Description = sp_des.stringValue;
                    info.Color = sp_color.colorValue;
                    OpenLibrarySetTool(info, sp_name.stringValue, index);
                });
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("C (克隆色卡项)"), false, () =>
                {
                    sp_ColorLibrary_Original.InsertArrayElementAtIndex(index);
                    sp_ColorLibrary_Original.serializedObject.ApplyModifiedProperties();
                    Repaint();
                });
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("D (拷贝色卡的 Hex 颜色)"), false, () =>
                {
                    GUIUtility.systemCopyBuffer = XHud_Utilitys.Color_To_HexColor(sp_color.colorValue, true).Split(new char[1] { '#' })[1];

                    string hexcol = XHud_Utilitys.Color_To_HexColor(sp_color.colorValue, true);

                    Editor_XHud_GUI.Open(XHud_DialogType.确认, "XHud - 色卡库消息", "获取色卡信息", $"<color={hexcol}>{sp_name.stringValue} </color>Hex 色卡信息 {hexcol} 已拷贝到系统剪贴板！", "明白");
                });
                menu.AddItem(new GUIContent("A (拷贝色卡的 R G B 颜色)"), false, () =>
                {
                    string hexcol = XHud_Utilitys.Color_To_HexColor(sp_color.colorValue, true);

                    string mode = Editor_XHud_GUI.Open(XHud_DialogType.帮助, "XHud - 色卡库消息", "获取色卡信息", $"请选择您要获取色卡 <color={hexcol}>{sp_name.stringValue} </color>的 R G B 信息的模式！", "色卡值", "代码块");
                    if (mode == "代码块")
                    {
                        string color = $"Color col = new Color({sp_color.colorValue.r}f, {sp_color.colorValue.g}f, {sp_color.colorValue.b}f);";
                        GUIUtility.systemCopyBuffer = color;
                        Editor_XHud_GUI.Open(XHud_DialogType.确认, "XHud - 色卡库消息", "获取色卡信息", $"<color={hexcol}>{sp_name.stringValue} </color>R G B 色卡代码块 {color} 已拷贝到系统剪贴板！", "明白");
                    }
                    else
                    {
                        string rgb = $"{sp_color.colorValue.r},{sp_color.colorValue.g},{sp_color.colorValue.b}";
                        GUIUtility.systemCopyBuffer = rgb;
                        Editor_XHud_GUI.Open(XHud_DialogType.确认, "XHud - 色卡库消息", "获取色卡信息", $"<color={hexcol}>{sp_name.stringValue} </color>R G B 色卡信息 {rgb} 已拷贝到系统剪贴板！", "明白");
                    }
                });
                menu.AddItem(new GUIContent("X (拷贝色卡的 R G B A 颜色)"), false, () =>
                {
                    string hexcol = XHud_Utilitys.Color_To_HexColor(sp_color.colorValue, true);

                    string mode = Editor_XHud_GUI.Open(XHud_DialogType.帮助, "XHud - 色卡库消息", "获取色卡信息", $"请选择您要获取色卡 <color={hexcol}>{sp_name.stringValue} </color>的 R G B A 信息的模式！", "色卡值", "代码块");
                    if (mode == "代码块")
                    {
                        string color = $"Color col = new Color({sp_color.colorValue.r}f, {sp_color.colorValue.g}f, {sp_color.colorValue.b}f, {sp_color.colorValue.a}f);";
                        GUIUtility.systemCopyBuffer = color;
                        Editor_XHud_GUI.Open(XHud_DialogType.确认, "XHud - 色卡库消息", "获取色卡信息", $"<color={hexcol}>{sp_name.stringValue} </color>R G B A 色卡代码块 {color} 已拷贝到系统剪贴板！", "明白");
                    }
                    else
                    {
                        string rgba = $"{sp_color.colorValue.r},{sp_color.colorValue.g},{sp_color.colorValue.b},{sp_color.colorValue.a}";
                        GUIUtility.systemCopyBuffer = rgba;
                        Editor_XHud_GUI.Open(XHud_DialogType.确认, "XHud - 色卡库消息", "获取色卡信息", $"<color={hexcol}>{sp_name.stringValue} </color>R G B A 色卡信息 {rgba} 已拷贝到系统剪贴板！", "明白");
                    }
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
                drawelement_rect.Set(rect.width - 145, rect.y + 12, 50, 20);
                Editor_XHud_GUI.Gui_Labelfield(drawelement_rect, "已定位", HudFilled.无, HudColor.亮白, XHud_Dashboard.Theme_Primary, TextAnchor.MiddleCenter, Vector2.zero, 11);
            }
        }

        /// <summary>
        /// 绘制列表
        /// </summary>
        private void DrawColorInfoList_Original()
        {
            // 绘制滚动视图
            scrollview_rect = GUILayoutUtility.GetRect(0, sp_visibleItemCount.intValue * sp_itemHeight.floatValue);
            sp_ColorInfoList_Original_Scroller.vector2Value = GUI.BeginScrollView(scrollview_rect, sp_ColorInfoList_Original_Scroller.vector2Value, new Rect(0, 0, scrollview_rect.width - 50, ColorInfoList_Original.count * sp_itemHeight.floatValue), false, true);

            // 计算可视区域的起始和结束索引
            int startIndex = Mathf.FloorToInt(sp_ColorInfoList_Original_Scroller.vector2Value.y / sp_itemHeight.floatValue);
            int endIndex = Mathf.CeilToInt((sp_ColorInfoList_Original_Scroller.vector2Value.y + scrollview_rect.height) / sp_itemHeight.floatValue);

            // 只绘制可视区域内的元素
            for (int i = startIndex; i < endIndex && i < ColorInfoList_Original.count; i++)
            {
                SerializedProperty prop = sp_ColorLibrary_Original.GetArrayElementAtIndex(i);
                SerializedProperty sp_color = prop.FindPropertyRelative("Color");
                SerializedProperty sp_name = prop.FindPropertyRelative("Name");

                // 如果当前元素被选中，绘制选中效果
                if (sp_name.stringValue.Contains(sp_Highlight.stringValue))
                {
                    if (sp_SelectedIndex.intValue == i)
                    {
                        // 高亮标记表示选中
                        item_rect.Set(1, i * sp_itemHeight.floatValue + 12, 5, 5);
                        EditorGUI.DrawRect(item_rect, XHud_Dashboard.Theme_Primary);
                        // 高亮背景表示选中
                        item_rect.Set(1, i * sp_itemHeight.floatValue, scrollview_rect.width + 20, sp_itemHeight.floatValue);
                        EditorGUI.DrawRect(item_rect, SelectedBg);
                    }
                }

                item_rect.Set(0, i * sp_itemHeight.floatValue, scrollview_rect.width, sp_itemHeight.floatValue);

                ColorInfoList_Original.drawElementCallback.Invoke(item_rect, i, i == ColorInfoList_Original.index, true);

                // 检测鼠标是否在当前元素区域内
                if (item_rect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.MouseDown)
                    {
                        if (sp_name.stringValue.Contains(sp_Highlight.stringValue))
                        {
                            // 更新选中项
                            sp_SelectedIndex.intValue = i;

                            SelectedColor = sp_color.colorValue;

                            ColorInfo = "R: " + SelectedColor.r.ToString("F2") + " | G: " + SelectedColor.g.ToString("F2") + " | B: " + SelectedColor.b.ToString("F2");

                            PreviewHeader = sp_name.stringValue;

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
        private void ColorInfoList_Original_Remove(ReorderableList list)
        {
            if (list.count <= 0)
                return;
            sp_ColorLibrary_Original.DeleteArrayElementAtIndex(list.index);
        }

        /// <summary>
        /// 列表增加
        /// </summary>
        /// <param name="list"></param>
        private void ColorInfoList_Original_Add(ReorderableList list)
        {
            if (list.count <= 0)
            {
                if (BaseScript.ColorLibrary == null)
                    BaseScript.ColorLibrary = new List<xHud_LibraryArg_Color>();
                BaseScript.ColorLibrary.Add(new XHud.xHud_LibraryArg_Color("颜色" + BaseScript.ColorLibrary.Count, Color.white, "颜色的说明文字内容"));
            }
            else
            {
                sp_ColorLibrary_Original.InsertArrayElementAtIndex(list.index);
                SerializedProperty prop = sp_ColorLibrary_Original.GetArrayElementAtIndex(list.index);
                SerializedProperty prop_name = prop.FindPropertyRelative("Name");
                SerializedProperty prop_color = prop.FindPropertyRelative("Color");
                SerializedProperty prop_des = prop.FindPropertyRelative("Description");

                prop_name.stringValue = "颜色" + sp_ColorLibrary_Original.arraySize;
                prop_color.colorValue = Color.white;
                prop_des.stringValue = "颜色文字说明";

                prop.serializedObject.ApplyModifiedProperties();
            }
        }
        #endregion

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            Editor_XHud_GUI.Gui_Layout_Banner(HudFilled.实体, HudColor.亮白, "XHud - 色卡库", Color.black);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_XHud_GUI.Gui_Layout_Property_Field("色卡库名称", sp_LibraryName);
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
                    BaseScript.ColorsLibrary_Location_Find(sp_Find.stringValue);
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
            Editor_XHud_GUI.Gui_Layout_Space(10);

            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_XHud_GUI.Gui_Layout_FlexSpace();

            if (string.IsNullOrEmpty(sp_Highlight.stringValue))
            {
                if (!Application.isPlaying)
                {
                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "读取色卡", import_r, import_p, 4))
                    {
                        LoadPallets();
                    }
                    Editor_XHud_GUI.Gui_Layout_Space(35);
                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "导出色卡", export_r, export_p, 4))
                    {
                        ExportPallets();
                    }
                    Editor_XHud_GUI.Gui_Layout_Space(35);
                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "清空色卡", clear_r, clear_p, 4))
                    {
                        string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 色卡库消息", "清空所有色卡", "是否清空所有色卡项？请注意！如果您的场景中或是预制体中的图元配色器用到了该色卡库中的色卡，清空后会导致图元配色器的色卡信息丢失，请谨慎操作！", "清空", "暂不", 0);
                        if (res == "暂不")
                            return;
                        sp_ColorLibrary_Original.ClearArray();
                    }
                    Editor_XHud_GUI.Gui_Layout_Space(35);
                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "添加色卡项", create_r, create_p, 4))
                    {
                        ColorInfoList_Original_Add(ColorInfoList_Original);
                    }
                    Editor_XHud_GUI.Gui_Layout_Space(35);
                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "删除色卡项", delete_r, delete_p, 4))
                    {
                        ColorInfoList_Original_Remove(ColorInfoList_Original);
                    }
                    Editor_XHud_GUI.Gui_Layout_Space(35);
                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "生成常用色卡颜色", random_r, random_p, 4))
                    {
                        string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 色卡库消息", "生成随机色卡", "是否要为色卡库随机生成一套颜色？生成后会覆盖当前的所有色卡项，请谨慎操作！", "暂不", "基础色", "高级灰", "通用色", 0);
                        if (res == "暂不")
                            return;

                        string res_m = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 色卡库消息", "生成随机色卡", "请选择生成模式！如果选择替换会覆盖当前的所有色卡项，请谨慎操作！", "追加", "替换", 1);

                        switch (res)
                        {
                            case "基础色":
                                BaseScript.ColorsLibrary_CreateColors_BaseColor(res_m);
                                break;
                            case "高级灰":
                                BaseScript.ColorsLibrary_CreateColors_AdvancedGrayColor(res_m);
                                break;
                            case "通用色":
                                BaseScript.ColorsLibrary_CreateColors_General(res_m);
                                break;
                        }
                    }
                }
            }
            else
            {
                Editor_XHud_GUI.Gui_Layout_LabelfieldThin("当前为过滤筛选状态", HudFilled.无, HudColor.无, XHud_Dashboard.Theme_Primary, TextAnchor.MiddleCenter, new Vector2(0, 0), 12);
            }
            Editor_XHud_GUI.Gui_Layout_FlexSpace();
            Editor_XHud_GUI.Gui_Layout_Horizontal_End();

            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Seperator(1, XHud_Dashboard.Theme_SeperateLine);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "色卡列表", Color.white);
            Editor_XHud_GUI.Gui_Layout_Space(5);

            DrawColorInfoList_Original();

            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();

            serializedObject.ApplyModifiedProperties();
        }

        #region 辅助
        /// <summary>
        /// 色卡库修改器
        /// </summary>
        public void OpenLibrarySetTool(xHud_LibraryArg_Color info, string originname, int index)
        {
            Editor_XHud_LibrarySetTool_Color window = EditorWindow.GetWindow<Editor_XHud_LibrarySetTool_Color>(true);

            window.titleContent = new GUIContent("XHud 色卡修改器");
            Editor_XHud_GUI.CenterEditorWindow(new Vector2Int(620, 530), window);

            window.SetTitle("XHud 色卡修改器");
            window.SetInfo(info.Name, info.Description, info.Color);
            window.ModifiedIndex = index;
            window.SetLibrarySetterMode(LibrarySetterMode.修改库源参数);
            window.Set_OriginColorName(originname);
            window.SetButtonText("更新", "取消");
            window.SetTarget_Hud_ColorsLibrary(BaseScript);
            //window.ShowModal();
            window.Show();
        }

        /// <summary>
        /// 创建色板
        /// </summary>
        /// <param name="info"></param>
        private void ExportPallets()
        {
            if (BaseScript.ColorLibrary != null && BaseScript.ColorLibrary.Count > 0)
            {
                Texture2D Pallet = new Texture2D(MultipleSize * BaseScript.ColorLibrary.Count, MultipleSize, TextureFormat.ARGB32, true, true);
                for (int i = 0; i < BaseScript.ColorLibrary.Count; i++)
                {
                    for (int x = MultipleSize * i; x < MultipleSize * (i + 1); x++)
                    {
                        for (int y = 0; y < MultipleSize; y++)
                        {
                            Pallet.SetPixel(x, y, BaseScript.ColorLibrary[i].Color);
                        }
                    }
                }
                Pallet.Apply();

                byte[] PalletData = Pallet.EncodeToPNG();
                string path = EditorUtility.SaveFilePanel("", Application.dataPath, "Pallet", "png");

                if (string.IsNullOrEmpty(path))
                    return;

                if (!string.IsNullOrEmpty(path))
                {
                    FileStream Fs = File.Open(path, FileMode.OpenOrCreate);
                    Fs.Write(PalletData, 0, PalletData.Length);
                    Fs.Close();

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }

                ExportColorLib lib = new ExportColorLib();
                lib.ColorsInfo = new List<ExportItem>();

                for (int i = 0; i < BaseScript.ColorLibrary.Count; i++)
                {
                    ExportItem item = new ExportItem();
                    item.color = XHud_Utilitys.Color_To_String(BaseScript.ColorLibrary[i].Color);
                    item.hex = XHud_Utilitys.Color_To_HexColor(BaseScript.ColorLibrary[i].Color);
                    item.name = BaseScript.ColorLibrary[i].Name;
                    item.description = BaseScript.ColorLibrary[i].Description;

                    lib.ColorsInfo.Add(item);
                }

                string json = JsonUtility.ToJson(lib, true);

                string pathjson = Path.GetDirectoryName(path);
                string filename = Path.GetFileNameWithoutExtension(path);
                File.WriteAllText(pathjson + "/" + filename + ".json", json);
            }
        }

        /// <summary>
        /// 读取色板
        /// </summary>
        private void LoadPallets()
        {
            string path = "";

            string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 色卡库消息", "读取色卡数据", "根据您的需要选择导入色卡数据的方式，如果是追加则会在当前色卡库的基础上后续叠加导入的色卡项，如果是替换则会完全替换当前色卡库的所有色卡项！", "追加", "替换", "暂不", 2);
            if (res == "暂不")
            {
                return;
            }

            switch (res)
            {
                case "追加":
                    path = EditorUtility.OpenFilePanel("读取色板信息文件", Application.dataPath, "json");
                    if (!string.IsNullOrEmpty(path))
                    {

                        string json = File.ReadAllText(path);

                        ExportColorLib lib = JsonUtility.FromJson<ExportColorLib>(json);
                        int count = lib.ColorsInfo.Count;

                        int size = sp_ColorLibrary_Original.arraySize;

                        for (int i = 0; i < count; i++)
                        {
                            sp_ColorLibrary_Original.InsertArrayElementAtIndex(i + size);

                            SerializedProperty sp_item = sp_ColorLibrary_Original.GetArrayElementAtIndex(i + size);
                            SerializedProperty sp_name = sp_item.FindPropertyRelative("Name");
                            SerializedProperty sp_color = sp_item.FindPropertyRelative("Color");
                            SerializedProperty sp_des = sp_item.FindPropertyRelative("Description");

                            sp_name.stringValue = lib.ColorsInfo[i].name;
                            sp_color.colorValue = XHud_Utilitys.Color_From_String(lib.ColorsInfo[i].color, false);
                            sp_des.stringValue = lib.ColorsInfo[i].description;

                            sp_name.serializedObject.ApplyModifiedProperties();
                            sp_color.serializedObject.ApplyModifiedProperties();
                            sp_des.serializedObject.ApplyModifiedProperties();
                            sp_item.serializedObject.ApplyModifiedProperties();
                        }
                        sp_ColorLibrary_Original.serializedObject.ApplyModifiedProperties();
                    }
                    break;
                case "替换":
                    path = EditorUtility.OpenFilePanel("读取色板信息文件", Application.dataPath, "json");
                    if (!string.IsNullOrEmpty(path))
                    {
                        sp_ColorLibrary_Original.ClearArray();

                        string json = File.ReadAllText(path);

                        ExportColorLib lib = JsonUtility.FromJson<ExportColorLib>(json);
                        int count = lib.ColorsInfo.Count;

                        for (int i = 0; i < count; i++)
                        {
                            sp_ColorLibrary_Original.InsertArrayElementAtIndex(i);

                            SerializedProperty sp_item = sp_ColorLibrary_Original.GetArrayElementAtIndex(i);
                            SerializedProperty sp_name = sp_item.FindPropertyRelative("Name");
                            SerializedProperty sp_color = sp_item.FindPropertyRelative("Color");
                            SerializedProperty sp_des = sp_item.FindPropertyRelative("Description");

                            sp_name.stringValue = lib.ColorsInfo[i].name;
                            sp_color.colorValue = XHud_Utilitys.Color_From_String(lib.ColorsInfo[i].color, false);
                            sp_des.stringValue = lib.ColorsInfo[i].description;
                            sp_name.serializedObject.ApplyModifiedProperties();
                            sp_color.serializedObject.ApplyModifiedProperties();
                            sp_des.serializedObject.ApplyModifiedProperties();
                            sp_item.serializedObject.ApplyModifiedProperties();
                        }
                        sp_ColorLibrary_Original.serializedObject.ApplyModifiedProperties();
                    }
                    break;
            }
        }
        #endregion
    }
}