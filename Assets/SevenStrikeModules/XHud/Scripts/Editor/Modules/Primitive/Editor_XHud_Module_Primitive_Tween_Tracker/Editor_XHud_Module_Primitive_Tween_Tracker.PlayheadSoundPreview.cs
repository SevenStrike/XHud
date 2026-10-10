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
    using UnityEngine;

    public partial class Editor_XHud_Module_Primitive_Tween_Tracker
    {
        #region 字段：飞梭音效预览
        /// <summary>
        /// 飞梭音效预览播放器（挂在隐藏 GameObject 上）。
        /// </summary>
        private AudioSource playheadPreviewAudioSource;
        /// <summary>
        /// 音效预览用的隐藏宿主 GameObject。
        /// </summary>
        private GameObject playheadPreviewAudioHost;
        /// <summary>
        /// 上一次驱动音效预览时的飞梭时间（秒）。
        /// <para/>用于判断飞梭是否"跨越了某个音效的 Delay（左边缘）"。
        /// <para/>初值 -1 表示尚未驱动过，本次驱动不做跨越判定。
        /// </summary>
        private float lastSoundDriveTime = -1f;
        #endregion

        #region 飞梭音效预览 - 总入口
        /// <summary>
        /// 飞梭音效预览总入口。
        /// <para/>触发语义：<b>飞梭跨越音效的左边缘（Delay）</b>时播放。
        /// <list type="bullet">
        /// <item><description>从左往右：飞梭从 Delay 左侧越到右侧 → 播；</description></item>
        /// <item><description>从右往左：飞梭从 Delay 右侧越到左侧 → 播；</description></item>
        /// <item><description>只在区间内移动但不越 Delay 线 → 不播。</description></item>
        /// </list>
        /// </summary>
        /// <param name="t">当前飞梭时间（内容绝对秒）</param>
        private void DrivePlayheadSoundPreview(float t)
        {
            if (target == null || target.Equals(null)) return;
            if (Sounds == null || Sounds.Count == 0)
            {
                lastSoundDriveTime = t;
                return;
            }

            // 首次驱动：只记录基准，不做跨越判定
            if (lastSoundDriveTime < 0f)
            {
                lastSoundDriveTime = t;
                return;
            }

            float prev = lastSoundDriveTime;
            float curr = t;

            // 遍历所有音效，检测是否跨越了 Delay 线
            for (int i = 0; i < Sounds.Count; i++)
            {
                TweenSound s = Sounds[i];
                if (s == null) continue;
                if (s.Mute) continue;
                if (s.Sound == null) continue;
                if (s.Sound.length <= 0f) continue;

                float delay = s.Delay;

                // 跨线判定：prev 与 curr 分别在 delay 两侧（含恰好落线）
                float dPrev = prev - delay;
                float dCurr = curr - delay;
                bool crossed = (dPrev * dCurr) <= 0f;

                // 排除"原地不动"的假跨越
                if (crossed && Mathf.Approximately(prev, curr))
                    crossed = false;

                if (!crossed) continue;

                // 每次跨线都播（策略 B）
                TriggerPreviewSound(i, curr);
            }

            // 更新基准时间
            lastSoundDriveTime = t;
        }
        /// <summary>
        /// 触发指定音效的预览播放。
        /// <para/>返回是否成功触发，供调用方决定要不要打"已触发"标记。
        /// </summary>
        /// <returns>true = 已播放；false = 被拒绝（组件缺失、资源缺失等）</returns>
        private bool TriggerPreviewSound(int index, float playheadTime)
        {
            if (index < 0 || index >= Sounds.Count) return false;
            TweenSound s = Sounds[index];
            if (s == null || s.Sound == null || s.Mute) return false;

            AudioSource src = EnsurePreviewAudioSource();
            if (src == null) return false;

            // ── 计算起始偏移 ──
            // 跨越 Delay 线时，无论正向还是反向，偏移都从 0 附近开始。
            // 反向时 curr 可能略小于 delay，钳到 0；
            // 正向时 curr 可能略大于 delay，也近似为 0。
            float offset = Mathf.Clamp(
                playheadTime - s.Delay,
                0f,
                Mathf.Max(0f, s.Sound.length - 0.001f));

            // ── 音量 / 音高 ──
            float volume = Mathf.Clamp01(s.Volume);

            float pitch = 1f;
            if (s.MinPitch > 0f && s.MaxPitch > 0f)
            {
                pitch = Random.Range(
                    Mathf.Min(s.MinPitch, s.MaxPitch),
                    Mathf.Max(s.MinPitch, s.MaxPitch));
            }
            else if (s.MinPitch > 0f)
            {
                pitch = s.MinPitch;
            }

            // ── 播放 ──
            src.pitch = pitch;
            src.volume = volume;

            if (offset <= 0.0001f)
            {
                src.PlayOneShot(s.Sound, volume);
            }
            else
            {
                // 带偏移播放：临时配置 clip 与 time
                src.clip = s.Sound;
                src.time = offset;
                src.Play();
            }

            return true;
        }
        /// <summary>
        /// 停止所有预览音效。
        /// </summary>
        private void StopAllPreviewSounds()
        {
            if (playheadPreviewAudioSource != null)
            {
                try
                {
                    playheadPreviewAudioSource.Stop();
                    playheadPreviewAudioSource.clip = null;
                }
                catch { }
            }

            lastSoundDriveTime = -1f;
        }
        /// <summary>
        /// 只重置基准时间，<b>不停止</b>正在播放的音效。
        /// <para/>用于：松手、按下刻度尺、跳转、播放/暂停按钮等场景。
        /// 这些场景下正在播的音效应该自然播完，不该被掐断。
        /// </summary>
        private void ResetSoundDriveBaseline()
        {
            lastSoundDriveTime = -1f;
        }
        #endregion

        #region 飞梭音效预览 - AudioSource 生命周期
        private AudioSource EnsurePreviewAudioSource()
        {
            if (playheadPreviewAudioSource != null)
                return playheadPreviewAudioSource;

            if (playheadPreviewAudioHost == null)
            {
                playheadPreviewAudioHost = new GameObject("[XHud] Playhead Audio Preview");
                playheadPreviewAudioHost.hideFlags = HideFlags.HideAndDontSave;
            }

            playheadPreviewAudioSource = playheadPreviewAudioHost.GetComponent<AudioSource>();
            if (playheadPreviewAudioSource == null)
            {
                playheadPreviewAudioSource = playheadPreviewAudioHost.AddComponent<AudioSource>();
            }

            playheadPreviewAudioSource.playOnAwake = false;
            playheadPreviewAudioSource.loop = false;
            playheadPreviewAudioSource.spatialBlend = 0f;
            playheadPreviewAudioSource.ignoreListenerPause = true;
            playheadPreviewAudioSource.ignoreListenerVolume = true;

            return playheadPreviewAudioSource;
        }
        /// <summary>
        /// 销毁预览 AudioSource 与其宿主对象（窗口关闭时调用）。
        /// </summary>
        private void DestroyPreviewAudioSource()
        {
            if (playheadPreviewAudioSource != null)
            {
                try
                {
                    playheadPreviewAudioSource.Stop();
                    playheadPreviewAudioSource.clip = null;
                }
                catch { }
                playheadPreviewAudioSource = null;
            }

            if (playheadPreviewAudioHost != null)
            {
                try
                {
                    Object.DestroyImmediate(playheadPreviewAudioHost);
                }
                catch { }
                playheadPreviewAudioHost = null;
            }

            lastSoundDriveTime = -1f;
        }
        #endregion
    }
}