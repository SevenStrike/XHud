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
    using System.IO;
    using System.Text;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.Rendering.Universal;
    using Random = System.Random;

    public class XHud_CameraCapture : MonoBehaviour
    {
        // 目标摄像机
        public Camera TargetCamera;
        // 截图尺寸
        public CaptureSize x_sizemode = CaptureSize.相机尺寸;
        // 相机类型
        public CaptureCameraType x_mode = CaptureCameraType.场景相机;
        // 相机类型
        public CaptureType x_type = CaptureType.PNG;
        // 相机类型
        public CaptureBgType x_bgtype = CaptureBgType.天空盒;

        public int x_bg_alpha;

        // 像素尺寸
        public Vector2 Pixels = new Vector2(1024, 768);

        // 保存路径
        public string x_path = "StreamingAssets/";
        // 文件名称
        public string x_name = "Capture";

        // UI相机背景颜色
        public Color x_UICamera_bgcolor = Color.black;
        // UI相机原始数据
        public Color x_UICamera_Bgcolor_Original = Color.black;
        public CameraClearFlags x_UICamera_ClearFlags_Original = CameraClearFlags.Skybox;

        public bool
          fold_options = true,
          fold_param = true,
          fold_camera = true,
          fold_based = true;

#if UNITY_EDITOR
        private void Reset()
        {
            TargetCamera = GetComponent<Camera>();
            Pixels = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
        }
#endif

        /// <summary>
        /// 保存截图
        /// </summary>
        public void CaptureNow()
        {
            Vector2 size = Pixels;
            if (x_sizemode == CaptureSize.相机尺寸)
            {
                size = new Vector2(TargetCamera.pixelWidth, TargetCamera.pixelHeight);
            }
            else if (x_sizemode == CaptureSize.屏幕分辨率)
            {
                size = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
            }
            string path = x_path + @"\" + x_name + "_" + x_mode.ToString() + "_" + RandomCode(4).ToUpper() + "." + x_type.ToString().ToLower();

            #region 截图前准备操作
            if (x_mode == CaptureCameraType.UI相机)
            {
                UniversalAdditionalCameraData universal = TargetCamera.GetComponent<UniversalAdditionalCameraData>();
                universal.renderType = CameraRenderType.Base;
                TargetCamera.clearFlags = CameraClearFlags.SolidColor;
                x_UICamera_Bgcolor_Original = TargetCamera.backgroundColor;
                TargetCamera.backgroundColor = x_UICamera_bgcolor;
            }

            if (x_mode == CaptureCameraType.场景相机)
            {
                x_UICamera_ClearFlags_Original = TargetCamera.clearFlags;
                x_UICamera_Bgcolor_Original = TargetCamera.backgroundColor;
                switch (x_bgtype)
                {
                    case CaptureBgType.天空盒:
                        TargetCamera.clearFlags = CameraClearFlags.Skybox;
                        break;
                    case CaptureBgType.颜色:
                        TargetCamera.clearFlags = CameraClearFlags.SolidColor;
                        TargetCamera.backgroundColor = x_UICamera_bgcolor;
                        break;
                    case CaptureBgType.透明:
                        TargetCamera.clearFlags = CameraClearFlags.Depth;
                        TargetCamera.backgroundColor = new Color(0, 0, 0, 0);
                        break;
                }
            }
            #endregion

            ExportTexture(path, CameraCaptureTool(TargetCamera, (int)size.x, (int)size.y), x_type, () =>
            {
                // 截图完成后的操作
                if (x_mode == CaptureCameraType.UI相机)
                {
                    UniversalAdditionalCameraData universal = TargetCamera.GetComponent<UniversalAdditionalCameraData>();
                    universal.renderType = CameraRenderType.Overlay;
                    TargetCamera.backgroundColor = x_UICamera_Bgcolor_Original;
                    TargetCamera.clearFlags = CameraClearFlags.Depth;
                }
                if (x_mode == CaptureCameraType.场景相机)
                {
                    TargetCamera.backgroundColor = x_UICamera_Bgcolor_Original;
                    TargetCamera.clearFlags = x_UICamera_ClearFlags_Original;
                }
            });
        }

        /// <summary>
        /// 相机截图
        /// </summary>
        /// <param name="camera">目标相机</param>
        /// <returns></returns>
        public static Texture2D CameraCaptureTool(Camera camera)
        {
            return CameraCaptureTool(camera, Screen.width, Screen.height);
        }

        /// <summary>
        /// 相机截图
        /// </summary>
        /// <param name="camera">相机</param>
        /// <param name="width">截图宽度</param>
        /// <param name="height">截图高度</param>
        /// <returns></returns>
        public static Texture2D CameraCaptureTool(Camera camera, int width, int height)
        {
            RenderTexture rt = new RenderTexture(width, height, 0);
            rt.depth = 24;
            rt.antiAliasing = 8;
            camera.targetTexture = rt;
            camera.RenderDontRestore();
            RenderTexture.active = rt;
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false, true);
            Rect rect = new Rect(0, 0, width, height);
            texture.ReadPixels(rect, 0, 0);
            texture.filterMode = FilterMode.Point;
            texture.Apply();
            camera.targetTexture = null;
            RenderTexture.active = null;
            DestroyImmediate(rt, true);

            return texture;
        }

        /// <summary>
        /// 保存截图
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="texture">贴图</param>
        /// <param name="filetype">文件类型</param>
        /// <param name="act_captured">动作事件回调</param>
        public static void ExportTexture(string path, Texture2D texture, CaptureType filetype, UnityAction act_captured)
        {
            switch (filetype)
            {
                case CaptureType.JPG:
                    File.WriteAllBytes(path, texture.EncodeToJPG());
                    break;
                case CaptureType.PNG:
                    File.WriteAllBytes(path, texture.EncodeToPNG());
                    break;
                case CaptureType.TGA:
                    File.WriteAllBytes(path, texture.EncodeToTGA());
                    break;
            }
            if (act_captured != null)
            {
                act_captured();
            }
        }

        /// <summary>
        /// 生成随机码
        /// </summary>
        /// <param name="length">随机码长度</param>
        /// <returns></returns>
        static string RandomCode(int length)
        {
            // 定义字符范围
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            StringBuilder sb = new StringBuilder();

            // 生成随机字符串
            for (int i = 0; i < length; i++)
            {
                int index = random.Next(chars.Length);
                sb.Append(chars[index]);
            }

            return sb.ToString();
        }
    }
}