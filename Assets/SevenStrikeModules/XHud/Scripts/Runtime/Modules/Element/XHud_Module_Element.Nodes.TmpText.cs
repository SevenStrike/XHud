namespace SevenStrikeModules.XHud
{
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.Utilitys;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Tmp文字的结构类
    /// </summary>
    [System.Serializable]
    public class ElementNode_TmpText
    {
        /// <summary>
        /// 文字组件
        /// </summary>
        public XHud_Module_TmpText TmpText;
    }

    public partial class XHud_Module_Element : MonoBehaviour
    {
        [SerializeField]
        /// <summary>
        /// Tmp文字容器
        /// </summary>
        public List<ElementNode_TmpText> TmpTextNodes;
        [SerializeField]
        /// <summary>
        /// TMP文字列表折叠状态
        /// true表示折叠，false表示展开
        /// 用于控制Inspector中TMP文字列表的显示/隐藏
        /// </summary>
        public bool TmpTextIsFold;

        /// <summary>
        /// 获取目标Tmp文字
        /// </summary>
        /// <param name="indicator">标识名称</param>
        public XHud_Module_TmpText element_TmpText_Get(string indicator)
        {
            XHud_Module_TmpText tex = null;
            for (int i = 0; i < TmpTextNodes.Count; i++)
            {
                if (TmpTextNodes[i].TmpText.Indicator == indicator)
                {
                    tex = TmpTextNodes[i].TmpText;
                }
            }
            if (tex == null)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("元素控件通知", "未找到对应标识的文字！", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("元素控件通知", "已获取到标识为 " + indicator + " 的Tmp文字！", HudMsgState.通知);
            }
            return tex;
        }
        /// <summary>
        /// 获取所有Tmp文字
        /// </summary>
        public XHud_Module_TmpText[] element_TmpText_GetAll()
        {
            List<XHud_Module_TmpText> textlist = new List<XHud_Module_TmpText>();
            for (int i = 0; i < TmpTextNodes.Count; i++)
            {
                textlist.Add(TmpTextNodes[i].TmpText);
            }
            if (textlist.Count <= 0)
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("元素控件通知", "获取的文字列表为空！", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("元素控件通知", "已获取到" + TmpTextNodes.Count + " 个文字组件！", HudMsgState.通知);
            }
            return textlist.ToArray();
        }
    }
}