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
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class Element_RMS_LayoutData
    {
        public string LayoutName;
        public XHudAnchor Anchor;
        public Vector3 Position;
        public Vector3 Euler;
        public Vector3 Scale;
        public Vector2 AnchorMin;
        public Vector2 AnchorMax;
        public Vector2 Pivot;
    }

    public partial class XHud_Module_Element : MonoBehaviour
    {
        [SerializeField]
        /// <summary>
        /// 布局匹配方案是否开启
        /// </summary>
        public bool RMS_Enabled;
        [SerializeField]
        /// <summary>
        ///  布局匹配方案列表
        /// </summary>
        public List<Element_RMS_LayoutData> RMS_LayoutDatas;
        [SerializeField]
        /// <summary>
        ///  布局匹配方案当前选择的标识名称
        /// </summary>
        public string RMS_Name;

        /// <summary>
        /// 根据方案名称获取RMS节点参数
        /// </summary>
        /// <param name="solution"></param>
        public Element_RMS_LayoutData elelemt_RMS_Get(string solution)
        {
            Element_RMS_LayoutData info = null;
            for (int i = 0; i < RMS_LayoutDatas.Count; i++)
            {
                if (solution == RMS_LayoutDatas[i].LayoutName)
                {
                    info = RMS_LayoutDatas[i];
                    break;
                }
            }
            return info;
        }
        /// <summary>
        /// Hud元素 - 是否开启设计布局模式
        /// </summary>
        public void element_RMS_Mode_Enabled(bool state)
        {
            RMS_Enabled = state;
        }
        /// <summary>
        /// 用于响应屏幕分辨率变化时根据元素自身记录的 RMS 布局方案（RMS_Name）重新初始化元素的位置、锚点、缩放等
        /// </summary>
        /// <param name="Indicator"></param>
        /// <param name="Res"></param>
        private void elelemt_RMS_ScreenResolutionChanged(string Indicator, Vector2 Res)
        {
            // 只有开启了 RMS 才处理
            if (!RMS_Enabled)
                return;

            // 检查是否有对应的布局方案
            if (string.IsNullOrEmpty(RMS_Name))
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "RMS已开启但未指定布局方案名称", HudMsgState.警告);
                return;
            }

            XHud_Manager.Instance.hm_ScreenElement_Initialize_By_RMS(this, RMS_Name, true);
        }
    }
}