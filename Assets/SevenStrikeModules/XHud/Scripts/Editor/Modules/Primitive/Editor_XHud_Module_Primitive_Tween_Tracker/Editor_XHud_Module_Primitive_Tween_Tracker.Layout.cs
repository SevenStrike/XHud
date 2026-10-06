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
        /// <summary>
        /// 是否需要在动画与音效之间插入间距行。
        /// <para/>
        /// 仅当两类轨道都非空时才插入，避免只有一类时出现无意义空白。
        /// </summary>
        private bool HasTrackKindGap => Nodes.Count > 0 && Sounds.Count > 0;
        /// <summary>
        /// 全局行总数 = 动画节点数 + 音效数 + （两类都非空时的 1 行间距）。
        /// <para/>
        /// 行号顺序：
        /// <list type="bullet">
        /// <item><description>0 .. Nodes.Count-1：动画节点；</description></item>
        /// <item><description>Nodes.Count：间距行（仅当 <see cref="HasTrackKindGap"/> 为 true）；</description></item>
        /// <item><description>其余：音效。</description></item>
        /// </list>
        /// </summary>
        private int TotalRowCount =>
            Nodes.Count + Sounds.Count + (HasTrackKindGap ? 1 : 0);
        /// <summary>
        /// 将全局行号解析为 (轨道种类, 列表内索引)。
        /// <para/>
        /// 约定：
        /// <list type="bullet">
        /// <item><description><c>row &lt; Nodes.Count</c> → (Node, row)</description></item>
        /// <item><description><c>row == Nodes.Count &amp;&amp; HasTrackKindGap</c> → (Gap, -1)</description></item>
        /// <item><description>其余 → (Sound, row - Nodes.Count - (HasTrackKindGap ? 1 : 0))</description></item>
        /// </list>
        /// </summary>
        private (TrackKind kind, int index) ResolveRow(int row)
        {
            if (row < Nodes.Count) return (TrackKind.Node, row);

            if (HasTrackKindGap && row == Nodes.Count)
                return (TrackKind.Gap, -1);

            int soundOffset = Nodes.Count + (HasTrackKindGap ? 1 : 0);
            return (TrackKind.Sound, row - soundOffset);
        }
        /// <summary>
        /// 计算某一行在内容坐标系中的顶部 Y 坐标。
        /// <para/>
        /// 与旧版 <c>row * trackHeight</c> 的关键差异：
        /// 间距行的高度是 <see cref="trackKindGapHeight"/> 而非 <see cref="trackHeight"/>，
        /// 因此不能再用简单乘法，必须逐段累加。
        /// <para/>
        /// 关键：间距行"自身"的顶部 Y 只累计动画行高，**不**加间距行高度；
        /// 只有音效行才需要跨过间距行，加上 <see cref="trackKindGapHeight"/>。
        /// </summary>
        private float GetRowTopY(int row)
        {
            float y = 0f;

            // 动画行：无论 row 是动画行、间距行还是音效行，
            // 都先累计前 Nodes.Count 个动画行的高度。
            int nodeRows = Mathf.Min(row, Nodes.Count);
            y += nodeRows * trackHeight;

            if (!HasTrackKindGap) return y;

            // 只有音效行（row > Nodes.Count）才跨过间距行，
            // 需要加上间距行高度；间距行自身（row == Nodes.Count）不加。
            if (row > Nodes.Count) y += trackKindGapHeight;

            // 音效行：再加上它前面的音效行高（不含间距行那 1 行）
            if (row > Nodes.Count) y += (row - Nodes.Count - 1) * trackHeight;

            return y;
        }
        /// <summary>
        /// 由全局行号计算行的命中矩形（内容坐标）。
        /// <para/>
        /// 上下边界取整，避免行高为小数时出现 1px 缝隙或重叠。
        /// 间距行返回的矩形高度 = <see cref="trackKindGapHeight"/>。
        /// </summary>
        private Rect GetRowHitRect(int row, float width)
        {
            float y0 = Mathf.Round(GetRowTopY(row));

            var (kind, _) = ResolveRow(row);
            float h = kind == TrackKind.Gap ? trackKindGapHeight : trackHeight;
            float y1 = Mathf.Round(y0 + h);

            return new Rect(0, y0, width, y1 - y0);
        }
        /// <summary>
        /// 内容区总高度（像素）。
        /// <para/>
        /// = 所有行高之和 + 底部 20px 留白。
        /// 所有需要内容高度的地方（ScrollView 内容矩形、垂直滚动上限、名字列内容）
        /// 都必须走本方法，不能再手算 <c>TotalRowCount * trackHeight</c>，
        /// 否则间距行的高度会被漏算。
        /// </summary>
        private float GetTotalContentHeight()
        {
            float rowsHeight =
                  Nodes.Count * trackHeight
                + Sounds.Count * trackHeight
                + (HasTrackKindGap ? trackKindGapHeight : 0f);

            // ★ 无条件追加底部留白
            return rowsHeight + contentBottomPadding;
        }
        /// <summary> 
        ///计算 Clip 区垂直滚动的上限（像素）
        /// </summary>
        /// <returns>垂直滚动上限（像素），恒 ≥ 0</returns>
        private float CalculateMaxVerticalScroll()
        {
            float contentHeight = GetTotalContentHeight();
            float viewH = cachedClipAreaRect.height - rulerHeight - HorizontalScrollbarHeight;
            return Mathf.Max(0f, contentHeight - Mathf.Max(1f, viewH));
        }
        /// <summary> 
        /// 计算 Clip 区内容矩形的宽度（像素）
        /// <para/>
        /// 内容时长 = 所有 Clip 的最右端时间 + <see cref="ContentTrailingSeconds"/>（末尾富余秒数）。
        /// <para/>
        /// 内容宽度 = 内容时长 × 每秒像素数。
        /// <para/>
        /// 最后与「视口宽度 + 1」取 max，保证内容至少铺满视口，
        /// 避免右侧出现背景空白、同时保证水平滚动条永远可拖。
        /// </summary>
        /// <returns>内容宽度（像素）</returns>
        private float CalculateContentWidthPixels()
        {
            // ── 步骤 1：遍历所有节点，取最右端时间 ──
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

            // ── 步骤 2：内容时长 = 最右端 + 末尾富余秒数 ──
            // 以「秒」为单位富余，不随缩放变化：
            //   内容 4s、富余 2s → 时间轴总时长 6s，任何缩放下末尾都是 2 秒。
            float totalSeconds = maxEnd + ContentTrailingSeconds;

            // 兜底：内容为空时，至少让时间轴有一个「视口宽度对应秒数」的可操作范围，
            // 避免 maxEnd = 0 时时间轴退化成极窄一条。
            float viewportWidth = cachedClipAreaRect.width;
            if (viewportWidth <= 1f)
                viewportWidth = Mathf.Max(1f, position.width - nameColumnWidth - paramPanelWidth);

            float viewportSeconds = viewportWidth / Mathf.Max(1f, pixelsPerSecond);
            totalSeconds = Mathf.Max(totalSeconds, viewportSeconds);

            // ── 步骤 3：换算成像素宽度 ──
            float width = totalSeconds * pixelsPerSecond;

            // ── 步骤 4：至少铺满视口（+1px 保证滚动条永远可拖）──
            return Mathf.Max(width, viewportWidth + 1f);
        }
        /// <summary>
        /// 把 <see cref="scrollPos"/>.x 钳制到合法滚动范围内。
        /// <para/>
        /// 上限 = <c>contentWidth - viewWidth</c>，且下限为 0；
        /// 当内容不超过视口时上限为 0，滚动位置自动归零。
        /// <para/>
        /// 所有对 <see cref="scrollPos"/>.x 的写入都应经过本方法，
        /// 避免多个写入源（滚动条、滚轮、中键平移、缩放）之间互相超界。
        /// </summary>
        private void ClampScrollX()
        {
            float contentWidth = CalculateContentWidthPixels();
            float viewWidth = cachedClipAreaRect.width;
            float max = Mathf.Max(0f, contentWidth - viewWidth);
            scrollPos.x = Mathf.Clamp(scrollPos.x, 0f, max);
        }
    }
}
