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
    using SevenStrikeModules.XHud.Utilitys;
    using UnityEngine;

    public partial class XHud_Module_Element : MonoBehaviour
    {
        [SerializeField]
        [Range(0f, 1f)]
        public float Alpha = 1;
        [SerializeField]
        /// <summary>
        /// CanvasGroup组件
        /// </summary>
        public CanvasGroup CanvasGroup;

        /// <summary>
        /// XHud元素 - 同步透明度
        /// </summary>
        public virtual void element_AlphaSyncUpdate()
        {
            // 浮点数边界值钳位与容差处理
            // 防止浮点数精度问题导致的值无法精确到达边界（0 或 1），主动将其钳位到精确的边界值
            if (CanvasGroup != null)
            {
                if (Alpha >= 0.9999f)
                {
                    CanvasGroup.alpha = 1;
                    Alpha = 1;
                }
                else if (Alpha <= 0.0001f)
                {
                    CanvasGroup.alpha = 0;
                    Alpha = 0;
                }
                else
                    CanvasGroup.alpha = Alpha;
            }
        }
        /// <summary>
        /// XHud元素 - 设置透明度
        /// </summary>
        /// <param name="alpha">透明度</param>
        public virtual void element_AlphaSet(float alpha)
        {
            Alpha = alpha;
            element_AlphaSyncUpdate();

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "透明度设置为：" + alpha, HudMsgState.通知);
        }
    }
}