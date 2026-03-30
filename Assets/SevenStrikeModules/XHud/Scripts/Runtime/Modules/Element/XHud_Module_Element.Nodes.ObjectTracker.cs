namespace SevenStrikeModules.XHud
{
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.Utilitys;
    using UnityEngine;

    public partial class XHud_Module_Element : MonoBehaviour
    {
        [SerializeField]
        /// <summary>
        /// 物体跟踪器
        /// </summary>
        public XHud_ObjectTracker ObjectTracker;

        /// <summary>
        /// Hud跟踪器 - 在ScreenCamera模式下设定跟踪目标
        /// </summary>
        /// <param name="self">跟踪目标</param>
        /// <param name="target">被跟踪目标</param>
        /// <param name="offset">偏移</param>
        /// <param name="smoottraker">平滑跟踪</param>
        /// <param name="smoottime">平滑事件</param>
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
                XHud_Utilitys.Func_PrintInfo("元素控件通知", "创建场景物体跟踪器！", HudMsgState.通知);
        }
        /// <summary>
        /// Hud跟踪器 - 创建
        /// </summary>
        /// <param name="info">跟踪通知</param>
        public void element_ObjectTracker_Create(TrackerArgs info)
        {
            ///---创建跟踪器
            element_ObjectTracker_Create();
            ///---设定跟踪器的参数
            ObjectTracker.Tracker_SetTrackerArgs(info);

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("元素控件通知", "创建场景物体跟踪器！", HudMsgState.通知);
        }
        /// <summary>
        /// 创建物体跟踪器
        /// </summary>
        /// <returns></returns>
        private XHud_ObjectTracker element_ObjectTracker_Create()
        {
            ObjectTracker = gameObject.AddComponent<XHud_ObjectTracker>();
            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("元素控件通知", "创建场景物体跟踪器！", HudMsgState.通知);
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
                XHud_Utilitys.Func_PrintInfo("元素控件通知", "移除场景物体跟踪器！", HudMsgState.通知);
        }
    }
}