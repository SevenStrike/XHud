/*
 * ============================================================================
 * ⚠ 版权声明（禁止删除、禁止修改、衍生作品必须保留此注释）⚠
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
namespace SevenStrikeModules.XHud.Editor
{
    using SevenStrikeModules.XGUI.Editor;
    using SevenStrikeModules.XGUI.Runtime;
    using SevenStrikeModules.XHud.Enums;
    using System;
    using System.Collections.Generic;
    using TMPro;
    using TMPro.EditorUtilities;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;
    using Application = UnityEngine.Application;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(XHud_Module_TmpText))]
    public class Editor_XHud_Module_TmpText : TMP_BaseEditorPanel
    {
        #region 字段 - 组件
        /// <summary>
        /// 目标脚本实例。
        /// <para/>
        /// 在 <see cref="OnEnable"/> 中缓存。
        /// </summary>
        private XHud_Module_TmpText BaseScript;
        /// <summary>
        /// XHud 管理器实例。
        /// <para/>
        /// 用于访问文字样式库。
        /// </summary>
        private XHud_Manager HudManager;
        #endregion

        #region 字段 - 折叠状态
        /// <summary>
        /// 原始脚本参数显示开关。
        /// </summary>
        private bool OriginalDisplay;
        #endregion

        #region 字段 - 序列化属性
        /// <summary>
        /// 标识名称。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_Indicator;
        /// <summary>
        /// 样式名称。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_StyleName;
        /// <summary>
        /// 尺寸全局联动开关。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_SyncGlobalFontSize;
        /// <summary>
        /// 实时匹配库开关。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_StyleLibSynching;
        /// <summary>
        /// 文字样式信息数据。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_TextStyleInfo;
        /// <summary>
        /// 文字内容。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_Text, sp_TextColorOwner;
        /// <summary>
        /// 富文本支持。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_tmp_rich;
        /// <summary>
        /// 包裹模式。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_tmp_contentwrap;
        /// <summary>
        /// 溢出模式。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_tmp_overflow;
        /// <summary>
        /// 样式。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_tmp_style;
        /// <summary>
        /// 内容锚点。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_tmp_anchor;
        /// <summary>
        /// 字体资源。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_tmp_font;
        /// <summary>
        /// 尺寸。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_tmp_size;
        /// <summary>
        /// 字符间距。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_tmp_space_character;
        /// <summary>
        /// 单词间距。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_tmp_space_word;
        /// <summary>
        /// 行高间距。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_tmp_space_lineheight;
        /// <summary>
        /// 段落间距。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_tmp_space_paragraph;
        /// <summary>
        /// 包裹比例。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_tmp_WrappingRatios;
        /// <summary>
        /// 内容边距。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_tmp_contentmargin;
        /// <summary>
        /// 字体颜色。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_tmp_color;
        /// <summary>
        /// 自适应尺寸。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_tmp_EnableAutoSizing;
        /// <summary>
        /// 自适应最小尺寸。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_tmp_FontSizeMin;
        /// <summary>
        /// 自适应最大尺寸。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_tmp_FontSizeMax;
        /// <summary>
        /// 最大字符宽度。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_tmp_CharWidthMaxAdj;
        /// <summary>
        /// 最大行高。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_tmp_LineSpacingMax;
        /// <summary>
        /// 渐变色 - 主色。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_gra_A;
        /// <summary>
        /// 渐变色 - 副色。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_gra_B;
        /// <summary>
        /// 渐变色 - 次色。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_gra_C;
        /// <summary>
        /// 渐变色 - 尾色。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_gra_D;
        /// <summary>
        /// 渐变色 - 翻转。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_gra_Invert;
        /// <summary>
        /// 渐变色 - 是否启用。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_gra_Used;
        /// <summary>
        /// 渐变色 - 模式。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_gra_ColorMode;
        /// <summary>
        /// 渐变色 - 模式名称。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_gra_ColorModeName;
        /// <summary>
        /// 渐变色 - 模式名称索引。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_gra_ColorModeNameIndex;
        /// <summary>
        /// 库同步 - 对齐。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_LibStyle_Effect_align;
        /// <summary>
        /// 库同步 - 字体。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_LibStyle_Effect_font;
        /// <summary>
        /// 库同步 - 样式。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_LibStyle_Effect_style;
        /// <summary>
        /// 库同步 - 尺寸。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_LibStyle_Effect_size;
        /// <summary>
        /// 库同步 - 颜色。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_LibStyle_Effect_color;
        /// <summary>
        /// 库同步 - 富文本。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_LibStyle_Effect_rich;
        /// <summary>
        /// 库同步 - 包裹。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_LibStyle_Effect_wrap;
        /// <summary>
        /// 库同步 - 包裹比例。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_LibStyle_Effect_wrapratio;
        /// <summary>
        /// 库同步 - 溢出。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_LibStyle_Effect_overflow;
        /// <summary>
        /// 库同步 - 边距。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_LibStyle_Effect_margin;
        /// <summary>
        /// 库同步 - 间距。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_LibStyle_Effect_space;
        /// <summary>
        /// 库同步 - 渐变色。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_LibStyle_Effect_gradientcolor;
        /// <summary>
        /// 库同步 - 自适应尺寸。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_LibStyle_Effect_autosizing;
        /// <summary>
        /// 库同步 - 射线检测。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_LibStyle_Effect_raycast;
        /// <summary>
        /// 库同步 - 遮罩。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_LibStyle_Effect_maskable;
        /// <summary>
        /// 射线检测。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_Raycast;
        /// <summary>
        /// 遮罩。
        /// </summary>
        [SerializeField]
        private SerializedProperty sp_Maskable;
        #endregion

        #region 字段 - 图标
        /// <summary>
        /// 定位按钮释放图标。
        /// </summary>
        private Texture2D locate_r;
        /// <summary>
        /// 定位按钮按下图标。
        /// </summary>
        private Texture2D locate_p;
        /// <summary>
        /// 窗口主图标。
        /// </summary>
        private Texture2D icon_main;
        /// <summary>
        /// 保存按钮释放图标。
        /// </summary>
        private Texture2D save_r;
        /// <summary>
        /// 保存按钮按下图标。
        /// </summary>
        private Texture2D save_p;
        /// <summary>
        /// 打开库按钮释放图标。
        /// </summary>
        private Texture2D openlib_r;
        /// <summary>
        /// 打开库按钮按下图标。
        /// </summary>
        private Texture2D openlib_p;
        /// <summary>
        /// 刷新按钮释放图标。
        /// </summary>
        private Texture2D update_r;
        /// <summary>
        /// 刷新按钮按下图标。
        /// </summary>
        private Texture2D update_p;
        /// <summary>
        /// 颜色接管者选择器参数。
        /// </summary>
        [NonSerialized]
        private clickselector_args[] clickselector_args = new clickselector_args[4];
        #endregion

        #region 字段 - 选项文字
        /// <summary>
        /// 开关选项文字（关闭 / 开启）。
        /// </summary>
        private string[] stroptions_enabled = new string[2] { "关闭", "开启" };
        /// <summary>
        /// 渐变色模式选项文字。
        /// </summary>
        private string[] stroptions_gramode = new string[] { "单色", "水平渐变", "垂直渐变", "四角渐变" };
        /// <summary>
        /// 尺寸同步选项文字（原生 / 增量）。
        /// </summary>
        private string[] stroptions_syncsize = new string[] { "原生", "增量" };
        /// <summary>
        /// 颜色同步选项文字（原生 / 接管）。
        /// </summary>
        private string[] stroptions_synccolor = new string[] { "原生", "接管" };
        /// <summary>
        /// 翻转选项文字（常规 / 翻转）。
        /// </summary>
        private string[] stroptions_flip = new string[] { "常规", "翻转" };
        /// <summary>
        /// 样式同步选项文字（原生 / 同步）。
        /// </summary>
        private string[] stroptions_syncstyle = new string[] { "原生", "同步" };
        /// <summary>
        /// 颜色接管者名称。
        /// </summary>
        private string[] tooltip_owner_names = new string[4] { "文字库", "配色器", "动画器", "自身 / 手动" };
        #endregion

        #region 字段 - 渐变色预览
        /// <summary>
        /// 渐变色预览纹理。
        /// </summary>
        private Texture2D GradientTexture;
        /// <summary>
        /// 渐变色预览纹理宽度。
        /// </summary>
        private int gra_TexWidth = 40;
        /// <summary>
        /// 渐变色预览纹理高度。
        /// </summary>
        private int gra_TexHeight = 40;
        #endregion

        #region 工具 - 批量化操作
        /// <summary>
        /// 当前 Inspector 选中的所有目标组件缓存。
        /// </summary>
        private XHud_Module_TmpText[] SelectedObjects;
        /// <summary>
        /// 缓存当前 Inspector 选中的所有目标组件到 <see cref="SelectedObjects"/>。
        /// <para/>
        /// 多选时逐个转换，单选时只填充一个元素。
        /// </summary>
        private void Targets_Get()
        {
            if (targets.Length > 1)
            {
                SelectedObjects = new XHud_Module_TmpText[targets.Length];
                for (int i = 0; i < SelectedObjects.Length; i++)
                {
                    var t = targets[i];
                    SelectedObjects[i] = (XHud_Module_TmpText)t;
                }
            }
            else
            {
                SelectedObjects = new XHud_Module_TmpText[targets.Length];
                SelectedObjects[0] = (XHud_Module_TmpText)target;
            }
        }
        /// <summary>
        /// 判断当前是否处于多选状态。
        /// </summary>
        /// <returns>true 表示选中了多个组件</returns>
        private bool Targets_Selected()
        {
            if (SelectedObjects == null)
                return false;
            if (SelectedObjects.Length > 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 生命周期
        /// <summary>
        /// 禁用预览窗口
        /// </summary>
        /// <returns></returns>
        public override bool HasPreviewGUI()
        {
            // 返回 false 即隐藏预览窗口
            return true;
        }
        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            string h_color = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.themes_get_color(ThemeColorType.primary));

            XGUI.gui_icon(
                rect: new Rect(r.x + (r.width / 2) - 30, r.y + (r.height / 2) - 40, 60, 60),
                icon: clickselector_args[sp_TextColorOwner.enumValueIndex].btn_icon_r);

            XGUI.gui_label(
                rect: new Rect(r.x + (r.width / 2) - (r.width / 2), r.y + (r.height / 2) + 15, r.width, XGUI.GetSingleLineHeight()),
                text: new GUIContent($"文字颜色接管者为：<color=#{h_color}> {tooltip_owner_names[sp_TextColorOwner.enumValueIndex]}</color>"),
                text_color: Color.white,
                size: XGUIFontSize.M,
                clipping: TextClipping.Clip,
                anchor: TextAnchor.MiddleCenter,
                offset: new Vector2(0, 0),
                font_style: FontStyle.Normal);
        }
        /// <summary>
        /// Unity 启用回调。
        /// <para/>
        /// 执行流程：
        /// <list type="number">
        /// <item><description>调用基类 OnEnable；</description></item>
        /// <item><description>获取 XHud 管理器；</description></item>
        /// <item><description>缓存目标脚本；</description></item>
        /// <item><description>绑定全部序列化属性；</description></item>
        /// <item><description>加载全部图标资源与颜色接管者选择器；</description></item>
        /// <item><description>缓存所有选中组件；</description></item>
        /// <item><description>初始化默认样式名称。</description></item>
        /// </list>
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            HudManager = XHud_Dashboard.get_manager();

            BaseScript = (XHud_Module_TmpText)target;

            #region 获取序列化变量
            sp_RaycastTargetProp = serializedObject.FindProperty("m_RaycastTarget");
            sp_MaskableProp = serializedObject.FindProperty("m_Maskable");

            sp_Indicator = serializedObject.FindProperty("Indicator");
            sp_StyleName = serializedObject.FindProperty("StyleName");
            sp_StyleLibSynching = serializedObject.FindProperty("StyleLibSynching");
            sp_SyncGlobalFontSize = serializedObject.FindProperty("SyncGlobalFontSize");
            sp_TextStyleInfo = serializedObject.FindProperty("TextStyleInfo");
            sp_Text = serializedObject.FindProperty("m_text");
            sp_TextColorOwner = serializedObject.FindProperty("TextColorOwner");

            sp_tmp_rich = sp_TextStyleInfo.FindPropertyRelative("tmp_rich");
            sp_tmp_contentwrap = sp_TextStyleInfo.FindPropertyRelative("tmp_contentwrap");
            sp_tmp_overflow = sp_TextStyleInfo.FindPropertyRelative("tmp_overflow");
            sp_tmp_style = sp_TextStyleInfo.FindPropertyRelative("tmp_style");
            sp_tmp_anchor = sp_TextStyleInfo.FindPropertyRelative("tmp_anchor");
            sp_tmp_font = sp_TextStyleInfo.FindPropertyRelative("tmp_font");
            sp_tmp_size = sp_TextStyleInfo.FindPropertyRelative("tmp_size");
            sp_tmp_space_character = sp_TextStyleInfo.FindPropertyRelative("tmp_space_character");
            sp_tmp_space_word = sp_TextStyleInfo.FindPropertyRelative("tmp_space_word");
            sp_tmp_space_lineheight = sp_TextStyleInfo.FindPropertyRelative("tmp_space_lineheight");
            sp_tmp_space_paragraph = sp_TextStyleInfo.FindPropertyRelative("tmp_space_paragraph");
            sp_tmp_WrappingRatios = sp_TextStyleInfo.FindPropertyRelative("tmp_WrappingRatios");
            sp_tmp_contentmargin = sp_TextStyleInfo.FindPropertyRelative("tmp_contentmargin");
            sp_tmp_color = sp_TextStyleInfo.FindPropertyRelative("tmp_color");
            sp_tmp_EnableAutoSizing = sp_TextStyleInfo.FindPropertyRelative("tmp_EnableAutoSizing");
            sp_tmp_FontSizeMin = sp_TextStyleInfo.FindPropertyRelative("tmp_FontSizeMin");
            sp_tmp_FontSizeMax = sp_TextStyleInfo.FindPropertyRelative("tmp_FontSizeMax");
            sp_tmp_CharWidthMaxAdj = sp_TextStyleInfo.FindPropertyRelative("tmp_CharWidthMaxAdj");
            sp_tmp_LineSpacingMax = sp_TextStyleInfo.FindPropertyRelative("tmp_LineSpacingMax");
            //---------------------------------Tmp Text Gradient ---------------------------------//
            sp_gra_A = sp_TextStyleInfo.FindPropertyRelative("gra_A");
            sp_gra_B = sp_TextStyleInfo.FindPropertyRelative("gra_B");
            sp_gra_C = sp_TextStyleInfo.FindPropertyRelative("gra_C");
            sp_gra_D = sp_TextStyleInfo.FindPropertyRelative("gra_D");
            sp_gra_Invert = sp_TextStyleInfo.FindPropertyRelative("gra_Invert");
            sp_gra_Used = sp_TextStyleInfo.FindPropertyRelative("gra_Used");
            sp_gra_ColorMode = sp_TextStyleInfo.FindPropertyRelative("gra_ColorMode");
            sp_gra_ColorModeName = sp_TextStyleInfo.FindPropertyRelative("gra_ColorModeName");
            sp_gra_ColorModeNameIndex = sp_TextStyleInfo.FindPropertyRelative("gra_ColorModeNameIndex");

            //---------------------------------LibraryToggle---------------------------------//
            sp_LibStyle_Effect_align = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_align");
            sp_LibStyle_Effect_font = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_font");
            sp_LibStyle_Effect_style = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_style");
            sp_LibStyle_Effect_size = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_size");
            sp_LibStyle_Effect_color = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_color");
            sp_LibStyle_Effect_rich = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_rich");
            sp_LibStyle_Effect_wrap = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_wrap");
            sp_LibStyle_Effect_wrapratio = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_wrapratio");
            sp_LibStyle_Effect_overflow = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_overflow");
            sp_LibStyle_Effect_margin = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_margin");
            sp_LibStyle_Effect_space = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_space");
            sp_LibStyle_Effect_gradientcolor = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_gradientcolor");
            sp_LibStyle_Effect_autosizing = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_autosizing");
            sp_LibStyle_Effect_raycast = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_raycast");
            sp_LibStyle_Effect_maskable = sp_TextStyleInfo.FindPropertyRelative("LibStyle_Effect_maskable");
            //--------------------------------- Features ---------------------------------//
            sp_Raycast = sp_TextStyleInfo.FindPropertyRelative("Raycast");
            sp_Maskable = sp_TextStyleInfo.FindPropertyRelative("Maskable");
            #endregion

            #region 获取图标
            locate_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_tmptext/locate_r");
            locate_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_tmptext/locate_p");
            icon_main = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_tmptext/icon_main");
            save_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_tmptext/save_r");
            save_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_tmptext/save_p");
            openlib_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_tmptext/openlib_r");
            openlib_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_tmptext/openlib_p");
            update_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_tmptext/update_r");
            update_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_tmptext/update_p");

            for (int i = 0; i < clickselector_args.Length; i++)
            {
                clickselector_args[i] = new clickselector_args();
                clickselector_args[i].btn_icon_r = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_text/icon_color_owner_r_{i}");
                clickselector_args[i].btn_icon_p = XGUI.GetCustomIcon($"{XHud_Dashboard.Get_Path_XHUD_GUIROOT_Path()}gui_module_text/icon_color_owner_p_{i}");
                clickselector_args[i].btn_tooltip = tooltip_owner_names[i];
            }
            #endregion

            Targets_Get();

            if (HudManager != null)
            {
                if (HudManager.Hud_TextStyleLibrary != null)
                {
                    if (string.IsNullOrEmpty(sp_StyleName.stringValue))
                    {
                        if (!HudManager.Hud_TextStyleLibrary.TextStyle_Library_IsEmpty())
                        {
                            sp_StyleName.stringValue = HudManager.Hud_TextStyleLibrary.TextStyle_Library_GetFirstStyleInfo_With_Type(xHud_TextType.TmpText).Name;
                            sp_StyleName.serializedObject.ApplyModifiedProperties();
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Unity 禁用回调。
        /// <para/>
        /// 直接调用基类实现。
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
        }
        /// <summary>
        /// 绘制 Inspector 主入口。
        /// <para/>
        /// 按自上而下的顺序绘制所有面板：
        /// 标题 → 快捷功能 → 颜色接管者 → 选项 → 文字样式库 → 库同步项 →
        /// 渐变色控件 → 文字内容 → 参数 → 右键菜单 → 源脚本。
        /// </summary>
        public override void OnInspectorGUI()
        {
            string h_color = XGUI_Utilitys.Color_To_HexString(XHud_Dashboard.themes_get_color(ThemeColorType.primary));

            serializedObject.Update();

            #region 标题
            XGUI.layout_banner(
                bg_fill: XGUIGroup.实体,
                bg_color: XGUIColor.深空灰,
                bg_height: 30,
                icon: icon_main,
                icon_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_text: "XHud  -  文字",
                title_anchor: TextAnchor.MiddleLeft,
                title_style: FontStyle.Normal,
                title_color: Color.white,
                title_size: XGUIFontSize.B,
                title_clipping: XGUI.TryEllipsisClipping(),
                bg_margin: new RectOffset(0, 0, 5, 5));
            #endregion

            #region 快捷功能
            XGUI.layout_group_start(
              type: XGUIContainerType.Vertical,
              bg_fill: XGUIGroup.缺口纯色边框,
              bg_color: XGUIColor.亮白,
              bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
              title: "快捷功能",
              title_size: XGUIFontSize.M,
              title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
              title_clipping: TextClipping.Clip,
              padding: new RectOffset(15, 15, 20, 15));

            #region 编组
            XGUI.layout_group_start(
               type: XGUIContainerType.Horizontal,
               absolute_margin: true,
               absolute_padding: true,
               margin: new RectOffset(0, 0, 0, 0),
               padding: new RectOffset(0, 0, 0, 0));

            if (!Targets_Selected())
            {
                if (HudManager.Hud_TextStyleLibrary)
                {
                    #region 将当前样式保存到字体库
                    if (XGUI.layout_button(
                    tooltip: "将当前样式保存到字体库",
                    tex_release: save_r,
                    tex_press: save_p,
                    tex_gui_color: Color.white,
                    border: new RectOffset(0, 0, 0, 0),
                    width: 14,
                    height: 14))
                    {
                        if (Application.isPlaying)
                            return;

                        ///---新建文字模版信息类
                        XHud_LibraryArg_TextStyle info = new XHud_LibraryArg_TextStyle();
                        info.Type = xHud_TextType.TmpText;
                        info.tmp_rich = sp_tmp_rich.boolValue;
                        info.tmp_contentwrap = (TextWrappingModes)sp_tmp_contentwrap.enumValueIndex;
                        info.tmp_overflow = (TextOverflowModes)sp_tmp_overflow.enumValueIndex;
                        info.tmp_style = (FontStyles)sp_tmp_style.enumValueIndex;
                        info.tmp_anchor = (TmpContentAnchor)sp_tmp_anchor.enumValueIndex;
                        info.tmp_font = sp_tmp_font.objectReferenceValue as TMP_FontAsset;
                        info.tmp_size = sp_tmp_size.floatValue;
                        info.tmp_space_character = sp_tmp_space_character.floatValue;
                        info.tmp_space_word = sp_tmp_space_word.floatValue;
                        info.tmp_space_lineheight = sp_tmp_space_lineheight.floatValue;
                        info.tmp_space_paragraph = sp_tmp_space_paragraph.floatValue;
                        info.tmp_WrappingRatios = sp_tmp_WrappingRatios.floatValue;
                        info.tmp_contentmargin = sp_tmp_contentmargin.vector4Value;
                        info.tmp_color = sp_tmp_color.colorValue;
                        info.tmp_EnableAutoSizing = sp_tmp_EnableAutoSizing.boolValue;
                        info.tmp_FontSizeMin = sp_tmp_FontSizeMin.floatValue;
                        info.tmp_FontSizeMax = sp_tmp_FontSizeMax.floatValue;
                        info.tmp_CharWidthMaxAdj = sp_tmp_CharWidthMaxAdj.floatValue;
                        info.tmp_LineSpacingMax = sp_tmp_LineSpacingMax.floatValue;
                        info.gra_A = sp_gra_A.colorValue;
                        info.gra_B = sp_gra_B.colorValue;
                        info.gra_C = sp_gra_C.colorValue;
                        info.gra_D = sp_gra_D.colorValue;
                        info.gra_Invert = sp_gra_Invert.boolValue;
                        info.gra_Used = sp_gra_Used.boolValue;
                        info.gra_ColorMode = (ColorMode)sp_gra_ColorMode.enumValueIndex;
                        info.gra_ColorModeName = sp_gra_ColorModeName.stringValue;
                        info.Raycast = sp_Raycast.boolValue;
                        info.Maskable = sp_Maskable.boolValue;

                        Open_Hud_Library_TextStyle_Setter(info);
                    }
                    #endregion

                    XGUI.layout_flexspace();

                    #region 查看当前字体库
                    if (XGUI.layout_button(
                    tooltip: "查看当前字体库",
                    tex_release: openlib_r,
                    tex_press: openlib_p,
                    tex_gui_color: Color.white,
                    border: new RectOffset(0, 0, 0, 0),
                    width: 14,
                    height: 14))
                    {
                        if (HudManager.Hud_TextStyleLibrary != null)
                            EditorUtility.OpenPropertyEditor(HudManager.Hud_TextStyleLibrary);
                    }
                    #endregion
                }
                if (HudManager.Hud_TextStyleLibrary != null && !HudManager.Hud_TextStyleLibrary.TextStyle_Library_IsEmpty())
                {
                    XGUI.layout_flexspace();

                    #region 刷新样式
                    if (XGUI.layout_button(
                        tooltip: "根据选择的快速样式刷新文字样式",
                        tex_release: update_r,
                        tex_press: update_p,
                        tex_gui_color: Color.white,
                        border: new RectOffset(0, 0, 0, 0),
                        width: 14,
                        height: 14))
                    {
                        UpdateTextStyle_WithTarget();
                        return;
                    }
                    #endregion
                }
            }
            else
            {
                #region 批量刷新样式
                if (!HudManager.Hud_TextStyleLibrary)
                {
                    if (!HudManager.Hud_TextStyleLibrary.TextStyle_Library_IsEmpty())
                    {
                        if (XGUI.layout_button(
                            tooltip: "根据选择的快速样式刷新文字样式",
                            tex_release: update_r,
                            tex_press: update_p,
                            tex_gui_color: Color.white,
                            border: new RectOffset(0, 0, 0, 0),
                            width: 14,
                            height: 14))
                        {
                            UpdateTextStyle_WithTarget();
                            return;
                        }
                    }
                }
                #endregion
            }

            XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 颜色接管者
            XGUI.layout_group_start(
                 type: XGUIContainerType.Vertical,
                 bg_fill: XGUIGroup.缺口纯色边框,
                 bg_color: XGUIColor.亮白,
                 bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                 title: "颜色接管者",
                 title_size: XGUIFontSize.M,
                 title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                 title_clipping: TextClipping.Clip,
                 padding: new RectOffset(10, 10, 15, 15),
                 foldout: BaseScript.fold_option);

            XGUI.layout_label(
                           text: $"文字颜色由  <color=#{h_color}>{tooltip_owner_names[sp_TextColorOwner.enumValueIndex]}</color>  接管",
                           size: XGUIFontSize.S,
                           anchor: TextAnchor.MiddleCenter,
                           text_color: Color.white,
                           offset: new Vector2(0, 0),
                           padding: new RectOffset(0, 0, 0, 0),
                           margin: new RectOffset(0, 0, 6, 0),
                           font_style: FontStyle.Normal,
                           clipping: TextClipping.Clip);

            sp_TextColorOwner.enumValueIndex = XGUI.layout_clickselector(
                index: sp_TextColorOwner.enumValueIndex,
                icon_btns: clickselector_args,
                btn_color_normal: Color.white,
                btn_color_selected: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                btn_height: 60,
                padding: new RectOffset(0, 0, 0, 0),
                margin: new RectOffset(0, 0, 0, 0)
                );

            sp_TextColorOwner.serializedObject.ApplyModifiedProperties();

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 选项
            BaseScript.fold_option = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "选项",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: BaseScript.fold_option);

            if (!BaseScript.fold_option)
            {
                #region 射线检测
                XGUI.SetEnabled(!sp_LibStyle_Effect_raycast.boolValue || !sp_StyleLibSynching.boolValue);
                DrawLabeledToggle("射线检测", "", sp_Raycast, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, stroptions_enabled, (b) => { });
                XGUI.SetEnabled(true);
                #endregion

                #region 遮罩
                XGUI.SetEnabled(!sp_LibStyle_Effect_maskable.boolValue || !sp_StyleLibSynching.boolValue);
                DrawLabeledToggle("遮罩", "", sp_Maskable, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, stroptions_enabled, (b) => { });
                XGUI.SetEnabled(true);
                #endregion

                #region 富文本
                XGUI.SetEnabled(!sp_LibStyle_Effect_rich.boolValue || !sp_StyleLibSynching.boolValue);
                DrawLabeledToggle("富文本", "", sp_tmp_rich, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, stroptions_enabled, (b) => { });
                XGUI.SetEnabled(true);
                #endregion

                #region 自适应尺寸
                XGUI.SetEnabled(!sp_LibStyle_Effect_autosizing.boolValue || !sp_StyleLibSynching.boolValue);
                DrawLabeledToggle("自适应尺寸", "", sp_tmp_EnableAutoSizing, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, stroptions_enabled, (b) => { });
                XGUI.SetEnabled(true);
                #endregion

                #region 尺寸全局联动
                DrawLabeledToggle("尺寸全局联动", "", sp_SyncGlobalFontSize, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, stroptions_syncsize, (b) => { });
                XGUI.SetEnabled(true);
                #endregion

                #region 实时匹配库
                if (HudManager != null && HudManager.Hud_TextStyleLibrary != null)
                {
                    if (!HudManager.Hud_TextStyleLibrary.TextStyle_Library_IsEmpty())
                    {
                        DrawLabeledToggle("实时匹配库", "", sp_StyleLibSynching, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, stroptions_enabled, (b) =>
                        {
                            sp_StyleLibSynching.boolValue = b;
                            sp_StyleLibSynching.serializedObject.ApplyModifiedProperties();

                            if (sp_StyleLibSynching.boolValue)
                            {
                                sp_TextColorOwner.enumValueIndex = (int)TextColorOwner.StyleLibrary;
                            }
                        });
                    }
                    else
                    {
                        XGUI.layout_label(
                            text: "字体样式库为空",
                            size: XGUIFontSize.S,
                            anchor: TextAnchor.MiddleCenter,
                            text_color: Color.white * 0.85f,
                            offset: new Vector2(0, 0),
                            padding: new RectOffset(0, 0, 0, 0),
                            margin: new RectOffset(0, 0, 6, 0),
                            clipping: TextClipping.Clip);
                    }
                }
                else
                {
                    sp_StyleLibSynching.boolValue = false;
                    sp_StyleLibSynching.serializedObject.ApplyModifiedProperties();

                    XGUI.layout_label(
                          text: "您未在Hud管理器中配置或指定字体样式库",
                          size: XGUIFontSize.S,
                          anchor: TextAnchor.MiddleCenter,
                          text_color: Color.white * 0.85f,
                          offset: new Vector2(0, 0),
                          padding: new RectOffset(0, 0, 0, 0),
                          margin: new RectOffset(0, 0, 6, 0),
                          clipping: TextClipping.Clip);
                }
                #endregion

                #region 使用渐变色
                XGUI.SetEnabled(!sp_LibStyle_Effect_gradientcolor.boolValue || !sp_StyleLibSynching.boolValue);
                DrawLabeledToggle("使用渐变色", "", sp_gra_Used, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, stroptions_enabled, (b) => { });
                XGUI.SetEnabled(true);
                #endregion

                #region 翻转渐变色
                XGUI.SetEnabled((!sp_LibStyle_Effect_gradientcolor.boolValue || !sp_StyleLibSynching.boolValue) && sp_gra_Used.boolValue);
                DrawLabeledToggle("翻转渐变色", "", sp_gra_Invert, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, stroptions_flip, (b) => { });
                XGUI.SetEnabled(true);
                #endregion

                #region 渐变色模式
                XGUI.SetEnabled((!sp_LibStyle_Effect_gradientcolor.boolValue || !sp_StyleLibSynching.boolValue) && sp_gra_Used.boolValue);
                sp_gra_ColorModeName.stringValue = XGUI.layout_string_popup(
                    title: "渐变色模式",
                    title_width: 120,
                    title_size: XGUIFontSize.M,
                    title_anchor: TextAnchor.MiddleLeft,
                    prop: sp_gra_ColorModeName,
                    options: stroptions_gramode,
                    opt_text_size: XGUIFontSize.M,
                    opt_text_color: Color.black,
                    opt_text_padding: new RectOffset(10, 10, 0, 0),
                    opt_anchor: TextAnchor.MiddleLeft,
                    opt_font_style: FontStyle.Normal,
                    opt_bg_fill: XGUIGroup.实体,
                    opt_bg_color: XGUIColor.亮白,
                    opt_bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    margin: new RectOffset(0, 0, 5, 5),
                    padding: new RectOffset(5, 5, 0, 0),
                    title_margin: new RectOffset(0, 0, 0, 0),
                    icon_arrow_color: Color.black,
                    act_on_changed: (s) =>
                    {
                        sp_gra_ColorModeName.stringValue = s;
                        sp_gra_ColorModeName.serializedObject.ApplyModifiedProperties();

                        if (Targets_Selected())
                        {
                            for (int i = 0; i < SelectedObjects.Length; i++)
                            {
                                switch (SelectedObjects[i].TextStyleInfo.gra_ColorModeName)
                                {
                                    case "单色":
                                        SelectedObjects[i].TextStyleInfo.gra_ColorMode = ColorMode.Single;
                                        break;
                                    case "水平渐变":
                                        SelectedObjects[i].TextStyleInfo.gra_ColorMode = ColorMode.HorizontalGradient;
                                        break;
                                    case "垂直渐变":
                                        SelectedObjects[i].TextStyleInfo.gra_ColorMode = ColorMode.VerticalGradient;
                                        break;
                                    case "四角渐变":
                                        SelectedObjects[i].TextStyleInfo.gra_ColorMode = ColorMode.FourCornersGradient;
                                        break;
                                }
                            }
                        }
                        else
                        {
                            switch (sp_gra_ColorModeName.stringValue)
                            {
                                case "单色":
                                    sp_gra_ColorMode.enumValueIndex = (int)ColorMode.Single;
                                    break;
                                case "水平渐变":
                                    sp_gra_ColorMode.enumValueIndex = (int)ColorMode.HorizontalGradient;
                                    break;
                                case "垂直渐变":
                                    sp_gra_ColorMode.enumValueIndex = (int)ColorMode.VerticalGradient;
                                    break;
                                case "四角渐变":
                                    sp_gra_ColorMode.enumValueIndex = (int)ColorMode.FourCornersGradient;
                                    break;
                            }
                        }
                    });
                XGUI.SetEnabled(true);
                #endregion
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 文字样式库
            XGUI.layout_group_start(
                 type: XGUIContainerType.Vertical,
                 bg_fill: XGUIGroup.缺口纯色边框,
                 bg_color: XGUIColor.亮白,
                 bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                 title: "文字样式库",
                 title_size: XGUIFontSize.M,
                 title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                 title_clipping: TextClipping.Clip,
                 padding: new RectOffset(10, 10, 15, 15),
                 foldout: BaseScript.fold_option);

            #region 文字样式库
            if (HudManager != null && HudManager.Hud_TextStyleLibrary != null)
            {
                if (HudManager.Hud_TextStyleLibrary.TextStyle_Library_GetCount(xHud_TextType.Text) > 0)
                {
                    XGUI.layout_group_start(
                        type: XGUIContainerType.Horizontal,
                        bg_fill: XGUIGroup.透明,
                        bg_color: XGUIColor.亮白,
                        bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                        absolute_margin: true,
                        absolute_padding: true,
                        padding: new RectOffset(5, 5, 5, 15));

                    string[] str_fotlib_ItemsName = HudManager.Hud_TextStyleLibrary.TextStyle_Library_GetAllNames_With_TmpText();

                    sp_StyleName.stringValue = XGUI.layout_string_popup(
                        //title: "文字样式库",
                        //title_width: 0,
                        //title_size: XGUIFontSize.M,
                        //title_anchor: TextAnchor.MiddleLeft,
                        prop: sp_StyleName,
                        options: str_fotlib_ItemsName,
                        opt_text_size: XGUIFontSize.M,
                        opt_text_color: Color.black,
                        opt_text_padding: new RectOffset(10, 10, 0, 0),
                        opt_anchor: TextAnchor.MiddleLeft,
                        opt_font_style: FontStyle.Normal,
                        opt_bg_fill: XGUIGroup.实体,
                        opt_bg_color: XGUIColor.亮白,
                        opt_bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        margin: new RectOffset(0, 0, 5, 5),
                        padding: new RectOffset(0, 0, 0, 0),
                        title_margin: new RectOffset(0, 0, 0, 0),
                        icon_arrow_color: Color.black,
                        act_on_changed: (s) =>
                        {
                            sp_StyleName.stringValue = s;
                            sp_StyleName.serializedObject.ApplyModifiedProperties();

                            if (Targets_Selected())
                            {
                                for (int i = 0; i < SelectedObjects.Length; i++)
                                {
                                    if (Application.isPlaying)
                                    {
                                        SelectedObjects[i].TextStyleInfo.CopyData_Ignored_LibraryToggle(HudManager.Hud_TextStyleLibrary.TextStyle_Library_GetTextStyleInfo(sp_StyleName.stringValue, xHud_TextType.TmpText));
                                    }
                                    else
                                    {
                                        SelectedObjects[i].TextStyleInfo.CopyData_Ignored_LibraryToggle(HudManager.Hud_TextStyleLibrary.TextStyle_Library_GetTextStyleInfo(sp_StyleName.stringValue, xHud_TextType.TmpText));
                                    }
                                }
                            }
                            else
                            {
                                if (Application.isPlaying)
                                {
                                    BaseScript.TextStyleInfo.CopyData_Ignored_LibraryToggle(HudManager.Hud_TextStyleLibrary.TextStyle_Library_GetTextStyleInfo(s, xHud_TextType.TmpText));
                                }
                                else
                                {
                                    BaseScript.TextStyleInfo.CopyData_Ignored_LibraryToggle(HudManager.Hud_TextStyleLibrary.TextStyle_Library_GetTextStyleInfo(s, xHud_TextType.TmpText));
                                }
                            }
                        });

                    if (!Targets_Selected())
                    {
                        XGUI.layout_space(10);

                        if (XGUI.layout_button(
                            tooltip: "定位到样式库",
                            tex_release: locate_r,
                            tex_press: locate_p,
                            tex_gui_color: Color.white,
                            border: new RectOffset(0, 0, 0, 0),
                            width: 14,
                            height: 14))
                        {
                            //获取目标元素的索引号
                            int index = 0;
                            for (int k = 0; k < str_fotlib_ItemsName.Length; k++)
                            {
                                if (str_fotlib_ItemsName[k] == sp_StyleName.stringValue)
                                {
                                    index = k;
                                    break;
                                }
                            }
                            //定位到元素库中的对应当前元素
                            //打开目标元素库
                            Editor_XHud_MenuItemsAction_OpenLibrary.open_textstyle();
                            HudManager.Hud_TextStyleLibrary.TextStyleLibrary_Location(str_fotlib_ItemsName[index]);
                        }
                    }
                    XGUI.layout_group_end(type: XGUIContainerType.Horizontal);
                }
                else
                {
                    XGUI.layout_label(
                         text: "字体样式库为空",
                         size: XGUIFontSize.S,
                         anchor: TextAnchor.MiddleCenter,
                         text_color: Color.white * 0.85f,
                         offset: new Vector2(0, 0),
                         padding: new RectOffset(0, 0, 0, 0),
                         margin: new RectOffset(0, 0, 6, 0),
                         clipping: TextClipping.Clip);
                }
            }
            else
            {
                XGUI.layout_label(
                          text: "您未在Hud管理器中配置或指定字体样式库",
                          size: XGUIFontSize.S,
                          anchor: TextAnchor.MiddleCenter,
                          text_color: Color.white * 0.85f,
                          offset: new Vector2(0, 0),
                          padding: new RectOffset(0, 0, 0, 0),
                          margin: new RectOffset(0, 0, 6, 0),
                          clipping: TextClipping.Clip);
            }
            #endregion

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            Rect rect_gra_color = XGUI.GetLastRect();

            #region 库同步项
            if (sp_StyleLibSynching.boolValue)
            {
                BaseScript.fold_sync = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "库同步项",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: BaseScript.fold_sync);

                if (!BaseScript.fold_sync)
                {
                    #region 射线检测
                    DrawLabeledToggle("射线检测", "", sp_LibStyle_Effect_raycast, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, stroptions_syncstyle, (b) => { });
                    #endregion

                    #region 遮罩
                    DrawLabeledToggle("遮罩", "", sp_LibStyle_Effect_maskable, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, stroptions_syncstyle, (b) => { });
                    #endregion

                    #region 富文本
                    DrawLabeledToggle("富文本", "", sp_LibStyle_Effect_rich, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, stroptions_syncstyle, (b) => { });
                    #endregion

                    #region 尺寸
                    DrawLabeledToggle("尺寸", "", sp_LibStyle_Effect_size, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, stroptions_syncstyle, (b) => { });
                    if (sp_LibStyle_Effect_size.boolValue)
                    {
                        sp_LibStyle_Effect_autosizing.boolValue = false;
                        sp_LibStyle_Effect_autosizing.serializedObject.ApplyModifiedProperties();
                        sp_tmp_EnableAutoSizing.boolValue = false;
                        sp_tmp_EnableAutoSizing.serializedObject.ApplyModifiedProperties();
                    }
                    #endregion

                    #region 自适应尺寸
                    DrawLabeledToggle("自适应尺寸", "", sp_LibStyle_Effect_autosizing, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, stroptions_syncstyle, (b) => { });
                    if (sp_LibStyle_Effect_autosizing.boolValue)
                    {
                        sp_LibStyle_Effect_size.boolValue = false;
                        sp_LibStyle_Effect_size.serializedObject.ApplyModifiedProperties();
                        sp_tmp_EnableAutoSizing.boolValue = true;
                        sp_tmp_EnableAutoSizing.serializedObject.ApplyModifiedProperties();
                    }
                    #endregion

                    #region 渐变色
                    DrawLabeledToggle("渐变色", "", sp_LibStyle_Effect_gradientcolor, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, stroptions_syncstyle, (b) => { });
                    #endregion

                    #region 对齐
                    DrawLabeledToggle("对齐", "", sp_LibStyle_Effect_align, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, stroptions_syncstyle, (b) => { });
                    #endregion

                    #region 字体
                    DrawLabeledToggle("字体", "", sp_LibStyle_Effect_font, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, stroptions_syncstyle, (b) => { });
                    #endregion

                    #region 样式
                    DrawLabeledToggle("样式", "", sp_LibStyle_Effect_style, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, stroptions_syncstyle, (b) => { });
                    #endregion

                    #region 颜色
                    DrawLabeledToggle("颜色", "", sp_LibStyle_Effect_color, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, stroptions_syncstyle, (b) => { });
                    #endregion

                    #region 文本包裹
                    DrawLabeledToggle("包裹", "", sp_LibStyle_Effect_wrap, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, stroptions_syncstyle, (b) => { });
                    #endregion

                    #region 溢出
                    DrawLabeledToggle("溢出", "", sp_LibStyle_Effect_overflow, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, stroptions_syncstyle, (b) => { });
                    #endregion

                    #region 包裹比例
                    DrawLabeledToggle("包裹比例", "", sp_LibStyle_Effect_wrapratio, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, stroptions_syncstyle, (b) => { });
                    #endregion

                    #region 边距
                    DrawLabeledToggle("边距", "", sp_LibStyle_Effect_margin, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, stroptions_syncstyle, (b) => { });
                    #endregion

                    #region 间距
                    DrawLabeledToggle("间距", "", sp_LibStyle_Effect_space, 120, XGUIToggleStyle.实体, XHud_Dashboard.themes_get_color(ThemeColorType.primary), Color.white * 0.65f, Color.white, Color.white, stroptions_syncstyle, (b) => { });
                    #endregion
                }
                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            }
            #endregion

            #region 文字内容
            XGUI.layout_group_start(
            type: XGUIContainerType.Vertical,
            bg_fill: XGUIGroup.缺口纯色边框,
            bg_color: XGUIColor.亮白,
            bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
            title: "文字内容",
            title_size: XGUIFontSize.M,
            title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
            title_clipping: TextClipping.Clip,
            padding: new RectOffset(10, 10, 10, 10));

            XGUI.ChangedCheck_Start();

            #region 文字内容
            sp_Text.stringValue = XGUI.layout_inputfield(
                prop: sp_Text.stringValue,
                text_wrap: true,
                field_fontsize: XGUIFontSize.M,
                field_text_offset: Vector2.zero,
                field_height: 80,
                field_padding: new RectOffset(0, 0, 5, 5),
                field_margin: new RectOffset(0, 0, 5, 5),
                field_text_color: Color.white,
                //field_text_font: XGUI.GetFont("xg-medium"),
                field_text_style: FontStyle.Normal,
                field_text_anchor: TextAnchor.UpperLeft);

            sp_Text.serializedObject.ApplyModifiedProperties();
            #endregion

            if (XGUI.ChangedCheck_End())
            {
                if (Targets_Selected())
                {
                    for (int i = 0; i < SelectedObjects.Length; i++)
                    {
                        SelectedObjects[i].text = sp_Text.stringValue;
                    }
                }
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 参数
            BaseScript.fold_param = XGUI.layout_group_start(
            type: XGUIContainerType.Vertical,
            bg_fill: XGUIGroup.缺口纯色边框,
            bg_color: XGUIColor.亮白,
            bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
            title: "参数",
            title_size: XGUIFontSize.M,
            title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
            title_clipping: TextClipping.Clip,
            padding: new RectOffset(10, 10, 15, 15),
            foldout: BaseScript.fold_param);

            if (!BaseScript.fold_param)
            {
                XGUI.ChangedCheck_Start();

                #region 标识
                XGUI.layout_property_field(
                    title: "标识",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: sp_Indicator,
                    prop_margin: new RectOffset(5, 5, 0, 5));
                #endregion

                #region 字体
                XGUI.SetEnabled(!sp_LibStyle_Effect_font.boolValue || !sp_StyleLibSynching.boolValue);
                XGUI.layout_property_field(
                    title: "字体",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: sp_tmp_font,
                    prop_margin: new RectOffset(5, 5, 0, 5));
                XGUI.SetEnabled(true);
                #endregion

                #region 锚点
                XGUI.layout_property_field(
                    title: "锚点",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: sp_tmp_anchor,
                    prop_margin: new RectOffset(5, 5, 0, 5));
                #endregion

                #region 均分
                TmpContentAnchor anchors = (TmpContentAnchor)sp_tmp_anchor.enumValueIndex;
                if (anchors == TmpContentAnchor.中心左右均分 || anchors == TmpContentAnchor.中线左右均分 || anchors == TmpContentAnchor.基线左右均分 || anchors == TmpContentAnchor.底部左右均分 || anchors == TmpContentAnchor.顶部均分 || anchors == TmpContentAnchor.中心填充 || anchors == TmpContentAnchor.中线左右填充 || anchors == TmpContentAnchor.基线左右填充 || anchors == TmpContentAnchor.底部左右填充 || anchors == TmpContentAnchor.顶部填充)
                {
                    XGUI.layout_property_field(
                        title: "均分",
                        title_size: XGUIFontSize.M,
                        title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                        title_width: 90,
                        prop: sp_tmp_WrappingRatios,
                        prop_margin: new RectOffset(5, 5, 0, 5));
                }
                #endregion

                #region 尺寸
                XGUI.SetEnabled((!sp_LibStyle_Effect_size.boolValue || !sp_LibStyle_Effect_autosizing.boolValue || !sp_StyleLibSynching.boolValue) && !sp_tmp_EnableAutoSizing.boolValue);
                XGUI.layout_property_field(
                    title: "尺寸",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: sp_tmp_size,
                    prop_margin: new RectOffset(5, 5, 0, 5));
                #endregion

                #region 自适应尺寸
                XGUI.SetEnabled((!sp_LibStyle_Effect_autosizing.boolValue || !sp_StyleLibSynching.boolValue) && sp_tmp_EnableAutoSizing.boolValue);

                #region 最小尺寸
                XGUI.layout_property_field(
                    title: "最小尺寸",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: sp_tmp_FontSizeMin,
                    prop_margin: new RectOffset(5, 5, 0, 5));
                #endregion

                #region 最大尺寸
                XGUI.layout_property_field(
                    title: "最大尺寸",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: sp_tmp_FontSizeMax,
                    prop_margin: new RectOffset(5, 5, 0, 5));
                #endregion

                #region 字符宽度
                XGUI.layout_property_field(
                    title: "字符宽度",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: sp_tmp_CharWidthMaxAdj,
                    prop_margin: new RectOffset(5, 5, 0, 5));
                #endregion

                #region 最大行高
                XGUI.layout_property_field(
                    title: "最大行高",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: sp_tmp_LineSpacingMax,
                    prop_margin: new RectOffset(5, 5, 0, 5));
                #endregion

                XGUI.SetEnabled(true);
                #endregion

                #region 包裹
                XGUI.SetEnabled((!sp_LibStyle_Effect_wrap.boolValue || !sp_StyleLibSynching.boolValue));
                XGUI.layout_property_field(
                    title: "包裹",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: sp_tmp_contentwrap,
                    prop_margin: new RectOffset(5, 5, 0, 5));
                #endregion

                #region 包裹比例
                XGUI.SetEnabled((!sp_LibStyle_Effect_wrapratio.boolValue || !sp_StyleLibSynching.boolValue));
                XGUI.layout_property_field(
                    title: "包裹比例",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: sp_tmp_WrappingRatios,
                    prop_margin: new RectOffset(5, 5, 0, 5));
                #endregion

                #region 溢出
                XGUI.SetEnabled((!sp_LibStyle_Effect_overflow.boolValue || !sp_StyleLibSynching.boolValue));
                XGUI.layout_property_field(
                    title: "溢出",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: sp_tmp_overflow,
                    prop_margin: new RectOffset(5, 5, 0, 5));
                #endregion

                #region 样式
                XGUI.SetEnabled((!sp_LibStyle_Effect_style.boolValue || !sp_StyleLibSynching.boolValue));
                XGUI.layout_property_field(
                    title: "样式",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: sp_tmp_style,
                    prop_margin: new RectOffset(5, 5, 0, 5));
                #endregion

                #region 字符间距
                XGUI.SetEnabled((!sp_LibStyle_Effect_space.boolValue || !sp_StyleLibSynching.boolValue));
                XGUI.layout_property_field(
                    title: "字符间距",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: sp_tmp_space_character,
                    prop_margin: new RectOffset(5, 5, 0, 5));
                #endregion

                #region 单词间距
                XGUI.SetEnabled((!sp_LibStyle_Effect_space.boolValue || !sp_StyleLibSynching.boolValue));
                XGUI.layout_property_field(
                    title: "单词间距",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: sp_tmp_space_word,
                    prop_margin: new RectOffset(5, 5, 0, 5));
                #endregion

                #region 行高间距
                XGUI.SetEnabled((!sp_LibStyle_Effect_space.boolValue || !sp_StyleLibSynching.boolValue));
                XGUI.layout_property_field(
                    title: "行高间距",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: sp_tmp_space_lineheight,
                    prop_margin: new RectOffset(5, 5, 0, 5));
                #endregion

                #region 段落间距
                XGUI.SetEnabled((!sp_LibStyle_Effect_space.boolValue || !sp_StyleLibSynching.boolValue));
                XGUI.layout_property_field(
                    title: "段落间距",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: sp_tmp_space_paragraph,
                    prop_margin: new RectOffset(5, 5, 0, 5));
                #endregion

                #region 边距
                XGUI.SetEnabled((!sp_LibStyle_Effect_margin.boolValue || !sp_StyleLibSynching.boolValue));
                XGUI.layout_property_field(
                    title: "边距",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: sp_tmp_contentmargin,
                    prop_margin: new RectOffset(5, 5, 0, 5));
                #endregion

                #region 颜色
                XGUI.SetEnabled((!sp_LibStyle_Effect_color.boolValue || !sp_StyleLibSynching.boolValue));
                XGUI.layout_property_field(
                    title: "颜色",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: sp_tmp_color,
                    prop_margin: new RectOffset(5, 5, 0, 5));
                #endregion

                #region 颜色接管者
                XGUI.SetEnabled((!sp_LibStyle_Effect_color.boolValue || !sp_StyleLibSynching.boolValue));
                XGUI.layout_property_field(
                    title: "颜色接管者",
                    title_size: XGUIFontSize.M,
                    title_hover_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                    title_width: 90,
                    prop: sp_TextColorOwner,
                    prop_margin: new RectOffset(5, 5, 0, 5));
                XGUI.SetEnabled(true);
                #endregion

                XGUI.layout_space(5);

                if (XGUI.ChangedCheck_End())
                {
                    if (Targets_Selected())
                    {
                        for (int i = 0; i < SelectedObjects.Length; i++)
                        {
                            TmpContentAnchor anchor = (TmpContentAnchor)sp_tmp_anchor.enumValueIndex;
                            SelectedObjects[i].TextStyleInfo.tmp_Set_Alignment(anchor);
                        }
                    }
                    else
                    {
                        TmpContentAnchor anchor = (TmpContentAnchor)sp_tmp_anchor.enumValueIndex;
                        BaseScript.TextStyleInfo.tmp_Set_Alignment(anchor);
                    }
                }
            }

            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            #region 渐变色
            if (sp_gra_Used.boolValue)
            {
                BaseScript.fold_gradient = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "渐变色",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: BaseScript.fold_gradient);

                if (!BaseScript.fold_gradient)
                {
                    CreateGradientTexture((ColorMode)sp_gra_ColorMode.enumValueIndex, sp_gra_Invert.boolValue, sp_gra_A.colorValue, sp_gra_B.colorValue, sp_gra_C.colorValue, sp_gra_D.colorValue);

                    Rect rect_gra_root = XGUI.GetControlRect(false, 130);

                    Rect rect_GradientIcon;

                    if (sp_gra_ColorModeName.stringValue == "单色")
                    {
                        rect_GradientIcon = new Rect((rect_gra_color.width / 2) - 12, rect_gra_root.y + 15, 60, 60);
                    }
                    else
                    {
                        rect_GradientIcon = new Rect((rect_gra_color.width / 2) - 12, rect_gra_root.y + 35, 60, 60);
                    }

                    XGUI.gui_icon(rect_GradientIcon, GradientTexture);

                    XGUI.ChangedCheck_Start();
                    switch (sp_gra_ColorModeName.stringValue)
                    {
                        case "单色":
                            sp_gra_A.colorValue = XGUI.gui_colorfield(
                                rect: new Rect(rect_GradientIcon.x, rect_GradientIcon.y + 80, 61, 20),
                                prop: sp_gra_A.colorValue);
                            break;
                        case "水平渐变":
                            if (sp_gra_Invert.boolValue)
                            {
                                sp_gra_B.colorValue = XGUI.gui_colorfield(
                                    rect: new Rect(rect_gra_color.x + 25, rect_GradientIcon.y + 20, 61, 20),
                                    prop: sp_gra_B.colorValue);

                                sp_gra_A.colorValue = XGUI.gui_colorfield(
                                    rect: new Rect(rect_gra_color.width - 68, rect_GradientIcon.y + 20, 61, 20),
                                    prop: sp_gra_A.colorValue);
                            }
                            else
                            {
                                sp_gra_A.colorValue = XGUI.gui_colorfield(
                                    rect: new Rect(rect_gra_color.x + 25, rect_GradientIcon.y + 20, 61, 20),
                                    prop: sp_gra_A.colorValue);

                                sp_gra_B.colorValue = XGUI.gui_colorfield(
                                    rect: new Rect(rect_gra_color.width - 68, rect_GradientIcon.y + 20, 61, 20),
                                    prop: sp_gra_B.colorValue);
                            }
                            break;
                        case "垂直渐变":
                            if (sp_gra_Invert.boolValue)
                            {
                                sp_gra_B.colorValue = XGUI.gui_colorfield(
                                    rect: new Rect(rect_GradientIcon.x, rect_GradientIcon.y + 72, 61, 20),
                                    prop: sp_gra_B.colorValue);

                                sp_gra_A.colorValue = XGUI.gui_colorfield(
                                    rect: new Rect(rect_GradientIcon.x, rect_GradientIcon.y - 32, 61, 20),
                                    prop: sp_gra_A.colorValue);
                            }
                            else
                            {
                                sp_gra_A.colorValue = XGUI.gui_colorfield(
                                    rect: new Rect(rect_GradientIcon.x, rect_GradientIcon.y + 72, 61, 20),
                                    prop: sp_gra_A.colorValue);

                                sp_gra_B.colorValue = XGUI.gui_colorfield(
                                    rect: new Rect(rect_GradientIcon.x, rect_GradientIcon.y - 32, 61, 20),
                                    prop: sp_gra_B.colorValue);
                            }
                            break;
                        case "四角渐变":
                            if (sp_gra_Invert.boolValue)
                            {
                                sp_gra_B.colorValue = XGUI.gui_colorfield(
                                   rect: new Rect(rect_gra_color.x + 25, rect_GradientIcon.y - 20, 61, 20),
                                   prop: sp_gra_B.colorValue);

                                sp_gra_A.colorValue = XGUI.gui_colorfield(
                                    rect: new Rect(rect_gra_color.width - 68, rect_GradientIcon.y - 20, 61, 20),
                                    prop: sp_gra_A.colorValue);

                                sp_gra_D.colorValue = XGUI.gui_colorfield(
                                   rect: new Rect(rect_gra_color.x + 25, rect_GradientIcon.y + 60, 61, 20),
                                   prop: sp_gra_D.colorValue);

                                sp_gra_C.colorValue = XGUI.gui_colorfield(
                                    rect: new Rect(rect_gra_color.width - 68, rect_GradientIcon.y + 60, 61, 20),
                                    prop: sp_gra_C.colorValue);
                            }
                            else
                            {
                                sp_gra_A.colorValue = XGUI.gui_colorfield(
                                    rect: new Rect(rect_gra_color.x + 25, rect_GradientIcon.y - 20, 61, 20),
                                    prop: sp_gra_A.colorValue);

                                sp_gra_B.colorValue = XGUI.gui_colorfield(
                                    rect: new Rect(rect_gra_color.width - 68, rect_GradientIcon.y - 20, 61, 20),
                                    prop: sp_gra_B.colorValue);

                                sp_gra_C.colorValue = XGUI.gui_colorfield(
                                   rect: new Rect(rect_gra_color.x + 25, rect_GradientIcon.y + 60, 61, 20),
                                   prop: sp_gra_C.colorValue);

                                sp_gra_D.colorValue = XGUI.gui_colorfield(
                                    rect: new Rect(rect_gra_color.width - 68, rect_GradientIcon.y + 60, 61, 20),
                                    prop: sp_gra_D.colorValue);
                            }
                            break;
                    }
                    if (XGUI.ChangedCheck_End())
                    {
                        sp_gra_A.serializedObject.ApplyModifiedProperties();
                        sp_gra_B.serializedObject.ApplyModifiedProperties();
                        sp_gra_C.serializedObject.ApplyModifiedProperties();
                        sp_gra_D.serializedObject.ApplyModifiedProperties();
                    }
                }

                XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            }
            #endregion

            #region 右键菜单
            Event e = Event.current;
            if (e.type == EventType.MouseDown && e.button == 1)
            {
                // 创建右键菜单
                GenericMenu menu = new GenericMenu();
                menu.AddDisabledItem(new GUIContent("组件样式操作"));
                menu.AddItem(new GUIContent("C (复制样式)"), false, () =>
                {
                    XHud_LibraryArg_TextStyle info = new XHud_LibraryArg_TextStyle();
                    info.CopyData_Ignored_LibraryToggle(BaseScript.TextStyleInfo);

                    string json = JsonUtility.ToJson(info);

                    XGUI.x_Editor_Data_Set_With_String("xData_XHud_TmpTextStyle", json);

                    XGUI.dialog(
                        type: XGUIDialogType.确认,
                        windowtitle: "XHud - Tmp文字组件消息",
                        title: "文字样式复制",
                        msg: $"已将当前文字样式存入 xData ！",
                        ok: "明白",
                        PrimaryIndex: 0,
                        usemodal: true,
                        themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                });
                menu.AddItem(new GUIContent("V (粘贴样式)"), false, () =>
                {
                    XHud_LibraryArg_TextStyle info = JsonUtility.FromJson<XHud_LibraryArg_TextStyle>(XGUI.x_Editor_Data_Get_With_String("xData_XHud_TmpTextStyle"));

                    if (Targets_Selected())
                    {
                        for (int i = 0; i < SelectedObjects.Length; i++)
                        {
                            // 使用 Undo.RecordObject 来记录对目标对象的修改
                            Undo.RecordObject(SelectedObjects[i], "Paste Style Name");
                            SelectedObjects[i].TextStyleInfo.CopyData(info);
                        }

                        XGUI.dialog(
                           type: XGUIDialogType.确认,
                           windowtitle: "XHud - Tmp文字组件消息",
                           title: "文字样式粘贴",
                           msg: $"已从 xData 中获取文字样式并应用到当前所选的文字组件！",
                           ok: "明白",
                           PrimaryIndex: 0,
                           usemodal: true,
                           themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                    }
                    else
                    {
                        // 使用 Undo.RecordObject 来记录对目标对象的修改
                        Undo.RecordObject(BaseScript, "Paste Style Name");
                        BaseScript.TextStyleInfo.CopyData(info);

                        XGUI.dialog(
                            type: XGUIDialogType.确认,
                            windowtitle: "XHud - Tmp文字组件消息",
                            title: "文字样式粘贴",
                            msg: $"已从 xData 中获取文字样式并应用到当前文字组件！",
                            ok: "明白",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                    }
                });
                menu.AddSeparator("");
                menu.AddDisabledItem(new GUIContent("文字样式库"));
                menu.AddItem(new GUIContent("X (识别)"), false, () =>
                {
                    XHud_LibraryArg_TextStyle info = JsonUtility.FromJson<XHud_LibraryArg_TextStyle>(XGUI.x_Editor_Data_Get_With_String("xData_XHud_TextStyle"));

                    if (Targets_Selected())
                    {
                        List<XGUIDialogListDatas> datas = new List<XGUIDialogListDatas>();

                        for (int i = 0; i < SelectedObjects.Length; i++)
                        {
                            XGUIDialogListDatas data = new XGUIDialogListDatas();

                            if (info.Type != xHud_TextType.TmpText)
                            {
                                XGUI.dialog(
                                   type: XGUIDialogType.帮助,
                                   windowtitle: "XHud - Tmp文字组件消息",
                                   title: "文字样式识别",
                                   msg: $"从 XData 中识别的文字样式似乎并不是用于 {SelectedObjects[i].name} ( {SelectedObjects[i].Indicator} ) 的文字组件的规范样式！请检查您复制的样式类型是否和当前组件一致？",
                                   ok: "明白",
                                   PrimaryIndex: 0,
                                   usemodal: true,
                                   themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));

                                data.Title = info.Name;
                                data.SubTitle = "文字样式类型未能匹配到";
                                data.Message = $"{SelectedObjects[i].name} ( {SelectedObjects[i].Indicator} )";

                                datas.Add(data);
                                continue;
                            }
                            else
                            {
                                // 使用 Undo.RecordObject 来记录对目标对象的修改
                                Undo.RecordObject(SelectedObjects[i], "Paste Style Name");

                                SelectedObjects[i].TextStyleInfo.CopyData_Ignored_LibraryToggle(info);

                                data.Title = info.Name;
                                data.SubTitle = "文字样式已应用到";
                                data.Message = $"{SelectedObjects[i].name} ( {SelectedObjects[i].Indicator} )";
                                datas.Add(data);
                            }
                        }

                        XGUI.dialog_listview(
                            datas: datas.ToArray(),
                            type: XGUIDialogType.确认,
                            windowtitle: "XHud - Tmp文字组件消息",
                            title: "文字样式识别",
                            msg: $"已从 xData 中识别到文字样式并应用到如下文字组件中！",
                            ok: "明白",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                    }
                    else
                    {
                        if (info.Type != xHud_TextType.TmpText)
                        {
                            XGUI.dialog(
                            type: XGUIDialogType.帮助,
                            windowtitle: "XHud - Tmp文字组件消息",
                            title: "文字样式识别",
                            msg: $"从 xData 中识别的文字样式似乎并不是用于当前文字组件的规范样式！请检查您复制的样式类型是否和当前组件一致？",
                            ok: "明白",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                            return;
                        }

                        // 使用 Undo.RecordObject 来记录对目标对象的修改
                        Undo.RecordObject(BaseScript, "Paste Style Name");

                        BaseScript.TextStyleInfo.CopyData_Ignored_LibraryToggle(info);

                        XGUI.dialog(
                            type: XGUIDialogType.确认,
                            windowtitle: "XHud - Tmp文字组件消息",
                            title: "文字样式识别",
                            msg: $"已从 xData 中识别到文字样式并应用到当前文字组件！",
                            ok: "明白",
                            PrimaryIndex: 0,
                            usemodal: true,
                            themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));
                    }
                });
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Q (折叠编组)"), false, () =>
                {
                    if (Targets_Selected())
                    {
                        for (int i = 0; i < SelectedObjects.Length; i++)
                        {
                            SelectedObjects[i].GroupFold(true);
                        }
                    }
                    else
                    {
                        BaseScript.GroupFold(true);
                    }
                });
                menu.AddItem(new GUIContent("W (展开编组)"), false, () =>
                {
                    if (Targets_Selected())
                    {
                        for (int i = 0; i < SelectedObjects.Length; i++)
                        {
                            SelectedObjects[i].GroupFold(false);
                        }
                    }
                    else
                    {
                        BaseScript.GroupFold(false);
                    }
                });
                menu.AddSeparator("");
                menu.AddDisabledItem(new GUIContent("同步开关"));
                menu.AddItem(new GUIContent("S (全部同步)"), false, () =>
                {
                    sp_LibStyle_Effect_align.boolValue = true;
                    sp_LibStyle_Effect_font.boolValue = true;
                    sp_LibStyle_Effect_style.boolValue = true;
                    sp_LibStyle_Effect_size.boolValue = true;
                    sp_LibStyle_Effect_color.boolValue = true;
                    sp_LibStyle_Effect_rich.boolValue = true;
                    sp_LibStyle_Effect_wrap.boolValue = true;
                    sp_LibStyle_Effect_wrapratio.boolValue = true;
                    sp_LibStyle_Effect_overflow.boolValue = true;
                    sp_LibStyle_Effect_margin.boolValue = true;
                    sp_LibStyle_Effect_space.boolValue = true;
                    sp_LibStyle_Effect_gradientcolor.boolValue = true;
                    sp_LibStyle_Effect_autosizing.boolValue = true;
                    sp_LibStyle_Effect_raycast.boolValue = true;
                    sp_LibStyle_Effect_maskable.boolValue = true;

                    sp_LibStyle_Effect_align.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_font.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_style.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_size.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_color.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_rich.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_wrap.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_wrapratio.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_overflow.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_margin.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_space.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_gradientcolor.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_autosizing.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_raycast.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_maskable.serializedObject.ApplyModifiedProperties();
                });
                menu.AddItem(new GUIContent("D (全部关闭)"), false, () =>
                {
                    sp_LibStyle_Effect_align.boolValue = false;
                    sp_LibStyle_Effect_font.boolValue = false;
                    sp_LibStyle_Effect_style.boolValue = false;
                    sp_LibStyle_Effect_size.boolValue = false;
                    sp_LibStyle_Effect_color.boolValue = false;
                    sp_LibStyle_Effect_rich.boolValue = false;
                    sp_LibStyle_Effect_wrap.boolValue = false;
                    sp_LibStyle_Effect_wrapratio.boolValue = false;
                    sp_LibStyle_Effect_overflow.boolValue = false;
                    sp_LibStyle_Effect_margin.boolValue = false;
                    sp_LibStyle_Effect_space.boolValue = false;
                    sp_LibStyle_Effect_gradientcolor.boolValue = false;
                    sp_LibStyle_Effect_autosizing.boolValue = false;
                    sp_LibStyle_Effect_raycast.boolValue = false;
                    sp_LibStyle_Effect_maskable.boolValue = false;

                    sp_LibStyle_Effect_align.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_font.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_style.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_size.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_color.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_rich.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_wrap.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_wrapratio.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_overflow.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_margin.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_space.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_gradientcolor.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_autosizing.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_raycast.serializedObject.ApplyModifiedProperties();
                    sp_LibStyle_Effect_maskable.serializedObject.ApplyModifiedProperties();
                });
                // 显示右键菜单
                menu.ShowAsContext();

                e.Use(); // 标记事件已被处理，防止其他操作处理该事件
            }
            #endregion

            #region 源脚本
            BaseScript.fold_based = XGUI.layout_group_start(
                type: XGUIContainerType.Vertical,
                bg_fill: XGUIGroup.缺口纯色边框,
                bg_color: XGUIColor.亮白,
                bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.group),
                title: "源脚本",
                title_size: XGUIFontSize.M,
                title_text_color: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                title_clipping: TextClipping.Clip,
                padding: new RectOffset(10, 10, 15, 15),
                foldout: BaseScript.fold_based);

            if (!BaseScript.fold_based)
            {
                DrawDefaultInspector();
            }
            XGUI.layout_group_end(type: XGUIContainerType.Vertical);
            #endregion

            serializedObject.ApplyModifiedProperties();
        }
        #endregion

        #region 工具 - 渐变色
        /// <summary>
        /// 创建渐变贴图的方法。
        /// <para/>
        /// 根据渐变模式与翻转设置，生成一张用于预览的渐变色纹理。
        /// </summary>
        /// <param name="mode">渐变模式</param>
        /// <param name="invert">是否翻转</param>
        /// <param name="col_start">主色</param>
        /// <param name="col_end">副色</param>
        /// <param name="col_start_cor">次色</param>
        /// <param name="col_end_cor">尾色</param>
        private void CreateGradientTexture(ColorMode mode, bool invert, Color col_start, Color col_end, Color col_start_cor, Color col_end_cor)
        {
            // 创建一个新的Texture2D对象
            GradientTexture = new Texture2D(gra_TexWidth, gra_TexHeight);
            Color pixelColor = Color.white;
            Color verticalBlend = Color.white;
            Color horizontalBlend = Color.white;
            Color colorBlend = Color.white;

            switch (mode)
            {
                case ColorMode.Single:
                    for (int x = 0; x < gra_TexWidth; x++)
                    {
                        for (int y = 0; y < gra_TexHeight; y++)
                        {
                            pixelColor = col_start;
                            GradientTexture.SetPixel(x, y, pixelColor);
                        }
                    }
                    break;
                case ColorMode.HorizontalGradient:
                    // 遍历每个像素并设置颜色
                    for (int x = 0; x < gra_TexWidth; x++)
                    {
                        for (int y = 0; y < gra_TexHeight; y++)
                        {
                            // 计算水平渐变的插值
                            float gradientValue = (float)x / (gra_TexWidth - 1);

                            if (invert)
                                pixelColor = Color.Lerp(col_end, col_start, gradientValue);
                            else
                                pixelColor = Color.Lerp(col_start, col_end, gradientValue);

                            GradientTexture.SetPixel(x, y, pixelColor);
                        }
                    }
                    break;
                case ColorMode.VerticalGradient:
                    for (int y = 0; y < gra_TexHeight; y++)
                    {
                        for (int x = 0; x < gra_TexWidth; x++)
                        {
                            // 计算垂直渐变的插值
                            float gradientValue = (float)y / (gra_TexHeight - 1);
                            if (invert)
                                pixelColor = Color.Lerp(col_end, col_start, gradientValue);
                            else
                                pixelColor = Color.Lerp(col_start, col_end, gradientValue);
                            GradientTexture.SetPixel(x, y, pixelColor);
                        }
                    }
                    break;
                case ColorMode.FourCornersGradient:
                    for (int y = 0; y < gra_TexHeight; y++)
                    {
                        for (int x = 0; x < gra_TexWidth; x++)
                        {
                            // 归一化坐标
                            float u = x / (float)(gra_TexWidth - 1);
                            float v = y / (float)(gra_TexHeight - 1);

                            // 计算四个角的颜色插值
                            if (invert)
                            {
                                verticalBlend = Color.Lerp(col_end, col_start, u);        // 顶部水平插值
                                horizontalBlend = Color.Lerp(col_end_cor, col_start_cor, u); // 底部水平插值
                                colorBlend = Color.Lerp(horizontalBlend, verticalBlend, v);       // 垂直插值  
                            }
                            else
                            {
                                verticalBlend = Color.Lerp(col_start, col_end, u);        // 顶部水平插值
                                horizontalBlend = Color.Lerp(col_start_cor, col_end_cor, u); // 底部水平插值
                                colorBlend = Color.Lerp(horizontalBlend, verticalBlend, v);       // 垂直插值
                            }
                            // 计算当前像素到中心的距离（归一化）
                            float distanceToCenter = Mathf.Sqrt((u - 0.5f) * (u - 0.5f) + (v - 0.5f) * (v - 0.5f));
                            float normalizedDistance = 1.0f - (distanceToCenter * 2); // 从外到内，值从1到0

                            // 根据距离中心的距离调整颜色
                            Color finalColor = Color.Lerp(colorBlend, Color.white, normalizedDistance); // 中心颜色为白色

                            GradientTexture.SetPixel(x, y, finalColor);
                        }
                    }
                    break;
            }

            // 应用修改并压缩贴图
            GradientTexture.Apply();
        }
        #endregion

        #region 工具 - 刷新文字样式
        /// <summary>
        /// 根据文字样式名称索引刷新文字样式。
        /// <para/>
        /// 单选时刷新当前组件；多选时逐个刷新所有选中组件。
        /// 若指定名称在样式库中不存在，则弹窗询问是否使用首个样式。
        /// </summary>
        private void UpdateTextStyle_WithTarget()
        {
            XHud_LibraryArg_TextStyle s_info = null;
            if (Targets_Selected())
            {
                var s = targets;
                foreach (var t in s)
                {
                    XHud_Module_TmpText tt = (XHud_Module_TmpText)t;

                    if (HudManager.Hud_TextStyleLibrary.TextStyle_Library_NameIsValid(tt.StyleName))
                    {
                        s_info = HudManager.Hud_TextStyleLibrary.TextStyle_Library_GetTextStyleInfo(tt.StyleName, xHud_TextType.TmpText);
                    }
                    else
                    {
                        string res = XGUI.dialog(
                             type: XGUIDialogType.警告,
                             windowtitle: "XHud - Tmp文字组件消息",
                             title: "刷新文字库样式",
                             msg: $"似乎 {tt.name} ( {tt.Indicator} ) 文字组件的样式库名称并不在文字样式库中存在，请检查文字样式库名称是否合法，或者可以快速选择文字样式库默认首个样式。是否要选择首个样式？",
                             ok: "暂不",
                             cancel: "选择",
                             PrimaryIndex: 0,
                             usemodal: true,
                             themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));

                        if (res == "选择")
                        {
                            s_info = HudManager.Hud_TextStyleLibrary.TextStyle_Library_GetFirstStyleInfo_With_Type(xHud_TextType.TmpText);

                            // 使用 Undo.RecordObject 来记录对目标对象的修改
                            Undo.RecordObject(tt, "Change TextStyle Name");

                            tt.StyleName = s_info.Name;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    tt.TextStyleInfo.CopyData_Ignored_LibraryToggle(s_info);
                }
            }
            else
            {
                if (HudManager.Hud_TextStyleLibrary.TextStyle_Library_NameIsValid(sp_StyleName.stringValue))
                    s_info = HudManager.Hud_TextStyleLibrary.TextStyle_Library_GetTextStyleInfo(sp_StyleName.stringValue, xHud_TextType.TmpText);
                else
                {
                    string res = XGUI.dialog(
                      type: XGUIDialogType.警告,
                      windowtitle: "XHud - Tmp文字组件消息",
                      title: "刷新文字库样式",
                      msg: $"似乎您指定的样式库名称并不在文字样式库中存在，请检查文字样式库名称是否合法，或者可以快速选择文字样式库默认首个样式。是否要选择首个样式？",
                      ok: "暂不",
                      cancel: "选择",
                      PrimaryIndex: 0,
                      usemodal: true,
                      themecolor: XHud_Dashboard.themes_get_color(ThemeColorType.primary));

                    if (res == "选择")
                    {
                        s_info = HudManager.Hud_TextStyleLibrary.TextStyle_Library_GetFirstStyleInfo_With_Type(xHud_TextType.TmpText);

                        // 使用 Undo.RecordObject 来记录对目标对象的修改
                        Undo.RecordObject(sp_StyleName.serializedObject.targetObject, "Change TextStyle Name");

                        sp_StyleName.stringValue = s_info.Name;
                        sp_StyleName.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                    }
                    else
                    {
                        return;
                    }
                }

                BaseScript.TextStyleInfo.CopyData_Ignored_LibraryToggle(s_info);
                return;
            }
        }
        #endregion

        #region 工具 - 打开字体样式添加器
        /// <summary>
        /// 打开字体样式采集器窗口。
        /// <para/>
        /// 将当前组件的样式信息与目标样式库传入采集器，用于保存为新的样式模版。
        /// </summary>
        /// <param name="info">文字样式数据</param>
        public void Open_Hud_Library_TextStyle_Setter(XHud_LibraryArg_TextStyle info)
        {
            Editor_XHud_LibrarySetTool_TextStyle window = EditorWindow.GetWindow<Editor_XHud_LibrarySetTool_TextStyle>(true);

            window.titleContent = new GUIContent("XHud - 文字样式采集器");
            XGUI.CenterEditorWindow(new Vector2Int(348, info.Type == xHud_TextType.Text ? 820 : 800), window);

            window.SetComponent(BaseScript);
            window.SetStyle(info);
            window.SetOriginStyle(info);
            window.SetLibrarySetterMode(LibrarySetterMode.添加到库);
            window.SetButtonText("添加", "取消");
            window.SetTitle("文字样式采集器");
            //window.ShowModal();
            window.Show();
        }
        #endregion

        #region TMP InternalMethods
        /// <summary>
        /// Raycast Target 标签。
        /// </summary>
        static readonly GUIContent k_RaycastTargetLabel = new GUIContent("Raycast Target", "Whether the text blocks raycasts from the Graphic Raycaster.");
        /// <summary>
        /// Maskable 标签。
        /// </summary>
        static readonly GUIContent k_MaskableLabel = new GUIContent("Maskable", "Determines if the text object will be affected by UI Mask.");

        /// <summary>
        /// Raycast Target 序列化属性。
        /// </summary>
        private SerializedProperty sp_RaycastTargetProp;
        /// <summary>
        /// Maskable 序列化属性。
        /// </summary>
        private SerializedProperty sp_MaskableProp;

        /// <summary>
        /// 绘制额外设置面板。
        /// </summary>
        protected override void DrawExtraSettings()
        {
            Rect rect = EditorGUILayout.GetControlRect(false, 24);

            if (GUI.Button(rect, new GUIContent("<b>Extra Settings</b>"), TMP_UIStyleManager.sectionHeader))
                Foldout.extraSettings = !Foldout.extraSettings;

            GUI.Label(rect, (Foldout.extraSettings ? k_UiStateLabel[0] : k_UiStateLabel[1]), TMP_UIStyleManager.rightLabel);
            if (Foldout.extraSettings)
            {
                //EditorGUI.indentLevel += 1;

                DrawMargins();

                DrawGeometrySorting();

                DrawIsTextObjectScaleStatic();

                DrawRichText();

                DrawRaycastTarget();

                DrawMaskable();

                DrawParsing();

                DrawSpriteAsset();

                DrawStyleSheet();

                //DrawKerning();

                DrawPadding();

                //EditorGUI.indentLevel -= 1;

            }
        }
        /// <summary>
        /// 绘制 Raycast Target 设置项。
        /// </summary>
        protected void DrawRaycastTarget()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(sp_RaycastTargetProp, k_RaycastTargetLabel);
            if (EditorGUI.EndChangeCheck())
            {
                // Change needs to propagate to the child sub objects.
                Graphic[] graphicComponents = m_TextComponent.GetComponentsInChildren<Graphic>();
                for (int i = 1; i < graphicComponents.Length; i++)
                    graphicComponents[i].raycastTarget = sp_RaycastTargetProp.boolValue;

                m_HavePropertiesChanged = true;
            }
        }
        /// <summary>
        /// 绘制 Maskable 设置项。
        /// </summary>
        protected void DrawMaskable()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(sp_MaskableProp, k_MaskableLabel);
            if (EditorGUI.EndChangeCheck())
            {
                m_TextComponent.maskable = sp_MaskableProp.boolValue;

                // Change needs to propagate to the child sub objects.
                MaskableGraphic[] maskableGraphics = m_TextComponent.GetComponentsInChildren<MaskableGraphic>();
                for (int i = 1; i < maskableGraphics.Length; i++)
                    maskableGraphics[i].maskable = sp_MaskableProp.boolValue;

                m_HavePropertiesChanged = true;
            }
        }
        /// <summary>
        /// 判断是否选中了不同类型的对象。
        /// <para/>
        /// 当选中的多个对象中任意一个不包含 <see cref="TextMeshProUGUI"/> 时返回 true。
        /// </summary>
        /// <returns>true 表示混合选择</returns>
        protected override bool IsMixSelectionTypes()
        {
            GameObject[] objects = Selection.gameObjects;
            if (objects.Length > 1)
            {
                for (int i = 0; i < objects.Length; i++)
                {
                    if (objects[i].GetComponent<TextMeshProUGUI>() == null)
                        return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 撤销 / 重做回调。
        /// <para/>
        /// 在撤销事件变化时通知 TextMeshPro 属性变更事件。
        /// </summary>
        protected override void OnUndoRedo()
        {
            int undoEventId = Undo.GetCurrentGroup();
            int lastUndoEventId = s_EventId;

            if (undoEventId != lastUndoEventId)
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    //sp_DebugMode.Log("Undo & Redo Performed detected in Editor Panel. Event ID:" + Undo.GetCurrentGroup());
                    TMPro_EventManager.ON_TEXTMESHPRO_UGUI_PROPERTY_CHANGED(true, targets[i] as TextMeshProUGUI);
                    s_EventId = undoEventId;
                }
            }
        }
        #endregion

        #region 绘制控件
        /// <summary>
        /// 通用方法：绘制带标题的开关控件。
        /// <para/>
        /// 封装 <see cref="XGUI.layout_toggle"/>，统一标题宽度、字体、混合选项样式。
        /// </summary>
        private void DrawLabeledToggle(string title = null, string tooltip = null, SerializedProperty prop = null, float width = 0, XGUIToggleStyle style = XGUIToggleStyle.实体, Color color_bg_on = default, Color color_bg_off = default, Color color_on = default, Color color_off = default, string[] options = null, Action<bool> act_on_changed = null)
        {
            XGUI.layout_toggle(
                title: title,
                tooltip: tooltip,
                title_size: XGUIFontSize.M,
                title_font_style: FontStyle.Normal,
                title_padding: new RectOffset(5, 10, 0, 0),
                title_width: width,
                prop: prop,
                tog_style: style,
                tog_padding: new RectOffset(0, 9, 0, 0),
                tog_margin: new RectOffset(0, 0, 0, 5),
                tog_mixed_options: options,
                tog_mixed_text_size: XGUIFontSize.M,
                tog_mixed_text_color: Color.black,
                tog_mixed_text_padding: new RectOffset(10, 10, 0, 0),
                tog_mixed_text_anchor: TextAnchor.MiddleCenter,
                tog_mixed_font_style: FontStyle.Normal,
                tog_bg_off_color: color_bg_off,
                tog_bg_on_color: color_bg_on,
                tog_handler_off_color: color_off,
                tog_handler_on_color: color_on,
                tog_mixed_bg_color_gui: XHud_Dashboard.themes_get_color(ThemeColorType.primary),
                act_on_changed: act_on_changed);
        }
        #endregion
    }
}