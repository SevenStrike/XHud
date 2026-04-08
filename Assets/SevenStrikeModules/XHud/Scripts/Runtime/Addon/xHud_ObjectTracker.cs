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
    using SevenStrikeModules.XTween;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Events;

    /// <summary>
    /// 跟踪参数
    /// </summary>
    public struct TrackerArgs
    {
        /// <summary>
        /// 跟踪的物体
        /// </summary>
        public RectTransform SelfObject;
        /// <summary>
        /// 被跟踪的物体
        /// </summary>
        public Transform TargetObject;
        /// <summary>
        /// 偏移
        /// </summary>
        public Vector2 Offset;
        /// <summary>
        /// 平滑跟踪
        /// </summary>
        public bool UseSmoothTracker;
        /// <summary>
        /// 平滑时间
        /// </summary>
        public float SmoothTime;
    }

    /// <summary>
    /// 物体跟踪器，适用于在屏幕空间模式下将元素跟踪位置到场景物体的方法
    /// </summary>
    public class XHud_ObjectTracker : MonoBehaviour
    {
        [SerializeField]
        /// <summary>
        /// 跟踪物体
        /// </summary>
        public RectTransform SelfObject;
        [SerializeField]
        /// <summary>
        /// 相对父物体
        /// </summary>
        public RectTransform RelativeObject;
        [SerializeField]
        /// <summary>
        /// 被跟踪的物体
        /// </summary>
        public Transform TargetObject;
        /// <summary>
        /// 平滑跟踪
        /// </summary>
        public bool UseSmoothTracker;
        /// <summary>
        /// 平滑时间
        /// </summary>
        public float SmoothTime = 5;
        /// <summary>
        /// 位置偏移
        /// </summary>
        public Vector2 TrackerOffset;
        private Vector2 GameScreen;
        private Vector2 PositionWithConvert;

        public XTween_Interface twn_Offset;

        void Start()
        {

        }

        void Update()
        {
            Tracker_Update();
        }

        /// <summary>
        /// 跟踪物体
        /// </summary>
        private void Tracker_Update()
        {
            if (TargetObject == null)
                return;

            GameScreen = Camera.main.WorldToScreenPoint(TargetObject.transform.position);
            bool isRect = RectTransformUtility.ScreenPointToLocalPointInRectangle(RelativeObject, GameScreen, XHud_Manager.Instance.HudCamera, out PositionWithConvert);

            if (!UseSmoothTracker)
                SelfObject.anchoredPosition = PositionWithConvert + TrackerOffset;
            else
                SelfObject.anchoredPosition = Vector3.Lerp(SelfObject.anchoredPosition, PositionWithConvert + TrackerOffset, Time.deltaTime * SmoothTime);
        }

        /// <summary>
        /// 跟踪物体 - 快速对齐
        /// </summary>
        private void Tracker_Update_FastAlign()
        {
            if (TargetObject == null)
                return;
            GameScreen = Camera.main.WorldToScreenPoint(TargetObject.transform.position);
            SelfObject.anchoredPosition = PositionWithConvert + TrackerOffset;
        }

        /// <summary>
        /// 跟踪物体 -  设置跟踪 /  SelfObject =Hud元素，TargetObject = 场景物体，RelativeObject = Hud元素的父物体
        /// </summary>
        /// <param name="info">跟踪信息</param>
        public void Tracker_SetTrackerArgs(TrackerArgs info)
        {
            SelfObject = info.SelfObject;
            RelativeObject = (RectTransform)info.SelfObject.parent;
            TargetObject = info.TargetObject;
            TrackerOffset = info.Offset;
            Tracker_Update_FastAlign();

            StartCoroutine(Tracker_DelayEnableSmooth(info));
        }

        /// <summary>
        /// 跟踪物体 -  设置跟踪
        /// </summary>
        /// <param name="self">Hud元素</param>
        /// <param name="target">场景物体</param>
        /// <param name="relative">Hud元素的父物体</param>
        /// <param name="offset">偏移值</param>
        public void Tracker_SetTrackerArgs(RectTransform self, Transform target, RectTransform relative, Vector2 offset)
        {
            TrackerArgs arg = new TrackerArgs();

            arg.SelfObject = self;
            arg.TargetObject = target;
            arg.Offset = offset;

            SelfObject = arg.SelfObject;
            RelativeObject = (RectTransform)arg.SelfObject.parent;
            TargetObject = arg.TargetObject;
            TrackerOffset = arg.Offset;
            Tracker_Update_FastAlign();

            StartCoroutine(Tracker_DelayEnableSmooth(arg));
        }

        IEnumerator Tracker_DelayEnableSmooth(TrackerArgs info)
        {
            yield return new WaitForSeconds(0.5f);
            UseSmoothTracker = info.UseSmoothTracker;
            SmoothTime = info.SmoothTime;
        }

        /// <summary>
        /// 设置偏移
        /// </summary>
        /// <param name="offset">偏移值</param>
        public void Tracker_SetOffset(Vector2 offset)
        {
            TrackerOffset = offset;
        }

        /// <summary>
        /// 偏移值平滑到
        /// </summary>
        /// <param name="val">目标偏移值</param>
        /// <param name="dur">耗时</param>
        /// <param name="ease">缓动</param>
        /// <param name="delay">延迟</param>
        /// <param name="action_start">委托 - 开始时</param>
        /// <param name="action_end">委托 - 结束时</param>
        public void Tracker_SetSmoothOffset(Vector2 val, float dur, EaseMode ease, float delay, UnityAction action_start = null, UnityAction action_end = null)
        {
            if (twn_Offset != null && twn_Offset.IsActive)
                if (twn_Offset.IsPlaying)
                    twn_Offset.Kill();
            twn_Offset = XTween.To(() => TrackerOffset, x => TrackerOffset = x, val, dur).SetEase(ease).SetAutoKill(true).SetDelay(delay).OnStart(() =>
            {
                if (action_start != null)
                    action_start();
            }).OnComplete((d) =>
            {
                if (action_end != null)
                    action_end();
            });
        }
    }
}