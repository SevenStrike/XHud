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
    using UnityEditor;
    using UnityEngine;

    public partial class Editor_XHud_Module_Primitive_Tween_Tracker
    {
        #region 字段：播放控制

        /// <summary>
        /// 飞梭步进的时间间隔（秒），后期可调。
        /// <para/>默认 0.01 秒，用于 &lt; / &gt; 快捷键与工具栏按钮的单步跳转。
        /// </summary>
        private float playheadStepSeconds = 0.01f;

        /// <summary>
        /// 是否正在播放（播放模式下飞梭自动前进）。
        /// <para/>与 <see cref="isDraggingPlayhead"/> 互斥：
        /// 播放中拖拽飞梭会暂停播放，松开后不会自动恢复（需再次按 Space）。
        /// </summary>
        private bool isPlayheadPlaying = false;

        /// <summary>
        /// 播放模式下，上一帧飞梭所在的时间（秒）。
        /// <para/>用于按真实时间推进飞梭，避免帧率波动导致播放速度不一致。
        /// </summary>
        private double playheadLastTime = 0.0;

        /// <summary>
        /// 播放模式下的播放速率倍率（1 = 正常速度）。
        /// <para/>实际推进速度 = Editor 时间增量 × 播放速率。
        /// </para>注意：不直接使用 XHud_Module_Primitive_Tween.GlobalDuration，
        /// 因为该值用于动画时长缩放（&gt;1 表示减速），而飞梭前进需要的是时间流速。
        /// </summary>
        private float playheadPlaySpeed = 1.0f;

        #endregion

        #region 播放控制 - 对外方法
        /// <summary>
        /// 快速跳转到轨道头（时间 0 秒），
        /// 并把"每类型第一个 Clip"的起点值应用到物体。
        /// <para/>快捷键：Home
        /// <para/>位置：永远跳到 0 秒（Home 键语义 = 轨道头）；
        /// 效果：按类型分组取 Delay 最小的节点，把其起点值硬写组件，不做时间插值。
        /// </summary>
        public void JumpToTimelineStart()
        {
            JumpToTimelineStartAndApplyFirstValues();
        }
        /// <summary>
        /// 快速跳转到轨道尾（最后一个动画 Clip 或音效 Clip 的结尾），
        /// 并把"每类型最后 Clip"的终点值应用到物体。
        /// <para/>快捷键：End
        /// <para/>位置：所有 Clip 的最右端时间（不叠加 ContentTrailingSeconds 富余）；
        /// 效果：按类型分组取 Delay 最大的节点，把其终点值硬写组件，不做时间插值。
        /// </summary>
        public void JumpToTimelineEnd()
        {
            JumpToTimelineEndAndApplyLastValues();
        }
        /// <summary>
        /// 求所有动画 Clip + 音效 Clip 的最右端时间（秒）。
        /// <para/>动画：Delay + Duration
        /// <para/>音效：Delay + （Sound 长度，为空时用 DefaultSoundClipSeconds）
        /// <para/>空列表返回 0。
        /// </summary>
        private float GetLastClipEndSecond()
        {
            float maxEnd = 0f;

            if (Nodes != null)
            {
                for (int i = 0; i < Nodes.Count; i++)
                {
                    float end = Nodes[i].Delay + Nodes[i].Duration;
                    if (end > maxEnd) maxEnd = end;
                }
            }

            if (Sounds != null)
            {
                for (int i = 0; i < Sounds.Count; i++)
                {
                    TweenSound s = Sounds[i];
                    float len = s.Sound != null ? s.Sound.length : DefaultSoundClipSeconds;
                    float end = s.Delay + len;
                    if (end > maxEnd) maxEnd = end;
                }
            }

            return maxEnd;
        }
        /// <summary>
        /// 飞梭步进后退（时间减少 <see cref="playheadStepSeconds"/> 秒）。
        /// <para/>快捷键：&lt;（逗号键 / 小于号键）
        /// </summary>
        public void StepPlayheadBackward()
        {
            StopPlayback();
            playheadSecond = QuantizeTime(Mathf.Max(0f, playheadSecond - playheadStepSeconds));
            snapGuideSecond = -1f;
            RefreshPlayheadPreview(playheadSecond);
            Repaint();
        }
        /// <summary>
        /// 飞梭步进前进（时间增加 <see cref="playheadStepSeconds"/> 秒）。
        /// <para/>快捷键：&gt;（句号键 / 大于号键）
        /// </summary>
        public void StepPlayheadForward()
        {
            StopPlayback();

            //  上限 = 最后一个 Clip 的结尾（不含末尾富余区），与 End / StartPlayback / OnPlaybackEditorUpdate 统一
            float contentEnd = GetLastClipEndSecond();
            playheadSecond = QuantizeTime(Mathf.Min(contentEnd, playheadSecond + playheadStepSeconds));
            snapGuideSecond = -1f;
            RefreshPlayheadPreview(playheadSecond);
            Repaint();
        }
        /// <summary>
        /// 切换播放 / 暂停状态。
        /// <para/>快捷键：Space
        /// <list type="bullet">
        /// <item><description>当前为暂停 → 开始播放（从飞梭当前位置继续）；</description></item>
        /// <item><description>当前为播放 → 暂停（飞梭停在原地，再次按 Space 从当前点继续）。</description></item>
        /// </list>
        /// </summary>
        public void TogglePlayback()
        {
            if (isPlayheadPlaying)
                StopPlayback();
            else
                StartPlayback();
        }
        /// <summary>
        /// 开始播放：飞梭按真实时间自动前进，直到轨道尾自动停止。
        /// <para/>播放期间每帧调用 <see cref="DrivePlayheadPreview"/> 驱动预览。
        /// </summary>
        public void StartPlayback()
        {
            if (target == null || target.Equals(null)) return;

            //  播放上限 = 最后一个 Clip 的结尾（不含末尾富余区）
            float contentEnd = GetLastClipEndSecond();
            if (contentEnd <= 0.0001f) return;

            // 已在（或超过）最后一个 Clip 结尾时，从头重新播放
            if (playheadSecond >= contentEnd - 0.0001f)
                playheadSecond = 0f;

            isPlayheadPlaying = true;
            playheadLastTime = EditorApplication.timeSinceStartup;

            EditorApplication.update -= OnPlaybackEditorUpdate;
            EditorApplication.update += OnPlaybackEditorUpdate;

            Repaint();
        }
        /// <summary>
        /// 停止 / 暂停播放：飞梭停在原地，预览保持当前状态。
        /// <para/>再次调用 <see cref="StartPlayback"/> 可从当前点继续。
        /// </summary>
        public void StopPlayback()
        {
            if (!isPlayheadPlaying) return;

            isPlayheadPlaying = false;
            EditorApplication.update -= OnPlaybackEditorUpdate;

            //  重编译 / 关闭窗口期间 this 可能已失效，Repaint 前判空
            if (this != null)
                Repaint();
        }
        #endregion

        #region 播放控制 - 内部驱动
        /// <summary>
        /// 编辑器每帧更新：推进飞梭时间。
        /// <para/>仅在 <see cref="isPlayheadPlaying"/> 为 true 时被注册。
        /// </summary>
        private void OnPlaybackEditorUpdate()
        {
            //  Play 期间不访问 target
            if (isFrozenByPlayMode || EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            //  自毁检查：窗口已销毁 / target 失效 → 主动摘除回调并 return
            if (this == null)
            {
                EditorApplication.update -= OnPlaybackEditorUpdate;
                return;
            }

            if (!isPlayheadPlaying) return;

            if (target == null || target.Equals(null))
            {
                StopPlayback();
                return;
            }

            double now = EditorApplication.timeSinceStartup;
            float delta = (float)(now - playheadLastTime);
            playheadLastTime = now;

            // 防止编辑器卡顿导致的超大跳变
            delta = Mathf.Min(delta, 0.1f);

            //  播放上限 = 最后一个 Clip 的结尾（不含末尾富余区）
            float contentEnd = GetLastClipEndSecond();

            float speed = playheadPlaySpeed;
            if (target != null && target.GlobalDuration > 0.0001f)
                speed /= target.GlobalDuration;

            playheadSecond += delta * speed;

            if (playheadSecond >= contentEnd)
            {
                playheadSecond = contentEnd;
                RefreshPlayheadPreview(playheadSecond);
                StopPlayback();
                return;
            }

            RefreshPlayheadPreview(playheadSecond);
        }
        #endregion

        #region 播放控制 - 快捷键处理
        /// <summary>
        /// 处理播放控制相关的快捷键。
        /// <para/>在 <see cref="HandleKeyboardShortcuts"/> 之前调用，
        /// 若命中则消费事件并返回 true，避免与既有快捷键冲突。
        /// </summary>
        /// <returns>true 表示已消费本次按键事件</returns>
        private bool HandlePlaybackShortcuts()
        {
            Event e = Event.current;
            if (e.type != EventType.KeyDown) return false;

            // 拖拽飞梭期间禁用播放快捷键
            if (isDraggingPlayhead) return false;

            // 焦点在输入框时不响应
            if (GUIUtility.keyboardControl != 0) return false;

            // 播放中按任意播放控制键，先停止
            switch (e.keyCode)
            {
                case KeyCode.Home:
                    JumpToTimelineStart();
                    e.Use();
                    return true;

                case KeyCode.End:
                    JumpToTimelineEnd();
                    e.Use();
                    return true;

                case KeyCode.Comma:
                    StepPlayheadBackward();
                    e.Use();
                    return true;

                case KeyCode.Period:
                    StepPlayheadForward();
                    e.Use();
                    return true;

                case KeyCode.Space:
                    TogglePlayback();
                    e.Use();
                    return true;
            }

            return false;
        }
        #endregion

        #region 播放控制 - 工具栏按钮绘制
        /// <summary>
        /// 在顶部工具栏绘制播放控制按钮组。
        /// <para/>从左到右依次为：
        /// <list type="number">
        /// <item><description>快速跳转到轨道头；</description></item>
        /// <item><description>飞梭步进后退；</description></item>
        /// <item><description>播放 / 停止（图标随状态切换）；</description></item>
        /// <item><description>飞梭步进前进；</description></item>
        /// <item><description>快速跳转到轨道尾。</description></item>
        /// </list>
        /// </summary>
        private void DrawPlaybackToolbarButtons()
        {
            const float btnSize = 30f;
            const float btnGap = 5f;

            XGUI.layout_space(10);

            // ── 跳转到轨道头 ──
            if (XGUI.layout_button(
                tooltip: "跳转到轨道头 (Home)",
                tex_release: icon_playback_home_r,
                tex_press: icon_playback_home_p,
                tex_gui_color: Color.white,
                margin: new RectOffset(0, 0, 0, 0),
                border: new RectOffset(0, 0, 0, 0),
                width: btnSize,
                height: TopBarHeight))
            {
                JumpToTimelineStart();
            }

            XGUI.layout_space(btnGap);

            // ── 步进后退 ──
            if (XGUI.layout_button(
                tooltip: $"后退 {playheadStepSeconds:F2}s (<)",
                tex_release: icon_playback_stepback_r,
                tex_press: icon_playback_stepback_p,
                tex_gui_color: Color.white,
                margin: new RectOffset(0, 0, 0, 0),
                border: new RectOffset(0, 0, 0, 0),
                width: btnSize,
                height: TopBarHeight))
            {
                StepPlayheadBackward();
            }

            XGUI.layout_space(btnGap);

            // ── 播放 / 停止（状态切换）──
            if (isPlayheadPlaying)
            {
                if (XGUI.layout_button(
                    tooltip: "停止播放 (Space)",
                    tex_release: icon_playback_stop_r,
                    tex_press: icon_playback_stop_p,
                    tex_gui_color: XHud_Dashboard.Theme_Primary,
                    margin: new RectOffset(0, 0, 0, 0),
                    border: new RectOffset(0, 0, 0, 0),
                    width: btnSize,
                    height: TopBarHeight))
                {
                    StopPlayback();
                }
            }
            else
            {
                if (XGUI.layout_button(
                    tooltip: "开始播放 (Space)",
                    tex_release: icon_playback_play_r,
                    tex_press: icon_playback_play_p,
                    tex_gui_color: Color.white,
                    margin: new RectOffset(0, 0, 0, 0),
                    border: new RectOffset(0, 0, 0, 0),
                    width: btnSize,
                    height: TopBarHeight))
                {
                    StartPlayback();
                }
            }

            XGUI.layout_space(btnGap);

            // ── 步进前进 ──
            if (XGUI.layout_button(
                tooltip: $"前进 {playheadStepSeconds:F2}s (>)",
                tex_release: icon_playback_stepforward_r,
                tex_press: icon_playback_stepforward_p,
                tex_gui_color: Color.white,
                margin: new RectOffset(0, 0, 0, 0),
                border: new RectOffset(0, 0, 0, 0),
                width: btnSize,
                height: TopBarHeight))
            {
                StepPlayheadForward();
            }

            XGUI.layout_space(btnGap);

            // ── 跳转到轨道尾 ──
            if (XGUI.layout_button(
                tooltip: "跳转到轨道尾 (End)",
                tex_release: icon_playback_end_r,
                tex_press: icon_playback_end_p,
                tex_gui_color: Color.white,
                margin: new RectOffset(0, 0, 0, 0),
                border: new RectOffset(0, 0, 0, 0),
                width: btnSize,
                height: TopBarHeight))
            {
                JumpToTimelineEnd();
            }

            XGUI.layout_space(10);
        }
        #endregion

        #region 播放控制 - 窗口关闭清理
        /// <summary>
        /// 窗口关闭时清理播放状态（取消编辑器更新回调）。
        /// <para/>由 <see cref="CleanupPlayheadPreviewOnDisable"/> 调用。
        /// </summary>
        private void CleanupPlaybackOnDisable()
        {
            isPlayheadPlaying = false;
            EditorApplication.update -= OnPlaybackEditorUpdate;
        }
        #endregion

        #region 播放控制 - 预览刷新
        /// <summary>
        /// 统一刷新预览：写入组件属性后，强制刷新 Scene 视图与编辑器窗口。
        /// <para/>解决"飞梭跳转后要动一下鼠标才看到效果"的问题。
        /// <para/>所有播放控制方法（跳转 / 步进 / 播放推进）都应通过本方法刷新。
        /// </summary>
        /// <param name="t">要驱动的飞梭时间（秒）</param>
        private void RefreshPlayheadPreview(float t)
        {
            // 1. 驱动预览：把节点属性写到目标组件上
            DrivePlayheadPreview(t);

            // 2. 强制刷新 Scene 视图（关键：否则组件属性变了但画面不动）
            //SceneView.RepaintAll();

            // 3. 刷新本编辑器窗口
            Repaint();

            // 4. 让编辑器尽快处理一次 PlayerLoop（部分 Unity 版本下属性变更需要这一步才可见）
            EditorApplication.QueuePlayerLoopUpdate();
        }
        #endregion

        /// <summary>
        /// 编辑器焦点变化：失去焦点时暂停播放，避免后台空转。
        /// </summary>
        private void OnEditorFocusChanged(bool hasFocus)
        {
            if (!hasFocus && isPlayheadPlaying)
                StopPlayback();
        }
    }
}