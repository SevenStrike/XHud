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

    /// <summary>
    /// 文字的结构类
    /// </summary>
    [System.Serializable]
    public class ElementNode_Text
    {
        [SerializeField]
        /// <summary>
        /// 文字组件
        /// </summary>
        public XHud_Module_Text Text;
    }

    public partial class XHud_Module_Element : MonoBehaviour
    {
        [SerializeField]
        /// <summary>
        /// 文字容器
        /// </summary>
        public List<ElementNode_Text> TextNodes;
        [SerializeField]
        /// <summary>
        /// 文字列表折叠状态
        /// true表示折叠，false表示展开
        /// 用于控制Inspector中Text文字列表的显示/隐藏
        /// </summary>
        public bool TextIsFold;

        /// <summary>
        /// 获取目标文字
        /// </summary>
        /// <param name="indicator">标识名称</param>
        public XHud_Module_Text element_Text_Get(string indicator)
        {
            XHud_Module_Text tex = null;
            for (int i = 0; i < TextNodes.Count; i++)
            {
                if (TextNodes[i].Text.Indicator == indicator)
                {
                    tex = TextNodes[i].Text;
                }
            }
            if (tex == null)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "未找到对应标识的文字！", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "已获取到标识为 " + indicator + " 的文字组件！", HudMsgState.通知);
            }
            return tex;
        }
        /// <summary>
        /// 获取所有文字
        /// </summary>
        public XHud_Module_Text[] element_Text_GetAll()
        {
            List<XHud_Module_Text> textlist = new List<XHud_Module_Text>();
            for (int i = 0; i < TextNodes.Count; i++)
            {
                textlist.Add(TextNodes[i].Text);
            }
            if (textlist.Count <= 0)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "获取的文字列表为空！", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("XHud - 元素控件通知", "已获取到" + TextNodes.Count + " 个文字组件！", HudMsgState.通知);
            }
            return textlist.ToArray();
        }
    }
}