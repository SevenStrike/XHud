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
namespace SevenStrikeModules.XGUI.Editor
{
    using SevenStrikeModules.XGUI.Runtime;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography;
    using UnityEditor;
    using UnityEngine;
    using static XGUI_EaseCurveGenerator;
    using Color = UnityEngine.Color;
    using Font = UnityEngine.Font;

    /// <summary>
    /// 为 Editor 界面提供样式控件
    /// </summary>
    public static partial class XGUI
    {
        /// <summary>
        /// GUI样式
        /// </summary>
        public static GUISkin GUICreator;

        /// <summary>
        /// 初始化样式文件
        /// </summary>
        public static void Gui_Layout_Initia()
        {
            GUICreator = AssetDatabase.LoadAssetAtPath<GUISkin>($"{XGUI_Dashboard.get_path_xgui_root()}XGUI.guiskin");

            InitialFontsGet();
            InitialFillTextures();
            InitialBtnFillTextures();
            InitialBasedTextures();
            InitialCurves();
        }

        #region GUI Style
        /// <summary>
        /// 样式 - 容器
        /// </summary>
        public static GUIStyle style_xg_group
        {
            get
            {
                if (GUICreator == null)
                {
                    Gui_Layout_Initia();
                }

                GUIStyle s = new GUIStyle(GUICreator.GetStyle("xg_group"));
                return s;
            }
        }
        /// <summary>
        /// 样式 - 区域
        /// </summary>
        public static GUIStyle style_xg_area
        {
            get
            {
                if (GUICreator == null)
                {
                    Gui_Layout_Initia();
                }

                GUIStyle s = new GUIStyle(GUICreator.GetStyle("xg_area"));
                return s;
            }
        }
        /// <summary>
        /// 样式 - 横幅
        /// </summary>
        public static GUIStyle style_xg_box
        {
            get
            {
                if (GUICreator == null)
                {
                    Gui_Layout_Initia();
                }

                GUIStyle s = new GUIStyle(GUICreator.GetStyle("xg_box"));
                return s;
            }
        }
        /// <summary>
        /// 样式 - 图标
        /// </summary>
        public static GUIStyle style_xg_icon
        {
            get
            {
                if (GUICreator == null)
                {
                    Gui_Layout_Initia();
                }

                GUIStyle gs = new GUIStyle(GUICreator.GetStyle("xg_icon"));
                return gs;
            }
        }
        /// <summary>
        /// 样式 - 文字标签
        /// </summary>
        public static GUIStyle style_xg_label
        {
            get
            {
                if (GUICreator == null)
                {
                    Gui_Layout_Initia();
                }

                GUIStyle s = new GUIStyle(GUICreator.GetStyle("xg_label"));
                return s;
            }
        }
        /// <summary>
        /// 样式 - 进度条
        /// </summary>
        public static GUIStyle style_xg_progress
        {
            get
            {
                if (GUICreator == null)
                {
                    Gui_Layout_Initia();
                }

                GUIStyle s = new GUIStyle(GUICreator.GetStyle("xg_progress"));
                return s;
            }
        }
        /// <summary>
        /// 样式 - 分割线
        /// </summary>
        public static GUIStyle style_xg_seperate
        {
            get
            {
                if (GUICreator == null)
                {
                    Gui_Layout_Initia();
                }

                GUIStyle s = new GUIStyle(GUICreator.GetStyle("xg_seperate"));
                return s;
            }
        }
        /// <summary>
        /// 样式 - 按钮
        /// </summary>
        public static GUIStyle style_xg_button
        {
            get
            {
                if (GUICreator == null)
                {
                    Gui_Layout_Initia();
                }

                GUIStyle s = new GUIStyle(GUICreator.GetStyle("xg_button"));
                return s;
            }
        }
        /// <summary>
        /// 样式 - 工具条
        /// </summary>
        public static GUIStyle style_xg_toolbar
        {
            get
            {
                if (GUICreator == null)
                {
                    Gui_Layout_Initia();
                }

                GUIStyle s = new GUIStyle(GUICreator.GetStyle("xg_toolbar"));
                return s;
            }
        }
        /// <summary>
        /// 样式 - 弹窗
        /// </summary>
        public static GUIStyle style_xg_popup
        {
            get
            {
                if (GUICreator == null)
                {
                    Gui_Layout_Initia();
                }

                GUIStyle s = new GUIStyle(GUICreator.GetStyle("xg_popup"));
                return s;
            }
        }
        /// <summary>
        /// 样式 - 属性框
        /// </summary>
        public static GUIStyle style_xg_prop
        {
            get
            {
                if (GUICreator == null)
                {
                    Gui_Layout_Initia();
                }

                GUIStyle s = new GUIStyle(GUICreator.GetStyle("xg_prop"));
                return s;
            }
        }
        /// <summary>
        /// 样式 - 开关
        /// </summary>
        public static GUIStyle style_xg_toggle
        {
            get
            {
                if (GUICreator == null)
                {
                    Gui_Layout_Initia();
                }

                GUIStyle s = new GUIStyle(GUICreator.GetStyle("xg_toggle"));
                return s;
            }
        }
        /// <summary>
        /// 样式 - 开关 - 控制柄
        /// </summary>
        public static GUIStyle style_xg_toggle_handler
        {
            get
            {
                if (GUICreator == null)
                {
                    Gui_Layout_Initia();
                }

                GUIStyle s = new GUIStyle(GUICreator.GetStyle("xg_toggle_handler"));
                return s;
            }
        }
        /// <summary>
        /// 样式 - 文字输入
        /// </summary>
        public static GUIStyle style_xg_input
        {
            get
            {
                if (GUICreator == null)
                {
                    Gui_Layout_Initia();
                }

                GUIStyle s = new GUIStyle(GUICreator.GetStyle("xg_input"));
                return s;
            }
        }
        #endregion

        #region Preget Font
        /// <summary>
        /// 缓存已加载的字体列表，避免重复加载。
        /// </summary>
        private static Font[] fontlist;
        /// <summary>
        /// 初始化并加载 XGUI 字体文件夹中的所有字体资源。
        /// </summary>
        /// <remarks>
        /// 该方法会从 <see cref="XGUI_Dashboard.get_path_xgui_fonts"/> 路径加载所有 .ttf 和 .otf 字体文件，
        /// 并存储到 <see cref="fontlist"/> 中供后续使用。加载失败时会记录错误日志并返回空数组。
        /// </remarks>
        public static void InitialFontsGet()
        {
            try
            {
                var fonts = XGUI.LoadAllAssetsAtPathWithPattern<Font>(
                    XGUI_Dashboard.get_path_xgui_fonts(), ".ttf", ".otf");
                fontlist = fonts?.ToArray() ?? new Font[0];
            }
            catch (Exception e)
            {
                Debug.LogError($"XGUI - 字体加载失败: {e.Message}");
                fontlist = new Font[0];
            }
        }
        #endregion

        #region Preget Bg
        /// <summary>
        /// 缓存已加载的填充纹理（Group 样式），键为 "{模式}_{颜色}"。
        /// </summary>
        private static Dictionary<string, Texture2D> fillTextureCache = new Dictionary<string, Texture2D>();
        /// <summary>
        /// 缓存已加载的开关纹理（Toggle 样式），键为纹理名称。
        /// </summary>
        private static Dictionary<string, Texture2D> togglesTextureCache = new Dictionary<string, Texture2D>();
        /// <summary>
        /// 缓存已加载的按钮填充纹理（Button 样式），键为 "{模式}_{颜色}" 或 "透明_无"。
        /// </summary>
        private static Dictionary<string, Texture2D> btnFillTextureCache = new Dictionary<string, Texture2D>();
        /// <summary>
        /// 缓存已加载的基础图标纹理（Based 图标），键为纹理名称。
        /// </summary>
        private static Dictionary<string, Texture2D> basedTextureCache = new Dictionary<string, Texture2D>();
        /// <summary>
        /// 缓存已加载的帮助框图标纹理（Helpbox 图标），键为纹理名称。
        /// </summary>
        private static Dictionary<string, Texture2D> helpboxTextureCache = new Dictionary<string, Texture2D>();

        /// <summary>
        /// 初始化并缓存 Group 样式填充纹理。
        /// </summary>
        /// <remarks>
        /// 该方法会遍历所有 <see cref="XGUIFilled"/> 模式（实体、纯色边框、边框、缺口边框、缺口纯色边框）
        /// 和所有 <see cref="XGUIColor"/> 颜色组合，从 "XGUI/Shapes/Group/" 路径加载对应的 PNG 纹理，
        /// 并存储到 <see cref="fillTextureCache"/> 中供后续快速访问。
        /// </remarks>
        public static void InitialFillTextures()
        {
            fillTextureCache.Clear();

            string shapesPath = XGUI_Dashboard.get_path_xgui_shapes() + "Group/";
            XGUIFilled[] modes =
            {
                XGUIFilled.实体,
                XGUIFilled.纯色边框,
                XGUIFilled.边框,
                XGUIFilled.缺口边框,
                XGUIFilled.缺口纯色边框
            };
            XGUIColor[] colors =
            {
                XGUIColor.深空灰,
                XGUIColor.阴影灰,
                XGUIColor.亮白,
                XGUIColor.柠檬绿,
                XGUIColor.工业蓝,
                XGUIColor.警示黄,
                XGUIColor.玫瑰粉,
                XGUIColor.神秘紫,
                XGUIColor.魅力红,
                XGUIColor.灰绿,
                XGUIColor.亮橘红,
                XGUIColor.枪灰,
                XGUIColor.亮金色,
                XGUIColor.沉暗红,
                XGUIColor.烟灰蓝,
                XGUIColor.健康绿,
                XGUIColor.浅灰
            };

            // 预加载普通纹理
            foreach (XGUIFilled mode in modes)
            {
                foreach (XGUIColor col in colors)
                {
                    string normalKey = $"{mode}_{col}";
                    string path = shapesPath + $"Group_{mode}_{col}.png";
                    Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                    if (tex != null)
                        fillTextureCache[normalKey] = tex;
                }
            }
        }
        /// <summary>
        /// 初始化并缓存 Button 样式填充纹理。
        /// </summary>
        /// <remarks>
        /// 该方法会遍历所有 <see cref="XGUIFilled"/> 模式（实体、纯色边框、边框）
        /// 和所有 <see cref="XGUIColor"/> 颜色组合，从 "XGUI/Shapes/Button/" 路径加载对应的 PNG 纹理。
        /// 透明按钮作为特殊项单独加载，所有纹理存储到 <see cref="btnFillTextureCache"/> 中供后续快速访问。
        /// </remarks>
        public static void InitialBtnFillTextures()
        {
            btnFillTextureCache.Clear();

            string btnPath = XGUI_Dashboard.get_path_xgui_shapes() + "Button/";
            XGUIFilled[] modes =
            {
                XGUIFilled.实体,
                XGUIFilled.纯色边框,
                XGUIFilled.边框
            };
            XGUIColor[] colors =
            {
                XGUIColor.深空灰,
                XGUIColor.阴影灰,
                XGUIColor.亮白,
                XGUIColor.柠檬绿,
                XGUIColor.工业蓝,
                XGUIColor.警示黄,
                XGUIColor.玫瑰粉,
                XGUIColor.神秘紫,
                XGUIColor.魅力红,
                XGUIColor.灰绿,
                XGUIColor.亮橘红,
                XGUIColor.枪灰,
                XGUIColor.亮金色,
                XGUIColor.沉暗红,
                XGUIColor.烟灰蓝,
                XGUIColor.健康绿,
                XGUIColor.浅灰
            };

            // 预加载透明按钮（特殊处理）
            string transparentPath = btnPath + "Btn_透明.png";
            Texture2D transparentTex = AssetDatabase.LoadAssetAtPath<Texture2D>(transparentPath);
            if (transparentTex != null)
                btnFillTextureCache["透明_无"] = transparentTex;

            // 预加载普通按钮纹理
            foreach (XGUIFilled mode in modes)
            {
                foreach (XGUIColor color in colors)
                {
                    string key = $"{mode}_{color}";
                    string path = btnPath + $"Btn_{mode}_{color}.png";
                    Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                    if (tex != null)
                        btnFillTextureCache[key] = tex;
                }
            }
        }
        /// <summary>
        /// 初始化并缓存 Based 基础图标纹理。
        /// </summary>
        /// <remarks>
        /// 该方法会从 "XGUI/Icons/Based/" 路径加载所有 PNG 图标文件，
        /// 并以文件名作为键存储到 <see cref="basedTextureCache"/> 中供后续快速访问。
        /// </remarks>
        public static void InitialBasedTextures()
        {
            basedTextureCache.Clear();
            string iconPath = XGUI_Dashboard.get_path_xgui_icons() + "Based/";
            List<Texture2D> texs = XGUI.LoadAllAssetsAtPathWithPattern<Texture2D>(iconPath, ".png");

            foreach (Texture2D tex in texs)
            {
                string normalKey = tex.name;
                if (tex != null)
                    basedTextureCache[normalKey] = tex;
            }
        }
        /// <summary>
        /// 初始化并缓存 Helpbox 帮助框图标纹理。
        /// </summary>
        /// <remarks>
        /// 该方法会从 "XGUI/Icons/Helpbox/" 路径加载所有 PNG 图标文件，
        /// 并以文件名作为键存储到 <see cref="helpboxTextureCache"/> 中供后续快速访问。
        /// </remarks>
        public static void InitialHelpboxTextures()
        {
            helpboxTextureCache.Clear();
            string iconPath = XGUI_Dashboard.get_path_xgui_icons() + "Helpbox/";
            List<Texture2D> texs = XGUI.LoadAllAssetsAtPathWithPattern<Texture2D>(iconPath, ".png");

            foreach (Texture2D tex in texs)
            {
                string normalKey = tex.name;
                if (tex != null)
                    helpboxTextureCache[normalKey] = tex;
            }
        }
        /// <summary>
        /// 初始化并缓存 Toggle 开关样式纹理。
        /// </summary>
        /// <remarks>
        /// 该方法会从 "XGUI/Shapes/Toggle/" 路径加载所有 PNG 纹理文件，
        /// 并以文件名作为键存储到 <see cref="togglesTextureCache"/> 中供后续快速访问。
        /// </remarks>
        public static void InitialToggleTextures()
        {
            togglesTextureCache.Clear();
            string iconPath = XGUI_Dashboard.get_path_xgui_shapes() + "Toggle/";
            List<Texture2D> texs = XGUI.LoadAllAssetsAtPathWithPattern<Texture2D>(iconPath, ".png");

            foreach (Texture2D tex in texs)
            {
                string normalKey = tex.name;
                if (tex != null)
                    togglesTextureCache[normalKey] = tex;
            }
        }
        #endregion

        #region Preget Curves
        /// <summary>
        /// 缓存已生成的曲线，键为曲线名称
        /// </summary>
        private static Dictionary<EaseType, AnimationCurve> CurveCache = new Dictionary<EaseType, AnimationCurve>();

        /// <summary>
        /// 初始化并缓存 Curve 样式。
        /// </summary>
        public static void InitialCurves()
        {
            CurveCache.Clear();

            EaseType[] easeTypes = EaseTypes_Get();
            for (int i = 0; i < easeTypes.Length; i++)
            {
                // 闭包捕获
                EaseType type = easeTypes[i];

                AnimationCurve curve = EaseType_Generate(type, 20);

                CurveCache[type] = curve;
            }
        }
        #endregion

        #region Gets
        /// <summary>
        /// 获取样式
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static GUIStyle GetStyle(string name)
        {
            if (GUICreator == null)
            {
                Gui_Layout_Initia();
            }

            return new GUIStyle(GUICreator.GetStyle(name));
        }
        /// <summary>
        /// 获取字体
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Font GetFont(string name)
        {
            if (GUICreator == null)
            {
                Gui_Layout_Initia();
            }

            // 确保字体列表已初始化
            if (fontlist == null)
            {
                InitialFontsGet();
            }

            // 如果初始化后仍然为空，直接返回
            if (fontlist == null || fontlist.Length == 0 || string.IsNullOrEmpty(name))
                return null;

            for (int i = 0; i < fontlist.Length; i++)
            {
                if (fontlist[i].name != name)
                    continue;
                return fontlist[i];
            }

            XGUI_Utilitys.Console("XGUI - 通知", $"未找到名称为  {name}  的字体文件！请检查 XGUI 的字体文件夹是否存在目标名称字体文件！", XGUIMsgState.错误);
            return null;
        }
        /// <summary>
        /// 获取背景填充
        /// </summary>
        /// <param name="Color"></param>
        public static Texture2D GetFillTexture(XGUIFilled Mode, XGUIColor Color)
        {
            if (GUICreator == null)
            {
                Gui_Layout_Initia();
            }

            string key = $"{Mode}_{Color}";

            if (fillTextureCache.TryGetValue(key, out Texture2D tex))
                return tex;

            // 缓存未命中时实时加载（兜底）
            string shapesPath = XGUI_Dashboard.get_path_xgui_shapes() + "Group/";
            tex = AssetDatabase.LoadAssetAtPath<Texture2D>(shapesPath + $"Group_{Mode}_{Color}.png");

            // 将加载结果加入缓存（即使是 null 也缓存，避免重复加载）
            fillTextureCache[key] = tex;

            return tex;
        }
        /// <summary>
        /// 获取按钮背景填充
        /// </summary>
        /// <param name="Mode">类型</param>
        /// <param name="Color">颜色（具体请查看工程目录中文件的实际名称）</param>
        public static Texture2D GetBtnFillTexture(XGUIFilled Mode, XGUIColor Color)
        {
            if (GUICreator == null)
            {
                Gui_Layout_Initia();
            }

            string key = Mode == XGUIFilled.透明 ? "透明_无" : $"{Mode}_{Color}";

            if (btnFillTextureCache.TryGetValue(key, out Texture2D tex))
                return tex;

            // 缓存未命中时实时加载（兜底）
            string btnPath = XGUI_Dashboard.get_path_xgui_shapes() + "Button/";
            if (Mode == XGUIFilled.透明)
            {
                tex = AssetDatabase.LoadAssetAtPath<Texture2D>(btnPath + $"Btn_{Mode}.png");
            }
            else
            {
                tex = AssetDatabase.LoadAssetAtPath<Texture2D>(btnPath + $"Btn_{Mode}_{Color}.png");
            }

            // 将加载结果加入缓存
            btnFillTextureCache[key] = tex;

            return tex;
        }
        public static Texture2D GetToggleHandlerShadowTexture(XGUIToggleHandlerState state)
        {
            if (GUICreator == null)
            {
                Gui_Layout_Initia();
            }

            string v = (state == XGUIToggleHandlerState.开 ? "On" : "Off");

            string key = $"Toggle_Handler_{v}_Shadow";

            if (togglesTextureCache.TryGetValue(key, out Texture2D tex))
                return tex;

            // 缓存未命中时实时加载（兜底）
            string shapesPath = XGUI_Dashboard.get_path_xgui_shapes() + "Toggle/";
            tex = AssetDatabase.LoadAssetAtPath<Texture2D>(shapesPath + $"Toggle_Handler_{v}_Shadow.png");

            // 将加载结果加入缓存（即使是 null 也缓存，避免重复加载）
            togglesTextureCache[key] = tex;

            return tex;
        }
        /// <summary>
        /// 获取基础图标
        /// </summary>
        /// <param name="str">图标名称</param>
        public static Texture2D GetIcon(string str)
        {
            if (GUICreator == null)
            {
                Gui_Layout_Initia();
            }

            return AssetDatabase.LoadAssetAtPath<Texture2D>(XGUI_Dashboard.get_path_xgui_icons() + $"{str}.png");
        }
        /// <summary>
        /// 获取开关背景填充
        /// </summary>
        /// <param name="style"></param>
        public static Texture2D GetToggleBgTexture(XGUIToggleStyle style)
        {
            if (GUICreator == null)
            {
                Gui_Layout_Initia();
            }

            string key = $"Toggle_Bg_{style}";

            if (togglesTextureCache.TryGetValue(key, out Texture2D tex))
                return tex;

            // 缓存未命中时实时加载（兜底）
            string shapesPath = XGUI_Dashboard.get_path_xgui_shapes() + "Toggle/";
            tex = AssetDatabase.LoadAssetAtPath<Texture2D>(shapesPath + $"Toggle_Bg_{style}.png");

            // 将加载结果加入缓存（即使是 null 也缓存，避免重复加载）
            togglesTextureCache[key] = tex;

            return tex;
        }
        /// <summary>
        /// 获取开关控制柄
        /// </summary>
        /// <param name="state"></param>
        public static Texture2D GetToggleHandlerTexture()
        {
            if (GUICreator == null)
            {
                Gui_Layout_Initia();
            }

            string key = $"Toggle_Handler";

            if (togglesTextureCache.TryGetValue(key, out Texture2D tex))
                return tex;

            // 缓存未命中时实时加载（兜底）
            string shapesPath = XGUI_Dashboard.get_path_xgui_shapes() + "Toggle/";
            tex = AssetDatabase.LoadAssetAtPath<Texture2D>(shapesPath + $"Toggle_Handler.png");

            // 将加载结果加入缓存（即使是 null 也缓存，避免重复加载）
            togglesTextureCache[key] = tex;

            return tex;
        }
        /// <summary>
        /// 获取基础图标
        /// </summary>
        /// <param name="str">图标名称</param>
        public static Texture2D GetBasedIcon(string str)
        {
            if (GUICreator == null)
            {
                Gui_Layout_Initia();
            }

            if (basedTextureCache.TryGetValue(str, out Texture2D tex))
                return tex;

            tex = AssetDatabase.LoadAssetAtPath<Texture2D>(XGUI_Dashboard.get_path_xgui_icons() + $"Based/{str}.png");

            // 将加载结果加入缓存
            basedTextureCache[str] = tex;

            return tex;
        }
        /// <summary>
        /// 获取Helpbox图标
        /// </summary>
        /// <param name="state">图标类型</param>
        public static Texture2D GetHelpboxStateIcon(XGUIHelboxState state)
        {
            if (GUICreator == null)
            {
                Gui_Layout_Initia();
            }

            string name = null;

            switch (state)
            {
                case XGUIHelboxState.通知:
                    name = "icon_notice";
                    break;
                case XGUIHelboxState.警告:
                    name = "icon_warning";
                    break;
                case XGUIHelboxState.错误:
                    name = "icon_error";
                    break;
            }
            if (helpboxTextureCache.TryGetValue(name, out Texture2D tex))
                return tex;

            tex = AssetDatabase.LoadAssetAtPath<Texture2D>(XGUI_Dashboard.get_path_xgui_icons() + $"Helpbox/{name}.png");

            // 将加载结果加入缓存
            helpboxTextureCache[name] = tex;

            return tex;
        }
        /// <summary>
        /// 获取自定义图标
        /// </summary>
        /// <param name="str">图标路径</param>
        public static Texture2D GetCustomIcon(string str)
        {
            if (GUICreator == null)
            {
                Gui_Layout_Initia();
            }

            return AssetDatabase.LoadAssetAtPath<Texture2D>($"{str}.png");
        }
        /// <summary>
        /// 获取内建图标
        /// </summary>
        /// <param name="IconName">图标名称</param>
        /// <returns></returns>
        public static Texture2D GetBuiltInIcon(string IconName)
        {
            if (GUICreator == null)
            {
                Gui_Layout_Initia();
            }

            return EditorGUIUtility.IconContent(IconName).image as Texture2D;
        }
        public static AnimationCurve GetEaseCurve(EaseType type)
        {
            if (GUICreator == null)
            {
                Gui_Layout_Initia();
            }

            if (CurveCache.TryGetValue(type, out AnimationCurve cur))
                return cur;

            cur = EaseType_Generate(type, 20);

            // 将加载结果加入缓存
            CurveCache[type] = cur;

            return cur;
        }
        /// <summary>
        /// 获取规范化颜色
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color GetXGUIColor(XGUIColor color)
        {
            if (GUICreator == null)
            {
                Gui_Layout_Initia();
            }

            Color col = new Color();
            switch (color)
            {
                case XGUIColor.深空灰:
                    ColorUtility.TryParseHtmlString("#232323", out col);
                    break;
                case XGUIColor.阴影灰:
                    ColorUtility.TryParseHtmlString("#909090", out col);
                    break;
                case XGUIColor.亮白:
                    ColorUtility.TryParseHtmlString("#e8e8e8", out col);
                    break;
                case XGUIColor.柠檬绿:
                    ColorUtility.TryParseHtmlString("#c9e63f", out col);
                    break;
                case XGUIColor.工业蓝:
                    ColorUtility.TryParseHtmlString("#38aafb", out col);
                    break;
                case XGUIColor.警示黄:
                    ColorUtility.TryParseHtmlString("#ffc230", out col);
                    break;
                case XGUIColor.玫瑰粉:
                    ColorUtility.TryParseHtmlString("#fa5d98", out col);
                    break;
                case XGUIColor.神秘紫:
                    ColorUtility.TryParseHtmlString("#9e5afb", out col);
                    break;
                case XGUIColor.魅力红:
                    ColorUtility.TryParseHtmlString("#ff3737", out col);
                    break;
                case XGUIColor.灰绿:
                    ColorUtility.TryParseHtmlString("#94a565", out col);
                    break;
                case XGUIColor.亮橘红:
                    ColorUtility.TryParseHtmlString("#ff872e", out col);
                    break;
                case XGUIColor.枪灰:
                    ColorUtility.TryParseHtmlString("#747474", out col);
                    break;
                case XGUIColor.亮金色:
                    ColorUtility.TryParseHtmlString("#ffd86b", out col);
                    break;
                case XGUIColor.沉暗红:
                    ColorUtility.TryParseHtmlString("#a32a2a", out col);
                    break;
                case XGUIColor.烟灰蓝:
                    ColorUtility.TryParseHtmlString("#76829a", out col);
                    break;
                case XGUIColor.健康绿:
                    ColorUtility.TryParseHtmlString("#4aeec9", out col);
                    break;
                case XGUIColor.浅灰:
                    ColorUtility.TryParseHtmlString("#414141", out col);
                    break;
                case XGUIColor.无:
                    break;
            }
            return col;
        }
        /// <summary>
        /// 获取上一个控件的矩形，只读，不参与布局
        /// 获取已绘制控件的位置
        /// </summary>
        /// <returns></returns>
        public static Rect GetLastRect()
        {
            return GUILayoutUtility.GetLastRect();
        }
        /// <summary>
        /// 请求一个标准控件矩形，主动请求参与布局
        /// 绘制自定义编辑器控件
        /// </summary>
        /// <returns></returns>
        public static Rect GetControlRect(bool haslabel, float height)
        {
            return EditorGUILayout.GetControlRect(haslabel, height);
        }
        /// <summary>
        /// 灵活请求自定义矩形，主动请求参与布局
        /// 完全控制尺寸的控件
        /// </summary>
        /// <returns></returns>
        public static Rect GetRect(GUIContent content, GUIStyle style)
        {
            return GUILayoutUtility.GetRect(content, style);
        }
        /// <summary>
        /// 获取当前窗口的宽度
        /// </summary>
        /// <returns></returns>
        public static float GetCurrentWindowWidth()
        {
            return EditorGUIUtility.currentViewWidth;
        }
        /// <summary>
        /// 获取内置默认行高度
        /// </summary>
        /// <returns></returns>
        public static float GetSingleLineHeight()
        {
            return EditorGUIUtility.singleLineHeight;
        }
        /// <summary>
        /// 获取目标文件夹下的指定类型所有资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="patterns"></param>
        /// <returns></returns>
        public static List<T> LoadAllAssetsAtPathWithPattern<T>(string path, params string[] patterns) where T : UnityEngine.Object
        {
            List<T> _out = new();

            string root_path = Directory.GetParent(Application.dataPath) + "/" + path;

            if (!Directory.Exists(root_path))
            {
                Debug.LogWarning("Path doesn't exist");
                return _out;
            }

            // 如果没有指定任何pattern，获取所有文件
            if (patterns == null || patterns.Length == 0)
            {
                patterns = new string[] { "*" };
            }

            foreach (string pattern in patterns)
            {
                string searchPattern = string.IsNullOrEmpty(pattern) ? "*" : (pattern.Contains("*") ? pattern : $"*{pattern}");
                string[] fileEntries = Directory.GetFiles(root_path, searchPattern);

                foreach (string FileName in fileEntries)
                {
                    string[] filepath = FileName.Split(Directory.GetParent(Application.dataPath).FullName + "/");
                    T asset = AssetDatabase.LoadAssetAtPath<T>(filepath[1]);
                    if (asset != null && !_out.Contains(asset)) // 避免重复添加
                    {
                        _out.Add(asset);
                    }
                }
            }

            return _out;
        }
        #endregion

        #region Common
        /// <summary>
        /// 控制GUI可用性开关
        /// </summary>
        /// <param name="state"></param>
        public static void SetEnabled(bool state)
        {
            GUI.enabled = state;
        }
        /// <summary>
        /// 获取字号
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static int GetFontSize(XGUIFontSize size)
        {
            int _size = 12;
            switch (size)
            {
                // ------------------------------最小
                case XGUIFontSize.XS:
                    _size = 8;
                    break;
                // ------------------------------小
                case XGUIFontSize.S:
                    _size = 10;
                    break;
                // ------------------------------中
                case XGUIFontSize.M:
                    _size = 11;
                    break;
                // ------------------------------标准
                case XGUIFontSize.B:
                    _size = 12;
                    break;
                // ------------------------------偏大
                case XGUIFontSize.BX:
                    _size = 13;
                    break;
                // ------------------------------大
                case XGUIFontSize.L:
                    _size = 14;
                    break;
                // ------------------------------最大
                case XGUIFontSize.XL:
                    _size = 18;
                    break;
                // ------------------------------极大
                case XGUIFontSize.XXL:
                    _size = 20;
                    break;
                // ------------------------------超大
                case XGUIFontSize.XXXL:
                    _size = 24;
                    break;
            }

            return _size;
        }
        /// <summary>
        /// 判断当前编辑器窗口宽度是否满足指定的阈值条件。
        /// </summary>
        /// <param name="mode">比较模式，支持 ">"、">="、"<"、"<="、"=="。</param>
        /// <param name="value">用于比较的阈值。</param>
        /// <returns>如果当前窗口宽度满足指定条件则返回 true，否则返回 false。</returns>
        /// <exception cref="System.ArgumentException">当 mode 参数不在支持的模式列表中时抛出。</exception>
        /// <example>
        /// <code>
        /// if (XGUI.CurrentWindowWidthThreshold(">", 800f))
        /// {
        ///     // 窗口宽度大于 800 像素时的处理逻辑
        /// }
        /// </code>
        /// </example>
        public static bool CurrentWindowWidthThreshold(string mode, float value)
        {
            bool state = false;
            switch (mode)
            {
                case ">":
                    if (GetCurrentWindowWidth() > value)
                        state = true;
                    else
                        state = false;
                    break;
                case ">=":
                    if (GetCurrentWindowWidth() >= value)
                        state = true;
                    else
                        state = false;
                    break;
                case "<":
                    if (GetCurrentWindowWidth() < value)
                        state = true;
                    else
                        state = false;
                    break;
                case "<=":
                    if (GetCurrentWindowWidth() <= value)
                        state = true;
                    else
                        state = false;
                    break;
                case "==":
                    if (GetCurrentWindowWidth() == value)
                        state = true;
                    else
                        state = false;
                    break;
            }

            return state;
        }
        /// <summary>
        /// 计算指定文本在给定样式和字体大小下所需的矩形尺寸。
        /// </summary>
        /// <param name="style">要应用的目标 GUIStyle。</param>
        /// <param name="fontsize">要使用的字体大小（来自 <see cref="XGUIFontSize"/> 枚举）。</param>
        /// <param name="text">需要计算尺寸的文本内容。</param>
        /// <returns>一个 <see cref="Vector2"/>，包含文本渲染所需的宽度和高度。</returns>
        /// <remarks>
        /// 该方法会复制传入的样式并覆盖其 fontSize 属性，然后调用 <see cref="GUIStyle.CalcSize(GUIContent)"/>
        /// 计算指定文本的渲染尺寸，适用于需要精确控制控件大小和对齐的场景。
        /// </remarks>
        /// <example>
        /// <code>
        /// Vector2 size = XGUI.CalculateRectSize(XGUI.style_xg_label, XGUIFontSize.M, "示例文本");
        /// GUILayout.Label("示例文本", GUILayout.Width(size.x), GUILayout.Height(size.y));
        /// </code>
        /// </example>
        public static Vector2 CalculateRectSize(GUIStyle style, XGUIFontSize fontsize, string text)
        {
            GUIStyle s = new GUIStyle(style);
            s.fontSize = XGUI.GetFontSize(fontsize);
            return s.CalcSize(new GUIContent(text));
        }
        /// <summary>
        /// 获取当前 Unity 版本适用的文本裁剪模式。
        /// 在 Unity 6000.0 或更高版本中使用省略号（Ellipsis），
        /// 在较低版本中使用裁剪（Clip）。
        /// </summary>
        /// <returns>适用于当前 Unity 版本的 <see cref="TextClipping"/> 枚举值。</returns>
        public static TextClipping TryEllipsisClipping()
        {
#if UNITY_6000_0_OR_NEWER
            TextClipping clipping = TextClipping.Ellipsis;
#else
    TextClipping clipping = TextClipping.Clip;
#endif

            return clipping;
        }
        #endregion

        #region  EditorGUI ChangeCheck
        public static void ChangedCheck_Start()
        {
            EditorGUI.BeginChangeCheck();
        }

        public static bool ChangedCheck_End()
        {
            return EditorGUI.EndChangeCheck();
        }
        #endregion

        #region Focus
        /// <summary>
        /// 设置控件焦点
        /// </summary>
        /// <param name="name"></param>
        public static void SetFocus(string name)
        {
            GUI.FocusControl(null);
        }
        /// <summary>
        /// 清空控件焦点
        /// </summary>
        public static void RemoveFocus()
        {
            GUI.FocusControl(null);
        }
        #endregion

        #region PathDialog
        /// <summary>
        /// 打开系统对话框选择路径。
        /// </summary>
        /// <param name="currentPath">当前路径（用于设置初始目录）。</param>
        /// <param name="pathType">路径类型。</param>
        /// <param name="filter">文件过滤器。</param>
        /// <param name="filterTitle">过滤器显示名称。</param>
        /// <param name="defaultName">默认文件名（保存时使用）。</param>
        /// <returns>用户选择的路径，若取消则返回 <c>null</c> 或空字符串。</returns>
        public static string BrowsePath(string currentPath, XGUIPathType pathType, string filter, string filterTitle, string defaultName)
        {
            // 确定初始目录
            string initialDirectory = string.IsNullOrEmpty(currentPath)
                ? Application.dataPath
                : System.IO.Path.GetDirectoryName(currentPath);

            // 如果当前路径是文件，使用其目录
            if (!string.IsNullOrEmpty(currentPath) && System.IO.File.Exists(currentPath))
            {
                initialDirectory = System.IO.Path.GetDirectoryName(currentPath);
            }
            else if (!string.IsNullOrEmpty(currentPath) && System.IO.Directory.Exists(currentPath))
            {
                initialDirectory = currentPath;
            }

            // 确保目录存在
            if (!System.IO.Directory.Exists(initialDirectory))
            {
                initialDirectory = Application.dataPath;
            }

            switch (pathType)
            {
                case XGUIPathType.文件:
                    // ✅ 使用 BuildFileFilter 构建完整过滤器
                    return EditorUtility.OpenFilePanel("选择文件", initialDirectory, filter);

                case XGUIPathType.文件夹:
                    return EditorUtility.OpenFolderPanel("选择文件夹", initialDirectory, "");

                case XGUIPathType.文件或文件夹:
                    return EditorUtility.OpenFolderPanel("选择文件或文件夹", initialDirectory, "");

                case XGUIPathType.保存文件:
                    return EditorUtility.SaveFilePanel("保存文件", initialDirectory, defaultName, filter);

                default:
                    return null;
            }
        }
        #endregion

        #region EditorWindowCenter
        /// <summary>
        /// 窗口居中
        /// </summary>
        /// <param name="size"></param>
        /// <param name="window"></param>
        public static void CenterEditorWindow(Vector2Int size, EditorWindow window, bool fixedsize = true)
        {
            window.minSize = size;
            if (fixedsize)
                window.maxSize = window.minSize;

            // 获取当前屏幕的分辨率
            int screenWidth = Screen.currentResolution.width;
            int screenHeight = Screen.currentResolution.height;

            // 计算窗口位置（屏幕中心）
            Rect windowRect = new Rect((screenWidth - size.x) / 2.0f, (screenHeight - size.y) / 2.0f, size.x, size.y);

            // 更新窗口位置和大小
            window.position = windowRect;
        }
        #endregion

        #region EditorData
        /// <summary>
        /// 数据是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool x_Editor_Data_Has_String(string key)
        {
            if (EditorPrefs.HasKey(key))
                return true;
            else return false;
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static float x_Editor_Data_Get_With_Float(string key)
        {
            if (EditorPrefs.HasKey(key))
                return EditorPrefs.GetFloat(key);
            else return 0;
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string x_Editor_Data_Get_With_String(string key)
        {
            if (EditorPrefs.HasKey(key))
                return EditorPrefs.GetString(key);
            else return null;
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int x_Editor_Data_Get_With_Int(string key)
        {
            if (EditorPrefs.HasKey(key))
                return EditorPrefs.GetInt(key);
            else return 0;
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool x_Editor_Data_Get_With_Bool(string key)
        {
            if (EditorPrefs.HasKey(key))
                return EditorPrefs.GetBool(key);
            else return false;
        }
        /// <summary>
        /// 存入数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public static void x_Editor_Data_Set_With_Float(string key, float data)
        {
            EditorPrefs.SetFloat(key, data);
        }
        /// <summary>
        /// 存入数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public static void x_Editor_Data_Set_With_String(string key, string data)
        {
            EditorPrefs.SetString(key, data);
        }
        /// <summary>
        /// 存入数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public static void x_Editor_Data_Set_With_Int(string key, int data)
        {
            EditorPrefs.SetInt(key, data);
        }
        /// <summary>
        /// 存入数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public static void x_Editor_Data_Set_With_Bool(string key, bool data)
        {
            EditorPrefs.SetBool(key, data);
        }
        /// <summary>
        /// 清空数据
        /// </summary>
        /// <param name="key"></param>
        public static void x_Editor_Data_Clear(string key)
        {
            if (EditorPrefs.HasKey(key))
                EditorPrefs.DeleteKey(key);
        }
        #endregion
    }
}