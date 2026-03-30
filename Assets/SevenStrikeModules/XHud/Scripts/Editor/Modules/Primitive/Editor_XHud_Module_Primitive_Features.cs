namespace SevenStrikeModules.XHud
{
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.GuiLib;
    using SevenStrikeModules.XHud.Utilitys;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(XHud_Module_Primitive_Feature))]
    public class Editor_XHud_Module_Primitive_Features : Editor
    {
        #region 组件 / 列表
        private XHud_Module_Primitive_Feature BaseScript;
        #endregion

        #region 序列化属性
        private SerializedProperty
            sp_Debug,
            sp_PrimitiveFeatures,
            FirstSaveFeatures;
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
        /// <summary>
        /// 多选特性的索引
        /// </summary>
        private int MultiPrimitiveFeature_Index;
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
        private Texture2D icon_main, f_pos, f_rot, f_sca, f_size, f_alp, f_fill, f_color, left_arrow_r, left_arrow_p, right_arrow_r, right_arrow_p, f_save_r, f_save_p, f_load_r, f_load_p;
        #endregion

        #region 批量化操作
        /// <summary>
        /// 批量选择脚本数组
        /// </summary>
        XHud_Module_Primitive_Feature[] SelectedObjects;
        /// <summary>
        /// 获取所有批量脚本目标
        /// </summary>
        private void Targets_Get()
        {
            if (targets.Length > 1)
            {
                SelectedObjects = new XHud_Module_Primitive_Feature[targets.Length];
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    var t = targets[i];
                    SelectedObjects[i] = (XHud_Module_Primitive_Feature)t;
                }
            }
            else
            {
                SelectedObjects = new XHud_Module_Primitive_Feature[targets.Length];
                SelectedObjects[0] = (XHud_Module_Primitive_Feature)target;
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

        private PrimitiveFeatures[] PrimitiveFeatures;

        private void OnEnable()
        {
            #region 获取系统GUI单行单位高度
            LineHeight = EditorGUIUtility.singleLineHeight;
            #endregion

            BaseScript = (XHud_Module_Primitive_Feature)target;

            Targets_Get();

            // 获取所有序列化字段
            GetSerializeFields();

            #region 获取字体
            Font_Bold = Editor_XHud_GUI.GetFont("SS_Editor_Bold");
            Font_Light = Editor_XHud_GUI.GetFont("SS_Editor_Light");
            #endregion

            #region 获取图标          
            icon_main = Editor_XHud_GUI.GetIcon("Icons_XHud_Module_Primitive_Features/icon_main");
            f_pos = Editor_XHud_GUI.GetIcon("Icons_XHud_Module_Primitive_Features/f_pos");
            f_rot = Editor_XHud_GUI.GetIcon("Icons_XHud_Module_Primitive_Features/f_rot");
            f_sca = Editor_XHud_GUI.GetIcon("Icons_XHud_Module_Primitive_Features/f_sca");
            f_size = Editor_XHud_GUI.GetIcon("Icons_XHud_Module_Primitive_Features/f_size");
            f_alp = Editor_XHud_GUI.GetIcon("Icons_XHud_Module_Primitive_Features/f_alp");
            f_fill = Editor_XHud_GUI.GetIcon("Icons_XHud_Module_Primitive_Features/f_fill");
            f_color = Editor_XHud_GUI.GetIcon("Icons_XHud_Module_Primitive_Features/f_color");
            left_arrow_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Module_Primitive_Features/left_arrow_r");
            left_arrow_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Module_Primitive_Features/left_arrow_p");
            right_arrow_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Module_Primitive_Features/right_arrow_r");
            right_arrow_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Module_Primitive_Features/right_arrow_p");
            f_save_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Module_Primitive_Features/f_save_r");
            f_save_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Module_Primitive_Features/f_save_p");
            f_load_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Module_Primitive_Features/f_load_r");
            f_load_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Module_Primitive_Features/f_load_p");

            #endregion

            CheckFirstCreatedAnimator();

            PrimitiveFeature_Capture(SelectedObjects);
        }

        private void OnDisable()
        {
            if (!Application.isPlaying)
            {
                #region 记录特性
                if (Targets_Selected())
                {
                    for (int i = 0; i < SelectedObjects.Length; i++)
                    {
                        if (SelectedObjects[i] != null)
                            PrimitiveFeature_Save(SelectedObjects[i]);
                    }
                }
                else
                {
                    if (target != null)
                    {
                        PrimitiveFeature_Save(BaseScript);
                        serializedObject.ApplyModifiedProperties();
                    }
                }
                #endregion
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            #region 标题
            string h_color = XHud_Utilitys.Color_To_HexColor(XHud_Dashboard.Theme_Primary);
            string titlename = "XHud - 图元  >  特性";
            Editor_XHud_GUI.Gui_Layout_Banner(icon_main, HudFilled.实体, HudColor.深空灰, titlename, Color.white, null, "", 20, 20);
            Rect rect = GUILayoutUtility.GetLastRect();
            #endregion

            // 获取 XHud 管理器
            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

            #region 快捷功能
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 0, "快捷功能", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);

            ///--- 特性的记录按钮
            if (Editor_XHud_GUI.Gui_Layout_Button(14, "", f_save_r, f_save_p, 4))
            {
                PrimitiveFeature_Dual_Save_WithDialog();
            }
            ///--- 特性的读取按钮
            if (Editor_XHud_GUI.Gui_Layout_Button(14, "", f_load_r, f_load_p, 4))
            {
                PrimitiveFeature_Dual_Load_WithDialog();
            }
            GUILayout.Space(10);
            GUILayout.EndHorizontal();

            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 特性参数
            string info_title = "特性参数";
            if (Targets_Selected())
                info_title = "特性参数 - ( 批量模式 )";
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, info_title, XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(5);

            if (!Targets_Selected())
            {
                SerializedProperty pos = sp_PrimitiveFeatures.FindPropertyRelative("Position");
                SerializedProperty eur = sp_PrimitiveFeatures.FindPropertyRelative("Euler");
                SerializedProperty sca = sp_PrimitiveFeatures.FindPropertyRelative("Scale");
                SerializedProperty size = sp_PrimitiveFeatures.FindPropertyRelative("Size");
                SerializedProperty alp = sp_PrimitiveFeatures.FindPropertyRelative("Alpha");
                SerializedProperty col = sp_PrimitiveFeatures.FindPropertyRelative("Color");
                SerializedProperty fil = sp_PrimitiveFeatures.FindPropertyRelative("Fill");

                PrimitiveFeature_Displayer(pos.vector3Value, eur.vector3Value, sca.vector3Value, size.vector2Value, alp.floatValue, fil.floatValue, col.colorValue);
            }
            else
            {
                Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_XHud_GUI.Gui_Layout_Space(10);
                if (Editor_XHud_GUI.Gui_Layout_Button($"{SelectedObjects[MultiPrimitiveFeature_Index].name} ( {SelectedObjects[MultiPrimitiveFeature_Index].controller.Indicator} )", "", HudFilled.透明, HudColor.无, SelectedObjects[MultiPrimitiveFeature_Index].controller.pt_Painting.OriginalColor, 20))
                {
                    EditorGUIUtility.PingObject(SelectedObjects[MultiPrimitiveFeature_Index]);
                }
                Editor_XHud_GUI.Gui_Layout_FlexSpace();
                if (Editor_XHud_GUI.Gui_Layout_Button(12, "", left_arrow_r, left_arrow_p))
                {
                    if (MultiPrimitiveFeature_Index <= 0)
                    {
                        MultiPrimitiveFeature_Index = PrimitiveFeatures.Length - 1;
                    }
                    else
                    {
                        MultiPrimitiveFeature_Index--;
                    }
                    EditorGUIUtility.PingObject(SelectedObjects[MultiPrimitiveFeature_Index]);
                }
                Editor_XHud_GUI.Gui_Layout_Space(16);
                if (Editor_XHud_GUI.Gui_Layout_Button(12, "", right_arrow_r, right_arrow_p))
                {
                    if (MultiPrimitiveFeature_Index >= PrimitiveFeatures.Length - 1)
                    {
                        MultiPrimitiveFeature_Index = 0;
                    }
                    else
                    {
                        MultiPrimitiveFeature_Index++;
                    }
                    EditorGUIUtility.PingObject(SelectedObjects[MultiPrimitiveFeature_Index]);
                }
                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                PrimitiveFeature_Displayer(PrimitiveFeatures[MultiPrimitiveFeature_Index].Position, PrimitiveFeatures[MultiPrimitiveFeature_Index].Euler, PrimitiveFeatures[MultiPrimitiveFeature_Index].Scale, PrimitiveFeatures[MultiPrimitiveFeature_Index].Size, PrimitiveFeatures[MultiPrimitiveFeature_Index].Alpha, PrimitiveFeatures[MultiPrimitiveFeature_Index].Fill, PrimitiveFeatures[MultiPrimitiveFeature_Index].Color);
            }

            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 选项
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "选项", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_Primitive_Feature>("调试", stroptions_debug, ref sp_Debug, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 右键菜单
            ContextMenu(rect);
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
        /// 右键菜单
        /// </summary>
        private void ContextMenu(Rect rect)
        {
            Rect rect_MenuArea = new Rect(0, rect.y, EditorGUIUtility.currentViewWidth, 260);

            // 在指定区域点击右键弹出菜单
            if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && (rect_MenuArea.Contains(Event.current.mousePosition)))
            {
                // 创建右键菜单
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("S (记录特性)"), false, () =>
                {
                    PrimitiveFeature_Dual_Save_WithDialog();
                });
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("X (读取特性)"), false, () =>
                {
                    PrimitiveFeature_Dual_Load_WithDialog();
                });
                menu.ShowAsContext(); // 在鼠标位置显示右键菜单
            }
        }

        /// <summary>
        /// 获取所有序列化字段
        /// </summary>
        private void GetSerializeFields()
        {
            sp_PrimitiveFeatures = serializedObject.FindProperty("PrimitiveFeatures");
            sp_Debug = serializedObject.FindProperty("Debug");
            FirstSaveFeatures = serializedObject.FindProperty("FirstSaveFeatures");
        }

        /// <summary>
        /// 检测图元特性首次初始状态
        /// </summary>
        private void CheckFirstCreatedAnimator()
        {
            if (Targets_Selected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    XHud_Module_Primitive_Feature feature = SelectedObjects[i];
                    SerializedObject so = new SerializedObject(feature);
                    so.Update();
                    SerializedProperty prop_FirstSaveFeatures = so.FindProperty("FirstSaveFeatures");
                    if (!prop_FirstSaveFeatures.boolValue)
                    {
                        prop_FirstSaveFeatures.boolValue = true;
                        // 首次记录姿态信息
                        PrimitiveFeature_Save(feature);
                    }
                    so.ApplyModifiedProperties();
                }
            }
            else
            {
                if (!FirstSaveFeatures.boolValue)
                {
                    FirstSaveFeatures.boolValue = true;
                    serializedObject.ApplyModifiedProperties();

                    // 首次记录姿态信息
                    PrimitiveFeature_Save(BaseScript);
                }
            }
        }

        #region 特性参数
        /// <summary>
        /// 绘制 PrimitiveFeature 特性参数控件
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="eur"></param>
        /// <param name="sca"></param>
        /// <param name="size"></param>
        /// <param name="alp"></param>
        /// <param name="col"></param>
        /// <param name="fil"></param>
        private void PrimitiveFeature_Displayer(Vector3 pos, Vector3 eur, Vector3 sca, Vector2 size, float alp, float fil, Color col)
        {
            int titlesize = 12;
            int contensize = 10;
            float iconsize = 12;
            Color valuecol = Color.white * 0.75f;

            Editor_XHud_GUI.StatuDisplayer_text(f_pos, iconsize, new Vector2(0, 7), "位置", titlesize, XHud_Utilitys.TrimParentheses(pos.ToString()), valuecol, contensize);
            Editor_XHud_GUI.StatuDisplayer_text(f_rot, iconsize, new Vector2(0, 7), "旋转", titlesize, XHud_Utilitys.TrimParentheses(eur.ToString()), valuecol, contensize);
            Editor_XHud_GUI.StatuDisplayer_text(f_sca, iconsize, new Vector2(0, 7), "缩放", titlesize, XHud_Utilitys.TrimParentheses(sca.ToString()), valuecol, contensize);
            Editor_XHud_GUI.StatuDisplayer_text(f_size, iconsize, new Vector2(0, 7), "尺寸", titlesize, XHud_Utilitys.TrimParentheses(size.ToString()), valuecol, contensize);
            Editor_XHud_GUI.StatuDisplayer_text(f_alp, iconsize, new Vector2(0, 7), "透明度", titlesize, XHud_Utilitys.TrimParentheses(alp.ToString()), valuecol, contensize);
            Editor_XHud_GUI.StatuDisplayer_text(f_fill, iconsize, new Vector2(0, 7), "填充度", titlesize, XHud_Utilitys.TrimParentheses(fil.ToString()), valuecol, contensize);
            Editor_XHud_GUI.StatuDisplayer_color(f_color, iconsize, new Vector2(0, 7), "颜色", titlesize, col);
        }
        /// <summary>
        /// 记录 PrimitiveFeature 特性参数
        /// </summary>
        private void PrimitiveFeature_Save(XHud_Module_Primitive_Feature features)
        {
            // 添加空值检查
            if (features == null || features.controller == null)
                return;

            using (SerializedObject so = new SerializedObject(features))
            {
                so.Update();

                // 获取特性类
                SerializedProperty feature = so.FindProperty("PrimitiveFeatures");
                // 获取特性类的字段
                SerializedProperty pos = feature.FindPropertyRelative("Position");
                SerializedProperty eur = feature.FindPropertyRelative("Euler");
                SerializedProperty sca = feature.FindPropertyRelative("Scale");
                SerializedProperty size = feature.FindPropertyRelative("Size");
                SerializedProperty alp = feature.FindPropertyRelative("Alpha");
                SerializedProperty col = feature.FindPropertyRelative("Color");
                SerializedProperty fil = feature.FindPropertyRelative("Fill");

                // 保存特性：Rect
                if (BaseScript.controller.mod_Rect != null)
                {
                    pos.vector3Value = BaseScript.controller.mod_Rect.anchoredPosition3D;
                    eur.vector3Value = BaseScript.controller.mod_Rect.localEulerAngles;
                    sca.vector3Value = BaseScript.controller.mod_Rect.localScale;
                    size.vector2Value = BaseScript.controller.mod_Rect.sizeDelta;
                }
                // 保存特性：CanvasGroup
                if (BaseScript.controller.mod_CanvasGroup != null)
                {
                    alp.floatValue = BaseScript.controller.mod_CanvasGroup.alpha;
                }
                // 保存特性：Image
                if (BaseScript.controller.mod_Image != null)
                {
                    col.colorValue = BaseScript.controller.mod_Image.color;
                    fil.floatValue = BaseScript.controller.mod_Image.fillAmount;
                }
                // 保存特性：Text
                else if (BaseScript.controller.mod_Text != null)
                {
                    col.colorValue = BaseScript.controller.mod_Text.color;
                }
                // 保存特性：TmpText
                else if (BaseScript.controller.mod_TmpText != null)
                {
                    col.colorValue = BaseScript.controller.mod_TmpText.color;
                }
                // 保存特性：_RawImage
                else if (BaseScript.controller.mod_RawImage != null)
                {
                    col.colorValue = BaseScript.controller.mod_RawImage.color;
                }

                so.ApplyModifiedProperties();
            }
        }
        /// <summary>
        /// 还原 PrimitiveFeature 特性参数
        /// </summary>
        private void PrimitiveFeature_Load(XHud_Module_Primitive_Feature features)
        {
            // 添加空值检查
            if (features == null || features.controller == null)
                return;

            using (SerializedObject so = new SerializedObject(features))
            {
                so.Update();

                // 获取特性类
                SerializedProperty feature = so.FindProperty("PrimitiveFeatures");
                // 获取特性类的字段
                SerializedProperty pos = feature.FindPropertyRelative("Position");
                SerializedProperty eur = feature.FindPropertyRelative("Euler");
                SerializedProperty sca = feature.FindPropertyRelative("Scale");
                SerializedProperty size = feature.FindPropertyRelative("Size");
                SerializedProperty alp = feature.FindPropertyRelative("Alpha");
                SerializedProperty col = feature.FindPropertyRelative("Color");
                SerializedProperty fil = feature.FindPropertyRelative("Fill");

                // 读取特性：Rect
                if (BaseScript.controller.mod_Rect != null)
                {
                    BaseScript.controller.mod_Rect.anchoredPosition3D = pos.vector3Value;
                    BaseScript.controller.mod_Rect.localEulerAngles = eur.vector3Value;
                    BaseScript.controller.mod_Rect.localScale = sca.vector3Value;
                    BaseScript.controller.mod_Rect.sizeDelta = size.vector2Value;
                }
                // 读取特性：CanvasGroup
                if (BaseScript.controller.mod_CanvasGroup != null)
                {
                    BaseScript.controller.mod_CanvasGroup.alpha = alp.floatValue;
                }
                // 读取特性：Image
                if (BaseScript.controller.mod_Image != null)
                {
                    BaseScript.controller.pt_Painting.UpdateColor(col.colorValue);
                    BaseScript.controller.mod_Image.color = col.colorValue;
                    BaseScript.controller.mod_Image.fillAmount = fil.floatValue;
                }
                // 读取特性：Text
                else if (BaseScript.controller.mod_Text != null)
                {
                    BaseScript.controller.mod_Text.color = col.colorValue;
                }
                // 读取特性：TmpText
                else if (BaseScript.controller.mod_TmpText != null)
                {
                    BaseScript.controller.mod_TmpText.color = col.colorValue;
                }
                // 读取特性：RawImage
                else if (BaseScript.controller.mod_RawImage != null)
                {
                    BaseScript.controller.pt_Painting.UpdateColor(col.colorValue);
                    BaseScript.controller.mod_RawImage.color = col.colorValue;
                }

                so.ApplyModifiedProperties();
            }
        }
        /// <summary>
        /// 采集 PrimitiveFeature 特性参数
        /// </summary>
        private void PrimitiveFeature_Capture(XHud_Module_Primitive_Feature[] features)
        {
            PrimitiveFeatures = new PrimitiveFeatures[features.Length];
            for (int i = 0; i < features.Length; i++)
            {
                PrimitiveFeatures[i] = features[i].PrimitiveFeatures;
            }
        }
        /// <summary>
        /// 批处理特性保存（含确认弹窗）
        /// </summary>
        private void PrimitiveFeature_Dual_Save_WithDialog()
        {
            // 多选时
            if (Targets_Selected())
            {
                List<XHud_GUI_Dialog_ListDatas> Datas = new List<XHud_GUI_Dialog_ListDatas>();

                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    PrimitiveFeature_Save(SelectedObjects[i]);
                    XHud_GUI_Dialog_ListDatas data = new XHud_GUI_Dialog_ListDatas();

                    data.Title = string.IsNullOrEmpty(SelectedObjects[i].controller.Indicator) ? SelectedObjects[i].name : SelectedObjects[i].controller.Indicator;
                    data.SubTitle = "";
                    data.Message = "已记录特性";
                    Datas.Add(data);
                }
                EditorApplication.delayCall += () =>
                {
                    Editor_XHud_GUI.Open(Datas.ToArray(), XHud_DialogType.确认, "XHud - 图元特性消息", "批量记录图元特性", "以下是批量已记录特性的所有图元列表，请您检查核对：", "明白");
                };
            }
            // 单选时
            else
            {
                PrimitiveFeature_Save(BaseScript);
                EditorApplication.delayCall += () =>
                {
                    Editor_XHud_GUI.Open(XHud_DialogType.确认, "XHud - 图元特性消息", "图元特性记录", "已记录图元的特性参数！", "明白");
                };
            }
        }
        /// <summary>
        /// 批处理特性读取（含确认弹窗）
        /// </summary>
        private void PrimitiveFeature_Dual_Load_WithDialog()
        {
            // 多选时
            if (Targets_Selected())
            {
                List<XHud_GUI_Dialog_ListDatas> Datas = new List<XHud_GUI_Dialog_ListDatas>();

                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    PrimitiveFeature_Load(SelectedObjects[i]);
                    XHud_GUI_Dialog_ListDatas data = new XHud_GUI_Dialog_ListDatas();

                    data.Title = string.IsNullOrEmpty(SelectedObjects[i].controller.Indicator) ? SelectedObjects[i].name : SelectedObjects[i].controller.Indicator;
                    data.SubTitle = "";
                    data.Message = "已读取特性";
                    Datas.Add(data);
                }

                EditorApplication.delayCall += () =>
                {
                    Editor_XHud_GUI.Open(Datas.ToArray(), XHud_DialogType.确认, "XHud - 图元特性消息", "批量读取图元特性", "以下是批量已读取特性的所有图元列表，请您检查核对：", "明白");
                };
            }
            // 单选时
            else
            {
                PrimitiveFeature_Load(BaseScript);
                EditorApplication.delayCall += () =>
                {
                    Editor_XHud_GUI.Open(XHud_DialogType.确认, "XHud - 图元特性消息", "图元特性读取", "已读取图元的特性参数！", "明白");
                };
            }
        }
        #endregion      
    }
}
