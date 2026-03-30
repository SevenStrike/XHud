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