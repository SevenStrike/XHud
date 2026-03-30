namespace SevenStrikeModules.XHud
{
    using UnityEngine;

    public partial class XHud_Module_Element : MonoBehaviour
    {
        [SerializeField]
        /// <summary>
        /// 动作器
        /// </summary>
        public XHud_Element_TriggerAction TriggerAction;

        /// <summary>
        /// 检测是否存在元素动作器
        /// </summary>
        /// <returns></returns>
        public bool element_TriggerActionIsExist()
        {
            if (TriggerAction != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 获取元素动作器
        /// </summary>
        /// <returns></returns>
        public XHud_Element_TriggerAction element_GetTriggerAction()
        {
            if (element_TriggerActionIsExist())
                return TriggerAction;
            else
                return null;
        }
    }
}