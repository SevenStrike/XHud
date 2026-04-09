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
    using System.Collections;
    using System.Collections.Generic;
    using Unity.EditorCoroutines.Editor;
    using UnityEditor;
    using UnityEngine;
    using Object = UnityEngine.Object;
    using Random = UnityEngine.Random;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(XHud_Element_Sounder))]
    public class Editor_XHud_Element_Sounder : Editor
    {
        #region 组件
        private XHud_Element_Sounder BaseScript;
        private XHud_Manager HudManager;
        #endregion

        private bool OriginalDisplay;

        #region 序列化属性
        SerializedProperty
            UseRandomPitch,
            IsLoop,
            Pitch_Min,
            Pitch_Max,
            DebugState,
            Volume,
            Indicator,
            DelayTime,
            PreviewKey,
            UnPreviewKey,
            SoundName,
            Element,
            Toggle,
            Slider,
            Progress,
            Option,
            Button,
            Timings;
        #endregion

        #region 选择弹窗音效文件
        string commandName;
        private bool IsSelection;
        private bool IsObjectSelector;
        private AudioClip SelectedClip;
        #endregion

        #region 预览音效
        public List<AudioSource> Preivew_HudSound_SoundList = new List<AudioSource>();
        private List<EditorCoroutine> Preivew_HudSound_CoroutineList_Play = new List<EditorCoroutine>();
        #endregion

        #region 批量模式查看索引
        private int SounderStatistic_Index;
        #endregion

        #region 图标
        private Texture2D left_arrow_r, left_arrow_p, right_arrow_r, right_arrow_p, locate_r, locate_p, play_r, play_p, add_r, add_p, icon_main, channel, soundname, play_r_mul, play_p_mul, freq, length;
        #endregion

        #region 选项文字
        string[] stroptions_random = new string[2] { "规律", "随机" }, stroptions_debug = new string[2] { "关闭", "调试" }, stroptions_cycle = new string[2] { "单次", "循环" };
        #endregion

        #region 批量化操作
        private XHud_Element_Sounder[] SelectedObjects;

        private void GetAllTargets()
        {
            if (targets.Length > 1)
            {
                SelectedObjects = new XHud_Element_Sounder[targets.Length];
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    var t = targets[i];
                    SelectedObjects[i] = (XHud_Element_Sounder)t;
                }
            }
            else
            {
                SelectedObjects = new XHud_Element_Sounder[targets.Length];
                SelectedObjects[0] = (XHud_Element_Sounder)target;
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

            BaseScript = (XHud_Element_Sounder)target;

            UseRandomPitch = serializedObject.FindProperty("UseRandomPitch");
            Pitch_Min = serializedObject.FindProperty("Pitch_Min");
            Pitch_Max = serializedObject.FindProperty("Pitch_Max");
            Volume = serializedObject.FindProperty("Volume");
            DebugState = serializedObject.FindProperty("DebugState");
            Indicator = serializedObject.FindProperty("Indicator");
            DelayTime = serializedObject.FindProperty("DelayTime");
            Timings = serializedObject.FindProperty("Timings");
            SoundName = serializedObject.FindProperty("SoundName");
            IsLoop = serializedObject.FindProperty("IsLoop");
            Element = serializedObject.FindProperty("Element");
            Toggle = serializedObject.FindProperty("Toggle");
            Slider = serializedObject.FindProperty("Slider");
            Progress = serializedObject.FindProperty("Progress");
            Option = serializedObject.FindProperty("Option");
            Button = serializedObject.FindProperty("Button");
            PreviewKey = serializedObject.FindProperty("PreviewKey");
            UnPreviewKey = serializedObject.FindProperty("UnPreviewKey");

            icon_main = Editor_XHud_GUI.GetIcon("Icons_XHud_Sounder/icon_main");
            channel = Editor_XHud_GUI.GetIcon("Icons_XHud_Sounder/channel");
            soundname = Editor_XHud_GUI.GetIcon("Icons_XHud_Sounder/soundname");
            freq = Editor_XHud_GUI.GetIcon("Icons_XHud_Sounder/freq");
            length = Editor_XHud_GUI.GetIcon("Icons_XHud_Sounder/length");
            locate_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Sounder/locate_r");
            locate_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Sounder/locate_p");
            play_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Sounder/play_r");
            play_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Sounder/play_p");
            add_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Sounder/add_r");
            add_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Sounder/add_p");
            left_arrow_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Sounder/left_arrow_r");
            left_arrow_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Sounder/left_arrow_p");
            right_arrow_r = Editor_XHud_GUI.GetIcon("Icons_XHud_Sounder/right_arrow_r");
            right_arrow_p = Editor_XHud_GUI.GetIcon("Icons_XHud_Sounder/right_arrow_p");
            play_r_mul = Editor_XHud_GUI.GetIcon("Icons_XHud_Sounder/play_r_mul");
            play_p_mul = Editor_XHud_GUI.GetIcon("Icons_XHud_Sounder/play_p_mul");

            GetAllTargets();

            if (Element.objectReferenceValue == null)
            {
                Element.objectReferenceValue = BaseScript.GetComponent<XHud_Module_Element>();
                Element.serializedObject.ApplyModifiedProperties();
            }
            if (Toggle.objectReferenceValue == null)
            {
                Toggle.objectReferenceValue = BaseScript.GetComponent<XHud_Module_Toggle>();
                Toggle.serializedObject.ApplyModifiedProperties();
            }
            if (Slider.objectReferenceValue == null)
            {
                Slider.objectReferenceValue = BaseScript.GetComponent<XHud_Module_Slider>();
                Slider.serializedObject.ApplyModifiedProperties();
            }
            if (Progress.objectReferenceValue == null)
            {
                Progress.objectReferenceValue = BaseScript.GetComponent<XHud_Module_Progress>();
                Progress.serializedObject.ApplyModifiedProperties();
            }
            if (Option.objectReferenceValue == null)
            {
                Option.objectReferenceValue = BaseScript.GetComponent<XHud_Module_Option>();
                Option.serializedObject.ApplyModifiedProperties();
            }
            if (Button.objectReferenceValue == null)
            {
                Button.objectReferenceValue = BaseScript.GetComponent<XHud_Module_Button>();
                Button.serializedObject.ApplyModifiedProperties();
            }

            #region 如果音效名称是空的检查音效名称是否有效

            if (!IsMultiSelected())
            {
                if (string.IsNullOrEmpty(SoundName.stringValue))
                {
                    if (HudManager.Hud_Sounds != null)
                    {
                        if (!HudManager.Hud_Sounds.SoundLibrary_IsEmpty())
                        {
                            SoundName.stringValue = HudManager.Hud_Sounds.SoundLibrary_GetSoundNames()[0];
                            SoundName.serializedObject.ApplyModifiedProperties();
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    if (string.IsNullOrEmpty(SelectedObjects[i].SoundName))
                    {
                        SelectedObjects[i].SoundName = HudManager.Hud_Sounds.SoundLibrary_GetSoundNames()[0];
                    }
                }
            }
            #endregion
        }

        private void OnDisable()
        {
            Preview_HudSound_Stop();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (string.IsNullOrEmpty(BaseScript.Indicator))
                Editor_XHud_GUI.Gui_Layout_Banner(icon_main, HudFilled.实体, HudColor.深空灰, "XHud - 元素音效器", Color.white);
            else
                Editor_XHud_GUI.Gui_Layout_Banner(icon_main, HudFilled.实体, HudColor.深空灰, "XHud - 元素音效器 -> ( " + BaseScript.Indicator + " )", Color.white);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            #region 快捷控制
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 0, "快捷控制", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            #region 工具栏
            GUILayout.BeginHorizontal();
            Editor_XHud_GUI.Gui_Layout_Space(10);

            #region 预览当前声音
            if (Application.isPlaying)
                GUI.enabled = false;
            else
                GUI.enabled = true;

            if (Editor_XHud_GUI.Gui_Layout_Button(14, "预览当前声音", IsMultiSelected() ? play_r_mul : play_r, IsMultiSelected() ? play_p_mul : play_p))
            {
                Preview_HudSound_Play(DelayTime.floatValue);
            }
            GUI.enabled = true;
            #endregion

            GUILayout.FlexibleSpace();

            #region 定位资源
            if (Editor_XHud_GUI.Gui_Layout_Button(14, "定位到音效资源", locate_r, locate_p))
            {
                if (Application.isPlaying)
                {
                    Debug.Log("程序正在运行，无法在运行期间执行此功能！");
                    return;
                }
                Editor_XHud_MenuItemsAction_OpenLibrary.open_sound();
                if (HudManager.Hud_Sounds == null)
                    return;
                HudManager.Hud_Sounds.SoundLibrary_Location(SoundName.stringValue);
            }
            GUI.enabled = true;
            #endregion

            GUILayout.FlexibleSpace();

            #region 添加到库
            if (Editor_XHud_GUI.Gui_Layout_Button(14, "添加音效到库", add_r, add_p))
            {
                if (Application.isPlaying)
                {
                    Debug.Log("程序正在运行，无法在运行期间执行此功能！");
                    return;
                }
                IsObjectSelector = true;
                EditorGUIUtility.ShowObjectPicker<AudioClip>(SelectedClip, false, "", 0);
            }
            GUI.enabled = true;
            #endregion

            Editor_XHud_GUI.Gui_Layout_Space(10);
            GUILayout.EndHorizontal();
            #endregion

            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 音效控制
            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 0, "音效控制", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            #region 时机
            Editor_XHud_GUI.Gui_Layout_Space(5);

            string[] str_tims = new string[1] { "无" };

            #region Element
            if (Element.objectReferenceValue != null)
            {
                str_tims = new string[4] { "元素进入时", "元素进入后", "元素退出时", "自定义" };
            }
            #endregion

            #region Slider
            if (Slider.objectReferenceValue != null)
            {
                str_tims = new string[] { "无", "按下滑动条", "松开滑动条", "滑动条数值改变" };
            }
            #endregion

            #region Toggle
            if (Toggle.objectReferenceValue != null)
            {
                str_tims = new string[] { "无", "开关打开时", "开关关闭时", "开关按下时", "开关抬起时", "开关变化时" };
            }
            #endregion

            #region Progress
            if (Progress.objectReferenceValue != null)
            {
                str_tims = new string[] { "无", "进度开始时", "进度变化时", "进度结束时" };
            }
            #endregion

            #region Button
            if (Button.objectReferenceValue != null)
            {
                str_tims = new string[] { "无", "鼠标进入", "鼠标退出", "鼠标按下", "鼠标松开", "鼠标长按", "鼠标点击", "鼠标选中", "鼠标取消选中" };
            }
            #endregion

            #region Option
            if (Option.objectReferenceValue != null)
            {
                str_tims = new string[] { "无", "点击选项", "光标移动开始", "光标移动结束", "光标位置改变" };
            }
            #endregion

            Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_Element_Sounder>("播放时机", str_tims, ref Timings, HudFilled.实体, 120, 22, SelectedObjects);
            #endregion

            #region 列表
            if (HudManager.Hud_Sounds != null)
            {
                string[] collist = HudManager.Hud_Sounds.SoundLibrary_GetSoundNames();
                Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_Element_Sounder>("音效列表", collist, ref SoundName, HudFilled.实体, 120, 22, SelectedObjects);
            }
            #endregion

            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            Editor_XHud_GUI.Gui_Layout_Space(5);

            #region 选项

            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 0, "选项", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Element_Sounder>("随机音高", stroptions_random, ref UseRandomPitch, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Element_Sounder>("状态调试", stroptions_debug, ref DebugState, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Element_Sounder>("循环播放", stroptions_cycle, ref IsLoop, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);

            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            Editor_XHud_GUI.Gui_Layout_Space(5);

            #region 统计
            string statistic_title = "统计信息";
            if (IsMultiSelected())
                statistic_title = "统计信息 - ( 批量模式 )";
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, statistic_title, XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(5);

            if (HudManager.Hud_Sounds == null)
            {
                Editor_XHud_GUI.Gui_Layout_Labelfield("暂未在管理器中配置音效库", HudFilled.无, HudColor.无, Editor_XHud_GUI.GetColor(HudColor.阴影灰), TextAnchor.MiddleCenter);
            }
            else
            {
                if (HudManager.Hud_Sounds.SoundLibrary.Count <= 0)
                {
                    Editor_XHud_GUI.Gui_Layout_Labelfield("暂未在音效库中找到任何音效资源", HudFilled.无, HudColor.无, Editor_XHud_GUI.GetColor(HudColor.阴影灰), TextAnchor.MiddleCenter);
                }
                else
                {
                    if (!IsMultiSelected())
                    {
                        XHud_LibraryArg_Sound soundInfo = HudManager.Hud_Sounds.SoundLibrary_GetSoundInfo(SoundName.stringValue);

                        if (soundInfo != null)
                        {
                            Editor_XHud_GUI.StatuDisplayer_text(soundname, 12, new Vector2(0, 7), "名称", 12, soundInfo.Name, XHud_Dashboard.Theme_Primary, 11);
                            Editor_XHud_GUI.StatuDisplayer_text(channel, 12, new Vector2(0, 7), "声道", 12, soundInfo.Channel == 1 ? "单声道" : "立体声", XHud_Dashboard.Theme_Primary, 11);
                            Editor_XHud_GUI.StatuDisplayer_text(this.freq, 12, new Vector2(0, 7), "频率", 12, soundInfo.Frequency.ToString() + "hz", XHud_Dashboard.Theme_Primary, 11);
                            Editor_XHud_GUI.StatuDisplayer_text(length, 12, new Vector2(0, 7), "时长", 12, soundInfo.Length.ToString("F2") + " 秒", XHud_Dashboard.Theme_Primary, 11);
                        }
                    }
                    else
                    {
                        #region 批量控件
                        Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                        Editor_XHud_GUI.Gui_Layout_Space(10);
                        if (Editor_XHud_GUI.Gui_Layout_Button($"{SelectedObjects[SounderStatistic_Index].name} ( {SelectedObjects[SounderStatistic_Index].Indicator} )", "", HudFilled.透明, HudColor.无, Color.gray, 20))
                        {
                            EditorGUIUtility.PingObject(SelectedObjects[SounderStatistic_Index]);
                        }
                        Editor_XHud_GUI.Gui_Layout_FlexSpace();
                        if (Editor_XHud_GUI.Gui_Layout_Button(12, "", left_arrow_r, left_arrow_p))
                        {
                            if (SounderStatistic_Index <= 0)
                            {
                                SounderStatistic_Index = SelectedObjects.Length - 1;
                            }
                            else
                            {
                                SounderStatistic_Index--;
                            }
                            EditorGUIUtility.PingObject(SelectedObjects[SounderStatistic_Index]);
                        }
                        Editor_XHud_GUI.Gui_Layout_Space(16);
                        if (Editor_XHud_GUI.Gui_Layout_Button(12, "", right_arrow_r, right_arrow_p))
                        {
                            if (SounderStatistic_Index >= SelectedObjects.Length - 1)
                            {
                                SounderStatistic_Index = 0;
                            }
                            else
                            {
                                SounderStatistic_Index++;
                            }
                            EditorGUIUtility.PingObject(SelectedObjects[SounderStatistic_Index]);
                        }
                        Editor_XHud_GUI.Gui_Layout_Space(5);
                        Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                        #endregion

                        XHud_LibraryArg_Sound soundInfo = HudManager.Hud_Sounds.SoundLibrary_GetSoundInfo(SelectedObjects[SounderStatistic_Index].SoundName);

                        Editor_XHud_GUI.StatuDisplayer_text(soundname, 12, new Vector2(0, 7), "名称", 12, soundInfo.Name, XHud_Dashboard.Theme_Primary, 11);
                        Editor_XHud_GUI.StatuDisplayer_text(channel, 12, new Vector2(0, 7), "声道", 12, soundInfo.Channel == 1 ? "单声道" : "立体声", XHud_Dashboard.Theme_Primary, 11);
                        Editor_XHud_GUI.StatuDisplayer_text(this.freq, 12, new Vector2(0, 7), "频率", 12, soundInfo.Frequency.ToString() + "hz", XHud_Dashboard.Theme_Primary, 11);
                        Editor_XHud_GUI.StatuDisplayer_text(length, 12, new Vector2(0, 7), "时长", 12, soundInfo.Length.ToString("F2") + " 秒", XHud_Dashboard.Theme_Primary, 11);
                    }
                }
            }

            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            Editor_XHud_GUI.Gui_Layout_Space(5);

            #region 参数
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 0, "参数", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            Editor_XHud_GUI.Gui_Layout_Property_Field("音效标识", Indicator);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("音效音量", Volume);

            if (UseRandomPitch.intValue == 1)
            {
                Editor_XHud_GUI.Gui_Layout_Space(5);

                Editor_XHud_GUI.Gui_Layout_Property_Field("最小音高", Pitch_Min);

                Editor_XHud_GUI.Gui_Layout_Space(5);

                Editor_XHud_GUI.Gui_Layout_Property_Field("最大音高", Pitch_Max);

                Editor_XHud_GUI.Gui_Layout_Space(5);

                Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_SliderMinMax("音高范围", ref BaseScript.Pitch_Min, ref BaseScript.Pitch_Max, -2f, 3f);
                Editor_XHud_GUI.Gui_Layout_Horizontal_End();
            }

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("延迟时间", DelayTime);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("开始预览", PreviewKey);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Property_Field("停止预览", UnPreviewKey);

            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            ///------------选择音效
            commandName = Event.current.commandName;

            if (commandName == "ObjectSelectorUpdated")
            {
                UpdateSoundParams();
                IsSelection = false;
            }
            else if (commandName == "ObjectSelectorClosed")
            {
                if (!IsObjectSelector)
                    return;

                Object obj = EditorGUIUtility.GetObjectPickerObject();

                if (obj != null)
                {
                    if (!IsSelection)
                    {
                        IsObjectSelector = false;
                        IsSelection = true;
                        if (HudManager.Hud_Sounds.SoundLibrary == null)
                            HudManager.Hud_Sounds.SoundLibrary = new List<XHud_LibraryArg_Sound>();
                        AudioClip clip = obj as AudioClip;
                        HudManager.Hud_Sounds.SoundLibrary.Add(new XHud_LibraryArg_Sound(clip.name, clip));
                        UpdateSoundParams();
                    }
                }
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
        /// 更新音效库的每一项音效信息
        /// </summary>
        private void UpdateSoundParams()
        {
            if (HudManager == null)
                return;
            if (HudManager.Hud_Sounds == null)
                return;
            if (HudManager.Hud_Sounds.SoundLibrary != null && HudManager.Hud_Sounds.SoundLibrary.Count > 0)
            {
                for (int i = 0; i < HudManager.Hud_Sounds.SoundLibrary.Count; i++)
                {
                    if (HudManager.Hud_Sounds.SoundLibrary[i].Clip != null)
                    {
                        HudManager.Hud_Sounds.SoundLibrary[i].Channel = HudManager.Hud_Sounds.SoundLibrary[i].Clip.channels;
                        HudManager.Hud_Sounds.SoundLibrary[i].Frequency = HudManager.Hud_Sounds.SoundLibrary[i].Clip.frequency;
                        HudManager.Hud_Sounds.SoundLibrary[i].Length = HudManager.Hud_Sounds.SoundLibrary[i].Clip.length;
                    }
                }
            }
        }
        #endregion

        #region 预览音效
        private void Preview_HudSound_Play(float Time)
        {
            if (HudManager.Hud_Sounds == null)
            {
                EditorApplication.delayCall += () =>
                {
                    string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 音效消息", "未指定音效库", "未在HudManager中配置音效库！请先前往XHud管理器指定一个音效库！", "明白", "前往", 1);
                    if (res == "前往")
                    {
                        Transform man = FindFirstObjectByType<XHud_Manager>().transform;
                        EditorGUIUtility.PingObject(man);
                    }
                    return;
                };
            }
            else
            {
                if (IsMultiSelected())
                {
                    for (int i = 0; i < SelectedObjects.Length; i++)
                    {
                        EditorCoroutine cor = EditorCoroutineUtility.StartCoroutineOwnerless(Preview_HudSound_Play(SelectedObjects[i].SoundName, SelectedObjects[i].DelayTime));
                        Preivew_HudSound_CoroutineList_Play.Add(cor);
                    }
                }
                else
                {
                    EditorCoroutine cor = EditorCoroutineUtility.StartCoroutineOwnerless(Preview_HudSound_Play(SoundName.stringValue, DelayTime.floatValue));
                    Preivew_HudSound_CoroutineList_Play.Add(cor);
                }
            }
        }

        IEnumerator Preview_HudSound_Play(string name, float time)
        {
            yield return new EditorWaitForSeconds(time);
            if (HudManager.Hud_Sounds.SoundLibrary_NameIsValid(name))
            {
                Preivew_HudSound_SoundList.Add(Preview_HudSound_Creator(HudManager.Hud_Sounds.SoundLibrary_GetSound(name)));
            }
            Repaint();
        }

        /// <summary>
        /// 预览声音
        /// </summary>
        /// <param name="info"></param>
        public AudioSource Preview_HudSound_Creator(AudioClip clip)
        {
            GameObject obj = new GameObject();
            obj.name = "SoundPreviewer-" + "[" + clip.length.ToString("F2") + " s]-" + "[" + clip.channels + " ch]-" + "[" + clip.frequency + " hz]";
            AudioSource au = obj.AddComponent<AudioSource>();
            au.clip = clip;
            au.volume = Volume.floatValue;
            if (UseRandomPitch.intValue == 1)
            {
                au.pitch = Random.Range(Pitch_Min.floatValue, Pitch_Max.floatValue);
            }
            else
            {
                au.pitch = 1.0f;
            }
            au.Play();
            XHud_AudioStoper sp = au.gameObject.AddComponent<XHud_AudioStoper>();
            sp.AudioSource = au;
            return au;
        }

        private void Preview_HudSound_Stop()
        {
            if (Preivew_HudSound_SoundList != null)
            {
                for (int i = 0; i < Preivew_HudSound_SoundList.Count; i++)
                {
                    if (Preivew_HudSound_SoundList[i] != null)
                    {
                        Preivew_HudSound_SoundList[i].Stop();
                        DestroyImmediate(Preivew_HudSound_SoundList[i].gameObject, true);
                        Preivew_HudSound_SoundList[i] = null;
                    }
                }
                Preivew_HudSound_SoundList.Clear();
            }
            for (int i = 0; i < Preivew_HudSound_CoroutineList_Play.Count; i++)
            {
                if (Preivew_HudSound_CoroutineList_Play[i] != null)
                    EditorCoroutineUtility.StopCoroutine(Preivew_HudSound_CoroutineList_Play[i]);
            }
            Preivew_HudSound_CoroutineList_Play.Clear();
            SceneView.RepaintAll();
        }
        #endregion
    }
}