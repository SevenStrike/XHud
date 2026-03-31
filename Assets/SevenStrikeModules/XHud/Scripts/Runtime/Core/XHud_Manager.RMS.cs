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
        public string Indicator;
        public Vector2 Res;
    }

    public partial class XHud_Manager : MonoBehaviour
    {
        /// <summary>
        ///布局匹配方案模式
        /// </summary>
        public bool RMS_Enabled;
        /// <summary>
        /// 布局匹配方案列表
        /// </summary>
        public List<ScreenResolutionNode> RMS_Nodes = new List<ScreenResolutionNode>();
        /// <summary>
        /// RMS当前选中的方案
        /// </summary>
        public string RMS_CurrentSolution;

        /// <summary>
        /// 获取RMS的节点列表
        /// </summary>
        public ScreenResolutionNode[] hm_RMS_GetResolutionNodes()
        {
            return RMS_Nodes.ToArray();
        }
        /// <summary>
        /// 获取RMS的节点所有名称
        /// </summary>
        public string[] hm_RMS_GetResolutionNodeNames()
        {
            string[] names = new string[RMS_Nodes.Count];
            for (int i = 0; i < RMS_Nodes.Count; i++)
            {
                names[i] = RMS_Nodes[i].Indicator;
            }
            return names;
        }
        /// <summary>
        /// 获取RMS的节点
        /// </summary>
        public ScreenResolutionNode hm_RMS_GetResolutionNode(string name)
        {
            ScreenResolutionNode node = new ScreenResolutionNode();
            for (int i = 0; i < RMS_Nodes.Count; i++)
            {
                if (RMS_Nodes[i].Indicator == name)
                    node = RMS_Nodes[i];
            }
            return node;
        }
        /// <summary>
        /// 获取当前使用中的RMS布局方案
        /// </summary>
        /// <returns></returns>
        public string hm_RMS_GetCurrentSolution()
        {
            return RMS_CurrentSolution;
        }
        /// <summary>
        /// 根据方案名称获取RMS的索引号
        /// </summary>
        /// <param name="solution"></param>
        /// <returns></returns>
        public int hm_RMS_GetSolutionIndex(string solution)
        {
            int index = 0;
            for (int i = 0; i < RMS_Nodes.Count; i++)
            {
                if (solution == RMS_Nodes[i].Indicator)
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
            return RMS_Nodes.Count;
        }
        /// <summary>
        /// 获取匹配屏幕分辨率节点列表是否为空
        /// </summary>
        public bool hm_RMS_IsEmpty()
        {
            return RMS_Nodes.Count > 0 ? false : true;
        }
        /// <summary>
        /// 根据分辨率标识符设置屏幕分辨率
        /// </summary>
        /// <param name="indicator">分辨率标识符，用于在分辨率列表中查找对应的分辨率配置</param>
        /// <param name="mode">全屏模式，指定窗口的显示方式（全屏、窗口化、无边框窗口等）</param>
        public void hm_RMS_SetResolution(string indicator, FullScreenMode mode)
        {
            // 遍历所有已配置的分辨率节点
            for (int i = 0; i < RMS_Nodes.Count; i++)
            {
                // 查找与传入标识符匹配的分辨率配置
                if (indicator == RMS_Nodes[i].Indicator)
                {
                    // 调用重载方法，根据匹配到的分辨率宽高设置屏幕分辨率
                    hm_RMS_SetResolution((int)RMS_Nodes[i].Res.x, (int)RMS_Nodes[i].Res.y, mode);

                    // 更新当前使用的分辨率标识符，用于记录当前分辨率状态
                    RMS_CurrentSolution = indicator;

                    // 如果屏幕分辨率改变事件有订阅者，则触发事件
                    // 事件参数：分辨率标识符 和 对应的分辨率向量
                    if (Act_ScreenResolution_Changed != null)
                        Act_ScreenResolution_Changed(indicator, RMS_Nodes[i].Res);
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