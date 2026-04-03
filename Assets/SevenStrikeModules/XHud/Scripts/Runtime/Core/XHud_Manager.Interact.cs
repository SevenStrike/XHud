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
    using System.Collections;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public partial class XHud_Manager : MonoBehaviour
    {
        [SerializeField]
        /// <summary>
        /// 是否正在与UI交互
        /// </summary>
        public bool IsInteractionUI;
        [SerializeField]
        /// <summary>
        /// UI 交互有效状态标志位
        /// 用于记录上一次检测时 UI 是否正在被交互（鼠标/触摸悬停或点击）
        /// 配合 Interacte_Invalid 实现状态变化的边缘检测
        /// 
        /// 状态变化逻辑：
        /// - 当 IsInteractionUI 从 false 变为 true 时，Interacte_Valid 变为 true
        /// - 此时会触发 Act_IsInteractionUI 事件，通知外部 UI 交互开始
        /// 
        /// 使用场景：
        /// - 检测用户何时开始与 UI 交互（用于隐藏游戏内 UI、暂停游戏逻辑等）
        /// - 避免在每一帧都触发事件，只在状态变化时触发一次
        /// </summary>
        private bool Interacte_Valid;
        [SerializeField]
        /// <summary>
        /// UI 交互无效状态标志位
        /// 用于记录上一次检测时 UI 是否未被交互
        /// 配合 Interacte_Valid 实现状态变化的边缘检测
        /// 
        /// 状态变化逻辑：
        /// - 当 IsInteractionUI 从 true 变为 false 时，Interacte_Invalid 变为 true
        /// - 此时会触发 Act_IsInteractionUI 事件，通知外部 UI 交互结束
        /// 
        /// 使用场景：
        /// - 检测用户何时停止与 UI 交互（用于恢复游戏内 UI、恢复游戏逻辑等）
        /// - 确保事件只在状态切换时触发，避免重复调用
        /// </summary>
        private bool Interacte_Invalid;

        /// <summary>
        /// /// <summary>
        /// 检查 UI 交互状态的协程
        /// 持续检测鼠标/触摸是否悬停或点击在 UI 元素上
        /// 
        /// 工作原理：
        /// 1. 通过 EventSystem.current.IsPointerOverGameObject() 检测当前鼠标/触摸点是否在 UI 上
        /// 2. 使用边缘检测机制（Interacte_Valid/Interacte_Invalid）只在状态变化时触发事件
        /// 3. 每帧执行一次检测，确保交互状态的实时性
        /// 
        /// 使用场景：
        /// - 判断玩家是否正在与 UI 交互（点击按钮、拖拽滑块、滚动列表等）
        /// - 根据 UI 交互状态控制游戏逻辑（如暂停游戏、隐藏游戏内 HUD、停止角色移动等）
        /// - 优化性能：避免每帧触发事件，只在状态切换时通知外部系统
        /// 
        /// 性能特点：
        /// - 每帧执行，开销极小（仅调用一次 EventSystem 检测）
        /// - 通过状态标志位避免频繁触发 UnityEvent
        /// - 使用 while(true) + yield return null 实现持续监控
        /// </summary>
        /// <returns>协程迭代器</returns>
        /// </summary>
        /// <returns></returns>
        IEnumerator CheckUIInteraction()
        {
            while (true)
            {
                IsInteractionUI = EventSystem.current.IsPointerOverGameObject();

                if (IsInteractionUI)
                {
                    Interacte_Invalid = false;
                    if (!Interacte_Valid)
                    {
                        Interacte_Valid = true;
                        if (Act_IsInteractionUI != null)
                            Act_IsInteractionUI(IsInteractionUI);
                    }
                }
                else
                {
                    Interacte_Valid = false;
                    if (!Interacte_Invalid)
                    {
                        Interacte_Invalid = true;
                        if (Act_IsInteractionUI != null)
                            Act_IsInteractionUI(IsInteractionUI);
                    }
                }

                yield return null;
            }
        }
    }
}