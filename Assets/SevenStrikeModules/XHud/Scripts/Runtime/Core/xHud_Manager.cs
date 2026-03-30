namespace SevenStrikeModules.XHud
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using SevenStrikeModules.XHud.Enums;
    using UnityEngine.UI;
    using UnityEngine.Events;
    using SevenStrikeModules.XTween;
    using UnityEngine.Rendering.Universal;
    using SevenStrikeModules.XHud.Utilitys;
    using System;
#if UNITY_EDITOR
    using UnityEditor;
#endif
    using UnityEngine.EventSystems;
    using UnityEngine.Rendering;
    using Unified.UniversalBlur.Runtime;
    using Image = UnityEngine.UI.Image;
    using UnityEngine.InputSystem.UI;
    using TMPro;

    #region CustomClass

    /// <summary>
    /// 元素信息包
    /// </summary>
    [System.Serializable]
    public class HudElementNode
    {
        /// <summary>
        /// 标识名称
        /// </summary>
        public string Indicator;
        /// <summary>
        /// 模块名称
        /// </summary>
        public string ModuleName;
        /// <summary>
        /// 模块ID编号
        /// </summary>
        public int ID;
        /// <summary>
        /// Hud元素
        /// </summary>
        public XHud_Module_Element Element;
        /// <summary>
        /// 动画状态
        /// </summary>
        public bool IsAnimating;
    }

    /// <summary>
    /// 锚点结构 - 布局
    /// </summary>
    [System.Serializable]
    public class Anchor_Layout
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 锚点
        /// </summary>
        public RectTransform Anchor;
        /// <summary>
        /// 锚点类型
        /// </summary>
        public XHudAnchor Type;
        /// <summary>
        /// 标记物
        /// </summary>
        public Image Mark;
        /// <summary>
        /// 元素项
        /// </summary>
        public List<HudElementNode> HudElementInfos;

        /// <summary>
        /// 实例化锚点节点
        /// </summary>
        /// <param name="Name"></param>
        public Anchor_Layout(string Name)
        {
            switch (Name)
            {
                case "Anchor_U":
                    Type = XHudAnchor.上;
                    break;
                case "Anchor_D":
                    Type = XHudAnchor.下;
                    break;
                case "Anchor_L":
                    Type = XHudAnchor.左;
                    break;
                case "Anchor_R":
                    Type = XHudAnchor.右;
                    break;
                case "Anchor_C":
                    Type = XHudAnchor.中心;
                    break;
                case "Anchor_L_U":
                    Type = XHudAnchor.左上;
                    break;
                case "Anchor_L_D":
                    Type = XHudAnchor.左下;
                    break;
                case "Anchor_R_U":
                    Type = XHudAnchor.右上;
                    break;
                case "Anchor_R_D":
                    Type = XHudAnchor.右下;
                    break;
                case "Anchor_B":
                    Type = XHudAnchor.底层;
                    break;
                case "Anchor_T":
                    Type = XHudAnchor.顶层;
                    break;
            }
            this.Name = "锚点： " + Type.ToString();
        }

        /// <summary>
        /// 实例化锚点节点
        /// </summary>
        public Anchor_Layout()
        {
            this.Name = "锚点： " + Type.ToString();
        }
    }

    /// <summary>
    /// 锚点结构 - 布局构图节点
    /// </summary>
    [System.Serializable]
    public class AuxiliaryAnchor_Layout
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 锚点
        /// </summary>
        public RectTransform Anchor;
        /// <summary>
        /// 锚点类型
        /// </summary>
        public HudAnchors_CompGuide Type;
    }

    /// <summary>
    /// 元素创建参数
    /// </summary>
    [System.Serializable]
    public class Motion_Creator
    {
        /// <summary>
        /// 目标固定锚点
        /// </summary>
        public XHudAnchor anchor = XHudAnchor.中心;
        public MotionAnimateEndState MotionAnimateEndState = MotionAnimateEndState.以_透明度为准;
        public MotionNode_Alpha Alpha = new MotionNode_Alpha();
        public MotionNode_Movement Movement = new MotionNode_Movement();
        public MotionNode_Rotation Rotation = new MotionNode_Rotation();

        /// <summary>
        /// 初始化元素参数
        /// </summary>
        /// <param name="m_anchortype">目标固定锚点</param>
        /// <param name="m_movement">运动样式</param>
        /// <param name="m_distance">位移距离</param>
        /// <param name="m_movement_duration">动画速度 - 位移</param>
        /// <param name="m_movement_delay">动画延迟 - 位移</param>
        /// <param name="m_movement_curve">位移运动曲线</param>
        /// <param name="m_movement_ease">位移运动缓动参数</param>
        /// <param name="m_alpha_duration">动画速度 - 透明度_Alpha</param>
        /// <param name="m_alpha_delay">动画延迟 - 透明度_Alpha</param>
        /// <param name="m_alpha_curve">透明度变化曲线</param>
        /// <param name="m_alpha_ease">透明度变化缓动参数</param>
        /// <param name="m_rotation">旋转样式</param>
        /// <param name="m_degree">旋转角度</param>
        /// <param name="m_rotation_duration">动画速度 - 旋转_Rotation</param>
        /// <param name="m_rotation_delay">动画延迟 - 旋转_Rotation</param>
        /// <param name="m_rotation_curve">旋转运动曲线</param>
        /// <param name="m_rotation_ease">旋转运动缓动参数</param>
        public Motion_Creator(XHudAnchor m_anchortype = XHudAnchor.中心, HudMotion_Movement m_movement = HudMotion_Movement.D_从上至下, float m_distance = 100, float m_movement_duration = 1f, float m_movement_delay = 0f, AnimationCurve m_movement_curve = null, string m_movement_curve_name = "", EaseMode m_movement_ease = EaseMode.InOutCubic, float m_alpha_duration = 1f, float m_alpha_delay = 0f, AnimationCurve m_alpha_curve = null, string m_alpha_curve_name = "", EaseMode m_alpha_ease = EaseMode.InOutCubic, HudMotion_Rotation m_rotation = HudMotion_Rotation.A_无旋转, float m_degree = 0, float m_rotation_duration = 1f, float m_rotation_delay = 0f, AnimationCurve m_rotation_curve = null, string m_rotation_curve_name = "", EaseMode m_rotation_ease = EaseMode.InOutCubic)
        {
            anchor = m_anchortype;

            Movement = new MotionNode_Movement();
            Movement.Movement = m_movement;
            Movement.Distance = m_distance;
            Movement.Duration = m_movement_duration;
            Movement.Delay = m_movement_delay;
            Movement.Curve = m_movement_curve;
            Movement.CurveName = m_movement_curve_name;
            Movement.Ease = m_movement_ease;

            Rotation = new MotionNode_Rotation();
            Rotation.Rotation = m_rotation;
            Rotation.Degree = m_degree;
            Rotation.Duration = m_rotation_duration;
            Rotation.Delay = m_rotation_delay;
            Rotation.Curve = m_rotation_curve;
            Rotation.CurveName = m_rotation_curve_name;
            Rotation.Ease = m_rotation_ease;

            Alpha = new MotionNode_Alpha();
            Alpha.Duration = m_alpha_duration;
            Alpha.Delay = m_alpha_delay;
            Alpha.Curve = m_alpha_curve;
            Alpha.CurveName = m_alpha_curve_name;
            Alpha.Ease = m_alpha_ease;
        }
    }

    /// <summary>
    /// 元素回收参数
    /// </summary>
    [System.Serializable]
    public class Motion_Recycler
    {
        public MotionAnimateEndState MotionAnimateEndState = MotionAnimateEndState.以_透明度为准;
        public MotionNode_Alpha Alpha;
        public MotionNode_Movement Movement;
        public MotionNode_Rotation Rotation;

        /// <summary>
        /// 初始化元素参数
        /// </summary>
        /// <param name="m_movement">运动样式</param>
        /// <param name="m_distance">位移距离</param>
        /// <param name="m_movement_duration">动画速度 - 位移</param>
        /// <param name="m_movement_delay">动画延迟 - 位移</param>
        /// <param name="m_movement_curve">位移运动曲线</param>
        /// <param name="m_movement_ease">位移运动缓动参数</param>
        /// <param name="m_alpha_duration">动画速度 - 透明度_Alpha</param>
        /// <param name="m_alpha_delay">动画延迟 - 透明度_Alpha</param>
        /// <param name="m_alpha_curve">透明度变化曲线</param>
        /// <param name="m_alpha_ease">透明度变化缓动参数</param>
        /// <param name="m_rotation">旋转样式</param>
        /// <param name="m_degree">旋转角度</param>
        /// <param name="m_rotation_duration">动画速度 - 旋转_Rotation</param>
        /// <param name="m_rotation_delay">动画延迟 - 旋转_Rotation</param>
        /// <param name="m_rotation_curve">旋转运动曲线</param>
        /// <param name="m_rotation_ease">旋转运动缓动参数</param>
        public Motion_Recycler(HudMotion_Movement m_movement = HudMotion_Movement.D_从上至下, float m_distance = 100, float m_movement_duration = 1f, float m_movement_delay = 0f, AnimationCurve m_movement_curve = null, string m_movement_curve_index = "", EaseMode m_movement_ease = EaseMode.InOutCubic, float m_alpha_duration = 1f, float m_alpha_delay = 0f, AnimationCurve m_alpha_curve = null, string m_alpha_curve_index = "", EaseMode m_alpha_ease = EaseMode.InOutCubic, HudMotion_Rotation m_rotation = HudMotion_Rotation.A_无旋转, float m_degree = 0, float m_rotation_duration = 1f, float m_rotation_delay = 0f, AnimationCurve m_rotation_curve = null, string m_rotation_curve_index = "", EaseMode m_rotation_ease = EaseMode.InOutCubic)
        {
            Movement = new MotionNode_Movement();
            Movement.Movement = m_movement;
            Movement.Distance = m_distance;
            Movement.Duration = m_movement_duration;
            Movement.Delay = m_movement_delay;
            Movement.Curve = m_movement_curve;
            Movement.CurveName = m_movement_curve_index;
            Movement.Ease = m_movement_ease;

            Rotation = new MotionNode_Rotation();
            Rotation.Rotation = m_rotation;
            Rotation.Degree = m_degree;
            Rotation.Duration = m_rotation_duration;
            Rotation.Delay = m_rotation_delay;
            Rotation.Curve = m_rotation_curve;
            Rotation.CurveName = m_rotation_curve_index;
            Rotation.Ease = m_rotation_ease;

            Alpha = new MotionNode_Alpha();
            Alpha.Duration = m_alpha_duration;
            Alpha.Delay = m_alpha_delay;
            Alpha.Curve = m_alpha_curve;
            Alpha.CurveName = m_alpha_curve_index;
            Alpha.Ease = m_alpha_ease;
        }
    }

    [System.Serializable]
    public class MotionNode_Movement
    {
        /// <summary>
        /// 运动样式 - 位移
        /// </summary>
        public HudMotion_Movement Movement = HudMotion_Movement.A_无运动;
        /// <summary>
        /// 位移距离
        /// </summary>
        public float Distance = 100;
        /// <summary>
        /// 动画速度 - 位移
        /// </summary>
        public float Duration = 1f;
        /// <summary>
        /// 动画延迟 - 位移
        /// </summary>
        public float Delay = 0f;
        /// <summary>
        /// 位移运动曲线
        /// </summary>
        public AnimationCurve Curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        /// <summary>
        /// 位移运动曲线索引
        /// </summary>
        public string CurveName = "";
        /// <summary>
        /// 位移运动缓动参数
        /// </summary>
        public EaseMode Ease = EaseMode.OutQuart;

        public void CopyData(MotionNode_Movement original)
        {
            Movement = original.Movement;
            Distance = original.Distance;
            Duration = original.Duration;
            Delay = original.Delay;
            Curve = original.Curve;
            CurveName = original.CurveName;
            Ease = original.Ease;
        }
    }

    [System.Serializable]
    public class MotionNode_Rotation
    {
        /// <summary>
        /// 运动样式 - 旋转_Rotation
        /// </summary>
        public HudMotion_Rotation Rotation = HudMotion_Rotation.A_无旋转;
        /// <summary>
        /// 角度
        /// </summary>
        public float Degree = 0;
        /// <summary>
        /// 动画速度 - 旋转_Rotation
        /// </summary>
        public float Duration = 1f;
        /// <summary>
        /// 动画延迟 - 旋转_Rotation
        /// </summary>
        public float Delay = 0f;
        /// <summary>
        /// 旋转运动曲线
        /// </summary>
        public AnimationCurve Curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        /// <summary>
        /// 旋转运动曲线索引
        /// </summary>
        public string CurveName = "";
        /// <summary>
        /// 旋转运动缓动参数
        /// </summary>
        public EaseMode Ease = EaseMode.OutQuart;

        public void CopyData(MotionNode_Rotation original)
        {
            Rotation = original.Rotation;
            Degree = original.Degree;
            Duration = original.Duration;
            Delay = original.Delay;
            Curve = original.Curve;
            CurveName = original.CurveName;
            Ease = original.Ease;
        }
    }

    [System.Serializable]
    public class MotionNode_Alpha
    {
        /// <summary>
        /// 动画速度 - 透明度_Alpha
        /// </summary>
        public float Duration = 1f;
        /// <summary>
        /// 动画延迟 - 透明度_Alpha
        /// </summary>
        public float Delay = 0f;
        /// <summary>
        /// 透明度变化曲线
        /// </summary>
        public AnimationCurve Curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        /// <summary>
        /// 透明度运动曲线索引
        /// </summary>
        public string CurveName = "";
        /// <summary>
        /// 透明度变化缓动参数
        /// </summary>
        public EaseMode Ease = EaseMode.OutQuart;

        public void CopyData(MotionNode_Alpha original)
        {
            Duration = original.Duration;
            Delay = original.Delay;
            Curve = original.Curve;
            CurveName = original.CurveName;
            Ease = original.Ease;
        }
    }

    /// <summary>
    /// 音效播放器
    /// </summary>
    [System.Serializable]
    public class AudioPlayer
    {
        public AudioSource Player;
        public bool IsPlaying;
    }

    /// <summary>
    /// 布局匹配分辨率类
    /// </summary>
    [System.Serializable]
    public class ScreenResolutionNode
    {
        public string Indicator;
        public Vector2 Res;
    }

    /// <summary>
    /// 元素信息
    /// </summary>
    [System.Serializable]
    public class HudElementInfo
    {
        public List<XHud_Module_Element> elements = new List<XHud_Module_Element>();
        public List<GameObject> notPrefabsList = new List<GameObject>();
    }

    [System.Serializable]
    public struct XHudComponentStatistic
    {
        public int elements;
        public int sounders;
        public int animators;
        public int sliders;
        public int toggles;
        public int progresses;
        public int buttons;
        public int options;
        public int texts;
        public int tmptexts;
        public int containers;
    }

    [System.Serializable]
    public struct XHUdRatioReferenceRes
    {
        [SerializeField]
        public Vector2 size;
        [SerializeField]
        public Sprite sprite;
    }

    #endregion

    public partial class XHud_Manager : MonoBehaviour
    {
        #region 元素库
        [Tooltip("此元素库用于配置存放场景中需要调用的元素预制体")]
        /// <summary>
        /// 元素库 - 配置化（Editor期间）
        /// </summary>
        public List<XHud_Library_Element> Hud_ElementLibrarys = new List<XHud_Library_Element>();
        #endregion

        #region 资源库类
        [Tooltip("曲线库")]
        /// <summary>
        /// 曲线库
        /// </summary>
        public XHud_Library_Curves Hud_Curves;
        [Tooltip("调色板")]
        /// <summary>
        /// 调色板
        /// </summary>
        public XHud_Library_Colors Hud_Colors;
        [Tooltip("音效库")]
        /// <summary>
        /// 音效库
        /// </summary>
        public XHud_Library_Sounds Hud_Sounds;
        [Tooltip("字体库")]
        /// <summary>
        /// 字体库
        /// </summary>
        public XHud_Library_TextStyle Hud_TextStyleLibrary;
        [Tooltip("转场")]
        /// <summary>
        /// 转场
        /// </summary>
        public XHud_Library_Transition Hud_TransitionLib;
        [Tooltip("元素动效库")]
        /// <summary>
        /// 元素动效库
        /// </summary>
        public XHud_Library_Motion Hud_ElementMotion;
        #endregion

        #region UI的全局动画速度
        [Range(0, 1)]
        [Tooltip("影响所有UI的动画速度")]
        /// <summary>
        /// 影响所有UI的动画速度
        /// </summary>
        public float DurationMultiply = 1f;
        #endregion

        #region 必要组件
        [Tooltip("事件系统")]
        /// <summary>
        /// EventSystem组件
        /// </summary>
        public EventSystem Hud_EventSystem;
        [Tooltip("输入系统")]
        /// <summary>
        /// InputSystemUIInputModule组件
        /// </summary>
        public InputSystemUIInputModule Hud_InputSystemUIInputModule;
        [Tooltip("XTween")]
        /// <summary>
        /// XTween
        /// </summary>
        public XTween_Manager XTweenManager;
        #endregion

        #region 元素创建与回收参数
        [Tooltip("此参数用于元素再生成时的默认动效效果")]
        /// <summary>
        /// 默认参数 - 元素生成
        /// </summary>
        public Motion_Creator CreateArgs_Default;
        [Tooltip("此参数用于元素再回收时的默认动效效果")]
        /// <summary>
        /// 默认参数 - 元素回收
        /// </summary>
        public Motion_Recycler RecycleArgs_Default;
        public string Crc_Lib_Name;
        public string Rec_Lib_Name;
        #endregion

        #region 状态开关
        [Tooltip("调试模式")]
        /// <summary>
        /// 调试模式
        /// </summary>
        public bool UseDebug;
        [Tooltip("管理器初始化状态")]
        /// <summary>
        /// 管理器初始化状态
        /// </summary>
        public bool IsInitialized;

        /// <summary>
        /// 支持世界UI
        /// </summary>
        public bool SupportWorldUI;

        /// <summary>
        /// Led闪烁效果开启
        /// </summary>
        public bool EnabledLedEffect;

        /// <summary>
        /// 关闭面板后是否折叠所有选项卡
        /// </summary>
        public bool FoldAllPanelWithDisabled;
        #endregion

        void Awake()
        {
            // 单例模式检查
            hm_InstanceModeCheck();

            if (HudCanvas_Screen != null)
            {
                if (!UsePerfectPixelUpdate)
                    HudCanvas_Screen.pixelPerfect = false;
                else
                    HudCanvas_Screen.pixelPerfect = true;
            }
            if (HudCanvasScaler == null)
                HudCanvasScaler = HudCanvas_Screen.GetComponentInChildren<CanvasScaler>();

            if (Hud_MouseCursor == null)
                Hud_MouseCursor = GetComponentInChildren<XHud_CustomMouseCursor>();

            if (Hud_TransitionController == null)
                Hud_TransitionController = GetComponentInChildren<XHud_TransitionController>();

            // 根据 BluePrint_OnStartHide 配置决定蓝图视觉在游戏启动时的显示状态
            hm_BluePrint_InitializeMode();

            // 让蓝图结构永远位于底层
            hm_BluePrintRootFirstSibling();

            // 将 Unity 渲染管线中的模糊强度与框架内部管理的强度值同步
            hm_UniversalFeature_Blur_Get();

            // 根据配置的预制体列表，预先生成指定数量的 UI 元素实例，放入对象池待用
            hm_ElementLibrary_Initialize();

            // 预创建指定数量的 AudioSource 组件，用于播放 UI 音效
            hm_LibrarySounds_Initialize(SounderPoolCount);

            // 检查 UI 交互状态的协程
            StartCoroutine(CheckUIInteraction());
        }

        private void Start()
        {

        }

        void Update()
        {
            // 获取屏幕分辨率
            hm_GetScreenResolution();
            // 更新所有已生成 Hud 元素的动画状态
            hm_HudElement_UpdateAnimating();
            // 负责同步屏幕遮罩（Mask）的视觉状态和射线检测属性
            hm_MaskUpdate();
            // 散焦遮罩更新
            hm_BlurMaskUpdate();
            // 转场控制器更新
            hm_TransitionUpdate();
            // 更新散焦模糊特性强度
            hm_UniversalFeature_Blur_Update();
            // 同步屏幕空间和世界空间 UI 内容的整体透明度，并触发相应的状态变化事件
            hm_ContentOpacity_Update();
            // 同步音效池中每个 AudioSource 的播放状态到对应的 AudioPlayer 记录中
            hm_LibrarySounds_Update();
            // 遍历所有元素库，更新每个预生成元素的使用状态、使用计数和回收计数
            hm_ElementLibrary_UpdateStates();
            // 负责更新安全框（Safe Frame）的所有视觉元素，包括框线、分割线和中心标记
            hm_SafeFrameUpdate();
            // 负责更新所有屏幕空间锚点的位置，根据配置的边距值动态计算每个锚点的坐标
            hm_Layout_Update();
            // 同步所有相机相关配置到实际的相机组件，确保 UI 相机和场景相机的渲染设置与配置一致
            hm_CameraUpdate();
            // 负责同步蓝图视觉模式的所有视觉元素，包括整体透明度、背景、网格线、标题水印等
            hm_BluePrint_Update();
        }

        private void OnDisable()
        {
            // 清理预生成的元素队列列表
            hm_ElementLibrary_Clean();
        }

        private void OnDrawGizmos()
        {
            // 绘制屏幕构图参考线
            hm_CompGuide_Draw();
        }

        /// <summary>
        /// 检测是否存在已生成的Hud元素
        /// </summary>
        /// <returns>返回True则当前xHud Manager管理器消息中已生成了Hud元素反之则说明已清空</returns>
        public bool hm_HasElements()
        {
            bool state = false;
            for (int i = 0; i < Anchors_Layout_Screen.Count; i++)
            {
                if (Anchors_Layout_Screen[i].HudElementInfos != null && Anchors_Layout_Screen[i].HudElementInfos.Count > 0)
                {
                    state = true;
                }
            }
            return state;
        }
        /// <summary>
        /// 画布像素对齐开关
        /// </summary>
        /// <param name="treeState">是否开启像素对齐</param>
        public void hm_UsePixelPerfect(bool state)
        {
            if (!UsePerfectPixelUpdate)
                return;
            HudCanvas_Screen.pixelPerfect = state;
        }
        /// <summary>
        /// 为指定的相机堆栈添加Hud叠加层
        /// </summary>
        /// <param name="uac"></param>
        public void hm_AssignedCameraStack(UniversalAdditionalCameraData uac)
        {
            if (HudCamera == null)
                if (UseDebug)
                    XHud_Utilitys.Func_PrintInfo("xHud Manager管理器消息", "当前HudCamera为空，无法为指定的相机堆栈添加Hud叠加层", HudMsgState.警告);
            if (!uac.cameraStack.Contains(HudCamera))
            {
                uac.cameraStack.Add(HudCamera);
            }
        }
        /// <summary>
        /// 更新所有已生成 Hud 元素的动画状态
        /// 遍历屏幕空间锚点列表中的所有元素，同步其动画状态到元素节点信息中
        /// 
        /// 工作原理：
        /// 1. 遍历屏幕空间的所有锚点布局（Anchors_Layout_Screen）
        /// 2. 遍历每个锚点下的所有 Hud 元素（HudElementInfos）
        /// 3. 将每个元素当前的动画状态（Animating）同步到对应的节点信息中（IsAnimating）
        /// 
        /// 为什么需要这个方法？
        /// - 外部系统可能需要查询某个 UI 元素是否正在播放动画
        /// - 通过 HudElementNode.IsAnimating 可以快速获取状态，无需直接访问元素
        /// - 提供统一的动画状态查询接口，便于 UI 状态机管理
        /// 
        /// 同步的状态：
        /// - 元素的 Animating 属性（true/false）
        ///   true: 元素正在播放入场/出场动画
        ///   false: 元素处于静止状态
        /// 
        /// 使用场景：
        /// - 判断是否可以与 UI 元素交互（动画播放时通常禁用交互）
        /// - 等待所有动画完成后执行后续逻辑
        /// - UI 状态机中需要知道当前动画状态
        /// - 调试时查看哪些元素正在播放动画
        /// 
        /// 调用频率：
        /// - 每帧在 XHud_Manager.Update() 中调用
        /// - 开销较小，遍历所有已生成的 UI 元素
        /// 
        /// 性能考虑：
        /// - 如果场景中 UI 元素数量较多（100+），每帧遍历可能有轻微开销
        /// - 可以考虑仅在状态变化时触发事件，而不是每帧遍历
        /// 
        /// 注意事项：
        /// - 此方法仅同步屏幕空间的 UI 元素（世界空间元素未包含）
        /// - 确保元素节点引用有效，避免空引用异常
        /// </summary>
        public void hm_HudElement_UpdateAnimating()
        {
            for (int i = 0; i < Anchors_Layout_Screen.Count; i++)
            {
                if (Anchors_Layout_Screen[i].HudElementInfos != null)
                {
                    for (int s = 0; s < Anchors_Layout_Screen[i].HudElementInfos.Count; s++)
                    {
                        if (Anchors_Layout_Screen[i].HudElementInfos[s] != null)
                        {
                            Anchors_Layout_Screen[i].HudElementInfos[s].IsAnimating = Anchors_Layout_Screen[i].HudElementInfos[s].Element.Animating;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 获取屏幕分辨率
        /// </summary>
        public Vector2 hm_GetScreenResolution()
        {
            return ScreenRes;
        }
        /// <summary>
        /// 获取画布分辨率
        /// </summary>
        public Vector2 hm_GetCanvasScalerScreenSize()
        {
            return CanvasScalerScreenSize;
        }


        #region 创建辅助

        /// <summary>
        /// xHud Manager管理器消息 - 收集所有生成的HudElementItem元素 - 屏幕
        /// </summary>
        /// <param name="structs"></param>
        /// <returns></returns>
        private HudElementNode[] hm_HudElement_CollectElements_Screen()
        {
            List<HudElementNode> items = new List<HudElementNode>();

            for (int i = 0; i < Anchors_Layout_Screen.Count; i++)
            {
                for (int s = 0; s < Anchors_Layout_Screen[i].HudElementInfos.Count; s++)
                {
                    items.Add(Anchors_Layout_Screen[i].HudElementInfos[s]);
                }
            }
            return items.ToArray();
        }

        /// <summary>
        /// xHud Manager管理器消息 - 收集所有生成的HudElementItem元素 - 世界
        /// </summary>
        /// <param name="structs"></param>
        /// <returns></returns>
        private HudElementNode[] hm_HudElement_CollectElements_World()
        {
            List<HudElementNode> items = new List<HudElementNode>();

            for (int i = 0; i < Anchors_Layout_World.Count; i++)
            {
                items.Add(Anchors_Layout_World[i]);
            }
            return items.ToArray();
        }

        /// <summary>
        /// xHud Manager管理器消息 - 实例化方式生成Hud元素
        /// </summary>
        /// <param name="ModuleName">模块名称</param>
        /// <param name="Parent">父物体</param>
        /// <param name="缩放_Scale">缩放尺寸</param>
        /// <returns>返回一个生成的HUD元素</returns>
        private XHud_Module_Element hm_HudElement_Create(string LibraryName, string ModuleName)
        {
            XHud_Module_Element element = null;

            for (int i = 0; i < Hud_ElementLibrarys.Count; i++)
            {
                if (LibraryName == Hud_ElementLibrarys[i].LibraryName)
                {
                    XHud_Library_Element lib = Hud_ElementLibrarys[i];
                    for (int s = 0; s < lib.ElementLibrary.Count; s++)
                    {
                        XHud_LibraryArg_Element_Item item = lib.ElementLibrary[s];
                        if (item.Target.name == ModuleName)
                        {
                            element = hm_ElementLibrary_Spawn(ModuleName);
                            break;
                        }
                    }
                    break;
                }
            }
            return element;
        }

        /// <summary>
        /// xHud Manager管理器消息 - 匹配锚点类型
        /// </summary>
        /// <param name="type">锚点类型</param>
        /// <returns>返回一个锚点布局</returns>
        public Anchor_Layout hm_HudElement_MatchType(XHudAnchor type)
        {
            for (int i = 0; i < Anchors_Layout_Screen.Count; i++)
            {
                if (Anchors_Layout_Screen[i].Type == type)
                {
                    return Anchors_Layout_Screen[i];
                }
            }
            return null;
        }

        /// <summary>
        /// xHud Manager管理器消息 - 清理已存在的Element项 - 屏幕
        /// </summary>
        /// <param name="element">目标元素</param>
        public void hm_HudElement_CleanAnchor_Screen(XHud_Module_Element element)
        {
            for (int i = 0; i < Anchors_Layout_Screen.Count; i++)
            {
                for (int s = 0; s < Anchors_Layout_Screen[i].HudElementInfos.Count; s++)
                {
                    if (element.ID == Anchors_Layout_Screen[i].HudElementInfos[s].ID && element.Indicator == Anchors_Layout_Screen[i].HudElementInfos[s].Indicator)
                    {
                        Anchors_Layout_Screen[i].HudElementInfos.RemoveAt(s);
                    }
                }
            }
        }

        /// <summary>
        /// xHud Manager管理器消息 - 清理已存在的Element项 - 世界
        /// </summary>
        /// <param name="element">目标元素</param>
        public void hm_HudElement_CleanAnchor_World(XHud_Module_Element element)
        {
            for (int i = 0; i < Anchors_Layout_World.Count; i++)
            {
                if (element.ID == Anchors_Layout_World[i].ID && element.Indicator == Anchors_Layout_World[i].Indicator)
                {
                    Anchors_Layout_World.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// xHud Manager管理器消息 - 设置父物体
        /// </summary>
        /// <param name="element">目标元素</param>
        /// <param name="anchor">锚点父物体</param>
        public void hm_HudElement_ParentSetup(XHud_Module_Element element, RectTransform anchor)
        {
            element.RectTransform.SetParent(anchor);
        }

        /// <summary>
        /// xHud Manager管理器消息 - 预存储到锚点列表 - 屏幕
        /// </summary>
        /// <param name="anchor_struct">锚点根节点</param>
        /// <param name="element">元素</param>
        /// <param name="module_name">模块名称</param>
        /// <param name="indicator_name">标识名称</param>
        /// <returns></returns>
        private HudElementNode hm_HudElement_Send_To_AnchorList_Screen(Anchor_Layout anchor_struct, XHud_Module_Element element, string module_name, string indicator_name)
        {
            if (anchor_struct.HudElementInfos == null)
                anchor_struct.HudElementInfos = new List<HudElementNode>();

            HudElementNode item = new HudElementNode();
            item.ModuleName = module_name;
            if (string.IsNullOrEmpty(indicator_name))
            {
                item.Indicator = "undicator_" + module_name;
            }
            item.Indicator = indicator_name;
            item.ID = element.ID;
            item.Element = element;

            anchor_struct.HudElementInfos.Add(item);
            return item;
        }

        /// <summary>
        /// xHud Manager管理器消息 - 预存储到锚点列表 - 世界
        /// </summary>
        /// <param name="anchor_struct">锚点根节点</param>
        /// <param name="element">元素</param>
        /// <param name="module_name">模块名称</param>
        /// <param name="indicator_name">标识名称</param>
        /// <returns></returns>
        private HudElementNode hm_HudElement_Send_To_AnchorList_World(XHud_Module_Element element, string module_name, string indicator_name)
        {
            HudElementNode item = new HudElementNode();
            item.ModuleName = module_name;
            item.Indicator = indicator_name;
            item.ID = element.ID;
            item.Element = element;

            Anchors_Layout_World.Add(item);

            return item;
        }

        /// <summary>
        /// xHud Manager管理器消息 - 获取目标元素身上的分辨率匹配方案的标识名称的信息
        /// </summary>
        /// <param name="element">目标元素</param>
        /// <param name="solutionName">分辨率匹配方案的标识名称</param>
        /// <returns></returns>
        private Anchor_Layout hm_GetCurrentSolutionLayout(XHud_Module_Element element, string solutionName)
        {
            Anchor_Layout layout = null;
            for (int i = 0; i < element.RMS_LayoutDatas.Count; i++)
            {
                if (element.RMS_LayoutDatas[i].LayoutName == solutionName)
                {
                    layout = hm_HudElement_MatchType(element.RMS_LayoutDatas[i].Anchor);
                }
            }
            return layout;
        }
        #endregion

        #region 元素和组件统计
        /// <summary>
        /// 获取场景中现有的各种组件的集合数量
        /// </summary>
        /// <returns></returns>
        public XHudComponentStatistic hm_GetXHudComponentsCount()
        {
            XHudComponentStatistic statistic = new XHudComponentStatistic();

            //统计屏幕UI组件数
            for (int i = 0; i < Anchors_Layout_Screen.Count; i++)
            {
                Anchor_Layout lay = Anchors_Layout_Screen[i];
                for (int s = 0; s < lay.HudElementInfos.Count; s++)
                {
                    statistic.elements++;

                    HudElementNode node = lay.HudElementInfos[s];
                    statistic.animators += node.Element.AnimatorNodes.Count;
                    statistic.containers += node.Element.ContainerNodes.Count;
                    statistic.sounders += node.Element.SounderNodes.Count;
                    statistic.buttons += node.Element.ButtonNodes.Count;
                    statistic.options += node.Element.OptionNodes.Count;
                    statistic.texts += node.Element.TextNodes.Count;
                    statistic.tmptexts += node.Element.TmpTextNodes.Count;
                    statistic.sliders += node.Element.SliderNodes.Count;
                    statistic.progresses += node.Element.ProgressNodes.Count;
                    statistic.toggles += node.Element.ToggleNodes.Count;
                }
            }

            //统计世界UI组件数
            for (int i = 0; i < Anchors_Layout_World.Count; i++)
            {
                statistic.elements++;

                HudElementNode node = Anchors_Layout_World[i];
                statistic.animators += node.Element.AnimatorNodes.Count;
                statistic.containers += node.Element.ContainerNodes.Count;
                statistic.sounders += node.Element.SounderNodes.Count;
                statistic.buttons += node.Element.ButtonNodes.Count;
                statistic.options += node.Element.OptionNodes.Count;
                statistic.texts += node.Element.TextNodes.Count;
                statistic.tmptexts += node.Element.TmpTextNodes.Count;
                statistic.sliders += node.Element.SliderNodes.Count;
                statistic.progresses += node.Element.ProgressNodes.Count;
                statistic.toggles += node.Element.ToggleNodes.Count;
            }

            return statistic;
        }
        #endregion

        #region 元素库

        /// <summary>
        /// 获取所有元素库
        /// </summary>
        /// <returns></returns>
        public XHud_Library_Element[] hm_ElementLibrary_GetArray()
        {
            return Hud_ElementLibrarys.ToArray();
        }

        /// <summary>
        /// 获取所有元素库
        /// </summary>
        /// <returns></returns>
        public List<XHud_Library_Element> hm_ElementLibrary_GetList()
        {
            return Hud_ElementLibrarys;
        }

        /// <summary>
        /// 获取所有元素库
        /// </summary>
        /// <returns></returns>
        public string[] hm_ElementLibrary_GetAllLibraryNames()
        {
            string[] names = new string[Hud_ElementLibrarys.Count];
            for (int i = 0; i < names.Length; i++)
            {
                names[i] = Hud_ElementLibrarys[i].LibraryName;
            }
            return names;
        }

        /// <summary>
        /// 获取目标元素库
        /// </summary>
        /// <returns></returns>
        public XHud_Library_Element hm_ElementLibrary_GetTargetLibrary(string name)
        {
            XHud_Library_Element lib = null;

            string[] lib_names = hm_ElementLibrary_GetAllLibraryNames();
            for (int i = 0; i < lib_names.Length; i++)
            {
                if (name == lib_names[i])
                {
                    lib = Hud_ElementLibrarys[i];
                }
            }
            return lib;
        }

        /// <summary>
        /// 获取首位元素库
        /// </summary>
        /// <param name="LibName">目标元素库</param>
        public XHud_Library_Element hm_ElementLibrary_GetFirstLibrary()
        {
            XHud_Library_Element res_lib = null;
            if (Hud_ElementLibrarys != null && Hud_ElementLibrarys.Count > 0)
                res_lib = Hud_ElementLibrarys[0];
            return res_lib;
        }

        /// <summary>
        /// 元素库状态更新
        /// 遍历所有元素库，更新每个预生成元素的使用状态、使用计数和回收计数
        /// 
        /// 工作原理：
        /// 1. 检查元素库是否已初始化且不为空
        /// 2. 遍历所有元素库（Hud_ElementLibrarys）
        /// 3. 遍历每个元素库中的所有预制体项（ElementLibrary）
        /// 4. 遍历每个预制体项中的预生成元素列表（PreloadElements）
        /// 5. 更新每个元素的使用状态（Using）
        /// 6. 重新计算该预制体项的使用数量（UsedCount）和回收数量（RecycledCount）
        /// 
        /// 状态定义：
        /// - Using: 元素当前是否正在被使用（CreateState == Created）
        /// - UsedCount: 当前正在使用的元素数量
        /// - RecycledCount: 当前在池中空闲的元素数量
        /// 
        /// 更新时机：
        /// - 每帧在 XHud_Manager.Update() 中调用
        /// - 确保元素使用状态的实时性
        /// 
        /// 使用场景：
        /// - 元素池监控：实时查看元素使用情况
        /// - 调试：快速定位哪些元素正在使用
        /// - 性能优化：发现某个预制体频繁耗尽，考虑增加初始化数量
        /// - 资源管理：确保元素回收后正确标记为空闲
        /// 
        /// 性能考虑：
        /// - 遍历所有库、所有项、所有预生成元素
        /// - 如果元素库较大（多个库，每个库多个预制体，每个预制体多个实例），每帧遍历可能有性能压力
        /// - 建议在实际项目中根据元素数量评估是否需要每帧调用
        /// 
        /// 注意事项：
        /// - 此方法仅更新统计数据，不改变元素的实际使用状态
        /// - 确保 PreloadElements 列表中的元素引用有效
        /// </summary>
        private void hm_ElementLibrary_UpdateStates()
        {
            if (Hud_ElementLibrarys == null || Hud_ElementLibrarys.Count <= 0)
                return;

            #region 更新元素是否正在在被使用的状态
            for (int i = 0; i < Hud_ElementLibrarys.Count; i++)
            {
                XHud_Library_Element lib = Hud_ElementLibrarys[i];
                for (int s = 0; s < lib.ElementLibrary.Count; s++)
                {
                    XHud_LibraryArg_Element_Item item = lib.ElementLibrary[s];
                    if (item.PreloadElements != null && item.PreloadElements.Count > 0)
                    {
                        for (int v = 0; v < item.PreloadElements.Count; v++)
                        {
                            item.PreloadElements[v].UpdateState();
                        }
                        item.GetUsedCount();
                        item.GetRecycledCount();
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            #endregion
        }

        /// <summary>
        /// 初始化元素库（对象池系统）
        /// 根据配置的预制体列表，预先生成指定数量的 UI 元素实例，放入对象池待用
        /// 
        /// 工作原理：
        /// 1. 创建 Pool_Elements 根节点，作为所有元素池的容器
        /// 2. 遍历所有元素库（XHud_Library_Element）
        /// 3. 为每个库创建独立的目录节点
        /// 4. 遍历库中的每个预制体，实例化指定数量（InitializeCount）的元素副本
        /// 5. 将生成的元素放入 PreloadElements 列表，初始状态设为 Recycled（已回收）
        /// 6. 禁用生成的元素，等待后续从池中取出使用
        /// 
        /// 为什么需要对象池？
        /// - UI 元素频繁创建销毁会造成 GC 压力，导致卡顿
        /// - 对象池通过复用机制，大幅减少内存分配和回收
        /// - 预先生成（预热）可以避免首次使用时产生卡顿
        /// 
        /// 数据结构：
        /// Pool_Elements/                          # 根节点
        ///   ├── Library -> (MainUI)/              # 主 UI 库目录
        ///   │     ├── Category - 按钮/             # 按钮预制体目录
        ///   │     │     ├── Button_Clone_0         # 已回收的按钮实例
        ///   │     │     ├── Button_Clone_1         # 已回收的按钮实例
        ///   │     │     └── Button_Clone_2         # 已回收的按钮实例
        ///   │     └── Category - 弹窗/             # 弹窗预制体目录
        ///   │           ├── Popup_Clone_0
        ///   │           └── Popup_Clone_1
        ///   └── Library -> (CommonUI)/             # 通用 UI 库目录
        ///         └── Category - 提示框/
        ///               └── Toast_Clone_0
        /// 
        /// 使用流程：
        /// 1. 初始化：预生成元素 → 放入池中（状态：Recycled）
        /// 2. 使用时：从池中取出（Spawn）→ 激活并播放动画（状态：Created）
        /// 3. 回收时：播放出场动画 → 放回池中（状态：Recycled）
        /// </summary>
        private void hm_ElementLibrary_Initialize()
        {
            if (Hud_ElementLibrarys.Count <= 0)
                return;
            ///---创建元素库根目录
            GameObject PoolRoot = new GameObject();
            PoolRoot.name = "Pool_Elements";
            PoolRoot.layer = LayerMask.NameToLayer("XHud");
            PoolRoot.transform.SetParent(transform);
            PoolRoot.transform.localPosition = Vector3.zero;
            PoolRoot.transform.localEulerAngles = Vector3.zero;
            PoolRoot.transform.localScale = Vector3.one;

            hm_ElementLibrary_Clean();

            for (int v = 0; v < Hud_ElementLibrarys.Count; v++)
            {
                XHud_Library_Element Lib = Hud_ElementLibrarys[v];

                ///---创建库容器
                GameObject lib_obj = new GameObject();
                lib_obj.name = "Library -> ( " + Lib.LibraryName + " )";
                lib_obj.layer = LayerMask.NameToLayer("XHud");
                lib_obj.transform.SetParent(PoolRoot.transform);
                lib_obj.transform.localPosition = Vector3.zero;
                lib_obj.transform.localEulerAngles = Vector3.zero;
                lib_obj.transform.localScale = Vector3.one;

                Lib.LibraryRoot = lib_obj.transform;

                for (int i = 0; i < Lib.ElementLibrary.Count; i++)
                {
                    XHud_LibraryArg_Element_Item item = Lib.ElementLibrary[i];

                    GameObject root = new GameObject();
                    root.name = "Category - " + item.Target.name;
                    root.layer = LayerMask.NameToLayer("XHud");
                    root.transform.SetParent(lib_obj.transform);
                    root.transform.localPosition = Vector3.zero;
                    root.transform.localEulerAngles = Vector3.zero;
                    root.transform.localScale = Vector3.one;
                    item.Root = root.transform;

                    for (int s = 0; s < item.InitializeCount; s++)
                    {
                        XHud_Module_Element element = Instantiate(item.Target, Vector3.zero, Quaternion.identity, item.Root);
                        ///---保存原始名称
                        element.OriginalName = element.transform.name.Substring(0, element.transform.name.Length - 7);
                        ///---改名
                        element.transform.name = element.OriginalName + "_Clone_" + s;
                        ///---赋值源库名
                        element.OriginPoolName = Lib.LibraryName;


                        XHud_LibraryArg_Element_Info pw = new XHud_LibraryArg_Element_Info();
                        pw.HudElement = element;
                        pw.HudElement.CreateState = HudElementCreateState.Recycled;
                        pw.HudElement.element_Reset();
                        pw.HudElement.gameObject.SetActive(false);
                        item.PreloadElements.Add(pw);

                    }
                }
            }
            if (Act_ElementsLib_Instantiated != null)
                Act_ElementsLib_Instantiated();
        }

        /// <summary>
        /// 从元素库里取出一个元素
        /// </summary>
        /// <param name="ElementName">元素标识名称</param>
        /// <returns></returns>
        public XHud_Module_Element hm_ElementLibrary_Spawn(string ElementName)
        {
            XHud_Module_Element element = null;

            for (int i = 0; i < Hud_ElementLibrarys.Count; i++)
            {
                XHud_Library_Element lib = Hud_ElementLibrarys[i];

                for (int k = 0; k < lib.ElementLibrary.Count; k++)
                {
                    XHud_LibraryArg_Element_Item item = lib.ElementLibrary[k];

                    if (item.Target.name == ElementName)
                    {
                        int index = item.NextIndex;

                        if (item.PreloadElements != null && item.PreloadElements.Count > 0)
                        {
                            if (item.PreloadElements[index] != null)
                            {
                                element = item.PreloadElements[index].HudElement;
                                element.gameObject.SetActive(true);
                                if (item.NextIndex >= item.InitializeCount - 1)
                                    item.NextIndex = 0;
                                else
                                    item.NextIndex++;
                            }

                            if (Act_SpawnElement != null)
                                Act_SpawnElement(element);
                        }
                    }
                }
            }

            return element;
        }

        /// <summary>
        /// 回收一个元素到元素库
        /// </summary>
        /// <param name="Element">目标元素</param>
        public void hm_ElementLibrary_Despawn(XHud_Module_Element Element)
        {
            for (int i = 0; i < Hud_ElementLibrarys.Count; i++)
            {
                XHud_Library_Element lib = Hud_ElementLibrarys[i];

                for (int m = 0; m < lib.ElementLibrary.Count; m++)
                {
                    XHud_LibraryArg_Element_Item item = lib.ElementLibrary[m];

                    if (Element.OriginalName == item.Name)
                    {
                        Element.transform.SetParent(item.Root);
                        Element.element_Reset();
                        Element.gameObject.SetActive(false);
                    }
                }
            }
        }

        /// <summary>
        /// 回收所有元素库的所有元素
        /// </summary>
        /// <returns></returns>
        public void hm_ElementLibrary_DespawnAll()
        {
            for (int i = 0; i < Hud_ElementLibrarys.Count; i++)
            {
                Hud_ElementLibrarys[i].ElementLibrary_RecycleAll();
            }
        }

        /// <summary>
        /// 获取元素库的数量
        /// </summary>
        /// <returns></returns>
        public int hm_ElementLibrary_GetCount()
        {
            return Hud_ElementLibrarys.Count;
        }

        /// <summary>
        /// 判断元素库是否存在有效
        /// </summary>
        /// <returns></returns>
        public bool hm_ElementLibrary_IsExist(string name)
        {
            string[] names = hm_ElementLibrary_GetAllLibraryNames();
            bool exist = false;

            for (int i = 0; i < names.Length; i++)
            {
                if (names[i] == name)
                {
                    exist = true;
                    break;
                }
            }

            return exist;
        }

        /// <summary>
        /// 清理预生成的元素队列列表，因为在Unity编辑器中如果对ScriptableObject临时赋值会被保留下来，因此需要清理队列和使用痕迹
        /// </summary>
        public void hm_ElementLibrary_Clean()
        {
            for (int v = 0; v < Hud_ElementLibrarys.Count; v++)
            {
                XHud_Library_Element lib = Hud_ElementLibrarys[v];
                #region 清理预生成元素及其使用信息痕迹，主要是为了弥补ScriptableObject在运行时也能被赋值
                lib.LibraryRoot = null;
                for (int i = 0; i < lib.ElementLibrary.Count; i++)
                {
                    lib.ElementLibrary[i].Root = null;
                    lib.ElementLibrary[i].UsedCount = 0;
                    lib.ElementLibrary[i].RecycledCount = 0;
                    lib.ElementLibrary[i].NextIndex = 0;
                    lib.ElementLibrary[i].Root = null;

                    if (lib.ElementLibrary[i].PreloadElements == null)
                        lib.ElementLibrary[i].PreloadElements = new List<XHud_LibraryArg_Element_Info>();

                    lib.ElementLibrary[i].PreloadElements.Clear();
                }
                #endregion
            }
        }

        #region 创建元素库
        /// <summary>
        /// 创建元素库的项
        /// </summary>
        /// <param name="Element">目标元素</param>
        /// <param name="InitialCount">初始化数量</param>
        /// <returns></returns>
        public XHud_LibraryArg_Element_Item hm_ElementLibrary_CreateItem(XHud_Module_Element Element, int InitialCount)
        {
            XHud_LibraryArg_Element_Item item = new XHud_LibraryArg_Element_Item();
            item.Target = Element;
            item.Name = string.IsNullOrEmpty(Element.transform.name) ? "NewElement" : Element.transform.name;
            item.InitializeCount = InitialCount;

            if (item.PreloadElements == null)
                item.PreloadElements = new List<XHud_LibraryArg_Element_Info>();

            return item;
        }

        /// <summary>
        /// 动态创建元素库
        /// </summary>
        /// <param name="LibraryName">元素库名称</param>
        /// <param name="Items">元素库项数组</param>
        /// <returns>返回元素库</returns>
        public XHud_Library_Element hm_ElementLibrary_AddItem(string LibraryName, XHud_LibraryArg_Element_Item[] Items)
        {
            XHud_Library_Element lib = ScriptableObject.CreateInstance<XHud_Library_Element>();
            lib.name = "RunTimeLib";
            lib.LibraryName = LibraryName;
            for (int i = 0; i < Items.Length; i++)
            {
                if (lib.ElementLibrary == null)
                    lib.ElementLibrary = new List<XHud_LibraryArg_Element_Item>();

                lib.ElementLibrary.Add(Items[i]);
            }
            return lib;
        }
        #endregion

        #region 元素库实例化 / 与销毁
        /// <summary>
        /// 实例化目标元素库
        /// </summary>
        /// <param name="Library">目标元素库</param>
        public void hm_ElementLibrary_Initialize(XHud_Library_Element Library)
        {
            GameObject PoolRoot = transform.Find("Pool_Elements").gameObject;
            if (PoolRoot == null)
            {
                ///---创建元素库根目录
                PoolRoot = new GameObject();
                PoolRoot.name = "Pool_Elements";
                PoolRoot.transform.SetParent(transform);
                PoolRoot.transform.localPosition = Vector3.zero;
                PoolRoot.transform.localEulerAngles = Vector3.zero;
                PoolRoot.transform.localScale = Vector3.one;
            }

            ///---创建库容器
            GameObject lib_obj = new GameObject();
            lib_obj.name = "Category -> ( " + Library.LibraryName + " )";
            lib_obj.transform.SetParent(PoolRoot.transform);
            lib_obj.transform.localPosition = Vector3.zero;
            lib_obj.transform.localEulerAngles = Vector3.zero;
            lib_obj.transform.localScale = Vector3.one;
            Library.LibraryRoot = lib_obj.transform;

            List<XHud_LibraryArg_Element_Item> items = Library.ElementLibrary;

            for (int i = 0; i < items.Count; i++)
            {
                GameObject root = new GameObject();
                root.name = "Case - " + items[i].Target.name;
                root.transform.SetParent(lib_obj.transform);
                root.transform.localPosition = Vector3.zero;
                root.transform.localEulerAngles = Vector3.zero;
                root.transform.localScale = Vector3.one;
                items[i].Root = root.transform;

                for (int s = 0; s < items[i].InitializeCount; s++)
                {
                    XHud_Module_Element element = Instantiate(items[i].Target, Vector3.zero, Quaternion.identity, items[i].Root);
                    ///---保存原始名称
                    element.OriginalName = element.transform.name.Substring(0, element.transform.name.Length - 7);
                    ///---改名
                    element.transform.name = element.OriginalName + "_Clone_" + s;

                    XHud_LibraryArg_Element_Info pw = new XHud_LibraryArg_Element_Info();
                    pw.HudElement = element;
                    pw.HudElement.CreateState = HudElementCreateState.Recycled;
                    pw.HudElement.element_Reset();
                    pw.HudElement.gameObject.SetActive(false);

                    if (items[i].PreloadElements == null)
                        items[i].PreloadElements = new List<XHud_LibraryArg_Element_Info>();

                    items[i].PreloadElements.Add(pw);
                }
            }

            Hud_ElementLibrarys.Add(Library);

            if (Act_ElementsLib_Instantiated != null)
                Act_ElementsLib_Instantiated();
        }

        /// <summary>
        /// 删除已经初始化的元素库
        /// </summary>
        /// <param name="LibName">目标元素库</param>
        public void hm_ElementLibrary_Destroyed(string LibName)
        {
            for (int i = 0; i < Hud_ElementLibrarys.Count; i++)
            {
                XHud_Library_Element lib = Hud_ElementLibrarys[i];
                if (lib.LibraryName == LibName)
                {
                    lib.ElementLibrary_RecycleAll();
                    DestroyImmediate(lib.LibraryRoot.gameObject, true);
                    Hud_ElementLibrarys.RemoveAt(i);
                }
            }
        }
        #endregion
        #endregion

        #region 创建

        #region 公共

        /// <summary>
        /// xHud Manager管理器消息 - 创建的元素的初始化设置 - 屏幕模式
        /// </summary>
        /// <param name="element">目标元素</param>
        /// <param name="anchor">锚点类型</param>
        /// <param name="alpha">透明度_Alpha</param>
        /// <param name="offset">位置偏移</param>
        /// <param name="scale">缩放</param>
        /// <param name="size">尺寸</param>
        public void hm_HudElement_Initialize_For_Screen(XHud_Module_Element element, XHudAnchor anchor, float alpha, Vector3 offset, Vector3 scale, Vector2 size)
        {
            element.RectTransform.SetParent(hm_Layout_GetAnchor(anchor));
            if (size.x > 0 && size.y > 0)
                element.element_SizeSet(size);
            element.element_PositionResetZero();
            element.element_RotationResetZero();
            element.element_PositionOffset(offset);
            element.element_ScaleSet(scale);
            element.ID = element.element_CreateID(hm_HudElement_CollectElements_Screen());
            element.element_AlphaSet(alpha);
        }

        /// <summary>
        /// xHud Manager管理器消息 - 创建的元素的初始化设置 - 屏幕模式 (依据元素自身设计布局信息)
        /// </summary>
        /// <param name="element">目标元素</param>
        /// <param name="alpha">透明度_Alpha</param>
        /// <param name="offset">位置偏移</param>
        /// <param name="rms_name">RMS 方案名称</param>
        /// <param name="DontCreateID">是否为自身生成随机ID</param>
        public void hm_HudElement_Initialize_ByDesignLayout_For_Screen(XHud_Module_Element element, float alpha, Vector3 offset, string rms_name, bool DontCreateID = false)
        {
            bool IsExist = false;
            for (int i = 0; i < element.RMS_LayoutDatas.Count; i++)
            {
                if (element.RMS_LayoutDatas[i].LayoutName == rms_name)
                {
                    Element_RMS_LayoutData info = element.RMS_LayoutDatas[i];
                    element.RectTransform.SetParent(hm_Layout_GetAnchor(info.Anchor));
                    element.element_AnchorRangeSet(info.AnchorMin, info.AnchorMax);
                    element.element_PivotSet(info.Pivot);
                    element.element_PositionSet(info.Position + offset);
                    element.element_RotationSet(info.Euler);
                    element.element_ScaleSet(info.Scale);

                    if (!DontCreateID)
                        element.ID = element.element_CreateID(hm_HudElement_CollectElements_Screen());
                    element.element_AlphaSet(alpha);
                    if (UseDebug)
                        XHud_Utilitys.Func_PrintInfo("xHud Manager管理器消息", "已将元素生成到指定设计布局！", HudMsgState.通知);
                    IsExist = true;
                    break;
                }
                else
                {
                    continue;
                }
            }
            if (!IsExist)
            {
                if (UseDebug)
                    XHud_Utilitys.Func_PrintInfo("xHud Manager管理器消息", "未找到指定标识名称的设计布局，请检查该元素是否有记录设计布局信息！", HudMsgState.错误);
            }
        }

        /// <summary>
        /// xHud Manager管理器消息 - 创建的元素的初始化设置 - 世界模式
        /// </summary>
        /// <param name="element">目标元素</param>
        /// <param name="size">尺寸</param>
        /// <param name="alpha">透明度_Alpha</param>
        /// <param name="offset">位置偏移</param>
        /// <param name="position">位置_Position</param>
        /// <param name="rotation">旋转_Rotation</param>
        public void hm_HudElement_Initialize_For_World(XHud_Module_Element element, Vector2 size, float alpha, Vector3 offset, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            element.RectTransform.SetParent(HudCanvas_WorldAnchor);
            if (size.x > 0 || size.y > 0)
                element.element_SizeSet(size);
            element.element_WorldPositionSet(position);
            element.element_WorldRotationSet(rotation);
            element.element_PositionOffset(offset);
            element.element_ScaleSet(scale);
            element.ID = element.element_CreateID(hm_HudElement_CollectElements_World());
            element.element_AlphaSet(alpha);
        }

        #endregion

        #region 屏幕空间
        /// <summary>
        /// xHud Manager管理器消息 - 创建一个Hud元素 - 屏幕空间
        /// </summary>
        /// <param name="libname">目标库名称</param>
        /// <param name="indicator">从库中取出后的自定义名称（仅为调用者自己理解的自定义名称）</param>
        /// <param name="modulename">预存入元素池的目标名称</param>
        /// <param name="offset">元素偏移</param>
        /// <param name="scale">元素缩放</param>
        /// <param name="size">元素尺寸</param>
        /// <param name="rms">RMS系统是否开启？</param>
        /// <param name="rms_name">RMS系统方案名称</param>
        /// <param name="args_creator">元素入场动画参数</param>
        /// <param name="action_in_start">元素入场开始委托</param>
        /// <param name="action_in_progress">元素入场进度委托</param>
        /// <param name="action_in_end">元素入场结束委托</param>        
        /// <param name="action_out_start">元素退场前委托</param>
        /// <param name="action_out_progress">元素退场进度委托</param>
        /// <param name="action_out_end">元素退场后委托</param>     
        /// <param name="autoin">此值是个非常关键的开关，如果你为一个元素编写了一个自定义控制的脚本绑定在它身上，并希望生成出来的时候由您自己决定何时播放动画，那么此值必须为False</param>
        /// <returns>返回一个HudElement节点元素体</returns>
        public HudElementNode hm_HudElement_Create_Screen(
            string libname, string indicator, string modulename,
            Vector3 offset = default(Vector3), Vector3 scale = default(Vector3), Vector2 size = default,
            bool rms = false, string rms_name = "",
            Motion_Creator args_creator = null,
            UnityAction<XHud_Module_Element> action_in_start = null,
            UnityAction<float> action_in_progress = null,
            UnityAction<XHud_Module_Element> action_in_end = null,
            UnityAction<XHud_Module_Element> action_out_start = null,
            UnityAction<float> action_out_progress = null,
            UnityAction<XHud_Module_Element> action_out_end = null,
            bool autoin = true)
        {
            #region 从元素库中取出元素
            XHud_Module_Element element = hm_HudElement_Create(libname, modulename);
            if (element == null)
            {
                Debug.Log("HudElement生成警告：您从元素池获取的目标元素为空！请检查该元素在元素池中的状态！");
                return null;
            }
            #endregion

            #region 是否开启RMS模式
            element.element_RMS_Mode_Enabled(rms);
            #endregion

            #region 从屏幕元素列表中清理目标元素
            hm_HudElement_CleanAnchor_Screen(element);
            #endregion

            #region 初始化元素到对应的目标锚点下
            if (args_creator == null)
                args_creator = CreateArgs_Default;

            HudElementNode node = null;
            Anchor_Layout anchor_struct = null;

            if (element.RMS_Enabled && RMS_Enabled)
            {
                #region 依据元素自身设计布局参数
                if (string.IsNullOrEmpty(rms_name))
                    anchor_struct = hm_GetCurrentSolutionLayout(element, RMS_CurrentSolution);
                else
                    anchor_struct = hm_GetCurrentSolutionLayout(element, rms_name);

                ///---如果RMS名称为空那么久参考HUDManager管理器当前选中的方案作为元素的RMS方案
                if (string.IsNullOrEmpty(rms_name))
                    hm_HudElement_Initialize_ByDesignLayout_For_Screen(element, 0, offset, RMS_CurrentSolution);
                else
                    hm_HudElement_Initialize_ByDesignLayout_For_Screen(element, 0, offset, rms_name);
                #endregion
            }
            else
            {
                #region 依据构造参数
                anchor_struct = hm_HudElement_MatchType(args_creator.anchor);
                hm_HudElement_Initialize_For_Screen(element, anchor_struct.Type, 0, offset, scale, size);
                #endregion
            }
            node = hm_HudElement_Send_To_AnchorList_Screen(anchor_struct, element, modulename, indicator);
            #endregion

            #region 委托-自动回收-动画-状态
            ///---动作
            node.Element.act_on_element_in_start += action_in_start;
            node.Element.act_on_element_in_progress += action_in_progress;
            node.Element.act_on_element_in_end += action_in_end;
            node.Element.act_on_element_out_start += action_out_start;
            node.Element.act_on_element_out_progress += action_out_progress;
            node.Element.act_on_element_out_end += action_out_end;

            node.Element.Animators_Rewind();

            if (autoin)
                node.Element.element_In(args_creator);
            node.Element.CreateState = HudElementCreateState.Created;
            #endregion

            return node;
        }
        #endregion

        #region 世界空间
        /// <summary>
        /// xHud Manager管理器消息 - 创建一个Hud元素 - 世界空间
        /// </summary>
        /// <param name="libname">目标元素库</param>
        /// <param name="indicator">目标标识名称</param>
        /// <param name="modulename">模块名称</param>
        /// <param name="size">锚点</param>
        /// <param name="position">位置_Position</param>
        /// <param name="rotation">旋转_Rotation</param>
        /// <param name="scale">缩放_Scale</param>
        /// <param name="offset">偏移</param>
        /// <param name="args_creator">元素动效参数 - 创建</param>
        /// <param name="action_in_start">委托-进入时</param>
        /// <param name="action_in_progress">委托-进入进度</param>
        /// <param name="action_in_end">委托-进入后</param>
        /// <param name="action_out_start">委托-退出时</param>
        /// <param name="action_out_progress">委托-退出进度</param>
        /// <param name="action_out_end">委托-退出后</param>
        /// <param name="autoin">元素自动执行ElementIn</param>
        /// <returns>返回一个HudElement节点元素体</returns>
        public HudElementNode hm_HudElement_Create_World(
            string libname, string indicator, string modulename,
            Vector2 size, Vector3 position, Vector3 rotation, Vector3 scale, Vector3 offset,
            Motion_Creator args_creator = null,
            UnityAction<XHud_Module_Element> action_in_start = null,
            UnityAction<float> action_in_progress = null,
            UnityAction<XHud_Module_Element> action_in_end = null,
            UnityAction<XHud_Module_Element> action_out_start = null,
            UnityAction<float> action_out_progress = null,
            UnityAction<XHud_Module_Element> action_out_end = null,
            bool autoin = true)
        {
            #region 从元素库中取出元素
            XHud_Module_Element element = hm_HudElement_Create(libname, modulename);
            if (element == null)
            {
                Debug.Log("HudElement生成警告：您从元素池获取的目标元素为空！请检查该元素在元素池中的状态！");
                return null;
            }
            #endregion

            #region 从世界元素列表中清理目标元素
            hm_HudElement_CleanAnchor_World(element);
            #endregion

            #region 初始化元素到世界锚点下
            hm_HudElement_Initialize_For_World(element, size, 0, offset, position, Quaternion.Euler(rotation), scale);
            HudElementNode node = hm_HudElement_Send_To_AnchorList_World(element, modulename, indicator);
            #endregion

            #region 委托-自动回收-动画-状态
            ///---动作
            node.Element.act_on_element_in_start += action_in_start;
            node.Element.act_on_element_in_progress += action_in_progress;
            node.Element.act_on_element_in_end += action_in_end;
            node.Element.act_on_element_out_start += action_out_start;
            node.Element.act_on_element_out_progress += action_out_progress;
            node.Element.act_on_element_out_end += action_out_end;

            if (autoin)
                node.Element.element_In(args_creator);
            node.Element.CreateState = HudElementCreateState.Created;
            #endregion

            return node;
        }
        #endregion

        #endregion

        #region 回收

        /// <summary>
        /// xHud Manager管理器消息 - 清空所有生成的Hud元素
        /// </summary>
        /// <param name="args">回收参数</param>
        /// <param name="action_out_start">委托 - 回收时</param>
        /// <param name="action_out_end">委托 - 回收后</param>
        public void hm_HudElement_RecycleAll(Motion_Recycler args, UnityAction<XHud_Module_Element> action_out_start = null, UnityAction<float> action_out_progress = null, UnityAction<XHud_Module_Element> action_out_end = null)
        {
            for (int i = 0; i < Anchors_Layout_Screen.Count; i++)
            {
                for (int s = 0; s < Anchors_Layout_Screen[i].HudElementInfos.Count; s++)
                {
                    Anchor_Layout anchorstruct = Anchors_Layout_Screen[i];
                    HudElementNode node = Anchors_Layout_Screen[i].HudElementInfos[s];

                    ///---动作
                    if (node.Element.act_on_element_out_start == null)
                        node.Element.act_on_element_out_start += action_out_start;
                    if (node.Element.act_on_element_out_progress == null)
                        node.Element.act_on_element_out_progress += action_out_progress;
                    if (node.Element.act_on_element_out_end == null)
                        node.Element.act_on_element_out_end += action_out_end;

                    node.Element.element_Out(args);
                }
            }

            for (int i = 0; i < Anchors_Layout_World.Count; i++)
            {
                HudElementNode node = Anchors_Layout_World[i];

                ///---动作
                if (node.Element.act_on_element_out_start == null)
                    node.Element.act_on_element_out_start += action_out_start;
                if (node.Element.act_on_element_out_progress == null)
                    node.Element.act_on_element_out_progress += action_out_progress;
                if (node.Element.act_on_element_out_end == null)
                    node.Element.act_on_element_out_end += action_out_end;

                node.Element.element_Out(args);
            }
        }

        /// <summary>
        /// xHud Manager管理器消息 - 清理目标ID的Hud元素
        /// </summary>
        /// <param name="id">目标ID</param>
        /// <param name="args">回收参数</param>
        /// <param name="action_out_start">委托 - 回收时</param>
        /// <param name="action_out_end">委托 - 回收后</param>
        public void hm_HudElement_RecycleAt(int id, Motion_Recycler args, UnityAction<XHud_Module_Element> action_out_start = null, UnityAction<float> action_out_progress = null, UnityAction<XHud_Module_Element> action_out_end = null)
        {
            bool finded = false;
            int x_id = 0;
            string x_name = "";

            #region 判定目标元素是屏幕类型还是世界类型
            bool IsMatchedScreen = false;
            bool IsMatchedWorld = false;

            for (int i = 0; i < Anchors_Layout_Screen.Count; i++)
            {
                for (int s = 0; s < Anchors_Layout_Screen[i].HudElementInfos.Count; s++)
                {
                    if (id == Anchors_Layout_Screen[i].HudElementInfos[s].ID)
                    {
                        IsMatchedScreen = true;
                        IsMatchedWorld = false;
                    }
                }
            }

            if (!IsMatchedScreen)
            {
                for (int i = 0; i < Anchors_Layout_World.Count; i++)
                {
                    if (id == Anchors_Layout_World[i].ID)
                    {
                        IsMatchedScreen = false;
                        IsMatchedWorld = true;
                    }
                }
            }
            #endregion

            #region 回收逻辑
            if (IsMatchedScreen && !IsMatchedWorld)///----如果是屏幕类型
            {
                for (int i = 0; i < Anchors_Layout_Screen.Count; i++)
                {
                    for (int s = 0; s < Anchors_Layout_Screen[i].HudElementInfos.Count; s++)
                    {
                        if (id == Anchors_Layout_Screen[i].HudElementInfos[s].ID)
                        {
                            x_id = Anchors_Layout_Screen[i].HudElementInfos[s].ID;
                            x_name = Anchors_Layout_Screen[i].HudElementInfos[s].Indicator;
                            finded = true;
                            Anchor_Layout anchorstruct = Anchors_Layout_Screen[i];
                            HudElementNode node = Anchors_Layout_Screen[i].HudElementInfos[s];

                            ///---动作
                            if (node.Element.act_on_element_out_start == null)
                                node.Element.act_on_element_out_start += action_out_start;
                            if (node.Element.act_on_element_out_progress == null)
                                node.Element.act_on_element_out_progress += action_out_progress;
                            if (node.Element.act_on_element_out_end == null)
                                node.Element.act_on_element_out_end += action_out_end;

                            node.Element.act_on_element_out_end += (XHud_Module_Element ele) =>
                            {
                                node.Element.element_Reset();
                                anchorstruct.HudElementInfos.Remove(node);
                                node.Element.CreateState = HudElementCreateState.Recycled;
                                hm_ElementLibrary_Despawn(node.Element);
                            };
                            if (args == null)
                                args = RecycleArgs_Default;
                            node.Element.element_Out(args);
                        }
                    }
                }
            }
            else///----如果是世界类型
            {
                for (int i = 0; i < Anchors_Layout_World.Count; i++)
                {
                    if (id == Anchors_Layout_World[i].ID)
                    {
                        x_id = Anchors_Layout_World[i].ID;
                        x_name = Anchors_Layout_World[i].Indicator;
                        finded = true;

                        HudElementNode node = Anchors_Layout_World[i];

                        ///---动作
                        if (node.Element.act_on_element_out_start == null)
                            node.Element.act_on_element_out_start += action_out_start;
                        if (node.Element.act_on_element_out_progress == null)
                            node.Element.act_on_element_out_progress += action_out_progress;
                        if (node.Element.act_on_element_out_end == null)
                            node.Element.act_on_element_out_end += action_out_end;

                        node.Element.act_on_element_out_end += (XHud_Module_Element ele) =>
                        {
                            node.Element.element_Reset();
                            Anchors_Layout_World.Remove(node);
                            node.Element.CreateState = HudElementCreateState.Recycled;
                            hm_ElementLibrary_Despawn(node.Element);
                        };
                        if (args == null)
                            args = RecycleArgs_Default;
                        node.Element.element_Out(args);
                    }
                }
            }
            #endregion

            #region 调试信息
            if (!finded)
            {
                if (UseDebug)
                    XHud_Utilitys.Func_PrintInfo("xHud Manager管理器消息", "Hud元素ID：" + id + " 不存在！未找到要回收的目标元素！", HudMsgState.通知);
            }
            else
            {
                if (UseDebug)
                    XHud_Utilitys.Func_PrintInfo("xHud Manager管理器消息", "已回收Hud元素：" + x_name + " / " + x_id, HudMsgState.通知);
            }
            #endregion
        }

        /// <summary>
        /// xHud Manager管理器消息 - 清理目标ID的Hud元素
        /// </summary>
        /// <param name="element">目标名称</param>
        /// <param name="args">回收参数</param>
        /// <param name="action_out_start">委托 - 回收时</param>
        /// <param name="action_out_end">委托 - 回收后</param>
        public void hm_HudElement_RecycleAt(XHud_Module_Element element, Motion_Recycler args, UnityAction<XHud_Module_Element> action_out_start = null, UnityAction<float> action_out_progress = null, UnityAction<XHud_Module_Element> action_out_end = null)
        {
            bool finded = false;
            int x_id = 0;
            string x_indicator = "";

            #region 判定目标元素是屏幕类型还是世界类型
            bool IsMatchedScreen = false;
            bool IsMatchedWorld = false;

            for (int i = 0; i < Anchors_Layout_Screen.Count; i++)
            {
                for (int s = 0; s < Anchors_Layout_Screen[i].HudElementInfos.Count; s++)
                {
                    if (element.ID == Anchors_Layout_Screen[i].HudElementInfos[s].ID)
                    {
                        IsMatchedScreen = true;
                        IsMatchedWorld = false;
                    }
                }
            }

            if (!IsMatchedScreen)
            {
                for (int i = 0; i < Anchors_Layout_World.Count; i++)
                {
                    if (element.ID == Anchors_Layout_World[i].ID)
                    {
                        IsMatchedScreen = false;
                        IsMatchedWorld = true;
                    }
                }
            }
            #endregion

            #region 回收逻辑
            if (IsMatchedScreen && !IsMatchedWorld)
            {
                for (int i = 0; i < Anchors_Layout_Screen.Count; i++)
                {
                    Anchor_Layout layout = Anchors_Layout_Screen[i];

                    for (int s = 0; s < layout.HudElementInfos.Count; s++)
                    {
                        if (element.ID == layout.HudElementInfos[s].ID)
                        {
                            x_id = layout.HudElementInfos[s].ID;
                            x_indicator = layout.HudElementInfos[s].Indicator;
                            finded = true;

                            HudElementNode node = Anchors_Layout_Screen[i].HudElementInfos[s];

                            ///---元素退出动作（需要注意的是：元素生成时可以一并指定元素退出动画后的动作委托，所以这里是需要先判断动作是否是空的）
                            if (node.Element.act_on_element_out_start == null)
                                node.Element.act_on_element_out_start += action_out_start;
                            if (node.Element.act_on_element_out_progress == null)
                                node.Element.act_on_element_out_progress += action_out_progress;
                            if (node.Element.act_on_element_out_end == null)
                                node.Element.act_on_element_out_end += action_out_end;

                            if (args == null)
                                args = RecycleArgs_Default;
                            node.Element.element_Out(args);
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < Anchors_Layout_World.Count; i++)
                {
                    if (element.ID == Anchors_Layout_World[i].ID)
                    {
                        x_id = Anchors_Layout_World[i].ID;
                        x_indicator = Anchors_Layout_World[i].Indicator;
                        finded = true;

                        HudElementNode node = Anchors_Layout_World[i];

                        ///---动作

                        if (node.Element.act_on_element_out_start == null)
                            node.Element.act_on_element_out_start += action_out_start;
                        if (node.Element.act_on_element_out_progress == null)
                            node.Element.act_on_element_out_progress += action_out_progress;
                        if (node.Element.act_on_element_out_end == null)
                            node.Element.act_on_element_out_end += action_out_end;

                        if (args == null)
                            args = RecycleArgs_Default;
                        node.Element.element_Out(args);
                    }
                }
            }
            #endregion

            #region 调试信息
            if (!finded)
            {
                if (UseDebug)
                    XHud_Utilitys.Func_PrintInfo("xHud Manager管理器消息", "Hud元素名称：" + element + " 不存在！未找到要回收的目标元素！", HudMsgState.通知);
            }
            else
            {
                if (UseDebug)
                    XHud_Utilitys.Func_PrintInfo("xHud Manager管理器消息", "已回收Hud元素：" + x_indicator + " / " + x_id, HudMsgState.通知);
            }
            #endregion
        }

        #endregion

        #region 获取

        /// <summary>
        /// xHud Manager管理器消息 - 根据目标ID从锚点列表中获取生成的Hud元素
        /// </summary>
        /// <param name="id">目标ID的元素</param>
        /// <returns>根据目标ID获取的元素</returns>
        public XHud_Module_Element hm_HudElement_Get(int id)
        {
            XHud_Module_Element element = null;
            for (int i = 0; i < Anchors_Layout_Screen.Count; i++)
            {
                for (int s = 0; s < Anchors_Layout_Screen[i].HudElementInfos.Count; s++)
                {
                    if (id == Anchors_Layout_Screen[i].HudElementInfos[s].ID)
                    {
                        element = Anchors_Layout_Screen[i].HudElementInfos[s].Element;
                    }
                }
            }
            return element;
        }

        /// <summary>
        ///  xHud Manager管理器消息 - 根据目标名称从锚点列表中获取生成的Hud元素
        /// </summary>
        /// <param name="name"></param>
        /// <returns>根据目标名称获取的元素</returns>
        public XHud_Module_Element hm_HudElement_Get(string name)
        {
            XHud_Module_Element element = null;
            for (int i = 0; i < Anchors_Layout_Screen.Count; i++)
            {
                for (int s = 0; s < Anchors_Layout_Screen[i].HudElementInfos.Count; s++)
                {
                    if (name == Anchors_Layout_Screen[i].HudElementInfos[s].Indicator)
                    {
                        element = Anchors_Layout_Screen[i].HudElementInfos[s].Element;
                    }
                }
            }
            return element;
        }

        /// <summary>
        ///  xHud Manager管理器消息 - 从锚点列表中获取所有已生成的Hud元素
        /// </summary>
        /// <returns>所有已生成到锚点里的Hud元素</returns>
        public XHud_Module_Element[] hm_HudElement_GetAll()
        {
            List<XHud_Module_Element> list = new List<XHud_Module_Element>();
            for (int i = 0; i < Anchors_Layout_Screen.Count; i++)
            {
                for (int s = 0; s < Anchors_Layout_Screen[i].HudElementInfos.Count; s++)
                {
                    list.Add(Anchors_Layout_Screen[i].HudElementInfos[s].Element);
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// xHud Manager管理器消息 - 获取已生成到锚点里的总Element数量
        /// </summary>
        /// <returns></returns>
        public int hm_HudElement_TotalCount()
        {
            int x = 0;
            for (int i = 0; i < Anchors_Layout_Screen.Count; i++)
            {
                for (int s = 0; s < Anchors_Layout_Screen[i].HudElementInfos.Count; s++)
                {
                    if (Anchors_Layout_Screen[i].HudElementInfos[s].Element != null)
                        x++;
                }
            }
            return x;
        }

        /// <summary>
        /// 主要在编辑器模式下用于获取所有锚点下的物体进行集控
        /// </summary>
        /// <returns></returns>
        public GameObject[] hm_AnchorChildsGet()
        {
            List<GameObject> list = new List<GameObject>();

            XHud_Manager man = FindFirstObjectByType<XHud_Manager>();

            for (int i = 0; i < man.HudCanvas_ScreenAnchor.childCount; i++)
            {
                Transform trans = man.HudCanvas_ScreenAnchor.GetChild(i);
                if (trans.childCount > 0)
                {
                    for (int s = 0; s < trans.childCount; s++)
                    {
                        GameObject obj = trans.GetChild(s).gameObject;
                        if (obj.hideFlags != HideFlags.HideInHierarchy)
                            list.Add(obj);
                    }
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// 主要在编辑器模式下用于获取所有锚点下的物体进行集控
        /// </summary>
        /// <returns></returns>
        public XHud_Module_Element[] hm_AnchorElementsGet()
        {
            XHud_Manager mgr = FindFirstObjectByType<XHud_Manager>();
            XHud_Module_Element[] trans = mgr.HudCanvas_ScreenAnchor.GetComponentsInChildren<XHud_Module_Element>();
            return trans;
        }

        public RectTransform hm_GetAnchorRoot(HudSpace Space)
        {
            if (Space == HudSpace.屏幕空间)
                return HudCanvas_ScreenAnchor;
            else
                return HudCanvas_WorldAnchor;
        }
        #endregion
    }
}