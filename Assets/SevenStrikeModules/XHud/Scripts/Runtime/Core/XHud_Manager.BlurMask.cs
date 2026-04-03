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
    using Unified.UniversalBlur.Runtime;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.Rendering.Universal;
    using UnityEngine.Rendering;
    using UnityEngine.UI;

    public partial class XHud_Manager : MonoBehaviour
    {
        [Tooltip("散焦遮罩")]
        /// <summary>
        /// 散焦遮罩
        /// </summary>
        public Image BlurMask;
        [Range(0, 1)]
        [Tooltip("散焦遮罩透明度")]
        /// <summary>
        /// 遮罩透明度
        /// </summary>
        public float BlurMaskAlpha;
        [Range(0, 1)]
        [Tooltip("当散焦遮罩透明度大于此值时散焦遮罩的射线点击特性(Raycast)处于启用")]
        /// <summary>
        /// 当散焦遮罩透明度大于此值时散焦遮罩的射线点击特性(Raycast)处于启用
        /// </summary>
        public float BlurMaskRaycastAlphaThreshold = 1;
        [Tooltip("是否开启散焦遮罩射线遮挡")]
        /// <summary>
        /// 是否开启散焦遮罩射线遮挡
        /// </summary>
        public bool BlurMaskRaycastEnabled;
        [Tooltip("散焦遮罩颜色")]
        /// <summary>
        /// 散焦遮罩颜色
        /// </summary>
        public Color BlurMaskColor = Color.white;
        [Tooltip("散焦遮罩贴图")]
        /// <summary>
        /// 散焦遮罩贴图
        /// </summary>
        public Texture2D BlurMaskTexture;
        [Tooltip("动画组件 - 散焦遮罩透明度")]
        /// <summary>
        /// 动画组件 - 散焦遮罩透明度
        /// </summary>
        private XTween_Interface twn_BlurMaskAlpha;
        [Tooltip("动画组件 - 散焦遮罩颜色通道 R")]
        /// <summary>
        /// 动画组件 - 散焦遮罩颜色通道 R
        /// </summary>
        private XTween_Interface twn_BlurMaskColor_R;
        [Tooltip("动画组件 - 散焦遮罩颜色通道 G")]
        /// <summary>
        /// 动画组件 - 散焦遮罩颜色通道 G
        /// </summary>
        private XTween_Interface twn_BlurMaskColor_G;
        [Tooltip("动画组件 - 散焦遮罩颜色通道 B")]
        /// <summary>
        /// 动画组件 - 散焦遮罩颜色通道 B
        /// </summary>
        private XTween_Interface twn_BlurMaskColor_B;

        #region UniversalFeature
        [Range(0, 1)]
        public float UniversalFeature_Blur_Intensity;

        [SerializeField]
        /// <summary>
        /// 特性 - 散焦模糊
        /// </summary>
        public UniversalBlurFeature UniversalFeature_Blur { get; set; }
        [Tooltip("渲染特性 - 散焦模糊效果")]
        /// <summary>
        /// 特性 - 散焦模糊 - 动画
        /// </summary>
        private XTween_Interface twn_UniversalFeature_Blur;
        /// <summary>
        /// 特性 - 散焦模糊强度到达最大
        /// </summary>
        private bool FeatureBlur_IntensityToMax;
        /// <summary>
        /// 特性 - 散焦模糊强度到达最小
        /// </summary>
        private bool FeatureBlur_IntensityToMin;
        #endregion

        #region BlurMask
        /// <summary>
        /// 散焦遮罩更新
        /// 负责同步散焦遮罩（BlurMask）的视觉状态和射线检测属性
        /// 
        /// 工作原理：
        /// 1. 同步散焦遮罩的颜色和透明度（BlurMaskColor + BlurMaskAlpha）
        /// 2. 设置散焦遮罩的贴图（如果有配置）
        /// 3. 根据透明度阈值动态控制遮罩的射线检测（raycastTarget）开关
        /// 4. 确保散焦遮罩始终位于 UI 层级的合适位置
        /// 
        /// 为什么需要散焦遮罩？
        /// - 实现 UI 弹出时的背景模糊效果
        /// - 创建沉浸式的 UI 交互体验（如弹窗打开时背景模糊）
        /// - 通过半透明遮罩配合 URP 的散焦特性，实现景深效果
        /// 
        /// 射线检测阈值逻辑：
        /// - 当遮罩透明度 < BlurMaskRaycastAlphaThreshold 时，遮罩不阻挡射线
        ///   -> UI 元素可被点击，适合轻微模糊效果
        /// - 当遮罩透明度 >= 阈值时，遮罩阻挡射线
        ///   -> 防止用户点击背后的 UI，适合需要强制交互的场景
        /// 
        /// 使用场景：
        /// - 弹窗打开时，背景模糊并禁止点击背景
        /// - 全屏菜单的过渡效果
        /// - 游戏暂停界面的景深效果
        /// 
        /// 调用频率：
        /// - 每帧在 XHud_Manager.Update() 中调用
        /// - 开销极小，仅做属性同步和阈值判断
        /// 
        /// 注意事项：
        /// - 如果 BlurMask 组件不存在，方法直接返回
        /// - 透明度阈值（BlurMaskRaycastAlphaThreshold）通常在 0.5-0.8 之间
        /// - 散焦效果需要配合 URP 的 UniversalBlurFeature 使用
        /// </summary>
        public void hm_BlurMaskUpdate()
        {
            if (BlurMask == null)
                return;

            BlurMaskColor.a = BlurMaskAlpha;
            if (BlurMask != null)
                BlurMask.color = BlurMaskColor;
            if (BlurMaskTexture != null)
                hm_BlurMaskTextureSet(BlurMaskTexture);
            else
                hm_BlurMaskTextureSet(null);

            if (UniversalFeature_Blur_Intensity < BlurMaskRaycastAlphaThreshold)
            {
                BlurMaskRaycastEnabled = false;
            }
            else
            {
                BlurMaskRaycastEnabled = true;
            }
            BlurMask.raycastTarget = BlurMaskRaycastEnabled;
            hm_BlurMaskTopView();

            if (Act_BlurMaskChanged_Value != null)
                Act_BlurMaskChanged_Value(BlurMaskAlpha);
        }
        /// <summary>
        /// 将Mask置于最上层
        /// </summary>
        public void hm_BlurMaskTopView()
        {
            if (BlurMask == null)
                return;
            BlurMask.transform.SetAsFirstSibling();
        }
        /// <summary>
        /// 散焦遮罩颜色平滑到
        /// </summary>
        /// <param name="col">目标颜色</param>
        /// <param name="dur">耗时</param>
        /// <param name="ease">缓动</param>
        /// <param name="delay">延迟</param>
        public void hm_BlurMaskColor_To(Color col, float dur, EaseMode ease, float delay)
        {
            if (twn_BlurMaskColor_R != null && twn_BlurMaskColor_R.IsActive)
                if (twn_BlurMaskColor_R.IsPlaying)
                    twn_BlurMaskColor_R.Kill();
            if (twn_BlurMaskColor_G != null && twn_BlurMaskColor_G.IsActive)
                if (twn_BlurMaskColor_G.IsPlaying)
                    twn_BlurMaskColor_G.Kill();
            if (twn_BlurMaskColor_B != null && twn_BlurMaskColor_B.IsActive)
                if (twn_BlurMaskColor_B.IsPlaying)
                    twn_BlurMaskColor_B.Kill();

            twn_BlurMaskColor_R = XTween.To(() => BlurMaskColor.r, r => BlurMaskColor.r = r, col.r, dur).SetEase(ease).SetAutoKill(true).SetDelay(delay);
            twn_BlurMaskColor_G = XTween.To(() => BlurMaskColor.g, g => BlurMaskColor.g = g, col.g, dur).SetEase(ease).SetAutoKill(true).SetDelay(delay);
            twn_BlurMaskColor_B = XTween.To(() => BlurMaskColor.b, b => BlurMaskColor.b = b, col.b, dur).SetEase(ease).SetAutoKill(true).SetDelay(delay);
        }
        /// <summary>
        /// 散焦遮罩颜色平滑到
        /// </summary>
        /// <param name="col">目标颜色</param>
        /// <param name="dur">耗时</param>
        /// <param name="ease">曲线</param>
        /// <param name="delay">延迟</param>
        public void hm_BlurMaskColor_To(Color col, float dur, AnimationCurve ease, float delay)
        {
            if (twn_BlurMaskColor_R != null && twn_BlurMaskColor_R.IsActive)
                if (twn_BlurMaskColor_R.IsPlaying)
                    twn_BlurMaskColor_R.Kill();
            if (twn_BlurMaskColor_G != null && twn_BlurMaskColor_G.IsActive)
                if (twn_BlurMaskColor_G.IsPlaying)
                    twn_BlurMaskColor_G.Kill();
            if (twn_BlurMaskColor_B != null && twn_BlurMaskColor_B.IsActive)
                if (twn_BlurMaskColor_B.IsPlaying)
                    twn_BlurMaskColor_B.Kill();

            twn_BlurMaskColor_R = XTween.To(() => BlurMaskColor.r, r => BlurMaskColor.r = r, col.r, dur).SetEase(ease).SetAutoKill(true).SetDelay(delay);
            twn_BlurMaskColor_G = XTween.To(() => BlurMaskColor.g, g => BlurMaskColor.g = g, col.g, dur).SetEase(ease).SetAutoKill(true).SetDelay(delay);
            twn_BlurMaskColor_B = XTween.To(() => BlurMaskColor.b, b => BlurMaskColor.b = b, col.b, dur).SetEase(ease).SetAutoKill(true).SetDelay(delay);
        }
        /// <summary>
        /// 散焦遮罩颜色快速到
        /// </summary>
        /// <param name="col">目标颜色</param>
        public void hm_BlurMaskColor_FastTo(Color col)
        {
            if (twn_BlurMaskColor_R != null && twn_BlurMaskColor_R.IsActive)
                if (twn_BlurMaskColor_R.IsPlaying)
                    twn_BlurMaskColor_R.Kill();

            if (twn_BlurMaskColor_G != null && twn_BlurMaskColor_G.IsActive)
                if (twn_BlurMaskColor_G.IsPlaying)
                    twn_BlurMaskColor_G.Kill();

            if (twn_BlurMaskColor_B != null && twn_BlurMaskColor_B.IsActive)
                if (twn_BlurMaskColor_B.IsPlaying)
                    twn_BlurMaskColor_B.Kill();

            BlurMaskColor.r = col.r;
            BlurMaskColor.g = col.g;
            BlurMaskColor.b = col.b;
        }
        /// <summary>
        /// 散焦遮罩透明度平滑到
        /// </summary>
        /// <param name="val">目标颜色</param>
        /// <param name="dur">耗时</param>
        /// <param name="ease">缓动</param>
        /// <param name="delay">延迟</param>
        /// <param name="action_start">委托 - 开始时</param>
        /// <param name="action_end">委托 - 结束时</param>
        public void hm_BlurMaskAlpha_To(float val, float dur, EaseMode ease, float delay, UnityAction action_start = null, UnityAction action_end = null)
        {
            if (twn_BlurMaskAlpha != null && twn_BlurMaskAlpha.IsActive)
                if (twn_BlurMaskAlpha.IsPlaying)
                    twn_BlurMaskAlpha.Kill();
            twn_BlurMaskAlpha = XTween.To(() => BlurMaskAlpha, x => BlurMaskAlpha = x, val, dur).SetEase(ease).SetAutoKill(true).SetDelay(delay).OnStart(() =>
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
        /// 散焦遮罩透明度平滑到
        /// </summary>
        /// <param name="val">目标颜色</param>
        /// <param name="dur">耗时</param>
        /// <param name="ease">曲线</param>
        /// <param name="delay">延迟</param>
        /// <param name="action_start">委托 - 开始时</param>
        /// <param name="action_end">委托 - 结束时</param>
        public void hm_BlurMaskAlpha_To(float val, float dur, AnimationCurve ease, float delay, UnityAction action_start = null, UnityAction action_end = null)
        {
            if (twn_BlurMaskAlpha != null && twn_BlurMaskAlpha.IsActive)
                if (twn_BlurMaskAlpha.IsPlaying)
                    twn_BlurMaskAlpha.Kill();
            twn_BlurMaskAlpha = XTween.To(() => BlurMaskAlpha, x => BlurMaskAlpha = x, val, dur).SetEase(ease).SetAutoKill(true).SetDelay(delay).OnStart(() =>
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
        /// 散焦遮罩透明度快速到
        /// </summary>
        /// <param name="val">目标透明度</param>
        public void hm_BlurMaskAlpha_FastTo(float val)
        {
            if (twn_BlurMaskAlpha != null && twn_BlurMaskAlpha.IsActive)
                if (twn_BlurMaskAlpha.IsPlaying)
                    twn_BlurMaskAlpha.Kill();
            BlurMaskAlpha = val;
        }
        /// <summary>
        /// 设置散焦遮罩贴图
        /// </summary>
        /// <param name="tex">目标散焦遮罩贴图</param>
        public void hm_BlurMaskTextureSet(Texture2D tex)
        {
            if (tex == null)
            {
                if (BlurMask != null)
                    BlurMask.sprite = null;
                return;
            }
            BlurMask.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);
            if (Act_BlurMaskChanged_Texture != null)
                Act_BlurMaskChanged_Texture(tex);
        }
        #endregion

        #region UniversalFeature
        /// <summary>
        /// 获取 URP 渲染管线中的散焦模糊特性（Universal Blur Feature）
        /// 用于实现 UI 打开时背景模糊（景深）效果
        /// 
        /// 工作原理：
        /// 1. 通过反射/查找方式获取 Universal Render Pipeline Asset 中的指定渲染特性
        /// 2. 将获取到的特性引用保存到 UniversalFeature_Blur 字段中
        /// 3. 同步当前模糊强度到 UniversalFeature_Blur_Intensity 字段
        /// 
        /// 使用场景：
        /// - UI 弹窗打开时，背景场景模糊，突出 UI 内容
        /// - 菜单切换时的过渡效果
        /// - 暂停界面、设置界面的景深效果
        /// 
        /// 技术依赖：
        /// - 项目必须使用 URP（Universal Render Pipeline）
        /// - 需要在 URP 渲染器特性中添加 UniversalBlurFeature
        /// - 需要 Unified.UniversalBlur.Runtime 命名空间支持
        /// 
        /// 注意事项：
        /// - 如果在非 URP 项目中使用，此方法不会报错但功能无效
        /// - 渲染特性名称必须与代码中查找的名称完全一致（"UniversalBlurFeature"）
        /// </summary>
        public void hm_UniversalFeature_Blur_Get()
        {
            if (UniversalFeature_Blur == null)
                UniversalFeature_Blur = GetFeature<UniversalBlurFeature>("UniversalBlurFeature");
            else
                UniversalFeature_Blur_Intensity = UniversalFeature_Blur.intensity;
        }
        /// <summary>
        /// 更新散焦模糊特性强度
        /// 将 Unity 渲染管线中的模糊强度与框架内部管理的强度值同步
        /// 
        /// 工作原理：
        /// 1. 检查 UniversalFeature_Blur 渲染特性是否存在
        /// 2. 将 UniversalFeature_Blur_Intensity 的值同步到实际渲染特性上
        /// 3. 检测强度值是否达到边界（0 或 1），触发相应的事件
        /// 
        /// 使用场景：
        /// - UI 弹窗打开时，背景模糊强度从 0 → 1
        /// - UI 弹窗关闭时，背景模糊强度从 1 → 0
        /// - 菜单切换时的过渡效果
        /// - 暂停界面的景深效果
        /// 
        /// 事件触发时机：
        /// - 强度达到最大值 1：触发 Act_BlurMask_Intensity_IsMax 事件
        /// - 强度达到最小值 0：触发 Act_BlurMask_Intensity_IsMin 事件
        /// 
        /// 性能特点：
        /// - 每帧执行，开销极小（仅一个赋值 + 边界判断）
        /// - 通过状态标志位避免重复触发事件
        /// </summary>
        public void hm_UniversalFeature_Blur_Update()
        {
            if (UniversalFeature_Blur == null)
                return;
            if (UniversalFeature_Blur_Intensity >= 1)
            {
                if (FeatureBlur_IntensityToMax)
                {
                    FeatureBlur_IntensityToMin = true;
                    FeatureBlur_IntensityToMax = false;
                    if (Act_BlurMask_Intensity_IsMax != null)
                        Act_BlurMask_Intensity_IsMax(UniversalFeature_Blur_Intensity);
                }
            }
            else if (UniversalFeature_Blur_Intensity <= 0)
            {
                if (FeatureBlur_IntensityToMin)
                {
                    FeatureBlur_IntensityToMin = false;
                    FeatureBlur_IntensityToMax = true;
                    if (Act_BlurMask_Intensity_IsMin != null)
                        Act_BlurMask_Intensity_IsMin(UniversalFeature_Blur_Intensity);
                }
            }
            UniversalFeature_Blur.intensity = UniversalFeature_Blur_Intensity;
        }
        /// <summary>
        /// 散焦特性效果强度平滑到
        /// </summary>
        /// <param name="val">目标强度</param>
        /// <param name="dur">耗时</param>
        /// <param name="ease">缓动</param>
        /// <param name="delay">延迟</param>
        /// <param name="action_start">委托 - 开始时</param>
        /// <param name="action_end">委托 - 结束时</param>
        public void hm_UniversalFeature_Blur_To(float val, float dur, EaseMode ease, float delay, UnityAction action_start = null, UnityAction action_end = null)
        {
            if (UniversalFeature_Blur == null)
                return;
            if (twn_UniversalFeature_Blur != null && twn_UniversalFeature_Blur.IsActive)
                if (twn_UniversalFeature_Blur.IsPlaying)
                    twn_UniversalFeature_Blur.Kill();
            twn_UniversalFeature_Blur = XTween.To(() => UniversalFeature_Blur_Intensity, x => UniversalFeature_Blur_Intensity = x, val, dur).SetEase(ease).SetAutoKill(true).SetDelay(delay).OnStart(() =>
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
        /// 散焦特性效果强度平滑到
        /// </summary>
        /// <param name="val">目标强度</param>
        /// <param name="dur">耗时</param>
        /// <param name="ease">缓动</param>
        /// <param name="delay">延迟</param>
        /// <param name="action_start">委托 - 开始时</param>
        /// <param name="action_end">委托 - 结束时</param>
        public void hm_UniversalFeature_Blur_To(float val, float dur, AnimationCurve ease, float delay, UnityAction action_start = null, UnityAction action_end = null)
        {
            if (UniversalFeature_Blur == null)
                return;
            if (twn_UniversalFeature_Blur != null && twn_UniversalFeature_Blur.IsActive)
                if (twn_UniversalFeature_Blur.IsPlaying)
                    twn_UniversalFeature_Blur.Kill();
            twn_UniversalFeature_Blur = XTween.To(() => UniversalFeature_Blur_Intensity, x => UniversalFeature_Blur_Intensity = x, val, dur).SetEase(ease).SetAutoKill(true).SetDelay(delay).OnStart(() =>
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
        /// 散焦特性效果强度快速到
        /// </summary>
        /// <param name="val">目标强度</param>
        public void hm_UniversalFeature_Blur_FastTo(float val)
        {
            if (UniversalFeature_Blur == null)
                return;
            if (twn_UniversalFeature_Blur != null && twn_UniversalFeature_Blur.IsActive)
                if (twn_UniversalFeature_Blur.IsPlaying)
                    twn_UniversalFeature_Blur.Kill();
            UniversalFeature_Blur_Intensity = val;
        }
        /// <summary>
        /// 散焦特性效果强度快速到 - Editor用
        /// </summary>
        /// <param name="val">目标强度</param>
        public void hm_UniversalFeature_Blur_FastTo_ForEditor(float val)
        {
            if (UniversalFeature_Blur == null)
                UniversalFeature_Blur = GetFeature<UniversalBlurFeature>("UniversalBlurFeature");
            else
                UniversalFeature_Blur.intensity = val;
        }
        /// <summary>
        /// 获取渲染特性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T GetFeature<T>(string name) where T : ScriptableRendererFeature
        {
            UniversalRenderPipelineAsset urpAsset = GraphicsSettings.defaultRenderPipeline as UniversalRenderPipelineAsset;
            foreach (var rendererData in urpAsset.rendererDataList)
            {
                foreach (var feature in rendererData.rendererFeatures)
                {
                    if (feature is T && feature.name == name)
                    {
                        return (T)feature;
                    }
                }
            }

            return null;
        }
        #endregion
    }
}