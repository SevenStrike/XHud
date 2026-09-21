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
    using UnityEngine.Rendering.Universal;

    public partial class XHud_Manager : MonoBehaviour
    {
        [Tooltip("场景相机")]
        /// <summary>
        /// 场景相机
        /// </summary>
        public Camera SceneCamera;
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

        #region 相机
        /// <summary>
        /// 设置相机剪切范围
        /// </summary>
        /// <param name="near">近距</param>
        /// <param name="far">远距</param>
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
        /// <param name="treeState">投影方式</param>
        public void hm_CameraOrthographicProjection(bool state)
        {
            if (HudCamera == null)
                return;
            HudCamera.orthographic = state;
        }
        /// <summary>
        /// 相机正交投影尺寸
        /// </summary>
        /// <param name="size">相机正交尺寸</param>
        public void hm_CameraOrthographicSize(float size)
        {
            if (HudCamera == null)
                return;
            HudCamera.orthographicSize = size;
        }
        /// <summary>
        /// 相机透视投影尺寸
        /// </summary>
        /// <param name="fov">透视角焦距</param>
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
        /// <param name="cam">场景相机</param>
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

        /// <summary>
        /// 相机参数更新
        /// 同步所有相机相关配置到实际的相机组件，确保 UI 相机和场景相机的渲染设置与配置一致
        /// 
        /// 工作原理：
        /// 1. 更新 UI 相机的裁剪范围（近平面/远平面）
        /// 2. 更新 UI 相机的投影模式（正交/透视）
        /// 3. 更新正交相机的尺寸（正交模式下有效）
        /// 4. 更新透视相机的视野角度（透视模式下有效）
        /// 5. 将 UI 相机添加到场景相机的渲染堆栈中
        /// 
        /// 为什么需要这个方法？
        /// - 允许开发者在 Inspector 中实时调整相机参数
        /// - 确保 UI 相机和场景相机的渲染关系正确
        /// - 支持运行时动态切换相机模式（如正交/透视切换）
        /// 
        /// UI 相机的作用：
        /// - 专门渲染 UI 元素的相机，独立于场景相机
        /// - 通过相机堆栈（Camera Stack）与场景相机叠加渲染
        /// - 确保 UI 始终显示在场景之上
        /// 
        /// 调用频率：
        /// - 每帧在 XHud_Manager.Update() 中调用
        /// - 开销极小，仅做属性赋值和状态检查
        /// 
        /// 注意事项：
        /// - 此方法会覆盖相机组件的所有设置
        /// - 如果需要在运行时动态修改相机参数，应修改对应的配置字段
        /// - 场景相机的相机堆栈需要确保 UI 相机被正确添加
        /// </summary>
        public void hm_CameraUpdate()
        {
            // ========== 1. 更新相机裁剪范围 ==========
            // 设置 UI 相机的近裁剪平面和远裁剪平面
            // 近裁剪平面：相机最近渲染距离，UI 相机通常设为较小的值（如 0.01）
            // 远裁剪平面：相机最远渲染距离，UI 相机通常设为较小的值（如 1）
            hm_CameraCutterRange(CameraCutter_Near, CameraCutter_Far);

            // ========== 2. 更新相机投影模式 ==========
            // 正交模式（Orthographic）：适用于 2D UI，物体大小不随距离变化
            // 透视模式（Perspective）：适用于 3D UI，有近大远小效果
            hm_CameraOrthographicProjection(CameraOthograpicMode);

            // ========== 3. 更新正交相机尺寸 ==========
            // 仅在正交模式下生效
            // 控制正交相机的视野范围，值越大看到的区域越大
            hm_CameraOrthographicSize(CameraOrthographicSize);

            // ========== 4. 更新透视相机视野角度 ==========
            // 仅在透视模式下生效
            // 控制透视相机的视野范围，值越大看到的范围越广
            hm_CameraPerspectiveFov(CameraFov);

            // ========== 5. 检查并添加相机堆栈 ==========
            // 将 UI 相机添加到场景相机的渲染堆栈中
            // 确保 UI 相机与场景相机叠加渲染
            // 场景相机先渲染场景，UI 相机后渲染 UI
            hm_SceneCam_CheckStack(SceneCamera);
        }

        /// <summary>
        /// 为指定的相机堆栈添加Hud叠加层
        /// </summary>
        /// <param name="uac"></param>
        public void hm_AssignedCameraStack(UniversalAdditionalCameraData uac)
        {
            if (HudCamera == null)
                if (UseDebug)
                    XGUI_Utilitys.Console("XHud - 管理器通知", "当前HudCamera为空，无法为指定的相机堆栈添加Hud叠加层", XGUIMsgState.警告);
            if (!uac.cameraStack.Contains(HudCamera))
            {
                uac.cameraStack.Add(HudCamera);
            }
        }
    }
}