/*
 * ============================================================================
 * ⚠️ 版权声明（禁止删除、禁止修改、衍生作品必须保留此注释）⚠️
 * ============================================================================
 * 版权声明 Copyright (C) 2025-Present Nanjing SevenStrike Media Co., Ltd.
 * 中文名称：南京塞维斯传媒有限公司
 * 英文名称：SevenStrikeMedia
 * 项目作者：徐寅智
 * 项目名称：XTween - Unity 高性能动画架构插件
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
namespace SevenStrikeModules.XTween
{
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.Callbacks;
#endif
    using UnityEngine;

    public static class XTween_Dashboard
    {
        /// <summary>
        /// 动画全局速率缩放
        /// </summary>
        public static float DurationMultiply = 1;

        /// <summary>
        /// XTween配置数据
        /// </summary>
        [SerializeField]
        public static XTween_Config XTweenConfig;

        #region ThemeColor 主题色
        public static Color Theme_Primary { get; set; } = new Color(0.2313726f, 0.9960784f, 0.6078432f, 1);
        public static Color Theme_Group { get; set; } = new Color(0.1176471f, 0.1176471f, 0.1176471f, 1);
        public static Color Theme_SeperateLine { get; set; } = new Color(0.3254902f, 0.3254902f, 0.3254902f, 1);

        public static string Version { get; set; }

#if UNITY_EDITOR
        [DidReloadScripts]
#endif
        public static void LoadColors_Themes()
        {
            GetXTweenConfig();

            Theme_Primary = XTweenConfig.Datas.Theme_Primary;
            Theme_Group = XTweenConfig.Datas.Theme_Group;
            Theme_SeperateLine = XTweenConfig.Datas.Theme_SeperateLine;
        }
        #endregion

        #region 公共路径
        public readonly static string path_XTween_GUIROOT = "Assets/SevenStrikeModules/XTween/GUI/";
        public readonly static string path_XTween_ROOT = "Assets/SevenStrikeModules/XTween/";
        public readonly static string path_XTween_MATERIAL = "Assets/SevenStrikeModules/XTween/Materials/";
        public readonly static string path_XTween_CONFIG = "Assets/SevenStrikeModules/XTween/Resources/Config/";
        public readonly static string path_XTween_PRESETS = "Assets/SevenStrikeModules/XTween/Resources/Presets/";
        public readonly static string path_XTween_PREFABS = "Assets/SevenStrikeModules/XTween/Prefabs/";
        public readonly static string path_XTween_SHADERS = "Assets/SevenStrikeModules/XTween/Shaders/";
        public readonly static string path_XTween_SOUND = "Assets/SevenStrikeModules/XTween/Sound/";
        public readonly static string path_XTween_SPRITES = "Assets/SevenStrikeModules/XTween/Sprites/";
        public readonly static string path_XTween_TEXTURES = "Assets/SevenStrikeModules/XTween/Textures/";
        public readonly static string path_XTween_FONTS = "Assets/SevenStrikeModules/XTween/Fonts/";
        public readonly static string path_XTween_SCRIPTS = "Assets/SevenStrikeModules/XTween/Scripts/";

        #region 路径获取
        /// <summary>
        /// 获取 XTween ROOT路径，根目录：SevenStrikeModules/XTween/
        /// </summary>
        /// <returns></returns>
        public static string Get_XTween_Root_Path()
        {
            return path_XTween_ROOT;
        }
        /// <summary>
        /// 获取 XTween GUISTYLE路径，根目录：SevenStrikeModules/XTween/GUI/
        /// </summary>
        /// <returns></returns>
        public static string Get_XTween_GUIRoot_Path()
        {
            return path_XTween_GUIROOT;
        }
        /// <summary>
        /// 获取 XTween 配置路径，根目录：SevenStrikeModules/XTween/Resources/Config/
        /// </summary>
        /// <returns></returns>
        public static string Get_path_XTween_Config_Path()
        {
            return path_XTween_CONFIG;
        }
        /// <summary>
        /// 获取 XTween 预设路径，根目录：SevenStrikeModules/XTween/Resources/Presets/
        /// </summary>
        /// <returns></returns>
        public static string Get_path_XTween_Presets_Path()
        {
            return path_XTween_PRESETS;
        }
        /// <summary>
        /// 获取 XTween 材质路径，根目录：SevenStrikeModules/XTween/Materials/
        /// </summary>
        /// <returns></returns>
        public static string Get_path_XTween_Materials_Path()
        {
            return path_XTween_MATERIAL;
        }
        /// <summary>
        /// 获取 XTween 预制体路径，根目录：SevenStrikeModules/XTween/Prefabs/
        /// </summary>
        /// <returns></returns>
        public static string Get_path_XTween_Prefabs_Path()
        {
            return path_XTween_PREFABS;
        }
        /// <summary>
        /// 获取 XTween 着色器路径，根目录：SevenStrikeModules/XTween/Shaders/
        /// </summary>
        /// <returns></returns>
        public static string Get_path_XTween_Shaders_Path()
        {
            return path_XTween_SHADERS;
        }
        /// <summary>
        /// 获取 XTween 声音路径，根目录：SevenStrikeModules/XTween/Sound/
        /// </summary>
        /// <returns></returns>
        public static string Get_path_XTween_Sound_Path()
        {
            return path_XTween_SOUND;
        }
        /// <summary>
        /// 获取 XTween 精灵路径，根目录：SevenStrikeModules/XTween/Sprites/
        /// </summary>
        /// <returns></returns>
        public static string Get_path_XTween_Sprites_Path()
        {
            return path_XTween_SPRITES;
        }
        /// <summary>
        /// 获取 XTween 贴图路径，根目录：SevenStrikeModules/XTween/Textures/
        /// </summary>
        /// <returns></returns>
        public static string Get_path_XTween_Textures_Path()
        {
            return path_XTween_TEXTURES;
        }
        /// <summary>
        /// 获取 XTween 字体路径，根目录：SevenStrikeModules/XTween/Fonts/
        /// </summary>
        /// <returns></returns>
        public static string Get_path_XTween_Fonts_Path()
        {
            return path_XTween_FONTS;
        }
        /// <summary>
        /// 获取 XTween 脚本路径，根目录：SevenStrikeModules/XTween/Scripts/
        /// </summary>
        /// <returns></returns>
        public static string Get_path_XTween_Scripts_Path()
        {
            return path_XTween_SCRIPTS;
        }
        #endregion
        #endregion

        #region 液晶面板预览样式配置      
        public static Color LiquidColor_Playing { get; set; } = new Color(0.5803922f, 0.6745098f, 0.3490196f, 1);
        public static Color LiquidColor_Idle { get; set; } = new Color(0.4666667f, 0.5176471f, 0.3372549f, 1);

#if UNITY_EDITOR
        [InitializeOnEnterPlayMode]
        [DidReloadScripts]
#endif
        public static void LoadColors_Liquid()
        {
            GetXTweenConfig();

            LiquidColor_Playing = XTweenConfig.Datas.LiquidColor_Playing;
            LiquidColor_Idle = XTweenConfig.Datas.LiquidColor_Idle;
        }

        #endregion

        #region 读取配置文件参数
        public static XTween_Config GetXTweenConfig()
        {
            //获取配置文件
            XTween_Config config = Resources.Load<XTween_Config>("Config/XTweenConfig");

            XTweenConfig = config;
            return config;
        }
        #endregion

        #region 动画预览选项
        /// <summary>
        /// 获取状态 - 自动杀死预览动画
        /// </summary>
        /// <returns></returns>
        public static bool Get_PreviewOption_AutoKillPreviewTweens()
        {
            if (XTweenConfig == null)
                GetXTweenConfig();

            // 额外容错：避免加载配置失败后仍为null
            return XTweenConfig?.Datas.PreviewOption_AutoKillPreviewTweens ?? false;
        }

        /// <summary>
        /// 获取状态 - 预览杀死前重置动画
        /// </summary>
        /// <returns></returns>
        public static bool Get_PreviewOption_RewindPreviewTweensWithKill()
        {
            if (XTweenConfig == null)
            {
                GetXTweenConfig();
            }
            return XTweenConfig?.Datas.PreviewOption_RewindPreviewTweensWithKill ?? false;
        }

        /// <summary>
        /// 获取状态 - 预览杀死后清空预览列表
        /// </summary>
        /// <returns></returns>
        public static bool Get_PreviewOption_ClearPreviewTweensWithKill()
        {
            if (XTweenConfig == null)
            {
                GetXTweenConfig();
            }
            return XTweenConfig?.Datas.PreviewOption_ClearPreviewTweensWithKill ?? false;
        }

        /// <summary>
        /// 设置状态 - 自动杀死预览动画
        /// </summary>
        /// <returns></returns>
        public static void Set_PreviewOption_AutoKillPreviewTweens(bool state)
        {
            XTweenConfig.Datas.PreviewOption_AutoKillPreviewTweens = state;
        }

        /// <summary>
        /// 设置状态 - 预览杀死前重置动画
        /// </summary>
        /// <returns></returns>
        public static void Set_PreviewOption_RewindPreviewTweensWithKill(bool state)
        {
            XTweenConfig.Datas.PreviewOption_RewindPreviewTweensWithKill = state;
        }

        /// <summary>
        /// 设置状态 - 预览杀死后清空预览列表
        /// </summary>
        /// <returns></returns>
        public static void Set_PreviewOption_ClearPreviewTweensWithKill(bool state)
        {
            XTweenConfig.Datas.PreviewOption_ClearPreviewTweensWithKill = state;
        }

        /// <summary>
        /// 设置状态 - 预览面板极简化
        /// </summary>
        /// <returns></returns>
        public static void Set_PerformanceLiquidMode(bool state)
        {
            XTweenConfig.Datas.PerformanceLiquidMode = state;
        }
        #endregion

        /// <summary>
        /// 设置动画全局速率缩放
        /// </summary>
        /// <param name="value"></param>
        public static void SetGlobalDurationMultiply(float value)
        {
            DurationMultiply = value;
        }
    }
}