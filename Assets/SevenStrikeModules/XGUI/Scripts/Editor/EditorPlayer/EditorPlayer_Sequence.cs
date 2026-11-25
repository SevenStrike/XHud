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
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    public static partial class XGUI
    {
        /*
         * ============================================================================
         *  XGUI.EditorPlayer_Sequence  运行逻辑说明
         * ============================================================================
         *
         *  【一、它是什么】
         *  ------------------------------------------------------------------------
         *  一个挂在 EditorApplication.update 上的“编辑器定时帧推进器”。
         *  序列帧只是它最直观的一种用法；本质是“每隔 N 秒推进一格，然后回调”。
         *
         *  与 EditorPlayer_Sound / EditorPlayer_Values 的关系：
         *      Sequence ：到点推“一格”帧索引（循环推进）；
         *      Values   ：到点推“t 值”（数值插值）；
         *      Sound    ：到点播“一次”音效（播完销毁）。
         *  三者共享驱动骨架（EditorApplication.update + 幂等 tick），语义不同。
         *
         *
         *  【二、它做什么 / 不做什么】
         *  ------------------------------------------------------------------------
         *  【做】
         *    1. 持有当前序列帧列表、索引、间隔、循环次数；
         *    2. 每帧检查是否到点，到点推进索引并调回调；
         *    3. 根据 loopCount 决定循环、播完停住；
         *    4. 统一 Stop：清空状态 + 摘 tick。
         *
         *  【不做】
         *    - 不加载资源（不知道 samples 目录、不知道 .asset）；
         *    - 不知道“色卡”“图元”“曲线”等业务概念；
         *    - 不自己 Repaint（由调用方在回调里决定）；
         *    - 不接触任何业务类 / 协程工具。
         *
         *
         *  【三、驱动归属：为什么用 EditorApplication.update】
         *  ------------------------------------------------------------------------
         *  EditorApplication.update 是编辑器真正每帧都跑的驱动。
         *  [ExecuteInEditMode] 的 MonoBehaviour.Update 在编辑器空闲时
         *  并不会每帧触发（没焦点、窗口失活、资源导入时都可能停），
         *  不能作为预览生命周期的依据。
         *
         *
         *  【四、运行时序】
         *  ------------------------------------------------------------------------
         *  【阶段 1：Play】
         *      Play(frames, interval, onFrame, loopCount)
         *          ├─ 替换 _frames
         *          ├─ 重置 _index = 0
         *          ├─ 重置 _nextAtTime = now + interval
         *          ├─ 重置 _playedCount = 0
         *          └─ EnsureTick()  → 挂 Tick 到 EditorApplication.update
         *
         *  【阶段 2：每帧 Tick】
         *      Tick()
         *          ├─ 帧数据为空？→ UnhookTick()，return
         *          ├─ now < _nextAtTime？→ return（还没到点）
         *          └─ 到点：
         *               ├─ _nextAtTime = now + _interval
         *               ├─ _index++
         *               ├─ _index >= Count？
         *               │     ├─ _playedCount++
         *               │     ├─ loopCount 有限 且 playedCount > loopCount？
         *               │     │     ├─ 是：_index = Count - 1（停在最后一帧）
         *               │     │     │      UnhookTick()，return
         *               │     │     └─ 否：_index = 0（还有下一遍）
         *               │     └─ （未越界则继续）
         *               └─ _onFrame?.Invoke()
         *
         *  【阶段 3：Stop】
         *      Stop()
         *          ├─ _frames = null
         *          ├─ _index = 0
         *          ├─ _interval = 0
         *          ├─ _onFrame = null
         *          ├─ _loopCount = -1
         *          ├─ _playedCount = 0
         *          └─ UnhookTick()
         *
         *
         *  【五、循环次数语义（与 XTween LoopCount 对齐）】
         *  ------------------------------------------------------------------------
         *      -1    ：无限循环
         *       0    ：循环 1 次（播一遍就停）
         *       N>=0 ：循环 N+1 次（播 N+1 遍）
         *
         *  播完时停在最后一帧，不再推进。
         *
         *
         *  【六、状态字段一览】
         *  ------------------------------------------------------------------------
         *      _frames       List<Texture2D>  当前序列帧列表
         *      _index        int              当前帧索引
         *      _interval     float            帧间隔（秒）
         *      _nextAtTime   double           下一帧触发时刻
         *      _onFrame      Action           帧推进回调
         *      _loopCount    int              循环次数
         *      _playedCount  int              已完成的循环次数
         *      IsTicking     bool             tick 是否已挂（正在推进）
         *
         *
         *  【七、IsPlaying 与 IsTicking 的区别】
         *  ------------------------------------------------------------------------
         *      IsPlaying ：有帧数据（“有帧可画”）
         *      IsTicking ：tick 已挂（“正在推进”）
         *
         *  播完后：IsPlaying 仍为 true（最后一帧还能画），IsTicking 为 false。
         *
         *
         *  【八、幂等与自毁】
         *  ------------------------------------------------------------------------
         *  【EnsureTick 先摘再挂】
         *      - 重编译时静态字段被重置，但委托链不会自动清空；
         *      - 若直接 +=，会出现“同一个 Tick 被注册多次”，推进速度翻倍；
         *      - 先 -= 再 +=，实现真正的幂等。
         *
         *  【UnhookTick 统一摘 tick】
         *      - 所有“自然停止”（空数据、播完）都走 UnhookTick；
         *      - 保证 IsTicking 和 tick 挂载状态始终一致。
         *
         *  【Stop 幂等】
         *      - 可重复调用，无副作用；
         *      - OnDisable / Play 空数据 / Tick 异常都走 Stop。
         *
         *
         *  【九、为什么是 static 单例而非实例】
         *  ------------------------------------------------------------------------
         *  编辑器同一时刻只有一个预览窗口处于激活态，
         *  切换/关闭会触发旧窗口的 OnDisable，而 OnDisable 会调用 Stop()。
         *  因此不同窗口的预览天然互斥，不存在跨窗口状态串台。
         *  用 static 单例让状态集中、生命周期明确。
         *
         *
         *  【十、典型调用方】
         *  ------------------------------------------------------------------------
         *      - Editor_XHud_LibrarySetTool_Color：色卡预览；
         *      - 未来：时间轴 playhead、曲线预览、打字机预览、呼吸灯等。
         *
         *
         *  【十一、与 EditorPlayer_Sound / EditorPlayer_Values 的对比】
         *  ------------------------------------------------------------------------
         *      维度     | Sequence                 | Values                  | Sound
         *      ---------|--------------------------|-------------------------|-------------------------
         *      驱动     | EditorApplication.update | 同                      | 同
         *      语义     | 到点推“一格”             | 到点推“t 值”            | 到点播“一次”
         *      数量     | 单一（当前序列）          | 单一（当前插值）        | 多个并存（队列）
         *      生命周期 | 替换 → 循环推进           | 替换 → 循环推进         | 入队 → 播 → 销毁
         *      停止     | 循环到 Stop 或耗尽       | 循环到 Stop 或耗尽       | 播完自动销毁
         *
         *  同构，不同义。共享驱动骨架，不共享业务语义。
         *
         *
         *  【十二、设计边界（再次强调）】
         *  ------------------------------------------------------------------------
         *      调用方负责：播什么、什么节奏、回调干嘛、翻页、渲染。
         *      播放器负责：到点推进、回调、循环、停止、tick 管理。
         *
         *  这条边界不能破，否则 Sequence 会退化成“什么都干”的上帝类。
         *
         *
         *  【十三、典型用法示例】
         *  ------------------------------------------------------------------------
         *  【示例 1：无限循环播放序列帧（最简）】
         *      // 打开窗口时启动；关闭时 Stop
         *      private void OnEnable()
         *      {
         *          XGUI.EditorPlayer_Sequence.Play(
         *              frames,
         *              interval: 0.035f,
         *              onFrame: () =>
         *              {
         *                  PreviewTex = XGUI.EditorPlayer_Sequence.Current;
         *                  Repaint();
         *              });
         *          // loopCount 默认 -1，无限循环
         *      }
         *
         *      private void OnDisable()
         *      {
         *          XGUI.EditorPlayer_Sequence.Stop();
         *      }
         *
         *
         *  【示例 2：播一遍就停（停在最后一帧）】
         *      XGUI.EditorPlayer_Sequence.Play(
         *          frames,
         *          0.05f,
         *          () => Repaint(),
         *          loopCount: 0);
         *      // 播完后 IsTicking = false，Current = 最后一帧
         *
         *
         *  【示例 3：播 N+1 遍就停】
         *      // 循环 3 次 = 播 4 遍
         *      XGUI.EditorPlayer_Sequence.Play(
         *          frames,
         *          0.05f,
         *          () => Repaint(),
         *          loopCount: 3);
         *
         *
         *  【示例 4：翻页时只换序列，不重置计时】
         *      // 避免翻页后第一帧停一拍
         *      XGUI.EditorPlayer_Sequence.SetFrames(newFrames);
         *      // 注意：SetFrames 不重置计时，但会重置 _playedCount
         *
         *
         *  【示例 5：用数值循环做动效预览（不绑纹理）】
         *      // 让一个 float 在 0~1 之间循环，套曲线，用于预览渐变 / Shader 参数
         *      int steps = 60;
         *      XGUI.EditorPlayer_Sequence.Play(
         *          dummyFrames,        // 占位，长度决定步数
         *          1f / 60f,
         *          () =>
         *          {
         *              int step = XGUI.EditorPlayer_Sequence.CurrentIndex;
         *              float t = step / (float)steps;
         *              previewColor = Color.Lerp(from, to, curve.Evaluate(t));
         *              Repaint();
         *          },
         *          loopCount: -1);
         *
         *
         *  【示例 6：在 OnGUI 里画当前帧】
         *      Texture2D frame = XGUI.EditorPlayer_Sequence.IsPlaying
         *          ? XGUI.EditorPlayer_Sequence.Current
         *          : fallbackTex;
         *
         *      XGUI.gui_icon(rect, frame, color: tint);
         *
         *
         *  【示例 7：判断“是否还在推进”】
         *      if (XGUI.EditorPlayer_Sequence.IsTicking)
         *      {
         *          // 还在推进，可以显示“播放中”指示器
         *      }
         *      else if (XGUI.EditorPlayer_Sequence.IsPlaying)
         *      {
         *          // 播完了，但最后一帧还能画
         *      }
         *
         *
         *  【示例 8：手动推进一帧（尊重 loopCount）】
         *      XGUI.EditorPlayer_Sequence.Next();
         *      // 未播完：推进一格；已播完：不推进
         *
         * ============================================================================
         */

        public static class EditorPlayer_Sequence
        {
            #region 状态
            /// <summary>
            /// 当前正在播放的序列帧列表。
            /// <para/>由 Play 设置，Stop 清空。
            /// </summary>
            private static List<Texture2D> _frames;
            /// <summary>
            /// 当前帧索引。
            /// <para/>取值范围 [0, _frames.Count)。
            /// </summary>
            private static int _index;
            /// <summary>
            /// 每帧间隔（秒）。
            /// <para/>由 Play 设置，会被 Clamp 到 [0.01, 1]。
            /// </summary>
            private static float _interval;
            /// <summary>
            /// 下一帧的触发时刻（EditorApplication.timeSinceStartup 基准）。
            /// </summary>
            private static double _nextAtTime;
            /// <summary>
            /// 每推进一帧回调一次（通常传 window.Repaint）。
            /// </summary>
            private static Action _onFrame;
            /// <summary>
            /// 循环次数。
            /// <para/>
            /// -1：无限循环；
            /// 0：循环 1 次（播一遍就停）；
            /// N &gt;= 0：循环 N+1 次（播 N+1 遍）。
            /// </summary>
            private static int _loopCount = -1;
            /// <summary>
            /// 已完成的循环次数。
            /// <para/>
            /// 每次 _index 从末尾回绕到 0 时 +1；
            /// 当 _loopCount &gt;= 0 且 _playedCount &gt; _loopCount 时停止。
            /// </summary>
            private static int _playedCount = 0;
            #endregion

            #region 播放控制
            /// <summary>
            /// 开始（或替换）播放一组序列帧。
            /// <para/>
            /// 【参数】
            /// <list type="bullet">
            /// <item><description>frames：非空时才会生效；空则直接 Stop。</description></item>
            /// <item><description>interval：会被 Clamp 到 [0.01, 1]（秒）。</description></item>
            /// <item><description>onFrame：每推进一帧回调一次，通常传 window.Repaint。</description></item>
            /// <item><description>loopCount：循环次数。-1 无限；0 播一遍；N 播 N+1 遍。默认 -1。</description></item>
            /// </list>
            /// <para/>
            /// 【行为】
            /// <list type="bullet">
            /// <item><description>替换当前播放内容，索引归零，计时重置，播放计数归零；</description></item>
            /// <item><description>确保 tick 已注册（幂等）。</description></item>
            /// </list>
            /// </summary>
            public static void Play(List<Texture2D> frames, float interval, Action onFrame, int loopCount = -1)
            {
                if (frames == null || frames.Count == 0)
                {
                    Stop();
                    return;
                }

                _frames = frames;
                _index = 0;
                _interval = Mathf.Clamp(interval, 0.01f, 1f);
                _nextAtTime = EditorApplication.timeSinceStartup + _interval;
                _onFrame = onFrame;
                _loopCount = loopCount;
                _playedCount = 0;
                EnsureTick();
            }

            /// <summary>
            /// 只换序列，不重置计时（翻页时用，避免第一帧闪烁）。
            /// <para/>
            /// 【与 Play 的区别】
            /// <list type="bullet">
            /// <item><description>Play：替换内容 + 索引归零 + 计时重置 + 播放计数归零；</description></item>
            /// <item><description>SetFrames：只替换内容，索引保持（越界则归零），计时保持，播放计数归零。</description></item>
            /// </list>
            /// </summary>
            public static void SetFrames(List<Texture2D> frames)
            {
                if (frames == null || frames.Count == 0) return;

                _frames = frames;
                if (_index >= _frames.Count) _index = 0;
                _playedCount = 0;
            }

            /// <summary>
            /// 手动推进到下一帧。
            /// <para/>
            /// 【行为】
            /// <list type="bullet">
            /// <item><description>未播完：推进一帧，到末尾回绕或停在末尾；</description></item>
            /// <item><description>已播完（loopCount 有限且 playedCount 超了）：不再推进。</description></item>
            /// </list>
            /// </summary>
            public static void Next()
            {
                if (_frames == null || _frames.Count == 0) return;

                if (_loopCount >= 0 && _playedCount > _loopCount)
                    return;

                _index++;
                if (_index >= _frames.Count)
                {
                    _playedCount++;

                    if (_loopCount >= 0 && _playedCount > _loopCount)
                    {
                        _index = _frames.Count - 1;
                    }
                    else
                    {
                        _index = 0;
                    }
                }

                _onFrame?.Invoke();
            }

            /// <summary>
            /// 停止播放并清空状态。
            /// <para/>
            /// 【幂等】可重复调用，无副作用。
            /// </summary>
            public static void Stop()
            {
                _frames = null;
                _index = 0;
                _interval = 0f;
                _onFrame = null;
                _loopCount = -1;
                _playedCount = 0;
                UnhookTick();
            }
            #endregion

            #region 查询
            /// <summary>
            /// 当前帧纹理（未播放时返回 null）。
            /// </summary>
            public static Texture2D Current
            {
                get
                {
                    if (_frames == null || _frames.Count == 0) return null;
                    return _frames[_index];
                }
            }

            /// <summary>
            /// 当前帧索引（未播放时返回 -1）。
            /// <para/>用于“数值循环”场景：调用方通过索引算插值。
            /// </summary>
            public static int CurrentIndex
            {
                get
                {
                    if (_frames == null || _frames.Count == 0) return -1;
                    return _index;
                }
            }

            /// <summary>
            /// 是否正在播放（有帧数据）。不表示“是否在推进”。
            /// </summary>
            public static bool IsPlaying => _frames != null && _frames.Count > 0;

            /// <summary>
            /// 是否已挂 tick（正在推进）。
            /// <para/>
            /// 与 IsPlaying 的区别：
            /// IsPlaying 表示“有帧可画”；
            /// IsTicking 表示“tick 已挂，正在推进”。
            /// 播完后，IsPlaying 为 true（最后一帧还能画），IsTicking 为 false。
            /// </summary>
            public static bool IsTicking { get; private set; }
            #endregion

            #region 帧驱动
            /// <summary>
            /// 确保 tick 已注册到 EditorApplication.update。
            /// <para/>
            /// 【为什么"先摘再挂"】Unity 编辑器重编译（改脚本、进 Play 模式）时，
            /// 静态字段会被重置，但 EditorApplication.update 的委托链不会自动清空。
            /// 若直接 +=，会出现"同一个 Tick 被注册两次"，
            /// 导致帧推进速度翻倍。
            /// 因此每次注册前先 -= 一次（不存在时是 no-op），再 +=，实现真正的幂等。
            /// </summary>
            private static void EnsureTick()
            {
                EditorApplication.update -= Tick;
                EditorApplication.update += Tick;
                IsTicking = true;
            }

            /// <summary>
            /// 摘除 tick 并把 IsTicking 置 false。
            /// <para/>
            /// 所有“自然停止”（空数据、播完）都走这里，
            /// 保证 IsTicking 和 tick 挂载状态始终一致。
            /// </summary>
            private static void UnhookTick()
            {
                EditorApplication.update -= Tick;
                IsTicking = false;
            }

            /// <summary>
            /// 每帧驱动：到点推进帧索引并回调。
            /// <para/>
            /// 【自毁机制】帧数据为空或播完时摘除 tick，避免空转。
            /// <para/>
            /// 【重编译兜底】用 try/catch 包住，重编译时可能访问到已失效的对象，
            /// 捕获异常后主动 Stop，避免 tick 残留。
            /// </summary>
            private static void Tick()
            {
                try
                {
                    if (_frames == null || _frames.Count == 0)
                    {
                        UnhookTick();
                        return;
                    }

                    double now = EditorApplication.timeSinceStartup;
                    if (now < _nextAtTime) return;

                    _nextAtTime = now + _interval;

                    _index++;
                    if (_index >= _frames.Count)
                    {
                        _playedCount++;

                        if (_loopCount >= 0 && _playedCount > _loopCount)
                        {
                            _index = _frames.Count - 1;   // 停在最后一帧
                            UnhookTick();
                            return;
                        }

                        _index = 0;   // 还有下一遍
                    }
                    _onFrame?.Invoke();
                }
                catch { Stop(); }
            }
            #endregion
        }
    }
}