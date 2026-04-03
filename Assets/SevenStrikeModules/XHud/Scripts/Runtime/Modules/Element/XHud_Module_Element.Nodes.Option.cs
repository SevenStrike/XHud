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
    /// 选项按钮的结构类
    /// </summary>
    [System.Serializable]
    public class ElementNode_OptionButton
    {
        /// <summary>
        /// 按钮组件
        /// </summary>
        public XHud_Module_Button Button;
        /// <summary>
        /// 选项按钮标识
        /// </summary>
        public string Indicator;
        /// <summary>
        /// 用于选项按钮
        /// </summary>
        public bool IsOptional;
    }

    /// <summary>
    /// 选项的结构类
    /// </summary>
    [System.Serializable]
    public class ElementNode_Option
    {
        /// <summary>
        /// 按钮组件
        /// </summary>
        public XHud_Module_Option Option;
    }

    public partial class XHud_Module_Element : MonoBehaviour
    {
        [SerializeField]
        /// <summary>
        /// 选项容器
        /// </summary>
        public List<ElementNode_Option> OptionNodes;
        [SerializeField]
        /// <summary>
        /// 选项列表折叠状态
        /// true表示折叠，false表示展开
        /// 用于控制Inspector中选项列表的显示/隐藏
        /// </summary>
        public bool OptionIsFold;

        /// <summary>
        /// 启用或禁用所有选项的交互
        /// </summary>
        /// <param name="treeState">状态</param>
        public void element_Option_InteractableSetAll(bool state)
        {
            if (OptionNodes.Count <= 0)
                return;
            for (int i = 0; i < OptionNodes.Count; i++)
            {
                for (int s = 0; s < OptionNodes[i].Option.OptionButtonNodes.Count; s++)
                {
                    OptionNodes[i].Option.OptionButtonNodes[s].Button.interactable = state;
                }
            }
            if (state)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "启用元素下所有选项交互！", HudMsgState.通知);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "禁用元素下所有选项交互！", HudMsgState.通知);
            }
        }
        /// <summary>
        /// 启用或禁用所有选项
        /// </summary>
        /// <param name="treeState">状态</param>
        public void element_Option_EnableSetAll(bool state)
        {
            for (int i = 0; i < OptionNodes.Count; i++)
            {
                OptionNodes[i].Option.enabled = state;
            }
            if (state)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "启用元素下所有选项脚本！", HudMsgState.通知);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "禁用元素下所有选项脚本！", HudMsgState.通知);
            }
        }
        /// <summary>
        /// 获取目标选项
        /// </summary>
        /// <param name="indicator">标识名称</param>
        public XHud_Module_Option element_Option_Get(string indicator)
        {
            XHud_Module_Option opt = null;
            for (int i = 0; i < OptionNodes.Count; i++)
            {
                if (OptionNodes[i].Option.Indicator == indicator)
                {
                    opt = OptionNodes[i].Option;
                }
            }
            if (opt == null)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "未找到对应标识的选项！", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "已获取到标识为 " + indicator + " 的选项！", HudMsgState.通知);
            }
            return opt;
        }
        /// <summary>
        /// 获取所有选项
        /// </summary>
        public XHud_Module_Option[] element_Option_GetAll()
        {
            List<XHud_Module_Option> optlist = new List<XHud_Module_Option>();
            for (int i = 0; i < OptionNodes.Count; i++)
            {
                optlist.Add(OptionNodes[i].Option);
            }
            if (optlist.Count <= 0)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "获取的选项列表为空！", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "已获取到" + OptionNodes.Count + " 个选项！", HudMsgState.通知);
            }
            return optlist.ToArray();
        }
        /// <summary>
        /// 清空所有选项的事件
        /// </summary>
        public void element_Option_EventsClear()
        {
            for (int i = 0; i < OptionNodes.Count; i++)
            {
                OptionNodes[i].Option.opt_ClearEvents();
            }
        }
        /// <summary>
        /// 清空所有选项的委托
        /// </summary>
        public void element_Option_ActionsClear()
        {
            for (int i = 0; i < OptionNodes.Count; i++)
            {
                OptionNodes[i].Option.opt_ClearActions();
            }
        }
        /// <summary>
        /// 清空目标选项的所有委托
        /// </summary>
        /// <param name="opt"></param>
        public void element_Option_Target_ActionsClear(XHud_Module_Option opt)
        {
            opt.opt_ClearActions();
        }
        /// <summary>
        /// 清空目标选项的所有事件
        /// </summary>
        /// <param name="opt"></param>
        public void element_Option_Target_EventsClear(XHud_Module_Option opt)
        {
            opt.opt_ClearEvents();
        }
    }
}