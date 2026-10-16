/*
 * ============================================================================
 *  ⚠ 测试用途，不属于生产代码，可随时删除。
 * ============================================================================
 */
namespace SevenStrikeModules.XGUI.Editor.Tests
{
    using SevenStrikeModules.XGUI.Editor;
    using SevenStrikeModules.XGUI.Runtime;
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// XGUI 三个编辑器 Player 的统一测试窗口。
    /// </summary>
    public class Editor_XGUI_Samples_EditorPlayer : EditorWindow
    {
        private enum Tab { Values, Sound, Sequence }
        private Tab currentTab = Tab.Values;

        // ── Values 面板状态 ──
        private float boxWidth;
        private Color gradientColor = Color.red;
        private Vector3 movingPos;
        private int counterValue;

        private int v_stepCount = 80;
        private float v_interval = 1f / 125f;
        private int v_loopCount = -1;
        private bool v_pingPong = true;
        private XGUIEaseMode v_ease = XGUIEaseMode.InOutCubic;
        private bool v_useCustomCurve = false;
        private AnimationCurve v_customCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        // ── Sound 面板状态 ──
        [SerializeField]
        private AudioClip s_clip;
        [SerializeField]
        private float s_delay = 0.5f;
        [SerializeField]
        private float s_volume = 1f;
        [SerializeField]
        private bool s_mute = false;
        [SerializeField]
        private bool s_useRandomPitch = false;
        [SerializeField]
        private float s_pitchMin = 0.95f;
        [SerializeField]
        private float s_pitchMax = 1.05f;
        [SerializeField]
        private int s_batchCount = 5;
        [SerializeField]
        private float s_batchInterval = 0.15f;

        // ── Sequence 面板状态 ──
        [SerializeField]
        private int seq_frameCount = 8;
        [SerializeField]
        private float seq_interval = 0.1f;
        [SerializeField]
        private int seq_loopCount = -1;
        [SerializeField]
        private int seq_currentIndex = 0;
        private List<Color> seq_palette = new List<Color>();

        private Vector2 scroll;

        // ── 折叠状态 ──
        private bool fold_values_params = true;
        private bool fold_values_controls = true;
        private bool fold_values_previews = true;
        private bool fold_values_status = true;

        private bool fold_sound_params = true;
        private bool fold_sound_controls = true;
        private bool fold_sound_status = true;

        private bool fold_seq_params = true;
        private bool fold_seq_controls = true;
        private bool fold_seq_preview = true;

        // ── 样式缓存 ──
        private string hexcolor_primary;
        private Color color_primary;
        private Color color_primary_off;

        SerializedObject serializedObject;

        // ============================================================
        //  生命周期
        // ============================================================
        [MenuItem("Tools/XGUI/Sample/EditorPlayer 完整测试窗口")]
        private static void Open()
        {
            var win = GetWindow<Editor_XGUI_Samples_EditorPlayer>();
            win.titleContent = new GUIContent("XGUI EditorPlayer 测试");
            win.minSize = new Vector2(350, 720);
        }

        private void OnEnable()
        {
            color_primary = XGUI_Utilitys.HexString_To_Color("3BFD9A");
            color_primary_off = Color.white * 0.65f;
            hexcolor_primary = XGUI_Utilitys.Color_To_HexString(color_primary, true);

            BuildSequencePalette();

            serializedObject = new SerializedObject(this);
        }

        private void OnDisable()
        {
            XGUI.EditorPlayer_Values.Stop();
            XGUI.EditorPlayer_Sound.Stop();
            XGUI.EditorPlayer_Sequence.Stop();
        }

        private void OnGUI()
        {
            serializedObject.Update();

            float w_width = XGUI.GetCurrentWindowWidth();
            TextClipping clipping_ellipsis = XGUI.TryEllipsisClipping();

            scroll = EditorGUILayout.BeginScrollView(scroll);

            // 左右 10 像素缩进
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(10);

                using (new EditorGUILayout.VerticalScope())
                {
                    DrawHeader(w_width, clipping_ellipsis);
                    DrawTabBar(w_width);

                    switch (currentTab)
                    {
                        case Tab.Values: DrawValuesPanel(w_width, clipping_ellipsis); break;
                        case Tab.Sound: DrawSoundPanel(w_width, clipping_ellipsis); break;
                        case Tab.Sequence: DrawSequencePanel(w_width, clipping_ellipsis); break;
                    }
                }

                GUILayout.Space(10);
            }

            EditorGUILayout.EndScrollView();

            Event e = Event.current;
            if (e.type == EventType.MouseDown && e.button == 1)
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("F 全部展开"), false, () =>
                {
                    fold_values_params = true;
                    fold_values_controls = true;
                    fold_values_previews = true;
                    fold_values_status = true;
                    fold_sound_params = true;
                    fold_sound_controls = true;
                    fold_sound_status = true;
                    fold_seq_params = true;
                    fold_seq_controls = true;
                    fold_seq_preview = true;
                    Repaint();
                });
                menu.AddItem(new GUIContent("D 全部折叠"), false, () =>
                {
                    fold_values_params = false;
                    fold_values_controls = false;
                    fold_values_previews = false;
                    fold_values_status = false;
                    fold_sound_params = false;
                    fold_sound_controls = false;
                    fold_sound_status = false;
                    fold_seq_params = false;
                    fold_seq_controls = false;
                    fold_seq_preview = false;
                    Repaint();
                });
                menu.ShowAsContext();
                e.Use();
            }

            serializedObject.ApplyModifiedProperties();
        }

        // ============================================================
        //  抬头
        // ============================================================
        private void DrawHeader(float w_width, TextClipping clipping_ellipsis)
        {
            XGUI.layout_space(6);

            XGUI.layout_banner(
                bg_fill: XGUIGroup.实体,
                bg_color: XGUIColor.深空灰,
                bg_height: 36,
                icon: XGUI.GetBasedIcon("icon_play_r"),
                icon_color: color_primary,
                icon_offset: new Vector2(0, 2),
                title_text: "XGUI  -  EditorPlayer 测试台",
                title_anchor: TextAnchor.MiddleLeft,
                title_style: FontStyle.Bold,
                title_color: Color.white,
                title_offset: new Vector2(0, 3),
                title_size: XGUIFontSize.B,
                title_clipping: clipping_ellipsis,
                bg_margin: new RectOffset(0, 0, 5, 5));

            XGUI.layout_helpbox(
                state: XGUIHelboxState.通知,
                title_text:
                    "· 顶部 Tab 切换三个 Player：Values / Sound / Sequence\n" +
                    "· Values：推 t 值，插值 float / Color / Vector3 / int\n" +
                    "· Sound：到点播一次，延迟 / 音量 / 随机音调\n" +
                    "· Sequence：到点推一帧，模拟纹理序列 / 循环 / 手动 Next",
                title_color: Color.white * 0.9f,
                wrap: true);
        }

        // ============================================================
        //  Tab 栏
        // ============================================================
        private void DrawTabBar(float w_width)
        {
            XGUI.layout_space(8);

            string[] tabs = { "数值插值", "音效调度", "序列帧" };
            int idx = (int)currentTab;

            idx = XGUI.layout_toolbar(
                     index: idx,
                     names: tabs,
                     bg_normal: XGUIGroup.无,
                     bg_selected: XGUIGroup.实体,
                     bg_color: XGUIColor.亮白,
                     bg_gui_color: Color.black * 0.5f,
                     text_color_normal: Color.white,
                     text_color_selected: color_primary,
                     bar_height: 25,
                     bar_offset: new Vector2(0, 15),
                     text_anchor: TextAnchor.MiddleCenter,
                     text_padding: new RectOffset(10, 10, 0, 0),
                     bar_margin: new RectOffset(10, 10, 5, 15),
                     text_offset: new Vector2(0, -2),
                     text_size: XGUIFontSize.M,
                     text_font: XGUI.GetFont("xg-medium"),
                     text_fontstyle: FontStyle.Bold,
                     bg_width_offset: 5,
                     bg_height_offset: 17,
                     navigate_style: true,
                     navigate_style_bg: XGUIGroup.纯色边框,
                     navigate_style_bg_color: Color.black * 0.5f);

            currentTab = (Tab)idx;

            XGUI.layout_space(8);
        }

        // ============================================================
        //  Values 面板
        // ============================================================
        private void DrawValuesPanel(float w_width, TextClipping clipping_ellipsis)
        {
            DrawValuesParams(w_width, clipping_ellipsis);
            DrawValuesControls(w_width, clipping_ellipsis);
            DrawValuesPreviews(w_width, clipping_ellipsis);
            DrawValuesStatus(w_width, clipping_ellipsis);
        }

        private void DrawValuesParams(float w_width, TextClipping clipping_ellipsis)
        {
            fold_values_params = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: Color.black * 0.3f,
                title: "Values · 参数",
                title_size: XGUIFontSize.M,
                title_text_color: color_primary,
                title_clipping: clipping_ellipsis,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: fold_values_params);

            if (fold_values_params)
            {
                v_stepCount = XGUI.layout_slider_int(
                    title: "总步数",
                    prop: v_stepCount,
                    prop_margin: new RectOffset(10, 10, 5, 0),
                    left: 2,
                    right: 240,
                    title_width: 100);

                v_interval = XGUI.layout_slider(
                    title: "每步间隔",
                    prop: v_interval,
                    prop_margin: new RectOffset(10, 10, 5, 0),
                    left: 0.001f,
                    right: 0.5f,
                    title_width: 100);

                XGUI.layout_label(
                    text: "循环次数",
                    size: XGUIFontSize.M,
                    text_color: Color.white,
                    offset: new Vector2(5, 0),
                    margin: new RectOffset(0, 0, 10, 10));

                XGUI.layout_group_start(
                    type: XGUIContainerType.Horizontal,
                    absolute_margin: true,
                    absolute_padding: true,
                    margin: new RectOffset(0, 0, 0, 0),
                    padding: new RectOffset(0, 0, 0, 0));

                if (XGUI.layout_button("无限\n (-1)", height: 42, bg_color_gui: color_primary, press_fill: XGUIGroup.实体, press_color: XGUIColor.深空灰, button_text_color: Color.black, press_text_color: Color.white)) v_loopCount = -1;
                if (XGUI.layout_button("1遍\n (0)", height: 42, bg_color_gui: color_primary, press_fill: XGUIGroup.实体, press_color: XGUIColor.深空灰, button_text_color: Color.black, press_text_color: Color.white)) v_loopCount = 0;
                if (XGUI.layout_button("2遍\n (1)", height: 42, bg_color_gui: color_primary, press_fill: XGUIGroup.实体, press_color: XGUIColor.深空灰, button_text_color: Color.black, press_text_color: Color.white)) v_loopCount = 1;
                if (XGUI.layout_button("3遍\n (2)", height: 42, bg_color_gui: color_primary, press_fill: XGUIGroup.实体, press_color: XGUIColor.深空灰, button_text_color: Color.black, press_text_color: Color.white)) v_loopCount = 2;

                XGUI.layout_group_end(XGUIContainerType.Horizontal);

                v_loopCount = XGUI.layout_inputfield(
                    title: "循环次数",
                    prop: v_loopCount,
                    title_width: 100,
                    field_margin: new RectOffset(0, 0, 10, 10),
                    field_text_color: Color.white);

                v_pingPong = XGUI.layout_toggle(
                    title: "往返模式",
                    title_width: 100,
                    prop: v_pingPong,
                    tog_bg_off_color: color_primary_off,
                    tog_bg_on_color: color_primary,
                    tog_margin: new RectOffset(15, 0, 5, 0),
                    tog_padding: new RectOffset(5, 5, 0, 0),
                    tog_handler_off_color: Color.white,
                    tog_handler_on_color: Color.white);

                v_useCustomCurve = XGUI.layout_toggle(
                    title: "使用自定义曲线",
                    title_width: 100,
                    prop: v_useCustomCurve,
                    tog_bg_off_color: color_primary_off,
                    tog_bg_on_color: color_primary,
                    tog_margin: new RectOffset(15, 0, 5, 5),
                    tog_padding: new RectOffset(5, 5, 0, 0),
                    tog_handler_off_color: Color.white,
                    tog_handler_on_color: Color.white);

                if (v_useCustomCurve)
                {
                    v_customCurve = XGUI.layout_inputfield(
                        title: "自定义曲线",
                        prop: v_customCurve,
                        field_margin: new RectOffset(0, 0, 5, 5),
                        field_padding: new RectOffset(0, 0, 0, 0),
                        title_width: 100);
                }
                else
                {
                    string[] easeNames = Enum.GetNames(typeof(XGUIEaseMode));
                    string currentName = v_ease.ToString();
                    string picked = XGUI.layout_string_popup(
                        title: "缓动模式",
                        title_width: 100,
                        prop: currentName,
                        options: easeNames,
                        margin: new RectOffset(0, 0, 0, 0),
                        padding: new RectOffset(5, 0, 0, 0),
                        opt_text_color: Color.black,
                        opt_bg_color_gui: color_primary,
                        icon_arrow_color: Color.black);

                    if (picked != currentName)
                        v_ease = (XGUIEaseMode)Enum.Parse(typeof(XGUIEaseMode), picked);
                }
            }

            XGUI.layout_group_end(XGUIContainerType.Vertical);
        }

        private void DrawValuesControls(float w_width, TextClipping clipping_ellipsis)
        {
            XGUI.layout_space(8);

            fold_values_controls = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: Color.black * 0.3f,
                title: "Values · 控制",
                title_size: XGUIFontSize.M,
                title_text_color: color_primary,
                title_clipping: clipping_ellipsis,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: fold_values_controls);

            if (fold_values_controls)
            {
                // 计算按钮宽度：让每行两个按钮撑满
                float btn_w = (w_width - 60) / 2;

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (XGUI.layout_button("▶   进度条 (Float)", height: 26, font_size: XGUIFontSize.M, layout_width: btn_w, bg_color_gui: color_primary, press_fill: XGUIGroup.实体, press_color: XGUIColor.深空灰, button_text_color: Color.black, press_text_color: Color.white)) PlayFloat();
                    if (XGUI.layout_button("▶   渐变颜色 (Color)", height: 26, font_size: XGUIFontSize.M, layout_width: btn_w, bg_color_gui: color_primary, press_fill: XGUIGroup.实体, press_color: XGUIColor.深空灰, button_text_color: Color.black, press_text_color: Color.white)) PlayColor();
                }
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (XGUI.layout_button("▶   滑动位置 (Vector3)", height: 26, font_size: XGUIFontSize.M, layout_width: btn_w, bg_color_gui: color_primary, press_fill: XGUIGroup.实体, press_color: XGUIColor.深空灰, button_text_color: Color.black, press_text_color: Color.white)) PlayVector3();
                    if (XGUI.layout_button("▶   计数器 (Int)", height: 26, font_size: XGUIFontSize.M, layout_width: btn_w, bg_color_gui: color_primary, press_fill: XGUIGroup.实体, press_color: XGUIColor.深空灰, button_text_color: Color.black, press_text_color: Color.white)) PlayInt();
                }
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (XGUI.layout_button("■   停止", height: 26, font_size: XGUIFontSize.M, layout_width: btn_w, bg_color_gui: color_primary, press_fill: XGUIGroup.实体, press_color: XGUIColor.深空灰, button_text_color: Color.black, press_text_color: Color.white)) XGUI.EditorPlayer_Values.Stop();
                    if (XGUI.layout_button("■   重置值", height: 26, font_size: XGUIFontSize.M, layout_width: btn_w, bg_color_gui: color_primary, press_fill: XGUIGroup.实体, press_color: XGUIColor.深空灰, button_text_color: Color.black, press_text_color: Color.white)) ResetValues();
                }
            }

            XGUI.layout_group_end(XGUIContainerType.Vertical);
        }

        private void DrawValuesPreviews(float w_width, TextClipping clipping_ellipsis)
        {
            XGUI.layout_space(10);

            fold_values_previews = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: Color.black * 0.3f,
                title: "Values · 预览",
                title_size: XGUIFontSize.M,
                title_text_color: color_primary,
                title_clipping: clipping_ellipsis,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: fold_values_previews);

            if (fold_values_previews)
            {
                XGUI.layout_label(text: $"进度条 (Float)：{boxWidth:F1} px", size: XGUIFontSize.M, text_color: Color.white, margin: new RectOffset(0, 0, 0, 10));
                Rect boxRect = XGUI.GetControlRect(false, 22);
                XGUI.gui_box(boxRect, new Color(0.15f, 0.15f, 0.15f, 1f));
                XGUI.gui_box(new Rect(boxRect.x, boxRect.y, Mathf.Clamp01(boxWidth / 400f) * boxRect.width, boxRect.height), new Color(0.2f, 0.6f, 1f, 1f));

                XGUI.layout_space(8);

                XGUI.layout_label(text: $"渐变颜色 (Color)：#{ColorUtility.ToHtmlStringRGBA(gradientColor)}", size: XGUIFontSize.M, text_color: Color.white, margin: new RectOffset(0, 0, 0, 10));
                Rect colorRect = XGUI.GetControlRect(false, 22);
                XGUI.gui_box(colorRect, gradientColor);

                XGUI.layout_space(8);

                XGUI.layout_label(text: $"滑动位置 (Vector3)：{movingPos}", size: XGUIFontSize.M, text_color: Color.white, margin: new RectOffset(0, 0, 0, 10));
                Rect trackRect = XGUI.GetControlRect(false, 22);
                XGUI.gui_box(trackRect, new Color(0.15f, 0.15f, 0.15f, 1f));
                float dotX = trackRect.x + Mathf.Clamp01(movingPos.x / 200f) * trackRect.width;
                XGUI.gui_box(new Rect(dotX, trackRect.y, 12, trackRect.height), new Color(1f, 0.85f, 0.2f, 1f));

                XGUI.layout_space(8);

                XGUI.layout_label(text: $"计数器 (Int)：{counterValue}", size: XGUIFontSize.M, text_color: Color.white, margin: new RectOffset(0, 0, 0, 10));
                Rect counterRect = XGUI.GetControlRect(false, 22);
                XGUI.gui_box(counterRect, new Color(0.15f, 0.15f, 0.15f, 1f));
                float counterFill = Mathf.Clamp01(counterValue / 100f) * counterRect.width;
                XGUI.gui_box(new Rect(counterRect.x, counterRect.y, counterFill, counterRect.height), new Color(0.4f, 0.9f, 0.4f, 1f));
            }

            XGUI.layout_group_end(XGUIContainerType.Vertical);
        }

        private void DrawValuesStatus(float w_width, TextClipping clipping_ellipsis)
        {
            XGUI.layout_space(10);

            fold_values_status = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: Color.black * 0.3f,
                title: "Values · 状态",
                title_size: XGUIFontSize.M,
                title_text_color: color_primary,
                title_clipping: clipping_ellipsis,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: fold_values_status);

            if (fold_values_status)
            {
                XGUI.layout_state_displayer_text(title: "正在播放中", subtitle: XGUI.EditorPlayer_Values.IsPlaying.ToString(), subtitle_color: color_primary, margin: new RectOffset(5, 5, 0, 5));
                XGUI.layout_state_displayer_text(title: "正在推进中", subtitle: XGUI.EditorPlayer_Values.IsTicking.ToString(), subtitle_color: color_primary, margin: new RectOffset(5, 5, 0, 5));
                XGUI.layout_state_displayer_text(title: "当前进度值", subtitle: XGUI.EditorPlayer_Values.CurrentT.ToString("F4"), subtitle_color: color_primary, margin: new RectOffset(5, 5, 0, 5));
                XGUI.layout_state_displayer_text(title: "当前进度值（原始）", subtitle: XGUI.EditorPlayer_Values.CurrentRawT.ToString("F4"), subtitle_color: color_primary, margin: new RectOffset(5, 5, 0, 5));
            }

            XGUI.layout_group_end(XGUIContainerType.Vertical);
        }

        // ============================================================
        //  Sound 面板
        // ============================================================
        private void DrawSoundPanel(float w_width, TextClipping clipping_ellipsis)
        {
            DrawSoundParams(w_width, clipping_ellipsis);
            DrawSoundControls(w_width, clipping_ellipsis);
            DrawSoundStatus(w_width, clipping_ellipsis);
        }

        private void DrawSoundParams(float w_width, TextClipping clipping_ellipsis)
        {
            fold_sound_params = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: Color.black * 0.3f,
                title: "Sound · 参数",
                title_size: XGUIFontSize.M,
                title_text_color: color_primary,
                title_clipping: clipping_ellipsis,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: fold_sound_params);

            if (fold_sound_params)
            {
                XGUI.layout_property_field(
                    title: "音频剪辑",
                    title_size: XGUIFontSize.M,
                    title_hover_color: color_primary,
                    title_width: 100,
                    prop: serializedObject.FindProperty("s_clip"),
                    prop_margin: new RectOffset(0, 0, 5, 0));

                XGUI.layout_property_field(
                    title: "延迟（秒）",
                    title_size: XGUIFontSize.M,
                    title_hover_color: color_primary,
                    title_width: 100,
                    prop: serializedObject.FindProperty("s_delay"),
                    prop_margin: new RectOffset(0, 0, 5, 0));

                XGUI.layout_property_field(
                    title: "音量（0-1）",
                    title_size: XGUIFontSize.M,
                    title_hover_color: color_primary,
                    title_width: 100,
                    prop: serializedObject.FindProperty("s_volume"),
                    prop_margin: new RectOffset(0, 0, 5, 0));
                s_volume = Mathf.Clamp01(s_volume);

                s_mute = XGUI.layout_toggle(
                    title: "静音",
                    title_width: 100,
                    prop: s_mute,
                    tog_bg_off_color: color_primary_off,
                    tog_bg_on_color: color_primary,
                    tog_margin: new RectOffset(15, 0, 10, 0),
                    tog_padding: new RectOffset(5, 5, 0, 0),
                    tog_handler_off_color: Color.white,
                    tog_handler_on_color: Color.white);

                s_useRandomPitch = XGUI.layout_toggle(
                    title: "随机音高",
                    title_width: 100,
                    prop: s_useRandomPitch,
                    tog_bg_off_color: color_primary_off,
                    tog_bg_on_color: color_primary,
                    tog_margin: new RectOffset(15, 0, 5, 5),
                    tog_padding: new RectOffset(5, 5, 0, 0),
                    tog_handler_off_color: Color.white,
                    tog_handler_on_color: Color.white);

                if (s_useRandomPitch)
                {
                    XGUI.layout_property_field(
                        title: "下限",
                        title_size: XGUIFontSize.M,
                        title_hover_color: color_primary,
                        title_width: 100,
                        prop: serializedObject.FindProperty("s_pitchMin"),
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    s_pitchMin = Mathf.Clamp01(s_pitchMin);

                    XGUI.layout_property_field(
                        title: "上限",
                        title_size: XGUIFontSize.M,
                        title_hover_color: color_primary,
                        title_width: 100,
                        prop: serializedObject.FindProperty("s_pitchMax"),
                        prop_margin: new RectOffset(0, 0, 5, 0));
                    s_pitchMax = Mathf.Clamp01(s_pitchMax);
                }
            }

            XGUI.layout_group_end(XGUIContainerType.Vertical);
        }

        private void DrawSoundControls(float w_width, TextClipping clipping_ellipsis)
        {
            XGUI.layout_space(8);

            fold_sound_controls = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: Color.black * 0.3f,
                title: "Sound · 控制",
                title_size: XGUIFontSize.M,
                title_text_color: color_primary,
                title_clipping: clipping_ellipsis,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: fold_sound_controls);

            if (fold_sound_controls)
            {
                float btn_w = (w_width - 60) / 2;

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (XGUI.layout_button("▶   立即播放 (Delay = 0)", height: 26, font_size: XGUIFontSize.M, layout_width: btn_w, bg_color_gui: color_primary, press_fill: XGUIGroup.实体, press_color: XGUIColor.深空灰, button_text_color: Color.black, press_text_color: Color.white))
                        EnqueueSound(0f);
                    if (XGUI.layout_button($"▶   立即播放 (Delay = {s_delay})", height: 26, font_size: XGUIFontSize.M, layout_width: btn_w, bg_color_gui: color_primary, press_fill: XGUIGroup.实体, press_color: XGUIColor.深空灰, button_text_color: Color.black, press_text_color: Color.white))
                        EnqueueSound(s_delay);
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (XGUI.layout_button("▶   批量播放", height: 26, font_size: XGUIFontSize.M, layout_width: btn_w, bg_color_gui: color_primary, press_fill: XGUIGroup.实体, press_color: XGUIColor.深空灰, button_text_color: Color.black, press_text_color: Color.white))
                        EnqueueSoundBatch();

                    if (XGUI.layout_button("■   停止全部", height: 26, font_size: XGUIFontSize.M, layout_width: btn_w, bg_color_gui: color_primary, press_fill: XGUIGroup.实体, press_color: XGUIColor.深空灰, button_text_color: Color.black, press_text_color: Color.white))
                        XGUI.EditorPlayer_Sound.Stop();
                }

                XGUI.layout_space(4);

                XGUI.layout_property_field(
                    title: "批量数量",
                    title_size: XGUIFontSize.M,
                    title_hover_color: color_primary,
                    title_width: 100,
                    prop: serializedObject.FindProperty("s_batchCount"),
                    prop_margin: new RectOffset(0, 0, 5, 0));

                XGUI.layout_property_field(
                    title: "批量间隔 (秒)",
                    title_size: XGUIFontSize.M,
                    title_hover_color: color_primary,
                    title_width: 100,
                    prop: serializedObject.FindProperty("s_batchInterval"),
                    prop_margin: new RectOffset(0, 0, 5, 0));
            }

            XGUI.layout_group_end(XGUIContainerType.Vertical);
        }

        private void DrawSoundStatus(float w_width, TextClipping clipping_ellipsis)
        {
            XGUI.layout_space(10);

            fold_sound_status = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: Color.black * 0.3f,
                title: "Sound · 说明",
                title_size: XGUIFontSize.M,
                title_text_color: color_primary,
                title_clipping: clipping_ellipsis,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: fold_sound_status);

            if (fold_sound_status)
            {
                XGUI.layout_helpbox(
                    state: XGUIHelboxState.通知,
                    title_text:
                        "· EditorPlayer_Sound 是“到点播一次”的调度器。\n" +
                        "· 它自己不管取 clip / 算音量，一切由调用方传入。\n" +
                        "· 音效播完后立即销毁临时 GameObject。\n" +
                        "· 想看当前播放状态，看 Unity 的 Hierarchy 里 XHud_TweenPreviewSound-* 临时物体。",
                    title_color: Color.white * 0.85f,
                    wrap: true);
            }

            XGUI.layout_group_end(XGUIContainerType.Vertical);
        }

        // ============================================================
        //  Sequence 面板
        // ============================================================
        private void DrawSequencePanel(float w_width, TextClipping clipping_ellipsis)
        {
            DrawSequenceParams(w_width, clipping_ellipsis);
            DrawSequenceControls(w_width, clipping_ellipsis);
            DrawSequencePreview(w_width, clipping_ellipsis);
        }

        private void DrawSequenceParams(float w_width, TextClipping clipping_ellipsis)
        {
            fold_seq_params = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: Color.black * 0.3f,
                title: "Sequence · 参数",
                title_size: XGUIFontSize.M,
                title_text_color: color_primary,
                title_clipping: clipping_ellipsis,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: fold_seq_params);

            if (fold_seq_params)
            {
                XGUI.layout_property_field(
                    title: "帧数量",
                    title_size: XGUIFontSize.M,
                    title_hover_color: color_primary,
                    title_width: 100,
                    prop: serializedObject.FindProperty("seq_frameCount"),
                    prop_margin: new RectOffset(0, 0, 5, 0));
                seq_frameCount = Mathf.Clamp(seq_frameCount, 2, 32);

                if (seq_palette.Count != seq_frameCount)
                    BuildSequencePalette();

                XGUI.layout_property_field(
                    title: "帧间隔（秒）",
                    title_size: XGUIFontSize.M,
                    title_hover_color: color_primary,
                    title_width: 100,
                    prop: serializedObject.FindProperty("seq_interval"),
                    prop_margin: new RectOffset(0, 0, 5, 0));
                seq_interval = Mathf.Clamp(seq_interval, 0.01f, 1f);

                XGUI.layout_label(
                     text: "循环次数",
                     size: XGUIFontSize.M,
                     text_color: Color.white,
                     offset: new Vector2(5, 0),
                     margin: new RectOffset(0, 0, 10, 10));

                XGUI.layout_group_start(
                    type: XGUIContainerType.Horizontal,
                    absolute_margin: true,
                    absolute_padding: true,
                    margin: new RectOffset(0, 0, 0, 0),
                    padding: new RectOffset(0, 0, 0, 0));

                if (XGUI.layout_button("无限\n (-1)", height: 42, bg_color_gui: color_primary, press_fill: XGUIGroup.实体, press_color: XGUIColor.深空灰, button_text_color: Color.black, press_text_color: Color.white)) seq_loopCount = -1;
                if (XGUI.layout_button("1遍\n (0)", height: 42, bg_color_gui: color_primary, press_fill: XGUIGroup.实体, press_color: XGUIColor.深空灰, button_text_color: Color.black, press_text_color: Color.white)) seq_loopCount = 0;
                if (XGUI.layout_button("2遍\n (1)", height: 42, bg_color_gui: color_primary, press_fill: XGUIGroup.实体, press_color: XGUIColor.深空灰, button_text_color: Color.black, press_text_color: Color.white)) seq_loopCount = 1;
                if (XGUI.layout_button("3遍\n (2)", height: 42, bg_color_gui: color_primary, press_fill: XGUIGroup.实体, press_color: XGUIColor.深空灰, button_text_color: Color.black, press_text_color: Color.white)) seq_loopCount = 2;

                XGUI.layout_group_end(XGUIContainerType.Horizontal);

                seq_loopCount = XGUI.layout_inputfield(
                    title: "循环次数",
                    prop: seq_loopCount,
                    title_width: 100,
                    field_margin: new RectOffset(0, 0, 10, 10),
                    field_text_color: Color.white);
            }

            XGUI.layout_group_end(XGUIContainerType.Vertical);
        }

        private void DrawSequenceControls(float w_width, TextClipping clipping_ellipsis)
        {
            XGUI.layout_space(8);

            fold_seq_controls = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: Color.black * 0.3f,
                title: "Sequence · 控制",
                title_size: XGUIFontSize.M,
                title_text_color: color_primary,
                title_clipping: clipping_ellipsis,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: fold_seq_controls);

            if (fold_seq_controls)
            {
                float btn_w = (w_width - 60) / 2;

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (XGUI.layout_button("▶   播放序列（模拟纹理）", height: 26, font_size: XGUIFontSize.M, layout_width: btn_w, bg_color_gui: color_primary, press_fill: XGUIGroup.实体, press_color: XGUIColor.深空灰, button_text_color: Color.black, press_text_color: Color.white))
                        PlaySequence();
                    if (XGUI.layout_button("■   停止序列", height: 26, font_size: XGUIFontSize.M, layout_width: btn_w, bg_color_gui: color_primary, press_fill: XGUIGroup.实体, press_color: XGUIColor.深空灰, button_text_color: Color.black, press_text_color: Color.white))
                        XGUI.EditorPlayer_Sequence.Stop();
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (XGUI.layout_button("▶ 手动 Next", height: 26, font_size: XGUIFontSize.M, layout_width: btn_w, bg_color_gui: color_primary, press_fill: XGUIGroup.实体, press_color: XGUIColor.深空灰, button_text_color: Color.black, press_text_color: Color.white))
                    {
                        var dummy = BuildDummyTextures();
                        XGUI.EditorPlayer_Sequence.Play(dummy, seq_interval,
                            () =>
                            {
                                seq_currentIndex = XGUI.EditorPlayer_Sequence.CurrentIndex;
                                Repaint();
                            },
                            loopCount: seq_loopCount);
                        XGUI.EditorPlayer_Sequence.Next();
                    }
                    if (XGUI.layout_button("■ 重置索引", height: 26, font_size: XGUIFontSize.M, layout_width: btn_w, bg_color_gui: color_primary, press_fill: XGUIGroup.实体, press_color: XGUIColor.深空灰, button_text_color: Color.black, press_text_color: Color.white))
                    {
                        seq_currentIndex = 0;
                        Repaint();
                    }
                }
            }

            XGUI.layout_group_end(XGUIContainerType.Vertical);
        }

        private void DrawSequencePreview(float w_width, TextClipping clipping_ellipsis)
        {
            XGUI.layout_space(10);

            fold_seq_preview = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: Color.black * 0.3f,
                title: "Sequence · 预览",
                title_size: XGUIFontSize.M,
                title_text_color: color_primary,
                title_clipping: clipping_ellipsis,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: fold_seq_preview);

            if (fold_seq_preview)
            {
                XGUI.layout_label(text: $"当前帧索引：{seq_currentIndex} / {seq_frameCount - 1}", size: XGUIFontSize.M, text_color: Color.white, margin: new RectOffset(0, 0, 5, 10));

                Rect frameRect = XGUI.GetControlRect(false, 60);
                Color col = seq_palette.Count > 0 ? seq_palette[Mathf.Clamp(seq_currentIndex, 0, seq_palette.Count - 1)] : Color.gray;
                XGUI.gui_box(frameRect, col);

                XGUI.layout_space(8);

                XGUI.layout_label(text: "帧指示器：", size: XGUIFontSize.M, text_color: Color.white * 0.8f, margin: new RectOffset(0, 0, 0, 4));

                Rect dotRow = XGUI.GetControlRect(false, 16);
                float dotW = dotRow.width / Mathf.Max(1, seq_frameCount);
                for (int i = 0; i < seq_frameCount; i++)
                {
                    Rect dot = new Rect(dotRow.x + i * dotW + dotW / 2 - 3, dotRow.y + 4, 6, 6);
                    Color dotColor = (i == seq_currentIndex) ? color_primary : Color.gray;
                    XGUI.gui_box(dot, dotColor);
                }

                XGUI.layout_space(8);

                XGUI.layout_helpbox(
                    state: XGUIHelboxState.通知,
                    title_text:
                        "· EditorPlayer_Sequence 接收 List<Texture2D>，按 interval 推进索引。\n" +
                        "· 本 Demo 用空纹理占位（真实场景传序列帧资源）。\n" +
                        "· 点“播放序列”后帧指示器会动；点“手动 Next”可逐帧推进。",
                    title_color: Color.white * 0.85f,
                    wrap: true);
            }

            XGUI.layout_group_end(XGUIContainerType.Vertical);
        }

        // ============================================================
        //  播放方法
        // ============================================================
        private XGUIEaseMode CurrentEase => v_useCustomCurve ? XGUIEaseMode.Linear : v_ease;
        private AnimationCurve CurrentCurve => v_useCustomCurve ? v_customCurve : null;

        private void PlayFloat()
        {
            XGUI.EditorPlayer_Values.Play(
                from: 0f, to: 400f,
                lerp: (a, b, t) => Mathf.Lerp(a, b, t),
                stepCount: v_stepCount, interval: v_interval,
                onStep: (t, v) => { boxWidth = v; Repaint(); },
                loopCount: v_loopCount,
                pingPong: v_pingPong,
                ease: CurrentEase,
                customCurve: CurrentCurve);
        }

        private void PlayColor()
        {
            XGUI.EditorPlayer_Values.Play(
                from: Color.red, to: Color.blue,
                lerp: (a, b, t) => Color.Lerp(a, b, t),
                stepCount: v_stepCount, interval: v_interval,
                onStep: (t, v) => { gradientColor = v; Repaint(); },
                loopCount: v_loopCount,
                pingPong: v_pingPong,
                ease: CurrentEase,
                customCurve: CurrentCurve);
        }

        private void PlayVector3()
        {
            XGUI.EditorPlayer_Values.Play(
                from: Vector3.zero, to: new Vector3(200f - 6, 0f, 0f),
                lerp: (a, b, t) => Vector3.Lerp(a, b, t),
                stepCount: v_stepCount, interval: v_interval,
                onStep: (t, v) => { movingPos = v; Repaint(); },
                loopCount: v_loopCount,
                pingPong: v_pingPong,
                ease: CurrentEase,
                customCurve: CurrentCurve);
        }

        private void PlayInt()
        {
            XGUI.EditorPlayer_Values.Play(
                from: 0, to: 100,
                lerp: (a, b, t) => Mathf.RoundToInt(Mathf.Lerp(a, b, t)),
                stepCount: v_stepCount, interval: v_interval,
                onStep: (t, v) => { counterValue = v; Repaint(); },
                loopCount: v_loopCount,
                pingPong: v_pingPong,
                ease: CurrentEase,
                customCurve: CurrentCurve);
        }

        private void ResetValues()
        {
            boxWidth = 0f;
            gradientColor = Color.red;
            movingPos = Vector3.zero;
            counterValue = 0;
            Repaint();
        }

        // ============================================================
        //  Sound 播放方法
        // ============================================================
        private void EnqueueSound(float delay)
        {
            if (s_clip == null) { Debug.LogWarning("[EditorPlayer 测试] 未指定 AudioClip"); return; }

            XGUI.EditorPlayer_Sound.Enqueue(
                clip: s_clip, delay: delay, volume: s_volume, mute: s_mute,
                useRandomPitch: s_useRandomPitch, pitchMin: s_pitchMin, pitchMax: s_pitchMax);
        }

        private void EnqueueSoundBatch()
        {
            if (s_clip == null) { Debug.LogWarning("[EditorPlayer 测试] 未指定 AudioClip"); return; }

            for (int i = 0; i < s_batchCount; i++)
            {
                XGUI.EditorPlayer_Sound.Enqueue(
                    clip: s_clip, delay: i * s_batchInterval, volume: s_volume, mute: s_mute,
                    useRandomPitch: s_useRandomPitch, pitchMin: s_pitchMin, pitchMax: s_pitchMax);
            }
        }

        // ============================================================
        //  Sequence 方法
        // ============================================================
        private void BuildSequencePalette()
        {
            seq_palette.Clear();
            for (int i = 0; i < seq_frameCount; i++)
            {
                float t = i / (float)Mathf.Max(1, seq_frameCount - 1);
                seq_palette.Add(Color.Lerp(Color.red, Color.blue, t));
            }
        }

        private List<Texture2D> BuildDummyTextures()
        {
            var list = new List<Texture2D>(seq_frameCount);
            for (int i = 0; i < seq_frameCount; i++) list.Add(null);
            return list;
        }

        private void PlaySequence()
        {
            var dummy = BuildDummyTextures();
            XGUI.EditorPlayer_Sequence.Play(
                frames: dummy, interval: seq_interval,
                onFrame: () => { seq_currentIndex = XGUI.EditorPlayer_Sequence.CurrentIndex; Repaint(); },
                loopCount: seq_loopCount);
        }
    }
}