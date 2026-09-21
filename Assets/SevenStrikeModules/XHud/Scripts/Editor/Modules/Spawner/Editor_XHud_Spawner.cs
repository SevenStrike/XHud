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
    using SevenStrikeModules.XHud.Utilitys;
    using SevenStrikeModules.XTween;
    using UnityEditor;
    using UnityEngine;

    public class XHud_Arg_Spawner
    {
        public string libname;
        public string spawnname;
        public string spawnindicator;
        public string spawnerindicator;

        public Vector2 size;
        public Vector3 pivot;
        public Vector3 offset;
        public Vector3 scale;
        public XHudAnchor anchor;
        public Vector3 world_pos;
        public Vector3 world_ang;
        public Vector3 world_sca;
        public string referobjpath;

        public Motion_Creator CreateArgs;
        public string createParamName;
        public Motion_Recycler RecycleArgs;
        public string recycleParamName;

        public KeyCode key_create;
        public KeyCode key_recycle;

        public string crc_lib_name;
        public string rec_lib_name;

        public bool iscreating;
        public bool worldcreate;
        public bool visuallercreate;
        public bool manullycreate;
        public bool rmsenabled;

        public SpawnFunctionKey SpawnFunctionKey_Primary;
        public SpawnFunctionKey SpawnFunctionKey_Secondary;

        public float autocreate_hold;
        public float autocreate_interval;
        public bool autoin;
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(XHud_Spawner))]
    public class Editor_XHud_Spawner : Editor
    {
        #region 组件 / 列表
        private XHud_Spawner BaseScript;
        private XHud_Manager HudManager;
        #endregion

        /// <summary>
        /// 原始脚本参数显示开关
        /// </summary>
        private bool OriginalDisplay;

        #region 序列化属性
        private SerializedProperty recycle_fold_move, recycle_fold_rotate, recycle_fold_alpha, create_fold_move, create_fold_rotate, create_fold_alpha, sp_VisuallerCreate, sp_ManullyCreate, sp_WorldCreate, SpawnerRunning, sp_RMSEnabled, sp_AutoIn, SpawnName, SpawnIndicator, SpawnerIndicator, CreateArgs, RecycleArgs, CreateArgs_MotionAnimateEndState, RecycleArgs_MotionAnimateEndState, RMS_SelctedName, CreateParamName, RecycleParamName, LibName, ElementOffset, ElementScale, Key_Create, Key_Recycle, SpawnFunctionKey_Primary, SpawnFunctionKey_Secondary, Crc_Lib_Name, Rec_Lib_Name, ElementSize, WorldPosition, WorldRotation, WorldScale, ReferObject, ProtectedAction, sp_UseElementSelfMotion;
        #endregion

        private string PrefsKeyFold_Motion = "XHUD-LAYOUTSPAWN-FOLD-MOTION";

        #region 选项文字
        string[] stroptions_enabled = new string[2] { "关闭", "开启" };
        string[] stroptions_world = new string[2] { "平面", "世界" };
        string[] stroptions_protecte = new string[2] { "开放", "保护" };
        #endregion

        #region 图标
        private Texture2D icon_main, save_r, save_p, locate_r, locate_p, reset_r, reset_p;
        #endregion                                                                                   

        #region 批量化操作
        XHud_Spawner[] SelectedObjects;

        private void GetAllTargets()
        {
            if (targets.Length > 1)
            {
                SelectedObjects = new XHud_Spawner[targets.Length];
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    var t = targets[i];
                    SelectedObjects[i] = (XHud_Spawner)t;
                }
            }
            else
            {
                SelectedObjects = new XHud_Spawner[targets.Length];
                SelectedObjects[0] = (XHud_Spawner)target;
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

        private void OnEnable()
        {
            HudManager = XHud_Dashboard.HudManagerGet();

            BaseScript = (XHud_Spawner)target;

            icon_main = Editor_XHud_GUI.GetIcon("Icons_XHud_Spawner/icon_main");
            save_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Spawner/save_r");
            save_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Spawner/save_p");
            locate_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Spawner/locate_r");
            locate_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Spawner/locate_p");
            reset_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Spawner/reset_r");
            reset_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Spawner/reset_p");

            GetSerializeFields();

            GetAllTargets();

            #region 用于第一次加载脚本判断库名和元素名是否是空的
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
                    //判断元素名是否是空的
                    if (string.IsNullOrEmpty(SpawnName.stringValue))
                    {
                        //获取指定库名的元素库
                        XHud_Library_Element lib = HudManager.hm_ElementLibrary_GetTargetLibrary(LibName.stringValue);
                        //如果元素库不是空的
                        if (lib != null)
                        {
                            //获取指定库名的元素库的所有元素名称
                            string[] ItemNames = lib.ElementsLibrary_GetAllElementsNames();
                            //如果目标元素库不是空的就把首个元素名给他
                            if (ItemNames.Length > 0)
                            {
                                SpawnName.stringValue = ItemNames[0];
                                SpawnName.serializedObject.ApplyModifiedProperties();

                                SpawnIndicator.stringValue = "Indicator_" + SpawnName.stringValue;
                                SpawnIndicator.serializedObject.ApplyModifiedProperties();
                            }
                        }
                    }
                }
            }
            #endregion
        }

        private void OnDisable()
        {

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            Editor_XHud_GUI.Gui_Layout_Banner(icon_main, HudFilled.实体, HudColor.深空灰, "XHud - 元素生成器", Color.white);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            #region 生成器选项
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 0, "生成器选项", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            #region 指定库名和元素名
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
                    Editor_XHud_GUI.Gui_Layout_Space(10);

                    string[] libsname = HudManager.hm_ElementLibrary_GetAllLibraryNames();

                    if (!HudManager.hm_ElementLibrary_IsExist(LibName.stringValue))
                    {
                        LibName.stringValue = HudManager.hm_ElementLibrary_GetFirstLibrary().LibraryName;
                    }

                    Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_Spawner>("元素库", libsname, ref LibName, HudFilled.实体, 120, 22, SelectedObjects, (comps) => { }, (res) =>
                    {
                        //如果切换库名后之前指定的目标元素不存在则使用切换后的库的首个元素名称来指定元素
                        if (!HudManager.hm_ElementLibrary_GetTargetLibrary(res).ElementLibrary_IsExist(SpawnName.stringValue))
                        {
                            SpawnName.stringValue = HudManager.hm_ElementLibrary_GetTargetLibrary(res).ElementLibrary[0].Name;
                            SpawnName.serializedObject.ApplyModifiedProperties();
                        }
                    });

                    string[] ele_names = HudManager.hm_ElementLibrary_GetTargetLibrary(LibName.stringValue).ElementsLibrary_GetAllElementsNames();

                    Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_Spawner>("元素", ele_names, ref SpawnName, HudFilled.实体, 120, 22, SelectedObjects, (comps) => { }, (res) =>
                    {
                        SpawnIndicator.stringValue = "Indicator_" + res;
                        SpawnIndicator.serializedObject.ApplyModifiedProperties();
                    });
                    #endregion
                }
            }
            #endregion

            Editor_XHud_GUI.Gui_Layout_Seperator(1, XHud_Dashboard.Theme_SeperateLine);

            #region 生成状态     
            Editor_XHud_GUI.StatuDisplayer_text(null, 12, new Vector2(0, 7), "生成状态", 12, SpawnerRunning.boolValue ? "生成中" : "待命中", SpawnerRunning.boolValue ? XHud_Dashboard.Theme_Primary : Color.gray, 11, false);
            #endregion

            Editor_XHud_GUI.Gui_Layout_Seperator(1, XHud_Dashboard.Theme_SeperateLine);
            Editor_XHud_GUI.Gui_Layout_Space(5);

            #region 根据脚本可见性自动创建与回收
            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Spawner>("可见性创建与回收", stroptions_enabled, ref sp_VisuallerCreate, HudFilled.无, HudFilled.实体, Color.white * 0.9f, 120, 22, SelectedObjects);
            #endregion

            #region 手动创建与回收
            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Spawner>("手动创建与回收", stroptions_enabled, ref sp_ManullyCreate, HudFilled.无, HudFilled.实体, Color.white * 0.9f, 120, 22, SelectedObjects);
            #endregion

            #region 生成空间
            EditorGUI.BeginChangeCheck();
            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Spawner>("生成的空间模式", stroptions_world, ref sp_WorldCreate, HudFilled.无, HudFilled.实体, Color.white * 0.9f, 120, 22, SelectedObjects);
            if (EditorGUI.EndChangeCheck())
            {
                if (sp_WorldCreate.intValue == 0)
                    CreateArgs.FindPropertyRelative("anchor").enumValueIndex = 5;
                else
                    CreateArgs.FindPropertyRelative("anchor").enumValueIndex = 10;
            }
            #endregion

            #region 生成时使用布局模式
            if (!sp_WorldCreate.boolValue)
            {
                Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Spawner>("R M S 布局模式", stroptions_enabled, ref sp_RMSEnabled, HudFilled.无, HudFilled.实体, Color.white * 0.9f, 120, 22, SelectedObjects);
            }
            #endregion

            #region 安全保护机制
            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Spawner>("安全保护机制", stroptions_protecte, ref ProtectedAction, HudFilled.无, HudFilled.实体, Color.white * 0.9f, 120, 22, SelectedObjects);
            #endregion

            #region 自动激活元素进入动作
            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Spawner>("元素自动播放", stroptions_enabled, ref sp_AutoIn, HudFilled.无, HudFilled.实体, Color.white * 0.9f, 120, 22, SelectedObjects);
            #endregion

            #region 使用元素自身动效
            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Spawner>("使用元素自身动效", stroptions_enabled, ref sp_UseElementSelfMotion, HudFilled.无, HudFilled.实体, Color.white * 0.9f, 120, 22, SelectedObjects);
            #endregion

            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 生成器参数
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 0, "生成器参数", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            #region 生成器标识
            Editor_XHud_GUI.Gui_Layout_Property_Field("生成器标识", SpawnerIndicator);
            #endregion

            if (sp_ManullyCreate.boolValue)
            {
                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Seperator(1, XHud_Dashboard.Theme_SeperateLine);
                Editor_XHud_GUI.Gui_Layout_Space(10);

                Editor_XHud_GUI.Gui_Layout_Property_Field("主要辅助按键", SpawnFunctionKey_Primary, 90);
                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Property_Field("次级辅助按键", SpawnFunctionKey_Secondary, 90);
                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Property_Field("生成按键", Key_Create, 90);
                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Property_Field("回收按键", Key_Recycle, 90);
                Editor_XHud_GUI.Gui_Layout_Space(10);
            }

            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 生成偏移
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 3, "生成偏移", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            EditorGUILayout.HelpBox("如果不希望指定生成的元素尺寸，请保持尺寸值为0", MessageType.Info);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("尺寸", ElementSize);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("偏移", ElementOffset);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            if (!sp_WorldCreate.boolValue)
            {
                Editor_XHud_GUI.Gui_Layout_Property_Field("缩放", ElementScale);

                Editor_XHud_GUI.Gui_Layout_Space(5);

                SerializedProperty sp_anchor_type = CreateArgs.FindPropertyRelative("anchor");
                Editor_XHud_GUI.Gui_Layout_Property_Field("锚点", sp_anchor_type);
            }
            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region RMS 布局信息
            if (!sp_WorldCreate.boolValue && sp_RMSEnabled.boolValue)
            {
                Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.阴影灰, 5, "RMS 布局信息", Color.white);
                Editor_XHud_GUI.Gui_Layout_Space(5);
                ScreenResolutionNode[] nodes = HudManager.hm_RMS_GetResolutionNodes();
                if (nodes.Length > 0)
                {
                    string[] nodesName = HudManager.hm_RMS_GetResolutionNodeNames();
                    Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_Spawner>("RMS 方案", nodesName, ref RMS_SelctedName, HudFilled.实体, 100, 22, SelectedObjects);
                }
                else
                {
                    Editor_XHud_GUI.Gui_Layout_Labelfield("暂未在管理器中配置 R M S 方案列表", HudFilled.无, HudColor.无, Editor_XHud_GUI.GetColor(HudColor.阴影灰), TextAnchor.MiddleCenter);
                }
                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Vertical_End();
            }
            #endregion

            #region 生成坐标
            if (sp_WorldCreate.boolValue)
            {
                Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 3, "生成坐标", XHud_Dashboard.Theme_Primary);
                Editor_XHud_GUI.Gui_Layout_Space(10);

                Editor_XHud_GUI.Gui_Layout_Property_Field("位置", WorldPosition);

                Editor_XHud_GUI.Gui_Layout_Space(5);

                Editor_XHud_GUI.Gui_Layout_Property_Field("角度", WorldRotation);

                Editor_XHud_GUI.Gui_Layout_Space(5);

                Editor_XHud_GUI.Gui_Layout_Property_Field("缩放", WorldScale);

                Editor_XHud_GUI.Gui_Layout_Space(10);
                Editor_XHud_GUI.Gui_Layout_Vertical_End();
            }
            #endregion

            #region 参考坐标
            if (sp_WorldCreate.intValue == 1)
            {

                Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 3, "参考坐标物体", XHud_Dashboard.Theme_Primary);
                Editor_XHud_GUI.Gui_Layout_Space(5);
                if (ReferObject.objectReferenceValue == null)
                    EditorGUILayout.HelpBox("如果不指定参考坐标物体，则默认使用生成坐标作为元素生成条件", MessageType.Warning);

                Editor_XHud_GUI.Gui_Layout_Space(5);

                Editor_XHud_GUI.Gui_Layout_Property_Field("参考物体", ReferObject);

                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Vertical_End();
            }
            #endregion

            #region 动效参数
            bool sw_motion = xHud_FunctionGroup("动效参数", 5, HudFilled.纯色边框, HudColor.亮白, XHud_Dashboard.Theme_Primary, XHud_Dashboard.Theme_Primary, Color.gray, new RectOffset(0, 0, 0, 0), new Vector2(20, 0), PrefsKeyFold_Motion, null);
            if (sw_motion)
            {
                string hexcol = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary, true);

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
                        Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_Spawner>("生成", motnames, ref Crc_Lib_Name, HudFilled.实体, 120, 22, SelectedObjects);
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

                        if (Editor_XHud_GUI.Gui_Layout_Button(14, "保存", save_r, save_p, 2))
                        {
                            OpenParameterSetter(HudElementMotionType.Creator);
                            return;
                        }

                        Editor_XHud_GUI.Gui_Layout_Space(10);

                        if (Editor_XHud_GUI.Gui_Layout_Button(14, "定位", locate_r, locate_p, 2))
                        {
                            if (!HudManager.Hud_Motions.ElementMotion_IsExist(Crc_Lib_Name.stringValue))
                                return;
                            Editor_XHud_MenuItemsAction_OpenLibrary.open_elementmotion();
                            HudManager.Hud_Motions.ElementMotionLibrary_Location(Crc_Lib_Name.stringValue);
                            return;
                        }

                        Editor_XHud_GUI.Gui_Layout_Space(10);

                        if (Editor_XHud_GUI.Gui_Layout_Button(14, "重置", reset_r, reset_p, 2))
                        {
                            string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 元素生成器消息", "重置动效参数", "确定要将动效参数重置吗？您将丢失当前的动效参数！", "重置", "暂不", 0);
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
                                Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_Spawner>("曲线样式", names, ref sp_CurveName, HudFilled.实体, 120, 22, SelectedObjects, (comps) => { }, (res) =>
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
                                Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_Spawner>("曲线样式", names, ref sp_CurveName, HudFilled.实体, 120, 22, SelectedObjects, (comps) => { }, (res) =>
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
                                Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_Spawner>("曲线样式", names, ref sp_CurveName, HudFilled.实体, 120, 22, SelectedObjects, (comps) => { }, (res) =>
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
                                Editor_XHud_GUI.Open(XHud_DialogType.警告, $"XHud - 元素生成器消息", "设定动画结束时机", $"当前位移方式为 <color={hexcol}> A_无运动 </color>，将发生<color={hexcol}> 动效动画无法正常结束 </color>从而导致<color={hexcol}> 相应的事件和动作委托 </color>不能正确触发的情况 </color>！请将<color={hexcol}> 位移方式 </color>改为<color={hexcol}> 非无运动方式 </color>！", "明白", 0);
                                CreateArgs_MotionAnimateEndState.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
                            }
                            break;
                        case MotionAnimateEndState.以_旋转为准:
                            HudMotion_Rotation r = (HudMotion_Rotation)CreateArgs.FindPropertyRelative("Rotation.Rotation").enumValueIndex;
                            if (r == HudMotion_Rotation.A_无旋转)
                            {
                                Editor_XHud_GUI.Open(XHud_DialogType.警告, $"XHud - 元素生成器消息", "设定动画结束时机", $"当前旋转方式为 <color={hexcol}> A_无旋转 </color>，将发生<color={hexcol}> 动效动画无法正常结束</color>从而导致<color={hexcol}>相应的事件和动作委托</color>不能正确触发的情况 </color>！请将<color={hexcol}> 旋转方式 </color>改为<color={hexcol}> 非无旋转方式 </color>！", "明白", 0);
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
                        Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_Spawner>("回收", motnames, ref Rec_Lib_Name, HudFilled.实体, 120, 22, SelectedObjects);
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

                        if (Editor_XHud_GUI.Gui_Layout_Button(14, "保存", save_r, save_p, 2))
                        {
                            OpenParameterSetter(HudElementMotionType.Recycler);
                            return;
                        }

                        Editor_XHud_GUI.Gui_Layout_Space(10);

                        if (Editor_XHud_GUI.Gui_Layout_Button(14, "定位", locate_r, locate_p, 2))
                        {
                            if (!HudManager.Hud_Motions.ElementMotion_IsExist(Rec_Lib_Name.stringValue, HudElementMotionType.Recycler))
                                return;
                            Editor_XHud_MenuItemsAction_OpenLibrary.open_elementmotion();
                            HudManager.Hud_Motions.ElementMotionLibrary_Location(Rec_Lib_Name.stringValue);
                            return;
                        }

                        Editor_XHud_GUI.Gui_Layout_Space(10);

                        if (Editor_XHud_GUI.Gui_Layout_Button(14, "重置", reset_r, reset_p, 2))
                        {
                            string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 元素生成器消息", "重置动效参数", "确定要将动效参数重置吗？您将丢失当前的动效参数！", "重置", "暂不", 0);
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
                                Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_Spawner>("曲线样式", names, ref sp_CurveName, HudFilled.实体, 120, 22, SelectedObjects, (comps) => { }, (res) =>
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
                                Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_Spawner>("曲线样式", names, ref sp_CurveName, HudFilled.实体, 120, 22, SelectedObjects, (comps) => { }, (res) =>
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
                                Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_Spawner>("曲线样式", names, ref sp_CurveName, HudFilled.实体, 120, 22, SelectedObjects, (comps) => { }, (res) =>
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
                                Editor_XHud_GUI.Open(XHud_DialogType.警告, $"XHud - 元素生成器消息", "设定动画结束时机", $"当前位移方式为 <color={hexcol}> A_无运动 </color>，将发生<color={hexcol}> 动效动画无法正常结束</color>从而导致<color={hexcol}> 相应的事件和动作委托 </color>不能正确触发的情况 </color>！请将<color={hexcol}> 位移方式 </color>改为<color={hexcol}> 非无运动方式 </color>！", "明白", 0);
                                RecycleArgs_MotionAnimateEndState.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
                            }
                            break;
                        case MotionAnimateEndState.以_旋转为准:
                            HudMotion_Rotation r = (HudMotion_Rotation)RecycleArgs.FindPropertyRelative("Rotation.Rotation").enumValueIndex;
                            if (r == HudMotion_Rotation.A_无旋转)
                            {
                                Editor_XHud_GUI.Open(XHud_DialogType.警告, $"XHud - 元素生成器消息", "设定动画结束时机", $"当前旋转方式为 <color={hexcol}> A_无旋转 </color>，将发生<color={hexcol}> 动效动画无法正常结束</color>从而导致<color={hexcol}> 相应的事件和动作委托 </color>不能正确触发的情况 </color>！请将<color={hexcol}> 旋转方式 </color>改为<color={hexcol}> 非无旋转方式 </color>！", "明白", 0);
                                RecycleArgs_MotionAnimateEndState.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
                            }
                            break;
                    }
                }
            }

            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                #region 获取属性
                SerializedProperty sp_LibName = serializedObject.FindProperty("LibName");
                SerializedProperty sp_SpawnName = serializedObject.FindProperty("SpawnName");
                SerializedProperty sp_SpawnIndicator = serializedObject.FindProperty("SpawnIndicator");
                SerializedProperty sp_SpawnerIndicator = serializedObject.FindProperty("SpawnerIndicator");
                SerializedProperty sp_CreateArgs = serializedObject.FindProperty("CreateArgs");
                SerializedProperty sp_RecycleArgs = serializedObject.FindProperty("RecycleArgs");
                SerializedProperty sp_CreateParamName = serializedObject.FindProperty("CreateParamName");
                SerializedProperty sp_RecycleParamName = serializedObject.FindProperty("RecycleParamName");
                SerializedProperty sp_ElementOffset = serializedObject.FindProperty("ElementOffset");
                SerializedProperty sp_ElementScale = serializedObject.FindProperty("ElementScale");
                SerializedProperty sp_world_Element_Pos = serializedObject.FindProperty("WorldPosition");
                SerializedProperty sp_world_Element_Rot = serializedObject.FindProperty("WorldRotation");
                SerializedProperty sp_world_Element_Sca = serializedObject.FindProperty("WorldScale");
                SerializedProperty sp_world_Element_ReferObject = serializedObject.FindProperty("ReferObject");
                SerializedProperty sp_keycreate = serializedObject.FindProperty("Key_Create");
                SerializedProperty sp_keyrecycle = serializedObject.FindProperty("Key_Recycle");
                SerializedProperty sp_AutoCreateHold = serializedObject.FindProperty("AutoCreateHold");
                SerializedProperty sp_AutoCreateInterval = serializedObject.FindProperty("AutoCreateInterval");
                SerializedProperty sp_Crc_Lib_Name = serializedObject.FindProperty("Crc_Lib_Name");
                SerializedProperty sp_Rec_Lib_Name = serializedObject.FindProperty("Rec_Lib_Name");
                SerializedProperty sp_spawner_running = serializedObject.FindProperty("m_SpawnerRunning");
                SerializedProperty sp_ManullyCreate = serializedObject.FindProperty("ManullyCreate");
                SerializedProperty sp_VisuallerCreate = serializedObject.FindProperty("VisuallerCreate");
                SerializedProperty sp_WorldCreate = serializedObject.FindProperty("WorldCreate");
                SerializedProperty sp_RMSEnabled = serializedObject.FindProperty("RMSEnabled");
                SerializedProperty sp_AutoIn = serializedObject.FindProperty("AutoIn");
                SerializedProperty sp_SpawnFunctionKey_Primary = serializedObject.FindProperty("SpawnFunctionKey_Primary");
                SerializedProperty sp_SpawnFunctionKey_Secondary = serializedObject.FindProperty("SpawnFunctionKey_Secondary");
                SerializedProperty sp_ElementSize = serializedObject.FindProperty("ElementSize");
                #endregion

                // 创建右键菜单
                GenericMenu menu = new GenericMenu();
                if (!IsMultiSelected())
                {
                    menu.AddItem(new GUIContent("C (拷贝脚本参数)"), false, () =>
                    {
                        XHud_Arg_Spawner hsp = new XHud_Arg_Spawner();

                        hsp.libname = sp_LibName.stringValue;
                        hsp.spawnname = sp_SpawnName.stringValue;
                        hsp.spawnindicator = sp_SpawnIndicator.stringValue;
                        hsp.spawnerindicator = sp_SpawnerIndicator.stringValue;

                        hsp.SpawnFunctionKey_Primary = (SpawnFunctionKey)sp_SpawnFunctionKey_Primary.enumValueIndex;
                        hsp.SpawnFunctionKey_Secondary = (SpawnFunctionKey)sp_SpawnFunctionKey_Secondary.enumValueIndex;

                        #region CreateArgs
                        Motion_Creator crc = new Motion_Creator();
                        crc.anchor = (XHudAnchor)sp_CreateArgs.FindPropertyRelative("anchor").enumValueIndex;

                        MotionNode_Movement hm_m = new MotionNode_Movement();
                        hm_m.Movement = (HudMotion_Movement)sp_CreateArgs.FindPropertyRelative("Movement.Movement").enumValueIndex;
                        hm_m.Distance = sp_CreateArgs.FindPropertyRelative("Movement.Distance").floatValue;
                        hm_m.Duration = sp_CreateArgs.FindPropertyRelative("Movement.Duration").floatValue;
                        hm_m.Delay = sp_CreateArgs.FindPropertyRelative("Movement.Delay").floatValue;
                        hm_m.Ease = (EaseMode)sp_CreateArgs.FindPropertyRelative("Movement.Ease").enumValueIndex;
                        hm_m.Curve = sp_CreateArgs.FindPropertyRelative("Movement.Curve").animationCurveValue;
                        hm_m.CurveName = sp_CreateArgs.FindPropertyRelative("Movement.CurveName").stringValue;

                        MotionNode_Rotation hm_r = new MotionNode_Rotation();
                        hm_r.Rotation = (HudMotion_Rotation)sp_CreateArgs.FindPropertyRelative("Rotation.Rotation").enumValueIndex;
                        hm_r.Degree = sp_CreateArgs.FindPropertyRelative("Rotation.Degree").floatValue;
                        hm_r.Duration = sp_CreateArgs.FindPropertyRelative("Rotation.Duration").floatValue;
                        hm_r.Delay = sp_CreateArgs.FindPropertyRelative("Rotation.Delay").floatValue;
                        hm_r.Ease = (EaseMode)sp_CreateArgs.FindPropertyRelative("Rotation.Ease").enumValueIndex;
                        hm_r.Curve = sp_CreateArgs.FindPropertyRelative("Rotation.Curve").animationCurveValue;
                        hm_r.CurveName = sp_CreateArgs.FindPropertyRelative("Rotation.CurveName").stringValue;

                        MotionNode_Alpha hm_a = new MotionNode_Alpha();
                        hm_a.Duration = sp_CreateArgs.FindPropertyRelative("Alpha.Duration").floatValue;
                        hm_a.Curve = sp_CreateArgs.FindPropertyRelative("Alpha.Curve").animationCurveValue;
                        hm_a.CurveName = sp_CreateArgs.FindPropertyRelative("Alpha.CurveName").stringValue;
                        hm_a.Ease = (EaseMode)sp_CreateArgs.FindPropertyRelative("Alpha.Ease").enumValueIndex;
                        hm_a.Delay = sp_CreateArgs.FindPropertyRelative("Alpha.Delay").floatValue;

                        crc.Movement = hm_m;
                        crc.Rotation = hm_r;
                        crc.Alpha = hm_a;
                        hsp.CreateArgs = crc;
                        #endregion

                        #region RecycleArgs
                        Motion_Recycler rec = new Motion_Recycler();

                        MotionNode_Movement rec_hm_m = new MotionNode_Movement();
                        rec_hm_m.Movement = (HudMotion_Movement)sp_RecycleArgs.FindPropertyRelative("Movement.Movement").enumValueIndex;
                        rec_hm_m.Distance = sp_RecycleArgs.FindPropertyRelative("Movement.Distance").floatValue;
                        rec_hm_m.Duration = sp_RecycleArgs.FindPropertyRelative("Movement.Duration").floatValue;
                        rec_hm_m.Delay = sp_RecycleArgs.FindPropertyRelative("Movement.Delay").floatValue;
                        rec_hm_m.Ease = (EaseMode)sp_RecycleArgs.FindPropertyRelative("Movement.Ease").enumValueIndex;
                        rec_hm_m.Curve = sp_RecycleArgs.FindPropertyRelative("Movement.Curve").animationCurveValue;
                        rec_hm_m.CurveName = sp_RecycleArgs.FindPropertyRelative("Movement.CurveName").stringValue;

                        MotionNode_Rotation rec_hm_r = new MotionNode_Rotation();
                        rec_hm_r.Rotation = (HudMotion_Rotation)sp_RecycleArgs.FindPropertyRelative("Rotation.Rotation").enumValueIndex;
                        rec_hm_r.Degree = sp_RecycleArgs.FindPropertyRelative("Rotation.Degree").floatValue;
                        rec_hm_r.Duration = sp_RecycleArgs.FindPropertyRelative("Rotation.Duration").floatValue;
                        rec_hm_r.Delay = sp_RecycleArgs.FindPropertyRelative("Rotation.Delay").floatValue;
                        rec_hm_r.Ease = (EaseMode)sp_RecycleArgs.FindPropertyRelative("Rotation.Ease").enumValueIndex;
                        rec_hm_r.Curve = sp_RecycleArgs.FindPropertyRelative("Rotation.Curve").animationCurveValue;
                        rec_hm_r.CurveName = sp_RecycleArgs.FindPropertyRelative("Rotation.CurveName").stringValue;

                        MotionNode_Alpha rec_hm_a = new MotionNode_Alpha();
                        rec_hm_a.Duration = sp_RecycleArgs.FindPropertyRelative("Alpha.Duration").floatValue;
                        rec_hm_a.Curve = sp_RecycleArgs.FindPropertyRelative("Alpha.Curve").animationCurveValue;
                        rec_hm_a.CurveName = sp_RecycleArgs.FindPropertyRelative("Alpha.CurveName").stringValue;
                        rec_hm_a.Ease = (EaseMode)sp_RecycleArgs.FindPropertyRelative("Alpha.Ease").enumValueIndex;
                        rec_hm_a.Delay = sp_RecycleArgs.FindPropertyRelative("Alpha.Delay").floatValue;

                        rec.Movement = rec_hm_m;
                        rec.Rotation = rec_hm_r;
                        rec.Alpha = rec_hm_a;
                        hsp.RecycleArgs = rec;
                        #endregion

                        hsp.createParamName = sp_CreateParamName.stringValue;
                        hsp.recycleParamName = sp_RecycleParamName.stringValue;

                        hsp.size = sp_ElementSize.vector2Value;
                        hsp.offset = sp_ElementOffset.vector3Value;
                        hsp.scale = sp_ElementScale.vector3Value;

                        hsp.world_pos = sp_world_Element_Pos.vector3Value;
                        hsp.world_ang = sp_world_Element_Rot.vector3Value;
                        hsp.world_sca = sp_world_Element_Sca.vector3Value;

                        hsp.key_create = (KeyCode)sp_keycreate.enumValueIndex;
                        hsp.key_recycle = (KeyCode)sp_keyrecycle.enumValueIndex;

                        hsp.autocreate_hold = sp_AutoCreateHold.floatValue;
                        hsp.autocreate_interval = sp_AutoCreateInterval.floatValue;

                        hsp.iscreating = sp_spawner_running.boolValue;
                        hsp.visuallercreate = sp_VisuallerCreate.boolValue;
                        hsp.manullycreate = sp_ManullyCreate.boolValue;
                        hsp.rmsenabled = sp_RMSEnabled.boolValue;
                        hsp.worldcreate = sp_WorldCreate.boolValue;
                        hsp.autoin = sp_AutoIn.boolValue;

                        hsp.crc_lib_name = sp_Crc_Lib_Name.stringValue;
                        hsp.rec_lib_name = sp_Rec_Lib_Name.stringValue;

                        Editor_XHud_GUI.EditorData_Set_With_String("XED_HudSpawner_Get_ScriptInfo", JsonUtility.ToJson(hsp));
                    });
                }
                menu.AddItem(new GUIContent("V (粘贴脚本参数)"), false, () =>
                {
                    string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 元素生成器消息", "粘贴脚本参数", "确认是否要粘贴拷贝的脚本参数？如果粘贴将覆盖现有的脚本参数！", "暂不", "粘贴", 0);
                    if (res == "暂不")
                        return;

                    string json = Editor_XHud_GUI.EditorData_Get_With_String("XED_HudSpawner_Get_ScriptInfo");
                    XHud_Arg_Spawner hsp = JsonUtility.FromJson<XHud_Arg_Spawner>(json);
                    if (IsMultiSelected())
                    {
                        for (int i = 0; i < SelectedObjects.Length; i++)
                        {
                            SerializedObject so_pre = new SerializedObject(SelectedObjects[i]);
                            SerializedProperty m_sp_LibName = so_pre.FindProperty("LibName");
                            SerializedProperty m_sp_SpawnName = so_pre.FindProperty("SpawnName");
                            SerializedProperty m_sp_SpawnIndicator = so_pre.FindProperty("SpawnIndicator");
                            SerializedProperty m_sp_SpawnerIndicator = so_pre.FindProperty("SpawnerIndicator");
                            SerializedProperty m_sp_CreateArgs = so_pre.FindProperty("CreateArgs");
                            SerializedProperty m_sp_RecycleArgs = so_pre.FindProperty("RecycleArgs");
                            SerializedProperty m_sp_CreateParamName = so_pre.FindProperty("CreateParamName");
                            SerializedProperty m_sp_RecycleParamName = so_pre.FindProperty("RecycleParamName");
                            SerializedProperty m_sp_ElementSize = so_pre.FindProperty("ElementSize");
                            SerializedProperty m_sp_ElementOffset = so_pre.FindProperty("ElementOffset");
                            SerializedProperty m_sp_ElementScale = so_pre.FindProperty("ElementScale");
                            SerializedProperty m_sp_world_Element_Pos = so_pre.FindProperty("WorldPosition");
                            SerializedProperty m_sp_world_Element_Rot = so_pre.FindProperty("WorldRotation");
                            SerializedProperty m_sp_world_Element_Sca = so_pre.FindProperty("WorldScale");
                            SerializedProperty m_sp_world_Element_ReferObject = so_pre.FindProperty("ReferObject");
                            SerializedProperty m_sp_keycreate = so_pre.FindProperty("Key_Create");
                            SerializedProperty m_sp_keyrecycle = so_pre.FindProperty("Key_Recycle");
                            SerializedProperty m_sp_AutoCreateHold = so_pre.FindProperty("AutoCreateHold");
                            SerializedProperty m_sp_AutoCreateInterval = so_pre.FindProperty("AutoCreateInterval");
                            SerializedProperty m_sp_Crc_Lib_Name = so_pre.FindProperty("Crc_Lib_Name");
                            SerializedProperty m_sp_Rec_Lib_Name = so_pre.FindProperty("Rec_Lib_Name");
                            SerializedProperty m_sp_spawner_running = so_pre.FindProperty("m_SpawnerRunning");
                            SerializedProperty m_sp_ManullyCreate = so_pre.FindProperty("ManullyCreate");
                            SerializedProperty m_sp_VisuallerCreate = so_pre.FindProperty("VisuallerCreate");
                            SerializedProperty m_sp_WorldCreate = so_pre.FindProperty("WorldCreate");
                            SerializedProperty m_sp_RMSEnabled = so_pre.FindProperty("RMSEnabled");
                            SerializedProperty m_sp_AutoIn = so_pre.FindProperty("AutoIn");
                            SerializedProperty m_SpawnFunctionKey_Primary = so_pre.FindProperty("SpawnFunctionKey_Primary");
                            SerializedProperty m_SpawnFunctionKey_Secondary = so_pre.FindProperty("SpawnFunctionKey_Secondary");

                            so_pre.Update();

                            m_sp_LibName.stringValue = hsp.libname;
                            m_sp_SpawnName.stringValue = hsp.spawnname;
                            m_sp_SpawnIndicator.stringValue = hsp.spawnindicator;
                            m_sp_SpawnerIndicator.stringValue = hsp.spawnerindicator;

                            #region CreateArgs
                            m_sp_CreateArgs.FindPropertyRelative("anchor").enumValueIndex = (int)hsp.CreateArgs.anchor;

                            m_sp_CreateArgs.FindPropertyRelative("Movement.Movement").enumValueIndex = (int)hsp.CreateArgs.Movement.Movement;
                            m_sp_CreateArgs.FindPropertyRelative("Movement.Distance").floatValue = hsp.CreateArgs.Movement.Distance;
                            m_sp_CreateArgs.FindPropertyRelative("Movement.Duration").floatValue = hsp.CreateArgs.Movement.Duration;
                            m_sp_CreateArgs.FindPropertyRelative("Movement.Delay").floatValue = hsp.CreateArgs.Movement.Delay;
                            m_sp_CreateArgs.FindPropertyRelative("Movement.Ease").enumValueIndex = (int)hsp.CreateArgs.Movement.Ease;
                            m_sp_CreateArgs.FindPropertyRelative("Movement.Curve").animationCurveValue = hsp.CreateArgs.Movement.Curve;
                            m_sp_CreateArgs.FindPropertyRelative("Movement.CurveName").stringValue = hsp.CreateArgs.Movement.CurveName;

                            m_sp_CreateArgs.FindPropertyRelative("Rotation.Rotation").enumValueIndex = (int)hsp.CreateArgs.Rotation.Rotation;
                            m_sp_CreateArgs.FindPropertyRelative("Rotation.Degree").floatValue = hsp.CreateArgs.Rotation.Degree;
                            m_sp_CreateArgs.FindPropertyRelative("Rotation.Duration").floatValue = hsp.CreateArgs.Rotation.Duration;
                            m_sp_CreateArgs.FindPropertyRelative("Rotation.Delay").floatValue = hsp.CreateArgs.Rotation.Delay;
                            m_sp_CreateArgs.FindPropertyRelative("Rotation.Ease").enumValueIndex = (int)hsp.CreateArgs.Rotation.Ease;
                            m_sp_CreateArgs.FindPropertyRelative("Rotation.Curve").animationCurveValue = hsp.CreateArgs.Rotation.Curve;
                            m_sp_CreateArgs.FindPropertyRelative("Rotation.CurveName").stringValue = hsp.CreateArgs.Rotation.CurveName;

                            m_sp_CreateArgs.FindPropertyRelative("Alpha.Duration").floatValue = hsp.CreateArgs.Alpha.Duration;
                            m_sp_CreateArgs.FindPropertyRelative("Alpha.Curve").animationCurveValue = hsp.CreateArgs.Alpha.Curve;
                            m_sp_CreateArgs.FindPropertyRelative("Alpha.CurveName").stringValue = hsp.CreateArgs.Alpha.CurveName;
                            m_sp_CreateArgs.FindPropertyRelative("Alpha.Ease").enumValueIndex = (int)hsp.CreateArgs.Alpha.Ease;
                            m_sp_CreateArgs.FindPropertyRelative("Alpha.Delay").floatValue = hsp.CreateArgs.Alpha.Delay;
                            #endregion

                            #region RecycleArgs
                            m_sp_RecycleArgs.FindPropertyRelative("Movement.Movement").enumValueIndex = (int)hsp.RecycleArgs.Movement.Movement;
                            m_sp_RecycleArgs.FindPropertyRelative("Movement.Distance").floatValue = hsp.RecycleArgs.Movement.Distance;
                            m_sp_RecycleArgs.FindPropertyRelative("Movement.Duration").floatValue = hsp.RecycleArgs.Movement.Duration;
                            m_sp_RecycleArgs.FindPropertyRelative("Movement.Delay").floatValue = hsp.RecycleArgs.Movement.Delay;
                            m_sp_RecycleArgs.FindPropertyRelative("Movement.Ease").enumValueIndex = (int)hsp.RecycleArgs.Movement.Ease;
                            m_sp_RecycleArgs.FindPropertyRelative("Movement.Curve").animationCurveValue = hsp.RecycleArgs.Movement.Curve;
                            m_sp_RecycleArgs.FindPropertyRelative("Movement.CurveName").stringValue = hsp.RecycleArgs.Movement.CurveName;

                            m_sp_RecycleArgs.FindPropertyRelative("Rotation.Rotation").enumValueIndex = (int)hsp.RecycleArgs.Rotation.Rotation;
                            m_sp_RecycleArgs.FindPropertyRelative("Rotation.Degree").floatValue = hsp.RecycleArgs.Rotation.Degree;
                            m_sp_RecycleArgs.FindPropertyRelative("Rotation.Duration").floatValue = hsp.RecycleArgs.Rotation.Duration;
                            m_sp_RecycleArgs.FindPropertyRelative("Rotation.Delay").floatValue = hsp.RecycleArgs.Rotation.Delay;
                            m_sp_RecycleArgs.FindPropertyRelative("Rotation.Ease").enumValueIndex = (int)hsp.RecycleArgs.Rotation.Ease;
                            m_sp_RecycleArgs.FindPropertyRelative("Rotation.Curve").animationCurveValue = hsp.RecycleArgs.Rotation.Curve;
                            m_sp_RecycleArgs.FindPropertyRelative("Rotation.CurveName").stringValue = hsp.RecycleArgs.Rotation.CurveName;

                            m_sp_RecycleArgs.FindPropertyRelative("Alpha.Duration").floatValue = hsp.RecycleArgs.Alpha.Duration;
                            m_sp_RecycleArgs.FindPropertyRelative("Alpha.Curve").animationCurveValue = hsp.RecycleArgs.Alpha.Curve;
                            m_sp_RecycleArgs.FindPropertyRelative("Alpha.CurveName").stringValue = hsp.RecycleArgs.Alpha.CurveName;
                            m_sp_RecycleArgs.FindPropertyRelative("Alpha.Ease").enumValueIndex = (int)hsp.RecycleArgs.Alpha.Ease;
                            m_sp_RecycleArgs.FindPropertyRelative("Alpha.Delay").floatValue = hsp.RecycleArgs.Alpha.Delay;
                            #endregion

                            m_SpawnFunctionKey_Primary.enumValueIndex = (int)hsp.SpawnFunctionKey_Primary;
                            m_SpawnFunctionKey_Secondary.enumValueIndex = (int)hsp.SpawnFunctionKey_Secondary;

                            m_sp_CreateParamName.stringValue = hsp.createParamName;
                            m_sp_RecycleParamName.stringValue = hsp.recycleParamName;

                            m_sp_ElementSize.vector2Value = hsp.size;
                            m_sp_ElementOffset.vector3Value = hsp.offset;
                            m_sp_ElementScale.vector3Value = hsp.scale;

                            m_sp_world_Element_Pos.vector3Value = hsp.world_pos;
                            m_sp_world_Element_Rot.vector3Value = hsp.world_ang;
                            m_sp_world_Element_Sca.vector3Value = hsp.world_sca;

                            m_sp_keycreate.enumValueIndex = (int)hsp.key_create;
                            m_sp_keyrecycle.enumValueIndex = (int)hsp.key_recycle;

                            m_sp_AutoCreateHold.floatValue = hsp.autocreate_hold;
                            m_sp_AutoCreateInterval.floatValue = hsp.autocreate_interval;

                            m_sp_Crc_Lib_Name.stringValue = hsp.crc_lib_name;
                            m_sp_Rec_Lib_Name.stringValue = hsp.rec_lib_name;

                            m_sp_spawner_running.boolValue = hsp.iscreating;
                            m_sp_ManullyCreate.boolValue = hsp.manullycreate;
                            m_sp_VisuallerCreate.boolValue = hsp.visuallercreate;
                            m_sp_WorldCreate.boolValue = hsp.worldcreate;
                            m_sp_RMSEnabled.boolValue = hsp.rmsenabled;
                            m_sp_AutoIn.boolValue = hsp.autoin;

                            so_pre.ApplyModifiedProperties();
                        }
                    }
                    else
                    {
                        sp_LibName.stringValue = hsp.libname;
                        sp_SpawnName.stringValue = hsp.spawnname;
                        sp_SpawnIndicator.stringValue = hsp.spawnindicator;
                        sp_SpawnerIndicator.stringValue = hsp.spawnerindicator;

                        #region CreateArgs
                        sp_CreateArgs.FindPropertyRelative("anchor").enumValueIndex = (int)hsp.CreateArgs.anchor;

                        sp_CreateArgs.FindPropertyRelative("Movement.Movement").enumValueIndex = (int)hsp.CreateArgs.Movement.Movement;
                        sp_CreateArgs.FindPropertyRelative("Movement.Distance").floatValue = hsp.CreateArgs.Movement.Distance;
                        sp_CreateArgs.FindPropertyRelative("Movement.Duration").floatValue = hsp.CreateArgs.Movement.Duration;
                        sp_CreateArgs.FindPropertyRelative("Movement.Delay").floatValue = hsp.CreateArgs.Movement.Delay;
                        sp_CreateArgs.FindPropertyRelative("Movement.Ease").enumValueIndex = (int)hsp.CreateArgs.Movement.Ease;
                        sp_CreateArgs.FindPropertyRelative("Movement.Curve").animationCurveValue = hsp.CreateArgs.Movement.Curve;
                        sp_CreateArgs.FindPropertyRelative("Movement.CurveName").stringValue = hsp.CreateArgs.Movement.CurveName;

                        sp_CreateArgs.FindPropertyRelative("Rotation.Rotation").enumValueIndex = (int)hsp.CreateArgs.Rotation.Rotation;
                        sp_CreateArgs.FindPropertyRelative("Rotation.Degree").floatValue = hsp.CreateArgs.Rotation.Degree;
                        sp_CreateArgs.FindPropertyRelative("Rotation.Duration").floatValue = hsp.CreateArgs.Rotation.Duration;
                        sp_CreateArgs.FindPropertyRelative("Rotation.Delay").floatValue = hsp.CreateArgs.Rotation.Delay;
                        sp_CreateArgs.FindPropertyRelative("Rotation.Ease").enumValueIndex = (int)hsp.CreateArgs.Rotation.Ease;
                        sp_CreateArgs.FindPropertyRelative("Rotation.Curve").animationCurveValue = hsp.CreateArgs.Rotation.Curve;
                        sp_CreateArgs.FindPropertyRelative("Rotation.CurveName").stringValue = hsp.CreateArgs.Rotation.CurveName;

                        sp_CreateArgs.FindPropertyRelative("Alpha.Duration").floatValue = hsp.CreateArgs.Alpha.Duration;
                        sp_CreateArgs.FindPropertyRelative("Alpha.Curve").animationCurveValue = hsp.CreateArgs.Alpha.Curve;
                        sp_CreateArgs.FindPropertyRelative("Alpha.CurveName").stringValue = hsp.CreateArgs.Alpha.CurveName;
                        sp_CreateArgs.FindPropertyRelative("Alpha.Ease").enumValueIndex = (int)hsp.CreateArgs.Alpha.Ease;
                        sp_CreateArgs.FindPropertyRelative("Alpha.Delay").floatValue = hsp.CreateArgs.Alpha.Delay;
                        #endregion

                        #region RecycleArgs
                        sp_RecycleArgs.FindPropertyRelative("Movement.Movement").enumValueIndex = (int)hsp.RecycleArgs.Movement.Movement;
                        sp_RecycleArgs.FindPropertyRelative("Movement.Distance").floatValue = hsp.RecycleArgs.Movement.Distance;
                        sp_RecycleArgs.FindPropertyRelative("Movement.Duration").floatValue = hsp.RecycleArgs.Movement.Duration;
                        sp_RecycleArgs.FindPropertyRelative("Movement.Delay").floatValue = hsp.RecycleArgs.Movement.Delay;
                        sp_RecycleArgs.FindPropertyRelative("Movement.Ease").enumValueIndex = (int)hsp.RecycleArgs.Movement.Ease;
                        sp_RecycleArgs.FindPropertyRelative("Movement.Curve").animationCurveValue = hsp.RecycleArgs.Movement.Curve;
                        sp_RecycleArgs.FindPropertyRelative("Movement.CurveName").stringValue = hsp.RecycleArgs.Movement.CurveName;

                        sp_RecycleArgs.FindPropertyRelative("Rotation.Rotation").enumValueIndex = (int)hsp.RecycleArgs.Rotation.Rotation;
                        sp_RecycleArgs.FindPropertyRelative("Rotation.Degree").floatValue = hsp.RecycleArgs.Rotation.Degree;
                        sp_RecycleArgs.FindPropertyRelative("Rotation.Duration").floatValue = hsp.RecycleArgs.Rotation.Duration;
                        sp_RecycleArgs.FindPropertyRelative("Rotation.Delay").floatValue = hsp.RecycleArgs.Rotation.Delay;
                        sp_RecycleArgs.FindPropertyRelative("Rotation.Ease").enumValueIndex = (int)hsp.RecycleArgs.Rotation.Ease;
                        sp_RecycleArgs.FindPropertyRelative("Rotation.Curve").animationCurveValue = hsp.RecycleArgs.Rotation.Curve;
                        sp_RecycleArgs.FindPropertyRelative("Rotation.CurveName").stringValue = hsp.RecycleArgs.Rotation.CurveName;

                        sp_RecycleArgs.FindPropertyRelative("Alpha.Duration").floatValue = hsp.RecycleArgs.Alpha.Duration;
                        sp_RecycleArgs.FindPropertyRelative("Alpha.Curve").animationCurveValue = hsp.RecycleArgs.Alpha.Curve;
                        sp_RecycleArgs.FindPropertyRelative("Alpha.CurveName").stringValue = hsp.RecycleArgs.Alpha.CurveName;
                        sp_RecycleArgs.FindPropertyRelative("Alpha.Ease").enumValueIndex = (int)hsp.RecycleArgs.Alpha.Ease;
                        sp_RecycleArgs.FindPropertyRelative("Alpha.Delay").floatValue = hsp.RecycleArgs.Alpha.Delay;
                        #endregion

                        sp_SpawnFunctionKey_Primary.enumValueIndex = (int)hsp.SpawnFunctionKey_Primary;
                        sp_SpawnFunctionKey_Secondary.enumValueIndex = (int)hsp.SpawnFunctionKey_Secondary;

                        sp_CreateParamName.stringValue = hsp.createParamName;
                        sp_RecycleParamName.stringValue = hsp.recycleParamName;

                        sp_ElementSize.vector2Value = hsp.size;
                        sp_ElementOffset.vector3Value = hsp.offset;
                        sp_ElementScale.vector3Value = hsp.scale;

                        sp_world_Element_Pos.vector3Value = hsp.world_pos;
                        sp_world_Element_Rot.vector3Value = hsp.world_ang;
                        sp_world_Element_Sca.vector3Value = hsp.world_sca;

                        sp_keycreate.enumValueIndex = (int)hsp.key_create;
                        sp_keyrecycle.enumValueIndex = (int)hsp.key_recycle;

                        sp_AutoCreateHold.floatValue = hsp.autocreate_hold;
                        sp_AutoCreateInterval.floatValue = hsp.autocreate_interval;

                        sp_Crc_Lib_Name.stringValue = hsp.crc_lib_name;
                        sp_Rec_Lib_Name.stringValue = hsp.rec_lib_name;

                        sp_spawner_running.boolValue = hsp.iscreating;
                        sp_ManullyCreate.boolValue = hsp.manullycreate;
                        sp_VisuallerCreate.boolValue = hsp.visuallercreate;
                        sp_WorldCreate.boolValue = hsp.worldcreate;
                        sp_RMSEnabled.boolValue = hsp.rmsenabled;
                        sp_AutoIn.boolValue = hsp.autoin;

                        serializedObject.ApplyModifiedProperties();
                    }
                });
                menu.AddSeparator("");
                menu.AddDisabledItem(new GUIContent("动效快速操作"));
                menu.AddItem(new GUIContent("E (复制动效)"), false, () =>
                {
                    string res = Editor_XHud_GUI.Open(XHud_DialogType.修改, "XHud - 元素生成器消息", "复制动效", "请选择动效参数复制模式！", "取消", "生成", "回收", 0);
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
                    Editor_XHud_GUI.Open(XHud_DialogType.确认, "XHud - 元素生成器消息", "复制动效", $"已复制 \" {mode} \" 到 XHudEditorData (XED) ！", "明白", 0);
                });
                menu.AddItem(new GUIContent("R (粘贴动效)"), false, () =>
                {
                    string buffer = Editor_XHud_GUI.EditorData_Get_With_String("XED_Copy_MotionArgs");
                    if (buffer.Contains("anchor"))//粘贴生成参数
                    {
                        string res = Editor_XHud_GUI.Open(XHud_DialogType.修改, "XHud - 元素生成器消息", "粘贴动效", "检测到动效参数类型为： \"生成动效\"，确定要使用这个参数吗？", "确定", "暂不", 0);
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

                        Editor_XHud_GUI.Open(XHud_DialogType.确认, "XHud - 元素生成器消息", "粘贴动效", "已更新 \"生成\" 动效参数!", "明白", 0);
                    }
                    else//粘贴回收参数
                    {
                        string res = Editor_XHud_GUI.Open(XHud_DialogType.修改, "XHud - 元素生成器消息", "粘贴动效", "检测到动效参数类型为： \"回收动效\"，确定要使用这个参数吗？", "确定", "暂不", 0);
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

                        Editor_XHud_GUI.Open(XHud_DialogType.确认, "XHud - 元素生成器消息", "粘贴动效", "已更新 \"回收\" 动效参数!", "明白", 0);
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
            if (state == "c")
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

                Rec_Lib_Name.stringValue = null;
                Rec_Lib_Name.serializedObject.ApplyModifiedProperties();
            }
        }

        /// <summary>
        /// 获取序列化变量
        /// </summary>
        private void GetSerializeFields()
        {
            recycle_fold_move = serializedObject.FindProperty("recycle_fold_move");
            recycle_fold_rotate = serializedObject.FindProperty("recycle_fold_rotate");
            recycle_fold_alpha = serializedObject.FindProperty("recycle_fold_alpha");
            ElementSize = serializedObject.FindProperty("ElementSize");
            create_fold_move = serializedObject.FindProperty("create_fold_move");
            create_fold_rotate = serializedObject.FindProperty("create_fold_rotate");
            create_fold_alpha = serializedObject.FindProperty("create_fold_alpha");
            sp_VisuallerCreate = serializedObject.FindProperty("VisuallerCreate");
            sp_ManullyCreate = serializedObject.FindProperty("ManullyCreate");
            sp_WorldCreate = serializedObject.FindProperty("WorldCreate");
            SpawnerRunning = serializedObject.FindProperty("m_SpawnerRunning");
            sp_RMSEnabled = serializedObject.FindProperty("RMSEnabled");
            sp_AutoIn = serializedObject.FindProperty("AutoIn");
            SpawnName = serializedObject.FindProperty("SpawnName");
            SpawnIndicator = serializedObject.FindProperty("SpawnIndicator");
            SpawnerIndicator = serializedObject.FindProperty("SpawnerIndicator");
            CreateArgs = serializedObject.FindProperty("CreateArgs");
            CreateArgs_MotionAnimateEndState = CreateArgs.FindPropertyRelative("MotionAnimateEndState");
            RecycleArgs = serializedObject.FindProperty("RecycleArgs");
            RecycleArgs_MotionAnimateEndState = RecycleArgs.FindPropertyRelative("MotionAnimateEndState");
            RMS_SelctedName = serializedObject.FindProperty("RMS_SelctedName");
            CreateParamName = serializedObject.FindProperty("CreateParamName");
            RecycleParamName = serializedObject.FindProperty("RecycleParamName");
            LibName = serializedObject.FindProperty("LibName");
            ElementOffset = serializedObject.FindProperty("ElementOffset");
            ElementScale = serializedObject.FindProperty("ElementScale");
            Key_Create = serializedObject.FindProperty("Key_Create");
            Key_Recycle = serializedObject.FindProperty("Key_Recycle");
            Crc_Lib_Name = serializedObject.FindProperty("Crc_Lib_Name");
            Rec_Lib_Name = serializedObject.FindProperty("Rec_Lib_Name");
            WorldPosition = serializedObject.FindProperty("WorldPosition");
            WorldRotation = serializedObject.FindProperty("WorldRotation");
            WorldScale = serializedObject.FindProperty("WorldScale");
            ReferObject = serializedObject.FindProperty("ReferObject");
            SpawnFunctionKey_Primary = serializedObject.FindProperty("SpawnFunctionKey_Primary");
            SpawnFunctionKey_Secondary = serializedObject.FindProperty("SpawnFunctionKey_Secondary");
            sp_UseElementSelfMotion = serializedObject.FindProperty("UseElementSelfMotion");
            ProtectedAction = serializedObject.FindProperty("ProtectedAction");
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
    }
}