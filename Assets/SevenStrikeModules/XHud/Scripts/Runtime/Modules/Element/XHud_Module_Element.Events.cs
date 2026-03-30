namespace SevenStrikeModules.XHud
{
    using UnityEngine;
    using UnityEngine.Events;

    public partial class XHud_Module_Element : MonoBehaviour
    {
        [SerializeField]
        /// <summary>
        /// 事件列表折叠状态
        /// true表示折叠，false表示展开
        /// 用于控制Inspector中事件列表的显示/隐藏
        /// </summary>
        public bool EventIsFold;

        /// <summary>
        /// 动作 - 动画播放 - 入场 - 开始
        /// </summary>
        public UnityEvent eve_on_element_in_start;
        /// <summary>
        /// 动作 - 动画播放 - 入场 - 结束
        /// </summary>
        public UnityEvent eve_on_element_in_end;
        /// <summary>
        /// 动作 - 动画播放 - 出场 - 开始
        /// </summary>
        public UnityEvent eve_on_element_out_start;
        /// <summary>
        /// 动作 - 动画播放 - 出场 - 结束
        /// </summary>
        public UnityEvent eve_on_element_out_end;
    }
}