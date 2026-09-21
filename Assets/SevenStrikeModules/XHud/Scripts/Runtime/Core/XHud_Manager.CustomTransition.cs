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
    using UnityEngine;

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