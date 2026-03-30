namespace SevenStrikeModules.XHud
{
    using UnityEngine;
    using UnityEngine.Events;

    public partial class XHud_Manager : MonoBehaviour
    {
        [Tooltip("转场器组件")]
        /// <summary>
        /// 转场器组件
        /// </summary>
        public XHud_TransitionController Hud_TransitionController;
        /// <summary>
        /// 使用转场库
        /// </summary>
        public bool CustomTransition;

        /// <summary>
        /// 转场控制器更新
        /// 负责将转场控制器组件置于 UI 层级的最上层，确保转场效果覆盖所有 UI 元素
        /// 
        /// 工作原理：
        /// 1. 检查转场控制器（Hud_TransitionController）是否存在
        /// 2. 如果存在，将其 Transform 设置为当前对象的最后一个子物体
        /// 3. 通过 SetAsLastSibling() 确保转场效果渲染在所有 UI 元素之上
        /// 
        /// 为什么需要转场控制器置顶？
        /// - 转场效果（如屏幕切换动画）需要覆盖整个 UI 界面
        /// - 必须确保转场蒙版或特效不被其他 UI 元素遮挡
        /// - 通过调整渲染顺序，实现完整的屏幕过渡效果
        /// 
        /// 使用场景：
        /// - 场景切换时的转场动画（淡入淡出、滑动、扭曲等）
        /// - 游戏开始/结束的过渡效果
        /// - 弹出全屏蒙版时的转场效果
        /// 
        /// 调用频率：
        /// - 每帧在 XHud_Manager.Update() 中调用
        /// - 开销极小，仅做层级检查和设置
        /// 
        /// 注意事项：/// - 如果转场控制器组件不存在，方法直接返回，不进行任何操作
        /// - 此方法确保转场效果始终在最上层，避免被 UI 元素遮挡
        /// </summary>
        public void hm_TransitionUpdate()
        {
            if (Hud_TransitionController == null)
                return;
            hm_TransitionTopView();
        }
        /// <summary>
        /// 将Transition置于最上层
        /// </summary>
        public void hm_TransitionTopView()
        {
            if (Hud_TransitionController == null)
                return;
            Hud_TransitionController.transform.SetAsLastSibling();
        }
    }
}