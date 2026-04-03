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
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class XHud_Module_Toggle : Toggle
    {
        public RectTransform RectTransform;
        [SerializeField]
        public string Indicator;
        [SerializeField]
        public bool DebugState;
        [SerializeField]
        public string ToggleName = "ToggleName";
        [SerializeField]
        public XHud_Module_Text ToggleText;
        [SerializeField]
        public XHud_Module_TmpText ToggleTmpText;

        /// <summary>
        /// 动作 - 当 - 开关按下时
        /// </summary>
        public UnityAction act_on_Press;
        /// <summary>
        /// 动作 - 当 - 开关松开时
        /// </summary>
        public UnityAction act_on_Released;
        /// <summary>
        /// 动作 - 当 - 开关打开时
        /// </summary>
        public UnityAction act_on_Checked;
        /// <summary>
        /// 动作 - 当 - 开关关闭时
        /// </summary>
        public UnityAction act_on_UnChecked;
        /// <summary>
        /// 动作 - 当 - 状态改变时
        /// </summary>
        public UnityAction act_on_ValueChanged;
        /// <summary>
        /// 动作 - 当 - 状态改变中时
        /// </summary>
        public UnityAction act_on_ValueChanging;

        /// <summary>
        /// 事件 - 当 - 开关按下时
        /// </summary>
        public UnityEvent eve_on_Press;
        /// <summary>
        /// 事件 - 当 - 开关松开时
        /// </summary>
        public UnityEvent eve_on_Released;
        /// <summary>
        /// 事件 - 当 - 开关打开时
        /// </summary>
        public UnityEvent eve_on_Checked;
        /// <summary>
        /// 事件 - 当 - 开关关闭时
        /// </summary>
        public UnityEvent eve_on_UnChecked;
        /// <summary>
        /// 事件 - 当 - 状态改变时
        /// </summary>
        public UnityEvent eve_on_ValueChanged;
        /// <summary>
        /// 事件 - 当 - 状态改变中时
        /// </summary>
        public UnityEvent eve_on_ValueChanging;

        public Image Tog_Bg;
        public Image Tog_Handle;

        public Color Tog_Color_Bg_Unchecked = new Color(0.282353f, 0.282353f, 0.282353f, 1);
        public Color Tog_Color_Bg_Checked = new Color(0.2313726f, 0.9921569f, 0.6039216f, 1);
        public Color Tog_Color_Handle_Unchecked = new Color(0.8392157f, 0.8392157f, 0.8392157f, 1);
        public Color Tog_Color_Handle_Checked = new Color(0.2156863f, 0.2156863f, 0.2156863f, 1);

        private XTween_Interface Tog_HandleTweener;
        private XTween_Interface Tog_HandleColorTweener;
        private XTween_Interface Tog_BgColorTweener;

        public float Toggle_Animators_GlobalDuration = 1f;
        public List<ElementNode_Animator> ToggleAnimatorNodes = new List<ElementNode_Animator>();

        public bool EaseMotion = true;
        public bool ToggleIsChecked = false;
        public bool BgCanToggle = true;
        public bool TitleCanToggle = true;

        public float ChangingTimer = 0f;
        public float ChangingInterval = 0.05f;

        [Range(0, 1)]
        public float HandleProgress = 0;
        public float HandleProgressDuration = 0.3f;
        public float ColorDuration = 0.3f;
        public EaseMode ProgressEase = EaseMode.OutQuart;
        public EaseMode ColorEase = EaseMode.OutQuart;

        public HudElementAnimateState AnimateState;
        public float ToggleAnimatorMaxDuration;

        public bool ToggleAnimatorListIsFold;
        public bool ToggleEventIsFold;
        public bool ToggleOriginalIsFold;

        public bool AutoStopPreview = true;

        /// <summary>
        /// 控制柄移动范围
        /// </summary>
        public Vector2 HandlePosRange;

        protected override void Awake()
        {
            base.Awake();
            if (RectTransform == null)
                RectTransform = GetComponent<RectTransform>();
        }

        protected override void Start()
        {
            base.Start();
            tog_NameSet();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (Application.isPlaying)
            {
                onValueChanged.AddListener(tog_ValueChanged);
                tog_Reset();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (Application.isPlaying)
            {
                onValueChanged.RemoveAllListeners();
                Animators_Rewind();
                EventsClear();
                ActionsClear();
                tog_Reset(true, true);
            }
        }

        private void Update()
        {
            tog_Update();
        }

        #region 获取动画器和动画节点
        /// <summary>
        /// 获取一个动画器
        /// </summary>
        /// <param tweenName="indicator">目标标识名称</param>
        /// <returns>返回一个匹配标识名称的HudAnimator动画器</returns>
        public XHud_Module_Animator GetAnimator(string indicator)
        {
            XHud_Module_Animator am = null;
            for (int i = 0; i < ToggleAnimatorNodes.Count; i++)
            {
                if (ToggleAnimatorNodes[i].Animator.GetIndicator() == indicator)
                {
                    am = ToggleAnimatorNodes[i].Animator;
                }
            }
            if (am == null)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 开关控件通知", "未获取到标识名为 " + indicator + " 的子级动画器！ ", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 开关控件通知", "已获取子级动画器 " + indicator, HudMsgState.通知);
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
            for (int i = 0; i < ToggleAnimatorNodes.Count; i++)
            {
                if (ToggleAnimatorNodes[i].Animator.gameObject.name == name)
                {
                    am = ToggleAnimatorNodes[i].Animator;
                }
            }
            if (am == null)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 开关控件通知", "未获取到名为 " + name + " 的子级动画器！ ", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 开关控件通知", "已获取子级动画器 " + name, HudMsgState.通知);
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
            for (int i = 0; i < ToggleAnimatorNodes.Count; i++)
            {
                if (ToggleAnimatorNodes[i].Animator.GetID() == id)
                {
                    am = ToggleAnimatorNodes[i].Animator;
                }
            }
            if (am == null)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 开关控件通知", "未获取到索引号为 " + id + " 的子级动画器！ ", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 开关控件通知", "已获取索引号为 " + id + " 子级动画器！", HudMsgState.通知);
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
                    XHud_Utilitys.Func_PrintInfo("XHud - 开关控件通知", "未获取到名为 " + animator_indicator + " 的子级动画器！ ", HudMsgState.错误);
            }
            else
            {
                if (node == null)
                {
                    if (DebugState)
                        XHud_Utilitys.Func_PrintInfo("XHud - 开关控件通知", "已获取子级动画器 " + animator_indicator, HudMsgState.通知);
                }
                else
                {
                    if (DebugState)
                        XHud_Utilitys.Func_PrintInfo("XHud - 开关控件通知", "已获取子级动画器 " + animator_indicator + "，但并未在其中找到索引号为 " + tween_id + " 的动画效果！", HudMsgState.警告);
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
                    XHud_Utilitys.Func_PrintInfo("XHud - 开关控件通知", "未获取到ID为 " + animator_id + " 的子级动画器！ ", HudMsgState.错误);
            }
            else
            {
                if (node == null)
                {
                    if (DebugState)
                        XHud_Utilitys.Func_PrintInfo("XHud - 开关控件通知", "已获取ID为 " + animator_id + " 子级动画器", HudMsgState.通知);
                }
                else
                {
                    if (DebugState)
                        XHud_Utilitys.Func_PrintInfo("XHud - 开关控件通知", "已获取ID为 " + animator_id + " 子级动画器，但并未在其中找到ID号为 " + tween_id + " 的动画节点！", HudMsgState.警告);
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
                    XHud_Utilitys.Func_PrintInfo("XHud - 开关控件通知", "未获取到名为 " + animator_indicator + " 的子级动画器！ ", HudMsgState.错误);
            }
            else
            {
                if (node == null)
                {
                    if (DebugState)
                        XHud_Utilitys.Func_PrintInfo("XHud - 开关控件通知", "已获取子级动画器 " + animator_indicator, HudMsgState.通知);
                }
                else
                {
                    if (DebugState)
                        XHud_Utilitys.Func_PrintInfo("XHud - 开关控件通知", "已获取子级动画器 " + animator_indicator + "，但并未在其中找到名称为 " + tween_indicator + " 的动画效果！", HudMsgState.警告);
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
            for (int i = 0; i < ToggleAnimatorNodes.Count; i++)
            {
                if (ToggleAnimatorNodes[i].Animator.GetID() == ID)
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
            for (int i = 0; i < ToggleAnimatorNodes.Count; i++)
            {
                if (ToggleAnimatorNodes[i].Animator.GetIndicator() == Indicator)
                {
                    isExist = true;
                }
            }
            return isExist;
        }

        /// <summary>
        /// 播放按钮子级中的所有动画
        /// </summary>
        private void Animators_Play(string tim)
        {
            if (ToggleAnimatorNodes == null || ToggleAnimatorNodes.Count <= 0)
                return;

            for (int i = 0; i < ToggleAnimatorNodes.Count; i++)
            {
                XHud_Module_Animator animator = ToggleAnimatorNodes[i].Animator;
                animator.Play(tim, ToggleAnimatorNodes[i].DelayTime, Toggle_Animators_GlobalDuration * animator.Animator_GlobalDuration, true, null, null, 0.5f);
            }

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 开关控件通知", "播放所有动画器动画！", HudMsgState.确认);
        }

        /// <summary>
        /// 播放按钮子级中的指定ID的动画
        /// </summary>
        /// <param tweenName="id">动画节点的ID</param>
        /// <param tweenName="tim">触发动画的时机</param>
        private void Animators_PlayAt(int id, string tim)
        {
            if (ToggleAnimatorNodes == null || ToggleAnimatorNodes.Count <= 0)
                return;

            if (!AnimatorIsExist(id))
                return;

            for (int i = 0; i < ToggleAnimatorNodes.Count; i++)
            {
                if (ToggleAnimatorNodes[i].Animator.GetID() != id)
                    continue;
                XHud_Module_Animator anim = ToggleAnimatorNodes[i].Animator;
                anim.Play(tim, ToggleAnimatorNodes[i].DelayTime, Toggle_Animators_GlobalDuration * anim.Animator_GlobalDuration);
            }

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 开关控件通知", "播放指定ID的动画器的动画！", HudMsgState.确认);
        }

        /// <summary>
        /// 倒退按钮子级中的所有动画
        /// </summary>
        private void Animators_Rewind()
        {
            for (int i = 0; i < ToggleAnimatorNodes.Count; i++)
            {
                XHud_Module_Animator anim = ToggleAnimatorNodes[i].Animator;
                anim.RewindAllTweenNode();
            }
            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 开关控件通知", "复位按钮动画！", HudMsgState.确认);
        }
        #endregion       

        #region 关联原生事件

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (act_on_Press != null)
                act_on_Press();
            eve_on_Press.Invoke();
            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 开关控件通知", "按下开关！", HudMsgState.通知);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            if (act_on_Released != null)
                act_on_Released();
            eve_on_Released.Invoke();
            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 开关控件通知", "松开开关！", HudMsgState.通知);
        }

        #endregion

        #region 事件动作

        /// <summary>
        /// 移除所有事件
        /// </summary>
        public void EventsClear()
        {
            eve_on_Checked.RemoveAllListeners();
            eve_on_UnChecked.RemoveAllListeners();
            eve_on_ValueChanged.RemoveAllListeners();
            eve_on_ValueChanging.RemoveAllListeners();
            eve_on_Press.RemoveAllListeners();
            eve_on_Released.RemoveAllListeners();

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 开关控件通知", "清空所有事件！", HudMsgState.通知);
        }

        /// <summary>
        /// 移除所有事件
        /// </summary>
        public void ActionsClear()
        {
            act_on_Checked = null;
            act_on_UnChecked = null;
            act_on_ValueChanged = null;
            act_on_ValueChanging = null;
            act_on_Press = null;
            act_on_Released = null;

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 开关控件通知", "清空所有委托！", HudMsgState.通知);
        }

        #endregion

        #region 辅助
        /// <summary>
        /// 更新控制柄的位置
        /// </summary>
        private void tog_Update()
        {
            if (Tog_Bg != null)
                Tog_Bg.raycastTarget = BgCanToggle;

            if (ToggleText != null)
                ToggleText.raycastTarget = TitleCanToggle;

            if (ToggleTmpText != null)
                ToggleTmpText.raycastTarget = TitleCanToggle;

            if (Tog_Handle != null)
            {
                Tog_Handle.rectTransform.anchoredPosition = new Vector2(tog_MapValue(), Tog_Handle.rectTransform.anchoredPosition.y);
            }
        }

        /// <summary>
        /// 开关状态同步变化方法
        /// </summary>
        /// <param tweenName="treeState"></param>
        private void tog_ValueChanged(bool State)
        {
            if (State)
            {
                tog_Checked();
                if (act_on_Checked != null)
                    act_on_Checked();
                eve_on_Checked.Invoke();
            }
            else
            {
                tog_UnChecked();
                if (act_on_UnChecked != null)
                    act_on_UnChecked();
                eve_on_UnChecked.Invoke();
            }
            ToggleIsChecked = State;
            if (act_on_ValueChanged != null)
                act_on_ValueChanged();
            eve_on_Checked.Invoke();
        }
        #endregion

        #region 控件控制
        /// <summary>
        /// 打开开关
        /// </summary>
        private void tog_Checked()
        {
            tog_HandleProgressTo(1);
            Animators_Play("开关打开时");
            tog_HandleColorTo(Tog_Color_Handle_Checked);
            tog_BgColorTo(Tog_Color_Bg_Checked);
            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 开关控件通知", "打开开关！", HudMsgState.通知);
        }

        /// <summary>
        /// 关闭开关
        /// </summary>
        private void tog_UnChecked()
        {
            tog_HandleProgressTo(0);
            Animators_Play("开关关闭时");
            tog_HandleColorTo(Tog_Color_Handle_Unchecked);
            tog_BgColorTo(Tog_Color_Bg_Unchecked);
            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 开关控件通知", "关闭开关！", HudMsgState.通知);
        }

        /// <summary>
        /// 设置开关状态
        /// </summary>
        /// <param tweenName="treeState"></param>
        public void tog_SetToggleChecked(bool state)
        {
            isOn = state;
            if (state)
            {
                tog_Checked();
            }
            else
            {
                tog_UnChecked();
            }

        }

        /// <summary>
        /// 设置开关的启用或禁用
        /// </summary>
        /// <param tweenName="treeState"></param>
        public void tog_SetInteractable(bool state)
        {
            interactable = state;

            if (state)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 开关控件通知", "启用开关交互", HudMsgState.通知);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 开关控件通知", "禁用开关交互", HudMsgState.通知);
            }
        }

        /// <summary>
        /// 控制柄进度到
        /// </summary>
        /// <param tweenName="value"></param>
        private void tog_HandleProgressTo(float value)
        {
            if (EaseMotion)
            {
                Tog_HandleTweener = XTween.To(() => HandleProgress, x => HandleProgress = x, value, HandleProgressDuration * XHud_Manager.Instance.DurationMultiply).SetEase(ProgressEase).SetAutoKill(true).OnUpdate<float>((v, d, t) =>
                {
                    ChangingTimer += Time.deltaTime;
                    if (ChangingTimer >= ChangingInterval)
                    {
                        if (act_on_ValueChanging != null)
                            act_on_ValueChanging();
                        eve_on_ValueChanging.Invoke();
                        ChangingTimer = 0f;
                    }
                });
            }
            else
                HandleProgress = value;




            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 开关控件通知", "开关值到：" + HandleProgress, HudMsgState.通知);
        }

        /// <summary>
        /// 重置状态
        /// </summary>
        public void tog_Reset(bool ClearEvents = false, bool ClearAction = false)
        {
            Tog_HandleTweener.Kill();
            Tog_HandleColorTweener.Kill();
            Tog_BgColorTweener.Kill();

            HandleProgress = 0;
            Tog_Handle.color = Tog_Color_Handle_Unchecked;
            Tog_Bg.color = Tog_Color_Bg_Unchecked;
            isOn = false;
            ToggleIsChecked = isOn;

            if (ClearEvents)
                EventsClear();
            if (ClearAction)
                ActionsClear();

            Animators_Rewind();
            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 开关控件通知", "开关已重置！", HudMsgState.通知);
        }

        /// <summary>
        /// 映射位置范围到0-1
        /// </summary>
        /// <returns></returns>
        private float tog_MapValue()
        {
            float mappedValue = (HandleProgress * (HandlePosRange.y - HandlePosRange.x)) + HandlePosRange.x;
            return mappedValue;
        }

        /// <summary>
        /// 设置开关文字名称
        /// </summary>
        /// <param tweenName="val"></param>
        public string tog_NameSet(string val = null)
        {
            string content = "";

            if (string.IsNullOrEmpty(val))
                content = ToggleName;
            else
                content = ToggleName = val;

            if (ToggleTmpText != null)
                ToggleTmpText.text = content;

            if (ToggleText != null)
                ToggleText.text = content;

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 开关控件通知", "设置开关显示名：" + val, HudMsgState.通知);
            return content;
        }

        /// <summary>
        /// 清空开关名称
        /// </summary>
        public void tog_NameClear()
        {
            ToggleName = null;

            if (ToggleTmpText != null)
                ToggleTmpText.text = null;

            if (ToggleText != null)
                ToggleText.text = null;

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 开关控件通知", "清空开关显示名！", HudMsgState.通知);
        }

        /// <summary>
        /// 设置控制柄的移动范围
        /// </summary>
        /// <param tweenName="start"></param>
        /// <param tweenName="end"></param>
        public void tog_SetHandleRange(float start, float end)
        {
            HandlePosRange = new Vector2(start, end);

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 开关控件通知", "设置开关控制柄运动范围为：左极限 - " + start + " 右极限 - " + end, HudMsgState.通知);
        }

        /// <summary>
        /// 设置控制柄的位置
        /// </summary>
        /// <param tweenName="val"></param>
        public void tog_SetHandleProgress(float val)
        {
            HandleProgress = val;

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 开关控件通知", "设置开关控制柄运动进度值为：" + val, HudMsgState.通知);
        }
        #endregion

        #region 可视化控制

        /// <summary>
        /// 控制柄颜色到
        /// </summary>
        /// <param tweenName="value"></param>
        /// <param tweenName="fastmode"></param>
        private void tog_HandleColorTo(Color value, bool fastmode = false)
        {
            if (fastmode)
                Tog_Handle.color = value;
            else
            {
                if (EaseMotion)
                    Tog_HandleColorTweener = Tog_Handle.xt_Color_To(value, ColorDuration * XHud_Manager.Instance.DurationMultiply, true).SetEase(ColorEase).SetAutoKill(true);
                else
                    Tog_Handle.color = value;
            }

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 开关控件通知", "开关控制柄颜色到：" + value, HudMsgState.通知);
        }

        /// <summary>
        /// 背景颜色到
        /// </summary>
        /// <param tweenName="value"></param>
        /// <param tweenName="fastmode"></param>
        private void tog_BgColorTo(Color value, bool fastmode = false)
        {
            if (fastmode)
                Tog_Handle.color = value;
            else
            {
                if (EaseMotion)
                    Tog_BgColorTweener = Tog_Bg.xt_Color_To(value, ColorDuration * XHud_Manager.Instance.DurationMultiply, true).SetEase(ColorEase).SetAutoKill(true);
                else
                    Tog_Bg.color = value;
            }
            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 开关控件通知", "开关背景颜色到：" + value, HudMsgState.通知);
        }

        /// <summary>
        /// 设置控制柄的图像
        /// </summary>
        /// <param tweenName="tex"></param>
        public void tog_SetHandle(Texture2D tex)
        {
            Tog_Handle.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);
            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 开关控件通知", "已设置开关控制柄图像为：" + tex.name, HudMsgState.通知);
        }

        /// <summary>
        /// 设置控制柄的图像
        /// </summary>
        /// <param tweenName="sprite"></param>
        public void tog_SetHandle(Sprite sprite)
        {
            Tog_Handle.sprite = sprite;
            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 开关控件通知", "已设置开关控制柄精灵图集图像为：" + sprite.name, HudMsgState.通知);
        }

        /// <summary>
        /// 设置背景的图像
        /// </summary>
        /// <param tweenName="tex"></param>
        public void tog_SetBg(Texture2D tex)
        {
            Tog_Bg.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 开关控件通知", "设置开关背景图像为：" + tex.name, HudMsgState.通知);
        }

        /// <summary>
        /// 设置背景的图像
        /// </summary>
        /// <param tweenName="sprite"></param>
        public void tog_SetBg(Sprite sprite)
        {
            Tog_Bg.sprite = sprite;

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 开关控件通知", "设置开关背景精灵图集图像为：" + sprite.name, HudMsgState.通知);
        }

        #endregion
    }
}