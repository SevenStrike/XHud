namespace SevenStrikeModules.XHud.Hud
{
    using SevenStrikeModules.XTween;
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.Utilitys;
    using System.Collections;
    using System.Collections.Generic;
#if UNITY_EDITOR
    using UnityEditor;
#endif
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;
    using Random = UnityEngine.Random;

    [SerializeField]
    [System.Serializable]
    public class TweenSound
    {
        /// <summary>
        /// 音效剪辑
        /// </summary>
        public AudioClip Sound;
        /// <summary>
        /// 音效路径
        /// </summary>
        public string Path;
        /// <summary>
        /// 触发点百分比
        /// </summary>
        public float Percentage;
        /// <summary>
        /// 已播放状态
        /// </summary>
        public bool IsPlayed;
        /// <summary>
        /// 音量
        /// </summary>
        public float Volume = 1f;
        /// <summary>
        /// 最大音高
        /// </summary>
        public float MaxPitch = 1f;
        /// <summary>
        /// 最小音高
        /// </summary>
        public float MinPitch = 1f;

        /// <summary>
        /// 动作 - 动画器音效 - 播放
        /// </summary>
        public UnityAction<AudioClip> act_on_SoundPlay;

        /// <summary>
        /// 设置音高
        /// </summary>
        /// <param tweenName="min">最小音高</param>
        /// <param tweenName="max">最大音高</param>
        public void SetPitch(float min, float max)
        {
            MinPitch = min;
            MaxPitch = max;
        }

        /// <summary>
        /// 设置音量值
        /// </summary>
        /// <param tweenName="vol">音量值</param>
        public void SetVolume(float vol)
        {
            Volume = vol;
        }
    }

    [SerializeField]
    [System.Serializable]
    public class AnimatorOriginalPoserInfo
    {
        public Vector3 Position;
        public Vector3 Euler;
        public Vector3 Scale;
        public float Alpha;
        public Color Color;
        public Vector2 Size;
        public float Fill;
    }

    [SerializeField]
    [System.Serializable]
    public class TweenNode
    {
        //[Header("--> 基础")]
        /// <summary>
        /// 动画参数 - 标识名称
        /// </summary>
        public string Indicator = "NewTween";
        /// <summary>
        /// 动画参数 - 标识ID
        /// </summary>
        public int ID;
        /// <summary>
        /// 动画参数 - 可用性开关
        /// </summary>
        public bool Enabled = true;
        /// <summary>
        /// 动画参数 - 动画类型
        /// </summary>
        public TweenNodeType Type = TweenNodeType.位移;
        /// <summary>
        /// 动画参数 - 播放时机
        /// </summary>
        public string Timings;
        /// <summary>
        /// 动画参数 - 动画耗时
        /// </summary>
        public float Duration = 1f;
        /// <summary>
        /// 动画参数 - 延时
        /// </summary>
        public float Delay = 0f;
        /// <summary>
        /// 动画参数 - 动画进度
        /// </summary>
        public float Progress = 0f;
        //[Header("--> 运动方式")]
        /// <summary>
        /// 动画参数 - 缓动参数
        /// </summary>
        public EaseMode Ease = EaseMode.OutQuart;
        public string AnimationCurveName = "";
        /// <summary>
        /// 动画参数 - 曲线
        /// </summary>
        public AnimationCurve Curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        #region  原始值
        /// <summary>
        /// 动画参数 - 原始整形数值
        /// </summary>
        public int Original_Int;
        /// <summary>
        /// 动画参数 - 原始浮点数值
        /// </summary>
        public float Original_Float;
        /// <summary>
        /// 动画参数 - 原始2维向量值
        /// </summary>
        public Vector2 Original_Vector2;
        /// <summary>
        /// 动画参数 - 原始3维向量值
        /// </summary>
        public Vector3 Original_Vector3;
        /// <summary>
        /// 动画参数 - 原始4维向量值
        /// </summary>
        public Vector4 Original_Vector4;
        /// <summary>
        /// 动画参数 - 原始颜色值
        /// </summary>
        public Color Original_Color;
        /// <summary>
        /// 动画参数 - 原始文字内容
        /// </summary>
        public string Original_String = "";
        #endregion

        #region  起始值
        /// <summary>
        /// 动画参数 - 是否激活起始
        /// </summary>
        public bool ActivateFrom;
        /// <summary>
        /// 动画参数 - 起始整形数值
        /// </summary>
        public int From_Int;
        /// <summary>
        /// 动画参数 - 起始浮点数值
        /// </summary>
        public float From_Float;
        /// <summary>
        /// 动画参数 - 起始2维向量值
        /// </summary>
        public Vector2 From_Vector2;
        /// <summary>
        /// 动画参数 - 起始3维向量值
        /// </summary>
        public Vector3 From_Vector3;
        /// <summary>
        /// 动画参数 - 起始4维向量值
        /// </summary>
        public Vector4 From_Vector4;
        /// <summary>
        /// 动画参数 - 起始颜色值
        /// </summary>
        public Color From_Color;
        /// <summary>
        /// 动画参数 - 起始文字内容
        /// </summary>
        public string From_String;
        #endregion

        #region  结束值
        /// <summary>
        /// 动画参数 - 是否激活结束
        /// </summary>
        public bool ActivateEnd;
        /// <summary>
        /// 动画参数 - 结束整形数值
        /// </summary>
        public int End_Int;
        /// <summary>
        /// 动画参数 - 结束浮点数值
        /// </summary>
        public float End_Float;
        /// <summary>
        /// 动画参数 - 结束2维向量值
        /// </summary>
        public Vector2 End_Vector2;
        /// <summary>
        /// 动画参数 - 结束3维向量值
        /// </summary>
        public Vector3 End_Vector3;
        /// <summary>
        /// 动画参数 - 结束4维向量值
        /// </summary>
        public Vector4 End_Vector4;
        /// <summary>
        /// 动画参数 - 结束颜色值
        /// </summary>
        public Color End_Color;
        /// <summary>
        /// 动画参数 - 结束文字内容
        /// </summary>
        public string End_String;
        #endregion

        /// <summary>
        /// 动画参数 - 是否激活只运动到结束
        /// </summary>
        public bool ActivateOnlyToEnd;

        //[Header("--> 选项")]
        /// <summary>
        /// 动画参数 - 旋转模式
        /// </summary>
        public XTweenRotationMode RotateMode = XTweenRotationMode.Normal;
        /// <summary>
        /// 动画参数 - 循环模式
        /// </summary>
        public XTween_LoopType LoopType = XTween_LoopType.Restart;
        /// <summary>
        /// 动画参数 - 循环次数
        /// </summary>
        public int LoopCount;
        /// <summary>
        /// 动画参数 - 动画器
        /// </summary>
        public XTween_Interface Tweener;
        /// <summary>
        /// 折叠
        /// </summary>
        public bool IsFold;

        #region UnityAction Int
        /// <summary>
        /// 委托事件 - 当 - 整形数值变化 - 时
        /// </summary>
        public UnityAction<int> Act_On_Int_Changed;
        /// <summary>
        /// 委托事件 - 当 - 整形数值倒退 - 时
        /// </summary>
        public UnityAction<int> Act_On_Int_Rewind;
        /// <summary>
        /// 委托事件 - 当 - 整形数值准备就绪 - 时
        /// </summary>
        public UnityAction<int> Act_On_Int_Ready;
        #endregion

        #region UnityAction Float
        /// <summary>
        /// 委托事件 - 当 - 浮点数值变化 - 时
        /// </summary>
        public UnityAction<float> Act_On_Float_Changed;
        /// <summary>
        /// 委托事件 - 当 - 浮点数值倒退 - 时
        /// </summary>
        public UnityAction<float> Act_On_Float_Rewind;
        /// <summary>
        /// 委托事件 - 当 - 浮点数值准备就绪 - 时
        /// </summary>
        public UnityAction<float> Act_On_Float_Ready;
        #endregion

        #region UnityAction Vector2
        /// <summary>
        /// 委托事件 - 当 - 2维向量变化 - 时
        /// </summary>
        public UnityAction<Vector2> Act_On_Vector2_Changed;
        /// <summary>
        /// 委托事件 - 当 - 2维向量倒退 - 时
        /// </summary>
        public UnityAction<Vector2> Act_On_Vector2_Rewind;
        /// <summary>
        /// 委托事件 - 当 - 2维向量准备就绪 - 时
        /// </summary>
        public UnityAction<Vector2> Act_On_Vector2_Ready;
        #endregion

        #region UnityAction Vector3
        /// <summary>
        /// 委托事件 - 当 - 3维向量变化 - 时
        /// </summary>
        public UnityAction<Vector3> Act_On_Vector3_Changed;
        /// <summary>
        /// 委托事件 - 当 - 3维向量倒退 - 时
        /// </summary>
        public UnityAction<Vector3> Act_On_Vector3_Rewind;
        /// <summary>
        /// 委托事件 - 当 - 3维向量准备就绪 - 时
        /// </summary>
        public UnityAction<Vector3> Act_On_Vector3_Ready;
        #endregion

        #region UnityAction Vector4
        /// <summary>
        /// 委托事件 - 当 - 4维向量变化 - 时
        /// </summary>
        public UnityAction<Vector4> Act_On_Vector4_Changed;
        /// <summary>
        /// 委托事件 - 当 - 4维向量倒退 - 时
        /// </summary>
        public UnityAction<Vector4> Act_On_Vector4_Rewind;
        /// <summary>
        /// 委托事件 - 当 - 4维向量准备就绪 - 时
        /// </summary>
        public UnityAction<Vector4> Act_On_Vector4_Ready;
        #endregion

        #region UnityAction Quaternion
        /// <summary>
        /// 委托事件 - 当 - 四元数变化时 - 时
        /// </summary>
        public UnityAction<Quaternion> Act_On_Quaternion_Changed;
        /// <summary>
        /// 委托事件 - 当 - 四元数倒退 - 时
        /// </summary>
        public UnityAction<Quaternion> Act_On_Quaternion_Rewind;
        /// <summary>
        /// 委托事件 - 当 - 四元数准备就绪 - 时
        /// </summary>
        public UnityAction<Quaternion> Act_On_Quaternion_Ready;
        #endregion

        #region UnityAction Color
        /// <summary>
        /// 委托事件 - 当 - 颜色值变化 - 时
        /// </summary>
        public UnityAction<Color> Act_On_Color_Changed;
        /// <summary>
        /// 委托事件 - 当 - 颜色值倒退 - 时
        /// </summary>
        public UnityAction<Color> Act_On_Color_Rewind;
        /// <summary>
        /// 委托事件 - 当 - 颜色值准备就绪 - 时
        /// </summary>
        public UnityAction<Color> Act_On_Color_Ready;
        #endregion

        #region UnityAction string
        /// <summary>
        /// 委托事件 - 当 - 文字内容变化 - 时
        /// </summary>
        public UnityAction<string> Act_On_Text_Changed;
        /// <summary>
        /// 委托事件 - 当 - 文字内容倒退 - 时
        /// </summary>
        public UnityAction<string> Act_On_Text_Rewind;
        /// <summary>
        /// 委托事件 - 当 - 文字内容准备 - 时
        /// </summary>
        public UnityAction<string> Act_On_Text_Ready;
        #endregion

        #region UnityAction AudioClip
        /// <summary>
        /// 委托事件 - 当 - 动画器音效 - 播放 - 时
        /// </summary>
        public UnityAction<AudioClip> Act_On_SoundPlayed;
        #endregion

        public List<TweenSound> TweenSounds;

        /// <summary>
        /// 清空委托事件
        /// </summary>
        public void ClearActions()
        {
            Act_On_Int_Changed = null;
            Act_On_Int_Rewind = null;

            Act_On_Float_Changed = null;
            Act_On_Float_Rewind = null;

            Act_On_Vector2_Changed = null;
            Act_On_Vector2_Rewind = null;

            Act_On_Vector3_Changed = null;
            Act_On_Vector3_Rewind = null;

            Act_On_Vector4_Changed = null;
            Act_On_Vector4_Rewind = null;

            Act_On_Quaternion_Changed = null;
            Act_On_Quaternion_Rewind = null;

            Act_On_Color_Changed = null;
            Act_On_Color_Rewind = null;

            Act_On_Text_Changed = null;
            Act_On_Text_Rewind = null;
        }

        /// <summary>
        /// 互换数值
        /// </summary>
        public void ExchangeValue()
        {
            //如果是 "起始 -> 结束" 模式，From 和 End互换
            if (ActivateFrom && ActivateEnd && !ActivateOnlyToEnd)
            {
                // 交换整形数值
                int tempInt = From_Int;
                From_Int = End_Int;
                End_Int = tempInt;

                // 交换浮点数值
                float tempFloat = From_Float;
                From_Float = End_Float;
                End_Float = tempFloat;

                // 交换2维向量值
                Vector2 tempVector2 = From_Vector2;
                From_Vector2 = End_Vector2;
                End_Vector2 = tempVector2;

                // 交换3维向量值
                Vector3 tempVector3 = From_Vector3;
                From_Vector3 = End_Vector3;
                End_Vector3 = tempVector3;

                // 交换4维向量值
                Vector4 tempVector4 = From_Vector4;
                From_Vector4 = End_Vector4;
                End_Vector4 = tempVector4;

                // 交换颜色值
                Color tempColor = From_Color;
                From_Color = End_Color;
                End_Color = tempColor;
            }
            //如果是 "起始 -> 默认" 模式，From 和 Original互换
            else if (ActivateFrom && !ActivateEnd && !ActivateOnlyToEnd)
            {
                // 交换整形数值
                int tempInt = From_Int;
                From_Int = Original_Int;
                Original_Int = tempInt;

                // 交换浮点数值
                float tempFloat = From_Float;
                From_Float = Original_Float;
                Original_Float = tempFloat;

                // 交换2维向量值
                Vector2 tempVector2 = From_Vector2;
                From_Vector2 = Original_Vector2;
                Original_Vector2 = tempVector2;

                // 交换3维向量值
                Vector3 tempVector3 = From_Vector3;
                From_Vector3 = Original_Vector3;
                Original_Vector3 = tempVector3;

                // 交换4维向量值
                Vector4 tempVector4 = From_Vector4;
                From_Vector4 = Original_Vector4;
                Original_Vector4 = tempVector4;

                // 交换颜色值
                Color tempColor = From_Color;
                From_Color = Original_Color;
                Original_Color = tempColor;
            }
            //如果是 "默认 -> 结束" 模式，End 和 Original互换
            else if (!ActivateFrom && ActivateEnd && !ActivateOnlyToEnd)
            {
                // 交换整形数值
                int tempInt = End_Int;
                End_Int = Original_Int;
                Original_Int = tempInt;

                // 交换浮点数值
                float tempFloat = End_Float;
                End_Float = Original_Float;
                Original_Float = tempFloat;

                // 交换2维向量值
                Vector2 tempVector2 = End_Vector2;
                End_Vector2 = Original_Vector2;
                Original_Vector2 = tempVector2;

                // 交换3维向量值
                Vector3 tempVector3 = End_Vector3;
                End_Vector3 = Original_Vector3;
                Original_Vector3 = tempVector3;

                // 交换4维向量值
                Vector4 tempVector4 = End_Vector4;
                End_Vector4 = Original_Vector4;
                Original_Vector4 = tempVector4;

                // 交换颜色值
                Color tempColor = End_Color;
                End_Color = Original_Color;
                Original_Color = tempColor;
            }
        }
    }

    [SerializeField]
    [System.Serializable]
    public class TweenNodesCopy
    {
        public List<TweenNode> TweenNodesCopyList;
    }

    /// <summary>
    /// Hud动画器
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
#if UNITY_EDITOR
    [RequireComponent(typeof(XHud_Module_Animator_ColorSynchronizer))]
#endif
    public class XHud_Module_Animator : MonoBehaviour
    {
        /// <summary>
        /// 动画器记录的组件原始形态
        /// </summary>
        [SerializeField] public AnimatorOriginalPoserInfo OriginalAnimatorPoser;

        #region 成员 - 基础信息
        /// <summary>
        /// 动画器编号
        /// </summary>
        [SerializeField] public int ID;
        /// <summary>
        /// 动画器名称
        /// </summary>
        [SerializeField] public string Indicator;
        /// <summary>
        /// 动画器类型
        /// </summary>
        [SerializeField] public ModuleType AnimatorModuleType;
        /// <summary>
        /// 动画节点
        /// </summary>
        [SerializeField] public List<TweenNode> AnimateTweenNodes;
        /// <summary>
        /// 起始动画节点
        /// </summary>
        [SerializeField] public string MainTweenNode;
        #endregion

        #region 成员 - 基础组件
        /// <summary>
        /// 组件 - 变换
        /// </summary>
        [SerializeField] public RectTransform mod_RectTransform;
        /// <summary>
        /// 组件 - Tmp文字
        /// </summary>
        [SerializeField] public XHud_Module_TmpText mod_TmpText;
        /// <summary>
        /// 组件 - 文字
        /// </summary>
        [SerializeField] public XHud_Module_Text mod_Text;
        /// <summary>
        /// 组件 - 图片
        /// </summary>
        [SerializeField] public Image mod_Image;
        /// <summary>
        /// 组件 - Raw图片
        /// </summary>
        [SerializeField] public RawImage mod_RawImage;
        /// <summary>
        /// 用于控制Animator的淡化透明度
        /// </summary>
        [SerializeField] public CanvasGroup CanvasGroup;
        #endregion

        #region 成员 - 特殊组件
        /// <summary>
        /// 组件 - 按钮
        /// </summary>
        [SerializeField] public XHud_Module_Button mod_HudButton;
        #endregion

        #region 成员 - 自定义数值
        /// <summary>
        /// 自定义数值 - INT
        /// </summary>
        [SerializeField] public int CustomValue_Int;
        /// <summary>
        /// 自定义数值 - FLOAT
        /// </summary>
        [SerializeField] public float CustomValue_Float;
        /// <summary>
        /// 自定义数值 - VECTOR4
        /// </summary>
        [SerializeField] public Vector4 CustomValue_Vector4;
        /// <summary>
        /// 自定义数值 - VECTOR3
        /// </summary>
        [SerializeField] public Vector3 CustomValue_Vector3;
        /// <summary>
        /// 自定义数值 - VECTOR2
        /// </summary>
        [SerializeField] public Vector2 CustomValue_Vector2;
        /// <summary>
        /// 自定义数值 - COLOR
        /// </summary>
        [SerializeField] public Color CustomValue_Color;
        #endregion

        #region 成员 - 速率&时间
        /// <summary>
        /// 动画全局速率
        /// </summary>
        [SerializeField] public float Animator_GlobalDuration = 1;
        /// <summary>
        /// 动画列表中筛选出的最小时长
        /// </summary>
        [SerializeField] public float MinTimer;
        /// <summary>
        /// 动画列表中筛选出的最大时长
        /// </summary>
        [SerializeField] public float MaxTimer;
        /// <summary>
        /// 动画列表中筛选出的最小时长且乘以动画器全局速率
        /// </summary>
        [SerializeField] public float MinTimerWithGlobalDuration;
        /// <summary>
        /// 动画列表中筛选出的最大时长且乘以动画器全局速率
        /// </summary>
        [SerializeField] public float MaxTimerWithGlobalDuration;
        #endregion

        #region 成员 - 配色样式索引        
        /// <summary>
        /// 原始色
        /// </summary>
        [SerializeField] public Color OriginalColor = Color.white;
        /// <summary>
        /// 配色名称
        /// </summary>
        [SerializeField] public string ColoriseName;
        #endregion

        #region 成员 - 协程
        /// <summary>
        /// 动画协程
        /// </summary>
        Coroutine CoroutineAnimation;
        #endregion

        #region 成员 - 状态开关
        /// <summary>
        /// 状态调试
        /// </summary>
        [SerializeField] public bool DebugState;
        /// <summary>
        /// 同步库颜色
        /// </summary>
        [SerializeField] public bool SyncLibraryColor;
        /// <summary>
        /// 静音播放
        /// </summary>
        [SerializeField] public bool MutePlay;
        /// <summary>
        /// 动画是否正在播放的中断模式开关
        /// </summary>
        [SerializeField] private bool AnimatingBreakState;
        /// <summary>
        /// 勾选后上级Element将不再控制该动画播放
        /// </summary>
        [SerializeField] public bool IgnoreElementAnimationPlay;
        /// <summary>
        /// 第一次点击的时候记录为首次使用动画器，并且会把所有动画器涉及的参数都收集，只有第一次使用才会起效
        /// </summary>
        [SerializeField] public bool FirstCreateRecordOriginalState;
        /// <summary>
        /// 预览动画开关
        /// </summary>
        [SerializeField] public bool TweenIsPreviewing;
        #endregion

        #region 成员 - 动作
        /// <summary>
        /// 动作 - 动画器 - 播放
        /// </summary>
        public UnityAction act_on_Animator_Play;
        /// <summary>
        /// 动作 - 动画器 - 播放其中一个动画节点
        /// </summary>
        public UnityAction<TweenNode> act_on_Animator_PlayAt;
        /// <summary>
        /// 动作 - 动画器 - 复位
        /// </summary>
        public UnityAction act_on_Animator_Rewind;
        /// <summary>
        /// 动作 - 动画器 - 准备
        /// </summary>
        public UnityAction act_on_Animator_Ready;
        /// <summary>
        /// 动作 - 动画器 - 完成
        /// </summary>
        public UnityAction act_on_Animator_Complete;
        /// <summary>
        /// 动作 - 动画器 - 初始化
        /// </summary>
        public UnityAction act_on_Animator_Initialized;
        /// <summary>
        /// 动作 - 动画器 - 音效触发播放
        /// </summary>
        public UnityAction<AudioClip> act_on_Animator_SoundPlay;
        /// <summary>
        /// 动作 - 动画器 - 动画播放状态
        /// </summary>
        public UnityAction<bool> act_on_Animator_IsAnimating;
        #endregion

        #region 成员 - 事件
        /// <summary>
        /// 事件 - 动画器 - 播放
        /// </summary>
        public UnityEvent eve_on_Animator_Play;
        /// <summary>
        /// 事件 - 动画器 - 播放其中一个动画节点
        /// </summary>
        public UnityEvent<TweenNode> eve_on_Animator_PlayAt;
        /// <summary>
        /// 事件 - 动画器 - 复位
        /// </summary>
        public UnityEvent eve_on_Animator_Rewind;
        /// <summary>
        /// 事件 - 动画器 - 准备
        /// </summary>
        public UnityEvent eve_on_Animator_Ready;
        /// <summary>
        /// 事件 - 动画器 - 完成
        /// </summary>
        public UnityEvent eve_on_Animator_Complete;
        /// <summary>
        /// 事件 - 动画器 - 初始化
        /// </summary>
        public UnityEvent eve_on_Animator_Initialized;
        /// <summary>
        /// 事件 - 动画器 - 音效触发播放
        /// </summary>
        public UnityEvent<AudioClip> eve_on_Animator_SoundPlay;
        /// <summary>
        /// 事件 - 动画器 - 动画播放状态
        /// </summary>
        public UnityEvent<bool> eve_on_Animator_AnimatingState;
        #endregion

        void Awake()
        {
            Initialize();
            Modules_RecognitionType();
        }

        void Start()
        {
            TweenNodeTimersGet();
            SynchroColor();
        }

        void Update()
        {
            AnimationProgressCalculation();
            SynchroColor();
            IsAnimating();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Initialize()
        {
            Modules_Get();
            TweenNodes_AnimateProgressReset();
            if (act_on_Animator_Initialized != null)
                act_on_Animator_Initialized();
            eve_on_Animator_Initialized.Invoke();
        }

        #region 实时刷新
        /// <summary>
        /// 动画器是否全部播放完毕
        /// </summary>
        public bool IsAnimating()
        {
            bool animating = false;
            if (AnimateTweenNodes != null && AnimateTweenNodes.Count > 0)
            {
                for (int i = 0; i < AnimateTweenNodes.Count; i++)
                {
                    if (AnimateTweenNodes[i].Tweener != null && AnimateTweenNodes[i].Tweener.IsActive && AnimateTweenNodes[i].Tweener.IsPlaying)
                    {
                        animating = true;
                        break;
                    }
                }
            }
            if (act_on_Animator_IsAnimating != null)
                act_on_Animator_IsAnimating(animating);
            eve_on_Animator_AnimatingState.Invoke(animating);
            if (animating)
            {
                if (!AnimatingBreakState)
                {
                    AnimatingBreakState = true;
                }
            }
            else
            {
                if (AnimatingBreakState)
                {
                    AnimatingBreakState = false;
                    if (act_on_Animator_Complete != null)
                        act_on_Animator_Complete();
                    eve_on_Animator_Complete.Invoke();
                }
            }
            return animating;
        }
        /// <summary>
        /// 计算每个动画的进度
        /// </summary>
        private void AnimationProgressCalculation()
        {
            if (AnimateTweenNodes.Count <= 0)
                return;
            for (int i = 0; i < AnimateTweenNodes.Count; i++)
            {
                #region 确保动画存在不为空的情况下，计算每个动画的动画进度

                if (AnimateTweenNodes[i].Tweener != null && AnimateTweenNodes[i].Tweener.IsActive && AnimateTweenNodes[i].Tweener.IsPlaying)
                {
                    if (AnimateTweenNodes[i].Progress > 0.985f)
                        AnimateTweenNodes[i].Progress = 1;
                    else
                        AnimateTweenNodes[i].Progress = AnimateTweenNodes[i].Tweener.ElapsedTime / AnimateTweenNodes[i].Tweener.Duration;
                }

                #endregion

                #region 确保动画音效列表不是空的
                if (AnimateTweenNodes[i].TweenSounds.Count > 0)
                {
                    ///---如果动画是循环模式则不会播放音效，以为初始化时动画的Progress为0，此时程序会判定已到达播放音效的触点位置，则会误判发出音效
                    if (AnimateTweenNodes[i].LoopCount == -1)
                        continue;

                    for (int s = 0; s < AnimateTweenNodes[i].TweenSounds.Count; s++)
                    {
                        if (AnimateTweenNodes[i].TweenSounds[s].Sound == null)
                            continue;

                        if (AnimateTweenNodes[i].TweenSounds[s].Percentage >= 1)
                        {
                            if (AnimateTweenNodes[i].Progress >= AnimateTweenNodes[i].TweenSounds[s].Percentage)
                            {
                                if (!AnimateTweenNodes[i].TweenSounds[s].IsPlayed)
                                {
                                    AnimateTweenNodes[i].TweenSounds[s].IsPlayed = true;
                                    AudioClip clip = AnimateTweenNodes[i].TweenSounds[s].Sound;
                                    float vol = AnimateTweenNodes[i].TweenSounds[s].Volume;
                                    float pitch_min = AnimateTweenNodes[i].TweenSounds[s].MinPitch;
                                    float pitch_max = AnimateTweenNodes[i].TweenSounds[s].MaxPitch;
                                    PlayTweenSound(clip, vol, pitch_min, pitch_max);
                                    if (AnimateTweenNodes[i].Act_On_SoundPlayed != null)
                                    {
                                        AnimateTweenNodes[i].Act_On_SoundPlayed(clip);
                                    }
                                }
                            }
                            else
                            {
                                AnimateTweenNodes[i].TweenSounds[s].IsPlayed = false;
                            }
                        }
                        else if (AnimateTweenNodes[i].TweenSounds[s].Percentage < 1)
                        {
                            if (AnimateTweenNodes[i].Progress > AnimateTweenNodes[i].TweenSounds[s].Percentage)
                            {
                                if (!AnimateTweenNodes[i].TweenSounds[s].IsPlayed)
                                {
                                    AnimateTweenNodes[i].TweenSounds[s].IsPlayed = true;
                                    AudioClip clip = AnimateTweenNodes[i].TweenSounds[s].Sound;
                                    float vol = AnimateTweenNodes[i].TweenSounds[s].Volume;
                                    float pitch_min = AnimateTweenNodes[i].TweenSounds[s].MinPitch;
                                    float pitch_max = AnimateTweenNodes[i].TweenSounds[s].MaxPitch;
                                    PlayTweenSound(clip, vol, pitch_min, pitch_max);
                                    if (AnimateTweenNodes[i].Act_On_SoundPlayed != null)
                                    {
                                        AnimateTweenNodes[i].Act_On_SoundPlayed(clip);
                                    }
                                }
                            }
                            else
                            {
                                AnimateTweenNodes[i].TweenSounds[s].IsPlayed = false;
                            }
                        }
                    }
                }
                #endregion
            }
        }
        #endregion

        #region 动画节点操作
        /// <summary>
        /// 创建动画节点
        /// </summary>
        /// <returns></returns>
        public TweenNode TweenNode_Create()
        {
            TweenNode tn = new TweenNode();
            tn.ID = TweenNode_ID_Create();
            AnimateTweenNodes.Add(tn);
            return tn;
        }
        /// <summary>
        /// 创建动画节点
        /// </summary>
        /// <param tweenName="node">动画节点参数</param>
        /// <returns></returns>
        public TweenNode TweenNode_Create(TweenNode node)
        {
            node.ID = TweenNode_ID_Create();
            AnimateTweenNodes.Add(node);
            return node;
        }
        /// <summary>
        /// 移除一个动画节点
        /// </summary>
        /// <param tweenName="name">动画节点名称标识</param>
        public void TweenNode_Remove(string name)
        {
            for (int i = 0; i < AnimateTweenNodes.Count; i++)
            {
                if (AnimateTweenNodes[i].Indicator == name)
                {
                    AnimateTweenNodes.RemoveAt(i);
                }
            }
        }
        /// <summary>
        /// 移除一个动画节点
        /// </summary>
        /// <param tweenName="id">动画节点ID</param>
        public void TweenNode_Remove(int id)
        {
            for (int i = 0; i < AnimateTweenNodes.Count; i++)
            {
                if (AnimateTweenNodes[i].ID == id)
                {
                    AnimateTweenNodes.RemoveAt(i);
                }
            }
        }
        #endregion

        #region 辅助
        /// <summary>
        /// 获取动画器ID
        /// </summary>
        /// <returns></returns>
        public int GetID()
        {
            return ID;
        }
        /// <summary>
        /// 设置动画器ID
        /// </summary>
        /// <param tweenName="id"></param>
        public void SetID(int id)
        {
            ID = id;
        }
        /// <summary>
        /// 获取动画器标识名称
        /// </summary>
        /// <returns></returns>
        public string GetIndicator()
        {
            return Indicator;
        }
        /// <summary>
        /// 设置动画器标识名称
        /// </summary>
        /// <param tweenName="indicator"></param>
        public void SetIndicator(string indicator)
        {
            Indicator = indicator;
        }
        /// <summary>
        /// 获取控件组件
        /// </summary>
        private void Modules_Get()
        {
            if (mod_RectTransform == null)
                mod_RectTransform = GetComponent<RectTransform>();

            if (mod_Image == null)
                mod_Image = GetComponent<Image>();

            if (mod_RawImage == null)
                mod_RawImage = GetComponent<RawImage>();

            if (mod_Text == null)
                mod_Text = GetComponent<XHud_Module_Text>();

            if (mod_TmpText == null)
                mod_TmpText = GetComponent<XHud_Module_TmpText>();

            if (mod_HudButton == null)
                mod_HudButton = GetComponentInParent<XHud_Module_Button>();
        }
        /// <summary>
        /// 获取组件基类类型，判断Image、RawImage、Text、TmpText是否不为空，取其一个不为空的作为他们的Graphic基类传出
        /// </summary>
        /// <returns></returns>
        public Graphic Modules_RecognitionType()
        {
            Graphic gc = null;

            ///---------Image组件
            if (mod_Image != null)
            {
                gc = mod_Image;
                AnimatorModuleType = ModuleType.Image;
            }
            ///---------RawImage
            if (mod_RawImage != null)
            {
                gc = mod_RawImage;
                AnimatorModuleType = ModuleType.RawImage;
            }
            ///---------Text
            if (mod_Text != null)
            {
                gc = mod_Text;
                AnimatorModuleType = ModuleType.Text;
            }
            ///-}--------TmpText
            if (mod_TmpText != null)
            {
                gc = mod_TmpText;
                AnimatorModuleType = ModuleType.TmpText;
            }

            return gc;
        }
        /// <summary>
        /// 设置组件颜色
        /// </summary>
        /// <param tweenName="color"></param>
        public void UpdateColor(Color color)
        {
            //如果Animator存在于上级按钮物体下
            if (mod_HudButton != null)
            {
                //--如果 - 此Animator类型为TmpText并作为Button下挂的组件且按钮控制  文字变色  则不同步颜色
                if (mod_HudButton.TextColorSyncFade && mod_HudButton.ButtonTmpText != null && mod_HudButton.ButtonTmpText == mod_TmpText)
                    return;
                //--如果 - 此Animator类型为Text并作为Button下挂的组件且按钮控制  文字变色  则不同步颜色
                if (mod_HudButton.TextColorSyncFade && mod_HudButton.ButtonText != null && mod_HudButton.ButtonText == mod_Text)
                    return;
                //--如果 - 此Animator类型为Image并作为Button下挂的组件且按钮控制  图标变色  则不同步颜色
                if (mod_HudButton.IconColorSyncFade && mod_HudButton.IconImage == mod_Image)
                    return;
                //--如果 - 此Animator类型为Image并作为Button下挂的组件且按钮控制  背景变色  则不同步颜色
                if (mod_HudButton.BgColorSyncFade && mod_HudButton.BgImage == mod_Image)
                    return;
            }

            //--如果 - 图像组件存在则使用color覆盖图像组件的颜色
            if (mod_Image != null)
            {
                mod_Image.color = color;
            }
            //--否则 - 如果 - 原始图像组件存在则使用color覆盖原始图像组件的颜色
            else if (mod_RawImage != null)
            {
                mod_RawImage.color = color;
            }
            //--否则 - 如果 - 文字组件存在则使用color覆盖文字组件的颜色
            else if (mod_Text != null)
            {
                if (!mod_Text.TextStyleInfo.SyncAnimatorColor)
                    return;
                if (mod_Text.StyleLibSynching)
                    if (mod_Text.TextStyleInfo.LibStyle_Effect_color)
                        return;
                mod_Text.TextStyleInfo.txt_Set_FontColor(color);
            }
            //--否则 - 如果 - 文字组件存在则使用color覆盖文字组件的颜色
            else if (mod_TmpText != null)
            {
                if (!mod_TmpText.TextStyleInfo.SyncAnimatorColor)
                    return;
                if (mod_TmpText.StyleLibSynching)
                    if (mod_TmpText.TextStyleInfo.LibStyle_Effect_color)
                        return;
                mod_TmpText.TextStyleInfo.tmp_Set_FontColor(color);
            }

            OriginalColor = color;
            OriginalAnimatorPoser.Color = color;
        }
        /// <summary>
        /// 同步颜色
        /// </summary>
        public void SynchroColor()
        {
            //--如果开启同步颜色库颜色则将组件的颜色值覆盖为库中选中的颜色，否则使用原始色
            if (SyncLibraryColor)
            {
                //--如果颜色库存在则设置组件颜色为指定名称的库中的颜色
                if (XHud_Manager.Instance.Hud_Colors != null)
                {
                    UpdateColor(XHud_Manager.Instance.Hud_Colors.ColorsLibrary_GetColor(ColoriseName));
                }
            }
            else
            {
                //--设置组件颜色为原始色
                UpdateColor(OriginalColor);
            }
        }
        /// <summary>
        /// 设置色卡
        /// </summary>
        /// <param tweenName="name"></param>
        public void SetColoriseName(string name)
        {
            ColoriseName = name;
        }
        /// <summary>
        /// 设置原始色
        /// </summary>
        /// <param tweenName="name"></param>
        public void SetOriginalColor(Color color)
        {
            OriginalColor = color;
        }
        /// <summary>
        /// 记录姿态状态
        /// </summary>
        public void OriginalAnimatorState_Record()
        {
            OriginalAnimatorPoser.Position = mod_RectTransform.anchoredPosition3D;
            OriginalAnimatorPoser.Euler = mod_RectTransform.localEulerAngles;
            OriginalAnimatorPoser.Scale = mod_RectTransform.localScale;
            OriginalAnimatorPoser.Size = mod_RectTransform.sizeDelta;
            OriginalAnimatorPoser.Alpha = CanvasGroup.alpha;
            if (mod_Image != null)
            {
                OriginalAnimatorPoser.Color = mod_Image.color;
                OriginalAnimatorPoser.Fill = mod_Image.fillAmount;
            }
            if (mod_Text != null)
                OriginalAnimatorPoser.Color = mod_Text.color;
            if (mod_TmpText != null)
                OriginalAnimatorPoser.Color = mod_TmpText.color;
        }
        /// <summary>
        /// 读取姿态状态
        /// </summary>
        public void OriginalAnimatorState_Load()
        {
            mod_RectTransform.anchoredPosition3D = OriginalAnimatorPoser.Position;
            mod_RectTransform.localEulerAngles = OriginalAnimatorPoser.Euler;
            mod_RectTransform.localScale = OriginalAnimatorPoser.Scale;
            mod_RectTransform.sizeDelta = OriginalAnimatorPoser.Size;
            CanvasGroup.alpha = OriginalAnimatorPoser.Alpha;
            if (mod_Image != null)
            {
                mod_Image.color = OriginalAnimatorPoser.Color;
                mod_Image.fillAmount = OriginalAnimatorPoser.Fill;
            }
            if (mod_Text != null)
                mod_Text.color = OriginalAnimatorPoser.Color;
            if (mod_TmpText != null)
                mod_TmpText.color = OriginalAnimatorPoser.Color;
        }
        #endregion

        #region 动画节点操作
        /// <summary>
        /// 获取最小与最大动画节点的耗时
        /// </summary>
        public void TweenNodeTimersGet()
        {
            MinTimer = TweenNode_GetTimer("Min");///---获取 - 所有动画节点中 - 耗时 - 最小的节点
            MaxTimer = TweenNode_GetTimer("Max"); ///---获取 - 所有动画节点中 - 耗时 - 最大的节点
            MinTimerWithGlobalDuration = MinTimer * Animator_GlobalDuration;///---获取 - 所有动画节点中 - 耗时 - 最小的节点 x 动画器全局速率
            MaxTimerWithGlobalDuration = MaxTimer * Animator_GlobalDuration;///---获取 - 所有动画节点中 - 耗时 - 最大的节点 x 动画器全局速率
        }
        /// <summary>
        /// 获取动画节点数量
        /// </summary>
        /// <returns></returns>
        public int TweenNode_GetCount()
        {
            return AnimateTweenNodes.Count;
        }
        /// <summary>
        /// 每个动画的进度归零
        /// </summary>
        public void TweenNodes_AnimateProgressReset()
        {
            if (AnimateTweenNodes.Count <= 0)
                return;
            for (int i = 0; i < AnimateTweenNodes.Count; i++)
            {
                #region 确保动画存在不为空的情况下，每个动画的动画进度归零
                AnimateTweenNodes[i].Progress = 0;
                #endregion
            }
        }
        /// <summary>
        /// 获取目标动画器的索引号
        /// </summary>
        /// <param tweenName="indicator"></param>
        /// <returns></returns>
        public int TweenNode_GetID(string indicator)
        {
            int index = 0;
            for (int i = 0; i < AnimateTweenNodes.Count; i++)
            {
                if (AnimateTweenNodes[i].Indicator == indicator)
                    index = AnimateTweenNodes[i].ID;
            }

            return index;
        }
        /// <summary>
        /// 获取动画节点 - 根据动画节点ID
        /// </summary>
        /// <param tweenName="index"></param>
        /// <returns></returns>
        public TweenNode TweenNode_GetByIndex(int index)
        {
            return AnimateTweenNodes[index];
        }
        /// <summary>
        /// 获取动画节点 - 根据动画节点ID
        /// </summary>
        /// <param tweenName="ID"></param>
        /// <returns></returns>
        public TweenNode TweenNode_GetByID(int ID)
        {
            TweenNode node = null;
            for (int i = 0; i < AnimateTweenNodes.Count; i++)
            {
                if (AnimateTweenNodes[i].ID == ID)
                {
                    node = AnimateTweenNodes[i];
                    break;
                }
            }
            return node;
        }
        /// <summary>
        /// 获取动画节点 - 根据动画节点名称
        /// </summary>
        /// <param tweenName="indicator"></param>
        /// <returns></returns>
        public TweenNode TweenNode_GetByIndicator(string indicator)
        {
            TweenNode node = null;
            for (int i = 0; i < AnimateTweenNodes.Count; i++)
            {
                if (AnimateTweenNodes[i].Indicator == indicator)
                {
                    node = AnimateTweenNodes[i];
                    break;
                }
            }
            return node;
        }
        /// <summary>
        /// 获取动画节点 - 根据动画节点类型
        /// </summary>
        /// <param tweenName="type"></param>
        /// <returns></returns>
        public bool TweenNode_GetByType(TweenNodeType type)
        {
            bool isExist = false;
            for (int i = 0; i < AnimateTweenNodes.Count; i++)
            {
                if (AnimateTweenNodes[i].Type == type)
                {
                    isExist = true;
                    break;
                }
            }
            return isExist;
        }
        /// <summary>
        /// 获取动画节点中最大或最小的动画耗时
        /// </summary>
        /// <returns></returns>
        public float TweenNode_GetTimer(string type)
        {
            if (AnimateTweenNodes.Count <= 0)
            {
                if (type == "Min")
                    MinTimer = 0;
                if (type == "Max")
                    MaxTimer = 0;
                return 0;
            }

            List<float> timers = new List<float>();
            for (int i = 0; i < AnimateTweenNodes.Count; i++)
            {
                timers.Add(AnimateTweenNodes[i].Duration * Mathf.Clamp(AnimateTweenNodes[i].LoopCount, 1, int.MaxValue) + AnimateTweenNodes[i].Delay);
            }

            if (type == "Min")
            {
                MinTimer = XHud_Utilitys.Array_MinValue(timers.ToArray());
                return MinTimer;
            }
            else if (type == "Max")
            {
                MaxTimer = XHud_Utilitys.Array_MaxValue(timers.ToArray());
                return MaxTimer;
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// 创建ID标识
        /// </summary>
        /// <returns>创建一个在所有TweenNodes中不重复的ID标识号</returns>
        public int TweenNode_ID_Create()
        {
            List<int> ids = new List<int>();
            for (int i = 0; i < AnimateTweenNodes.Count; i++)
            {
                ids.Add(AnimateTweenNodes[i].ID);
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
                    break;
                }
            }

            return ran_id;
        }
        /// <summary>
        /// 自动分配ID
        /// </summary>
        public void TweenNode_ID_AutoAssign()
        {
            for (int i = 0; i < AnimateTweenNodes.Count; i++)
            {
                AnimateTweenNodes[i].ID = TweenNode_ID_Create();
            }
        }
        /// <summary>
        /// 验证指定ID的动画节点是否存在
        /// </summary>
        /// <returns></returns>
        public bool TweenNode_ID_IsExist(int ID)
        {
            bool isExist = false;
            for (int i = 0; i < AnimateTweenNodes.Count; i++)
            {
                if (AnimateTweenNodes[i].ID == ID)
                {
                    isExist = true;
                }
            }
            return isExist;
        }
        #endregion

        #region 动画播放
        /// <summary>
        /// 根据索引号播放动画
        /// </summary>
        /// <param tweenName="index">动画ID号</param>
        /// <param tweenName="AnimationAction_Complete"></param>
        /// <param tweenName="AnimationAction_Percentage"></param>
        /// <param tweenName="PercentageLimite"></param>
        public void PlayAt_Index(int index, UnityAction AnimationAction_Complete = null, UnityAction AnimationAction_Percentage = null, float PercentageLimite = 0.5f)
        {
            TweenNode arg = AnimateTweenNodes[index];

            Tweener_Play(arg, XHud_Manager.Instance.DurationMultiply * Animator_GlobalDuration, AnimationAction_Complete, AnimationAction_Percentage, PercentageLimite);

            if (act_on_Animator_PlayAt != null)
                act_on_Animator_PlayAt(arg);
            eve_on_Animator_PlayAt.Invoke(arg);
        }
        /// <summary>
        /// 根据ID号播放动画
        /// </summary>
        /// <param tweenName="id">动画ID号</param>
        /// <param tweenName="AnimationAction_Complete"></param>
        /// <param tweenName="AnimationAction_Percentage"></param>
        /// <param tweenName="PercentageLimite"></param>
        public void PlayAt_ID(int id, UnityAction AnimationAction_Complete = null, UnityAction AnimationAction_Percentage = null, float PercentageLimite = 0.5f)
        {
            TweenNode arg = TweenNode_GetByID(id);

            Tweener_Play(arg, XHud_Manager.Instance.DurationMultiply * Animator_GlobalDuration, AnimationAction_Complete, AnimationAction_Percentage, PercentageLimite);

            if (act_on_Animator_PlayAt != null)
                act_on_Animator_PlayAt(arg);
            eve_on_Animator_PlayAt.Invoke(arg);
        }
        /// <summary>
        /// 根据标识名称播放动画
        /// </summary>
        /// <param tweenName="indicator"></param>
        /// <param tweenName="AnimationAction_Complete"></param>
        /// <param tweenName="AnimationAction_Percentage"></param>
        /// <param tweenName="PercentageLimite"></param>
        public void PlayAt_Indicator(string indicator, UnityAction AnimationAction_Complete = null, UnityAction AnimationAction_Percentage = null, float PercentageLimite = 0.5f)
        {
            TweenNode arg = TweenNode_GetByIndicator(indicator);

            Tweener_Play(arg, XHud_Manager.Instance.DurationMultiply * Animator_GlobalDuration, AnimationAction_Complete, AnimationAction_Percentage, PercentageLimite);

            if (act_on_Animator_PlayAt != null)
                act_on_Animator_PlayAt(arg);
            eve_on_Animator_PlayAt.Invoke(arg);
        }
        /// <summary>
        /// 动画播放
        /// </summary>
        /// <param tweenName="tim"></param>
        /// <param tweenName="delay">延迟时间</param>
        /// <param tweenName="dur">耗时</param>
        /// <param tweenName="MatchTiming">是否启用匹配播放条件</param>
        /// <param tweenName="AnimationAction_Complete">动画播放完成后动作</param>
        /// <param tweenName="AnimationAction_Percentage">动画进度状态动作</param>
        /// <param tweenName="PercentageLimite">激活动画进度状态动作的阈值（最小0，最大1）</param>
        public void Play(string tim = "", float delay = 0, float dur = 1, bool MatchTiming = true, UnityAction AnimationAction_Complete = null, UnityAction AnimationAction_Percentage = null, float PercentageLimite = 0.5f)
        {
            if (mod_RectTransform == null)
                return;
            if (CoroutineAnimation != null)
                CoroutineAnimation = null;
            CoroutineAnimation = StartCoroutine(Coroutine_Play(tim, delay, dur, MatchTiming, AnimationAction_Complete, AnimationAction_Percentage, PercentageLimite));
        }
        /// <summary>
        /// 动画器重置
        /// </summary>
        public void RewindAllTweenNode()
        {
            ///--停止所有协程
            if (CoroutineAnimation != null)
            {
                StopCoroutine(CoroutineAnimation);
                CoroutineAnimation = null;
            }

            ///--动画恢复初始
            for (int i = 0; i < AnimateTweenNodes.Count; i++)
            {
                Tweener_Rewind(AnimateTweenNodes[i]);
            }

            if (act_on_Animator_Rewind != null)
                act_on_Animator_Rewind();
            eve_on_Animator_Rewind.Invoke();
        }
        /// <summary>
        /// 延迟播放动画
        /// </summary>
        /// <param tweenName="tim">播放匹配条件</param>
        /// <param tweenName="delay">延迟时间</param>
        /// <param tweenName="dur">耗时</param>
        /// <param tweenName="MatchTiming">是否启用匹配播放条件</param>
        /// <param tweenName="AnimationAction_Complete">动画播放完成后动作</param>
        /// <param tweenName="AnimationAction_Percentage">动画进度状态动作</param>
        /// <param tweenName="PercentageLimite">激活动画进度状态动作的阈值（最小0，最大1）</param>
        private IEnumerator Coroutine_Play(string tim, float delay = 0, float dur = 1, bool MatchTiming = true, UnityAction AnimationAction_Complete = null, UnityAction AnimationAction_Percentage = null, float PercentageLimite = 0.5f)
        {
            ///--延迟
            yield return new WaitForSeconds(delay);

            ///--播放
            for (int i = 0; i < AnimateTweenNodes.Count; i++)
            {
                if (AnimateTweenNodes[i].Enabled)
                {
                    if (MatchTiming)
                    {
                        if (AnimateTweenNodes[i].Timings == tim)
                        {
                            TweenNode arg = AnimateTweenNodes[i];
                            Tweener_Play(arg, XHud_Manager.Instance.DurationMultiply * dur, AnimationAction_Complete, AnimationAction_Percentage, PercentageLimite);
                        }
                    }
                    else
                    {
                        TweenNode arg = AnimateTweenNodes[i];
                        Tweener_Play(arg, XHud_Manager.Instance.DurationMultiply * dur, AnimationAction_Complete, AnimationAction_Percentage, PercentageLimite);
                    }
                }
                else
                {
                    continue;
                }
            }

            if (act_on_Animator_Play != null)
                act_on_Animator_Play();
            eve_on_Animator_Play.Invoke();
        }
        #endregion

        #region 动画节点逻辑
        /// <summary>
        /// 动画播放
        /// </summary>
        /// <param tweenName="arg">目标动画节点</param>
        /// <param tweenName="Duration">耗时</param>
        /// <param tweenName="AnimationAction_Complete">动画播放完成后动作</param>
        /// <param tweenName="AnimationAction_Percentage">动画进度状态动作</param>
        /// <param tweenName="PercentageLimite">激活动画进度状态动作的阈值（最小0，最大1）</param>
        /// <returns></returns>
        public XTween_Interface Tweener_Play(TweenNode arg, float Duration, UnityAction AnimationAction_Complete = null, UnityAction AnimationAction_Percentage = null, float PercentageLimite = 0.5f)
        {
            XTween_Interface tween = null;
            bool sw_From = arg.ActivateFrom;
            bool sw_End = arg.ActivateEnd;
            bool sw_OnlyToEnd = arg.ActivateOnlyToEnd;


            /* “ActivateOnlyToEnd” 的概念为：从 "当前状态值到结束值"
             * 正常情况下，如果使用的动画方式为以下几种：
             * 
             * -------- "起始 -> 默认"
             * 
             * -------- "默认 -> 结束"
             * 
             * -------- "起始 -> 结束"
             * 
             * 则播放动画时是根据  ”起始值 -> 结束值 / 默认值到结束值“ 这种方式来运行的
             * 但是有一种情况要考虑到的是“独立式动画节点播放机制”
             * 例如：将根据一个按钮操作分为 ”按下“ 和 ”松开“ 的状态来实现分离式独立动画节点
             * 那么此时以上所述的3种动画机制就无法实现，因为他们都需要有一个起始值，然后再到目标结束值
             * --------------------------------------------------------
             * 所以在调用此播放动画的方法前，检查并设置您想独立根据Tim匹配条件来播放的动画节点的 “动向”参数
             * 将其设为：-------- "当前 -> 结束"
             * 这样在播放动画时不会退回动画而是先杀死动画，然后再播放动画，以达到多个动画节点独立配合播放的效果
             * --------------------------------------------------------
             * 简述：动画的动向是根据自己的需求来设置的，有些动画只需要结束值而不需要 “从 ... 到 ...” 的这个过程，
             * 选择前3种适用于有起始值且有结束值的，而第4种适用于只需要运动到目标结束值即可
             * --------------------------------------------------------
             * 注意：“ActivateOnlyToEnd”此参数在Animator的动画节点中的动向中设置，设置方法是将参数改为："当前 -> 结束"，一旦使用这种动向模式则动画不支持回退和编辑器下预览
            */
            if (!arg.ActivateOnlyToEnd)
            {
                Tweener_Rewind(arg);
            }
            else
                Tweener_Killed(arg);

            if (arg.Type == TweenNodeType.位移)
            {
                bool sw = false;
                //方式：起始 -> 默认
                if (sw_From && !sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = mod_RectTransform.xt_AnchoredPosition3D_To(arg.Original_Vector3, arg.Duration * Duration, false, true).SetFrom(arg.From_Vector3).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Vector3_Changed != null)
                                arg.Act_On_Vector3_Changed(mod_RectTransform.anchoredPosition3D);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                            if (AnimationAction_Complete != null)
                                AnimationAction_Complete();
                        });
                    else
                        tween = mod_RectTransform.xt_AnchoredPosition3D_To(arg.Original_Vector3, arg.Duration * Duration).SetFrom(arg.From_Vector3).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Vector3_Changed != null)
                                arg.Act_On_Vector3_Changed(mod_RectTransform.anchoredPosition3D);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                            if (AnimationAction_Complete != null)
                                AnimationAction_Complete();
                        });
                }
                //方式：默认 -> 结束
                else if (!sw_From && sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = mod_RectTransform.xt_AnchoredPosition3D_To(arg.End_Vector3, arg.Duration * Duration, false, true).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Vector3_Changed != null)
                                arg.Act_On_Vector3_Changed(mod_RectTransform.anchoredPosition3D);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = mod_RectTransform.xt_AnchoredPosition3D_To(arg.End_Vector3, arg.Duration * Duration, false, true).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Vector3_Changed != null)
                                arg.Act_On_Vector3_Changed(mod_RectTransform.anchoredPosition3D);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：起始 -> 结束
                else if (sw_From && sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = mod_RectTransform.xt_AnchoredPosition3D_To(arg.End_Vector3, arg.Duration * Duration, false, true).SetFrom(arg.From_Vector3).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Vector3_Changed != null)
                                arg.Act_On_Vector3_Changed(mod_RectTransform.anchoredPosition3D);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = mod_RectTransform.xt_AnchoredPosition3D_To(arg.End_Vector3, arg.Duration * Duration, false, true).SetFrom(arg.From_Vector3).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Vector3_Changed != null)
                                arg.Act_On_Vector3_Changed(mod_RectTransform.anchoredPosition3D);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：当前 -> 结束
                else if (!sw_From && !sw_End && sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = mod_RectTransform.xt_AnchoredPosition3D_To(arg.End_Vector3, arg.Duration * Duration, false, true).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Vector3_Changed != null)
                                arg.Act_On_Vector3_Changed(mod_RectTransform.anchoredPosition3D);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = mod_RectTransform.xt_AnchoredPosition3D_To(arg.End_Vector3, arg.Duration * Duration, false, true).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Vector3_Changed != null)
                                arg.Act_On_Vector3_Changed(mod_RectTransform.anchoredPosition3D);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
            }
            else if (arg.Type == TweenNodeType.旋转)
            {
                bool sw = false;
                //方式：起始 -> 默认
                if (sw_From && !sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = mod_RectTransform.xt_Rotate_To(arg.Original_Vector3, arg.Duration * Duration, false, true, XTweenRotationSpace.绝对, arg.RotateMode).SetFrom(arg.From_Vector3).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Quaternion_Changed != null)
                                arg.Act_On_Quaternion_Changed(mod_RectTransform.rotation);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = mod_RectTransform.xt_Rotate_To(arg.Original_Vector3, arg.Duration * Duration, false, true, XTweenRotationSpace.绝对, arg.RotateMode).SetFrom(arg.From_Vector3).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            arg.Progress = tween.ElapsedTime / tween.Duration;
                            if (arg.Act_On_Quaternion_Changed != null)
                                arg.Act_On_Quaternion_Changed(mod_RectTransform.rotation);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：默认 -> 结束
                else if (!sw_From && sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = mod_RectTransform.xt_Rotate_To(arg.End_Vector3, arg.Duration * Duration, false, true, XTweenRotationSpace.绝对, arg.RotateMode).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Quaternion_Changed != null)
                                arg.Act_On_Quaternion_Changed(mod_RectTransform.rotation);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = mod_RectTransform.xt_Rotate_To(arg.End_Vector3, arg.Duration * Duration, false, true, XTweenRotationSpace.绝对, arg.RotateMode).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Quaternion_Changed != null)
                                arg.Act_On_Quaternion_Changed(mod_RectTransform.rotation);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：起始 -> 结束
                else if (sw_From && sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = mod_RectTransform.xt_Rotate_To(arg.End_Vector3, arg.Duration * Duration, false, true, XTweenRotationSpace.绝对, arg.RotateMode).SetFrom(arg.From_Vector3).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Quaternion_Changed != null)
                                arg.Act_On_Quaternion_Changed(mod_RectTransform.rotation);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = mod_RectTransform.xt_Rotate_To(arg.End_Vector3, arg.Duration * Duration, false, true, XTweenRotationSpace.绝对, arg.RotateMode).SetFrom(arg.From_Vector3).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Quaternion_Changed != null)
                                arg.Act_On_Quaternion_Changed(mod_RectTransform.rotation);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：当前 -> 结束
                else if (!sw_From && !sw_End && sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = mod_RectTransform.xt_Rotate_To(arg.End_Vector3, arg.Duration * Duration, false, true, XTweenRotationSpace.绝对, arg.RotateMode).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Quaternion_Changed != null)
                                arg.Act_On_Quaternion_Changed(mod_RectTransform.rotation);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = mod_RectTransform.xt_Rotate_To(arg.End_Vector3, arg.Duration * Duration, false, true, XTweenRotationSpace.绝对, arg.RotateMode).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Quaternion_Changed != null)
                                arg.Act_On_Quaternion_Changed(mod_RectTransform.rotation);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
            }
            else if (arg.Type == TweenNodeType.缩放)
            {
                bool sw = false;
                //方式：起始 -> 默认
                if (sw_From && !sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = mod_RectTransform.xt_Scale_To(arg.Original_Vector3, arg.Duration * Duration, false, true).SetFrom(arg.From_Vector3).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Vector3_Changed != null)
                                arg.Act_On_Vector3_Changed(mod_RectTransform.localScale);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = mod_RectTransform.xt_Scale_To(arg.Original_Vector3, arg.Duration * Duration, false, true).SetFrom(arg.From_Vector3).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Vector3_Changed != null)
                                arg.Act_On_Vector3_Changed(mod_RectTransform.localScale);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：默认 -> 结束
                else if (!sw_From && sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = mod_RectTransform.xt_Scale_To(arg.End_Vector3, arg.Duration * Duration, false, true).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Vector3_Changed != null)
                                arg.Act_On_Vector3_Changed(mod_RectTransform.localScale);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = mod_RectTransform.xt_Scale_To(arg.End_Vector3, arg.Duration * Duration, false, true).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Vector3_Changed != null)
                                arg.Act_On_Vector3_Changed(mod_RectTransform.localScale);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：起始 -> 结束
                else if (sw_From && sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = mod_RectTransform.xt_Scale_To(arg.End_Vector3, arg.Duration * Duration, false, true).SetFrom(arg.From_Vector3).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Vector3_Changed != null)
                                arg.Act_On_Vector3_Changed(mod_RectTransform.localScale);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = mod_RectTransform.xt_Scale_To(arg.End_Vector3, arg.Duration * Duration, false, true).SetFrom(arg.From_Vector3).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Vector3_Changed != null)
                                arg.Act_On_Vector3_Changed(mod_RectTransform.localScale);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：当前 -> 结束
                else if (!sw_From && !sw_End && sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = mod_RectTransform.xt_Scale_To(arg.End_Vector3, arg.Duration * Duration, false, true).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Vector3_Changed != null)
                                arg.Act_On_Vector3_Changed(mod_RectTransform.localScale);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = mod_RectTransform.xt_Scale_To(arg.End_Vector3, arg.Duration * Duration, false, true).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Vector3_Changed != null)
                                arg.Act_On_Vector3_Changed(mod_RectTransform.localScale);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
            }
            else if (arg.Type == TweenNodeType.颜色)
            {
                bool sw = false;

                #region 判断文字组件是否为库同步样式状态
                bool IsTextColorMode = false;
                if ((mod_Text && mod_Text.StyleLibSynching && mod_Text.TextStyleInfo.LibStyle_Effect_color) ||
                (mod_TmpText && mod_TmpText.StyleLibSynching && mod_TmpText.TextStyleInfo.LibStyle_Effect_color))
                {
                    IsTextColorMode = true;
                }
                if (IsTextColorMode)
                    return null;
                #endregion

                Graphic gc = Modules_RecognitionType();
                if (gc == null)
                    return null;
                //方式：起始 -> 默认
                if (sw_From && !sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = gc.xt_Color_To(arg.Original_Color, arg.Duration * Duration, true).SetFrom(arg.From_Color).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Color>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Color_Changed != null)
                                arg.Act_On_Color_Changed(gc.color);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = gc.xt_Color_To(arg.Original_Color, arg.Duration * Duration, true).SetFrom(arg.From_Color).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Color>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Color_Changed != null)
                                arg.Act_On_Color_Changed(gc.color);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：默认 -> 结束
                if (!sw_From && sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = gc.xt_Color_To(arg.End_Color, arg.Duration * Duration, true).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Color>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Color_Changed != null)
                                arg.Act_On_Color_Changed(gc.color);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = gc.xt_Color_To(arg.End_Color, arg.Duration * Duration, true).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Color>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Color_Changed != null)
                                arg.Act_On_Color_Changed(gc.color);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：起始 -> 结束
                if (sw_From && sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = gc.xt_Color_To(arg.End_Color, arg.Duration * Duration, true).SetFrom(arg.From_Color).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Color>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Color_Changed != null)
                                arg.Act_On_Color_Changed(gc.color);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = gc.xt_Color_To(arg.End_Color, arg.Duration * Duration, true).SetFrom(arg.From_Color).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Color>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Color_Changed != null)
                                arg.Act_On_Color_Changed(gc.color);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：当前 -> 结束
                else if (!sw_From && !sw_End && sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = gc.xt_Color_To(arg.End_Color, arg.Duration * Duration, true).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Color>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Color_Changed != null)
                                arg.Act_On_Color_Changed(gc.color);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = gc.xt_Color_To(arg.End_Color, arg.Duration * Duration, true).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Color>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Color_Changed != null)
                                arg.Act_On_Color_Changed(gc.color);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
            }
            else if (arg.Type == TweenNodeType.淡化)
            {
                bool sw = false;
                if (CanvasGroup == null)
                    return null;
                //方式：起始 -> 默认
                if (sw_From && !sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = CanvasGroup.xt_Alpha_To(arg.Original_Float, arg.Duration * Duration, true).SetFrom(arg.From_Float).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<float>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Float_Changed != null)
                                arg.Act_On_Float_Changed(CanvasGroup.alpha);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = CanvasGroup.xt_Alpha_To(arg.Original_Float, arg.Duration * Duration, true).SetFrom(arg.From_Float).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<float>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Float_Changed != null)
                                arg.Act_On_Float_Changed(CanvasGroup.alpha);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：默认 -> 结束
                else if (!sw_From && sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = CanvasGroup.xt_Alpha_To(arg.End_Float, arg.Duration * Duration, true).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<float>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Float_Changed != null)
                                arg.Act_On_Float_Changed(CanvasGroup.alpha);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = CanvasGroup.xt_Alpha_To(arg.End_Float, arg.Duration * Duration, true).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<float>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Float_Changed != null)
                                arg.Act_On_Float_Changed(CanvasGroup.alpha);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：起始 -> 结束
                else if (sw_From && sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = CanvasGroup.xt_Alpha_To(arg.End_Float, arg.Duration * Duration, true).SetFrom(arg.From_Float).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<float>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Float_Changed != null)
                                arg.Act_On_Float_Changed(CanvasGroup.alpha);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = CanvasGroup.xt_Alpha_To(arg.End_Float, arg.Duration * Duration, true).SetFrom(arg.From_Float).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<float>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Float_Changed != null)
                                arg.Act_On_Float_Changed(CanvasGroup.alpha);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：当前 -> 结束
                else if (!sw_From && !sw_End && sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = CanvasGroup.xt_Alpha_To(arg.End_Float, arg.Duration * Duration, true).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<float>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Float_Changed != null)
                                arg.Act_On_Float_Changed(CanvasGroup.alpha);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = CanvasGroup.xt_Alpha_To(arg.End_Float, arg.Duration * Duration, true).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<float>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Float_Changed != null)
                                arg.Act_On_Float_Changed(CanvasGroup.alpha);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
            }
            else if (arg.Type == TweenNodeType.打字机)
            {
                bool sw = false;
                if (mod_Text == null && mod_TmpText == null)
                {
                    return null;
                }

                //方式：起始 -> 默认
                if (sw_From && !sw_End && !sw_OnlyToEnd)
                {
                    if (mod_Text != null)
                    {
                        if (arg.Ease != EaseMode.Unset)
                            tween = mod_Text.xt_FontText_To(false, " |", arg.Original_String, arg.Duration * Duration, true).SetFrom(arg.From_String).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<string>((v, d, t) =>
                            {
                                if (tween.CurrentEasedProgress >= PercentageLimite)
                                {
                                    if (!sw)
                                    {
                                        sw = true;
                                        if (AnimationAction_Percentage != null)
                                            AnimationAction_Percentage();
                                    }
                                }
                                if (arg.Act_On_Text_Changed != null)
                                    arg.Act_On_Text_Changed(mod_Text.text);
                            }).OnComplete((d) =>
                            {
                                arg.Progress = 0;
                            });
                        else
                            tween = mod_Text.xt_FontText_To(false, " |", arg.Original_String, arg.Duration * Duration, true).SetFrom(arg.From_String).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<string>((v, d, t) =>
                            {
                                if (tween.CurrentEasedProgress >= PercentageLimite)
                                {
                                    if (!sw)
                                    {
                                        sw = true;
                                        if (AnimationAction_Percentage != null)
                                            AnimationAction_Percentage();
                                    }
                                }
                                if (arg.Act_On_Text_Changed != null)
                                    arg.Act_On_Text_Changed(mod_Text.text);
                            }).OnComplete((d) =>
                            {
                                arg.Progress = 0;
                            });
                    }
                    else if (mod_TmpText != null)
                    {
                        if (arg.Ease != EaseMode.Unset)
                            tween = mod_TmpText.xt_FontText_To(false, arg.Original_String, arg.Duration * Duration, true).SetFrom(arg.From_String).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<string>((v, d, t) =>
                            {
                                if (tween.CurrentEasedProgress >= PercentageLimite)
                                {
                                    if (!sw)
                                    {
                                        sw = true;
                                        if (AnimationAction_Percentage != null)
                                            AnimationAction_Percentage();
                                    }
                                }
                                if (arg.Act_On_Text_Changed != null)
                                    arg.Act_On_Text_Changed(mod_TmpText.text);
                            }).OnComplete((d) =>
                            {
                                arg.Progress = 0;
                            });
                        else
                            tween = mod_TmpText.xt_FontText_To(false, arg.Original_String, arg.Duration * Duration, true).SetFrom(arg.From_String).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<string>((v, d, t) =>
                            {
                                if (tween.CurrentEasedProgress >= PercentageLimite)
                                {
                                    if (!sw)
                                    {
                                        sw = true;
                                        if (AnimationAction_Percentage != null)
                                            AnimationAction_Percentage();
                                    }
                                }
                                if (arg.Act_On_Text_Changed != null)
                                    arg.Act_On_Text_Changed(mod_TmpText.text);
                            }).OnComplete((d) =>
                            {
                                arg.Progress = 0;
                            });
                    }
                }
                //方式：默认 -> 结束
                else if (!sw_From && sw_End && !sw_OnlyToEnd)
                {
                    if (mod_Text != null)
                    {
                        if (arg.Ease != EaseMode.Unset)
                            tween = mod_Text.xt_FontText_To(false, " |", arg.End_String, arg.Duration * Duration).SetFrom(arg.Original_String).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<string>((v, d, t) =>
                            {
                                if (tween.CurrentEasedProgress >= PercentageLimite)
                                {
                                    if (!sw)
                                    {
                                        sw = true;
                                        if (AnimationAction_Percentage != null)
                                            AnimationAction_Percentage();
                                    }
                                }
                                if (arg.Act_On_Text_Changed != null)
                                    arg.Act_On_Text_Changed(mod_Text.text);
                            }).OnComplete((d) =>
                            {
                                arg.Progress = 0;
                            });
                        else
                            tween = mod_Text.xt_FontText_To(false, " |", arg.End_String, arg.Duration * Duration).SetFrom(arg.Original_String).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<string>((v, d, t) =>
                            {
                                if (tween.CurrentEasedProgress >= PercentageLimite)
                                {
                                    if (!sw)
                                    {
                                        sw = true;
                                        if (AnimationAction_Percentage != null)
                                            AnimationAction_Percentage();
                                    }
                                }
                                if (arg.Act_On_Text_Changed != null)
                                    arg.Act_On_Text_Changed(mod_Text.text);
                            }).OnComplete((d) =>
                            {
                                arg.Progress = 0;
                            });
                    }
                    else if (mod_TmpText != null)
                    {
                        if (arg.Ease != EaseMode.Unset)
                            tween = mod_TmpText.xt_FontText_To(false, arg.End_String, arg.Duration * Duration, true).SetFrom(arg.Original_String).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<string>((v, d, t) =>
                            {
                                if (tween.CurrentEasedProgress >= PercentageLimite)
                                {
                                    if (!sw)
                                    {
                                        sw = true;
                                        if (AnimationAction_Percentage != null)
                                            AnimationAction_Percentage();
                                    }
                                }
                                if (arg.Act_On_Text_Changed != null)
                                    arg.Act_On_Text_Changed(mod_TmpText.text);
                            }).OnComplete((d) =>
                            {
                                arg.Progress = 0;
                            });
                        else
                            tween = mod_TmpText.xt_FontText_To(false, arg.End_String, arg.Duration * Duration, true).SetFrom(arg.Original_String).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<string>((v, d, t) =>
                            {
                                if (tween.CurrentEasedProgress >= PercentageLimite)
                                {
                                    if (!sw)
                                    {
                                        sw = true;
                                        if (AnimationAction_Percentage != null)
                                            AnimationAction_Percentage();
                                    }
                                }
                                if (arg.Act_On_Text_Changed != null)
                                    arg.Act_On_Text_Changed(mod_TmpText.text);
                            }).OnComplete((d) =>
                            {
                                arg.Progress = 0;
                            });
                    }
                }
                //方式：起始 -> 结束
                else if (sw_From && sw_End && !sw_OnlyToEnd)
                {
                    if (mod_Text != null)
                    {
                        if (arg.Ease != EaseMode.Unset)
                            tween = mod_Text.xt_FontText_To(false, " |", arg.End_String, arg.Duration * Duration, true).SetFrom(arg.From_String).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<string>((v, d, t) =>
                            {
                                if (tween.CurrentEasedProgress >= PercentageLimite)
                                {
                                    if (!sw)
                                    {
                                        sw = true;
                                        if (AnimationAction_Percentage != null)
                                            AnimationAction_Percentage();
                                    }
                                }
                                if (arg.Act_On_Text_Changed != null)
                                    arg.Act_On_Text_Changed(mod_Text.text);
                            }).OnComplete((d) =>
                            {
                                arg.Progress = 0;
                            });
                        else
                            tween = mod_Text.xt_FontText_To(false, " |", arg.End_String, arg.Duration * Duration, true).SetFrom(arg.From_String).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<string>((v, d, t) =>
                            {
                                if (tween.CurrentEasedProgress >= PercentageLimite)
                                {
                                    if (!sw)
                                    {
                                        sw = true;
                                        if (AnimationAction_Percentage != null)
                                            AnimationAction_Percentage();
                                    }
                                }
                                if (arg.Act_On_Text_Changed != null)
                                    arg.Act_On_Text_Changed(mod_Text.text);
                            }).OnComplete((d) =>
                            {
                                arg.Progress = 0;
                            });
                    }
                    else if (mod_TmpText != null)
                    {
                        if (arg.Ease != EaseMode.Unset)
                            tween = mod_TmpText.xt_FontText_To(false, arg.End_String, arg.Duration * Duration, true).SetFrom(arg.From_String).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<string>((v, d, t) =>
                            {
                                if (tween.CurrentEasedProgress >= PercentageLimite)
                                {
                                    if (!sw)
                                    {
                                        sw = true;
                                        if (AnimationAction_Percentage != null)
                                            AnimationAction_Percentage();
                                    }
                                }
                                if (arg.Act_On_Text_Changed != null)
                                    arg.Act_On_Text_Changed(mod_TmpText.text);
                            }).OnComplete((d) =>
                            {
                                arg.Progress = 0;
                            });
                        else
                            tween = mod_TmpText.xt_FontText_To(false, arg.End_String, arg.Duration * Duration, true).SetFrom(arg.From_String).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<string>((v, d, t) =>
                            {
                                if (tween.CurrentEasedProgress >= PercentageLimite)
                                {
                                    if (!sw)
                                    {
                                        sw = true;
                                        if (AnimationAction_Percentage != null)
                                            AnimationAction_Percentage();
                                    }
                                }
                                if (arg.Act_On_Text_Changed != null)
                                    arg.Act_On_Text_Changed(mod_TmpText.text);
                            }).OnComplete((d) =>
                            {
                                arg.Progress = 0;
                            });
                    }
                }
                //方式：当前 -> 结束
                else if (!sw_From && !sw_End && sw_OnlyToEnd)
                {
                    if (mod_Text != null)
                    {
                        if (arg.Ease != EaseMode.Unset)
                            tween = mod_Text.xt_FontText_To(false, " |", arg.End_String, arg.Duration * Duration, true).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<string>((v, d, t) =>
                            {
                                if (tween.CurrentEasedProgress >= PercentageLimite)
                                {
                                    if (!sw)
                                    {
                                        sw = true;
                                        if (AnimationAction_Percentage != null)
                                            AnimationAction_Percentage();
                                    }
                                }
                                if (arg.Act_On_Text_Changed != null)
                                    arg.Act_On_Text_Changed(mod_Text.text);
                            }).OnComplete((d) =>
                            {
                                arg.Progress = 0;
                            });
                        else
                            tween = mod_Text.xt_FontText_To(false, " |", arg.End_String, arg.Duration * Duration, true).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<string>((v, d, t) =>
                            {
                                if (tween.CurrentEasedProgress >= PercentageLimite)
                                {
                                    if (!sw)
                                    {
                                        sw = true;
                                        if (AnimationAction_Percentage != null)
                                            AnimationAction_Percentage();
                                    }
                                }
                                if (arg.Act_On_Text_Changed != null)
                                    arg.Act_On_Text_Changed(mod_Text.text);
                            }).OnComplete((d) =>
                            {
                                arg.Progress = 0;
                            });
                    }
                    else if (mod_TmpText != null)
                    {
                        if (arg.Ease != EaseMode.Unset)
                            tween = mod_TmpText.xt_FontText_To(false, arg.End_String, arg.Duration * Duration, true).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<string>((v, d, t) =>
                            {
                                if (tween.CurrentEasedProgress >= PercentageLimite)
                                {
                                    if (!sw)
                                    {
                                        sw = true;
                                        if (AnimationAction_Percentage != null)
                                            AnimationAction_Percentage();
                                    }
                                }
                                if (arg.Act_On_Text_Changed != null)
                                    arg.Act_On_Text_Changed(mod_TmpText.text);
                            }).OnComplete((d) =>
                            {
                                arg.Progress = 0;
                            });
                        else
                            tween = mod_TmpText.xt_FontText_To(false, arg.End_String, arg.Duration * Duration, true).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<string>((v, d, t) =>
                            {
                                if (tween.CurrentEasedProgress >= PercentageLimite)
                                {
                                    if (!sw)
                                    {
                                        sw = true;
                                        if (AnimationAction_Percentage != null)
                                            AnimationAction_Percentage();
                                    }
                                }
                                if (arg.Act_On_Text_Changed != null)
                                    arg.Act_On_Text_Changed(mod_TmpText.text);
                            }).OnComplete((d) =>
                            {
                                arg.Progress = 0;
                            });
                    }
                }
            }
            else if (arg.Type == TweenNodeType.尺寸)
            {
                bool sw = false;
                //方式：起始 -> 默认
                if (sw_From && !sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = mod_RectTransform.xt_Size_To(arg.Original_Vector2, arg.Duration * Duration, false, true).SetFrom(arg.From_Vector2).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector2>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Vector2_Changed != null)
                                arg.Act_On_Vector2_Changed(mod_RectTransform.sizeDelta);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = mod_RectTransform.xt_Size_To(arg.Original_Vector2, arg.Duration * Duration, false, true).SetFrom(arg.From_Vector2).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector2>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Vector2_Changed != null)
                                arg.Act_On_Vector2_Changed(mod_RectTransform.sizeDelta);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：默认 -> 结束
                else if (!sw_From && sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = mod_RectTransform.xt_Size_To(arg.End_Vector2, arg.Duration * Duration, false, true).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector2>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Vector2_Changed != null)
                                arg.Act_On_Vector2_Changed(mod_RectTransform.sizeDelta);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = mod_RectTransform.xt_Size_To(arg.End_Vector2, arg.Duration * Duration, false, true).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector2>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Vector2_Changed != null)
                                arg.Act_On_Vector2_Changed(mod_RectTransform.sizeDelta);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：起始 -> 结束
                else if (sw_From && sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = mod_RectTransform.xt_Size_To(arg.End_Vector2, arg.Duration * Duration, false, true).SetFrom(arg.From_Vector2).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector2>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Vector2_Changed != null)
                                arg.Act_On_Vector2_Changed(mod_RectTransform.sizeDelta);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                    {
                        tween = mod_RectTransform.xt_Size_To(arg.End_Vector2, arg.Duration * Duration, false, true).SetFrom(arg.From_Vector2).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector2>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Vector2_Changed != null)
                                arg.Act_On_Vector2_Changed(mod_RectTransform.sizeDelta);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    }
                }
                //方式：当前 -> 结束
                else if (!sw_From && !sw_End && sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = mod_RectTransform.xt_Size_To(arg.End_Vector2, arg.Duration * Duration, false, true).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector2>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Vector2_Changed != null)
                                arg.Act_On_Vector2_Changed(mod_RectTransform.sizeDelta);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = mod_RectTransform.xt_Size_To(arg.End_Vector2, arg.Duration * Duration, false, true).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector2>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Vector2_Changed != null)
                                arg.Act_On_Vector2_Changed(mod_RectTransform.sizeDelta);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
            }
            else if (arg.Type == TweenNodeType.图像填充)
            {
                bool sw = false;
                if (mod_Image == null)
                {
                    return null;
                }
                //方式：起始 -> 默认
                if (sw_From && !sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = XTween.To(() => mod_Image.fillAmount, x => mod_Image.fillAmount = x, Mathf.Clamp01(arg.Original_Float), arg.Duration * Duration, true).SetFrom(arg.From_Float).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<float>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Float_Changed != null)
                                arg.Act_On_Float_Changed(mod_Image.fillAmount);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = XTween.To(() => mod_Image.fillAmount, x => mod_Image.fillAmount = x, Mathf.Clamp01(arg.Original_Float), arg.Duration * Duration, true).SetFrom(arg.From_Float).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<float>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Float_Changed != null)
                                arg.Act_On_Float_Changed(mod_Image.fillAmount);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：默认 -> 结束
                else if (!sw_From && sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = XTween.To(() => mod_Image.fillAmount, x => mod_Image.fillAmount = x, Mathf.Clamp01(arg.End_Float), arg.Duration * Duration, true).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<float>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Float_Changed != null)
                                arg.Act_On_Float_Changed(mod_Image.fillAmount);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = XTween.To(() => mod_Image.fillAmount, x => mod_Image.fillAmount = x, Mathf.Clamp01(arg.End_Float), arg.Duration * Duration, true).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<float>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Float_Changed != null)
                                arg.Act_On_Float_Changed(mod_Image.fillAmount);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：起始 -> 结束
                else if (sw_From && sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = XTween.To(() => mod_Image.fillAmount, x => mod_Image.fillAmount = x, Mathf.Clamp01(arg.End_Float), arg.Duration * Duration, true).SetFrom(Mathf.Clamp01(arg.From_Float)).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<float>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Float_Changed != null)
                                arg.Act_On_Float_Changed(mod_Image.fillAmount);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = XTween.To(() => mod_Image.fillAmount, x => mod_Image.fillAmount = x, Mathf.Clamp01(arg.End_Float), arg.Duration * Duration, true).SetFrom(Mathf.Clamp01(arg.From_Float)).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<float>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Float_Changed != null)
                                arg.Act_On_Float_Changed(mod_Image.fillAmount);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：当前 -> 结束
                else if (!sw_From && !sw_End && sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = XTween.To(() => mod_Image.fillAmount, x => mod_Image.fillAmount = x, Mathf.Clamp01(arg.End_Float), arg.Duration * Duration, true).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<float>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Float_Changed != null)
                                arg.Act_On_Float_Changed(mod_Image.fillAmount);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = XTween.To(() => mod_Image.fillAmount, x => mod_Image.fillAmount = x, Mathf.Clamp01(arg.End_Float), arg.Duration * Duration, true).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<float>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Float_Changed != null)
                                arg.Act_On_Float_Changed(mod_Image.fillAmount);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
            }
            else if (arg.Type == TweenNodeType.自定义整数)
            {
                bool sw = false;
                if (sw_From && !sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = XTween.To(() => CustomValue_Int, x => CustomValue_Int = x, arg.Original_Int, arg.Duration * Duration, true).SetFrom(arg.From_Int).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<int>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Int_Changed != null)
                                arg.Act_On_Int_Changed(CustomValue_Int);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = XTween.To(() => CustomValue_Int, x => CustomValue_Int = x, arg.Original_Int, arg.Duration * Duration, true).SetFrom(arg.From_Int).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<int>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Int_Changed != null)
                                arg.Act_On_Int_Changed(CustomValue_Int);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                else if (!sw_From && sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = XTween.To(() => CustomValue_Int, x => CustomValue_Int = x, arg.End_Int, arg.Duration * Duration, true).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<int>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Int_Changed != null)
                                arg.Act_On_Int_Changed(CustomValue_Int);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = XTween.To(() => CustomValue_Int, x => CustomValue_Int = x, arg.End_Int, arg.Duration * Duration, true).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<int>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Int_Changed != null)
                                arg.Act_On_Int_Changed(CustomValue_Int);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                else if (sw_From && sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = XTween.To(() => CustomValue_Int, x => CustomValue_Int = x, arg.End_Int, arg.Duration * Duration, true).SetFrom(arg.From_Int).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<int>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Int_Changed != null)
                                arg.Act_On_Int_Changed(CustomValue_Int);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = XTween.To(() => CustomValue_Int, x => CustomValue_Int = x, arg.End_Int, arg.Duration * Duration, true).SetFrom(arg.From_Int).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<int>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Int_Changed != null)
                                arg.Act_On_Int_Changed(CustomValue_Int);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                else if (!sw_From && !sw_End && sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = XTween.To(() => CustomValue_Int, x => CustomValue_Int = x, arg.End_Int, arg.Duration * Duration, true).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<int>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Int_Changed != null)
                                arg.Act_On_Int_Changed(CustomValue_Int);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = XTween.To(() => CustomValue_Int, x => CustomValue_Int = x, arg.End_Int, arg.Duration * Duration, true).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<int>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Int_Changed != null)
                                arg.Act_On_Int_Changed(CustomValue_Int);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
            }
            else if (arg.Type == TweenNodeType.自定义浮点数)
            {
                bool sw = false;
                //方式：起始 -> 默认
                if (sw_From && !sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = XTween.To(() => CustomValue_Float, x => CustomValue_Float = x, arg.Original_Float, arg.Duration * Duration, true).SetFrom(arg.From_Float).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<float>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Float_Changed != null)
                                arg.Act_On_Float_Changed(CustomValue_Float);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = XTween.To(() => CustomValue_Float, x => CustomValue_Float = x, arg.Original_Float, arg.Duration * Duration, true).SetFrom(arg.From_Float).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<float>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Float_Changed != null)
                                arg.Act_On_Float_Changed(CustomValue_Float);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：默认 -> 结束
                else if (!sw_From && sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = XTween.To(() => CustomValue_Float, x => CustomValue_Float = x, arg.End_Float, arg.Duration * Duration, true).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<float>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Float_Changed != null)
                                arg.Act_On_Float_Changed(CustomValue_Float);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = XTween.To(() => CustomValue_Float, x => CustomValue_Float = x, arg.End_Float, arg.Duration * Duration, true).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<float>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Float_Changed != null)
                                arg.Act_On_Float_Changed(CustomValue_Float);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：起始 -> 结束
                else if (sw_From && sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = XTween.To(() => CustomValue_Float, x => CustomValue_Float = x, arg.End_Float, arg.Duration * Duration, true).SetFrom(arg.From_Float).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<float>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Float_Changed != null)
                                arg.Act_On_Float_Changed(CustomValue_Float);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = XTween.To(() => CustomValue_Float, x => CustomValue_Float = x, arg.End_Float, arg.Duration * Duration, true).SetFrom(arg.From_Float).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<float>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Float_Changed != null)
                                arg.Act_On_Float_Changed(CustomValue_Float);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：当前 -> 结束
                else if (!sw_From && !sw_End && sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = XTween.To(() => CustomValue_Float, x => CustomValue_Float = x, arg.End_Float, arg.Duration * Duration, true).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<float>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Float_Changed != null)
                                arg.Act_On_Float_Changed(CustomValue_Float);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = XTween.To(() => CustomValue_Float, x => CustomValue_Float = x, arg.End_Float, arg.Duration * Duration, true).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<float>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Float_Changed != null)
                                arg.Act_On_Float_Changed(CustomValue_Float);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
            }
            else if (arg.Type == TweenNodeType.自定义2维向量)
            {
                bool sw = false;
                //方式：起始 -> 默认
                if (sw_From && !sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = XTween.To(() => CustomValue_Vector2, x => CustomValue_Vector2 = x, arg.Original_Vector2, arg.Duration * Duration, true).SetFrom(arg.From_Vector2).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector2>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Vector2_Changed != null)
                                arg.Act_On_Vector2_Changed(CustomValue_Vector2);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = XTween.To(() => CustomValue_Vector2, x => CustomValue_Vector2 = x, arg.Original_Vector2, arg.Duration * Duration, true).SetFrom(arg.From_Vector2).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector2>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Vector2_Changed != null)
                                arg.Act_On_Vector2_Changed(CustomValue_Vector2);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：默认 -> 结束
                else if (!sw_From && sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = XTween.To(() => CustomValue_Vector2, x => CustomValue_Vector2 = x, arg.End_Vector2, arg.Duration * Duration, true).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector2>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Vector2_Changed != null)
                                arg.Act_On_Vector2_Changed(CustomValue_Vector2);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = XTween.To(() => CustomValue_Vector2, x => CustomValue_Vector2 = x, arg.End_Vector2, arg.Duration * Duration, true).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector2>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Vector2_Changed != null)
                                arg.Act_On_Vector2_Changed(CustomValue_Vector2);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：起始 -> 结束
                else if (sw_From && sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = XTween.To(() => CustomValue_Vector2, x => CustomValue_Vector2 = x, arg.End_Vector2, arg.Duration * Duration, true).SetFrom(arg.From_Vector2).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector2>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Vector2_Changed != null)
                                arg.Act_On_Vector2_Changed(CustomValue_Vector2);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = XTween.To(() => CustomValue_Vector2, x => CustomValue_Vector2 = x, arg.End_Vector2, arg.Duration * Duration, true).SetFrom(arg.From_Vector2).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector2>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Vector2_Changed != null)
                                arg.Act_On_Vector2_Changed(CustomValue_Vector2);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：当前 -> 结束
                else if (!sw_From && !sw_End && sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = XTween.To(() => CustomValue_Vector2, x => CustomValue_Vector2 = x, arg.End_Vector2, arg.Duration * Duration, true).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector2>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Vector2_Changed != null)
                                arg.Act_On_Vector2_Changed(CustomValue_Vector2);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = XTween.To(() => CustomValue_Vector2, x => CustomValue_Vector2 = x, arg.End_Vector2, arg.Duration * Duration, true).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector2>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Vector2_Changed != null)
                                arg.Act_On_Vector2_Changed(CustomValue_Vector2);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
            }
            else if (arg.Type == TweenNodeType.自定义3维向量)
            {
                bool sw = false;
                //方式：起始 -> 默认
                if (sw_From && !sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = XTween.To(() => CustomValue_Vector3, x => CustomValue_Vector3 = x, arg.Original_Vector3, arg.Duration * Duration, true).SetFrom(arg.From_Vector3).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Vector3_Changed != null)
                                arg.Act_On_Vector3_Changed(CustomValue_Vector3);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = XTween.To(() => CustomValue_Vector3, x => CustomValue_Vector3 = x, arg.Original_Vector3, arg.Duration * Duration, true).SetFrom(arg.From_Vector3).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Vector3_Changed != null)
                                arg.Act_On_Vector3_Changed(CustomValue_Vector3);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：默认 -> 结束
                else if (!sw_From && sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = XTween.To(() => CustomValue_Vector3, x => CustomValue_Vector3 = x, arg.End_Vector3, arg.Duration * Duration, true).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Vector3_Changed != null)
                                arg.Act_On_Vector3_Changed(CustomValue_Vector3);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = XTween.To(() => CustomValue_Vector3, x => CustomValue_Vector3 = x, arg.End_Vector3, arg.Duration * Duration, true).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Vector3_Changed != null)
                                arg.Act_On_Vector3_Changed(CustomValue_Vector3);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：起始 -> 结束
                else if (sw_From && sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = XTween.To(() => CustomValue_Vector3, x => CustomValue_Vector3 = x, arg.End_Vector3, arg.Duration * Duration, true).SetFrom(arg.From_Vector3).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Vector3_Changed != null)
                                arg.Act_On_Vector3_Changed(CustomValue_Vector3);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = XTween.To(() => CustomValue_Vector3, x => CustomValue_Vector3 = x, arg.End_Vector3, arg.Duration * Duration, true).SetFrom(arg.From_Vector3).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Vector3_Changed != null)
                                arg.Act_On_Vector3_Changed(CustomValue_Vector3);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：当前 -> 结束
                else if (!sw_From && !sw_End && sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = XTween.To(() => CustomValue_Vector3, x => CustomValue_Vector3 = x, arg.End_Vector3, arg.Duration * Duration, true).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Vector3_Changed != null)
                                arg.Act_On_Vector3_Changed(CustomValue_Vector3);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = XTween.To(() => CustomValue_Vector3, x => CustomValue_Vector3 = x, arg.End_Vector3, arg.Duration * Duration, true).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Vector3_Changed != null)
                                arg.Act_On_Vector3_Changed(CustomValue_Vector3);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
            }
            else if (arg.Type == TweenNodeType.自定义4维向量)
            {
                bool sw = false;
                //方式：起始 -> 默认
                if (sw_From && !sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = XTween.To(() => CustomValue_Vector4, x => CustomValue_Vector4 = x, arg.Original_Vector4, arg.Duration * Duration, true).SetFrom(arg.From_Vector4).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector4>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Vector4_Changed != null)
                                arg.Act_On_Vector4_Changed(CustomValue_Vector4);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = XTween.To(() => CustomValue_Vector4, x => CustomValue_Vector4 = x, arg.Original_Vector4, arg.Duration * Duration, true).SetFrom(arg.From_Vector4).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector4>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Vector4_Changed != null)
                                arg.Act_On_Vector4_Changed(CustomValue_Vector4);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：默认 -> 结束
                else if (!sw_From && sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = XTween.To(() => CustomValue_Vector4, x => CustomValue_Vector4 = x, arg.End_Vector4, arg.Duration * Duration, true).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector4>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Vector4_Changed != null)
                                arg.Act_On_Vector4_Changed(CustomValue_Vector4);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = XTween.To(() => CustomValue_Vector4, x => CustomValue_Vector4 = x, arg.End_Vector4, arg.Duration * Duration, true).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector4>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Vector4_Changed != null)
                                arg.Act_On_Vector4_Changed(CustomValue_Vector4);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：起始 -> 结束
                else if (sw_From && sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = XTween.To(() => CustomValue_Vector4, x => CustomValue_Vector4 = x, arg.End_Vector4, arg.Duration * Duration, true).SetFrom(arg.From_Vector4).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector4>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Vector4_Changed != null)
                                arg.Act_On_Vector4_Changed(CustomValue_Vector4);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = XTween.To(() => CustomValue_Vector4, x => CustomValue_Vector4 = x, arg.End_Vector4, arg.Duration * Duration, true).SetFrom(arg.From_Vector4).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector4>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Vector4_Changed != null)
                                arg.Act_On_Vector4_Changed(CustomValue_Vector4);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：当前 -> 结束
                else if (!sw_From && !sw_End && sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = XTween.To(() => CustomValue_Vector4, x => CustomValue_Vector4 = x, arg.End_Vector4, arg.Duration * Duration, true).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector4>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Vector4_Changed != null)
                                arg.Act_On_Vector4_Changed(CustomValue_Vector4);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = XTween.To(() => CustomValue_Vector4, x => CustomValue_Vector4 = x, arg.End_Vector4, arg.Duration * Duration, true).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector4>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Vector4_Changed != null)
                                arg.Act_On_Vector4_Changed(CustomValue_Vector4);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
            }
            else if (arg.Type == TweenNodeType.自定义颜色)
            {
                bool sw = false;
                //方式：起始 -> 默认
                if (sw_From && !sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = XTween.To(() => CustomValue_Color, x => CustomValue_Color = x, arg.Original_Color, arg.Duration * Duration, true).SetFrom(arg.From_Color).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Color>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Color_Changed != null)
                                arg.Act_On_Color_Changed(CustomValue_Color);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = XTween.To(() => CustomValue_Color, x => CustomValue_Color = x, arg.Original_Color, arg.Duration * Duration, true).SetFrom(arg.From_Color).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Color>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Color_Changed != null)
                                arg.Act_On_Color_Changed(CustomValue_Color);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：默认 -> 结束
                else if (!sw_From && sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = XTween.To(() => CustomValue_Color, x => CustomValue_Color = x, arg.End_Color, arg.Duration * Duration).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Color>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Color_Changed != null)
                                arg.Act_On_Color_Changed(CustomValue_Color);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = XTween.To(() => CustomValue_Color, x => CustomValue_Color = x, arg.End_Color, arg.Duration * Duration).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Color>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Color_Changed != null)
                                arg.Act_On_Color_Changed(CustomValue_Color);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：起始 -> 结束
                else if (sw_From && sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = XTween.To(() => CustomValue_Color, x => CustomValue_Color = x, arg.End_Color, arg.Duration * Duration, true).SetFrom(arg.From_Color).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Color>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Color_Changed != null)
                                arg.Act_On_Color_Changed(CustomValue_Color);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = XTween.To(() => CustomValue_Color, x => CustomValue_Color = x, arg.End_Color, arg.Duration * Duration, true).SetFrom(arg.From_Color).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Color>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Color_Changed != null)
                                arg.Act_On_Color_Changed(CustomValue_Color);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：当前 -> 结束
                else if (!sw_From && !sw_End && sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.Unset)
                        tween = XTween.To(() => CustomValue_Color, x => CustomValue_Color = x, arg.End_Color, arg.Duration * Duration).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Color>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Color_Changed != null)
                                arg.Act_On_Color_Changed(CustomValue_Color);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        tween = XTween.To(() => CustomValue_Color, x => CustomValue_Color = x, arg.End_Color, arg.Duration * Duration).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Color>((v, d, t) =>
                        {
                            if (tween.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (AnimationAction_Percentage != null)
                                        AnimationAction_Percentage();
                                }
                            }
                            if (arg.Act_On_Color_Changed != null)
                                arg.Act_On_Color_Changed(CustomValue_Color);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
            }

            arg.Tweener = tween;

            return tween;
        }
        /*如果目标动画方式为：“当前 -> 结束”则不执行退回动画，因为此方式是指让动画直接到达目标值（没有起始值，如果有也只是他的当前值），所以倒退对其没有意义，其他动画方式可忽略此提示*/
        /// <summary>
        /// 复位动画
        /// </summary>
        /// <param tweenName="arg">目标动画节点</param>
        public void Tweener_Rewind(TweenNode arg)
        {
            /*如果动画方式为：当前 -> 结束则不执行退回动画，因为此方式是指让动画直接到达目标值（没有起始值，如果有也只是他的当前值），所以倒退对其没有意义*/
            if (arg.ActivateOnlyToEnd)
                return;

            bool sw_From = arg.ActivateFrom;
            bool sw_End = arg.ActivateEnd;

            #region 杀死动画
            arg.Tweener.Kill();
            arg.Tweener.Rewind();
            #endregion

            arg.Progress = 0;

            #region 重置声音
            arg.Progress = 0;
            if (arg.TweenSounds != null && arg.TweenSounds.Count > 0)
            {
                for (int i = 0; i < arg.TweenSounds.Count; i++)
                {
                    arg.TweenSounds[i].IsPlayed = false;
                }
            }
            #endregion            

            if (arg.Type == TweenNodeType.位移)
            {
                if (sw_From && sw_End)
                {
                    mod_RectTransform.anchoredPosition3D = arg.From_Vector3;
                    if (arg.Act_On_Vector3_Rewind != null)
                    {
                        arg.Act_On_Vector3_Rewind(arg.From_Vector3);
                    }
                }
                else if (sw_From && !sw_End)
                {
                    mod_RectTransform.anchoredPosition3D = arg.From_Vector3;
                    if (arg.Act_On_Vector3_Rewind != null)
                    {
                        arg.Act_On_Vector3_Rewind(arg.From_Vector3);
                    }
                }
                else if (!sw_From && sw_End)
                {
                    mod_RectTransform.anchoredPosition3D = arg.Original_Vector3;
                    if (arg.Act_On_Vector3_Rewind != null)
                    {
                        arg.Act_On_Vector3_Rewind(arg.Original_Vector3);
                    }
                }
            }
            else if (arg.Type == TweenNodeType.旋转)
            {
                if (sw_From && sw_End)
                {
                    mod_RectTransform.rotation = Quaternion.Euler(arg.From_Vector3);
                    if (arg.Act_On_Quaternion_Rewind != null)
                    {
                        arg.Act_On_Quaternion_Rewind(Quaternion.Euler(arg.From_Vector3));
                    }
                }
                else if (sw_From && !sw_End)
                {
                    mod_RectTransform.rotation = Quaternion.Euler(arg.From_Vector3);
                    if (arg.Act_On_Quaternion_Rewind != null)
                    {
                        arg.Act_On_Quaternion_Rewind(Quaternion.Euler(arg.From_Vector3));
                    }
                }
                else if (!sw_From && sw_End)
                {
                    mod_RectTransform.rotation = Quaternion.Euler(arg.Original_Vector3);
                    if (arg.Act_On_Quaternion_Rewind != null)
                    {
                        arg.Act_On_Quaternion_Rewind(Quaternion.Euler(arg.Original_Vector3));
                    }
                }
            }
            else if (arg.Type == TweenNodeType.缩放)
            {
                if (sw_From && sw_End)
                {
                    mod_RectTransform.localScale = arg.From_Vector3;
                    if (arg.Act_On_Vector3_Rewind != null)
                    {
                        arg.Act_On_Vector3_Rewind(arg.From_Vector3);
                    }
                }
                else if (sw_From && !sw_End)
                {
                    mod_RectTransform.localScale = arg.From_Vector3;
                    if (arg.Act_On_Vector3_Rewind != null)
                    {
                        arg.Act_On_Vector3_Rewind(arg.From_Vector3);
                    }
                }
                else if (!sw_From && sw_End)
                {
                    mod_RectTransform.localScale = arg.Original_Vector3;
                    if (arg.Act_On_Vector3_Rewind != null)
                    {
                        arg.Act_On_Vector3_Rewind(arg.Original_Vector3);
                    }
                }
            }
            else if (arg.Type == TweenNodeType.颜色)
            {
                Graphic gc = Modules_RecognitionType();
                if (gc == null)
                {
                    return;
                }
                if (sw_From && sw_End)
                {
                    gc.color = arg.From_Color;
                    if (arg.Act_On_Color_Rewind != null)
                    {
                        arg.Act_On_Color_Rewind(arg.From_Color);
                    }
                }
                else if (sw_From && !sw_End)
                {
                    gc.color = arg.From_Color;
                    if (arg.Act_On_Color_Rewind != null)
                    {
                        arg.Act_On_Color_Rewind(arg.From_Color);
                    }
                }
                else if (!sw_From && sw_End)
                {
                    gc.color = arg.Original_Color;
                    if (arg.Act_On_Color_Rewind != null)
                    {
                        arg.Act_On_Color_Rewind(arg.Original_Color);
                    }
                }
            }
            else if (arg.Type == TweenNodeType.淡化)
            {
                if (CanvasGroup == null)
                {
                    return;
                }
                if (sw_From && sw_End)
                {
                    CanvasGroup.alpha = arg.From_Float;
                    if (arg.Act_On_Float_Rewind != null)
                    {
                        arg.Act_On_Float_Rewind(arg.From_Float);
                    }
                }
                else if (sw_From && !sw_End)
                {
                    CanvasGroup.alpha = arg.From_Float;
                    if (arg.Act_On_Float_Rewind != null)
                    {
                        arg.Act_On_Float_Rewind(arg.From_Float);
                    }
                }
                else if (!sw_From && sw_End)
                {
                    CanvasGroup.alpha = arg.Original_Float;
                    if (arg.Act_On_Float_Rewind != null)
                    {
                        arg.Act_On_Float_Rewind(arg.Original_Float);
                    }
                }
            }
            else if (arg.Type == TweenNodeType.打字机)
            {
                if (mod_Text == null && mod_TmpText == null)
                {
                    return;
                }
                else
                {
                    if (sw_From && sw_End)
                    {
                        if (mod_Text != null)
                        {
                            mod_Text.txt_Set_Content(arg.From_String);
                        }
                        if (mod_TmpText != null)
                        {
                            mod_TmpText.SetText(arg.From_String);
                        }
                        if (arg.Act_On_Text_Rewind != null)
                        {
                            arg.Act_On_Text_Rewind(arg.From_String);
                        }
                    }
                    else if (sw_From && !sw_End)
                    {
                        if (mod_Text != null)
                        {
                            mod_Text.txt_Set_Content(arg.From_String);
                        }
                        if (mod_TmpText != null)
                        {
                            mod_TmpText.SetText(arg.From_String);
                        }
                        if (arg.Act_On_Text_Rewind != null)
                        {
                            arg.Act_On_Text_Rewind(arg.From_String);
                        }
                    }
                    else if (!sw_From && sw_End)
                    {
                        if (mod_Text != null)
                        {
                            mod_Text.txt_Set_Content(arg.Original_String);
                        }
                        if (mod_TmpText != null)
                        {
                            mod_TmpText.SetText(arg.Original_String);
                        }
                        if (arg.Act_On_Text_Rewind != null)
                        {
                            arg.Act_On_Text_Rewind(arg.Original_String);
                        }
                    }
                }
            }
            else if (arg.Type == TweenNodeType.尺寸)
            {
                if (sw_From && sw_End)
                {
                    mod_RectTransform.sizeDelta = arg.From_Vector2;
                    if (arg.Act_On_Vector2_Rewind != null)
                    {
                        arg.Act_On_Vector2_Rewind(arg.From_Vector2);
                    }
                }
                else if (sw_From && !sw_End)
                {
                    mod_RectTransform.sizeDelta = arg.From_Vector2;
                    if (arg.Act_On_Vector2_Rewind != null)
                    {
                        arg.Act_On_Vector2_Rewind(arg.From_Vector2);
                    }
                }
                else if (!sw_From && sw_End)
                {
                    mod_RectTransform.sizeDelta = arg.Original_Vector2;
                    if (arg.Act_On_Vector2_Rewind != null)
                    {
                        arg.Act_On_Vector2_Rewind(arg.Original_Vector2);
                    }
                }
            }
            else if (arg.Type == TweenNodeType.图像填充)
            {
                if (mod_Image == null)
                {
                    return;
                }
                if (sw_From && sw_End)
                {
                    mod_Image.fillAmount = arg.From_Float;
                    if (arg.Act_On_Float_Rewind != null)
                    {
                        arg.Act_On_Float_Rewind(arg.From_Float);
                    }
                }
                else if (sw_From && !sw_End)
                {
                    mod_Image.fillAmount = arg.From_Float;
                    if (arg.Act_On_Float_Rewind != null)
                    {
                        arg.Act_On_Float_Rewind(arg.From_Float);
                    }
                }
                else if (!sw_From && sw_End)
                {
                    mod_Image.fillAmount = arg.Original_Float;
                    if (arg.Act_On_Float_Rewind != null)
                    {
                        arg.Act_On_Float_Rewind(arg.Original_Float);
                    }
                }
            }
            else if (arg.Type == TweenNodeType.自定义整数)
            {
                if (sw_From && sw_End)
                {
                    CustomValue_Int = arg.From_Int;
                    if (arg.Act_On_Int_Rewind != null)
                    {
                        arg.Act_On_Int_Rewind(arg.From_Int);
                    }
                }
                else if (sw_From && !sw_End)
                {
                    CustomValue_Int = arg.From_Int;
                    if (arg.Act_On_Int_Rewind != null)
                    {
                        arg.Act_On_Int_Rewind(arg.From_Int);
                    }
                }
                else if (!sw_From && sw_End)
                {
                    CustomValue_Int = arg.Original_Int;
                    if (arg.Act_On_Int_Rewind != null)
                    {
                        arg.Act_On_Int_Rewind(arg.Original_Int);
                    }
                }
            }
            else if (arg.Type == TweenNodeType.自定义浮点数)
            {
                if (sw_From && sw_End)
                {
                    CustomValue_Float = arg.From_Float;
                    if (arg.Act_On_Float_Rewind != null)
                    {
                        arg.Act_On_Float_Rewind(arg.From_Float);
                    }
                }
                else if (sw_From && !sw_End)
                {
                    CustomValue_Float = arg.From_Float;
                    if (arg.Act_On_Float_Rewind != null)
                    {
                        arg.Act_On_Float_Rewind(arg.From_Float);
                    }
                }
                else if (!sw_From && sw_End)
                {
                    CustomValue_Float = arg.Original_Float;
                    if (arg.Act_On_Float_Rewind != null)
                    {
                        arg.Act_On_Float_Rewind(arg.Original_Float);
                    }
                }
            }
            else if (arg.Type == TweenNodeType.自定义2维向量)
            {
                if (sw_From && sw_End)
                {
                    CustomValue_Vector2 = arg.From_Vector2;
                    if (arg.Act_On_Vector2_Rewind != null)
                    {
                        arg.Act_On_Vector2_Rewind(arg.From_Vector2);
                    }
                }
                else if (sw_From && !sw_End)
                {
                    CustomValue_Vector2 = arg.From_Vector2;
                    if (arg.Act_On_Vector2_Rewind != null)
                    {
                        arg.Act_On_Vector2_Rewind(arg.From_Vector2);
                    }
                }
                else if (!sw_From && sw_End)
                {
                    CustomValue_Vector2 = arg.Original_Vector2;
                    if (arg.Act_On_Vector2_Rewind != null)
                    {
                        arg.Act_On_Vector2_Rewind(arg.Original_Vector2);
                    }
                }
            }
            else if (arg.Type == TweenNodeType.自定义3维向量)
            {
                if (sw_From && sw_End)
                {
                    CustomValue_Vector3 = arg.From_Vector3;
                    if (arg.Act_On_Vector3_Rewind != null)
                    {
                        arg.Act_On_Vector3_Rewind(arg.From_Vector3);
                    }
                }
                else if (sw_From && !sw_End)
                {
                    CustomValue_Vector3 = arg.From_Vector3;
                    if (arg.Act_On_Vector3_Rewind != null)
                    {
                        arg.Act_On_Vector3_Rewind(arg.From_Vector3);
                    }
                }
                else if (!sw_From && sw_End)
                {
                    CustomValue_Vector3 = arg.Original_Vector3;
                    if (arg.Act_On_Vector3_Rewind != null)
                    {
                        arg.Act_On_Vector3_Rewind(arg.Original_Vector3);
                    }
                }
            }
            else if (arg.Type == TweenNodeType.自定义4维向量)
            {
                if (sw_From && sw_End)
                {
                    CustomValue_Vector4 = arg.From_Vector4;
                    if (arg.Act_On_Vector4_Rewind != null)
                    {
                        arg.Act_On_Vector4_Rewind(arg.From_Vector4);
                    }
                }
                else if (sw_From && !sw_End)
                {
                    CustomValue_Vector4 = arg.From_Vector4;
                    if (arg.Act_On_Vector4_Rewind != null)
                    {
                        arg.Act_On_Vector4_Rewind(arg.From_Vector4);
                    }
                }
                else if (!sw_From && sw_End)
                {
                    CustomValue_Vector4 = arg.Original_Vector4;
                    if (arg.Act_On_Vector4_Rewind != null)
                    {
                        arg.Act_On_Vector4_Rewind(arg.Original_Vector4);
                    }
                }
            }
            else if (arg.Type == TweenNodeType.自定义颜色)
            {
                if (sw_From && sw_End)
                {
                    CustomValue_Color = arg.From_Color;
                    if (arg.Act_On_Color_Rewind != null)
                    {
                        arg.Act_On_Color_Rewind(arg.From_Color);
                    }
                }
                else if (sw_From && !sw_End)
                {
                    CustomValue_Color = arg.From_Color;
                    if (arg.Act_On_Color_Rewind != null)
                    {
                        arg.Act_On_Color_Rewind(arg.From_Color);
                    }
                }
                else if (!sw_From && sw_End)
                {
                    CustomValue_Color = arg.Original_Color;
                    if (arg.Act_On_Color_Rewind != null)
                    {
                        arg.Act_On_Color_Rewind(arg.Original_Color);
                    }
                }
            }
        }
        /// <summary>
        /// 杀死动画
        /// </summary>
        /// <param tweenName="arg">目标动画节点</param>
        public void Tweener_Killed(TweenNode arg)
        {
            #region 杀死动画
            arg.Tweener.Kill();
            #endregion

            #region 重置声音
            arg.Progress = 0;
            if (arg.TweenSounds != null && arg.TweenSounds.Count > 0)
            {
                for (int i = 0; i < arg.TweenSounds.Count; i++)
                {
                    arg.TweenSounds[i].IsPlayed = false;
                }
            }
            #endregion          
        }
        #endregion

        #region 音效播放
        /// <summary>
        /// 生成动画音效
        /// </summary>
        /// <param tweenName="name"></param>
        /// <returns></returns>
        private AudioSource PlayTweenSound(AudioClip clip, float vol, float pitch_min, float pitch_max)
        {
            if (MutePlay)
                return null;
            if (act_on_Animator_SoundPlay != null)
                act_on_Animator_SoundPlay(clip);
            eve_on_Animator_SoundPlay.Invoke(clip);
            ///-------获取播放器
            AudioSource player = XHud_Manager.Instance.hm_LibrarySounds_GetSounder();
            if (player != null)
            {
                ///-------获取声音剪辑
                player.clip = clip;
                player.volume = XHud_Manager.Instance.Volume * 0.01f * vol;
                player.mute = XHud_Manager.Instance.VolumeMute;
                player.pitch = Random.Range(pitch_min, pitch_max);
                if (player.clip != null)
                {
                    player.Play();
                }
                else
                {
                    Debug.Log("未找到音频剪辑，请检查并确保音频剪辑是否存在！");
                }
            }
            return player;
        }
        #endregion
    }
}