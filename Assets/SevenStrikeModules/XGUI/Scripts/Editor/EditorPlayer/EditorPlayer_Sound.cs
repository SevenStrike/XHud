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
namespace SevenStrikeModules.XGUI.Editor
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using Object = UnityEngine.Object;

    public static partial class XGUI
    {
        /*
         * ============================================================================
         *  XGUI.EditorPlayer_Sound  运行逻辑说明
         * ============================================================================
         *
         *  【一、它是什么】
         *  ------------------------------------------------------------------------
         *  一个挂在 EditorApplication.update 上的“编辑器音效调度器”。
         *  调用方把“已算好的 AudioClip + 播放参数 + 延迟”交给它，
         *  它到点创建 AudioSource 播一次，播完立即销毁。
         *
         *  与 EditorPlayer_Sequence / EditorPlayer_Values 的关系：
         *      Sequence ：到点推“一格”帧索引（循环推进）；
         *      Values   ：到点推“t 值”（数值插值）；
         *      Sound    ：到点播“一次”音效（播完销毁）。
         *  三者共享驱动骨架（EditorApplication.update + 幂等 tick），语义不同。
         *
         *
         *  【二、它做什么 / 不做什么】
         *  ------------------------------------------------------------------------
         *  【做】
         *    1. 接收"已算好的" AudioClip + 播放参数 + 延迟，加入待触发池；
         *    2. 每帧检查待触发池，到点创建 AudioSource 播放；
         *    3. 每帧清理已播完的 AudioSource（DestroyImmediate）；
         *    4. 统一 Stop：待触发全丢 + 正在播立即掐断并销毁。
         *
         *  【不做】
         *    - 不接触 HudManager / TweenSound / XHud_Element_Sounder / 音效库；
         *    - 不取 clip（由调用方算好传入）；
         *    - 不算音量 / 静音 / 音调（由调用方算好传入）；
         *    - 不自己 Repaint；
         *    - 不依赖任何业务概念。
         *
         *
         *  【三、驱动归属：为什么用 EditorApplication.update】
         *  ------------------------------------------------------------------------
         *  同 Sequence / Values。
         *  [ExecuteInEditMode] 的 MonoBehaviour.Update 在编辑器空闲时并不会每帧触发，
         *  不能作为音效生命周期的依据。
         *  EditorApplication.update 才是编辑器真正每帧都跑的驱动。
         *
         *
         *  【四、运行时序】
         *  ------------------------------------------------------------------------
         *  【阶段 1：Enqueue】
         *      Enqueue(clip, delay, volume, mute, useRandomPitch, pitchMin, pitchMax)
         *          ├─ clip 为空？→ 静默忽略
         *          ├─ 创建 PendingPreviewSound，算 TriggerAtTime = now + delay
         *          ├─ 加入 _pending
         *          └─ EnsureTick()  → 挂 Tick 到 EditorApplication.update
         *
         *  【阶段 2：每帧 Tick】
         *      Tick()
         *          ├─ CleanupFinished()  → 清理已播完的 AudioSource
         *          ├─ _pending 和 _active 都空？→ 摘 tick，return
         *          └─ 倒序遍历 _pending：
         *               └─ now >= TriggerAtTime？
         *                     ├─ PlayImmediate(待触发项)
         *                     │     ├─ 创建临时 GameObject + AudioSource
         *                     │     ├─ 设置 clip / volume / mute / pitch
         *                     │     ├─ au.Play()
         *                     │     └─ 加入 _active
         *                     └─ 从 _pending 移除
         *
         *  【阶段 3：Stop】
         *      Stop()
         *          ├─ _pending.Clear()
         *          ├─ 摘 tick
         *          ├─ 遍历 _active：au.Stop() + DestroyImmediate(GameObject)
         *          └─ _active.Clear()
         *
         *
         *  【五、待触发池 vs 正在播池】
         *  ------------------------------------------------------------------------
         *      _pending ：延迟未到、尚未创建 AudioSource 的音效。
         *      _active  ：已创建 AudioSource、正在播的音效。
         *
         *  【迁移路径】
         *      Enqueue         →  _pending
         *      Tick 到点        →  _pending → _active
         *      CleanupFinished →  _active  → 销毁
         *
         *
         *  【六、状态字段一览】
         *  ------------------------------------------------------------------------
         *      _pending      List<PendingPreviewSound>  待触发池
         *      _active       List<AudioSource>          正在播池
         *
         *  【PendingPreviewSound 内部字段】
         *      Clip            AudioClip   待播放音频
         *      Volume          float       音量（0-1）
         *      Mute            bool        是否静音
         *      UseRandomPitch  bool        是否使用随机音调
         *      PitchMin        float       随机音调下限
         *      PitchMax        float       随机音调上限
         *      TriggerAtTime   double      触发时刻
         *
         *
         *  【七、销毁归属】
         *  ------------------------------------------------------------------------
         *  预览音效的临时 GameObject 由本类全权负责销毁：
         *  一旦 !isPlaying 就立即 DestroyImmediate，
         *  不依赖任何场景组件，也不设延迟缓冲。
         *
         *  【为什么立即销毁】
         *      预览音效只是试听，不需要尾部缓冲；
         *      延迟反而会让临时对象残留在 Hierarchy 里。
         *
         *
         *  【八、幂等与自毁】
         *  ------------------------------------------------------------------------
         *  【EnsureTick 先摘再挂】
         *      - 重编译时静态字段被重置，但委托链不会自动清空；
         *      - 若直接 +=，会出现"同一个 Tick 被注册多次"，
         *        导致音效重复触发、AudioSource 重复创建；
         *      - 先 -= 再 +=，实现真正的幂等。
         *
         *  【CleanupFinished 空池摘 tick】
         *      - _pending 和 _active 都空时，完全摘 tick，避免空转；
         *      - 所有"自然停止"（播完）都走这条路径。
         *
         *  【Stop 幂等】
         *      - 可重复调用，无副作用；
         *      - OnDisable / Tick 异常都走 Stop。
         *
         *
         *  【九、为什么是 static 单例而非实例】
         *  ------------------------------------------------------------------------
         *  编辑器同一时刻只有一个 Inspector 处于激活态，
         *  切换选中会触发旧 Editor 的 OnDisable，
         *  而 OnDisable 会调用 Stop()。
         *  因此不同窗口的音效预览天然互斥，不存在跨窗口状态串台。
         *
         *
         *  【十、典型调用方】
         *  ------------------------------------------------------------------------
         *      - Editor_XHud_Module_Primitive_Tween：图元动画音效预览；
         *      - Editor_XHud_Module_Element.Previewer_Sound：元素音效器预览；
         *      - 未来：任何需要"到点播一次"的编辑器场景。
         *
         *
         *  【十一、与 Sequence / Values 的对比】
         *  ------------------------------------------------------------------------
         *      维度     | Sound                    | Sequence                | Values
         *      ---------|--------------------------|-------------------------|-------------------------
         *      驱动     | EditorApplication.update | 同                      | 同
         *      语义     | 到点播"一次"             | 到点推"一格"            | 到点推"t 值"
         *      数量     | 多个并存（队列）          | 单一（当前序列）        | 单一（当前插值）
         *      生命周期 | 入队 → 播 → 销毁          | 替换 → 循环推进         | 替换 → 循环推进
         *      停止     | 播完自动销毁              | 循环到 Stop 或耗尽       | 循环到 Stop 或耗尽
         *
         *  同构，不同义。共享驱动骨架，不共享业务语义。
         *
         *
         *  【十二、设计边界（再次强调）】
         *  ------------------------------------------------------------------------
         *      调用方负责：取 clip、算音量、算静音、算音调、算延迟。
         *      播放器负责：入队、到点播、播完销毁、tick 管理。
         *
         *  这条边界不能破，否则 Sound 会退化成"什么都干"的上帝类。
         *
         *
         *  【十三、典型用法示例】
         *  ------------------------------------------------------------------------
         *  【示例 1：延迟 0.5 秒播一个音效】
         *      XGUI.EditorPlayer_Sound.Enqueue(
         *          clip: myClip,
         *          delay: 0.5f,
         *          volume: 1f,
         *          mute: false,
         *          useRandomPitch: false,
         *          pitchMin: 1f,
         *          pitchMax: 1f);
         *
         *  【示例 2：带随机音调】
         *      XGUI.EditorPlayer_Sound.Enqueue(
         *          clip: myClip,
         *          delay: 0f,
         *          volume: 0.8f,
         *          mute: false,
         *          useRandomPitch: true,
         *          pitchMin: 0.95f,
         *          pitchMax: 1.05f);
         *
         *  【示例 3：窗口关闭时统一停止】
         *      private void OnDisable()
         *      {
         *          XGUI.EditorPlayer_Sound.Stop();
         *      }
         *
         *  【示例 4：多个音效同时入队，各自延迟不同】
         *      Enqueue(clipA, 0.0f, 1f, false, false, 1f, 1f);
         *      Enqueue(clipB, 0.3f, 1f, false, false, 1f, 1f);
         *      Enqueue(clipC, 0.6f, 1f, false, false, 1f, 1f);
         *      // 0.0s 播 A，0.3s 播 B，0.6s 播 C
         *
         * ============================================================================
         */

        public static class EditorPlayer_Sound
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
}