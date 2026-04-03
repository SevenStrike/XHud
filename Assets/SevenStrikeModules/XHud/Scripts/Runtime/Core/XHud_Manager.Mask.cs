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
    using UnityEngine.UI;

    public partial class XHud_Manager : MonoBehaviour
    {
        [Tooltip("屏幕遮罩")]
        /// <summary>
        /// 屏幕遮罩
        /// </summary>
        public Image Mask;
        [Range(0, 1)]
        [Tooltip("遮罩透明度")]
        /// <summary>
        /// 遮罩透明度
        /// </summary>
        public float MaskAlpha;
        [Range(0, 1)]
        [Tooltip("当遮罩透明度大于此值时遮罩的射线点击特性(Raycast)处于启用")]
        /// <summary>
        /// 当遮罩透明度大于此值时遮罩的射线点击特性(Raycast)处于启用
        /// </summary>
        public float MaskRaycastAlphaThreshold = 1;
        [Tooltip("是否开启遮罩射线遮挡")]
        /// <summary>
        /// 是否开启遮罩射线遮挡
        /// </summary>
        public bool MaskRaycastEnabled;
        [Tooltip("遮罩颜色")]
        /// <summary>
        /// 遮罩颜色
        /// </summary>
        public Color MaskColor = Color.clear;
        [Tooltip("遮罩贴图")]
        /// <summary>
        /// 遮罩贴图
        /// </summary>
        public Texture2D MaskTexture;
        [Tooltip("动画组件 - 遮罩透明度")]
        /// <summary>
        /// 动画组件 - 遮罩透明度
        /// </summary>
        private XTween_Interface twn_MaskAlpha;
        [Tooltip("动画组件 - 遮罩颜色通道 R")]
        /// <summary>
        /// 动画组件 - 遮罩颜色通道 R
        /// </summary>
        private XTween_Interface twn_MaskColor_R;
        [Tooltip("动画组件 - 遮罩颜色通道 G")]
        /// <summary>
        /// 动画组件 - 遮罩颜色通道 G
        /// </summary>
        private XTween_Interface twn_MaskColor_G;
        [Tooltip("动画组件 - 遮罩颜色通道 B")]
        /// <summary>
        /// 动画组件 - 遮罩颜色通道 B
        /// </summary>
        private XTween_Interface twn_MaskColor_B;

        /// <summary>
        /// 屏幕遮罩更新
        /// 负责同步屏幕遮罩（Mask）的视觉状态和射线检测属性
        /// 
        /// 工作原理：
        /// 1. 同步遮罩的颜色和透明度（MaskColor + MaskAlpha）
        /// 2. 设置遮罩的贴图（如果有配置）
        /// 3. 根据透明度阈值动态控制遮罩的射线检测（raycastTarget）开关
        /// 4. 确保遮罩始终位于 UI 层级的最上层
        /// 
        /// 为什么需要屏幕遮罩？
        /// - 实现 UI 弹窗时的半透明背景遮罩（如对话框、全屏菜单）
        /// - 创建沉浸式的 UI 交互体验，引导用户注意力
        /// - 阻止用户在弹窗打开时与背景 UI 交互
        /// - 配合动画实现淡入淡出的过渡效果
        /// 
        /// 与散焦遮罩（BlurMask）的区别：
        /// - Mask：纯色半透明遮罩，用于阻挡点击和视觉遮挡
        /// - BlurMask：配合 URP 散焦特性，实现背景模糊效果
        /// 
        /// 射线检测阈值逻辑：
        /// - 当遮罩透明度 < MaskRaycastAlphaThreshold 时，遮罩不阻挡射线
        ///   -> 用户可点击背后的 UI，适合轻提示场景
        /// - 当遮罩透明度 >= 阈值时，遮罩阻挡射线
        ///   -> 防止用户点击背后的 UI，适合需要强制交互的场景
        /// 
        /// 使用场景：
        /// - 弹窗/对话框打开时的背景遮罩
        /// - 全屏菜单的过渡效果
        /// - 游戏暂停界面的半透明背景
        /// - 引导教程的高亮遮罩（配合镂空效果）
        /// 
        /// 调用频率：
        /// - 每帧在 XHud_Manager.Update() 中调用
        /// - 开销极小，仅做属性同步和阈值判断
        /// 
        /// 注意事项：
        /// - 如果 Mask 组件不存在，方法直接返回
        /// - 透明度阈值（MaskRaycastAlphaThreshold）通常设置为 0.1-0.3
        /// - 遮罩的 alpha 值通常为 0（透明）到 0.8（半透明）之间
        /// - 确保遮罩始终在顶层，才能有效阻挡点击
        /// </summary>
        public void hm_MaskUpdate()
        {
            if (Mask == null)
                return;

            MaskColor.a = MaskAlpha;
            if (Mask != null)
                Mask.color = MaskColor;
            if (MaskTexture != null)
                hm_MaskTextureSet(MaskTexture);
            else
                hm_MaskTextureSet(null);

            if (MaskAlpha < MaskRaycastAlphaThreshold)
            {
                MaskRaycastEnabled = false;
            }
            else
            {
                MaskRaycastEnabled = true;
            }
            Mask.raycastTarget = MaskRaycastEnabled;
            hm_MaskTopView();

            if (Act_MaskChanged_Value != null)
                Act_MaskChanged_Value(MaskAlpha);
        }
        /// <summary>
        /// 将Mask置于最上层
        /// </summary>
        public void hm_MaskTopView()
        {
            if (Mask == null)
                return;
            Mask.transform.SetAsLastSibling();
        }
        /// <summary>
        /// 遮罩颜色平滑到
        /// </summary>
        /// <param name="col">目标颜色</param>
        /// <param name="dur">耗时</param>
        /// <param name="ease">缓动</param>
        /// <param name="delay">延迟</param>
        public void hm_MaskColor_To(Color col, float dur, EaseMode ease, float delay)
        {
            if (twn_MaskColor_R != null && twn_MaskColor_R.IsActive)
                if (twn_MaskColor_R.IsPlaying)
                    twn_MaskColor_R.Kill();
            if (twn_MaskColor_G != null && twn_MaskColor_G.IsActive)
                if (twn_MaskColor_G.IsPlaying)
                    twn_MaskColor_G.Kill();
            if (twn_MaskColor_B != null && twn_MaskColor_B.IsActive)
                if (twn_MaskColor_B.IsPlaying)
                    twn_MaskColor_B.Kill();

            twn_MaskColor_R = XTween.To(() => MaskColor.r, r => MaskColor.r = r, col.r, dur).SetEase(ease).SetAutoKill(true).SetDelay(delay);
            twn_MaskColor_G = XTween.To(() => MaskColor.g, g => MaskColor.g = g, col.g, dur).SetEase(ease).SetAutoKill(true).SetDelay(delay);
            twn_MaskColor_B = XTween.To(() => MaskColor.b, b => MaskColor.b = b, col.b, dur).SetEase(ease).SetAutoKill(true).SetDelay(delay);
        }
        /// <summary>
        /// 遮罩颜色平滑到
        /// </summary>
        /// <param name="col">目标颜色</param>
        /// <param name="dur">耗时</param>
        /// <param name="ease">曲线</param>
        /// <param name="delay">延迟</param>
        public void hm_MaskColor_To(Color col, float dur, AnimationCurve ease, float delay)
        {
            if (twn_MaskColor_R != null && twn_MaskColor_R.IsActive)
                if (twn_MaskColor_R.IsPlaying)
                    twn_MaskColor_R.Kill();
            if (twn_MaskColor_G != null && twn_MaskColor_G.IsActive)
                if (twn_MaskColor_G.IsPlaying)
                    twn_MaskColor_G.Kill();
            if (twn_MaskColor_B != null && twn_MaskColor_B.IsActive)
                if (twn_MaskColor_B.IsPlaying)
                    twn_MaskColor_B.Kill();

            twn_MaskColor_R = XTween.To(() => MaskColor.r, r => MaskColor.r = r, col.r, dur).SetEase(ease).SetAutoKill(true).SetDelay(delay);
            twn_MaskColor_G = XTween.To(() => MaskColor.g, g => MaskColor.g = g, col.g, dur).SetEase(ease).SetAutoKill(true).SetDelay(delay);
            twn_MaskColor_B = XTween.To(() => MaskColor.b, b => MaskColor.b = b, col.b, dur).SetEase(ease).SetAutoKill(true).SetDelay(delay);
        }
        /// <summary>
        /// 遮罩颜色快速到
        /// </summary>
        /// <param name="col">目标颜色</param>
        public void hm_MaskColor_FastTo(Color col)
        {
            if (twn_MaskColor_R != null && twn_MaskColor_R.IsActive)
                if (twn_MaskColor_R.IsPlaying)
                    twn_MaskColor_R.Kill();

            if (twn_MaskColor_G != null && twn_MaskColor_G.IsActive)
                if (twn_MaskColor_G.IsPlaying)
                    twn_MaskColor_G.Kill();

            if (twn_MaskColor_B != null && twn_MaskColor_B.IsActive)
                if (twn_MaskColor_B.IsPlaying)
                    twn_MaskColor_B.Kill();

            MaskColor.r = col.r;
            MaskColor.g = col.g;
            MaskColor.b = col.b;
        }
        /// <summary>
        /// 遮罩透明度平滑到
        /// </summary>
        /// <param name="val">目标颜色</param>
        /// <param name="dur">耗时</param>
        /// <param name="ease">缓动</param>
        /// <param name="delay">延迟</param>
        /// <param name="action_start">委托 - 开始时</param>
        /// <param name="action_end">委托 - 结束时</param>
        public void hm_MaskAlpha_To(float val, float dur, EaseMode ease, float delay, UnityAction action_start = null, UnityAction action_end = null)
        {
            if (twn_MaskAlpha != null && twn_MaskAlpha.IsActive)
                if (twn_MaskAlpha.IsPlaying)
                    twn_MaskAlpha.Kill();
            twn_MaskAlpha = XTween.To(() => MaskAlpha, x => MaskAlpha = x, val, dur).SetEase(ease).SetAutoKill(true).SetDelay(delay).OnStart(() =>
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
        /// 遮罩透明度平滑到
        /// </summary>
        /// <param name="val">目标颜色</param>
        /// <param name="dur">耗时</param>
        /// <param name="ease">曲线</param>
        /// <param name="delay">延迟</param>
        /// <param name="action_start">委托 - 开始时</param>
        /// <param name="action_end">委托 - 结束时</param>
        public void hm_MaskAlpha_To(float val, float dur, AnimationCurve ease, float delay, UnityAction action_start = null, UnityAction action_end = null)
        {
            if (twn_MaskAlpha != null && twn_MaskAlpha.IsActive)
                if (twn_MaskAlpha.IsPlaying)
                    twn_MaskAlpha.Kill();
            twn_MaskAlpha = XTween.To(() => MaskAlpha, x => MaskAlpha = x, val, dur).SetEase(ease).SetAutoKill(true).SetDelay(delay).OnStart(() =>
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
        /// 遮罩透明度快速到
        /// </summary>
        /// <param name="val">目标透明度</param>
        public void hm_MaskAlpha_FastTo(float val)
        {
            if (twn_MaskAlpha != null && twn_MaskAlpha.IsActive)
                if (twn_MaskAlpha.IsPlaying)
                    twn_MaskAlpha.Kill();
            MaskAlpha = val;
        }
        /// <summary>
        /// 设置遮罩贴图
        /// </summary>
        /// <param name="tex">目标遮罩贴图</param>
        public void hm_MaskTextureSet(Texture2D tex)
        {
            if (tex == null)
            {
                if (Mask != null)
                    Mask.sprite = null;
                return;
            }
            Mask.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);
            if (Act_MaskChanged_Texture != null)
                Act_MaskChanged_Texture(tex);
        }
    }
}