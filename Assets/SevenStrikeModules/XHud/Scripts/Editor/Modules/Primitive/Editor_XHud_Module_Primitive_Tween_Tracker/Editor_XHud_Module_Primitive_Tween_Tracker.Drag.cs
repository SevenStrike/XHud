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
    using UnityEditor;
    using UnityEngine;

    public partial class Editor_XHud_Module_Primitive_Tween_Tracker
    {
        /// <summary> 
        ///在 Clip 矩形上检测 MouseDown 命中，命中则进入对应的拖拽模式
        /// </summary>
        /// <param name="clipRect">Clip 矩形（内容坐标）</param>
        /// <param name="index">动画节点索引</param>
        private void TryBeginClipDrag(Rect clipRect, int index)
        {
            Event e = Event.current;

            // ★ 右键菜单
            if (e.type == EventType.MouseDown && e.button == 1 && clipRect.Contains(e.mousePosition))
            {
                ShowClipContextMenu(TrackKind.Node, index);
                e.Use();
                return;
            }

            if (e.type != EventType.MouseDown) return;
            if (e.button != 0) return;
            if (e.alt) return;
            if (dragMode != DragMode.无) return;
            if (!clipRect.Contains(e.mousePosition)) return;

            bool ctrl = e.control || e.command;
            bool shift = e.shift;

            if (ctrl || shift)
            {
                SetSelection(TrackKind.Node, index, ctrl, shift);
            }
            else if (!IsSelected(TrackKind.Node, index))
            {
                SetSelection(TrackKind.Node, index, false, false);
            }
            else
            {
                // 点在已选中的动画上 → 保持全部选中，只更新主选中
                selectedKind = TrackKind.Node;
                selectedIndex = index;
                GUIUtility.keyboardControl = 0;
            }

            // 判断拖拽模式
            if (Mathf.Abs(e.mousePosition.x - clipRect.xMin) < ClipEdgeZone)
                dragMode = DragMode.左边缘;
            else if (Mathf.Abs(e.mousePosition.x - clipRect.xMax) < ClipEdgeZone)
                dragMode = DragMode.右边缘;
            else
                dragMode = DragMode.移动;

            if (dragMode == DragMode.移动)
            {
                float clipCenterX = (clipRect.xMin + clipRect.xMax) * 0.5f;
                moveDragAnchorSide = e.mousePosition.x < clipCenterX ? 1 : 2;
            }
            else
            {
                moveDragAnchorSide = 0;
            }

            primaryDragIndex = selectedNodeIndices.Contains(index)
                ? GetTopmostSelectedIndex(selectedNodeIndices)
                : index;
            if (primaryDragIndex < 0) primaryDragIndex = index;

            // 记录起始 Delay（两类都记）
            dragStartNodeDelays.Clear();
            dragStartNodeDurations.Clear();
            dragStartSoundDelays.Clear();

            foreach (int i in selectedNodeIndices)
            {
                if (i < 0 || i >= Nodes.Count) continue;
                dragStartNodeDelays[i] = QuantizeTime(Nodes[i].Delay);
                dragStartNodeDurations[i] = QuantizeTime(Nodes[i].Duration);
            }
            foreach (int i in selectedSoundIndices)
            {
                if (i < 0 || i >= Sounds.Count) continue;
                dragStartSoundDelays[i] = QuantizeTime(Sounds[i].Delay);
            }

            // 保证当前拖拽项在字典里
            if (!dragStartNodeDelays.ContainsKey(index))
            {
                dragStartNodeDelays[index] = QuantizeTime(Nodes[index].Delay);
                dragStartNodeDurations[index] = QuantizeTime(Nodes[index].Duration);
                primaryDragIndex = index;
            }

            dragStartDelay = QuantizeTime(Nodes[primaryDragIndex].Delay);
            dragStartDuration = QuantizeTime(Nodes[primaryDragIndex].Duration);

            draggingIndex = index;
            draggingKind = TrackKind.Node;
            dragControlID = GUIUtility.GetControlID(FocusType.Passive);
            GUIUtility.hotControl = dragControlID;
            GUIUtility.keyboardControl = 0;
            dragStartContentSecond = e.mousePosition.x / pixelsPerSecond;
            snapGuideSecond = -1f;

            if (!IsPreviewing)
            {
                Undo.IncrementCurrentGroup();
                Undo.SetCurrentGroupName("Edit Tween Clip");
                dragUndoGroup = Undo.GetCurrentGroup();
                Undo.RecordObject(target, "Edit Tween Clip");
            }
            else
            {
                dragUndoGroup = -1;
            }

            clipHitThisFrame = true;
            e.Use();
            Repaint();
        }
        /// <summary> 
        /// 在音效 Clip 上检测 MouseDown 命中，命中则进入"移动"拖拽模式。
        /// <para/>
        /// 与动画 Clip 不同：音效不做左右边缘拉伸，只做整体移动。
        /// </summary>
        private void TryBeginSoundClipDrag(Rect clipRect, int soundIndex)
        {
            Event e = Event.current;

            // ★ 右键菜单
            if (e.type == EventType.MouseDown && e.button == 1 && clipRect.Contains(e.mousePosition))
            {
                ShowClipContextMenu(TrackKind.Sound, soundIndex);
                e.Use();
                return;
            }

            if (e.type != EventType.MouseDown) return;
            if (e.button != 0) return;
            if (e.alt) return;
            if (dragMode != DragMode.无) return;
            if (!clipRect.Contains(e.mousePosition)) return;

            bool ctrl = e.control || e.command;
            bool shift = e.shift;

            if (ctrl || shift)
            {
                SetSelection(TrackKind.Sound, soundIndex, ctrl, shift);
            }
            else if (!IsSelected(TrackKind.Sound, soundIndex))
            {
                SetSelection(TrackKind.Sound, soundIndex, false, false);
            }
            else
            {
                selectedKind = TrackKind.Sound;
                selectedIndex = soundIndex;
                GUIUtility.keyboardControl = 0;
            }

            dragMode = DragMode.移动;
            draggingKind = TrackKind.Sound;
            draggingIndex = soundIndex;
            primaryDragIndex = soundIndex;

            // ★ 与动画 Clip 一致：按鼠标点在 Clip 左半 / 右半锁定吸附锚定侧
            float clipCenterX = (clipRect.xMin + clipRect.xMax) * 0.5f;
            moveDragAnchorSide = e.mousePosition.x < clipCenterX ? 1 : 2;

            // 记录起始 Delay（两类都记）
            dragStartNodeDelays.Clear();
            dragStartNodeDurations.Clear();
            dragStartSoundDelays.Clear();

            foreach (int i in selectedNodeIndices)
            {
                if (i < 0 || i >= Nodes.Count) continue;
                dragStartNodeDelays[i] = QuantizeTime(Nodes[i].Delay);
                dragStartNodeDurations[i] = QuantizeTime(Nodes[i].Duration);
            }
            foreach (int i in selectedSoundIndices)
            {
                if (i < 0 || i >= Sounds.Count) continue;
                dragStartSoundDelays[i] = QuantizeTime(Sounds[i].Delay);
            }

            if (!dragStartSoundDelays.ContainsKey(soundIndex))
                dragStartSoundDelays[soundIndex] = QuantizeTime(Sounds[soundIndex].Delay);

            dragStartDelay = QuantizeTime(Sounds[soundIndex].Delay);
            dragStartDuration = 0f;

            dragControlID = GUIUtility.GetControlID(FocusType.Passive);
            GUIUtility.hotControl = dragControlID;
            GUIUtility.keyboardControl = 0;
            dragStartContentSecond = e.mousePosition.x / pixelsPerSecond;
            snapGuideSecond = -1f;

            if (!IsPreviewing)
            {
                Undo.IncrementCurrentGroup();
                Undo.SetCurrentGroupName("Edit Tween Sound Clip");
                dragUndoGroup = Undo.GetCurrentGroup();
                Undo.RecordObject(target, "Edit Tween Sound Clip");
            }
            else
            {
                dragUndoGroup = -1;
            }

            clipHitThisFrame = true;
            e.Use();
            Repaint();
        }
        /// <summary> 
        ///统一处理 Clip 拖拽的 MouseDrag / MouseUp
        /// </summary>
        /// <param name="viewportArea">ScrollView 视口矩形（Clip 区局部坐标，用于判断边缘自动平移）</param>
        private void ProcessClipDrag(Rect viewportArea)
        {
            Event e = Event.current;

            // 按拖拽类型分别做有效性校验
            bool valid = false;
            if (dragMode != DragMode.无)
            {
                if (draggingKind == TrackKind.Sound)
                    valid = draggingIndex >= 0 && draggingIndex < Sounds.Count;
                else
                    valid = draggingIndex >= 0 && draggingIndex < Nodes.Count;
            }

            if (!valid)
            {
                if (e.type == EventType.MouseUp && GUIUtility.hotControl == dragControlID)
                    GUIUtility.hotControl = 0;
                return;
            }
            switch (e.type)
            {
                case EventType.MouseDrag:
                    if (GUIUtility.hotControl != dragControlID) return;
                    if (draggingKind == TrackKind.Sound)
                        ProcessSoundDrag(e, viewportArea);
                    else
                        ProcessNodeDrag(e, viewportArea);
                    break;
                case EventType.MouseUp:
                    if (dragMode != DragMode.无)
                    {
                        GUIUtility.hotControl = 0;
                        dragMode = DragMode.无;
                        draggingKind = TrackKind.无;
                        draggingIndex = -1;
                        snapGuideSecond = -1f;
                        primaryDragIndex = -1;
                        moveDragAnchorSide = 0;
                        dragStartNodeDelays.Clear();
                        dragStartNodeDurations.Clear();
                        dragStartSoundDelays.Clear();
                        if (dragUndoGroup >= 0)
                        {
                            Undo.CollapseUndoOperations(dragUndoGroup);
                            dragUndoGroup = -1;
                        }
                        e.Use();
                        Repaint();
                    }
                    break;
            }
        }
        private void ProcessNodeDrag(Event e, Rect viewportArea)
        {
            TweenNode node = Nodes[draggingIndex];
            float currentContentSecond = e.mousePosition.x / pixelsPerSecond;
            float totalDelta = currentContentSecond - dragStartContentSecond;
            snapGuideSecond = -1f;
            bool snapOn = this.snapEnabled || e.shift;

            if (!IsPreviewing)
                Undo.RecordObject(target, "Edit Tween Clip");

            switch (dragMode)
            {
                case DragMode.移动:
                    {
                        TweenNode primary = Nodes[primaryDragIndex];
                        float rawDelay = Mathf.Max(0f, dragStartDelay + totalDelta);
                        float rawEnd = QuantizeTime(rawDelay + dragStartDuration);
                        float snappedPrimaryDelay;

                        if (snapOn && moveDragAnchorSide != 0)
                        {
                            if (moveDragAnchorSide == 1)
                            {
                                float snappedStart;
                                bool leftSnapped = TrySnapTimeToReference(rawDelay, selectedNodeIndices, out snappedStart);
                                snappedStart = QuantizeTime(Mathf.Max(0f, snappedStart));
                                if (leftSnapped)
                                {
                                    snappedPrimaryDelay = snappedStart;
                                    snapGuideSecond = snappedStart;
                                }
                                else
                                {
                                    snappedPrimaryDelay = QuantizeTime(rawDelay);
                                    snapGuideSecond = -1f;
                                }
                            }
                            else
                            {
                                float snappedEnd;
                                bool rightSnapped = TrySnapTimeToReference(rawEnd, selectedNodeIndices, out snappedEnd);
                                snappedEnd = QuantizeTime(Mathf.Max(0f, snappedEnd));
                                if (rightSnapped)
                                {
                                    snappedPrimaryDelay = QuantizeTime(Mathf.Max(0f, snappedEnd - dragStartDuration));
                                    snapGuideSecond = snappedEnd;
                                }
                                else
                                {
                                    snappedPrimaryDelay = QuantizeTime(rawDelay);
                                    snapGuideSecond = -1f;
                                }
                            }
                        }
                        else
                        {
                            snappedPrimaryDelay = QuantizeTime(rawDelay);
                            snapGuideSecond = -1f;
                        }

                        float finalDelta = snappedPrimaryDelay - dragStartDelay;

                        // 整体下界：所有选中的动画和音效，Delay + delta >= 0
                        float minStartDelay = float.MaxValue;
                        foreach (var kv in dragStartNodeDelays)
                            if (kv.Value < minStartDelay) minStartDelay = kv.Value;
                        foreach (var kv in dragStartSoundDelays)
                            if (kv.Value < minStartDelay) minStartDelay = kv.Value;
                        if (minStartDelay + finalDelta < 0f)
                            finalDelta = -minStartDelay;

                        // 写回动画
                        foreach (var kv in dragStartNodeDelays)
                        {
                            int idx = kv.Key;
                            if (idx < 0 || idx >= Nodes.Count) continue;
                            Nodes[idx].Delay = QuantizeTime(kv.Value + finalDelta);
                        }
                        // 写回音效
                        foreach (var kv in dragStartSoundDelays)
                        {
                            int idx = kv.Key;
                            if (idx < 0 || idx >= Sounds.Count) continue;
                            Sounds[idx].Delay = QuantizeTime(kv.Value + finalDelta);
                        }
                        break;
                    }

                case DragMode.左边缘:
                    {
                        TweenNode primary = Nodes[primaryDragIndex];
                        float primaryStartDelay = dragStartNodeDelays.ContainsKey(primaryDragIndex)
                            ? dragStartNodeDelays[primaryDragIndex]
                            : QuantizeTime(primary.Delay);

                        float rawDelay = Mathf.Max(0f, primaryStartDelay + totalDelta);
                        float snapped;
                        if (snapOn)
                        {
                            float snapValue;
                            bool snappedFlag = TrySnapTimeToReference(rawDelay, selectedNodeIndices, out snapValue);
                            snapped = QuantizeTime(Mathf.Max(0f, snapValue));
                            snapGuideSecond = snappedFlag ? snapped : -1f;
                        }
                        else
                        {
                            snapped = QuantizeTime(rawDelay);
                            snapGuideSecond = -1f;
                        }

                        float delta = snapped - primaryStartDelay;

                        // 动画约束：左边缘不能越过右边缘，也不能让 Delay < 0
                        float minDelta = float.MinValue;
                        float maxDelta = float.MaxValue;
                        foreach (var kv in dragStartNodeDelays)
                        {
                            int idx = kv.Key;
                            if (idx < 0 || idx >= Nodes.Count) continue;
                            float startD = kv.Value;
                            float startDur = dragStartNodeDurations[idx];
                            float lo = -startD;
                            float hi = startDur - 0.01f;
                            if (lo > minDelta) minDelta = lo;
                            if (hi < maxDelta) maxDelta = hi;
                        }
                        // 音效约束：只约束"起点不能为负"（音效不参与拉伸，但要跟随整体位移）
                        foreach (var kv in dragStartSoundDelays)
                        {
                            float lo = -kv.Value;
                            if (lo > minDelta) minDelta = lo;
                        }

                        delta = Mathf.Clamp(delta, minDelta, maxDelta);

                        // 写回动画：拉伸（Delay + Duration 同时改）
                        foreach (var kv in dragStartNodeDelays)
                        {
                            int idx = kv.Key;
                            if (idx < 0 || idx >= Nodes.Count) continue;
                            float startD = kv.Value;
                            float startDur = dragStartNodeDurations[idx];
                            float endFixed = QuantizeTime(startD + startDur);
                            float newDelay = QuantizeTime(startD + delta);
                            Nodes[idx].Delay = newDelay;
                            Nodes[idx].Duration = QuantizeTime(Mathf.Max(0.01f, endFixed - newDelay));
                        }
                        // 写回音效：只跟随整体位移（Delay 改，长度不变）
                        foreach (var kv in dragStartSoundDelays)
                        {
                            int idx = kv.Key;
                            if (idx < 0 || idx >= Sounds.Count) continue;
                            Sounds[idx].Delay = QuantizeTime(kv.Value + delta);
                        }
                        break;
                    }

                case DragMode.右边缘:
                    {
                        float primaryStartDelay = dragStartNodeDelays.ContainsKey(primaryDragIndex)
                            ? dragStartNodeDelays[primaryDragIndex] : dragStartDelay;
                        float primaryStartDur = dragStartNodeDurations.ContainsKey(primaryDragIndex)
                            ? dragStartNodeDurations[primaryDragIndex] : dragStartDuration;
                        float primaryStartEnd = QuantizeTime(primaryStartDelay + primaryStartDur);
                        float rawEnd = primaryStartEnd + totalDelta;
                        float snapped;
                        if (snapOn)
                        {
                            float snapValue;
                            bool snappedFlag = TrySnapTimeToReference(rawEnd, selectedNodeIndices, out snapValue);
                            snapped = QuantizeTime(Mathf.Max(primaryStartDelay + 0.01f, snapValue));
                            snapGuideSecond = snappedFlag ? snapped : -1f;
                        }
                        else
                        {
                            snapped = QuantizeTime(rawEnd);
                            snapGuideSecond = -1f;
                        }
                        float delta = snapped - primaryStartEnd;

                        float minDelta = float.MinValue;
                        float maxDelta = float.MaxValue;
                        foreach (var kv in dragStartNodeDelays)
                        {
                            int idx = kv.Key;
                            if (idx < 0 || idx >= Nodes.Count) continue;
                            float startD = kv.Value;
                            float startDur = dragStartNodeDurations[idx];
                            float lo = 0.01f - startDur;
                            float hi = float.MaxValue;
                            if (lo > minDelta) minDelta = lo;
                            if (hi < maxDelta) maxDelta = hi;
                        }
                        delta = Mathf.Clamp(delta, minDelta, maxDelta);

                        // 写回动画：仅改 Duration
                        foreach (var kv in dragStartNodeDelays)
                        {
                            int idx = kv.Key;
                            if (idx < 0 || idx >= Nodes.Count) continue;
                            float startD = kv.Value;
                            float startDur = dragStartNodeDurations[idx];
                            Nodes[idx].Delay = QuantizeTime(startD);
                            Nodes[idx].Duration = QuantizeTime(Mathf.Max(0.01f, startDur + delta));
                        }
                        // 右边缘拉伸：音效**不动**（决策 2.a）
                        break;
                    }
            }

            AutoScrollViewOnDragEdge(e.mousePosition.x - scrollPos.x, new Rect(0, 0, viewportArea.width, viewportArea.height));
            EditorUtility.SetDirty(target);
            Repaint();
            e.Use();
        }
        /// <summary> 
        /// 处理音效 Clip 拖拽：只修改 <see cref="TweenSound.Delay"/>。
        /// <para/>
        /// 吸附锚定规则（与动画 Clip 一致）：
        /// <list type="bullet">
        /// <item><description><see cref="moveDragAnchorSide"/> == 1：以左边缘（Delay）为吸附基准；</description></item>
        /// <item><description><see cref="moveDragAnchorSide"/> == 2：以右边缘（Delay + Sound.length）为吸附基准。</description></item>
        /// </list>
        /// </summary>
        private void ProcessSoundDrag(Event e, Rect viewportArea)
        {
            if (draggingIndex < 0 || draggingIndex >= Sounds.Count) return;

            float currentContentSecond = e.mousePosition.x / pixelsPerSecond;
            float totalDelta = currentContentSecond - dragStartContentSecond;
            snapGuideSecond = -1f;
            bool snapOn = this.snapEnabled || e.shift;

            if (!IsPreviewing)
                Undo.RecordObject(target, "Edit Tween Sound Clip");

            // 主拖拽音效
            TweenSound primary = Sounds[draggingIndex];
            float primaryStart = dragStartSoundDelays.ContainsKey(draggingIndex)
                ? dragStartSoundDelays[draggingIndex]
                : QuantizeTime(primary.Delay);

            // 音效长度（Sound 为空时用默认占位长度，与 CalculateSoundClipRect 保持一致）
            float primaryLen = primary.Sound != null ? primary.Sound.length : DefaultSoundClipSeconds;

            // 拖拽起点时间：左边缘锚定 vs 右边缘锚定
            float rawDelay;
            if (moveDragAnchorSide == 1)
            {
                // 左边缘锚定：Delay 跟随鼠标
                rawDelay = Mathf.Max(0f, primaryStart + totalDelta);
            }
            else
            {
                // 右边缘锚定：Delay + 音频长度 跟随鼠标
                float rawEnd = primaryStart + primaryLen + totalDelta;
                rawDelay = Mathf.Max(0f, rawEnd - primaryLen);
            }

            float snapped;
            if (snapOn)
            {
                float snapValue;
                bool snappedFlag;

                if (moveDragAnchorSide == 1)
                {
                    // 以左边缘吸附
                    snappedFlag = TrySnapTimeToReferenceForSound(rawDelay, selectedSoundIndices, out snapValue);
                    snapped = QuantizeTime(Mathf.Max(0f, snapValue));
                    snapGuideSecond = snappedFlag ? snapped : -1f;
                }
                else
                {
                    // 以右边缘吸附：先吸附 rawEnd，再反推 Delay
                    float rawEnd = rawDelay + primaryLen;
                    snappedFlag = TrySnapTimeToReferenceForSound(rawEnd, selectedSoundIndices, out snapValue);
                    snapValue = Mathf.Max(0f, snapValue);
                    snapped = QuantizeTime(snapValue) - primaryLen;
                    snapped = QuantizeTime(Mathf.Max(0f, snapped));
                    snapGuideSecond = snappedFlag ? QuantizeTime(snapValue) : -1f;
                }
            }
            else
            {
                snapped = QuantizeTime(rawDelay);
            }

            float finalDelta = snapped - primaryStart;

            // 整体下界：所有选中的动画和音效，Delay + delta >= 0
            float minStart = float.MaxValue;
            foreach (var kv in dragStartNodeDelays)
                if (kv.Value < minStart) minStart = kv.Value;
            foreach (var kv in dragStartSoundDelays)
                if (kv.Value < minStart) minStart = kv.Value;
            if (minStart + finalDelta < 0f)
                finalDelta = -minStart;

            // 写回音效
            foreach (var kv in dragStartSoundDelays)
            {
                int idx = kv.Key;
                if (idx < 0 || idx >= Sounds.Count) continue;
                Sounds[idx].Delay = QuantizeTime(kv.Value + finalDelta);
            }
            // 写回动画（跟随平移）
            foreach (var kv in dragStartNodeDelays)
            {
                int idx = kv.Key;
                if (idx < 0 || idx >= Nodes.Count) continue;
                Nodes[idx].Delay = QuantizeTime(kv.Value + finalDelta);
            }

            AutoScrollViewOnDragEdge(e.mousePosition.x - scrollPos.x,
                new Rect(0, 0, viewportArea.width, viewportArea.height));
            EditorUtility.SetDirty(target);
            Repaint();
            e.Use();
        }
        /// <summary> 
        ///拖拽时若鼠标靠近 Clip 区左右边缘，自动平移 <see cref="scrollPos"/>
        /// </summary>
        /// <param name="mouseViewportX">鼠标在 ScrollView 视口坐标系下的 X（已减 scrollPos.x）</param>
        /// <param name="viewportArea">ScrollView 视口矩形（宽高用于判断边缘）</param>
        private void AutoScrollViewOnDragEdge(float mouseViewportX, Rect viewportArea)
        {
            if (mouseViewportX < AutoScrollEdgeMargin)
            {
                float t = 1f - Mathf.Clamp01(mouseViewportX / AutoScrollEdgeMargin);
                float speed = AutoScrollSpeed * (0.5f + t * 1.5f);
                scrollPos.x = Mathf.Max(0f, scrollPos.x - speed);
            }
            else if (mouseViewportX > viewportArea.width - AutoScrollEdgeMargin)
            {
                float over = mouseViewportX - (viewportArea.width - AutoScrollEdgeMargin);
                float t = Mathf.Clamp01(over / AutoScrollEdgeMargin);
                float speed = AutoScrollSpeed * (0.5f + t * 1.5f);
                scrollPos.x += speed;
            }
        }
        /// <summary> 
        ///处理中键 / Alt+鼠标左键拖拽平移视图（水平 + 垂直）
        /// </summary>
        /// <param name="localArea">Clip 区局部矩形（刻度尺下方），用于判断按下的位置是否在区内</param>
        private void HandleViewPan(Rect localArea)
        {
            Event e = Event.current;
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            switch (e.type)
            {
                case EventType.MouseDown:
                    bool isPanTrigger = (e.button == 2) || (e.button == 0 && e.alt);
                    if (isPanTrigger && localArea.Contains(e.mousePosition))
                    {
                        isPanning = true;
                        panControlID = controlID;
                        GUIUtility.hotControl = controlID;
                        GUIUtility.keyboardControl = 0;
                        e.Use();
                    }
                    break;
                case EventType.MouseDrag:
                    if (isPanning && GUIUtility.hotControl == panControlID)
                    {
                        scrollPos.x -= e.delta.x;
                        ClampScrollX();   // ★ 替换
                        float maxScrollY = CalculateMaxVerticalScroll();
                        scrollPos.y = Mathf.Clamp(scrollPos.y - e.delta.y, 0f, maxScrollY);
                        nameScroll.y = scrollPos.y;
                        e.Use();
                        Repaint();
                    }
                    break;
                case EventType.MouseUp:
                    bool isPanRelease = (e.button == 2) || (e.button == 0);
                    if (isPanRelease && isPanning)
                    {
                        isPanning = false;
                        GUIUtility.hotControl = 0;
                        e.Use();
                    }
                    break;
            }
        }
        /// <summary> 
        ///处理滚轮操作：单独滚轮水平平移 / Alt+滚轮缩放 / Shift+滚轮调整轨道高度 / Ctrl+滚轮垂直滚动
        /// </summary>
        private void HandleMouseScrollWheel()
        {
            Event e = Event.current;
            if (e.type != EventType.ScrollWheel) return;
            if (target == null) return;
            if (!cachedClipAreaRect.Contains(e.mousePosition)
                && !cachedNameAreaRect.Contains(e.mousePosition)) return;
            if (e.alt)
            {
                float mouseLocalX = e.mousePosition.x - cachedClipAreaRect.x;
                float mouseSecond = (scrollPos.x + mouseLocalX) / pixelsPerSecond;
                float zoomFactor = 1f - e.delta.y * 0.03f;
                pixelsPerSecond = Mathf.Clamp(pixelsPerSecond * zoomFactor, 1f, 20000f);
                scrollPos.x = mouseSecond * pixelsPerSecond - mouseLocalX;
                ClampScrollX();   // ★ 替换
                e.Use();
                Repaint();
                return;
            }
            if (e.shift)
            {
                float wheel = e.delta.y != 0f ? e.delta.y : e.delta.x;
                if (Mathf.Approximately(wheel, 0f)) wheel = 1f;
                float factor = 1f - wheel * TrackHeightZoomStep;
                trackHeight = Mathf.Clamp(trackHeight * factor, MinTrackHeight, MaxTrackHeight);
                float maxScrollY = CalculateMaxVerticalScroll();
                scrollPos.y = Mathf.Clamp(scrollPos.y, 0f, maxScrollY);
                nameScroll.y = scrollPos.y;
                e.Use();
                Repaint();
                return;
            }
            if (e.control)
            {
                float maxScrollY = CalculateMaxVerticalScroll();
                if (maxScrollY <= 0f)
                {
                    e.Use();
                    return;
                }
                scrollPos.y = Mathf.Clamp(scrollPos.y + e.delta.y * 20f, 0f, maxScrollY);
                nameScroll.y = scrollPos.y;
                e.Use();
                Repaint();
                return;
            }
            const float panSpeed = 5f;
            scrollPos.x += e.delta.y * panSpeed;
            ClampScrollX();   // ★ 替换
            e.Use();
            Repaint();
        }
        /// <summary> 
        ///在 Clip 区空白处按下左键时，取消当前选中并清除参数面板焦点
        /// </summary>
        /// <param name="localArea">Clip 区局部矩形（刻度尺下方）</param>
        private void HandleClipAreaEmptyClick(Rect localArea)
        {
            Event e = Event.current;
            if (e.type != EventType.MouseDown) return;
            if (e.button != 0) return;
            if (e.alt) return;
            if (!localArea.Contains(e.mousePosition)) return;
            if (clipHitThisFrame) return;
            selectedNodeIndices.Clear();
            selectedSoundIndices.Clear();
            selectedIndex = -1;
            selectedKind = TrackKind.无;
            GUIUtility.keyboardControl = 0;
            e.Use();
            Repaint();
        }
        /// <summary> 
        ///处理刻度尺区域的左键点击：只清除参数面板焦点，不取消选中
        /// </summary>
        /// <param name="rulerArea">刻度尺在 Clip 区局部坐标系下的矩形</param>
        private void HandleTimeRulerClick(Rect rulerArea)
        {
            Event e = Event.current;
            if (e.type != EventType.MouseDown) return;
            if (e.button != 0) return;
            if (e.alt) return;
            if (!rulerArea.Contains(e.mousePosition)) return;
            GUIUtility.keyboardControl = 0;
            e.Use();
            Repaint();
        }
    }
}
