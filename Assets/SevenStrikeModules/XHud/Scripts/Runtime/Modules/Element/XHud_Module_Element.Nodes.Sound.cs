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
    }
}