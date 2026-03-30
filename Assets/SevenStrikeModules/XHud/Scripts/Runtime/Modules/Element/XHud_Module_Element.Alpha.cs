namespace SevenStrikeModules.XHud
{
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.Utilitys;
    using UnityEngine;

    public partial class XHud_Module_Element : MonoBehaviour
    {
        [SerializeField]
        [Range(0f, 1f)]
        public float Alpha = 1;
        [SerializeField]
        /// <summary>
        /// CanvasGroup组件
        /// </summary>
        public CanvasGroup CanvasGroup;

        /// <summary>
        /// XHud元素 - 同步透明度
        /// </summary>
        public virtual void element_AlphaSyncUpdate()
        {
            if (CanvasGroup != null)
            {
                if (Alpha > 0.9999f)
                    CanvasGroup.alpha = 1;
                else if (Alpha <= 0.0001f)
                    CanvasGroup.alpha = 0;
                else
                    CanvasGroup.alpha = Alpha;
            }
        }
        /// <summary>
        /// XHud元素 - 设置透明度
        /// </summary>
        /// <param name="alpha">透明度</param>
        public virtual void element_AlphaSet(float alpha)
        {
            Alpha = alpha;
            element_AlphaSyncUpdate();

            if (DebugState)
                XHud_Utilitys.Func_PrintInfo("元素控件通知", "透明度设置为：" + alpha, HudMsgState.通知);
        }
    }
}