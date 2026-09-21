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
    using SevenStrikeModules.XGUI.Editor;
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XTween;
    using SevenStrikeModules.XTween.Editor;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    /// <summary>
    /// 元素信息
    /// </summary>
    [System.Serializable]
    public class HudElementInfo
    {
        public List<XHud_Module_Element> elements = new List<XHud_Module_Element>();
        public List<GameObject> notPrefabsList = new List<GameObject>();
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(XHud_LayoutSpawner))]
    public class Editor_XHud_LayoutSpawner : Editor
    {
        #region 组件 / 列表
        private XHud_LayoutSpawner BaseScript;
        /// <summary>
        /// 元素列表 - 屏幕
        /// </summary>
        public ReorderableList ElementList_Screen;
        /// <summary>
        /// 元素列表 - 屏幕
        /// </summary>
        public ReorderableList ElementList_World;
        private XHud_Manager HudManager;
        #endregion

        /// <summary>
        /// 原始脚本参数显示开关
        /// </summary>
        private bool OriginalDisplay;

        #region 序列化属性
        private SerializedProperty LibName, IsLoadedLayout_Screen, IsLoadedLayout_World, SpawnItemList_Screen, SpawnItemList_World, SpawnerIndicator, CreateArgs, RecycleArgs, Key_Create, Key_Recycle, UseManullyKey, create_fold_move, create_fold_rotate, create_fold_alpha, recycle_fold_move, recycle_fold_rotate, recycle_fold_alpha, Crc_Lib_Name, Rec_Lib_Name, SpawnFunctionKey_Primary, SpawnFunctionKey_Secondary, KeyControl_Screen, KeyControl_World, FoldScreen, FoldWorld, spawnerisRunning, CreateArgs_MotionAnimateEndState, RecycleArgs_MotionAnimateEndState, IsPreviewing, UseInstantiateMode, UseDebug;
        #endregion

        #region 选项文字
        string[] stroptions_enabled = new string[2] { "关闭", "开启" };
        string[] stroptions_control = new string[2] { "禁用", "可控" };
        #endregion

        #region 图标
        private Texture2D
            icon_main,
            icon_save_r,
            icon_save_p,
            icon_locate_r,
            icon_locate_p,
            icon_elementicon,
            icon_dot,
            icon_icon_unfold_r,
            icon_icon_unfold_p,
            icon_anim_fold_r,
            icon_anim_fold_p,
            icon_reset_r,
            icon_reset_p,
            icon_scan_r,
            icon_scan_p,
            icon_preview_in_r,
            icon_preview_in_p,
            icon_preview_out_r,
            icon_preview_out_p,
            icon_stoppreview_r,
            icon_stoppreview_p,
            icon_load_r,
            icon_load_p,
            icon_unload_r,
            icon_unload_p,
            icon_recreate_id_r,
            icon_recreate_id_p;
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

        #region 批量化操作
        XHud_LayoutSpawner[] SelectedObjects;

        private void Targets_Get()
        {
            if (targets.Length > 1)
            {
                SelectedObjects = new XHud_LayoutSpawner[targets.Length];
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    var t = targets[i];
                    SelectedObjects[i] = (XHud_LayoutSpawner)t;
                }
            }
            else
            {
                SelectedObjects = new XHud_LayoutSpawner[targets.Length];
                SelectedObjects[0] = (XHud_LayoutSpawner)target;
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

        private string PrefsKeyFold_Prams = "XHUD-LAYOUTSPAWN-FOLD-PARAMS", PrefsKeyFold_Motion = "XHUD-LAYOUTSPAWN-FOLD-MOTION";

        Rect drawelement_screen_rect;
        Rect item_rect_screen;
        int selected_Screen = -1;

        Rect drawelement_world_rect;
        Rect item_rect_world;
        int selected_World = -1;

        private void OnEnable()
        {
            HudManager = XHud_Dashboard.HudManagerGet();

            BaseScript = (XHud_LayoutSpawner)target;

            GetSerializeFields();

            #region 获取字体
            Font_Bold = Editor_XHud_GUI.GetFont("sx_bold");
            Font_Light = Editor_XHud_GUI.GetFont("sx_regular");
            #endregion

            #region 获取图标
            icon_main = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner/icon_main");
            icon_save_r = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner/icon_save_r");
            icon_save_p = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner/icon_save_p");
            icon_locate_r = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner/icon_locate_r");
            icon_locate_p = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner/icon_locate_p");
            icon_elementicon = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner/icon_elementicon");
            icon_dot = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner/icon_dot");
            icon_icon_unfold_r = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner/icon_unfold_r");
            icon_icon_unfold_p = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner/icon_unfold_p");
            icon_anim_fold_r = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner/icon_anim_fold_r");
            icon_anim_fold_p = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner/icon_anim_fold_p");
            icon_reset_r = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner/icon_reset_r");
            icon_reset_p = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner/icon_reset_p");
            icon_scan_r = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner/icon_scan_r");
            icon_scan_p = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner/icon_scan_p");
            icon_preview_in_r = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner/icon_preview_in_r");
            icon_preview_in_p = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner/icon_preview_in_p");
            icon_preview_out_r = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner/icon_preview_out_r");
            icon_preview_out_p = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner/icon_preview_out_p");
            icon_stoppreview_r = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner/icon_stoppreview_r");
            icon_stoppreview_p = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner/icon_stoppreview_p");
            icon_load_r = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner/icon_load_r");
            icon_load_p = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner/icon_load_p");
            icon_unload_r = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner/icon_unload_r");
            icon_unload_p = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner/icon_unload_p");
            icon_recreate_id_r = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner/icon_recreate_id_r");
            icon_recreate_id_p = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner/icon_recreate_id_p");
            #endregion

            Targets_Get();

            #region 用于第一次加载脚本判断库名是否是空的
            if (!Application.isPlaying && HudManager != null)
            {
                //获取所有元素库名称
                string[] LibNames = HudManager.hm_ElementLibrary_GetAllLibraryNames();
                //如果不是空的
                if (LibNames.Length > 0)
                {
                    //判断如果库名是空的就把首个元素库名给他
                    if (string.IsNullOrEmpty(LibName.stringValue))
                    {
                        LibName.stringValue = LibNames[0];
                        LibName.serializedObject.ApplyModifiedProperties();
                    }
                }
            }
            #endregion

            #region 屏幕元素
            ElementList_Screen = new ReorderableList(serializedObject, SpawnItemList_Screen)
            {
                draggable = false,
                drawElementBackgroundCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    if (selected_Screen == index)
                    {
                        // 高亮标记表示选中
                        item_rect_screen.Set(rect.x - 1, rect.y + 4, 3, 36);
                        EditorGUI.DrawRect(item_rect_screen, XHud_Dashboard.Theme_Primary);
                    }
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    Event e = Event.current;

                    SerializedProperty prop = SpawnItemList_Screen.GetArrayElementAtIndex(index);

                    SerializedProperty sp_SpawnName = prop.FindPropertyRelative("SpawnName");
                    SerializedProperty sp_Indicator = prop.FindPropertyRelative("Indicator");
                    SerializedProperty sp_ID = prop.FindPropertyRelative("ID");
                    SerializedProperty sp_Delay_Spawn = prop.FindPropertyRelative("Delay_Spawn");
                    SerializedProperty sp_Delay_Despawn = prop.FindPropertyRelative("Delay_Despawn");
                    SerializedProperty sp_AutoIn = prop.FindPropertyRelative("AutoIn");
                    SerializedProperty sp_InMotion = prop.FindPropertyRelative("InMotion");
                    SerializedProperty sp_Spawned = prop.FindPropertyRelative("Spawned");
                    SerializedProperty sp_MotionPercentage = prop.FindPropertyRelative("MotionPercentage");
                    SerializedProperty sp_UseSpawnerMotion = prop.FindPropertyRelative("UseSpawnerMotion");
                    SerializedProperty sp_Position = prop.FindPropertyRelative("Position");
                    SerializedProperty sp_Euler = prop.FindPropertyRelative("Euler");
                    SerializedProperty sp_Offset = prop.FindPropertyRelative("Offset");
                    SerializedProperty sp_Scale = prop.FindPropertyRelative("Scale");
                    SerializedProperty sp_Size = prop.FindPropertyRelative("Size");
                    SerializedProperty sp_Pivot = prop.FindPropertyRelative("Pivot");
                    SerializedProperty sp_Anchor_Min = prop.FindPropertyRelative("Anchor_Min");
                    SerializedProperty sp_Anchor_Max = prop.FindPropertyRelative("Anchor_Max");
                    SerializedProperty sp_isFold = prop.FindPropertyRelative("isFold");
                    SerializedProperty sp_isEnabled = prop.FindPropertyRelative("isEnabled");

                    #region 开关
                    drawelement_screen_rect.Set(rect.x, rect.y, 20, 20);
                    sp_isEnabled.boolValue = Editor_XHud_GUI.Gui_Toggle(drawelement_screen_rect, true, null, sp_isEnabled.boolValue);
                    #endregion

                    #region 索引号
                    drawelement_screen_rect.Set(rect.x + 25, rect.y, 40, 20);
                    if (EditorGUIUtility.currentViewWidth >= 125)
                        Editor_XHud_GUI.Gui_Labelfield(drawelement_screen_rect, index.ToString("D1"), HudFilled.无, HudColor.无, Color.gray, TextAnchor.MiddleLeft, Vector2.zero, 11, Font_Light);
                    #endregion

                    #region 徽标 （已生成）
                    if (sp_Spawned.boolValue)
                        GUI.backgroundColor = XHud_Dashboard.Theme_Primary;
                    else
                        GUI.backgroundColor = Color.gray;

                    drawelement_screen_rect.Set(rect.x + 45, rect.y + 5, 10, 10);
                    if (EditorGUIUtility.currentViewWidth >= 140)
                        Editor_XHud_GUI.Gui_Icon(drawelement_screen_rect, icon_elementicon);
                    GUI.backgroundColor = Color.white;
                    #endregion

                    #region 徽标（运动中）
                    if (sp_InMotion.boolValue)
                        GUI.backgroundColor = XHud_Dashboard.Theme_Primary;
                    else
                        GUI.backgroundColor = Color.gray;

                    drawelement_screen_rect.Set(rect.x + 65, rect.y + 5, 10, 10);
                    if (EditorGUIUtility.currentViewWidth >= 160)
                        Editor_XHud_GUI.Gui_Icon(drawelement_screen_rect, icon_elementicon);
                    GUI.backgroundColor = Color.white;
                    #endregion

                    #region 拖拽数据
                    drawelement_screen_rect.Set(rect.x, rect.y + 5, rect.width - 70, rect.height - 10);
                    Editor_XHud_GUI.Gui_Box(drawelement_screen_rect, Color.clear);

                    if (e.type == EventType.MouseDrag && drawelement_screen_rect.Contains(e.mousePosition))
                    {
                        DragAndDrop.PrepareStartDrag();
                        DragAndDrop.SetGenericData("LayoutSpawnerBindData", JsonUtility.ToJson(BaseScript.SpawnItemList_Screen[index]));
                        DragAndDrop.StartDrag("LayoutSpawnerBindDataDrag");
                        e.Use();
                    }
                    #endregion

                    #region 名称类
                    drawelement_screen_rect.Set(rect.x + 90, rect.y, rect.width - 10 - 150, 20);
                    Editor_XHud_GUI.Gui_Labelfield(drawelement_screen_rect, sp_SpawnName.stringValue, HudFilled.无, HudColor.深空灰, XHud_Dashboard.Theme_Primary, TextAnchor.MiddleLeft, Vector2.zero, 12, true, TextClipping.Ellipsis, true, Font_Bold);

                    drawelement_screen_rect.Set(rect.x + 30, rect.y + 30, rect.width - 130 - 15, 20);
                    Editor_XHud_GUI.Gui_Labelfield(drawelement_screen_rect, sp_Indicator.stringValue, HudFilled.无, HudColor.深空灰, Color.white * 0.85f, TextAnchor.MiddleLeft, Vector2.zero, 11, true, TextClipping.Ellipsis, true, Font_Light);

                    drawelement_screen_rect.Set(rect.x + rect.width - 55, rect.y + 5, 50, 16);
                    if (EditorGUIUtility.currentViewWidth >= 230)
                    {
                        Editor_XHud_GUI.Gui_Labelfield(drawelement_screen_rect, sp_ID.stringValue, HudFilled.实体, HudColor.深空灰, Color.white, TextAnchor.MiddleCenter, new Vector2(0, -1.5f), 11, true, TextClipping.Ellipsis, true, Font_Light);
                    }
                    if (e.type == EventType.MouseDown && e.button == 0 && drawelement_screen_rect.Contains(e.mousePosition))
                    {
                        EditorGUIUtility.systemCopyBuffer = sp_ID.stringValue;
                        EditorApplication.delayCall += () =>
                        {
                            string hexcol = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary, true);
                            string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 布局元素生成器消息", "拷贝元素项ID", $"已将元素项的ID： <color={hexcol}><b>   {sp_ID.stringValue}   </b></color>拷贝至系统剪贴板中！", "明白", 0);
                        };
                        e.Use();
                    }

                    #endregion

                    #region 折叠开关
                    if (!sp_isFold.boolValue)
                    {
                        drawelement_screen_rect.Set(rect.x + 2, rect.y + 25, 12, 12);
                        if (Editor_XHud_GUI.Gui_Button(drawelement_screen_rect, icon_icon_unfold_r, icon_icon_unfold_p, true, "", "", Color.white))
                        {
                            for (int i = 0; i < ElementList_Screen.count; i++)
                            {
                                SerializedProperty name = SpawnItemList_Screen.GetArrayElementAtIndex(i).FindPropertyRelative("SpawnName");
                                SerializedProperty foldstate = SpawnItemList_Screen.GetArrayElementAtIndex(i).FindPropertyRelative("isFold");
                                foldstate.boolValue = true;
                                foldstate.serializedObject.ApplyModifiedProperties();
                            }

                            sp_isFold.boolValue = true;
                            sp_isFold.serializedObject.ApplyModifiedProperties();
                        }
                    }
                    else
                    {
                        drawelement_screen_rect.Set(rect.x + 2, rect.y + 25, 12, 12);
                        if (Editor_XHud_GUI.Gui_Button(drawelement_screen_rect, icon_anim_fold_r, icon_anim_fold_p, true, "", "", Color.white))
                        {
                            for (int i = 0; i < ElementList_Screen.count; i++)
                            {
                                SerializedProperty name = SpawnItemList_Screen.GetArrayElementAtIndex(i).FindPropertyRelative("SpawnName");
                                SerializedProperty foldstate = SpawnItemList_Screen.GetArrayElementAtIndex(i).FindPropertyRelative("isFold");
                                foldstate.boolValue = true;
                                foldstate.serializedObject.ApplyModifiedProperties();
                            }
                            sp_isFold.boolValue = false;
                            sp_isFold.serializedObject.ApplyModifiedProperties();
                        }
                    }
                    #endregion

                    #region AutoIn
                    drawelement_screen_rect.Set(rect.width - 15, rect.y + 35, 55, 12);
                    if (EditorGUIUtility.currentViewWidth >= 137)
                        Editor_XHud_GUI.Gui_PopupWithString(drawelement_screen_rect, ref sp_AutoIn, new string[2] { "自动 In", "手动 In" }, HudFilled.无, HudColor.亮白, Color.white * 0.85f);
                    GUI.backgroundColor = Color.white;
                    #endregion

                    #region 使用生成器的通用动效
                    GUI.backgroundColor = XHud_Dashboard.Theme_Primary;
                    drawelement_screen_rect.Set(rect.width - 95, rect.y + 35, 88, 12);
                    if (EditorGUIUtility.currentViewWidth >= 210)
                        Editor_XHud_GUI.Gui_PopupWithString(drawelement_screen_rect, ref sp_UseSpawnerMotion, new string[3] { "元素自身动效", "列表项动效", "生成器动效" }, HudFilled.无, HudColor.亮白, Color.white * 0.85f);
                    GUI.backgroundColor = Color.white;
                    #endregion

                    float baseheight = rect.y + 52;

                    #region 正在生成 / 回收进度条
                    drawelement_screen_rect.Set(rect.x, baseheight, rect.width, 1);
                    Editor_XHud_GUI.Gui_Box(drawelement_screen_rect, Color.gray);

                    if (sp_InMotion.boolValue)
                    {
                        GUI.backgroundColor = Color.white;
                        Repaint();
                        drawelement_screen_rect.Set(rect.x, baseheight, rect.width * sp_MotionPercentage.floatValue, 1);
                        Editor_XHud_GUI.Gui_Box(drawelement_screen_rect, XHud_Dashboard.Theme_Primary);
                    }
                    #endregion

                    #region 延迟参数
                    if (EditorGUIUtility.currentViewWidth >= 140)
                    {
                        drawelement_screen_rect.Set(rect.x, baseheight + 10, rect.width / 2 - 5, 18);
                        Editor_XHud_GUI.Gui_Property_Field(drawelement_screen_rect, "延迟生成", "-S", sp_Delay_Spawn);
                    }
                    if (EditorGUIUtility.currentViewWidth >= 200)
                    {
                        drawelement_screen_rect.Set(rect.x + rect.width / 2 + 5, baseheight + 10, rect.width / 2 - 5, 18);
                        Editor_XHud_GUI.Gui_Property_Field(drawelement_screen_rect, "延迟回收", "-D", sp_Delay_Despawn);
                    }
                    #endregion

                    #region 信息
                    if (!sp_isFold.boolValue)
                    {
                        string hexcol = XGUI_Utilitys.Color_To_HexString(Color.white * 0.75f, true);
                        string darkcol = XGUI_Utilitys.Color_To_HexString(Color.gray, true);

                        drawelement_screen_rect.Set(rect.x, baseheight + 40, rect.width / 2 - 5, 20);
                        Editor_XHud_GUI.Gui_Labelfield(drawelement_screen_rect, $"<color={hexcol}>pos：</color> <color={darkcol}>{sp_Position.vector3Value}</color>", HudFilled.无, HudColor.深空灰, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 11, true, TextClipping.Ellipsis, true, Font_Light);

                        drawelement_screen_rect.Set(rect.x + rect.width / 2 + 5, baseheight + 40, rect.width / 2 - 5, 20);
                        Editor_XHud_GUI.Gui_Labelfield(drawelement_screen_rect, $"<color={hexcol}>rot：</color> <color={darkcol}>{sp_Euler.vector3Value}</color>", HudFilled.无, HudColor.深空灰, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 11, true, TextClipping.Ellipsis, true, Font_Light);

                        drawelement_screen_rect.Set(rect.x, baseheight + 65, rect.width / 2 - 5, 20);
                        Editor_XHud_GUI.Gui_Labelfield(drawelement_screen_rect, $"<color={hexcol}>offset：</color> <color={darkcol}>{sp_Offset.vector3Value}</color>", HudFilled.无, HudColor.深空灰, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 11, true, TextClipping.Ellipsis, true, Font_Light);

                        drawelement_screen_rect.Set(rect.x + rect.width / 2 + 5, baseheight + 65, rect.width / 2 - 5, 20);
                        Editor_XHud_GUI.Gui_Labelfield(drawelement_screen_rect, $"<color={hexcol}>scl：</color> <color={darkcol}>{sp_Scale.vector3Value}</color>", HudFilled.无, HudColor.深空灰, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 11, true, TextClipping.Ellipsis, true, Font_Light);

                        drawelement_screen_rect.Set(rect.x, baseheight + 90, rect.width, 1);
                        Editor_XHud_GUI.Gui_Box(drawelement_screen_rect, Color.gray * 0.7f);

                        drawelement_screen_rect.Set(rect.x, baseheight + 98, rect.width / 2 - 5, 20);
                        Editor_XHud_GUI.Gui_Labelfield(drawelement_screen_rect, $"<color={hexcol}>size：</color> <color={darkcol}>{sp_Size.vector2Value}</color>", HudFilled.无, HudColor.深空灰, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 11, true, TextClipping.Ellipsis, true, Font_Light);

                        drawelement_screen_rect.Set(rect.x + rect.width / 2 + 5, baseheight + 98, rect.width / 2 - 5, 20);
                        Editor_XHud_GUI.Gui_Labelfield(drawelement_screen_rect, $"<color={hexcol}>pivot：</color> <color={darkcol}>{sp_Pivot.vector2Value}</color>", HudFilled.无, HudColor.深空灰, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 11, true, TextClipping.Ellipsis, true, Font_Light);

                        drawelement_screen_rect.Set(rect.x, baseheight + 118, rect.width / 2 - 5, 20);
                        Editor_XHud_GUI.Gui_Labelfield(drawelement_screen_rect, $"<color={hexcol}>anc min：</color> <color={darkcol}>{sp_Anchor_Min.vector2Value}</color>", HudFilled.无, HudColor.深空灰, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 11, true, TextClipping.Ellipsis, true, Font_Light);

                        drawelement_screen_rect.Set(rect.x + rect.width / 2 + 5, baseheight + 118, rect.width / 2 - 5, 20);
                        Editor_XHud_GUI.Gui_Labelfield(drawelement_screen_rect, $"<color={hexcol}>anc max：</color> <color={darkcol}>{sp_Anchor_Max.vector2Value}</color>", HudFilled.无, HudColor.深空灰, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 11, true, TextClipping.Ellipsis, true, Font_Light);
                    }
                    #endregion

                    #region  右键菜单
                    drawelement_screen_rect.Set(rect.x, rect.y, rect.width - 50, rect.height);
                    if (e.type == EventType.MouseDown && e.button == 1 && drawelement_screen_rect.Contains(e.mousePosition))
                    {
                        // 创建右键菜单
                        GenericMenu menu = new GenericMenu();
                        menu.AddItem(new GUIContent("E 修改动效"), false, () =>
                        {
                            if (!IsLoadedLayout_Screen.boolValue && sp_UseSpawnerMotion.stringValue == "元素自身动效")
                            {
                                Debug.Log("在 \"元素自身动效\" 模式下，前提要先把元素加载才可以对元素自身动效参数进行修改");
                                return;
                            }

                            XHud_LibraryArg_Motion motion = new XHud_LibraryArg_Motion("", 0, "",
                                sp_UseSpawnerMotion.stringValue == "列表项动效" ? BaseScript.SpawnItemList_Screen[index].CreateArgs : sp_UseSpawnerMotion.stringValue == "元素自身动效" ? BaseScript.SpawnItemList_Screen[index].SpawnedElementNode.Element.CreateArgs : BaseScript.CreateArgs,
                                sp_UseSpawnerMotion.stringValue == "列表项动效" ? BaseScript.SpawnItemList_Screen[index].RecycleArgs : sp_UseSpawnerMotion.stringValue == "元素自身动效" ? BaseScript.SpawnItemList_Screen[index].SpawnedElementNode.Element.RecycleArgs : BaseScript.RecycleArgs);
                            OpenParameterModifier(motion, sp_SpawnName.stringValue, HudElementMotionType.Creator);
                            return;
                        });

                        // 显示右键菜单
                        menu.ShowAsContext();

                        e.Use(); // 标记事件已被处理，防止其他操作处理该事件
                    }
                    GUI.enabled = true;
                    #endregion
                },
                elementHeightCallback = index =>
                {
                    float height = 0;

                    if (BaseScript.SpawnItemList_Screen[index].isFold)
                    {
                        height = 90;
                    }
                    else
                    {
                        height = 200;
                    }
                    return height;
                },
                onMouseUpCallback = list =>
                {
                    selected_Screen = list.index;
                },
                onSelectCallback = list =>
                {
                    selected_Screen = list.index;

                    if (BaseScript.SpawnItemList_Screen.Count > 0)
                    {
                        XHud_Module_Element element = BaseScript.SpawnItemList_Screen[list.index].SpawnedElementNode.Element;

                        if (element != null)
                        {
                            EditorGUIUtility.PingObject(element);
                            // 元素高亮闪烁
                            XTween_Preview_Start(new XTween_Interface[] { element.element_Highlight() }, XHudSpace.屏幕空间);
                        }
                    }
                }
            };
            #endregion

            #region 世界元素
            ElementList_World = new ReorderableList(serializedObject, SpawnItemList_World)
            {
                draggable = false,
                drawElementBackgroundCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    if (selected_World == index)
                    {
                        // 高亮标记表示选中
                        item_rect_world.Set(rect.x - 1, rect.y + 4, 3, 36);
                        EditorGUI.DrawRect(item_rect_world, XHud_Dashboard.Theme_Primary);
                    }
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    Event e = Event.current;

                    SerializedProperty prop = SpawnItemList_World.GetArrayElementAtIndex(index);

                    SerializedProperty sp_SpawnName = prop.FindPropertyRelative("SpawnName");
                    SerializedProperty sp_Indicator = prop.FindPropertyRelative("Indicator");
                    SerializedProperty sp_ID = prop.FindPropertyRelative("ID");
                    SerializedProperty sp_Delay_Spawn = prop.FindPropertyRelative("Delay_Spawn");
                    SerializedProperty sp_Delay_Despawn = prop.FindPropertyRelative("Delay_Despawn");
                    SerializedProperty sp_AutoIn = prop.FindPropertyRelative("AutoIn");
                    SerializedProperty sp_InMotion = prop.FindPropertyRelative("InMotion");
                    SerializedProperty sp_Spawned = prop.FindPropertyRelative("Spawned");
                    SerializedProperty sp_MotionPercentage = prop.FindPropertyRelative("MotionPercentage");
                    SerializedProperty sp_UseSpawnerMotion = prop.FindPropertyRelative("UseSpawnerMotion");
                    SerializedProperty sp_Position = prop.FindPropertyRelative("Position");
                    SerializedProperty sp_Euler = prop.FindPropertyRelative("Euler");
                    SerializedProperty sp_Offset = prop.FindPropertyRelative("Offset");
                    SerializedProperty sp_Scale = prop.FindPropertyRelative("Scale");
                    SerializedProperty sp_Size = prop.FindPropertyRelative("Size");
                    SerializedProperty sp_Pivot = prop.FindPropertyRelative("Pivot");
                    SerializedProperty sp_Anchor_Min = prop.FindPropertyRelative("Anchor_Min");
                    SerializedProperty sp_Anchor_Max = prop.FindPropertyRelative("Anchor_Max");
                    SerializedProperty sp_isFold = prop.FindPropertyRelative("isFold");
                    SerializedProperty sp_isEnabled = prop.FindPropertyRelative("isEnabled");

                    #region 开关
                    drawelement_world_rect.Set(rect.x, rect.y, 20, 20);
                    sp_isEnabled.boolValue = Editor_XHud_GUI.Gui_Toggle(drawelement_world_rect, true, null, sp_isEnabled.boolValue);
                    #endregion

                    #region 索引号
                    drawelement_world_rect.Set(rect.x + 25, rect.y, 40, 20);
                    if (EditorGUIUtility.currentViewWidth >= 125)
                        Editor_XHud_GUI.Gui_Labelfield(drawelement_world_rect, index.ToString("D1"), HudFilled.无, HudColor.无, Color.gray, TextAnchor.MiddleLeft, Vector2.zero, 11, Font_Light);
                    #endregion

                    #region 徽标 （已生成）
                    if (sp_Spawned.boolValue)
                        GUI.backgroundColor = XHud_Dashboard.Theme_Primary;
                    else
                        GUI.backgroundColor = Color.gray;

                    drawelement_world_rect.Set(rect.x + 45, rect.y + 5, 10, 10);
                    if (EditorGUIUtility.currentViewWidth >= 140)
                        Editor_XHud_GUI.Gui_Icon(drawelement_world_rect, icon_elementicon);
                    GUI.backgroundColor = Color.white;
                    #endregion

                    #region 徽标（运动中）
                    if (sp_InMotion.boolValue)
                        GUI.backgroundColor = XHud_Dashboard.Theme_Primary;
                    else
                        GUI.backgroundColor = Color.gray;

                    drawelement_world_rect.Set(rect.x + 65, rect.y + 5, 10, 10);
                    if (EditorGUIUtility.currentViewWidth >= 160)
                        Editor_XHud_GUI.Gui_Icon(drawelement_world_rect, icon_elementicon);
                    GUI.backgroundColor = Color.white;
                    #endregion

                    #region 拖拽数据
                    drawelement_screen_rect.Set(rect.x, rect.y + 5, rect.width - 70, rect.height - 10);
                    Editor_XHud_GUI.Gui_Box(drawelement_screen_rect, Color.clear);

                    if (e.type == EventType.MouseDrag && drawelement_screen_rect.Contains(e.mousePosition))
                    {
                        DragAndDrop.PrepareStartDrag();
                        DragAndDrop.SetGenericData("LayoutSpawnerBindData", JsonUtility.ToJson(BaseScript.SpawnItemList_World[index]));
                        DragAndDrop.StartDrag("LayoutSpawnerBindData");
                        e.Use();
                    }
                    #endregion

                    #region 名称类
                    drawelement_world_rect.Set(rect.x + 90, rect.y, rect.width - 10 - 150, 20);
                    Editor_XHud_GUI.Gui_Labelfield(drawelement_world_rect, sp_SpawnName.stringValue, HudFilled.无, HudColor.深空灰, XHud_Dashboard.Theme_Primary, TextAnchor.MiddleLeft, Vector2.zero, 12, true, TextClipping.Ellipsis, true, Font_Bold);

                    drawelement_world_rect.Set(rect.x + 30, rect.y + 30, rect.width - 130 - 15, 20);
                    Editor_XHud_GUI.Gui_Labelfield(drawelement_world_rect, sp_Indicator.stringValue, HudFilled.无, HudColor.深空灰, Color.white * 0.85f, TextAnchor.MiddleLeft, Vector2.zero, 11, true, TextClipping.Ellipsis, true, Font_Light);

                    drawelement_world_rect.Set(rect.x + rect.width - 55, rect.y + 5, 50, 16);
                    if (EditorGUIUtility.currentViewWidth >= 230)
                    {
                        Editor_XHud_GUI.Gui_Labelfield(drawelement_world_rect, sp_ID.stringValue, HudFilled.实体, HudColor.深空灰, Color.white, TextAnchor.MiddleCenter, new Vector2(0, -1.5f), 11, true, TextClipping.Ellipsis, true, Font_Light);
                    }
                    if (e.type == EventType.MouseDown && e.button == 0 && drawelement_world_rect.Contains(e.mousePosition))
                    {
                        EditorGUIUtility.systemCopyBuffer = sp_ID.stringValue;
                        EditorApplication.delayCall += () =>
                        {
                            string hexcol = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary, true);
                            string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 布局元素生成器消息", "拷贝元素项ID", $"已将元素项的ID： <color={hexcol}><b>   {sp_ID.stringValue}   </b></color>拷贝至系统剪贴板中！", "明白", 0);
                        };
                        e.Use();
                    }
                    #endregion

                    #region 折叠开关
                    if (!sp_isFold.boolValue)
                    {
                        drawelement_world_rect.Set(rect.x + 2, rect.y + 25, 12, 12);
                        if (Editor_XHud_GUI.Gui_Button(drawelement_world_rect, icon_icon_unfold_r, icon_icon_unfold_p, true, "", "", Color.white))
                        {
                            for (int i = 0; i < ElementList_World.count; i++)
                            {
                                SerializedProperty name = SpawnItemList_World.GetArrayElementAtIndex(i).FindPropertyRelative("SpawnName");
                                SerializedProperty foldstate = SpawnItemList_World.GetArrayElementAtIndex(i).FindPropertyRelative("isFold");
                                foldstate.boolValue = true;
                                foldstate.serializedObject.ApplyModifiedProperties();
                            }

                            sp_isFold.boolValue = true;
                            sp_isFold.serializedObject.ApplyModifiedProperties();
                        }
                    }
                    else
                    {
                        drawelement_world_rect.Set(rect.x + 2, rect.y + 25, 12, 12);
                        if (Editor_XHud_GUI.Gui_Button(drawelement_world_rect, icon_anim_fold_r, icon_anim_fold_p, true, "", "", Color.white))
                        {
                            for (int i = 0; i < ElementList_World.count; i++)
                            {
                                SerializedProperty name = SpawnItemList_World.GetArrayElementAtIndex(i).FindPropertyRelative("SpawnName");
                                SerializedProperty foldstate = SpawnItemList_World.GetArrayElementAtIndex(i).FindPropertyRelative("isFold");
                                foldstate.boolValue = true;
                                foldstate.serializedObject.ApplyModifiedProperties();
                            }
                            sp_isFold.boolValue = false;
                            sp_isFold.serializedObject.ApplyModifiedProperties();
                        }
                    }
                    #endregion

                    #region AutoIn
                    GUI.backgroundColor = XHud_Dashboard.Theme_Primary;
                    drawelement_world_rect.Set(rect.width - 15, rect.y + 35, 55, 12);
                    if (EditorGUIUtility.currentViewWidth >= 137)
                        Editor_XHud_GUI.Gui_PopupWithString(drawelement_world_rect, ref sp_AutoIn, new string[2] { "自动 In", "手动 In" }, HudFilled.无, HudColor.亮白, Color.white * 0.85f);
                    GUI.backgroundColor = Color.white;
                    #endregion

                    #region 使用生成器的通用动效
                    GUI.backgroundColor = XHud_Dashboard.Theme_Primary;
                    drawelement_world_rect.Set(rect.width - 95, rect.y + 35, 88, 12);
                    if (EditorGUIUtility.currentViewWidth >= 210)
                        Editor_XHud_GUI.Gui_PopupWithString(drawelement_world_rect, ref sp_UseSpawnerMotion, new string[2] { "自身动效", "生成器动效" }, HudFilled.无, HudColor.亮白, Color.white * 0.85f);
                    GUI.backgroundColor = Color.white;
                    #endregion

                    float baseheight = rect.y + 52;

                    #region 正在生成 / 回收进度条
                    drawelement_world_rect.Set(rect.x, baseheight, rect.width, 1);
                    Editor_XHud_GUI.Gui_Box(drawelement_world_rect, Color.gray);

                    if (sp_InMotion.boolValue)
                    {
                        GUI.backgroundColor = Color.white;
                        Repaint();
                        drawelement_world_rect.Set(rect.x, baseheight, rect.width * sp_MotionPercentage.floatValue, 1);
                        Editor_XHud_GUI.Gui_Box(drawelement_world_rect, XHud_Dashboard.Theme_Primary);
                    }
                    #endregion

                    #region 延迟参数
                    drawelement_world_rect.Set(rect.x, baseheight + 10, rect.width / 2 - 5, 18);
                    Editor_XHud_GUI.Gui_Property_Field(drawelement_world_rect, "延迟生成", "ds", sp_Delay_Spawn);

                    drawelement_world_rect.Set(rect.x + rect.width / 2 + 5, baseheight + 10, rect.width / 2 - 5, 18);
                    Editor_XHud_GUI.Gui_Property_Field(drawelement_world_rect, "延迟回收", "dd", sp_Delay_Despawn);
                    #endregion

                    #region 信息
                    if (!sp_isFold.boolValue)
                    {
                        string hexcol = XGUI_Utilitys.Color_To_HexString(Color.white * 0.75f, true);
                        string darkcol = XGUI_Utilitys.Color_To_HexString(Color.gray, true);

                        drawelement_world_rect.Set(rect.x, baseheight + 40, rect.width / 2 - 5, 20);
                        Editor_XHud_GUI.Gui_Labelfield(drawelement_world_rect, $"<color={hexcol}>pos：</color> <color={darkcol}>{sp_Position.vector3Value}</color>", HudFilled.无, HudColor.深空灰, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 11, true, TextClipping.Ellipsis, true, Font_Light);

                        drawelement_world_rect.Set(rect.x + rect.width / 2 + 5, baseheight + 40, rect.width / 2 - 5, 20);
                        Editor_XHud_GUI.Gui_Labelfield(drawelement_world_rect, $"<color={hexcol}>rot：</color> <color={darkcol}>{sp_Euler.vector3Value}</color>", HudFilled.无, HudColor.深空灰, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 11, true, TextClipping.Ellipsis, true, Font_Light);

                        drawelement_world_rect.Set(rect.x, baseheight + 65, rect.width / 2 - 5, 20);
                        Editor_XHud_GUI.Gui_Labelfield(drawelement_world_rect, $"<color={hexcol}>offset：</color> <color={darkcol}>{sp_Offset.vector3Value}</color>", HudFilled.无, HudColor.深空灰, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 11, true, TextClipping.Ellipsis, true, Font_Light);

                        drawelement_world_rect.Set(rect.x + rect.width / 2 + 5, baseheight + 65, rect.width / 2 - 5, 20);
                        Editor_XHud_GUI.Gui_Labelfield(drawelement_world_rect, $"<color={hexcol}>scl：</color> <color={darkcol}>{sp_Scale.vector3Value}</color>", HudFilled.无, HudColor.深空灰, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 11, true, TextClipping.Ellipsis, true, Font_Light);

                        drawelement_world_rect.Set(rect.x, baseheight + 90, rect.width, 1);
                        Editor_XHud_GUI.Gui_Box(drawelement_world_rect, Color.gray * 0.7f);

                        drawelement_world_rect.Set(rect.x, baseheight + 98, rect.width / 2 - 5, 20);
                        Editor_XHud_GUI.Gui_Labelfield(drawelement_world_rect, $"<color={hexcol}>size：</color> <color={darkcol}>{sp_Size.vector2Value}</color>", HudFilled.无, HudColor.深空灰, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 11, true, TextClipping.Ellipsis, true, Font_Light);

                        drawelement_world_rect.Set(rect.x + rect.width / 2 + 5, baseheight + 98, rect.width / 2 - 5, 20);
                        Editor_XHud_GUI.Gui_Labelfield(drawelement_world_rect, $"<color={hexcol}>pivot：</color> <color={darkcol}>{sp_Pivot.vector2Value}</color>", HudFilled.无, HudColor.深空灰, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 11, true, TextClipping.Ellipsis, true, Font_Light);

                        drawelement_world_rect.Set(rect.x, baseheight + 118, rect.width / 2 - 5, 20);
                        Editor_XHud_GUI.Gui_Labelfield(drawelement_world_rect, $"<color={hexcol}>anc min：</color> <color={darkcol}>{sp_Anchor_Min.vector2Value}</color>", HudFilled.无, HudColor.深空灰, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 11, true, TextClipping.Ellipsis, true, Font_Light);

                        drawelement_world_rect.Set(rect.x + rect.width / 2 + 5, baseheight + 118, rect.width / 2 - 5, 20);
                        Editor_XHud_GUI.Gui_Labelfield(drawelement_world_rect, $"<color={hexcol}>anc max：</color> <color={darkcol}>{sp_Anchor_Max.vector2Value}</color>", HudFilled.无, HudColor.深空灰, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 11, true, TextClipping.Ellipsis, true, Font_Light);
                    }
                    #endregion

                    #region  右键菜单
                    drawelement_world_rect.Set(rect.x, rect.y, rect.width - 50, rect.height);
                    if (e.type == EventType.MouseDown && e.button == 1 && drawelement_world_rect.Contains(e.mousePosition))
                    {
                        // 创建右键菜单
                        GenericMenu menu = new GenericMenu();
                        menu.AddItem(new GUIContent("E 修改动效"), false, () =>
                        {
                            if (!IsLoadedLayout_World.boolValue && sp_UseSpawnerMotion.stringValue == "元素自身动效")
                            {
                                Debug.Log("在 \"元素自身动效\" 模式下，前提要先把元素加载才可以对元素自身动效参数进行修改");
                                return;
                            }

                            XHud_LibraryArg_Motion motion = new XHud_LibraryArg_Motion("", 0, "",
                               sp_UseSpawnerMotion.stringValue == "列表项动效" ? BaseScript.SpawnItemList_World[index].CreateArgs : sp_UseSpawnerMotion.stringValue == "元素自身动效" ? BaseScript.SpawnItemList_World[index].SpawnedElementNode.Element.CreateArgs : BaseScript.CreateArgs,
                               sp_UseSpawnerMotion.stringValue == "列表项动效" ? BaseScript.SpawnItemList_World[index].RecycleArgs : sp_UseSpawnerMotion.stringValue == "元素自身动效" ? BaseScript.SpawnItemList_World[index].SpawnedElementNode.Element.RecycleArgs : BaseScript.RecycleArgs);
                            OpenParameterModifier(motion, sp_SpawnName.stringValue, HudElementMotionType.Creator);
                            return;
                        });
                        // 显示右键菜单
                        menu.ShowAsContext();

                        e.Use(); // 标记事件已被处理，防止其他操作处理该事件                            
                    }
                    GUI.enabled = true;
                    #endregion
                },
                elementHeightCallback = index =>
                {
                    float height = 0;

                    if (BaseScript.SpawnItemList_World[index].isFold)
                    {
                        height = 90;
                    }
                    else
                    {
                        height = 200;
                    }
                    return height;
                },
                onMouseUpCallback = list =>
                {
                    selected_World = list.index;
                },
                onSelectCallback = list =>
                {
                    selected_World = list.index;

                    if (BaseScript.SpawnItemList_World.Count > 0)
                    {
                        XHud_Module_Element element = BaseScript.SpawnItemList_World[list.index].SpawnedElementNode.Element;

                        if (element != null)
                        {
                            EditorGUIUtility.PingObject(element);
                            // 元素高亮闪烁
                            XTween_Preview_Start(new XTween_Interface[] { element.element_Highlight() }, XHudSpace.世界空间);
                        }
                    }
                }
            };
            #endregion

            //// 从元素库中同步动效参数
            //BaseScript.SyncLayoutItemsMotionArgs();
        }

        private void OnDisable()
        {
            if (target != null)
            {
                for (int i = 0; i < ElementList_Screen.count; i++)
                {
                    SerializedProperty name = SpawnItemList_Screen.GetArrayElementAtIndex(i).FindPropertyRelative("SpawnName");
                    SerializedProperty foldstate = SpawnItemList_Screen.GetArrayElementAtIndex(i).FindPropertyRelative("isFold");
                    foldstate.boolValue = true;
                    foldstate.serializedObject.ApplyModifiedProperties();
                }

                for (int i = 0; i < ElementList_World.count; i++)
                {
                    SerializedProperty name = SpawnItemList_World.GetArrayElementAtIndex(i).FindPropertyRelative("SpawnName");
                    SerializedProperty foldstate = SpawnItemList_World.GetArrayElementAtIndex(i).FindPropertyRelative("isFold");
                    foldstate.boolValue = true;
                    foldstate.serializedObject.ApplyModifiedProperties();
                }

                //  停止预览
                StopPreviewing();

                Editor_XHud_Tool_SceneView_Activate_Mark.SetEnabled(false);
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            Editor_XHud_GUI.Gui_Layout_Banner(icon_main, HudFilled.实体, HudColor.深空灰, "XHud - 布局元素生成器", Color.white);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            #region 状态
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 0, "状态", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            Editor_XHud_GUI.StatuDisplayer_text(null, 12, new Vector2(0, 7), "生成器状态", 12, spawnerisRunning.boolValue ? "运行中" : "待命中", XHud_Dashboard.Theme_Primary, 11, false);

            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 选项
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 0, "选项", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_LayoutSpawner>("调试信息", stroptions_enabled, ref UseDebug, HudFilled.无, HudFilled.实体, Color.white * 0.9f, 120, 22, SelectedObjects);

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_LayoutSpawner>("手动生成与回收", stroptions_enabled, ref UseManullyKey, HudFilled.无, HudFilled.实体, Color.white * 0.9f, 120, 22, SelectedObjects);

            if (UseManullyKey.boolValue)
            {
                Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_LayoutSpawner>("屏幕元素控制", stroptions_control, ref KeyControl_Screen, HudFilled.无, HudFilled.实体, Color.white * 0.9f, 120, 22, SelectedObjects);

                Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_LayoutSpawner>("世界元素控制", stroptions_control, ref KeyControl_World, HudFilled.无, HudFilled.实体, Color.white * 0.9f, 120, 22, SelectedObjects);
            }

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_LayoutSpawner>("使用实例化", stroptions_enabled, ref UseInstantiateMode, HudFilled.无, HudFilled.实体, Color.white * 0.9f, 120, 22, SelectedObjects);

            if (HudManager == null)
            {
                EditorGUILayout.HelpBox("未找到Hud管理器!", MessageType.Error);
            }
            else
            {
                if (HudManager.Hud_ElementLibrarys == null || HudManager.Hud_ElementLibrarys.Count <= 0)
                {
                    EditorGUILayout.HelpBox("未找到任何已指定的元素库，请先添加元素库项!", MessageType.Warning);
                }
                else
                {
                    #region 选择元素库
                    string[] libsname = HudManager.hm_ElementLibrary_GetAllLibraryNames();

                    Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_LayoutSpawner>("元素库", libsname, ref LibName, HudFilled.实体, 120, 22, SelectedObjects, (comps) => { }, (res) => { });
                    #endregion
                }
            }

            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 参数
            bool sw_option = xHud_FunctionGroup("参数", 5, HudFilled.纯色边框, HudColor.亮白, XHud_Dashboard.Theme_Primary, XHud_Dashboard.Theme_Primary, Color.gray, new RectOffset(0, 0, 0, 0), new Vector2(20, 0), PrefsKeyFold_Prams, null);
            if (sw_option)
            {
                Editor_XHud_GUI.Gui_Layout_Property_Field("生成器标识", SpawnerIndicator);

                if (UseManullyKey.boolValue)
                {
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Seperator(1, XHud_Dashboard.Theme_SeperateLine);
                    Editor_XHud_GUI.Gui_Layout_Space(10);

                    Editor_XHud_GUI.Gui_Layout_Property_Field("主要辅助按键", SpawnFunctionKey_Primary, 170);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Property_Field("次级辅助按键", SpawnFunctionKey_Secondary, 170);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Property_Field("生成按键", Key_Create, 170);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Property_Field("回收按键", Key_Recycle, 170);
                }

                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Seperator(1, XHud_Dashboard.Theme_SeperateLine);
                Editor_XHud_GUI.Gui_Layout_Space(10);

                SerializedProperty sp_anchor_type = CreateArgs.FindPropertyRelative("anchor");
                Editor_XHud_GUI.Gui_Layout_Property_Field("锚点", sp_anchor_type);
            }
            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 元素项

            string hexcol = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary, true);

            #region 屏幕空间
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 0, "元素项 - 屏幕空间", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(15);

            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            #region 收集 & 重置
            if (SpawnItemList_Screen.arraySize <= 0)
            {
                #region 收集布局                
                if (Editor_XHud_GUI.Gui_Layout_Button(15, "收集当前在LayoutScreen锚点下的所有屏幕元素部署布局", icon_scan_r, icon_scan_p))
                {
                    if (string.IsNullOrEmpty(LibName.stringValue))
                    {
                        Editor_XHud_GUI.Open(XHud_DialogType.警告, $"XHud - 布局元素生成器消息", "收集元素异常", $"请先指定目标元素库！", "明白", 0);
                        return;
                    }
                    if (HudManager == null)
                        return;

                    ClearSpawnItemList_Screen();
                    HudElementInfo ele_info_s = AnchorElementInfosGet_Screen();

                    //无效名称列表
                    List<XHud_GUI_Dialog_ListDatas> DialogDatas = new List<XHud_GUI_Dialog_ListDatas>();

                    //---获取元素
                    for (int i = 0; i < ele_info_s.elements.Count; i++)
                    {
                        string item_name = PrefabUtility.GetCorrespondingObjectFromOriginalSource(ele_info_s.elements[i]).name;
                        if (!IsInLibrary(item_name))
                        {
                            #region 加入非库中元素名称列表
                            string s = item_name;
                            XHud_GUI_Dialog_ListDatas data = new XHud_GUI_Dialog_ListDatas();
                            data.Title = SpawnerIndicator.stringValue;
                            data.SubTitle = "非库中元素";
                            data.Message = s;
                            DialogDatas.Add(data);
                            #endregion
                            continue;
                        }
                        else
                        {
                            #region 新建一个生成项，用于快速收集目标元素的基础信息（坐标、缩放、尺寸）
                            XHud_LayoutSpawner_Item item = new XHud_LayoutSpawner_Item();
                            item.SpawnName = item_name;
                            item.isEnabled = true;
                            item.isFold = true;
                            item.Position = ele_info_s.elements[i].RectTransform.anchoredPosition3D;
                            item.Scale = ele_info_s.elements[i].RectTransform.localScale;
                            item.Euler = ele_info_s.elements[i].RectTransform.localEulerAngles;
                            item.Size = ele_info_s.elements[i].RectTransform.sizeDelta;
                            item.Pivot = ele_info_s.elements[i].RectTransform.pivot;
                            item.Anchor_Min = ele_info_s.elements[i].RectTransform.anchorMin;
                            item.Anchor_Max = ele_info_s.elements[i].RectTransform.anchorMax;
                            item.CreateArgs = ele_info_s.elements[i].CreateArgs.Clone();
                            item.RecycleArgs = ele_info_s.elements[i].RecycleArgs.Clone();
                            #endregion

                            #region 判断收集的元素在哪个锚点下
                            XHudAnchor anchor = XHudAnchor.中心;
                            string ele_parent_name = ele_info_s.elements[i].RectTransform.parent.name;

                            if (ele_parent_name == "Anchor_B")
                            {
                                anchor = XHudAnchor.底层;
                            }
                            if (ele_parent_name == "Anchor_C")
                            {
                                anchor = XHudAnchor.中心;
                            }
                            if (ele_parent_name == "Anchor_L")
                            {
                                anchor = XHudAnchor.左;
                            }
                            if (ele_parent_name == "Anchor_R")
                            {
                                anchor = XHudAnchor.右;
                            }
                            if (ele_parent_name == "Anchor_U")
                            {
                                anchor = XHudAnchor.上;
                            }
                            if (ele_parent_name == "Anchor_D")
                            {
                                anchor = XHudAnchor.下;
                            }
                            if (ele_parent_name == "Anchor_L_U")
                            {
                                anchor = XHudAnchor.左上;
                            }
                            if (ele_parent_name == "Anchor_L_D")
                            {
                                anchor = XHudAnchor.左下;
                            }
                            if (ele_parent_name == "Anchor_R_U")
                            {
                                anchor = XHudAnchor.右上;
                            }
                            if (ele_parent_name == "Anchor_R_D")
                            {
                                anchor = XHudAnchor.右下;
                            }
                            if (ele_parent_name == "Anchor_T")
                            {
                                anchor = XHudAnchor.顶层;
                            }
                            item.CreateArgs.anchor = anchor;
                            #endregion

                            //加入屏幕列表中
                            AddedSpawnItem_Screen(item);
                        }
                    }

                    #region 异常提示
                    //如果获取到无效元素名称
                    if (DialogDatas.Count > 0)
                    {
                        Editor_XHud_GUI.Open(DialogDatas.ToArray(), XHud_DialogType.警告, "XHud - 布局元素生成器消息", "元素物体异常", $"检测到以下不合理的物体，原因：未加入到元素池中。解决方法：如果您想让这些元素被记录到部署布局中，请将其加入元素池！", "明白", 0, true);
                    }

                    //如果获取到非预制体元素名称
                    if (ele_info_s.notPrefabsList.Count > 0)
                    {
                        DialogDatas.Clear();
                        for (int w = 0; w < ele_info_s.notPrefabsList.Count; w++)
                        {
                            string s = ele_info_s.notPrefabsList[w].name;

                            #region 加入非预制体元素名称列表
                            XHud_GUI_Dialog_ListDatas data = new XHud_GUI_Dialog_ListDatas();
                            data.Title = SpawnerIndicator.stringValue;
                            data.SubTitle = "非预制体元素";
                            data.Message = s;
                            DialogDatas.Add(data);
                            #endregion
                        }

                        string res = Editor_XHud_GUI.Open(DialogDatas.ToArray(), XHud_DialogType.警告, "XHud - 布局元素生成器消息", "元素物体异常", $"检测到以下不合理的物体，原因：物体虽然是HudElement元素但未将其制成预制体。解决方法：如果您想让这些元素被记录到部署布局中，请将其制成预制体！", "明白", "选中", 0, true);
                        if (res == "选中")
                        {
                            Selection.objects = ele_info_s.notPrefabsList.ToArray();
                        }
                    }
                    #endregion

                    #region 记录后处理元素方式
                    if (ele_info_s.elements.Count > 0)
                    {
                        string res = Editor_XHud_GUI.Open(XHud_DialogType.帮助, "XHud - 布局元素生成器消息", "元素物体处理", $"如何处理场景中被记录的屏幕UI的HudElement元素物体？", "不做处理", "移除", "隐藏", 0, true);
                        if (res == "不做处理")
                        {

                        }
                        else if (res == "移除")
                        {
                            for (int i = 0; i < ele_info_s.elements.Count; i++)
                            {
                                Undo.DestroyObjectImmediate(ele_info_s.elements[i].gameObject);
                            }
                        }
                        else if (res == "隐藏")
                        {
                            for (int i = 0; i < ele_info_s.elements.Count; i++)
                            {
                                Undo.RecordObject(ele_info_s.elements[i].gameObject, "HideOriginalElementPrefab_" + i);
                                ele_info_s.elements[i].gameObject.SetActive(false);
                            }
                        }
                    }
                    else
                    {
                        Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 布局元素生成器消息", "元素物体收集", $"未发现<color={hexcol}> 屏幕根节点 </color>下存在任何Hud元素物体！", "明白", 0);
                    }
                    #endregion
                    return;
                }
                #endregion
            }
            else
            {
                #region 重置清空
                if (Editor_XHud_GUI.Gui_Layout_Button(15, "将当前的元素部署布局列表清空", icon_reset_r, icon_reset_p))
                {
                    EditorApplication.delayCall += () =>
                    {
                        string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 布局元素生成器消息", "重置元素列表", "确定要将元素列表重置吗？您将丢失当前的所有元素记录和他们的延迟（生成 & 回收）参数！", "重置", "暂不", 0);
                        if (res == "暂不")
                            return;

                        if (HudManager == null)
                            return;

                        IsLoadedLayout_Screen.boolValue = false;
                        IsLoadedLayout_Screen.serializedObject.ApplyModifiedProperties();
                        for (int i = 0; i < SpawnItemList_Screen.arraySize; i++)
                        {
                            UnLoadSpawnItem_Screen(BaseScript.SpawnItemList_Screen[i]);
                        }
                        ClearSpawnItemList_Screen();
                    };
                }
                #endregion

                Editor_XHud_GUI.Gui_Layout_Space(40);

                #region 重新生成ID
                if (Editor_XHud_GUI.Gui_Layout_Button(15, "为当前的元素列表中的项重新创建ID", icon_recreate_id_r, icon_recreate_id_p))
                {
                    EditorApplication.delayCall += () =>
                    {
                        string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 布局元素生成器消息", "元素ID刷新", "确定要将元素列表中的所有项的ID进行重新生成吗？您将丢失所有元素的当前ID！我们并不建议您这么做，因为会导致您其他脚本在获取元素时发生\" 无法找到目标元素 \"的异常！", "重新生成", "暂不", 0);
                        if (res == "暂不")
                            return;

                        Undo.RecordObject(BaseScript, "刷新所有元素项ID");
                        for (int i = 0; i < BaseScript.SpawnItemList_Screen.Count; i++)
                        {
                            BaseScript.SpawnItemList_Screen[i].ID = XGUI_Utilitys.GenerateUniqueId(CollectIDs(XHudSpace.屏幕空间));
                        }
                    };
                }
                #endregion
            }
            #endregion

            Editor_XHud_GUI.Gui_Layout_Space(40);

            #region 加载 & 卸载布局 & 预览
            if (SpawnItemList_Screen.arraySize > 0)
            {
                #region 加载 & 卸载布局 
                if (!IsLoadedLayout_Screen.boolValue)
                {
                    #region 加载
                    if (Editor_XHud_GUI.Gui_Layout_Button(15, "根据元素列表中的信息重建元素部署布局到LayoutScreen锚点下", icon_load_r, icon_load_p))
                    {
                        if (HudManager == null)
                            return;
                        if (IsLoadedLayout_Screen.boolValue)
                            return;
                        IsLoadedLayout_Screen.boolValue = true;
                        IsLoadedLayout_Screen.serializedObject.ApplyModifiedProperties();

                        for (int i = 0; i < SpawnItemList_Screen.arraySize; i++)
                        {
                            LoadSpawnItem_Screen(BaseScript.SpawnItemList_Screen[i]);
                        }

                        // 从元素库中同步动效参数
                        //BaseScript.SyncLayoutItemsMotionArgs();

                        if (BaseScript.act_on_spawn != null)
                            BaseScript.act_on_spawn(XHudSpace.屏幕空间);
                    }
                    #endregion
                }
                else
                {
                    #region 卸载
                    if (Editor_XHud_GUI.Gui_Layout_Button(15, "将当前重建的元素部署布局清空", icon_unload_r, icon_unload_p))
                    {
                        if (HudManager == null)
                            return;

                        if (!IsLoadedLayout_Screen.boolValue)
                            return;
                        IsLoadedLayout_Screen.boolValue = false;
                        IsLoadedLayout_Screen.serializedObject.ApplyModifiedProperties();

                        //  停止预览
                        StopPreviewing();

                        for (int i = 0; i < SpawnItemList_Screen.arraySize; i++)
                        {
                            UnLoadSpawnItem_Screen(BaseScript.SpawnItemList_Screen[i]);
                        }

                        // 从元素库中同步动效参数
                        //BaseScript.SyncLayoutItemsMotionArgs();

                        if (BaseScript.act_on_despawn != null)
                            BaseScript.act_on_despawn(XHudSpace.屏幕空间);
                    }
                    #endregion
                }
                #endregion

                Editor_XHud_GUI.Gui_Layout_Space(40);

                #region 动画预览        
                if (IsLoadedLayout_Screen.boolValue)
                {
                    #region 预览播放 - 生成
                    if (!IsPreviewing.boolValue && Editor_XHud_GUI.Gui_Layout_Button(15, "预览当前加载的元素的 - 生成 - 动效", icon_preview_in_r, icon_preview_in_p))
                    {
                        if (HudManager == null)
                            return;

                        IsPreviewing.boolValue = true;
                        LayoutsTweenPreview(XHudSpace.屏幕空间, XHudElementCreateState.Created);
                    }
                    #endregion

                    #region 预览停止 - 生成
                    if (IsPreviewing.boolValue && Editor_XHud_GUI.Gui_Layout_Button(15, "停止预览当前播放的元素的 - 生成 - 动效", icon_stoppreview_r, icon_stoppreview_p))
                    {
                        if (HudManager == null)
                            return;

                        IsPreviewing.boolValue = false;
                        XTween_Preview_Kill(XHudSpace.屏幕空间);
                    }
                    #endregion

                    Editor_XHud_GUI.Gui_Layout_Space(40);

                    #region 预览播放 - 回收
                    if (!IsPreviewing.boolValue && Editor_XHud_GUI.Gui_Layout_Button(15, "预览当前加载的元素的 - 回收 - 动效", icon_preview_out_r, icon_preview_out_p))
                    {
                        if (HudManager == null)
                            return;

                        IsPreviewing.boolValue = true;
                        LayoutsTweenPreview(XHudSpace.屏幕空间, XHudElementCreateState.Recycled);
                    }
                    #endregion

                    #region 预览停止 - 回收
                    if (IsPreviewing.boolValue && Editor_XHud_GUI.Gui_Layout_Button(15, "停止预览当前播放的元素的 - 回收 - 动效", icon_stoppreview_r, icon_stoppreview_p))
                    {
                        if (HudManager == null)
                            return;

                        IsPreviewing.boolValue = false;
                        XTween_Preview_Kill(XHudSpace.屏幕空间);
                    }
                    #endregion
                }
                #endregion
            }
            #endregion

            Editor_XHud_GUI.Gui_Layout_FlexSpace();
            Editor_XHud_GUI.Gui_Layout_Horizontal_End();

            Editor_XHud_GUI.Gui_Layout_Space(10);

            FoldScreen.boolValue = EditorGUILayout.Foldout(FoldScreen.boolValue, new GUIContent("元素项列表"), true);
            if (FoldScreen.boolValue)
            {
                Editor_XHud_GUI.Gui_Layout_Space(5);
                ElementList_Screen.DoLayoutList();
            }

            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 世界空间
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 0, "元素项 - 世界空间", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            #region 收集 & 重置
            if (SpawnItemList_World.arraySize <= 0)
            {
                #region 收集布局
                if (Editor_XHud_GUI.Gui_Layout_Button(15, "收集当前在LayoutScreen锚点下的所有屏幕元素部署布局", icon_scan_r, icon_scan_p))
                {
                    if (string.IsNullOrEmpty(LibName.stringValue))
                    {
                        Editor_XHud_GUI.Open(XHud_DialogType.警告, $"XHud - 布局元素生成器消息", "收集元素异常", $"请先指定目标元素库！", "明白", 0);
                        return;
                    }

                    if (HudManager == null)
                        return;

                    if (!HudManager.SupportWorldUI)
                    {
                        string hex_col = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary, true);
                        Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 布局元素生成器消息", "收集世界元素布局", $"暂未支持世界元素！请检查XHud 管理器中是否开启了<b><color={hex_col}> 支持世界元素 </color></b>选项？", "明白", 0);
                        return;
                    }

                    ClearSpawnItemList_World();
                    HudElementInfo ele_info_w = AnchorElementInfosGet_World();

                    //无效名称列表
                    List<XHud_GUI_Dialog_ListDatas> DialogDatas = new List<XHud_GUI_Dialog_ListDatas>();

                    //---获取
                    for (int i = 0; i < ele_info_w.elements.Count; i++)
                    {
                        string item_name = PrefabUtility.GetCorrespondingObjectFromOriginalSource(ele_info_w.elements[i]).name;
                        if (!IsInLibrary(item_name))
                        {
                            #region 加入非库中元素名称列表
                            string s = item_name;
                            XHud_GUI_Dialog_ListDatas data = new XHud_GUI_Dialog_ListDatas();
                            data.Title = SpawnerIndicator.stringValue;
                            data.SubTitle = "非库中元素";
                            data.Message = s;
                            DialogDatas.Add(data);
                            #endregion
                            continue;
                        }
                        else
                        {
                            #region 新建一个生成项，用于快速收集目标元素的基础信息（坐标、缩放、尺寸）
                            XHud_LayoutSpawner_Item item = new XHud_LayoutSpawner_Item();
                            item.SpawnName = item_name;
                            item.Position = ele_info_w.elements[i].RectTransform.position;
                            item.Scale = ele_info_w.elements[i].RectTransform.localScale;
                            item.Euler = ele_info_w.elements[i].RectTransform.localEulerAngles;
                            item.Size = ele_info_w.elements[i].RectTransform.sizeDelta;
                            item.CreateArgs = ele_info_w.elements[i].CreateArgs.Clone();
                            item.RecycleArgs = ele_info_w.elements[i].RecycleArgs.Clone();
                            AddedSpawnItem_World(item);
                            #endregion
                        }
                    }

                    #region 异常提示
                    //如果获取到无效元素名称
                    if (DialogDatas.Count > 0)
                    {
                        Editor_XHud_GUI.Open(DialogDatas.ToArray(), XHud_DialogType.警告, "XHud - 布局元素生成器消息", "元素物体异常", $"检测到以下不合理的物体，原因：未加入到元素池中。解决方法：如果您想让这些元素被记录到部署布局中，请将其加入元素池！", "明白", 0, true);
                    }

                    //如果获取到非预制体元素名称
                    if (ele_info_w.notPrefabsList.Count > 0)
                    {
                        DialogDatas.Clear();
                        for (int w = 0; w < ele_info_w.notPrefabsList.Count; w++)
                        {
                            string s = ele_info_w.notPrefabsList[w].name;

                            #region 加入非预制体元素名称列表
                            XHud_GUI_Dialog_ListDatas data = new XHud_GUI_Dialog_ListDatas();
                            data.Title = SpawnerIndicator.stringValue;
                            data.SubTitle = "非预制体元素";
                            data.Message = s;
                            DialogDatas.Add(data);
                            #endregion
                        }

                        string res = Editor_XHud_GUI.Open(DialogDatas.ToArray(), XHud_DialogType.警告, "XHud - 布局元素生成器消息", "元素物体异常", $"检测到以下不合理的物体，原因：物体虽然是HudElement元素但未将其制成预制体。解决方法：如果您想让这些元素被记录到部署布局中，请将其制成预制体！", "明白", "选中", 0, true);
                        if (res == "选中")
                        {
                            Selection.objects = ele_info_w.notPrefabsList.ToArray();
                        }
                    }
                    #endregion

                    #region 记录后处理元素方式
                    if (ele_info_w.elements.Count > 0)
                    {
                        string res = Editor_XHud_GUI.Open(XHud_DialogType.帮助, "XHud - 布局元素生成器消息", "元素物体处理", $"如何处理场景中被记录的世界UI的HudElement元素物体？", "不做处理", "移除", "隐藏", 0, true);
                        if (res == "不做处理")
                        {

                        }
                        else if (res == "移除")
                        {
                            for (int i = 0; i < ele_info_w.elements.Count; i++)
                            {
                                Undo.DestroyObjectImmediate(ele_info_w.elements[i].gameObject);
                            }
                        }
                        else if (res == "隐藏")
                        {
                            for (int i = 0; i < ele_info_w.elements.Count; i++)
                            {
                                Undo.RecordObject(ele_info_w.elements[i].gameObject, "HideOriginalElementPrefab_" + i);
                                ele_info_w.elements[i].gameObject.SetActive(false);
                            }
                        }
                    }
                    else
                    {
                        Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 布局元素生成器消息", "元素物体收集", $"未发现<color={hexcol}> 世界根节点 </color>下存在任何Hud元素物体！", "明白", 0);
                    }
                    #endregion               
                    return;
                }
                #endregion
            }
            else
            {
                #region 重置清空
                if (Editor_XHud_GUI.Gui_Layout_Button(15, "将当前的元素部署布局列表清空", icon_reset_r, icon_reset_p))
                {
                    EditorApplication.delayCall += () =>
                    {
                        string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 布局元素生成器消息", "重置元素列表", "确定要将元素列表重置吗？您将丢失当前的所有元素记录和他们的延迟（生成 & 回收）参数！", "重置", "暂不", 0);
                        if (res == "暂不")
                            return;

                        if (HudManager == null)
                            return;

                        IsLoadedLayout_World.boolValue = false;
                        IsLoadedLayout_World.serializedObject.ApplyModifiedProperties();
                        for (int i = 0; i < SpawnItemList_World.arraySize; i++)
                        {
                            UnLoadSpawnItem_World(BaseScript.SpawnItemList_World[i]);
                        }
                        ClearSpawnItemList_World();
                    };
                }
                #endregion

                Editor_XHud_GUI.Gui_Layout_Space(40);

                #region 重新生成ID
                if (Editor_XHud_GUI.Gui_Layout_Button(15, "为当前的元素列表中的项重新创建ID", icon_recreate_id_r, icon_recreate_id_p))
                {
                    EditorApplication.delayCall += () =>
                    {
                        string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 布局元素生成器消息", "元素ID刷新", "确定要将元素列表中的所有项的ID进行重新生成吗？您将丢失所有元素的当前ID！我们并不建议您这么做，因为会导致您其他脚本在获取元素时发生\" 无法找到目标元素 \"的异常！", "重新生成", "暂不", 0);
                        if (res == "暂不")
                            return;

                        Undo.RecordObject(BaseScript, "刷新所有元素项ID");
                        for (int i = 0; i < BaseScript.SpawnItemList_World.Count; i++)
                        {
                            BaseScript.SpawnItemList_World[i].ID = XGUI_Utilitys.GenerateUniqueId(CollectIDs(XHudSpace.世界空间));
                        }
                    };
                }
                #endregion
            }
            #endregion

            Editor_XHud_GUI.Gui_Layout_Space(40);

            #region 加载 & 卸载布局 & 预览
            if (SpawnItemList_World.arraySize > 0)
            {
                #region 加载 & 卸载布局 
                if (!IsLoadedLayout_World.boolValue)
                {
                    #region 加载
                    if (Editor_XHud_GUI.Gui_Layout_Button(15, "根据元素列表中的信息重建元素部署布局到LayoutScreen锚点下", icon_load_r, icon_load_p))
                    {
                        if (HudManager == null)
                            return;
                        if (IsLoadedLayout_World.boolValue)
                            return;
                        IsLoadedLayout_World.boolValue = true;
                        IsLoadedLayout_World.serializedObject.ApplyModifiedProperties();

                        for (int i = 0; i < SpawnItemList_World.arraySize; i++)
                        {
                            LoadSpawnItem_World(BaseScript.SpawnItemList_World[i]);
                        }

                        // 从元素库中同步动效参数
                        //BaseScript.SyncLayoutItemsMotionArgs();

                        if (BaseScript.act_on_spawn != null)
                            BaseScript.act_on_spawn(XHudSpace.世界空间);
                    }
                    #endregion
                }
                else
                {
                    #region 卸载
                    if (Editor_XHud_GUI.Gui_Layout_Button(15, "将当前重建的元素部署布局清空", icon_unload_r, icon_unload_p))
                    {
                        if (HudManager == null)
                            return;

                        if (!IsLoadedLayout_World.boolValue)
                            return;
                        IsLoadedLayout_World.boolValue = false;
                        IsLoadedLayout_World.serializedObject.ApplyModifiedProperties();

                        //  停止预览
                        StopPreviewing();

                        for (int i = 0; i < SpawnItemList_World.arraySize; i++)
                        {
                            UnLoadSpawnItem_World(BaseScript.SpawnItemList_World[i]);
                        }

                        // 从元素库中同步动效参数
                        //BaseScript.SyncLayoutItemsMotionArgs();

                        if (BaseScript.act_on_despawn != null)
                            BaseScript.act_on_despawn(XHudSpace.世界空间);
                    }
                    #endregion
                }
                #endregion

                Editor_XHud_GUI.Gui_Layout_Space(40);

                #region 动画预览      
                if (IsLoadedLayout_World.boolValue)
                {
                    #region 预览播放 - 生成
                    if (!IsPreviewing.boolValue && Editor_XHud_GUI.Gui_Layout_Button(15, "预览当前加载的元素的 - 生成 - 动效", icon_preview_in_r, icon_preview_in_p))
                    {
                        if (HudManager == null)
                            return;

                        IsPreviewing.boolValue = true;
                        LayoutsTweenPreview(XHudSpace.世界空间, XHudElementCreateState.Created);
                    }
                    #endregion

                    #region 预览停止 - 生成
                    if (IsPreviewing.boolValue && Editor_XHud_GUI.Gui_Layout_Button(15, "停止预览当前播放的元素的 - 生成 - 动效", icon_stoppreview_r, icon_stoppreview_p))
                    {
                        if (HudManager == null)
                            return;

                        IsPreviewing.boolValue = false;
                        XTween_Preview_Kill(XHudSpace.世界空间);
                    }
                    #endregion

                    Editor_XHud_GUI.Gui_Layout_Space(40);

                    #region 预览播放 - 回收
                    if (!IsPreviewing.boolValue && Editor_XHud_GUI.Gui_Layout_Button(15, "预览当前加载的元素的 - 回收 - 动效", icon_preview_out_r, icon_preview_out_p))
                    {
                        if (HudManager == null)
                            return;

                        IsPreviewing.boolValue = true;
                        LayoutsTweenPreview(XHudSpace.世界空间, XHudElementCreateState.Recycled);
                    }
                    #endregion

                    #region 预览停止 - 回收
                    if (IsPreviewing.boolValue && Editor_XHud_GUI.Gui_Layout_Button(15, "停止预览当前播放的元素的 - 回收 - 动效", icon_stoppreview_r, icon_stoppreview_p))
                    {
                        if (HudManager == null)
                            return;

                        IsPreviewing.boolValue = false;
                        XTween_Preview_Kill(XHudSpace.世界空间);
                    }
                    #endregion
                }
                #endregion
            }
            #endregion            

            Editor_XHud_GUI.Gui_Layout_FlexSpace();
            Editor_XHud_GUI.Gui_Layout_Horizontal_End();

            Editor_XHud_GUI.Gui_Layout_Space(10);

            FoldWorld.boolValue = EditorGUILayout.Foldout(FoldWorld.boolValue, new GUIContent("元素项列表"), true);
            if (FoldWorld.boolValue)
            {
                Editor_XHud_GUI.Gui_Layout_Space(5);
                ElementList_World.DoLayoutList();
            }

            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #endregion

            #region 动效参数
            bool sw_motion = xHud_FunctionGroup("动效参数", 5, HudFilled.纯色边框, HudColor.亮白, XHud_Dashboard.Theme_Primary, XHud_Dashboard.Theme_Primary, Color.gray, new RectOffset(0, 0, 0, 0), new Vector2(20, 0), PrefsKeyFold_Motion, null);
            if (sw_motion)
            {
                #region 模版库                             
                //确保动效库存在
                if (HudManager.Hud_Motions != null)
                {
                    //确保动效库不是空的
                    if (HudManager.Hud_Motions.ElementMotionList != null && HudManager.Hud_Motions.ElementMotionList.Count > 0)
                    {
                        Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);

                        //动效列表
                        string[] motnames = HudManager.Hud_Motions.ElementMotion_GetAllName_With_Create();
                        EditorGUI.BeginChangeCheck();
                        Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_LayoutSpawner>("生成", motnames, ref Crc_Lib_Name, HudFilled.实体, 120, 22, SelectedObjects);
                        if (EditorGUI.EndChangeCheck())
                        {
                            Motion_Creator crc = HudManager.Hud_Motions.ElementMotion_GetElementCreator_At_Create(Crc_Lib_Name.stringValue);

                            CreateArgs.FindPropertyRelative("anchor").enumValueIndex = (int)crc.anchor;
                            CreateArgs.FindPropertyRelative("Movement.Movement").enumValueIndex = (int)crc.Movement.Movement;
                            CreateArgs.FindPropertyRelative("Movement.Distance").floatValue = crc.Movement.Distance;
                            CreateArgs.FindPropertyRelative("Movement.Duration").floatValue = crc.Movement.Duration;
                            CreateArgs.FindPropertyRelative("Movement.Delay").floatValue = crc.Movement.Delay;
                            CreateArgs.FindPropertyRelative("Movement.Curve").animationCurveValue = crc.Movement.Curve;
                            CreateArgs.FindPropertyRelative("Movement.CurveName").stringValue = crc.Movement.CurveName;
                            CreateArgs.FindPropertyRelative("Movement.Ease").enumValueIndex = (int)crc.Movement.Ease;
                            CreateArgs.FindPropertyRelative("Rotation.Rotation").enumValueIndex = (int)crc.Rotation.Rotation;
                            CreateArgs.FindPropertyRelative("Rotation.Degree").floatValue = crc.Rotation.Degree;
                            CreateArgs.FindPropertyRelative("Rotation.Duration").floatValue = crc.Rotation.Duration;
                            CreateArgs.FindPropertyRelative("Rotation.Delay").floatValue = crc.Rotation.Delay;
                            CreateArgs.FindPropertyRelative("Rotation.Curve").animationCurveValue = crc.Rotation.Curve;
                            CreateArgs.FindPropertyRelative("Rotation.CurveName").stringValue = crc.Rotation.CurveName;
                            CreateArgs.FindPropertyRelative("Rotation.Ease").enumValueIndex = (int)crc.Rotation.Ease;
                            CreateArgs.FindPropertyRelative("Alpha.Duration").floatValue = crc.Alpha.Duration;
                            CreateArgs.FindPropertyRelative("Alpha.Delay").floatValue = crc.Alpha.Delay;
                            CreateArgs.FindPropertyRelative("Alpha.Curve").animationCurveValue = crc.Alpha.Curve;
                            CreateArgs.FindPropertyRelative("Alpha.CurveName").stringValue = crc.Alpha.CurveName;
                            CreateArgs.FindPropertyRelative("Alpha.Ease").enumValueIndex = (int)crc.Alpha.Ease;
                            CreateArgs.FindPropertyRelative("MotionAnimateEndState").enumValueIndex = (int)crc.MotionAnimateEndState;
                            CreateArgs.serializedObject.ApplyModifiedProperties();
                        }

                        #region 保存 & 定位模板
                        Editor_XHud_GUI.Gui_Layout_Space(10);

                        if (Editor_XHud_GUI.Gui_Layout_Button(14, "保存", icon_save_r, icon_save_p, 2))
                        {
                            OpenParameterSetter(HudElementMotionType.Creator);
                            return;
                        }

                        Editor_XHud_GUI.Gui_Layout_Space(10);

                        if (Editor_XHud_GUI.Gui_Layout_Button(14, "定位", icon_locate_r, icon_locate_p, 2))
                        {
                            if (!HudManager.Hud_Motions.ElementMotion_IsExist(Crc_Lib_Name.stringValue))
                                return;
                            Editor_XHud_MenuItemsAction_OpenLibrary.open_elementmotion();
                            HudManager.Hud_Motions.ElementMotionLibrary_Location(Crc_Lib_Name.stringValue);
                            return;
                        }

                        Editor_XHud_GUI.Gui_Layout_Space(10);

                        if (Editor_XHud_GUI.Gui_Layout_Button(14, "重置", icon_reset_r, icon_reset_p, 2))
                        {
                            string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 布局元素生成器消息", "重置动效参数", "确定要将动效参数重置吗？您将丢失当前的动效参数！", "重置", "暂不", 0);
                            if (res == "重置")
                                ResetMotionParams("c");
                            return;
                        }

                        Editor_XHud_GUI.Gui_Layout_Space(5);
                        #endregion

                        Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("未在动效库中发现任何动效资源，请先为其添加动效资源!", MessageType.Warning);
                        Editor_XHud_GUI.Gui_Layout_Space(5);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("Hud管理器中未指定动效库，请先配置动效库!", MessageType.Warning);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                }
                #endregion

                #region 位移
                Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_XHud_GUI.Gui_Layout_Space(10);
                create_fold_move.boolValue = EditorGUILayout.Foldout(create_fold_move.boolValue, "位移", true);
                create_fold_move.serializedObject.ApplyModifiedProperties();
                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                if (create_fold_move.boolValue)
                {
                    SerializedProperty sp_move_type = CreateArgs.FindPropertyRelative("Movement.Movement");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("方式", sp_move_type);

                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    SerializedProperty sp_move_dis = CreateArgs.FindPropertyRelative("Movement.Distance");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("距离", sp_move_dis);

                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    SerializedProperty sp_move_dur = CreateArgs.FindPropertyRelative("Movement.Duration");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("耗时", sp_move_dur);

                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    SerializedProperty sp_move_delay = CreateArgs.FindPropertyRelative("Movement.Delay");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("延迟", sp_move_delay);

                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    SerializedProperty sp_move_ease = CreateArgs.FindPropertyRelative("Movement.Ease");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("缓动", sp_move_ease);

                    if ((EaseMode)sp_move_ease.enumValueIndex == EaseMode.None)
                    {
                        Editor_XHud_GUI.Gui_Layout_Space(5);

                        SerializedProperty sp_move_curve = CreateArgs.FindPropertyRelative("Movement.Curve");
                        Editor_XHud_GUI.Gui_Layout_Property_Field("曲线", sp_move_curve);

                        Editor_XHud_GUI.Gui_Layout_Space(5);

                        #region 曲线列表
                        SerializedProperty sp_CurveName = CreateArgs.FindPropertyRelative("Movement.CurveName");
                        SerializedProperty sp_Curve = CreateArgs.FindPropertyRelative("Movement.Curve");
                        if (HudManager.Hud_Curves != null)
                        {
                            if (HudManager.Hud_Curves.CurveLibrary != null && HudManager.Hud_Curves.CurveLibrary.Count > 0)
                            {
                                string[] names = HudManager.Hud_Curves.CurveLibrary_GetCurveNames();
                                Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_LayoutSpawner>("曲线样式", names, ref sp_CurveName, HudFilled.实体, 120, 22, SelectedObjects, (comps) => { }, (res) =>
                                {
                                    sp_Curve.animationCurveValue = HudManager.Hud_Curves.CurveLibrary_GetCurve(res);
                                    sp_Curve.serializedObject.ApplyModifiedProperties();
                                });
                            }
                            else
                            {
                                EditorGUILayout.HelpBox("未在动效库中发现任何曲线资源，请先为其添加曲线资源!", MessageType.Warning);
                                Editor_XHud_GUI.Gui_Layout_Space(5);
                            }
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("Hud管理器中未指定曲线库，请先配置曲线库!", MessageType.Warning);
                            Editor_XHud_GUI.Gui_Layout_Space(5);
                        }
                        #endregion
                    }
                }
                #endregion

                #region 旋转
                Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_XHud_GUI.Gui_Layout_Space(10);
                create_fold_rotate.boolValue = EditorGUILayout.Foldout(create_fold_rotate.boolValue, "旋转", true);
                create_fold_rotate.serializedObject.ApplyModifiedProperties();
                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                if (create_fold_rotate.boolValue)
                {
                    SerializedProperty sp_rot_type = CreateArgs.FindPropertyRelative("Rotation.Rotation");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("方式", sp_rot_type);

                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    SerializedProperty sp_rot_deg = CreateArgs.FindPropertyRelative("Rotation.Degree");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("角度", sp_rot_deg);

                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    SerializedProperty sp_rot_dur = CreateArgs.FindPropertyRelative("Rotation.Duration");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("耗时", sp_rot_dur);

                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    SerializedProperty sp_rot_delay = CreateArgs.FindPropertyRelative("Rotation.Delay");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("延迟", sp_rot_delay);

                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    SerializedProperty sp_rot_ease = CreateArgs.FindPropertyRelative("Rotation.Ease");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("缓动", sp_rot_ease);

                    Editor_XHud_GUI.Gui_Layout_Space(5);


                    if ((EaseMode)sp_rot_ease.enumValueIndex == EaseMode.None)
                    {
                        Editor_XHud_GUI.Gui_Layout_Space(5);

                        SerializedProperty sp_rot_curve = CreateArgs.FindPropertyRelative("Rotation.Curve");
                        Editor_XHud_GUI.Gui_Layout_Property_Field("曲线", sp_rot_curve);

                        Editor_XHud_GUI.Gui_Layout_Space(5);

                        #region 曲线列表
                        SerializedProperty sp_CurveName = CreateArgs.FindPropertyRelative("Rotation.CurveName");
                        SerializedProperty sp_Curve = CreateArgs.FindPropertyRelative("Rotation.Curve");
                        if (HudManager.Hud_Curves != null)
                        {
                            if (HudManager.Hud_Curves.CurveLibrary != null && HudManager.Hud_Curves.CurveLibrary.Count > 0)
                            {
                                string[] names = HudManager.Hud_Curves.CurveLibrary_GetCurveNames();
                                Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_LayoutSpawner>("曲线样式", names, ref sp_CurveName, HudFilled.实体, 120, 22, SelectedObjects, (comps) => { }, (res) =>
                                {
                                    sp_Curve.animationCurveValue = HudManager.Hud_Curves.CurveLibrary_GetCurve(res);
                                    sp_Curve.serializedObject.ApplyModifiedProperties();
                                });
                            }
                            else
                            {
                                EditorGUILayout.HelpBox("未在动效库中发现任何曲线资源，请先为其添加曲线资源!", MessageType.Warning);
                                Editor_XHud_GUI.Gui_Layout_Space(5);
                            }
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("Hud管理器中未指定曲线库，请先配置曲线库!", MessageType.Warning);
                            Editor_XHud_GUI.Gui_Layout_Space(5);
                        }
                        #endregion
                    }
                }
                #endregion

                #region 透明度
                Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_XHud_GUI.Gui_Layout_Space(10);
                create_fold_alpha.boolValue = EditorGUILayout.Foldout(create_fold_alpha.boolValue, "透明度", true);
                create_fold_alpha.serializedObject.ApplyModifiedProperties();
                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                if (create_fold_alpha.boolValue)
                {
                    SerializedProperty sp_alpha_type = CreateArgs.FindPropertyRelative("Alpha.Duration");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("耗时", sp_alpha_type);

                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    SerializedProperty sp_alpha_delay = CreateArgs.FindPropertyRelative("Alpha.Delay");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("延迟", sp_alpha_delay);

                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    SerializedProperty sp_alpha_ease = CreateArgs.FindPropertyRelative("Alpha.Ease");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("缓动", sp_alpha_ease);

                    Editor_XHud_GUI.Gui_Layout_Space(5);


                    if ((EaseMode)sp_alpha_ease.enumValueIndex == EaseMode.None)
                    {
                        Editor_XHud_GUI.Gui_Layout_Space(5);

                        SerializedProperty sp_alpha_curve = CreateArgs.FindPropertyRelative("Alpha.Curve");
                        Editor_XHud_GUI.Gui_Layout_Property_Field("曲线", sp_alpha_curve);

                        Editor_XHud_GUI.Gui_Layout_Space(5);

                        #region 曲线列表
                        SerializedProperty sp_CurveName = CreateArgs.FindPropertyRelative("Alpha.CurveName");
                        SerializedProperty sp_Curve = CreateArgs.FindPropertyRelative("Alpha.Curve");
                        if (HudManager.Hud_Curves != null)
                        {
                            if (HudManager.Hud_Curves.CurveLibrary != null && HudManager.Hud_Curves.CurveLibrary.Count > 0)
                            {
                                string[] names = HudManager.Hud_Curves.CurveLibrary_GetCurveNames();
                                Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_LayoutSpawner>("曲线样式", names, ref sp_CurveName, HudFilled.实体, 120, 22, SelectedObjects, (comps) => { }, (res) =>
                                {
                                    sp_Curve.animationCurveValue = HudManager.Hud_Curves.CurveLibrary_GetCurve(res);
                                    sp_Curve.serializedObject.ApplyModifiedProperties();
                                });
                            }
                            else
                            {
                                EditorGUILayout.HelpBox("未在动效库中发现任何曲线资源，请先为其添加曲线资源!", MessageType.Warning);
                                Editor_XHud_GUI.Gui_Layout_Space(5);
                            }
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("Hud管理器中未指定曲线库，请先配置曲线库!", MessageType.Warning);
                            Editor_XHud_GUI.Gui_Layout_Space(5);
                        }
                        #endregion
                    }
                }
                #endregion

                EditorGUI.BeginChangeCheck();
                Editor_XHud_GUI.Gui_Layout_Property_Field("动效结束时机", CreateArgs_MotionAnimateEndState, 85);
                if (EditorGUI.EndChangeCheck())
                {
                    MotionAnimateEndState state = (MotionAnimateEndState)CreateArgs_MotionAnimateEndState.enumValueIndex;
                    switch (state)
                    {
                        case MotionAnimateEndState.以_移动为准:
                            HudMotion_Movement m = (HudMotion_Movement)CreateArgs.FindPropertyRelative("Movement.Movement").enumValueIndex;
                            if (m == HudMotion_Movement.A_无运动)
                            {
                                Editor_XHud_GUI.Open(XHud_DialogType.警告, $"XHud - 布局元素生成器消息", "设定动画结束时机", $"当前位移方式为 <color={hexcol}> A_无运动 </color>，将发生<color={hexcol}> 动效动画无法正常结束</color>从而导致<color={hexcol}> 相应的事件和动作委托 </color>不能正确触发的情况 </color>！请将<color={hexcol}> 位移方式 </color>改为<color={hexcol}> 非无运动方式 </color>！", "明白", 0);
                                CreateArgs_MotionAnimateEndState.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
                            }
                            break;
                        case MotionAnimateEndState.以_旋转为准:
                            HudMotion_Rotation r = (HudMotion_Rotation)CreateArgs.FindPropertyRelative("Rotation.Rotation").enumValueIndex;
                            if (r == HudMotion_Rotation.A_无旋转)
                            {
                                Editor_XHud_GUI.Open(XHud_DialogType.警告, $"XHud - 布局元素生成器消息", "设定动画结束时机", $"当前旋转方式为 <color={hexcol}> A_无旋转 </color>，将发生<color={hexcol}> 动效动画无法正常结束</color>从而导致<color={hexcol}> 相应的事件和动作委托 </color>不能正确触发的情况 </color>！请将<color={hexcol}> 旋转方式 </color>改为<color={hexcol}> 非无旋转方式 </color>！", "明白", 0);
                                CreateArgs_MotionAnimateEndState.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
                            }
                            break;
                    }
                }

                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Seperator(1, XHud_Dashboard.Theme_SeperateLine);
                Editor_XHud_GUI.Gui_Layout_Space(5);

                #region 模版库                             
                //确保动效库存在
                if (HudManager.Hud_Motions != null)
                {
                    //确保动效库不是空的
                    if (HudManager.Hud_Motions.ElementMotionList != null && HudManager.Hud_Motions.ElementMotionList.Count > 0)
                    {
                        Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);

                        //动效列表
                        string[] motnames = HudManager.Hud_Motions.ElementMotion_GetAllName_With_Recycle();
                        EditorGUI.BeginChangeCheck();
                        Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_LayoutSpawner>("动效模板 - 回收", motnames, ref Rec_Lib_Name, HudFilled.实体, 120, 22, SelectedObjects);
                        if (EditorGUI.EndChangeCheck())
                        {
                            Motion_Recycler rec = HudManager.Hud_Motions.ElementMotion_GetElementCreator_At_Recycle(Rec_Lib_Name.stringValue);

                            RecycleArgs.FindPropertyRelative("Movement.Movement").enumValueIndex = (int)rec.Movement.Movement;
                            RecycleArgs.FindPropertyRelative("Movement.Distance").floatValue = rec.Movement.Distance;
                            RecycleArgs.FindPropertyRelative("Movement.Duration").floatValue = rec.Movement.Duration;
                            RecycleArgs.FindPropertyRelative("Movement.Delay").floatValue = rec.Movement.Delay;
                            RecycleArgs.FindPropertyRelative("Movement.Curve").animationCurveValue = rec.Movement.Curve;
                            RecycleArgs.FindPropertyRelative("Movement.CurveName").stringValue = rec.Movement.CurveName;
                            RecycleArgs.FindPropertyRelative("Movement.Ease").enumValueIndex = (int)rec.Movement.Ease;
                            RecycleArgs.FindPropertyRelative("Rotation.Rotation").enumValueIndex = (int)rec.Rotation.Rotation;
                            RecycleArgs.FindPropertyRelative("Rotation.Degree").floatValue = rec.Rotation.Degree;
                            RecycleArgs.FindPropertyRelative("Rotation.Duration").floatValue = rec.Rotation.Duration;
                            RecycleArgs.FindPropertyRelative("Rotation.Delay").floatValue = rec.Rotation.Delay;
                            RecycleArgs.FindPropertyRelative("Rotation.Curve").animationCurveValue = rec.Rotation.Curve;
                            RecycleArgs.FindPropertyRelative("Rotation.CurveName").stringValue = rec.Rotation.CurveName;
                            RecycleArgs.FindPropertyRelative("Rotation.Ease").enumValueIndex = (int)rec.Rotation.Ease;
                            RecycleArgs.FindPropertyRelative("Alpha.Duration").floatValue = rec.Alpha.Duration;
                            RecycleArgs.FindPropertyRelative("Alpha.Delay").floatValue = rec.Alpha.Delay;
                            RecycleArgs.FindPropertyRelative("Alpha.Curve").animationCurveValue = rec.Alpha.Curve;
                            RecycleArgs.FindPropertyRelative("Alpha.CurveName").stringValue = rec.Alpha.CurveName;
                            RecycleArgs.FindPropertyRelative("Alpha.Ease").enumValueIndex = (int)rec.Alpha.Ease;
                            RecycleArgs.FindPropertyRelative("MotionAnimateEndState").enumValueIndex = (int)rec.MotionAnimateEndState;
                            RecycleArgs.serializedObject.ApplyModifiedProperties();
                        }

                        #region 保存 & 定位模板
                        Editor_XHud_GUI.Gui_Layout_Space(10);

                        if (Editor_XHud_GUI.Gui_Layout_Button(14, "保存", icon_save_r, icon_save_p, 2))
                        {
                            OpenParameterSetter(HudElementMotionType.Recycler);
                            return;
                        }

                        Editor_XHud_GUI.Gui_Layout_Space(10);

                        if (Editor_XHud_GUI.Gui_Layout_Button(14, "定位", icon_locate_r, icon_locate_p, 2))
                        {
                            if (!HudManager.Hud_Motions.ElementMotion_IsExist(Rec_Lib_Name.stringValue, HudElementMotionType.Recycler))
                                return;
                            Editor_XHud_MenuItemsAction_OpenLibrary.open_elementmotion();
                            HudManager.Hud_Motions.ElementMotionLibrary_Location(Rec_Lib_Name.stringValue);
                            return;
                        }

                        Editor_XHud_GUI.Gui_Layout_Space(10);

                        if (Editor_XHud_GUI.Gui_Layout_Button(14, "重置", icon_reset_r, icon_reset_p, 2))
                        {
                            string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 布局元素生成器消息", "重置动效参数", "确定要将动效参数重置吗？您将丢失当前的动效参数！", "重置", "暂不", 0);
                            if (res == "重置")
                                ResetMotionParams("r");
                            return;
                        }

                        Editor_XHud_GUI.Gui_Layout_Space(5);
                        #endregion

                        Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("未在动效库中发现任何动效资源，请先为其添加动效资源!", MessageType.Warning);
                        Editor_XHud_GUI.Gui_Layout_Space(5);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("Hud管理器中未指定动效库，请先配置动效库!", MessageType.Warning);
                    Editor_XHud_GUI.Gui_Layout_Space(5);
                }
                #endregion

                #region 位移
                Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_XHud_GUI.Gui_Layout_Space(10);
                recycle_fold_move.boolValue = EditorGUILayout.Foldout(recycle_fold_move.boolValue, "位移", true);
                recycle_fold_move.serializedObject.ApplyModifiedProperties();
                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                if (recycle_fold_move.boolValue)
                {
                    SerializedProperty sp_move_type = RecycleArgs.FindPropertyRelative("Movement.Movement");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("方式", sp_move_type);

                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    SerializedProperty sp_move_dis = RecycleArgs.FindPropertyRelative("Movement.Distance");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("距离", sp_move_dis);

                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    SerializedProperty sp_move_dur = RecycleArgs.FindPropertyRelative("Movement.Duration");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("耗时", sp_move_dur);

                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    SerializedProperty sp_move_delay = RecycleArgs.FindPropertyRelative("Movement.Delay");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("延迟", sp_move_delay);

                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    SerializedProperty sp_move_ease = RecycleArgs.FindPropertyRelative("Movement.Ease");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("缓动", sp_move_ease);

                    if ((EaseMode)sp_move_ease.enumValueIndex == EaseMode.None)
                    {
                        Editor_XHud_GUI.Gui_Layout_Space(5);

                        SerializedProperty sp_move_curve = RecycleArgs.FindPropertyRelative("Movement.Curve");
                        Editor_XHud_GUI.Gui_Layout_Property_Field("曲线", sp_move_curve);

                        Editor_XHud_GUI.Gui_Layout_Space(5);

                        #region 曲线列表
                        SerializedProperty sp_CurveName = RecycleArgs.FindPropertyRelative("Movement.CurveName");
                        SerializedProperty sp_Curve = RecycleArgs.FindPropertyRelative("Movement.Curve");
                        if (HudManager.Hud_Curves != null)
                        {
                            if (HudManager.Hud_Curves.CurveLibrary != null && HudManager.Hud_Curves.CurveLibrary.Count > 0)
                            {
                                string[] names = HudManager.Hud_Curves.CurveLibrary_GetCurveNames();
                                Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_LayoutSpawner>("曲线样式", names, ref sp_CurveName, HudFilled.实体, 120, 22, SelectedObjects, (comps) => { }, (res) =>
                                {
                                    sp_Curve.animationCurveValue = HudManager.Hud_Curves.CurveLibrary_GetCurve(res);
                                    sp_Curve.serializedObject.ApplyModifiedProperties();
                                });
                            }
                            else
                            {
                                EditorGUILayout.HelpBox("未在动效库中发现任何曲线资源，请先为其添加曲线资源!", MessageType.Warning);
                                Editor_XHud_GUI.Gui_Layout_Space(5);
                            }
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("Hud管理器中未指定曲线库，请先配置曲线库!", MessageType.Warning);
                            Editor_XHud_GUI.Gui_Layout_Space(5);
                        }
                        #endregion
                    }
                }
                #endregion

                #region 旋转
                Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_XHud_GUI.Gui_Layout_Space(10);
                recycle_fold_rotate.boolValue = EditorGUILayout.Foldout(recycle_fold_rotate.boolValue, "旋转", true);
                recycle_fold_rotate.serializedObject.ApplyModifiedProperties();
                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                if (recycle_fold_rotate.boolValue)
                {
                    SerializedProperty sp_rot_type = RecycleArgs.FindPropertyRelative("Rotation.Rotation");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("方式", sp_rot_type);

                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    SerializedProperty sp_rot_deg = RecycleArgs.FindPropertyRelative("Rotation.Degree");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("角度", sp_rot_deg);

                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    SerializedProperty sp_rot_dur = RecycleArgs.FindPropertyRelative("Rotation.Duration");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("耗时", sp_rot_dur);

                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    SerializedProperty sp_rot_delay = RecycleArgs.FindPropertyRelative("Rotation.Delay");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("延迟", sp_rot_delay);

                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    SerializedProperty sp_rot_ease = RecycleArgs.FindPropertyRelative("Rotation.Ease");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("缓动", sp_rot_ease);

                    if ((EaseMode)sp_rot_ease.enumValueIndex == EaseMode.None)
                    {
                        Editor_XHud_GUI.Gui_Layout_Space(5);

                        SerializedProperty sp_rot_curve = RecycleArgs.FindPropertyRelative("Rotation.Curve");
                        Editor_XHud_GUI.Gui_Layout_Property_Field("曲线", sp_rot_curve);

                        Editor_XHud_GUI.Gui_Layout_Space(5);

                        #region 曲线列表
                        SerializedProperty sp_CurveName = RecycleArgs.FindPropertyRelative("Rotation.CurveName");
                        SerializedProperty sp_Curve = RecycleArgs.FindPropertyRelative("Rotation.Curve");
                        if (HudManager.Hud_Curves != null)
                        {
                            if (HudManager.Hud_Curves.CurveLibrary != null && HudManager.Hud_Curves.CurveLibrary.Count > 0)
                            {
                                string[] names = HudManager.Hud_Curves.CurveLibrary_GetCurveNames();
                                Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_LayoutSpawner>("曲线样式", names, ref sp_CurveName, HudFilled.实体, 120, 22, SelectedObjects, (comps) => { }, (res) =>
                                {
                                    sp_Curve.animationCurveValue = HudManager.Hud_Curves.CurveLibrary_GetCurve(res);
                                    sp_Curve.serializedObject.ApplyModifiedProperties();
                                });
                            }
                            else
                            {
                                EditorGUILayout.HelpBox("未在动效库中发现任何曲线资源，请先为其添加曲线资源!", MessageType.Warning);
                                Editor_XHud_GUI.Gui_Layout_Space(5);
                            }
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("Hud管理器中未指定曲线库，请先配置曲线库!", MessageType.Warning);
                            Editor_XHud_GUI.Gui_Layout_Space(5);
                        }
                        #endregion
                    }
                }
                #endregion

                #region 透明度
                Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_XHud_GUI.Gui_Layout_Space(10);
                recycle_fold_alpha.boolValue = EditorGUILayout.Foldout(recycle_fold_alpha.boolValue, "透明度", true);
                recycle_fold_alpha.serializedObject.ApplyModifiedProperties();
                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                if (recycle_fold_alpha.boolValue)
                {
                    SerializedProperty sp_alpha_type = RecycleArgs.FindPropertyRelative("Alpha.Duration");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("耗时", sp_alpha_type);

                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    SerializedProperty sp_alpha_delay = RecycleArgs.FindPropertyRelative("Alpha.Delay");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("延迟", sp_alpha_delay);

                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    SerializedProperty sp_alpha_ease = RecycleArgs.FindPropertyRelative("Alpha.Ease");
                    Editor_XHud_GUI.Gui_Layout_Property_Field("缓动", sp_alpha_ease);

                    Editor_XHud_GUI.Gui_Layout_Space(5);

                    if ((EaseMode)sp_alpha_ease.enumValueIndex == EaseMode.None)
                    {
                        Editor_XHud_GUI.Gui_Layout_Space(5);

                        SerializedProperty sp_alpha_curve = RecycleArgs.FindPropertyRelative("Alpha.Curve");
                        Editor_XHud_GUI.Gui_Layout_Property_Field("曲线", sp_alpha_curve);

                        Editor_XHud_GUI.Gui_Layout_Space(5);

                        #region 曲线列表
                        SerializedProperty sp_CurveName = RecycleArgs.FindPropertyRelative("Alpha.CurveName");
                        SerializedProperty sp_Curve = RecycleArgs.FindPropertyRelative("Alpha.Curve");
                        if (HudManager.Hud_Curves != null)
                        {
                            if (HudManager.Hud_Curves.CurveLibrary != null && HudManager.Hud_Curves.CurveLibrary.Count > 0)
                            {
                                string[] names = HudManager.Hud_Curves.CurveLibrary_GetCurveNames();
                                Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_LayoutSpawner>("曲线样式", names, ref sp_CurveName, HudFilled.实体, 120, 22, SelectedObjects, (comps) => { }, (res) =>
                                {
                                    sp_Curve.animationCurveValue = HudManager.Hud_Curves.CurveLibrary_GetCurve(res);
                                    sp_Curve.serializedObject.ApplyModifiedProperties();
                                });
                            }
                            else
                            {
                                EditorGUILayout.HelpBox("未在动效库中发现任何曲线资源，请先为其添加曲线资源!", MessageType.Warning);
                                Editor_XHud_GUI.Gui_Layout_Space(5);
                            }
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("Hud管理器中未指定曲线库，请先配置曲线库!", MessageType.Warning);
                            Editor_XHud_GUI.Gui_Layout_Space(5);
                        }
                        #endregion
                    }
                }
                #endregion

                EditorGUI.BeginChangeCheck();
                Editor_XHud_GUI.Gui_Layout_Property_Field("动效结束时机", RecycleArgs_MotionAnimateEndState, 85);
                if (EditorGUI.EndChangeCheck())
                {
                    MotionAnimateEndState state = (MotionAnimateEndState)RecycleArgs_MotionAnimateEndState.enumValueIndex;
                    switch (state)
                    {
                        case MotionAnimateEndState.以_移动为准:
                            HudMotion_Movement m = (HudMotion_Movement)RecycleArgs.FindPropertyRelative("Movement.Movement").enumValueIndex;
                            if (m == HudMotion_Movement.A_无运动)
                            {
                                Editor_XHud_GUI.Open(XHud_DialogType.警告, $"XHud - 布局元素生成器消息", "设定动画结束时机", $"当前位移方式为 <color={hexcol}> A_无运动 </color>，将发生<color={hexcol}> 动效动画无法正常结束</color>从而导致<color={hexcol}> 相应的事件和动作委托 </color>不能正确触发的情况 </color>！请将<color={hexcol}> 位移方式 </color>改为<color={hexcol}> 非无运动方式 </color>！", "明白", 0);
                                RecycleArgs_MotionAnimateEndState.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
                            }
                            break;
                        case MotionAnimateEndState.以_旋转为准:
                            HudMotion_Rotation r = (HudMotion_Rotation)RecycleArgs.FindPropertyRelative("Rotation.Rotation").enumValueIndex;
                            if (r == HudMotion_Rotation.A_无旋转)
                            {
                                Editor_XHud_GUI.Open(XHud_DialogType.警告, $"XHud - 布局元素生成器消息", "设定动画结束时机", $"当前旋转方式为 <color={hexcol}> A_无旋转 </color>，将发生<color={hexcol}> 动效动画无法正常结束</color>从而导致<color={hexcol}> 相应的事件和动作委托 </color>不能正确触发的情况 </color>！请将<color={hexcol}> 旋转方式 </color>改为<color={hexcol}> 非无旋转方式 </color>！", "明白", 0);
                                RecycleArgs_MotionAnimateEndState.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
                            }
                            break;
                    }
                }
            }
            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                SerializedProperty sp_CreateArgs = serializedObject.FindProperty("CreateArgs");
                SerializedProperty sp_RecycleArgs = serializedObject.FindProperty("RecycleArgs");

                // 创建右键菜单
                GenericMenu menu = new GenericMenu();
                menu.AddDisabledItem(new GUIContent("动效快速操作"));
                menu.AddItem(new GUIContent("E (复制动效)"), false, () =>
                {
                    string res = Editor_XHud_GUI.Open(XHud_DialogType.修改, "XHud - 布局元素生成器消息", "复制动效", "请选择动效参数复制模式！", "取消", "生成", "回收", 0);
                    if (res == "取消")
                        return;

                    MotionNode_Movement M = null;
                    MotionNode_Rotation R = null;
                    MotionNode_Alpha A = null;
                    string json = "";

                    switch (res)
                    {
                        case "生成"://生成
                            Motion_Creator crc = new Motion_Creator();
                            crc.anchor = (XHudAnchor)sp_CreateArgs.FindPropertyRelative("anchor").enumValueIndex;

                            M = new MotionNode_Movement();
                            M.Movement = (HudMotion_Movement)sp_CreateArgs.FindPropertyRelative("Movement.Movement").enumValueIndex;
                            M.Distance = sp_CreateArgs.FindPropertyRelative("Movement.Distance").floatValue;
                            M.Duration = sp_CreateArgs.FindPropertyRelative("Movement.Duration").floatValue;
                            M.Delay = sp_CreateArgs.FindPropertyRelative("Movement.Delay").floatValue;
                            M.Ease = (EaseMode)sp_CreateArgs.FindPropertyRelative("Movement.Ease").enumValueIndex;
                            M.Curve = sp_CreateArgs.FindPropertyRelative("Movement.Curve").animationCurveValue;
                            M.CurveName = sp_CreateArgs.FindPropertyRelative("Movement.CurveName").stringValue;

                            R = new MotionNode_Rotation();
                            R.Rotation = (HudMotion_Rotation)sp_CreateArgs.FindPropertyRelative("Rotation.Rotation").enumValueIndex;
                            R.Degree = sp_CreateArgs.FindPropertyRelative("Rotation.Degree").floatValue;
                            R.Duration = sp_CreateArgs.FindPropertyRelative("Rotation.Duration").floatValue;
                            R.Delay = sp_CreateArgs.FindPropertyRelative("Rotation.Delay").floatValue;
                            R.Ease = (EaseMode)sp_CreateArgs.FindPropertyRelative("Rotation.Ease").enumValueIndex;
                            R.Curve = sp_CreateArgs.FindPropertyRelative("Rotation.Curve").animationCurveValue;
                            R.CurveName = sp_CreateArgs.FindPropertyRelative("Rotation.CurveName").stringValue;

                            A = new MotionNode_Alpha();
                            A.Duration = sp_CreateArgs.FindPropertyRelative("Alpha.Duration").floatValue;
                            A.Curve = sp_CreateArgs.FindPropertyRelative("Alpha.Curve").animationCurveValue;
                            A.CurveName = sp_CreateArgs.FindPropertyRelative("Alpha.CurveName").stringValue;
                            A.Ease = (EaseMode)sp_CreateArgs.FindPropertyRelative("Alpha.Ease").enumValueIndex;
                            A.Delay = sp_CreateArgs.FindPropertyRelative("Alpha.Delay").floatValue;

                            crc.Movement = M;
                            crc.Rotation = R;
                            crc.Alpha = A;

                            json = JsonUtility.ToJson(crc);
                            Editor_XHud_GUI.EditorData_Set_With_String("XED_Copy_MotionArgs", json);
                            break;
                        case "回收"://回收
                            Motion_Recycler rec = new Motion_Recycler();

                            M = new MotionNode_Movement();
                            M.Movement = (HudMotion_Movement)sp_RecycleArgs.FindPropertyRelative("Movement.Movement").enumValueIndex;
                            M.Distance = sp_RecycleArgs.FindPropertyRelative("Movement.Distance").floatValue;
                            M.Duration = sp_RecycleArgs.FindPropertyRelative("Movement.Duration").floatValue;
                            M.Delay = sp_RecycleArgs.FindPropertyRelative("Movement.Delay").floatValue;
                            M.Ease = (EaseMode)sp_RecycleArgs.FindPropertyRelative("Movement.Ease").enumValueIndex;
                            M.Curve = sp_RecycleArgs.FindPropertyRelative("Movement.Curve").animationCurveValue;
                            M.CurveName = sp_RecycleArgs.FindPropertyRelative("Movement.CurveName").stringValue;

                            R = new MotionNode_Rotation();
                            R.Rotation = (HudMotion_Rotation)sp_RecycleArgs.FindPropertyRelative("Rotation.Rotation").enumValueIndex;
                            R.Degree = sp_RecycleArgs.FindPropertyRelative("Rotation.Degree").floatValue;
                            R.Duration = sp_RecycleArgs.FindPropertyRelative("Rotation.Duration").floatValue;
                            R.Delay = sp_RecycleArgs.FindPropertyRelative("Rotation.Delay").floatValue;
                            R.Ease = (EaseMode)sp_RecycleArgs.FindPropertyRelative("Rotation.Ease").enumValueIndex;
                            R.Curve = sp_RecycleArgs.FindPropertyRelative("Rotation.Curve").animationCurveValue;
                            R.CurveName = sp_RecycleArgs.FindPropertyRelative("Rotation.CurveName").stringValue;

                            A = new MotionNode_Alpha();
                            A.Duration = sp_RecycleArgs.FindPropertyRelative("Alpha.Duration").floatValue;
                            A.Curve = sp_RecycleArgs.FindPropertyRelative("Alpha.Curve").animationCurveValue;
                            A.CurveName = sp_RecycleArgs.FindPropertyRelative("Alpha.CurveName").stringValue;
                            A.Ease = (EaseMode)sp_RecycleArgs.FindPropertyRelative("Alpha.Ease").enumValueIndex;
                            A.Delay = sp_RecycleArgs.FindPropertyRelative("Alpha.Delay").floatValue;

                            rec.Movement = M;
                            rec.Rotation = R;
                            rec.Alpha = A;

                            json = JsonUtility.ToJson(rec);
                            Editor_XHud_GUI.EditorData_Set_With_String("XED_Copy_MotionArgs", json);
                            break;
                    }

                    string mode = "";

                    if (res == "生成")
                        mode = "生成动效参数";
                    else if (res == "回收")
                        mode = "回收动效参数";

                    Editor_XHud_GUI.Open(XHud_DialogType.确认, "XHud - 布局元素生成器消息", "复制动效", $"已复制 \" {mode} \" 到 XHudEditorData (XED) ！", "明白", 0);
                });
                menu.AddItem(new GUIContent("R (粘贴动效)"), false, () =>
                {
                    string buffer = Editor_XHud_GUI.EditorData_Get_With_String("XED_Copy_MotionArgs");
                    if (buffer.Contains("anchor"))//粘贴生成参数
                    {
                        string res = Editor_XHud_GUI.Open(XHud_DialogType.修改, "XHud - 布局元素生成器消息", "粘贴动效", "检测到动效参数类型为： \"生成动效\"，确定要使用这个参数吗？", "确定", "暂不", 0);
                        if (res == "暂不")
                            return;

                        Motion_Creator crc = JsonUtility.FromJson<Motion_Creator>(buffer);

                        sp_CreateArgs.FindPropertyRelative("anchor").enumValueIndex = (int)crc.anchor;

                        sp_CreateArgs.FindPropertyRelative("Movement.Movement").enumValueIndex = (int)crc.Movement.Movement;
                        sp_CreateArgs.FindPropertyRelative("Movement.Distance").floatValue = crc.Movement.Distance;
                        sp_CreateArgs.FindPropertyRelative("Movement.Duration").floatValue = crc.Movement.Duration;
                        sp_CreateArgs.FindPropertyRelative("Movement.Delay").floatValue = crc.Movement.Delay;
                        sp_CreateArgs.FindPropertyRelative("Movement.Ease").enumValueIndex = (int)crc.Movement.Ease;
                        sp_CreateArgs.FindPropertyRelative("Movement.Curve").animationCurveValue = crc.Movement.Curve;
                        sp_CreateArgs.FindPropertyRelative("Movement.CurveName").stringValue = crc.Movement.CurveName;

                        sp_CreateArgs.FindPropertyRelative("Rotation.Rotation").enumValueIndex = (int)crc.Rotation.Rotation;
                        sp_CreateArgs.FindPropertyRelative("Rotation.Degree").floatValue = crc.Rotation.Degree;
                        sp_CreateArgs.FindPropertyRelative("Rotation.Duration").floatValue = crc.Rotation.Duration;
                        sp_CreateArgs.FindPropertyRelative("Rotation.Delay").floatValue = crc.Rotation.Delay;
                        sp_CreateArgs.FindPropertyRelative("Rotation.Ease").enumValueIndex = (int)crc.Rotation.Ease;
                        sp_CreateArgs.FindPropertyRelative("Rotation.Curve").animationCurveValue = crc.Rotation.Curve;
                        sp_CreateArgs.FindPropertyRelative("Rotation.CurveName").stringValue = crc.Rotation.CurveName;

                        sp_CreateArgs.FindPropertyRelative("Alpha.Duration").floatValue = crc.Alpha.Duration;
                        sp_CreateArgs.FindPropertyRelative("Alpha.Curve").animationCurveValue = crc.Alpha.Curve;
                        sp_CreateArgs.FindPropertyRelative("Alpha.CurveName").stringValue = crc.Alpha.CurveName;
                        sp_CreateArgs.FindPropertyRelative("Alpha.Ease").enumValueIndex = (int)crc.Alpha.Ease;
                        sp_CreateArgs.FindPropertyRelative("Alpha.Delay").floatValue = crc.Alpha.Delay;

                        sp_CreateArgs.serializedObject.ApplyModifiedProperties();

                        Editor_XHud_GUI.Open(XHud_DialogType.确认, "XHud - 布局元素生成器消息", "粘贴动效", "已更新 \"生成\" 动效参数!", "明白", 0);
                    }
                    else//粘贴回收参数
                    {
                        string res = Editor_XHud_GUI.Open(XHud_DialogType.修改, "XHud - 布局元素生成器消息", "粘贴动效", "检测到动效参数类型为： \"回收动效\"，确定要使用这个参数吗？", "确定", "暂不", 0);
                        if (res == "暂不")
                            return;

                        Motion_Recycler rec = JsonUtility.FromJson<Motion_Recycler>(buffer);

                        sp_RecycleArgs.FindPropertyRelative("Movement.Movement").enumValueIndex = (int)rec.Movement.Movement;
                        sp_RecycleArgs.FindPropertyRelative("Movement.Distance").floatValue = rec.Movement.Distance;
                        sp_RecycleArgs.FindPropertyRelative("Movement.Duration").floatValue = rec.Movement.Duration;
                        sp_RecycleArgs.FindPropertyRelative("Movement.Delay").floatValue = rec.Movement.Delay;
                        sp_RecycleArgs.FindPropertyRelative("Movement.Ease").enumValueIndex = (int)rec.Movement.Ease;
                        sp_RecycleArgs.FindPropertyRelative("Movement.Curve").animationCurveValue = rec.Movement.Curve;
                        sp_RecycleArgs.FindPropertyRelative("Movement.CurveName").stringValue = rec.Movement.CurveName;

                        sp_RecycleArgs.FindPropertyRelative("Rotation.Rotation").enumValueIndex = (int)rec.Rotation.Rotation;
                        sp_RecycleArgs.FindPropertyRelative("Rotation.Degree").floatValue = rec.Rotation.Degree;
                        sp_RecycleArgs.FindPropertyRelative("Rotation.Duration").floatValue = rec.Rotation.Duration;
                        sp_RecycleArgs.FindPropertyRelative("Rotation.Delay").floatValue = rec.Rotation.Delay;
                        sp_RecycleArgs.FindPropertyRelative("Rotation.Ease").enumValueIndex = (int)rec.Rotation.Ease;
                        sp_RecycleArgs.FindPropertyRelative("Rotation.Curve").animationCurveValue = rec.Rotation.Curve;
                        sp_RecycleArgs.FindPropertyRelative("Rotation.CurveName").stringValue = rec.Rotation.CurveName;

                        sp_RecycleArgs.FindPropertyRelative("Alpha.Duration").floatValue = rec.Alpha.Duration;
                        sp_RecycleArgs.FindPropertyRelative("Alpha.Curve").animationCurveValue = rec.Alpha.Curve;
                        sp_RecycleArgs.FindPropertyRelative("Alpha.CurveName").stringValue = rec.Alpha.CurveName;
                        sp_RecycleArgs.FindPropertyRelative("Alpha.Ease").enumValueIndex = (int)rec.Alpha.Ease;
                        sp_RecycleArgs.FindPropertyRelative("Alpha.Delay").floatValue = rec.Alpha.Delay;

                        sp_RecycleArgs.serializedObject.ApplyModifiedProperties();

                        Editor_XHud_GUI.Open(XHud_DialogType.确认, "XHud - 布局元素生成器消息", "粘贴动效", "已更新 \"回收\" 动效参数!", "明白", 0);
                    }
                });
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("F (折叠动效参数)"), false, () =>
                {
                    recycle_fold_move.boolValue = false;
                    recycle_fold_rotate.boolValue = false;
                    recycle_fold_alpha.boolValue = false;
                    create_fold_move.boolValue = false;
                    create_fold_rotate.boolValue = false;
                    create_fold_alpha.boolValue = false;

                    recycle_fold_move.serializedObject.ApplyModifiedProperties();
                    recycle_fold_rotate.serializedObject.ApplyModifiedProperties();
                    recycle_fold_alpha.serializedObject.ApplyModifiedProperties();
                    create_fold_move.serializedObject.ApplyModifiedProperties();
                    create_fold_rotate.serializedObject.ApplyModifiedProperties();
                    create_fold_alpha.serializedObject.ApplyModifiedProperties();
                });
                menu.AddItem(new GUIContent("D (展开动效参数)"), false, () =>
                {
                    recycle_fold_move.boolValue = true;
                    recycle_fold_rotate.boolValue = true;
                    recycle_fold_alpha.boolValue = true;
                    create_fold_move.boolValue = true;
                    create_fold_rotate.boolValue = true;
                    create_fold_alpha.boolValue = true;

                    recycle_fold_move.serializedObject.ApplyModifiedProperties();
                    recycle_fold_rotate.serializedObject.ApplyModifiedProperties();
                    recycle_fold_alpha.serializedObject.ApplyModifiedProperties();
                    create_fold_move.serializedObject.ApplyModifiedProperties();
                    create_fold_rotate.serializedObject.ApplyModifiedProperties();
                    create_fold_alpha.serializedObject.ApplyModifiedProperties();
                });
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("V (折叠元素列表)"), false, () =>
                {
                    FoldScreen.boolValue = false;
                    FoldWorld.boolValue = false;

                    FoldScreen.serializedObject.ApplyModifiedProperties();
                    FoldWorld.serializedObject.ApplyModifiedProperties();
                });
                menu.AddItem(new GUIContent("B (展开元素列表)"), false, () =>
                {
                    FoldScreen.boolValue = true;
                    FoldWorld.boolValue = true;

                    FoldScreen.serializedObject.ApplyModifiedProperties();
                    FoldWorld.serializedObject.ApplyModifiedProperties();
                });
                menu.ShowAsContext(); // 在鼠标位置显示右键菜单

                Event.current.Use();
            }

            #region 源脚本
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 3, "源脚本", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(5);

            #region 原始变量
            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_XHud_GUI.Gui_Layout_Space(10);
            OriginalDisplay = EditorGUILayout.Foldout(OriginalDisplay, "变量/属性", true);
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
        /// 停止预览
        /// </summary>
        private void StopPreviewing()
        {
            if (ElementList_Screen != null && ElementList_Screen.count > 0)
                XTween_Preview_Kill(XHudSpace.屏幕空间);

            if (ElementList_World != null && ElementList_World.count > 0)
                XTween_Preview_Kill(XHudSpace.世界空间);

            IsPreviewing.boolValue = false;
            IsPreviewing.serializedObject.ApplyModifiedProperties();
        }

        #region 辅助

        /// <summary>
        /// 重置生成与回收的参数到默认
        /// </summary>
        private void ResetMotionParams(string state)
        {
            if (state == "c")
            {
                CreateArgs.FindPropertyRelative("anchor").enumValueIndex = (int)XHudAnchor.中心;
                CreateArgs.FindPropertyRelative("Movement.Movement").enumValueIndex = (int)HudMotion_Movement.S_从下至上;
                CreateArgs.FindPropertyRelative("Movement.Distance").floatValue = 100;
                CreateArgs.FindPropertyRelative("Movement.Duration").floatValue = 1;
                CreateArgs.FindPropertyRelative("Movement.Delay").floatValue = 0;
                CreateArgs.FindPropertyRelative("Movement.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
                CreateArgs.FindPropertyRelative("Movement.CurveName").stringValue = "";
                CreateArgs.FindPropertyRelative("Movement.Ease").enumValueIndex = (int)EaseMode.InOutCubic;
                CreateArgs.FindPropertyRelative("Rotation.Rotation").enumValueIndex = (int)HudMotion_Rotation.A_无旋转;
                CreateArgs.FindPropertyRelative("Rotation.Degree").floatValue = 0;
                CreateArgs.FindPropertyRelative("Rotation.Duration").floatValue = 1;
                CreateArgs.FindPropertyRelative("Rotation.Delay").floatValue = 0;
                CreateArgs.FindPropertyRelative("Rotation.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
                CreateArgs.FindPropertyRelative("Rotation.CurveName").stringValue = "";
                CreateArgs.FindPropertyRelative("Rotation.Ease").enumValueIndex = (int)EaseMode.InOutCubic;
                CreateArgs.FindPropertyRelative("Alpha.Duration").floatValue = 1;
                CreateArgs.FindPropertyRelative("Alpha.Delay").floatValue = 0;
                CreateArgs.FindPropertyRelative("Alpha.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
                CreateArgs.FindPropertyRelative("Alpha.CurveName").stringValue = "";
                CreateArgs.FindPropertyRelative("Alpha.Ease").enumValueIndex = (int)EaseMode.InOutCubic;
                CreateArgs_MotionAnimateEndState.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
                CreateArgs.serializedObject.ApplyModifiedProperties();

                Crc_Lib_Name.stringValue = null;
                Crc_Lib_Name.serializedObject.ApplyModifiedProperties();
            }
            else if (state == "r")
            {
                RecycleArgs.FindPropertyRelative("Movement.Movement").enumValueIndex = (int)HudMotion_Movement.D_从上至下;
                RecycleArgs.FindPropertyRelative("Movement.Distance").floatValue = 100;
                RecycleArgs.FindPropertyRelative("Movement.Duration").floatValue = 1;
                RecycleArgs.FindPropertyRelative("Movement.Delay").floatValue = 0;
                RecycleArgs.FindPropertyRelative("Movement.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
                RecycleArgs.FindPropertyRelative("Movement.CurveName").stringValue = "";
                RecycleArgs.FindPropertyRelative("Movement.Ease").enumValueIndex = (int)EaseMode.InOutCubic;
                RecycleArgs.FindPropertyRelative("Rotation.Rotation").enumValueIndex = (int)HudMotion_Rotation.A_无旋转;
                RecycleArgs.FindPropertyRelative("Rotation.Degree").floatValue = 0;
                RecycleArgs.FindPropertyRelative("Rotation.Duration").floatValue = 1;
                RecycleArgs.FindPropertyRelative("Rotation.Delay").floatValue = 0;
                RecycleArgs.FindPropertyRelative("Rotation.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
                RecycleArgs.FindPropertyRelative("Rotation.CurveName").stringValue = "";
                RecycleArgs.FindPropertyRelative("Rotation.Ease").enumValueIndex = (int)EaseMode.InOutCubic;
                RecycleArgs.FindPropertyRelative("Alpha.Duration").floatValue = 1;
                RecycleArgs.FindPropertyRelative("Alpha.Delay").floatValue = 0;
                RecycleArgs.FindPropertyRelative("Alpha.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
                RecycleArgs.FindPropertyRelative("Alpha.CurveName").stringValue = "";
                RecycleArgs.FindPropertyRelative("Alpha.Ease").enumValueIndex = (int)EaseMode.InOutCubic;
                RecycleArgs_MotionAnimateEndState.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
                RecycleArgs.serializedObject.ApplyModifiedProperties();

                Rec_Lib_Name.stringValue = null;
                Rec_Lib_Name.serializedObject.ApplyModifiedProperties();
            }
        }

        /// <summary>
        /// 主要在编辑器模式下用于获取所有锚点下的物体进行集控 - 屏幕级别
        /// </summary>
        /// <returns></returns>
        public HudElementInfo AnchorElementInfosGet_Screen()
        {
            XHud_Module_Element[] trans = HudManager.HudCanvas_ScreenAnchor.GetComponentsInChildren<XHud_Module_Element>();
            HudElementInfo ElementInfos = new HudElementInfo();
            for (int i = 0; i < trans.Length; i++)
            {
                if (PrefabUtility.IsPartOfPrefabInstance(trans[i]))
                {
                    ElementInfos.elements.Add(trans[i]);
                }
                else
                {
                    ElementInfos.notPrefabsList.Add(trans[i].gameObject);
                }
            }
            return ElementInfos;
        }

        /// <summary>
        /// 主要在编辑器模式下用于获取所有锚点下的物体进行集控 - 世界级别
        /// </summary>
        /// <returns></returns>
        public HudElementInfo AnchorElementInfosGet_World()
        {
            XHud_Module_Element[] trans = HudManager.HudCanvas_World.GetComponentsInChildren<XHud_Module_Element>();
            HudElementInfo ElementInfos = new HudElementInfo();
            for (int i = 0; i < trans.Length; i++)
            {
                if (PrefabUtility.IsPartOfPrefabInstance(trans[i]))
                {
                    ElementInfos.elements.Add(trans[i]);
                }
                else
                {
                    ElementInfos.notPrefabsList.Add(trans[i].gameObject);
                }
            }
            return ElementInfos;
        }

        /// <summary>
        /// 检测元素是否在元素池中存在
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool IsInLibrary(string name)
        {
            bool sw = false;
            for (int i = 0; i < HudManager.Hud_ElementLibrarys.Count; i++)
            {
                if (HudManager.Hud_ElementLibrarys[i].LibraryName == LibName.stringValue)
                {
                    sw = HudManager.Hud_ElementLibrarys[i].ElementLibrary_IsExist(name);
                    break;
                }
            }
            return sw;
        }

        /// <summary>
        /// 获取序列化变量
        /// </summary>
        private void GetSerializeFields()
        {
            LibName = serializedObject.FindProperty("LibName");
            SpawnItemList_Screen = serializedObject.FindProperty("SpawnItemList_Screen");
            SpawnItemList_World = serializedObject.FindProperty("SpawnItemList_World");
            SpawnerIndicator = serializedObject.FindProperty("SpawnerIndicator");
            CreateArgs = serializedObject.FindProperty("CreateArgs");
            RecycleArgs = serializedObject.FindProperty("RecycleArgs");
            Key_Create = serializedObject.FindProperty("Key_Create");
            Key_Recycle = serializedObject.FindProperty("Key_Recycle");
            UseManullyKey = serializedObject.FindProperty("UseManullyKey");
            create_fold_move = serializedObject.FindProperty("create_fold_move");
            create_fold_rotate = serializedObject.FindProperty("create_fold_rotate");
            create_fold_alpha = serializedObject.FindProperty("create_fold_alpha");
            recycle_fold_move = serializedObject.FindProperty("recycle_fold_move");
            recycle_fold_rotate = serializedObject.FindProperty("recycle_fold_rotate");
            recycle_fold_alpha = serializedObject.FindProperty("recycle_fold_alpha");
            Crc_Lib_Name = serializedObject.FindProperty("Crc_Lib_Name");
            Rec_Lib_Name = serializedObject.FindProperty("Rec_Lib_Name");
            IsLoadedLayout_Screen = serializedObject.FindProperty("IsLoadedLayout_Screen");
            IsLoadedLayout_World = serializedObject.FindProperty("IsLoadedLayout_World");
            SpawnFunctionKey_Primary = serializedObject.FindProperty("SpawnFunctionKey_Primary");
            SpawnFunctionKey_Secondary = serializedObject.FindProperty("SpawnFunctionKey_Secondary");
            KeyControl_Screen = serializedObject.FindProperty("KeyControl_Screen");
            KeyControl_World = serializedObject.FindProperty("KeyControl_World");
            FoldScreen = serializedObject.FindProperty("FoldScreen");
            FoldWorld = serializedObject.FindProperty("FoldWorld");
            spawnerisRunning = serializedObject.FindProperty("spawnerisRunning");
            CreateArgs_MotionAnimateEndState = CreateArgs.FindPropertyRelative("MotionAnimateEndState");
            RecycleArgs_MotionAnimateEndState = RecycleArgs.FindPropertyRelative("MotionAnimateEndState");
            IsPreviewing = serializedObject.FindProperty("IsPreviewing");
            UseInstantiateMode = serializedObject.FindProperty("UseInstantiateMode");
            UseDebug = serializedObject.FindProperty("UseDebug");
        }

        /// <summary>
        /// 动效库添加器
        /// </summary>
        public void OpenParameterSetter(HudElementMotionType Type)
        {
            Editor_XHud_LibrarySetTool_Motion window = EditorWindow.GetWindow<Editor_XHud_LibrarySetTool_Motion>(true);

            window.titleContent = new GUIContent("XHud - 元素动效资源采集器");
            Editor_XHud_GUI.CenterEditorWindow(new Vector2Int(348, Type == HudElementMotionType.Creator ? 850 : 780), window);

            switch (Type)
            {
                case HudElementMotionType.Recycler:
                    Motion_Recycler rec = new Motion_Recycler();
                    rec.MotionAnimateEndState = BaseScript.RecycleArgs.MotionAnimateEndState;
                    rec.Alpha = BaseScript.RecycleArgs.Alpha;

                    rec.Movement = new MotionNode_Movement();
                    rec.Movement.CopyData(BaseScript.RecycleArgs.Movement);

                    rec.Rotation = new MotionNode_Rotation();
                    rec.Rotation.CopyData(BaseScript.RecycleArgs.Rotation);

                    rec.Alpha = new MotionNode_Alpha();
                    rec.Alpha.CopyData(BaseScript.RecycleArgs.Alpha);

                    window.SetElementMotion(rec);
                    break;
                case HudElementMotionType.Creator:
                    Motion_Creator crc = new Motion_Creator();
                    crc.MotionAnimateEndState = BaseScript.CreateArgs.MotionAnimateEndState;
                    crc.anchor = BaseScript.CreateArgs.anchor;

                    crc.Alpha = BaseScript.CreateArgs.Alpha;

                    crc.Movement = new MotionNode_Movement();
                    crc.Movement.CopyData(BaseScript.CreateArgs.Movement);

                    crc.Rotation = new MotionNode_Rotation();
                    crc.Rotation.CopyData(BaseScript.CreateArgs.Rotation);

                    crc.Alpha = new MotionNode_Alpha();
                    crc.Alpha.CopyData(BaseScript.CreateArgs.Alpha);

                    window.SetElementMotion(crc);
                    break;
            }

            window.SetElementMotionType(Type);
            window.SetLibrarySetterMode(LibrarySetterMode.添加到库);
            window.SetButtonText("添加", "取消");
            window.SetTitle("元素动效资源采集器");
            window.SetTarget_Hud_MotionLibrary(HudManager.Hud_Motions);
            //window.ShowModal();
            window.Show();
        }

        /// <summary>
        /// 动效参数修改器
        /// </summary>
        public void OpenParameterModifier(XHud_LibraryArg_Motion motion, string motionName, HudElementMotionType type)
        {
            Editor_XHud_LibrarySetTool_Motion window = EditorWindow.GetWindow<Editor_XHud_LibrarySetTool_Motion>(true);

            window.titleContent = new GUIContent("XHud - 元素动效资源修改器");
            Editor_XHud_GUI.CenterEditorWindow(new Vector2Int(348, 690), window);
            window.SetElementMotionType(type);
            window.SetElementMotion(motion.Crc, motion.Rec);
            window.SetLibrarySetterMode(LibrarySetterMode.修改生成器项参数);
            window.SetButtonText("完成");

            string hexcol = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary, true);
            window.SetTitle($"<color={hexcol}>{motionName} </color>");

            //window.ShowModal();
            window.Show();
        }

        public string[] CollectIDs(XHudSpace space)
        {
            List<string> list = new List<string>();

            for (int i = 0; i < (space == XHudSpace.屏幕空间 ? BaseScript.SpawnItemList_Screen.Count : BaseScript.SpawnItemList_World.Count); i++)
            {
                list.Add(space == XHudSpace.屏幕空间 ? BaseScript.SpawnItemList_Screen[i].ID : BaseScript.SpawnItemList_World[i].ID);
            }

            return list.ToArray();
        }

        /// <summary>
        /// 清空生成项列表 - 屏幕 Screen
        /// </summary>
        private void ClearSpawnItemList_Screen()
        {
            SpawnItemList_Screen.ClearArray();
            SpawnItemList_Screen.serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 添加一个生成项 - 屏幕 Screen
        /// </summary>
        /// <param name="item"></param>
        private void AddedSpawnItem_Screen(XHud_LayoutSpawner_Item item)
        {
            SerializedProperty sp_Item;
            if (SpawnItemList_Screen.arraySize <= 0)
            {
                SpawnItemList_Screen.InsertArrayElementAtIndex(0);
                sp_Item = SpawnItemList_Screen.GetArrayElementAtIndex(0);
            }
            else
            {
                SpawnItemList_Screen.InsertArrayElementAtIndex(SpawnItemList_Screen.arraySize);
                sp_Item = SpawnItemList_Screen.GetArrayElementAtIndex(SpawnItemList_Screen.arraySize - 1);
            }

            SerializedProperty sp_SpawnName = sp_Item.FindPropertyRelative("SpawnName");
            SerializedProperty sp_Indicator = sp_Item.FindPropertyRelative("Indicator");
            SerializedProperty sp_ID = sp_Item.FindPropertyRelative("ID");
            SerializedProperty sp_Delay_Spawn = sp_Item.FindPropertyRelative("Delay_Spawn");
            SerializedProperty sp_Position = sp_Item.FindPropertyRelative("Position");
            SerializedProperty sp_Euler = sp_Item.FindPropertyRelative("Euler");
            SerializedProperty sp_Pivot = sp_Item.FindPropertyRelative("Pivot");
            SerializedProperty sp_Anchor_Min = sp_Item.FindPropertyRelative("Anchor_Min");
            SerializedProperty sp_Anchor_Max = sp_Item.FindPropertyRelative("Anchor_Max");
            SerializedProperty sp_Offset = sp_Item.FindPropertyRelative("Offset");
            SerializedProperty sp_Scale = sp_Item.FindPropertyRelative("Scale");
            SerializedProperty sp_Size = sp_Item.FindPropertyRelative("Size");
            SerializedProperty sp_AutoIn = sp_Item.FindPropertyRelative("AutoIn");
            SerializedProperty sp_UseSpawnerMotion = sp_Item.FindPropertyRelative("UseSpawnerMotion");
            SerializedProperty sp_isFold = sp_Item.FindPropertyRelative("isFold");
            SerializedProperty sp_isEnabled = sp_Item.FindPropertyRelative("isEnabled");

            #region SerializedProperty - CreateArgs
            SerializedProperty sp_CreateArgs = sp_Item.FindPropertyRelative("CreateArgs");
            SerializedProperty sp_creator_anchor = sp_CreateArgs.FindPropertyRelative("anchor");
            SerializedProperty sp_creator_Movement = sp_CreateArgs.FindPropertyRelative("Movement.Movement");
            SerializedProperty sp_creator_Movement_Distance = sp_CreateArgs.FindPropertyRelative("Movement.Distance");
            SerializedProperty sp_creator_Movement_Duration = sp_CreateArgs.FindPropertyRelative("Movement.Duration");
            SerializedProperty sp_creator_Movement_Delay = sp_CreateArgs.FindPropertyRelative("Movement.Delay");
            SerializedProperty sp_creator_Movement_Ease = sp_CreateArgs.FindPropertyRelative("Movement.Ease");
            SerializedProperty sp_creator_Rotation = sp_CreateArgs.FindPropertyRelative("Rotation.Rotation");
            SerializedProperty sp_creator_Rotation_Degree = sp_CreateArgs.FindPropertyRelative("Rotation.Degree");
            SerializedProperty sp_creator_Rotation_Duration = sp_CreateArgs.FindPropertyRelative("Rotation.Duration");
            SerializedProperty sp_creator_Rotation_Delay = sp_CreateArgs.FindPropertyRelative("Rotation.Delay");
            SerializedProperty sp_creator_Rotation_Ease = sp_CreateArgs.FindPropertyRelative("Rotation.Ease");
            SerializedProperty sp_creator_Alpha_Duration = sp_CreateArgs.FindPropertyRelative("Alpha.Duration");
            SerializedProperty sp_creator_Alpha_Delay = sp_CreateArgs.FindPropertyRelative("Alpha.Delay");
            SerializedProperty sp_creator_Alpha_Ease = sp_CreateArgs.FindPropertyRelative("Alpha.Ease");
            #endregion

            #region SerializedProperty - RecycleArgs
            SerializedProperty sp_RecycleArgs = sp_Item.FindPropertyRelative("RecycleArgs");
            SerializedProperty sp_recycle_Movement = sp_RecycleArgs.FindPropertyRelative("Movement.Movement");
            SerializedProperty sp_recycle_Movement_Distance = sp_RecycleArgs.FindPropertyRelative("Movement.Distance");
            SerializedProperty sp_recycle_Movement_Duration = sp_RecycleArgs.FindPropertyRelative("Movement.Duration");
            SerializedProperty sp_recycle_Movement_Delay = sp_RecycleArgs.FindPropertyRelative("Movement.Delay");
            SerializedProperty sp_recycle_Movement_Ease = sp_RecycleArgs.FindPropertyRelative("Movement.Ease");
            SerializedProperty sp_recycle_Rotation = sp_RecycleArgs.FindPropertyRelative("Rotation.Rotation");
            SerializedProperty sp_recycle_Rotation_Degree = sp_RecycleArgs.FindPropertyRelative("Rotation.Degree");
            SerializedProperty sp_recycle_Rotation_Duration = sp_RecycleArgs.FindPropertyRelative("Rotation.Duration");
            SerializedProperty sp_recycle_Rotation_Delay = sp_RecycleArgs.FindPropertyRelative("Rotation.Delay");
            SerializedProperty sp_recycle_Rotation_Ease = sp_RecycleArgs.FindPropertyRelative("Rotation.Ease");
            SerializedProperty sp_recycle_Alpha_Duration = sp_RecycleArgs.FindPropertyRelative("Alpha.Duration");
            SerializedProperty sp_recycle_Alpha_Delay = sp_RecycleArgs.FindPropertyRelative("Alpha.Delay");
            SerializedProperty sp_recycle_Alpha_Ease = sp_RecycleArgs.FindPropertyRelative("Alpha.Ease");
            #endregion

            #region CreateArgs
            sp_creator_anchor.enumValueIndex = (int)item.CreateArgs.anchor;

            #region Movement
            sp_creator_Movement.enumValueIndex = (int)item.CreateArgs.Movement.Movement;
            sp_creator_Movement_Distance.floatValue = item.CreateArgs.Movement.Distance;
            sp_creator_Movement_Duration.floatValue = item.CreateArgs.Movement.Duration;
            sp_creator_Movement_Delay.floatValue = item.CreateArgs.Movement.Delay;
            sp_creator_Movement_Ease.enumValueIndex = (int)item.CreateArgs.Movement.Ease;
            #endregion

            #region Rotation
            sp_creator_Rotation.enumValueIndex = (int)item.CreateArgs.Rotation.Rotation;
            sp_creator_Rotation_Degree.floatValue = item.CreateArgs.Rotation.Degree;
            sp_creator_Rotation_Duration.floatValue = item.CreateArgs.Rotation.Duration;
            sp_creator_Rotation_Delay.floatValue = item.CreateArgs.Rotation.Delay;
            sp_creator_Rotation_Ease.enumValueIndex = (int)item.CreateArgs.Rotation.Ease;
            #endregion

            #region Alpha
            sp_creator_Alpha_Duration.floatValue = item.CreateArgs.Alpha.Duration;
            sp_creator_Alpha_Delay.floatValue = item.CreateArgs.Alpha.Delay;
            sp_creator_Alpha_Ease.enumValueIndex = (int)item.CreateArgs.Alpha.Ease;
            #endregion
            #endregion

            #region RecycleArgs
            #region Movement
            sp_recycle_Movement.enumValueIndex = (int)item.RecycleArgs.Movement.Movement;
            sp_recycle_Movement_Distance.floatValue = item.RecycleArgs.Movement.Distance;
            sp_recycle_Movement_Duration.floatValue = item.RecycleArgs.Movement.Duration;
            sp_recycle_Movement_Delay.floatValue = item.RecycleArgs.Movement.Delay;
            sp_recycle_Movement_Ease.enumValueIndex = (int)item.RecycleArgs.Movement.Ease;
            #endregion

            #region Rotation
            sp_recycle_Rotation.enumValueIndex = (int)item.RecycleArgs.Rotation.Rotation;
            sp_recycle_Rotation_Degree.floatValue = item.RecycleArgs.Rotation.Degree;
            sp_recycle_Rotation_Duration.floatValue = item.RecycleArgs.Rotation.Duration;
            sp_recycle_Rotation_Delay.floatValue = item.RecycleArgs.Rotation.Delay;
            sp_recycle_Rotation_Ease.enumValueIndex = (int)item.RecycleArgs.Rotation.Ease;
            #endregion

            #region Alpha
            sp_recycle_Alpha_Duration.floatValue = item.RecycleArgs.Alpha.Duration;
            sp_recycle_Alpha_Delay.floatValue = item.RecycleArgs.Alpha.Delay;
            sp_recycle_Alpha_Ease.enumValueIndex = (int)item.RecycleArgs.Alpha.Ease;
            #endregion
            #endregion

            sp_SpawnName.stringValue = item.SpawnName;
            sp_Indicator.stringValue = "Indicator_" + item.SpawnName;
            sp_ID.stringValue = XGUI_Utilitys.GenerateUniqueId(CollectIDs(XHudSpace.屏幕空间));
            sp_Delay_Spawn.floatValue = item.Delay_Spawn;
            sp_Position.vector3Value = item.Position;
            sp_Offset.vector3Value = item.Offset;
            sp_Scale.vector3Value = item.Scale;
            sp_Euler.vector3Value = item.Euler;
            sp_Pivot.vector2Value = item.Pivot;
            sp_Anchor_Min.vector2Value = item.Anchor_Min;
            sp_Anchor_Max.vector2Value = item.Anchor_Max;
            sp_Size.vector2Value = item.Size;
            sp_AutoIn.stringValue = item.AutoIn;
            sp_UseSpawnerMotion.stringValue = item.UseSpawnerMotion;
            sp_isFold.boolValue = item.isFold;
            sp_isEnabled.boolValue = item.isEnabled;

            SpawnItemList_Screen.serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 载入一个生成元素 - 屏幕 Screen
        /// </summary>
        /// <param name="item"></param>
        private void LoadSpawnItem_Screen(XHud_LayoutSpawner_Item item)
        {
            string hex_col = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary, true);
            for (int i = 0; i < HudManager.Hud_ElementLibrarys.Count; i++)
            {
                if (HudManager.Hud_ElementLibrarys[i].LibraryName != LibName.stringValue)
                {
                    XGUI_Utilitys.Console("XHud - 布局元素生成器通知", $"未找到元素库： <b><color={hex_col}>{LibName.stringValue}</color></b>！请检查XHud 管理器中是否正确 配置 / 加入 了<b><color={hex_col}> {LibName.stringValue} </color></b>元素库？", XGUIMsgState.通知);
                    continue;
                }

                if (!item.isEnabled)
                    continue;

                XHud_Module_Element ele = HudManager.Hud_ElementLibrarys[i].ElementsLibrary_GetTargetElement(item.SpawnName);

                // 验证从库中能获取到目标元素，如果不能说明库中不存在目标元素，则跳过继续下一个
                if (ele == null)
                {
                    XGUI_Utilitys.Console("XHud - 布局元素生成器通知", $"未在元素库： <b><color={hex_col}>{LibName.stringValue} </color></b>中找到：<b><color={hex_col}> {item.SpawnName} </color></b>元素！请检查目标元素库中是否存在该元素！", XGUIMsgState.通知);
                    continue;
                }

                XHud_Module_Element Element = (XHud_Module_Element)PrefabUtility.InstantiatePrefab(ele);
                // 这里的判断是为了更符合直觉的锚点选用，如果用户切换为  "元素自身动效" 那么生成的元素锚点则跟随元素自身的锚点设置，如果用户切换为  "列表项动效" 那么就是根据用户扫描的锚点为基准
                RectTransform Root = HudManager.hm_ScreenElement_GetAnchored_RectTransform(item.UseSpawnerMotion == "元素自身动效" ? Element.CreateArgs.anchor : item.UseSpawnerMotion == "列表项动效" ? item.CreateArgs.anchor : BaseScript.CreateArgs.anchor);
                Element.RectTransform.SetParent(Root);
                Element.RectTransform.anchoredPosition3D = item.Position + item.Offset;
                Element.RectTransform.localScale = item.Scale;
                Element.RectTransform.localEulerAngles = item.Euler;
                Element.RectTransform.pivot = item.Pivot;
                Element.RectTransform.anchorMin = item.Anchor_Min;
                Element.RectTransform.anchorMax = item.Anchor_Max;
                Element.element_AlphaSet(1);

                Element.RectTransform.sizeDelta = item.Size;
                item.SpawnedElementNode = new XHudElementNode();
                item.SpawnedElementNode.Element = Element;
                item.SpawnedElementNode.ID = 0;
                item.SpawnedElementNode.Indicator = Element.Indicator;
                item.SpawnedElementNode.ModuleName = Element.name;
                break;
            }
        }

        /// <summary>
        /// 卸载一个生成元素 - 屏幕 Screen
        /// </summary>
        /// <param name="item"></param>
        private void UnLoadSpawnItem_Screen(XHud_LayoutSpawner_Item item)
        {
            if (item.SpawnedElementNode.Element == null)
                return;
            Undo.DestroyObjectImmediate(item.SpawnedElementNode.Element.gameObject);
            item.SpawnedElementNode = null;
        }

        /// <summary>
        /// 清空生成项列表 - 世界 World
        /// </summary>
        private void ClearSpawnItemList_World()
        {
            SpawnItemList_World.ClearArray();
            SpawnItemList_World.serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 添加一个生成项 - 世界 World
        /// </summary>
        /// <param name="item"></param>
        private void AddedSpawnItem_World(XHud_LayoutSpawner_Item item)
        {
            SerializedProperty sp_Item;
            if (SpawnItemList_World.arraySize <= 0)
            {
                SpawnItemList_World.InsertArrayElementAtIndex(0);
                sp_Item = SpawnItemList_World.GetArrayElementAtIndex(0);
            }
            else
            {
                SpawnItemList_World.InsertArrayElementAtIndex(SpawnItemList_World.arraySize);
                sp_Item = SpawnItemList_World.GetArrayElementAtIndex(SpawnItemList_World.arraySize - 1);
            }

            SerializedProperty sp_SpawnName = sp_Item.FindPropertyRelative("SpawnName");
            SerializedProperty sp_Indicator = sp_Item.FindPropertyRelative("Indicator");
            SerializedProperty sp_ID = sp_Item.FindPropertyRelative("ID");
            SerializedProperty sp_Delay_Spawn = sp_Item.FindPropertyRelative("Delay_Spawn");
            SerializedProperty sp_Position = sp_Item.FindPropertyRelative("Position");
            SerializedProperty sp_Euler = sp_Item.FindPropertyRelative("Euler");
            SerializedProperty sp_Offset = sp_Item.FindPropertyRelative("Offset");
            SerializedProperty sp_Scale = sp_Item.FindPropertyRelative("Scale");
            SerializedProperty sp_Size = sp_Item.FindPropertyRelative("Size");
            SerializedProperty sp_AutoIn = sp_Item.FindPropertyRelative("AutoIn");
            SerializedProperty sp_UseSpawnerMotion = sp_Item.FindPropertyRelative("UseSpawnerMotion");
            SerializedProperty sp_isFold = sp_Item.FindPropertyRelative("isFold");
            SerializedProperty sp_isEnabled = sp_Item.FindPropertyRelative("isEnabled");

            #region SerializedProperty - CreateArgs
            SerializedProperty sp_CreateArgs = sp_Item.FindPropertyRelative("CreateArgs");
            SerializedProperty sp_creator_anchor = sp_CreateArgs.FindPropertyRelative("anchor");
            SerializedProperty sp_creator_Movement = sp_CreateArgs.FindPropertyRelative("Movement.Movement");
            SerializedProperty sp_creator_Movement_Distance = sp_CreateArgs.FindPropertyRelative("Movement.Distance");
            SerializedProperty sp_creator_Movement_Duration = sp_CreateArgs.FindPropertyRelative("Movement.Duration");
            SerializedProperty sp_creator_Movement_Delay = sp_CreateArgs.FindPropertyRelative("Movement.Delay");
            SerializedProperty sp_creator_Movement_Ease = sp_CreateArgs.FindPropertyRelative("Movement.Ease");

            SerializedProperty sp_creator_Rotation = sp_CreateArgs.FindPropertyRelative("Rotation.Rotation");
            SerializedProperty sp_creator_Rotation_Degree = sp_CreateArgs.FindPropertyRelative("Rotation.Degree");
            SerializedProperty sp_creator_Rotation_Duration = sp_CreateArgs.FindPropertyRelative("Rotation.Duration");
            SerializedProperty sp_creator_Rotation_Delay = sp_CreateArgs.FindPropertyRelative("Rotation.Delay");
            SerializedProperty sp_creator_Rotation_Ease = sp_CreateArgs.FindPropertyRelative("Rotation.Ease");

            SerializedProperty sp_creator_Alpha_Duration = sp_CreateArgs.FindPropertyRelative("Alpha.Duration");
            SerializedProperty sp_creator_Alpha_Delay = sp_CreateArgs.FindPropertyRelative("Alpha.Delay");
            SerializedProperty sp_creator_Alpha_Ease = sp_CreateArgs.FindPropertyRelative("Alpha.Ease");
            #endregion

            #region SerializedProperty - RecycleArgs
            SerializedProperty sp_RecycleArgs = sp_Item.FindPropertyRelative("RecycleArgs");
            SerializedProperty sp_recycle_Movement = sp_RecycleArgs.FindPropertyRelative("Movement.Movement");
            SerializedProperty sp_recycle_Movement_Distance = sp_RecycleArgs.FindPropertyRelative("Movement.Distance");
            SerializedProperty sp_recycle_Movement_Duration = sp_RecycleArgs.FindPropertyRelative("Movement.Duration");
            SerializedProperty sp_recycle_Movement_Delay = sp_RecycleArgs.FindPropertyRelative("Movement.Delay");
            SerializedProperty sp_recycle_Movement_Ease = sp_RecycleArgs.FindPropertyRelative("Movement.Ease");

            SerializedProperty sp_recycle_Rotation = sp_RecycleArgs.FindPropertyRelative("Rotation.Rotation");
            SerializedProperty sp_recycle_Rotation_Degree = sp_RecycleArgs.FindPropertyRelative("Rotation.Degree");
            SerializedProperty sp_recycle_Rotation_Duration = sp_RecycleArgs.FindPropertyRelative("Rotation.Duration");
            SerializedProperty sp_recycle_Rotation_Delay = sp_RecycleArgs.FindPropertyRelative("Rotation.Delay");
            SerializedProperty sp_recycle_Rotation_Ease = sp_RecycleArgs.FindPropertyRelative("Rotation.Ease");

            SerializedProperty sp_recycle_Alpha_Duration = sp_RecycleArgs.FindPropertyRelative("Alpha.Duration");
            SerializedProperty sp_recycle_Alpha_Delay = sp_RecycleArgs.FindPropertyRelative("Alpha.Delay");
            SerializedProperty sp_recycle_Alpha_Ease = sp_RecycleArgs.FindPropertyRelative("Alpha.Ease");
            #endregion

            #region CreateArgs - 赋值
            sp_creator_anchor.enumValueIndex = (int)item.CreateArgs.anchor;

            #region Movement
            sp_creator_Movement.enumValueIndex = (int)item.CreateArgs.Movement.Movement;
            sp_creator_Movement_Distance.floatValue = item.CreateArgs.Movement.Distance;
            sp_creator_Movement_Duration.floatValue = item.CreateArgs.Movement.Duration;
            sp_creator_Movement_Delay.floatValue = item.CreateArgs.Movement.Delay;
            sp_creator_Movement_Ease.enumValueIndex = (int)item.CreateArgs.Movement.Ease;
            #endregion

            #region Rotation
            sp_creator_Rotation.enumValueIndex = (int)item.CreateArgs.Rotation.Rotation;
            sp_creator_Rotation_Degree.floatValue = item.CreateArgs.Rotation.Degree;
            sp_creator_Rotation_Duration.floatValue = item.CreateArgs.Rotation.Duration;
            sp_creator_Rotation_Delay.floatValue = item.CreateArgs.Rotation.Delay;
            sp_creator_Rotation_Ease.enumValueIndex = (int)item.CreateArgs.Rotation.Ease;
            #endregion

            #region Alpha
            sp_creator_Alpha_Duration.floatValue = item.CreateArgs.Alpha.Duration;
            sp_creator_Alpha_Delay.floatValue = item.CreateArgs.Alpha.Delay;
            sp_creator_Alpha_Ease.enumValueIndex = (int)item.CreateArgs.Alpha.Ease;
            #endregion
            #endregion

            #region RecycleArgs - 赋值
            #region Movement
            sp_recycle_Movement.enumValueIndex = (int)item.RecycleArgs.Movement.Movement;
            sp_recycle_Movement_Distance.floatValue = item.RecycleArgs.Movement.Distance;
            sp_recycle_Movement_Duration.floatValue = item.RecycleArgs.Movement.Duration;
            sp_recycle_Movement_Delay.floatValue = item.RecycleArgs.Movement.Delay;
            sp_recycle_Movement_Ease.enumValueIndex = (int)item.RecycleArgs.Movement.Ease;
            #endregion

            #region Rotation
            sp_recycle_Rotation.enumValueIndex = (int)item.RecycleArgs.Rotation.Rotation;
            sp_recycle_Rotation_Degree.floatValue = item.RecycleArgs.Rotation.Degree;
            sp_recycle_Rotation_Duration.floatValue = item.RecycleArgs.Rotation.Duration;
            sp_recycle_Rotation_Delay.floatValue = item.RecycleArgs.Rotation.Delay;
            sp_recycle_Rotation_Ease.enumValueIndex = (int)item.RecycleArgs.Rotation.Ease;
            #endregion

            #region Alpha
            sp_recycle_Alpha_Duration.floatValue = item.RecycleArgs.Alpha.Duration;
            sp_recycle_Alpha_Delay.floatValue = item.RecycleArgs.Alpha.Delay;
            sp_recycle_Alpha_Ease.enumValueIndex = (int)item.RecycleArgs.Alpha.Ease;
            #endregion
            #endregion

            sp_SpawnName.stringValue = item.SpawnName;
            sp_Indicator.stringValue = "Indicator_" + item.SpawnName;
            sp_ID.stringValue = XGUI_Utilitys.GenerateUniqueId(CollectIDs(XHudSpace.世界空间));
            sp_Delay_Spawn.floatValue = item.Delay_Spawn;
            sp_Position.vector3Value = item.Position;
            sp_Euler.vector3Value = item.Euler;
            sp_Offset.vector3Value = item.Offset;
            sp_Scale.vector3Value = item.Scale;
            sp_Size.vector2Value = item.Size;
            sp_AutoIn.stringValue = item.AutoIn;
            sp_UseSpawnerMotion.stringValue = item.UseSpawnerMotion;
            sp_isFold.boolValue = item.isFold;
            sp_isEnabled.boolValue = item.isEnabled;

            SpawnItemList_World.serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 卸载一个生成元素 - 世界 World
        /// </summary>
        /// <param name="item"></param>
        private void UnLoadSpawnItem_World(XHud_LayoutSpawner_Item item)
        {
            if (item.SpawnedElementNode.Element == null)
                return;

            Undo.DestroyObjectImmediate(item.SpawnedElementNode.Element.gameObject);
            item.SpawnedElementNode = null;
        }

        /// <summary>
        /// 载入一个生成元素 - 世界 World
        /// </summary>
        /// <param name="item"></param>
        private void LoadSpawnItem_World(XHud_LayoutSpawner_Item item)
        {
            string hex_col = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary, true);

            for (int i = 0; i < HudManager.Hud_ElementLibrarys.Count; i++)
            {
                if (HudManager.Hud_ElementLibrarys[i].LibraryName != LibName.stringValue)
                {
                    XGUI_Utilitys.Console("XHud - 布局元素生成器通知", $"未找到元素库： <b><color={hex_col}>{LibName.stringValue}</color></b>！请检查XHud 管理器中是否正确 配置 / 加入 了<b><color={hex_col}> {LibName.stringValue} </color></b>元素库？", XGUIMsgState.通知);
                    continue;
                }

                if (!item.isEnabled)
                    continue;

                XHud_Module_Element ele = HudManager.Hud_ElementLibrarys[i].ElementsLibrary_GetTargetElement(item.SpawnName);

                // 验证从库中能获取到目标元素，如果不能说明库中不存在目标元素，则跳过继续下一个
                if (ele == null)
                {
                    XGUI_Utilitys.Console("XHud - 布局元素生成器通知", $"未在元素库： <b><color={hex_col}>{LibName.stringValue} </color></b>中找到：<b><color={hex_col}> {item.SpawnName} </color></b>元素！请检查目标元素库中是否存在该元素！", XGUIMsgState.通知);
                    continue;
                }

                XHud_Module_Element Element = (XHud_Module_Element)PrefabUtility.InstantiatePrefab(ele);
                RectTransform Root = HudManager.hm_GetAnchorRoot(XHudSpace.世界空间);
                Element.RectTransform.SetParent(Root);
                Element.RectTransform.position = item.Position + item.Offset;
                Element.RectTransform.localScale = item.Scale;
                Element.RectTransform.localEulerAngles = item.Euler;
                Element.RectTransform.sizeDelta = item.Size;
                item.SpawnedElementNode = new XHudElementNode();
                item.SpawnedElementNode.Element = Element;
                item.SpawnedElementNode.ID = 0;
                item.SpawnedElementNode.Indicator = Element.Indicator;
                item.SpawnedElementNode.ModuleName = Element.name;
                break;
            }
        }
        #endregion

        #region GUI
        /// <summary>
        /// 标题面板
        /// </summary>
        /// <param name="title"></param>
        /// <param name="margin"></param>
        /// <param name="Fill"></param>
        /// <param name="color"></param>
        /// <param name="titlecolor"></param>
        /// <param name="titlecolor_hover"></param>
        /// <param name="titlecolor_active"></param>
        /// <param name="margin_btn"></param>
        /// <param name="offset"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool xHud_FunctionGroup(string title, float margin, HudFilled Fill, HudColor color, Color titlecolor, Color titlecolor_hover, Color titlecolor_active, RectOffset margin_btn, Vector2 offset, string key, Texture2D icon)
        {
            int inspectorwidth = Screen.width;
            //sp_DebugMode.Log(inspectorwidth);

            bool sw_option = XGUI.x_Editor_Data_Get_With_Bool(key);
            sw_option = Editor_XHud_GUI.Gui_Layout_Vertical_Start_WithFolder(Fill, color, margin, title, titlecolor, titlecolor_hover, titlecolor_active, margin_btn, offset, icon, sw_option, inspectorwidth);
            XGUI.x_Editor_Data_Set_With_Bool(key, sw_option);
            return sw_option;
        }
        #endregion

        #region 动画预览
        /// <summary>
        /// 创建收集图元动画器的动画节点列表所有动画
        /// </summary>
        /// <param name="tweener"></param>
        /// <returns></returns>
        private XTween_Interface[] Preview_PrimitiveTweens_Collected(XHud_Module_Primitive_Tween tweener, string tim, float dur, float delay)
        {
            List<XTween_Interface> tweens = new List<XTween_Interface>();
            for (int i = 0; i < tweener.PrimitiveTweenNodes.Count; i++)
            {
                if (!tweener.PrimitiveTweenNodes[i].Enabled)
                    continue;
                if (tweener.PrimitiveTweenNodes[i].Timings != tim)
                    continue;
                XTween_Interface tween = tweener.Tween_Create(tweener.PrimitiveTweenNodes[i], tweener.GlobalDuration * HudManager.DurationMultiply * dur);

                tween.SetDelay(tween.Delay + delay);

                if (tween != null)
                    tweens.Add(tween);
            }

            return tweens.ToArray();
        }
        /// <summary>
        /// 根据空间类型预览元素动画
        /// </summary>
        private void LayoutsTweenPreview(XHudSpace space, XHudElementCreateState state)
        {
            XTween_Preview_Kill(space);

            // 预览列表
            List<XTween_Interface> preview_twns = new List<XTween_Interface>();

            for (int i = 0; i < (space == XHudSpace.屏幕空间 ? BaseScript.SpawnItemList_Screen.Count : BaseScript.SpawnItemList_World.Count); i++)
            {
                XHud_LayoutSpawner_Item spw_Item = (space == XHudSpace.屏幕空间 ? BaseScript.SpawnItemList_Screen[i] : BaseScript.SpawnItemList_World[i]);

                if (spw_Item.SpawnedElementNode.Element == null)
                    continue;

                XHud_Module_Element ele = spw_Item.SpawnedElementNode.Element;

                switch (state)
                {
                    case XHudElementCreateState.Created:
                        // 每个元素创建动画
                        ele.ElementTweens_Creator(spw_Item.UseSpawnerMotion == "元素自身动效" ? ele.CreateArgs : (spw_Item.UseSpawnerMotion == "列表项动效" ? spw_Item.CreateArgs : BaseScript.CreateArgs), null, true);
                        break;
                    case XHudElementCreateState.Recycled:
                        // 每个元素创建动画
                        ele.ElementTweens_Recycler(spw_Item.UseSpawnerMotion == "元素自身动效" ? ele.RecycleArgs : (spw_Item.UseSpawnerMotion == "列表项动效" ? spw_Item.RecycleArgs : BaseScript.RecycleArgs), null, true);
                        break;
                }

                // 生成器列表中的延迟时间
                float delay_spawner = state == XHudElementCreateState.Created ? spw_Item.Delay_Spawn : spw_Item.Delay_Despawn;

                #region 每个元素的基础三项动画 - 加入预览列表
                if (ele.Tween_Alpha != null)
                {
                    ele.Tween_Alpha.SetDelay(ele.Tween_Alpha.Delay + delay_spawner);
                    preview_twns.Add(ele.Tween_Alpha);
                }
                if (ele.Tween_Move != null)
                {
                    ele.Tween_Move.SetDelay(ele.Tween_Move.Delay + delay_spawner);
                    preview_twns.Add(ele.Tween_Move);
                }
                if (ele.Tween_Rotation != null)
                {
                    ele.Tween_Rotation.SetDelay(ele.Tween_Rotation.Delay + delay_spawner);
                    preview_twns.Add(ele.Tween_Rotation);
                }
                #endregion

                #region 元素子级中的图元动画 - 加入预览列表
                if (ele.PreviewIncludePrimitivesTween)
                {
                    for (int c = 0; c < ele.PrimitiveControllerNodes.Count; c++)
                    {
                        if (ele.PrimitiveControllerNodes[c].Controller.pt_Tween == null)
                            continue;

                        XTween_Interface[] primitive_tweens = Preview_PrimitiveTweens_Collected(ele.PrimitiveControllerNodes[c].Controller.pt_Tween, state == XHudElementCreateState.Created ? "元素进入时" : "元素退出时", ele.PrimitivesTweenGlobalDuration, ele.PrimitiveControllerNodes[c].DelayTime + delay_spawner);

                        for (int s = 0; s < primitive_tweens.Length; s++)
                        {
                            preview_twns.Add(primitive_tweens[s]);
                        }
                    }
                }
                #endregion
            }

            XTween_Preview_Start(preview_twns.ToArray(), space);
        }

        //------------------------------------------------------------------------------------

        public XHudSpace cur_space;

        /// <summary>
        /// 动画预览 - 播放
        /// </summary>
        /// <param name="tweens">传入需要预览的动画，但前提是动画已创建，如果是空的则会导致预览异常</param>
        public void XTween_Preview_Start(XTween_Interface[] tweens, XHudSpace space)
        {
            if (Application.isPlaying)
                return;

            // 预览动画杀死后自动清空预览器的列表
            Editor_XTween_Previewer.AfterKillClear = true;
            // 预览动画杀死前将动画目标的属性倒退
            Editor_XTween_Previewer.BeforeKillRewind = true;

            Editor_XTween_Previewer.AutoKillWithDuration = true;

            // 预览动画杀死后的委托事件
            Editor_XTween_Previewer.act_on_editor_autokill += XTween_OnAutoKillPreview;


            for (int i = 0; i < tweens.Length; i++)
            {
                Editor_XTween_Previewer.Append(tweens[i]);
            }

            Editor_XTween_Previewer.Play(null);

            cur_space = space;

            Editor_XHud_Tool_SceneView_Activate_Mark.SetEnabled(
               state: true,
               x_color: XHud_Dashboard.Theme_Primary,
               x_title: "Seven Strike Media",
               x_msg: "元素动效预览中...",
               x_anchor: XHudSceneActivateMarkAnchor.左下);
        }
        /// <summary>
        ///  动画预览 - 杀死
        /// </summary>
        private void XTween_Preview_Kill(XHudSpace space)
        {
            if (Application.isPlaying)
                return;

            // 预览器执行动作：杀死动画
            Editor_XTween_Previewer.Kill(true, true, () =>
            {
                //BaseScript.CurrentTweener = null;
            });

            // 当动画预览器为根据动画耗时自动杀死的情况下
            Editor_XTween_Previewer.act_on_editor_autokill -= XTween_OnAutoKillPreview;

            // 根据是否包含图元动画预览类型对图元动画进行特性复位
            for (int i = 0; i < (space == XHudSpace.屏幕空间 ? BaseScript.SpawnItemList_Screen.Count : BaseScript.SpawnItemList_World.Count); i++)
            {
                XHud_LayoutSpawner_Item spw_item = space == XHudSpace.屏幕空间 ? BaseScript.SpawnItemList_Screen[i] : BaseScript.SpawnItemList_World[i];

                if (spw_item.SpawnedElementNode.Element == null)
                    continue;

                for (int s = 0; s < spw_item.SpawnedElementNode.Element.PrimitiveControllerNodes.Count; s++)
                {
                    PrimitiveControllerNode node = spw_item.SpawnedElementNode.Element.PrimitiveControllerNodes[s];
                    if (node.Controller.pt_Feature == null)
                        continue;
                    node.Controller.pt_Feature.PrimitiveFeature_Load();
                }

                // 杀死元素三项自身动画
                spw_item.SpawnedElementNode.Element.ElementTweens_Kill();
                // 清空元素三项自身动画
                spw_item.SpawnedElementNode.Element.ElementTweens_Clear();
            }

            cur_space = space;

            Editor_XHud_Tool_SceneView_Activate_Mark.SetEnabled(false);
        }
        /// <summary>
        ///  动画预览 - 自动杀死的委托
        /// </summary>
        private void XTween_OnAutoKillPreview()
        {
            // 根据是否包含图元动画预览类型对图元动画进行特性复位
            for (int i = 0; i < (cur_space == XHudSpace.屏幕空间 ? BaseScript.SpawnItemList_Screen.Count : BaseScript.SpawnItemList_World.Count); i++)
            {
                XHud_LayoutSpawner_Item spw_item = (cur_space == XHudSpace.屏幕空间 ? BaseScript.SpawnItemList_Screen[i] : BaseScript.SpawnItemList_World[i]);

                for (int s = 0; s < spw_item.SpawnedElementNode.Element.PrimitiveControllerNodes.Count; s++)
                {
                    PrimitiveControllerNode node = spw_item.SpawnedElementNode.Element.PrimitiveControllerNodes[s];
                    if (node.Controller.pt_Feature == null)
                        continue;
                    node.Controller.pt_Feature.PrimitiveFeature_Load();
                }

                // 杀死元素三项自身动画
                spw_item.SpawnedElementNode.Element.ElementTweens_Kill();
                // 清空元素三项自身动画
                spw_item.SpawnedElementNode.Element.ElementTweens_Clear();
            }

            // 清空预览动画杀死后的委托事件
            Editor_XTween_Previewer.act_on_editor_autokill -= XTween_OnAutoKillPreview;

            Editor_XHud_Tool_SceneView_Activate_Mark.SetEnabled(false);
        }
        /// <summary>
        /// 预览倒退重置
        /// </summary>
        private void XTween_Preview_Rewind()
        {
            Editor_XTween_Previewer.Rewind();
        }
        #endregion
    }
}