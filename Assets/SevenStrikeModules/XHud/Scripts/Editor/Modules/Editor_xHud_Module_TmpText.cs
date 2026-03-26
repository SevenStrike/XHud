namespace SevenStrikeModules.XHud.Hud
{
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.GuiLib;
    using System.Collections.Generic;
    using TMPro;
    using TMPro.EditorUtilities;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;
    using Application = UnityEngine.Application;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(XHud_Module_TmpText))]
    public class Editor_XHud_Module_TmpText : TMP_BaseEditorPanel
    {
        #region 组件
        private XHud_Module_TmpText BaseScript;
        #endregion

        private bool BasicVars;

        #region 序列化属性
        private SerializedProperty sp_Indicator, sp_StyleName, sp_SyncGlobalFontSize, sp_StyleLibSynching, sp_TextStyleInfo, sp_Text;
        #endregion

        #region 图标
        private Texture2D locate_r, locate_p, icon_main, save_r, save_p, openlib_r, openlib_p, update_r, update_p;
        #endregion

        #region 选项文字
        string[] stroptions_enabled = new string[2] { "关闭", "开启" }, stroptions_gramode = new string[] { "单色", "水平渐变", "垂直渐变", "四角渐变" }, stroptions_syncsize = new string[] { "原生", "增量" }, stroptions_synccolor = new string[] { "原生", "接管" }, stroptions_flip = new string[] { "常规", "翻转" }, stroptions_syncstyle = new string[] { "原生", "同步" };
        #endregion

        #region 渐变色预览
        private Texture2D GradientTexture;
        private int gra_TexWidth = 40;
        private int gra_TexHeight = 40;
        #endregion

        #region 批量化操作
        private XHud_Module_TmpText[] SelectedObjects;

        private void GetAllTargets()
        {
            if (targets.Length > 1)
            {
                SelectedObjects = new XHud_Module_TmpText[targets.Length];
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    var t = targets[i];
                    SelectedObjects[i] = (XHud_Module_TmpText)t;
                }
            }
            else
            {
                SelectedObjects = new XHud_Module_TmpText[targets.Length];
                SelectedObjects[0] = (XHud_Module_TmpText)target;
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
            BaseScript = (XHud_Module_TmpText)target;

            sp_RaycastTargetProp = serializedObject.FindProperty("m_RaycastTarget");
            sp_MaskableProp = serializedObject.FindProperty("m_Maskable");

            sp_Indicator = serializedObject.FindProperty("Indicator");
            sp_StyleName = serializedObject.FindProperty("StyleName");
            sp_StyleLibSynching = serializedObject.FindProperty("StyleLibSynching");
            sp_SyncGlobalFontSize = serializedObject.FindProperty("SyncGlobalFontSize");
            sp_TextStyleInfo = serializedObject.FindProperty("TextStyleInfo");
            sp_Text = serializedObject.FindProperty("m_text");

            locate_r = Editor_XHud_GUI.GetIcon("Icons_Hud_TmpText/locate_r");
            locate_p = Editor_XHud_GUI.GetIcon("Icons_Hud_TmpText/locate_p");
            icon_main = Editor_XHud_GUI.GetIcon("Icons_Hud_TmpText/icon_main");
            save_r = Editor_XHud_GUI.GetIcon("Icons_Hud_TmpText/save_r");
            save_p = Editor_XHud_GUI.GetIcon("Icons_Hud_TmpText/save_p");
            openlib_r = Editor_XHud_GUI.GetIcon("Icons_Hud_TmpText/openlib_r");
            openlib_p = Editor_XHud_GUI.GetIcon("Icons_Hud_TmpText/openlib_p");
            update_r = Editor_XHud_GUI.GetIcon("Icons_Hud_TmpText/update_r");
            update_p = Editor_XHud_GUI.GetIcon("Icons_Hud_TmpText/update_p");

            GetAllTargets();

            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

            if (mgr != null)
            {
                if (mgr.Hud_TextStyleLibrary != null)
                {
                    if (string.IsNullOrEmpty(sp_StyleName.stringValue))
                    {
                        if (!mgr.Hud_TextStyleLibrary.TextStyle_Library_IsEmpty())
                        {
                            sp_StyleName.stringValue = mgr.Hud_TextStyleLibrary.TextStyle_Library_GetFirstStyleInfo_With_Type(xHud_TextType.TmpText).Name;
                            sp_StyleName.serializedObject.ApplyModifiedProperties();
                        }
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
            SerializedProperty sp_tmp_rich = sp_TextStyleInfo.FindPropertyRelative("tmp_rich");
            SerializedProperty sp_tmp_contentwrap = sp_TextStyleInfo.FindPropertyRelative("tmp_contentwrap");
            SerializedProperty sp_tmp_overflow = sp_TextStyleInfo.FindPropertyRelative("tmp_overflow");
            SerializedProperty sp_tmp_style = sp_TextStyleInfo.FindPropertyRelative("tmp_style");
            SerializedProperty sp_tmp_anchor = sp_TextStyleInfo.FindPropertyRelative("tmp_anchor");
            SerializedProperty sp_tmp_font = sp_TextStyleInfo.FindPropertyRelative("tmp_font");
            SerializedProperty sp_tmp_size = sp_TextStyleInfo.FindPropertyRelative("tmp_size");
            SerializedProperty sp_tmp_space_character = sp_TextStyleInfo.FindPropertyRelative("tmp_space_character");
            SerializedProperty sp_tmp_space_word = sp_TextStyleInfo.FindPropertyRelative("tmp_space_word");
            SerializedProperty sp_tmp_space_lineheight = sp_TextStyleInfo.FindPropertyRelative("tmp_space_lineheight");
            SerializedProperty sp_tmp_space_paragraph = sp_TextStyleInfo.FindPropertyRelative("tmp_space_paragraph");
            SerializedProperty sp_tmp_WrappingRatios = sp_TextStyleInfo.FindPropertyRelative("tmp_WrappingRatios");
            SerializedProperty sp_tmp_contentmargin = sp_TextStyleInfo.FindPropertyRelative("tmp_contentmargin");
            SerializedProperty sp_tmp_color = sp_TextStyleInfo.FindPropertyRelative("tmp_color");
            SerializedProperty sp_tmp_EnableAutoSizing = sp_TextStyleInfo.FindPropertyRelative("tmp_EnableAutoSizing");
            SerializedProperty sp_tmp_FontSizeMin = sp_TextStyleInfo.FindPropertyRelative("tmp_FontSizeMin");
            SerializedProperty sp_tmp_FontSizeMax = sp_TextStyleInfo.FindPropertyRelative("tmp_FontSizeMax");
            SerializedProperty sp_tmp_CharWidthMaxAdj = sp_TextStyleInfo.FindPropertyRelative("tmp_CharWidthMaxAdj");
            SerializedProperty sp_tmp_LineSpacingMax = sp_TextStyleInfo.FindPropertyRelative("tmp_LineSpacingMax");
            //---------------------------------Tmp Text Gradient ---------------------------------//
            SerializedProperty sp_gra_A = sp_TextStyleInfo.FindPropertyRelative("gra_A");
            SerializedProperty sp_gra_B = sp_TextStyleInfo.FindPropertyRelative("gra_B");
            SerializedProperty sp_gra_C = sp_TextStyleInfo.FindPropertyRelative("gra_C");
            SerializedProperty sp_gra_D = sp_TextStyleInfo.FindPropertyRelative("gra_D");
            SerializedProperty sp_gra_Invert = sp_TextStyleInfo.FindPropertyRelative("gra_Invert");
            SerializedProperty sp_gra_Used = sp_TextStyleInfo.FindPropertyRelative("gra_Used");
            SerializedProperty sp_gra_ColorMode = sp_TextStyleInfo.FindPropertyRelative("gra_ColorMode");
            SerializedProperty sp_gra_ColorModeName = sp_TextStyleInfo.FindPropertyRelative("gra_ColorModeName");
            SerializedProperty sp_gra_ColorModeNameIndex = sp_TextStyleInfo.FindPropertyRelative("gra_ColorModeNameIndex");

            //---------------------------------LibraryToggle---------------------------------//
            SerializedProperty sp_LibStyle_Effect_align = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_align");
            SerializedProperty sp_LibStyle_Effect_font = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_font");
            SerializedProperty sp_LibStyle_Effect_style = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_style");
            SerializedProperty sp_LibStyle_Effect_size = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_size");
            SerializedProperty sp_LibStyle_Effect_color = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_color");
            SerializedProperty sp_LibStyle_Effect_rich = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_rich");
            SerializedProperty sp_LibStyle_Effect_wrap = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_wrap");
            SerializedProperty sp_LibStyle_Effect_wrapratio = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_wrapratio");
            SerializedProperty sp_LibStyle_Effect_overflow = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_overflow");
            SerializedProperty sp_LibStyle_Effect_margin = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_margin");
            SerializedProperty sp_LibStyle_Effect_space = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_space");
            SerializedProperty sp_LibStyle_Effect_gradientcolor = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_gradientcolor");
            SerializedProperty sp_LibStyle_Effect_autosizing = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_autosizing");
            SerializedProperty sp_LibStyle_Effect_raycast = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_raycast");
            SerializedProperty sp_LibStyle_Effect_maskable = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_maskable");
            //--------------------------------- Features ---------------------------------//
            SerializedProperty sp_Raycast = sp_TextStyleInfo.FindPropertyRelative("Raycast");
            SerializedProperty sp_Maskable = sp_TextStyleInfo.FindPropertyRelative("Maskable");
            SerializedProperty sp_SyncAnimatorColor = sp_TextStyleInfo.FindPropertyRelative("SyncAnimatorColor");
            #endregion

            #region 标题
            string titlename = "";
            if (string.IsNullOrEmpty(sp_Indicator.stringValue))
                titlename = "Hud - Tmp文字";
            else
                titlename = sp_Indicator.stringValue;
            Editor_XHud_GUI.Gui_Layout_Banner(icon_main, HudFilled.实体, HudColor.深空灰, titlename, Color.white);
            #endregion

            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

            #region 按钮

            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "快捷功能", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            if (!IsMultiSelected())
            {
                Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_XHud_GUI.Gui_Layout_Space(10);

                if (mgr.Hud_TextStyleLibrary)
                {
                    #region 保存到字体库
                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "将当前样式保存到字体库", save_r, save_p))
                    {
                        if (Application.isPlaying)
                            return;

                        ///---新建文字模版信息类
                        XHud_LibraryArg_TextStyle info = new XHud_LibraryArg_TextStyle();
                        info.Type = xHud_TextType.TmpText;
                        info.tmp_rich = sp_tmp_rich.boolValue;
                        info.tmp_contentwrap = (TextWrappingModes)sp_tmp_contentwrap.enumValueIndex;
                        info.tmp_overflow = (TextOverflowModes)sp_tmp_overflow.enumValueIndex;
                        info.tmp_style = (FontStyles)sp_tmp_style.enumValueIndex;
                        info.tmp_anchor = (TmpContentAnchor)sp_tmp_anchor.enumValueIndex;
                        info.tmp_font = sp_tmp_font.objectReferenceValue as TMP_FontAsset;
                        info.tmp_size = sp_tmp_size.floatValue;
                        info.tmp_space_character = sp_tmp_space_character.floatValue;
                        info.tmp_space_word = sp_tmp_space_word.floatValue;
                        info.tmp_space_lineheight = sp_tmp_space_lineheight.floatValue;
                        info.tmp_space_paragraph = sp_tmp_space_paragraph.floatValue;
                        info.tmp_WrappingRatios = sp_tmp_WrappingRatios.floatValue;
                        info.tmp_contentmargin = sp_tmp_contentmargin.vector4Value;
                        info.tmp_color = sp_tmp_color.colorValue;
                        info.tmp_EnableAutoSizing = sp_tmp_EnableAutoSizing.boolValue;
                        info.tmp_FontSizeMin = sp_tmp_FontSizeMin.floatValue;
                        info.tmp_FontSizeMax = sp_tmp_FontSizeMax.floatValue;
                        info.tmp_CharWidthMaxAdj = sp_tmp_CharWidthMaxAdj.floatValue;
                        info.tmp_LineSpacingMax = sp_tmp_LineSpacingMax.floatValue;
                        info.gra_A = sp_gra_A.colorValue;
                        info.gra_B = sp_gra_B.colorValue;
                        info.gra_C = sp_gra_C.colorValue;
                        info.gra_D = sp_gra_D.colorValue;
                        info.gra_Invert = sp_gra_Invert.boolValue;
                        info.gra_Used = sp_gra_Used.boolValue;
                        info.gra_ColorMode = (ColorMode)sp_gra_ColorMode.enumValueIndex;
                        info.gra_ColorModeName = sp_gra_ColorModeName.stringValue;
                        info.Raycast = sp_Raycast.boolValue;
                        info.Maskable = sp_Maskable.boolValue;
                        info.SyncAnimatorColor = sp_SyncAnimatorColor.boolValue;

                        Open_Hud_Library_TextStyle_Setter(info);
                    }
                    #endregion

                    GUILayout.FlexibleSpace();

                    #region 查看字体库
                    GUI.enabled = true;
                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "查看当前字体库", openlib_r, openlib_p))
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
                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "根据选择的快速样式刷新文字样式", update_r, update_p))
                    {
                        UpdateTextStyle_WithTarget();
                        return;
                    }
                    #endregion
                }

                Editor_XHud_GUI.Gui_Layout_Space(10);
                Editor_XHud_GUI.Gui_Layout_Horizontal_End();
            }
            else
            {
                Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_XHud_GUI.Gui_Layout_Space(10);
                GUILayout.FlexibleSpace();

                if (mgr.Hud_TextStyleLibrary)
                    #region 批量刷新样式
                    if (!mgr.Hud_TextStyleLibrary.TextStyle_Library_IsEmpty())
                    {
                        if (Editor_XHud_GUI.Gui_Layout_Button(14, "根据选择的快速样式刷新文字样式", update_r, update_p))
                        {
                            UpdateTextStyle_WithTarget();
                            return;
                        }
                    }
                #endregion

                GUILayout.FlexibleSpace();
                Editor_XHud_GUI.Gui_Layout_Space(10);
                Editor_XHud_GUI.Gui_Layout_Horizontal_End();
            }

            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();

            #endregion

            #region 选项
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "选项", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            #region 射线检测可用性   
            if (!sp_LibStyle_Effect_raycast.boolValue || !sp_StyleLibSynching.boolValue)
                Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_TmpText>("射线检测", stroptions_enabled, ref sp_Raycast, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
            #endregion

            #region 遮罩 
            if (!sp_LibStyle_Effect_maskable.boolValue || !sp_StyleLibSynching.boolValue)
                Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_TmpText>("遮罩", stroptions_enabled, ref sp_Maskable, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
            #endregion

            #region 富文本支持           
            if (!sp_LibStyle_Effect_rich.boolValue || !sp_StyleLibSynching.boolValue)
                Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_TmpText>("富文本", stroptions_enabled, ref sp_tmp_rich, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
            #endregion

            #region 自动尺寸         
            if (!sp_LibStyle_Effect_autosizing.boolValue || !sp_StyleLibSynching.boolValue)
                Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_TmpText>("自动尺寸", stroptions_enabled, ref sp_tmp_EnableAutoSizing, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
            #endregion

            #region 尺寸全局受控               
            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_TmpText>("全局尺寸增量", stroptions_syncsize, ref sp_SyncGlobalFontSize, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
            #endregion

            #region Animator接管字体颜色        
            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_TmpText>("动画器接管颜色", stroptions_synccolor, ref sp_SyncAnimatorColor, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
            #endregion

            #region 渐变色支持      
            if (!sp_LibStyle_Effect_gradientcolor.boolValue || !sp_StyleLibSynching.boolValue)
                Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_TmpText>("渐变色支持", stroptions_enabled, ref sp_gra_Used, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
            #endregion

            #region 渐变色模式
            if (sp_gra_Used.boolValue && !sp_LibStyle_Effect_gradientcolor.boolValue || !sp_StyleLibSynching.boolValue)
            {
                Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_Module_TmpText>("渐变色模式", stroptions_gramode, ref sp_gra_ColorModeName, HudFilled.实体, 120, 22, SelectedObjects);

                if (IsMultiSelected())
                {
                    for (int i = 0; i < SelectedObjects.Length; i++)
                    {
                        switch (SelectedObjects[i].TextStyleInfo.gra_ColorModeName)
                        {
                            case "单色":
                                SelectedObjects[i].TextStyleInfo.gra_ColorMode = ColorMode.Single;
                                break;
                            case "水平渐变":
                                SelectedObjects[i].TextStyleInfo.gra_ColorMode = ColorMode.HorizontalGradient;
                                break;
                            case "垂直渐变":
                                SelectedObjects[i].TextStyleInfo.gra_ColorMode = ColorMode.VerticalGradient;
                                break;
                            case "四角渐变":
                                SelectedObjects[i].TextStyleInfo.gra_ColorMode = ColorMode.FourCornersGradient;
                                break;
                        }
                    }
                }
                else
                {
                    switch (sp_gra_ColorModeName.stringValue)
                    {
                        case "单色":
                            sp_gra_ColorMode.enumValueIndex = (int)ColorMode.Single;
                            break;
                        case "水平渐变":
                            sp_gra_ColorMode.enumValueIndex = (int)ColorMode.HorizontalGradient;
                            break;
                        case "垂直渐变":
                            sp_gra_ColorMode.enumValueIndex = (int)ColorMode.VerticalGradient;
                            break;
                        case "四角渐变":
                            sp_gra_ColorMode.enumValueIndex = (int)ColorMode.FourCornersGradient;
                            break;
                    }
                }

            }
            #endregion

            #region 渐变色方向
            if (sp_gra_Used.boolValue && !sp_LibStyle_Effect_gradientcolor.boolValue || !sp_StyleLibSynching.boolValue)
            {
                Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_TmpText>("渐变色方向", stroptions_flip, ref sp_gra_Invert, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
            }
            #endregion

            #region 实时匹配库
            if (mgr != null && mgr.Hud_TextStyleLibrary != null)
            {
                if (!mgr.Hud_TextStyleLibrary.TextStyle_Library_IsEmpty())
                    Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_TmpText>("实时匹配库", stroptions_enabled, ref sp_StyleLibSynching, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                else
                {
                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Space(10);
                    EditorGUILayout.HelpBox("字体样式库为空", MessageType.Warning);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                }
            }
            else
            {
                sp_StyleLibSynching.boolValue = false;
                sp_StyleLibSynching.serializedObject.ApplyModifiedProperties();
                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_XHud_GUI.Gui_Layout_Space(10);
                EditorGUILayout.HelpBox("您未在Hud管理器中配置或指定字体样式库", MessageType.Warning);
                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Horizontal_End();
            }
            #endregion

            #region 文字样式
            if (mgr != null && mgr.Hud_TextStyleLibrary != null)
            {
                if (mgr.Hud_TextStyleLibrary.TextStyle_Library_GetCount(xHud_TextType.TmpText) > 0)
                {
                    string[] str_fotlib_ItemsName = mgr.Hud_TextStyleLibrary.TextStyle_Library_GetAllNames_With_TmpText();
                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_Module_TmpText>("文字样式库", str_fotlib_ItemsName, ref sp_StyleName, HudFilled.实体, 94, 22, SelectedObjects, (comps) =>
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
                        Editor_XHud_GUI.Gui_Layout_Space(10);

                        if (Editor_XHud_GUI.Gui_Layout_Button(14, "定位到样式库", locate_r, locate_p, 2))
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
                            Editor_MenuItemsAction_OpenLibrary.open_font();
                            mgr.Hud_TextStyleLibrary.TextStyleLibrary_Location(str_fotlib_ItemsName[index]);
                        }
                        //util_EditorGuiLib.Gui_Layout_Space(5);
                    }
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                }
            }
            #endregion

            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            Rect rect1 = Editor_XHud_GUI.Gui_GetLastRect();

            #region 库样式同步选项
            if (sp_StyleLibSynching.boolValue)
            {
                Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "库样式同步选项", XHud_Dashboard.Theme_Primary);
                Editor_XHud_GUI.Gui_Layout_Space(10);

                #region 射线检测
                Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_TmpText>("射线检测", stroptions_syncstyle, ref sp_LibStyle_Effect_raycast, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                #endregion

                #region 遮罩
                Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_TmpText>("遮罩", stroptions_syncstyle, ref sp_LibStyle_Effect_maskable, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                #endregion

                #region 富文本
                Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_TmpText>("富文本", stroptions_syncstyle, ref sp_LibStyle_Effect_rich, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                #endregion

                #region 尺寸
                Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_TmpText>("尺寸", stroptions_syncstyle, ref sp_LibStyle_Effect_size, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                if (sp_LibStyle_Effect_size.boolValue)
                {
                    sp_LibStyle_Effect_autosizing.boolValue = false;
                    sp_LibStyle_Effect_autosizing.serializedObject.ApplyModifiedProperties();
                    sp_tmp_EnableAutoSizing.boolValue = false;
                    sp_tmp_EnableAutoSizing.serializedObject.ApplyModifiedProperties();
                }
                #endregion

                #region 自适应尺寸
                Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_TmpText>("自适应尺寸", stroptions_syncstyle, ref sp_LibStyle_Effect_autosizing, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                if (sp_LibStyle_Effect_autosizing.boolValue)
                {
                    sp_LibStyle_Effect_size.boolValue = false;
                    sp_LibStyle_Effect_size.serializedObject.ApplyModifiedProperties();
                    sp_tmp_EnableAutoSizing.boolValue = true;
                    sp_tmp_EnableAutoSizing.serializedObject.ApplyModifiedProperties();
                }
                #endregion

                #region 渐变色
                Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_TmpText>("渐变色", stroptions_syncstyle, ref sp_LibStyle_Effect_gradientcolor, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                #endregion

                #region 对齐
                Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_TmpText>("对齐", stroptions_syncstyle, ref sp_LibStyle_Effect_align, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                #endregion

                #region 字体
                Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_TmpText>("字体", stroptions_syncstyle, ref sp_LibStyle_Effect_font, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                #endregion

                #region 样式
                Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_TmpText>("样式", stroptions_syncstyle, ref sp_LibStyle_Effect_style, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                #endregion

                #region 颜色
                Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_TmpText>("颜色", stroptions_syncstyle, ref sp_LibStyle_Effect_color, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                #endregion

                #region 文本包裹
                Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_TmpText>("包裹", stroptions_syncstyle, ref sp_LibStyle_Effect_wrap, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                #endregion

                #region 溢出
                Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_TmpText>("溢出", stroptions_syncstyle, ref sp_LibStyle_Effect_overflow, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                #endregion

                #region 包裹比例
                Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_TmpText>("包裹比例", stroptions_syncstyle, ref sp_LibStyle_Effect_wrapratio, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                #endregion

                #region 边距
                Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_TmpText>("边距", stroptions_syncstyle, ref sp_LibStyle_Effect_margin, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                #endregion

                #region 间距
                Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_TmpText>("间距", stroptions_syncstyle, ref sp_LibStyle_Effect_space, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
                #endregion

                Editor_XHud_GUI.Gui_Layout_Space(10);
                Editor_XHud_GUI.Gui_Layout_Vertical_End();
            }
            #endregion

            #region 渐变色控件
            if (sp_gra_Used.boolValue)
            {
                Editor_XHud_GUI.Gui_Layout_Space(0);
                #region 参数
                Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "渐变色", XHud_Dashboard.Theme_Primary);

                Editor_XHud_GUI.Gui_Layout_Space(10);


                CreateGradientTexture((ColorMode)sp_gra_ColorMode.enumValueIndex, sp_gra_Invert.boolValue, sp_gra_A.colorValue, sp_gra_B.colorValue, sp_gra_C.colorValue, sp_gra_D.colorValue);
                Rect rect2 = Editor_XHud_GUI.Gui_GetLastRect();

                Rect rect_GradientIcon;

                if (sp_gra_ColorModeName.stringValue == "单色")
                {
                    rect_GradientIcon = new Rect((rect1.width / 2) - 12, rect2.y + 15, 60, 60);
                }
                else
                {
                    rect_GradientIcon = new Rect((rect1.width / 2) - 12, rect2.y + 35, 60, 60);
                }


                Editor_XHud_GUI.Gui_Icon(rect_GradientIcon, GradientTexture);


                switch (sp_gra_ColorModeName.stringValue)
                {
                    case "单色":
                        Editor_XHud_GUI.Gui_ColorField(new Rect(rect_GradientIcon.x, rect_GradientIcon.y + 80, 61, 20), sp_gra_A);
                        Editor_XHud_GUI.Gui_Layout_Space(120);
                        break;
                    case "水平渐变":
                        if (sp_gra_Invert.boolValue)
                        {
                            Editor_XHud_GUI.Gui_ColorField(new Rect(rect1.x + 25, rect_GradientIcon.y + 20, 61, 20), sp_gra_B);
                            Editor_XHud_GUI.Gui_ColorField(new Rect(rect1.width - 68, rect_GradientIcon.y + 20, 61, 20), sp_gra_A);
                        }
                        else
                        {
                            Editor_XHud_GUI.Gui_ColorField(new Rect(rect1.x + 25, rect_GradientIcon.y + 20, 61, 20), sp_gra_A);
                            Editor_XHud_GUI.Gui_ColorField(new Rect(rect1.width - 68, rect_GradientIcon.y + 20, 61, 20), sp_gra_B);
                        }
                        Editor_XHud_GUI.Gui_Layout_Space(120);
                        break;
                    case "垂直渐变":
                        if (sp_gra_Invert.boolValue)
                        {
                            Editor_XHud_GUI.Gui_ColorField(new Rect(rect_GradientIcon.x, rect_GradientIcon.y + 72, 61, 20), sp_gra_B);
                            Editor_XHud_GUI.Gui_ColorField(new Rect(rect_GradientIcon.x, rect_GradientIcon.y - 32, 61, 20), sp_gra_A);
                        }
                        else
                        {
                            Editor_XHud_GUI.Gui_ColorField(new Rect(rect_GradientIcon.x, rect_GradientIcon.y + 72, 61, 20), sp_gra_A);
                            Editor_XHud_GUI.Gui_ColorField(new Rect(rect_GradientIcon.x, rect_GradientIcon.y - 32, 61, 20), sp_gra_B);
                        }
                        Editor_XHud_GUI.Gui_Layout_Space(120);
                        break;
                    case "四角渐变":
                        if (sp_gra_Invert.boolValue)
                        {
                            Editor_XHud_GUI.Gui_ColorField(new Rect(rect1.x + 25, rect_GradientIcon.y - 20, 61, 20), sp_gra_B);
                            Editor_XHud_GUI.Gui_ColorField(new Rect(rect1.width - 68, rect_GradientIcon.y - 20, 61, 20), sp_gra_A);
                            Editor_XHud_GUI.Gui_ColorField(new Rect(rect1.x + 25, rect_GradientIcon.y + 60, 61, 20), sp_gra_D);
                            Editor_XHud_GUI.Gui_ColorField(new Rect(rect1.width - 68, rect_GradientIcon.y + 60, 61, 20), sp_gra_C);
                        }
                        else
                        {
                            Editor_XHud_GUI.Gui_ColorField(new Rect(rect1.x + 25, rect_GradientIcon.y - 20, 61, 20), sp_gra_A);
                            Editor_XHud_GUI.Gui_ColorField(new Rect(rect1.width - 68, rect_GradientIcon.y - 20, 61, 20), sp_gra_B);
                            Editor_XHud_GUI.Gui_ColorField(new Rect(rect1.x + 25, rect_GradientIcon.y + 60, 61, 20), sp_gra_C);
                            Editor_XHud_GUI.Gui_ColorField(new Rect(rect1.width - 68, rect_GradientIcon.y + 60, 61, 20), sp_gra_D);
                        }
                        Editor_XHud_GUI.Gui_Layout_Space(120);
                        break;
                }

                sp_gra_A.serializedObject.ApplyModifiedProperties();
                sp_gra_B.serializedObject.ApplyModifiedProperties();
                sp_gra_C.serializedObject.ApplyModifiedProperties();
                sp_gra_D.serializedObject.ApplyModifiedProperties();

                Editor_XHud_GUI.Gui_Layout_Space(10);
                Editor_XHud_GUI.Gui_Layout_Vertical_End();
                #endregion
            }
            #endregion

            #region 参数
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "参数", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            #region 内容
            EditorGUI.BeginChangeCheck();
            Editor_XHud_GUI.Gui_Layout_Property_Field("文字内容", sp_Text);
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
            Editor_XHud_GUI.Gui_Layout_Space(10);

            #endregion

            Editor_XHud_GUI.Gui_Layout_Space(5);

            #region 标识
            EditorGUI.BeginChangeCheck();
            Editor_XHud_GUI.Gui_Layout_Property_Field("标识", sp_Indicator);
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

            Editor_XHud_GUI.Gui_Layout_Space(5);

            #region 字体资源
            if (!sp_LibStyle_Effect_font.boolValue || !sp_StyleLibSynching.boolValue)
            {
                Editor_XHud_GUI.Gui_Layout_Property_Field("字体", sp_tmp_font);
                Editor_XHud_GUI.Gui_Layout_Space(5);
            }
            #endregion

            #region 内容锚点 
            if (!sp_LibStyle_Effect_align.boolValue || !sp_StyleLibSynching.boolValue)
            {
                EditorGUI.BeginChangeCheck();
                Editor_XHud_GUI.Gui_Layout_Property_Field("锚点", sp_tmp_anchor);
                if (EditorGUI.EndChangeCheck())
                {
                    if (IsMultiSelected())
                    {
                        for (int i = 0; i < SelectedObjects.Length; i++)
                        {
                            TmpContentAnchor anchor = (TmpContentAnchor)sp_tmp_anchor.enumValueIndex;
                            SelectedObjects[i].TextStyleInfo.tmp_Set_Alignment(anchor);
                        }
                    }
                    else
                    {
                        TmpContentAnchor anchor = (TmpContentAnchor)sp_tmp_anchor.enumValueIndex;
                        BaseScript.TextStyleInfo.tmp_Set_Alignment(anchor);
                    }
                }

                #region 均分

                TmpContentAnchor anchors = (TmpContentAnchor)sp_tmp_anchor.enumValueIndex;
                if (anchors == TmpContentAnchor.中心左右均分 || anchors == TmpContentAnchor.中线左右均分 || anchors == TmpContentAnchor.基线左右均分 || anchors == TmpContentAnchor.底部左右均分 || anchors == TmpContentAnchor.顶部均分 || anchors == TmpContentAnchor.中心填充 || anchors == TmpContentAnchor.中线左右填充 || anchors == TmpContentAnchor.基线左右填充 || anchors == TmpContentAnchor.底部左右填充 || anchors == TmpContentAnchor.顶部填充)
                {
                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    #region 均分对齐 
                    Editor_XHud_GUI.Gui_Layout_Property_Field("均分", sp_tmp_WrappingRatios);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    #endregion
                }
                #endregion
                Editor_XHud_GUI.Gui_Layout_Space(5);
            }
            #endregion

            #region 字体大小 
            if (!sp_tmp_EnableAutoSizing.boolValue)
            {
                if (!sp_LibStyle_Effect_size.boolValue || !sp_StyleLibSynching.boolValue)
                {
                    Editor_XHud_GUI.Gui_Layout_Property_Field("尺寸", sp_tmp_size);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                }
            }
            #endregion

            #region 自适应字体大小 
            if (sp_tmp_EnableAutoSizing.boolValue)
            {
                if (!sp_LibStyle_Effect_autosizing.boolValue || !sp_StyleLibSynching.boolValue)
                {
                    Editor_XHud_GUI.Gui_Layout_Property_Field("最小尺寸", sp_tmp_FontSizeMin);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Property_Field("最大尺寸", sp_tmp_FontSizeMax);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Property_Field("字符宽度", sp_tmp_CharWidthMaxAdj);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Property_Field("最大行高", sp_tmp_LineSpacingMax);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                }
            }
            #endregion

            #region 包裹开关
            if (!sp_LibStyle_Effect_wrap.boolValue || !sp_StyleLibSynching.boolValue)
            {
                Editor_XHud_GUI.Gui_Layout_Property_Field("包裹", sp_tmp_contentwrap);
                Editor_XHud_GUI.Gui_Layout_Space(5);
            }
            #endregion

            #region 溢出模式
            if (!sp_LibStyle_Effect_overflow.boolValue || !sp_StyleLibSynching.boolValue)
            {
                Editor_XHud_GUI.Gui_Layout_Property_Field("溢出", sp_tmp_overflow);
                Editor_XHud_GUI.Gui_Layout_Space(5);
            }
            #endregion

            #region 字体样式
            if (!sp_LibStyle_Effect_style.boolValue || !sp_StyleLibSynching.boolValue)
            {
                Editor_XHud_GUI.Gui_Layout_Property_Field("样式", sp_tmp_style);
                Editor_XHud_GUI.Gui_Layout_Space(5);
            }
            #endregion

            #region 间距
            if (!sp_LibStyle_Effect_space.boolValue || !sp_StyleLibSynching.boolValue)
            {
                #region 字符间距 
                Editor_XHud_GUI.Gui_Layout_Property_Field("字符", sp_tmp_space_character);
                #endregion

                Editor_XHud_GUI.Gui_Layout_Space(5);

                #region 单词间距 
                Editor_XHud_GUI.Gui_Layout_Property_Field("单词", sp_tmp_space_word);
                #endregion

                Editor_XHud_GUI.Gui_Layout_Space(5);

                #region 行高 
                Editor_XHud_GUI.Gui_Layout_Property_Field("行高", sp_tmp_space_lineheight);
                #endregion

                Editor_XHud_GUI.Gui_Layout_Space(5);

                #region 段落间距 
                Editor_XHud_GUI.Gui_Layout_Property_Field("段落", sp_tmp_space_paragraph);
                #endregion

                Editor_XHud_GUI.Gui_Layout_Space(5);
            }
            #endregion

            #region 内容边距 
            if (!sp_LibStyle_Effect_margin.boolValue || !sp_StyleLibSynching.boolValue)
            {
                Editor_XHud_GUI.Gui_Layout_Property_Field("边距", sp_tmp_contentmargin);
                Editor_XHud_GUI.Gui_Layout_Space(5);
            }
            #endregion

            #region 颜色 
            if (!sp_LibStyle_Effect_color.boolValue || !sp_StyleLibSynching.boolValue)
            {
                Editor_XHud_GUI.Gui_Layout_Property_Field("颜色", sp_tmp_color);
            }
            #endregion

            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            Event e = Event.current;
            if (e.type == EventType.MouseDown && e.button == 1)
            {
                // 创建右键菜单
                GenericMenu menu = new GenericMenu();
                menu.AddDisabledItem(new GUIContent("组件样式操作"));
                menu.AddItem(new GUIContent("C (复制样式)"), false, () =>
                {
                    XHud_LibraryArg_TextStyle info = new XHud_LibraryArg_TextStyle();
                    info.CopyData_Ignored_LibraryToggle(BaseScript.TextStyleInfo);

                    string json = JsonUtility.ToJson(info);

                    Editor_XHud_GUI.EditorData_Set_With_String("XED_HudTmpText_Get_TextStyle", json);

                    Editor_XHud_GUI.Open(XHud_DialogType.确认, "HudTmpText 文字组件消息", "文字样式复制", $"已将当前文字样式存入 XHudEditorData (XED)！", "好的");
                });
                menu.AddItem(new GUIContent("V (粘贴样式)"), false, () =>
                {
                    XHud_LibraryArg_TextStyle info = JsonUtility.FromJson<XHud_LibraryArg_TextStyle>(Editor_XHud_GUI.EditorData_Get_With_String("XED_HudTmpText_Get_TextStyle"));
                    BaseScript.TextStyleInfo.CopyData(info);

                    if (IsMultiSelected())
                    {
                        for (int i = 0; i < SelectedObjects.Length; i++)
                        {
                            // 使用 Undo.RecordObject 来记录对目标对象的修改
                            Undo.RecordObject(SelectedObjects[i], "Paste Style");
                            SelectedObjects[i].TextStyleInfo = info;
                        }
                    }
                    else
                    {
                        // 使用 Undo.RecordObject 来记录对目标对象的修改
                        Undo.RecordObject(BaseScript, "Paste Style");
                        BaseScript.TextStyleInfo = info;

                        Editor_XHud_GUI.Open(XHud_DialogType.确认, "HudTmpText 文字组件消息", "文字样式粘贴", $"已从 XHudEditorData (XED) 中获取文字样式并应用到当前文字组件！", "好的");
                    }
                });
                menu.AddSeparator("");
                menu.AddDisabledItem(new GUIContent("文字样式库"));
                menu.AddItem(new GUIContent("X (识别)"), false, () =>
                {
                    XHud_LibraryArg_TextStyle info = JsonUtility.FromJson<XHud_LibraryArg_TextStyle>(Editor_XHud_GUI.EditorData_Get_With_String("XED_HudTextStyleLibrary_Get_TextStyle"));

                    if (IsMultiSelected())
                    {
                        List<XHud_GUI_Dialog_ListDatas> datas = new List<XHud_GUI_Dialog_ListDatas>();

                        for (int i = 0; i < SelectedObjects.Length; i++)
                        {
                            XHud_GUI_Dialog_ListDatas data = new XHud_GUI_Dialog_ListDatas();

                            if (info.Type != xHud_TextType.TmpText)
                            {
                                Editor_XHud_GUI.Open(XHud_DialogType.错误, "HudTmpText 文字组件消息", "文字样式识别", $"从 XHudEditorData (XED) 中识别的文字样式似乎并不是用于 {SelectedObjects[i].name} ( {SelectedObjects[i].Indicator} ) 的文字组件的规范样式！请检查您复制的样式类型是否和当前组件一致？", "好的");

                                data.Title = info.Name;
                                data.SubTitle = "文字样式类型未能匹配到";
                                data.Message = $"{SelectedObjects[i].name} ( {SelectedObjects[i].Indicator} )";

                                datas.Add(data);
                                continue;
                            }
                            else
                            {

                                // 使用 Undo.RecordObject 来记录对目标对象的修改
                                Undo.RecordObject(SelectedObjects[i], "Paste Style");

                                SelectedObjects[i].TextStyleInfo.CopyData_Ignored_LibraryToggle(info);

                                data.Title = info.Name;
                                data.SubTitle = "文字样式已应用到";
                                data.Message = $"{SelectedObjects[i].name} ( {SelectedObjects[i].Indicator} )";
                                datas.Add(data);
                            }
                        }

                        Editor_XHud_GUI.Open(datas.ToArray(), XHud_DialogType.确认, "HudTmpText 文字组件消息", "文字样式识别", $"已从 XHudEditorData (XED) 中识别到文字样式并应用到如下文字组件中！", "好的");
                    }
                    else
                    {
                        if (info.Type != xHud_TextType.TmpText)
                        {
                            Editor_XHud_GUI.Open(XHud_DialogType.错误, "HudTmpText 文字组件消息", "文字样式识别", $"从 XHudEditorData (XED) 中识别的文字样式似乎并不是用于当前文字组件的规范样式！请检查您复制的样式类型是否和当前组件一致？", "好的");
                            return;
                        }

                        // 使用 Undo.RecordObject 来记录对目标对象的修改
                        Undo.RecordObject(BaseScript, "Paste Style");

                        BaseScript.TextStyleInfo.CopyData_Ignored_LibraryToggle(info);

                        Editor_XHud_GUI.Open(XHud_DialogType.确认, "HudTmpText 文字组件消息", "文字样式识别", $"已从 XHudEditorData (XED) 中识别到文字样式并应用到当前文字组件！", "好的");
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
                    sp_LibStyle_Effect_wrap.boolValue = true;
                    sp_LibStyle_Effect_wrapratio.boolValue = true;
                    sp_LibStyle_Effect_overflow.boolValue = true;
                    sp_LibStyle_Effect_margin.boolValue = true;
                    sp_LibStyle_Effect_space.boolValue = true;
                    sp_LibStyle_Effect_gradientcolor.boolValue = true;
                    sp_LibStyle_Effect_autosizing.boolValue = true;
                    sp_LibStyle_Effect_raycast.boolValue = true;
                    sp_LibStyle_Effect_maskable.boolValue = true;

                    sp_LibStyle_Effect_align.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_font.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_style.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_size.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_color.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_rich.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_wrap.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_wrapratio.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_overflow.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_margin.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_space.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_gradientcolor.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_autosizing.serializedObject.ApplyModifiedProperties();
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
                    sp_LibStyle_Effect_wrap.boolValue = false;
                    sp_LibStyle_Effect_wrapratio.boolValue = false;
                    sp_LibStyle_Effect_overflow.boolValue = false;
                    sp_LibStyle_Effect_margin.boolValue = false;
                    sp_LibStyle_Effect_space.boolValue = false;
                    sp_LibStyle_Effect_gradientcolor.boolValue = false;
                    sp_LibStyle_Effect_autosizing.boolValue = false;
                    sp_LibStyle_Effect_raycast.boolValue = false;
                    sp_LibStyle_Effect_maskable.boolValue = false;

                    sp_LibStyle_Effect_align.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_font.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_style.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_size.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_color.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_rich.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_wrap.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_wrapratio.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_overflow.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_margin.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_space.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_gradientcolor.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_autosizing.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_raycast.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_maskable.serializedObject.ApplyModifiedProperties();
                });
                // 显示右键菜单
                menu.ShowAsContext();

                e.Use(); // 标记事件已被处理，防止其他操作处理该事件
            }

            #region 源脚本
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.阴影灰, 3, "源脚本", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(5);

            #region 原始变量
            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_XHud_GUI.Gui_Layout_Space(10);
            BasicVars = EditorGUILayout.Foldout(BasicVars, "变量/属性", true);
            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Horizontal_End();
            if (BasicVars)
            {
                DrawDefaultInspector();
            }
            #endregion

            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            serializedObject.ApplyModifiedProperties();
        }

        #region 辅助
        /// <summary>
        /// 创建渐变贴图的方法
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="invert"></param>
        /// <param name="col_start"></param>
        /// <param name="col_end"></param>
        /// <param name="col_start_cor"></param>
        /// <param name="col_end_cor"></param>
        private void CreateGradientTexture(ColorMode mode, bool invert, Color col_start, Color col_end, Color col_start_cor, Color col_end_cor)
        {
            // 创建一个新的Texture2D对象
            GradientTexture = new Texture2D(gra_TexWidth, gra_TexHeight);
            Color pixelColor = Color.white;
            Color verticalBlend = Color.white;
            Color horizontalBlend = Color.white;
            Color colorBlend = Color.white;

            switch (mode)
            {
                case ColorMode.Single:
                    for (int x = 0; x < gra_TexWidth; x++)
                    {
                        for (int y = 0; y < gra_TexHeight; y++)
                        {
                            pixelColor = col_start;
                            GradientTexture.SetPixel(x, y, pixelColor);
                        }
                    }
                    break;
                case ColorMode.HorizontalGradient:
                    // 遍历每个像素并设置颜色
                    for (int x = 0; x < gra_TexWidth; x++)
                    {
                        for (int y = 0; y < gra_TexHeight; y++)
                        {
                            // 计算水平渐变的插值
                            float gradientValue = (float)x / (gra_TexWidth - 1);

                            if (invert)
                                pixelColor = Color.Lerp(col_end, col_start, gradientValue);
                            else
                                pixelColor = Color.Lerp(col_start, col_end, gradientValue);

                            GradientTexture.SetPixel(x, y, pixelColor);
                        }
                    }
                    break;
                case ColorMode.VerticalGradient:
                    for (int y = 0; y < gra_TexHeight; y++)
                    {
                        for (int x = 0; x < gra_TexWidth; x++)
                        {
                            // 计算垂直渐变的插值
                            float gradientValue = (float)y / (gra_TexHeight - 1);
                            if (invert)
                                pixelColor = Color.Lerp(col_end, col_start, gradientValue);
                            else
                                pixelColor = Color.Lerp(col_start, col_end, gradientValue);
                            GradientTexture.SetPixel(x, y, pixelColor);
                        }
                    }
                    break;
                case ColorMode.FourCornersGradient:
                    for (int y = 0; y < gra_TexHeight; y++)
                    {
                        for (int x = 0; x < gra_TexWidth; x++)
                        {
                            // 归一化坐标
                            float u = x / (float)(gra_TexWidth - 1);
                            float v = y / (float)(gra_TexHeight - 1);

                            // 计算四个角的颜色插值
                            if (invert)
                            {
                                verticalBlend = Color.Lerp(col_end, col_start, u);        // 顶部水平插值
                                horizontalBlend = Color.Lerp(col_end_cor, col_start_cor, u); // 底部水平插值
                                colorBlend = Color.Lerp(horizontalBlend, verticalBlend, v);       // 垂直插值  
                            }
                            else
                            {
                                verticalBlend = Color.Lerp(col_start, col_end, u);        // 顶部水平插值
                                horizontalBlend = Color.Lerp(col_start_cor, col_end_cor, u); // 底部水平插值
                                colorBlend = Color.Lerp(horizontalBlend, verticalBlend, v);       // 垂直插值
                            }
                            // 计算当前像素到中心的距离（归一化）
                            float distanceToCenter = Mathf.Sqrt((u - 0.5f) * (u - 0.5f) + (v - 0.5f) * (v - 0.5f));
                            float normalizedDistance = 1.0f - (distanceToCenter * 2); // 从外到内，值从1到0

                            // 根据距离中心的距离调整颜色
                            Color finalColor = Color.Lerp(colorBlend, Color.white, normalizedDistance); // 中心颜色为白色

                            GradientTexture.SetPixel(x, y, finalColor);
                        }
                    }
                    break;
            }

            // 应用修改并压缩贴图
            GradientTexture.Apply();
        }

        /// <summary>
        /// 根据文字样式名称索引刷新文字样式
        /// </summary>
        private void UpdateTextStyle_WithTarget()
        {
            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

            XHud_LibraryArg_TextStyle s_info = null;
            if (IsMultiSelected())
            {
                var s = targets;
                foreach (var t in s)
                {
                    XHud_Module_TmpText tt = (XHud_Module_TmpText)t;

                    if (Application.isPlaying)
                    {
                        if (mgr.Hud_TextStyleLibrary.TextStyle_Library_NameIsValid(tt.StyleName))
                        {
                            s_info = mgr.Hud_TextStyleLibrary.TextStyle_Library_GetTextStyleInfo(tt.StyleName, xHud_TextType.TmpText);
                        }
                        else
                        {
                            string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "HudText 文字组件消息", "刷新文字库样式", "似乎 {tt.name} ( {tt.Indicator} ) 文字组件的样式库名称并不在文字样式库中存在，请检查文字样式库名称是否合法，或者可以快速选择文字样式库默认首个样式。是否要选择首个样式？", "暂不", "选择", 0);
                            if (res == "选择")
                            {
                                s_info = mgr.Hud_TextStyleLibrary.TextStyle_Library_GetFirstStyleInfo_With_Type(xHud_TextType.TmpText);

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
                            s_info = mgr.Hud_TextStyleLibrary.TextStyle_Library_GetTextStyleInfo(tt.StyleName, xHud_TextType.TmpText);
                        }
                        else
                        {
                            string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "HudText 文字组件消息", "刷新文字库样式", $"似乎 {tt.name} ( {tt.Indicator} ) 文字组件指定的样式库名称并不在文字样式库中存在，请检查文字样式库名称是否合法，或者可以快速选择文字样式库默认首个样式。是否要选择首个样式？", "暂不", "选择", 0);
                            if (res == "选择")
                            {
                                s_info = mgr.Hud_TextStyleLibrary.TextStyle_Library_GetFirstStyleInfo_With_Type(xHud_TextType.TmpText);

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
                        s_info = mgr.Hud_TextStyleLibrary.TextStyle_Library_GetTextStyleInfo(sp_StyleName.stringValue, xHud_TextType.TmpText);
                    else
                    {
                        string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "HudText 文字组件消息", "刷新文字库样式", "似乎您指定的样式库名称并不在文字样式库中存在，请检查文字样式库名称是否合法，或者可以快速选择文字样式库默认首个样式。是否要选择首个样式？", "暂不", "选择", 0);
                        if (res == "选择")
                        {
                            s_info = mgr.Hud_TextStyleLibrary.TextStyle_Library_GetFirstStyleInfo_With_Type(xHud_TextType.TmpText);

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
                        s_info = mgr.Hud_TextStyleLibrary.TextStyle_Library_GetTextStyleInfo(sp_StyleName.stringValue, xHud_TextType.TmpText);
                    }
                    else
                    {
                        string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "HudText 文字组件消息", "刷新文字库样式", $"似乎 {BaseScript.name} ( {BaseScript.Indicator} ) 文字组件样式库名称并不在文字样式库中存在，请检查文字样式库名称是否合法，或者可以快速选择文字样式库默认首个样式。是否要选择首个样式？", "暂不", "选择", 0);
                        if (res == "选择")
                        {
                            s_info = mgr.Hud_TextStyleLibrary.TextStyle_Library_GetFirstStyleInfo_With_Type(xHud_TextType.TmpText);

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
        public void Open_Hud_Library_TextStyle_Setter(XHud_LibraryArg_TextStyle info)
        {
            Editor_XHud_LibrarySetTool_TextStyle window = EditorWindow.GetWindow<Editor_XHud_LibrarySetTool_TextStyle>(true);

            window.titleContent = new GUIContent("XHud 字体样式库采集器");
            Editor_XHud_GUI.CenterEditorWindow(new Vector2Int(850, 640), window);

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

        #region TMP InternalMethods
        static readonly GUIContent k_RaycastTargetLabel = new GUIContent("Raycast Target", "Whether the text blocks raycasts from the Graphic Raycaster.");
        static readonly GUIContent k_MaskableLabel = new GUIContent("Maskable", "Determines if the text object will be affected by UI Mask.");

        private SerializedProperty
            sp_RaycastTargetProp,
            sp_MaskableProp;

        protected override void DrawExtraSettings()
        {
            Rect rect = EditorGUILayout.GetControlRect(false, 24);

            if (GUI.Button(rect, new GUIContent("<b>Extra Settings</b>"), TMP_UIStyleManager.sectionHeader))
                Foldout.extraSettings = !Foldout.extraSettings;

            GUI.Label(rect, (Foldout.extraSettings ? k_UiStateLabel[0] : k_UiStateLabel[1]), TMP_UIStyleManager.rightLabel);
            if (Foldout.extraSettings)
            {
                //EditorGUI.indentLevel += 1;

                DrawMargins();

                DrawGeometrySorting();

                DrawIsTextObjectScaleStatic();

                DrawRichText();

                DrawRaycastTarget();

                DrawMaskable();

                DrawParsing();

                DrawSpriteAsset();

                DrawStyleSheet();

                //DrawKerning();

                DrawPadding();

                //EditorGUI.indentLevel -= 1;

            }
        }

        protected void DrawRaycastTarget()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(sp_RaycastTargetProp, k_RaycastTargetLabel);
            if (EditorGUI.EndChangeCheck())
            {
                // Change needs to propagate to the child sub objects.
                Graphic[] graphicComponents = m_TextComponent.GetComponentsInChildren<Graphic>();
                for (int i = 1; i < graphicComponents.Length; i++)
                    graphicComponents[i].raycastTarget = sp_RaycastTargetProp.boolValue;

                m_HavePropertiesChanged = true;
            }
        }

        protected void DrawMaskable()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(sp_MaskableProp, k_MaskableLabel);
            if (EditorGUI.EndChangeCheck())
            {
                m_TextComponent.maskable = sp_MaskableProp.boolValue;

                // Change needs to propagate to the child sub objects.
                MaskableGraphic[] maskableGraphics = m_TextComponent.GetComponentsInChildren<MaskableGraphic>();
                for (int i = 1; i < maskableGraphics.Length; i++)
                    maskableGraphics[i].maskable = sp_MaskableProp.boolValue;

                m_HavePropertiesChanged = true;
            }
        }

        protected override bool IsMixSelectionTypes()
        {
            GameObject[] objects = Selection.gameObjects;
            if (objects.Length > 1)
            {
                for (int i = 0; i < objects.Length; i++)
                {
                    if (objects[i].GetComponent<TextMeshProUGUI>() == null)
                        return true;
                }
            }
            return false;
        }

        protected override void OnUndoRedo()
        {
            int undoEventId = Undo.GetCurrentGroup();
            int lastUndoEventId = s_EventId;

            if (undoEventId != lastUndoEventId)
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    //sp_DebugMode.Log("Undo & Redo Performed detected in Editor Panel. Event ID:" + Undo.GetCurrentGroup());
                    TMPro_EventManager.ON_TEXTMESHPRO_UGUI_PROPERTY_CHANGED(true, targets[i] as TextMeshProUGUI);
                    s_EventId = undoEventId;
                }
            }
        }
        #endregion
    }
}