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
    using UnityEngine.Events;

    public partial class XHud_Module_Element : MonoBehaviour
    {
        /// <summary>
        /// 设置标识名称
        /// </summary>
        public XHud_Module_Element SetIndicator(string indicator)
        {
            this.Indicator = indicator;
            return this;
        }
        /// <summary>
        /// 设置位置偏移
        /// </summary>
        public XHud_Module_Element SetOffset(Vector3 offset)
        {
            element_PositionOffset(offset);
            return this;
        }
        /// <summary>
        /// 设置位置偏移 (x, y, z)
        /// </summary>
        public XHud_Module_Element SetOffset(float x, float y, float z)
        {
            return SetOffset(new Vector3(x, y, z));
        }
        /// <summary>
        /// 设置缩放
        /// </summary>
        public XHud_Module_Element SetScale(Vector3 scale)
        {
            element_ScaleSet(scale);
            return this;
        }
        /// <summary>
        /// 设置缩放 (x, y, z)
        /// </summary>
        public XHud_Module_Element SetScale(float x, float y, float z)
        {
            return SetScale(new Vector3(x, y, z));
        }
        /// <summary>
        /// 设置尺寸
        /// </summary>
        public XHud_Module_Element SetSize(Vector2 size)
        {
            element_SizeSet(size);
            return this;
        }
        /// <summary>
        /// 设置尺寸 (width, height)
        /// </summary>
        public XHud_Module_Element SetSize(float width, float height)
        {
            return SetSize(new Vector2(width, height));
        }
        /// <summary>
        /// 启用 RMS 布局适配
        /// </summary>
        public XHud_Module_Element RMSEnabled(bool enabled = true, string rmsName = "")
        {
            element_RMS_Mode_Enabled(enabled);
            if (!string.IsNullOrEmpty(rmsName))
                RMS_Name = rmsName;
            return this;
        }
        /// <summary>
        /// 设置入场动画开始回调
        /// </summary>
        public XHud_Module_Element On_In_Start(UnityAction<XHud_Module_Element> action)
        {
            act_on_element_in_start += action;
            return this;
        }
        /// <summary>
        /// 设置入场动画进度回调
        /// </summary>
        public XHud_Module_Element On_In_Progress(UnityAction<float> action)
        {
            act_on_element_in_progress += action;
            return this;
        }
        /// <summary>
        /// 设置入场动画结束回调
        /// </summary>
        public XHud_Module_Element On_In_End(UnityAction<XHud_Module_Element> action)
        {
            act_on_element_in_end += action;
            return this;
        }
        /// <summary>
        /// 设置出场动画开始回调
        /// </summary>
        public XHud_Module_Element On_Out_Start(UnityAction<XHud_Module_Element> action)
        {
            act_on_element_out_start += action;
            return this;
        }
        /// <summary>
        /// 设置出场动画进度回调
        /// </summary>
        public XHud_Module_Element On_Out_Progress(UnityAction<float> action)
        {
            act_on_element_out_progress += action;
            return this;
        }
        /// <summary>
        /// 设置出场动画结束回调
        /// </summary>
        public XHud_Module_Element On_Out_End(UnityAction<XHud_Module_Element> action)
        {
            act_on_element_out_end += action;
            return this;
        }
        /// <summary>
        /// 手动播放入场动画（当 autoin = false 时使用）
        /// </summary>
        public XHud_Module_Element Element_In(Motion_Creator args = null)
        {
            Element_In(args ?? XHud_Manager.Instance.CreateArgs_Default);
            return this;
        }
        /// <summary>
        /// 手动播放出场动画
        /// </summary>
        public XHud_Module_Element Element_Out(Motion_Recycler args = null)
        {
            Element_Out(args ?? XHud_Manager.Instance.RecycleArgs_Default);
            return this;
        }
    }
}