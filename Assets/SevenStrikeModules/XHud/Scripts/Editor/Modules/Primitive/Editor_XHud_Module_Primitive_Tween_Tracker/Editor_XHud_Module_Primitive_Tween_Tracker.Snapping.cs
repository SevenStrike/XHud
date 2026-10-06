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
    using UnityEngine;

    public partial class Editor_XHud_Module_Primitive_Tween_Tracker
    {
        /// <summary> 
        ///吸附阈值（秒）：按像素阈值随缩放换算
        /// </summary>
        private float SnapThresholdSeconds => SnapThresholdPixels / pixelsPerSecond;
        /// <summary> 
        /// 对给定时间尝试吸附：优先吸附到其他 Clip 的边界，其次吸附到整秒网格
        /// <para/>
        /// 吸附优先级（从高到低）：
        /// <list type="number">
        /// <item><description>其他 Clip 的起始边界（<c>other.Delay</c>）；</description></item>
        /// <item><description>其他 Clip 的结束边界（<c>other.Delay + other.Duration</c>）；</description></item>
        /// <item><description>整秒网格（<see cref="Mathf.Round(float)"/>）。</description></item>
        /// </list>
        /// 之所以 Clip 边界优先：用户拖拽时更希望对齐到已有动画的首尾，
        /// 而非被整秒网格「抢走」，否则会导致视觉上明显的错位感。
        /// <para/>
        /// 判定阈值 <see cref="SnapThresholdSeconds"/> 按像素阈值换算成秒，
        /// 保证任意缩放下手感一致（缩放大时阈值小、缩放小时阈值大）。
        /// <para/>
        /// 命中判定使用<b>严格小于</b>（<c>&lt;</c>）而非小于等于，
        /// 这样多个候选时间距离相同时，会保留<b>先遍历到的</b>那个，
        /// 配合「先 Clip 边界后整秒网格」的遍历顺序，间接实现了「Clip 边界优先」。
        /// <para/>
        /// 输出 <paramref name="snappedTime"/> 一定经过 <see cref="QuantizeTime"/> 量化（两位小数），
        /// 与节点 Delay / Duration 的量化规则保持一致；未命中时直接返回原时间。
        /// </summary>
        /// <param name="time">待吸附的时间（秒）</param>
        /// <param name="exclude">排除的节点索引集合；为 null 或空表示不排除</param>
        /// <param name="snappedTime">输出：吸附后的时间（已量化）；未命中时等于 <paramref name="time"/></param>
        /// <returns>true 表示命中吸附；false 表示未命中</returns>
        private bool TrySnapTimeToReference(float time, HashSet<int> exclude, out float snappedTime)
        {
            float threshold = SnapThresholdSeconds;

            float bestTime = time;
            float bestDist = threshold;
            bool snapped = false;

            // ── 优先级 0：飞梭（如果存在有效值）──
            // 飞梭没有索引，不参与 exclude，永远作为候选。
            // 只要 playheadSecond 在合法范围内就纳入比较。
            {
                float dPlayhead = Mathf.Abs(time - playheadSecond);
                if (dPlayhead < bestDist)
                {
                    bestDist = dPlayhead;
                    bestTime = playheadSecond;
                    snapped = true;
                }
            }

            // ── 优先级 1 / 2：遍历其他 Clip 的起始与结束边界 ──
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (exclude != null && exclude.Contains(i)) continue;
                TweenNode other = Nodes[i];
                float otherStart = other.Delay;
                float otherEnd = other.Delay + other.Duration;

                float dStart = Mathf.Abs(time - otherStart);
                if (dStart < bestDist) { bestDist = dStart; bestTime = otherStart; snapped = true; }

                float dEnd = Mathf.Abs(time - otherEnd);
                if (dEnd < bestDist) { bestDist = dEnd; bestTime = otherEnd; snapped = true; }
            }

            // ── 优先级 3：整秒网格 ──
            float rounded = Mathf.Round(time);
            float distToSecond = Mathf.Abs(time - rounded);
            if (distToSecond < bestDist) { bestDist = distToSecond; bestTime = rounded; snapped = true; }

            snappedTime = snapped ? QuantizeTime(bestTime) : time;
            return snapped;
        }
        /// <summary> 
        /// 音效吸附：参照物 = 所有动画节点边界 + 所有音效边界（排除拖拽中的音效自身）+ 整秒网格。
        /// </summary>
        private bool TrySnapTimeToReferenceForSound(float time, HashSet<int> excludeSoundIndices, out float snappedTime)
        {
            float threshold = SnapThresholdSeconds;
            float bestTime = time;
            float bestDist = threshold;
            bool snapped = false;

            // ── 优先级 0：飞梭 ──
            {
                float dPlayhead = Mathf.Abs(time - playheadSecond);
                if (dPlayhead < bestDist)
                {
                    bestDist = dPlayhead;
                    bestTime = playheadSecond;
                    snapped = true;
                }
            }

            // ── 动画节点边界 ──
            for (int i = 0; i < Nodes.Count; i++)
            {
                TweenNode other = Nodes[i];
                float s = other.Delay;
                float e = other.Delay + other.Duration;
                float ds = Mathf.Abs(time - s);
                if (ds < bestDist) { bestDist = ds; bestTime = s; snapped = true; }
                float de = Mathf.Abs(time - e);
                if (de < bestDist) { bestDist = de; bestTime = e; snapped = true; }
            }

            // ── 其他音效边界 ──
            for (int i = 0; i < Sounds.Count; i++)
            {
                if (excludeSoundIndices != null && excludeSoundIndices.Contains(i)) continue;
                TweenSound other = Sounds[i];
                float s = other.Delay;
                float len = other.Sound != null ? other.Sound.length : DefaultSoundClipSeconds;
                float e = other.Delay + len;
                float ds = Mathf.Abs(time - s);
                if (ds < bestDist) { bestDist = ds; bestTime = s; snapped = true; }
                float de = Mathf.Abs(time - e);
                if (de < bestDist) { bestDist = de; bestTime = e; snapped = true; }
            }

            // ── 整秒网格 ──
            float rounded = Mathf.Round(time);
            float distToSecond = Mathf.Abs(time - rounded);
            if (distToSecond < bestDist) { bestDist = distToSecond; bestTime = rounded; snapped = true; }

            snappedTime = snapped ? QuantizeTime(bestTime) : time;
            return snapped;
        }
        /// <summary>
        /// 飞梭吸附：参照物 = 所有动画节点边界 + 所有音效边界 + 整秒网格。
        /// <para/>与 <see cref="TrySnapTimeToReference"/> / <see cref="TrySnapTimeToReferenceForSound"/>
        /// 的差异：飞梭不占用任何轨道数据，因此不做任何排除，
        /// 所有 Clip（动画 + 音效）的左右两端都是它的吸附目标。
        /// <para/>优先级：Clip 边界（先动画后音效）> 整秒网格。
        /// 命中判定使用严格小于，先遍历到的候选在距离相同时胜出。
        /// </summary>
        /// <param name="time">待吸附的时间（秒）</param>
        /// <param name="snappedTime">输出：吸附后的时间（已量化）；未命中时等于 <paramref name="time"/></param>
        /// <returns>true 表示命中吸附；false 表示未命中</returns>
        private bool TrySnapTimeForPlayhead(float time, out float snappedTime)
        {
            float threshold = SnapThresholdSeconds;
            float bestTime = time;
            float bestDist = threshold;
            bool snapped = false;

            // ── 动画节点边界 ──
            for (int i = 0; i < Nodes.Count; i++)
            {
                TweenNode other = Nodes[i];
                float s = other.Delay;
                float e = other.Delay + other.Duration;

                float ds = Mathf.Abs(time - s);
                if (ds < bestDist) { bestDist = ds; bestTime = s; snapped = true; }

                float de = Mathf.Abs(time - e);
                if (de < bestDist) { bestDist = de; bestTime = e; snapped = true; }
            }

            // ── 音效边界 ──
            for (int i = 0; i < Sounds.Count; i++)
            {
                TweenSound other = Sounds[i];
                float s = other.Delay;
                float len = other.Sound != null ? other.Sound.length : DefaultSoundClipSeconds;
                float e = other.Delay + len;

                float ds = Mathf.Abs(time - s);
                if (ds < bestDist) { bestDist = ds; bestTime = s; snapped = true; }

                float de = Mathf.Abs(time - e);
                if (de < bestDist) { bestDist = de; bestTime = e; snapped = true; }
            }

            // ── 整秒网格（兜底）──
            float rounded = Mathf.Round(time);
            float distToSecond = Mathf.Abs(time - rounded);
            if (distToSecond < bestDist) { bestDist = distToSecond; bestTime = rounded; snapped = true; }

            snappedTime = snapped ? QuantizeTime(bestTime) : time;
            return snapped;
        }
    }
}
