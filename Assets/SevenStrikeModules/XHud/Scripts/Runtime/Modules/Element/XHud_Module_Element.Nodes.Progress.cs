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
    using SevenStrikeModules.XGUI.Runtime;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// 进度条的结构类
    /// </summary>
    [System.Serializable]
    public class ElementNode_Progress
    {
        [SerializeField]
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
                    XGUI_Utilitys.Console("XHud - 元素控件通知", "启用元素下所有进度条脚本！", XGUIMsgState.通知);
            }
            else
            {
                if (DebugState)
                    XGUI_Utilitys.Console("XHud - 元素控件通知", "禁用元素下所有进度条脚本！", XGUIMsgState.通知);
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
                    XGUI_Utilitys.Console("XHud - 元素控件通知", "未找到对应标识的进度条！", XGUIMsgState.错误);
            }
            else
            {
                if (DebugState)
                    XGUI_Utilitys.Console("XHud - 元素控件通知", "已获取到标识为 " + indicator + " 的进度条！", XGUIMsgState.通知);
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
                    XGUI_Utilitys.Console("XHud - 元素控件通知", "获取的进度条列表为空！", XGUIMsgState.错误);
            }
            else
            {
                if (DebugState)
                    XGUI_Utilitys.Console("XHud - 元素控件通知", "已获取到" + ProgressNodes.Count + " 个进度条！", XGUIMsgState.通知);
            }
            return progresslist.ToArray();
        }
    }
}