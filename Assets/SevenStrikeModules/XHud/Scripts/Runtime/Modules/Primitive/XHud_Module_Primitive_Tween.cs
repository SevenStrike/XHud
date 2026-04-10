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
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.InputSystem.XR;
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
            newNode.ActivateFrom = this.ActivateFrom;
            newNode.From_Int = this.From_Int;
            newNode.From_Float = this.From_Float;
            newNode.From_Vector2 = this.From_Vector2;
            newNode.From_Vector3 = this.From_Vector3;
            newNode.From_Vector4 = this.From_Vector4;
            newNode.From_Color = this.From_Color;
            newNode.From_String = this.From_String;

            // ========== 结束值 ==========
            newNode.ActivateEnd = this.ActivateEnd;
            newNode.End_Int = this.End_Int;
            newNode.End_Float = this.End_Float;
            newNode.End_Vector2 = this.End_Vector2;
            newNode.End_Vector3 = this.End_Vector3;
            newNode.End_Vector4 = this.End_Vector4;
            newNode.End_Color = this.End_Color;
            newNode.End_String = this.End_String;

            // ========== 动画选项 ==========
            newNode.ActivateOnlyToEnd = this.ActivateOnlyToEnd;
            newNode.RotateMode = this.RotateMode;
            newNode.LoopType = this.LoopType;
            newNode.LoopCount = this.LoopCount;

            // ========== 音效列表（深度克隆）==========
            if (this.TweenSounds != null && this.TweenSounds.Count > 0)
            {
                newNode.TweenSounds = new List<TweenSound>();
                foreach (var sound in this.TweenSounds)
                {
                    TweenSound newSound = new TweenSound();
                    newSound.Sound = sound.Sound;           // AudioClip 是 UnityEngine.Object，引用即可
                    newSound.Path = sound.Path;
                    newSound.Percentage = sound.Percentage;
                    newSound.IsPlayed = false;               // 克隆后重置播放状态
                    newSound.Volume = sound.Volume;
                    newSound.MaxPitch = sound.MaxPitch;
                    newSound.MinPitch = sound.MinPitch;
                    // act_on_SoundPlay 委托不克隆，新节点需要重新绑定
                    newSound.act_on_SoundPlay = null;
                    newNode.TweenSounds.Add(newSound);
                }
            }

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
            this.ActivateFrom = source.ActivateFrom;
            this.From_Int = source.From_Int;
            this.From_Float = source.From_Float;
            this.From_Vector2 = source.From_Vector2;
            this.From_Vector3 = source.From_Vector3;
            this.From_Vector4 = source.From_Vector4;
            this.From_Color = source.From_Color;
            this.From_String = source.From_String;

            // 结束值
            this.ActivateEnd = source.ActivateEnd;
            this.End_Int = source.End_Int;
            this.End_Float = source.End_Float;
            this.End_Vector2 = source.End_Vector2;
            this.End_Vector3 = source.End_Vector3;
            this.End_Vector4 = source.End_Vector4;
            this.End_Color = source.End_Color;
            this.End_String = source.End_String;

            // 动画选项
            this.ActivateOnlyToEnd = source.ActivateOnlyToEnd;
            this.RotateMode = source.RotateMode;
            this.LoopType = source.LoopType;
            this.LoopCount = source.LoopCount;

            // 音效列表深度复制
            if (source.TweenSounds != null && source.TweenSounds.Count > 0)
            {
                this.TweenSounds = new List<TweenSound>();
                foreach (var sound in source.TweenSounds)
                {
                    TweenSound newSound = new TweenSound();
                    newSound.Sound = sound.Sound;
                    newSound.Path = sound.Path;
                    newSound.Percentage = sound.Percentage;
                    newSound.IsPlayed = false;
                    newSound.Volume = sound.Volume;
                    newSound.MaxPitch = sound.MaxPitch;
                    newSound.MinPitch = sound.MinPitch;
                    this.TweenSounds.Add(newSound);
                }
            }
            else
            {
                this.TweenSounds = null;
            }

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
        /// 图元控制器
        /// </summary>
        public XHud_Module_Primitive_Controller controller;

        #region 图元动画参数
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
        /// 预览动画开关
        /// </summary>
        [SerializeField] public bool TweenIsPreviewing;
        /// <summary>
        /// 动画节点
        /// </summary>
        [SerializeField] public List<TweenNode> PrimitiveTweenNodes = new List<TweenNode>();
        /// <summary>
        /// 起始动画节点
        /// </summary>
        [SerializeField] public string MainTweenNode;
        #endregion

        #region 成员 - 速率&时间
        /// <summary>
        /// 动画全局速率
        /// </summary>
        [SerializeField] public float GlobalDuration = 1;
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

        #region 成员 - 动作
        /// <summary>
        /// 动作 - 动画 - 播放 - 根据 XtweenInterface
        /// </summary>
        public UnityAction<XTween_Interface> act_on_Tween_Play;
        /// <summary>
        /// 动作 - 动画 - 播放 - 根据 TweenNode
        /// </summary>
        public UnityAction<TweenNode> act_on_Tween_Play_At_Node;
        /// <summary>
        /// 动作 - 动画 - 播放 - 根据 ID
        /// </summary>
        public UnityAction<int> act_on_Tween_Play_At_ID;
        /// <summary>
        /// 动作 - 动画 - 播放 - 根据 Indicator
        /// </summary>
        public UnityAction<string> act_on_Tween_Play_At_Indicator;
        /// <summary>
        /// 动作 - 动画 - 播放所有节点
        /// </summary>
        public UnityAction act_on_Tween_PlayAll;
        /// <summary>
        /// 动作 - 动画 - 创建 - 根据 ID
        /// </summary>
        public UnityAction<TweenNode> act_on_Tween_Create_At_ID;
        /// <summary>
        /// 动作 - 动画 - 创建 - 根据 Index
        /// </summary>
        public UnityAction<TweenNode> act_on_Tween_Create_At_Index;
        /// <summary>
        /// 动作 - 动画 - 创建 - 根据 Indicator
        /// </summary>
        public UnityAction<TweenNode> act_on_Tween_Create_At_Indicator;
        /// <summary>
        /// 动作 - 动画 - 复位
        /// </summary>
        public UnityAction<TweenNode> act_on_Tween_Rewind;
        /// <summary>
        /// 动作 - 动画 - 复位所有
        /// </summary>
        public UnityAction act_on_Tween_RewindAll;
        /// <summary>
        /// 动作 - 动画 - 杀死
        /// </summary>
        public UnityAction<TweenNode> act_on_Tween_Kill;
        /// <summary>
        /// 动作 - 动画 - 杀死所有
        /// </summary>
        public UnityAction act_on_Tween_KillAll;
        /// <summary>
        /// 动作 - 动画 - 准备
        /// </summary>
        public UnityAction act_on_Tween_Ready;
        /// <summary>
        /// 动作 - 动画 - 完成
        /// </summary>
        public UnityAction act_on_Tween_Complete;
        /// <summary>
        /// 动作 - 动画 - 初始化
        /// </summary>
        public UnityAction act_on_Tween_Initialized;
        /// <summary>
        /// 动作 - 动画 - 音效触发播放
        /// </summary>
        public UnityAction<AudioClip> act_on_Tween_SoundPlay;
        /// <summary>
        /// 动作 - 动画 - 动画播放状态
        /// </summary>
        public UnityAction<bool> act_on_Tween_IsAnimating;
        #endregion

        #region 成员 - 事件
        /// <summary>
        /// 事件 - 动画 - 播放 - 根据 XtweenInterface
        /// </summary>
        public UnityEvent<XTween_Interface> eve_on_Tween_Play;
        /// <summary>
        /// 事件 - 动画 - 播放 - 根据 TweenNode
        /// </summary>
        public UnityEvent<TweenNode> eve_on_Tween_Play_At_Node;
        /// <summary>
        /// 事件 - 动画 - 播放 - 根据 ID
        /// </summary>
        public UnityEvent<int> eve_on_Tween_Play_At_ID;
        /// <summary>
        /// 事件 - 动画 - 播放 - 根据 Indicator
        /// </summary>
        public UnityEvent<string> eve_on_Tween_Play_At_Indicator;
        /// <summary>
        /// 事件 - 动画 - 播放所有节点
        /// </summary>
        public UnityEvent eve_on_Tween_PlayAll;
        /// <summary>
        /// 事件 - 动画 -  创建 - 根据 ID
        /// </summary>
        public UnityEvent<TweenNode> eve_on_Tween_Create_At_ID;
        /// <summary>
        /// 事件 - 动画 -  创建 - 根据 Index
        /// </summary>
        public UnityEvent<TweenNode> eve_on_Tween_Create_At_Index;
        /// <summary>
        /// 事件 - 动画 -  创建 - 根据 Indicator
        /// </summary>
        public UnityEvent<TweenNode> eve_on_Tween_Create_At_Indicator;
        /// <summary>
        /// 事件 - 动画 -  复位
        /// </summary>
        public UnityEvent<TweenNode> eve_on_Tween_Rewind;
        /// <summary>
        /// 事件 - 动画 - 复位所有
        /// </summary>
        public UnityEvent eve_on_Tween_RewindAll;
        /// <summary>
        /// 事件 - 动画 -  杀死
        /// </summary>
        public UnityEvent<TweenNode> eve_on_Tween_Kill;
        /// <summary>
        /// 事件 - 动画 - 杀死所有
        /// </summary>
        public UnityEvent eve_on_Tween_KillAll;
        /// <summary>
        /// 事件 - 动画 - 准备
        /// </summary>
        public UnityEvent eve_on_Tween_Ready;
        /// <summary>
        /// 事件 - 动画 - 完成
        /// </summary>
        public UnityEvent eve_on_Tween_Complete;
        /// <summary>
        /// 事件 - 动画 - 初始化
        /// </summary>
        public UnityEvent eve_on_Tween_Initialized;
        /// <summary>
        /// 事件 - 动画 - 音效触发播放
        /// </summary>
        public UnityEvent<AudioClip> eve_on_Tween_SoundPlay;
        /// <summary>
        /// 事件 - 动画 - 动画播放状态
        /// </summary>
        public UnityEvent<bool> eve_on_Tween_AnimatingState;
        #endregion

        /// <summary>
        /// 动画协程
        /// </summary>
        Coroutine Coroutine_TweenPlayAll;

        #region 状态开关
        /// <summary>
        /// 调试开关
        /// </summary>
        [SerializeField] public bool Debug;
        #endregion

        #region 预览时机
        /// <summary>
        /// 预览时机
        /// </summary>
        [SerializeField] public string PreviewTiming;
        #endregion

        void Awake()
        {
            TweenNode_AllTweenProgress_Reset();

            if (act_on_Tween_Initialized != null)
                act_on_Tween_Initialized();
            eve_on_Tween_Initialized.Invoke();
        }

        void Start()
        {

        }

        void Update()
        {
            TweenNode_AllTweenProgress_Calculation();
            TweenNode_IsAllAnimating();
        }

        #region 动画节点列表操作
        /// <summary>
        /// 创建动画节点
        /// </summary>
        /// <returns></returns>
        public TweenNode TweenNode_Create()
        {
            TweenNode tn = new TweenNode();
            tn.ID = TweenNode_ID_Create();
            PrimitiveTweenNodes.Add(tn);
            return tn;
        }
        /// <summary>
        /// 创建动画节点
        /// </summary>
        /// <param name="node">动画节点参数</param>
        /// <returns></returns>
        public TweenNode TweenNode_Create(TweenNode node)
        {
            node.ID = TweenNode_ID_Create();
            PrimitiveTweenNodes.Add(node);
            return node;
        }
        /// <summary>
        /// 移除一个动画节点
        /// </summary>
        /// <param name="name">动画节点名称标识</param>
        public void TweenNode_Remove(string name)
        {
            for (int i = 0; i < PrimitiveTweenNodes.Count; i++)
            {
                if (PrimitiveTweenNodes[i].Indicator == name)
                {
                    PrimitiveTweenNodes.RemoveAt(i);
                }
            }
        }
        /// <summary>
        /// 移除一个动画节点
        /// </summary>
        /// <param name="id">动画节点ID</param>
        public void TweenNode_Remove(int id)
        {
            for (int i = 0; i < PrimitiveTweenNodes.Count; i++)
            {
                if (PrimitiveTweenNodes[i].ID == id)
                {
                    PrimitiveTweenNodes.RemoveAt(i);
                }
            }
        }
        /// <summary>
        /// 获取动画节点 - 根据动画节点ID
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TweenNode TweenNode_GetByIndex(int index)
        {
            return PrimitiveTweenNodes[index];
        }
        /// <summary>
        /// 获取动画节点 - 根据动画节点ID
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public TweenNode TweenNode_GetByID(int ID)
        {
            TweenNode node = null;
            for (int i = 0; i < PrimitiveTweenNodes.Count; i++)
            {
                if (PrimitiveTweenNodes[i].ID == ID)
                {
                    node = PrimitiveTweenNodes[i];
                    break;
                }
            }
            return node;
        }
        /// <summary>
        /// 获取动画节点 - 根据动画节点名称
        /// </summary>
        /// <param name="indicator"></param>
        /// <returns></returns>
        public TweenNode TweenNode_GetByIndicator(string indicator)
        {
            TweenNode node = null;
            for (int i = 0; i < PrimitiveTweenNodes.Count; i++)
            {
                if (PrimitiveTweenNodes[i].Indicator == indicator)
                {
                    node = PrimitiveTweenNodes[i];
                    break;
                }
            }
            return node;
        }
        /// <summary>
        /// 生成动画节点的唯一标识 ID
        /// 用于在动画节点列表中区分不同的节点，确保每个节点有独立的标识符
        /// 
        /// 工作原理：
        /// 1. 收集当前所有已存在的动画节点 ID，存入临时列表
        /// 2. 在 1111-9999 范围内生成一个随机数
        /// 3. 如果生成的 ID 与已有 ID 冲突，则将范围扩大到 111111-999999 重新生成
        /// 4. 重复直到生成一个不重复的 ID
        /// 
        /// 为什么需要唯一 ID？
        /// - 动画节点可能重名（Indicator 可以相同），但 ID 必须唯一
        /// - 外部系统通过 ID 来定位和操作特定的动画节点
        /// - 删除、修改、播放指定动画时都需要通过 ID 来识别目标节点
        /// 
        /// 使用场景：
        /// - 创建新动画节点时调用，自动分配唯一 ID
        /// - 编辑器中的“重新生成 ID”按钮调用
        /// - 复制/粘贴动画节点时重新生成 ID 避免冲突
        /// 
        /// 注意事项：
        /// - 随机数范围从 4 位数开始，冲突概率较低
        /// - 如果冲突，自动扩大到 6 位数，基本不会重复
        /// - 理论上在极端情况下（如已有 999999-111111 个节点）可能无限循环
        ///   但实际项目中动画节点数量通常不会超过 100 个，因此安全
        /// </summary>
        /// <returns>一个在当前动画节点列表中不重复的唯一整数 ID</returns>
        public int TweenNode_ID_Create()
        {
            List<int> ids = new List<int>();
            for (int i = 0; i < PrimitiveTweenNodes.Count; i++)
            {
                ids.Add(PrimitiveTweenNodes[i].ID);
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
        #endregion

        #region 获取
        /// <summary>
        /// 在自身寻找图元控制器
        /// </summary>
        public void FindController()
        {
            controller = transform.GetComponent<XHud_Module_Primitive_Controller>();
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
        /// </summary>
        public void TweenNode_GetTimers()
        {
            // 获取所有动画节点中的最短耗时
            MinTimer = TweenNode_GetTimerByType("Min");

            // 获取所有动画节点中的最长耗时
            MaxTimer = TweenNode_GetTimerByType("Max");

            // 应用动画器全局速率后的最短耗时
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
        /// </summary>
        /// <param name="type">指定获取类型："Min" 获取最小耗时，"Max" 获取最大耗时</param>
        /// <returns>计算出的耗时值（秒），如果节点列表为空则返回 0</returns>
        public float TweenNode_GetTimerByType(string type)
        {
            if (PrimitiveTweenNodes.Count <= 0)
            {
                if (type == "Min")
                    MinTimer = 0;
                if (type == "Max")
                    MaxTimer = 0;
                return 0;
            }

            List<float> timers = new List<float>();
            for (int i = 0; i < PrimitiveTweenNodes.Count; i++)
            {
                timers.Add(PrimitiveTweenNodes[i].Duration * Mathf.Clamp(PrimitiveTweenNodes[i].LoopCount, 1, int.MaxValue) + PrimitiveTweenNodes[i].Delay);
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
        /// 检测动画器是否正在播放动画
        /// 
        /// 工作原理：
        /// 1. 遍历所有动画节点（AnimateTweenNodes）
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
        /// - act_on_Tween_Complete：只在动画从播放变为停止时触发一次
        /// 
        /// 使用场景：
        /// - 外部系统判断动画是否还在播放（如等待所有动画完成）
        /// - UI 状态机管理（动画播放期间禁用交互）
        /// - 链式动画播放（一个动画结束后播放下一个）
        /// 
        /// 性能考虑：
        /// - 每帧在 Update 中调用，开销极小
        /// - 仅遍历动画节点列表，检查 Tweener 状态
        /// 
        /// </summary>
        /// <returns>true 表示至少有一个动画正在播放，false 表示所有动画都已停止</returns>
        public bool TweenNode_IsAllAnimating()
        {
            bool animating = false;

            // ========== 1. 检查是否有动画正在播放 ==========
            if (PrimitiveTweenNodes != null && PrimitiveTweenNodes.Count > 0)
            {
                for (int i = 0; i < PrimitiveTweenNodes.Count; i++)
                {
                    TweenNode node = PrimitiveTweenNodes[i];

                    // 检查该节点的动画是否正在播放
                    // Tweener != null：动画已创建
                    // Tweener.IsActive：动画未被杀死
                    // Tweener.IsPlaying：动画正在播放中
                    if (node.Tweener != null && node.Tweener.IsActive && node.Tweener.IsPlaying)
                    {
                        animating = true;
                        break;  // 只要有一个在播放，就可以提前退出循环
                    }
                }
            }

            // ========== 2. 触发动画播放状态事件（每帧） ==========
            if (act_on_Tween_IsAnimating != null)
                act_on_Tween_IsAnimating(animating);
            eve_on_Tween_AnimatingState.Invoke(animating);

            // ========== 3. 边缘检测：动画完成时触发完成事件 ==========
            if (animating)
            {
                // 正在播放：如果之前是停止状态，标记为播放中
                if (!AnimatingBreakState)
                {
                    AnimatingBreakState = true;
                }
            }
            else
            {
                // 已停止：如果之前是播放状态，触发完成事件
                if (AnimatingBreakState)
                {
                    AnimatingBreakState = false;  // 重置标志位

                    // 触发动画完成事件（只触发一次）
                    if (act_on_Tween_Complete != null)
                        act_on_Tween_Complete();
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

                // 步骤1：更新该节点的播放进度
                TweenNode_UpdateTweenProgress(node);

                // 步骤2：根据进度判断是否需要触发音效
                TweenNode_ProcessNodeSounds(node);
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

        #region 触发播放音效
        /// <summary>
        /// 处理单个动画节点的音效触发逻辑
        /// 
        /// 音效触发规则：
        /// 1. 每个动画节点可以配置多个音效触发点（TweenSound）
        /// 2. 触发点可以是 0-1 之间的任意百分比
        /// 3. 当动画进度 >= 触发点百分比时，播放对应的音效
        /// 4. 使用 IsPlayed 标志确保每个音效只触发一次
        /// 5. 当进度回退时（如动画被重置），IsPlayed 标志会被重置，允许再次触发
        /// 
        /// 特殊处理：
        /// - 循环动画（LoopCount = -1）不触发音效，避免无限重复播放
        /// - 触发点 >= 1 时，在动画完成时刻触发
        /// - 触发点 < 1 时，在进度超过触发点的瞬间触发
        /// 
        /// </summary>
        /// <param name="node">目标动画节点</param>
        private void TweenNode_ProcessNodeSounds(TweenNode node)
        {
            // 条件1：节点没有配置任何音效
            if (node.TweenSounds == null || node.TweenSounds.Count == 0)
                return;

            // 条件2：循环动画不触发音效（避免无限重复）
            if (node.LoopCount == -1)
                return;

            // 遍历该节点的所有音效触发点
            for (int i = 0; i < node.TweenSounds.Count; i++)
            {
                TweenSound sound = node.TweenSounds[i];

                // 跳过未配置音效剪辑的触发点
                if (sound.Sound == null)
                    continue;

                // 判断当前进度是否达到触发条件
                // - 触发点 >= 1：使用 >= 判断（动画完成时刻触发）
                // - 触发点 < 1：使用 > 判断（超过触发点的瞬间触发）
                bool shouldTrigger = sound.Percentage >= 1f
                    ? node.Progress >= sound.Percentage
                    : node.Progress > sound.Percentage;

                if (shouldTrigger)
                {
                    // 达到触发条件，且尚未播放过该音效
                    if (!sound.IsPlayed)
                    {
                        sound.IsPlayed = true;                      // 标记已播放，防止重复
                        PlayTweenSound(sound.Sound, sound.Volume, sound.MinPitch, sound.MaxPitch);
                        node.Act_On_SoundPlayed?.Invoke(sound.Sound); // 触发外部事件
                    }
                }
                else
                {
                    // 进度回退到触发点之前时，重置播放标志
                    // 例如：动画被 Rewind 或重新播放时，音效可以再次触发
                    sound.IsPlayed = false;
                }
            }
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
        /// 动画播放
        /// </summary>
        /// <param name="arg">目标动画节点</param>
        /// <param name="Duration">耗时</param>
        /// <param name="AnimationAction_Complete">动画播放完成后动作</param>
        /// <param name="AnimationAction_Percentage">动画进度状态动作</param>
        /// <param name="PercentageLimite">激活动画进度状态动作的阈值（最小0，最大1）</param>
        /// <returns></returns>
        public XTween_Interface Tween_Create(TweenNode arg, float Duration, UnityAction complete = null, UnityAction percentage = null, float PercentageLimite = 0.5f)
        {
            XTween_Interface twn = null;
            bool sw_From = arg.ActivateFrom;
            bool sw_End = arg.ActivateEnd;
            bool sw_OnlyToEnd = arg.ActivateOnlyToEnd;

            // 模式                |    使用场景                                 |       示例

            // 起始 → 默认    |    动画需要回退到原始状态         |      按钮悬停放大，离开时缩回原大小
            // 默认 → 结束    |    动画只改变到新状态不回退      |      进度条从当前值增加到目标值
            // 起始 → 结束    |    精确控制的往返动画                |      卡片翻转动画（从 A 角度到 B 角度）
            // 当前 → 结束    |    独立动画节点支持中断             |       连续点击触发不同动画，不互相干扰

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
             * 注意：“ActivateOnlyToEnd”此参数在PrimitiveTween的动画节点中的动向中设置，设置方法是将参数改为："当前 -> 结束"，一旦使用这种动向模式则动画不支持回退和编辑器下预览
            */
            if (!arg.ActivateOnlyToEnd)
            {
                Tween_Rewind(arg);
            }
            else
            {
                Tween_Kill(arg);
            }

            if (arg.Type == TweenNodeType.位移)
            {
                bool sw = false;
                //方式：起始 -> 默认
                if (sw_From && !sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.None)
                        twn = controller.mod_Rect.xt_AnchoredPosition3D_To(arg.Original_Vector3, arg.Duration * Duration, false, true, arg.Rewind_Set_Startvalue).SetFrom(arg.From_Vector3).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Vector3_Changed != null)
                                arg.Act_On_Vector3_Changed(controller.mod_Rect.anchoredPosition3D);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                            if (complete != null)
                                complete();
                        });
                    else
                        twn = controller.mod_Rect.xt_AnchoredPosition3D_To(arg.Original_Vector3, arg.Duration * Duration, false, true, arg.Rewind_Set_Startvalue).SetFrom(arg.From_Vector3).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Vector3_Changed != null)
                                arg.Act_On_Vector3_Changed(controller.mod_Rect.anchoredPosition3D);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                            if (complete != null)
                                complete();
                        });
                }
                //方式：默认 -> 结束
                else if (!sw_From && sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.None)
                        twn = controller.mod_Rect.xt_AnchoredPosition3D_To(arg.End_Vector3, arg.Duration * Duration, false, true, arg.Rewind_Set_Startvalue).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Vector3_Changed != null)
                                arg.Act_On_Vector3_Changed(controller.mod_Rect.anchoredPosition3D);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        twn = controller.mod_Rect.xt_AnchoredPosition3D_To(arg.End_Vector3, arg.Duration * Duration, false, true, arg.Rewind_Set_Startvalue).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Vector3_Changed != null)
                                arg.Act_On_Vector3_Changed(controller.mod_Rect.anchoredPosition3D);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：起始 -> 结束
                else if (sw_From && sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.None)
                        twn = controller.mod_Rect.xt_AnchoredPosition3D_To(arg.End_Vector3, arg.Duration * Duration, false, true, arg.Rewind_Set_Startvalue).SetFrom(arg.From_Vector3).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Vector3_Changed != null)
                                arg.Act_On_Vector3_Changed(controller.mod_Rect.anchoredPosition3D);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        twn = controller.mod_Rect.xt_AnchoredPosition3D_To(arg.End_Vector3, arg.Duration * Duration, false, true, arg.Rewind_Set_Startvalue).SetFrom(arg.From_Vector3).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Vector3_Changed != null)
                                arg.Act_On_Vector3_Changed(controller.mod_Rect.anchoredPosition3D);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：当前 -> 结束
                else if (!sw_From && !sw_End && sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.None)
                        twn = controller.mod_Rect.xt_AnchoredPosition3D_To(arg.End_Vector3, arg.Duration * Duration, false, true, arg.Rewind_Set_Startvalue).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Vector3_Changed != null)
                                arg.Act_On_Vector3_Changed(controller.mod_Rect.anchoredPosition3D);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        twn = controller.mod_Rect.xt_AnchoredPosition3D_To(arg.End_Vector3, arg.Duration * Duration, false, true, arg.Rewind_Set_Startvalue).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Vector3_Changed != null)
                                arg.Act_On_Vector3_Changed(controller.mod_Rect.anchoredPosition3D);
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
                    if (arg.Ease != EaseMode.None)
                        twn = controller.mod_Rect.xt_Rotate_To(arg.Original_Vector3, arg.Duration * Duration, false, true, XTweenRotationSpace.绝对, arg.RotateMode).SetFrom(arg.From_Vector3).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Quaternion_Changed != null)
                                arg.Act_On_Quaternion_Changed(controller.mod_Rect.rotation);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        twn = controller.mod_Rect.xt_Rotate_To(arg.Original_Vector3, arg.Duration * Duration, false, true, XTweenRotationSpace.绝对, arg.RotateMode).SetFrom(arg.From_Vector3).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Quaternion_Changed != null)
                                arg.Act_On_Quaternion_Changed(controller.mod_Rect.rotation);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：默认 -> 结束
                else if (!sw_From && sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.None)
                        twn = controller.mod_Rect.xt_Rotate_To(arg.End_Vector3, arg.Duration * Duration, false, true, XTweenRotationSpace.绝对, arg.RotateMode).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Quaternion_Changed != null)
                                arg.Act_On_Quaternion_Changed(controller.mod_Rect.rotation);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        twn = controller.mod_Rect.xt_Rotate_To(arg.End_Vector3, arg.Duration * Duration, false, true, XTweenRotationSpace.绝对, arg.RotateMode).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Quaternion_Changed != null)
                                arg.Act_On_Quaternion_Changed(controller.mod_Rect.rotation);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：起始 -> 结束
                else if (sw_From && sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.None)
                        twn = controller.mod_Rect.xt_Rotate_To(arg.End_Vector3, arg.Duration * Duration, false, true, XTweenRotationSpace.绝对, arg.RotateMode).SetFrom(arg.From_Vector3).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Quaternion_Changed != null)
                                arg.Act_On_Quaternion_Changed(controller.mod_Rect.rotation);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        twn = controller.mod_Rect.xt_Rotate_To(arg.End_Vector3, arg.Duration * Duration, false, true, XTweenRotationSpace.绝对, arg.RotateMode).SetFrom(arg.From_Vector3).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Quaternion_Changed != null)
                                arg.Act_On_Quaternion_Changed(controller.mod_Rect.rotation);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：当前 -> 结束
                else if (!sw_From && !sw_End && sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.None)
                        twn = controller.mod_Rect.xt_Rotate_To(arg.End_Vector3, arg.Duration * Duration, false, true, XTweenRotationSpace.绝对, arg.RotateMode).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Quaternion_Changed != null)
                                arg.Act_On_Quaternion_Changed(controller.mod_Rect.rotation);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        twn = controller.mod_Rect.xt_Rotate_To(arg.End_Vector3, arg.Duration * Duration, false, true, XTweenRotationSpace.绝对, arg.RotateMode).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Quaternion_Changed != null)
                                arg.Act_On_Quaternion_Changed(controller.mod_Rect.rotation);
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
                    if (arg.Ease != EaseMode.None)
                        twn = controller.mod_Rect.xt_Scale_To(arg.Original_Vector3, arg.Duration * Duration, false, true, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetFrom(arg.From_Vector3).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Vector3_Changed != null)
                                arg.Act_On_Vector3_Changed(controller.mod_Rect.localScale);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        twn = controller.mod_Rect.xt_Scale_To(arg.Original_Vector3, arg.Duration * Duration, false, true, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetFrom(arg.From_Vector3).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Vector3_Changed != null)
                                arg.Act_On_Vector3_Changed(controller.mod_Rect.localScale);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：默认 -> 结束
                else if (!sw_From && sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.None)
                        twn = controller.mod_Rect.xt_Scale_To(arg.End_Vector3, arg.Duration * Duration, false, true, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Vector3_Changed != null)
                                arg.Act_On_Vector3_Changed(controller.mod_Rect.localScale);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        twn = controller.mod_Rect.xt_Scale_To(arg.End_Vector3, arg.Duration * Duration, false, true, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Vector3_Changed != null)
                                arg.Act_On_Vector3_Changed(controller.mod_Rect.localScale);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：起始 -> 结束
                else if (sw_From && sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.None)
                        twn = controller.mod_Rect.xt_Scale_To(arg.End_Vector3, arg.Duration * Duration, false, true, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetFrom(arg.From_Vector3).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Vector3_Changed != null)
                                arg.Act_On_Vector3_Changed(controller.mod_Rect.localScale);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        twn = controller.mod_Rect.xt_Scale_To(arg.End_Vector3, arg.Duration * Duration, false, true, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetFrom(arg.From_Vector3).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Vector3_Changed != null)
                                arg.Act_On_Vector3_Changed(controller.mod_Rect.localScale);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：当前 -> 结束
                else if (!sw_From && !sw_End && sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.None)
                        twn = controller.mod_Rect.xt_Scale_To(arg.End_Vector3, arg.Duration * Duration, false, true, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Vector3_Changed != null)
                                arg.Act_On_Vector3_Changed(controller.mod_Rect.localScale);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        twn = controller.mod_Rect.xt_Scale_To(arg.End_Vector3, arg.Duration * Duration, false, true, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector3>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Vector3_Changed != null)
                                arg.Act_On_Vector3_Changed(controller.mod_Rect.localScale);
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

                /*  如果图元配色器处于Image & RawImage 颜色接管状态则直接返回空（接管状态下是无法通过动画改变颜色的）
                 *  可以选择在播放颜色动画前先关闭接管状态，如果有需要动画结束后再开启接管
                */
                if (controller.GetModuleType() == ModuleType.Image && controller.pt_Painting.SyncLibraryColor)
                    return null;
                if (controller.GetModuleType() == ModuleType.RawImage && controller.pt_Painting.SyncLibraryColor)
                    return null;

                Graphic gc = controller.RecognizeType();
                if (gc == null)
                    return null;
                //方式：起始 -> 默认
                if (sw_From && !sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.None)
                        twn = gc.xt_Color_To(arg.Original_Color, arg.Duration * Duration, true, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetFrom(arg.From_Color).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Color>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Color_Changed != null)
                                arg.Act_On_Color_Changed(gc.color);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        twn = gc.xt_Color_To(arg.Original_Color, arg.Duration * Duration, true, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetFrom(arg.From_Color).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Color>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
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
                    if (arg.Ease != EaseMode.None)
                        twn = gc.xt_Color_To(arg.End_Color, arg.Duration * Duration, true, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Color>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Color_Changed != null)
                                arg.Act_On_Color_Changed(gc.color);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        twn = gc.xt_Color_To(arg.End_Color, arg.Duration * Duration, true, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Color>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
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
                    if (arg.Ease != EaseMode.None)
                        twn = gc.xt_Color_To(arg.End_Color, arg.Duration * Duration, true, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetFrom(arg.From_Color).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Color>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Color_Changed != null)
                                arg.Act_On_Color_Changed(gc.color);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        twn = gc.xt_Color_To(arg.End_Color, arg.Duration * Duration, true, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetFrom(arg.From_Color).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Color>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
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
                    if (arg.Ease != EaseMode.None)
                        twn = gc.xt_Color_To(arg.End_Color, arg.Duration * Duration, true, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Color>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Color_Changed != null)
                                arg.Act_On_Color_Changed(gc.color);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        twn = gc.xt_Color_To(arg.End_Color, arg.Duration * Duration, true, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Color>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
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
                if (controller.mod_CanvasGroup == null)
                    return null;
                //方式：起始 -> 默认
                if (sw_From && !sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.None)
                        twn = controller.mod_CanvasGroup.xt_Alpha_To(arg.Original_Float, arg.Duration * Duration, true, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetFrom(arg.From_Float).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<float>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Float_Changed != null)
                                arg.Act_On_Float_Changed(controller.mod_Image.color.a);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        twn = controller.mod_CanvasGroup.xt_Alpha_To(arg.Original_Float, arg.Duration * Duration, true, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetFrom(arg.From_Float).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<float>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Float_Changed != null)
                                arg.Act_On_Float_Changed(controller.mod_Image.color.a);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：默认 -> 结束
                else if (!sw_From && sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.None)
                        twn = controller.mod_CanvasGroup.xt_Alpha_To(arg.End_Float, arg.Duration * Duration, true, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<float>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Float_Changed != null)
                                arg.Act_On_Float_Changed(controller.mod_Image.color.a);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        twn = controller.mod_CanvasGroup.xt_Alpha_To(arg.End_Float, arg.Duration * Duration, true, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<float>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Float_Changed != null)
                                arg.Act_On_Float_Changed(controller.mod_Image.color.a);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：起始 -> 结束
                else if (sw_From && sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.None)
                        twn = controller.mod_CanvasGroup.xt_Alpha_To(arg.End_Float, arg.Duration * Duration, true, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetFrom(arg.From_Float).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<float>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Float_Changed != null)
                                arg.Act_On_Float_Changed(controller.mod_Image.color.a);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        twn = controller.mod_CanvasGroup.xt_Alpha_To(arg.End_Float, arg.Duration * Duration, true, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetFrom(arg.From_Float).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<float>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Float_Changed != null)
                                arg.Act_On_Float_Changed(controller.mod_Image.color.a);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：当前 -> 结束
                else if (!sw_From && !sw_End && sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.None)
                        twn = controller.mod_CanvasGroup.xt_Alpha_To(arg.End_Float, arg.Duration * Duration, true, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<float>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Float_Changed != null)
                                arg.Act_On_Float_Changed(controller.mod_Image.color.a);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        twn = controller.mod_CanvasGroup.xt_Alpha_To(arg.End_Float, arg.Duration * Duration, true, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<float>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Float_Changed != null)
                                arg.Act_On_Float_Changed(controller.mod_Image.color.a);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
            }
            else if (arg.Type == TweenNodeType.打字机)
            {
                bool sw = false;
                if (controller.mod_Text == null && controller.mod_TmpText == null)
                {
                    return null;
                }

                //方式：起始 -> 默认
                if (sw_From && !sw_End && !sw_OnlyToEnd)
                {
                    if (controller.mod_Text != null)
                    {
                        if (arg.Ease != EaseMode.None)
                            twn = controller.mod_Text.xt_FontText_To(false, " |", arg.Original_String, arg.Duration * Duration, true, 0.5f, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetFrom(arg.From_String).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<string>((v, d, t) =>
                            {
                                arg.Progress = twn.ElapsedTime / twn.Duration;
                                if (twn.CurrentEasedProgress >= PercentageLimite)
                                {
                                    if (!sw)
                                    {
                                        sw = true;
                                        if (percentage != null)
                                            percentage();
                                    }
                                }
                                if (arg.Act_On_Text_Changed != null)
                                    arg.Act_On_Text_Changed(controller.mod_Text.text);
                            }).OnComplete((d) =>
                            {
                                arg.Progress = 0;
                            });
                        else
                            twn = controller.mod_Text.xt_FontText_To(false, " |", arg.Original_String, arg.Duration * Duration, true, 0.5f, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetFrom(arg.From_String).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<string>((v, d, t) =>
                            {
                                arg.Progress = twn.ElapsedTime / twn.Duration;
                                if (twn.CurrentEasedProgress >= PercentageLimite)
                                {
                                    if (!sw)
                                    {
                                        sw = true;
                                        if (percentage != null)
                                            percentage();
                                    }
                                }
                                if (arg.Act_On_Text_Changed != null)
                                    arg.Act_On_Text_Changed(controller.mod_Text.text);
                            }).OnComplete((d) =>
                            {
                                arg.Progress = 0;
                            });
                    }
                    else if (controller.mod_TmpText != null)
                    {
                        if (arg.Ease != EaseMode.None)
                            twn = controller.mod_TmpText.xt_FontText_To(false, arg.Original_String, arg.Duration * Duration, true, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetFrom(arg.From_String).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<string>((v, d, t) =>
                            {
                                arg.Progress = twn.ElapsedTime / twn.Duration;
                                if (twn.CurrentEasedProgress >= PercentageLimite)
                                {
                                    if (!sw)
                                    {
                                        sw = true;
                                        if (percentage != null)
                                            percentage();
                                    }
                                }
                                if (arg.Act_On_Text_Changed != null)
                                    arg.Act_On_Text_Changed(controller.mod_TmpText.text);
                            }).OnComplete((d) =>
                            {
                                arg.Progress = 0;
                            });
                        else
                            twn = controller.mod_TmpText.xt_FontText_To(false, arg.Original_String, arg.Duration * Duration, true, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetFrom(arg.From_String).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<string>((v, d, t) =>
                            {
                                arg.Progress = twn.ElapsedTime / twn.Duration;
                                if (twn.CurrentEasedProgress >= PercentageLimite)
                                {
                                    if (!sw)
                                    {
                                        sw = true;
                                        if (percentage != null)
                                            percentage();
                                    }
                                }
                                if (arg.Act_On_Text_Changed != null)
                                    arg.Act_On_Text_Changed(controller.mod_TmpText.text);
                            }).OnComplete((d) =>
                            {
                                arg.Progress = 0;
                            });
                    }
                }
                //方式：默认 -> 结束
                else if (!sw_From && sw_End && !sw_OnlyToEnd)
                {
                    if (controller.mod_Text != null)
                    {
                        if (arg.Ease != EaseMode.None)
                            twn = controller.mod_Text.xt_FontText_To(false, " |", arg.End_String, arg.Duration * Duration, true, 0.5f, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetFrom(arg.Original_String).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<string>((v, d, t) =>
                            {
                                arg.Progress = twn.ElapsedTime / twn.Duration;
                                if (twn.CurrentEasedProgress >= PercentageLimite)
                                {
                                    if (!sw)
                                    {
                                        sw = true;
                                        if (percentage != null)
                                            percentage();
                                    }
                                }
                                if (arg.Act_On_Text_Changed != null)
                                    arg.Act_On_Text_Changed(controller.mod_Text.text);
                            }).OnComplete((d) =>
                            {
                                arg.Progress = 0;
                            });
                        else
                            twn = controller.mod_Text.xt_FontText_To(false, " |", arg.End_String, arg.Duration * Duration, true, 0.5f, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetFrom(arg.Original_String).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<string>((v, d, t) =>
                            {
                                arg.Progress = twn.ElapsedTime / twn.Duration;
                                if (twn.CurrentEasedProgress >= PercentageLimite)
                                {
                                    if (!sw)
                                    {
                                        sw = true;
                                        if (percentage != null)
                                            percentage();
                                    }
                                }
                                if (arg.Act_On_Text_Changed != null)
                                    arg.Act_On_Text_Changed(controller.mod_Text.text);
                            }).OnComplete((d) =>
                            {
                                arg.Progress = 0;
                            });
                    }
                    else if (controller.mod_TmpText != null)
                    {
                        if (arg.Ease != EaseMode.None)
                            twn = controller.mod_TmpText.xt_FontText_To(false, arg.End_String, arg.Duration * Duration, true, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetFrom(arg.Original_String).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<string>((v, d, t) =>
                            {
                                arg.Progress = twn.ElapsedTime / twn.Duration;
                                if (twn.CurrentEasedProgress >= PercentageLimite)
                                {
                                    if (!sw)
                                    {
                                        sw = true;
                                        if (percentage != null)
                                            percentage();
                                    }
                                }
                                if (arg.Act_On_Text_Changed != null)
                                    arg.Act_On_Text_Changed(controller.mod_TmpText.text);
                            }).OnComplete((d) =>
                            {
                                arg.Progress = 0;
                            });
                        else
                            twn = controller.mod_TmpText.xt_FontText_To(false, arg.End_String, arg.Duration * Duration, true, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetFrom(arg.Original_String).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<string>((v, d, t) =>
                            {
                                arg.Progress = twn.ElapsedTime / twn.Duration;
                                if (twn.CurrentEasedProgress >= PercentageLimite)
                                {
                                    if (!sw)
                                    {
                                        sw = true;
                                        if (percentage != null)
                                            percentage();
                                    }
                                }
                                if (arg.Act_On_Text_Changed != null)
                                    arg.Act_On_Text_Changed(controller.mod_TmpText.text);
                            }).OnComplete((d) =>
                            {
                                arg.Progress = 0;
                            });
                    }
                }
                //方式：起始 -> 结束
                else if (sw_From && sw_End && !sw_OnlyToEnd)
                {
                    if (controller.mod_Text != null)
                    {
                        if (arg.Ease != EaseMode.None)
                            twn = controller.mod_Text.xt_FontText_To(false, " |", arg.End_String, arg.Duration * Duration, true, 0.5f, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetFrom(arg.From_String).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<string>((v, d, t) =>
                            {
                                arg.Progress = twn.ElapsedTime / twn.Duration;
                                if (twn.CurrentEasedProgress >= PercentageLimite)
                                {
                                    if (!sw)
                                    {
                                        sw = true;
                                        if (percentage != null)
                                            percentage();
                                    }
                                }
                                if (arg.Act_On_Text_Changed != null)
                                    arg.Act_On_Text_Changed(controller.mod_Text.text);
                            }).OnComplete((d) =>
                            {
                                arg.Progress = 0;
                            });
                        else
                            twn = controller.mod_Text.xt_FontText_To(false, " |", arg.End_String, arg.Duration * Duration, true, 0.5f, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetFrom(arg.From_String).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<string>((v, d, t) =>
                            {
                                arg.Progress = twn.ElapsedTime / twn.Duration;
                                if (twn.CurrentEasedProgress >= PercentageLimite)
                                {
                                    if (!sw)
                                    {
                                        sw = true;
                                        if (percentage != null)
                                            percentage();
                                    }
                                }
                                if (arg.Act_On_Text_Changed != null)
                                    arg.Act_On_Text_Changed(controller.mod_Text.text);
                            }).OnComplete((d) =>
                            {
                                arg.Progress = 0;
                            });
                    }
                    else if (controller.mod_TmpText != null)
                    {
                        if (arg.Ease != EaseMode.None)
                            twn = controller.mod_TmpText.xt_FontText_To(false, arg.End_String, arg.Duration * Duration, true, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetFrom(arg.From_String).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<string>((v, d, t) =>
                            {
                                arg.Progress = twn.ElapsedTime / twn.Duration;
                                if (twn.CurrentEasedProgress >= PercentageLimite)
                                {
                                    if (!sw)
                                    {
                                        sw = true;
                                        if (percentage != null)
                                            percentage();
                                    }
                                }
                                if (arg.Act_On_Text_Changed != null)
                                    arg.Act_On_Text_Changed(controller.mod_TmpText.text);
                            }).OnComplete((d) =>
                            {
                                arg.Progress = 0;
                            });
                        else
                            twn = controller.mod_TmpText.xt_FontText_To(false, arg.End_String, arg.Duration * Duration, true, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetFrom(arg.From_String).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<string>((v, d, t) =>
                            {
                                arg.Progress = twn.ElapsedTime / twn.Duration;
                                if (twn.CurrentEasedProgress >= PercentageLimite)
                                {
                                    if (!sw)
                                    {
                                        sw = true;
                                        if (percentage != null)
                                            percentage();
                                    }
                                }
                                if (arg.Act_On_Text_Changed != null)
                                    arg.Act_On_Text_Changed(controller.mod_TmpText.text);
                            }).OnComplete((d) =>
                            {
                                arg.Progress = 0;
                            });
                    }
                }
                //方式：当前 -> 结束
                else if (!sw_From && !sw_End && sw_OnlyToEnd)
                {
                    if (controller.mod_Text != null)
                    {
                        if (arg.Ease != EaseMode.None)
                            twn = controller.mod_Text.xt_FontText_To(false, " |", arg.End_String, arg.Duration * Duration, true).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<string>((v, d, t) =>
                            {
                                arg.Progress = twn.ElapsedTime / twn.Duration;
                                if (twn.CurrentEasedProgress >= PercentageLimite)
                                {
                                    if (!sw)
                                    {
                                        sw = true;
                                        if (percentage != null)
                                            percentage();
                                    }
                                }
                                if (arg.Act_On_Text_Changed != null)
                                    arg.Act_On_Text_Changed(controller.mod_Text.text);
                            }).OnComplete((d) =>
                            {
                                arg.Progress = 0;
                            });
                        else
                            twn = controller.mod_Text.xt_FontText_To(false, " |", arg.End_String, arg.Duration * Duration, true).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<string>((v, d, t) =>
                            {
                                arg.Progress = twn.ElapsedTime / twn.Duration;
                                if (twn.CurrentEasedProgress >= PercentageLimite)
                                {
                                    if (!sw)
                                    {
                                        sw = true;
                                        if (percentage != null)
                                            percentage();
                                    }
                                }
                                if (arg.Act_On_Text_Changed != null)
                                    arg.Act_On_Text_Changed(controller.mod_Text.text);
                            }).OnComplete((d) =>
                            {
                                arg.Progress = 0;
                            });
                    }
                    else if (controller.mod_TmpText != null)
                    {
                        if (arg.Ease != EaseMode.None)
                            twn = controller.mod_TmpText.xt_FontText_To(false, arg.End_String, arg.Duration * Duration, true).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<string>((v, d, t) =>
                            {
                                arg.Progress = twn.ElapsedTime / twn.Duration;
                                if (twn.CurrentEasedProgress >= PercentageLimite)
                                {
                                    if (!sw)
                                    {
                                        sw = true;
                                        if (percentage != null)
                                            percentage();
                                    }
                                }
                                if (arg.Act_On_Text_Changed != null)
                                    arg.Act_On_Text_Changed(controller.mod_TmpText.text);
                            }).OnComplete((d) =>
                            {
                                arg.Progress = 0;
                            });
                        else
                            twn = controller.mod_TmpText.xt_FontText_To(false, arg.End_String, arg.Duration * Duration, true).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<string>((v, d, t) =>
                            {
                                arg.Progress = twn.ElapsedTime / twn.Duration;
                                if (twn.CurrentEasedProgress >= PercentageLimite)
                                {
                                    if (!sw)
                                    {
                                        sw = true;
                                        if (percentage != null)
                                            percentage();
                                    }
                                }
                                if (arg.Act_On_Text_Changed != null)
                                    arg.Act_On_Text_Changed(controller.mod_TmpText.text);
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
                    if (arg.Ease != EaseMode.None)
                        twn = controller.mod_Rect.xt_Size_To(arg.Original_Vector2, arg.Duration * Duration, false, true, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetFrom(arg.From_Vector2).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector2>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Vector2_Changed != null)
                                arg.Act_On_Vector2_Changed(controller.mod_Rect.sizeDelta);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        twn = controller.mod_Rect.xt_Size_To(arg.Original_Vector2, arg.Duration * Duration, false, true, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetFrom(arg.From_Vector2).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector2>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Vector2_Changed != null)
                                arg.Act_On_Vector2_Changed(controller.mod_Rect.sizeDelta);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：默认 -> 结束
                else if (!sw_From && sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.None)
                        twn = controller.mod_Rect.xt_Size_To(arg.End_Vector2, arg.Duration * Duration, false, true, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector2>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Vector2_Changed != null)
                                arg.Act_On_Vector2_Changed(controller.mod_Rect.sizeDelta);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        twn = controller.mod_Rect.xt_Size_To(arg.End_Vector2, arg.Duration * Duration, false, true, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector2>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Vector2_Changed != null)
                                arg.Act_On_Vector2_Changed(controller.mod_Rect.sizeDelta);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：起始 -> 结束
                else if (sw_From && sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.None)
                        twn = controller.mod_Rect.xt_Size_To(arg.End_Vector2, arg.Duration * Duration, false, true, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetFrom(arg.From_Vector2).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector2>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Vector2_Changed != null)
                                arg.Act_On_Vector2_Changed(controller.mod_Rect.sizeDelta);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                    {
                        twn = controller.mod_Rect.xt_Size_To(arg.End_Vector2, arg.Duration * Duration, false, true, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetFrom(arg.From_Vector2).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector2>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Vector2_Changed != null)
                                arg.Act_On_Vector2_Changed(controller.mod_Rect.sizeDelta);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    }
                }
                //方式：当前 -> 结束
                else if (!sw_From && !sw_End && sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.None)
                        twn = controller.mod_Rect.xt_Size_To(arg.End_Vector2, arg.Duration * Duration, false, true, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector2>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Vector2_Changed != null)
                                arg.Act_On_Vector2_Changed(controller.mod_Rect.sizeDelta);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        twn = controller.mod_Rect.xt_Size_To(arg.End_Vector2, arg.Duration * Duration, false, true, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<Vector2>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Vector2_Changed != null)
                                arg.Act_On_Vector2_Changed(controller.mod_Rect.sizeDelta);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
            }
            else if (arg.Type == TweenNodeType.图像填充)
            {
                bool sw = false;
                if (controller.mod_Image == null)
                {
                    return null;
                }
                //方式：起始 -> 默认
                if (sw_From && !sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.None)
                        twn = XTween.To(() => controller.mod_Image.fillAmount, x => controller.mod_Image.fillAmount = x, Mathf.Clamp01(arg.Original_Float), arg.Duration * Duration, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetFrom(arg.From_Float).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<float>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Float_Changed != null)
                                arg.Act_On_Float_Changed(controller.mod_Image.fillAmount);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        twn = XTween.To(() => controller.mod_Image.fillAmount, x => controller.mod_Image.fillAmount = x, Mathf.Clamp01(arg.Original_Float), arg.Duration * Duration, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetFrom(arg.From_Float).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<float>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Float_Changed != null)
                                arg.Act_On_Float_Changed(controller.mod_Image.fillAmount);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：默认 -> 结束
                else if (!sw_From && sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.None)
                        twn = XTween.To(() => controller.mod_Image.fillAmount, x => controller.mod_Image.fillAmount = x, Mathf.Clamp01(arg.End_Float), arg.Duration * Duration, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<float>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Float_Changed != null)
                                arg.Act_On_Float_Changed(controller.mod_Image.fillAmount);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        twn = XTween.To(() => controller.mod_Image.fillAmount, x => controller.mod_Image.fillAmount = x, Mathf.Clamp01(arg.End_Float), arg.Duration * Duration, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<float>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Float_Changed != null)
                                arg.Act_On_Float_Changed(controller.mod_Image.fillAmount);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：起始 -> 结束
                else if (sw_From && sw_End && !sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.None)
                        twn = XTween.To(() => controller.mod_Image.fillAmount, x => controller.mod_Image.fillAmount = x, Mathf.Clamp01(arg.End_Float), arg.Duration * Duration, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetFrom(Mathf.Clamp01(arg.From_Float)).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<float>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Float_Changed != null)
                                arg.Act_On_Float_Changed(controller.mod_Image.fillAmount);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        twn = XTween.To(() => controller.mod_Image.fillAmount, x => controller.mod_Image.fillAmount = x, Mathf.Clamp01(arg.End_Float), arg.Duration * Duration, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetFrom(Mathf.Clamp01(arg.From_Float)).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<float>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Float_Changed != null)
                                arg.Act_On_Float_Changed(controller.mod_Image.fillAmount);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
                //方式：当前 -> 结束
                else if (!sw_From && !sw_End && sw_OnlyToEnd)
                {
                    if (arg.Ease != EaseMode.None)
                        twn = XTween.To(() => controller.mod_Image.fillAmount, x => controller.mod_Image.fillAmount = x, Mathf.Clamp01(arg.End_Float), arg.Duration * Duration, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetEase(arg.Ease).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<float>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Float_Changed != null)
                                arg.Act_On_Float_Changed(controller.mod_Image.fillAmount);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                    else
                        twn = XTween.To(() => controller.mod_Image.fillAmount, x => controller.mod_Image.fillAmount = x, Mathf.Clamp01(arg.End_Float), arg.Duration * Duration, arg.Rewind_Set_Startvalue, arg.Complete_Set_Endvalue).SetEase(arg.Curve).SetDelay(arg.Delay).SetLoop(arg.LoopCount, arg.LoopType).OnUpdate<float>((v, d, t) =>
                        {
                            arg.Progress = twn.ElapsedTime / twn.Duration;
                            if (twn.CurrentEasedProgress >= PercentageLimite)
                            {
                                if (!sw)
                                {
                                    sw = true;
                                    if (percentage != null)
                                        percentage();
                                }
                            }
                            if (arg.Act_On_Float_Changed != null)
                                arg.Act_On_Float_Changed(controller.mod_Image.fillAmount);
                        }).OnComplete((d) =>
                        {
                            arg.Progress = 0;
                        });
                }
            }

            return twn;
        }
        /// <summary>
        /// 根据索引号播放动画
        /// </summary>
        /// <param name="index">动画ID号</param>
        /// <param name="AnimationAction_Complete"></param>
        /// <param name="AnimationAction_Percentage"></param>
        /// <param name="PercentageLimite"></param>
        public XTween_Interface Tween_Create_At_Index(int index, UnityAction complete = null, UnityAction percentage = null, float PercentageLimite = 0.5f)
        {
            TweenNode arg = PrimitiveTweenNodes[index];

            arg.Tweener = Tween_Create(arg, XHud_Manager.Instance.DurationMultiply * GlobalDuration, complete, percentage, PercentageLimite);

            if (act_on_Tween_Create_At_Index != null)
                act_on_Tween_Create_At_Index(arg);
            eve_on_Tween_Create_At_Index.Invoke(arg);

            return arg.Tweener;
        }
        /// <summary>
        /// 根据ID号播放动画
        /// </summary>
        /// <param name="id">动画ID号</param>
        /// <param name="AnimationAction_Complete"></param>
        /// <param name="AnimationAction_Percentage"></param>
        /// <param name="PercentageLimite"></param>
        public XTween_Interface Tween_Create_At_ID(int id, UnityAction complete = null, UnityAction percentage = null, float PercentageLimite = 0.5f)
        {
            TweenNode arg = TweenNode_GetByID(id);

            arg.Tweener = Tween_Create(arg, XHud_Manager.Instance.DurationMultiply * GlobalDuration, complete, percentage, PercentageLimite);

            if (act_on_Tween_Create_At_ID != null)
                act_on_Tween_Create_At_ID(arg);
            eve_on_Tween_Create_At_ID.Invoke(arg);

            return arg.Tweener;
        }
        /// <summary>
        /// 根据标识名称播放动画
        /// </summary>
        /// <param name="indicator"></param>
        /// <param name="AnimationAction_Complete"></param>
        /// <param name="AnimationAction_Percentage"></param>
        /// <param name="PercentageLimite"></param>
        public XTween_Interface Tween_Create_At_Indicator(string indicator, UnityAction complete = null, UnityAction percentage = null, float PercentageLimite = 0.5f)
        {
            TweenNode arg = TweenNode_GetByIndicator(indicator);

            arg.Tweener = Tween_Create(arg, XHud_Manager.Instance.DurationMultiply * GlobalDuration, complete, percentage, PercentageLimite);

            if (act_on_Tween_Create_At_Indicator != null)
                act_on_Tween_Create_At_Indicator(arg);
            eve_on_Tween_Create_At_Indicator.Invoke(arg);

            return arg.Tweener;
        }
        #endregion

        #region 播放独立动画
        /// <summary>
        /// 动画播放
        /// </summary>
        public void Tween_Play(TweenNode node)
        {
            node.Tweener.Play();

            if (act_on_Tween_Play_At_Node != null)
                act_on_Tween_Play_At_Node(node);
            eve_on_Tween_Play_At_Node.Invoke(node);
        }
        /// <summary>
        /// 动画播放
        /// </summary>
        public void Tween_Play(XTween_Interface tween)
        {
            tween.Play();

            if (act_on_Tween_Play != null)
                act_on_Tween_Play(tween);
            eve_on_Tween_Play.Invoke(tween);
        }
        /// <summary>
        /// 动画播放
        /// </summary>
        public void Tween_Play(int id)
        {
            Tween_Create_At_ID(id).Play();

            if (act_on_Tween_Play_At_ID != null)
                act_on_Tween_Play_At_ID(id);
            eve_on_Tween_Play_At_ID.Invoke(id);
        }
        /// <summary>
        /// 动画播放
        /// </summary>
        public void Tween_Play(string indicator)
        {
            Tween_Create_At_Indicator(indicator).Play();

            if (act_on_Tween_Play_At_Indicator != null)
                act_on_Tween_Play_At_Indicator(indicator);
            eve_on_Tween_Play_At_Indicator.Invoke(indicator);
        }
        #endregion

        #region 播放全部动画
        /// <summary>
        /// 播放动画（批量所有）
        /// </summary>
        /// <param name="tim"></param>
        /// <param name="dur"></param>
        /// <param name="MatchTiming"></param>
        /// <param name="complete"></param>
        /// <param name="percentage"></param>
        /// <param name="PercentageLimite"></param>
        public void Tween_PlayAll_Forced(float dur = 1, bool MatchTiming = false, string tim = "", UnityAction complete = null, UnityAction percentage = null, float PercentageLimite = 0.5f, bool ForPreview = false)
        {
            ///--播放
            for (int i = 0; i < PrimitiveTweenNodes.Count; i++)
            {
                if (PrimitiveTweenNodes[i].Enabled)
                {
                    if (MatchTiming)
                    {
                        if (PrimitiveTweenNodes[i].Timings == tim)
                        {
                            TweenNode arg = PrimitiveTweenNodes[i];
                            arg.Tweener = Tween_Create(arg, (ForPreview ? XHud_Dashboard.HudManagerGet().DurationMultiply : XHud_Manager.Instance.DurationMultiply) * GlobalDuration * dur, complete, percentage, PercentageLimite);
                            arg.Tweener.Play();
                        }
                    }
                    else
                    {
                        TweenNode arg = PrimitiveTweenNodes[i];
                        arg.Tweener = Tween_Create(arg, ForPreview ? XHud_Dashboard.HudManagerGet().DurationMultiply : XHud_Manager.Instance.DurationMultiply * GlobalDuration * dur, complete, percentage, PercentageLimite);
                        arg.Tweener.Play();
                    }
                }
                else
                {
                    continue;
                }
            }

            if (act_on_Tween_PlayAll != null)
                act_on_Tween_PlayAll();
            eve_on_Tween_PlayAll.Invoke();
        }
        /// <summary>
        /// 动画播放（所有节点）
        /// </summary>
        /// <param name="tim"></param>
        /// <param name="delay"></param>
        /// <param name="dur"></param>
        /// <param name="MatchTiming"></param>
        /// <param name="complete"></param>
        /// <param name="percentage"></param>
        /// <param name="PercentageLimite"></param>
        public void Tween_PlayAll_WithDelay(float delay, float dur = 1, bool MatchTiming = false, string tim = "", UnityAction complete = null, UnityAction percentage = null, float PercentageLimite = 0.5f)
        {
            if (controller.mod_Rect == null)
                return;
            if (Coroutine_TweenPlayAll != null)
                Coroutine_TweenPlayAll = null;

            Coroutine_TweenPlayAll = StartCoroutine(Tween_PlayAll_Coroutine(tim, delay, dur, MatchTiming, complete, percentage, PercentageLimite));
        }
        /// <summary>
        /// 延迟播放动画（批量所有）
        /// </summary>
        /// <param name="tim">播放匹配条件</param>
        /// <param name="delay">延迟时间</param>
        /// <param name="dur">耗时</param>
        /// <param name="MatchTiming">是否启用匹配播放条件</param>
        /// <param name="complete">动画播放完成后动作</param>
        /// <param name="percentage">动画进度状态动作</param>
        /// <param name="PercentageLimite">激活动画进度状态动作的阈值（最小0，最大1）</param>
        private IEnumerator Tween_PlayAll_Coroutine(string tim, float delay = 0, float dur = 1, bool MatchTiming = true, UnityAction complete = null, UnityAction percentage = null, float PercentageLimite = 0.5f)
        {
            ///--延迟
            yield return new WaitForSeconds(delay);

            ///--播放
            for (int i = 0; i < PrimitiveTweenNodes.Count; i++)
            {
                if (PrimitiveTweenNodes[i].Enabled)
                {
                    if (MatchTiming)
                    {
                        if (PrimitiveTweenNodes[i].Timings == tim)
                        {
                            TweenNode arg = PrimitiveTweenNodes[i];
                            arg.Tweener = Tween_Create(arg, XHud_Manager.Instance.DurationMultiply * GlobalDuration * dur, complete, percentage, PercentageLimite);
                            arg.Tweener.Play();
                        }
                    }
                    else
                    {
                        TweenNode arg = PrimitiveTweenNodes[i];
                        arg.Tweener = Tween_Create(arg, XHud_Manager.Instance.DurationMultiply * GlobalDuration * dur, complete, percentage, PercentageLimite);
                        arg.Tweener.Play();
                    }
                }
                else
                {
                    continue;
                }
            }

            if (act_on_Tween_PlayAll != null)
                act_on_Tween_PlayAll();
            eve_on_Tween_PlayAll.Invoke();
        }
        #endregion

        #region 动画复位  /  杀死
        /// <summary>
        /// 动画重置（所有节点）
        /// </summary>
        public void Tween_RewindAll()
        {
            ///--停止所有协程
            if (Coroutine_TweenPlayAll != null)
            {
                StopCoroutine(Coroutine_TweenPlayAll);
                Coroutine_TweenPlayAll = null;
            }

            ///--动画恢复初始
            for (int i = 0; i < PrimitiveTweenNodes.Count; i++)
            {
                Tween_Rewind(PrimitiveTweenNodes[i]);
            }

            if (act_on_Tween_RewindAll != null)
                act_on_Tween_RewindAll();
            eve_on_Tween_RewindAll.Invoke();
        }
        /// <summary>
        /// 动画杀死（所有节点）
        /// </summary>
        public void Tween_KillAll()
        {
            ///--停止所有协程
            if (Coroutine_TweenPlayAll != null)
            {
                StopCoroutine(Coroutine_TweenPlayAll);
                Coroutine_TweenPlayAll = null;
            }

            ///--动画恢复初始
            for (int i = 0; i < PrimitiveTweenNodes.Count; i++)
            {
                Tween_Kill(PrimitiveTweenNodes[i]);
            }

            if (act_on_Tween_KillAll != null)
                act_on_Tween_KillAll();
            eve_on_Tween_KillAll.Invoke();
        }

        /*如果目标动画动向为：“当前 -> 结束”则不执行退回动画，
         * 因为此动向是指让动画直接到达目标值（没有起始值，如果有也只是他的当前值）
         * 所以倒退对其没有意义，其他动画方式可忽略此提示
         */

        /// <summary>
        /// 复位动画
        /// </summary>
        /// <param name="arg">目标动画节点</param>
        public void Tween_Rewind(TweenNode arg)
        {
            /*如果动画方式为：当前 -> 结束则不执行退回动画，因为此方式是指让动画直接到达目标值（没有起始值，如果有也只是他的当前值），所以倒退对其没有意义*/
            if (arg.ActivateOnlyToEnd)
                return;

            bool sw_From = arg.ActivateFrom;
            bool sw_End = arg.ActivateEnd;

            #region 杀死动画
            if (arg.Tweener != null)  // ← 添加空值检查
            {
                arg.Tweener.Kill();
                arg.Tweener.Rewind();
            }
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
                    controller.mod_Rect.anchoredPosition3D = arg.From_Vector3;
                    if (arg.Act_On_Vector3_Rewind != null)
                    {
                        arg.Act_On_Vector3_Rewind(arg.From_Vector3);
                    }
                }
                else if (sw_From && !sw_End)
                {
                    controller.mod_Rect.anchoredPosition3D = arg.From_Vector3;
                    if (arg.Act_On_Vector3_Rewind != null)
                    {
                        arg.Act_On_Vector3_Rewind(arg.From_Vector3);
                    }
                }
                else if (!sw_From && sw_End)
                {
                    controller.mod_Rect.anchoredPosition3D = arg.Original_Vector3;
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
                    controller.mod_Rect.rotation = Quaternion.Euler(arg.From_Vector3);
                    if (arg.Act_On_Quaternion_Rewind != null)
                    {
                        arg.Act_On_Quaternion_Rewind(Quaternion.Euler(arg.From_Vector3));
                    }
                }
                else if (sw_From && !sw_End)
                {
                    controller.mod_Rect.rotation = Quaternion.Euler(arg.From_Vector3);
                    if (arg.Act_On_Quaternion_Rewind != null)
                    {
                        arg.Act_On_Quaternion_Rewind(Quaternion.Euler(arg.From_Vector3));
                    }
                }
                else if (!sw_From && sw_End)
                {
                    controller.mod_Rect.rotation = Quaternion.Euler(arg.Original_Vector3);
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
                    controller.mod_Rect.localScale = arg.From_Vector3;
                    if (arg.Act_On_Vector3_Rewind != null)
                    {
                        arg.Act_On_Vector3_Rewind(arg.From_Vector3);
                    }
                }
                else if (sw_From && !sw_End)
                {
                    controller.mod_Rect.localScale = arg.From_Vector3;
                    if (arg.Act_On_Vector3_Rewind != null)
                    {
                        arg.Act_On_Vector3_Rewind(arg.From_Vector3);
                    }
                }
                else if (!sw_From && sw_End)
                {
                    controller.mod_Rect.localScale = arg.Original_Vector3;
                    if (arg.Act_On_Vector3_Rewind != null)
                    {
                        arg.Act_On_Vector3_Rewind(arg.Original_Vector3);
                    }
                }
            }
            else if (arg.Type == TweenNodeType.颜色)
            {
                Graphic gc = controller.RecognizeType();
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
                if (controller.mod_CanvasGroup == null)
                {
                    return;
                }
                if (sw_From && sw_End)
                {
                    controller.mod_CanvasGroup.alpha = arg.From_Float;
                    if (arg.Act_On_Float_Rewind != null)
                    {
                        arg.Act_On_Float_Rewind(arg.From_Float);
                    }
                }
                else if (sw_From && !sw_End)
                {
                    controller.mod_CanvasGroup.alpha = arg.From_Float;
                    if (arg.Act_On_Float_Rewind != null)
                    {
                        arg.Act_On_Float_Rewind(arg.From_Float);
                    }
                }
                else if (!sw_From && sw_End)
                {
                    controller.mod_CanvasGroup.alpha = arg.Original_Float;
                    if (arg.Act_On_Float_Rewind != null)
                    {
                        arg.Act_On_Float_Rewind(arg.Original_Float);
                    }
                }
            }
            else if (arg.Type == TweenNodeType.打字机)
            {
                if (controller.mod_Text == null && controller.mod_TmpText == null)
                {
                    return;
                }
                else
                {
                    if (sw_From && sw_End)
                    {
                        if (controller.mod_Text != null)
                        {
                            controller.mod_Text.txt_Set_Content(arg.From_String);
                        }
                        if (controller.mod_TmpText != null)
                        {
                            controller.mod_TmpText.SetText(arg.From_String);
                        }
                        if (arg.Act_On_Text_Rewind != null)
                        {
                            arg.Act_On_Text_Rewind(arg.From_String);
                        }
                    }
                    else if (sw_From && !sw_End)
                    {
                        if (controller.mod_Text != null)
                        {
                            controller.mod_Text.txt_Set_Content(arg.From_String);
                        }
                        if (controller.mod_TmpText != null)
                        {
                            controller.mod_TmpText.SetText(arg.From_String);
                        }
                        if (arg.Act_On_Text_Rewind != null)
                        {
                            arg.Act_On_Text_Rewind(arg.From_String);
                        }
                    }
                    else if (!sw_From && sw_End)
                    {
                        if (controller.mod_Text != null)
                        {
                            controller.mod_Text.txt_Set_Content(arg.Original_String);
                        }
                        if (controller.mod_TmpText != null)
                        {
                            controller.mod_TmpText.SetText(arg.Original_String);
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
                    controller.mod_Rect.sizeDelta = arg.From_Vector2;
                    if (arg.Act_On_Vector2_Rewind != null)
                    {
                        arg.Act_On_Vector2_Rewind(arg.From_Vector2);
                    }
                }
                else if (sw_From && !sw_End)
                {
                    controller.mod_Rect.sizeDelta = arg.From_Vector2;
                    if (arg.Act_On_Vector2_Rewind != null)
                    {
                        arg.Act_On_Vector2_Rewind(arg.From_Vector2);
                    }
                }
                else if (!sw_From && sw_End)
                {
                    controller.mod_Rect.sizeDelta = arg.Original_Vector2;
                    if (arg.Act_On_Vector2_Rewind != null)
                    {
                        arg.Act_On_Vector2_Rewind(arg.Original_Vector2);
                    }
                }
            }
            else if (arg.Type == TweenNodeType.图像填充)
            {
                if (controller.mod_Image == null)
                {
                    return;
                }
                if (sw_From && sw_End)
                {
                    controller.mod_Image.fillAmount = arg.From_Float;
                    if (arg.Act_On_Float_Rewind != null)
                    {
                        arg.Act_On_Float_Rewind(arg.From_Float);
                    }
                }
                else if (sw_From && !sw_End)
                {
                    controller.mod_Image.fillAmount = arg.From_Float;
                    if (arg.Act_On_Float_Rewind != null)
                    {
                        arg.Act_On_Float_Rewind(arg.From_Float);
                    }
                }
                else if (!sw_From && sw_End)
                {
                    controller.mod_Image.fillAmount = arg.Original_Float;
                    if (arg.Act_On_Float_Rewind != null)
                    {
                        arg.Act_On_Float_Rewind(arg.Original_Float);
                    }
                }
            }

            if (act_on_Tween_Rewind != null)
                act_on_Tween_Rewind(arg);
            eve_on_Tween_Rewind.Invoke(arg);
        }
        /// <summary>
        /// 杀死动画
        /// </summary>
        /// <param name="arg">目标动画节点</param>
        public void Tween_Kill(TweenNode arg)
        {
            #region 杀死动画
            if (arg.Tweener != null)  // ← 添加空值检查
            {
                arg.Tweener.Kill();
            }
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

            if (act_on_Tween_Kill != null)
                act_on_Tween_Kill(arg);
            eve_on_Tween_Kill.Invoke(arg);
        }

        /// <summary>
        /// 检查列表中是否存在相同ID的节点（ID应该唯一）
        /// </summary>
        /// <param name="node">要检查的节点</param>
        /// <returns>true 表示存在重复，false 表示无重复</returns>
        public bool TweenNode_IsRepeat(TweenNode node)
        {
            if (node == null || PrimitiveTweenNodes == null)
                return false;

            for (int i = 0; i < PrimitiveTweenNodes.Count; i++)
            {
                // 跳过自身比较
                if (PrimitiveTweenNodes[i] == node)
                    continue;

                // 检查ID是否重复
                if (PrimitiveTweenNodes[i].ID == node.ID && PrimitiveTweenNodes[i].Indicator == node.Indicator)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 查找与指定节点重复的节点（基于ID）
        /// </summary>
        /// <param name="node">要检查的节点</param>
        /// <returns>重复的节点，如果没有重复则返回 null</returns>
        public TweenNode TweenNode_GetRepeat(TweenNode node)
        {
            if (node == null || PrimitiveTweenNodes == null)
                return null;

            for (int i = 0; i < PrimitiveTweenNodes.Count; i++)
            {
                // 跳过自身比较
                if (PrimitiveTweenNodes[i] == node)
                    continue;

                // 检查ID是否重复
                if (PrimitiveTweenNodes[i].ID == node.ID && PrimitiveTweenNodes[i].Indicator == node.Indicator)
                    return PrimitiveTweenNodes[i];
            }

            return null;
        }
        #endregion
    }
}