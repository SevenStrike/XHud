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
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// 为 Editor 界面提供样式控件
    /// </summary>
    public static partial class XGUI
    {
        /// <summary>
        /// 绘制一个高性能的虚拟化滚动视图，仅渲染可见区域内的列表项。
        /// </summary>
        /// <typeparam name="T">列表元素的类型。</typeparam>
        /// <param name="rect">滚动视图的位置和尺寸。</param>
        /// <param name="scroll">当前的滚动位置（传入引用，返回更新后的值）。</param>
        /// <param name="list">要显示的数据列表。</param>
        /// <param name="cachedItemHeights">缓存的项目高度列表（引用传递，用于优化性能）。</param>
        /// <param name="heightsDirty">指示缓存是否为脏的标志（引用传递）。为 <c>true</c> 时触发缓存重建。</param>
        /// <param name="onscroller">绘制每个列表项的回调委托。参数为项目矩形区域和对应的数据项。</param>
        /// <param name="fixheight">每个列表项的固定高度值。默认值为 65 像素。</param>
        /// <returns>更新后的滚动位置向量。</returns>
        /// <remarks>
        /// 该方法实现了虚拟化滚动（UI Virtualization），通过计算可见范围仅渲染当前视口内的列表项，
        /// 避免了为大量数据创建过多 GUI 元素导致的性能问题。
        /// <para>
        /// 工作原理：
        /// <list type="number">
        /// <item><description>检查并更新项目高度缓存</description></item>
        /// <item><description>计算所有项目的总高度以确定滚动内容区域</description></item>
        /// <item><description>根据当前滚动位置计算可见范围</description></item>
        /// <item><description>仅对可见范围内的项目调用 <paramref name="onscroller"/> 回调进行绘制</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// 该方法会在可见范围前后各增加 2 个项目的缓冲区，防止快速滚动时出现闪烁。
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// private Vector2 scrollPos;
        /// private List&lt;string&gt; items = new List&lt;string&gt;();
        /// private List&lt;float&gt; heights = new List&lt;float&gt;();
        /// private bool heightsDirty = true;
        /// 
        /// void OnGUI()
        /// {
        ///     Rect viewRect = new Rect(10, 10, 300, 400);
        ///     scrollPos = XGUI.gui_scrollview(
        ///         viewRect,
        ///         scrollPos,
        ///         items,
        ///         ref heights,
        ///         ref heightsDirty,
        ///         (rect, item) => {
        ///             GUI.Label(rect, item);
        ///         },
        ///         fixheight: 50f
        ///     );
        /// }
        /// </code>
        /// </example>
        public static Vector2 gui_scrollview<T>(Rect rect, Vector2 scroll, List<T> list, ref List<float> cachedItemHeights, ref bool heightsDirty, Action<Rect, T, int> onscroller = null, float fixheight = 65f)
        {
            Rect rect_scroll = rect;

            if (list == null || list.Count == 0) return Vector2.zero;

            // 更新缓存的高度（修复 null 检查）
            update_scrollview_cached_heights(ref list, ref cachedItemHeights, ref heightsDirty, fixheight);

            // 防御性检查：确保 cachedItemHeights 不为 null 且长度匹配
            if (cachedItemHeights == null || cachedItemHeights.Count != list.Count)
            {
                // 如果还是 null，强制初始化
                cachedItemHeights = new List<float>(list.Count);
                for (int i = 0; i < list.Count; i++)
                    cachedItemHeights.Add(fixheight);
                heightsDirty = false;
            }

            // 2. 计算内容总高度
            float totalHeight = 0;
            foreach (var h in cachedItemHeights)
                totalHeight += h;

            float contentWidth = rect.width - 15;
            Rect contentRect = new Rect(0, 0, contentWidth, totalHeight);

            // 3. 开始滚动视图
            Vector2 _scroll = GUI.BeginScrollView(rect, scroll, contentRect);

            // 4. 计算可见范围
            float viewTop = scroll.y;
            float viewBottom = viewTop + rect.height;

            // 找到起始索引
            float currentY = 0;
            int startIndex = -1;
            int endIndex = -1;

            for (int i = 0; i < list.Count; i++)
            {
                float itemTop = currentY;
                float itemBottom = currentY + cachedItemHeights[i];

                if (startIndex == -1 && itemBottom > viewTop)
                    startIndex = i;

                if (endIndex == -1 && itemTop > viewBottom)
                {
                    endIndex = i - 1;
                    break;
                }

                currentY += cachedItemHeights[i];
            }

            if (startIndex == -1) startIndex = 0;
            if (endIndex == -1) endIndex = list.Count - 1;

            // 添加缓冲区（±2个项目，避免滚动时闪烁）
            startIndex = Mathf.Max(0, startIndex - 2);
            endIndex = Mathf.Min(list.Count - 1, endIndex + 2);

            // 5. 计算起始Y位置
            currentY = 0;
            for (int i = 0; i < startIndex; i++)
            {
                currentY += cachedItemHeights[i];
            }

            for (int i = startIndex; i <= endIndex; i++)
            {
                T v = list[i];
                Rect itemRect = new Rect(0, currentY, contentWidth, cachedItemHeights[i]);

                if (onscroller != null)
                {
                    onscroller(itemRect, v, i);
                }

                currentY += cachedItemHeights[i];
            }

            GUI.EndScrollView();

            return _scroll;
        }
        /// <summary>
        /// 更新滚动视图中列表项的高度缓存。
        /// </summary>
        /// <typeparam name="T">列表元素的类型。</typeparam>
        /// <param name="items">数据列表的引用。</param>
        /// <param name="cachedItemHeights">缓存的项目高度列表的引用。</param>
        /// <param name="state">表示缓存是否为脏（需要更新）的标志。若为 <c>false</c> 且缓存长度匹配则跳过更新。</param>
        /// <param name="fixheight">每个列表项的固定高度值。</param>
        /// <remarks>
        /// 当缓存为空、长度不匹配或 <paramref name="state"/> 为 <c>true</c> 时，重新初始化缓存。
        /// 所有项目使用 <paramref name="fixheight"/> 指定的统一高度。
        /// </remarks>
        private static void update_scrollview_cached_heights<T>(ref List<T> items, ref List<float> cachedItemHeights, ref bool state, float fixheight)
        {
            if (!state && cachedItemHeights != null && cachedItemHeights.Count == items.Count)
                return;

            cachedItemHeights = new List<float>(items.Count);
            foreach (var item in items)
            {
                // 每个项目固定25像素高（根据你的实际情况）
                cachedItemHeights.Add(fixheight);
            }
            state = false;
        }
    }
}