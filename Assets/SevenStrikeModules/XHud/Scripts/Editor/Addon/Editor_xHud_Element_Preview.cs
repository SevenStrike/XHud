namespace SevenStrikeModules.XHud
{
    using Newtonsoft.Json;
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.GuiLib;
    using SevenStrikeModules.XHud.Utilitys;
    using SevenStrikeModules.XTween;
    using UnityEditor;
    using UnityEngine;

    public class XHud_Element_PreviewArgs
    {
        public float DelayWithIn;
        public float DelayWithOut;
        public KeyCode key_Element_In;
        public KeyCode key_Element_Out;

        public bool HideWithStart;

        public Motion_Creator CreateArgs;
        public Motion_Recycler RecycleArgs;

        public KeyCode key_create;
        public KeyCode key_recycle;
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(XHud_Element_Preview))]
    public class Editor_XHud_Element_Preview : Editor
    {
        #region 组件
        private XHud_Element_Preview BaseScript;
        #endregion

        public bool BasicVars;

        #region 序列化属性
        SerializedProperty IsEnable, DebugState, DurationScaler, OriginalPosition, create_fold_move, create_fold_rotate, create_fold_alpha, recycle_fold_move, recycle_fold_rotate, recycle_fold_alpha, key_Element_In, key_Element_Out, HideWithStart, Crc_Lib_Name, Rec_Lib_Name, CreateArgs, RecycleArgs, DelayWithIn, DelayWithOut, RMS_Enabled, RMS_Name, HudElement, CreateArgs_MotionAnimateEndState, RecycleArgs_MotionAnimateEndState, previewIsRunning;
        #endregion

        #region 图标
        private Texture2D
            icon_main,
            save_r,
            save_p,
            locate_r,
            locate_p,
            rms_move,
            rms_rotate,
            rms_anchor,
            rms_anchor_center,
            rms_scale,
            reset_r,
            reset_p;
        #endregion

        #region 选项文字
        string[] stroptions_enabled = new string[2] { "关闭", "开启" };
        string[] stroptions_debug = new string[2] { "关闭", "调试" };
        #endregion

        #region 批量化操作
        private XHud_Element_Preview[] SelectedObjects;

        private void GetAllTargets()
        {
            if (targets.Length > 1)
            {
                SelectedObjects = new XHud_Element_Preview[targets.Length];
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    var t = targets[i];
                    SelectedObjects[i] = (XHud_Element_Preview)t;
                }
            }
            else
            {
                SelectedObjects = new XHud_Element_Preview[targets.Length];
                SelectedObjects[0] = (XHud_Element_Preview)target;
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
            BaseScript = (XHud_Element_Preview)target;

            #region 获取序列化属性
            HudElement = serializedObject.FindProperty("HudElement");
            key_Element_In = serializedObject.FindProperty("key_Element_In");
            key_Element_Out = serializedObject.FindProperty("key_Element_Out");
            CreateArgs = serializedObject.FindProperty("CreateArgs");
            RecycleArgs = serializedObject.FindProperty("RecycleArgs");
            HideWithStart = serializedObject.FindProperty("HideWithStart");
            Crc_Lib_Name = serializedObject.FindProperty("Crc_Lib_Name");
            Rec_Lib_Name = serializedObject.FindProperty("Rec_Lib_Name");
            DelayWithIn = serializedObject.FindProperty("DelayWithIn");
            DelayWithOut = serializedObject.FindProperty("DelayWithOut");
            IsEnable = serializedObject.FindProperty("IsEnable");
            DebugState = serializedObject.FindProperty("DebugState");
            create_fold_move = serializedObject.FindProperty("create_fold_move");
            create_fold_rotate = serializedObject.FindProperty("create_fold_rotate");
            create_fold_alpha = serializedObject.FindProperty("create_fold_alpha");
            recycle_fold_move = serializedObject.FindProperty("recycle_fold_move");
            recycle_fold_rotate = serializedObject.FindProperty("recycle_fold_rotate");
            recycle_fold_alpha = serializedObject.FindProperty("recycle_fold_alpha");
            OriginalPosition = serializedObject.FindProperty("OriginalPosition");
            DurationScaler = serializedObject.FindProperty("DurationScaler");
            RMS_Enabled = serializedObject.FindProperty("RMS_Enabled");
            RMS_Name = serializedObject.FindProperty("RMS_Name");
            CreateArgs_MotionAnimateEndState = CreateArgs.FindPropertyRelative("MotionAnimateEndState");
            RecycleArgs_MotionAnimateEndState = RecycleArgs.FindPropertyRelative("MotionAnimateEndState");
            previewIsRunning = serializedObject.FindProperty("previewIsRunning");
            #endregion

            #region 获取图标
            icon_main = Editor_XHud_GUI.GetIcon("Icons_XHud_ElementPreview/icon_main");
            save_r = Editor_XHud_GUI.GetIcon("Icons_XHud_ElementPreview/save_r");
            save_p = Editor_XHud_GUI.GetIcon("Icons_XHud_ElementPreview/save_p");
            locate_r = Editor_XHud_GUI.GetIcon("Icons_XHud_ElementPreview/locate_r");
            locate_p = Editor_XHud_GUI.GetIcon("Icons_XHud_ElementPreview/locate_p");
            rms_move = Editor_XHud_GUI.GetIcon("Icons_XHud_ElementPreview/rms_move");
            rms_rotate = Editor_XHud_GUI.GetIcon("Icons_XHud_ElementPreview/rms_rotate");
            rms_anchor = Editor_XHud_GUI.GetIcon("Icons_XHud_ElementPreview/rms_anchor");
            rms_scale = Editor_XHud_GUI.GetIcon("Icons_XHud_ElementPreview/rms_scale");
            rms_anchor_center = Editor_XHud_GUI.GetIcon("Icons_XHud_ElementPreview/rms_anchor_center");
            reset_r = Editor_XHud_GUI.GetIcon("Icons_XHud_ElementPreview/reset_r");
            reset_p = Editor_XHud_GUI.GetIcon("Icons_XHud_ElementPreview/reset_p");
            #endregion

            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

            if (HudElement.objectReferenceValue == null)
            {
                HudElement.objectReferenceValue = BaseScript.GetComponent<XHud_Module_Element>();
                HudElement.serializedObject.ApplyModifiedProperties();
            }
            else
            {
                if (HudElement.objectReferenceValue.GetInstanceID() != BaseScript.GetInstanceID())
                {
                    HudElement.objectReferenceValue = BaseScript.GetComponent<XHud_Module_Element>();
                    HudElement.serializedObject.ApplyModifiedProperties();
                }
            }

            GetAllTargets();

            CheckRmsNameValid();
        }

        private void OnDisable()
        {

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            Editor_XHud_GUI.Gui_Layout_Banner(icon_main, HudFilled.实体, HudColor.深空灰, "XHud - 元素预览器", Color.white);

            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

            Editor_XHud_GUI.Gui_Layout_Space(5);

            #region 基础参数
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 0, "基础参数", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            Editor_XHud_GUI.Gui_Layout_Property_Field("元素组件", HudElement);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("延时进入", DelayWithIn);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("延时退出", DelayWithOut);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("元素进入", key_Element_In);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("元素退出", key_Element_Out);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("缩放耗时", DurationScaler);

            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 选项
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 0, "选项", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Element_Preview>("状态调试", stroptions_debug, ref DebugState, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Element_Preview>("预览开关", stroptions_enabled, ref IsEnable, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Element_Preview>("开始时隐藏", stroptions_enabled, ref HideWithStart, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            EditorGUI.BeginChangeCheck();
            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Element_Preview>("R M S 模式", stroptions_enabled, ref RMS_Enabled, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
            if (EditorGUI.EndChangeCheck())
            {
                CheckRmsNameValid();
            }

            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            Editor_XHud_GUI.Gui_Layout_Space(5);

            #region 状态
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "状态", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(5);

            #region 生成状态     
            Editor_XHud_GUI.StatuDisplayer_text(null, 12, new Vector2(0, 7), "动效状态", 12, previewIsRunning.boolValue ? "动效中" : "待命中", previewIsRunning.boolValue ? XHud_Dashboard.Theme_Primary : Color.gray, 11, false);
            #endregion

            Editor_XHud_GUI.Gui_Layout_Seperator(1, XHud_Dashboard.Theme_SeperateLine);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            #region 原始位置     
            Editor_XHud_GUI.Gui_Layout_Property_Field("原始位置", OriginalPosition);
            #endregion

            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region RMS布局信息
            if (RMS_Enabled.boolValue)
            {
                Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "RMS 布局信息", XHud_Dashboard.Theme_Primary);
                Editor_XHud_GUI.Gui_Layout_Space(5);

                #region 分辨率方案列表
                ScreenResolutionNode[] nodes = mgr.hm_RMS_GetResolutionNodes();
                if (nodes.Length > 0)
                {
                    string[] nodesName = mgr.hm_RMS_GetResolutionNodeNames();
                    Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_Element_Preview>("RMS 方案", nodesName, ref RMS_Name, HudFilled.实体, 100, 22, SelectedObjects);

                    Element_RMS_LayoutData info = BaseScript.HudElement.elelemt_RMS_Get(RMS_Name.stringValue);

                    if (info != null)
                    {
                        int titlesize = 12;
                        int contensize = 10;
                        float iconsize = 12;
                        Color valuecol = Editor_XHud_GUI.GetColor(HudColor.阴影灰);

                        Editor_XHud_GUI.StatuDisplayer_text(rms_anchor, iconsize, new Vector2(0, 7), "锚点", titlesize, info.Anchor.ToString(), valuecol, contensize);
                        Editor_XHud_GUI.StatuDisplayer_text(rms_anchor, iconsize, new Vector2(0, 7), "最小锚点", titlesize, info.AnchorMin.ToString(), valuecol, contensize);
                        Editor_XHud_GUI.StatuDisplayer_text(rms_anchor, iconsize, new Vector2(0, 7), "最大锚点", titlesize, info.AnchorMax.ToString(), valuecol, contensize);
                        Editor_XHud_GUI.StatuDisplayer_text(rms_move, iconsize, new Vector2(0, 7), "位置", titlesize, info.Position.ToString(), valuecol, contensize);
                        Editor_XHud_GUI.StatuDisplayer_text(rms_rotate, iconsize, new Vector2(0, 7), "角度", titlesize, info.Euler.ToString(), valuecol, contensize);
                        Editor_XHud_GUI.StatuDisplayer_text(rms_scale, iconsize, new Vector2(0, 7), "缩放", titlesize, info.Scale.ToString(), valuecol, contensize);
                        Editor_XHud_GUI.StatuDisplayer_text(rms_anchor_center, iconsize, new Vector2(0, 7), "轴心", titlesize, info.Pivot.ToString(), valuecol, contensize);
                    }
                    else
                    {
                        Editor_XHud_GUI.Gui_Layout_Labelfield("暂未找到布局信息", HudFilled.无, HudColor.无, Editor_XHud_GUI.GetColor(HudColor.阴影灰), TextAnchor.MiddleCenter);
                    }
                }
                else
                {
                    Editor_XHud_GUI.Gui_Layout_Labelfield("暂未在管理器中配置 R M S 布局列表", HudFilled.无, HudColor.无, Editor_XHud_GUI.GetColor(HudColor.阴影灰), TextAnchor.MiddleCenter);
                }
                #endregion

                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Vertical_End();
            }
            #endregion

            #region 元素动效参数
            string hexcol = XHud_Utilitys.Color_To_HexColor(XHud_Dashboard.Theme_Primary, true);

            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 3, "元素动效参数", XHud_Dashboard.Theme_Primary);
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
                    Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_Element_Preview>("生成", motnames, ref Crc_Lib_Name, HudFilled.实体, 120, 22, SelectedObjects);
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
                        string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 元素预览器消息", "重置动效参数", "确定要将动效参数重置吗？您将丢失当前的动效参数！", "重置", "暂不", 0);
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
                            Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_Element_Preview>("曲线样式", names, ref sp_CurveName, HudFilled.实体, 120, 22, SelectedObjects, (comps) => { }, (res) =>
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
                            Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_Element_Preview>("曲线样式", names, ref sp_CurveName, HudFilled.实体, 120, 22, SelectedObjects, (comps) => { }, (res) =>
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
                            Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_Element_Preview>("曲线样式", names, ref sp_CurveName, HudFilled.实体, 120, 22, SelectedObjects, (comps) => { }, (res) =>
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
                            Editor_XHud_GUI.Open(XHud_DialogType.警告, $"XHud - 元素预览器消息", "设定动画结束时机", $"当前位移方式为 <color={hexcol}> A_无运动 </color>，将发生<color={hexcol}> 动效动画无法正常结束</color>从而导致<color={hexcol}> 相应的事件和动作委托 </color>不能正确触发的情况 </color>！请将<color={hexcol}> 位移方式 </color>改为<color={hexcol}> 非无运动方式 </color>！", "明白", 0);
                            CreateArgs_MotionAnimateEndState.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
                        }
                        break;
                    case MotionAnimateEndState.以_旋转为准:
                        HudMotion_Rotation r = (HudMotion_Rotation)CreateArgs.FindPropertyRelative("Rotation.Rotation").enumValueIndex;
                        if (r == HudMotion_Rotation.A_无旋转)
                        {
                            Editor_XHud_GUI.Open(XHud_DialogType.警告, $"XHud - 元素预览器消息", "设定动画结束时机", $"当前旋转方式为 <color={hexcol}> A_无旋转 </color>，将发生<color={hexcol}> 动效动画无法正常结束</color>从而导致<color={hexcol}> 相应的事件和动作委托 </color>不能正确触发的情况 </color>！请将<color={hexcol}> 旋转方式 </color>改为<color={hexcol}> 非无旋转方式 </color>！", "明白", 0);
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
                    Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_Element_Preview>("回收", motnames, ref Rec_Lib_Name, HudFilled.实体, 120, 22, SelectedObjects);
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
                        string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 元素预览器消息", "重置动效参数", "确定要将动效参数重置吗？您将丢失当前的动效参数！", "重置", "暂不", 0);
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
                            Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_Element_Preview>("曲线样式", names, ref sp_CurveName, HudFilled.实体, 120, 22, SelectedObjects, (comps) => { }, (res) =>
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
                            Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_Element_Preview>("曲线样式", names, ref sp_CurveName, HudFilled.实体, 120, 22, SelectedObjects, (comps) => { }, (res) =>
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
                            Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_Element_Preview>("曲线样式", names, ref sp_CurveName, HudFilled.实体, 120, 22, SelectedObjects, (comps) => { }, (res) =>
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
                            Editor_XHud_GUI.Open(XHud_DialogType.警告, $"XHud - 元素预览器消息", "设定动画结束时机", $"当前位移方式为 <color={hexcol}> A_无运动 </color>，将发生<color={hexcol}> 动效动画无法正常结束</color>从而导致<color={hexcol}> 相应的事件和动作委托 </color>不能正确触发的情况 </color>！请将<color={hexcol}> 位移方式 </color>改为<color={hexcol}> 非无运动方式 </color>！", "明白", 0);
                            RecycleArgs_MotionAnimateEndState.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
                        }
                        break;
                    case MotionAnimateEndState.以_旋转为准:
                        HudMotion_Rotation r = (HudMotion_Rotation)RecycleArgs.FindPropertyRelative("Rotation.Rotation").enumValueIndex;
                        if (r == HudMotion_Rotation.A_无旋转)
                        {
                            Editor_XHud_GUI.Open(XHud_DialogType.警告, $"XHud - 元素预览器消息", "设定动画结束时机", $"当前旋转方式为 <color={hexcol}> A_无旋转 </color>，将发生<color={hexcol}> 动效动画无法正常结束</color>从而导致<color={hexcol}> 相应的事件和动作委托 </color>不能正确触发的情况 </color>！请将<color={hexcol}> 旋转方式 </color>改为<color={hexcol}> 非无旋转方式 </color>！", "明白", 0);
                            RecycleArgs_MotionAnimateEndState.enumValueIndex = (int)MotionAnimateEndState.以_透明度为准;
                        }
                        break;
                }
            }

            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 菜单
            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                if (IsMultiSelected())
                    return;

                SerializedProperty sp_DelayWithIn = serializedObject.FindProperty("DelayWithIn");
                SerializedProperty sp_DelayWithOut = serializedObject.FindProperty("DelayWithOut");
                SerializedProperty sp_key_Element_In = serializedObject.FindProperty("key_Element_In");
                SerializedProperty sp_key_Element_Out = serializedObject.FindProperty("key_Element_Out");
                SerializedProperty sp_HideWithStart = serializedObject.FindProperty("HideWithStart");

                SerializedProperty sp_CreateArgs = serializedObject.FindProperty("CreateArgs");
                SerializedProperty sp_RecycleArgs = serializedObject.FindProperty("RecycleArgs");


                // 创建右键菜单
                GenericMenu menu = new GenericMenu();

                menu.AddItem(new GUIContent("C (拷贝脚本参数)"), false, () =>
                {
                    XHud_Element_PreviewArgs hpp = new XHud_Element_PreviewArgs();

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
                    hpp.CreateArgs = crc;
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
                    hpp.RecycleArgs = rec;
                    #endregion


                    hpp.DelayWithIn = sp_DelayWithIn.floatValue;
                    hpp.DelayWithOut = sp_DelayWithOut.floatValue;

                    hpp.key_create = (KeyCode)sp_key_Element_In.enumValueIndex;
                    hpp.key_recycle = (KeyCode)sp_key_Element_Out.enumValueIndex;

                    hpp.HideWithStart = sp_HideWithStart.boolValue;


                    string json = JsonUtility.ToJson(hpp);
                    GUIUtility.systemCopyBuffer = json;
                });
                menu.AddItem(new GUIContent("V (粘贴脚本参数)"), false, () =>
                {
                    XHud_Element_PreviewArgs hpp = JsonUtility.FromJson<XHud_Element_PreviewArgs>(GUIUtility.systemCopyBuffer);

                    sp_DelayWithIn.floatValue = hpp.DelayWithIn;
                    sp_DelayWithOut.floatValue = hpp.DelayWithOut;

                    sp_key_Element_In.enumValueIndex = (int)hpp.key_create;
                    sp_key_Element_Out.enumValueIndex = (int)hpp.key_recycle;

                    sp_HideWithStart.boolValue = hpp.HideWithStart;

                    #region CreateArgs
                    sp_CreateArgs.FindPropertyRelative("anchor").enumValueIndex = (int)hpp.CreateArgs.anchor;

                    sp_CreateArgs.FindPropertyRelative("Movement.Movement").enumValueIndex = (int)hpp.CreateArgs.Movement.Movement;
                    sp_CreateArgs.FindPropertyRelative("Movement.Distance").floatValue = hpp.CreateArgs.Movement.Distance;
                    sp_CreateArgs.FindPropertyRelative("Movement.Duration").floatValue = hpp.CreateArgs.Movement.Duration;
                    sp_CreateArgs.FindPropertyRelative("Movement.Delay").floatValue = hpp.CreateArgs.Movement.Delay;
                    sp_CreateArgs.FindPropertyRelative("Movement.Ease").enumValueIndex = (int)hpp.CreateArgs.Movement.Ease;
                    sp_CreateArgs.FindPropertyRelative("Movement.Curve").animationCurveValue = hpp.CreateArgs.Movement.Curve;
                    sp_CreateArgs.FindPropertyRelative("Movement.CurveName").stringValue = hpp.CreateArgs.Movement.CurveName;

                    sp_CreateArgs.FindPropertyRelative("Rotation.Rotation").enumValueIndex = (int)hpp.CreateArgs.Rotation.Rotation;
                    sp_CreateArgs.FindPropertyRelative("Rotation.Degree").floatValue = hpp.CreateArgs.Rotation.Degree;
                    sp_CreateArgs.FindPropertyRelative("Rotation.Duration").floatValue = hpp.CreateArgs.Rotation.Duration;
                    sp_CreateArgs.FindPropertyRelative("Rotation.Delay").floatValue = hpp.CreateArgs.Rotation.Delay;
                    sp_CreateArgs.FindPropertyRelative("Rotation.Ease").enumValueIndex = (int)hpp.CreateArgs.Rotation.Ease;
                    sp_CreateArgs.FindPropertyRelative("Rotation.Curve").animationCurveValue = hpp.CreateArgs.Rotation.Curve;
                    sp_CreateArgs.FindPropertyRelative("Rotation.CurveName").stringValue = hpp.CreateArgs.Rotation.CurveName;

                    sp_CreateArgs.FindPropertyRelative("Alpha.Duration").floatValue = hpp.CreateArgs.Alpha.Duration;
                    sp_CreateArgs.FindPropertyRelative("Alpha.Curve").animationCurveValue = hpp.CreateArgs.Alpha.Curve;
                    sp_CreateArgs.FindPropertyRelative("Alpha.CurveName").stringValue = hpp.CreateArgs.Alpha.CurveName;
                    sp_CreateArgs.FindPropertyRelative("Alpha.Ease").enumValueIndex = (int)hpp.CreateArgs.Alpha.Ease;
                    sp_CreateArgs.FindPropertyRelative("Alpha.Delay").floatValue = hpp.CreateArgs.Alpha.Delay;
                    #endregion

                    #region RecycleArgs
                    sp_RecycleArgs.FindPropertyRelative("Movement.Movement").enumValueIndex = (int)hpp.RecycleArgs.Movement.Movement;
                    sp_RecycleArgs.FindPropertyRelative("Movement.Distance").floatValue = hpp.RecycleArgs.Movement.Distance;
                    sp_RecycleArgs.FindPropertyRelative("Movement.Duration").floatValue = hpp.RecycleArgs.Movement.Duration;
                    sp_RecycleArgs.FindPropertyRelative("Movement.Delay").floatValue = hpp.RecycleArgs.Movement.Delay;
                    sp_RecycleArgs.FindPropertyRelative("Movement.Ease").enumValueIndex = (int)hpp.RecycleArgs.Movement.Ease;
                    sp_RecycleArgs.FindPropertyRelative("Movement.Curve").animationCurveValue = hpp.RecycleArgs.Movement.Curve;
                    sp_RecycleArgs.FindPropertyRelative("Movement.CurveName").stringValue = hpp.RecycleArgs.Movement.CurveName;

                    sp_RecycleArgs.FindPropertyRelative("Rotation.Rotation").enumValueIndex = (int)hpp.RecycleArgs.Rotation.Rotation;
                    sp_RecycleArgs.FindPropertyRelative("Rotation.Degree").floatValue = hpp.RecycleArgs.Rotation.Degree;
                    sp_RecycleArgs.FindPropertyRelative("Rotation.Duration").floatValue = hpp.RecycleArgs.Rotation.Duration;
                    sp_RecycleArgs.FindPropertyRelative("Rotation.Delay").floatValue = hpp.RecycleArgs.Rotation.Delay;
                    sp_RecycleArgs.FindPropertyRelative("Rotation.Ease").enumValueIndex = (int)hpp.RecycleArgs.Rotation.Ease;
                    sp_RecycleArgs.FindPropertyRelative("Rotation.Curve").animationCurveValue = hpp.RecycleArgs.Rotation.Curve;
                    sp_RecycleArgs.FindPropertyRelative("Rotation.CurveName").stringValue = hpp.RecycleArgs.Rotation.CurveName;

                    sp_RecycleArgs.FindPropertyRelative("Alpha.Duration").floatValue = hpp.RecycleArgs.Alpha.Duration;
                    sp_RecycleArgs.FindPropertyRelative("Alpha.Curve").animationCurveValue = hpp.RecycleArgs.Alpha.Curve;
                    sp_RecycleArgs.FindPropertyRelative("Alpha.CurveName").stringValue = hpp.RecycleArgs.Alpha.CurveName;
                    sp_RecycleArgs.FindPropertyRelative("Alpha.Ease").enumValueIndex = (int)hpp.RecycleArgs.Alpha.Ease;
                    sp_RecycleArgs.FindPropertyRelative("Alpha.Delay").floatValue = hpp.RecycleArgs.Alpha.Delay;
                    #endregion


                    sp_CreateArgs.serializedObject.ApplyModifiedProperties();
                    sp_RecycleArgs.serializedObject.ApplyModifiedProperties();

                    sp_DelayWithIn.serializedObject.ApplyModifiedProperties();
                    sp_DelayWithOut.serializedObject.ApplyModifiedProperties();
                    sp_key_Element_In.serializedObject.ApplyModifiedProperties();
                    sp_key_Element_Out.serializedObject.ApplyModifiedProperties();
                    sp_HideWithStart.serializedObject.ApplyModifiedProperties();
                });
                menu.AddSeparator("");
                menu.AddDisabledItem(new GUIContent("动效快速操作"));
                menu.AddItem(new GUIContent("E (复制动效)"), false, () =>
                {
                    string res = Editor_XHud_GUI.Open(XHud_DialogType.修改, "XHud - 元素预览器消息", "复制动效", "请选择动效参数复制模式！", "取消", "生成", "回收", 0);
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

                            json = JsonConvert.SerializeObject(crc);
                            GUIUtility.systemCopyBuffer = json;
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

                            json = JsonConvert.SerializeObject(rec);
                            GUIUtility.systemCopyBuffer = json;
                            break;
                    }

                    string mode = "";

                    if (res == "生成")
                        mode = "生成动效参数";
                    else if (res == "回收")
                        mode = "回收动效参数";

                    Editor_XHud_GUI.Open(XHud_DialogType.确认, "XHud - 元素预览器消息", "复制动效", $"已复制 \" {mode} \" 到系统剪贴板 ！", "明白", 0);
                });
                menu.AddItem(new GUIContent("R (粘贴动效)"), false, () =>
                {
                    string buffer = GUIUtility.systemCopyBuffer;
                    if (buffer.Contains("anchor"))//粘贴生成参数
                    {
                        string res = Editor_XHud_GUI.Open(XHud_DialogType.修改, "XHud - 元素预览器消息", "粘贴动效", "检测到动效参数类型为： \"生成动效\"，确定要使用这个参数吗？", "确定", "暂不", 0);
                        if (res == "暂不")
                            return;

                        Motion_Creator crc = JsonConvert.DeserializeObject<Motion_Creator>(GUIUtility.systemCopyBuffer);

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

                        Editor_XHud_GUI.Open(XHud_DialogType.确认, "XHud - 元素预览器消息", "粘贴动效", "已更新 \"生成\" 动效参数!", "明白", 0);
                    }
                    else//粘贴回收参数
                    {
                        string res = Editor_XHud_GUI.Open(XHud_DialogType.修改, "XHud - 元素预览器消息", "粘贴动效", "检测到动效参数类型为： \"回收动效\"，确定要使用这个参数吗？", "确定", "暂不", 0);
                        if (res == "暂不")
                            return;

                        Motion_Recycler rec = JsonConvert.DeserializeObject<Motion_Recycler>(buffer);

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

                        Editor_XHud_GUI.Open(XHud_DialogType.确认, "XHud - 元素预览器消息", "粘贴动效", "已更新 \"回收\" 动效参数!", "明白", 0);
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
            }
            #endregion

            #region 源脚本
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 3, "源脚本", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(5);

            #region 原始变量
            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_XHud_GUI.Gui_Layout_Space(10);
            BasicVars = EditorGUILayout.Foldout(BasicVars, "变量/属性", true);
            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Horizontal_End();
            if (BasicVars)
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

        private void CheckRmsNameValid()
        {
            XHud_Manager mgr = XHud_Dashboard.HudManagerGet();

            if (!mgr.RMS_Enabled)
                return;
            if (!RMS_Enabled.boolValue)
                return;
            if (string.IsNullOrEmpty(RMS_Name.stringValue))
            {
                string[] nodes = mgr.hm_RMS_GetResolutionNodeNames();
                RMS_Name.stringValue = nodes[0];
                RMS_Name.serializedObject.ApplyModifiedProperties();
            }
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
        #endregion
    }
}