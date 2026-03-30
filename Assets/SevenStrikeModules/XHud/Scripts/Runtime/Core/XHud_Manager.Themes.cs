namespace SevenStrikeModules.XHud
{
    using UnityEngine;
    using UnityEngine.Events;

    public partial class XHud_Manager : MonoBehaviour
    {
        /// <summary>
        /// 主题 Editor GUI 主色调
        /// </summary>
        [SerializeField]
        public Vector3 theme_color;
        /// <summary>
        /// 主题 Editor GUI 组背景色
        /// </summary>
        [SerializeField]
        public Vector3 theme_color_gp;
        /// <summary>
        /// 主题 Editor GUI 分割线颜色
        /// </summary>
        [SerializeField]
        public Vector3 theme_color_sep;
        /// <summary>
        /// 当前使用的主题方案名称
        /// </summary>
        [SerializeField]
        public string ThemeSolution = "默认";
        /// <summary>
        /// 当前使用的主题边缘处理方案
        /// </summary>
        [SerializeField]
        public string ThemeEdgeSolution = "默认";
    }
}