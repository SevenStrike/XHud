namespace SevenStrikeModules.XHud
{
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
    }
}