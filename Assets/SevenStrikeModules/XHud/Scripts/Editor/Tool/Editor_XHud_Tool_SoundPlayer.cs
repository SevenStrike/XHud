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
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using Object = UnityEngine.Object;

    /// <summary>
    /// 编辑器预览音效统一播放器（全局单例，static）。
    /// <para/>
    /// 【职责】只做三件事：
    /// <list type="number">
    /// <item><description>接收"已算好的" AudioClip + 播放参数 + 延迟，加入待触发池；</description></item>
    /// <item><description>由 EditorApplication.update 每帧驱动，到点创建 AudioSource 播放；</description></item>
    /// <item><description>统一 Stop：待触发全丢 + 正在播立即掐断并销毁。</description></item>
    /// </list>
    /// <para/>
    /// 【销毁归属】预览音效的临时 GameObject 由本类<b>全权负责</b>销毁：
    /// 播完（!isPlaying）后立即 DestroyImmediate，不依赖任何场景组件。
    /// 之所以能在编辑器下可靠工作，是因为 Tick 挂在 EditorApplication.update 上，
    /// 而它才是编辑器真正每帧都跑的驱动；[ExecuteInEditMode] 的 MonoBehaviour.Update
    /// 在编辑器空闲时并不会每帧触发，不能作为预览生命周期的依据。
    /// <para/>
    /// 【设计边界】本类<b>不</b>接触 HudManager / TweenSound / XHud_Element_Sounder / 音效库，
    /// 一切"从哪里取 clip、音量怎么算"都交给调用方。这样：
    /// <list type="bullet">
    /// <item><description>图元侧直接传 sod.Sound；</description></item>
    /// <item><description>元素音效器侧传 SoundLibrary_GetSound(sounder.SoundName)；</description></item>
    /// <item><description>未来若图元也改为从音效库取，helper 无需改动。</description></item>
    /// </list>
    /// <para/>
    /// 【为什么用 static 单例而非实例化】
    /// 编辑器同一时刻只有一个 Inspector 处于激活态，切换选中会触发旧 Editor 的 OnDisable，
    /// 而 OnDisable 会调用 Stop()。因此不同窗口的预览天然互斥，不存在跨窗口状态串台。
    /// </summary>
    internal static class Editor_XHud_Tool_SoundPlayer
    {
        #region 内部类型
        /// <summary>
        /// 待触发音效：延迟未到、尚未创建 AudioSource。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>由 <c>Enqueue</c> 创建并加入 <c>_pending</c> 池。</description></item>
        /// <item><description>由 <c>Tick</c> 在到点时取出播放。</description></item>
        /// </list>
        /// </summary>
        private class PendingPreviewSound
        {
            /// <summary>待播放音频剪辑。</summary>
            public AudioClip Clip;
            /// <summary>音量（0-1）。</summary>
            public float Volume;
            /// <summary>是否静音。</summary>
            public bool Mute;
            /// <summary>是否使用随机音调。</summary>
            public bool UseRandomPitch;
            /// <summary>随机音调下限。</summary>
            public float PitchMin;
            /// <summary>随机音调上限。</summary>
            public float PitchMax;
            /// <summary>触发时刻（EditorApplication.timeSinceStartup + delay）</summary>
            public double TriggerAtTime;
        }
        #endregion

        #region 字段 - 池
        /// <summary>
        /// 待触发池。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>存放延迟未到、尚未创建 AudioSource 的音效。</description></item>
        /// </list>
        /// </summary>
        private static readonly List<PendingPreviewSound> _pending = new List<PendingPreviewSound>();
        /// <summary>
        /// 正在播的 AudioSource 列表。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>由本类创建并持有，播完后由 <c>CleanupFinished</c> 销毁。</description></item>
        /// </list>
        /// </summary>
        private static readonly List<AudioSource> _active = new List<AudioSource>();
        #endregion

        #region 入队
        /// <summary>
        /// 加入一条待触发音效。
        /// <para/>
        /// 【参数】clip 必须非空；delay 为秒；volume/mute/pitch 由调用方算好传入。
        /// <para/>
        /// 【行为】
        /// <list type="bullet">
        /// <item><description>若 clip 为空，静默忽略（避免调用方判空）；</description></item>
        /// <item><description>delay &lt;= 0 时立即触发（下一帧 Tick 会处理）；</description></item>
        /// <item><description>加入后确保 tick 已注册（幂等）。</description></item>
        /// </list>
        /// </summary>
        public static void Enqueue(AudioClip clip, float delay, float volume, bool mute, bool useRandomPitch, float pitchMin, float pitchMax)
        {
            if (clip == null)
                return;

            _pending.Add(new PendingPreviewSound
            {
                Clip = clip,
                Volume = Mathf.Clamp01(volume),
                Mute = mute,
                UseRandomPitch = useRandomPitch,
                PitchMin = pitchMin,
                PitchMax = pitchMax,
                TriggerAtTime = EditorApplication.timeSinceStartup + Mathf.Max(0f, delay),
            });

            EnsureTick();
        }
        /// <summary>
        /// 确保 tick 已注册到 EditorApplication.update。
        /// <para/>
        /// 【为什么"先摘再挂"】Unity 编辑器重编译（改脚本、进 Play 模式）时，
        /// 静态字段会被重置，但 EditorApplication.update 的委托链不会自动清空。
        /// 若直接 +=，会出现"同一个 Tick 被注册两次"，
        /// 导致音效重复触发、AudioSource 重复创建。
        /// 因此每次注册前先 -= 一次（不存在时是 no-op），再 +=，实现真正的幂等。
        /// </summary>
        private static void EnsureTick()
        {
            // 幂等：先摘干净，再挂一个
            EditorApplication.update -= Tick;
            EditorApplication.update += Tick;
        }
        #endregion

        #region 帧驱动
        /// <summary>
        /// 每帧驱动：清理已播完的 AudioSource + 检查待触发池。
        /// <para/>
        /// 【自毁机制】池空时 CleanupFinished 会摘除 tick，避免空转。
        /// <para/>
        /// 【重编译兜底】用 try/catch 包住，重编译时可能访问到已失效的对象，
        /// 捕获异常后主动 Stop，避免 tick 残留。
        /// </summary>
        private static void Tick()
        {
            try
            {
                CleanupFinished();

                // CleanupFinished 在池空时已摘 tick；若此刻仍被调用（例如同帧内刚 Enqueue），
                // 说明还有待触发项，继续往下走。
                if (_pending.Count == 0 && _active.Count == 0)
                    return;

                double now = EditorApplication.timeSinceStartup;
                for (int i = _pending.Count - 1; i >= 0; i--)
                {
                    if (now >= _pending[i].TriggerAtTime)
                    {
                        PlayImmediate(_pending[i]);
                        _pending.RemoveAt(i);
                    }
                }
            }
            catch { Stop(); }
        }
        #endregion

        #region 播放
        /// <summary>
        /// 立即播放一条待触发音效：创建临时 GameObject + AudioSource，播完由 CleanupFinished 销毁。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>临时物体命名含剪辑时长与剪辑名，便于在 Hierarchy 中辨识。</description></item>
        /// <item><description>默认不隐藏物体（<c>hideFlags</c> 那行被有意注释）。</description></item>
        /// </list>
        /// </summary>
        private static void PlayImmediate(PendingPreviewSound p)
        {
            if (p == null || p.Clip == null)
                return;

            GameObject obj = new GameObject();
            obj.name = $"XHud_TweenPreviewSound-[{p.Clip.length:F2}s]-{p.Clip.name}";

            // 隐藏创建的预览音效物体
            //obj.hideFlags = HideFlags.HideAndDontSave;

            AudioSource au = obj.AddComponent<AudioSource>();
            au.playOnAwake = false;
            au.clip = p.Clip;
            au.volume = p.Volume;
            au.mute = p.Mute;
            au.spatialBlend = 0f;

            float pMin = Mathf.Min(p.PitchMin, p.PitchMax);
            float pMax = Mathf.Max(p.PitchMin, p.PitchMax);
            au.pitch = p.UseRandomPitch ? Random.Range(pMin, pMax) : 1f;

            au.Play();

            _active.Add(au);
        }
        #endregion

        #region 清理
        /// <summary>
        /// 清理已播完的 AudioSource：立即 DestroyImmediate 对应 GameObject，并从 _active 列表摘除。
        /// <para/>
        /// 【写法约定】必须先判 au != null（Unity 假 null 机制），
        /// 再判 au.gameObject != null。反过来会在 au 已销毁时访问 .gameObject 抛 MissingReferenceException。
        /// <para/>
        /// 【立即销毁语义】不设延迟缓冲——一旦 !isPlaying 就立即销毁。
        /// 预览音效只是试听，不需要尾部缓冲；延迟反而会让临时对象残留在 Hierarchy 里。
        /// </summary>
        private static void CleanupFinished()
        {
            for (int i = _active.Count - 1; i >= 0; i--)
            {
                AudioSource au = _active[i];
                if (au == null || !au.isPlaying)
                {
                    if (au != null && au.gameObject != null)
                        Object.DestroyImmediate(au.gameObject, true);
                    _active.RemoveAt(i);
                }
            }

            // 池空 + 正在播也空 → 完全摘 tick（避免 tick 常驻）
            if (_pending.Count == 0 && _active.Count == 0)
            {
                EditorApplication.update -= Tick;
            }
        }
        #endregion

        #region 停止
        /// <summary>
        /// 停止并清空所有预览音效。
        /// <para/>
        /// 【语义】立即掐断：
        /// <list type="bullet">
        /// <item><description>待触发池清空（还没响的全丢）；</description></item>
        /// <item><description>正在播的 au.Stop() 立即停 + DestroyImmediate 立即销毁；</description></item>
        /// <item><description>tick 摘除。</description></item>
        /// </list>
        /// <para/>
        /// 【幂等】可重复调用，无副作用。
        /// </summary>
        public static void Stop()
        {
            _pending.Clear();
            EditorApplication.update -= Tick;

            for (int i = 0; i < _active.Count; i++)
            {
                AudioSource au = _active[i];
                if (au == null) continue;
                au.Stop();
                if (au.gameObject != null)
                    Object.DestroyImmediate(au.gameObject, true);
            }
            _active.Clear();
        }
        #endregion
    }
}