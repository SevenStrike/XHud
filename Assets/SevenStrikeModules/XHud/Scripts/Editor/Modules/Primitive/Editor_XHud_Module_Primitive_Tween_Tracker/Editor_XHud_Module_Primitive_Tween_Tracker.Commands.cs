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
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XTween;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    public partial class Editor_XHud_Module_Primitive_Tween_Tracker
    {
        /// <summary> 
        /// 在指定索引位置插入一条新的空白动画节点
        /// <para/>
        /// 本方法是节点插入的唯一底层入口：
        /// <list type="bullet">
        /// <item><description>窗口空状态下的「新增首个动画」按钮 → <c>InsertTweenNodeAt(0)</c>；</description></item>
        /// <item><description>名字行「插入」按钮 → <see cref="InsertTweenNodeAfter"/> → <c>InsertTweenNodeAt(index + 1)</c>。</description></item>
        /// </list>
        /// <para/>
        /// 执行流程：
        /// <list type="number">
        /// <item><description>注册 Undo（<see cref="Undo.RegisterCompleteObjectUndo"/>），保证 Ctrl+Z 可撤销；</description></item>
        /// <item><description>构造新节点并填入一套「安全默认值」（详见下方字段初始化）；</description></item>
        /// <item><description>钳制插入索引后插入集合；</description></item>
        /// <item><description>清空并重设选中集合，把新节点设为唯一选中项。</description></item>
        /// </list>
        /// <para/>
        /// 注意：新节点的 <c>Type</c> 使用 <see cref="TweenNode"/> 的默认值（枚举 0），
        /// 因此「新增首个动画」后需要用户在参数面板手动选择类型。
        /// </summary>
        /// <param name="insertAt">插入位置；0 表示插到最前，Nodes.Count 表示追加到末尾</param>
        private void InsertTweenNodeAt(int insertAt)
        {
            // 前置校验：目标为空时直接返回，避免 NRE
            if (target == null || target.Equals(null)) return;

            // 注册完整对象 Undo，保证本次插入可通过 Ctrl+Z 撤销
            if (!IsPreviewing)
                Undo.RegisterCompleteObjectUndo(target, "Insert Tween Node");

            // ── 构造新节点并填入安全默认值 ──
            // 所有字段均显式赋值，避免依赖字段初始化器的默认值（尤其是 Color 等引用类型）。
            TweenNode newNode = new TweenNode();

            // ID：由目标统一分配，保证全局唯一
            newNode.ID = target.TweenNode_GenerateId();

            // 显示名：默认 "NewTween"，用户可在参数面板中修改
            newNode.Indicator = "NewTween";

            // 启用状态：默认启用，插入后即可在预览中看到效果
            newNode.Enabled = true;

            // 触发时机：默认「无」，需用户手动选择
            newNode.Timings = "元素进入时";

            // 时长 / 延迟：默认 1 秒、无延迟，是一个「开箱即用」的起始值
            newNode.Duration = 1f;
            newNode.Delay = 0f;

            // 循环：默认 Restart 模式、循环 0 次（即不循环）
            newNode.LoopType = XTween_LoopType.Restart;
            newNode.LoopCount = 0;

            // 运行时进度：默认 0，由 XTween 运行时驱动
            newNode.Progress = 0f;

            // 参数面板折叠状态：默认展开
            newNode.IsFold = false;

            // 重置 / 完成时是否写回起始 / 结束值：默认都开启
            newNode.Rewind_Set_Startvalue = true;
            newNode.Complete_Set_Endvalue = true;

            // 颜色三值：默认白色（对颜色动画是安全起始值，对其他类型无影响）
            newNode.Original_Color = Color.white;
            newNode.From_Color = Color.white;
            newNode.End_Color = Color.white;

            newNode.ValueModeIndex = 0;
            newNode.TweenValueMode = TweenValueMode.起始到默认_S_D;

            // 缓动曲线：默认 EaseInOut(0,0,1,1)，即标准缓入缓出
            newNode.Curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

            // ── 钳制插入索引后插入集合 ──
            // Clamp 到 [0, Nodes.Count]，防止调用方传入越界索引
            insertAt = Mathf.Clamp(insertAt, 0, Nodes.Count);
            Nodes.Insert(insertAt, newNode);
            EditorUtility.SetDirty(target);

            // ── 重置选中集合：新节点成为唯一选中项 ──
            // 无论之前选中多少节点，插入后都只选中新节点，
            // 这样参数面板会立即显示新节点的属性，用户可马上编辑。
            // 同时清空音效选中（普通单击语义：重新开始选择）。
            selectedNodeIndices.Clear();
            selectedNodeIndices.Add(insertAt);
            selectedSoundIndices.Clear();
            selectedIndex = insertAt;
            selectedKind = TrackKind.Node;
            Repaint();
        }
        /// <summary> 
        /// 在指定索引的节点下方插入一条新的空白动画节点
        /// <para/>
        /// 使用方式：名字行「插入」按钮点击时调用（<see cref="DrawNameColumnPanel"/> 中
        /// 记录 <c>pendingInsertIndex</c> 后延迟执行）。
        /// <para/>
        /// 本方法是 <see cref="InsertTweenNodeAt"/> 的便捷包装，只做两件事：
        /// <list type="number">
        /// <item><description>做一次边界保护（目标非空、索引有效）；</description></item>
        /// <item><description>把「在 <paramref name="index"/> 之后插入」翻译为
        /// <c>InsertTweenNodeAt(index + 1)</c>。</description></item>
        /// </list>
        /// 真正的节点构造、Undo 注册、选中重置等逻辑全部由
        /// <see cref="InsertTweenNodeAt"/> 承担。
        /// <para/>
        /// 之所以「插入到之后」而非「之前」：名字行的插入按钮位于行内，
        /// 用户点击时的直觉是「在当前行下方新增一条」，符合自上而下的列表阅读顺序。
        /// </summary>
        /// <param name="index">参考节点索引；新节点将插入到该节点之后（即索引 <c>index + 1</c> 处）</param>
        private void InsertTweenNodeAfter(int index)
        {
            // 目标为空时直接返回，避免后续调用 NRE
            if (target == null) return;

            // 索引越界保护：index 必须落在 [0, Nodes.Count - 1] 范围内
            // 越界时直接返回，不做任何操作（调用方本不应传入越界值）
            if (index < 0 || index >= Nodes.Count) return;

            // 委托给底层入口：在 index + 1 处插入
            // 注意此处不需要再 Clamp，因为 InsertTweenNodeAt 内部已做钳制
            InsertTweenNodeAt(index + 1);
        }
        /// <summary> 
        /// 删除指定索引的动画节点，并修正选中集合
        /// <para/>
        /// 本方法是节点删除的底层入口：
        /// <list type="bullet">
        /// <item><description>名字行「删除」按钮 → <c>DeleteTweenNodeAt(i)</c>
        /// （由 <see cref="DrawNameColumnPanel"/> 记录 <c>pendingDeleteIndex</c> 后延迟执行）。</description></item>
        /// </list>
        /// 批量删除请使用 <see cref="DeleteSelectedTweenNodes"/>。
        /// <para/>
        /// 执行流程：
        /// <list type="number">
        /// <item><description>注册 Undo（<see cref="Undo.RegisterCompleteObjectUndo"/>）；</description></item>
        /// <item><description>若节点仍有运行时 Tweener，先 Kill 并置空，避免残留动画继续影响目标；</description></item>
        /// <item><description>从集合移除该节点；</description></item>
        /// <item><description>重建选中集合：删除位置之后的索引全部前移一位；</description></item>
        /// <item><description>修正 <see cref="selectedIndex"/>；</description></item>
        /// <item><description>钳制垂直滚动，避免删除后出现滚动越界。</description></item>
        /// </list>
        /// </summary>
        /// <param name="index">要删除的节点索引</param>
        private void DeleteTweenNodeAt(int index)
        {
            // 前置校验：目标为空、索引越界时直接返回
            if (target == null) return;
            if (index < 0 || index >= Nodes.Count) return;

            // 注册完整对象 Undo，保证本次删除可撤销（预览期间不记录）
            if (!IsPreviewing)
                Undo.RegisterCompleteObjectUndo(target, "Delete Tween Node");

            // ── 运行时清理：先 Kill 掉仍在运行的 Tweener ──
            TweenNode node = Nodes[index];
            if (node.Tweener != null)
            {
                node.Tweener.Kill();
                node.Tweener = null;
            }

            // 从集合移除
            Nodes.RemoveAt(index);
            EditorUtility.SetDirty(target);

            // ── 重建动画选中集合：删除位置之后的索引全部前移一位 ──
            // 规则：
            //   i <  index → 保留原值
            //   i >  index → 减 1（因为前面的元素被删，索引整体前移）
            //   i == index → 丢弃（该节点已不存在）
            HashSet<int> newSelection = new HashSet<int>();
            foreach (int i in selectedNodeIndices)
            {
                if (i < index) newSelection.Add(i);
                else if (i > index) newSelection.Add(i - 1);
            }
            selectedNodeIndices.Clear();
            foreach (int i in newSelection) selectedNodeIndices.Add(i);

            // ── 修正主选中索引（仅当主选中是动画时）──
            if (selectedKind == TrackKind.Node)
            {
                if (selectedIndex == index)
                {
                    // 被删的正是主选中项：改为动画集合中最上面那个；若动画空了则切到音效
                    selectedIndex = selectedNodeIndices.Count > 0
                        ? GetTopmostSelectedIndex(selectedNodeIndices) : -1;
                }
                else if (selectedIndex > index)
                {
                    // 主选中项位于被删项之后：索引前移一位
                    selectedIndex--;
                }

                // 动画已无主选中 → 尝试切到音效
                if (selectedIndex < 0 && selectedNodeIndices.Count == 0)
                {
                    if (selectedSoundIndices.Count > 0)
                    {
                        selectedKind = TrackKind.Sound;
                        selectedIndex = GetTopmostSelectedIndex(selectedSoundIndices);
                    }
                    else
                    {
                        selectedKind = TrackKind.无;
                    }
                }
            }
            // 其余情况（selectedKind == Sound / 无）保持不变

            // ── 钳制垂直滚动 ──
            // 删除后内容高度变小，原 scrollPos.y 可能超出新的滚动上限，
            // 需重新钳制并同步 nameScroll.y，保持名字列与轨道对齐。
            scrollPos.y = Mathf.Clamp(scrollPos.y, 0f, CalculateMaxVerticalScroll());
            nameScroll.y = scrollPos.y;
            Repaint();
        }
        /// <summary> 
        /// 删除所有当前选中的动画节点（批量删除）
        /// <para/>
        /// 使用方式：在 Clip 区 / 名字列选中若干节点后，按下 Delete 或 Backspace 键触发
        /// （见 <see cref="HandleKeyboardShortcuts"/>）。
        /// <para/>
        /// 与 <see cref="DeleteTweenNodeAt"/> 的关键区别：
        /// <list type="bullet">
        /// <item><description>本方法<b>倒序</b>删除，避免正序删除时索引错位；</description></item>
        /// <item><description>本方法删除后<b>只清空动画选中</b>，音效选中保留。</description></item>
        /// </list>
        /// <para/>
        /// 为什么倒序删除：删除索引 i 会使其后所有元素前移一位，
        /// 若从 0 开始正序删除，则后续原本记录的下标全部失效；
        /// 从大到小删除则不会影响尚未处理的下标。
        /// </summary>
        private void DeleteSelectedTweenNodes()
        {
            // 前置校验：目标为空、无选中项时直接返回
            if (target == null) return;
            if (selectedNodeIndices.Count == 0) return;

            // 注册完整对象 Undo，保证批量删除可一次撤销（预览期间不记录）
            if (!IsPreviewing)
                Undo.RegisterCompleteObjectUndo(target, "Delete Tween Nodes");

            // ── 拷贝选中集合到列表，并降序排序 ──
            // 降序是为了「从后往前删」，使未处理的下标始终有效。
            List<int> indices = new List<int>(selectedNodeIndices);
            indices.Sort((a, b) => b.CompareTo(a));

            // ── 按降序逐个删除 ──
            foreach (int i in indices)
            {
                // 越界保护：理论上不会触发，此处仅作防御
                if (i < 0 || i >= Nodes.Count) continue;

                // 与 DeleteTweenNodeAt 一致：先 Kill 运行时 Tweener，
                // 否则节点虽删但动画仍在跑。
                TweenNode node = Nodes[i];
                if (node.Tweener != null)
                {
                    node.Tweener.Kill();
                    node.Tweener = null;
                }

                Nodes.RemoveAt(i);
            }

            EditorUtility.SetDirty(target);

            // ── 清空动画选中 ──
            selectedNodeIndices.Clear();

            // ── 修正主选中：若主选中是动画且已空，切到音效（如果还有）──
            if (selectedKind == TrackKind.Node)
            {
                if (selectedSoundIndices.Count > 0)
                {
                    selectedKind = TrackKind.Sound;
                    selectedIndex = GetTopmostSelectedIndex(selectedSoundIndices);
                }
                else
                {
                    selectedIndex = -1;
                    selectedKind = TrackKind.无;
                }
            }

            // ── 钳制垂直滚动 ──
            scrollPos.y = Mathf.Clamp(scrollPos.y, 0f, CalculateMaxVerticalScroll());
            nameScroll.y = scrollPos.y;
            Repaint();
        }
        /// <summary>
        /// 在指定索引处插入一条新的空白音效。
        /// </summary>
        private void InsertSoundAt(int insertAt)
        {
            if (target == null || target.Equals(null)) return;

            if (!IsPreviewing)
                Undo.RegisterCompleteObjectUndo(target, "Insert Tween Sound");

            TweenSound s = new TweenSound();
            s.ID = target.TweenSound_GenerateId();
            s.Sound = null;
            s.Path = "";
            s.Delay = 0f;
            s.Volume = 1f;
            s.MinPitch = 1f;
            s.MaxPitch = 1f;

            insertAt = Mathf.Clamp(insertAt, 0, Sounds.Count);
            Sounds.Insert(insertAt, s);
            EditorUtility.SetDirty(target);

            // 新音效成为唯一选中项；同时清空动画选中（普通单击语义）
            selectedSoundIndices.Clear();
            selectedSoundIndices.Add(insertAt);
            selectedNodeIndices.Clear();
            selectedIndex = insertAt;
            selectedKind = TrackKind.Sound;
            Repaint();
        }
        /// <summary>
        /// 在指定音效下方插入一条新的空白音效。
        /// </summary>
        private void InsertSoundAfter(int index)
        {
            if (target == null) return;
            if (index < 0 || index >= Sounds.Count) return;
            InsertSoundAt(index + 1);
        }
        /// <summary>
        /// 删除指定索引的音效，并修正选中集合。
        /// </summary>
        private void DeleteSoundAt(int index)
        {
            if (target == null) return;
            if (index < 0 || index >= Sounds.Count) return;

            if (!IsPreviewing)
                Undo.RegisterCompleteObjectUndo(target, "Delete Tween Sound");

            Sounds.RemoveAt(index);
            EditorUtility.SetDirty(target);

            // ── 重建音效选中集合：删除位置之后的索引全部前移一位 ──
            HashSet<int> newSelection = new HashSet<int>();
            foreach (int i in selectedSoundIndices)
            {
                if (i < index) newSelection.Add(i);
                else if (i > index) newSelection.Add(i - 1);
            }
            selectedSoundIndices.Clear();
            foreach (int i in newSelection) selectedSoundIndices.Add(i);

            // ── 修正主选中索引（仅当主选中是音效时）──
            if (selectedKind == TrackKind.Sound)
            {
                if (selectedIndex == index)
                    selectedIndex = selectedSoundIndices.Count > 0
                        ? GetTopmostSelectedIndex(selectedSoundIndices) : -1;
                else if (selectedIndex > index)
                    selectedIndex--;

                // 音效已无主选中 → 尝试切到动画
                if (selectedIndex < 0 && selectedSoundIndices.Count == 0)
                {
                    if (selectedNodeIndices.Count > 0)
                    {
                        selectedKind = TrackKind.Node;
                        selectedIndex = GetTopmostSelectedIndex(selectedNodeIndices);
                    }
                    else
                    {
                        selectedKind = TrackKind.无;
                    }
                }
            }

            scrollPos.y = Mathf.Clamp(scrollPos.y, 0f, CalculateMaxVerticalScroll());
            nameScroll.y = scrollPos.y;
            Repaint();
        }
        /// <summary>
        /// 删除所有当前选中的音效。
        /// </summary>
        private void DeleteSelectedSounds()
        {
            if (target == null) return;
            if (selectedSoundIndices.Count == 0) return;

            if (!IsPreviewing)
                Undo.RegisterCompleteObjectUndo(target, "Delete Tween Sounds");

            List<int> indices = new List<int>(selectedSoundIndices);
            indices.Sort((a, b) => b.CompareTo(a));
            foreach (int i in indices)
            {
                if (i < 0 || i >= Sounds.Count) continue;
                Sounds.RemoveAt(i);
            }
            EditorUtility.SetDirty(target);

            // ── 清空音效选中 ──
            selectedSoundIndices.Clear();

            // ── 修正主选中：若主选中是音效且已空，切到动画（如果还有）──
            if (selectedKind == TrackKind.Sound)
            {
                if (selectedNodeIndices.Count > 0)
                {
                    selectedKind = TrackKind.Node;
                    selectedIndex = GetTopmostSelectedIndex(selectedNodeIndices);
                }
                else
                {
                    selectedIndex = -1;
                    selectedKind = TrackKind.无;
                }
            }

            // ── 钳制垂直滚动 ──
            scrollPos.y = Mathf.Clamp(scrollPos.y, 0f, CalculateMaxVerticalScroll());
            nameScroll.y = scrollPos.y;
            Repaint();
        }
        /// <summary>
        /// 克隆当前选中的所有动画 / 音效 Clip。
        /// <para/>
        /// 与"复制粘贴"的区别：
        /// <list type="bullet">
        /// <item><description>复制粘贴：复制到鼠标落点，可跨编辑器实例；</description></item>
        /// <item><description>克隆：每个 Clip 独立克隆一份，副本**紧跟在原 Clip 下方**（同类型列表内），
        /// Delay / Duration 保持不变，用户可立即对副本继续操作。</description></item>
        /// </list>
        /// <para/>
        /// 支持三种情况：
        /// <list type="bullet">
        /// <item><description>只选中动画：只克隆动画；</description></item>
        /// <item><description>只选中音效：只克隆音效；</description></item>
        /// <item><description>跨类型多选：两类都克隆。</description></item>
        /// </list>
        /// <para/>
        /// 克隆后，**所有新副本成为新的选中集合**，方便用户立即对副本继续操作
        /// （比如整体拖动、改参数、再次克隆）。
        /// </summary>
        private void CloneSelectedClips()
        {
            if (target == null) return;
            if (TotalSelectedCount == 0) return;

            if (!IsPreviewing)
                Undo.RegisterCompleteObjectUndo(target, "Clone Tween Clips");

            // ── 克隆动画 ──
            // 必须**从后往前**处理，否则前面插入会让后面的索引错位。
            List<int> newSelectedNodes = new List<int>();
            if (selectedNodeIndices.Count > 0)
            {
                List<int> sortedDesc = new List<int>(selectedNodeIndices);
                sortedDesc.Sort((a, b) => b.CompareTo(a));   // 降序

                foreach (int i in sortedDesc)
                {
                    if (i < 0 || i >= Nodes.Count) continue;
                    TweenNode src = Nodes[i];

                    // 调用 TweenNode.Clone() 做深拷贝（内部会处理 AnimationCurve 深拷贝）
                    TweenNode copy = src.Clone();

                    // 重新分配 ID，避免与原节点冲突
                    copy.ID = target.TweenNode_GenerateId();

                    // 运行时字段重置
                    copy.Tweener = null;
                    copy.Progress = 0f;
                    copy.IsFold = false;

                    // 副本插到原节点**下方**（即原索引 +1 处）
                    int insertAt = i + 1;
                    Nodes.Insert(insertAt, copy);
                    newSelectedNodes.Add(insertAt);
                }
            }

            // ── 克隆音效 ──
            List<int> newSelectedSounds = new List<int>();
            if (selectedSoundIndices.Count > 0)
            {
                List<int> sortedDesc = new List<int>(selectedSoundIndices);
                sortedDesc.Sort((a, b) => b.CompareTo(a));

                foreach (int i in sortedDesc)
                {
                    if (i < 0 || i >= Sounds.Count) continue;
                    TweenSound src = Sounds[i];

                    // 调用 TweenSound.Clone()
                    TweenSound copy = src.Clone();

                    int insertAt = i + 1;
                    Sounds.Insert(insertAt, copy);
                    newSelectedSounds.Add(insertAt);
                }
            }

            EditorUtility.SetDirty(target);

            // ── 更新选中：克隆出的副本成为新的选中集合 ──
            selectedNodeIndices.Clear();
            foreach (int i in newSelectedNodes) selectedNodeIndices.Add(i);

            selectedSoundIndices.Clear();
            foreach (int i in newSelectedSounds) selectedSoundIndices.Add(i);

            // 主选中：优先动画，其次音效
            if (newSelectedNodes.Count > 0)
            {
                selectedKind = TrackKind.Node;
                selectedIndex = newSelectedNodes[newSelectedNodes.Count - 1];
            }
            else if (newSelectedSounds.Count > 0)
            {
                selectedKind = TrackKind.Sound;
                selectedIndex = newSelectedSounds[newSelectedSounds.Count - 1];
            }
            else
            {
                selectedKind = TrackKind.无;
                selectedIndex = -1;
            }

            Repaint();

            //Debug.Log($"[XHud] 已克隆 {newSelectedNodes.Count} 个动画 + {newSelectedSounds.Count} 个音效");
        }
        /// <summary>
        /// 显示 Clip 的右键菜单。
        /// <para/>
        /// 若右键的 Clip 不在当前选中集合中，会先把它设为唯一选中；
        /// 若已在选中集合中，则保留当前多选，菜单操作作用于整组。
        /// </summary>
        private void ShowClipContextMenu(TrackKind kind, int index)
        {
            // 右键的 Clip 未被选中 → 先选中它
            if (!IsSelected(kind, index))
            {
                SetSelection(kind, index, false, false);
            }

            GenericMenu menu = new GenericMenu();

            // ── 克隆 ──
            menu.AddItem(new GUIContent("D 克隆  Ctrl+D"), false, () =>
            {
                CloneSelectedClips();
            });

            menu.AddSeparator("");

            // ── 删除 ──
            menu.AddItem(new GUIContent("E 删除  Delete"), false, () =>
            {
                if (selectedNodeIndices.Count > 0) DeleteSelectedTweenNodes();
                if (selectedSoundIndices.Count > 0) DeleteSelectedSounds();
            });

            menu.AddSeparator("");

            // ── 对齐到 0 秒 ──
            menu.AddItem(new GUIContent("R 对齐到 0 秒"), false, () =>
            {
                AlignSelectedClipsToTime(0f);
            });

            menu.ShowAsContext();
        }
        /// <summary>
        /// 把当前选中的所有 Clip 整组平移到以 <paramref name="targetSecond"/> 为最左端。
        /// </summary>
        private void AlignSelectedClipsToTime(float targetSecond)
        {
            if (target == null) return;
            if (TotalSelectedCount == 0) return;

            // 找整组最左 Delay
            float minDelay = float.MaxValue;
            foreach (int i in selectedNodeIndices)
            {
                if (i < 0 || i >= Nodes.Count) continue;
                if (Nodes[i].Delay < minDelay) minDelay = Nodes[i].Delay;
            }
            foreach (int i in selectedSoundIndices)
            {
                if (i < 0 || i >= Sounds.Count) continue;
                if (Sounds[i].Delay < minDelay) minDelay = Sounds[i].Delay;
            }
            if (minDelay == float.MaxValue) return;

            float delta = targetSecond - minDelay;

            if (!IsPreviewing)
                Undo.RegisterCompleteObjectUndo(target, "Align Tween Clips");

            foreach (int i in selectedNodeIndices)
            {
                if (i < 0 || i >= Nodes.Count) continue;
                Nodes[i].Delay = QuantizeTime(Mathf.Max(0f, Nodes[i].Delay + delta));
            }
            foreach (int i in selectedSoundIndices)
            {
                if (i < 0 || i >= Sounds.Count) continue;
                Sounds[i].Delay = QuantizeTime(Mathf.Max(0f, Sounds[i].Delay + delta));
            }

            EditorUtility.SetDirty(target);
            Repaint();
        }
    }
}
