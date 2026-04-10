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
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// 元素音效器的结构类
    /// </summary>
    [System.Serializable]
    public class ElementNode_Sound
    {
        [SerializeField]
        /// <summary>
        /// 音效器
        /// </summary>
        public XHud_Element_Sounder Sounder;
    }

    public partial class XHud_Module_Element : MonoBehaviour
    {
        [SerializeField]
        /// <summary>
        /// 元素音效容器
        /// </summary>
        public List<ElementNode_Sound> SounderNodes;
        [SerializeField]
        /// <summary>
        /// 音效器列表折叠状态
        /// true表示折叠，false表示展开
        /// 用于控制Inspector中音效器列表的显示/隐藏
        /// </summary>
        public bool SounderIsFold;

        /// <summary>
        /// 将所有的音效器委托释放
        /// </summary>
        public void element_SounderUnRegisterAction()
        {
            for (int i = 0; i < SounderNodes.Count; i++)
            {
                SounderNodes[i].Sounder.UnRegisterAction();
            }
        }
    }
}