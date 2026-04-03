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
    /// 数据源的结构类
    /// </summary>
    [System.Serializable]
    public class ElementNode_Container
    {
        /// <summary>
        /// Hud容器
        /// </summary>
        public XHud_Module_Container Container;
    }

    public partial class XHud_Module_Element : MonoBehaviour
    {
        [SerializeField]
        /// <summary>
        /// 数据源容器
        /// </summary>
        public List<ElementNode_Container> ContainerNodes;
        [SerializeField]
        /// <summary>
        /// 容器动画自动播放开关
        /// true表示容器内的子元素生成时自动播放动画
        /// false表示需要手动控制容器动画
        /// 默认值为true
        /// </summary>
        public bool AutoPlayContainersAnimators = true;
        [SerializeField]
        /// <summary>
        /// 容器列表折叠状态
        /// true表示折叠，false表示展开
        /// 用于控制Inspector中容器列表的显示/隐藏
        /// </summary>
        public bool ContainerIsFold;


        /// <summary>
        /// 启用或禁用所有容器的交互
        /// </summary>
        /// <param name="treeState">状态</param>
        public void element_Container_InteractableSetAll(bool state)
        {
            if (ContainerNodes.Count <= 0)
                return;
            for (int i = 0; i < ContainerNodes.Count; i++)
            {
                if (ContainerNodes[i] != null)
                {
                    ContainerNodes[i].Container.IsEnabled = state;
                }
                else
                {
                    if (DebugState)
                        XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "失效的容器项！索引号：" + i, HudMsgState.通知);
                }
            }
            if (state)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "启用元素下所有容器功能！", HudMsgState.通知);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "禁用元素下所有容器功能！", HudMsgState.通知);
            }
        }
        /// <summary>
        /// 启用或禁用所有容器
        /// </summary>
        /// <param name="treeState">状态</param>
        public void element_Container_EnableSetAll(bool state)
        {
            if (ContainerNodes.Count <= 0)
                return;
            for (int i = 0; i < ContainerNodes.Count; i++)
            {
                if (ContainerNodes[i] != null)
                {
                    ContainerNodes[i].Container.enabled = state;
                }
                else
                {
                    if (DebugState)
                        XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "失效的容器项！索引号：" + i, HudMsgState.通知);
                }
            }
            if (state)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "启用元素下所有容器脚本！", HudMsgState.通知);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "禁用元素下所有容器脚本！", HudMsgState.通知);
            }
        }
        /// <summary>
        /// 获取目标容器
        /// </summary>
        /// <param name="indicator">标识名称</param>
        public XHud_Module_Container element_Container_Get(string indicator)
        {
            XHud_Module_Container con = null;
            for (int i = 0; i < ContainerNodes.Count; i++)
            {
                if (ContainerNodes[i].Container.Indicator == indicator)
                {
                    con = ContainerNodes[i].Container;
                }
            }
            if (con == null)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "未找到对应标识的容器！", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "已获取到标识为 " + indicator + " 的容器！", HudMsgState.通知);
            }
            return con;
        }
        /// <summary>
        /// 获取所有容器
        /// </summary>
        public XHud_Module_Container[] element_Container_GetAll()
        {
            List<XHud_Module_Container> conlist = new List<XHud_Module_Container>();
            for (int i = 0; i < ContainerNodes.Count; i++)
            {
                conlist.Add(ContainerNodes[i].Container);
            }
            if (conlist.Count <= 0)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "获取的容器列表为空！", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "已获取到" + ContainerNodes.Count + " 个容器！", HudMsgState.通知);
            }
            return conlist.ToArray();
        }

        #region Play

        /// <summary>
        /// 播放容器动画
        /// </summary>
        /// <param name="usedelay">播放延迟</param>
        public void element_Container_Animators_PlayAll(bool usedelay = true)
        {
            for (int i = 0; i < ContainerNodes.Count; i++)
            {
                element_Container_Animators_PlayAt(ContainerNodes[i].Container.Indicator, usedelay);
            }
        }
        /// <summary>
        /// 播放容器动画
        /// </summary>
        /// <param name="con_indicator">容器标识名称</param>
        /// <param name="usedelay">播放延迟</param>
        public void element_Container_Animators_PlayAt(string con_indicator, bool usedelay = true)
        {
            for (int i = 0; i < ContainerNodes.Count; i++)
            {
                if (ContainerNodes[i].Container.Indicator == con_indicator)
                    ContainerNodes[i].Container.Con_Animators_PlayAll(usedelay);
            }
        }
        /// <summary>
        /// 播放容器动画
        /// </summary>
        /// <param name="con_indicator">容器标识名称</param>
        /// <param name="item_indicator">项标识名称</param>
        /// <param name="usedelay">播放延迟</param>
        public void element_Container_Animators_PlayAt(string con_indicator, string item_indicator, bool usedelay = true)
        {
            for (int i = 0; i < ContainerNodes.Count; i++)
            {
                if (ContainerNodes[i].Container.Indicator == con_indicator)
                {
                    for (int s = 0; s < ContainerNodes[i].Container.ContainerItems.Count; s++)
                    {
                        if (ContainerNodes[i].Container.ContainerItems[s].Indicator == item_indicator)
                        {
                            ContainerNodes[i].Container.Con_Animators_PlayAt(item_indicator, usedelay);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 播放容器动画
        /// </summary>
        /// <param name="con_indicator">容器标识名称</param>
        /// <param name="item_id">项标识ID</param>
        /// <param name="usedelay">播放延迟</param>
        public void element_Container_Animators_PlayAt(string con_indicator, int item_id, bool usedelay = true)
        {
            for (int i = 0; i < ContainerNodes.Count; i++)
            {
                if (ContainerNodes[i].Container.Indicator == con_indicator)
                {
                    for (int s = 0; s < ContainerNodes[i].Container.ContainerItems.Count; s++)
                    {
                        if (ContainerNodes[i].Container.ContainerItems[s].ID == item_id)
                        {
                            ContainerNodes[i].Container.Con_Animators_PlayAt(item_id, usedelay);
                        }
                    }
                }
            }
        }
        #endregion

        #region Ready
        /// <summary>
        /// 就绪容器动画
        /// </summary>
        //public void element_Container_Animators_ReadyAll()
        //{
        //    for (int i = 0; i < ContainerNodes.Count; i++)
        //    {
        //        element_Container_Animators_ReadyAt(ContainerNodes[i].Container.Indicator);
        //    }
        //}

        /// <summary>
        /// 就绪容器动画
        /// </summary>
        /// <param name="con_indicator">容器标识名称</param>
        //public void element_Container_Animators_ReadyAt(string con_indicator)
        //{
        //    for (int i = 0; i < ContainerNodes.Count; i++)
        //    {
        //        if (ContainerNodes[i].Container.Indicator == con_indicator)
        //            ContainerNodes[i].Container.Con_Animators_ReadyAll();
        //    }
        //}

        /// <summary>
        /// 就绪容器动画
        /// </summary>
        /// <param name="con_indicator">容器标识名称</param>
        /// <param name="item_indicator">项标识名称</param>
        //public void element_Container_Animators_ReadyAt(string con_indicator, string item_indicator)
        //{
        //    for (int i = 0; i < ContainerNodes.Count; i++)
        //    {
        //        if (ContainerNodes[i].Container.Indicator == con_indicator)
        //        {
        //            for (int s = 0; s < ContainerNodes[i].Container.ContainerItems.Count; s++)
        //            {
        //                if (ContainerNodes[i].Container.ContainerItems[s].Indicator == item_indicator)
        //                {
        //                    ContainerNodes[i].Container.Con_Animators_ReadyAt(item_indicator);
        //                }
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// 就绪容器动画
        /// </summary>
        /// <param name="con_indicator">容器标识名称</param>
        /// <param name="item_id">项标识ID</param>
        //public void element_Container_Animators_ReadyAt(string con_indicator, int item_id)
        //{
        //    for (int i = 0; i < ContainerNodes.Count; i++)
        //    {
        //        if (ContainerNodes[i].Container.Indicator == con_indicator)
        //        {
        //            for (int s = 0; s < ContainerNodes[i].Container.ContainerItems.Count; s++)
        //            {
        //                if (ContainerNodes[i].Container.ContainerItems[s].ID == item_id)
        //                {
        //                    ContainerNodes[i].Container.Con_Animators_ReadyAt(item_id);
        //                }
        //            }
        //        }
        //    }
        //}
        #endregion

        #region Rewind
        /// <summary>
        /// 复位容器动画
        /// </summary>
        public void element_Container_Animators_RewindAll()
        {
            for (int i = 0; i < ContainerNodes.Count; i++)
            {
                element_Container_Animators_RewindAt(ContainerNodes[i].Container.Indicator);
            }
        }
        /// <summary>
        /// 复位容器动画
        /// </summary>
        /// <param name="con_indicator">容器标识名称</param>
        public void element_Container_Animators_RewindAt(string con_indicator)
        {
            for (int i = 0; i < ContainerNodes.Count; i++)
            {
                if (ContainerNodes[i].Container.Indicator == con_indicator)
                    ContainerNodes[i].Container.Con_Animators_RewindAt(con_indicator);
            }
        }
        /// <summary>
        /// 复位容器动画
        /// </summary>
        /// <param name="con_indicator">容器标识名称</param>
        /// <param name="item_indicator">项标识名称</param>
        public void element_Container_Animators_RewindAt(string con_indicator, string item_indicator)
        {
            for (int i = 0; i < ContainerNodes.Count; i++)
            {
                if (ContainerNodes[i].Container.Indicator == con_indicator)
                {
                    for (int s = 0; s < ContainerNodes[i].Container.ContainerItems.Count; s++)
                    {
                        if (ContainerNodes[i].Container.ContainerItems[s].Indicator == item_indicator)
                        {
                            ContainerNodes[i].Container.Con_Animators_RewindAt(item_indicator);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 复位容器动画
        /// </summary>
        /// <param name="con_indicator">容器标识名称</param>
        /// <param name="item_id">项标识ID</param>
        public void element_Container_Animators_RewindAt(string con_indicator, int item_id)
        {
            for (int i = 0; i < ContainerNodes.Count; i++)
            {
                if (ContainerNodes[i].Container.Indicator == con_indicator)
                {
                    for (int s = 0; s < ContainerNodes[i].Container.ContainerItems.Count; s++)
                    {
                        if (ContainerNodes[i].Container.ContainerItems[s].ID == item_id)
                        {
                            ContainerNodes[i].Container.Con_Animators_RewindAt(item_id);
                        }
                    }
                }
            }
        }
        #endregion

        #region Actions & Events
        /// <summary>
        /// 清空所有开关的事件
        /// </summary>
        public void element_Container_EventsClear()
        {
            for (int i = 0; i < ContainerNodes.Count; i++)
            {
                ContainerNodes[i].Container.EventsClear();
            }
        }
        /// <summary>
        /// 清空所有开关的委托
        /// </summary>
        public void element_Container_ActionsClear()
        {
            for (int i = 0; i < ContainerNodes.Count; i++)
            {
                ContainerNodes[i].Container.ActionsClear();
            }
        }
        /// <summary>
        /// 清空目标开关的所有委托
        /// </summary>
        /// <param name="toggle"></param>
        public void element_Container_Target_ActionsClear(XHud_Module_Container container)
        {
            container.ActionsClear();
        }
        /// <summary>
        /// 清空目标开关的所有事件
        /// </summary>
        /// <param name="toggle"></param>
        public void element_Container_Target_EventsClear(XHud_Module_Container container)
        {
            container.EventsClear();
        }
        #endregion

        #region ValueSet
        /// <summary>
        /// 修改目标容器项内容
        /// </summary>
        /// <param name="container">目标容器</param>
        /// <param name="indicator">目标项</param>
        /// <param name="value">字符串内容</param>
        public void element_Container_SetValue(XHud_Module_Container container, string indicator, string value)
        {
            container.Con_ChangeItemValue(indicator, value);
        }
        /// <summary>
        /// 修改目标容器项内容
        /// </summary>
        /// <param name="container">目标容器</param>
        /// <param name="indicator">目标项</param>
        /// <param name="value">图片精灵内容</param>
        public void element_Container_SetValue(XHud_Module_Container container, string indicator, Sprite value)
        {
            container.Con_ChangeItemValue(indicator, value);
        }
        /// <summary>
        /// 修改目标容器项内容
        /// </summary>
        /// <param name="container">目标容器</param>
        /// <param name="indicator">目标项</param>
        /// <param name="value">图片内容</param>
        public void element_Container_SetValue(XHud_Module_Container container, string indicator, Texture2D value)
        {
            container.Con_ChangeItemValue(indicator, value);
        }
        /// <summary>
        /// 修改目标容器项内容
        /// </summary>
        /// <param name="container">目标容器</param>
        /// <param name="id">目标项ID</param>
        /// <param name="value">字符串内容</param>
        public void element_Container_SetValue(XHud_Module_Container container, int id, string value)
        {
            container.Con_ChangeItemValue(id, value);
        }
        /// <summary>
        /// 修改目标容器项内容
        /// </summary>
        /// <param name="container">目标容器</param>
        /// <param name="id">目标项ID</param>
        /// <param name="value">图片精灵内容</param>
        public void element_Container_SetValue(XHud_Module_Container container, int id, Sprite value)
        {
            container.Con_ChangeItemValue(id, value);
        }
        /// <summary>
        /// 修改目标容器项内容
        /// </summary>
        /// <param name="container">目标容器</param>
        /// <param name="id">目标项ID</param>
        /// <param name="value">图片内容</param>
        public void element_Container_SetValue(XHud_Module_Container container, int id, Texture2D value)
        {
            container.Con_ChangeItemValue(id, value);
        }
        /// <summary>
        /// 修改目标容器项内容
        /// </summary>
        /// <param name="container">目标容器</param>
        /// <param name="id">目标项ID</param>
        /// <param name="value">图片内容</param>
        public void element_Container_SetValue_RawImage(XHud_Module_Container container, int id, Texture2D value)
        {
            container.Con_ChangeItemValue_RawImage(id, value);
        }
        /// <summary>
        /// 修改目标容器项内容
        /// </summary>
        /// <param name="container">目标容器</param>
        /// <param name="id">目标项ID</param>
        /// <param name="value">图片内容</param>
        public void element_Container_SetValue_RawImage(XHud_Module_Container container, string indicator, Texture2D value)
        {
            container.Con_ChangeItemValue_RawImage(indicator, value);
        }
        #endregion
    }
}