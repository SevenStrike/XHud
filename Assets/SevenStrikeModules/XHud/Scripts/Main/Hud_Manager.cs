namespace SevenStrikeModules.XHud.Hud
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using SevenStrikeModules.XHud.Enums;
    using UnityEngine.UI;
    using UnityEngine.Events;
    using DG.Tweening;
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
        public Hud_Element Element;
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
        public HudAnchor Type;
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
        /// <param tweenName="Name"></param>
        public Anchor_Layout(string Name)
        {
            switch (Name)
            {
                case "Anchor_U":
                    Type = HudAnchor.上;
                    break;
                case "Anchor_D":
                    Type = HudAnchor.下;
                    break;
                case "Anchor_L":
                    Type = HudAnchor.左;
                    break;
                case "Anchor_R":
                    Type = HudAnchor.右;
                    break;
                case "Anchor_C":
                    Type = HudAnchor.中心;
                    break;
                case "Anchor_L_U":
                    Type = HudAnchor.左上;
                    break;
                case "Anchor_L_D":
                    Type = HudAnchor.左下;
                    break;
                case "Anchor_R_U":
                    Type = HudAnchor.右上;
                    break;
                case "Anchor_R_D":
                    Type = HudAnchor.右下;
                    break;
                case "Anchor_B":
                    Type = HudAnchor.底层;
                    break;
                case "Anchor_T":
                    Type = HudAnchor.顶层;
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
    /// 锚点结构 - 安全框
    /// </summary>
    [System.Serializable]
    public class Safe_Structure
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
        public HudAnchor Type;
        /// <summary>
        /// 标记物
        /// </summary>
        public Image Mark;
        /// <summary>
        /// 锚点坐标
        /// </summary>
        public Vector2 Pivot;

        /// <summary>
        /// 实例化锚点节点
        /// </summary>
        /// <param tweenName="Name"></param>
        public Safe_Structure(string Name)
        {
            switch (Name)
            {
                case "Anchor_U":
                    Type = HudAnchor.上;
                    Pivot.Set(0.5f, 1f);
                    break;
                case "Anchor_D":
                    Type = HudAnchor.下;
                    Pivot.Set(0.5f, 0f);
                    break;
                case "Anchor_L":
                    Type = HudAnchor.左;
                    Pivot.Set(0f, 0.5f);
                    break;
                case "Anchor_R":
                    Type = HudAnchor.右;
                    Pivot.Set(1f, 0.5f);
                    break;
                case "Anchor_C":
                    Type = HudAnchor.中心;
                    Pivot.Set(0.5f, 0.5f);
                    break;
                case "Anchor_L_U":
                    Type = HudAnchor.左上;
                    Pivot.Set(0f, 1f);
                    break;
                case "Anchor_L_D":
                    Type = HudAnchor.左下;
                    Pivot.Set(0f, 0f);
                    break;
                case "Anchor_R_U":
                    Type = HudAnchor.右上;
                    Pivot.Set(1f, 1f);
                    break;
                case "Anchor_R_D":
                    Type = HudAnchor.右下;
                    Pivot.Set(1f, 0f);
                    break;
                case "Anchor_B":
                    Type = HudAnchor.底层;
                    Pivot.Set(0.5f, 0.5f);
                    break;
                case "Anchor_T":
                    Type = HudAnchor.顶层;
                    Pivot.Set(0.5f, 0.5f);
                    break;
            }
            this.Name = "锚点： " + Type.ToString();
        }
    }

    /// <summary>
    /// 安全框 => 框线
    /// </summary>
    [System.Serializable]
    public class Safe_FrameLine
    {
        public Image Frame;
        public HudAnchor Type;
    }

    /// <summary>
    /// 安全框 => 中心标记物
    /// </summary>
    [System.Serializable]
    public class Safe_CenterMark
    {
        public List<Image> Edge = new List<Image>();
        public Image Center;
    }

    /// <summary>
    /// 安全框 => 分割线
    /// </summary>
    [System.Serializable]
    public class Safe_Seperater
    {
        public Image Line;
        public HudAnchor Type;
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
        public HudAnchor anchor = HudAnchor.中心;
        public MotionAnimateEndState MotionAnimateEndState = MotionAnimateEndState.以_透明度为准;
        public MotionNode_Alpha Alpha = new MotionNode_Alpha();
        public MotionNode_Movement Movement = new MotionNode_Movement();
        public MotionNode_Rotation Rotation = new MotionNode_Rotation();

        /// <summary>
        /// 初始化元素参数
        /// </summary>
        /// <param tweenName="m_anchortype">目标固定锚点</param>
        /// <param tweenName="m_movement">运动样式</param>
        /// <param tweenName="m_distance">位移距离</param>
        /// <param tweenName="m_movement_duration">动画速度 - 位移</param>
        /// <param tweenName="m_movement_delay">动画延迟 - 位移</param>
        /// <param tweenName="m_movement_curve">位移运动曲线</param>
        /// <param tweenName="m_movement_ease">位移运动缓动参数</param>
        /// <param tweenName="m_alpha_duration">动画速度 - 透明度_Alpha</param>
        /// <param tweenName="m_alpha_delay">动画延迟 - 透明度_Alpha</param>
        /// <param tweenName="m_alpha_curve">透明度变化曲线</param>
        /// <param tweenName="m_alpha_ease">透明度变化缓动参数</param>
        /// <param tweenName="m_rotation">旋转样式</param>
        /// <param tweenName="m_degree">旋转角度</param>
        /// <param tweenName="m_rotation_duration">动画速度 - 旋转_Rotation</param>
        /// <param tweenName="m_rotation_delay">动画延迟 - 旋转_Rotation</param>
        /// <param tweenName="m_rotation_curve">旋转运动曲线</param>
        /// <param tweenName="m_rotation_ease">旋转运动缓动参数</param>
        public Motion_Creator(HudAnchor m_anchortype = HudAnchor.中心, HudMotion_Movement m_movement = HudMotion_Movement.D_从上至下, float m_distance = 100, float m_movement_duration = 1f, float m_movement_delay = 0f, AnimationCurve m_movement_curve = null, string m_movement_curve_name = "", Ease m_movement_ease = Ease.OutQuart, float m_alpha_duration = 1f, float m_alpha_delay = 0f, AnimationCurve m_alpha_curve = null, string m_alpha_curve_name = "", Ease m_alpha_ease = Ease.OutQuart, HudMotion_Rotation m_rotation = HudMotion_Rotation.A_无旋转, float m_degree = 0, float m_rotation_duration = 1f, float m_rotation_delay = 0f, AnimationCurve m_rotation_curve = null, string m_rotation_curve_name = "", Ease m_rotation_ease = Ease.OutQuart)
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
        /// <param tweenName="m_movement">运动样式</param>
        /// <param tweenName="m_distance">位移距离</param>
        /// <param tweenName="m_movement_duration">动画速度 - 位移</param>
        /// <param tweenName="m_movement_delay">动画延迟 - 位移</param>
        /// <param tweenName="m_movement_curve">位移运动曲线</param>
        /// <param tweenName="m_movement_ease">位移运动缓动参数</param>
        /// <param tweenName="m_alpha_duration">动画速度 - 透明度_Alpha</param>
        /// <param tweenName="m_alpha_delay">动画延迟 - 透明度_Alpha</param>
        /// <param tweenName="m_alpha_curve">透明度变化曲线</param>
        /// <param tweenName="m_alpha_ease">透明度变化缓动参数</param>
        /// <param tweenName="m_rotation">旋转样式</param>
        /// <param tweenName="m_degree">旋转角度</param>
        /// <param tweenName="m_rotation_duration">动画速度 - 旋转_Rotation</param>
        /// <param tweenName="m_rotation_delay">动画延迟 - 旋转_Rotation</param>
        /// <param tweenName="m_rotation_curve">旋转运动曲线</param>
        /// <param tweenName="m_rotation_ease">旋转运动缓动参数</param>
        public Motion_Recycler(HudMotion_Movement m_movement = HudMotion_Movement.D_从上至下, float m_distance = 100, float m_movement_duration = 1f, float m_movement_delay = 0f, AnimationCurve m_movement_curve = null, string m_movement_curve_index = "", Ease m_movement_ease = Ease.InQuart, float m_alpha_duration = 1f, float m_alpha_delay = 0f, AnimationCurve m_alpha_curve = null, string m_alpha_curve_index = "", Ease m_alpha_ease = Ease.InQuart, HudMotion_Rotation m_rotation = HudMotion_Rotation.A_无旋转, float m_degree = 0, float m_rotation_duration = 1f, float m_rotation_delay = 0f, AnimationCurve m_rotation_curve = null, string m_rotation_curve_index = "", Ease m_rotation_ease = Ease.InQuart)
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
        public Ease Ease = Ease.OutQuart;

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
        public Ease Ease = Ease.OutQuart;

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
        public Ease Ease = Ease.OutQuart;

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
        public List<Hud_Element> elements = new List<Hud_Element>();
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

    /// <summary>
    /// Hud Manager管理器消息
    /// </summary>
    public class Hud_Manager : MonoBehaviour
    {
        #region 单例模式
        protected Hud_Manager() { }
        private static Hud_Manager _instance;
        public static Hud_Manager Instance
        {
            get
            {
                if (_instance == null)
                {
                    util_Tools.Func_PrintInfo("Hud Manager管理器消息", "HudManager 还没有被实例化！", HudMsgState.确认);
                }
                return _instance;
            }
        }
        /// <summary>
        /// 检测单例状态
        /// </summary>
        private void InstanceModeCheck()
        {
            if (!UseInstanceMode)
                return;
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        #endregion

        #region 音效池
        [Tooltip("音效池")]
        /// <summary>
        /// 音效池
        /// </summary>
        public AudioPlayer[] Pool_Sounder;
        [Tooltip("音效池预加载数量")]
        /// <summary>
        /// 音效池预加载数量
        /// </summary>
        public int SounderPoolCount;
        #endregion

        #region UI 相机
        [Tooltip("UI事件相机")]
        /// <summary>
        /// UI事件相机
        /// </summary>
        public Camera HudCamera;
        [Tooltip("近距剪切")]
        /// <summary>
        /// 近距剪切
        /// </summary>
        public float CameraCutter_Near = 0.01f;
        [Tooltip("远距剪切")]
        /// <summary>
        /// 远距剪切
        /// </summary>
        public float CameraCutter_Far = 1f;
        [Tooltip("是否使用正交投影")]
        /// <summary>
        /// 是否使用正交投影
        /// </summary>
        public bool CameraOthograpicMode;
        [Tooltip("正交投影尺寸")]
        /// <summary>
        /// 正交投影尺寸
        /// </summary>
        public float CameraOrthographicSize = 1f;
        [Range(0, 180)]
        [Tooltip("透视投影尺寸")]
        /// <summary>
        /// 透视投影尺寸
        /// </summary>
        public float CameraFov = 60f;
        #endregion

        #region 物理参考比例
        /// <summary>
        /// 物理世界中的显示器的尺寸大小
        /// </summary>
        public Vector2 PhysicsScreenSize;
        /// <summary>
        /// 参考尺寸
        /// </summary>
        public Vector2 PhysicsReferenceSize;
        /// <summary>
        /// 参考比例标识显示组件
        /// </summary>
        public Image Reference_Image;
        /// <summary>
        /// 参考比例标识显示颜色
        /// </summary>
        public Color Reference_Image_Color;
        public XHUdRatioReferenceRes Res_Full;
        public XHUdRatioReferenceRes Res_Part;
        public Vector2 ReferShape_RatioSize = Vector2.one;
        public Vector2 ReferShape_RatioTolerance;
        #endregion

        #region 画布
        [Tooltip("Hud画布 - 屏幕空间")]
        /// <summary>
        /// Hud画布 - 屏幕空间
        /// </summary>
        public Canvas HudCanvas_Screen;
        [Tooltip("Hud画布 - 世界空间")]
        /// <summary>
        /// Hud画布 - 世界空间
        /// </summary>
        public Canvas HudCanvas_World;
        [Tooltip("Hud画布 - 屏幕空间缩放器")]
        /// <summary>
        /// Hud画布 - 屏幕空间缩放器
        /// </summary>
        public CanvasScaler HudCanvasScaler;
        [Tooltip("Hud画布 - 屏幕空间缩放模式索引")]
        /// <summary>
        /// Hud画布 - 屏幕空间缩放模式索引
        /// </summary>
        public int CanvasScalerModeIndex;
        [Range(0, 1)]
        [Tooltip("高度优先匹配值")]
        /// <summary>
        /// 高度优先匹配值
        /// </summary>
        public float CanvasMatchDir;
        [Tooltip("画布尺寸")]
        /// <summary>
        /// 画布尺寸
        /// </summary>
        public Vector2 CanvasScalerScreenSize;
        [Tooltip("Hud画布锚点")]
        /// <summary>
        /// Hud画布锚点
        /// </summary>
        public CanvasAnchor HudCanvasAnchor = CanvasAnchor.CameraFar;
        [Tooltip("画布距离模式索引")]
        /// <summary>
        /// 画布距离模式索引
        /// </summary>
        public int HudCanvasAnchorIndex;
        [Tooltip("画布和相机的距离")]
        /// <summary>
        /// 画布和相机的距离
        /// </summary>
        public float CanvasDistance = 0.5f;
        [Tooltip("是否使用像素对齐")]
        /// <summary>
        /// 是否使用像素对齐
        /// </summary>
        public bool UsePerfectPixelUpdate;
        [Tooltip("是否使用运动停止后自动像素对齐")]
        /// <summary>
        /// 是否使用运动停止后自动像素对齐
        /// </summary>
        public bool UseAutoPerfectPixel;
        #endregion

        #region 锚点布局
        [Tooltip("Hud画布-屏幕锚点")]
        /// <summary>
        /// Hud画布-屏幕锚点
        /// </summary>
        public RectTransform HudCanvas_ScreenAnchor;
        [Tooltip("Hud画布-世界锚点")]
        /// <summary>
        /// Hud画布-世界锚点
        /// </summary>
        public RectTransform HudCanvas_WorldAnchor;
        [Tooltip("世界锚点列表")]
        /// <summary>
        /// 世界锚点列表
        /// </summary>
        public List<HudElementNode> Anchors_Layout_World = new List<HudElementNode>();
        [Tooltip("屏幕锚点列表")]
        /// <summary>
        /// 屏幕锚点列表
        /// </summary>
        public List<Anchor_Layout> Anchors_Layout_Screen = new List<Anchor_Layout>();
        [Tooltip("X：上边距 | Y：下边距 | Z：左边距 | W：右边距")]
        /// <summary>
        /// 屏幕边距
        /// </summary>
        public Vector4 Margins = Vector4.one * 10f;
        [Tooltip("水平边距")]
        /// <summary>
        /// 屏幕边距 - 水平
        /// </summary>
        public float MarginHorizontal = 1f;
        [Tooltip("垂直边距")]
        /// <summary>
        /// 屏幕边距 - 垂直
        /// </summary>
        public float MarginVertical = 1f;
        [Tooltip("边距倍增")]
        /// <summary>
        /// 主要方向锚点尺寸倍增
        /// </summary>
        public float MarginMultiply = 1f;
        [Tooltip("当前屏幕分辨率尺寸")]
        /// <summary>
        /// 屏幕分辨率
        /// </summary>
        public Vector2 ScreenRes;
        [Tooltip("是否使用了布局可视化")]
        /// <summary>
        /// 是否使用了布局可视化
        /// </summary>
        public bool UseSafeFrame;
        #endregion

        #region 辅助视觉安全框
        /// <summary>
        /// 锚点可视化尺寸
        /// </summary>
        public float MarkSize = 10f;
        /// <summary>
        /// 是否在hierarchy中显示安全框结构
        /// </summary>
        public bool SafeFrameStructureDisplayer;
        [Tooltip("安全框锚点物体")]
        /// <summary>
        /// 安全框锚点物体
        /// </summary>
        public Transform Safe_Frame;
        [Tooltip("锚点颜色 - 角点")]
        /// <summary>
        /// 锚点颜色 - 角点
        /// </summary>
        public Color Color_LayoutAnchorMark = Color.white;
        [Tooltip(" 锚点颜色 - 中心点")]
        /// <summary>
        /// 锚点颜色 - 中心点
        /// </summary>
        public Color Color_CenterMark = util_Dashboard.Theme_Primary;
        [Tooltip("安全框颜色")]
        /// <summary>
        /// 安全框颜色
        /// </summary>
        public Color Color_FrameLine = Color.white * 0.7f;
        [Tooltip("锚点颜色 - 主要方向")]
        /// <summary>
        /// 锚点颜色 - 主要方向
        /// </summary>
        public Color Color_SeperaterLine = util_Dashboard.Theme_Primary;
        [Tooltip("安全框粗细")]
        /// <summary>
        /// 安全框粗细
        /// </summary>
        public float Safe_FrameLine_Width = 0.5f;
        [Tooltip("安全框元素组")]
        /// <summary>
        /// 安全框元素组
        /// </summary>
        public Safe_FrameLine[] Safe_FrameLine;
        [Tooltip("安全框边距")]
        /// <summary>
        /// 安全框边距
        /// </summary>
        public Vector2 Safe_FrameLine_Margins = Vector2.one * 50f;
        [Tooltip("安全框中心线组")]
        /// <summary>
        /// 安全框中心线组
        /// </summary>
        public Safe_Seperater[] Safe_Seperater;
        [Tooltip("安全框中心线长度")]
        /// <summary>
        /// 安全框中心线长度
        /// </summary>
        public float Safe_Seperater_Length = 30f;
        [Tooltip("X：水平边距 | Y：垂直边距")]
        /// <summary>
        /// 中心标记物
        /// </summary>
        public Safe_CenterMark Safe_CenterMarks;
        [Tooltip("安全框中心标记 - 间距")]
        /// <summary>
        /// 安全框中心标记 - 间距
        /// </summary>
        public float Safe_CenterMarkDistance = 40f;
        [Tooltip("安全框中心标记 - 宽度")]
        /// <summary>
        /// 安全框中心标记 - 宽度
        /// </summary>
        public float Safe_CenterMarkWidth = 1f;
        [Tooltip("安全框中心标记 - 长度")]
        /// <summary>
        /// 安全框中心标记 - 长度
        /// </summary>
        public float Safe_CenterMarkLength = 12f;
        #endregion

        #region 布局构图参考线
        [Tooltip("Hud画布-辅助构图锚点")]
        /// <summary>
        /// Hud画布-辅助构图锚点
        /// </summary>
        public RectTransform CompGuide_AnchorRoot;
        [SerializeField]
        /// <summary>
        /// 布局构图参考线锚点集合
        /// </summary>
        public AuxiliaryAnchor_Layout[] CompGuide_Anchors;
        [SerializeField]
        public string CompGuideMode = "水平对称";
        [SerializeField]
        private float GuideParam_CenterPointSize = 1f;

        #region 左右对称构图
        [SerializeField]
        [Range(-1, 1)]
        public float GuideParam_Mirror_LR_Offset;
        [SerializeField]
        public string GuideParam_Mirror_LR_GoldenMode = "自定义对称分割";
        #endregion

        #region 上下对称构图
        [SerializeField]
        [Range(-1, 1)]
        public float GuideParam_Mirror_UD_Offset;
        [SerializeField]
        public string GuideParam_Mirror_UD_GoldenMode = "自定义对称分割";
        #endregion

        #region 黄金螺旋构图      
        [SerializeField]
        public string GuideParam_Fibonacci_Mode = "右上";
        private Vector3 previousPoint;
        #endregion

        #region 对角线构图      
        [SerializeField]
        [Range(0, 1)]
        private float GuideParam_CornerLookat_Offset_H;
        [SerializeField]
        [Range(0, 1)]
        private float GuideParam_CornerLookat_Offset_V;
        #endregion

        #region 三分线构图      
        [SerializeField]
        [Range(0, 1)]
        private float GuideParam_Three_Offset_H = 0.5f;
        [SerializeField]
        [Range(0, 1)]
        private float GuideParam_Three_Offset_V = 0.5f;
        [SerializeField]
        [Range(0, 1)]
        private float GuideParam_Three_Offset_Coverage = 0.5f;
        #endregion

        #region 引导线构图
        [SerializeField]
        [Range(-1, 1)]
        private float GuideParam_GuideLine_BaseOffset = 0;
        [SerializeField]
        [Range(0, 1)]
        private float GuideParam_GuideLine_BaseHeight = 0.5f;
        [SerializeField]
        [Range(0, 1)]
        private float GuideParam_GuideLine_Offset_Far;
        [SerializeField]
        [Range(0, 1)]
        private float GuideParam_GuideLine_Offset_Near;
        [SerializeField]
        [Range(0, 1)]
        private float GuideParam_GuideLine_Offset_NearHeight;
        #endregion

        #region 三角构图
        [SerializeField]
        [Range(-1, 1)]
        private float GuideParam_Triangle_TopOffset = 0f;
        [SerializeField]
        [Range(0, 1)]
        private float GuideParam_Triangle_BaseHeight = 0.8f;
        [SerializeField]
        [Range(0, 1)]
        private float GuideParam_Triangle_BottomHeight = 0.2f;
        [SerializeField]
        [Range(0, 1)]
        private float GuideParam_Triangle_Offset_Left = 0.5f;
        [SerializeField]
        [Range(0, 1)]
        private float GuideParam_Triangle_Offset_Right = 0.5f;
        #endregion

        #region 工字型
        [SerializeField]
        [Range(0, 1)]
        private float GuideParam_IShape_TopHeight = 0.8f;
        [SerializeField]
        [Range(0, 1)]
        private float GuideParam_IShape_BottomHeight = 0.2f;
        [SerializeField]
        [Range(0, 1)]
        private float GuideParam_IShape_Offset_Left = 0.5f;
        [SerializeField]
        [Range(0, 1)]
        private float GuideParam_IShape_Offset_Right = 0.5f;
        #endregion

        public Color GuideColor = new Color(0.5f, 0.5f, 0.5f, 0.2f);
        public Color GuidePointColor = util_Dashboard.Theme_Primary;
        #endregion

        #region 屏幕遮罩
        [Tooltip("屏幕遮罩")]
        /// <summary>
        /// 屏幕遮罩
        /// </summary>
        public Image Mask;
        [Range(0, 1)]
        [Tooltip("遮罩透明度")]
        /// <summary>
        /// 遮罩透明度
        /// </summary>
        public float MaskAlpha;
        [Range(0, 1)]
        [Tooltip("当遮罩透明度大于此值时遮罩的射线点击特性(Raycast)处于启用")]
        /// <summary>
        /// 当遮罩透明度大于此值时遮罩的射线点击特性(Raycast)处于启用
        /// </summary>
        public float MaskRaycastAlphaThreshold = 1;
        [Tooltip("是否开启遮罩射线遮挡")]
        /// <summary>
        /// 是否开启遮罩射线遮挡
        /// </summary>
        public bool MaskRaycastEnabled;
        [Tooltip("遮罩颜色")]
        /// <summary>
        /// 遮罩颜色
        /// </summary>
        public Color MaskColor = Color.clear;
        [Tooltip("遮罩贴图")]
        /// <summary>
        /// 遮罩贴图
        /// </summary>
        public Texture2D MaskTexture;
        [Tooltip("动画组件 - 遮罩透明度")]
        /// <summary>
        /// 动画组件 - 遮罩透明度
        /// </summary>
        private Tweener twn_MaskAlpha;
        [Tooltip("动画组件 - 遮罩颜色通道 R")]
        /// <summary>
        /// 动画组件 - 遮罩颜色通道 R
        /// </summary>
        private Tweener twn_MaskColor_R;
        [Tooltip("动画组件 - 遮罩颜色通道 G")]
        /// <summary>
        /// 动画组件 - 遮罩颜色通道 G
        /// </summary>
        private Tweener twn_MaskColor_G;
        [Tooltip("动画组件 - 遮罩颜色通道 B")]
        /// <summary>
        /// 动画组件 - 遮罩颜色通道 B
        /// </summary>
        private Tweener twn_MaskColor_B;
        #endregion

        #region 场景散焦遮罩
        [Tooltip("散焦遮罩")]
        /// <summary>
        /// 散焦遮罩
        /// </summary>
        public Image BlurMask;
        [Range(0, 1)]
        [Tooltip("散焦遮罩透明度")]
        /// <summary>
        /// 遮罩透明度
        /// </summary>
        public float BlurMaskAlpha;
        [Range(0, 1)]
        [Tooltip("当散焦遮罩透明度大于此值时散焦遮罩的射线点击特性(Raycast)处于启用")]
        /// <summary>
        /// 当散焦遮罩透明度大于此值时散焦遮罩的射线点击特性(Raycast)处于启用
        /// </summary>
        public float BlurMaskRaycastAlphaThreshold = 1;
        [Tooltip("是否开启散焦遮罩射线遮挡")]
        /// <summary>
        /// 是否开启散焦遮罩射线遮挡
        /// </summary>
        public bool BlurMaskRaycastEnabled;
        [Tooltip("散焦遮罩颜色")]
        /// <summary>
        /// 散焦遮罩颜色
        /// </summary>
        public Color BlurMaskColor = Color.white;
        [Tooltip("散焦遮罩贴图")]
        /// <summary>
        /// 散焦遮罩贴图
        /// </summary>
        public Texture2D BlurMaskTexture;
        [Tooltip("动画组件 - 散焦遮罩透明度")]
        /// <summary>
        /// 动画组件 - 散焦遮罩透明度
        /// </summary>
        private Tweener twn_BlurMaskAlpha;
        [Tooltip("动画组件 - 散焦遮罩颜色通道 R")]
        /// <summary>
        /// 动画组件 - 散焦遮罩颜色通道 R
        /// </summary>
        private Tweener twn_BlurMaskColor_R;
        [Tooltip("动画组件 - 散焦遮罩颜色通道 G")]
        /// <summary>
        /// 动画组件 - 散焦遮罩颜色通道 G
        /// </summary>
        private Tweener twn_BlurMaskColor_G;
        [Tooltip("动画组件 - 散焦遮罩颜色通道 B")]
        /// <summary>
        /// 动画组件 - 散焦遮罩颜色通道 B
        /// </summary>
        private Tweener twn_BlurMaskColor_B;

        #region 渲染特性
        [Range(0, 1)]
        public float UniversalFeature_Blur_Intensity;

        [SerializeField]
        /// <summary>
        /// 特性 - 散焦模糊
        /// </summary>
        public UniversalBlurFeature UniversalFeature_Blur { get; set; }

        [Tooltip("渲染特性 - 散焦模糊效果")]
        /// <summary>
        /// 特性 - 散焦模糊 - 动画
        /// </summary>
        private Tweener twn_UniversalFeature_Blur;
        /// <summary>
        /// 特性 - 散焦模糊强度到达最大
        /// </summary>
        private bool FeatureBlur_IntensityToMax;
        /// <summary>
        /// 特性 - 散焦模糊强度到达最小
        /// </summary>
        private bool FeatureBlur_IntensityToMin;

        #endregion
        #endregion

        #region 屏幕/世界内容透明度组件
        [Tooltip("屏幕内容透明度控制组件")]
        /// <summary>
        /// 屏幕内容透明度控制组件
        /// </summary>
        public CanvasGroup HudCanvasGroup_Screen;
        [Tooltip("世界内容透明度控制组件")]
        /// <summary>
        /// 世界内容透明度控制组件
        /// </summary>
        public CanvasGroup HudCanvasGroup_World;
        [Range(0, 1)]
        [Tooltip("屏幕内容透明度")]
        /// <summary>
        /// 屏幕内容透明度
        /// </summary>
        public float ContentAlpha_Screen = 1f;
        [Range(0, 1)]
        [Tooltip("世界内容透明度")]
        /// <summary>
        /// 世界内容透明度
        /// </summary>
        public float ContentAlpha_World = 1f;
        [Tooltip("世界内容透明度")]
        /// <summary>
        /// 世界内容透明度
        /// </summary>
        private Tweener twn_ContentAlpha_Screen;
        [Tooltip("世界内容透明度")]
        /// <summary>
        /// 世界内容透明度
        /// </summary>
        private Tweener twn_ContentAlpha_World;
        #endregion

        #region 元素库
        [Tooltip("此元素库用于配置存放场景中需要调用的元素预制体")]
        /// <summary>
        /// 元素库 - 配置化（Editor期间）
        /// </summary>
        public List<Hud_ElementLibrary> Hud_ElementLibrarys = new List<Hud_ElementLibrary>();
        #endregion

        #region 资源库类
        [Tooltip("曲线库")]
        /// <summary>
        /// 曲线库
        /// </summary>
        public Hud_CurvesLibrary Hud_Curves;
        [Tooltip("调色板")]
        /// <summary>
        /// 调色板
        /// </summary>
        public Hud_ColorsLibrary Hud_Colors;
        [Tooltip("音效库")]
        /// <summary>
        /// 音效库
        /// </summary>
        public Hud_SoundsLibrary Hud_Sounds;
        [Tooltip("字体库")]
        /// <summary>
        /// 字体库
        /// </summary>
        public Hud_TextStyleLibrary Hud_TextStyleLibrary;
        [Tooltip("光标样式")]
        /// <summary>
        /// 光标样式
        /// </summary>
        public Hud_MouseCursor Hud_MouseCursor;
        [Tooltip("转场")]
        /// <summary>
        /// 转场
        /// </summary>
        public Hud_TransitionLibrary Hud_TransitionLib;
        [Tooltip("元素动效库")]
        /// <summary>
        /// 元素动效库
        /// </summary>
        public Hud_MotionLibrary Hud_ElementMotion;
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
        [Tooltip("场景相机")]
        /// <summary>
        /// 场景相机
        /// </summary>
        public Camera SceneCamera;
        [Tooltip("转场器组件")]
        /// <summary>
        /// 转场器组件
        /// </summary>
        public Hud_TransitionController Hud_TransitionController;
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
        #endregion

        #region 音效声音
        [Range(0, 100)]
        [Tooltip("影响所有UI的音效音量")]
        /// <summary>
        /// 音效音量
        /// </summary>        
        public float Volume = 100;
        [Tooltip("勾选后所有音效都会静音")]
        /// <summary>
        /// 音效静音
        /// </summary>
        public bool VolumeMute;
        #endregion

        #region 全局字体缩放
        [Tooltip("影响所有UI的字体尺寸")]
        /// <summary>
        /// 影响所有UI的字体尺寸
        /// </summary>
        public float FontSizeMultiply = 1;
        #endregion

        #region 蓝图视觉
        [Range(0, 1)]
        [Tooltip("蓝图视觉整体透明度")]
        /// <summary>
        /// 蓝图视觉透明度
        /// </summary>
        public float BluePrint_opacity = 1;
        [Tooltip("蓝图视觉总体淡化动画")]
        /// <summary>
        /// 蓝图视觉总体淡化动画
        /// </summary>
        private Tweener BluePrint_opacityTweener;
        [Tooltip("蓝图视觉网格淡化动画")]
        /// <summary>
        /// 蓝图视觉网格淡化动画
        /// </summary>
        private Tweener BluePrint_grid_opacityTweener;
        [Tooltip("蓝图视觉网格尺寸")]
        /// <summary>
        /// 蓝图视觉网格尺寸
        /// </summary>
        public float BluePrint_grid_size = 35;
        [Tooltip("蓝图视觉网格线宽度")]
        /// <summary>
        /// 蓝图视觉网格线宽度
        /// </summary>
        public float BluePrint_linewidth = 1;
        [Tooltip("蓝图视觉网格线颜色")]
        /// <summary>
        /// 蓝图视觉网格线颜色
        /// </summary>
        public Color BluePrint_grid_color = util_Tools.Color_From_RGBA(166, 166, 166, 26);
        [Tooltip("蓝图视觉背景颜色")]
        /// <summary>
        /// 蓝图视觉背景颜色
        /// </summary>
        public Color BluePrint_bg_color = util_Tools.Color_From_RGBA(55, 55, 55, 255);
        [Tooltip("蓝图视觉叠加颜色")]
        /// <summary>
        /// 蓝图视觉叠加颜色
        /// </summary>
        public Color BluePrint_bg_decal_color = util_Tools.Color_From_RGBA(0, 0, 0, 80);
        [Tooltip("蓝图视觉背景图像索引名称")]
        /// <summary>
        /// 蓝图视觉背景图像索引名称
        /// </summary>
        public string BluePrint_bg_name = "";
        [Range(0, 1)]
        [Tooltip("蓝图视觉网格线透明度")]
        /// <summary>
        /// 蓝图视觉网格线透明度
        /// </summary>
        public float BluePrint_grid_Opacity = 1;
        [Range(0, 1)]
        [Tooltip("蓝图视觉背景图像透明度")]
        /// <summary>
        /// 蓝图视觉背景图像透明度
        /// </summary>
        public float BluePrint_bg_mapOpacity = 1;
        [Tooltip("蓝图视觉背景图像平铺模式开关")]
        /// <summary>
        /// 蓝图视觉背景图像平铺模式开关
        /// </summary>
        public int BluePrint_bg_usetilling_index = 0;
        [Tooltip("蓝图视觉背景图像强制方形比例")]
        /// <summary>
        /// 蓝图视觉背景图像强制方形比例
        /// </summary>
        public int BluePrint_bg_usesquareratio_index = 0;
        [Range(0, 1)]
        [Tooltip("蓝图视觉背景透明度")]
        /// <summary>
        /// 蓝图视觉背景透明度
        /// </summary>
        public float BluePrint_bg_opacity = 1;
        [Tooltip("蓝图视觉背景图像平铺")]
        /// <summary>
        /// 蓝图视觉背景图像平铺
        /// </summary>
        public float BluePrint_bg_tilling = 1;
        [Tooltip("蓝图视觉大标题颜色")]
        /// <summary>
        /// 蓝图视觉大标题颜色
        /// </summary>
        public Color BluePrint_marktitle_color = Color.white;
        [Tooltip("蓝图视觉小标题颜色")]
        /// <summary>
        /// 蓝图视觉小标题颜色
        /// </summary>
        public Color BluePrint_marksubtitle_color = util_Dashboard.Theme_Primary;
        [Tooltip("蓝图视觉标题尺寸")]
        /// <summary>
        /// 蓝图视觉标题尺寸
        /// </summary>
        public float BluePrint_mark_size = 40;
        [Range(0, 1)]
        [Tooltip("蓝图视觉标题透明度")]
        /// <summary>
        /// 蓝图视觉标题透明度
        /// </summary>
        public float BluePrint_mark_opacity = 1;
        [Tooltip("蓝图视觉标题边距")]
        /// <summary>
        /// 蓝图视觉标题边距
        /// </summary>
        public Vector4 BluePrint_mark_margin = new Vector4(60, 60, 60, 60);
        [Tooltip("蓝图视觉标题间距")]
        /// <summary>
        /// 蓝图视觉标题间距
        /// </summary>
        public float BluePrint_mark_space = 1;
        [Tooltip("蓝图视觉透明度组件")]
        /// <summary>
        ///蓝图视觉透明度组件
        /// </summary>
        public CanvasGroup BluePrint_canvasgroup;
        [Tooltip("蓝图视觉根节点")]
        /// <summary>
        ///蓝图视觉根节点
        /// </summary>
        public RectTransform BluePrint_root;
        [Tooltip("蓝图视觉背景组件")]
        /// <summary>
        ///蓝图视觉背景组件
        /// </summary>
        public Image BluePrint_mainbg;
        [Tooltip("蓝图视觉网格线数组H")]
        /// <summary>
        /// 蓝图视觉网格线数组V
        /// </summary>
        public List<Image> BluePrint_gridlines_H;
        [Tooltip("蓝图视觉网格线数组V")]
        /// <summary>
        /// 蓝图视觉网格线数组V
        /// </summary>
        public List<Image> BluePrint_gridlines_V;
        [Tooltip("蓝图视觉大标题组件")]
        /// <summary>
        /// 蓝图视觉大标题组件
        /// </summary>
        public Hud_TmpText BluePrint_title_module;
        [Tooltip("蓝图视觉小标题组件")]
        /// <summary>
        /// 蓝图视觉小标题组件
        /// </summary>
        public Hud_TmpText BluePrint_subtitle_module;
        [Tooltip("蓝图视觉大标题内容")]
        /// <summary>
        /// 蓝图视觉大标题内容
        /// </summary>
        public string BluePrint_title_content = "XHUD CustomUI";
        [Tooltip("蓝图视觉小标题内容")]
        /// <summary>
        /// 蓝图视觉小标题内容
        /// </summary>
        public string BluePrint_subtitle_content = "powered by sevenstrike media";
        [Tooltip("蓝图视觉标题锚点")]
        /// <summary>
        /// 蓝图视觉标题锚点
        /// </summary>
        public IsolateVisualModeMarkAnchor BluePrint_MarkAnchors = IsolateVisualModeMarkAnchor.右下;
        [Tooltip("蓝图视觉背景动画速率")]
        /// <summary>
        /// 蓝图视觉背景动画速率
        /// </summary>
        public float BluePrint_AnimationDuration = 1;
        [Tooltip("蓝图视觉背景动画退场延迟")]
        /// <summary>
        /// 蓝图视觉背景动画退场延迟
        /// </summary>
        public float BluePrint_Bg_FadeAnimationDelay = 0;
        [Tooltip("蓝图视觉网格线动画速率")]
        /// <summary>
        /// 蓝图视觉网格线动画速率
        /// </summary>
        public float BluePrint_Grid_AnimationDuration = 1;
        [Tooltip("蓝图视觉背景淡化动画")]
        /// <summary>
        /// 蓝图视觉背景淡化动画
        /// </summary>
        private Tweener BluePrint_BgFadeTweener;
        [Tooltip("蓝图视觉水印淡化动画")]
        /// <summary>
        /// 蓝图视觉水印淡化动画
        /// </summary>
        private Tweener BluePrint_MarkFadeTweener;
        [Tooltip("蓝图网格状态")]
        /// <summary>
        /// 蓝图网格状态
        /// </summary>
        public bool BluePrint_Displayed = false;
        [Tooltip("蓝图视觉网格线动画起始时退场状态")]
        /// <summary>
        /// 蓝图视觉网格线动画起始时退场状态
        /// </summary>
        public bool BluePrint_OnStartHide = false;
        [Tooltip("蓝图视觉网格线动画缓动参数")]
        /// <summary>
        /// 蓝图视觉网格线动画缓动参数
        /// </summary>
        public Ease BluePrint_Grid_AnimationEase = Ease.OutQuart;
        [Tooltip("蓝图视觉背景动画缓动参数 - 入场")]
        /// <summary>
        /// 蓝图视觉背景动画缓动参数 - 入场
        /// </summary>
        public Ease BluePrint_Bg_AnimationEase_In = Ease.OutQuart;
        [Tooltip("蓝图视觉背景动画缓动参数 - 出场")]
        /// <summary>
        /// 蓝图视觉背景动画缓动参数 - 出场
        /// </summary>
        public Ease BluePrint_Bg_AnimationEase_Out = Ease.InQuart;
        [Tooltip("蓝图视觉网格长度百分比")]
        /// <summary>
        /// 蓝图视觉网格长度百分比
        /// </summary>
        public float BluePrint_Grid_LengthPercentage = 1;
        [Tooltip("蓝图视觉网格分级高差")]
        /// <summary>
        /// 蓝图视觉网格分级高差
        /// </summary>
        public float BluePrint_Grid_LevelHeight = 0;
        [Tooltip("网格动画结束百分比")]
        /// <summary>
        /// 网格动画结束百分比
        /// </summary>
        public float BluePrint_GridEnd = 1;
        [Tooltip("网格动画起始百分比")]
        /// <summary>
        /// 网格动画起始百分比
        /// </summary>
        public float BluePrint_GridStart;
        [Tooltip("蓝图视觉网格长度动画")]
        /// <summary>
        /// 蓝图模式网格长度动画
        /// </summary>
        private Tweener BluePrint_GridLengthTweener;
        public bool Eft_Grid;
        public bool Eft_GridFade;
        public bool Eft_Bg;
        public bool Eft_Mark;
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

        #region 事件委托
        /// <summary>
        /// 委托事件 - 元素库初始化后
        /// </summary>
        public UnityAction Act_ElementsLib_Instantiated;
        /// <summary>
        /// 委托事件 - 遮罩透明度变化时
        /// </summary>
        public UnityAction<float> Act_MaskChanged_Value;
        /// <summary>
        /// 委托事件 - 散焦遮罩透明度变化时
        /// </summary>
        public UnityAction<float> Act_BlurMaskChanged_Value;
        /// <summary>
        /// 委托事件 - 散焦特性强度最大时
        /// </summary>
        public UnityAction<float> Act_BlurMask_Intensity_IsMax;
        /// <summary>
        /// 委托事件 - 散焦特性强度最小时
        /// </summary>
        public UnityAction<float> Act_BlurMask_Intensity_IsMin;
        /// <summary>
        /// 委托事件 - 屏幕空间的内容的透明度最大时
        /// </summary>
        public UnityAction<float> Act_ContentOpacity_Screen_IsMax;
        /// <summary>
        /// 委托事件 - 屏幕空间的内容的透明度最小时
        /// </summary>
        public UnityAction<float> Act_ContentOpacity_Screen_IsMin;
        /// <summary>
        /// 委托事件 - 屏幕空间的内容的透明度变化时
        /// </summary>
        public UnityAction<float> Act_ContentOpacity_Screen_Changed;
        /// <summary>
        /// 委托事件 - 屏幕空间的内容的透明度最大时
        /// </summary>
        public UnityAction<float> Act_ContentOpacity_World_IsMax;
        /// <summary>
        /// 委托事件 - 屏幕空间的内容的透明度最小时
        /// </summary>
        public UnityAction<float> Act_ContentOpacity_World_IsMin;
        /// <summary>
        /// 委托事件 - 屏幕空间的内容的透明度变化时
        /// </summary>
        public UnityAction<float> Act_ContentOpacity_World_Changed;
        /// <summary>
        /// 委托事件 - 遮罩贴图变化时
        /// </summary>
        public UnityAction<Texture2D> Act_MaskChanged_Texture;
        /// <summary>
        /// 委托事件 - 散焦遮罩贴图变化时
        /// </summary>
        public UnityAction<Texture2D> Act_BlurMaskChanged_Texture;
        /// <summary>
        /// 委托事件 - 蓝图视觉进场
        /// </summary>
        public UnityAction Act_BluePrint_In;
        /// <summary>
        /// 委托事件 - 蓝图视觉退场
        /// </summary>
        public UnityAction Act_BluePrint_Out;
        /// <summary>
        /// 委托事件 - 蓝图视觉整体完全透明化
        /// </summary>
        public UnityAction Act_BluePrint_IsTransparency;
        /// <summary>
        /// 委托事件 - 蓝图视觉整体完全实心化
        /// </summary>
        public UnityAction Act_BluePrint_IsSolid;
        /// <summary>
        /// 委托事件 - 字体缩放变化
        /// </summary>
        public UnityAction Act_FontSize_Changed;
        /// <summary>
        /// 委托事件 - 分辨率变化
        /// </summary>
        public UnityAction<string, Vector2> Act_ScreenResolution_Changed;
        /// <summary>
        /// 委托事件 - 鼠标是否正在与UI交互
        /// </summary>
        public UnityAction<bool> Act_IsInteractionUI;
        /// <summary>
        /// 委托事件 - 全局音效音量变化时
        /// </summary>
        public UnityAction<float> Act_VolumeChanged_Value;
        /// <summary>
        /// 委托事件 - 全局速率变化时
        /// </summary>
        public UnityAction<float> Act_DurationChanged_Value;
        /// <summary>
        /// 委托事件 - 全局字体尺寸变化时
        /// </summary>
        public UnityAction<float> Act_FontSizeChanged_Value;
        /// <summary>
        /// 委托事件 - 生成元素时
        /// </summary>
        public UnityAction<Hud_Element> Act_SpawnElement;
        #endregion

        #region 多语言
        [SerializeField]
        public bool UseLocalization;
        #endregion

        #region 状态开关
        [Tooltip("单例模式开关")]
        /// <summary>
        /// 单例模式开关
        /// </summary>
        public bool UseInstanceMode = true;
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
        /// 中断开关 - 屏幕内容透明度到达最大
        /// </summary>
        private bool OpacityIsChangedToMax_Screen;
        /// <summary>
        /// 中断开关 - 屏幕内容透明度到达最小
        /// </summary>
        private bool OpacityIsChangedToMin_Screen;
        /// <summary>
        /// 中断开关 - 世界内容透明度到达最大
        /// </summary>
        private bool OpacityIsChangedToMax_World;
        /// <summary>
        /// 中断开关 - 世界内容透明度到达最小
        /// </summary>
        private bool OpacityIsChangedToMin_World;
        /// <summary>
        /// 使用自定义鼠标样式
        /// </summary>
        public bool CustomCursor;
        /// <summary>
        /// 颜色匹配
        /// </summary>
        public bool ColorMatcher;
        /// <summary>
        /// 使用转场库
        /// </summary>
        public bool CustomTransition;
        /// <summary>
        /// 支持世界UI
        /// </summary>
        public bool SupportWorldUI;
        /// <summary>
        /// 独立可视化背景
        /// </summary>
        public bool BluePrintMode;
        /// <summary>
        /// 是否正在与UI交互
        /// </summary>
        public bool IsInteractionUI;
        /// <summary>
        /// Led闪烁效果开启
        /// </summary>
        public bool EnabledLedEffect;
        /// <summary>
        /// 使用人形比例参考
        /// </summary>
        public bool UseRatioReference;
        /// <summary>
        /// 人形比例参考类型
        /// </summary>
        public bool RatioReferenceIsPart = true;
        /// <summary>
        /// 关闭面板后是否折叠所有选项卡
        /// </summary>
        public bool FoldAllPanelWithDisabled;
        /// <summary>
        /// 是否启用参考构图模式
        /// </summary>
        public bool UseCompGuide;
        #endregion

        #region 布局匹配方案(RMS = R esolution M atching S olution)
        /// <summary>
        ///布局匹配方案模式
        /// </summary>
        public bool RMS_Enabled;
        /// <summary>
        /// 布局匹配方案列表
        /// </summary>
        public List<ScreenResolutionNode> RMS_Nodes = new List<ScreenResolutionNode>();
        /// <summary>
        /// RMS当前选中的方案
        /// </summary>
        public string RMS_CurrentSolution;
        #endregion

        #region 主题色 Themes
        [SerializeField]
        public Vector3 theme_color;
        [SerializeField]
        public Vector3 theme_color_gp;
        [SerializeField]
        public Vector3 theme_color_sep;
        [SerializeField]
        public string ThemeSolution = "默认";
        [SerializeField]
        public string ThemeEdgeSolution = "默认";
        #endregion

        #region 判断是否正在与UI进行交互
        bool Interacte_Valid;
        bool Interacte_Invalid;
        #endregion

        void Awake()
        {
            InstanceModeCheck();

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
                Hud_MouseCursor = GetComponentInChildren<Hud_MouseCursor>();

            if (Hud_TransitionController == null)
                Hud_TransitionController = GetComponentInChildren<Hud_TransitionController>();

            #region 启动时隐藏蓝图
            if (BluePrint_OnStartHide)
            {
                hm_BluePrint_Hide();
                BluePrint_Displayed = false;
            }
            else
            {
                BluePrint_Displayed = true;
            }
            #endregion

            #region 让蓝图结构永远位于底层
            hm_BluePrintRootFirstSibling();
            #endregion

            hm_UniversalFeature_Blur_Get();

            hm_ElementLibrary_Initialize();

            hm_LibrarySounds_Initialize(SounderPoolCount);

            StartCoroutine(CheckUIInteraction());
        }

        private void Start()
        {

        }

        void Update()
        {
            hm_GetScreenResolution();
            hm_HudElement_UpdateAnimating();
            hm_MaskUpdate();
            hm_BlurMaskUpdate();
            hm_TransitionUpdate();
            hm_UniversalFeature_Blur_Update();
            hm_ContentAlpha_Update();
            hm_LibrarySounds_Update();
            hm_ElementLibrary_UpdateStates();
            hm_SafeFrameUpdate();
            hm_Layout_CanvasDistance_Update();
            hm_Layout_Update();
            hm_Layout_CanvasDistance(HudCanvasAnchor);
            hm_CameraCutterRange(CameraCutter_Near, CameraCutter_Far);
            hm_CameraOrthographicProjection(CameraOthograpicMode);
            hm_CameraOrthographicSize(CameraOrthographicSize);
            hm_CameraPerspectiveFov(CameraFov);
            hm_SceneCam_CheckStack(SceneCamera);
            hm_BluePrint_Update();

            if (Input.GetKeyDown(KeyCode.H))
            {
                hm_BluePrint_In();
            }
            if (Input.GetKeyDown(KeyCode.J))
            {
                hm_BluePrint_Out();
            }
        }

        private void OnDisable()
        {
            hm_ElementLibrary_Clean();
        }

        private void OnDrawGizmos()
        {
            switch (CompGuideMode)
            {
                case "水平对称":
                    CompGuide_LeftRightMirror();
                    break;
                case "垂直对称":
                    CompGuide_UpDownMirror();
                    break;
                case "黄金螺旋":
                    CompGuide_GoldenSpiral();
                    break;
                case "对角线":
                    CompGuide_DiagonalLine();
                    break;
                case "三分线":
                    CompGuide_ThreeCut();
                    break;
                case "引导线":
                    CompGuide_GuideLineRef();
                    break;
                case "三角":
                    CompGuide_Triangle();
                    break;
                case "工字型":
                    CompGuide_IShape();
                    break;
            }
        }

        #region 辅助构图参考

        /// <summary>
        /// 左右对称
        /// </summary>
        private void CompGuide_LeftRightMirror()
        {
            if (!UseCompGuide)
                return;


            RectTransform rect_up = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.上);
            RectTransform rect_leftup = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.左上);
            RectTransform rect_down = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.下);
            RectTransform rect_leftdown = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.左下);

            Vector3 start = Vector3.zero;
            Vector3 end = Vector3.zero;

            if (GuideParam_Mirror_LR_GoldenMode == "自定义对称分割")
            {
                start = rect_up.TransformPoint(rect_up.anchoredPosition3D + Vector3.right * GuideParam_Mirror_LR_Offset * ScreenRes.x / 2);
                end = rect_down.TransformPoint(rect_down.anchoredPosition3D + Vector3.right * GuideParam_Mirror_LR_Offset * ScreenRes.x / 2);
            }
            else if (GuideParam_Mirror_LR_GoldenMode == "靠右黄金比例")
            {
                start = rect_leftup.TransformPoint(rect_leftup.anchoredPosition3D + Vector3.right * ScreenRes.x / 1.618f);
                end = rect_leftdown.TransformPoint(rect_leftdown.anchoredPosition3D + Vector3.right * ScreenRes.x / 1.618f);
            }
            else if (GuideParam_Mirror_LR_GoldenMode == "靠左黄金比例")
            {
                float glod = ScreenRes.x / 1.618f;
                start = rect_leftup.TransformPoint(rect_leftup.anchoredPosition3D + Vector3.right * (ScreenRes.x - glod));
                end = rect_leftdown.TransformPoint(rect_leftdown.anchoredPosition3D + Vector3.right * (ScreenRes.x - glod));
            }

            Gizmos.color = GuidePointColor;
            Vector3 center = CalculateMidpoint(start, end);
            Gizmos.DrawSphere(center, GuideParam_CenterPointSize * 0.005f);

            Gizmos.color = GuideColor;
            Gizmos.DrawLine(start, end);
            Gizmos.color = Color.white;
        }

        /// <summary>
        /// 上下对称
        /// </summary>
        private void CompGuide_UpDownMirror()
        {
            if (!UseCompGuide)
                return;


            RectTransform rect_left = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.左);
            RectTransform rect_leftup = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.左上);
            RectTransform rect_right = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.右);
            RectTransform rect_rightup = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.右上);

            Vector3 start = Vector3.zero;
            Vector3 end = Vector3.zero;

            if (GuideParam_Mirror_UD_GoldenMode == "自定义对称分割")
            {
                start = rect_left.TransformPoint(rect_left.anchoredPosition3D + Vector3.up * GuideParam_Mirror_UD_Offset * ScreenRes.y / 2);
                end = rect_right.TransformPoint(rect_right.anchoredPosition3D + Vector3.up * GuideParam_Mirror_UD_Offset * ScreenRes.y / 2);
            }
            else if (GuideParam_Mirror_UD_GoldenMode == "靠上黄金比例")
            {
                float glod = ScreenRes.y / 1.618f;
                start = rect_leftup.TransformPoint(rect_leftup.anchoredPosition3D - Vector3.up * (ScreenRes.y - glod));
                end = rect_rightup.TransformPoint(rect_rightup.anchoredPosition3D - Vector3.up * (ScreenRes.y - glod));
            }
            else if (GuideParam_Mirror_UD_GoldenMode == "靠下黄金比例")
            {
                start = rect_leftup.TransformPoint(rect_leftup.anchoredPosition3D - Vector3.up * ScreenRes.y / 1.618f);
                end = rect_rightup.TransformPoint(rect_rightup.anchoredPosition3D - Vector3.up * ScreenRes.y / 1.618f);
            }

            Gizmos.color = GuidePointColor;
            Vector3 center = CalculateMidpoint(start, end);
            Gizmos.DrawSphere(center, GuideParam_CenterPointSize * 0.005f);

            Gizmos.color = GuideColor;
            Gizmos.DrawLine(start, end);
            Gizmos.color = Color.white;
        }

        /// <summary>
        /// 黄金螺旋
        /// </summary>
        private void CompGuide_GoldenSpiral()
        {
            if (!UseCompGuide)
                return;

            float scr_w = ScreenRes.x;
            float scr_h = ScreenRes.y;

            float gold_w = scr_w / 1.618f;
            float gold_h = scr_h / 1.618f;

            if (GuideParam_Fibonacci_Mode == "右上")
            {
                RectTransform rect_leftup = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.左上);
                Vector3 start_pos = rect_leftup.anchoredPosition3D;

                #region 第0分割
                Vector3 pos_s_v0 = start_pos + (Vector3.right * gold_w);
                Vector3 start_v0 = rect_leftup.TransformPoint(pos_s_v0);

                Vector3 pos_e_v0 = pos_s_v0 + start_pos - (Vector3.up * scr_h);
                Vector3 end_v0 = rect_leftup.TransformPoint(pos_e_v0);

                Gizmos.color = GuideColor;
                Gizmos.DrawLine(start_v0, end_v0);

                Vector3 curve_calc_v0_p1 = start_pos - (Vector3.up * scr_h / 2);
                Vector3 curve_v0_p1 = rect_leftup.TransformPoint(curve_calc_v0_p1);

                Vector3 curve_calc_v0_p2 = pos_e_v0 - (Vector3.right * gold_w / 2);
                Vector3 curve_v0_p2 = rect_leftup.TransformPoint(curve_calc_v0_p2);

                DrawBezierCurve(rect_leftup.position, curve_v0_p1, curve_v0_p2, end_v0);
                #endregion

                #region 第1分割
                float dis_v1_h = scr_h - gold_h;
                float dis_v1_w = scr_w - gold_w;

                Vector3 pos_s_v1 = pos_s_v0 - (start_pos + Vector3.up * dis_v1_h);
                Vector3 start_v1 = rect_leftup.TransformPoint(pos_s_v1);

                Vector3 pos_e_v1 = pos_s_v1 + (start_pos + (Vector3.right * dis_v1_w));
                Vector3 end_v1 = rect_leftup.TransformPoint(pos_e_v1);

                Gizmos.DrawLine(start_v1, end_v1);

                Vector3 curve_calc_v1_p1 = pos_s_v1 - (Vector3.up * (gold_h)) + (Vector3.right * dis_v1_w / 2);
                Vector3 curve_v1_p1 = rect_leftup.TransformPoint(curve_calc_v1_p1);

                Vector3 curve_calc_v1_p2 = pos_s_v1 - (Vector3.up * (gold_h / 2)) + (Vector3.right * dis_v1_w);
                Vector3 curve_v1_p2 = rect_leftup.TransformPoint(curve_calc_v1_p2);

                DrawBezierCurve(end_v0, curve_v1_p1, curve_v1_p2, end_v1);
                #endregion

                #region 第2分割
                float dis_v2_h = dis_v1_h;
                float dis_v2_w = dis_v1_w - (dis_v1_w / 1.618f);

                Vector3 pos_s_v2 = pos_s_v0 + (start_pos + Vector3.right * dis_v2_w);
                Vector3 start_v2 = rect_leftup.TransformPoint(pos_s_v2);

                Vector3 pos_e_v2 = pos_s_v2 - (start_pos + Vector3.up * dis_v2_h);
                Vector3 end_v2 = rect_leftup.TransformPoint(pos_e_v2);

                Gizmos.DrawLine(start_v2, end_v2);

                Vector3 curve_calc_v2_p1 = pos_s_v2 - (Vector3.up * dis_v2_h / 2) + (Vector3.right * (dis_v1_w / 1.618f));
                Vector3 curve_v2_p1 = rect_leftup.TransformPoint(curve_calc_v2_p1);

                Vector3 curve_calc_v2_p2 = pos_s_v2 + (Vector3.right * (dis_v1_w / 1.618f) / 2);
                Vector3 curve_v2_p2 = rect_leftup.TransformPoint(curve_calc_v2_p2);

                DrawBezierCurve(end_v1, curve_v2_p1, curve_v2_p2, start_v2);
                #endregion

                #region 第3分割
                float dis_v3_h = (dis_v2_h / 1.618f);
                float dis_v3_w = dis_v2_w;

                Vector3 pos_s_v3 = pos_s_v0 - (start_pos + Vector3.up * dis_v3_h);
                Vector3 start_v3 = rect_leftup.TransformPoint(pos_s_v3);

                Vector3 pos_e_v3 = pos_s_v3 + (start_pos + Vector3.right * dis_v3_w);
                Vector3 end_v3 = rect_leftup.TransformPoint(pos_e_v3);

                Gizmos.DrawLine(start_v3, end_v3);

                Vector3 curve_calc_v3_p1 = pos_s_v2 - (Vector3.right * (dis_v3_w / 2));
                Vector3 curve_v3_p1 = rect_leftup.TransformPoint(curve_calc_v3_p1);

                Vector3 curve_calc_v3_p2 = pos_e_v3 - (Vector3.right * dis_v3_w) + (Vector3.up * (dis_v3_h / 2));
                Vector3 curve_v3_p2 = rect_leftup.TransformPoint(curve_calc_v3_p2);

                DrawBezierCurve(start_v2, curve_v3_p1, curve_v3_p2, start_v3);
                #endregion

                #region 第4分割
                float dis_v4_h = (dis_v3_h / 1.618f);
                float dis_v4_w = (dis_v3_w / 1.618f);

                Vector3 pos_s_v4 = pos_s_v3 + (start_pos + Vector3.right * dis_v4_w);
                Vector3 start_v4 = rect_leftup.TransformPoint(pos_s_v4);

                Vector3 pos_e_v4 = pos_s_v4 - (start_pos + Vector3.up * dis_v4_h);
                Vector3 end_v4 = rect_leftup.TransformPoint(pos_e_v4);

                Gizmos.DrawLine(start_v4, end_v4);

                Vector3 curve_calc_v4_p1 = pos_s_v3 - (Vector3.up * (dis_v4_h / 2));
                Vector3 curve_v4_p1 = rect_leftup.TransformPoint(curve_calc_v4_p1);

                Vector3 curve_calc_v4_p2 = pos_e_v4 - (Vector3.right * dis_v4_w / 2);
                Vector3 curve_v4_p2 = rect_leftup.TransformPoint(curve_calc_v4_p2);

                DrawBezierCurve(start_v3, curve_v4_p1, curve_v4_p2, end_v4);
                #endregion

                #region 第5分割
                float dis_v5_h = dis_v4_h - (dis_v4_h / 1.618f);
                float dis_v5_w = dis_v4_w / 1.618f;

                Vector3 pos_s_v5 = pos_s_v4 - (start_pos + Vector3.up * dis_v5_h);
                Vector3 start_v5 = rect_leftup.TransformPoint(pos_s_v5);

                Vector3 pos_e_v5 = pos_s_v5 + (start_pos + Vector3.right * dis_v5_w);
                Vector3 end_v5 = rect_leftup.TransformPoint(pos_e_v5);

                Gizmos.DrawLine(start_v5, end_v5);

                Vector3 curve_calc_v5_p1 = pos_e_v4 + (Vector3.right * dis_v5_w / 2);
                Vector3 curve_v5_p1 = rect_leftup.TransformPoint(curve_calc_v5_p1);

                Vector3 curve_calc_v5_p2 = pos_e_v5 - (Vector3.up * ((dis_v4_h / 1.618f) / 2));
                Vector3 curve_v5_p2 = rect_leftup.TransformPoint(curve_calc_v5_p2);

                DrawBezierCurve(end_v4, curve_v5_p1, curve_v5_p2, end_v5);
                #endregion

                #region 第6分割
                float dis_v6_h = dis_v5_h;
                float dis_v6_w = dis_v5_w - (dis_v5_w / 1.618f);

                Vector3 pos_s_v6 = pos_s_v5 + (start_pos + Vector3.right * dis_v6_w);
                Vector3 start_v6 = rect_leftup.TransformPoint(pos_s_v6);

                Vector3 pos_e_v6 = pos_s_v6 + (start_pos + Vector3.up * dis_v6_h);
                Vector3 end_v6 = rect_leftup.TransformPoint(pos_e_v6);

                Gizmos.DrawLine(start_v6, end_v6);

                Vector3 curve_calc_v6_p1 = pos_e_v5 + (Vector3.up * (dis_v6_h / 2));
                Vector3 curve_v6_p1 = rect_leftup.TransformPoint(curve_calc_v6_p1);

                Vector3 curve_calc_v6_p2 = pos_e_v6 + (Vector3.right * ((dis_v5_w / 1.618f) / 2));
                Vector3 curve_v6_p2 = rect_leftup.TransformPoint(curve_calc_v6_p2);

                DrawBezierCurve(end_v5, curve_v6_p1, curve_v6_p2, end_v6);
                #endregion

                #region 第7分割
                float dis_v7_h = dis_v6_h - (dis_v6_h / 1.618f);
                float dis_v7_w = dis_v6_w;

                Vector3 pos_s_v7 = pos_s_v5 + (start_pos + Vector3.up * dis_v7_h);
                Vector3 start_v7 = rect_leftup.TransformPoint(pos_s_v7);

                Vector3 pos_e_v7 = pos_s_v7 + (start_pos + Vector3.right * dis_v7_w);
                Vector3 end_v7 = rect_leftup.TransformPoint(pos_e_v7);

                Gizmos.DrawLine(start_v7, end_v7);

                Vector3 curve_calc_v7_p1 = pos_e_v6 - (Vector3.right * (dis_v7_w / 2));
                Vector3 curve_v7_p1 = rect_leftup.TransformPoint(curve_calc_v7_p1);

                Vector3 curve_calc_v7_p2 = pos_s_v7 + (Vector3.up * ((dis_v6_h / 1.618f) / 2));
                Vector3 curve_v7_p2 = rect_leftup.TransformPoint(curve_calc_v7_p2);

                DrawBezierCurve(end_v6, curve_v7_p1, curve_v7_p2, start_v7);
                #endregion

                #region 第8分割
                float dis_v8_h = dis_v7_h;
                float dis_v8_w = (dis_v7_w / 1.618f);

                Vector3 pos_s_v8 = pos_s_v7 + (start_pos + Vector3.right * dis_v8_w);
                Vector3 start_v8 = rect_leftup.TransformPoint(pos_s_v8);

                Vector3 pos_e_v8 = pos_s_v8 - (start_pos + Vector3.up * dis_v8_h);
                Vector3 end_v8 = rect_leftup.TransformPoint(pos_e_v8);

                Gizmos.DrawLine(start_v8, end_v8);

                Vector3 curve_calc_v8_p1 = pos_s_v7 - (Vector3.up * (dis_v8_h / 2));
                Vector3 curve_v8_p1 = rect_leftup.TransformPoint(curve_calc_v8_p1);

                Vector3 curve_calc_v8_p2 = pos_e_v8 - (Vector3.right * (dis_v8_w / 2));
                Vector3 curve_v8_p2 = rect_leftup.TransformPoint(curve_calc_v8_p2);

                DrawBezierCurve(start_v7, curve_v8_p1, curve_v8_p2, end_v8);
                #endregion

                #region 第9分割
                float dis_v9_h = dis_v8_h - (dis_v8_h / 1.618f);
                float dis_v9_w = dis_v8_w / 1.618f;

                Vector3 pos_s_v9 = pos_s_v8 - (start_pos + Vector3.up * dis_v9_h);
                Vector3 start_v9 = rect_leftup.TransformPoint(pos_s_v9);

                Vector3 pos_e_v9 = pos_s_v9 + (start_pos + Vector3.right * dis_v9_w);
                Vector3 end_v9 = rect_leftup.TransformPoint(pos_e_v9);

                Gizmos.DrawLine(start_v9, end_v9);

                Vector3 curve_calc_v9_p1 = pos_e_v8 + (Vector3.right * (dis_v9_w / 2));
                Vector3 curve_v9_p1 = rect_leftup.TransformPoint(curve_calc_v9_p1);

                Vector3 curve_calc_v9_p2 = pos_e_v9 - (Vector3.up * (dis_v9_h / 2));
                Vector3 curve_v9_p2 = rect_leftup.TransformPoint(curve_calc_v9_p2);

                DrawBezierCurve(end_v8, curve_v9_p1, curve_v9_p2, end_v9);
                #endregion
            }
            else if (GuideParam_Fibonacci_Mode == "右下")
            {
                RectTransform rect_leftdown = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.左下);
                Vector3 start_pos = rect_leftdown.anchoredPosition3D;

                #region 第0分割
                Vector3 pos_s_v0 = start_pos + (Vector3.right * gold_w);
                Vector3 start_v0 = rect_leftdown.TransformPoint(pos_s_v0);

                Vector3 pos_e_v0 = pos_s_v0 + start_pos + (Vector3.up * scr_h);
                Vector3 end_v0 = rect_leftdown.TransformPoint(pos_e_v0);

                Gizmos.color = GuideColor;
                Gizmos.DrawLine(start_v0, end_v0);

                Vector3 curve_calc_v0_p1 = start_pos + (Vector3.up * scr_h / 2);
                Vector3 curve_v0_p1 = rect_leftdown.TransformPoint(curve_calc_v0_p1);

                Vector3 curve_calc_v0_p2 = pos_e_v0 - (Vector3.right * gold_w / 2);
                Vector3 curve_v0_p2 = rect_leftdown.TransformPoint(curve_calc_v0_p2);

                DrawBezierCurve(rect_leftdown.position, curve_v0_p1, curve_v0_p2, end_v0);
                #endregion

                #region 第1分割
                float dis_v1_h = (scr_h - gold_h);
                float dis_v1_w = scr_w - gold_w;

                Vector3 pos_s_v1 = pos_s_v0 + (start_pos + Vector3.up * dis_v1_h);
                Vector3 start_v1 = rect_leftdown.TransformPoint(pos_s_v1);

                Vector3 pos_e_v1 = pos_s_v1 + (start_pos + (Vector3.right * dis_v1_w));
                Vector3 end_v1 = rect_leftdown.TransformPoint(pos_e_v1);

                Gizmos.DrawLine(start_v1, end_v1);

                Vector3 curve_calc_v1_p1 = pos_e_v0 + (Vector3.right * (dis_v1_w / 2));
                Vector3 curve_v1_p1 = rect_leftdown.TransformPoint(curve_calc_v1_p1);

                Vector3 curve_calc_v1_p2 = pos_e_v1 + (Vector3.up * (gold_h / 2));
                Vector3 curve_v1_p2 = rect_leftdown.TransformPoint(curve_calc_v1_p2);

                DrawBezierCurve(end_v0, curve_v1_p1, curve_v1_p2, end_v1);
                #endregion

                #region 第2分割
                float dis_v2_h = dis_v1_h;
                float dis_v2_w = dis_v1_w - (dis_v1_w / 1.618f);

                Vector3 pos_s_v2 = pos_s_v0 + (start_pos + Vector3.right * dis_v2_w);
                Vector3 start_v2 = rect_leftdown.TransformPoint(pos_s_v2);

                Vector3 pos_e_v2 = pos_s_v2 + (start_pos + Vector3.up * dis_v2_h);
                Vector3 end_v2 = rect_leftdown.TransformPoint(pos_e_v2);

                Gizmos.DrawLine(start_v2, end_v2);

                Vector3 curve_calc_v2_p1 = pos_e_v1 - (Vector3.up * (dis_v2_h / 2));
                Vector3 curve_v2_p1 = rect_leftdown.TransformPoint(curve_calc_v2_p1);

                Vector3 curve_calc_v2_p2 = pos_s_v2 + (Vector3.right * ((dis_v1_w / 1.618f) / 2));
                Vector3 curve_v2_p2 = rect_leftdown.TransformPoint(curve_calc_v2_p2);

                DrawBezierCurve(end_v1, curve_v2_p1, curve_v2_p2, start_v2);
                #endregion

                #region 第3分割
                float dis_v3_h = (dis_v2_h / 1.618f);
                float dis_v3_w = dis_v2_w;

                Vector3 pos_s_v3 = pos_s_v0 + (start_pos + Vector3.up * dis_v3_h);
                Vector3 start_v3 = rect_leftdown.TransformPoint(pos_s_v3);

                Vector3 pos_e_v3 = pos_s_v3 + (start_pos + Vector3.right * dis_v3_w);
                Vector3 end_v3 = rect_leftdown.TransformPoint(pos_e_v3);

                Gizmos.DrawLine(start_v3, end_v3);

                Vector3 curve_calc_v3_p1 = pos_s_v2 - (Vector3.right * (dis_v3_w / 2));
                Vector3 curve_v3_p1 = rect_leftdown.TransformPoint(curve_calc_v3_p1);

                Vector3 curve_calc_v3_p2 = pos_s_v3 - (Vector3.up * (dis_v3_h / 2));
                Vector3 curve_v3_p2 = rect_leftdown.TransformPoint(curve_calc_v3_p2);

                DrawBezierCurve(start_v2, curve_v3_p1, curve_v3_p2, start_v3);
                #endregion

                #region 第4分割
                float dis_v4_h = (dis_v3_h / 1.618f);
                float dis_v4_w = (dis_v3_w / 1.618f);

                Vector3 pos_s_v4 = pos_s_v3 + (start_pos + Vector3.right * dis_v4_w);
                Vector3 start_v4 = rect_leftdown.TransformPoint(pos_s_v4);

                Vector3 pos_e_v4 = pos_s_v4 + (start_pos + Vector3.up * dis_v4_h);
                Vector3 end_v4 = rect_leftdown.TransformPoint(pos_e_v4);

                Gizmos.DrawLine(start_v4, end_v4);

                Vector3 curve_calc_v4_p1 = pos_s_v3 + (Vector3.up * (dis_v4_h / 2));
                Vector3 curve_v4_p1 = rect_leftdown.TransformPoint(curve_calc_v4_p1);

                Vector3 curve_calc_v4_p2 = pos_e_v4 - (Vector3.right * (dis_v4_w / 2));
                Vector3 curve_v4_p2 = rect_leftdown.TransformPoint(curve_calc_v4_p2);

                DrawBezierCurve(start_v3, curve_v4_p1, curve_v4_p2, end_v4);
                #endregion

                #region 第5分割
                float dis_v5_h = dis_v4_h - (dis_v4_h / 1.618f);
                float dis_v5_w = dis_v4_w / 1.618f;

                Vector3 pos_s_v5 = pos_s_v4 + (start_pos + Vector3.up * dis_v5_h);
                Vector3 start_v5 = rect_leftdown.TransformPoint(pos_s_v5);

                Vector3 pos_e_v5 = pos_s_v5 + (start_pos + Vector3.right * dis_v5_w);
                Vector3 end_v5 = rect_leftdown.TransformPoint(pos_e_v5);

                Gizmos.DrawLine(start_v5, end_v5);

                Vector3 curve_calc_v5_p1 = pos_e_v4 + (Vector3.right * (dis_v5_w / 2));
                Vector3 curve_v5_p1 = rect_leftdown.TransformPoint(curve_calc_v5_p1);

                Vector3 curve_calc_v5_p2 = pos_e_v5 + (Vector3.up * ((dis_v4_h / 1.618f) / 2));
                Vector3 curve_v5_p2 = rect_leftdown.TransformPoint(curve_calc_v5_p2);

                DrawBezierCurve(end_v4, curve_v5_p1, curve_v5_p2, end_v5);
                #endregion

                #region 第6分割
                float dis_v6_h = dis_v5_h;
                float dis_v6_w = dis_v5_w - (dis_v5_w / 1.618f);

                Vector3 pos_s_v6 = pos_s_v5 + (start_pos + Vector3.right * dis_v6_w);
                Vector3 start_v6 = rect_leftdown.TransformPoint(pos_s_v6);

                Vector3 pos_e_v6 = pos_s_v6 - (start_pos + Vector3.up * dis_v6_h);
                Vector3 end_v6 = rect_leftdown.TransformPoint(pos_e_v6);

                Gizmos.DrawLine(start_v6, end_v6);

                Vector3 curve_calc_v6_p1 = pos_e_v5 - (Vector3.up * (dis_v6_h / 2));
                Vector3 curve_v6_p1 = rect_leftdown.TransformPoint(curve_calc_v6_p1);

                Vector3 curve_calc_v6_p2 = pos_e_v6 + (Vector3.right * (dis_v5_w / 1.618f / 2));
                Vector3 curve_v6_p2 = rect_leftdown.TransformPoint(curve_calc_v6_p2);

                DrawBezierCurve(end_v5, curve_v6_p1, curve_v6_p2, end_v6);
                #endregion

                #region 第7分割
                float dis_v7_h = dis_v6_h - (dis_v6_h / 1.618f);
                float dis_v7_w = dis_v6_w;

                Vector3 pos_s_v7 = pos_s_v5 - (start_pos + Vector3.up * dis_v7_h);
                Vector3 start_v7 = rect_leftdown.TransformPoint(pos_s_v7);

                Vector3 pos_e_v7 = pos_s_v7 + (start_pos + Vector3.right * dis_v7_w);
                Vector3 end_v7 = rect_leftdown.TransformPoint(pos_e_v7);

                Gizmos.DrawLine(start_v7, end_v7);

                Vector3 curve_calc_v7_p1 = pos_e_v6 - (Vector3.right * (dis_v7_w / 2));
                Vector3 curve_v7_p1 = rect_leftdown.TransformPoint(curve_calc_v7_p1);

                Vector3 curve_calc_v7_p2 = pos_s_v7 - (Vector3.up * ((dis_v6_h / 1.618f) / 2));
                Vector3 curve_v7_p2 = rect_leftdown.TransformPoint(curve_calc_v7_p2);

                DrawBezierCurve(end_v6, curve_v7_p1, curve_v7_p2, start_v7);
                #endregion

                #region 第8分割
                float dis_v8_h = dis_v7_h;
                float dis_v8_w = (dis_v7_w / 1.618f);

                Vector3 pos_s_v8 = pos_s_v7 + (start_pos + Vector3.right * dis_v8_w);
                Vector3 start_v8 = rect_leftdown.TransformPoint(pos_s_v8);

                Vector3 pos_e_v8 = pos_s_v8 + (start_pos + Vector3.up * dis_v8_h);
                Vector3 end_v8 = rect_leftdown.TransformPoint(pos_e_v8);

                Gizmos.DrawLine(start_v8, end_v8);

                Vector3 curve_calc_v8_p1 = pos_s_v7 + (Vector3.up * (dis_v8_h / 2));
                Vector3 curve_v8_p1 = rect_leftdown.TransformPoint(curve_calc_v8_p1);

                Vector3 curve_calc_v8_p2 = pos_e_v8 - (Vector3.right * (dis_v8_w / 2));
                Vector3 curve_v8_p2 = rect_leftdown.TransformPoint(curve_calc_v8_p2);

                DrawBezierCurve(start_v7, curve_v8_p1, curve_v8_p2, end_v8);
                #endregion

                #region 第9分割
                float dis_v9_h = dis_v8_h - (dis_v8_h / 1.618f);
                float dis_v9_w = dis_v8_w / 1.618f;

                Vector3 pos_s_v9 = pos_s_v8 + (start_pos + Vector3.up * dis_v9_h);
                Vector3 start_v9 = rect_leftdown.TransformPoint(pos_s_v9);

                Vector3 pos_e_v9 = pos_s_v9 + (start_pos + Vector3.right * dis_v9_w);
                Vector3 end_v9 = rect_leftdown.TransformPoint(pos_e_v9);

                Gizmos.DrawLine(start_v9, end_v9);

                Vector3 curve_calc_v9_p1 = pos_e_v8 + (Vector3.right * (dis_v9_w / 2));
                Vector3 curve_v9_p1 = rect_leftdown.TransformPoint(curve_calc_v9_p1);

                Vector3 curve_calc_v9_p2 = pos_e_v9 + (Vector3.up * ((dis_v8_h / 1.618f) / 2));
                Vector3 curve_v9_p2 = rect_leftdown.TransformPoint(curve_calc_v9_p2);

                DrawBezierCurve(end_v8, curve_v9_p1, curve_v9_p2, end_v9);
                #endregion
            }
            else if (GuideParam_Fibonacci_Mode == "左下")
            {
                RectTransform rect_rightdown = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.右下);
                Vector3 start_pos = rect_rightdown.anchoredPosition3D;

                #region 第0分割
                Vector3 pos_s_v0 = start_pos - (Vector3.right * gold_w);
                Vector3 start_v0 = rect_rightdown.TransformPoint(pos_s_v0);

                Vector3 pos_e_v0 = pos_s_v0 + start_pos + (Vector3.up * scr_h);
                Vector3 end_v0 = rect_rightdown.TransformPoint(pos_e_v0);

                Gizmos.color = GuideColor;
                Gizmos.DrawLine(start_v0, end_v0);

                Vector3 curve_calc_v0_p1 = start_pos + (Vector3.up * scr_h / 2);
                Vector3 curve_v0_p1 = rect_rightdown.TransformPoint(curve_calc_v0_p1);

                Vector3 curve_calc_v0_p2 = pos_e_v0 + (Vector3.right * gold_w / 2);
                Vector3 curve_v0_p2 = rect_rightdown.TransformPoint(curve_calc_v0_p2);

                DrawBezierCurve(rect_rightdown.position, curve_v0_p1, curve_v0_p2, end_v0);
                #endregion

                #region 第1分割
                float dis_v1_h = (scr_h - gold_h);
                float dis_v1_w = scr_w - gold_w;

                Vector3 pos_s_v1 = pos_s_v0 + (start_pos + Vector3.up * dis_v1_h);
                Vector3 start_v1 = rect_rightdown.TransformPoint(pos_s_v1);

                Vector3 pos_e_v1 = pos_s_v1 - (start_pos + (Vector3.right * dis_v1_w));
                Vector3 end_v1 = rect_rightdown.TransformPoint(pos_e_v1);

                Gizmos.DrawLine(start_v1, end_v1);

                Vector3 curve_calc_v1_p1 = pos_e_v0 - (Vector3.right * (dis_v1_w / 2));
                Vector3 curve_v1_p1 = rect_rightdown.TransformPoint(curve_calc_v1_p1);

                Vector3 curve_calc_v1_p2 = pos_e_v1 + (Vector3.up * gold_h / 2);
                Vector3 curve_v1_p2 = rect_rightdown.TransformPoint(curve_calc_v1_p2);

                DrawBezierCurve(end_v0, curve_v1_p1, curve_v1_p2, end_v1);
                #endregion

                #region 第2分割
                float dis_v2_h = dis_v1_h;
                float dis_v2_w = dis_v1_w - (dis_v1_w / 1.618f);

                Vector3 pos_s_v2 = pos_s_v0 - (start_pos + Vector3.right * dis_v2_w);
                Vector3 start_v2 = rect_rightdown.TransformPoint(pos_s_v2);

                Vector3 pos_e_v2 = pos_s_v2 + (start_pos + Vector3.up * dis_v2_h);
                Vector3 end_v2 = rect_rightdown.TransformPoint(pos_e_v2);

                Gizmos.DrawLine(start_v2, end_v2);

                Vector3 curve_calc_v2_p1 = pos_e_v1 - (Vector3.up * (dis_v2_h / 2));
                Vector3 curve_v2_p1 = rect_rightdown.TransformPoint(curve_calc_v2_p1);

                Vector3 curve_calc_v2_p2 = pos_s_v2 - (Vector3.right * ((dis_v1_w / 1.618f) / 2));
                Vector3 curve_v2_p2 = rect_rightdown.TransformPoint(curve_calc_v2_p2);

                DrawBezierCurve(end_v1, curve_v2_p1, curve_v2_p2, start_v2);
                #endregion

                #region 第3分割
                float dis_v3_h = (dis_v2_h / 1.618f);
                float dis_v3_w = dis_v2_w;

                Vector3 pos_s_v3 = pos_s_v0 + (start_pos + Vector3.up * dis_v3_h);
                Vector3 start_v3 = rect_rightdown.TransformPoint(pos_s_v3);

                Vector3 pos_e_v3 = pos_s_v3 - (start_pos + Vector3.right * dis_v3_w);
                Vector3 end_v3 = rect_rightdown.TransformPoint(pos_e_v3);

                Gizmos.DrawLine(start_v3, end_v3);

                Vector3 curve_calc_v3_p1 = pos_s_v2 + (Vector3.right * (dis_v3_w / 2));
                Vector3 curve_v3_p1 = rect_rightdown.TransformPoint(curve_calc_v3_p1);

                Vector3 curve_calc_v3_p2 = pos_s_v3 - (Vector3.up * (dis_v3_h / 2));
                Vector3 curve_v3_p2 = rect_rightdown.TransformPoint(curve_calc_v3_p2);

                DrawBezierCurve(start_v2, curve_v3_p1, curve_v3_p2, start_v3);
                #endregion

                #region 第4分割
                float dis_v4_h = (dis_v3_h / 1.618f);
                float dis_v4_w = (dis_v3_w / 1.618f);

                Vector3 pos_s_v4 = pos_s_v3 - (start_pos + Vector3.right * dis_v4_w);
                Vector3 start_v4 = rect_rightdown.TransformPoint(pos_s_v4);

                Vector3 pos_e_v4 = pos_s_v4 + (start_pos + Vector3.up * dis_v4_h);
                Vector3 end_v4 = rect_rightdown.TransformPoint(pos_e_v4);

                Gizmos.DrawLine(start_v4, end_v4);

                Vector3 curve_calc_v4_p1 = pos_s_v3 + (Vector3.up * (dis_v4_h / 2));
                Vector3 curve_v4_p1 = rect_rightdown.TransformPoint(curve_calc_v4_p1);

                Vector3 curve_calc_v4_p2 = pos_e_v4 + (Vector3.right * (dis_v4_w / 2));
                Vector3 curve_v4_p2 = rect_rightdown.TransformPoint(curve_calc_v4_p2);

                DrawBezierCurve(start_v3, curve_v4_p1, curve_v4_p2, end_v4);
                #endregion

                #region 第5分割
                float dis_v5_h = dis_v4_h - (dis_v4_h / 1.618f);
                float dis_v5_w = dis_v4_w / 1.618f;

                Vector3 pos_s_v5 = pos_s_v4 + (start_pos + Vector3.up * dis_v5_h);
                Vector3 start_v5 = rect_rightdown.TransformPoint(pos_s_v5);

                Vector3 pos_e_v5 = pos_s_v5 - (start_pos + Vector3.right * dis_v5_w);
                Vector3 end_v5 = rect_rightdown.TransformPoint(pos_e_v5);

                Gizmos.DrawLine(start_v5, end_v5);

                Vector3 curve_calc_v5_p1 = pos_e_v4 - (Vector3.right * (dis_v5_w / 2));
                Vector3 curve_v5_p1 = rect_rightdown.TransformPoint(curve_calc_v5_p1);

                Vector3 curve_calc_v5_p2 = pos_e_v5 + (Vector3.up * ((dis_v4_h / 1.618f) / 2));
                Vector3 curve_v5_p2 = rect_rightdown.TransformPoint(curve_calc_v5_p2);

                DrawBezierCurve(end_v4, curve_v5_p1, curve_v5_p2, end_v5);
                #endregion

                #region 第6分割
                float dis_v6_h = dis_v5_h;
                float dis_v6_w = dis_v5_w - (dis_v5_w / 1.618f);

                Vector3 pos_s_v6 = pos_s_v5 - (start_pos + Vector3.right * dis_v6_w);
                Vector3 start_v6 = rect_rightdown.TransformPoint(pos_s_v6);

                Vector3 pos_e_v6 = pos_s_v6 - (start_pos + Vector3.up * dis_v6_h);
                Vector3 end_v6 = rect_rightdown.TransformPoint(pos_e_v6);

                Gizmos.DrawLine(start_v6, end_v6);

                Vector3 curve_calc_v6_p1 = pos_e_v5 - (Vector3.up * (dis_v6_h / 2));
                Vector3 curve_v6_p1 = rect_rightdown.TransformPoint(curve_calc_v6_p1);

                Vector3 curve_calc_v6_p2 = pos_e_v6 - (Vector3.right * ((dis_v5_w / 1.618f) / 2));
                Vector3 curve_v6_p2 = rect_rightdown.TransformPoint(curve_calc_v6_p2);

                DrawBezierCurve(end_v5, curve_v6_p1, curve_v6_p2, end_v6);
                #endregion

                #region 第7分割
                float dis_v7_h = dis_v6_h - (dis_v6_h / 1.618f);
                float dis_v7_w = dis_v6_w;

                Vector3 pos_s_v7 = pos_s_v5 - (start_pos + Vector3.up * dis_v7_h);
                Vector3 start_v7 = rect_rightdown.TransformPoint(pos_s_v7);

                Vector3 pos_e_v7 = pos_s_v7 - (start_pos + Vector3.right * dis_v7_w);
                Vector3 end_v7 = rect_rightdown.TransformPoint(pos_e_v7);

                Gizmos.DrawLine(start_v7, end_v7);

                Vector3 curve_calc_v7_p1 = pos_e_v6 + (Vector3.right * (dis_v7_w / 2));
                Vector3 curve_v7_p1 = rect_rightdown.TransformPoint(curve_calc_v7_p1);

                Vector3 curve_calc_v7_p2 = pos_s_v7 - (Vector3.up * ((dis_v6_h / 1.618f) / 2));
                Vector3 curve_v7_p2 = rect_rightdown.TransformPoint(curve_calc_v7_p2);

                DrawBezierCurve(end_v6, curve_v7_p1, curve_v7_p2, start_v7);
                #endregion

                #region 第8分割
                float dis_v8_h = dis_v7_h;
                float dis_v8_w = (dis_v7_w / 1.618f);

                Vector3 pos_s_v8 = pos_s_v7 - (start_pos + Vector3.right * dis_v8_w);
                Vector3 start_v8 = rect_rightdown.TransformPoint(pos_s_v8);

                Vector3 pos_e_v8 = pos_s_v8 + (start_pos + Vector3.up * dis_v8_h);
                Vector3 end_v8 = rect_rightdown.TransformPoint(pos_e_v8);

                Gizmos.DrawLine(start_v8, end_v8);

                Vector3 curve_calc_v8_p1 = pos_s_v7 + (Vector3.up * (dis_v8_h / 2));
                Vector3 curve_v8_p1 = rect_rightdown.TransformPoint(curve_calc_v8_p1);

                Vector3 curve_calc_v8_p2 = pos_e_v8 + (Vector3.right * (dis_v8_w / 2));
                Vector3 curve_v8_p2 = rect_rightdown.TransformPoint(curve_calc_v8_p2);

                DrawBezierCurve(start_v7, curve_v8_p1, curve_v8_p2, end_v8);
                #endregion

                #region 第9分割
                float dis_v9_h = dis_v8_h - (dis_v8_h / 1.618f);
                float dis_v9_w = dis_v8_w / 1.618f;

                Vector3 pos_s_v9 = pos_s_v8 + (start_pos + Vector3.up * dis_v9_h);
                Vector3 start_v9 = rect_rightdown.TransformPoint(pos_s_v9);

                Vector3 pos_e_v9 = pos_s_v9 - (start_pos + Vector3.right * dis_v9_w);
                Vector3 end_v9 = rect_rightdown.TransformPoint(pos_e_v9);

                Gizmos.DrawLine(start_v9, end_v9);

                Vector3 curve_calc_v9_p1 = pos_e_v8 - (Vector3.right * (dis_v9_w / 2));
                Vector3 curve_v9_p1 = rect_rightdown.TransformPoint(curve_calc_v9_p1);

                Vector3 curve_calc_v9_p2 = pos_e_v9 + (Vector3.up * ((dis_v8_h / 1.618f) / 2));
                Vector3 curve_v9_p2 = rect_rightdown.TransformPoint(curve_calc_v9_p2);

                DrawBezierCurve(end_v8, curve_v9_p1, curve_v9_p2, end_v9);
                #endregion
            }
            else if (GuideParam_Fibonacci_Mode == "左上")
            {
                RectTransform rect_rightup = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.右上);
                Vector3 start_pos = rect_rightup.anchoredPosition3D;

                #region 第0分割
                Vector3 pos_s_v0 = start_pos - (Vector3.right * gold_w);
                Vector3 start_v0 = rect_rightup.TransformPoint(pos_s_v0);

                Vector3 pos_e_v0 = pos_s_v0 + start_pos - (Vector3.up * scr_h);
                Vector3 end_v0 = rect_rightup.TransformPoint(pos_e_v0);

                Gizmos.color = GuideColor;
                Gizmos.DrawLine(start_v0, end_v0);

                Vector3 curve_calc_v0_p1 = start_pos - (Vector3.up * scr_h / 2);
                Vector3 curve_v0_p1 = rect_rightup.TransformPoint(curve_calc_v0_p1);

                Vector3 curve_calc_v0_p2 = pos_e_v0 + (Vector3.right * gold_w / 2);
                Vector3 curve_v0_p2 = rect_rightup.TransformPoint(curve_calc_v0_p2);

                DrawBezierCurve(rect_rightup.position, curve_v0_p1, curve_v0_p2, end_v0);
                #endregion

                #region 第1分割
                float dis_v1_h = scr_h - gold_h;
                float dis_v1_w = scr_w - gold_w;

                Vector3 pos_s_v1 = pos_s_v0 - (start_pos + Vector3.up * dis_v1_h);
                Vector3 start_v1 = rect_rightup.TransformPoint(pos_s_v1);

                Vector3 pos_e_v1 = pos_s_v1 - (start_pos + (Vector3.right * dis_v1_w));
                Vector3 end_v1 = rect_rightup.TransformPoint(pos_e_v1);

                Gizmos.DrawLine(start_v1, end_v1);

                Vector3 curve_calc_v1_p1 = pos_e_v0 - (Vector3.right * (dis_v1_w / 2));
                Vector3 curve_v1_p1 = rect_rightup.TransformPoint(curve_calc_v1_p1);

                Vector3 curve_calc_v1_p2 = pos_e_v1 - (Vector3.up * gold_h / 2);
                Vector3 curve_v1_p2 = rect_rightup.TransformPoint(curve_calc_v1_p2);

                DrawBezierCurve(end_v0, curve_v1_p1, curve_v1_p2, end_v1);
                #endregion

                #region 第2分割
                float dis_v2_h = dis_v1_h;
                float dis_v2_w = dis_v1_w - (dis_v1_w / 1.618f);

                Vector3 pos_s_v2 = pos_s_v0 - (start_pos + Vector3.right * dis_v2_w);
                Vector3 start_v2 = rect_rightup.TransformPoint(pos_s_v2);

                Vector3 pos_e_v2 = pos_s_v2 - (start_pos + Vector3.up * dis_v2_h);
                Vector3 end_v2 = rect_rightup.TransformPoint(pos_e_v2);

                Gizmos.DrawLine(start_v2, end_v2);

                Vector3 curve_calc_v2_p1 = pos_e_v1 + (Vector3.up * (dis_v2_h / 2));
                Vector3 curve_v2_p1 = rect_rightup.TransformPoint(curve_calc_v2_p1);

                Vector3 curve_calc_v2_p2 = pos_s_v2 - (Vector3.right * ((dis_v1_w / 1.618f) / 2));
                Vector3 curve_v2_p2 = rect_rightup.TransformPoint(curve_calc_v2_p2);

                DrawBezierCurve(end_v1, curve_v2_p1, curve_v2_p2, start_v2);
                #endregion

                #region 第3分割
                float dis_v3_h = (dis_v2_h / 1.618f);
                float dis_v3_w = dis_v2_w;

                Vector3 pos_s_v3 = pos_s_v0 - (start_pos + Vector3.up * dis_v3_h);
                Vector3 start_v3 = rect_rightup.TransformPoint(pos_s_v3);

                Vector3 pos_e_v3 = pos_s_v3 - (start_pos + Vector3.right * dis_v3_w);
                Vector3 end_v3 = rect_rightup.TransformPoint(pos_e_v3);

                Gizmos.DrawLine(start_v3, end_v3);

                Vector3 curve_calc_v3_p1 = pos_s_v2 + (Vector3.right * (dis_v3_w / 2));
                Vector3 curve_v3_p1 = rect_rightup.TransformPoint(curve_calc_v3_p1);

                Vector3 curve_calc_v3_p2 = pos_s_v3 + (Vector3.up * (dis_v3_h / 2));
                Vector3 curve_v3_p2 = rect_rightup.TransformPoint(curve_calc_v3_p2);

                DrawBezierCurve(start_v2, curve_v3_p1, curve_v3_p2, start_v3);
                #endregion

                #region 第4分割
                float dis_v4_h = (dis_v3_h / 1.618f);
                float dis_v4_w = (dis_v3_w / 1.618f);

                Vector3 pos_s_v4 = pos_s_v3 - (start_pos + Vector3.right * dis_v4_w);
                Vector3 start_v4 = rect_rightup.TransformPoint(pos_s_v4);

                Vector3 pos_e_v4 = pos_s_v4 - (start_pos + Vector3.up * dis_v4_h);
                Vector3 end_v4 = rect_rightup.TransformPoint(pos_e_v4);

                Gizmos.DrawLine(start_v4, end_v4);

                Vector3 curve_calc_v4_p1 = pos_s_v3 - (Vector3.up * (dis_v4_h / 2));
                Vector3 curve_v4_p1 = rect_rightup.TransformPoint(curve_calc_v4_p1);

                Vector3 curve_calc_v4_p2 = pos_e_v4 + (Vector3.right * (dis_v4_w / 2));
                Vector3 curve_v4_p2 = rect_rightup.TransformPoint(curve_calc_v4_p2);

                DrawBezierCurve(start_v3, curve_v4_p1, curve_v4_p2, end_v4);
                #endregion

                #region 第5分割
                float dis_v5_h = dis_v4_h - (dis_v4_h / 1.618f);
                float dis_v5_w = dis_v4_w / 1.618f;

                Vector3 pos_s_v5 = pos_s_v4 - (start_pos + Vector3.up * dis_v5_h);
                Vector3 start_v5 = rect_rightup.TransformPoint(pos_s_v5);

                Vector3 pos_e_v5 = pos_s_v5 - (start_pos + Vector3.right * dis_v5_w);
                Vector3 end_v5 = rect_rightup.TransformPoint(pos_e_v5);

                Gizmos.DrawLine(start_v5, end_v5);

                Vector3 curve_calc_v5_p1 = pos_e_v4 - (Vector3.right * (dis_v5_w / 2));
                Vector3 curve_v5_p1 = rect_rightup.TransformPoint(curve_calc_v5_p1);

                Vector3 curve_calc_v5_p2 = pos_e_v5 - (Vector3.up * ((dis_v4_h / 1.618f) / 2));
                Vector3 curve_v5_p2 = rect_rightup.TransformPoint(curve_calc_v5_p2);

                DrawBezierCurve(end_v4, curve_v5_p1, curve_v5_p2, end_v5);
                #endregion

                #region 第6分割
                float dis_v6_h = dis_v5_h;
                float dis_v6_w = dis_v5_w - (dis_v5_w / 1.618f);

                Vector3 pos_s_v6 = pos_s_v5 - (start_pos + Vector3.right * dis_v6_w);
                Vector3 start_v6 = rect_rightup.TransformPoint(pos_s_v6);

                Vector3 pos_e_v6 = pos_s_v6 + (start_pos + Vector3.up * dis_v6_h);
                Vector3 end_v6 = rect_rightup.TransformPoint(pos_e_v6);

                Gizmos.DrawLine(start_v6, end_v6);

                Vector3 curve_calc_v6_p1 = pos_e_v5 + (Vector3.up * (dis_v6_h / 2));
                Vector3 curve_v6_p1 = rect_rightup.TransformPoint(curve_calc_v6_p1);

                Vector3 curve_calc_v6_p2 = pos_e_v6 - (Vector3.right * ((dis_v5_w / 1.618f) / 2));
                Vector3 curve_v6_p2 = rect_rightup.TransformPoint(curve_calc_v6_p2);

                DrawBezierCurve(end_v5, curve_v6_p1, curve_v6_p2, end_v6);
                #endregion

                #region 第7分割
                float dis_v7_h = dis_v6_h - (dis_v6_h / 1.618f);
                float dis_v7_w = dis_v6_w;

                Vector3 pos_s_v7 = pos_s_v5 + (start_pos + Vector3.up * dis_v7_h);
                Vector3 start_v7 = rect_rightup.TransformPoint(pos_s_v7);

                Vector3 pos_e_v7 = pos_s_v7 - (start_pos + Vector3.right * dis_v7_w);
                Vector3 end_v7 = rect_rightup.TransformPoint(pos_e_v7);

                Gizmos.DrawLine(start_v7, end_v7);

                Vector3 curve_calc_v7_p1 = pos_e_v6 + (Vector3.right * (dis_v7_w / 2));
                Vector3 curve_v7_p1 = rect_rightup.TransformPoint(curve_calc_v7_p1);

                Vector3 curve_calc_v7_p2 = pos_s_v7 + (Vector3.up * ((dis_v6_h / 1.618f) / 2));
                Vector3 curve_v7_p2 = rect_rightup.TransformPoint(curve_calc_v7_p2);

                DrawBezierCurve(end_v6, curve_v7_p1, curve_v7_p2, start_v7);
                #endregion

                #region 第8分割
                float dis_v8_h = dis_v7_h;
                float dis_v8_w = (dis_v7_w / 1.618f);

                Vector3 pos_s_v8 = pos_s_v7 - (start_pos + Vector3.right * dis_v8_w);
                Vector3 start_v8 = rect_rightup.TransformPoint(pos_s_v8);

                Vector3 pos_e_v8 = pos_s_v8 - (start_pos + Vector3.up * dis_v8_h);
                Vector3 end_v8 = rect_rightup.TransformPoint(pos_e_v8);

                Gizmos.DrawLine(start_v8, end_v8);

                Vector3 curve_calc_v8_p1 = pos_s_v7 - (Vector3.up * (dis_v8_h / 2));
                Vector3 curve_v8_p1 = rect_rightup.TransformPoint(curve_calc_v8_p1);

                Vector3 curve_calc_v8_p2 = pos_e_v8 + (Vector3.right * (dis_v8_w / 2));
                Vector3 curve_v8_p2 = rect_rightup.TransformPoint(curve_calc_v8_p2);

                DrawBezierCurve(start_v7, curve_v8_p1, curve_v8_p2, end_v8);
                #endregion

                #region 第9分割
                float dis_v9_h = dis_v8_h - (dis_v8_h / 1.618f);
                float dis_v9_w = dis_v8_w / 1.618f;

                Vector3 pos_s_v9 = pos_s_v8 - (start_pos + Vector3.up * dis_v9_h);
                Vector3 start_v9 = rect_rightup.TransformPoint(pos_s_v9);

                Vector3 pos_e_v9 = pos_s_v9 - (start_pos + Vector3.right * dis_v9_w);
                Vector3 end_v9 = rect_rightup.TransformPoint(pos_e_v9);

                Gizmos.DrawLine(start_v9, end_v9);

                Vector3 curve_calc_v9_p1 = pos_e_v8 - (Vector3.right * (dis_v9_w / 2));
                Vector3 curve_v9_p1 = rect_rightup.TransformPoint(curve_calc_v9_p1);

                Vector3 curve_calc_v9_p2 = pos_e_v9 - (Vector3.up * ((dis_v8_h / 1.618f) / 2));
                Vector3 curve_v9_p2 = rect_rightup.TransformPoint(curve_calc_v9_p2);

                DrawBezierCurve(end_v8, curve_v9_p1, curve_v9_p2, end_v9);
                #endregion
            }
            Gizmos.color = Color.white;
        }

        /// <summary>
        /// 对角线构图
        /// </summary>
        private void CompGuide_DiagonalLine()
        {
            if (!UseCompGuide)
                return;

            float scr_w = ScreenRes.x;
            float scr_h = ScreenRes.y;

            RectTransform rect_leftup = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.左上);
            RectTransform rect_rightdown = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.右下);

            Vector3 start_pos = rect_leftup.anchoredPosition3D;
            Vector3 end_pos = rect_rightdown.anchoredPosition3D;

            Vector3 calc_pos_start = start_pos + (Vector3.right * scr_w * GuideParam_CornerLookat_Offset_H) - (Vector3.up * scr_h * GuideParam_CornerLookat_Offset_V);
            Vector3 start = rect_leftup.TransformPoint(calc_pos_start);

            Vector3 calc_pos_end = end_pos - (Vector3.right * scr_w * GuideParam_CornerLookat_Offset_H) + (Vector3.up * scr_h * GuideParam_CornerLookat_Offset_V);
            Vector3 end = rect_rightdown.TransformPoint(calc_pos_end);

            Gizmos.color = GuidePointColor;
            Vector3 center = CalculateMidpoint(start, end);
            Gizmos.DrawSphere(center, GuideParam_CenterPointSize * 0.005f);

            Gizmos.color = GuideColor;
            Gizmos.DrawLine(start, end);
            Gizmos.color = Color.white;
        }

        /// <summary>
        /// 三分线构图
        /// </summary>
        private void CompGuide_ThreeCut()
        {
            if (!UseCompGuide)
                return;

            float scr_w = ScreenRes.x;
            float scr_h = ScreenRes.y;

            Gizmos.color = GuideColor;

            #region 左分线
            RectTransform rect_leftup = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.左上);
            RectTransform rect_leftdown = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.左下);

            Vector3 L_start_pos = rect_leftup.anchoredPosition3D;
            Vector3 L_end_pos = rect_leftdown.anchoredPosition3D;

            Vector3 L_calc_pos_start = L_start_pos + (Vector3.right * scr_w * GuideParam_Three_Offset_H * 0.5f * GuideParam_Three_Offset_Coverage);
            Vector3 L_start = rect_leftup.TransformPoint(L_calc_pos_start);

            Vector3 L_calc_pos_end = L_end_pos + (Vector3.right * scr_w * GuideParam_Three_Offset_H * 0.5f * GuideParam_Three_Offset_Coverage);
            Vector3 L_end = rect_leftdown.TransformPoint(L_calc_pos_end);

            Gizmos.DrawLine(L_start, L_end);
            #endregion

            #region 右分线
            RectTransform rect_rightup = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.右上);
            RectTransform rect_rightdown = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.右下);

            Vector3 R_start_pos = rect_rightup.anchoredPosition3D;
            Vector3 R_end_pos = rect_rightdown.anchoredPosition3D;

            Vector3 R_calc_pos_start = R_start_pos - (Vector3.right * scr_w * GuideParam_Three_Offset_H * 0.5f * GuideParam_Three_Offset_Coverage);
            Vector3 R_start = rect_rightup.TransformPoint(R_calc_pos_start);

            Vector3 R_calc_pos_end = R_end_pos - (Vector3.right * scr_w * GuideParam_Three_Offset_H * 0.5f * GuideParam_Three_Offset_Coverage);
            Vector3 R_end = rect_rightdown.TransformPoint(R_calc_pos_end);

            Gizmos.DrawLine(R_start, R_end);
            #endregion

            #region 上分线
            Vector3 T_start_pos = rect_leftup.anchoredPosition3D;
            Vector3 T_end_pos = rect_rightup.anchoredPosition3D;

            Vector3 T_calc_pos_start = T_start_pos - (Vector3.up * scr_h * GuideParam_Three_Offset_V * 0.5f * GuideParam_Three_Offset_Coverage);
            Vector3 T_start = rect_leftup.TransformPoint(T_calc_pos_start);

            Vector3 T_calc_pos_end = T_end_pos - (Vector3.up * scr_h * GuideParam_Three_Offset_V * 0.5f * GuideParam_Three_Offset_Coverage);
            Vector3 T_end = rect_rightup.TransformPoint(T_calc_pos_end);

            Gizmos.DrawLine(T_start, T_end);
            #endregion

            #region 下分线
            Vector3 D_start_pos = rect_leftdown.anchoredPosition3D;
            Vector3 D_end_pos = rect_rightdown.anchoredPosition3D;

            Vector3 D_calc_pos_start = D_start_pos + (Vector3.up * scr_h * GuideParam_Three_Offset_V * 0.5f * GuideParam_Three_Offset_Coverage);
            Vector3 D_start = rect_leftdown.TransformPoint(D_calc_pos_start);

            Vector3 D_calc_pos_end = D_end_pos + (Vector3.up * scr_h * GuideParam_Three_Offset_V * 0.5f * GuideParam_Three_Offset_Coverage);
            Vector3 D_end = rect_rightdown.TransformPoint(D_calc_pos_end);

            Gizmos.DrawLine(D_start, D_end);
            #endregion

            Gizmos.color = Color.white;
        }

        /// <summary>
        /// 引导线构图
        /// </summary>
        private void CompGuide_GuideLineRef()
        {
            if (!UseCompGuide)
                return;

            float scr_w = ScreenRes.x;
            float scr_h = ScreenRes.y;

            Gizmos.color = GuideColor;

            RectTransform rect_down = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.下);
            RectTransform rect_leftdown = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.左下);
            RectTransform rect_rightdown = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.右下);

            #region 水平高度
            Vector3 Start_pos = rect_down.anchoredPosition3D;

            Vector3 BaseHeight_calc_pos_start = Start_pos + (Vector3.up * scr_h * GuideParam_GuideLine_BaseHeight) - (Vector3.right * (scr_w / 2));
            Vector3 BaseHeight_start = rect_down.TransformPoint(BaseHeight_calc_pos_start);

            Vector3 BaseHeight_calc_pos_end = Start_pos + (Vector3.up * scr_h * GuideParam_GuideLine_BaseHeight) + (Vector3.right * (scr_w / 2));
            Vector3 BaseHeight_end = rect_down.TransformPoint(BaseHeight_calc_pos_end);

            Gizmos.DrawLine(BaseHeight_start, BaseHeight_end);
            #endregion

            #region 左侧引导线
            Vector3 Start_pos_L = rect_leftdown.anchoredPosition3D;

            Vector3 Near_calc_pos_start_L = Start_pos_L + (Vector3.right * (scr_w / 2) * (GuideParam_GuideLine_Offset_Near)) + (Vector3.up * scr_h * GuideParam_GuideLine_Offset_NearHeight);
            Vector3 Near_start_L = rect_leftdown.TransformPoint(Near_calc_pos_start_L);

            Vector3 Far_calc_pos_end_L = Start_pos + (Vector3.up * scr_h * GuideParam_GuideLine_BaseHeight) - (Vector3.right * (scr_w / 2) * (GuideParam_GuideLine_Offset_Far) - ((Vector3.right * (scr_w / 2) * GuideParam_GuideLine_BaseOffset)));
            Vector3 Far_end_L = rect_down.TransformPoint(Far_calc_pos_end_L);

            Gizmos.DrawLine(Near_start_L, Far_end_L);
            #endregion

            #region 右侧引导线            
            Vector3 Start_pos_R = rect_rightdown.anchoredPosition3D;

            Vector3 Near_calc_pos_start_R = Start_pos_R - (Vector3.right * (scr_w / 2) * (GuideParam_GuideLine_Offset_Near)) + (Vector3.up * scr_h * GuideParam_GuideLine_Offset_NearHeight);
            Vector3 Near_start_R = rect_rightdown.TransformPoint(Near_calc_pos_start_R);

            Vector3 Far_calc_pos_end_R = Start_pos + (Vector3.up * scr_h * GuideParam_GuideLine_BaseHeight) + (Vector3.right * (scr_w / 2) * (GuideParam_GuideLine_Offset_Far) + ((Vector3.right * (scr_w / 2) * GuideParam_GuideLine_BaseOffset)));
            Vector3 Far_end_R = rect_down.TransformPoint(Far_calc_pos_end_R);

            Gizmos.DrawLine(Near_start_R, Far_end_R);
            #endregion

            Gizmos.color = GuidePointColor;
            Vector3 center = CalculateMidpoint(BaseHeight_start, BaseHeight_end);
            Gizmos.DrawSphere(center, GuideParam_CenterPointSize * 0.005f);

            Gizmos.color = Color.white;
        }

        /// <summary>
        /// 三角构图
        /// </summary>
        private void CompGuide_Triangle()
        {
            if (!UseCompGuide)
                return;

            float scr_w = ScreenRes.x;
            float scr_h = ScreenRes.y;


            RectTransform rect_down = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.下);
            RectTransform rect_leftdown = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.左下);
            RectTransform rect_rightdown = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.右下);

            #region 顶部高度
            Vector3 Start_pos = rect_down.anchoredPosition3D;

            Vector3 BaseHeight_calc_pos_start = Start_pos + (Vector3.up * scr_h * GuideParam_Triangle_BaseHeight) + (Vector3.right * (scr_w / 2) * GuideParam_Triangle_TopOffset);
            Vector3 BaseHeight_start = rect_down.TransformPoint(BaseHeight_calc_pos_start);

            Vector3 BaseHeight_calc_pos_end = Start_pos + (Vector3.up * scr_h * GuideParam_Triangle_BottomHeight);
            Vector3 BaseHeight_end = rect_down.TransformPoint(BaseHeight_calc_pos_end);
            #endregion

            Gizmos.color = GuideColor;

            #region 左侧角点线
            Vector3 Start_pos_L = rect_down.anchoredPosition3D;

            Vector3 Near_calc_pos_start_L = Start_pos_L - (Vector3.right * (scr_w / 2) * (GuideParam_Triangle_Offset_Left)) + BaseHeight_calc_pos_end;
            Vector3 Near_start_L = rect_down.TransformPoint(Near_calc_pos_start_L);

            Vector3 Far_calc_pos_end_L = BaseHeight_calc_pos_start;
            Vector3 Far_end_L = rect_down.TransformPoint(BaseHeight_calc_pos_start);

            Gizmos.DrawLine(Near_start_L, Far_end_L);
            #endregion

            #region 右侧角点线
            Vector3 Start_pos_R = rect_down.anchoredPosition3D;

            Vector3 Near_calc_pos_start_R = Start_pos_R + (Vector3.right * (scr_w / 2) * (GuideParam_Triangle_Offset_Right)) + BaseHeight_calc_pos_end;
            Vector3 Near_start_R = rect_down.TransformPoint(Near_calc_pos_start_R);

            Vector3 Far_calc_pos_end_R = BaseHeight_calc_pos_start;
            Vector3 Far_end_R = rect_down.TransformPoint(Far_calc_pos_end_R);

            Gizmos.DrawLine(Near_start_R, Far_end_R);
            #endregion

            Gizmos.DrawLine(Near_start_L, Near_start_R);

            Gizmos.color = GuidePointColor;
            Vector3 center = CalculateCentroid(Near_start_L, Far_end_L, Near_start_R);
            Gizmos.DrawSphere(center, GuideParam_CenterPointSize * 0.005f);

            Gizmos.color = Color.white;
        }

        /// <summary>
        /// 工字型构图
        /// </summary>
        private void CompGuide_IShape()
        {
            if (!UseCompGuide)
                return;

            float scr_w = ScreenRes.x;
            float scr_h = ScreenRes.y;


            RectTransform rect_down = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.下);
            RectTransform rect_leftdown = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.左下);
            RectTransform rect_rightdown = hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide.右下);

            Gizmos.color = GuideColor;

            #region 顶部
            Vector3 Start_pos = rect_down.anchoredPosition3D;

            Vector3 Isp_top_calc_pos_start = Start_pos + (Vector3.up * scr_h * GuideParam_IShape_TopHeight) - (Vector3.right * (scr_w / 2));
            Vector3 Isp_top_start = rect_down.TransformPoint(Isp_top_calc_pos_start);

            Vector3 Isp_top_calc_pos_end = Start_pos + (Vector3.up * scr_h * GuideParam_IShape_TopHeight) + (Vector3.right * (scr_w / 2));
            Vector3 Isp_top_end = rect_down.TransformPoint(Isp_top_calc_pos_end);

            Gizmos.DrawLine(Isp_top_start, Isp_top_end);
            #endregion

            #region 底部

            Vector3 Isp_bottom_calc_pos_start = Start_pos + (Vector3.up * scr_h * GuideParam_IShape_BottomHeight) - (Vector3.right * (scr_w / 2));
            Vector3 Isp_bottom_start = rect_down.TransformPoint(Isp_bottom_calc_pos_start);

            Vector3 Isp_bottom_calc_pos_end = Start_pos + (Vector3.up * scr_h * GuideParam_IShape_BottomHeight) + (Vector3.right * (scr_w / 2));
            Vector3 Isp_bottom_end = rect_down.TransformPoint(Isp_bottom_calc_pos_end);

            Gizmos.DrawLine(Isp_bottom_start, Isp_bottom_end);
            #endregion

            #region 左侧线
            Vector3 calc_pos_start_L = Start_pos + (Vector3.up * scr_h * GuideParam_IShape_TopHeight) - (Vector3.right * (scr_w / 2) * GuideParam_IShape_Offset_Left);
            Vector3 start_L = rect_down.TransformPoint(calc_pos_start_L);

            Vector3 calc_pos_end_L = Start_pos + (Vector3.up * scr_h * GuideParam_IShape_BottomHeight) - (Vector3.right * (scr_w / 2) * GuideParam_IShape_Offset_Left);
            Vector3 end_L = rect_down.TransformPoint(calc_pos_end_L);

            Gizmos.DrawLine(start_L, end_L);
            #endregion

            #region 右侧线
            Vector3 calc_pos_start_R = Start_pos + (Vector3.up * scr_h * GuideParam_IShape_TopHeight) + (Vector3.right * (scr_w / 2) * GuideParam_IShape_Offset_Right);
            Vector3 start_R = rect_down.TransformPoint(calc_pos_start_R);

            Vector3 calc_pos_end_R = Start_pos + (Vector3.up * scr_h * GuideParam_IShape_BottomHeight) + (Vector3.right * (scr_w / 2) * GuideParam_IShape_Offset_Right);
            Vector3 end_R = rect_down.TransformPoint(calc_pos_end_R);

            Gizmos.DrawLine(start_R, end_R);
            #endregion

            Gizmos.color = GuidePointColor;
            Vector3 center_top = CalculateMidpoint(Isp_top_start, Isp_top_end);
            Gizmos.DrawSphere(center_top, GuideParam_CenterPointSize * 0.005f);

            Vector3 center_bottom = CalculateMidpoint(Isp_bottom_start, Isp_bottom_end);
            Gizmos.DrawSphere(center_bottom, GuideParam_CenterPointSize * 0.005f);
            Gizmos.color = Color.white;
        }

        /// <summary>
        /// 计算三个点的中心点坐标
        /// </summary>
        /// <param tweenName="pointA">第一个点的坐标</param>
        /// <param tweenName="pointB">第二个点的坐标</param>
        /// <param tweenName="pointC">第三个点的坐标</param>
        /// <returns>中心点的坐标</returns>
        public static Vector3 CalculateCentroid(Vector3 pointA, Vector3 pointB, Vector3 pointC)
        {
            // 计算三个点的平均值
            return (pointA + pointB + pointC) / 3f;
        }

        /// <summary>
        /// 计算两个点的中心点坐标
        /// </summary>
        /// <param tweenName="pointA">第一个点的坐标</param>
        /// <param tweenName="pointB">第二个点的坐标</param>
        /// <returns>中心点的坐标</returns>
        public static Vector3 CalculateMidpoint(Vector3 pointA, Vector3 pointB)
        {
            // 计算两个点的平均值
            return (pointA + pointB) / 2f;
        }

        /// <summary>
        /// 曲线绘制
        /// </summary>
        /// <param tweenName="start">开始点</param>
        /// <param tweenName="control1">开始点贝塞尔</param>
        /// <param tweenName="control2">结束点贝塞尔</param>
        /// <param tweenName="end">结束点</param>
        private void DrawBezierCurve(Vector3 start, Vector3 control1, Vector3 control2, Vector3 end)
        {
            // 分割曲线的点数
            int segments = 30;

            // 绘制曲线
            for (int i = 0; i <= segments; i++)
            {
                float t = i / (float)segments;
                Vector3 point = CalculateBezierPoint(t, start, control1, control2, end);

                if (i == 0)
                {
                    Gizmos.DrawLine(start, point);
                }
                else
                {
                    Gizmos.DrawLine(previousPoint, point);
                }

                previousPoint = point;
            }
        }

        /// <summary>
        /// 计算贝塞尔
        /// </summary>
        /// <param tweenName="t"></param>
        /// <param tweenName="p0"></param>
        /// <param tweenName="p1"></param>
        /// <param tweenName="p2"></param>
        /// <param tweenName="p3"></param>
        /// <returns></returns>
        private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            // 贝塞尔曲线公式
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector3 p = uuu * p0; // 第一项
            p += 3 * uu * t * p1; // 第二项
            p += 3 * u * tt * p2; // 第三项
            p += ttt * p3; // 第四项

            return p;
        }

        /// <summary>
        /// 获取布局构图参考锚点根物体
        /// </summary>
        /// <returns></returns>
        public RectTransform hm_Auxiliary_GetAnchorRoot()
        {
            return CompGuide_AnchorRoot;
        }

        /// <summary>
        /// 获取布局构图参考锚点
        /// </summary>
        /// <param tweenName="anchortype"></param>
        /// <returns></returns>
        public RectTransform hm_Auxiliary_GetAnchorRoot(HudAnchors_CompGuide anchortype)
        {
            RectTransform rect = null;
            for (int i = 0; i < CompGuide_Anchors.Length; i++)
            {
                if (CompGuide_Anchors[i].Type == anchortype)
                {
                    rect = CompGuide_Anchors[i].Anchor;
                    break;
                }
            }

            return rect;
        }
        #endregion

        #region 判断是否正在与UI进行交互
        IEnumerator CheckUIInteraction()
        {
            while (true)
            {
                IsInteractionUI = EventSystem.current.IsPointerOverGameObject();

                if (IsInteractionUI)
                {
                    Interacte_Invalid = false;
                    if (!Interacte_Valid)
                    {
                        Interacte_Valid = true;
                        if (Act_IsInteractionUI != null)
                            Act_IsInteractionUI(IsInteractionUI);
                    }
                }
                else
                {
                    Interacte_Valid = false;
                    if (!Interacte_Invalid)
                    {
                        Interacte_Invalid = true;
                        if (Act_IsInteractionUI != null)
                            Act_IsInteractionUI(IsInteractionUI);
                    }
                }

                yield return null;
            }
        }
        #endregion

        #region 布局
        /// <summary>
        /// 布局锚点更新
        /// </summary>
        public void hm_Layout_Update()
        {
            if (HudCanvas_Screen != null)
            {
                HudCanvas_Screen.renderMode = RenderMode.ScreenSpaceCamera;

                if (HudCamera != null)
                {
                    HudCanvas_Screen.worldCamera = HudCamera;
                }
            }

            if (HudCanvas_World != null)
            {
                HudCanvas_World.renderMode = RenderMode.WorldSpace;
                if (SceneCamera != null)
                {
                    HudCanvas_World.worldCamera = SceneCamera;
                }
            }

            if (Anchors_Layout_Screen != null)
            {
                for (int i = 0; i < Anchors_Layout_Screen.Count; i++)
                {
                    string sp_name = Anchors_Layout_Screen[i].Name;
                    RectTransform rect = Anchors_Layout_Screen[i].Anchor;

                    if (sp_name == "Anchor_B")
                    {
                        Anchors_Layout_Screen[i].Type = HudAnchor.底层;
                        if (rect != null)
                            rect.anchoredPosition3D = new Vector3(0, 0, 0);
                    }
                    if (sp_name == "Anchor_U")
                    {
                        Anchors_Layout_Screen[i].Type = HudAnchor.上;
                        if (rect != null)
                            rect.anchoredPosition3D = new Vector3(0, (-Margins.x - MarginVertical) * MarginMultiply, 0);
                    }
                    if (sp_name == "Anchor_D")
                    {
                        Anchors_Layout_Screen[i].Type = HudAnchor.下;
                        if (rect != null)
                            rect.anchoredPosition3D = new Vector3(0, (Margins.y + MarginVertical) * MarginMultiply, 0);
                    }
                    if (sp_name == "Anchor_L")
                    {
                        Anchors_Layout_Screen[i].Type = HudAnchor.左;
                        if (rect != null)
                            rect.anchoredPosition3D = new Vector3((Margins.z + MarginHorizontal) * MarginMultiply, 0, 0);
                    }
                    if (sp_name == "Anchor_R")
                    {
                        Anchors_Layout_Screen[i].Type = HudAnchor.右;
                        if (rect != null)
                            rect.anchoredPosition3D = new Vector3((-Margins.w - MarginHorizontal) * MarginMultiply, 0, 0);
                    }
                    if (sp_name == "Anchor_C")
                    {
                        Anchors_Layout_Screen[i].Type = HudAnchor.中心;
                        if (rect != null)
                            rect.anchoredPosition3D = new Vector3(0, 0, 0);
                    }
                    if (sp_name == "Anchor_L_U")
                    {
                        Anchors_Layout_Screen[i].Type = HudAnchor.左上;
                        if (rect != null)
                            rect.anchoredPosition3D = new Vector3((Margins.z + MarginHorizontal) * MarginMultiply, (-Margins.x - MarginVertical) * MarginMultiply, 0);
                    }
                    if (sp_name == "Anchor_L_D")
                    {
                        Anchors_Layout_Screen[i].Type = HudAnchor.左下;
                        if (rect != null)
                            rect.anchoredPosition3D = new Vector3((Margins.z + MarginHorizontal) * MarginMultiply, (Margins.y + MarginVertical) * MarginMultiply, 0);
                    }
                    if (sp_name == "Anchor_R_U")
                    {
                        Anchors_Layout_Screen[i].Type = HudAnchor.右上;
                        if (rect != null)
                            rect.anchoredPosition3D = new Vector3((-Margins.w - MarginHorizontal) * MarginMultiply, (-Margins.x - MarginVertical) * MarginMultiply, 0);
                    }
                    if (sp_name == "Anchor_R_D")
                    {
                        Anchors_Layout_Screen[i].Type = HudAnchor.右下;
                        if (rect != null)
                            rect.anchoredPosition3D = new Vector3((-Margins.w - MarginHorizontal) * MarginMultiply, (Margins.y + MarginVertical) * MarginMultiply, 0);
                    }
                    if (sp_name == "Anchor_T")
                    {
                        Anchors_Layout_Screen[i].Type = HudAnchor.顶层;
                        if (rect != null)
                            rect.anchoredPosition3D = new Vector3(0, 0, 0);
                    }

                    Image rect_mark = Anchors_Layout_Screen[i].Mark;

                    if (rect_mark != null)
                    {
                        rect_mark.rectTransform.sizeDelta = Vector2.one * MarkSize;
                        rect_mark.color = Color_LayoutAnchorMark;
                        rect_mark.transform.SetAsLastSibling();
                    }
                }
            }
        }

        /// <summary>
        /// 设置Hud画布的锚点位置
        /// </summary>
        /// <param tweenName="anchor">画布目标锚点</param>
        public void hm_Layout_CanvasDistance(CanvasAnchor anchor)
        {
            if (HudCamera == null)
                return;
            switch (anchor)
            {
                case CanvasAnchor.CameraNear:
                    CanvasDistance = HudCamera.nearClipPlane + 0.001f;
                    break;
                case CanvasAnchor.CameraFar:
                    CanvasDistance = HudCamera.farClipPlane - 0.001f;
                    break;
                case CanvasAnchor.Custom:

                    break;
            }
        }

        /// <summary>
        /// 根据锚点类型获取锚点根物体
        /// </summary>
        /// <param tweenName="anchor"></param>
        /// <returns></returns>
        public RectTransform hm_Layout_GetAnchor(HudAnchor anchor)
        {
            RectTransform rect = null;
            for (int i = 0; i < Anchors_Layout_Screen.Count; i++)
            {
                if (Anchors_Layout_Screen[i].Type == anchor)
                {
                    rect = Anchors_Layout_Screen[i].Anchor;
                    break;
                }
            }
            return rect;
        }

        /// <summary>
        /// 获取布局边距
        /// </summary>
        /// <param tweenName="anchor"></param>
        /// <returns></returns>
        public float hm_Layout_GetMargins(HudAnchorMargin anchor)
        {
            float val = 0;
            switch (anchor)
            {
                case HudAnchorMargin.上:
                    val = Margins.x;
                    break;
                case HudAnchorMargin.下:
                    val = Margins.y;
                    break;
                case HudAnchorMargin.左:
                    val = Margins.z;
                    break;
                case HudAnchorMargin.右:
                    val = Margins.w;
                    break;
            }
            return val;
        }

        /// <summary>
        /// 更新画布距离
        /// </summary>
        public void hm_Layout_CanvasDistance_Update()
        {
            if (HudCanvas_Screen == null)
                return;
            HudCanvas_Screen.planeDistance = CanvasDistance;
        }
        #endregion

        #region 相机

        /// <summary>
        /// 设置相机剪切范围
        /// </summary>
        /// <param tweenName="near">近距</param>
        /// <param tweenName="far">远距</param>
        public void hm_CameraCutterRange(float near, float far)
        {
            if (HudCamera == null)
                return;
            HudCamera.nearClipPlane = near;
            HudCamera.farClipPlane = far;
        }

        /// <summary>
        /// 相机投影方式设置
        /// </summary>
        /// <param tweenName="treeState">投影方式</param>
        public void hm_CameraOrthographicProjection(bool state)
        {
            if (HudCamera == null)
                return;
            HudCamera.orthographic = state;
        }

        /// <summary>
        /// 相机正交投影尺寸
        /// </summary>
        /// <param tweenName="size">相机正交尺寸</param>
        public void hm_CameraOrthographicSize(float size)
        {
            if (HudCamera == null)
                return;
            HudCamera.orthographicSize = size;
        }

        /// <summary>
        /// 相机透视投影尺寸
        /// </summary>
        /// <param tweenName="fov">透视角焦距</param>
        public void hm_CameraPerspectiveFov(float fov)
        {
            if (HudCamera == null)
                return;
            HudCamera.fieldOfView = fov;
        }

        #endregion

        #region 场景相机

        /// <summary>
        /// 检查场景相机并自动添加叠加相机堆栈
        /// </summary>
        /// <param tweenName="cam">场景相机</param>
        public void hm_SceneCam_CheckStack(Camera cam)
        {
            if (cam == null)
                return;
            UniversalAdditionalCameraData camdata = cam.GetUniversalAdditionalCameraData();

            bool sw = false;
            for (int i = 0; i < camdata.cameraStack.Count; i++)
            {
                if (camdata.cameraStack[i] == HudCamera)
                {
                    sw = true;
                    break;
                }
            }
            if (!sw)
            {
                camdata.cameraStack.Add(HudCamera);
            }
        }

        #endregion

        #region 安全框

        /// <summary>
        /// 辅助安全框架更新
        /// </summary>
        public void hm_SafeFrameUpdate()
        {
            if (!UseSafeFrame)
                return;

            hm_Safe_ClampValue();

            hm_Safe_FrameLine_Update();

            hm_Safe_Seperater_Update();

            hm_Safe_CenterMark_Update();
        }

        /// <summary>
        ///分割线更新
        /// </summary>
        public void hm_Safe_Seperater_Update()
        {
            if (Safe_Seperater != null)
            {
                Vector2 pivot = Vector2.zero;
                Vector2 anchorMin = Vector2.zero;
                Vector2 anchorMax = Vector2.zero;
                Vector2 sizeDelta = Vector2.zero;
                Vector2 anchoredPosition3D = Vector2.zero;

                for (int i = 0; i < Safe_Seperater.Length; i++)
                {
                    if (Safe_Seperater[i] == null)
                        continue;
                    if (Safe_Seperater[i].Line == null)
                        continue;
                    if (Safe_Seperater[i].Line.rectTransform == null)
                        continue;

                    Safe_Seperater[i].Line.color = Color_SeperaterLine;

                    if (Safe_Seperater[i].Type == HudAnchor.上)
                    {
                        pivot = new Vector2(0.5f, 0.5f);
                        anchorMin = new Vector2(0.5f, 1f);
                        anchorMax = new Vector2(0.5f, 1f);
                        sizeDelta = new Vector2(Safe_FrameLine_Width, Safe_Seperater_Length);
                        anchoredPosition3D = new Vector3(0, -Safe_FrameLine_Margins.y, 0);
                    }
                    if (Safe_Seperater[i].Type == HudAnchor.下)
                    {
                        pivot = new Vector2(0.5f, 0.5f);
                        anchorMin = new Vector2(0.5f, 0f);
                        anchorMax = new Vector2(0.5f, 0f);
                        sizeDelta = new Vector2(Safe_FrameLine_Width, Safe_Seperater_Length);
                        anchoredPosition3D = new Vector3(0, Safe_FrameLine_Margins.y, 0);
                    }
                    if (Safe_Seperater[i].Type == HudAnchor.左)
                    {
                        pivot = new Vector2(0.5f, 0.5f);
                        anchorMin = new Vector2(0f, 0.5f);
                        anchorMax = new Vector2(0f, 0.5f);
                        sizeDelta = new Vector2(Safe_Seperater_Length, Safe_FrameLine_Width);
                        anchoredPosition3D = new Vector3(Safe_FrameLine_Margins.x, 0, 0);
                    }
                    if (Safe_Seperater[i].Type == HudAnchor.右)
                    {
                        pivot = new Vector2(0.5f, 0.5f);
                        anchorMin = new Vector2(1f, 0.5f);
                        anchorMax = new Vector2(1f, 0.5f);
                        sizeDelta = new Vector2(Safe_Seperater_Length, Safe_FrameLine_Width);
                        anchoredPosition3D = new Vector3(-Safe_FrameLine_Margins.x, 0, 0);
                    }

                    Safe_Seperater[i].Line.rectTransform.pivot = pivot;
                    Safe_Seperater[i].Line.rectTransform.anchorMin = anchorMin;
                    Safe_Seperater[i].Line.rectTransform.anchorMax = anchorMax;
                    Safe_Seperater[i].Line.rectTransform.sizeDelta = sizeDelta;
                    Safe_Seperater[i].Line.rectTransform.anchoredPosition3D = anchoredPosition3D;
                }
            }
        }

        /// <summary>
        /// 框线更新
        /// </summary>
        public void hm_Safe_FrameLine_Update()
        {
            if (Safe_FrameLine != null)
            {
                Vector2 pivot = Vector2.zero;
                Vector2 anchorMin = Vector2.zero;
                Vector2 anchorMax = Vector2.zero;
                Vector2 sizeDelta = Vector2.zero;
                Vector2 anchoredPosition3D = Vector2.zero;

                for (int i = 0; i < Safe_FrameLine.Length; i++)
                {
                    if (Safe_FrameLine[i] == null)
                        continue;
                    if (Safe_FrameLine[i].Frame == null)
                        continue;
                    if (Safe_FrameLine[i].Frame.rectTransform == null)
                        continue;

                    Safe_FrameLine[i].Frame.color = Color_FrameLine;

                    if (Safe_FrameLine[i].Type == HudAnchor.上)
                    {
                        if (CanvasScalerModeIndex == 0)
                        {
                            pivot = new Vector2(0.5f, 1f);
                            anchorMin = new Vector2(0.5f, 1f);
                            anchorMax = new Vector2(0.5f, 1f);
                            sizeDelta = new Vector2(ScreenRes.x - (Safe_FrameLine_Margins.x * 2), Safe_FrameLine_Width);
                            anchoredPosition3D = new Vector3(0, -Safe_FrameLine_Margins.y, 0);
                        }
                        if (CanvasScalerModeIndex == 1)
                        {
                            pivot = new Vector2(0.5f, 1f);
                            anchorMin = new Vector2(0.5f, 1f);
                            anchorMax = new Vector2(0.5f, 1f);
                            sizeDelta = new Vector2(CanvasScalerScreenSize.x - (Safe_FrameLine_Margins.x * 2), Safe_FrameLine_Width);
                            anchoredPosition3D = new Vector3(0, -Safe_FrameLine_Margins.y, 0);
                        }
                    }
                    if (Safe_FrameLine[i].Type == HudAnchor.下)
                    {
                        if (CanvasScalerModeIndex == 0)
                        {
                            pivot = new Vector2(0.5f, 0f);
                            anchorMin = new Vector2(0.5f, 0f);
                            anchorMax = new Vector2(0.5f, 0f);
                            sizeDelta = new Vector2(ScreenRes.x - (Safe_FrameLine_Margins.x * 2), Safe_FrameLine_Width);
                            anchoredPosition3D = new Vector3(0, Safe_FrameLine_Margins.y, 0);
                        }
                        if (CanvasScalerModeIndex == 1)
                        {
                            pivot = new Vector2(0.5f, 0f);
                            anchorMin = new Vector2(0.5f, 0f);
                            anchorMax = new Vector2(0.5f, 0f);
                            sizeDelta = new Vector2(CanvasScalerScreenSize.x - (Safe_FrameLine_Margins.x * 2), Safe_FrameLine_Width);
                            anchoredPosition3D = new Vector3(0, Safe_FrameLine_Margins.y, 0);
                        }
                    }
                    if (Safe_FrameLine[i].Type == HudAnchor.左)
                    {
                        if (CanvasScalerModeIndex == 0)
                        {
                            pivot = new Vector2(0f, 0.5f);
                            anchorMin = new Vector2(0f, 0.5f);
                            anchorMax = new Vector2(0f, 0.5f);
                            sizeDelta = new Vector2(Safe_FrameLine_Width, ScreenRes.y - (Safe_FrameLine_Margins.y * 2));
                            anchoredPosition3D = new Vector3(Safe_FrameLine_Margins.x, 0, 0);
                        }
                        if (CanvasScalerModeIndex == 1)
                        {
                            pivot = new Vector2(0f, 0.5f);
                            anchorMin = new Vector2(0f, 0.5f);
                            anchorMax = new Vector2(0f, 0.5f);
                            sizeDelta = new Vector2(Safe_FrameLine_Width, CanvasScalerScreenSize.y - (Safe_FrameLine_Margins.y * 2));
                            anchoredPosition3D = new Vector3(Safe_FrameLine_Margins.x, 0, 0);
                        }
                    }
                    if (Safe_FrameLine[i].Type == HudAnchor.右)
                    {
                        if (CanvasScalerModeIndex == 0)
                        {
                            pivot = new Vector2(1f, 0.5f);
                            anchorMin = new Vector2(1f, 0.5f);
                            anchorMax = new Vector2(1f, 0.5f);
                            sizeDelta = new Vector2(Safe_FrameLine_Width, ScreenRes.y - (Safe_FrameLine_Margins.y * 2));
                            anchoredPosition3D = new Vector3(-Safe_FrameLine_Margins.x, 0, 0);
                        }
                        if (CanvasScalerModeIndex == 1)
                        {
                            pivot = new Vector2(1f, 0.5f);
                            anchorMin = new Vector2(1f, 0.5f);
                            anchorMax = new Vector2(1f, 0.5f);
                            sizeDelta = new Vector2(Safe_FrameLine_Width, CanvasScalerScreenSize.y - (Safe_FrameLine_Margins.y * 2));
                            anchoredPosition3D = new Vector3(-Safe_FrameLine_Margins.x, 0, 0);
                        }
                    }

                    Safe_FrameLine[i].Frame.rectTransform.pivot = pivot;
                    Safe_FrameLine[i].Frame.rectTransform.anchorMin = anchorMin;
                    Safe_FrameLine[i].Frame.rectTransform.anchorMax = anchorMax;
                    Safe_FrameLine[i].Frame.rectTransform.sizeDelta = sizeDelta;
                    Safe_FrameLine[i].Frame.rectTransform.anchoredPosition3D = anchoredPosition3D;
                }
            }
        }

        /// <summary>
        /// 限制数值
        /// </summary>
        private void hm_Safe_ClampValue()
        {
            Safe_FrameLine_Width = Mathf.Clamp(Safe_FrameLine_Width, 0, float.MaxValue);
            Safe_Seperater_Length = Mathf.Clamp(Safe_Seperater_Length, 0, float.MaxValue);

            Safe_FrameLine_Margins.x = Mathf.Clamp(Safe_FrameLine_Margins.x, 0, float.MaxValue);
            Safe_FrameLine_Margins.y = Mathf.Clamp(Safe_FrameLine_Margins.y, 0, float.MaxValue);
        }

        /// <summary>
        /// 中心标记物布局
        /// </summary>
        public void hm_Safe_CenterMark_Update()
        {
            if (Safe_CenterMarks != null)
            {
                if (Safe_CenterMarks.Edge != null)
                {
                    for (int i = 0; i < Safe_CenterMarks.Edge.Count; i++)
                    {
                        if (Safe_CenterMarks.Edge[i] != null)
                        {
                            Safe_CenterMarks.Edge[i].rectTransform.sizeDelta = new Vector2(Safe_CenterMarkLength, Safe_CenterMarkWidth);
                            Vector3 pos = Vector3.zero;

                            if (i == 0)
                            {
                                pos.x = Safe_CenterMarkDistance;
                                pos.y = Safe_CenterMarkDistance;
                            }
                            if (i == 1)
                            {
                                pos.x = -Safe_CenterMarkDistance;
                                pos.y = Safe_CenterMarkDistance;
                            }
                            if (i == 2)
                            {
                                pos.x = Safe_CenterMarkDistance;
                                pos.y = -Safe_CenterMarkDistance;
                            }
                            if (i == 3)
                            {
                                pos.x = -Safe_CenterMarkDistance;
                                pos.y = -Safe_CenterMarkDistance;
                            }
                            Safe_CenterMarks.Edge[i].rectTransform.localPosition = pos;
                            Safe_CenterMarks.Edge[i].color = Color_CenterMark;
                        }
                    }
                }
            }
        }

        #endregion

        #region RMS布局匹配

        /// <summary>
        /// 获取RMS的节点列表
        /// </summary>
        public ScreenResolutionNode[] hm_RMS_GetResolutionNodes()
        {
            return RMS_Nodes.ToArray();
        }

        /// <summary>
        /// 获取RMS的节点所有名称
        /// </summary>
        public string[] hm_RMS_GetResolutionNodeNames()
        {
            string[] names = new string[RMS_Nodes.Count];
            for (int i = 0; i < RMS_Nodes.Count; i++)
            {
                names[i] = RMS_Nodes[i].Indicator;
            }
            return names;
        }

        /// <summary>
        /// 获取RMS的节点
        /// </summary>
        public ScreenResolutionNode hm_RMS_GetResolutionNode(string name)
        {
            ScreenResolutionNode node = new ScreenResolutionNode();
            for (int i = 0; i < RMS_Nodes.Count; i++)
            {
                if (RMS_Nodes[i].Indicator == name)
                    node = RMS_Nodes[i];
            }
            return node;
        }

        /// <summary>
        /// 获取当前使用中的RMS布局方案
        /// </summary>
        /// <returns></returns>
        public string hm_RMS_GetCurrentSolution()
        {
            return RMS_CurrentSolution;
        }

        /// <summary>
        /// 根据方案名称获取RMS的索引号
        /// </summary>
        /// <param tweenName="solution"></param>
        /// <returns></returns>
        public int hm_RMS_GetSolutionIndex(string solution)
        {
            int index = 0;
            for (int i = 0; i < RMS_Nodes.Count; i++)
            {
                if (solution == RMS_Nodes[i].Indicator)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        /// <summary>
        /// 获取匹配屏幕分辨率节点列表总数
        /// </summary>
        public int hm_RMS_GetResolutionNodesLength()
        {
            return RMS_Nodes.Count;
        }

        /// <summary>
        /// 获取匹配屏幕分辨率节点列表是否为空
        /// </summary>
        public bool hm_RMS_IsEmpty()
        {
            return RMS_Nodes.Count > 0 ? false : true;
        }

        #endregion

        #region 分辨率切换
        public void hm_SetResolution(string indicator, FullScreenMode mode)
        {
            for (int i = 0; i < RMS_Nodes.Count; i++)
            {
                if (indicator == RMS_Nodes[i].Indicator)
                {
                    hm_SetResolution((int)RMS_Nodes[i].Res.x, (int)RMS_Nodes[i].Res.y, mode);
                    RMS_CurrentSolution = indicator;
                    if (Act_ScreenResolution_Changed != null)
                        Act_ScreenResolution_Changed(indicator, RMS_Nodes[i].Res);
                }
            }
        }

        public void hm_SetResolution(int width, int height, FullScreenMode mode)
        {
            Screen.SetResolution(width, height, mode);
        }
        #endregion

        #region 辅助

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

        /// <summary>
        /// 检测是否存在已生成的Hud元素
        /// </summary>
        /// <returns>返回True则当前Hud Manager管理器消息中已生成了Hud元素反之则说明已清空</returns>
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
        /// <param tweenName="treeState">是否开启像素对齐</param>
        public void hm_UsePixelPerfect(bool state)
        {
            if (!UsePerfectPixelUpdate)
                return;
            HudCanvas_Screen.pixelPerfect = state;
        }

        /// <summary>
        /// 为指定的相机堆栈添加Hud叠加层
        /// </summary>
        /// <param tweenName="uac"></param>
        public void hm_AssignedCameraStack(UniversalAdditionalCameraData uac)
        {
            if (HudCamera == null)
                if (UseDebug)
                    util_Tools.Func_PrintInfo("Hud Manager管理器消息", "当前HudCamera为空，无法为指定的相机堆栈添加Hud叠加层", HudMsgState.警告);
            if (!uac.cameraStack.Contains(HudCamera))
            {
                uac.cameraStack.Add(HudCamera);
            }
        }

        /// <summary>
        /// Hud Manager管理器消息 - 刷新所有生成的Hud元素的动画状态
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

        #endregion

        #region 字体尺寸

        /// <summary>
        /// 修改字体尺寸
        /// </summary>
        /// <param tweenName="size"></param>
        public void hm_ChangeFontGlobalSize(float size)
        {
            FontSizeMultiply = size;
            if (Act_FontSize_Changed != null)
                Act_FontSize_Changed();
        }

        #endregion

        #region 辅助视觉

        #region 创建

        /// <summary>
        /// 锚点可视化 - 创建
        /// </summary>
        public void hm_AnchorMarks_Create()
        {
            if (Safe_Frame != null)
                return;
            if (Anchors_Layout_Screen == null || Anchors_Layout_Screen.Count <= 0)
                return;

            hm_CreateSafeElements();

            hm_CreateCornerMark();
        }

        /// <summary>
        /// 创建锚点标记物
        /// </summary>
        /// <param tweenName="parent"></param>
        /// <param tweenName="name"></param>
        /// <returns></returns>
        private Image hm_CreateAnchorMark(Transform parent, string name)
        {
            GameObject obj = new GameObject();
#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(obj, "MarkObject");
#endif
            obj.name = name;
            obj.layer = LayerMask.NameToLayer("XHud");
            obj.transform.SetParent(parent);
            obj.transform.SetAsFirstSibling();
            obj.transform.localScale = Vector3.one;
            Image img = obj.AddComponent<Image>();
            img.raycastTarget = false;
            return img;
        }

        /// <summary>
        /// 创建角落标记
        /// </summary>
        private void hm_CreateCornerMark()
        {
            for (int i = 0; i < Anchors_Layout_Screen.Count; i++)
            {
                if (Anchors_Layout_Screen[i].Mark != null)
                    continue;
                else
                {
                    Image img = hm_CreateAnchorMark(Anchors_Layout_Screen[i].Anchor, "Mark");

                    switch (Anchors_Layout_Screen[i].Type)
                    {
                        case HudAnchor.上:
                            img.rectTransform.pivot = new Vector2(0.5f, 1f);
                            img.rectTransform.anchorMin = new Vector2(0.5f, 1f);
                            img.rectTransform.anchorMax = new Vector2(0.5f, 1f);
                            img.rectTransform.anchoredPosition3D = Vector3.zero;
                            break;
                        case HudAnchor.下:
                            img.rectTransform.pivot = new Vector2(0.5f, 0f);
                            img.rectTransform.anchorMin = new Vector2(0.5f, 0f);
                            img.rectTransform.anchorMax = new Vector2(0.5f, 0f);
                            img.rectTransform.anchoredPosition3D = Vector3.zero;
                            break;
                        case HudAnchor.左:
                            img.rectTransform.pivot = new Vector2(0f, 0.5f);
                            img.rectTransform.anchorMin = new Vector2(0f, 0.5f);
                            img.rectTransform.anchorMax = new Vector2(0f, 0.5f);
                            img.rectTransform.anchoredPosition3D = Vector3.zero;
                            break;
                        case HudAnchor.右:
                            img.rectTransform.pivot = new Vector2(1f, 0.5f);
                            img.rectTransform.anchorMin = new Vector2(1f, 0.5f);
                            img.rectTransform.anchorMax = new Vector2(1f, 0.5f);
                            img.rectTransform.anchoredPosition3D = Vector3.zero;
                            break;
                        case HudAnchor.中心:
                            img.rectTransform.pivot = new Vector2(0.5f, 0.5f);
                            img.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                            img.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                            img.rectTransform.anchoredPosition3D = Vector3.zero;

                            #region 中标标记
                            Safe_CenterMarks.Center = img;

                            ///----创建中心标记
                            for (int s = 0; s < 4; s++)
                            {
                                Image img_mark = hm_CreateAnchorMark(img.transform, "ch");
                                img_mark.rectTransform.sizeDelta = new Vector2(15f, 2f);
                                if (s == 0)
                                {
                                    img_mark.rectTransform.pivot = new Vector2(0f, 0.5f);
                                    img_mark.rectTransform.anchoredPosition3D = Vector3.zero + new Vector3(20, 20, 0);
                                    img_mark.rectTransform.localRotation = Quaternion.Euler(0, 0, 45);
                                }
                                else if (s == 1)
                                {
                                    img_mark.rectTransform.pivot = new Vector2(1f, 0.5f);
                                    img_mark.rectTransform.anchoredPosition3D = Vector3.zero + new Vector3(-20, 20, 0);
                                    img_mark.rectTransform.localRotation = Quaternion.Euler(0, 0, -45);
                                }
                                else if (s == 2)
                                {
                                    img_mark.rectTransform.pivot = new Vector2(1f, 0.5f);
                                    img_mark.rectTransform.anchoredPosition3D = Vector3.zero + new Vector3(20, -20, 0);
                                    img_mark.rectTransform.localRotation = Quaternion.Euler(0, 0, 135);
                                }
                                else if (s == 3)
                                {
                                    img_mark.rectTransform.pivot = new Vector2(0f, 0.5f);
                                    img_mark.rectTransform.anchoredPosition3D = Vector3.zero + new Vector3(-20, -20, 0);
                                    img_mark.rectTransform.localRotation = Quaternion.Euler(0, 0, -135);
                                }

                                Safe_CenterMarks.Edge.Add(img_mark);
                            }
                            #endregion
                            break;
                        case HudAnchor.左上:
                            img.rectTransform.pivot = new Vector2(0f, 1f);
                            img.rectTransform.anchorMin = new Vector2(0f, 1f);
                            img.rectTransform.anchorMax = new Vector2(0f, 1f);
                            img.rectTransform.anchoredPosition3D = Vector3.zero;
                            break;
                        case HudAnchor.左下:
                            img.rectTransform.pivot = new Vector2(0f, 0f);
                            img.rectTransform.anchorMin = new Vector2(0f, 0f);
                            img.rectTransform.anchorMax = new Vector2(0f, 0f);
                            img.rectTransform.anchoredPosition3D = Vector3.zero;
                            break;
                        case HudAnchor.右上:
                            img.rectTransform.pivot = new Vector2(1f, 1f);
                            img.rectTransform.anchorMin = new Vector2(1f, 1f);
                            img.rectTransform.anchorMax = new Vector2(1f, 1f);
                            img.rectTransform.anchoredPosition3D = Vector3.zero;
                            break;
                        case HudAnchor.右下:
                            img.rectTransform.pivot = new Vector2(1f, 0f);
                            img.rectTransform.anchorMin = new Vector2(1f, 0f);
                            img.rectTransform.anchorMax = new Vector2(1f, 0f);
                            img.rectTransform.anchoredPosition3D = Vector3.zero;
                            break;
                        case HudAnchor.底层:
                            img.rectTransform.pivot = new Vector2(0.5f, 0.5f);
                            img.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                            img.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                            img.rectTransform.anchoredPosition3D = Vector3.zero;
                            break;
                        case HudAnchor.顶层:
                            img.rectTransform.pivot = new Vector2(0.5f, 0.5f);
                            img.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                            img.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                            img.rectTransform.anchoredPosition3D = Vector3.zero;
                            break;
                    }
                    Anchors_Layout_Screen[i].Mark = img;
                }
            }
        }

        /// <summary>
        /// 创建安全框
        /// </summary>
        private void hm_CreateSafeElements()
        {
            GameObject obj_SafeFrame_Seperaters = new GameObject();
            obj_SafeFrame_Seperaters.name = "SafeFrames";
            obj_SafeFrame_Seperaters.layer = LayerMask.NameToLayer("XHud");
            RectTransform SafeFrame_Seperaters = obj_SafeFrame_Seperaters.AddComponent<RectTransform>();
            Canvas canvas = (Canvas)HudCanvas_Screen;
            SafeFrame_Seperaters.SetParent(canvas.transform);
            SafeFrame_Seperaters.localPosition = Vector3.zero;
            SafeFrame_Seperaters.localScale = Vector3.one;
            SafeFrame_Seperaters.anchorMin = new Vector2(0, 0);
            SafeFrame_Seperaters.anchorMax = new Vector2(1, 1);
            SafeFrame_Seperaters.sizeDelta = new Vector2(0, 0);

            Safe_FrameLine img_frameline_up = new Safe_FrameLine();
            img_frameline_up.Type = HudAnchor.上;
            img_frameline_up.Frame = hm_CreateAnchorMark(obj_SafeFrame_Seperaters.transform, "frameline_up");

            Safe_FrameLine img_frameline_down = new Safe_FrameLine();
            img_frameline_down.Type = HudAnchor.下;
            img_frameline_down.Frame = hm_CreateAnchorMark(obj_SafeFrame_Seperaters.transform, "frameline_down");

            Safe_FrameLine img_frameline_left = new Safe_FrameLine();
            img_frameline_left.Type = HudAnchor.左;
            img_frameline_left.Frame = hm_CreateAnchorMark(obj_SafeFrame_Seperaters.transform, "frameline_left");

            Safe_FrameLine img_frameline_right = new Safe_FrameLine();
            img_frameline_right.Type = HudAnchor.右;
            img_frameline_right.Frame = hm_CreateAnchorMark(obj_SafeFrame_Seperaters.transform, "frameline_right");

            List<Safe_FrameLine> imgs = new List<Safe_FrameLine>();
            imgs.Add(img_frameline_up);
            imgs.Add(img_frameline_down);
            imgs.Add(img_frameline_left);
            imgs.Add(img_frameline_right);

            Safe_FrameLine = new Safe_FrameLine[imgs.Count];
            for (int i = 0; i < imgs.Count; i++)
            {
                Safe_FrameLine[i] = new Hud.Safe_FrameLine();
                Safe_FrameLine[i].Frame = imgs[i].Frame;
                Safe_FrameLine[i].Type = imgs[i].Type;
            }

            Safe_Seperater img_seperater_up = new Safe_Seperater();
            img_seperater_up.Type = HudAnchor.上;
            img_seperater_up.Line = hm_CreateAnchorMark(obj_SafeFrame_Seperaters.transform, "seperater_up");

            Safe_Seperater img_seperater_down = new Safe_Seperater();
            img_seperater_down.Type = HudAnchor.下;
            img_seperater_down.Line = hm_CreateAnchorMark(obj_SafeFrame_Seperaters.transform, "seperater_down");

            Safe_Seperater img_seperater_left = new Safe_Seperater();
            img_seperater_left.Type = HudAnchor.左;
            img_seperater_left.Line = hm_CreateAnchorMark(obj_SafeFrame_Seperaters.transform, "seperater_left");

            Safe_Seperater img_seperater_right = new Safe_Seperater();
            img_seperater_right.Type = HudAnchor.右;
            img_seperater_right.Line = hm_CreateAnchorMark(obj_SafeFrame_Seperaters.transform, "seperater_right");

            List<Safe_Seperater> seperaters = new List<Safe_Seperater>();
            seperaters.Add(img_seperater_up);
            seperaters.Add(img_seperater_down);
            seperaters.Add(img_seperater_left);
            seperaters.Add(img_seperater_right);

            Safe_Seperater = new Safe_Seperater[seperaters.Count];
            for (int i = 0; i < seperaters.Count; i++)
            {
                Safe_Seperater[i] = new Hud.Safe_Seperater();
                Safe_Seperater[i].Line = seperaters[i].Line;
                Safe_Seperater[i].Type = seperaters[i].Type;
            }

            Safe_Frame = obj_SafeFrame_Seperaters.transform;
        }

        #endregion

        #region 销毁

        /// <summary>
        /// 锚点可视化 - 销毁
        /// </summary>
        public void hm_AnchorMarks_Destroy()
        {
            if (Safe_Frame == null)
                return;
            if (Anchors_Layout_Screen == null || Anchors_Layout_Screen.Count <= 0)
                return;

            for (int i = 0; i < Anchors_Layout_Screen.Count; i++)
            {
                if (Anchors_Layout_Screen[i].Mark != null)
                {
                    DestroyImmediate(Anchors_Layout_Screen[i].Mark.gameObject);
                    Anchors_Layout_Screen[i].Mark = null;
                }
            }

            Safe_CenterMarks.Edge.Clear();
            Safe_CenterMarks.Center = null;

            if (Safe_FrameLine.Length <= 0)
                return;
            for (int i = 0; i < Safe_FrameLine.Length; i++)
            {
                if (Safe_FrameLine[i].Frame != null)
                {
                    DestroyImmediate(Safe_FrameLine[i].Frame.gameObject, true);
                }
            }
            Safe_FrameLine = null;

            if (Safe_Seperater.Length <= 0)
                return;
            for (int i = 0; i < Safe_Seperater.Length; i++)
            {
                if (Safe_Seperater[i].Line != null)
                {
                    DestroyImmediate(Safe_Seperater[i].Line.gameObject, true);
                }
            }
            Safe_Seperater = null;

            if (Safe_Frame != null)
            {
                DestroyImmediate(Safe_Frame.gameObject, true);
                Safe_Frame = null;
            }

        }

        #endregion

        #endregion

        #region 创建辅助

        /// <summary>
        /// Hud Manager管理器消息 - 收集所有生成的HudElementItem元素 - 屏幕
        /// </summary>
        /// <param tweenName="structs"></param>
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
        /// Hud Manager管理器消息 - 收集所有生成的HudElementItem元素 - 世界
        /// </summary>
        /// <param tweenName="structs"></param>
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
        /// Hud Manager管理器消息 - 实例化方式生成Hud元素
        /// </summary>
        /// <param tweenName="ModuleName">模块名称</param>
        /// <param tweenName="Parent">父物体</param>
        /// <param tweenName="缩放_Scale">缩放尺寸</param>
        /// <returns>返回一个生成的HUD元素</returns>
        private Hud_Element hm_HudElement_Create(string LibraryName, string ModuleName)
        {
            Hud_Element element = null;

            for (int i = 0; i < Hud_ElementLibrarys.Count; i++)
            {
                if (LibraryName == Hud_ElementLibrarys[i].LibraryName)
                {
                    Hud_ElementLibrary lib = Hud_ElementLibrarys[i];
                    for (int s = 0; s < lib.ElementLibrary.Count; s++)
                    {
                        Library_Item item = lib.ElementLibrary[s];
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
        /// Hud Manager管理器消息 - 匹配锚点类型
        /// </summary>
        /// <param tweenName="type">锚点类型</param>
        /// <returns>返回一个锚点布局</returns>
        public Anchor_Layout hm_HudElement_MatchType(HudAnchor type)
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
        /// Hud Manager管理器消息 - 清理已存在的Element项 - 屏幕
        /// </summary>
        /// <param tweenName="element">目标元素</param>
        public void hm_HudElement_CleanAnchor_Screen(Hud_Element element)
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
        /// Hud Manager管理器消息 - 清理已存在的Element项 - 世界
        /// </summary>
        /// <param tweenName="element">目标元素</param>
        public void hm_HudElement_CleanAnchor_World(Hud_Element element)
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
        /// Hud Manager管理器消息 - 设置父物体
        /// </summary>
        /// <param tweenName="element">目标元素</param>
        /// <param tweenName="anchor">锚点父物体</param>
        public void hm_HudElement_ParentSetup(Hud_Element element, RectTransform anchor)
        {
            element.RectTransform.SetParent(anchor);
        }

        /// <summary>
        /// Hud Manager管理器消息 - 预存储到锚点列表 - 屏幕
        /// </summary>
        /// <param tweenName="anchor_struct">锚点根节点</param>
        /// <param tweenName="element">元素</param>
        /// <param tweenName="module_name">模块名称</param>
        /// <param tweenName="indicator_name">标识名称</param>
        /// <returns></returns>
        private HudElementNode hm_HudElement_Send_To_AnchorList_Screen(Anchor_Layout anchor_struct, Hud_Element element, string module_name, string indicator_name)
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
        /// Hud Manager管理器消息 - 预存储到锚点列表 - 世界
        /// </summary>
        /// <param tweenName="anchor_struct">锚点根节点</param>
        /// <param tweenName="element">元素</param>
        /// <param tweenName="module_name">模块名称</param>
        /// <param tweenName="indicator_name">标识名称</param>
        /// <returns></returns>
        private HudElementNode hm_HudElement_Send_To_AnchorList_World(Hud_Element element, string module_name, string indicator_name)
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
        /// Hud Manager管理器消息 - 获取目标元素身上的分辨率匹配方案的标识名称的信息
        /// </summary>
        /// <param tweenName="element">目标元素</param>
        /// <param tweenName="solutionName">分辨率匹配方案的标识名称</param>
        /// <returns></returns>
        private Anchor_Layout hm_GetCurrentSolutionLayout(Hud_Element element, string solutionName)
        {
            Anchor_Layout layout = null;
            for (int i = 0; i < element.RMS_InfoList.Count; i++)
            {
                if (element.RMS_InfoList[i].LayoutName == solutionName)
                {
                    layout = hm_HudElement_MatchType(element.RMS_InfoList[i].Anchor);
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
        public Hud_ElementLibrary[] hm_ElementLibrary_GetArray()
        {
            return Hud_ElementLibrarys.ToArray();
        }

        /// <summary>
        /// 获取所有元素库
        /// </summary>
        /// <returns></returns>
        public List<Hud_ElementLibrary> hm_ElementLibrary_GetList()
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
        public Hud_ElementLibrary hm_ElementLibrary_GetTargetLibrary(string name)
        {
            Hud_ElementLibrary lib = null;

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
        /// <param tweenName="LibName">目标元素库</param>
        public Hud_ElementLibrary hm_ElementLibrary_GetFirstLibrary()
        {
            Hud_ElementLibrary res_lib = null;
            if (Hud_ElementLibrarys != null && Hud_ElementLibrarys.Count > 0)
                res_lib = Hud_ElementLibrarys[0];
            return res_lib;
        }

        /// <summary>
        /// 更新库里元素的使用状态
        /// </summary>
        private void hm_ElementLibrary_UpdateStates()
        {
            if (Hud_ElementLibrarys == null || Hud_ElementLibrarys.Count <= 0)
                return;

            #region 更新元素是否正在在被使用的状态
            for (int i = 0; i < Hud_ElementLibrarys.Count; i++)
            {
                Hud_ElementLibrary lib = Hud_ElementLibrarys[i];
                for (int s = 0; s < lib.ElementLibrary.Count; s++)
                {
                    Library_Item item = lib.ElementLibrary[s];
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
        /// 元素库初始化 / 预加载元素物体
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
                Hud_ElementLibrary Lib = Hud_ElementLibrarys[v];

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
                    Library_Item item = Lib.ElementLibrary[i];

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
                        Hud_Element element = Instantiate(item.Target, Vector3.zero, Quaternion.identity, item.Root);
                        ///---保存原始名称
                        element.OriginalName = element.transform.name.Substring(0, element.transform.name.Length - 7);
                        ///---改名
                        element.transform.name = element.OriginalName + "_Clone_" + s;
                        ///---赋值源库名
                        element.OriginPoolName = Lib.LibraryName;


                        Library_Element pw = new Library_Element();
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
        /// <param tweenName="ElementName">元素标识名称</param>
        /// <returns></returns>
        public Hud_Element hm_ElementLibrary_Spawn(string ElementName)
        {
            Hud_Element element = null;

            for (int i = 0; i < Hud_ElementLibrarys.Count; i++)
            {
                Hud_ElementLibrary lib = Hud_ElementLibrarys[i];

                for (int k = 0; k < lib.ElementLibrary.Count; k++)
                {
                    Library_Item item = lib.ElementLibrary[k];

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
        /// <param tweenName="Element">目标元素</param>
        public void hm_ElementLibrary_Despawn(Hud_Element Element)
        {
            for (int i = 0; i < Hud_ElementLibrarys.Count; i++)
            {
                Hud_ElementLibrary lib = Hud_ElementLibrarys[i];

                for (int m = 0; m < lib.ElementLibrary.Count; m++)
                {
                    Library_Item item = lib.ElementLibrary[m];

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
                Hud_ElementLibrary lib = Hud_ElementLibrarys[v];
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
                        lib.ElementLibrary[i].PreloadElements = new List<Library_Element>();

                    lib.ElementLibrary[i].PreloadElements.Clear();
                }
                #endregion
            }
        }

        #region 创建元素库
        /// <summary>
        /// 创建元素库的项
        /// </summary>
        /// <param tweenName="Element">目标元素</param>
        /// <param tweenName="InitialCount">初始化数量</param>
        /// <returns></returns>
        public Library_Item hm_ElementLibrary_CreateItem(Hud_Element Element, int InitialCount)
        {
            Library_Item item = new Library_Item();
            item.Target = Element;
            item.Name = string.IsNullOrEmpty(Element.transform.name) ? "NewElement" : Element.transform.name;
            item.InitializeCount = InitialCount;

            if (item.PreloadElements == null)
                item.PreloadElements = new List<Library_Element>();

            return item;
        }

        /// <summary>
        /// 动态创建元素库
        /// </summary>
        /// <param tweenName="LibraryName">元素库名称</param>
        /// <param tweenName="Items">元素库项数组</param>
        /// <returns>返回元素库</returns>
        public Hud_ElementLibrary hm_ElementLibrary_AddItem(string LibraryName, Library_Item[] Items)
        {
            Hud_ElementLibrary lib = ScriptableObject.CreateInstance<Hud_ElementLibrary>();
            lib.name = "RunTimeLib";
            lib.LibraryName = LibraryName;
            for (int i = 0; i < Items.Length; i++)
            {
                if (lib.ElementLibrary == null)
                    lib.ElementLibrary = new List<Library_Item>();

                lib.ElementLibrary.Add(Items[i]);
            }
            return lib;
        }
        #endregion

        #region 元素库实例化 / 与销毁
        /// <summary>
        /// 实例化目标元素库
        /// </summary>
        /// <param tweenName="Library">目标元素库</param>
        public void hm_ElementLibrary_Initialize(Hud_ElementLibrary Library)
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

            List<Library_Item> items = Library.ElementLibrary;

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
                    Hud_Element element = Instantiate(items[i].Target, Vector3.zero, Quaternion.identity, items[i].Root);
                    ///---保存原始名称
                    element.OriginalName = element.transform.name.Substring(0, element.transform.name.Length - 7);
                    ///---改名
                    element.transform.name = element.OriginalName + "_Clone_" + s;

                    Library_Element pw = new Library_Element();
                    pw.HudElement = element;
                    pw.HudElement.CreateState = HudElementCreateState.Recycled;
                    pw.HudElement.element_Reset();
                    pw.HudElement.gameObject.SetActive(false);

                    if (items[i].PreloadElements == null)
                        items[i].PreloadElements = new List<Library_Element>();

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
        /// <param tweenName="LibName">目标元素库</param>
        public void hm_ElementLibrary_Destroyed(string LibName)
        {
            for (int i = 0; i < Hud_ElementLibrarys.Count; i++)
            {
                Hud_ElementLibrary lib = Hud_ElementLibrarys[i];
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
        /// Hud Manager管理器消息 - 创建的元素的初始化设置 - 屏幕模式
        /// </summary>
        /// <param tweenName="element">目标元素</param>
        /// <param tweenName="anchor">锚点类型</param>
        /// <param tweenName="alpha">透明度_Alpha</param>
        /// <param tweenName="offset">位置偏移</param>
        public void hm_HudElement_Initialize_For_Screen(Hud_Element element, HudAnchor anchor, float alpha, Vector3 offset, Vector3 scale, Vector2 size)
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
        /// Hud Manager管理器消息 - 创建的元素的初始化设置 - 屏幕模式 (依据元素自身设计布局信息)
        /// </summary>
        /// <param tweenName="element">目标元素</param>
        /// <param tweenName="alpha">透明度_Alpha</param>
        /// <param tweenName="offset">位置偏移</param>
        /// <param tweenName="DontCreateID">是否为自身生成随机ID</param>
        public void hm_HudElement_Initialize_ByDesignLayout_For_Screen(Hud_Element element, float alpha, Vector3 offset, string rms_name, bool DontCreateID = false)
        {
            bool IsExist = false;
            for (int i = 0; i < element.RMS_InfoList.Count; i++)
            {
                if (element.RMS_InfoList[i].LayoutName == rms_name)
                {
                    OriginalLayoutInfo info = element.RMS_InfoList[i];
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
                        util_Tools.Func_PrintInfo("Hud Manager管理器消息", "已将元素生成到指定设计布局！", HudMsgState.通知);
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
                    util_Tools.Func_PrintInfo("Hud Manager管理器消息", "未找到指定标识名称的设计布局，请检查该元素是否有记录设计布局信息！", HudMsgState.错误);
            }
        }

        /// <summary>
        /// Hud Manager管理器消息 - 创建的元素的初始化设置 - 世界模式
        /// </summary>
        /// <param tweenName="element">目标元素</param>
        /// <param tweenName="pivot">锚点类型</param>
        /// <param tweenName="alpha">透明度_Alpha</param>
        /// <param tweenName="offset">位置偏移</param>
        /// <param tweenName="position">位置_Position</param>
        /// <param tweenName="rotation">旋转_Rotation</param>
        public void hm_HudElement_Initialize_For_World(Hud_Element element, Vector2 size, float alpha, Vector3 offset, Vector3 position, Quaternion rotation, Vector3 scale)
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
        /// Hud Manager管理器消息 - 创建一个Hud元素 - 屏幕空间
        /// </summary>
        /// <param tweenName="libname">目标库名称</param>
        /// <param tweenName="indicator">从库中取出后的自定义名称（仅为调用者自己理解的自定义名称）</param>
        /// <param tweenName="modulename">预存入元素池的目标名称</param>
        /// <param tweenName="offset">元素偏移</param>
        /// <param tweenName="scale">元素缩放</param>
        /// <param tweenName="size">元素尺寸</param>
        /// <param tweenName="rms">RMS系统是否开启？</param>
        /// <param tweenName="rms_name">RMS系统方案名称</param>
        /// <param tweenName="args_creator">元素入场动画参数</param>
        /// <param tweenName="action_in_start">元素入场开始委托</param>
        /// <param tweenName="action_in_progress">元素入场进度委托</param>
        /// <param tweenName="action_in_end">元素入场结束委托</param>        
        /// <param tweenName="action_out_start">元素退场前委托</param>
        /// <param tweenName="action_out_progress">元素退场进度委托</param>
        /// <param tweenName="action_out_end">元素退场后委托</param>     
        /// <param tweenName="autoin">此值是个非常关键的开关，如果你为一个元素编写了一个自定义控制的脚本绑定在它身上，并希望生成出来的时候由您自己决定何时播放动画，那么此值必须为False</param>
        /// <returns>返回一个HudElement节点元素体</returns>
        public HudElementNode hm_HudElement_Create_Screen(
            string libname, string indicator, string modulename,
            Vector3 offset = default(Vector3), Vector3 scale = default(Vector3), Vector2 size = default,
            bool rms = false, string rms_name = "",
            Motion_Creator args_creator = null,
            UnityAction<Hud_Element> action_in_start = null,
            UnityAction<float> action_in_progress = null,
            UnityAction<Hud_Element> action_in_end = null,
            UnityAction<Hud_Element> action_out_start = null,
            UnityAction<float> action_out_progress = null,
            UnityAction<Hud_Element> action_out_end = null,
            bool autoin = true)
        {
            #region 从元素库中取出元素
            Hud_Element element = hm_HudElement_Create(libname, modulename);
            if (element == null)
            {
                Debug.Log("HudElement生成警告：您从元素池获取的目标元素为空！请检查该元素在元素池中的状态！");
                return null;
            }
            #endregion

            #region 是否开启RMS模式
            element.element_RMSMode_Enabled(rms);
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
        /// Hud Manager管理器消息 - 创建一个Hud元素 - 世界空间
        /// </summary>
        /// <param tweenName="libname">目标元素库</param>
        /// <param tweenName="indicator">目标标识名称</param>
        /// <param tweenName="modulename">模块名称</param>
        /// <param tweenName="size">锚点</param>
        /// <param tweenName="position">位置_Position</param>
        /// <param tweenName="rotation">旋转_Rotation</param>
        /// <param tweenName="scale">缩放_Scale</param>
        /// <param tweenName="offset">偏移</param>
        /// <param tweenName="args_creator">元素动效参数 - 创建</param>
        /// <param tweenName="action_in_start">委托-进入时</param>
        /// <param tweenName="action_in_progress">委托-进入进度</param>
        /// <param tweenName="action_in_end">委托-进入后</param>
        /// <param tweenName="action_out_start">委托-退出时</param>
        /// <param tweenName="action_out_progress">委托-退出进度</param>
        /// <param tweenName="action_out_end">委托-退出后</param>
        /// <param tweenName="autoin">元素自动执行ElementIn</param>
        /// <returns>返回一个HudElement节点元素体</returns>
        public HudElementNode hm_HudElement_Create_World(
            string libname, string indicator, string modulename,
            Vector2 size, Vector3 position, Vector3 rotation, Vector3 scale, Vector3 offset,
            Motion_Creator args_creator = null,
            UnityAction<Hud_Element> action_in_start = null,
            UnityAction<float> action_in_progress = null,
            UnityAction<Hud_Element> action_in_end = null,
            UnityAction<Hud_Element> action_out_start = null,
            UnityAction<float> action_out_progress = null,
            UnityAction<Hud_Element> action_out_end = null,
            bool autoin = true)
        {
            #region 从元素库中取出元素
            Hud_Element element = hm_HudElement_Create(libname, modulename);
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
        /// Hud Manager管理器消息 - 清空所有生成的Hud元素
        /// </summary>
        /// <param tweenName="args">回收参数</param>
        /// <param tweenName="action_out_start">委托 - 回收时</param>
        /// <param tweenName="action_out_end">委托 - 回收后</param>
        public void hm_HudElement_RecycleAll(Motion_Recycler args, UnityAction<Hud_Element> action_out_start = null, UnityAction<float> action_out_progress = null, UnityAction<Hud_Element> action_out_end = null)
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
        /// Hud Manager管理器消息 - 清理目标ID的Hud元素
        /// </summary>
        /// <param tweenName="id">目标ID</param>
        /// <param tweenName="args">回收参数</param>
        /// <param tweenName="action_out_start">委托 - 回收时</param>
        /// <param tweenName="action_out_end">委托 - 回收后</param>
        public void hm_HudElement_RecycleAt(int id, Motion_Recycler args, UnityAction<Hud_Element> action_out_start = null, UnityAction<float> action_out_progress = null, UnityAction<Hud_Element> action_out_end = null)
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

                            node.Element.act_on_element_out_end += (Hud_Element ele) =>
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

                        node.Element.act_on_element_out_end += (Hud_Element ele) =>
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
                    util_Tools.Func_PrintInfo("Hud Manager管理器消息", "Hud元素ID：" + id + " 不存在！未找到要回收的目标元素！", HudMsgState.通知);
            }
            else
            {
                if (UseDebug)
                    util_Tools.Func_PrintInfo("Hud Manager管理器消息", "已回收Hud元素：" + x_name + " / " + x_id, HudMsgState.通知);
            }
            #endregion
        }

        /// <summary>
        /// Hud Manager管理器消息 - 清理目标ID的Hud元素
        /// </summary>
        /// <param tweenName="element">目标名称</param>
        /// <param tweenName="args">回收参数</param>
        /// <param tweenName="action_out_start">委托 - 回收时</param>
        /// <param tweenName="action_out_end">委托 - 回收后</param>
        public void hm_HudElement_RecycleAt(Hud_Element element, Motion_Recycler args, UnityAction<Hud_Element> action_out_start = null, UnityAction<float> action_out_progress = null, UnityAction<Hud_Element> action_out_end = null)
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
                    util_Tools.Func_PrintInfo("Hud Manager管理器消息", "Hud元素名称：" + element + " 不存在！未找到要回收的目标元素！", HudMsgState.通知);
            }
            else
            {
                if (UseDebug)
                    util_Tools.Func_PrintInfo("Hud Manager管理器消息", "已回收Hud元素：" + x_indicator + " / " + x_id, HudMsgState.通知);
            }
            #endregion
        }

        #endregion

        #region 获取

        /// <summary>
        /// Hud Manager管理器消息 - 根据目标ID从锚点列表中获取生成的Hud元素
        /// </summary>
        /// <param tweenName="id">目标ID的元素</param>
        /// <returns>根据目标ID获取的元素</returns>
        public Hud_Element hm_HudElement_Get(int id)
        {
            Hud_Element element = null;
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
        ///  Hud Manager管理器消息 - 根据目标名称从锚点列表中获取生成的Hud元素
        /// </summary>
        /// <param tweenName="name"></param>
        /// <returns>根据目标名称获取的元素</returns>
        public Hud_Element hm_HudElement_Get(string name)
        {
            Hud_Element element = null;
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
        ///  Hud Manager管理器消息 - 从锚点列表中获取所有已生成的Hud元素
        /// </summary>
        /// <returns>所有已生成到锚点里的Hud元素</returns>
        public Hud_Element[] hm_HudElement_GetAll()
        {
            List<Hud_Element> list = new List<Hud_Element>();
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
        /// Hud Manager管理器消息 - 获取已生成到锚点里的总Element数量
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

            Hud_Manager man = FindFirstObjectByType<Hud_Manager>();

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
        public Hud_Element[] hm_AnchorElementsGet()
        {
            Hud_Manager mgr = FindFirstObjectByType<Hud_Manager>();
            Hud_Element[] trans = mgr.HudCanvas_ScreenAnchor.GetComponentsInChildren<Hud_Element>();
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

        #region 透明度

        /// <summary>
        /// 内容透明度更新
        /// </summary>
        public void hm_ContentAlpha_Update()
        {
            if (HudCanvasGroup_Screen != null)
            {
                HudCanvasGroup_Screen.alpha = ContentAlpha_Screen;
                if (ContentAlpha_Screen >= 1)
                {
                    if (OpacityIsChangedToMax_Screen)
                    {
                        OpacityIsChangedToMax_Screen = false;
                        if (Act_ContentOpacity_Screen_IsMax != null)
                            Act_ContentOpacity_Screen_IsMax(ContentAlpha_Screen);
                    }
                }
                else if (ContentAlpha_Screen <= 0)
                {
                    if (OpacityIsChangedToMin_Screen)
                    {
                        OpacityIsChangedToMin_Screen = false;
                        if (Act_ContentOpacity_Screen_IsMin != null)
                            Act_ContentOpacity_Screen_IsMin(ContentAlpha_Screen);
                    }
                }
                else
                {
                    OpacityIsChangedToMax_Screen = true;
                    OpacityIsChangedToMin_Screen = true;
                    if (Act_ContentOpacity_Screen_Changed != null)
                        Act_ContentOpacity_Screen_Changed(ContentAlpha_Screen);
                }
            }


            if (HudCanvasGroup_World != null)
            {
                HudCanvasGroup_World.alpha = ContentAlpha_World;
                if (ContentAlpha_World >= 1)
                {
                    if (OpacityIsChangedToMax_World)
                    {
                        OpacityIsChangedToMax_World = false;
                        if (Act_ContentOpacity_World_IsMax != null)
                            Act_ContentOpacity_World_IsMax(ContentAlpha_World);
                    }
                }
                else if (ContentAlpha_World <= 0)
                {
                    if (OpacityIsChangedToMin_World)
                    {
                        OpacityIsChangedToMin_World = false;
                        if (Act_ContentOpacity_World_IsMin != null)
                            Act_ContentOpacity_World_IsMin(ContentAlpha_World);
                    }
                }
                else
                {
                    OpacityIsChangedToMax_World = true;
                    OpacityIsChangedToMin_World = true;
                    if (Act_ContentOpacity_World_Changed != null)
                        Act_ContentOpacity_World_Changed(ContentAlpha_World);
                }
            }
        }

        /// <summary>
        /// 屏幕内容透明度平滑到
        /// </summary>
        /// <param tweenName="val">目标透明度</param>
        /// <param tweenName="dur">耗时</param>
        /// <param tweenName="ease">缓动</param>
        /// <param tweenName="delay">延迟</param>
        /// <param tweenName="action_start">委托 - 开始时</param>
        /// <param tweenName="action_end">委托 - 结束时</param>
        public void hm_Screen_ContentAlpha_To(float val, float dur, Ease ease, float delay, UnityAction action_start = null, UnityAction action_end = null)
        {
            if (twn_ContentAlpha_Screen != null && twn_ContentAlpha_Screen.active)
                if (twn_ContentAlpha_Screen.IsPlaying())
                    twn_ContentAlpha_Screen.Kill();
            twn_ContentAlpha_Screen = DOTween.To(() => ContentAlpha_Screen, x => ContentAlpha_Screen = x, val, dur).SetEase(ease).SetAutoKill(true).SetDelay(delay).OnStart(() =>
            {
                if (action_start != null)
                    action_start();
            }).OnComplete(() =>
            {
                if (action_end != null)
                    action_end();
            });
        }

        /// <summary>
        /// 屏幕内容透明度平滑到
        /// </summary>
        /// <param tweenName="val">目标透明度</param>
        /// <param tweenName="dur">耗时</param>
        /// <param tweenName="ease">缓动</param>
        /// <param tweenName="delay">延迟</param>
        /// <param tweenName="action_start">委托 - 开始时</param>
        /// <param tweenName="action_end">委托 - 结束时</param>
        public void hm_Screen_ContentAlpha_To(float val, float dur, AnimationCurve ease, float delay, UnityAction action_start = null, UnityAction action_end = null)
        {
            if (twn_ContentAlpha_Screen != null && twn_ContentAlpha_Screen.active)
                if (twn_ContentAlpha_Screen.IsPlaying())
                    twn_ContentAlpha_Screen.Kill();
            twn_ContentAlpha_Screen = DOTween.To(() => ContentAlpha_Screen, x => ContentAlpha_Screen = x, val, dur).SetEase(ease).SetAutoKill(true).SetDelay(delay).OnStart(() =>
            {
                if (action_start != null)
                    action_start();
            }).OnComplete(() =>
            {
                if (action_end != null)
                    action_end();
            });
        }

        /// <summary>
        /// 屏幕内容透明度快速到
        /// </summary>
        /// <param tweenName="val">目标透明度</param>
        public void hm_Screen_ContentAlpha_FastTo(float val)
        {
            if (twn_ContentAlpha_Screen != null && twn_ContentAlpha_Screen.active)
                if (twn_ContentAlpha_Screen.IsPlaying())
                    twn_ContentAlpha_Screen.Kill();
            ContentAlpha_Screen = val;
        }

        /// <summary>
        /// 世界内容透明度平滑到
        /// </summary>
        /// <param tweenName="val">目标透明度</param>
        /// <param tweenName="dur">耗时</param>
        /// <param tweenName="ease">缓动</param>
        /// <param tweenName="delay">延迟</param>
        /// <param tweenName="action_start">委托 - 开始时</param>
        /// <param tweenName="action_end">委托 - 结束时</param>
        public void hm_World_ContentAlpha_To(float val, float dur, Ease ease, float delay, UnityAction action_start = null, UnityAction action_end = null)
        {
            if (twn_ContentAlpha_World != null && twn_ContentAlpha_World.active)
                if (twn_ContentAlpha_World.IsPlaying())
                    twn_ContentAlpha_World.Kill();
            twn_ContentAlpha_World = DOTween.To(() => ContentAlpha_World, x => ContentAlpha_World = x, val, dur).SetEase(ease).SetAutoKill(true).SetDelay(delay).OnStart(() =>
            {
                if (action_start != null)
                    action_start();
            }).OnComplete(() =>
            {
                if (action_end != null)
                    action_end();
            });
        }

        /// <summary>
        /// 世界内容透明度平滑到
        /// </summary>
        /// <param tweenName="val">目标透明度</param>
        /// <param tweenName="dur">耗时</param>
        /// <param tweenName="ease">缓动</param>
        /// <param tweenName="delay">延迟</param>
        /// <param tweenName="action_start">委托 - 开始时</param>
        /// <param tweenName="action_end">委托 - 结束时</param>
        public void hm_World_ContentAlpha_To(float val, float dur, AnimationCurve ease, float delay, UnityAction action_start = null, UnityAction action_end = null)
        {
            if (twn_ContentAlpha_World != null && twn_ContentAlpha_World.active)
                if (twn_ContentAlpha_World.IsPlaying())
                    twn_ContentAlpha_World.Kill();
            twn_ContentAlpha_World = DOTween.To(() => ContentAlpha_World, x => ContentAlpha_World = x, val, dur).SetEase(ease).SetAutoKill(true).SetDelay(delay).OnStart(() =>
            {
                if (action_start != null)
                    action_start();
            }).OnComplete(() =>
            {
                if (action_end != null)
                    action_end();
            });
        }

        /// <summary>
        /// 世界内容透明度快速到
        /// </summary>
        /// <param tweenName="val">目标透明度</param>
        public void hm_World_ContentAlpha_FastTo(float val)
        {
            if (twn_ContentAlpha_World != null && twn_ContentAlpha_World.active)
                if (twn_ContentAlpha_World.IsPlaying())
                    twn_ContentAlpha_World.Kill();
            ContentAlpha_World = val;
        }

        #endregion

        #region 遮罩

        /// <summary>
        /// 遮罩更新
        /// </summary>
        public void hm_MaskUpdate()
        {
            if (Mask == null)
                return;

            MaskColor.a = MaskAlpha;
            if (Mask != null)
                Mask.color = MaskColor;
            if (MaskTexture != null)
                hm_MaskTextureSet(MaskTexture);
            else
                hm_MaskTextureSet(null);

            if (MaskAlpha < MaskRaycastAlphaThreshold)
            {
                MaskRaycastEnabled = false;
            }
            else
            {
                MaskRaycastEnabled = true;
            }
            Mask.raycastTarget = MaskRaycastEnabled;
            hm_MaskTopView();

            if (Act_MaskChanged_Value != null)
                Act_MaskChanged_Value(MaskAlpha);
        }

        /// <summary>
        /// 将Mask置于最上层
        /// </summary>
        public void hm_MaskTopView()
        {
            if (Mask == null)
                return;
            Mask.transform.SetAsLastSibling();
        }

        /// <summary>
        /// 遮罩颜色平滑到
        /// </summary>
        /// <param tweenName="col">目标颜色</param>
        /// <param tweenName="dur">耗时</param>
        /// <param tweenName="ease">缓动</param>
        /// <param tweenName="delay">延迟</param>
        public void hm_MaskColor_To(Color col, float dur, Ease ease, float delay)
        {
            if (twn_MaskColor_R != null && twn_MaskColor_R.active)
                if (twn_MaskColor_R.IsPlaying())
                    twn_MaskColor_R.Kill();
            if (twn_MaskColor_G != null && twn_MaskColor_G.active)
                if (twn_MaskColor_G.IsPlaying())
                    twn_MaskColor_G.Kill();
            if (twn_MaskColor_B != null && twn_MaskColor_B.active)
                if (twn_MaskColor_B.IsPlaying())
                    twn_MaskColor_B.Kill();

            twn_MaskColor_R = DOTween.To(() => MaskColor.r, r => MaskColor.r = r, col.r, dur).SetEase(ease).SetAutoKill(true).SetDelay(delay);
            twn_MaskColor_G = DOTween.To(() => MaskColor.g, g => MaskColor.g = g, col.g, dur).SetEase(ease).SetAutoKill(true).SetDelay(delay);
            twn_MaskColor_B = DOTween.To(() => MaskColor.b, b => MaskColor.b = b, col.b, dur).SetEase(ease).SetAutoKill(true).SetDelay(delay);
        }

        /// <summary>
        /// 遮罩颜色平滑到
        /// </summary>
        /// <param tweenName="col">目标颜色</param>
        /// <param tweenName="dur">耗时</param>
        /// <param tweenName="ease">曲线</param>
        /// <param tweenName="delay">延迟</param>
        public void hm_MaskColor_To(Color col, float dur, AnimationCurve ease, float delay)
        {
            if (twn_MaskColor_R != null && twn_MaskColor_R.active)
                if (twn_MaskColor_R.IsPlaying())
                    twn_MaskColor_R.Kill();
            if (twn_MaskColor_G != null && twn_MaskColor_G.active)
                if (twn_MaskColor_G.IsPlaying())
                    twn_MaskColor_G.Kill();
            if (twn_MaskColor_B != null && twn_MaskColor_B.active)
                if (twn_MaskColor_B.IsPlaying())
                    twn_MaskColor_B.Kill();

            twn_MaskColor_R = DOTween.To(() => MaskColor.r, r => MaskColor.r = r, col.r, dur).SetEase(ease).SetAutoKill(true).SetDelay(delay);
            twn_MaskColor_G = DOTween.To(() => MaskColor.g, g => MaskColor.g = g, col.g, dur).SetEase(ease).SetAutoKill(true).SetDelay(delay);
            twn_MaskColor_B = DOTween.To(() => MaskColor.b, b => MaskColor.b = b, col.b, dur).SetEase(ease).SetAutoKill(true).SetDelay(delay);
        }

        /// <summary>
        /// 遮罩颜色快速到
        /// </summary>
        /// <param tweenName="col">目标颜色</param>
        public void hm_MaskColor_FastTo(Color col)
        {
            if (twn_MaskColor_R != null && twn_MaskColor_R.active)
                if (twn_MaskColor_R.IsPlaying())
                    twn_MaskColor_R.Kill();

            if (twn_MaskColor_G != null && twn_MaskColor_G.active)
                if (twn_MaskColor_G.IsPlaying())
                    twn_MaskColor_G.Kill();

            if (twn_MaskColor_B != null && twn_MaskColor_B.active)
                if (twn_MaskColor_B.IsPlaying())
                    twn_MaskColor_B.Kill();

            MaskColor.r = col.r;
            MaskColor.g = col.g;
            MaskColor.b = col.b;
        }

        /// <summary>
        /// 遮罩透明度平滑到
        /// </summary>
        /// <param tweenName="val">目标颜色</param>
        /// <param tweenName="dur">耗时</param>
        /// <param tweenName="ease">缓动</param>
        /// <param tweenName="delay">延迟</param>
        /// <param tweenName="action_start">委托 - 开始时</param>
        /// <param tweenName="action_end">委托 - 结束时</param>
        public void hm_MaskAlpha_To(float val, float dur, Ease ease, float delay, UnityAction action_start = null, UnityAction action_end = null)
        {
            if (twn_MaskAlpha != null && twn_MaskAlpha.active)
                if (twn_MaskAlpha.IsPlaying())
                    twn_MaskAlpha.Kill();
            twn_MaskAlpha = DOTween.To(() => MaskAlpha, x => MaskAlpha = x, val, dur).SetEase(ease).SetAutoKill(true).SetDelay(delay).OnStart(() =>
            {
                if (action_start != null)
                    action_start();
            }).OnComplete(() =>
            {
                if (action_end != null)
                    action_end();
            });
        }

        /// <summary>
        /// 遮罩透明度平滑到
        /// </summary>
        /// <param tweenName="val">目标颜色</param>
        /// <param tweenName="dur">耗时</param>
        /// <param tweenName="ease">曲线</param>
        /// <param tweenName="delay">延迟</param>
        /// <param tweenName="action_start">委托 - 开始时</param>
        /// <param tweenName="action_end">委托 - 结束时</param>
        public void hm_MaskAlpha_To(float val, float dur, AnimationCurve ease, float delay, UnityAction action_start = null, UnityAction action_end = null)
        {
            if (twn_MaskAlpha != null && twn_MaskAlpha.active)
                if (twn_MaskAlpha.IsPlaying())
                    twn_MaskAlpha.Kill();
            twn_MaskAlpha = DOTween.To(() => MaskAlpha, x => MaskAlpha = x, val, dur).SetEase(ease).SetAutoKill(true).SetDelay(delay).OnStart(() =>
            {
                if (action_start != null)
                    action_start();
            }).OnComplete(() =>
            {
                if (action_end != null)
                    action_end();
            });
        }

        /// <summary>
        /// 遮罩透明度快速到
        /// </summary>
        /// <param tweenName="val">目标透明度</param>
        public void hm_MaskAlpha_FastTo(float val)
        {
            if (twn_MaskAlpha != null && twn_MaskAlpha.active)
                if (twn_MaskAlpha.IsPlaying())
                    twn_MaskAlpha.Kill();
            MaskAlpha = val;
        }

        /// <summary>
        /// 设置遮罩贴图
        /// </summary>
        /// <param tweenName="tex">目标遮罩贴图</param>
        public void hm_MaskTextureSet(Texture2D tex)
        {
            if (tex == null)
            {
                if (Mask != null)
                    Mask.sprite = null;
                return;
            }
            Mask.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);
            if (Act_MaskChanged_Texture != null)
                Act_MaskChanged_Texture(tex);
        }

        #endregion

        #region 散焦遮罩

        /// <summary>
        /// 散焦遮罩更新
        /// </summary>
        public void hm_BlurMaskUpdate()
        {
            if (BlurMask == null)
                return;

            BlurMaskColor.a = BlurMaskAlpha;
            if (BlurMask != null)
                BlurMask.color = BlurMaskColor;
            if (BlurMaskTexture != null)
                hm_BlurMaskTextureSet(BlurMaskTexture);
            else
                hm_BlurMaskTextureSet(null);

            if (UniversalFeature_Blur_Intensity < BlurMaskRaycastAlphaThreshold)
            {
                BlurMaskRaycastEnabled = false;
            }
            else
            {
                BlurMaskRaycastEnabled = true;
            }
            BlurMask.raycastTarget = BlurMaskRaycastEnabled;
            hm_BlurMaskTopView();

            if (Act_BlurMaskChanged_Value != null)
                Act_BlurMaskChanged_Value(BlurMaskAlpha);
        }

        /// <summary>
        /// 将Mask置于最上层
        /// </summary>
        public void hm_BlurMaskTopView()
        {
            if (BlurMask == null)
                return;
            BlurMask.transform.SetAsFirstSibling();
        }

        /// <summary>
        /// 散焦遮罩颜色平滑到
        /// </summary>
        /// <param tweenName="col">目标颜色</param>
        /// <param tweenName="dur">耗时</param>
        /// <param tweenName="ease">缓动</param>
        /// <param tweenName="delay">延迟</param>
        public void hm_BlurMaskColor_To(Color col, float dur, Ease ease, float delay)
        {
            if (twn_BlurMaskColor_R != null && twn_BlurMaskColor_R.active)
                if (twn_BlurMaskColor_R.IsPlaying())
                    twn_BlurMaskColor_R.Kill();
            if (twn_BlurMaskColor_G != null && twn_BlurMaskColor_G.active)
                if (twn_BlurMaskColor_G.IsPlaying())
                    twn_BlurMaskColor_G.Kill();
            if (twn_BlurMaskColor_B != null && twn_BlurMaskColor_B.active)
                if (twn_BlurMaskColor_B.IsPlaying())
                    twn_BlurMaskColor_B.Kill();

            twn_BlurMaskColor_R = DOTween.To(() => BlurMaskColor.r, r => BlurMaskColor.r = r, col.r, dur).SetEase(ease).SetAutoKill(true).SetDelay(delay);
            twn_BlurMaskColor_G = DOTween.To(() => BlurMaskColor.g, g => BlurMaskColor.g = g, col.g, dur).SetEase(ease).SetAutoKill(true).SetDelay(delay);
            twn_BlurMaskColor_B = DOTween.To(() => BlurMaskColor.b, b => BlurMaskColor.b = b, col.b, dur).SetEase(ease).SetAutoKill(true).SetDelay(delay);
        }

        /// <summary>
        /// 散焦遮罩颜色平滑到
        /// </summary>
        /// <param tweenName="col">目标颜色</param>
        /// <param tweenName="dur">耗时</param>
        /// <param tweenName="ease">曲线</param>
        /// <param tweenName="delay">延迟</param>
        public void hm_BlurMaskColor_To(Color col, float dur, AnimationCurve ease, float delay)
        {
            if (twn_BlurMaskColor_R != null && twn_BlurMaskColor_R.active)
                if (twn_BlurMaskColor_R.IsPlaying())
                    twn_BlurMaskColor_R.Kill();
            if (twn_BlurMaskColor_G != null && twn_BlurMaskColor_G.active)
                if (twn_BlurMaskColor_G.IsPlaying())
                    twn_BlurMaskColor_G.Kill();
            if (twn_BlurMaskColor_B != null && twn_BlurMaskColor_B.active)
                if (twn_BlurMaskColor_B.IsPlaying())
                    twn_BlurMaskColor_B.Kill();

            twn_BlurMaskColor_R = DOTween.To(() => BlurMaskColor.r, r => BlurMaskColor.r = r, col.r, dur).SetEase(ease).SetAutoKill(true).SetDelay(delay);
            twn_BlurMaskColor_G = DOTween.To(() => BlurMaskColor.g, g => BlurMaskColor.g = g, col.g, dur).SetEase(ease).SetAutoKill(true).SetDelay(delay);
            twn_BlurMaskColor_B = DOTween.To(() => BlurMaskColor.b, b => BlurMaskColor.b = b, col.b, dur).SetEase(ease).SetAutoKill(true).SetDelay(delay);
        }

        /// <summary>
        /// 散焦遮罩颜色快速到
        /// </summary>
        /// <param tweenName="col">目标颜色</param>
        public void hm_BlurMaskColor_FastTo(Color col)
        {
            if (twn_BlurMaskColor_R != null && twn_BlurMaskColor_R.active)
                if (twn_BlurMaskColor_R.IsPlaying())
                    twn_BlurMaskColor_R.Kill();

            if (twn_BlurMaskColor_G != null && twn_BlurMaskColor_G.active)
                if (twn_BlurMaskColor_G.IsPlaying())
                    twn_BlurMaskColor_G.Kill();

            if (twn_BlurMaskColor_B != null && twn_BlurMaskColor_B.active)
                if (twn_BlurMaskColor_B.IsPlaying())
                    twn_BlurMaskColor_B.Kill();

            BlurMaskColor.r = col.r;
            BlurMaskColor.g = col.g;
            BlurMaskColor.b = col.b;
        }

        /// <summary>
        /// 散焦遮罩透明度平滑到
        /// </summary>
        /// <param tweenName="val">目标颜色</param>
        /// <param tweenName="dur">耗时</param>
        /// <param tweenName="ease">缓动</param>
        /// <param tweenName="delay">延迟</param>
        /// <param tweenName="action_start">委托 - 开始时</param>
        /// <param tweenName="action_end">委托 - 结束时</param>
        public void hm_BlurMaskAlpha_To(float val, float dur, Ease ease, float delay, UnityAction action_start = null, UnityAction action_end = null)
        {
            if (twn_BlurMaskAlpha != null && twn_BlurMaskAlpha.active)
                if (twn_BlurMaskAlpha.IsPlaying())
                    twn_BlurMaskAlpha.Kill();
            twn_BlurMaskAlpha = DOTween.To(() => BlurMaskAlpha, x => BlurMaskAlpha = x, val, dur).SetEase(ease).SetAutoKill(true).SetDelay(delay).OnStart(() =>
            {
                if (action_start != null)
                    action_start();
            }).OnComplete(() =>
            {
                if (action_end != null)
                    action_end();
            });
        }

        /// <summary>
        /// 散焦遮罩透明度平滑到
        /// </summary>
        /// <param tweenName="val">目标颜色</param>
        /// <param tweenName="dur">耗时</param>
        /// <param tweenName="ease">曲线</param>
        /// <param tweenName="delay">延迟</param>
        /// <param tweenName="action_start">委托 - 开始时</param>
        /// <param tweenName="action_end">委托 - 结束时</param>
        public void hm_BlurMaskAlpha_To(float val, float dur, AnimationCurve ease, float delay, UnityAction action_start = null, UnityAction action_end = null)
        {
            if (twn_BlurMaskAlpha != null && twn_BlurMaskAlpha.active)
                if (twn_BlurMaskAlpha.IsPlaying())
                    twn_BlurMaskAlpha.Kill();
            twn_BlurMaskAlpha = DOTween.To(() => BlurMaskAlpha, x => BlurMaskAlpha = x, val, dur).SetEase(ease).SetAutoKill(true).SetDelay(delay).OnStart(() =>
            {
                if (action_start != null)
                    action_start();
            }).OnComplete(() =>
            {
                if (action_end != null)
                    action_end();
            });
        }

        /// <summary>
        /// 散焦遮罩透明度快速到
        /// </summary>
        /// <param tweenName="val">目标透明度</param>
        public void hm_BlurMaskAlpha_FastTo(float val)
        {
            if (twn_BlurMaskAlpha != null && twn_BlurMaskAlpha.active)
                if (twn_BlurMaskAlpha.IsPlaying())
                    twn_BlurMaskAlpha.Kill();
            BlurMaskAlpha = val;
        }

        /// <summary>
        /// 设置散焦遮罩贴图
        /// </summary>
        /// <param tweenName="tex">目标散焦遮罩贴图</param>
        public void hm_BlurMaskTextureSet(Texture2D tex)
        {
            if (tex == null)
            {
                if (BlurMask != null)
                    BlurMask.sprite = null;
                return;
            }
            BlurMask.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);
            if (Act_BlurMaskChanged_Texture != null)
                Act_BlurMaskChanged_Texture(tex);
        }

        #endregion

        #region 控制散焦特性效果

        /// <summary>
        /// 控制散焦特性强度
        /// </summary>
        /// <param tweenName="Intensity">强度</param>
        public void hm_UniversalFeature_Blur_Get()
        {
            if (UniversalFeature_Blur == null)
                UniversalFeature_Blur = GetFeature<UniversalBlurFeature>("UniversalBlurFeature");
            else
                UniversalFeature_Blur_Intensity = UniversalFeature_Blur.intensity;
        }

        /// <summary>
        /// 内容透明度更新
        /// </summary>
        public void hm_UniversalFeature_Blur_Update()
        {
            if (UniversalFeature_Blur == null)
                return;
            if (UniversalFeature_Blur_Intensity >= 1)
            {
                if (FeatureBlur_IntensityToMax)
                {
                    FeatureBlur_IntensityToMin = true;
                    FeatureBlur_IntensityToMax = false;
                    if (Act_BlurMask_Intensity_IsMax != null)
                        Act_BlurMask_Intensity_IsMax(UniversalFeature_Blur_Intensity);
                }
            }
            else if (UniversalFeature_Blur_Intensity <= 0)
            {
                if (FeatureBlur_IntensityToMin)
                {
                    FeatureBlur_IntensityToMin = false;
                    FeatureBlur_IntensityToMax = true;
                    if (Act_BlurMask_Intensity_IsMin != null)
                        Act_BlurMask_Intensity_IsMin(UniversalFeature_Blur_Intensity);
                }
            }
            UniversalFeature_Blur.intensity = UniversalFeature_Blur_Intensity;
        }

        /// <summary>
        /// 散焦特性效果强度平滑到
        /// </summary>
        /// <param tweenName="val">目标强度</param>
        /// <param tweenName="dur">耗时</param>
        /// <param tweenName="ease">缓动</param>
        /// <param tweenName="delay">延迟</param>
        /// <param tweenName="action_start">委托 - 开始时</param>
        /// <param tweenName="action_end">委托 - 结束时</param>
        public void hm_UniversalFeature_Blur_To(float val, float dur, Ease ease, float delay, UnityAction action_start = null, UnityAction action_end = null)
        {
            if (UniversalFeature_Blur == null)
                return;
            if (twn_UniversalFeature_Blur != null && twn_UniversalFeature_Blur.active)
                if (twn_UniversalFeature_Blur.IsPlaying())
                    twn_UniversalFeature_Blur.Kill();
            twn_UniversalFeature_Blur = DOTween.To(() => UniversalFeature_Blur_Intensity, x => UniversalFeature_Blur_Intensity = x, val, dur).SetEase(ease).SetAutoKill(true).SetDelay(delay).OnStart(() =>
            {
                if (action_start != null)
                    action_start();
            }).OnComplete(() =>
            {
                if (action_end != null)
                    action_end();
            });
        }

        /// <summary>
        /// 散焦特性效果强度平滑到
        /// </summary>
        /// <param tweenName="val">目标强度</param>
        /// <param tweenName="dur">耗时</param>
        /// <param tweenName="ease">缓动</param>
        /// <param tweenName="delay">延迟</param>
        /// <param tweenName="action_start">委托 - 开始时</param>
        /// <param tweenName="action_end">委托 - 结束时</param>
        public void hm_UniversalFeature_Blur_To(float val, float dur, AnimationCurve ease, float delay, UnityAction action_start = null, UnityAction action_end = null)
        {
            if (UniversalFeature_Blur == null)
                return;
            if (twn_UniversalFeature_Blur != null && twn_UniversalFeature_Blur.active)
                if (twn_UniversalFeature_Blur.IsPlaying())
                    twn_UniversalFeature_Blur.Kill();
            twn_UniversalFeature_Blur = DOTween.To(() => UniversalFeature_Blur_Intensity, x => UniversalFeature_Blur_Intensity = x, val, dur).SetEase(ease).SetAutoKill(true).SetDelay(delay).OnStart(() =>
            {
                if (action_start != null)
                    action_start();
            }).OnComplete(() =>
            {
                if (action_end != null)
                    action_end();
            });
        }

        /// <summary>
        /// 散焦特性效果强度快速到
        /// </summary>
        /// <param tweenName="val">目标强度</param>
        public void hm_UniversalFeature_Blur_FastTo(float val)
        {
            if (UniversalFeature_Blur == null)
                return;
            if (twn_UniversalFeature_Blur != null && twn_UniversalFeature_Blur.active)
                if (twn_UniversalFeature_Blur.IsPlaying())
                    twn_UniversalFeature_Blur.Kill();
            UniversalFeature_Blur_Intensity = val;
        }

        /// <summary>
        /// 散焦特性效果强度快速到 - Editor用
        /// </summary>
        /// <param tweenName="val">目标强度</param>
        public void hm_UniversalFeature_Blur_FastTo_ForEditor(float val)
        {
            if (UniversalFeature_Blur == null)
                UniversalFeature_Blur = GetFeature<UniversalBlurFeature>("UniversalBlurFeature");
            else
                UniversalFeature_Blur.intensity = val;
        }
        #endregion

        #region 转场器

        /// <summary>
        /// 遮罩更新
        /// </summary>
        public void hm_TransitionUpdate()
        {
            if (Hud_TransitionController == null)
                return;
            hm_TransitionTopView();
        }

        /// <summary>
        /// 将Transition置于最上层
        /// </summary>
        public void hm_TransitionTopView()
        {
            if (Hud_TransitionController == null)
                return;
            Hud_TransitionController.transform.SetAsLastSibling();
        }
        #endregion

        #region 音效播放器池

        /// <summary>
        /// 音效播放器池初始化
        /// </summary>
        /// <param tweenName="count">预制数量</param>
        private void hm_LibrarySounds_Initialize(int count)
        {
            if (count == 0)
                return;

            Pool_Sounder = new AudioPlayer[count];

            GameObject PoolRoot = new GameObject();
            PoolRoot.name = "Pool_Sound";
            PoolRoot.transform.SetParent(transform);
            PoolRoot.transform.localPosition = Vector3.zero;
            PoolRoot.transform.localEulerAngles = Vector3.zero;
            PoolRoot.transform.localScale = Vector3.one;

            for (int i = 0; i < Pool_Sounder.Length; i++)
            {
                AudioPlayer ap = new AudioPlayer();
                GameObject obj = new GameObject();
                obj.name = "Sounder_" + i;
                obj.transform.SetParent(PoolRoot.transform);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localEulerAngles = Vector3.zero;
                obj.transform.localScale = Vector3.one;
                ap.Player = obj.AddComponent<AudioSource>();
                ap.Player.minDistance = 0.1f;
                ap.Player.maxDistance = 0.2f;
                ap.Player.playOnAwake = false;
                Pool_Sounder[i] = ap;
            }
        }

        /// <summary>
        /// 取得一个音效播放器
        /// </summary>
        /// <returns></returns>
        public AudioSource hm_LibrarySounds_GetSounder()
        {
            AudioSource aus = null;
            if (Pool_Sounder == null && Pool_Sounder.Length <= 0)
            {
                Debug.Log("您未指定音效池的数量！因此无法播放声音！请前往HudManager中配置音效池的数量！");
                return null;
            }
            for (int i = 0; i < Pool_Sounder.Length; i++)
            {
                if (!Pool_Sounder[i].Player.isPlaying)
                {
                    aus = Pool_Sounder[i].Player;
                    break;
                }
            }
            return aus;
        }

        /// <summary>
        /// 根据音效名字取得其在音效库中的索引号
        /// </summary>
        /// <returns></returns>
        public int hm_LibrarySounds_GetSoundIndex(string name)
        {
            int index = -1;
            for (int i = 0; i < Hud_Sounds.SoundLibrary.Count; i++)
            {
                if (Hud_Sounds.SoundLibrary[i].Name == name)
                {
                    index = i;
                }
            }
            if (index == -1)
            {
                if (UseDebug)
                    util_Tools.Func_PrintInfo("Hud Manager管理器消息", "无法在音效库里找到目标名称的索引号！", HudMsgState.警告);
                return 0;
            }
            else
            {
                return index;
            }
        }

        /// <summary>
        /// 根据音效索引号取得其在音效库中的名称
        /// </summary>
        /// <returns></returns>
        public string hm_LibrarySounds_GetSoundName(int index)
        {
            if (Hud_Sounds.SoundLibrary_IndexIsValid(index))
            {
                return Hud_Sounds.SoundLibrary[index].Name;
            }
            else
            {
                if (UseDebug)
                    util_Tools.Func_PrintInfo("Hud Manager管理器消息", "无法在音效库里找到目标索引号的音效名称！", HudMsgState.警告);
                return "";
            }
        }

        public void hm_LibrarySounds_Update()
        {
            if (Pool_Sounder == null || Pool_Sounder.Length <= 0)
                return;
            for (int i = 0; i < Pool_Sounder.Length; i++)
            {
                Pool_Sounder[i].IsPlaying = Pool_Sounder[i].Player.isPlaying;
            }
        }

        /// <summary>
        /// 播放器池清理
        /// </summary>
        public void hm_LibrarySounds_Clean()
        {
            for (int i = 0; i < Pool_Sounder.Length; i++)
            {
                if (Pool_Sounder[i].Player.clip != null)
                {
                    Pool_Sounder[i].Player.clip = null;
                }
                Pool_Sounder[i].Player.transform.localPosition = Vector3.zero;
                Pool_Sounder[i].Player.transform.localEulerAngles = Vector3.zero;
                Pool_Sounder[i].Player.transform.localScale = Vector3.one;
            }
        }

        #endregion

        #region 光标

        /// <summary>
        /// 光标 - 尺寸_Size
        /// </summary>
        /// <param tweenName="size">尺寸_Size</param>
        public void hm_Cursor_SizeSet(float size)
        {
            if (Hud_MouseCursor == null)
                return;
            Hud_MouseCursor.mc_SetCursorSize(size);
        }

        /// <summary>
        /// 光标 - 启用
        /// </summary>
        public void hm_Cursor_Enabled()
        {
            if (Hud_MouseCursor == null)
                return;
            Hud_MouseCursor.mc_SetCustomCursorEnabled(true);
        }

        /// <summary>
        /// 光标 - 禁用
        /// </summary>
        public void hm_Cursor_Disabled()
        {
            if (Hud_MouseCursor == null)
                return;
            Hud_MouseCursor.mc_SetCustomCursorEnabled(false);
        }

        /// <summary>
        /// 光标 - 显示
        /// </summary>
        public void hm_Cursor_Display()
        {
            if (Hud_MouseCursor == null)
                return;
            Hud_MouseCursor.mc_CursorOpacitySet(1);
        }

        /// <summary>
        /// 光标 - 隐藏
        /// </summary>
        public void hm_Cursor_Hide()
        {
            if (Hud_MouseCursor == null)
                return;
            Hud_MouseCursor.mc_CursorOpacitySet(0);
        }

        /// <summary>
        /// 光标 - 快速显示
        /// </summary>
        public void hm_Cursor_FastDisplay()
        {
            if (Hud_MouseCursor == null)
                return;
            Hud_MouseCursor.mc_CursorOpacityFastSet(1);
        }

        /// <summary>
        /// 光标 - 快速隐藏
        /// </summary>
        public void hm_Cursor_FastHide()
        {
            if (Hud_MouseCursor == null)
                return;
            Hud_MouseCursor.mc_CursorOpacityFastSet(0);
        }

        /// <summary>
        /// 光标 - 解锁
        /// </summary>
        public void hm_Cursor_UnLock()
        {
            if (Hud_MouseCursor == null)
                return;
            Hud_MouseCursor.mc_LockCursor(CursorLockMode.None);
        }

        /// <summary>
        /// 光标 - 锁定
        /// </summary>
        public void hm_Cursor_Locked()
        {
            if (Hud_MouseCursor == null)
                return;
            Hud_MouseCursor.mc_LockCursor(CursorLockMode.Locked);
        }

        /// <summary>
        /// 光标 - 改变样式
        /// </summary>
        /// <param tweenName="style">目标样式</param>
        public void hm_Cursor_ChangeStyle(string style)
        {
            if (Hud_MouseCursor == null)
                return;
            Hud_MouseCursor.mc_ChangeStyle(style);
        }

        /// <summary>
        /// 光标 - 重置样式
        /// </summary>
        public void hm_Cursor_ResetStyle()
        {
            if (Hud_MouseCursor == null)
                return;
            Hud_MouseCursor.mc_ResetCursorImage();
        }

        /// <summary>
        /// 光标 - 设置颜色
        /// </summary>
        /// <param tweenName="color">目标颜色</param>
        public void hm_Cursor_SetColor(Color color)
        {
            if (Hud_MouseCursor == null)
                return;
            Hud_MouseCursor.mc_SetCursorColor(color);
        }
        #endregion

        #region 蓝图

        #region 调用

        /// <summary>
        /// 蓝图视觉透明度
        /// </summary>
        public void hm_BluePrint_OpacitySet(float opacity, float dur = 1, Ease ease = Ease.OutExpo, float delay = 0)
        {
            if (!BluePrintMode)
            {
                if (UseDebug)
                    util_Tools.Func_PrintInfo("Hud Manager管理器消息", "蓝图模式已关闭！", HudMsgState.设置);
                return;
            }

            hm_BluePrint_Fade(opacity, dur, ease, delay);

            if (opacity == 0)
            {
                if (Act_BluePrint_IsTransparency != null)
                    Act_BluePrint_IsTransparency();
            }
            else
            {
                if (Act_BluePrint_IsSolid != null)
                    Act_BluePrint_IsSolid();
            }

            if (UseDebug)
                util_Tools.Func_PrintInfo("Hud Manager管理器消息", "蓝图网格入场", HudMsgState.设置);
        }

        /// <summary>
        /// 蓝图入场
        /// </summary>
        /// <param tweenName="eft_grid">影响网格</param>
        /// <param tweenName="eft_bg">影响背景</param>
        /// <param tweenName="eft_mark">影响水印</param>
        public void hm_BluePrint_In(bool eft_grid = true, bool eft_gridfade = true, bool eft_bg = true, bool eft_mark = true)
        {
            if (!BluePrintMode)
            {
                if (UseDebug)
                    util_Tools.Func_PrintInfo("Hud Manager管理器消息", "蓝图模式已关闭！", HudMsgState.设置);
                return;
            }

            if (BluePrint_Displayed)
            {
                if (UseDebug)
                    util_Tools.Func_PrintInfo("Hud Manager管理器消息", "蓝图网格入场，无需重复入场！", HudMsgState.设置);
                return;
            }

            BluePrint_Displayed = true;

            hm_BluePrint_ResetTweeners();

            Eft_Grid = eft_grid;
            Eft_GridFade = eft_gridfade;
            Eft_Bg = eft_bg;
            Eft_Mark = eft_mark;

            if (Eft_GridFade)
                hm_BluePrint_Grid_Fade(1, BluePrint_AnimationDuration, BluePrint_Grid_AnimationEase);
            if (Eft_Grid)
                hm_BluePrint_Grid_Length(BluePrint_GridEnd, BluePrint_Grid_AnimationDuration, BluePrint_Grid_AnimationEase);
            if (Eft_Bg)
                hm_BluePrint_Bg_Fade(1, BluePrint_AnimationDuration, BluePrint_Bg_AnimationEase_In);
            if (Eft_Mark)
                hm_BluePrint_Mark_Fade(1, BluePrint_AnimationDuration, BluePrint_Bg_AnimationEase_In);

            if (Act_BluePrint_In != null)
                Act_BluePrint_In();

            if (UseDebug)
                util_Tools.Func_PrintInfo("Hud Manager管理器消息", "蓝图网格入场", HudMsgState.设置);
        }

        /// <summary>
        /// 蓝图入场
        /// </summary>
        /// <param tweenName="eft_grid">影响网格</param>
        /// <param tweenName="eft_bg">影响背景</param>
        /// <param tweenName="eft_mark">影响水印</param>
        public void hm_BluePrint_In()
        {
            if (!BluePrintMode)
            {
                if (UseDebug)
                    util_Tools.Func_PrintInfo("Hud Manager管理器消息", "蓝图模式已关闭！", HudMsgState.设置);
                return;
            }

            if (BluePrint_Displayed)
            {
                if (UseDebug)
                    util_Tools.Func_PrintInfo("Hud Manager管理器消息", "蓝图网格入场，无需重复入场！", HudMsgState.设置);
                return;
            }

            BluePrint_Displayed = true;

            hm_BluePrint_ResetTweeners();

            if (Eft_GridFade)
                hm_BluePrint_Grid_Fade(1, BluePrint_AnimationDuration, BluePrint_Grid_AnimationEase);
            if (Eft_Grid)
                hm_BluePrint_Grid_Length(BluePrint_GridEnd, BluePrint_Grid_AnimationDuration, BluePrint_Grid_AnimationEase);
            if (Eft_Bg)
                hm_BluePrint_Bg_Fade(1, BluePrint_AnimationDuration, BluePrint_Bg_AnimationEase_In);
            if (Eft_Mark)
                hm_BluePrint_Mark_Fade(1, BluePrint_AnimationDuration, BluePrint_Bg_AnimationEase_In);

            if (Act_BluePrint_In != null)
                Act_BluePrint_In();

            if (UseDebug)
                util_Tools.Func_PrintInfo("Hud Manager管理器消息", "蓝图网格入场", HudMsgState.设置);
        }

        /// <summary>
        /// 蓝图退场
        /// </summary>
        public void hm_BluePrint_Out()
        {
            if (!BluePrintMode)
            {
                if (UseDebug)
                    util_Tools.Func_PrintInfo("Hud Manager管理器消息", "蓝图模式已关闭！", HudMsgState.设置);
                return;
            }

            if (!BluePrint_Displayed)
            {
                if (UseDebug)
                    util_Tools.Func_PrintInfo("Hud Manager管理器消息", "蓝图网格已经退场，无需重复退场！", HudMsgState.设置);
                return;
            }

            BluePrint_Displayed = false;

            hm_BluePrint_ResetTweeners();

            hm_BluePrint_Grid_Fade(0, BluePrint_AnimationDuration, BluePrint_Grid_AnimationEase);
            hm_BluePrint_Grid_Length(BluePrint_GridStart, BluePrint_AnimationDuration, BluePrint_Grid_AnimationEase);
            hm_BluePrint_Bg_Fade(-0.5f, BluePrint_AnimationDuration, BluePrint_Bg_AnimationEase_Out, BluePrint_Bg_FadeAnimationDelay);
            hm_BluePrint_Mark_Fade(-0.5f, BluePrint_AnimationDuration, BluePrint_Bg_AnimationEase_Out);

            if (Act_BluePrint_Out != null)
                Act_BluePrint_Out();

            if (UseDebug)
                util_Tools.Func_PrintInfo("Hud Manager管理器消息", "蓝图网格退场", HudMsgState.设置);
        }

        /// <summary>
        /// 蓝图动画清空重置
        /// </summary>
        public void hm_BluePrint_ResetTweeners()
        {
            if (!BluePrintMode)
            {
                if (UseDebug)
                    util_Tools.Func_PrintInfo("Hud Manager管理器消息", "蓝图模式已关闭！", HudMsgState.设置);
                return;
            }

            hm_BluePrint_StopTweener(BluePrint_BgFadeTweener);
            hm_BluePrint_StopTweener(BluePrint_grid_opacityTweener);
            hm_BluePrint_StopTweener(BluePrint_MarkFadeTweener);
            hm_BluePrint_StopTweener(BluePrint_GridLengthTweener);

            if (UseDebug)
                util_Tools.Func_PrintInfo("Hud Manager管理器消息", "蓝图网格清空并停止动画", HudMsgState.设置);
        }

        /// <summary>
        /// 停止动画器
        /// </summary>
        /// <param tweenName="twn"></param>
        private void hm_BluePrint_StopTweener(Tweener twn)
        {
            twn.Kill();
            twn.Rewind();
        }

        /// <summary>
        /// 蓝图动画快速到起始隐藏状态
        /// </summary>
        public void hm_BluePrint_Hide()
        {
            if (!BluePrintMode)
            {
                if (UseDebug)
                    util_Tools.Func_PrintInfo("Hud Manager管理器消息", "蓝图模式已关闭！", HudMsgState.设置);
                return;
            }

            BluePrint_Grid_LengthPercentage = 0;
            BluePrint_bg_opacity = 0;
            BluePrint_mark_opacity = 0;

            if (UseDebug)
                util_Tools.Func_PrintInfo("Hud Manager管理器消息", "蓝图网格快速到隐藏状态", HudMsgState.设置);
        }

        /// <summary>
        /// 将蓝图设为图层级的最底层
        /// </summary>
        public void hm_BluePrintRootFirstSibling()
        {
            if (BluePrint_root != null)
            {
                Transform trs = (Transform)BluePrint_root;
                trs.SetAsFirstSibling();
            }
        }

        #endregion

        #region 动画

        /// <summary>
        /// 蓝图淡化方式
        /// </summary>
        /// <param tweenName="val">透明度_Alpha</param>
        /// <param tweenName="dur">耗时</param>
        /// <param tweenName="ease">缓动</param>
        /// <param tweenName="delay">延迟</param>
        private void hm_BluePrint_Fade(float val, float dur = 1, Ease ease = Ease.OutQuart, float delay = 0)
        {
            BluePrint_opacityTweener = DOTween.To(() => BluePrint_opacity, opa => BluePrint_opacity = opa, val, dur).SetRelative(false).SetEase(ease).SetDelay(delay).OnComplete(() =>
            {
                hm_BluePrint_StopTweener(BluePrint_opacityTweener);
            });
        }

        /// <summary>
        /// 蓝图背景淡化方式
        /// </summary>
        /// <param tweenName="val">透明度_Alpha</param>
        /// <param tweenName="dur">耗时</param>
        /// <param tweenName="ease">缓动</param>
        /// <param tweenName="delay">延迟</param>
        private void hm_BluePrint_Bg_Fade(float val, float dur = 1, Ease ease = Ease.OutQuart, float delay = 0)
        {
            BluePrint_BgFadeTweener = DOTween.To(() => BluePrint_bg_opacity, opa => BluePrint_bg_opacity = opa, val, dur).SetRelative(false).SetEase(ease).SetDelay(delay);
        }

        /// <summary>
        /// 水印淡化方式
        /// </summary>
        /// <param tweenName="val">透明度_Alpha</param>
        /// <param tweenName="dur">耗时</param>
        /// <param tweenName="ease">缓动</param>
        /// <param tweenName="delay">延迟</param>
        private void hm_BluePrint_Mark_Fade(float val, float dur = 1, Ease ease = Ease.OutQuart, float delay = 0f)
        {
            BluePrint_MarkFadeTweener = DOTween.To(() => BluePrint_mark_opacity, opa => BluePrint_mark_opacity = opa, val, dur).SetRelative(false).SetEase(ease).SetDelay(delay);
        }

        /// <summary>
        /// 网格生长方式
        /// </summary>
        /// <param tweenName="val">透明度_Alpha</param>
        /// <param tweenName="dur">耗时</param>
        /// <param tweenName="ease">缓动</param>
        /// <param tweenName="delay">延迟</param>
        private void hm_BluePrint_Grid_Length(float val, float dur = 1, Ease ease = Ease.OutQuart, float delay = 0f)
        {
            BluePrint_GridLengthTweener = DOTween.To(() => BluePrint_Grid_LengthPercentage, opa => BluePrint_Grid_LengthPercentage = opa, val, dur).SetRelative(false).SetEase(ease).SetDelay(delay).OnComplete(() =>
            {
                hm_BluePrint_ResetTweeners();
            });
        }

        /// <summary>
        /// 网格淡化方式
        /// </summary>
        /// <param tweenName="val">透明度_Alpha</param>
        /// <param tweenName="dur">耗时</param>
        /// <param tweenName="ease">缓动</param>
        /// <param tweenName="delay">延迟</param>
        private void hm_BluePrint_Grid_Fade(float val, float dur = 1, Ease ease = Ease.OutQuart, float delay = 0)
        {
            BluePrint_grid_opacityTweener = DOTween.To(() => BluePrint_grid_Opacity, opa => BluePrint_grid_Opacity = opa, val, dur).SetRelative(false).SetEase(ease).SetDelay(delay).OnComplete(() =>
            {
                hm_BluePrint_StopTweener(BluePrint_grid_opacityTweener);
            });
        }
        #endregion

        #region 操作预更新

        /// <summary>
        /// 蓝图创建网格线
        /// </summary>
        public void hm_BluePrint_Create(bool Hide = true, Material bluemat = null, TMP_FontAsset font_title = null, TMP_FontAsset font_subtitle = null)
        {
            if (!BluePrintMode)
            {
                if (UseDebug)
                    util_Tools.Func_PrintInfo("Hud Manager管理器消息", "蓝图模式已关闭！", HudMsgState.设置);
                return;
            }

            if (BluePrint_root != null)
                return;

            #region 创建根物体
            GameObject obj_isolateVisual_root = new GameObject();
            obj_isolateVisual_root.name = "IsolateVisual";
            obj_isolateVisual_root.layer = LayerMask.NameToLayer("XHud");
            RectTransform trs_root = obj_isolateVisual_root.AddComponent<RectTransform>();
            CanvasGroup cg = obj_isolateVisual_root.AddComponent<CanvasGroup>();
            cg.blocksRaycasts = false;
            cg.interactable = false;
            BluePrint_canvasgroup = cg;

            trs_root.SetParent(hm_Layout_GetAnchor(HudAnchor.底层));
            trs_root.anchorMin = new Vector2(0, 0);
            trs_root.anchorMax = new Vector2(1, 1);
            trs_root.pivot = new Vector2(0.5f, 0.5f);
            trs_root.sizeDelta = new Vector2(0, 0);
            trs_root.anchoredPosition3D = Vector3.zero;
            trs_root.localEulerAngles = Vector3.zero;
            trs_root.localScale = Vector3.one;
            BluePrint_root = trs_root;

            #endregion

            #region 创建底色背景
            GameObject obj_isolateVisual_bg = new GameObject();
            obj_isolateVisual_bg.name = "bg";
            obj_isolateVisual_bg.layer = LayerMask.NameToLayer("XHud");
            RectTransform trs_bg = obj_isolateVisual_bg.AddComponent<RectTransform>();
            trs_bg.SetParent(trs_root);
            trs_bg.anchorMin = new Vector2(0, 0);
            trs_bg.anchorMax = new Vector2(1, 1);
            trs_bg.pivot = new Vector2(0.5f, 0.5f);
            trs_bg.sizeDelta = new Vector2(0, 0);
            trs_bg.anchoredPosition3D = Vector3.zero;
            trs_bg.localEulerAngles = Vector3.zero;
            trs_bg.localScale = Vector3.one;
            Image img_bg = obj_isolateVisual_bg.AddComponent<Image>();
            img_bg.raycastTarget = false;
            img_bg.maskable = false;
            img_bg.color = BluePrint_bg_color;
            img_bg.material = bluemat;
            BluePrint_mainbg = img_bg;
            #endregion

            #region 创建网格
            int h_count = (int)BluePrint_grid_size;
            float dis = ScreenRes.x / h_count;
            int v_count = (int)(ScreenRes.y / dis);

            #region 垂直平铺
            for (int i = 0; i < v_count + 1; i++)
            {
                GameObject obj_isolateVisual_grid = new GameObject();
                obj_isolateVisual_grid.name = "grid_v_" + i;
                obj_isolateVisual_grid.layer = LayerMask.NameToLayer("XHud");
                RectTransform trs_grid = obj_isolateVisual_grid.AddComponent<RectTransform>();
                trs_grid.SetParent(trs_root);

                if (i == 0)
                {
                    trs_grid.pivot = new Vector2(0f, 0f);
                }
                else if (i == v_count + 1)
                {
                    trs_grid.pivot = new Vector2(0f, 0f);
                }
                else
                {
                    trs_grid.pivot = new Vector2(0f, 0.5f);
                }
                trs_grid.anchorMin = new Vector2(0, 0);
                trs_grid.anchorMax = new Vector2(0, 0);
                trs_grid.sizeDelta = new Vector2(ScreenRes.x - (BluePrint_linewidth * 2), BluePrint_linewidth);

                trs_grid.anchoredPosition3D = new Vector3(BluePrint_linewidth, i * dis, 0);

                trs_grid.localEulerAngles = Vector3.zero;
                trs_grid.localScale = Vector3.one;
                Image img_grid = obj_isolateVisual_grid.AddComponent<Image>();
                img_grid.raycastTarget = false;
                img_grid.maskable = false;
                img_grid.color = BluePrint_grid_color;
                BluePrint_gridlines_V.Add(img_grid);
            }
            #endregion

            #region 水平平铺
            for (int i = 0; i < h_count + 1; i++)
            {
                GameObject obj_isolateVisual_grid = new GameObject();
                obj_isolateVisual_grid.name = "grid_h_" + i;
                obj_isolateVisual_grid.layer = LayerMask.NameToLayer("XHud");
                RectTransform trs_grid = obj_isolateVisual_grid.AddComponent<RectTransform>();
                trs_grid.SetParent(trs_root);

                if (i == 0)
                {
                    trs_grid.pivot = new Vector2(0f, 0f);
                }
                else if (i == h_count + 1)
                {
                    trs_grid.pivot = new Vector2(1f, 0f);
                }
                else
                {
                    trs_grid.pivot = new Vector2(0.5f, 0f);
                }
                trs_grid.anchorMin = new Vector2(0, 0);
                trs_grid.anchorMax = new Vector2(0, 0);

                if (i == 0)
                {
                    trs_grid.sizeDelta = new Vector2(BluePrint_linewidth, ScreenRes.y);
                    trs_grid.anchoredPosition3D = new Vector3(i * dis, 0, 0);
                }
                else if (i == h_count)
                {
                    trs_grid.sizeDelta = new Vector2(BluePrint_linewidth, ScreenRes.y);
                    trs_grid.anchoredPosition3D = new Vector3(i * dis - (BluePrint_linewidth * 0.5f), 0, 0);
                }
                else
                {
                    trs_grid.sizeDelta = new Vector2(BluePrint_linewidth, ScreenRes.y - BluePrint_linewidth);
                    trs_grid.anchoredPosition3D = new Vector3(i * dis, BluePrint_linewidth, 0);
                }


                trs_grid.localEulerAngles = Vector3.zero;
                trs_grid.localScale = Vector3.one;
                Image img_grid = obj_isolateVisual_grid.AddComponent<Image>();
                img_grid.raycastTarget = false;
                img_grid.maskable = false;
                img_grid.color = BluePrint_grid_color;
                BluePrint_gridlines_H.Add(img_grid);
            }
            #endregion

            #endregion

            #region 创建标签

            GameObject obj_isolateVisual_mark_title = new GameObject();
            obj_isolateVisual_mark_title.name = "title";
            obj_isolateVisual_mark_title.layer = LayerMask.NameToLayer("XHud");
            RectTransform trs_mark_title = obj_isolateVisual_mark_title.AddComponent<RectTransform>();
            trs_mark_title.SetParent(trs_root);
            trs_mark_title.anchorMin = new Vector2(0, 0);
            trs_mark_title.anchorMax = new Vector2(1, 1);
            trs_mark_title.pivot = new Vector2(0.5f, 0.5f);
            trs_mark_title.sizeDelta = new Vector2(0, 0);
            trs_mark_title.anchoredPosition3D = Vector3.zero;
            trs_mark_title.localEulerAngles = Vector3.zero;
            trs_mark_title.localScale = Vector3.one;
            Hud_TmpText title = obj_isolateVisual_mark_title.AddComponent<Hud_TmpText>();
            title.TextStyleInfo.gen_RayCastSet(false);
            title.TextStyleInfo.gen_Set_MaskableSet(false);
            title.TextStyleInfo.tmp_font = font_title;
            title.SyncGlobalFontSize = false;
            BluePrint_title_module = title;

            GameObject obj_isolateVisual_mark_subtitle = new GameObject();
            obj_isolateVisual_mark_subtitle.name = "subtitle";
            obj_isolateVisual_mark_subtitle.layer = LayerMask.NameToLayer("XHud");
            RectTransform trs_mark_subtitle = obj_isolateVisual_mark_subtitle.AddComponent<RectTransform>();
            trs_mark_subtitle.SetParent(trs_root);
            trs_mark_subtitle.anchorMin = new Vector2(0, 0);
            trs_mark_subtitle.anchorMax = new Vector2(1, 1);
            trs_mark_subtitle.pivot = new Vector2(0.5f, 0.5f);
            trs_mark_subtitle.sizeDelta = new Vector2(0, 0);
            trs_mark_subtitle.anchoredPosition3D = Vector3.zero;
            trs_mark_subtitle.localEulerAngles = Vector3.zero;
            trs_mark_subtitle.localScale = Vector3.one;
            Hud_TmpText subtitle = obj_isolateVisual_mark_subtitle.AddComponent<Hud_TmpText>();
            subtitle.TextStyleInfo.gen_RayCastSet(false);
            subtitle.TextStyleInfo.gen_Set_MaskableSet(false);
            subtitle.TextStyleInfo.tmp_font = font_subtitle;
            subtitle.SyncGlobalFontSize = false;
            BluePrint_subtitle_module = subtitle;

            #endregion

            BluePrint_bg_opacity = 1;
            BluePrint_Grid_LengthPercentage = BluePrint_GridEnd;
            BluePrint_mark_opacity = 1;

            hm_BluePrint_Update();

            if (UseDebug)
                util_Tools.Func_PrintInfo("Hud Manager管理器消息", "已创建蓝图网格", HudMsgState.设置);
        }

        /// <summary>
        /// 蓝图移除网格线
        /// </summary>
        public void hm_BluePrint_Remove(bool reset = true)
        {
            if (BluePrint_root == null)
                return;

            DestroyImmediate(BluePrint_root.gameObject, true);

            BluePrint_gridlines_H.Clear();
            BluePrint_gridlines_V.Clear();

            BluePrint_root = null;
            BluePrint_canvasgroup = null;
            BluePrint_mainbg = null;
            BluePrint_title_module = null;
            BluePrint_subtitle_module = null;
            BluePrint_Displayed = false;
            if (reset)
            {
                BluePrint_bg_opacity = 0;
                BluePrint_Grid_LengthPercentage = 0;
                BluePrint_mark_opacity = 0;
            }
            hm_BluePrint_ClearHidden();

            if (UseDebug)
                util_Tools.Func_PrintInfo("Hud Manager管理器消息", "已移除蓝图网格", HudMsgState.设置);
        }

        private void hm_BluePrint_ClearHidden()
        {
            RectTransform rect_back = hm_Layout_GetAnchor(HudAnchor.底层);
            if (rect_back.childCount > 0)
            {
                for (int i = 0; i < rect_back.childCount; i++)
                {
                    //if (rect_back.GetChild(i).gameObject.hideFlags == HideFlags.HideInHierarchy)
                    DestroyImmediate(rect_back.GetChild(i).gameObject, true);
                }
            }
        }

        /// <summary>
        /// 设置背景图像
        /// </summary>
        /// <param tweenName="tex"></param>
        public void hm_BluePrint_SetBgTexture(Texture2D tex)
        {
            if (BluePrint_mainbg != null)
                BluePrint_mainbg.material.SetTexture("_BgMap", tex);
        }

        /// <summary>
        /// 设置背景图像平铺模式
        /// </summary>
        /// <param tweenName="treeState"></param>
        public void hm_BluePrint_SetBg_TilingMode(bool state)
        {
            if (BluePrint_mainbg != null)
            {
                if (state)
                {
                    BluePrint_mainbg.material.SetFloat("_UseTiling", 1);
                }
                else
                {
                    BluePrint_mainbg.material.SetFloat("_UseTiling", 0);
                }
            }
        }

        /// <summary>
        /// 设置背景图像强制方形比例
        /// </summary>
        /// <param tweenName="treeState"></param>
        public void hm_BluePrint_SetBg_SquareRatio(bool state)
        {
            if (BluePrint_mainbg != null)
            {
                if (state)
                {
                    BluePrint_mainbg.material.SetFloat("_UseRatio", 1);
                }
                else
                {
                    BluePrint_mainbg.material.SetFloat("_UseRatio", 0);
                }
            }
        }

        /// <summary>
        /// 解析Int到Bool
        /// </summary>
        /// <param tweenName="val"></param>
        /// <returns></returns>
        public bool ConvertIntToBool(int val)
        {
            return val != 0;
        }

        /// <summary>
        /// 蓝图模式更新
        /// </summary>
        public void hm_BluePrint_Update()
        {
            if (!BluePrintMode)
                return;

            #region 整体透明度
            CanvasGroup img_cg = (CanvasGroup)BluePrint_canvasgroup;
            if (img_cg != null)
                img_cg.alpha = BluePrint_opacity;
            #endregion

            #region 主背景颜色
            if (BluePrint_mainbg != null)
            {
                float alp_bg = BluePrint_bg_color.a * BluePrint_bg_opacity;

                Color bgcolor = BluePrint_bg_color;
                bgcolor.a = alp_bg;

                BluePrint_mainbg.color = bgcolor;
                BluePrint_mainbg.material.SetVector("_Tiling", new Vector2(BluePrint_bg_tilling, BluePrint_bg_tilling));
                hm_BluePrint_SetBg_TilingMode(ConvertIntToBool(BluePrint_bg_usetilling_index));
                hm_BluePrint_SetBg_SquareRatio(ConvertIntToBool(BluePrint_bg_usesquareratio_index));
            }
            #endregion

            #region 背景叠加图片颜色
            if (BluePrint_mainbg != null)
            {
                BluePrint_mainbg.material.SetColor("_Color", BluePrint_bg_decal_color);
                BluePrint_mainbg.material.SetFloat("_BgMapOpacity", BluePrint_bg_mapOpacity);
            }
            #endregion

            #region 网格尺寸&颜色

            float alp = BluePrint_grid_color.a * BluePrint_grid_Opacity;

            Color gridcolor = BluePrint_grid_color;
            gridcolor.a = alp;

            if (BluePrint_gridlines_H.Count > 0)
            {
                for (int i = 0; i < BluePrint_gridlines_H.Count; i++)
                {
                    if (BluePrint_gridlines_H[i] == null)
                        continue;

                    BluePrint_gridlines_H[i].color = gridcolor;

                    float height = ScreenRes.y * BluePrint_Grid_LengthPercentage - (BluePrint_Grid_LevelHeight * i) * 0.55f;

                    BluePrint_gridlines_H[i].rectTransform.sizeDelta = new Vector2(BluePrint_linewidth, Mathf.Clamp(height, 0, ScreenRes.y));
                }
            }
            if (BluePrint_gridlines_V.Count > 0)
            {
                for (int i = 0; i < BluePrint_gridlines_V.Count; i++)
                {
                    if (BluePrint_gridlines_V[i] == null)
                        continue;

                    BluePrint_gridlines_V[i].color = gridcolor;

                    float height = ScreenRes.x * BluePrint_Grid_LengthPercentage - (BluePrint_Grid_LevelHeight * i);

                    BluePrint_gridlines_V[i].rectTransform.sizeDelta = new Vector2(Mathf.Clamp(height, 0, ScreenRes.x), BluePrint_linewidth);
                }
            }

            #endregion

            BluePrint_mark_opacity = Mathf.Clamp01(BluePrint_mark_opacity);

            #region 大标题
            if (BluePrint_title_module != null)
            {
                #region anchor
                switch (BluePrint_MarkAnchors)
                {
                    case IsolateVisualModeMarkAnchor.左上:
                        BluePrint_title_module.TextStyleInfo.tmp_Set_Alignment(TmpContentAnchor.顶部靠左);
                        break;
                    case IsolateVisualModeMarkAnchor.左下:
                        BluePrint_title_module.TextStyleInfo.tmp_Set_Alignment(TmpContentAnchor.底部靠左);
                        break;
                    case IsolateVisualModeMarkAnchor.右上:
                        BluePrint_title_module.TextStyleInfo.tmp_Set_Alignment(TmpContentAnchor.顶部靠右);
                        break;
                    case IsolateVisualModeMarkAnchor.右下:
                        BluePrint_title_module.TextStyleInfo.tmp_Set_Alignment(TmpContentAnchor.底部靠右);
                        break;
                    case IsolateVisualModeMarkAnchor.中心:
                        BluePrint_title_module.TextStyleInfo.tmp_Set_Alignment(TmpContentAnchor.中心);
                        break;
                    case IsolateVisualModeMarkAnchor.左:
                        BluePrint_title_module.TextStyleInfo.tmp_Set_Alignment(TmpContentAnchor.左);
                        break;
                    case IsolateVisualModeMarkAnchor.右:
                        BluePrint_title_module.TextStyleInfo.tmp_Set_Alignment(TmpContentAnchor.右);
                        break;
                    case IsolateVisualModeMarkAnchor.上:
                        BluePrint_title_module.TextStyleInfo.tmp_Set_Alignment(TmpContentAnchor.顶部);
                        break;
                    case IsolateVisualModeMarkAnchor.下:
                        BluePrint_title_module.TextStyleInfo.tmp_Set_Alignment(TmpContentAnchor.底部);
                        break;
                }
                #endregion

                #region margin
                Vector4 margin = BluePrint_mark_margin;

                if (BluePrint_MarkAnchors == IsolateVisualModeMarkAnchor.中心 || BluePrint_MarkAnchors == IsolateVisualModeMarkAnchor.左 || BluePrint_MarkAnchors == IsolateVisualModeMarkAnchor.右)
                    BluePrint_title_module.TextStyleInfo.tmp_Set_MarginSet(new Vector4(margin.x, margin.y, margin.z, margin.w * BluePrint_mark_space));
                else
                    BluePrint_title_module.TextStyleInfo.tmp_Set_MarginSet(BluePrint_mark_margin);
                #endregion

                #region content
                BluePrint_title_module.tmp_Set_Content(BluePrint_title_content);
                #endregion

                #region size
                BluePrint_title_module.TextStyleInfo.tmp_Set_FontSize(BluePrint_mark_size);
                #endregion

                #region color
                BluePrint_title_module.TextStyleInfo.tmp_Set_FontColor(BluePrint_marktitle_color);
                BluePrint_title_module.color = BluePrint_marktitle_color;
                #endregion

                #region opacity
                Color cc = BluePrint_title_module.color;
                cc.a *= BluePrint_mark_opacity;
                BluePrint_title_module.TextStyleInfo.tmp_Set_FontColor(cc);
                #endregion
            }
            #endregion

            #region 小标题
            if (BluePrint_subtitle_module != null)
            {
                #region anchor
                TmpContentAnchor x_anchor = TmpContentAnchor.中心;
                switch (BluePrint_MarkAnchors)
                {
                    case IsolateVisualModeMarkAnchor.左上:
                        x_anchor = TmpContentAnchor.顶部靠左;
                        break;
                    case IsolateVisualModeMarkAnchor.左下:
                        x_anchor = TmpContentAnchor.底部靠左;
                        break;
                    case IsolateVisualModeMarkAnchor.右上:
                        x_anchor = TmpContentAnchor.顶部靠右;
                        break;
                    case IsolateVisualModeMarkAnchor.右下:
                        x_anchor = TmpContentAnchor.底部靠右;
                        break;
                    case IsolateVisualModeMarkAnchor.中心:
                        x_anchor = TmpContentAnchor.中心;
                        break;
                    case IsolateVisualModeMarkAnchor.左:
                        x_anchor = TmpContentAnchor.左;
                        break;
                    case IsolateVisualModeMarkAnchor.右:
                        x_anchor = TmpContentAnchor.右;
                        break;
                    case IsolateVisualModeMarkAnchor.上:
                        x_anchor = TmpContentAnchor.顶部;
                        break;
                    case IsolateVisualModeMarkAnchor.下:
                        x_anchor = TmpContentAnchor.底部;
                        break;
                }
                BluePrint_subtitle_module.TextStyleInfo.tmp_Set_Alignment(x_anchor);
                #endregion

                #region margin
                Vector4 margin = BluePrint_mark_margin;
                Vector4 x_margin = new Vector4();
                switch (BluePrint_MarkAnchors)
                {
                    case IsolateVisualModeMarkAnchor.左上:
                        x_margin = new Vector4(margin.x, margin.y + 80 * BluePrint_mark_space, margin.z, margin.w);
                        break;
                    case IsolateVisualModeMarkAnchor.左下:
                        x_margin = new Vector4(margin.x, margin.y, margin.z, margin.w + 80 * BluePrint_mark_space);
                        break;
                    case IsolateVisualModeMarkAnchor.右上:
                        x_margin = new Vector4(margin.x, margin.y + 80 * BluePrint_mark_space, margin.z, margin.w);
                        break;
                    case IsolateVisualModeMarkAnchor.右下:
                        x_margin = new Vector4(margin.x, margin.y, margin.z, margin.w + 80 * BluePrint_mark_space);
                        break;
                    case IsolateVisualModeMarkAnchor.中心:
                        x_margin = new Vector4(margin.x, margin.y + 110 * BluePrint_mark_space, margin.z, margin.w);
                        break;
                    case IsolateVisualModeMarkAnchor.左:
                        x_margin = new Vector4(margin.x, margin.y + 110 * BluePrint_mark_space, margin.z, margin.w);
                        break;
                    case IsolateVisualModeMarkAnchor.右:
                        x_margin = new Vector4(margin.x, margin.y + 110 * BluePrint_mark_space, margin.z, margin.w);
                        break;
                    case IsolateVisualModeMarkAnchor.上:
                        x_margin = new Vector4(margin.x, margin.y + 80 * BluePrint_mark_space, margin.z, margin.w);
                        break;
                    case IsolateVisualModeMarkAnchor.下:
                        x_margin = new Vector4(margin.x, margin.y, margin.z, margin.w + 80 * BluePrint_mark_space);
                        break;
                }
                BluePrint_subtitle_module.TextStyleInfo.tmp_Set_MarginSet(x_margin);
                #endregion

                #region content
                BluePrint_subtitle_module.tmp_Set_Content(BluePrint_subtitle_content);
                #endregion

                #region size
                BluePrint_subtitle_module.TextStyleInfo.tmp_Set_FontSize(BluePrint_mark_size * 0.65f);
                #endregion

                #region color
                BluePrint_subtitle_module.TextStyleInfo.tmp_Set_FontColor(BluePrint_marksubtitle_color);
                BluePrint_subtitle_module.color = BluePrint_marksubtitle_color;
                #endregion

                #region opacity
                Color cc = BluePrint_subtitle_module.color;
                cc.a *= BluePrint_mark_opacity;
                BluePrint_subtitle_module.TextStyleInfo.tmp_Set_FontColor(cc);
                #endregion
            }
            #endregion
        }

        #endregion

        #endregion

        #region 获取渲染特性
        public static T GetFeature<T>(string name) where T : ScriptableRendererFeature
        {
            UniversalRenderPipelineAsset urpAsset = GraphicsSettings.defaultRenderPipeline as UniversalRenderPipelineAsset;
            foreach (var rendererData in urpAsset.rendererDataList)
            {
                foreach (var feature in rendererData.rendererFeatures)
                {
                    if (feature is T && feature.name == name)
                    {
                        return (T)feature;
                    }
                }
            }

            return null;
        }
        #endregion
    }
}