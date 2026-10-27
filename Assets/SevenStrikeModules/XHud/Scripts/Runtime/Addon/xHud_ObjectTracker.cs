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
        #region 字段 - 跟踪目标
        /// <summary>
        /// 跟踪物体（自身）
        /// </summary>
        [SerializeField]
        public RectTransform self_object;
        /// <summary>
        /// 相对父物体
        /// </summary>
        [SerializeField]
        public RectTransform relative_object;
        /// <summary>
        /// 被跟踪的物体
        /// </summary>
        [SerializeField]
        public Transform target_object;
        #endregion

        #region 字段 - 平滑跟踪
        /// <summary>
        /// 是否启用平滑跟踪
        /// </summary>
        public bool use_smooth_tracker;
        /// <summary>
        /// 平滑时间
        /// </summary>
        public float smooth_time = 5;
        /// <summary>
        /// 位置偏移
        /// </summary>
        public Vector2 tracker_offset;
        #endregion

        #region 字段 - 运行时缓存
        /// <summary>
        /// 自身在屏幕空间中的位置缓存
        /// </summary>
        private Vector2 game_screen;
        /// <summary>
        /// 换算后的目标位置缓存
        /// </summary>
        private Vector2 position_with_convert;
        #endregion

        #region 字段 - 缓动接口
        /// <summary>
        /// 偏移量的缓动接口
        /// </summary>
        public XTween_Interface twn_offset;
        #endregion

        #region 折叠
        public bool
            fold_param = true,
            fold_option = true,
            fold_based = true;

        public void GroupFold(bool state)
        {
            fold_param = state;
            fold_option = state;
            fold_based = state;
        }

        public void GroupFold(bool param = true, bool option = true, bool based = true)
        {
            fold_param = param;
            fold_option = option;
            fold_based = based;
        }
        #endregion

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
            if (target_object == null)
                return;

            game_screen = Camera.main.WorldToScreenPoint(target_object.transform.position);
            bool isRect = RectTransformUtility.ScreenPointToLocalPointInRectangle(relative_object, game_screen, XHud_Manager.instance.camera_ui, out position_with_convert);

            if (!use_smooth_tracker)
                self_object.anchoredPosition = position_with_convert + tracker_offset;
            else
                self_object.anchoredPosition = Vector3.Lerp(self_object.anchoredPosition, position_with_convert + tracker_offset, Time.deltaTime * smooth_time);
        }

        /// <summary>
        /// 跟踪物体 - 快速对齐
        /// </summary>
        private void Tracker_Update_FastAlign()
        {
            if (target_object == null)
                return;
            game_screen = Camera.main.WorldToScreenPoint(target_object.transform.position);
            self_object.anchoredPosition = position_with_convert + tracker_offset;
        }

        /// <summary>
        /// 跟踪物体 -  设置跟踪 /  SelfObject =Hud元素，TargetObject = 场景物体，RelativeObject = Hud元素的父物体
        /// </summary>
        /// <param name="info">跟踪信息</param>
        public void Tracker_SetTrackerArgs(TrackerArgs info)
        {
            self_object = info.SelfObject;
            relative_object = (RectTransform)info.SelfObject.parent;
            target_object = info.TargetObject;
            tracker_offset = info.Offset;
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

            self_object = arg.SelfObject;
            relative_object = (RectTransform)arg.SelfObject.parent;
            target_object = arg.TargetObject;
            tracker_offset = arg.Offset;
            Tracker_Update_FastAlign();

            StartCoroutine(Tracker_DelayEnableSmooth(arg));
        }

        IEnumerator Tracker_DelayEnableSmooth(TrackerArgs info)
        {
            yield return new WaitForSeconds(0.5f);
            use_smooth_tracker = info.UseSmoothTracker;
            smooth_time = info.SmoothTime;
        }

        /// <summary>
        /// 设置偏移
        /// </summary>
        /// <param name="offset">偏移值</param>
        public void Tracker_SetOffset(Vector2 offset)
        {
            tracker_offset = offset;
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
            if (twn_offset != null && twn_offset.IsActive)
                if (twn_offset.IsPlaying)
                    twn_offset.Kill();
            twn_offset = XTween.To(() => tracker_offset, x => tracker_offset = x, val, dur).SetEase(ease).SetAutoKill(true).SetDelay(delay).OnStart(() =>
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