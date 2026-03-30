namespace SevenStrikeModules.XHud
{
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.Utilitys;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// 按钮的结构类
    /// </summary>
    [System.Serializable]
    public class ElementNode_Button
    {
        /// <summary>
        /// 按钮组件
        /// </summary>
        public XHud_Module_Button Button;
        /// <summary>
        /// 用于选项按钮
        /// </summary>
        public bool IsOptional;
    }

    public partial class XHud_Module_Element : MonoBehaviour
    {
        [SerializeField]
        /// <summary>
        /// 按钮容器
        /// </summary>
        public List<ElementNode_Button> ButtonNodes;
        [SerializeField]
        /// <summary>
        /// 按钮列表折叠状态
        /// true表示折叠，false表示展开
        /// 用于控制Inspector中按钮列表的显示/隐藏
        /// </summary>
        public bool ButtonIsFold;

        /// <summary>
        /// 启用或禁用所有按钮的交互
        /// </summary>
        /// <param name="treeState">状态</param>
        public void element_Button_InteractableSetAll(bool state)
        {
            if (ButtonNodes.Count <= 0)
                return;
            for (int i = 0; i < ButtonNodes.Count; i++)
            {
                ButtonNodes[i].Button.interactable = state;
            }
            if (state)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("元素控件通知", "启用元素下所有按钮交互！", HudMsgState.通知);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("元素控件通知", "禁用元素下所有按钮交互！", HudMsgState.通知);
            }
        }
        /// <summary>
        /// 启用或禁用所有按钮
        /// </summary>
        /// <param name="treeState">状态</param>
        public void element_Button_EnableSetAll(bool state)
        {
            for (int i = 0; i < ButtonNodes.Count; i++)
            {
                ButtonNodes[i].Button.enabled = state;
            }
            if (state)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("元素控件通知", "启用元素下所有按钮脚本！", HudMsgState.通知);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("元素控件通知", "禁用元素下所有按钮脚本！", HudMsgState.通知);
            }
        }
        /// <summary>
        /// 获取目标按钮
        /// </summary>
        /// <param name="indicator">标识名称</param>
        public XHud_Module_Button element_Button_Get(string indicator)
        {
            XHud_Module_Button btn = null;
            for (int i = 0; i < ButtonNodes.Count; i++)
            {
                if (ButtonNodes[i].Button.Indicator == indicator)
                {
                    btn = ButtonNodes[i].Button;
                }
            }
            if (btn == null)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("元素控件通知", "未找到对应标识的按钮！", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("元素控件通知", "已获取到标识为 " + indicator + " 的按钮！", HudMsgState.通知);
            }
            return btn;
        }
        /// <summary>
        /// 获取所有按钮
        /// </summary>
        public XHud_Module_Button[] element_Button_GetAll(bool IgnoreOptionBtn = true)
        {
            List<XHud_Module_Button> btnlist = new List<XHud_Module_Button>();
            for (int i = 0; i < ButtonNodes.Count; i++)
            {
                if (IgnoreOptionBtn)
                    if (ButtonNodes[i].Button.IsOptionButton)
                        continue;
                btnlist.Add(ButtonNodes[i].Button);
            }
            if (btnlist.Count <= 0)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("元素控件通知", "获取的按钮列表为空！", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("元素控件通知", "已获取到" + ButtonNodes.Count + " 个按钮！", HudMsgState.通知);
            }
            return btnlist.ToArray();
        }
        /// <summary>
        /// 清空所有按钮的事件
        /// </summary>
        public void element_Button_EventsClear(bool IgnoreOptionBtn = true)
        {
            for (int i = 0; i < ButtonNodes.Count; i++)
            {
                if (IgnoreOptionBtn)
                    if (ButtonNodes[i].Button.IsOptionButton)
                        continue;
                ButtonNodes[i].Button.btn_EventsClear();
            }
        }
        /// <summary>
        /// 清空所有按钮的委托
        /// </summary>
        public void element_Button_ActionsClear(bool IgnoreOptionBtn = true)
        {
            for (int i = 0; i < ButtonNodes.Count; i++)
            {
                if (IgnoreOptionBtn)
                    if (ButtonNodes[i].Button.IsOptionButton)
                        continue;
                ButtonNodes[i].Button.btn_ActionsClear();
            }
        }
        /// <summary>
        /// 清空目标按钮的所有委托
        /// </summary>
        /// <param name="btn"></param>
        public void element_Button_Target_ActionsClear(XHud_Module_Button btn, bool IgnoreOptionBtn = true)
        {
            if (IgnoreOptionBtn)
                if (btn.IsOptionButton)
                    return;
            btn.btn_ActionsClear();
        }
        /// <summary>
        /// 清空目标按钮的所有事件
        /// </summary>
        /// <param name="btn"></param>
        public void element_Button_Target_EventsClear(XHud_Module_Button btn, bool IgnoreOptionBtn = true)
        {
            if (IgnoreOptionBtn)
                if (btn.IsOptionButton)
                    return;
            btn.btn_EventsClear();
        }
    }
}