namespace SevenStrikeModules.XHud.Editor
{
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.Utilitys;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(XHud_Module_Primitive_Controller))]
    public class Editor_XHud_Module_Primitive_Controller : Editor
    {
        #region 组件 / 列表
        private XHud_Module_Primitive_Controller BaseScript;
        #endregion

        #region 序列化属性
        private SerializedProperty
            sp_ID,
            sp_Indicator,
            sp_ModuleType,
            sp_Debug,
            sp_IsInitial;
        #endregion

        #region GUI 参数
        /// <summary>
        /// 原始脚本参数显示开关
        /// </summary>
        private bool OriginalDisplay;
        /// <summary>
        /// 系统默认GUI行高
        /// </summary>
        private float LineHeight;
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
        string[] stroptions_debug = new string[2] { "关闭", "调试" },
            stroptions_mute = new string[] { "正常", "静音" },
            stroptions_control = new string[] { "可控", "忽略" };
        #endregion

        #region 图标
        private Texture2D icon_type, icon_text, icon_tmptext, icon_image, icon_rawimage, icon_main, icon_initial_r, icon_initial_p;
        #endregion

        #region 批量化操作
        /// <summary>
        /// 批量选择脚本数组
        /// </summary>
        XHud_Module_Primitive_Controller[] SelectedObjects;
        /// <summary>
        /// 获取所有批量脚本目标
        /// </summary>
        private void Targets_Get()
        {
            if (targets.Length > 1)
            {
                SelectedObjects = new XHud_Module_Primitive_Controller[targets.Length];
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    var t = targets[i];
                    SelectedObjects[i] = (XHud_Module_Primitive_Controller)t;
                }
            }
            else
            {
                SelectedObjects = new XHud_Module_Primitive_Controller[targets.Length];
                SelectedObjects[0] = (XHud_Module_Primitive_Controller)target;
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
            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

            BaseScript = (XHud_Module_Primitive_Controller)target;

            Targets_Get();

            GetSerializeFields();

            // 收集被控组件
            BaseScript.GetControlComponent();
            // 识别类型
            BaseScript.RecognizeType();

            #region 获取字体
            Font_Bold = Editor_XHud_GUI.GetFont("SS_Editor_Bold");
            Font_Light = Editor_XHud_GUI.GetFont("SS_Editor_Light");
            #endregion

            #region 获取图标          
            icon_main = Editor_XHud_GUI.GetIcon("Icons_XHud_Module_Primitive_Controller/icon_main");
            icon_text = Editor_XHud_GUI.GetIcon("Icons_XHud_Module_Primitive_Controller/icon_text");
            icon_tmptext = Editor_XHud_GUI.GetIcon("Icons_XHud_Module_Primitive_Controller/icon_tmptext");
            icon_image = Editor_XHud_GUI.GetIcon("Icons_XHud_Module_Primitive_Controller/icon_image");
            icon_rawimage = Editor_XHud_GUI.GetIcon("Icons_XHud_Module_Primitive_Controller/icon_rawimage");
            icon_initial_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Module_Primitive_Controller/icon_initial_r");
            icon_initial_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Module_Primitive_Controller/icon_initial_p");
            #endregion
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            #region 类型识别
            ModuleType m_type = (ModuleType)sp_ModuleType.enumValueIndex;
            switch (m_type)
            {
                case ModuleType.Text:
                    icon_type = icon_text;
                    break;
                case ModuleType.TmpText:
                    icon_type = icon_tmptext;
                    break;
                case ModuleType.Image:
                    icon_type = icon_image;
                    break;
                case ModuleType.RawImage:
                    icon_type = icon_rawimage;
                    break;
            }
            #endregion

            #region 标题
            string h_color = XHud_Utilitys.Color_To_HexColor(XHud_Dashboard.Theme_Primary);
            string titlename = "XHud - 图元  >  控制器";
            Editor_XHud_GUI.Gui_Layout_Banner(icon_main, HudFilled.实体, HudColor.深空灰, titlename, Color.white, icon_type, m_type.ToString(), 20, 20);
            Rect rect = GUILayoutUtility.GetLastRect();
            #endregion

            // 获取 XHud 管理器
            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

            #region 快捷功能
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 0, "快捷功能", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            #region 快捷按钮
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);

            ///---切换模式按钮
            if (Editor_XHud_GUI.Gui_Layout_Button(14, "", icon_initial_r, icon_initial_p, 4))
            {
                if (Targets_Selected())
                {
                    List<XHud_GUI_Dialog_ListDatas> Datas = new List<XHud_GUI_Dialog_ListDatas>();

                    for (int i = 0; i < SelectedObjects.Length; i++)
                    {
                        XHud_GUI_Dialog_ListDatas data = new XHud_GUI_Dialog_ListDatas();
                        data.Title = string.IsNullOrEmpty(SelectedObjects[i].Indicator) ? SelectedObjects[i].name : SelectedObjects[i].Indicator;
                        data.SubTitle = "当前颜色模式";
                        data.Message = "";
                        Datas.Add(data);
                    }

                    EditorApplication.delayCall += () =>
                    {
                        string res_x = Editor_XHud_GUI.Open(Datas.ToArray(), XHud_DialogType.警告, "XHud - 图元控制器消息", "批量初始化图元结构", "是否需要批量为图元建立控制脚本结构吗？", "建立", "暂不", 1);
                        if (res_x == "建立")
                        {
                            for (int i = 0; i < SelectedObjects.Length; i++)
                            {
                                if (SelectedObjects[i].IsInitial)
                                {
                                    string csd = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 图元控制器消息", "重新初始化", $"确认为 {SelectedObjects[i].gameObject.name} 图元重新建立控制脚本结构吗？", "重建", "暂不", 1);
                                    if (csd == "重建")
                                    {
                                        Undo.DestroyObjectImmediate(SelectedObjects[i].gameObject.GetComponent<CanvasGroup>());
                                        Undo.DestroyObjectImmediate(SelectedObjects[i].gameObject.GetComponent<XHud_Module_Primitive_Painting_Synchronizer>());
                                        Undo.DestroyObjectImmediate(SelectedObjects[i].gameObject.GetComponent<XHud_Module_Primitive_Painting>());
                                        Undo.DestroyObjectImmediate(SelectedObjects[i].gameObject.GetComponent<XHud_Module_Primitive_Feature>());
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }

                                Undo.AddComponent(SelectedObjects[i].gameObject, typeof(CanvasGroup));
                                Undo.AddComponent(SelectedObjects[i].gameObject, typeof(XHud_Module_Primitive_Painting_Synchronizer));
                                XHud_Module_Primitive_Painting comp_painting = (XHud_Module_Primitive_Painting)Undo.AddComponent(SelectedObjects[i].gameObject, typeof(XHud_Module_Primitive_Painting));
                                comp_painting.FindController();
                                XHud_Module_Primitive_Feature comp_feature = (XHud_Module_Primitive_Feature)Undo.AddComponent(SelectedObjects[i].gameObject, typeof(XHud_Module_Primitive_Feature));
                                comp_feature.FindController();

                                SelectedObjects[i].IsInitial = true;
                            }
                        }
                    };
                }
                else
                {
                    EditorApplication.delayCall += () =>
                    {
                        string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 图元控制器消息", "初始化图元结构", $"确认为此图元建立控制脚本结构吗？", "建立", "暂不", 1);
                        if (res == "建立")
                        {
                            if (BaseScript.IsInitial)
                            {
                                string csd = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 图元控制器消息", "重新初始化", $"确认为此图元重新建立控制脚本结构吗？", "重建", "暂不", 1);
                                if (csd == "重建")
                                {
                                    Undo.DestroyObjectImmediate(BaseScript.gameObject.GetComponent<CanvasGroup>());
                                    Undo.DestroyObjectImmediate(BaseScript.gameObject.GetComponent<XHud_Module_Primitive_Painting_Synchronizer>());
                                    Undo.DestroyObjectImmediate(BaseScript.gameObject.GetComponent<XHud_Module_Primitive_Painting>());
                                    Undo.DestroyObjectImmediate(BaseScript.gameObject.GetComponent<XHud_Module_Primitive_Feature>());
                                }
                                else
                                {
                                    return;
                                }
                            }

                            Undo.AddComponent(BaseScript.gameObject, typeof(CanvasGroup));
                            Undo.AddComponent(BaseScript.gameObject, typeof(XHud_Module_Primitive_Painting_Synchronizer));
                            XHud_Module_Primitive_Painting comp_painting = (XHud_Module_Primitive_Painting)Undo.AddComponent(BaseScript.gameObject, typeof(XHud_Module_Primitive_Painting));
                            comp_painting.FindController();
                            XHud_Module_Primitive_Feature comp_feature = (XHud_Module_Primitive_Feature)Undo.AddComponent(BaseScript.gameObject, typeof(XHud_Module_Primitive_Feature));
                            comp_feature.FindController();

                            BaseScript.IsInitial = true;
                        }
                    };
                }
                return;
            }

            GUILayout.Space(10);
            GUILayout.EndHorizontal();

            #endregion

            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 基础参数
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 0, "基础参数", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            #region ID 
            Editor_XHud_GUI.Gui_Layout_Property_Field("ID", sp_ID);
            #endregion

            Editor_XHud_GUI.Gui_Layout_Space(5);

            #region 标识
            Editor_XHud_GUI.Gui_Layout_Property_Field("标识", sp_Indicator);
            sp_Indicator.serializedObject.ApplyModifiedProperties();
            #endregion

            Editor_XHud_GUI.Gui_Layout_Space(5);

            #region 类型
            GUI.enabled = false;
            Editor_XHud_GUI.Gui_Layout_Property_Field("被控类型", sp_ModuleType);
            GUI.enabled = true;
            #endregion

            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 选项
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "选项", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_Primitive_Controller>("调试", stroptions_debug, ref sp_Debug, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

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
            sp_ID = serializedObject.FindProperty("ID");
            sp_Indicator = serializedObject.FindProperty("Indicator");
            sp_ModuleType = serializedObject.FindProperty("ModuleType");
            sp_Debug = serializedObject.FindProperty("Debug");
            sp_IsInitial = serializedObject.FindProperty("IsInitial");
        }
    }
}
