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
    using System.Diagnostics;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class XHud_Module_Text : Text
    {
        #region 字段 - 基础参数
        /// <summary>
        /// 标识名称。
        /// </summary>
        [SerializeField]
        public string Indicator;
        /// <summary>
        /// 文字字体样式索引名称。
        /// </summary>
        [SerializeField]
        public string StyleName;
        /// <summary>
        /// 是否使用实时同步字体样式库。
        /// </summary>
        [SerializeField]
        public bool StyleLibSynching;
        /// <summary>
        /// 判断内容是否改变。
        /// </summary>
        [SerializeField]
        private string PreviousText;
        /// <summary>
        /// 内容已改变。
        /// </summary>
        [SerializeField]
        private bool TextIsChanged;
        /// <summary>
        /// 事件内容改变。
        /// </summary>
        [SerializeField]
        public UnityAction<string> Act_OnTextChanged;
        /// <summary>
        /// 同步全局字体尺寸。
        /// </summary>
        [SerializeField]
        public bool SyncGlobalFontSize = true;
        /// <summary>
        /// 指定文字颜色接管的拥有者。
        /// </summary>
        [SerializeField]
        public TextColorOwner TextColorOwner;
        #endregion

        #region 字段 - 样式
        /// <summary>
        /// 字体样式 - 本地。
        /// </summary>
        [SerializeField]
        public XHud_LibraryArg_TextStyle TextStyleInfo = new XHud_LibraryArg_TextStyle();
        /// <summary>
        /// 字体样式 - 库。
        /// </summary>
        [SerializeField]
        public XHud_LibraryArg_TextStyle TextStyleInfo_Library = new XHud_LibraryArg_TextStyle();
        #endregion

        #region 字段 - 折叠状态
        /// <summary>
        /// 参数面板折叠状态。
        /// </summary>
        public bool
            fold_param = true,
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
        }
        /// <summary>
        /// 分别设置各面板折叠状态。
        /// </summary>
        /// <param name="param">参数面板</param>
        /// <param name="option">选项面板</param>
        /// <param name="based">源脚本面板</param>
        /// <param name="sync">库同步项面板</param>
        public void GroupFold(bool param = true, bool option = true, bool based = true, bool sync = true)
        {
            fold_param = param;
            fold_option = option;
            fold_based = based;
            fold_sync = sync;
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
                PreviousText = text;
            }
        }
        /// <summary>
        /// Unity 每帧回调。
        /// <para/>
        /// 执行内容变更检测、运行时样式刷新与全局字体尺寸同步。
        /// </summary>
        private void Update()
        {
            //-------------------内容变动检测
            txt_Detect_Content();
            //-------------------运行时样式刷新
            txt_Update_Style_Runtime();
        }
        #endregion

        #region 实时刷新类
        /// <summary>
        /// 内容改变检测。
        /// <para/>
        /// 对比 <see cref="PreviousText"/> 与当前 <c>text</c>，
        /// 若发生变化则触发 <see cref="Act_OnTextChanged"/>。
        /// </summary>
        private void txt_Detect_Content()
        {
            if (PreviousText != text)
            {
                if (!TextIsChanged)
                {
                    TextIsChanged = true;
                    PreviousText = text;
                    if (Act_OnTextChanged != null)
                        Act_OnTextChanged(text);
                }
            }
            else
            {
                TextIsChanged = false;
            }
        }
        /// <summary>
        /// 从字体样式库中切换样式并刷新文字效果。
        /// <para/>
        /// 若 <see cref="StyleLibSynching"/> 开启，则从样式库读取对应样式，
        /// 按各 <c>LibStyle_Effect_*</c> 开关决定哪些属性同步到本地样式；
        /// 最后调用 <see cref="txt_Syncing_Style"/> 将本地样式写入 Text 组件。
        /// </summary>
        /// <param name="StyleName">样式名称</param>
        public void txt_Update_Style_Runtime()
        {
            //--------------------同步全局字体尺寸
            txt_Set_SyncGlobalFontSize(SyncGlobalFontSize);

            #region 如果同步库样式
            if (StyleLibSynching)
            {
                XHud_Manager mgr = XHud_Dashboard.GetManager();

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
                    TextStyleInfo.RichText = TextStyleInfo_Library.RichText;
                }

                //-----如果开启库同步 - GeoMetreAlign
                if (TextStyleInfo.LibStyle_Effect_geometre_align)
                {
                    TextStyleInfo.GeometreAlign = TextStyleInfo_Library.GeometreAlign;
                }

                //-----如果开启库同步 - 溢出文字 H
                if (TextStyleInfo.LibStyle_Effect_overflow_h)
                {
                    TextStyleInfo.Overflow_Horizon = TextStyleInfo_Library.Overflow_Horizon;
                }

                //-----如果开启库同步 - 溢出文字 V
                if (TextStyleInfo.LibStyle_Effect_overflow_v)
                {
                    TextStyleInfo.Overflow_Vertical = TextStyleInfo_Library.Overflow_Vertical;
                }

                //-----如果开启库同步 - 字体样式
                if (TextStyleInfo.LibStyle_Effect_style)
                {
                    TextStyleInfo.Style = TextStyleInfo_Library.Style;
                }

                //-----如果开启库同步 - 锚点
                if (TextStyleInfo.LibStyle_Effect_align)
                {
                    TextStyleInfo.ContentAnchor = TextStyleInfo_Library.ContentAnchor;
                }

                //-----如果开启库同步 - 字体
                if (TextStyleInfo.LibStyle_Effect_font)
                {
                    TextStyleInfo.Font = TextStyleInfo_Library.Font;
                }

                //-----如果开启库同步 - 尺寸_Size
                if (TextStyleInfo.LibStyle_Effect_size)
                {
                    TextStyleInfo.Size = TextStyleInfo_Library.Size;
                }

                //-----如果开启库同步 - 自动匹配尺寸
                if (TextStyleInfo.LibStyle_Effect_bestfit)
                {
                    TextStyleInfo.BestFit = TextStyleInfo_Library.BestFit;
                    TextStyleInfo.Fit_Min = TextStyleInfo_Library.Fit_Min;
                    TextStyleInfo.Fit_Max = TextStyleInfo_Library.Fit_Max;
                }

                //-----如果开启库同步 - 行高
                if (TextStyleInfo.LibStyle_Effect_line)
                {
                    TextStyleInfo.LineHeight = TextStyleInfo_Library.LineHeight;
                }

                //-----如果开启库同步 - 颜色_Color
                if (TextStyleInfo.LibStyle_Effect_color)
                {
                    TextStyleInfo.FontColor = TextStyleInfo_Library.FontColor;
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
            txt_Syncing_Style();
            #endregion
        }
        /// <summary>
        /// 将 <see cref="TextStyleInfo"/> 的值同步给 Text 组件。
        /// <para/>
        /// 涵盖：富文本 / 溢出 / 字体样式 / 对齐 / 几何对齐 / 字体 /
        /// 自适应尺寸 / 全局尺寸倍率 / 行高 / 颜色 / 射线检测 / 遮罩。
        /// </para>
        /// 颜色仅在 <see cref="TextColorOwner"/> 不为 Tween 时同步。
        /// </summary>
        public void txt_Syncing_Style()
        {
            //--同步 - RichText
            if (supportRichText != TextStyleInfo.RichText)
            {
                supportRichText = TextStyleInfo.RichText;
            }

            //--同步 - 溢出文字 H
            if (horizontalOverflow != TextStyleInfo.Overflow_Horizon)
            {
                horizontalOverflow = TextStyleInfo.Overflow_Horizon;
            }

            //--同步 - 溢出文字 V
            if (verticalOverflow != TextStyleInfo.Overflow_Vertical)
            {
                verticalOverflow = TextStyleInfo.Overflow_Vertical;
            }

            //--同步 - 字体样式
            if (fontStyle != TextStyleInfo.Style)
            {
                fontStyle = TextStyleInfo.Style;
            }

            //--同步 - 对齐锚点
            switch (TextStyleInfo.ContentAnchor)
            {
                case ContentAnchor.上:
                    alignment = TextAnchor.UpperCenter;
                    break;
                case ContentAnchor.下:
                    alignment = TextAnchor.LowerCenter;
                    break;
                case ContentAnchor.左:
                    alignment = TextAnchor.MiddleLeft;
                    break;
                case ContentAnchor.右:
                    alignment = TextAnchor.MiddleRight;
                    break;
                case ContentAnchor.中心:
                    alignment = TextAnchor.MiddleCenter;
                    break;
                case ContentAnchor.左上:
                    alignment = TextAnchor.UpperLeft;
                    break;
                case ContentAnchor.左下:
                    alignment = TextAnchor.LowerLeft;
                    break;
                case ContentAnchor.右上:
                    alignment = TextAnchor.UpperRight;
                    break;
                case ContentAnchor.右下:
                    alignment = TextAnchor.LowerRight;
                    break;
            }

            //--同步 - 几何对齐
            if (alignByGeometry != TextStyleInfo.GeometreAlign)
            {
                alignByGeometry = TextStyleInfo.GeometreAlign;
            }

            //--同步 - 字体
            if (TextStyleInfo.Font != null && TextStyleInfo.Font != font)
            {
                font = TextStyleInfo.Font;
            }

            //--同步 - 自动尺寸
            if (resizeTextForBestFit != TextStyleInfo.BestFit)
            {
                resizeTextForBestFit = TextStyleInfo.BestFit;
            }

            //--同步 - 原生尺寸 - 逻辑赋值
            if (!resizeTextForBestFit)
            {
                if (SyncGlobalFontSize)
                {
                    if (!Application.isPlaying)
                    {
                        TextStyleInfo.Size_GlobalScaled = XHud_Dashboard.GetManager().FontSizeMultiply * TextStyleInfo.Size;
                    }
                    else
                    {
                        TextStyleInfo.Size_GlobalScaled = XHud_Manager.Instance.FontSizeMultiply * TextStyleInfo.Size;
                    }

                    if (TextStyleInfo.Size_GlobalScaled != fontSize)
                    {
                        fontSize = (int)TextStyleInfo.Size_GlobalScaled;
                    }
                }
                else
                {
                    if (TextStyleInfo.Size != fontSize)
                    {
                        fontSize = (int)TextStyleInfo.Size;
                    }
                }
            }
            else//--同步 - 自动尺寸 - 逻辑赋值
            {
                //--最小尺寸
                if (SyncGlobalFontSize)
                {
                    if (!Application.isPlaying)
                    {
                        TextStyleInfo.Fit_Min_GlobalScaled = XHud_Dashboard.GetManager().FontSizeMultiply * TextStyleInfo.Fit_Min;
                    }
                    else
                    {
                        TextStyleInfo.Fit_Min_GlobalScaled = XHud_Manager.Instance.FontSizeMultiply * TextStyleInfo.Fit_Min;
                    }

                    if (TextStyleInfo.Fit_Min_GlobalScaled != resizeTextMinSize)
                    {
                        resizeTextMinSize = (int)TextStyleInfo.Fit_Min_GlobalScaled;
                    }
                }
                else
                {
                    if (TextStyleInfo.Fit_Min != resizeTextMinSize)
                    {
                        resizeTextMinSize = (int)TextStyleInfo.Fit_Min;
                    }
                }

                //--最大尺寸
                if (SyncGlobalFontSize)
                {
                    if (!Application.isPlaying)
                    {
                        TextStyleInfo.Fit_Max_GlobalScaled = XHud_Dashboard.GetManager().FontSizeMultiply * TextStyleInfo.Fit_Max;
                    }
                    else
                    {
                        TextStyleInfo.Fit_Max_GlobalScaled = XHud_Manager.Instance.FontSizeMultiply * TextStyleInfo.Fit_Max;
                    }

                    if (TextStyleInfo.Fit_Max_GlobalScaled != resizeTextMaxSize)
                    {
                        resizeTextMaxSize = (int)TextStyleInfo.Fit_Max_GlobalScaled;
                    }
                }
                else
                {
                    if (TextStyleInfo.Fit_Max != resizeTextMaxSize)
                    {
                        resizeTextMaxSize = (int)TextStyleInfo.Fit_Max;
                    }
                }
            }

            //--同步 - 间距 - 行高
            if (lineSpacing != TextStyleInfo.LineHeight)
            {
                lineSpacing = TextStyleInfo.LineHeight;
            }

            //--同步 - 颜色_Color
            if (TextColorOwner != TextColorOwner.Tween)
            {
                if (color != TextStyleInfo.FontColor)
                {
                    color = TextStyleInfo.FontColor;
                }
            }

            //--同步 - 射线检测
            if (raycastTarget != TextStyleInfo.Raycast)
            {
                raycastTarget = TextStyleInfo.Raycast;
                //SetRaycastDirty();
            }

            //--同步 - 遮罩
            if (maskable != TextStyleInfo.Maskable)
            {
                maskable = TextStyleInfo.Maskable;
            }
        }
        #endregion

        #region 参数设置
        /// <summary>
        /// 设置全局尺寸影响可用性。
        /// </summary>
        /// <param name="state">true = 启用全局尺寸倍增</param>
        public void txt_Set_SyncGlobalFontSize(bool state)
        {
            if (SyncGlobalFontSize == state)
                return;
            SyncGlobalFontSize = state;
        }
        /// <summary>
        /// 设置字体模板样式名称。
        /// </summary>
        /// <param name="name">样式名称</param>
        public void txt_Set_StyleName(string name)
        {
            StyleName = name;
        }
        /// <summary>
        /// 改变文字内容。
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="ParseEscape">是否解析转义字符</param>
        public void txt_Set_Content(string content, bool ParseEscape = true)
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
        public void txt_Set_Style_ForSetter(XHud_LibraryArg_TextStyle styleinfo)
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