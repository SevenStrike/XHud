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
    using SevenStrikeModules.XTween.Editor;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.InputSystem.LowLevel;
    using UnityEngine.UI;
    using Random = UnityEngine.Random;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(XHud_Module_Primitive_Tween))]
    public class Editor_XHud_Module_Primitive_Tween : Editor
    {
        #region 组件 / 列表
        private XHud_Module_Primitive_Tween BaseScript;
        private ReorderableList AnimateTweenNodesList;
        private XHud_Manager HudManager;
        #endregion

        #region 序列化属性
        private SerializedProperty sp_Debug, sp_PrimitiveTweenNodes, sp_TweenIsPreviewing, sp_GlobalDuration, sp_MutePlay, sp_MaxTimer, sp_MinTimer, sp_MinTimerWithGlobalDuration, sp_MaxTimerWithGlobalDuration, sp_IgnoreElementAnimationPlay, sp_PreviewTiming;

        #endregion

        Rect draw_rect;

        #region GUI 参数
        /// <summary>
        /// 系统默认GUI行高
        /// </summary>
        private float LineHeight;
        /// <summary>
        /// 原始脚本参数显示开关
        /// </summary>
        private bool OriginalDisplay;
        /// <summary>
        /// 多选特性的索引
        /// </summary>
        private int MultiPrimitiveFeature_Index;
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

        #region 选项文字
        string[] opt_debug = new string[2] { "关闭", "调试" }, opt_mute = new string[] { "正常", "静音" }, opt_control = new string[] { "可控", "忽略" };
        #endregion

        #region 图标
        private Texture2D prw_play_r, prw_play_p, prw_stop_r, prw_stop_p, Add_r, Add_p, locate_r, locate_p, left_arrow_r, left_arrow_p, right_arrow_r, right_arrow_p, statu, count, timer_min, timer_max, pos, rot, sca, size, alp, fill, color, anim_dir_bak, anim_dir_for, anim_dir_for_long, anim_dir_bak_p, anim_dir_for_p, anim_dir_for_long_p, anim_dot, anim_dot_dark, anim_type_mover, anim_type_rotator, anim_type_scale, anim_type_color, anim_type_fade, anim_type_writter, anim_type_fill, anim_type_size, icon_unfold_r, icon_unfold_p, anim_fold_r, anim_fold_p, anim_change_id_r, anim_change_id_p, anim_sound_r, anim_sound_p, anim_dir_war_r, anim_dir_war_p, icon_text, icon_tmptext, icon_image, icon_rawimage, icon_trans, icon_main, opentrack_r, opentrack_p;
        #endregion

        #region 必要组件
        private XHud_Module_Text HudText;
        private XHud_Module_TmpText HudTmpText;
        private XHud_Module_Button HudButton;
        private XHud_Module_Progress HudProgress;
        private XHud_Module_Toggle HudToggle;
        private XHud_Module_Slider HudSlider;
        private XHud_Module_Option HudOption;
        #endregion

        #region 音效预览
        private List<AudioSource> Preview_PrimitiveTweens_SoundList = new List<AudioSource>();
        private List<XCoroutine> Preview_PrimitiveTweens_SoundCoroutineList_Stop = new List<XCoroutine>();
        #endregion

        /// <summary>
        /// 音效设置器
        /// </summary>
        private Editor_XHud_PrimitiveTweenSoundSetTool Editor_XHud_PrimitiveTweenSoundSetTool;

        /// <summary>
        /// 预览时机
        /// </summary>
        string[] PreviewTimings;

        #region 批量模式查看索引
        private int TweenStatu_Index;
        #endregion

        #region 批量化操作
        /// <summary>
        /// 批量选择脚本数组
        /// </summary>
        XHud_Module_Primitive_Tween[] SelectedObjects;
        /// <summary>
        /// 获取所有批量脚本目标
        /// </summary>
        private void Targets_Get()
        {
            if (targets.Length > 1)
            {
                SelectedObjects = new XHud_Module_Primitive_Tween[targets.Length];
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    var t = targets[i];
                    SelectedObjects[i] = (XHud_Module_Primitive_Tween)t;
                }
            }
            else
            {
                SelectedObjects = new XHud_Module_Primitive_Tween[targets.Length];
                SelectedObjects[0] = (XHud_Module_Primitive_Tween)target;
            }
        }
        /// <summary>
        /// 判断是否是多选状态
        /// </summary>
        /// <returns></returns>
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

        private PrimitiveFeatures[] PrimitiveFeatures;

        private void OnEnable()
        {
            HudManager = XHud_Dashboard.HudManagerGet();

            #region 获取系统GUI单行单位高度
            LineHeight = EditorGUIUtility.singleLineHeight;
            #endregion

            BaseScript = (XHud_Module_Primitive_Tween)target;

            Targets_Get();

            // 获取所有序列化字段
            GetSerializeFields();

            #region 获取字体
            Font_Bold = Editor_XHud_GUI.GetFont("sx_bold");
            Font_Light = Editor_XHud_GUI.GetFont("sx_regular");
            #endregion

            #region 获取图标          
            icon_main = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/icon_main");
            left_arrow_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/left_arrow_r");
            left_arrow_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/left_arrow_p");
            right_arrow_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/right_arrow_r");
            right_arrow_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/right_arrow_p");
            prw_play_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/prw_play_r");
            prw_play_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/prw_play_p");
            prw_stop_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/prw_stop_r");
            prw_stop_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/prw_stop_p");
            Add_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/Add_r");
            Add_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/Add_p");
            locate_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/locate_r");
            locate_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/locate_p");
            statu = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/statu");
            count = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/count");
            timer_min = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/timer_min");
            timer_max = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/timer_max");
            pos = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/pos");
            rot = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/rot");
            sca = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/sca");
            size = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/size");
            alp = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/alp");
            fill = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/fill");
            color = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/color");
            anim_dir_bak = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/anim_dir_bak");
            anim_dir_bak_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/anim_dir_bak_p");
            anim_dir_for = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/anim_dir_for");
            anim_dir_for_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/anim_dir_for_p");
            anim_dir_for_long = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/anim_dir_for_long");
            anim_dir_for_long_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/anim_dir_for_long_p");
            anim_dir_war_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/anim_dir_war_r");
            anim_dir_war_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/anim_dir_war_p");
            anim_dot = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/anim_dot");
            anim_dot_dark = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/anim_dot_dark");
            anim_type_color = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/anim_type_color");
            anim_type_fade = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/anim_type_fade");
            anim_type_fill = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/anim_type_fill");
            anim_type_mover = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/anim_type_move");
            anim_type_rotator = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/anim_type_rotator");
            anim_type_scale = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/anim_type_scale");
            anim_type_size = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/anim_type_size");
            anim_type_writter = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/anim_type_writter");
            anim_fold_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/anim_fold_r");
            anim_fold_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/anim_fold_p");
            icon_unfold_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/icon_unfold_r");
            icon_unfold_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/icon_unfold_p");
            anim_sound_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/anim_sound_r");
            anim_sound_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/anim_sound_p");
            anim_change_id_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/anim_change_id_r");
            anim_change_id_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/anim_change_id_p");
            icon_text = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/icon_text");
            icon_tmptext = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/icon_tmptext");
            icon_image = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/icon_image");
            icon_rawimage = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/icon_rawimage");
            icon_trans = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/icon_trans");
            opentrack_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/opentrack_r");
            opentrack_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_primitive_tween/opentrack_p");

            #endregion

            #region 获取父物体组件以判断类型
            HudText = BaseScript.GetComponentInParent<XHud_Module_Text>();
            HudTmpText = BaseScript.GetComponentInParent<XHud_Module_TmpText>();
            HudButton = BaseScript.GetComponentInParent<XHud_Module_Button>();
            HudProgress = BaseScript.GetComponentInParent<XHud_Module_Progress>();
            HudToggle = BaseScript.GetComponentInParent<XHud_Module_Toggle>();
            HudSlider = BaseScript.GetComponentInParent<XHud_Module_Slider>();
            HudOption = BaseScript.GetComponentInParent<XHud_Module_Option>();
            #endregion

            #region 动画列表
            AnimateTweenNodesList = new ReorderableList(serializedObject, sp_PrimitiveTweenNodes)
            {
                displayAdd = true,
                displayRemove = true,
                draggable = true,
                drawHeaderCallback = rect =>
                {
                    // 绘制一个标签作为标题
                    EditorGUI.LabelField(rect, "动画堆栈");
                },
                drawElementBackgroundCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    if (index >= 0)
                    {
                        if (isFocused)
                            EditorGUI.DrawRect(new Rect(rect.x + (rect.width / 2) + 36, rect.y + 25, 5, 5), XHud_Dashboard.Theme_Primary);
                    }
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    SerializedProperty sp_node = sp_PrimitiveTweenNodes.GetArrayElementAtIndex(index);

                    #region 获取动画节点的序列化字段
                    SerializedProperty sp_ID = sp_node.FindPropertyRelative("ID");
                    SerializedProperty sp_Duration = sp_node.FindPropertyRelative("Duration");
                    SerializedProperty sp_TweenSounds = sp_node.FindPropertyRelative("TweenSounds");
                    SerializedProperty sp_Delay = sp_node.FindPropertyRelative("Delay");
                    SerializedProperty sp_Ease = sp_node.FindPropertyRelative("Ease");
                    SerializedProperty sp_Rewind_Set_Startvalue = sp_node.FindPropertyRelative("Rewind_Set_Startvalue");
                    SerializedProperty sp_Complete_Set_Endvalue = sp_node.FindPropertyRelative("Complete_Set_Endvalue");
                    SerializedProperty sp_AnimationCurveName = sp_node.FindPropertyRelative("AnimationCurveName");
                    SerializedProperty sp_Curve = sp_node.FindPropertyRelative("Curve");
                    SerializedProperty sp_IsFold = sp_node.FindPropertyRelative("IsFold");
                    SerializedProperty sp_Timings = sp_node.FindPropertyRelative("Timings");
                    SerializedProperty sp_ActivateFrom = sp_node.FindPropertyRelative("ActivateFrom");
                    SerializedProperty sp_ActivateEnd = sp_node.FindPropertyRelative("ActivateEnd");
                    SerializedProperty sp_ActivateOnlyToEnd = sp_node.FindPropertyRelative("ActivateOnlyToEnd");
                    SerializedProperty sp_LoopType = sp_node.FindPropertyRelative("LoopType");
                    SerializedProperty sp_LoopCount = sp_node.FindPropertyRelative("LoopCount");
                    SerializedProperty sp_Progress = sp_node.FindPropertyRelative("Progress");
                    SerializedProperty sp_Type = sp_node.FindPropertyRelative("Type");
                    #endregion

                    float topheight = 10;

                    #region 开关
                    draw_rect.Set(rect.x + 30, rect.y + 8 + topheight, 85, LineHeight);
                    SerializedProperty sp_Enabled = sp_PrimitiveTweenNodes.GetArrayElementAtIndex(index).FindPropertyRelative("Enabled");
                    sp_Enabled.boolValue = Editor_XHud_GUI.Gui_Toggle(draw_rect, true, null, sp_Enabled.boolValue);
                    sp_Enabled.serializedObject.ApplyModifiedProperties();
                    #endregion

                    if (!sp_Enabled.boolValue)
                    {
                        draw_rect.Set(rect.x - 8, rect.y + 12 + topheight, 1, 10);
                        Editor_XHud_GUI.Gui_Box(draw_rect, Color.red);
                    }

                    #region 名称
                    draw_rect.Set(rect.x + 55, rect.y + 7 + topheight, (rect.width / 2) - 40, LineHeight);
                    SerializedProperty sp_Name = sp_PrimitiveTweenNodes.GetArrayElementAtIndex(index).FindPropertyRelative("Indicator");
                    Editor_XHud_GUI.Gui_Property_Field(draw_rect, "", sp_Name, 0, 0);
                    sp_Name.serializedObject.ApplyModifiedProperties();
                    #endregion

                    #region 类型标签

                    string[] TweenNodeTypes = System.Enum.GetNames(typeof(TweenNodeType));
                    Color bgscol = GUI.color;
                    GUI.color = XHud_Dashboard.Theme_Primary;
                    draw_rect.Set(rect.width / 2 + 90, rect.y + 8 + topheight, rect.width / 2 - 80, LineHeight);
                    sp_Type.intValue = Editor_XHud_GUI.Gui_Popup(draw_rect, sp_Type.intValue, TweenNodeTypes, HudFilled.实体, HudColor.亮白, Color.black);

                    GUI.color = bgscol;
                    sp_Type.serializedObject.ApplyModifiedProperties();
                    #endregion

                    #region 类型图标
                    Texture2D typeicon = null;
                    TweenNodeType nodetype = (TweenNodeType)sp_Type.enumValueIndex;
                    switch (nodetype)
                    {
                        case TweenNodeType.位移:
                            typeicon = anim_type_mover;
                            break;
                        case TweenNodeType.旋转:
                            typeicon = anim_type_rotator;
                            break;
                        case TweenNodeType.缩放:
                            typeicon = anim_type_scale;
                            break;
                        case TweenNodeType.颜色:
                            typeicon = anim_type_color;
                            break;
                        case TweenNodeType.淡化:
                            typeicon = anim_type_fade;
                            break;
                        case TweenNodeType.打字机:
                            typeicon = anim_type_writter;
                            break;
                        case TweenNodeType.图像填充:
                            typeicon = anim_type_fill;
                            break;
                        case TweenNodeType.尺寸:
                            typeicon = anim_type_size;
                            break;
                    }
                    GUI.color = Color.white;
                    draw_rect.Set(rect.width + 28, rect.y + 12 + topheight, 12, 12);
                    Editor_XHud_GUI.Gui_Icon(draw_rect, typeicon);
                    GUI.color = Color.white;
                    #endregion

                    float baseheight = rect.y + 80;
                    if (!sp_IsFold.boolValue)
                    {
                        draw_rect.Set(rect.x + 10, rect.y + 10 + topheight, 12, 12);
                        if (Editor_XHud_GUI.Gui_Button(draw_rect, icon_unfold_r, icon_unfold_p, true, "", "", Color.white))
                        {
                            sp_IsFold.boolValue = !sp_IsFold.boolValue;
                            sp_IsFold.serializedObject.ApplyModifiedProperties();
                        }
                    }
                    else
                    {
                        draw_rect.Set(rect.x + 10, rect.y + 10 + topheight, 12, 12);
                        if (Editor_XHud_GUI.Gui_Button(draw_rect, anim_fold_r, anim_fold_p, true, "", "", Color.white))
                        {
                            sp_IsFold.boolValue = !sp_IsFold.boolValue;
                            sp_IsFold.serializedObject.ApplyModifiedProperties();
                        }
                    }
                    if (!sp_IsFold.boolValue)
                    {
                        #region 判断文字组件是否为库同步样式状态
                        bool IsTextColorMode = false;
                        if (BaseScript.controller != null)
                        {
                            ModuleType animtype = BaseScript.controller.GetModuleType();
                            if (nodetype == TweenNodeType.颜色)
                            {
                                if (animtype == Enums.ModuleType.Text || animtype == Enums.ModuleType.TmpText)
                                {
                                    XHud_Module_Text text = BaseScript.controller.mod_Text;
                                    XHud_Module_TmpText tmptext = BaseScript.controller.mod_TmpText;
                                    if ((text && text.StyleLibSynching && text.TextStyleInfo.LibStyle_Effect_color) ||
                                    (tmptext && tmptext.StyleLibSynching && tmptext.TextStyleInfo.LibStyle_Effect_color))
                                    {
                                        IsTextColorMode = true;
                                    }
                                }
                            }
                        }
                        #endregion
                        if (!IsTextColorMode)
                        {
                            #region ID
                            Color bgcol = GUI.color;
                            GUI.color = XHud_Dashboard.Theme_Primary;
                            draw_rect.Set(rect.x + 5, baseheight - 28f, 100, 15);
                            Editor_XHud_GUI.Gui_Labelfield(draw_rect, "ID： " + sp_ID.intValue, HudFilled.无, HudColor.无, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 11);
                            GUI.color = bgcol;
                            draw_rect.Set(rect.width + 25, baseheight - 28f, 15, 15);
                            // 刷新动画节点 ID
                            if (Editor_XHud_GUI.Gui_Button(draw_rect, anim_change_id_r, anim_change_id_p, true, "", "", Color.white))
                            {
                                int id = BaseScript.TweenNode_ID_Create();
                                sp_ID.intValue = id;
                                sp_ID.serializedObject.ApplyModifiedProperties();
                            }
                            #endregion

                            #region 耗时
                            draw_rect.Set(rect.x, baseheight, rect.width / 2 - 10, LineHeight);
                            Editor_XHud_GUI.Gui_Property_Field(draw_rect, "耗时", sp_Duration, 5, 30);
                            #endregion

                            #region 延迟
                            draw_rect.Set(rect.width / 2 + 50, baseheight, rect.width / 2 - 10, LineHeight);
                            Editor_XHud_GUI.Gui_Property_Field(draw_rect, "延迟", sp_Delay, 5, 30);
                            sp_Delay.serializedObject.ApplyModifiedProperties();
                            #endregion

                            #region 缓动
                            if ((EaseMode)sp_Ease.enumValueIndex == EaseMode.None)
                            {
                                draw_rect.Set(rect.x, baseheight + 30, rect.width / 2 + 20, LineHeight);
                                Editor_XHud_GUI.Gui_Property_Field(draw_rect, "缓动", sp_Ease, 5, 30);
                            }
                            else
                            {
                                draw_rect.Set(rect.x, baseheight + 30, rect.width - 10, LineHeight);
                                Editor_XHud_GUI.Gui_Property_Field(draw_rect, "缓动", sp_Ease, 5, 30);
                            }
                            sp_Ease.serializedObject.ApplyModifiedProperties();
                            #endregion

                            float hh = -30;

                            #region 曲线
                            if (HudManager != null)
                            {
                                if (HudManager.Hud_Curves != null && !HudManager.Hud_Curves.CurveLibrary_IsEmpty() && (EaseMode)sp_Ease.enumValueIndex == EaseMode.None)
                                {
                                    hh = 0;
                                    string[] names = HudManager.Hud_Curves.CurveLibrary_GetCurveNames();
                                    EditorGUI.BeginChangeCheck();
                                    draw_rect.Set(rect.width / 2 + 80, baseheight + 31, rect.width / 2 - 34, 17);
                                    Editor_XHud_GUI.Gui_PopupWithString(draw_rect, ref sp_AnimationCurveName, names, HudFilled.实体, HudColor.亮白, Color.black);
                                    sp_AnimationCurveName.serializedObject.ApplyModifiedProperties();
                                    if (!HudManager.Hud_Curves.CurvesLibrary_NameIsValid(sp_AnimationCurveName.stringValue))
                                    {
                                        sp_AnimationCurveName.stringValue = "";
                                    }
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        sp_Curve.animationCurveValue = HudManager.Hud_Curves.CurveLibrary_GetCurve(sp_AnimationCurveName.stringValue);
                                    }
                                    draw_rect.Set(rect.x, baseheight + 60, rect.width - 10, LineHeight);
                                    Editor_XHud_GUI.Gui_Property_Field(draw_rect, "曲线", sp_Curve, 5, 30);

                                    sp_Curve.serializedObject.ApplyModifiedProperties();
                                    sp_AnimationCurveName.serializedObject.ApplyModifiedProperties();
                                }
                            }
                            #endregion

                            #region 时机
                            string[] TimingType = null;

                            if (HudButton != null)
                            {
                                TimingType = new string[9] { "无", "鼠标进入", "鼠标退出", "鼠标按下", "鼠标松开", "鼠标长按", "鼠标点击", "鼠标选中", "鼠标取消选中" };
                            }
                            else if (HudProgress != null)
                            {
                                TimingType = new string[5] { "无", "进度开始时", "进度变化时", "进度结束时", "进度重置时" };
                            }
                            else if (HudToggle != null)
                            {
                                TimingType = new string[7] { "无", "开关打开时", "开关关闭时", "开关按下时", "开关抬起时", "开关变化时", "开关变化中" };
                            }
                            else if (HudSlider != null)
                            {
                                TimingType = new string[5] { "无", "按下滑动条", "松开滑动条", "滑动条数值改变", "滑动条数值变化中" };
                            }
                            else if (HudOption != null)
                            {
                                TimingType = new string[5] { "无", "点击选项", "光标移动开始", "光标移动结束", "光标位置改变" };
                            }
                            else
                            {
                                TimingType = new string[4] { "元素进入时", "元素进入后", "元素退出时", "自定义" };

                            }

                            draw_rect.Set(rect.x + 5, baseheight + 90 + hh, 30, LineHeight);
                            Editor_XHud_GUI.Gui_Labelfield_Thin(draw_rect, "时机", HudFilled.无, HudColor.无, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 11);
                            Color bgstcol = GUI.color;
                            GUI.color = XHud_Dashboard.Theme_Primary;
                            draw_rect.Set(rect.x + 35, baseheight + 90 + hh, rect.width / 2 - 40, LineHeight);
                            EditorGUI.BeginChangeCheck();
                            Editor_XHud_GUI.Gui_PopupWithString(draw_rect, ref sp_Timings, TimingType, HudFilled.实体, HudColor.亮白, Color.black);
                            if (EditorGUI.EndChangeCheck())
                            {
                                // 再次收集动画列表所有动画时机名称
                                Preview_PrimitiveTweens_CollectedTimings(BaseScript);
                            }
                            GUI.color = bgstcol;
                            #endregion

                            #region 动向
                            draw_rect.Set(rect.x + rect.width / 2 + 3, baseheight + 90 + hh, 30, LineHeight);
                            Editor_XHud_GUI.Gui_Labelfield_Thin(draw_rect, "动向", HudFilled.无, HudColor.无, Color.white, TextAnchor.MiddleLeft, Vector2.zero, 11);
                            int dir_index = 0;

                            if (sp_ActivateFrom.boolValue && !sp_ActivateEnd.boolValue && !sp_ActivateOnlyToEnd.boolValue)
                            {
                                dir_index = 0;
                            }
                            if (!sp_ActivateFrom.boolValue && sp_ActivateEnd.boolValue && !sp_ActivateOnlyToEnd.boolValue)
                            {
                                dir_index = 1;
                            }
                            if (sp_ActivateFrom.boolValue && sp_ActivateEnd.boolValue && !sp_ActivateOnlyToEnd.boolValue)
                            {
                                dir_index = 2;
                            }
                            if (!sp_ActivateFrom.boolValue && !sp_ActivateEnd.boolValue && sp_ActivateOnlyToEnd.boolValue)
                            {
                                dir_index = 3;
                            }

                            string[] directionTexts = new string[4] { "起始 -> 默认", "默认 -> 结束", "起始 -> 结束", "当前 -> 结束" };

                            Color bgscol_dir = GUI.color;
                            GUI.color = XHud_Dashboard.Theme_Primary;
                            draw_rect.Set(rect.width / 2 + 83, baseheight + 90 + hh, rect.width / 2 - 70, LineHeight);
                            EditorGUI.BeginChangeCheck();
                            dir_index = Editor_XHud_GUI.Gui_Popup(draw_rect, dir_index, directionTexts, HudFilled.实体, HudColor.亮白, Color.black);
                            if (EditorGUI.EndChangeCheck())
                            {
                                // 再次收集动画列表所有动画时机名称
                                Preview_PrimitiveTweens_CollectedTimings(BaseScript);
                            }
                            GUI.color = bgscol_dir;

                            if (dir_index == 0)
                            {
                                sp_ActivateFrom.boolValue = true;
                                sp_ActivateEnd.boolValue = false;
                                sp_ActivateOnlyToEnd.boolValue = false;
                            }
                            if (dir_index == 1)
                            {
                                sp_ActivateFrom.boolValue = false;
                                sp_ActivateEnd.boolValue = true;
                                sp_ActivateOnlyToEnd.boolValue = false;
                            }
                            if (dir_index == 2)
                            {
                                sp_ActivateFrom.boolValue = true;
                                sp_ActivateEnd.boolValue = true;
                                sp_ActivateOnlyToEnd.boolValue = false;
                            }
                            if (dir_index == 3)
                            {
                                sp_ActivateFrom.boolValue = false;
                                sp_ActivateEnd.boolValue = false;
                                sp_ActivateOnlyToEnd.boolValue = true;
                            }
                            sp_ActivateFrom.serializedObject.ApplyModifiedProperties();
                            sp_ActivateEnd.serializedObject.ApplyModifiedProperties();
                            sp_ActivateOnlyToEnd.serializedObject.ApplyModifiedProperties();
                            #endregion

                            #region 动向使用警告
                            draw_rect.Set(rect.width + 23, baseheight + 91 + hh, 14, 14);
                            if (Editor_XHud_GUI.Gui_Button(draw_rect, anim_dir_war_r, anim_dir_war_p, true, "", "", Color.white))
                            {
                                string hexcol = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary, true);
                                string nav = "";
                                switch (dir_index)
                                {
                                    case 0:
                                        nav = $"该动向模式是指一个<color={hexcol}>  目标值  </color>从<color={hexcol}>  起始值  </color>开始向<color={hexcol}>  默认值  </color>进行变化，在该动向模式下如果<color={hexcol}>  调用Rewind方法  </color>则会让<color={hexcol}>  目标值  </color>回退到<color={hexcol}>  起始值  </color>的状态！\n\n<color={hexcol}>变化方向：</color>起始值  -->  默认值\n<color={hexcol}>适用场景：</color>既定数值的变化状态";
                                        break;
                                    case 1:
                                        nav = $"该动向模式是指一个<color={hexcol}>  目标值  <color>从<color={hexcol}>  默认值  <color>开始向<color={hexcol}>  结束值  <color>进行变化，在该动向模式下如果<color={hexcol}>  调用Rewind方法  <color>则会让<color={hexcol}>  目标值  <color>回退到<color={hexcol}>  默认值  <color>的状态！\n\n<color={hexcol}>变化方向：</color>默认值  -->  结束值\n<color={hexcol}>适用场景：</color>既定数值的变化状态";
                                        break;
                                    case 2:
                                        nav = $"该动向模式是指一个<color={hexcol}>  目标值  <color>从<color={hexcol}>  起始值  <color>开始向<color={hexcol}>  结束值  <color>进行变化，在该动向模式下如果<color={hexcol}>  调用Rewind方法  <color>则会让<color={hexcol}>  目标值  <color>回退到<color={hexcol}>  起始值  <color>的状态！\n\n<color={hexcol}>变化方向：起始值  -->  结束值\n适用场景：既定数值的变化状态";
                                        break;
                                    case 3:
                                        nav = $"该动向模式较为特殊，是指一个<color={hexcol}>  目标值  <color>从<color={hexcol}>  自身当前值  <color>开始向<color={hexcol}>  结束值  <color>进行变化，在该动向模式下如果<color={hexcol}>  调用Rewind方法  <color>则不会影响任何数值变化的状态！\n\n<color={hexcol}>变化方向：</color>自身当前值  -->  结束值\n<color={hexcol}>适用场景：</color>动态指定数值的变化状态";
                                        break;
                                }
                                Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 图元动画器消息", "动向模式解释", nav, "明白", 0, false);

                                return;
                            }
                            #endregion

                            #region 循环
                            draw_rect.Set(rect.x, baseheight + 120 + hh, rect.width / 2 + 20, LineHeight);
                            Editor_XHud_GUI.Gui_Property_Field(draw_rect, "循环", sp_LoopType, 5, 30);
                            #endregion

                            #region 循环次数
                            draw_rect.Set(rect.width / 2 + 78, baseheight + 120 + hh, rect.width / 2 - 38, LineHeight);
                            Editor_XHud_GUI.Gui_Property_Field(draw_rect, "次数", sp_LoopCount, 5, 30);
                            #endregion

                            #region 重置时设为起始值
                            draw_rect.Set(rect.x, baseheight + 150 + hh, rect.width - 10, LineHeight);
                            Editor_XHud_GUI.Gui_Property_Field(draw_rect, "动画 - 重置时 - 设为起始值", sp_Rewind_Set_Startvalue, 5, rect.width - 26);
                            #endregion

                            #region 完成时设为结束值
                            draw_rect.Set(rect.x, baseheight + 180 + hh, rect.width - 10, LineHeight);
                            Editor_XHud_GUI.Gui_Property_Field(draw_rect, "动画 - 完成时 - 设为结束值", sp_Complete_Set_Endvalue, 5, rect.width - 26);
                            #endregion

                            #region 音效
                            if (sp_LoopCount.intValue != -1)
                            {
                                draw_rect.Set(rect.width - 10, baseheight - 29, 15, 15);
                                if (Editor_XHud_GUI.Gui_Button(draw_rect, anim_sound_r, anim_sound_p, true, "", "", Color.white))
                                {
                                    Editor_XHud_PrimitiveTweenSoundSetTool = (Editor_XHud_PrimitiveTweenSoundSetTool)EditorWindow.GetWindow(typeof(Editor_XHud_PrimitiveTweenSoundSetTool), false, "图元动画器节点音效设置器", true);
                                    Editor_XHud_PrimitiveTweenSoundSetTool.minSize = new Vector2(350, 600);
                                    Editor_XHud_PrimitiveTweenSoundSetTool.maxSize = Editor_XHud_PrimitiveTweenSoundSetTool.minSize;
                                    Editor_XHud_PrimitiveTweenSoundSetTool.Show();
                                    PrimitiveTweenSoundNode node = new PrimitiveTweenSoundNode();
                                    node.Tween = BaseScript;
                                    node.Index = index;
                                    node.Name = sp_Name.stringValue;
                                    node.Type = nodetype;
                                    node.MaxDuration = (sp_Duration.floatValue * sp_GlobalDuration.floatValue) + sp_Delay.floatValue;
                                    node.TweenSounds = new List<TweenSound>();
                                    for (int i = 0; i < sp_TweenSounds.arraySize; i++)
                                    {
                                        TweenSound ts = new TweenSound();
                                        ts.Sound = (AudioClip)sp_TweenSounds.GetArrayElementAtIndex(i).FindPropertyRelative("Sound").objectReferenceValue;
                                        ts.Percentage = sp_TweenSounds.GetArrayElementAtIndex(i).FindPropertyRelative("Percentage").floatValue;
                                        ts.Volume = sp_TweenSounds.GetArrayElementAtIndex(i).FindPropertyRelative("Volume").floatValue;
                                        ts.MaxPitch = sp_TweenSounds.GetArrayElementAtIndex(i).FindPropertyRelative("MaxPitch").floatValue;
                                        ts.MinPitch = sp_TweenSounds.GetArrayElementAtIndex(i).FindPropertyRelative("MinPitch").floatValue;
                                        node.TweenSounds.Add(ts);
                                    }
                                    Editor_XHud_PrimitiveTweenSoundSetTool.PrimitiveTweenSoundNode = node;
                                    Editor_XHud_PrimitiveTweenSoundSetTool.Repaint();
                                }
                            }
                            GUI.color = Color.white;
                            #endregion

                            GUI.backgroundColor = new Color(255, 255, 255, 0.45f);
                            draw_rect.Set(rect.x, baseheight + 210 + hh, rect.width, 1);
                            GUI.Box(draw_rect, "");
                            GUI.backgroundColor = Color.white;

                            Rect rect_valuepanel = new Rect(draw_rect.x, draw_rect.y + 15, rect.width, 110);

                            #region 分类
                            switch (nodetype)
                            {
                                case TweenNodeType.位移:
                                    SerializedProperty sp_pos_ori = sp_node.FindPropertyRelative("Original_Vector3");
                                    SerializedProperty sp_pos_from = sp_node.FindPropertyRelative("From_Vector3");
                                    SerializedProperty sp_pos_end = sp_node.FindPropertyRelative("End_Vector3");
                                    Draw_TweenValues(rect_valuepanel, dir_index, sp_pos_ori, sp_pos_from, sp_pos_end, nodetype, sp_ActivateOnlyToEnd.boolValue);
                                    break;
                                case TweenNodeType.旋转:
                                    SerializedProperty sp_rot_ori = sp_node.FindPropertyRelative("Original_Vector3");
                                    SerializedProperty sp_rot_from = sp_node.FindPropertyRelative("From_Vector3");
                                    SerializedProperty sp_rot_end = sp_node.FindPropertyRelative("End_Vector3");
                                    Draw_TweenValues(rect_valuepanel, dir_index, sp_rot_ori, sp_rot_from, sp_rot_end, nodetype, sp_ActivateOnlyToEnd.boolValue);
                                    break;
                                case TweenNodeType.缩放:
                                    SerializedProperty sp_scale_ori = sp_node.FindPropertyRelative("Original_Vector3");
                                    SerializedProperty sp_scale_from = sp_node.FindPropertyRelative("From_Vector3");
                                    SerializedProperty sp_scale_end = sp_node.FindPropertyRelative("End_Vector3");
                                    Draw_TweenValues(rect_valuepanel, dir_index, sp_scale_ori, sp_scale_from, sp_scale_end, nodetype, sp_ActivateOnlyToEnd.boolValue);
                                    break;
                                case TweenNodeType.颜色:
                                    SerializedProperty sp_color_ori = sp_node.FindPropertyRelative("Original_Color");
                                    SerializedProperty sp_color_from = sp_node.FindPropertyRelative("From_Color");
                                    SerializedProperty sp_color_end = sp_node.FindPropertyRelative("End_Color");
                                    Draw_TweenValues(rect_valuepanel, dir_index, sp_color_ori, sp_color_from, sp_color_end, nodetype, sp_ActivateOnlyToEnd.boolValue);
                                    break;
                                case TweenNodeType.淡化:
                                    SerializedProperty sp_fade_ori = sp_node.FindPropertyRelative("Original_Float");
                                    SerializedProperty sp_fade_from = sp_node.FindPropertyRelative("From_Float");
                                    SerializedProperty sp_fade_end = sp_node.FindPropertyRelative("End_Float");
                                    Draw_TweenValues(rect_valuepanel, dir_index, sp_fade_ori, sp_fade_from, sp_fade_end, nodetype, sp_ActivateOnlyToEnd.boolValue);
                                    break;
                                case TweenNodeType.打字机:
                                    SerializedProperty sp_text_ori = sp_node.FindPropertyRelative("Original_String");
                                    SerializedProperty sp_text_from = sp_node.FindPropertyRelative("From_String");
                                    SerializedProperty sp_text_end = sp_node.FindPropertyRelative("End_String");
                                    Draw_TweenValues(rect_valuepanel, dir_index, sp_text_ori, sp_text_from, sp_text_end, nodetype, sp_ActivateOnlyToEnd.boolValue);
                                    break;
                                case TweenNodeType.图像填充:
                                    SerializedProperty sp_fill_ori = sp_node.FindPropertyRelative("Original_Float");
                                    SerializedProperty sp_fill_from = sp_node.FindPropertyRelative("From_Float");
                                    SerializedProperty sp_fill_end = sp_node.FindPropertyRelative("End_Float");
                                    Draw_TweenValues(rect_valuepanel, dir_index, sp_fill_ori, sp_fill_from, sp_fill_end, nodetype, sp_ActivateOnlyToEnd.boolValue);
                                    break;
                                case TweenNodeType.尺寸:
                                    SerializedProperty sp_size_ori = sp_node.FindPropertyRelative("Original_Vector2");
                                    SerializedProperty sp_size_from = sp_node.FindPropertyRelative("From_Vector2");
                                    SerializedProperty sp_size_end = sp_node.FindPropertyRelative("End_Vector2");
                                    Draw_TweenValues(rect_valuepanel, dir_index, sp_size_ori, sp_size_from, sp_size_end, nodetype, sp_ActivateOnlyToEnd.boolValue);
                                    break;
                            }
                            #endregion
                        }
                        else
                        {
                            #region 文字变色不支持提示
                            draw_rect.Set(rect.x, baseheight - 36f, rect.width, 35);
                            EditorGUI.HelpBox(draw_rect, "当文字使用  \"库样式同步\" 状态时不支持颜色动画", MessageType.Warning);
                            #endregion
                        }
                    }

                    #region 动画进度条
                    draw_rect.Set(rect.x + 10, rect.y + 6, (rect.width - 20), 1);
                    EditorGUI.DrawRect(draw_rect, Editor_XHud_GUI.GetColor(HudColor.深空灰));
                    draw_rect.Set(rect.x + 10, rect.y + 6, (rect.width - 20) * sp_Progress.floatValue, 1);
                    EditorGUI.DrawRect(draw_rect, Editor_XHud_GUI.GetColor(HudColor.工业蓝));
                    ///---音效触发点
                    for (int p = 0; p < sp_TweenSounds.arraySize; p++)
                    {
                        SerializedProperty sp_soundnode = sp_TweenSounds.GetArrayElementAtIndex(p);
                        SerializedProperty sp_soundper = sp_soundnode.FindPropertyRelative("Percentage");
                        draw_rect.Set(rect.x + 10 + ((rect.width - 20) * sp_soundper.floatValue * 0.01f), rect.y + 6, 3, 3);
                        EditorGUI.DrawRect(draw_rect, Editor_XHud_GUI.GetColor(HudColor.亮金色));
                    }
                    draw_rect.Set(rect.x + 10 + ((rect.width - 20) * sp_Progress.floatValue), rect.y + 3, 2, 8);
                    EditorGUI.DrawRect(draw_rect, Editor_XHud_GUI.GetColor(HudColor.亮白));
                    #endregion

                    // ========== 处理右键菜单 ==========
                    // 获取当前元素所在的矩形区域（整个元素的范围）
                    Rect elementRect = new Rect(rect.x, rect.y, rect.width, rect.height);

                    // 检查鼠标是否在当前元素区域内
                    if (Event.current.type == EventType.ContextClick && elementRect.Contains(Event.current.mousePosition))
                    {
                        GenericMenu menu = new GenericMenu();
                        menu.AddItem(new GUIContent("C 拷贝节点"), false, () =>
                        {
                            TweenNode node = BaseScript.PrimitiveTweenNodes[index];
                            string json = JsonUtility.ToJson(node);

                            Editor_XHud_GUI.EditorData_Set_With_String("XED_Hud_Copied_TweenNode", json);
                        });
                        menu.AddSeparator("");
                        menu.AddItem(new GUIContent("V 粘贴节点"), false, () =>
                        {
                            TweenNode node = new TweenNode();
                            node = JsonUtility.FromJson<TweenNode>(Editor_XHud_GUI.EditorData_Get_With_String("XED_Hud_Copied_TweenNode"));

                            if (BaseScript.TweenNode_IsRepeat(node))
                            {
                                EditorApplication.delayCall += () =>
                                {
                                    string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 图元动画器消息", "节点已存在", "发现存在重复的动画节点！请您检查后再做决定！", "追加", "覆盖", "跳过", 0, true);
                                    if (res == "追加")
                                    {
                                        node.Indicator += "Copied";
                                        node.ID = BaseScript.TweenNode_ID_Create();
                                        BaseScript.PrimitiveTweenNodes.Add(node);
                                    }
                                    if (res == "覆盖")
                                    {
                                        TweenNode r_node = BaseScript.TweenNode_GetRepeat(node);
                                        r_node.CopyFrom(node, true);
                                    }
                                };
                            }
                            else
                            {
                                node.Indicator += "Copied";
                                node.ID = BaseScript.TweenNode_ID_Create();
                                BaseScript.PrimitiveTweenNodes.Add(node);
                            }
                        });
                        menu.ShowAsContext();

                        // 使用事件，防止传递给其他控件
                        Event.current.Use();
                    }

                    sp_node.serializedObject.ApplyModifiedProperties();

                    sp_PrimitiveTweenNodes.serializedObject.ApplyModifiedProperties();
                },
                onAddCallback = (ReorderableList list) =>
                {
                    sp_PrimitiveTweenNodes.InsertArrayElementAtIndex(list.count);

                    SerializedProperty sp_Root = sp_PrimitiveTweenNodes.GetArrayElementAtIndex(list.count - 1);

                    sp_Root.FindPropertyRelative("Indicator").stringValue = "NewTween";
                    sp_Root.FindPropertyRelative("ID").intValue = BaseScript.TweenNode_ID_Create();
                    sp_Root.FindPropertyRelative("Type").enumValueIndex = 0;
                    sp_Root.FindPropertyRelative("Enabled").boolValue = true;
                    sp_Root.FindPropertyRelative("Timings").stringValue = "无";
                    sp_Root.FindPropertyRelative("Duration").floatValue = 1;
                    sp_Root.FindPropertyRelative("Ease").enumValueIndex = 10;
                    sp_Root.FindPropertyRelative("Rewind_Set_Startvalue").boolValue = true;
                    sp_Root.FindPropertyRelative("Complete_Set_Endvalue").boolValue = true;
                    sp_Root.FindPropertyRelative("Curve").animationCurveValue = AnimationCurve.EaseInOut(0, 0, 1, 1);
                    sp_Root.FindPropertyRelative("Original_Color").colorValue = Color.white;
                    sp_Root.FindPropertyRelative("From_Color").colorValue = Color.white;
                    sp_Root.FindPropertyRelative("End_Color").colorValue = Color.white;
                    sp_Root.FindPropertyRelative("IsFold").boolValue = false;
                    sp_Root.FindPropertyRelative("TweenSounds").ClearArray();
                    sp_Root.FindPropertyRelative("ActivateFrom").boolValue = true;
                    sp_Root.FindPropertyRelative("ActivateEnd").boolValue = false;
                    sp_Root.FindPropertyRelative("ActivateOnlyToEnd").boolValue = false;
                    sp_Root.FindPropertyRelative("LoopType").enumValueIndex = (int)XTween_LoopType.Restart;
                    sp_Root.FindPropertyRelative("LoopCount").intValue = 0;
                    sp_Root.FindPropertyRelative("Progress").floatValue = 0;
                    sp_Root.serializedObject.ApplyModifiedProperties();

                    EditorApplication.delayCall += () =>
                    {
                        // 刷新获取动画列表所有动画时机名称
                        Preview_PrimitiveTweens_CollectedTimings(BaseScript);
                    };
                },
                onRemoveCallback = (ReorderableList list) =>
                {
                    sp_PrimitiveTweenNodes.DeleteArrayElementAtIndex(list.index);
                    sp_PrimitiveTweenNodes.serializedObject.ApplyModifiedProperties();

                    EditorApplication.delayCall += () =>
                    {
                        // 刷新获取动画列表所有动画时机名称
                        Preview_PrimitiveTweens_CollectedTimings(BaseScript);
                    };
                },
                elementHeightCallback = index =>
                {
                    SerializedProperty sp_Root = sp_PrimitiveTweenNodes.GetArrayElementAtIndex(index);
                    SerializedProperty sp_type = sp_Root.FindPropertyRelative("Type");
                    var types = Enum.GetValues(typeof(TweenNodeType)).GetValue(sp_type.enumValueIndex);
                    SerializedProperty sp_Ease = sp_Root.FindPropertyRelative("Ease");
                    SerializedProperty sp_IsFold = sp_Root.FindPropertyRelative("IsFold");
                    SerializedProperty sp_dir_onlyend = sp_Root.FindPropertyRelative("ActivateOnlyToEnd");

                    float height = 3;
                    float add = 0;

                    if ((EaseMode)sp_Ease.enumValueIndex == EaseMode.None)
                    {
                        add = 1.2f;
                    }
                    else
                    {
                        add = -0.3f;
                    }

                    #region 分类
                    switch (types)
                    {
                        case TweenNodeType.位移:
                            if (sp_dir_onlyend.boolValue)
                                height = 17.3f + add;
                            else
                                height = 20f + add;
                            break;
                        case TweenNodeType.旋转:
                            if (sp_dir_onlyend.boolValue)
                                height = 17.3f + add;
                            else
                                height = 20f + add;
                            break;
                        case TweenNodeType.缩放:
                            if (sp_dir_onlyend.boolValue)
                                height = 17.3f + add;
                            else
                                height = 20f + add;
                            break;
                        case TweenNodeType.颜色:
                            bool IsTextColorMode = false;

                            #region 判断文字组件是否为库同步样式状态
                            ModuleType animtype = BaseScript.controller.GetModuleType();
                            if (animtype == ModuleType.Text || animtype == ModuleType.TmpText)
                            {
                                XHud_Module_Text text = BaseScript.controller.mod_Text;
                                XHud_Module_TmpText tmptext = BaseScript.controller.mod_TmpText;
                                if ((text && text.StyleLibSynching && text.TextStyleInfo.LibStyle_Effect_color) ||
                                (tmptext && tmptext.StyleLibSynching && tmptext.TextStyleInfo.LibStyle_Effect_color))
                                {
                                    IsTextColorMode = true;
                                }
                            }
                            #endregion

                            if (IsTextColorMode)
                            {
                                height = 5f + add;
                            }
                            else
                            {
                                if (sp_dir_onlyend.boolValue)
                                    height = 17.3f + add;
                                else
                                    height = 20f + add;
                            }
                            break;
                        case TweenNodeType.淡化:
                            if (sp_dir_onlyend.boolValue)
                                height = 17.3f + add;
                            else
                                height = 20f + add;
                            break;
                        case TweenNodeType.打字机:
                            if (sp_dir_onlyend.boolValue)
                                height = 17.3f + add;
                            else
                                height = 20f + add;
                            break;
                        case TweenNodeType.图像填充:
                            if (sp_dir_onlyend.boolValue)
                                height = 17.3f + add;
                            else
                                height = 20f + add;
                            break;
                        case TweenNodeType.尺寸:
                            if (sp_dir_onlyend.boolValue)
                                height = 17.3f + add;
                            else
                                height = 20f + add;
                            break;
                    }
                    #endregion

                    if (sp_IsFold.boolValue)
                    {
                        height = 2.6f;
                    }
                    return height * LineHeight;
                }
            };
            #endregion

            // 收集动画列表所有动画时机名称
            Preview_PrimitiveTweens_CollectedTimings(BaseScript);
        }

        private void OnDisable()
        {
            if (!Application.isPlaying)
            {
                Preview_PrimitiveTweens_Stop();
                Preview_PrimitiveTweens_Sound_Stop();

                // 如果音效设置器是打开的就关闭它
                if (Editor_XHud_PrimitiveTweenSoundSetTool != null)
                    Editor_XHud_PrimitiveTweenSoundSetTool.Close();
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            #region 标题
            string h_color = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.Theme_Primary);
            string titlename = "XHud - 图元  >  动画";
            Editor_XHud_GUI.Gui_Layout_Banner(icon_main, HudFilled.实体, HudColor.深空灰, titlename, Color.white, null, "", 20, 20);
            Rect rect = GUILayoutUtility.GetLastRect();
            #endregion

            #region 快捷功能
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 0, "快捷功能", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            if (Editor_XHud_GUI.Gui_Layout_Button(14, "打开时间线轨道编辑器", opentrack_r, opentrack_p, 4))
            {
                Editor_XHud_Module_Primitive_Tween_Tracker.OpenWith(BaseScript);
            }
            GUILayout.Space(10);
            #region 预览按钮
            if (!Application.isPlaying)
            {
                if (!sp_TweenIsPreviewing.boolValue)
                {
                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "预览", prw_play_r, prw_play_p, 4))
                    {
                        Preview_PrimitiveTweens_Play();
                    }
                }
                else
                {
                    if (Editor_XHud_GUI.Gui_Layout_Button(14, "停止", prw_stop_r, prw_stop_p, 4))
                    {
                        Preview_PrimitiveTweens_Stop();
                    }
                }
                Editor_XHud_GUI.Gui_Layout_FlexSpace();
            }
            #endregion

            Rect last = Editor_XHud_GUI.Gui_GetLastRect();
            Rect timRefresh_Rect = new Rect(rect.width - 140, last.y - 10, 150, 38);
            //Editor_XHud_GUI.Gui_Box(timRefresh_Rect, Color.green * 0.5f);

            if (!Targets_Selected())
            {
                // 点击预览时机下拉菜单时先更新一下
                Event e = Event.current;
                if (e.type == EventType.MouseDown && e.button == (int)MouseButton.Left && timRefresh_Rect.Contains(e.mousePosition))
                {
                    // 再次收集动画列表所有动画时机名称
                    Preview_PrimitiveTweens_CollectedTimings(BaseScript);
                }

                Editor_XHud_GUI.Gui_Layout_Popup<string, XHud_Module_Primitive_Tween>("预览时机", PreviewTimings, ref sp_PreviewTiming, HudFilled.实体, 120, 22, SelectedObjects, (comps) => { }, (res) => { });
            }
            GUILayout.Space(10);
            GUILayout.EndHorizontal();

            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 动画参数
            string info_title = "动画参数";
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, info_title, XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(5);

            #region 动画全局耗时
            Editor_XHud_GUI.Gui_Layout_Property_Field("速率倍增", sp_GlobalDuration);
            #endregion

            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion          

            #region 动画节点列表
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 0, "动画节点列表", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(10);
            if (Targets_Selected())
            {
                Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_XHud_GUI.Gui_Layout_Space(10);
                EditorGUILayout.HelpBox("动画节点不支持多项操作", MessageType.Warning);
                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Horizontal_End();
            }
            else
            {
                AnimateTweenNodesList.DoLayoutList();
                sp_PrimitiveTweenNodes.serializedObject.ApplyModifiedProperties();
            }
            Editor_XHud_GUI.Gui_Layout_Space(10);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 选项
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, "选项", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(5);

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_Primitive_Tween>("调试", opt_debug, ref sp_Debug, HudFilled.无, HudFilled.实体, Color.white * 0.9f, 120, 22, SelectedObjects);

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_Primitive_Tween>("静音", opt_mute, ref sp_MutePlay, HudFilled.无, HudFilled.实体, Color.white * 0.9f, 120, 22, SelectedObjects);

            Editor_XHud_GUI.Gui_Layout_Toggle<bool, XHud_Module_Primitive_Tween>("元素联动", opt_control, ref sp_IgnoreElementAnimationPlay, HudFilled.无, HudFilled.实体, Color.white * 0.9f, 120, 22, SelectedObjects);

            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 状态
            string statistic_title = "状态";
            if (Targets_Selected())
                statistic_title = "状态 - ( 批量模式 )";
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 5, statistic_title, XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(5);

            AnimationsTimerStatistic();

            if (!Targets_Selected())
            {
                #region 组件数量 - 动画节点
                if (sp_PrimitiveTweenNodes.arraySize <= 0)
                {
                    Editor_XHud_GUI.Gui_Layout_Labelfield("暂无统计数据", HudFilled.无, HudColor.无, Editor_XHud_GUI.GetColor(HudColor.阴影灰), TextAnchor.MiddleCenter, 11);
                }
                else
                {
                    Editor_XHud_GUI.StatuDisplayer_text(statu, 12, new Vector2(0, 8), "动画状态", 12, sp_TweenIsPreviewing.boolValue ? "运动" : "静止", XHud_Dashboard.Theme_Primary, 12);
                    Editor_XHud_GUI.StatuDisplayer_text(count, 12, new Vector2(0, 7), "动画节点", 12, sp_PrimitiveTweenNodes.arraySize.ToString() + " 个", XHud_Dashboard.Theme_Primary, 11);
                    Editor_XHud_GUI.StatuDisplayer_text(timer_min, 12, new Vector2(0, 7), "最小耗时", 12, sp_MinTimer.floatValue.ToString() + " 秒", XHud_Dashboard.Theme_Primary, 11);
                    Editor_XHud_GUI.StatuDisplayer_text(timer_max, 12, new Vector2(0, 7), "最大耗时", 12, sp_MaxTimer.floatValue.ToString() + " 秒", XHud_Dashboard.Theme_Primary, 11);
                    Editor_XHud_GUI.StatuDisplayer_text(timer_min, 12, new Vector2(0, 7), "最小耗时<color=#909090>（图元动画器倍增）</color>", 12, sp_MinTimerWithGlobalDuration.floatValue.ToString() + " 秒", XHud_Dashboard.Theme_Primary, 11);
                    Editor_XHud_GUI.StatuDisplayer_text(timer_max, 12, new Vector2(0, 7), "最大耗时<color=#909090>（图元动画器倍增）</color>", 12, sp_MaxTimerWithGlobalDuration.floatValue.ToString() + " 秒", XHud_Dashboard.Theme_Primary, 11);
                    Editor_XHud_GUI.StatuDisplayer_text(timer_max, 12, new Vector2(0, 7), "最大耗时<color=#909090>（XHUD倍增）</color>", 12, (sp_MaxTimerWithGlobalDuration.floatValue * HudManager.DurationMultiply).ToString() + " 秒", XHud_Dashboard.Theme_Primary, 11);
                }
                #endregion
            }
            else
            {
                #region 批量控件
                Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
                Editor_XHud_GUI.Gui_Layout_Space(10);

                string indicator = (string.IsNullOrEmpty(SelectedObjects[TweenStatu_Index].controller.Indicator) ? "" : $" ( {SelectedObjects[TweenStatu_Index].controller.Indicator} )");

                if (Editor_XHud_GUI.Gui_Layout_Button($"{SelectedObjects[TweenStatu_Index].name}{indicator}", "", HudFilled.透明, HudColor.无, Color.gray, 20))
                {
                    EditorGUIUtility.PingObject(SelectedObjects[TweenStatu_Index]);
                }
                Editor_XHud_GUI.Gui_Layout_FlexSpace();
                if (Editor_XHud_GUI.Gui_Layout_Button(12, "", left_arrow_r, left_arrow_p))
                {
                    if (TweenStatu_Index <= 0)
                    {
                        TweenStatu_Index = SelectedObjects.Length - 1;
                    }
                    else
                    {
                        TweenStatu_Index--;
                    }
                    EditorGUIUtility.PingObject(SelectedObjects[TweenStatu_Index]);
                }
                Editor_XHud_GUI.Gui_Layout_Space(16);
                if (Editor_XHud_GUI.Gui_Layout_Button(12, "", right_arrow_r, right_arrow_p))
                {
                    if (TweenStatu_Index >= SelectedObjects.Length - 1)
                    {
                        TweenStatu_Index = 0;
                    }
                    else
                    {
                        TweenStatu_Index++;
                    }
                    EditorGUIUtility.PingObject(SelectedObjects[TweenStatu_Index]);
                }
                Editor_XHud_GUI.Gui_Layout_Space(5);
                Editor_XHud_GUI.Gui_Layout_Horizontal_End();
                #endregion

                #region 组件数量 - 动画节点
                if (SelectedObjects[TweenStatu_Index].PrimitiveTweenNodes.Count <= 0)
                {
                    Editor_XHud_GUI.Gui_Layout_Labelfield("暂无统计数据", HudFilled.无, HudColor.无, Editor_XHud_GUI.GetColor(HudColor.阴影灰), TextAnchor.MiddleCenter, 11);
                }
                else
                {
                    Editor_XHud_GUI.StatuDisplayer_text(count, 12, new Vector2(0, 7), "动画节点数", 12, SelectedObjects[TweenStatu_Index].PrimitiveTweenNodes.Count + " 个", XHud_Dashboard.Theme_Primary, 10);
                    Editor_XHud_GUI.StatuDisplayer_text(timer_min, 12, new Vector2(0, 7), "最小耗时", 12, SelectedObjects[TweenStatu_Index].MinTimer.ToString() + " 秒", XHud_Dashboard.Theme_Primary, 10);
                    Editor_XHud_GUI.StatuDisplayer_text(timer_max, 12, new Vector2(0, 7), "最大耗时", 12, SelectedObjects[TweenStatu_Index].MaxTimer.ToString() + " 秒", XHud_Dashboard.Theme_Primary, 10);
                    Editor_XHud_GUI.StatuDisplayer_text(timer_min, 12, new Vector2(0, 7), "最小耗时<color=#909090>（图元动画器倍增）</color>", 12, SelectedObjects[TweenStatu_Index].MinTimerWithGlobalDuration.ToString() + " 秒", XHud_Dashboard.Theme_Primary, 10);
                    Editor_XHud_GUI.StatuDisplayer_text(timer_max, 12, new Vector2(0, 7), "最大耗时<color=#909090>（图元动画器倍增）</color>", 12, SelectedObjects[TweenStatu_Index].MaxTimerWithGlobalDuration.ToString() + " 秒", XHud_Dashboard.Theme_Primary, 10);
                    Editor_XHud_GUI.StatuDisplayer_text(timer_max, 12, new Vector2(0, 7), "最大耗时<color=#909090>（XHUD倍增）</color>", 12, (SelectedObjects[TweenStatu_Index].MaxTimerWithGlobalDuration * HudManager.DurationMultiply).ToString() + " 秒", XHud_Dashboard.Theme_Primary, 10);
                }
                #endregion
            }

            Editor_XHud_GUI.Gui_Layout_Space(5);
            Editor_XHud_GUI.Gui_Layout_Vertical_End();
            #endregion

            #region 右键菜单
            ContextMenu(rect);
            #endregion

            #region 源脚本
            Editor_XHud_GUI.Gui_Layout_Vertical_Start(HudFilled.纯色边框, HudColor.亮白, 3, "源脚本", XHud_Dashboard.Theme_Primary);
            Editor_XHud_GUI.Gui_Layout_Space(5);

            #region 脚本类
            Editor_XHud_GUI.Gui_Layout_Horizontal_Start(HudFilled.无, HudColor.无, 0);
            Editor_XHud_GUI.Gui_Layout_Space(10);
            OriginalDisplay = EditorGUILayout.Foldout(OriginalDisplay, "脚本类", true);
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
        /// 右键菜单
        /// </summary>
        private void ContextMenu(Rect rect)
        {
            Rect rect_MenuArea = new Rect(0, rect.y, EditorGUIUtility.currentViewWidth, 150);

            float foldvalue = 0;
            for (int i = 0; i < BaseScript.PrimitiveTweenNodes.Count; i++)
            {
                if (BaseScript.PrimitiveTweenNodes[i].IsFold)
                {
                    foldvalue += 48.8f;
                }
                else
                {
                    foldvalue += 288.2f;
                }
            }

            Rect rect_MenuArea_down = new Rect(0, rect.y + 245 + (foldvalue), EditorGUIUtility.currentViewWidth, 650);

            if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && (rect_MenuArea.Contains(Event.current.mousePosition) || rect_MenuArea_down.Contains(Event.current.mousePosition)))
            {
                // 创建右键菜单
                GenericMenu menu = new GenericMenu();
                if (!sp_TweenIsPreviewing.boolValue)
                {
                    menu.AddItem(new GUIContent("S (预览动画)"), false, () =>
                    {
                        Preview_PrimitiveTweens_Play();
                    });
                }
                else
                {
                    menu.AddItem(new GUIContent("S (停止预览)"), false, () =>
                    {
                        Preview_PrimitiveTweens_Stop();
                    });
                }
                menu.AddSeparator("");
                menu.AddDisabledItem(new GUIContent("动画效果"));
                if (!Targets_Selected())
                {
                    menu.AddItem(new GUIContent("A (拷贝动画列表)"), false, () =>
                    {
                        TweenNodeArray tnc = new TweenNodeArray();
                        tnc.TweenNodeList = new List<TweenNode>();

                        for (int i = 0; i < BaseScript.PrimitiveTweenNodes.Count; i++)
                        {
                            tnc.TweenNodeList.Add(BaseScript.PrimitiveTweenNodes[i]);
                        }

                        string json = JsonUtility.ToJson(tnc);

                        Editor_XHud_GUI.EditorData_Set_With_String("XED_PrimitiveTween_Copied_TweenNodes", json);
                        string indicator = $"( {BaseScript.controller.Indicator} )";
                        Editor_XHud_GUI.Open(XHud_DialogType.确认, "XHud - 图元动画器消息", "动画节点数据", $"已拷贝 {BaseScript.name}{(string.IsNullOrEmpty(indicator) ? "" : indicator)} 动画器的动画节点数据！", "明白");
                    });
                }
                menu.AddItem(new GUIContent("C (粘贴动画列表)"), false, () =>
                {
                    string json = Editor_XHud_GUI.EditorData_Get_With_String("XED_PrimitiveTween_Copied_TweenNodes");
                    TweenNodeArray tnc = JsonUtility.FromJson<TweenNodeArray>(json);

                    if (Targets_Selected())
                    {
                        #region 询问
                        List<XHud_GUI_Dialog_ListDatas> Datas = new List<XHud_GUI_Dialog_ListDatas>();
                        for (int i = 0; i < SelectedObjects.Length; i++)
                        {
                            XHud_GUI_Dialog_ListDatas dataitem = new XHud_GUI_Dialog_ListDatas();
                            dataitem.Title = $"动画节点数据";
                            dataitem.SubTitle = "即将粘贴到";
                            string indicator = $"( {SelectedObjects[i].controller.Indicator} )";
                            dataitem.Message = $"{SelectedObjects[i].name}{(string.IsNullOrEmpty(indicator) ? "" : indicator)}";
                            Datas.Add(dataitem);
                        }
                        string res_mul = Editor_XHud_GUI.Open(Datas.ToArray(), XHud_DialogType.警告, "XHud - 图元动画器消息", "批量粘贴动画节点数据", $"确认要将 XHudEditorData (XED) 中的动画节点数据粘贴到列表中的动画器中吗？", "粘贴", "暂不", 0);
                        if (res_mul == "暂不")
                            return;
                        #endregion

                        #region 批量粘贴动画节点数据
                        Datas.Clear();
                        for (int s = 0; s < SelectedObjects.Length; s++)
                        {
                            List<TweenNode> tweenNodes = new List<TweenNode>();
                            for (int c = 0; c < tnc.TweenNodeList.Count; c++)
                            {
                                tweenNodes.Add(tnc.TweenNodeList[c].Clone());
                            }

                            SelectedObjects[s].PrimitiveTweenNodes = tweenNodes;

                            XHud_GUI_Dialog_ListDatas dataitem = new XHud_GUI_Dialog_ListDatas();
                            dataitem.Title = "动画节点数据";
                            dataitem.SubTitle = "已粘贴到动画器";
                            string indicator = $"( {SelectedObjects[s].controller.Indicator} )";
                            dataitem.Message = $"{SelectedObjects[s].name}{(string.IsNullOrEmpty(indicator) ? "" : indicator)}";

                            Datas.Add(dataitem);
                        }
                        Editor_XHud_GUI.Open(Datas.ToArray(), XHud_DialogType.确认, "XHud - 图元动画器消息", "批量粘贴动画节点数据", "以下是已粘贴动画节点数据的动画器列表，请您检查核对：", "明白");
                        #endregion
                    }
                    else
                    {
                        #region 询问
                        string indicator = $"( {BaseScript.controller.Indicator} )";
                        string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 图元动画器消息", "粘贴动画节点数据", $"确认要将 XHudEditorData (XED) 中的动画节点数据粘贴到  {BaseScript.name}{(string.IsNullOrEmpty(indicator) ? "" : indicator)} 动画器中吗？", "粘贴", "暂不", 0);
                        if (res == "暂不")
                            return;
                        #endregion

                        #region 粘贴动画节点数据
                        List<TweenNode> tweenNodes = new List<TweenNode>();
                        for (int s = 0; s < tnc.TweenNodeList.Count; s++)
                        {
                            tweenNodes.Add(tnc.TweenNodeList[s].Clone());
                        }
                        BaseScript.PrimitiveTweenNodes = tweenNodes;
                        Editor_XHud_GUI.Open(XHud_DialogType.确认, "XHud - 图元动画器消息", "粘贴动画节点", $"已将动画节点粘贴到： {BaseScript.name} {BaseScript.name}{(string.IsNullOrEmpty(indicator) ? "" : indicator)}", "明白");
                        #endregion
                    }
                });
                menu.AddItem(new GUIContent("E (清空动画列表)"), false, () =>
                {
                    string res = Editor_XHud_GUI.Open(XHud_DialogType.警告, "XHud - 图元动画器消息", "清空动画效果列表", "快速清空动画效果列表此操作不可逆，是否需要清空？清空后您为此动画器做的动画效果参数将全部丢失，请谨慎此操作！", "清空", "暂不", 1);
                    if (res == "暂不")
                    {
                        return;
                    }

                    if (Targets_Selected())
                    {
                        List<XHud_GUI_Dialog_ListDatas> Datas = new List<XHud_GUI_Dialog_ListDatas>();
                        for (int s = 0; s < SelectedObjects.Length; s++)
                        {
                            Undo.RecordObject(SelectedObjects[s], "PasteAnimationTweenNodes");
                            for (int i = 1; i < SelectedObjects.Length; i++)
                            {
                                SelectedObjects[s].PrimitiveTweenNodes.Clear();

                                XHud_GUI_Dialog_ListDatas dataitem = new XHud_GUI_Dialog_ListDatas();
                                dataitem.Title = $"图元动画器";
                                string indicator = $"( {SelectedObjects[s].controller.Indicator} )";
                                dataitem.SubTitle = $"{SelectedObjects[s].name}{(string.IsNullOrEmpty(indicator) ? "" : indicator)}";
                                dataitem.Message = $"动画效果已清空";

                                Datas.Add(dataitem);
                            }
                        }
                        Editor_XHud_GUI.Open(Datas.ToArray(), XHud_DialogType.确认, "XHud - 图元动画器消息", "批量清空动画效果列表", "以下是已清空动画效果列表的图元动画器列表，请您检查核对：", "明白");
                    }
                    else
                    {
                        BaseScript.PrimitiveTweenNodes.Clear();

                        string indicator = $"( {BaseScript.controller.Indicator} )";
                        Editor_XHud_GUI.Open(XHud_DialogType.确认, "XHud - 图元动画器消息", "清空图元动画效果列表", $"已将  {BaseScript.name}{(string.IsNullOrEmpty(indicator) ? "" : indicator)} 动画器的动画效果列表清空！", "明白");
                    }
                });
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("F (动画列表 - 全部折叠)"), false, () =>
                {
                    if (Targets_Selected())
                    {
                        for (int i = 0; i < SelectedObjects.Length; i++)
                        {
                            for (int s = 1; s < SelectedObjects[i].PrimitiveTweenNodes.Count; s++)
                            {
                                SelectedObjects[i].PrimitiveTweenNodes[s].IsFold = true;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < BaseScript.PrimitiveTweenNodes.Count; i++)
                        {
                            BaseScript.PrimitiveTweenNodes[i].IsFold = true;
                        }
                    }
                });
                menu.AddItem(new GUIContent("D (动画列表 - 全部展开)"), false, () =>
                {
                    if (Targets_Selected())
                    {
                        for (int i = 0; i < SelectedObjects.Length; i++)
                        {
                            for (int s = 1; s < SelectedObjects[i].PrimitiveTweenNodes.Count; s++)
                            {
                                SelectedObjects[i].PrimitiveTweenNodes[s].IsFold = false;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < BaseScript.PrimitiveTweenNodes.Count; i++)
                        {
                            BaseScript.PrimitiveTweenNodes[i].IsFold = false;
                        }
                    }
                });
                menu.ShowAsContext(); // 在鼠标位置显示右键菜单
            }
        }

        /// <summary>
        /// 获取所有序列化字段
        /// </summary>
        private void GetSerializeFields()
        {
            sp_Debug = serializedObject.FindProperty("Debug");
            sp_PrimitiveTweenNodes = serializedObject.FindProperty("PrimitiveTweenNodes");
            sp_TweenIsPreviewing = serializedObject.FindProperty("TweenIsPreviewing");
            sp_GlobalDuration = serializedObject.FindProperty("GlobalDuration");
            sp_MutePlay = serializedObject.FindProperty("MutePlay");
            sp_MaxTimer = serializedObject.FindProperty("MaxTimer");
            sp_MinTimer = serializedObject.FindProperty("MinTimer");
            sp_MinTimerWithGlobalDuration = serializedObject.FindProperty("MinTimerWithGlobalDuration");
            sp_MaxTimerWithGlobalDuration = serializedObject.FindProperty("MaxTimerWithGlobalDuration");
            sp_IgnoreElementAnimationPlay = serializedObject.FindProperty("IgnoreElementAnimationPlay");
            sp_PreviewTiming = serializedObject.FindProperty("PreviewTiming");
        }

        #region 动画值控件
        /// <summary>
        /// 通用参数面板
        /// </summary>
        private void Draw_TweenValues(Rect rect, int dir_index, SerializedProperty origin, SerializedProperty from, SerializedProperty end, TweenNodeType type, bool onlyToEnd)
        {
            if (!onlyToEnd)
            {
                Draw_DirectionButton(rect, onlyToEnd, dir_index, origin, from, end, false);
                Draw_ValueButton(rect, origin, dir_index, "默认", new Vector2(80, 0), type);
                Draw_ValueButton(rect, from, dir_index, "起始", new Vector2(80, 25), type);
                Draw_ValueButton(rect, end, dir_index, "结束", new Vector2(80, 50), type);
            }
            else
            {
                Draw_ValueButton(rect, end, dir_index, "结束", new Vector2(80, 0), type);
            }
        }
        /// <summary>
        /// 动画值标记按钮控件绘制
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="prop"></param>
        /// <param name="dir_index"></param>
        /// <param name="title"></param>
        /// <param name="offset"></param>
        /// <param name="type"></param>
        private void Draw_ValueButton(Rect rect, SerializedProperty prop, int dir_index, string title, Vector2 offset, TweenNodeType type = TweenNodeType.位移)
        {
            Rect re = new Rect(rect.x + offset.x, rect.y + offset.y, rect.width - (5 + offset.x), LineHeight);

            #region 根据类型显示属性框，根据不同值类型显示不同的值控件
            if (prop.type == "Vector4")
            {
                prop.vector4Value = Editor_XHud_GUI.Gui_InputField_Vector4(re, prop.vector4Value);
                prop.serializedObject.ApplyModifiedProperties();
            }
            else if (prop.type == "Vector3")
            {
                prop.vector3Value = Editor_XHud_GUI.Gui_InputField_Vector3(re, prop.vector3Value);
                prop.serializedObject.ApplyModifiedProperties();
            }
            else if (prop.type == "Vector2")
            {
                prop.vector2Value = Editor_XHud_GUI.Gui_InputField_Vector2(re, prop.vector2Value);
                prop.serializedObject.ApplyModifiedProperties();
            }
            else if (prop.type == "Color")
                Editor_XHud_GUI.Gui_Property_Field(re, "", prop, 0, 0);
            else if (prop.type == "float")
                Editor_XHud_GUI.Gui_Property_Field(re, "", prop, 0, 0);
            else if (prop.type == "int")
                Editor_XHud_GUI.Gui_Property_Field(re, "", prop, 0, 0);
            else if (prop.type == "string")
                Editor_XHud_GUI.Gui_Property_Field(re, "", prop, 0, 0);
            #endregion

            // 根据不同的动向模式来着色圆点按钮
            ValueButtonStyle(dir_index, title);
            re.Set(rect.x + 5, rect.y + 3 + offset.y, 12, 12);

            #region 按下圆点按钮：根据鼠标按下按键类型进行 - 获取值 / 设置值 / 重置值
            if (Editor_XHud_GUI.Gui_Button(re, anim_dot, anim_dot_dark, true, "", "", Color.white))
            {
                #region 获取组件
                // Controller 提前获取的  -  mod_RectTransform
                RectTransform m_recttransform = BaseScript.controller.mod_Rect;
                // Controller 提前获取的  -  mod_CanvasGroup
                CanvasGroup m_canvasgroup = BaseScript.controller.mod_CanvasGroup;
                // Controller 提前获取的  -  mod_Text
                XHud_Module_Text m_Text = BaseScript.controller.mod_Text;
                // Controller 提前获取的  -  mod_TmpText
                XHud_Module_TmpText m_TmpText = BaseScript.controller.mod_TmpText;
                // Controller 提前获取的  -  mod_Image
                Image m_image = BaseScript.controller.mod_Image;
                #endregion

                #region  根据鼠标按下按钮类型执行相应动作
                Event eve = Event.current;
                // ( 鼠标左键 0，鼠标右键 1，鼠标中键 2 )
                switch (eve.button)
                {
                    #region 鼠标左键：点击
                    case 0:
                        switch (type)
                        {
                            case TweenNodeType.位移:
                                prop.vector3Value = m_recttransform.anchoredPosition3D;
                                break;
                            case TweenNodeType.旋转:
                                prop.vector3Value = m_recttransform.localEulerAngles;
                                break;
                            case TweenNodeType.缩放:
                                prop.vector3Value = m_recttransform.localScale;
                                break;
                            case TweenNodeType.颜色:
                                // 根据 Controller 提前获取的 类型进行转换 Graphic
                                Graphic gc = BaseScript.controller.RecognizeType();

                                prop.colorValue = gc.color;
                                break;
                            case TweenNodeType.淡化:
                                prop.floatValue = m_canvasgroup.alpha;
                                break;
                            case TweenNodeType.打字机:
                                if (m_Text != null)
                                {
                                    prop.stringValue = m_Text.text;
                                }
                                else if (m_TmpText != null)
                                {
                                    prop.stringValue = m_TmpText.text;
                                }
                                break;
                            case TweenNodeType.图像填充:
                                prop.floatValue = m_image.fillAmount;
                                break;
                            case TweenNodeType.尺寸:
                                prop.vector2Value = m_recttransform.sizeDelta;
                                break;
                        }
                        break;
                    #endregion
                    #region 鼠标右键：点击
                    case 1:
                        switch (type)
                        {
                            case TweenNodeType.位移:
                                Undo.RecordObject(BaseScript.controller.mod_Rect, "undotransform-position");
                                m_recttransform.anchoredPosition3D = prop.vector3Value;
                                break;
                            case TweenNodeType.旋转:
                                Undo.RecordObject(BaseScript.controller.mod_Rect, "undotransform-eulerangle");
                                m_recttransform.localEulerAngles = prop.vector3Value;
                                break;
                            case TweenNodeType.缩放:
                                Undo.RecordObject(BaseScript.controller.mod_Rect, "undotransform-localscale");
                                m_recttransform.localScale = prop.vector3Value;
                                break;
                            case TweenNodeType.颜色:
                                // 根据 Controller 提前获取的 类型进行转换 Graphic
                                Graphic gc = BaseScript.controller.RecognizeType();

                                Undo.RecordObject(BaseScript.controller.pt_Painting, "undocolor-origin");
                                ModuleType x_Type = BaseScript.controller.GetModuleType();
                                if (x_Type == ModuleType.Image)
                                    BaseScript.controller.pt_Painting.OriginalColor = prop.colorValue;
                                else if (x_Type == ModuleType.RawImage)
                                    BaseScript.controller.pt_Painting.OriginalColor = prop.colorValue;
                                gc.color = prop.colorValue;
                                break;
                            case TweenNodeType.淡化:
                                Undo.RecordObject(BaseScript.controller.mod_CanvasGroup, "undoAlpha-origin");
                                m_canvasgroup.alpha = prop.floatValue;
                                break;
                            case TweenNodeType.打字机:
                                if (m_Text != null)
                                {
                                    Undo.RecordObject(m_Text, "undoText-origin");
                                    m_Text.text = prop.stringValue;
                                }
                                else if (m_TmpText != null)
                                {
                                    Undo.RecordObject(m_TmpText, "undoTmpText-origin");
                                    m_TmpText.text = prop.stringValue;
                                }
                                Repaint();
                                SceneView.RepaintAll();
                                break;
                            case TweenNodeType.图像填充:
                                Undo.RecordObject(BaseScript.controller.mod_Image, "undofill-end");
                                m_image.fillAmount = prop.floatValue;
                                break;
                            case TweenNodeType.尺寸:
                                Undo.RecordObject(BaseScript.controller.mod_Rect, "undosize-origin");
                                m_recttransform.sizeDelta = prop.vector2Value;
                                break;
                        }
                        break;
                    #endregion
                    #region 鼠标中键：点击
                    case 2:
                        switch (type)
                        {
                            case TweenNodeType.位移:
                                prop.vector3Value = Vector3.zero;
                                break;
                            case TweenNodeType.旋转:
                                prop.vector3Value = Vector3.zero;
                                break;
                            case TweenNodeType.缩放:
                                prop.vector3Value = Vector3.zero;
                                break;
                            case TweenNodeType.颜色:
                                prop.colorValue = Color.white;
                                break;
                            case TweenNodeType.淡化:
                                prop.floatValue = 0;
                                break;
                            case TweenNodeType.打字机:
                                prop.stringValue = null;
                                break;
                            case TweenNodeType.图像填充:
                                prop.floatValue = 0;
                                break;
                            case TweenNodeType.尺寸:
                                prop.vector2Value = Vector2.zero;
                                break;
                        }
                        break;
                        #endregion
                }
                #endregion

                // 修改完后应用序列化值并保存图元特性
                prop.serializedObject.ApplyModifiedProperties();
                BaseScript.controller.pt_Feature.PrimitiveFeature_Save();

                eve.Use();
                return;
            }
            #endregion

            GUI.color = Color.white;
            re.Set(rect.x + 28, rect.y + 2 + offset.y, 48, 15);
            Editor_XHud_GUI.Gui_Labelfield(re, title, HudFilled.无, HudColor.无, Color.white * 0.88f, TextAnchor.MiddleLeft, Vector2.zero, 12, Font_Light);
        }
        /// <summary>
        /// 动画值标记按钮之间的关系示意连线
        /// </summary>
        /// <param name="valuepanel_rect"></param>
        /// <param name="onlyToEnd"></param>
        /// <param name="dir_index"></param>
        /// <param name="original"></param>
        /// <param name="from"></param>
        /// <param name="end"></param>
        /// <param name="isCustomValue"></param>
        private void Draw_DirectionButton(Rect valuepanel_rect, bool onlyToEnd, int dir_index, SerializedProperty original, SerializedProperty from, SerializedProperty end, bool isCustomValue = true)
        {
            Rect rect = valuepanel_rect;
            float Added = -25;

            #region 动向方向连线按钮图标设置
            // 动向方向连线图标
            Texture2D arw_released = null;
            Texture2D arw_press = null;

            float offset = 0;

            // 动向：起始 - 默认
            if (dir_index == 0)
            {
                offset = 31 + (!isCustomValue ? Added : 0);
                arw_released = anim_dir_bak;
                arw_press = anim_dir_bak_p;
            }
            // 动向：默认 - 结束
            if (dir_index == 1)
            {
                offset = 33 + (!isCustomValue ? Added : 0);
                arw_released = anim_dir_for_long;
                arw_press = anim_dir_for_long_p;
            }
            // 动向：起始 - 结束
            if (dir_index == 2)
            {
                offset = 59 + (!isCustomValue ? Added : 0);
                arw_released = anim_dir_for;
                arw_press = anim_dir_for_p;
            }
            #endregion

            #region 点击动向方向连线按钮，根据动向模式（进行数值的交换）
            rect.Set(valuepanel_rect.x - 10, rect.y + offset, arw_released.width, arw_released.height);
            if (Editor_XHud_GUI.Gui_Button(rect, arw_released, arw_press, true, "", "", Color.white))
            {
                DirectionButton_SwapValues(from, end, original, dir_index);
            }
            #endregion
        }
        /// <summary>
        /// 动画值标记按钮颜色
        /// </summary>
        /// <param name="dir_index"></param>
        /// <param name="state"></param>
        private void ValueButtonStyle(int dir_index, string state)
        {
            switch (state)
            {
                case "当前":
                    GUI.color = Editor_XHud_GUI.GetColor(HudColor.神秘紫);
                    break;
                case "默认":
                    if (dir_index == 0) { GUI.color = Editor_XHud_GUI.GetColor(HudColor.魅力红); }
                    if (dir_index == 1) { GUI.color = Editor_XHud_GUI.GetColor(HudColor.魅力红); }
                    if (dir_index == 2) { GUI.color = Editor_XHud_GUI.GetColor(HudColor.深空灰); }
                    break;
                case "起始":
                    if (dir_index == 0) { GUI.color = Editor_XHud_GUI.GetColor(HudColor.工业蓝); }
                    if (dir_index == 1) { GUI.color = Editor_XHud_GUI.GetColor(HudColor.深空灰); }
                    if (dir_index == 2) { GUI.color = Editor_XHud_GUI.GetColor(HudColor.工业蓝); }
                    break;
                case "结束":
                    if (dir_index == 0) { GUI.color = Editor_XHud_GUI.GetColor(HudColor.深空灰); }
                    if (dir_index == 1) { GUI.color = Editor_XHud_GUI.GetColor(HudColor.警示黄); }
                    if (dir_index == 2) { GUI.color = Editor_XHud_GUI.GetColor(HudColor.警示黄); }
                    break;
            }
        }
        /// <summary>
        /// 根据动向交换两个 SerializedProperty 的值
        /// </summary>
        /// <param name="from">起始属性</param>
        /// <param name="end">结束属性</param>
        /// <param name="original">原始属性</param>
        /// <param name="dirIndex">动向索引：0=起始-默认，1=默认-结束，2=起始-结束</param>
        private void DirectionButton_SwapValues(SerializedProperty from, SerializedProperty end, SerializedProperty original, int dirIndex)
        {
            // 确定要交换的两个属性
            SerializedProperty a, b;

            switch (dirIndex)
            {
                case 0: // 起始 → 默认：交换 from 和 original
                    a = from;
                    b = original;
                    break;
                case 1: // 默认 → 结束：交换 end 和 original
                    a = end;
                    b = original;
                    break;
                case 2: // 起始 → 结束：交换 from 和 end
                    a = from;
                    b = end;
                    break;
                default:
                    return;
            }

            // 根据类型交换值
            switch (a.type)
            {
                case "Color":
                    (a.colorValue, b.colorValue) = (b.colorValue, a.colorValue);
                    break;
                case "Vector4":
                    (a.vector4Value, b.vector4Value) = (b.vector4Value, a.vector4Value);
                    break;
                case "Vector3":
                    (a.vector3Value, b.vector3Value) = (b.vector3Value, a.vector3Value);
                    break;
                case "Vector2":
                    (a.vector2Value, b.vector2Value) = (b.vector2Value, a.vector2Value);
                    break;
                case "float":
                    (a.floatValue, b.floatValue) = (b.floatValue, a.floatValue);
                    break;
                case "int":
                    (a.intValue, b.intValue) = (b.intValue, a.intValue);
                    break;
                case "string":
                    (a.stringValue, b.stringValue) = (b.stringValue, a.stringValue);
                    break;
            }
        }
        #endregion

        #region 解析TweenerNode

        private TweenNode TweenNodeConvert_From_SerialProperty(SerializedProperty args)
        {
            TweenNode node = new TweenNode();
            node.Indicator = args.FindPropertyRelative("Indicator").stringValue;
            node.ID = args.FindPropertyRelative("ID").intValue;
            node.Enabled = args.FindPropertyRelative("Enabled").boolValue;
            node.Type = (TweenNodeType)args.FindPropertyRelative("Type").enumValueIndex;
            node.Timings = args.FindPropertyRelative("Timings").stringValue;
            node.Duration = args.FindPropertyRelative("Duration").floatValue;
            node.Progress = args.FindPropertyRelative("Progress").floatValue;
            node.Delay = args.FindPropertyRelative("Delay").floatValue;
            node.Ease = (EaseMode)args.FindPropertyRelative("Ease").enumValueIndex;
            node.AnimationCurveName = args.FindPropertyRelative("AnimationCurveName").stringValue;
            node.Curve = args.FindPropertyRelative("Curve").animationCurveValue;
            node.Original_Int = args.FindPropertyRelative("Original_Int").intValue;
            node.Original_Float = args.FindPropertyRelative("Original_Float").floatValue;
            node.Original_Vector2 = args.FindPropertyRelative("Original_Vector2").vector2Value;
            node.Original_Vector3 = args.FindPropertyRelative("Original_Vector3").vector3Value;
            node.Original_Vector4 = args.FindPropertyRelative("Original_Vector4").vector4Value;
            node.Original_Color = args.FindPropertyRelative("Original_Color").colorValue;
            node.Original_String = args.FindPropertyRelative("Original_String").stringValue;
            node.ActivateFrom = args.FindPropertyRelative("ActivateFrom").boolValue;
            node.From_Int = args.FindPropertyRelative("From_Int").intValue;
            node.From_Float = args.FindPropertyRelative("From_Float").floatValue;
            node.From_Vector2 = args.FindPropertyRelative("From_Vector2").vector2Value;
            node.From_Vector3 = args.FindPropertyRelative("From_Vector3").vector3Value;
            node.From_Vector4 = args.FindPropertyRelative("From_Vector4").vector4Value;
            node.From_Color = args.FindPropertyRelative("From_Color").colorValue;
            node.From_String = args.FindPropertyRelative("From_String").stringValue;
            node.ActivateEnd = args.FindPropertyRelative("ActivateEnd").boolValue;
            node.ActivateOnlyToEnd = args.FindPropertyRelative("ActivateOnlyToEnd").boolValue;
            node.End_Int = args.FindPropertyRelative("End_Int").intValue;
            node.End_Float = args.FindPropertyRelative("End_Float").floatValue;
            node.End_Vector2 = args.FindPropertyRelative("End_Vector2").vector2Value;
            node.End_Vector3 = args.FindPropertyRelative("End_Vector3").vector3Value;
            node.End_Vector4 = args.FindPropertyRelative("End_Vector4").vector4Value;
            node.End_Color = args.FindPropertyRelative("End_Color").colorValue;
            node.End_String = args.FindPropertyRelative("End_String").stringValue;
            node.RotateMode = (XTweenRotationMode)args.FindPropertyRelative("RotateMode").enumValueIndex;
            node.LoopType = (XTween_LoopType)args.FindPropertyRelative("LoopType").enumValueIndex;
            node.LoopCount = args.FindPropertyRelative("LoopCount").intValue;
            node.IsFold = args.FindPropertyRelative("IsFold").boolValue;
            SerializedProperty sp_TweenSounds = args.FindPropertyRelative("TweenSounds");

            node.TweenSounds = new List<TweenSound>();

            for (int i = 0; i < sp_TweenSounds.arraySize; i++)
            {
                SerializedProperty sp_sounditem = sp_TweenSounds.GetArrayElementAtIndex(i);

                TweenSound s_node = new TweenSound();
                s_node.IsPlayed = sp_sounditem.FindPropertyRelative("IsPlayed").boolValue;
                s_node.Path = sp_sounditem.FindPropertyRelative("Path").stringValue;
                s_node.Percentage = sp_sounditem.FindPropertyRelative("Percentage").floatValue;
                s_node.Sound = (AudioClip)sp_sounditem.FindPropertyRelative("Sound").objectReferenceValue;
                s_node.Volume = sp_sounditem.FindPropertyRelative("Volume").floatValue;
                s_node.MaxPitch = sp_sounditem.FindPropertyRelative("MaxPitch").floatValue;
                s_node.MinPitch = sp_sounditem.FindPropertyRelative("MinPitch").floatValue;
                node.TweenSounds.Add(s_node);
            }

            return node;
        }

        private void TweenNodeConvert_From_Class(SerializedProperty property, TweenNode node)
        {
            property.FindPropertyRelative("Indicator").stringValue = node.Indicator;
            property.FindPropertyRelative("ID").intValue = node.ID;
            property.FindPropertyRelative("Enabled").boolValue = node.Enabled;
            property.FindPropertyRelative("Type").enumValueIndex = (int)node.Type;
            property.FindPropertyRelative("Timings").stringValue = node.Timings;
            property.FindPropertyRelative("Duration").floatValue = node.Duration;
            property.FindPropertyRelative("Progress").floatValue = node.Progress;
            property.FindPropertyRelative("Delay").floatValue = node.Delay;
            property.FindPropertyRelative("Ease").enumValueIndex = (int)node.Ease;
            property.FindPropertyRelative("AnimationCurveName").stringValue = node.AnimationCurveName;
            property.FindPropertyRelative("Curve").animationCurveValue = node.Curve;
            property.FindPropertyRelative("Original_Int").intValue = node.Original_Int;
            property.FindPropertyRelative("Original_Float").floatValue = node.Original_Float;
            property.FindPropertyRelative("Original_Vector2").vector2Value = node.Original_Vector2;
            property.FindPropertyRelative("Original_Vector3").vector3Value = node.Original_Vector3;
            property.FindPropertyRelative("Original_Vector4").vector4Value = node.Original_Vector4;
            property.FindPropertyRelative("Original_Color").colorValue = node.Original_Color;
            property.FindPropertyRelative("Original_String").stringValue = node.Original_String;
            property.FindPropertyRelative("ActivateFrom").boolValue = node.ActivateFrom;
            property.FindPropertyRelative("ActivateOnlyToEnd").boolValue = node.ActivateOnlyToEnd;
            property.FindPropertyRelative("From_Int").intValue = node.From_Int;
            property.FindPropertyRelative("From_Float").floatValue = node.From_Float;
            property.FindPropertyRelative("From_Vector2").vector2Value = node.From_Vector2;
            property.FindPropertyRelative("From_Vector3").vector3Value = node.From_Vector3;
            property.FindPropertyRelative("From_Vector4").vector4Value = node.From_Vector4;
            property.FindPropertyRelative("From_Color").colorValue = node.From_Color;
            property.FindPropertyRelative("From_String").stringValue = node.From_String;
            property.FindPropertyRelative("ActivateEnd").boolValue = node.ActivateEnd;
            property.FindPropertyRelative("End_Int").intValue = node.End_Int;
            property.FindPropertyRelative("End_Float").floatValue = node.End_Float;
            property.FindPropertyRelative("End_Vector2").vector2Value = node.End_Vector2;
            property.FindPropertyRelative("End_Vector3").vector3Value = node.End_Vector3;
            property.FindPropertyRelative("End_Vector4").vector4Value = node.End_Vector4;
            property.FindPropertyRelative("End_Color").colorValue = node.End_Color;
            property.FindPropertyRelative("End_String").stringValue = node.End_String;
            property.FindPropertyRelative("RotateMode").enumValueIndex = (int)node.RotateMode;
            property.FindPropertyRelative("LoopType").enumValueIndex = (int)node.LoopType;
            property.FindPropertyRelative("LoopCount").intValue = node.LoopCount;
            property.FindPropertyRelative("IsFold").boolValue = node.IsFold;
            SerializedProperty sp_TweenSounds = property.FindPropertyRelative("TweenSounds");

            for (int i = 0; i < sp_TweenSounds.arraySize; i++)
            {
                SerializedProperty sp_sounditem = sp_TweenSounds.GetArrayElementAtIndex(i);

                sp_sounditem.FindPropertyRelative("IsPlayed").boolValue = node.TweenSounds[i].IsPlayed;
                sp_sounditem.FindPropertyRelative("Path").stringValue = node.TweenSounds[i].Path;
                sp_sounditem.FindPropertyRelative("Percentage").floatValue = node.TweenSounds[i].Percentage;
                sp_sounditem.FindPropertyRelative("Sound").objectReferenceValue = node.TweenSounds[i].Sound;
                sp_sounditem.FindPropertyRelative("Volume").floatValue = node.TweenSounds[i].Volume;
                sp_sounditem.FindPropertyRelative("MaxPitch").floatValue = node.TweenSounds[i].MaxPitch;
                sp_sounditem.FindPropertyRelative("MinPitch").floatValue = node.TweenSounds[i].MinPitch;

                sp_sounditem.serializedObject.ApplyModifiedProperties();
            }
            sp_TweenSounds.serializedObject.ApplyModifiedProperties();
            property.serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region 辅助
        /// <summary>
        /// 获取动画器中是否存在循环模式
        /// </summary>
        /// <returns></returns>
        public bool HasLoopMode()
        {
            bool hasLoop = false;
            for (int i = 0; i < sp_PrimitiveTweenNodes.arraySize; i++)
            {
                SerializedProperty sp_tween_loopcount = sp_PrimitiveTweenNodes.GetArrayElementAtIndex(i).FindPropertyRelative("LoopCount");
                if (sp_tween_loopcount.intValue == -1)
                {
                    hasLoop = true;
                    break;
                }
            }
            return hasLoop;
        }
        /// <summary>
        /// 获取最新的动画耗时信息
        /// </summary>
        private void AnimationsTimerStatistic()
        {
            if (!Targets_Selected())
            {
                BaseScript.TweenNode_GetTimers();
            }
            else
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    SelectedObjects[i].TweenNode_GetTimers();
                }
            }
        }
        #endregion

        #region 动画预览
        /// <summary>
        /// 创建收集图元动画器的动画节点列表所有动画
        /// </summary>
        /// <param name="tweener"></param>
        /// <returns></returns>
        private XTween_Interface[] Preview_PrimitiveTweens_Collected(XHud_Module_Primitive_Tween tweener, string tim)
        {
            List<XTween_Interface> tweens = new List<XTween_Interface>();
            for (int i = 0; i < tweener.PrimitiveTweenNodes.Count; i++)
            {
                if (!tweener.PrimitiveTweenNodes[i].Enabled)
                    continue;
                if (tweener.PrimitiveTweenNodes[i].Timings != tim)
                    continue;
                XTween_Interface tween = tweener.Tween_Create(tweener.PrimitiveTweenNodes[i], tweener.GlobalDuration * HudManager.DurationMultiply);

                if (tween != null)
                    tweens.Add(tween);
            }

            return tweens.ToArray();
        }
        /// <summary>
        /// 创建收集图元动画器的动画节点列表所有动画
        /// </summary>
        /// <param name="tweener"></param>
        /// <returns></returns>
        private string[] Preview_PrimitiveTweens_CollectedTimings(XHud_Module_Primitive_Tween tweener)
        {
            List<string> tims = new List<string>();
            for (int i = 0; i < tweener.PrimitiveTweenNodes.Count; i++)
            {
                if (!tweener.PrimitiveTweenNodes[i].Enabled)
                    continue;

                string timing = tweener.PrimitiveTweenNodes[i].Timings;
                if (!tims.Contains(timing))  // 添加前检查是否已存在
                    tims.Add(timing);
            }

            PreviewTimings = tims.ToArray();
            return tims.ToArray();
        }
        /// <summary>
        /// 杀死并清空图元动画器的动画节点列表所有已生成的 XTweenInterface 动画
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        private void Preview_PrimitiveTweens_KillAndClear(List<TweenNode> nodes)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                TweenNode node = nodes[i];
                if (node == null || !node.Enabled)
                    continue;

                node.Tweener?.Kill();  // 使用 ?. 简化
                node.Tweener = null;
                node.Progress = 0;
            }
        }

        /// <summary>
        /// 预览动画
        /// </summary>
        private void Preview_PrimitiveTweens_Play()
        {
            if (sp_PrimitiveTweenNodes == null || sp_PrimitiveTweenNodes.arraySize <= 0)
                return;

            //先停止之前的动画预览
            Preview_PrimitiveTweens_Stop();

            //将动画预览中的开关打开
            sp_TweenIsPreviewing.boolValue = true;
            sp_TweenIsPreviewing.serializedObject.ApplyModifiedProperties();

            XTween_Interface[] tweens = null;

            if (Targets_Selected())
            {
                List<XTween_Interface> mo = new List<XTween_Interface>();
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    XTween_Interface[] sel_tweens = Preview_PrimitiveTweens_Collected(SelectedObjects[i], SelectedObjects[i].PreviewTiming);
                    for (int s = 0; s < sel_tweens.Length; s++)
                    {
                        mo.Add(sel_tweens[s]);
                    }
                }
                tweens = mo.ToArray();
                // 预览收集到的有效的音效
                Preview_PrimitiveTweens_Sounds(sp_PreviewTiming.stringValue, SelectedObjects);
            }
            else
            {
                tweens = Preview_PrimitiveTweens_Collected(BaseScript, sp_PreviewTiming.stringValue);
                // 预览收集到的有效的音效
                Preview_PrimitiveTweens_Sounds(sp_PreviewTiming.stringValue, BaseScript);
            }


            // 使用XTween预览器预览收集到的有效的动画
            XTween_Preview_Start(tweens);
        }
        /// <summary>
        /// 停止预览动画
        /// </summary>
        private void Preview_PrimitiveTweens_Stop(bool LoadOriginalState = true)
        {
            if (target != null)
            {
                sp_TweenIsPreviewing.boolValue = false;
                sp_TweenIsPreviewing.serializedObject.ApplyModifiedProperties();

                XTween_Preview_Kill();

                if (Targets_Selected())
                {
                    for (int i = 0; i < SelectedObjects.Length; i++)
                    {
                        if (LoadOriginalState && SelectedObjects[i] != null)
                            SelectedObjects[i].controller.pt_Feature.PrimitiveFeature_Load();
                    }
                }
                else
                {
                    if (LoadOriginalState && target != null)
                        BaseScript.controller.pt_Feature.PrimitiveFeature_Load();
                }
            }
        }

        //------------------------------------------------------------------------------------

        /// <summary>
        /// 动画预览 - 播放
        /// </summary>
        /// <param name="tweens">传入需要预览的动画，但前提是动画已创建，如果是空的则会导致预览异常</param>
        public void XTween_Preview_Start(XTween_Interface[] tweens)
        {
            if (Application.isPlaying)
                return;

            Editor_XTween_Previewer.AutoKillWithDuration = false;
            // 预览动画杀死后自动清空预览器的列表 - 状态根据元素脚本的开关
            Editor_XTween_Previewer.AfterKillClear = true;
            // 预览动画杀死前将动画目标的属性倒退 - 状态根据元素脚本的开关
            Editor_XTween_Previewer.BeforeKillRewind = true;

            for (int i = 0; i < tweens.Length; i++)
            {
                Editor_XTween_Previewer.Append(tweens[i]);
            }

            Editor_XTween_Previewer.Play(null);
        }
        /// <summary>
        ///  动画预览 - 杀死
        /// </summary>
        private void XTween_Preview_Kill()
        {
            if (Application.isPlaying)
                return;

            // 预览开关状态复位
            sp_TweenIsPreviewing.boolValue = false;
            sp_TweenIsPreviewing.serializedObject.ApplyModifiedProperties();

            if (Targets_Selected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    XHud_Module_Primitive_Tween tween = SelectedObjects[i];
                    Preview_PrimitiveTweens_KillAndClear(tween.PrimitiveTweenNodes);
                }
            }
            else
            {
                Preview_PrimitiveTweens_KillAndClear(BaseScript.PrimitiveTweenNodes);
            }

            // 预览器执行动作：杀死动画
            Editor_XTween_Previewer.Kill(true, true, () =>
            {
                //BaseScript.CurrentTweener = null;
            });
        }
        /// <summary>
        /// 预览倒退重置
        /// </summary>
        private void XTween_Preview_Rewind()
        {
            Editor_XTween_Previewer.Rewind();
        }

        //------------------------------------------------------------------------------------


        #endregion

        #region 音效预览实现
        /// <summary>
        /// 预览图元动画器身上挂载的所有音效
        /// </summary>
        /// <param name="Timings">匹配时机</param>
        /// <param name="tweener">音效节点列表对象</param>
        private void Preview_PrimitiveTweens_Sounds(string Timings, XHud_Module_Primitive_Tween tweener)
        {
            // 循环生成音效，但是音效的延迟时间由以下条件决定：
            // 音效本身设置的百分比参数 x 动画节点的基础耗时 x 动画器的全局耗时 + 动画节点的延迟时间
            for (int i = 0; i < tweener.PrimitiveTweenNodes.Count; i++)
            {
                TweenNode node = tweener.PrimitiveTweenNodes[i];

                if (!node.Enabled)
                    continue;

                // 判断该音效的播放时机是否匹配，如果不匹配则跳过
                if (Timings != node.Timings)
                    continue;

                for (int s = 0; s < node.TweenSounds.Count; s++)
                {
                    TweenSound tsound = node.TweenSounds[s];

                    float x_vol = tsound.Volume;
                    float x_pit_min = tsound.MinPitch;
                    float x_pit_max = tsound.MaxPitch;
                    float x_delay = ((tsound.Percentage * 0.01f) * node.Duration * tweener.GlobalDuration) + node.Delay;
                    bool x_userandom = !(tsound.MinPitch == 1 && tsound.MaxPitch == 1);
                    string x_soundname = tsound.Sound.name;

                    AudioClip x_clip = HudManager.Hud_Sounds.SoundLibrary_GetSound(x_soundname);
                    Preview_PrimitiveTweens_SoundCoroutineList_Stop.Add(XCoroutineUtility.xec_StartCoroutineOwnerless(Preview_PrimitiveTweens_Sound_Play(x_vol, x_pit_min, x_pit_max, x_userandom, x_clip, x_delay)));
                }
            }
        }
        /// <summary>
        /// 预览图元动画器身上挂载的所有音效
        /// </summary>
        /// <param name="Timings">匹配时机</param>
        /// <param name="tweener">图元动画数组</param>
        private void Preview_PrimitiveTweens_Sounds(string Timings, XHud_Module_Primitive_Tween[] tweener)
        {
            // 循环生成音效，但是音效的延迟时间由以下条件决定：
            // 音效本身设置的百分比参数 x 动画节点的基础耗时 x 动画器的全局耗时 + 动画节点的延迟时间
            for (int i = 0; i < tweener.Length; i++)
            {
                XHud_Module_Primitive_Tween tween = tweener[i];
                for (int s = 0; s < tween.PrimitiveTweenNodes.Count; s++)
                {
                    TweenNode node = tween.PrimitiveTweenNodes[s];
                    if (!node.Enabled)
                        continue;
                    if (node.Timings != Timings)
                        continue;

                    for (int k = 0; k < node.TweenSounds.Count; k++)
                    {
                        TweenSound tsound = node.TweenSounds[k];

                        float x_vol = tsound.Volume;
                        float x_pit_min = tsound.MinPitch;
                        float x_pit_max = tsound.MaxPitch;
                        float x_delay = (tsound.Percentage * node.Duration * tween.GlobalDuration) + node.Delay;
                        bool x_userandom = !(tsound.MinPitch == 1 && tsound.MaxPitch == 1);
                        string x_soundname = tsound.Sound.name;

                        AudioClip x_clip = HudManager.Hud_Sounds.SoundLibrary_GetSound(x_soundname);
                        Preview_PrimitiveTweens_SoundCoroutineList_Stop.Add(XCoroutineUtility.xec_StartCoroutineOwnerless(Preview_PrimitiveTweens_Sound_Play(x_vol, x_pit_min, x_pit_max, x_userandom, x_clip, x_delay)));
                    }
                }
            }
        }
        /// <summary>
        ///  PrimitiveTweens_Sound 音效预览
        /// </summary>
        IEnumerator Preview_PrimitiveTweens_Sound_Play(float sp_vol, float sp_pitch_min, float sp_pitch_max, bool sp_userandom, AudioClip clip, float delay)
        {
            yield return new XCoroutineWaitForSeconds(delay);
            Preview_PrimitiveTweens_SoundList.Add(Preview_PrimitiveTweens_Sound_Create(sp_vol, sp_pitch_min, sp_pitch_max, sp_userandom, clip));
            AudioSource au = Preview_PrimitiveTweens_SoundList[Preview_PrimitiveTweens_SoundList.Count - 1];
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
        ///  停止协程列表 - PrimitiveTweens_Sound 音效预览播放 / 停止播放并清空 PrimitiveTweens_Sound 预览列表与生成的音效物体
        /// </summary>
        private void Preview_PrimitiveTweens_Sound_Stop()
        {
            for (int i = 0; i < Preview_PrimitiveTweens_SoundCoroutineList_Stop.Count; i++)
            {
                if (Preview_PrimitiveTweens_SoundCoroutineList_Stop[i] != null)
                    XCoroutineUtility.xec_StopCoroutine(Preview_PrimitiveTweens_SoundCoroutineList_Stop[i]);
            }
            Preview_PrimitiveTweens_SoundCoroutineList_Stop.Clear();

            if (Preview_PrimitiveTweens_SoundList != null)
            {
                for (int i = 0; i < Preview_PrimitiveTweens_SoundList.Count; i++)
                {
                    if (Preview_PrimitiveTweens_SoundList[i] != null)
                    {
                        Preview_PrimitiveTweens_SoundList[i].Stop();
                        DestroyImmediate(Preview_PrimitiveTweens_SoundList[i].gameObject, true);
                        Preview_PrimitiveTweens_SoundList[i] = null;
                    }
                }
                Preview_PrimitiveTweens_SoundList.Clear();
            }

            SceneView.RepaintAll();
        }
        /// <summary>
        /// 创建 PrimitiveTweens_Sound 预览指定声音
        /// </summary>
        /// <param name="sp_vol"></param>
        /// <param name="sp_pitch_min"></param>
        /// <param name="sp_pitch_max"></param>
        /// <param name="sp_userandom"></param>
        /// <param name="clip"></param>
        /// <returns></returns>
        public AudioSource Preview_PrimitiveTweens_Sound_Create(float sp_vol, float sp_pitch_min, float sp_pitch_max, bool sp_userandom, AudioClip clip)
        {
            GameObject obj = new GameObject();
            obj.name = "PrimitiveTweens_Sound_Previewer-" + "[" + clip.length.ToString("F2") + " s]-" + "[" + clip.channels + " ch]-" + "[" + clip.frequency + " hz]";
            AudioSource au = obj.AddComponent<AudioSource>();
            au.clip = clip;
            au.volume = sp_vol;
            if (sp_userandom)
            {
                au.pitch = Random.Range(sp_pitch_min, sp_pitch_max);
            }
            else
            {
                au.pitch = 1.0f;
            }
            au.Play();
            XHud_AudioStoper sp = au.gameObject.AddComponent<XHud_AudioStoper>();
            sp.SetAudioSource(au);
            return au;
        }
        #endregion
    }
}
