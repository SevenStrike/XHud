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
namespace SevenStrikeModules.XHud
{
    using SevenStrikeModules.XGUI.Runtime;
    using SevenStrikeModules.XHud.Enums;
    using TMPro;
    using UnityEngine;
    using UnityEngine.Events;

    #region 数据模型
    /// <summary>
    /// TmpText 间距值数据容器。
    /// <para/>
    /// 用于集中管理字符间距 / 单词间距 / 行高 / 段落间距四类间距参数。
    /// </summary>
    [System.Serializable]
    public class XHud_TmpTextSpacingValue
    {
        /// <summary>
        /// 字符间距。
        /// </summary>
        public float space_Character;
        /// <summary>
        /// 单词间距。
        /// </summary>
        public float space_Word;
        /// <summary>
        /// 行高。
        /// </summary>
        public float space_Line;
        /// <summary>
        /// 段落间距。
        /// </summary>
        public float space_Paragraph;
        /// <summary>
        /// 构造间距值容器。
        /// </summary>
        /// <param name="value_cha">字符间距</param>
        /// <param name="value_word">单词间距</param>
        /// <param name="value_line">行高</param>
        /// <param name="value_para">段落间距</param>
        public XHud_TmpTextSpacingValue(float value_cha, float value_word, float value_line, float value_para)
        {
            space_Character = value_cha;
            space_Word = value_word;
            space_Line = value_line;
            space_Paragraph = value_para;
        }
    }
    #endregion

    public class XHud_Module_TmpText : TextMeshProUGUI
    {
        #region 字段 - 基础参数
        /// <summary>
        /// 标识名称。
        /// </summary>
        [SerializeField]
        public string indicator;
        /// <summary>
        /// 文字字体样式索引名称。
        /// </summary>
        [SerializeField]
        public string style_name;
        /// <summary>
        /// 是否使用实时同步字体样式库。
        /// </summary>
        [SerializeField]
        public bool style_library_synching;
        /// <summary>
        /// 判断内容是否改变。
        /// </summary>
        [SerializeField]
        private string previous_text;
        /// <summary>
        /// 内容已改变。
        /// </summary>
        [SerializeField]
        private bool text_is_changed;
        /// <summary>
        /// 事件内容改变。
        /// </summary>
        [SerializeField]
        public UnityAction<string> act_on_text_changed;
        /// <summary>
        /// 同步全局字体尺寸。
        /// </summary>
        [SerializeField]
        public bool sync_global_font_size = true;
        /// <summary>
        /// 指定文字颜色接管的拥有者。
        /// </summary>
        [SerializeField]
        public TextColorOwner text_color_owner;
        #endregion

        #region 字段 - 样式
        /// <summary>
        /// 字体样式 - 本地。
        /// </summary>
        [SerializeField]
        public XHud_LibraryArg_TextStyle text_style_info = new XHud_LibraryArg_TextStyle();
        /// <summary>
        /// 字体样式 - 库。
        /// </summary>
        [SerializeField]
        public XHud_LibraryArg_TextStyle text_style_info_library = new XHud_LibraryArg_TextStyle();
        #endregion

        #region 字段 - 折叠状态
        /// <summary>
        /// 参数面板折叠状态。
        /// </summary>
        public bool
            fold_param = true,
            fold_gradient = true,
            fold_option = true,
            fold_based = true,
            fold_sync = true;
        /// <summary>
        /// 统一设置所有折叠状态。
        /// </summary>
        /// <param name="state">true = 折叠；false = 展开</param>
        public void GroupFold(bool state)
        {
            fold_param = state;
            fold_option = state;
            fold_based = state;
            fold_sync = state;
            fold_gradient = state;
        }
        /// <summary>
        /// 分别设置各面板折叠状态。
        /// </summary>
        /// <param name="param">参数面板</param>
        /// <param name="option">选项面板</param>
        /// <param name="based">源脚本面板</param>
        /// <param name="sync">库同步项面板</param>
        public void GroupFold(bool param = true, bool option = true, bool based = true, bool sync = true, bool gradient = true)
        {
            fold_param = param;
            fold_option = option;
            fold_based = based;
            fold_sync = sync;
            fold_gradient = gradient;
        }
        #endregion

        #region 生命周期
        /// <summary>
        /// Unity 启用回调。
        /// <para/>
        /// 在非运行时获取 XHud 管理器，供样式同步使用。
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
        }
        /// <summary>
        /// Unity 启动回调。
        /// <para/>
        /// 运行时记录初始文字内容，用于变更检测。
        /// </summary>
        protected override void Start()
        {
            base.Start();
            if (Application.isPlaying)
            {
                previous_text = text;
            }
        }
        /// <summary>
        /// Unity 每帧回调。
        /// <para/>
        /// 执行内容变更检测、运行时样式刷新、全局字体尺寸同步与渐变色更新。
        /// </summary>
        private void Update()
        {
            //-------------------内容变动检测
            tmp_Detect_Content();
            //-------------------运行时样式刷新
            tmp_Update_Style_Runtime();
            //--------------------同步全局字体尺寸
            tmp_Set_SyncGlobalFontSize(sync_global_font_size);
            //--------------------更新渐变色
            tmp_Update_GradientColor();
        }
        /// <summary>
        /// 更新材质回调。
        /// <para/>
        /// 直接调用基类实现。
        /// </summary>
        protected override void UpdateMaterial()
        {
            base.UpdateMaterial();
        }
        #endregion

        #region 实时刷新类
        /// <summary>
        /// 内容改变检测。
        /// <para/>
        /// 对比 <see cref="previous_text"/> 与当前 <c>text</c>，
        /// 若发生变化则触发 <see cref="act_on_text_changed"/>。
        /// </summary>
        private void tmp_Detect_Content()
        {
            if (previous_text != text)
            {
                if (!text_is_changed)
                {
                    text_is_changed = true;
                    previous_text = text;
                    if (act_on_text_changed != null)
                        act_on_text_changed(text);
                }
            }
            else
            {
                text_is_changed = false;
            }
        }
        /// <summary>
        /// 从字体样式库中切换样式并刷新文字效果。
        /// <para/>
        /// 若 <see cref="style_library_synching"/> 开启，则从样式库读取对应样式，
        /// 按各 <c>LibStyle_Effect_*</c> 开关决定哪些属性同步到本地样式；
        /// 最后调用 <see cref="tmp_Syncing_Style"/> 将本地样式写入 TmpText 组件。
        /// </summary>
        /// <param name="StyleName">样式名称</param>
        public void tmp_Update_Style_Runtime()
        {
            #region 如果同步库样式
            if (style_library_synching)
            {
                XHud_Manager mgr = XHud_Dashboard.get_manager();

                #region 样式集合为空判断
                if (mgr == null)
                    return;
                if (mgr.library_textstyles == null)
                    return;
                if (mgr.library_textstyles.TextStyle_Library_IsEmpty())
                    return;
                #endregion

                #region 从库中获取字体样式资源到 TextStyleInfo
                if (Application.isPlaying)
                {
                    text_style_info_library = XHud_Manager.instance.library_textstyles.TextStyle_Library_GetTextStyleInfo(style_name);
                }
                else
                {
                    if (mgr != null && mgr.library_textstyles != null)
                        text_style_info_library = mgr.library_textstyles.TextStyle_Library_GetTextStyleInfo(style_name);
                }
                #endregion

                #region 将TextStyleInfo同步到文字效果上
                //-----如果开启库同步 - RichText
                if (text_style_info.lib_style_effect_rich)
                {
                    text_style_info.tmp_rich = text_style_info_library.tmp_rich;
                }

                //-----如果开启库同步 - 包裹文字
                if (text_style_info.lib_style_effect_wrap)
                {
                    text_style_info.tmp_contentwrap = text_style_info_library.tmp_contentwrap;
                }

                //-----如果开启库同步 - 溢出文字
                if (text_style_info.lib_style_effect_overflow)
                {
                    text_style_info.tmp_overflow = text_style_info_library.tmp_overflow;
                }

                //-----如果开启库同步 - 字体样式
                if (text_style_info.lib_style_effect_style)
                {
                    text_style_info.tmp_style = text_style_info_library.tmp_style;
                }

                //-----如果开启库同步 - 对齐锚点
                if (text_style_info.lib_style_effect_align)
                {
                    text_style_info.tmp_anchor = text_style_info_library.tmp_anchor;
                }

                //-----如果开启库同步 - 字体
                if (text_style_info.lib_style_effect_font)
                {
                    text_style_info.tmp_font = text_style_info_library.tmp_font;
                }

                //-----如果开启库同步 - 尺寸_Size
                if (text_style_info.lib_style_effect_size)
                {
                    text_style_info.tmp_size = text_style_info_library.tmp_size;
                }

                //-----如果开启库同步 - 自动尺寸
                if (text_style_info.lib_style_effect_autosizing)
                {
                    text_style_info.tmp_enable_auto_sizing = text_style_info_library.tmp_enable_auto_sizing;
                    text_style_info.tmp_font_size_min = text_style_info_library.tmp_font_size_min;
                    text_style_info.tmp_font_size_max = text_style_info_library.tmp_font_size_max;
                }

                //-----如果开启库同步 - 间距
                if (text_style_info.lib_style_effect_space)
                {
                    text_style_info.tmp_space_character = text_style_info_library.tmp_space_character;
                    text_style_info.tmp_space_word = text_style_info_library.tmp_space_word;
                    text_style_info.tmp_space_lineheight = text_style_info_library.tmp_space_lineheight;
                    text_style_info.tmp_space_paragraph = text_style_info_library.tmp_space_paragraph;
                }

                //-----如果开启库同步 - 包裹对齐
                if (text_style_info.lib_style_effect_wrapratio)
                {
                    text_style_info.tmp_wrapping_ratios = text_style_info_library.tmp_wrapping_ratios;
                }

                //-----如果开启库同步 - 边距
                if (text_style_info.lib_style_effect_margin)
                {
                    text_style_info.tmp_content_margin = text_style_info_library.tmp_content_margin;
                }

                //-----如果开启库同步 - 颜色_Color
                if (text_style_info.lib_style_effect_color)
                {
                    text_style_info.tmp_color = text_style_info_library.tmp_color;
                }

                //-----如果开启库同步 - 渐变色
                if (text_style_info.lib_style_effect_gradientcolor)
                {
                    text_style_info.gra_a = text_style_info_library.gra_a;
                    text_style_info.gra_b = text_style_info_library.gra_b;
                    text_style_info.gra_c = text_style_info_library.gra_c;
                    text_style_info.gra_d = text_style_info_library.gra_d;
                    text_style_info.gra_color_mode = text_style_info_library.gra_color_mode;
                    text_style_info.gra_color_mode_name = text_style_info_library.gra_color_mode_name;
                    text_style_info.gra_invert = text_style_info_library.gra_invert;
                    text_style_info.gra_used = text_style_info_library.gra_used;
                }

                //-----如果开启库同步 - 射线检测
                if (text_style_info.lib_style_effect_raycast)
                {
                    text_style_info.raycast = text_style_info_library.raycast;
                }

                //-----如果开启库同步 - 遮罩                
                if (text_style_info.lib_style_effect_maskable)
                {
                    text_style_info.maskable = text_style_info_library.maskable;
                }
                #endregion
            }
            #endregion

            #region 刷新字体样式
            tmp_Syncing_Style();
            #endregion
        }
        /// <summary>
        /// 将 <see cref="text_style_info"/> 的值同步给 TmpText 组件。
        /// <para/>
        /// 涵盖：富文本 / 包裹 / 溢出 / 字体样式 / 对齐 / 字体 /
        /// 自适应尺寸 / 全局尺寸倍率 / 间距 / 包裹对齐 / 边距 / 颜色 /
        /// 渐变色 / 射线检测 / 遮罩。
        /// </para>
        /// 颜色仅在 <see cref="text_color_owner"/> 不为 Tween 时同步。
        /// </summary>
        public void tmp_Syncing_Style()
        {
            //--同步 - RichText
            if (richText != text_style_info.tmp_rich)
            {
                richText = text_style_info.tmp_rich;
            }

            //--同步 - 包裹文字
            if (textWrappingMode != text_style_info.tmp_contentwrap)
            {
                textWrappingMode = text_style_info.tmp_contentwrap;
            }

            //--同步 - 溢出文字
            if (overflowMode != text_style_info.tmp_overflow)
            {
                overflowMode = text_style_info.tmp_overflow;
            }

            //--同步 - 字体样式
            if (fontStyle != text_style_info.tmp_style)
            {
                fontStyle = text_style_info.tmp_style;
            }

            //--同步 - 对齐锚点
            switch (text_style_info.tmp_anchor)
            {
                case TmpContentAnchor.顶部:
                    alignment = TextAlignmentOptions.Top;
                    break;
                case TmpContentAnchor.顶部靠左:
                    alignment = TextAlignmentOptions.TopLeft;
                    break;
                case TmpContentAnchor.顶部靠右:
                    alignment = TextAlignmentOptions.TopRight;
                    break;
                case TmpContentAnchor.顶部填充:
                    alignment = TextAlignmentOptions.TopJustified;
                    break;
                case TmpContentAnchor.顶部均分:
                    alignment = TextAlignmentOptions.TopFlush;
                    break;
                case TmpContentAnchor.顶部基线:
                    alignment = TextAlignmentOptions.TopGeoAligned;
                    break;
                case TmpContentAnchor.底部:
                    alignment = TextAlignmentOptions.Bottom;
                    break;
                case TmpContentAnchor.底部靠左:
                    alignment = TextAlignmentOptions.BottomLeft;
                    break;
                case TmpContentAnchor.底部靠右:
                    alignment = TextAlignmentOptions.BottomRight;
                    break;
                case TmpContentAnchor.底部左右填充:
                    alignment = TextAlignmentOptions.BottomJustified;
                    break;
                case TmpContentAnchor.底部左右均分:
                    alignment = TextAlignmentOptions.BottomFlush;
                    break;
                case TmpContentAnchor.底部基线:
                    alignment = TextAlignmentOptions.BottomGeoAligned;
                    break;
                case TmpContentAnchor.左:
                    alignment = TextAlignmentOptions.Left;
                    break;
                case TmpContentAnchor.右:
                    alignment = TextAlignmentOptions.Right;
                    break;
                case TmpContentAnchor.中心:
                    alignment = TextAlignmentOptions.Center;
                    break;
                case TmpContentAnchor.中心填充:
                    alignment = TextAlignmentOptions.Justified;
                    break;
                case TmpContentAnchor.中心左右均分:
                    alignment = TextAlignmentOptions.Flush;
                    break;
                case TmpContentAnchor.基线:
                    alignment = TextAlignmentOptions.Baseline;
                    break;
                case TmpContentAnchor.基线靠左:
                    alignment = TextAlignmentOptions.BaselineLeft;
                    break;
                case TmpContentAnchor.基线靠右:
                    alignment = TextAlignmentOptions.BaselineRight;
                    break;
                case TmpContentAnchor.基线左右填充:
                    alignment = TextAlignmentOptions.BaselineJustified;
                    break;
                case TmpContentAnchor.基线左右均分:
                    alignment = TextAlignmentOptions.BaselineFlush;
                    break;
                case TmpContentAnchor.中线:
                    alignment = TextAlignmentOptions.Capline;
                    break;
                case TmpContentAnchor.中线靠左:
                    alignment = TextAlignmentOptions.CaplineLeft;
                    break;
                case TmpContentAnchor.中线靠右:
                    alignment = TextAlignmentOptions.CaplineRight;
                    break;
                case TmpContentAnchor.中线左右填充:
                    alignment = TextAlignmentOptions.CaplineJustified;
                    break;
                case TmpContentAnchor.中线左右均分:
                    alignment = TextAlignmentOptions.CaplineFlush;
                    break;
            }

            //--同步 - 字体
            if (text_style_info.tmp_font != null && text_style_info.tmp_font != font)
            {
                font = text_style_info.tmp_font;
            }

            //--同步 - 自动尺寸 - 开关
            if (enableAutoSizing != text_style_info.tmp_enable_auto_sizing)
            {
                enableAutoSizing = text_style_info.tmp_enable_auto_sizing;
            }

            //--同步 - 自动尺寸 - 逻辑赋值
            if (enableAutoSizing)
            {
                //--最小尺寸
                if (sync_global_font_size)
                {
                    if (!Application.isPlaying)
                    {
                        text_style_info.tmp_FontSizeMin_globalscaled = XHud_Dashboard.get_manager().ft_font_size_multiply * text_style_info.tmp_font_size_min;
                    }
                    else
                    {
                        text_style_info.tmp_FontSizeMin_globalscaled = XHud_Manager.instance.ft_font_size_multiply * text_style_info.tmp_font_size_min;
                    }

                    if (text_style_info.tmp_FontSizeMin_globalscaled != fontSizeMin)
                    {
                        fontSizeMin = text_style_info.tmp_FontSizeMin_globalscaled;
                    }
                }
                else
                {
                    if (fontSizeMin != text_style_info.tmp_font_size_min)
                    {
                        fontSizeMin = text_style_info.tmp_font_size_min;
                    }
                }

                //--最大尺寸
                if (sync_global_font_size)
                {
                    if (!Application.isPlaying)
                    {
                        text_style_info.tmp_FontSizeMax_globalscaled = XHud_Dashboard.get_manager().ft_font_size_multiply * text_style_info.tmp_font_size_max;
                    }
                    else
                    {
                        text_style_info.tmp_FontSizeMax_globalscaled = XHud_Manager.instance.ft_font_size_multiply * text_style_info.tmp_font_size_max;
                    }

                    if (text_style_info.tmp_FontSizeMax_globalscaled != fontSizeMax)
                    {
                        fontSizeMax = text_style_info.tmp_FontSizeMax_globalscaled;
                    }
                }
                else
                {
                    if (fontSizeMax != text_style_info.tmp_font_size_max)
                    {
                        fontSizeMax = text_style_info.tmp_font_size_max;
                    }
                }

                //--字符宽度百分比
                if (characterWidthAdjustment != text_style_info.tmp_char_width_max_adj)
                {
                    characterWidthAdjustment = text_style_info.tmp_char_width_max_adj;
                }

                //--最大行高
                if (lineSpacingAdjustment != text_style_info.tmp_line_spacing_max)
                {
                    lineSpacingAdjustment = text_style_info.tmp_line_spacing_max;
                }
            }
            //--同步 - 原生尺寸 - 逻辑赋值
            else
            {
                if (sync_global_font_size)
                {
                    if (!Application.isPlaying)
                    {
                        text_style_info.tmp_size_globalscaled = XHud_Dashboard.get_manager().ft_font_size_multiply * text_style_info.tmp_size;
                    }
                    else
                    {
                        text_style_info.tmp_size_globalscaled = XHud_Manager.instance.ft_font_size_multiply * text_style_info.tmp_size;
                    }

                    if (text_style_info.tmp_size_globalscaled != fontSize || m_fontSizeBase != m_fontSize)
                    {
                        fontSize = text_style_info.tmp_size_globalscaled;
                    }
                }
                else
                {
                    if (text_style_info.tmp_size != fontSize || m_fontSizeBase != m_fontSize)
                    {
                        fontSize = text_style_info.tmp_size;
                    }
                }
            }

            //--同步 - 间距 - 字符
            if (characterSpacing != text_style_info.tmp_space_character)
            {
                characterSpacing = text_style_info.tmp_space_character;
            }

            //--同步 - 间距 - 单词
            if (wordSpacing != text_style_info.tmp_space_word)
            {
                wordSpacing = text_style_info.tmp_space_word;
            }

            //--同步 - 间距 - 行高
            if (lineSpacing != text_style_info.tmp_space_lineheight)
            {
                lineSpacing = text_style_info.tmp_space_lineheight;
            }

            //--同步 - 间距 - 段落
            if (paragraphSpacing != text_style_info.tmp_space_paragraph)
            {
                paragraphSpacing = text_style_info.tmp_space_paragraph;
            }

            //--同步 - 包裹对齐
            if (wordWrappingRatios != text_style_info.tmp_wrapping_ratios)
            {
                wordWrappingRatios = text_style_info.tmp_wrapping_ratios;
            }

            //--同步 - 边距
            if (margin != text_style_info.tmp_content_margin)
            {
                margin = text_style_info.tmp_content_margin;
            }

            //--同步 - 颜色_Color
            if (text_color_owner != TextColorOwner.Tween)
            {
                if (color != text_style_info.tmp_color)
                {
                    color = text_style_info.tmp_color;
                }
            }

            //--同步 - 渐变色开关
            if (enableVertexGradient != text_style_info.gra_used)
            {
                enableVertexGradient = text_style_info.gra_used;
            }

            //--同步 - 渐变色模式
            if (m_colorMode != text_style_info.gra_color_mode)
            {
                m_colorMode = text_style_info.gra_color_mode;

                switch (text_style_info.gra_color_mode)
                {
                    case ColorMode.Single:
                        text_style_info.gra_color_mode_name = "单色";
                        break;
                    case ColorMode.HorizontalGradient:
                        text_style_info.gra_color_mode_name = "水平渐变";
                        break;
                    case ColorMode.VerticalGradient:
                        text_style_info.gra_color_mode_name = "垂直渐变";
                        break;
                    case ColorMode.FourCornersGradient:
                        text_style_info.gra_color_mode_name = "四角渐变";
                        break;
                }
            }

            //--同步 - 射线检测
            if (raycastTarget != text_style_info.raycast)
            {
                raycastTarget = text_style_info.raycast;
            }

            //--同步 - 遮罩
            if (maskable != text_style_info.maskable)
            {
                maskable = text_style_info.maskable;
            }
        }
        /// <summary>
        /// 从文字样式添加器反向更新文字样式。
        /// </summary>
        /// <param name="styleinfo">样式数据</param>
        public void tmp_Update_Style_LibrarySetter(XHud_LibraryArg_TextStyle styleinfo)
        {
            text_style_info = styleinfo;
        }
        /// <summary>
        /// 更新渐变色。
        /// <para/>
        /// 根据 <see cref="text_style_info"/> 中的渐变色模式与翻转设置，
        /// 将四个角的颜色写入 <see cref="TMP_Text.colorGradient"/>。
        /// </summary>
        private void tmp_Update_GradientColor()
        {
            VertexGradient cg = colorGradient;
            switch (text_style_info.gra_color_mode)
            {
                case ColorMode.Single:
                    cg.topLeft = text_style_info.gra_a;
                    cg.topRight = text_style_info.gra_a;
                    cg.bottomLeft = text_style_info.gra_a;
                    cg.bottomRight = text_style_info.gra_a;
                    break;
                case ColorMode.HorizontalGradient:
                    if (text_style_info.gra_invert)
                    {
                        cg.topLeft = text_style_info.gra_b;
                        cg.topRight = text_style_info.gra_a;
                        cg.bottomLeft = text_style_info.gra_b;
                        cg.bottomRight = text_style_info.gra_a;
                    }
                    else
                    {
                        cg.topLeft = text_style_info.gra_a;
                        cg.topRight = text_style_info.gra_b;
                        cg.bottomLeft = text_style_info.gra_a;
                        cg.bottomRight = text_style_info.gra_b;
                    }
                    break;
                case ColorMode.VerticalGradient:
                    if (text_style_info.gra_invert)
                    {
                        cg.topLeft = text_style_info.gra_a;
                        cg.topRight = text_style_info.gra_a;
                        cg.bottomLeft = text_style_info.gra_b;
                        cg.bottomRight = text_style_info.gra_b;
                    }
                    else
                    {
                        cg.topLeft = text_style_info.gra_b;
                        cg.topRight = text_style_info.gra_b;
                        cg.bottomLeft = text_style_info.gra_a;
                        cg.bottomRight = text_style_info.gra_a;
                    }
                    break;
                case ColorMode.FourCornersGradient:
                    if (text_style_info.gra_invert)
                    {
                        cg.topLeft = text_style_info.gra_b;
                        cg.topRight = text_style_info.gra_a;
                        cg.bottomLeft = text_style_info.gra_d;
                        cg.bottomRight = text_style_info.gra_c;
                    }
                    else
                    {
                        cg.topLeft = text_style_info.gra_a;
                        cg.topRight = text_style_info.gra_b;
                        cg.bottomLeft = text_style_info.gra_c;
                        cg.bottomRight = text_style_info.gra_d;
                    }
                    break;
            }
            colorGradient = cg;
        }
        #endregion

        #region 参数设置
        /// <summary>
        /// 设置全局尺寸影响可用性。
        /// </summary>
        /// <param name="state">true = 启用全局尺寸倍增</param>
        public void tmp_Set_SyncGlobalFontSize(bool state)
        {
            if (sync_global_font_size == state)
                return;
            sync_global_font_size = state;
        }
        /// <summary>
        /// 设置字体模板样式名称。
        /// </summary>
        /// <param name="name">样式名称</param>
        public void tmp_Set_StyleName(string name)
        {
            style_name = name;
        }
        /// <summary>
        /// 改变文字内容。
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="ParseEscape">是否解析转义字符</param>
        public void tmp_Set_Content(string content, bool ParseEscape = true)
        {
            if (ParseEscape)
                base.text = XGUI_Utilitys.Unescape(content);
            else
                base.text = content;
        }
        /// <summary>
        /// 从文字样式添加器反向更新文字样式。
        /// </summary>
        /// <param name="styleinfo">样式数据</param>
        public void tmp_Set_Style_ForSetter(XHud_LibraryArg_TextStyle styleinfo)
        {
            text_style_info = styleinfo;
            //Text_Set_Alignment(styleinfo.ContentAnchor);
            //Text_Set_Font(styleinfo.Font);
            //Text_Set_FontStyle(styleinfo.Style);
            //Text_Set_FontSize((int)styleinfo.尺寸_Size);
            //Text_Set_FontColor(styleinfo.FontColor);
            //Text_RichTextEnabledSet(styleinfo.RichText);
            //Text_Set_LineSpace(styleinfo.文字行高_LineHeight);
            //Text_Set_HorizontalOverflow(styleinfo.Overflow_Horizon);
            //Text_Set_VerticleOverflow(styleinfo.Overflow_Vertical);
            //Text_Set_FontBestSize(styleinfo.BestFit);
            //Text_Set_FontBestSize_Min(styleinfo.Fit_Min);
            //Text_Set_FontBestSize_Max(styleinfo.Fit_Max);
            ////Text_Set_SyncGlobalFontSize(styleinfo.SyncGlobalFontSize);
        }
        #endregion
    }
}