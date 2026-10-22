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
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XTween;
    using UnityEngine;
    using UnityEngine.Events;

    public partial class XHud_Module_Element : MonoBehaviour
    {
        #region 动画状态字段
        [SerializeField]
        /// <summary>
        /// 动画状态
        /// 标识当前元素是否正在播放动画
        /// </summary>
        public bool Animating;
        [SerializeField]
        /// <summary>
        /// 元素动画预览状态 - In
        /// </summary>
        public bool TweensPreivew_In_State;
        [SerializeField]
        /// <summary>
        /// 元素动画预览状态 - Out
        /// </summary>
        public bool TweensPreivew_Out_State;
        [SerializeField]
        /// <summary>
        /// 自动停止预览开关
        /// true表示动画预览结束后自动停止
        /// false表示需要手动停止预览
        /// 仅用于编辑器预览模式，不影响运行时行为
        /// 默认值为true
        /// </summary>
        public bool AutoKillPreviewTweens = true;
        [SerializeField]
        /// <summary>
        /// 预览杀死前先重置
        /// </summary>
        public bool RewindPreviewTweensWithKill;
        [SerializeField]
        /// <summary>
        /// 杀死后清空预览器列表
        /// </summary>
        public bool ClearPreviewTweensWithKill;
        #endregion

        #region 元素自身基础动画节点
        [SerializeField]
        /// <summary>
        /// 动画模组 - 透明度_Alpha
        /// </summary>
        public XTween_Interface Tween_Alpha;
        [SerializeField]
        /// <summary>
        /// 动画模组 - 位移
        /// </summary>
        public XTween_Interface Tween_Move;
        [SerializeField]
        /// <summary>
        /// 动画模组 - 旋转_Rotation
        /// </summary>
        public XTween_Interface Tween_Rotation;
        #endregion

        #region 元素预览位置参数
        [SerializeField]
        /// <summary>
        /// 元素预览生成时设定的位置
        /// </summary>
        public Vector3 Crc_Preview_Position;
        [SerializeField]
        /// <summary>
        /// 元素预览回收时设定的位置
        /// </summary>
        public Vector3 Rec_Preview_Position;
        [SerializeField]
        /// <summary>
        /// 预览时是否包含子级所有图元动画？
        /// </summary>
        public bool PreviewIncludePrimitivesTween;
        [SerializeField]
        /// <summary>
        /// 预览生成时是否指定位置？
        /// </summary>
        public bool Crc_Preview_SetPosition;
        [SerializeField]
        /// <summary>
        /// 预览回收时是否指定位置
        /// </summary>
        public bool Rec_Preview_SetPosition;
        #endregion

        #region 元素动效库与动效参数
        [SerializeField]
        /// <summary>
        /// 元素生成动效库名称
        /// 用于从动效库中加载预设的生成动效参数
        /// 对应 Motion_Creator 类型的动效模板
        /// </summary>
        public string Crc_Lib_Name;
        [SerializeField]
        /// <summary>
        /// 元素回收动效库名称
        /// 用于从动效库中加载预设的回收动效参数
        /// 对应 Motion_Recycler 类型的动效模板
        /// </summary>
        public string Rec_Lib_Name;
        [SerializeField]
        /// <summary>
        /// 元素生成动效参数
        /// 定义元素进入/显示时的动画效果，包括位移、旋转、透明度等
        /// 支持缓动曲线、持续时间、延迟等参数配置
        /// </summary>
        public Motion_Creator CreateArgs = new Motion_Creator
        {
            Alpha = new MotionNode_Alpha
            {
                Duration = 1,
                Delay = 0,
                Curve = AnimationCurve.EaseInOut(0, 0, 1, 1),
                CurveName = "",
                Ease = EaseMode.InOutCubic
            },
            Movement = new MotionNode_Movement
            {
                Movement = HudMotion_Movement.S_从下至上,
                Distance = 100,
                Duration = 1,
                Delay = 0,
                Curve = AnimationCurve.EaseInOut(0, 0, 1, 1),
                CurveName = "",
                Ease = EaseMode.InOutCubic
            },
            Rotation = new MotionNode_Rotation
            {
                Rotation = HudMotion_Rotation.A_无旋转,
                Degree = 0,
                Duration = 1,
                Delay = 0,
                Curve = AnimationCurve.EaseInOut(0, 0, 1, 1),
                CurveName = "",
                Ease = EaseMode.InOutCubic
            }
        };
        [SerializeField]
        /// <summary>
        /// 元素回收动效参数
        /// 定义元素退出/隐藏时的动画效果，包括位移、旋转、透明度等
        /// 支持缓动曲线、持续时间、延迟等参数配置
        /// </summary>
        public Motion_Recycler RecycleArgs = new Motion_Recycler
        {
            Alpha = new MotionNode_Alpha
            {
                Duration = 1,
                Delay = 0,
                Curve = AnimationCurve.EaseInOut(0, 0, 1, 1),
                CurveName = "",
                Ease = EaseMode.InOutCubic
            },
            Movement = new MotionNode_Movement
            {
                Movement = HudMotion_Movement.D_从上至下,
                Distance = 100,
                Duration = 1,
                Delay = 0,
                Curve = AnimationCurve.EaseInOut(0, 0, 1, 1),
                CurveName = "",
                Ease = EaseMode.InOutCubic
            },
            Rotation = new MotionNode_Rotation
            {
                Rotation = HudMotion_Rotation.A_无旋转,
                Degree = 0,
                Duration = 1,
                Delay = 0,
                Curve = AnimationCurve.EaseInOut(0, 0, 1, 1),
                CurveName = "",
                Ease = EaseMode.InOutCubic
            }
        };
        [SerializeField]
        /// <summary>
        /// 动效参数折叠状态（编辑器使用）
        /// 控制 Inspector 面板中动效参数区域的展开/折叠状态
        /// </summary>
        public bool preview_motionArgs_fold;
        #endregion

        #region 动画播放与控制
        /// <summary>
        /// 播放已创建的元素三项基础动画（透明度、位移、旋转）
        /// </summary>
        public void ElementTweensPlay()
        {
            if (Tween_Alpha != null)
                Tween_Alpha.Play();
            if (Tween_Move != null)
                Tween_Move.Play();
            if (Tween_Rotation != null)
                Tween_Rotation.Play();
        }
        /// <summary>
        /// 杀死已创建的元素三项基础动画（透明度、位移、旋转）
        /// </summary>
        /// <param name="complete">是否以"完成"语义杀死（传递给底层 XTween 的 Kill）</param>
        public void ElementTweens_Kill(bool complete = true)
        {
            if (Tween_Alpha != null)
                Tween_Alpha.Kill(complete);
            if (Tween_Move != null)
                Tween_Move.Kill(complete);
            if (Tween_Rotation != null)
                Tween_Rotation.Kill(complete);
        }
        /// <summary>
        /// 清空已创建的元素三项基础动画（透明度、位移、旋转）的引用
        /// </summary>
        public void ElementTweens_Clear()
        {
            if (Tween_Alpha != null)
                Tween_Alpha = null;
            if (Tween_Move != null)
                Tween_Move = null;
            if (Tween_Rotation != null)
                Tween_Rotation = null;
        }
        /// <summary>
        /// 杀死已创建的元素子级下的所有图元动画
        /// </summary>
        /// <param name="complete">是否以"完成"语义杀死</param>
        public void PrimitiveTweens_Kill(bool complete = true)
        {
            for (int i = 0; i < PrimitiveControllerNodes.Count; i++)
            {
                PrimitiveControllerNode node = PrimitiveControllerNodes[i];
                if (node.Controller.pt_Tween == null)
                    continue;
                node.Controller.pt_Tween.Tween_KillAll(complete);
            }
        }
        /// <summary>
        /// 清空已创建的元素子级下的所有图元动画引用
        /// </summary>
        public void PrimitiveTweens_Clear()
        {
            for (int i = 0; i < PrimitiveControllerNodes.Count; i++)
            {
                PrimitiveControllerNode node = PrimitiveControllerNodes[i];
                if (node.Controller.pt_Tween == null)
                    continue;
                node.Controller.pt_Tween.Tween_CleanAll();
            }
        }
        #endregion

        #region 元素自身进入动画
        /// <summary>
        /// 元素动画 - 进入
        /// <para/>流程：杀死并清空旧动画 → 创建生成动画 → 播放 → 触发开始事件
        /// </summary>
        /// <param name="args">生成动效参数（null 时使用默认 CreateArgs）</param>
        /// <param name="act_InComplete">进入完成时的额外回调</param>
        public virtual void Element_In(Motion_Creator args = null, UnityAction act_InComplete = null)
        {
            if (DebugState)
                XGUI_Utilitys.Console("XHud - 元素控件通知", "杀死元素自身（透明度、位移、旋转）动画！", XGUIMsgState.警告);
            ElementTweens_Kill(true);
            ElementTweens_Clear();
            if (DebugState)
                XGUI_Utilitys.Console("XHud - 元素控件通知", "进入动画开始！", XGUIMsgState.通知);
            ElementTweens_Creator(args, act_InComplete);
            ElementTweensPlay();
            #region 动作 - 动画开始
            if (act_on_element_in_start != null)
                act_on_element_in_start(this);
            if (eve_on_element_in_start != null)
                eve_on_element_in_start.Invoke();
            Element_In_Start();
            #endregion
        }
        /// <summary>
        /// 元素动画 - 进入 - 前
        /// <para/>职责：切换动画状态、禁用像素对齐、联动播放图元入场动画
        /// </summary>
        public virtual void Element_In_Start()
        {
            if (DebugState)
                XGUI_Utilitys.Console("XHud - 元素控件通知", "元素进入前逻辑调用", XGUIMsgState.通知);
            AnimateState = XHudElementAnimateState.Animating;
            Animating = true;
            if (DebugState)
                XGUI_Utilitys.Console("XHud - 元素控件通知", "禁用Canvas像素对齐！", XGUIMsgState.警告);
            if (Application.isPlaying)
            {
                if (XHud_Manager.Instance.UseAutoPerfectPixel)
                    XHud_Manager.Instance.hm_UsePixelPerfect(false);
            }
            else
            {
                if (XHud_Dashboard.get_manager().UseAutoPerfectPixel)
                    XHud_Dashboard.get_manager().hm_UsePixelPerfect(false);
            }
            if (AutoPlayPrimitivesTween)
                PrimitiveTweens_Play("元素进入时");
        }
        /// <summary>
        /// 元素动画 - 进入 - 后
        /// <para/>职责：切回静态状态、恢复像素对齐、联动播放图元入场后动画
        /// </summary>
        public virtual void Element_In_End()
        {
            if (DebugState)
                XGUI_Utilitys.Console("XHud - 元素控件通知", "元素进入后逻辑调用", XGUIMsgState.通知);
            AnimateState = XHudElementAnimateState.Static;
            Animating = false;
            if (DebugState)
                XGUI_Utilitys.Console("XHud - 元素控件通知", "恢复Canvas像素对齐！", XGUIMsgState.警告);
            if (Application.isPlaying)
            {
                if (XHud_Manager.Instance.UseAutoPerfectPixel)
                    XHud_Manager.Instance.hm_UsePixelPerfect(true);
            }
            else
            {
                if (XHud_Dashboard.get_manager().UseAutoPerfectPixel)
                    XHud_Dashboard.get_manager().hm_UsePixelPerfect(true);
            }
            if (AutoPlayPrimitivesTween)
                PrimitiveTweens_Play("元素进入后");
        }
        #endregion

        #region 元素自身退出动画
        /// <summary>
        /// 元素动画 - 退出
        /// <para/>流程：杀死并清空旧动画 → 创建回收动画 → 播放 → 触发开始事件
        /// </summary>
        /// <param name="args">回收动效参数（null 时使用默认 RecycleArgs）</param>
        /// <param name="act_OutComplete">退出完成时的额外回调</param>
        public virtual void Element_Out(Motion_Recycler args = null, UnityAction act_OutComplete = null)
        {
            if (DebugState)
                XGUI_Utilitys.Console("XHud - 元素控件通知", "杀死元素自身（透明度、位移、旋转）动画！", XGUIMsgState.警告);
            ElementTweens_Kill(true);
            ElementTweens_Clear();
            if (DebugState)
                XGUI_Utilitys.Console("XHud - 元素控件通知", "退出动画开始！", XGUIMsgState.通知);
            ElementTweens_Recycler(args, act_OutComplete);
            ElementTweensPlay();
            #region 动作 - 动画开始
            if (act_on_element_out_start != null)
                act_on_element_out_start(this);
            if (eve_on_element_out_start != null)
                eve_on_element_out_start.Invoke();
            Element_Out_Start();
            #endregion
        }
        /// <summary>
        /// 元素动画 - 退出 - 前
        /// <para/>职责：切换动画状态、禁用像素对齐、联动播放图元退场动画
        /// </summary>
        public virtual void Element_Out_Start()
        {
            if (DebugState)
                XGUI_Utilitys.Console("XHud - 元素控件通知", "退出前逻辑调用", XGUIMsgState.通知);
            AnimateState = XHudElementAnimateState.Animating;
            if (DebugState)
                XGUI_Utilitys.Console("XHud - 元素控件通知", "禁用Canvas像素对齐！", XGUIMsgState.警告);
            if (Application.isPlaying)
            {
                if (XHud_Manager.Instance.UseAutoPerfectPixel)
                    XHud_Manager.Instance.hm_UsePixelPerfect(false);
            }
            else
            {
                if (XHud_Dashboard.get_manager().UseAutoPerfectPixel)
                    XHud_Dashboard.get_manager().hm_UsePixelPerfect(false);
            }
            Animating = true;
            if (AutoPlayPrimitivesTween)
                PrimitiveTweens_Play("元素退出时");
        }
        /// <summary>
        /// 元素动画 - 退出 - 后
        /// <para/>职责：切回静态状态、恢复像素对齐、移除跟踪器、清理布局记录、回收或销毁元素
        /// </summary>
        /// <param name="args">回收动效参数（透传给后续逻辑）</param>
        public virtual void Element_Out_End(Motion_Recycler args = null)
        {
            if (DebugState)
                XGUI_Utilitys.Console("XHud - 元素控件通知", "退出后逻辑调用", XGUIMsgState.通知);
            AnimateState = XHudElementAnimateState.Static;
            if (DebugState)
                XGUI_Utilitys.Console("XHud - 元素控件通知", "恢复Canvas像素对齐！", XGUIMsgState.警告);
            if (Application.isPlaying)
            {
                if (XHud_Manager.Instance.UseAutoPerfectPixel)
                    XHud_Manager.Instance.hm_UsePixelPerfect(true);
            }
            else
            {
                if (XHud_Dashboard.get_manager().UseAutoPerfectPixel)
                    XHud_Dashboard.get_manager().hm_UsePixelPerfect(true);
            }
            Animating = false;
            element_ObjectTracker_Remove();
            if (Application.isPlaying)
            {
                for (int i = 0; i < XHud_Manager.Instance.Anchors_Layout_Screen.Count; i++)
                {
                    for (int s = 0; s < XHud_Manager.Instance.Anchors_Layout_Screen[i].HudElementInfos.Count; s++)
                    {
                        if (this.ID == XHud_Manager.Instance.Anchors_Layout_Screen[i].HudElementInfos[s].ID)
                            XHud_Manager.Instance.Anchors_Layout_Screen[i].HudElementInfos.RemoveAt(s);
                    }
                }
            }
            if (DebugState)
                XGUI_Utilitys.Console("XHud - 元素控件通知", "回收到元素池！", XGUIMsgState.通知);
            if (CreateState == XHudElementCreateState.Created)
            {
                if (this.CreatedSourceType == XHudElementCreatedSourceType.Library)
                {
                    element_Reset(true);
                    XHud_Manager.Instance.hm_ElementLibrary_Despawn(this);
                }
                else
                {
                    Destroy(this.gameObject);
                }
            }
        }
        #endregion

        #region 辅助方法
        /// <summary>
        /// 统一 Ease/Curve 分支：Ease != None 走缓动模式，否则走自定义曲线。
        /// <para/>只做"配置"动作，无副作用，可安全链式合并。
        /// </summary>
        /// <param name="t">目标动画实例</param>
        /// <param name="ease">缓动模式（为 None 时改用 curve）</param>
        /// <param name="curve">自定义缓动曲线</param>
        /// <returns>配置完成的动画实例</returns>
        private XTween_Interface ApplyEase(XTween_Interface t, EaseMode ease, AnimationCurve curve)
        {
            return ease == EaseMode.None ? t.SetEase(curve) : t.SetEase(ease);
        }
        #endregion

        #region 剥离的元素独立动画单元 - 生成
        /// <summary>
        /// 剥离的独立动画单元：生成
        /// <para/>创建透明度、位移、旋转三类动画并挂载完整回调链。
        /// </para>
        /// </summary>
        /// <param name="args">生成动效参数</param>
        /// <param name="act_InComplete">完成时的额外回调</param>
        /// <param name="isPreview">是否为编辑器预览模式</param>
        /// <param name="PosSet">预览时是否预设位置</param>
        /// <param name="pos">预览预设位置</param>
        public void ElementTweens_Creator(Motion_Creator args, UnityAction act_InComplete, bool isPreview = false, bool PosSet = false, Vector3 pos = default)
        {
            float duration = isPreview ? XHud_Dashboard.get_manager().DurationMultiply * PrimitivesTweenGlobalDuration : XHud_Manager.Instance.DurationMultiply * PrimitivesTweenGlobalDuration;
            Vector3 ori_pos = Vector3.zero;
            if (isPreview)
            {
                if (PosSet)
                {
                    ori_pos = RectTransform.anchoredPosition3D;
                    element_PositionSet(pos);
                }
                element_AlphaSet(0);
                element_AlphaSyncUpdate();
            }

            #region 动作 - 透明度动画
            if (DebugState)
                XGUI_Utilitys.Console("XHud - 元素控件通知", args.Alpha.Ease != EaseMode.None ? "播放透明度动画 - 根据缓动参数" : "播放透明度动画 - 根据曲线参数", XGUIMsgState.通知);
            Tween_Alpha = ApplyEase(XTween.To(() => Alpha, x => Alpha = x, 1, args.Alpha.Duration * duration, isPreview ? false : true), args.Alpha.Ease, args.Alpha.Curve).SetDelay(args.Alpha.Delay)
                .OnUpdate<float>((v, d, t) =>
                {
                    if (isPreview)
                        element_AlphaSyncUpdate();
                    if (!isPreview)
                    {
                        if (args.MotionAnimateEndState == MotionAnimateEndState.以_透明度为准)
                        {
                            if (act_on_element_in_progress != null)
                                act_on_element_in_progress(Tween_Alpha.CurrentEasedProgress);
                        }
                    }
                })
                .OnComplete((d) =>
                {
                    if (!isPreview)
                    {
                        if (args.MotionAnimateEndState == MotionAnimateEndState.以_透明度为准)
                        {
                            if (act_on_element_in_end != null)
                                act_on_element_in_end(this);
                            if (eve_on_element_in_end != null)
                                eve_on_element_in_end.Invoke();
                            if (act_InComplete != null)
                                act_InComplete();
                            Element_In_End();
                        }
                    }
                })
                .OnKill(() =>
                {
                    if (isPreview)
                    {
                        element_AlphaSet(1);
                        element_AlphaSyncUpdate();
                    }
                });
            #endregion

            #region 动作 - 运动动画
            if (args.Movement.Movement != HudMotion_Movement.A_无运动)
            {
                #region 位置计算
                Vector3 endvalue = Vector3.zero;
                Vector3 fromvalue = Vector3.zero;
                Vector3 current = RectTransform.anchoredPosition3D;
                Vector3 scale = RectTransform.localScale;
                switch (args.Movement.Movement)
                {
                    case HudMotion_Movement.A_无运动:
                        break;
                    case HudMotion_Movement.S_从下至上:
                        endvalue = Vector3.zero;
                        fromvalue = Vector3.up * -args.Movement.Distance;
                        break;
                    case HudMotion_Movement.D_从上至下:
                        endvalue = Vector3.zero;
                        fromvalue = Vector3.up * args.Movement.Distance;
                        break;
                    case HudMotion_Movement.F_从左至右:
                        endvalue = Vector3.zero;
                        fromvalue = Vector3.right * -args.Movement.Distance;
                        break;
                    case HudMotion_Movement.G_从右至左:
                        endvalue = Vector3.zero;
                        fromvalue = Vector3.right * args.Movement.Distance;
                        break;
                    case HudMotion_Movement.H_从左上至中心:
                        endvalue = Vector3.zero;
                        fromvalue = (Vector3.left + Vector3.up) * args.Movement.Distance;
                        break;
                    case HudMotion_Movement.J_从左下至中心:
                        endvalue = Vector3.zero;
                        fromvalue = (Vector3.right + Vector3.up) * -args.Movement.Distance;
                        break;
                    case HudMotion_Movement.K_从右上至中心:
                        endvalue = Vector3.zero;
                        fromvalue = (Vector3.right + Vector3.up) * args.Movement.Distance;
                        break;
                    case HudMotion_Movement.L_从右下至中心:
                        endvalue = Vector3.zero;
                        fromvalue = (Vector3.right + Vector3.down) * args.Movement.Distance;
                        break;
                    case HudMotion_Movement.Z_从中心至左上:
                        endvalue = (Vector3.right + Vector3.down) * -args.Movement.Distance;
                        fromvalue = Vector3.zero;
                        break;
                    case HudMotion_Movement.X_从中心至左下:
                        endvalue = (Vector3.right + Vector3.up) * -args.Movement.Distance;
                        fromvalue = Vector3.zero;
                        break;
                    case HudMotion_Movement.C_从中心至右上:
                        endvalue = (Vector3.right + Vector3.up) * args.Movement.Distance;
                        fromvalue = Vector3.zero;
                        break;
                    case HudMotion_Movement.V_从中心至右下:
                        endvalue = (Vector3.right + Vector3.down) * args.Movement.Distance;
                        fromvalue = Vector3.zero;
                        break;
                    case HudMotion_Movement.B_从中心至左:
                        endvalue = Vector3.right * -args.Movement.Distance;
                        fromvalue = Vector3.zero;
                        break;
                    case HudMotion_Movement.N_从中心至右:
                        endvalue = Vector3.right * args.Movement.Distance;
                        fromvalue = Vector3.zero;
                        break;
                    case HudMotion_Movement.M_从中心至上:
                        endvalue = Vector3.up * args.Movement.Distance;
                        fromvalue = Vector3.zero;
                        break;
                    case HudMotion_Movement.Q_从中心至下:
                        endvalue = Vector3.up * -args.Movement.Distance;
                        fromvalue = Vector3.zero;
                        break;
                    case HudMotion_Movement.W_中心缩放:
                        endvalue = Vector3.one * args.Movement.Distance;
                        fromvalue = Vector3.zero;
                        break;
                    case HudMotion_Movement.E_从前到中心:
                        fromvalue = Vector3.forward * -args.Movement.Distance;
                        endvalue = Vector3.zero;
                        break;
                    case HudMotion_Movement.T_从后到中心:
                        fromvalue = Vector3.forward * args.Movement.Distance;
                        endvalue = Vector3.zero;
                        break;
                    case HudMotion_Movement.R_从中心到前:
                        fromvalue = Vector3.zero;
                        endvalue = Vector3.forward * -args.Movement.Distance;
                        break;
                    case HudMotion_Movement.Y_从中心到后:
                        fromvalue = Vector3.zero;
                        endvalue = Vector3.forward * args.Movement.Distance;
                        break;
                }
                if (args.Movement.Movement != HudMotion_Movement.W_中心缩放)
                {
                    endvalue += current;
                    fromvalue += current;
                }
                #endregion

                if (DebugState)
                    XGUI_Utilitys.Console("XHud - 元素控件通知", args.Movement.Ease != EaseMode.None ? "播放运动动画 - 根据缓动参数" : "播放运动动画 - 根据曲线参数", XGUIMsgState.通知);
                if (args.Movement.Movement == HudMotion_Movement.W_中心缩放)
                {
                    Tween_Move = ApplyEase(RectTransform.xt_Scale_To(endvalue, args.Movement.Duration * duration).SetAutoKill(isPreview ? false : true).SetFrom(fromvalue).SetDelay(args.Movement.Delay), args.Movement.Ease, args.Movement.Curve)
                        .OnUpdate<Vector3>((v, d, t) =>
                        {
                            if (!isPreview)
                            {
                                if (args.MotionAnimateEndState == MotionAnimateEndState.以_移动为准)
                                {
                                    if (act_on_element_in_progress != null)
                                        act_on_element_in_progress(Tween_Move.CurrentEasedProgress);
                                }
                            }
                        })
                        .OnComplete((d) =>
                        {
                            if (isPreview)
                                RectTransform.localScale = scale;
                            if (!isPreview)
                            {
                                if (args.MotionAnimateEndState == MotionAnimateEndState.以_移动为准)
                                {
                                    if (act_on_element_in_end != null)
                                        act_on_element_in_end(this);
                                    if (eve_on_element_in_end != null)
                                        eve_on_element_in_end.Invoke();
                                    if (act_InComplete != null)
                                        act_InComplete();
                                    Element_In_End();
                                }
                            }
                        })
                        .OnKill(() =>
                        {
                            if (isPreview)
                                element_ScaleSet(scale);
                        });
                }
                else
                {
                    Tween_Move = ApplyEase(RectTransform.xt_AnchoredPosition3D_To(endvalue, args.Movement.Duration * duration).SetAutoKill(isPreview ? false : true).SetFrom(fromvalue).SetRelative(true).SetDelay(args.Movement.Delay), args.Movement.Ease, args.Movement.Curve)
                        .OnUpdate<Vector3>((v, d, t) =>
                        {
                            if (!isPreview)
                            {
                                if (args.MotionAnimateEndState == MotionAnimateEndState.以_移动为准)
                                {
                                    if (act_on_element_in_progress != null)
                                        act_on_element_in_progress(Tween_Move.CurrentEasedProgress);
                                }
                            }
                        })
                        .OnComplete((d) =>
                        {
                            if (!isPreview)
                            {
                                if (args.MotionAnimateEndState == MotionAnimateEndState.以_移动为准)
                                {
                                    if (act_on_element_in_end != null)
                                        act_on_element_in_end(this);
                                    if (eve_on_element_in_end != null)
                                        eve_on_element_in_end.Invoke();
                                    if (act_InComplete != null)
                                        act_InComplete();
                                    Element_In_End();
                                }
                            }
                        })
                        .OnKill(() =>
                        {
                            if (isPreview)
                            {
                                if (PosSet)
                                    element_PositionSet(ori_pos);
                                else
                                    element_PositionSet(current);
                            }
                        });
                }
            }
            #endregion

            #region 动作 - 旋转动画
            if (args.Rotation.Rotation != HudMotion_Rotation.A_无旋转)
            {
                #region 角度计算
                Vector3 endvalue = Vector3.zero;
                Vector3 current = RectTransform.localEulerAngles;
                switch (args.Rotation.Rotation)
                {
                    case HudMotion_Rotation.A_无旋转:
                        break;
                    case HudMotion_Rotation.B_顺向_水平:
                        endvalue = Vector3.up * args.Rotation.Degree;
                        break;
                    case HudMotion_Rotation.C_顺向_垂直:
                        endvalue = Vector3.right * args.Rotation.Degree;
                        break;
                    case HudMotion_Rotation.D_顺向_倾角:
                        endvalue = Vector3.forward * args.Rotation.Degree;
                        break;
                    case HudMotion_Rotation.E_逆向_水平:
                        endvalue = Vector3.down * args.Rotation.Degree;
                        break;
                    case HudMotion_Rotation.F_逆向_垂直:
                        endvalue = Vector3.left * args.Rotation.Degree;
                        break;
                    case HudMotion_Rotation.G_逆向_倾角:
                        endvalue = Vector3.back * args.Rotation.Degree;
                        break;
                }
                #endregion

                if (DebugState)
                    XGUI_Utilitys.Console("XHud - 元素控件通知", args.Rotation.Ease != EaseMode.None ? "播放旋转动画 - 根据缓动参数" : "播放旋转动画 - 根据曲线参数", XGUIMsgState.通知);
                Tween_Rotation = ApplyEase(RectTransform.xt_Rotate_To(endvalue, args.Rotation.Duration * duration, true, false, XTweenRotationSpace.本地坐标, XTweenRotationMode.Normal).SetRelative(true).SetDelay(args.Rotation.Delay).SetAutoKill(isPreview ? false : true), args.Rotation.Ease, args.Rotation.Curve)
                    .OnUpdate<Vector3>((v, d, t) =>
                    {
                        if (!isPreview)
                        {
                            if (args.MotionAnimateEndState == MotionAnimateEndState.以_旋转为准)
                            {
                                if (act_on_element_in_progress != null)
                                    act_on_element_in_progress(Tween_Rotation.CurrentEasedProgress);
                            }
                        }
                    })
                    .OnComplete((d) =>
                    {
                        if (!isPreview)
                        {
                            if (args.MotionAnimateEndState == MotionAnimateEndState.以_旋转为准)
                            {
                                if (act_on_element_in_end != null)
                                    act_on_element_in_end(this);
                                if (eve_on_element_in_end != null)
                                    eve_on_element_in_end.Invoke();
                                if (act_InComplete != null)
                                    act_InComplete();
                                Element_In_End();
                            }
                        }
                    })
                    .OnKill(() =>
                    {
                        if (isPreview)
                            element_RotationSet(current);
                    });
            }
            #endregion
        }
        #endregion

        #region 剥离的元素独立动画单元 - 回收
        /// <summary>
        /// 剥离的独立动画单元：回收
        /// <para/>创建透明度、位移、旋转三类回收动画并挂载完整回调链。
        /// <para/>⚠️ 本次重构修复：Recycler-Rotation 的 OnKill 增加 isPreview 判断（原运行时 Kill 会误复位旋转）；
        /// 删除 Recycler-Move(普通位移) 无意义的空 OnStart 回调。
        /// </para>
        /// </summary>
        /// <param name="args">回收动效参数</param>
        /// <param name="act_OutComplete">完成时的额外回调</param>
        /// <param name="isPreview">是否为编辑器预览模式</param>
        /// <param name="PosSet">预览时是否预设位置</param>
        /// <param name="pos">预览预设位置</param>
        public void ElementTweens_Recycler(Motion_Recycler args, UnityAction act_OutComplete, bool isPreview = false, bool PosSet = false, Vector3 pos = default)
        {
            float duration = isPreview ? XHud_Dashboard.get_manager().DurationMultiply : XHud_Manager.Instance.DurationMultiply * PrimitivesTweenGlobalDuration;
            Vector3 ori_pos = Vector3.zero;
            if (isPreview)
            {
                if (PosSet)
                    element_PositionSet(pos);
                element_AlphaSet(1);
                element_AlphaSyncUpdate();
            }

            #region 动作 - 透明度动画
            if (DebugState)
                XGUI_Utilitys.Console("XHud - 元素控件通知", args.Alpha.Ease != EaseMode.None ? "播放透明度动画 - 根据缓动参数" : "播放透明度动画 - 根据曲线参数", XGUIMsgState.通知);
            Tween_Alpha = ApplyEase(XTween.To(() => Alpha, x => Alpha = x, 0, args.Alpha.Duration * duration, isPreview ? false : true), args.Alpha.Ease, args.Alpha.Curve)
                .SetDelay(args.Alpha.Delay)
                .OnUpdate<float>((v, d, t) =>
                {
                    if (isPreview)
                        element_AlphaSyncUpdate();
                    if (!isPreview)
                    {
                        if (args.MotionAnimateEndState == MotionAnimateEndState.以_透明度为准)
                        {
                            if (act_on_element_out_progress != null)
                                act_on_element_out_progress(Tween_Alpha.CurrentEasedProgress);
                        }
                    }
                })
                .OnComplete((d) =>
                {
                    if (!isPreview)
                    {
                        if (args.MotionAnimateEndState == MotionAnimateEndState.以_透明度为准)
                        {
                            if (act_on_element_out_end != null)
                                act_on_element_out_end(this);
                            if (eve_on_element_out_end != null)
                                eve_on_element_out_end.Invoke();
                            if (act_OutComplete != null)
                                act_OutComplete();
                            Element_Out_End(args);
                        }
                    }
                })
                .OnKill(() =>
                {
                    if (isPreview)
                    {
                        element_AlphaSet(1);
                        element_AlphaSyncUpdate();
                    }
                });
            #endregion

            #region 动作 - 运动动画
            if (args.Movement.Movement != HudMotion_Movement.A_无运动)
            {
                #region 位置计算
                Vector3 endvalue = Vector3.zero;
                Vector3 current = RectTransform.anchoredPosition3D;
                Vector3 scale = RectTransform.localScale;
                switch (args.Movement.Movement)
                {
                    case HudMotion_Movement.A_无运动:
                        break;
                    case HudMotion_Movement.S_从下至上:
                        endvalue = Vector3.up * args.Movement.Distance;
                        break;
                    case HudMotion_Movement.D_从上至下:
                        endvalue = Vector3.up * -args.Movement.Distance;
                        break;
                    case HudMotion_Movement.F_从左至右:
                        endvalue = Vector3.right * args.Movement.Distance;
                        break;
                    case HudMotion_Movement.G_从右至左:
                        endvalue = Vector3.right * -args.Movement.Distance;
                        break;
                    case HudMotion_Movement.H_从左上至中心:
                    case HudMotion_Movement.J_从左下至中心:
                    case HudMotion_Movement.K_从右上至中心:
                    case HudMotion_Movement.L_从右下至中心:
                        endvalue = Vector3.zero;
                        break;
                    case HudMotion_Movement.Z_从中心至左上:
                        endvalue = (Vector3.right + Vector3.down) * -args.Movement.Distance;
                        break;
                    case HudMotion_Movement.X_从中心至左下:
                        endvalue = (Vector3.right + Vector3.up) * -args.Movement.Distance;
                        break;
                    case HudMotion_Movement.C_从中心至右上:
                        endvalue = (Vector3.right + Vector3.up) * args.Movement.Distance;
                        break;
                    case HudMotion_Movement.V_从中心至右下:
                        endvalue = (Vector3.right + Vector3.down) * args.Movement.Distance;
                        break;
                    case HudMotion_Movement.B_从中心至左:
                        endvalue = Vector3.right * -args.Movement.Distance;
                        break;
                    case HudMotion_Movement.N_从中心至右:
                        endvalue = Vector3.right * args.Movement.Distance;
                        break;
                    case HudMotion_Movement.M_从中心至上:
                        endvalue = Vector3.up * args.Movement.Distance;
                        break;
                    case HudMotion_Movement.Q_从中心至下:
                        endvalue = Vector3.up * -args.Movement.Distance;
                        break;
                    case HudMotion_Movement.W_中心缩放:
                        endvalue = Vector3.zero;
                        break;
                    case HudMotion_Movement.R_从中心到前:
                        endvalue = Vector3.forward * -args.Movement.Distance;
                        break;
                    case HudMotion_Movement.Y_从中心到后:
                        endvalue = Vector3.forward * args.Movement.Distance;
                        break;
                }
                if (args.Movement.Movement != HudMotion_Movement.W_中心缩放)
                    endvalue += current;
                #endregion

                if (DebugState)
                    XGUI_Utilitys.Console("XHud - 元素控件通知", args.Movement.Ease != EaseMode.None ? "播放运动动画 - 根据缓动参数" : "播放运动动画 - 根据曲线参数", XGUIMsgState.通知);
                if (args.Movement.Movement == HudMotion_Movement.W_中心缩放)
                {
                    Tween_Move = ApplyEase(RectTransform.xt_Scale_To(endvalue, args.Movement.Duration * duration).SetDelay(args.Movement.Delay).SetAutoKill(isPreview ? false : true), args.Movement.Ease, args.Movement.Curve)
                        .OnUpdate<Vector3>((v, d, t) =>
                        {
                            if (!isPreview)
                            {
                                if (args.MotionAnimateEndState == MotionAnimateEndState.以_移动为准)
                                {
                                    if (act_on_element_out_progress != null)
                                        act_on_element_out_progress(Tween_Move.CurrentEasedProgress);
                                }
                            }
                        })
                        .OnComplete((d) =>
                        {
                            if (!isPreview)
                            {
                                if (args.MotionAnimateEndState == MotionAnimateEndState.以_移动为准)
                                {
                                    if (act_on_element_out_end != null)
                                        act_on_element_out_end(this);
                                    if (eve_on_element_out_end != null)
                                        eve_on_element_out_end.Invoke();
                                    if (act_OutComplete != null)
                                        act_OutComplete();
                                    Element_Out_End(args);
                                }
                            }
                        })
                        .OnKill(() =>
                        {
                            if (isPreview)
                                element_ScaleSet(scale);
                        });
                }
                else
                {
                    Tween_Move = ApplyEase(RectTransform.xt_AnchoredPosition3D_To(endvalue, args.Movement.Duration * duration).SetRelative(true).SetDelay(args.Movement.Delay).SetAutoKill(isPreview ? false : true), args.Movement.Ease, args.Movement.Curve)
                        .OnUpdate<Vector3>((v, d, t) =>
                        {
                            if (!isPreview)
                            {
                                if (args.MotionAnimateEndState == MotionAnimateEndState.以_移动为准)
                                {
                                    if (act_on_element_out_progress != null)
                                        act_on_element_out_progress(Tween_Move.CurrentEasedProgress);
                                }
                            }
                        })
                        .OnComplete((d) =>
                        {
                            if (!isPreview)
                            {
                                if (args.MotionAnimateEndState == MotionAnimateEndState.以_移动为准)
                                {
                                    if (act_on_element_out_end != null)
                                        act_on_element_out_end(this);
                                    if (eve_on_element_out_end != null)
                                        eve_on_element_out_end.Invoke();
                                    if (act_OutComplete != null)
                                        act_OutComplete();
                                    Element_Out_End(args);
                                }
                            }
                        })
                        .OnKill(() =>
                        {
                            if (isPreview)
                            {
                                if (PosSet)
                                    element_PositionSet(ori_pos);
                                else
                                    element_PositionSet(current);
                            }
                        });
                }
            }
            #endregion

            #region 动作 - 旋转动画
            if (args.Rotation.Rotation != HudMotion_Rotation.A_无旋转)
            {
                #region 角度计算
                Vector3 endvalue = Vector3.zero;
                Vector3 euler = RectTransform.localEulerAngles;
                switch (args.Rotation.Rotation)
                {
                    case HudMotion_Rotation.A_无旋转:
                        break;
                    case HudMotion_Rotation.B_顺向_水平:
                        endvalue = Vector3.up * args.Rotation.Degree;
                        break;
                    case HudMotion_Rotation.C_顺向_垂直:
                        endvalue = Vector3.right * args.Rotation.Degree;
                        break;
                    case HudMotion_Rotation.D_顺向_倾角:
                        endvalue = Vector3.forward * args.Rotation.Degree;
                        break;
                    case HudMotion_Rotation.E_逆向_水平:
                        endvalue = Vector3.down * args.Rotation.Degree;
                        break;
                    case HudMotion_Rotation.F_逆向_垂直:
                        endvalue = Vector3.left * args.Rotation.Degree;
                        break;
                    case HudMotion_Rotation.G_逆向_倾角:
                        endvalue = Vector3.back * args.Rotation.Degree;
                        break;
                }
                #endregion

                if (DebugState)
                    XGUI_Utilitys.Console("XHud - 元素控件通知", args.Rotation.Ease != EaseMode.None ? "播放旋转动画 - 根据缓动参数" : "播放旋转动画 - 根据曲线参数", XGUIMsgState.通知);
                Tween_Rotation = ApplyEase(RectTransform.xt_Rotate_To(endvalue, args.Rotation.Duration * duration, true, isPreview ? false : true, XTweenRotationSpace.本地坐标, XTweenRotationMode.Normal).SetRelative(true).SetDelay(args.Rotation.Delay).SetAutoKill(isPreview ? false : true), args.Rotation.Ease, args.Rotation.Curve)
                    .OnUpdate<Vector3>((v, d, t) =>
                    {
                        if (!isPreview)
                        {
                            if (args.MotionAnimateEndState == MotionAnimateEndState.以_旋转为准)
                            {
                                if (act_on_element_out_progress != null)
                                    act_on_element_out_progress(Tween_Rotation.CurrentEasedProgress);
                            }
                        }
                    })
                    .OnComplete((d) =>
                    {
                        if (!isPreview)
                        {
                            if (args.MotionAnimateEndState == MotionAnimateEndState.以_旋转为准)
                            {
                                if (act_on_element_out_end != null)
                                    act_on_element_out_end(this);
                                if (eve_on_element_out_end != null)
                                    eve_on_element_out_end.Invoke();
                                if (act_OutComplete != null)
                                    act_OutComplete();
                                Element_Out_End(args);
                            }
                        }
                    })
                    .OnKill(() =>
                    {
                        if (isPreview)
                            element_RotationSet(euler);
                    });
            }
            #endregion
        }
        #endregion
    }
}