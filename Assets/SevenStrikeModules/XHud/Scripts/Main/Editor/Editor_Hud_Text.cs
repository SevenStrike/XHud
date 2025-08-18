namespace SevenStrikeModules.XHud.Hud
{
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.GuiLib;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using TextEditor = UnityEditor.UI.TextEditor;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(Hud_Text))]
    public class Editor_Hud_Text : TextEditor
    {
        #region 组件
        private Hud_Text BaseScript;
        #endregion

        private bool BasicVars;

        #region 序列化属性
        private SerializedProperty sp_Indicator, sp_StyleName, sp_SyncGlobalFontSize, sp_StyleLibSynching, sp_TextStyleInfo, sp_Text;
        #endregion

        #region 图标
        private Texture2D locate_r, locate_p, icon_main, save_r, save_p, openlib_r, openlib_p, update_r, update_p;
        #endregion

        #region 选项文字
        string[] stroptions_enabled = new string[2] { "关闭", "开启" }, stroptions_syncsize = new string[] { "原生", "增量" }, stroptions_synccolor = new string[] { "原生", "接管" }, stroptions_syncstyle = new string[] { "原生", "同步" };
        #endregion

        #region 批量化操作
        private Hud_Text[] SelectedObjects;

        private void GetAllTargets()
        {
            if (targets.Length > 1)
            {
                SelectedObjects = new Hud_Text[targets.Length];
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    var t = targets[i];
                    SelectedObjects[i] = (Hud_Text)t;
                }
            }
            else
            {
                SelectedObjects = new Hud_Text[targets.Length];
                SelectedObjects[0] = (Hud_Text)target;
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

        protected override void OnEnable()
        {
            base.OnEnable();
            BaseScript = (Hud_Text)target;

            sp_Indicator = serializedObject.FindProperty("Indicator");
            sp_StyleName = serializedObject.FindProperty("StyleName");
            sp_StyleLibSynching = serializedObject.FindProperty("StyleLibSynching");
            sp_SyncGlobalFontSize = serializedObject.FindProperty("SyncGlobalFontSize");
            sp_TextStyleInfo = serializedObject.FindProperty("TextStyleInfo");
            sp_Text = serializedObject.FindProperty("m_Text");

            locate_r = util_XHUDGUI.GetIcon("Icons_Hud_Text/locate_r");
            locate_p = util_XHUDGUI.GetIcon("Icons_Hud_Text/locate_p");
            icon_main = util_XHUDGUI.GetIcon("Icons_Hud_Text/icon_main");
            save_r = util_XHUDGUI.GetIcon("Icons_Hud_Text/save_r");
            save_p = util_XHUDGUI.GetIcon("Icons_Hud_Text/save_p");
            openlib_r = util_XHUDGUI.GetIcon("Icons_Hud_Text/openlib_r");
            openlib_p = util_XHUDGUI.GetIcon("Icons_Hud_Text/openlib_p");
            update_r = util_XHUDGUI.GetIcon("Icons_Hud_Text/update_r");
            update_p = util_XHUDGUI.GetIcon("Icons_Hud_Text/update_p");

            GetAllTargets();

            Hud_Manager mgr = util_Dashboard.HudManagerGet();

            if (mgr != null)
            {
                if (mgr.Hud_TextStyleLibrary != null)
                {
                    if (string.IsNullOrEmpty(sp_StyleName.stringValue))
                    {
                        sp_StyleName.stringValue = mgr.Hud_TextStyleLibrary.TextStyle_Library_GetFirstStyleInfo_With_Type(TextType.Text).Name;
                        sp_StyleName.serializedObject.ApplyModifiedProperties();
                    }
                }
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            #region 寻找属性
            SerializedProperty sp_txt_style = sp_TextStyleInfo.FindPropertyRelative("Style");
            SerializedProperty sp_txt_content_anchor = sp_TextStyleInfo.FindPropertyRelative("ContentAnchor");
            SerializedProperty sp_txt_font = sp_TextStyleInfo.FindPropertyRelative("Font");
            SerializedProperty sp_txt_size = sp_TextStyleInfo.FindPropertyRelative("Size");
            SerializedProperty sp_txt_line_height = sp_TextStyleInfo.FindPropertyRelative("LineHeight");
            SerializedProperty sp_txt_overflow_h = sp_TextStyleInfo.FindPropertyRelative("Overflow_Horizon");
            SerializedProperty sp_txt_overflow_v = sp_TextStyleInfo.FindPropertyRelative("Overflow_Vertical");
            SerializedProperty sp_txt_rich = sp_TextStyleInfo.FindPropertyRelative("RichText");
            SerializedProperty sp_txt_bestfit = sp_TextStyleInfo.FindPropertyRelative("BestFit");
            SerializedProperty sp_txt_fit_min = sp_TextStyleInfo.FindPropertyRelative("Fit_Min");
            SerializedProperty sp_txt_fit_max = sp_TextStyleInfo.FindPropertyRelative("Fit_Max");
            SerializedProperty sp_txt_font_color = sp_TextStyleInfo.FindPropertyRelative("FontColor");
            SerializedProperty sp_txt_geometre_align = sp_TextStyleInfo.FindPropertyRelative("GeometreAlign");

            //---------------------------------LibraryToggle---------------------------------//
            SerializedProperty sp_LibStyle_Effect_align = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_align");
            SerializedProperty sp_LibStyle_Effect_font = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_font");
            SerializedProperty sp_LibStyle_Effect_style = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_style");
            SerializedProperty sp_LibStyle_Effect_size = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_size");
            SerializedProperty sp_LibStyle_Effect_color = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_color");
            SerializedProperty sp_LibStyle_Effect_rich = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_rich");
            SerializedProperty sp_LibStyle_Effect_line = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_line");
            SerializedProperty sp_LibStyle_Effect_overflow_h = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_overflow_h");
            SerializedProperty sp_LibStyle_Effect_overflow_v = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_overflow_v");
            SerializedProperty sp_LibStyle_Effect_bestfit = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_bestfit");
            SerializedProperty sp_LibStyle_Effect_raycast = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_raycast");
            SerializedProperty sp_LibStyle_Effect_maskable = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_maskable");
            SerializedProperty sp_LibStyle_Effect_geometre_align = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_geometre_align");
            //--------------------------------- Features ---------------------------------//
            SerializedProperty sp_Raycast = sp_TextStyleInfo.FindPropertyRelative("Raycast");
            SerializedProperty sp_Maskable = sp_TextStyleInfo.FindPropertyRelative("Maskable");
            SerializedProperty sp_SyncAnimatorColor = sp_TextStyleInfo.FindPropertyRelative("SyncAnimatorColor");
            #endregion

            #region 标题
            string titlename = "";
            if (string.IsNullOrEmpty(sp_Indicator.stringValue))
                titlename = "Hud - 文字";
            else
                titlename = sp_Indicator.stringValue;
            util_XHUDGUI.Gui_Layout_Banner(icon_main, HudFilled.实体, HudColor.深空灰, titlename, Color.white);
            #endregion

            Hud_Manager mgr = util_Dashboard.HudManagerGet();


            #region 按钮
            util_XHUDGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "快捷功能", util_Dashboard.Theme_Primary);
            util_XHUDGUI.Gui_Layout_Space(10);
            if (!IsMultiSelected())
            {
                util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                util_XHUDGUI.Gui_Layout_Space(10);

                if (mgr.Hud_TextStyleLibrary)
                {
                    #region 保存到字体库
                    if (util_XHUDGUI.Gui_Layout_Button(14, "将当前样式保存到字体库", save_r, save_p))
                    {
                        if (Application.isPlaying)
                            return;

                        ///---新建文字模版信息类
                        TextStyleInfo info = new TextStyleInfo();
                        info.Type = TextType.Text;
                        info.Style = (FontStyle)sp_txt_style.enumValueIndex;
                        info.ContentAnchor = (ContentAnchor)sp_txt_content_anchor.enumValueIndex;
                        info.Font = sp_txt_font.objectReferenceValue as Font;
                        info.Size = sp_txt_size.floatValue;
                        info.LineHeight = sp_txt_line_height.floatValue;
                        info.Overflow_Horizon = (HorizontalWrapMode)sp_txt_overflow_h.enumValueIndex;
                        info.Overflow_Vertical = (VerticalWrapMode)sp_txt_overflow_v.enumValueIndex;
                        info.RichText = sp_txt_rich.boolValue;
                        info.GeometreAlign = sp_txt_geometre_align.boolValue;
                        info.BestFit = sp_txt_bestfit.boolValue;
                        info.Fit_Min = sp_txt_fit_min.intValue;
                        info.Fit_Max = sp_txt_fit_max.intValue;
                        info.FontColor = sp_txt_font_color.colorValue;
                        info.Raycast = sp_Raycast.boolValue;
                        info.Maskable = sp_Maskable.boolValue;
                        info.SyncAnimatorColor = sp_SyncAnimatorColor.boolValue;

                        Open_Hud_Library_TextStyle_Setter(info);
                    }
                    #endregion

                    GUILayout.FlexibleSpace();

                    #region 查看字体库
                    GUI.enabled = true;
                    if (util_XHUDGUI.Gui_Layout_Button(14, "查看当前字体库", openlib_r, openlib_p))
                    {
                        if (mgr.Hud_TextStyleLibrary != null)
                            EditorUtility.OpenPropertyEditor(mgr.Hud_TextStyleLibrary);
                    }
                    #endregion
                }

                if (mgr.Hud_TextStyleLibrary != null && !mgr.Hud_TextStyleLibrary.TextStyle_Library_IsEmpty())
                {
                    GUILayout.FlexibleSpace();

                    #region 刷新样式
                    if (util_XHUDGUI.Gui_Layout_Button(14, "根据选择的快速样式刷新文字样式", update_r, update_p))
                    {
                        UpdateTextStyle_WithTarget();
                        return;
                    }
                    #endregion
                }

                util_XHUDGUI.Gui_Layout_Space(10);
                util_XHUDGUI.Gui_Layout_Horizontal_End();

            }
            else
            {
                util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                util_XHUDGUI.Gui_Layout_Space(10);
                GUILayout.FlexibleSpace();

                #region 批量刷新样式
                if (!mgr.Hud_TextStyleLibrary.TextStyle_Library_IsEmpty())
                {
                    if (util_XHUDGUI.Gui_Layout_Button(14, "根据选择的快速样式刷新文字样式", update_r, update_p))
                    {
                        UpdateTextStyle_WithTarget();
                        return;
                    }
                }
                #endregion

                GUILayout.FlexibleSpace();
                util_XHUDGUI.Gui_Layout_Space(10);
                util_XHUDGUI.Gui_Layout_Horizontal_End();
            }
            util_XHUDGUI.Gui_Layout_Space(10);
            util_XHUDGUI.Gui_Layout_Vertical_End();

            #endregion

            #region 选项
            util_XHUDGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "选项", util_Dashboard.Theme_Primary);
            util_XHUDGUI.Gui_Layout_Space(10);

            #region 射线检测可用性   
            if (!sp_LibStyle_Effect_raycast.boolValue || !sp_StyleLibSynching.boolValue)
                util_XHUDGUI.Gui_Layout_Toggle<bool, Hud_Text>("射线检测", stroptions_enabled, ref sp_Raycast, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
            #endregion

            #region 遮罩 
            if (!sp_LibStyle_Effect_maskable.boolValue || !sp_StyleLibSynching.boolValue)
                util_XHUDGUI.Gui_Layout_Toggle<bool, Hud_Text>("遮罩", stroptions_enabled, ref sp_Maskable, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
            #endregion

            #region 富文本支持           
            if (!sp_LibStyle_Effect_rich.boolValue || !sp_StyleLibSynching.boolValue)
                util_XHUDGUI.Gui_Layout_Toggle<bool, Hud_Text>("富文本", stroptions_enabled, ref sp_txt_rich, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
            #endregion

            #region 几何对齐
            if (!sp_LibStyle_Effect_geometre_align.boolValue || !sp_StyleLibSynching.boolValue)
                util_XHUDGUI.Gui_Layout_Toggle<bool, Hud_Text>("几何对齐", stroptions_enabled, ref sp_txt_geometre_align, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
            #endregion

            #region 自动字体尺寸
            if (!sp_LibStyle_Effect_size.boolValue && !sp_LibStyle_Effect_bestfit.boolValue || !sp_StyleLibSynching.boolValue)
            {
                util_XHUDGUI.Gui_Layout_Toggle<bool, Hud_Text>("自动尺寸", stroptions_enabled, ref sp_txt_bestfit, HudFilled.无, HudFilled.实体, Color.white, 120, 25, SelectedObjects);
            }
            #endregion

            #region 尺寸全局受控               
            util_XHUDGUI.Gui_Layout_Toggle<bool, Hud_Text>("全局尺寸增量", stroptions_syncsize, ref sp_SyncGlobalFontSize, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
            #endregion

            #region Animator接管字体颜色        
            util_XHUDGUI.Gui_Layout_Toggle<bool, Hud_Text>("动画器接管颜色", stroptions_synccolor, ref sp_SyncAnimatorColor, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
            #endregion

            #region 实时匹配库
            if (mgr != null && mgr.Hud_TextStyleLibrary != null)
            {
                if (!mgr.Hud_TextStyleLibrary.TextStyle_Library_IsEmpty())
                    util_XHUDGUI.Gui_Layout_Toggle<bool, Hud_Text>("实时匹配库", stroptions_enabled, ref sp_StyleLibSynching, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                else
                {
                    util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    util_XHUDGUI.Gui_Layout_Space(10);
                    EditorGUILayout.HelpBox("字体样式库为空", MessageType.Warning);
                    util_XHUDGUI.Gui_Layout_Space(5);
                    util_XHUDGUI.Gui_Layout_Horizontal_End();
                }
            }
            else
            {
                sp_StyleLibSynching.boolValue = false;
                sp_StyleLibSynching.serializedObject.ApplyModifiedProperties();
                util_XHUDGUI.Gui_Layout_Space(5);
                util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                util_XHUDGUI.Gui_Layout_Space(10);
                EditorGUILayout.HelpBox("您未在Hud管理器中配置或指定字体样式库", MessageType.Warning);
                util_XHUDGUI.Gui_Layout_Space(5);
                util_XHUDGUI.Gui_Layout_Horizontal_End();
            }
            #endregion

            #region 文字样式
            if (mgr != null && mgr.Hud_TextStyleLibrary != null)
            {
                if (mgr.Hud_TextStyleLibrary.TextStyle_Library_GetCount(TextType.Text) > 0)
                {
                    string[] str_fotlib_ItemsName = mgr.Hud_TextStyleLibrary.TextStyle_Library_GetAllNames_With_Text();
                    util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);

                    util_XHUDGUI.Gui_Layout_Popup<string, Hud_Text>("文字样式库", str_fotlib_ItemsName, ref sp_StyleName, HudFilled.实体, 94, 22, SelectedObjects, (comps) =>
                    {
                        for (int i = 0; i < comps.Length; i++)
                        {
                            if (Application.isPlaying)
                            {
                                comps[i].TextStyleInfo.CopyData_Ignored_LibraryToggle(mgr.Hud_TextStyleLibrary.TextStyle_Library_GetTextStyleInfo(sp_StyleName.stringValue));
                            }
                            else
                            {
                                comps[i].TextStyleInfo.CopyData_Ignored_LibraryToggle(mgr.Hud_TextStyleLibrary.TextStyle_Library_GetTextStyleInfo(sp_StyleName.stringValue));
                            }
                        }
                    }, (res) =>
                    {
                        if (Application.isPlaying)
                        {
                            BaseScript.TextStyleInfo.CopyData_Ignored_LibraryToggle(mgr.Hud_TextStyleLibrary.TextStyle_Library_GetTextStyleInfo(res));
                        }
                        else
                        {
                            BaseScript.TextStyleInfo.CopyData_Ignored_LibraryToggle(mgr.Hud_TextStyleLibrary.TextStyle_Library_GetTextStyleInfo(res));
                        }
                    });

                    if (!IsMultiSelected())
                    {
                        util_XHUDGUI.Gui_Layout_Space(10);

                        if (util_XHUDGUI.Gui_Layout_Button(14, "定位到样式库", locate_r, locate_p, 2))
                        {
                            //获取目标元素的索引号
                            int index = 0;
                            for (int k = 0; k < str_fotlib_ItemsName.Length; k++)
                            {
                                if (str_fotlib_ItemsName[k] == sp_StyleName.stringValue)
                                {
                                    index = k;
                                    break;
                                }
                            }
                            //定位到元素库中的对应当前元素
                            //打开目标元素库
                            util_OpenLibrarys.open_font();
                            mgr.Hud_TextStyleLibrary.TextStyleLibrary_Location(str_fotlib_ItemsName[index]);
                        }
                        //util_EditorGuiLib.Gui_Layout_Space(5);
                    }
                    util_XHUDGUI.Gui_Layout_Horizontal_End();
                }
            }
            #endregion

            util_XHUDGUI.Gui_Layout_Space(10);
            util_XHUDGUI.Gui_Layout_Vertical_End();
            #endregion

            #region 库样式同步选项
            if (sp_StyleLibSynching.boolValue)
            {
                util_XHUDGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "库样式同步选项", util_Dashboard.Theme_Primary);
                util_XHUDGUI.Gui_Layout_Space(10);

                #region 射线检测
                util_XHUDGUI.Gui_Layout_Toggle<bool, Hud_Text>("射线检测", stroptions_syncstyle, ref sp_LibStyle_Effect_raycast, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                #endregion

                #region 遮罩
                util_XHUDGUI.Gui_Layout_Toggle<bool, Hud_Text>("遮罩", stroptions_syncstyle, ref sp_LibStyle_Effect_maskable, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                #endregion

                #region 富文本
                util_XHUDGUI.Gui_Layout_Toggle<bool, Hud_Text>("富文本", stroptions_syncstyle, ref sp_LibStyle_Effect_rich, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                #endregion

                #region 尺寸
                util_XHUDGUI.Gui_Layout_Toggle<bool, Hud_Text>("尺寸", stroptions_syncstyle, ref sp_LibStyle_Effect_size, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                if (sp_LibStyle_Effect_size.boolValue)
                {
                    sp_LibStyle_Effect_bestfit.boolValue = false;
                    sp_LibStyle_Effect_bestfit.serializedObject.ApplyModifiedProperties();
                    sp_txt_bestfit.boolValue = false;
                    sp_txt_bestfit.serializedObject.ApplyModifiedProperties();
                }
                #endregion

                #region 自适应尺寸
                util_XHUDGUI.Gui_Layout_Toggle<bool, Hud_Text>("自适应尺寸", stroptions_syncstyle, ref sp_LibStyle_Effect_bestfit, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                if (sp_LibStyle_Effect_bestfit.boolValue)
                {
                    sp_LibStyle_Effect_size.boolValue = false;
                    sp_LibStyle_Effect_size.serializedObject.ApplyModifiedProperties();
                    sp_txt_bestfit.boolValue = true;
                    sp_txt_bestfit.serializedObject.ApplyModifiedProperties();
                }
                #endregion

                #region 对齐
                util_XHUDGUI.Gui_Layout_Toggle<bool, Hud_Text>("对齐", stroptions_syncstyle, ref sp_LibStyle_Effect_align, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                #endregion

                #region 对齐
                util_XHUDGUI.Gui_Layout_Toggle<bool, Hud_Text>("几何对齐", stroptions_syncstyle, ref sp_LibStyle_Effect_geometre_align, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                #endregion

                #region 行高
                util_XHUDGUI.Gui_Layout_Toggle<bool, Hud_Text>("行高", stroptions_syncstyle, ref sp_LibStyle_Effect_line, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                #endregion

                #region 字体
                util_XHUDGUI.Gui_Layout_Toggle<bool, Hud_Text>("字体", stroptions_syncstyle, ref sp_LibStyle_Effect_font, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                #endregion

                #region 样式
                util_XHUDGUI.Gui_Layout_Toggle<bool, Hud_Text>("样式", stroptions_syncstyle, ref sp_LibStyle_Effect_style, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                #endregion

                #region 颜色
                util_XHUDGUI.Gui_Layout_Toggle<bool, Hud_Text>("颜色", stroptions_syncstyle, ref sp_LibStyle_Effect_color, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                #endregion

                #region 水平溢出
                util_XHUDGUI.Gui_Layout_Toggle<bool, Hud_Text>("水平溢出", stroptions_syncstyle, ref sp_LibStyle_Effect_overflow_h, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                #endregion

                #region 垂直溢出
                util_XHUDGUI.Gui_Layout_Toggle<bool, Hud_Text>("垂直溢出", stroptions_syncstyle, ref sp_LibStyle_Effect_overflow_v, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                #endregion

                util_XHUDGUI.Gui_Layout_Space(10);
                util_XHUDGUI.Gui_Layout_Vertical_End();
            }
            #endregion

            #region 参数
            util_XHUDGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "参数", util_Dashboard.Theme_Primary);
            util_XHUDGUI.Gui_Layout_Space(10);

            #region 内容
            EditorGUI.BeginChangeCheck();
            util_XHUDGUI.Gui_Layout_Property_Field("文字内容", sp_Text);
            if (EditorGUI.EndChangeCheck())
            {
                if (IsMultiSelected())
                {
                    for (int i = 0; i < SelectedObjects.Length; i++)
                    {
                        SelectedObjects[i].text = sp_Text.stringValue;
                    }
                }
            }
            util_XHUDGUI.Gui_Layout_Space(10);

            #endregion

            util_XHUDGUI.Gui_Layout_Space(5);

            #region 标识
            EditorGUI.BeginChangeCheck();
            util_XHUDGUI.Gui_Layout_Property_Field("标识", sp_Indicator);
            if (EditorGUI.EndChangeCheck())
            {
                if (IsMultiSelected())
                {
                    for (int i = 0; i < SelectedObjects.Length; i++)
                    {
                        SelectedObjects[i].Indicator = sp_Indicator.stringValue;
                    }
                }
            }
            #endregion

            util_XHUDGUI.Gui_Layout_Space(5);

            #region 字体 
            if (!sp_LibStyle_Effect_font.boolValue || !sp_StyleLibSynching.boolValue)
            {
                util_XHUDGUI.Gui_Layout_Property_Field("字体", sp_txt_font);
                util_XHUDGUI.Gui_Layout_Space(5);
            }
            #endregion


            #region 内容锚点 
            if (!sp_LibStyle_Effect_align.boolValue || !sp_StyleLibSynching.boolValue)
            {
                EditorGUI.BeginChangeCheck();
                util_XHUDGUI.Gui_Layout_Property_Field("锚点", sp_txt_content_anchor);
                if (EditorGUI.EndChangeCheck())
                {
                    if (IsMultiSelected())
                    {
                        for (int i = 0; i < SelectedObjects.Length; i++)
                        {
                            ContentAnchor anchor = (ContentAnchor)sp_txt_content_anchor.enumValueIndex;
                            SelectedObjects[i].TextStyleInfo.txt_Set_Alignment(anchor);
                        }
                    }
                    else
                    {
                        ContentAnchor anchor = (ContentAnchor)sp_txt_content_anchor.enumValueIndex;
                        BaseScript.TextStyleInfo.txt_Set_Alignment(anchor);
                    }
                }
                util_XHUDGUI.Gui_Layout_Space(5);
            }
            #endregion


            #region 水平溢出 
            if (!sp_LibStyle_Effect_overflow_h.boolValue || !sp_StyleLibSynching.boolValue)
            {
                EditorGUI.BeginChangeCheck();
                util_XHUDGUI.Gui_Layout_Property_Field("水平溢出", sp_txt_overflow_h);
                if (EditorGUI.EndChangeCheck())
                {
                    if (IsMultiSelected())
                    {
                        for (int i = 0; i < SelectedObjects.Length; i++)
                        {
                            HorizontalWrapMode anchor = (HorizontalWrapMode)sp_txt_overflow_h.enumValueIndex;
                            SelectedObjects[i].TextStyleInfo.txt_Set_Overflow(anchor);
                        }
                    }
                    else
                    {
                        HorizontalWrapMode anchor = (HorizontalWrapMode)sp_txt_overflow_h.enumValueIndex;
                        BaseScript.TextStyleInfo.txt_Set_Overflow(anchor);
                    }
                }
                util_XHUDGUI.Gui_Layout_Space(5);
            }
            #endregion


            #region 垂直溢出 
            if (!sp_LibStyle_Effect_overflow_v.boolValue || !sp_StyleLibSynching.boolValue)
            {
                EditorGUI.BeginChangeCheck();
                util_XHUDGUI.Gui_Layout_Property_Field("垂直溢出", sp_txt_overflow_v);
                if (EditorGUI.EndChangeCheck())
                {
                    if (IsMultiSelected())
                    {
                        for (int i = 0; i < SelectedObjects.Length; i++)
                        {
                            VerticalWrapMode anchor = (VerticalWrapMode)sp_txt_overflow_v.enumValueIndex;
                            SelectedObjects[i].TextStyleInfo.txt_Set_Overflow(anchor);
                        }
                    }
                    else
                    {
                        VerticalWrapMode anchor = (VerticalWrapMode)sp_txt_overflow_v.enumValueIndex;
                        BaseScript.TextStyleInfo.txt_Set_Overflow(anchor);
                    }
                }
                util_XHUDGUI.Gui_Layout_Space(5);
            }
            #endregion


            #region 字体大小 
            if (!sp_LibStyle_Effect_size.boolValue || !sp_StyleLibSynching.boolValue)
            {
                if (!sp_txt_bestfit.boolValue)
                {
                    util_XHUDGUI.Gui_Layout_Property_Field("尺寸", sp_txt_size);
                    util_XHUDGUI.Gui_Layout_Space(5);
                }
            }
            #endregion

            #region 自适应字体大小           
            if (!sp_LibStyle_Effect_bestfit.boolValue || !sp_StyleLibSynching.boolValue)
            {
                if (sp_txt_bestfit.boolValue)
                {
                    util_XHUDGUI.Gui_Layout_Property_Field("最小尺寸", sp_txt_fit_min);
                    util_XHUDGUI.Gui_Layout_Space(5);
                    util_XHUDGUI.Gui_Layout_Property_Field("最大尺寸", sp_txt_fit_max);
                    util_XHUDGUI.Gui_Layout_Space(5);
                }
            }
            #endregion

            #region 字体样式 
            if (!sp_LibStyle_Effect_style.boolValue || !sp_StyleLibSynching.boolValue)
            {
                util_XHUDGUI.Gui_Layout_Property_Field("样式", sp_txt_style);
                util_XHUDGUI.Gui_Layout_Space(5);
            }
            #endregion

            #region 行高 
            if (!sp_LibStyle_Effect_line.boolValue || !sp_StyleLibSynching.boolValue)
            {
                util_XHUDGUI.Gui_Layout_Property_Field("行高", sp_txt_line_height);
                util_XHUDGUI.Gui_Layout_Space(5);
            }
            #endregion

            #region 颜色 
            if (!sp_LibStyle_Effect_color.boolValue || !sp_StyleLibSynching.boolValue)
            {
                util_XHUDGUI.Gui_Layout_Property_Field("颜色", sp_txt_font_color);
            }
            #endregion

            util_XHUDGUI.Gui_Layout_Space(10);
            util_XHUDGUI.Gui_Layout_Vertical_End();
            #endregion

            Event e = Event.current;
            if (e.type == EventType.MouseDown && e.button == 1)
            {
                // 创建右键菜单
                GenericMenu menu = new GenericMenu();
                menu.AddDisabledItem(new GUIContent("组件样式操作"));
                menu.AddItem(new GUIContent("C (复制样式)"), false, () =>
                {
                    TextStyleInfo info = new TextStyleInfo();
                    info.CopyData_Ignored_LibraryToggle(BaseScript.TextStyleInfo);

                    string json = JsonUtility.ToJson(info);
                    util_XHUDGUI.EditorData_Set_With_String("XED_HudText_Get_TextStyle", json);

                    util_XHUDGUI.Open(XHudDialogType.确认, "HudText 文字组件消息", "文字样式复制", $"已将当前文字样式存入 XHudEditorData (XED)！", "好的");
                });
                menu.AddItem(new GUIContent("V (粘贴样式)"), false, () =>
                {
                    TextStyleInfo info = JsonUtility.FromJson<TextStyleInfo>(util_XHUDGUI.EditorData_Get_With_String("XED_HudText_Get_TextStyle"));

                    if (IsMultiSelected())
                    {
                        for (int i = 0; i < SelectedObjects.Length; i++)
                        {
                            // 使用 Undo.RecordObject 来记录对目标对象的修改
                            Undo.RecordObject(SelectedObjects[i], "Paste Style Name");
                            SelectedObjects[i].TextStyleInfo.CopyData(info);
                        }

                        util_XHUDGUI.Open(XHudDialogType.确认, "HudText 文字组件消息", "文字样式粘贴", $"已从 XHudEditorData (XED) 中获取文字样式并应用到当前文字组件！", "好的");
                    }
                    else
                    {
                        // 使用 Undo.RecordObject 来记录对目标对象的修改
                        Undo.RecordObject(BaseScript, "Paste Style Name");

                        BaseScript.TextStyleInfo.CopyData(info);

                        util_XHUDGUI.Open(XHudDialogType.确认, "HudText 文字组件消息", "文字样式粘贴", $"已从 XHudEditorData (XED) 中获取文字样式并应用到当前文字组件！", "好的");
                    }
                });
                menu.AddSeparator("");
                menu.AddDisabledItem(new GUIContent("文字样式库"));
                menu.AddItem(new GUIContent("X (识别)"), false, () =>
                {
                    TextStyleInfo info = JsonUtility.FromJson<TextStyleInfo>(util_XHUDGUI.EditorData_Get_With_String("XED_HudTextStyleLibrary_Get_TextStyle"));

                    if (IsMultiSelected())
                    {
                        List<XHudDialogListDatas> datas = new List<XHudDialogListDatas>();

                        for (int i = 0; i < SelectedObjects.Length; i++)
                        {
                            XHudDialogListDatas data = new XHudDialogListDatas();

                            if (info.Type != TextType.Text)
                            {
                                util_XHUDGUI.Open(XHudDialogType.错误, "HudText 文字组件消息", "文字样式识别", $"从 XHudEditorData (XED) 中识别的文字样式似乎并不是用于 {SelectedObjects[i].name} ( {SelectedObjects[i].Indicator} ) 的文字组件的规范样式！请检查您复制的样式类型是否和当前组件一致？", "好的");

                                data.Title = info.Name;
                                data.SubTitle = "文字样式类型未能匹配到";
                                data.Message = $"{SelectedObjects[i].name} ( {SelectedObjects[i].Indicator} )";

                                datas.Add(data);
                                continue;
                            }
                            else
                            {

                                // 使用 Undo.RecordObject 来记录对目标对象的修改
                                Undo.RecordObject(SelectedObjects[i], "Paste Style Name");

                                SelectedObjects[i].TextStyleInfo.CopyData_Ignored_LibraryToggle(info);

                                data.Title = info.Name;
                                data.SubTitle = "文字样式已应用到";
                                data.Message = $"{SelectedObjects[i].name} ( {SelectedObjects[i].Indicator} )";
                                datas.Add(data);
                            }
                        }

                        util_XHUDGUI.Open(datas.ToArray(), XHudDialogType.确认, "HudText 文字组件消息", "文字样式识别", $"已从 XHudEditorData (XED) 中识别到文字样式并应用到如下文字组件中！", "好的");
                    }
                    else
                    {
                        if (info.Type != TextType.Text)
                        {
                            util_XHUDGUI.Open(XHudDialogType.错误, "HudText 文字组件消息", "文字样式识别", $"从 XHudEditorData (XED) 中识别的文字样式似乎并不是用于当前文字组件的规范样式！请检查您复制的样式类型是否和当前组件一致？", "好的");
                            return;
                        }

                        // 使用 Undo.RecordObject 来记录对目标对象的修改
                        Undo.RecordObject(BaseScript, "Paste Style Name");

                        BaseScript.TextStyleInfo.CopyData_Ignored_LibraryToggle(info);

                        util_XHUDGUI.Open(XHudDialogType.确认, "HudText 文字组件消息", "文字样式识别", $"已从 XHudEditorData (XED) 中识别到文字样式并应用到当前文字组件！", "好的");
                    }
                });
                menu.AddSeparator("");
                menu.AddDisabledItem(new GUIContent("同步开关"));
                menu.AddItem(new GUIContent("S (全部同步)"), false, () =>
                {
                    sp_LibStyle_Effect_align.boolValue = true;
                    sp_LibStyle_Effect_font.boolValue = true;
                    sp_LibStyle_Effect_style.boolValue = true;
                    sp_LibStyle_Effect_size.boolValue = true;
                    sp_LibStyle_Effect_color.boolValue = true;
                    sp_LibStyle_Effect_rich.boolValue = true;
                    sp_LibStyle_Effect_line.boolValue = true;
                    sp_LibStyle_Effect_overflow_h.boolValue = true;
                    sp_LibStyle_Effect_overflow_v.boolValue = true;
                    sp_LibStyle_Effect_bestfit.boolValue = true;
                    sp_LibStyle_Effect_raycast.boolValue = true;
                    sp_LibStyle_Effect_maskable.boolValue = true;

                    sp_LibStyle_Effect_align.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_font.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_style.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_size.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_color.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_rich.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_line.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_overflow_h.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_overflow_v.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_bestfit.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_raycast.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_maskable.serializedObject.ApplyModifiedProperties();
                });
                menu.AddItem(new GUIContent("D (全部关闭)"), false, () =>
                {
                    sp_LibStyle_Effect_align.boolValue = false;
                    sp_LibStyle_Effect_font.boolValue = false;
                    sp_LibStyle_Effect_style.boolValue = false;
                    sp_LibStyle_Effect_size.boolValue = false;
                    sp_LibStyle_Effect_color.boolValue = false;
                    sp_LibStyle_Effect_rich.boolValue = false;
                    sp_LibStyle_Effect_line.boolValue = false;
                    sp_LibStyle_Effect_overflow_h.boolValue = false;
                    sp_LibStyle_Effect_overflow_v.boolValue = false;
                    sp_LibStyle_Effect_bestfit.boolValue = false;
                    sp_LibStyle_Effect_raycast.boolValue = false;
                    sp_LibStyle_Effect_maskable.boolValue = false;

                    sp_LibStyle_Effect_align.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_font.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_style.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_size.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_color.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_rich.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_line.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_overflow_h.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_overflow_v.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_bestfit.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_raycast.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_maskable.serializedObject.ApplyModifiedProperties();
                });
                // 显示右键菜单
                menu.ShowAsContext();

                e.Use(); // 标记事件已被处理，防止其他操作处理该事件
            }

            #region 源脚本
            util_XHUDGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 3, "源脚本", util_Dashboard.Theme_Primary);
            util_XHUDGUI.Gui_Layout_Space(5);

            #region 原始变量
            util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            util_XHUDGUI.Gui_Layout_Space(10);
            BasicVars = EditorGUILayout.Foldout(BasicVars, "变量/属性", true);
            util_XHUDGUI.Gui_Layout_Space(5);
            util_XHUDGUI.Gui_Layout_Horizontal_End();
            if (BasicVars)
                DrawDefaultInspector();
            #endregion

            util_XHUDGUI.Gui_Layout_Space(5);
            util_XHUDGUI.Gui_Layout_Vertical_End();
            #endregion

            serializedObject.ApplyModifiedProperties();
        }

        #region 辅助

        /// <summary>
        /// 根据文字样式名称索引刷新文字样式
        /// </summary>
        private void UpdateTextStyle_WithTarget()
        {
            Hud_Manager mgr = util_Dashboard.HudManagerGet();

            TextStyleInfo s_info = null;
            if (IsMultiSelected())
            {
                var s = targets;
                foreach (var t in s)
                {
                    Hud_Text tt = (Hud_Text)t;

                    if (Application.isPlaying)
                    {
                        if (mgr.Hud_TextStyleLibrary.TextStyle_Library_NameIsValid(tt.StyleName))
                        {
                            s_info = mgr.Hud_TextStyleLibrary.TextStyle_Library_GetTextStyleInfo(tt.StyleName, TextType.Text);
                        }
                        else
                        {
                            string res = util_XHUDGUI.Open(XHudDialogType.警告, "HudText 文字组件消息", "刷新文字库样式", "似乎 {tt.name} ( {tt.Indicator} ) 文字组件的样式库名称并不在文字样式库中存在，请检查文字样式库名称是否合法，或者可以快速选择文字样式库默认首个样式。是否要选择首个样式？", "暂不", "选择", 0);
                            if (res == "选择")
                            {
                                s_info = mgr.Hud_TextStyleLibrary.TextStyle_Library_GetFirstStyleInfo_With_Type(TextType.Text);

                                // 使用 Undo.RecordObject 来记录对目标对象的修改
                                Undo.RecordObject(tt, "Change Style Name");

                                tt.StyleName = s_info.Name;
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                    else
                    {
                        if (mgr.Hud_TextStyleLibrary.TextStyle_Library_NameIsValid(tt.StyleName))
                        {
                            s_info = mgr.Hud_TextStyleLibrary.TextStyle_Library_GetTextStyleInfo(tt.StyleName, TextType.Text);
                        }
                        else
                        {
                            string res = util_XHUDGUI.Open(XHudDialogType.警告, "HudText 文字组件消息", "刷新文字库样式", $"似乎 {tt.name} ( {tt.Indicator} ) 文字组件指定的样式库名称并不在文字样式库中存在，请检查文字样式库名称是否合法，或者可以快速选择文字样式库默认首个样式。是否要选择首个样式？", "暂不", "选择", 0);
                            if (res == "选择")
                            {
                                s_info = mgr.Hud_TextStyleLibrary.TextStyle_Library_GetFirstStyleInfo_With_Type(TextType.Text);

                                // 使用 Undo.RecordObject 来记录对目标对象的修改
                                Undo.RecordObject(tt, "Change Style Name");

                                tt.StyleName = s_info.Name;
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                    tt.TextStyleInfo.CopyData_Ignored_LibraryToggle(s_info);
                }
            }
            else
            {
                if (Application.isPlaying)
                {
                    if (mgr.Hud_TextStyleLibrary.TextStyle_Library_NameIsValid(sp_StyleName.stringValue))
                        s_info = mgr.Hud_TextStyleLibrary.TextStyle_Library_GetTextStyleInfo(sp_StyleName.stringValue, TextType.Text);
                    else
                    {
                        string res = util_XHUDGUI.Open(XHudDialogType.警告, "HudText 文字组件消息", "刷新文字库样式", "似乎您指定的样式库名称并不在文字样式库中存在，请检查文字样式库名称是否合法，或者可以快速选择文字样式库默认首个样式。是否要选择首个样式？", "暂不", "选择", 0);
                        if (res == "选择")
                        {
                            s_info = mgr.Hud_TextStyleLibrary.TextStyle_Library_GetFirstStyleInfo_With_Type(TextType.Text);

                            // 使用 Undo.RecordObject 来记录对目标对象的修改
                            Undo.RecordObject(sp_StyleName.serializedObject.targetObject, "Change Style Name");

                            sp_StyleName.stringValue = s_info.Name;
                            sp_StyleName.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                        }
                        else
                        {
                            return;
                        }
                    }
                }
                else
                {
                    if (mgr.Hud_TextStyleLibrary.TextStyle_Library_NameIsValid(sp_StyleName.stringValue))
                    {
                        s_info = mgr.Hud_TextStyleLibrary.TextStyle_Library_GetTextStyleInfo(sp_StyleName.stringValue, TextType.Text);
                    }
                    else
                    {
                        string res = util_XHUDGUI.Open(XHudDialogType.警告, "HudText 文字组件消息", "刷新文字库样式", $"似乎 {BaseScript.name} ( {BaseScript.Indicator} ) 文字组件样式库名称并不在文字样式库中存在，请检查文字样式库名称是否合法，或者可以快速选择文字样式库默认首个样式。是否要选择首个样式？", "暂不", "选择", 0);
                        if (res == "选择")
                        {
                            s_info = mgr.Hud_TextStyleLibrary.TextStyle_Library_GetFirstStyleInfo_With_Type(TextType.Text);

                            // 使用 Undo.RecordObject 来记录对目标对象的修改
                            Undo.RecordObject(sp_StyleName.serializedObject.targetObject, "Change Style Name");

                            sp_StyleName.stringValue = s_info.Name;
                            sp_StyleName.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                        }
                        else
                        {
                            return;
                        }
                    }
                }

                BaseScript.TextStyleInfo.CopyData_Ignored_LibraryToggle(s_info);
                return;
            }
        }

        #endregion

        #region 打开字体样式添加器

        /// <summary>
        /// 字体库添加器
        /// </summary>
        public void Open_Hud_Library_TextStyle_Setter(TextStyleInfo info)
        {
            util_Hud_Library_TextStyle_Setter window = EditorWindow.GetWindow<util_Hud_Library_TextStyle_Setter>(true);

            window.titleContent = new GUIContent("XHud 字体样式库采集器");
            util_XHUDGUI.CenterEditorWindow(new Vector2Int(630, 620), window);

            window.SetComponent(BaseScript);
            window.SetStyle(info);
            window.SetOriginStyle(info);
            window.SetLibrarySetterMode(LibrarySetterMode.添加到库);
            window.SetButtonText("添加", "取消");
            window.SetTitle("XHud 文字样式库采集器");
            //window.ShowModal();
            window.Show();
        }
        #endregion
    }
}