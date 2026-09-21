/*
 * ============================================================================
 * ⚠️ 版权声明（禁止删除、禁止修改、衍生作品必须保留此注释）⚠️
 * ============================================================================
 * 版权声明 Copyright (C) 2025-Present Nanjing SevenStrike Media Co., Ltd.
 * 中文名称：南京塞维斯传媒有限公司
 * 英文名称：SevenStrikeMedia
 * 项目作者：徐寅智
 * 项目名称：XGUI - Unity Editor界面可视化组件工具
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
    using UnityEngine;

    /// <summary>
    /// 所有支持的缓动类型
    /// </summary>
    public enum EaseType
    {
        Linear,
        InSine,
        OutSine,
        InOutSine,
        InQuad,
        OutQuad,
        InOutQuad,
        InCubic,
        OutCubic,
        InOutCubic,
        InQuart,
        OutQuart,
        InOutQuart,
        InQuint,
        OutQuint,
        InOutQuint,
        InExpo,
        OutExpo,
        InOutExpo,
        InCirc,
        OutCirc,
        InOutCirc,
        InBack,
        OutBack,
        InOutBack,
        InElastic,
        OutElastic,
        InOutElastic,
        InBounce,
        OutBounce,
        InOutBounce,
        // ✅ Shake & Flash 系列
        Shake,
        ShakeX,
        ShakeY,
        Flash,
        PulsingShake,
        SmoothShake,
        SharpFlash,
        Glitch,
        // ✅ 新增：闪光渐暗
        FlashThenFade,      // 先闪光后渐暗
        FlashThenFadeOut,   // 先闪光后淡出
        BrightPulse,        // 明亮脉冲
        FlashHoldFade,      // 闪光保持然后渐暗
    }

    /// <summary>
    /// 标准缓动曲线生成器 - 输出范围严格在 [0,1] x [0,1]
    /// </summary>
    public static class XGUI_EaseCurveGenerator
    {
        /// <summary>
        /// 生成曲线，默认采样30个关键帧，保证精度且不过度
        /// </summary>
        public static AnimationCurve EaseType_Generate(EaseType type, int samples = 60)
        {
            if (samples < 2) samples = 2;
            if (samples > 100) samples = 100;

            var keys = new Keyframe[samples];

            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)(samples - 1);
                float value = EvaluateEase(type, t);

                // 计算切线（用于平滑插值）
                float dt = 0.0001f;
                float slope = (EvaluateEase(type, t + dt) - EvaluateEase(type, t - dt)) / (2 * dt);

                keys[i] = new Keyframe(t, value, slope, slope);
            }

            var curve = new AnimationCurve(keys);

            // 设置所有关键帧为平滑切线模式
            for (int i = 0; i < curve.keys.Length; i++)
            {
                curve.SmoothTangents(i, 0f);
            }

            return curve;
        }

        /// <summary>
        /// 获取所有缓动类型
        /// </summary>
        public static EaseType[] EaseTypes_Get()
        {
            return (EaseType[])System.Enum.GetValues(typeof(EaseType));
        }

        /// <summary>
        /// 核心缓动函数
        /// </summary>
        private static float EvaluateEase(EaseType type, float t)
        {
            t = Mathf.Clamp01(t);

            switch (type)
            {
                // ========== Linear ==========
                case EaseType.Linear:
                    return t;

                // ========== Sine ==========
                case EaseType.InSine:
                    return 1f - Mathf.Cos(t * Mathf.PI * 0.5f);
                case EaseType.OutSine:
                    return Mathf.Sin(t * Mathf.PI * 0.5f);
                case EaseType.InOutSine:
                    return 0.5f * (1f - Mathf.Cos(t * Mathf.PI));

                // ========== Quad ==========
                case EaseType.InQuad:
                    return t * t;
                case EaseType.OutQuad:
                    return t * (2f - t);
                case EaseType.InOutQuad:
                    return t < 0.5f ? 2f * t * t : -1f + (4f - 2f * t) * t;

                // ========== Cubic ==========
                case EaseType.InCubic:
                    return t * t * t;
                case EaseType.OutCubic:
                    return 1f - Mathf.Pow(1f - t, 3f);
                case EaseType.InOutCubic:
                    return t < 0.5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) * 0.5f;

                // ========== Quart ==========
                case EaseType.InQuart:
                    return t * t * t * t;
                case EaseType.OutQuart:
                    return 1f - Mathf.Pow(1f - t, 4f);
                case EaseType.InOutQuart:
                    return t < 0.5f ? 8f * t * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 4f) * 0.5f;

                // ========== Quint ==========
                case EaseType.InQuint:
                    return t * t * t * t * t;
                case EaseType.OutQuint:
                    return 1f - Mathf.Pow(1f - t, 5f);
                case EaseType.InOutQuint:
                    return t < 0.5f ? 16f * t * t * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 5f) * 0.5f;

                // ========== Expo ==========
                case EaseType.InExpo:
                    return t == 0f ? 0f : Mathf.Pow(2f, 10f * (t - 1f));
                case EaseType.OutExpo:
                    return t == 1f ? 1f : 1f - Mathf.Pow(2f, -10f * t);
                case EaseType.InOutExpo:
                    if (t == 0f) return 0f;
                    if (t == 1f) return 1f;
                    return t < 0.5f ? Mathf.Pow(2f, 20f * t - 10f) * 0.5f : (2f - Mathf.Pow(2f, -20f * t + 10f)) * 0.5f;

                // ========== Circ ==========
                case EaseType.InCirc:
                    return 1f - Mathf.Sqrt(1f - t * t);
                case EaseType.OutCirc:
                    return Mathf.Sqrt(1f - Mathf.Pow(t - 1f, 2f));
                case EaseType.InOutCirc:
                    return t < 0.5f
                        ? 0.5f * (1f - Mathf.Sqrt(1f - 4f * t * t))
                        : 0.5f * (Mathf.Sqrt(1f - Mathf.Pow(-2f * t + 2f, 2f)) + 1f);

                // ========== Back ==========
                case EaseType.InBack:
                    {
                        float c1 = 1.70158f;
                        float c3 = c1 + 1f;
                        return c3 * t * t * t - c1 * t * t;
                    }
                case EaseType.OutBack:
                    {
                        float c1 = 1.70158f;
                        float c3 = c1 + 1f;
                        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
                    }
                case EaseType.InOutBack:
                    {
                        float c1 = 1.70158f;
                        float c2 = c1 * 1.525f;
                        return t < 0.5f
                            ? (Mathf.Pow(2f * t, 2f) * ((c2 + 1f) * 2f * t - c2)) * 0.5f
                            : (Mathf.Pow(2f * t - 2f, 2f) * ((c2 + 1f) * (t * 2f - 2f) + c2) + 2f) * 0.5f;
                    }

                // ========== Elastic ==========
                case EaseType.InElastic:
                    {
                        if (t == 0f) return 0f;
                        if (t == 1f) return 1f;
                        float c4 = (2f * Mathf.PI) / 3f;
                        return -Mathf.Pow(2f, 10f * t - 10f) * Mathf.Sin((t * 10f - 10.75f) * c4);
                    }
                case EaseType.OutElastic:
                    {
                        if (t == 0f) return 0f;
                        if (t == 1f) return 1f;
                        float c4 = (2f * Mathf.PI) / 3f;
                        return Mathf.Pow(2f, -10f * t) * Mathf.Sin((t * 10f - 0.75f) * c4) + 1f;
                    }
                case EaseType.InOutElastic:
                    {
                        if (t == 0f) return 0f;
                        if (t == 1f) return 1f;
                        float c5 = (2f * Mathf.PI) / 4.5f;
                        return t < 0.5f
                            ? -(Mathf.Pow(2f, 20f * t - 10f) * Mathf.Sin((20f * t - 11.125f) * c5)) * 0.5f
                            : (Mathf.Pow(2f, -20f * t + 10f) * Mathf.Sin((20f * t - 11.125f) * c5)) * 0.5f + 1f;
                    }

                // ========== Bounce ==========
                case EaseType.InBounce:
                    return 1f - EvaluateEase(EaseType.OutBounce, 1f - t);
                case EaseType.OutBounce:
                    {
                        float n1 = 7.5625f;
                        float d1 = 2.75f;
                        if (t < 1f / d1)
                            return n1 * t * t;
                        if (t < 2f / d1)
                            return n1 * (t -= 1.5f / d1) * t + 0.75f;
                        if (t < 2.5f / d1)
                            return n1 * (t -= 2.25f / d1) * t + 0.9375f;
                        return n1 * (t -= 2.625f / d1) * t + 0.984375f;
                    }
                case EaseType.InOutBounce:
                    return t < 0.5f
                        ? (1f - EvaluateEase(EaseType.OutBounce, 1f - 2f * t)) * 0.5f
                        : (1f + EvaluateEase(EaseType.OutBounce, 2f * t - 1f)) * 0.5f;

                // ========== Shake & Flash ==========
                case EaseType.Shake:
                    return EvaluateShake(t, 5, 0.5f);
                case EaseType.ShakeX:
                    return EvaluateShake(t, 3, 0.3f);
                case EaseType.ShakeY:
                    return EvaluateShake(t, 4, 0.4f);
                case EaseType.Flash:
                    return EvaluateFlash(t, 8, 0.3f);
                case EaseType.PulsingShake:
                    return EvaluatePulsingShake(t, 6, 0.5f);
                case EaseType.SmoothShake:
                    return EvaluateSmoothShake(t, 4, 0.4f);
                case EaseType.SharpFlash:
                    return EvaluateSharpFlash(t, 10, 0.2f);
                case EaseType.Glitch:
                    return EvaluateGlitch(t, 8, 0.4f);

                // ========== Flash & Fade ==========
                case EaseType.FlashThenFade:
                    return EvaluateFlashThenFade(t, 1.5f, 2.0f);
                case EaseType.FlashThenFadeOut:
                    return EvaluateFlashThenFadeOut(t, 1.5f, 2.5f);
                case EaseType.BrightPulse:
                    return EvaluateBrightPulse(t, 2.0f);
                case EaseType.FlashHoldFade:
                    return EvaluateFlashHoldFade(t, 1.2f, 0.4f, 2.0f);

                default:
                    return t;
            }
        }

        // 实现方法
        private static float EvaluateSmoothShake(float t, int frequency = 4, float amplitude = 0.4f)
        {
            if (t <= 0) return 0f;
            if (t >= 1) return 1f;

            // 使用更平滑的正弦波
            float shake = Mathf.Sin(t * frequency * Mathf.PI * 2f);
            float smoothEnvelope = Mathf.Sin(t * Mathf.PI) * 0.5f + 0.5f;
            float decay = Mathf.Exp(-t * 2f * amplitude);

            float result = 0.5f + shake * smoothEnvelope * decay * 0.5f;
            return Mathf.Clamp01(result);
        }

        private static float EvaluateSharpFlash(float t, int flashCount = 10, float intensity = 0.2f)
        {
            if (t <= 0) return 0f;
            if (t >= 1) return 1f;

            // 使用方波产生尖锐闪光
            float flash = Mathf.Sign(Mathf.Sin(t * flashCount * Mathf.PI * 2f));
            flash = Mathf.Max(0, flash); // 只在正半周闪光

            float decay = Mathf.Exp(-t * 5f * intensity);
            float result = flash * decay * 1.5f;

            // 在最后保留一个最终闪光
            if (t > 0.8f)
            {
                float finalFlash = Mathf.Sign(Mathf.Sin(t * 20f * Mathf.PI)) * 0.5f + 0.5f;
                result = Mathf.Max(result, finalFlash * 0.3f);
            }

            return Mathf.Clamp01(result);
        }

        private static float EvaluateGlitch(float t, int frequency = 8, float amplitude = 0.4f)
        {
            if (t <= 0) return 0f;
            if (t >= 1) return 1f;

            // 随机跳跃 + 震动
            float glitch = GetDeterministicRandom(t, 123) * 0.8f + 0.2f;
            float shake = Mathf.Sin(t * frequency * Mathf.PI * 2f) * 0.3f;

            // 在特定时间点产生突发跳跃
            float burst = 0f;
            for (int i = 0; i < 5; i++)
            {
                float burstTime = (i + 1) / 6f;
                float burstStrength = Mathf.Exp(-Mathf.Abs(t - burstTime) * 20f);
                burst = Mathf.Max(burst, burstStrength * GetDeterministicRandom(burstTime, i * 100));
            }

            float decay = Mathf.Exp(-t * 2f * amplitude);
            float result = Mathf.Lerp(0.5f + shake * 0.3f, glitch, burst) * decay;
            result = Mathf.Clamp01(result + t * 0.1f);

            return Mathf.Clamp01(result);
        }

        /// <summary>
        /// 确定性随机数生成（基于时间和种子）
        /// </summary>
        private static float GetDeterministicRandom(float t, int seed)
        {
            float value = Mathf.Sin(t * 1000f + seed) * 0.5f + 0.5f;
            return value - Mathf.Floor(value);
        }

        /// <summary>
        /// 标准震动效果 - 模拟物体抖动
        /// </summary>
        /// <param name="t">进度 0-1</param>
        /// <param name="frequency">震动频率</param>
        /// <param name="amplitude">振幅衰减系数</param>
        private static float EvaluateShake(float t, int frequency = 5, float amplitude = 0.5f)
        {
            if (t <= 0) return 0f;
            if (t >= 1) return 1f;

            // 使用正弦波产生震动
            float shake = Mathf.Sin(t * frequency * Mathf.PI * 2f);

            // 振幅随进度衰减（指数衰减）
            float decay = Mathf.Exp(-t * 3f * amplitude);

            // 最终值在 0-1 范围内震荡
            float result = 0.5f + shake * decay * 0.5f;

            return Mathf.Clamp01(result);
        }

        /// <summary>
        /// 闪光效果 - 模拟相机闪光或高亮闪烁
        /// </summary>
        /// <param name="t">进度 0-1</param>
        /// <param name="flashCount">闪光次数</param>
        /// <param name="intensity">强度</param>
        private static float EvaluateFlash(float t, int flashCount = 8, float intensity = 0.3f)
        {
            if (t <= 0) return 0f;
            if (t >= 1) return 1f;

            // 计算闪光脉冲
            float flash = Mathf.Sin(t * flashCount * Mathf.PI * 2f);

            // 使用绝对值产生快速闪烁
            flash = Mathf.Abs(flash);

            // 指数衰减
            float decay = Mathf.Exp(-t * 4f * intensity);

            // 产生尖锐的闪光脉冲
            float result = flash * decay * 1.2f;

            // 添加一个基础上升趋势
            result = Mathf.Lerp(result, t * 0.2f, t);

            return Mathf.Clamp01(result);
        }

        /// <summary>
        /// 脉冲震动 - 结合震动和脉冲效果
        /// </summary>
        private static float EvaluatePulsingShake(float t, int frequency = 6, float amplitude = 0.5f)
        {
            if (t <= 0) return 0f;
            if (t >= 1) return 1f;

            // 基础震动
            float shake = Mathf.Sin(t * frequency * Mathf.PI * 2f);

            // 使用脉冲包络
            float pulse = Mathf.Sin(t * Mathf.PI) * 0.8f + 0.2f;
            float decay = Mathf.Exp(-t * 2.5f * amplitude);

            // 组合
            float result = 0.5f + shake * pulse * decay * 0.5f;

            return Mathf.Clamp01(result);
        }

        /// <summary>
        /// 先闪现高亮然后慢慢变暗
        /// 适用于：技能冷却完成、新消息提醒、高亮提示
        /// </summary>
        /// <param name="t">进度 0-1</param>
        /// <param name="flashPeak">闪光峰值高度（默认1.5，即超过1.0的过冲）</param>
        /// <param name="fadeSpeed">渐暗速度（越大越快变暗）</param>
        private static float EvaluateFlashThenFade(float t, float flashPeak = 1.5f, float fadeSpeed = 2.0f)
        {
            if (t <= 0) return 0f;
            if (t >= 1) return 0f;  // 最终完全变暗

            // 第一阶段：快速上升到峰值（0 ~ 0.15）
            float riseTime = 0.15f;
            float riseValue;
            if (t <= riseTime)
            {
                // 使用缓动曲线快速上升
                float p = t / riseTime;
                riseValue = flashPeak * (p * p);  // InQuad 快速上升
                return riseValue;
            }

            // 第二阶段：从峰值慢慢衰减到0（0.15 ~ 1.0）
            float fadeStart = riseTime;
            float fadeProgress = (t - fadeStart) / (1f - fadeStart);

            // 使用指数衰减 + 缓动曲线使衰减更自然
            float decay = Mathf.Exp(-fadeProgress * fadeSpeed);
            float smoothDecay = 1f - Mathf.Pow(fadeProgress, 0.6f);  // 使开头下降快一点，后面慢

            // 组合：先快速下降，然后缓慢接近0
            float result = flashPeak * Mathf.Lerp(decay, smoothDecay, 0.5f);

            // 确保最终接近0
            result = result * (1f - Mathf.Pow(fadeProgress, 3f) * 0.3f);

            return Mathf.Max(0, result);
        }

        /// <summary>
        /// 先闪光后完全淡出（最终值归零）
        /// 适用于：提示消失、特效消失
        /// </summary>
        private static float EvaluateFlashThenFadeOut(float t, float flashPeak = 1.5f, float fadeSpeed = 2.5f)
        {
            if (t <= 0) return 0f;
            if (t >= 1) return 0f;

            // 极快上升到峰值
            float riseTime = 0.1f;
            if (t <= riseTime)
            {
                float p = t / riseTime;
                return flashPeak * (p * p * p);  // InCubic 更快速上升
            }

            // 平滑衰减到0
            float fadeStart = riseTime;
            float fadeProgress = (t - fadeStart) / (1f - fadeStart);

            // 使用更强的指数衰减
            float decay = Mathf.Exp(-fadeProgress * fadeSpeed);

            // 在最后阶段加速淡出
            float endFade = 1f - Mathf.Pow(fadeProgress, 4f);
            float result = flashPeak * decay * (0.8f + 0.2f * endFade);

            return Mathf.Max(0, result);
        }

        /// <summary>
        /// 明亮脉冲 - 快速闪光然后缓慢恢复
        /// 适用于：能量充能、充能完成
        /// </summary>
        private static float EvaluateBrightPulse(float t, float peak = 2.0f)
        {
            if (t <= 0) return 0f;
            if (t >= 1) return 1f;  // 最终恢复为1（全亮）

            // 前期急速上升
            float riseTime = 0.08f;
            if (t <= riseTime)
            {
                float p = t / riseTime;
                return peak * (p * p);  // InQuad
            }

            // 快速下降到过冲
            float overshootTime = 0.2f;
            float overshootValue = 1.2f;
            if (t <= overshootTime)
            {
                float p = (t - riseTime) / (overshootTime - riseTime);
                float decay = 1f - p;
                float overshoot = peak + (overshootValue - peak) * (p * p);
                return Mathf.Lerp(peak, overshoot, p);
            }

            // 缓慢恢复到1
            float recoverStart = overshootTime;
            float recoverProgress = (t - recoverStart) / (1f - recoverStart);

            // 使用缓动函数使恢复更自然
            float recover = 1f + (overshootValue - 1f) * Mathf.Pow(1f - recoverProgress, 2.5f);

            return Mathf.Clamp(recover, 0.5f, peak);
        }

        /// <summary>
        /// 闪光保持然后渐暗 - 闪光后保持高亮一段时间再渐暗
        /// 适用于：里程碑达成、成就解锁、重要通知
        /// </summary>
        /// <param name="holdTime">保持高亮的时间比例 (0-1)</param>
        private static float EvaluateFlashHoldFade(float t, float flashPeak = 1.2f, float holdTime = 0.4f, float fadeSpeed = 2.0f)
        {
            if (t <= 0) return 0f;
            if (t >= 1) return 0.5f;  // 最终保持50%亮度，不完全消失

            // 快速上升到峰值
            float riseTime = 0.1f;
            if (t <= riseTime)
            {
                float p = t / riseTime;
                return flashPeak * (p * p * p);  // InCubic
            }

            // 保持在峰值
            float holdStart = riseTime;
            float holdEnd = holdStart + holdTime * 0.6f;  // 保持时间

            if (t <= holdEnd)
            {
                // 轻微波动
                float holdProgress = (t - holdStart) / (holdTime * 0.6f);
                float ripple = 1f + 0.05f * Mathf.Sin(holdProgress * Mathf.PI * 4f);
                return flashPeak * ripple;
            }

            // 渐暗
            float fadeStart = holdEnd;
            float fadeProgress = (t - fadeStart) / (1f - fadeStart);

            // 指数衰减到最终值
            float decay = Mathf.Exp(-fadeProgress * fadeSpeed);
            float targetValue = 0.5f;  // 最终值
            float result = targetValue + (flashPeak - targetValue) * decay;

            // 添加轻微的呼吸效果
            float breathe = 1f + 0.03f * Mathf.Sin(fadeProgress * Mathf.PI * 2f);

            return Mathf.Clamp(result * breathe, 0.3f, flashPeak);
        }
    }
}