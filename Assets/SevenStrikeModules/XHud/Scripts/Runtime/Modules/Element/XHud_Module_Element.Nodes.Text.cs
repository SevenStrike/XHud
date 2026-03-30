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
                    XHud_Utilitys.Func_PrintInfo("元素控件通知", "未找到对应标识的文字！", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("元素控件通知", "已获取到标识为 " + indicator + " 的文字组件！", HudMsgState.通知);
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
                    XHud_Utilitys.Func_PrintInfo("元素控件通知", "获取的文字列表为空！", HudMsgState.错误);
            }
            else
            {
                if (DebugState)
                    XHud_Utilitys.Func_PrintInfo("元素控件通知", "已获取到" + TextNodes.Count + " 个文字组件！", HudMsgState.通知);
            }
            return textlist.ToArray();
        }
    }
}