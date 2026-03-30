namespace SevenStrikeModules.XHud
{
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.Utilitys;
    using SevenStrikeModules.XTween;
    using UnityEngine;
    using UnityEngine.Events;

    public partial class XHud_Module_Element : MonoBehaviour
    {
        [SerializeField]
        /// <summary>
        /// 动画状态
        /// 标识当前元素是否正在播放动画
        /// </summary>
        public bool Animating;

        #region 元素自身基础动画节点
        /// <summary>
        /// 动画模组 - 透明度_Alpha
        /// </summary>
        private XTween_Interface Tween_Alpha;
        /// <summary>
        /// 动画模组 - 位移
        /// </summary>
        private XTween_Interface Tween_Move;
        /// <summary>
        /// 动画模组 - 旋转_Rotation
        /// </summary>
        private XTween_Interface Tween_Rotation;
        #endregion

        #region 元素自身进入动画

        /// <summary>
        /// 元素动画 - 进入
        /// </summary>
        public virtual void element_In(Motion_Creator args = null, UnityAction act_InComplete = null)
        {
            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("元素控件通知", "杀死元素自身（透明度、位移、旋转）动画！", HudMsgState.警告);

            if (Tween_Alpha != null)
            {
                Tween_Alpha.Kill();
            }
            if (Tween_Move != null)
            {
                Tween_Move.Kill();
            }
            if (Tween_Rotation != null)
            {
                Tween_Rotation.Kill();
            }

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("元素控件通知", "进入动画开始！", HudMsgState.通知);

            #region 动作 - 动画开始
            if (act_on_element_in_start != null)
            {
                act_on_element_in_start(this);
            }
            if (eve_on_element_in_start != null)
                eve_on_element_in_start.Invoke();
            element_In_Start();
            #endregion

            #region 动作 - 透明度动画
            ///---动画 - 透明度_Alpha（Ease）
            if (args.Alpha.Ease != EaseMode.None)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("元素控件通知", "播放透明度动画 - 根据缓动参数", HudMsgState.通知);
                Tween_Alpha = XTween.To(
                    () => Alpha,
                    x => Alpha = x,
                    1,
                    args.Alpha.Duration * XHud_Manager.Instance.DurationMultiply * Element_Animators_GlobalDuration,
                    true)
                    .SetEase(args.Alpha.Ease)
                    .SetDelay(args.Alpha.Delay)
                    .OnUpdate<float>((v, d, t) =>
                    {
                        if (args.MotionAnimateEndState == MotionAnimateEndState.以_透明度为准)
                        {
                            if (act_on_element_in_progress != null)
                                act_on_element_in_progress(Tween_Alpha.CurrentEasedProgress);
                        }
                    }).OnComplete((d) =>
                    {
                        if (args.MotionAnimateEndState == MotionAnimateEndState.以_透明度为准)
                        {
                            if (act_on_element_in_end != null)
                                act_on_element_in_end(this);
                            if (eve_on_element_in_end != null)
                                eve_on_element_in_end.Invoke();
                            if (act_InComplete != null)
                                act_InComplete();
                            element_In_End();
                        }
                    }).Play();
            }
            ///---动画 - 透明度_Alpha（Curve）
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("元素控件通知", "播放透明度动画 - 根据曲线参数", HudMsgState.通知);
                Tween_Alpha = XTween.To(
                    () => Alpha,
                    x => Alpha = x,
                    1,
                    args.Alpha.Duration * XHud_Manager.Instance.DurationMultiply * Element_Animators_GlobalDuration,
                    true)
                    .SetEase(args.Alpha.Curve)
                    .SetDelay(args.Alpha.Delay)
                    .OnUpdate<float>((v, d, t) =>
                    {
                        if (args.MotionAnimateEndState == MotionAnimateEndState.以_透明度为准)
                        {
                            if (act_on_element_in_progress != null)
                                act_on_element_in_progress(Tween_Alpha.CurrentEasedProgress);
                        }
                    }).OnComplete((d) =>
                    {
                        if (args.MotionAnimateEndState == MotionAnimateEndState.以_透明度为准)
                        {
                            if (act_on_element_in_end != null)
                                act_on_element_in_end(this);
                            if (eve_on_element_in_end != null)
                                eve_on_element_in_end.Invoke();
                            if (act_InComplete != null)
                                act_InComplete();
                            element_In_End();
                        }
                    }).Play();
            }
            #endregion

            #region 动作 - 运动动画
            if (args.Movement.Movement != HudMotion_Movement.A_无运动)
            {
                Vector3 endvalue = Vector3.zero;
                Vector3 fromvalue = Vector3.zero;

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
                        endvalue = Vector3.one;
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

                ///---动画 - 位移（Ease）
                if (args.Movement.Ease != EaseMode.None)
                {
                    if (DebugState)
                        XHud_Utilitys.Func_PrintInfo("元素控件通知", "播放运动动画 - 根据缓动参数", HudMsgState.通知);
                    if (args.Movement.Movement == HudMotion_Movement.W_中心缩放)
                    {
                        Tween_Move = RectTransform.xt_Scale_To(
                            endvalue,
                            args.Movement.Duration * XHud_Manager.Instance.DurationMultiply * Element_Animators_GlobalDuration)
                            .SetAutoKill(true)
                            .SetFrom(fromvalue)
                            .SetDelay(args.Movement.Delay)
                            .SetEase(args.Movement.Ease)
                            .OnUpdate<Vector3>((v, d, t) =>
                            {
                                if (args.MotionAnimateEndState == MotionAnimateEndState.以_移动为准)
                                {
                                    if (act_on_element_in_progress != null)
                                        act_on_element_in_progress(Tween_Move.CurrentEasedProgress);
                                }
                            })
                            .OnComplete((d) =>
                            {
                                if (args.MotionAnimateEndState == MotionAnimateEndState.以_移动为准)
                                {
                                    if (act_on_element_in_end != null)
                                        act_on_element_in_end(this);
                                    if (eve_on_element_in_end != null)
                                        eve_on_element_in_end.Invoke();
                                    if (act_InComplete != null)
                                        act_InComplete();
                                    element_In_End();
                                }
                            }).Play();
                    }
                    else
                    {
                        Tween_Move = RectTransform.xt_AnchoredPosition3D_To(
                            endvalue,
                            args.Movement.Duration * XHud_Manager.Instance.DurationMultiply * Element_Animators_GlobalDuration)
                            .SetAutoKill(true)
                            .SetFrom(fromvalue)
                            .SetRelative(true)
                            .SetDelay(args.Movement.Delay)
                            .SetEase(args.Movement.Ease)
                            .OnUpdate<Vector3>((v, d, t) =>
                            {
                                if (args.MotionAnimateEndState == MotionAnimateEndState.以_移动为准)
                                {
                                    if (act_on_element_in_progress != null)
                                        act_on_element_in_progress(Tween_Move.CurrentEasedProgress);
                                }
                            })
                            .OnComplete((d) =>
                            {
                                if (args.MotionAnimateEndState == MotionAnimateEndState.以_移动为准)
                                {
                                    if (act_on_element_in_end != null)
                                        act_on_element_in_end(this);
                                    if (eve_on_element_in_end != null)
                                        eve_on_element_in_end.Invoke();
                                    if (act_InComplete != null)
                                        act_InComplete();
                                    element_In_End();
                                }
                            }).Play();
                    }
                }

                ///---动画 - 位移（Curve）
                if (args.Movement.Ease == EaseMode.None)
                {
                    if (DebugState)
                        XHud_Utilitys.Func_PrintInfo("元素控件通知", "播放运动动画 - 根据曲线参数", HudMsgState.通知);
                    if (args.Movement.Movement == HudMotion_Movement.W_中心缩放)
                    {
                        Tween_Move = RectTransform.xt_Scale_To(
                            endvalue,
                            args.Movement.Duration * XHud_Manager.Instance.DurationMultiply * Element_Animators_GlobalDuration)
                            .SetAutoKill(true)
                            .SetFrom(fromvalue)
                            .SetDelay(args.Movement.Delay)
                            .SetEase(args.Movement.Curve)
                            .OnUpdate<Vector3>((v, d, t) =>
                            {
                                if (args.MotionAnimateEndState == MotionAnimateEndState.以_移动为准)
                                {
                                    if (act_on_element_in_progress != null)
                                        act_on_element_in_progress(Tween_Move.CurrentEasedProgress);
                                }
                            })
                            .OnComplete((d) =>
                            {
                                if (args.MotionAnimateEndState == MotionAnimateEndState.以_移动为准)
                                {
                                    if (act_on_element_in_end != null)
                                        act_on_element_in_end(this);
                                    if (eve_on_element_in_end != null)
                                        eve_on_element_in_end.Invoke();
                                    if (act_InComplete != null)
                                        act_InComplete();
                                    element_In_End();
                                }
                            }).Play();
                    }
                    else
                    {
                        Tween_Move = RectTransform.xt_AnchoredPosition3D_To(
                            endvalue,
                            args.Movement.Duration * XHud_Manager.Instance.DurationMultiply * Element_Animators_GlobalDuration)
                            .SetAutoKill(true)
                            .SetFrom(fromvalue)
                            .SetRelative(true)
                            .SetDelay(args.Movement.Delay)
                            .SetEase(args.Movement.Curve)
                            .OnUpdate<Vector3>((v, d, t) =>
                            {
                                if (args.MotionAnimateEndState == MotionAnimateEndState.以_移动为准)
                                {
                                    if (act_on_element_in_progress != null)
                                        act_on_element_in_progress(Tween_Move.CurrentEasedProgress);
                                }
                            })
                            .OnComplete((d) =>
                            {
                                if (args.MotionAnimateEndState == MotionAnimateEndState.以_移动为准)
                                {
                                    if (act_on_element_in_end != null)
                                        act_on_element_in_end(this);
                                    if (eve_on_element_in_end != null)
                                        eve_on_element_in_end.Invoke();
                                    if (act_InComplete != null)
                                        act_InComplete();
                                    element_In_End();
                                }
                            }).Play();
                    }
                }
            }
            #endregion

            #region 动作 - 旋转动画

            if (args.Rotation.Rotation != HudMotion_Rotation.A_无旋转)
            {

                Vector3 fromvalue = Vector3.zero;
                Vector3 endvalue = Vector3.zero;

                switch (args.Rotation.Rotation)
                {
                    case HudMotion_Rotation.A_无旋转:
                        break;
                    case HudMotion_Rotation.B_顺向_水平:
                        fromvalue = Vector3.zero;
                        endvalue = Vector3.up * args.Rotation.Degree;
                        break;
                    case HudMotion_Rotation.C_顺向_垂直:
                        fromvalue = Vector3.zero;
                        endvalue = Vector3.right * args.Rotation.Degree;
                        break;
                    case HudMotion_Rotation.D_顺向_倾角:
                        fromvalue = Vector3.zero;
                        endvalue = Vector3.forward * args.Rotation.Degree;
                        break;
                    case HudMotion_Rotation.E_逆向_水平:
                        fromvalue = Vector3.zero;
                        endvalue = Vector3.down * args.Rotation.Degree;
                        break;
                    case HudMotion_Rotation.F_逆向_垂直:
                        fromvalue = Vector3.zero;
                        endvalue = Vector3.left * args.Rotation.Degree;
                        break;
                    case HudMotion_Rotation.G_逆向_倾角:
                        fromvalue = Vector3.zero;
                        endvalue = Vector3.back * args.Rotation.Degree;
                        break;
                }

                if (args.Rotation.Ease != EaseMode.None)
                {
                    if (DebugState)
                        XHud_Utilitys.Func_PrintInfo("元素控件通知", "播放旋转动画 - 根据缓动参数", HudMsgState.通知);
                    Tween_Rotation = RectTransform.xt_Rotate_To(endvalue,
                        args.Rotation.Duration * XHud_Manager.Instance.DurationMultiply * Element_Animators_GlobalDuration,
                        true,
                        true,
                        XTweenRotationSpace.绝对,
                        XTweenRotationMode.Normal)
                        .SetFrom(fromvalue)
                        .SetRelative(true)
                        .SetDelay(args.Rotation.Delay)
                        .SetEase(args.Rotation.Ease)
                        .SetAutoKill(true)
                        .OnUpdate<Vector3>((v, d, t) =>
                        {
                            if (args.MotionAnimateEndState == MotionAnimateEndState.以_旋转为准)
                            {
                                if (act_on_element_in_progress != null)
                                    act_on_element_in_progress(Tween_Rotation.CurrentEasedProgress);
                            }
                        })
                        .OnComplete((d) =>
                        {
                            if (args.MotionAnimateEndState == MotionAnimateEndState.以_旋转为准)
                            {
                                if (act_on_element_in_end != null)
                                    act_on_element_in_end(this);
                                if (eve_on_element_in_end != null)
                                    eve_on_element_in_end.Invoke();
                                if (act_InComplete != null)
                                    act_InComplete();
                                element_In_End();
                            }
                        }).Play();
                }

                if (args.Rotation.Ease == EaseMode.None)
                {
                    if (DebugState)
                        XHud_Utilitys.Func_PrintInfo("元素控件通知", "播放旋转动画 - 根据曲线参数", HudMsgState.通知);
                    Tween_Rotation = RectTransform.xt_Rotate_To(
                        endvalue,
                        args.Rotation.Duration * XHud_Manager.Instance.DurationMultiply * Element_Animators_GlobalDuration,
                        true,
                        true,
                        XTweenRotationSpace.绝对,
                        XTweenRotationMode.Normal)
                        .SetFrom(fromvalue)
                        .SetRelative(true)
                        .SetDelay(args.Rotation.Delay)
                        .SetEase(args.Rotation.Curve)
                        .SetAutoKill(true)
                        .OnUpdate<Vector3>((v, d, t) =>
                        {
                            if (args.MotionAnimateEndState == MotionAnimateEndState.以_旋转为准)
                            {
                                if (act_on_element_in_progress != null)
                                    act_on_element_in_progress(Tween_Rotation.CurrentEasedProgress);
                            }
                        })
                        .OnComplete((d) =>
                        {
                            if (args.MotionAnimateEndState == MotionAnimateEndState.以_旋转为准)
                            {
                                if (act_on_element_in_end != null)
                                    act_on_element_in_end(this);
                                if (eve_on_element_in_end != null)
                                    eve_on_element_in_end.Invoke();
                                if (act_InComplete != null)
                                    act_InComplete();
                                element_In_End();
                            }
                        }).Play();
                }
            }

            #endregion
        }

        /// <summary>
        /// 元素动画 - 进入 - 前
        /// </summary>
        public virtual void element_In_Start()
        {
            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("元素控件通知", "元素进入前逻辑调用", HudMsgState.通知);
            AnimateState = HudElementAnimateState.Animating;

            Animating = true;

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("元素控件通知", "禁用Canvas像素对齐！", HudMsgState.警告);

            if (AutoPlayAnimators)
            {
                ///-----播放Animator上的动画
                Animators_Play("元素进入时");
            }

            if (AutoPlayContainersAnimators)
            {
                for (int i = 0; i < ContainerNodes.Count; i++)
                {
                    if (ContainerNodes[i].Container.AnimatorPlayTiming == "元素进入时")
                        ContainerNodes[i].Container.Con_Animators_PlayAll();
                }
            }
        }

        /// <summary>
        /// 元素动画 - 进入 - 后
        /// </summary>
        public virtual void element_In_End()
        {
            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("元素控件通知", "元素进入后逻辑调用", HudMsgState.通知);
            AnimateState = HudElementAnimateState.Static;

            Animating = false;

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("元素控件通知", "恢复Canvas像素对齐！", HudMsgState.警告);

            if (AutoPlayAnimators)
            {
                ///-----播放Animator上的动画
                Animators_Play("元素进入后");
            }

            if (AutoPlayContainersAnimators)
            {
                for (int i = 0; i < ContainerNodes.Count; i++)
                {
                    if (ContainerNodes[i].Container.AnimatorPlayTiming == "元素进入后")
                        ContainerNodes[i].Container.Con_Animators_PlayAll();
                }
            }
        }

        #endregion

        #region 元素自身退出动画

        /// <summary>
        /// 元素动画 - 退出
        /// </summary>
        public virtual void element_Out(Motion_Recycler args = null, UnityAction act_OutComplete = null)
        {
            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("元素控件通知", "杀死元素自身（透明度、位移、旋转）动画！", HudMsgState.警告);

            if (Tween_Alpha != null)
                Tween_Alpha.Kill();
            if (Tween_Move != null)
                Tween_Move.Kill();
            if (Tween_Rotation != null)
                Tween_Rotation.Kill();

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("元素控件通知", "退出动画开始！", HudMsgState.通知);

            #region 动作 - 动画开始
            if (act_on_element_out_start != null)
                act_on_element_out_start(this);
            if (eve_on_element_out_start != null)
                eve_on_element_out_start.Invoke();
            element_Out_Start();
            #endregion

            #region 动作 - 透明度动画
            ///---动画 - 透明度_Alpha（Ease）
            if (args.Alpha.Ease != EaseMode.None)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("元素控件通知", "播放透明度动画 - 根据缓动参数", HudMsgState.通知);
                Tween_Alpha = XTween.To(
                    () => Alpha,
                    x => Alpha = x,
                    0,
                    args.Alpha.Duration * XHud_Manager.Instance.DurationMultiply * Element_Animators_GlobalDuration)
                    .SetEase(args.Alpha.Ease)
                    .SetDelay(args.Alpha.Delay)
                    .SetAutoKill(true)
                    .OnUpdate<float>((v, d, t) =>
                    {
                        if (args.MotionAnimateEndState == MotionAnimateEndState.以_透明度为准)
                        {
                            if (act_on_element_out_progress != null)
                                act_on_element_out_progress(Tween_Alpha.CurrentEasedProgress);
                        }
                    })
                    .OnComplete((d) =>
                    {
                        if (args.MotionAnimateEndState == MotionAnimateEndState.以_透明度为准)
                        {
                            if (act_on_element_out_end != null)
                                act_on_element_out_end(this);
                            if (eve_on_element_out_end != null)
                                eve_on_element_out_end.Invoke();
                            if (act_OutComplete != null)
                                act_OutComplete();
                            element_Out_End(args);
                        }
                    }).Play();
            }

            ///---动画 - 透明度_Alpha（Curve）
            if (args.Alpha.Ease == EaseMode.None)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("元素控件通知", "播放透明度动画 - 根据曲线参数", HudMsgState.通知);
                Tween_Alpha = XTween.To(
                    () => Alpha,
                    x => Alpha = x,
                    0,
                    args.Alpha.Duration * XHud_Manager.Instance.DurationMultiply * Element_Animators_GlobalDuration)
                    .SetEase(args.Alpha.Curve)
                    .SetDelay(args.Alpha.Delay)
                    .SetAutoKill(true)
                    .OnUpdate<float>((v, d, t) =>
                    {
                        if (args.MotionAnimateEndState == MotionAnimateEndState.以_透明度为准)
                        {
                            if (act_on_element_out_progress != null)
                                act_on_element_out_progress(Tween_Alpha.CurrentEasedProgress);
                        }
                    })
                    .OnComplete((d) =>
                    {
                        if (args.MotionAnimateEndState == MotionAnimateEndState.以_透明度为准)
                        {
                            if (act_on_element_out_end != null)
                                act_on_element_out_end(this);
                            if (eve_on_element_out_end != null)
                                eve_on_element_out_end.Invoke();
                            if (act_OutComplete != null)
                                act_OutComplete();
                            element_Out_End(args);
                        }
                    }).Play();
            }
            #endregion

            #region 动作 - 运动动画
            if (args.Movement.Movement != HudMotion_Movement.A_无运动)
            {
                Vector3 endvalue = Vector3.zero;

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

                ///---动画 - 位移（Ease）
                if (args.Movement.Ease != EaseMode.None)
                {
                    if (DebugState)
                        XHud_Utilitys.Func_PrintInfo("元素控件通知", "播放运动动画 - 根据缓动参数", HudMsgState.通知);
                    if (args.Movement.Movement == HudMotion_Movement.W_中心缩放)
                    {
                        Tween_Move = RectTransform.xt_Scale_To(
                            endvalue,
                            args.Movement.Duration * XHud_Manager.Instance.DurationMultiply * Element_Animators_GlobalDuration)
                            .SetDelay(args.Movement.Delay).
                            SetEase(args.Movement.Ease).
                            SetAutoKill(true).
                            OnUpdate<Vector3>((v, d, t) =>
                            {
                                if (args.MotionAnimateEndState == MotionAnimateEndState.以_移动为准)
                                {
                                    if (act_on_element_out_progress != null)
                                        act_on_element_out_progress(Tween_Move.CurrentEasedProgress);
                                }
                            }).
                        OnComplete((d) =>
                        {
                            if (args.MotionAnimateEndState == MotionAnimateEndState.以_移动为准)
                            {
                                if (act_on_element_out_end != null)
                                    act_on_element_out_end(this);
                                if (eve_on_element_out_end != null)
                                    eve_on_element_out_end.Invoke();
                                if (act_OutComplete != null)
                                    act_OutComplete();
                                element_Out_End(args);
                            }
                        }).Play();
                    }
                    else
                    {
                        Tween_Move = RectTransform.xt_AnchoredPosition3D_To(
                            endvalue,
                            args.Movement.Duration * XHud_Manager.Instance.DurationMultiply * Element_Animators_GlobalDuration)
                            .SetRelative(true)
                            .SetDelay(args.Movement.Delay)
                            .SetEase(args.Movement.Ease)
                            .SetAutoKill(true)
                            .OnUpdate<Vector3>((v, d, t) =>
                            {
                                if (args.MotionAnimateEndState == MotionAnimateEndState.以_移动为准)
                                {
                                    if (act_on_element_out_progress != null)
                                        act_on_element_out_progress(Tween_Move.CurrentEasedProgress);
                                }
                            })
                            .OnComplete((d) =>
                            {
                                if (args.MotionAnimateEndState == MotionAnimateEndState.以_移动为准)
                                {
                                    if (act_on_element_out_end != null)
                                        act_on_element_out_end(this);
                                    if (eve_on_element_out_end != null)
                                        eve_on_element_out_end.Invoke();
                                    if (act_OutComplete != null)
                                        act_OutComplete();
                                    element_Out_End(args);
                                }
                            }).Play();
                    }
                }

                ///---动画 - 位移（Curve）
                if (args.Movement.Ease == EaseMode.None)
                {
                    if (DebugState)
                        XHud_Utilitys.Func_PrintInfo("元素控件通知", "播放运动动画 - 根据曲线参数", HudMsgState.通知);
                    if (args.Movement.Movement == HudMotion_Movement.W_中心缩放)
                    {
                        Tween_Move = RectTransform.xt_Scale_To(
                            endvalue,
                            args.Movement.Duration * XHud_Manager.Instance.DurationMultiply * Element_Animators_GlobalDuration)
                            .SetDelay(args.Movement.Delay)
                            .SetEase(args.Movement.Curve)
                            .SetAutoKill(true)
                            .OnUpdate<Vector3>((v, d, t) =>
                            {
                                if (args.MotionAnimateEndState == MotionAnimateEndState.以_移动为准)
                                {
                                    if (act_on_element_out_progress != null)
                                        act_on_element_out_progress(Tween_Move.CurrentEasedProgress);
                                }
                            })
                            .OnComplete((d) =>
                            {
                                if (args.MotionAnimateEndState == MotionAnimateEndState.以_移动为准)
                                {
                                    if (act_on_element_out_end != null)
                                        act_on_element_out_end(this);
                                    if (eve_on_element_out_end != null)
                                        eve_on_element_out_end.Invoke();
                                    if (act_OutComplete != null)
                                        act_OutComplete();
                                    element_Out_End(args);
                                }
                            }).Play();
                    }
                    else
                    {
                        Tween_Move = RectTransform.xt_AnchoredPosition3D_To(
                            endvalue,
                            args.Movement.Duration * XHud_Manager.Instance.DurationMultiply * Element_Animators_GlobalDuration)
                            .SetRelative(true)
                            .SetDelay(args.Movement.Delay)
                            .SetEase(args.Movement.Curve)
                            .SetAutoKill(true)
                            .OnUpdate<Vector3>((v, d, t) =>
                            {
                                if (args.MotionAnimateEndState == MotionAnimateEndState.以_移动为准)
                                {
                                    if (act_on_element_out_progress != null)
                                        act_on_element_out_progress(Tween_Move.CurrentEasedProgress);
                                }
                            })
                            .OnComplete((d) =>
                            {
                                if (args.MotionAnimateEndState == MotionAnimateEndState.以_移动为准)
                                {
                                    if (act_on_element_out_end != null)
                                        act_on_element_out_end(this);
                                    if (eve_on_element_out_end != null)
                                        eve_on_element_out_end.Invoke();
                                    if (act_OutComplete != null)
                                        act_OutComplete();
                                    element_Out_End(args);
                                }
                            }).Play();
                    }
                }
            }
            #endregion

            #region 动作 - 旋转动画
            if (args.Rotation.Rotation != HudMotion_Rotation.A_无旋转)
            {
                Vector3 endvalue = Vector3.zero;

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

                if (args.Rotation.Ease != EaseMode.None)
                {
                    if (DebugState)
                        XHud_Utilitys.Func_PrintInfo("元素控件通知", "播放旋转动画 - 根据缓动参数", HudMsgState.通知);
                    Tween_Rotation = RectTransform.xt_Rotate_To(
                        endvalue,
                        args.Rotation.Duration * XHud_Manager.Instance.DurationMultiply * Element_Animators_GlobalDuration,
                        true,
                        true,
                        XTweenRotationSpace.绝对,
                        XTweenRotationMode.Normal)
                        .SetRelative(true)
                        .SetDelay(args.Rotation.Delay)
                        .SetEase(args.Rotation.Ease)
                        .SetAutoKill(true)
                        .OnUpdate<Vector3>((v, d, t) =>
                        {
                            if (args.MotionAnimateEndState == MotionAnimateEndState.以_旋转为准)
                            {
                                if (act_on_element_out_progress != null)
                                    act_on_element_out_progress(Tween_Rotation.CurrentEasedProgress);
                            }
                        })
                        .OnComplete((d) =>
                        {
                            if (args.MotionAnimateEndState == MotionAnimateEndState.以_旋转为准)
                            {
                                if (act_on_element_out_end != null)
                                    act_on_element_out_end(this);
                                if (eve_on_element_out_end != null)
                                    eve_on_element_out_end.Invoke();
                                if (act_OutComplete != null)
                                    act_OutComplete();
                                element_Out_End(args);
                            }
                        }).Play();
                }

                if (args.Rotation.Ease == EaseMode.None)
                {
                    if (DebugState)
                        XHud_Utilitys.Func_PrintInfo("元素控件通知", "播放旋转动画 - 根据曲线参数", HudMsgState.通知);
                    Tween_Rotation = RectTransform.xt_Rotate_To(
                        endvalue,
                        args.Rotation.Duration * XHud_Manager.Instance.DurationMultiply * Element_Animators_GlobalDuration,
                        true,
                        true,
                        XTweenRotationSpace.绝对,
                        XTweenRotationMode.Normal)
                        .SetRelative(true)
                        .SetDelay(args.Rotation.Delay)
                        .SetEase(args.Rotation.Curve)
                        .SetAutoKill(true)
                        .OnUpdate<Vector3>((v, d, t) =>
                        {
                            if (args.MotionAnimateEndState == MotionAnimateEndState.以_旋转为准)
                            {
                                if (act_on_element_out_progress != null)
                                    act_on_element_out_progress(Tween_Rotation.CurrentEasedProgress);
                            }
                        })
                        .OnComplete((d) =>
                        {
                            if (args.MotionAnimateEndState == MotionAnimateEndState.以_旋转为准)
                            {
                                if (act_on_element_out_end != null)
                                    act_on_element_out_end(this);
                                if (eve_on_element_out_end != null)
                                    eve_on_element_out_end.Invoke();
                                if (act_OutComplete != null)
                                    act_OutComplete();
                                element_Out_End(args);
                            }
                        }).Play();
                }
            }
            #endregion
        }

        /// <summary>
        /// 元素动画 - 退出 - 前
        /// </summary>
        public virtual void element_Out_Start()
        {
            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("元素控件通知", "退出前逻辑调用", HudMsgState.通知);

            ///---元素动画状态变为动画中
            AnimateState = HudElementAnimateState.Animating;

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("元素控件通知", "禁用Canvas像素对齐！", HudMsgState.警告);

            if (XHud_Manager.Instance.UseAutoPerfectPixel)
                XHud_Manager.Instance.hm_UsePixelPerfect(false);

            Animating = true;

            if (AutoPlayAnimators)
            {
                ///-----播放Animator上的动画
                Animators_Play("元素退出时");
            }

            if (AutoPlayContainersAnimators)
            {
                for (int i = 0; i < ContainerNodes.Count; i++)
                {
                    if (ContainerNodes[i].Container.AnimatorPlayTiming == "元素退出时")
                        ContainerNodes[i].Container.Con_Animators_PlayAll();
                }
            }
        }

        /// <summary>
        /// 元素动画 - 退出 - 后
        /// </summary>
        public virtual void element_Out_End(Motion_Recycler args = null)
        {
            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("元素控件通知", "退出后逻辑调用", HudMsgState.通知);

            ///---元素动画状态变为静态
            AnimateState = HudElementAnimateState.Static;

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("元素控件通知", "恢复Canvas像素对齐！", HudMsgState.警告);

            ///---像素对齐开启
            if (XHud_Manager.Instance.UseAutoPerfectPixel)
                XHud_Manager.Instance.hm_UsePixelPerfect(true);

            ///---元素动画开关为关
            Animating = false;

            ///---移除跟踪器
            element_ObjectTracker_Remove();

            ///---所有动画重置
            Animators_Rewind();

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("元素控件通知", "将所有子节点中的Animator的动画都立即杀死！", HudMsgState.通知);
            for (int i = 0; i < AnimatorNodes.Count; i++)
            {
                for (int s = 0; s < AnimatorNodes[i].Animator.AnimateTweenNodes.Count; s++)
                {
                    AnimatorNodes[i].Animator.AnimateTweenNodes[s].Tweener.Kill();
                }
            }

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("元素控件通知", "从HudManager的布局列表中移除！", HudMsgState.通知);
            ///---从管理器的布局中移除元素项以便于下一次使用（元素生成后会在HudManager中的LayoutAnchors中记录赋值）
            for (int i = 0; i < XHud_Manager.Instance.Anchors_Layout_Screen.Count; i++)
            {
                for (int s = 0; s < XHud_Manager.Instance.Anchors_Layout_Screen[i].HudElementInfos.Count; s++)
                {
                    if (this.ID == XHud_Manager.Instance.Anchors_Layout_Screen[i].HudElementInfos[s].ID)
                    {
                        XHud_Manager.Instance.Anchors_Layout_Screen[i].HudElementInfos.RemoveAt(s);
                    }
                }
            }

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("元素控件通知", "回收到元素池！", HudMsgState.通知);
            if (CreateState == HudElementCreateState.Created)
            {
                ///---回收元素
                XHud_Manager.Instance.hm_ElementLibrary_Despawn(this);
            }
        }

        #endregion
    }
}