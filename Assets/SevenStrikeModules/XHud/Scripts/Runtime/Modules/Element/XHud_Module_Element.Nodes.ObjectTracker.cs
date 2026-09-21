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
    using SevenStrikeModules.XGUI.Runtime;
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
                XGUI_Utilitys.Console("XHud - 元素控件通知", "创建场景物体跟踪器！", XGUIMsgState.通知);
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
                XGUI_Utilitys.Console("XHud - 元素控件通知", "创建场景物体跟踪器！", XGUIMsgState.通知);
        }
        /// <summary>
        /// 创建物体跟踪器
        /// </summary>
        /// <returns></returns>
        private XHud_ObjectTracker element_ObjectTracker_Create()
        {
            ObjectTracker = gameObject.AddComponent<XHud_ObjectTracker>();
            if (DebugState)
                XGUI_Utilitys.Console("XHud - 元素控件通知", "创建场景物体跟踪器！", XGUIMsgState.通知);
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
                XGUI_Utilitys.Console("XHud - 元素控件通知", "移除场景物体跟踪器！", XGUIMsgState.通知);
        }
    }
}