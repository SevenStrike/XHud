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
    /// 开关的结构类
    /// </summary>
    [System.Serializable]
    public class ElementNode_Toggle
    {
        [SerializeField]
        /// <summary>
        /// 开关组件
        /// </summary>
        public XHud_Module_Toggle Toggle;
    }

    public partial class XHud_Module_Element : MonoBehaviour
    {
        [SerializeField]
        /// <summary>
        /// 开关容器
        /// </summary>
        public List<ElementNode_Toggle> ToggleNodes;
        [SerializeField]
        /// <summary>
        /// 开关列表折叠状态
        /// true表示折叠，false表示展开
        /// 用于控制Inspector中开关列表的显示/隐藏
        /// </summary>
        public bool ToggleIsFold;

        /// <summary>
        /// 启用或禁用所有开关的交互
        /// </summary>
        /// <param name="treeState">状态</param>
        public void element_Toggle_InteractableSetAll(bool state)
        {
            if (ToggleNodes.Count <= 0)
                return;
            for (int i = 0; i < ToggleNodes.Count; i++)
            {
                ToggleNodes[i].Toggle.interactable = state;
            }
            if (state)
            {
                if (DebugState)
                    XGUI_Utilitys.Console("XHud - 元素控件通知", "启用元素下所有开关交互！", XGUIMsgState.通知);
            }
            else
            {
                if (DebugState)
                    XGUI_Utilitys.Console("XHud - 元素控件通知", "禁用元素下所有开关交互！", XGUIMsgState.通知);
            }
        }
        /// <summary>
        /// 启用或禁用所有开关
        /// </summary>
        /// <param name="treeState">状态</param>
        public void element_Toggle_EnableSet(bool state)
        {
            for (int i = 0; i < ToggleNodes.Count; i++)
            {
                ToggleNodes[i].Toggle.enabled = state;
            }
            if (state)
            {
                if (DebugState)
                    XGUI_Utilitys.Console("XHud - 元素控件通知", "启用元素下所有开关脚本！", XGUIMsgState.通知);
            }
            else
            {
                if (DebugState)
                    XGUI_Utilitys.Console("XHud - 元素控件通知", "禁用元素下所有开关脚本！", XGUIMsgState.通知);
            }
        }
        /// <summary>
        /// 获取目标开关
        /// </summary>
        /// <param name="indicator">标识名称</param>
        public XHud_Module_Toggle element_Toggle_Get(string indicator)
        {
            XHud_Module_Toggle toggle = null;
            for (int i = 0; i < ToggleNodes.Count; i++)
            {
                if (ToggleNodes[i].Toggle.Indicator == indicator)
                {
                    toggle = ToggleNodes[i].Toggle;
                }
            }
            if (toggle == null)
            {
                if (DebugState)
                    XGUI_Utilitys.Console("XHud - 元素控件通知", "未找到对应标识的开关！", XGUIMsgState.错误);
            }
            else
            {
                if (DebugState)
                    XGUI_Utilitys.Console("XHud - 元素控件通知", "已获取到标识为 " + indicator + " 的开关！", XGUIMsgState.通知);
            }
            return toggle;
        }
        /// <summary>
        /// 获取所有开关
        /// </summary>
        public XHud_Module_Toggle[] element_Toggle_GetAll()
        {
            List<XHud_Module_Toggle> togglelist = new List<XHud_Module_Toggle>();
            for (int i = 0; i < ToggleNodes.Count; i++)
            {
                togglelist.Add(ToggleNodes[i].Toggle);
            }
            if (togglelist.Count <= 0)
            {
                if (DebugState)
                    XGUI_Utilitys.Console("XHud - 元素控件通知", "获取的开关列表为空！", XGUIMsgState.错误);
            }
            else
            {
                if (DebugState)
                    XGUI_Utilitys.Console("XHud - 元素控件通知", "已获取到" + ToggleNodes.Count + " 个开关！", XGUIMsgState.通知);
            }
            return togglelist.ToArray();
        }
        /// <summary>
        /// 清空所有开关的事件
        /// </summary>
        public void element_Toggle_EventsClear()
        {
            for (int i = 0; i < ToggleNodes.Count; i++)
            {
                ToggleNodes[i].Toggle.EventsClear();
            }
        }
        /// <summary>
        /// 清空所有开关的委托
        /// </summary>
        public void element_Toggle_ActionsClear()
        {
            for (int i = 0; i < ToggleNodes.Count; i++)
            {
                ToggleNodes[i].Toggle.ActionsClear();
            }
        }
        /// <summary>
        /// 清空目标开关的所有委托
        /// </summary>
        /// <param name="toggle"></param>
        public void element_Toggle_Target_ActionsClear(XHud_Module_Toggle toggle)
        {
            toggle.ActionsClear();
        }
        /// <summary>
        /// 清空目标开关的所有事件
        /// </summary>
        /// <param name="toggle"></param>
        public void element_Toggle_Target_EventsClear(XHud_Module_Toggle toggle)
        {
            toggle.EventsClear();
        }
    }
}