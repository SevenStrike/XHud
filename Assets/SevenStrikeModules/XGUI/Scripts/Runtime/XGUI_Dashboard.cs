/*
 * ============================================================================
 * ⚠️ 版权声明（禁止删除、禁止修改、衍生作品必须保留此注释）⚠️
 * ============================================================================
 * 版权声明 Copyright (C) 2025-Present Nanjing SevenStrike Media Co., Ltd.
 * 中文名称：南京塞维斯传媒有限公司
 * 英文名称：SevenStrikeMedia
 * 项目作者：徐寅智
 * 项目名称：XGUI - Unity Editor界面可视化组件工具
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
namespace SevenStrikeModules.XGUI.Runtime
{
    using UnityEngine;

    public static class XGUI_Dashboard
    {
        #region 预获取字体
        public static Font[] fontlist;
        #endregion

        #region 公共路径
        public static string path_ROOT = "Assets/SevenStrikeModules/XGUI/";
        public static string path_FONTS = "Assets/SevenStrikeModules/XGUI/Fonts/";
        public static string path_SHAPES = "Assets/SevenStrikeModules/XGUI/Shapes/";
        public static string path_ICONS = "Assets/SevenStrikeModules/XGUI/Icons/";
        #endregion

        #region 路径获取
        /// <summary>
        /// 获取 XGUI ROOT路径，根目录：SevenStrikeModules/XGUI/
        /// </summary>
        /// <returns></returns>
        public static string get_path_xgui_root()
        {
            return path_ROOT;
        }
        /// <summary>
        /// 获取 XGUI 字体路径，根目录：Assets/SevenStrikeModules/XGUI/Fonts/
        /// </summary>
        /// <returns></returns>
        public static string get_path_xgui_fonts()
        {
            return path_FONTS;
        }
        /// <summary>
        /// 获取 XGUI 图形路径，根目录：Assets/SevenStrikeModules/XGUI/Shapes/
        /// </summary>
        /// <returns></returns>
        public static string get_path_xgui_shapes()
        {
            return path_SHAPES;
        }
        /// <summary>
        /// 获取 XGUI 图形路径，根目录：Assets/SevenStrikeModules/XGUI/Icons/
        /// </summary>
        /// <returns></returns>
        public static string get_path_xgui_icons()
        {
            return path_ICONS;
        }
        #endregion
    }
}