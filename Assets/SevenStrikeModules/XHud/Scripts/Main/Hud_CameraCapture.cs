namespace SevenStrikeModules.XHud.Hud
{
    using UnityEngine;
    using System.IO;
    using UnityEngine.Rendering.Universal;
    using UnityEngine.Events;
    using System.Text;
    using Random = System.Random;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    public class Hud_CameraCapture : MonoBehaviour
    {
        /// <summary>
        /// 截图格式
        /// </summary>
        public enum CaptureType
        {
            JPG = 0,
            PNG = 1,
            TGA = 2
        }

        /// <summary>
        /// 截图尺寸
        /// </summary>
        public enum CaptureSize
        {
            相机尺寸 = 0,
            屏幕分辨率 = 1,
            固定尺寸 = 2
        }

        /// <summary>
        /// 截图背景
        /// </summary>
        public enum CaptureBgType
        {
            天空盒 = 0,
            颜色 = 1,
            透明 = 2,
        }

        /// <summary>
        /// 相机类型
        /// </summary>
        public enum CaptureCameraType
        {
            场景相机 = 0,
            UI相机 = 1
        }

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
                util_Dashboard.HudManagerGet().BlurMask.enabled = false;
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
                    util_Dashboard.HudManagerGet().BlurMask.enabled = true;
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
        /// <param tweenName="camera">目标相机</param>
        /// <returns></returns>
        public static Texture2D CameraCaptureTool(Camera camera)
        {
            return CameraCaptureTool(camera, Screen.width, Screen.height);
        }

        /// <summary>
        /// 相机截图
        /// </summary>
        /// <param tweenName="camera">相机</param>
        /// <param tweenName="width">截图宽度</param>
        /// <param tweenName="height">截图高度</param>
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
        /// <param tweenName="path">路径</param>
        /// <param tweenName="texture">贴图</param>
        /// <param tweenName="filetype">文件类型</param>
        /// <param tweenName="act_captured">动作事件回调</param>
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
        /// <param tweenName="length">随机码长度</param>
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