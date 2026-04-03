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
    using SevenStrikeModules.XTween;
    using UnityEngine;

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
        public EaseMode Ease = EaseMode.InOutCubic;

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
        public EaseMode Ease = EaseMode.InOutCubic;

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
        public EaseMode Ease = EaseMode.InOutCubic;

        public void CopyData(MotionNode_Alpha original)
        {
            Duration = original.Duration;
            Delay = original.Delay;
            Curve = original.Curve;
            CurveName = original.CurveName;
            Ease = original.Ease;
        }
    }
}