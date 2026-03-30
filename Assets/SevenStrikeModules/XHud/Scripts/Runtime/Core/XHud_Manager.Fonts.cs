namespace SevenStrikeModules.XHud
{
    using UnityEngine;
    using UnityEngine.Events;

    public partial class XHud_Manager : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("影响所有UI的字体尺寸")]
        /// <summary>
        /// 影响所有UI的字体尺寸
        /// </summary>
        public float FontSizeMultiply = 1;

        /// <summary>
        /// 修改全局字体尺寸
        /// </summary>
        /// <param name="size"></param>
        public void hm_ChangeFontGlobalSize(float size)
        {
            FontSizeMultiply = size;
            if (Act_FontSize_Changed != null)
                Act_FontSize_Changed();
        }
    }
}