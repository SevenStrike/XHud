namespace SevenStrikeModules.XHud.XTween
{
    using SevenStrikeModules.XHud.Hud;
    using UnityEngine;

    public interface util_MonitorData
    {
        /// <summary>
        /// 标题内容
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 数值内容
        /// </summary>
        public string value { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public Hud_TmpText tmp_title { get; set; }
        /// <summary>
        /// 数值
        /// </summary>
        public Hud_TmpText tmp_value { get; set; }
        /// <summary>
        /// 标题颜色
        /// </summary>
        [SerializeField] public Color col_title { get; set; }
        /// <summary>
        /// 数值颜色
        /// </summary>
        [SerializeField] public Color col_value { get; set; }
        /// <summary>
        /// 用于刷新数据
        /// </summary>
        public void UpdateData();
        /// <summary>
        /// 用于初始化
        /// </summary>
        public void Initialize();
    }
}