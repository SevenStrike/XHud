namespace SevenStrikeModules.XHud.Hud
{
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.GuiLib;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;

    [CustomEditor(typeof(Hud_TransitionController))]
    public class Editor_Hud_TransitionController : Editor
    {
        #region 组件
        private Hud_TransitionController BaseScript;
        #endregion

        private bool BasicVars;

        #region 序列化属性
        private SerializedProperty TransitionImage, TransitionMat, Mode, ModeSwitch, DebugState, TransitionTime, TransitionName, IsTransiting, TransitionProgress, CurrentTransitionNode, TransitionPlayKey, TransitionFlip_H_Key, TransitionFlip_V_Key, TransitionMod_Key, TransitionOverlayColor, LimiteFramePer_Start, LimiteFramePer_End, IsTransiting_WithEnd, IsTransiting_WithStart, TransitionAlpha, CurrentFrame, Flip_Hor, Flip_Ver, UseKeyControl;
        #endregion

        #region 图标
        private Texture2D icon_main, trans_state, trans_first_frame, trans_end_frame, openlib_r, openlib_p, update_r, update_p, locate_r, locate_p;
        #endregion

        #region 选项文字
        string[] stroptions_enabled = new string[2] { "关闭", "开启" }, stroptions_debug = new string[2] { "关闭", "调试" }, stroptions_transmode = new string[] { "入场", "出场" }, stroptions_flip = new string[] { "常规", "翻转" };
        #endregion

        #region 批量化操作
        Hud_TransitionController[] SelectedObjects;

        private void GetAllTargets()
        {
            if (targets.Length > 1)
            {
                SelectedObjects = new Hud_TransitionController[targets.Length];
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    var t = targets[i];
                    SelectedObjects[i] = (Hud_TransitionController)t;
                }
            }
            else
            {
                SelectedObjects = new Hud_TransitionController[targets.Length];
                SelectedObjects[0] = (Hud_TransitionController)target;
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

        void OnEnable()
        {
            BaseScript = (Hud_TransitionController)target;

            #region 获取序列化属性
            TransitionImage = serializedObject.FindProperty("TransitionImage");
            TransitionMat = serializedObject.FindProperty("TransitionMat");
            Mode = serializedObject.FindProperty("Mode");
            ModeSwitch = serializedObject.FindProperty("ModeSwitch");
            TransitionTime = serializedObject.FindProperty("TransitionTime");
            TransitionName = serializedObject.FindProperty("TransitionName");
            IsTransiting = serializedObject.FindProperty("IsTransiting");
            TransitionProgress = serializedObject.FindProperty("TransitionProgress");
            CurrentFrame = serializedObject.FindProperty("CurrentFrame");
            TransitionName = serializedObject.FindProperty("TransitionName");
            CurrentTransitionNode = serializedObject.FindProperty("CurrentTransitionNode");
            TransitionPlayKey = serializedObject.FindProperty("TransitionPlayKey");
            TransitionFlip_H_Key = serializedObject.FindProperty("TransitionFlip_H_Key");
            TransitionFlip_V_Key = serializedObject.FindProperty("TransitionFlip_V_Key");
            TransitionMod_Key = serializedObject.FindProperty("TransitionMod_Key");
            LimiteFramePer_Start = serializedObject.FindProperty("LimiteFramePer_Start");
            LimiteFramePer_End = serializedObject.FindProperty("LimiteFramePer_End");
            IsTransiting_WithEnd = serializedObject.FindProperty("IsTransiting_WithEnd");
            IsTransiting_WithStart = serializedObject.FindProperty("IsTransiting_WithStart");
            DebugState = serializedObject.FindProperty("DebugState");
            TransitionOverlayColor = serializedObject.FindProperty("TransitionOverlayColor");
            TransitionAlpha = serializedObject.FindProperty("TransitionAlpha");
            Flip_Hor = serializedObject.FindProperty("Flip_Hor");
            Flip_Ver = serializedObject.FindProperty("Flip_Ver");
            UseKeyControl = serializedObject.FindProperty("UseKeyControl");

            Hud_Manager mgr = util_Dashboard.HudManagerGet();
            #endregion

            #region 获取图标
            icon_main = util_XHUDGUI.GetIcon("Icons_Hud_TransitionController/icon_main");
            trans_state = util_XHUDGUI.GetIcon("Icons_Hud_TransitionController/trans_state");
            trans_first_frame = util_XHUDGUI.GetIcon("Icons_Hud_TransitionController/trans_first_frame");
            trans_end_frame = util_XHUDGUI.GetIcon("Icons_Hud_TransitionController/trans_end_frame");
            openlib_r = util_XHUDGUI.GetIcon("Icons_Hud_TransitionController/openlib_r");
            openlib_p = util_XHUDGUI.GetIcon("Icons_Hud_TransitionController/openlib_p");
            update_r = util_XHUDGUI.GetIcon("Icons_Hud_TransitionController/update_r");
            update_p = util_XHUDGUI.GetIcon("Icons_Hud_TransitionController/update_p");
            locate_r = util_XHUDGUI.GetIcon("Icons_Hud_TransitionController/locate_r");
            locate_p = util_XHUDGUI.GetIcon("Icons_Hud_TransitionController/locate_p");
            #endregion

            GetAllTargets();

            if ((Image)TransitionImage.objectReferenceValue == null)
            {
                TransitionImage.objectReferenceValue = BaseScript.GetComponent<Image>();
                TransitionImage.serializedObject.ApplyModifiedProperties();
            }

            if (TransitionMat.objectReferenceValue != null)
            {
                Material mat = TransitionMat.objectReferenceValue as Material;
                TransitionOverlayColor.colorValue = mat.color;
                TransitionOverlayColor.serializedObject.ApplyModifiedProperties();
            }

            if (string.IsNullOrEmpty(TransitionName.stringValue))
            {
                if (mgr.Hud_TransitionLib != null && !mgr.Hud_TransitionLib.TransitionLibrary_IsEmpty())
                {
                    TransitionName.stringValue = mgr.Hud_TransitionLib.TransitionLibrary_GetAllNames()[0];
                    TransitionName.serializedObject.ApplyModifiedProperties();

                    BaseScript.CurrentTransitionNode = mgr.Hud_TransitionLib.TransitionLibrary_Get(TransitionName.stringValue);
                }
            }
        }

        void OnDisable()
        {

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            util_XHUDGUI.Gui_Layout_Banner(icon_main, HudFilled.实体, HudColor.深空灰, "Hud - 转场控制器", Color.white);

            Hud_Manager mgr = util_Dashboard.HudManagerGet();


            util_XHUDGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "快捷功能", util_Dashboard.Theme_Primary);
            util_XHUDGUI.Gui_Layout_Space(10);

            util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            util_XHUDGUI.Gui_Layout_Space(10);

            #region 查看转场库
            GUI.enabled = true;
            if (util_XHUDGUI.Gui_Layout_Button(14, "查看当前转场库", openlib_r, openlib_p))
            {
                if (mgr.Hud_TransitionLib != null)
                    EditorUtility.OpenPropertyEditor(mgr.Hud_TransitionLib);
            }
            #endregion

            GUILayout.FlexibleSpace();

            #region 快速刷新转场资源
            if (util_XHUDGUI.Gui_Layout_Button(14, "快速刷新转场资源", update_r, update_p))
            {
                if (mgr.Hud_TransitionLib.TransitionLibrary_IsEmpty())
                    return;

                SerializedProperty sp_Name = CurrentTransitionNode.FindPropertyRelative("Name");
                SerializedProperty sp_TotalFramesCount = CurrentTransitionNode.FindPropertyRelative("TotalFramesCount");
                SerializedProperty sp_LastFrameIndex = CurrentTransitionNode.FindPropertyRelative("LastFrameIndex");
                SerializedProperty sp_SkipFrame = CurrentTransitionNode.FindPropertyRelative("SkipFrame");
                SerializedProperty sp_Res = CurrentTransitionNode.FindPropertyRelative("Res");
                SerializedProperty sp_x_frames = CurrentTransitionNode.FindPropertyRelative("Frames");

                TransitionNode node = mgr.Hud_TransitionLib.TransitionLibrary_Get(TransitionName.stringValue);

                sp_Name.stringValue = node.Name;
                sp_TotalFramesCount.intValue = node.TotalFramesCount;
                sp_LastFrameIndex.intValue = node.LastFrameIndex;
                sp_SkipFrame.intValue = node.SkipFrame;
                sp_Res.vector2IntValue = node.Res;
                sp_x_frames.ClearArray();
                for (int i = 0; i < node.Frames.Count; i++)
                {
                    sp_x_frames.InsertArrayElementAtIndex(i);
                    sp_x_frames.GetArrayElementAtIndex(i).objectReferenceValue = node.Frames[i];
                }

                TransitionProgress.floatValue = 0;
                TransitionProgress.serializedObject.ApplyModifiedProperties();
                UpdateTransition(sp_x_frames, sp_LastFrameIndex);
                return;
            }
            #endregion

            util_XHUDGUI.Gui_Layout_Space(10);
            util_XHUDGUI.Gui_Layout_Horizontal_End();

            util_XHUDGUI.Gui_Layout_Space(10);
            util_XHUDGUI.Gui_Layout_Vertical_End();

            #region 选项
            util_XHUDGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "选项", util_Dashboard.Theme_Primary);
            util_XHUDGUI.Gui_Layout_Space(10);

            #region 按键控制
            util_XHUDGUI.Gui_Layout_Toggle<bool, Hud_TransitionController>("按键控制", stroptions_enabled, ref UseKeyControl, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
            #endregion

            #region 转场方式
            EditorGUI.BeginChangeCheck();
            util_XHUDGUI.Gui_Layout_Toggle<bool, Hud_TransitionController>("转场方式", stroptions_transmode, ref ModeSwitch, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
            if (EditorGUI.EndChangeCheck())
            {
                if (ModeSwitch.boolValue)
                    Mode.enumValueIndex = (int)TransitionMode.出场;
                else
                    Mode.enumValueIndex = (int)TransitionMode.入场;

                Mode.serializedObject.ApplyModifiedProperties();
                TransitionProgress.floatValue = 0;
                TransitionProgress.serializedObject.ApplyModifiedProperties();
            }
            #endregion

            #region 水平翻转
            util_XHUDGUI.Gui_Layout_Toggle<bool, Hud_TransitionController>("水平翻转", stroptions_flip, ref Flip_Hor, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects, null, (res) =>
            {
                Material mat = (Material)TransitionMat.objectReferenceValue;
                mat.SetInt("_Flip_Hor", Flip_Hor.boolValue ? 1 : 0);
                TransitionMat.serializedObject.ApplyModifiedProperties();
            });
            #endregion

            #region 垂直翻转
            util_XHUDGUI.Gui_Layout_Toggle<bool, Hud_TransitionController>("垂直翻转", stroptions_flip, ref Flip_Ver, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects, null, (res) =>
            {
                Material mat = (Material)TransitionMat.objectReferenceValue;
                mat.SetInt("_Flip_Ver", Flip_Ver.boolValue ? 1 : 0);
                TransitionMat.serializedObject.ApplyModifiedProperties();
            });
            #endregion         

            #region 调试模式
            util_XHUDGUI.Gui_Layout_Toggle<bool, Hud_TransitionController>("调试模式", stroptions_debug, ref DebugState, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
            #endregion         

            #region 快速获取转场效果
            if (mgr != null && mgr.Hud_TransitionLib != null)
            {
                string[] TransLibNames = mgr.Hud_TransitionLib.TransitionLibrary_GetAllNames();
                util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);

                util_XHUDGUI.Gui_Layout_Popup<string, Hud_TransitionController>("转场资源库", TransLibNames, ref TransitionName, HudFilled.实体, 94, 22, SelectedObjects, (comps) => { }, (res) =>
                {
                    SerializedProperty sp_Name = CurrentTransitionNode.FindPropertyRelative("Name");
                    SerializedProperty sp_TotalFramesCount = CurrentTransitionNode.FindPropertyRelative("TotalFramesCount");
                    SerializedProperty sp_LastFrameIndex = CurrentTransitionNode.FindPropertyRelative("LastFrameIndex");
                    SerializedProperty sp_SkipFrame = CurrentTransitionNode.FindPropertyRelative("SkipFrame");
                    SerializedProperty sp_Res = CurrentTransitionNode.FindPropertyRelative("Res");
                    SerializedProperty sp_x_frames = CurrentTransitionNode.FindPropertyRelative("Frames");

                    TransitionNode node = mgr.Hud_TransitionLib.TransitionLibrary_Get(res);
                    sp_Name.stringValue = node.Name;
                    sp_TotalFramesCount.intValue = node.TotalFramesCount;
                    sp_LastFrameIndex.intValue = node.LastFrameIndex;
                    sp_SkipFrame.intValue = node.SkipFrame;
                    sp_Res.vector2IntValue = node.Res;
                    sp_x_frames.ClearArray();
                    for (int i = 0; i < node.Frames.Count; i++)
                    {
                        sp_x_frames.InsertArrayElementAtIndex(i);
                        sp_x_frames.GetArrayElementAtIndex(i).objectReferenceValue = node.Frames[i];
                    }

                    TransitionProgress.floatValue = 0;

                    UpdateTransition(sp_x_frames, sp_LastFrameIndex);
                });

                if (!IsMultiSelected())
                {
                    util_XHUDGUI.Gui_Layout_Space(10);

                    if (util_XHUDGUI.Gui_Layout_Button(14, "定位到转场库", locate_r, locate_p, 4))
                    {
                        //获取目标转场的索引号
                        int index = 0;
                        for (int k = 0; k < TransLibNames.Length; k++)
                        {
                            if (TransLibNames[k] == TransitionName.stringValue)
                            {
                                index = k;
                                break;
                            }
                        }
                        //定位到元素库中的对应当前资源
                        //打开目标转场库
                        util_OpenLibrarys.open_transition();
                        mgr.Hud_TransitionLib.TransitionLibrary_Location(TransLibNames[index]);
                    }
                }
                util_XHUDGUI.Gui_Layout_Horizontal_End();
            }
            #endregion

            util_XHUDGUI.Gui_Layout_Space(10);
            util_XHUDGUI.Gui_Layout_Vertical_End();
            #endregion

            #region 状态
            util_XHUDGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "状态", util_Dashboard.Theme_Primary);
            util_XHUDGUI.Gui_Layout_Space(10);

            #region 转场状态
            util_XHUDGUI.StatuDisplayer_text(trans_state, 12, new Vector2(0, 7), "转场状态", 12, IsTransiting.boolValue ? "正在转场中" : "等待转场", util_Dashboard.Theme_Primary, 11);
            #endregion

            #region 接近首帧状态
            SerializedProperty sp_frames_co = CurrentTransitionNode.FindPropertyRelative("Frames");
            int frameLimite_start = (int)Mathf.Floor(sp_frames_co.arraySize * LimiteFramePer_Start.floatValue);

            util_XHUDGUI.StatuDisplayer_text(trans_first_frame, 12, new Vector2(0, 7), "接近首帧状态", 12, IsTransiting_WithStart.boolValue ? $"接近首帧状态 ({frameLimite_start})" : $"等待转场 ({frameLimite_start})", util_Dashboard.Theme_Primary, 11);
            #endregion

            #region 接近尾帧状态
            int frameLimite_end = (int)Mathf.Floor(sp_frames_co.arraySize * LimiteFramePer_End.floatValue);

            util_XHUDGUI.StatuDisplayer_text(trans_end_frame, 12, new Vector2(0, 7), "接近尾帧状态", 12, IsTransiting_WithEnd.boolValue ? $"接近尾帧状态 ({frameLimite_end})" : $"等待转场 ({frameLimite_end})", util_Dashboard.Theme_Primary, 11);
            #endregion

            util_XHUDGUI.Gui_Layout_Space(10);
            util_XHUDGUI.Gui_Layout_Vertical_End();
            #endregion

            #region 参数
            util_XHUDGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.阴影灰, 5, "参数", util_Dashboard.Theme_Primary);
            util_XHUDGUI.Gui_Layout_Space(10);

            #region 转场组件
            util_XHUDGUI.Gui_Layout_Property_Field("转场组件", TransitionImage);
            #endregion

            util_XHUDGUI.Gui_Layout_Space(5);

            #region 转场材质
            EditorGUI.BeginChangeCheck();
            util_XHUDGUI.Gui_Layout_Property_Field("转场材质", TransitionMat);
            if (EditorGUI.EndChangeCheck())
            {
                Image img = (Image)TransitionImage.objectReferenceValue;
                img.material = (Material)TransitionMat.objectReferenceValue;
                TransitionImage.serializedObject.ApplyModifiedProperties();
            }
            #endregion

            util_XHUDGUI.Gui_Layout_Space(5);

            #region 转场叠加颜色
            EditorGUI.BeginChangeCheck();
            util_XHUDGUI.Gui_Layout_Property_Field("叠加颜色", TransitionOverlayColor);
            if (EditorGUI.EndChangeCheck())
            {
                Image img = (Image)TransitionImage.objectReferenceValue;
                img.color = Color.white;
                img.material = (Material)TransitionMat.objectReferenceValue;
                img.material.SetColor("_Color", TransitionOverlayColor.colorValue);
                TransitionImage.serializedObject.ApplyModifiedProperties();
            }
            #endregion

            util_XHUDGUI.Gui_Layout_Space(5);

            #region 转场参数
            util_XHUDGUI.Gui_Layout_Property_Field("转场参数", CurrentTransitionNode);
            #endregion

            util_XHUDGUI.Gui_Layout_Space(5);

            #region 转场进度
            SerializedProperty sp_frames = CurrentTransitionNode.FindPropertyRelative("Frames");
            SerializedProperty sp_lastframeindex = CurrentTransitionNode.FindPropertyRelative("LastFrameIndex");
            if (sp_frames != null && sp_frames.arraySize > 0)
            {
                EditorGUI.BeginChangeCheck();
                util_XHUDGUI.Gui_Layout_Property_Field("转场进度", TransitionProgress);
                if (EditorGUI.EndChangeCheck())
                {
                    UpdateTransition(sp_frames, sp_lastframeindex);
                }
            }
            #endregion

            util_XHUDGUI.Gui_Layout_Space(5);

            #region 转场透明度            
            util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            util_XHUDGUI.Gui_Layout_Space(5);
            EditorGUI.BeginChangeCheck();
            TransitionAlpha.floatValue = util_XHUDGUI.Gui_Layout_Slider("透明度", TransitionAlpha.floatValue, 0, 1);
            TransitionAlpha.serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
            {
                BaseScript.Transition_Set_Alpha(TransitionAlpha.floatValue);
                if (TransitionMat.objectReferenceValue != null)
                {
                    Material mat = TransitionMat.objectReferenceValue as Material;
                    mat.SetFloat("_Alpha", TransitionAlpha.floatValue);
                    TransitionOverlayColor.serializedObject.ApplyModifiedProperties();
                }
            }
            util_XHUDGUI.Gui_Layout_Horizontal_End();
            #endregion

            util_XHUDGUI.Gui_Layout_Space(5);

            #region 当前帧
            EditorGUI.BeginChangeCheck();
            util_XHUDGUI.Gui_Layout_Property_Field("当前帧", CurrentFrame);
            if (EditorGUI.EndChangeCheck())
            {

            }
            #endregion

            util_XHUDGUI.Gui_Layout_Space(5);

            #region 帧间隔速率
            EditorGUI.BeginChangeCheck();
            util_XHUDGUI.Gui_Layout_Property_Field("帧间隔速率", TransitionTime);
            if (EditorGUI.EndChangeCheck())
            {

            }
            #endregion

            util_XHUDGUI.Gui_Layout_Space(5);

            #region 接近开始帧
            util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            util_XHUDGUI.Gui_Layout_Space(5);
            util_XHUDGUI.Gui_Layout_SliderMinMax("接近转场", ref BaseScript.LimiteFramePer_Start, ref BaseScript.LimiteFramePer_End, 0, 1);
            util_XHUDGUI.Gui_Layout_Horizontal_End();
            #endregion

            util_XHUDGUI.Gui_Layout_Space(5);

            #region 测试按键
            util_XHUDGUI.Gui_Layout_Property_Field("转场模式", TransitionMod_Key);
            util_XHUDGUI.Gui_Layout_Space(5);
            util_XHUDGUI.Gui_Layout_Property_Field("按键转场", TransitionPlayKey);
            util_XHUDGUI.Gui_Layout_Space(5);
            util_XHUDGUI.Gui_Layout_Property_Field("水平翻转", TransitionFlip_H_Key);
            util_XHUDGUI.Gui_Layout_Space(5);
            util_XHUDGUI.Gui_Layout_Property_Field("垂直翻转", TransitionFlip_V_Key);
            #endregion

            util_XHUDGUI.Gui_Layout_Space(10);
            util_XHUDGUI.Gui_Layout_Vertical_End();
            #endregion

            #region 源脚本
            util_XHUDGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 3, "源脚本", util_Dashboard.Theme_Primary);
            util_XHUDGUI.Gui_Layout_Space(5);

            #region 原始变量
            util_XHUDGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            util_XHUDGUI.Gui_Layout_Space(10);
            BasicVars = EditorGUILayout.Foldout(BasicVars, "变量/属性", true);
            util_XHUDGUI.Gui_Layout_Space(5);
            util_XHUDGUI.Gui_Layout_Horizontal_End();
            if (BasicVars)
                DrawDefaultInspector();
            #endregion

            util_XHUDGUI.Gui_Layout_Space(5);
            util_XHUDGUI.Gui_Layout_Vertical_End();
            #endregion

            serializedObject.ApplyModifiedProperties();
        }

        #region 辅助

        /// <summary>
        /// 更新转场数据
        /// </summary>
        /// <param name="sp_frames"></param>
        /// <param name="sp_lastframeindex"></param>
        private void UpdateTransition(SerializedProperty sp_frames, SerializedProperty sp_lastframeindex)
        {
            TransitionMode mode = (TransitionMode)Mode.enumValueIndex;
            CurrentFrame.intValue = (int)(TransitionProgress.floatValue * sp_lastframeindex.intValue);
            if (mode == TransitionMode.入场)
            {
                BaseScript.Transition_Set_ChannelInvert(false);
            }
            else
            {
                BaseScript.Transition_Set_ChannelInvert(true);
            }
            if (sp_frames.arraySize > 0)
            {
                Texture2D tex = (Texture2D)sp_frames.GetArrayElementAtIndex(CurrentFrame.intValue).objectReferenceValue;
                BaseScript.Transition_Set_MaskTexture(tex);
            }
        }
        #endregion
    }
}