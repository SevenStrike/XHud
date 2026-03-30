namespace SevenStrikeModules.XHud
{
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.Utilitys;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// 开关的结构类
    /// </summary>
    [System.Serializable]
    public class ElementNode_Toggle
    {
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
                    XHud_Utilitys.Func_PrintInfo("元素控件通知", "启用元素下所有开关交互！", HudMsgState.通知);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("元素控件通知", "禁用元素下所有开关交互！", HudMsgState.通知);
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
                    XHud_Utilitys.Func_PrintInfo("元素控件通知", "启用元素下所有开关脚本！", HudMsgState.通知);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("元素控件通知", "禁用元素下所有开关脚本！", HudMsgState.通知);
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
                    XHud_Utilitys.Func_PrintInfo("元素控件通知", "未找到对应标识的开关！", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("元素控件通知", "已获取到标识为 " + indicator + " 的开关！", HudMsgState.通知);
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
                    XHud_Utilitys.Func_PrintInfo("元素控件通知", "获取的开关列表为空！", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("元素控件通知", "已获取到" + ToggleNodes.Count + " 个开关！", HudMsgState.通知);
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