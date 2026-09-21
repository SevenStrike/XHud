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
    using SevenStrikeModules.XGUI.Runtime;
    using SevenStrikeModules.XHud.Enums;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(XHud_LayoutSpawner_ValueBinder), true)]
    public class Editor_XHud_LayoutSpawner_ValueBinder : Editor
    {
        private XHud_LayoutSpawner_ValueBinder BaseScript;

        #region 序列化属性
        private SerializedProperty UseDebug;
        #endregion

        #region 图标
        private Texture2D
            icon_main,
            icon_add_r,
            icon_add_p,
            icon_edge;
        #endregion

        /// <summary>
        /// 字体 - 细体
        /// </summary>
        Font Font_Light;
        /// <summary>
        /// 字体 - 粗体
        /// </summary>
        Font Font_Bold;

        #region 选项文字
        string[] stroptions_enabled = new string[2] { "关闭", "开启" };
        #endregion

        #region 批量化操作
        XHud_LayoutSpawner_ValueBinder[] SelectedObjects;

        private void Targets_Get()
        {
            if (targets.Length > 1)
            {
                SelectedObjects = new XHud_LayoutSpawner_ValueBinder[targets.Length];
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    var t = targets[i];
                    SelectedObjects[i] = (XHud_LayoutSpawner_ValueBinder)t;
                }
            }
            else
            {
                SelectedObjects = new XHud_LayoutSpawner_ValueBinder[targets.Length];
                SelectedObjects[0] = (XHud_LayoutSpawner_ValueBinder)target;
            }
        }

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
            BaseScript = (XHud_LayoutSpawner_ValueBinder)target;

            Targets_Get();

            GetSerializeFields();

            Font_Light = Editor_XHud_GUI.GetFont("sx_regular");
            Font_Bold = Editor_XHud_GUI.GetFont("sx_bold");

            #region 获取图标
            icon_main = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner_ValueBinder/icon_main");
            icon_add_r = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner_ValueBinder/icon_add_r");
            icon_add_p = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner_ValueBinder/icon_add_p");
            icon_edge = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner_ValueBinder/icon_edge");
            #endregion

            BaseScript.layoutspawner = BaseScript.GetComponent<XHud_LayoutSpawner>();


            if (BaseScript.layoutspawner != null)
            {
                BaseScript.layoutspawner.act_on_spawn += layoutspawner_act_on_spawn;
                BaseScript.layoutspawner.act_on_despawn += layoutspawner_act_on_despawn;
            }
        }

        private void OnDisable()
        {
            if (BaseScript.layoutspawner != null)
            {
                BaseScript.layoutspawner.act_on_spawn -= layoutspawner_act_on_spawn;
                BaseScript.layoutspawner.act_on_despawn -= layoutspawner_act_on_despawn;
            }
        }

        private void layoutspawner_act_on_spawn(XHudSpace space)
        {
            if (Application.isPlaying)
                return;

            for (int i = 0; i < BaseScript.valuebinders.Length; i++)
            {
                ElementBinder valuebinder = BaseScript.valuebinders[i];

                if (!valuebinder.IsEnabled)
                    continue;

                // 根据ID查找元素
                XHud_Module_Element element = BaseScript.layoutspawner.GetElement_With_ID(space, valuebinder.ID);

                if (element == null && UseDebug.boolValue)
                {
                    string hex_col = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary, true);
                    XGUI_Utilitys.Console("XHud - 布局元素数据绑定器通知", $"未找到匹配ID<color={hex_col}> {valuebinder.ID} </b></color>的元素项！请确认布局元素生成器<b><color={hex_col}>  {space.ToString()}  </b></color>列表中存在ID为：<b><color={hex_col}>  {valuebinder.ID} </b></color>的布局元素数据！", XGUIMsgState.通知);
                }

                // 遍历绑定器中的图元控制器绑定器列表来和元素下的所有图元控制器匹配ID
                for (int w = 0; w < valuebinder.binders.Length; w++)
                {
                    PrimitiveBinder binder = valuebinder.binders[w];

                    BaseScript.ModifiyValue(element, binder);
                }
            }
        }

        private void layoutspawner_act_on_despawn(XHudSpace space)
        {
            if (Application.isPlaying)
                return;
            //Debug.Log("模拟回收");
        }

        private bool isDraggingOver = false;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            Editor_XHud_GUI.Gui_Layout_Banner(icon_main, HudFilled.实体, HudColor.深空灰, "XHud - 布局元素数据绑定器", Color.white);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            #region 编辑数据绑定
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 0, "可视化编辑", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            if (Editor_XHud_GUI.Gui_Layout_Button("编辑", "在独立窗口中进行布局元素数据绑定操作", HudFilled
                .实体, HudColor.深空灰, XHud_Dashboard.Theme_Primary, 30))
            {
                Editor_XHud_LayoutSpawner_ValueBinder_SetTool window = EditorWindow.GetWindow<Editor_XHud_LayoutSpawner_ValueBinder_SetTool>(false);

                window.titleContent = new GUIContent("XHud 布局元素数据绑定器");
                Editor_XHud_GUI.CenterEditorWindow(new Vector2Int(420, 620), window);
            }

            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 选项
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 0, "选项", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_LayoutSpawner_ValueBinder>("调试信息", stroptions_enabled, ref UseDebug, HudFilled.无, HudFilled.实体, Color.white * 0.9f, 120, 22, SelectedObjects);

            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 元素项拖放区域
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 0, "布局元素数据拖放区域", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            Rect rect_drop = GUILayoutUtility.GetRect(200, 90);
            if (Event.current.type == EventType.DragUpdated)
            {
                if (rect_drop.Contains(Event.current.mousePosition))
                {
                    if (!isDraggingOver)
                    {
                        //Debug.Log("拖入区域");
                        isDraggingOver = true;
                    }
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                }
                else
                {
                    if (isDraggingOver)
                    {
                        //Debug.Log("离开区域");
                        isDraggingOver = false;
                    }
                }
                Event.current.Use();
            }
            else if (Event.current.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();
                string data = ((string)DragAndDrop.GetGenericData("LayoutSpawnerBindData"));

                // 获取拖入的元素项
                XHud_LayoutSpawner_Item item = JsonUtility.FromJson<XHud_LayoutSpawner_Item>(data);

                if (item == null)
                    return;

                Undo.RecordObject(BaseScript, "BindChanged");

                // 检查校验有效性
                for (int i = 0; i < BaseScript.valuebinders.Length; i++)
                {
                    ref var e_binder = ref BaseScript.valuebinders[i];

                    if (e_binder.ID != item.ID)
                        continue;

                    // 过滤掉无效的 binder
                    var validBinders = new List<PrimitiveBinder>();
                    foreach (var binder in e_binder.binders)
                    {
                        var con = item.SpawnedElementNode.Element.GetPrimitiveController_With_ID(binder.ID);
                        if (con != null)
                            validBinders.Add(binder);
                    }

                    e_binder.binders = validBinders.ToArray();
                    // 因为是 ref，不需要再赋值回去
                }

                BindValue_Element(item);

                isDraggingOver = false;
                Repaint();
                Event.current.Use();
            }

            Rect rect_drop_visual = new Rect(rect_drop.x, rect_drop.y, rect_drop.width, rect_drop.height);

            if (isDraggingOver)
            {
                Rect rect_drop_visual_icon = new Rect(rect_drop_visual.x + 5, rect_drop_visual.y + 5, rect_drop_visual.width - 10, rect_drop_visual.height - 10);
                Editor_XHud_GUI.Gui_Icon(rect_drop_visual_icon, icon_edge, new RectOffset(25, 25, 25, 25), XHud_Dashboard.Theme_Primary);
                Editor_XHud_GUI.Gui_Box(rect_drop_visual_icon, XHud_Dashboard.Theme_Primary * 0.2f);

                Rect rect_drop_visual_text = new Rect(rect_drop_visual.x + 5, rect_drop_visual.y + 5, rect_drop_visual.width - 10, rect_drop_visual.height - 10);
                Editor_XHud_GUI.Gui_Labelfield(rect_drop_visual_text, "添加到数据绑定列表", HudFilled.无, HudColor.无, XHud_Dashboard.Theme_Primary, TextAnchor.MiddleCenter, 12, Font_Bold);
            }
            else
            {
                Rect rect_drop_visual_icon = new Rect(rect_drop_visual.x + 5, rect_drop_visual.y + 5, rect_drop_visual.width - 10, rect_drop_visual.height - 10);
                Editor_XHud_GUI.Gui_Icon(rect_drop_visual_icon, icon_edge, new RectOffset(25, 25, 25, 25), Color.white * 0.65f);

                Rect rect_drop_visual_text = new Rect(rect_drop_visual.x + 5, rect_drop_visual.y + 5, rect_drop_visual.width - 10, rect_drop_visual.height - 10);
                Editor_XHud_GUI.Gui_Labelfield(rect_drop_visual_text, "拖放布局元素数据至此", HudFilled.无, HudColor.无, Color.white * 0.75f, TextAnchor.MiddleCenter, 12, Font_Light);
            }

            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            base.OnInspectorGUI();

            serializedObject.ApplyModifiedProperties();
        }

        public void BindValue_Element(XHud_LayoutSpawner_Item item)
        {
            XHud_Module_Element ele = item.SpawnedElementNode.Element;
            string hex_col = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary, true);

            // 用于覆盖原数据
            List<ElementBinder> BindList = new List<ElementBinder>();

            // 检测是否已经绑定了目标元素项
            for (int i = 0; i < BaseScript.valuebinders.Length; i++)
            {
                ElementBinder ele_binder = BaseScript.valuebinders[i];

                BindList.Add(ele_binder);

                // 如果绑定了那么就完善未绑定的图元
                if (ele_binder.ID == item.ID)
                {
                    if (string.IsNullOrEmpty(ele_binder.Name) || ele_binder.Name != item.SpawnName)
                        ele_binder.Name = item.SpawnName;

                    if (ele != null)
                    {
                        ele_binder = BindValue_Primitives(ele_binder, ele);
                    }

                    BaseScript.valuebinders[i] = ele_binder;

                    if (ele_binder.binders.Length <= 0)
                    {
                        XGUI_Utilitys.Console("XHud - 布局元素数据绑定器通知", $"注意！ID为：<b><color={hex_col}> {item.ID} </b></color>的目标布局元素数据已经绑定，<b><color={hex_col}>请勿重复绑定！</b></color>此操作已被跳过！", XGUIMsgState.通知);
                    }
                    else
                    {
                        XGUI_Utilitys.Console("XHud - 布局元素数据绑定器通知", $"注意！ID为：<b><color={hex_col}> {item.ID} </b></color>的目标布局元素数据已经绑定，<b><color={hex_col}>因为您之前绑定过该元素ID</b></color>，但当时可能其下并没有任何图元！所以完善了其下的图元数据的绑定！</b></color>", XGUIMsgState.通知);
                    }

                    return;
                }
            }

            // 能执行到此处则表示没有任何图元被绑定

            // 加入绑定列表
            ElementBinder x_ele_binder = new ElementBinder();

            // 新建的绑定器的ID为拖入的元素项的加载的元素的ID
            x_ele_binder.ID = item.ID;
            x_ele_binder.Name = item.SpawnName;

            // 如果元素未载入，只绑定元素项自身ID，后续不处理元素下的图元数据绑定，否则则跳过这一步判断
            if (item.SpawnedElementNode.Element == null && UseDebug.boolValue)
            {
                XGUI_Utilitys.Console("XHud - 布局元素数据绑定器通知", $"注意！ID为：<b><color={hex_col}> {item.ID} </b></color>的目标布局元素并未加载到场景！所以只能绑定元素基础信息而不能绑定元素下的图元控制器数据！", XGUIMsgState.警告);
                return;
            }

            if (ele != null)
            {
                // 处理元素下的图元数据绑定
                x_ele_binder = BindValue_Primitives(x_ele_binder, ele);
            }

            BindList.Add(x_ele_binder);
            BaseScript.valuebinders = BindList.ToArray();
        }

        /// <summary>
        /// 绑定元素下所有图元的ID
        /// </summary>
        /// <param name="ele_binder"></param>
        /// <param name="ele"></param>
        /// <returns></returns>
        private ElementBinder BindValue_Primitives(ElementBinder ele_binder, XHud_Module_Element ele)
        {
            if (ele.PrimitiveControllerNodes.Count <= 0)
                return ele_binder;

            #region 如果图元绑定器不为空，先储存现有的图元绑定器
            List<PrimitiveBinder> prim_binders = new List<PrimitiveBinder>();
            if (ele_binder.binders != null)
            {
                for (int i = 0; i < ele_binder.binders.Length; i++)
                {
                    prim_binders.Add(ele_binder.binders[i]);
                }
            }
            #endregion

            // 遍历元素下的所有图元控制器，如果在现有的绑定其中找不到则添加
            for (int i = 0; i < ele.PrimitiveControllerNodes.Count; i++)
            {
                XHud_Module_Primitive_Controller con = ele.PrimitiveControllerNodes[i].Controller;

                if (con == null)
                    continue;

                #region 判断是否已经存在？如果存在在判定是否名称已赋值？
                bool exist = false;
                for (int a = 0; a < prim_binders.Count; a++)
                {
                    if (prim_binders[a].ID == con.ID)
                    {
                        PrimitiveBinder binder = prim_binders[a];

                        exist = true;

                        // 如果图元绑定关系已存在，则根据情况更新绑定名称
                        if (prim_binders[a].Name != con.transform.name)
                        {
                            binder.Name = con.transform.name;
                        }

                        if (binder.type == ModuleType.Image)
                        {
                            if (con.pt_Painting != null &&
                                con.pt_Painting.SyncImageColor &&
                                !con.pt_Painting.SyncLibraryColor)
                            {
                                binder.val_color = con.pt_Painting.OriginalColor;
                            }
                            else
                            {
                                binder.val_color = con.mod_Image.color;
                            }
                        }
                        if (binder.type == ModuleType.RawImage)
                        {
                            if (con.pt_Painting != null &&
                                con.pt_Painting.SyncImageColor &&
                                !con.pt_Painting.SyncLibraryColor)
                            {
                                binder.val_color = con.pt_Painting.OriginalColor;
                            }
                            else
                            {
                                binder.val_color = con.mod_RawImage.color;
                            }
                        }
                        if (binder.type == ModuleType.Text)
                        {
                            if (con.pt_Painting != null &&
                                con.pt_Painting.SyncImageColor &&
                                !con.pt_Painting.SyncLibraryColor)
                            {
                                if (con.mod_Text.TextStyleInfo.SyncPrimitivePaintingColor)
                                    binder.val_color = con.pt_Painting.OriginalColor;
                            }
                            else
                                binder.val_color = con.mod_Text.TextStyleInfo.FontColor;
                        }
                        if (binder.type == ModuleType.TmpText)
                        {
                            if (con.pt_Painting != null &&
                                con.pt_Painting.SyncImageColor &&
                                !con.pt_Painting.SyncLibraryColor)
                            {
                                if (con.mod_TmpText.TextStyleInfo.SyncPrimitivePaintingColor)
                                    binder.val_color = con.pt_Painting.OriginalColor;
                            }
                            else
                                binder.val_color = con.mod_TmpText.TextStyleInfo.tmp_color;
                        }

                        prim_binders[a] = binder;

                        break;
                    }
                }
                #endregion

                // 如果不存在则加入列表，准备覆盖回原图元绑定列表
                if (!exist)
                {
                    PrimitiveBinder p_binder = new PrimitiveBinder();
                    p_binder.Name = con.transform.name;
                    p_binder.ID = con.ID;
                    p_binder.type = con.GetModuleType();

                    if (p_binder.type == ModuleType.Image)
                    {
                        if (con.pt_Painting != null && con.pt_Painting.SyncImageColor && !con.pt_Painting.SyncLibraryColor)
                            p_binder.val_color = con.pt_Painting.OriginalColor;
                        else
                            p_binder.val_color = con.mod_Image.color;
                    }
                    if (p_binder.type == ModuleType.RawImage)
                    {
                        if (con.pt_Painting != null && con.pt_Painting.SyncImageColor && !con.pt_Painting.SyncLibraryColor)
                            p_binder.val_color = con.pt_Painting.OriginalColor;
                        else
                            p_binder.val_color = con.mod_RawImage.color;
                    }
                    if (p_binder.type == ModuleType.Text)
                    {
                        if (con.pt_Painting != null && con.pt_Painting.SyncImageColor && !con.pt_Painting.SyncLibraryColor)
                        {
                            if (con.mod_Text.TextStyleInfo.SyncPrimitivePaintingColor)
                                p_binder.val_color = con.pt_Painting.OriginalColor;
                        }
                        else
                            p_binder.val_color = con.mod_Text.TextStyleInfo.FontColor;
                    }
                    if (p_binder.type == ModuleType.TmpText)
                    {
                        if (con.pt_Painting != null && con.pt_Painting.SyncImageColor && !con.pt_Painting.SyncLibraryColor)
                        {
                            if (con.mod_TmpText.TextStyleInfo.SyncPrimitivePaintingColor)
                                p_binder.val_color = con.pt_Painting.OriginalColor;
                        }
                        else
                            p_binder.val_color = con.mod_TmpText.TextStyleInfo.tmp_color;
                    }
                    prim_binders.Add(p_binder);
                }
            }
            ele_binder.binders = prim_binders.ToArray();

            return ele_binder;
        }

        /// <summary>
        /// 获取序列化变量
        /// </summary>
        private void GetSerializeFields()
        {
            UseDebug = serializedObject.FindProperty("UseDebug");
        }
    }
}
