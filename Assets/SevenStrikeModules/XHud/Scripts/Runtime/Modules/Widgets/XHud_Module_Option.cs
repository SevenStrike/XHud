/*
 * ============================================================================
 * ⚠ 版权声明（禁止删除、禁止修改、衍生作品必须保留此注释）⚠
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
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    public class XHud_Module_Option : MonoBehaviour
    {
        public bool DebugState;
        public float AnimatorsMaxDuration;
        public string Indicator;

        #region 节点组件
        public RectTransform SelectorMark;
        public RectTransform OptionRoot;
        #endregion

        public XHudElementAnimateState AnimateState;

        public Vector3 Pos_Destination;
        public Vector3 Prev_Pos_Destination;
        public Vector3 sm_Pos_Destination;

        public bool RepeatAnimatorPlay = true;

        #region 运动动态参数
        public bool UseBlinked;
        private XTween_Interface Tween_Motion;
        public float TweenSpeed = 1;
        public EaseMode SelectorTweenMotion;
        public bool UseEaseMotion = true;
        public float LerpSpeed = 1;
        #endregion

        /// <summary>
        /// 动画器集合
        /// </summary>
        public List<ElementNode_Animator> SelectorAnimatorNodes = new List<ElementNode_Animator>();
        /// <summary>
        /// 选项按钮集合
        /// </summary>
        public List<ElementNode_OptionButton> OptionButtonNodes = new List<ElementNode_OptionButton>();

        #region 事件动作
        /// <summary>
        /// 动作 - 当选项选择器改变位置时
        /// </summary>
        public UnityAction<Vector3, RectTransform> act_on_selector_position_changed;
        /// <summary>
        /// 动作 - 当选项选择器改变位置开始时
        /// </summary>
        public UnityAction<Vector3, RectTransform> act_on_selector_position_started;
        /// <summary>
        /// 动作 - 当选项选择器改变位置完成时
        /// </summary>
        public UnityAction<Vector3, RectTransform> act_on_selector_position_complete;
        /// <summary>
        /// 动作 - 选择选项时 - 传递选项的索引号和标识名称以及位置
        /// </summary>
        public UnityAction<int, string, Vector3> act_on_option_clicked;
        /// <summary>
        /// 动作 - 选择选项时 - 传递选项的标识名称
        /// </summary>
        public UnityAction<string> act_on_option_clicked_with_indicator;
        /// <summary>
        /// 动作 - 选择选项时 - 传递选项的索引号
        /// </summary>
        public UnityAction<int> act_on_option_clicked_with_index;
        /// <summary>
        /// 动作 - 选择选项时 - 传递选项的位置
        /// </summary>
        public UnityAction<Vector3> act_on_option_clicked_with_position;

        /// <summary>
        /// 事件 - 当选项选择器改变位置时
        /// </summary>
        public UnityEvent<Vector3, RectTransform> eve_on_selector_position_changed;
        /// <summary>
        /// 事件 - 当选项选择器改变位置开始时
        /// </summary>
        public UnityEvent<Vector3, RectTransform> eve_on_selector_position_started;
        /// <summary>
        /// 事件 - 当选项选择器改变位置完成时
        /// </summary>
        public UnityEvent<Vector3, RectTransform> eve_on_selector_position_complete;
        /// <summary>
        /// 事件 - 选择选项时 - 依据索引号和选项标识名称
        /// </summary>
        public UnityEvent<int, string, Vector3> eve_on_option_clicked;
        /// <summary>
        /// 事件 - 选择选项时 - 依据选项标识名称
        /// </summary>
        public UnityEvent<string> eve_on_option_clicked_with_indicator;
        /// <summary>
        /// 事件 - 选择选项时 - 依据选项索引号
        /// </summary>
        public UnityEvent<int> eve_on_option_clicked_with_index;
        /// <summary>
        /// 事件 - 选择选项时 - 传递选项的位置
        /// </summary>
        public UnityEvent<Vector3> eve_on_option_clicked_with_position;
        #endregion

        /// <summary>
        /// 所有动画器的动画速度倍乘系数
        /// </summary>
        public float OptionSelector_Animators_GlobalDuration = 1;

        #region 选中的信息
        public int OptionIndex;
        public string CurrentOptionName;
        #endregion

        public bool AnimatorsIsFold;
        public bool ButtonIsFold;
        public bool EventIsFold;
        public bool AutoStopPreview = true;

        #region 光标偏移
        /// <summary>
        /// 实际的光标偏移
        /// </summary>
        public Vector3 SelectorOffset;
        /// <summary>
        /// 用于编辑器调试的光标偏移
        /// </summary>
        public Vector3 SelectorOffsetAdded;
        #endregion

        private void OnDisable()
        {
            if (Tween_Motion != null)
                Tween_Motion.Kill();
            Animators_Rewind();
            opt_ClearIndexInfo();
        }

        private void OnEnable()
        {
            Animators_Play();
        }

        private void Start()
        {
        }

        void Update()
        {
            opt_UpdateOption();
        }

        private void opt_UpdateOption()
        {
            if (!UseEaseMotion)
            {
                if (SelectorMark != null)
                {
                    sm_Pos_Destination = Vector3.Lerp(sm_Pos_Destination, Pos_Destination, Time.deltaTime * LerpSpeed);
                    SelectorMark.anchoredPosition3D = new Vector3(sm_Pos_Destination.x, sm_Pos_Destination.y, sm_Pos_Destination.z) + SelectorOffset;
                }
            }
            else
            {
                if (SelectorMark != null)
                    SelectorMark.anchoredPosition3D = new Vector3(Pos_Destination.x, Pos_Destination.y, Pos_Destination.z) + SelectorOffset;
            }

            Prev_Pos_Destination = Pos_Destination;
        }

        #region 事件动作

        public void opt_ClearActions()
        {
            act_on_selector_position_changed = null;
            act_on_selector_position_started = null;
            act_on_selector_position_complete = null;
            act_on_option_clicked = null;
            act_on_option_clicked_with_indicator = null;
            act_on_option_clicked_with_index = null;
            act_on_option_clicked_with_position = null;

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 选项控件通知", "清空所有委托！", HudMsgState.通知);
        }

        public void opt_ClearEvents()
        {
            eve_on_selector_position_changed.RemoveAllListeners();
            eve_on_selector_position_started.RemoveAllListeners();
            eve_on_selector_position_complete.RemoveAllListeners();
            eve_on_option_clicked.RemoveAllListeners();
            eve_on_option_clicked_with_indicator.RemoveAllListeners();
            eve_on_option_clicked_with_index.RemoveAllListeners();
            eve_on_option_clicked_with_position.RemoveAllListeners();

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 选项控件通知", "清空所有事件！", HudMsgState.通知);
        }

        #endregion

        #region 选项动作
        /// <summary>
        /// 选择选项并调用委托
        /// </summary>
        /// <param tweenName="index"></param>
        public void opt_Select(int index)
        {
            if (SelectorMark == null)
                return;
            if (!RepeatAnimatorPlay)
                if (OptionIndex == index)
                {
                    return;
                }

            Selector_Animators_Rewind();

            OptionIndex = index;
            CurrentOptionName = OptionButtonNodes[index].Button.Indicator;

            Vector3 pos = OptionRoot.parent.InverseTransformPoint(OptionButtonNodes[index].Button.RectTransform.position);

            Selector_Animators_Play("点击选项");
            Animators_Play("点击选项");

            opt_SetSelectorPosition(pos);

            if (act_on_option_clicked != null)
                act_on_option_clicked(OptionIndex, CurrentOptionName, pos);
            eve_on_option_clicked.Invoke(OptionIndex, CurrentOptionName, pos);

            if (act_on_option_clicked_with_indicator != null)
                act_on_option_clicked_with_indicator(CurrentOptionName);
            eve_on_option_clicked_with_indicator.Invoke(CurrentOptionName);

            if (act_on_option_clicked_with_index != null)
                act_on_option_clicked_with_index(OptionIndex);
            eve_on_option_clicked_with_index.Invoke(OptionIndex);

            if (act_on_option_clicked_with_position != null)
                act_on_option_clicked_with_position(pos);
            eve_on_option_clicked_with_position.Invoke(pos);

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 选项控件通知", "点击了选项：" + CurrentOptionName, HudMsgState.通知);
        }

        /// <summary>
        /// 选择选项并调用委托
        /// </summary>
        /// <param tweenName="index"></param>
        public void opt_Select(string indicator)
        {
            if (SelectorMark == null)
                return;

            if (!RepeatAnimatorPlay)
                if (CurrentOptionName == indicator)
                {
                    return;
                }

            Selector_Animators_Rewind();

            OptionIndex = opt_GetIndex_WithButtonIndicator(indicator);
            CurrentOptionName = indicator;

            Vector3 pos = OptionRoot.parent.InverseTransformPoint(opt_GetRectTransform_WithButtonIndicator(indicator).position);

            Selector_Animators_Play("点击选项");
            Animators_Play("点击选项");

            opt_SetSelectorPosition(pos);

            if (act_on_option_clicked != null)
                act_on_option_clicked(OptionIndex, CurrentOptionName, pos);
            eve_on_option_clicked.Invoke(OptionIndex, CurrentOptionName, pos);

            if (act_on_option_clicked_with_indicator != null)
                act_on_option_clicked_with_indicator(CurrentOptionName);
            eve_on_option_clicked_with_indicator.Invoke(CurrentOptionName);

            if (act_on_option_clicked_with_index != null)
                act_on_option_clicked_with_index(OptionIndex);
            eve_on_option_clicked_with_index.Invoke(OptionIndex);

            if (act_on_option_clicked_with_position != null)
                act_on_option_clicked_with_position(pos);
            eve_on_option_clicked_with_position.Invoke(pos);

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 选项控件通知", "点击了选项：" + CurrentOptionName, HudMsgState.通知);
        }

        /// <summary>
        /// 只设置选项的光标的位置和当前选项器的选中信息
        /// </summary>
        /// <param tweenName="indicator"></param>
        public void opt_Select(string indicator, bool playanimator = true)
        {
            if (SelectorMark == null)
                return;
            if (SelectorMark == null)
                return;

            OptionIndex = opt_GetIndex_WithButtonIndicator(indicator);
            CurrentOptionName = indicator;

            Vector3 pos = OptionRoot.parent.InverseTransformPoint(opt_GetRectTransform_WithButtonIndicator(indicator).position);
            opt_SetSelectorPosition_Fast(pos);

            #region 改变
            if (act_on_selector_position_changed != null)
                act_on_selector_position_changed(Pos_Destination, SelectorMark);
            eve_on_selector_position_changed.Invoke(Pos_Destination, SelectorMark);
            #endregion

            if (playanimator)
                Animators_Play("光标位置改变");

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 选项控件通知", "已将光标移动到标识为：" + indicator + " 的选项按钮上！", HudMsgState.通知);
        }

        /// <summary>
        /// 清空选项按钮的选择信息
        /// </summary>
        public void opt_ClearIndexInfo()
        {
            OptionIndex = 0;
            CurrentOptionName = "";

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 选项控件通知", "选项信息复位", HudMsgState.通知);
        }
        #endregion

        #region 辅助
        /// <summary>
        /// 设置选项器的光标切换方式
        /// </summary>
        /// <param tweenName="treeState">True为闪现，False为平滑运动</param>
        public void opt_SetBlinkedMode(bool state)
        {
            UseBlinked = state;
        }

        /// <summary>
        /// 设置选项器的光标运动方式
        /// </summary>
        /// <param tweenName="treeState">True为Ease缓动，False为差值平滑运动</param>
        public void opt_SetMotionType(bool state)
        {
            UseEaseMotion = state;
        }

        /// <summary>
        /// 当选项器的运动方式为缓动时，此方法设置缓动方式和耗时
        /// </summary>
        /// <param tweenName="treeState">True为Ease缓动，False为差值平滑运动</param>
        public void opt_SetMotionEase(EaseMode ease, float dur)
        {
            SelectorTweenMotion = ease;
            TweenSpeed = dur;
        }

        /// <summary>
        /// 当选项器的运动方式为差值平滑时，此方法设置差值平滑耗时
        /// </summary>
        /// <param tweenName="treeState">True为Ease缓动，False为差值平滑运动</param>
        public void opt_SetMotionLerp(float dur)
        {
            LerpSpeed = dur;
        }

        /// <summary>
        /// 设置光标位置
        /// </summary>
        /// <param tweenName="pos"></param>
        private void opt_SetSelectorPosition(Vector3 pos)
        {
            if (!UseBlinked)
            {
                if (!UseEaseMotion)
                {
                    #region 平滑差值
                    Pos_Destination = pos;
                    #endregion
                }
                else
                {
                    #region 缓动参数
                    if (Tween_Motion != null && Tween_Motion.IsActive)
                        if (Tween_Motion.IsPlaying)
                            Tween_Motion.Kill();
                    Tween_Motion = XTween.To(() => Pos_Destination, x => Pos_Destination = x, pos, TweenSpeed).SetEase(SelectorTweenMotion).SetAutoKill(true).OnStart(() =>
                    {
                        if (act_on_selector_position_started != null)
                            act_on_selector_position_started(Pos_Destination, SelectorMark);
                        eve_on_selector_position_started.Invoke(Pos_Destination, SelectorMark);
                        Animators_Play("光标移动开始");
                    }).OnComplete((d) =>
                    {
                        if (act_on_selector_position_complete != null)
                            act_on_selector_position_complete(pos, SelectorMark);
                        eve_on_selector_position_complete.Invoke(pos, SelectorMark);
                        Animators_Play("光标移动结束");
                    });
                    sm_Pos_Destination = pos;
                    #endregion
                }

                #region 改变
                if (act_on_selector_position_changed != null)
                    act_on_selector_position_changed(pos, SelectorMark);
                eve_on_selector_position_changed.Invoke(pos, SelectorMark);
                #endregion

                Animators_Play("光标位置改变");

                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 选项控件通知", "更改了光标位置（平滑移动）", HudMsgState.通知);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 选项控件通知", "更改了光标位置（闪现移动）", HudMsgState.通知);
                opt_SetSelectorPosition_Fast(pos);
            }
        }

        /// <summary>
        /// 快速设置光标位置
        /// </summary>
        /// <param tweenName="pos"></param>
        private void opt_SetSelectorPosition_Fast(Vector3 pos)
        {
            #region 设置
            Pos_Destination = pos;
            sm_Pos_Destination = Pos_Destination;
            #endregion

            #region 改变
            if (act_on_selector_position_changed != null)
                act_on_selector_position_changed(pos, SelectorMark);
            eve_on_selector_position_changed.Invoke(pos, SelectorMark);
            #endregion

            Animators_Play("光标位置改变");

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 选项控件通知", "快速更改光标位置！", HudMsgState.通知);
        }

        /// <summary>
        /// 获取按钮列表中所有类别为选项性质的按钮
        /// </summary>
        /// <returns></returns>
        public XHud_Module_Button[] opt_GetButtons()
        {
            List<XHud_Module_Button> OptionalButtons = new List<XHud_Module_Button>();

            for (int i = 0; i < OptionButtonNodes.Count; i++)
            {
                OptionalButtons.Add(OptionButtonNodes[i].Button);
            }

            if (OptionButtonNodes.Count <= 0)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 选项控件通知", "获取的选项按钮列表为空！", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 选项控件通知", "已获取到" + OptionButtonNodes.Count + " 个选项按钮！", HudMsgState.通知);
            }

            return OptionalButtons.ToArray();
        }

        /// <summary>
        /// 获取按钮列表中所有类别为选项性质的按钮标识名称
        /// </summary>
        /// <returns></returns>
        public string[] opt_GetOptionButtonNames()
        {
            List<string> Names = new List<string>();

            for (int i = 0; i < OptionButtonNodes.Count; i++)
            {
                if (OptionButtonNodes[i].IsOptional)
                    Names.Add(OptionButtonNodes[i].Button.Indicator);
            }

            if (OptionButtonNodes.Count <= 0)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 选项控件通知", "获取的选项按钮列表为空！", HudMsgState.错误);
            }
            else
            {
                string names = "";
                for (int i = 0; i < Names.Count; i++)
                {
                    names += Names[i] + " | ";
                }
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 选项控件通知", "已获取到" + OptionButtonNodes.Count + " 个选项按钮名称，分别是：" + names, HudMsgState.通知);
            }

            return Names.ToArray();
        }

        /// <summary>
        /// 根据标识名称获取对应的按钮索引号
        /// </summary>
        /// <param tweenName="indicator"></param>
        /// <returns></returns>
        private int opt_GetIndex_WithButtonIndicator(string indicator)
        {
            if (OptionButtonNodes.Count <= 0)
                return 0;

            int x = 0;
            for (int i = 0; i < OptionButtonNodes.Count; i++)
            {
                if (OptionButtonNodes[i].Indicator == indicator)
                {
                    x = i;
                    break;
                }
            }
            return x;
        }

        /// <summary>
        /// 根据标识名称获取对应的按钮的RectTransform
        /// </summary>
        /// <param tweenName="indicator"></param>
        /// <returns></returns>
        private RectTransform opt_GetRectTransform_WithButtonIndicator(string indicator)
        {
            if (OptionButtonNodes.Count <= 0)
                return null;

            RectTransform x = null;
            for (int i = 0; i < OptionButtonNodes.Count; i++)
            {
                if (OptionButtonNodes[i].Indicator == indicator)
                {
                    x = OptionButtonNodes[i].Button.RectTransform;
                    break;
                }
            }
            return x;
        }
        #endregion

        #region 获取动画器和动画节点

        /// <summary>
        /// 获取一个动画器
        /// </summary>
        /// <param tweenName="indicator">目标标识名称</param>
        /// <returns>返回一个匹配标识名称的HudAnimator动画器</returns>
        public XHud_Module_Animator GetAnimator(string indicator)
        {
            XHud_Module_Animator am = null;
            for (int i = 0; i < SelectorAnimatorNodes.Count; i++)
            {
                if (SelectorAnimatorNodes[i].Animator.GetIndicator() == indicator)
                {
                    am = SelectorAnimatorNodes[i].Animator;
                }
            }
            if (am == null)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 选项控件通知", "未获取到标识名为 " + indicator + " 的子级动画器！ ", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 选项控件通知", "已获取子级动画器 " + indicator, HudMsgState.通知);
            }
            return am;
        }

        /// <summary>
        /// 获取一个动画器
        /// </summary>
        /// <param tweenName="name">目标物体名称</param>
        /// <returns>返回一个匹配物体名称名称的HudAnimator动画器</returns>
        public XHud_Module_Animator GetAnimator_WithObjectName(string name)
        {
            XHud_Module_Animator am = null;
            for (int i = 0; i < SelectorAnimatorNodes.Count; i++)
            {
                if (SelectorAnimatorNodes[i].Animator.gameObject.name == name)
                {
                    am = SelectorAnimatorNodes[i].Animator;
                }
            }
            if (am == null)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 选项控件通知", "未获取到名为 " + name + " 的子级动画器！ ", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 选项控件通知", "已获取子级动画器 " + name, HudMsgState.通知);
            }
            return am;
        }

        /// <summary>
        /// 获取一个动画器
        /// </summary>
        /// <param tweenName="id">目标动画器的ID</param>
        /// <returns>返回一个匹配ID的HudAnimator动画器</returns>
        public XHud_Module_Animator GetAnimator(int id)
        {
            XHud_Module_Animator am = null;
            for (int i = 0; i < SelectorAnimatorNodes.Count; i++)
            {
                if (SelectorAnimatorNodes[i].Animator.GetID() == id)
                {
                    am = SelectorAnimatorNodes[i].Animator;
                }
            }
            if (am == null)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 选项控件通知", "未获取到索引号为 " + id + " 的子级动画器！ ", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 选项控件通知", "已获取索引号为 " + id + " 子级动画器！", HudMsgState.通知);
            }
            return am;
        }

        /// <summary>
        /// 获取一个目标动画器上的目标动画节点
        /// </summary>
        /// <param tweenName="animator_indicator">目标动画器名称</param>
        /// <param tweenName="tween_id">目标动画节点的ID</param>
        /// <returns></returns>
        public TweenNode GetAnimatorTween(string animator_indicator, int tween_id)
        {
            XHud_Module_Animator anim = GetAnimator(animator_indicator);
            TweenNode node = anim.TweenNode_GetByID(tween_id);

            if (anim == null)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 选项控件通知", "未获取到名为 " + animator_indicator + " 的子级动画器！ ", HudMsgState.错误);
            }
            else
            {
                if (node == null)
                {
                    if (DebugState)
                        XHud_Utilitys.Func_PrintInfo("XHud - 选项控件通知", "已获取子级动画器 " + animator_indicator, HudMsgState.通知);
                }
                else
                {
                    if (DebugState)
                        XHud_Utilitys.Func_PrintInfo("XHud - 选项控件通知", "已获取子级动画器 " + animator_indicator + "，但并未在其中找到索引号为 " + tween_id + " 的动画效果！", HudMsgState.警告);
                }
            }

            return node;
        }

        /// <summary>
        /// 获取一个目标动画器上的目标动画节点
        /// </summary>
        /// <param tweenName="animator_id">目标动画器ID</param>
        /// <param tweenName="tween_id">目标动画节点的ID</param>
        /// <returns></returns>
        public TweenNode GetAnimatorTween(int animator_id, int tween_id)
        {
            XHud_Module_Animator anim = GetAnimator(animator_id);
            TweenNode node = anim.TweenNode_GetByID(tween_id);

            if (anim == null)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 选项控件通知", "未获取到ID为 " + animator_id + " 的子级动画器！ ", HudMsgState.错误);
            }
            else
            {
                if (node == null)
                {
                    if (DebugState)
                        XHud_Utilitys.Func_PrintInfo("XHud - 选项控件通知", "已获取ID为 " + animator_id + " 子级动画器", HudMsgState.通知);
                }
                else
                {
                    if (DebugState)
                        XHud_Utilitys.Func_PrintInfo("XHud - 选项控件通知", "已获取ID为 " + animator_id + " 子级动画器，但并未在其中找到ID号为 " + tween_id + " 的动画节点！", HudMsgState.警告);
                }
            }

            return node;
        }

        /// <summary>
        /// 获取一个目标动画器上的目标动画节点
        /// </summary>
        /// <param tweenName="animator_indicator">目标动画器名称</param>
        /// <param tweenName="tween_indicator">目标动画节点的名称</param>
        /// <returns></returns>
        public TweenNode GetAnimatorTween(string animator_indicator, string tween_indicator)
        {
            XHud_Module_Animator anim = GetAnimator(animator_indicator);
            TweenNode node = anim.TweenNode_GetByIndicator(tween_indicator);

            if (anim == null)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 选项控件通知", "未获取到名为 " + animator_indicator + " 的子级动画器！ ", HudMsgState.错误);
            }
            else
            {
                if (node == null)
                {
                    if (DebugState)
                        XHud_Utilitys.Func_PrintInfo("XHud - 选项控件通知", "已获取子级动画器 " + animator_indicator, HudMsgState.通知);
                }
                else
                {
                    if (DebugState)
                        XHud_Utilitys.Func_PrintInfo("XHud - 选项控件通知", "已获取子级动画器 " + animator_indicator + "，但并未在其中找到名称为 " + tween_indicator + " 的动画效果！", HudMsgState.警告);
                }
            }

            return node;
        }

        #endregion

        #region 动画器播放与倒退
        /// <summary>
        /// 验证是否存在指定ID的动画器
        /// </summary>
        /// <returns></returns>
        public bool AnimatorIsExist(int ID)
        {
            bool isExist = false;
            for (int i = 0; i < SelectorAnimatorNodes.Count; i++)
            {
                if (SelectorAnimatorNodes[i].Animator.GetID() == ID)
                {
                    isExist = true;
                }
            }
            return isExist;
        }

        /// <summary>
        /// 验证是否存在指定昵称的动画器
        /// </summary>
        /// <returns></returns>
        public bool AnimatorIsExist(string Indicator)
        {
            bool isExist = false;
            for (int i = 0; i < SelectorAnimatorNodes.Count; i++)
            {
                if (SelectorAnimatorNodes[i].Animator.GetIndicator() == Indicator)
                {
                    isExist = true;
                }
            }
            return isExist;
        }

        /// <summary>
        /// 播放选项器子级中的所有动画，不包含光标动画
        /// </summary>
        /// <param tweenName="tim">点击选项 | 光标移动开始 | 光标移动结束 | 光标位置改变</param>
        public void Animators_Play(string tim = "无")
        {
            if (SelectorAnimatorNodes == null || SelectorAnimatorNodes.Count <= 0)
                return;
            for (int i = 0; i < SelectorAnimatorNodes.Count; i++)
            {
                ElementNode_Animator node = SelectorAnimatorNodes[i];
                int v = node.Animator.mod_RectTransform.GetInstanceID();
                int x = SelectorMark.GetInstanceID();
                if (v == x)
                {
                    continue;
                }
                node.Animator.Play(tim, node.DelayTime, node.Animator.Animator_GlobalDuration * OptionSelector_Animators_GlobalDuration);
            }
            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 选项控件通知", "播放选项的子级动画！", HudMsgState.通知);
        }

        /// <summary>
        /// 播放按钮子级中的指定ID的动画
        /// </summary>
        /// <param tweenName="id">动画节点的ID</param>
        /// <param tweenName="tim">触发动画的时机</param>
        private void Animators_PlayAt(int id, string tim)
        {
            if (SelectorAnimatorNodes == null || SelectorAnimatorNodes.Count <= 0)
                return;

            if (!AnimatorIsExist(id))
                return;

            for (int i = 0; i < SelectorAnimatorNodes.Count; i++)
            {
                if (SelectorAnimatorNodes[i].Animator.GetID() != id)
                    continue;
                XHud_Module_Animator anim = SelectorAnimatorNodes[i].Animator;
                anim.Play(tim, SelectorAnimatorNodes[i].DelayTime, OptionSelector_Animators_GlobalDuration * anim.Animator_GlobalDuration);
            }

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 选项控件通知", "播放指定ID的动画器的动画！", HudMsgState.确认);
        }

        /// <summary>
        /// 倒退选项器子级中的所有动画，不包含光标动画
        /// </summary>
        /// <param tweenName="IncludeSelectorMark">忽略光标的动画倒退</param>
        public void Animators_Rewind()
        {
            for (int i = 0; i < SelectorAnimatorNodes.Count; i++)
            {
                ElementNode_Animator node = SelectorAnimatorNodes[i];
                int v = node.Animator.mod_RectTransform.GetInstanceID();
                int x = SelectorMark.GetInstanceID();
                if (v == x)
                {
                    continue;
                }
                node.Animator.RewindAllTweenNode();
            }
            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 选项控件通知", "倒退复位选项的子级动画！", HudMsgState.通知);
        }

        //--------------光标动画独立控制

        /// <summary>
        /// 播放光标动画
        /// </summary>
        /// <param tweenName="tim">点击选项 | 光标移动开始 | 光标移动结束 | 光标位置改变</param>
        public void Selector_Animators_Play(string tim)
        {
            for (int i = 0; i < SelectorAnimatorNodes.Count; i++)
            {
                int v = SelectorAnimatorNodes[i].Animator.mod_RectTransform.GetInstanceID();
                int x = SelectorMark.GetInstanceID();
                if (v == x)
                {
                    SelectorAnimatorNodes[i].Animator.Play(tim, SelectorAnimatorNodes[i].DelayTime, SelectorAnimatorNodes[i].Animator.Animator_GlobalDuration * OptionSelector_Animators_GlobalDuration);
                    break;
                }
            }
            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 选项控件通知", "倒退复位选项的光标动画！", HudMsgState.通知);
        }

        /// <summary>
        /// 倒退光标动画
        /// </summary>
        /// <param tweenName="IncludeSelectorMark">忽略光标的动画倒退</param>
        public void Selector_Animators_Rewind()
        {
            for (int i = 0; i < SelectorAnimatorNodes.Count; i++)
            {
                int v = SelectorAnimatorNodes[i].Animator.mod_RectTransform.GetInstanceID();
                int x = SelectorMark.GetInstanceID();
                if (v == x)
                {
                    SelectorAnimatorNodes[i].Animator.RewindAllTweenNode();
                    break;
                }
            }
            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 选项控件通知", "倒退复位选项的光标动画！", HudMsgState.通知);
        }
        #endregion       
    }
}