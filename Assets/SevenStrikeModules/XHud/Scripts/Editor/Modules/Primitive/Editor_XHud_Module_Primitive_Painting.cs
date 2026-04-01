namespace SevenStrikeModules.XHud.Editor
{
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.Utilitys;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(XHud_Module_Primitive_Painting))]
    public class Editor_XHud_Module_Primitive_Painting : Editor
    {
        #region 组件 / 列表
        private XHud_Module_Primitive_Painting BaseScript;
        #endregion

        #region 序列化属性
        private SerializedProperty
            sp_OriginalColor,
            sp_ColoriseName,
            sp_Debug,
            sp_SyncLibraryColor;
        #endregion

        #region GUI 参数
        /// <summary>
        /// 系统默认GUI行高
        /// </summary>
        private float LineHeight;
        /// <summary>
        /// 原始脚本参数显示开关
        /// </summary>
        private bool OriginalDisplay;
        #endregion

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

        #region 选项文字
        string[] stroptions_debug = new string[2] { "关闭", "调试" };
        #endregion

        #region 图标
        private Texture2D icon_text, icon_tmptext, icon_image, icon_rawimage, icon_main, colormode_lib_r, colormode_lib_p, colormode_ori_r, colormode_ori_p, colormode_mix_r, colormode_mix_p, Add_r, Add_p, locate_r, locate_p;
        #endregion

        #region 批量化操作
        /// <summary>
        /// 批量选择脚本数组
        /// </summary>
        XHud_Module_Primitive_Painting[] SelectedObjects;
        /// <summary>
        /// 获取所有批量脚本目标
        /// </summary>
        private void Targets_Get()
        {
            if (targets.Length > 1)
            {
                SelectedObjects = new XHud_Module_Primitive_Painting[targets.Length];
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    var t = targets[i];
                    SelectedObjects[i] = (XHud_Module_Primitive_Painting)t;
                }
            }
            else
            {
                SelectedObjects = new XHud_Module_Primitive_Painting[targets.Length];
                SelectedObjects[0] = (XHud_Module_Primitive_Painting)target;
            }
        }
        /// <summary>
        /// 判断是否是多选状态
        /// </summary>
        /// <returns></returns>
        private bool Targets_Selected()
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

        private void OnEnable()
        {
            #region 获取系统GUI单行单位高度
            LineHeight = EditorGUIUtility.singleLineHeight;
            #endregion

            BaseScript = (XHud_Module_Primitive_Painting)target;

            Targets_Get();

            // 获取所有序列化字段
            GetSerializeFields();

            #region 获取字体
            Font_Bold = Editor_XHud_GUI.GetFont("SS_Editor_Bold");
            Font_Light = Editor_XHud_GUI.GetFont("SS_Editor_Light");
            #endregion

            #region 获取图标          
            icon_main = Editor_XHud_GUI.GetIcon("Icons_XHud_Module_Primitive_Painting/icon_main");
            colormode_lib_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Module_Primitive_Painting/col_lib_r");
            colormode_lib_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Module_Primitive_Painting/col_lib_p");
            colormode_ori_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Module_Primitive_Painting/col_ori_r");
            colormode_ori_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Module_Primitive_Painting/col_ori_p");
            colormode_mix_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Module_Primitive_Painting/col_mix_r");
            colormode_mix_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Module_Primitive_Painting/col_mix_p");
            Add_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Module_Primitive_Painting/Add_r");
            Add_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Module_Primitive_Painting/Add_p");
            locate_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Module_Primitive_Painting/locate_r");
            locate_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Module_Primitive_Painting/locate_p");
            #endregion
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            #region 标题
            string h_color = XHud_Utilitys.Color_To_HexColor(XHud_Dashboard.Theme_Primary);
            string titlename = "XHud - 图元  >  配色器";
            Editor_XHud_GUI.Gui_Layout_Banner(icon_main, HudFilled.实体, HudColor.深空灰, titlename, Color.white, null, "", 20, 20);
            Rect rect = GUILayoutUtility.GetLastRect();
            #endregion

            // 获取 XHud 管理器
            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

            #region 检测多选模式下的颜色模式是否一致
            // 该字段用于检测多选情况下每个Painting的颜色模式是否是一致的
            bool MultiColorModeSame = true;
            bool mode = SelectedObjects[0].SyncLibraryColor;
            if (Targets_Selected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    if (SelectedObjects[i].SyncLibraryColor != mode)
                    {
                        MultiColorModeSame = false;
                        break;
                    }
                }
            }
            #endregion

            #region 快捷功能
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 0, "快捷功能", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            #region 快捷按钮
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);

            #region 默认配色 / 色卡库的模式切换按钮
            Texture2D icon_r = null;
            Texture2D icon_p = null;

            #region 根据模式类型显示不同的内容与图标

            if (MultiColorModeSame)
            {
                if (mode)
                {
                    icon_r = colormode_lib_r;
                    icon_p = colormode_lib_p;
                }
                else
                {
                    icon_r = colormode_ori_r;
                    icon_p = colormode_ori_p;
                }
            }
            else
            {
                icon_r = colormode_mix_r;
                icon_p = colormode_mix_p;
            }
            #endregion

            ///---切换模式按钮
            if (Editor_XHud_GUI.Gui_Layout_Button(14, "", icon_r, icon_p, 4))
            {
                if (Targets_Selected())
                {
                    List<XHud_GUI_Dialog_ListDatas> Datas = new List<XHud_GUI_Dialog_ListDatas>();

                    for (int i = 0; i < SelectedObjects.Length; i++)
                    {
                        XHud_GUI_Dialog_ListDatas data = new XHud_GUI_Dialog_ListDatas();
                        data.Title = string.IsNullOrEmpty(SelectedObjects[i].controller.Indicator) ? SelectedObjects[i].name : SelectedObjects[i].controller.Indicator;
                        data.SubTitle = "当前颜色模式";
                        data.Message = SelectedObjects[i].SyncLibraryColor ? "色卡库" : "原始色";
                        Datas.Add(data);
                    }

                    string res_x = Editor_XHud_GUI.Open(Datas.ToArray(), XHud_DialogType.警告, "XHud - 图元配色器消息", "批量切换颜色模式", "是否需要批量切换以下列表中的动画器物体的颜色模式？", "暂不", "切换", 0);
                    if (res_x == "切换")
                    {
                        string res_y = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 图元配色器消息", "切换颜色模式", "需要切换为那种模式？", "色卡库", "原始色", 1);
                        if (res_y == "原始色")
                        {
                            for (int i = 0; i < SelectedObjects.Length; i++)
                            {
                                SelectedObjects[i].SyncLibraryColor = false;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < SelectedObjects.Length; i++)
                            {
                                SelectedObjects[i].SyncLibraryColor = true;

                                if (!Application.isPlaying)
                                {
                                    //--检查色卡名称是否失效
                                    if (!mgr.Hud_Colors.ColorsLibrary_IsExist(SelectedObjects[i].ColoriseName) || string.IsNullOrEmpty(SelectedObjects[i].ColoriseName))
                                    {
                                        SelectedObjects[i].ColoriseName = mgr.Hud_Colors.ColorsLibrary_GetColorName(0);
                                    }
                                }
                                else
                                {
                                    //--检查色卡名称是否失效
                                    if (!mgr.Hud_Colors.ColorsLibrary_IsExist(SelectedObjects[i].ColoriseName) || string.IsNullOrEmpty(SelectedObjects[i].ColoriseName))
                                    {
                                        SelectedObjects[i].ColoriseName = mgr.Hud_Colors.ColorsLibrary_GetColorName(0);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 图元配色器消息", "切换颜色模式", $"需要将 {(string.IsNullOrEmpty(BaseScript.controller.Indicator) ? BaseScript.name : BaseScript.controller.Indicator)} 颜色显示切换为那种模式？", "色卡库", "原始色", 1);
                    if (res == "原始色")
                    {
                        sp_SyncLibraryColor.boolValue = false;
                        sp_SyncLibraryColor.serializedObject.ApplyModifiedProperties();
                    }
                    else
                    {
                        sp_SyncLibraryColor.boolValue = true;
                        sp_SyncLibraryColor.serializedObject.ApplyModifiedProperties();

                        if (!Application.isPlaying)
                        {
                            //--检查色卡名称是否失效
                            if (!mgr.Hud_Colors.ColorsLibrary_IsExist(sp_ColoriseName.stringValue) || string.IsNullOrEmpty(sp_ColoriseName.stringValue))
                            {
                                sp_ColoriseName.stringValue = mgr.Hud_Colors.ColorsLibrary_GetColorName(0);
                                sp_ColoriseName.serializedObject.ApplyModifiedProperties();
                            }
                        }
                        else
                        {
                            //--检查色卡名称是否失效
                            if (!mgr.Hud_Colors.ColorsLibrary_IsExist(sp_ColoriseName.stringValue) || string.IsNullOrEmpty(sp_ColoriseName.stringValue))
                            {
                                sp_ColoriseName.stringValue = mgr.Hud_Colors.ColorsLibrary_GetColorName(0);
                                sp_ColoriseName.serializedObject.ApplyModifiedProperties();
                            }
                        }
                    }
                }
                return;
            }
            #endregion

            GUILayout.Space(10);
            GUILayout.EndHorizontal();

            #endregion

            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 配色参数
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 0, "配色参数", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            bool TextSyncing = false;

            XHud_Module_Primitive_Controller con = BaseScript.controller;

            #region 如果存在文字组件
            // 如果是 XHud_Text 受控组件类型，则获取文字组件的“同步库颜色状态开关”到 TextSyncing 状态开关
            // 表明文字组件的文字颜色是否是使用同步到库状态
            if (con.ModuleType == ModuleType.Text)
            {
                if (con.mod_Text.StyleLibSynching)
                {
                    if (con.mod_Text.TextStyleInfo.LibStyle_Effect_color)
                    {
                        TextSyncing = true;
                    }
                }
            }
            // 如果是 XHud_TmpText 受控组件类型，则获取文字组件的“同步库颜色状态开关”到 TextSyncing 状态开关
            // 表明文字组件的文字颜色是否是使用同步到库状态
            if (con.ModuleType == ModuleType.TmpText)
            {
                if (con.mod_TmpText.StyleLibSynching)
                {
                    if (con.mod_TmpText.TextStyleInfo.LibStyle_Effect_color)
                    {
                        TextSyncing = true;
                    }
                }
            }
            #endregion

            if (!TextSyncing)
            {
                #region 配色调整
                // 如果不同步到库
                if (!sp_SyncLibraryColor.boolValue)
                {
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    GUILayout.BeginHorizontal();
                    if (!Targets_Selected())
                    {
                        Editor_XHud_GUI.Gui_Layout_Space(10);
                        if (Editor_XHud_GUI.Gui_Layout_Button(14, "将当前颜色添加到色卡库中", Add_r, Add_p, 4))
                        {
                            xHud_LibraryArg_Color info = new xHud_LibraryArg_Color();
                            info.Painting = BaseScript;
                            info.Color = sp_OriginalColor.colorValue;
                            OpenLibrarySetTool(info);
                        }
                    }
                    EditorGUI.BeginChangeCheck();
                    Editor_XHud_GUI.Gui_Layout_Property_Field("原始配色", sp_OriginalColor);
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (Targets_Selected())
                        {
                            for (int i = 0; i < SelectedObjects.Length; i++)
                            {
                                SelectedObjects[i].UpdateColor(sp_OriginalColor.colorValue);
                            }
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                // 如果同步到库
                else
                {
                    #region 色卡库选项列表
                    if (mgr != null)
                    {
                        if (mgr.Hud_Colors != null && !mgr.Hud_Colors.ColorsLibrary_IsEmpty())
                        {
                            Editor_XHud_GUI.Gui_Layout_Space(5);
                            GUILayout.BeginHorizontal();

                            if (!Targets_Selected())
                            {
                                Editor_XHud_GUI.Gui_Layout_Space(10);
                                if (Editor_XHud_GUI.Gui_Layout_Button(14, "定位色卡", locate_r, locate_p, 0))
                                {
                                    if (!Application.isPlaying)
                                    {
                                        if (!mgr.Hud_Colors.ColorsLibrary_IsExist(sp_ColoriseName.stringValue))
                                            return;
                                        Editor_XHud_MenuItemsAction_OpenLibrary.open_col();
                                        mgr.Hud_Colors.ColorsLibrary_Location(sp_ColoriseName.stringValue);
                                    }
                                    else
                                    {
                                        if (!mgr.Hud_Colors.ColorsLibrary_IsExist(sp_ColoriseName.stringValue))
                                            return;
                                        Editor_XHud_MenuItemsAction_OpenLibrary.open_col();
                                        mgr.Hud_Colors.ColorsLibrary_Location(sp_ColoriseName.stringValue);
                                    }
                                }
                                Editor_XHud_GUI.Gui_Layout_Space(20);
                            }
                            else
                            {
                                Editor_XHud_GUI.Gui_Layout_Space(10);
                            }

                            #region 控件列表文字 （色卡库列表下拉菜单）
                            string[] collist = mgr.Hud_Colors.ColorsLibrary_GetColorNames();
                            #endregion

                            #region 控件背景色
                            Color cc = Color.white;
                            bool SameColor = true;
                            string FirstColoriseName = SelectedObjects[0].ColoriseName;
                            if (!Targets_Selected())
                            {
                                cc = mgr.Hud_Colors.ColorsLibrary_GetColor(sp_ColoriseName.stringValue);
                            }
                            else
                            {
                                for (int k = 1; k < SelectedObjects.Length; k++)
                                {
                                    if (SelectedObjects[k].ColoriseName != FirstColoriseName)
                                    {
                                        SameColor = false;
                                        break;
                                    }
                                }
                            }

                            if (SameColor)
                            {
                                cc = mgr.Hud_Colors.ColorsLibrary_GetColor(FirstColoriseName);
                            }
                            else
                            {
                                cc = Color.gray;
                            }
                            #endregion

                            if (Targets_Selected())
                            {
                                for (int i = 0; i < SelectedObjects.Length; i++)
                                {
                                    // 使用 Undo.RecordObject 来记录对目标对象的修改
                                    Undo.RecordObject(SelectedObjects[i], "Selected ColoriseName");
                                }
                            }
                            else
                            {
                                // 使用 Undo.RecordObject 来记录对目标对象的修改
                                Undo.RecordObject(sp_ColoriseName.serializedObject.targetObject, "Selected ColoriseName");
                            }

                            string xx = Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_Module_Primitive_Painting>("色卡", collist, ref sp_ColoriseName, HudFilled.实体, cc, 400, 20, SelectedObjects);
                            GUILayout.EndHorizontal();
                        }
                        else
                        {
                            Editor_XHud_GUI.Gui_Layout_Space(10);
                            GUILayout.BeginHorizontal();
                            Editor_XHud_GUI.Gui_Layout_Space(10);
                            Editor_XHud_GUI.Gui_Layout_Labelfield("未找到色卡库", HudFilled.无, HudColor.无, Editor_XHud_GUI.GetColor(HudColor.魅力红), TextAnchor.MiddleCenter, new Vector2(0, 0), 11);
                            Editor_XHud_GUI.Gui_Layout_Space(10);
                            GUILayout.EndHorizontal();
                            Editor_XHud_GUI.Gui_Layout_Space(10);
                        }
                    }
                    #endregion
                }
                #endregion
            }
            else
            {
                Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_XHud_GUI.Gui_Layout_Space(10);
                EditorGUILayout.HelpBox("字体颜色当前正在和字体样式库同步中，如果需要接管文字颜色请先让文字组件的同步样式关闭！", MessageType.Warning);
                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Horizontal_End();
            }
            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 选项
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "选项", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_Primitive_Painting>("调试", stroptions_debug, ref sp_Debug, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 源脚本
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 3, "源脚本", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(5);

            #region 脚本类
            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_XHud_GUI.Gui_Layout_Space(10);
            OriginalDisplay = EditorGUILayout.Foldout(OriginalDisplay, "脚本类", true);
            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Horizontal_End();
            if (OriginalDisplay)
                DrawDefaultInspector();
            #endregion

            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 获取所有序列化字段
        /// </summary>
        private void GetSerializeFields()
        {
            sp_OriginalColor = serializedObject.FindProperty("OriginalColor");
            sp_ColoriseName = serializedObject.FindProperty("ColoriseName");
            sp_Debug = serializedObject.FindProperty("Debug");
            sp_SyncLibraryColor = serializedObject.FindProperty("SyncLibraryColor");
        }

        /// <summary>
        /// 色卡库采集设置器
        /// </summary>
        public void OpenLibrarySetTool(xHud_LibraryArg_Color info)
        {
            Editor_XHud_LibrarySetTool_Color window = EditorWindow.GetWindow<Editor_XHud_LibrarySetTool_Color>(true);

            window.titleContent = new GUIContent("XHud 色卡库采集器");
            Editor_XHud_GUI.CenterEditorWindow(new Vector2Int(620, 530), window);

            window.SetLibrarySetterMode(LibrarySetterMode.添加到库);
            window.SetTitle("XHud 色卡库采集器");
            window.SetInfo(info.Name, info.Description, info.Color);
            window.SetPainting(info.Painting);
            window.SetButtonText("添加", "取消");
            window.SetTarget_Hud_ColorsLibrary(XHud_Dashboard.HudManagerGet().Hud_Colors);
            //window.ShowModal();
            window.Show();
        }
    }
}
