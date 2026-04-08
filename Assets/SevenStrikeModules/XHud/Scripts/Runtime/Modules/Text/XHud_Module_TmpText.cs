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
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.Utilitys;
    using TMPro;
    using UnityEngine;
    using UnityEngine.Events;

    [System.Serializable]
    /// <summary>
    /// 间距值
    /// </summary>
    public class XHud_TmpTextSpacingValue
    {
        /// <summary>
        /// 字符间距
        /// </summary>
        public float space_Character;
        /// <summary>
        /// 单词间距
        /// </summary>
        public float space_Word;
        /// <summary>
        /// 行高
        /// </summary>
        public float space_Line;
        /// <summary>
        /// 段落间距
        /// </summary>
        public float space_Paragraph;

        public XHud_TmpTextSpacingValue(float value_cha, float value_word, float value_line, float value_para)
        {
            space_Character = value_cha;
            space_Word = value_word;
            space_Line = value_line;
            space_Paragraph = value_para;
        }
    }

    public class XHud_Module_TmpText : TextMeshProUGUI
    {
        [SerializeField]
        /// <summary>
        /// 标识
        /// </summary>
        public string Indicator;
        [SerializeField]
        /// <summary>
        /// 文字字体样式索引名称
        /// </summary>
        public string StyleName;
        [SerializeField]
        /// <summary>
        /// 是否使用实时同步字体样式库
        /// </summary>
        public bool StyleLibSynching;
        [SerializeField]
        /// <summary>
        /// 判断内容是否改变
        /// </summary>
        private string PreviousText;
        [SerializeField]
        /// <summary>
        /// 内容已改变
        /// </summary>
        private bool TextIsChanged;
        [SerializeField]
        /// <summary>
        /// 事件内容改变
        /// </summary>
        public UnityAction Act_OnTextChanged;
        [SerializeField]
        /// <summary>
        /// 同步全局字体尺寸
        /// </summary>
        public bool SyncGlobalFontSize = true;
        [SerializeField]
        /// <summary>
        /// Hud管理器
        /// </summary>
        private XHud_Manager mgr;

        [SerializeField]
        /// <summary>
        /// 字体样式 - 本地
        /// </summary>
        public XHud_LibraryArg_TextStyle TextStyleInfo = new XHud_LibraryArg_TextStyle();
        [SerializeField]
        /// <summary>
        /// 字体样式 - 库
        /// </summary>
        public XHud_LibraryArg_TextStyle TextStyleInfo_Library = new XHud_LibraryArg_TextStyle();

        protected override void OnEnable()
        {
            base.OnEnable();
            if (!Application.isPlaying)
                if (mgr == null)
                    mgr = FindFirstObjectByType<XHud_Manager>();
        }

        protected override void Start()
        {
            base.Start();
            if (Application.isPlaying)
            {
                PreviousText = text;
            }
        }

        private void Update()
        {
            //-------------------内容变动检测
            tmp_Detect_Content();
            //-------------------运行时样式刷新
            tmp_Update_Style_Runtime();
            //--------------------同步全局字体尺寸
            tmp_Set_SyncGlobalFontSize(SyncGlobalFontSize);
            //--------------------更新渐变色
            tmp_Update_GradientColor();
        }

        protected override void UpdateMaterial()
        {
            base.UpdateMaterial();
        }

        #region 实时刷新类
        /// <summary>
        /// 内容改变事件
        /// </summary>
        private void tmp_Detect_Content()
        {
            if (PreviousText != text)
            {
                if (!TextIsChanged)
                {
                    TextIsChanged = true;
                    PreviousText = text;
                    if (Act_OnTextChanged != null)
                        Act_OnTextChanged();
                }
            }
            else
            {
                TextIsChanged = false;
            }
        }

        /// <summary>
        /// 从字体样式库中切换样式
        /// </summary>
        /// <param name="StyleName">样式名称</param>
        public void tmp_Update_Style_Runtime()
        {
            // 在未运行时获取HudManager
            if (!Application.isPlaying)
            {
                if (mgr == null)
                {
                    mgr = FindFirstObjectByType<XHud_Manager>();
                }
            }

            #region 如果同步库样式
            if (StyleLibSynching)
            {
                #region 样式集合为空判断
                if (mgr == null)
                    return;
                if (mgr.Hud_TextStyleLibrary == null)
                    return;
                if (mgr.Hud_TextStyleLibrary.TextStyle_Library_IsEmpty())
                    return;
                #endregion

                #region 从库中获取字体样式资源到 TextStyleInfo
                if (Application.isPlaying)
                {
                    TextStyleInfo_Library = XHud_Manager.Instance.Hud_TextStyleLibrary.TextStyle_Library_GetTextStyleInfo(StyleName);
                }
                else
                {
                    if (mgr != null && mgr.Hud_TextStyleLibrary != null)
                        TextStyleInfo_Library = mgr.Hud_TextStyleLibrary.TextStyle_Library_GetTextStyleInfo(StyleName);
                }
                #endregion

                #region 将TextStyleInfo同步到文字效果上
                //-----如果开启库同步 - RichText
                if (TextStyleInfo.LibStyle_Effect_rich)
                {
                    TextStyleInfo.tmp_rich = TextStyleInfo_Library.tmp_rich;
                }

                //-----如果开启库同步 - 包裹文字
                if (TextStyleInfo.LibStyle_Effect_wrap)
                {
                    TextStyleInfo.tmp_contentwrap = TextStyleInfo_Library.tmp_contentwrap;
                }

                //-----如果开启库同步 - 溢出文字
                if (TextStyleInfo.LibStyle_Effect_overflow)
                {
                    TextStyleInfo.tmp_overflow = TextStyleInfo_Library.tmp_overflow;
                }

                //-----如果开启库同步 - 字体样式
                if (TextStyleInfo.LibStyle_Effect_style)
                {
                    TextStyleInfo.tmp_style = TextStyleInfo_Library.tmp_style;
                }

                //-----如果开启库同步 - 对齐锚点
                if (TextStyleInfo.LibStyle_Effect_align)
                {
                    TextStyleInfo.tmp_anchor = TextStyleInfo_Library.tmp_anchor;
                }

                //-----如果开启库同步 - 字体
                if (TextStyleInfo.LibStyle_Effect_font)
                {
                    TextStyleInfo.tmp_font = TextStyleInfo_Library.tmp_font;
                }

                //-----如果开启库同步 - 尺寸_Size
                if (TextStyleInfo.LibStyle_Effect_size)
                {
                    TextStyleInfo.tmp_size = TextStyleInfo_Library.tmp_size;
                }

                //-----如果开启库同步 - 自动尺寸
                if (TextStyleInfo.LibStyle_Effect_autosizing)
                {
                    TextStyleInfo.tmp_EnableAutoSizing = TextStyleInfo_Library.tmp_EnableAutoSizing;
                    TextStyleInfo.tmp_FontSizeMin = TextStyleInfo_Library.tmp_FontSizeMin;
                    TextStyleInfo.tmp_FontSizeMax = TextStyleInfo_Library.tmp_FontSizeMax;
                }

                //-----如果开启库同步 - 间距
                if (TextStyleInfo.LibStyle_Effect_space)
                {
                    TextStyleInfo.tmp_space_character = TextStyleInfo_Library.tmp_space_character;
                    TextStyleInfo.tmp_space_word = TextStyleInfo_Library.tmp_space_word;
                    TextStyleInfo.tmp_space_lineheight = TextStyleInfo_Library.tmp_space_lineheight;
                    TextStyleInfo.tmp_space_paragraph = TextStyleInfo_Library.tmp_space_paragraph;
                }

                //-----如果开启库同步 - 包裹对齐
                if (TextStyleInfo.LibStyle_Effect_wrapratio)
                {
                    TextStyleInfo.tmp_WrappingRatios = TextStyleInfo_Library.tmp_WrappingRatios;
                }

                //-----如果开启库同步 - 边距
                if (TextStyleInfo.LibStyle_Effect_margin)
                {
                    TextStyleInfo.tmp_contentmargin = TextStyleInfo_Library.tmp_contentmargin;
                }

                //-----如果开启库同步 - 颜色_Color
                if (TextStyleInfo.LibStyle_Effect_color)
                {
                    TextStyleInfo.tmp_color = TextStyleInfo_Library.tmp_color;
                }

                //-----如果开启库同步 - 渐变色
                if (TextStyleInfo.LibStyle_Effect_gradientcolor)
                {
                    TextStyleInfo.gra_A = TextStyleInfo_Library.gra_A;
                    TextStyleInfo.gra_B = TextStyleInfo_Library.gra_B;
                    TextStyleInfo.gra_C = TextStyleInfo_Library.gra_C;
                    TextStyleInfo.gra_D = TextStyleInfo_Library.gra_D;
                    TextStyleInfo.gra_ColorMode = TextStyleInfo_Library.gra_ColorMode;
                    TextStyleInfo.gra_ColorModeName = TextStyleInfo_Library.gra_ColorModeName;
                    TextStyleInfo.gra_Invert = TextStyleInfo_Library.gra_Invert;
                    TextStyleInfo.gra_Used = TextStyleInfo_Library.gra_Used;
                }

                //-----如果开启库同步 - 射线检测
                if (TextStyleInfo.LibStyle_Effect_raycast)
                {
                    TextStyleInfo.Raycast = TextStyleInfo_Library.Raycast;
                }

                //-----如果开启库同步 - 遮罩                
                if (TextStyleInfo.LibStyle_Effect_maskable)
                {
                    TextStyleInfo.Maskable = TextStyleInfo_Library.Maskable;
                }
                #endregion
            }
            #endregion

            #region 刷新字体样式
            tmp_Syncing_Style();
            #endregion
        }

        /// <summary>
        /// 将TextStyleInfo的值同步给字体样式
        /// </summary>
        public void tmp_Syncing_Style()
        {
            //--同步 - RichText
            if (richText != TextStyleInfo.tmp_rich)
            {
                richText = TextStyleInfo.tmp_rich;
            }

            //--同步 - 包裹文字
            if (textWrappingMode != TextStyleInfo.tmp_contentwrap)
            {
                textWrappingMode = TextStyleInfo.tmp_contentwrap;
            }

            //--同步 - 溢出文字
            if (overflowMode != TextStyleInfo.tmp_overflow)
            {
                overflowMode = TextStyleInfo.tmp_overflow;
            }

            //--同步 - 字体样式
            if (fontStyle != TextStyleInfo.tmp_style)
            {
                fontStyle = TextStyleInfo.tmp_style;
            }

            //--同步 - 对齐锚点
            switch (TextStyleInfo.tmp_anchor)
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
            if (TextStyleInfo.tmp_font != null && TextStyleInfo.tmp_font != font)
            {
                font = TextStyleInfo.tmp_font;
            }

            //--同步 - 自动尺寸 - 开关
            if (enableAutoSizing != TextStyleInfo.tmp_EnableAutoSizing)
            {
                enableAutoSizing = TextStyleInfo.tmp_EnableAutoSizing;
            }

            //--同步 - 自动尺寸 - 逻辑赋值
            if (enableAutoSizing)
            {
                //--最小尺寸
                if (SyncGlobalFontSize)
                {
                    if (!Application.isPlaying)
                    {
                        TextStyleInfo.tmp_FontSizeMin_globalscaled = mgr.FontSizeMultiply * TextStyleInfo.tmp_FontSizeMin;
                    }
                    else
                    {
                        TextStyleInfo.tmp_FontSizeMin_globalscaled = XHud_Manager.Instance.FontSizeMultiply * TextStyleInfo.tmp_FontSizeMin;
                    }

                    if (TextStyleInfo.tmp_FontSizeMin_globalscaled != fontSizeMin)
                    {
                        fontSizeMin = TextStyleInfo.tmp_FontSizeMin_globalscaled;
                    }
                }
                else
                {
                    if (fontSizeMin != TextStyleInfo.tmp_FontSizeMin)
                    {
                        fontSizeMin = TextStyleInfo.tmp_FontSizeMin;
                    }
                }

                //--最大尺寸
                if (SyncGlobalFontSize)
                {
                    if (!Application.isPlaying)
                    {
                        TextStyleInfo.tmp_FontSizeMax_globalscaled = mgr.FontSizeMultiply * TextStyleInfo.tmp_FontSizeMax;
                    }
                    else
                    {
                        TextStyleInfo.tmp_FontSizeMax_globalscaled = XHud_Manager.Instance.FontSizeMultiply * TextStyleInfo.tmp_FontSizeMax;
                    }

                    if (TextStyleInfo.tmp_FontSizeMax_globalscaled != fontSizeMax)
                    {
                        fontSizeMax = TextStyleInfo.tmp_FontSizeMax_globalscaled;
                    }
                }
                else
                {
                    if (fontSizeMax != TextStyleInfo.tmp_FontSizeMax)
                    {
                        fontSizeMax = TextStyleInfo.tmp_FontSizeMax;
                    }
                }

                //--字符宽度百分比
                if (characterWidthAdjustment != TextStyleInfo.tmp_CharWidthMaxAdj)
                {
                    characterWidthAdjustment = TextStyleInfo.tmp_CharWidthMaxAdj;
                }

                //--最大行高
                if (lineSpacingAdjustment != TextStyleInfo.tmp_LineSpacingMax)
                {
                    lineSpacingAdjustment = TextStyleInfo.tmp_LineSpacingMax;
                }
            }
            //--同步 - 原生尺寸 - 逻辑赋值
            else
            {
                if (SyncGlobalFontSize)
                {
                    if (!Application.isPlaying)
                    {
                        TextStyleInfo.tmp_size_globalscaled = mgr.FontSizeMultiply * TextStyleInfo.tmp_size;
                    }
                    else
                    {
                        TextStyleInfo.tmp_size_globalscaled = XHud_Manager.Instance.FontSizeMultiply * TextStyleInfo.tmp_size;
                    }

                    if (TextStyleInfo.tmp_size_globalscaled != fontSize || m_fontSizeBase != m_fontSize)
                    {
                        fontSize = TextStyleInfo.tmp_size_globalscaled;
                    }
                }
                else
                {
                    if (TextStyleInfo.tmp_size != fontSize || m_fontSizeBase != m_fontSize)
                    {
                        fontSize = TextStyleInfo.tmp_size;
                    }
                }
            }

            //--同步 - 间距 - 字符
            if (characterSpacing != TextStyleInfo.tmp_space_character)
            {
                characterSpacing = TextStyleInfo.tmp_space_character;
            }

            //--同步 - 间距 - 单词
            if (wordSpacing != TextStyleInfo.tmp_space_word)
            {
                wordSpacing = TextStyleInfo.tmp_space_word;
            }

            //--同步 - 间距 - 行高
            if (lineSpacing != TextStyleInfo.tmp_space_lineheight)
            {
                lineSpacing = TextStyleInfo.tmp_space_lineheight;
            }

            //--同步 - 间距 - 段落
            if (paragraphSpacing != TextStyleInfo.tmp_space_paragraph)
            {
                paragraphSpacing = TextStyleInfo.tmp_space_paragraph;
            }

            //--同步 - 包裹对齐
            if (wordWrappingRatios != TextStyleInfo.tmp_WrappingRatios)
            {
                wordWrappingRatios = TextStyleInfo.tmp_WrappingRatios;
            }

            //--同步 - 边距
            if (margin != TextStyleInfo.tmp_contentmargin)
            {
                margin = TextStyleInfo.tmp_contentmargin;
            }

            //--同步 - 颜色_Color
            if (color != TextStyleInfo.tmp_color)
            {
                color = TextStyleInfo.tmp_color;
            }

            //--同步 - 渐变色开关
            if (enableVertexGradient != TextStyleInfo.gra_Used)
            {
                enableVertexGradient = TextStyleInfo.gra_Used;
            }

            //--同步 - 渐变色模式
            if (m_colorMode != TextStyleInfo.gra_ColorMode)
            {
                m_colorMode = TextStyleInfo.gra_ColorMode;

                switch (TextStyleInfo.gra_ColorMode)
                {
                    case ColorMode.Single:
                        TextStyleInfo.gra_ColorModeName = "单色";
                        break;
                    case ColorMode.HorizontalGradient:
                        TextStyleInfo.gra_ColorModeName = "水平渐变";
                        break;
                    case ColorMode.VerticalGradient:
                        TextStyleInfo.gra_ColorModeName = "垂直渐变";
                        break;
                    case ColorMode.FourCornersGradient:
                        TextStyleInfo.gra_ColorModeName = "四角渐变";
                        break;
                }
            }

            //--同步 - 射线检测
            if (raycastTarget != TextStyleInfo.Raycast)
            {
                raycastTarget = TextStyleInfo.Raycast;
            }

            //--同步 - 遮罩
            if (maskable != TextStyleInfo.Maskable)
            {
                maskable = TextStyleInfo.Maskable;
            }
        }

        /// <summary>
        /// 从文字样式添加器反向更新文字样式
        /// </summary>
        /// <param name="styleinfo"></param>
        public void tmp_Update_Style_LibrarySetter(XHud_LibraryArg_TextStyle styleinfo)
        {
            TextStyleInfo = styleinfo;
        }

        /// <summary>
        /// 更新渐变色
        /// </summary>  
        private void tmp_Update_GradientColor()
        {
            VertexGradient cg = colorGradient;
            switch (TextStyleInfo.gra_ColorMode)
            {
                case ColorMode.Single:
                    cg.topLeft = TextStyleInfo.gra_A;
                    cg.topRight = TextStyleInfo.gra_A;
                    cg.bottomLeft = TextStyleInfo.gra_A;
                    cg.bottomRight = TextStyleInfo.gra_A;
                    break;
                case ColorMode.HorizontalGradient:
                    if (TextStyleInfo.gra_Invert)
                    {
                        cg.topLeft = TextStyleInfo.gra_B;
                        cg.topRight = TextStyleInfo.gra_A;
                        cg.bottomLeft = TextStyleInfo.gra_B;
                        cg.bottomRight = TextStyleInfo.gra_A;
                    }
                    else
                    {
                        cg.topLeft = TextStyleInfo.gra_A;
                        cg.topRight = TextStyleInfo.gra_B;
                        cg.bottomLeft = TextStyleInfo.gra_A;
                        cg.bottomRight = TextStyleInfo.gra_B;
                    }
                    break;
                case ColorMode.VerticalGradient:
                    if (TextStyleInfo.gra_Invert)
                    {
                        cg.topLeft = TextStyleInfo.gra_A;
                        cg.topRight = TextStyleInfo.gra_A;
                        cg.bottomLeft = TextStyleInfo.gra_B;
                        cg.bottomRight = TextStyleInfo.gra_B;
                    }
                    else
                    {
                        cg.topLeft = TextStyleInfo.gra_B;
                        cg.topRight = TextStyleInfo.gra_B;
                        cg.bottomLeft = TextStyleInfo.gra_A;
                        cg.bottomRight = TextStyleInfo.gra_A;
                    }
                    break;
                case ColorMode.FourCornersGradient:
                    if (TextStyleInfo.gra_Invert)
                    {
                        cg.topLeft = TextStyleInfo.gra_B;
                        cg.topRight = TextStyleInfo.gra_A;
                        cg.bottomLeft = TextStyleInfo.gra_D;
                        cg.bottomRight = TextStyleInfo.gra_C;
                    }
                    else
                    {
                        cg.topLeft = TextStyleInfo.gra_A;
                        cg.topRight = TextStyleInfo.gra_B;
                        cg.bottomLeft = TextStyleInfo.gra_C;
                        cg.bottomRight = TextStyleInfo.gra_D;
                    }
                    break;
            }
            colorGradient = cg;
        }
        #endregion

        #region 参数设置
        /// <summary>
        /// 设置全局尺寸影响可用性
        /// </summary>
        /// <param name="treeState"></param>
        public void tmp_Set_SyncGlobalFontSize(bool state)
        {
            if (SyncGlobalFontSize == state)
                return;
            SyncGlobalFontSize = state;
        }

        /// <summary>
        /// 设置字体模板样式名称
        /// </summary>
        /// <param name="name"></param>
        public void tmp_Set_StyleName(string name)
        {
            StyleName = name;
        }

        /// <summary>
        /// 改变文字内容
        /// </summary>
        /// <param name="content">内容</param>
        public void tmp_Set_Content(string content, bool ParseEscape = true)
        {
            if (ParseEscape)
                base.text = XHud_Utilitys.ProcessEscapeSequences(content);
            else
                base.text = content;
        }

        /// <summary>
        /// 从文字样式添加器反向更新文字样式
        /// </summary>
        /// <param name="styleinfo"></param>
        public void tmp_Set_Style_ForSetter(XHud_LibraryArg_TextStyle styleinfo)
        {
            TextStyleInfo = styleinfo;
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