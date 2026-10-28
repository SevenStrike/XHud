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

    public class XTween_Specialized_String : XTween_Base<string>
    {
        public XTween_Specialized_String(string defaultFromValue, string endValue, float duration) : base(defaultFromValue, endValue, duration)
        {
        }

        public XTween_Specialized_String()
        {
            _DefaultValue = string.Empty;
            _EndValue = string.Empty;
            _Duration = 1;
            _StartValue = string.Empty;
            _CustomEaseCurve = null;
            _UseCustomEaseCurve = false;

            ResetState();
        }

        protected override string Lerp(string a, string b, float t)
        {
            // 字符串类型不支持 Lerp，直接返回目标值
            return b;
        }

        protected override string GetDefaultValue()
        {
            return string.Empty;
        }

        public override XTween_Base<string> ReturnSelf()
        {
            return this;
        }

        protected override string CalculateCurrentValue()
        {
            // ── 起点字符数 ──
            // IsFromMode 时，以 _StartValue.Length 作为打字起始位置；
            // 非 IsFromMode 时，从空字符串开始（打字机默认行为）。
            // 注意：String 的 Substring 只能从头截取，如果 _StartValue 不是 _EndValue 的前缀，
            // 视觉上会从 _EndValue 的前 startCharCount 个字符开始打字（不会崩，但不会匹配 _StartValue 内容）。
            int startCharCount = 0;
            if (_IsFromMode && !string.IsNullOrEmpty(_StartValue))
            {
                startCharCount = Mathf.Min(_StartValue.Length, _EndValue.Length);
            }

            bool isYoyoReversing = _loopType == XTween_LoopType.Yoyo && _IsReversing;
            float progress = _CurrentLinearProgress;

            // ── 端点精确返回 ──
            if (progress >= 0.9999f)
            {
                // 正向终点 = 完整 _EndValue；反向终点 = 回退到起点字符数
                return isYoyoReversing
                    ? _EndValue.Substring(0, startCharCount)
                    : _EndValue;
            }
            if (progress <= 0.0001f)
            {
                // 正向起点 = 起点字符数；反向起点 = 完整 _EndValue
                return isYoyoReversing
                    ? _EndValue
                    : _EndValue.Substring(0, startCharCount);
            }

            // ── 中间段：按缓动进度计算当前字符数 ──
            float easedProgress = CalculateEasedProgress(progress);
            if (isYoyoReversing)
                easedProgress = 1f - easedProgress;   // ← 补上反向翻转

            int charCount = Mathf.RoundToInt(
                Mathf.Lerp(startCharCount, _EndValue.Length, easedProgress));
            charCount = Mathf.Clamp(charCount, 0, _EndValue.Length);

            return _EndValue.Substring(0, charCount);
        }
    }
}