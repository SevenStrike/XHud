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
namespace SevenStrikeModules.XHud.Editor
{
    using UnityEditor;
    using UnityEngine;

    public partial class Editor_XHud_Manager : Editor
    {
        #region 诊断面板
        /// <summary>
        /// 诊断配置是否完善。
        /// <para/>
        /// <list type="bullet">
        /// <item><description>检查分辨率、场景相机、各资源库与元素库是否已配置。</description></item>
        /// <item><description>任一缺失即返回 <c>true</c>。</description></item>
        /// </list>
        /// </summary>
        /// <returns>存在缺失时返回 <c>true</c>，否则返回 <c>false</c>。</returns>
        private bool xHud_Diagnostic()
        {
            bool hasDiagnostic = false;

            if (sp_cv_screen_res.vector2Value == Vector2.zero)
            {
                hasDiagnostic = true;
            }

            if (sp_camera_scene.objectReferenceValue == null)
            {
                hasDiagnostic = true;
            }

            if (sp_library_colors.objectReferenceValue == null)
            {
                hasDiagnostic = true;
            }

            if (sp_library_curves.objectReferenceValue == null)
            {
                hasDiagnostic = true;
            }

            if (sp_library_sounds.objectReferenceValue == null)
            {
                hasDiagnostic = true;
            }

            if (sp_library_textstyles.objectReferenceValue == null)
            {
                hasDiagnostic = true;
            }

            if (sp_library_motions.objectReferenceValue == null)
            {
                hasDiagnostic = true;
            }

            if (sp_library_transitions.objectReferenceValue == null)
            {
                hasDiagnostic = true;
            }

            if (sp_ele_library_elements.arraySize <= 0)
            {
                hasDiagnostic = true;
            }

            return hasDiagnostic;
        }
        #endregion
    }
}