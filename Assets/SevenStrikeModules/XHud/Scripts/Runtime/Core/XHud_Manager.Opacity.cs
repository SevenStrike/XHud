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
    using SevenStrikeModules.XTween;
    using UnityEngine;
    using UnityEngine.Events;

    public partial class XHud_Manager : MonoBehaviour
    {
        [Tooltip("屏幕内容透明度控制组件")]
        /// <summary>
        /// 屏幕内容透明度控制组件
        /// </summary>
        public CanvasGroup HudCanvasGroup_Screen;
        [Tooltip("世界内容透明度控制组件")]
        /// <summary>
        /// 世界内容透明度控制组件
        /// </summary>
        public CanvasGroup HudCanvasGroup_World;
        [Range(0, 1)]
        [Tooltip("屏幕内容透明度")]
        /// <summary>
        /// 屏幕内容透明度
        /// </summary>
        public float ContentOpacity_Screen = 1f;
        [Range(0, 1)]
        [Tooltip("世界内容透明度")]
        /// <summary>
        /// 世界内容透明度
        /// </summary>
        public float ContentOpacity_World = 1f;
        [Tooltip("世界内容透明度")]
        /// <summary>
        /// 世界内容透明度
        /// </summary>
        private XTween_Interface twn_ContentOpacity_Screen;
        [Tooltip("世界内容透明度")]
        /// <summary>
        /// 世界内容透明度
        /// </summary>
        private XTween_Interface twn_ContentOpacity_World;
        /// <summary>
        /// 中断开关 - 屏幕内容透明度到达最大
        /// </summary>
        private bool OpacityIsChangedToMax_Screen;
        /// <summary>
        /// 中断开关 - 屏幕内容透明度到达最小
        /// </summary>
        private bool OpacityIsChangedToMin_Screen;
        /// <summary>
        /// 中断开关 - 世界内容透明度到达最大
        /// </summary>
        private bool OpacityIsChangedToMax_World;
        /// <summary>
        /// 中断开关 - 世界内容透明度到达最小
        /// </summary>
        private bool OpacityIsChangedToMin_World;

        /// <summary>
        /// 内容透明度更新
        /// 同步屏幕空间和世界空间 UI 内容的整体透明度，并触发相应的状态变化事件
        /// 
        /// 工作原理：
        /// 1. 同步屏幕空间内容的透明度（HudCanvasGroup_Screen.alpha = ContentAlpha_Screen）
        /// 2. 同步世界空间内容的透明度（HudCanvasGroup_World.alpha = ContentAlpha_World）
        /// 3. 检测透明度是否达到边界值（0 或 1），触发相应事件
        /// 4. 检测透明度变化，触发变化事件
        /// 
        /// 为什么需要整体内容透明度控制？
        /// - 实现 UI 整体的淡入淡出效果（如场景切换、游戏暂停）
        /// - 创建沉浸式的 UI 过渡体验
        /// - 统一控制所有 UI 元素的可见性
        /// - 配合动画系统实现平滑的 UI 显隐
        /// 
        /// 边界事件触发时机：
        /// - 透明度达到 1：触发 Act_ContentOpacity_Screen_IsMax / Act_ContentOpacity_World_IsMax
        /// - 透明度达到 0：触发 Act_ContentOpacity_Screen_IsMin / Act_ContentOpacity_World_IsMin
        /// - 透明度在 0-1 之间：触发 Act_ContentOpacity_Screen_Changed / Act_ContentOpacity_World_Changed
        /// 
        /// 使用场景：
        /// - 场景切换时的 UI 淡入淡出
        /// - 游戏暂停界面的整体淡入效果
        /// - 全屏菜单打开/关闭的过渡动画
        /// - UI 整体隐藏/显示（如截图模式、相机模式切换）
        /// 
        /// 调用频率：
        /// - 每帧在 XHud_Manager.Update() 中调用
        /// - 开销极小，仅做属性同步和边界判断
        /// 
        /// 注意事项：
        /// - 屏幕空间和世界空间的透明度独立控制
        /// - 透明度通过 CanvasGroup 组件实现，不影响子物体的独立透明度
        /// - 需要确保 HudCanvasGroup_Screen/World 组件已正确配置
        /// </summary>
        public void hm_ContentOpacity_Update()
        {
            if (HudCanvasGroup_Screen != null)
            {
                HudCanvasGroup_Screen.alpha = ContentOpacity_Screen;
                if (ContentOpacity_Screen >= 1)
                {
                    if (OpacityIsChangedToMax_Screen)
                    {
                        OpacityIsChangedToMax_Screen = false;
                        if (Act_ContentOpacity_Screen_IsMax != null)
                            Act_ContentOpacity_Screen_IsMax(ContentOpacity_Screen);
                    }
                }
                else if (ContentOpacity_Screen <= 0)
                {
                    if (OpacityIsChangedToMin_Screen)
                    {
                        OpacityIsChangedToMin_Screen = false;
                        if (Act_ContentOpacity_Screen_IsMin != null)
                            Act_ContentOpacity_Screen_IsMin(ContentOpacity_Screen);
                    }
                }
                else
                {
                    OpacityIsChangedToMax_Screen = true;
                    OpacityIsChangedToMin_Screen = true;
                    if (Act_ContentOpacity_Screen_Changed != null)
                        Act_ContentOpacity_Screen_Changed(ContentOpacity_Screen);
                }
            }


            if (HudCanvasGroup_World != null)
            {
                HudCanvasGroup_World.alpha = ContentOpacity_World;
                if (ContentOpacity_World >= 1)
                {
                    if (OpacityIsChangedToMax_World)
                    {
                        OpacityIsChangedToMax_World = false;
                        if (Act_ContentOpacity_World_IsMax != null)
                            Act_ContentOpacity_World_IsMax(ContentOpacity_World);
                    }
                }
                else if (ContentOpacity_World <= 0)
                {
                    if (OpacityIsChangedToMin_World)
                    {
                        OpacityIsChangedToMin_World = false;
                        if (Act_ContentOpacity_World_IsMin != null)
                            Act_ContentOpacity_World_IsMin(ContentOpacity_World);
                    }
                }
                else
                {
                    OpacityIsChangedToMax_World = true;
                    OpacityIsChangedToMin_World = true;
                    if (Act_ContentOpacity_World_Changed != null)
                        Act_ContentOpacity_World_Changed(ContentOpacity_World);
                }
            }
        }
        /// <summary>
        /// 屏幕内容透明度平滑到
        /// </summary>
        /// <param name="val">目标透明度</param>
        /// <param name="dur">耗时</param>
        /// <param name="ease">缓动</param>
        /// <param name="delay">延迟</param>
        /// <param name="action_start">委托 - 开始时</param>
        /// <param name="action_end">委托 - 结束时</param>
        public void hm_Screen_ContentOpacity_To(float val, float dur, EaseMode ease, float delay, UnityAction action_start = null, UnityAction action_end = null)
        {
            if (twn_ContentOpacity_Screen != null && twn_ContentOpacity_Screen.IsActive)
                if (twn_ContentOpacity_Screen.IsPlaying)
                    twn_ContentOpacity_Screen.Kill();
            twn_ContentOpacity_Screen = XTween.To(() => ContentOpacity_Screen, x => ContentOpacity_Screen = x, val, dur).SetEase(ease).SetAutoKill(true).SetDelay(delay).OnStart(() =>
            {
                if (action_start != null)
                    action_start();
            }).OnComplete((d) =>
            {
                if (action_end != null)
                    action_end();
            });
        }
        /// <summary>
        /// 屏幕内容透明度平滑到
        /// </summary>
        /// <param name="val">目标透明度</param>
        /// <param name="dur">耗时</param>
        /// <param name="ease">缓动</param>
        /// <param name="delay">延迟</param>
        /// <param name="action_start">委托 - 开始时</param>
        /// <param name="action_end">委托 - 结束时</param>
        public void hm_Screen_ContentOpacity_To(float val, float dur, AnimationCurve ease, float delay, UnityAction action_start = null, UnityAction action_end = null)
        {
            if (twn_ContentOpacity_Screen != null && twn_ContentOpacity_Screen.IsActive)
                if (twn_ContentOpacity_Screen.IsPlaying)
                    twn_ContentOpacity_Screen.Kill();
            twn_ContentOpacity_Screen = XTween.To(() => ContentOpacity_Screen, x => ContentOpacity_Screen = x, val, dur).SetEase(ease).SetAutoKill(true).SetDelay(delay).OnStart(() =>
            {
                if (action_start != null)
                    action_start();
            }).OnComplete((d) =>
            {
                if (action_end != null)
                    action_end();
            });
        }
        /// <summary>
        /// 屏幕内容透明度快速到
        /// </summary>
        /// <param name="val">目标透明度</param>
        public void hm_Screen_ContentOpacity_FastTo(float val)
        {
            if (twn_ContentOpacity_Screen != null && twn_ContentOpacity_Screen.IsActive)
                if (twn_ContentOpacity_Screen.IsPlaying)
                    twn_ContentOpacity_Screen.Kill();
            ContentOpacity_Screen = val;
        }
        /// <summary>
        /// 世界内容透明度平滑到
        /// </summary>
        /// <param name="val">目标透明度</param>
        /// <param name="dur">耗时</param>
        /// <param name="ease">缓动</param>
        /// <param name="delay">延迟</param>
        /// <param name="action_start">委托 - 开始时</param>
        /// <param name="action_end">委托 - 结束时</param>
        public void hm_World_ContentOpacity_To(float val, float dur, EaseMode ease, float delay, UnityAction action_start = null, UnityAction action_end = null)
        {
            if (twn_ContentOpacity_World != null && twn_ContentOpacity_World.IsActive)
                if (twn_ContentOpacity_World.IsPlaying)
                    twn_ContentOpacity_World.Kill();
            twn_ContentOpacity_World = XTween.To(() => ContentOpacity_World, x => ContentOpacity_World = x, val, dur).SetEase(ease).SetAutoKill(true).SetDelay(delay).OnStart(() =>
            {
                if (action_start != null)
                    action_start();
            }).OnComplete((d) =>
            {
                if (action_end != null)
                    action_end();
            });
        }
        /// <summary>
        /// 世界内容透明度平滑到
        /// </summary>
        /// <param name="val">目标透明度</param>
        /// <param name="dur">耗时</param>
        /// <param name="ease">缓动</param>
        /// <param name="delay">延迟</param>
        /// <param name="action_start">委托 - 开始时</param>
        /// <param name="action_end">委托 - 结束时</param>
        public void hm_World_ContentOpacity_To(float val, float dur, AnimationCurve ease, float delay, UnityAction action_start = null, UnityAction action_end = null)
        {
            if (twn_ContentOpacity_World != null && twn_ContentOpacity_World.IsActive)
                if (twn_ContentOpacity_World.IsPlaying)
                    twn_ContentOpacity_World.Kill();
            twn_ContentOpacity_World = XTween.To(() => ContentOpacity_World, x => ContentOpacity_World = x, val, dur).SetEase(ease).SetAutoKill(true).SetDelay(delay).OnStart(() =>
            {
                if (action_start != null)
                    action_start();
            }).OnComplete((d) =>
            {
                if (action_end != null)
                    action_end();
            });
        }
        /// <summary>
        /// 世界内容透明度快速到
        /// </summary>
        /// <param name="val">目标透明度</param>
        public void hm_World_ContentOpacity_FastTo(float val)
        {
            if (twn_ContentOpacity_World != null && twn_ContentOpacity_World.IsActive)
                if (twn_ContentOpacity_World.IsPlaying)
                    twn_ContentOpacity_World.Kill();
            ContentOpacity_World = val;
        }
    }
}