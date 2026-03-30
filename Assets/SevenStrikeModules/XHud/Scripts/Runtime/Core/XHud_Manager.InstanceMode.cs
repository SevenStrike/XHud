namespace SevenStrikeModules.XHud
{
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.Utilitys;
    using UnityEngine;

    public partial class XHud_Manager : MonoBehaviour
    {
        [Tooltip("单例模式开关")]
        /// <summary>
        /// 单例模式开关
        /// </summary>
        public bool UseInstanceMode = true;

        /// <summary>
        /// XHudManager 构造
        /// </summary>
        protected XHud_Manager() { }
        /// <summary>
        /// XHudManager 构造单例实例（私有）
        /// </summary>
        private static XHud_Manager _instance;
        /// <summary>
        /// XHudManager 构造单例实例（公开）
        /// </summary>
        public static XHud_Manager Instance
        {
            get
            {
                if (_instance == null)
                {
                    XHud_Utilitys.Func_PrintInfo("xHud Manager管理器消息", "HudManager 还没有被实例化！", HudMsgState.确认);
                }
                return _instance;
            }
        }

        /// <summary>
        /// 检测单例状态
        /// </summary>
        private void hm_InstanceModeCheck()
        {
            // 检查是否启用了单例模式
            // 如果 UseInstanceMode 为 false，表示不需要使用单例模式，直接返回，不执行后续代码
            if (!UseInstanceMode)
                return;

            // 检查私有静态字段 _instance 是否已经被赋值（即是否已经存在实例）
            // 如果 _instance 不为 null，说明已经存在一个单例实例
            if (_instance != null)
            {
                // 销毁当前游戏对象
                // 因为已经存在一个有效的单例实例，当前对象是多余的，需要被销毁
                // Destroy 是 Unity 引擎的方法，用于从场景中移除并销毁游戏对象
                Destroy(gameObject);

                // 返回，不再执行后续代码
                // 注意：对象被销毁后，该方法后续代码不应再执行
                return;
            }

            // 如果代码执行到这里，说明 UseInstanceMode 为 true，且 _instance 为 null
            // 即当前是第一次创建单例实例，将当前对象赋值给静态字段 _instance
            _instance = this;

            // 确保当前游戏对象在场景切换时不会被销毁
            // DontDestroyOnLoad 是 Unity 引擎的方法，用于标记游戏对象在加载新场景时保持存在
            // 这样单例实例可以跨场景持续存在，保证全局唯一性
            DontDestroyOnLoad(gameObject);
        }
    }
}