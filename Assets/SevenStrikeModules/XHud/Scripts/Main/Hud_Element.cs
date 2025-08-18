namespace SevenStrikeModules.XHud.Hud
{
    using DG.Tweening;
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.Utilitys;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using Random = UnityEngine.Random;

    #region CustomClass

    /// <summary>
    /// 元素音效器的结构类
    /// </summary>
    [System.Serializable]
    public class SounderNode
    {
        /// <summary>
        /// 音效器
        /// </summary>
        public Hud_Sounder Sounder;
    }

    /// <summary>
    /// 动画器的结构类
    /// </summary>
    [System.Serializable]
    public class AnimatorNode
    {
        [SerializeField]
        /// <summary>
        /// 动画器
        /// </summary>
        public Hud_Animator Animator;
        [SerializeField]
        /// <summary>
        /// 动画器的动画总计时间
        /// </summary>
        public float TotalTime;
        [SerializeField]
        /// <summary>
        /// 动画器的动画延迟时间
        /// </summary>
        public float DelayTime;
    }

    /// <summary>
    /// 数据源的结构类
    /// </summary>
    [System.Serializable]
    public class ContainerNode
    {
        /// <summary>
        /// Hud容器
        /// </summary>
        public Hud_Container Container;
    }

    /// <summary>
    /// 按钮的结构类
    /// </summary>
    [System.Serializable]
    public class ButtonNode
    {
        /// <summary>
        /// 按钮组件
        /// </summary>
        public Hud_Button Button;
        /// <summary>
        /// 用于选项按钮
        /// </summary>
        public bool IsOptional;
    }

    /// <summary>
    /// 选项按钮的结构类
    /// </summary>
    [System.Serializable]
    public class OptionButtonNode
    {
        /// <summary>
        /// 按钮组件
        /// </summary>
        public Hud_Button Button;
        /// <summary>
        /// 选项按钮标识
        /// </summary>
        public string Indicator;
        /// <summary>
        /// 用于选项按钮
        /// </summary>
        public bool IsOptional;
    }

    /// <summary>
    /// 选项的结构类
    /// </summary>
    [System.Serializable]
    public class OptionNode
    {
        /// <summary>
        /// 按钮组件
        /// </summary>
        public Hud_Option Option;
    }

    /// <summary>
    /// 文字的结构类
    /// </summary>
    [System.Serializable]
    public class TextNode
    {
        /// <summary>
        /// 文字组件
        /// </summary>
        public Hud_Text Text;
    }

    /// <summary>
    /// Tmp文字的结构类
    /// </summary>
    [System.Serializable]
    public class TmpTextNode
    {
        /// <summary>
        /// 文字组件
        /// </summary>
        public Hud_TmpText TmpText;
    }

    /// <summary>
    /// 滑动条的结构类
    /// </summary>
    [System.Serializable]
    public class SliderNode
    {
        /// <summary>
        /// 滑动条组件
        /// </summary>
        public Hud_Slider Slider;
    }

    /// <summary>
    /// 进度条的结构类
    /// </summary>
    [System.Serializable]
    public class ProgressNode
    {
        /// <summary>
        /// 进度条组件
        /// </summary>
        public Hud_Progress Progress;
    }

    /// <summary>
    /// 开关的结构类
    /// </summary>
    [System.Serializable]
    public class ToggleNode
    {
        /// <summary>
        /// 开关组件
        /// </summary>
        public Hud_Toggle Toggle;
    }

    [System.Serializable]
    public class OriginalLayoutInfo
    {
        public string LayoutName;
        public HudAnchor Anchor;
        public Vector3 Position;
        public Vector3 Euler;
        public Vector3 Scale;
        public Vector2 AnchorMin;
        public Vector2 AnchorMax;
        public Vector2 Pivot;
    }

    #endregion

    [System.Serializable]
    /// <summary>    
    /// Hud元素
    /// </summary>    
    [RequireComponent(typeof(CanvasGroup))]
    public class Hud_Element : MonoBehaviour
    {
        [SerializeField]
        /// <summary>
        /// 元素昵称
        /// </summary>
        public string Indicator;
        [SerializeField]
        /// <summary>
        /// 元素原始名称
        /// </summary>
        public string OriginalName;
        [SerializeField]
        public bool DebugState;
        /// <summary>
        /// CanvasGroup组件
        /// </summary>
        public CanvasGroup CanvasGroup;

        private int id;
        [SerializeField]
        /// <summary>
        /// ID编号
        /// </summary>
        public int ID
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }

        /// <summary>
        /// 元素的生成状态
        /// </summary>
        public HudElementCreateState CreateState = HudElementCreateState.Recycled;

        /// <summary>
        /// 元素的动画状态
        /// </summary>
        public HudElementAnimateState AnimateState = HudElementAnimateState.Static;

        /// <summary>
        /// 判定元素是屏幕类型还是世界空间类型
        /// </summary>
        public ElementSpaceType ElementSpaceType = ElementSpaceType.None;

        /// <summary>
        /// 变换组件
        /// </summary>
        public RectTransform RectTransform;

        [Range(0f, 1f)]
        /// <summary>
        /// 透明度_Alpha
        /// </summary>
        public float Alpha = 1;

        /// <summary>
        /// 所有动画器中最长的耗时
        /// </summary>
        public float AnimatorsMaxDuration;

        /// <summary>
        /// 元素下所有动画器的动画速度倍乘系数
        /// </summary>
        public float Element_Animators_GlobalDuration = 1;

        /// <summary>
        /// 锚点坐标
        /// </summary>
        public Vector2 CurrentPivot;

        /// <summary>
        /// 动作 - 动画播放 - 入场 - 开始
        /// </summary>
        public UnityAction<Hud_Element> act_on_element_in_start;
        /// <summary>
        /// 动作 - 动画播放 - 入场 - 进度中
        /// </summary>
        public UnityAction<float> act_on_element_in_progress;
        /// <summary>
        /// 动作 - 动画播放 - 入场 - 结束
        /// </summary>
        public UnityAction<Hud_Element> act_on_element_in_end;
        /// <summary>
        /// 动作 - 动画播放 - 出场 - 开始
        /// </summary>
        public UnityAction<Hud_Element> act_on_element_out_start;
        /// <summary>
        /// 动作 - 动画播放 - 出场 - 进度中
        /// </summary>
        public UnityAction<float> act_on_element_out_progress;
        /// <summary>
        /// 动作 - 动画播放 - 出场 - 结束
        /// </summary>
        public UnityAction<Hud_Element> act_on_element_out_end;

        /// <summary>
        /// 动作 - 动画播放 - 入场 - 开始
        /// </summary>
        public UnityEvent eve_on_element_in_start;
        /// <summary>
        /// 动作 - 动画播放 - 入场 - 结束
        /// </summary>
        public UnityEvent eve_on_element_in_end;
        /// <summary>
        /// 动作 - 动画播放 - 出场 - 开始
        /// </summary>
        public UnityEvent eve_on_element_out_start;
        /// <summary>
        /// 动作 - 动画播放 - 出场 - 结束
        /// </summary>
        public UnityEvent eve_on_element_out_end;

        /// <summary>
        /// 动作器
        /// </summary>
        public Hud_ElementTriggerAction TriggerAction;

        /// <summary>
        /// 物体跟踪器
        /// </summary>
        public Hud_ObjectTracker ObjectTracker;

        /// <summary>
        /// 元素音效容器
        /// </summary>
        public List<SounderNode> SounderNodes;

        /// <summary>
        /// 动画状态
        /// </summary>
        public bool Animating;

        /// <summary>
        /// 数据源容器
        /// </summary>
        public List<ContainerNode> ContainerNodes;

        /// <summary>
        /// 动画器容器
        /// </summary>
        public List<AnimatorNode> AnimatorNodes;

        /// <summary>
        /// 按钮容器
        /// </summary>
        public List<ButtonNode> ButtonNodes;

        /// <summary>
        /// 选项容器
        /// </summary>
        public List<OptionNode> OptionNodes;

        /// <summary>
        /// 文字容器
        /// </summary>
        public List<TextNode> TextNodes;

        /// <summary>
        /// Tmp文字容器
        /// </summary>
        public List<TmpTextNode> TmpTextNodes;

        /// <summary>
        /// 滑动条容器
        /// </summary>
        public List<SliderNode> SliderNodes;

        /// <summary>
        /// 进度条容器
        /// </summary>
        public List<ProgressNode> ProgressNodes;

        /// <summary>
        /// 开关容器
        /// </summary>
        public List<ToggleNode> ToggleNodes;

        /// <summary>
        /// 源自元素池的名称
        /// </summary>
        public string OriginPoolName;

        /// <summary>
        /// 动画模组 - 透明度_Alpha
        /// </summary>
        private Tweener Tween_Alpha;

        /// <summary>
        /// 动画模组 - 位移
        /// </summary>
        private Tweener Tween_Move;

        /// <summary>
        /// 动画模组 - 旋转_Rotation
        /// </summary>
        private Tweener Tween_Rotation;

        public bool AnimatorsIsFold;
        public bool ButtonIsFold;
        public bool OptionIsFold;
        public bool SliderIsFold;
        public bool ProgressIsFold;
        public bool ToggleIsFold;
        public bool EventIsFold;
        public bool SounderIsFold;
        public bool ContainerIsFold;
        public bool TextIsFold;
        public bool TmpTextIsFold;
        public bool AutoPlayAnimators = true;
        public bool AutoPlayContainersAnimators = true;
        public bool AutoStopPreview = true;
        public bool TweenIsPreviewing = false;

        ///---RMS = Resolution Matching Solution

        [SerializeField]
        /// <summary>
        /// 布局匹配方案是否开启
        /// </summary>
        public bool RMS_Enabled;

        [SerializeField]
        /// <summary>
        ///  布局匹配方案列表
        /// </summary>
        public List<OriginalLayoutInfo> RMS_InfoList;

        [SerializeField]
        /// <summary>
        ///  布局匹配方案当前选择的标识名称
        /// </summary>
        public string RMS_Name;

        private void Awake()
        {
            if (RectTransform == null)
                RectTransform = GetComponent<RectTransform>();
            if (CanvasGroup == null)
                CanvasGroup = GetComponent<CanvasGroup>();
            if (TriggerAction == null)
                TriggerAction = GetComponent<Hud_ElementTriggerAction>();
        }

        public virtual void OnEnable()
        {
            element_SetInteractable(true);
        }

        public virtual void OnDisable()
        {
        }

        public virtual void Start()
        {
            Hud_Manager.Instance.Act_ScreenResolution_Changed += ScreenResolution_Changed;
        }

        private void ScreenResolution_Changed(string Indicator, Vector2 Res)
        {
            Hud_Manager.Instance.hm_HudElement_Initialize_ByDesignLayout_For_Screen(this, Alpha, Vector3.zero, RMS_Name, true);
        }

        public virtual void Update()
        {
            element_AlphaSyncUpdate();
        }

        #region 获取动画器和动画节点
        /// <summary>
        /// 获取一个动画器
        /// </summary>
        /// <param tweenName="indicator">目标名称</param>
        /// <returns>返回一个匹配名称的HudAnimator动画器</returns>
        public Hud_Animator GetAnimator(string indicator)
        {
            Hud_Animator am = null;
            for (int i = 0; i < AnimatorNodes.Count; i++)
            {
                if (AnimatorNodes[i].Animator.GetIndicator() == indicator)
                {
                    am = AnimatorNodes[i].Animator;
                }
            }
            if (am == null)
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "未获取到名为 " + indicator + " 的子级动画器！ ", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "已获取子级动画器 " + indicator, HudMsgState.通知);
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
            for (int i = 0; i < AnimatorNodes.Count; i++)
            {
                if (AnimatorNodes[i].Animator.gameObject.name == name)
                {
                    am = AnimatorNodes[i].Animator;
                }
            }
            if (am == null)
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "未获取到名为 " + name + " 的子级动画器！ ", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "已获取子级动画器 " + name, HudMsgState.通知);
            }
            return am;
        }

        /// <summary>
        /// Hud元素 - 获取一个动画器
        /// </summary>
        /// <param tweenName="id">目标ID</param>
        /// <returns>返回一个匹配ID的HudAnimator动画器</returns>
        public Hud_Animator GetAnimator(int id)
        {
            Hud_Animator am = null;
            for (int i = 0; i < AnimatorNodes.Count; i++)
            {
                if (AnimatorNodes[i].Animator.GetID() == id)
                {
                    am = AnimatorNodes[i].Animator;
                }
            }
            if (am == null)
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "未获取到索引号为 " + id + " 的子级动画器！ ", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "已获取索引号为 " + id + " 子级动画器！", HudMsgState.通知);
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
                    util_Tools.Func_PrintInfo("元素控件通知", "未获取到名为 " + animator_indicator + " 的子级动画器！ ", HudMsgState.错误);
            }
            else
            {
                if (node == null)
                {
                    if (DebugState)
                        util_Tools.Func_PrintInfo("元素控件通知", "已获取子级动画器 " + animator_indicator, HudMsgState.通知);
                }
                else
                {
                    if (DebugState)
                        util_Tools.Func_PrintInfo("元素控件通知", "已获取子级动画器 " + animator_indicator + "，但并未在其中找到索引号为 " + tween_id + " 的动画效果！", HudMsgState.警告);
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
                    util_Tools.Func_PrintInfo("元素控件通知", "未获取到ID为 " + animator_id + " 的子级动画器！ ", HudMsgState.错误);
            }
            else
            {
                if (node == null)
                {
                    if (DebugState)
                        util_Tools.Func_PrintInfo("元素控件通知", "已获取ID为 " + animator_id + " 子级动画器", HudMsgState.通知);
                }
                else
                {
                    if (DebugState)
                        util_Tools.Func_PrintInfo("元素控件通知", "已获取ID为 " + animator_id + " 子级动画器，但并未在其中找到ID号为 " + tween_id + " 的动画节点！", HudMsgState.警告);
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
                    util_Tools.Func_PrintInfo("元素控件通知", "未获取到名为 " + animator_indicator + " 的子级动画器！ ", HudMsgState.错误);
            }
            else
            {
                if (node == null)
                {
                    if (DebugState)
                        util_Tools.Func_PrintInfo("元素控件通知", "已获取子级动画器 " + animator_indicator, HudMsgState.通知);
                }
                else
                {
                    if (DebugState)
                        util_Tools.Func_PrintInfo("元素控件通知", "已获取子级动画器 " + animator_indicator + "，但并未在其中找到名称为 " + tween_indicator + " 的动画效果！", HudMsgState.警告);
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
            for (int i = 0; i < AnimatorNodes.Count; i++)
            {
                if (AnimatorNodes[i].Animator.GetID() == ID)
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
            for (int i = 0; i < AnimatorNodes.Count; i++)
            {
                if (AnimatorNodes[i].Animator.GetIndicator() == Indicator)
                {
                    isExist = true;
                }
            }
            return isExist;
        }

        /// <summary>
        /// Hud元素 - 播放所有子元素动画器的动画
        /// </summary>
        /// <returns></returns>
        public void Animators_Play(string tim)
        {
            if (AnimatorNodes == null || AnimatorNodes.Count <= 0)
                return;
            for (int i = 0; i < AnimatorNodes.Count; i++)
            {
                Hud_Animator anim = AnimatorNodes[i].Animator;
                anim.Play(tim, AnimatorNodes[i].DelayTime, Element_Animators_GlobalDuration * anim.Animator_GlobalDuration);
            }
            if (DebugState)
                util_Tools.Func_PrintInfo("元素控件通知", "播放动画器列表中的所有动画！播放时机为：" + tim.ToString(), HudMsgState.通知);
        }

        /// <summary>
        /// 播放按钮子级中的指定ID的动画
        /// </summary>
        /// <param tweenName="id">动画节点的ID</param>
        /// <param tweenName="tim">触发动画的时机</param>
        private void Animators_PlayAt(int id, string tim)
        {
            if (AnimatorNodes == null || AnimatorNodes.Count <= 0)
                return;

            if (!AnimatorIsExist(id))
                return;

            for (int i = 0; i < AnimatorNodes.Count; i++)
            {
                if (AnimatorNodes[i].Animator.GetID() != id)
                    continue;
                Hud_Animator anim = AnimatorNodes[i].Animator;
                anim.Play(tim, AnimatorNodes[i].DelayTime, Element_Animators_GlobalDuration * anim.Animator_GlobalDuration);
            }

            if (DebugState)
                util_Tools.Func_PrintInfo("元素控件通知", "播放指定ID的动画器的动画！", HudMsgState.确认);
        }

        /// <summary>
        /// Hud元素 - 倒退所有子元素动画器的动画
        /// </summary>
        /// <returns></returns>
        public void Animators_Rewind()
        {
            for (int i = 0; i < AnimatorNodes.Count; i++)
            {
                Hud_Animator anim = AnimatorNodes[i].Animator;
                anim.RewindAllTweenNode();
            }

            if (DebugState)
                util_Tools.Func_PrintInfo("元素控件通知", "倒退复位动画器列表中的所有动画！", HudMsgState.通知);
        }
        #endregion

        #region 设置

        /// <summary>
        /// Hud元素 - 交互总开关
        /// </summary>
        /// <param tweenName="treeState">交互开关状态</param>
        public virtual void element_SetInteractable(bool state)
        {
            CanvasGroup.interactable = state;
            element_Button_InteractableSetAll(state);
            element_Option_InteractableSetAll(state);
            element_Slider_InteractableSetAll(state);
            element_Progress_EnableSetAll(state);
            element_Toggle_InteractableSetAll(state);
            if (state)
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "将所有下级控件的交互开启！", HudMsgState.通知);
            }
            else
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "将所有下级控件的交互禁用！", HudMsgState.通知);
            }
        }

        /// <summary>
        /// Hud元素 - 生成后自动播放动画的开关
        /// </summary>
        /// <param tweenName="treeState">开关状态</param>
        public virtual void element_SetAutoAnimator(bool state)
        {
            AutoPlayAnimators = state;
        }

        /// <summary>
        /// Hud元素 - 重置状态
        /// </summary>
        /// <param tweenName="Hidden">重置后是否隐藏物体</param>
        public virtual void element_Reset(bool Hidden = true, bool ClearEvents = true, bool ClearActions = true)
        {
            ID = 0;
            element_AlphaSet(0);
            element_CanvasAlphaSet(0);

            CurrentPivot = Vector2.one * 0.5f;

            if (CanvasGroup != null)
                CanvasGroup.interactable = true;

            if (ClearEvents)
            {
                act_on_element_in_start = null;
                act_on_element_in_progress = null;
                act_on_element_in_end = null;
                act_on_element_out_start = null;
                act_on_element_out_progress = null;
                act_on_element_out_end = null;
            }
            if (ClearActions)
            {
                eve_on_element_in_start.RemoveAllListeners();
                eve_on_element_in_end.RemoveAllListeners();
                eve_on_element_out_start.RemoveAllListeners();
                eve_on_element_out_end.RemoveAllListeners();
            }

            if (Tween_Alpha != null)
            {
                Tween_Alpha.Kill();
                Tween_Alpha = null;
            }

            if (Tween_Move != null)
            {
                Tween_Move.Kill();
                Tween_Move = null;
            }

            if (Tween_Rotation != null)
            {
                Tween_Rotation.Kill();
                Tween_Rotation = null;
            }

            Animators_Rewind();
            CreateState = HudElementCreateState.Recycled;
            gameObject.SetActive(!Hidden);

            if (DebugState)
                util_Tools.Func_PrintInfo("元素控件通知", "已重置！", HudMsgState.警告);
        }

        /// <summary>
        /// Hud元素 - 同步透明度
        /// </summary>
        public virtual void element_AlphaSyncUpdate()
        {
            if (CanvasGroup != null)
                CanvasGroup.alpha = Alpha;
        }

        /// <summary>
        /// Hud元素 - 设置透明度
        /// </summary>
        /// <param tweenName="alpha"></param>
        public virtual void element_AlphaSet(float alpha)
        {
            Alpha = alpha;

            if (DebugState)
                util_Tools.Func_PrintInfo("元素控件通知", "透明度设置为：" + alpha, HudMsgState.通知);
        }

        /// <summary>
        /// Hud元素 - 设置CanvasGroup透明度
        /// </summary>
        /// <param tweenName="alpha"></param>
        public virtual void element_CanvasAlphaSet(float alpha)
        {
            if (CanvasGroup != null)
                CanvasGroup.alpha = alpha;

            if (DebugState)
                util_Tools.Func_PrintInfo("元素控件通知", "透明度设置为：" + alpha, HudMsgState.通知);
        }

        /// <summary>
        /// Hud元素 - 设置锚点
        /// </summary>
        /// <param tweenName="pivot"></param>
        public virtual void element_PivotSet(Vector2 pivot)
        {
            CurrentPivot = pivot;
            RectTransform.pivot = pivot;

            if (DebugState)
                util_Tools.Func_PrintInfo("元素控件通知", "元素的锚点设置为：" + pivot, HudMsgState.通知);
        }

        /// <summary>
        /// Hud元素 - 设置锚点
        /// </summary>
        /// <param tweenName="type"></param>
        public virtual void element_PivotSet(HudAnchor type)
        {
            switch (type)
            {
                case HudAnchor.上:
                    CurrentPivot = new Vector2(0.5f, 1f);
                    break;
                case HudAnchor.下:
                    CurrentPivot = new Vector2(0.5f, 0f);
                    break;
                case HudAnchor.左:
                    CurrentPivot = new Vector2(0f, 0.5f);
                    break;
                case HudAnchor.右:
                    CurrentPivot = new Vector2(1f, 0.5f);
                    break;
                case HudAnchor.中心:
                    CurrentPivot = new Vector2(0.5f, 0.5f);
                    break;
                case HudAnchor.左上:
                    CurrentPivot = new Vector2(0f, 1f);
                    break;
                case HudAnchor.左下:
                    CurrentPivot = new Vector2(0f, 0f);
                    break;
                case HudAnchor.右上:
                    CurrentPivot = new Vector2(1f, 1f);
                    break;
                case HudAnchor.右下:
                    CurrentPivot = new Vector2(1f, 0f);
                    break;
                case HudAnchor.底层:
                    CurrentPivot = new Vector2(0.5f, 0.5f);
                    break;
                case HudAnchor.顶层:
                    CurrentPivot = new Vector2(0.5f, 0.5f);
                    break;
            }
            RectTransform.pivot = CurrentPivot;

            if (DebugState)
                util_Tools.Func_PrintInfo("元素控件通知", "元素的锚点设置为：" + type.ToString(), HudMsgState.通知);
        }

        /// <summary>
        /// Hud元素 - 设置锚点区域范围
        /// </summary>
        /// <param tweenName="AnchorMin"></param>
        /// <param tweenName="AnchorMax"></param>
        public virtual void element_AnchorRangeSet(Vector2 AnchorMin, Vector2 AnchorMax)
        {
            RectTransform.anchorMin = AnchorMin;
            RectTransform.anchorMax = AnchorMax;
        }

        /// <summary>
        /// Hud元素 - 位置归零
        /// </summary>
        public virtual void element_PositionResetZero()
        {
            RectTransform.anchoredPosition3D = Vector3.zero;

            if (DebugState)
                util_Tools.Func_PrintInfo("元素控件通知", "元素的3D锚点位置归零！", HudMsgState.通知);
        }

        /// <summary>
        /// Hud元素 - 旋转归零
        /// </summary>
        public virtual void element_RotationResetZero()
        {
            RectTransform.localEulerAngles = Vector3.zero;

            if (DebugState)
                util_Tools.Func_PrintInfo("元素控件通知", "元素的旋转归零！", HudMsgState.通知);
        }

        /// <summary>
        /// Hud元素 - 位置设置
        /// </summary>
        /// <param tweenName="pos">锚点位置_AnchoredPosition</param>
        public virtual void element_PositionSet(Vector3 pos)
        {
            RectTransform.anchoredPosition3D = pos;

            if (DebugState)
                util_Tools.Func_PrintInfo("元素控件通知", "元素的锚点位置设置为：" + pos, HudMsgState.通知);
        }

        /// <summary>
        /// Hud元素 - 尺寸设置
        /// </summary>
        /// <param tweenName="size">尺寸_Size</param>
        public virtual void element_SizeSet(Vector2 size)
        {
            RectTransform.sizeDelta = size;

            if (DebugState)
                util_Tools.Func_PrintInfo("元素控件通知", "元素的尺寸设置为：" + size, HudMsgState.通知);
        }

        /// <summary>
        /// Hud元素 - 位置设置 - 世界
        /// </summary>
        /// <param tweenName="pos">世界位置</param>
        public virtual void element_WorldPositionSet(Vector3 pos)
        {
            RectTransform.position = pos;

            if (DebugState)
                util_Tools.Func_PrintInfo("元素控件通知", "元素的世界位置设置为：" + pos, HudMsgState.通知);
        }

        /// <summary>
        /// Hud元素 - 旋转设置
        /// </summary>
        /// <param tweenName="rot">本地旋转角度</param>
        public virtual void element_RotationSet(Vector3 rot)
        {
            RectTransform.localEulerAngles = rot;
        }

        /// <summary>
        /// Hud元素 - 旋转设置 - 世界
        /// </summary>
        /// <param tweenName="rot">世界旋转角度</param>
        public virtual void element_WorldRotationSet(Quaternion rot)
        {
            RectTransform.rotation = rot;
            if (DebugState)
                util_Tools.Func_PrintInfo("元素控件通知", "元素的世界旋转设置为：" + rot, HudMsgState.通知);
        }

        /// <summary>
        /// Hud元素 - 位置偏移设置
        /// </summary>
        /// <param tweenName="offset">空间偏移</param>
        public virtual void element_PositionOffset(Vector3 offset)
        {
            RectTransform.anchoredPosition3D += offset;

            if (DebugState)
                util_Tools.Func_PrintInfo("元素控件通知", "元素的3D锚点位置偏移设置为：" + offset, HudMsgState.通知);
        }

        /// <summary>
        /// Hud元素 - 缩放设置
        /// </summary>
        /// <param tweenName="sca">缩放大小</param>
        public virtual void element_ScaleSet(Vector3 sca)
        {
            RectTransform.localScale = sca;
            if (DebugState)
                util_Tools.Func_PrintInfo("元素控件通知", "元素的缩放设置为：" + sca, HudMsgState.通知);
        }

        /// <summary>
        /// Hud元素 - 检查自身是否已从对象池生成或者为启用/禁用
        /// </summary>
        /// <returns>返回True：启用并且当前已被对象池生成到场景，返回False：禁用并且在对象池中</returns>
        public virtual bool element_ActiveState()
        {
            return gameObject.activeSelf;
        }

        /// <summary>
        /// Hud元素 - 创建ID编号
        /// </summary>
        /// <param tweenName="IDList"></param>
        /// <returns></returns>
        public virtual int element_CreateID(HudElementNode[] ElementNodes)
        {
            List<int> ids = new List<int>();
            for (int i = 0; i < ElementNodes.Length; i++)
            {
                ids.Add(ElementNodes[i].ID);
            }

            int ran_id = Random.Range(1111, 9999);
            while (true)
            {
                if (ids.Contains(ran_id))
                {
                    ran_id = Random.Range(111111, 999999);
                }
                else
                {
                    this.ID = ran_id;
                    break;
                }
            }

            if (DebugState)
                util_Tools.Func_PrintInfo("元素控件通知", "生成的随机ID为：" + ran_id, HudMsgState.通知);
            return ran_id;
        }

        #endregion

        #region RMS

        /// <summary>
        /// 根据方案名称获取RMS节点参数
        /// </summary>
        /// <param tweenName="solution"></param>
        public OriginalLayoutInfo elelemt_RMS_Get(string solution)
        {
            OriginalLayoutInfo info = null;
            for (int i = 0; i < RMS_InfoList.Count; i++)
            {
                if (solution == RMS_InfoList[i].LayoutName)
                {
                    info = RMS_InfoList[i];
                    break;
                }
            }
            return info;
        }

        /// <summary>
        /// Hud元素 - 是否开启设计布局模式
        /// </summary>
        public void element_RMSMode_Enabled(bool state)
        {
            RMS_Enabled = state;
        }

        #endregion

        #region 进入

        /// <summary>
        /// 元素动画 - 进入
        /// </summary>
        public virtual void element_In(Motion_Creator args = null, UnityAction act_InComplete = null)
        {
            if (DebugState)
                util_Tools.Func_PrintInfo("元素控件通知", "杀死元素自身（透明度、位移、旋转）动画！", HudMsgState.警告);
            Tween_Alpha.Kill();
            Tween_Move.Kill();
            Tween_Rotation.Kill();

            if (DebugState)
                util_Tools.Func_PrintInfo("元素控件通知", "进入动画开始！", HudMsgState.通知);
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
            if (args.Alpha.Ease != Ease.Unset)
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "播放透明度动画 - 根据缓动参数", HudMsgState.通知);
                Tween_Alpha = DOTween.To(() => Alpha, x => Alpha = x, 1, args.Alpha.Duration * Hud_Manager.Instance.DurationMultiply * Element_Animators_GlobalDuration).SetEase(args.Alpha.Ease).SetDelay(args.Alpha.Delay).SetAutoKill(true).OnUpdate(() =>
                {
                    if (args.MotionAnimateEndState == MotionAnimateEndState.以_透明度为准)
                    {
                        if (act_on_element_in_progress != null)
                            act_on_element_in_progress(Tween_Alpha.ElapsedPercentage());
                    }
                }).OnComplete(() =>
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
                });
            }

            ///---动画 - 透明度_Alpha（Curve）
            if (args.Alpha.Ease == Ease.Unset)
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "播放透明度动画 - 根据曲线参数", HudMsgState.通知);
                Tween_Alpha = DOTween.To(() => Alpha, x => Alpha = x, 1, args.Alpha.Duration * Hud_Manager.Instance.DurationMultiply * Element_Animators_GlobalDuration).SetEase(args.Alpha.Curve).SetDelay(args.Alpha.Delay).SetAutoKill(true).OnUpdate(() =>
                {
                    if (args.MotionAnimateEndState == MotionAnimateEndState.以_透明度为准)
                    {
                        if (act_on_element_in_progress != null)
                            act_on_element_in_progress(Tween_Alpha.ElapsedPercentage());
                    }
                }).OnComplete(() =>
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
                 });
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
                if (args.Movement.Ease != Ease.Unset)
                {
                    if (DebugState)
                        util_Tools.Func_PrintInfo("元素控件通知", "播放运动动画 - 根据缓动参数", HudMsgState.通知);
                    if (args.Movement.Movement == HudMotion_Movement.W_中心缩放)
                    {
                        Tween_Move = RectTransform.DOScale(endvalue, args.Movement.Duration * Hud_Manager.Instance.DurationMultiply * Element_Animators_GlobalDuration).From(fromvalue).SetDelay(args.Movement.Delay).SetEase(args.Movement.Ease).SetAutoKill(true).OnUpdate(() =>
                        {
                            if (args.MotionAnimateEndState == MotionAnimateEndState.以_移动为准)
                            {
                                if (act_on_element_in_progress != null)
                                    act_on_element_in_progress(Tween_Move.ElapsedPercentage());
                            }
                        }).OnComplete(() =>
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
                         });
                    }
                    else
                    {
                        Tween_Move = RectTransform.DOLocalMove(endvalue, args.Movement.Duration * Hud_Manager.Instance.DurationMultiply * Element_Animators_GlobalDuration).From(fromvalue, true, true).SetRelative(true).SetDelay(args.Movement.Delay).SetEase(args.Movement.Ease).SetAutoKill(true).OnUpdate(() =>
                        {
                            if (args.MotionAnimateEndState == MotionAnimateEndState.以_移动为准)
                            {
                                if (act_on_element_in_progress != null)
                                    act_on_element_in_progress(Tween_Move.ElapsedPercentage());
                            }
                        }).OnComplete(() =>
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
                          });
                    }
                }

                ///---动画 - 位移（Curve）
                if (args.Movement.Ease == Ease.Unset)
                {
                    if (DebugState)
                        util_Tools.Func_PrintInfo("元素控件通知", "播放运动动画 - 根据曲线参数", HudMsgState.通知);
                    if (args.Movement.Movement == HudMotion_Movement.W_中心缩放)
                    {
                        Tween_Move = RectTransform.DOScale(endvalue, args.Movement.Duration * Hud_Manager.Instance.DurationMultiply * Element_Animators_GlobalDuration).From(fromvalue).SetDelay(args.Movement.Delay).SetEase(args.Movement.Curve).SetAutoKill(true).OnUpdate(() =>
                        {
                            if (args.MotionAnimateEndState == MotionAnimateEndState.以_移动为准)
                            {
                                if (act_on_element_in_progress != null)
                                    act_on_element_in_progress(Tween_Move.ElapsedPercentage());
                            }
                        }).OnComplete(() =>
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
                          });
                    }
                    else
                    {
                        Tween_Move = RectTransform.DOLocalMove(endvalue, args.Movement.Duration * Hud_Manager.Instance.DurationMultiply * Element_Animators_GlobalDuration).From(fromvalue, true, true).SetRelative(true).SetDelay(args.Movement.Delay).SetEase(args.Movement.Curve).SetAutoKill(true).OnUpdate(() =>
                        {
                            if (args.MotionAnimateEndState == MotionAnimateEndState.以_移动为准)
                            {
                                if (act_on_element_in_progress != null)
                                    act_on_element_in_progress(Tween_Move.ElapsedPercentage());
                            }
                        }).OnComplete(() =>
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
                          });
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

                if (args.Rotation.Ease != Ease.Unset)
                {
                    if (DebugState)
                        util_Tools.Func_PrintInfo("元素控件通知", "播放旋转动画 - 根据缓动参数", HudMsgState.通知);
                    Tween_Rotation = RectTransform.DOLocalRotate(endvalue, args.Rotation.Duration * Hud_Manager.Instance.DurationMultiply * Element_Animators_GlobalDuration).From(fromvalue, true, true).SetRelative(true).SetDelay(args.Rotation.Delay).SetEase(args.Rotation.Ease).SetAutoKill(true).OnUpdate(() =>
                    {
                        if (args.MotionAnimateEndState == MotionAnimateEndState.以_旋转为准)
                        {
                            if (act_on_element_in_progress != null)
                                act_on_element_in_progress(Tween_Rotation.ElapsedPercentage());
                        }
                    }).OnComplete(() =>
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
                    });
                }

                if (args.Rotation.Ease == Ease.Unset)
                {
                    if (DebugState)
                        util_Tools.Func_PrintInfo("元素控件通知", "播放旋转动画 - 根据曲线参数", HudMsgState.通知);
                    Tween_Rotation = RectTransform.DOLocalRotate(endvalue, args.Rotation.Duration * Hud_Manager.Instance.DurationMultiply * Element_Animators_GlobalDuration).From(fromvalue, true, true).SetRelative(true).SetDelay(args.Rotation.Delay).SetEase(args.Rotation.Curve).SetAutoKill(true).OnUpdate(() =>
                    {
                        if (args.MotionAnimateEndState == MotionAnimateEndState.以_旋转为准)
                        {
                            if (act_on_element_in_progress != null)
                                act_on_element_in_progress(Tween_Rotation.ElapsedPercentage());
                        }
                    }).OnComplete(() =>
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
                      });
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
                util_Tools.Func_PrintInfo("元素控件通知", "元素进入前逻辑调用", HudMsgState.通知);
            AnimateState = HudElementAnimateState.Animating;

            Animating = true;

            if (DebugState)
                util_Tools.Func_PrintInfo("元素控件通知", "禁用Canvas像素对齐！", HudMsgState.警告);

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
                util_Tools.Func_PrintInfo("元素控件通知", "元素进入后逻辑调用", HudMsgState.通知);
            AnimateState = HudElementAnimateState.Static;

            Animating = false;

            if (DebugState)
                util_Tools.Func_PrintInfo("元素控件通知", "恢复Canvas像素对齐！", HudMsgState.警告);

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

        #region 退出

        /// <summary>
        /// 元素动画 - 退出
        /// </summary>
        public virtual void element_Out(Motion_Recycler args = null, UnityAction act_OutComplete = null)
        {
            if (DebugState)
                util_Tools.Func_PrintInfo("元素控件通知", "杀死元素自身（透明度、位移、旋转）动画！", HudMsgState.警告);
            Tween_Alpha.Kill();
            Tween_Move.Kill();
            Tween_Rotation.Kill();

            if (DebugState)
                util_Tools.Func_PrintInfo("元素控件通知", "退出动画开始！", HudMsgState.通知);

            #region 动作 - 动画开始
            if (act_on_element_out_start != null)
                act_on_element_out_start(this);
            if (eve_on_element_out_start != null)
                eve_on_element_out_start.Invoke();
            element_Out_Start();
            #endregion

            #region 动作 - 透明度动画
            ///---动画 - 透明度_Alpha（Ease）
            if (args.Alpha.Ease != Ease.Unset)
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "播放透明度动画 - 根据缓动参数", HudMsgState.通知);
                Tween_Alpha = DOTween.To(() => Alpha, x => Alpha = x, 0, args.Alpha.Duration * Hud_Manager.Instance.DurationMultiply * Element_Animators_GlobalDuration).SetEase(args.Alpha.Ease).SetDelay(args.Alpha.Delay).SetAutoKill(true).OnUpdate(() =>
                {
                    if (args.MotionAnimateEndState == MotionAnimateEndState.以_透明度为准)
                    {
                        if (act_on_element_out_progress != null)
                            act_on_element_out_progress(Tween_Alpha.ElapsedPercentage());
                    }
                }).OnComplete(() =>
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
                 });
            }

            ///---动画 - 透明度_Alpha（Curve）
            if (args.Alpha.Ease == Ease.Unset)
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "播放透明度动画 - 根据曲线参数", HudMsgState.通知);
                Tween_Alpha = DOTween.To(() => Alpha, x => Alpha = x, 0, args.Alpha.Duration * Hud_Manager.Instance.DurationMultiply * Element_Animators_GlobalDuration).SetEase(args.Alpha.Curve).SetDelay(args.Alpha.Delay).SetAutoKill(true).OnUpdate(() =>
                {
                    if (args.MotionAnimateEndState == MotionAnimateEndState.以_透明度为准)
                    {
                        if (act_on_element_out_progress != null)
                            act_on_element_out_progress(Tween_Alpha.ElapsedPercentage());
                    }
                }).OnComplete(() =>
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
                  });
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
                if (args.Movement.Ease != Ease.Unset)
                {
                    if (DebugState)
                        util_Tools.Func_PrintInfo("元素控件通知", "播放运动动画 - 根据缓动参数", HudMsgState.通知);
                    if (args.Movement.Movement == HudMotion_Movement.W_中心缩放)
                    {
                        Tween_Move = RectTransform.DOScale(endvalue, args.Movement.Duration * Hud_Manager.Instance.DurationMultiply * Element_Animators_GlobalDuration).SetDelay(args.Movement.Delay).SetEase(args.Movement.Ease).SetAutoKill(true).OnUpdate(() =>
                        {
                            if (args.MotionAnimateEndState == MotionAnimateEndState.以_移动为准)
                            {
                                if (act_on_element_out_progress != null)
                                    act_on_element_out_progress(Tween_Move.ElapsedPercentage());
                            }
                        }).OnComplete(() =>
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
                        });
                    }
                    else
                    {
                        Tween_Move = RectTransform.DOLocalMove(endvalue, args.Movement.Duration * Hud_Manager.Instance.DurationMultiply * Element_Animators_GlobalDuration).SetRelative(true).SetDelay(args.Movement.Delay).SetEase(args.Movement.Ease).SetAutoKill(true).OnUpdate(() =>
                {
                    if (args.MotionAnimateEndState == MotionAnimateEndState.以_移动为准)
                    {
                        if (act_on_element_out_progress != null)
                            act_on_element_out_progress(Tween_Move.ElapsedPercentage());
                    }
                }).OnComplete(() =>
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
                          });
                    }
                }

                ///---动画 - 位移（Curve）
                if (args.Movement.Ease == Ease.Unset)
                {
                    if (DebugState)
                        util_Tools.Func_PrintInfo("元素控件通知", "播放运动动画 - 根据曲线参数", HudMsgState.通知);
                    if (args.Movement.Movement == HudMotion_Movement.W_中心缩放)
                    {
                        Tween_Move = RectTransform.DOScale(endvalue, args.Movement.Duration * Hud_Manager.Instance.DurationMultiply * Element_Animators_GlobalDuration).SetDelay(args.Movement.Delay).SetEase(args.Movement.Curve).SetAutoKill(true).OnUpdate(() =>
                        {
                            if (args.MotionAnimateEndState == MotionAnimateEndState.以_移动为准)
                            {
                                if (act_on_element_out_progress != null)
                                    act_on_element_out_progress(Tween_Move.ElapsedPercentage());
                            }
                        }).OnComplete(() =>
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
                          });
                    }
                    else
                    {
                        Tween_Move = RectTransform.DOLocalMove(endvalue, args.Movement.Duration * Hud_Manager.Instance.DurationMultiply * Element_Animators_GlobalDuration).SetRelative(true).SetDelay(args.Movement.Delay).SetEase(args.Movement.Curve).SetAutoKill(true).OnUpdate(() =>
                        {
                            if (args.MotionAnimateEndState == MotionAnimateEndState.以_移动为准)
                            {
                                if (act_on_element_out_progress != null)
                                    act_on_element_out_progress(Tween_Move.ElapsedPercentage());
                            }
                        }).OnComplete(() =>
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
                        });
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

                if (args.Rotation.Ease != Ease.Unset)
                {
                    if (DebugState)
                        util_Tools.Func_PrintInfo("元素控件通知", "播放旋转动画 - 根据缓动参数", HudMsgState.通知);
                    Tween_Rotation = RectTransform.DOLocalRotate(endvalue, args.Rotation.Duration * Hud_Manager.Instance.DurationMultiply * Element_Animators_GlobalDuration).SetRelative(true).SetDelay(args.Rotation.Delay).SetEase(args.Rotation.Ease).SetAutoKill(true).OnUpdate(() =>
                    {
                        if (args.MotionAnimateEndState == MotionAnimateEndState.以_旋转为准)
                        {
                            if (act_on_element_out_progress != null)
                                act_on_element_out_progress(Tween_Rotation.ElapsedPercentage());
                        }
                    }).OnComplete(() =>
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
                      });
                }

                if (args.Rotation.Ease == Ease.Unset)
                {
                    if (DebugState)
                        util_Tools.Func_PrintInfo("元素控件通知", "播放旋转动画 - 根据曲线参数", HudMsgState.通知);
                    Tween_Rotation = RectTransform.DOLocalRotate(endvalue, args.Rotation.Duration * Hud_Manager.Instance.DurationMultiply * Element_Animators_GlobalDuration).SetRelative(true).SetDelay(args.Rotation.Delay).SetEase(args.Rotation.Curve).SetAutoKill(true).OnUpdate(() =>
                        {
                            if (args.MotionAnimateEndState == MotionAnimateEndState.以_旋转为准)
                            {
                                if (act_on_element_out_progress != null)
                                    act_on_element_out_progress(Tween_Rotation.ElapsedPercentage());
                            }
                        }).OnComplete(() =>
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
                      });
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
                util_Tools.Func_PrintInfo("元素控件通知", "退出前逻辑调用", HudMsgState.通知);

            ///---元素动画状态变为动画中
            AnimateState = HudElementAnimateState.Animating;

            if (DebugState)
                util_Tools.Func_PrintInfo("元素控件通知", "禁用Canvas像素对齐！", HudMsgState.警告);

            if (Hud_Manager.Instance.UseAutoPerfectPixel)
                Hud_Manager.Instance.hm_UsePixelPerfect(false);

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
                util_Tools.Func_PrintInfo("元素控件通知", "退出后逻辑调用", HudMsgState.通知);

            ///---元素动画状态变为静态
            AnimateState = HudElementAnimateState.Static;

            if (DebugState)
                util_Tools.Func_PrintInfo("元素控件通知", "恢复Canvas像素对齐！", HudMsgState.警告);

            ///---像素对齐开启
            if (Hud_Manager.Instance.UseAutoPerfectPixel)
                Hud_Manager.Instance.hm_UsePixelPerfect(true);

            ///---元素动画开关为关
            Animating = false;

            ///---移除跟踪器
            element_ObjectTracker_Remove();

            ///---所有动画重置
            Animators_Rewind();

            if (DebugState)
                util_Tools.Func_PrintInfo("元素控件通知", "将所有子节点中的Animator的动画都立即杀死！", HudMsgState.通知);
            for (int i = 0; i < AnimatorNodes.Count; i++)
            {
                for (int s = 0; s < AnimatorNodes[i].Animator.AnimateTweenNodes.Count; s++)
                {
                    AnimatorNodes[i].Animator.AnimateTweenNodes[s].Tweener.Kill();
                }
            }

            if (DebugState)
                util_Tools.Func_PrintInfo("元素控件通知", "从HudManager的布局列表中移除！", HudMsgState.通知);
            ///---从管理器的布局中移除元素项以便于下一次使用（元素生成后会在HudManager中的LayoutAnchors中记录赋值）
            for (int i = 0; i < Hud_Manager.Instance.Anchors_Layout_Screen.Count; i++)
            {
                for (int s = 0; s < Hud_Manager.Instance.Anchors_Layout_Screen[i].HudElementInfos.Count; s++)
                {
                    if (this.ID == Hud_Manager.Instance.Anchors_Layout_Screen[i].HudElementInfos[s].ID)
                    {
                        Hud_Manager.Instance.Anchors_Layout_Screen[i].HudElementInfos.RemoveAt(s);
                    }
                }
            }

            if (DebugState)
                util_Tools.Func_PrintInfo("元素控件通知", "回收到元素池！", HudMsgState.通知);
            if (CreateState == HudElementCreateState.Created)
            {
                ///---回收元素
                Hud_Manager.Instance.hm_ElementLibrary_Despawn(this);
            }
        }

        #endregion

        #region 元素动作器

        /// <summary>
        /// 检测是否存在元素动作器
        /// </summary>
        /// <returns></returns>
        public bool element_TriggerActionIsExist()
        {
            if (TriggerAction != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 获取元素动作器
        /// </summary>
        /// <returns></returns>
        public Hud_ElementTriggerAction element_GetTriggerAction()
        {
            if (element_TriggerActionIsExist())
                return TriggerAction;
            else
                return null;
        }

        #endregion

        #region 按钮

        /// <summary>
        /// 启用或禁用所有按钮的交互
        /// </summary>
        /// <param tweenName="treeState">状态</param>
        public void element_Button_InteractableSetAll(bool state)
        {
            if (ButtonNodes.Count <= 0)
                return;
            for (int i = 0; i < ButtonNodes.Count; i++)
            {
                ButtonNodes[i].Button.interactable = state;
            }
            if (state)
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "启用元素下所有按钮交互！", HudMsgState.通知);
            }
            else
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "禁用元素下所有按钮交互！", HudMsgState.通知);
            }
        }

        /// <summary>
        /// 启用或禁用所有按钮
        /// </summary>
        /// <param tweenName="treeState">状态</param>
        public void element_Button_EnableSetAll(bool state)
        {
            for (int i = 0; i < ButtonNodes.Count; i++)
            {
                ButtonNodes[i].Button.enabled = state;
            }
            if (state)
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "启用元素下所有按钮脚本！", HudMsgState.通知);
            }
            else
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "禁用元素下所有按钮脚本！", HudMsgState.通知);
            }
        }

        /// <summary>
        /// 获取目标按钮
        /// </summary>
        /// <param tweenName="indicator">标识名称</param>
        public Hud_Button element_Button_Get(string indicator)
        {
            Hud_Button btn = null;
            for (int i = 0; i < ButtonNodes.Count; i++)
            {
                if (ButtonNodes[i].Button.Indicator == indicator)
                {
                    btn = ButtonNodes[i].Button;
                }
            }
            if (btn == null)
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "未找到对应标识的按钮！", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "已获取到标识为 " + indicator + " 的按钮！", HudMsgState.通知);
            }
            return btn;
        }

        /// <summary>
        /// 获取所有按钮
        /// </summary>
        public Hud_Button[] element_Button_GetAll(bool IgnoreOptionBtn = true)
        {
            List<Hud_Button> btnlist = new List<Hud_Button>();
            for (int i = 0; i < ButtonNodes.Count; i++)
            {
                if (IgnoreOptionBtn)
                    if (ButtonNodes[i].Button.IsOptionButton)
                        continue;
                btnlist.Add(ButtonNodes[i].Button);
            }
            if (btnlist.Count <= 0)
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "获取的按钮列表为空！", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "已获取到" + ButtonNodes.Count + " 个按钮！", HudMsgState.通知);
            }
            return btnlist.ToArray();
        }

        /// <summary>
        /// 清空所有按钮的事件
        /// </summary>
        public void element_Button_EventsClear(bool IgnoreOptionBtn = true)
        {
            for (int i = 0; i < ButtonNodes.Count; i++)
            {
                if (IgnoreOptionBtn)
                    if (ButtonNodes[i].Button.IsOptionButton)
                        continue;
                ButtonNodes[i].Button.btn_EventsClear();
            }
        }

        /// <summary>
        /// 清空所有按钮的委托
        /// </summary>
        public void element_Button_ActionsClear(bool IgnoreOptionBtn = true)
        {
            for (int i = 0; i < ButtonNodes.Count; i++)
            {
                if (IgnoreOptionBtn)
                    if (ButtonNodes[i].Button.IsOptionButton)
                        continue;
                ButtonNodes[i].Button.btn_ActionsClear();
            }
        }

        /// <summary>
        /// 清空目标按钮的所有委托
        /// </summary>
        /// <param tweenName="btn"></param>
        public void element_Button_Target_ActionsClear(Hud_Button btn, bool IgnoreOptionBtn = true)
        {
            if (IgnoreOptionBtn)
                if (btn.IsOptionButton)
                    return;
            btn.btn_ActionsClear();
        }

        /// <summary>
        /// 清空目标按钮的所有事件
        /// </summary>
        /// <param tweenName="btn"></param>
        public void element_Button_Target_EventsClear(Hud_Button btn, bool IgnoreOptionBtn = true)
        {
            if (IgnoreOptionBtn)
                if (btn.IsOptionButton)
                    return;
            btn.btn_EventsClear();
        }

        #endregion

        #region 选项

        /// <summary>
        /// 启用或禁用所有选项的交互
        /// </summary>
        /// <param tweenName="treeState">状态</param>
        public void element_Option_InteractableSetAll(bool state)
        {
            if (OptionNodes.Count <= 0)
                return;
            for (int i = 0; i < OptionNodes.Count; i++)
            {
                for (int s = 0; s < OptionNodes[i].Option.OptionButtonNodes.Count; s++)
                {
                    OptionNodes[i].Option.OptionButtonNodes[s].Button.interactable = state;
                }
            }
            if (state)
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "启用元素下所有选项交互！", HudMsgState.通知);
            }
            else
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "禁用元素下所有选项交互！", HudMsgState.通知);
            }
        }

        /// <summary>
        /// 启用或禁用所有选项
        /// </summary>
        /// <param tweenName="treeState">状态</param>
        public void element_Option_EnableSetAll(bool state)
        {
            for (int i = 0; i < OptionNodes.Count; i++)
            {
                OptionNodes[i].Option.enabled = state;
            }
            if (state)
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "启用元素下所有选项脚本！", HudMsgState.通知);
            }
            else
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "禁用元素下所有选项脚本！", HudMsgState.通知);
            }
        }

        /// <summary>
        /// 获取目标选项
        /// </summary>
        /// <param tweenName="indicator">标识名称</param>
        public Hud_Option element_Option_Get(string indicator)
        {
            Hud_Option opt = null;
            for (int i = 0; i < OptionNodes.Count; i++)
            {
                if (OptionNodes[i].Option.Indicator == indicator)
                {
                    opt = OptionNodes[i].Option;
                }
            }
            if (opt == null)
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "未找到对应标识的选项！", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "已获取到标识为 " + indicator + " 的选项！", HudMsgState.通知);
            }
            return opt;
        }

        /// <summary>
        /// 获取所有选项
        /// </summary>
        public Hud_Option[] element_Option_GetAll()
        {
            List<Hud_Option> optlist = new List<Hud_Option>();
            for (int i = 0; i < OptionNodes.Count; i++)
            {
                optlist.Add(OptionNodes[i].Option);
            }
            if (optlist.Count <= 0)
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "获取的选项列表为空！", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "已获取到" + OptionNodes.Count + " 个选项！", HudMsgState.通知);
            }
            return optlist.ToArray();
        }

        /// <summary>
        /// 清空所有选项的事件
        /// </summary>
        public void element_Option_EventsClear()
        {
            for (int i = 0; i < OptionNodes.Count; i++)
            {
                OptionNodes[i].Option.opt_ClearEvents();
            }
        }

        /// <summary>
        /// 清空所有选项的委托
        /// </summary>
        public void element_Option_ActionsClear()
        {
            for (int i = 0; i < OptionNodes.Count; i++)
            {
                OptionNodes[i].Option.opt_ClearActions();
            }
        }

        /// <summary>
        /// 清空目标选项的所有委托
        /// </summary>
        /// <param tweenName="opt"></param>
        public void element_Option_Target_ActionsClear(Hud_Option opt)
        {
            opt.opt_ClearActions();
        }

        /// <summary>
        /// 清空目标选项的所有事件
        /// </summary>
        /// <param tweenName="opt"></param>
        public void element_Option_Target_EventsClear(Hud_Option opt)
        {
            opt.opt_ClearEvents();
        }

        #endregion

        #region 文字

        /// <summary>
        /// 获取目标文字
        /// </summary>
        /// <param tweenName="indicator">标识名称</param>
        public Hud_Text element_Text_Get(string indicator)
        {
            Hud_Text tex = null;
            for (int i = 0; i < TextNodes.Count; i++)
            {
                if (TextNodes[i].Text.Indicator == indicator)
                {
                    tex = TextNodes[i].Text;
                }
            }
            if (tex == null)
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "未找到对应标识的文字！", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "已获取到标识为 " + indicator + " 的文字组件！", HudMsgState.通知);
            }
            return tex;
        }

        /// <summary>
        /// 获取所有文字
        /// </summary>
        public Hud_Text[] element_Text_GetAll()
        {
            List<Hud_Text> textlist = new List<Hud_Text>();
            for (int i = 0; i < TextNodes.Count; i++)
            {
                textlist.Add(TextNodes[i].Text);
            }
            if (textlist.Count <= 0)
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "获取的文字列表为空！", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "已获取到" + TextNodes.Count + " 个文字组件！", HudMsgState.通知);
            }
            return textlist.ToArray();
        }

        #endregion

        #region Tmp文字

        /// <summary>
        /// 获取目标Tmp文字
        /// </summary>
        /// <param tweenName="indicator">标识名称</param>
        public Hud_TmpText element_TmpText_Get(string indicator)
        {
            Hud_TmpText tex = null;
            for (int i = 0; i < TmpTextNodes.Count; i++)
            {
                if (TmpTextNodes[i].TmpText.Indicator == indicator)
                {
                    tex = TmpTextNodes[i].TmpText;
                }
            }
            if (tex == null)
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "未找到对应标识的文字！", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "已获取到标识为 " + indicator + " 的Tmp文字！", HudMsgState.通知);
            }
            return tex;
        }

        /// <summary>
        /// 获取所有Tmp文字
        /// </summary>
        public Hud_TmpText[] element_TmpText_GetAll()
        {
            List<Hud_TmpText> textlist = new List<Hud_TmpText>();
            for (int i = 0; i < TmpTextNodes.Count; i++)
            {
                textlist.Add(TmpTextNodes[i].TmpText);
            }
            if (textlist.Count <= 0)
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "获取的文字列表为空！", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "已获取到" + TmpTextNodes.Count + " 个文字组件！", HudMsgState.通知);
            }
            return textlist.ToArray();
        }

        #endregion

        #region 滑动条

        /// <summary>
        /// 启用或禁用所有滑动条的交互
        /// </summary>
        /// <param tweenName="treeState">状态</param>
        public void element_Slider_InteractableSetAll(bool state)
        {
            if (SliderNodes.Count <= 0)
                return;
            for (int i = 0; i < SliderNodes.Count; i++)
            {
                SliderNodes[i].Slider.interactable = state;
            }
            if (state)
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "启用元素下所有滑动条交互！", HudMsgState.通知);
            }
            else
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "禁用元素下所有滑动条交互！", HudMsgState.通知);
            }
        }

        /// <summary>
        /// 启用或禁用所有滑动条
        /// </summary>
        /// <param tweenName="treeState">状态</param>
        public void element_Slider_EnableSetAll(bool state)
        {
            for (int i = 0; i < SliderNodes.Count; i++)
            {
                SliderNodes[i].Slider.enabled = state;
            }
            if (state)
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "启用元素下所有滑动条脚本！", HudMsgState.通知);
            }
            else
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "禁用元素下所有滑动条脚本！", HudMsgState.通知);
            }
        }

        /// <summary>
        /// 获取目标滑动条
        /// </summary>
        /// <param tweenName="indicator">标识名称</param>
        public Hud_Slider element_Slider_Get(string indicator)
        {
            Hud_Slider sli = null;
            for (int i = 0; i < SliderNodes.Count; i++)
            {
                if (SliderNodes[i].Slider.Indicator == indicator)
                {
                    sli = SliderNodes[i].Slider;
                }
            }
            if (sli == null)
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "未找到对应标识的滑动条！", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "已获取到标识为 " + indicator + " 的滑动条！", HudMsgState.通知);
            }
            return sli;
        }

        /// <summary>
        /// 获取所有滑动条
        /// </summary>
        public Hud_Slider[] element_Slider_GetAll()
        {
            List<Hud_Slider> sliderlist = new List<Hud_Slider>();
            for (int i = 0; i < SliderNodes.Count; i++)
            {
                sliderlist.Add(SliderNodes[i].Slider);
            }
            if (sliderlist.Count <= 0)
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "获取的滑动条列表为空！", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "已获取到" + SliderNodes.Count + " 个滑动条！", HudMsgState.通知);
            }
            return sliderlist.ToArray();
        }

        /// <summary>
        /// 清空所有滑动条的事件
        /// </summary>
        public void element_Slider_EventsClear()
        {
            for (int i = 0; i < SliderNodes.Count; i++)
            {
                SliderNodes[i].Slider.sli_EventsClear();
            }
        }

        /// <summary>
        /// 清空所有滑动条的委托
        /// </summary>
        public void element_Slider_ActionsClear()
        {
            for (int i = 0; i < SliderNodes.Count; i++)
            {
                SliderNodes[i].Slider.sli_ActionsClear();
            }
        }

        /// <summary>
        /// 清空目标滑动条的所有委托
        /// </summary>
        /// <param tweenName="slider"></param>
        public void element_Slider_Target_ActionsClear(Hud_Slider slider)
        {
            slider.sli_ActionsClear();
        }

        /// <summary>
        /// 清空目标滑动条的所有事件
        /// </summary>
        /// <param tweenName="slider"></param>
        public void element_Slider_Target_EventsClear(Hud_Slider slider)
        {
            slider.sli_EventsClear();
        }

        #endregion

        #region 进度条
        /// <summary>
        /// 启用或禁用所有进度条
        /// </summary>
        /// <param tweenName="treeState">状态</param>
        public void element_Progress_EnableSetAll(bool state)
        {
            if (ProgressNodes.Count <= 0)
                return;
            for (int i = 0; i < ProgressNodes.Count; i++)
            {
                ProgressNodes[i].Progress.enabled = state;
            }
            if (state)
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "启用元素下所有进度条脚本！", HudMsgState.通知);
            }
            else
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "禁用元素下所有进度条脚本！", HudMsgState.通知);
            }
        }

        /// <summary>
        /// 获取目标进度条
        /// </summary>
        /// <param tweenName="indicator">标识名称</param>
        public Hud_Progress element_Progress_Get(string indicator)
        {
            Hud_Progress progress = null;
            for (int i = 0; i < ProgressNodes.Count; i++)
            {
                if (ProgressNodes[i].Progress.Indicator == indicator)
                {
                    progress = ProgressNodes[i].Progress;
                }
            }
            if (progress == null)
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "未找到对应标识的进度条！", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "已获取到标识为 " + indicator + " 的进度条！", HudMsgState.通知);
            }
            return progress;
        }

        /// <summary>
        /// 获取所有进度条
        /// </summary>
        public Hud_Progress[] element_Progress_GetAll()
        {
            List<Hud_Progress> progresslist = new List<Hud_Progress>();
            for (int i = 0; i < ProgressNodes.Count; i++)
            {
                progresslist.Add(ProgressNodes[i].Progress);
            }
            if (progresslist.Count <= 0)
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "获取的进度条列表为空！", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "已获取到" + ProgressNodes.Count + " 个进度条！", HudMsgState.通知);
            }
            return progresslist.ToArray();
        }
        #endregion

        #region 开关

        /// <summary>
        /// 启用或禁用所有开关的交互
        /// </summary>
        /// <param tweenName="treeState">状态</param>
        public void element_Toggle_InteractableSetAll(bool state)
        {
            if (ToggleNodes.Count <= 0)
                return;
            for (int i = 0; i < ToggleNodes.Count; i++)
            {
                ToggleNodes[i].Toggle.interactable = state;
            }
            if (state)
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "启用元素下所有开关交互！", HudMsgState.通知);
            }
            else
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "禁用元素下所有开关交互！", HudMsgState.通知);
            }
        }

        /// <summary>
        /// 启用或禁用所有开关
        /// </summary>
        /// <param tweenName="treeState">状态</param>
        public void element_Toggle_EnableSet(bool state)
        {
            for (int i = 0; i < ToggleNodes.Count; i++)
            {
                ToggleNodes[i].Toggle.enabled = state;
            }
            if (state)
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "启用元素下所有开关脚本！", HudMsgState.通知);
            }
            else
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "禁用元素下所有开关脚本！", HudMsgState.通知);
            }
        }

        /// <summary>
        /// 获取目标开关
        /// </summary>
        /// <param tweenName="indicator">标识名称</param>
        public Hud_Toggle element_Toggle_Get(string indicator)
        {
            Hud_Toggle toggle = null;
            for (int i = 0; i < ToggleNodes.Count; i++)
            {
                if (ToggleNodes[i].Toggle.Indicator == indicator)
                {
                    toggle = ToggleNodes[i].Toggle;
                }
            }
            if (toggle == null)
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "未找到对应标识的开关！", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "已获取到标识为 " + indicator + " 的开关！", HudMsgState.通知);
            }
            return toggle;
        }

        /// <summary>
        /// 获取所有开关
        /// </summary>
        public Hud_Toggle[] element_Toggle_GetAll()
        {
            List<Hud_Toggle> togglelist = new List<Hud_Toggle>();
            for (int i = 0; i < ToggleNodes.Count; i++)
            {
                togglelist.Add(ToggleNodes[i].Toggle);
            }
            if (togglelist.Count <= 0)
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "获取的开关列表为空！", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "已获取到" + ToggleNodes.Count + " 个开关！", HudMsgState.通知);
            }
            return togglelist.ToArray();
        }

        /// <summary>
        /// 清空所有开关的事件
        /// </summary>
        public void element_Toggle_EventsClear()
        {
            for (int i = 0; i < ToggleNodes.Count; i++)
            {
                ToggleNodes[i].Toggle.EventsClear();
            }
        }

        /// <summary>
        /// 清空所有开关的委托
        /// </summary>
        public void element_Toggle_ActionsClear()
        {
            for (int i = 0; i < ToggleNodes.Count; i++)
            {
                ToggleNodes[i].Toggle.ActionsClear();
            }
        }

        /// <summary>
        /// 清空目标开关的所有委托
        /// </summary>
        /// <param tweenName="toggle"></param>
        public void element_Toggle_Target_ActionsClear(Hud_Toggle toggle)
        {
            toggle.ActionsClear();
        }

        /// <summary>
        /// 清空目标开关的所有事件
        /// </summary>
        /// <param tweenName="toggle"></param>
        public void element_Toggle_Target_EventsClear(Hud_Toggle toggle)
        {
            toggle.EventsClear();
        }
        #endregion

        #region 容器

        /// <summary>
        /// 启用或禁用所有容器的交互
        /// </summary>
        /// <param tweenName="treeState">状态</param>
        public void element_Container_InteractableSetAll(bool state)
        {
            if (ContainerNodes.Count <= 0)
                return;
            for (int i = 0; i < ContainerNodes.Count; i++)
            {
                if (ContainerNodes[i] != null)
                {
                    ContainerNodes[i].Container.IsEnabled = state;
                }
                else
                {
                    if (DebugState)
                        util_Tools.Func_PrintInfo("元素控件通知", "失效的容器项！索引号：" + i, HudMsgState.通知);
                }
            }
            if (state)
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "启用元素下所有容器功能！", HudMsgState.通知);
            }
            else
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "禁用元素下所有容器功能！", HudMsgState.通知);
            }
        }

        /// <summary>
        /// 启用或禁用所有容器
        /// </summary>
        /// <param tweenName="treeState">状态</param>
        public void element_Container_EnableSetAll(bool state)
        {
            if (ContainerNodes.Count <= 0)
                return;
            for (int i = 0; i < ContainerNodes.Count; i++)
            {
                if (ContainerNodes[i] != null)
                {
                    ContainerNodes[i].Container.enabled = state;
                }
                else
                {
                    if (DebugState)
                        util_Tools.Func_PrintInfo("元素控件通知", "失效的容器项！索引号：" + i, HudMsgState.通知);
                }
            }
            if (state)
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "启用元素下所有容器脚本！", HudMsgState.通知);
            }
            else
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "禁用元素下所有容器脚本！", HudMsgState.通知);
            }
        }

        /// <summary>
        /// 获取目标容器
        /// </summary>
        /// <param tweenName="indicator">标识名称</param>
        public Hud_Container element_Container_Get(string indicator)
        {
            Hud_Container con = null;
            for (int i = 0; i < ContainerNodes.Count; i++)
            {
                if (ContainerNodes[i].Container.Indicator == indicator)
                {
                    con = ContainerNodes[i].Container;
                }
            }
            if (con == null)
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "未找到对应标识的容器！", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "已获取到标识为 " + indicator + " 的容器！", HudMsgState.通知);
            }
            return con;
        }

        /// <summary>
        /// 获取所有容器
        /// </summary>
        public Hud_Container[] element_Container_GetAll()
        {
            List<Hud_Container> conlist = new List<Hud_Container>();
            for (int i = 0; i < ContainerNodes.Count; i++)
            {
                conlist.Add(ContainerNodes[i].Container);
            }
            if (conlist.Count <= 0)
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "获取的容器列表为空！", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    util_Tools.Func_PrintInfo("元素控件通知", "已获取到" + ContainerNodes.Count + " 个容器！", HudMsgState.通知);
            }
            return conlist.ToArray();
        }

        #region Play

        /// <summary>
        /// 播放容器动画
        /// </summary>
        /// <param tweenName="usedelay">播放延迟</param>
        public void element_Container_Animators_PlayAll(bool usedelay = true)
        {
            for (int i = 0; i < ContainerNodes.Count; i++)
            {
                element_Container_Animators_PlayAt(ContainerNodes[i].Container.Indicator, usedelay);
            }
        }

        /// <summary>
        /// 播放容器动画
        /// </summary>
        /// <param tweenName="con_indicator">容器标识名称</param>
        /// <param tweenName="usedelay">播放延迟</param>
        public void element_Container_Animators_PlayAt(string con_indicator, bool usedelay = true)
        {
            for (int i = 0; i < ContainerNodes.Count; i++)
            {
                if (ContainerNodes[i].Container.Indicator == con_indicator)
                    ContainerNodes[i].Container.Con_Animators_PlayAll(usedelay);
            }
        }

        /// <summary>
        /// 播放容器动画
        /// </summary>
        /// <param tweenName="con_indicator">容器标识名称</param>
        /// <param tweenName="item_indicator">项标识名称</param>
        /// <param tweenName="usedelay">播放延迟</param>
        public void element_Container_Animators_PlayAt(string con_indicator, string item_indicator, bool usedelay = true)
        {
            for (int i = 0; i < ContainerNodes.Count; i++)
            {
                if (ContainerNodes[i].Container.Indicator == con_indicator)
                {
                    for (int s = 0; s < ContainerNodes[i].Container.ContainerItems.Count; s++)
                    {
                        if (ContainerNodes[i].Container.ContainerItems[s].Indicator == item_indicator)
                        {
                            ContainerNodes[i].Container.Con_Animators_PlayAt(item_indicator, usedelay);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 播放容器动画
        /// </summary>
        /// <param tweenName="con_indicator">容器标识名称</param>
        /// <param tweenName="item_id">项标识ID</param>
        /// <param tweenName="usedelay">播放延迟</param>
        public void element_Container_Animators_PlayAt(string con_indicator, int item_id, bool usedelay = true)
        {
            for (int i = 0; i < ContainerNodes.Count; i++)
            {
                if (ContainerNodes[i].Container.Indicator == con_indicator)
                {
                    for (int s = 0; s < ContainerNodes[i].Container.ContainerItems.Count; s++)
                    {
                        if (ContainerNodes[i].Container.ContainerItems[s].ID == item_id)
                        {
                            ContainerNodes[i].Container.Con_Animators_PlayAt(item_id, usedelay);
                        }
                    }
                }
            }
        }
        #endregion

        #region Ready
        /// <summary>
        /// 就绪容器动画
        /// </summary>
        //public void element_Container_Animators_ReadyAll()
        //{
        //    for (int i = 0; i < ContainerNodes.Count; i++)
        //    {
        //        element_Container_Animators_ReadyAt(ContainerNodes[i].Container.Indicator);
        //    }
        //}

        /// <summary>
        /// 就绪容器动画
        /// </summary>
        /// <param tweenName="con_indicator">容器标识名称</param>
        //public void element_Container_Animators_ReadyAt(string con_indicator)
        //{
        //    for (int i = 0; i < ContainerNodes.Count; i++)
        //    {
        //        if (ContainerNodes[i].Container.Indicator == con_indicator)
        //            ContainerNodes[i].Container.Con_Animators_ReadyAll();
        //    }
        //}

        /// <summary>
        /// 就绪容器动画
        /// </summary>
        /// <param tweenName="con_indicator">容器标识名称</param>
        /// <param tweenName="item_indicator">项标识名称</param>
        //public void element_Container_Animators_ReadyAt(string con_indicator, string item_indicator)
        //{
        //    for (int i = 0; i < ContainerNodes.Count; i++)
        //    {
        //        if (ContainerNodes[i].Container.Indicator == con_indicator)
        //        {
        //            for (int s = 0; s < ContainerNodes[i].Container.ContainerItems.Count; s++)
        //            {
        //                if (ContainerNodes[i].Container.ContainerItems[s].Indicator == item_indicator)
        //                {
        //                    ContainerNodes[i].Container.Con_Animators_ReadyAt(item_indicator);
        //                }
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// 就绪容器动画
        /// </summary>
        /// <param tweenName="con_indicator">容器标识名称</param>
        /// <param tweenName="item_id">项标识ID</param>
        //public void element_Container_Animators_ReadyAt(string con_indicator, int item_id)
        //{
        //    for (int i = 0; i < ContainerNodes.Count; i++)
        //    {
        //        if (ContainerNodes[i].Container.Indicator == con_indicator)
        //        {
        //            for (int s = 0; s < ContainerNodes[i].Container.ContainerItems.Count; s++)
        //            {
        //                if (ContainerNodes[i].Container.ContainerItems[s].ID == item_id)
        //                {
        //                    ContainerNodes[i].Container.Con_Animators_ReadyAt(item_id);
        //                }
        //            }
        //        }
        //    }
        //}
        #endregion

        #region Rewind
        /// <summary>
        /// 复位容器动画
        /// </summary>
        public void element_Container_Animators_RewindAll()
        {
            for (int i = 0; i < ContainerNodes.Count; i++)
            {
                element_Container_Animators_RewindAt(ContainerNodes[i].Container.Indicator);
            }
        }

        /// <summary>
        /// 复位容器动画
        /// </summary>
        /// <param tweenName="con_indicator">容器标识名称</param>
        public void element_Container_Animators_RewindAt(string con_indicator)
        {
            for (int i = 0; i < ContainerNodes.Count; i++)
            {
                if (ContainerNodes[i].Container.Indicator == con_indicator)
                    ContainerNodes[i].Container.Con_Animators_RewindAt(con_indicator);
            }
        }

        /// <summary>
        /// 复位容器动画
        /// </summary>
        /// <param tweenName="con_indicator">容器标识名称</param>
        /// <param tweenName="item_indicator">项标识名称</param>
        public void element_Container_Animators_RewindAt(string con_indicator, string item_indicator)
        {
            for (int i = 0; i < ContainerNodes.Count; i++)
            {
                if (ContainerNodes[i].Container.Indicator == con_indicator)
                {
                    for (int s = 0; s < ContainerNodes[i].Container.ContainerItems.Count; s++)
                    {
                        if (ContainerNodes[i].Container.ContainerItems[s].Indicator == item_indicator)
                        {
                            ContainerNodes[i].Container.Con_Animators_RewindAt(item_indicator);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 复位容器动画
        /// </summary>
        /// <param tweenName="con_indicator">容器标识名称</param>
        /// <param tweenName="item_id">项标识ID</param>
        public void element_Container_Animators_RewindAt(string con_indicator, int item_id)
        {
            for (int i = 0; i < ContainerNodes.Count; i++)
            {
                if (ContainerNodes[i].Container.Indicator == con_indicator)
                {
                    for (int s = 0; s < ContainerNodes[i].Container.ContainerItems.Count; s++)
                    {
                        if (ContainerNodes[i].Container.ContainerItems[s].ID == item_id)
                        {
                            ContainerNodes[i].Container.Con_Animators_RewindAt(item_id);
                        }
                    }
                }
            }
        }
        #endregion

        #region Actions & Events
        /// <summary>
        /// 清空所有开关的事件
        /// </summary>
        public void element_Container_EventsClear()
        {
            for (int i = 0; i < ContainerNodes.Count; i++)
            {
                ContainerNodes[i].Container.EventsClear();
            }
        }

        /// <summary>
        /// 清空所有开关的委托
        /// </summary>
        public void element_Container_ActionsClear()
        {
            for (int i = 0; i < ContainerNodes.Count; i++)
            {
                ContainerNodes[i].Container.ActionsClear();
            }
        }

        /// <summary>
        /// 清空目标开关的所有委托
        /// </summary>
        /// <param tweenName="toggle"></param>
        public void element_Container_Target_ActionsClear(Hud_Container container)
        {
            container.ActionsClear();
        }

        /// <summary>
        /// 清空目标开关的所有事件
        /// </summary>
        /// <param tweenName="toggle"></param>
        public void element_Container_Target_EventsClear(Hud_Container container)
        {
            container.EventsClear();
        }
        #endregion

        #region ValueSet

        /// <summary>
        /// 修改目标容器项内容
        /// </summary>
        /// <param tweenName="container">目标容器</param>
        /// <param tweenName="indicator">目标项</param>
        /// <param tweenName="value">字符串内容</param>
        public void element_Container_SetValue(Hud_Container container, string indicator, string value)
        {
            container.Con_ChangeItemValue(indicator, value);
        }

        /// <summary>
        /// 修改目标容器项内容
        /// </summary>
        /// <param tweenName="container">目标容器</param>
        /// <param tweenName="indicator">目标项</param>
        /// <param tweenName="value">图片精灵内容</param>
        public void element_Container_SetValue(Hud_Container container, string indicator, Sprite value)
        {
            container.Con_ChangeItemValue(indicator, value);
        }

        /// <summary>
        /// 修改目标容器项内容
        /// </summary>
        /// <param tweenName="container">目标容器</param>
        /// <param tweenName="indicator">目标项</param>
        /// <param tweenName="value">图片内容</param>
        public void element_Container_SetValue(Hud_Container container, string indicator, Texture2D value)
        {
            container.Con_ChangeItemValue(indicator, value);
        }

        /// <summary>
        /// 修改目标容器项内容
        /// </summary>
        /// <param tweenName="container">目标容器</param>
        /// <param tweenName="id">目标项ID</param>
        /// <param tweenName="value">字符串内容</param>
        public void element_Container_SetValue(Hud_Container container, int id, string value)
        {
            container.Con_ChangeItemValue(id, value);
        }

        /// <summary>
        /// 修改目标容器项内容
        /// </summary>
        /// <param tweenName="container">目标容器</param>
        /// <param tweenName="id">目标项ID</param>
        /// <param tweenName="value">图片精灵内容</param>
        public void element_Container_SetValue(Hud_Container container, int id, Sprite value)
        {
            container.Con_ChangeItemValue(id, value);
        }

        /// <summary>
        /// 修改目标容器项内容
        /// </summary>
        /// <param tweenName="container">目标容器</param>
        /// <param tweenName="id">目标项ID</param>
        /// <param tweenName="value">图片内容</param>
        public void element_Container_SetValue(Hud_Container container, int id, Texture2D value)
        {
            container.Con_ChangeItemValue(id, value);
        }

        /// <summary>
        /// 修改目标容器项内容
        /// </summary>
        /// <param tweenName="container">目标容器</param>
        /// <param tweenName="id">目标项ID</param>
        /// <param tweenName="value">图片内容</param>
        public void element_Container_SetValue_RawImage(Hud_Container container, int id, Texture2D value)
        {
            container.Con_ChangeItemValue_RawImage(id, value);
        }

        /// <summary>
        /// 修改目标容器项内容
        /// </summary>
        /// <param tweenName="container">目标容器</param>
        /// <param tweenName="id">目标项ID</param>
        /// <param tweenName="value">图片内容</param>
        public void element_Container_SetValue_RawImage(Hud_Container container, string indicator, Texture2D value)
        {
            container.Con_ChangeItemValue_RawImage(indicator, value);
        }
        #endregion

        #endregion

        #region 跟随

        /// <summary>
        /// Hud跟踪器 - 在ScreenCamera模式下设定跟踪目标
        /// </summary>
        /// <param tweenName="self">跟踪目标</param>
        /// <param tweenName="relative">相对父物体</param>
        /// <param tweenName="target">被跟踪目标</param>
        /// <param tweenName="offset">偏移</param>
        /// <param tweenName="smoottraker">平滑跟踪</param>
        /// <param tweenName="smoottime">平滑事件</param>
        public void element_ObjectTracker_Create(RectTransform self, Transform target, bool smoottraker = false, float smoottime = 5, Vector3 offset = default)
        {
            ///---设定跟踪器的参数
            TrackerArgs info = new TrackerArgs();
            info.SelfObject = self;
            info.TargetObject = target;
            info.Offset = offset;
            info.UseSmoothTracker = smoottraker;
            info.SmoothTime = smoottime;

            ///---创建跟踪器
            element_ObjectTracker_Create();

            ObjectTracker.Tracker_SetTrackerArgs(info);

            if (DebugState)
                util_Tools.Func_PrintInfo("元素控件通知", "创建场景物体跟踪器！", HudMsgState.通知);
        }

        /// <summary>
        /// Hud跟踪器 - 创建
        /// </summary>
        /// <param tweenName="info">跟踪通知</param>
        public void element_ObjectTracker_Create(TrackerArgs info)
        {
            ///---创建跟踪器
            element_ObjectTracker_Create();
            ///---设定跟踪器的参数
            ObjectTracker.Tracker_SetTrackerArgs(info);

            if (DebugState)
                util_Tools.Func_PrintInfo("元素控件通知", "创建场景物体跟踪器！", HudMsgState.通知);
        }

        /// <summary>
        /// 创建物体跟踪器
        /// </summary>
        /// <returns></returns>
        private Hud_ObjectTracker element_ObjectTracker_Create()
        {
            ObjectTracker = gameObject.AddComponent<Hud_ObjectTracker>();
            if (DebugState)
                util_Tools.Func_PrintInfo("元素控件通知", "创建场景物体跟踪器！", HudMsgState.通知);
            return ObjectTracker;
        }

        /// <summary>
        /// 移除跟踪器
        /// </summary>
        public void element_ObjectTracker_Remove()
        {
            if (ObjectTracker != null)
                DestroyImmediate(ObjectTracker, true);
            ObjectTracker = null;

            if (DebugState)
                util_Tools.Func_PrintInfo("元素控件通知", "移除场景物体跟踪器！", HudMsgState.通知);
        }

        #endregion
    }
}