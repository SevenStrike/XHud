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
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    [System.Serializable]
    public class ReadyElementInfo
    {
        public string Name;
        public string Indicator;
        public Vector2 Size;
        public Vector3 Pos;
        public Vector3 Rot;
    }

    [System.Serializable]
    public class ReadyLibElement
    {
        [SerializeField]
        public XHud_Module_Element Element;
        [SerializeField]
        public ReadyElementInfo Info;
    }

    public class Editor_XHud_LibrarySetTool_ElementImporter : EditorWindow
    {
        private SerializedObject BaseObject;
        private SerializedProperty sp_LibName, sp_ReadySaveElementList;

        /// <summary>
        /// 数据列表
        /// </summary>
        public ReorderableList ReorderableList;
        /// <summary>
        /// Editor列表项高度
        /// </summary>
        public float itemHeight = 25;
        /// <summary>
        /// 可视区域显示的元素数量
        /// </summary>
        public int visibleItemCount = 8;
        /// <summary>
        /// 列表滚动位置
        /// </summary>
        public Vector2 DataList_Scroller;
        /// <summary>
        /// 选中项索引号
        /// </summary>
        public int SelectedIndex = 0;

        public List<ReadyLibElement> ReadySaveElementList = new List<ReadyLibElement>();

        private Texture2D icon_libsetter_element;

        /// <summary>
        /// 字体 - 粗体
        /// </summary>
        Font Font_Bold;
        /// <summary>
        /// 字体 - 细体
        /// </summary>
        Font Font_Light;

        [SerializeField]
        public string LibName;
        [SerializeField]
        public string DateTimes;

        /// <summary>
        /// 按钮宽度
        /// </summary>
        private float ButtonWidth = 110;
        /// <summary>
        /// 按钮高度
        /// </summary>
        private float ButtonHeight = 25;
        /// <summary>
        /// 按钮间距
        /// </summary>
        private float ButtonDistance = 15;

        Color SepLineColor = new Color(1, 1, 1, 0.15f);
        Color MessageColor = new Color(1, 1, 1, 0.62f);
        Color DateTimeColor = new Color(1, 1, 1, 0.42f);

        Rect Sepline_rect;
        Rect Title_rect;
        Rect Date_rect;
        Rect Icon_rect;
        Rect TotalCount_rect;
        Rect LibSelector_rect;

        private ReadyElementInfo CurrentReadyElemetInfo = new ReadyElementInfo();

        #region 将工程预制体添加到元素库
        [MenuItem("Assets/XHud/SendToElementLibrary (将元素预制体添加到元素库)", priority = 3000, validate = true)]
        private static bool Valid_OpenElementLibrarySetter()
        {
            // 获取当前选中的对象
            GameObject selectedObject = Selection.activeObject as GameObject;
            if (selectedObject != null)
            {
                XHud_Module_Element element = selectedObject.GetComponent<XHud_Module_Element>();
                if (element != null)
                {
                    // 检查选中的对象是否为 Hud_Element 类型
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        [MenuItem("Assets/XHud/SendToElementLibrary (将元素预制体添加到元素库)", priority = 3000)]
        private static void OpenElementLibrarySetter()
        {
            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

            if (mgr == null)
            {
                Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 元素库添加器消息", "寻找Hud管理器", "场景中未找到HudManager管理器！", "明白");
                return;
            }
            Transform[] sel_ele_objs = Selection.GetTransforms(SelectionMode.Unfiltered);

            List<XHud_Module_Element> Hud_Elements = new List<XHud_Module_Element>();

            for (int i = 0; i < sel_ele_objs.Length; i++)
            {
                XHud_Module_Element sel_ele = sel_ele_objs[i].GetComponent<XHud_Module_Element>();

                if (mgr.hm_ElementLibrary_GetCount() <= 0)
                {
                    Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 元素库添加器消息", "元素库未配置", "您未在HudManager中配置元素库！请先至少指定一个元素库后再执行此操作！ ", "明白");
                    return;
                }

                Hud_Elements.Add(sel_ele);
            }

            Editor_XHud_LibrarySetTool_ElementImporter window = EditorWindow.GetWindow<Editor_XHud_LibrarySetTool_ElementImporter>(true);
            window.titleContent = new GUIContent("XHud 元素库添加器");
            Editor_XHud_GUI.CenterEditorWindow(new Vector2Int(620, 450), window);

            // 将要存入元素库的物体信息发送至窗口
            window.SetElements(Hud_Elements);
            window.Show();
        }

        #endregion

        #region DataList

        Rect index_rect;
        Rect indicator_rect;
        Rect scrollview_rect;
        Rect item_rect;
        Rect itemmark_rect;
        Rect itemmarkBg_rect;
        Rect readyelementinfo_rect;

        private void Remove(ReorderableList list)
        {
            Debug.Log(1);
        }

        /// <summary>
        /// 绘制元素
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="index"></param>
        /// <param name="isActive"></param>
        /// <param name="isFocused"></param>
        private void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            index_rect = new Rect(rect.x + 15, rect.y + 5, 30, 20);
            indicator_rect = new Rect(rect.x + 39, rect.y + 5, 180, 20);

            SerializedProperty prop = sp_ReadySaveElementList.GetArrayElementAtIndex(index);
            SerializedProperty prop_Info = prop.FindPropertyRelative("Info");
            SerializedProperty sp_name = prop_Info.FindPropertyRelative("Name");

            Editor_XHud_GUI.Gui_Labelfield(index_rect, index.ToString("D2"), HudFilled.无, HudColor.无, Color.gray, TextAnchor.MiddleLeft, Vector2.zero, 11);

            #region 标识名称
            Editor_XHud_GUI.Gui_Labelfield(indicator_rect, sp_name.stringValue, HudFilled.无, HudColor.无, Editor_XHud_GUI.GetColor(HudColor.亮白), TextAnchor.MiddleLeft, Vector2.zero, 12, TextClipping.Ellipsis);
            #endregion
        }

        private void DeleteElementAtIndex(int index)
        {
            if (index < 0 || index >= sp_ReadySaveElementList.arraySize)
                return;

            // 删除项
            sp_ReadySaveElementList.DeleteArrayElementAtIndex(index);
            sp_ReadySaveElementList.serializedObject.ApplyModifiedProperties();

            // 调整选中索引
            if (SelectedIndex >= sp_ReadySaveElementList.arraySize)
            {
                SelectedIndex = sp_ReadySaveElementList.arraySize - 1;
            }

            // 刷新 UI
            Repaint();
        }

        /// <summary>
        /// 绘制列表
        /// </summary>
        private void DrawDataList()
        {
            // 绘制滚动视图
            scrollview_rect = GUILayoutUtility.GetRect(0, visibleItemCount * itemHeight);
            scrollview_rect.x = 18;
            scrollview_rect.width = 300;
            scrollview_rect.height = 290;
            DataList_Scroller = GUI.BeginScrollView(scrollview_rect, DataList_Scroller, new Rect(0, 0, scrollview_rect.width - 50, ReadySaveElementList.Count * itemHeight), false, true);

            // 计算可视区域的起始和结束索引
            int startIndex = Mathf.FloorToInt(DataList_Scroller.y / itemHeight);
            int endIndex = Mathf.CeilToInt((DataList_Scroller.y + scrollview_rect.height) / itemHeight);

            // 只绘制可视区域内的元素
            for (int i = startIndex; i < endIndex && i < ReorderableList.count; i++)
            {
                SerializedProperty prop = sp_ReadySaveElementList.GetArrayElementAtIndex(i);

                item_rect = new Rect(0, i * itemHeight, scrollview_rect.width, itemHeight);

                // 如果当前元素被选中，绘制选中效果
                if (SelectedIndex == i)
                {
                    itemmark_rect = new Rect(item_rect.x + 1, item_rect.y + 11, 5, 5);
                    // 高亮标记表示选中
                    EditorGUI.DrawRect(itemmark_rect, XHud_Dashboard.Theme_Primary);
                    itemmarkBg_rect = new Rect(item_rect.x, item_rect.y, item_rect.width + 20, item_rect.height);
                    // 高亮背景表示选中
                    EditorGUI.DrawRect(itemmarkBg_rect, new Color(0, 0, 0, 0.2f));
                }

                ReorderableList.drawElementCallback.Invoke(item_rect, i, i == ReorderableList.index, true);

                // 检测鼠标是否在当前元素区域内
                if (item_rect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.MouseDown)
                    {
                        // 更新选中项
                        SelectedIndex = i;

                        CurrentReadyElemetInfo = ReadySaveElementList[SelectedIndex].Info;

                        // 标记界面需要更新
                        GUI.changed = true;
                    }
                }

                // 检查当前事件是否为键盘事件
                if (Event.current.type == EventType.KeyDown)
                {
                    // 检测是否按下 Delete 键
                    if (Event.current.keyCode == KeyCode.Delete)
                    {
                        // 确保有选中的项
                        if (SelectedIndex >= 0 && SelectedIndex < sp_ReadySaveElementList.arraySize)
                        {
                            // 删除选中的项
                            DeleteElementAtIndex(SelectedIndex);

                            // 标记事件已处理，防止其他逻辑重复处理
                            Event.current.Use();
                        }
                    }
                }

                // 绘制元素
                //EditorGUI.PropertyField(item_rect, prop, GUIContent.none);
            }

            GUI.EndScrollView();
        }
        #endregion

        private void OnDisable()
        {

        }

        private void OnEnable()
        {
            BaseObject = new SerializedObject(this);
            sp_LibName = BaseObject.FindProperty("LibName");
            sp_ReadySaveElementList = BaseObject.FindProperty("ReadySaveElementList");

            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

            icon_libsetter_element = Editor_XHud_GUI.GetIcon("LibSetter/icon_libsetter_element");

            Font_Bold = Editor_XHud_GUI.GetFont("SS_Editor_Bold");
            Font_Light = Editor_XHud_GUI.GetFont("SS_Editor_Dialog");

            CurrentReadyElemetInfo = new ReadyElementInfo();

            #region ReorderableList
            ReorderableList = new ReorderableList(BaseObject, sp_ReadySaveElementList, true, true, true, true);
            ReorderableList.drawElementCallback = DrawElementCallback;
            ReorderableList.onRemoveCallback = Remove;
            #endregion
        }


        private void OnGUI()
        {
            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

            BaseObject.Update();

            Rect rect = new Rect(0, 0, position.width, position.height);

            Icon_rect = new Rect(26, 15, 48, 48);

            Editor_XHud_GUI.Gui_Icon(Icon_rect, icon_libsetter_element);

            Title_rect = new Rect(rect.x + 100, rect.y + 15, rect.width - 80, 30);
            Editor_XHud_GUI.Gui_Labelfield(Title_rect, "元素库添加器", HudFilled.无, HudColor.无, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 20, Font_Bold);

            Sepline_rect = new Rect(rect.x + 102, rect.y + 60, 200, 1);
            Editor_XHud_GUI.Gui_Box(Sepline_rect, SepLineColor);

            Editor_XHud_GUI.Gui_Labelfield_Thin_WrapClip(new Rect(rect.x + 26, rect.y + 80, rect.width - 45, rect.height), "以下列表中的元素为待入库的Hud元素物体，您可以检查或按Delete键剔除不需要入库的选中元素项，点击每一项可查看此项元素的基础信息", HudFilled.无, HudColor.无, MessageColor, TextAnchor.UpperLeft, new Vector2(0, 0), 12, true, Font_Light);

            DateTimes = DateTime.Now.ToString("yyyy-MM-dd  HH:mm:ss:ff");
            Date_rect = new Rect(rect.x + 150, rect.y + 15, rect.width - 180, rect.height);
            Editor_XHud_GUI.Gui_Labelfield_Thin_WrapClip(Date_rect, DateTimes, HudFilled.无, HudColor.无, DateTimeColor, TextAnchor.UpperRight, new Vector2(0, 0), 13, true, Font_Light);

            TotalCount_rect = new Rect(rect.width - 300, rect.y + 15, 80, 30);
            Editor_XHud_GUI.Gui_Labelfield(TotalCount_rect, ReadySaveElementList.Count.ToString(), HudFilled.无, HudColor.无, XHud_Dashboard.Theme_Primary, TextAnchor.MiddleRight, Vector2.zero, 17, Font_Bold);

            Editor_XHud_GUI.Gui_Labelfield(new Rect(TotalCount_rect.x + TotalCount_rect.width + 7, TotalCount_rect.y + 1, 20, TotalCount_rect.height), " 项", HudFilled.无, HudColor.无, Color.gray, TextAnchor.MiddleLeft, Vector2.zero, 14, Font_Bold);

            #region 库选择
            string[] libnames = mgr.hm_ElementLibrary_GetAllLibraryNames();
            LibSelector_rect = new Rect(rect.width - 275, rect.y + 140, 255, 25);
            Editor_XHud_GUI.Gui_PopupWithString(LibSelector_rect, ref sp_LibName, libnames, HudFilled.实体, HudColor.深空灰, Color.white);
            sp_LibName.serializedObject.ApplyModifiedProperties();
            #endregion

            #region 选中的元素信息
            string colorhex = XHud_Utilitys.Color_To_HexColor(XHud_Dashboard.Theme_Primary, true);

            readyelementinfo_rect = new Rect(rect.width - 270, rect.y + 185, 200, 15);
            Editor_XHud_GUI.Gui_Labelfield(readyelementinfo_rect, "元素信息：", HudFilled.无, HudColor.无, Color.white, false, Color.blue, TextAnchor.UpperLeft, Vector2.zero, 12, Font_Bold);

            if (ReadySaveElementList != null && ReadySaveElementList.Count > 0)
            {
                SelectedElementInfoViewer(rect, 220, $" 名称： <color={colorhex}>{ReadySaveElementList[SelectedIndex].Info.Name}</color>");
                if (string.IsNullOrEmpty(ReadySaveElementList[SelectedIndex].Info.Indicator))
                    SelectedElementInfoViewer(rect, 250, $" 标识： <color={colorhex}>未设置标识名称</color>");
                else
                    SelectedElementInfoViewer(rect, 250, $" 标识： <color={colorhex}>{ReadySaveElementList[SelectedIndex].Info.Indicator}</color>");
                SelectedElementInfoViewer(rect, 280, $" 尺寸： <color={colorhex}>{ReadySaveElementList[SelectedIndex].Info.Size}</color>");
                SelectedElementInfoViewer(rect, 310, $" 位置： <color={colorhex}>{ReadySaveElementList[SelectedIndex].Info.Pos}</color>");
                SelectedElementInfoViewer(rect, 340, $" 旋转： <color={colorhex}>{ReadySaveElementList[SelectedIndex].Info.Rot}</color>");
            }
            #endregion

            #region 列表

            Editor_XHud_GUI.Gui_Layout_Space(140);

            DrawDataList();

            BaseObject.ApplyModifiedProperties();
            #endregion

            Repaint();

            Editor_XHud_GUI.Gui_Layout_Space(65);
            DialogType_Buttons();

            Event e = Event.current;
            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
            {
                e.Use();
                Close();
            }
        }

        private void SelectedElementInfoViewer(Rect rect, float offset, string content)
        {
            readyelementinfo_rect = new Rect(rect.width - 275, rect.y + offset, 200, 15);
            Editor_XHud_GUI.Gui_Labelfield(readyelementinfo_rect, content, HudFilled.无, HudColor.无, Color.gray, TextAnchor.UpperLeft, Vector2.zero, 12, true, TextClipping.Ellipsis, true, Font_Light);
        }

        /// <summary>
        /// 发送到元素库
        /// </summary>
        private void SendToLibrary()
        {
            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

            XHud_Library_Element target_library = mgr.hm_ElementLibrary_GetTargetLibrary(sp_LibName.stringValue);

            List<XHud_GUI_Dialog_ListDatas> Datas = new List<XHud_GUI_Dialog_ListDatas>();

            string colorhex = XHud_Utilitys.Color_To_HexColor(XHud_Dashboard.Theme_Primary, true);

            for (int s = 0; s < ReadySaveElementList.Count; s++)
            {
                XHud_GUI_Dialog_ListDatas dataitem = new XHud_GUI_Dialog_ListDatas();

                if (target_library.ElementLibrary_IsExist(ReadySaveElementList[s].Element.name))
                {
                    string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 元素库添加器消息", "存在重复元素名称", $"名称为<color={colorhex}> {ReadySaveElementList[s].Element.transform.name} </color>的元素已经存在于<color={colorhex}> \" {sp_LibName.stringValue} \" </color>元素库中，是否以当前元素更新替换库中的元素？", "更新替换", "跳过", 1);
                    if (res == "更新替换")
                    {
                        target_library.ElementsLibrary_ReplaceElement(ReadySaveElementList[s].Element);

                        dataitem.Title = ReadySaveElementList[s].Element.transform.name;
                        dataitem.SubTitle = "替换库中元素";
                        dataitem.Message = sp_LibName.stringValue;
                        Datas.Add(dataitem);
                    }
                }
                else
                {
                    target_library.ElementsLibrary_Add(ReadySaveElementList[s].Element, 1);

                    dataitem.Title = ReadySaveElementList[s].Element.transform.name;
                    dataitem.SubTitle = "新增元素到库";
                    dataitem.Message = sp_LibName.stringValue;

                    Datas.Add(dataitem);
                }
            }

            if (Datas.Count > 0)
                Editor_XHud_GUI.Open(Datas.ToArray(), XHud_DialogType.确认, "XHud - 元素库添加器消息", "元素入库", "以下是已被新增入库和被替换已有的元素列表，请您检查核对：", "明白");
        }

        /// <summary>
        /// 设置元素
        /// </summary>
        /// <param name="eles"></param>
        public void SetElements(List<XHud_Module_Element> eles)
        {
            if (ReadySaveElementList == null)
                ReadySaveElementList = new List<ReadyLibElement>();

            for (int i = 0; i < eles.Count; i++)
            {
                ReadyLibElement ready = new ReadyLibElement();
                ready.Element = eles[i];

                ready.Info = new ReadyElementInfo();
                ready.Info.Name = eles[i].name;
                ready.Info.Indicator = eles[i].Indicator;
                ready.Info.Pos = eles[i].RectTransform.anchoredPosition3D;
                ready.Info.Size = eles[i].RectTransform.sizeDelta;
                ready.Info.Rot = eles[i].RectTransform.localEulerAngles;

                ReadySaveElementList.Add(ready);
            }
        }

        /// <summary>
        /// 控件按钮
        /// </summary>
        private void DialogType_Buttons()
        {
            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_XHud_GUI.Gui_Layout_FlexSpace();

            if (Editor_XHud_GUI.Gui_Layout_Button("关闭", "", HudFilled.实体, HudColor.亮白, Color.black, 12, ButtonWidth, ButtonHeight, Font_Light, "关闭"))
            {
                Close();
            }
            GUI.backgroundColor = Color.white;
            Editor_XHud_GUI.Gui_Layout_Space(ButtonDistance);
            GUI.backgroundColor = XHud_Dashboard.Theme_Primary;
            if (Editor_XHud_GUI.Gui_Layout_Button("添加", "", HudFilled.实体, HudColor.亮白, XHud_Utilitys.GetBrightnessLimite(XHud_Dashboard.Theme_Primary) ? Color.black : Color.white, 12, ButtonWidth, ButtonHeight, Font_Light, "添加"))
            {
                SendToLibrary();
                Close();
                return;
            }
            GUI.backgroundColor = Color.white;

            Editor_XHud_GUI.Gui_Layout_Space(25);
            Editor_XHud_GUI.Gui_Layout_Horizontal_End();
        }
    }
}
