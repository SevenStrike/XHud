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
    using SevenStrikeModules.XGUI.Editor;
    using SevenStrikeModules.XGUI.Runtime;
    using SevenStrikeModules.XHud.Enums;
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(XHud_LayoutSpawner_ValueBinder), true)]
    public class Editor_XHud_LayoutSpawner_ValueBinder : Editor
    {
        #region 字段 - 脚本引用
        /// <summary>
        /// 布局生成器数值绑定器脚本引用
        /// </summary>
        private XHud_LayoutSpawner_ValueBinder base_script;
        #endregion

        #region 序列化属性
        /// <summary>
        /// 调试开关属性
        /// </summary>
        private SerializedProperty sp_use_debug;
        #endregion

        #region 图标
        /// <summary>
        /// 主图标
        /// </summary>
        private Texture2D icon_main;
        /// <summary>
        /// 添加按钮常态图标
        /// </summary>
        private Texture2D icon_add_r;
        /// <summary>
        /// 添加按钮按下图标
        /// </summary>
        private Texture2D icon_add_p;
        /// <summary>
        /// 边缘图标
        /// </summary>
        private Texture2D icon_edge;
        #endregion

        private bool is_dragging_over = false;

        #region 选项文字
        string[] stroptions_enabled = new string[2] { "关闭", "开启" };
        #endregion

        #region 批量化操作
        XHud_LayoutSpawner_ValueBinder[] selected_objects;

        private void Targets_Get()
        {
            if (targets.Length > 1)
            {
                selected_objects = new XHud_LayoutSpawner_ValueBinder[targets.Length];
                for (int i = 0; i < selected_objects.Length; i++)
                {
                    var t = targets[i];
                    selected_objects[i] = (XHud_LayoutSpawner_ValueBinder)t;
                }
            }
            else
            {
                selected_objects = new XHud_LayoutSpawner_ValueBinder[targets.Length];
                selected_objects[0] = (XHud_LayoutSpawner_ValueBinder)target;
            }
        }

        private bool Targets_Selected()
        {
            if (selected_objects == null)
                return false;
            if (selected_objects.Length > 1)
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
            base_script = (XHud_LayoutSpawner_ValueBinder)target;

            Targets_Get();

            #region 获取序列化属性
            sp_use_debug = serializedObject.FindProperty(nameof(XHud_LayoutSpawner_ValueBinder.use_debug));
            #endregion

            #region 获取图标
            icon_main = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_layout_spawner_value_binder/icon_main");
            icon_add_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_layout_spawner_value_binder/icon_add_r");
            icon_add_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_layout_spawner_value_binder/icon_add_p");
            icon_edge = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_layout_spawner_value_binder/icon_edge");
            #endregion

            base_script.layoutspawner = base_script.GetComponent<XHud_LayoutSpawner>();

            if (base_script.layoutspawner != null)
            {
                base_script.layoutspawner.act_on_spawn += layoutspawner_act_on_spawn;
                base_script.layoutspawner.act_on_despawn += layoutspawner_act_on_despawn;
            }
        }

        private void OnDisable()
        {
            if (base_script.layoutspawner != null)
            {
                base_script.layoutspawner.act_on_spawn -= layoutspawner_act_on_spawn;
                base_script.layoutspawner.act_on_despawn -= layoutspawner_act_on_despawn;
            }
        }

        private void layoutspawner_act_on_spawn(XHudSpace space)
        {
            if (Application.isPlaying)
                return;

            for (int i = 0; i < base_script.valuebinders.Length; i++)
            {
                ElementBinder valuebinder = base_script.valuebinders[i];

                if (!valuebinder.is_enabled)
                    continue;

                // 根据ID查找元素
                XHud_Module_Element element = base_script.layoutspawner.GetElement_With_ID(space, valuebinder.id);

                if (element == null && sp_use_debug.boolValue)
                {
                    string hex_col = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.themes_get_color(ThemeColorType.primary), true);
                    XGUI_Utilitys.Console("XHud - 布局元素数据绑定器通知", $"未找到匹配ID<color={hex_col}> {valuebinder.id} </b></color>的元素项！请确认布局元素生成器<b><color={hex_col}>  {space.ToString()}  </b></color>列表中存在ID为：<b><color={hex_col}>  {valuebinder.id} </b></color>的布局元素数据！", XGUIMsgState.通知);
                }

                // 遍历绑定器中的图元控制器绑定器列表来和元素下的所有图元控制器匹配ID
                for (int w = 0; w < valuebinder.binders.Length; w++)
                {
                    PrimitiveBinder binder = valuebinder.binders[w];

                    base_script.ModifiyValue(element, binder);
                }
            }
        }

        private void layoutspawner_act_on_despawn(XHudSpace space)
        {
            if (Application.isPlaying)
                return;
            //Debug.Log("模拟回收");
        }

        public override void OnInspectorGUI()
        {
            XHud_Manager mgr = XHud_Dashboard.get_manager();

            serializedObject.Update();

            string hexcol = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.themes_get_color(ThemeColorType.primary), true);

            TextClipping clipping_ellipsis = XGUI.TryEllipsisClipping();

            #region 标题
            XGUI.layout_banner(
                bg_fill: XGUIGroup.实体,
                bg_color: XGUIColor.深空灰,
                bg_height: 30,
                icon: icon_main,
                icon_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_text: "XHud  -  布局元素数据绑定器",
                title_anchor: TextAnchor.MiddleLeft,
                title_style: FontStyle.Normal,
                title_color: Color.white,
                title_size: XGUIFontSize.B,
                title_clipping: clipping_ellipsis,
                bg_margin: new RectOffset(0, 0, 5, 5));
            #endregion

            #region 可视化编辑
            XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "可视化编辑",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15));

            if (XGUI.layout_button(
                text: "编辑绑定",
                tooltip: "在独立窗口中进行布局元素数据绑定操作",
                height: 35,
                font_size: XGUIFontSize.M,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                press_fill: XGUIGroup.实体,
                press_color: XGUIColor.深空灰,
                button_text_color: Color.black,
                press_text_color: Color.white))
            {
                Editor_XHud_LayoutSpawner_ValueBinder_SetTool window = EditorWindow.GetWindow<Editor_XHud_LayoutSpawner_ValueBinder_SetTool>(false);

                window.titleContent = new GUIContent("XHud 布局元素数据绑定器");
                XGUI.CenterEditorWindow(new Vector2Int(420, 620), window);
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 选项
            base_script.fold_option = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "选项",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: base_script.fold_option);

            if (!base_script.fold_option)
            {
                #region 调试信息
                DrawToggle("调试信息", sp_use_debug, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, stroptions_enabled, (b) => { });
                #endregion
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 拖放区域
            XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "拖放区域",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15));

            #region 拖放区域
            Rect rect_drop = XGUI.GetControlRect(false, 90);
            if (Event.current.type == EventType.DragUpdated)
            {
                if (rect_drop.Contains(Event.current.mousePosition))
                {
                    if (!is_dragging_over)
                    {
                        //Debug.Log("拖入区域");
                        is_dragging_over = true;
                    }
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                }
                else
                {
                    if (is_dragging_over)
                    {
                        //Debug.Log("离开区域");
                        is_dragging_over = false;
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

                Undo.RecordObject(base_script, "BindChanged");

                // 检查校验有效性
                for (int i = 0; i < base_script.valuebinders.Length; i++)
                {
                    ref var e_binder = ref base_script.valuebinders[i];

                    if (e_binder.id != item.id)
                        continue;

                    // 过滤掉无效的 binder
                    var validBinders = new List<PrimitiveBinder>();
                    foreach (var binder in e_binder.binders)
                    {
                        var con = item.spawned_element_node.element.GetPrimitiveController_With_ID(binder.id);
                        if (con != null)
                            validBinders.Add(binder);
                    }

                    e_binder.binders = validBinders.ToArray();
                    // 因为是 ref，不需要再赋值回去
                }

                BindValue_Element(item);

                is_dragging_over = false;
                Repaint();
                Event.current.Use();
            }

            Rect rect_drop_visual = new Rect(rect_drop.x, rect_drop.y, rect_drop.width, rect_drop.height);

            if (is_dragging_over)
            {
                Rect rect_drop_visual_icon = new Rect(rect_drop_visual.x + 5, rect_drop_visual.y + 5, rect_drop_visual.width - 10, rect_drop_visual.height - 10);
                XGUI.gui_icon(
                    rect: rect_drop_visual_icon,
                    icon: icon_edge,
                    border: new RectOffset(25, 25, 25, 25),
                    color: XHud_Dashboard.themes_get_color(ThemeColorType.primary));

                XGUI.gui_box(
                    rect: rect_drop_visual_icon,
                    bg_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary) * 0.2f);

                Rect rect_drop_visual_text = new Rect(rect_drop_visual.x + 5, rect_drop_visual.y + 5, rect_drop_visual.width - 10, rect_drop_visual.height - 10);
                XGUI.gui_label(
                          rect: rect_drop_visual_text,
                          text: new GUIContent("松开鼠标即可添加到绑定列表"),
                          text_color: Color.gray,
                          size: XGUIFontSize.M,
                          clipping: TextClipping.Clip,
                          anchor: TextAnchor.MiddleCenter,
                          offset: new Vector2(0, 0),
                          font_style: FontStyle.Normal);
            }
            else
            {
                Rect rect_drop_visual_icon = new Rect(rect_drop_visual.x + 5, rect_drop_visual.y + 5, rect_drop_visual.width - 10, rect_drop_visual.height - 10);
                XGUI.gui_icon(
                    rect: rect_drop_visual_icon,
                    icon: icon_edge,
                    border: new RectOffset(25, 25, 25, 25),
                    color: Color.white * 0.65f);

                Rect rect_drop_visual_text = new Rect(rect_drop_visual.x + 5, rect_drop_visual.y + 5, rect_drop_visual.width - 10, rect_drop_visual.height - 10);
                XGUI.gui_label(
                    rect: rect_drop_visual_text,
                    text: new GUIContent("拖放布局元素数据至此"),
                    text_color: Color.gray,
                    size: XGUIFontSize.M,
                    clipping: TextClipping.Clip,
                    anchor: TextAnchor.MiddleCenter,
                    offset: new Vector2(0, 0),
                    font_style: FontStyle.Normal);
            }
            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 源脚本
            base_script.fold_based = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "源脚本",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: base_script.fold_based);

            if (!base_script.fold_based)
            {
                DrawDefaultInspector();
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            serializedObject.ApplyModifiedProperties();
        }

        public void BindValue_Element(XHud_LayoutSpawner_Item item)
        {
            XHud_Module_Element ele = item.spawned_element_node.element;
            string hex_col = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.themes_get_color(ThemeColorType.primary), true);

            // 用于覆盖原数据
            List<ElementBinder> BindList = new List<ElementBinder>();

            // 检测是否已经绑定了目标元素项
            for (int i = 0; i < base_script.valuebinders.Length; i++)
            {
                ElementBinder ele_binder = base_script.valuebinders[i];

                BindList.Add(ele_binder);

                // 如果绑定了那么就完善未绑定的图元
                if (ele_binder.id == item.id)
                {
                    if (string.IsNullOrEmpty(ele_binder.name) || ele_binder.name != item.spawn_name)
                        ele_binder.name = item.spawn_name;

                    if (ele != null)
                    {
                        ele_binder = BindValue_Primitives(ele_binder, ele);
                    }

                    base_script.valuebinders[i] = ele_binder;

                    if (ele_binder.binders.Length <= 0)
                    {
                        XGUI_Utilitys.Console("XHud - 布局元素数据绑定器通知", $"注意！ID为：<b><color={hex_col}> {item.id} </b></color>的目标布局元素数据已经绑定，<b><color={hex_col}>请勿重复绑定！</b></color>此操作已被跳过！", XGUIMsgState.通知);
                    }
                    else
                    {
                        XGUI_Utilitys.Console("XHud - 布局元素数据绑定器通知", $"注意！ID为：<b><color={hex_col}> {item.id} </b></color>的目标布局元素数据已经绑定，<b><color={hex_col}>因为您之前绑定过该元素ID</b></color>，但当时可能其下并没有任何图元！所以完善了其下的图元数据的绑定！</b></color>", XGUIMsgState.通知);
                    }

                    return;
                }
            }

            // 能执行到此处则表示没有任何图元被绑定

            // 加入绑定列表
            ElementBinder x_ele_binder = new ElementBinder();

            // 新建的绑定器的ID为拖入的元素项的加载的元素的ID
            x_ele_binder.id = item.id;
            x_ele_binder.name = item.spawn_name;

            // 如果元素未载入，只绑定元素项自身ID，后续不处理元素下的图元数据绑定，否则则跳过这一步判断
            if (item.spawned_element_node.element == null && sp_use_debug.boolValue)
            {
                XGUI_Utilitys.Console("XHud - 布局元素数据绑定器通知", $"注意！ID为：<b><color={hex_col}> {item.id} </b></color>的目标布局元素并未加载到场景！所以只能绑定元素基础信息而不能绑定元素下的图元控制器数据！", XGUIMsgState.警告);
                return;
            }

            if (ele != null)
            {
                // 处理元素下的图元数据绑定
                x_ele_binder = BindValue_Primitives(x_ele_binder, ele);
            }

            BindList.Add(x_ele_binder);
            base_script.valuebinders = BindList.ToArray();
        }

        /// <summary>
        /// 绑定元素下所有图元的ID
        /// </summary>
        /// <param name="ele_binder"></param>
        /// <param name="ele"></param>
        /// <returns></returns>
        private ElementBinder BindValue_Primitives(ElementBinder ele_binder, XHud_Module_Element ele)
        {
            if (ele.primitive_controller_nodes.Count <= 0)
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
            for (int i = 0; i < ele.primitive_controller_nodes.Count; i++)
            {
                XHud_Module_Primitive_Controller con = ele.primitive_controller_nodes[i].controller;

                if (con == null)
                    continue;

                #region 判断是否已经存在？如果存在在判定是否名称已赋值？
                bool exist = false;
                for (int a = 0; a < prim_binders.Count; a++)
                {
                    if (prim_binders[a].id == con.id)
                    {
                        PrimitiveBinder binder = prim_binders[a];

                        exist = true;

                        // 如果图元绑定关系已存在，则根据情况更新绑定名称
                        if (prim_binders[a].name != con.transform.name)
                        {
                            binder.name = con.transform.name;
                        }

                        if (binder.type == ModuleType.Image)
                        {
                            if (con.pt_painting != null &&
                                con.pt_painting.sync_image_color &&
                                !con.pt_painting.sync_library_color)
                            {
                                binder.val_color = con.pt_painting.original_color;
                            }
                            else
                            {
                                binder.val_color = con.mod_image.color;
                            }
                        }
                        if (binder.type == ModuleType.RawImage)
                        {
                            if (con.pt_painting != null &&
                                con.pt_painting.sync_image_color &&
                                !con.pt_painting.sync_library_color)
                            {
                                binder.val_color = con.pt_painting.original_color;
                            }
                            else
                            {
                                binder.val_color = con.mod_raw_image.color;
                            }
                        }
                        if (binder.type == ModuleType.Text)
                        {
                            if (con.pt_painting != null &&
                                con.pt_painting.sync_image_color &&
                                !con.pt_painting.sync_library_color)
                            {
                                if (con.mod_text.text_color_owner == TextColorOwner.Painting)
                                    binder.val_color = con.pt_painting.original_color;
                            }
                            else
                                binder.val_color = con.mod_text.text_style_info.font_color;
                        }
                        if (binder.type == ModuleType.TmpText)
                        {
                            if (con.pt_painting != null &&
                                con.pt_painting.sync_image_color &&
                                !con.pt_painting.sync_library_color)
                            {
                                if (con.mod_tmp_text.text_color_owner == TextColorOwner.Painting)
                                    binder.val_color = con.pt_painting.original_color;
                            }
                            else
                                binder.val_color = con.mod_tmp_text.text_style_info.tmp_color;
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
                    p_binder.name = con.transform.name;
                    p_binder.id = con.id;
                    p_binder.type = con.GetModuleType();

                    if (p_binder.type == ModuleType.Image)
                    {
                        if (con.pt_painting != null && con.pt_painting.sync_image_color && !con.pt_painting.sync_library_color)
                            p_binder.val_color = con.pt_painting.original_color;
                        else
                            p_binder.val_color = con.mod_image.color;
                    }
                    if (p_binder.type == ModuleType.RawImage)
                    {
                        if (con.pt_painting != null && con.pt_painting.sync_image_color && !con.pt_painting.sync_library_color)
                            p_binder.val_color = con.pt_painting.original_color;
                        else
                            p_binder.val_color = con.mod_raw_image.color;
                    }
                    if (p_binder.type == ModuleType.Text)
                    {
                        if (con.pt_painting != null && con.pt_painting.sync_image_color && !con.pt_painting.sync_library_color)
                        {
                            if (con.mod_text.text_color_owner == TextColorOwner.Painting)
                                p_binder.val_color = con.pt_painting.original_color;
                        }
                        else
                            p_binder.val_color = con.mod_text.text_style_info.font_color;
                    }
                    if (p_binder.type == ModuleType.TmpText)
                    {
                        if (con.pt_painting != null && con.pt_painting.sync_image_color && !con.pt_painting.sync_library_color)
                        {
                            if (con.mod_tmp_text.text_color_owner == TextColorOwner.Painting)
                                p_binder.val_color = con.pt_painting.original_color;
                        }
                        else
                            p_binder.val_color = con.mod_tmp_text.text_style_info.tmp_color;
                    }
                    prim_binders.Add(p_binder);
                }
            }
            ele_binder.binders = prim_binders.ToArray();

            return ele_binder;
        }

        #region 绘制控件
        /// <summary>
        /// 通用方法：绘制带标题的开关控件，统一标题宽度、字体与混合选项样式
        /// </summary>
        /// <param name="title">开关标题文字</param>
        /// <param name="prop">开关对应的序列化属性</param>
        /// <param name="width">标题宽度</param>
        /// <param name="style">开关样式</param>
        /// <param name="color_bg_on">开启状态背景色</param>
        /// <param name="color_bg_off">关闭状态背景色</param>
        /// <param name="color_on">开启状态手柄颜色</param>
        /// <param name="color_off">关闭状态手柄颜色</param>
        /// <param name="options">混合选项文字（多选时显示）</param>
        /// <param name="act_on_changed">值变化时的回调</param>
        private void DrawToggle(string title, SerializedProperty prop, float width, XGUIToggleStyle style = XGUIToggleStyle.实体, Color color_bg_on = default, Color color_bg_off = default, Color color_on = default, Color color_off = default, string[] options = null, Action<bool> act_on_changed = null)
        {
            XGUI.layout_toggle(
                title: title,
                title_size: XGUIFontSize.M,
                title_font_style: FontStyle.Normal,
                title_padding: new RectOffset(5, 10, 0, 0),
                title_width: width,
                prop: prop,
                tog_style: style,
                tog_padding: new RectOffset(0, 9, 0, 0),
                tog_margin: new RectOffset(0, 0, 0, 5),
                tog_mixed_options: options,
                tog_mixed_text_size: XGUIFontSize.M,
                tog_mixed_text_color: Color.black,
                tog_mixed_text_padding: new RectOffset(10, 10, 0, 0),
                tog_mixed_text_anchor: TextAnchor.MiddleCenter,
                tog_mixed_font_style: FontStyle.Normal,
                tog_bg_off_color: color_bg_off,
                tog_bg_on_color: color_bg_on,
                tog_handler_off_color: color_off,
                tog_handler_on_color: color_on,
                tog_mixed_bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                act_on_changed: act_on_changed);
        }
        #endregion
    }
}
