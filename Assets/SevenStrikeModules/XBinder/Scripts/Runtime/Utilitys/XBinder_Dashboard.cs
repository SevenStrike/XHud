/*
 * ============================================================================
 * ⚠ 版权声明（禁止删除、禁止修改、衍生作品必须保留此注释）⚠
 * ============================================================================
 * 版权声明 Copyright (C) 2025-Present Nanjing SevenStrike Media Co., Ltd.
 * 中文名称：南京塞维斯传媒有限公司
 * 英文名称：SevenStrikeMedia
 * 项目作者：徐寅智
 * 项目名称：XBinder - Unity 高级数据绑定管理插件
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
namespace SevenStrikeModules.XBinder
{
    using SevenStrikeModules.XGUI.Runtime;
    using UnityEngine;

    public static class XBinder_Dashboard
    {
        #region 主题色
        public static Color theme_primary = XGUI_Utilitys.HexString_To_Color("#3BFE9B");
        public static Color theme_group = XGUI_Utilitys.HexString_To_Color("#1E1E1E");
        public static Color theme_sepline = XGUI_Utilitys.HexString_To_Color("#535353");
        #endregion

        // 包ID，对应package.json的name字段
        public const string PackageName = "com.sevenstrike.xbinder";
        // 缓存包物理路径，避免反复查询
        private static string _packageRootPath;

        #region 路径获取
        private static string root()
        {
            string root = GetPackageRootAssetPath();
            if (string.IsNullOrEmpty(root)) return string.Empty;
            return root.TrimEnd('/') + "/";
        }
        /// <summary>
        /// 获取 XBinder 资源根目录（带尾部斜杠）。
        /// <list type="bullet">
        ///     <item>包模式：<c>"Packages/com.sevenstrike.xbinder/"</c></item>
        ///     <item>Assets 模式：<c>"Assets/SevenStrikeModules/XBinder/"</c></item>
        ///     <item>非 Editor 环境：空串</c></item>
        /// </list>
        /// </summary>
        public static string get_path_xbinder_root() { return root(); }
        /// <summary>
        /// 获取 GUI 资源目录。
        /// <list type="bullet">
        ///     <item>包模式：<c>"Packages/com.sevenstrike.xbinder/GUI/"</c></item>
        ///     <item>Assets 模式：<c>"Assets/SevenStrikeModules/XBinder/GUI/"</c></item>
        /// </list>
        /// </summary>
        public static string get_path_xbinder_gui() { return root() + "GUI/"; }
        #endregion

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        private static void ResetPackageRootCache()
        {
            _packageRootPath = null;
        }
#endif

        /// <summary>
        /// 获取 XBinder 根目录（Editor 内的 AssetDatabase 路径）
        /// 包模式 → "Packages/com.sevenstrike.xbinder"
        /// Assets 模式 → "Assets/.../XBinder"
        /// </summary>
        public static string GetPackageRootAssetPath()
        {
#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(_packageRootPath))
                return _packageRootPath;

            // 1) 本程序集隶属于某个 UPM 包：缓存包 / 本地磁盘包 / 内嵌包（Packages/）都适用
            UnityEditor.PackageManager.PackageInfo info =
                UnityEditor.PackageManager.PackageInfo.FindForAssembly(typeof(XBinder_Dashboard).Assembly);

            if (info != null && !string.IsNullOrEmpty(info.assetPath))
                return _packageRootPath = info.assetPath;

            // 2) 返回 null 说明脚本位于 Assets 内，从脚本自身位置反推根目录
            string[] guids = UnityEditor.AssetDatabase.FindAssets("XBinder_Dashboard t:MonoScript");
            foreach (string guid in guids)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                if (string.IsNullOrEmpty(path)) continue;
                if (path.StartsWith("Packages/")) continue;
                if (!path.EndsWith("/XBinder_Dashboard.cs")) continue;

                int idx = path.IndexOf("/Scripts/", System.StringComparison.Ordinal);
                if (idx > 0)
                    return _packageRootPath = path.Substring(0, idx);
            }

            // 3) 兜底，兼容旧结构
            return _packageRootPath = "Assets/SevenStrikeModules/XBinder";
#else
            // 构建产物内没有 Editor 资源，调用方不应依赖这些路径
            return string.Empty;
#endif
        }
    }
}