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
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    [CustomEditor(typeof(XHud_Library_Sounds))]
    public class Editor_XHud_Library_Sounds : Editor
    {
        #region 组件 / 列表
        private XHud_Library_Sounds BaseScript;
        /// <summary>
        /// 音效列表
        /// </summary>
        public ReorderableList SoundInfoList;
        #endregion

        #region 序列化属性
        private SerializedProperty sp_SoundInfoList_Original_Scroller, sp_itemHeight, sp_visibleItemCount, sp_SoundList, sp_UseRandomPitch, sp_Pitch_Min, sp_Pitch_Max, sp_SelectedIndex, sp_LocationSelectedIndex, sp_Highlight, sp_Find, LibraryName;
        #endregion

        private float LineHeight;

        private AudioClip SelectedClip;

        #region 图标
        private Texture2D scan_p, scan_r, stoppreview_p, stoppreview_r, clear_p, clear_r, create_p, create_r, delete_p, delete_r;
        #endregion

        string commandName;
        private bool selectobject;

        List<AudioSource> PreviewAudioList;
        private bool IsObjectSelector;
        private bool IsObjectSelectorUpdated;

        /// <summary>
        /// 屏蔽颜色
        /// </summary>
        private Color blocked_col;
        /// <summary>
        /// 屏蔽颜色透明度
        /// </summary>
        private float blocked_alp = 0.45f;

        #region 选项文字
        string[] stroptions_enabled = new string[2] { "关闭", "开启" };
        #endregion

        #region 批量化操作
        XHud_Library_Sounds[] SelectedObjects;

        private void GetAllTargets()
        {
            if (targets.Length > 1)
            {
                SelectedObjects = new XHud_Library_Sounds[targets.Length];
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    var t = targets[i];
                    SelectedObjects[i] = (XHud_Library_Sounds)t;
                }
            }
            else
            {
                SelectedObjects = new XHud_Library_Sounds[targets.Length];
                SelectedObjects[0] = (XHud_Library_Sounds)target;
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
            BaseScript = (XHud_Library_Sounds)target;

            #region 获取序列化属性
            sp_SoundList = serializedObject.FindProperty("SoundLibrary");
            sp_UseRandomPitch = serializedObject.FindProperty("UseRandomPitch");
            sp_Pitch_Min = serializedObject.FindProperty("Pitch_Min");
            sp_Pitch_Max = serializedObject.FindProperty("Pitch_Max");
            LibraryName = serializedObject.FindProperty("LibraryName");
            sp_SelectedIndex = serializedObject.FindProperty("SelectedIndex");
            sp_Highlight = serializedObject.FindProperty("Highlight");
            sp_LocationSelectedIndex = serializedObject.FindProperty("LocationSelectedIndex");
            sp_SoundInfoList_Original_Scroller = serializedObject.FindProperty("SoundInfoList_Original_Scroller");
            sp_itemHeight = serializedObject.FindProperty("itemHeight");
            sp_visibleItemCount = serializedObject.FindProperty("visibleItemCount");
            sp_Find = serializedObject.FindProperty("Find");
            #endregion

            LineHeight = EditorGUIUtility.singleLineHeight;

            GetAllTargets();

            //设定列表项高度值
            sp_itemHeight.floatValue = 50;
            sp_itemHeight.serializedObject.ApplyModifiedProperties();

            sp_visibleItemCount.intValue = 9;
            sp_visibleItemCount.serializedObject.ApplyModifiedProperties();

            #region 获取图标
            scan_p = Editor_XHud_GUI.GetIcon("Icons_XHud_SoundLibrary/scan_p");
            scan_r = Editor_XHud_GUI.GetIcon("Icons_XHud_SoundLibrary/scan_r");
            stoppreview_p = Editor_XHud_GUI.GetIcon("Icons_XHud_SoundLibrary/stoppreview_p");
            stoppreview_r = Editor_XHud_GUI.GetIcon("Icons_XHud_SoundLibrary/stoppreview_r");
            clear_p = Editor_XHud_GUI.GetIcon("Icons_XHud_SoundLibrary/clear_p");
            clear_r = Editor_XHud_GUI.GetIcon("Icons_XHud_SoundLibrary/clear_r");
            create_p = Editor_XHud_GUI.GetIcon("Icons_XHud_SoundLibrary/create_p");
            create_r = Editor_XHud_GUI.GetIcon("Icons_XHud_SoundLibrary/create_r");
            delete_p = Editor_XHud_GUI.GetIcon("Icons_XHud_SoundLibrary/delete_p");
            delete_r = Editor_XHud_GUI.GetIcon("Icons_XHud_SoundLibrary/delete_r");
            #endregion

            blocked_col = new Color(0, 0, 0, blocked_alp);

            Vector2 ButtonSize = new Vector2(16, 16);

            #region ReorderableList - SoundInfoList
            SoundInfoList = new ReorderableList(serializedObject, sp_SoundList, true, true, true, true);
            SoundInfoList.drawElementCallback = SoundInfoList_Original_DrawElementCallback;
            #endregion

            EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;
            UpdateSoundParams();
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= EditorApplication_playModeStateChanged;

            sp_SoundInfoList_Original_Scroller.vector2Value = Vector2.zero;
            sp_SoundInfoList_Original_Scroller.serializedObject.ApplyModifiedProperties();

            sp_LocationSelectedIndex.intValue = -1;
            sp_LocationSelectedIndex.serializedObject.ApplyModifiedProperties();

            sp_SelectedIndex.intValue = -1;
            sp_SelectedIndex.serializedObject.ApplyModifiedProperties();

            sp_Find.stringValue = string.Empty;
            sp_Find.serializedObject.ApplyModifiedProperties();

            sp_Highlight.stringValue = string.Empty;
            sp_Highlight.serializedObject.ApplyModifiedProperties();

            RecycleAudio();
        }

        #region SoundInfoList_Original      

        Rect drawelement_rect;
        Rect scrollview_rect;
        Rect item_rect;
        Color SelectedBg = new Color(0, 0, 0, 0.2f);

        Rect x_dragarea;
        Rect dragarea;

        private void BlockGUI(string name)
        {
            if (!name.Contains(sp_Highlight.stringValue))
            {
                GUI.enabled = false;
            }
            else
            {
                GUI.enabled = true;
            }
        }

        /// <summary>
        /// 绘制元素
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="index"></param>
        /// <param name="isActive"></param>
        /// <param name="isFocused"></param>
        private void SoundInfoList_Original_DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            float titleheight = rect.y + 2;

            SerializedProperty prop = sp_SoundList.GetArrayElementAtIndex(index);

            SerializedProperty sp_name = prop.FindPropertyRelative("Name");
            SerializedProperty sp_clip = prop.FindPropertyRelative("Clip");
            SerializedProperty sp_length = prop.FindPropertyRelative("Length");
            SerializedProperty sp_chan = prop.FindPropertyRelative("Channel");
            SerializedProperty sp_freq = prop.FindPropertyRelative("Frequency");

            drawelement_rect.Set(rect.x + 15, rect.y, 30, 20);
            Editor_XHud_GUI.Gui_Labelfield(drawelement_rect, index.ToString("D2"), HudFilled.无, HudColor.无, Color.gray, TextAnchor.MiddleLeft, Vector2.zero, 11);

            BlockGUI(sp_name.stringValue);

            drawelement_rect.Set(rect.x + 35, titleheight, rect.width - 200, LineHeight);
            Editor_XHud_GUI.Gui_Property_Field(drawelement_rect, "", sp_name, 0, 30);

            BlockGUI(sp_name.stringValue);

            drawelement_rect.Set(rect.width - 120, titleheight, 90, LineHeight);
            Editor_XHud_GUI.Gui_Property_Field(drawelement_rect, "", sp_clip, 10, 40);

            BlockGUI(sp_name.stringValue);

            string cxs = sp_length.floatValue.ToString("F2") + " s";
            string xxc = "";
            if (BaseScript.SoundLibrary[index].Channel == 2)
            {
                xxc = sp_chan.intValue + " ch (Stereo)";
            }
            else
            {
                xxc = sp_chan.intValue + " ch (Mono)";
            }
            string fsr = sp_freq.intValue + " hz";

            string res = cxs + " | " + xxc + " | " + fsr;

            drawelement_rect.Set(rect.x + 36, rect.y + 28, rect.width - 10, LineHeight);
            Editor_XHud_GUI.Gui_Labelfield(drawelement_rect, res, HudFilled.无, HudColor.无, Color.gray, TextAnchor.MiddleLeft, Vector2.zero, 11);

            GUI.enabled = true;

            if (!sp_name.stringValue.Contains(sp_Highlight.stringValue))
            {
                drawelement_rect.Set(rect.x - 20, rect.y, rect.width + 20, rect.height);
                EditorGUI.DrawRect(drawelement_rect, blocked_col);
            }

            if (index == sp_LocationSelectedIndex.intValue)
            {
                drawelement_rect.Set(rect.width - 80, rect.y + 25, 50, 20);
                Editor_XHud_GUI.Gui_Labelfield(drawelement_rect, "已定位", HudFilled.无, HudColor.亮白, XHud_Dashboard.Theme_Primary, TextAnchor.MiddleRight, Vector2.zero, 11);
            }
        }

        /// <summary>
        /// 绘制列表
        /// </summary>
        private void DrawSoundInfoList_Original()
        {
            // 绘制滚动视图
            scrollview_rect = GUILayoutUtility.GetRect(0, sp_visibleItemCount.intValue * sp_itemHeight.floatValue);
            sp_SoundInfoList_Original_Scroller.vector2Value = GUI.BeginScrollView(scrollview_rect, sp_SoundInfoList_Original_Scroller.vector2Value, new Rect(0, 0, scrollview_rect.width - 50, SoundInfoList.count * sp_itemHeight.floatValue), false, true);

            // 计算可视区域的起始和结束索引
            int startIndex = Mathf.FloorToInt(sp_SoundInfoList_Original_Scroller.vector2Value.y / sp_itemHeight.floatValue);
            int endIndex = Mathf.CeilToInt((sp_SoundInfoList_Original_Scroller.vector2Value.y + scrollview_rect.height) / sp_itemHeight.floatValue);

            // 只绘制可视区域内的元素
            for (int i = startIndex; i < endIndex && i < SoundInfoList.count; i++)
            {
                SerializedProperty prop = sp_SoundList.GetArrayElementAtIndex(i);
                SerializedProperty sp_color = prop.FindPropertyRelative("Color");
                SerializedProperty sp_name = prop.FindPropertyRelative("Name");

                item_rect.Set(0, i * sp_itemHeight.floatValue, scrollview_rect.width, sp_itemHeight.floatValue);

                // 如果当前元素被选中，绘制选中效果
                if (sp_name.stringValue.Contains(sp_Highlight.stringValue))
                {
                    if (sp_SelectedIndex.intValue == i)
                    {
                        item_rect.Set(1, i * sp_itemHeight.floatValue + 6, 5, 5);
                        // 高亮标记表示选中
                        EditorGUI.DrawRect(item_rect, XHud_Dashboard.Theme_Primary);
                        item_rect.Set(0, i * sp_itemHeight.floatValue, scrollview_rect.width, sp_itemHeight.floatValue);
                        // 高亮背景表示选中
                        EditorGUI.DrawRect(item_rect, SelectedBg);
                    }
                }

                item_rect.Set(0, i * sp_itemHeight.floatValue, scrollview_rect.width, sp_itemHeight.floatValue);
                SoundInfoList.drawElementCallback.Invoke(item_rect, i, i == SoundInfoList.index, true);

                // 检测鼠标是否在当前元素区域内
                if (item_rect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.MouseDown)
                    {
                        if (sp_name.stringValue.Contains(sp_Highlight.stringValue))
                        {
                            // 更新选中项
                            sp_SelectedIndex.intValue = i;

                            if (!Application.isPlaying)
                            {
                                PreviewSound(BaseScript.SoundLibrary[i].Clip);
                            }
                            // 标记界面需要更新
                            GUI.changed = true;
                        }
                    }
                }

                // 绘制元素
                EditorGUI.PropertyField(item_rect, prop, GUIContent.none);
            }

            GUI.EndScrollView();
        }

        /// <summary>
        /// 列表移除
        /// </summary>
        /// <param name="list"></param>
        private void SoundInfoList_Original_Remove(ReorderableList list)
        {
            if (list.count <= 0)
                return;

            sp_SoundList.DeleteArrayElementAtIndex(list.index);

            if (BaseScript.act_on_SoundRemoved != null)
                BaseScript.act_on_SoundRemoved();
        }

        /// <summary>
        /// 列表增加
        /// </summary>
        /// <param name="list"></param>
        private void SoundInfoList_Original_Add(ReorderableList list)
        {
            IsObjectSelector = true;
            EditorGUIUtility.ShowObjectPicker<AudioClip>(SelectedClip, false, "", 0);
        }
        #endregion

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            Editor_XHud_GUI.Gui_Layout_Banner(HudFilled.实体, HudColor.亮白, "XHud - 音效库", Color.black);


            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_XHud_GUI.Gui_Layout_Property_Field("音效库名称", LibraryName);
            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Horizontal_End();

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            EditorGUI.BeginChangeCheck();
            Editor_XHud_GUI.Gui_Layout_Property_Field("过滤（包含）", sp_Highlight);
            if (EditorGUI.EndChangeCheck())
            {
                sp_LocationSelectedIndex.intValue = -1;
                sp_LocationSelectedIndex.serializedObject.ApplyModifiedProperties();
            }
            Editor_XHud_GUI.Gui_Layout_Space(5);
            EditorGUI.BeginChangeCheck();
            Editor_XHud_GUI.Gui_Layout_Property_Field("查找（精确）", sp_Find);
            if (EditorGUI.EndChangeCheck())
            {
                if (!string.IsNullOrEmpty(sp_Find.stringValue))
                    BaseScript.SoundLibrary_Location_Find(sp_Find.stringValue);
                else
                {
                    sp_SelectedIndex.intValue = -1;
                    sp_SelectedIndex.serializedObject.ApplyModifiedProperties();

                    sp_LocationSelectedIndex.intValue = -1;
                    sp_LocationSelectedIndex.serializedObject.ApplyModifiedProperties();
                }
            }
            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Horizontal_End();

            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Seperator(1, XHud_Dashboard.Theme_SeperateLine);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            if (string.IsNullOrEmpty(sp_Highlight.stringValue))
            {
                if (!Application.isPlaying)
                {
                    Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    GUILayout.FlexibleSpace();
                    if (BaseScript.SoundLibrary != null && BaseScript.SoundLibrary.Count > 0)
                        GUI.enabled = true;
                    else
                        GUI.enabled = false;
                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "扫描取所有音效参数", scan_r, scan_p, 4))
                    {
                        if (Application.isPlaying)
                            return;
                        for (int i = 0; i < sp_SoundList.arraySize; i++)
                        {
                            SerializedProperty item = sp_SoundList.GetArrayElementAtIndex(i);
                            SerializedProperty x_Clip = item.FindPropertyRelative("Clip");
                            SerializedProperty x_Frequency = item.FindPropertyRelative("Frequency");
                            SerializedProperty x_Channel = item.FindPropertyRelative("Channel");
                            SerializedProperty x_Length = item.FindPropertyRelative("Length");

                            if (x_Clip.objectReferenceValue != null)
                            {
                                AudioClip ac = (AudioClip)x_Clip.objectReferenceValue;

                                x_Channel.intValue = ac.channels;
                                x_Frequency.intValue = ac.frequency;
                                x_Length.floatValue = ac.length;

                                x_Length.serializedObject.ApplyModifiedProperties();
                                x_Frequency.serializedObject.ApplyModifiedProperties();
                                x_Channel.serializedObject.ApplyModifiedProperties();
                                item.serializedObject.ApplyModifiedProperties();
                            }
                        }
                    }
                    GUI.enabled = true;
                    GUILayout.Space(35);
                    if (BaseScript.SoundLibrary != null && BaseScript.SoundLibrary.Count > 0)
                        GUI.enabled = true;
                    else
                        GUI.enabled = false;
                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "停止并删除所有预览音效", stoppreview_r, stoppreview_p, 4))
                    {
                        if (Application.isPlaying)
                            return;
                        RecycleAudio();
                    }
                    GUI.enabled = true;
                    Editor_XHud_GUI.Gui_Layout_Space(35);
                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "移除所有音效项", clear_r, clear_p, 4))
                    {
                        string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 音效库消息", "清空所有音效项", "是否确认要清空当前音效库的所有音效项？", "清空", "暂不", 1);
                        if (res == "暂不")
                        {
                            return;
                        }

                        sp_SoundList.ClearArray();
                        sp_SoundList.serializedObject.ApplyModifiedProperties();
                    }
                    Editor_XHud_GUI.Gui_Layout_Space(35);
                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "添加项", create_r, create_p, 4))
                    {
                        SoundInfoList_Original_Add(SoundInfoList);
                    }
                    Editor_XHud_GUI.Gui_Layout_Space(35);
                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "删除项", delete_r, delete_p, 4))
                    {
                        SoundInfoList_Original_Remove(SoundInfoList);
                    }
                    GUILayout.FlexibleSpace();
                    Editor_XHud_GUI.Gui_Layout_Horizontal_End();

                    Editor_XHud_GUI.Gui_Layout_Space(5);
                    Editor_XHud_GUI.Gui_Layout_Seperator(1, XHud_Dashboard.Theme_SeperateLine);
                    x_dragarea = GUILayoutUtility.GetLastRect();
                    Editor_XHud_GUI.Gui_Layout_Space(10);

                    #region 拖放操作
                    dragarea.Set((x_dragarea.width / 2) - 65, x_dragarea.y + 10, 200, 33);

                    Event evt = Event.current;
                    if (dragarea.Contains(evt.mousePosition))
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                        if (evt.type == EventType.DragPerform)
                        {
                            DragAndDrop.AcceptDrag();
                            Object[] dropobjs = DragAndDrop.objectReferences;
                            string names = "";
                            foreach (var item in dropobjs)
                            {
                                XHud_LibraryArg_Sound it = new XHud_LibraryArg_Sound();
                                it.Clip = (AudioClip)item;
                                if (it.Clip != null)
                                {
                                    bool isExist = false;
                                    for (int s = 0; s < sp_SoundList.arraySize; s++)
                                    {
                                        SerializedProperty ser = sp_SoundList.GetArrayElementAtIndex(s);
                                        SerializedProperty ser_target = ser.FindPropertyRelative("Clip");
                                        if (ser_target.objectReferenceValue.name == it.Clip.name)
                                        {
                                            isExist = true;
                                            break;
                                        }
                                    }
                                    if (isExist)
                                    {
                                        Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 音效库消息", "重复添加音效", $"此音效已存在于该库中！请勿重复添加！重复音效名称： {it.Clip.name}", "明白");
                                        continue;
                                    }
                                    else
                                    {
                                        it.Name = it.Clip.name;

                                        sp_SoundList.InsertArrayElementAtIndex(sp_SoundList.arraySize);
                                        SerializedProperty root = sp_SoundList.GetArrayElementAtIndex(sp_SoundList.arraySize - 1);
                                        SerializedProperty x_Clip = root.FindPropertyRelative("Clip");
                                        SerializedProperty x_Name = root.FindPropertyRelative("Name");
                                        SerializedProperty x_Frequency = root.FindPropertyRelative("Frequency");
                                        SerializedProperty x_Channel = root.FindPropertyRelative("Channel");
                                        SerializedProperty x_Length = root.FindPropertyRelative("Length");

                                        x_Clip.objectReferenceValue = it.Clip;
                                        x_Name.stringValue = it.Name;
                                        x_Channel.intValue = it.Clip.channels;
                                        x_Frequency.intValue = it.Clip.frequency;
                                        x_Length.floatValue = it.Clip.length;

                                        root.serializedObject.ApplyModifiedProperties();

                                        names += x_Name.stringValue + " / ";
                                    }
                                }
                            }

                            if (!string.IsNullOrEmpty(names))
                                Editor_XHud_GUI.Open(XHud_DialogType.确认, "XHud - 音效库消息", "添加音效完成", $"所选有效音效 : {names}已添加到音效库中！", "明白");
                        }
                    }

                    Editor_XHud_GUI.Gui_Box_Style(dragarea, HudFilled.实体, HudColor.深空灰);
                    Editor_XHud_GUI.Gui_Labelfield(dragarea, "拖放音效剪辑到此处快速入库", HudFilled.无, HudColor.无, Color.gray, TextAnchor.MiddleCenter, new Vector2(0, 0), 11);
                    #endregion
                }
            }
            else
            {
                Editor_XHud_GUI.Gui_Layout_LabelfieldThin("当前为过滤筛选状态", HudFilled.无, HudColor.无, XHud_Dashboard.Theme_Primary, TextAnchor.MiddleCenter, new Vector2(0, 0), 12);
            }

            Editor_XHud_GUI.Gui_Layout_Space(30);
            Editor_XHud_GUI.Gui_Layout_Seperator(1, XHud_Dashboard.Theme_SeperateLine);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Library_Sounds>("随机音高", stroptions_enabled, ref sp_UseRandomPitch, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Horizontal_End();

            Editor_XHud_GUI.Gui_Layout_Space(10);

            if (sp_UseRandomPitch.boolValue)
            {
                Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_XHud_GUI.Gui_Layout_Property_Field("最小音高", sp_Pitch_Min);
                Editor_XHud_GUI.Gui_Layout_Space(10);
                Editor_XHud_GUI.Gui_Layout_Property_Field("最大音高", sp_Pitch_Max);
                Editor_XHud_GUI.Gui_Layout_Space(10);
                Editor_XHud_GUI.Gui_Layout_Horizontal_End();
            }
            sp_UseRandomPitch.serializedObject.ApplyModifiedProperties();

            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "音效列表", Color.white);

            Editor_XHud_GUI.Gui_Layout_Space(5);

            DrawSoundInfoList_Original();

            ///------------选择音效
            commandName = Event.current.commandName;
            if (commandName == "ObjectSelectorUpdated")
            {
                UpdateSoundParams();
                if (IsObjectSelectorUpdated)
                {
                    if (BaseScript.act_on_SoundChanged != null)
                        BaseScript.act_on_SoundChanged();
                }
                IsObjectSelectorUpdated = false;
                selectobject = false;
            }
            else if (commandName == "ObjectSelectorClosed")
            {
                if (!IsObjectSelector)
                    return;

                Object obj = EditorGUIUtility.GetObjectPickerObject();

                if (obj != null)
                {
                    if (!selectobject)
                    {
                        IsObjectSelector = false;
                        selectobject = true;
                        if (BaseScript.SoundLibrary == null)
                            BaseScript.SoundLibrary = new List<XHud_LibraryArg_Sound>();
                        AudioClip clip = obj as AudioClip;
                        BaseScript.SoundLibrary.Add(new XHud_LibraryArg_Sound(clip.name, clip));
                        if (BaseScript.act_on_SoundAdded != null)
                            BaseScript.act_on_SoundAdded(clip.name, clip);
                    }
                }
            }
            else
            {
                IsObjectSelectorUpdated = true;
            }

            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();

            serializedObject.ApplyModifiedProperties();
        }

        #region 辅助
        private void UpdateSoundParams()
        {
            if (BaseScript.SoundLibrary != null && BaseScript.SoundLibrary.Count > 0)
            {
                for (int i = 0; i < BaseScript.SoundLibrary.Count; i++)
                {
                    if (BaseScript.SoundLibrary[i].Clip != null)
                    {
                        BaseScript.SoundLibrary[i].Channel = BaseScript.SoundLibrary[i].Clip.channels;
                        BaseScript.SoundLibrary[i].Frequency = BaseScript.SoundLibrary[i].Clip.frequency;
                        BaseScript.SoundLibrary[i].Length = BaseScript.SoundLibrary[i].Clip.length;
                    }
                }
            }
        }

        private void EditorApplication_playModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                RecycleAudio();

                for (int i = 0; i < PreviewAudioList.Count; i++)
                {
                    if (PreviewAudioList[i] != null)
                    {
                        DestroyImmediate(PreviewAudioList[i].gameObject, true);
                        PreviewAudioList[i] = null;
                        PreviewAudioList.RemoveAt(i);
                    }
                }
                PreviewAudioList.Clear();
            }
        }

        /// <summary>
        /// 回收预览音效
        /// </summary>
        private void RecycleAudio()
        {
            if (PreviewAudioList != null)
            {
                for (int i = 0; i < PreviewAudioList.Count; i++)
                {
                    if (PreviewAudioList[i] != null)
                    {
                        PreviewAudioList[i].Stop();
                    }
                }
            }
        }

        /// <summary>
        /// 预览声音
        /// </summary>
        /// <param name="info"></param>
        private void PreviewSound(AudioClip clip)
        {
            GameObject obj = new GameObject();
            obj.name = "SoundPreviewer-" + "[" + clip.length.ToString("F2") + " s]-" + "[" + clip.channels + " ch]-" + "[" + clip.frequency + " hz]";
            AudioSource au = obj.AddComponent<AudioSource>();

            if (PreviewAudioList == null)
                PreviewAudioList = new List<AudioSource>();
            PreviewAudioList.Add(au);

            au.clip = clip;
            if (sp_UseRandomPitch.boolValue)
                au.pitch = Random.Range(sp_Pitch_Min.floatValue, sp_Pitch_Max.floatValue);
            else
                au.pitch = 1f;
            au.Play();
            XHud_AudioStoper sp = au.gameObject.AddComponent<XHud_AudioStoper>();
            sp.AudioSource = au;
        }
        #endregion
    }
}