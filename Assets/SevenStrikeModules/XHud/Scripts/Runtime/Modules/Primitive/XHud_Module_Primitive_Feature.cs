/*
 * ============================================================================
 * ⚠ 版权声明（禁止删除、禁止修改、衍生作品必须保留此注释）⚠
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

    [SerializeField]
    [System.Serializable]
    /// <summary>
    /// 图元的特性参数
    /// </summary>
    public class PrimitiveFeatures
    {
        /// <summary>
        /// 位置
        /// </summary>
        public Vector3 Position;
        /// <summary>
        /// 角度
        /// </summary>
        public Vector3 Euler;
        /// <summary>
        /// 缩放
        /// </summary>
        public Vector3 Scale;
        /// <summary>
        /// 透明度
        /// </summary>
        public float Alpha;
        /// <summary>
        /// 颜色
        /// </summary>
        public Color Color;
        /// <summary>
        /// 尺寸
        /// </summary>
        public Vector2 Size;
        /// <summary>
        /// 填充
        /// </summary>
        public float Fill;
    }

    public class XHud_Module_Primitive_Feature : MonoBehaviour
    {
        /// <summary>
        /// 图元控制器
        /// </summary>
        public XHud_Module_Primitive_Controller controller;

        #region 图元特性参数
        /// <summary>
        /// 图元特性参数
        /// </summary>
        [SerializeField] public PrimitiveFeatures PrimitiveFeatures;
        /// <summary>
        /// 第一次点击的时候记录为首次使用动画器，并且会把所有动画器涉及的参数都收集，只有第一次使用才会起效
        /// </summary>
        [SerializeField] public bool FirstSaveFeatures;
        #endregion

        #region 状态开关
        /// <summary>
        /// 调试开关
        /// </summary>
        [SerializeField] public bool Debug;
        #endregion

        void Awake()
        {

        }

        void Start()
        {

        }

        void Update()
        {

        }

        /// <summary>
        /// 记录 PrimitiveFeature 特性参数
        /// </summary>
        public void PrimitiveFeature_Save()
        {
            // 保存特性：Rect
            if (controller.mod_Rect != null)
            {
                PrimitiveFeatures.Position = controller.mod_Rect.anchoredPosition3D;
                PrimitiveFeatures.Euler = controller.mod_Rect.localEulerAngles;
                PrimitiveFeatures.Scale = controller.mod_Rect.localScale;
                PrimitiveFeatures.Size = controller.mod_Rect.sizeDelta;
            }
            // 保存特性：CanvasGroup
            if (controller.mod_CanvasGroup != null)
            {
                PrimitiveFeatures.Alpha = controller.mod_CanvasGroup.alpha;
            }
            // 保存特性：Image
            if (controller.mod_Image != null)
            {
                PrimitiveFeatures.Color = controller.mod_Image.color;
                PrimitiveFeatures.Fill = controller.mod_Image.fillAmount;
            }
            // 保存特性：Text
            else if (controller.mod_Text != null)
            {
                PrimitiveFeatures.Color = controller.mod_Text.color;
            }
            // 保存特性：TmpText
            else if (controller.mod_TmpText != null)
            {
                PrimitiveFeatures.Color = controller.mod_TmpText.color;
            }
            // 保存特性：_RawImage
            else if (controller.mod_RawImage != null)
            {
                PrimitiveFeatures.Color = controller.mod_RawImage.color;
            }
        }
        /// <summary>
        /// 还原 PrimitiveFeature 特性参数
        /// </summary>
        public void PrimitiveFeature_Load()
        {
            // 读取特性：Rect
            if (controller.mod_Rect != null)
            {
                controller.mod_Rect.anchoredPosition3D = PrimitiveFeatures.Position;
                controller.mod_Rect.localEulerAngles = PrimitiveFeatures.Euler;
                controller.mod_Rect.localScale = PrimitiveFeatures.Scale;
                controller.mod_Rect.sizeDelta = PrimitiveFeatures.Size;
            }
            // 读取特性：CanvasGroup
            if (controller.mod_CanvasGroup != null)
            {
                controller.mod_CanvasGroup.alpha = PrimitiveFeatures.Alpha;
            }
            // 读取特性：Image
            if (controller.mod_Image != null)
            {
                controller.mod_Image.color = PrimitiveFeatures.Color;
                controller.mod_Image.fillAmount = PrimitiveFeatures.Fill;
            }
            // 读取特性：Text
            else if (controller.mod_Text != null)
            {
                controller.mod_Text.color = PrimitiveFeatures.Color;
            }
            // 读取特性：TmpText
            else if (controller.mod_TmpText != null)
            {
                controller.mod_TmpText.color = PrimitiveFeatures.Color;
            }
            // 读取特性：RawImage
            else if (controller.mod_RawImage != null)
            {
                controller.mod_RawImage.color = PrimitiveFeatures.Color;
            }
        }

        /// <summary>
        /// 在自身寻找图元控制器
        /// </summary>
        public void FindController()
        {
            controller = transform.GetComponent<XHud_Module_Primitive_Controller>();
            if (controller != null)
            {
                controller.pt_Feature = this;
            }
        }
    }
}