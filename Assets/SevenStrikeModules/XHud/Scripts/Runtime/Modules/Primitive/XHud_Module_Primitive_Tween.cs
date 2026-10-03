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
    using SevenStrikeModules.XGUI.Runtime;
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XTween;
    using System.Collections.Generic;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif
    using UnityEngine.Events;
    using UnityEngine.UI;

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
        /// 音效Timings
        /// </summary>
        public string Timings;
        /// <summary>
        /// 音效延迟播放时间
        /// </summary>
        public float Delay;
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

        public TweenSound() { }

        public TweenSound(AudioClip sound, string path, float delay, float volume, float maxPitch, float minPitch)
        {
            Sound = sound;
            Path = path;
            Delay = delay;
            Volume = volume;
            MaxPitch = maxPitch;
            MinPitch = minPitch;
        }

        /// <summary>
        /// 设置音高
        /// </summary>
        /// <param name="min">最小音高</param>
        /// <param name="max">最大音高</param>
        public void SetPitch(float min, float max)
        {
            MinPitch = min;
            MaxPitch = max;
        }

        /// <summary>
        /// 设置音量值
        /// </summary>
        /// <param name="vol">音量值</param>
        public void SetVolume(float vol)
        {
            Volume = vol;
        }

        public void GetSoundPath()
        {
#if UNITY_EDITOR
            if (Sound == null)
            {
                Path = "";
                return;
            }
            Path = AssetDatabase.GetAssetPath(Sound);
#endif
        }

        private TweenSound Clone()
        {
            TweenSound sod = new TweenSound();
            sod.Sound = this.Sound;           // AudioClip 是 UnityEngine.Object，引用即可
            sod.Path = this.Path;
            sod.Volume = this.Volume;
            sod.MaxPitch = this.MaxPitch;
            sod.MinPitch = this.MinPitch;
            // act_on_SoundPlay 委托不克隆，新节点需要重新绑定
            sod.act_on_SoundPlay = null;

            return sod;
        }

        public void CopyTo(TweenSound source)
        {
            if (source == null) return;

            source.Sound = this.Sound;
            source.Delay = this.Delay;
            source.Path = this.Path;
            source.Volume = this.Volume;
            source.MaxPitch = this.MaxPitch;
            source.MinPitch = this.MinPitch;
        }
    }

    [SerializeField]
    [System.Serializable]
    public class TweenNode
    {
        /// <summary>
        /// 动画参数 - 标识ID
        /// </summary>
        public int ID;
        /// <summary>
        /// 动画参数 - 标识名称
        /// </summary>
        public string Indicator = "NewTween";
        /// <summary>
        /// 动画参数 - 可用性开关
        /// </summary>
        public bool Enabled = true;
        /// <summary>
        /// 动画参数 - 动画类型
        /// </summary>
        public TweenNodeType Type = TweenNodeType.a_位移;
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
        /// <summary>
        /// 动画参数 - 文字动画光标字符闪烁速度
        /// </summary>
        public float TextCursorBlinkSpeed = 0.5f;
        /// <summary>
        /// 动画参数 - 文字动画光标字符
        /// </summary>
        public string TextCursor = "|";
        /// <summary>
        /// 动画参数 - 动画倒退后是否将值设为起始值
        /// </summary>
        public bool Rewind_Set_Startvalue = true;
        /// <summary>
        /// 动画参数 - 动画完成后是否将值设为结束值
        /// </summary>
        public bool Complete_Set_Endvalue = true;
        /// <summary>
        /// 动画参数 - 缓动参数
        /// </summary>
        public EaseMode Ease = EaseMode.InOutCubic;
        public string AnimationCurveName = "";
        /// <summary>
        /// 数值模式索引
        /// </summary>
        public int ValueModeIndex = 0;
        public TweenValueMode TweenValueMode;
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
        /// 深度克隆当前动画节点，创建完全独立的副本
        /// </summary>
        /// <returns>新的 TweenNode 实例，所有值类型字段独立复制，引用类型字段重新创建</returns>
        public TweenNode Clone()
        {
            TweenNode newNode = new TweenNode();

            // ========== 基础属性 ==========
            newNode.ID = this.ID;
            newNode.Indicator = this.Indicator;
            newNode.Enabled = this.Enabled;
            newNode.Type = this.Type;
            newNode.Timings = this.Timings;
            newNode.Duration = this.Duration;
            newNode.Delay = this.Delay;
            newNode.Progress = this.Progress;
            newNode.Rewind_Set_Startvalue = this.Rewind_Set_Startvalue;
            newNode.Complete_Set_Endvalue = this.Complete_Set_Endvalue;

            // ========== 缓动参数 ==========
            newNode.Ease = this.Ease;
            newNode.AnimationCurveName = this.AnimationCurveName;
            // AnimationCurve 需要深拷贝，不能直接引用
            newNode.Curve = new AnimationCurve(this.Curve.keys);

            // ========== 原始值（值类型直接复制）==========
            newNode.Original_Int = this.Original_Int;
            newNode.Original_Float = this.Original_Float;
            newNode.Original_Vector2 = this.Original_Vector2;
            newNode.Original_Vector3 = this.Original_Vector3;
            newNode.Original_Vector4 = this.Original_Vector4;
            newNode.Original_Color = this.Original_Color;
            newNode.Original_String = this.Original_String;

            // ========== 起始值 ==========
            newNode.From_Int = this.From_Int;
            newNode.From_Float = this.From_Float;
            newNode.From_Vector2 = this.From_Vector2;
            newNode.From_Vector3 = this.From_Vector3;
            newNode.From_Vector4 = this.From_Vector4;
            newNode.From_Color = this.From_Color;
            newNode.From_String = this.From_String;

            // ========== 结束值 ==========
            newNode.End_Int = this.End_Int;
            newNode.End_Float = this.End_Float;
            newNode.End_Vector2 = this.End_Vector2;
            newNode.End_Vector3 = this.End_Vector3;
            newNode.End_Vector4 = this.End_Vector4;
            newNode.End_Color = this.End_Color;
            newNode.End_String = this.End_String;

            // ========== 动画选项 ==========
            newNode.RotateMode = this.RotateMode;
            newNode.LoopType = this.LoopType;
            newNode.LoopCount = this.LoopCount;

            // ========== 数值模式索引 ==========
            newNode.ValueModeIndex = this.ValueModeIndex;
            newNode.TweenValueMode = this.TweenValueMode;

            // ========== 注意：以下字段不克隆 ==========
            // - Tweener: 动画器实例，克隆后应该为 null，由新节点独立创建
            // - IsFold: UI 折叠状态，编辑器专用，不克隆
            // - 所有 UnityAction 委托: 不克隆，需要重新绑定
            //   (Act_On_Int_Changed, Act_On_Float_Changed, Act_On_Vector3_Changed 等)

            return newNode;
        }

        /// <summary>
        /// 浅克隆（仅复制值类型，引用类型仍指向原对象）
        /// 用于性能要求高且不需要深度独立的场景
        /// </summary>
        /// <returns>新的 TweenNode 实例（浅拷贝）</returns>
        public TweenNode ShallowClone()
        {
            return (TweenNode)this.MemberwiseClone();
        }

        /// <summary>
        /// 复制动画节点数据到当前节点（用于编辑器中的复制/粘贴）
        /// </summary>
        /// <param name="source">源动画节点</param>
        /// <param name="RecreateID">是否重新分配ID</param>
        public void CopyFrom(TweenNode source, bool RecreateID)
        {
            if (source == null) return;

            // 基础属性
            this.Indicator = source.Indicator;
            this.Enabled = source.Enabled;
            this.Type = source.Type;
            this.Timings = source.Timings;
            this.Duration = source.Duration;
            this.Delay = source.Delay;
            this.Rewind_Set_Startvalue = source.Rewind_Set_Startvalue;
            this.Complete_Set_Endvalue = source.Complete_Set_Endvalue;

            // 缓动参数
            this.Ease = source.Ease;
            this.AnimationCurveName = source.AnimationCurveName;
            this.Curve = new AnimationCurve(source.Curve.keys);

            // 原始值
            this.Original_Int = source.Original_Int;
            this.Original_Float = source.Original_Float;
            this.Original_Vector2 = source.Original_Vector2;
            this.Original_Vector3 = source.Original_Vector3;
            this.Original_Vector4 = source.Original_Vector4;
            this.Original_Color = source.Original_Color;
            this.Original_String = source.Original_String;

            // 起始值
            this.From_Int = source.From_Int;
            this.From_Float = source.From_Float;
            this.From_Vector2 = source.From_Vector2;
            this.From_Vector3 = source.From_Vector3;
            this.From_Vector4 = source.From_Vector4;
            this.From_Color = source.From_Color;
            this.From_String = source.From_String;

            // 结束值
            this.End_Int = source.End_Int;
            this.End_Float = source.End_Float;
            this.End_Vector2 = source.End_Vector2;
            this.End_Vector3 = source.End_Vector3;
            this.End_Vector4 = source.End_Vector4;
            this.End_Color = source.End_Color;
            this.End_String = source.End_String;

            // 动画选项
            this.RotateMode = source.RotateMode;
            this.LoopType = source.LoopType;
            this.LoopCount = source.LoopCount;

            // 数值模式索引
            this.ValueModeIndex = source.ValueModeIndex;
            this.TweenValueMode = source.TweenValueMode;

            // 注意：ID 不复制，保持原节点的 ID 或由调用方重新生成
            // Tweener 不复制
            // 委托事件不复制
        }
    }

    [SerializeField]
    [System.Serializable]
    public class TweenNodeArray
    {
        public List<TweenNode> TweenNodeList = new List<TweenNode>();
    }

    public class XHud_Module_Primitive_Tween : MonoBehaviour
    {
        /// <summary>
        /// 图元控制器引用
        /// 提供图元各组件（RectTransform、Image、Text、TMP_Text、CanvasGroup 等）的访问入口
        /// 动画创建与复位时通过它获取实际的目标组件
        /// </summary>
        public XHud_Module_Primitive_Controller controller;

        #region 图元动画参数
        /// <summary>
        /// 静音播放开关
        /// 勾选后，动画过程中所有音效触发点将被忽略，不播放任何音效
        /// 用途：批量关闭音效、测试环境静音、性能优化
        /// </summary>
        [SerializeField] public bool MutePlay;
        /// <summary>
        /// 动画播放状态的中断模式开关（私有）
        /// 用于 TweenNode_IsAllAnimating() 中的边缘检测：
        /// - true  ：表示当前有动画正在播放
        /// - false ：表示所有动画已停止
        /// 状态由"播放中 → 已停止"切换时，触发一次 Complete 事件
        /// 设计目的：确保 Complete 事件只在状态切换的瞬间触发一次，而非每帧触发
        /// </summary>
        [SerializeField] private bool AnimatingBreakState;
        /// <summary>
        /// 忽略上级 Element 控制的开关
        /// 勾选后，上级 Element 将不再控制该动画器的播放、复位、杀死等操作
        /// 用途：某些图元动画需要独立于父级元素的生命周期自行管理
        /// </summary>
        [SerializeField] public bool IgnoreElementAnimationPlay;
        /// <summary>
        /// 预览动画开关（编辑器专用）
        /// 用于在编辑器模式下预览动画效果，运行时不影响实际逻辑
        /// </summary>
        [SerializeField] public bool TweenIsPreviewing;
        /// <summary>
        /// 起始动画节点标识（Indicator 名称）
        /// 指定图元在 Element 系统调度时作为"主入口"优先播放的动画节点
        /// 通过 TweenNode_GetByIndicator(MainTweenNode) 定位
        /// </summary>
        [SerializeField] public string MainTweenNode;
        /// <summary>
        /// 动画节点列表
        /// 存储当前图元的所有动画配置与运行时状态
        /// 每个 TweenNode 代表一条独立的动画轨道（位移、旋转、缩放、颜色等）
        /// </summary>
        [SerializeField] public List<TweenNode> PrimitiveTweenNodes = new List<TweenNode>();
        [SerializeField] public List<TweenSound> PrimitiveTweenSounds = new List<TweenSound>();
        #endregion

        #region 成员 - 速率&时间
        /// <summary>
        /// 动画全局速率倍率
        /// 作为时长乘数作用于所有动画节点：
        /// 实际时长 = node.Duration × XHud_Manager.DurationMultiply × GlobalDuration
        /// - 值为 1  ：正常速度
        /// - 值 &lt; 1 ：加速播放（如 0.5 表示两倍速）
        /// - 值 &gt; 1 ：减速播放（如 2 表示半速）
        /// </summary>
        [SerializeField] public float GlobalDuration = 1;
        /// <summary>
        /// 动画列表中筛选出的最短耗时（未乘全局速率）
        /// 由 TweenNode_GetTimers() 计算并缓存
        /// 单个节点耗时公式：Duration × Clamp(LoopCount, 1, int.MaxValue) + Delay
        /// 循环次数为 -1（无限循环）时按 1 次处理，避免结果无限大
        /// </summary>
        [SerializeField] public float MinTimer;
        /// <summary>
        /// 动画列表中筛选出的最长耗时（未乘全局速率）
        /// 由 TweenNode_GetTimers() 计算并缓存
        /// 用途：预估所有动画播放完毕所需的最长等待时间
        /// </summary>
        [SerializeField] public float MaxTimer;
        /// <summary>
        /// 应用全局速率后的最短耗时
        /// 计算公式：MinTimer × GlobalDuration
        /// 用途：UI 显示、调度预估时使用最终有效时长
        /// </summary>
        [SerializeField] public float MinTimerWithGlobalDuration;
        /// <summary>
        /// 应用全局速率后的最长耗时
        /// 计算公式：MaxTimer × GlobalDuration
        /// 用途：协程等待所有动画完成时的最大等待时间
        /// </summary>
        [SerializeField] public float MaxTimerWithGlobalDuration;
        #endregion

        #region 成员 - 动作
        /// <summary>
        /// 动作 - 动画播放时触发 - 参数为 XTween_Interface
        /// 触发时机：调用 Tween_Play(XTween_Interface tween) 时
        /// 用途：外部系统对底层动画实例进行监听
        /// </summary>
        public UnityAction<XTween_Interface> act_on_Tween_Play;
        /// <summary>
        /// 动作 - 动画播放时触发 - 参数为 TweenNode
        /// 触发时机：调用 Tween_Play(TweenNode node) 时
        /// 用途：以节点为单位监听播放事件
        /// </summary>
        public UnityAction<TweenNode> act_on_Tween_Play_At_Node;
        /// <summary>
        /// 动作 - 动画播放时触发 - 参数为动画 ID
        /// 触发时机：调用 Tween_Play(int id) 时
        /// 用途：通过 ID 精确追踪某条动画的播放
        /// </summary>
        public UnityAction<int> act_on_Tween_Play_At_ID;
        /// <summary>
        /// 动作 - 动画播放时触发 - 参数为动画标识名称
        /// 触发时机：调用 Tween_Play(string indicator) 时
        /// 用途：通过名称追踪某条动画的播放（名称可重名，不唯一）
        /// </summary>
        public UnityAction<string> act_on_Tween_Play_At_Indicator;
        /// <summary>
        /// 动作 - 全部动画播放时触发 - 无参数
        /// 触发时机：调用 Tweens_Play_With_Delay(...) 批量播放后
        /// 用途：批量播放完成后的统一回调
        /// </summary>
        public UnityAction act_on_Tween_PlayAll;
        /// <summary>
        /// 动作 - 动画创建完成时触发 - 参数为 TweenNode
        /// 触发时机：Tween_CreateById(...) 创建动画实例后
        /// 用途：获取已创建动画节点的引用，绑定后续逻辑
        /// </summary>
        public UnityAction<TweenNode> act_on_Tween_Create_At_ID;
        /// <summary>
        /// 动作 - 动画创建完成时触发 - 参数为 TweenNode
        /// 触发时机：Tween_CreateByIndex(...) 创建动画实例后
        /// 用途：以索引方式创建后获取节点引用
        /// </summary>
        public UnityAction<TweenNode> act_on_Tween_Create_At_Index;
        /// <summary>
        /// 动作 - 动画创建完成时触发 - 参数为 TweenNode
        /// 触发时机：Tween_CreateByIndicator(...) 创建动画实例后
        /// 用途：以名称方式创建后获取节点引用
        /// </summary>
        public UnityAction<TweenNode> act_on_Tween_Create_At_Indicator;
        /// <summary>
        /// 动作 - 动画复位时触发 - 参数为 TweenNode
        /// 触发时机：调用 Tween_Rewind(node, ...) 时
        /// 用途：复位后的数值恢复、状态重置等后续处理
        /// </summary>
        public UnityAction<TweenNode> act_on_Tween_Rewind;
        /// <summary>
        /// 动作 - 全部动画复位时触发 - 无参数
        /// 触发时机：调用 Tween_RewindAll(...) 后
        /// 用途：批量复位后的统一回调
        /// </summary>
        public UnityAction act_on_Tween_RewindAll;
        /// <summary>
        /// 动作 - 动画杀死时触发 - 参数为 TweenNode
        /// 触发时机：调用 Tween_Kill(node, ...) 时
        /// 用途：杀死动画（释放 Tweener 实例）后的清理回调
        /// </summary>
        public UnityAction<TweenNode> act_on_Tween_Kill;
        /// <summary>
        /// 动作 - 全部动画杀死时触发 - 无参数
        /// 触发时机：调用 Tween_KillAll(...) 后
        /// 用途：批量杀死后的统一回调
        /// </summary>
        public UnityAction act_on_Tween_KillAll;
        /// <summary>
        /// 动作 - 动画准备就绪时触发 - 无参数
        /// 触发时机：动画系统初始化完成、可开始播放前
        /// 用途：外部系统在动画播放前进行前置准备
        /// </summary>
        public UnityAction act_on_Tween_Ready;
        /// <summary>
        /// 动作 - 所有动画完成时触发 - 无参数
        /// 触发时机：TweenNode_IsAllAnimating() 边缘检测到"播放中 → 已停止"时
        /// 特性：只在状态切换瞬间触发一次，不会每帧触发
        /// 用途：动画结束后的链式逻辑（播放下一段、切换 UI 状态等）
        /// </summary>
        public UnityAction act_on_Tween_Complete;
        /// <summary>
        /// 动作 - 动画器初始化完成时触发 - 无参数
        /// 触发时机：Awake() 中，进度重置之后
        /// 用途：外部系统在动画器首次可用时进行绑定
        /// </summary>
        public UnityAction act_on_Tween_Initialized;
        /// <summary>
        /// 动作 - 音效触发播放时触发 - 参数为 AudioClip
        /// 触发时机：TweenNode_ProcessNodeSounds 达到触发点并调用 PlayTweenSound 时
        /// 用途：外部系统对音效播放进行二次处理（如字幕、震动、日志）
        /// </summary>
        public UnityAction<AudioClip> act_on_Tween_SoundPlay;
        /// <summary>
        /// 动作 - 动画播放状态变化时触发 - 参数为 bool（true=正在播放）
        /// 触发时机：每帧在 TweenNode_IsAllAnimating() 中触发
        /// 用途：UI 交互禁用/启用、加载指示器显隐等
        /// </summary>
        public UnityAction<bool> act_on_Tween_IsAnimating;
        #endregion

        #region 成员 - 事件
        /// <summary>
        /// 事件 - 动画播放时触发 - 参数为 XTween_Interface
        /// 触发时机：Tween_Play(XTween_Interface tween)
        /// Inspector 绑定：可拖拽对象监听底层动画实例的播放
        /// </summary>
        public UnityEvent<XTween_Interface> eve_on_Tween_Play;
        /// <summary>
        /// 事件 - 动画播放时触发 - 参数为 TweenNode
        /// 触发时机：Tween_Play(TweenNode node)
        /// </summary>
        public UnityEvent<TweenNode> eve_on_Tween_Play_At_Node;
        /// <summary>
        /// 事件 - 动画播放时触发 - 参数为动画 ID
        /// 触发时机：Tween_Play(int id)
        /// </summary>
        public UnityEvent<int> eve_on_Tween_Play_At_ID;
        /// <summary>
        /// 事件 - 动画播放时触发 - 参数为动画标识名称
        /// 触发时机：Tween_Play(string indicator)
        /// </summary>
        public UnityEvent<string> eve_on_Tween_Play_At_Indicator;
        /// <summary>
        /// 事件 - 全部动画播放时触发 - 无参数
        /// 触发时机：Tweens_Play_With_Delay(...) 批量播放后
        /// </summary>
        public UnityEvent eve_on_Tween_PlayAll;
        /// <summary>
        /// 事件 - 动画创建完成时触发 - 参数为 TweenNode
        /// 触发时机：Tween_CreateById(...) 创建后
        /// </summary>
        public UnityEvent<TweenNode> eve_on_Tween_Create_At_ID;
        /// <summary>
        /// 事件 - 动画创建完成时触发 - 参数为 TweenNode
        /// 触发时机：Tween_CreateByIndex(...) 创建后
        /// </summary>
        public UnityEvent<TweenNode> eve_on_Tween_Create_At_Index;
        /// <summary>
        /// 事件 - 动画创建完成时触发 - 参数为 TweenNode
        /// 触发时机：Tween_CreateByIndicator(...) 创建后
        /// </summary>
        public UnityEvent<TweenNode> eve_on_Tween_Create_At_Indicator;
        /// <summary>
        /// 事件 - 动画复位时触发 - 参数为 TweenNode
        /// 触发时机：Tween_Rewind(node, ...)
        /// </summary>
        public UnityEvent<TweenNode> eve_on_Tween_Rewind;
        /// <summary>
        /// 事件 - 全部动画复位时触发 - 无参数
        /// 触发时机：Tween_RewindAll(...)
        /// </summary>
        public UnityEvent eve_on_Tween_RewindAll;
        /// <summary>
        /// 事件 - 动画杀死时触发 - 参数为 TweenNode
        /// 触发时机：Tween_Kill(node, ...)
        /// </summary>
        public UnityEvent<TweenNode> eve_on_Tween_Kill;
        /// <summary>
        /// 事件 - 全部动画杀死时触发 - 无参数
        /// 触发时机：Tween_KillAll(...)
        /// </summary>
        public UnityEvent eve_on_Tween_KillAll;
        /// <summary>
        /// 事件 - 动画准备就绪时触发 - 无参数
        /// 触发时机：动画系统初始化完成、可开始播放前
        /// </summary>
        public UnityEvent eve_on_Tween_Ready;
        /// <summary>
        /// 事件 - 所有动画完成时触发 - 无参数
        /// 触发时机：TweenNode_IsAllAnimating() 检测到播放状态结束
        /// 特性：只在状态切换瞬间触发一次
        /// </summary>
        public UnityEvent eve_on_Tween_Complete;
        /// <summary>
        /// 事件 - 动画器初始化完成时触发 - 无参数
        /// 触发时机：Awake() 中，进度重置之后
        /// </summary>
        public UnityEvent eve_on_Tween_Initialized;
        /// <summary>
        /// 事件 - 音效触发播放时触发 - 参数为 AudioClip
        /// 触发时机：PlayTweenSound(...) 实际播放音效时
        /// </summary>
        public UnityEvent<AudioClip> eve_on_Tween_SoundPlay;
        /// <summary>
        /// 事件 - 动画播放状态变化时触发 - 参数为 bool（true=正在播放）
        /// 触发时机：每帧在 TweenNode_IsAllAnimating() 中触发
        /// 用途：与 act_on_Tween_IsAnimating 对应，供 Inspector 可视化绑定
        /// </summary>
        public UnityEvent<bool> eve_on_Tween_AnimatingState;
        #endregion

        #region 状态开关
        /// <summary>
        /// 调试开关
        /// </summary>
        [SerializeField] public bool UseDebug;
        #endregion

        #region 预览时机
        /// <summary>
        /// 预览时机
        /// </summary>
        [SerializeField] public string PreviewTiming;
        #endregion

        #region 折叠
        public bool
            fold_param = true,
            fold_option = true,
            fold_based = true,
            fold_list = true,
            fold_state = true;

        public void GroupFold(bool state)
        {
            fold_param = state;
            fold_option = state;
            fold_based = state;
            fold_list = state;
            fold_state = state;
        }

        public void GroupFold(bool param = true, bool option = true, bool based = true, bool list = true, bool state = true)
        {
            fold_param = param;
            fold_option = option;
            fold_based = based;
            fold_list = list;
            fold_state = state;
        }
        #endregion

        #region 编辑器视图状态
        /// <summary>动
        /// 画轨道时间轴 - 每秒像素数（缩放）
        /// </summary>
        [SerializeField] public float Timeline_TrackPosition = 100f;
        /// <summary>
        /// 动画轨道时间轴 - 水平滚动量（像素）
        /// </summary>
        [SerializeField] public Vector2 Timeline_TrackScroll = Vector2.zero;
        /// <summary>
        /// 动画轨道时间轴 - 轨道高度（像素）
        /// </summary>
        [SerializeField] public float Timeline_TrackHeight = 0f;
        /// <summary>
        /// 动画轨道时间轴 - 轨道吸附（像素）
        /// </summary>
        [SerializeField] public bool Timeline_TrackSnapEnabled = false;
        #endregion

        /*               Unity 引擎加载 GameObject
         *                 │
         *                 ▼
         *       ┌─────────────────────────────┐
         *       │         Awake()             │
         *       │                             │
         *       │  ① TweenNode_AllTweenPro... │  ← 进度归零
         *       │         Progress_Reset()    │
         *       │                             │
         *       │  ② act_on_Tween_Initialized │  ← 代码回调
         *       │         ?.Invoke()          │
         *       │                             │
         *       │  ③ eve_on_Tween_Initialized │  ← Inspector 事件
         *       │         .Invoke()           │
         *       └─────────────────────────────┘
         *                 │
         *                 ▼
         *             Start() 后续调用
         *                 │
         *                 ▼
         *             Update() 每帧执行
         */

        #region 生命周期
        /// <summary>
        /// Unity 生命周期方法 - Awake（唤醒）
        /// 
        /// 调用时机：
        /// - 在脚本实例被加载时调用（早于 Start）
        /// - 若 GameObject 初始为激活状态，场景加载时立即调用
        /// - 若通过 Instantiate 动态创建，实例化时立即调用
        /// - 若 GameObject 初始为未激活，则在其首次激活时调用
        /// 
        /// 职责：
        /// 1. 重置所有动画节点的播放进度，确保动画系统从干净状态启动
        /// 2. 触发初始化事件，通知外部系统该动画器已就绪
        /// 
        /// 注意事项：
        /// - 此时 controller（图元控制器）可能尚未绑定，因为绑定逻辑通常在外部
        ///   Element 系统的初始化流程中执行（可参考 Tween_GetController()）
        /// - 此方法仅做状态重置与事件通知，不涉及实际动画创建
        /// </summary>
        void Awake()
        {
            // ========== 1. 重置所有动画节点的播放进度 ==========
            // 遍历 PrimitiveTweenNodes 列表，将每个节点的 Progress 字段归零
            // 意义：
            //   - 确保本次生命周期内动画进度从 0 开始
            //   - 清除对象池复用或编辑器残留的旧进度值
            //   - 避免音效触发点因残留进度误判为已触发
            // 
            // 内部实现（参考 TweenNode_AllTweenProgress_Reset）：
            //   - 若 PrimitiveTweenNodes 为空则直接返回
            //   - 否则遍历所有节点，将 Progress 设置为 0
            //   - 注意：此方法仅重置 Progress 数值，不影响实际动画实例（Tweener）
            TweenNode_AllTweenProgress_Reset();

            // ========== 2. 触发初始化动作委托（代码绑定） ==========
            // 空引用检查：只有外部代码通过 act_on_Tween_Initialized += ... 绑定过回调时才执行
            // 触发时机：动画器首次唤醒、进度重置之后
            // 典型用途：
            //   - 外部系统在动画器可用时进行一次性绑定（如绑定按钮点击、注册监听）
            //   - 初始化与动画相关的 UI 状态（如进度条归零、按钮置灰）
            //   - 记录动画器创建时刻的日志/埋点
            // 
            // 注意：使用 UnityAction 无参委托，因此无参传递
            if (act_on_Tween_Initialized != null)
                act_on_Tween_Initialized();

            // ========== 3. 触发初始化事件（Inspector 可视化绑定） ==========
            // UnityEvent 内部已做空引用保护，无需外部判空
            // 与上一行的委托互为补充：
            //   - act_on_Tween_Initialized：代码动态绑定
            //   - eve_on_Tween_Initialized：Inspector 拖拽绑定
            // 两者可同时存在、同时触发，互不影响
            // 
            // 典型 Inspector 绑定场景：
            //   - 拖拽父级 Element 对象，调用其某个初始化方法
            //   - 拖拽音效管理器，播放"图元入场"音效
            eve_on_Tween_Initialized.Invoke();
        }

        void Start()
        {

        }

        private void OnDestroy()
        {
            Tween_KillAll(false);
        }

        void Update()
        {
            TweenNode_AllTweenProgress_Calculation();
            TweenNode_IsAllAnimating();
        }
        #endregion

        #region 动画节点列表操作
        /// <summary>
        /// 创建动画节点（无参重载）
        /// 
        /// 功能：
        /// 新建一个默认配置的 TweenNode，自动分配唯一 ID，并加入节点列表
        /// 
        /// 执行流程：
        /// 1. new TweenNode()          → 创建默认实例（字段使用 TweenNode 的默认值）
        /// 2. TweenNode_GenerateId()   → 生成不与现有节点冲突的唯一 ID
        /// 3. PrimitiveTweenNodes.Add  → 加入列表，后续可被遍历/播放/复位
        /// 
        /// 使用场景：
        /// - 编辑器点击"新建动画"按钮
        /// - 代码动态追加一条动画轨道
        /// - 从模板复制前先创建空白节点
        /// 
        /// 注意事项：
        /// - 新节点的 Indicator 默认为 "NewTween"，可能与其他节点重名（ID 唯一即可）
        /// - 新节点的 Enabled 默认为 true（具体取决于 TweenNode 字段默认值）
        /// - 新节点的 Tweener 为 null，需调用 Tween_Create 系列方法后才具备播放能力
        /// </summary>
        /// <returns>新创建的 TweenNode 实例（已加入 PrimitiveTweenNodes）</returns>
        public TweenNode TweenNode_Create()
        {
            // 步骤 1：创建默认动画节点实例
            TweenNode tn = new TweenNode();

            // 步骤 2：分配唯一 ID（避免与现有节点冲突）
            tn.ID = TweenNode_GenerateId();

            // 步骤 3：加入节点列表，纳入统一管理
            PrimitiveTweenNodes.Add(tn);

            // 返回引用，便于外部继续配置该节点
            return tn;
        }
        /// <summary>
        /// 创建动画节点（带参重载）
        /// 
        /// 功能：
        /// 将一个已配置好的 TweenNode 实例加入节点列表，并重新分配唯一 ID
        /// 
        /// 与无参重载的区别：
        /// - 无参版：新建空白节点
        /// - 带参版：复用外部已配置好的节点（如从其他来源构造、从文件反序列化）
        /// 
        /// 关键行为：
        /// - 传入节点的原 ID 会被覆盖（因为 ID 必须在本列表中唯一）
        /// - 传入节点本身被直接加入列表（引用添加，非克隆）
        /// 
        /// 使用场景：
        /// - 从预制配置批量导入动画节点
        /// - 代码中手动构造好节点后注册到列表中
        /// - 从其他图元复制动画节点后，重新分配 ID 再加入
        /// 
        /// 注意事项：
        /// - 如果外部希望保留传入节点对象，克隆一份再调用更安全
        /// - 传参为 null 时未做保护，调用方需自行判空
        /// </summary>
        /// <param name="node">已配置好的动画节点（ID 会被重新分配）</param>
        /// <returns>传入的同一个节点实例（ID 已更新）</returns>
        public TweenNode TweenNode_Create(TweenNode node)
        {
            // 覆盖原 ID，确保在本节点列表内唯一
            node.ID = TweenNode_GenerateId();

            // 直接引用加入列表
            PrimitiveTweenNodes.Add(node);

            // 返回传入的实例，便于链式调用
            return node;
        }
        /// <summary>
        /// 移除动画节点（按名称标识 Indicator）
        /// <para/>
        /// 遍历动画节点列表，移除所有 <c>Indicator</c> 与传入名称匹配的节点。
        /// 由于 Indicator 允许重名，此方法会清除所有同名节点。
        /// <para/>
        /// 移除前会先杀死节点上的 Tweener 实例并置空引用，
        /// 避免动画实例脱离列表管理后继续运行（幽灵动画）以及内存泄漏。
        /// </summary>
        /// <param name="name">动画节点的名称标识（Indicator）</param>
        /// <remarks>
        /// <para>匹配规则：<c>Indicator == name</c>。</para>
        /// <para>如需精确移除单个节点，建议改用 <see cref="TweenNode_RemoveById(int)"/>。</para>
        /// </remarks>
        public void TweenNode_RemoveByName(string name)
        {
            // 逆序遍历：删除后不影响未检查元素的索引
            for (int i = PrimitiveTweenNodes.Count - 1; i >= 0; i--)
            {
                if (PrimitiveTweenNodes[i].Indicator == name)
                {
                    // 先杀死动画实例，避免幽灵动画与内存泄漏
                    if (PrimitiveTweenNodes[i].Tweener != null)
                    {
                        PrimitiveTweenNodes[i].Tweener.Kill(false);
                        PrimitiveTweenNodes[i].Tweener = null;
                    }

                    PrimitiveTweenNodes.RemoveAt(i);
                }
            }
        }
        /// <summary>
        /// 移除动画节点（按唯一 ID）
        /// <para/>
        /// 遍历动画节点列表，移除 <c>ID</c> 与传入值匹配的节点。
        /// ID 在本动画器内保证唯一，因此实际至多命中一项。
        /// <para/>
        /// 移除前会先杀死节点上的 Tweener 实例并置空引用，
        /// 避免动画实例脱离列表管理后继续运行（幽灵动画）以及内存泄漏。
        /// </summary>
        /// <param name="id">动画节点的唯一 ID</param>
        /// <remarks>
        /// <para>匹配规则：<c>ID == id</c>。</para>
        /// <para>如需按名称批量移除，请使用 <see cref="TweenNode_RemoveByName(string)"/>。</para>
        /// </remarks>
        public void TweenNode_RemoveById(int id)
        {
            // 逆序遍历：删除后不影响未检查元素的索引
            for (int i = PrimitiveTweenNodes.Count - 1; i >= 0; i--)
            {
                if (PrimitiveTweenNodes[i].ID == id)
                {
                    // 先杀死动画实例，避免幽灵动画与内存泄漏
                    if (PrimitiveTweenNodes[i].Tweener != null)
                    {
                        PrimitiveTweenNodes[i].Tweener.Kill(false);
                        PrimitiveTweenNodes[i].Tweener = null;
                    }

                    PrimitiveTweenNodes.RemoveAt(i);
                }
            }
        }
        /// <summary>
        /// 获取动画节点 - 根据列表索引
        /// 
        /// 功能：
        /// 直接返回 PrimitiveTweenNodes 中指定索引位置的节点
        /// 
        /// 特性：
        /// - O(1) 时间复杂度，最快
        /// - 不做越界检查（越界时由 List 抛出 ArgumentOutOfRangeException）
        /// 
        /// 使用场景：
        /// - 遍历所有节点（配合 Count 使用）
        /// - 已知顺序的场景下快速访问
        /// - 编辑器列表 UI 的行点击定位
        /// 
        /// 注意事项：
        /// - 索引随节点增删而变动，不适合持久化引用
        /// - 调用方需自行保证 index 合法（0 <= index < Count）
        /// </summary>
        /// <param name="index">列表索引</param>
        /// <returns>对应索引的 TweenNode 实例</returns>
        public TweenNode TweenNode_GetByIndex(int index)
        {
            // 直接按索引访问（无判空、无越界保护）
            return PrimitiveTweenNodes[index];
        }
        /// <summary>
        /// 获取动画节点 - 根据唯一 ID
        /// 
        /// 功能：
        /// 线性遍历节点列表，返回第一个 ID 匹配的节点
        /// 
        /// 特性：
        /// - O(n) 时间复杂度
        /// - 找到即 break，平均查找效率约 n/2
        /// - ID 唯一，命中即终止，无需继续遍历
        /// 
        /// 使用场景：
        /// - 外部系统通过保存的 ID 定位节点（推荐做法）
        /// - 动画播放/复位/杀死时按 ID 定位目标
        /// - 复制节点的冲突检测
        /// 
        /// 与按名称获取的差异：
        /// - 按 ID：唯一，可信任结果
        /// - 按名称：可能重名，返回首个匹配项
        /// 
        /// 注意事项：
        /// - 未找到时返回 null，调用方需判空
        /// - ID 是持久化引用的首选标识（不受增删影响）
        /// </summary>
        /// <param name="ID">动画节点的唯一 ID</param>
        /// <returns>匹配的节点，未找到返回 null</returns>
        public TweenNode TweenNode_GetByID(int ID)
        {
            TweenNode node = null;

            // 线性查找
            for (int i = 0; i < PrimitiveTweenNodes.Count; i++)
            {
                if (PrimitiveTweenNodes[i].ID == ID)
                {
                    node = PrimitiveTweenNodes[i];
                    break;  // ID 唯一，命中即可退出
                }
            }

            return node;
        }
        /// <summary>
        /// 获取动画节点 - 根据名称标识 Indicator
        /// 
        /// 功能：
        /// 线性遍历节点列表，返回第一个 Indicator 匹配的节点
        /// 
        /// 特性：
        /// - O(n) 时间复杂度
        /// - 找到即 break，返回首个匹配项
        /// - Indicator 允许重名，若有多个同名节点只返回第一个
        /// 
        /// 使用场景：
        /// - 通过有语义的名称定位节点（如 "FadeIn"、"MoveUp"）
        /// - 上层 Element 系统通过 MainTweenNode 定位主入口动画
        /// - 代码中以名称调用动画的便捷方式
        /// 
        /// 与按 ID 获取的差异：
        /// - ID：唯一，可信
        /// - Indicator：可重名，返回首个匹配，结果可能非预期
        /// 
        /// 注意事项：
        /// - 未找到时返回 null，调用方需判空
        /// - 若列表中可能存在同名节点，建议改用 ID 定位
        /// - 命名建议：项目内保持 Indicator 唯一（如加前缀区分）
        /// </summary>
        /// <param name="indicator">动画节点的名称标识</param>
        /// <returns>首个匹配的节点，未找到返回 null</returns>
        public TweenNode TweenNode_GetByIndicator(string indicator)
        {
            TweenNode node = null;

            // 线性查找
            for (int i = 0; i < PrimitiveTweenNodes.Count; i++)
            {
                if (PrimitiveTweenNodes[i].Indicator == indicator)
                {
                    node = PrimitiveTweenNodes[i];
                    break;  // 命中首个匹配即退出
                }
            }

            return node;
        }
        /// <summary>
        /// 生成动画节点的唯一标识 ID
        /// 
        /// 用于在动画节点列表中区分不同的节点，确保每个节点有独立的标识符。
        /// 
        /// 【工作原理】
        /// 1. 收集当前所有已存在的动画节点 ID，存入临时列表
        /// 2. 在 1111-9999 范围内生成一个随机数
        /// 3. 如果生成的 ID 与已有 ID 冲突，则将范围扩大到 111111-999999 重新生成
        /// 4. 重复直到生成一个不重复的 ID
        /// 
        /// 【为什么需要唯一 ID？】
        /// - 动画节点可能重名（Indicator 可以相同），但 ID 必须唯一
        /// - 外部系统通过 ID 来定位和操作特定的动画节点
        /// - 删除、修改、播放指定动画时都需要通过 ID 来识别目标节点
        /// 
        /// 【使用场景】
        /// - 创建新动画节点时调用，自动分配唯一 ID
        /// - 编辑器中的"重新生成 ID"按钮调用
        /// - 复制/粘贴动画节点时重新生成 ID 避免冲突
        /// 
        /// 【注意事项】
        /// - 随机数范围从 4 位数开始，冲突概率较低
        /// - 如果冲突，自动扩大到 6 位数，基本不会重复
        /// - 理论上在极端情况下（如已有 999999-111111 个节点）可能无限循环
        ///   但实际项目中动画节点数量通常不会超过 100 个，因此安全
        /// 
        /// 【优化建议】
        /// - 可改为使用 GUID 或自增计数器（如 static int _nextId）
        ///   彻底避免冲突，同时省去收集与循环开销
        /// - 当前实现适合节点数量少（< 1000）、创建频率低的场景
        /// </summary>
        /// <returns>一个在当前动画节点列表中不重复的唯一整数 ID</returns>
        public int TweenNode_GenerateId()
        {
            // ========== 步骤 1：收集现有所有 ID ==========
            // 用于后续 Contains 冲突检测
            // 注意：每次调用都重新构建列表，存在重复内存分配
            List<int> ids = new List<int>();
            for (int i = 0; i < PrimitiveTweenNodes.Count; i++)
            {
                ids.Add(PrimitiveTweenNodes[i].ID);
            }

            // ========== 步骤 2：首次尝试（4 位数范围） ==========
            // 1111-9999 共约 8888 个候选项，节点数少时几乎必中
            int ran_id = Random.Range(1111, 9999);

            // ========== 步骤 3：冲突检测循环 ==========
            while (true)
            {
                if (ids.Contains(ran_id))
                {
                    // 冲突：扩大到 6 位数范围重新生成
                    // 111111-999999 共约 888888 个候选项，几乎不会再次冲突
                    ran_id = Random.Range(111111, 999999);
                }
                else
                {
                    // 无冲突：跳出循环
                    break;
                }
            }

            return ran_id;
        }
        #endregion

        #region 获取耗时统计值
        /// <summary>
        /// 在自身寻找图元控制器
        /// 
        /// 功能：
        /// 从当前 GameObject 上获取 XHud_Module_Primitive_Controller 组件，并建立双向绑定关系
        /// 
        /// 执行流程：
        /// 1. 通过 transform.GetComponent 查找同物体上的图元控制器
        /// 2. 若找到，则将控制器的 pt_Tween 反向指向本动画器（建立双向引用）
        /// 
        /// 为什么要双向绑定？
        /// - 动画器需要 controller 提供目标组件（RectTransform / Image / Text / CanvasGroup 等）
        /// - 控制器需要 pt_Tween 反向调用动画器的方法（如播放、复位等）
        /// - 双向引用避免了每次查找组件的开销
        /// 
        /// 使用场景：
        /// - 图元初始化时调用一次
        /// - 通过代码动态创建图元时手动调用
        /// - 编辑器菜单中"刷新引用"时调用
        /// 
        /// 注意事项：
        /// - 若当前物体上不存在 XHud_Module_Primitive_Controller，controller 将保持原值（可能为 null）
        /// - 此方法通常由图元控制器在其初始化流程中调用
        /// - 若已绑定其他控制器，将被覆盖为新的引用
        /// </summary>
        public void Tween_GetController()
        {
            // 在同物体上查找图元控制器组件
            controller = transform.GetComponent<XHud_Module_Primitive_Controller>();

            // 若找到控制器，则建立反向引用，形成双向绑定
            if (controller != null)
            {
                controller.pt_Tween = this;
            }
        }
        /// <summary>
        /// 计算并缓存所有动画节点的耗时统计信息
        /// 
        /// 计算内容包括：
        /// - MinTimer：所有动画节点中的最短耗时（秒）
        /// - MaxTimer：所有动画节点中的最长耗时（秒）
        /// - MinTimerWithGlobalDuration：最短耗时 × 动画器全局速率
        /// - MaxTimerWithGlobalDuration：最长耗时 × 动画器全局速率
        /// 
        /// 耗时计算公式：
        /// 单个动画节点耗时 = Duration × LoopCount + Delay
        /// - Duration：动画单次播放时长
        /// - LoopCount：循环次数（-1 表示无限循环，计算时按 1 处理）
        /// - Delay：动画开始前的延迟时间
        /// 
        /// 使用场景：
        /// - UI 显示动画时长统计信息
        /// - 计算动画系统的最长等待时间（用于协程等待所有动画完成）
        /// - 优化动画调度，预知总播放时长
        /// 
        /// 注意事项：
        /// - 循环次数为 -1（无限循环）时，按 1 次计算，避免无限大
        /// - 结果会缓存在 MinTimer/MaxTimer 字段中，供外部直接读取
        /// - 节点列表变化后需重新调用以刷新缓存
        /// </summary>
        public void TweenNode_GetTimers()
        {
            // 获取所有动画节点中的最短耗时
            // 内部会遍历 PrimitiveTweenNodes 并缓存到 MinTimer 字段
            MinTimer = TweenNode_GetTimerByType("min");

            // 获取所有动画节点中的最长耗时
            // 内部会遍历 PrimitiveTweenNodes 并缓存到 MaxTimer 字段
            MaxTimer = TweenNode_GetTimerByType("max");

            // 应用动画器全局速率后的最短耗时
            // GlobalDuration 为时长倍率：<1 加速、>1 减速
            MinTimerWithGlobalDuration = MinTimer * GlobalDuration;

            // 应用动画器全局速率后的最长耗时
            MaxTimerWithGlobalDuration = MaxTimer * GlobalDuration;
        }
        /// <summary>
        /// 获取动画节点列表中的最大或最小耗时
        /// 
        /// 耗时计算公式：
        /// 单个节点耗时 = Duration × LoopCount + Delay
        /// - 使用 Mathf.Clamp(LoopCount, 1, int.MaxValue) 确保：
        ///   - 如果 LoopCount = 0，按 1 次计算（至少播放一次）
        ///   - 如果 LoopCount = -1（无限循环），按 1 次计算（避免无限大）
        ///   - 如果 LoopCount > 0，正常计算
        /// 
        /// 为什么 LoopCount = -1 时按 1 次计算？
        /// - 无限循环动画理论上没有结束时间
        /// - 在计算最大/最小时，按单次播放时长处理，便于 UI 统计和调度
        /// - 实际播放时会无限循环，不影响运行时行为
        /// 
        /// 性能说明：
        /// - 每次调用都会新建 List 与数组，存在内存分配
        /// - 若频繁调用（如每帧），建议缓存结果或改为无分配算法
        /// 
        /// 使用场景：
        /// - TweenNode_GetTimers() 内部调用
        /// - 外部需要单独获取最大/最小耗时的场景
        /// 
        /// 注意事项：
        /// - type 参数为字符串比较，不区分大小写不生效，必须精确传 "min" / "max"
        /// - 传入非 "min"/"max" 的值会返回 0 且不更新缓存字段
        /// - 节点列表为空时返回 0，并清空对应缓存字段
        /// </summary>
        /// <param name="type">指定获取类型："min" 获取最小耗时，"max" 获取最大耗时</param>
        /// <returns>计算出的耗时值（秒），如果节点列表为空或 type 非法则返回 0</returns>
        public float TweenNode_GetTimerByType(string type)
        {
            // ========== 空列表保护 ==========
            // 节点列表为空时，直接返回 0，并清空对应缓存
            if (PrimitiveTweenNodes.Count <= 0)
            {
                if (type == "min")
                    MinTimer = 0;
                if (type == "max")
                    MaxTimer = 0;
                return 0;
            }

            // ========== 计算每个节点的耗时 ==========
            // 逐个节点计算：Duration × Clamp(LoopCount, 1, int.MaxValue) + Delay
            // - Clamp 确保 LoopCount 至少为 1（避免 0 或负数导致耗时为 0 或负数）
            // - 无限循环（-1）被 Clamp 到 1，按单次播放时长计算
            List<float> timers = new List<float>();
            for (int i = 0; i < PrimitiveTweenNodes.Count; i++)
            {
                float time = PrimitiveTweenNodes[i].Duration * Mathf.Clamp(PrimitiveTweenNodes[i].LoopCount, 1, int.MaxValue) + PrimitiveTweenNodes[i].Delay;
                timers.Add(time);
            }

            // ========== 按类型返回结果并更新缓存 ==========
            if (type == "min")
            {
                // 取所有节点耗时中的最小值
                MinTimer = XGUI_Utilitys.MinValue(timers.ToArray());
                return MinTimer;
            }
            else if (type == "max")
            {
                // 取所有节点耗时中的最大值
                MaxTimer = XGUI_Utilitys.MaxValue(timers.ToArray());
                return MaxTimer;
            }
            else
            {
                // type 非法：不更新缓存，返回 0
                return 0;
            }
        }
        /// <summary>
        /// 检测动画器是否正在播放动画
        /// 
        /// 工作原理：
        /// 1. 遍历所有动画节点（PrimitiveTweenNodes）
        /// 2. 检查每个节点的 Tweener 是否存在、是否激活、是否正在播放
        /// 3. 只要有一个节点正在播放，就返回 true
        /// 4. 当所有节点都停止后，触发动画完成事件（act_on_Tween_Complete / eve_on_Tween_Complete）
        /// 
        /// 状态机逻辑：
        /// - AnimatingBreakState 字段用于记录动画是否正在播放
        /// - 当从"播放中"变为"已停止"时，触发完成事件
        /// - 这种设计确保完成事件只在状态切换时触发一次，而不是每帧都触发
        /// 
        /// 事件触发时机：
        /// - act_on_Tween_IsAnimating：每帧都会触发，传递当前是否在播放的状态
        /// - eve_on_Tween_AnimatingState：每帧都会触发，与上面委托互补（Inspector 绑定）
        /// - act_on_Tween_Complete：只在动画从播放变为停止时触发一次
        /// - eve_on_Tween_Complete：同上，Inspector 绑定版本
        /// 
        /// 使用场景：
        /// - 外部系统判断动画是否还在播放（如等待所有动画完成）
        /// - UI 状态机管理（动画播放期间禁用交互）
        /// - 链式动画播放（一个动画结束后播放下一个）
        /// 
        /// 性能考虑：
        /// - 每帧在 Update 中调用，开销极小
        /// - 仅遍历动画节点列表，检查 Tweener 状态
        /// - 只要发现一个在播放即 break，平均开销远小于全遍历
        /// 
        /// 注意事项：
        /// - 若节点 Tweener 为 null（未创建动画），会被跳过
        /// - 若 Tweener.IsActive 为 false（被杀死），会被跳过
        /// - Complete 事件仅在"有 → 无"播放状态的边缘触发，不会重复触发
        /// </summary>
        /// <returns>true 表示至少有一个动画正在播放，false 表示所有动画都已停止</returns>
        public bool TweenNode_IsAllAnimating()
        {
            bool animating = false;

            // ========== 1. 检查是否有动画正在播放 ==========
            // 遍历所有节点，只要发现一个在播放即可提前退出
            if (PrimitiveTweenNodes != null && PrimitiveTweenNodes.Count > 0)
            {
                for (int i = 0; i < PrimitiveTweenNodes.Count; i++)
                {
                    TweenNode node = PrimitiveTweenNodes[i];

                    // 三重条件判定该节点正在播放：
                    // - Tweener != null  ：动画实例已创建
                    // - Tweener.IsActive ：动画未被杀死（生命周期有效）
                    // - Tweener.IsPlaying：动画处于播放状态
                    if (node.Tweener != null && node.Tweener.IsActive && node.Tweener.IsPlaying)
                    {
                        animating = true;
                        break;  // 只要有一个在播放，就可以提前退出循环
                    }
                }
            }

            // ========== 2. 触发动画播放状态事件（每帧） ==========
            // 委托版本（代码绑定）
            if (act_on_Tween_IsAnimating != null)
                act_on_Tween_IsAnimating(animating);

            // 事件版本（Inspector 绑定，内部已判空）
            eve_on_Tween_AnimatingState.Invoke(animating);

            // ========== 3. 边缘检测：动画完成时触发完成事件 ==========
            if (animating)
            {
                // 当前正在播放：若之前是停止状态，则标记为"播放中"
                // 这是"停止 → 播放"的上升沿，不需要触发完成事件
                if (!AnimatingBreakState)
                {
                    AnimatingBreakState = true;
                }
            }
            else
            {
                // 当前已停止：若之前是播放状态，则触发完成事件
                // 这是"播放 → 停止"的下降沿，需要触发一次完成事件
                if (AnimatingBreakState)
                {
                    AnimatingBreakState = false;  // 重置标志位，避免重复触发

                    // 触发动画完成动作（代码绑定）
                    if (act_on_Tween_Complete != null)
                        act_on_Tween_Complete();

                    // 触发动画完成事件（Inspector 绑定）
                    eve_on_Tween_Complete.Invoke();
                }
            }

            return animating;
        }
        #endregion

        #region 所有动画进度计算
        /// <summary>
        /// 计算所有动画节点的播放进度，并在达到触发点时播放音效
        /// 
        /// 功能概述：
        /// 1. 遍历所有动画节点，更新每个节点的播放进度（0-1）
        /// 2. 检查是否达到音效触发点，达到则播放对应的音效
        /// 
        /// 调用频率：
        /// - 每帧在 Update 中调用
        /// - 开销与动画节点数量和音效数量成正比
        /// 
        /// </summary>
        private void TweenNode_AllTweenProgress_Calculation()
        {
            // 安全检查：没有动画节点时直接返回
            if (PrimitiveTweenNodes == null || PrimitiveTweenNodes.Count == 0)
                return;

            // 遍历所有动画节点
            for (int i = 0; i < PrimitiveTweenNodes.Count; i++)
            {
                TweenNode node = PrimitiveTweenNodes[i];

                // 更新该节点的播放进度
                TweenNode_UpdateTweenProgress(node);
            }
        }
        /// <summary>
        /// 重置所有动画节点的播放进度
        /// 将所有动画节点的 Progress 属性归零，表示动画尚未开始或已回退到起始状态
        /// 
        /// 工作原理：
        /// 1. 检查动画节点列表是否为空，为空则直接返回
        /// 2. 遍历所有动画节点
        /// 3. 将每个节点的 Progress 值设置为 0
        /// 
        /// Progress 属性的含义：
        /// - 0：动画尚未开始，或已完全回退到起始状态
        /// - 0.5：动画播放到一半
        /// - 1：动画已完成
        /// 
        /// 使用场景：
        /// - 动画器初始化时（Awake），确保所有节点从 0 开始
        /// - 停止预览动画时，重置进度状态
        /// - 重新播放动画前，清空旧的进度记录
        /// - 元素从对象池回收时，重置动画状态
        /// 
        /// 注意事项：
        /// - 此方法仅重置 Progress 数值，不会影响实际的动画播放器（Tweener）
        /// - 实际的动画回退需要调用 Tweener_Rewind 方法
        /// - Progress 主要用于 UI 显示（如进度条）和音效触发判断
        /// </summary>
        public void TweenNode_AllTweenProgress_Reset()
        {
            if (PrimitiveTweenNodes.Count <= 0)
                return;
            for (int i = 0; i < PrimitiveTweenNodes.Count; i++)
            {
                #region 确保动画存在不为空的情况下，每个动画的动画进度归零
                PrimitiveTweenNodes[i].Progress = 0;
                #endregion
            }
        }
        /// <summary>
        /// 更新单个动画节点的播放进度
        /// 
        /// 进度计算方式：
        /// - 进度 = 已播放时间 / 总时长
        /// - 返回值范围：0（未开始）~ 1（已完成）
        /// 
        /// 精度修正说明：
        /// - 浮点数计算可能导致进度无限接近但无法等于 1（如 0.99999994）
        /// - 当进度 > 0.985f 时，直接设为 1，确保完成状态被正确识别
        /// 
        /// </summary>
        /// <param name="node">目标动画节点</param>
        private void TweenNode_UpdateTweenProgress(TweenNode node)
        {
            // 检查动画是否正在播放
            if (node.Tweener == null || !node.Tweener.IsActive || !node.Tweener.IsPlaying)
                return;

            // 计算原始进度值
            float rawProgress = node.Tweener.ElapsedTime / node.Tweener.Duration;

            // 精度修正：接近完成时直接设为 1
            node.Progress = rawProgress > 0.985f ? 1f : rawProgress;
        }
        #endregion

        #region 音效播放
        /// <summary>
        /// 播放动画音效
        /// </summary>
        /// <param name="clip">要播放的音频剪辑</param>
        /// <param name="volume">音量系数（0-1），最终音量 = 系统音量 × 系数</param>
        /// <param name="pitchMin">最小音高（随机范围下限）</param>
        /// <param name="pitchMax">最大音高（随机范围上限）</param>
        /// <returns>播放音效的 AudioSource，如果播放失败则返回 null</returns>
        private AudioSource PlayTweenSound(AudioClip clip, float volume, float pitchMin, float pitchMax)
        {
            // ========== 1. 静音检查 ==========
            if (MutePlay)
                return null;

            // ========== 2. 音频剪辑有效性检查 ==========
            if (clip == null)
            {
                UnityEngine.Debug.LogWarning("PlayTweenSound: 音频剪辑为空，无法播放");
                return null;
            }

            // ========== 3. 触发外部事件 ==========
            act_on_Tween_SoundPlay?.Invoke(clip);
            eve_on_Tween_SoundPlay.Invoke(clip);

            // ========== 4. 获取音效播放器 ==========
            XHud_Manager manager = XHud_Manager.Instance;
            if (manager == null)
            {
                UnityEngine.Debug.LogWarning("PlayTweenSound: XHud_Manager 实例不存在，无法播放音效");
                return null;
            }

            AudioSource player = manager.hm_LibrarySounds_GetSounder();
            if (player == null)
            {
                UnityEngine.Debug.LogWarning("PlayTweenSound: 音效池中没有可用的 AudioSource");
                return null;
            }

            // ========== 5. 配置并播放音效 ==========
            player.clip = clip;
            player.volume = manager.Volume * 0.01f * Mathf.Clamp01(volume);
            player.mute = manager.VolumeMute;
            player.pitch = Random.Range(pitchMin, pitchMax);
            player.Play();

            return player;
        }
        #endregion

        #region 动画创建
        /// <summary>
        /// 动画创建总入口（工厂方法）
        /// 
        /// 根据 TweenNode.Type 分派到对应的专用创建方法，统一处理前置状态复位与参数转发。
        /// 
        /// 【四种数值模式说明】
        ///     模式                        |  使用场景
        ///     ----------------------------|--------------------------------
        ///     起始 → 默认  (S - D)        |  动画需要回退到原始状态
        ///     默认 → 结束  (D - E)        |  动画只改变到新状态，不回退
        ///     起始 → 结束  (S - E)        |  精确控制的往返动画
        ///     当前 → 结束  (C - E)        |  独立动画节点，支持中断
        /// 
        /// 【前置处理】
        /// - TweenValueMode != 当前到结束_C_E → 先 Tween_Rewind(node, true) 复位到起始状态
        /// - TweenValueMode == 当前到结束_C_E → 先 Tween_Kill(node, true) 杀死旧动画实例
        /// 目的：避免旧动画残留状态污染新动画
        /// 
        /// 【返回值】
        /// - 成功：对应的 XTween_Interface 实例
        /// - 失败：null（如目标组件缺失、被配色器接管等）
        /// 
        /// 【注意事项】
        /// - 调用方通常需要将返回值赋给 node.Tweener 以保持引用
        /// - 打字机动画会根据 Text / TMP_Text 组件自动选择实现
        /// </summary>
        /// <param name="node">目标动画节点（配置来源）</param>
        /// <param name="duration">时长倍率（与 node.Duration 相乘得到最终时长）</param>
        /// <param name="delay">额外延时（叠加到 node.Delay 之上）</param>
        /// <param name="percentage_threshold">进度阈值（0-1），达到后触发 on_percentage 一次</param>
        /// <param name="on_update">每帧更新回调</param>
        /// <param name="on_percentage">进度达到阈值时触发一次的回调</param>
        /// <param name="on_complete">动画完成时的回调</param>
        /// <param name="on_rewind">动画回退时的回调</param>
        /// <returns>创建成功的动画实例，或 null</returns>
        public XTween_Interface Tween_Create(TweenNode node, float duration, float delay = 0, float percentage_threshold = 0.5f, UnityAction on_update = null, UnityAction on_percentage = null, UnityAction on_complete = null, UnityAction on_rewind = null)
        {
            XTween_Interface twn = null;

            // ========== 前置状态处理 ==========
            // 非"仅到结束"模式：先复位到起始值，避免旧动画残留
            // "仅到结束"模式：直接杀死旧动画，无需复位（因为不复位也无意义）
            if (node.TweenValueMode != TweenValueMode.当前到结束_C_E)
            {
                Tween_Rewind(node, true);
            }
            else
            {
                Tween_Kill(node, true);
            }

            // ========== 按类型分派 ==========
            switch (node.Type)
            {
                case TweenNodeType.a_位移:
                    twn = Tween_Create_Position(node, duration, delay, percentage_threshold, on_update, on_percentage, on_complete, on_rewind);
                    break;
                case TweenNodeType.r_旋转:
                    twn = Tween_Create_Rotate(node, duration, delay, percentage_threshold, on_update, on_percentage, on_complete, on_rewind);
                    break;
                case TweenNodeType.s_缩放:
                    twn = Tween_Create_Scale(node, duration, delay, percentage_threshold, on_update, on_percentage, on_complete, on_rewind);
                    break;
                case TweenNodeType.c_颜色:
                    twn = Tween_Create_Color(node, duration, delay, percentage_threshold, on_update, on_percentage, on_complete, on_rewind);
                    break;
                case TweenNodeType.g_淡化:
                    twn = Tween_Create_Fader(node, duration, delay, percentage_threshold, on_update, on_percentage, on_complete, on_rewind);
                    break;
                case TweenNodeType.w_打字机:
                    // 打字机动画根据实际文本组件类型自动选择实现
                    if (controller.mod_Text != null)
                    {
                        twn = Tween_Create_Writer_Text(node, duration, delay, percentage_threshold, on_update, on_percentage, on_complete, on_rewind);
                    }
                    else if (controller.mod_TmpText != null)
                    {
                        twn = Tween_Create_Writer_TmpText(node, duration, delay, percentage_threshold, on_update, on_percentage, on_complete, on_rewind);
                    }
                    break;
                case TweenNodeType.z_尺寸:
                    twn = Tween_Create_Size(node, duration, delay, percentage_threshold, on_update, on_percentage, on_complete, on_rewind);
                    break;
                case TweenNodeType.f_图像填充:
                    twn = Tween_Create_Fill(node, duration, delay, percentage_threshold, on_update, on_percentage, on_complete, on_rewind);
                    break;
            }

            return twn;
        }
        /// <summary>
        /// 创建位移动画（anchoredPosition3D）
        /// 
        /// 【依赖组件】controller.mod_Rect（RectTransform）
        /// 
        /// 【数值模式处理】
        /// - S_D（起始→默认）：目标值 = Original_Vector3，SetFrom(From_Vector3)
        /// - S_E（起始→结束）：目标值 = End_Vector3，SetFrom(From_Vector3)
        /// - D_E（默认→结束）：目标值 = End_Vector3，不 SetFrom
        /// - C_E（当前→结束）：目标值 = End_Vector3，不 SetFrom
        /// 
        /// 【回调语义】
        /// - OnUpdate：更新 node.Progress，判定阈值触发 on_percentage，转发 on_update
        ///   并触发 node.Act_On_Vector3_Changed（携带当前位置值）
        /// - OnRewind：转发 on_rewind，并触发 node.Act_On_Vector3_Rewind
        /// - OnComplete：重置 Progress = 0，转发 on_complete
        /// 
        /// 【sw 标志】
        /// 用于保证 on_percentage 在整段动画中只触发一次
        /// </summary>
        /// <returns>XTween_Interface 实例；若 mod_Rect 为 null 则返回 null</returns>
        private XTween_Interface Tween_Create_Position(TweenNode node, float duration, float delay, float percentage_threshold, UnityAction on_update = null, UnityAction on_percentage = null, UnityAction on_complete = null, UnityAction on_rewind = null)
        {
            // 前置校验：缺少 RectTransform 则无法创建位移动画
            if (controller.mod_Rect == null)
                return null;

            XTween_Interface twn = null;
            bool sw = false;  // on_percentage 单次触发标志

            // ========== 按数值模式解析目标值与是否 SetFrom ==========
            Vector3 target_value;
            bool isfrom;
            switch (node.TweenValueMode)
            {
                case TweenValueMode.起始到默认_S_D:
                    target_value = node.Original_Vector3;
                    isfrom = true;
                    break;
                case TweenValueMode.起始到结束_S_E:
                    target_value = node.End_Vector3;
                    isfrom = true;
                    break;
                case TweenValueMode.默认到结束_D_E:
                    target_value = node.End_Vector3;
                    isfrom = false;
                    break;
                case TweenValueMode.当前到结束_C_E:
                    target_value = node.End_Vector3;
                    isfrom = false;
                    break;
                default:
                    target_value = node.End_Vector3;
                    isfrom = false;
                    break;
            }

            // ========== 创建底层动画 ==========
            // xt_AnchoredPosition3D_To 参数说明：
            //   target_value            : 目标位置
            //   node.Duration * duration: 最终时长（节点时长 × 传入倍率）
            //   false                   : 不立即播放（由外部决定何时 Play）
            //   true                    : 使用局部坐标系
            //   Rewind_Set_Startvalue   : 回退时是否重置到起始值
            //   Complete_Set_Endvalue   : 完成时是否设为结束值
            twn = controller.mod_Rect.xt_AnchoredPosition3D_To(
                target_value,
                node.Duration * duration,
                false,
                true,
                node.Rewind_Set_Startvalue,
                node.Complete_Set_Endvalue)
                .SetDelay(node.Delay + delay)
                .SetLoop(node.LoopCount, node.LoopType);

            // 仅在需要 From 时设置起始值
            // S-* 模式：静态起点
            if (isfrom)
                twn = twn.SetFrom(node.From_Vector3);

            // C-E 模式：动态起点
            if (node.TweenValueMode == TweenValueMode.当前到结束_C_E)
                twn = twn.SetFromDynamic(() => controller.mod_Rect.anchoredPosition3D);

            // ========== 缓动配置 ==========
            // Ease == None 时使用自定义曲线，否则使用预设缓动模式
            if (node.Ease == EaseMode.None)
                twn = twn.SetEase(node.Curve);
            else
                twn = twn.SetEase(node.Ease);

            // ========== 更新回调 ==========
            twn = twn.OnUpdate<Vector3>((v, d, t) =>
            {
                // 实时写入进度，供音效触发与外部读取
                node.Progress = twn.ElapsedTime / twn.Duration;

                // 阈值单次触发
                if (twn.CurrentEasedProgress >= percentage_threshold)
                {
                    if (!sw)
                    {
                        sw = true;
                        if (on_percentage != null)
                            on_percentage();
                    }
                }

                if (on_update != null)
                    on_update();

                // 触发节点自身的值变化回调（携带实际位置）
                if (node.Act_On_Vector3_Changed != null)
                    node.Act_On_Vector3_Changed(controller.mod_Rect.anchoredPosition3D);
            });

            // ========== 回退回调 ==========
            twn = twn.OnRewind(() =>
            {
                if (on_rewind != null)
                    on_rewind();

                if (node.Act_On_Vector3_Rewind != null)
                    node.Act_On_Vector3_Rewind(controller.mod_Rect.anchoredPosition3D);
            });

            // ========== 完成回调 ==========
            twn = twn.OnComplete((d) =>
            {
                node.Progress = 0;  // 完成后重置进度（避免残留）
                if (on_complete != null)
                    on_complete();
            });

            return twn;
        }
        /// <summary>
        /// 创建旋转动画（RectTransform.rotation）
        /// 
        /// 【依赖组件】controller.mod_Rect
        /// 
        /// 【数值模式】与位移一致，但使用 Vector3 作为欧拉角输入
        /// 
        /// 【旋转空间】固定使用世界坐标（XTweenRotationSpace.世界坐标）
        /// 【旋转模式】由 node.RotateMode 决定（Normal / LocalAxis 等）
        /// 
        /// 【回调携带值】Act_On_Quaternion_Changed/Rewind 携带 Quaternion 值
        /// </summary>
        private XTween_Interface Tween_Create_Rotate(TweenNode node, float duration, float delay, float percentage_threshold, UnityAction on_update = null, UnityAction on_percentage = null, UnityAction on_complete = null, UnityAction on_rewind = null)
        {
            if (controller.mod_Rect == null)
                return null;

            XTween_Interface twn = null;
            bool sw = false;

            Vector3 target_value;
            bool isfrom;
            switch (node.TweenValueMode)
            {
                case TweenValueMode.起始到默认_S_D:
                    target_value = node.Original_Vector3;
                    isfrom = true;
                    break;
                case TweenValueMode.起始到结束_S_E:
                    target_value = node.End_Vector3;
                    isfrom = true;
                    break;
                case TweenValueMode.默认到结束_D_E:
                    target_value = node.End_Vector3;
                    isfrom = false;
                    break;
                case TweenValueMode.当前到结束_C_E:
                    target_value = node.End_Vector3;
                    isfrom = false;
                    break;
                default:
                    target_value = node.End_Vector3;
                    isfrom = false;
                    break;
            }

            // xt_Rotate_To 参数：目标欧拉角、时长、是否立即播放、是否使用局部、旋转空间、旋转模式
            twn = controller.mod_Rect.xt_Rotate_To(
                target_value,
                node.Duration * duration,
                false,
                true,
                XTweenRotationSpace.世界坐标,
                node.RotateMode)
                .SetDelay(node.Delay + delay)
                .SetLoop(node.LoopCount, node.LoopType);

            // 仅在需要 From 时设置起始值
            // S-* 模式：静态起点
            if (isfrom)
                twn = twn.SetFrom(node.From_Vector3);

            // C-E 模式：动态起点
            if (node.TweenValueMode == TweenValueMode.当前到结束_C_E)
                twn = twn.SetFromDynamic(() => controller.mod_Rect.localEulerAngles);

            if (node.Ease == EaseMode.None)
                twn = twn.SetEase(node.Curve);
            else
                twn = twn.SetEase(node.Ease);

            twn = twn.OnUpdate<Vector3>((v, d, t) =>
            {
                node.Progress = twn.ElapsedTime / twn.Duration;

                if (twn.CurrentEasedProgress >= percentage_threshold)
                {
                    if (!sw)
                    {
                        sw = true;
                        if (on_percentage != null)
                            on_percentage();
                    }
                }

                if (on_update != null)
                    on_update();

                // 旋转动画回调携带四元数
                if (node.Act_On_Quaternion_Changed != null)
                    node.Act_On_Quaternion_Changed(controller.mod_Rect.rotation);
            });

            twn = twn.OnRewind(() =>
            {
                if (on_rewind != null)
                    on_rewind();

                if (node.Act_On_Quaternion_Rewind != null)
                    node.Act_On_Quaternion_Rewind(controller.mod_Rect.rotation);
            });

            twn = twn.OnComplete((d) =>
            {
                node.Progress = 0;
                if (on_complete != null)
                    on_complete();
            });

            return twn;
        }
        /// <summary>
        /// 创建缩放动画（RectTransform.localScale）
        /// 
        /// 【依赖组件】controller.mod_Rect
        /// 【数值模式】与其他 Vector3 类型动画一致
        /// 【回调携带值】Act_On_Vector3_Changed/Rewind 携带 localScale 值
        /// </summary>
        private XTween_Interface Tween_Create_Scale(TweenNode node, float duration, float delay, float percentage_threshold, UnityAction on_update = null, UnityAction on_percentage = null, UnityAction on_complete = null, UnityAction on_rewind = null)
        {
            if (controller.mod_Rect == null)
                return null;

            XTween_Interface twn = null;
            bool sw = false;

            Vector3 target_value;
            bool isfrom;
            switch (node.TweenValueMode)
            {
                case TweenValueMode.起始到默认_S_D:
                    target_value = node.Original_Vector3;
                    isfrom = true;
                    break;
                case TweenValueMode.起始到结束_S_E:
                    target_value = node.End_Vector3;
                    isfrom = true;
                    break;
                case TweenValueMode.默认到结束_D_E:
                    target_value = node.End_Vector3;
                    isfrom = false;
                    break;
                case TweenValueMode.当前到结束_C_E:
                    target_value = node.End_Vector3;
                    isfrom = false;
                    break;
                default:
                    target_value = node.End_Vector3;
                    isfrom = false;
                    break;
            }

            twn = controller.mod_Rect.xt_Scale_To(
                target_value,
                node.Duration * duration,
                false,
                true,
                node.Rewind_Set_Startvalue,
                node.Complete_Set_Endvalue)
                .SetDelay(node.Delay + delay)
                .SetLoop(node.LoopCount, node.LoopType);

            // 仅在需要 From 时设置起始值
            // S-* 模式：静态起点
            if (isfrom)
                twn = twn.SetFrom(node.From_Vector3);

            // C-E 模式：动态起点
            if (node.TweenValueMode == TweenValueMode.当前到结束_C_E)
                twn = twn.SetFromDynamic(() => controller.mod_Rect.localScale);

            if (node.Ease == EaseMode.None)
                twn = twn.SetEase(node.Curve);
            else
                twn = twn.SetEase(node.Ease);

            twn = twn.OnUpdate<Vector3>((v, d, t) =>
            {
                node.Progress = twn.ElapsedTime / twn.Duration;

                if (twn.CurrentEasedProgress >= percentage_threshold)
                {
                    if (!sw)
                    {
                        sw = true;
                        if (on_percentage != null)
                            on_percentage();
                    }
                }

                if (on_update != null)
                    on_update();

                if (node.Act_On_Vector3_Changed != null)
                    node.Act_On_Vector3_Changed(controller.mod_Rect.localScale);
            });

            twn = twn.OnRewind(() =>
            {
                if (on_rewind != null)
                    on_rewind();

                if (node.Act_On_Vector3_Rewind != null)
                    node.Act_On_Vector3_Rewind(controller.mod_Rect.localScale);
            });

            twn = twn.OnComplete((d) =>
            {
                node.Progress = 0;
                if (on_complete != null)
                    on_complete();
            });

            return twn;
        }
        /// <summary>
        /// 创建颜色动画（Graphic.color）
        /// 
        /// 【依赖组件】通过 controller.RecognizeType() 获取 Graphic（Image / RawImage / Text 等）
        /// 
        /// 【前置校验（重要）】
        /// 1. 文本库同步样式检查：若 Text 或 TMP_Text 处于"库同步样式"且启用颜色效果，则禁止动画
        /// 2. 配色器接管检查：若 Image / RawImage 处于"同步库颜色"接管状态，则禁止动画
        ///    （接管状态下颜色由配色系统控制，动画会冲突）
        /// 3. Graphic 组件存在性检查
        /// 
        /// 【为什么需要这些校验？】
        /// 颜色可能被上层样式系统或配色系统接管，此时强制播放颜色动画会导致：
        /// - 数值被覆盖（动画与接管系统互相拉扯）
        /// - 视觉闪烁或错乱
        /// - 同步状态被破坏
        /// 
        /// 【使用建议】
        /// 需要播放颜色动画前，先关闭接管状态，动画结束后再恢复
        /// </summary>
        private XTween_Interface Tween_Create_Color(TweenNode node, float duration, float delay, float percentage_threshold, UnityAction on_update = null, UnityAction on_percentage = null, UnityAction on_complete = null, UnityAction on_rewind = null)
        {
            #region 判断文字组件是否为库同步样式状态
            bool IsTextColorMode = false;
            if ((controller.mod_Text && controller.mod_Text.StyleLibSynching &&
                controller.mod_Text.TextStyleInfo.LibStyle_Effect_color) ||
                (controller.mod_TmpText && controller.mod_TmpText.StyleLibSynching &&
                controller.mod_TmpText.TextStyleInfo.LibStyle_Effect_color))
            {
                IsTextColorMode = true;
            }
            if (IsTextColorMode)
                return null;
            #endregion

            // 配色器接管状态检查：接管状态下无法通过动画改变颜色
            if (controller.GetModuleType() == ModuleType.Image && controller.pt_Painting.SyncLibraryColor)
                return null;
            if (controller.GetModuleType() == ModuleType.RawImage && controller.pt_Painting.SyncLibraryColor)
                return null;

            Graphic gc = controller.RecognizeType();
            if (gc == null)
                return null;

            XTween_Interface twn = null;
            bool sw = false;

            Color target_value;
            bool isfrom;
            switch (node.TweenValueMode)
            {
                case TweenValueMode.起始到默认_S_D:
                    target_value = node.Original_Color;
                    isfrom = true;
                    break;
                case TweenValueMode.起始到结束_S_E:
                    target_value = node.End_Color;
                    isfrom = true;
                    break;
                case TweenValueMode.默认到结束_D_E:
                    target_value = node.End_Color;
                    isfrom = false;
                    break;
                case TweenValueMode.当前到结束_C_E:
                    target_value = node.End_Color;
                    isfrom = false;
                    break;
                default:
                    target_value = node.End_Color;
                    isfrom = false;
                    break;
            }

            // xt_Color_To 参数：目标颜色、时长、是否立即播放、回退重置、完成设置
            twn = gc.xt_Color_To(
                target_value,
                node.Duration * duration,
                true,
                node.Rewind_Set_Startvalue,
                node.Complete_Set_Endvalue)
                .SetDelay(node.Delay + delay)
                .SetLoop(node.LoopCount, node.LoopType);

            // 仅在需要 From 时设置起始值
            // S-* 模式：静态起点
            if (isfrom)
                twn = twn.SetFrom(node.From_Color);

            // C-E 模式：动态起点
            if (node.TweenValueMode == TweenValueMode.当前到结束_C_E)
                twn = twn.SetFromDynamic(() => gc.color);

            if (node.Ease == EaseMode.None)
                twn = twn.SetEase(node.Curve);
            else
                twn = twn.SetEase(node.Ease);

            twn = twn.OnUpdate<Color>((v, d, t) =>
            {
                node.Progress = twn.ElapsedTime / twn.Duration;

                if (twn.CurrentEasedProgress >= percentage_threshold)
                {
                    if (!sw)
                    {
                        sw = true;
                        if (on_percentage != null)
                            on_percentage();
                    }
                }

                if (on_update != null)
                    on_update();

                if (node.Act_On_Color_Changed != null)
                    node.Act_On_Color_Changed(gc.color);
            });

            twn = twn.OnRewind(() =>
            {
                if (on_rewind != null)
                    on_rewind();

                if (node.Act_On_Color_Rewind != null)
                    node.Act_On_Color_Rewind(gc.color);
            });

            twn = twn.OnComplete((d) =>
            {
                node.Progress = 0;
                if (on_complete != null)
                    on_complete();
            });

            return twn;
        }
        /// <summary>
        /// 创建淡化动画（CanvasGroup.alpha）
        /// 
        /// 【依赖组件】controller.mod_CanvasGroup
        /// 【数值类型】float（0-1）
        /// 【回调携带值】Act_On_Float_Changed/Rewind 携带 alpha 值
        /// 
        /// 【说明】
        /// 淡化本质上是 CanvasGroup 的 alpha 插值，适用于整体透明度的渐变控制
        /// 与颜色动画的区别：颜色动画作用于 Graphic 的 color.a，会影响子物体继承叠加
        /// </summary>
        private XTween_Interface Tween_Create_Fader(TweenNode node, float duration, float delay, float percentage_threshold, UnityAction on_update = null, UnityAction on_percentage = null, UnityAction on_complete = null, UnityAction on_rewind = null)
        {
            if (controller.mod_CanvasGroup == null)
                return null;

            XTween_Interface twn = null;
            bool sw = false;

            float target_value;
            bool isfrom;
            switch (node.TweenValueMode)
            {
                case TweenValueMode.起始到默认_S_D:
                    target_value = node.Original_Float;
                    isfrom = true;
                    break;
                case TweenValueMode.起始到结束_S_E:
                    target_value = node.End_Float;
                    isfrom = true;
                    break;
                case TweenValueMode.默认到结束_D_E:
                    target_value = node.End_Float;
                    isfrom = false;
                    break;
                case TweenValueMode.当前到结束_C_E:
                    target_value = node.End_Float;
                    isfrom = false;
                    break;
                default:
                    target_value = node.End_Float;
                    isfrom = false;
                    break;
            }

            twn = controller.mod_CanvasGroup.xt_Alpha_To(
                target_value,
                node.Duration * duration,
                true,
                node.Rewind_Set_Startvalue,
                node.Complete_Set_Endvalue)
                .SetDelay(node.Delay + delay)
                .SetLoop(node.LoopCount, node.LoopType);

            // 仅在需要 From 时设置起始值
            // S-* 模式：静态起点
            if (isfrom)
                twn = twn.SetFrom(node.From_Float);

            // C-E 模式：动态起点
            if (node.TweenValueMode == TweenValueMode.当前到结束_C_E)
                twn = twn.SetFromDynamic(() => controller.mod_CanvasGroup.alpha);

            if (node.Ease == EaseMode.None)
                twn = twn.SetEase(node.Curve);
            else
                twn = twn.SetEase(node.Ease);

            twn = twn.OnUpdate<float>((v, d, t) =>
            {
                node.Progress = twn.ElapsedTime / twn.Duration;

                if (twn.CurrentEasedProgress >= percentage_threshold)
                {
                    if (!sw)
                    {
                        sw = true;
                        if (on_percentage != null)
                            on_percentage();
                    }
                }

                if (on_update != null)
                    on_update();

                if (node.Act_On_Float_Changed != null)
                    node.Act_On_Float_Changed(controller.mod_CanvasGroup.alpha);
            });

            twn = twn.OnRewind(() =>
            {
                if (on_rewind != null)
                    on_rewind();

                if (node.Act_On_Float_Rewind != null)
                    node.Act_On_Float_Rewind(controller.mod_CanvasGroup.alpha);
            });

            twn = twn.OnComplete((d) =>
            {
                node.Progress = 0;
                if (on_complete != null)
                    on_complete();
            });

            return twn;
        }
        /// <summary>
        /// 创建打字机动画（UGUI Text 版本）
        /// 
        /// 【依赖组件】controller.mod_Text（UnityEngine.UI.Text）
        /// 
        /// 【专属参数】
        /// - node.TextCursor           : 光标字符（如 "|"）
        /// - node.TextCursorBlinkSpeed : 光标闪烁速度；<= 0 时默认 0.5f
        /// 
        /// 【底层调用】
        /// xt_Text_To(false, cursor, target, duration, true, blinkSpeed, rewind, complete)
        /// - 第 1 个 false  : 不立即播放（延迟由外部决定）
        /// - 第 2 个参数    : 光标字符
        /// - 第 3 个参数    : 目标文本
        /// - 第 5 个参数 true: 启用光标闪烁
        /// 
        /// 【回调携带值】Act_On_Text_Changed/Rewind 携带当前完整文本内容
        /// </summary>
        private XTween_Interface Tween_Create_Writer_Text(TweenNode node, float duration, float delay, float percentage_threshold, UnityAction on_update = null, UnityAction on_percentage = null, UnityAction on_complete = null, UnityAction on_rewind = null)
        {
            if (controller.mod_Text == null)
            {
                return null;
            }

            XTween_Interface twn = null;
            bool sw = false;

            string target_value;
            bool isfrom;
            switch (node.TweenValueMode)
            {
                case TweenValueMode.起始到默认_S_D:
                    target_value = node.Original_String;
                    isfrom = true;
                    break;
                case TweenValueMode.起始到结束_S_E:
                    target_value = node.End_String;
                    isfrom = true;
                    break;
                case TweenValueMode.默认到结束_D_E:
                    target_value = node.End_String;
                    isfrom = false;
                    break;
                case TweenValueMode.当前到结束_C_E:
                    target_value = node.End_String;
                    isfrom = false;
                    break;
                default:
                    target_value = node.End_String;
                    isfrom = false;
                    break;
            }

            twn = controller.mod_Text.xt_Text_To(
                false,
                node.TextCursor,
                target_value,
                node.Duration * duration,
                true,
                node.TextCursorBlinkSpeed > 0 ? node.TextCursorBlinkSpeed : 0.5f,
                node.Rewind_Set_Startvalue,
                node.Complete_Set_Endvalue)
                .SetDelay(node.Delay + delay)
                .SetLoop(node.LoopCount, node.LoopType);

            // 仅在需要 From 时设置起始值
            // S-* 模式：静态起点
            if (isfrom)
                twn = twn.SetFrom(node.From_String);

            // C-E 模式：动态起点
            if (node.TweenValueMode == TweenValueMode.当前到结束_C_E)
                twn = twn.SetFromDynamic(() => controller.mod_Text.text);

            if (node.Ease == EaseMode.None)
                twn = twn.SetEase(node.Curve);
            else
                twn = twn.SetEase(node.Ease);

            twn = twn.OnUpdate<string>((v, d, t) =>
            {
                node.Progress = twn.ElapsedTime / twn.Duration;

                if (twn.CurrentEasedProgress >= percentage_threshold)
                {
                    if (!sw)
                    {
                        sw = true;
                        if (on_percentage != null)
                            on_percentage();
                    }
                }

                if (on_update != null)
                    on_update();

                if (node.Act_On_Text_Changed != null)
                    node.Act_On_Text_Changed(controller.mod_Text.text);
            });

            twn = twn.OnRewind(() =>
            {
                if (on_rewind != null)
                    on_rewind();

                if (node.Act_On_Text_Rewind != null)
                    node.Act_On_Text_Rewind(controller.mod_Text.text);
            });

            twn = twn.OnComplete((d) =>
            {
                node.Progress = 0;
                if (on_complete != null)
                    on_complete();
            });

            return twn;
        }
        /// <summary>
        /// 创建打字机动画（TextMeshPro 版本）
        /// 
        /// 【依赖组件】controller.mod_TmpText（TMP_Text）
        /// 
        /// 【与 UGUI Text 版本的差异】
        /// - 底层调用参数少：xt_Text_To(false, target, duration, true, rewind, complete)
        ///   不含光标字符与闪烁速度（TMP 版本不需要）
        /// 
        /// 【回调携带值】Act_On_Text_Changed/Rewind 携带 mod_TmpText.text
        /// </summary>
        private XTween_Interface Tween_Create_Writer_TmpText(TweenNode node, float duration, float delay, float percentage_threshold, UnityAction on_update = null, UnityAction on_percentage = null, UnityAction on_complete = null, UnityAction on_rewind = null)
        {
            if (controller.mod_TmpText == null)
            {
                return null;
            }

            XTween_Interface twn = null;
            bool sw = false;

            string target_value;
            bool isfrom;
            switch (node.TweenValueMode)
            {
                case TweenValueMode.起始到默认_S_D:
                    target_value = node.Original_String;
                    isfrom = true;
                    break;
                case TweenValueMode.起始到结束_S_E:
                    target_value = node.End_String;
                    isfrom = true;
                    break;
                case TweenValueMode.默认到结束_D_E:
                    target_value = node.End_String;
                    isfrom = false;
                    break;
                case TweenValueMode.当前到结束_C_E:
                    target_value = node.End_String;
                    isfrom = false;
                    break;
                default:
                    target_value = node.End_String;
                    isfrom = false;
                    break;
            }

            twn = controller.mod_TmpText.xt_Text_To(
                false,
                target_value,
                node.Duration * duration,
                true,
                node.Rewind_Set_Startvalue,
                node.Complete_Set_Endvalue)
                .SetDelay(node.Delay + delay)
                .SetLoop(node.LoopCount, node.LoopType);

            // 仅在需要 From 时设置起始值
            // S-* 模式：静态起点
            if (isfrom)
                twn = twn.SetFrom(node.From_String);

            // C-E 模式：动态起点
            if (node.TweenValueMode == TweenValueMode.当前到结束_C_E)
                twn = twn.SetFromDynamic(() => controller.mod_TmpText.text);

            if (node.Ease == EaseMode.None)
                twn = twn.SetEase(node.Curve);
            else
                twn = twn.SetEase(node.Ease);

            twn = twn.OnUpdate<string>((v, d, t) =>
            {
                node.Progress = twn.ElapsedTime / twn.Duration;

                if (twn.CurrentEasedProgress >= percentage_threshold)
                {
                    if (!sw)
                    {
                        sw = true;
                        if (on_percentage != null)
                            on_percentage();
                    }
                }

                if (on_update != null)
                    on_update();

                if (node.Act_On_Text_Changed != null)
                    node.Act_On_Text_Changed(controller.mod_TmpText.text);
            });

            twn = twn.OnRewind(() =>
            {
                if (on_rewind != null)
                    on_rewind();

                if (node.Act_On_Text_Rewind != null)
                    node.Act_On_Text_Rewind(controller.mod_TmpText.text);
            });

            twn = twn.OnComplete((d) =>
            {
                node.Progress = 0;
                if (on_complete != null)
                    on_complete();
            });

            return twn;
        }
        /// <summary>
        /// 创建尺寸动画（RectTransform.sizeDelta）
        /// 
        /// 【依赖组件】controller.mod_Rect
        /// 【数值类型】Vector2
        /// 【回调携带值】Act_On_Vector2_Changed/Rewind 携带 sizeDelta
        /// 
        /// 【说明】
        /// 尺寸动画改变的是 RectTransform 的宽高尺寸，不改变缩放
        /// 与缩放动画的区别：
        /// - 缩放：局部缩放系数，影响视觉大小与布局占位无关
        /// - 尺寸：直接改变布局尺寸，可能触发布局重建（性能开销略高）
        /// </summary>
        private XTween_Interface Tween_Create_Size(TweenNode node, float duration, float delay, float percentage_threshold, UnityAction on_update = null, UnityAction on_percentage = null, UnityAction on_complete = null, UnityAction on_rewind = null)
        {
            if (controller.mod_Rect == null)
                return null;

            XTween_Interface twn = null;
            bool sw = false;

            Vector2 target_value;
            bool isfrom;
            switch (node.TweenValueMode)
            {
                case TweenValueMode.起始到默认_S_D:
                    target_value = node.Original_Vector2;
                    isfrom = true;
                    break;
                case TweenValueMode.起始到结束_S_E:
                    target_value = node.End_Vector2;
                    isfrom = true;
                    break;
                case TweenValueMode.默认到结束_D_E:
                    target_value = node.End_Vector2;
                    isfrom = false;
                    break;
                case TweenValueMode.当前到结束_C_E:
                    target_value = node.End_Vector2;
                    isfrom = false;
                    break;
                default:
                    target_value = node.End_Vector2;
                    isfrom = false;
                    break;
            }

            twn = controller.mod_Rect.xt_Size_To(
                target_value,
                node.Duration * duration,
                false,
                true,
                node.Rewind_Set_Startvalue,
                node.Complete_Set_Endvalue)
                .SetDelay(node.Delay + delay)
                .SetLoop(node.LoopCount, node.LoopType);

            // 仅在需要 From 时设置起始值
            // S-* 模式：静态起点
            if (isfrom)
                twn = twn.SetFrom(node.From_Vector2);

            // C-E 模式：动态起点
            if (node.TweenValueMode == TweenValueMode.当前到结束_C_E)
                twn = twn.SetFromDynamic(() => controller.mod_Rect.sizeDelta);

            if (node.Ease == EaseMode.None)
                twn = twn.SetEase(node.Curve);
            else
                twn = twn.SetEase(node.Ease);

            twn = twn.OnUpdate<Vector2>((v, d, t) =>
            {
                node.Progress = twn.ElapsedTime / twn.Duration;

                if (twn.CurrentEasedProgress >= percentage_threshold)
                {
                    if (!sw)
                    {
                        sw = true;
                        if (on_percentage != null)
                            on_percentage();
                    }
                }

                if (on_update != null)
                    on_update();

                if (node.Act_On_Vector2_Changed != null)
                    node.Act_On_Vector2_Changed(controller.mod_Rect.sizeDelta);
            });

            twn = twn.OnRewind(() =>
            {
                if (on_rewind != null)
                    on_rewind();

                if (node.Act_On_Vector2_Rewind != null)
                    node.Act_On_Vector2_Rewind(controller.mod_Rect.sizeDelta);
            });

            twn = twn.OnComplete((d) =>
            {
                node.Progress = 0;
                if (on_complete != null)
                    on_complete();
            });

            return twn;
        }
        /// <summary>
        /// 创建图像填充动画（Image.fillAmount）
        /// 
        /// 【依赖组件】controller.mod_Image（UnityEngine.UI.Image）
        /// 【数值类型】float，范围限定 0-1（使用 Mathf.Clamp01）
        /// 【回调携带值】Act_On_Float_Changed/Rewind 携带 fillAmount
        /// 
        /// 【Clamp01 说明】
        /// - 目标值与 From 值都经过 Clamp01 处理
        /// - 防止外部传入超出 0-1 范围导致 Image 渲染异常
        /// 
        /// 【适用条件】
        /// Image 的 Type 需为 Filled（填充模式），否则 fillAmount 不生效
        /// </summary>
        private XTween_Interface Tween_Create_Fill(TweenNode node, float duration, float delay, float percentage_threshold, UnityAction on_update = null, UnityAction on_percentage = null, UnityAction on_complete = null, UnityAction on_rewind = null)
        {
            if (controller.mod_Image == null)
                return null;

            XTween_Interface twn = null;
            bool sw = false;

            float target_value;
            bool isfrom;
            switch (node.TweenValueMode)
            {
                case TweenValueMode.起始到默认_S_D:
                    target_value = node.Original_Float;
                    isfrom = true;
                    break;
                case TweenValueMode.起始到结束_S_E:
                    target_value = node.End_Float;
                    isfrom = true;
                    break;
                case TweenValueMode.默认到结束_D_E:
                    target_value = node.End_Float;
                    isfrom = false;
                    break;
                case TweenValueMode.当前到结束_C_E:
                    target_value = node.End_Float;
                    isfrom = false;
                    break;
                default:
                    target_value = node.End_Float;
                    isfrom = false;
                    break;
            }

            // 目标值 Clamp01 保证填充量合法
            twn = controller.mod_Image.xt_Fill_To(
                Mathf.Clamp01(target_value),
                node.Duration * duration,
                true,
                node.Rewind_Set_Startvalue,
                node.Complete_Set_Endvalue)
                .SetDelay(node.Delay + delay)
                .SetLoop(node.LoopCount, node.LoopType);

            // 仅在需要 From 时设置起始值
            // S-* 模式：静态起点
            if (isfrom)
                twn = twn.SetFrom(Mathf.Clamp01(node.From_Float));

            // C-E 模式：动态起点
            if (node.TweenValueMode == TweenValueMode.当前到结束_C_E)
                twn = twn.SetFromDynamic(() => controller.mod_Image.fillAmount);

            if (node.Ease == EaseMode.None)
                twn = twn.SetEase(node.Curve);
            else
                twn = twn.SetEase(node.Ease);

            twn = twn.OnUpdate<float>((v, d, t) =>
            {
                node.Progress = twn.ElapsedTime / twn.Duration;

                if (twn.CurrentEasedProgress >= percentage_threshold)
                {
                    if (!sw)
                    {
                        sw = true;
                        if (on_percentage != null)
                            on_percentage();
                    }
                }

                if (on_update != null)
                    on_update();

                if (node.Act_On_Float_Changed != null)
                    node.Act_On_Float_Changed(controller.mod_Image.fillAmount);
            });

            twn = twn.OnRewind(() =>
            {
                if (on_rewind != null)
                    on_rewind();

                if (node.Act_On_Float_Rewind != null)
                    node.Act_On_Float_Rewind(controller.mod_Image.fillAmount);
            });

            twn = twn.OnComplete((d) =>
            {
                node.Progress = 0;
                if (on_complete != null)
                    on_complete();
            });

            return twn;
        }
        /// <summary>
        /// 根据列表索引创建动画
        /// 
        /// 【定位方式】PrimitiveTweenNodes[index]
        /// 【时长倍率】XHud_Manager.Instance.DurationMultiply × GlobalDuration
        /// 【副作用】
        /// 1. 将创建的 XTween_Interface 赋值给 node.Tweener
        /// 2. 触发 act_on_Tween_Create_At_Index / eve_on_Tween_Create_At_Index
        /// 
        /// 【注意事项】
        /// - 索引随节点增删而变动，不适合持久化使用
        /// - 索引越界会抛异常，调用方需保证 index 合法
        /// </summary>
        /// <param name="index">节点列表索引</param>
        /// <param name="delay">额外延时</param>
        /// <param name="percentage_threshold">进度阈值</param>
        /// <returns>创建成功的动画实例</returns>
        public XTween_Interface Tween_CreateByIndex(int index, float delay = 0, float percentage_threshold = 0.5f, UnityAction on_update = null, UnityAction on_percentage = null, UnityAction on_complete = null, UnityAction on_rewind = null)
        {
            TweenNode arg = PrimitiveTweenNodes[index];

            // 全局速率倍率：管理器速率 × 本动画器速率
            arg.Tweener = Tween_Create(arg, XHud_Manager.Instance.DurationMultiply * GlobalDuration, delay, percentage_threshold, on_update, on_percentage, on_complete, on_rewind);

            // 委托 + 事件双触发
            if (act_on_Tween_Create_At_Index != null)
                act_on_Tween_Create_At_Index(arg);
            eve_on_Tween_Create_At_Index.Invoke(arg);

            return arg.Tweener;
        }
        /// <summary>
        /// 根据唯一 ID 创建动画
        /// 
        /// 【定位方式】TweenNode_GetByID(id)
        /// 【时长倍率】XHud_Manager.Instance.DurationMultiply × GlobalDuration
        /// 【副作用】
        /// 1. 将创建的 XTween_Interface 赋值给 node.Tweener
        /// 2. 触发 act_on_Tween_Create_At_ID / eve_on_Tween_Create_At_ID
        /// 
        /// 【注意事项】
        /// - ID 是持久化引用的首选方式，不受节点增删影响
        /// - 若 ID 不存在，TweenNode_GetByID 返回 null，后续访问会抛异常
        /// </summary>
        /// <param name="id">动画节点唯一 ID</param>
        /// <param name="delay">额外延时</param>
        /// <param name="percentage_threshold">进度阈值</param>
        /// <returns>创建成功的动画实例</returns>
        public XTween_Interface Tween_CreateById(int id, float delay = 0, float percentage_threshold = 0.5f, UnityAction on_update = null, UnityAction on_percentage = null, UnityAction on_complete = null, UnityAction on_rewind = null)
        {
            TweenNode arg = TweenNode_GetByID(id);

            arg.Tweener = Tween_Create(arg, XHud_Manager.Instance.DurationMultiply * GlobalDuration, delay, percentage_threshold, on_update, on_percentage, on_complete, on_rewind);

            if (act_on_Tween_Create_At_ID != null)
                act_on_Tween_Create_At_ID(arg);
            eve_on_Tween_Create_At_ID.Invoke(arg);

            return arg.Tweener;
        }
        /// <summary>
        /// 根据名称标识创建动画
        /// 
        /// 【定位方式】TweenNode_GetByIndicator(indicator)
        /// 【时长倍率】XHud_Manager.Instance.DurationMultiply × GlobalDuration
        /// 【副作用】
        /// 1. 将创建的 XTween_Interface 赋值给 node.Tweener
        /// 2. 触发 act_on_Tween_Create_At_Indicator / eve_on_Tween_Create_At_Indicator
        /// 
        /// 【注意事项】
        /// - Indicator 允许重名，仅返回首个匹配项
        /// - 若名称不存在，TweenNode_GetByIndicator 返回 null，后续访问会抛异常
        /// - 推荐在项目内保持 Indicator 唯一
        /// </summary>
        /// <param name="indicator">动画节点名称标识</param>
        /// <param name="delay">额外延时</param>
        /// <param name="percentage_threshold">进度阈值</param>
        /// <returns>创建成功的动画实例</returns>
        public XTween_Interface Tween_CreateByIndicator(string indicator, float delay = 0, float percentage_threshold = 0.5f, UnityAction on_update = null, UnityAction on_percentage = null, UnityAction on_complete = null, UnityAction on_rewind = null)
        {
            TweenNode arg = TweenNode_GetByIndicator(indicator);

            arg.Tweener = Tween_Create(arg, XHud_Manager.Instance.DurationMultiply * GlobalDuration, delay, percentage_threshold, on_update, on_percentage, on_complete, on_rewind);

            if (act_on_Tween_Create_At_Indicator != null)
                act_on_Tween_Create_At_Indicator(arg);
            eve_on_Tween_Create_At_Indicator.Invoke(arg);

            return arg.Tweener;
        }
        #endregion

        #region 动画播放
        /// <summary>
        /// 动画播放（重载一：直接传入 TweenNode 实例）
        /// 
        /// 【使用前提】
        /// - 传入的 node.Tweener 必须已被创建（非 null）
        ///   通常通过 Tween_Create / Tween_CreateByIndex / Tween_CreateById / Tween_CreateByIndicator 得到
        /// - 若 node.Tweener 为 null，会在 node.Tweener.Play() 处抛 NullReferenceException
        /// 
        /// 【执行流程】
        /// 1. 调用底层 XTween_Interface.Play() 启动播放
        /// 2. 触发 act_on_Tween_Play_At_Node（代码绑定委托）
        /// 3. 触发 eve_on_Tween_Play_At_Node（Inspector 绑定事件）
        /// 
        /// 【适用场景】
        /// - 已持有节点引用，且已调用过 Tween_Create 系列方法创建过动画实例
        /// - 需要"创建后播放"分离控制的场景（例如提前创建、延迟播放）
        /// 
        /// 【注意事项】
        /// - 本方法只负责"播放"，不负责"创建"
        /// - 若需"创建 + 播放"一步到位，请使用 Tween_Play(int id) 或 Tween_Play(string indicator)
        /// </summary>
        /// <param name="node">已创建好 Tweener 的动画节点</param>
        public void Tween_Play(TweenNode node)
        {
            // 直接调用底层动画实例的播放接口
            node.Tweener.Play();

            // 触发代码绑定委托（需手动判空）
            if (act_on_Tween_Play_At_Node != null)
                act_on_Tween_Play_At_Node(node);

            // 触发 Inspector 绑定事件（内部已判空）
            eve_on_Tween_Play_At_Node.Invoke(node);
        }
        /// <summary>
        /// 动画播放（重载二：直接传入 XTween_Interface 实例）
        /// 
        /// 【使用前提】
        /// - 传入的 tween 必须为有效实例（非 null 且未被 Kill）
        /// 
        /// 【执行流程】
        /// 1. 调用 tween.Play() 启动播放
        /// 2. 触发 act_on_Tween_Play（携带 XTween_Interface）
        /// 3. 触发 eve_on_Tween_Play
        /// 
        /// 【与重载一的区别】
        /// - 重载一：以 TweenNode 为单位，隐含"节点 = 动画"的语义
        /// - 重载二：以底层 XTween_Interface 为单位，适合外部持有原始实例的场景
        /// 
        /// 【适用场景】
        /// - 外部系统直接持有 XTween_Interface 引用，无需通过 TweenNode 中转
        /// - 由 Tween_Create 系列方法返回后直接调用本重载
        /// </summary>
        /// <param name="tween">底层动画实例</param>
        public void Tween_Play(XTween_Interface tween)
        {
            // 直接播放底层实例
            tween.Play();

            // 触发代码绑定委托
            if (act_on_Tween_Play != null)
                act_on_Tween_Play(tween);

            // 触发 Inspector 绑定事件
            eve_on_Tween_Play.Invoke(tween);
        }
        /// <summary>
        /// 动画播放（重载三：按唯一 ID 创建并播放）
        /// 
        /// 【执行流程】
        /// 1. 调用 Tween_CreateById(id) 创建动画实例
        ///    （内部会查找节点、创建 Tweener、赋值给 node.Tweener、触发 Create 事件）
        /// 2. 立即调用返回值的 Play() 启动播放
        /// 3. 触发 act_on_Tween_Play_At_ID / eve_on_Tween_Play_At_ID
        /// 
        /// 【与重载一/二的区别】
        /// - 重载一/二：假设 Tweener 已存在，只负责"播放"
        /// - 重载三：一步到位"创建 + 播放"，适合外部只持有 ID 的场景
        /// 
        /// 【适用场景】
        /// - 外部系统通过保存的 ID 触发某条动画
        /// - 数据驱动的动画播放（如配置表中记录的动画 ID）
        /// - UI 按钮点击直接播放指定动画
        /// 
        /// 【注意事项】
        /// - 每次调用都会重新创建 Tweener 实例（旧实例被 Kill 或 Rewind）
        /// - 若 ID 不存在，Tween_CreateById 内部会因 arg 为 null 而抛异常
        /// - 播放事件参数为 int（ID），而非 TweenNode
        /// </summary>
        /// <param name="id">动画节点的唯一 ID</param>
        public void Tween_Play(int id)
        {
            // 创建动画实例并立即播放
            Tween_CreateById(id).Play();

            // 触发代码绑定委托（携带 ID）
            if (act_on_Tween_Play_At_ID != null)
                act_on_Tween_Play_At_ID(id);

            // 触发 Inspector 绑定事件（携带 ID）
            eve_on_Tween_Play_At_ID.Invoke(id);
        }
        /// <summary>
        /// 动画播放（重载四：按名称标识创建并播放）
        /// 
        /// 【执行流程】
        /// 1. 调用 Tween_CreateByIndicator(indicator) 创建动画实例
        /// 2. 立即调用返回值的 Play() 启动播放
        /// 3. 触发 act_on_Tween_Play_At_Indicator / eve_on_Tween_Play_At_Indicator
        /// 
        /// 【与重载三的区别】
        /// - 重载三：按 ID 定位（唯一，可信任）
        /// - 重载四：按 Indicator 定位（可重名，返回首个匹配项）
        /// 
        /// 【适用场景】
        /// - 代码中以语义化名称直接播放动画（如 "FadeIn"、"Shake"）
        /// - 编辑器/工具中以名称快速调试
        /// - 上层 Element 系统通过 MainTweenNode 名称播放主入口动画
        /// 
        /// 【注意事项】
        /// - 若名称不存在，Tween_CreateByIndicator 返回的节点为 null，会抛异常
        /// - Indicator 允许重名，只返回列表中的首个匹配项
        /// - 播放事件参数为 string（Indicator），而非 TweenNode
        /// </summary>
        /// <param name="indicator">动画节点的名称标识</param>
        public void Tween_Play(string indicator)
        {
            // 创建动画实例并立即播放
            Tween_CreateByIndicator(indicator).Play();

            // 触发代码绑定委托（携带名称）
            if (act_on_Tween_Play_At_Indicator != null)
                act_on_Tween_Play_At_Indicator(indicator);

            // 触发 Inspector 绑定事件（携带名称）
            eve_on_Tween_Play_At_Indicator.Invoke(indicator);
        }
        #endregion

        #region 播放全部动画
        /*        Tweens_Play_With_Delay(delay, dur, MatchTiming, tim, ...)
         *         │
         *         ▼
         *   ┌──────────────────────┐
         *   │ ① controller 判空     │── null ──► return
         *   └──────────────────────┘
         *         │ 非空
         *         ▼
         *   ┌──────────────────────┐
         *   │ ② 遍历 PrimitiveTween │
         *   │    Nodes 列表          │
         *   └──────────────────────┘
         *         │
         *         ├─ Enabled == false ──► 跳过
         *         │
         *         ├─ MatchTiming == true
         *         │       │
         *         │       ├─ Timings == tim ──► 创建 + 播放
         *         │       └─ Timings != tim ──► 跳过
         *         │
         *         └─ MatchTiming == false
         *                 └─ 创建 + 播放
         *         │
         *         ▼
         *   ┌──────────────────────┐
         *   │ ③ 触发 PlayAll 事件   │
         *   └──────────────────────┘
        */

        /// <summary>
        /// 动画播放（所有节点 · 批量播放 + 统一延时）
        /// 
        /// 【功能概述】
        /// 遍历 PrimitiveTweenNodes 列表，对所有 Enabled 的节点：
        ///   ① 创建（或重建）动画实例
        ///   ② 立即播放
        /// 并在全部处理完成后，触发一次"全部播放"事件
        /// 
        /// 【与单个播放方法的区别】
        /// - Tween_Play(...)         : 单节点播放，需先创建 Tweener
        /// - Tweens_Play_With_Delay  : 批量播放，内部创建 + 播放一步到位
        /// 
        /// 【执行流程】
        /// 1. 前置校验：controller.mod_Rect 为 null 则直接返回（图元无 RectTransform 无法播放）
        /// 2. 遍历所有节点：
        ///    a. 跳过 Enabled == false 的节点
        ///    b. 若 MatchTiming 为 true，则进一步筛选 Timings == tim 的节点
        ///    c. 对通过筛选的节点：
        ///       - 调用 Tween_Create 创建/重建 Tweener
        ///       - 赋值给 node.Tweener
        ///       - 立即调用 Play()
        /// 3. 全部处理完成后，触发 act_on_Tween_PlayAll / eve_on_Tween_PlayAll
        /// 
        /// 【时长计算】
        /// 最终时长 = node.Duration × XHud_Manager.DurationMultiply × GlobalDuration × dur
        /// - DurationMultiply    : 全局（管理器级）速率倍率
        /// - GlobalDuration      : 本动画器（图元级）速率倍率
        /// - dur                 : 本次调用的额外倍率（单次生效）
        /// 
        /// 【Timings 筛选说明】
        /// - MatchTiming == false : 播放所有 Enabled 节点，Timings 无意义
        /// - MatchTiming == true  : 只播放 Enabled 且 Timings == tim 的节点
        /// - Timings 是字符串标记，用于将动画节点分组（如 "Enter" / "Exit" / "Loop"）
        /// 
        /// 【注意事项】
        /// - 每个节点都会被重新创建 Tweener（旧实例在 Tween_Create 内被 Rewind 或 Kill）
        /// - on_update / on_percentage / on_complete 会同时绑定到**所有**创建的动画节点
        ///   因此可能一帧内被调用多次（每个节点各调用一次）
        /// - on_rewind 参数虽然声明了，但**并未传递给 Tween_Create**（疑似遗漏）
        /// - 传参 tim 在 MatchTiming == false 时不参与逻辑
        /// </summary>
        /// <param name="delay">统一延时（叠加到每个节点的 node.Delay 之上）</param>
        /// <param name="dur">时长倍率（本方法独有，叠加到全局速率之上）</param>
        /// <param name="MatchTiming">是否按 Timings 筛选节点</param>
        /// <param name="tim">筛选用的 Timings 值（仅当 MatchTiming 为 true 时生效）</param>
        /// <param name="percentage_threshold">进度阈值（0-1），各节点达到后触发一次 on_percentage</param>
        /// <param name="on_update">每帧更新回调（绑定到所有创建的节点）</param>
        /// <param name="on_percentage">进度达到阈值的单次回调（绑定到所有创建的节点）</param>
        /// <param name="on_complete">动画完成时的回调（绑定到所有创建的节点）</param>
        /// <param name="on_rewind">动画回退时的回调（⚠ 当前实现未传递给 Tween_Create）</param>
        public void Tweens_Play_With_Delay(float delay, float dur = 1, bool MatchTiming = false, string tim = "", float percentage_threshold = 0.5f, UnityAction on_update = null, UnityAction on_percentage = null, UnityAction on_complete = null, UnityAction on_rewind = null)
        {
            // ========== 前置校验 ==========
            // 图元缺少 RectTransform 时无法承载大多数动画（位移动画等），直接返回
            // 注意：这也意味着淡化、颜色等其他类型动画会被一并跳过
            if (controller.mod_Rect == null)
                return;

            ///--播放
            // ========== 遍历所有节点，逐个创建并播放 ==========
            for (int i = 0; i < PrimitiveTweenNodes.Count; i++)
            {
                // 跳过未启用的节点
                if (PrimitiveTweenNodes[i].Enabled)
                {
                    // ---------- 分支 A：按 Timings 筛选 ----------
                    if (MatchTiming)
                    {
                        // 仅播放 Timings 匹配的节点
                        if (PrimitiveTweenNodes[i].Timings == tim)
                        {
                            TweenNode arg = PrimitiveTweenNodes[i];

                            // 创建动画实例（内部会先 Rewind 或 Kill 旧实例）
                            // 时长 = 节点时长 × 管理器速率 × 本动画器速率 × 本次倍率
                            arg.Tweener = Tween_Create(
                                arg,
                                XHud_Manager.Instance.DurationMultiply * GlobalDuration * dur,
                                delay,
                                percentage_threshold,
                                on_update,
                                on_percentage,
                                on_complete,
                                on_rewind);

                            // 立即播放
                            arg.Tweener.Play();
                        }
                    }
                    // ---------- 分支 B：播放全部 Enabled 节点 ----------
                    else
                    {
                        TweenNode arg = PrimitiveTweenNodes[i];

                        // 创建动画实例（同上）
                        arg.Tweener = Tween_Create(
                            arg,
                            XHud_Manager.Instance.DurationMultiply * GlobalDuration * dur,
                            delay,
                            percentage_threshold,
                            on_update,
                            on_percentage,
                            on_complete,
                            on_rewind);

                        // 立即播放
                        arg.Tweener.Play();
                    }
                }
                else
                    continue;   // 未启用节点直接跳过（此 continue 实际可省略，因为已在 if 分支内）
            }

            // ========== 触发"全部播放"事件 ==========
            // 代码绑定委托（需判空）
            if (act_on_Tween_PlayAll != null)
                act_on_Tween_PlayAll();

            // Inspector 绑定事件（内部判空）
            eve_on_Tween_PlayAll.Invoke();
        }
        #endregion

        #region 动画复位  /  杀死

        /// <summary>
        /// 动画重置（所有节点）
        /// 
        /// 【功能】
        /// 遍历所有动画节点，逐个调用 Tween_Rewind 复位到起始状态，
        /// 最后统一触发"全部复位"事件。
        /// 
        /// 【执行流程】
        /// 1. 遍历 PrimitiveTweenNodes 列表
        /// 2. 对每个节点调用 Tween_Rewind(node, complete)
        ///    - 内部会 Kill + Rewind Tweener
        ///    - 并根据 TweenValueMode 恢复到 From 或 Original 值
        /// 3. 触发 act_on_Tween_RewindAll / eve_on_Tween_RewindAll
        /// 
        /// 【注意事项】
        /// - TweenValueMode == 当前到结束_C_E 的节点会被 Tween_Rewind 内部跳过
        ///   （因为该模式没有可回退的起始状态）
        /// </summary>
        /// <param name="complete">是否以"完成"语义杀死（传递给底层 XTween 的 Kill/Rewind）</param>
        public void Tween_RewindAll(bool complete = true)
        {
            ///--动画恢复初始
            for (int i = 0; i < PrimitiveTweenNodes.Count; i++)
            {
                // 逐个节点复位（内部含类型判别与值恢复）
                Tween_Rewind(PrimitiveTweenNodes[i], complete);
            }

            // 代码绑定委托（需判空）
            if (act_on_Tween_RewindAll != null)
                act_on_Tween_RewindAll();

            // Inspector 绑定事件（内部判空）
            eve_on_Tween_RewindAll.Invoke();
        }

        /// <summary>
        /// 动画杀死（所有节点）
        /// 
        /// 【功能】
        /// 遍历所有动画节点，逐个调用 Tween_Kill 销毁动画实例（Tweener = null），
        /// 最后统一触发"全部杀死"事件。
        /// 
        /// 【与 Tween_RewindAll 的区别】
        /// - RewindAll：恢复数值到起始状态，保留节点结构，Tweener 可能仍存在
        /// - KillAll  ：销毁 Tweener 实例，节点保留但失去播放能力，数值**不恢复**
        /// 
        /// 【执行流程】
        /// 1. 遍历 PrimitiveTweenNodes
        /// 2. 对每个节点调用 Tween_Kill(node, complete)
        ///    - Kill 掉 Tweener 实例并置空
        ///    - 重置 Progress 与音效状态
        /// 3. 触发 act_on_Tween_KillAll / eve_on_Tween_KillAll
        /// 
        /// 【使用场景】
        /// - 图元销毁前清理所有动画实例（防止内存泄漏）
        /// - 对象池回收时彻底重置
        /// - 需要"重新创建"动画前先杀死旧实例
        /// 
        /// 【注意事项】
        /// - 杀死后节点数值不会自动恢复，如需恢复请改用 Tween_RewindAll
        /// - 杀死后仍可再次调用 Tween_Create 重新创建动画
        /// - 事件参数无参
        /// </summary>
        /// <param name="complete">是否以"完成"语义杀死</param>
        public void Tween_KillAll(bool complete = true)
        {
            ///--动画恢复初始
            for (int i = 0; i < PrimitiveTweenNodes.Count; i++)
            {
                // 逐个节点杀死动画实例
                Tween_Kill(PrimitiveTweenNodes[i], complete);
            }

            // 代码绑定委托（需判空）
            if (act_on_Tween_KillAll != null)
                act_on_Tween_KillAll();

            // Inspector 绑定事件（内部判空）
            eve_on_Tween_KillAll.Invoke();
        }

        /// <summary>
        /// 动画清理（所有节点）
        /// 
        /// 【功能】
        /// 遍历所有节点，仅将 node.Tweener 引用置空，不做其他处理。
        /// 
        /// 【与 Tween_KillAll 的区别】
        /// - Tween_KillAll：调用 Tweener.Kill() 主动销毁实例 + 置空 + 重置状态
        /// - Tween_CleanAll：仅置空引用，不调用 Kill，也不重置 Progress/音效
        /// 
        /// 【为什么要区分？】
        /// - 某些场景下 Tweener 已被底层系统自动销毁（如完成时自动 Kill）
        ///   此时只需断引用，无需再次 Kill
        /// - 性能敏感场景下，仅断引用比完整 Kill 更轻量
        /// 
        /// 【使用场景】
        /// - 已知底层动画已停止，仅需清理引用
        /// - 组件销毁前的引用清理（配合 GC）
        /// - 避免悬空引用导致的误访问
        /// 
        /// 【注意事项】
        /// - 不会重置 Progress，也不会重置音效 IsPlayed
        /// - 若 Tweener 仍在播放，置空引用后无法再控制它（可能造成"幽灵动画"）
        ///   因此正常使用应优先选择 Tween_KillAll
        /// </summary>
        public void Tween_CleanAll()
        {
            ///--动画清理
            for (int i = 0; i < PrimitiveTweenNodes.Count; i++)
            {
                // 仅断开引用，不调用 Kill，不重置状态
                PrimitiveTweenNodes[i].Tweener = null;
            }
        }

        /// <summary>
        /// 复位动画（单个节点）
        /// 
        /// 【核心语义】
        /// 将动画节点恢复到"起始状态"，包括：
        /// - 销毁/回退 Tweener 实例
        /// - 恢复目标组件的数值（根据 TweenValueMode 决策）
        /// - 重置 Progress 与音效触发状态
        /// - 触发节点自身的 Rewind 回调
        /// 
        /// 【前置跳过条件】
        /// 若 arg.TweenValueMode == TweenValueMode.当前到结束_C_E，直接 return：
        /// 该模式没有可回退的起始值（只有"当前值"，回退无意义）
        /// 
        /// 【数值恢复策略（按 TweenValueMode 分派）】
        ///   ┌──────────────────────┬──────────────────────┐
        ///   │ TweenValueMode       │  恢复到               │
        ///   ├──────────────────────┼──────────────────────┤
        ///   │ 起始到默认_S_D        │  From 值              │
        ///   │ 起始到结束_S_E        │  From 值              │
        ///   │ 默认到结束_D_E        │  Original 值          │
        ///   │ 当前到结束_C_E        │  （不恢复，已提前返回）│
        ///   └──────────────────────┴──────────────────────┘
        /// 
        /// 【按类型分派】
        /// 每种 TweenNodeType 对应不同的目标组件与数值字段：
        /// - a_位移      → RectTransform.anchoredPosition3D（Vector3）
        /// - r_旋转      → RectTransform.rotation（Quaternion，欧拉角输入）
        /// - s_缩放      → RectTransform.localScale（Vector3）
        /// - c_颜色      → Graphic.color（Color）
        /// - g_淡化      → CanvasGroup.alpha（float）
        /// - w_打字机    → Text/TMP 文本内容（string）
        /// - z_尺寸      → RectTransform.sizeDelta（Vector2）
        /// - f_图像填充  → Image.fillAmount（float）
        /// 
        /// 【使用场景】
        /// - 单个节点需要精确复位
        /// - Tween_RewindAll 内部逐节点调用
        /// - 播放前的前置清理（Tween_Create 内部也会调用）
        /// 
        /// 【注意事项】
        /// - 各类型分支中若目标组件为 null 会提前 return，可能跳过后续的事件触发
        /// - 事件在最后统一触发（若前面提前 return 则不会触发）
        /// - AnimatingBreakState 会被重置为 false
        /// </summary>
        /// <param name="arg">目标动画节点</param>
        /// <param name="complete">是否以"完成"语义杀死（传递给底层 Tweener.Kill / Rewind）</param>
        public void Tween_Rewind(TweenNode arg, bool complete = true)
        {
            /*如果动画方式为：当前 -> 结束则不执行退回动画，因为此方式是指让动画直接到达目标值（没有起始值，如果有也只是他的当前值），所以倒退对其没有意义*/
            if (arg.TweenValueMode == TweenValueMode.当前到结束_C_E)
                return;

            #region 杀死动画
            // 先 Kill 再 Rewind：Kill 停止播放并释放，Rewind 触发回退回调
            // 空值检查：未创建 Tweener 时跳过
            if (arg.Tweener != null)
            {
                arg.Tweener.Kill(complete);
                arg.Tweener.Rewind(complete);
            }
            #endregion

            // 重置进度（此处重复赋值，下一区域会再赋一次，疑似冗余）
            arg.Progress = 0;

            // ========== 按动画类型恢复数值 ==========
            if (arg.Type == TweenNodeType.a_位移)
            {
                // 起始类模式（S_D / S_E）：恢复到 From 值
                if (arg.TweenValueMode == TweenValueMode.起始到默认_S_D ||
                    arg.TweenValueMode == TweenValueMode.起始到结束_S_E)
                {
                    controller.mod_Rect.anchoredPosition3D = arg.From_Vector3;
                    if (arg.Act_On_Vector3_Rewind != null)
                        arg.Act_On_Vector3_Rewind(arg.From_Vector3);
                }
                // 默认→结束模式（D_E）：恢复到 Original 值
                else if (arg.TweenValueMode == TweenValueMode.默认到结束_D_E)
                {
                    controller.mod_Rect.anchoredPosition3D = arg.Original_Vector3;
                    if (arg.Act_On_Vector3_Rewind != null)
                        arg.Act_On_Vector3_Rewind(arg.Original_Vector3);
                }
            }
            else if (arg.Type == TweenNodeType.r_旋转)
            {
                if (arg.TweenValueMode == TweenValueMode.起始到默认_S_D ||
                    arg.TweenValueMode == TweenValueMode.起始到结束_S_E)
                {
                    controller.mod_Rect.rotation = Quaternion.Euler(arg.From_Vector3);
                    if (arg.Act_On_Quaternion_Rewind != null)
                        arg.Act_On_Quaternion_Rewind(Quaternion.Euler(arg.From_Vector3));
                }
                else if (arg.TweenValueMode == TweenValueMode.默认到结束_D_E)
                {
                    controller.mod_Rect.rotation = Quaternion.Euler(arg.Original_Vector3);
                    if (arg.Act_On_Quaternion_Rewind != null)
                        arg.Act_On_Quaternion_Rewind(Quaternion.Euler(arg.Original_Vector3));
                }
            }
            else if (arg.Type == TweenNodeType.s_缩放)
            {
                if (arg.TweenValueMode == TweenValueMode.起始到默认_S_D ||
                    arg.TweenValueMode == TweenValueMode.起始到结束_S_E)
                {
                    controller.mod_Rect.localScale = arg.From_Vector3;
                    if (arg.Act_On_Vector3_Rewind != null)
                        arg.Act_On_Vector3_Rewind(arg.From_Vector3);
                }
                else if (arg.TweenValueMode == TweenValueMode.默认到结束_D_E)
                {
                    controller.mod_Rect.localScale = arg.Original_Vector3;
                    if (arg.Act_On_Vector3_Rewind != null)
                        arg.Act_On_Vector3_Rewind(arg.Original_Vector3);
                }
            }
            else if (arg.Type == TweenNodeType.c_颜色)
            {
                Graphic gc = controller.RecognizeType();
                if (gc == null)
                    return;

                if (arg.TweenValueMode == TweenValueMode.起始到默认_S_D ||
                    arg.TweenValueMode == TweenValueMode.起始到结束_S_E)
                {
                    gc.color = arg.From_Color;
                    if (arg.Act_On_Color_Rewind != null)
                        arg.Act_On_Color_Rewind(arg.From_Color);
                }
                else if (arg.TweenValueMode == TweenValueMode.默认到结束_D_E)
                {
                    gc.color = arg.Original_Color;
                    if (arg.Act_On_Color_Rewind != null)
                        arg.Act_On_Color_Rewind(arg.Original_Color);
                }
            }
            else if (arg.Type == TweenNodeType.g_淡化)
            {
                if (controller.mod_CanvasGroup == null)
                    return;

                if (arg.TweenValueMode == TweenValueMode.起始到默认_S_D ||
                    arg.TweenValueMode == TweenValueMode.起始到结束_S_E)
                {
                    controller.mod_CanvasGroup.alpha = arg.From_Float;
                    if (arg.Act_On_Float_Rewind != null)
                        arg.Act_On_Float_Rewind(arg.From_Float);
                }
                else if (arg.TweenValueMode == TweenValueMode.默认到结束_D_E)
                {
                    controller.mod_CanvasGroup.alpha = arg.Original_Float;
                    if (arg.Act_On_Float_Rewind != null)
                        arg.Act_On_Float_Rewind(arg.Original_Float);
                }
            }
            else if (arg.Type == TweenNodeType.w_打字机)
            {
                if (controller.mod_Text == null && controller.mod_TmpText == null)
                    return;

                if (arg.TweenValueMode == TweenValueMode.起始到默认_S_D ||
                    arg.TweenValueMode == TweenValueMode.起始到结束_S_E)
                {
                    if (controller.mod_Text != null)
                        controller.mod_Text.txt_Set_Content(arg.From_String);
                    if (controller.mod_TmpText != null)
                        controller.mod_TmpText.SetText(arg.From_String);
                    if (arg.Act_On_Text_Rewind != null)
                        arg.Act_On_Text_Rewind(arg.From_String);
                }
                else if (arg.TweenValueMode == TweenValueMode.默认到结束_D_E)
                {
                    if (controller.mod_Text != null)
                        controller.mod_Text.txt_Set_Content(arg.Original_String);
                    if (controller.mod_TmpText != null)
                        controller.mod_TmpText.SetText(arg.Original_String);
                    if (arg.Act_On_Text_Rewind != null)
                        arg.Act_On_Text_Rewind(arg.Original_String);
                }
            }
            else if (arg.Type == TweenNodeType.z_尺寸)
            {
                if (arg.TweenValueMode == TweenValueMode.起始到默认_S_D ||
                    arg.TweenValueMode == TweenValueMode.起始到结束_S_E)
                {
                    controller.mod_Rect.sizeDelta = arg.From_Vector2;
                    if (arg.Act_On_Vector2_Rewind != null)
                        arg.Act_On_Vector2_Rewind(arg.From_Vector2);
                }
                else if (arg.TweenValueMode == TweenValueMode.默认到结束_D_E)
                {
                    controller.mod_Rect.sizeDelta = arg.Original_Vector2;
                    if (arg.Act_On_Vector2_Rewind != null)
                        arg.Act_On_Vector2_Rewind(arg.Original_Vector2);
                }
            }
            else if (arg.Type == TweenNodeType.f_图像填充)
            {
                if (controller.mod_Image == null)
                    return;

                if (arg.TweenValueMode == TweenValueMode.起始到默认_S_D ||
                    arg.TweenValueMode == TweenValueMode.起始到结束_S_E)
                {
                    controller.mod_Image.fillAmount = arg.From_Float;
                    if (arg.Act_On_Float_Rewind != null)
                        arg.Act_On_Float_Rewind(arg.From_Float);
                }
                else if (arg.TweenValueMode == TweenValueMode.默认到结束_D_E)
                {
                    controller.mod_Image.fillAmount = arg.Original_Float;
                    if (arg.Act_On_Float_Rewind != null)
                        arg.Act_On_Float_Rewind(arg.Original_Float);
                }
            }


            // 复位中断状态标志，确保 Complete 事件在下次播放时能正常触发
            AnimatingBreakState = false;

            // 触发节点级 Rewind 事件（代码 + Inspector 双通道）
            if (act_on_Tween_Rewind != null)
                act_on_Tween_Rewind(arg);
            eve_on_Tween_Rewind.Invoke(arg);
        }

        /// <summary>
        /// 杀死动画（单个节点）
        /// 
        /// 【核心语义】
        /// 销毁动画实例，并重置相关状态：
        /// - 调用 Tweener.Kill() 停止播放
        /// - 将 node.Tweener 置为 null
        /// - 重置 Progress 与音效 IsPlayed
        /// - 触发节点级 Kill 事件
        /// 
        /// 【与 Tween_Rewind 的区别】
        /// - Rewind：恢复数值到起始状态（保留节点结构）
        /// - Kill  ：仅销毁实例，**不恢复数值**（节点数值停留在当前时刻）
        /// 
        /// 【使用场景】
        /// - 动画播放完毕后的清理
        /// - 重新创建动画前先杀死旧实例（Tween_Create 内部会调用）
        /// - 图元销毁前的资源释放
        /// 
        /// 【注意事项】
        /// - 杀死后 node.Tweener 为 null，再次调用 Tween_Play 会抛异常
        /// - 需要重新播放时，需先调用 Tween_Create 系列重新创建
        /// - 不恢复数值，如需恢复请用 Tween_Rewind
        /// </summary>
        /// <param name="arg">目标动画节点</param>
        /// <param name="complete">是否以"完成"语义杀死</param>
        public void Tween_Kill(TweenNode arg, bool complete = true)
        {
            #region 杀死动画
            // 空值检查：未创建 Tweener 时跳过
            if (arg.Tweener != null)
            {
                arg.Tweener.Kill(complete);
                arg.Tweener = null;   // 断开引用，防止悬空访问
            }
            #endregion

            // 触发节点级 Kill 事件（代码 + Inspector 双通道）
            if (act_on_Tween_Kill != null)
                act_on_Tween_Kill(arg);
            eve_on_Tween_Kill.Invoke(arg);
        }

        /// <summary>
        /// 检查列表中是否存在与指定节点重复的节点
        /// 
        /// 【判重规则】
        /// ID 与 Indicator **同时相同**才视为重复：
        /// - ID 相同但 Indicator 不同 → 不算重复（可能是复制后改名）
        /// - Indicator 相同但 ID 不同 → 不算重复（Indicator 允许重名）
        /// - 两者都相同 → 视为重复（真正的冲突）
        /// 
        /// 【执行流程】
        /// 1. 空值保护：node 或列表为 null 时返回 false
        /// 2. 遍历列表，跳过自身比较（引用相等）
        /// 3. 检查每项的 ID 和 Indicator 是否同时匹配
        /// 4. 命中即返回 true
        /// 
        /// 【使用场景】
        /// - 编辑器保存前做冲突检查
        /// - 复制节点时检测新 ID 是否冲突
        /// - 数据导入时的合法性校验
        /// 
        /// 【注意事项】
        /// - 跳过自身比较依赖引用相等（同一对象引用）
        /// - 若传入的 node 不在列表中，也不会误判为重复
        /// - ID 是唯一性核心，Indicator 只是辅助判断
        /// </summary>
        /// <param name="node">要检查的节点</param>
        /// <returns>true 表示存在重复，false 表示无重复</returns>
        public bool TweenNode_IsDuplicated(TweenNode node)
        {
            // 空值保护
            if (node == null || PrimitiveTweenNodes == null)
                return false;

            for (int i = 0; i < PrimitiveTweenNodes.Count; i++)
            {
                // 跳过自身比较（引用相等）
                if (PrimitiveTweenNodes[i] == node)
                    continue;

                // 同时检查 ID 与 Indicator 是否都相同
                if (PrimitiveTweenNodes[i].ID == node.ID && PrimitiveTweenNodes[i].Indicator == node.Indicator)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 查找与指定节点重复的节点（基于 ID + Indicator）
        /// 
        /// 【与 TweenNode_IsDuplicated 的区别】
        /// - IsDuplicated ：仅判断是否存在重复（返回 bool）
        /// - GetDuplicate ：返回重复的那个节点引用（返回 TweenNode）
        /// 
        /// 【判重规则】
        /// 与 IsDuplicated 一致：ID 与 Indicator 同时相同才视为重复
        /// 
        /// 【执行流程】
        /// 1. 空值保护
        /// 2. 遍历列表，跳过自身
        /// 3. 检查每项的 ID 和 Indicator 是否同时匹配
        /// 4. 命中即返回该节点引用
        /// 
        /// 【使用场景】
        /// - 编辑器自动修复冲突（定位冲突节点后重新生成 ID）
        /// - 需要向用户展示"与哪个节点冲突"的场景
        /// - 数据迁移时的冲突提示
        /// 
        /// 【注意事项】
        /// - 返回 null 表示无重复
        /// - 只返回首个匹配项，若存在多个重复需循环调用
        /// - 找到的节点可能不是"原始项"，仅表示"与之冲突的另一项"
        /// </summary>
        /// <param name="node">要检查的节点</param>
        /// <returns>重复的节点，若无重复返回 null</returns>
        public TweenNode TweenNode_GetDuplicate(TweenNode node)
        {
            // 空值保护
            if (node == null || PrimitiveTweenNodes == null)
                return null;

            for (int i = 0; i < PrimitiveTweenNodes.Count; i++)
            {
                // 跳过自身比较
                if (PrimitiveTweenNodes[i] == node)
                    continue;

                // 同时匹配 ID 与 Indicator
                if (PrimitiveTweenNodes[i].ID == node.ID && PrimitiveTweenNodes[i].Indicator == node.Indicator)
                    return PrimitiveTweenNodes[i];
            }

            return null;
        }

        #endregion
    }
}