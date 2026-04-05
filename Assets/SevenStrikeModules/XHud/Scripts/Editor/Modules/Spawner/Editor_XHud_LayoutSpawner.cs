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
    using SevenStrikeModules.XTween;
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
        #endregion

        /// <summary>
        /// 原始脚本参数显示开关
        /// </summary>
        private bool OriginalDisplay;

        #region 序列化属性
        private SerializedProperty LibName, IsLoadedLayout_Screen, IsLoadedLayout_World, SpawnItemList_Screen, SpawnItemList_World, SpawnerIndicator, CreateArgs, RecycleArgs, Key_Create, Key_Recycle, UseManullyKey, IsSpawning, create_fold_move, create_fold_rotate, create_fold_alpha, recycle_fold_move, recycle_fold_rotate, recycle_fold_alpha, Crc_Lib_Name, Rec_Lib_Name, SpawnFunctionKey_Primary, SpawnFunctionKey_Secondary, Sequence_Spawn_Screen, Sequence_Spawn_World, Sequence_Despawn_Screen, Sequence_Despawn_World, ControlScreen, ControlWorld, FoldScreen, FoldWorld, ProtectedAction, inMotion, CreateArgs_MotionAnimateEndState, RecycleArgs_MotionAnimateEndState;
        #endregion

        #region 选项文字
        string[] stroptions_enabled = new string[2] { "关闭", "开启" };
        string[] stroptions_control = new string[2] { "禁用", "可控" };
        string[] stroptions_protecte = new string[2] { "开放", "保护" };
        #endregion

        #region 图标
        private Texture2D icon_main, save_r, save_p, locate_r, locate_p, elementicon, dot, icon_unfold_r, icon_unfold_p, anim_fold_r, anim_fold_p, reset_r, reset_p;
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

        Rect drawelement_screen_rect;
        Rect item_rect_screen;
        int selected_Screen = -1;

        Rect drawelement_world_rect;
        Rect item_rect_world;
        int selected_World = -1;

        private void OnEnable()
        {
            BaseScript = (XHud_LayoutSpawner)target;

            GetSerializeFields();

            #region 获取字体
            Font_Bold = Editor_XHud_GUI.GetFont("SS_Editor_Bold");
            Font_Light = Editor_XHud_GUI.GetFont("SS_Editor_Light");
            #endregion

            #region 获取图标
            icon_main = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner/icon_main");
            save_r = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner/save_r");
            save_p = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner/save_p");
            locate_r = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner/locate_r");
            locate_p = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner/locate_p");
            elementicon = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner/elementicon");
            dot = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner/dot");
            icon_unfold_r = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner/icon_unfold_r");
            icon_unfold_p = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner/icon_unfold_p");
            anim_fold_r = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner/anim_fold_r");
            anim_fold_p = Editor_XHud_GUI.GetIcon("Icons_XHud_LayoutSpawner/anim_fold_p");
            reset_r = Editor_XHud_GUI.GetIcon("Icons_XHud_ElementPreview/reset_r");
            reset_p = Editor_XHud_GUI.GetIcon("Icons_XHud_ElementPreview/reset_p");
            #endregion

            Targets_Get();

            #region 用于第一次加载脚本判断库名是否是空的
            if (!Application.isPlaying && XHud_Dashboard.HudManagerGet() != null)
            {
                //获取所有元素库名称
                string[] LibNames = XHud_Dashboard.HudManagerGet().hm_ElementLibrary_GetAllLibraryNames();
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
                draggable = true,
                drawElementBackgroundCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    if (selected_Screen == index)
                    {
                        // 高亮标记表示选中
                        item_rect_screen.Set(rect.x + 8, rect.y + 30, 5, 5);
                        EditorGUI.DrawRect(item_rect_screen, XHud_Dashboard.Theme_Primary);
                    }
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    SerializedProperty prop = SpawnItemList_Screen.GetArrayElementAtIndex(index);

                    SerializedProperty sp_SpawnName = prop.FindPropertyRelative("SpawnName");
                    SerializedProperty sp_Indicator = prop.FindPropertyRelative("Indicator");
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
                    Editor_XHud_GUI.Gui_Labelfield(drawelement_screen_rect, index.ToString("D1"), HudFilled.无, HudColor.无, Color.gray, TextAnchor.MiddleLeft, Vector2.zero, 11, Font_Light);
                    #endregion

                    #region 徽标
                    if (sp_Spawned.boolValue)
                        GUI.backgroundColor = XHud_Dashboard.Theme_Primary;
                    else
                        GUI.backgroundColor = Color.gray;

                    drawelement_screen_rect.Set(rect.x + 45, rect.y + 5, 10, 10);
                    Editor_XHud_GUI.Gui_Icon(drawelement_screen_rect, elementicon);
                    GUI.backgroundColor = Color.white;
                    #endregion

                    #region 名称类
                    drawelement_screen_rect.Set(rect.x + 68, rect.y, rect.width - 10 - 68, 20);
                    Editor_XHud_GUI.Gui_Labelfield(drawelement_screen_rect, sp_SpawnName.stringValue, HudFilled.无, HudColor.深空灰, XHud_Dashboard.Theme_Primary, TextAnchor.MiddleLeft, Vector2.zero, 12, true, TextClipping.Ellipsis, true, Font_Bold);

                    drawelement_screen_rect.Set(rect.x + 30, rect.y + 23, rect.width - 130 - 5, 20);
                    Editor_XHud_GUI.Gui_Labelfield(drawelement_screen_rect, sp_Indicator.stringValue, HudFilled.无, HudColor.深空灰, Color.white * 0.85f, TextAnchor.MiddleLeft, Vector2.zero, 11, true, TextClipping.Ellipsis, true, Font_Light);
                    #endregion

                    #region 折叠开关
                    if (!sp_isFold.boolValue)
                    {
                        drawelement_screen_rect.Set(rect.x + 2, rect.y + 25, 12, 12);
                        if (Editor_XHud_GUI.Gui_Button(drawelement_screen_rect, icon_unfold_r, icon_unfold_p, true, "", "", Color.white))
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
                        if (Editor_XHud_GUI.Gui_Button(drawelement_screen_rect, anim_fold_r, anim_fold_p, true, "", "", Color.white))
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
                    GUI.backgroundColor = XHud_Dashboard.Theme_Primary;
                    drawelement_screen_rect.Set(rect.width, rect.y + 27, 55, 12);
                    Editor_XHud_GUI.Gui_PopupWithString(drawelement_screen_rect, ref sp_AutoIn, new string[2] { "自动 In", "手动 In" }, HudFilled.无, HudColor.亮白, XHud_Dashboard.Theme_Primary);
                    GUI.backgroundColor = Color.white;
                    #endregion

                    #region 使用生成器的通用动效
                    GUI.backgroundColor = XHud_Dashboard.Theme_Primary;
                    drawelement_screen_rect.Set(rect.width - 70, rect.y + 27, 75, 12);
                    Editor_XHud_GUI.Gui_PopupWithString(drawelement_screen_rect, ref sp_UseSpawnerMotion, new string[2] { "自身动效", "生成器动效" }, HudFilled.无, HudColor.亮白, XHud_Dashboard.Theme_Primary);
                    GUI.backgroundColor = Color.white;
                    #endregion

                    float baseheight = rect.y + 48;

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
                    drawelement_screen_rect.Set(rect.x, baseheight + 10, rect.width / 2 - 5, 18);
                    Editor_XHud_GUI.Gui_Property_Field(drawelement_screen_rect, "延迟生成", "ds", sp_Delay_Spawn);

                    drawelement_screen_rect.Set(rect.x + rect.width / 2 + 5, baseheight + 10, rect.width / 2 - 5, 18);
                    Editor_XHud_GUI.Gui_Property_Field(drawelement_screen_rect, "延迟回收", "dd", sp_Delay_Despawn);
                    #endregion

                    #region 信息
                    if (!sp_isFold.boolValue)
                    {
                        string hexcol = XHud_Utilitys.Color_To_HexColor(Color.white * 0.75f, true);
                        string darkcol = XHud_Utilitys.Color_To_HexColor(Color.gray, true);

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

                    #region  检测鼠标点击事件
                    Event e = Event.current;
                    drawelement_screen_rect.Set(rect.x, rect.y, rect.width - 50, rect.height);
                    if (e.type == EventType.MouseDown && e.button == 1 && drawelement_screen_rect.Contains(e.mousePosition))
                    {
                        // 创建右键菜单
                        GenericMenu menu = new GenericMenu();
                        menu.AddItem(new GUIContent("E 修改元素动效"), false, () =>
                        {
                            XHud.XHud_LibraryArg_Motion motion = new XHud.XHud_LibraryArg_Motion("", 0, "", BaseScript.SpawnItemList_Screen[index].CreateArgs, BaseScript.SpawnItemList_Screen[index].RecycleArgs);
                            OpenParameterModifier(motion, sp_SpawnName.stringValue);
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
                        height = 100;
                    }
                    else
                    {
                        height = 210;
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
                }
            };
            #endregion

            #region 世界元素
            ElementList_World = new ReorderableList(serializedObject, SpawnItemList_World)
            {
                draggable = true,
                drawElementBackgroundCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    if (selected_World == index)
                    {
                        // 高亮标记表示选中
                        item_rect_world.Set(rect.x + 8, rect.y + 30, 5, 5);
                        EditorGUI.DrawRect(item_rect_world, XHud_Dashboard.Theme_Primary);
                    }
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    SerializedProperty prop = SpawnItemList_World.GetArrayElementAtIndex(index);

                    SerializedProperty sp_SpawnName = prop.FindPropertyRelative("SpawnName");
                    SerializedProperty sp_Indicator = prop.FindPropertyRelative("Indicator");
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
                    Editor_XHud_GUI.Gui_Labelfield(drawelement_world_rect, index.ToString("D1"), HudFilled.无, HudColor.无, Color.gray, TextAnchor.MiddleLeft, Vector2.zero, 11, Font_Light);
                    #endregion

                    #region 徽标
                    if (sp_Spawned.boolValue)
                        GUI.backgroundColor = XHud_Dashboard.Theme_Primary;
                    else
                        GUI.backgroundColor = Color.gray;

                    drawelement_world_rect.Set(rect.x + 45, rect.y + 5, 10, 10);
                    Editor_XHud_GUI.Gui_Icon(drawelement_world_rect, elementicon);
                    GUI.backgroundColor = Color.white;
                    #endregion

                    #region 名称类
                    drawelement_world_rect.Set(rect.x + 68, rect.y, rect.width - 10 - 68, 20);
                    Editor_XHud_GUI.Gui_Labelfield(drawelement_world_rect, sp_SpawnName.stringValue, HudFilled.无, HudColor.深空灰, XHud_Dashboard.Theme_Primary, TextAnchor.MiddleLeft, Vector2.zero, 12, true, TextClipping.Ellipsis, true, Font_Bold);

                    drawelement_world_rect.Set(rect.x + 30, rect.y + 23, rect.width - 130 - 5, 20);
                    Editor_XHud_GUI.Gui_Labelfield(drawelement_world_rect, sp_Indicator.stringValue, HudFilled.无, HudColor.深空灰, Color.white * 0.85f, TextAnchor.MiddleLeft, Vector2.zero, 11, true, TextClipping.Ellipsis, true, Font_Light);
                    #endregion

                    #region 折叠开关
                    if (!sp_isFold.boolValue)
                    {
                        drawelement_world_rect.Set(rect.x + 2, rect.y + 25, 12, 12);
                        if (Editor_XHud_GUI.Gui_Button(drawelement_world_rect, icon_unfold_r, icon_unfold_p, true, "", "", Color.white))
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
                        if (Editor_XHud_GUI.Gui_Button(drawelement_world_rect, anim_fold_r, anim_fold_p, true, "", "", Color.white))
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
                    drawelement_world_rect.Set(rect.width, rect.y + 27, 55, 12);
                    Editor_XHud_GUI.Gui_PopupWithString(drawelement_world_rect, ref sp_AutoIn, new string[2] { "自动 In", "手动 In" }, HudFilled.无, HudColor.亮白, XHud_Dashboard.Theme_Primary);
                    GUI.backgroundColor = Color.white;
                    #endregion

                    #region 使用生成器的通用动效
                    GUI.backgroundColor = XHud_Dashboard.Theme_Primary;
                    drawelement_world_rect.Set(rect.width - 70, rect.y + 27, 75, 12);
                    Editor_XHud_GUI.Gui_PopupWithString(drawelement_world_rect, ref sp_UseSpawnerMotion, new string[2] { "自身动效", "生成器动效" }, HudFilled.无, HudColor.亮白, XHud_Dashboard.Theme_Primary);
                    GUI.backgroundColor = Color.white;
                    #endregion

                    float baseheight = rect.y + 48;

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
                        string hexcol = XHud_Utilitys.Color_To_HexColor(Color.white * 0.75f, true);
                        string darkcol = XHud_Utilitys.Color_To_HexColor(Color.gray, true);

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

                    #region  检测鼠标点击事件
                    Event e = Event.current;
                    drawelement_world_rect.Set(rect.x, rect.y, rect.width - 50, rect.height);
                    if (e.type == EventType.MouseDown && e.button == 1 && drawelement_world_rect.Contains(e.mousePosition))
                    {
                        // 创建右键菜单
                        GenericMenu menu = new GenericMenu();
                        menu.AddItem(new GUIContent("E 修改元素动效"), false, () =>
                        {
                            XHud.XHud_LibraryArg_Motion motion = new XHud.XHud_LibraryArg_Motion("", 0, "", BaseScript.SpawnItemList_World[index].CreateArgs, BaseScript.SpawnItemList_World[index].RecycleArgs);
                            OpenParameterModifier(motion, sp_SpawnName.stringValue);
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
                        height = 100;
                    }
                    else
                    {
                        height = 210;
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
                }
            };
            #endregion
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
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            Editor_XHud_GUI.Gui_Layout_Banner(icon_main, HudFilled.实体, HudColor.深空灰, "XHud - 布局元素生成器", Color.white);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

            #region 状态
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 0, "状态", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            Editor_XHud_GUI.StatuDisplayer_text(null, 12, new Vector2(0, 7), "生成器状态", 12, inMotion.boolValue ? "运行中" : "待命中", XHud_Dashboard.Theme_Primary, 11, false);

            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 选项
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 0, "选项", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_LayoutSpawner>("手动生成与回收", stroptions_enabled, ref UseManullyKey, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_LayoutSpawner>("屏幕元素控制", stroptions_control, ref ControlScreen, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_LayoutSpawner>("世界元素控制", stroptions_control, ref ControlWorld, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_LayoutSpawner>("防误触操作", stroptions_protecte, ref ProtectedAction, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

            if (mgr == null)
            {
                EditorGUILayout.HelpBox("未找到Hud管理器!", MessageType.Error);
            }
            else
            {
                if (mgr.Hud_ElementLibrarys == null || mgr.Hud_ElementLibrarys.Count <= 0)
                {
                    EditorGUILayout.HelpBox("未找到任何已指定的元素库，请先添加元素库项!", MessageType.Warning);
                }
                else
                {
                    #region 选择元素库
                    string[] libsname = mgr.hm_ElementLibrary_GetAllLibraryNames();

                    Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_LayoutSpawner>("元素库", libsname, ref LibName, HudFilled.实体, 120, 22, SelectedObjects, (comps) => { }, (res) => { });
                    #endregion
                }
            }

            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 参数
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 0, "参数", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(10);

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

            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_XHud_GUI.Gui_Layout_Space(5);
            EditorGUILayout.HelpBox("间隔时间为0时，元素按照并发模式生成 / 回收", MessageType.Info);
            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Horizontal_End();

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("生成间隔  (屏幕空间)", Sequence_Spawn_Screen, 170);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("回收间隔  (屏幕空间)", Sequence_Despawn_Screen, 170);

            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Seperator(1, XHud_Dashboard.Theme_SeperateLine);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_XHud_GUI.Gui_Layout_Space(5);
            EditorGUILayout.HelpBox("间隔时间为0时，元素按照并发模式生成 / 回收", MessageType.Info);
            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Horizontal_End();

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("生成间隔  (世界空间)", Sequence_Spawn_World, 170);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("回收间隔  (世界空间)", Sequence_Despawn_World, 170);

            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Seperator(1, XHud_Dashboard.Theme_SeperateLine);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            SerializedProperty sp_anchor_type = CreateArgs.FindPropertyRelative("anchor");
            Editor_XHud_GUI.Gui_Layout_Property_Field("锚点", sp_anchor_type);

            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 元素项

            string hexcol = XHud_Utilitys.Color_To_HexColor(XHud_Dashboard.Theme_Primary, true);

            #region 屏幕空间
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 0, "元素项 - 屏幕空间", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_XHud_GUI.Gui_Layout_Space(5);
            if (SpawnItemList_Screen.arraySize <= 0)
            {
                #region 收集布局
                if (Editor_XHud_GUI.Gui_Layout_Button("收集", "收集当前在LayoutScreen锚点下的所有屏幕元素部署布局", HudFilled.实体, HudColor.深空灰, Color.white, 30, Font_Bold))
                {
                    if (string.IsNullOrEmpty(LibName.stringValue))
                    {
                        Editor_XHud_GUI.Open(XHud_DialogType.警告, $"XHud - 布局元素生成器消息", "收集元素异常", $"请先指定目标元素库！", "明白", 0);
                        return;
                    }
                    if (mgr == null)
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
                        Editor_XHud_GUI.Open(XHud_DialogType.通知, "XHud - 布局元素生成器消息", "元素物体收集", $"未发现<color={hexcol}> 屏幕根节点 </color>下存在任何Hud元素物体！", "明白", 0);
                    }
                    #endregion
                    return;
                }
                #endregion

                Editor_XHud_GUI.Gui_Layout_Space(5);
            }

            #region 重置清空
            if (Editor_XHud_GUI.Gui_Layout_Button("重置", "将当前的元素部署布局列表清空", HudFilled.实体, HudColor.深空灰, Editor_XHud_GUI.GetColor(HudColor.魅力红), 30, Font_Bold))
            {
                if (mgr == null)
                    return;

                IsLoadedLayout_Screen.boolValue = false;
                IsLoadedLayout_Screen.serializedObject.ApplyModifiedProperties();
                for (int i = 0; i < SpawnItemList_Screen.arraySize; i++)
                {
                    UnLoadSpawnItem_Screen(BaseScript.SpawnItemList_Screen[i]);
                }
                ClearSpawnItemList_Screen();
            }
            #endregion

            #region 加载 & 卸载布局
            if (SpawnItemList_Screen.arraySize > 0)
            {
                #region 加载布局
                if (!IsLoadedLayout_Screen.boolValue && Editor_XHud_GUI.Gui_Layout_Button("加载", "根据元素列表中的信息重建元素部署布局到LayoutScreen锚点下", HudFilled.实体, HudColor.深空灰, XHud_Dashboard.Theme_Primary, 30, Font_Bold))
                {
                    if (mgr == null)
                        return;
                    if (IsLoadedLayout_Screen.boolValue)
                        return;
                    IsLoadedLayout_Screen.boolValue = true;
                    IsLoadedLayout_Screen.serializedObject.ApplyModifiedProperties();

                    for (int i = 0; i < SpawnItemList_Screen.arraySize; i++)
                    {
                        LoadSpawnItem_Screen(BaseScript.SpawnItemList_Screen[i]);
                    }
                }
                #endregion

                #region 卸载布局
                if (IsLoadedLayout_Screen.boolValue && Editor_XHud_GUI.Gui_Layout_Button("卸载", "将当前重建的元素部署布局清空", HudFilled.实体, HudColor.深空灰, Editor_XHud_GUI.GetColor(HudColor.魅力红), 30, Font_Bold))
                {
                    if (mgr == null)
                        return;

                    if (!IsLoadedLayout_Screen.boolValue)
                        return;
                    IsLoadedLayout_Screen.boolValue = false;
                    IsLoadedLayout_Screen.serializedObject.ApplyModifiedProperties();

                    for (int i = 0; i < SpawnItemList_Screen.arraySize; i++)
                    {
                        UnLoadSpawnItem_Screen(BaseScript.SpawnItemList_Screen[i]);
                    }
                }
                #endregion
            }
            #endregion

            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Horizontal_End();

            Editor_XHud_GUI.Gui_Layout_Space(5);

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
            Editor_XHud_GUI.Gui_Layout_Space(5);
            if (SpawnItemList_World.arraySize <= 0)
            {
                #region 收集布局
                if (Editor_XHud_GUI.Gui_Layout_Button("收集", "收集当前在LayoutWorld锚点下的所有世界元素部署布局", HudFilled.实体, HudColor.深空灰, Color.white, 30, Font_Bold))
                {
                    if (string.IsNullOrEmpty(LibName.stringValue))
                    {
                        Editor_XHud_GUI.Open(XHud_DialogType.警告, $"XHud - 布局元素生成器消息", "收集元素异常", $"请先指定目标元素库！", "明白", 0);
                        return;
                    }

                    if (mgr == null)
                        return;

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
                        Editor_XHud_GUI.Open(XHud_DialogType.通知, "XHud - 布局元素生成器消息", "元素物体收集", $"未发现<color={hexcol}> 世界根节点 </color>下存在任何Hud元素物体！", "明白", 0);
                    }
                    #endregion               
                    return;
                }
                #endregion

                Editor_XHud_GUI.Gui_Layout_Space(5);
            }

            #region 重置清空
            if (Editor_XHud_GUI.Gui_Layout_Button("重置", "将当前的元素部署布局列表清空", HudFilled.实体, HudColor.深空灰, Editor_XHud_GUI.GetColor(HudColor.魅力红), 30, Font_Bold))
            {
                if (mgr == null)
                    return;

                IsLoadedLayout_World.boolValue = false;
                IsLoadedLayout_World.serializedObject.ApplyModifiedProperties();
                for (int i = 0; i < SpawnItemList_World.arraySize; i++)
                {
                    UnLoadSpawnItem_World(BaseScript.SpawnItemList_World[i]);
                }
                ClearSpawnItemList_World();
            }
            #endregion

            #region 加载 & 卸载布局
            if (SpawnItemList_World.arraySize > 0)
            {
                #region 加载布局
                if (!IsLoadedLayout_World.boolValue && Editor_XHud_GUI.Gui_Layout_Button("加载", "根据元素列表中的信息重建元素部署布局到LayoutWorld锚点下", HudFilled.实体, HudColor.深空灰, XHud_Dashboard.Theme_Primary, 30, Font_Bold))
                {
                    if (mgr == null)
                        return;
                    if (IsLoadedLayout_World.boolValue)
                        return;
                    IsLoadedLayout_World.boolValue = true;
                    IsLoadedLayout_World.serializedObject.ApplyModifiedProperties();

                    for (int i = 0; i < SpawnItemList_World.arraySize; i++)
                    {
                        LoadSpawnItem_World(BaseScript.SpawnItemList_World[i]);
                    }
                }
                #endregion

                #region 卸载布局
                if (IsLoadedLayout_World.boolValue && Editor_XHud_GUI.Gui_Layout_Button("卸载", "将当前重建的元素部署布局清空", HudFilled.实体, HudColor.深空灰, Editor_XHud_GUI.GetColor(HudColor.魅力红), 30, Font_Bold))
                {
                    if (mgr == null)
                        return;

                    if (!IsLoadedLayout_World.boolValue)
                        return;
                    IsLoadedLayout_World.boolValue = false;
                    IsLoadedLayout_World.serializedObject.ApplyModifiedProperties();

                    for (int i = 0; i < SpawnItemList_World.arraySize; i++)
                    {
                        UnLoadSpawnItem_World(BaseScript.SpawnItemList_World[i]);
                    }
                }
                #endregion
            }
            #endregion

            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Horizontal_End();

            Editor_XHud_GUI.Gui_Layout_Space(5);

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
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 3, "动效参数", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            #region 模版库                             
            //确保动效库存在
            if (mgr.Hud_Motions != null)
            {
                //确保动效库不是空的
                if (mgr.Hud_Motions.ElementMotionList != null && mgr.Hud_Motions.ElementMotionList.Count > 0)
                {
                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);

                    //动效列表
                    string[] motnames = mgr.Hud_Motions.ElementMotion_GetAllName_With_Create();
                    EditorGUI.BeginChangeCheck();
                    Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_LayoutSpawner>("生成", motnames, ref Crc_Lib_Name, HudFilled.实体, 120, 22, SelectedObjects);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Motion_Creator crc = mgr.Hud_Motions.ElementMotion_GetElementCreator_At_Create(Crc_Lib_Name.stringValue);

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
                        CreateArgs.serializedObject.ApplyModifiedProperties();
                    }

                    #region 保存 & 定位模板
                    Editor_XHud_GUI.Gui_Layout_Space(10);

                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "保存", save_r, save_p, 2))
                    {
                        OpenParameterSetter(HudElementMotionType.Creator);
                        return;
                    }

                    Editor_XHud_GUI.Gui_Layout_Space(10);

                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "定位", locate_r, locate_p, 2))
                    {
                        if (!mgr.Hud_Motions.ElementMotion_IsExist(Crc_Lib_Name.stringValue))
                            return;
                        Editor_XHud_MenuItemsAction_OpenLibrary.open_elementmotion();
                        mgr.Hud_Motions.ElementMotionLibrary_Location(Crc_Lib_Name.stringValue);
                        return;
                    }

                    Editor_XHud_GUI.Gui_Layout_Space(10);

                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "重置", reset_r, reset_p, 2))
                    {
                        string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 布局元素生成器消息", "重置动效参数", "确定要将动效参数重置吗？您将丢失当前的动效参数！", "重置", "暂不", 0);
                        if (res == "重置")
                            ResetMotionParams("CreateArgs");
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
                    if (mgr.Hud_Curves != null)
                    {
                        if (mgr.Hud_Curves.CurveLibrary != null && mgr.Hud_Curves.CurveLibrary.Count > 0)
                        {
                            string[] names = mgr.Hud_Curves.CurveLibrary_GetCurveNames();
                            Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_LayoutSpawner>("曲线样式", names, ref sp_CurveName, HudFilled.实体, 120, 22, SelectedObjects, (comps) => { }, (res) =>
                            {
                                sp_Curve.animationCurveValue = mgr.Hud_Curves.CurveLibrary_GetCurve(res);
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
                    if (mgr.Hud_Curves != null)
                    {
                        if (mgr.Hud_Curves.CurveLibrary != null && mgr.Hud_Curves.CurveLibrary.Count > 0)
                        {
                            string[] names = mgr.Hud_Curves.CurveLibrary_GetCurveNames();
                            Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_LayoutSpawner>("曲线样式", names, ref sp_CurveName, HudFilled.实体, 120, 22, SelectedObjects, (comps) => { }, (res) =>
                            {
                                sp_Curve.animationCurveValue = mgr.Hud_Curves.CurveLibrary_GetCurve(res);
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
                    if (mgr.Hud_Curves != null)
                    {
                        if (mgr.Hud_Curves.CurveLibrary != null && mgr.Hud_Curves.CurveLibrary.Count > 0)
                        {
                            string[] names = mgr.Hud_Curves.CurveLibrary_GetCurveNames();
                            Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_LayoutSpawner>("曲线样式", names, ref sp_CurveName, HudFilled.实体, 120, 22, SelectedObjects, (comps) => { }, (res) =>
                            {
                                sp_Curve.animationCurveValue = mgr.Hud_Curves.CurveLibrary_GetCurve(res);
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
            if (mgr.Hud_Motions != null)
            {
                //确保动效库不是空的
                if (mgr.Hud_Motions.ElementMotionList != null && mgr.Hud_Motions.ElementMotionList.Count > 0)
                {
                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);

                    //动效列表
                    string[] motnames = mgr.Hud_Motions.ElementMotion_GetAllName_With_Recycle();
                    EditorGUI.BeginChangeCheck();
                    Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_LayoutSpawner>("动效模板 - 回收", motnames, ref Rec_Lib_Name, HudFilled.实体, 120, 22, SelectedObjects);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Motion_Recycler rec = mgr.Hud_Motions.ElementMotion_GetElementCreator_At_Recycle(Rec_Lib_Name.stringValue);

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
                        RecycleArgs.serializedObject.ApplyModifiedProperties();
                    }

                    #region 保存 & 定位模板
                    Editor_XHud_GUI.Gui_Layout_Space(10);

                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "保存", save_r, save_p, 2))
                    {
                        OpenParameterSetter(HudElementMotionType.Recycler);
                        return;
                    }

                    Editor_XHud_GUI.Gui_Layout_Space(10);

                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "定位", locate_r, locate_p, 2))
                    {
                        if (!mgr.Hud_Motions.ElementMotion_IsExist(Rec_Lib_Name.stringValue, HudElementMotionType.Recycler))
                            return;
                        Editor_XHud_MenuItemsAction_OpenLibrary.open_elementmotion();
                        mgr.Hud_Motions.ElementMotionLibrary_Location(Rec_Lib_Name.stringValue);
                        return;
                    }

                    Editor_XHud_GUI.Gui_Layout_Space(10);

                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "重置", reset_r, reset_p, 2))
                    {
                        string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 布局元素生成器消息", "重置动效参数", "确定要将动效参数重置吗？您将丢失当前的动效参数！", "重置", "暂不", 0);
                        if (res == "重置")
                            ResetMotionParams("RecycleArgs");
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
                    if (mgr.Hud_Curves != null)
                    {
                        if (mgr.Hud_Curves.CurveLibrary != null && mgr.Hud_Curves.CurveLibrary.Count > 0)
                        {
                            string[] names = mgr.Hud_Curves.CurveLibrary_GetCurveNames();
                            Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_LayoutSpawner>("曲线样式", names, ref sp_CurveName, HudFilled.实体, 120, 22, SelectedObjects, (comps) => { }, (res) =>
                            {
                                sp_Curve.animationCurveValue = mgr.Hud_Curves.CurveLibrary_GetCurve(res);
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
                    if (mgr.Hud_Curves != null)
                    {
                        if (mgr.Hud_Curves.CurveLibrary != null && mgr.Hud_Curves.CurveLibrary.Count > 0)
                        {
                            string[] names = mgr.Hud_Curves.CurveLibrary_GetCurveNames();
                            Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_LayoutSpawner>("曲线样式", names, ref sp_CurveName, HudFilled.实体, 120, 22, SelectedObjects, (comps) => { }, (res) =>
                            {
                                sp_Curve.animationCurveValue = mgr.Hud_Curves.CurveLibrary_GetCurve(res);
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
                    if (mgr.Hud_Curves != null)
                    {
                        if (mgr.Hud_Curves.CurveLibrary != null && mgr.Hud_Curves.CurveLibrary.Count > 0)
                        {
                            string[] names = mgr.Hud_Curves.CurveLibrary_GetCurveNames();
                            Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_LayoutSpawner>("曲线样式", names, ref sp_CurveName, HudFilled.实体, 120, 22, SelectedObjects, (comps) => { }, (res) =>
                            {
                                sp_Curve.animationCurveValue = mgr.Hud_Curves.CurveLibrary_GetCurve(res);
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
                            Editor_XHud_GUI.EditorData_Set_With_String("XED_HudSpawner_Copy_MotionArgs", json);
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
                            Editor_XHud_GUI.EditorData_Set_With_String("XED_HudSpawner_Copy_MotionArgs", json);
                            break;
                    }

                    string mode = "";

                    if (res == "生成")
                        mode = "生成动效参数";
                    else if (res == "回收")
                        mode = "回收动效参数";

                    Editor_XHud_GUI.Open(XHud_DialogType.确认, "XHud - 布局元素生成器消息", "复制动效", $"已复制 \" {mode} \" 到系统剪贴板 ！", "明白", 0);
                });
                menu.AddItem(new GUIContent("R (粘贴动效)"), false, () =>
                {
                    string buffer = Editor_XHud_GUI.EditorData_Get_With_String("XED_HudSpawner_Copy_MotionArgs");
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

        #region 辅助

        /// <summary>
        /// 重置生成与回收的参数到默认
        /// </summary>
        private void ResetMotionParams(string state)
        {
            if (state == "CreateArgs")
            {
                CreateArgs.FindPropertyRelative("anchor").enumValueIndex = (int)XHudAnchor.中心;
                CreateArgs.FindPropertyRelative("Movement.Movement").enumValueIndex = (int)HudMotion_Movement.S_从下至上;
                CreateArgs.FindPropertyRelative("Movement.Distance").floatValue = 100;
                CreateArgs.FindPropertyRelative("Movement.Duration").floatValue = 1;
                CreateArgs.FindPropertyRelative("Movement.Delay").floatValue = 0;
                CreateArgs.FindPropertyRelative("Movement.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
                CreateArgs.FindPropertyRelative("Movement.CurveName").stringValue = "";
                CreateArgs.FindPropertyRelative("Movement.Ease").enumValueIndex = (int)EaseMode.OutQuart;
                CreateArgs.FindPropertyRelative("Rotation.Rotation").enumValueIndex = (int)HudMotion_Rotation.A_无旋转;
                CreateArgs.FindPropertyRelative("Rotation.Degree").floatValue = 0;
                CreateArgs.FindPropertyRelative("Rotation.Duration").floatValue = 1;
                CreateArgs.FindPropertyRelative("Rotation.Delay").floatValue = 0;
                CreateArgs.FindPropertyRelative("Rotation.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
                CreateArgs.FindPropertyRelative("Rotation.CurveName").stringValue = "";
                CreateArgs.FindPropertyRelative("Rotation.Ease").enumValueIndex = (int)EaseMode.OutQuart;
                CreateArgs.FindPropertyRelative("Alpha.Duration").floatValue = 1;
                CreateArgs.FindPropertyRelative("Alpha.Delay").floatValue = 0;
                CreateArgs.FindPropertyRelative("Alpha.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
                CreateArgs.FindPropertyRelative("Alpha.CurveName").stringValue = "";
                CreateArgs.FindPropertyRelative("Alpha.Ease").enumValueIndex = (int)EaseMode.OutQuart;
                CreateArgs_MotionAnimateEndState.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
                CreateArgs.serializedObject.ApplyModifiedProperties();
            }
            else if (state == "RecycleArgs")
            {
                RecycleArgs.FindPropertyRelative("Movement.Movement").enumValueIndex = (int)HudMotion_Movement.D_从上至下;
                RecycleArgs.FindPropertyRelative("Movement.Distance").floatValue = 100;
                RecycleArgs.FindPropertyRelative("Movement.Duration").floatValue = 1;
                RecycleArgs.FindPropertyRelative("Movement.Delay").floatValue = 0;
                RecycleArgs.FindPropertyRelative("Movement.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
                RecycleArgs.FindPropertyRelative("Movement.CurveName").stringValue = "";
                RecycleArgs.FindPropertyRelative("Movement.Ease").enumValueIndex = (int)EaseMode.OutQuart;
                RecycleArgs.FindPropertyRelative("Rotation.Rotation").enumValueIndex = (int)HudMotion_Rotation.A_无旋转;
                RecycleArgs.FindPropertyRelative("Rotation.Degree").floatValue = 0;
                RecycleArgs.FindPropertyRelative("Rotation.Duration").floatValue = 1;
                RecycleArgs.FindPropertyRelative("Rotation.Delay").floatValue = 0;
                RecycleArgs.FindPropertyRelative("Rotation.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
                RecycleArgs.FindPropertyRelative("Rotation.CurveName").stringValue = "";
                RecycleArgs.FindPropertyRelative("Rotation.Ease").enumValueIndex = (int)EaseMode.OutQuart;
                RecycleArgs.FindPropertyRelative("Alpha.Duration").floatValue = 1;
                RecycleArgs.FindPropertyRelative("Alpha.Delay").floatValue = 0;
                RecycleArgs.FindPropertyRelative("Alpha.Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
                RecycleArgs.FindPropertyRelative("Alpha.CurveName").stringValue = "";
                RecycleArgs.FindPropertyRelative("Alpha.Ease").enumValueIndex = (int)EaseMode.OutQuart;
                RecycleArgs_MotionAnimateEndState.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
                RecycleArgs.serializedObject.ApplyModifiedProperties();
            }
        }

        /// <summary>
        /// 主要在编辑器模式下用于获取所有锚点下的物体进行集控 - 屏幕级别
        /// </summary>
        /// <returns></returns>
        public HudElementInfo AnchorElementInfosGet_Screen()
        {
            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();
            XHud_Module_Element[] trans = mgr.HudCanvas_ScreenAnchor.GetComponentsInChildren<XHud_Module_Element>();
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
            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();
            XHud_Module_Element[] trans = mgr.HudCanvas_World.GetComponentsInChildren<XHud_Module_Element>();
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
            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();
            bool sw = false;
            for (int i = 0; i < mgr.Hud_ElementLibrarys.Count; i++)
            {
                if (mgr.Hud_ElementLibrarys[i].LibraryName == LibName.stringValue)
                {
                    sw = mgr.Hud_ElementLibrarys[i].ElementLibrary_IsExist(name);
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
            IsSpawning = serializedObject.FindProperty("IsSpawning");
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
            Sequence_Spawn_Screen = serializedObject.FindProperty("Sequence_Spawn_Screen");
            Sequence_Spawn_World = serializedObject.FindProperty("Sequence_Spawn_World");
            Sequence_Despawn_Screen = serializedObject.FindProperty("Sequence_Despawn_Screen");
            Sequence_Despawn_World = serializedObject.FindProperty("Sequence_Despawn_World");
            ControlScreen = serializedObject.FindProperty("ControlScreen");
            ControlWorld = serializedObject.FindProperty("ControlWorld");
            FoldScreen = serializedObject.FindProperty("FoldScreen");
            FoldWorld = serializedObject.FindProperty("FoldWorld");
            ProtectedAction = serializedObject.FindProperty("ProtectedAction");
            inMotion = serializedObject.FindProperty("inMotion");
            CreateArgs_MotionAnimateEndState = CreateArgs.FindPropertyRelative("MotionAnimateEndState");
            RecycleArgs_MotionAnimateEndState = RecycleArgs.FindPropertyRelative("MotionAnimateEndState");
        }

        /// <summary>
        /// 动效库添加器
        /// </summary>
        public void OpenParameterSetter(HudElementMotionType Type)
        {
            Editor_XHud_LibrarySetTool_Motion window = EditorWindow.GetWindow<Editor_XHud_LibrarySetTool_Motion>(true);

            window.titleContent = new GUIContent("XHud 动效库采集器");
            Editor_XHud_GUI.CenterEditorWindow(new Vector2Int(600, 530), window);

            // 将要存入元素库的物体信息发送至窗口
            switch (Type)
            {
                case HudElementMotionType.Recycler:
                    Motion_Recycler rec = new Motion_Recycler();
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
            window.SetLibrarySetterMode(LibrarySetterMode.添加到库);
            window.SetButtonText("添加", "取消");
            window.SetTitle("XHud 动效库采集器");
            window.SetTarget_Hud_MotionLibrary(XHud_Dashboard.HudManagerGet().Hud_Motions);
            //window.ShowModal();
            window.Show();
        }

        /// <summary>
        /// 动效参数修改器
        /// </summary>
        public void OpenParameterModifier(XHud.XHud_LibraryArg_Motion motion, string motionName)
        {
            Editor_XHud_LibrarySetTool_Motion window = EditorWindow.GetWindow<Editor_XHud_LibrarySetTool_Motion>(true);

            window.titleContent = new GUIContent("生成器动效修改器");
            Editor_XHud_GUI.CenterEditorWindow(new Vector2Int(600, 530), window);

            window.SetElementMotion(motion.Crc, motion.Rec);
            window.SetLibrarySetterMode(LibrarySetterMode.修改生成器项参数);
            window.SetButtonText("完成");

            string hexcol = XHud_Utilitys.Color_To_HexColor(XHud_Dashboard.Theme_Primary, true);
            window.SetTitle($"<color={hexcol}>{motionName} </color>");
            window.ShowModal();
            //window.Show();
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
            SerializedProperty sp_CreateArgs = sp_Item.FindPropertyRelative("CreateArgs");
            SerializedProperty sp_RecycleArgs = sp_Item.FindPropertyRelative("RecycleArgs");
            SerializedProperty sp_creator_anchor = sp_CreateArgs.FindPropertyRelative("anchor");
            SerializedProperty sp_creator_Movement = sp_CreateArgs.FindPropertyRelative("Movement.Movement");
            SerializedProperty sp_creator_Movement_Distance = sp_CreateArgs.FindPropertyRelative("Movement.Distance");
            SerializedProperty sp_creator_Movement_Duration = sp_CreateArgs.FindPropertyRelative("Movement.Duration");
            SerializedProperty sp_creator_Movement_Ease = sp_CreateArgs.FindPropertyRelative("Movement.Ease");
            SerializedProperty sp_creator_Alpha_Duration = sp_CreateArgs.FindPropertyRelative("Alpha.Duration");
            SerializedProperty sp_creator_Alpha_Ease = sp_CreateArgs.FindPropertyRelative("Alpha.Ease");
            SerializedProperty sp_recycle_Movement = sp_RecycleArgs.FindPropertyRelative("Movement.Movement");
            SerializedProperty sp_recycle_Movement_Distance = sp_RecycleArgs.FindPropertyRelative("Movement.Distance");
            SerializedProperty sp_recycle_Movement_Duration = sp_RecycleArgs.FindPropertyRelative("Movement.Duration");
            SerializedProperty sp_recycle_Movement_Ease = sp_RecycleArgs.FindPropertyRelative("Movement.Ease");
            SerializedProperty sp_recycle_Alpha_Duration = sp_RecycleArgs.FindPropertyRelative("Alpha.Duration");
            SerializedProperty sp_recycle_Alpha_Ease = sp_RecycleArgs.FindPropertyRelative("Alpha.Ease");
            SerializedProperty sp_isFold = sp_Item.FindPropertyRelative("isFold");
            SerializedProperty sp_isEnabled = sp_Item.FindPropertyRelative("isEnabled");

            sp_creator_Movement.enumValueIndex = (int)HudMotion_Movement.S_从下至上;
            sp_creator_Movement_Distance.floatValue = 100;
            sp_creator_Movement_Duration.floatValue = 1;
            sp_creator_Movement_Ease.enumValueIndex = (int)EaseMode.OutQuart;
            sp_creator_Alpha_Duration.floatValue = 1;
            sp_creator_Alpha_Ease.enumValueIndex = (int)EaseMode.InOutQuart;

            sp_recycle_Movement.enumValueIndex = (int)HudMotion_Movement.D_从上至下;
            sp_recycle_Movement_Distance.floatValue = 100;
            sp_recycle_Movement_Duration.floatValue = 1;
            sp_recycle_Movement_Ease.enumValueIndex = (int)EaseMode.InOutQuart;
            sp_recycle_Alpha_Duration.floatValue = 1;
            sp_recycle_Alpha_Ease.enumValueIndex = (int)EaseMode.OutQuart;

            sp_creator_anchor.enumValueIndex = (int)item.CreateArgs.anchor;

            sp_SpawnName.stringValue = item.SpawnName;
            sp_Indicator.stringValue = "Indicator_" + item.SpawnName;
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
            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();
            for (int i = 0; i < mgr.Hud_ElementLibrarys.Count; i++)
            {
                if (mgr.Hud_ElementLibrarys[i].LibraryName == LibName.stringValue)
                {
                    XHud_Module_Element ele = mgr.Hud_ElementLibrarys[i].ElementsLibrary_GetTargetElement(item.SpawnName);
                    XHud_Module_Element Element = (XHud_Module_Element)PrefabUtility.InstantiatePrefab(ele);
                    RectTransform Root = mgr.hm_ScreenElement_GetAnchored_RectTransform(item.CreateArgs.anchor);
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
            SerializedProperty sp_Delay_Spawn = sp_Item.FindPropertyRelative("Delay_Spawn");
            SerializedProperty sp_Position = sp_Item.FindPropertyRelative("Position");
            SerializedProperty sp_Euler = sp_Item.FindPropertyRelative("Euler");
            SerializedProperty sp_Offset = sp_Item.FindPropertyRelative("Offset");
            SerializedProperty sp_Scale = sp_Item.FindPropertyRelative("Scale");
            SerializedProperty sp_Size = sp_Item.FindPropertyRelative("Size");
            SerializedProperty sp_AutoIn = sp_Item.FindPropertyRelative("AutoIn");
            SerializedProperty sp_UseSpawnerMotion = sp_Item.FindPropertyRelative("UseSpawnerMotion");
            SerializedProperty sp_CreateArgs = sp_Item.FindPropertyRelative("CreateArgs");
            SerializedProperty sp_RecycleArgs = sp_Item.FindPropertyRelative("RecycleArgs");
            SerializedProperty sp_creator_Movement = sp_CreateArgs.FindPropertyRelative("Movement.Movement");
            SerializedProperty sp_creator_Movement_Distance = sp_CreateArgs.FindPropertyRelative("Movement.Distance");
            SerializedProperty sp_creator_Movement_Duration = sp_CreateArgs.FindPropertyRelative("Movement.Duration");
            SerializedProperty sp_creator_Movement_Ease = sp_CreateArgs.FindPropertyRelative("Movement.Ease");
            SerializedProperty sp_creator_Alpha_Duration = sp_CreateArgs.FindPropertyRelative("Alpha.Duration");
            SerializedProperty sp_creator_Alpha_Ease = sp_CreateArgs.FindPropertyRelative("Alpha.Ease");
            SerializedProperty sp_recycle_Movement = sp_RecycleArgs.FindPropertyRelative("Movement.Movement");
            SerializedProperty sp_recycle_Movement_Distance = sp_RecycleArgs.FindPropertyRelative("Movement.Distance");
            SerializedProperty sp_recycle_Movement_Duration = sp_RecycleArgs.FindPropertyRelative("Movement.Duration");
            SerializedProperty sp_recycle_Movement_Ease = sp_RecycleArgs.FindPropertyRelative("Movement.Ease");
            SerializedProperty sp_recycle_Alpha_Duration = sp_RecycleArgs.FindPropertyRelative("Alpha.Duration");
            SerializedProperty sp_recycle_Alpha_Ease = sp_RecycleArgs.FindPropertyRelative("Alpha.Ease");
            SerializedProperty sp_isFold = sp_Item.FindPropertyRelative("isFold");
            SerializedProperty sp_isEnabled = sp_Item.FindPropertyRelative("isEnabled");

            sp_creator_Movement.enumValueIndex = (int)HudMotion_Movement.S_从下至上;
            sp_creator_Movement_Distance.floatValue = 100;
            sp_creator_Movement_Duration.floatValue = 1;
            sp_creator_Movement_Ease.enumValueIndex = (int)EaseMode.OutQuart;
            sp_creator_Alpha_Duration.floatValue = 1;
            sp_creator_Alpha_Ease.enumValueIndex = (int)EaseMode.InOutQuart;

            sp_recycle_Movement.enumValueIndex = (int)HudMotion_Movement.D_从上至下;
            sp_recycle_Movement_Distance.floatValue = 100;
            sp_recycle_Movement_Duration.floatValue = 1;
            sp_recycle_Movement_Ease.enumValueIndex = (int)EaseMode.InOutQuart;
            sp_recycle_Alpha_Duration.floatValue = 1;
            sp_recycle_Alpha_Ease.enumValueIndex = (int)EaseMode.OutQuart;

            sp_SpawnName.stringValue = item.SpawnName;
            sp_Indicator.stringValue = "Indicator_" + item.SpawnName;
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
            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();
            for (int i = 0; i < mgr.Hud_ElementLibrarys.Count; i++)
            {
                if (mgr.Hud_ElementLibrarys[i].LibraryName == LibName.stringValue)
                {
                    XHud_Module_Element ele = mgr.Hud_ElementLibrarys[i].ElementsLibrary_GetTargetElement(item.SpawnName);
                    XHud_Module_Element Element = (XHud_Module_Element)PrefabUtility.InstantiatePrefab(ele);
                    RectTransform Root = mgr.hm_GetAnchorRoot(XHudSpace.世界空间);
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
        }
        #endregion
    }
}