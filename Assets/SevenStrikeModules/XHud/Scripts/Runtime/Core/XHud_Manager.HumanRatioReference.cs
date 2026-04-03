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
    using UnityEngine;
    using UnityEngine.UI;

    [System.Serializable]
    public struct XHUdRatioReferenceRes
    {
        [SerializeField]
        public Vector2 size;
        [SerializeField]
        public Sprite sprite;
    }

    public partial class XHud_Manager : MonoBehaviour
    {
        [SerializeField]
        /// <summary>
        /// 物理世界中的显示器的尺寸大小
        /// </summary>
        public Vector2 PhysicsScreenSize;
        [SerializeField]
        /// <summary>
        /// 参考尺寸
        /// </summary>
        public Vector2 PhysicsReferenceSize;
        [SerializeField]
        /// <summary>
        /// 参考比例标识显示组件
        /// </summary>
        public Image Reference_Image;
        [SerializeField]
        /// <summary>
        /// 参考比例标识显示颜色
        /// </summary>
        public Color Reference_Image_Color;
        /// <summary>
        /// 全屏参考比例标识资源
        /// 当 RatioReferenceIsPart = false 时使用此资源
        /// 用于在全屏模式下显示物理参考标识（如完整的人形轮廓）
        /// 帮助判断 UI 元素与完整物理参考物的比例关系
        /// </summary>
        [SerializeField]
        public XHUdRatioReferenceRes Res_Full;
        /// <summary>
        /// 局部参考比例标识资源
        /// 当 RatioReferenceIsPart = true 时使用此资源
        /// 用于在局部区域显示物理参考标识（如头像、手部等局部参考）
        /// 常用于判断 UI 元素与特定身体部位的比例关系
        /// 示例：头部参考图用于判断头像框大小是否合理
        /// </summary>
        [SerializeField]
        public XHUdRatioReferenceRes Res_Part;
        /// <summary>
        /// 参考形状的比例尺寸
        /// 定义物理参考标识在屏幕上的显示尺寸（相对于屏幕宽高比）
        /// 基于物理世界中的真实尺寸进行换算，确保 UI 元素与真实世界比例一致
        /// 示例：Vector2(0.5f, 0.5f) 表示参考标识占屏幕宽高的 50%
        /// </summary>
        [SerializeField]
        public Vector2 ReferShape_RatioSize;
        /// <summary>
        /// 参考形状的比例容差
        /// 定义参考标识允许的尺寸偏差范围，用于自动适配不同分辨率
        /// X 值：水平方向的容差比例，Y 值：垂直方向的容差比例
        /// 当屏幕比例与参考比例不匹配时，在此容差范围内仍可接受
        /// 示例：Vector2(0.05f, 0.05f) 表示允许 5% 的尺寸偏差
        /// </summary>
        [SerializeField]
        public Vector2 ReferShape_RatioTolerance;
        [SerializeField]
        /// <summary>
        /// 使用人形比例参考
        /// </summary>
        public bool UseRatioReference;
        [SerializeField]
        /// <summary>
        /// 人形比例参考类型
        /// </summary>
        public bool RatioReferenceIsPart = true;
    }
}