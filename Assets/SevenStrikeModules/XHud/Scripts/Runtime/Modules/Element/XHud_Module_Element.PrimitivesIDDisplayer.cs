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
    using UnityEditor;
    using UnityEngine;

    public partial class XHud_Module_Element : MonoBehaviour
    {
        /// <summary>
        /// 用于在场景中显示元素下的所有图元ID标签，便于绑定数据
        /// </summary>
        private void DisplayPrimitivesID()
        {
            #region 用于在场景中显示元素下的所有图元ID标签，便于绑定数据
#if UNITY_EDITOR
            if (!XHud_Dashboard.DisplayPrimitiveControllerIDConfig.XHudPrimitiveController_ID_Displayer)
                return;

            if (PrimitiveControllerNodes == null || PrimitiveControllerNodes.Count == 0)
                return;

            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.MiddleCenter;
            style.normal.textColor = XHud_Dashboard.HudManagerGet().PrimtiveID_LabelFont_Color;
            style.fontStyle = FontStyle.Bold;
            style.fontSize = XHud_Dashboard.HudManagerGet().PrimtiveID_LabelFont_Size;


            for (int i = 0; i < PrimitiveControllerNodes.Count; i++)
            {
                XHud_Module_Primitive_Controller con = PrimitiveControllerNodes[i].Controller;

                if (con == null)
                    continue;

                if (con.mod_Rect != null)
                {
                    Vector3 target = con.mod_Rect.position + (new Vector3(0, XHud_Dashboard.HudManagerGet().PrimtiveID_LabelLine_Height, 0) * 0.3f);

                    // 获取实际宽度（世界空间）
                    float actualWidth = GetActualWidth(con.mod_Rect);

                    float offset = 0;

                    if (con.mod_Text != null)
                    {
                        TextAnchor anchor = con.mod_Text.alignment;

                        if (anchor == TextAnchor.UpperLeft || anchor == TextAnchor.LowerLeft || anchor == TextAnchor.MiddleLeft)
                        {
                            // 左对齐：向左偏移
                            offset = -actualWidth / 2;

                        }
                        if (anchor == TextAnchor.UpperRight || anchor == TextAnchor.LowerRight || anchor == TextAnchor.MiddleRight)
                        {
                            // 左对齐：向右偏移
                            offset = actualWidth / 2;
                        }
                    }

                    if (con.mod_TmpText != null)
                    {
                        TmpContentAnchor anchor = con.mod_TmpText.TextStyleInfo.tmp_anchor;

                        if (anchor == TmpContentAnchor.中线靠左 ||
                            anchor == TmpContentAnchor.基线靠左 ||
                            anchor == TmpContentAnchor.底部靠左 ||
                            anchor == TmpContentAnchor.顶部靠左 ||
                            anchor == TmpContentAnchor.左
                            )
                        {
                            // 左对齐：向左偏移
                            offset = -actualWidth / 2;
                        }
                        if (anchor == TmpContentAnchor.中线靠右 ||
                            anchor == TmpContentAnchor.基线靠右 ||
                            anchor == TmpContentAnchor.底部靠右 ||
                            anchor == TmpContentAnchor.顶部靠右 ||
                            anchor == TmpContentAnchor.右)
                        {
                            // 左对齐：向左偏移
                            offset = actualWidth / 2;
                        }
                    }

                    // 应用偏移后的目标位置
                    Vector3 offsetTarget = target + new Vector3(offset, 0, 0);
                    Vector3 offsetssddTarget = con.mod_Rect.position + new Vector3(offset, 0, 0);

                    Handles.color = XHud_Dashboard.HudManagerGet().PrimtiveID_LabelLine_Color;

                    Handles.DrawLine(offsetssddTarget, offsetTarget);  // 这里改成 offsetTarget

                    Handles.color = Color.white;

                    // 显示文本标签
                    Handles.Label(offsetTarget + Vector3.up * 0.05f, con.ID, style);
                }
            }
#endif
            #endregion
        }

        /// <summary>
        /// 获取RectTransform在世界空间中的实际宽度
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <returns></returns>
        private float GetActualWidth(RectTransform rectTransform)
        {
            if (rectTransform == null) return 0f;

            // 方法1：使用rect.width（局部坐标下的宽度）
            float localWidth = rectTransform.rect.width;

            // 考虑父级缩放，转换到世界空间
            Vector3[] worldCorners = new Vector3[4];
            rectTransform.GetWorldCorners(worldCorners);

            // 计算世界空间中的实际宽度（右上角x - 左上角x）
            float worldWidth = Mathf.Abs(worldCorners[2].x - worldCorners[0].x);

            // 如果世界宽度有效则使用，否则使用局部宽度
            if (worldWidth > 0.001f)
            {
                return worldWidth;
            }

            // 备选方案：考虑lossyScale
            Vector3 lossyScale = rectTransform.lossyScale;
            float scaledWidth = localWidth * lossyScale.x;

            return scaledWidth > 0 ? scaledWidth : localWidth;
        }
    }
}