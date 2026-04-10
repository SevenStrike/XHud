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
    using System.Collections.Generic;
    using Unity.EditorCoroutines.Editor;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    public partial class Editor_XHud_Module_Element : Editor
    {
        private ReorderableList
            PrimitivesTweenList,
            SounderList,
            ButtonList,
            OptionList,
            SliderList,
            ProgressList,
            ToggleList,
            TextList,
            TmpTextList;

        public void ReorderableList_Draw_Sounder()
        {
            SounderList = new ReorderableList(serializedObject, SounderNodes)
            {
                displayAdd = false,
                displayRemove = true,
                draggable = true,

                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "元素音效列表");
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    float titleheight = rect.y + 6;
                    float baseheight = rect.y + (rect.height - 25);

                    SerializedProperty sp_root = SounderNodes.GetArrayElementAtIndex(index);
                    if (sp_root != null)
                    {
                        SerializedProperty sp_sounder = sp_root.FindPropertyRelative("Sounder");
                        XHud_Element_Sounder sounder = (XHud_Element_Sounder)sp_sounder.objectReferenceValue;
                        if (sounder != null)
                        {
                            string title = "";
                            if (!string.IsNullOrEmpty(sounder.Indicator))
                                title += sounder.Indicator;
                            else
                                title += sounder.gameObject.name;
                            Editor_XHud_GUI.Gui_Labelfield(new Rect(rect.width - (rect.width - 90), titleheight - 2, (rect.width * 0.45f) - 20, LineHeight), title, HudFilled.无, HudColor.亮白, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 11, TextClipping.Clip);

                            GUI.color = XHud_Dashboard.Theme_Primary;
                            Editor_XHud_GUI.Gui_Icon(new Rect(rect.width - (rect.width - 60), titleheight, 10, 10), icon_sound);

                            SerializedObject so_sounder = new SerializedObject(sounder);

                            SerializedProperty sp_PitchMin = so_sounder.FindProperty("Pitch_Min");
                            SerializedProperty sp_PitchMax = so_sounder.FindProperty("Pitch_Max");
                            SerializedProperty sp_Delay = so_sounder.FindProperty("DelayTime");
                            SerializedProperty sp_Vol = so_sounder.FindProperty("Volume");
                            SerializedProperty sp_SoundName = so_sounder.FindProperty("SoundName");

                            so_sounder.Update();

                            GUI.color = Color.white;

                            #region 最小音高
                            Editor_XHud_GUI.Gui_Property_Field(new Rect(rect.width - 40, rect.y + 4, 30, 19), "音高", sp_PitchMin, 0, 60, LineHeight, 30);
                            #endregion

                            #region 最大音高                         
                            Editor_XHud_GUI.Gui_Property_Field(new Rect(rect.width + 25, rect.y + 4, 30, 19), "", sp_PitchMax, 0, 25, LineHeight, 5);
                            #endregion

                            #region 音量          
                            float fieldwidth = EditorGUIUtility.fieldWidth;
                            EditorGUIUtility.fieldWidth = 40;
                            EditorGUI.Slider(new Rect(rect.x + 5, rect.y + 30, 120, 19), sp_Vol, 0, 1, "");
                            EditorGUIUtility.fieldWidth = fieldwidth;
                            so_sounder.ApplyModifiedProperties();
                            #endregion

                            #region 延迟                         
                            Editor_XHud_GUI.Gui_Property_Field(new Rect(rect.width - 40, rect.y + 30, 30, 19), "延迟", sp_Delay, 0, 60, LineHeight, 30);
                            #endregion

                            #region 音效列表
                            if (HudManager.Hud_Sounds != null)
                            {
                                string[] collist = HudManager.Hud_Sounds.SoundLibrary_GetSoundNames();
                                Color bgcol = GUI.color;
                                GUI.color = XHud_Dashboard.Theme_Primary;
                                Editor_XHud_GUI.Gui_PopupWithString(new Rect(rect.width + 25, rect.y + 30, 30, 19), ref sp_SoundName, collist, HudFilled.实体, HudColor.亮白, Color.black);
                                GUI.color = bgcol;
                            }
                            #endregion

                            so_sounder.ApplyModifiedProperties();
                        }
                    }
                },
                onSelectCallback = (ReorderableList list) =>
                {
                    SerializedProperty sp_sounder = SounderNodes.GetArrayElementAtIndex(list.index).FindPropertyRelative("Sounder");
                    EditorGUIUtility.PingObject(sp_sounder.objectReferenceValue);

                    #region 播放音效
                    SerializedObject so_sod = new SerializedObject((XHud_Element_Sounder)sp_sounder.objectReferenceValue);
                    SerializedProperty sp_name = so_sod.FindProperty("SoundName");
                    SerializedProperty sp_vol = so_sod.FindProperty("Volume");
                    SerializedProperty sp_delay = so_sod.FindProperty("DelayTime");
                    SerializedProperty sp_pit_min = so_sod.FindProperty("Pitch_Min");
                    SerializedProperty sp_pit_max = so_sod.FindProperty("Pitch_Max");
                    SerializedProperty sp_userandom = so_sod.FindProperty("UseRandomPitch");

                    float x_vol = sp_vol.floatValue;
                    float x_pit_min = sp_pit_min.floatValue;
                    float x_pit_max = sp_pit_max.floatValue;
                    float x_delay = sp_delay.floatValue;
                    bool x_userandom = false;
                    if (sp_userandom.intValue == 1)
                        x_userandom = true;

                    AudioClip x_clip = HudManager.Hud_Sounds.SoundLibrary_GetSound(sp_name.stringValue);
                    Preivew_HudSounder_CoroutineList_Stop.Add(EditorCoroutineUtility.StartCoroutineOwnerless(Preview_XHudSounder_Play(x_vol, x_pit_min, x_pit_max, x_userandom, x_clip, x_delay)));
                    #endregion
                },
                elementHeightCallback = index =>
                {
                    return 2.8f * LineHeight;
                }
            };
        }
        public void ReorderableList_Draw_PrimitiveControllerNodes()
        {
            PrimitivesTweenList = new ReorderableList(serializedObject, PrimitiveControllerNodes)
            {
                displayAdd = false,
                displayRemove = true,
                draggable = true,

                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "图元控制器列表");
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    if (PrimitiveControllerNodes == null)
                        return;
                    if (PrimitiveControllerNodes.arraySize <= 0)
                        return;
                    SerializedProperty sp_node = PrimitiveControllerNodes.GetArrayElementAtIndex(index);
                    if (sp_node != null)
                    {
                        SerializedProperty sp_node_con = sp_node.FindPropertyRelative("Controller");
                        SerializedProperty sp_delay = sp_node.FindPropertyRelative("DelayTime");
                        XHud_Module_Primitive_Controller sp_con = (XHud_Module_Primitive_Controller)sp_node_con.objectReferenceValue;

                        if (sp_con != null)
                        {
                            float titleheight = rect.y + 7;
                            float baseheight = rect.y + 25;

                            #region 类型图标         
                            GUI.color = XHud_Dashboard.Theme_Primary;
                            Editor_XHud_GUI.Gui_Icon(new Rect(rect.width - (rect.width - 60), titleheight + 1, 10, 10), icon_anim);
                            GUI.color = Color.white;
                            #endregion

                            #region 标题文字
                            string title = "";
                            string indicator = sp_con.GetIndicator();
                            if (!string.IsNullOrEmpty(indicator))
                                title += indicator;
                            else
                                title += sp_con.gameObject.name;
                            Editor_XHud_GUI.Gui_Labelfield(new Rect(rect.width - (rect.width - 90), titleheight - 2, (rect.width * 0.45f) - 20, LineHeight), title, HudFilled.无, HudColor.亮白, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 11, TextClipping.Clip);
                            #endregion

                            #region 延迟
                            Editor_XHud_GUI.Gui_Property_Field(new Rect(rect.width - 5, rect.y + 4, 30, 19), "D", sp_delay, 10, 40, LineHeight, 15);
                            #endregion

                            #region 速率                         
                            SerializedObject so_tween = new SerializedObject(sp_con.pt_Tween);
                            so_tween.Update();

                            SerializedProperty sp_glodur = so_tween.FindProperty("GlobalDuration");
                            SerializedProperty sp_maxdur = so_tween.FindProperty("MaxTimerWithGlobalDuration");

                            Editor_XHud_GUI.Gui_Property_Field(new Rect(rect.width - 50, rect.y + 4, 30, 19), "G", sp_glodur, 10, 40, LineHeight, 15);

                            Editor_XHud_GUI.Gui_Labelfield_Thin(new Rect(rect.width - 80, rect.y + 4, 30, 19), $"{sp_maxdur.floatValue.ToString()} s", HudFilled.无, HudColor.无, XHud_Dashboard.Theme_Primary, TextAnchor.MiddleCenter, Vector2.zero, 11);

                            so_tween.ApplyModifiedProperties();
                            #endregion

                            sp_node_con.serializedObject.ApplyModifiedProperties();
                            sp_node.serializedObject.ApplyModifiedProperties();
                        }
                    }
                },
                onSelectCallback = (ReorderableList list) =>
                {
                    SerializedProperty sp_node = PrimitiveControllerNodes.GetArrayElementAtIndex(list.index);
                    if (sp_node != null)
                    {
                        SerializedProperty sp_node_con = sp_node.FindPropertyRelative("Controller");
                        EditorGUIUtility.PingObject(sp_node_con.objectReferenceValue);
                    }
                },
                elementHeightCallback = index =>
                {
                    return 1.5f * LineHeight;
                }
            };
        }
        public void ReorderableList_Draw_Button()
        {
            ButtonList = new ReorderableList(serializedObject, ButtonNodes)
            {
                displayAdd = false,
                displayRemove = true,
                draggable = true,

                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "按钮列表");
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    float titleheight = rect.y + 7;
                    float baseheight = rect.y + (rect.height - 25);

                    SerializedProperty sp_root = ButtonNodes.GetArrayElementAtIndex(index);
                    if (sp_root != null)
                    {
                        SerializedProperty sp_btn = sp_root.FindPropertyRelative("Button");
                        XHud_Module_Button btn = (XHud_Module_Button)sp_btn.objectReferenceValue;
                        if (btn != null)
                        {
                            string title = "";
                            if (!string.IsNullOrEmpty(btn.Indicator))
                                title += btn.Indicator;
                            else
                                title += btn.gameObject.name;
                            Editor_XHud_GUI.Gui_Labelfield(new Rect(rect.width - (rect.width - 90), titleheight - 2, (rect.width * 0.45f) - 20, LineHeight), title, HudFilled.无, HudColor.亮白, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 11, TextClipping.Clip);

                            GUI.color = XHud_Dashboard.Theme_Primary;
                            Editor_XHud_GUI.Gui_Icon(new Rect(rect.width - (rect.width - 60), titleheight + 1, 10, 10), icon_button);

                            GUI.color = Color.white;
                        }
                    }
                },
                onSelectCallback = (ReorderableList list) =>
                {
                    SerializedProperty sp_btn = ButtonNodes.GetArrayElementAtIndex(list.index).FindPropertyRelative("Button");
                    //Hud_Button btn = (Hud_Button)sp_btn.objectReferenceValue;
                    EditorGUIUtility.PingObject(sp_btn.objectReferenceValue);
                },
                elementHeightCallback = index =>
                {
                    return 1.5f * LineHeight;
                }
            };
        }
        public void ReorderableList_Draw_Option()
        {
            OptionList = new ReorderableList(serializedObject, OptionNodes)
            {
                displayAdd = false,
                displayRemove = true,
                draggable = true,

                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "选项列表");
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    float titleheight = rect.y + 7;
                    float baseheight = rect.y + (rect.height - 25);

                    SerializedProperty sp_root = OptionNodes.GetArrayElementAtIndex(index);
                    if (sp_root != null)
                    {
                        SerializedProperty sp_opt = sp_root.FindPropertyRelative("Option");
                        XHud_Module_Option opt = (XHud_Module_Option)sp_opt.objectReferenceValue;
                        if (opt != null)
                        {
                            string title = "";
                            if (!string.IsNullOrEmpty(opt.Indicator))
                                title += opt.Indicator;
                            else
                                title += opt.gameObject.name;
                            Editor_XHud_GUI.Gui_Labelfield(new Rect(rect.width - (rect.width - 90), titleheight - 2, (rect.width * 0.45f) - 20, LineHeight), title, HudFilled.无, HudColor.亮白, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 11, TextClipping.Clip);

                            GUI.color = XHud_Dashboard.Theme_Primary;
                            Editor_XHud_GUI.Gui_Icon(new Rect(rect.width - (rect.width - 60), titleheight + 1, 10, 10), icon_option);

                            GUI.color = Color.white;
                        }
                    }
                },
                onSelectCallback = (ReorderableList list) =>
                {
                    SerializedProperty sp_opt = OptionNodes.GetArrayElementAtIndex(list.index).FindPropertyRelative("Option");
                    EditorGUIUtility.PingObject(sp_opt.objectReferenceValue);
                },
                elementHeightCallback = index =>
                {
                    return 1.5f * LineHeight;
                }
            };
        }
        public void ReorderableList_Draw_Slider()
        {
            SliderList = new ReorderableList(serializedObject, SliderNodes)
            {
                displayAdd = false,
                displayRemove = true,
                draggable = true,

                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "滑动条列表");
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    float titleheight = rect.y + 7;
                    float baseheight = rect.y + (rect.height - 25);

                    SerializedProperty sp_root = SliderNodes.GetArrayElementAtIndex(index);
                    if (sp_root != null)
                    {
                        SerializedProperty sp_sli = sp_root.FindPropertyRelative("Slider");
                        XHud_Module_Slider sli = (XHud_Module_Slider)sp_sli.objectReferenceValue;
                        if (sli != null)
                        {
                            string title = "";
                            if (!string.IsNullOrEmpty(sli.Indicator))
                                title += sli.Indicator;
                            else
                                title += sli.gameObject.name;
                            Editor_XHud_GUI.Gui_Labelfield(new Rect(rect.width - (rect.width - 90), titleheight - 2, (rect.width * 0.45f) - 20, LineHeight), title, HudFilled.无, HudColor.亮白, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 11, TextClipping.Clip);

                            GUI.color = XHud_Dashboard.Theme_Primary;
                            Editor_XHud_GUI.Gui_Icon(new Rect(rect.width - (rect.width - 60), titleheight + 1, 10, 10), icon_slider);

                            GUI.color = Color.white;
                        }
                    }
                },
                onSelectCallback = (ReorderableList list) =>
                {
                    SerializedProperty sp_opt = SliderNodes.GetArrayElementAtIndex(list.index).FindPropertyRelative("Slider");
                    EditorGUIUtility.PingObject(sp_opt.objectReferenceValue);
                },
                elementHeightCallback = index =>
                {
                    return 1.5f * LineHeight;
                }
            };
        }
        public void ReorderableList_Draw_Progress()
        {
            ProgressList = new ReorderableList(serializedObject, ProgressNodes)
            {
                displayAdd = false,
                displayRemove = true,
                draggable = true,

                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "进度条列表");
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    float titleheight = rect.y + 7;
                    float baseheight = rect.y + (rect.height - 25);

                    SerializedProperty sp_root = ProgressNodes.GetArrayElementAtIndex(index);
                    if (sp_root != null)
                    {
                        SerializedProperty sp_pro = sp_root.FindPropertyRelative("Progress");
                        XHud_Module_Progress pro = (XHud_Module_Progress)sp_pro.objectReferenceValue;
                        if (pro != null)
                        {
                            string title = "";
                            if (!string.IsNullOrEmpty(pro.Indicator))
                                title += pro.Indicator;
                            else
                                title += pro.gameObject.name;
                            Editor_XHud_GUI.Gui_Labelfield(new Rect(rect.width - (rect.width - 90), titleheight - 2, (rect.width * 0.45f) - 20, LineHeight), title, HudFilled.无, HudColor.亮白, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 11, TextClipping.Clip);

                            GUI.color = XHud_Dashboard.Theme_Primary;
                            Editor_XHud_GUI.Gui_Icon(new Rect(rect.width - (rect.width - 60), titleheight + 1, 10, 10), icon_progress);

                            GUI.color = Color.white;
                        }
                    }
                },
                onSelectCallback = (ReorderableList list) =>
                {
                    SerializedProperty sp_opt = ProgressNodes.GetArrayElementAtIndex(list.index).FindPropertyRelative("Progress");
                    EditorGUIUtility.PingObject(sp_opt.objectReferenceValue);
                },
                elementHeightCallback = index =>
                {
                    return 1.5f * LineHeight;
                }
            };
        }
        public void ReorderableList_Draw_Toggle()
        {
            ToggleList = new ReorderableList(serializedObject, ToggleNodes)
            {
                displayAdd = false,
                displayRemove = true,
                draggable = true,

                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "开关列表");
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    float titleheight = rect.y + 7;
                    float baseheight = rect.y + (rect.height - 25);

                    SerializedProperty sp_root = ToggleNodes.GetArrayElementAtIndex(index);
                    if (sp_root != null)
                    {
                        SerializedProperty sp_tog = sp_root.FindPropertyRelative("Toggle");
                        XHud_Module_Toggle tog = (XHud_Module_Toggle)sp_tog.objectReferenceValue;
                        if (tog != null)
                        {
                            string title = "";
                            if (!string.IsNullOrEmpty(tog.Indicator))
                                title += tog.Indicator;
                            else
                                title += tog.gameObject.name;
                            Editor_XHud_GUI.Gui_Labelfield(new Rect(rect.width - (rect.width - 90), titleheight - 2, (rect.width * 0.45f) - 20, LineHeight), title, HudFilled.无, HudColor.亮白, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 11, TextClipping.Clip);

                            GUI.color = XHud_Dashboard.Theme_Primary;
                            Editor_XHud_GUI.Gui_Icon(new Rect(rect.width - (rect.width - 60), titleheight + 1, 10, 10), icon_toggle);

                            GUI.color = Color.white;
                        }
                    }
                },
                onSelectCallback = (ReorderableList list) =>
                {
                    SerializedProperty sp_opt = ToggleNodes.GetArrayElementAtIndex(list.index).FindPropertyRelative("Toggle");
                    EditorGUIUtility.PingObject(sp_opt.objectReferenceValue);
                },
                elementHeightCallback = index =>
                {
                    return 1.5f * LineHeight;
                }
            };
        }
        public void ReorderableList_Draw_Text()
        {
            TextList = new ReorderableList(serializedObject, TextNodes)
            {
                displayAdd = false,
                displayRemove = true,
                draggable = true,

                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "文字列表");
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    float titleheight = rect.y + 7;
                    float baseheight = rect.y + (rect.height - 25);

                    SerializedProperty sp_root = TextNodes.GetArrayElementAtIndex(index);
                    if (sp_root != null)
                    {
                        SerializedProperty sp_text = sp_root.FindPropertyRelative("Text");
                        XHud_Module_Text txt = (XHud_Module_Text)sp_text.objectReferenceValue;
                        if (txt != null)
                        {
                            string title = "";
                            if (!string.IsNullOrEmpty(txt.Indicator))
                                title += txt.Indicator;
                            else
                                title += txt.gameObject.name;
                            Editor_XHud_GUI.Gui_Labelfield(new Rect(rect.width - (rect.width - 90), titleheight - 2, (rect.width * 0.45f) - 20, LineHeight), title, HudFilled.无, HudColor.亮白, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 11, TextClipping.Clip);

                            GUI.color = XHud_Dashboard.Theme_Primary;
                            Editor_XHud_GUI.Gui_Icon(new Rect(rect.width - (rect.width - 60), titleheight + 1, 10, 10), icon_text);

                            GUI.color = Color.white;
                        }
                    }
                },
                onSelectCallback = (ReorderableList list) =>
                {
                    SerializedProperty sp_text = TextNodes.GetArrayElementAtIndex(list.index).FindPropertyRelative("Text");
                    EditorGUIUtility.PingObject(sp_text.objectReferenceValue);
                },
                elementHeightCallback = index =>
                {
                    return 1.5f * LineHeight;
                }
            };
        }
        public void ReorderableList_Draw_TmpText()
        {
            TmpTextList = new ReorderableList(serializedObject, TmpTextNodes)
            {
                displayAdd = false,
                displayRemove = true,
                draggable = true,

                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "Tmp文字列表");
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    float titleheight = rect.y + 7;
                    float baseheight = rect.y + (rect.height - 25);

                    SerializedProperty sp_root = TmpTextNodes.GetArrayElementAtIndex(index);
                    if (sp_root != null)
                    {
                        SerializedProperty sp_text = sp_root.FindPropertyRelative("TmpText");
                        XHud_Module_TmpText txt = (XHud_Module_TmpText)sp_text.objectReferenceValue;
                        if (txt != null)
                        {
                            string title = "";
                            if (!string.IsNullOrEmpty(txt.Indicator))
                                title += txt.Indicator;
                            else
                                title += txt.gameObject.name;
                            Editor_XHud_GUI.Gui_Labelfield(new Rect(rect.width - (rect.width - 90), titleheight - 2, (rect.width * 0.45f) - 20, LineHeight), title, HudFilled.无, HudColor.亮白, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 11, TextClipping.Clip);

                            GUI.color = XHud_Dashboard.Theme_Primary;
                            Editor_XHud_GUI.Gui_Icon(new Rect(rect.width - (rect.width - 60), titleheight + 1, 10, 10), icon_tmptext);

                            GUI.color = Color.white;
                        }
                    }
                },
                onSelectCallback = (ReorderableList list) =>
                {
                    SerializedProperty sp_text = TmpTextNodes.GetArrayElementAtIndex(list.index).FindPropertyRelative("TmpText");
                    EditorGUIUtility.PingObject(sp_text.objectReferenceValue);
                },
                elementHeightCallback = index =>
                {
                    return 1.5f * LineHeight;
                }
            };
        }

        /// <summary>
        /// 获取所有图元控制器
        /// </summary>
        private void GetPrimitivesTween()
        {
            if (Targets_Selected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    SerializedObject so_ele = new SerializedObject(SelectedObjects[i]);
                    SerializedProperty sp_nodes = so_ele.FindProperty("PrimitiveControllerNodes");
                    so_ele.Update();
                    sp_nodes.ClearArray();

                    #region 获取所有 PrimitiveController
                    XHud_Module_Primitive_Controller[] cons = SelectedObjects[i].GetComponentsInChildren<XHud_Module_Primitive_Controller>();

                    #region 过滤 PrimitiveController
                    // 如果找到的 PrimitiveController 的父级是按钮、选项、滑动条、进度条、开关控件那么则要忽略
                    List<XHud_Module_Primitive_Controller> con_fillter = new List<XHud_Module_Primitive_Controller>();
                    for (int g = 0; g < cons.Length; g++)
                    {
                        XHud_Module_Button hud_Button = cons[g].GetComponentInParent<XHud_Module_Button>();
                        XHud_Module_Progress hud_Progress = cons[g].GetComponentInParent<XHud_Module_Progress>();
                        XHud_Module_Slider hud_Slider = cons[g].GetComponentInParent<XHud_Module_Slider>();
                        XHud_Module_Option hud_optselector = cons[g].GetComponentInParent<XHud_Module_Option>();
                        XHud_Module_Toggle hud_tog = cons[g].GetComponentInParent<XHud_Module_Toggle>();
                        if (hud_Button != null)
                            continue;
                        if (hud_Progress != null)
                            continue;
                        if (hud_Slider != null)
                            continue;
                        if (hud_optselector != null)
                            continue;
                        if (hud_tog != null)
                            continue;
                        con_fillter.Add(cons[g]);
                    }
                    #endregion

                    XHud_Module_Primitive_Controller[] con_confirm = con_fillter.ToArray();
                    for (int m = 0; m < con_confirm.Length; m++)
                    {
                        #region 判断是否已存在 PrimitiveController
                        bool isrepeat = false;

                        for (int s = 0; s < sp_nodes.arraySize; s++)
                        {
                            SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                            SerializedProperty sp_node_con = sp_node.FindPropertyRelative("Controller");
                            XHud_Module_Primitive_Controller sp_con = (XHud_Module_Primitive_Controller)sp_node_con.objectReferenceValue;
                            if (con_confirm[m] == sp_con)
                            {
                                isrepeat = true;
                            }
                        }

                        if (!isrepeat)
                        {
                            int index = 0;

                            if (sp_nodes.arraySize <= 0)
                                index = 0;
                            else
                                index = sp_nodes.arraySize;

                            sp_nodes.InsertArrayElementAtIndex(index);
                            SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(index);

                            #region 加入列表 PrimitiveControllerNodes 的 PrimitiveController
                            SerializedProperty sp_con = sp_node.FindPropertyRelative("Controller");
                            sp_con.objectReferenceValue = con_confirm[m];
                            sp_con.serializedObject.ApplyModifiedProperties();
                            #endregion

                            sp_node.serializedObject.ApplyModifiedProperties();
                        }
                        #endregion
                    }

                    sp_nodes.serializedObject.ApplyModifiedProperties();
                    so_ele.ApplyModifiedProperties();
                    #endregion
                }
            }
            else
            {
                SerializedProperty sp_nodes = serializedObject.FindProperty("PrimitiveControllerNodes");
                sp_nodes.ClearArray();

                #region 获取所有 PrimitiveController
                XHud_Module_Primitive_Controller[] cons = BaseScript.GetComponentsInChildren<XHud_Module_Primitive_Controller>();

                #region 过滤 PrimitiveController
                // 如果找到的 PrimitiveController 的父级是按钮、选项、滑动条、进度条、开关控件那么则要忽略
                List<XHud_Module_Primitive_Controller> cons_fillter = new List<XHud_Module_Primitive_Controller>();
                for (int i = 0; i < cons.Length; i++)
                {
                    XHud_Module_Button hud_Button = cons[i].GetComponentInParent<XHud_Module_Button>();
                    XHud_Module_Progress hud_Progress = cons[i].GetComponentInParent<XHud_Module_Progress>();
                    XHud_Module_Slider hud_Slider = cons[i].GetComponentInParent<XHud_Module_Slider>();
                    XHud_Module_Option hud_optselector = cons[i].GetComponentInParent<XHud_Module_Option>();
                    XHud_Module_Toggle hud_tog = cons[i].GetComponentInParent<XHud_Module_Toggle>();
                    if (hud_Button != null)
                        continue;
                    if (hud_Progress != null)
                        continue;
                    if (hud_Slider != null)
                        continue;
                    if (hud_optselector != null)
                        continue;
                    if (hud_tog != null)
                        continue;
                    cons_fillter.Add(cons[i]);
                }
                #endregion

                XHud_Module_Primitive_Controller[] cons_confirm = cons_fillter.ToArray();
                for (int i = 0; i < cons_confirm.Length; i++)
                {
                    #region 判断是否已存在 PrimitiveController
                    bool isrepeat = false;

                    for (int s = 0; s < sp_nodes.arraySize; s++)
                    {
                        SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                        SerializedProperty sp_node_con = sp_node.FindPropertyRelative("Controller");
                        XHud_Module_Primitive_Controller sp_con = (XHud_Module_Primitive_Controller)sp_node_con.objectReferenceValue;
                        if (cons_confirm[i] == sp_con)
                        {
                            isrepeat = true;
                        }
                    }

                    if (!isrepeat)
                    {
                        int index = 0;

                        if (sp_nodes.arraySize <= 0)
                            index = 0;
                        else
                            index = sp_nodes.arraySize;

                        sp_nodes.InsertArrayElementAtIndex(index);
                        SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(index);

                        #region 加入列表 PrimitiveControllerNodes 的 PrimitiveController
                        SerializedProperty sp_con = sp_node.FindPropertyRelative("Controller");
                        sp_con.objectReferenceValue = cons_confirm[i];
                        sp_con.serializedObject.ApplyModifiedProperties();
                        #endregion

                        sp_node.serializedObject.ApplyModifiedProperties();
                    }
                    #endregion
                }
                sp_nodes.serializedObject.ApplyModifiedProperties();
                #endregion
            }
            serializedObject.ApplyModifiedProperties();
        }
        /// <summary>
        /// 获取所有文字
        /// </summary>
        private void GetAllText()
        {
            if (Targets_Selected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    SerializedObject so_ele = new SerializedObject(SelectedObjects[i]);
                    SerializedProperty sp_nodes = so_ele.FindProperty("TextNodes");
                    so_ele.Update();

                    #region 获取所有 Text文字组件 并过滤条件
                    XHud_Module_Text[] texts = SelectedObjects[i].GetComponentsInChildren<XHud_Module_Text>();

                    List<XHud_Module_Text> texts_fillter = new List<XHud_Module_Text>();
                    for (int g = 0; g < texts.Length; g++)
                    {
                        XHud_Module_Button hud_Button = texts[g].GetComponentInParent<XHud_Module_Button>();
                        XHud_Module_Progress hud_Progress = texts[g].GetComponentInParent<XHud_Module_Progress>();
                        XHud_Module_Slider hud_Slider = texts[g].GetComponentInParent<XHud_Module_Slider>();
                        XHud_Module_Option hud_optselector = texts[g].GetComponentInParent<XHud_Module_Option>();
                        XHud_Module_Toggle hud_tog = texts[g].GetComponentInParent<XHud_Module_Toggle>();
                        if (hud_Button != null)
                            continue;
                        if (hud_Progress != null)
                            continue;
                        if (hud_Slider != null)
                            continue;
                        if (hud_optselector != null)
                            continue;
                        if (hud_tog != null)
                            continue;
                        texts_fillter.Add(texts[g]);
                    }

                    #endregion

                    XHud_Module_Text[] texts_confirm = texts_fillter.ToArray();
                    for (int m = 0; m < texts_confirm.Length; m++)
                    {
                        #region 判断是否已存在Text
                        bool isrepeat = false;

                        for (int s = 0; s < sp_nodes.arraySize; s++)
                        {
                            SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                            SerializedProperty sp_node_text = sp_node.FindPropertyRelative("Text");
                            XHud_Module_Text sp_text = (XHud_Module_Text)sp_node_text.objectReferenceValue;
                            if (texts_confirm[m] == sp_text)
                            {
                                isrepeat = true;
                            }
                        }

                        if (!isrepeat)
                        {
                            int index = 0;

                            if (sp_nodes.arraySize <= 0)
                                index = 0;
                            else
                                index = sp_nodes.arraySize;

                            sp_nodes.InsertArrayElementAtIndex(index);
                            SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(index);

                            #region 赋值 Text
                            SerializedProperty sp_Text = sp_node.FindPropertyRelative("Text");
                            sp_Text.objectReferenceValue = texts_confirm[m];
                            sp_Text.serializedObject.ApplyModifiedProperties();
                            #endregion

                            sp_node.serializedObject.ApplyModifiedProperties();
                        }
                        #endregion
                    }

                    sp_nodes.serializedObject.ApplyModifiedProperties();
                    so_ele.ApplyModifiedProperties();
                }
            }
            else
            {
                SerializedProperty sp_nodes = serializedObject.FindProperty("TextNodes");

                #region 获取所有 文字组件 并过滤条件
                XHud_Module_Text[] texts = BaseScript.GetComponentsInChildren<XHud_Module_Text>();

                List<XHud_Module_Text> texts_fillter = new List<XHud_Module_Text>();
                for (int i = 0; i < texts.Length; i++)
                {
                    XHud_Module_Button hud_Button = texts[i].GetComponentInParent<XHud_Module_Button>();
                    XHud_Module_Progress hud_Progress = texts[i].GetComponentInParent<XHud_Module_Progress>();
                    XHud_Module_Slider hud_Slider = texts[i].GetComponentInParent<XHud_Module_Slider>();
                    XHud_Module_Option hud_optselector = texts[i].GetComponentInParent<XHud_Module_Option>();
                    XHud_Module_Toggle hud_tog = texts[i].GetComponentInParent<XHud_Module_Toggle>();
                    if (hud_Button != null)
                        continue;
                    if (hud_Progress != null)
                        continue;
                    if (hud_Slider != null)
                        continue;
                    if (hud_optselector != null)
                        continue;
                    if (hud_tog != null)
                        continue;
                    texts_fillter.Add(texts[i]);
                }

                #endregion

                XHud_Module_Text[] texts_confirm = texts_fillter.ToArray();
                for (int i = 0; i < texts_confirm.Length; i++)
                {
                    #region 判断是否已存在 Text
                    bool isrepeat = false;

                    for (int s = 0; s < sp_nodes.arraySize; s++)
                    {
                        SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                        SerializedProperty sp_node_text = sp_node.FindPropertyRelative("Text");
                        XHud_Module_Text sp_text = (XHud_Module_Text)sp_node_text.objectReferenceValue;
                        if (texts_confirm[i] == sp_text)
                        {
                            isrepeat = true;
                        }
                    }

                    if (!isrepeat)
                    {
                        int index = 0;

                        if (sp_nodes.arraySize <= 0)
                            index = 0;
                        else
                            index = sp_nodes.arraySize;

                        sp_nodes.InsertArrayElementAtIndex(index);
                        SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(index);

                        #region 赋值 Text
                        SerializedProperty sp_text = sp_node.FindPropertyRelative("Text");
                        sp_text.objectReferenceValue = texts_confirm[i];
                        sp_text.serializedObject.ApplyModifiedProperties();
                        #endregion

                        sp_node.serializedObject.ApplyModifiedProperties();
                    }
                    #endregion
                }
                sp_nodes.serializedObject.ApplyModifiedProperties();
            }
            serializedObject.ApplyModifiedProperties();
        }
        /// <summary>
        /// 获取所有Tmp文字
        /// </summary>
        private void GetAllTmpText()
        {
            if (Targets_Selected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    SerializedObject so_ele = new SerializedObject(SelectedObjects[i]);
                    SerializedProperty sp_nodes = so_ele.FindProperty("TmpTextNodes");
                    so_ele.Update();

                    #region 获取所有 TmpText组件 并过滤条件
                    XHud_Module_TmpText[] texts = SelectedObjects[i].GetComponentsInChildren<XHud_Module_TmpText>();

                    List<XHud_Module_TmpText> texts_fillter = new List<XHud_Module_TmpText>();
                    for (int g = 0; g < texts.Length; g++)
                    {
                        XHud_Module_Button hud_Button = texts[g].GetComponentInParent<XHud_Module_Button>();
                        XHud_Module_Progress hud_Progress = texts[g].GetComponentInParent<XHud_Module_Progress>();
                        XHud_Module_Slider hud_Slider = texts[g].GetComponentInParent<XHud_Module_Slider>();
                        XHud_Module_Option hud_optselector = texts[g].GetComponentInParent<XHud_Module_Option>();
                        XHud_Module_Toggle hud_tog = texts[g].GetComponentInParent<XHud_Module_Toggle>();
                        if (hud_Button != null)
                            continue;
                        if (hud_Progress != null)
                            continue;
                        if (hud_Slider != null)
                            continue;
                        if (hud_optselector != null)
                            continue;
                        if (hud_tog != null)
                            continue;
                        texts_fillter.Add(texts[g]);
                    }

                    #endregion

                    XHud_Module_TmpText[] texts_confirm = texts_fillter.ToArray();
                    for (int m = 0; m < texts_confirm.Length; m++)
                    {
                        #region 判断是否已存在Text
                        bool isrepeat = false;

                        for (int s = 0; s < sp_nodes.arraySize; s++)
                        {
                            SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                            SerializedProperty sp_node_text = sp_node.FindPropertyRelative("TmpText");
                            XHud_Module_TmpText sp_text = (XHud_Module_TmpText)sp_node_text.objectReferenceValue;
                            if (texts_confirm[m] == sp_text)
                            {
                                isrepeat = true;
                            }
                        }

                        if (!isrepeat)
                        {
                            int index = 0;

                            if (sp_nodes.arraySize <= 0)
                                index = 0;
                            else
                                index = sp_nodes.arraySize;

                            sp_nodes.InsertArrayElementAtIndex(index);
                            SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(index);

                            #region 赋值 TmpText
                            SerializedProperty sp_Text = sp_node.FindPropertyRelative("TmpText");
                            sp_Text.objectReferenceValue = texts_confirm[m];
                            sp_Text.serializedObject.ApplyModifiedProperties();
                            #endregion

                            sp_node.serializedObject.ApplyModifiedProperties();
                        }
                        #endregion
                    }

                    sp_nodes.serializedObject.ApplyModifiedProperties();
                    so_ele.ApplyModifiedProperties();
                }
            }
            else
            {
                SerializedProperty sp_nodes = serializedObject.FindProperty("TmpTextNodes");

                #region 获取所有 TmpText组件 并过滤条件
                XHud_Module_TmpText[] texts = BaseScript.GetComponentsInChildren<XHud_Module_TmpText>();

                List<XHud_Module_TmpText> texts_fillter = new List<XHud_Module_TmpText>();
                for (int i = 0; i < texts.Length; i++)
                {
                    XHud_Module_Button hud_Button = texts[i].GetComponentInParent<XHud_Module_Button>();
                    XHud_Module_Progress hud_Progress = texts[i].GetComponentInParent<XHud_Module_Progress>();
                    XHud_Module_Slider hud_Slider = texts[i].GetComponentInParent<XHud_Module_Slider>();
                    XHud_Module_Option hud_optselector = texts[i].GetComponentInParent<XHud_Module_Option>();
                    XHud_Module_Toggle hud_tog = texts[i].GetComponentInParent<XHud_Module_Toggle>();
                    if (hud_Button != null)
                        continue;
                    if (hud_Progress != null)
                        continue;
                    if (hud_Slider != null)
                        continue;
                    if (hud_optselector != null)
                        continue;
                    if (hud_tog != null)
                        continue;
                    texts_fillter.Add(texts[i]);
                }

                #endregion

                XHud_Module_TmpText[] texts_confirm = texts_fillter.ToArray();
                for (int i = 0; i < texts_confirm.Length; i++)
                {
                    #region 判断是否已存在Text
                    bool isrepeat = false;

                    for (int s = 0; s < sp_nodes.arraySize; s++)
                    {
                        SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                        SerializedProperty sp_node_text = sp_node.FindPropertyRelative("TmpText");
                        XHud_Module_TmpText sp_text = (XHud_Module_TmpText)sp_node_text.objectReferenceValue;
                        if (texts_confirm[i] == sp_text)
                        {
                            isrepeat = true;
                        }
                    }

                    if (!isrepeat)
                    {
                        int index = 0;

                        if (sp_nodes.arraySize <= 0)
                            index = 0;
                        else
                            index = sp_nodes.arraySize;

                        sp_nodes.InsertArrayElementAtIndex(index);
                        SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(index);

                        #region 赋值 TmpText
                        SerializedProperty sp_text = sp_node.FindPropertyRelative("TmpText");
                        sp_text.objectReferenceValue = texts_confirm[i];
                        sp_text.serializedObject.ApplyModifiedProperties();
                        #endregion

                        sp_node.serializedObject.ApplyModifiedProperties();
                    }
                    #endregion
                }
                sp_nodes.serializedObject.ApplyModifiedProperties();
            }
            serializedObject.ApplyModifiedProperties();
        }
        /// <summary>
        /// 获取所有选项
        /// </summary>
        private void GetAllOptions()
        {
            if (Targets_Selected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    SerializedObject so_ele = new SerializedObject(SelectedObjects[i]);
                    SerializedProperty sp_nodes = so_ele.FindProperty("OptionNodes");
                    so_ele.Update();

                    XHud_Module_Option[] allopts = SelectedObjects[i].GetComponentsInChildren<XHud_Module_Option>();

                    List<XHud_Module_Option> Filter = new List<XHud_Module_Option>();
                    for (int s = 0; s < allopts.Length; s++)
                    {
                        Filter.Add(allopts[s]);
                    }

                    XHud_Module_Option[] gettedOpts = Filter.ToArray();

                    for (int x = 0; x < gettedOpts.Length; x++)
                    {
                        bool repeat = false;

                        if (sp_nodes.arraySize > 0)
                        {
                            for (int s = 0; s < sp_nodes.arraySize; s++)
                            {
                                SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                                SerializedProperty sp_node_opt = sp_node.FindPropertyRelative("Option");
                                XHud_Module_Option sp_opt = (XHud_Module_Option)sp_node_opt.objectReferenceValue;
                                if (sp_opt == gettedOpts[x])
                                    repeat = true;
                            }
                        }

                        if (!repeat)
                        {
                            int index = 0;

                            if (sp_nodes.arraySize <= 0)
                                index = 0;
                            else
                                index = sp_nodes.arraySize - 1;

                            sp_nodes.InsertArrayElementAtIndex(index);

                            SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(index);
                            SerializedProperty sp_opt = sp_node.FindPropertyRelative("Option");
                            sp_opt.objectReferenceValue = gettedOpts[x];

                            sp_opt.serializedObject.ApplyModifiedProperties();
                            sp_node.serializedObject.ApplyModifiedProperties();
                        }
                    }
                    sp_nodes.serializedObject.ApplyModifiedProperties();
                    so_ele.ApplyModifiedProperties();
                }
            }
            else
            {
                SerializedProperty sp_nodes = serializedObject.FindProperty("OptionNodes");

                XHud_Module_Option[] options = BaseScript.GetComponentsInChildren<XHud_Module_Option>();

                List<XHud_Module_Option> Filter = new List<XHud_Module_Option>();
                for (int i = 0; i < options.Length; i++)
                {
                    Filter.Add(options[i]);
                }

                XHud_Module_Option[] gettedOpts = Filter.ToArray();

                for (int i = 0; i < gettedOpts.Length; i++)
                {
                    bool repeat = false;

                    if (sp_nodes.arraySize > 0)
                    {
                        for (int s = 0; s < sp_nodes.arraySize; s++)
                        {
                            SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                            SerializedProperty sp_node_opt = sp_node.FindPropertyRelative("Option");
                            XHud_Module_Option sp_opt = (XHud_Module_Option)sp_node_opt.objectReferenceValue;
                            if (sp_opt == gettedOpts[i])
                                repeat = true;
                        }
                    }

                    if (!repeat)
                    {
                        int index = 0;

                        if (sp_nodes.arraySize <= 0)
                            index = 0;
                        else
                            index = sp_nodes.arraySize - 1;

                        sp_nodes.InsertArrayElementAtIndex(index);

                        SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(index);
                        SerializedProperty sp_Option = sp_node.FindPropertyRelative("Option");
                        sp_Option.objectReferenceValue = gettedOpts[i];

                        sp_Option.serializedObject.ApplyModifiedProperties();
                        sp_node.serializedObject.ApplyModifiedProperties();
                    }
                }
                sp_nodes.serializedObject.ApplyModifiedProperties();
            }
        }
        /// <summary>
        /// 获取所有滑动条
        /// </summary>
        private void GetAllSliders()
        {
            if (Targets_Selected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    SerializedObject so_ele = new SerializedObject(SelectedObjects[i]);
                    SerializedProperty sp_nodes = so_ele.FindProperty("SliderNodes");
                    so_ele.Update();

                    XHud_Module_Slider[] allSliders = SelectedObjects[i].GetComponentsInChildren<XHud_Module_Slider>();

                    List<XHud_Module_Slider> Filter = new List<XHud_Module_Slider>();
                    for (int s = 0; s < allSliders.Length; s++)
                    {
                        Filter.Add(allSliders[s]);
                    }

                    XHud_Module_Slider[] gettedSliders = Filter.ToArray();

                    for (int x = 0; x < gettedSliders.Length; x++)
                    {
                        bool repeat = false;

                        if (sp_nodes.arraySize > 0)
                        {
                            for (int s = 0; s < sp_nodes.arraySize; s++)
                            {
                                SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                                SerializedProperty sp_node_Slider = sp_node.FindPropertyRelative("Slider");
                                XHud_Module_Slider sp_opt = (XHud_Module_Slider)sp_node_Slider.objectReferenceValue;
                                if (sp_opt == gettedSliders[x])
                                    repeat = true;
                            }
                        }

                        if (!repeat)
                        {
                            int index = 0;

                            if (sp_nodes.arraySize <= 0)
                                index = 0;
                            else
                                index = sp_nodes.arraySize - 1;

                            sp_nodes.InsertArrayElementAtIndex(index);

                            SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(index);
                            SerializedProperty sp_Slider = sp_node.FindPropertyRelative("Slider");
                            sp_Slider.objectReferenceValue = gettedSliders[x];

                            sp_Slider.serializedObject.ApplyModifiedProperties();
                            sp_node.serializedObject.ApplyModifiedProperties();
                        }
                    }
                    sp_nodes.serializedObject.ApplyModifiedProperties();
                    so_ele.ApplyModifiedProperties();
                }
            }
            else
            {
                SerializedProperty sp_nodes = serializedObject.FindProperty("SliderNodes");

                XHud_Module_Slider[] allslider = BaseScript.GetComponentsInChildren<XHud_Module_Slider>();

                List<XHud_Module_Slider> Filter = new List<XHud_Module_Slider>();
                for (int i = 0; i < allslider.Length; i++)
                {
                    Filter.Add(allslider[i]);
                }

                XHud_Module_Slider[] gettedSliders = Filter.ToArray();

                for (int i = 0; i < gettedSliders.Length; i++)
                {
                    bool repeat = false;

                    if (sp_nodes.arraySize > 0)
                    {
                        for (int s = 0; s < sp_nodes.arraySize; s++)
                        {
                            SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                            SerializedProperty sp_node_sli = sp_node.FindPropertyRelative("Slider");
                            XHud_Module_Slider sp_opt = (XHud_Module_Slider)sp_node_sli.objectReferenceValue;
                            if (sp_opt == gettedSliders[i])
                                repeat = true;
                        }
                    }

                    if (!repeat)
                    {
                        int index = 0;

                        if (sp_nodes.arraySize <= 0)
                            index = 0;
                        else
                            index = sp_nodes.arraySize - 1;

                        sp_nodes.InsertArrayElementAtIndex(index);

                        SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(index);
                        SerializedProperty sp_Slider = sp_node.FindPropertyRelative("Slider");
                        sp_Slider.objectReferenceValue = gettedSliders[i];

                        sp_Slider.serializedObject.ApplyModifiedProperties();
                        sp_node.serializedObject.ApplyModifiedProperties();
                    }
                }
                sp_nodes.serializedObject.ApplyModifiedProperties();
            }
        }
        /// <summary>
        /// 获取所有按钮
        /// </summary>
        private void GetAllButtons()
        {
            if (Targets_Selected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    SerializedObject so_ele = new SerializedObject(SelectedObjects[i]);
                    SerializedProperty sp_nodes = so_ele.FindProperty("ButtonNodes");
                    so_ele.Update();

                    XHud_Module_Button[] allbtns = SelectedObjects[i].GetComponentsInChildren<XHud_Module_Button>();

                    List<XHud_Module_Button> Filter = new List<XHud_Module_Button>();
                    for (int s = 0; s < allbtns.Length; s++)
                    {
                        XHud_Module_Option hud_optselector = allbtns[s].GetComponentInParent<XHud_Module_Option>();
                        if (hud_optselector != null)
                            continue;
                        Filter.Add(allbtns[s]);
                    }

                    XHud_Module_Button[] gettedBtns = Filter.ToArray();

                    for (int x = 0; x < gettedBtns.Length; x++)
                    {
                        bool repeat = false;

                        if (sp_nodes.arraySize > 0)
                        {
                            for (int s = 0; s < sp_nodes.arraySize; s++)
                            {
                                SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                                SerializedProperty sp_node_btn = sp_node.FindPropertyRelative("Button");
                                XHud_Module_Button sp_btn = (XHud_Module_Button)sp_node_btn.objectReferenceValue;
                                if (sp_btn == gettedBtns[x])
                                    repeat = true;
                            }
                        }

                        if (!repeat)
                        {
                            int index = 0;

                            if (sp_nodes.arraySize <= 0)
                                index = 0;
                            else
                                index = sp_nodes.arraySize - 1;

                            sp_nodes.InsertArrayElementAtIndex(index);

                            SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(index);
                            SerializedProperty sp_btn = sp_node.FindPropertyRelative("Button");
                            sp_btn.objectReferenceValue = gettedBtns[x];

                            sp_btn.serializedObject.ApplyModifiedProperties();
                            sp_node.serializedObject.ApplyModifiedProperties();
                        }
                    }
                    sp_nodes.serializedObject.ApplyModifiedProperties();
                    so_ele.ApplyModifiedProperties();
                }
            }
            else
            {
                SerializedProperty sp_nodes = serializedObject.FindProperty("ButtonNodes");

                XHud_Module_Button[] buttons = BaseScript.GetComponentsInChildren<XHud_Module_Button>();

                List<XHud_Module_Button> Filter = new List<XHud_Module_Button>();
                for (int i = 0; i < buttons.Length; i++)
                {
                    XHud_Module_Option hud_optselector = buttons[i].GetComponentInParent<XHud_Module_Option>();
                    if (hud_optselector != null)
                        continue;
                    Filter.Add(buttons[i]);
                }

                XHud_Module_Button[] gettedBtns = Filter.ToArray();

                for (int i = 0; i < gettedBtns.Length; i++)
                {
                    bool repeat = false;

                    if (sp_nodes.arraySize > 0)
                    {
                        for (int s = 0; s < sp_nodes.arraySize; s++)
                        {
                            SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                            SerializedProperty sp_node_btn = sp_node.FindPropertyRelative("Button");
                            XHud_Module_Button sp_btn = (XHud_Module_Button)sp_node_btn.objectReferenceValue;
                            if (sp_btn == gettedBtns[i])
                                repeat = true;
                        }
                    }

                    if (!repeat)
                    {
                        int index = 0;

                        if (sp_nodes.arraySize <= 0)
                            index = 0;
                        else
                            index = sp_nodes.arraySize - 1;

                        sp_nodes.InsertArrayElementAtIndex(index);

                        SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(index);
                        SerializedProperty sp_button = sp_node.FindPropertyRelative("Button");
                        sp_button.objectReferenceValue = gettedBtns[i];

                        sp_button.serializedObject.ApplyModifiedProperties();
                        sp_node.serializedObject.ApplyModifiedProperties();
                    }
                }
                sp_nodes.serializedObject.ApplyModifiedProperties();
            }
        }
        /// <summary>
        /// 获取所有进度条
        /// </summary>
        private void GetAllProgress()
        {
            if (Targets_Selected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    SerializedObject so_ele = new SerializedObject(SelectedObjects[i]);
                    SerializedProperty sp_nodes = so_ele.FindProperty("ProgressNodes");
                    so_ele.Update();

                    XHud_Module_Progress[] allpros = SelectedObjects[i].GetComponentsInChildren<XHud_Module_Progress>();

                    List<XHud_Module_Progress> Filter = new List<XHud_Module_Progress>();
                    for (int s = 0; s < allpros.Length; s++)
                    {
                        Filter.Add(allpros[s]);
                    }

                    XHud_Module_Progress[] gettedPros = Filter.ToArray();

                    for (int x = 0; x < gettedPros.Length; x++)
                    {
                        bool repeat = false;

                        if (sp_nodes.arraySize > 0)
                        {
                            for (int s = 0; s < sp_nodes.arraySize; s++)
                            {
                                SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                                SerializedProperty sp_node_pro = sp_node.FindPropertyRelative("Progress");
                                XHud_Module_Progress sp_pro = (XHud_Module_Progress)sp_node_pro.objectReferenceValue;
                                if (sp_pro == gettedPros[x])
                                    repeat = true;
                            }
                        }

                        if (!repeat)
                        {
                            int index = 0;

                            if (sp_nodes.arraySize <= 0)
                                index = 0;
                            else
                                index = sp_nodes.arraySize - 1;

                            sp_nodes.InsertArrayElementAtIndex(index);

                            SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(index);
                            SerializedProperty sp_pro = sp_node.FindPropertyRelative("Progress");
                            sp_pro.objectReferenceValue = gettedPros[x];

                            sp_pro.serializedObject.ApplyModifiedProperties();
                            sp_node.serializedObject.ApplyModifiedProperties();
                        }
                    }
                    sp_nodes.serializedObject.ApplyModifiedProperties();
                    so_ele.ApplyModifiedProperties();
                }
            }
            else
            {
                SerializedProperty sp_nodes = serializedObject.FindProperty("ProgressNodes");

                XHud_Module_Progress[] allpros = BaseScript.GetComponentsInChildren<XHud_Module_Progress>();

                List<XHud_Module_Progress> Filter = new List<XHud_Module_Progress>();
                for (int i = 0; i < allpros.Length; i++)
                {
                    Filter.Add(allpros[i]);
                }

                XHud_Module_Progress[] gettedPros = Filter.ToArray();

                for (int i = 0; i < gettedPros.Length; i++)
                {
                    bool repeat = false;

                    if (sp_nodes.arraySize > 0)
                    {
                        for (int s = 0; s < sp_nodes.arraySize; s++)
                        {
                            SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                            SerializedProperty sp_node_pro = sp_node.FindPropertyRelative("Progress");
                            XHud_Module_Progress sp_pro = (XHud_Module_Progress)sp_node_pro.objectReferenceValue;
                            if (sp_pro == gettedPros[i])
                                repeat = true;
                        }
                    }

                    if (!repeat)
                    {
                        int index = 0;

                        if (sp_nodes.arraySize <= 0)
                            index = 0;
                        else
                            index = sp_nodes.arraySize - 1;

                        sp_nodes.InsertArrayElementAtIndex(index);

                        SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(index);
                        SerializedProperty sp_pro = sp_node.FindPropertyRelative("Progress");
                        sp_pro.objectReferenceValue = gettedPros[i];

                        sp_pro.serializedObject.ApplyModifiedProperties();
                        sp_node.serializedObject.ApplyModifiedProperties();
                    }
                }
                sp_nodes.serializedObject.ApplyModifiedProperties();
            }
        }
        /// <summary>
        /// 获取所有开关
        /// </summary>
        private void GetAllToggle()
        {
            if (Targets_Selected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    SerializedObject so_ele = new SerializedObject(SelectedObjects[i]);
                    SerializedProperty sp_nodes = so_ele.FindProperty("ToggleNodes");
                    so_ele.Update();

                    XHud_Module_Toggle[] alltog = SelectedObjects[i].GetComponentsInChildren<XHud_Module_Toggle>();

                    List<XHud_Module_Toggle> Filter = new List<XHud_Module_Toggle>();
                    for (int s = 0; s < alltog.Length; s++)
                    {
                        Filter.Add(alltog[s]);
                    }

                    XHud_Module_Toggle[] gettedtogs = Filter.ToArray();

                    for (int x = 0; x < gettedtogs.Length; x++)
                    {
                        bool repeat = false;

                        if (sp_nodes.arraySize > 0)
                        {
                            for (int s = 0; s < sp_nodes.arraySize; s++)
                            {
                                SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                                SerializedProperty sp_node_tog = sp_node.FindPropertyRelative("Toggle");
                                XHud_Module_Toggle sp_tog = (XHud_Module_Toggle)sp_node_tog.objectReferenceValue;
                                if (sp_tog == gettedtogs[x])
                                    repeat = true;
                            }
                        }

                        if (!repeat)
                        {
                            int index = 0;

                            if (sp_nodes.arraySize <= 0)
                                index = 0;
                            else
                                index = sp_nodes.arraySize - 1;

                            sp_nodes.InsertArrayElementAtIndex(index);

                            SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(index);
                            SerializedProperty sp_tog = sp_node.FindPropertyRelative("Toggle");
                            sp_tog.objectReferenceValue = gettedtogs[x];

                            sp_tog.serializedObject.ApplyModifiedProperties();
                            sp_node.serializedObject.ApplyModifiedProperties();
                        }
                    }
                    sp_nodes.serializedObject.ApplyModifiedProperties();
                    so_ele.ApplyModifiedProperties();
                }
            }
            else
            {
                SerializedProperty sp_nodes = serializedObject.FindProperty("ToggleNodes");

                XHud_Module_Toggle[] alltog = BaseScript.GetComponentsInChildren<XHud_Module_Toggle>();

                List<XHud_Module_Toggle> Filter = new List<XHud_Module_Toggle>();
                for (int i = 0; i < alltog.Length; i++)
                {
                    Filter.Add(alltog[i]);
                }

                XHud_Module_Toggle[] gettedtogs = Filter.ToArray();

                for (int i = 0; i < gettedtogs.Length; i++)
                {
                    bool repeat = false;

                    if (sp_nodes.arraySize > 0)
                    {
                        for (int s = 0; s < sp_nodes.arraySize; s++)
                        {
                            SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                            SerializedProperty sp_node_tog = sp_node.FindPropertyRelative("Toggle");
                            XHud_Module_Toggle sp_tog = (XHud_Module_Toggle)sp_node_tog.objectReferenceValue;
                            if (sp_tog == gettedtogs[i])
                                repeat = true;
                        }
                    }

                    if (!repeat)
                    {
                        int index = 0;

                        if (sp_nodes.arraySize <= 0)
                            index = 0;
                        else
                            index = sp_nodes.arraySize - 1;

                        sp_nodes.InsertArrayElementAtIndex(index);

                        SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(index);
                        SerializedProperty sp_tog = sp_node.FindPropertyRelative("Toggle");
                        sp_tog.objectReferenceValue = gettedtogs[i];

                        sp_tog.serializedObject.ApplyModifiedProperties();
                        sp_node.serializedObject.ApplyModifiedProperties();
                    }
                }
                sp_nodes.serializedObject.ApplyModifiedProperties();
            }
        }
        /// <summary>
        /// 获取所有音效器
        /// </summary>
        private void GetAllSounder()
        {
            if (Targets_Selected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    SerializedObject so_ele = new SerializedObject(SelectedObjects[i]);
                    SerializedProperty sp_nodes = so_ele.FindProperty("SounderNodes");
                    so_ele.Update();

                    XHud_Element_Sounder[] allsod = SelectedObjects[i].GetComponentsInChildren<XHud_Element_Sounder>();

                    List<XHud_Element_Sounder> Filter = new List<XHud_Element_Sounder>();
                    for (int s = 0; s < allsod.Length; s++)
                    {
                        Filter.Add(allsod[s]);
                    }

                    XHud_Element_Sounder[] gettedsods = Filter.ToArray();

                    for (int x = 0; x < gettedsods.Length; x++)
                    {
                        bool repeat = false;

                        if (sp_nodes.arraySize > 0)
                        {
                            for (int s = 0; s < sp_nodes.arraySize; s++)
                            {
                                SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                                SerializedProperty sp_node_sod = sp_node.FindPropertyRelative("Sounder");
                                XHud_Element_Sounder sp_sod = (XHud_Element_Sounder)sp_node_sod.objectReferenceValue;
                                if (sp_sod == gettedsods[x])
                                    repeat = true;
                            }
                        }

                        if (!repeat)
                        {
                            int index = 0;

                            if (sp_nodes.arraySize <= 0)
                                index = 0;
                            else
                                index = sp_nodes.arraySize;

                            sp_nodes.InsertArrayElementAtIndex(index);

                            SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(index);
                            SerializedProperty sp_tog = sp_node.FindPropertyRelative("Sounder");
                            sp_tog.objectReferenceValue = gettedsods[x];

                            sp_tog.serializedObject.ApplyModifiedProperties();
                            sp_node.serializedObject.ApplyModifiedProperties();
                        }
                    }
                    sp_nodes.serializedObject.ApplyModifiedProperties();
                    so_ele.ApplyModifiedProperties();
                }
            }
            else
            {
                SerializedProperty sp_nodes = serializedObject.FindProperty("SounderNodes");

                XHud_Element_Sounder[] allsod = BaseScript.GetComponentsInChildren<XHud_Element_Sounder>();

                List<XHud_Element_Sounder> Filter = new List<XHud_Element_Sounder>();
                for (int i = 0; i < allsod.Length; i++)
                {
                    XHud_Module_Button hud_Button = allsod[i].GetComponentInParent<XHud_Module_Button>();
                    XHud_Module_Progress hud_Progress = allsod[i].GetComponentInParent<XHud_Module_Progress>();
                    XHud_Module_Slider hud_Slider = allsod[i].GetComponentInParent<XHud_Module_Slider>();
                    XHud_Module_Option hud_optselector = allsod[i].GetComponentInParent<XHud_Module_Option>();
                    XHud_Module_Toggle hud_tog = allsod[i].GetComponentInParent<XHud_Module_Toggle>();
                    if (hud_Button != null)
                        continue;
                    if (hud_Progress != null)
                        continue;
                    if (hud_Slider != null)
                        continue;
                    if (hud_optselector != null)
                        continue;
                    if (hud_tog != null)
                        continue;

                    Filter.Add(allsod[i]);
                }

                XHud_Element_Sounder[] gettedsods = Filter.ToArray();

                for (int i = 0; i < gettedsods.Length; i++)
                {
                    bool repeat = false;

                    if (sp_nodes.arraySize > 0)
                    {
                        for (int s = 0; s < sp_nodes.arraySize; s++)
                        {
                            SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(s);
                            SerializedProperty sp_node_sod = sp_node.FindPropertyRelative("Sounder");
                            XHud_Element_Sounder sp_tog = (XHud_Element_Sounder)sp_node_sod.objectReferenceValue;
                            if (sp_tog == gettedsods[i])
                                repeat = true;
                        }
                    }

                    if (!repeat)
                    {
                        int index = 0;

                        if (sp_nodes.arraySize <= 0)
                            index = 0;
                        else
                            index = sp_nodes.arraySize;

                        sp_nodes.InsertArrayElementAtIndex(index);

                        SerializedProperty sp_node = sp_nodes.GetArrayElementAtIndex(index);
                        SerializedProperty sp_sod = sp_node.FindPropertyRelative("Sounder");
                        sp_sod.objectReferenceValue = gettedsods[i];

                        sp_sod.serializedObject.ApplyModifiedProperties();
                        sp_node.serializedObject.ApplyModifiedProperties();
                    }
                }
                sp_nodes.serializedObject.ApplyModifiedProperties();
            }
        }
        /// <summary>
        /// 清理所有无效控件节点
        /// </summary>
        private void ClearEmptyNodes()
        {
            for (int i = 0; i < BaseScript.TmpTextNodes.Count; i++)
            {
                // 检查是否为 null 或类型不匹配
                if (BaseScript.TmpTextNodes[i].TmpText == null)
                {
                    TmpTextNodes.DeleteArrayElementAtIndex(i);
                }
            }
            TmpTextNodes.serializedObject.ApplyModifiedProperties();

            for (int i = 0; i < BaseScript.TextNodes.Count; i++)
            {
                // 检查是否为 null 或类型不匹配
                if (BaseScript.TextNodes[i].Text == null)
                {
                    TextNodes.DeleteArrayElementAtIndex(i);
                }
            }
            TextNodes.serializedObject.ApplyModifiedProperties();

            for (int i = 0; i < BaseScript.ButtonNodes.Count; i++)
            {
                // 检查是否为 null 或类型不匹配
                if (BaseScript.ButtonNodes[i].Button == null)
                {
                    TextNodes.DeleteArrayElementAtIndex(i);
                }
            }
            ButtonNodes.serializedObject.ApplyModifiedProperties();

            for (int i = 0; i < BaseScript.SliderNodes.Count; i++)
            {
                // 检查是否为 null 或类型不匹配
                if (BaseScript.SliderNodes[i].Slider == null)
                {
                    SliderNodes.DeleteArrayElementAtIndex(i);
                }
            }
            SliderNodes.serializedObject.ApplyModifiedProperties();

            for (int i = 0; i < BaseScript.OptionNodes.Count; i++)
            {
                // 检查是否为 null 或类型不匹配
                if (BaseScript.OptionNodes[i].Option == null)
                {
                    OptionNodes.DeleteArrayElementAtIndex(i);
                }
            }
            OptionNodes.serializedObject.ApplyModifiedProperties();

            for (int i = 0; i < BaseScript.ProgressNodes.Count; i++)
            {
                // 检查是否为 null 或类型不匹配
                if (BaseScript.ProgressNodes[i].Progress == null)
                {
                    ProgressNodes.DeleteArrayElementAtIndex(i);
                }
            }
            ProgressNodes.serializedObject.ApplyModifiedProperties();

            for (int i = 0; i < BaseScript.ToggleNodes.Count; i++)
            {
                // 检查是否为 null 或类型不匹配
                if (BaseScript.ToggleNodes[i].Toggle == null)
                {
                    ToggleNodes.DeleteArrayElementAtIndex(i);
                }
            }
            ToggleNodes.serializedObject.ApplyModifiedProperties();
        }
    }
}