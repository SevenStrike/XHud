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

        #region 通用类配置 - 路径
        /// <summary>
        /// XBinder 目录：<c>Assets/SevenStrikeModules/XBinder/</c>
        /// </summary>
        public static string path_XBinder_ROOT = "Assets/SevenStrikeModules/XBinder/";
        /// <summary>
        /// GUI 目录：<c>Assets/SevenStrikeModules/XBinder/GUI/</c>
        /// </summary>
        public static string path_XBinder_GUIROOT = "Assets/SevenStrikeModules/XBinder/GUI/";
        /// <summary>
        /// Demo 目录：<c>Assets/SevenStrikeModules/XBinder/Demo/</c>
        /// </summary>
        public static string path_XBinder_Demo = "Assets/SevenStrikeModules/XBinder/Demo/";
        /// <summary>
        /// Scripts 目录：<c>Assets/SevenStrikeModules/XBinder/Scripts/</c>
        /// </summary>
        public static string path_XBinder_Scripts = "Assets/SevenStrikeModules/XBinder/Scripts/";
        /// <summary>
        /// Sprite 目录：<c>Assets/SevenStrikeModules/XBinder/Sprite/</c>
        /// </summary>
        public static string path_XBinder_Sprite = "Assets/SevenStrikeModules/XBinder/Sprite/";

        /// <summary>获取 XBinder 根目录路径</summary>
        /// <returns><c>Assets/SevenStrikeModules/XBinder/</c></returns>
        public static string Get_Path_XBinder_ROOT()
        {
            return path_XBinder_ROOT;
        }
        /// <summary>获取 XBinder GUIROOT 目录路径</summary>
        /// <returns><c>Assets/SevenStrikeModules/XBinder/GUI/</c></returns>
        public static string Get_Path_XBinder_GUIROOT()
        {
            return path_XBinder_GUIROOT;
        }
        /// <summary>获取 XBinder  Demo 目录路径</summary>
        /// <returns><c>Assets/SevenStrikeModules/XBinder/Demo/</c></returns>
        public static string Get_Path_XBinder_Demo()
        {
            return path_XBinder_Demo;
        }
        /// <summary>获取 XBinder Scripts 目录路径</summary>
        /// <returns><c>Assets/SevenStrikeModules/XBinder/Scripts/</c></returns>
        public static string Get_Path_XBinder_Scripts()
        {
            return path_XBinder_Scripts;
        }
        /// <summary>获取 XBinder  Sprite 目录路径</summary>
        /// <returns><c>AAssets/SevenStrikeModules/XBinder/Sprite/</c></returns>
        public static string Get_Path_XBinder_Sprite()
        {
            return path_XBinder_Sprite;
        }
        #endregion
    }
}