/*
 * ============================================================================
 * ⚠️ 版权声明（禁止删除、禁止修改、衍生作品必须保留此注释）⚠️
 * ============================================================================
 * 版权声明 Copyright (C) 2025-Present Nanjing SevenStrike Media Co., Ltd.
 * 中文名称：南京塞维斯传媒有限公司
 * 英文名称：SevenStrikeMedia
 * 项目作者：徐寅智
 * 项目名称：XHud - Unity UGUI 高级管理架构插件
 * 项目启动：2025年8月
 * 官方网站：http://sevenstrike.com/
 * 授权协议：GNU Affero General Public License Version 3 (AGPL 3.0)
 * 协议说明：
 * 1. 你可以自由使用、修改、分发本插件的源代码，但必须保留此版权注释
 * 2. 基于本插件修改后的衍生作品，必须同样遵循 AGPL 3.0 授权协议
 * 3. 若将本插件用于网络服务（如云端Unity编辑器、在线动效生成工具），必须公开修改后的完整源代码
 * 4. 完整协议文本可查阅：https://www.gnu.org/licenses/agpl-3.0.html
 * ============================================================================
 * 违反本注释保留要求，将违反 AGPL 3.0 授权协议，需承担相应法律责任
 */
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
                    XHud_Utilitys.Func_PrintInfo("XHud - 管理器通知", "HudManager 还没有被实例化！", HudMsgState.确认);
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