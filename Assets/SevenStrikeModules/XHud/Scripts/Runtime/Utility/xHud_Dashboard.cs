namespace SevenStrikeModules.XHud
{
    using SevenStrikeModules.XHud.Utilitys;
#if UNITY_EDITOR
    using UnityEditor.Callbacks;
#endif
    using UnityEngine;

    public static class XHud_Dashboard
    {
        #region ThemeColor 主题色
#pragma warning disable CS0414
        private static readonly string PrefsKeyColor_Theme = "XHUD-MANAGER-COLOR-THEME";
        private static readonly string PrefsKeyColor_Theme_GP = "XHUD-MANAGER-COLOR-THEME-GROUP";
        private static readonly string PrefsKeyColor_Theme_SEP = "XHUD-MANAGER-COLOR-THEME-SEPERATE";
#pragma warning restore CS0414
        public static Color Theme_Primary { get; set; } = XHud_Utilitys.Color_From_HexString("#3BFE9B");
        public static Color Theme_Group { get; set; } = XHud_Utilitys.Color_From_HexString("#1E1E1E");
        public static Color Theme_SeperateLine { get; set; } = XHud_Utilitys.Color_From_HexString("#535353");

        public static string Version { get; set; }

#if UNITY_EDITOR
        [DidReloadScripts]
        public static void LoadThemes()
        {
            Color color_theme = XHud_Utilitys.Color_From_HexString("3BFE9B");
            if (!XHud_Utilitys.PlayerPrefs_KeyIsExist_ForEditor(PrefsKeyColor_Theme))
            {
                XHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyColor_Theme, $"{color_theme.r},{color_theme.g},{color_theme.b}");
                Theme_Primary = new Color(color_theme.r, color_theme.g, color_theme.b);
            }
            else
            {
                string colorval = XHud_Utilitys.PlayerPrefs_ReadValue_String_ForEditor(PrefsKeyColor_Theme);
                Color v3 = XHud_Utilitys.Color_From_String(colorval + ",1", false);
                Theme_Primary = v3;
            }

            Color color_theme_gp = XHud_Utilitys.Color_From_HexString("1E1E1E");
            if (!XHud_Utilitys.PlayerPrefs_KeyIsExist_ForEditor(PrefsKeyColor_Theme_GP))
            {
                XHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyColor_Theme_GP, $"{color_theme_gp.r},{color_theme_gp.g},{color_theme_gp.b}");
                Theme_Group = new Color(color_theme_gp.r, color_theme_gp.g, color_theme_gp.b);
            }
            else
            {
                string colorval = XHud_Utilitys.PlayerPrefs_ReadValue_String_ForEditor(PrefsKeyColor_Theme_GP);
                Color v3 = XHud_Utilitys.Color_From_String(colorval + ",1", false);
                Theme_Group = v3;
            }

            Color color_theme_sep = XHud_Utilitys.Color_From_HexString("535353");
            if (!XHud_Utilitys.PlayerPrefs_KeyIsExist_ForEditor(PrefsKeyColor_Theme_SEP))
            {
                XHud_Utilitys.PlayerPrefs_SaveValue_ForEditor(PrefsKeyColor_Theme_SEP, $"{color_theme_sep.r},{color_theme_sep.g},{color_theme_sep.b}");
                Theme_SeperateLine = new Color(color_theme_sep.r, color_theme_sep.g, color_theme_sep.b);
            }
            else
            {
                string colorval = XHud_Utilitys.PlayerPrefs_ReadValue_String_ForEditor(PrefsKeyColor_Theme_SEP);
                Color v3 = XHud_Utilitys.Color_From_String(colorval + ",1", false);
                Theme_SeperateLine = v3;
            }

        }
#endif
        #endregion

        #region 获取HudManger
        /// <summary>
        /// 获取HudManager
        /// </summary>
        /// <returns></returns>
        public static XHud_Manager HudManagerGet()
        {
            if (Application.isPlaying)
                return XHud_Manager.Instance;
            else
                return Transform.FindFirstObjectByType<XHud_Manager>(FindObjectsInactive.Exclude);
        }
        #endregion

        #region 公共路径
        public static string path_XHUD_ROOT = "Assets/SevenStrikeModules/XHud/";
        public static string path_XHUD_GUIROOT = "Assets/SevenStrikeModules/XHud/GUI/";
        public static string path_XHUD_GUISTYLE = "Assets/SevenStrikeModules/XHud/GUI/HudGuiStyle/";
        public static string path_XHUD_MATERIAL = "Assets/SevenStrikeModules/XHud/Materials/";
        public static string path_XHUD_PREFABS = "Assets/SevenStrikeModules/XHud/Prefabs/";
        public static string path_XHUD_SHADERS = "Assets/SevenStrikeModules/XHud/Shaders/";
        public static string path_XHUD_SOUND = "Assets/SevenStrikeModules/XHud/Sound/";
        public static string path_XHUD_SPRITES = "Assets/SevenStrikeModules/XHud/Sprites/";
        public static string path_XHUD_TEXTURES = "Assets/SevenStrikeModules/XHud/Textures/";
        public static string path_XHUD_FONTS = "Assets/SevenStrikeModules/XHud/Fonts/";
        public static string path_XHUD_SCRIPTS = "Assets/SevenStrikeModules/XHud/Scripts/";
        public static string path_XHUD_CONFIG = "Assets/SevenStrikeModules/XHud/Config/";

        #region 路径获取
        /// <summary>
        /// 获取XHUD XHUDROOT路径，根目录：SevenStrikeModules/XHud/
        /// </summary>
        /// <returns></returns>
        public static string Get_Path_XHUD_ROOT()
        {
            return path_XHUD_ROOT;
        }
        /// <summary>
        /// 获取XHUD GUIROOT路径，根目录：SevenStrikeModules/XHud/GUI/
        /// </summary>
        /// <returns></returns>
        public static string Get_Path_XHUD_GUIROOT_Path()
        {
            return path_XHUD_GUIROOT;
        }
        /// <summary>
        /// 获取XHUD GUISTYLE路径，根目录：SevenStrikeModules/XHud/GUI/HudGuiStyle/
        /// </summary>
        /// <returns></returns>
        public static string Get_Path_XHUD_GUISTYLE_Path()
        {
            return path_XHUD_GUISTYLE;
        }
        /// <summary>
        /// 获取XHUD 材质路径，根目录：SevenStrikeModules/XHud/Materials/
        /// </summary>
        /// <returns></returns>
        public static string Get_Path_XHUD_MATERIALS_Path()
        {
            return path_XHUD_MATERIAL;
        }
        /// <summary>
        /// 获取XHUD 预制体路径，根目录：SevenStrikeModules/XHud/Prefabs/
        /// </summary>
        /// <returns></returns>
        public static string Get_Path_XHUD_PREFABS_Path()
        {
            return path_XHUD_PREFABS;
        }
        /// <summary>
        /// 获取XHUD 着色器路径，根目录：SevenStrikeModules/XHud/Shaders/
        /// </summary>
        /// <returns></returns>
        public static string Get_Path_XHUD_SHADERS_Path()
        {
            return path_XHUD_SHADERS;
        }
        /// <summary>
        /// 获取XHUD 声音路径，根目录：SevenStrikeModules/XHud/Sound/
        /// </summary>
        /// <returns></returns>
        public static string Get_Path_XHUD_SOUND_Path()
        {
            return path_XHUD_SOUND;
        }
        /// <summary>
        /// 获取XHUD 精灵路径，根目录：SevenStrikeModules/XHud/Sprites/
        /// </summary>
        /// <returns></returns>
        public static string Get_Path_XHUD_SPRITES_Path()
        {
            return path_XHUD_SPRITES;
        }
        /// <summary>
        /// 获取XHUD 贴图路径，根目录：SevenStrikeModules/XHud/Textures/
        /// </summary>
        /// <returns></returns>
        public static string Get_Path_XHUD_TEXTURES_Path()
        {
            return path_XHUD_TEXTURES;
        }
        /// <summary>
        /// 获取XHUD 字体路径，根目录：SevenStrikeModules/XHud/Fonts/
        /// </summary>
        /// <returns></returns>
        public static string Get_Path_XHUD_FONTS_Path()
        {
            return path_XHUD_FONTS;
        }
        /// <summary>
        /// 获取XHUD 字体路径，根目录：SevenStrikeModules/XHud/Scripts/
        /// </summary>
        /// <returns></returns>
        public static string Get_Path_XHUD_SCRIPTS_Path()
        {
            return path_XHUD_SCRIPTS;
        }
        /// <summary>
        /// 获取XHUD 配置路径，根目录：SevenStrikeModules/XHud/Config/
        /// </summary>
        /// <returns></returns>
        public static string Get_Path_XHUD_CONFIG_Path()
        {
            return path_XHUD_CONFIG;
        }
        #endregion
        #endregion
    }
}