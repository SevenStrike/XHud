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
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// 锚点结构 - 布局
    /// </summary>
    [System.Serializable]
    public class Anchor_Layout
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 锚点
        /// </summary>
        public RectTransform Anchor;
        /// <summary>
        /// 锚点类型
        /// </summary>
        public XHudAnchor Type;
        /// <summary>
        /// 标记物
        /// </summary>
        public Image Mark;
        /// <summary>
        /// 元素项
        /// </summary>
        public List<HudElementNode> HudElementInfos;

        /// <summary>
        /// 实例化锚点节点
        /// </summary>
        /// <param name="Name"></param>
        public Anchor_Layout(string Name)
        {
            switch (Name)
            {
                case "Anchor_U":
                    Type = XHudAnchor.上;
                    break;
                case "Anchor_D":
                    Type = XHudAnchor.下;
                    break;
                case "Anchor_L":
                    Type = XHudAnchor.左;
                    break;
                case "Anchor_R":
                    Type = XHudAnchor.右;
                    break;
                case "Anchor_C":
                    Type = XHudAnchor.中心;
                    break;
                case "Anchor_L_U":
                    Type = XHudAnchor.左上;
                    break;
                case "Anchor_L_D":
                    Type = XHudAnchor.左下;
                    break;
                case "Anchor_R_U":
                    Type = XHudAnchor.右上;
                    break;
                case "Anchor_R_D":
                    Type = XHudAnchor.右下;
                    break;
                case "Anchor_B":
                    Type = XHudAnchor.底层;
                    break;
                case "Anchor_T":
                    Type = XHudAnchor.顶层;
                    break;
            }
            this.Name = "锚点： " + Type.ToString();
        }

        /// <summary>
        /// 实例化锚点节点
        /// </summary>
        public Anchor_Layout()
        {
            this.Name = "锚点： " + Type.ToString();
        }
    }

    public partial class XHud_Manager : MonoBehaviour
    {
        [Tooltip("XHud画布 - 屏幕空间缩放器")]
        /// <summary>
        /// Hud画布 - 屏幕空间缩放器
        /// </summary>
        public CanvasScaler HudCanvasScaler;
        [Tooltip("XHud画布 - 屏幕空间缩放模式索引")]
        /// <summary>
        /// Hud画布 - 屏幕空间缩放模式索引
        /// </summary>
        public int CanvasScalerModeIndex;
        [Range(0, 1)]
        [Tooltip("高度优先匹配值")]
        /// <summary>
        /// 高度优先匹配值
        /// </summary>
        public float CanvasMatchDir;
        [Tooltip("当前屏幕分辨率尺寸")]
        /// <summary>
        /// 屏幕分辨率
        /// </summary>
        public Vector2 ScreenRes;
        [Tooltip("是否使用像素对齐")]
        /// <summary>
        /// 是否使用像素对齐
        /// </summary>
        public bool UsePerfectPixelUpdate;
        [Tooltip("是否使用运动停止后自动像素对齐")]
        /// <summary>
        /// 是否使用运动停止后自动像素对齐
        /// </summary>
        public bool UseAutoPerfectPixel;
        [Tooltip("XHud画布-屏幕锚点")]
        /// <summary>
        /// XHud画布-屏幕锚点
        /// </summary>
        public RectTransform HudCanvas_ScreenAnchor;
        [Tooltip("XHud画布-世界锚点")]
        /// <summary>
        /// XHud画布-世界锚点
        /// </summary>
        public RectTransform HudCanvas_WorldAnchor;
        [SerializeField]
        /// <summary>
        /// 支持世界UI
        /// </summary>
        public bool SupportWorldUI;
        [Tooltip("世界锚点列表")]
        /// <summary>
        /// 世界锚点列表
        /// </summary>
        public List<HudElementNode> Anchors_Layout_World = new List<HudElementNode>();
        [Tooltip("屏幕锚点列表")]
        /// <summary>
        /// 屏幕锚点列表
        /// </summary>
        public List<Anchor_Layout> Anchors_Layout_Screen = new List<Anchor_Layout>();
        [Tooltip("X：上边距 | Y：下边距 | Z：左边距 | W：右边距")]
        /// <summary>
        /// 屏幕边距
        /// </summary>
        public Vector4 Margins = Vector4.one * 10f;
        [Tooltip("水平边距")]
        /// <summary>
        /// 屏幕边距 - 水平
        /// </summary>
        public float MarginHorizontal = 1f;
        [Tooltip("垂直边距")]
        /// <summary>
        /// 屏幕边距 - 垂直
        /// </summary>
        public float MarginVertical = 1f;
        [Tooltip("边距倍增")]
        /// <summary>
        /// 主要方向锚点尺寸倍增
        /// </summary>
        public float MarginMultiply = 1f;
        [Tooltip("画布和相机的距离")]
        /// <summary>
        /// 画布和相机的距离
        /// </summary>
        public float CanvasDistance = 0.5f;
        [Tooltip("XHud画布 - 屏幕空间")]
        /// <summary>
        /// XHud画布 - 屏幕空间
        /// </summary>
        public Canvas HudCanvas_Screen;
        [Tooltip("XHud画布 - 世界空间")]
        /// <summary>
        /// XHud画布 - 世界空间
        /// </summary>
        public Canvas HudCanvas_World;
        [Tooltip("XHud画布锚点")]
        /// <summary>
        /// XHud画布锚点
        /// </summary>
        public CanvasAnchor HudCanvasAnchor = CanvasAnchor.CameraFar;
        [Tooltip("画布距离模式索引")]
        /// <summary>
        /// 画布距离模式索引
        /// </summary>
        public int HudCanvasAnchorIndex;

        /// <summary>
        /// 布局锚点更新
        /// 负责更新所有屏幕空间锚点的位置，根据配置的边距值动态计算每个锚点的坐标
        /// 
        /// 工作原理：
        /// 1. 设置屏幕空间画布的渲染模式为 ScreenSpaceCamera
        /// 2. 设置世界空间画布的渲染模式为 WorldSpace 并关联场景相机
        /// 3. 遍历 Anchors_Layout_Screen 列表中的所有锚点
        /// 4. 根据锚点名称（类型）计算其在屏幕上的位置
        /// 5. 更新锚点对应的标记物（Mark）的尺寸和颜色
        /// 
        /// 锚点类型与位置计算：
        /// - 底层/顶层/中心：位置为原点 (0, 0, 0)
        /// - 上：Y 轴负方向偏移（屏幕顶部）
        /// - 下：Y 轴正方向偏移（屏幕底部）
        /// - 左：X 轴正方向偏移（屏幕左侧）
        /// - 右：X 轴负方向偏移（屏幕右侧）
        /// - 左上/左下/右上/右下：组合偏移
        /// 
        /// 边距计算公式：
        /// 位置 = ± (边距 + 水平/垂直偏移) × 边距倍增系数
        /// 
        /// 使用场景：
        /// - UI 元素需要锚定在屏幕特定位置（如血条、技能栏、小地图）
        /// - 响应屏幕分辨率变化，自动调整 UI 元素位置
        /// - 统一管理所有 UI 锚点，便于整体调整边距
        /// 
        /// 调用频率：
        /// - 每帧在 XHud_Manager.Update() 中调用
        /// - 开销较小，仅更新固定数量的锚点（通常 10 个左右）
        /// 
        /// 注意事项：
        /// - 此方法影响所有已生成 UI 元素的父级锚点位置
        /// - 边距值（Margins）和倍增系数（MarginMultiply）可动态调整
        /// - 锚点标记物（Mark）仅用于编辑器可视化，运行时不影响功能
        /// </summary>
        public void hm_Layout_Update()
        {
            hm_Layout_CanvasDistance_Update();

            if (HudCanvas_Screen != null)
            {
                HudCanvas_Screen.renderMode = RenderMode.ScreenSpaceCamera;

                if (HudCamera != null)
                {
                    HudCanvas_Screen.worldCamera = HudCamera;
                }
            }

            if (HudCanvas_World != null)
            {
                HudCanvas_World.renderMode = RenderMode.WorldSpace;
                if (SceneCamera != null)
                {
                    HudCanvas_World.worldCamera = SceneCamera;
                }
            }

            if (Anchors_Layout_Screen != null)
            {
                for (int i = 0; i < Anchors_Layout_Screen.Count; i++)
                {
                    string sp_name = Anchors_Layout_Screen[i].Name;
                    RectTransform rect = Anchors_Layout_Screen[i].Anchor;

                    if (sp_name == "Anchor_B")
                    {
                        Anchors_Layout_Screen[i].Type = XHudAnchor.底层;
                        if (rect != null)
                            rect.anchoredPosition3D = new Vector3(0, 0, 0);
                    }
                    if (sp_name == "Anchor_U")
                    {
                        Anchors_Layout_Screen[i].Type = XHudAnchor.上;
                        if (rect != null)
                            rect.anchoredPosition3D = new Vector3(0, (-Margins.x - MarginVertical) * MarginMultiply, 0);
                    }
                    if (sp_name == "Anchor_D")
                    {
                        Anchors_Layout_Screen[i].Type = XHudAnchor.下;
                        if (rect != null)
                            rect.anchoredPosition3D = new Vector3(0, (Margins.y + MarginVertical) * MarginMultiply, 0);
                    }
                    if (sp_name == "Anchor_L")
                    {
                        Anchors_Layout_Screen[i].Type = XHudAnchor.左;
                        if (rect != null)
                            rect.anchoredPosition3D = new Vector3((Margins.z + MarginHorizontal) * MarginMultiply, 0, 0);
                    }
                    if (sp_name == "Anchor_R")
                    {
                        Anchors_Layout_Screen[i].Type = XHudAnchor.右;
                        if (rect != null)
                            rect.anchoredPosition3D = new Vector3((-Margins.w - MarginHorizontal) * MarginMultiply, 0, 0);
                    }
                    if (sp_name == "Anchor_C")
                    {
                        Anchors_Layout_Screen[i].Type = XHudAnchor.中心;
                        if (rect != null)
                            rect.anchoredPosition3D = new Vector3(0, 0, 0);
                    }
                    if (sp_name == "Anchor_L_U")
                    {
                        Anchors_Layout_Screen[i].Type = XHudAnchor.左上;
                        if (rect != null)
                            rect.anchoredPosition3D = new Vector3((Margins.z + MarginHorizontal) * MarginMultiply, (-Margins.x - MarginVertical) * MarginMultiply, 0);
                    }
                    if (sp_name == "Anchor_L_D")
                    {
                        Anchors_Layout_Screen[i].Type = XHudAnchor.左下;
                        if (rect != null)
                            rect.anchoredPosition3D = new Vector3((Margins.z + MarginHorizontal) * MarginMultiply, (Margins.y + MarginVertical) * MarginMultiply, 0);
                    }
                    if (sp_name == "Anchor_R_U")
                    {
                        Anchors_Layout_Screen[i].Type = XHudAnchor.右上;
                        if (rect != null)
                            rect.anchoredPosition3D = new Vector3((-Margins.w - MarginHorizontal) * MarginMultiply, (-Margins.x - MarginVertical) * MarginMultiply, 0);
                    }
                    if (sp_name == "Anchor_R_D")
                    {
                        Anchors_Layout_Screen[i].Type = XHudAnchor.右下;
                        if (rect != null)
                            rect.anchoredPosition3D = new Vector3((-Margins.w - MarginHorizontal) * MarginMultiply, (Margins.y + MarginVertical) * MarginMultiply, 0);
                    }
                    if (sp_name == "Anchor_T")
                    {
                        Anchors_Layout_Screen[i].Type = XHudAnchor.顶层;
                        if (rect != null)
                            rect.anchoredPosition3D = new Vector3(0, 0, 0);
                    }

                    Image rect_mark = Anchors_Layout_Screen[i].Mark;

                    if (rect_mark != null)
                    {
                        rect_mark.rectTransform.sizeDelta = Vector2.one * MarkSize;
                        rect_mark.color = Color_LayoutAnchorMark;
                        rect_mark.transform.SetAsLastSibling();
                    }
                }
            }

            hm_Layout_CanvasDistance(HudCanvasAnchor);
        }
        /// <summary>
        /// 设置Hud画布的锚点位置
        /// </summary>
        /// <param name="anchor">画布目标锚点</param>
        public void hm_Layout_CanvasDistance(CanvasAnchor anchor)
        {
            if (HudCamera == null)
                return;
            switch (anchor)
            {
                case CanvasAnchor.CameraNear:
                    CanvasDistance = HudCamera.nearClipPlane + 0.001f;
                    break;
                case CanvasAnchor.CameraFar:
                    CanvasDistance = HudCamera.farClipPlane - 0.001f;
                    break;
                case CanvasAnchor.Custom:

                    break;
            }
        }
        /// <summary>
        /// 获取布局边距
        /// </summary>
        /// <param name="anchor"></param>
        /// <returns></returns>
        public float hm_Layout_GetMargins(HudAnchorMargin anchor)
        {
            float val = 0;
            switch (anchor)
            {
                case HudAnchorMargin.上:
                    val = Margins.x;
                    break;
                case HudAnchorMargin.下:
                    val = Margins.y;
                    break;
                case HudAnchorMargin.左:
                    val = Margins.z;
                    break;
                case HudAnchorMargin.右:
                    val = Margins.w;
                    break;
            }
            return val;
        }
        /// <summary>
        /// 更新画布距离
        /// </summary>
        public void hm_Layout_CanvasDistance_Update()
        {
            if (HudCanvas_Screen == null)
                return;
            HudCanvas_Screen.planeDistance = CanvasDistance;
        }
        /// <summary>
        /// 画布像素对齐开关
        /// </summary>
        /// <param name="treeState">是否开启像素对齐</param>
        public void hm_UsePixelPerfect(bool state)
        {
            if (!UsePerfectPixelUpdate)
                return;
            HudCanvas_Screen.pixelPerfect = state;
        }
        /// <summary>
        /// 获取屏幕分辨率
        /// </summary>
        public Vector2 hm_GetScreenResolution()
        {
            return ScreenRes;
        }
        /// <summary>
        /// 获取画布分辨率
        /// </summary>
        public Vector2 hm_GetCanvasScalerScreenSize()
        {
            return CanvasScalerScreenSize;
        }
        /// <summary>
        /// 启动时根据配置开关，设置屏幕画布（Canvas）的像素完美对齐模式，避免模糊。
        /// </summary>
        public void hm_CanvasPixelPerfect_InitializeMode()
        {
            if (HudCanvas_Screen != null)
            {
                HudCanvas_Screen.pixelPerfect = UsePerfectPixelUpdate;
            }
        }
    }
}