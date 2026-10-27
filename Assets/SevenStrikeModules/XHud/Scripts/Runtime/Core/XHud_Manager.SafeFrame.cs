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
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// 锚点结构 - 安全框
    /// </summary>
    [System.Serializable]
    public class Safe_Structure
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
        /// 锚点坐标
        /// </summary>
        public Vector2 Pivot;

        /// <summary>
        /// 实例化锚点节点
        /// </summary>
        /// <param name="Name"></param>
        public Safe_Structure(string Name)
        {
            switch (Name)
            {
                case "Anchor_U":
                    Type = XHudAnchor.上;
                    Pivot.Set(0.5f, 1f);
                    break;
                case "Anchor_D":
                    Type = XHudAnchor.下;
                    Pivot.Set(0.5f, 0f);
                    break;
                case "Anchor_L":
                    Type = XHudAnchor.左;
                    Pivot.Set(0f, 0.5f);
                    break;
                case "Anchor_R":
                    Type = XHudAnchor.右;
                    Pivot.Set(1f, 0.5f);
                    break;
                case "Anchor_C":
                    Type = XHudAnchor.中心;
                    Pivot.Set(0.5f, 0.5f);
                    break;
                case "Anchor_L_U":
                    Type = XHudAnchor.左上;
                    Pivot.Set(0f, 1f);
                    break;
                case "Anchor_L_D":
                    Type = XHudAnchor.左下;
                    Pivot.Set(0f, 0f);
                    break;
                case "Anchor_R_U":
                    Type = XHudAnchor.右上;
                    Pivot.Set(1f, 1f);
                    break;
                case "Anchor_R_D":
                    Type = XHudAnchor.右下;
                    Pivot.Set(1f, 0f);
                    break;
                case "Anchor_B":
                    Type = XHudAnchor.底层;
                    Pivot.Set(0.5f, 0.5f);
                    break;
                case "Anchor_T":
                    Type = XHudAnchor.顶层;
                    Pivot.Set(0.5f, 0.5f);
                    break;
            }
            this.Name = "锚点： " + Type.ToString();
        }
    }

    /// <summary>
    /// 安全框 => 框线
    /// </summary>
    [System.Serializable]
    public class Safe_FrameLine
    {
        public Image Frame;
        public XHudAnchor Type;
    }

    /// <summary>
    /// 安全框 => 中心标记物
    /// </summary>
    [System.Serializable]
    public class Safe_CenterMark
    {
        public List<Image> Edge = new List<Image>();
        public Image Center;
    }

    /// <summary>
    /// 安全框 => 分割线
    /// </summary>
    [System.Serializable]
    public class Safe_Seperater
    {
        public Image Line;
        public XHudAnchor Type;
    }

    public partial class XHud_Manager : MonoBehaviour
    {
        /// <summary>
        /// 锚点可视化尺寸
        /// </summary>
        public float sf_mark_size = 10f;
        /// <summary>
        /// 是否在hierarchy中显示安全框结构
        /// </summary>
        public bool sf_structure_displayer;
        [Tooltip("安全框锚点物体")]
        /// <summary>
        /// 安全框锚点物体
        /// </summary>
        public Transform sf_safeframe;
        [Tooltip("锚点颜色 - 角点")]
        /// <summary>
        /// 锚点颜色 - 角点
        /// </summary>
        public Color sf_color_layout_anchor_mark = Color.white;
        [Tooltip(" 锚点颜色 - 中心点")]
        /// <summary>
        /// 锚点颜色 - 中心点
        /// </summary>
        public Color sf_color_center_mark = XHud_Dashboard.themes_get_color(ThemeColorType.primary);
        [Tooltip("安全框颜色")]
        /// <summary>
        /// 安全框颜色
        /// </summary>
        public Color sf_color_frame_line = Color.white * 0.7f;
        [Tooltip("锚点颜色 - 主要方向")]
        /// <summary>
        /// 锚点颜色 - 主要方向
        /// </summary>
        public Color sf_color_seperater_line = XHud_Dashboard.themes_get_color(ThemeColorType.primary);
        [Tooltip("安全框粗细")]
        /// <summary>
        /// 安全框粗细
        /// </summary>
        public float sf_safe_frame_line_width = 0.5f;
        [Tooltip("安全框元素组")]
        /// <summary>
        /// 安全框元素组
        /// </summary>
        public Safe_FrameLine[] sf_framelines;
        [Tooltip("安全框边距")]
        /// <summary>
        /// 安全框边距
        /// </summary>
        public Vector2 sf_line_margins = Vector2.one * 50f;
        [Tooltip("安全框中心线组")]
        /// <summary>
        /// 安全框中心线组
        /// </summary>
        public Safe_Seperater[] sf_seperaters;
        [Tooltip("安全框中心线长度")]
        /// <summary>
        /// 安全框中心线长度
        /// </summary>
        public float sf_seperater_length = 30f;
        [Tooltip("X：水平边距 | Y：垂直边距")]
        /// <summary>
        /// 中心标记物
        /// </summary>
        public Safe_CenterMark sf_center_marks;
        [Tooltip("安全框中心标记 - 间距")]
        /// <summary>
        /// 安全框中心标记 - 间距
        /// </summary>
        public float sf_center_mark_distance = 40f;
        [Tooltip("安全框中心标记 - 宽度")]
        /// <summary>
        /// 安全框中心标记 - 宽度
        /// </summary>
        public float sf_center_mark_width = 1f;
        [Tooltip("安全框中心标记 - 长度")]
        /// <summary>
        /// 安全框中心标记 - 长度
        /// </summary>
        public float sf_center_mark_length = 12f;
        [Tooltip("画布尺寸")]
        /// <summary>
        /// 画布尺寸
        /// </summary>
        public Vector2 sf_canvas_scaler_screen_size;
        [Tooltip("是否使用了布局可视化")]
        /// <summary>
        /// 是否使用了布局可视化
        /// </summary>
        public bool sf_use;

        /// <summary>
        /// 安全框更新
        /// 负责更新安全框（Safe Frame）的所有视觉元素，包括框线、分割线和中心标记
        /// 
        /// 工作原理：
        /// 1. 检查安全框功能是否开启（UseSafeFrame）
        /// 2. 调用 hm_Safe_ClampValue() 限制所有安全框参数的有效范围
        /// 3. 调用 hm_Safe_FrameLine_Update() 更新四条框线（上、下、左、右）
        /// 4. 调用 hm_Safe_Seperater_Update() 更新四条分割线（上、下、左、右）
        /// 5. 调用 hm_Safe_CenterMark_Update() 更新中心标记物（中心点和四个方向标记）
        /// 
        /// 什么是安全框（Safe Frame）？
        /// - 用于 UI 设计时的视觉参考框架
        /// - 标记屏幕的安全区域，确保 UI 元素不被裁剪
        /// - 帮助设计师将重要内容放置在安全区域内
        /// 
        /// 安全框的组成部分：
        /// 1. 框线（FrameLine）：四条边框线，定义安全区域的边界
        /// 2. 分割线（Seperater）：从边框中心延伸的辅助线，用于对齐
        /// 3. 中心标记（CenterMark）：屏幕中心的十字标记，包含中心点和四个方向指示
        /// 
        /// 使用场景：
        /// - UI 布局设计时，确保重要内容在安全区域内
        /// - 适配不同屏幕比例（16:9、4:3、21:9 等）
        /// - 考虑屏幕圆角、刘海、虚拟按键等区域
        /// - 电视游戏开发时确保 UI 在 Overscan 安全区内
        /// 
        /// 调用频率：
        /// - 每帧在 XHud_Manager.Update() 中调用
        /// - 开销较小，仅更新几个 UI 元素的位置和大小
        /// 
        /// 注意事项：
        /// - 此方法仅在 UseSafeFrame = true 时执行
        /// - 安全框的所有视觉元素都是动态创建和更新的
        /// - 安全框的颜色、粗细、边距等可在 Inspector 中配置
        /// </summary>
        public void hm_SafeFrameUpdate()
        {
            if (!sf_use)
                return;

            hm_Safe_ClampValue();
            hm_Safe_FrameLine_Update();
            hm_Safe_Seperater_Update();
            hm_Safe_CenterMark_Update();
        }
        /// <summary>
        ///分割线更新
        /// </summary>
        public void hm_Safe_Seperater_Update()
        {
            if (sf_seperaters != null)
            {
                Vector2 pivot = Vector2.zero;
                Vector2 anchorMin = Vector2.zero;
                Vector2 anchorMax = Vector2.zero;
                Vector2 sizeDelta = Vector2.zero;
                Vector2 anchoredPosition3D = Vector2.zero;

                for (int i = 0; i < sf_seperaters.Length; i++)
                {
                    if (sf_seperaters[i] == null)
                        continue;
                    if (sf_seperaters[i].Line == null)
                        continue;
                    if (sf_seperaters[i].Line.rectTransform == null)
                        continue;

                    sf_seperaters[i].Line.color = sf_color_seperater_line;

                    if (sf_seperaters[i].Type == XHudAnchor.上)
                    {
                        pivot = new Vector2(0.5f, 0.5f);
                        anchorMin = new Vector2(0.5f, 1f);
                        anchorMax = new Vector2(0.5f, 1f);
                        sizeDelta = new Vector2(sf_safe_frame_line_width, sf_seperater_length);
                        anchoredPosition3D = new Vector3(0, -sf_line_margins.y, 0);
                    }
                    if (sf_seperaters[i].Type == XHudAnchor.下)
                    {
                        pivot = new Vector2(0.5f, 0.5f);
                        anchorMin = new Vector2(0.5f, 0f);
                        anchorMax = new Vector2(0.5f, 0f);
                        sizeDelta = new Vector2(sf_safe_frame_line_width, sf_seperater_length);
                        anchoredPosition3D = new Vector3(0, sf_line_margins.y, 0);
                    }
                    if (sf_seperaters[i].Type == XHudAnchor.左)
                    {
                        pivot = new Vector2(0.5f, 0.5f);
                        anchorMin = new Vector2(0f, 0.5f);
                        anchorMax = new Vector2(0f, 0.5f);
                        sizeDelta = new Vector2(sf_seperater_length, sf_safe_frame_line_width);
                        anchoredPosition3D = new Vector3(sf_line_margins.x, 0, 0);
                    }
                    if (sf_seperaters[i].Type == XHudAnchor.右)
                    {
                        pivot = new Vector2(0.5f, 0.5f);
                        anchorMin = new Vector2(1f, 0.5f);
                        anchorMax = new Vector2(1f, 0.5f);
                        sizeDelta = new Vector2(sf_seperater_length, sf_safe_frame_line_width);
                        anchoredPosition3D = new Vector3(-sf_line_margins.x, 0, 0);
                    }

                    sf_seperaters[i].Line.rectTransform.pivot = pivot;
                    sf_seperaters[i].Line.rectTransform.anchorMin = anchorMin;
                    sf_seperaters[i].Line.rectTransform.anchorMax = anchorMax;
                    sf_seperaters[i].Line.rectTransform.sizeDelta = sizeDelta;
                    sf_seperaters[i].Line.rectTransform.anchoredPosition3D = anchoredPosition3D;
                }
            }
        }
        /// <summary>
        /// 框线更新
        /// </summary>
        public void hm_Safe_FrameLine_Update()
        {
            if (sf_framelines != null)
            {
                Vector2 pivot = Vector2.zero;
                Vector2 anchorMin = Vector2.zero;
                Vector2 anchorMax = Vector2.zero;
                Vector2 sizeDelta = Vector2.zero;
                Vector2 anchoredPosition3D = Vector2.zero;

                for (int i = 0; i < sf_framelines.Length; i++)
                {
                    if (sf_framelines[i] == null)
                        continue;
                    if (sf_framelines[i].Frame == null)
                        continue;
                    if (sf_framelines[i].Frame.rectTransform == null)
                        continue;

                    sf_framelines[i].Frame.color = sf_color_frame_line;

                    if (sf_framelines[i].Type == XHudAnchor.上)
                    {
                        if (cv_scaler_mode_index == 0)
                        {
                            pivot = new Vector2(0.5f, 1f);
                            anchorMin = new Vector2(0.5f, 1f);
                            anchorMax = new Vector2(0.5f, 1f);
                            sizeDelta = new Vector2(cv_screen_res.x - (sf_line_margins.x * 2), sf_safe_frame_line_width);
                            anchoredPosition3D = new Vector3(0, -sf_line_margins.y, 0);
                        }
                        if (cv_scaler_mode_index == 1)
                        {
                            pivot = new Vector2(0.5f, 1f);
                            anchorMin = new Vector2(0.5f, 1f);
                            anchorMax = new Vector2(0.5f, 1f);
                            sizeDelta = new Vector2(sf_canvas_scaler_screen_size.x - (sf_line_margins.x * 2), sf_safe_frame_line_width);
                            anchoredPosition3D = new Vector3(0, -sf_line_margins.y, 0);
                        }
                    }
                    if (sf_framelines[i].Type == XHudAnchor.下)
                    {
                        if (cv_scaler_mode_index == 0)
                        {
                            pivot = new Vector2(0.5f, 0f);
                            anchorMin = new Vector2(0.5f, 0f);
                            anchorMax = new Vector2(0.5f, 0f);
                            sizeDelta = new Vector2(cv_screen_res.x - (sf_line_margins.x * 2), sf_safe_frame_line_width);
                            anchoredPosition3D = new Vector3(0, sf_line_margins.y, 0);
                        }
                        if (cv_scaler_mode_index == 1)
                        {
                            pivot = new Vector2(0.5f, 0f);
                            anchorMin = new Vector2(0.5f, 0f);
                            anchorMax = new Vector2(0.5f, 0f);
                            sizeDelta = new Vector2(sf_canvas_scaler_screen_size.x - (sf_line_margins.x * 2), sf_safe_frame_line_width);
                            anchoredPosition3D = new Vector3(0, sf_line_margins.y, 0);
                        }
                    }
                    if (sf_framelines[i].Type == XHudAnchor.左)
                    {
                        if (cv_scaler_mode_index == 0)
                        {
                            pivot = new Vector2(0f, 0.5f);
                            anchorMin = new Vector2(0f, 0.5f);
                            anchorMax = new Vector2(0f, 0.5f);
                            sizeDelta = new Vector2(sf_safe_frame_line_width, cv_screen_res.y - (sf_line_margins.y * 2));
                            anchoredPosition3D = new Vector3(sf_line_margins.x, 0, 0);
                        }
                        if (cv_scaler_mode_index == 1)
                        {
                            pivot = new Vector2(0f, 0.5f);
                            anchorMin = new Vector2(0f, 0.5f);
                            anchorMax = new Vector2(0f, 0.5f);
                            sizeDelta = new Vector2(sf_safe_frame_line_width, sf_canvas_scaler_screen_size.y - (sf_line_margins.y * 2));
                            anchoredPosition3D = new Vector3(sf_line_margins.x, 0, 0);
                        }
                    }
                    if (sf_framelines[i].Type == XHudAnchor.右)
                    {
                        if (cv_scaler_mode_index == 0)
                        {
                            pivot = new Vector2(1f, 0.5f);
                            anchorMin = new Vector2(1f, 0.5f);
                            anchorMax = new Vector2(1f, 0.5f);
                            sizeDelta = new Vector2(sf_safe_frame_line_width, cv_screen_res.y - (sf_line_margins.y * 2));
                            anchoredPosition3D = new Vector3(-sf_line_margins.x, 0, 0);
                        }
                        if (cv_scaler_mode_index == 1)
                        {
                            pivot = new Vector2(1f, 0.5f);
                            anchorMin = new Vector2(1f, 0.5f);
                            anchorMax = new Vector2(1f, 0.5f);
                            sizeDelta = new Vector2(sf_safe_frame_line_width, sf_canvas_scaler_screen_size.y - (sf_line_margins.y * 2));
                            anchoredPosition3D = new Vector3(-sf_line_margins.x, 0, 0);
                        }
                    }

                    sf_framelines[i].Frame.rectTransform.pivot = pivot;
                    sf_framelines[i].Frame.rectTransform.anchorMin = anchorMin;
                    sf_framelines[i].Frame.rectTransform.anchorMax = anchorMax;
                    sf_framelines[i].Frame.rectTransform.sizeDelta = sizeDelta;
                    sf_framelines[i].Frame.rectTransform.anchoredPosition3D = anchoredPosition3D;
                }
            }
        }
        /// <summary>
        /// 限制数值
        /// </summary>
        private void hm_Safe_ClampValue()
        {
            sf_safe_frame_line_width = Mathf.Clamp(sf_safe_frame_line_width, 0, float.MaxValue);
            sf_seperater_length = Mathf.Clamp(sf_seperater_length, 0, float.MaxValue);

            sf_line_margins.x = Mathf.Clamp(sf_line_margins.x, 0, float.MaxValue);
            sf_line_margins.y = Mathf.Clamp(sf_line_margins.y, 0, float.MaxValue);
        }
        /// <summary>
        /// 中心标记物布局
        /// </summary>
        public void hm_Safe_CenterMark_Update()
        {
            if (sf_center_marks != null)
            {
                if (sf_center_marks.Edge != null)
                {
                    for (int i = 0; i < sf_center_marks.Edge.Count; i++)
                    {
                        if (sf_center_marks.Edge[i] != null)
                        {
                            sf_center_marks.Edge[i].rectTransform.sizeDelta = new Vector2(sf_center_mark_length, sf_center_mark_width);
                            Vector3 pos = Vector3.zero;

                            if (i == 0)
                            {
                                pos.x = sf_center_mark_distance;
                                pos.y = sf_center_mark_distance;
                            }
                            if (i == 1)
                            {
                                pos.x = -sf_center_mark_distance;
                                pos.y = sf_center_mark_distance;
                            }
                            if (i == 2)
                            {
                                pos.x = sf_center_mark_distance;
                                pos.y = -sf_center_mark_distance;
                            }
                            if (i == 3)
                            {
                                pos.x = -sf_center_mark_distance;
                                pos.y = -sf_center_mark_distance;
                            }
                            sf_center_marks.Edge[i].rectTransform.localPosition = pos;
                            sf_center_marks.Edge[i].color = sf_color_center_mark;
                        }
                    }
                }
            }
        }

        #region 辅助视觉 - 创建
        /// <summary>
        /// 锚点可视化 - 创建
        /// </summary>
        public void hm_AnchorMarks_Create()
        {
            if (sf_safeframe != null)
                return;
            if (cv_anchors_layout_screen == null || cv_anchors_layout_screen.Count <= 0)
                return;

            hm_CreateSafeElements();

            hm_CreateCornerMark();
        }
        /// <summary>
        /// 创建锚点标记物
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private Image hm_CreateAnchorMark(Transform parent, string name)
        {
            GameObject obj = new GameObject();
#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(obj, "MarkObject");
#endif
            obj.name = name;
            obj.layer = LayerMask.NameToLayer("XHud");
            obj.transform.SetParent(parent);
            obj.transform.SetAsFirstSibling();
            obj.transform.localScale = Vector3.one;
            Image img = obj.AddComponent<Image>();
            img.raycastTarget = false;
            return img;
        }
        /// <summary>
        /// 创建角落标记
        /// </summary>
        private void hm_CreateCornerMark()
        {
            for (int i = 0; i < cv_anchors_layout_screen.Count; i++)
            {
                if (cv_anchors_layout_screen[i].Mark != null)
                    continue;
                else
                {
                    Image img = hm_CreateAnchorMark(cv_anchors_layout_screen[i].Anchor, "Mark");

                    switch (cv_anchors_layout_screen[i].Type)
                    {
                        case XHudAnchor.上:
                            img.rectTransform.pivot = new Vector2(0.5f, 1f);
                            img.rectTransform.anchorMin = new Vector2(0.5f, 1f);
                            img.rectTransform.anchorMax = new Vector2(0.5f, 1f);
                            img.rectTransform.anchoredPosition3D = Vector3.zero;
                            break;
                        case XHudAnchor.下:
                            img.rectTransform.pivot = new Vector2(0.5f, 0f);
                            img.rectTransform.anchorMin = new Vector2(0.5f, 0f);
                            img.rectTransform.anchorMax = new Vector2(0.5f, 0f);
                            img.rectTransform.anchoredPosition3D = Vector3.zero;
                            break;
                        case XHudAnchor.左:
                            img.rectTransform.pivot = new Vector2(0f, 0.5f);
                            img.rectTransform.anchorMin = new Vector2(0f, 0.5f);
                            img.rectTransform.anchorMax = new Vector2(0f, 0.5f);
                            img.rectTransform.anchoredPosition3D = Vector3.zero;
                            break;
                        case XHudAnchor.右:
                            img.rectTransform.pivot = new Vector2(1f, 0.5f);
                            img.rectTransform.anchorMin = new Vector2(1f, 0.5f);
                            img.rectTransform.anchorMax = new Vector2(1f, 0.5f);
                            img.rectTransform.anchoredPosition3D = Vector3.zero;
                            break;
                        case XHudAnchor.中心:
                            img.rectTransform.pivot = new Vector2(0.5f, 0.5f);
                            img.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                            img.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                            img.rectTransform.anchoredPosition3D = Vector3.zero;

                            #region 中标标记
                            sf_center_marks.Center = img;

                            ///----创建中心标记
                            for (int s = 0; s < 4; s++)
                            {
                                Image img_mark = hm_CreateAnchorMark(img.transform, "ch");
                                img_mark.rectTransform.sizeDelta = new Vector2(15f, 2f);
                                if (s == 0)
                                {
                                    img_mark.rectTransform.pivot = new Vector2(0f, 0.5f);
                                    img_mark.rectTransform.anchoredPosition3D = Vector3.zero + new Vector3(20, 20, 0);
                                    img_mark.rectTransform.localRotation = Quaternion.Euler(0, 0, 45);
                                }
                                else if (s == 1)
                                {
                                    img_mark.rectTransform.pivot = new Vector2(1f, 0.5f);
                                    img_mark.rectTransform.anchoredPosition3D = Vector3.zero + new Vector3(-20, 20, 0);
                                    img_mark.rectTransform.localRotation = Quaternion.Euler(0, 0, -45);
                                }
                                else if (s == 2)
                                {
                                    img_mark.rectTransform.pivot = new Vector2(1f, 0.5f);
                                    img_mark.rectTransform.anchoredPosition3D = Vector3.zero + new Vector3(20, -20, 0);
                                    img_mark.rectTransform.localRotation = Quaternion.Euler(0, 0, 135);
                                }
                                else if (s == 3)
                                {
                                    img_mark.rectTransform.pivot = new Vector2(0f, 0.5f);
                                    img_mark.rectTransform.anchoredPosition3D = Vector3.zero + new Vector3(-20, -20, 0);
                                    img_mark.rectTransform.localRotation = Quaternion.Euler(0, 0, -135);
                                }

                                sf_center_marks.Edge.Add(img_mark);
                            }
                            #endregion
                            break;
                        case XHudAnchor.左上:
                            img.rectTransform.pivot = new Vector2(0f, 1f);
                            img.rectTransform.anchorMin = new Vector2(0f, 1f);
                            img.rectTransform.anchorMax = new Vector2(0f, 1f);
                            img.rectTransform.anchoredPosition3D = Vector3.zero;
                            break;
                        case XHudAnchor.左下:
                            img.rectTransform.pivot = new Vector2(0f, 0f);
                            img.rectTransform.anchorMin = new Vector2(0f, 0f);
                            img.rectTransform.anchorMax = new Vector2(0f, 0f);
                            img.rectTransform.anchoredPosition3D = Vector3.zero;
                            break;
                        case XHudAnchor.右上:
                            img.rectTransform.pivot = new Vector2(1f, 1f);
                            img.rectTransform.anchorMin = new Vector2(1f, 1f);
                            img.rectTransform.anchorMax = new Vector2(1f, 1f);
                            img.rectTransform.anchoredPosition3D = Vector3.zero;
                            break;
                        case XHudAnchor.右下:
                            img.rectTransform.pivot = new Vector2(1f, 0f);
                            img.rectTransform.anchorMin = new Vector2(1f, 0f);
                            img.rectTransform.anchorMax = new Vector2(1f, 0f);
                            img.rectTransform.anchoredPosition3D = Vector3.zero;
                            break;
                        case XHudAnchor.底层:
                            img.rectTransform.pivot = new Vector2(0.5f, 0.5f);
                            img.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                            img.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                            img.rectTransform.anchoredPosition3D = Vector3.zero;
                            break;
                        case XHudAnchor.顶层:
                            img.rectTransform.pivot = new Vector2(0.5f, 0.5f);
                            img.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                            img.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                            img.rectTransform.anchoredPosition3D = Vector3.zero;
                            break;
                    }
                    cv_anchors_layout_screen[i].Mark = img;
                }
            }
        }
        /// <summary>
        /// 创建安全框
        /// </summary>
        private void hm_CreateSafeElements()
        {
            GameObject obj_SafeFrame_Seperaters = new GameObject();
            obj_SafeFrame_Seperaters.name = "SafeFrames";
            obj_SafeFrame_Seperaters.layer = LayerMask.NameToLayer("XHud");
            RectTransform SafeFrame_Seperaters = obj_SafeFrame_Seperaters.AddComponent<RectTransform>();
            Canvas canvas = (Canvas)cv_screen;
            SafeFrame_Seperaters.SetParent(canvas.transform);
            SafeFrame_Seperaters.localPosition = Vector3.zero;
            SafeFrame_Seperaters.localScale = Vector3.one;
            SafeFrame_Seperaters.anchorMin = new Vector2(0, 0);
            SafeFrame_Seperaters.anchorMax = new Vector2(1, 1);
            SafeFrame_Seperaters.sizeDelta = new Vector2(0, 0);

            Safe_FrameLine img_frameline_up = new Safe_FrameLine();
            img_frameline_up.Type = XHudAnchor.上;
            img_frameline_up.Frame = hm_CreateAnchorMark(obj_SafeFrame_Seperaters.transform, "frameline_up");

            Safe_FrameLine img_frameline_down = new Safe_FrameLine();
            img_frameline_down.Type = XHudAnchor.下;
            img_frameline_down.Frame = hm_CreateAnchorMark(obj_SafeFrame_Seperaters.transform, "frameline_down");

            Safe_FrameLine img_frameline_left = new Safe_FrameLine();
            img_frameline_left.Type = XHudAnchor.左;
            img_frameline_left.Frame = hm_CreateAnchorMark(obj_SafeFrame_Seperaters.transform, "frameline_left");

            Safe_FrameLine img_frameline_right = new Safe_FrameLine();
            img_frameline_right.Type = XHudAnchor.右;
            img_frameline_right.Frame = hm_CreateAnchorMark(obj_SafeFrame_Seperaters.transform, "frameline_right");

            List<Safe_FrameLine> imgs = new List<Safe_FrameLine>();
            imgs.Add(img_frameline_up);
            imgs.Add(img_frameline_down);
            imgs.Add(img_frameline_left);
            imgs.Add(img_frameline_right);

            sf_framelines = new Safe_FrameLine[imgs.Count];
            for (int i = 0; i < imgs.Count; i++)
            {
                sf_framelines[i] = new Safe_FrameLine();
                sf_framelines[i].Frame = imgs[i].Frame;
                sf_framelines[i].Type = imgs[i].Type;
            }

            Safe_Seperater img_seperater_up = new Safe_Seperater();
            img_seperater_up.Type = XHudAnchor.上;
            img_seperater_up.Line = hm_CreateAnchorMark(obj_SafeFrame_Seperaters.transform, "seperater_up");

            Safe_Seperater img_seperater_down = new Safe_Seperater();
            img_seperater_down.Type = XHudAnchor.下;
            img_seperater_down.Line = hm_CreateAnchorMark(obj_SafeFrame_Seperaters.transform, "seperater_down");

            Safe_Seperater img_seperater_left = new Safe_Seperater();
            img_seperater_left.Type = XHudAnchor.左;
            img_seperater_left.Line = hm_CreateAnchorMark(obj_SafeFrame_Seperaters.transform, "seperater_left");

            Safe_Seperater img_seperater_right = new Safe_Seperater();
            img_seperater_right.Type = XHudAnchor.右;
            img_seperater_right.Line = hm_CreateAnchorMark(obj_SafeFrame_Seperaters.transform, "seperater_right");

            List<Safe_Seperater> seperaters = new List<Safe_Seperater>();
            seperaters.Add(img_seperater_up);
            seperaters.Add(img_seperater_down);
            seperaters.Add(img_seperater_left);
            seperaters.Add(img_seperater_right);

            sf_seperaters = new Safe_Seperater[seperaters.Count];
            for (int i = 0; i < seperaters.Count; i++)
            {
                sf_seperaters[i] = new Safe_Seperater();
                sf_seperaters[i].Line = seperaters[i].Line;
                sf_seperaters[i].Type = seperaters[i].Type;
            }

            sf_safeframe = obj_SafeFrame_Seperaters.transform;
        }
        #endregion

        #region 辅助视觉 - 销毁
        /// <summary>
        /// 锚点可视化 - 销毁
        /// </summary>
        public void hm_AnchorMarks_Destroy()
        {
            if (sf_safeframe == null)
                return;
            if (cv_anchors_layout_screen == null || cv_anchors_layout_screen.Count <= 0)
                return;

            for (int i = 0; i < cv_anchors_layout_screen.Count; i++)
            {
                if (cv_anchors_layout_screen[i].Mark != null)
                {
                    DestroyImmediate(cv_anchors_layout_screen[i].Mark.gameObject);
                    cv_anchors_layout_screen[i].Mark = null;
                }
            }

            sf_center_marks.Edge.Clear();
            sf_center_marks.Center = null;

            if (sf_framelines.Length <= 0)
                return;
            for (int i = 0; i < sf_framelines.Length; i++)
            {
                if (sf_framelines[i].Frame != null)
                {
                    DestroyImmediate(sf_framelines[i].Frame.gameObject, true);
                }
            }
            sf_framelines = null;

            if (sf_seperaters.Length <= 0)
                return;
            for (int i = 0; i < sf_seperaters.Length; i++)
            {
                if (sf_seperaters[i].Line != null)
                {
                    DestroyImmediate(sf_seperaters[i].Line.gameObject, true);
                }
            }
            sf_seperaters = null;

            if (sf_safeframe != null)
            {
                DestroyImmediate(sf_safeframe.gameObject, true);
                sf_safeframe = null;
            }

        }
        #endregion
    }
}