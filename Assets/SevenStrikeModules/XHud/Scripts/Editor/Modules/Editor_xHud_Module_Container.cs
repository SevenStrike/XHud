namespace SevenStrikeModules.XHud.Hud
{
    using DG.DOTweenEditor;
    using DG.Tweening;
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.GuiLib;
    using SevenStrikeModules.XHud.Utilitys;
    using SoftMasking;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Unity.EditorCoroutines.Editor;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.UI;
    using Object = UnityEngine.Object;
    using Random = UnityEngine.Random;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(xHud_Module_Container), true)]
    public class Editor_xHud_Module_Container : Editor
    {
        #region 组件 / 列表
        private xHud_Module_Container BaseScript;
        private ReorderableList list_Container;
        #endregion

        private float LineHeight;
        public bool BasicVars;
        private float labelwidth;

        #region 序列化属性
        SerializedProperty UseDebug, AutoStopPreview, AnimatorPlayTiming, ContainerItems, AnimateState, Animators_GlobalDuration, Animators_MaxDuration, EventIsFold, eve_on_itemcount_get, eve_on_item_get, eve_on_items_get, eve_on_item_add, eve_on_item_remove, eve_on_items_remove, eve_on_animate_play_all, eve_on_animate_play_by_item, eve_on_animate_rewind_all, eve_on_animate_rewind_by_item, eve_on_animate_ready_all, eve_on_animate_ready_by_item, eve_on_item_changevalue, Indicator;
        #endregion

        #region Preview - Animator
        private List<Tweener> Preivew_Animator_TweenList = new List<Tweener>();
        private List<EditorCoroutine> Preivew_Animator_CoroutineList_Play = new List<EditorCoroutine>();
        private EditorCoroutine Preivew_Animator_Coroutine_Stop;
        #endregion

        #region Preview - AnimatorSound
        public List<AudioSource> Preivew_AnimatorSound_SoundList = new List<AudioSource>();
        private EditorCoroutine Preivew_AnimatorSound_Coroutine_Play;
        private List<EditorCoroutine> Preivew_AnimatorSound_CoroutineList_Stop = new List<EditorCoroutine>();
        #endregion

        #region 图标
        private Texture2D scan_r, scan_p, reid_r, reid_p, clear_r, clear_p, prw_play_r, prw_play_p, prw_stop_r, prw_stop_p, animator, timer_max, icon_main, animstate, left_arrow_r, left_arrow_p, right_arrow_r, right_arrow_p, icon_anim, icon_text, icon_tmptext, icon_image, icon_rawimage, icon_rect, conitem;
        #endregion

        #region 容器项类型数量统计
        public int ImageCount;
        public int RawImageCount;
        public int TextCount;
        public int TmpTextCount;
        public int AnimatorCount;
        #endregion

        #region 批量模式查看索引
        private int Containner_Status_Index;
        private int Containner_Statistic_Index;
        #endregion

        #region 选项文字
        string[] stroptions_debug = new string[2] { "关闭", "调试" }, stroptions_enabled = new string[2] { "关闭", "开启" }, stroptions_clickthreadhold = new string[4] { "无", "元素进入时", "元素进入后", "元素退出时" };
        #endregion

        #region 批量化操作
        private xHud_Module_Container[] SelectedObjects;

        private void GetAllTargets()
        {
            if (targets.Length > 1)
            {
                SelectedObjects = new xHud_Module_Container[targets.Length];
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    var t = targets[i];
                    SelectedObjects[i] = (xHud_Module_Container)t;
                }
            }
            else
            {
                SelectedObjects = new xHud_Module_Container[targets.Length];
                SelectedObjects[0] = (xHud_Module_Container)target;
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
            BaseScript = (xHud_Module_Container)target;

            #region 属性获取
            ContainerItems = serializedObject.FindProperty("ContainerItems");
            Indicator = serializedObject.FindProperty("Indicator");
            UseDebug = serializedObject.FindProperty("UseDebug");
            Animators_GlobalDuration = serializedObject.FindProperty("Animators_GlobalDuration");
            AnimateState = serializedObject.FindProperty("AnimateState");
            Animators_MaxDuration = serializedObject.FindProperty("Animators_MaxDuration");
            EventIsFold = serializedObject.FindProperty("EventIsFold");
            eve_on_itemcount_get = serializedObject.FindProperty("eve_on_itemcount_get");
            eve_on_item_get = serializedObject.FindProperty("eve_on_item_get");
            eve_on_items_get = serializedObject.FindProperty("eve_on_items_get");
            eve_on_item_add = serializedObject.FindProperty("eve_on_item_add");
            eve_on_item_remove = serializedObject.FindProperty("eve_on_item_remove");
            eve_on_items_remove = serializedObject.FindProperty("eve_on_items_remove");
            eve_on_animate_play_all = serializedObject.FindProperty("eve_on_animate_play_all");
            eve_on_animate_play_by_item = serializedObject.FindProperty("eve_on_animate_play_by_item");
            eve_on_animate_rewind_all = serializedObject.FindProperty("eve_on_animate_rewind_all");
            eve_on_animate_rewind_by_item = serializedObject.FindProperty("eve_on_animate_rewind_by_item");
            eve_on_animate_ready_all = serializedObject.FindProperty("eve_on_animate_ready_all");
            eve_on_animate_ready_by_item = serializedObject.FindProperty("eve_on_animate_ready_by_item");
            eve_on_item_changevalue = serializedObject.FindProperty("eve_on_item_changevalue");
            AnimatorPlayTiming = serializedObject.FindProperty("AnimatorPlayTiming");
            AutoStopPreview = serializedObject.FindProperty("AutoStopPreview");
            #endregion

            LineHeight = EditorGUIUtility.singleLineHeight;

            #region 图标获取
            icon_main = Editor_xHudGUI.GetIcon("Icons_Hud_Container/icon_main");
            scan_r = Editor_xHudGUI.GetIcon("Icons_Hud_Container/scan_r");
            scan_p = Editor_xHudGUI.GetIcon("Icons_Hud_Container/scan_p");
            reid_r = Editor_xHudGUI.GetIcon("Icons_Hud_Container/reid_r");
            reid_p = Editor_xHudGUI.GetIcon("Icons_Hud_Container/reid_p");
            clear_r = Editor_xHudGUI.GetIcon("Icons_Hud_Container/clear_r");
            clear_p = Editor_xHudGUI.GetIcon("Icons_Hud_Container/clear_p");
            prw_play_r = Editor_xHudGUI.GetIcon("Icons_Hud_Container/prw_play_r");
            prw_play_p = Editor_xHudGUI.GetIcon("Icons_Hud_Container/prw_play_p");
            prw_stop_r = Editor_xHudGUI.GetIcon("Icons_Hud_Container/prw_stop_r");
            prw_stop_p = Editor_xHudGUI.GetIcon("Icons_Hud_Container/prw_stop_p");
            animator = Editor_xHudGUI.GetIcon("Icons_Hud_Container/animator");
            timer_max = Editor_xHudGUI.GetIcon("Icons_Hud_Container/timer_max");
            animstate = Editor_xHudGUI.GetIcon("Icons_Hud_Container/animstate");
            left_arrow_r = Editor_xHudGUI.GetIcon("Icons_Hud_Container/left_arrow_r");
            left_arrow_p = Editor_xHudGUI.GetIcon("Icons_Hud_Container/left_arrow_p");
            right_arrow_r = Editor_xHudGUI.GetIcon("Icons_Hud_Container/right_arrow_r");
            right_arrow_p = Editor_xHudGUI.GetIcon("Icons_Hud_Container/right_arrow_p");
            conitem = Editor_xHudGUI.GetIcon("Icons_Hud_Container/conitem");
            icon_anim = Editor_xHudGUI.GetIcon("Icons_Hud_Container/icon_anim");
            icon_text = Editor_xHudGUI.GetIcon("Icons_Hud_Container/icon_text");
            icon_tmptext = Editor_xHudGUI.GetIcon("Icons_Hud_Container/icon_tmptext");
            icon_image = Editor_xHudGUI.GetIcon("Icons_Hud_Container/icon_image");
            icon_rawimage = Editor_xHudGUI.GetIcon("Icons_Hud_Container/icon_rawimage");
            icon_rect = Editor_xHudGUI.GetIcon("Icons_Hud_Container/icon_rect");
            #endregion

            #region 容器项列表
            list_Container = new ReorderableList(serializedObject, ContainerItems)
            {
                displayAdd = true,
                displayRemove = true,
                draggable = true,

                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "容器项");
                },
                drawElementBackgroundCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    if (index >= 0)
                    {
                        if (isFocused)
                            EditorGUI.DrawRect(new Rect(rect.x + 10, rect.y + 20, 5, 5), Editor_xHudGUI.GetColor(HudColor.灰绿));
                        else
                            EditorGUI.DrawRect(new Rect(rect.x + 10, rect.y + 20, 5, 5), Editor_xHudGUI.GetColor(HudColor.深空灰));
                    }
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    SerializedProperty sp_Root = ContainerItems.GetArrayElementAtIndex(index);

                    SerializedProperty sp_id = sp_Root.FindPropertyRelative("ID");
                    SerializedProperty sp_Type = sp_Root.FindPropertyRelative("Type");
                    SerializedProperty sp_Indicator = sp_Root.FindPropertyRelative("Indicator");
                    SerializedProperty sp_Delay = sp_Root.FindPropertyRelative("DelayTime");
                    SerializedProperty sp_animator = sp_Root.FindPropertyRelative("Animator");

                    SerializedProperty sp_Text = sp_Root.FindPropertyRelative("Text");
                    SerializedProperty sp_Text_text = sp_Text.FindPropertyRelative("Base");

                    SerializedProperty sp_TmpText = sp_Root.FindPropertyRelative("TmpText");
                    SerializedProperty sp_TmpText_text = sp_TmpText.FindPropertyRelative("Base");

                    SerializedProperty sp_Image = sp_Root.FindPropertyRelative("Image");
                    SerializedProperty sp_Image_image = sp_Image.FindPropertyRelative("Base");

                    SerializedProperty sp_RawImage = sp_Root.FindPropertyRelative("RawImage");
                    SerializedProperty sp_RawImage_img = sp_RawImage.FindPropertyRelative("Base");

                    #region 标题文字
                    string title = "";
                    string indicator = sp_Indicator.stringValue;
                    if (!string.IsNullOrEmpty(indicator))
                        title += indicator;
                    else
                        title += BaseScript.ContainerItems[index].Animator.name;
                    Editor_xHudGUI.Gui_Labelfield(new Rect(rect.x + 7, rect.y, (rect.width * 0.45f) - 20, LineHeight), title, HudFilled.无, HudColor.亮白, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 12, TextClipping.Clip);
                    #endregion

                    #region ID                        
                    GUI.color = Editor_xHudGUI.GetColor(HudColor.阴影灰);
                    Editor_xHudGUI.Gui_Labelfield(new Rect(rect.x + 7, rect.y + 29, 100, 15), "ID： " + sp_id.intValue, HudFilled.无, HudColor.无, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 11);
                    GUI.color = Color.white;
                    #endregion

                    #region 类型图标
                    Texture2D typeicon = null;
                    ContainerType type = (ContainerType)sp_Type.enumValueIndex;
                    switch (type)
                    {
                        case ContainerType.None:
                            typeicon = icon_rect;
                            break;
                        case ContainerType.Text:
                            typeicon = icon_text;
                            break;
                        case ContainerType.TmpText:
                            typeicon = icon_tmptext;
                            break;
                        case ContainerType.Image:
                            typeicon = icon_image;
                            break;
                        case ContainerType.RawImage:
                            typeicon = icon_rawimage;
                            break;
                    }
                    GUI.color = xHud_Dashboard.Theme_Primary;
                    Editor_xHudGUI.Gui_Icon(new Rect(rect.width + 30, rect.y + 5, 12, 12), typeicon);
                    GUI.color = Color.white;
                    #endregion

                    float baseheight = rect.y + 30;

                    #region 延迟时间
                    if (BaseScript.ContainerItems[index].Animator != null)
                    {
                        Editor_xHudGUI.Gui_Property_Field(new Rect(rect.x + 125, baseheight - 5, 75, 19), "D", sp_Delay, 10, 40, LineHeight, 15);
                    }
                    #endregion

                    #region 速率
                    if (BaseScript.ContainerItems[index].Animator != null)
                    {
                        SerializedObject so_anim = new SerializedObject(BaseScript.ContainerItems[index].Animator);

                        SerializedProperty sp_glodur = so_anim.FindProperty("Animator_GlobalDuration");
                        SerializedProperty sp_maxdur = so_anim.FindProperty("MaxTimerWithGlobalDuration");

                        so_anim.Update();

                        Editor_xHudGUI.Gui_Property_Field(new Rect(rect.x + 75, baseheight - 5, 75, 19), "G", sp_glodur, 10, 40, LineHeight, 15);
                        Editor_xHudGUI.Gui_Labelfield_Thin(new Rect(rect.x + 160, baseheight - 5, 75, 19), $"{sp_maxdur.floatValue.ToString()} s", HudFilled.无, HudColor.无, xHud_Dashboard.Theme_Primary, TextAnchor.MiddleCenter, Vector2.zero, 11);

                        so_anim.ApplyModifiedProperties();
                    }
                    #endregion

                    #region 重新生成ID
                    if (Editor_xHudGUI.Gui_Button(new Rect(rect.width + 32, baseheight, 13, 13), reid_r, reid_p, true, "", "", Color.white))
                    {
                        int id = Dv_CreateID(BaseScript.ContainerItems);
                        sp_id.intValue = id;
                        sp_id.serializedObject.ApplyModifiedProperties();
                    }
                    #endregion

                    #region 类型名称                                            
                    string abbr = "";
                    switch (type)
                    {
                        case ContainerType.None:
                            abbr = "None";
                            break;
                        case ContainerType.Text:
                            abbr = "Txt";
                            break;
                        case ContainerType.TmpText:
                            abbr = "Tmp";
                            break;
                        case ContainerType.Image:
                            abbr = "Img";
                            break;
                        case ContainerType.RawImage:
                            abbr = "Raw";
                            break;
                    }
                    GUI.color = Color.gray;
                    Editor_xHudGUI.Gui_Labelfield(new Rect(rect.width - 85, rect.y + 5, 100, 15), abbr, HudFilled.无, HudColor.无, Color.white, TextAnchor.MiddleRight, Vector2.zero, 9, true);
                    GUI.color = Color.white;
                    #endregion                   

                    sp_id.serializedObject.ApplyModifiedProperties();
                    sp_Type.serializedObject.ApplyModifiedProperties();
                    sp_Indicator.serializedObject.ApplyModifiedProperties();
                    sp_Root.serializedObject.ApplyModifiedProperties();

                    ContainerItems.serializedObject.ApplyModifiedProperties();
                },
                onAddCallback = (ReorderableList list) =>
                {
                    ContainerItems.InsertArrayElementAtIndex(list.count);
                    int index = list.count - 1;
                    SerializedProperty dts = ContainerItems.GetArrayElementAtIndex(index);

                    dts.FindPropertyRelative("Indicator").stringValue = "NewDataItem";
                    dts.FindPropertyRelative("ID").intValue = Dv_CreateID(BaseScript.ContainerItems);
                    dts.FindPropertyRelative("Type").enumValueIndex = 0;
                    dts.FindPropertyRelative("DelayTime").floatValue = 0;

                    SerializedProperty sp_animator = dts.FindPropertyRelative("Animator");

                    SerializedProperty sp_Text = dts.FindPropertyRelative("Text");
                    SerializedProperty sp_Text_text = sp_Text.FindPropertyRelative("Base");

                    SerializedProperty sp_TmpText = dts.FindPropertyRelative("TmpText");
                    SerializedProperty sp_TmpText_text = sp_TmpText.FindPropertyRelative("Base");

                    SerializedProperty sp_Image = dts.FindPropertyRelative("Image");
                    SerializedProperty sp_Image_image = sp_Image.FindPropertyRelative("Base");

                    SerializedProperty sp_RawImage = dts.FindPropertyRelative("RawImage");
                    SerializedProperty sp_RawImage_img = sp_RawImage.FindPropertyRelative("Base");

                    sp_Text_text.objectReferenceValue = null;
                    sp_TmpText_text.objectReferenceValue = null;
                    sp_Image_image.objectReferenceValue = null;
                    sp_RawImage_img.objectReferenceValue = null;
                    sp_animator.objectReferenceValue = null;

                    sp_Text_text.serializedObject.ApplyModifiedProperties();
                    sp_TmpText_text.serializedObject.ApplyModifiedProperties();
                    sp_Image_image.serializedObject.ApplyModifiedProperties();
                    sp_RawImage_img.serializedObject.ApplyModifiedProperties();

                    dts.serializedObject.ApplyModifiedProperties();
                },
                onRemoveCallback = (ReorderableList list) =>
                {
                    ContainerItems.DeleteArrayElementAtIndex(list.index);
                },
                onSelectCallback = (ReorderableList list) =>
                {
                    SerializedProperty sp_Root = ContainerItems.GetArrayElementAtIndex(list.index);

                    SerializedProperty sp_id = sp_Root.FindPropertyRelative("ID");

                    SerializedProperty sp_Text = sp_Root.FindPropertyRelative("Text");
                    SerializedProperty sp_Text_text = sp_Text.FindPropertyRelative("Base");

                    SerializedProperty sp_TmpText = sp_Root.FindPropertyRelative("TmpText");
                    SerializedProperty sp_TmpText_text = sp_TmpText.FindPropertyRelative("Base");

                    SerializedProperty sp_Image = sp_Root.FindPropertyRelative("Image");
                    SerializedProperty sp_Image_image = sp_Image.FindPropertyRelative("Base");

                    SerializedProperty sp_RawImage = sp_Root.FindPropertyRelative("RawImage");
                    SerializedProperty sp_RawImage_img = sp_RawImage.FindPropertyRelative("Base");

                    if (sp_Root != null && sp_Text_text.objectReferenceValue != null)
                    {
                        EditorGUIUtility.PingObject(sp_Text_text.objectReferenceValue);
                    }
                    if (sp_Root != null && sp_TmpText_text.objectReferenceValue != null)
                    {
                        EditorGUIUtility.PingObject(sp_TmpText_text.objectReferenceValue);
                    }
                    if (sp_Root != null && sp_Image_image.objectReferenceValue != null)
                    {
                        EditorGUIUtility.PingObject(sp_Image_image.objectReferenceValue);
                    }
                    if (sp_Root != null && sp_RawImage_img.objectReferenceValue != null)
                    {
                        EditorGUIUtility.PingObject(sp_RawImage_img.objectReferenceValue);
                    }
                    if (!Application.isPlaying)
                    {
                        Preview_Animator_Stop();
                    }
                    Preview_Animator_PlayAt(list.index);
                },
                elementHeightCallback = index =>
                {
                    SerializedProperty sp_Root = ContainerItems.GetArrayElementAtIndex(index);

                    SerializedProperty sp_id = sp_Root.FindPropertyRelative("ID");
                    SerializedProperty sp_Type = sp_Root.FindPropertyRelative("Type");
                    SerializedProperty sp_Indicator = sp_Root.FindPropertyRelative("Indicator");

                    float height = 3;
                    float add = 0;

                    #region 分类
                    ContainerType type = (ContainerType)sp_Type.enumValueIndex;
                    switch (type)
                    {
                        case ContainerType.None:
                            height = 3f + add;
                            break;
                        case ContainerType.Text:
                            height = 3f + add;
                            break;
                        case ContainerType.TmpText:
                            height = 3f + add;
                            break;
                        case ContainerType.Image:
                            height = 3f + add;
                            break;
                    }
                    #endregion

                    return height * LineHeight;
                }
            };
            #endregion

            GetContainTypes();

            GetAllTargets();
        }

        private void OnDisable()
        {
            if (!Application.isPlaying)
            {
                AllListFoldState(false);

                try
                {
                    Preview_Animator_Stop();

                    Preview_AnimatorSound_CoroutineList_Stop();
                }
                catch (System.Exception e)
                {
                    string msg = e.Message;
                }
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (string.IsNullOrEmpty(Indicator.stringValue))
                Editor_xHudGUI.Gui_Layout_Banner(icon_main, HudFilled.实体, HudColor.深空灰, "Hud - 容器", Color.white);
            else
                Editor_xHudGUI.Gui_Layout_Banner(icon_main, HudFilled.实体, HudColor.深空灰, $"{BaseScript.name} ( {BaseScript.Indicator} )", Color.white);

            xHud_Manager mgr = xHud_Dashboard.HudManagerGet();

            #region 快捷功能
            Editor_xHudGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "快捷功能", xHud_Dashboard.Theme_Primary);
            Editor_xHudGUI.Gui_Layout_Space(10);

            Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_xHudGUI.Gui_Layout_Space(10);

            #region 获取子元素
            if (Editor_xHudGUI.Gui_Layout_Button(14, "获取子元素", scan_r, scan_p))
            {
                if (Application.isPlaying)
                {
                    Editor_xHudGUI.Open(xHudDialogType.警告, "HudContainer容器消息", "获取容器元素", "程序正在运行，无法在运行期间执行此功能！", "明白");
                    return;
                }

                AutoCollects();
                GetContainTypes();
                return;
            }
            #endregion

            GUILayout.FlexibleSpace();

            #region 重新生成ID           
            if (Editor_xHudGUI.Gui_Layout_Button(14, "重新生成ID", reid_r, reid_p))
            {
                if (Application.isPlaying)
                {
                    Debug.Log("程序正在运行，无法在运行期间执行此功能！");
                    return;
                }
                RegenerateID();
            }
            #endregion

            GUILayout.FlexibleSpace();

            #region 清空                       
            if (Editor_xHudGUI.Gui_Layout_Button(14, "清空元素项", clear_r, clear_p))
            {
                if (Application.isPlaying)
                {
                    Editor_xHudGUI.Open(xHudDialogType.警告, "HudContainer容器消息", "清空容器元素", "程序正在运行，无法在运行期间执行此功能！", "明白");
                    return;
                }
                if (ContainerItems.arraySize <= 0)
                    return;
                ClearDataItems();
                GetContainTypes();
                return;
            }
            #endregion

            #region 预览动画
            if (!IsMultiSelected() && ContainerItems.arraySize > 0)
            {
                if (!IsMultiSelected())
                {
                    GUILayout.FlexibleSpace();

                    if (AnimateState.enumValueIndex == 0)
                    {
                        if (Editor_xHudGUI.Gui_Layout_Button(14, "播放所有动画器预览", prw_play_r, prw_play_p))
                        {
                            if (Application.isPlaying)
                            {
                                Debug.Log("程序正在运行，无法在运行期间执行此功能！");
                                return;
                            }
                            Preview_Animator_Play();
                        }
                    }
                    else
                    {
                        if (Editor_xHudGUI.Gui_Layout_Button(14, "停止所有动画器预览", prw_stop_r, prw_stop_p))
                        {
                            if (Application.isPlaying)
                            {
                                Debug.Log("程序正在运行，无法在运行期间执行此功能！");
                                return;
                            }
                            Preview_Animator_Stop();
                        }
                    }
                }
            }
            #endregion

            Editor_xHudGUI.Gui_Layout_Space(10);
            Editor_xHudGUI.Gui_Layout_Horizontal_End();

            Editor_xHudGUI.Gui_Layout_Space(10);
            Editor_xHudGUI.Gui_Layout_Vertical_End();
            #endregion

            Editor_xHudGUI.Gui_Layout_Space(10);

            #region 参数
            Editor_xHudGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "参数", xHud_Dashboard.Theme_Primary);
            Editor_xHudGUI.Gui_Layout_Space(10);

            #region 标识名称     
            Editor_xHudGUI.Gui_Layout_Property_Field("标识名称", Indicator);
            #endregion

            Editor_xHudGUI.Gui_Layout_Space(5);

            #region 速率倍增     
            Editor_xHudGUI.Gui_Layout_Property_Field("速率倍增", Animators_GlobalDuration);
            #endregion

            Editor_xHudGUI.Gui_Layout_Space(5);

            #region 动画播放时机
            Editor_xHudGUI.Gui_Layout_Popup<string, xHud_Module_Container>("播放时机", stroptions_clickthreadhold, ref AnimatorPlayTiming, HudFilled.实体, 120, 22, SelectedObjects);
            #endregion

            Editor_xHudGUI.Gui_Layout_Space(10);
            Editor_xHudGUI.Gui_Layout_Vertical_End();
            #endregion

            #region 选项
            Editor_xHudGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "选项", xHud_Dashboard.Theme_Primary);
            Editor_xHudGUI.Gui_Layout_Space(10);

            #region 状态调试
            Editor_xHudGUI.Gui_Layout_Toggle<bool, xHud_Module_Container>("状态调试", stroptions_debug, ref UseDebug, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
            #endregion

            #region 自动停止预览
            Editor_xHudGUI.Gui_Layout_Toggle<bool, xHud_Module_Container>("自动停止预览", stroptions_enabled, ref AutoStopPreview, HudFilled.无, HudFilled.实体, Color.white, 120, 22, SelectedObjects);
            #endregion

            Editor_xHudGUI.Gui_Layout_Space(10);
            Editor_xHudGUI.Gui_Layout_Vertical_End();
            #endregion

            #region 容器项列表
            Editor_xHudGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "容器项列表", xHud_Dashboard.Theme_Primary);
            Editor_xHudGUI.Gui_Layout_Space(5);

            #region 容器项列表
            if (IsMultiSelected())
            {
                Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_xHudGUI.Gui_Layout_Space(10);
                EditorGUILayout.HelpBox("容器项列表不支持多项操作", MessageType.Warning);
                Editor_xHudGUI.Gui_Layout_Space(5);
                Editor_xHudGUI.Gui_Layout_Horizontal_End();
            }
            else
            {
                Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_xHudGUI.Gui_Layout_Space(5);
                list_Container.DoLayoutList();
                Editor_xHudGUI.Gui_Layout_Space(5);
                Editor_xHudGUI.Gui_Layout_Horizontal_End();
            }
            #endregion

            Editor_xHudGUI.Gui_Layout_Space(5);
            Editor_xHudGUI.Gui_Layout_Vertical_End();
            #endregion

            #region 状态
            string statu_title = "状态";
            if (IsMultiSelected())
                statu_title = "状态 - ( 批量模式 )";
            Editor_xHudGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.阴影灰, 5, statu_title, xHud_Dashboard.Theme_Primary);
            Editor_xHudGUI.Gui_Layout_Space(5);

            if (!IsMultiSelected())
            {
                #region 动画状态     
                Editor_xHudGUI.StatuDisplayer_text(animstate, 12, new Vector2(0, 7), "动画状态", 12, (HudElementAnimateState)AnimateState.enumValueIndex == HudElementAnimateState.Animating ? "动画中" : "静止状态", AnimateState.enumValueIndex == 1 ? xHud_Dashboard.Theme_Primary : Color.gray, 11);
                #endregion

                #region 最大耗时 
                Editor_xHudGUI.StatuDisplayer_text(timer_max, 12, new Vector2(0, 7), "最大耗时<color=#909090>（速率倍增）</color>", 12, Animators_MaxDuration.floatValue.ToString() + " 秒", xHud_Dashboard.Theme_Primary, 11);
                #endregion

                #region 最大耗时 
                Editor_xHudGUI.StatuDisplayer_text(timer_max, 12, new Vector2(0, 7), "最大耗时<color=#909090>（XHUD倍增）</color>", 12, (Animators_MaxDuration.floatValue * mgr.DurationMultiply).ToString() + " 秒", xHud_Dashboard.Theme_Primary, 11);
                #endregion
            }
            else
            {
                #region 批量控件
                Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_xHudGUI.Gui_Layout_Space(10);
                if (Editor_xHudGUI.Gui_Layout_Button($"{SelectedObjects[Containner_Status_Index].name} ( {SelectedObjects[Containner_Status_Index].Indicator} )", "", HudFilled.透明, HudColor.无, Color.gray, 20))
                {
                    EditorGUIUtility.PingObject(SelectedObjects[Containner_Status_Index]);
                }
                Editor_xHudGUI.Gui_Layout_FlexSpace();
                if (Editor_xHudGUI.Gui_Layout_Button(12, "", left_arrow_r, left_arrow_p))
                {
                    if (Containner_Status_Index <= 0)
                    {
                        Containner_Status_Index = SelectedObjects.Length - 1;
                    }
                    else
                    {
                        Containner_Status_Index--;
                    }
                    EditorGUIUtility.PingObject(SelectedObjects[Containner_Status_Index]);
                }
                Editor_xHudGUI.Gui_Layout_Space(16);
                if (Editor_xHudGUI.Gui_Layout_Button(12, "", right_arrow_r, right_arrow_p))
                {
                    if (Containner_Status_Index >= SelectedObjects.Length - 1)
                    {
                        Containner_Status_Index = 0;
                    }
                    else
                    {
                        Containner_Status_Index++;
                    }
                    EditorGUIUtility.PingObject(SelectedObjects[Containner_Status_Index]);
                }
                Editor_xHudGUI.Gui_Layout_Space(5);
                Editor_xHudGUI.Gui_Layout_Horizontal_End();
                #endregion

                #region 动画状态     
                Editor_xHudGUI.StatuDisplayer_text(animstate, 12, new Vector2(0, 7), "动画状态", 12, SelectedObjects[Containner_Status_Index].AnimateState == HudElementAnimateState.Animating ? "动画中" : "静止状态", AnimateState.enumValueIndex == 1 ? xHud_Dashboard.Theme_Primary : Color.gray, 11);
                #endregion

                SelectedObjects[Containner_Status_Index].Animators_MaxDuration = Animators_GetAnimatorsMaxDuration(SelectedObjects[Containner_Status_Index].ContainerItems, SelectedObjects[Containner_Status_Index].Animators_GlobalDuration);

                #region 最大耗时 
                Editor_xHudGUI.StatuDisplayer_text(timer_max, 12, new Vector2(0, 7), "最大耗时<color=#909090>（速率倍增）</color>", 12, SelectedObjects[Containner_Status_Index].Animators_MaxDuration.ToString() + " 秒", xHud_Dashboard.Theme_Primary, 11);
                #endregion

                #region 最大耗时 
                Editor_xHudGUI.StatuDisplayer_text(timer_max, 12, new Vector2(0, 7), "最大耗时<color=#909090>（XHUD倍增）</color>", 12, (SelectedObjects[Containner_Status_Index].Animators_MaxDuration * mgr.DurationMultiply).ToString() + " 秒", xHud_Dashboard.Theme_Primary, 11);
                #endregion
            }

            Editor_xHudGUI.Gui_Layout_Space(5);
            Editor_xHudGUI.Gui_Layout_Vertical_End();
            #endregion

            #region 统计
            Editor_xHudGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "统计", xHud_Dashboard.Theme_Primary);
            Editor_xHudGUI.Gui_Layout_Space(5);

            if (ContainerItems.arraySize <= 0)
            {
                Editor_xHudGUI.Gui_Layout_Labelfield("暂无统计数据", HudFilled.无, HudColor.无, Editor_xHudGUI.GetColor(HudColor.阴影灰), TextAnchor.MiddleCenter);
            }
            else
            {
                #region 容器项     
                Editor_xHudGUI.StatuDisplayer_text(conitem, 12, new Vector2(0, 7), "容器项", 12, ContainerItems.arraySize.ToString() + " 个", xHud_Dashboard.Theme_Primary, 11);
                #endregion

                #region 动画器数量     
                Editor_xHudGUI.StatuDisplayer_text(icon_anim, 12, new Vector2(0, 7), "动画器", 12, AnimatorCount.ToString() + " 个", xHud_Dashboard.Theme_Primary, 11);
                #endregion

                #region 文字数量    
                Editor_xHudGUI.StatuDisplayer_text(icon_text, 12, new Vector2(0, 7), "文字", 12, TextCount.ToString() + " 个", xHud_Dashboard.Theme_Primary, 11);
                #endregion

                #region Tmp文字数量     
                Editor_xHudGUI.StatuDisplayer_text(icon_tmptext, 12, new Vector2(0, 7), "Tmp文字", 12, TmpTextCount.ToString() + " 个", xHud_Dashboard.Theme_Primary, 11);
                #endregion

                #region 图像数量     
                Editor_xHudGUI.StatuDisplayer_text(icon_image, 12, new Vector2(0, 7), "图像", 12, ImageCount.ToString() + " 个", xHud_Dashboard.Theme_Primary, 11);
                #endregion

                #region Raw图像数量     
                Editor_xHudGUI.StatuDisplayer_text(icon_rawimage, 12, new Vector2(0, 7), "Raw图像", 12, RawImageCount.ToString() + " 个", xHud_Dashboard.Theme_Primary, 11);
                #endregion
            }

            Editor_xHudGUI.Gui_Layout_Space(5);
            Editor_xHudGUI.Gui_Layout_Vertical_End();
            #endregion

            #region 列表
            Editor_xHudGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "事件/列表", xHud_Dashboard.Theme_Primary);
            Editor_xHudGUI.Gui_Layout_Space(5);

            #region 事件列表
            if (IsMultiSelected())
            {
                Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_xHudGUI.Gui_Layout_Space(10);
                EditorGUILayout.HelpBox("事件不支持多项操作", MessageType.Warning);
                Editor_xHudGUI.Gui_Layout_Space(5);
                Editor_xHudGUI.Gui_Layout_Horizontal_End();
            }
            else
            {
                Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_xHudGUI.Gui_Layout_Space(10);
                EventIsFold.boolValue = EditorGUILayout.Foldout(EventIsFold.boolValue, "事件", true);
                Editor_xHudGUI.Gui_Layout_Space(5);
                Editor_xHudGUI.Gui_Layout_Horizontal_End();

                if (EventIsFold.boolValue)
                {
                    Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_itemcount_get);
                    eve_on_itemcount_get.serializedObject.ApplyModifiedProperties();
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    Editor_xHudGUI.Gui_Layout_Horizontal_End();

                    Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_item_get);
                    eve_on_item_get.serializedObject.ApplyModifiedProperties();
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    Editor_xHudGUI.Gui_Layout_Horizontal_End();

                    Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_xHudGUI.Gui_Layout_Space(5);

                    EditorGUILayout.PropertyField(eve_on_items_get);
                    eve_on_items_get.serializedObject.ApplyModifiedProperties();
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    Editor_xHudGUI.Gui_Layout_Horizontal_End();

                    Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_item_add);
                    eve_on_item_add.serializedObject.ApplyModifiedProperties();
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    Editor_xHudGUI.Gui_Layout_Horizontal_End();

                    Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_item_remove);
                    eve_on_item_remove.serializedObject.ApplyModifiedProperties();
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    Editor_xHudGUI.Gui_Layout_Horizontal_End();

                    Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_items_remove);
                    eve_on_items_remove.serializedObject.ApplyModifiedProperties();
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    Editor_xHudGUI.Gui_Layout_Horizontal_End();

                    Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_animate_play_all);
                    eve_on_animate_play_all.serializedObject.ApplyModifiedProperties();
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    Editor_xHudGUI.Gui_Layout_Horizontal_End();

                    Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_animate_play_by_item);
                    eve_on_animate_play_by_item.serializedObject.ApplyModifiedProperties();
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    Editor_xHudGUI.Gui_Layout_Horizontal_End();

                    Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_animate_rewind_all);
                    eve_on_animate_rewind_all.serializedObject.ApplyModifiedProperties();
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    Editor_xHudGUI.Gui_Layout_Horizontal_End();

                    Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_animate_rewind_by_item);
                    eve_on_animate_rewind_by_item.serializedObject.ApplyModifiedProperties();
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    Editor_xHudGUI.Gui_Layout_Horizontal_End();

                    Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_animate_ready_all);
                    eve_on_animate_ready_all.serializedObject.ApplyModifiedProperties();
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    Editor_xHudGUI.Gui_Layout_Horizontal_End();

                    Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_animate_ready_by_item);
                    eve_on_animate_ready_by_item.serializedObject.ApplyModifiedProperties();
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    Editor_xHudGUI.Gui_Layout_Horizontal_End();

                    Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    EditorGUILayout.PropertyField(eve_on_item_changevalue);
                    eve_on_item_changevalue.serializedObject.ApplyModifiedProperties();
                    Editor_xHudGUI.Gui_Layout_Space(5);
                    Editor_xHudGUI.Gui_Layout_Horizontal_End();
                }
            }
            #endregion

            Editor_xHudGUI.Gui_Layout_Space(5);
            Editor_xHudGUI.Gui_Layout_Vertical_End();
            #endregion

            #region 检查是否存在无效的容器组件
            CheckItemsValid();
            #endregion

            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                // 创建右键菜单
                GenericMenu menu = new GenericMenu();
                menu.AddDisabledItem(new GUIContent("组件"));
                menu.AddItem(new GUIContent("X (扫描)"), false, () =>
                {
                    if (Application.isPlaying)
                    {
                        Debug.Log("程序正在运行，无法在运行期间执行此功能！");
                        return;
                    }
                    AutoCollects();
                    GetContainTypes();
                });
                menu.AddSeparator("");
                menu.AddDisabledItem(new GUIContent("ID"));
                menu.AddItem(new GUIContent("D (更新)"), false, () =>
                {
                    if (Application.isPlaying)
                    {
                        Debug.Log("程序正在运行，无法在运行期间执行此功能！");
                        return;
                    }

                    RegenerateID();
                });

                if (!IsMultiSelected())
                {
                    menu.AddSeparator("");
                    menu.AddDisabledItem(new GUIContent("预览"));
                    if (AnimateState.enumValueIndex == 0)
                        menu.AddItem(new GUIContent("S (开始)"), false, () =>
                        {
                            if (Application.isPlaying)
                            {
                                Debug.Log("程序正在运行，无法在运行期间执行此功能！");
                                return;
                            }
                            Preview_Animator_Play();
                        });
                    else
                        menu.AddItem(new GUIContent("S (停止)"), false, () =>
                        {
                            if (Application.isPlaying)
                            {
                                Debug.Log("程序正在运行，无法在运行期间执行此功能！");
                                return;
                            }
                            Preview_Animator_Stop();
                        });
                }

                if (PrefabUtility.IsAnyPrefabInstanceRoot(BaseScript.gameObject))
                {
                    menu.AddSeparator("");
                    menu.AddDisabledItem(new GUIContent("预制体"));
                    menu.AddItem(new GUIContent("A (应用)"), false, () =>
                    {
                        if (Application.isPlaying)
                        {
                            Debug.Log("程序正在运行，无法在运行期间执行此功能！");
                            return;
                        }
                        string path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(BaseScript.gameObject);
                        PrefabUtility.SaveAsPrefabAssetAndConnect(BaseScript.gameObject, path, InteractionMode.AutomatedAction);
                    });
                    menu.AddItem(new GUIContent("Z (定位)"), false, () =>
                    {
                        if (Application.isPlaying)
                        {
                            Debug.Log("程序正在运行，无法在运行期间执行此功能！");
                            return;
                        }

                        string path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(BaseScript.gameObject);
                        //Selection.activeObject = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
                        Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
                        EditorGUIUtility.PingObject(obj);
                    });
                }
                menu.ShowAsContext(); // 在鼠标位置显示右键菜单
            }

            #region 源脚本
            Editor_xHudGUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 3, "源脚本", xHud_Dashboard.Theme_Primary);
            Editor_xHudGUI.Gui_Layout_Space(5);

            #region 原始变量
            Editor_xHudGUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_xHudGUI.Gui_Layout_Space(10);
            BasicVars = EditorGUILayout.Foldout(BasicVars, "变量/属性", true);
            Editor_xHudGUI.Gui_Layout_Space(5);
            Editor_xHudGUI.Gui_Layout_Horizontal_End();
            if (BasicVars)
                DrawDefaultInspector();
            #endregion

            Editor_xHudGUI.Gui_Layout_Space(5);
            Editor_xHudGUI.Gui_Layout_Vertical_End();
            #endregion

            CalculateAnimatorMaxDuration(BaseScript.ContainerItems, Animators_GlobalDuration.floatValue);

            serializedObject.ApplyModifiedProperties();
        }

        #region 辅助
        /// <summary>
        /// 检查是否存在无效的容器组件
        /// </summary>
        private void CheckItemsValid()
        {
            for (int i = 0; i < ContainerItems.arraySize; i++)
            {
                SerializedProperty sp_animotrNode = ContainerItems.GetArrayElementAtIndex(i);
                SerializedProperty sp_transform = sp_animotrNode.FindPropertyRelative("Transform");

                if (sp_transform.objectReferenceValue == null)
                {
                    ContainerItems.DeleteArrayElementAtIndex(i);
                }
            }
        }

        /// <summary>
        /// 清空数据项
        /// </summary>
        private void ClearDataItems()
        {
            string res = Editor_xHudGUI.Open(xHudDialogType.确认, "HudContainer容器消息", "清空容器元素", "此操作会清空所有已获取的容器元素列表！", "清空", "暂不", 0);

            if (res == "清空")
            {
                if (IsMultiSelected())
                {
                    for (int s = 0; s < SelectedObjects.Length; s++)
                    {
                        SerializedObject so = new SerializedObject(SelectedObjects[s]);
                        so.Update();

                        SerializedProperty sp_Cons = so.FindProperty("ContainerItems");
                        sp_Cons.ClearArray();
                        sp_Cons.serializedObject.ApplyModifiedProperties();
                        so.ApplyModifiedProperties();
                    }
                }
                else
                {
                    ContainerItems.ClearArray();
                    ContainerItems.serializedObject.ApplyModifiedProperties();
                }
            }
        }

        /// <summary>
        /// 重复项检测
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private int IsExistItem_Get_Status(int id, List<HudContainerItem> list)
        {
            int isRepeat = -1;
            ///遍历现有的数据项
            for (int s = 0; s < list.Count; s++)
            {
                HudContainerItem item = list[s];

                if (id == item.Transform.GetInstanceID())
                {
                    isRepeat = s;
                    break;
                }
            }
            return isRepeat;
        }

        /// <summary>
        /// 自动收集元素
        /// </summary>
        private void AutoCollects()
        {
            List<xHudGUI_Dialog_ListDatas> Datas = new List<xHudGUI_Dialog_ListDatas>();

            if (IsMultiSelected())
            {
                for (int s = 0; s < SelectedObjects.Length; s++)
                {
                    SerializedObject so = new SerializedObject(SelectedObjects[s]);
                    so.Update();

                    SerializedProperty sp_Cons = so.FindProperty("ContainerItems");

                    Transform[] allChildren = SelectedObjects[s].transform.GetComponentsInChildren<Transform>();
                    Transform[] trans = new Transform[allChildren.Length - 1];
                    Array.Copy(allChildren, 1, trans, 0, trans.Length);

                    ///遍历所有子物体
                    for (int i = 0; i < trans.Length; i++)
                    {
                        ///获取每一个子物体
                        RectTransform trs = trans[i] as RectTransform;

                        ///如果物体带有SoftMask则跳过
                        SoftMask t_mask = trs.GetComponent<SoftMask>();
                        if (t_mask != null)
                            continue;

                        int statuid = IsExistItem_Get_Status(trs.GetInstanceID(), SelectedObjects[s].ContainerItems);

                        if (statuid == -1)
                        {
                            if (sp_Cons.arraySize <= 0)
                                sp_Cons.InsertArrayElementAtIndex(0);
                            else
                                sp_Cons.InsertArrayElementAtIndex(sp_Cons.arraySize - 1);

                            SerializedProperty sp_Root = sp_Cons.GetArrayElementAtIndex(sp_Cons.arraySize - 1);

                            #region 获取属性
                            SerializedProperty sp_id = sp_Root.FindPropertyRelative("ID");
                            SerializedProperty sp_Type = sp_Root.FindPropertyRelative("Type");
                            SerializedProperty sp_Indicator = sp_Root.FindPropertyRelative("Indicator");
                            SerializedProperty sp_Transform = sp_Root.FindPropertyRelative("Transform");
                            SerializedProperty sp_animator = sp_Root.FindPropertyRelative("Animator");

                            SerializedProperty sp_Text = sp_Root.FindPropertyRelative("Text");
                            SerializedProperty sp_Text_text = sp_Text.FindPropertyRelative("Base");

                            SerializedProperty sp_TmpText = sp_Root.FindPropertyRelative("TmpText");
                            SerializedProperty sp_TmpText_text = sp_TmpText.FindPropertyRelative("Base");

                            SerializedProperty sp_Image = sp_Root.FindPropertyRelative("Image");
                            SerializedProperty sp_Image_img = sp_Image.FindPropertyRelative("Base");

                            SerializedProperty sp_RawImage = sp_Root.FindPropertyRelative("RawImage");
                            SerializedProperty sp_RawImage_img = sp_RawImage.FindPropertyRelative("Base");
                            #endregion

                            sp_id.intValue = Dv_CreateID(BaseScript.ContainerItems);
                            sp_Indicator.stringValue = trs.name;

                            #region 清空物体
                            sp_Text_text.objectReferenceValue = null;
                            sp_TmpText_text.objectReferenceValue = null;
                            sp_Image_img.objectReferenceValue = null;
                            sp_RawImage_img.objectReferenceValue = null;
                            sp_animator.objectReferenceValue = null;
                            sp_Transform.objectReferenceValue = null;
                            #endregion

                            #region 获取组件
                            xHud_Module_Text t_text = trs.GetComponent<xHud_Module_Text>();
                            xHud_Module_TmpText t_tmptext = trs.GetComponent<xHud_Module_TmpText>();
                            Image t_image = trs.GetComponent<Image>();
                            RawImage t_rawimage = trs.GetComponent<RawImage>();
                            xHud_Module_Animator animator = trs.GetComponent<xHud_Module_Animator>();
                            sp_Transform.objectReferenceValue = trs.GetComponent<RectTransform>();
                            #endregion

                            #region 类型判断

                            //如果存在文字组件
                            if (t_text != null)
                            {
                                sp_Type.enumValueIndex = (int)ContainerType.Text;
                                sp_Text_text.objectReferenceValue = t_text;
                            }
                            //如果存在Tmp文字组件
                            else if (t_tmptext != null)
                            {
                                sp_Type.enumValueIndex = (int)ContainerType.TmpText;
                                sp_TmpText_text.objectReferenceValue = t_tmptext;
                            }
                            //如果存在图像组件
                            else if (t_image != null)
                            {
                                sp_Type.enumValueIndex = (int)ContainerType.Image;
                                sp_Image_img.objectReferenceValue = t_image;
                            }
                            //如果存在Raw图像组件
                            else if (t_rawimage != null)
                            {
                                sp_Type.enumValueIndex = (int)ContainerType.RawImage;
                                sp_RawImage_img.objectReferenceValue = t_rawimage;
                            }
                            //如果不存在文字组件或者Tmp文字组件以及图像组件
                            else
                            {
                                sp_Type.enumValueIndex = (int)ContainerType.None;
                            }

                            if (animator != null)
                                sp_animator.objectReferenceValue = animator;
                            #endregion

                            #region 保存属性
                            sp_Transform.serializedObject.ApplyModifiedProperties();
                            sp_animator.serializedObject.ApplyModifiedProperties();
                            sp_Text_text.serializedObject.ApplyModifiedProperties();
                            sp_TmpText_text.serializedObject.ApplyModifiedProperties();
                            sp_Image_img.serializedObject.ApplyModifiedProperties();
                            sp_RawImage_img.serializedObject.ApplyModifiedProperties();
                            sp_Type.serializedObject.ApplyModifiedProperties();
                            sp_id.serializedObject.ApplyModifiedProperties();
                            sp_Indicator.serializedObject.ApplyModifiedProperties();
                            #endregion

                            #region 添加到弹窗列表
                            xHudGUI_Dialog_ListDatas dataitem = new xHudGUI_Dialog_ListDatas();

                            dataitem.Title = $"{SelectedObjects[s].name} ( {SelectedObjects[s].Indicator} )";
                            dataitem.SubTitle = "添加新组件";
                            dataitem.Message = $"{trs.name} ( {sp_Indicator.stringValue} )";

                            Datas.Add(dataitem);
                            #endregion
                        }
                        else
                        {
                            #region 获取属性
                            SerializedProperty sp_Root = sp_Cons.GetArrayElementAtIndex(statuid);
                            SerializedProperty sp_Indicator = sp_Root.FindPropertyRelative("Indicator");
                            SerializedProperty sp_Transform = sp_Root.FindPropertyRelative("Transform");
                            SerializedProperty sp_animator = sp_Root.FindPropertyRelative("Animator");
                            #endregion

                            #region 重新尝试获取Animator
                            if (sp_animator.objectReferenceValue == null)
                            {
                                RectTransform anim_obj = (RectTransform)sp_Transform.objectReferenceValue;
                                xHud_Module_Animator anim = anim_obj.GetComponent<xHud_Module_Animator>();
                                if (anim != null)
                                {
                                    sp_animator.objectReferenceValue = anim;
                                    sp_animator.serializedObject.ApplyModifiedProperties();
                                }
                            }
                            #endregion

                            #region 添加到弹窗列表
                            xHudGUI_Dialog_ListDatas dataitem = new xHudGUI_Dialog_ListDatas();

                            dataitem.Title = $"{SelectedObjects[s].name} ( {SelectedObjects[s].Indicator} )";
                            dataitem.SubTitle = "更新已有项";
                            dataitem.Message = $"{trs.name} ( {sp_Indicator.stringValue} )";
                            Datas.Add(dataitem);
                            #endregion
                        }
                    }

                    sp_Cons.serializedObject.ApplyModifiedProperties();
                    so.ApplyModifiedProperties();
                }

                Editor_xHudGUI.Open(Datas.ToArray(), xHudDialogType.确认, "HudContainer容器消息", "批量扫描容器元素", "以下是批量扫描到的所有元素列表，请您检查核对：", "明白");
            }
            else
            {
                Transform[] allChildren = BaseScript.transform.GetComponentsInChildren<Transform>();
                Transform[] trans = new Transform[allChildren.Length - 1];
                Array.Copy(allChildren, 1, trans, 0, trans.Length);

                ///遍历所有子物体 - 添加有效组件
                for (int i = 0; i < trans.Length; i++)
                {
                    ///获取每一个子物体
                    RectTransform trs = trans[i] as RectTransform;

                    ///如果物体带有SoftMask则跳过
                    SoftMask t_mask = trs.GetComponent<SoftMask>();
                    if (t_mask != null)
                        continue;

                    int statuid = IsExistItem_Get_Status(trs.GetInstanceID(), BaseScript.ContainerItems);

                    //如果不存在就新增项，反之更新现有项
                    if (statuid == -1)
                    {

                        if (ContainerItems.arraySize <= 0)
                            ContainerItems.InsertArrayElementAtIndex(0);
                        else
                            ContainerItems.InsertArrayElementAtIndex(ContainerItems.arraySize - 1);

                        SerializedProperty sp_Root = ContainerItems.GetArrayElementAtIndex(ContainerItems.arraySize - 1);

                        #region 获取属性
                        SerializedProperty sp_id = sp_Root.FindPropertyRelative("ID");
                        SerializedProperty sp_Type = sp_Root.FindPropertyRelative("Type");
                        SerializedProperty sp_Indicator = sp_Root.FindPropertyRelative("Indicator");
                        SerializedProperty sp_Transform = sp_Root.FindPropertyRelative("Transform");
                        SerializedProperty sp_animator = sp_Root.FindPropertyRelative("Animator");

                        SerializedProperty sp_Text = sp_Root.FindPropertyRelative("Text");
                        SerializedProperty sp_Text_text = sp_Text.FindPropertyRelative("Base");

                        SerializedProperty sp_TmpText = sp_Root.FindPropertyRelative("TmpText");
                        SerializedProperty sp_TmpText_text = sp_TmpText.FindPropertyRelative("Base");

                        SerializedProperty sp_Image = sp_Root.FindPropertyRelative("Image");
                        SerializedProperty sp_Image_img = sp_Image.FindPropertyRelative("Base");

                        SerializedProperty sp_RawImage = sp_Root.FindPropertyRelative("RawImage");
                        SerializedProperty sp_RawImage_img = sp_RawImage.FindPropertyRelative("Base");
                        #endregion

                        sp_id.intValue = Dv_CreateID(BaseScript.ContainerItems);
                        sp_Indicator.stringValue = trs.name;

                        #region 清空物体
                        sp_Text_text.objectReferenceValue = null;
                        sp_TmpText_text.objectReferenceValue = null;
                        sp_Image_img.objectReferenceValue = null;
                        sp_RawImage_img.objectReferenceValue = null;
                        sp_animator.objectReferenceValue = null;
                        sp_Transform.objectReferenceValue = null;
                        #endregion

                        #region 获取组件
                        xHud_Module_Text t_text = trs.GetComponent<xHud_Module_Text>();
                        xHud_Module_TmpText t_tmptext = trs.GetComponent<xHud_Module_TmpText>();
                        Image t_image = trs.GetComponent<Image>();
                        RawImage t_rawimage = trs.GetComponent<RawImage>();
                        xHud_Module_Animator animator = trs.GetComponent<xHud_Module_Animator>();
                        sp_Transform.objectReferenceValue = trs.GetComponent<RectTransform>();
                        #endregion

                        #region 类型判断

                        //如果存在文字组件
                        if (t_text != null)
                        {
                            sp_Type.enumValueIndex = (int)ContainerType.Text;
                            sp_Text_text.objectReferenceValue = t_text;
                        }
                        //如果存在Tmp文字组件
                        else if (t_tmptext != null)
                        {
                            sp_Type.enumValueIndex = (int)ContainerType.TmpText;
                            sp_TmpText_text.objectReferenceValue = t_tmptext;
                        }
                        //如果存在图像组件
                        else if (t_image != null)
                        {
                            sp_Type.enumValueIndex = (int)ContainerType.Image;
                            sp_Image_img.objectReferenceValue = t_image;
                        }
                        //如果存在Raw图像组件
                        else if (t_rawimage != null)
                        {
                            sp_Type.enumValueIndex = (int)ContainerType.RawImage;
                            sp_RawImage_img.objectReferenceValue = t_rawimage;
                        }
                        //如果不存在文字组件或者Tmp文字组件以及图像组件
                        else
                        {
                            sp_Type.enumValueIndex = (int)ContainerType.None;
                        }


                        if (animator != null)
                            sp_animator.objectReferenceValue = animator;
                        #endregion

                        #region 保存属性
                        sp_Transform.serializedObject.ApplyModifiedProperties();
                        sp_animator.serializedObject.ApplyModifiedProperties();
                        sp_Text_text.serializedObject.ApplyModifiedProperties();
                        sp_TmpText_text.serializedObject.ApplyModifiedProperties();
                        sp_Image_img.serializedObject.ApplyModifiedProperties();
                        sp_RawImage_img.serializedObject.ApplyModifiedProperties();
                        sp_Type.serializedObject.ApplyModifiedProperties();
                        sp_id.serializedObject.ApplyModifiedProperties();
                        sp_Indicator.serializedObject.ApplyModifiedProperties();
                        #endregion

                        #region 添加到弹窗列表
                        xHudGUI_Dialog_ListDatas dataitem = new xHudGUI_Dialog_ListDatas();

                        dataitem.Title = $"{BaseScript.name} ( {BaseScript.Indicator} )";
                        dataitem.SubTitle = "添加新组件";
                        dataitem.Message = $"{trs.name} ( {sp_Indicator.stringValue} )";

                        Datas.Add(dataitem);
                        #endregion
                    }
                    else
                    {
                        #region 获取属性
                        SerializedProperty sp_Root = ContainerItems.GetArrayElementAtIndex(statuid);
                        SerializedProperty sp_Indicator = sp_Root.FindPropertyRelative("Indicator");
                        SerializedProperty sp_Transform = sp_Root.FindPropertyRelative("Transform");
                        SerializedProperty sp_animator = sp_Root.FindPropertyRelative("Animator");
                        #endregion

                        #region 重新尝试获取Animator
                        if (sp_animator.objectReferenceValue == null)
                        {
                            RectTransform anim_obj = (RectTransform)sp_Transform.objectReferenceValue;
                            xHud_Module_Animator anim = anim_obj.GetComponent<xHud_Module_Animator>();
                            if (anim != null)
                            {
                                sp_animator.objectReferenceValue = anim;
                                sp_animator.serializedObject.ApplyModifiedProperties();
                            }
                        }
                        #endregion

                        #region 添加到弹窗列表
                        xHudGUI_Dialog_ListDatas dataitem = new xHudGUI_Dialog_ListDatas();

                        dataitem.Title = $"{BaseScript.name} ( {BaseScript.Indicator} )";
                        dataitem.SubTitle = "更新已有项";
                        dataitem.Message = $"{trs.name} ( {sp_Indicator.stringValue} )";
                        Datas.Add(dataitem);
                        #endregion
                    }
                }
                if (ContainerItems.arraySize > 0)
                    Editor_xHudGUI.Open(Datas.ToArray(), xHudDialogType.确认, "HudContainer容器消息", "扫描容器元素", "以下是扫描到的所有元素列表，请您检查核对：", "明白");
                else
                    Editor_xHudGUI.Open(xHudDialogType.警告, "HudContainer容器消息", "扫描元素组件", "未扫描到任何有效容器元素组件！", "明白");
            }
        }

        /// <summary>
        /// 重新生成ID
        /// </summary>
        private void RegenerateID()
        {
            if (IsMultiSelected())
            {
                for (int s = 0; s < SelectedObjects.Length; s++)
                {
                    SerializedObject so = new SerializedObject(SelectedObjects[s]);
                    so.Update();

                    SerializedProperty sp_Cons = so.FindProperty("ContainerItems");

                    for (int i = 0; i < sp_Cons.arraySize; i++)
                    {
                        SerializedProperty sp_Root = sp_Cons.GetArrayElementAtIndex(i);

                        SerializedProperty sp_id = sp_Root.FindPropertyRelative("ID");

                        sp_id.intValue = Dv_CreateID(SelectedObjects[s].ContainerItems);
                        sp_id.serializedObject.ApplyModifiedProperties();

                        sp_Root.serializedObject.ApplyModifiedProperties();
                    }
                    sp_Cons.serializedObject.ApplyModifiedProperties();
                    so.ApplyModifiedProperties();
                }
            }
            else
            {
                for (int i = 0; i < ContainerItems.arraySize; i++)
                {
                    SerializedProperty sp_Root = ContainerItems.GetArrayElementAtIndex(i);

                    SerializedProperty sp_id = sp_Root.FindPropertyRelative("ID");

                    sp_id.intValue = Dv_CreateID(BaseScript.ContainerItems);
                    sp_id.serializedObject.ApplyModifiedProperties();
                }
            }
        }

        /// <summary>
        /// 折叠
        /// </summary>
        /// <param name="state"></param>
        private void AllListFoldState(bool state)
        {
            if (target == null)
                return;
            EventIsFold.boolValue = state;
            EventIsFold.serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 数据可视化 - 创建ID编号
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public int Dv_CreateID(List<HudContainerItem> nodes)
        {
            List<int> ids = new List<int>();
            for (int i = 0; i < nodes.Count; i++)
            {
                ids.Add(nodes[i].ID);
            }

            int ran_id = Random.Range(1111, 9999);

            while (true)
            {
                if (ids.Contains(ran_id))
                {
                    ran_id = Random.Range(1111, 9999);
                }
                else
                {
                    return ran_id;
                }
            }
        }

        /// <summary>
        /// 还原Animator姿态
        /// </summary>
        private void OriginalPoseState_Load(xHud_Module_Animator animator)
        {
            if (animator == null)
                return;

            SerializedObject so = new SerializedObject(animator);
            so.Update();
            SerializedProperty origin = so.FindProperty("OriginalAnimatorPoser");
            SerializedProperty m_rect = so.FindProperty("mod_RectTransform");
            SerializedProperty m_img = so.FindProperty("mod_Image");
            SerializedProperty m_text = so.FindProperty("mod_Text");
            SerializedProperty m_tmptext = so.FindProperty("mod_TmpText");
            SerializedProperty m_cav = so.FindProperty("CanvasGroup");

            SerializedProperty pos = origin.FindPropertyRelative("Position");
            SerializedProperty eur = origin.FindPropertyRelative("Euler");
            SerializedProperty sca = origin.FindPropertyRelative("Scale");
            SerializedProperty size = origin.FindPropertyRelative("Size");
            SerializedProperty alp = origin.FindPropertyRelative("Alpha");
            SerializedProperty col = origin.FindPropertyRelative("Color");
            SerializedProperty fil = origin.FindPropertyRelative("Fill");

            RectTransform rect = (RectTransform)m_rect.objectReferenceValue;

            rect.anchoredPosition3D = pos.vector3Value;
            rect.localEulerAngles = eur.vector3Value;
            rect.localScale = sca.vector3Value;
            rect.sizeDelta = size.vector2Value;
            m_rect.serializedObject.ApplyModifiedProperties();

            ((CanvasGroup)m_cav.objectReferenceValue).alpha = alp.floatValue;
            m_cav.serializedObject.ApplyModifiedProperties();

            if (m_img.objectReferenceValue != null)
            {
                Image img = (Image)m_img.objectReferenceValue;
                img.color = col.colorValue;
                img.fillAmount = fil.floatValue;
                m_img.serializedObject.ApplyModifiedProperties();
            }
            if (m_text.objectReferenceValue != null)
            {
                ((xHud_Module_Text)m_text.objectReferenceValue).color = col.colorValue;
                m_text.serializedObject.ApplyModifiedProperties();
            }
            if (m_tmptext.objectReferenceValue != null)
            {
                ((xHud_Module_TmpText)m_tmptext.objectReferenceValue).color = col.colorValue;
                m_tmptext.serializedObject.ApplyModifiedProperties();
            }

            so.ApplyModifiedProperties();
        }

        private void CalculateAnimatorMaxDuration(List<HudContainerItem> list, float globaldur)
        {
            Animators_MaxDuration.floatValue = Animators_GetAnimatorsMaxDuration(list, globaldur);
            Animators_MaxDuration.serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 从所有子动画器中获取最大耗时
        /// </summary>
        /// <returns></returns>
        public float Animators_GetAnimatorsMaxDuration(List<HudContainerItem> list, float globaldur)
        {
            if (list.Count <= 0)
                return 0;

            float[] x_list = new float[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Animator == null)
                {
                    x_list[i] = 0;
                    continue;
                }

                xHud_Module_Animator anim = list[i].Animator;
                if (anim != null)
                {
                    anim.TweenNodeTimersGet();
                    x_list[i] = anim.MaxTimerWithGlobalDuration + list[i].DelayTime;
                }
                else
                {
                    x_list[i] = 1;
                }
            }
            float v = xHud_Utilitys.Array_MaxValue(x_list);
            return v * globaldur;
        }

        /// <summary>
        /// 获取动画器中是否存在循环模式
        /// </summary>
        /// <returns></returns>
        public bool AnimatorTweenNodes_HasLoopMode(List<TweenNode> tweenlist)
        {
            bool hasLoop = false;

            for (int s = 0; s < tweenlist.Count; s++)
            {
                if (tweenlist[s].LoopCount == -1)
                {
                    hasLoop = true;
                    break;
                }
            }
            return hasLoop;
        }

        /// <summary>
        /// 获取动画器中是否存在循环模式
        /// </summary>
        /// <returns></returns>
        public bool Animators_HasLoopMode()
        {
            bool hasLoop = false;

            List<int> loops = new List<int>();
            for (int i = 0; i < BaseScript.ContainerItems.Count; i++)
            {
                xHud_Module_Animator anim = BaseScript.ContainerItems[i].Animator;
                if (anim == null)
                {
                    continue;
                }
                if (anim == null && anim.AnimateTweenNodes == null || anim.AnimateTweenNodes.Count <= 0)
                {
                    continue;
                }
                else
                {
                    for (int s = 0; s < anim.AnimateTweenNodes.Count; s++)
                    {
                        loops.Add(anim.AnimateTweenNodes[s].LoopCount);
                    }
                }
            }

            for (int i = 0; i < loops.Count; i++)
            {
                if (loops[i] == -1)
                {
                    hasLoop = true;
                    break;
                }
            }
            return hasLoop;
        }

        private void GetContainTypes()
        {
            ImageCount = 0;
            RawImageCount = 0;
            TextCount = 0;
            TmpTextCount = 0;
            AnimatorCount = 0;

            for (int i = 0; i < BaseScript.ContainerItems.Count; i++)
            {
                HudContainerItem item = BaseScript.ContainerItems[i];

                switch (item.Type)
                {
                    case ContainerType.None:
                        break;
                    case ContainerType.Text:
                        TextCount++;
                        break;
                    case ContainerType.TmpText:
                        TmpTextCount++;
                        break;
                    case ContainerType.Image:
                        ImageCount++;
                        break;
                    case ContainerType.RawImage:
                        RawImageCount++;
                        break;
                }

                if (item.Animator != null)
                {
                    AnimatorCount++;
                }
            }
        }
        #endregion

        #region 预览

        #region Animator 动画预览
        /// <summary>
        /// 预览动画
        /// </summary>
        private void Preview_Animator_Play()
        {
            xHud_Manager mgr = xHud_Dashboard.HudManagerGet();

            AnimateState.enumValueIndex = 1;
            AnimateState.serializedObject.ApplyModifiedProperties();

            ///---用来存放调用动画器的动画效果的延迟时间
            List<float> delaytimes = new List<float>();

            ///---动画逻辑
            for (int i = 0; i < BaseScript.ContainerItems.Count; i++)
            {
                HudContainerItem node = BaseScript.ContainerItems[i];

                xHud_Module_Animator anim = node.Animator;

                if (anim != null)
                {
                    ///---判断动画器是否为有效
                    if (anim.AnimateTweenNodes == null || anim.AnimateTweenNodes.Count <= 0)
                        continue;
                    ///---循环第 c 个动画器节点的第 i 个动画效果是否开启
                    for (int s = 0; s < anim.AnimateTweenNodes.Count; s++)
                    {
                        TweenNode twnnode = anim.AnimateTweenNodes[s];
                        ///---如果动画效果未开启则跳过
                        if (!twnnode.Enabled)
                        {
                            continue;
                        }
                        else
                        {
                            if (twnnode.ActivateOnlyToEnd)
                                continue;

                            if (mgr == null)
                                Preivew_Animator_TweenList.Add(anim.Tweener_Play(twnnode, 1 * Animators_GlobalDuration.floatValue * anim.Animator_GlobalDuration));
                            else
                                Preivew_Animator_TweenList.Add(anim.Tweener_Play(twnnode, mgr.DurationMultiply * Animators_GlobalDuration.floatValue * anim.Animator_GlobalDuration));
                            delaytimes.Add(node.DelayTime);
                        }
                    }
                }
            }

            ///---预备动画器
            for (int i = 0; i < BaseScript.ContainerItems.Count; i++)
            {
                HudContainerItem node = BaseScript.ContainerItems[i];
                if (node.Animator != null)
                {
                    ///---判断动画器是否为有效
                    if (node.Animator.AnimateTweenNodes == null || node.Animator.AnimateTweenNodes.Count <= 0)
                        continue;
                    node.Animator.RewindAllTweenNode();
                }
            }

            //同步启动所有Animator的所有音效播放
            Preivew_AnimatorSound_Coroutine_Play = EditorCoroutineUtility.StartCoroutineOwnerless(Preview_AnimatorSound_Play());

            ///---播放预览动画
            if (Preivew_Animator_TweenList != null && Preivew_Animator_TweenList.Count > 0)
            {
                for (int i = 0; i < Preivew_Animator_TweenList.Count; i++)
                {
                    Preivew_Animator_CoroutineList_Play.Add(EditorCoroutineUtility.StartCoroutineOwnerless(Preview_Animator_Coroutine_Play(Preivew_Animator_TweenList[i], delaytimes[i])));
                }
                if (!Animators_HasLoopMode())
                {
                    if (AutoStopPreview.boolValue)
                        Preivew_Animator_Coroutine_Stop = EditorCoroutineUtility.StartCoroutine(Preview_Animator_Coroutine_Stop(Animators_MaxDuration.floatValue), this);
                }
                DOTweenEditorPreview.Start();
            }
        }

        /// <summary>
        /// 延迟预览指定动画
        /// </summary>
        IEnumerator Preview_Animator_Coroutine_Play(Tweener tween, float delay)
        {
            yield return new EditorWaitForSeconds(delay);
            DOTweenEditorPreview.PrepareTweenForPreview(tween, true, true, true);
        }

        /// <summary>
        /// 预览指定Animator的动画
        /// </summary>
        private void Preview_Animator_PlayAt(int index)
        {
            xHud_Manager mgr = xHud_Dashboard.HudManagerGet();

            Preview_Animator_Stop();

            HudContainerItem node = BaseScript.ContainerItems[index];
            xHud_Module_Animator anim = node.Animator;
            if (anim == null)
                return;

            if (anim.TweenNode_GetCount() <= 0)
                return;

            AnimateState.enumValueIndex = 1;
            AnimateState.serializedObject.ApplyModifiedProperties();

            ///---用来存放调用动画器的动画效果的延迟时间
            float delaytime = 0;

            if (anim.AnimateTweenNodes == null || anim.AnimateTweenNodes.Count <= 0)
                return;

            for (int i = 0; i < anim.AnimateTweenNodes.Count; i++)
            {
                if (anim.AnimateTweenNodes[i].Enabled)
                {
                    TweenNode twnnode = anim.AnimateTweenNodes[i];
                    if (twnnode.ActivateOnlyToEnd)
                        continue;

                    Preivew_Animator_TweenList.Add(anim.Tweener_Play(twnnode, mgr.DurationMultiply * anim.Animator_GlobalDuration));
                    delaytime = node.DelayTime;

                    //播放动画节点包含的音效
                    for (int k = 0; k < twnnode.TweenSounds.Count; k++)
                    {
                        TweenSound sod = twnnode.TweenSounds[k];
                        #region 播放音效
                        AudioClip x_clip = sod.Sound;
                        //此处考虑到延迟计算 / 音效占动画耗时的百分比
                        float delay = sod.Percentage * ((twnnode.Duration * anim.Animator_GlobalDuration)) + twnnode.Delay + node.DelayTime;
                        Preivew_AnimatorSound_CoroutineList_Stop.Add(EditorCoroutineUtility.StartCoroutineOwnerless(Preview_AnimatorSound_Play_At(x_clip, sod.Volume, sod.MinPitch, sod.MaxPitch, anim.MutePlay, delay)));
                        #endregion
                    }
                }
                else
                {
                    continue;
                }
            }

            if (Preivew_Animator_TweenList != null && Preivew_Animator_TweenList.Count > 0)
            {
                for (int i = 0; i < Preivew_Animator_TweenList.Count; i++)
                {
                    Preivew_Animator_CoroutineList_Play.Add(EditorCoroutineUtility.StartCoroutineOwnerless(Preview_Animator_Coroutine_Play(Preivew_Animator_TweenList[i], delaytime)));
                }
                if (!AnimatorTweenNodes_HasLoopMode(anim.AnimateTweenNodes))
                {
                    if (AutoStopPreview.boolValue)
                        Preivew_Animator_Coroutine_Stop = EditorCoroutineUtility.StartCoroutine(Preview_Animator_Coroutine_Stop(anim.MaxTimerWithGlobalDuration + delaytime), this);
                }
                DOTweenEditorPreview.Start();
            }
        }

        /// <summary>
        /// 停止预览动画
        /// </summary>
        private void Preview_Animator_Stop()
        {
            if (target != null)
            {
                AnimateState.enumValueIndex = 0;
                AnimateState.serializedObject.ApplyModifiedProperties();

                #region 杀死所有预览的节点的动画
                if (Preivew_Animator_TweenList != null && Preivew_Animator_TweenList.Count > 0)
                {
                    for (int i = 0; i < Preivew_Animator_TweenList.Count; i++)
                    {
                        if (Preivew_Animator_TweenList[i] != null)
                        {
                            Preivew_Animator_TweenList[i].Complete();
                            Preivew_Animator_TweenList[i].Kill();
                            Preivew_Animator_TweenList[i].Rewind();
                        }
                    }
                }
                #endregion

                #region 复位所有节点的动画
                if (BaseScript.ContainerItems != null && BaseScript.ContainerItems.Count > 0)
                {
                    for (int c = 0; c < BaseScript.ContainerItems.Count; c++)
                    {
                        xHud_Module_Animator animator = BaseScript.ContainerItems[c].Animator;
                        if (animator != null)
                        {
                            List<TweenNode> TwnNodes = animator.AnimateTweenNodes;

                            if (TwnNodes != null && TwnNodes.Count > 0)
                            {
                                for (int i = 0; i < TwnNodes.Count; i++)
                                {
                                    TweenNode twnnode = TwnNodes[i];
                                    if (!twnnode.Enabled)
                                        continue;
                                    else
                                    {
                                        if (twnnode.ActivateOnlyToEnd)
                                            continue;

                                        animator.Tweener_Rewind(twnnode);
                                    }
                                }
                            }

                            ///---还原姿态                        
                            OriginalPoseState_Load(animator);
                        }
                    }
                }
                #endregion

                #region 清空 Animator 动画预览列表
                Preivew_Animator_TweenList.Clear();
                for (int i = 0; i < Preivew_Animator_CoroutineList_Play.Count; i++)
                {
                    EditorCoroutineUtility.StopCoroutine(Preivew_Animator_CoroutineList_Play[i]);
                }
                Preivew_Animator_CoroutineList_Play.Clear();
                #endregion

                #region 停止协程 - Animator 动画停止预览
                if (Preivew_Animator_Coroutine_Stop != null)
                {
                    EditorCoroutineUtility.StopCoroutine(Preivew_Animator_Coroutine_Stop);
                    Preivew_Animator_Coroutine_Stop = null;
                }
                #endregion

                #region 停止协程 - AnimatorSound 音效预览播放
                if (Preivew_AnimatorSound_Coroutine_Play != null)
                {
                    EditorCoroutineUtility.StopCoroutine(Preivew_AnimatorSound_Coroutine_Play);
                    Preivew_AnimatorSound_Coroutine_Play = null;
                }
                #endregion

                #region 停止播放并清空 AnimatorSound 预览列表与生成的音效物体
                if (Preivew_AnimatorSound_SoundList != null)
                {
                    for (int i = 0; i < Preivew_AnimatorSound_SoundList.Count; i++)
                    {
                        if (Preivew_AnimatorSound_SoundList[i] != null)
                        {
                            //因为考虑到音效长度如果大于动画长度，那么得由生成的音效自己决定音效播放完后销毁的动作，否会出现动画放完而音效未放完被强行终止而销毁的BUG，所以这里只要清空音效预览列表即可，但如果你需要在动画放完时音效也都跟着一起销毁就取消注释下面得代码块
                            //Preivew_AnimatorSound_SoundList[i].Stop();
                            //DestroyImmediate(Preivew_AnimatorSound_SoundList[i].gameObject, true);
                            Preivew_AnimatorSound_SoundList[i] = null;
                        }
                    }
                    Preivew_AnimatorSound_SoundList.Clear();
                }
                #endregion
            }
            DOTweenEditorPreview.Stop();
        }

        /// <summary>
        /// 延迟停止
        /// </summary>
        IEnumerator Preview_Animator_Coroutine_Stop(float stopdelay)
        {
            xHud_Manager mgr = xHud_Dashboard.HudManagerGet();

            yield return new EditorWaitForSeconds(stopdelay * mgr.DurationMultiply);
            if (!Application.isPlaying)
            {
                Preview_Animator_Stop();
                Repaint();
            }
        }
        #endregion

        #region Animator 音效预览
        /// <summary>
        /// Animator 音效预览
        /// </summary>
        /// <returns></returns>
        IEnumerator Preview_AnimatorSound_Play()
        {
            while (BaseScript.AnimateState == HudElementAnimateState.Animating)
            {
                for (int w = 0; w < BaseScript.ContainerItems.Count; w++)
                {
                    HudContainerItem a_node = BaseScript.ContainerItems[w];
                    if (a_node.Animator == null)
                        continue;
                    for (int i = 0; i < a_node.Animator.AnimateTweenNodes.Count; i++)
                    {
                        TweenNode tweenNode = a_node.Animator.AnimateTweenNodes[i];
                        Tweener tweener = tweenNode.Tweener;

                        if (tweener != null && tweener.IsActive() && tweener.IsPlaying())
                        {
                            if (tweenNode.Progress > 0.985f)
                                tweenNode.Progress = 1;
                            else
                                tweenNode.Progress = tweener.Elapsed(true) / tweener.Duration(true);
                        }

                        for (int s = 0; s < tweenNode.TweenSounds.Count; s++)
                        {
                            ///---如果动画是循环模式则不会播放音效，以为初始化时动画的Progress为0，此时程序会判定已到达播放音效的触点位置，则会误判发出音效
                            if (tweenNode.LoopCount == -1)
                                continue;

                            TweenSound tweenSound = tweenNode.TweenSounds[s];

                            if (tweenSound.Sound == null)
                                continue;

                            if (tweenSound.Percentage >= 1)
                            {
                                if (tweenNode.Progress >= tweenSound.Percentage)
                                {
                                    if (!tweenSound.IsPlayed)
                                    {
                                        tweenSound.IsPlayed = true;

                                        AudioClip clip = tweenSound.Sound;
                                        float vol = tweenSound.Volume;
                                        float pitch_min = tweenSound.MinPitch;
                                        float pitch_max = tweenSound.MaxPitch;
                                        Preivew_AnimatorSound_SoundList.Add(Preview_AnimatorSound_Creator(clip, vol, pitch_min, pitch_max, a_node.Animator.MutePlay));
                                    }
                                }
                                else
                                {
                                    tweenSound.IsPlayed = false;
                                }
                            }
                            else if (tweenSound.Percentage < 1)
                            {
                                if (tweenNode.Progress > tweenSound.Percentage)
                                {
                                    if (!tweenSound.IsPlayed)
                                    {
                                        tweenSound.IsPlayed = true;

                                        AudioClip clip = tweenSound.Sound;
                                        float vol = tweenSound.Volume;
                                        float pitch_min = tweenSound.MinPitch;
                                        float pitch_max = tweenSound.MaxPitch;
                                        Preivew_AnimatorSound_SoundList.Add(Preview_AnimatorSound_Creator(clip, vol, pitch_min, pitch_max, a_node.Animator.MutePlay));
                                    }
                                }
                                else
                                {
                                    tweenSound.IsPlayed = false;
                                }
                            }
                        }
                    }
                }
                Repaint();
                yield return null;
            }
        }

        /// <summary>
        /// 创建 Animator 预览声音
        /// </summary>
        /// <param name="clip"></param>
        public AudioSource Preview_AnimatorSound_Creator(AudioClip clip, float vol, float pitch_min, float pitch_max, bool ismute)
        {
            xHud_Manager mgr = xHud_Dashboard.HudManagerGet();

            if (ismute)
                return null;
            GameObject obj = new GameObject();
            obj.name = "AnimatorSound_Previewer-" + "[" + clip.length.ToString("F2") + " s]-" + "[" + clip.channels + " ch]-" + "[" + clip.frequency + " hz]";
            AudioSource au = obj.AddComponent<AudioSource>();
            au.volume = mgr.Volume * 0.01f * vol;
            au.mute = mgr.VolumeMute;
            au.pitch = Random.Range(pitch_min, pitch_max);
            au.clip = clip;
            au.Play();
            xHud_AudioStoper sp = au.gameObject.AddComponent<xHud_AudioStoper>();
            sp.SetAudioSource(au);
            return au;
        }

        /// <summary>
        /// 创建 Animator 预览指定声音协程
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="sp_vol"></param>
        /// <param name="sp_pitch_min"></param>
        /// <param name="sp_pitch_max"></param>
        /// <param name="sp_ismute"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        IEnumerator Preview_AnimatorSound_Play_At(AudioClip clip, float sp_vol, float sp_pitch_min, float sp_pitch_max, bool sp_ismute, float delay)
        {
            yield return new EditorWaitForSeconds(delay);
            Preivew_AnimatorSound_SoundList.Add(Preview_AnimatorSound_Creator(clip, sp_vol, sp_pitch_min, sp_pitch_max, sp_ismute));
            AudioSource au = Preivew_AnimatorSound_SoundList[Preivew_AnimatorSound_SoundList.Count - 1];
            while (true)
            {
                if (au != null && !au.isPlaying)
                {
                    break;
                }
                yield return null;
            }
            DestroyImmediate(au.gameObject, true);
        }

        /// <summary>
        ///  停止协程列表 - AnimatorSound 音效预览播放 / 停止播放并清空 AnimatorSound 预览列表与生成的音效物体
        /// </summary>
        private void Preview_AnimatorSound_CoroutineList_Stop()
        {
            for (int i = 0; i < Preivew_AnimatorSound_CoroutineList_Stop.Count; i++)
            {
                if (Preivew_AnimatorSound_CoroutineList_Stop[i] != null)
                    EditorCoroutineUtility.StopCoroutine(Preivew_AnimatorSound_CoroutineList_Stop[i]);
            }
            Preivew_AnimatorSound_CoroutineList_Stop.Clear();

            if (Preivew_AnimatorSound_SoundList != null)
            {
                for (int i = 0; i < Preivew_AnimatorSound_SoundList.Count; i++)
                {
                    if (Preivew_AnimatorSound_SoundList[i] != null)
                    {
                        Preivew_AnimatorSound_SoundList[i].Stop();
                        DestroyImmediate(Preivew_AnimatorSound_SoundList[i].gameObject, true);
                        Preivew_AnimatorSound_SoundList[i] = null;
                    }
                }
                Preivew_AnimatorSound_SoundList.Clear();
            }

            SceneView.RepaintAll();
        }
        #endregion

        #endregion
    }
}