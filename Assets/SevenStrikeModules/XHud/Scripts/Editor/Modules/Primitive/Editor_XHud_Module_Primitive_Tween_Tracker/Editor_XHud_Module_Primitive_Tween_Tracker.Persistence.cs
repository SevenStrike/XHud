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
    using SevenStrikeModules.XGUI.Editor;
    using UnityEditor;
    using UnityEngine;

    public partial class Editor_XHud_Module_Primitive_Tween_Tracker
    {
        /// <summary>
        /// 缩放到合适范围。
        /// <para/>
        /// 按当前选中状态分四种策略：
        /// <list type="bullet">
        /// <item><description>多选（总数 &gt; 1）：以选中内容（动画 + 音效）的最右端适配；</description></item>
        /// <item><description>单选动画节点：以该节点 Duration 铺满视口；</description></item>
        /// <item><description>单选音效：以该音效的音频长度（Sound 为空时用 <see cref="DefaultSoundClipSeconds"/>）铺满视口；</description></item>
        /// <item><description>无选中 / 选中项越界：以所有内容（动画 + 音效）的最右端适配。</description></item>
        /// </list>
        /// <para/>
        /// 单选项分支保持一致的"左边距 padding"手感：
        /// 若起始位置（Delay）换算成像素后大于 padding，则起点留出 padding 余量，让 Delay 部分可见；
        /// 否则从 0 开始让内容铺满整个宽度。
        /// <para/>
        /// 调用入口：快捷键 F、工具栏「合适范围」按钮、底部「缩放到全部」按钮。
        /// </summary>
        private void FitViewToContent()
        {
            // ── 前置校验：动画节点为空时退化为重置视图 ──
            // 注：这里保持与原有逻辑一致，动画节点为空即视为"无可适配内容"。
            // 若将来要支持"只有音效没有动画"的场景，需要放宽这个判断。
            if (Nodes == null || Nodes.Count == 0)
            {
                ResetTimelineView();
                return;
            }

            // ── 计算可用宽度 ──
            // 优先用缓存矩形；缓存无效时用窗口宽度减去左右两栏宽度兜底。
            float availableWidth = cachedClipAreaRect.width;
            if (availableWidth <= 1f)
            {
                availableWidth = position.width - nameColumnWidth - paramPanelWidth;
            }
            if (availableWidth <= 1f) return;

            // 两侧各留 20px 的视觉边距
            const float padding = 20f;

            // ══════════════════════════════════════════════════════════
            // 分支 0：多选（总数 > 1）→ 以选中内容的最右端适配
            // ══════════════════════════════════════════════════════════
            if (TotalSelectedCount > 1)
            {
                // 找出选中集合的左右边界
                float minDelaySel = float.MaxValue;
                float maxEndSel = 0f;

                foreach (int i in selectedNodeIndices)
                {
                    if (i < 0 || i >= Nodes.Count) continue;
                    float d = Nodes[i].Delay;
                    float e = Nodes[i].Delay + Nodes[i].Duration;
                    if (d < minDelaySel) minDelaySel = d;
                    if (e > maxEndSel) maxEndSel = e;
                }
                foreach (int i in selectedSoundIndices)
                {
                    if (i < 0 || i >= Sounds.Count) continue;
                    TweenSound s = Sounds[i];
                    float len = s.Sound != null ? s.Sound.length : DefaultSoundClipSeconds;
                    float d = s.Delay;
                    float e = s.Delay + len;
                    if (d < minDelaySel) minDelaySel = d;
                    if (e > maxEndSel) maxEndSel = e;
                }

                if (minDelaySel == float.MaxValue || maxEndSel <= 0.0001f)
                {
                    ResetTimelineView();
                    return;
                }

                float range = maxEndSel - minDelaySel;
                if (range <= 0.0001f)
                {
                    ResetTimelineView();
                    return;
                }

                float usable = Mathf.Max(1f, availableWidth - padding * 2f);
                pixelsPerSecond = Mathf.Clamp(usable / range, 1f, 20000f);

                // 把选中内容的最左端对齐到视口左侧（留 padding）
                scrollPos.x = minDelaySel * pixelsPerSecond - padding;
                ClampScrollX();   // ★ 仍要钳制，因为 minDelaySel 可能为 0 导致 scrollPos.x 变负
                scrollPos.y = 0f;
                nameScroll.y = 0f;
                Repaint();
                return;
            }

            // ══════════════════════════════════════════════════════════
            // 分支 1：单选动画节点 → 以该节点 Duration 铺满
            // ══════════════════════════════════════════════════════════
            if (TotalSelectedCount == 1
                && selectedKind == TrackKind.Node
                && selectedIndex >= 0 && selectedIndex < Nodes.Count)
            {
                TweenNode node = Nodes[selectedIndex];

                if (node.Duration <= 0.0001f) { ResetTimelineView(); return; }

                float usableWidth = Mathf.Max(1f, availableWidth - padding * 2f);
                float tempPixelsPerSecond = usableWidth / node.Duration;
                tempPixelsPerSecond = Mathf.Clamp(tempPixelsPerSecond, 1f, 20000f);

                float delayPixels = node.Delay * tempPixelsPerSecond;
                if (delayPixels > padding)
                {
                    pixelsPerSecond = tempPixelsPerSecond;
                    scrollPos.x = node.Delay * pixelsPerSecond - padding;
                    ClampScrollX();   // ★ 替换原来的 Mathf.Max(0f, ...)
                }
                else
                {
                    usableWidth = Mathf.Max(1f, availableWidth - padding);
                    pixelsPerSecond = usableWidth / node.Duration;
                    pixelsPerSecond = Mathf.Clamp(pixelsPerSecond, 1f, 20000f);
                    scrollPos.x = 0f;   // ✅ 已经是 0
                }

                scrollPos.y = 0f;
                nameScroll.y = 0f;
                Repaint();
                return;
            }

            // ══════════════════════════════════════════════════════════
            // 分支 2：单选音效 → 以该音效的音频长度铺满
            // ══════════════════════════════════════════════════════════
            if (TotalSelectedCount == 1
                && selectedKind == TrackKind.Sound
                && selectedIndex >= 0 && selectedIndex < Sounds.Count)
            {
                TweenSound sound = Sounds[selectedIndex];

                float len = sound.Sound != null ? sound.Sound.length : DefaultSoundClipSeconds;

                if (len <= 0.0001f) { ResetTimelineView(); return; }

                float usableWidth = Mathf.Max(1f, availableWidth - padding * 2f);
                float tempPixelsPerSecond = usableWidth / len;
                tempPixelsPerSecond = Mathf.Clamp(tempPixelsPerSecond, 1f, 20000f);

                float delayPixels = sound.Delay * tempPixelsPerSecond;
                if (delayPixels > padding)
                {
                    pixelsPerSecond = tempPixelsPerSecond;
                    scrollPos.x = sound.Delay * pixelsPerSecond - padding;
                    ClampScrollX();   // ★ 替换
                }
                else
                {
                    usableWidth = Mathf.Max(1f, availableWidth - padding);
                    pixelsPerSecond = usableWidth / len;
                    pixelsPerSecond = Mathf.Clamp(pixelsPerSecond, 1f, 20000f);
                    scrollPos.x = 0f;   // ✅ 已经是 0
                }

                scrollPos.y = 0f;
                nameScroll.y = 0f;
                Repaint();
                return;
            }

            // ══════════════════════════════════════════════════════════
            // 分支 3：无选中 / 选中项越界 → 以所有内容（动画 + 音效）最右端适配
            // ══════════════════════════════════════════════════════════
            float maxEnd = 0f;

            // 动画节点最右端
            for (int i = 0; i < Nodes.Count; i++)
            {
                float end = Nodes[i].Delay + Nodes[i].Duration;
                if (end > maxEnd) maxEnd = end;
            }

            // 音效最右端
            for (int i = 0; i < Sounds.Count; i++)
            {
                TweenSound s = Sounds[i];
                float len = s.Sound != null ? s.Sound.length : DefaultSoundClipSeconds;
                float end = s.Delay + len;
                if (end > maxEnd) maxEnd = end;
            }

            if (maxEnd <= 0.0001f)
            {
                ResetTimelineView();
                return;
            }

            float usableWidthAll = Mathf.Max(1f, availableWidth - padding * 2f);
            pixelsPerSecond = usableWidthAll / maxEnd;
            pixelsPerSecond = Mathf.Clamp(pixelsPerSecond, 1f, 20000f);
            scrollPos.x = 0f;
            scrollPos.y = 0f;
            nameScroll.y = 0f;
            Repaint();
        }
        /// <summary> 
        ///重置视图：水平 / 垂直滚动归零，缩放恢复默认值（100 px/s），轨道高度恢复默认值（26 px）
        /// </summary>
        private void ResetTimelineView()
        {
            scrollPos.x = 0f;
            scrollPos.y = 0f;
            nameScroll.y = 0f;
            trackHeight = 26f;

            // 让 2 秒铺满当前 Clip 区视口宽度
            float viewportWidth = cachedClipAreaRect.width;
            if (viewportWidth <= 1f)
                viewportWidth = position.width - nameColumnWidth - paramPanelWidth;
            if (viewportWidth <= 1f)
            {
                pixelsPerSecond = 100f;   // 兜底
                return;
            }

            const float targetSeconds = 5f;
            pixelsPerSecond = Mathf.Clamp(viewportWidth / targetSeconds, 1f, 20000f);
        }
        /// <summary> 
        ///从 <see cref="target"/> 读取持久化的视图状态
        /// </summary>
        private void LoadPersistedViewState()
        {
            if (target == null) return;
            pixelsPerSecond = Mathf.Clamp(target.Timeline_TrackPosition, 1f, 20000f);
            scrollPos = Vector2.Max(Vector2.zero, target.Timeline_TrackScroll);
            nameScroll.y = scrollPos.y;

            trackHeight = Mathf.Clamp(target.Timeline_TrackHeight, MinTrackHeight, MaxTrackHeight);
            snapEnabled = target.Timeline_TrackSnapEnabled;

            nameColumnWidth = LoadClampedFloatPreference(PrefKey_NameWidthWidth, DefaultNameColumnWidth, MinNameColumnWidth, MaxNameColumnWidth);
            paramPanelWidth = LoadClampedFloatPreference(PrefKey_ParamWidthHeight, DefaultParamPanelWidth, MinParamPanelWidth, MaxParamPanelWidth);
        }
        /// <summary> 
        ///将当前视图状态写回 <see cref="target"/>垂直滚动不保存
        /// </summary>
        private void SavePersistedViewState()
        {
            if (target == null) return;
            target.Timeline_TrackPosition = pixelsPerSecond;
            target.Timeline_TrackScroll = scrollPos;

            target.Timeline_TrackHeight = trackHeight;
            target.Timeline_TrackSnapEnabled = snapEnabled;

            XGUI.x_Editor_Data_Set_With_Float(PrefKey_NameWidthWidth, nameColumnWidth);
            XGUI.x_Editor_Data_Set_With_Float(PrefKey_ParamWidthHeight, paramPanelWidth);

            EditorUtility.SetDirty(target);
        }
        /// <summary> 
        ///读取持久化的 float 偏好值无记录时返回 <paramref name="defaultValue"/>，
        ///并按 <paramref name="min"/> / <paramref name="max"/> 钳制
        /// </summary>
        private static float LoadClampedFloatPreference(string key, float defaultValue, float min, float max)
        {
            if (!XGUI.x_Editor_Data_Has_String(key))
                return Mathf.Clamp(defaultValue, min, max);
            return Mathf.Clamp(XGUI.x_Editor_Data_Get_With_Float(key), min, max);
        }
        /// <summary> 
        ///读取上次关闭时保存的窗口尺寸无记录时返回 <see cref="DefaultWindowSize"/>
        /// </summary>
        /// <returns>窗口尺寸（已按 <see cref="MinWindowSize"/> / <see cref="MaxWindowSize"/> 钳制）</returns>
        private static Vector2 LoadPersistedWindowSize()
        {
            if (!XGUI.x_Editor_Data_Has_String(PrefKey_WindowWidth) || !XGUI.x_Editor_Data_Has_String(PrefKey_WindowHeight))
            {
                return DefaultWindowSize;
            }
            float w = XGUI.x_Editor_Data_Get_With_Float(PrefKey_WindowWidth);
            float h = XGUI.x_Editor_Data_Get_With_Float(PrefKey_WindowHeight);
            if (w <= 0f || h <= 0f)
                return DefaultWindowSize;
            return new Vector2(
                Mathf.Clamp(w, MinWindowSize.x, MaxWindowSize.x),
                Mathf.Clamp(h, MinWindowSize.y, MaxWindowSize.y));
        }
        /// <summary> 
        ///将当前窗口尺寸写入 EditorPrefs
        /// </summary>
        private void SavePersistedWindowSize()
        {
            if (this == null) return;
            Vector2 size = position.size;
            if (size.x <= 0f || size.y <= 0f) return;
            size.x = Mathf.Clamp(size.x, MinWindowSize.x, MaxWindowSize.x);
            size.y = Mathf.Clamp(size.y, MinWindowSize.y, MaxWindowSize.y);
            XGUI.x_Editor_Data_Set_With_Float(PrefKey_WindowWidth, size.x);
            XGUI.x_Editor_Data_Set_With_Float(PrefKey_WindowHeight, size.y);
        }
        /// <summary> 
        /// 【测试】清空本窗口在 EditorPrefs 中保存的所有 key，
        /// 使下次打开等同于「首次运行」：窗口尺寸 / 名字列宽 / 参数面板宽全部回落到默认值
        /// <para/>
        /// 清空的 key 共四个：
        /// <list type="bullet">
        /// <item><description><see cref="PrefKey_WindowWidth"/>：窗口宽度；</description></item>
        /// <item><description><see cref="PrefKey_WindowHeight"/>：窗口高度；</description></item>
        /// <item><description><see cref="PrefKey_NameWidthWidth"/>：名字列宽度；</description></item>
        /// <item><description><see cref="PrefKey_ParamWidthHeight"/>：参数面板宽度。</description></item>
        /// </list>
        /// <para/>
        /// 注意：本方法<b>不会</b>动 <c>target</c> 上的视图状态
        /// （缩放 <c>Timeline_TrackPosition</c>、滚动 <c>Timeline_TrackScroll</c>、
        /// 轨道高 <c>Timeline_TrackHeight</c>、吸附 <c>Timeline_TrackSnapEnabled</c>），
        /// 因为那些状态存在 <see cref="XHud_Module_Primitive_Tween"/> 组件上，而非 EditorPrefs 中。
        /// <para/>
        /// 调用方式：默认被 <c>[MenuItem]</c> 注释掉，需手动取消注释后从菜单
        /// <c>Tools/XHud/Tween Tracker/Reset Window Prefs (Test)</c> 触发。
        /// </summary>
        //[MenuItem("Tools/XHud/Tween Tracker/Reset Window Prefs (Test)")]
        private static void ResetAllEditorPrefsForTest()
        {
            // ── 清空四个 EditorPrefs key ──
            // 逐个调用 XGUI.x_Editor_Data_Clear，内部会从 EditorPrefs 中移除对应项；
            // 下次 LoadPersistedWindowSize / LoadClampedFloatPreference 时会读不到值，
            // 从而回落到各自方法中的默认值。
            XGUI.x_Editor_Data_Clear(PrefKey_WindowWidth);
            XGUI.x_Editor_Data_Clear(PrefKey_WindowHeight);
            XGUI.x_Editor_Data_Clear(PrefKey_NameWidthWidth);
            XGUI.x_Editor_Data_Clear(PrefKey_ParamWidthHeight);

            // 提示日志：确认操作已执行
            Debug.Log("[XHud] Primitive Tween Tracker EditorPrefs 已清空，窗口已回落默认尺寸");
        }
    }
}
