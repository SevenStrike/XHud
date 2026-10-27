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
        public Vector3 position;
        /// <summary>
        /// 角度
        /// </summary>
        public Vector3 euler;
        /// <summary>
        /// 缩放
        /// </summary>
        public Vector3 scale;
        /// <summary>
        /// 透明度
        /// </summary>
        public float alpha;
        /// <summary>
        /// 颜色
        /// </summary>
        public Color color;
        /// <summary>
        /// 尺寸
        /// </summary>
        public Vector2 size;
        /// <summary>
        /// 填充
        /// </summary>
        public float fill;
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
        [SerializeField] public PrimitiveFeatures primitive_features;
        /// <summary>
        /// 第一次点击的时候记录为首次使用控制器特性，并且会把所有涉及的特性参数都收集，只有第一次使用才会起效
        /// </summary>
        [SerializeField] public bool first_save_features;
        #endregion

        #region 状态开关
        /// <summary>
        /// 调试开关
        /// </summary>
        [SerializeField] public bool debug;
        #endregion

        #region 折叠
        public bool
            fold_param = true,
            fold_option = true,
            fold_based = true;

        public void GroupFold(bool state)
        {
            fold_param = state;
            fold_option = state;
            fold_based = state;
        }

        public void GroupFold(bool param = true, bool option = true, bool based = true)
        {
            fold_param = param;
            fold_option = option;
            fold_based = based;
        }
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
            if (controller.mod_rect != null)
            {
                primitive_features.position = controller.mod_rect.anchoredPosition3D;
                primitive_features.euler = controller.mod_rect.localEulerAngles;
                primitive_features.scale = controller.mod_rect.localScale;
                primitive_features.size = controller.mod_rect.sizeDelta;
            }
            // 保存特性：CanvasGroup
            if (controller.mod_canvas_group != null)
            {
                primitive_features.alpha = controller.mod_canvas_group.alpha;
            }
            // 保存特性：Image
            if (controller.mod_image != null)
            {
                primitive_features.color = controller.mod_image.color;
                primitive_features.fill = controller.mod_image.fillAmount;
            }
            // 保存特性：Text
            else if (controller.mod_text != null)
            {
                primitive_features.color = controller.mod_text.color;
            }
            // 保存特性：TmpText
            else if (controller.mod_tmp_text != null)
            {
                primitive_features.color = controller.mod_tmp_text.color;
            }
            // 保存特性：_RawImage
            else if (controller.mod_raw_image != null)
            {
                primitive_features.color = controller.mod_raw_image.color;
            }
        }
        /// <summary>
        /// 还原 PrimitiveFeature 特性参数
        /// </summary>
        public void PrimitiveFeature_Load()
        {
            // 读取特性：Rect
            if (controller.mod_rect != null)
            {
                controller.mod_rect.anchoredPosition3D = primitive_features.position;
                controller.mod_rect.localEulerAngles = primitive_features.euler;
                controller.mod_rect.localScale = primitive_features.scale;
                controller.mod_rect.sizeDelta = primitive_features.size;
            }
            // 读取特性：CanvasGroup
            if (controller.mod_canvas_group != null)
            {
                controller.mod_canvas_group.alpha = primitive_features.alpha;
            }
            // 读取特性：Image
            if (controller.mod_image != null)
            {
                controller.mod_image.color = primitive_features.color;
                controller.mod_image.fillAmount = primitive_features.fill;
            }
            // 读取特性：Text
            else if (controller.mod_text != null)
            {
                controller.mod_text.color = primitive_features.color;
            }
            // 读取特性：TmpText
            else if (controller.mod_tmp_text != null)
            {
                controller.mod_tmp_text.color = primitive_features.color;
            }
            // 读取特性：RawImage
            else if (controller.mod_raw_image != null)
            {
                controller.mod_raw_image.color = primitive_features.color;
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
                controller.pt_feature = this;
            }
        }
    }
}