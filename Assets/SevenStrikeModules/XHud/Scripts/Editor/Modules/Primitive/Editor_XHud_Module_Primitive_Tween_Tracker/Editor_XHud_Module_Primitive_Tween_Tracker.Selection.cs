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
        /// 取指定集合里索引最小的（视觉最上面）。 
        /// </summary>
        private int GetTopmostSelectedIndex(HashSet<int> set)
        {
            int top = int.MaxValue;
            foreach (int i in set) if (i < top) top = i;
            return top == int.MaxValue ? -1 : top;
        }
        /// <summary> 
        /// 取当前主类型选中集合里索引最小的。 
        /// </summary>
        private int GetTopmostSelectedIndex()
        {
            if (selectedKind == TrackKind.Node) return GetTopmostSelectedIndex(selectedNodeIndices);
            if (selectedKind == TrackKind.Sound) return GetTopmostSelectedIndex(selectedSoundIndices);
            return -1;
        }
        /// <summary> 
        /// 判断某个轨道项是否被选中。
        /// </summary>
        private bool IsSelected(TrackKind kind, int index)
        {
            if (kind == TrackKind.Node) return selectedNodeIndices.Contains(index);
            if (kind == TrackKind.Sound) return selectedSoundIndices.Contains(index);
            return false;
        }
        /// <summary>
        /// 统一的选中设置入口。
        /// <para/>
        /// 支持 Ctrl 切换单项（允许跨类型累加）、Shift 范围选（仅同类型内生效）；
        /// 普通单击会清空所有类型的选中，只保留当前项。
        /// </summary>
        private void SetSelection(TrackKind kind, int index, bool additive, bool range)
        {
            int count = kind == TrackKind.Node ? Nodes.Count : Sounds.Count;
            if (index < 0 || index >= count)
            {
                // 越界：清空该类型选中
                if (kind == TrackKind.Node) selectedNodeIndices.Clear();
                else if (kind == TrackKind.Sound) selectedSoundIndices.Clear();
                selectedIndex = -1;
                if (TotalSelectedCount == 0) selectedKind = TrackKind.无;
                return;
            }

            HashSet<int> currentSet = kind == TrackKind.Node ? selectedNodeIndices : selectedSoundIndices;

            if (range && selectedKind == kind && selectedIndex >= 0)
            {
                // Shift 范围选：只在同类型内生效
                int from = Mathf.Min(selectedIndex, index);
                int to = Mathf.Max(selectedIndex, index);
                currentSet.Clear();
                for (int i = from; i <= to; i++) currentSet.Add(i);
            }
            else if (additive)
            {
                // Ctrl 切换单项：允许跨类型累加
                if (currentSet.Contains(index))
                {
                    currentSet.Remove(index);
                    if (selectedKind == kind && selectedIndex == index)
                        selectedIndex = currentSet.Count > 0 ? GetTopmostSelectedIndex(currentSet) : -1;
                }
                else
                {
                    currentSet.Add(index);
                    selectedIndex = index;
                    selectedKind = kind;
                }
            }
            else
            {
                // 普通单击：清空所有类型的选中，只保留当前项
                selectedNodeIndices.Clear();
                selectedSoundIndices.Clear();
                currentSet.Add(index);
                selectedIndex = index;
                selectedKind = kind;
            }

            GUIUtility.keyboardControl = 0;
            Repaint();
        }
        /// <summary> 
        /// 当前选中集合对应的列表数量（Node / Sound）。
        /// </summary>
        private int SelectedListCount =>
            selectedKind == TrackKind.Node ? Nodes.Count :
            selectedKind == TrackKind.Sound ? Sounds.Count : 0;
    }
}
