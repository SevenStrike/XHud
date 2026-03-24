namespace SevenStrikeModules.XHud.Hud
{
    using SevenStrikeModules.XHud.Enums;
    using SevenStrikeModules.XHud.Utilitys;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class xHud_Module_Text : Text
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
        private xHud_Manager mgr;

        [SerializeField]
        /// <summary>
        /// 字体样式 - 本地
        /// </summary>
        public xHud_LibraryArg_TextStyle TextStyleInfo = new xHud_LibraryArg_TextStyle();
        [SerializeField]
        /// <summary>
        /// 字体样式 - 库
        /// </summary>
        public xHud_LibraryArg_TextStyle TextStyleInfo_Library = new xHud_LibraryArg_TextStyle();

        protected override void OnEnable()
        {
            base.OnEnable();
            if (!Application.isPlaying)
                if (mgr == null)
                    mgr = FindFirstObjectByType<xHud_Manager>();
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
            txt_Detect_Content();
            //-------------------运行时样式刷新
            txt_Update_Style_Runtime();
            //--------------------同步全局字体尺寸
            txt_Set_SyncGlobalFontSize(SyncGlobalFontSize);
        }

        #region 实时刷新类

        /// <summary>
        /// 内容改变事件
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
        /// <param tweenName="StyleName">样式名称</param>
        public void txt_Update_Style_Runtime()
        {
            // 在未运行时获取HudManager
            if (!Application.isPlaying)
            {
                if (mgr == null)
                {
                    mgr = FindFirstObjectByType<xHud_Manager>();
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
                    TextStyleInfo_Library = xHud_Manager.Instance.Hud_TextStyleLibrary.TextStyle_Library_GetTextStyleInfo(StyleName);
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
        /// 将TextStyleInfo的值同步给字体样式
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
                        TextStyleInfo.Size_GlobalScaled = mgr.FontSizeMultiply * TextStyleInfo.Size;
                    }
                    else
                    {
                        TextStyleInfo.Size_GlobalScaled = xHud_Manager.Instance.FontSizeMultiply * TextStyleInfo.Size;
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
                        TextStyleInfo.Fit_Min_GlobalScaled = mgr.FontSizeMultiply * TextStyleInfo.Fit_Min;
                    }
                    else
                    {
                        TextStyleInfo.Fit_Min_GlobalScaled = xHud_Manager.Instance.FontSizeMultiply * TextStyleInfo.Fit_Min;
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
                        TextStyleInfo.Fit_Max_GlobalScaled = mgr.FontSizeMultiply * TextStyleInfo.Fit_Max;
                    }
                    else
                    {
                        TextStyleInfo.Fit_Max_GlobalScaled = xHud_Manager.Instance.FontSizeMultiply * TextStyleInfo.Fit_Max;
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
            if (color != TextStyleInfo.FontColor)
            {
                color = TextStyleInfo.FontColor;
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

        #endregion

        #region 参数设置

        /// <summary>
        /// 设置全局尺寸影响可用性
        /// </summary>
        /// <param tweenName="treeState"></param>
        public void txt_Set_SyncGlobalFontSize(bool state)
        {
            if (SyncGlobalFontSize == state)
                return;
            SyncGlobalFontSize = state;
        }

        /// <summary>
        /// 设置字体模板样式名称
        /// </summary>
        /// <param tweenName="name"></param>
        public void txt_Set_StyleName(string name)
        {
            StyleName = name;
        }

        /// <summary>
        /// 改变文字内容
        /// </summary>
        /// <param tweenName="content">内容</param>
        public void txt_Set_Content(string content, bool ParseEscape = true)
        {
            if (ParseEscape)
                base.text = xHud_Utilitys.ProcessEscapeSequences(content);
            else
                base.text = content;
        }

        /// <summary>
        /// 从文字样式添加器反向更新文字样式
        /// </summary>
        /// <param tweenName="styleinfo"></param>
        public void txt_Set_Style_ForSetter(xHud_LibraryArg_TextStyle styleinfo)
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