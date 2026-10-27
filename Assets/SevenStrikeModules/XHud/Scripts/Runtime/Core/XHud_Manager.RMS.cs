/*
 * ============================================================================
 * ⚠️ 版权声明（禁止删除、禁止修改、衍生作品必须保留此注释）⚠️
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
namespace SevenStrikeModules.XHud
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// 布局匹配分辨率类
    /// </summary>
    [System.Serializable]
    public class ScreenResolutionNode
    {
        public string indicator;
        public Vector2 res;
    }

    public partial class XHud_Manager : MonoBehaviour
    {
        /// <summary>
        ///布局匹配方案模式
        /// </summary>
        public bool rms_enabled;
        /// <summary>
        /// 布局匹配方案列表
        /// </summary>
        public List<ScreenResolutionNode> rms_nodes = new List<ScreenResolutionNode>();
        /// <summary>
        /// RMS当前选中的方案
        /// </summary>
        public string rms_current_solution;

        /// <summary>
        /// 获取RMS的节点列表
        /// </summary>
        public ScreenResolutionNode[] hm_RMS_GetResolutionNodes()
        {
            return rms_nodes.ToArray();
        }
        /// <summary>
        /// 获取RMS的节点所有名称
        /// </summary>
        public string[] hm_RMS_GetResolutionNodeNames()
        {
            string[] names = new string[rms_nodes.Count];
            for (int i = 0; i < rms_nodes.Count; i++)
            {
                names[i] = rms_nodes[i].indicator;
            }
            return names;
        }
        /// <summary>
        /// 获取RMS的节点
        /// </summary>
        public ScreenResolutionNode hm_RMS_GetResolutionNode(string name)
        {
            ScreenResolutionNode node = new ScreenResolutionNode();
            for (int i = 0; i < rms_nodes.Count; i++)
            {
                if (rms_nodes[i].indicator == name)
                    node = rms_nodes[i];
            }
            return node;
        }
        /// <summary>
        /// 获取当前使用中的RMS布局方案
        /// </summary>
        /// <returns></returns>
        public string hm_RMS_GetCurrentSolution()
        {
            return rms_current_solution;
        }
        /// <summary>
        /// 根据方案名称获取RMS的索引号
        /// </summary>
        /// <param name="solution"></param>
        /// <returns></returns>
        public int hm_RMS_GetSolutionIndex(string solution)
        {
            int index = 0;
            for (int i = 0; i < rms_nodes.Count; i++)
            {
                if (solution == rms_nodes[i].indicator)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }
        /// <summary>
        /// 获取匹配屏幕分辨率节点列表总数
        /// </summary>
        public int hm_RMS_GetResolutionNodesLength()
        {
            return rms_nodes.Count;
        }
        /// <summary>
        /// 获取匹配屏幕分辨率节点列表是否为空
        /// </summary>
        public bool hm_RMS_IsEmpty()
        {
            return rms_nodes.Count > 0 ? false : true;
        }
        /// <summary>
        /// 根据分辨率标识符设置屏幕分辨率
        /// </summary>
        /// <param name="indicator">分辨率标识符，用于在分辨率列表中查找对应的分辨率配置</param>
        /// <param name="mode">全屏模式，指定窗口的显示方式（全屏、窗口化、无边框窗口等）</param>
        public void hm_RMS_SetResolution(string indicator, FullScreenMode mode)
        {
            // 遍历所有已配置的分辨率节点
            for (int i = 0; i < rms_nodes.Count; i++)
            {
                // 查找与传入标识符匹配的分辨率配置
                if (indicator == rms_nodes[i].indicator)
                {
                    // 调用重载方法，根据匹配到的分辨率宽高设置屏幕分辨率
                    hm_RMS_SetResolution((int)rms_nodes[i].res.x, (int)rms_nodes[i].res.y, mode);

                    // 更新当前使用的分辨率标识符，用于记录当前分辨率状态
                    rms_current_solution = indicator;

                    // 如果屏幕分辨率改变事件有订阅者，则触发事件
                    // 事件参数：分辨率标识符 和 对应的分辨率向量
                    if (act_screen_resolution_changed != null)
                        act_screen_resolution_changed(indicator, rms_nodes[i].res);
                }
            }
        }
        /// <summary>
        /// 通过指定宽高设置屏幕分辨率
        /// </summary>
        /// <param name="width">屏幕宽度（像素）</param>
        /// <param name="height">屏幕高度（像素）</param>
        /// <param name="mode">全屏模式，可选值：ExclusiveFullScreen（独占全屏）、FullScreenWindow（无边框全屏窗口）、Windowed（窗口模式）</param>
        public void hm_RMS_SetResolution(int width, int height, FullScreenMode mode)
        {
            // 调用 Unity 引擎的 Screen.SetResolution 方法实际应用分辨率设置
            // 该方法会立即更改游戏窗口的显示分辨率
            Screen.SetResolution(width, height, mode);
        }
    }
}