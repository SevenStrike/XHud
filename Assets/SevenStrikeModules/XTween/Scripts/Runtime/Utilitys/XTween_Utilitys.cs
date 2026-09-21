/*
 * ============================================================================
 * ⚠️ 版权声明（禁止删除、禁止修改、衍生作品必须保留此注释）⚠️
 * ============================================================================
 * 版权声明 Copyright (C) 2025-Present Nanjing SevenStrike Media Co., Ltd.
 * 中文名称：南京塞维斯传媒有限公司
 * 英文名称：SevenStrikeMedia
 * 项目作者：徐寅智
 * 项目名称：XTween - Unity 高性能动画架构插件
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
namespace SevenStrikeModules.XTween
{
    using UnityEngine;

    public static class XTween_Utilitys
    {
        /// <summary>
        /// 计算考虑旋转的相对位移坐标（2D空间）
        /// </summary>
        /// <param name="rectTransform">目标UI变换组件</param>
        /// <param name="currentPos">当前锚点位置</param>
        /// <param name="offset">局部空间偏移量</param>
        /// <returns>世界空间位移后的新坐标</returns>
        /// <remarks>
        /// 数学原理：
        /// 1. 获取RectTransform的Z轴旋转角度
        /// 2. 应用二维旋转矩阵计算实际偏移：
        ///    newX = offset.x * cosθ - offset.y * sinθ
        ///    newY = offset.x * sinθ + offset.y * cosθ
        /// 3. 叠加到当前位置
        ///
        /// 典型应用场景：
        /// - 实现"相对当前旋转角度的位移"
        /// - 处理旋转后的UI元素坐标变换
        ///
        /// 示例：
        /// 按钮旋转45度后，向右移动100像素：
        /// CalculateRelativePosition(transform, pos, new 二维向量_Vector2(100,0))
        /// </remarks>
        public static Vector2 CalculateRelativePosition(RectTransform rectTransform, Vector2 currentPos, Vector2 offset)
        {
            float angle = rectTransform.localEulerAngles.z * Mathf.Deg2Rad;
            return new Vector2(
                currentPos.x + offset.x * Mathf.Cos(angle) - offset.y * Mathf.Sin(angle),
                currentPos.y + offset.x * Mathf.Sin(angle) + offset.y * Mathf.Cos(angle)
            );
        }
        /// <summary>
        /// 计算考虑旋转的相对位移坐标（3D空间扩展版）
        /// </summary>
        /// <param name="offset">
        /// 偏移量：
        /// - XY轴：受旋转影响
        /// - Z轴：直接叠加
        /// </param>
        /// <returns>
        /// 新坐标：
        /// - XY分量：经过旋转计算
        /// - Z分量：currentPos.z + offset.z
        /// </returns>
        /// <remarks>
        /// 与2D版本的区别：
        /// 1. 保持Z轴独立性（不受旋转影响）
        /// 2. 使用Vector3类型参数
        ///
        /// 注意：
        /// - 仅处理Z轴旋转，忽略X/Y轴旋转
        /// - 适用于大部分UI动画场景
        /// </remarks>
        public static Vector3 CalculateRelativePosition(RectTransform rectTransform, Vector3 currentPos, Vector3 offset)
        {
            Vector2 xyOffset = CalculateRelativePosition(rectTransform, (Vector2)currentPos, (Vector2)offset);
            return new Vector3(xyOffset.x, xyOffset.y, currentPos.z + offset.z);
        }
        /// <summary>
        /// 计算 Transform 的相对位置
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="currentPos"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static Vector3 CalculateRelativePosition(Transform transform, Vector3 currentPos, Vector3 offset)
        {
            // 对于 Transform，使用四元数旋转来处理完整的3D旋转
            return currentPos + transform.rotation * offset;
        }
    }
}