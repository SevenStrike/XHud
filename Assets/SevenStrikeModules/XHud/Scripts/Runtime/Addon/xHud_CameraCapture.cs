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
        #region 字段 - 目标摄像机
        /// <summary>
        /// 目标摄像机
        /// </summary>
        public Camera target_camera;
        /// <summary>
        /// 截图尺寸模式
        /// </summary>
        public CaptureSize size_mode = CaptureSize.相机尺寸;
        /// <summary>
        /// 相机类型（场景相机 / UI 相机等）
        /// </summary>
        public CaptureCameraType mode = CaptureCameraType.场景相机;
        /// <summary>
        /// 截图文件格式
        /// </summary>
        public CaptureType type = CaptureType.PNG;
        /// <summary>
        /// 截图背景类型
        /// </summary>
        public CaptureBgType bg_type = CaptureBgType.天空盒;
        /// <summary>
        /// 背景透明度
        /// </summary>
        public int bg_alpha;
        #endregion

        #region 字段 - 截图尺寸
        /// <summary>
        /// 像素尺寸
        /// </summary>
        public Vector2 pixels = new Vector2(1024, 768);
        #endregion

        #region 字段 - 保存路径
        /// <summary>
        /// 保存路径
        /// </summary>
        public string cap_path = "StreamingAssets/";
        /// <summary>
        /// 文件名称
        /// </summary>
        public string cap_name = "Capture";
        #endregion

        #region 字段 - UI 相机背景
        /// <summary>
        /// UI 相机背景颜色
        /// </summary>
        public Color ui_camera_bg_color = Color.black;
        /// <summary>
        /// UI 相机背景颜色原始数据（用于截图后还原）
        /// </summary>
        public Color ui_camera_bg_color_original = Color.black;
        /// <summary>
        /// UI 相机清除标志原始数据（用于截图后还原）
        /// </summary>
        public CameraClearFlags ui_camera_clear_flags_original = CameraClearFlags.Skybox;
        #endregion

        #region 折叠
        public bool
          fold_options = true,
          fold_param = true,
          fold_camera = true,
          fold_based = true;

        public void GroupFold(bool state)
        {
            fold_options = state;
            fold_param = state;
            fold_camera = state;
            fold_based = state;
        }

        public void GroupFold(bool options = true, bool param = true, bool camera = true, bool based = true)
        {
            fold_options = options;
            fold_param = param;
            fold_camera = camera;
            fold_based = based;
        }
        #endregion

#if UNITY_EDITOR
        private void Reset()
        {
            target_camera = GetComponent<Camera>();
            pixels = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
        }
#endif

        /// <summary>
        /// 保存截图
        /// </summary>
        public void CaptureNow()
        {
            Vector2 size = pixels;
            if (size_mode == CaptureSize.相机尺寸)
            {
                size = new Vector2(target_camera.pixelWidth, target_camera.pixelHeight);
            }
            else if (size_mode == CaptureSize.屏幕分辨率)
            {
                size = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
            }
            string path = this.cap_path + @"\" + cap_name + "_" + mode.ToString() + "_" + RandomCode(4).ToUpper() + "." + type.ToString().ToLower();

            #region 截图前准备操作
            if (mode == CaptureCameraType.UI相机)
            {
                UniversalAdditionalCameraData universal = target_camera.GetComponent<UniversalAdditionalCameraData>();
                universal.renderType = CameraRenderType.Base;
                target_camera.clearFlags = CameraClearFlags.SolidColor;
                ui_camera_bg_color_original = target_camera.backgroundColor;
                target_camera.backgroundColor = ui_camera_bg_color;
            }

            if (mode == CaptureCameraType.场景相机)
            {
                ui_camera_clear_flags_original = target_camera.clearFlags;
                ui_camera_bg_color_original = target_camera.backgroundColor;
                switch (bg_type)
                {
                    case CaptureBgType.天空盒:
                        target_camera.clearFlags = CameraClearFlags.Skybox;
                        break;
                    case CaptureBgType.颜色:
                        target_camera.clearFlags = CameraClearFlags.SolidColor;
                        target_camera.backgroundColor = ui_camera_bg_color;
                        break;
                    case CaptureBgType.透明:
                        target_camera.clearFlags = CameraClearFlags.Depth;
                        target_camera.backgroundColor = new Color(0, 0, 0, 0);
                        break;
                }
            }
            #endregion

            ExportTexture(path, CameraCaptureTool(target_camera, (int)size.x, (int)size.y), type, () =>
            {
                // 截图完成后的操作
                if (mode == CaptureCameraType.UI相机)
                {
                    UniversalAdditionalCameraData universal = target_camera.GetComponent<UniversalAdditionalCameraData>();
                    universal.renderType = CameraRenderType.Overlay;
                    target_camera.backgroundColor = ui_camera_bg_color_original;
                    target_camera.clearFlags = CameraClearFlags.Depth;
                }
                if (mode == CaptureCameraType.场景相机)
                {
                    target_camera.backgroundColor = ui_camera_bg_color_original;
                    target_camera.clearFlags = ui_camera_clear_flags_original;
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