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
    /// 图元控制器的结构类
    /// </summary>
    [System.Serializable]
    public class PrimitiveControllerNode
    {
        [SerializeField]
        /// <summary>
        /// 控制器
        /// </summary>
        public XHud_Module_Primitive_Controller Controller;
        [SerializeField]
        /// <summary>
        /// 动画器的动画总计时间
        /// </summary>
        public float TotalTime;
        [SerializeField]
        /// <summary>
        /// 动画器的动画延迟时间
        /// </summary>
        public float DelayTime;
    }

    public partial class XHud_Module_Element : MonoBehaviour
    {
        [SerializeField]
        /// <summary>
        /// 图元动画器容器
        /// </summary>
        public List<PrimitiveControllerNode> PrimitiveControllerNodes = new List<PrimitiveControllerNode>();
        [SerializeField]
        /// <summary>
        /// 图元列表折叠状态
        /// true表示折叠，false表示展开
        /// 用于控制Inspector中图元列表的显示/隐藏
        /// </summary>
        public bool PrimitivesIsFold;

        #region 获取图元控制器
        /// <summary>
        /// 获取一个图元控制器
        /// </summary>
        /// <param name="indicator">目标名称</param>
        /// <returns>返回一个匹配名称的图元控制器</returns>
        public XHud_Module_Primitive_Controller GetPrimitiveController_With_Indicator(string indicator)
        {
            XHud_Module_Primitive_Controller am = null;
            for (int i = 0; i < PrimitiveControllerNodes.Count; i++)
            {
                if (PrimitiveControllerNodes[i].Controller.GetIndicator() == indicator)
                {
                    am = PrimitiveControllerNodes[i].Controller;
                }
            }
            if (am == null)
            {
                if (DebugState)
                    XGUI_Utilitys.Console("XHud - 元素控件通知", "未获取到标识名为 " + indicator + " 的子级图元控制器！ ", XGUIMsgState.错误);
            }
            else
            {
                if (DebugState)
                    XGUI_Utilitys.Console("XHud - 元素控件通知", "已获取子级图元控制器 " + indicator, XGUIMsgState.通知);
            }
            return am;
        }
        /// <summary>
        /// 获取一个图元控制器
        /// </summary>
        /// <param name="name">目标物体名称</param>
        /// <returns>返回一个匹配物体名称名称的图元控制器</returns>
        public XHud_Module_Primitive_Controller GetPrimitiveController_With_GameObjectName(string name)
        {
            XHud_Module_Primitive_Controller am = null;
            for (int i = 0; i < PrimitiveControllerNodes.Count; i++)
            {
                if (PrimitiveControllerNodes[i].Controller.gameObject.name == name)
                {
                    am = PrimitiveControllerNodes[i].Controller;
                }
            }
            if (am == null)
            {
                if (DebugState)
                    XGUI_Utilitys.Console("XHud - 元素控件通知", "未获取到名为 " + name + " 的子级图元控制器！ ", XGUIMsgState.错误);
            }
            else
            {
                if (DebugState)
                    XGUI_Utilitys.Console("XHud - 元素控件通知", "已获取子级图元控制器 " + name, XGUIMsgState.通知);
            }
            return am;
        }
        /// <summary>
        /// Hud元素 - 获取一个图元控制器
        /// </summary>
        /// <param name="id">目标ID</param>
        /// <returns>返回一个匹配ID的图元控制器</returns>
        public XHud_Module_Primitive_Controller GetPrimitiveController_With_ID(string id)
        {
            XHud_Module_Primitive_Controller am = null;
            for (int i = 0; i < PrimitiveControllerNodes.Count; i++)
            {
                if (PrimitiveControllerNodes[i].Controller.GetID() == id)
                {
                    am = PrimitiveControllerNodes[i].Controller;
                }
            }
            if (am == null)
            {
                if (DebugState)
                    XGUI_Utilitys.Console("XHud - 元素控件通知", "未获取到 ID 号为 " + id + " 的子级图元控制器！ ", XGUIMsgState.错误);
            }
            else
            {
                if (DebugState)
                    XGUI_Utilitys.Console("XHud - 元素控件通知", "已获取 ID 号为 " + id + " 子级图元控制器！", XGUIMsgState.通知);
            }
            return am;
        }
        #endregion

    }
}