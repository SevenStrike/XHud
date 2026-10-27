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
    using SevenStrikeModules.XHud.Enums;
    using UnityEngine;

    public class XHud_Module_Primitive_Painting : MonoBehaviour
    {
        /// <summary>
        /// 图元控制器
        /// </summary>
        public XHud_Module_Primitive_Controller controller;

        #region 配色
        /// <summary>
        /// 原始色
        /// </summary>
        [SerializeField] public Color original_color = Color.white;
        /// <summary>
        /// 配色名称（从颜色库中读取）
        /// </summary>
        [SerializeField] public string colorise_name;
        #endregion

        #region 状态开关
        /// <summary>
        /// 调试开关
        /// </summary>
        [SerializeField] public bool debug;
        /// <summary>
        /// 同步库颜色
        /// </summary>
        [SerializeField] public bool sync_library_color;
        /// <summary>
        /// 同步Image颜色
        /// </summary>
        [SerializeField] public bool sync_image_color;
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
            ColorSync();
        }

        void Update()
        {
            ColorSync();
        }

        #region 颜色设置
        /// <summary>
        /// 设置受控组件颜色
        /// </summary>
        /// <param name="color"></param>
        public void UpdateColor(Color color)
        {
            //如果图元控制器存在于上级按钮物体下
            if (controller.mod_hud_button != null)
            {
                //--如果 - 此图元控制器识别类型为TmpText并作为Button下挂的组件且按钮控制  文字变色  则不同步颜色
                if (controller.mod_hud_button.TextColorSyncFade && controller.mod_hud_button.ButtonTmpText != null && controller.mod_hud_button.ButtonTmpText == controller.mod_tmp_text)
                    return;
                //--如果 - 此图元控制器识别类型为Text并作为Button下挂的组件且按钮控制  文字变色  则不同步颜色
                if (controller.mod_hud_button.TextColorSyncFade && controller.mod_hud_button.ButtonText != null && controller.mod_hud_button.ButtonText == controller.mod_text)
                    return;
                //--如果 - 此图元控制器识别类型为Image并作为Button下挂的组件且按钮控制  图标变色  则不同步颜色
                if (controller.mod_hud_button.IconColorSyncFade && controller.mod_hud_button.IconImage == controller.mod_image)
                    return;
                //--如果 - 此图元控制器识别类型为Image并作为Button下挂的组件且按钮控制  背景变色  则不同步颜色
                if (controller.mod_hud_button.BgColorSyncFade && controller.mod_hud_button.BgImage == controller.mod_image)
                    return;
            }

            //--如果 - 图像组件存在则使用color覆盖图像组件的颜色
            if (controller.mod_image != null)
            {
                controller.mod_image.color = color;
            }
            //--否则 - 如果 - 文字组件存在则使用color覆盖文字组件的颜色
            else if (controller.mod_text != null)
            {
                if (controller.mod_text.text_color_owner != TextColorOwner.Painting)
                    return;
                if (controller.mod_text.style_library_synching && controller.mod_text.text_style_info.lib_style_effect_color)
                    return;
                controller.mod_text.text_style_info.txt_Set_FontColor(color);
            }
            //--否则 - 如果 - 文字组件存在则使用color覆盖文字组件的颜色
            else if (controller.mod_tmp_text != null)
            {
                if (controller.mod_tmp_text.text_color_owner != TextColorOwner.Painting)
                    return;
                if (controller.mod_tmp_text.style_library_synching && controller.mod_tmp_text.text_style_info.lib_style_effect_color)
                    return;
                controller.mod_tmp_text.text_style_info.tmp_Set_FontColor(color);
            }
            //--否则 - 如果 - 原始图像组件存在则使用color覆盖原始图像组件的颜色
            else if (controller.mod_raw_image != null)
            {
                controller.mod_raw_image.color = color;
            }

            original_color = color;
        }
        /// <summary>
        /// 根据色卡库匹配名同步/直接设置颜色
        /// </summary>
        public void ColorSync()
        {
            //--如果开启同步颜色库颜色则将组件的颜色值覆盖为库中选中的颜色，否则使用原始色
            if (sync_library_color)
            {
                //--如果颜色库存在则设置组件颜色为指定名称的库中的颜色
                if (XHud_Manager.instance.library_colors != null)
                {
                    if (sync_image_color)
                    {
                        Color cc = XHud_Manager.instance.library_colors.ColorsLibrary_GetColor(colorise_name);
                        UpdateColor(cc);
                    }
                }
            }
            else
            {
                if (sync_image_color)
                    //--设置组件颜色为原始色
                    UpdateColor(original_color);
            }
        }
        /// <summary>
        /// 设置目标色卡名称
        /// </summary>
        /// <param name="name"></param>
        public void SetColoriseName(string name)
        {
            colorise_name = name;
        }
        /// <summary>
        /// 设置原始色
        /// </summary>
        /// <param name="name"></param>
        public void SetOriginalColor(Color color)
        {
            original_color = color;
        }
        /// <summary>
        /// 设置接管 Image 颜色
        /// </summary>
        /// <param name="name"></param>
        public void SetSyncImageColor(bool state)
        {
            sync_image_color = state;
        }
        #endregion

        /// <summary>
        /// 在自身寻找图元控制器
        /// </summary>
        public void FindController()
        {
            controller = transform.GetComponent<XHud_Module_Primitive_Controller>();
            if (controller != null)
            {
                controller.pt_painting = this;
            }
        }
    }
}