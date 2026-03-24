namespace SevenStrikeModules.XHud.Hud
{
    using DG.Tweening;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.Rendering.Universal;

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
    public class xHud_ObjectTracker : MonoBehaviour
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

        public Tweener twn_Offset;

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
            bool isRect = RectTransformUtility.ScreenPointToLocalPointInRectangle(RelativeObject, GameScreen, xHud_Manager.Instance.HudCamera, out PositionWithConvert);

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
        /// <param tweenName="info">跟踪信息</param>
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
        /// <param tweenName="self">Hud元素</param>
        /// <param tweenName="target">场景物体</param>
        /// <param tweenName="relative">Hud元素的父物体</param>
        /// <param tweenName="offset">偏移值</param>
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
        /// <param tweenName="offset">偏移值</param>
        public void Tracker_SetOffset(Vector2 offset)
        {
            TrackerOffset = offset;
        }

        /// <summary>
        /// 偏移值平滑到
        /// </summary>
        /// <param tweenName="val">目标偏移值</param>
        /// <param tweenName="dur">耗时</param>
        /// <param tweenName="ease">缓动</param>
        /// <param tweenName="delay">延迟</param>
        /// <param tweenName="action_start">委托 - 开始时</param>
        /// <param tweenName="action_end">委托 - 结束时</param>
        public void Tracker_SetSmoothOffset(Vector2 val, float dur, Ease ease, float delay, UnityAction action_start = null, UnityAction action_end = null)
        {
            if (twn_Offset != null && twn_Offset.active)
                if (twn_Offset.IsPlaying())
                    twn_Offset.Kill();
            twn_Offset = DOTween.To(() => TrackerOffset, x => TrackerOffset = x, val, dur).SetEase(ease).SetAutoKill(true).SetDelay(delay).OnStart(() =>
            {
                if (action_start != null)
                    action_start();
            }).OnComplete(() =>
            {
                if (action_end != null)
                    action_end();
            });
        }
    }
}