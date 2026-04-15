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
                        data.SubTitle = "即将初始化图元结构";
                        data.Message = "";
                        Datas.Add(data);
                    }

                    EditorApplication.delayCall += () =>
                    {
                        string res_x = Editor_XHud_GUI.Open(Datas.ToArray(), XHud_DialogType.警告, "XHud - 图元控制器消息", "批量初始化图元结构", "是否需要批量为图元建立控制脚本结构吗？", "建立", "暂不", 0);
                        if (res_x == "建立")
                        {
                            for (int i = 0; i < SelectedObjects.Length; i++)
                            {
                                if (SelectedObjects[i].IsInitial)
                                {
                                    string csd = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 图元控制器消息", "重新初始化", $"确认为 {SelectedObjects[i].gameObject.name} 图元重新建立控制脚本结构吗？", "重建", "暂不", 1);
                                    if (csd == "重建")
                                    {
                                        SelectedObjects[i].ClearComponents_For_Editor();
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }

                                SelectedObjects[i].InitialComponents_For_Editor();
                            }
                        }
                    };
                }
                else
                {
                    EditorApplication.delayCall += () =>
                    {
                        string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 图元控制器消息", "初始化图元结构", $"确认为此图元建立控制脚本结构吗？", "建立", "暂不", 0);
                        if (res == "建立")
                        {
                            if (BaseScript.IsInitial)
                            {
                                string csd = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 图元控制器消息", "重新初始化", $"确认为此图元重新建立控制脚本结构吗？", "重建", "暂不", 1);
                                if (csd == "重建")
                                {
                                    BaseScript.ClearComponents_For_Editor();
                                }
                                else
                                {
                                    return;
                                }
                            }
                            BaseScript.InitialComponents_For_Editor();
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
