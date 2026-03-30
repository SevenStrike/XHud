namespace SevenStrikeModules.XHud
{
    using SevenStrikeModules.XHud.Enums;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

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
        /// 根据锚点类型获取锚点根物体
        /// </summary>
        /// <param name="anchor"></param>
        /// <returns></returns>
        public RectTransform hm_Layout_GetAnchor(XHudAnchor anchor)
        {
            RectTransform rect = null;
            for (int i = 0; i < Anchors_Layout_Screen.Count; i++)
            {
                if (Anchors_Layout_Screen[i].Type == anchor)
                {
                    rect = Anchors_Layout_Screen[i].Anchor;
                    break;
                }
            }
            return rect;
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
    }
}