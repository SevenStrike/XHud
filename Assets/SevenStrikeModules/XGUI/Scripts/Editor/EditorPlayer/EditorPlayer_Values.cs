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
    using SevenStrikeModules.XGUI.Runtime;
    using System;
    using UnityEditor;
    using UnityEngine;

    public static partial class XGUI
    {
        /*
         * ============================================================================
         *  XGUI.EditorPlayer_Values  运行逻辑说明
         * ============================================================================
         *
         *  【一、它是什么】
         *  ------------------------------------------------------------------------
         *  一个挂在 EditorApplication.update 上的“编辑器数值插值驱动器”。
         *  按固定步数推进 t ∈ [0, 1]，每步回调 onStep(t, value)，
         *  由调用方决定“value 怎么算、t 怎么用”。
         *
         *  与 EditorPlayer_Sequence / EditorPlayer_Sound 的关系：
         *      Sequence ：到点推“一格”帧索引（循环推进）；
         *      Values   ：到点推“t 值”（数值插值）；
         *      Sound    ：到点播“一次”音效（播完销毁）。
         *  三者共享驱动骨架（EditorApplication.update + 幂等 tick），语义不同。
         *
         *
         *  【二、它做什么 / 不做什么】
         *  ------------------------------------------------------------------------
         *  【做】
         *    1. 持有 from / to / lerp / stepCount / interval / loopCount / pingPong；
         *    2. 每帧检查是否到点，到点推进 step，算 t，调 onStep；
         *    3. 根据 loopCount 决定循环、播完停住；
         *    4. 统一 Stop：清空状态 + 摘 tick。
         *
         *  【不做】
         *    - 不知道 T 是什么类型（float / Vector2/3/4 / Color / int 都由调用方决定）；
         *    - 不自己插值（由调用方传 lerp 委托）；
         *    - 不自己 Repaint；
         *    - 不接触资源加载 / 业务概念。
         *
         *
         *  【三、驱动归属：为什么用 EditorApplication.update】
         *  ------------------------------------------------------------------------
         *  同 Sequence。EditorApplication.update 才是编辑器
         *  真正每帧都跑的驱动；MonoBehaviour.Update 在编辑器空闲时会停。
         *
         *
         *  【四、泛型怎么处理（关键设计）】
         *  ------------------------------------------------------------------------
         *  Play<T> 是泛型方法，但 Tick 是 static 非泛型。
         *  解法：在 Play 入口处把泛型委托“拍平”成非泛型委托：
         *
         *      Func<T,T,float,T>  →  Func<object,object,float,object>
         *      Action<float,T>    →  Action<float,object>
         *
         *  Tick 只跟 object 打交道，不再需要反射。
         *  装箱开销可忽略（编辑器预览每秒几十次）。
         *
         *
         *  【五、运行时序】
         *  ------------------------------------------------------------------------
         *  【阶段 1：Play】
         *      Play<T>(from, to, lerp, stepCount, interval, onStep, loopCount, pingPong, ease, customCurve)
         *          ├─ 拍平 lerp / onStep
         *          ├─ 替换 _from / _to / _lerp / _onStep
         *          ├─ 重置 _step = 0
         *          ├─ 重置 _nextAtTime = now + interval
         *          ├─ 重置 _playedCount = 0
         *          ├─ 重置 _direction = +1（pingPong 用）
         *          ├─ 重置 _currentT / _currentRawT = 0
         *          └─ EnsureTick()  → 挂 Tick 到 EditorApplication.update
         *
         *  【阶段 2：每帧 Tick】
         *      Tick()
         *          ├─ 状态无效？→ UnhookTick()，return
         *          ├─ now < _nextAtTime？→ return（还没到点）
         *          └─ 到点：
         *               ├─ _nextAtTime = now + _interval
         *               ├─ _step++
         *               ├─ _step >= _stepCount？
         *               │     ├─ _playedCount++
         *               │     ├─ loopCount 有限 且 playedCount > loopCount？
         *               │     │     ├─ 是：_step = _stepCount
         *               │     │     │      _currentRawT = (pingPong && _direction<0) ? 0 : 1
         *               │     │     │      _currentT = ApplyEase(_currentRawT)
         *               │     │     │      算值、调回调、UnhookTick()，return
         *               │     │     └─ 否：_step = 0（还有下一遍）
         *               │     │           pingPong：_direction = -_direction
         *               │     └─ （未越界则继续）
         *               ├─ _currentRawT = _step / (float)_stepCount
         *               ├─ pingPong && _direction < 0：_currentRawT = 1 - _currentRawT
         *               ├─ _currentT = ApplyEase(_currentRawT)
         *               ├─ 算值 = _lerp(_from, _to, _currentT)
         *               └─ _onStep(_currentT, 值)
         *
         *  【阶段 3：Stop】
         *      Stop()
         *          ├─ 清空所有状态
         *          └─ UnhookTick()
         *
         *
         *  【六、循环次数语义（与 Sequence / XTween 对齐）】
         *  ------------------------------------------------------------------------
         *      -1    ：无限循环
         *       0    ：循环 1 次（播一遍就停）
         *       N>=0 ：循环 N+1 次（播 N+1 遍）
         *
         *
         *  【七、pingPong 语义】
         *  ------------------------------------------------------------------------
         *      false（默认）：每遍都从 from → to（单向）
         *      true          ：from → to → from → to ...（往返）
         *
         *  每完成一遍反向一次。播完停住时，_currentRawT 根据最后一遍方向决定：
         *      最后一遍方向为 +1 → 停在 1（即 to 端）
         *      最后一遍方向为 -1 → 停在 0（即 from 端）
         *
         *
         *  【八、缓动（Ease）语义】
         *  ------------------------------------------------------------------------
         *  【优先级】customCurve 非空时用它；否则用 ease 枚举。
         *
         *  【ApplyEase 做的事】
         *      把 raw t 映射成 eased t，再传给 lerp。
         *      CurrentT     = eased t（缓动后）
         *      CurrentRawT  = raw t（缓动前）
         *
         *  【覆盖的缓动】
         *      Linear / Quad / Cubic / Quart / Quint / Sine / Expo
         *      Circ / Back / Elastic / Bounce
         *      以及它们各自的 In / Out / InOut 变体。
         *
         *
         *  【九、状态字段一览】
         *  ------------------------------------------------------------------------
         *      _from          object                            起始值（装箱）
         *      _to            object                            结束值（装箱）
         *      _lerp          Func<object,object,float,object>  插值委托（拍平）
         *      _onStep        Action<float,object>              每步回调（拍平）
         *      _stepCount     int                               总步数
         *      _step          int                               当前步
         *      _interval      float                             每步间隔（秒）
         *      _nextAtTime    double                            下一步触发时刻
         *      _loopCount     int                               循环次数
         *      _playedCount   int                               已完成的循环次数
         *      _pingPong      bool                              是否往返
         *      _direction     int                               当前方向（+1 / -1）
         *      _currentT      float                             当前 eased t
         *      _currentRawT   float                             当前 raw t
         *      _currentValue  object                            当前值
         *      _ease          XGUIEaseMode                      缓动模式
         *      _customCurve   AnimationCurve                    自定义曲线（可选）
         *      IsTicking      bool                              tick 是否已挂
         *
         *
         *  【十、IsPlaying 与 IsTicking 的区别】
         *  ------------------------------------------------------------------------
         *      IsPlaying ：有插值委托（“有值可算”）
         *      IsTicking ：tick 已挂（“正在推进”）
         *
         *  播完后：IsPlaying 仍为 true（CurrentT / CurrentValue 还能读），IsTicking 为 false。
         *
         *
         *  【十一、幂等与自毁】
         *  ------------------------------------------------------------------------
         *  【EnsureTick 先摘再挂】
         *      - 重编译时静态字段被重置，但委托链不会自动清空；
         *      - 若直接 +=，会出现“同一个 Tick 被注册多次”，推进速度翻倍；
         *      - 先 -= 再 +=，实现真正的幂等。
         *
         *  【UnhookTick 统一摘 tick】
         *      - 所有“自然停止”（状态无效、播完）都走 UnhookTick；
         *      - 保证 IsTicking 和 tick 挂载状态始终一致。
         *
         *  【Stop 幂等】
         *      - 可重复调用，无副作用；
         *      - OnDisable / Play 参数无效 / Tick 异常都走 Stop。
         *
         *
         *  【十二、为什么是 static 单例】
         *  ------------------------------------------------------------------------
         *  同 Sequence。编辑器同一时刻只有一个预览窗口活跃，
         *  切换/关闭会触发旧窗口 OnDisable → Stop()，天然互斥。
         *
         *
         *  【十三、与 Sequence / Sound 的对比】
         *  ------------------------------------------------------------------------
         *      维度     | Values                   | Sequence                | Sound
         *      ---------|--------------------------|-------------------------|-------------------------
         *      驱动     | EditorApplication.update | 同                      | 同
         *      语义     | 到点推“t 值”             | 到点推“一格”            | 到点播“一次”
         *      数量     | 单一（当前插值）          | 单一（当前序列）        | 多个并存（队列）
         *      生命周期 | 替换 → 循环推进           | 替换 → 循环推进         | 入队 → 播 → 销毁
         *      停止     | 循环到 Stop 或耗尽       | 循环到 Stop 或耗尽       | 播完自动销毁
         *
         *  同构，不同义。共享驱动骨架，不共享业务语义。
         *
         *
         *  【十四、设计边界（再次强调）】
         *  ------------------------------------------------------------------------
         *      调用方负责：传 from / to / lerp、决定 ease、决定 pingPong、用值干嘛。
         *      播放器负责：推 t、应用 ease、回调、循环、停止、tick 管理。
         *
         *  这条边界不能破，否则 Values 会退化成“什么都干”的上帝类。
         *
         *
         *  【十五、典型用法示例】
         *  ------------------------------------------------------------------------
         *  【示例 1：float 0 → 10 无限循环】
         *      XGUI.EditorPlayer_Values.Play(
         *          from: 0f, to: 10f,
         *          lerp: (a, b, t) => Mathf.Lerp(a, b, t),
         *          stepCount: 60,
         *          interval: 1f / 60f,
         *          onStep: (t, v) => { boxWidth = v; Repaint(); },
         *          loopCount: -1);
         *
         *  【示例 2：Vector3 往返】
         *      XGUI.EditorPlayer_Values.Play(
         *          from: Vector3.zero, to: Vector3.one * 100f,
         *          lerp: (a, b, t) => Vector3.Lerp(a, b, t),
         *          stepCount: 30,
         *          interval: 0.05f,
         *          onStep: (t, v) => { previewPos = v; Repaint(); },
         *          loopCount: -1,
         *          pingPong: true);
         *
         *  【示例 3：Color 渐变一次就停】
         *      XGUI.EditorPlayer_Values.Play(
         *          from: Color.red, to: Color.blue,
         *          lerp: (a, b, t) => Color.Lerp(a, b, t),
         *          stepCount: 40,
         *          interval: 0.02f,
         *          onStep: (t, v) => { previewColor = v; Repaint(); },
         *          loopCount: 0);
         *
         *  【示例 4：套 easing 曲线】
         *      XGUI.EditorPlayer_Values.Play(
         *          from: 0f, to: 1f,
         *          lerp: (a, b, t) => Mathf.Lerp(a, b, t),
         *          stepCount: 60,
         *          interval: 1f / 60f,
         *          onStep: (t, v) => { alpha = v; Repaint(); },
         *          loopCount: 0,
         *          ease: XGUIEaseMode.OutBack);
         *
         *  【示例 5：OnGUI 里读当前值】
         *      float t = XGUI.EditorPlayer_Values.CurrentT;
         *      float rawT = XGUI.EditorPlayer_Values.CurrentRawT;
         *      Vector3 v = XGUI.EditorPlayer_Values.CurrentValue<Vector3>();
         *
         *  【示例 6：判断是否还在推进】
         *      if (XGUI.EditorPlayer_Values.IsTicking)
         *      {
         *          // 还在推进
         *      }
         *      else if (XGUI.EditorPlayer_Values.IsPlaying)
         *      {
         *          // 播完了，还能读 CurrentT / CurrentValue
         *      }
         *
         * ============================================================================
         */

        public static class EditorPlayer_Values
        {
            #region 状态
            /// <summary>
            /// 起始值（装箱）。
            /// </summary>
            private static object _from;
            /// <summary>
            /// 结束值（装箱）。
            /// </summary>
            private static object _to;
            /// <summary>
            /// 插值委托（拍平后）。
            /// </summary>
            private static Func<object, object, float, object> _lerp;
            /// <summary>
            /// 每步回调（拍平后）。
            /// </summary>
            private static Action<float, object> _onStep;
            /// <summary>
            /// 总步数。
            /// </summary>
            private static int _stepCount;
            /// <summary>
            /// 当前步。
            /// </summary>
            private static int _step;
            /// <summary>
            /// 每步间隔（秒）。
            /// </summary>
            private static float _interval;
            /// <summary>
            /// 下一步触发时刻。
            /// </summary>
            private static double _nextAtTime;
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
            /// </summary>
            private static int _playedCount;
            /// <summary>
            /// 是否往返。
            /// </summary>
            private static bool _pingPong;
            /// <summary>
            /// 当前方向（+1 / -1）。
            /// </summary>
            private static int _direction = 1;
            /// <summary>
            /// 当前 t（供 CurrentT 查询）。
            /// </summary>
            private static float _currentT;
            /// <summary>
            /// 当前值（供 CurrentValue 查询）。
            /// </summary>
            private static object _currentValue;
            /// <summary>
            /// 缓动模式。
            /// </summary>
            private static XGUIEaseMode _ease = XGUIEaseMode.Linear;
            /// <summary>
            /// 自定义缓动曲线（可选，优先于 _ease）。
            /// </summary>
            private static AnimationCurve _customCurve;
            /// <summary>
            /// 当前 raw t（缓动前）。
            /// </summary>
            private static float _currentRawT;
            #endregion

            #region 播放控制
            /// <summary>
            /// 开始（或替换）数值插值预览。
            /// <para/>
            /// 【参数】
            /// <list type="bullet">
            /// <item><description>from / to：起始值和结束值。</description></item>
            /// <item><description>lerp：插值委托，签名 (a, b, t) =&gt; 结果。</description></item>
            /// <item><description>stepCount：总步数（决定播完用多久 = stepCount * interval）。</description></item>
            /// <item><description>interval：每步间隔（秒），会被 Clamp 到 [0.01, 1]。</description></item>
            /// <item><description>onStep：每步回调，参数 (t, value)，t ∈ [0, 1]。</description></item>
            /// <item><description>loopCount：循环次数。-1 无限；0 播一遍；N 播 N+1 遍。默认 -1。</description></item>
            /// <item><description>pingPong：true 往返；false 单向。默认 false。</description></item>
            /// </list>
            /// <para/>
            /// 【行为】
            /// <list type="bullet">
            /// <item><description>替换当前播放内容，步数归零，计时重置，播放计数归零；</description></item>
            /// <item><description>确保 tick 已注册（幂等）。</description></item>
            /// </list>
            /// </summary>
            public static void Play<T>(
                T from,
                T to,
                Func<T, T, float, T> lerp,
                int stepCount,
                float interval,
                Action<float, T> onStep,
                int loopCount = -1,
                bool pingPong = false,
                XGUIEaseMode ease = XGUIEaseMode.Linear,
                AnimationCurve customCurve = null)
            {
                if (lerp == null || onStep == null || stepCount <= 0)
                {
                    Stop();
                    return;
                }

                Func<object, object, float, object> lerpBoxed =
                    (a, b, t) => lerp((T)a, (T)b, t);
                Action<float, object> onStepBoxed =
                    (t, v) => onStep(t, (T)v);

                _from = from;
                _to = to;
                _lerp = lerpBoxed;
                _onStep = onStepBoxed;
                _stepCount = stepCount;
                _step = 0;
                _interval = Mathf.Clamp(interval, 0.01f, 1f);
                _nextAtTime = EditorApplication.timeSinceStartup + _interval;
                _loopCount = loopCount;
                _playedCount = 0;
                _pingPong = pingPong;
                _direction = 1;
                _ease = ease;
                _customCurve = customCurve;
                _currentRawT = 0f;
                _currentT = 0f;
                _currentValue = from;

                EnsureTick();
            }
            /// <summary>
            /// 停止播放并清空状态。
            /// <para/>【幂等】可重复调用，无副作用。
            /// </summary>
            public static void Stop()
            {
                _from = null;
                _to = null;
                _lerp = null;
                _onStep = null;
                _stepCount = 0;
                _step = 0;
                _interval = 0f;
                _loopCount = -1;
                _playedCount = 0;
                _pingPong = false;
                _direction = 1;
                _ease = XGUIEaseMode.Linear;
                _customCurve = null;
                _currentRawT = 0f;
                _currentT = 0f;
                _currentValue = null;
                UnhookTick();
            }
            #endregion

            #region 查询
            /// <summary>
            /// 当前 t 值（未播放时返回 0）。
            /// </summary>
            public static float CurrentT => _currentT;
            /// <summary>
            /// 当前 raw t（缓动前，未 pingPong 变换）。
            /// </summary>
            public static float CurrentRawT => _currentRawT;
            /// <summary>
            /// 当前值（未播放时返回 default）。
            /// <para/>调用时指定 T，如 <c>CurrentValue&lt;Vector3&gt;()</c>。
            /// </summary>
            public static T CurrentValue<T>() => _currentValue is T v ? v : default;

            /// <summary>
            /// 是否正在播放（有插值委托）。不表示“是否在推进”。
            /// </summary>
            public static bool IsPlaying => _lerp != null;

            /// <summary>
            /// 是否已挂 tick（正在推进）。
            /// <para/>
            /// 与 IsPlaying 的区别：
            /// IsPlaying 表示“有值可算”；
            /// IsTicking 表示“tick 已挂，正在推进”。
            /// 播完后，IsPlaying 为 true（CurrentT / CurrentValue 还能读），IsTicking 为 false。
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
            /// 导致推进速度翻倍。
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
            /// 所有“自然停止”（状态无效、播完）都走这里，
            /// 保证 IsTicking 和 tick 挂载状态始终一致。
            /// </summary>
            private static void UnhookTick()
            {
                EditorApplication.update -= Tick;
                IsTicking = false;
            }
            /// <summary>
            /// 每帧驱动：到点推进步数并回调。
            /// <para/>
            /// 【自毁机制】状态无效或播完时摘除 tick，避免空转。
            /// <para/>
            /// 【重编译兜底】用 try/catch 包住，重编译时可能访问到已失效的对象，
            /// 捕获异常后主动 Stop，避免 tick 残留。
            /// </summary>
            private static void Tick()
            {
                try
                {
                    if (_lerp == null || _onStep == null || _stepCount <= 0)
                    {
                        UnhookTick();
                        return;
                    }

                    double now = EditorApplication.timeSinceStartup;
                    if (now < _nextAtTime) return;

                    _nextAtTime = now + _interval;

                    _step++;
                    if (_step >= _stepCount)
                    {
                        _playedCount++;

                        if (_loopCount >= 0 && _playedCount > _loopCount)
                        {
                            // 播完：停在自然终点
                            _step = _stepCount;
                            _currentRawT = (_pingPong && _direction < 0) ? 0f : 1f;
                            _currentT = ApplyEase(_currentRawT, _ease, _customCurve);
                            _currentValue = _lerp(_from, _to, _currentT);
                            _onStep(_currentT, _currentValue);
                            UnhookTick();
                            return;
                        }

                        _step = 0;
                        if (_pingPong) _direction = -_direction;
                    }

                    // raw t
                    _currentRawT = _step / (float)_stepCount;
                    if (_pingPong && _direction < 0)
                        _currentRawT = 1f - _currentRawT;

                    // eased t
                    _currentT = ApplyEase(_currentRawT, _ease, _customCurve);

                    _currentValue = _lerp(_from, _to, _currentT);
                    _onStep(_currentT, _currentValue);
                }
                catch { Stop(); }
            }
            /// <summary>
            /// 对 t 应用缓动。
            /// <para/>
            /// 【优先级】customCurve 非空时用它；否则用 ease 枚举。
            /// </summary>
            public static float ApplyEase(float t, XGUIEaseMode ease, AnimationCurve customCurve = null)
            {
                if (customCurve != null)
                    return customCurve.Evaluate(t);

                switch (ease)
                {
                    case XGUIEaseMode.Linear: return t;

                    case XGUIEaseMode.InQuad: return t * t;
                    case XGUIEaseMode.OutQuad: return 1f - (1f - t) * (1f - t);
                    case XGUIEaseMode.InOutQuad: return t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;

                    case XGUIEaseMode.InCubic: return t * t * t;
                    case XGUIEaseMode.OutCubic: return 1f - Mathf.Pow(1f - t, 3f);
                    case XGUIEaseMode.InOutCubic: return t < 0.5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;

                    case XGUIEaseMode.InQuart: return t * t * t * t;
                    case XGUIEaseMode.OutQuart: return 1f - Mathf.Pow(1f - t, 4f);
                    case XGUIEaseMode.InOutQuart: return t < 0.5f ? 8f * t * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 4f) / 2f;

                    case XGUIEaseMode.InQuint: return t * t * t * t * t;
                    case XGUIEaseMode.OutQuint: return 1f - Mathf.Pow(1f - t, 5f);
                    case XGUIEaseMode.InOutQuint: return t < 0.5f ? 16f * t * t * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 5f) / 2f;

                    case XGUIEaseMode.InSine: return 1f - Mathf.Cos((t * Mathf.PI) / 2f);
                    case XGUIEaseMode.OutSine: return Mathf.Sin((t * Mathf.PI) / 2f);
                    case XGUIEaseMode.InOutSine: return -(Mathf.Cos(Mathf.PI * t) - 1f) / 2f;

                    case XGUIEaseMode.InExpo: return t == 0f ? 0f : Mathf.Pow(2f, 10f * t - 10f);
                    case XGUIEaseMode.OutExpo: return t == 1f ? 1f : 1f - Mathf.Pow(2f, -10f * t);
                    case XGUIEaseMode.InOutExpo:
                        if (t == 0f) return 0f;
                        if (t == 1f) return 1f;
                        return t < 0.5f ? Mathf.Pow(2f, 20f * t - 10f) / 2f : (2f - Mathf.Pow(2f, -20f * t + 10f)) / 2f;

                    case XGUIEaseMode.InCirc: return 1f - Mathf.Sqrt(1f - t * t);
                    case XGUIEaseMode.OutCirc: return Mathf.Sqrt(1f - Mathf.Pow(t - 1f, 2f));
                    case XGUIEaseMode.InOutCirc:
                        return t < 0.5f
                            ? (1f - Mathf.Sqrt(1f - Mathf.Pow(2f * t, 2f))) / 2f
                            : (Mathf.Sqrt(1f - Mathf.Pow(-2f * t + 2f, 2f)) + 1f) / 2f;

                    case XGUIEaseMode.InBack: return 2.70158f * t * t * t - 1.70158f * t * t;
                    case XGUIEaseMode.OutBack: return 1f + 2.70158f * Mathf.Pow(t - 1f, 3f) + 1.70158f * Mathf.Pow(t - 1f, 2f);
                    case XGUIEaseMode.InOutBack:
                        return t < 0.5f
                            ? (Mathf.Pow(2f * t, 2f) * ((2.5949095f + 1f) * 2f * t - 2.5949095f)) / 2f
                            : (Mathf.Pow(2f * t - 2f, 2f) * ((2.5949095f + 1f) * (t * 2f - 2f) + 2.5949095f) + 2f) / 2f;

                    case XGUIEaseMode.InElastic:
                        if (t == 0f) return 0f;
                        if (t == 1f) return 1f;
                        return -Mathf.Pow(2f, 10f * t - 10f) * Mathf.Sin((t * 10f - 10.75f) * (2f * Mathf.PI) / 3f);

                    case XGUIEaseMode.OutElastic:
                        if (t == 0f) return 0f;
                        if (t == 1f) return 1f;
                        return Mathf.Pow(2f, -10f * t) * Mathf.Sin((t * 10f - 0.75f) * (2f * Mathf.PI) / 3f) + 1f;

                    case XGUIEaseMode.InOutElastic:
                        if (t == 0f) return 0f;
                        if (t == 1f) return 1f;
                        return t < 0.5f
                            ? -(Mathf.Pow(2f, 20f * t - 10f) * Mathf.Sin((20f * t - 11.125f) * (2f * Mathf.PI) / 4.5f)) / 2f
                            : (Mathf.Pow(2f, -20f * t + 10f) * Mathf.Sin((20f * t - 11.125f) * (2f * Mathf.PI) / 4.5f)) / 2f + 1f;

                    case XGUIEaseMode.InBounce: return 1f - OutBounce(1f - t);
                    case XGUIEaseMode.OutBounce: return OutBounce(t);
                    case XGUIEaseMode.InOutBounce:
                        return t < 0.5f
                            ? (1f - OutBounce(1f - 2f * t)) / 2f
                            : (1f + OutBounce(2f * t - 1f)) / 2f;
                }

                return t;
            }
            private static float OutBounce(float t)
            {
                const float n1 = 7.5625f;
                const float d1 = 2.75f;

                if (t < 1f / d1) return n1 * t * t;
                if (t < 2f / d1) { t -= 1.5f / d1; return n1 * t * t + 0.75f; }
                if (t < 2.5f / d1) { t -= 2.25f / d1; return n1 * t * t + 0.9375f; }
                t -= 2.625f / d1;
                return n1 * t * t + 0.984375f;
            }
            #endregion
        }
    }
}