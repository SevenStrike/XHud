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
                    XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "启用元素下所有按钮交互！", HudMsgState.通知);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "禁用元素下所有按钮交互！", HudMsgState.通知);
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
                    XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "启用元素下所有按钮脚本！", HudMsgState.通知);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "禁用元素下所有按钮脚本！", HudMsgState.通知);
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
                    XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "未找到对应标识的按钮！", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "已获取到标识为 " + indicator + " 的按钮！", HudMsgState.通知);
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
                    XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "获取的按钮列表为空！", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "已获取到" + ButtonNodes.Count + " 个按钮！", HudMsgState.通知);
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