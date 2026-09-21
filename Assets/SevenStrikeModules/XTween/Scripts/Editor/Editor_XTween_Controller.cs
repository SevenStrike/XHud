/*
 * ============================================================================
 * ⚠️ 版权声明（禁止删除、禁止修改、衍生作品必须保留此注释）⚠️
 * ============================================================================
 * 版权声明 Copyright (C) 2025-Present Nanjing SevenStrike Media Co., Ltd.
 * 中文名称：南京塞维斯传媒有限公司
 * 英文名称：SevenStrikeMedia
 * 项目作者：徐寅智
 * 项目名称：XTween - Unity 高性能动画架构插件
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
namespace SevenStrikeModules.XTween.Editor
{
    using SevenStrikeModules.XGUI.Editor;
    using SevenStrikeModules.XGUI.Runtime;
    using System;
    using UnityEditor;
    using UnityEngine;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(XTween_Controller))]
    public class Editor_XTween_Controller : Editor
    {
        private XTween_Controller BaseScript;

        #region 序列化属性
        /// <summary>
        /// 序列化属性
        /// </summary>
        private SerializedProperty
            sp_Duration,
            sp_Delay,
            sp_UseRandomDelay,
            sp_RandomDelay,
            sp_EaseMode,
            sp_UseCurve,
            sp_Curve,
            sp_LoopCount,
            sp_LoopDelay,
            sp_LoopType,
            sp_IsFromMode,
            sp_IsRelative,
            sp_IsAutoKill,
            sp_EndValue_String,
            sp_EndValue_Int,
            sp_EndValue_Float,
            sp_EndValue_Vector2,
            sp_EndValue_Vector3,
            sp_EndValue_Vector4,
            sp_EndValue_Color,
            sp_EndValue_Quaternion,
            sp_FromValue_Int,
            sp_FromValue_Float,
            sp_FromValue_String,
            sp_FromValue_Vector2,
            sp_FromValue_Vector3,
            sp_FromValue_Vector4,
            sp_FromValue_Color,
            sp_FromValue_Quaternion,
            sp_Target_PathTool,
            sp_index_TweenTypes,
            sp_index_TweenTypes_Positions,
            sp_index_TweenTypes_Rotations,
            sp_index_TweenTypes_Alphas,
            sp_index_TweenTypes_Shakes,
            sp_index_TweenTypes_Text,
            sp_index_TweenTypes_To,
            sp_Target_RectTransform,
            sp_Target_Image,
            sp_Target_CanvasGroup,
            sp_Target_Text,
            sp_Target_Int,
            sp_Target_Float,
            sp_Target_String,
            sp_Target_Vector2,
            sp_Target_Vector3,
            sp_Target_Vector4,
            sp_Target_Color,
            sp_index_AutoKillPreviewTweens,
            sp_index_RewindPreviewTweensWithKill,
            sp_index_ClearPreviewTweensWithKill,
            sp_keyControl_Tween_Play,
            sp_keyControl_Tween_Rewind,
            sp_keyControl_Tween_Pause_Resume,
            sp_keyControl_Tween_Kill,
            sp_keyControl_Tween_Replay,
            sp_keyControl_Enabled,
            sp_keyControl_Tween_Create,
            sp_DebugMode,
            sp_IsExtendedString,
            sp_TextCursor,
            sp_CursorBlinkTime,
            sp_RotateLerpMode,
            sp_RotationMode,
            sp_Vibrato,
            sp_Randomness,
            sp_FadeShake,
            sp_AutoStart,
            sp_index_TweenTypes_Rotation_Space,
            sp_Target_TmpText,
            sp_index_TweenTypes_TmpText;
        #endregion

        /// <summary>
        /// 图标
        /// </summary>
        private Texture2D icon_main, icon_statu_autokill, icon_statu_cycle, icon_statu_relative, icon_statu_tomode, icon_statu_remode_restart, icon_statu_remode_yoyo, liquid_bg_expand_pure, liquid_bg_expand_scan, liquid_plug, liquid_metal_grid, liquid_dirty;

        private bool IsPreviewed = false;
        private float TweenEasedProgress, TweenLoopProgress;
        private Color TweenLedOnColor;
        private bool fold_raw;
        private Texture2D liquid_bg;

        #region 动画类型枚举选项
        string[] tween_types_name;
        string[] tween_types_name_pos;
        string[] tween_types_name_rot;
        string[] tween_types_name_rot_space;
        string[] tween_types_name_alp;
        string[] tween_types_name_shake;
        string[] tween_types_name_text;
        string[] tween_types_name_tmp;
        string[] tween_types_name_to;
        #endregion

        #region 呼吸灯效果
        private static double lastUpdateTime;
        private static float LedBreathSpeed = 6;
        #endregion

        #region 批量化操作

        private XTween_Controller[] SelectedObjects;

        private void Targets_Get()
        {
            if (targets.Length > 1)
            {
                SelectedObjects = new XTween_Controller[targets.Length];
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    var t = targets[i];
                    SelectedObjects[i] = (XTween_Controller)t;
                }
            }
            else
            {
                SelectedObjects = new XTween_Controller[targets.Length];
                SelectedObjects[0] = (XTween_Controller)target;
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

        void OnEnable()
        {
            BaseScript = (XTween_Controller)target;

            #region 获取序列化属性
            sp_Duration = serializedObject.FindProperty("Duration");
            sp_Delay = serializedObject.FindProperty("Delay");
            sp_UseRandomDelay = serializedObject.FindProperty("UseRandomDelay");
            sp_RandomDelay = serializedObject.FindProperty("RandomDelay");
            sp_EaseMode = serializedObject.FindProperty("EaseMode");
            sp_UseCurve = serializedObject.FindProperty("UseCurve");
            sp_Curve = serializedObject.FindProperty("Curve");
            sp_LoopCount = serializedObject.FindProperty("LoopCount");
            sp_LoopDelay = serializedObject.FindProperty("LoopDelay");
            sp_LoopType = serializedObject.FindProperty("LoopType");
            sp_IsFromMode = serializedObject.FindProperty("IsFromMode");
            sp_IsRelative = serializedObject.FindProperty("IsRelative");
            sp_IsAutoKill = serializedObject.FindProperty("IsAutoKill");
            sp_IsExtendedString = serializedObject.FindProperty("IsExtendedString");
            sp_TextCursor = serializedObject.FindProperty("TextCursor");
            sp_CursorBlinkTime = serializedObject.FindProperty("CursorBlinkTime");
            sp_EndValue_String = serializedObject.FindProperty("EndValue_String");
            sp_EndValue_Int = serializedObject.FindProperty("EndValue_Int");
            sp_EndValue_Float = serializedObject.FindProperty("EndValue_Float");
            sp_EndValue_Vector2 = serializedObject.FindProperty("EndValue_Vector2");
            sp_EndValue_Vector3 = serializedObject.FindProperty("EndValue_Vector3");
            sp_EndValue_Vector4 = serializedObject.FindProperty("EndValue_Vector4");
            sp_EndValue_Color = serializedObject.FindProperty("EndValue_Color");
            sp_EndValue_Quaternion = serializedObject.FindProperty("EndValue_Quaternion");
            sp_RotateLerpMode = serializedObject.FindProperty("RotateLerpMode");
            sp_RotationMode = serializedObject.FindProperty("RotationMode");
            sp_Vibrato = serializedObject.FindProperty("Vibrato");
            sp_Randomness = serializedObject.FindProperty("Randomness");
            sp_FadeShake = serializedObject.FindProperty("FadeShake");
            sp_AutoStart = serializedObject.FindProperty("AutoStart");
            sp_index_TweenTypes_Rotation_Space = serializedObject.FindProperty("index_TweenTypes_Rotation_Space");

            sp_FromValue_Int = serializedObject.FindProperty("FromValue_Int");
            sp_FromValue_Float = serializedObject.FindProperty("FromValue_Float");
            sp_FromValue_String = serializedObject.FindProperty("FromValue_String");
            sp_FromValue_Vector2 = serializedObject.FindProperty("FromValue_Vector2");
            sp_FromValue_Vector3 = serializedObject.FindProperty("FromValue_Vector3");
            sp_FromValue_Vector4 = serializedObject.FindProperty("FromValue_Vector4");
            sp_FromValue_Color = serializedObject.FindProperty("FromValue_Color");
            sp_FromValue_Quaternion = serializedObject.FindProperty("FromValue_Quaternion");

            sp_Target_PathTool = serializedObject.FindProperty("Target_PathTool");

            sp_index_TweenTypes = serializedObject.FindProperty("index_TweenTypes");
            sp_index_TweenTypes_Positions = serializedObject.FindProperty("index_TweenTypes_Positions");
            sp_index_TweenTypes_Rotations = serializedObject.FindProperty("index_TweenTypes_Rotations");
            sp_index_TweenTypes_Alphas = serializedObject.FindProperty("index_TweenTypes_Alphas");
            sp_index_TweenTypes_Shakes = serializedObject.FindProperty("index_TweenTypes_Shakes");
            sp_index_TweenTypes_Text = serializedObject.FindProperty("index_TweenTypes_Text");
            sp_index_TweenTypes_TmpText = serializedObject.FindProperty("index_TweenTypes_TmpText");

            sp_index_TweenTypes_To = serializedObject.FindProperty("index_TweenTypes_To");

            sp_Target_RectTransform = serializedObject.FindProperty("Target_RectTransform");
            sp_Target_Image = serializedObject.FindProperty("Target_Image");
            sp_Target_CanvasGroup = serializedObject.FindProperty("Target_CanvasGroup");
            sp_Target_Text = serializedObject.FindProperty("Target_Text");
            sp_Target_TmpText = serializedObject.FindProperty("Target_TmpText");
            sp_Target_Int = serializedObject.FindProperty("Target_Int");
            sp_Target_Float = serializedObject.FindProperty("Target_Float");
            sp_Target_String = serializedObject.FindProperty("Target_String");
            sp_Target_Vector2 = serializedObject.FindProperty("Target_Vector2");
            sp_Target_Vector3 = serializedObject.FindProperty("Target_Vector3");
            sp_Target_Vector4 = serializedObject.FindProperty("Target_Vector4");
            sp_Target_Color = serializedObject.FindProperty("Target_Color");

            sp_index_AutoKillPreviewTweens = serializedObject.FindProperty("index_AutoKillPreviewTweens");
            sp_index_RewindPreviewTweensWithKill = serializedObject.FindProperty("index_RewindPreviewTweensWithKill");
            sp_index_ClearPreviewTweensWithKill = serializedObject.FindProperty("index_ClearPreviewTweensWithKill");

            sp_keyControl_Tween_Play = serializedObject.FindProperty("keyControl_Tween_Play");
            sp_keyControl_Tween_Rewind = serializedObject.FindProperty("keyControl_Tween_Rewind");
            sp_keyControl_Tween_Pause_Resume = serializedObject.FindProperty("keyControl_Tween_Pause_Resume");
            sp_keyControl_Tween_Kill = serializedObject.FindProperty("keyControl_Tween_Kill");
            sp_keyControl_Tween_Replay = serializedObject.FindProperty("keyControl_Tween_Replay");
            sp_keyControl_Enabled = serializedObject.FindProperty("keyControl_Enabled");
            sp_keyControl_Tween_Create = serializedObject.FindProperty("keyControl_Tween_Create");

            sp_DebugMode = serializedObject.FindProperty("DebugMode");
            #endregion

            #region 图标获取
            icon_main = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_controller/icon_main");

            icon_statu_autokill = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_controller/icon_statu_autokill");
            icon_statu_cycle = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_controller/icon_statu_cycle");
            icon_statu_relative = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_controller/icon_statu_relative");
            icon_statu_tomode = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_controller/icon_statu_tomode");
            icon_statu_remode_restart = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_controller/icon_statu_remode_restart");
            icon_statu_remode_yoyo = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_controller/icon_statu_remode_yoyo");

            liquid_bg_expand_pure = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_liquid/controller/liquid_bg_expand_pure");
            liquid_bg_expand_scan = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_liquid/controller/liquid_bg_expand_scan");

            liquid_plug = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_liquid/plug/liquid_plug_red");
            liquid_dirty = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_liquid/dirty/liquid_dirty");
            liquid_metal_grid = XGUI.GetCustomIcon($"{XTween_Dashboard.Get_XTween_GUIRoot_Path()}gui_liquid/liquid_metal_grid");
            #endregion

            Targets_Get();

            // 内部已处理TMPro的条件编译
            GetComponents();

            XTween_Dashboard.GetXTweenConfig();

            TweenLedOnColor = XTween_Dashboard.Theme_Primary;

            #region 动画预览器参数状态获取

            sp_index_AutoKillPreviewTweens.boolValue = XTween_Dashboard.Get_PreviewOption_AutoKillPreviewTweens();
            sp_index_AutoKillPreviewTweens.serializedObject.ApplyModifiedProperties();

            sp_index_RewindPreviewTweensWithKill.boolValue = XTween_Dashboard.Get_PreviewOption_RewindPreviewTweensWithKill();
            sp_index_RewindPreviewTweensWithKill.serializedObject.ApplyModifiedProperties();

            sp_index_ClearPreviewTweensWithKill.boolValue = XTween_Dashboard.Get_PreviewOption_ClearPreviewTweensWithKill();
            sp_index_ClearPreviewTweensWithKill.serializedObject.ApplyModifiedProperties();
            #endregion

            #region 液晶LED闪烁
            if (XTween_Dashboard.XTweenConfig != null && XTween_Dashboard.XTweenConfig.Datas.LiquidBlinker == 1)
            {
                // 注册更新回调
                EditorApplication.update += OnEditorUpdate;
                lastUpdateTime = EditorApplication.timeSinceStartup;
            }
            #endregion

            tween_types_name = System.Enum.GetNames(typeof(XTweenTypes));
            tween_types_name_pos = System.Enum.GetNames(typeof(XTweenTypes_Positions));
            tween_types_name_rot = System.Enum.GetNames(typeof(XTweenTypes_Rotations));
            tween_types_name_alp = System.Enum.GetNames(typeof(XTweenTypes_Alphas));
            tween_types_name_shake = System.Enum.GetNames(typeof(XTweenTypes_Shakes));
            tween_types_name_text = System.Enum.GetNames(typeof(XTweenTypes_Text));
            tween_types_name_tmp = System.Enum.GetNames(typeof(XTweenTypes_TmpText));
            tween_types_name_to = System.Enum.GetNames(typeof(XTweenTypes_To));
            tween_types_name_rot_space = System.Enum.GetNames(typeof(XTweenRotationSpace));
        }

        private void OnDisable()
        {
            // 注销更新回调
            EditorApplication.update -= OnEditorUpdate;

            Preview_Kill();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            Event currentEvent = Event.current;
            if (currentEvent.type == EventType.MouseDown)
            {
                // 取消当前拥有键盘焦点的控件
                GUI.FocusControl(null);
                Repaint();
            }

            XGUI.layout_banner(
                bg_fill: XGUIFilled.实体,
                bg_color: XGUIColor.深空灰,
                bg_height: 30,
                icon: icon_main,
                icon_color: XTween_Dashboard.Theme_Primary,
                title_text: "XTween  -  动画控制器",
                title_anchor: TextAnchor.MiddleLeft,
                title_style: FontStyle.Normal,
                title_color: Color.white,
                title_size: XGUIFontSize.B,
                title_clipping: TextClipping.Ellipsis);

            #region 预览
            XGUI.layout_group_start(
                type: XGUIContainerType.Horizontal,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XTween_Dashboard.Theme_Group,
                title: "预览",
                title_size: XGUIFontSize.M,
                title_text_color: XTween_Dashboard.Theme_Primary,
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(20, 20, 20, 20));

            #region 播放
            GUI.enabled = true;
            if (IsPreviewed)
            {
                if (XGUI.layout_button(
                    tooltip: "杀死预览",
                    tex_release: XGUI.GetBasedIcon("icon_stop_r"),
                    tex_press: XGUI.GetBasedIcon("icon_stop_p"),
                    tex_gui_color: Color.white,
                    width: 15,
                    height: 15))
                {
                    Preview_Kill();
                    return;
                }
            }
            else
            {
                if (XGUI.layout_button(
                    tooltip: "播放预览",
                    tex_release: XGUI.GetBasedIcon("icon_play_r"),
                    tex_press: XGUI.GetBasedIcon("icon_play_p"),
                    tex_gui_color: Color.white,
                    width: 15,
                    height: 15))
                {
                    if (!ValidPreviewed())
                    {
                        if (sp_DebugMode.boolValue)
                            XGUI_Utilitys.Console("XTween动画管理器消息", "因缺失组件或异常问题，导致无法预览动画！请检查组件项中是否缺失组件或是其他异常问题弹窗内容！", XGUIMsgState.警告);
                        return;
                    }
                    Preview_Start();
                    return;
                }
            }
            #endregion

            GUILayout.FlexibleSpace();

            #region 倒退
            GUI.enabled = true;
            if (XGUI.layout_button(
                tooltip: "倒退预览",
                tex_release: XGUI.GetBasedIcon("icon_rewind_r"),
                tex_press: XGUI.GetBasedIcon("icon_rewind_p"),
                tex_gui_color: Color.white,
                width: 15,
                height: 15))
            {
                Preview_Rewind();
                return;
            }
            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            #endregion

            #region 判断窗口宽度阈值
            bool panel_expand = XGUI.CurrentWindowWidthThreshold(">", 300);
            bool panel_extra_expand = XGUI.CurrentWindowWidthThreshold("<", 215);
            #endregion

            #region 状态
            // 获取液晶屏的根锚点
            Rect liquid_root = XGUI.GetControlRect(false, 0);
            liquid_root.Set(liquid_root.x, liquid_root.y, liquid_root.width, 0);

            BaseScript.fold_status = XGUI.layout_group_start(
               type: XGUIContainerType.Horizontal,
               bg_fill: XGUIFilled.缺口纯色边框,
               bg_color: XGUIColor.亮白,
               bg_color_gui: XTween_Dashboard.Theme_Group,
               title: "状态",
               title_size: XGUIFontSize.M,
               title_text_color: XTween_Dashboard.Theme_Primary,
               title_clipping: TextClipping.Clip,
               margin: new RectOffset(0, 0, -3, 0),
               padding: new RectOffset(
                   XTween_Dashboard.XTweenConfig.Datas.PerformanceLiquidMode ? 0 : 0,
                   XTween_Dashboard.XTweenConfig.Datas.PerformanceLiquidMode ? 0 : 0,
                   XTween_Dashboard.XTweenConfig.Datas.PerformanceLiquidMode ? (Targets_Selected() ? 15 : 15) : (Targets_Selected() ? 15 : 15),
                   !Targets_Selected() ? (XTween_Dashboard.XTweenConfig.Datas.PerformanceLiquidMode ? 15 : (BaseScript.fold_status ? 15 : 225)) : 15),
               foldout: BaseScript.fold_status);

            Color liquid_bg_color = Color.white;
            string liquid_title = "已就绪";
            bool tween_playing = false;
            if (IsPreviewed || BaseScript.CurrentTweener != null ? BaseScript.CurrentTweener.IsPlaying : false)
                tween_playing = true;
            else
                tween_playing = false;

            #region 差异化参数 - TextClipping
#if UNITY_6000_0_OR_NEWER
            // Unity 6+ 使用 Ellipsis
            TextClipping clipping = TextClipping.Ellipsis;
#else
    // Unity 2021.1 之前使用 Clip
    TextClipping clipping = TextClipping.Clip;
#endif
            #endregion

            if (!BaseScript.fold_status)
            {
                if (!Targets_Selected())
                {
                    #region 动画信息刷新
                    if (tween_playing)
                    {
                        // 状态文字
                        liquid_title = "正在动画...";
                        // 液晶背景颜色
                        liquid_bg_color = XTween_Dashboard.LiquidColor_Playing;
                        // 动画进度显示
                        TweenEasedProgress = BaseScript.CurrentTweener != null ? BaseScript.CurrentTweener.CurrentEasedProgress : 0;
                        TweenLoopProgress = BaseScript.CurrentTweener != null ? BaseScript.CurrentTweener.CurrentLoopProgress : 0;
                        // 刷新
                        Repaint();
                    }
                    else
                    {
                        // 状态文字
                        liquid_title = "已就绪";
                        // 液晶背景颜色
                        liquid_bg_color = XTween_Dashboard.LiquidColor_Idle;
                        // 动画进度清零
                        TweenEasedProgress = 0;
                        TweenLoopProgress = 0;
                        // 液晶 led 打开时的颜色
                        TweenLedOnColor = Color.white * 0.5f;
                    }
                    #endregion

                    #region 面板样式切换
                    if (XTween_Dashboard.XTweenConfig.Datas.LiquidScanStyle)
                        liquid_bg = liquid_bg_expand_scan;
                    else
                        liquid_bg = liquid_bg_expand_pure;
                    #endregion

                    if (!XTween_Dashboard.XTweenConfig.Datas.PerformanceLiquidMode)
                    {
                        Rect rect_liquid = liquid_root;

                        // 测试区域
                        //XGUI.gui_box(new Rect(rect_liquid.x, rect_liquid.y, rect_liquid.width, 100), Color.red);

                        #region 液晶 - 背景
                        float liquid_bg_x = rect_liquid.x + 10;
                        float liquid_bg_y = rect_liquid.y + 38;
                        float liquid_bg_w = liquid_root.width - 20;
                        float liquid_bg_h = liquid_bg.height;

                        rect_liquid.Set(liquid_bg_x, liquid_bg_y, liquid_bg_w, liquid_bg_h);
                        XGUI.gui_box(
                            rect: rect_liquid,
                            bg: liquid_bg,
                            bg_color_gui: liquid_bg_color,
                            border: new RectOffset(45, 45, 20, 20));
                        #endregion

                        #region 液晶 - 附加图形 - 肮脏层
                        if (XTween_Dashboard.XTweenConfig.Datas.LiquidDirty)
                        {
                            float d_x = rect_liquid.width - liquid_dirty.width;
                            float d_y = 0;
                            float d_w = liquid_dirty.width;
                            float d_h = liquid_dirty.height;
                            Rect rect_dirty = new Rect(d_x, d_y, d_w, d_h);

                            GUI.BeginGroup(rect_liquid);
                            XGUI.gui_box(
                                rect: rect_dirty,
                                bg: liquid_dirty);
                            GUI.EndGroup();
                        }
                        #endregion

                        #region 液晶 - 附加图形 - 接口
                        if (!panel_extra_expand)
                        {
                            float liquid_plug_x = ((liquid_root.width / 2)) - (liquid_plug.width / 2);
                            float liquid_plug_y = 228;
                            float liquid_plug_w = liquid_plug.width;
                            float liquid_plug_h = liquid_plug.height;

                            rect_liquid.Set(liquid_root.x + liquid_plug_x, liquid_root.y + liquid_plug_y, liquid_plug_w, liquid_plug_h);
                            XGUI.gui_box(
                                rect: rect_liquid,
                                bg: liquid_plug);
                        }
                        #endregion

                        #region 液晶 - 附加图形 - 金属网格角
                        float liquid_metal_grid_x = liquid_root.x + liquid_root.width - liquid_metal_grid.width;
                        float liquid_metal_grid_y = liquid_root.y + 188;
                        float liquid_metal_grid_w = liquid_metal_grid.width;
                        float liquid_metal_grid_h = liquid_metal_grid.height;

                        rect_liquid.Set(liquid_metal_grid_x, liquid_metal_grid_y, liquid_metal_grid_w, liquid_metal_grid_h);
                        XGUI.gui_box(
                            rect: rect_liquid,
                            bg: liquid_metal_grid);
                        #endregion

                        #region 光标闪烁指示器
                        if (tween_playing)
                        {
                            // 呼吸效果计算
                            if (XTween_Dashboard.XTweenConfig.Datas.LiquidBlinker == 1)
                            {
                                float alpha = (Mathf.Sin((float)(EditorApplication.timeSinceStartup * LedBreathSpeed) * Mathf.PI) + 1) * 0.5f;
                                TweenLedOnColor = new Color(XTween_Dashboard.Theme_Primary.r, XTween_Dashboard.Theme_Primary.g, XTween_Dashboard.Theme_Primary.b, alpha);
                            }
                            else
                                TweenLedOnColor = XTween_Dashboard.Theme_Primary;
                        }

                        float liquid_led_x = liquid_root.x + (liquid_root.width / 2) - 2;
                        float liquid_led_y = liquid_root.y + 219;
                        float liquid_led_w = 4;
                        float liquid_led_h = 2;

                        rect_liquid.Set(liquid_led_x, liquid_led_y, liquid_led_w, liquid_led_h);
                        XGUI.gui_box(rect_liquid, TweenLedOnColor);
                        #endregion

                        #region 动画状态显示
                        Rect rect_liquid_data_status = new Rect(liquid_root.x + 25, liquid_root.y + 50, liquid_root.width - liquid_root.x - 30, XGUI.GetSingleLineHeight());

                        #region 状态标题
                        // 测试区域
                        //XGUI.gui_box(rect_liquid_data_status, Color.green);

                        XGUI.gui_label(
                            rect: rect_liquid_data_status,
                            text: new GUIContent(liquid_title),
                            text_color: Color.black,
                            size: XGUIFontSize.L,
                            clipping: clipping,
                            font: XGUI.GetFont("xg-medium"));
                        #endregion

                        #region ID 显示
                        Rect rect_liquid_data_id = new Rect(rect_liquid_data_status.x, rect_liquid_data_status.y + 25, rect_liquid_data_status.width, XGUI.GetSingleLineHeight());

                        // 测试区域
                        //XGUI.gui_box(rect_liquid_data_id, Color.yellow);

                        XGUI.gui_label(
                         rect: rect_liquid_data_id,
                         text: new GUIContent($"ID :  {(BaseScript.CurrentTweener == null ? "-" : BaseScript.CurrentTweener.UniqueId.ToString())}"),
                         text_color: Color.black,
                         size: XGUIFontSize.M,
                         clipping: clipping);
                        #endregion

                        #region 短 ID 显示
                        Rect rect_liquid_data_id_short = new Rect(rect_liquid_data_status.x, rect_liquid_data_status.y + 45, rect_liquid_data_status.width, XGUI.GetSingleLineHeight());

                        // 测试区域
                        //XGUI.gui_box(rect_liquid_data_id, Color.yellow);

                        XGUI.gui_label(
                        rect: rect_liquid_data_id_short,
                        text: new GUIContent($"短 ID :  {(BaseScript.CurrentTweener == null ? "-" : BaseScript.CurrentTweener.ShortId)}"),
                        text_color: Color.black,
                        size: XGUIFontSize.M,
                        clipping: clipping);
                        #endregion

                        float margin = 152;
                        float width_max = liquid_root.width - liquid_root.x - 25 - (panel_expand ? 135 : 10);

                        #region 进度条 - EasedProgress
                        rect_liquid.Set(liquid_root.x + 25, liquid_root.y + margin, width_max, 0);
                        XGUI.gui_progress(
                            rect: rect_liquid,
                            title: panel_extra_expand ? "" : "缓动进度",
                            title_size: XGUIFontSize.M,
                            title_offset: new Vector2(0, 0),
                            title_color: Color.black,
                            subtitle: panel_extra_expand ? "" : sp_UseCurve.boolValue ? "CustomCurve" : ((EaseMode)sp_EaseMode.enumValueIndex).ToString(),
                            subtitle_size: XGUIFontSize.S,
                            subtitle_offset: new Vector2(0, 0),
                            subtitle_color: Color.black,
                            line_left_color: Color.white * 0.5f,
                            line_right_color: Color.white * 0.5f,
                            line_center_color: Color.white * 0.5f,
                            progress_fg_color: Color.black,
                            progress_bg_color: Color.black * 0.12f,
                            indicator_color: Color.black,
                            icon_indicator: XGUI.GetBasedIcon("icon_mark_arrow_up"),
                            value: TweenEasedProgress,
                            thickness: 2);
                        #endregion

                        margin += 40;

                        #region 进度条 - RawProgress
                        rect_liquid.Set(liquid_root.x + 25, liquid_root.y + margin, width_max, 0);
                        XGUI.gui_progress(
                            rect: rect_liquid,
                            title: panel_extra_expand ? "" : "原始进度",
                            title_size: XGUIFontSize.M,
                            title_offset: new Vector2(0, 0),
                            title_color: Color.black,
                            subtitle: panel_extra_expand ? "" : $"{sp_Duration.floatValue} s / 延迟 {sp_Delay.floatValue} s",
                            subtitle_size: XGUIFontSize.S,
                            subtitle_offset: new Vector2(0, 0),
                            subtitle_color: Color.black,
                             line_left_color: Color.white * 0.5f,
                             line_right_color: Color.white * 0.5f,
                             line_center_color: Color.white * 0.5f,
                            progress_fg_color: Color.black,
                            progress_bg_color: Color.black * 0.12f,
                            indicator_color: Color.black,
                            icon_indicator: XGUI.GetBasedIcon("icon_mark_arrow_up"),
                            value: TweenLoopProgress,
                            thickness: 2);
                        #endregion

                        #region 动画状态图标
                        if (panel_expand)
                        {
                            float distance = 40;
                            float div = liquid_root.width / 6 - 2.2f;

                            if (sp_IsAutoKill.boolValue)
                                GUI.color = Color.white;
                            else
                                GUI.color = Color.white * 0.2f;
                            rect_liquid.Set(panel_expand ? liquid_root.x + (liquid_root.width - distance) : liquid_root.x + div, panel_expand ? liquid_root.y + 50 : liquid_root.y + 130, 15, 15);
                            XGUI.gui_box(rect: rect_liquid, bg: icon_statu_autokill);

                            distance += 30;

                            if (sp_IsRelative.boolValue)
                                GUI.color = Color.white;
                            else
                                GUI.color = Color.white * 0.2f;
                            rect_liquid.Set(panel_expand ? liquid_root.x + (liquid_root.width - distance) : liquid_root.x + div * 2, panel_expand ? liquid_root.y + 50 : liquid_root.y + 130, 15, 15);
                            XGUI.gui_box(rect: rect_liquid, bg: icon_statu_relative);

                            distance += 30;

                            if (sp_LoopCount.intValue < 0 || sp_LoopCount.intValue > 0)
                                GUI.color = Color.white;
                            else
                                GUI.color = Color.white * 0.2f;
                            rect_liquid.Set(panel_expand ? liquid_root.x + (liquid_root.width - distance) : liquid_root.x + div * 3, panel_expand ? liquid_root.y + 50 : liquid_root.y + 130, 15, 15);
                            XGUI.gui_box(rect: rect_liquid, bg: icon_statu_cycle);

                            distance += 30;

                            if (BaseScript.TweenTypes == XTweenTypes.原生动画_To)
                                GUI.color = Color.white;
                            else
                                GUI.color = Color.white * 0.2f;
                            rect_liquid.Set(panel_expand ? liquid_root.x + (liquid_root.width - distance) : liquid_root.x + div * 4, panel_expand ? liquid_root.y + 50 : liquid_root.y + 130, 15, 15);
                            XGUI.gui_box(rect: rect_liquid, bg: icon_statu_tomode);
                            GUI.color = Color.white;

                            distance += 30;

                            rect_liquid.Set(panel_expand ? liquid_root.x + (liquid_root.width - distance) : liquid_root.x + div * 5, panel_expand ? liquid_root.y + 50 : liquid_root.y + 130, 15, 15);
                            XGUI.gui_box(rect: rect_liquid, bg: sp_LoopType.enumValueIndex == 0 ? icon_statu_remode_restart : icon_statu_remode_yoyo);
                        }
                        #endregion

                        #region EaseGraph图形
                        if (panel_expand)
                        {
                            rect_liquid.Set(liquid_root.width - 110, liquid_root.y + 140, 100, 65);

                            // 测试区域
                            //XGUI.gui_box(rect_liquid, Color.yellow);

                            XGUI.gui_icon(
                                rect: rect_liquid,
                                icon: XGUI.GetBasedIcon("EaseGraph/bg"),
                                color: sp_UseCurve.boolValue ? Color.black * 0.4f : Color.black * 0.8f);

                            if (!sp_UseCurve.boolValue)
                            {
                                rect_liquid.Set(liquid_root.width - 110, liquid_root.y + 140, 100, 65);
                                XGUI.gui_icon(
                                    rect: rect_liquid,
                                    icon: XGUI.GetBasedIcon($"EaseGraph/{((EaseMode)sp_EaseMode.enumValueIndex)}"),
                                    color: sp_UseCurve.boolValue ? Color.black * 0.4f : tween_playing ? Color.black * 0.3f : Color.black * 0.5f);

                                rect_liquid.Set(liquid_root.width - 110, liquid_root.y + 140, 100 * TweenEasedProgress, 65);
                                GUI.BeginGroup(rect_liquid);
                                rect_liquid.Set(0, 0, 100, 65);
                                XGUI.gui_icon(
                                    rect: rect_liquid,
                                    icon: XGUI.GetBasedIcon($"EaseGraph/{((EaseMode)sp_EaseMode.enumValueIndex)}"),
                                    color: tween_playing ? Color.black : Color.clear);
                                GUI.EndGroup();
                            }

                            if (sp_UseCurve.boolValue)
                            {
                                rect_liquid.Set(liquid_root.width - 95, liquid_root.y + 138, 100, 65);
                                XGUI.gui_label(
                                    rect: rect_liquid,
                                    text: new GUIContent("CustomCurve"),
                                    text_color: Color.black * 0.5f,
                                    size: XGUIFontSize.S,
                                    font_style: FontStyle.Bold,
                                    clipping: clipping);
                            }
                        }
                        #endregion
                        #endregion

                        XGUI.layout_space(218);
                    }
                    else
                    {
                        string hexcol = XGUI_Utilitys.Color_To_HexString(XTween_Dashboard.Theme_Primary, true);
                        string hexcol_gray = XGUI_Utilitys.Color_To_HexString(Color.white * 0.8f, true);

                        Rect rect_ease = liquid_root;

                        #region EaseGraph图形
                        if (panel_expand)
                        {
                            rect_ease.Set(liquid_root.width - 95, liquid_root.y + 157, 100, 65);
                            XGUI.gui_icon(
                                rect: rect_ease,
                                icon: XGUI.GetBasedIcon("EaseGraph/bg"),
                                color: sp_UseCurve.boolValue ? Color.white * 0.4f : Color.white * 0.8f);

                            if (!sp_UseCurve.boolValue)
                            {
                                rect_ease.Set(liquid_root.width - 95, liquid_root.y + 157, 100, 65);
                                XGUI.gui_icon(
                                    rect: rect_ease,
                                     icon: XGUI.GetBasedIcon($"EaseGraph/{((EaseMode)sp_EaseMode.enumValueIndex)}"),
                                    color: sp_UseCurve.boolValue ? Color.white * 0.4f : tween_playing ? Color.white * 0.5f : XTween_Dashboard.Theme_Primary);

                                rect_ease.Set(liquid_root.width - 95, liquid_root.y + 157, 100 * TweenEasedProgress, 65);
                                GUI.BeginGroup(rect_ease);
                                rect_ease.Set(0, 0, 100, 65);
                                XGUI.gui_icon(
                                    rect: rect_ease,
                                    icon: XGUI.GetBasedIcon($"EaseGraph/{((EaseMode)sp_EaseMode.enumValueIndex)}"),
                                    color: tween_playing ? XTween_Dashboard.Theme_Primary : Color.clear);
                                GUI.EndGroup();
                            }

                            if (sp_UseCurve.boolValue)
                            {
                                rect_ease.Set(liquid_root.width - 72, liquid_root.y + 125, 100, 65);
                                XGUI.gui_label(
                                    rect: rect_ease,
                                    text: new GUIContent("CustomCurve"),
                                    text_color: Color.white,
                                    size: XGUIFontSize.S,
                                    font_style: FontStyle.Bold,
                                    clipping: clipping);
                            }
                        }
                        #endregion

                        XGUI.layout_group_start(
                            type: XGUIContainerType.Vertical,
                            bg_fill: XGUIFilled.无,
                            bg_color: XGUIColor.无,
                            margin: new RectOffset(0, 0, 0, 0),
                            padding: new RectOffset(0, 0, 0, 0));

                        XGUI.layout_label(
                            text: liquid_title,
                            size: XGUIFontSize.L,
                            text_color: Color.white,
                            margin: new RectOffset(15, 0, 0, 0),
                            clipping: TextClipping.Clip,
                            font_style: FontStyle.Bold,
                            anchor: TextAnchor.MiddleLeft);

                        XGUI.layout_label(
                            text: $"<color={hexcol_gray}>ID :  </color>{(BaseScript.CurrentTweener == null ? "-" : BaseScript.CurrentTweener.UniqueId.ToString())}",
                            size: XGUIFontSize.M,
                            text_color: Color.white * 0.8f,
                            margin: new RectOffset(15, 0, 10, 0),
                            clipping: clipping,
                            font_style: FontStyle.Normal,
                            anchor: TextAnchor.MiddleLeft);

                        XGUI.layout_label(
                            text: $"<color={hexcol_gray}>短 ID :  </color>{(BaseScript.CurrentTweener == null ? " - " : BaseScript.CurrentTweener.ShortId)}",
                            size: XGUIFontSize.M,
                            text_color: Color.white * 0.8f,
                            margin: new RectOffset(15, 0, 10, 0),
                            clipping: clipping,
                            font_style: FontStyle.Normal,
                            anchor: TextAnchor.MiddleLeft);

                        XGUI.layout_label(
                            text: $"Ease:   <color={hexcol}>{TweenEasedProgress.ToString("F2")}</color>   /   Raw:   <color={hexcol}>{TweenLoopProgress.ToString("F2")}</color>",
                            size: XGUIFontSize.M,
                            text_color: Color.white,
                            margin: new RectOffset(15, 0, 10, 0),
                            clipping: clipping,
                            font_style: FontStyle.Normal,
                            anchor: TextAnchor.MiddleLeft);

                        XGUI.layout_seperator(thickness: 1,
                            color: Color.white * 0.5f,
                            margin: new RectOffset(15, 30, 12, 0));

                        XGUI.layout_label(
                            text: $"LoopMode:   {BaseScript.LoopType}  /  LoopCount:   {BaseScript.LoopCount}",
                            size: XGUIFontSize.M,
                            text_color: Color.white * 0.7f,
                            margin: new RectOffset(15, panel_expand ? 130 : 0, 18, 0),
                            clipping: TextClipping.Clip,
                            font_style: FontStyle.Normal,
                            anchor: TextAnchor.MiddleLeft);

                        XGUI.layout_label(
                           text: $"Relative:   {BaseScript.IsRelative}  /  From:   {BaseScript.IsFromMode}",
                           size: XGUIFontSize.M,
                           text_color: Color.white * 0.7f,
                           margin: new RectOffset(15, panel_expand ? 130 : 0, 10, 0),
                           clipping: TextClipping.Clip,
                           font_style: FontStyle.Normal,
                           anchor: TextAnchor.MiddleLeft);

                        XGUI.layout_group_end(type: XGUIContainerType.Vertical);
                    }
                }
                else
                {
                    string msg = "暂不支持多选控制信息查看";
                    XGUI.layout_label(
                        text: msg,
                        size: XGUIFontSize.M,
                        text_color: Color.white * 0.85f,
                        margin: new RectOffset(10, 10, 0, 0),
                        clipping: TextClipping.Clip,
                        anchor: TextAnchor.MiddleCenter);
                }
            }
            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            #endregion

            #region 预设
            BaseScript.fold_preset = XGUI.layout_group_start(
                type: XGUIContainerType.Horizontal,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XTween_Dashboard.Theme_Group,
                title: "预设",
                title_size: XGUIFontSize.M,
                title_text_color: XTween_Dashboard.Theme_Primary,
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: BaseScript.fold_preset);

            if (!BaseScript.fold_preset)
            {
                if (XGUI.layout_button(
                    text: "保存预设",
                    tooltip: "保存当前动画参数到预设",
                    bg_fill: XGUIFilled.实体,
                    bg_color: XGUIColor.亮白,
                    press_fill: XGUIFilled.实体,
                    press_color: XGUIColor.阴影灰,
                    bg_color_gui: XTween_Dashboard.Theme_Primary,
                    button_text_color: Color.black,
                    font_size: XGUIFontSize.M,
                    margin: new RectOffset(0, 0, 0, 0),
                    padding: new RectOffset(0, 0, 0, 0),
                    width: 0,
                    height: 0,
                    focus_name: "btn_save_preset"))
                {
                    Preset_Save();
                }

                if (XGUI.layout_button(
                    text: "选择预设",
                    tooltip: "在预设中心挑选预设并应用到当前动画控制器",
                    bg_fill: XGUIFilled.实体,
                    bg_color: XGUIColor.亮白,
                    press_fill: XGUIFilled.实体,
                    press_color: XGUIColor.阴影灰,
                    bg_color_gui: XTween_Dashboard.Theme_Primary,
                    button_text_color: Color.black,
                    font_size: XGUIFontSize.M,
                    margin: new RectOffset(0, 0, 0, 0),
                    padding: new RectOffset(0, 0, 0, 0),
                    width: 0,
                    height: 0,
                    focus_name: "btn_load_preset"))
                {
                    Preset_Load();
                }
            }

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            #endregion

            #region 类型选项
            BaseScript.fold_type = XGUI.layout_group_start(
             type: XGUIContainerType.Vertical,
             bg_fill: XGUIFilled.缺口纯色边框,
             bg_color: XGUIColor.亮白,
             bg_color_gui: XTween_Dashboard.Theme_Group,
             title: "类型选项",
             title_size: XGUIFontSize.M,
             title_text_color: XTween_Dashboard.Theme_Primary,
             title_clipping: TextClipping.Clip,
             padding: new RectOffset(10, 5, 15, 15),
             foldout: BaseScript.fold_type);

            if (!BaseScript.fold_type)
            {
                #region 动画类型           
                XGUI.layout_string_popup(
                   title: "动画类型",
                   title_width: 100,
                   title_color: Color.white,
                   title_size: XGUIFontSize.M,
                   title_anchor: TextAnchor.MiddleLeft,
                   prop: sp_index_TweenTypes,
                   options: tween_types_name,
                   opt_text_size: XGUIFontSize.M,
                   opt_text_color: Color.black,
                   opt_text_padding: new RectOffset(10, 10, 0, 0),
                   opt_anchor: TextAnchor.MiddleLeft,
                   opt_font_style: FontStyle.Normal,
                   opt_bg_fill: XGUIFilled.实体,
                   opt_bg_color: XGUIColor.亮白,
                   opt_bg_color_gui: XTween_Dashboard.Theme_Primary,
                   margin: new RectOffset(0, 0, 5, 5),
                   padding: new RectOffset(5, 5, 0, 0),
                   title_margin: new RectOffset(0, 0, 0, 0),
                   icon_arrow_color: Color.black,

                   act_on_changed: (value) =>
                   {
                       GetComponents();
                       RecognizedTweenTypes();
                   });
                #endregion

                #region 动画方式
                SerializedProperty prop_types = null;
                string[] options_types = null;
                string title = null;

                switch (BaseScript.TweenTypes)
                {
                    case XTweenTypes.原生动画_To:
                        prop_types = sp_index_TweenTypes_To;
                        options_types = tween_types_name_to;
                        title = "原生方式";
                        break;
                    case XTweenTypes.位置_Position:
                        prop_types = sp_index_TweenTypes_Positions;
                        options_types = tween_types_name_pos;
                        title = "位置类型";
                        break;
                    case XTweenTypes.旋转_Rotation:
                        prop_types = sp_index_TweenTypes_Rotations;
                        options_types = tween_types_name_rot;
                        title = "旋转类型";
                        break;
                    case XTweenTypes.震动_Shake:
                        prop_types = sp_index_TweenTypes_Shakes;
                        options_types = tween_types_name_shake;
                        title = "震动类型";
                        break;
                    case XTweenTypes.透明度_Alpha:
                        prop_types = sp_index_TweenTypes_Alphas;
                        options_types = tween_types_name_alp;
                        title = "透明度类型";
                        break;
                    case XTweenTypes.文字_Text:
                        prop_types = sp_index_TweenTypes_Text;
                        options_types = tween_types_name_text;
                        title = "Text属性";
                        break;
                    case XTweenTypes.文字_TmpText:
                        prop_types = sp_index_TweenTypes_TmpText;
                        options_types = tween_types_name_tmp;
                        title = "TmpText属性";
                        break;
                }

                if (prop_types != null && options_types != null)
                {
                    XGUI.layout_string_popup(
                     title: title,
                     title_width: 100,
                     title_color: Color.white,
                     title_size: XGUIFontSize.M,
                     title_anchor: TextAnchor.MiddleLeft,
                     prop: prop_types,
                     options: options_types,
                     opt_text_size: XGUIFontSize.M,
                     opt_text_color: Color.black,
                     opt_text_padding: new RectOffset(10, 10, 0, 0),
                     opt_anchor: TextAnchor.MiddleLeft,
                     opt_font_style: FontStyle.Normal,
                     opt_bg_fill: XGUIFilled.实体,
                     opt_bg_color: XGUIColor.亮白,
                     opt_bg_color_gui: XTween_Dashboard.Theme_Primary,
                     icon_arrow_color: Color.black,
                     margin: new RectOffset(0, 0, 5, 5),
                     padding: new RectOffset(5, 5, 0, 0),
                     title_margin: new RectOffset(0, 0, 0, 0),
                     act_on_changed: (value) =>
                     {
                         GetComponents();
                         RecognizedTweenTypes();
                     });
                }

                #region 旋转坐标空间独立下拉菜单
                if (BaseScript.TweenTypes == XTweenTypes.旋转_Rotation)
                {
                    XGUI.layout_string_popup(
                   title: "旋转坐标空间",
                   title_width: 100,
                   title_color: Color.white,
                   title_size: XGUIFontSize.M,
                   title_anchor: TextAnchor.MiddleLeft,
                   prop: sp_index_TweenTypes_Rotation_Space,
                   options: tween_types_name_rot_space,
                   opt_text_size: XGUIFontSize.M,
                   opt_text_color: Color.black,
                   opt_text_padding: new RectOffset(10, 10, 0, 0),
                   opt_anchor: TextAnchor.MiddleLeft,
                   opt_font_style: FontStyle.Normal,
                   opt_bg_fill: XGUIFilled.实体,
                   opt_bg_color: XGUIColor.亮白,
                   opt_bg_color_gui: XTween_Dashboard.Theme_Primary,
                   icon_arrow_color: Color.black,
                   margin: new RectOffset(0, 0, 5, 5),
                   padding: new RectOffset(5, 5, 0, 0),
                   title_margin: new RectOffset(0, 0, 0, 0),
                   act_on_changed: (value) =>
                   {
                       GetComponents();
                       RecognizedTweenTypes();
                   });
                }
                #endregion
                #endregion
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 目标值 & 起始值
            if (BaseScript.TweenTypes != XTweenTypes.无_None)
            {
                BaseScript.fold_endvalue = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIFilled.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XTween_Dashboard.Theme_Group,
                    title: "动画值",
                    title_size: XGUIFontSize.M,
                    title_text_color: XTween_Dashboard.Theme_Primary,
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 5, 15, 15),
                    foldout: BaseScript.fold_endvalue);

                if (!BaseScript.fold_endvalue)
                {
                    #region String
                    if (BaseScript.TweenTypes == XTweenTypes.原生动画_To && BaseScript.TweenTypes_To == XTweenTypes_To.字符串_String)
                    {
                        DrawField_Values("原生字符串 ( String )", sp_EndValue_String, sp_FromValue_String, sp_IsFromMode.boolValue);
                    }
                    if (BaseScript.TweenTypes == XTweenTypes.文字_Text && BaseScript.TweenTypes_Text == XTweenTypes_Text.文字内容_Content)
                    {
                        DrawField_Values("Text文字内容 ( String )", sp_EndValue_String, sp_FromValue_String, sp_IsFromMode.boolValue);
                    }
                    if (BaseScript.TweenTypes == XTweenTypes.文字_TmpText && BaseScript.TweenTypes_TmpText == XTweenTypes_TmpText.文字内容_Content)
                    {
                        DrawField_Values("TmpText文字内容 ( String )", sp_EndValue_String, sp_FromValue_String, sp_IsFromMode.boolValue);
                    }
                    #endregion

                    #region Int
                    if (BaseScript.TweenTypes == XTweenTypes.原生动画_To && BaseScript.TweenTypes_To == XTweenTypes_To.整数_Int)
                    {
                        DrawField_Values("原生整数 ( Int )", sp_EndValue_Int, sp_FromValue_Int, sp_IsFromMode.boolValue);
                    }
                    if (BaseScript.TweenTypes == XTweenTypes.文字_Text && BaseScript.TweenTypes_Text == XTweenTypes_Text.文字尺寸_FontSize)
                    {
                        DrawField_Values("Text文字尺寸 ( Int )", sp_EndValue_Int, sp_FromValue_Int, sp_IsFromMode.boolValue);
                    }
                    #endregion

                    #region Float         
                    if (BaseScript.TweenTypes == XTweenTypes.原生动画_To && BaseScript.TweenTypes_To == XTweenTypes_To.浮点数_Float)
                    {
                        DrawField_Values("原生浮点数 ( Float )", sp_EndValue_Float, sp_FromValue_Float, sp_IsFromMode.boolValue);
                    }
                    if (BaseScript.TweenTypes == XTweenTypes.透明度_Alpha && BaseScript.TweenTypes_Alphas == XTweenTypes_Alphas.Image组件)
                    {
                        DrawField_Values("Image组件的透明度 ( Float )", sp_EndValue_Float, sp_FromValue_Float, sp_IsFromMode.boolValue);
                    }
                    if (BaseScript.TweenTypes == XTweenTypes.透明度_Alpha && BaseScript.TweenTypes_Alphas == XTweenTypes_Alphas.CanvasGroup组件)
                    {
                        DrawField_Values("CanvasGroup组件的透明度 ( Float )", sp_EndValue_Float, sp_FromValue_Float, sp_IsFromMode.boolValue);
                    }
                    if (BaseScript.TweenTypes == XTweenTypes.填充_Fill)
                    {
                        DrawField_Values("Image组件的填充度 ( Float )", sp_EndValue_Float, sp_FromValue_Float, sp_IsFromMode.boolValue);
                    }
                    if (BaseScript.TweenTypes == XTweenTypes.平铺_Tiled)
                    {
                        DrawField_Values("Image组件的平铺度 ( Float )", sp_EndValue_Float, sp_FromValue_Float, sp_IsFromMode.boolValue);
                    }
                    if (BaseScript.TweenTypes == XTweenTypes.文字_Text)
                    {
                        if (BaseScript.TweenTypes_Text == XTweenTypes_Text.文字行高_LineHeight)
                        {
                            DrawField_Values("Text文字行高 ( Float )", sp_EndValue_Float, sp_FromValue_Float, sp_IsFromMode.boolValue);
                        }
                    }
                    if (BaseScript.TweenTypes == XTweenTypes.文字_TmpText)
                    {
                        if (BaseScript.TweenTypes_TmpText == XTweenTypes_TmpText.文字尺寸_FontSize)
                        {
                            DrawField_Values("TmpText文字尺寸 ( Float )", sp_EndValue_Float, sp_FromValue_Float, sp_IsFromMode.boolValue);
                        }
                        if (BaseScript.TweenTypes_TmpText == XTweenTypes_TmpText.文字行高_LineHeight)
                        {
                            DrawField_Values("TmpText文字行高 ( Float )", sp_EndValue_Float, sp_FromValue_Float, sp_IsFromMode.boolValue);
                        }
                        if (BaseScript.TweenTypes_TmpText == XTweenTypes_TmpText.文字间距_Character)
                        {
                            DrawField_Values("TmpText文字间距 ( Float )", sp_EndValue_Float, sp_FromValue_Float, sp_IsFromMode.boolValue);
                        }
                    }
                    #endregion

                    #region Vector2
                    if (BaseScript.TweenTypes == XTweenTypes.尺寸_Size)
                    {
                        DrawField_Values("RectTransform组件的尺寸 ( Vector2 )", sp_EndValue_Vector2, sp_FromValue_Vector2, sp_IsFromMode.boolValue);
                    }
                    if (BaseScript.TweenTypes == XTweenTypes.震动_Shake && BaseScript.TweenTypes_Shakes == XTweenTypes_Shakes.尺寸_Size)
                    {
                        DrawField_Values("RectTransform组件的尺寸震动 ( Vector2 )", sp_EndValue_Vector2, sp_FromValue_Vector2, sp_IsFromMode.boolValue);
                    }
                    if (BaseScript.TweenTypes == XTweenTypes.原生动画_To && BaseScript.TweenTypes_To == XTweenTypes_To.二维向量_Vector2)
                    {
                        DrawField_Values("原生二维向量 ( Vector2 )", sp_EndValue_Vector2, sp_FromValue_Vector2, sp_IsFromMode.boolValue);
                    }
                    if (BaseScript.TweenTypes == XTweenTypes.位置_Position && BaseScript.TweenTypes_Positions == XTweenTypes_Positions.锚点位置_AnchoredPosition)
                    {
                        DrawField_Values("RectTransform组件的锚点位置 ( Vector2 )", sp_EndValue_Vector2, sp_FromValue_Vector2, sp_IsFromMode.boolValue);
                    }
                    #endregion

                    #region Vector3
                    if (BaseScript.TweenTypes == XTweenTypes.原生动画_To && BaseScript.TweenTypes_To == XTweenTypes_To.三维向量_Vector3)
                    {
                        DrawField_Values("原生三维向量 ( Vector3 )", sp_EndValue_Vector3, sp_FromValue_Vector3, sp_IsFromMode.boolValue);
                    }
                    if (BaseScript.TweenTypes == XTweenTypes.旋转_Rotation && BaseScript.TweenTypes_Rotations == XTweenTypes_Rotations.欧拉角度_Euler)
                    {
                        DrawField_Values("RectTransform组件的欧拉角旋转 ( Vector3 )", sp_EndValue_Vector3, sp_FromValue_Vector3, sp_IsFromMode.boolValue);
                    }
                    if (BaseScript.TweenTypes == XTweenTypes.缩放_Scale)
                    {
                        DrawField_Values("RectTransform组件的缩放 ( Vector3 )", sp_EndValue_Vector3, sp_FromValue_Vector3, sp_IsFromMode.boolValue);
                    }
                    if (BaseScript.TweenTypes == XTweenTypes.震动_Shake && BaseScript.TweenTypes_Shakes == XTweenTypes_Shakes.位置_Position)
                    {
                        DrawField_Values("RectTransform组件的位置震动 ( Vector3 )", sp_EndValue_Vector3, sp_FromValue_Vector3, sp_IsFromMode.boolValue);
                    }
                    if (BaseScript.TweenTypes == XTweenTypes.震动_Shake && BaseScript.TweenTypes_Shakes == XTweenTypes_Shakes.旋转_Rotation)
                    {
                        DrawField_Values("RectTransform组件的旋转震动 ( Vector3 )", sp_EndValue_Vector3, sp_FromValue_Vector3, sp_IsFromMode.boolValue);
                    }
                    if (BaseScript.TweenTypes == XTweenTypes.震动_Shake && BaseScript.TweenTypes_Shakes == XTweenTypes_Shakes.缩放_Scale)
                    {
                        DrawField_Values("RectTransform组件的缩放震动 ( Vector3 )", sp_EndValue_Vector3, sp_FromValue_Vector3, sp_IsFromMode.boolValue);
                    }
                    if (BaseScript.TweenTypes == XTweenTypes.位置_Position && BaseScript.TweenTypes_Positions == XTweenTypes_Positions.锚点位置3D_AnchoredPosition3D)
                    {
                        DrawField_Values("RectTransform组件的3D锚点位置 ( Vector3 )", sp_EndValue_Vector3, sp_FromValue_Vector3, sp_IsFromMode.boolValue);
                    }
                    #endregion

                    #region Vector4
                    if (BaseScript.TweenTypes == XTweenTypes.原生动画_To && BaseScript.TweenTypes_To == XTweenTypes_To.四维向量_Vector4)
                    {
                        DrawField_Values("原生四维向量 ( Vector4 )", sp_EndValue_Vector4, sp_FromValue_Vector4, sp_IsFromMode.boolValue);
                    }
                    if (BaseScript.TweenTypes == XTweenTypes.文字_TmpText && BaseScript.TweenTypes_TmpText == XTweenTypes_TmpText.文字边距_Margin)
                    {
                        DrawField_Values("TmpText文字边距 ( Vector4 )", sp_EndValue_Vector4, sp_FromValue_Vector4, sp_IsFromMode.boolValue);
                    }
                    #endregion

                    #region Color          
                    if (BaseScript.TweenTypes == XTweenTypes.原生动画_To && BaseScript.TweenTypes_To == XTweenTypes_To.颜色_Color)
                    {
                        DrawField_Values("原生颜色 ( Color )", sp_EndValue_Color, sp_FromValue_Color, sp_IsFromMode.boolValue);
                    }
                    if (BaseScript.TweenTypes == XTweenTypes.颜色_Color)
                    {
                        DrawField_Values("Image组件的颜色 ( Color )", sp_EndValue_Color, sp_FromValue_Color, sp_IsFromMode.boolValue);
                    }
                    if (BaseScript.TweenTypes == XTweenTypes.文字_Text && BaseScript.TweenTypes_Text == XTweenTypes_Text.文字颜色_Color)
                    {
                        DrawField_Values("Text文字颜色 ( Color )", sp_EndValue_Color, sp_FromValue_Color, sp_IsFromMode.boolValue);
                    }
                    if (BaseScript.TweenTypes == XTweenTypes.文字_TmpText && BaseScript.TweenTypes_TmpText == XTweenTypes_TmpText.文字颜色_Color)
                    {
                        DrawField_Values("TmpText文字颜色 ( Color )", sp_EndValue_Color, sp_FromValue_Color, sp_IsFromMode.boolValue);
                    }
                    #endregion

                    #region Quaternion
                    if (BaseScript.TweenTypes == XTweenTypes.旋转_Rotation && BaseScript.TweenTypes_Rotations == XTweenTypes_Rotations.四元数_Quaternion)
                    {
                        DrawField_Values("RectTransform组件的四元数旋转 ( Quaternion )", sp_EndValue_Quaternion, sp_FromValue_Quaternion, sp_IsFromMode.boolValue);
                    }
                    #endregion
                }

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            }
            #endregion

            #region 当前值 & 组件
            if (BaseScript.TweenTypes != XTweenTypes.无_None)
            {
                BaseScript.fold_current_or_components = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIFilled.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XTween_Dashboard.Theme_Group,
                    title: BaseScript.TweenTypes == XTweenTypes.原生动画_To ? "当前值" : "组件",
                    title_size: XGUIFontSize.M,
                    title_text_color: XTween_Dashboard.Theme_Primary,
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 5, 15, 15),
                    margin: new RectOffset(0, 0, 0, 0),
                    foldout: BaseScript.fold_current_or_components);

                if (!BaseScript.fold_current_or_components)
                {
                    #region 当前值
                    if (BaseScript.TweenTypes == XTweenTypes.原生动画_To)
                    {
                        if (BaseScript.TweenTypes_To == XTweenTypes_To.字符串_String)
                        {
                            DrawField_CurrentValue(sp_Target_String, 100);
                        }
                        if (BaseScript.TweenTypes_To == XTweenTypes_To.整数_Int)
                        {
                            DrawField_CurrentValue(sp_Target_Int, 100);
                        }
                        if (BaseScript.TweenTypes_To == XTweenTypes_To.浮点数_Float)
                        {
                            DrawField_CurrentValue(sp_Target_Float, 100);
                        }
                        if (BaseScript.TweenTypes_To == XTweenTypes_To.二维向量_Vector2)
                        {
                            DrawField_CurrentValue(sp_Target_Vector2, 100);
                        }
                        if (BaseScript.TweenTypes_To == XTweenTypes_To.三维向量_Vector3)
                        {
                            DrawField_CurrentValue(sp_Target_Vector3, 100);
                        }
                        if (BaseScript.TweenTypes_To == XTweenTypes_To.四维向量_Vector4)
                        {
                            DrawField_CurrentValue(sp_Target_Vector4, 100);
                        }
                        if (BaseScript.TweenTypes_To == XTweenTypes_To.颜色_Color)
                        {
                            DrawField_CurrentValue(sp_Target_Color, 100);
                        }

                        XGUI.layout_helpbox(
                            state: XGUIHelboxState.警告,
                            title_text: "此值是动画的当前数值",
                            title_size: XGUIFontSize.M,
                            title_style: FontStyle.Normal,
                            title_color: Color.white * 0.75f);
                    }
                    #endregion

                    #region 组件
                    if (BaseScript.TweenTypes == XTweenTypes.路径_Path)
                    {
                        XGUI.layout_property_field(
                            title: "目标组件",
                            title_size: XGUIFontSize.M,
                            title_color: Color.white,
                            title_hover_color: XTween_Dashboard.Theme_Primary,
                            title_width: 100,
                            status_icon: "icon_field_status",
                            status_icon_color: sp_Target_PathTool.objectReferenceValue != null ? XTween_Dashboard.Theme_Primary : Color.black * 0.7f,
                            prop: sp_Target_PathTool,
                            prop_margin: new RectOffset(0, 0, 10, 10));

                        if (sp_Target_PathTool.objectReferenceValue == null)
                        {
                            XGUI.layout_helpbox(
                                state: XGUIHelboxState.警告,
                                title_text: "PathTool 为路径绘制组件！不可为空！",
                                title_size: XGUIFontSize.M,
                                title_style: FontStyle.Normal,
                                title_color: Color.white * 0.75f);
                        }
                    }
                    if (BaseScript.TweenTypes == XTweenTypes.位置_Position || BaseScript.TweenTypes == XTweenTypes.旋转_Rotation || BaseScript.TweenTypes == XTweenTypes.缩放_Scale || BaseScript.TweenTypes == XTweenTypes.尺寸_Size || BaseScript.TweenTypes == XTweenTypes.震动_Shake)
                    {
                        XGUI.layout_property_field(
                         title: "目标组件",
                         title_size: XGUIFontSize.M,
                         title_color: Color.white,
                         title_hover_color: XTween_Dashboard.Theme_Primary,
                         title_width: 100,
                         status_icon: "icon_field_status",
                         status_icon_color: sp_Target_RectTransform.objectReferenceValue != null ? XTween_Dashboard.Theme_Primary : Color.black * 0.7f,
                         prop: sp_Target_RectTransform,
                         prop_margin: new RectOffset(0, 0, 10, 10));

                        if (sp_Target_RectTransform.objectReferenceValue == null)
                        {
                            XGUI.layout_helpbox(
                                state: XGUIHelboxState.警告,
                                title_text: "该 RectTransform 为你需要动画化的变换组件！不可为空！",
                                title_size: XGUIFontSize.M,
                                title_style: FontStyle.Normal,
                                title_color: Color.white * 0.75f);
                        }
                    }
                    if (BaseScript.TweenTypes == XTweenTypes.颜色_Color || (BaseScript.TweenTypes == XTweenTypes.透明度_Alpha && BaseScript.TweenTypes_Alphas == XTweenTypes_Alphas.Image组件) || BaseScript.TweenTypes == XTweenTypes.填充_Fill || BaseScript.TweenTypes == XTweenTypes.平铺_Tiled)
                    {
                        XGUI.layout_property_field(
                       title: "目标组件",
                       title_size: XGUIFontSize.M,
                       title_color: Color.white,
                       title_hover_color: XTween_Dashboard.Theme_Primary,
                       title_width: 100,
                       status_icon: "icon_field_status",
                       status_icon_color: sp_Target_Image.objectReferenceValue != null ? XTween_Dashboard.Theme_Primary : Color.black * 0.7f,
                       prop: sp_Target_Image,
                       prop_margin: new RectOffset(0, 0, 10, 10));

                        if (sp_Target_Image.objectReferenceValue == null)
                        {
                            XGUI.layout_helpbox(
                                state: XGUIHelboxState.警告,
                                title_text: "该 Image 为你需要动画化的变换组件！不可为空！",
                                title_size: XGUIFontSize.M,
                                title_style: FontStyle.Normal,
                                title_color: Color.white * 0.75f);
                        }
                    }
                    if (BaseScript.TweenTypes == XTweenTypes.透明度_Alpha && BaseScript.TweenTypes_Alphas == XTweenTypes_Alphas.CanvasGroup组件)
                    {
                        XGUI.layout_property_field(
                            title: "目标组件",
                            title_size: XGUIFontSize.M,
                            title_color: Color.white,
                            title_hover_color: XTween_Dashboard.Theme_Primary,
                            title_width: 100,
                            status_icon: "icon_field_status",
                            status_icon_color: sp_Target_CanvasGroup.objectReferenceValue != null ? XTween_Dashboard.Theme_Primary : Color.black * 0.7f,
                            prop: sp_Target_CanvasGroup,
                            prop_margin: new RectOffset(0, 0, 10, 10));

                        if (sp_Target_CanvasGroup.objectReferenceValue == null)
                        {
                            XGUI.layout_helpbox(
                                state: XGUIHelboxState.警告,
                                title_text: "该 CanvasGroup 为你需要动画化的画布组件！不可为空！",
                                title_size: XGUIFontSize.M,
                                title_style: FontStyle.Normal,
                                title_color: Color.white * 0.75f);
                        }
                    }
                    if (BaseScript.TweenTypes == XTweenTypes.文字_Text)
                    {
                        XGUI.layout_property_field(
                            title: "目标组件",
                            title_size: XGUIFontSize.M,
                            title_color: Color.white,
                            title_hover_color: XTween_Dashboard.Theme_Primary,
                            title_width: 100,
                            status_icon: "icon_field_status",
                            status_icon_color: sp_Target_Text.objectReferenceValue != null ? XTween_Dashboard.Theme_Primary : Color.black * 0.7f,
                            prop: sp_Target_Text,
                            prop_margin: new RectOffset(0, 0, 10, 10));

                        if (sp_Target_Text.objectReferenceValue == null)
                        {
                            XGUI.layout_helpbox(
                                state: XGUIHelboxState.警告,
                                title_text: "该 Text 为你需要动画化的文字组件！不可为空！",
                                title_size: XGUIFontSize.M,
                                title_style: FontStyle.Normal,
                                title_color: Color.white * 0.75f);
                        }
                    }
                    if (BaseScript.TweenTypes == XTweenTypes.文字_TmpText)
                    {
                        XGUI.layout_property_field(
                            title: "目标组件",
                            title_size: XGUIFontSize.M,
                            title_color: Color.white,
                            title_hover_color: XTween_Dashboard.Theme_Primary,
                            title_width: 100,
                            status_icon: "icon_field_status",
                            status_icon_color: sp_Target_TmpText.objectReferenceValue != null ? XTween_Dashboard.Theme_Primary : Color.black * 0.7f,
                            prop: sp_Target_TmpText,
                            prop_margin: new RectOffset(0, 0, 10, 10));

                        if (sp_Target_TmpText.objectReferenceValue == null)
                        {
                            XGUI.layout_helpbox(
                                state: XGUIHelboxState.警告,
                                title_text: "该 Text 为你需要动画化的文字组件！不可为空！",
                                title_size: XGUIFontSize.M,
                                title_style: FontStyle.Normal,
                                title_color: Color.white * 0.75f);
                        }
                    }
                    #endregion
                }

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            }
            #endregion

            #region 动画参数
            BaseScript.fold_params = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XTween_Dashboard.Theme_Group,
                title: "动画参数",
                title_size: XGUIFontSize.M,
                title_text_color: XTween_Dashboard.Theme_Primary,
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 5, 15, 15),
                foldout: BaseScript.fold_params);

            if (!BaseScript.fold_params)
            {
                DrawField_Params("耗时", sp_Duration, 80);
                DrawField_Params("延迟", sp_Delay, 80);
                DrawField_Params("随机延迟", sp_UseRandomDelay, 80);
                DrawField_Params("延迟范围", sp_RandomDelay, 80);
                DrawField_Params("缓动模式", sp_EaseMode, 80);
                DrawField_Params("使用曲线", sp_UseCurve, 80);
                DrawField_Params("运动曲线", sp_Curve, 80);
                DrawField_Params("循环次数", sp_LoopCount, 80);
                DrawField_Params("循环延迟", sp_LoopDelay, 80);
                DrawField_Params("循环方式", sp_LoopType, 80);
                DrawField_Params("指定起始", sp_IsFromMode, 80);
                DrawField_Params("相对动画", sp_IsRelative, 80);
                DrawField_Params("自动杀死", sp_IsAutoKill, 80);

                if ((BaseScript.TweenTypes == XTweenTypes.原生动画_To && BaseScript.TweenTypes_To == XTweenTypes_To.字符串_String) ||
                    (BaseScript.TweenTypes == XTweenTypes.文字_Text && BaseScript.TweenTypes_Text == XTweenTypes_Text.文字内容_Content) ||
                    (BaseScript.TweenTypes == XTweenTypes.文字_TmpText && BaseScript.TweenTypes_TmpText == XTweenTypes_TmpText.文字内容_Content))
                {
                    DrawField_Params("扩展字符串", sp_IsExtendedString, 100);
                    DrawField_Params("光标符号", sp_TextCursor, 100);
                    DrawField_Params("光标闪烁速率", sp_CursorBlinkTime, 100);
                }
                if (BaseScript.TweenTypes == XTweenTypes.旋转_Rotation && BaseScript.TweenTypes_Rotations == XTweenTypes_Rotations.欧拉角度_Euler)
                    DrawField_Params("欧拉角度旋转方式", sp_RotationMode, 100);
                if (BaseScript.TweenTypes == XTweenTypes.旋转_Rotation && BaseScript.TweenTypes_Rotations == XTweenTypes_Rotations.四元数_Quaternion)
                    DrawField_Params("四元数过渡方式", sp_RotateLerpMode, 100);
                if (BaseScript.TweenTypes == XTweenTypes.震动_Shake)
                {
                    DrawField_Params("震动频率", sp_Vibrato, 100);
                    DrawField_Params("震动随机度", sp_Randomness, 100);
                    DrawField_Params("震动渐变", sp_FadeShake, 100);
                }
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 选项
            BaseScript.fold_option = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIFilled.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XTween_Dashboard.Theme_Group,
                title: "选项",
                title_size: XGUIFontSize.M,
                title_text_color: XTween_Dashboard.Theme_Primary,
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: BaseScript.fold_option);
            if (!BaseScript.fold_option)
            {
                // 调试信息
                DrawToggle("调试信息", sp_DebugMode, 120, (b) =>
                {

                });

                // 启用按键控制
                DrawToggle("启用按键控制", sp_keyControl_Enabled, 120, (b) =>
                {

                });

                // 自动播放
                DrawToggle("自动播放", sp_AutoStart, 120, (b) =>
                {

                });

                XGUI.layout_seperator(
                    thickness: 1,
                    color: XTween_Dashboard.Theme_SeperateLine,
                    margin: new RectOffset(0, 0, 15, 15),
                    padding: new RectOffset(0, 0, 0, 0));

                // 自动停止预览
                DrawToggle("自动停止预览", sp_index_AutoKillPreviewTweens, 120, (b) =>
                {
                    XTween_Dashboard.Set_PreviewOption_AutoKillPreviewTweens(sp_index_AutoKillPreviewTweens.boolValue);
                    Editor_XTween_Previewer.AutoKillWithDuration = sp_index_AutoKillPreviewTweens.boolValue;

                    // 如果为自动杀死动画则会强制开启：杀死前重置动画 / 杀死后清空预览列表
                    if (sp_index_AutoKillPreviewTweens.boolValue)
                    {
                        sp_index_RewindPreviewTweensWithKill.boolValue = true;
                        sp_index_RewindPreviewTweensWithKill.serializedObject.ApplyModifiedProperties();
                        sp_index_ClearPreviewTweensWithKill.boolValue = true;
                        sp_index_ClearPreviewTweensWithKill.serializedObject.ApplyModifiedProperties();

                        XTween_Dashboard.Set_PreviewOption_RewindPreviewTweensWithKill(sp_index_RewindPreviewTweensWithKill.boolValue);
                        Editor_XTween_Previewer.BeforeKillRewind = sp_index_RewindPreviewTweensWithKill.boolValue;

                        XTween_Dashboard.Set_PreviewOption_ClearPreviewTweensWithKill(sp_index_ClearPreviewTweensWithKill.boolValue);
                        Editor_XTween_Previewer.AfterKillClear = sp_index_ClearPreviewTweensWithKill.boolValue;
                    }
                });

                // 杀死前先重置动画
                DrawToggle("杀死前先重置动画", sp_index_RewindPreviewTweensWithKill, 120, (b) =>
                {
                    XTween_Dashboard.Set_PreviewOption_RewindPreviewTweensWithKill(sp_index_RewindPreviewTweensWithKill.boolValue);
                    Editor_XTween_Previewer.BeforeKillRewind = sp_index_RewindPreviewTweensWithKill.boolValue;
                });

                // 杀死后清空预览器列表
                DrawToggle("杀死后清空预览器列表", sp_index_ClearPreviewTweensWithKill, 120, (b) =>
                {
                    XTween_Dashboard.Set_PreviewOption_ClearPreviewTweensWithKill(sp_index_ClearPreviewTweensWithKill.boolValue);
                    Editor_XTween_Previewer.AfterKillClear = sp_index_ClearPreviewTweensWithKill.boolValue;
                });

            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 控制按键 
            if (sp_keyControl_Enabled.boolValue)
            {
                BaseScript.fold_keycontrol = XGUI.layout_group_start(
                    type: XGUIContainerType.Vertical,
                    bg_fill: XGUIFilled.缺口纯色边框,
                    bg_color: XGUIColor.亮白,
                    bg_color_gui: XTween_Dashboard.Theme_Group,
                    title: "控制按键",
                    title_size: XGUIFontSize.M,
                    title_text_color: XTween_Dashboard.Theme_Primary,
                    title_clipping: TextClipping.Clip,
                    padding: new RectOffset(10, 10, 15, 15),
                    foldout: BaseScript.fold_keycontrol);
                if (!BaseScript.fold_keycontrol)
                {
                    DrawField_Params("动画创建", sp_keyControl_Tween_Create, 100);
                    DrawField_Params("动画播放", sp_keyControl_Tween_Play, 100);
                    DrawField_Params("动画倒退", sp_keyControl_Tween_Rewind, 100);
                    DrawField_Params("动画暂停&继续", sp_keyControl_Tween_Pause_Resume, 100);
                    DrawField_Params("动画杀死", sp_keyControl_Tween_Kill, 100);
                    DrawField_Params("动画重播", sp_keyControl_Tween_Replay, 100);
                }
                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            }
            #endregion

            #region 快捷菜单
            Event e = Event.current;
            if (e.type == EventType.MouseDown && e.button == 1)
            {
                // 创建右键菜单
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("D (折叠所有面板)"), false, () =>
                {
                    BaseScript.fold_params = true;
                    BaseScript.fold_preset = true;
                    BaseScript.fold_status = true;
                    BaseScript.fold_type = true;
                    BaseScript.fold_endvalue = true;
                    BaseScript.fold_current_or_components = true;
                    BaseScript.fold_option = true;
                    return;
                });
                menu.AddItem(new GUIContent("F (展开所有面板)"), false, () =>
                {
                    BaseScript.fold_params = false;
                    BaseScript.fold_preset = false;
                    BaseScript.fold_status = false;
                    BaseScript.fold_type = false;
                    BaseScript.fold_endvalue = false;
                    BaseScript.fold_current_or_components = false;
                    BaseScript.fold_option = false;
                    return;
                });
                menu.AddDisabledItem(new GUIContent("动画预览"));
                menu.AddItem(new GUIContent("A (预览)"), false, () =>
                {
                    if (!ValidPreviewed())
                    {
                        if (sp_DebugMode.boolValue)
                            XGUI_Utilitys.Console("XTween动画管理器消息", "因缺失组件，导致无法预览动画！请检查组件项中是否未指定组件！", XGUIMsgState.警告);
                        return;
                    }
                    Preview_Start();
                    return;
                });
                menu.AddItem(new GUIContent("R (倒退)"), false, () =>
                {
                    Preview_Rewind();
                    return;
                });
                menu.AddItem(new GUIContent("S (杀死)"), false, () =>
                {
                    Preview_Kill();
                    return;
                });

                menu.ShowAsContext(); // 在鼠标位置显示右键菜单
                e.Use();
            }
            #endregion

            #region 原始类
            fold_raw = XGUI.layout_group_start(
                 type: XGUIContainerType.Vertical,
                 bg_fill: XGUIFilled.缺口纯色边框,
                 bg_color: XGUIColor.亮白,
                 bg_color_gui: XTween_Dashboard.Theme_Group,
                 title: "原始类",
                 title_size: XGUIFontSize.M,
                 title_text_color: XTween_Dashboard.Theme_Primary,
                 title_clipping: TextClipping.Clip,
                 padding: new RectOffset(10, 5, 15, 15),
                 foldout: fold_raw);

            if (fold_raw)
            {
                DrawDefaultInspector();
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 编辑器更新（用于动画状态LED闪烁更新）
        /// </summary>
        private void OnEditorUpdate()
        {
            // 限制刷新频率，每0.05秒检查一次是否需要重绘
            if (EditorApplication.timeSinceStartup - lastUpdateTime > 0.05)
            {
                lastUpdateTime = EditorApplication.timeSinceStartup;
            }
        }

        /// <summary>
        /// 杀死预览动画后的操作逻辑
        /// </summary>
        private void OnAutoKillPreview()
        {
            IsPreviewed = false;
            //BaseScript.CurrentTweener.Rewind();
            BaseScript.CurrentTweener = null;
            Editor_XTween_Previewer.act_on_editor_autokill -= OnAutoKillPreview;
        }

        #region 动画方法
        /// <summary>
        ///  动画预览 - 倒退
        /// </summary>
        private void Preview_Rewind()
        {
            IsPreviewed = false;
            Editor_XTween_Previewer.Rewind();
        }
        /// <summary>
        ///  动画预览 - 杀死
        /// </summary>
        private void Preview_Kill()
        {
            if (Application.isPlaying)
                return;
            if (BaseScript.TweenTypes == XTweenTypes.无_None)
                return;
            IsPreviewed = false;
            Editor_XTween_Previewer.Kill(Editor_XTween_Previewer.AfterKillClear, Editor_XTween_Previewer.BeforeKillRewind, () => { BaseScript.CurrentTweener = null; });

            // 当动画预览器为根据动画耗时自动杀死的情况下
            if (sp_index_AutoKillPreviewTweens.boolValue)
                Editor_XTween_Previewer.act_on_editor_autokill -= OnAutoKillPreview;
        }
        /// <summary>
        ///  动画预览 - 开始
        /// </summary>
        private void Preview_Start()
        {
            if (Application.isPlaying)
                return;
            if (BaseScript.TweenTypes == XTweenTypes.无_None)
                return;

            IsPreviewed = true;
            if (BaseScript.CurrentTweener == null)
                BaseScript.Tween_Create();

            // 当动画预览器为根据动画耗时自动杀死的情况下
            if (sp_index_AutoKillPreviewTweens.boolValue)
            {
                Editor_XTween_Previewer.AfterKillClear = true;
                Editor_XTween_Previewer.BeforeKillRewind = true;
                Editor_XTween_Previewer.act_on_editor_autokill += OnAutoKillPreview;
            }

            #region 添加至预览器并播放
            Editor_XTween_Previewer.Append(BaseScript.CurrentTweener);
            Editor_XTween_Previewer.Play(null, sp_DebugMode.boolValue);
            #endregion
        }
        #endregion

        #region 辅助方法
        /// <summary>
        /// 通用方法：绘制动画目标值和起始值字段
        /// </summary>
        private void DrawField_Values(string label, SerializedProperty prop_end, SerializedProperty prop_from, bool isFromMode)
        {
            #region 值说明
            XGUI.layout_label(
                text: label,
                size: XGUIFontSize.L,
                text_color: Color.white * 0.75f,
                margin: new RectOffset(20, 0, 0, 10),
                clipping: TextClipping.Clip,
                font: XGUI.GetFont("xg-bold"),
                font_style: FontStyle.Normal,
                anchor: TextAnchor.MiddleLeft);
            #endregion

            if (isFromMode)
            {
                XGUI.layout_property_field(
                title: "起始",
                title_size: XGUIFontSize.M,
                //title_color: Color.white,
                title_hover_color: XTween_Dashboard.Theme_Primary,
                title_width: 100,
                //status_icon: "icon_field_status",
                //status_icon_color: Color.green,
                title_anchor: TextAnchor.MiddleLeft,
                prop: prop_from,
                prop_margin: new RectOffset(0, 0, 5, 0));
            }

            XGUI.layout_property_field(
                title: "目标",
                title_size: XGUIFontSize.M,
                //title_color: Color.white,
                title_hover_color: XTween_Dashboard.Theme_Primary,
                title_width: 100,
                //status_icon: "icon_field_status",
                //status_icon_color: Color.green,
                title_anchor: TextAnchor.MiddleLeft,
                prop: prop_end,
                prop_margin: new RectOffset(0, 0, 5, 0));
        }
        /// <summary>
        /// 通用方法：绘制动画当前值
        /// </summary>
        private void DrawField_CurrentValue(SerializedProperty prop, float title_width)
        {
            XGUI.layout_property_field(
                title: "当前",
                title_size: XGUIFontSize.M,
                title_hover_color: XTween_Dashboard.Theme_Primary,
                title_width: title_width,
                //status_icon: "icon_field_status",
                //status_icon_color: Color.green,
                prop: prop,
                prop_margin: new RectOffset(0, 0, 10, 10));
        }
        /// <summary>
        /// 通用方法：绘制动画参数
        /// </summary>
        private void DrawField_Params(string title, SerializedProperty prop, float title_width)
        {
            XGUI.layout_property_field(
              title: title,
              title_size: XGUIFontSize.M,
              title_anchor: TextAnchor.MiddleLeft,
              //title_color: Color.white,
              title_hover_color: XTween_Dashboard.Theme_Primary,
              title_width: title_width,
              //status_icon: "icon_field_status",
              //status_icon_color: Color.green,
              prop: prop,
              prop_margin: new RectOffset(0, 0, 5, 0));
        }
        /// <summary>
        /// 通用方法：绘制开关
        /// </summary>
        private void DrawToggle(string title, SerializedProperty prop, float width, Action<bool> act_on_changed = null)
        {
            XGUI.layout_toggle(
                title: title,
                title_size: XGUIFontSize.M,
                title_font_style: FontStyle.Normal,
                title_padding: new RectOffset(0, 10, 0, 0),
                title_width: width,
                prop: prop,
                tog_style: XGUIToggleStyle.实体,
                tog_padding: new RectOffset(5, 0, 0, 0),
                tog_margin: new RectOffset(0, 0, 0, 5),
                tog_mixed_options: new string[] { "禁用", "启用" },
                tog_mixed_text_size: XGUIFontSize.M,
                tog_mixed_text_color: Color.black,
                tog_mixed_text_padding: new RectOffset(10, 10, 0, 0),
                tog_mixed_text_anchor: TextAnchor.MiddleCenter,
                tog_mixed_font_style: FontStyle.Normal,
                tog_bg_off_color: new Color(0.38f, 0.38f, 0.38f),
                tog_bg_on_color: XTween_Dashboard.Theme_Primary,
                tog_handler_off_color: Color.white,
                tog_handler_on_color: Color.white,
                tog_mixed_bg_color_gui: XTween_Dashboard.Theme_Primary,
                act_on_changed: act_on_changed);
        }
        /// <summary>
        /// 检查被动画的组件是否正确指定并有效
        /// </summary>
        /// <returns></returns>
        private bool ValidPreviewed()
        {
            bool valid = true;

            string hexcol = XGUI_Utilitys.Color_To_HexString(XTween_Dashboard.Theme_Primary, true);

            if (BaseScript.TweenTypes == XTweenTypes.文字_TmpText)
            {
                if (sp_Target_TmpText.objectReferenceValue == null)
                {
                    valid = false;
                    XGUI.dialog(
                        type: XGUIDialogType.警告,
                        windowtitle: "XTween动画控制器消息",
                        title: "文字组件异常",
                        msg: $"检测到您未正确指定对应动画所需要的\"<color={hexcol}>  TmpText </color>\"组件! ，请正确指定后再预览！",
                        ok: "明白",
                        PrimaryIndex: 0);
                }
            }
            if (BaseScript.TweenTypes == XTweenTypes.文字_Text)
            {
                if (sp_Target_Text.objectReferenceValue == null)
                {
                    valid = false;
                    XGUI.dialog(
                        type: XGUIDialogType.警告,
                        windowtitle: "XTween动画控制器消息",
                        title: "文字组件异常",
                        msg: $"检测到您未正确指定对应动画所需要的\"<color={hexcol}>  Text </color>\"组件!，请正确指定后再预览！",
                        ok: "明白",
                        PrimaryIndex: 0);
                }
            }
            if (BaseScript.TweenTypes == XTweenTypes.位置_Position)
            {
                if (sp_Target_RectTransform.objectReferenceValue == null)
                {
                    valid = false;
                    XGUI.dialog(
                        type: XGUIDialogType.警告,
                        windowtitle: "XTween动画控制器消息",
                        title: "变换组件异常",
                        msg: $"检测到您未正确指定对应动画所需要的\"<color={hexcol}>  RectTransform </color>\"组件!，请正确指定后再预览！",
                        ok: "明白",
                        PrimaryIndex: 0);
                }
            }
            if (BaseScript.TweenTypes == XTweenTypes.旋转_Rotation)
            {
                if (sp_Target_RectTransform.objectReferenceValue == null)
                {
                    valid = false;
                    XGUI.dialog(
                        type: XGUIDialogType.警告,
                        windowtitle: "XTween动画控制器消息",
                        title: "变换组件异常",
                        msg: $"检测到您未正确指定对应动画所需要的\"<color={hexcol}>  RectTransform </color>\"组件!，请正确指定后再预览！",
                        ok: "明白",
                        PrimaryIndex: 0);
                }
            }
            if (BaseScript.TweenTypes == XTweenTypes.缩放_Scale)
            {
                if (sp_Target_RectTransform.objectReferenceValue == null)
                {
                    valid = false;
                    XGUI.dialog(
                        type: XGUIDialogType.警告,
                        windowtitle: "XTween动画控制器消息",
                        title: "变换组件异常",
                        msg: $"检测到您未正确指定对应动画所需要的\"<color={hexcol}>  RectTransform </color>\"组件!，请正确指定后再预览！",
                        ok: "明白",
                        PrimaryIndex: 0);
                }
            }
            if (BaseScript.TweenTypes == XTweenTypes.尺寸_Size)
            {
                if (sp_Target_RectTransform.objectReferenceValue == null)
                {
                    valid = false;
                    XGUI.dialog(
                        type: XGUIDialogType.警告,
                        windowtitle: "XTween动画控制器消息",
                        title: "变换组件异常",
                        msg: $"检测到您未正确指定对应动画所需要的\"<color={hexcol}>  RectTransform </color>\"组件!，请正确指定后再预览！",
                        ok: "明白",
                        PrimaryIndex: 0);
                }
            }
            if (BaseScript.TweenTypes == XTweenTypes.颜色_Color)
            {
                if (sp_Target_Image.objectReferenceValue == null)
                {
                    valid = false;
                    XGUI.dialog(
                        type: XGUIDialogType.警告,
                        windowtitle: "XTween动画控制器消息",
                        title: "图像组件异常",
                        msg: $"检测到您未正确指定对应动画所需要的\"<color={hexcol}>  Image </color>\"组件!，请正确指定后再预览！",
                        ok: "明白",
                        PrimaryIndex: 0);
                }
            }
            if (BaseScript.TweenTypes == XTweenTypes.透明度_Alpha)
            {
                if (BaseScript.TweenTypes_Alphas == XTweenTypes_Alphas.Image组件)
                {
                    if (sp_Target_Image.objectReferenceValue == null)
                    {
                        valid = false;
                        XGUI.dialog(
                            type: XGUIDialogType.警告,
                            windowtitle: "XTween动画控制器消息",
                            title: "图像组件异常",
                            msg: $"检测到您未正确指定对应动画所需要的\"<color={hexcol}>  Image </color>\"组件!，请正确指定后再预览！",
                            ok: "明白",
                            PrimaryIndex: 0);
                    }
                }
                else if (BaseScript.TweenTypes_Alphas == XTweenTypes_Alphas.CanvasGroup组件)
                {
                    if (sp_Target_CanvasGroup.objectReferenceValue == null)
                    {
                        valid = false;
                        XGUI.dialog(
                            type: XGUIDialogType.警告,
                            windowtitle: "XTween动画控制器消息",
                            title: "画布编组组件异常",
                            msg: $"检测到您未正确指定对应动画所需要的\"<color={hexcol}>  CanvasGroup </color>\"组件!，请正确指定后再预览！",
                            ok: "明白",
                            PrimaryIndex: 0);
                    }
                }
            }
            if (BaseScript.TweenTypes == XTweenTypes.填充_Fill)
            {
                if (sp_Target_Image.objectReferenceValue == null)
                {
                    valid = false;
                    XGUI.dialog(
                        type: XGUIDialogType.警告,
                        windowtitle: "XTween动画控制器消息",
                        title: "图像组件异常",
                        msg: $"检测到您未正确指定对应动画所需要的\"<color={hexcol}>  Image </color>\"组件!，请正确指定后再预览！",
                        ok: "明白",
                        PrimaryIndex: 0);
                }
            }
            if (BaseScript.TweenTypes == XTweenTypes.平铺_Tiled)
            {
                if (sp_Target_Image.objectReferenceValue == null)
                {
                    valid = false;
                    XGUI.dialog(
                        type: XGUIDialogType.警告,
                        windowtitle: "XTween动画控制器消息",
                        title: "图像组件异常",
                        msg: $"检测到您未正确指定对应动画所需要的\"<color={hexcol}>  Image </color>\"组件!，请正确指定后再预览！",
                        ok: "明白",
                        PrimaryIndex: 0);
                }
            }
            if (BaseScript.TweenTypes == XTweenTypes.震动_Shake)
            {
                if (sp_Target_RectTransform.objectReferenceValue == null)
                {
                    valid = false;
                    XGUI.dialog(
                        type: XGUIDialogType.警告,
                        windowtitle: "XTween动画控制器消息",
                        title: "变换组件异常",
                        msg: $"检测到您未正确指定对应动画所需要的\"<color={hexcol}>  RectTransform </color>\"组件!，请正确指定后再预览！",
                        ok: "明白",
                        PrimaryIndex: 0);
                }
            }
            if (BaseScript.TweenTypes == XTweenTypes.路径_Path)
            {
                if (BaseScript.Target_PathTool == null)
                {
                    valid = false;
                    EditorApplication.delayCall += () =>
                    {
                        XGUI.dialog(
                            type: XGUIDialogType.警告,
                            windowtitle: "XTween动画控制器消息",
                            title: "路径工具组件异常",
                            msg: $"检测到您未正确指定对应动画所需要的\"<color={hexcol}>  PathTool </color>\"组件!，请正确指定后再预览！",
                            ok: "明白",
                            PrimaryIndex: 0);
                    };
                }
                else if (BaseScript.Target_PathTool.GetPathPointCount() <= 0)
                {
                    valid = false;
                    EditorApplication.delayCall += () =>
                    {
                        XGUI.dialog(
                            type: XGUIDialogType.警告,
                            windowtitle: "XTween动画控制器消息",
                            title: "路径工具组件异常",
                            msg: $"检测到您正使用路径动画，但是路径却没有任何\"<color={hexcol}> 路径点 </color>\"，请正确创建路径点后再预览！",
                            ok: "明白",
                            PrimaryIndex: 0);
                    };
                }
                else if (BaseScript.Target_PathTool.GetPathPointCount() < 2)
                {
                    valid = false;
                    EditorApplication.delayCall += () =>
                    {
                        XGUI.dialog(
                            type: XGUIDialogType.警告,
                            windowtitle: "XTween动画控制器消息",
                            title: "路径工具组件异常",
                            msg: $"检测到您正使用路径动画，但是路径却只有\"<color={hexcol}> 1个路径点 </color>\"，请确保最少创建\"<color={hexcol}> 2个路径点 </color>\"后再预览！",
                            ok: "明白",
                            PrimaryIndex: 0);
                    };
                }
            }

            return valid;
        }
        /// <summary>
        /// 获取目标组件
        /// </summary>
        private void GetComponents()
        {
            if (Targets_Selected())
            {
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    // 内部已处理TMPro的条件编译
                    SelectedObjects[i].GetComponents();
                }
            }
            else
            {
                // 内部已处理TMPro的条件编译
                BaseScript.GetComponents();
            }
        }
        /// <summary>
        /// 根据字符串识别动画方式的枚举值
        /// </summary>
        private void RecognizedTweenTypes()
        {
            if (Targets_Selected())
            {
                // 多选时，为每个对象记录 Undo
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    Undo.RecordObject(SelectedObjects[i], "修改动画类型");
                    SelectedObjects[i].RecognizedTweenTypes();
                    EditorUtility.SetDirty(SelectedObjects[i]); // 标记为脏，确保保存
                }
            }
            else
            {
                // 单选时，记录 Undo
                Undo.RecordObject(BaseScript, "修改动画类型");
                BaseScript.RecognizedTweenTypes();
                EditorUtility.SetDirty(BaseScript);
            }
        }
        #endregion

        #region 预设操作
        /// <summary>
        /// 预设保存
        /// </summary>
        private void Preset_Load()
        {
            Editor_XTween_PresetsCentral.OpenXTweenPresetsCentral();

            if (BaseScript.act_onPreset_loaded != null)
                BaseScript.act_onPreset_loaded();
        }

        /// <summary>
        /// 预设读取
        /// </summary>
        private void Preset_Save()
        {
            EditorApplication.delayCall += () =>
            {
                dialog_listdata res = XGUI.dialog_submit(
                    type: XGUIDialogType.修改,
                    windowtitle: "XTween动画控制器消息",
                    title: "保存动画预设",
                    msg: "是否确认要将当前的动画参数保存为动画预设？",
                    ok: "保存",
                    cancel: "暂不",
                    arg_name_text: "预设名称",
                    arg_data_text: "预设说明",
                    themecolor: XTween_Dashboard.Theme_Primary,
                    PrimaryIndex: 0);
                if (string.IsNullOrEmpty(res.name) && string.IsNullOrEmpty(res.description))
                {
                    return;
                }
                if (res.state == "暂不")
                {
                    return;
                }

                #region 检查预设文件
                XTween_PresetManager.preset_JsonFile_Checker();
                #endregion

                #region 预设参数收集
                // 预设标题
                string pre_name = res.name;
                // 预设解释
                string pre_des = res.description;
                #endregion

                #region 检查预设重名状况并做出相应逻辑

                // 提示文字重点部分文字颜色
                string keycol = XGUI_Utilitys.Color_To_HexString(XTween_Dashboard.Theme_Primary, true);

                // 检查重名
                bool isExist = XTween_PresetManager.preset_Check_NameExists(BaseScript.TweenTypes, pre_name);
                if (isExist)
                {
                    string cdr = XGUI.dialog(
                        type: XGUIDialogType.警告,
                        windowtitle: "XTween预设管理器消息",
                        title: $"预设重名 {pre_name}",
                        msg: "您当前保存的预设在库中已存在，是否确认要用当前的动画参数覆盖重名预设？",
                        ok: "覆盖",
                        cancel: "暂不",
                        themecolor: XTween_Dashboard.Theme_Primary,
                        PrimaryIndex: 0);
                    if (cdr == "覆盖")
                    {
                        // 覆盖模式：先删除现有同名预设
                        XTween_PresetManager.preset_Delete_ByName(BaseScript.TweenTypes, pre_name);
                        // 自动推断类型保存，覆盖方式
                        if (BaseScript.preset_Save_From_Controller(pre_name, pre_des))
                        {
                            string v = XGUI.dialog(
                                type: XGUIDialogType.确认,
                                windowtitle: "XTween预设管理器消息",
                                title: "预设保存完成",
                                msg: $"已将<color={keycol}> {pre_name} </color>的预设存入<color={keycol}> {BaseScript.TweenTypes} </color>预设库中，是否打开预设库查看？",
                                ok: "查看",
                                cancel: "暂不",
                                themecolor: XTween_Dashboard.Theme_Primary,
                                PrimaryIndex: 0);
                            if (v == "查看")
                            {
                                // 打开预设库面板进行管理
                                Editor_XTween_PresetsCentral.OpenXTweenPresetsCentral();
                            }
                        }
                        if (XTween_PresetManager.EnableDebugLogs)
                            XGUI_Utilitys.Console("XTween预设管理器消息", $"预设名称 '{pre_name}' 已存在，将覆盖保存到预设库！", XGUIMsgState.警告);

                        if (BaseScript.act_onPreset_saved != null)
                            BaseScript.act_onPreset_saved();
                    }
                }
                else
                {
                    // 自动推断类型保存，新增方式
                    if (BaseScript.preset_Save_From_Controller(pre_name, pre_des))
                    {
                        string v = XGUI.dialog(
                            type: XGUIDialogType.确认,
                            windowtitle: "XTween预设管理器消息",
                            title: "预设保存完成",
                            msg: $"已将<color={keycol}> {pre_name} </color>的预设存入<color={keycol}> {BaseScript.TweenTypes} </color>预设库中，是否打开预设库查看？",
                            ok: "查看",
                            cancel: "暂不",
                            themecolor: XTween_Dashboard.Theme_Primary,
                            PrimaryIndex: 0);
                        if (v == "查看")
                        {
                            // 打开预设库面板进行管理
                            Editor_XTween_PresetsCentral.OpenXTweenPresetsCentral();
                        }
                    }
                    if (XTween_PresetManager.EnableDebugLogs)
                        XGUI_Utilitys.Console("XTween预设管理器消息", $"预设名称 '{pre_name}' 未在库中发现重复，已保存到预设库！", XGUIMsgState.通知);

                    if (BaseScript.act_onPreset_saved != null)
                        BaseScript.act_onPreset_saved();
                }
                #endregion
            };
        }
        #endregion
    }
}