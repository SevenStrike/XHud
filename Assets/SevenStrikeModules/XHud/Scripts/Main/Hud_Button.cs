namespace SevenStrikeModules.XHud.Hud
{
    using DG.Tweening;
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.Utilitys;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    /// <summary>    
    /// Hud按钮
    /// </summary>    
    public class Hud_Button : Button
    {
        public Hud_Option HudOption;

        public RectTransform RectTransform;
        [SerializeField]
        public string Indicator;
        [SerializeField]
        public bool DebugState;
        [SerializeField]
        public bool IsOptionButton;
        [SerializeField]
        public string ButtonName = "BtnName";
        [SerializeField]
        public Hud_Text ButtonText;
        [SerializeField]
        public Image IconImage;
        [SerializeField]
        public Image BgImage;
        [SerializeField]
        public Hud_TmpText ButtonTmpText;

        [SerializeField]
        public Hud_ButtonAction HudButtonState = Hud_ButtonAction.待命;
        [SerializeField]
        public Hud_ButtonAction HudButtonState_Clicked = Hud_ButtonAction.待命;
        [SerializeField]
        public Hud_ButtonAction HudButtonState_Selector = Hud_ButtonAction.待命;
        [SerializeField]
        public Hud_ButtonAction HudButtonState_Holder = Hud_ButtonAction.待命;
        [SerializeField]
        public Hud_ButtonAction HudButtonState_Pressed = Hud_ButtonAction.待命;
        [SerializeField]
        public Hud_ButtonAction HudButtonState_LongPressed = Hud_ButtonAction.待命;

        #region UnityAction
        /// <summary>
        /// 动作 - 当 - 取消选择时
        /// </summary>
        public UnityAction<Hud_Button, Hud_ButtonAction> act_on_DeSelect;
        /// <summary>
        /// 动作 - 当 - 被选择时
        /// </summary>
        public UnityAction<Hud_Button, Hud_ButtonAction> act_on_Select;
        /// <summary>
        /// 动作 - 当 - 鼠标点击时
        /// </summary>
        public UnityAction<Hud_Button, Hud_ButtonAction> act_on_Clicked;
        /// <summary>
        /// 动作 - 当 - 鼠标按下时
        /// </summary>
        public UnityAction<Hud_Button, Hud_ButtonAction> act_on_Press;
        /// <summary>
        /// 动作 - 当 - 鼠标进入时
        /// </summary>
        public UnityAction<Hud_Button, Hud_ButtonAction> act_on_Enter;
        /// <summary>
        /// 动作 - 当 - 鼠标退出时
        /// </summary>
        public UnityAction<Hud_Button, Hud_ButtonAction> act_on_Exit;
        /// <summary>
        /// 动作 - 当 - 鼠标抬起时
        /// </summary>
        public UnityAction<Hud_Button, Hud_ButtonAction> act_on_Release;
        /// <summary>
        /// 动作 - 当 - 鼠标长按时
        /// </summary>
        public UnityAction<Hud_Button, Hud_ButtonAction> act_on_LongPressed;
        /// <summary>
        /// 动作 - 当 - 鼠标长按时 - 过程
        /// </summary>
        public UnityAction<Hud_Button, float> act_on_LongPressPer;
        #endregion

        #region UnityEvent        
        /// <summary>
        /// 事件 - 当 - 取消选择时
        /// </summary>
        public UnityEvent eve_on_Deselect;        
        /// <summary>
        /// 事件 - 当 - 被选择时
        /// </summary>
        public UnityEvent eve_on_Select;        
        /// <summary>
        /// 事件 - 当 - 鼠标点击时
        /// </summary>
        public UnityEvent eve_on_Click;        
        /// <summary>
        /// 事件 - 当 - 鼠标按下时
        /// </summary>
        public UnityEvent eve_on_Press;        
        /// <summary>
        /// 事件 - 当 - 鼠标进入时
        /// </summary>
        public UnityEvent eve_on_Enter;        
        /// <summary>
        /// 事件 - 当 - 鼠标退出时
        /// </summary>
        public UnityEvent eve_on_Exit;        
        /// <summary>
        /// 事件 - 当 - 鼠标抬起时
        /// </summary>
        public UnityEvent eve_on_Release;        
        /// <summary>
        /// 事件 - 当 - 鼠标长按时
        /// </summary>
        public UnityEvent eve_on_Longpressed;        
        /// <summary>
        /// 事件 - 当 - 鼠标长按时-过程
        /// </summary>
        public UnityEvent eve_on_LongpressPer;
        #endregion

        /// <summary>
        /// 长按计时
        /// </summary>
        public float LongPress_Tick = 0;
        /// <summary>
        /// 长按计时步进值
        /// </summary>
        public float LongPress_Step = 1;
        /// <summary>
        /// 长按阈值极限
        /// </summary>
        public float LongPress_Threshold = 200;
        [Range(0, 1)]
        /// <summary>
        /// 长按进度百分比
        /// </summary>
        public float LongPress_Percent;
        /// <summary>
        /// 长按松开时平滑倒退
        /// </summary>
        public bool LongPress_SmoothRewind;
        /// <summary>
        /// 长按进度到达阈值自动倒退
        /// </summary>
        public bool LongPress_UseRewind;
        [SerializeField]
        /// <summary>
        /// 是否长按中
        /// </summary>
        private bool IsLongPress;
        /// <summary>
        /// 动画状态
        /// </summary>
        public HudElementAnimateState AnimateState = HudElementAnimateState.Static;
        bool LongPressIsInvoke;
        public float Button_Animators_GlobalDuration = 1;
        public List<AnimatorNode> BtnAnimatorNodes = new List<AnimatorNode>();

        /// <summary>
        /// 动画列表中筛选出的最大时长
        /// </summary>
        public float AnimatorsMaxDuration;

        /// <summary>
        /// 按钮文字同步变色
        /// </summary>
        public bool TextColorSyncFade = false;

        /// <summary>
        /// 按钮图标同步变色
        /// </summary>
        public bool IconColorSyncFade = false;

        /// <summary>
        /// 背景图标同步变色
        /// </summary>
        public bool BgColorSyncFade = false;

        public bool AutoStopPreview = true;
        public bool AnimatorsIsFold;
        public bool EventIsFold;
        public bool ToggleOriginalIsFold;

        public string ButtonActionTiming = "鼠标点击";

        private Tweener tweener_TextColor;

        protected override void Awake()
        {
            base.Awake();
            if (HudOption == null)
                HudOption = GetComponentInParent<Hud_Option>();
            if (RectTransform == null)
                RectTransform = GetComponent<RectTransform>();

            //---如果按钮文字模块上有Animator那么将当前按钮组件赋值给Animator的modButton模块以此防止按钮文字变色功能受到Animator实时同步颜色的阻碍
            if (ButtonTmpText != null)
            {
                Hud_Animator animator = ButtonTmpText.GetComponent<Hud_Animator>();
                if (animator != null)
                {
                    animator.mod_HudButton = this;
                }
            }
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (Application.isPlaying)
            {
                StartCoroutine(LongPress());
                btn_Reset();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (Application.isPlaying)
            {
                btn_Reset(true, true);
            }
        }

        private void Update()
        {

        }

        #region 获取动画器和动画节点

        /// <summary>
        /// 获取一个动画器
        /// </summary>
        /// <param tweenName="indicator">目标标识名称</param>
        /// <returns>返回一个匹配标识名称的HudAnimator动画器</returns>
        public Hud_Animator GetAnimator(string indicator)
        {
            Hud_Animator am = null;
            for (int i = 0; i < BtnAnimatorNodes.Count; i++)
            {
                if (BtnAnimatorNodes[i].Animator.GetIndicator() == indicator)
                {
                    am = BtnAnimatorNodes[i].Animator;
                }
            }
            if (am == null)
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("按钮控件通知", "未获取到标识名为 " + indicator + " 的子级动画器！ ", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("按钮控件通知", "已获取子级动画器 " + indicator, HudMsgState.通知);
            }
            return am;
        }

        /// <summary>
        /// 获取一个动画器
        /// </summary>
        /// <param tweenName="name">目标物体名称</param>
        /// <returns>返回一个匹配物体名称名称的HudAnimator动画器</returns>
        public Hud_Animator GetAnimator_WithObjectName(string name)
        {
            Hud_Animator am = null;
            for (int i = 0; i < BtnAnimatorNodes.Count; i++)
            {
                if (BtnAnimatorNodes[i].Animator.gameObject.name == name)
                {
                    am = BtnAnimatorNodes[i].Animator;
                }
            }
            if (am == null)
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("按钮控件通知", "未获取到名为 " + name + " 的子级动画器！ ", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("按钮控件通知", "已获取子级动画器 " + name, HudMsgState.通知);
            }
            return am;
        }

        /// <summary>
        /// 获取一个动画器
        /// </summary>
        /// <param tweenName="id">目标动画器的ID</param>
        /// <returns>返回一个匹配ID的HudAnimator动画器</returns>
        public Hud_Animator GetAnimator(int id)
        {
            Hud_Animator am = null;
            for (int i = 0; i < BtnAnimatorNodes.Count; i++)
            {
                if (BtnAnimatorNodes[i].Animator.GetID() == id)
                {
                    am = BtnAnimatorNodes[i].Animator;
                }
            }
            if (am == null)
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("按钮控件通知", "未获取到索引号为 " + id + " 的子级动画器！ ", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("按钮控件通知", "已获取索引号为 " + id + " 子级动画器！", HudMsgState.通知);
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
            Hud_Animator anim = GetAnimator(animator_indicator);
            TweenNode node = anim.TweenNode_GetByID(tween_id);

            if (anim == null)
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("按钮控件通知", "未获取到名为 " + animator_indicator + " 的子级动画器！ ", HudMsgState.错误);
            }
            else
            {
                if (node == null)
                {
                    if (DebugState)
                        util_Tools.Func_PrintInfo("按钮控件通知", "已获取子级动画器 " + animator_indicator, HudMsgState.通知);
                }
                else
                {
                    if (DebugState)
                        util_Tools.Func_PrintInfo("按钮控件通知", "已获取子级动画器 " + animator_indicator + "，但并未在其中找到索引号为 " + tween_id + " 的动画效果！", HudMsgState.警告);
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
            Hud_Animator anim = GetAnimator(animator_id);
            TweenNode node = anim.TweenNode_GetByID(tween_id);

            if (anim == null)
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("按钮控件通知", "未获取到ID为 " + animator_id + " 的子级动画器！ ", HudMsgState.错误);
            }
            else
            {
                if (node == null)
                {
                    if (DebugState)
                        util_Tools.Func_PrintInfo("按钮控件通知", "已获取ID为 " + animator_id + " 子级动画器", HudMsgState.通知);
                }
                else
                {
                    if (DebugState)
                        util_Tools.Func_PrintInfo("按钮控件通知", "已获取ID为 " + animator_id + " 子级动画器，但并未在其中找到ID号为 " + tween_id + " 的动画节点！", HudMsgState.警告);
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
            Hud_Animator anim = GetAnimator(animator_indicator);
            TweenNode node = anim.TweenNode_GetByIndicator(tween_indicator);

            if (anim == null)
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("按钮控件通知", "未获取到名为 " + animator_indicator + " 的子级动画器！ ", HudMsgState.错误);
            }
            else
            {
                if (node == null)
                {
                    if (DebugState)
                        util_Tools.Func_PrintInfo("按钮控件通知", "已获取子级动画器 " + animator_indicator, HudMsgState.通知);
                }
                else
                {
                    if (DebugState)
                        util_Tools.Func_PrintInfo("按钮控件通知", "已获取子级动画器 " + animator_indicator + "，但并未在其中找到名称为 " + tween_indicator + " 的动画效果！", HudMsgState.警告);
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
            for (int i = 0; i < BtnAnimatorNodes.Count; i++)
            {
                if (BtnAnimatorNodes[i].Animator.GetID() == ID)
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
            for (int i = 0; i < BtnAnimatorNodes.Count; i++)
            {
                if (BtnAnimatorNodes[i].Animator.GetIndicator() == Indicator)
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
            if (BtnAnimatorNodes == null || BtnAnimatorNodes.Count <= 0)
                return;

            for (int i = 0; i < BtnAnimatorNodes.Count; i++)
            {
                Hud_Animator animator = BtnAnimatorNodes[i].Animator;
                animator.Play(tim, BtnAnimatorNodes[i].DelayTime, Button_Animators_GlobalDuration * animator.Animator_GlobalDuration, true, null, null, 0.5f);
            }

            if (DebugState)
                util_Tools.Func_PrintInfo("按钮控件通知", "播放所有动画器动画！", HudMsgState.确认);
        }

        /// <summary>
        /// 播放按钮子级中的指定ID的动画
        /// </summary>
        /// <param tweenName="id">动画节点的ID</param>
        /// <param tweenName="tim">触发动画的时机</param>
        private void Animators_PlayAt(int id, string tim)
        {
            if (BtnAnimatorNodes == null || BtnAnimatorNodes.Count <= 0)
                return;

            if (!AnimatorIsExist(id))
                return;

            for (int i = 0; i < BtnAnimatorNodes.Count; i++)
            {
                if (BtnAnimatorNodes[i].Animator.GetID() != id)
                    continue;
                Hud_Animator anim = BtnAnimatorNodes[i].Animator;
                anim.Play(tim, BtnAnimatorNodes[i].DelayTime, Button_Animators_GlobalDuration * anim.Animator_GlobalDuration);
            }

            if (DebugState)
                util_Tools.Func_PrintInfo("按钮控件通知", "播放指定ID的动画器的动画！", HudMsgState.确认);
        }

        /// <summary>
        /// 倒退按钮子级中的所有动画
        /// </summary>
        private void Animators_Rewind()
        {
            for (int i = 0; i < BtnAnimatorNodes.Count; i++)
            {
                Hud_Animator anim = BtnAnimatorNodes[i].Animator;
                anim.RewindAllTweenNode();
            }
            if (DebugState)
                util_Tools.Func_PrintInfo("按钮控件通知", "复位按钮动画！", HudMsgState.确认);
        }
        #endregion       

        #region 按钮事件和委托

        /// <summary>
        /// 移除所有委托
        /// </summary>
        public void btn_ActionsClear()
        {
            act_on_DeSelect = null;
            act_on_Select = null;
            act_on_Clicked = null;
            act_on_Press = null;
            act_on_Enter = null;
            act_on_Exit = null;
            act_on_Release = null;
            act_on_LongPressed = null;
            act_on_LongPressPer = null;

            if (DebugState)
                util_Tools.Func_PrintInfo("按钮控件通知", "清空所有委托！", HudMsgState.设置);
        }

        /// <summary>
        /// 移除所有事件
        /// </summary>
        public void btn_EventsClear()
        {
            eve_on_Deselect.RemoveAllListeners();
            eve_on_Select.RemoveAllListeners();
            eve_on_Click.RemoveAllListeners();
            eve_on_Press.RemoveAllListeners();
            eve_on_Enter.RemoveAllListeners();
            eve_on_Exit.RemoveAllListeners();
            eve_on_Release.RemoveAllListeners();
            eve_on_Longpressed.RemoveAllListeners();
            eve_on_LongpressPer.RemoveAllListeners();

            if (DebugState)
                util_Tools.Func_PrintInfo("按钮控件通知", "清空所有事件！", HudMsgState.设置);
        }

        #endregion

        #region 按钮动作

        /// <summary>
        /// 按钮取消选中
        /// </summary>
        /// <param tweenName="eventData"></param>
        public override void OnDeselect(BaseEventData eventData)
        {
            if (!interactable)
                return;
            base.OnDeselect(eventData);
            Animators_Play("鼠标取消选中");
            if (ButtonActionTiming == "鼠标取消选中")
                btn_Option_Selected();
            HudButtonState = Hud_ButtonAction.取消选中;
            HudButtonState_Selector = Hud_ButtonAction.取消选中;
            HudButtonState_Clicked = Hud_ButtonAction.待命;

            if (act_on_DeSelect != null)
                act_on_DeSelect(this, HudButtonState);
            eve_on_Deselect.Invoke();
            if (DebugState)
                util_Tools.Func_PrintInfo("按钮控件通知", "取消选中！", HudMsgState.通知);
        }

        /// <summary>
        /// 按钮选中
        /// </summary>
        /// <param tweenName="eventData"></param>
        public override void OnSelect(BaseEventData eventData)
        {
            if (!interactable)
                return;
            base.OnSelect(eventData);
            Animators_Play("鼠标选中");
            if (ButtonActionTiming == "鼠标选中")
                btn_Option_Selected();
            HudButtonState = Hud_ButtonAction.选中;
            HudButtonState_Selector = Hud_ButtonAction.选中;

            if (act_on_Select != null)
                act_on_Select(this, HudButtonState);
            eve_on_Select.Invoke();
            if (DebugState)
                util_Tools.Func_PrintInfo("按钮控件通知", "选中！", HudMsgState.确认);
        }

        /// <summary>
        /// 按钮点击
        /// </summary>
        /// <param tweenName="eventData"></param>
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (!interactable)
                return;
            if (LongPressIsInvoke)
                return;
            if (LongPress_Percent >= ClickDelayTimeThreadhold)
                return;

            base.OnPointerClick(eventData);
            Animators_Play("鼠标点击");
            if (ButtonActionTiming == "鼠标点击")
                btn_Option_Selected();
            HudButtonState = Hud_ButtonAction.点击;
            HudButtonState_Clicked = Hud_ButtonAction.点击;

            if (act_on_Clicked != null)
                act_on_Clicked(this, HudButtonState);
            eve_on_Click.Invoke();
            if (DebugState)
                util_Tools.Func_PrintInfo("按钮控件通知", "点击！", HudMsgState.确认);
        }

        /// <summary>
        /// 按钮进入
        /// </summary>
        /// <param tweenName="eventData"></param>
        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (!interactable)
                return;
            base.OnPointerEnter(eventData);
            Animators_Play("鼠标进入");
            if (ButtonActionTiming == "鼠标进入")
                btn_Option_Selected();
            HudButtonState = Hud_ButtonAction.进入;
            HudButtonState_Holder = Hud_ButtonAction.进入;

            if (act_on_Enter != null)
                act_on_Enter(this, HudButtonState);
            eve_on_Enter.Invoke();
            if (DebugState)
                util_Tools.Func_PrintInfo("按钮控件通知", "鼠标进入！", HudMsgState.确认);
        }

        /// <summary>
        /// 按钮离开
        /// </summary>
        /// <param tweenName="eventData"></param>
        public override void OnPointerExit(PointerEventData eventData)
        {
            if (!interactable)
                return;
            base.OnPointerExit(eventData);
            Animators_Play("鼠标退出");
            if (ButtonActionTiming == "鼠标退出")
                btn_Option_Selected();
            HudButtonState = Hud_ButtonAction.离开;
            HudButtonState_Holder = Hud_ButtonAction.离开;

            if (act_on_Exit != null)
                act_on_Exit(this, HudButtonState);
            eve_on_Exit.Invoke();
            IsLongPress = false;
            if (DebugState)
                util_Tools.Func_PrintInfo("按钮控件通知", "鼠标退出！", HudMsgState.确认);
        }

        /// <summary>
        /// 按钮按下
        /// </summary>
        /// <param tweenName="eventData"></param>
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!interactable)
                return;
            base.OnPointerDown(eventData);
            Animators_Play("鼠标按下");
            if (ButtonActionTiming == "鼠标按下")
                btn_Option_Selected();
            HudButtonState = Hud_ButtonAction.按下;
            HudButtonState_Pressed = Hud_ButtonAction.按下;

            if (TextColorSyncFade)
            {
                if (ButtonText != null)
                {
                    if (tweener_TextColor != null && tweener_TextColor.active)
                    {
                        if (tweener_TextColor.IsPlaying())
                        {
                            tweener_TextColor.Kill();
                        }
                    }
                    tweener_TextColor = DOTween.To(() => ButtonText.TextStyleInfo.FontColor, x => ButtonText.TextStyleInfo.FontColor = x, colors.pressedColor, colors.fadeDuration).SetAutoKill(true);
                }
                if (ButtonTmpText != null)
                {
                    if (tweener_TextColor != null && tweener_TextColor.active)
                    {
                        if (tweener_TextColor.IsPlaying())
                        {
                            tweener_TextColor.Kill();
                        }
                    }
                    tweener_TextColor = DOTween.To(() => ButtonTmpText.TextStyleInfo.tmp_color, x => ButtonTmpText.TextStyleInfo.tmp_color = x, colors.pressedColor, colors.fadeDuration).SetAutoKill(true);
                }
            }
            if (IconColorSyncFade)
            {
                if (IconImage != null)
                    IconImage.DOColor(colors.pressedColor, colors.fadeDuration);
            }
            if (BgColorSyncFade)
            {
                if (BgImage != null)
                    BgImage.DOColor(colors.pressedColor, colors.fadeDuration);
            }

            if (act_on_Press != null)
                act_on_Press(this, HudButtonState);
            eve_on_Press.Invoke();
            IsLongPress = true;
            if (DebugState)
                util_Tools.Func_PrintInfo("按钮控件通知", "按住！", HudMsgState.确认);
        }

        /// <summary>
        /// 按钮抬起
        /// </summary>
        /// <param tweenName="eventData"></param>
        public override void OnPointerUp(PointerEventData eventData)
        {
            if (!interactable)
                return;
            base.OnPointerUp(eventData);
            Animators_Play("鼠标松开");
            if (ButtonActionTiming == "鼠标松开")
                btn_Option_Selected();
            HudButtonState = Hud_ButtonAction.抬起;
            HudButtonState_Pressed = Hud_ButtonAction.抬起;
            HudButtonState_Clicked = Hud_ButtonAction.待命;
            HudButtonState_LongPressed = Hud_ButtonAction.待命;

            if (TextColorSyncFade)
            {
                if (ButtonText != null)
                {
                    if (tweener_TextColor != null && tweener_TextColor.active)
                    {
                        if (tweener_TextColor.IsPlaying())
                        {
                            tweener_TextColor.Kill();
                        }
                    }
                    tweener_TextColor = DOTween.To(() => ButtonText.TextStyleInfo.FontColor, x => ButtonText.TextStyleInfo.FontColor = x, colors.normalColor, colors.fadeDuration).SetAutoKill(true);
                }
                if (ButtonTmpText != null)
                {
                    if (tweener_TextColor != null && tweener_TextColor.active)
                    {
                        if (tweener_TextColor.IsPlaying())
                        {
                            tweener_TextColor.Kill();
                        }
                    }
                    tweener_TextColor = DOTween.To(() => ButtonTmpText.TextStyleInfo.tmp_color, x => ButtonTmpText.TextStyleInfo.tmp_color = x, colors.normalColor, colors.fadeDuration).SetAutoKill(true);
                }
            }
            if (IconColorSyncFade)
            {
                if (IconImage != null)
                    IconImage.DOColor(colors.normalColor, colors.fadeDuration);
            }
            if (BgColorSyncFade)
            {
                if (BgImage != null)
                    BgImage.DOColor(colors.normalColor, colors.fadeDuration);
            }
            if (act_on_Release != null)
                act_on_Release(this, HudButtonState);
            eve_on_Release.Invoke();
            IsLongPress = false;
            if (DebugState)
                util_Tools.Func_PrintInfo("按钮控件通知", "松开！", HudMsgState.确认);
        }

        #endregion

        #region 按钮辅助
        /// <summary>
        /// 设置按钮背景组件的图片
        /// </summary>
        /// <param tweenName="tex"></param>
        public void btn_SetBg(Texture2D tex)
        {
            if (tex != null)
                BgImage.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);
        }

        /// <summary>
        /// 设置按钮背景组件的图片
        /// </summary>
        /// <param tweenName="spr"></param>
        public void btn_SetBg(Sprite spr)
        {
            if (spr != null)
                BgImage.sprite = spr;
        }

        /// <summary>
        /// 设置按钮图标组件的图片
        /// </summary>
        /// <param tweenName="tex"></param>
        public void btn_SetIcon(Texture2D tex)
        {
            if (tex != null)
                IconImage.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);
        }

        /// <summary>
        /// 设置按钮图标组件的图片
        /// </summary>
        /// <param tweenName="spr"></param>
        public void btn_SetIcon(Sprite spr)
        {
            if (spr != null)
                IconImage.sprite = spr;
        }

        /// <summary>
        /// 设置按钮的启用或禁用
        /// </summary>
        /// <param tweenName="treeState"></param>
        public void btn_SetInteractable(bool state)
        {
            interactable = state;

            if (state)
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("按钮控件通知", "启用按钮交互", HudMsgState.警告);
            }
            else
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("按钮控件通知", "禁用按钮交互", HudMsgState.警告);
            }
        }

        /// <summary>
        /// 设置当按钮作为选项作用时的交互方式
        /// </summary>
        /// <param tweenName="tim"></param>
        public void btn_SetOptionTiming(string tim)
        {
            ButtonActionTiming = tim;
        }

        /// <summary>
        /// 获取长按进度百分比
        /// </summary>
        /// <returns></returns>
        public float btn_GetLongPressPercent()
        {
            if (DebugState)
                util_Tools.Func_PrintInfo("按钮控件通知", "获取到长按百分比：" + LongPress_Percent, HudMsgState.警告);
            return LongPress_Percent;
        }

        /// <summary>
        /// 设置长按时松开后进度倒退平滑
        /// </summary>
        /// <param tweenName="treeState"></param>
        public void btn_SetLongpressSmoothRewind(bool state)
        {
            LongPress_SmoothRewind = state;
            if (state)
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("按钮控件通知", "长按松开平滑回退启用！", HudMsgState.警告);
            }
            else
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("按钮控件通知", "长按松开平滑回退禁用！", HudMsgState.通知);
            }
        }

        /// <summary>
        /// 设置长按的阈值
        /// </summary>
        /// <param tweenName="value"></param>
        public void btn_LongpressThreshold(float value)
        {
            LongPress_Threshold = value;

            if (DebugState)
                util_Tools.Func_PrintInfo("按钮控件通知", "设置长按阈值为：" + value, HudMsgState.警告);
        }

        /// <summary>
        /// 设置按钮文字名称
        /// </summary>
        /// <param tweenName="val"></param>
        public string btn_SetText(string val = null)
        {
            string content;

            if (string.IsNullOrEmpty(val))
                content = ButtonName;
            else
                content = ButtonName = val;

            if (ButtonTmpText != null)
                ButtonTmpText.text = content;

            if (ButtonText != null)
                ButtonText.text = content;

            if (DebugState)
                util_Tools.Func_PrintInfo("按钮控件通知", "设置按钮显示文字为：" + content, HudMsgState.设置);
            return content;
        }

        /// <summary>
        /// 按钮重置
        /// </summary>
        public void btn_Reset(bool ClearEvents = false, bool ClearAction = false)
        {
            Animators_Rewind();

            StopCoroutine(LongPress());

            if (ClearEvents)
                btn_EventsClear();
            if (ClearAction)
                btn_ActionsClear();

            //变色动画器杀死
            if (tweener_TextColor != null && tweener_TextColor.active)
            {
                if (tweener_TextColor.IsPlaying())
                {
                    tweener_TextColor.Kill();
                    tweener_TextColor = null;
                }
            }

            LongPress_Tick = 0;
            LongPress_Percent = 0;

            if (DebugState)
                util_Tools.Func_PrintInfo("按钮控件通知", "已停止长按逻辑检测协程！", HudMsgState.设置);
        }

        public float ClickDelayTime;
        [Range(0, 1)]
        public float ClickDelayTimeThreadhold = 1;

        /// <summary>
        /// 按钮长按逻辑
        /// </summary>
        /// <returns></returns>
        IEnumerator LongPress()
        {
            if (DebugState)
                util_Tools.Func_PrintInfo("按钮控件通知", "已启动长按逻辑检测协程！", HudMsgState.设置);
            while (true)
            {
                if (IsLongPress)
                {
                    if (LongPress_Tick >= LongPress_Threshold)
                    {
                        if (!LongPressIsInvoke)
                        {
                            LongPressIsInvoke = true;

                            Animators_Play("鼠标长按");
                            if (ButtonActionTiming == "鼠标长按")
                                btn_Option_Selected();
                            HudButtonState = Hud_ButtonAction.长按;
                            HudButtonState_LongPressed = Hud_ButtonAction.长按;

                            if (act_on_LongPressed != null)
                                act_on_LongPressed(this, HudButtonState);
                            eve_on_Longpressed.Invoke();

                            if (DebugState)
                                util_Tools.Func_PrintInfo("按钮控件通知", "长按动作完成！", HudMsgState.设置);
                        }
                        if (LongPress_UseRewind)
                            IsLongPress = false;
                    }
                    else
                    {
                        LongPress_Tick += LongPress_Step;
                    }
                    ClickDelayTime = LongPress_Percent;
                    yield return new WaitForEndOfFrame();
                }
                else
                {
                    LongPressIsInvoke = false;
                    if (LongPress_SmoothRewind)
                        LongPress_Tick = Mathf.Lerp(LongPress_Tick, 0, Time.deltaTime * 4);
                    else
                        LongPress_Tick = 0;
                    yield return new WaitForEndOfFrame();
                }

                LongPress_Percent = Mathf.Clamp01(LongPress_Tick / LongPress_Threshold);

                if (LongPress_Percent < 0.005f)
                    LongPress_Percent = 0;

                if (act_on_LongPressPer != null)
                    act_on_LongPressPer(this, LongPress_Percent);
                eve_on_LongpressPer.Invoke();

                yield return null;
            }
        }

        #endregion

        #region 选项按钮动作

        /// <summary>
        /// 选择选项
        /// </summary>
        private void btn_Option_Selected()
        {
            if (IsOptionButton)
            {
                if (HudOption == null)
                {
                    if (DebugState)
                        util_Tools.Func_PrintInfo("按钮控件通知", "未发现此按钮处于选项层级中！", HudMsgState.错误);
                    return;
                }
                for (int i = 0; i < HudOption.OptionButtonNodes.Count; i++)
                {
                    if (HudOption.OptionButtonNodes[i].Button.IsOptionButton &&
                        HudOption.OptionButtonNodes[i].Indicator == Indicator)
                    {
                        //HudOption.opt_Clicked(i);
                        HudOption.opt_Select(Indicator);
                        if (DebugState)
                            util_Tools.Func_PrintInfo("按钮控件通知", "选项点击！", HudMsgState.确认);
                    }
                }
            }
        }

        /// <summary>
        /// 清空选项名称
        /// </summary>
        public void btn_Option_NameClear()
        {
            if (ButtonTmpText != null)
                ButtonTmpText.text = null;

            if (ButtonText != null)
                ButtonText.text = null;

            if (DebugState)
                util_Tools.Func_PrintInfo("按钮控件通知", "清空显示文字！", HudMsgState.通知);
        }

        #endregion
    }
}