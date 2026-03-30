namespace SevenStrikeModules.XHud
{
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.Utilitys;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// 进度条的结构类
    /// </summary>
    [System.Serializable]
    public class ElementNode_Progress
    {
        /// <summary>
        /// 进度条组件
        /// </summary>
        public XHud_Module_Progress Progress;
    }

    public partial class XHud_Module_Element : MonoBehaviour
    {
        [SerializeField]
        /// <summary>
        /// 进度条容器
        /// </summary>
        public List<ElementNode_Progress> ProgressNodes;
        [SerializeField]
        /// <summary>
        /// 进度条列表折叠状态
        /// true表示折叠，false表示展开
        /// 用于控制Inspector中进度条列表的显示/隐藏
        /// </summary>
        public bool ProgressIsFold;

        /// <summary>
        /// 启用或禁用所有进度条
        /// </summary>
        /// <param name="treeState">状态</param>
        public void element_Progress_EnableSetAll(bool state)
        {
            if (ProgressNodes.Count <= 0)
                return;
            for (int i = 0; i < ProgressNodes.Count; i++)
            {
                ProgressNodes[i].Progress.enabled = state;
            }
            if (state)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("元素控件通知", "启用元素下所有进度条脚本！", HudMsgState.通知);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("元素控件通知", "禁用元素下所有进度条脚本！", HudMsgState.通知);
            }
        }
        /// <summary>
        /// 获取目标进度条
        /// </summary>
        /// <param name="indicator">标识名称</param>
        public XHud_Module_Progress element_Progress_Get(string indicator)
        {
            XHud_Module_Progress progress = null;
            for (int i = 0; i < ProgressNodes.Count; i++)
            {
                if (ProgressNodes[i].Progress.Indicator == indicator)
                {
                    progress = ProgressNodes[i].Progress;
                }
            }
            if (progress == null)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("元素控件通知", "未找到对应标识的进度条！", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("元素控件通知", "已获取到标识为 " + indicator + " 的进度条！", HudMsgState.通知);
            }
            return progress;
        }
        /// <summary>
        /// 获取所有进度条
        /// </summary>
        public XHud_Module_Progress[] element_Progress_GetAll()
        {
            List<XHud_Module_Progress> progresslist = new List<XHud_Module_Progress>();
            for (int i = 0; i < ProgressNodes.Count; i++)
            {
                progresslist.Add(ProgressNodes[i].Progress);
            }
            if (progresslist.Count <= 0)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("元素控件通知", "获取的进度条列表为空！", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("元素控件通知", "已获取到" + ProgressNodes.Count + " 个进度条！", HudMsgState.通知);
            }
            return progresslist.ToArray();
        }
    }
}