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
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.Utilitys;
    using SevenStrikeModules.XTween;
    using UnityEngine;
    using UnityEngine.Events;
    using static UnityEditor.PlayerSettings;

    public partial class XHud_Module_Element : MonoBehaviour
    {
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
                Duration = 1,           // 透明度动画持续时间（秒）
                Delay = 0,              // 透明度动画延迟时间（秒）
                Curve = AnimationCurve.EaseInOut(0, 0, 1, 1),  // 透明度动画曲线
                CurveName = "",         // 曲线库中对应的曲线名称
                Ease = EaseMode.InOutCubic  // 缓动模式
            },
            Movement = new MotionNode_Movement
            {
                Movement = HudMotion_Movement.S_从下至上,  // 位移方向类型
                Distance = 100,         // 位移距离（像素）
                Duration = 1,           // 位移动画持续时间（秒）
                Delay = 0,              // 位移动画延迟时间（秒）
                Curve = AnimationCurve.EaseInOut(0, 0, 1, 1),  // 位移动画曲线
                CurveName = "",         // 曲线库中对应的曲线名称
                Ease = EaseMode.InOutCubic  // 缓动模式
            },
            Rotation = new MotionNode_Rotation
            {
                Rotation = HudMotion_Rotation.A_无旋转,  // 旋转方向类型
                Degree = 0,             // 旋转角度（度）
                Duration = 1,           // 旋转动画持续时间（秒）
                Delay = 0,              // 旋转动画延迟时间（秒）
                Curve = AnimationCurve.EaseInOut(0, 0, 1, 1),  // 旋转动画曲线
                CurveName = "",         // 曲线库中对应的曲线名称
                Ease = EaseMode.InOutCubic  // 缓动模式
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
                Duration = 1,           // 透明度动画持续时间（秒）
                Delay = 0,              // 透明度动画延迟时间（秒）
                Curve = AnimationCurve.EaseInOut(0, 0, 1, 1),  // 透明度动画曲线
                CurveName = "",         // 曲线库中对应的曲线名称
                Ease = EaseMode.InOutCubic  // 缓动模式
            },
            Movement = new MotionNode_Movement
            {
                Movement = HudMotion_Movement.D_从上至下,  // 位移方向类型
                Distance = 100,         // 位移距离（像素）
                Duration = 1,           // 位移动画持续时间（秒）
                Delay = 0,              // 位移动画延迟时间（秒）
                Curve = AnimationCurve.EaseInOut(0, 0, 1, 1),  // 位移动画曲线
                CurveName = "",         // 曲线库中对应的曲线名称
                Ease = EaseMode.InOutCubic  // 缓动模式
            },
            Rotation = new MotionNode_Rotation
            {
                Rotation = HudMotion_Rotation.A_无旋转,  // 旋转方向类型
                Degree = 0,             // 旋转角度（度）
                Duration = 1,           // 旋转动画持续时间（秒）
                Delay = 0,              // 旋转动画延迟时间（秒）
                Curve = AnimationCurve.EaseInOut(0, 0, 1, 1),  // 旋转动画曲线
                CurveName = "",         // 曲线库中对应的曲线名称
                Ease = EaseMode.InOutCubic  // 缓动模式
            }
        };
        [SerializeField]
        /// <summary>
        /// 动效参数折叠状态（编辑器使用）
        /// 控制 Inspector 面板中动效参数区域的展开/折叠状态
        /// </summary>
        public bool preview_motionArgs_fold;
        [SerializeField]
        /// <summary>
        /// 生成动效 - 位移参数折叠状态（编辑器使用）
        /// 控制 Inspector 面板中生成动效位移参数区域的展开/折叠状态
        /// </summary>
        public bool create_fold_move;
        [SerializeField]
        /// <summary>
        /// 生成动效 - 旋转参数折叠状态（编辑器使用）
        /// 控制 Inspector 面板中生成动效旋转参数区域的展开/折叠状态
        /// </summary>
        public bool create_fold_rotate;
        [SerializeField]
        /// <summary>
        /// 生成动效 - 透明度参数折叠状态（编辑器使用）
        /// 控制 Inspector 面板中生成动效透明度参数区域的展开/折叠状态
        /// </summary>
        public bool create_fold_alpha;
        [SerializeField]
        /// <summary>
        /// 回收动效 - 位移参数折叠状态（编辑器使用）
        /// 控制 Inspector 面板中回收动效位移参数区域的展开/折叠状态
        /// </summary>
        public bool recycle_fold_move;
        [SerializeField]
        /// <summary>
        /// 回收动效 - 旋转参数折叠状态（编辑器使用）
        /// 控制 Inspector 面板中回收动效旋转参数区域的展开/折叠状态
        /// </summary>
        public bool recycle_fold_rotate;
        [SerializeField]
        /// <summary>
        /// 回收动效 - 透明度参数折叠状态（编辑器使用）
        /// 控制 Inspector 面板中回收动效透明度参数区域的展开/折叠状态
        /// </summary>
        public bool recycle_fold_alpha;

        /// <summary>
        /// 杀死已创建的元素三项基础动画
        /// </summary>
        public void KillElementTweens()
        {
            if (Tween_Alpha != null)
                Tween_Alpha.Kill();
            if (Tween_Move != null)
                Tween_Move.Kill();
            if (Tween_Rotation != null)
                Tween_Rotation.Kill();
        }
        /// <summary>
        /// 清空已创建的元素三项基础动画
        /// </summary>
        public void ClearElementTweens()
        {
            // 如果 透明度 动画已创建不为空则清空
            if (Tween_Alpha != null)
                Tween_Alpha = null;
            // 如果 运动 动画已创建不为空则清空
            if (Tween_Move != null)
                Tween_Move = null;
            // 如果 旋转 动画已创建不为空则清空
            if (Tween_Rotation != null)
                Tween_Rotation = null;
        }
        /// <summary>
        /// 杀死已创建的元子级下的所有图元动画
        /// </summary>
        public void KillPrimitiveTweens()
        {
            if (Tween_Alpha != null)
                Tween_Alpha.Kill();
            if (Tween_Move != null)
                Tween_Move.Kill();
            if (Tween_Rotation != null)
                Tween_Rotation.Kill();
        }
        /// <summary>
        /// 清空已创建的元素子级下的所有图元画
        /// </summary>
        public void ClearPrimitiveTweens()
        {
            // 如果 透明度 动画已创建不为空则清空
            if (Tween_Alpha != null)
                Tween_Alpha = null;
            // 如果 运动 动画已创建不为空则清空
            if (Tween_Move != null)
                Tween_Move = null;
            // 如果 旋转 动画已创建不为空则清空
            if (Tween_Rotation != null)
                Tween_Rotation = null;
        }

        #region 元素自身进入动画
        /// <summary>
        /// 元素动画 - 进入
        /// </summary>
        public virtual void Element_In(Motion_Creator args = null, UnityAction act_InComplete = null)
        {
            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "杀死元素自身（透明度、位移、旋转）动画！", HudMsgState.警告);

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
                XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "进入动画开始！", HudMsgState.通知);

            #region 动作 - 动画开始
            if (act_on_element_in_start != null)
            {
                act_on_element_in_start(this);
            }
            if (eve_on_element_in_start != null)
                eve_on_element_in_start.Invoke();
            Element_In_Start();
            #endregion

            // 剥离的独立动画单元：生成
            ElementTweens_Creator(args, act_InComplete);
        }
        /// <summary>
        /// 元素动画 - 进入 - 前
        /// </summary>
        public virtual void Element_In_Start()
        {
            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "元素进入前逻辑调用", HudMsgState.通知);
            AnimateState = XHudElementAnimateState.Animating;

            Animating = true;

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "禁用Canvas像素对齐！", HudMsgState.警告);

            if (AutoPlayPrimitivesTween)
            {
                ///-----播放 图元动画器上的动画
                PrimitiveTween_Play("元素进入时");
            }
        }
        /// <summary>
        /// 元素动画 - 进入 - 后
        /// </summary>
        public virtual void Element_In_End()
        {
            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "元素进入后逻辑调用", HudMsgState.通知);
            AnimateState = XHudElementAnimateState.Static;

            Animating = false;

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "恢复Canvas像素对齐！", HudMsgState.警告);

            if (AutoPlayPrimitivesTween)
            {
                ///-----播放 图元动画器上的动画
                PrimitiveTween_Play("元素进入后");
            }
        }
        #endregion

        #region 元素自身退出动画
        /// <summary>
        /// 元素动画 - 退出
        /// </summary>
        public virtual void Element_Out(Motion_Recycler args = null, UnityAction act_OutComplete = null)
        {
            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "杀死元素自身（透明度、位移、旋转）动画！", HudMsgState.警告);

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
                XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "退出动画开始！", HudMsgState.通知);

            #region 动作 - 动画开始
            if (act_on_element_out_start != null)
                act_on_element_out_start(this);
            if (eve_on_element_out_start != null)
                eve_on_element_out_start.Invoke();
            Element_Out_Start();
            #endregion

            // 剥离的独立动画单元：回收
            ElementTweens_Recycler(args, act_OutComplete);
        }
        /// <summary>
        /// 元素动画 - 退出 - 前
        /// </summary>
        public virtual void Element_Out_Start()
        {
            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "退出前逻辑调用", HudMsgState.通知);

            ///---元素动画状态变为动画中
            AnimateState = XHudElementAnimateState.Animating;

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "禁用Canvas像素对齐！", HudMsgState.警告);

            if (XHud_Manager.Instance.UseAutoPerfectPixel)
                XHud_Manager.Instance.hm_UsePixelPerfect(false);

            Animating = true;

            if (AutoPlayPrimitivesTween)
            {
                ///-----播放 图元动画器上的动画
                PrimitiveTween_Play("元素退出时");
            }
        }
        /// <summary>
        /// 元素动画 - 退出 - 后
        /// </summary>
        public virtual void Element_Out_End(Motion_Recycler args = null)
        {
            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "退出后逻辑调用", HudMsgState.通知);

            ///---元素动画状态变为静态
            AnimateState = XHudElementAnimateState.Static;

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "恢复Canvas像素对齐！", HudMsgState.警告);

            ///---像素对齐开启
            if (Application.isPlaying)
            {
                if (XHud_Manager.Instance.UseAutoPerfectPixel)
                    XHud_Manager.Instance.hm_UsePixelPerfect(true);
            }
            else
            {
                if (XHud_Dashboard.HudManagerGet().UseAutoPerfectPixel)
                    XHud_Dashboard.HudManagerGet().hm_UsePixelPerfect(true);
            }

            ///---元素动画开关为关
            Animating = false;

            ///---移除跟踪器
            element_ObjectTracker_Remove();

            ///---所有动画重置
            PrimitiveTween_Rewind();

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "将所有子节点中的图元动画器的动画都立即杀死！", HudMsgState.通知);
            for (int i = 0; i < PrimitiveControllerNodes.Count; i++)
            {
                for (int s = 0; s < PrimitiveControllerNodes[i].Controller.pt_Tween.PrimitiveTweenNodes.Count; s++)
                {
                    TweenNode node = PrimitiveControllerNodes[i].Controller.pt_Tween.PrimitiveTweenNodes[s];
                    if (!PrimitiveControllerNodes[i].Controller.gameObject.activeInHierarchy)
                        continue;
                    if (node.Tweener != null)
                        node.Tweener.Kill();
                }
            }

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "从HudManager的布局列表中移除！", HudMsgState.通知);
            ///---从管理器的布局中移除元素项以便于下一次使用（元素生成后会在HudManager中的LayoutAnchors中记录赋值）
            if (Application.isPlaying)
            {
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
            }

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "回收到元素池！", HudMsgState.通知);
            if (CreateState == XHudElementCreateState.Created)
            {
                if (this.CreatedSourceType == XHudElementCreatedSourceType.Library)
                {
                    ///---回收元素
                    XHud_Manager.Instance.hm_ElementLibrary_Despawn(this);
                }
                else
                {
                    Destroy(this.gameObject);
                }
            }
        }
        #endregion

        #region 剥离的元素独立动画单元
        /// <summary>
        /// 剥离的独立动画单元：生成
        /// </summary>
        /// <param name="args"></param>
        /// <param name="act_InComplete"></param>
        public void ElementTweens_Creator(Motion_Creator args, UnityAction act_InComplete, bool isPreview = false, bool PosSet = false, Vector3 pos = default)
        {
            float duration = isPreview ? XHud_Dashboard.HudManagerGet().DurationMultiply : XHud_Manager.Instance.DurationMultiply * PrimitivesTweenGlobalDuration;
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
            ///---动画 - 透明度_Alpha（Ease）
            if (args.Alpha.Ease != EaseMode.None)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "播放透明度动画 - 根据缓动参数", HudMsgState.通知);
                Tween_Alpha = XTween.To(() => Alpha, x => Alpha = x, 1, args.Alpha.Duration * duration, isPreview ? false : true).SetEase(args.Alpha.Ease).SetDelay(args.Alpha.Delay).OnUpdate<float>((v, d, t) =>
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
                }).OnComplete((d) =>
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
                }).OnKill(() =>
                        {
                            if (isPreview)
                            {
                                element_AlphaSet(1);
                                element_AlphaSyncUpdate();
                            }
                        }).Play();
            }
            ///---动画 - 透明度_Alpha（Curve）
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "播放透明度动画 - 根据曲线参数", HudMsgState.通知);
                Tween_Alpha = XTween.To(() => Alpha, x => Alpha = x, 1, args.Alpha.Duration * duration, isPreview ? false : true).SetEase(args.Alpha.Curve).SetDelay(args.Alpha.Delay).OnUpdate<float>((v, d, t) =>
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
                }).OnComplete((d) =>
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
                }).OnKill(() =>
                {
                    if (isPreview)
                    {
                        element_AlphaSet(1);
                        element_AlphaSyncUpdate();
                    }
                }).Play();
            }
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

                // 叠加自身原始位置
                if (args.Movement.Movement != HudMotion_Movement.W_中心缩放)
                {
                    endvalue += current;
                    fromvalue += current;
                }
                #endregion

                ///---动画 - 位移（Ease）
                if (args.Movement.Ease != EaseMode.None)
                {
                    if (DebugState)
                        XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "播放运动动画 - 根据缓动参数", HudMsgState.通知);
                    if (args.Movement.Movement == HudMotion_Movement.W_中心缩放)
                    {
                        Tween_Move = RectTransform.xt_Scale_To(endvalue, args.Movement.Duration * duration).SetAutoKill(isPreview ? false : true).SetFrom(fromvalue).SetDelay(args.Movement.Delay).SetEase(args.Movement.Ease).OnUpdate<Vector3>((v, d, t) =>
                        {
                            if (!isPreview)
                            {
                                if (args.MotionAnimateEndState == MotionAnimateEndState.以_移动为准)
                                {
                                    if (act_on_element_in_progress != null)
                                        act_on_element_in_progress(Tween_Move.CurrentEasedProgress);
                                }
                            }
                        }).OnComplete((d) =>
                        {
                            if (isPreview)
                            {
                                RectTransform.localScale = scale;
                            }
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
                        }).Play();
                    }
                    else
                    {
                        Tween_Move = RectTransform.xt_AnchoredPosition3D_To(endvalue, args.Movement.Duration * duration).SetAutoKill(isPreview ? false : true).SetFrom(fromvalue).SetRelative(true).SetDelay(args.Movement.Delay).SetEase(args.Movement.Ease).OnUpdate<Vector3>((v, d, t) =>
                        {
                            if (!isPreview)
                            {
                                if (args.MotionAnimateEndState == MotionAnimateEndState.以_移动为准)
                                {
                                    if (act_on_element_in_progress != null)
                                        act_on_element_in_progress(Tween_Move.CurrentEasedProgress);
                                }
                            }
                        }).OnComplete((d) =>
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
                        }).OnKill(() =>
                        {
                            if (isPreview)
                            {
                                if (PosSet)
                                    element_PositionSet(ori_pos);
                                else
                                    element_PositionSet(current);
                            }
                        }).Play();
                    }
                }

                ///---动画 - 位移（Curve）
                if (args.Movement.Ease == EaseMode.None)
                {
                    if (DebugState)
                        XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "播放运动动画 - 根据曲线参数", HudMsgState.通知);
                    if (args.Movement.Movement == HudMotion_Movement.W_中心缩放)
                    {
                        Tween_Move = RectTransform.xt_Scale_To(endvalue, args.Movement.Duration * duration).SetAutoKill(isPreview ? false : true).SetFrom(fromvalue).SetDelay(args.Movement.Delay).SetEase(args.Movement.Curve).OnUpdate<Vector3>((v, d, t) =>
                        {
                            if (!isPreview)
                            {
                                if (args.MotionAnimateEndState == MotionAnimateEndState.以_移动为准)
                                {
                                    if (act_on_element_in_progress != null)
                                        act_on_element_in_progress(Tween_Move.CurrentEasedProgress);
                                }
                            }
                        }).OnComplete((d) =>
                        {
                            if (isPreview)
                            {
                                RectTransform.localScale = scale;
                            }
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
                        }).Play();
                    }
                    else
                    {
                        Tween_Move = RectTransform.xt_AnchoredPosition3D_To(endvalue, args.Movement.Duration * duration).SetAutoKill(isPreview ? false : true).SetFrom(fromvalue).SetRelative(true).SetDelay(args.Movement.Delay).SetEase(args.Movement.Curve).OnUpdate<Vector3>((v, d, t) =>
                        {
                            if (!isPreview)
                            {
                                if (args.MotionAnimateEndState == MotionAnimateEndState.以_移动为准)
                                {
                                    if (act_on_element_in_progress != null)
                                        act_on_element_in_progress(Tween_Move.CurrentEasedProgress);
                                }
                            }
                        }).OnComplete((d) =>
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
                        }).OnKill(() =>
                        {
                            if (isPreview)
                            {
                                if (PosSet)
                                    element_PositionSet(ori_pos);
                                else
                                    element_PositionSet(current);
                            }
                        }).Play();
                    }
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
                        //fromvalue = Vector3.zero;
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

                ///---动画 - 旋转（Ease）
                if (args.Rotation.Ease != EaseMode.None)
                {
                    if (DebugState)
                        XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "播放旋转动画 - 根据缓动参数", HudMsgState.通知);
                    Tween_Rotation = RectTransform.xt_Rotate_To(endvalue, args.Rotation.Duration * duration, true, false, XTweenRotationSpace.相对, XTweenRotationMode.Normal).SetRelative(true).SetDelay(args.Rotation.Delay).SetEase(args.Rotation.Ease).SetAutoKill(isPreview ? false : true).OnUpdate<Vector3>((v, d, t) =>
                    {
                        if (!isPreview)
                        {
                            if (args.MotionAnimateEndState == MotionAnimateEndState.以_旋转为准)
                            {
                                if (act_on_element_in_progress != null)
                                    act_on_element_in_progress(Tween_Rotation.CurrentEasedProgress);
                            }
                        }
                    }).OnComplete((d) =>
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
                    }).OnKill(() =>
                    {
                        if (isPreview)
                            RectTransform.localEulerAngles = current;
                    }).Play();
                }

                ///---动画 - 旋转（Curve）
                if (args.Rotation.Ease == EaseMode.None)
                {
                    if (DebugState)
                        XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "播放旋转动画 - 根据曲线参数", HudMsgState.通知);
                    Tween_Rotation = RectTransform.xt_Rotate_To(endvalue, args.Rotation.Duration * duration, true, false, XTweenRotationSpace.绝对, XTweenRotationMode.Normal).SetRelative(true).SetDelay(args.Rotation.Delay).SetEase(args.Rotation.Curve).SetAutoKill(isPreview ? false : true).OnUpdate<Vector3>((v, d, t) =>
                    {
                        if (!isPreview)
                        {
                            if (args.MotionAnimateEndState == MotionAnimateEndState.以_旋转为准)
                            {
                                if (act_on_element_in_progress != null)
                                    act_on_element_in_progress(Tween_Rotation.CurrentEasedProgress);
                            }
                        }
                    }).OnComplete((d) =>
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
                    }).OnKill(() =>
                    {
                        if (isPreview)
                            RectTransform.localEulerAngles = current;
                    }).Play();
                }
            }

            #endregion
        }
        /// <summary>
        /// 剥离的独立动画单元：回收
        /// </summary>
        /// <param name="args"></param>
        /// <param name="act_OutComplete"></param>
        public void ElementTweens_Recycler(Motion_Recycler args, UnityAction act_OutComplete, bool isPreview = false, bool PosSet = false, Vector3 pos = default)
        {
            float duration = isPreview ? XHud_Dashboard.HudManagerGet().DurationMultiply : XHud_Manager.Instance.DurationMultiply * PrimitivesTweenGlobalDuration;
            Vector3 ori_pos = Vector3.zero;

            if (isPreview)
            {
                if (PosSet)
                    element_PositionSet(pos);
                element_AlphaSet(1);
                element_AlphaSyncUpdate();
            }

            #region 动作 - 透明度动画
            ///---动画 - 透明度_Alpha（Ease）
            if (args.Alpha.Ease != EaseMode.None)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "播放透明度动画 - 根据缓动参数", HudMsgState.通知);
                Tween_Alpha = XTween.To(() => Alpha, x => Alpha = x, 0, args.Alpha.Duration * duration, isPreview ? false : true).SetEase(args.Alpha.Ease).SetDelay(args.Alpha.Delay).OnUpdate<float>((v, d, t) =>
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
                    }).OnComplete((d) =>
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
                    }).OnKill(() =>
                    {
                        if (isPreview)
                        {
                            element_AlphaSet(1);
                            element_AlphaSyncUpdate();
                        }
                    }).Play();
            }

            ///---动画 - 透明度_Alpha（Curve）
            if (args.Alpha.Ease == EaseMode.None)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "播放透明度动画 - 根据曲线参数", HudMsgState.通知);
                Tween_Alpha = XTween.To(() => Alpha, x => Alpha = x, 0, args.Alpha.Duration * duration, isPreview ? false : true).SetEase(args.Alpha.Curve).SetDelay(args.Alpha.Delay).OnUpdate<float>((v, d, t) =>
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
                    }).OnComplete((d) =>
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
                    }).OnKill(() =>
                    {
                        if (isPreview)
                        {
                            element_AlphaSet(1);
                            element_AlphaSyncUpdate();
                        }
                    }).Play();
            }
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
                {
                    // 叠加自身原始位置
                    endvalue += current;
                }
                #endregion

                ///---动画 - 位移（Ease）
                if (args.Movement.Ease != EaseMode.None)
                {
                    if (DebugState)
                        XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "播放运动动画 - 根据缓动参数", HudMsgState.通知);
                    if (args.Movement.Movement == HudMotion_Movement.W_中心缩放)
                    {
                        Tween_Move = RectTransform.xt_Scale_To(endvalue, args.Movement.Duration * duration).SetDelay(args.Movement.Delay).SetEase(args.Movement.Ease).SetAutoKill(isPreview ? false : true).OnUpdate<Vector3>((v, d, t) =>
                            {
                                if (!isPreview)
                                {
                                    if (args.MotionAnimateEndState == MotionAnimateEndState.以_移动为准)
                                    {
                                        if (act_on_element_out_progress != null)
                                            act_on_element_out_progress(Tween_Move.CurrentEasedProgress);
                                    }
                                }
                            }).OnComplete((d) =>
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
                            }).OnKill(() =>
                            {
                                if (isPreview)
                                    RectTransform.localScale = scale;
                            }).Play();
                    }
                    else
                    {
                        Tween_Move = RectTransform.xt_AnchoredPosition3D_To(endvalue, args.Movement.Duration * duration).SetRelative(true).SetDelay(args.Movement.Delay).SetEase(args.Movement.Ease).SetAutoKill(isPreview ? false : true).OnStart(() =>
                        {

                        }).OnUpdate<Vector3>((v, d, t) =>
                        {
                            if (!isPreview)
                            {
                                if (args.MotionAnimateEndState == MotionAnimateEndState.以_移动为准)
                                {
                                    if (act_on_element_out_progress != null)
                                        act_on_element_out_progress(Tween_Move.CurrentEasedProgress);
                                }
                            }
                        }).OnComplete((d) =>
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
                        }).OnKill(() =>
                        {
                            if (isPreview)
                            {
                                if (PosSet)
                                    element_PositionSet(ori_pos);
                                else
                                    element_PositionSet(current);
                            }
                        }).Play();
                    }
                }

                ///---动画 - 位移（Curve）
                if (args.Movement.Ease == EaseMode.None)
                {
                    if (DebugState)
                        XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "播放运动动画 - 根据曲线参数", HudMsgState.通知);
                    if (args.Movement.Movement == HudMotion_Movement.W_中心缩放)
                    {
                        Tween_Move = RectTransform.xt_Scale_To(endvalue, args.Movement.Duration * duration).SetDelay(args.Movement.Delay).SetEase(args.Movement.Curve).SetAutoKill(isPreview ? false : true).OnUpdate<Vector3>((v, d, t) =>
                        {
                            if (!isPreview)
                            {
                                if (args.MotionAnimateEndState == MotionAnimateEndState.以_移动为准)
                                {
                                    if (act_on_element_out_progress != null)
                                        act_on_element_out_progress(Tween_Move.CurrentEasedProgress);
                                }
                            }
                        }).OnComplete((d) =>
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
                        }).OnKill(() =>
                        {
                            if (isPreview)
                                RectTransform.localScale = scale;
                        }).Play();
                    }
                    else
                    {
                        Tween_Move = RectTransform.xt_AnchoredPosition3D_To(endvalue, args.Movement.Duration * duration).SetRelative(true).SetDelay(args.Movement.Delay).SetEase(args.Movement.Curve).SetAutoKill(isPreview ? false : true).OnUpdate<Vector3>((v, d, t) =>
                        {
                            if (!isPreview)
                                if (args.MotionAnimateEndState == MotionAnimateEndState.以_移动为准)
                                {
                                    if (act_on_element_out_progress != null)
                                        act_on_element_out_progress(Tween_Move.CurrentEasedProgress);
                                }
                        }).OnComplete((d) =>
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
                        }).OnKill(() =>
                        {
                            if (isPreview)
                            {
                                if (PosSet)
                                    element_PositionSet(ori_pos);
                                else
                                    element_PositionSet(current);
                            }
                        }).Play();
                    }
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

                ///---动画 - 旋转（Ease）
                if (args.Rotation.Ease != EaseMode.None)
                {
                    if (DebugState)
                        XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "播放旋转动画 - 根据缓动参数", HudMsgState.通知);
                    Tween_Rotation = RectTransform.xt_Rotate_To(endvalue, args.Rotation.Duration * duration, true, isPreview ? false : true, XTweenRotationSpace.相对, XTweenRotationMode.Normal).SetRelative(true).SetDelay(args.Rotation.Delay).SetEase(args.Rotation.Ease).SetAutoKill(isPreview ? false : true).OnUpdate<Vector3>((v, d, t) =>
                    {
                        if (!isPreview)
                        {
                            if (args.MotionAnimateEndState == MotionAnimateEndState.以_旋转为准)
                            {
                                if (act_on_element_out_progress != null)
                                    act_on_element_out_progress(Tween_Rotation.CurrentEasedProgress);
                            }
                        }
                    }).OnComplete((d) =>
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
                    }).OnKill(() =>
                    {
                        RectTransform.localEulerAngles = euler;
                    }).Play();
                }

                ///---动画 - 旋转（Curve）
                if (args.Rotation.Ease == EaseMode.None)
                {
                    if (DebugState)
                        XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "播放旋转动画 - 根据曲线参数", HudMsgState.通知);
                    Tween_Rotation = RectTransform.xt_Rotate_To(endvalue, args.Rotation.Duration * duration, true, isPreview ? false : true, XTweenRotationSpace.相对, XTweenRotationMode.Normal).SetRelative(true).SetDelay(args.Rotation.Delay).SetEase(args.Rotation.Curve).SetAutoKill(isPreview ? false : true).OnUpdate<Vector3>((v, d, t) =>
                        {
                            if (!isPreview)
                            {
                                if (args.MotionAnimateEndState == MotionAnimateEndState.以_旋转为准)
                                {
                                    if (act_on_element_out_progress != null)
                                        act_on_element_out_progress(Tween_Rotation.CurrentEasedProgress);
                                }
                            }
                        }).OnComplete((d) =>
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
                        }).OnKill(() =>
                        {
                            RectTransform.localEulerAngles = euler;
                        }).Play();
                }
            }
            #endregion
        }
        #endregion
    }
}