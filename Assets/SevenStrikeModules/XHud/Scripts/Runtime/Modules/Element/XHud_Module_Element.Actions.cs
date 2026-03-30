namespace SevenStrikeModules.XHud
{
    using UnityEngine;
    using UnityEngine.Events;

    public partial class XHud_Module_Element : MonoBehaviour
    {
        /// <summary>
        /// 动作 - 动画播放 - 入场 - 开始
        /// </summary>
        public UnityAction<XHud_Module_Element> act_on_element_in_start;
        /// <summary>
        /// 动作 - 动画播放 - 入场 - 进度中
        /// </summary>
        public UnityAction<float> act_on_element_in_progress;
        /// <summary>
        /// 动作 - 动画播放 - 入场 - 结束
        /// </summary>
        public UnityAction<XHud_Module_Element> act_on_element_in_end;
        /// <summary>
        /// 动作 - 动画播放 - 出场 - 开始
        /// </summary>
        public UnityAction<XHud_Module_Element> act_on_element_out_start;
        /// <summary>
        /// 动作 - 动画播放 - 出场 - 进度中
        /// </summary>
        public UnityAction<float> act_on_element_out_progress;
        /// <summary>
        /// 动作 - 动画播放 - 出场 - 结束
        /// </summary>
        public UnityAction<XHud_Module_Element> act_on_element_out_end;
    }
}