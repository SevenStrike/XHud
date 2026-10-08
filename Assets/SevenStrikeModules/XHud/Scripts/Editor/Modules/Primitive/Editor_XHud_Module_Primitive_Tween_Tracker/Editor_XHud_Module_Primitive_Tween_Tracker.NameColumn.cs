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
    using SevenStrikeModules.XGUI.Runtime;
    using UnityEditor;
    using UnityEngine;

    public partial class Editor_XHud_Module_Primitive_Tween_Tracker
    {
        /// <summary> 
        /// 绘制左侧名字列（固定面板，垂直滚动由 Clip 区主导）
        /// <para/>
        /// 布局自上而下分为三部分：
        /// <list type="bullet">
        /// <item><description>顶部标题栏（高 <see cref="rulerHeight"/>）：显示「图元动画列表」；</description></item>
        /// <item><description>中部行列表（ScrollView）：每行对应一个动画节点，含类型圆点、
        /// 标识文字、插入 / 删除 / 显隐 / 菜单四个小按钮；</description></item>
        /// <item><description>底部滚动条占位条（高 <see cref="HorizontalScrollbarHeight"/>）：
        /// 与 Clip 区水平滚动条等高，保证两列底边对齐。</description></item>
        /// </list>
        /// <para/>
        /// 关键设计：名字列自身不处理垂直滚动，其 <c>nameScroll.y</c> 每帧从
        /// <see cref="scrollPos"/>.y 同步，从而实现与右侧轨道的垂直对齐。
        /// <para/>
        /// 增删操作采用「延迟执行」模式：循环中只记录 <c>pendingInsertIndex</c> /
        /// <c>pendingDeleteIndex</c>，循环结束后再统一调用增删方法，
        /// 避免在遍历 <see cref="Nodes"/> 过程中修改集合引发异常。
        /// </summary>
        /// <param name="area">名字列在窗口坐标系中的矩形区域</param>
        private void DrawNameColumnPanel(Rect area)
        {
            GUI.BeginGroup(area);
            XGUI.gui_box(new Rect(0, 0, area.width, area.height), ColorBasedBg);

            // ── 顶部标题栏占位（先铺底，文字最后叠加，避免被行列表覆盖）──
            Rect rect_name = new Rect(0, 0, area.width, rulerHeight);
            XGUI.gui_box(rect_name, ColorBasedBg);

            // ── 中部行列表视口：扣除顶部标题栏与底部滚动条占位条 ──
            Rect scrollViewportRect = new Rect(
                0,
                rulerHeight,
                area.width,
                area.height - rulerHeight - HorizontalScrollbarHeight);

            // 内容高度 = 行数 × 行高 + 20px 底部留白（与 Clip 区保持一致，保证滚动范围对齐）
            float contentWidth = scrollViewportRect.width;
            float contentHeight = GetTotalContentHeight();

            // 关键：nameScroll.y 每帧从 scrollPos.y 同步，实现与右侧轨道垂直对齐。
            // 两个方向滚动条均隐藏（false, false），因为名字列不接受独立滚动输入。
            nameScroll = new Vector2(0f, scrollPos.y);
            nameScroll = GUI.BeginScrollView(
                scrollViewportRect,
                nameScroll,
                new Rect(0, 0, contentWidth, contentHeight),
                false,
                false,
                GUIStyle.none,
                GUIStyle.none);

            bool nameHitThisFrame = false;
            int pendingInsertIndex = -1;
            int pendingDeleteIndex = -1;
            int pendingSoundInsertIndex = -1;
            int pendingSoundDeleteIndex = -1;

            for (int row = 0; row < TotalRowCount; row++)
            {
                var (kind, idx) = ResolveRow(row);

                Rect rowRect = GetRowHitRect(row, scrollViewportRect.width);
                Rect nameRect = CalculateNameRowVisualRect(rowRect);

                if (kind == TrackKind.Node)
                {
                    DrawNodeNameRow(rowRect, nameRect, idx,
                        ref nameHitThisFrame,
                        ref pendingInsertIndex,
                        ref pendingDeleteIndex);
                }
                else if (kind == TrackKind.Sound)
                {
                    DrawSoundNameRow(rowRect, nameRect, idx,
                        ref nameHitThisFrame,
                        ref pendingSoundInsertIndex,
                        ref pendingSoundDeleteIndex);
                }
                else if (kind == TrackKind.Gap)
                {
                    DrawNameColumnKindGapRow(rowRect);
                }
            }
            GUI.EndScrollView();

            // 同步回写：防止 GUI.BeginScrollView 修改 nameScroll.y 后污染下一帧
            nameScroll.y = scrollPos.y;

            // ── 底部滚动条占位条：与 Clip 区水平滚动条等高，保证两列底边对齐 ──
            Rect bottomStrip = new Rect(
                0,
                area.height - HorizontalScrollbarHeight,
                area.width,
                HorizontalScrollbarHeight);
            EditorGUI.DrawRect(bottomStrip, ColorBasedBg);

            // ── 名字列空白处点击：清空选中 ──
            // 条件：左键、非 Alt、非行内控件命中、且点击落在中部行列表视口内。
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && !Event.current.alt && !nameHitThisFrame && scrollViewportRect.Contains(Event.current.mousePosition))
            {
                selectedNodeIndices.Clear();
                selectedSoundIndices.Clear();
                selectedIndex = -1;
                selectedKind = TrackKind.无;
                GUIUtility.keyboardControl = 0;
                Event.current.Use();
                Repaint();
            }

            // ── 顶部标题栏：最后叠加绘制，避免被行列表覆盖 ──
            XGUI.gui_box(rect_name, Color_Name_Header_BG);
            XGUI.gui_label(
                rect: new Rect(rect_name.x + 8, rect_name.y, rect_name.width, rect_name.height),
                text: new GUIContent("图元动画列表"),
                text_color: Color.white,
                size: XGUIFontSize.M,
                clipping: TextClipping.Clip,
                anchor: TextAnchor.MiddleLeft,
                font_style: FontStyle.Normal);
            GUI.EndGroup();

            // 名字列右边界分隔线（视觉上区分名字列与 Clip 区）
            //XGUI.gui_box(new Rect(area.x + (area.width - 1), area.y, 1, area.height), Color.black * 0.35f);

            // ── 名字列右边界分隔线：分三段绘制，Gap 区间跳过 ──
            // 说明：
            //   名字列视口起点 = area.y + rulerHeight
            //   Gap 行顶部内容坐标 = GetRowTopY(Nodes.Count)
            //   转成窗口坐标 = area.y + rulerHeight + (内容Y - scrollPos.y)
            //   Gap 行视觉矩形上下各外扩 RowVerticalPadding（与 DrawNameColumnKindGapRow 一致），
            //   因此断口区间 = [GapWinTop - RowVerticalPadding, GapWinBottom + RowVerticalPadding]
            //   断口外的两段黑线各自绘制。
            //XGUI.gui_box(GetNameColumnRightBorderRect(area), Color.black * 0.35f);
            DrawNameColumnRightBorder(area);

            if (pendingDeleteIndex >= 0)
            {
                DeleteTweenNodeAt(pendingDeleteIndex);
            }
            else if (pendingInsertIndex >= 0)
            {
                InsertTweenNodeAfter(pendingInsertIndex);
            }
            else if (pendingSoundDeleteIndex >= 0)
            {
                DeleteSoundAt(pendingSoundDeleteIndex);
            }
            else if (pendingSoundInsertIndex >= 0)
            {
                InsertSoundAfter(pendingSoundInsertIndex);
            }
        }
        /// <summary>
        /// 绘制单条动画节点名字行。
        /// <para/>
        /// 这是从原 <see cref="DrawNameColumnPanel"/> 循环体中抽出的逻辑，
        /// 内容与原来完全一致，只是把 <c>i</c> 换成了参数 <paramref name="index"/>，
        /// 把 <c>SetNodeSelection</c> 换成了 <c>SetSelection(TrackKind.Node, ...)</c>，
        /// 把 <c>selectedIndices.Contains(i)</c> 换成了 <c>IsSelected(TrackKind.Node, index)</c>。
        /// </summary>
        private void DrawNodeNameRow(Rect rowRect, Rect nameRect, int index, ref bool nameHitThisFrame, ref int pendingInsertIndex, ref int pendingDeleteIndex)
        {
            TweenNode node = Nodes[index];
            bool isSelected = IsSelected(TrackKind.Node, index);

            // 行背景
            EditorGUI.DrawRect(nameRect, isSelected ? ColorNameRowSelectedBg : ColorNameRowBg);

            // 四个小按钮自右向左排列
            float btnY = nameRect.y + (nameRect.height - NameRowButtonSize) * 0.5f - 1;
            float dis = 3;

            Rect menuRect = new Rect(nameRect.xMax - NameRowButtonSize - 5, btnY, NameRowButtonSize, NameRowButtonSize);
            Rect eyeRect = new Rect(menuRect.x - NameRowButtonSize - dis, btnY, NameRowButtonSize, NameRowButtonSize);
            Rect deleteRect = new Rect(eyeRect.x - NameRowButtonSize - dis, btnY, NameRowButtonSize, NameRowButtonSize);
            Rect insertRect = new Rect(deleteRect.x - NameRowButtonSize - dis, btnY, NameRowButtonSize, NameRowButtonSize);

            Rect sepRect = new Rect(insertRect.x - dis - 2, nameRect.y, 1, nameRect.height);

            // 标识文字
            float buttonZoneLeft = insertRect.x - 35;
            XGUI.gui_label(
                rect: new Rect(nameRect.x + 25, nameRect.y, buttonZoneLeft - nameRect.x, nameRect.height),
                text: new GUIContent(node.Indicator),
                text_color: Color.white * 0.9f,
                size: XGUIFontSize.B,
                clipping: XGUI.TryEllipsisClipping(),
                anchor: TextAnchor.MiddleLeft,
                font_style: FontStyle.Normal);

            XGUI.gui_box(sepRect, Color.white * 0.35f);

            // ── 按钮：插入 ──
            if (XGUI.gui_button(
                rect: insertRect, tooltip: "",
                tex_release: icon_add_r, tex_press: icon_add_p,
                tex_gui_color: Color.white,
                width: NameRowButtonSize, height: NameRowButtonSize,
                border: new RectOffset(0, 0, 0, 0),
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0)))
            {
                pendingInsertIndex = index;
                nameHitThisFrame = true;
            }

            // ── 按钮：删除 ──
            if (XGUI.gui_button(
                rect: deleteRect, tooltip: "",
                tex_release: icon_del_r, tex_press: icon_del_p,
                tex_gui_color: new Color(0.8f, 0.32f, 0.32f, 1),
                width: NameRowButtonSize, height: NameRowButtonSize,
                border: new RectOffset(0, 0, 0, 0),
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0)))
            {
                pendingDeleteIndex = index;
                nameHitThisFrame = true;
            }

            // ── 按钮：显隐 ──
            if (XGUI.gui_button(
                rect: eyeRect, tooltip: "",
                tex_release: node.Enabled ? icon_enabled_r : icon_disabled_r,
                tex_press: node.Enabled ? icon_enabled_p : icon_disabled_p,
                tex_gui_color: node.Enabled ? Color.white : Color.gray * 0.85f,
                width: NameRowButtonSize, height: NameRowButtonSize,
                border: new RectOffset(0, 0, 0, 0),
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0)))
            {
                node.Enabled = !node.Enabled;
                nameHitThisFrame = true;
            }

            // ── 按钮：菜单 ──
            if (XGUI.gui_button(
                rect: menuRect, tooltip: "",
                tex_release: icon_menu_r, tex_press: icon_menu_p,
                tex_gui_color: Color.white,
                width: NameRowButtonSize, height: NameRowButtonSize,
                border: new RectOffset(0, 0, 0, 0),
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0)))
            {
                nameHitThisFrame = true;
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("A"), false, () => { });
                menu.AddItem(new GUIContent("B"), false, () => { });
                menu.AddItem(new GUIContent("C"), false, () => { });
                menu.ShowAsContext();
            }

            // ── 类型圆点 ──
            XGUI.gui_icon(
                rect: new Rect(nameRect.x + 10, nameRect.y + ((nameRect.height / 2) - TweenTypeDotSize / 2), TweenTypeDotSize, TweenTypeDotSize),
                icon: icon_led,
                color: GetTweenTypeColor(node.Type));

            // ── 行本体点击 ──
            if (Event.current.type == EventType.MouseDown
                && Event.current.button == 0
                && rowRect.Contains(Event.current.mousePosition)
                && !nameHitThisFrame)
            {
                bool ctrl = Event.current.control || Event.current.command;
                bool shift = Event.current.shift;
                SetSelection(TrackKind.Node, index, ctrl, shift);
                nameHitThisFrame = true;
                Event.current.Use();
                Repaint();
            }
        }
        /// <summary>
        /// 绘制单条音效名字行。
        /// <para/>
        /// 与节点行的差异：
        /// <list type="bullet">
        /// <item><description>无"显隐"按钮（<see cref="TweenSound"/> 没有 Enabled）；</description></item>
        /// <item><description>标识文字用音频名或"(空音效)"；</description></item>
        /// <item><description>左侧圆点固定青绿色。</description></item>
        /// </list>
        /// </summary>
        private void DrawSoundNameRow(Rect rowRect, Rect nameRect, int index, ref bool nameHitThisFrame, ref int pendingInsertIndex, ref int pendingDeleteIndex)
        {
            TweenSound sound = Sounds[index];
            bool isSelected = IsSelected(TrackKind.Sound, index);

            EditorGUI.DrawRect(nameRect, isSelected ? ColorNameRowSelectedBg : ColorNameRowBg);

            float btnY = nameRect.y + (nameRect.height - NameRowButtonSize) * 0.5f - 1;
            float dis = 3;

            // 音效只有三个按钮：菜单 / 删除 / 插入
            Rect menuRect = new Rect(nameRect.xMax - NameRowButtonSize - 5, btnY, NameRowButtonSize, NameRowButtonSize);
            Rect muteRect = new Rect(menuRect.x - NameRowButtonSize - dis, btnY, NameRowButtonSize, NameRowButtonSize);
            Rect deleteRect = new Rect(muteRect.x - NameRowButtonSize - dis, btnY, NameRowButtonSize, NameRowButtonSize);
            Rect insertRect = new Rect(deleteRect.x - NameRowButtonSize - dis, btnY, NameRowButtonSize, NameRowButtonSize);

            Rect sepRect = new Rect(insertRect.x - dis - 2, nameRect.y, 1, nameRect.height);

            float buttonZoneLeft = insertRect.x - 35;
            string label = sound.Sound != null ? sound.Sound.name : "(空音效)";
            XGUI.gui_label(
                rect: new Rect(nameRect.x + 25, nameRect.y, buttonZoneLeft - nameRect.x, nameRect.height),
                text: new GUIContent(label),
                text_color: Color.white * 0.9f,
                size: XGUIFontSize.B,
                clipping: XGUI.TryEllipsisClipping(),
                anchor: TextAnchor.MiddleLeft,
                font_style: FontStyle.Normal);

            XGUI.gui_box(sepRect, Color.white * 0.35f);

            // ── 按钮：插入 ──
            if (XGUI.gui_button(
                rect: insertRect, tooltip: "",
                tex_release: icon_add_r, tex_press: icon_add_p,
                tex_gui_color: Color.white,
                width: NameRowButtonSize, height: NameRowButtonSize,
                border: new RectOffset(0, 0, 0, 0),
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0)))
            {
                pendingInsertIndex = index;
                nameHitThisFrame = true;
            }

            // ── 按钮：删除 ──
            if (XGUI.gui_button(
                rect: deleteRect, tooltip: "",
                tex_release: icon_del_r, tex_press: icon_del_p,
                tex_gui_color: new Color(0.8f, 0.32f, 0.32f, 1),
                width: NameRowButtonSize, height: NameRowButtonSize,
                border: new RectOffset(0, 0, 0, 0),
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0)))
            {
                pendingDeleteIndex = index;
                nameHitThisFrame = true;
            }

            // ── 按钮：静音 ──
            if (XGUI.gui_button(
                rect: muteRect, tooltip: "",
                tex_release: sound.Mute ? icon_mute_r : icon_unmute_r,
                tex_press: sound.Mute ? icon_mute_p : icon_unmute_p,
                tex_gui_color: sound.Mute ? Color.gray * 0.85f : Color.white,
                width: NameRowButtonSize, height: NameRowButtonSize,
                border: new RectOffset(0, 0, 0, 0),
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0)))
            {
                sound.Mute = !sound.Mute;
                nameHitThisFrame = true;
            }

            // ── 按钮：菜单 ──
            if (XGUI.gui_button(
                rect: menuRect, tooltip: "",
                tex_release: icon_menu_r, tex_press: icon_menu_p,
                tex_gui_color: Color.white,
                width: NameRowButtonSize, height: NameRowButtonSize,
                border: new RectOffset(0, 0, 0, 0),
                margin: new RectOffset(0, 0, 0, 0),
                padding: new RectOffset(0, 0, 0, 0)))
            {
                nameHitThisFrame = true;
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("选择音频资源"), false, () => { });
                menu.ShowAsContext();
            }

            // ── 类型圆点（音效固定青绿色）──
            XGUI.gui_icon(
                rect: new Rect(nameRect.x + 10, nameRect.y + ((nameRect.height / 2) - TweenTypeDotSize / 2), TweenTypeDotSize, TweenTypeDotSize),
                icon: icon_led,
                color: new Color(0.4f, 0.8f, 0.8f));

            // ── 行本体点击 ──
            if (Event.current.type == EventType.MouseDown
                && Event.current.button == 0
                && rowRect.Contains(Event.current.mousePosition)
                && !nameHitThisFrame)
            {
                bool ctrl = Event.current.control || Event.current.command;
                bool shift = Event.current.shift;
                SetSelection(TrackKind.Sound, index, ctrl, shift);
                nameHitThisFrame = true;
                Event.current.Use();
                Repaint();
            }
        }
        /// <summary> 
        /// 由行命中矩形计算名字行的视觉矩形：上下按 <see cref="RowVerticalPadding"/> 内缩
        /// <para/>
        /// 与命中矩形的关系：
        /// <list type="bullet">
        /// <item><description>命中矩形：完整行高，用于鼠标点击判定；</description></item>
        /// <item><description>视觉矩形：上下各内缩 <see cref="RowVerticalPadding"/>，
        /// 用于行背景、按钮、文字等的绘制。</description></item>
        /// </list>
        /// 内缩后若高度小于 4px（行高极小时的边界保护），强制设为 4px，
        /// 避免 <see cref="EditorGUI.DrawRect"/> 绘制出 0 或负高度的无效矩形。
        /// </summary>
        /// <param name="rowRect">行命中矩形</param>
        /// <returns>行视觉矩形</returns>
        private Rect CalculateNameRowVisualRect(Rect rowRect)
        {
            float pad = RowVerticalPadding;
            return new Rect(
                rowRect.x,
                rowRect.y + pad,
                rowRect.width,
                Mathf.Max(rowRect.height - pad * 2f, 4f));
        }
        /// <summary>
        /// 计算名字列右边界分隔线在窗口坐标系中需要绘制的区域。
        /// <para/>
        /// 无 Gap 行时返回整条（从 area.y 到 area.yMax）。
        /// 有 Gap 行时返回两段合并区域：
        /// <list type="bullet">
        /// <item><description>上段：[area.y, GapWinTop - RowVerticalPadding]</description></item>
        /// <item><description>下段：[GapWinBottom + RowVerticalPadding, area.yMax]</description></item>
        /// </list>
        /// 两段之间（即 Gap 行的视觉区间）不绘制，让间距行在名字列与 Clip 区之间横向贯通。
        /// <para/>
        /// 注意：返回的 Rect 是**两段中较长的那段**，如果两段都有效会分别绘制。
        /// 为了简化调用方，这里改为返回一个可枚举的两段 Rect；由于 C# 在 Unity 编辑器
        /// 中不便直接返回元组集合，本方法改为直接绘制。
        /// </summary>
        private void DrawNameColumnRightBorder(Rect area)
        {
            float lineX = area.x + (area.width - 1);
            const float lineW = 1f;
            Color lineColor = Color.black * 0.35f;

            // ── 无 Gap 行：整条绘制，保持原样 ──
            if (!HasTrackKindGap)
            {
                XGUI.gui_box(new Rect(lineX, area.y, lineW, area.height), lineColor);
                return;
            }

            // ── 有 Gap 行：计算 Gap 行的窗口坐标区间 ──
            // 名字列视口起点
            float viewportTop = area.y + rulerHeight;

            // Gap 行顶部内容坐标（未外扩）
            float gapContentTop = GetRowTopY(Nodes.Count);

            // Gap 行外扩后的视觉区间（上下各 RowVerticalPadding，与 DrawNameColumnKindGapRow 一致）
            float gapVisualTop = gapContentTop - RowVerticalPadding;
            float gapVisualBottom = gapContentTop + trackKindGapHeight + RowVerticalPadding;

            // 转成窗口坐标
            float gapWinTop = viewportTop + (gapVisualTop - scrollPos.y);
            float gapWinBottom = viewportTop + (gapVisualBottom - scrollPos.y);

            // 钳制到 area 范围内
            gapWinTop = Mathf.Clamp(gapWinTop, area.y, area.yMax);
            gapWinBottom = Mathf.Clamp(gapWinBottom, area.y, area.yMax);

            // ── 上段 ──
            if (gapWinTop > area.y)
            {
                XGUI.gui_box(
                    new Rect(lineX, area.y, lineW, gapWinTop - area.y),
                    lineColor);
            }

            // ── 下段 ──
            if (gapWinBottom < area.yMax)
            {
                XGUI.gui_box(
                    new Rect(lineX, gapWinBottom, lineW, area.yMax - gapWinBottom),
                    lineColor);
            }
        }
        /// <summary>
        /// 绘制动画行与音效行之间的间距行（名字列一侧）。
        /// <para/>
        /// 与 Clip 区的 <see cref="DrawTrackKindGapRow"/> 高度一致、底色一致，
        /// 保证左右两栏在视觉上横向贯通，形成一条完整的"分隔带"。
        /// <para/>
        /// 本方法是**独立的自定义绘制入口**，与 Clip 区一侧各自负责自己那半边的绘制。
        /// </summary>
        /// <param name="gapRect">间距行的完整矩形（内容坐标）</param>
        private void DrawNameColumnKindGapRow(Rect gapRect)
        {
            //  关键：Gap 行上下各扩展 1px，吃掉相邻行的内缩空间，
            // 让分隔带在视觉上紧贴上下相邻行。
            gapRect = new Rect(
                gapRect.x,
                gapRect.y - RowVerticalPadding,
                gapRect.width,
                gapRect.height + RowVerticalPadding * 2f);

            // 底色
            XGUI.gui_box(gapRect, ColorTrackKindGapBg);

            // 上下描边
            //XGUI.gui_box(new Rect(gapRect.x, gapRect.y, gapRect.width, 1f), ColorTrackKindGapEdge);
            //XGUI.gui_box(new Rect(gapRect.x, gapRect.yMax - 1f, gapRect.width, 1f), ColorTrackKindGapEdge);

            // ── 自定义绘制区 ──
        }
    }
}
