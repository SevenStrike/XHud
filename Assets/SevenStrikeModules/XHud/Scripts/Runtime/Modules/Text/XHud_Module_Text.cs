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
                previous_text = text;
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
        /// 对比 <see cref="previous_text"/> 与当前 <c>text</c>，
        /// 若发生变化则触发 <see cref="act_on_text_changed"/>。
        /// </summary>
        private void txt_Detect_Content()
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
        /// 最后调用 <see cref="txt_Syncing_Style"/> 将本地样式写入 Text 组件。
        /// </summary>
        /// <param name="StyleName">样式名称</param>
        public void txt_Update_Style_Runtime()
        {
            //--------------------同步全局字体尺寸
            txt_Set_SyncGlobalFontSize(sync_global_font_size);

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
                    text_style_info.rich_text = text_style_info_library.rich_text;
                }

                //-----如果开启库同步 - GeoMetreAlign
                if (text_style_info.lib_style_effect_geometre_align)
                {
                    text_style_info.geometre_align = text_style_info_library.geometre_align;
                }

                //-----如果开启库同步 - 溢出文字 H
                if (text_style_info.lib_style_effect_overflow_h)
                {
                    text_style_info.overflow_horizon = text_style_info_library.overflow_horizon;
                }

                //-----如果开启库同步 - 溢出文字 V
                if (text_style_info.lib_style_effect_overflow_v)
                {
                    text_style_info.overflow_vertical = text_style_info_library.overflow_vertical;
                }

                //-----如果开启库同步 - 字体样式
                if (text_style_info.lib_style_effect_style)
                {
                    text_style_info.style = text_style_info_library.style;
                }

                //-----如果开启库同步 - 锚点
                if (text_style_info.lib_style_effect_align)
                {
                    text_style_info.content_anchor = text_style_info_library.content_anchor;
                }

                //-----如果开启库同步 - 字体
                if (text_style_info.lib_style_effect_font)
                {
                    text_style_info.font = text_style_info_library.font;
                }

                //-----如果开启库同步 - 尺寸_Size
                if (text_style_info.lib_style_effect_size)
                {
                    text_style_info.size = text_style_info_library.size;
                }

                //-----如果开启库同步 - 自动匹配尺寸
                if (text_style_info.lib_style_effect_bestfit)
                {
                    text_style_info.best_fit = text_style_info_library.best_fit;
                    text_style_info.fit_min = text_style_info_library.fit_min;
                    text_style_info.fit_max = text_style_info_library.fit_max;
                }

                //-----如果开启库同步 - 行高
                if (text_style_info.lib_style_effect_line)
                {
                    text_style_info.line_height = text_style_info_library.line_height;
                }

                //-----如果开启库同步 - 颜色_Color
                if (text_style_info.lib_style_effect_color)
                {
                    text_style_info.font_color = text_style_info_library.font_color;
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
            txt_Syncing_Style();
            #endregion
        }
        /// <summary>
        /// 将 <see cref="text_style_info"/> 的值同步给 Text 组件。
        /// <para/>
        /// 涵盖：富文本 / 溢出 / 字体样式 / 对齐 / 几何对齐 / 字体 /
        /// 自适应尺寸 / 全局尺寸倍率 / 行高 / 颜色 / 射线检测 / 遮罩。
        /// </para>
        /// 颜色仅在 <see cref="text_color_owner"/> 不为 Tween 时同步。
        /// </summary>
        public void txt_Syncing_Style()
        {
            //--同步 - RichText
            if (supportRichText != text_style_info.rich_text)
            {
                supportRichText = text_style_info.rich_text;
            }

            //--同步 - 溢出文字 H
            if (horizontalOverflow != text_style_info.overflow_horizon)
            {
                horizontalOverflow = text_style_info.overflow_horizon;
            }

            //--同步 - 溢出文字 V
            if (verticalOverflow != text_style_info.overflow_vertical)
            {
                verticalOverflow = text_style_info.overflow_vertical;
            }

            //--同步 - 字体样式
            if (fontStyle != text_style_info.style)
            {
                fontStyle = text_style_info.style;
            }

            //--同步 - 对齐锚点
            switch (text_style_info.content_anchor)
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
            if (alignByGeometry != text_style_info.geometre_align)
            {
                alignByGeometry = text_style_info.geometre_align;
            }

            //--同步 - 字体
            if (text_style_info.font != null && text_style_info.font != font)
            {
                font = text_style_info.font;
            }

            //--同步 - 自动尺寸
            if (resizeTextForBestFit != text_style_info.best_fit)
            {
                resizeTextForBestFit = text_style_info.best_fit;
            }

            //--同步 - 原生尺寸 - 逻辑赋值
            if (!resizeTextForBestFit)
            {
                if (sync_global_font_size)
                {
                    if (!Application.isPlaying)
                    {
                        text_style_info.size_global_scaled = XHud_Dashboard.get_manager().ft_font_size_multiply * text_style_info.size;
                    }
                    else
                    {
                        text_style_info.size_global_scaled = XHud_Manager.instance.ft_font_size_multiply * text_style_info.size;
                    }

                    if (text_style_info.size_global_scaled != fontSize)
                    {
                        fontSize = (int)text_style_info.size_global_scaled;
                    }
                }
                else
                {
                    if (text_style_info.size != fontSize)
                    {
                        fontSize = (int)text_style_info.size;
                    }
                }
            }
            else//--同步 - 自动尺寸 - 逻辑赋值
            {
                //--最小尺寸
                if (sync_global_font_size)
                {
                    if (!Application.isPlaying)
                    {
                        text_style_info.fit_min_global_scaled = XHud_Dashboard.get_manager().ft_font_size_multiply * text_style_info.fit_min;
                    }
                    else
                    {
                        text_style_info.fit_min_global_scaled = XHud_Manager.instance.ft_font_size_multiply * text_style_info.fit_min;
                    }

                    if (text_style_info.fit_min_global_scaled != resizeTextMinSize)
                    {
                        resizeTextMinSize = (int)text_style_info.fit_min_global_scaled;
                    }
                }
                else
                {
                    if (text_style_info.fit_min != resizeTextMinSize)
                    {
                        resizeTextMinSize = (int)text_style_info.fit_min;
                    }
                }

                //--最大尺寸
                if (sync_global_font_size)
                {
                    if (!Application.isPlaying)
                    {
                        text_style_info.fit_max_global_scaled = XHud_Dashboard.get_manager().ft_font_size_multiply * text_style_info.fit_max;
                    }
                    else
                    {
                        text_style_info.fit_max_global_scaled = XHud_Manager.instance.ft_font_size_multiply * text_style_info.fit_max;
                    }

                    if (text_style_info.fit_max_global_scaled != resizeTextMaxSize)
                    {
                        resizeTextMaxSize = (int)text_style_info.fit_max_global_scaled;
                    }
                }
                else
                {
                    if (text_style_info.fit_max != resizeTextMaxSize)
                    {
                        resizeTextMaxSize = (int)text_style_info.fit_max;
                    }
                }
            }

            //--同步 - 间距 - 行高
            if (lineSpacing != text_style_info.line_height)
            {
                lineSpacing = text_style_info.line_height;
            }

            //--同步 - 颜色_Color
            if (text_color_owner != TextColorOwner.Tween)
            {
                if (color != text_style_info.font_color)
                {
                    color = text_style_info.font_color;
                }
            }

            //--同步 - 射线检测
            if (raycastTarget != text_style_info.raycast)
            {
                raycastTarget = text_style_info.raycast;
                //SetRaycastDirty();
            }

            //--同步 - 遮罩
            if (maskable != text_style_info.maskable)
            {
                maskable = text_style_info.maskable;
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
            if (sync_global_font_size == state)
                return;
            sync_global_font_size = state;
        }
        /// <summary>
        /// 设置字体模板样式名称。
        /// </summary>
        /// <param name="name">样式名称</param>
        public void txt_Set_StyleName(string name)
        {
            style_name = name;
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