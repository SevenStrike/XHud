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
namespace SevenStrikeModules.XTween
{
    using UnityEngine;

    public static partial class XTween
    {
        public static XTween_Interface To(XTween_Getter<string> getter, XTween_Setter<string> setter, bool extended, float blinkInterval, string cursor, string endValue, float duration, bool autokill = true, bool rewind_set_startvalue = true, bool complete_set_endvalue = true)
        {
            string startText = getter();
            // 前缀：extended = true 时保留 startText；extended = false 时前缀为空（经典打字机）
            string prefix = extended ? startText : string.Empty;

            if (Application.isPlaying)
            {
                var tweener = XTween_Pool.CreateTween<XTween_Specialized_String>();
                // ⚠️ 关键修复：_EndValue 就是增量部分，不拼接 startText
                tweener.Initialize(startText, endValue, duration * XTween_Dashboard.DurationMultiply);

                tweener.OnUpdate((currentText, linearProgress, time) =>
                {
                    bool showCursor = false;
                    if (!string.IsNullOrEmpty(cursor))
                    {
                        float t = Time.time / blinkInterval;
                        showCursor = Mathf.FloorToInt(t) % 2 == 0;
                    }
                    // 前缀 + 打字区 + 光标
                    setter(prefix + currentText + (showCursor ? cursor : ""));
                })
                .OnRewind(() =>
                {
                    if (rewind_set_startvalue)
                        setter(prefix);   // ← 回到前缀状态（extended=false 时就是空串）
                })
                .OnComplete((duration) =>
                {
                    if (complete_set_endvalue)
                        setter(prefix + endValue);   // ← 完整目标（不再重复前缀）
                })
                .SetAutokill(autokill);

                return tweener;
            }
            else
            {
                XTween_Interface tweener = new XTween_Specialized_String(startText, endValue, duration * XTween_Dashboard.DurationMultiply)
                    .OnUpdate((currentText, linearProgress, time) =>
                    {
                        bool showCursor = false;
                        if (!string.IsNullOrEmpty(cursor))
                        {
                            float t = Time.time / blinkInterval;
                            showCursor = Mathf.FloorToInt(t) % 2 == 0;
                        }
                        setter(prefix + currentText + (showCursor ? cursor : ""));
                    })
                    .OnRewind(() =>
                    {
                        if (rewind_set_startvalue)
                            setter(prefix);
                    })
                    .OnComplete((duration) =>
                    {
                        if (complete_set_endvalue)
                            setter(prefix + endValue);
                    })
                    .SetAutokill(false);

                return tweener;
            }
        }
        public static XTween_Interface To(XTween_Getter<string> getter, XTween_Setter<string> setter, bool extended, float blinkInterval, string cursor, string endValue, float duration, bool autokill, EaseMode easeMode, bool isFromMode, XTween_Getter<string> fromvalue, bool useCurve, AnimationCurve curve)
        {
            string startText = getter();
            string prefix = string.Empty;
            if (extended)
            {
                prefix = isFromMode ? fromvalue() : startText;
            }

            if (Application.isPlaying)
            {
                var tweener = XTween_Pool.CreateTween<XTween_Specialized_String>();
                tweener.Initialize(startText, endValue, duration * XTween_Dashboard.DurationMultiply);

                if (useCurve)
                    tweener.SetEase(curve);
                else
                    tweener.SetEase(easeMode);

                tweener.OnUpdate((currentText, linearProgress, time) =>
                {
                    bool showCursor = false;
                    if (!string.IsNullOrEmpty(cursor))
                    {
                        float t = Time.time / blinkInterval;
                        showCursor = Mathf.FloorToInt(t) % 2 == 0;
                    }
                    setter(prefix + currentText + (showCursor ? cursor : ""));
                })
                .OnRewind(() =>
                {
                    setter(prefix);
                })
                .OnComplete((duration) =>
                {
                    setter(prefix + endValue);
                })
                .SetAutokill(autokill);

                return tweener;
            }
            else
            {
                XTween_Interface tweener = new XTween_Specialized_String(startText, endValue, duration * XTween_Dashboard.DurationMultiply);

                if (useCurve)
                    tweener.SetEase(curve);
                else
                    tweener.SetEase(easeMode);

                tweener.OnUpdate<string>((currentText, linearProgress, time) =>
                {
                    bool showCursor = false;
                    if (!string.IsNullOrEmpty(cursor))
                    {
                        float t = Time.time / blinkInterval;
                        showCursor = Mathf.FloorToInt(t) % 2 == 0;
                    }
                    setter(prefix + currentText + (showCursor ? cursor : ""));
                })
                .OnRewind(() =>
                {
                    setter(prefix);
                })
                .OnComplete((duration) =>
                {
                    setter(prefix + endValue);
                })
                .SetAutoKill(false);

                return tweener;
            }
        }
    }
}
