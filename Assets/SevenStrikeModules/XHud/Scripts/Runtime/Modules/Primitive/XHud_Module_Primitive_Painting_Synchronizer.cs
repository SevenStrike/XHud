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
    using UnityEditor;
    using UnityEngine;

    [ExecuteInEditMode]
#if UNITY_EDITOR
    [CanEditMultipleObjects]
#endif
    public class XHud_Module_Primitive_Painting_Synchronizer : MonoBehaviour
    {
        /// <summary>
        /// 图元配色器
        /// </summary>
        private XHud_Module_Primitive_Painting Painting;
        /// <summary>
        /// XHud 管理器
        /// </summary>
        private XHud_Manager mgr;

        private Vector3 ActivateScale;

        private void OnEnable()
        {
            if (Application.isPlaying)
                return;

            if (Painting == null)
                Painting = GetComponent<XHud_Module_Primitive_Painting>();

            if (mgr == null)
            {
                mgr = FindFirstObjectByType<XHud_Manager>();
            }

            if (mgr.Hud_Colors != null)
            {
                mgr.Hud_Colors.act_on_ColorChanged += act_on_ColorChanged;
            }
        }

        private void act_on_ColorChanged()
        {
            UpdateLibraryColor(); // 执行逻辑
        }

        private void OnDisable()
        {
            if (Application.isPlaying)
                return;

            if (mgr.Hud_Colors != null)
                mgr.Hud_Colors.act_on_ColorChanged -= act_on_ColorChanged;
        }

        void Update()
        {
            if (Application.isPlaying)
                return;
            UpdateLibraryColor();
        }

        /// <summary>
        /// 同步刷新颜色信息
        /// </summary>
        private void UpdateLibraryColor()
        {
            if (Painting != null)
            {
                //--如果使用颜色库颜色则将组件的颜色值覆盖为库中选中的颜色，否则使用原始色
                if (Painting.SyncLibraryColor)
                {
                    if (mgr == null)
                    {
                        //--在场景中找到Hud管理器                
                        mgr = FindFirstObjectByType<XHud_Manager>();
                    }

                    //--检查色卡名称是否失效
                    if (!mgr.Hud_Colors.ColorsLibrary_IsExist(Painting.ColoriseName) || string.IsNullOrEmpty(Painting.ColoriseName))
                    {
                        Painting.ColoriseName = mgr.Hud_Colors.ColorsLibrary_GetColorName(0);
                    }

                    //--判断颜色库是否正确配置
                    if (mgr != null && mgr.Hud_Colors != null)
                    {
                        if (mgr.Hud_Colors.ColorLibrary.Count > 0)
                        {
                            Painting.UpdateColor(mgr.Hud_Colors.ColorsLibrary_GetColor(Painting.ColoriseName));
                        }
                    }
                }
                else
                {
                    if (Painting.SyncImageColor)
                        Painting.UpdateColor(Painting.OriginalColor);
                }

                ////做缩放并还原动作来激活颜色变化
                //ActivateScale = Painting.transform.localScale;
                //Painting.transform.localScale = Vector3.zero;
                //Painting.transform.localScale = ActivateScale;
            }
            else
            {
                Painting = GetComponent<XHud_Module_Primitive_Painting>();
            }
        }
    }
}